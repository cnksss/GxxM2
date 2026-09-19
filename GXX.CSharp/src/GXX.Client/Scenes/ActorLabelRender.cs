using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>RC_PEACENPC（Grobal2.pas 196：= 15，攻击NPC；DrawActorLabel 血条判定用）。</summary>
public static class ActorLabelConsts
{
    public const int RC_PLAYOBJECT = 0;
    public const int RC_HEROOBJECT = 1;
    public const int RC_MERCHANT = 50;      // = RC_ANIMAL（Grobal2.pas 206）
    public const int RC_PEACENPC = 15;      // Grobal2.pas 196

    // Grobal2.IsMonster 判定所需的怪物种族（Grobal2.pas 190-206）
    public const int RC_NPC = 10;
    public const int RC_GUARD = 11;
    public const int RC_GUARD2 = 12;
    public const int RC_BOX = 30;
    public const int RC_MOONOBJECT = 99;    // Grobal2.pas 195
    public const int RC_ANIMAL = 50;
    public const int RC_MONSTER = 80;
    public const int RC_ARCHERGUARD = 112;
    public const int RC_TRUCKOBJECT = 128;
    public const int RC_MOVE_ARCHERGUARD = 142;
    public const int RC_PLAYMOSTER = 150;
    public const int RC_BOX2 = 159;

    /// <summary>ShowName 内的 _OFFSET 常量（9611）。</summary>
    public const int NameOffset = 2;

    /// <summary>每行喊话行高（ShowSay 9464：I * 14）。</summary>
    public const int SayLineStep = 14;

    /// <summary>喊话整体上移基数（ShowSay 9464：m_nSayLineCount * 16）。</summary>
    public const int SayBlockStep = 16;

    /// <summary>封号存在时喊话再上移（ShowSay 9452）。</summary>
    public const int SayFengHaoExtra = 18;
}

/// <summary>
/// g_ClientConfig 中 Actor 名字/喊话/数字显血布局所需的偏移与开关（headless 注入）。
/// 字段名与 Delphi 侧一致，便于逐条对照。
/// </summary>
public sealed class ActorLabelConfig
{
    // HP 条偏移族
    public int nHumHPBarOffsetX;
    public int nHumHPBarOffsetY;
    public int nNpcHPBarOffsetX;
    public int nNpcHPBarOffsetY;
    public int nMonHPBarOffsetX;
    public int nMonHPBarOffsetY;

    // 名字偏移族
    public int nHumNameOffsetX;
    public int nHumNameOffsetY;
    public int nNpcNameOffsetX;
    public int nNpcNameOffsetY;
    public int nMonNameOffsetX;
    public int nMonNameOffsetY;

    // 显示开关
    public bool boShopHeadPic;

    /// <summary>g_ConfigDlg.ConfigCheckeds[ckShowNumberLable]。</summary>
    public bool ckShowNumberLable;

    /// <summary>g_ConfigDlg.ConfigCheckeds[ckShowJobAndLevel]。</summary>
    public bool ckShowJobAndLevel;

    /// <summary>g_ConfigDlg.ConfigCheckeds[ckShowHPLabel]。</summary>
    public bool ckShowHPLabel;

    /// <summary>g_ClientConfig.btMerchant273NameColor（雕像 NPC 名字取色）。</summary>
    public int btMerchant273NameColor;

    /// <summary>g_CurrentFontHeight（封号横向排版用）。</summary>
    public int CurrentFontHeight;
}

/// <summary>文本/图像产物的 headless 镜像（TTexture/TImageIndexs 的尺寸 + 原点）。</summary>
public sealed class LabelSurface
{
    public int Width;
    public int Height;
    public int OriginX;
    public int OriginY;

    /// <summary>ImageIndexs 是否非空（Length(...ImageIndexs) &gt; 0 的等效位）。</summary>
    public bool HasImage;

    public static LabelSurface Of(int width, int height, bool hasImage = true)
        => new() { Width = width, Height = height, HasImage = hasImage };
}

/// <summary>一次文本绘制（CurrentFont.TextOut / GameCanvas.DrawColor 的 headless 产物）。</summary>
public sealed record LabelDrawOp(int X, int Y, int Color, string Kind);

/// <summary>
/// Actor.pas 名字/喊话/称号/数字显血 的排版层（批次J75）——纯几何与配色计算，
/// 与真实字体/画布无关，可逐分支 1:1 移植：
/// ShowSay（9400-9478）、ShowName（9609-9724）、ShowShopName（9726-9789）、
/// ShowNumberLable（9793-9845）。
/// 依赖注入：g_ClientConfig 偏移（ActorLabelConfig）、g_NewopUI170TextureArray[0] 尺寸、
/// g_MyTargetList 名单、CurrentFont 是否存在。
/// </summary>
public partial class TActorCore
{
    // ===================== 布局所需字段 =====================

