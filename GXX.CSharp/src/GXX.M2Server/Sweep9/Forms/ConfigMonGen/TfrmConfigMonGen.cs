// ============================================================================
// 源单元：Source/M2Engine/Forms/ConfigMonGen.pas（60 行，GBK）
// 同源 DFM：Source/M2Engine/Forms/ConfigMonGen.dfm（文本，37 行）
// 类型：TfrmConfigMonGen（:10-19）；单元级全局 `frmConfigMonGen`（:22）
// 方法：ListBoxMonGenDblClick（:32-36）、Open（:38-57）
//
// DFM 实测（:1-36）：
//   窗体 Caption='刷怪配制'（#21047#24618#37197#21046）、Left=989 Top=614、
//   ClientWidth=578 ClientHeight=326、Position=poMainFormCenter、
//   Font.Height=-12 Font.Name=宋体、**无 BorderStyle**（默认 bsSizeable）、
//   **无 OnCreate**。
//   控件 2 个：ListBoxMonGen（TListBox，Align=alTop 578×300，:23 OnDblClick）、
//              pnl（TPanel，Align=alClient 578×26，Caption='双击条目复制到剪贴板'）。
//   事件绑定 **1** 个（ListBoxMonGen.OnDblClick）。
//
// ★ 原文行为要点（已用差异断言锁定，见 tests\Sweep9FormsConfigMonGenTests.cs）：
//   1. `Open`（:38-57）**不清空** ListBoxMonGen ⇒ 同一实例反复 `Open` 会把刷怪条目
//      **累积**（对比同族 `TFrmDummySetting.FormCreate` 是先 Clear 的）—— 原文如此。
//   2. `Open` 的 `try/finally` 里**没有 `Self.Free`**，`ShowModal` 在 finally 之后（:56）
//      ⇒ 模态结束后函数才返回；`Open` 无返回值，`ShowModal` 结果被丢弃。
//   3. `:43-54` 的加锁/解锁**同时**受 `g_MultiThreadRun` 控制，且 LockR(14) 的 id 是 **14**
//      （非 1/pets 那种小号），逐字保留。
//   4. `Items.AddObject(..., TObject(MonGen))` 把 `pTMonGenInfo` 指针塞进 `Objects[]`，
//      但**全单元无任何消费者**（双击用的是 `Items[ItemIndex]` 文本）⇒ 托管侧以
//      `ItemObjects` 平行列表照抄该载体，不省。
// ============================================================================

using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// `pTMonGenInfo`（`Envir.pas` 刷怪模板记录）在托管侧的**接缝载体**。
/// <para>
/// ⚠ 该记录（`Envir.pas` 的 `TMonGenInfo`）**尚未移植**（全树 0 命中），
/// 故本类型只是本单元实际读到的那 4 个字段的接缝载体，
/// 名字带 `Sweep9Forms` 前缀以示**不是**正式移植件（《并行派发台账》§14.2 精神：
/// 不冒充、不重复定义正式类型）。字段名与原文 `sMapName/nX/nY/sMonName` 逐字一致。
/// </para>
/// </summary>
public sealed class Sweep9FormsMonGenInfo
{
    /// <summary>原文 `TMonGenInfo.sMapName: string`。</summary>
    public string sMapName = "";
    /// <summary>原文 `TMonGenInfo.nX: Integer`。</summary>
    public int nX;
    /// <summary>原文 `TMonGenInfo.nY: Integer`。</summary>
    public int nY;
    /// <summary>原文 `TMonGenInfo.sMonName: string`。</summary>
    public string sMonName = "";
}

/// <summary>
/// 原文 `ConfigMonGen.pas:10-19 TfrmConfigMonGen = class(TForm)` 1:1。
/// </summary>
public sealed class TfrmConfigMonGen : System.Windows.Forms.Form
{
    // ------------------------------------------------------------------
    // DFM 控件（层级与顺序即 DFM 顺序）
    // ------------------------------------------------------------------

    /// <summary>DFM :15 `ListBoxMonGen: TListBox`（`Align = alTop`，`OnDblClick = ListBoxMonGenDblClick`）。</summary>
    public System.Windows.Forms.ListBox ListBoxMonGen = null!;

    /// <summary>DFM :26 `pnl: TPanel`（`Align = alClient`，`Caption = '双击条目复制到剪贴板'`）。</summary>
    public System.Windows.Forms.Panel pnl = null!;

    /// <summary>
    /// DFM :32 `pnl.Caption = '双击条目复制到剪贴板'`（`#21452#20987#26465#30446#22797#21046#21040#21049#36148#26495`）。
    /// WinForms `Panel` 无 `Caption` ⇒ 文本存此字段（偏离 **D-P9-03**：不自绘、不加控件，
    /// 以保住控件数 2 / 绑定数 1 的 DFM 对账）。
    /// </summary>
    public string pnlCaption = "双击条目复制到剪贴板";

