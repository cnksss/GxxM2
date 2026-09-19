using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>Actor.pas 58 TNumberType（飘血类型）。</summary>
public enum TNumberType
{
    tHP = 0,
    tMP = 1,
    tGreen = 2,
    tMiss = 3,
    tTextHP = 4,
    tBlastHP = 5,
    tFatalBlow1 = 6,
    tFatalBlow2 = 7,
    tFatalBlow3 = 8,
    tFatalBlow4 = 9,
}

/// <summary>Actor.pas 65 TNumberDrawStyle（飘血绘制样式）。</summary>
public enum TNumberDrawStyle
{
    /// <summary>无效果，退场条件 |nOffsetY - cfgY| ≥ 50。</summary>
    ndsNormal = 0,

    /// <summary>快速退场，退场高度 |nOffsetY| ≥ 20。</summary>
    ndsFastExit = 1,

    /// <summary>移动渐隐退场，边上升边消隐，退场条件 |nOffsetY - cfgY| ≥ 50。</summary>
    ndsMovingFadeOut = 2,

    /// <summary>停留渐隐退场，升至固定位置后渐隐，退场条件 byAlpha ≤ 24。</summary>
    ndsStayFadeOut = 3,
}

/// <summary>
/// Actor.pas 80-97 THealthNumber 见 ActorData.cs（本批次仅补 ImageIndexs 字段）。
/// 其 nNmType/nDrawStyle 以 int 承载，此处提供强类型访问器。
/// </summary>
public static class HealthNumberExt
{
    public static TNumberType TypeOf(this THealthNumber hn) => (TNumberType)hn.nNmType;

    public static TNumberDrawStyle StyleOf(this THealthNumber hn) => (TNumberDrawStyle)hn.nDrawStyle;
}

/// <summary>飘血数字的一次绘制（GameCanvas.DrawAlpha 的 headless 产物）。</summary>
public sealed record HealthNumberDrawOp(int X, int Y, int ImageIndex, int TexWidth, byte Alpha);

/// <summary>取图接缝（g_WNewopUIImages 与 MShare.GetEffectImageListTexture 的统一入口）。</summary>
public interface IHealthNumberImageSource
{
    /// <summary>g_WNewopUIImages.Images[index]（nResID &lt; 0 时用）。</summary>
    LabelSurface? GetNewopUIImage(int index);

    /// <summary>MShare.GetEffectImageListTexture(resId, index)（nResID ≥ 0 时用）。</summary>
    LabelSurface? GetEffectImageListTexture(int resId, int index);
}

/// <summary>
/// Actor.pas 飘血（伤害数字）系统（批次J80）——CheckLoadHealthNumber(9212-9305)、
/// LoadHealthNumber(9035-9203)、ShowHealthNumber(8667-8749)、ClearHealthNumber(9205-9210)
/// 与 CheckLoadNumberLable(9307-9384)。
/// 全部为纯数值/索引计算，与真实画布无关。
/// </summary>
public partial class TActorCore
{
    /// <summary>Actor.pas 75-76：各类型在 m_HealthNumberArray 中的起始位置。</summary>
    public static readonly int[] NumberTypeIndexs = { 0, 1, 2, 3, 4, 14, 24, 34, 44, 54 };

    /// <summary>Actor.pas 76：各类型在数组中的槽位数量。</summary>
    public static readonly int[] NumberTypeCounts = { 1, 1, 1, 1, 10, 10, 10, 10, 10, 10 };

    /// <summary>THealthNumberArray = array[0..64]（97）。</summary>
    public const int HealthNumberArraySize = 65;

    /// <summary>m_HealthNumberArray。</summary>
    public readonly List<THealthNumber> m_HealthNumberArray = new();

    /// <summary>m_boShowHealthNumber（9300-9304 的开关缓存）。</summary>
    public bool m_boShowHealthNumber;

    /// <summary>m_dwDeathTick 见 ActorMessages.cs（本批次直接复用）。</summary>

    /// <summary>m_OAbil（上一轮 m_Abil 快照；CheckLoadNumberLable 差异检测用）。</summary>
    public GXX.Core.Protocol.TAbility m_OAbil;

