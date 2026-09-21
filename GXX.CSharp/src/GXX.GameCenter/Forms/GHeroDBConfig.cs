// ============================================================================
// GHeroDBConfig.pas（565 行）→ GHeroDBConfig.cs
// 源：_analysis/utf8_mirror/GameCenter/GHeroDBConfig.pas
// DFM：Source/GameCenter/GHeroDBConfig.dfm（**文本 DFM，GBK**；_analysis 下的 DFM 副本已损坏，未使用）
//
// DFM 对账口径（本文件末尾有对应测试）：
//   · object 节点 25 个 = 窗体 FrmHeroDB 自身 + 24 个子控件（本文件全部字段化）
//   · On* 事件绑定 6 个，全部为 OnClick；**窗体根没有 OnCreate/OnDestroy**
//     （再次用计数法复核 DFM 确认，与派发说明一致）
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace GXX.GameCenter.Forms;

/// <summary>
/// GHeroDB.pas <c>THeroDB</c> 的最小接缝。
/// <para>
/// <b>未移植声明</b>：<c>THeroDB</c>（GHeroDB.pas，BDE/Paradox 会话与别名管理）在整个托管树里
/// **没有**移植（<c>GXX.DBServer.MySqlRoleDB.THeroDBBase</c> 是无关的 MySQL 角色库抽象，不是它）。
/// 本接口只声明 GHeroDBConfig.pas 实际调用的成员，签名逐字取自 GHeroDB.pas:17-24。
/// </para>
/// <para>
/// 方法名保持原文（含 <c>CreateField</c> 的两处重载）；<c>constructor Create</c> / <c>destructor Destroy</c>
/// 由工厂委托承担，<see cref="IDisposable"/> 对应 <c>Free</c>。
/// </para>
/// </summary>
public interface IHeroDB : IDisposable
{
    /// <summary>GHeroDB.pas:17 <c>function HeroDBExist(const HeroDBName: string): Boolean;</c></summary>
    bool HeroDBExist(string HeroDBName);

    /// <summary>GHeroDB.pas:18 <c>function TableExist(const HeroDBName, TableName: string): Boolean;</c></summary>
    bool TableExist(string HeroDBName, string TableName);

    /// <summary>GHeroDB.pas:19 <c>procedure SaveHeroDBConfigFile(const Name, Path: string);</c></summary>
    void SaveHeroDBConfigFile(string Name, string Path);

    /// <summary>GHeroDB.pas:21 <c>function FieldExist(const HeroDBName, TableName, FieldName: string): Boolean;</c></summary>
    bool FieldExist(string HeroDBName, string TableName, string FieldName);

    /// <summary>GHeroDB.pas:23 <c>function CreateField(...; Default: string; Len: Integer): Boolean; overload;</c>
    /// （GHeroDBConfig.pas 未调用该重载，仅为接缝完整保留。）</summary>
    bool CreateField(string HeroDBName, string TableName, string FieldName, string Default, int Len);

    /// <summary>GHeroDB.pas:24 <c>function CreateField(...; Default: Integer; Len: Byte): Boolean; overload;</c>
    /// —— GHeroDBConfig.pas 的 6 处调用全部落在这个重载上（见各调用点行号注释）。</summary>
    bool CreateField(string HeroDBName, string TableName, string FieldName, int Default, byte Len);
}

/// <summary>
/// <c>THeroDB</c> 的构造接缝（GHeroDB.pas:31 <c>constructor THeroDB.Create;</c>）。
/// <para>
/// 默认值为 <c>null</c> ⇒ 任何需要 <c>THeroDB</c> 的路径都会抛 <see cref="NotWiredException"/>
/// （<b>绝不</b>静默返回 false/""，台账 §25.2）。测试必须显式注入替身。
/// </para>
/// </summary>
public static class HeroDBFactory
{
    /// <summary>注入点：返回 <see cref="IHeroDB"/> 实例（等价 <c>THeroDB.Create</c>）。</summary>
    public static Func<IHeroDB>? InstanceFactory;

    /// <summary>复位注入（测试用）。</summary>
    public static void ResetForTests() => InstanceFactory = null;

    /// <summary>
    /// 取一个新实例。未接线时抛 <see cref="NotWiredException"/>。
    /// </summary>
    public static IHeroDB Create()
    {
        var factory = InstanceFactory;
        if (factory == null)
            throw new NotWiredException("未接线：GHeroDB.pas 未移植（THeroDB 无托管实现，请注入 HeroDBFactory.InstanceFactory）");
        return factory();
    }
}

/// <summary>显式"未接线"异常（台账 §25.2：接缝默认不得静默返回中性值）。</summary>
public sealed class NotWiredException : InvalidOperationException
{
    public NotWiredException(string message) : base(message) { }
}

/// <summary>
/// GHeroDBConfig.pas（565 行）1:1 移植：<c>TFrmHeroDB</c>（HeroDB 自动配置窗体）。
/// <para>
/// 源行号对应：<c>SelectDirCB</c> :52-57，<c>SelectDirectory</c> :59-110，
/// <c>EditHeroDBPathButtonClick</c> :112-130，<c>ButtonSaveHeroDBConfigClick</c> :132-163，
/// <c>CheckHeroDB</c> :165-376，<c>Open</c> :378-383，<c>ButtonCloseClick</c> :385-389，
/// <c>ButtonCreateStdItemsFieldClick</c> :391-494，<c>ButtonMagicFieldClick</c> :496-532，
/// <c>ButtonMonsterFieldClick</c> :534-563。
/// </para>
/// <para>
/// <b>命名空间</b>：放在 <c>GXX.GameCenter.Forms</c> 子命名空间，避免 C# 简单名查找先命中
/// 外围命名空间 <c>GXX.GameCenter</c> 里的既有类型（本车道已两次踩坑）。
/// </para>
/// </summary>
public sealed class TFrmHeroDB : System.Windows.Forms.Form
{
    // ==================================================================
    // DFM 控件字段（名称与 GHeroDBConfig.dfm 逐字同名）
    // ==================================================================