    /// <summary>
    /// `Items.AddObject(..., TObject(MonGen))`（:49-50）的载体镜像 ——
    /// 与 `ListBoxMonGen.Items` **同序同长**（WinForms `ListBox` 无 `Objects[]`）。
    /// </summary>
    public readonly List<object?> ItemObjects = new();

    // ------------------------------------------------------------------
    // 接缝（原文依赖未移植单元 / 不可测试的副作用）
    // ------------------------------------------------------------------

    /// <summary>
    /// 接缝：`UserEngine.m_MonGenList` 的枚举（:46-51）。
    /// <para>
    /// `m_MonGenList` 的条目类型是 `pTMonGenInfo` ⇒ 返回 <see cref="Sweep9FormsMonGenInfo"/>。
    /// 返回 `null` 视为空表（原文 `Count = 0` ⇒ 循环体不执行）。
    /// 接缝：待 `UsrEngn.pas` / `Envir.pas` 刷怪表移植后接入。
    /// </para>
    /// </summary>
    public Func<IReadOnlyList<Sweep9FormsMonGenInfo>?>? MonGenListHandler;

    /// <summary>接缝：`UserEngine.m_MonGenList.LockR(14)`（:44）。接缝：待 `UsrEngn.pas`。</summary>
    public Action<int>? MonGenListLockR;

    /// <summary>接缝：`UserEngine.m_MonGenList.UnLockR`（:54）。接缝：待 `UsrEngn.pas`。</summary>
    public Action? MonGenListUnLockR;

    /// <summary>接缝：`g_MultiThreadRun`（`M2Threads.pas:59`，初值 False）。接缝：待 `M2Threads.pas`。</summary>
    public bool g_MultiThreadRun;

    /// <summary>
    /// 接缝：`Clipboard.AsText := ...`（:35）。
    /// <para>
    /// 无头环境无剪贴板（且 `Clipboard.SetText` 需 STA + 真实窗口站）⇒ 抽成注入点；
    /// 默认为真实 `System.Windows.Forms.Clipboard.SetText`（生产行为）。
    /// </para>
    /// </summary>
    public Action<string> SetClipboardText = text => System.Windows.Forms.Clipboard.SetText(text);

    // ------------------------------------------------------------------
    // 构造（DFM 控件树 1:1）
    // ------------------------------------------------------------------

    /// <summary>构造 + 装载 DFM 控件树（原文 `OnCreate` 未绑定任何处理器）。</summary>
    public TfrmConfigMonGen()
    {
        InitializeComponents();
    }

    /// <summary>DFM 控件树 1:1 实例化（属性逐条取自 ConfigMonGen.dfm）。</summary>
    private void InitializeComponents()
    {
        // ---- DFM :1-14 窗体自身 ----
        Name = "frmConfigMonGen";                                      // DFM object 名
        Text = "刷怪配制";                                             // DFM Caption（#21047#24618#37197#21046）
        Left = 989;                                                    // DFM Left
        Top = 614;                                                     // DFM Top
        ClientSize = new System.Drawing.Size(578, 326);                 // DFM ClientWidth/ClientHeight
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;  // DFM Position = poMainFormCenter
        // DFM 未写 BorderStyle ⇒ Delphi 默认 bsSizeable ⇒ WinForms 默认 Sizable（不显式设置）
        // DFM Font.Height = -12 / Font.Name = 宋体  ⇒ 9pt 宋体
        Font = new System.Drawing.Font("宋体", 9F);

        // ---- DFM :15-25 ListBoxMonGen ----
        ListBoxMonGen = new System.Windows.Forms.ListBox
        {
            Name = "ListBoxMonGen",                                    // DFM object 名
            Left = 0,
            Top = 0,
            Width = 578,
            Height = 300,
            Dock = System.Windows.Forms.DockStyle.Top,                  // DFM Align = alTop
            IntegralHeight = false,                                     // DFM ItemHeight = 12（非默认 13）
            TabIndex = 0,
        };
        ListBoxMonGen.DoubleClick += (_, _) => ListBoxMonGenDblClick(ListBoxMonGen);
        Controls.Add(ListBoxMonGen);

        // ---- DFM :26-36 pnl ----
        pnl = new System.Windows.Forms.Panel
        {
            Name = "pnl",                                              // DFM object 名
            Left = 0,
            Top = 300,
            Width = 578,
            Height = 26,
            Dock = System.Windows.Forms.DockStyle.Fill,                 // DFM Align = alClient
            TabIndex = 1,
        };
        // TPanel.Caption 在 WinForms 无对应属性。**既不新增控件、也不订阅 Paint**
        // —— 两者都会污染 DFM 对账（前者破坏控件数、后者破坏事件绑定数）。
        // 处置：文本存在 `pnlCaption` 字段（可断言），是否自绘交由宿主适配层决定。
        // 已登记偏离 **D-P9-03**（见 docs\并行报告-p9-m2-forms.md）。
        Controls.Add(pnl);
    }

