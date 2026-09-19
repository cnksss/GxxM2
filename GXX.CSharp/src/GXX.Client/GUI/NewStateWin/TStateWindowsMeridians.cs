using GXX.Core.Protocol;

namespace GXX.Client.GUI.NewStateWin;

/// <summary>
/// StateWindows.pas 经脉（Meridians / 内功经络）相关的**纯逻辑**移植。
///
/// 对应用户端侧原文（Source\Client-HGE\GUI\NewStateWin\StateWindows.pas）：
///   - Initialize                    4801-4812  MeridiansImageIndexArray 初始化
///   - GetMeridianStateInfo          4814-4827  经脉状态文本（顶层函数）
///   - SetMeridiansLevel             6275-6281  设置经脉重数 → 图层索引
///   - DTrainingMeridianClick        6283-6304  修炼经络按钮 → 发 CM_SENDTRAININGMERIDIANCLICK
///   - DBotAcupointsClick            6306-6314  点击穴位   → 发 CM_SENDACUPOINTCLICK
///   - DBotAcupointsMouseMove        6316-6421  穴位悬浮提示（FunA 内嵌过程）
///   - DBotMeridiansClick            6457-6485  选择经络 → 页索引 / 按钮标题 / 状态文本
///   - DRecallDeputyHeroClick        4099-4111  召唤副将英雄 → 发 CM_HEROLOGON
///   - SetDeputyHeroJob              9843-9851  勾选副将职业
///   （英雄侧 DHero* 是同一套逻辑的镜像，见 5595/5603/5613/5706/5740/5750 行）
///
/// 接缝：g_MySelf / g_MyHero（FState.pas，另一条车道）未移植 —— 凡需要 g_MySelf.m_HumMeridians /
/// m_AbilNG.Level 的地方一律由调用方传入（见 <see cref="MeridianStatusProvider"/>）。
/// 接缝：g_AcupointLevels（MShare.pas，二维表 [0..4,0..4]）以 (page, acupoint) → 需求量 的委托传入。
/// </summary>
public static class TStateWindowsMeridians
{
    // ===================== 常量（原文 4803-4807、6286、6328-6329） =====================

    /// <summary>经脉页数（原文 MeridiansImageIndexArray: array[0..4] / AcupointArray: array[0..4,0..4]）。</summary>
    public const int MeridianPageCount = 5;

    /// <summary>每页穴位数（原文 array[0..4, 0..4] 的第二维）。</summary>
    public const int AcupointCount = 5;

    /// <summary>原文 4803-4807：MeridiansImageIndexArray[0..4] = (860, 870, 880, 890, 1180)。</summary>
    public static readonly int[] MeridiansImageIndexArray = { 860, 870, 880, 890, 1180 };

    /// <summary>
    /// 原文 6286（被注释掉的 const Arr_Names，但 6298 行仍引用它）与原文 6468-6472 的按钮标题一致：
    /// 内部索引 0..4 ↔ 冲脉 / 阴跷 / 阴维 / 任脉 / 奇经。
    /// 原文如此（StateWindows.pas:6286）：注释里的顺序是 ('冲脉','阴跷','阴维','任脉','奇经')，
    /// 而 6468-6472 的页索引 1..4 依次是 冲脉/阴跷/阴维/任脉，页索引 0（= 内部索引 4）是奇经。
    /// </summary>
    public static readonly string[] MeridianNames = { "冲脉", "阴跷", "阴维", "任脉", "奇经" };

    /// <summary>原文 6329：NeedLev = '需要内功等级%d级'。</summary>
    public const string NeedLevFormat = "需要内功等级{0}级";

    /// <summary>原文 6328：DxImageButtonName = 'DBotAcupoints%d_%d'。</summary>
    public const string AcupointButtonNameFormat = "DBotAcupoints{0}_{1}";

    /// <summary>
    /// 原文 6364-6404：按 DMeridiansPageControl.ActivePageIndex（0..4）排列的穴位名，
    /// 每组 5 个对应按钮 DBotAcupoints{MeridianIndex}_{0..4}。
    /// </summary>
    public static readonly string[][] AcupointNamesByActivePage =
    {
        new[] { "神冲穴", "二百穴", "夹脊穴", "八风穴", "涌泉穴" }, // ActivePageIndex = 0 → 内部经脉 4（奇经）
        new[] { "幽门穴", "通骨穴", "商曲穴", "四满穴", "横骨穴" }, // ActivePageIndex = 1 → 内部经脉 0（冲脉）
        new[] { "睛明穴", "盘缺穴", "交信穴", "照海穴", "然骨穴" }, // ActivePageIndex = 2 → 内部经脉 1（阴跷）
        new[] { "廉泉穴", "期门穴", "府舍穴", "冲门穴", "筑宾穴" }, // ActivePageIndex = 3 → 内部经脉 2（阴维）
        new[] { "承浆穴", "天突穴", "鸠尾穴", "气海穴", "骨曲穴" }, // ActivePageIndex = 4 → 内部经脉 3（任脉）
    };