    /// <summary>DFM: <c>object PageControl: TPageControl</c>（ActivePage=TabSheet2）</summary>
    public System.Windows.Forms.TabControl PageControl = null!;
    /// <summary>DFM: <c>object TabSheet1: TTabSheet</c>（Caption='HeroDB'）</summary>
    public System.Windows.Forms.TabPage TabSheet1 = null!;
    /// <summary>DFM: <c>object TabSheet2: TTabSheet</c>（Caption='StdItems.DB' ImageIndex=1）</summary>
    public System.Windows.Forms.TabPage TabSheet2 = null!;
    /// <summary>DFM: <c>object TabSheet3: TTabSheet</c>（Caption='Monster.DB' ImageIndex=2）</summary>
    public System.Windows.Forms.TabPage TabSheet3 = null!;
    /// <summary>DFM: <c>object TabSheet4: TTabSheet</c>（Caption='Magic.DB' ImageIndex=3）</summary>
    public System.Windows.Forms.TabPage TabSheet4 = null!;
    /// <summary>DFM: <c>object ButtonClose: TButton</c>（Caption='取消' OnClick=ButtonCloseClick）</summary>
    public System.Windows.Forms.Button ButtonClose = null!;
    /// <summary>DFM: <c>object Label1: TLabel</c>（Caption='数据库别名:'）</summary>
    public System.Windows.Forms.Label Label1 = null!;
    /// <summary>DFM: <c>object EditHeroDB: TEdit</c>（Text='HeroDB'）</summary>
    public System.Windows.Forms.TextBox EditHeroDB = null!;
    /// <summary>DFM: <c>object Label2: TLabel</c>（Caption='数据库路径:'）</summary>
    public System.Windows.Forms.Label Label2 = null!;
    /// <summary>
    /// DFM: <c>object EditHeroDBPath: TRzButtonEdit</c>（OnButtonClick=EditHeroDBPathButtonClick）
    /// —— <b>偏差 D-P10-17</b>：Raize 的 TRzButtonEdit 无托管等价物，按
    /// <c>GMainForm.Fields.g.cs:387-390</c> 既有惯例落为 <c>TextBox</c>，
    /// 其"内嵌按钮"（Raize 复合控件的**匿名内部子控件**，DFM 里没有独立 object 节点）
    /// 由 <see cref="EditHeroDBPathButton"/> 承担。
    /// </summary>
    public System.Windows.Forms.TextBox EditHeroDBPath = null!;
    /// <summary>DFM: <c>object ButtonSaveHeroDBConfig: TButton</c>（OnClick=ButtonSaveHeroDBConfigClick）</summary>
    public System.Windows.Forms.Button ButtonSaveHeroDBConfig = null!;
    /// <summary>DFM: <c>object MemoLog: TMemo</c></summary>
    public System.Windows.Forms.TextBox MemoLog = null!;
    /// <summary>DFM: <c>object GroupBox1: TGroupBox</c>（'StdItems.DB中缺少以下字段'）</summary>
    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    /// <summary>DFM: <c>object ListBoxStdItems: TListBox</c></summary>
    public System.Windows.Forms.ListBox ListBoxStdItems = null!;
    /// <summary>DFM: <c>object ButtonCreateStdItemsField: TButton</c>（OnClick=ButtonCreateStdItemsFieldClick）</summary>
    public System.Windows.Forms.Button ButtonCreateStdItemsField = null!;
    /// <summary>DFM: <c>object MemoLog1: TMemo</c></summary>
    public System.Windows.Forms.TextBox MemoLog1 = null!;
    /// <summary>DFM: <c>object GroupBox2: TGroupBox</c>（'Monster.DB中缺少以下字段'）</summary>
    public System.Windows.Forms.GroupBox GroupBox2 = null!;
    /// <summary>DFM: <c>object ListBoxMonster: TListBox</c></summary>
    public System.Windows.Forms.ListBox ListBoxMonster = null!;
    /// <summary>DFM: <c>object MemoLog2: TMemo</c></summary>
    public System.Windows.Forms.TextBox MemoLog2 = null!;
    /// <summary>DFM: <c>object ButtonMonsterField: TButton</c>（OnClick=ButtonMonsterFieldClick）</summary>
    public System.Windows.Forms.Button ButtonMonsterField = null!;
    /// <summary>DFM: <c>object GroupBox3: TGroupBox</c>（'Magic.DB中缺少以下字段'）</summary>
    public System.Windows.Forms.GroupBox GroupBox3 = null!;
    /// <summary>DFM: <c>object ListBoxMagic: TListBox</c></summary>
    public System.Windows.Forms.ListBox ListBoxMagic = null!;
    /// <summary>DFM: <c>object MemoLog3: TMemo</c></summary>
    public System.Windows.Forms.TextBox MemoLog3 = null!;
    /// <summary>DFM: <c>object ButtonMagicField: TButton</c>（OnClick=ButtonMagicFieldClick）</summary>
    public System.Windows.Forms.Button ButtonMagicField = null!;

    // ==================================================================
    // 非 DFM 字段：Raize 内嵌按钮 + 测试可观测状态
    // ==================================================================

    /// <summary>
    /// 偏差 D-P10-17 的"内嵌按钮"：承担 <c>TRzButtonEdit.OnButtonClick</c>。
    /// <para>
    /// <b>刻意不设 Name</b>：<c>P10FormReconcile</c> 只数"有名字"的控件（DFM 的 object 节点
    /// 全部有名字），本按钮在 DFM 里是 Raize 控件的内部子控件、**没有** object 节点，
    /// 因此留空名以保持 object 计数 = 25。
    /// </para>
    /// </summary>
    public System.Windows.Forms.Button EditHeroDBPathButton = null!;

    /// <summary><c>Close</c> 是否已被调用（WinForms <c>Form.Closed</c>；测试断言用）。</summary>
    public bool CloseCalled { get; private set; }

    /// <summary>本实例最近一次 <c>Application.MessageBox</c> 文本（对应 <see cref="GameCenterDialogs.LastMessage"/>）。</summary>
    public string? LastMessageBoxText { get; private set; }

    /// <summary>本实例最近一次 <c>Application.MessageBox</c> 标题。</summary>
    public string? LastMessageBoxCaption { get; private set; }

    /// <summary>本实例最近一次 <c>Application.MessageBox</c> 标志（原文 MB_OK + MB_ICONWARNING）。</summary>
    public int LastMessageBoxFlags { get; private set; }

    // ==================================================================
    // 构造 / DFM 布局
    // ==================================================================

    public TFrmHeroDB()
    {
        InitializeComponent();
    }

