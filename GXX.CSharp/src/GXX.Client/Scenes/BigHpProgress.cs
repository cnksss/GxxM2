using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>NearActorHintEffect.pas 12-16 TNearActorHintInfo 镜像。</summary>
public sealed class TNearActorHintInfo
{
    public int X;
    public int Y;
    public int nFriendFlag;
}

/// <summary>一次邻近提示绘制（GameCanvas.Draw + Blend_SrcColorAdd 的 headless 产物）。</summary>
public sealed record NearActorHintDrawOp(int X, int Y, int ImageIndex, int OriginX, int OriginY);

/// <summary>一次大血条文本绘制（CurrentFont.TextOut 的 headless 产物）。</summary>
public sealed record BigHpTextOp(int X, int Y, string Text, int Color, string Kind);

/// <summary>一次大血条贴图绘制（GameCanvas.Draw 的 headless 产物）。</summary>
public sealed record BigHpImageOp(int X, int Y, int ImageIndex, int SrcLeft, int SrcTop,
    int SrcRight, int SrcBottom, string Kind);

/// <summary>
/// NearActorHintEffect.pas 1:1 移植（批次J83）——TNearActorHintEffectMgr 全类。
/// 常量 FRAME_TICK_COUNT = 150；Create 预分配 200；AddActorHint 不足时每次 +50；
/// CleaerHint（原文拼写，保留）把帧计数置 **6** 而非 0。
/// </summary>
public sealed class TNearActorHintEffectMgr
{
    public const int FrameTickCount = 150;
    private const int InitialCapacity = 200;
    private const int GrowStep = 50;

    private readonly List<TNearActorHintInfo> m_arrHintInfo = new();
    private int m_nInfoCount;
    private uint m_dwHintEffectFrameCount;
    private uint m_dwLastHintEffectTick;
    private uint m_dwHintEffctFrameIndex;

    /// <summary>MyGetTickCount 接缝。</summary>
    public Func<uint> TickFn = () => SceneTime.TickNow();

    public TNearActorHintEffectMgr()
    {
        // 72-74：SetLength(m_arrHintInfo, 200)
        for (int i = 0; i < InitialCapacity; i++)
            m_arrHintInfo.Add(new TNearActorHintInfo());
    }

    /// <summary>54-63：容量不足时每次 +50（`Length < m_nInfoCount + 1`）。</summary>
    public void AddActorHint(int x, int y, int nFriendFlag)
    {
        if (m_arrHintInfo.Count < m_nInfoCount + 1)
        {
            for (int i = 0; i < GrowStep; i++)
                m_arrHintInfo.Add(new TNearActorHintInfo());
        }
        m_arrHintInfo[m_nInfoCount].X = x;
        m_arrHintInfo[m_nInfoCount].Y = y;
        m_arrHintInfo[m_nInfoCount].nFriendFlag = nFriendFlag;
        m_nInfoCount++;
    }

    /// <summary>65-69：**原文拼写 CleaerHint**；把 InfoCount 置 0 且帧计数置 6。</summary>
    public void CleaerHint()
    {
        m_nInfoCount = 0;
        m_dwHintEffectFrameCount = 6;
    }

    /// <summary>76-81：仅当 InfoCount &gt; 0 时递减。</summary>
    public void DeleteLatestHint()
    {
        if (m_nInfoCount > 0)
            m_nInfoCount--;
    }

    /// <summary>
    /// 88-106：首帧仅记 tick 并清零帧索引；其后每满 FRAME_TICK_COUNT 推进一帧，
    /// 达到 `FrameCount + 0` 时回卷 0（`+ 0` 为原文冗余写法，保留）。
    /// </summary>
    public void FrameDrive()
    {
        uint dwCurTick = TickFn();

        if (m_dwLastHintEffectTick == 0)
        {
            m_dwLastHintEffectTick = dwCurTick;
            m_dwHintEffctFrameIndex = 0;
        }
        else if (dwCurTick - m_dwLastHintEffectTick >= FrameTickCount)
        {
            m_dwLastHintEffectTick = dwCurTick;
            m_dwHintEffctFrameIndex++;
            if (m_dwHintEffctFrameIndex >= m_dwHintEffectFrameCount + 0)
                m_dwHintEffctFrameIndex = 0;
        }
    }

    /// <summary>108-111：帧索引即以 DWORD 承载的 m_dwHintEffctFrameIndex。</summary>
    public int FrameIndex => (int)m_dwHintEffctFrameIndex;

    public int InfoCount => m_nInfoCount;

    /// <summary>130-133：m_nInfoCount - 1（**空表时为 -1**）。</summary>
    public int LatestInfoIndex => m_nInfoCount - 1;