    /// <summary>
    /// 原文 6468-6472：DBotMeridians{0..4}.Tag 与 ActivePageIndex 一一对应，DTrainingMeridian.Caption 随页变化。
    /// 索引即 ActivePageIndex。
    /// </summary>
    public static readonly string[] TrainingButtonCaptions =
    {
        "修炼穴位", // 页 0
        "修炼冲脉", // 页 1
        "修炼阴跷", // 页 2
        "修炼阴维", // 页 3
        "修炼任脉", // 页 4
    };

    // ===================== 页索引 ↔ 经脉索引（原文 6292-6293 / 6311-6312 / 6474-6475） =====================

    /// <summary>
    /// 原文三处「nPage := DMeridiansPageControl.ActivePageIndex - 1; if nPage &lt; 0 then nPage := 4;」
    /// （6292-6293 / 6311-6312 / 6474-6475，逐字相同）。
    /// 只有 ActivePageIndex = 0 会被映射为 4；负数输入（不该出现）同样落 4。
    /// </summary>
    public static int ActivePageToMeridianIndex(int activePageIndex)
    {
        int nPage = activePageIndex - 1;
        if (nPage < 0)
            nPage = 4;
        return nPage;
    }

    /// <summary>
    /// 原文 6324-6350 的 FunA：按「经脉页 → 内部经脉序号」与「穴位页内序号」拼出按钮名，
    /// 只有 Sender 名匹配的**第一个** i 会被处理（Break）。
    /// 返回 -1 表示没有按钮名匹配（原文此时 sMsg1/sMsg2 都保持空串）。
    /// </summary>
    /// <param name="senderButtonName">TDxControl(Sender).Name</param>
    /// <param name="nameIndex">原文 FunA 的 NameIndex 实参（0..4）</param>
    public static int MatchAcupointIndex(string senderButtonName, int nameIndex)
    {
        for (int i = 0; i < AcupointCount; i++)
        {
            // 原文 Format('DBotAcupoints%d_%d', [NameIndex, i]) 与 Name 比较（Delphi 字符串区分大小写）。
            if (senderButtonName == string.Format(AcupointButtonNameFormat, nameIndex, i))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 原文 6362-6408：ActivePageIndex → FunA 的 (Page, NameIndex) 实参。
    /// 注意 Page 与 NameIndex 是**反序**的（页 0 传 (4, 0)、页 1 传 (0, 1)…），原文如此。
    /// 返回 false 表示 ActivePageIndex 不在 0..4（原文 case 无 else，两个消息都保持空串）。
    /// </summary>
    public static bool ActivePageToFunAArgs(int activePageIndex, out int page, out int nameIndex)
    {
        switch (activePageIndex)
        {
            case 0: page = 4; nameIndex = 0; return true;
            case 1: page = 0; nameIndex = 1; return true;
            case 2: page = 1; nameIndex = 2; return true;
            case 3: page = 2; nameIndex = 3; return true;
            case 4: page = 3; nameIndex = 4; return true;
            default: page = 0; nameIndex = 0; return false;
        }
    }

    // ===================== GetMeridianStateInfo（原文 4814-4827） =====================

    /// <summary>原文 4816：MeridianLevelStrings:array[1..10]（1-based，索引 0 越界）。</summary>
    public static readonly string[] MeridianLevelStrings =
        { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

    /// <summary>
    /// 原文 4814-4827 的 GetMeridianStateInfo。
    /// 判据顺序（必须逐字保留）：
    ///   1) Acupoints[4] &gt; 0（第 5 个穴位已打通）才算「通」；
    ///   2) 在「通」的前提下 Level &gt; 0 显示重数，否则显示「经络已通」。
    /// 文本用 #13（CR）分隔 —— 不是换行 #13#10，也不是 '\'。
    /// </summary>
    /// <param name="acupoint4">Meridians[nPage].Acupoints[4]</param>
    /// <param name="level">Meridians[nPage].Level（1..10 有效；0 表示已通但无重数）</param>
    public static string GetMeridianStateInfo(int acupoint4, int level)
    {
        if (acupoint4 > 0)
        {
            if (level > 0)
            {
                // 原文 4820：MeridianLevelStrings[Level]（Delphi 1-based；Level > 10 会越界）。
                string levelText = (level >= 1 && level <= MeridianLevelStrings.Length)
                    ? MeridianLevelStrings[level - 1]
                    : string.Empty;   // 原文越界行为未定义（Delphi 不检查）——此处取空串并保持其余文本
                return levelText + "\r" + "重" + "\r" + "经" + "\r" + "络";
            }
            else
                return "经" + "\r" + "络" + "\r" + "已" + "\r" + "通";
        }
        else
            return "经" + "\r" + "络" + "\r" + "未" + "\r" + "通";
    }

    /// <summary>经脉状态数据源（接缝：g_MySelf.m_HumMeridians）。</summary>
    public delegate void MeridianStatusProvider(int meridianIndex, out int acupoint4, out int level);

    /// <summary>原文 6477 + 6480-6483：DBotMeridiansClick 末尾取状态文本。</summary>
    public static string GetMeridianStateInfo(int meridianIndex, MeridianStatusProvider provider)
    {
        provider(meridianIndex, out int acupoint4, out int level);
        return GetMeridianStateInfo(acupoint4, level);
    }

    // ===================== SetMeridiansLevel（原文 6275-6281 / 5595-5601） =====================

    /// <summary>
    /// 原文 6278-6279：`if (Meridian in [0..4]) and (Level in [0..5]) then
    ///   MeridiansFormArray[Meridian].Draw1.ImageIndex := MeridiansImageIndexArray[Meridian] + Level;`
    /// 越界时**什么都不做**（不抛异常、不回退）。
    /// </summary>
    /// <returns>true = 已设置；false = 参数越界，调用方应保持原样</returns>
    public static bool TrySetMeridiansLevel(int initialized, int meridian, int level, out int imageIndex)
    {
        imageIndex = 0;

        // 原文 6277：if not Initialized then Exit;
        if (initialized == 0)
            return false;

        if (meridian >= 0 && meridian <= 4 && level >= 0 && level <= 5)
        {
            imageIndex = MeridiansImageIndexArray[meridian] + level;
            return true;
        }

        return false;
    }

    // ===================== 点击事件路由（原文 6283-6304 / 6306-6314 / 4099-4111） =====================

    /// <summary>SendClientMessage(Ident, Recog, Param, Tag, Series) 的参数包（wIdent 由 Grobal2Const 给出）。</summary>
    public readonly struct ClientMessage
    {
        public readonly int Ident;   // wIdent
        public readonly int Recog;   // wRecog
        public readonly int Param;   // wParam
        public readonly int Tag;     // wTag
        public readonly int Series;  // wSeries

        public ClientMessage(int ident, int recog, int param, int tag, int series)
        {
            Ident = ident;
            Recog = recog;
            Param = param;
            Tag = tag;
            Series = series;
        }
    }

    /// <summary>
    /// 原文 6283-6304 DTrainingMeridianClick（修炼经络）：`frmMain.SendClientMessage(CM_SENDTRAININGMERIDIANCLICK, 0, nPage, 0, 0)`。
    /// 返回 -1 表示 g_MySelf = nil（原文 6291 直接 Exit，不发消息）。
    /// </summary>
    public static int TryBuildTrainingMeridianClick(int mySelfNull, int activePageIndex, out ClientMessage msg)
    {
        msg = default;

        if (mySelfNull != 0)
            return -1;

        int nPage = ActivePageToMeridianIndex(activePageIndex);
        msg = new ClientMessage(Grobal2Const.CM_SENDTRAININGMERIDIANCLICK, 0, nPage, 0, 0);
        return nPage;
    }

    /// <summary>
    /// 原文 6306-6314 DBotAcupointsClick（点击穴位）：
    /// `frmMain.SendClientMessage(CM_SENDACUPOINTCLICK, 0, nPage, TDxControl(Sender).Tag, 0)`。
    /// 返回 -1 表示 g_MySelf = nil（原文 6310 直接 Exit）。
    /// </summary>
    public static int TryBuildAcupointClick(int mySelfNull, int activePageIndex, int senderTag, out ClientMessage msg)
    {
        msg = default;

        if (mySelfNull != 0)
            return -1;

        int nPage = ActivePageToMeridianIndex(activePageIndex);
        msg = new ClientMessage(Grobal2Const.CM_SENDACUPOINTCLICK, 0, nPage, senderTag, 0);
        return nPage;
    }

    /// <summary>
    /// 原文 4099-4111 DRecallDeputyHeroClick（召唤副将英雄）：
    /// 三个单选按钮依次判 Checked，**最后一个命中者胜出**（原文用 if/else if，故只可能命中一个）；
    /// 都不勾选时 btJob 保持 0。
    /// `frmMain.SendClientMessage(CM_HEROLOGON, 1, btJob, 0, 0)`。
    /// </summary>
    /// <param name="job0Checked">DBotStateDeputyHeroJob0.Checked</param>
    /// <param name="job1Checked">DBotStateDeputyHeroJob1.Checked</param>
    /// <param name="job2Checked">DBotStateDeputyHeroJob2.Checked</param>
    public static ClientMessage BuildRecallDeputyHeroClick(bool job0Checked, bool job1Checked, bool job2Checked)
    {
        byte btJob = 0;
        if (job0Checked)
            btJob = 0;
        else if (job1Checked)
            btJob = 1;
        else if (job2Checked)
            btJob = 2;

        return new ClientMessage(Grobal2Const.CM_HEROLOGON, 1, btJob, 0, 0);
    }

    /// <summary>
    /// 原文 9843-9851 SetDeputyHeroJob(btJob:Byte)：把对应单选按钮置 Checked。
    /// 返回被勾选的索引；-1 表示 Initialized = False（原文 9845 直接 Exit）或 btJob 不在 0..2（case 无 else）。
    /// 原文**不会**取消其它按钮的勾选状态（依赖单选分组）。
    /// </summary>
    public static int TrySetDeputyHeroJob(int initialized, int btJob)
    {
        if (initialized == 0)
            return -1;

        switch (btJob)
        {
            case 0: return 0;
            case 1: return 1;
            case 2: return 2;
            default: return -1;
        }
    }

    // ===================== 穴位悬浮提示（原文 6316-6421 的 FunA） =====================

    /// <summary>穴位提示结果（对应原文的 sMsg1 / sMsg2 局部变量）。</summary>
    public readonly struct AcupointHint
    {
        /// <summary>sMsg1：白色/黄色提示（原文 clYellow）。</summary>
        public readonly string Msg1;
        /// <summary>sMsg2：红色提示（原文 clRed）。</summary>
        public readonly string Msg2;

        public AcupointHint(string msg1, string msg2)
        {
            Msg1 = msg1;
            Msg2 = msg2;
        }
    }

    /// <summary>
    /// 原文 6332-6349 的 FunA 主体（逐字对照）：
    ///   - 只有按钮名匹配的**第一个** i 才处理（Break）；
    ///   - Acupoints[i] &gt; 0 → sMsg1 = '穴位：已打通'；
    ///   - 否则若 nCurLevel &gt;= g_AcupointLevels[Page, i] → sMsg1 = '穴位：待打通\需要内功等级N级'；
    ///   - 否则                                        → sMsg2 = '穴位：待打通\需要内功等级N级'。
    /// 注意 '待打通' 与 '需要内功等级' 之间是**反斜杠**（原文 6340/6344 的字面量），不是换行。
    /// </summary>
    /// <param name="acupointNames">当前页的 5 个穴位名（<see cref="AcupointNamesByActivePage"/>）</param>
    /// <param name="senderButtonName">Sender.Name</param>
    /// <param name="nameIndex">FunA 的 NameIndex 实参</param>
    /// <param name="opened">g_MySelf.m_HumMeridians[Page].Acupoints[i]</param>
    /// <param name="currentNgLevel">g_MySelf.m_AbilNG.Level</param>
    /// <param name="requiredLevel">g_AcupointLevels[Page, i]</param>
    public static AcupointHint BuildAcupointHint(
        IReadOnlyList<string> acupointNames,
        string senderButtonName,
        int nameIndex,
        Func<int, int> opened,
        int currentNgLevel,
        Func<int, int> requiredLevel)
    {
        string msg1 = string.Empty;
        string msg2 = string.Empty;

        for (int i = 0; i < acupointNames.Count; i++)
        {
            if (senderButtonName == string.Format(AcupointButtonNameFormat, nameIndex, i))
            {
                if (opened(i) > 0)
                {
                    msg1 = acupointNames[i] + "：已打通";
                    break;
                }
                else
                {
                    if (currentNgLevel >= requiredLevel(i))
                    {
                        msg1 = acupointNames[i] + "：待打通\\" + string.Format(NeedLevFormat, requiredLevel(i));
                        break;
                    }
                    else
                    {
                        msg2 = acupointNames[i] + "：待打通\\" + string.Format(NeedLevFormat, requiredLevel(i));
                        break;
                    }
                }
            }
        }

        return new AcupointHint(msg1, msg2);
    }
}