    /// <summary>g_boMapHumAndHeroPercentHP（别人/英雄按百分比显示血量）。</summary>
    public static bool g_boMapHumAndHeroPercentHP;

    // ---- g_ClientConfig ----
    public static bool boHealthNumberText;
    public static bool boShowMoveLable;
    public static bool boShowNumberLable;
    public static bool boHumStruckShowNumber;
    public static bool boMonStruckShowNumber;
    public static bool boShowHPUnit;
    public static bool boShowJobAndLevel;
    public static int nHealthNumberMoveSpeed = 100;
    public static int nHealthNumberOffsetX;
    public static int nHealthNumberOffsetY;

    // ---- g_ConfigDlg.ConfigCheckeds ----
    public static bool ckShowMoveLable;
    public static bool ckShowNumberLable;
    public static bool ckShowHPUnit;
    public static bool ckShowJobAndLevel;

    // ---- g_ConfigClient（自定义飘血偏移） ----
    public static int nHealthNumberSelfOffset;
    public static int nHealthNumberHumOffset;
    public static bool boHealthNumberSeparate;

    /// <summary>PlugInEnabled（原文为全局；TPlaySceneCore 另有同名，此处独立承载 Actor 侧用途）。</summary>
    public static bool PlugInEnabled = true;

    /// <summary>取图接缝。</summary>
    public static IHealthNumberImageSource? HealthNumberImages;

    /// <summary>TimeGetTime（与 SceneTime 接缝区分，飘血用真实 tick）。</summary>
    public static Func<uint> HealthNumberTickFn = () => SceneTime.TickNow();

    /// <summary>CheckLoadHealthNumber 中的 timeGetTime（Win32 版，独立于 TimeGetTime）。</summary>
    public static Func<uint> CheckLoadTickFn = () => SceneTime.TickNow();

    /// <summary>HpAddUnit（血量单位换算；原文 HUtil32）。</summary>
    public static Func<uint, string> HpAddUnitFn = v => v.ToString();

    /// <summary>g_MySelf / g_MyHero（CheckLoadNumberLable 的自身判定）。</summary>
    public TActor? MySelfRef2;
    public TActor? MyHeroRef2;

    /// <summary>THealthNumberArray 初始化（按 NumberTypeCounts 布局，原文为定长数组）。</summary>
    public void EnsureHealthNumberArray()
    {
        if (m_HealthNumberArray.Count > 0)
            return;
        for (int i = 0; i < HealthNumberArraySize; i++)
            m_HealthNumberArray.Add(new THealthNumber());
        for (int i = 0; i < HealthNumberArraySize; i++)
        {
            m_HealthNumberArray[i].nResID = -1;
            m_HealthNumberArray[i].nResStartIdx = -1;
        }
    }

    /// <summary>ClearHealthNumber 1:1（9205-9210）：清宽、清索引、清数字串（**不动 nOffsetX/Y 与 alpha**）。</summary>
    public static void ClearHealthNumber(THealthNumber hn)
    {
        hn.nWidth = 0;
        hn.ImageIndexs.Clear();
        hn.sNumber = "";
    }

    // ===================== CheckLoadHealthNumber（9212-9305） =====================

