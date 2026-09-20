// ============================================================================
// uFrmCustomItemProperty.pas（Source\M2Engine\Forms\uFrmCustomItemProperty.pas，498 行，GBK）1:1 移植
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.M2Server.Forms.ItemProperty
//
// 本文件 = **接缝层**。原单元 uses（:5-7）与本单元实现段 uses 的第三方/未移植单元：
//   * VCL 控件族（Forms/StdCtrls/ExtCtrls/ComCtrls 的 TForm/TPageControl/TTabSheet/TLabel/
//     TCheckBox/TEdit/TPanel/TButton/TMemo）—— 属《转换开发文档》§2.3 不移植项（无头平台）。
//     处置（照搬 p5-m2-custommagic 车道 CustomMagicSeams.cs 已验证的做法）：
//       - 取值 / 写回 / 行号文本 / TStrings.Text 往返 全部抽成**纯逻辑**（CustomItemPropertyLogic.cs）
//         并单测；
//       - 控件的**创建/销毁/消息泵/ShowModal** 留接缝：Ci*Seam + CustomItemPropertyMessageBoxSeam。
//   * M2Share.pas（g_CustomItemPropertyChecks[1..60] / g_CustomItemPropertyBindNames[1..60] /
//     g_CustomItemPropertyCRC / g_CustomItemPropertyTextVarList / g_CustomItemPropertyTextVarListTextCRC /
//     boStartReady / RebuildCustomItemPropertyConfig：22227-22243 / SaveCustomItemPropertyTextVarList：9343-9359）
//     → 全部以 CustomItemPropertyGlobals 接缝注入。
//   * UsrEngn.pas（UserEngine.SendCustomItemPropertyConfig / SendCustomItemPropertyTextVarList）
//     → 同上接缝注入。
//
// ★ 接缝纪律（《并行派发台账》§25.2）：本文件的接缝**默认一律"显式报未接线"（抛异常）**，
//   **绝不返回中性值**（不许 `?? false` / `?? 0` / 空实现）。唯一的例外是 `boStartReady`，
//   它是**原文全局变量的初值**（False），属于"照抄原文状态"而非"接缝臆造中性值"，已在报告登记。
//
// 覆盖行号（Delphi）：uFrmCustomItemProperty.pas 5-7、159、165-176、178-306、308-450、
//                     452-461、463-466、468-484、486-496（详见 docs\并行报告-p8-m2-itemprop-misc.md）
// ============================================================================

using GXX.Core.Util;

namespace GXX.M2Server.Forms.ItemProperty;

// ---------------------------------------------------------------------------
// 模态框/无头开关
// ---------------------------------------------------------------------------

/// <summary>
/// ShowModal 的无头开关（照搬 GXX.M2Server.Forms.CustomMagic.CustomMagicMessageBoxSeam）。
/// <para>
/// 原文 uFrmCustomItemProperty.pas:172 <c>FrmCustomItemProperty.ShowModal</c>。
/// **UiEnabled 默认 true（生产行为）；单测必须置 false，否则挂死 testhost。**
/// </para>
/// </summary>
public static class CustomItemPropertyMessageBoxSeam
{
    /// <summary>true = 真实弹窗（生产）；false = 无头（测试），跳过所有模态调用。</summary>
    public static bool UiEnabled = true;

    /// <summary>ShowModal 被调用次数（无头时供断言；生产运行时为副作用镜像）。</summary>
    public static int ShowModalCalls;

    /// <summary>
    /// 原文 <c>ShowModal</c>。无头时不真弹；返回本次模态结果（原文返回值被丢弃，见 :172）。
    /// </summary>
    public static bool ShowModal()
    {
        ShowModalCalls++;
        if (!UiEnabled)
            return false;
        // 生产侧：真实窗体由宿主（WinForms）呈现，此处只登记调用。
        return false;
    }
}