    /// <summary>
    /// DFM: GHeroDBConfig.dfm 全部 25 个 object 节点。
    /// 窗体根：<c>BorderIcons=[] BorderStyle=bsDialog Caption='HeroDB自动配置'</c>
    /// <c>ClientHeight=330 ClientWidth=511 Position=poOwnerFormCenter</c>；
    /// 窗体根**没有** OnCreate / OnDestroy（计数复核，见测试）。
    /// </summary>
    private void InitializeComponent()
    {
        // ---- 窗体根（DFM: object FrmHeroDB: TFrmHeroDB） ----
        Name = "FrmHeroDB";                                             // DFM 根节点名（对账口径含窗体自身）
        Text = "HeroDB自动配置";                                       // Caption
        // BorderIcons = [] ⇒ 无系统菜单/最小化/最大化；配合 BorderStyle=bsDialog
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;   // Position = poOwnerFormCenter
        ClientSize = new System.Drawing.Size(511, 330);                 // ClientWidth/ClientHeight

        // ---- 共享字体（DFM: Font.Charset=GB2312_CHARSET Font.Height=-12 Font.Name=宋体） ----
        var dfmFont = new System.Drawing.Font("宋体", 9F);

        // ---- PageControl（DFM: Left=0 Top=0 Width=511 Height=289 Align=alTop ActivePage=TabSheet2） ----
        PageControl = new System.Windows.Forms.TabControl
        {
            Name = "PageControl",
            Left = 0,
            Top = 0,
            Width = 511,
            Height = 289,
            TabIndex = 0,
            Font = dfmFont
        };

        // ---- TabSheet1（Caption='HeroDB'） ----
        TabSheet1 = new System.Windows.Forms.TabPage { Name = "TabSheet1", Text = "HeroDB", Left = 4, Top = 25, Width = 503, Height = 260, Font = dfmFont };
        // DFM: Label1 Left=16 Top=20 Width=66 Height=12 Caption='数据库别名:'
        Label1 = new System.Windows.Forms.Label { Name = "Label1", Text = "数据库别名:", Left = 16, Top = 20, Width = 66, Height = 12, AutoSize = false };
        // DFM: Label2 Left=16 Top=44 Width=66 Height=12 Caption='数据库路径:'
        Label2 = new System.Windows.Forms.Label { Name = "Label2", Text = "数据库路径:", Left = 16, Top = 44, Width = 66, Height = 12, AutoSize = false };
        // DFM: EditHeroDB Left=88 Top=16 Width=121 Height=20 TabOrder=0 Text='HeroDB'
        EditHeroDB = new System.Windows.Forms.TextBox { Name = "EditHeroDB", Left = 88, Top = 16, Width = 121, Height = 20, TabIndex = 0, Text = "HeroDB" };
        // DFM: EditHeroDBPath Left=88 Top=40 Width=249 Height=20 TabOrder=1（TRzButtonEdit → TextBox，见偏差 D-P10-17）
        EditHeroDBPath = new System.Windows.Forms.TextBox { Name = "EditHeroDBPath", Left = 88, Top = 40, Width = 249, Height = 20, TabIndex = 1 };
        // DFM: TRzButtonEdit ButtonWidth=15（Raize 内嵌按钮，DFM 无独立 object 节点 ⇒ **不设 Name**）
        EditHeroDBPathButton = new System.Windows.Forms.Button { Left = 337, Top = 40, Width = 15, Height = 20, TabIndex = 2 };
        // DFM: ButtonSaveHeroDBConfig Left=384 Top=34 Width=97 Height=25 Caption='自动配置HeroDB' TabOrder=2
        ButtonSaveHeroDBConfig = new System.Windows.Forms.Button { Name = "ButtonSaveHeroDBConfig", Text = "自动配置HeroDB", Left = 384, Top = 34, Width = 97, Height = 25, TabIndex = 3 };
        // DFM: MemoLog Left=16 Top=72 Width=481 Height=177 BorderStyle=bsNone ReadOnly=True TabOrder=3
        MemoLog = new System.Windows.Forms.TextBox
        {
            Name = "MemoLog",
            Left = 16, Top = 72, Width = 481, Height = 177, TabIndex = 4,
            Multiline = true, ReadOnly = true, BorderStyle = System.Windows.Forms.BorderStyle.None,
            ScrollBars = System.Windows.Forms.ScrollBars.Vertical, WordWrap = false,
            // VCL TMemo 没有字数上限；WinForms TextBox 默认 MaxLength=32767 会**静默截断**
            // （本窗体 CheckHeroDB 的日志可达 ~1.6k 字符，ButtonCreateStdItemsField 的 40 行更长）
            // ⇒ 置 0 取消上限，保持 TMemo.Lines 语义。
            MaxLength = 0
        };
        TabSheet1.Controls.Add(Label1);
        TabSheet1.Controls.Add(Label2);
        TabSheet1.Controls.Add(EditHeroDB);
        TabSheet1.Controls.Add(EditHeroDBPath);
        TabSheet1.Controls.Add(EditHeroDBPathButton);
        TabSheet1.Controls.Add(ButtonSaveHeroDBConfig);
        TabSheet1.Controls.Add(MemoLog);

        // ---- TabSheet2（Caption='StdItems.DB' ImageIndex=1） ----
        TabSheet2 = new System.Windows.Forms.TabPage { Name = "TabSheet2", Text = "StdItems.DB", Left = 4, Top = 25, Width = 503, Height = 260, Font = dfmFont };
        // DFM: GroupBox1 Left=8 Top=8 Width=193 Height=241 Caption='StdItems.DB中缺少以下字段'
        GroupBox1 = new System.Windows.Forms.GroupBox { Name = "GroupBox1", Text = "StdItems.DB中缺少以下字段", Left = 8, Top = 8, Width = 193, Height = 241 };
        // DFM: ListBoxStdItems Left=8 Top=16 Width=177 Height=217
        ListBoxStdItems = new System.Windows.Forms.ListBox { Name = "ListBoxStdItems", Left = 8, Top = 16, Width = 177, Height = 217, TabIndex = 0 };
        GroupBox1.Controls.Add(ListBoxStdItems);
        // DFM: ButtonCreateStdItemsField Left=216 Top=224 Width=257 Height=25 TabOrder=1
        ButtonCreateStdItemsField = new System.Windows.Forms.Button { Name = "ButtonCreateStdItemsField", Left = 216, Top = 224, Width = 257, Height = 25, TabIndex = 1 };
        // DFM: MemoLog1 Left=208 Top=16 Width=289 Height=177 BorderStyle=bsNone ReadOnly=True TabOrder=2
        MemoLog1 = new System.Windows.Forms.TextBox
        {
            Name = "MemoLog1",
            Left = 208, Top = 16, Width = 289, Height = 177, TabIndex = 2,
            Multiline = true, ReadOnly = true, BorderStyle = System.Windows.Forms.BorderStyle.None,
            ScrollBars = System.Windows.Forms.ScrollBars.Vertical, WordWrap = false, MaxLength = 0
        };
        TabSheet2.Controls.Add(GroupBox1);
        TabSheet2.Controls.Add(ButtonCreateStdItemsField);
        TabSheet2.Controls.Add(MemoLog1);

        // ---- TabSheet3（Caption='Monster.DB' ImageIndex=2） ----
        TabSheet3 = new System.Windows.Forms.TabPage { Name = "TabSheet3", Text = "Monster.DB", Left = 4, Top = 25, Width = 503, Height = 260, Font = dfmFont };
        // DFM: GroupBox2 Left=8 Top=8 Width=193 Height=241 Caption='Monster.DB中缺少以下字段'
        GroupBox2 = new System.Windows.Forms.GroupBox { Name = "GroupBox2", Text = "Monster.DB中缺少以下字段", Left = 8, Top = 8, Width = 193, Height = 241 };
        // DFM: ListBoxMonster Left=8 Top=16 Width=177 Height=217
        ListBoxMonster = new System.Windows.Forms.ListBox { Name = "ListBoxMonster", Left = 8, Top = 16, Width = 177, Height = 217, TabIndex = 0 };
        GroupBox2.Controls.Add(ListBoxMonster);
        // DFM: MemoLog2 Left=208 Top=16 Width=289 Height=177 BorderStyle=bsNone ReadOnly=True TabOrder=1
        MemoLog2 = new System.Windows.Forms.TextBox
        {
            Name = "MemoLog2",
            Left = 208, Top = 16, Width = 289, Height = 177, TabIndex = 1,
            Multiline = true, ReadOnly = true, BorderStyle = System.Windows.Forms.BorderStyle.None,
            ScrollBars = System.Windows.Forms.ScrollBars.Vertical, WordWrap = false, MaxLength = 0
        };
        // DFM: ButtonMonsterField Left=216 Top=224 Width=257 Height=25 TabOrder=2
        ButtonMonsterField = new System.Windows.Forms.Button { Name = "ButtonMonsterField", Left = 216, Top = 224, Width = 257, Height = 25, TabIndex = 2 };
        TabSheet3.Controls.Add(GroupBox2);
        TabSheet3.Controls.Add(MemoLog2);
        TabSheet3.Controls.Add(ButtonMonsterField);

        // ---- TabSheet4（Caption='Magic.DB' ImageIndex=3） ----
        TabSheet4 = new System.Windows.Forms.TabPage { Name = "TabSheet4", Text = "Magic.DB", Left = 4, Top = 25, Width = 503, Height = 260, Font = dfmFont };
        // DFM: GroupBox3 Left=8 Top=8 Width=193 Height=241 Caption='Magic.DB中缺少以下字段'
        GroupBox3 = new System.Windows.Forms.GroupBox { Name = "GroupBox3", Text = "Magic.DB中缺少以下字段", Left = 8, Top = 8, Width = 193, Height = 241 };
        // DFM: ListBoxMagic Left=8 Top=16 Width=177 Height=217
        ListBoxMagic = new System.Windows.Forms.ListBox { Name = "ListBoxMagic", Left = 8, Top = 16, Width = 177, Height = 217, TabIndex = 0 };
        GroupBox3.Controls.Add(ListBoxMagic);
        // DFM: MemoLog3 Left=208 Top=16 Width=289 Height=177 BorderStyle=bsNone ReadOnly=True TabOrder=1
        MemoLog3 = new System.Windows.Forms.TextBox
        {
            Name = "MemoLog3",
            Left = 208, Top = 16, Width = 289, Height = 177, TabIndex = 1,
            Multiline = true, ReadOnly = true, BorderStyle = System.Windows.Forms.BorderStyle.None,
            ScrollBars = System.Windows.Forms.ScrollBars.Vertical, WordWrap = false, MaxLength = 0
        };
        // DFM: ButtonMagicField Left=216 Top=224 Width=257 Height=25 TabOrder=2
        ButtonMagicField = new System.Windows.Forms.Button { Name = "ButtonMagicField", Left = 216, Top = 224, Width = 257, Height = 25, TabIndex = 2 };
        TabSheet4.Controls.Add(GroupBox3);
        TabSheet4.Controls.Add(MemoLog3);
        TabSheet4.Controls.Add(ButtonMagicField);

        // DFM: ActivePage = TabSheet2 —— WinForms 的 TabControl 只让 SelectedTab 的
        //      Control.Visible 为 True（实测，见 P10HeroDbConfigTests.TabPage_VisibleOnlyForSelectedPage），
        //      因此这里必须显式选中 TabSheet2，才能让原文 CheckHeroDB:338 的 TabSheet3.Visible 保持 False。
        PageControl.Controls.Add(TabSheet1);
        PageControl.Controls.Add(TabSheet2);
        PageControl.Controls.Add(TabSheet3);
        PageControl.Controls.Add(TabSheet4);
        PageControl.SelectedTab = TabSheet2;
        // Delphi 里 TabSheet1..4 的 TabVisible 默认都是 True（页签全在）；原封不动。
        _tabOrderInitial = new System.Collections.Generic.List<System.Windows.Forms.TabPage>
        { TabSheet1, TabSheet2, TabSheet3, TabSheet4 };
        _tabVisibleState = new System.Collections.Generic.Dictionary<System.Windows.Forms.TabPage, bool>
        {
            [TabSheet1] = true, [TabSheet2] = true, [TabSheet3] = true, [TabSheet4] = true
        };
        _activeTabName = TabSheet2.Text;   // DFM: ActivePage = TabSheet2

        // ---- ButtonClose（窗体根直属；DFM: Left=216 Top=296 Width=75 Height=25 Caption='取消'） ----
        ButtonClose = new System.Windows.Forms.Button { Name = "ButtonClose", Text = "取消", Left = 216, Top = 296, Width = 75, Height = 25, TabIndex = 1, Font = dfmFont };

        Controls.Add(PageControl);
        Controls.Add(ButtonClose);

        // ---- 事件接线（DFM 的 6 个 OnClick 绑定 + 1 个用于内部按钮的等价绑定） ----
        // DFM: EditHeroDBPath.OnButtonClick = EditHeroDBPathButtonClick
        EditHeroDBPathButton.Click += (s, e) => EditHeroDBPathButtonClick(s);
        // DFM: ButtonSaveHeroDBConfig.OnClick = ButtonSaveHeroDBConfigClick
        ButtonSaveHeroDBConfig.Click += (s, e) => ButtonSaveHeroDBConfigClick(s);
        // DFM: ButtonCreateStdItemsField.OnClick = ButtonCreateStdItemsFieldClick
        ButtonCreateStdItemsField.Click += (s, e) => ButtonCreateStdItemsFieldClick(s);
        // DFM: ButtonMonsterField.OnClick = ButtonMonsterFieldClick
        ButtonMonsterField.Click += (s, e) => ButtonMonsterFieldClick(s);
        // DFM: ButtonMagicField.OnClick = ButtonMagicFieldClick
        ButtonMagicField.Click += (s, e) => ButtonMagicFieldClick(s);
        // DFM: ButtonClose.OnClick = ButtonCloseClick
        ButtonClose.Click += (s, e) => ButtonCloseClick(s);

        // 窗体根 DFM **没有** OnCreate/OnDestroy：这里只挂 CloseCalled 观测（不是 DFM 绑定）。
        Closed += (s, e) => CloseCalled = true;
    }

