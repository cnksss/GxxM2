// ============================================================================
// 源单元：Source/M2Engine/Forms/ViewHeroRcd.pas（453 行，GBK）
// 同源 DFM：Source/M2Engine/Forms/ViewHeroRcd.dfm（**二进制 DFM**，4,510 字节）
//   ⚠ `_analysis/utf8_mirror/**` 里的同名 .dfm **已损坏**（二进制被当文本转码：
//     首字节 FF→EF A3 B5、长度字段丢字节）⇒ 本单元回读 `Source/` 原始二进制并手工解码，
//     解码结论见 docs\并行报告-p9-m2-forms.md §0.2（解到 4510 = 文件长度，零残留）。
//   DFM 控件 **29** 个；事件绑定 **1** 个（`OnCreate = FormCreate`）；窗体属性见 DfmBounds/InitializeComponents。
//
// 类型/方法清单（.pas 行号）：
//   TFrmHeroFDBViewer（:10-64 声明；public 字段 n2F8 :56 / s2FC :57）
//     FormCreate            :72-78
//     ShowHumData           :80-90
//     GetUserItemGrid       :92-100    GetBagItemGrid :102-110   GetUseMagicGrid :112-120
//     sub_49A0C0            :122-175
//     InitUserItemGrid      :177-204
//     sub_49A9DC            :206-218
//     sub_49AB10            :220-237
//     ShowBagItem           :239-257
//     ShowUserItem          :259-275
//     ShowHumanInfo         :277-335   （**整段被 (* *) 注释掉 ⇒ 空体**）
//     ShowBagItems          :337-360   （同上，空体）
//     ShowUserItems         :362-395   （同上，空体）
//     ShowUseMagic          :397-451   （同上，空体）
//   单元级全局 `FrmHeroFDBViewer`（:67）
//
// 原文 uses（:5-8）Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms,
//   Dialogs, StdCtrls, TabNotBk, Grids, ExtCtrls, Buttons, ComCtrls, Grobal2, DBShare, HUtil32。
//   派生依赖：`GetStdItemName`（HUtil32/DBShare 侧）⇒ 实例接缝，默认转调既有
//   `GXX.M2Server.DbLayer.DbLayerRunSeam.GetStdItemName`（同一未接线函数，未臆造第二份）。
//
// ★ 原文缺陷/怪癖（逐字保留 + 差异断言锁定）：
//   1. `ShowUserItem`（:259-275）**从不写第 0 列**，而 `ShowBagItem`（:239-257）**两个分支都写第 0 列**
//      —— 同一单元内两份几乎相同的代码不对称。逐字保留。
//   2. `sub_49A0C0`（:122-175）写 `HumanGrid.Cells[0..11, 1/3/5/7]`，全部落在 DFM `RowCount = 15` 内；
//      但 `InitUserItemGrid`（:177-204）写 `Cells[0, 15]`，而 `UserItemGrid1/2` 的 DFM `RowCount = 14`
//      ⇒ 靠 `TStringGrid` 的**自动扩容**涨到 16；同理 `sub_49AB10` 写 `Cells[4, 0]` 而
//      `UseMagicGrid1/2` 的 DFM `ColCount = 4` ⇒ 涨到 5。两个事实都锁了测试。
//   3. DFM 的 `ActivePage` 三处不对称：`PageControlHero = TabSheet1`（**最后一页**，索引 4）、
//      `PageControlHeroJob0 = TabSheet9`（索引 2）、`Job1 = TabSheet10`（索引 0）、`Job2 = TabSheet15`（索引 2）。
//      逐字保留（构造期就摆成这个初始态），并由 `ShowHumData`（:86-89）在打开时全部归零。
//   4. `ShowHumData`（:80-90）**不 Show/不 ShowModal**（对比同族窗体都有 ShowModal）—— 原文如此。
//   5. `GetUserItemGrid` 一族的 `case Job of 1: ... 2: ... else ...` —— `else` 吞掉**一切**其它值
//      （含 0 与负数），逐字保留。
// ============================================================================