    /// <summary>
    /// DFM :15-25 `ListBoxMonGen` 的**原始几何**（`Left=0 Top=0 Width=578 Height=300`）。
    /// <para>
    /// ⚠ `Align = alTop` 在 WinForms 由 `Dock` 承载，而 `Dock` 会在布局期**重算**
    /// `Top/Height`（无窗口句柄时塌成 0）⇒ 原文几何不能在控件属性上读到。
    /// 故单独留证；对账用例同时断言 `Dock`（布局意图）与这里的原始值（DFM 属性）。
    /// </para>
    /// </summary>
    public static readonly System.Drawing.Rectangle ListBoxMonGenDfmBounds = new(0, 0, 578, 300);

    /// <summary>DFM :26-36 `pnl` 的原始几何（`Left=0 Top=300 Width=578 Height=26`，`Align = alClient`）。</summary>
    public static readonly System.Drawing.Rectangle pnlDfmBounds = new(0, 300, 578, 26);

    // ------------------------------------------------------------------
    // 原文方法 1:1
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `:32-36 procedure TfrmConfigMonGen.ListBoxMonGenDblClick(Sender: TObject)`。
    /// <para>原文 `Sender` 未被使用（托管侧保留形参以对齐签名）。</para>
    /// </summary>
    public void ListBoxMonGenDblClick(object? Sender)
    {
        // 原文 if ListBoxMonGen.ItemIndex >= 0 then
        if (ListBoxMonGen.SelectedIndex >= 0)
        {
            // 原文 Clipboard.AsText := ListBoxMonGen.Items[ListBoxMonGen.ItemIndex];
            SetClipboardText(ListBoxMonGen.Items[ListBoxMonGen.SelectedIndex].ToString() ?? "");
        }
    }

    /// <summary>
    /// 原文 `:38-57 procedure TfrmConfigMonGen.Open`。
    /// </summary>
    public void Open()
    {
        // 原文 var I: Integer; MonGen: pTMonGenInfo;
        int I;
        Sweep9FormsMonGenInfo MonGen;

        // 原文 if g_MultiThreadRun then UserEngine.m_MonGenList.LockR(14);
        if (g_MultiThreadRun)
            MonGenListLockR?.Invoke(14);
        // 原文 try ... finally if g_MultiThreadRun then UserEngine.m_MonGenList.UnLockR; end;
        try
        {
            // 原文 for I := 0 to UserEngine.m_MonGenList.Count - 1 do
            var list = MonGenListHandler?.Invoke();
            int count = list?.Count ?? 0;                       // 未接线 ⇒ 空表（Count = 0）
            for (I = 0; I < count; I++)
            {
                MonGen = list![I];
                // 原文 ListBoxMonGen.Items.AddObject(MonGen.sMapName + '(' + IntToStr(MonGen.nX) + ':' +
                //   IntToStr(MonGen.nY) + ')' + ' - ' + MonGen.sMonName, TObject(MonGen));
                // ★ 原文**不清空**列表（无 ListBoxMonGen.Clear）⇒ 反复 Open 会累积（原文如此）。
                ListBoxMonGen.Items.Add(MonGen.sMapName + "(" + DelphiRTL.IntToStr(MonGen.nX) + ":" +
                    DelphiRTL.IntToStr(MonGen.nY) + ")" + " - " + MonGen.sMonName);
                ItemObjects.Add(MonGen);                        // Objects[] 载体（原文 AddObject 的第二参）
            }
        }
        finally
        {
            if (g_MultiThreadRun)
                MonGenListUnLockR?.Invoke();
        }
        // 原文 Self.ShowModal;（:56 —— 在 try/finally **之外**）
        Sweep9FormsMessageBoxSeam.ShowModal(this);
    }
}

/// <summary>
/// 原文 `ConfigMonGen.pas:22 var frmConfigMonGen: TfrmConfigMonGen;`（单元级全局窗体变量）。
/// <para>
/// 生命周期由调用方持有：`svMain.pas:2804 frmConfigMonGen := TfrmConfigMonGen.Create(nil);
/// :2806 frmConfigMonGen.Open(); :2808 frmConfigMonGen.Free;`（**不在本单元内**）。
/// </para>
/// </summary>
public static class Sweep9FormsConfigMonGenGlobals
{
    /// <summary>原文 `frmConfigMonGen`（未创建时为 <c>null</c> == 原文 nil）。</summary>
    public static TfrmConfigMonGen? frmConfigMonGen;

    /// <summary>测试隔离。</summary>
    public static void Reset() => frmConfigMonGen = null;
}