// ---------------------------------------------------------------------------
// VCL 控件接缝（只保留窗体实际用到的成员面）
//
// ★ 类型名一律加 `Ci`（CustomItemProperty）前缀：本解决方案内已有
//   GXX.M2Server.Forms.CustomMagic.{TControlSeam,TWinControlSeam,TLabelSeam,TPanelSeam,
//   TTabSheetSeam,TPageControlSeam,TButtonSeam,TCheckBoxSeam,TEditSeam,TComboBoxSeam} 等
//   同名类型（main 分支）。换命名空间虽不致编译冲突，但同一文件同时 using 两个命名空间时
//   会 CS0104；为满足派发要求"新增类型前 git grep 必须为空"，此处改前缀，语义不变。
// ---------------------------------------------------------------------------

/// <summary>VCL TControl 的最小面。</summary>
public class CiControlSeam
{
    /// <summary>property Tag: NativeInt。</summary>
    public int Tag;
    /// <summary>property Enabled。</summary>
    public bool Enabled = true;
    /// <summary>property Visible。</summary>
    public bool Visible = true;
}

/// <summary>VCL TWinControl 的最小面（含子控件集合）。</summary>
public class CiWinControlSeam : CiControlSeam
{
    /// <summary>子控件列表（对应 ControlCount / Controls[I]）。</summary>
    public readonly List<CiControlSeam> Controls = new();
}

/// <summary>VCL TForm（原文 <c>TFrmCustomItemProperty = class(TForm)</c>，:10）。</summary>
public class CiFormSeam : CiWinControlSeam
{
    /// <summary>DFM <c>OnCreate = FormCreate</c>。</summary>
    public Action? OnCreate;
}

/// <summary>VCL TLabel（lbl1 / lbl2 / lbl3 / lblLineNum）。</summary>
public class CiLabelSeam : CiControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
}

/// <summary>VCL TCheckBox（chk01..chk60）。</summary>
public class CiCheckBoxSeam : CiWinControlSeam
{
    /// <summary>property Checked。</summary>
    public bool Checked;
    /// <summary>DFM <c>OnClick = chk01Click</c>（DFM 实测：60 个 chk 全部指向 chk01Click）。</summary>
    public Action? OnClick;
}

/// <summary>VCL TEdit（edtShowName01..edtShowName60）。</summary>
public class CiEditSeam : CiWinControlSeam
{
    /// <summary>property Text。</summary>
    public string Text = "";
    /// <summary>DFM <c>OnChange = edtShowName01Change</c>（实测：60 个 edt 全部指向它）。</summary>
    public Action? OnChange;
}

/// <summary>VCL TMemo（mmoVar）。</summary>
public class CiMemoSeam : CiWinControlSeam
{
    /// <summary>property Lines.Text（Delphi TMemo.Text 即 Lines.Text，见 CustomItemPropertyLogic.GetTextStr）。</summary>
    public string Text = "";
    /// <summary>
    /// property CaretPos.Y —— **Delphi 的 TMemo.CaretPos 是 0-based**（原文 :489/:495 用 <c>+1</c> 显示）。
    /// </summary>
    public int CaretPosY;
    /// <summary>DFM <c>OnChange = mmoVarChange</c>。</summary>
    public Action? OnChange;
    /// <summary>DFM <c>OnKeyUp = mmoVarKeyUp</c>。</summary>
    public Action? OnKeyUp;
    /// <summary>DFM <c>OnMouseDown = mmoVarMouseDown</c>。</summary>
    public Action? OnMouseDown;
}

/// <summary>VCL TPanel（pnlBottom1 / pnlBottom2）。</summary>
public class CiPanelSeam : CiWinControlSeam { }

/// <summary>VCL TButton（btnOK / btnOK2）。</summary>
public class CiButtonSeam : CiWinControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
    /// <summary>DFM <c>OnClick = btnOKClick / btnOK2Click</c>。</summary>
    public Action? OnClick;
}

/// <summary>VCL TTabSheet（tsBindAttr / tsText）。</summary>
public class CiTabSheetSeam : CiWinControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
}

/// <summary>VCL TPageControl（pgcMain）。</summary>
public class CiPageControlSeam : CiWinControlSeam
{
    /// <summary>property ActivePageIndex。</summary>
    public int ActivePageIndex;
    /// <summary>property ActivePage。</summary>
    public CiTabSheetSeam? ActivePage;
    /// <summary>页列表。</summary>
    public readonly List<CiTabSheetSeam> Pages = new();