using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// 原文 `ViewHeroRcd.pas:10-64 TFrmHeroFDBViewer = class(TForm)` 1:1。
/// <para>英雄 FDB 数据查看窗体：1 个主页签（5 页）+ 3 个职业子页签（各 3 页，含 3 张表格）。</para>
/// </summary>
public sealed partial class TFrmHeroFDBViewer : System.Windows.Forms.Form
{
    // ==================================================================
    // DFM 控件（29 个；声明顺序 = DFM 顺序，见 :11-39）
    // ==================================================================

    /// <summary>DFM `PageControlHero: TPageControl`（0,0 918×317，`ActivePage = TabSheet1`）。</summary>
    public System.Windows.Forms.TabControl PageControlHero = null!;

    /// <summary>DFM `TabSheet3`（`Caption = '英雄信息'`）。</summary>
    public System.Windows.Forms.TabPage TabSheet3 = null!;
    /// <summary>DFM `TabSheet4`（`Caption = '战士英雄'`，`ImageIndex = 1`）。</summary>
    public System.Windows.Forms.TabPage TabSheet4 = null!;
    /// <summary>DFM `TabSheet5`（`Caption = '法师英雄'`，`ImageIndex = 2`）。</summary>
    public System.Windows.Forms.TabPage TabSheet5 = null!;
    /// <summary>DFM `TabSheet6`（`Caption = '道士英雄'`，`ImageIndex = 3`）。</summary>
    public System.Windows.Forms.TabPage TabSheet6 = null!;
    /// <summary>DFM `TabSheet1`（`Caption = '连击技能'`，`ImageIndex = 4`）。</summary>
    public System.Windows.Forms.TabPage TabSheet1 = null!;

    /// <summary>DFM `HumanGrid: TStringGrid`（0,0 910×289，`Align = alClient`）。</summary>
    public Sweep9FormsStringGrid HumanGrid = null!;

    /// <summary>DFM `PageControlHeroJob0: TPageControl`（`ActivePage = TabSheet9`）。</summary>
    public System.Windows.Forms.TabControl PageControlHeroJob0 = null!;
    /// <summary>DFM `TabSheet7`（`Caption = '身上装备'`）。</summary>
    public System.Windows.Forms.TabPage TabSheet7 = null!;
    /// <summary>DFM `UserItemGrid0: TStringGrid`。</summary>
    public Sweep9FormsStringGrid UserItemGrid0 = null!;
    /// <summary>DFM `TabSheet8`（`Caption = '背包物品'`，`ImageIndex = 1`）。</summary>
    public System.Windows.Forms.TabPage TabSheet8 = null!;
    /// <summary>DFM `BagItemGrid0: TStringGrid`。</summary>
    public Sweep9FormsStringGrid BagItemGrid0 = null!;
    /// <summary>DFM `TabSheet9`（`Caption = '修炼技能'`，`ImageIndex = 2`）。</summary>
    public System.Windows.Forms.TabPage TabSheet9 = null!;
    /// <summary>DFM `UseMagicGrid0: TStringGrid`。</summary>
    public Sweep9FormsStringGrid UseMagicGrid0 = null!;

    /// <summary>DFM `PageControlHeroJob1: TPageControl`（`ActivePage = TabSheet10`）。</summary>
    public System.Windows.Forms.TabControl PageControlHeroJob1 = null!;
    /// <summary>DFM `TabSheet10`（`Caption = '身上装备'`）。</summary>
    public System.Windows.Forms.TabPage TabSheet10 = null!;
    /// <summary>DFM `UserItemGrid1: TStringGrid`。</summary>
    public Sweep9FormsStringGrid UserItemGrid1 = null!;
    /// <summary>DFM `TabSheet11`（`Caption = '背包物品'`，`ImageIndex = 1`）。</summary>
    public System.Windows.Forms.TabPage TabSheet11 = null!;
    /// <summary>DFM `BagItemGrid1: TStringGrid`。</summary>
    public Sweep9FormsStringGrid BagItemGrid1 = null!;
    /// <summary>DFM `TabSheet12`（`Caption = '修炼技能'`，`ImageIndex = 2`）。</summary>
    public System.Windows.Forms.TabPage TabSheet12 = null!;
    /// <summary>DFM `UseMagicGrid1: TStringGrid`。</summary>
    public Sweep9FormsStringGrid UseMagicGrid1 = null!;