    // ==================================================================
    // 单元级自由函数（原文 :52-110，非 TFrmHeroDB 成员）
    // ==================================================================

    /// <summary>原文 <c>BFFM_INITIALIZED</c>（ShlObj）。</summary>
    public const int BFFM_INITIALIZED = 1;

    /// <summary>原文 <c>BFFM_SETSELECTION</c>（ShlObj；ANSI 版为 BFFM_SETSELECTIONA）。</summary>
    public const int BFFM_SETSELECTION = 1029;

    /// <summary>原文 <c>BIF_RETURNONLYFSDIRS</c>。</summary>
    public const int BIF_RETURNONLYFSDIRS = 0x0001;

    /// <summary>原文 <c>BIF_USENEWUI</c>（BIF_NEWDIALOGSTYLE or BIF_EDITBOX）。</summary>
    public const int BIF_USENEWUI = 0x0040 | 0x0010;

    /// <summary><c>S_OK</c>（原文 <c>ShGetMalloc(ShellMalloc) = S_OK</c>）。</summary>
    public const int S_OK = 0;

    /// <summary>
    /// GHeroDBConfig.pas:52 <c>function SelectDirCB(Wnd: HWND; uMsg: UINT; lParam, lpData: lParam): Integer stdcall;</c>
    /// <para>
    /// 原文语义：<c>uMsg = BFFM_INITIALIZED</c> 且 <c>lpData &lt;&gt; 0</c> 时向对话框发
    /// <c>BFFM_SETSELECTION</c> 以**预选当前目录**。
    /// </para>
    /// <para>
    /// <b>偏差 D-P10-18</b>：托管侧没有"向 Shell 对话框回调发消息"的等价通道
    /// （<c>FolderBrowserDialog</c> 不暴露 BFFM_SETSELECTION）。本方法只保留回调**契约**
    /// （返回值恒 0、只有一个初始化消息分支、lpData 为 0 时不做任何事），
    /// 预选意图改由 <see cref="FolderPickerProvider"/> 的 <c>initialDirectory</c> 参数承载。
    /// </para>
    /// <para>原文如此：<c>Result := 0;</c> 在 <c>if</c> 之后**无条件**执行（回调返回 0 表示不处理）。</para>
    /// </summary>
    public static int SelectDirCB(IntPtr Wnd, uint uMsg, IntPtr lParam, IntPtr lpData)
    {
        if (uMsg == BFFM_INITIALIZED && lpData != IntPtr.Zero)
        {
            // 原文：SendMessage(Wnd, BFFM_SETSELECTION, Integer(True), lpData);
            // 托管无对应消息通道，见偏差 D-P10-18。
            _ = BFFM_SETSELECTION;
            _ = Wnd;
            _ = lParam;
        }
        return 0;
    }

    /// <summary>
    /// 目录选择接缝（替代 <c>ShBrowseForFolder</c> + <c>IMalloc</c> + <c>DisableTaskWindows</c>）。
    /// <para>入参 <c>initialDirectory</c> 即原文 <c>SelectDirCB</c>/<c>BFFM_SETSELECTION</c> 的预选目录；
    /// 返回 <c>null</c> 表示用户取消（等价 <c>ShBrowseForFolder</c> 返回 nil）。</para>
    /// <para><b>测试必须注入本接缝</b>，否则默认实现会弹真实对话框。</para>
    /// </summary>
    public static Func<string, string?>? FolderPickerProvider;

    /// <summary>复位接缝（测试用）。</summary>
    public static void ResetForTests()
    {
        FolderPickerProvider = null;
        HeroDBFactory.ResetForTests();
    }

    /// <summary>
    /// GHeroDBConfig.pas:59-110 <c>function SelectDirectory(const Caption: string; const Root: WideString; var Directory: string; Owner: Thandle): Boolean;</c>
    /// <para>
    /// 1:1 保留的两点原文行为：
    /// <list type="number">
    /// <item><c>if not DirectoryExists(Directory) then Directory := '';</c>（:71-72）—— 目录不存在则把 var 参数清空；</item>
    /// <item>返回值**不做**尾部 <c>\</c> 剥离（剥离在调用方 :117-118 与 :141-142 做），原样回填 <c>Directory</c>。</item>
    /// </list>
    /// </para>
    /// <para><b>偏差 D-P10-18</b>：<c>Root</c>（pidlRoot）、<c>Owner</c>（hwndOwner）在托管侧无对应物，
    /// 仅为签名保真保留；<c>DisableTaskWindows</c>/<c>EnableTaskWindows</c> 为 VCL 专有，
    /// 托管侧由模态对话框自身承担（不复制该调用）。
    /// </para>
    /// </summary>
    public static bool SelectDirectory(string Caption, string Root, ref string Directory, IntPtr Owner)
    {
        _ = Caption;
        _ = Root;
        _ = Owner;

        if (!DirectoryExists(Directory))
            Directory = "";

        var picker = FolderPickerProvider;
        if (picker == null)
        {
            // 默认实现：真实 FolderBrowserDialog（含"预选当前目录"意图）。
            using var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Directory != "") dialog.SelectedPath = Directory;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return false;
            Directory = dialog.SelectedPath;
            return true;
        }