    /// <summary>
    /// CheckLoadHealthNumber 1:1（9212-9305）：
    /// 画布未就绪直接 Exit(False) → 总开关 = PlugInEnabled 且 boShowMoveLable 且 ckShowMoveLable。
    /// 开关为真时：遍历全部槽，对 sNumber 非空的槽，若
    /// `dwCurTick - dwUpdateHPTick >= Cardinal(nHealthNumberMoveSpeed)` 则更新 tick 并按
    /// nDrawStyle 四分支推进偏移/透明度（可能 ClearHealthNumber）；
    /// 其后**独立地**判 `nWidth <= 0` → Result := True（需要重载贴图）。
    /// 末尾开关变化检测：与 m_boShowHealthNumber 不同则写回，且**开启时** Result := True。
    /// </summary>
    public bool CheckLoadHealthNumber()
    {
        EnsureHealthNumberArray();
        bool result = false;

        if (!CanvasReadyFn())
            return result;

        bool boShowHealthNumber = PlugInEnabled && boShowMoveLable && ckShowMoveLable;

        if (boShowHealthNumber)
        {
            uint dwCurTick = CheckLoadTickFn();

            for (int i = 0; i < m_HealthNumberArray.Count; i++)
            {
                var hn = m_HealthNumberArray[i];
                if (hn.sNumber == "")
                    continue;

                if (dwCurTick - hn.dwUpdateHPTick >= (uint)nHealthNumberMoveSpeed)
                {
                    hn.dwUpdateHPTick = dwCurTick;
                    switch (hn.StyleOf())
                    {
                        case TNumberDrawStyle.ndsNormal:
                            hn.nOffsetX++;
                            hn.nOffsetY--;
                            if (Math.Abs(hn.nOffsetY - nHealthNumberOffsetY) >= 50)
                                ClearHealthNumber(hn);
                            break;

                        case TNumberDrawStyle.ndsFastExit:
                            hn.nOffsetY--;
                            if (Math.Abs(hn.nOffsetY) >= 20)
                                ClearHealthNumber(hn);
                            break;

                        case TNumberDrawStyle.ndsMovingFadeOut:
                            hn.nOffsetX += 2;
                            hn.nOffsetY -= 2;
                            hn.byAlpha = (byte)Math.Max(0,
                                255 - Math.Abs(hn.nOffsetX - nHealthNumberOffsetX) * 3);
                            if (Math.Abs(hn.nOffsetY - nHealthNumberOffsetY) >= 50)
                                ClearHealthNumber(hn);
                            break;

                        case TNumberDrawStyle.ndsStayFadeOut:
                            if (Math.Abs(hn.nOffsetY - nHealthNumberOffsetY) >= 30)
                            {
                                if (hn.byAlpha >= 24)
                                    hn.byAlpha = (byte)(hn.byAlpha - 24);
                                else
                                    hn.byAlpha = 0;

                                if (hn.byAlpha <= 24)
                                    ClearHealthNumber(hn);
                            }
                            else
                            {
                                hn.nOffsetX += 1;
                                hn.nOffsetY -= 1;
                            }
                            break;

                        default:
                            // 原文无显式 default 标签，此 else 分支即「其余样式」
                            hn.nOffsetX += 2;
                            hn.nOffsetY -= 2;
                            if (Math.Abs(hn.nOffsetY - nHealthNumberOffsetY) >= 50)
                                ClearHealthNumber(hn);
                            break;
                    }
                }

                if (hn.nWidth <= 0)
                    result = true;
            }
        }

        if (m_boShowHealthNumber != boShowHealthNumber)
        {
            m_boShowHealthNumber = boShowHealthNumber;
            if (m_boShowHealthNumber)
                result = true;
        }

        return result;
    }

    // ===================== LoadHealthNumber（9035-9203） =====================