    /// <summary>44：可读写的 FrameCount。</summary>
    public uint FrameCount
    {
        get => m_dwHintEffectFrameCount;
        set => m_dwHintEffectFrameCount = value;
    }

    /// <summary>
    /// 113-128 三段取值：InfoCount = 0 → nil；nIndex ≥ 0 且 &lt; InfoCount → 该项，
    /// ≥ InfoCount → nil；nIndex &lt; 0 → **末项**（负数索引取最新项）。
    /// </summary>
    public TNearActorHintInfo? GetHintInfo(int nIndex)
    {
        if (m_nInfoCount <= 0)
            return null;

        if (nIndex >= 0)
            return nIndex < m_nInfoCount ? m_arrHintInfo[nIndex] : null;

        return m_arrHintInfo[m_nInfoCount - 1];
    }
}

/// <summary>
/// PlayScn.pas 邻近提示与大血条渲染（批次J83，无纹理 headless 化）——
/// DrawNearActorHintEffect(4003-4027) 与 DrawCurMonBigHPProgress(4208-4370)。
/// </summary>
public static class BigHpProgressRender
{
    /// <summary>4009-4010 提示起始图号。</summary>
    public const int NormalStartIndex = 144;
    public const int FriendStartIndex = 154;

    /// <summary>4223：最后一次攻击后仍显示大血条的窗口（毫秒）。</summary>
    public const int ShowWindowMs = 5000;

    /// <summary>4238/4256 背景图基址；4265 图像基址；4275/4310/4317 血条基址。</summary>
    public const int HpBgImageBase = 760;
    public const int HpImageBase = 780;
    public const int HpBarImageBase = 770;

    /// <summary>4328 数字首位图号（块数标签的图标）。</summary>
    public const int HpBlockIconIndex = 749;

    /// <summary>4331 数字字形基址（'0' → 750）。</summary>
    public const int HpBlockDigitBase = 750;

    /// <summary>
    /// DrawNearActorHintEffect 1:1（4003-4027）：逐条信息取图号
    /// （nFriendFlag ≠ 0 → 154 基址，否则 144 基址；再加管理器当前帧索引），
    /// 命中纹理则以 (X + px, Y + py) 加色混合绘制；**全部处理完后**调用 FrameDrive。
    /// </summary>
    public static List<NearActorHintDrawOp> DrawNearActorHintEffect(
        TNearActorHintEffectMgr mgr,
        Func<int, FxImage?> resolve)
    {
        var ops = new List<NearActorHintDrawOp>();

        for (int i = 0; i < mgr.InfoCount; i++)
        {
            var info = mgr.GetHintInfo(i);
            if (info == null)
                continue;

            int nStartIndex = info.nFriendFlag != 0 ? FriendStartIndex : NormalStartIndex;
            var d = resolve(nStartIndex + mgr.FrameIndex);
            if (d != null)
                ops.Add(new NearActorHintDrawOp(info.X + d.OriginX, info.Y + d.OriginY, d.ImageIndex, d.OriginX, d.OriginY));
        }

        mgr.FrameDrive();          // 4026：循环之外，即使无信息也驱动
        return ops;
    }

    /// <summary>
    /// 血条块索引计算 1:1（4292-4307）：
    /// `HPBlockSize = (MaxHP + Count - 1) div Count`（向上取整），下限 1；
    /// `HPBlockIndex = HP div HPBlockSize`，**余数非 0 时 +1，否则仅当索引 &lt; 1 时置 1**；
    /// `nPreSize = (Index - 1) * Size`；`nCurSize = HP - nPreSize`；
    /// **`Index = HPBlockSize` 时 `nCurMaxSize = MaxHP - nPreSize` 否则 `= HPBlockSize`**
    /// （原文把「块索引」与「块大小」比较，疑为笔误，逐字保留）。
    /// </summary>
    public static (uint BlockSize, uint BlockIndex, uint PreSize, uint CurSize, uint CurMaxSize)
        CalcHpBlock(uint hp, uint maxHp, uint blockCount)
    {
        uint hpBlockSize = (maxHp + blockCount - 1) / blockCount;
        if (hpBlockSize < 1)
            hpBlockSize = 1;

        uint hpBlockIndex = hp / hpBlockSize;
        if (hp % hpBlockSize != 0)
            hpBlockIndex = hpBlockIndex + 1;
        else if (hpBlockIndex < 1)
            hpBlockIndex = 1;

        uint nPreSize = (hpBlockIndex - 1) * hpBlockSize;
        uint nCurSize = hp - nPreSize;

        uint nCurMaxSize = hpBlockIndex == hpBlockSize
            ? maxHp - nPreSize
            : hpBlockSize;

        return (hpBlockSize, hpBlockIndex, nPreSize, nCurSize, nCurMaxSize);
    }