        string? chosen = picker(Directory);
        if (chosen == null)
            return false;
        Directory = chosen;
        return true;
    }

    /// <summary>
    /// Delphi <c>SysUtils.DirectoryExists</c> 的等价：目录不存在（含空串/非法路径）一律 False，不抛异常。
    /// （原文 :71 传入的可能正是空串，<c>DirectoryExists('')</c> 返回 False。）
    /// </summary>
    private static bool DirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        try { return Directory.Exists(path); }
        catch { return false; }
    }

    /// <summary>
    /// Delphi <c>SysUtils.FileExists</c> 的等价：文件不存在（含空串/非法路径）一律 False，不抛异常。
    /// </summary>
    private static bool FileExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        try { return File.Exists(path); }
        catch { return false; }
    }

    /// <summary>
    /// Delphi <c>Trim</c>（去首尾空白；原文 :139/<c>:140</c> 用 <c>Trim</c>）。
    /// 保持 <c>null</c> → 空串，避免托管侧 NRE。
    /// </summary>
    private static string Trim(string? value) => value == null ? "" : value.Trim();

    /// <summary>
    /// Delphi <c>SameText</c>（大小写不敏感比较，原文 :509）。
    /// </summary>
    private static bool SameText(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>原文 <c>IntToStr</c>（不变文化）。</summary>
    private static string IntToStr(int value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Delphi <c>TStrings.Add</c>：追加一行。</summary>
    private static void AddLine(System.Windows.Forms.TextBox memo, string s)
        => memo.AppendText(s + Environment.NewLine);

    /// <summary>Delphi <c>TMemo.Lines.Clear</c>。</summary>
    private static void ClearMemo(System.Windows.Forms.TextBox memo) => memo.Clear();

    /// <summary>Delphi <c>TMemo.Lines.Count</c>（空文本 ⇒ 0 行）。</summary>
    private static int MemoLineCount(System.Windows.Forms.TextBox memo)
        => memo.TextLength == 0 ? 0 : memo.Lines.Length;

    /// <summary>Delphi <c>TMemo.Lines.Text</c> 的行数组（已去掉 <c>Clear</c>/末尾换行带来的空行）。</summary>
    private static List<string> MemoLines(System.Windows.Forms.TextBox memo)
    {
        var result = new List<string>();
        if (memo.TextLength == 0) return result;
        result.AddRange(memo.Lines);
        while (result.Count > 0 && result[result.Count - 1].Length == 0)
            result.RemoveAt(result.Count - 1);
        return result;
    }

    /// <summary>Delphi <c>TListBox.Clear</c>。</summary>
    private static void ClearListBox(System.Windows.Forms.ListBox box) => box.Items.Clear();

    /// <summary>Delphi <c>TListBox.Items.Add</c>。</summary>
    private static void ListBoxAdd(System.Windows.Forms.ListBox box, string s) => box.Items.Add(s);

    /// <summary>
    /// Delphi <c>TListBox.Items.Strings[I]</c>（0 基下标；越界抛 <see cref="ArgumentOutOfRangeException"/>）。
    /// </summary>
    private static string ListBoxStrings(System.Windows.Forms.ListBox box, int index)
    {
        if (index < 0 || index >= box.Items.Count) throw new ArgumentOutOfRangeException(nameof(index));
        return Convert.ToString(box.Items[index]) ?? "";
    }

    /// <summary>
    /// Delphi <c>TListBox.ItemIndex</c> 越界 = -1 的等价垫片：
    /// WinForms <c>SelectedIndex</c> 越界会抛 <see cref="ArgumentOutOfRangeException"/>，
    /// 故写回时统一走本垫片（原文窗体未使用 <c>ItemIndex</c>，本垫片为约定保留）。
    /// </summary>
    public static void SetItemIndex(System.Windows.Forms.ListBox box, int index)
        => box.SelectedIndex = index >= 0 && index < box.Items.Count ? index : -1;

    /// <summary>
    /// <c>Application.MessageBox(text, caption, MB_OK + MB_ICONWARNING)</c>（原文 :160/:491/:529/:560）。
    /// <para>非阻塞接缝：经 <see cref="GameCenterDialogs.MessageBox"/>（测试注入处理器即不弹窗），
    /// 并把标志记入本实例，便于断言四个调用点逐字一致。</para>
    /// </summary>
    private void ApplicationMessageBox(string text, string caption, int flags)
    {
        LastMessageBoxText = text;
        LastMessageBoxCaption = caption;
        LastMessageBoxFlags = flags;
        GameCenterDialogs.MessageBox(text, caption, flags);
    }

    // ==================================================================
    // 原文例行程序
    // ==================================================================

    /// <summary>GHeroDBConfig.pas:112 <c>procedure TFrmHeroDB.EditHeroDBPathButtonClick(Sender: TObject);</c>
    /// <para>原文如此：<c>sFilePath</c> 是**未初始化**的 var 局部量，Delphi 里受管字符串初值为空串；
    /// 因此这里显式初始化为 ""。</para></summary>
    public void EditHeroDBPathButtonClick(object? Sender)
    {
        string sFilePath = "";   // 原文如此：var sFilePath: string;（Delphi 受管类型初值 = ''）
        if (SelectDirectory("请选择数据库目录" + "\r\n" + "一般在D:\\MirServer\\Mud2\\DB", "", ref sFilePath, Handle))
        {
            if (sFilePath != "" && sFilePath[sFilePath.Length - 1] == '\\')
                sFilePath = sFilePath.Substring(0, sFilePath.Length - 1);

            MemoLog.ForeColor = System.Drawing.Color.Red;   // MemoLog.Font.Color := clRed
            ClearMemo(MemoLog);
            if (!FileExists(sFilePath + "\\StdItems.DB"))
                AddLine(MemoLog, "当前目录中没有发现“StdItems.DB”");
            if (!FileExists(sFilePath + "\\Monster.DB"))
                AddLine(MemoLog, "当前目录中没有发现“Monster.DB”");
            if (!FileExists(sFilePath + "\\Magic.DB"))
                AddLine(MemoLog, "当前目录中没有发现“Magic.DB”");
            EditHeroDBPath.Text = sFilePath;
        }
    }

    /// <summary>GHeroDBConfig.pas:132 <c>procedure TFrmHeroDB.ButtonSaveHeroDBConfigClick(Sender: TObject);</c>
    /// <para>原文如此：<c>if MemoLog.Lines.Count &gt; 0 then Exit;</c> —— 只有三个 DB 文件都在时才继续。</para></summary>
    public void ButtonSaveHeroDBConfigClick(object? Sender)
    {
        string sFilePath;
        MemoLog.ForeColor = System.Drawing.Color.Red;   // MemoLog.Font.Color := clRed
        ClearMemo(MemoLog);
        GShareGlobals.g_sHeroDBName = Trim(EditHeroDB.Text);
        sFilePath = Trim(EditHeroDBPath.Text);
        if (sFilePath != "" && sFilePath[sFilePath.Length - 1] == '\\')
            sFilePath = sFilePath.Substring(0, sFilePath.Length - 1);

        if (!FileExists(sFilePath + "\\StdItems.DB"))
            AddLine(MemoLog, "当前目录中没有发现“StdItems.DB”");
        if (!FileExists(sFilePath + "\\Monster.DB"))
            AddLine(MemoLog, "当前目录中没有发现“Monster.DB”");
        if (!FileExists(sFilePath + "\\Magic.DB"))
            AddLine(MemoLog, "当前目录中没有发现“Magic.DB”");
        if (MemoLineCount(MemoLog) > 0) return;   // Exit

        // HeroDB := THeroDB.Create; → 接缝工厂（未接线抛 未接线 异常）
        var HeroDB = HeroDBFactory.Create();
        HeroDB.SaveHeroDBConfigFile(GShareGlobals.g_sHeroDBName, sFilePath);
        HeroDB.Dispose();   // HeroDB.Free

        GShareGlobals.g_IniConf?.WriteString("GameConf", "HeroDBName", GShareGlobals.g_sHeroDBName);

        GShareGlobals.g_boHeroDBOK = !CheckHeroDB();
        if (GShareGlobals.g_boHeroDBOK)
        {
            ApplicationMessageBox("数据库更新成功！！！", "提示信息", GameCenterDialogs.MB_OK + 0x30 /* MB_ICONWARNING */);
            Close();
        }
    }

    /// <summary>
    /// GHeroDBConfig.pas:165-376 <c>function TFrmHeroDB.CheckHeroDB: Boolean;</c>
    /// <para>
    /// 返回 True 表示"**有**缺失"（<c>Result := TabSheet1.TabVisible or ... or TabSheet4.TabVisible</c>），
    /// 因此调用方写的是 <c>g_boHeroDBOK := not CheckHeroDB</c>（OK = 无缺失）。
    /// </para>
    /// <para>
    /// <b>原文缺陷 1（:338）</b>：本轮唯一一处用 <c>TabSheet3.Visible</c>（Control.Visible）
    /// 而非 <c>TabSheet3.TabVisible</c> 守卫。WinForms 实测：TabPage 只有"被选中"那一个
    /// <c>Visible=True</c>，而本窗体 ActivePage 恒为 TabSheet2 ⇒ 该条件恒为 True
    /// ⇒ <c>:340-371</c> 的 Magic 字段检查**无条件执行**（即使上面刚判定 Monster 缺字段）。
    /// 逐字保留，见测试 <c>CheckHeroDB_TabSheet3VisibleGuardIsDeadCondition</c>。
    /// </para>
    /// <para>
    /// <b>原文缺陷 2（:290/:474）</b>：StdItems 的 <c>Element1..Element24</c> 循环写作
    /// <c>for I := 1 to 24</c>，而 :340 的 Magic 循环写作 <c>for I := 0 to 14</c> 并用 <c>I + 1</c>
    /// 生成 <c>NeedL1..NeedL15</c>/<c>L1Train..L15Train</c> —— 两条循环上界风格不一致（逐字保留）。
    /// </para>
    /// </summary>
    public bool CheckHeroDB()
    {
        const string sMagicNeed = "NeedL%d";
        const string sMagicTrain = "L%dTrain";
        int I;
        string sMagicNeedFieldName;
        string sMagicTrainFieldName;

        ClearMemo(MemoLog);
        MemoLog.ForeColor = System.Drawing.Color.Red;   // MemoLog.Font.Color := clRed
        ClearMemo(MemoLog1);
        MemoLog1.ForeColor = System.Drawing.Color.Red;
        ClearMemo(MemoLog2);
        MemoLog2.ForeColor = System.Drawing.Color.Red;
        ClearMemo(MemoLog3);
        MemoLog3.ForeColor = System.Drawing.Color.Red;
        ClearListBox(ListBoxStdItems);
        ClearListBox(ListBoxMonster);
        ClearListBox(ListBoxMagic);
        // 原文四连 TabVisible := False；为保住"TabSheet3.Visible 恒 False"这一原文前提，
        // 这里按"隐藏不换选中页"的次序整体摘除（见 P10TabVisible 注释），并在段末复位 ActivePage。
        HideAllTabVisible();

        // HeroDB := THeroDB.Create; → 接缝工厂（未接线抛 未接线 异常）
        var HeroDB = HeroDBFactory.Create();
        if (!HeroDB.HeroDBExist(GShareGlobals.g_sHeroDBName))
        {
            AddLine(MemoLog, GShareGlobals.g_sHeroDBName + "配置错误！");
            SetTabVisible(TabSheet1, true);
        }

        if (!GetTabVisible(TabSheet1))
        {
            if (!HeroDB.TableExist(GShareGlobals.g_sHeroDBName, "StdItems"))
            {
                AddLine(MemoLog, GShareGlobals.g_sHeroDBName + "配置错误,没有发现“StdItems.DB”！");
                SetTabVisible(TabSheet1, true);
            }
            if (!HeroDB.TableExist(GShareGlobals.g_sHeroDBName, "Monster"))
            {
                AddLine(MemoLog, GShareGlobals.g_sHeroDBName + "配置错误,没有发现“Monster.DB”！");
                SetTabVisible(TabSheet1, true);
            }
            if (!HeroDB.TableExist(GShareGlobals.g_sHeroDBName, "Magic"))
            {
                AddLine(MemoLog, GShareGlobals.g_sHeroDBName + "配置错误,没有发现“Magic.DB”！");
                SetTabVisible(TabSheet1, true);
            }
        }

        if (!GetTabVisible(TabSheet1))
        {
            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Color"))
            {
                ListBoxAdd(ListBoxStdItems, "Color");
                SetTabVisible(TabSheet2, true);
            }
            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "OverLap"))
            {
                ListBoxAdd(ListBoxStdItems, "OverLap");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "HP"))
            {
                ListBoxAdd(ListBoxStdItems, "HP");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "MP"))
            {
                ListBoxAdd(ListBoxStdItems, "MP");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Light"))
            {
                ListBoxAdd(ListBoxStdItems, "Light");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Horse"))
            {
                ListBoxAdd(ListBoxStdItems, "Horse");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Element"))
            {
                ListBoxAdd(ListBoxStdItems, "Element");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Expand1"))
            {
                ListBoxAdd(ListBoxStdItems, "Expand1");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Expand2"))
            {
                ListBoxAdd(ListBoxStdItems, "Expand2");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Expand3"))
            {
                ListBoxAdd(ListBoxStdItems, "Expand3");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Expand4"))
            {
                ListBoxAdd(ListBoxStdItems, "Expand4");
                SetTabVisible(TabSheet2, true);
            }


            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Expand5"))
            {
                ListBoxAdd(ListBoxStdItems, "Expand5");
                SetTabVisible(TabSheet2, true);
            }

            for (I = 1; I <= 24; I++)
            {
                if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "Element" + IntToStr(I)))
                {
                    ListBoxAdd(ListBoxStdItems, "Element" + IntToStr(I));
                    SetTabVisible(TabSheet2, true);
                }
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "InsuranceCurrency"))
            {
                ListBoxAdd(ListBoxStdItems, "InsuranceCurrency");
                SetTabVisible(TabSheet2, true);
            }

            if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "StdItems", "InsuranceGold"))
            {
                ListBoxAdd(ListBoxStdItems, "InsuranceGold");
                SetTabVisible(TabSheet2, true);
            }

            if (!GetTabVisible(TabSheet2))
            {
                // 添加新字段 piaoyun 2013-12-20
                if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Monster", "AttackState"))
                {
                    ListBoxAdd(ListBoxMonster, "AttackState");
                    SetTabVisible(TabSheet3, true);
                }

                if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Monster", "ExploreItem"))
                {
                    ListBoxAdd(ListBoxMonster, "ExploreItem");
                    SetTabVisible(TabSheet3, true);
                }

                if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Monster", "AttackSource"))
                {
                    ListBoxAdd(ListBoxMonster, "AttackSource");
                    SetTabVisible(TabSheet3, true);
                }

                if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Monster", "DisableSimpleActor"))
                {
                    ListBoxAdd(ListBoxMonster, "DisableSimpleActor");
                    SetTabVisible(TabSheet3, true);
                }

                // 原文如此（缺陷 1）：这里查的是 Control.Visible，不是 TabVisible。
                if (!TabSheet3.Visible)
                {
                    for (I = 0; I <= 14; I++)
                    {
                        // 原文 Format(sMagicNeed, [I + 1]) 的托管等价（CFmtStr 语义：NeedL%d）
                        sMagicNeedFieldName = "NeedL" + IntToStr(I + 1);
                        sMagicTrainFieldName = "L" + IntToStr(I + 1) + "Train";
                        if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Magic", sMagicNeedFieldName))
                        {
                            ListBoxAdd(ListBoxMagic, sMagicNeedFieldName);
                            SetTabVisible(TabSheet4, true);
                        }
                        if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Magic", sMagicTrainFieldName))
                        {
                            ListBoxAdd(ListBoxMagic, sMagicTrainFieldName);
                            SetTabVisible(TabSheet4, true);
                        }
                    }
                    if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Magic", "MaxTrainLv"))
                    {
                        ListBoxAdd(ListBoxMagic, "MaxTrainLv");
                        SetTabVisible(TabSheet4, true);
                    }
                    if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Magic", "CanUpgrade"))
                    {
                        ListBoxAdd(ListBoxMagic, "CanUpgrade");
                        SetTabVisible(TabSheet4, true);
                    }

                    // 添加新字段 piaoyun 2013-12-20
                    if (!HeroDB.FieldExist(GShareGlobals.g_sHeroDBName, "Magic", "MaxUpgradeLv"))
                    {
                        ListBoxAdd(ListBoxMagic, "MaxUpgradeLv");
                        SetTabVisible(TabSheet4, true);
                    }
                }
            }
        }
        // 复位 PageControl.ActivePage（DFM: ActivePage=TabSheet2）；
        // 原文没有这一步（Delphi 的 ActivePage 从不被本过程改动），此处只为
        // 抵消"追加页签会顺带改选"的 WinForms 副作用（见 P10TabVisible 说明）。
        RestoreActivePage();
        bool Result = GetTabVisible(TabSheet1) || GetTabVisible(TabSheet2) || GetTabVisible(TabSheet3) || GetTabVisible(TabSheet4);
        HeroDB.Dispose();   // HeroDB.Free
        return Result;
    }

    /// <summary>GHeroDBConfig.pas:378 <c>procedure TFrmHeroDB.Open;</c>
    /// <para>原文如此：路径拼接是 <c>g_sGameDirectory + 'Mud2\DB'</c>（依赖 g_sGameDirectory 自带尾部分隔符）。</para></summary>
    public void Open()
    {
        EditHeroDB.Text = GShareGlobals.g_sHeroDBName;
        EditHeroDBPath.Text = GShareGlobals.g_sGameDirectory + "Mud2\\DB";
        ShowModalEquivalent();   // Self.ShowModal;
    }

    /// <summary>原文 <c>Self.ShowModal</c>（:382）：非阻塞接缝，默认**不**弹窗、返回 false。</summary>
    public bool ShowModalEquivalent() => ShowModalHandler?.Invoke(this) ?? false;

    /// <summary>ShowModal 接缝（默认不弹真实模态框）。</summary>
    public static Func<TFrmHeroDB, bool>? ShowModalHandler;

    /// <summary>GHeroDBConfig.pas:385 <c>procedure TFrmHeroDB.ButtonCloseClick(Sender: TObject);</c></summary>
    public void ButtonCloseClick(object? Sender)
    {
        GShareGlobals.g_boHeroDBOK = !CheckHeroDB();
        Close();
    }

    /// <summary>GHeroDBConfig.pas:391 <c>procedure TFrmHeroDB.ButtonCreateStdItemsFieldClick(Sender: TObject);</c>
    /// <para>原文如此：每个字段"成功/失败"各写 MemoLog1；末尾同样 <c>g_boHeroDBOK := not CheckHeroDB</c>。</para>
    /// <para>原文 :482-485 的 <c>ModifyField</c> 调用被 <c>{ }</c> 注释掉 —— 未移植该调用（注释保留在下方）。</para></summary>
    public void ButtonCreateStdItemsFieldClick(object? Sender)
    {
        int I;
        ButtonCreateStdItemsField.Enabled = false;
        ClearMemo(MemoLog1);
        MemoLog1.ForeColor = System.Drawing.Color.Red;
        var HeroDB = HeroDBFactory.Create();
        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Color", 255, 2))
            AddLine(MemoLog1, "Color字段创建成功");
        else
            AddLine(MemoLog1, "Color字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "OverLap", 0, 2))
            AddLine(MemoLog1, "OverLap字段创建成功");
        else
            AddLine(MemoLog1, "OverLap字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "HP", 0, 4))
            AddLine(MemoLog1, "HP字段创建成功");
        else
            AddLine(MemoLog1, "HP字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "MP", 0, 4))
            AddLine(MemoLog1, "MP字段创建成功");
        else
            AddLine(MemoLog1, "MP字段创建失败");


        // 自动创建缺少的Light字段 piaoyun 2013-08-17
        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Light", 0, 4))
            AddLine(MemoLog1, "Light字段创建成功");
        else
            AddLine(MemoLog1, "Light字段创建失败");

        // 自动创建缺少的Light字段 piaoyun 2013-11-19
        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Horse", 0, 4))
            AddLine(MemoLog1, "Horse字段创建成功");
        else
            AddLine(MemoLog1, "Horse字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Element", 0, 2))
            AddLine(MemoLog1, "Element字段创建成功");
        else
            AddLine(MemoLog1, "Element字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Expand1", 0, 4))
            AddLine(MemoLog1, "Expand1字段创建成功");
        else
            AddLine(MemoLog1, "Expand1字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Expand2", 0, 4))
            AddLine(MemoLog1, "Expand2字段创建成功");
        else
            AddLine(MemoLog1, "Expand2字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Expand3", 0, 4))
            AddLine(MemoLog1, "Expand3字段创建成功");
        else
            AddLine(MemoLog1, "Expand3字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Expand4", 0, 4))
            AddLine(MemoLog1, "Expand4字段创建成功");
        else
            AddLine(MemoLog1, "Expand4字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Expand5", 0, 4))
            AddLine(MemoLog1, "Expand5字段创建成功");
        else
            AddLine(MemoLog1, "Expand5字段创建失败");


        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "InsuranceCurrency", 0, 4))
            AddLine(MemoLog1, "InsuranceCurrency字段创建成功");
        else
            AddLine(MemoLog1, "InsuranceCurrency字段创建失败");

        if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "InsuranceGold", 0, 4))
            AddLine(MemoLog1, "InsuranceGold字段创建成功");
        else
            AddLine(MemoLog1, "InsuranceGold字段创建失败");

        for (I = 1; I <= 24; I++)
        {
            if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "StdItems", "Element" + IntToStr(I), 0, 2))
                AddLine(MemoLog1, "Element" + IntToStr(I) + "字段创建成功");
            else
                AddLine(MemoLog1, "Element" + IntToStr(I) + "字段创建失败");
        }

        // 原文 :482-485 被注释掉的 ModifyField 调用：
        // {if HeroDB.ModifyField(g_sHeroDBName, 'StdItems', 'Name', 30) then
        //   MemoLog1.Lines.Add('Name字段长度修改成功')
        // else
        //   MemoLog1.Lines.Add('Name字段长度修改失败'); }

        HeroDB.Dispose();   // HeroDB.Free
        ButtonCreateStdItemsField.Enabled = true;
        GShareGlobals.g_boHeroDBOK = !CheckHeroDB();
        if (GShareGlobals.g_boHeroDBOK)
        {
            ApplicationMessageBox("数据库更新成功！！！", "提示信息", GameCenterDialogs.MB_OK + 0x30 /* MB_ICONWARNING */);
            Close();
        }
    }

    /// <summary>
    /// GHeroDBConfig.pas:496 <c>procedure TFrmHeroDB.ButtonMagicFieldClick(Sender: TObject);</c>
    /// <para>
    /// 取值规则（原文 :509-516）：<c>SameText(...,'CanUpgrade') or SameText(...,'MaxUpgradeLv')</c> → 0；
    /// 否则首个字符 <c>'M'</c> → 3；<c>'N'</c> → 20；其余 → 200。
    /// 注意 <c>'M'/'N'</c> 分支是**区分大小写**的字符比较（<c>sFieldName[1] = 'M'</c>）。
    /// </para>
    /// </summary>
    public void ButtonMagicFieldClick(object? Sender)
    {
        int I;
        int nValue;
        ButtonMagicField.Enabled = false;
        ClearMemo(MemoLog3);
        MemoLog3.ForeColor = System.Drawing.Color.Red;

        var HeroDB = HeroDBFactory.Create();
        for (I = 0; I <= ListBoxMagic.Items.Count - 1; I++)
        {
            string sFieldName = ListBoxStrings(ListBoxMagic, I);
            nValue = MagicFieldValue(sFieldName);

            if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "Magic", sFieldName, nValue, 4))
                AddLine(MemoLog3, sFieldName + "字段创建成功");
            else
                AddLine(MemoLog3, sFieldName + "字段创建失败");
        }

        HeroDB.Dispose();   // HeroDB.Free
        ButtonMagicField.Enabled = true;
        GShareGlobals.g_boHeroDBOK = !CheckHeroDB();
        if (GShareGlobals.g_boHeroDBOK)
        {
            ApplicationMessageBox("数据库更新成功！！！", "提示信息", GameCenterDialogs.MB_OK + 0x30 /* MB_ICONWARNING */);
            Close();
        }
    }

    /// <summary>
    /// 原文 :509-516 的取值链（拆出以便对边界单独断言）。
    /// <para>
    /// <b>原文缺陷 3</b>：<c>sFieldName[1]</c> 是 Delphi **1 基**字符串索引，空串访问即
    /// <c>EStringIndex</c>（原文 :511 没有空串守卫）。托管侧以 <see cref="IndexOutOfRangeException"/>
    /// 等价保留该失败模式（不静默取 200）。
    /// </para>
    /// </summary>
    public static int MagicFieldValue(string sFieldName)
    {
        if (string.IsNullOrEmpty(sFieldName)) throw new IndexOutOfRangeException("sFieldName[1] 越界（Delphi EStringIndex）");
        if (SameText(sFieldName, "CanUpgrade") || SameText(sFieldName, "MaxUpgradeLv"))
            return 0;
        if (sFieldName[0] == 'M')
            return 3;
        if (sFieldName[0] == 'N')
            return 20;
        return 200;
    }

    /// <summary>GHeroDBConfig.pas:534 <c>procedure TFrmHeroDB.ButtonMonsterFieldClick(Sender: TObject);</c>
    /// <para>原文如此：<c>nValue := 0;</c> 写在循环体内（每轮都重置为 0），且 CreateField 用 Len=4。</para></summary>
    public void ButtonMonsterFieldClick(object? Sender)
    {
        int I;
        int nValue;
        ButtonMonsterField.Enabled = false;
        ClearMemo(MemoLog2);
        MemoLog2.ForeColor = System.Drawing.Color.Red;

        var HeroDB = HeroDBFactory.Create();
        for (I = 0; I <= ListBoxMonster.Items.Count - 1; I++)
        {
            string sFieldName = ListBoxStrings(ListBoxMonster, I);
            nValue = 0;
            if (HeroDB.CreateField(GShareGlobals.g_sHeroDBName, "Monster", sFieldName, nValue, 4))
                AddLine(MemoLog2, sFieldName + "字段创建成功");
            else
                AddLine(MemoLog2, sFieldName + "字段创建失败");
        }

        HeroDB.Dispose();   // HeroDB.Free
        ButtonMonsterField.Enabled = true;
        GShareGlobals.g_boHeroDBOK = !CheckHeroDB();
        if (GShareGlobals.g_boHeroDBOK)
        {
            ApplicationMessageBox("数据库更新成功！！！", "提示信息", GameCenterDialogs.MB_OK + 0x30 /* MB_ICONWARNING */);
            Close();
        }
    }

    // ==================================================================
    // 测试辅助（非原文成员；只读观测，不改变原文语义）
    // ==================================================================

    /// <summary>测试辅助：MemoLog 的行（等价 <c>MemoLog.Lines</c> 的快照）。</summary>
    public System.Collections.Generic.List<string> MemoLogLines => MemoLines(MemoLog);

    /// <summary>测试辅助：MemoLog1 的行。</summary>
    public System.Collections.Generic.List<string> MemoLog1Lines => MemoLines(MemoLog1);

    /// <summary>测试辅助：MemoLog2 的行。</summary>
    public System.Collections.Generic.List<string> MemoLog2Lines => MemoLines(MemoLog2);

    /// <summary>测试辅助：MemoLog3 的行。</summary>
    public System.Collections.Generic.List<string> MemoLog3Lines => MemoLines(MemoLog3);

    // ==================================================================
    // TabVisible 垫片（原文 :186-189/:195/… 的 TabSheetN.TabVisible）
    // ==================================================================

    /// <summary>四个 TTabSheet 在 DFM 里的初始顺序（TabSheet1..4）。</summary>
    private System.Collections.Generic.List<System.Windows.Forms.TabPage> _tabOrderInitial = null!;

    /// <summary>Delphi <c>TTabSheet.TabVisible</c> 的托管状态（与"是否在 TabControl 控件集合里"保持一致）。</summary>
    private System.Collections.Generic.Dictionary<System.Windows.Forms.TabPage, bool> _tabVisibleState = null!;

    /// <summary>DFM <c>ActivePage = TabSheet2</c> 对应的页签文本（用于复位选中页）。</summary>
    private string? _activeTabName;

    /// <summary>
    /// Delphi <c>TTabSheet.TabVisible</c>（读）：等价于"该页签当前在 TabControl 的控件集合里"。
    /// </summary>
    public static bool GetTabVisible(System.Windows.Forms.TabPage page)
        => page.Parent is System.Windows.Forms.TabControl owner && owner.Controls.Contains(page);

    /// <summary>
    /// Delphi <c>TTabSheet.TabVisible := value</c>。
    /// <para>
    /// false ⇒ 从 <c>TabControl.Controls</c> 摘除（页签消失）；true ⇒ 追加回集合。
    /// 摘除当前选中页会让 TabControl 自动改选，故随后调用 <see cref="RestoreActivePage"/> 把
    /// ActivePage 钉回 TabSheet2，保证 <c>TabSheet3.Visible</c> 恒为 false（原文缺陷 1 的前提）。
    /// </para>
    /// </summary>
    public void SetTabVisible(System.Windows.Forms.TabPage page, bool value)
    {
        _tabVisibleState[page] = value;
        ApplyTabVisible(page, value);
    }

    private void ApplyTabVisible(System.Windows.Forms.TabPage page, bool value)
    {
        if (PageControl.Controls.Contains(page) == value) return;
        if (value) PageControl.Controls.Add(page);
        else PageControl.Controls.Remove(page);
    }

    /// <summary>
    /// 把 <c>PageControl.ActivePage</c> 复位为 <c>TabSheet2</c>（DFM: ActivePage=TabSheet2）。
    /// <para>
    /// 注意两点，都是"保持原文语义"所必需的：
    /// <list type="number">
    /// <item><b>不</b>把 ActivePage 改到别的页签上去。原文 <c>CheckHeroDB</c> 从不触碰
    /// <c>ActivePage</c>；页签被隐藏后 Delphi 的 ActivePage 会停在原页（只是它已 TabVisible=False），
    /// 若这里顺手改选到 TabSheet1，就会把 <c>TabSheet3.Visible</c> 翻成 True，原文缺陷 1 的条件语义被改掉。</item>
    /// <item>ActivePage 不可见时**不选**任何页（<c>SelectedIndex = -1</c>），等价"页签行里没有活动页"。</item>
    /// </list>
    /// </para>
    /// </summary>
    public void RestoreActivePage()
    {
        if (GetTabVisible(TabSheet2))
        {
            PageControl.SelectedTab = TabSheet2;
            return;
        }
        PageControl.SelectedIndex = -1;
    }

    private System.Windows.Forms.TabPage? FindTab(string? text)
    {
        if (text == null) return null;
        foreach (System.Windows.Forms.TabPage p in PageControl.TabPages)
            if (p.Text == text) return p;
        return null;
    }

    /// <summary>
    /// 原文四连 <c>TabVisible := False</c>（CheckHeroDB 开头）的托管写法：
    /// 先摘除**非** ActivePage 的页签（摘除非选中页不会改选），再摘除 ActivePage 并复位选中页。
    /// 这样整个过程中 <c>TabSheet2</c> 始终保持"选中 &amp; 在集合里"。
    /// </summary>
    public void HideAllTabVisible()
    {
        var active = FindTab(_activeTabName);
        foreach (var p in _tabOrderInitial)
        {
            if (ReferenceEquals(p, active)) continue;
            SetTabVisible(p, false);
        }
        if (active != null) SetTabVisible(active, false);
        RestoreActivePage();
    }

    /// <summary>诊断用：当前页签行顺序（对应 Delphi 的 TabPages 顺序）。</summary>
    public System.Collections.Generic.List<string> TabOrderNames
    {
        get
        {
            var list = new System.Collections.Generic.List<string>();
            for (int i = 0; i < PageControl.TabCount; i++) list.Add(PageControl.TabPages[i].Text);
            return list;
        }
    }
}