    /// <summary>DFM `PageControlHeroJob2: TPageControl`（`ActivePage = TabSheet15`）。</summary>
    public System.Windows.Forms.TabControl PageControlHeroJob2 = null!;
    /// <summary>DFM `TabSheet13`（`Caption = '身上装备'`）。</summary>
    public System.Windows.Forms.TabPage TabSheet13 = null!;
    /// <summary>DFM `UserItemGrid2: TStringGrid`。</summary>
    public Sweep9FormsStringGrid UserItemGrid2 = null!;
    /// <summary>DFM `TabSheet14`（`Caption = '背包物品'`，`ImageIndex = 1`）。</summary>
    public System.Windows.Forms.TabPage TabSheet14 = null!;
    /// <summary>DFM `BagItemGrid2: TStringGrid`。</summary>
    public Sweep9FormsStringGrid BagItemGrid2 = null!;
    /// <summary>DFM `TabSheet15`（`Caption = '修炼技能'`，`ImageIndex = 2`）。</summary>
    public System.Windows.Forms.TabPage TabSheet15 = null!;
    /// <summary>DFM `UseMagicGrid2: TStringGrid`。</summary>
    public Sweep9FormsStringGrid UseMagicGrid2 = null!;

    /// <summary>DFM `UseSuccessiveMagicGrid: TStringGrid`（0,0 910×289，`Align = alClient`）。</summary>
    public Sweep9FormsStringGrid UseSuccessiveMagicGrid = null!;

    // ==================================================================
    // :56-57 public 字段
    // ==================================================================

    /// <summary>原文 `:56 n2F8: Integer;`（**全单元无写入点** —— 只被注释掉的 `ShowHumanInfo` 读过，照抄保留）。</summary>
    public int n2F8;

    /// <summary>原文 `:57 s2FC: string;`（同上，无写入点）。</summary>
    public string s2FC = "";

    /// <summary>
    /// 接缝：`GetStdItemName(wIndex)`（被 `ShowBagItem` :247 与 `ShowUserItem` :266 调用）。
    /// <para>
    /// 默认转调**既有** `GXX.M2Server.DbLayer.DbLayerRunSeam.GetStdItemName`
    /// （该接缝本身就代表"`UsrEngn.pas` 的同一函数尚未接线"，未臆造第二份语义）。
    /// </para>
    /// </summary>
    public Func<int, string> GetStdItemNameHandler = DbLayerRunSeam.GetStdItemName;

    // ==================================================================
    // DFM 原始几何留证（`Align` 由 Dock 承载 ⇒ 布局期会被重算，故单独留证）
    // ==================================================================

    /// <summary>
    /// 二进制 DFM 手工解码出的**原始几何表**（键 = DFM `object` 名）。
    /// <para>只列 DFM 里**确实写了** `Left/Top/Width/Height` 的节点 —— `TTabSheet` 在 VCL 里
    /// 不写几何（由页签控件计算），故 15 个 TabSheet 不在表内（它们的 `Caption`/`ImageIndex`
    /// 另行断言）。</para>
    /// </summary>
    public static readonly IReadOnlyDictionary<string, System.Drawing.Rectangle> DfmBounds =
        new Dictionary<string, System.Drawing.Rectangle>(StringComparer.Ordinal)
        {
            ["FrmHeroFDBViewer"] = new(851, 295, 918, 317),        // 窗体（ClientWidth/ClientHeight）
            ["PageControlHero"] = new(0, 0, 918, 317),
            ["HumanGrid"] = new(0, 0, 910, 289),
            ["PageControlHeroJob0"] = new(0, 0, 910, 289),
            ["UserItemGrid0"] = new(0, 0, 902, 261),
            ["BagItemGrid0"] = new(0, 0, 902, 261),
            ["UseMagicGrid0"] = new(0, 0, 902, 261),
            ["PageControlHeroJob1"] = new(0, 0, 910, 289),
            ["UserItemGrid1"] = new(0, 0, 902, 261),
            ["BagItemGrid1"] = new(0, 0, 902, 261),
            ["UseMagicGrid1"] = new(0, 0, 902, 261),
            ["PageControlHeroJob2"] = new(0, 0, 910, 289),
            ["UserItemGrid2"] = new(0, 0, 902, 261),
            ["BagItemGrid2"] = new(0, 0, 902, 261),
            ["UseMagicGrid2"] = new(0, 0, 902, 261),
            ["UseSuccessiveMagicGrid"] = new(0, 0, 910, 289),
        };

