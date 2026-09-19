namespace GXX.Client.GUI.NewStateWin;

/// <summary>
/// StateWindows.pas 技能（Magic）分页与升级按钮刷新的**纯逻辑**移植。
///
/// 对应用户端侧原文（Source\Client-HGE\GUI\NewStateWin\StateWindows.pas）：
///   - MagicPageChange              8110-8147   自己技能页（每页 5 或 6 个）
///   - NGMagicPageChange            8781-8811   内功技能页（固定每页 6 个）
///   - HeroMagicPageChange          8417-8448   英雄技能页（固定每页 6 个）
///   - HeroNGMagicPageChange        9460-9490   英雄内功技能页（固定每页 6 个）
///   - DStPageUpClick               8470-8489   自己技能翻页（PageUp / PageDown）
///   - DStNGPageUpClick             8701-8719   内功技能翻页
///   - DHeroStPageUpClick           8450-8468   英雄技能翻页
///   - DHeroStNGPageUpClick         9402-9421   英雄内功技能翻页
///   - RefreshUpgradeButtons        11625-11663 自己技能升级按钮
///   - RefreshHeroUpgradeButtons    11665-11703 英雄技能升级按钮
///
/// 接缝：g_MagicList / g_MagicNGList / g_HeroMagicList / g_HeroMagicNGList（MShare.pas + FState.pas）
/// 尚未移植，本文件只接收它们的 Count 与逐项数据（见 <see cref="IMagicSlot"/>）。
/// 接缝：g_ClientConfig.boUseOldSerialWindows / boStateWindowsType（MShare.pas）以参数传入。
/// </summary>
public static class TStateWindowsMagic
{
    /// <summary>升级按钮数组长度（原文 RefreshUpgradeButtons 的 array[0..5]）。</summary>
    public const int UpgradeButtonCount = 6;

    /// <summary>
    /// 原文 8133/8434/8799/9478 行每次跨页移动的条目数（字面量 6）。
    /// 注意：自己是 176 界面时 CountOnPage 为 5，但翻页步长**仍然是 6**（原文如此）。
    /// </summary>
    public const int PageStep = 6;

    /// <summary>
    /// 单个技能在分页/升级按钮上的可观察字段。
    /// 对应 PTClientMagic 的 Def.wMagicId / Def.CanUpgrade / Def.MaxUpgradeLevel / NewLevel。
    /// </summary>
    public interface IMagicSlot
    {
        /// <summary>PTClientMagic.Def.wMagicId（Word）。</summary>
        int MagicId { get; }

        /// <summary>PTClientMagic.Def.CanUpgrade（Byte）。</summary>
        int CanUpgrade { get; }

        /// <summary>PTClientMagic.Def.MaxUpgradeLevel（Byte）。</summary>
        int MaxUpgradeLevel { get; }

        /// <summary>PTClientMagic.NewLevel（Integer）。</summary>
        int NewLevel { get; }
    }

    /// <summary>
    /// 原文 8116-8119 / 11634-11637 / 11674-11677 的每页条目数。
    /// 「176界面技能分页不对 chongchong 2013-11-08」注释保留。
    /// </summary>
    /// <param name="boUseOldSerialWindows">g_ClientConfig.boUseOldSerialWindows</param>
    /// <param name="boStateWindowsType">g_ClientConfig.boStateWindowsType</param>
    public static int GetCountOnPage(bool boUseOldSerialWindows, int boStateWindowsType)
    {
        // 原文如此（StateWindows.pas:8116）：两个条件都为真才是 5，否则 6。
        if (boUseOldSerialWindows && boStateWindowsType == 0)
            return 5;
        return 6;
    }

    /// <summary>分页计算结果（对应原文局部变量 nPage / nPageCount）。</summary>
    public readonly struct PageInfo
    {
        public readonly int Page;      // nPage
        public readonly int PageCount; // nPageCount
        public readonly int Index;     // 被就地修正后的 *MagicIndex

        public PageInfo(int page, int pageCount, int index)
        {
            Page = page;
            PageCount = pageCount;
            Index = index;
        }
    }