    /// <summary>
    /// LoadHealthNumber 1:1（9035-9203）：按 nNmType 解析数字贴图索引序列并累计宽高。
    /// nOffsetIndex 解析：nResID &lt; 0 时按类型取固定基址（tHP=50, tMP=70, tGreen=90,
    /// tMiss=204, tTextHP=500, tBlastHP=520, tFatalBlow1..4=1670/1690/1710/1730），
    /// 否则取 nResStartIdx；随后若 boHealthNumberSeparate 且 nResID &lt; 0，再按
    /// 自己/他人/怪物三支覆写（自己 tHP 起 nHealthNumberSelfOffset 步进 20；
    /// 人型他人起 nHealthNumberHumOffset 步进 20；其余回落 nResStartIdx）。
    /// 装载条件 `nOffsetIndex >= 0 且 nWidth <= 0`。
    /// </summary>
    public void LoadHealthNumber()
    {
        EnsureHealthNumberArray();
        if (!CanvasReadyFn())
            return;

        bool boShowHealthNumber = PlugInEnabled && boShowMoveLable && ckShowMoveLable;
        if (!boShowHealthNumber)
            return;

        for (int i = 0; i < m_HealthNumberArray.Count; i++)
        {
            var hn = m_HealthNumberArray[i];
            int nOffsetIndex = -1;
            var numberType = hn.TypeOf();

            if (hn.sNumber == "")
                continue;

            if (hn.nResID < 0)
            {
                nOffsetIndex = numberType switch
                {
                    TNumberType.tHP => 50,
                    TNumberType.tMP => 70,
                    TNumberType.tGreen => 90,
                    TNumberType.tMiss => 204,
                    TNumberType.tTextHP => 500,
                    TNumberType.tBlastHP => 520,
                    TNumberType.tFatalBlow1 => 1670,
                    TNumberType.tFatalBlow2 => 1690,
                    TNumberType.tFatalBlow3 => 1710,
                    TNumberType.tFatalBlow4 => 1730,
                    _ => -1,
                };
            }
            else
            {
                nOffsetIndex = hn.nResStartIdx;
            }

            if (boHealthNumberSeparate)
            {
                if (hn.nResID < 0)
                {
                    if (ReferenceEquals(this, MySelfRef2) || ReferenceEquals(this, MyHeroRef2))
                    {
                        nOffsetIndex = numberType switch
                        {
                            TNumberType.tHP => nHealthNumberSelfOffset,
                            TNumberType.tMP => nHealthNumberSelfOffset + 20,
                            TNumberType.tGreen => nHealthNumberSelfOffset + 40,
                            TNumberType.tTextHP => nHealthNumberSelfOffset + 60,
                            TNumberType.tBlastHP => nHealthNumberSelfOffset + 80,
                            // tFatalBlow1..4 在原文中被注释掉，保持不覆写
                            _ => nOffsetIndex,
                        };
                    }
                    else if ((m_btRace == ActorLabelConsts.RC_PLAYOBJECT
                              || m_btRace == ActorLabelConsts.RC_HEROOBJECT)
                             && !m_boPlayMoster)
                    {
                        nOffsetIndex = numberType switch
                        {
                            TNumberType.tHP => nHealthNumberHumOffset,
                            TNumberType.tMP => nHealthNumberHumOffset + 20,
                            TNumberType.tGreen => nHealthNumberHumOffset + 40,
                            TNumberType.tTextHP => nHealthNumberHumOffset + 60,
                            TNumberType.tBlastHP => nHealthNumberHumOffset + 80,
                            _ => nOffsetIndex,
                        };
                    }
                }
                else
                {
                    nOffsetIndex = hn.nResStartIdx;
                }
            }

            // 9109：装载条件
            if (nOffsetIndex >= 0 && hn.nWidth <= 0)
            {
                if (hn.nResID >= 0 && hn.nResStartIdx >= 0)
                {
                    // 9110-9135：自定义飘血资源
                    int nIndex = hn.nNumber > 0 ? nOffsetIndex + 11 : nOffsetIndex + 10;
                    var tex = HealthNumberImages?.GetEffectImageListTexture(hn.nResID, nIndex);
                    if (tex != null)
                        AppendDigit(hn, nIndex, tex);

                    for (int ii = 1; ii <= hn.sNumber.Length; ii++)
                    {
                        nIndex = nOffsetIndex + DigitOf(hn.sNumber[ii - 1]);
                        tex = HealthNumberImages?.GetEffectImageListTexture(hn.nResID, nIndex);
                        if (tex != null)
                            AppendDigit(hn, nIndex, tex);
                    }

                    hn.nHeight += 2;
                }
                else if (numberType == TNumberType.tMiss)
                {
                    // 9137-9144：Miss 只画一张
                    var tex = HealthNumberImages?.GetNewopUIImage(nOffsetIndex);
                    if (tex != null)
                        AppendDigit(hn, nOffsetIndex, tex);
                }
                else
                {
                    // 9145-9194
                    hn.nHeight = 0;

                    if (numberType is TNumberType.tFatalBlow1 or TNumberType.tFatalBlow2
                        or TNumberType.tFatalBlow3 or TNumberType.tFatalBlow4)
                    {
                        int nIndex = nOffsetIndex + 11;
                        var tex = HealthNumberImages?.GetNewopUIImage(nIndex);
                        if (tex != null)
                            AppendDigit(hn, nIndex, tex);

                        if (hn.nNumber < 0)
                        {
                            nIndex = nOffsetIndex + 10;
                            tex = HealthNumberImages?.GetNewopUIImage(nIndex);
                            if (tex != null)
                                AppendDigit(hn, nIndex, tex);
                        }
                    }
                    else
                    {
                        int nIndex = hn.nNumber > 0 ? nOffsetIndex + 11 : nOffsetIndex + 10;
                        var tex = HealthNumberImages?.GetNewopUIImage(nIndex);
                        if (tex != null)
                            AppendDigit(hn, nIndex, tex);
                    }

                    for (int ii = 1; ii <= hn.sNumber.Length; ii++)
                    {
                        int nIndex = nOffsetIndex + DigitOf(hn.sNumber[ii - 1]);
                        var tex = HealthNumberImages?.GetNewopUIImage(nIndex);
                        if (tex != null)
                            AppendDigit(hn, nIndex, tex);
                    }

                    hn.nHeight += 2;
                }
            }
        }
    }