    /// <summary>
    /// DFM 里 `TTabSheet` 的 `Caption`（原样，含 `ImageIndex` 为 0 时 DFM 未写的情况）。
    /// 键 = DFM 名，值 = (`Caption`, `ImageIndex` 或 -1 表示 DFM 未写)。
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (string Caption, int ImageIndex)> DfmTabSheets =
        new Dictionary<string, (string, int)>(StringComparer.Ordinal)
        {
            ["TabSheet3"] = ("英雄信息", -1),
            ["TabSheet4"] = ("战士英雄", 1),
            ["TabSheet5"] = ("法师英雄", 2),
            ["TabSheet6"] = ("道士英雄", 3),
            ["TabSheet1"] = ("连击技能", 4),
            ["TabSheet7"] = ("身上装备", -1),
            ["TabSheet8"] = ("背包物品", 1),
            ["TabSheet9"] = ("修炼技能", 2),
            ["TabSheet10"] = ("身上装备", -1),
            ["TabSheet11"] = ("背包物品", 1),
            ["TabSheet12"] = ("修炼技能", 2),
            ["TabSheet13"] = ("身上装备", -1),
            ["TabSheet14"] = ("背包物品", 1),
            ["TabSheet15"] = ("修炼技能", 2),
        };

    /// <summary>DFM 里三个 `TPageControl` 的 `ActivePage`（名 → 页名），逐字保留其**不对称**。</summary>
    public static readonly IReadOnlyDictionary<string, string> DfmActivePages =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["PageControlHero"] = "TabSheet1",          // ★ 最后一页（索引 4）
            ["PageControlHeroJob0"] = "TabSheet9",      // 索引 2
            ["PageControlHeroJob1"] = "TabSheet10",     // 索引 0
            ["PageControlHeroJob2"] = "TabSheet15",     // 索引 2
        };

    // ==================================================================
    // 构造
    // ==================================================================

    /// <summary>构造 + 装载二进制 DFM 手工解码出的控件树（DFM `OnCreate = FormCreate` 由宿主触发）。</summary>
    public TFrmHeroFDBViewer()
    {
        InitializeComponents();
    }

    /// <summary>
    /// 原文 `:72-78 procedure TFrmHeroFDBViewer.FormCreate(Sender: TObject)`。
    /// <para>原文 `Sender` 未被使用（保留形参以对齐签名）。</para>
    /// </summary>
    public void FormCreate(object? Sender = null)
    {
        // 原文 sub_49A0C0(); sub_49A9DC(); sub_49AB10(); InitUserItemGrid();
        sub_49A0C0();
        sub_49A9DC();
        sub_49AB10();
        InitUserItemGrid();
    }

    /// <summary>
    /// 原文 `:80-90 procedure TFrmHeroFDBViewer.ShowHumData();`。
    /// <para>★ 原文**没有** `ShowModal`（对比同族窗体）—— 逐字保留。</para>
    /// </summary>
    public void ShowHumData()
    {
        ShowHumanInfo();
        ShowUserItems();
        ShowBagItems();
        ShowUseMagic();
        // 原文 PageControlHero.ActivePageIndex := 0;（Delphi 越界静默，见 §21.3）
        SetActivePageIndex(PageControlHero, 0);
        SetActivePageIndex(PageControlHeroJob0, 0);
        SetActivePageIndex(PageControlHeroJob1, 0);
        SetActivePageIndex(PageControlHeroJob2, 0);
    }

    // ==================================================================
    // 索引属性（原文 `property UserItemGrid[Job: Integer]: TStringGrid read GetUserItemGrid`）
    // C# 无"具名索引器" ⇒ 落为方法，调用形态 `UserItemGrid(Job)`（与 `[Job]` 一一对应）
    // ==================================================================

    /// <summary>原文 `:61 property UserItemGrid[Job: Integer]`。</summary>
    public Sweep9FormsStringGrid UserItemGrid(int Job) => GetUserItemGrid(Job);

    /// <summary>原文 `:62 property BagItemGrid[Job: Integer]`。</summary>
    public Sweep9FormsStringGrid BagItemGrid(int Job) => GetBagItemGrid(Job);

    /// <summary>原文 `:63 property UseMagicGrid[Job: Integer]`。</summary>
    public Sweep9FormsStringGrid UseMagicGrid(int Job) => GetUseMagicGrid(Job);

    /// <summary>
    /// 原文 `:92-100 function TFrmHeroFDBViewer.GetUserItemGrid(Job: Integer): TStringGrid`。
    /// </summary>
    public Sweep9FormsStringGrid GetUserItemGrid(int Job)
    {
        // 原文 case Job of 1: ...; 2: ...; else ...; end;
        switch (Job)
        {
            case 1: return UserItemGrid1;
            case 2: return UserItemGrid2;
            default: return UserItemGrid0;      // ★ else 吞掉一切其它值（含 0 / 负数），原文如此
        }
    }

    /// <summary>原文 `:102-110 function TFrmHeroFDBViewer.GetBagItemGrid(Job: Integer): TStringGrid`。</summary>
    public Sweep9FormsStringGrid GetBagItemGrid(int Job)
    {
        switch (Job)
        {
            case 1: return BagItemGrid1;
            case 2: return BagItemGrid2;
            default: return BagItemGrid0;
        }
    }

    /// <summary>原文 `:112-120 function TFrmHeroFDBViewer.GetUseMagicGrid(Job: Integer): TStringGrid`。</summary>
    public Sweep9FormsStringGrid GetUseMagicGrid(int Job)
    {
        switch (Job)
        {
            case 1: return UseMagicGrid1;
            case 2: return UseMagicGrid2;
            default: return UseMagicGrid0;
        }
    }

    // ==================================================================
    // :122-175 sub_49A0C0 —— HumanGrid 表头（12 列 × 4 行，共 48 条）
    // ==================================================================

    /// <summary>原文 `:122-175 procedure TFrmHeroFDBViewer.sub_49A0C0();`。</summary>
    public void sub_49A0C0()
    {
        HumanGrid.Cells[0, 1] = "索引号";
        HumanGrid.Cells[1, 1] = "名称";
        HumanGrid.Cells[2, 1] = "地图";
        HumanGrid.Cells[3, 1] = "CX";
        HumanGrid.Cells[4, 1] = "CY";
        HumanGrid.Cells[5, 1] = "方向";
        HumanGrid.Cells[6, 1] = "职业";
        HumanGrid.Cells[7, 1] = "性别";
        HumanGrid.Cells[8, 1] = "头发";
        HumanGrid.Cells[9, 1] = "金币数";
        HumanGrid.Cells[10, 1] = "主人名称";
        HumanGrid.Cells[11, 1] = "Home";

        HumanGrid.Cells[0, 3] = "HomeX";
        HumanGrid.Cells[1, 3] = "HomeY";
        HumanGrid.Cells[2, 3] = "等级";
        HumanGrid.Cells[3, 3] = "AC";
        HumanGrid.Cells[4, 3] = "MAC";
        HumanGrid.Cells[5, 3] = "Reserved1";
        HumanGrid.Cells[6, 3] = "DC/1";
        HumanGrid.Cells[7, 3] = "DC/2";
        HumanGrid.Cells[8, 3] = "MC/1";
        HumanGrid.Cells[9, 3] = "MC/2";
        HumanGrid.Cells[10, 3] = "SC/1";
        HumanGrid.Cells[11, 3] = "SC/2";

        HumanGrid.Cells[0, 5] = "Reserved2";
        HumanGrid.Cells[1, 5] = "HP";
        HumanGrid.Cells[2, 5] = "MaxHP";
        HumanGrid.Cells[3, 5] = "MP";
        HumanGrid.Cells[4, 5] = "MaxMP";
        HumanGrid.Cells[5, 5] = "Reserved2";
        HumanGrid.Cells[6, 5] = "当前经验";
        HumanGrid.Cells[7, 5] = "升级经验";
        HumanGrid.Cells[8, 5] = "PK点数";
        HumanGrid.Cells[9, 5] = "忠诚度";
        HumanGrid.Cells[10, 5] = "登录帐号";
        HumanGrid.Cells[11, 5] = "最后登录时间";

        HumanGrid.Cells[0, 7] = "修炼内功";
        HumanGrid.Cells[1, 7] = "修炼心法";
        HumanGrid.Cells[2, 7] = "内功等级";
        HumanGrid.Cells[3, 7] = "当前内力值";
        HumanGrid.Cells[4, 7] = "内力值上限";
        HumanGrid.Cells[5, 7] = "当前内功经验";
        HumanGrid.Cells[6, 7] = "内功最高经验";
        HumanGrid.Cells[7, 7] = "酒量";
        HumanGrid.Cells[8, 7] = "酒量上限";
        HumanGrid.Cells[9, 7] = "药力值";
        HumanGrid.Cells[10, 7] = "药力值上限";
        HumanGrid.Cells[11, 7] = "醉酒度";
    }

    // ==================================================================
    // :177-204 InitUserItemGrid
    // ==================================================================

    /// <summary>原文 `:177-204 procedure TFrmHeroFDBViewer.InitUserItemGrid;`。</summary>
    public void InitUserItemGrid()
    {
        // 原文 for I := 0 to 2 do（用**索引属性**取表 ⇒ 0/1/2 分别落到 UserItemGrid0/1/2）
        for (int I = 0; I <= 2; I++)
        {
            UserItemGrid(I).Cells[0, 0] = "物品位置";
            UserItemGrid(I).Cells[1, 0] = "物品ID";
            UserItemGrid(I).Cells[2, 0] = "物品号";
            UserItemGrid(I).Cells[3, 0] = "持久";
            UserItemGrid(I).Cells[4, 0] = "物品名称";
            UserItemGrid(I).Cells[0, 1] = "衣服";
            UserItemGrid(I).Cells[0, 2] = "武器";
            UserItemGrid(I).Cells[0, 3] = "照明物";
            UserItemGrid(I).Cells[0, 4] = "项链";
            UserItemGrid(I).Cells[0, 5] = "头盔";
            UserItemGrid(I).Cells[0, 6] = "左手镯";
            UserItemGrid(I).Cells[0, 7] = "右手镯";
            UserItemGrid(I).Cells[0, 8] = "左戒指";
            UserItemGrid(I).Cells[0, 9] = "右戒指";
            UserItemGrid(I).Cells[0, 10] = "物品";
            UserItemGrid(I).Cells[0, 11] = "腰带";
            UserItemGrid(I).Cells[0, 12] = "鞋子";
            UserItemGrid(I).Cells[0, 13] = "宝石";
            UserItemGrid(I).Cells[0, 14] = "斗笠";
            // ★ 原文 :202 —— DFM 的 UserItemGrid1/2 `RowCount = 14` ⇒ 这一句靠
            //   TStringGrid 的**自动扩容**把行数涨到 16（UserItemGrid0 的 DFM 就是 16）。
            UserItemGrid(I).Cells[0, 15] = "军鼓";
        }
    }

    // ==================================================================
    // :206-218 sub_49A9DC
    // ==================================================================

    /// <summary>原文 `:206-218 procedure TFrmHeroFDBViewer.sub_49A9DC();`。</summary>
    public void sub_49A9DC()
    {
        for (int I = 0; I <= 2; I++)
        {
            BagItemGrid(I).Cells[0, 0] = "物品号";
            BagItemGrid(I).Cells[1, 0] = "物品ID";
            BagItemGrid(I).Cells[2, 0] = "物品号";
            BagItemGrid(I).Cells[3, 0] = "持久";
            BagItemGrid(I).Cells[4, 0] = "物品名称";
        }
    }

    // ==================================================================
    // :220-237 sub_49AB10
    // ==================================================================

    /// <summary>原文 `:220-237 procedure TFrmHeroFDBViewer.sub_49AB10();`。</summary>
    public void sub_49AB10()
    {
        for (int I = 0; I <= 2; I++)
        {
            UseMagicGrid(I).Cells[0, 0] = "技能ID";
            UseMagicGrid(I).Cells[1, 0] = "快捷键";
            UseMagicGrid(I).Cells[2, 0] = "修练状态";
            UseMagicGrid(I).Cells[3, 0] = "技能名称";
            // ★ 原文 :230 —— DFM 的 UseMagicGrid1/2 `ColCount = 4` ⇒ 这一句靠自动扩容涨到 5。
            UseMagicGrid(I).Cells[4, 0] = "技能类型";
        }
        // 连击表在循环**之外**（原文 :232-236），且与循环体逐字相同
        UseSuccessiveMagicGrid.Cells[0, 0] = "技能ID";
        UseSuccessiveMagicGrid.Cells[1, 0] = "快捷键";
        UseSuccessiveMagicGrid.Cells[2, 0] = "修练状态";
        UseSuccessiveMagicGrid.Cells[3, 0] = "技能名称";
        UseSuccessiveMagicGrid.Cells[4, 0] = "技能类型";
    }

    // ==================================================================
    // :239-257 ShowBagItem
    // ==================================================================

    /// <summary>
    /// 原文 `:239-257 procedure TFrmHeroFDBViewer.ShowBagItem(nIndex, nJob: Integer; sName: string; Item: TUserItem);`。
    /// </summary>
    public void ShowBagItem(int nIndex, int nJob, string sName, GXX.Core.Protocol.TUserItem Item)
    {
        // 原文 if Item.wIndex > 0 then（wIndex: Word ⇒ 无符号比较）
        if (Item.wIndex > 0)
        {
            // ★ 与 ShowUserItem 不对称：本方法**两个分支都写第 0 列**（原文如此）
            BagItemGrid(nJob).Cells[0, nIndex] = sName;
            BagItemGrid(nJob).Cells[1, nIndex] = GXX.Core.Rtl.DelphiRTL.IntToStr(Item.MakeIndex);
            BagItemGrid(nJob).Cells[2, nIndex] = GXX.Core.Rtl.DelphiRTL.IntToStr(Item.wIndex);
            BagItemGrid(nJob).Cells[3, nIndex] =
                GXX.Core.Rtl.DelphiRTL.IntToStr(Item.Dura) + "/" + GXX.Core.Rtl.DelphiRTL.IntToStr(Item.DuraMax);
            BagItemGrid(nJob).Cells[4, nIndex] = GetStdItemNameHandler(Item.wIndex);
        }
        else
        {
            BagItemGrid(nJob).Cells[0, nIndex] = sName;
            BagItemGrid(nJob).Cells[1, nIndex] = "";
            BagItemGrid(nJob).Cells[2, nIndex] = "";
            BagItemGrid(nJob).Cells[3, nIndex] = "";
            BagItemGrid(nJob).Cells[4, nIndex] = "";
        }
    }

    // ==================================================================
    // :259-275 ShowUserItem
    // ==================================================================

    /// <summary>
    /// 原文 `:259-275 procedure TFrmHeroFDBViewer.ShowUserItem(nIndex, nJob: Integer; sName: string; Item: TUserItem);`。
    /// <para>★ 与 `ShowBagItem` 不对称：本方法**从不写第 0 列**（原文如此，逐字保留）。</para>
    /// </summary>
    public void ShowUserItem(int nIndex, int nJob, string sName, GXX.Core.Protocol.TUserItem Item)
    {
        if (Item.wIndex > 0)
        {
            UserItemGrid(nJob).Cells[1, nIndex] = GXX.Core.Rtl.DelphiRTL.IntToStr(Item.MakeIndex);
            UserItemGrid(nJob).Cells[2, nIndex] = GXX.Core.Rtl.DelphiRTL.IntToStr(Item.wIndex);
            UserItemGrid(nJob).Cells[3, nIndex] =
                GXX.Core.Rtl.DelphiRTL.IntToStr(Item.Dura) + "/" + GXX.Core.Rtl.DelphiRTL.IntToStr(Item.DuraMax);
            UserItemGrid(nJob).Cells[4, nIndex] = GetStdItemNameHandler(Item.wIndex);
        }
        else
        {
            UserItemGrid(nJob).Cells[1, nIndex] = "";
            UserItemGrid(nJob).Cells[2, nIndex] = "";
            UserItemGrid(nJob).Cells[3, nIndex] = "";
            UserItemGrid(nJob).Cells[4, nIndex] = "";
        }
    }

    // ==================================================================
    // :277-335 ShowHumanInfo —— **整段被 (* *) 注释掉 ⇒ 空体**
    // ==================================================================

    /// <summary>
    /// 原文 `:277-335 procedure TFrmHeroFDBViewer.ShowHumanInfo();` —— 函数体**整段**在
    /// `(* ... *)` 里 ⇒ Delphi 侧是**空过程**。原文内容逐字保留在下方注释中（不进托管代码）。
    /// <code>
    /// (*
    /// var
    ///   HumData: pTHeroDataPublic;
    /// begin
    ///   HumData := @HeroRecord.PublicData;
    ///   HumanGrid.Cells[0, 2] := IntToStr(n2F8);
    ///   HumanGrid.Cells[1, 2] := HumData.sChrName;
    ///   ... （:284-333，共 50 条赋值）
    ///   HumanGrid.Cells[11, 8] := IntToStr(HumData.Alcohol.WineDrinkValue);
    /// *)
    /// </code>
    /// </summary>
    public void ShowHumanInfo()
    {
        // 原文空体（整段被注释掉）。逐字保留原文注释见 XML 摘要与本文件头。
    }

    // ==================================================================
    // :337-360 ShowBagItems —— **整段被 (* *) 注释掉 ⇒ 空体**
    // ==================================================================

    /// <summary>
    /// 原文 `:337-360 procedure TFrmHeroFDBViewer.ShowBagItems();` —— 空体
    /// （`var I, II, III: Integer;` 三个局部变量在 Delphi 侧**未使用**，整段被注释掉）。
    /// </summary>
    public void ShowBagItems()
    {
        // 原文空体（整段被注释掉）。
    }

    // ==================================================================
    // :362-395 ShowUserItems —— **整段被 (* *) 注释掉 ⇒ 空体**
    // ==================================================================

    /// <summary>原文 `:362-395 procedure TFrmHeroFDBViewer.ShowUserItems();` —— 空体（整段被注释掉）。</summary>
    public void ShowUserItems()
    {
        // 原文空体（整段被注释掉）。
    }

    // ==================================================================
    // :397-451 ShowUseMagic —— **整段被 (* *) 注释掉 ⇒ 空体**
    // ==================================================================

    /// <summary>原文 `:397-451 procedure TFrmHeroFDBViewer.ShowUseMagic();` —— 空体（整段被注释掉）。</summary>
    public void ShowUseMagic()
    {
        // 原文空体（整段被注释掉）。
    }

    // ==================================================================
    // 内部：Delphi `TPageControl.ActivePageIndex := X` 的静默越界语义（§21.3）
    // ==================================================================

    /// <summary>
    /// 复刻 Delphi `TPageControl.ActivePageIndex := X`：越界**静默置 -1**（WinForms 抛异常）。
    /// </summary>
    public static void SetActivePageIndex(System.Windows.Forms.TabControl pageControl, int value)
    {
        if (value < -1 || value >= pageControl.TabPages.Count)
        {
            pageControl.SelectedIndex = -1;
            return;
        }
        pageControl.SelectedIndex = value;
    }
}

/// <summary>
/// 原文 `ViewHeroRcd.pas:67 var FrmHeroFDBViewer: TFrmHeroFDBViewer;`（单元级全局窗体变量）。
/// <para>生命周期由调用方持有（本单元内 0 处创建/释放；全树 .pas 里也无可执行调用点）。</para>
/// </summary>
public static class Sweep9FormsHeroRcdGlobals
{
    /// <summary>原文 `FrmHeroFDBViewer`（未创建时为 <c>null</c> == 原文 nil）。</summary>
    public static TFrmHeroFDBViewer? FrmHeroFDBViewer;

    /// <summary>测试隔离。</summary>
    public static void Reset() => FrmHeroFDBViewer = null;
}