    /// <summary>m_nSayX / m_nSayY（角色头顶锚点，LoadSurface 内计算）。</summary>
    public int m_nSayX, m_nSayY;

    /// <summary>m_boShopStall 见 PlaySceneActors.cs；m_btHorse 见 PlaySceneActors.cs（int 型）。</summary>

    /// <summary>m_HearMsgColor（喊话正常颜色）。</summary>
    public int m_HearMsgColor = 0xFFFFFF;

    /// <summary>m_dwSayTime（喊话起始时刻；4 秒内显示）。</summary>
    public uint m_dwSayTime;

    /// <summary>m_SayingArr（喊话行；Delphi array[0..MAXSAY-1]，固定 5 槽）。</summary>
    public readonly List<LabelSurface> SayingArr = new();
    public readonly List<string> SayingText = new();

    /// <summary>按 MAXSAY 补齐槽位（Delphi 固定数组语义；Say 前调用）。</summary>
    public void EnsureSayingSlots(int count = ActorSay.MaxSay)
    {
        while (SayingArr.Count < count)
            SayingArr.Add(new LabelSurface());
        while (SayingText.Count < count)
            SayingText.Add("");
    }

    /// <summary>m_nSayLineCount（实际绘制行数）。</summary>
    public int m_nSayLineCount;

    /// <summary>m_NameTextSurface（名字纹理；null 表示未装载）。</summary>
    public LabelSurface? m_NameTextSurface;

    /// <summary>m_ShopNameImageInfo（摆摊店名纹理）。</summary>
    public LabelSurface? m_ShopNameImageInfo;

    /// <summary>m_NumberLableImageInfo（数字显血纹理）。</summary>
    public LabelSurface? m_NumberLableImageInfo;

    /// <summary>m_FengHaoEffectSurface（封号特效纹理；null = 无）。</summary>
    public LabelSurface? m_FengHaoEffectSurface;

    /// <summary>m_FengHaoImageInfo（封号文字纹理数组；空 = 无封号文字）。</summary>
    public readonly List<LabelSurface> FengHaoImageInfo = new();

    /// <summary>m_nActiveFengHaoColor / m_sUserName / m_nNameColor / m_btHorse / m_boShopStall
    /// 均已存在于 PlaySceneActors.cs 与 ActorMessages.cs（本批次直接复用，不重复声明）。</summary>

    /// <summary>m_dwShowShopNameTimeTick / m_ShowNumberLableTimeTick（绘制打点）。</summary>
    public uint m_dwShowShopNameTimeTick;
    public uint m_ShowNumberLableTimeTick;

    /// <summary>是否为 TStatuaryNpcActor（ShowName 取 btMerchant273NameColor）。</summary>
    public bool IsStatuaryNpcActor;

    /// <summary>是否为 THumActor（ShowName 查 g_MyTargetList）。</summary>
    public bool IsHumActor;

    /// <summary>m_sUserName（g_MyTargetList 名单匹配键）见 PlaySceneActors.cs。</summary>

    /// <summary>g_MyTargetList（目标名单，命中 → clLime）。</summary>
    public static readonly HashSet<string> MyTargetList = new(StringComparer.Ordinal);

    /// <summary>g_NewopUI170TextureArray[0] 尺寸（null = 未装载）。</summary>
    public static LabelSurface? NewopUI170Texture0;

    /// <summary>g_ClientConfig 布局配置。</summary>
    public static ActorLabelConfig LabelConfig = new();

    /// <summary>CurrentFont 是否存在（null 时全部文本绘制跳过）。</summary>
    public static Func<bool> CurrentFontAvailableFn = () => true;

    /// <summary>颜色常量（Delphi clBlack / clGray / clLime 的 BGR 值）。</summary>
    public const int clBlack = 0x000000;
    public const int clGray = 0x808080;
    public const int clLime = 0x00FF00;

    /// <summary>g_WNewopUIImages.Grays[102]（摆摊店名底图；null = 无）。</summary>
    public static LabelSurface? ShopNameBackgroundImage;

    // ===================== 按种族取偏移（原文三处同构 switch） =====================