    /// <summary>
    /// 原文 8121-8144：自己技能页 MagicPageChange。
    /// 1-based 显示页：nPage := (MagicIndex + 1) div CountOnPage + 1。
    /// 返回的 Index 是回写 MagicIndex 的值（当 nPage 超界时减一个 CountOnPage，且不小于 0）。
    /// </summary>
    /// <param name="magicCount">g_MagicList.Count（原文在 Lock 内读取）</param>
    /// <param name="magicIndex">MagicIndex（in/out 语义：以返回值回写）</param>
    /// <param name="countOnPage">GetCountOnPage 的结果</param>
    public static PageInfo MagicPageChange(int magicCount, int magicIndex, int countOnPage)
    {
        int nPage;
        int nPageCount;

        if (magicCount > 0)
        {
            nPage = (magicIndex + 1) / countOnPage + 1;   // div：两者皆非负时 = 截断除法
            nPageCount = magicCount / countOnPage;
            if (magicCount % countOnPage > 0)
                nPageCount = nPageCount + 1;              // Inc(nPageCount)
            if (nPageCount <= 0)
                nPageCount = 1;

            // TODO -ochongchong -c修改 : 第二页只有一个魔法，删除魔法后，不重定位到第一页 【2013-09-04】
            if (nPage > nPageCount)
            {
                nPage = nPageCount;

                if (magicIndex > 0)
                    magicIndex = magicIndex - countOnPage;   // Dec(MagicIndex, CountOnPage)
                if (magicIndex < 0)
                    magicIndex = 0;
            }
        }
        else
        {
            nPage = 0;
            nPageCount = 0;
        }

        return new PageInfo(nPage, nPageCount, magicIndex);
    }

    /// <summary>
    /// 原文 8417-8448 / 8781-8811 / 9460-9490：英雄技能页 / 内功页 / 英雄内功页。
    /// 与 <see cref="MagicPageChange"/> 的差异（必须逐字保留）：
    ///   1) 每页固定 6（不受 176 界面影响）；
    ///   2) nPageCount &lt;= 0 的兜底仍然存在；
    ///   3) 自己技能页在 nPage 超界时**总会** RefreshUpgradeButtons，另外三个只在 Count &gt; 0 分支里刷新。
    /// </summary>
    public static PageInfo FixedSixPageChange(int listCount, int index)
    {
        int nPage;
        int nPageCount;

        if (listCount > 0)
        {
            nPage = (index + 1) / 6 + 1;
            nPageCount = listCount / 6;
            if (listCount % 6 > 0)
                nPageCount = nPageCount + 1;
            if (nPageCount <= 0)
                nPageCount = 1;

            // TODO -ochongchong -c修改 : 第二页只有一个魔法，删除魔法后，不重定位到第一页 【2013-09-04】
            if (nPage > nPageCount)
            {
                nPage = nPageCount;

                if (index > 0)
                    index = index - 6;
                if (index < 0)
                    index = 0;
            }
        }
        else
        {
            nPage = 0;
            nPageCount = 0;
        }

        return new PageInfo(nPage, nPageCount, index);
    }

    /// <summary>翻页方向（原文用 Sender = DStPageUp / DStNGPageUp 判定）。</summary>
    public enum PageDirection
    {
        /// <summary>Sender 是 PageUp 按钮。</summary>
        Up,
        /// <summary>Sender 不是 PageUp 按钮（即 PageDown）。</summary>
        Down,
    }

    /// <summary>
    /// 原文 8470-8489（自己技能，步长固定 6，**不使用** CountOnPage）：
    ///   PageUp : if MagicIndex &gt; 0 then Dec(MagicIndex, 6); if MagicIndex &lt; 0 then MagicIndex := 0;
    ///   PageDown: if MagicIndex + 6 &lt; Count then Inc(MagicIndex, 6);
    /// 用户侧原文同样的逻辑见 8450-8468 / 8701-8719 / 9402-9421。
    /// </summary>
    /// <param name="listCount">对应 g_*MagicList.Count（原文在 Lock 内读取）</param>
    /// <param name="index">MagicIndex / MagicNGIndex / HeroMagicIndex / HeroMagicNGIndex</param>
    /// <param name="direction">Sender 判定结果</param>
    public static int PageUpDown(int listCount, int index, PageDirection direction)
    {
        if (direction == PageDirection.Up)
        {
            if (index > 0)
                index = index - 6;              // Dec(MagicIndex, 6)
            if (index < 0)
                index = 0;
        }
        else
        {
            // 严格小于：index + 6 == Count 时不翻（最后一页只有 1 个时仍可停留在该页）
            if (index + 6 < listCount)
                index = index + 6;              // Inc(MagicIndex, 6)
        }

        return index;
    }