    /// <summary>
    /// 单条血条宽度（4278-4280 / 4312-4322）：
    /// `R.Right := R.Left + Round((R.Right - R.Left) / 分母 * 分子)`。
    /// 分母 ≤ 0 时原文直接沿用整宽（4312 分支不调宽度公式，4279 分支判 MaxHP &gt; 0）。
    /// </summary>
    public static int HpBarWidth(int texW, uint numerator, uint denominator)
    {
        if (denominator == 0)
            return texW;
        return (int)Math.Round(texW / (double)denominator * numerator, MidpointRounding.ToEven);
    }

    /// <summary>4362：HP 百分比文本 `Format('%d%%', [Round(HP / MaxHP * 100)])`。</summary>
    public static string HpPercentText(uint hp, uint maxHp)
        => (int)Math.Round(hp / (double)maxHp * 100, MidpointRounding.ToEven) + "%";

    /// <summary>4288/4346：血量文本 `Format('%u/%u', ...)`。</summary>
    public static string HpValueText(uint hp, uint maxHp) => hp + "/" + maxHp;

    /// <summary>4367：归属文本 `Format('归属(%s)', [name])`。</summary>
    public static string ExpHinterText(string name) => "归属(" + name + ")";

    /// <summary>4245-4250：水平对齐 1 = 居中、2 = 右对齐、其余 = 左对齐（原文无 3+ 特判）。</summary>
    public static int HpBgX(int screenWidth, int bgWidth, int hpBgX, byte btHorizAlign)
    {
        if (btHorizAlign == 1)
            return (screenWidth - bgWidth) / 2 + hpBgX;
        if (btHorizAlign == 2)
            return screenWidth - bgWidth + hpBgX;
        return hpBgX;
    }

    /// <summary>大血条一次完整绘制的产物。</summary>
    public sealed class BigHpResult
    {
        public List<BigHpImageOp> Images { get; } = new();
        public List<BigHpTextOp> Texts { get; } = new();
    }