    private static void AppendDigit(THealthNumber hn, int index, LabelSurface tex)
    {
        hn.ImageIndexs.Add(index);
        hn.nWidth += tex.Width;
        hn.nHeight = Math.Max(hn.nHeight, tex.Height);
    }

    /// <summary>StrToInt(sNumber[II])（Delphi 单字符 → 数字）。</summary>
    private static int DigitOf(char c) => c - '0';

    // ===================== ShowHealthNumber（8667-8749） =====================

    /// <summary>
    /// ShowHealthNumber 1:1（8667-8749）：
    /// m_boShowHealthNumber 为真时，若 m_boCanDraw 且
    /// `(boHealthNumberText or not m_boDeath) or (boHealthNumberText and m_boDeath and
    /// tick_diff(m_dwDeathTick, TimeGetTime) <= 10000)` 则逐槽绘制：
    /// nX = m_nSayX - nWidth/2 + nOffsetX；
    /// nY = m_nSayY - 15 - nHeight + nOffsetY（ndsMovingFadeOut 且死亡时再 -35）；
    /// 依 ImageIndexs 逐张 DrawAlpha 并**累加 nX += Texture.Width**。
    /// m_boShowHealthNumber 为假时清空全部槽的宽高与索引。
    /// </summary>
    public List<HealthNumberDrawOp> ShowHealthNumber()
    {
        EnsureHealthNumberArray();
        var ops = new List<HealthNumberDrawOp>();

        if (m_boShowHealthNumber)
        {
            bool gate = m_boCanDraw
                && ((boHealthNumberText || !m_boDeath)
                    || (boHealthNumberText && m_boDeath
                        && TickDiff(m_dwDeathTick, HealthNumberTickFn()) <= 10000));

            if (gate)
            {
                for (int i = 0; i < m_HealthNumberArray.Count; i++)
                {
                    var hn = m_HealthNumberArray[i];
                    if (hn.nWidth <= 0)
                        continue;

                    int nX = m_nSayX - hn.nWidth / 2 + hn.nOffsetX;

                    int nY = m_nSayY - 15 - hn.nHeight + hn.nOffsetY;
                    if (hn.StyleOf() == TNumberDrawStyle.ndsMovingFadeOut && m_boDeath)
                        nY -= 35;

                    for (int ii = 0; ii < hn.ImageIndexs.Count; ii++)
                    {
                        int idx = hn.ImageIndexs[ii];
                        LabelSurface? tex = hn.nResID < 0
                            ? HealthNumberImages?.GetNewopUIImage(idx)
                            : HealthNumberImages?.GetEffectImageListTexture(hn.nResID, idx);

                        if (tex != null)
                        {
                            ops.Add(new HealthNumberDrawOp(nX, nY, idx, tex.Width, hn.byAlpha));
                            nX += tex.Width;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < m_HealthNumberArray.Count; i++)
            {
                var hn = m_HealthNumberArray[i];
                hn.nWidth = 0;
                hn.nHeight = 0;
                hn.ImageIndexs.Clear();
            }
        }

        return ops;
    }

    /// <summary>tick_diff（HUtil32：无符号回绕安全的差值）。</summary>
    public static uint TickDiff(uint start, uint now) => now - start;

    // ===================== CheckLoadNumberLable（9307-9384） =====================

    /// <summary>
    /// CheckLoadNumberLable 1:1（9307-9384）：生成 m_sNumberLableText 并返回是否需要重载。
    /// 先清空文本；画布未就绪 Exit(False)；PlugInEnabled 且未死亡时按人物/其他两支拼文本
    /// （人物含百分比、HP 单位、职业等级后缀）；末尾按文本/HP/MaxHP 差异或 30 秒超时决定重载。
    /// </summary>
    public bool CheckLoadNumberLable()
    {
        bool result = false;
        if (!CanvasReadyFn())
            return result;

        m_sNumberLableText = "";

        if (PlugInEnabled && !m_boDeath)
        {
            // 9317：Abil := m_Abil 值拷贝；9320-9321 的 MaxHP 夹紧被注释掉，仅保留 MaxMP
            var abil = m_Abil;
            if (abil.MaxMP < abil.MP)
                abil.MaxMP = abil.MP;

            if (m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
            {
                if ((boHumStruckShowNumber && m_boStruckShowNumber) || !boHumStruckShowNumber
                    || ReferenceEquals(this, MySelfRef2) || ReferenceEquals(this, MyHeroRef2)
                    || m_boOpenHealth)
                {
                    if (((boShowNumberLable && ckShowNumberLable) || m_boOpenHealth) && abil.MaxHP > 0)
                    {
                        if (g_boMapHumAndHeroPercentHP
                            && ((!ReferenceEquals(this, MySelfRef2) && !ReferenceEquals(this, MyHeroRef2))
                                || m_boOpenHealth))
                        {
                            m_sNumberLableText =
                                Math.Min(100, (int)Math.Round((double)abil.HP / abil.MaxHP * 100)) + "%";
                        }
                        else if (boShowHPUnit && ckShowHPUnit)
                        {
                            m_sNumberLableText = HpAddUnitFn(abil.HP) + "/" + HpAddUnitFn(abil.MaxHP);
                        }
                        else
                        {
                            m_sNumberLableText = abil.HP + "/" + abil.MaxHP;
                        }
                    }
                }
            }
            else
            {
                if ((boMonStruckShowNumber && m_boStruckShowNumber) || !boMonStruckShowNumber
                    || m_boOpenHealth)
                {
                    if (m_btRace != ActorLabelConsts.RC_MERCHANT && abil.MaxHP > 0
                        && ((boShowNumberLable && ckShowNumberLable) || m_boOpenHealth))
                    {
                        if (boShowHPUnit && ckShowHPUnit)
                            m_sNumberLableText = HpAddUnitFn(abil.HP) + "/" + HpAddUnitFn(abil.MaxHP);
                        else
                            m_sNumberLableText = abil.HP + "/" + abil.MaxHP;
                    }
                }
            }

            // 9362-9375：职业等级后缀
            if ((m_btRace == 0 || m_btRace == 1) && abil.MaxHP > 0 && abil.Level > 0)
            {
                if (boShowJobAndLevel && ckShowJobAndLevel)
                {
                    if (m_sNumberLableText != "")
                        m_sNumberLableText += "/";

                    m_sNumberLableText += m_btJob switch
                    {
                        0 => "Z",
                        1 => "F",
                        2 => "D",
                        3 => "C",
                        _ => "UnKnow",
                    };
                    m_sNumberLableText += abil.Level;
                }
            }
        }

        if (!string.Equals(m_sCurNumberLableText, m_sNumberLableText, StringComparison.OrdinalIgnoreCase)
            || m_Abil.HP != m_OAbil.HP || m_Abil.MaxHP != m_OAbil.MaxHP
            || HealthNumberTickFn() - m_ShowNumberLableTimeTick > 30)
        {
            m_OAbil = m_Abil;
            result = true;
        }

        return result;
    }
}