    /// <summary>
    /// 原文 11651-11662 / 11691-11702 的一个升级按钮槽位。
    /// Visible 与 Enabled 的判据**不同**（见 <see cref="EvalUpgradeButton"/>）。
    /// </summary>
    public readonly struct UpgradeButtonState
    {
        /// <summary>ImageButton.Visible</summary>
        public readonly bool Visible;
        /// <summary>ImageButton.Enabled</summary>
        public readonly bool Enabled;
        /// <summary>ImageButton.Tag（= Magic.Def.wMagicId；未被填充时保持 0）</summary>
        public readonly int Tag;

        public UpgradeButtonState(bool visible, bool enabled, int tag)
        {
            Visible = visible;
            Enabled = enabled;
            Tag = tag;
        }
    }

    /// <summary>
    /// 原文 11645-11649 / 11685-11689 的按钮清空循环：所有按钮 Visible := False; Tag := 0;
    /// 原文**不重置** Enabled，故上一帧的 Enabled 会残留到本帧未被填充的按钮上（<see cref="EvalUpgradeButton"/>
    /// 会为被填充的槽位重算 Enabled，未被填充的槽位则保留旧值）。
    /// 传入 <paramref name="previous"/> 复现该残留语义；不传则以 Enabled=false 起步。
    /// </summary>
    public static UpgradeButtonState[] ClearUpgradeButtons(UpgradeButtonState[] previous = null)
    {
        var result = new UpgradeButtonState[UpgradeButtonCount];
        for (int i = 0; i < UpgradeButtonCount; i++)
        {
            bool enabled = previous != null && previous.Length == UpgradeButtonCount && previous[i].Enabled;
            result[i] = new UpgradeButtonState(false, enabled, 0);
        }
        return result;
    }

    /// <summary>
    /// 原文 11651-11662：对自己技能列表按页填充 6 个升级按钮。
    /// 返回长度固定 6 的数组；未被填充的槽位 Visible=false、Tag=0（Enabled 见 <see cref="ClearUpgradeButtons"/>）。
    /// </summary>
    /// <param name="list">g_MagicList / g_HeroMagicList（按 index 取，nil 元素被跳过）</param>
    /// <param name="magicIndex">MagicIndex / HeroMagicIndex</param>
    /// <param name="countOnPage">GetCountOnPage 的结果</param>
    /// <param name="previous">上一帧的按钮数组（复现原文「Enabled 残留」语义，可空）</param>
    public static UpgradeButtonState[] RefreshUpgradeButtons(
        IReadOnlyList<IMagicSlot> list, int magicIndex, int countOnPage,
        UpgradeButtonState[] previous = null)
    {
        var buttons = ClearUpgradeButtons(previous);

        for (int i = magicIndex; i <= magicIndex + countOnPage - 1; i++)
        {
            if (i >= 0 && i < list.Count)
            {
                var magic = list[i];
                if (magic != null)
                    buttons[i - magicIndex] = EvalUpgradeButton(magic);
            }
        }

        return buttons;
    }

    /// <summary>
    /// 原文 11657-11659 的三行判据（Visible 与 Enabled 判据不同，务必区分）：
    ///   Visible := (CanUpgrade &lt;&gt; 0) and (NewLevel &lt; MaxUpgradeLevel);
    ///   Enabled := (CanUpgrade = 1);
    ///   Tag     := wMagicId;
    /// </summary>
    public static UpgradeButtonState EvalUpgradeButton(IMagicSlot magic)
    {
        bool visible = (magic.CanUpgrade != 0) && (magic.NewLevel < magic.MaxUpgradeLevel);
        bool enabled = magic.CanUpgrade == 1;
        return new UpgradeButtonState(visible, enabled, magic.MagicId);
    }

    /// <summary>
    /// 把「技能在列表中的下标 → 该页按钮下标」的映射单独暴露，供鼠标移动（原文 8737：
    /// Index := DxButton.Tag + MagicNGIndex）等命中测试复用。
    /// 原文按钮按下时 Tag 由 <see cref="RefreshUpgradeButtons"/> 写成 wMagicId，
    /// 但 DStNGMagMouseMove 用的是 Tag + MagicNGIndex（把 Tag 当作按钮序号），此处照抄该加法。
    /// </summary>
    public static int ButtonTagToListItemIndex(int buttonTag, int magicIndex)
        => buttonTag + magicIndex;   // 原文如此（StateWindows.pas:8737）
}
