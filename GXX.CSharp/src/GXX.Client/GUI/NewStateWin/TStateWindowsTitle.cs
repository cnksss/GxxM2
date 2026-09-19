namespace GXX.Client.GUI.NewStateWin;

/// <summary>
/// StateWindows.pas 称号（FengHao / 封号）分页、时装首饰显隐的**纯逻辑**移植。
///
/// 对应用户端侧原文（Source\Client-HGE\GUI\NewStateWin\StateWindows.pas）：
///   - DSTitlePageClick        12187-12206 自己称号翻页（FengHaoIndex + g_FengHaoItems.Count）
///   - DUSTitlePageClick       12636-12655 观察者称号翻页（USFengHaoIndex + g_USFengHaoItems.Count）
///   - DSHeroTitlePageClick    13124-13143 英雄称号翻页（HeroFengHaoIndex + g_HeroFengHaoItems.Count）
///   - RefreshMyFashionJewelryInfo    13157-13172
///   - RefreshHeroFashionJewelryInfo  13174-13189
///   - RefreshUserFashionJewelryInfo  13191-13206
///   - RefreshFashionJewelryInfo      13145-13155 三者按可见性分派
///
/// 接缝：g_ClientVersion（MShare.pas:2324，TClientVersion = DxComponents.pas:27）尚未移植，
/// 本文件用 <see cref="TStateWindowsClientVersion"/> 复刻其序号（cv176=0 … cvMirNewUI205=6）。
/// 接缝：g_FengHaoItems / g_USFengHaoItems / g_HeroFengHaoItems（MShare.pas）只取 Count。
/// 接缝：g_MySelf / g_MyHero / g_UserState1 与 g_ClientConfig.boFashionJewelryOpen（FState.pas + MShare.pas）以参数传入。
/// </summary>
public static class TStateWindowsTitle
{
    /// <summary>
    /// DxComponents.pas:27 `TClientVersion = (cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205);`
    /// 的序号复刻（枚举尚未移植，先以常量接缝）。
    /// </summary>
    public static class TStateWindowsClientVersion
    {
        public const int cv176 = 0;
        public const int cv185 = 1;
        public const int cvHero = 2;
        public const int cvSerial = 3;
        public const int cvMirSequel = 4;
        public const int cvMirNewUI205 = 5;

        /// <summary>MShare.pas:2324 / Share.pas:34 的初值 g_ClientVersion = cvSerial。</summary>
        public const int Default = cvSerial;
    }

    /// <summary>翻页方向（原文用 Sender = DSTitlePageUp / DUSTitlePageUp / DSHeroTitlePageUp 判定）。</summary>
    public enum PageDirection
    {
        /// <summary>Sender 是 PageUp 按钮。</summary>
        Up,
        /// <summary>Sender 不是 PageUp 按钮（即 PageDown）。</summary>
        Down,
    }

    /// <summary>
    /// 原文 12191-12194 / 12640-12643 / 13128-13131 的每页称号数：
    ///   非 cvMirNewUI205 → 6；cvMirNewUI205 → 5。
    /// 与 LoadFromStream（2151-2158）中「DSTitleButton6/DSTitleName6 仅在非 205 版可见」一致。
    /// </summary>
    public static int GetTitlePageCount(int clientVersion)
        => clientVersion != TStateWindowsClientVersion.cvMirNewUI205 ? 6 : 5;

    /// <summary>
    /// 原文 12196-12205 / 12645-12654 / 13133-13142（三处逐字相同，仅索引变量与列表不同）：
    ///   PageUp : if FengHaoIndex &gt; 0 then Dec(FengHaoIndex, PageCount); if FengHaoIndex &lt; 0 then FengHaoIndex := 0;
    ///   PageDown: if FengHaoIndex + PageCount &lt; Count then Inc(FengHaoIndex, PageCount);
    /// 注意向下翻页用**严格小于**，且**不回调 MagicPageChange 那样的重定位**。
    /// </summary>
    /// <param name="itemCount">g_FengHaoItems.Count / g_USFengHaoItems.Count / g_HeroFengHaoItems.Count</param>
    /// <param name="index">FengHaoIndex / USFengHaoIndex / HeroFengHaoIndex</param>
    /// <param name="clientVersion">g_ClientVersion</param>
    /// <param name="direction">Sender 判定结果</param>
    public static int PageClick(int itemCount, int index, int clientVersion, PageDirection direction)
    {
        int pageCount = GetTitlePageCount(clientVersion);

        if (direction == PageDirection.Up)
        {
            if (index > 0)
                index = index - pageCount;
            if (index < 0)
                index = 0;
        }
        else
        {
            if (index + pageCount < itemCount)
                index = index + pageCount;
        }

        return index;
    }