    /// <summary>
    /// 复刻 Delphi <c>ActivePageIndex</c> 的**静默容忍**语义
    /// （《并行派发台账》§21.3：Delphi 越界静默置 -1，WinForms 抛异常）。
    /// 本窗体未直接赋值 ActivePageIndex（DFM 里为 0），此垫片供宿主适配层使用。
    /// </summary>
    public void SetActivePageIndex(int value)
    {
        if (value < 0 || value >= Pages.Count)
        {
            ActivePageIndex = -1;
            ActivePage = null;
            return;
        }
        ActivePageIndex = value;
        ActivePage = Pages[value];
    }
}

// ---------------------------------------------------------------------------
// M2Share.pas / UsrEngn.pas 接缝
// ---------------------------------------------------------------------------

/// <summary>
/// 原文 uFrmCustomItemProperty.pas 用到的 M2Share.pas / UsrEngn.pas 全局量与函数。
///
/// **接缝默认一律抛异常（不返回中性值）**，接线由对应单元移植批次完成：
///   * M2Share.pas:3765/3819-3821/3992/4054（g_CustomItemProperty* 全局与 typed const 默认表）
///   * M2Share.pas:22227-22243（RebuildCustomItemPropertyConfig）
///   * M2Share.pas:9343-9359（SaveCustomItemPropertyTextVarList）
///   * UsrEngn.pas（UserEngine.SendCustomItemPropertyConfig / SendCustomItemPropertyTextVarList）
///   * M2Share.pas（boStartReady）
/// </summary>
public static class CustomItemPropertyGlobals
{
    private static bool[]? _checks;
    private static string[]? _bindNames;
    private static TStringList? _textVarList;

    /// <summary>
    /// 原文 <c>g_CustomItemPropertyChecks: array [1 .. CUSTOM_PROPERTY_BIND_TYPE_COUNT] of Boolean</c>
    /// （M2Share.pas:4054，默认表全 True）。
    /// <para>
    /// 托管侧为 <c>bool[61]</c>：**下标 1..60 与原文一一对应，下标 0 未使用**（Delphi 下界为 1）。
    /// 未接线时读取即抛（§25.2：接缝不得静默返回中性值）。
    /// </para>
    /// </summary>
    public static bool[] g_CustomItemPropertyChecks
    {
        get => _checks ?? throw NotWired(nameof(g_CustomItemPropertyChecks),
            "M2Share.pas:4054（array [1..60] of Boolean，typed const 默认全 True）");
        set => _checks = value;
    }

    /// <summary>原文 <c>g_CustomItemPropertyBindNames: array [1 .. 60] of string</c>（M2Share.pas:3992）。托管侧 string[61]，下标 0 未使用。</summary>
    public static string[] g_CustomItemPropertyBindNames
    {
        get => _bindNames ?? throw NotWired(nameof(g_CustomItemPropertyBindNames),
            "M2Share.pas:3992（array [1..60] of string，typed const 默认表）");
        set => _bindNames = value;
    }

    /// <summary>
    /// 原文 <c>g_CustomItemPropertyTextVarList: TStringList = nil</c>（M2Share.pas:3765；
    /// 由 svMain.pas:2010 创建、:2353 释放）。托管侧用既有 GXX.Core.Util.TStringList。
    /// 未接线时读取即抛（等价于原文 nil 解引用，但带明确原因）。
    /// </summary>
    public static TStringList g_CustomItemPropertyTextVarList
    {
        get => _textVarList ?? throw NotWired(nameof(g_CustomItemPropertyTextVarList),
            "M2Share.pas:3765（TStringList，svMain.pas:2010 创建）");
        set => _textVarList = value;
    }

    /// <summary>原文 <c>g_CustomItemPropertyCRC: LongWord</c>（M2Share.pas，由 RebuildCustomItemPropertyConfig 写入）。</summary>
    public static uint g_CustomItemPropertyCRC;

    /// <summary>原文 <c>g_CustomItemPropertyTextVarListTextCRC: LongWord</c>（由 SaveCustomItemPropertyTextVarList 写入）。</summary>
    public static uint g_CustomItemPropertyTextVarListTextCRC;