    /// <summary>
    /// DrawCurMonBigHPProgress 1:1（4208-4370）：外层条件
    /// `(not ckHideBigHPProgress) and (now - m_nCurrMonLastAttack <= 5000)`；
    /// 内层五道退出（Actor 未找到 / 死亡或幽灵 / 未开大血条 / MaxHP = 0）。
    /// 背景宽取自 760+Index 的纹理宽（未命中则为 0）；随后按 btHorizAlign 定位 nX、nY 取 nHPBGY；
    /// 依次绘背景、图像、血条（块数 ≤ 1 走单条 + 满血文本，否则走分块 + 块数字标签
    /// + **分块模式的血量文本用 nCurSize/nCurMaxSize**），最后按四个 boShow* 绘等级/怪物名/百分比/归属。
    /// 原文背景与图像各查两次表（4237 与 4255 逻辑重复），此处合并为一次查询但保持绘制顺序。
    /// </summary>
    public static BigHpResult DrawCurMonBigHPProgress(
        bool ckHideBigHPProgress,
        uint m_nCurrMonLastAttack,
        uint now,
        bool actorFound, bool actorDeath, bool actorGhost,
        bool boShowBigHPProgress,
        uint actorHp, uint actorMaxHp, uint actorLevel,
        string actorName,
        GXX.Core.Protocol.TMonHPProgress info,
        int screenWidth,
        Func<int, FxImage?> resolveImage,
        int hpBarTexWidth)
    {
        var r = new BigHpResult();

        if (ckHideBigHPProgress)
            return r;
        if (now - m_nCurrMonLastAttack > ShowWindowMs)
            return r;
        if (!actorFound)
            return r;
        if (actorDeath || actorGhost)
            return r;
        if (!boShowBigHPProgress)
            return r;
        if (actorMaxHp == 0)
            return r;

        int nBGWidth = 0;
        if (info.nHPBGIndex >= 0 && info.nHPBGIndex < 10)
        {
            var bg = resolveImage(HpBgImageBase + info.nHPBGIndex);
            if (bg != null)
                nBGWidth = bg.Width;
        }

        int nX = HpBgX(screenWidth, nBGWidth, info.nHPBGX, info.btHorizAlign);
        int nY = info.nHPBGY;

        // 背景
        if (info.nHPBGIndex >= 0 && info.nHPBGIndex < 10)
        {
            var bg = resolveImage(HpBgImageBase + info.nHPBGIndex);
            if (bg != null)
                r.Images.Add(new BigHpImageOp(nX, nY, HpBgImageBase + info.nHPBGIndex,
                    0, 0, bg.Width, bg.Height, "Background"));
        }

        // 图像
        if (info.nImageIndex >= 0 && info.nImageIndex < 100)
        {
            int idx = HpImageBase + info.nImageIndex;
            var img = resolveImage(idx);
            if (img != null)
                r.Images.Add(new BigHpImageOp(nX + info.nImageX, nY + info.nImageY, idx,
                    0, 0, img.Width, img.Height, "Image"));
        }

        if (info.wHPBlockCount <= 1)
        {
            // 单条血条
            if (info.nHPIndex >= 0 && info.nHPIndex < 10)
            {
                int idx = HpBarImageBase + info.nHPIndex;
                var bar = resolveImage(idx);
                if (bar != null)
                {
                    int right = bar.Width;
                    if (actorMaxHp > 0)
                        right = (int)Math.Round(bar.Width / (double)actorMaxHp * actorHp,
                            MidpointRounding.ToEven);
                    r.Images.Add(new BigHpImageOp(nX + info.nHPX, nY + info.nHPY, idx,
                        0, 0, right, bar.Height, "HpBar"));
                }
            }

            if (info.boShowHPValue >= 0 && info.boShowHPValue <= 255)
            {
                r.Texts.Add(new BigHpTextOp(nX + info.nHPValueX, nY + info.nHPValueY,
                    HpValueText(actorHp, actorMaxHp), info.boShowHPValue, "HPValue"));
            }
        }
        else
        {
            var (_, hpBlockIndex, _, nCurSize, nCurMaxSize) =
                CalcHpBlock(actorHp, actorMaxHp, info.wHPBlockCount);

            if (hpBlockIndex > 1)
            {
                int idx = HpBarImageBase + (int)((hpBlockIndex - 2 + 10) % 10);
                var bar = resolveImage(idx);
                if (bar != null)
                    r.Images.Add(new BigHpImageOp(nX + info.nHPX, nY + info.nHPY, idx,
                        0, 0, bar.Width, bar.Height, "HpBarPrev"));
            }

            {
                int idx = HpBarImageBase + (int)((hpBlockIndex - 1) % 10);
                var bar = resolveImage(idx);
                if (bar != null)
                {
                    int right = bar.Width;
                    if (nCurMaxSize > 0)
                        right = (int)Math.Round(bar.Width / (double)nCurMaxSize * nCurSize,
                            MidpointRounding.ToEven);
                    r.Images.Add(new BigHpImageOp(nX + info.nHPX, nY + info.nHPY, idx,
                        0, 0, right, bar.Height, "HpBarCur"));
                }
            }

            // 块数字标签：首位图标 + 逐位字形
            string s = hpBlockIndex.ToString();
            var digits = new List<(int Idx, FxImage? Img)>();
            digits.Add((HpBlockIconIndex, resolveImage(HpBlockIconIndex)));
            foreach (char c in s)
            {
                int idx = HpBlockDigitBase + (c - '0');
                digits.Add((idx, resolveImage(idx)));
            }

            int nX2 = info.nHPBlockOffsetX;
            int nY2 = info.nHPBlockOffsetY;
            foreach (var (idx, img) in digits)
            {
                if (img != null)
                {
                    r.Images.Add(new BigHpImageOp(nX + nX2, nY + nY2, idx,
                        0, 0, img.Width, img.Height, "HpBlockDigit"));
                    nX2 += img.Width;
                }
            }

            if (info.boShowHPValue >= 0 && info.boShowHPValue <= 255)
            {
                r.Texts.Add(new BigHpTextOp(nX + info.nHPValueX, nY + info.nHPValueY,
                    HpValueText(nCurSize, nCurMaxSize), info.boShowHPValue, "HPValue"));
            }
        }

        if (info.boShowLevel >= 0 && info.boShowLevel <= 255)
        {
            r.Texts.Add(new BigHpTextOp(nX + info.nLevelX, nY + info.nLevelY,
                actorLevel.ToString(), info.boShowLevel, "Level"));
        }

        if (info.boShowMonName >= 0 && info.boShowMonName <= 255)
        {
            r.Texts.Add(new BigHpTextOp(nX + info.nMonNameX, nY + info.nMonNameY,
                actorName, info.boShowMonName, "MonName"));
        }

        if (info.boShowHPPercent >= 0 && info.boShowHPPercent <= 255)
        {
            r.Texts.Add(new BigHpTextOp(nX + info.nHPPercentX, nY + info.nHPPercentY,
                HpPercentText(actorHp, actorMaxHp), info.boShowHPPercent, "HPPercent"));
        }

        if (info.boShowExpHinter >= 0 && info.boShowExpHinter <= 255)
        {
            r.Texts.Add(new BigHpTextOp(nX + info.nExpHinterX, nY + info.nExpHinterY,
                ExpHinterText(info.ExpHinterName), info.boShowExpHinter, "ExpHinter"));
        }

        return r;
    }
}