    // ===================== 时装首饰显隐（原文 13157-13206） =====================

    /// <summary>
    /// 原文 13159-13171 / 13176-13188 / 13193-13205 的公共结果：
    ///   IsMale    := 性别 = 0；
    ///   UseSetting2 := not boFashionJewelryOpen；
    ///   10 个首饰位按钮 Visible := boFashionJewelryOpen。
    /// 三处唯一的差别是控件名前缀（DSWFashion* / DHeroSWFashion* / DFashion*US1）与性别来源。
    /// </summary>
    public readonly struct FashionJewelryState
    {
        /// <summary>D*StateFashion.IsMale</summary>
        public readonly bool IsMale;
        /// <summary>D*StateFashion.UseSetting2（= not boFashionJewelryOpen）</summary>
        public readonly bool UseSetting2;
        /// <summary>10 个首饰位按钮的 Visible（全部等于 boFashionJewelryOpen）</summary>
        public readonly bool JewelryVisible;

        public FashionJewelryState(bool isMale, bool useSetting2, bool jewelryVisible)
        {
            IsMale = isMale;
            UseSetting2 = useSetting2;
            JewelryVisible = jewelryVisible;
        }
    }

    /// <summary>原文 13159-13160：性别 == 0 才是男性。</summary>
    public static FashionJewelryState BuildFashionJewelryState(int sex, bool boFashionJewelryOpen)
        => new FashionJewelryState(sex == 0, !boFashionJewelryOpen, boFashionJewelryOpen);

    /// <summary>
    /// 原文 13145-13155 RefreshFashionJewelryInfo 的分派顺序（**不是** else-if，三个 if 互不排斥）。
    /// </summary>
    /// <param name="stateWinVisible">DStateWin.Visible</param>
    /// <param name="heroStateWinVisible">DHeroStateWin.Visible</param>
    /// <param name="userState1Visible">DUserState1.Visible</param>
    public static (bool MySelf, bool Hero, bool User) RefreshDispatch(
        bool stateWinVisible, bool heroStateWinVisible, bool userState1Visible)
        => (stateWinVisible, heroStateWinVisible, userState1Visible);

    /// <summary>
    /// 原文 13193：观察者窗口的性别来自 THumFeature.Buffer 的 btGender 字节（不是 m_btSex）。
    /// pTHumFeature(@g_UserState1.feature.Buffer).btGender = 0 才是男性。
    /// </summary>
    public static bool IsUserStateFashionMale(int featureGenderByte) => featureGenderByte == 0;

    /// <summary>
    /// 原文 2166-2174（LoadFromStream）：把 DStateBigGoldLabel.Caption 去掉尾部数字并多留一个字符。
    /// <code>
    /// FLableCaption_BigGold := Trim(Caption);
    /// if FLableCaption_BigGold &lt;&gt; '' then
    ///   for Index := Length(FLableCaption_BigGold) downto 1 do
    ///     if not (FLableCaption_BigGold[Index] in ['0'..'9']) then begin
    ///       FLableCaption_BigGold := Copy(FLableCaption_BigGold, 1, Index);   // 1-based，含 Index 本身
    ///       Break;
    ///     end;
    /// </code>
    /// 注意：若整个串都是数字，则循环走完也不 Break → 保持原串（原文如此）。
    /// </summary>
    public static string ExtractBigGoldPrefix(string caption)
    {
        string text = (caption ?? string.Empty).Trim();
        if (text.Length == 0)
            return text;

        // Delphi 1-based：Index 从 Length 递减到 1；Copy(s, 1, Index) 取前 Index 个字符。
        for (int index = text.Length; index >= 1; index--)
        {
            char c = text[index - 1];
            if (c < '0' || c > '9')
            {
                // 原文保留 Index 位置上的那个非数字字符本身（Copy 长度 = Index，非 Index-1）。
                return text.Substring(0, index);
            }
        }

        return text;
    }
}