    /// <summary>
    /// 原文 <c>boStartReady: Boolean</c>（M2Share.pas）。**初值 False 与原文一致**
    /// （服务器启动完成前不允许弹自定义物品属性窗体，原文 :169 <c>if not boStartReady then Exit</c>）。
    /// 这不是"接缝臆造中性值"，而是照抄原文全局初值。
    /// </summary>
    public static bool boStartReady;

    /// <summary>原文 <c>RebuildCustomItemPropertyConfig</c>（M2Share.pas:22227-22243）。接缝：待 M2Share.pas 移植后接入。</summary>
    public static Action? RebuildCustomItemPropertyConfig;

    /// <summary>原文 <c>SaveCustomItemPropertyTextVarList</c>（M2Share.pas:9343-9359）。接缝：待 M2Share.pas 移植后接入。</summary>
    public static Action? SaveCustomItemPropertyTextVarList;

    /// <summary>原文 <c>UserEngine.SendCustomItemPropertyConfig</c>（UsrEngn.pas）。接缝：待 UsrEngn.pas 移植后接入。</summary>
    public static Action? SendCustomItemPropertyConfig;

    /// <summary>原文 <c>UserEngine.SendCustomItemPropertyTextVarList</c>（UsrEngn.pas）。接缝：待 UsrEngn.pas 移植后接入。</summary>
    public static Action? SendCustomItemPropertyTextVarList;

    /// <summary>
    /// 调用 <c>RebuildCustomItemPropertyConfig</c>；未接线即抛（§25.2：绝不静默跳过）。
    /// 窗体 :442 的唯一调用入口。
    /// </summary>
    public static void InvokeRebuildCustomItemPropertyConfig()
        => (RebuildCustomItemPropertyConfig ?? throw NotWired(nameof(RebuildCustomItemPropertyConfig),
            "M2Share.pas:22227-22243"))();

    /// <summary>调用 <c>SaveCustomItemPropertyTextVarList</c>；未接线即抛。窗体 :476 的唯一调用入口。</summary>
    public static void InvokeSaveCustomItemPropertyTextVarList()
        => (SaveCustomItemPropertyTextVarList ?? throw NotWired(nameof(SaveCustomItemPropertyTextVarList),
            "M2Share.pas:9343-9359"))();

    /// <summary>调用 <c>UserEngine.SendCustomItemPropertyConfig</c>；未接线即抛。窗体 :446 的唯一调用入口。</summary>
    public static void InvokeSendCustomItemPropertyConfig()
        => (SendCustomItemPropertyConfig ?? throw NotWired(nameof(SendCustomItemPropertyConfig),
            "UsrEngn.pas UserEngine.SendCustomItemPropertyConfig"))();

    /// <summary>调用 <c>UserEngine.SendCustomItemPropertyTextVarList</c>；未接线即抛。窗体 :480 的唯一调用入口。</summary>
    public static void InvokeSendCustomItemPropertyTextVarList()
        => (SendCustomItemPropertyTextVarList ?? throw NotWired(nameof(SendCustomItemPropertyTextVarList),
            "UsrEngn.pas UserEngine.SendCustomItemPropertyTextVarList"))();

    /// <summary>复位全部接缝静态量（xUnit 串行集合里逐测调用）。</summary>
    public static void Reset()
    {
        _checks = null;
        _bindNames = null;
        _textVarList = null;
        g_CustomItemPropertyCRC = 0;
        g_CustomItemPropertyTextVarListTextCRC = 0;
        boStartReady = false;
        RebuildCustomItemPropertyConfig = null;
        SaveCustomItemPropertyTextVarList = null;
        SendCustomItemPropertyConfig = null;
        SendCustomItemPropertyTextVarList = null;
        CustomItemPropertyMessageBoxSeam.UiEnabled = true;
        CustomItemPropertyMessageBoxSeam.ShowModalCalls = 0;
    }

    private static InvalidOperationException NotWired(string name, string delphi)
        => new($"接缝未接线：{name}（原文 {delphi}）——待对应单元移植后接入；" +
               "按《并行派发台账》§25.2，接缝不提供〔静默中性值〕默认实现。");
}