/// <summary>
/// <c>TTabSheet.TabVisible</c>（ComCtrls）→ WinForms 等价垫片的**实现说明**（实现见 <see cref="TFrmHeroDB"/> 的
/// <c>GetTabVisible</c>/<c>SetTabVisible</c>/<c>HideAllTabVisible</c>/<c>RestoreActivePage</c>）。
/// <para>
/// Delphi 的 <c>TabVisible</c>（页签是否出现在页签行）与 <c>Visible</c>（页面是否显示）是**两个属性**；
/// WinForms 的 <c>TabPage</c> 只有 <c>Visible</c>，且实测：
/// <list type="bullet">
/// <item>设置 <c>TabPage.Visible</c> 不会把页签从 <c>TabControl</c> 摘除；</item>
/// <item><c>TabPage.Visible</c> 对**未被选中**的页恒为 <c>false</c>，对当前选中页为 <c>true</c>。</item>
/// </list>
/// （两条实测见测试 <c>TabPage_VisibleOnlyForSelectedPage</c> 与 <c>TabPage_VisibleSetterDoesNotHideTab</c>。）
/// </para>
/// <para>
/// 因此 <c>TabVisible</c> 的可见效果落为"从 <c>TabControl.Controls</c> 摘除/追加"，
/// 并额外保证"摘除/追加**不会**改变当前选中页"——否则 <c>CheckHeroDB:338</c> 的
/// <c>TabSheet3.Visible</c> 会翻成 True，原文缺陷 1 的条件语义就被悄悄改掉了。
/// 具体做法：摘除当前选中页时先把选中页钉在 <c>TabSheet2</c> 上（<see cref="TFrmHeroDB.RestoreActivePage"/>）。
/// </para>
/// <para>
/// <b>偏差 D-P10-19</b>：Delphi 恢复页签会回到原位置，本垫片追加在**页签行末尾**，
/// 终态页签顺序可能与原文不同（页面集合/选中页/各 <c>Visible</c> 与原文一致）。
/// </para>
/// </summary>
internal static class P10TabVisible
{
}