    /// <summary>HP 条偏移（9637-9648 / 9420-9431 / 9805-9816 同一形态）。</summary>
    public (int X, int Y) HpBarOffset()
    {
        var cfg = LabelConfig;
        if (m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
            return (cfg.nHumHPBarOffsetX, cfg.nHumHPBarOffsetY);
        if (m_btRace == ActorLabelConsts.RC_MERCHANT)
            return (cfg.nNpcHPBarOffsetX, cfg.nNpcHPBarOffsetY);
        return (cfg.nMonHPBarOffsetX, cfg.nMonHPBarOffsetY);
    }

    /// <summary>名字偏移（9683-9694 / 9737-9748 同一形态）。</summary>
    public (int X, int Y) NameOffsetPair()
    {
        var cfg = LabelConfig;
        if (m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
            return (cfg.nHumNameOffsetX, cfg.nHumNameOffsetY);
        if (m_btRace == ActorLabelConsts.RC_MERCHANT)
            return (cfg.nNpcNameOffsetX, cfg.nNpcNameOffsetY);
        return (cfg.nMonNameOffsetX, cfg.nMonNameOffsetY);
    }

    // ===================== ShowSay（9400-9478） =====================

    /// <summary>
    /// ShowSay 1:1（9400-9478）：首行文本非空且 4 秒内且 m_boCanDraw 才绘制；
    /// 超时则清空首行文本与纹理（原文只清 [0]）。返回全部文本绘制指令。
    /// </summary>
    public List<LabelDrawOp> PlanShowSay(uint now)
    {
        var ops = new List<LabelDrawOp>();
        if (SayingArr.Count == 0 || SayingText.Count == 0 || SayingText[0].Length == 0)
            return ops;

        if (now - m_dwSayTime < 4 * 1000)
        {
            if (!m_boCanDraw)
                return ops;

            var (hpX, hpY) = HpBarOffset();

            int hpOffsetY;
            if (NewopUI170Texture0 != null)
            {
                if (LabelConfig.ckShowNumberLable || LabelConfig.ckShowJobAndLevel)
                    hpOffsetY = NewopUI170Texture0.Height * 2 + 14;
                else if (LabelConfig.ckShowHPLabel)
                    hpOffsetY = NewopUI170Texture0.Height * 2 + 2;
                else
                    hpOffsetY = 2;
            }
            else
            {
                // 原文此处只判 ckShowNumberLable（不判 ckShowJobAndLevel）——逐字保留
                if (LabelConfig.ckShowNumberLable)
                    hpOffsetY = 18;
                else if (LabelConfig.ckShowHPLabel)
                    hpOffsetY = 6;
                else
                    hpOffsetY = 2;
            }

            // 称号显示时坐标再上移
            if (CurrentFontAvailableFn() && (m_FengHaoEffectSurface != null || FengHaoImageInfo.Count > 0))
                hpOffsetY += ActorLabelConsts.SayFengHaoExtra;

            // 原文 DrawSay 内部再判一次 CurrentFont <> nil，为 nil 时整段不绘制
            if (!CurrentFontAvailableFn())
                return ops;

            for (int i = 0; i < m_nSayLineCount; i++)
            {
                if (i >= SayingArr.Count)
                    break;
                var line = SayingArr[i];
                if (!line.HasImage)
                    continue;

                int color = m_boDeath ? clGray : m_HearMsgColor;

                int x = m_nSayX - line.Width / 2 + hpX;
                int y = m_nSayY - hpOffsetY + hpY - (m_nSayLineCount * ActorLabelConsts.SayBlockStep)
                        + i * ActorLabelConsts.SayLineStep;

                // DrawSay 内部 5 次 TextOut（四向描边 + 本色）
                ops.Add(new LabelDrawOp(x - 1, y, clBlack, "SayOutline"));
                ops.Add(new LabelDrawOp(x + 1, y, clBlack, "SayOutline"));
                ops.Add(new LabelDrawOp(x, y - 1, clBlack, "SayOutline"));
                ops.Add(new LabelDrawOp(x, y + 1, clBlack, "SayOutline"));
                ops.Add(new LabelDrawOp(x, y, color, "SayText"));
            }
        }
        else
        {
            // 原文只重置 [0]（其余行保留——保持原样）
            SayingText[0] = "";
            if (SayingArr.Count > 0)
            {
                SayingArr[0].Width = 0;
                SayingArr[0].Height = 0;
                SayingArr[0].HasImage = false;
            }
        }

        return ops;
    }

    // ===================== ShowName（9609-9724） =====================

    /// <summary>ShowName 头部 nY 计算（9616-9635）：利率于 NewopUI170Texture0 与勾选项组合。</summary>
    public int NameBaseY()
    {
        var cfg = LabelConfig;
        if (NewopUI170Texture0 != null)
        {
            if (cfg.ckShowNumberLable || cfg.ckShowJobAndLevel)
                return m_nSayY - NewopUI170Texture0.Height * 2 - 32;
            if (cfg.ckShowHPLabel)
                return m_nSayY - NewopUI170Texture0.Height * 2 - 20;
            return m_nSayY - 19;
        }

        if (cfg.ckShowNumberLable)
            return m_nSayY - 38;
        if (cfg.ckShowHPLabel)
            return m_nSayY - 26;
        return m_nSayY - 19;
    }

    /// <summary>
    /// ShowName 1:1（9609-9724）：封号特效/文字排版 → 人名绘制（四向黑描边 + 本色）。
    /// 名字颜色：雕像 NPC 取 btMerchant273NameColor、THumActor 命中 g_MyTargetList 取 clLime、否则 m_nNameColor。
    /// </summary>
    public List<LabelDrawOp> PlanShowName()
    {
        var ops = new List<LabelDrawOp>();
        const int offset = ActorLabelConsts.NameOffset;

        int nX = m_nSayX;
        int nY = NameBaseY();

        var (hpX, hpY) = HpBarOffset();

        // ---- 封号（9651-9681） ----
        if (!m_boShopStall)
        {
            if (CurrentFontAvailableFn())
            {
                if (m_FengHaoEffectSurface != null)
                {
                    nY = nY + 16 - m_FengHaoEffectSurface.Height + hpY;

                    if (FengHaoImageInfo.Count == 0)
                    {
                        ops.Add(new LabelDrawOp(
                            nX - m_FengHaoEffectSurface.Width / 2 + hpX, nY, 0, "FengHaoEffect"));
                    }
                    else
                    {
                        var fh = FengHaoImageInfo[0];
                        nX = nX - (fh.Width + offset + m_FengHaoEffectSurface.Width) / 2;
                        ops.Add(new LabelDrawOp(
                            nX + hpX,
                            nY - (m_FengHaoEffectSurface.Height - LabelConfig.CurrentFontHeight) / 2,
                            0, "FengHaoEffect"));

                        nX = nX + m_FengHaoEffectSurface.Width + offset + hpX;
                        AddOutlineText(ops, nX, nY, m_nActiveFengHaoColor, "FengHaoText");
                    }
                }
                else if (FengHaoImageInfo.Count > 0)
                {
                    var fh = FengHaoImageInfo[0];
                    nX = nX - fh.Width / 2 + hpX;
                    AddOutlineText(ops, nX, nY, m_nActiveFengHaoColor, "FengHaoText");
                }
            }
        }

        // ---- 人名（9696-9723） ----
        var (nameX, nameY) = NameOffsetPair();

        nX = m_nSayX;
        nY = m_btHorse == 0 ? m_nSayY + 30 : m_nSayY + 50;

        nX += nameX;
        nY += nameY;

        if (m_NameTextSurface != null && m_boCanDraw)
        {
            int nColor = m_nNameColor;

            if (IsStatuaryNpcActor)
                nColor = GetRgb(LabelConfig.btMerchant273NameColor);
            else if (IsHumActor)
            {
                if (MyTargetList.Contains(m_sUserName))
                    nColor = clLime;
            }

            int half = m_NameTextSurface.Width / 2;
            ops.Add(new LabelDrawOp(nX - 1 - half, nY, clBlack, "NameOutline"));
            ops.Add(new LabelDrawOp(nX + 1 - half, nY, clBlack, "NameOutline"));
            ops.Add(new LabelDrawOp(nX - half, nY - 1, clBlack, "NameOutline"));
            ops.Add(new LabelDrawOp(nX - half, nY + 1, clBlack, "NameOutline"));
            ops.Add(new LabelDrawOp(nX - half, nY, nColor, "NameText"));
        }

        return ops;
    }

    /// <summary>DrawSay/名字共用的四向描边 + 本色（原文 5 次 TextOut）。</summary>
    private static void AddOutlineText(List<LabelDrawOp> ops, int x, int y, int color, string kind)
    {
        ops.Add(new LabelDrawOp(x - 1, y, clBlack, kind + "Outline"));
        ops.Add(new LabelDrawOp(x + 1, y, clBlack, kind + "Outline"));
        ops.Add(new LabelDrawOp(x, y - 1, clBlack, kind + "Outline"));
        ops.Add(new LabelDrawOp(x, y + 1, clBlack, kind + "Outline"));
        ops.Add(new LabelDrawOp(x, y, color, kind + "Text"));
    }

    /// <summary>GetRGB（HUtil）：颜色分量重排的 headless 占位（测试可覆写）。</summary>
    public static Func<int, int> GetRgbFn = c => c;
    private static int GetRgb(int c) => GetRgbFn(c);

    // ===================== ShowShopName（9726-9789） =====================

    /// <summary>
    /// ShowShopName 1:1（9726-9789）：摆摊且有店名纹理且有字体才绘制；
    /// nY 依 boShopHeadPic 与底图存在性三分支；底图 StretchDraw 矩形按高度差居中。
    /// </summary>
    public List<LabelDrawOp> PlanShowShopName(uint now)
    {
        var ops = new List<LabelDrawOp>();
        if (!m_boShopStall || m_ShopNameImageInfo == null || m_ShopNameImageInfo.Width <= 0)
            return ops;
        if (!CurrentFontAvailableFn())
            return ops;

        m_dwShowShopNameTimeTick = now;
        if (!m_boCanDraw)
            return ops;

        var (nameX, nameY) = NameOffsetPair();
        var bg = ShopNameBackgroundImage;

        int nY;
        if (!LabelConfig.boShopHeadPic)
            nY = m_nSayY - 25;
        else
            nY = bg != null ? m_nSayY - 43 : m_nSayY - 40;

        int nX = m_nSayX - m_ShopNameImageInfo.Width / 2;
        nX += nameX;
        nY += nameY;

        if (bg != null)
        {
            int tempN = (bg.Height - m_ShopNameImageInfo.Height) / 2;
            int left = nX - 1 - tempN;
            int top = nY - 1 - tempN;
            int right = left + m_ShopNameImageInfo.Width + 2 + tempN * 2;
            int bottom = top + bg.Height;
            ops.Add(new LabelDrawOp(left, top, right, $"ShopBg:{bottom}"));
        }

        int color = IsMySelf || IsFocused ? GetRgb(250) : 0x0086C2DF;
        AddOutlineText(ops, nX, nY, color, "ShopName");
        return ops;
    }

    /// <summary>Self = g_FocusCret（ShowShopName 9783）。</summary>
    public bool IsFocused;

    // ===================== ShowNumberLable（9793-9845） =====================

    /// <summary>是否为 TCustomActor（ShowNumberLable 自定义怪 HP 偏移分支）。</summary>
    public bool IsCustomActor;

    /// <summary>TCustomActor.Config.BaseConfig 的 HP 偏移四元组。</summary>
    public int CustomActorHpOffsetX, CustomActorHpOffsetY;
    public int CustomActorHpTextOffsetX, CustomActorHpTextOffsetY;

    /// <summary>
    /// ShowNumberLable 1:1（9793-9845）：摆摊直接退出；有数字纹理且字体可用才绘制；
    /// 打点后按种族取 HP 偏移；自定义怪（m_nChangeAppr &lt; 0 且 TCustomActor）额外叠加
    /// BaseConfig.HPOffsetX+HPTextOffsetX / HPOffsetY+HPTextOffsetY；
    /// nY 依 NewopUI170Texture0 是否存在二分；末次本色恒 clWhite。
    /// </summary>
    public List<LabelDrawOp> PlanShowNumberLable(uint now)
    {
        var ops = new List<LabelDrawOp>();
        // 摆摊屏蔽数字显血
        if (m_boShopStall)
            return ops;
        if (m_NumberLableImageInfo == null || m_NumberLableImageInfo.Width <= 0)
            return ops;
        if (!CurrentFontAvailableFn())
            return ops;

        m_ShowNumberLableTimeTick = now;
        if (!m_boCanDraw)
            return ops;

        var (hpX, hpY) = HpBarOffset();

        // 自定义怪 HP 文字偏移（原文 wx：注释掉的 HPStartIndex >= 0 判断保留为恒真）
        int oX = 0;
        int oY = 0;
        if (m_nChangeAppr < 0 && IsCustomActor)
        {
            oX = CustomActorHpOffsetX + CustomActorHpTextOffsetX;
            oY = CustomActorHpOffsetY + CustomActorHpTextOffsetY;
        }

        int nX = m_nSayX - m_NumberLableImageInfo.Width / 2 + oX + hpX;

        int nY;
        if (NewopUI170Texture0 != null)
            nY = m_nSayY - (NewopUI170Texture0.Height * 2 + 4) - 12 + oY + hpY;
        else
            nY = m_nSayY - 22 + oY + hpY;

        AddOutlineText(ops, nX, nY, 0xFFFFFF, "NumberLable");
        return ops;
    }
}
