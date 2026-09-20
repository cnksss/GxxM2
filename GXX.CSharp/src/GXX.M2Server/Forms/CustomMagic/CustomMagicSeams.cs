// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = **接缝层**。原单元 uses（:5-7、:855-856）中尚未移植的第三方/框架单元：
//   * VirtualTrees.pas（TVirtualStringTree / TBaseVirtualTree / IVTEditLink / THitInfo /
//     TVirtualNode / TVSTTextType / TColumnIndex）→ 属《转换开发文档》§2.3 不移植项。
//     处置（照搬 p2-rungate-impl 车道已验证的做法，见 docs\并行报告-p2-rungate-impl.md §6.1）：
//       - 列文本 / 红字 / 勾叉 / 编辑许可 全部抽成**纯逻辑**（见 CustomMagicTreeLogic.cs、
//         CustomMagicEditLinkLogic.cs）并单测；
//       - 控件的**创建/销毁/消息泵**（IVTEditLink 的实现面、TVirtualStringTree 的窗口过程）
//         留接缝：IVTEditLinkSeam + IVirtualTreeHost，生产侧接 WinForms TreeView(OwnerDraw)。
//   * SpinEditEx.pas（TSpinEditEx）→ 接缝 TSpinEditExSeam。
//   * M2Threads.pas（g_MultiThreadRun / LockR / UnLockR）、CheckUnit.pas（BufferCrc）、
//     M2Share.pas（g_Config / g_EffectImageList / g_CustomMagicListText*）、
//     UsrEngn.pas（UserEngine.m_CustomMagicList / SendServerConfig / ResetMagicCDList）、
//     EDCode.pas（zLibCompressBuffer）→ 全部以 CustomMagicFormGlobals 接缝注入。
//
// 覆盖行号（Delphi）：uFrmCustomMagic.pas 5-13、855-856、1605-1623、1625-1637（ShowModal 门控）
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Forms.CustomMagic;

// ---------------------------------------------------------------------------
// 消息/绘制接缝
// ---------------------------------------------------------------------------

/// <summary>
/// 模态框/ShowModal 的无头开关（照搬 GXX.RunGate.uFrmGameSpeedLogic 的 MessageBoxSeam.UiEnabled）。
/// <para>
/// 原文 uFrmCustomMagic.pas:1632 <c>ShowCustomMagic</c> 里 <c>ShowModal = mrOK</c>，
/// 以及 :3664 <c>Showmessage('已经生成自定义技能登录器配置文件')</c>。
/// **UiEnabled 默认 true（生产行为）；单测必须置 false，否则挂死 testhost。**
/// </para>
/// </summary>
public static class CustomMagicMessageBoxSeam
{
    /// <summary>true = 真实弹窗（生产）；false = 无头（测试），跳过所有模态调用。</summary>
    public static bool UiEnabled = true;

    /// <summary>最近一次 Showmessage 的文本（无头时供断言；生产运行时为副作用镜像）。</summary>
    public static readonly List<string> ShownMessages = new();

    /// <summary>原文 Showmessage(Text)（:3664）。</summary>
    public static void ShowMessage(string text)
    {
        ShownMessages.Add(text);
        if (UiEnabled)
            System.Windows.Forms.MessageBox.Show(text);
    }

    /// <summary>ShowModal 返回值替身：DoOpen 后由接缝返回 mrOK/mrCancel。默认 mrCancel（原文无用户操作时不确认）。</summary>
    public static Func<System.Windows.Forms.DialogResult>? ShowModalHandler;
}

/// <summary>Delphi <c>mrOK</c> / <c>mrCancel</c> 常量（System.UITypes）。</summary>
public static class TModalResult
{
    /// <summary>mrNone。</summary>
    public const int mrNone = 0;
    /// <summary>mrOk。</summary>
    public const int mrOk = 1;
    /// <summary>mrCancel。</summary>
    public const int mrCancel = 2;
}

// ---------------------------------------------------------------------------
// TSpinEditEx 接缝（SpinEditEx.pas 未移植）
// ---------------------------------------------------------------------------

/// <summary>SpinEditEx.pas TSpinEditEx 的托管替身（只保留原窗体用到的成员面）。</summary>
public class TSpinEditExSeam : TWinControlSeam
{
    /// <summary>property Value: Integer。</summary>
    public int Value;

    /// <summary>property MinValue。</summary>
    public int MinValue;

    /// <summary>property MaxValue。</summary>
    public int MaxValue;
}

/// <summary>Delphi <c>TSpinEdit</c>（VCL 标准，被 seMagicAC* 族用作 Sender）。</summary>
public class TSpinEditSeam : TWinControlSeam
{
    /// <summary>property Value: Integer。</summary>
    public int Value;
}

// ---------------------------------------------------------------------------
// VCL 控件接缝（只保留窗体实际用到的成员面）
// ---------------------------------------------------------------------------

/// <summary>VCL TControl 的最小面。</summary>
public class TControlSeam
{
    /// <summary>property Tag: NativeInt。</summary>
    public int Tag;
    /// <summary>property Enabled。</summary>
    public bool Enabled = true;
    /// <summary>property Visible。</summary>
    public bool Visible = true;
    /// <summary>property Left。</summary>
    public int Left;
    /// <summary>property Top。</summary>
    public int Top;
    /// <summary>property Width。</summary>
    public int Width;
    /// <summary>property Height。</summary>
    public int Height;
}

/// <summary>VCL TWinControl 的最小面（含子控件集合，供 SetControlEnabled 递归）。</summary>
public class TWinControlSeam : TControlSeam
{
    /// <summary>子控件列表（对应 ControlCount / Controls[I]）。</summary>
    public readonly List<TControlSeam> Controls = new();
}

/// <summary>VCL TLabel。</summary>
public class TLabelSeam : TControlSeam { }

/// <summary>VCL TStaticText。</summary>
public class TStaticTextSeam : TControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
    /// <summary>property Font.Color（只用于 clRed/clBlue 分支判定）。</summary>
    public int FontColor;
}

/// <summary>VCL TBevel。</summary>
public class TBevelSeam : TControlSeam { }

/// <summary>VCL TPanel（SetControlEnabled 会递归进它）。</summary>
public class TPanelSeam : TWinControlSeam { }

/// <summary>VCL TGroupBox（SetControlEnabled 会递归进它）。</summary>
public class TGroupBoxSeam : TWinControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
}

/// <summary>VCL TTabSheet（SetControlEnabled 会递归进它）。</summary>
public class TTabSheetSeam : TWinControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
}

/// <summary>VCL TPageControl。</summary>
public class TPageControlSeam : TWinControlSeam
{
    /// <summary>property ActivePageIndex。</summary>
    public int ActivePageIndex;
    /// <summary>property ActivePage（原文 :3188/:3192 直接赋 TTabSheet）。</summary>
    public TTabSheetSeam? ActivePage;
    /// <summary>页列表（AddItem 等价）。</summary>
    public readonly List<TTabSheetSeam> Pages = new();
}

/// <summary>VCL TButton。</summary>
public class TButtonSeam : TWinControlSeam
{
    /// <summary>property Caption。</summary>
    public string Caption = "";
}

/// <summary>VCL TCheckBox。</summary>
public class TCheckBoxSeam : TWinControlSeam
{
    /// <summary>property Checked。</summary>
    public bool Checked;
}

/// <summary>VCL TEdit。</summary>
public class TEditSeam : TWinControlSeam
{
    /// <summary>property Text。</summary>
    public string Text = "";
    /// <summary>property MaxLength。</summary>
    public int MaxLength;
}

/// <summary>VCL TComboBox。</summary>
public class TComboBoxSeam : TWinControlSeam
{
    /// <summary>property Items（TStrings）。</summary>
    public readonly List<string> Items = new();
    /// <summary>property ItemIndex（无选中为 -1，与 Delphi 一致）。</summary>
    public int ItemIndex = -1;
    /// <summary>property Style（csDropDownList 判定用）。</summary>
    public int Style;
    /// <summary>property DroppedDown（EditLink.EditKeyDown 判定用）。</summary>
    public bool DroppedDown;
    /// <summary>property Text（csDropDown 时可编辑）。</summary>
    public string Text = "";
}

/// <summary>VCL TImageList（只保留 Width/Height/Draw）。</summary>
public class TImageListSeam
{
    /// <summary>property Width。</summary>
    public int Width;
    /// <summary>property Height。</summary>
    public int Height;
    /// <summary>Draw 调用记录（无头环境不能真画，记录 (x,y,index) 供断言）。</summary>
    public readonly List<(int X, int Y, int Index)> DrawCalls = new();

    /// <summary>原文 ilCheck.Draw(TargetCanvas, X, Y, Index)（:4337/:4479/:4598）。</summary>
    public void Draw(object targetCanvas, int x, int y, int index) => DrawCalls.Add((x, y, index));
}

/// <summary>VCL TSaveDialog。</summary>
public class TSaveDialogSeam
{
    /// <summary>property FileName。</summary>
    public string FileName = "";
    /// <summary>property Execute（无头接缝：默认 false = 用户取消）。</summary>
    public Func<bool>? ExecuteHandler;
    /// <summary>Execute 被调用次数（断言用）。</summary>
    public int ExecuteCalls;

    /// <summary>原文 dlgSaveMagics.Execute（:3632）。</summary>
    public bool Execute()
    {
        ExecuteCalls++;
        return ExecuteHandler?.Invoke() ?? false;
    }
}

// ---------------------------------------------------------------------------
// VirtualTrees.pas 接缝（TVirtualStringTree / IVTEditLink）
// 接缝：待 VirtualTrees.pas 移植后由其正式接管 —— 但该单元属 §2.3 不移植项，
// 接缝将长期存在；生产侧由 WinForms TreeView(OwnerDraw) 承接。
// ---------------------------------------------------------------------------

/// <summary>VirtualTrees.pas TCheckType（原文用到 ctCheckBox）。</summary>
public enum TCheckType
{
    ctNone,
    ctCheckBox,
    ctRadioButton,
    ctTriStateCheckBox,
}

/// <summary>VirtualTrees.pas TCheckState（原文用到 csCheckedNormal / csUnCheckedNormal）。</summary>
public enum TCheckState
{
    csUnCheckedNormal,
    csCheckedNormal,
    csMixedNormal,
}

/// <summary>VirtualTrees.pas TVSTTextType（只作为形参出现，原文未使用其分支）。</summary>
public enum TVSTTextType
{
    ttNormal,
    ttStatic,
}

/// <summary>VirtualTrees.pas TVirtualNode（接缝替身；节点数据由宿主持有）。</summary>
public sealed class TVirtualNodeSeam
{
    /// <summary>property CheckType。</summary>
    public TCheckType CheckType = TCheckType.ctNone;
    /// <summary>property CheckState。</summary>
    public TCheckState CheckState = TCheckState.csUnCheckedNormal;
    /// <summary>宿主任意挂载的节点数据（对应 GetNodeData 的返回）。</summary>
    public object? Data;
    /// <summary>节点序号（调试/断言用）。</summary>
    public int Index;
    /// <summary>树的 DisplayText（GetText 回调的产物镜像）。</summary>
    public string CellText = "";
}

/// <summary>VirtualTrees.pas THitInfo（原文用 HitNode / HitColumn）。</summary>
public struct THitInfo
{
    /// <summary>HitNode。</summary>
    public TVirtualNodeSeam? HitNode;
    /// <summary>HitColumn。</summary>
    public int HitColumn;
}

/// <summary>VirtualTrees.pas TRect 的最小面（接缝内自足）。</summary>
public struct TRectSeam
{
    /// <summary>Left。</summary>
    public int Left;
    /// <summary>Top。</summary>
    public int Top;
    /// <summary>Right。</summary>
    public int Right;
    /// <summary>Bottom。</summary>
    public int Bottom;
}

/// <summary>
/// VirtualTrees.pas IVTEditLink 的托管替身。
/// 原文实现类 TDecAttribPropertyEditLink（:888）与 TElementPropertyEditLink（:909）。
/// **创建/销毁/消息泵留接缝**（见 CustomMagicEditLinkLogic 的实现类）；
/// 取值/校验/写回全部走纯逻辑函数。
/// </summary>
public interface IVTEditLinkSeam
{
    /// <summary>function BeginEdit: Boolean; stdcall（原文 :990/:1344）。</summary>
    bool BeginEdit();
    /// <summary>function CancelEdit: Boolean; stdcall（原文 :999/:1353）。</summary>
    bool CancelEdit();
    /// <summary>function EndEdit: Boolean; stdcall（原文 :1007/:1361）。</summary>
    bool EndEdit();
    /// <summary>function GetBounds: TRect; stdcall（原文 :1149/:1479）。</summary>
    TRectSeam GetBounds();
    /// <summary>function PrepareEdit(Tree, Node, Column): Boolean; stdcall（原文 :1156/:1486）。</summary>
    bool PrepareEdit(IVirtualTreeHost tree, TVirtualNodeSeam node, int column);
    /// <summary>procedure ProcessMessage(var Message); stdcall（原文 :1267/:1588）。</summary>
    void ProcessMessage(ref object message);
    /// <summary>procedure SetBounds(R: TRect); stdcall（原文 :1274/:1595）。</summary>
    void SetBounds(TRectSeam r);
}

/// <summary>
/// TVirtualStringTree 的宿主接缝：只暴露原文窗体真正调用的成员。
/// 生产侧由 WinForms TreeView(OwnerDraw) 的适配器实现；
/// 测试侧用 CustomMagicTreeHost（内存实现）。
/// </summary>
public interface IVirtualTreeHost
{
    /// <summary>property FocusedNode。</summary>
    TVirtualNodeSeam? FocusedNode { get; }
    /// <summary>property Focused。</summary>
    bool Focused { get; }
    /// <summary>property CanFocus。</summary>
    bool CanFocus { get; }
    /// <summary>procedure SetFocus。</summary>
    void SetFocus();
    /// <summary>property Owner（原文 FTree.Owner is TFrmCustomMagic 判定）。</summary>
    object? Owner { get; }
    /// <summary>property Handle（PostMessage 目标）。</summary>
    IntPtr Handle { get; }
    /// <summary>property Font.Color。</summary>
    int FontColor { get; }
    /// <summary>function GetNodeData(Node)。</summary>
    object? GetNodeData(TVirtualNodeSeam? node);
    /// <summary>procedure InvalidateNode(Node)。</summary>
    void InvalidateNode(TVirtualNodeSeam? node);
    /// <summary>procedure EndEditNode。</summary>
    void EndEditNode();
    /// <summary>procedure CancelEditNode。</summary>
    void CancelEditNode();
    /// <summary>procedure Invalidate。</summary>
    void Invalidate();
    /// <summary>property Selected[Node]。</summary>
    bool IsSelected(TVirtualNodeSeam node);
    /// <summary>property CheckState[Node]。</summary>
    TCheckState GetCheckState(TVirtualNodeSeam node);
    /// <summary>procedure Clear。</summary>
    void Clear();
    /// <summary>function AddChild(Parent)。</summary>
    TVirtualNodeSeam AddChild(TVirtualNodeSeam? parent);
    /// <summary>function GetFirst。</summary>
    TVirtualNodeSeam? GetFirst();
    /// <summary>function GetNext(Node)。</summary>
    TVirtualNodeSeam? GetNext(TVirtualNodeSeam node);
    /// <summary>procedure EditNode(Node, Column)。</summary>
    void EditNode(TVirtualNodeSeam? node, int column);
    /// <summary>Header.Columns.GetColumnBounds(Column, Dummy, Right)。</summary>
    int GetColumnBounds(int column, int right);
}

/// <summary>
/// PostMessage 接缝（原文 WM_STARTEDITING_* 自投递 + VK_UP/VK_DOWN 转发）。
/// 生产侧接 Win32 PostMessage；测试侧记录调用并可直接派发。
/// </summary>
public static class CustomMagicPostMessageSeam
{
    /// <summary>消息投递记录：(Handle, Msg, WParam, LParam)。</summary>
    public static readonly List<(IntPtr Handle, int Msg, int WParam, int LParam)> Posted = new();

    /// <summary>生产侧实现（默认 null = 只记录不真投递）。</summary>
    public static Action<IntPtr, int, int, int>? Post;

    /// <summary>原文 PostMessage(Handle, Msg, WParam, LParam)。</summary>
    public static void PostMessage(IntPtr handle, int msg, int wParam, int lParam)
    {
        Posted.Add((handle, msg, wParam, lParam));
        Post?.Invoke(handle, msg, wParam, lParam);
    }

    /// <summary>清空记录（测试用）。</summary>
    public static void Clear() => Posted.Clear();
}

// ---------------------------------------------------------------------------
// M2Share / UsrEngn / M2Threads / CheckUnit / EDCode 接缝
// ---------------------------------------------------------------------------

/// <summary>
/// 原文 uses（:855-856 M2Share, UsrEngn, EDCode）与 :5-7（M2Threads, CheckUnit）里
/// 窗体用到的全局量与函数。全部为接缝，默认实现是"内存计数/无落盘"，
/// 真实接线随对应单元移植接入。
/// </summary>
public static class CustomMagicFormGlobals
{
    // ---- g_Config（M2Share.pas TM2Config）----

    /// <summary>g_Config.boSendCustomMagicConfig（原文 :1645、:3667）。</summary>
    public static bool boSendCustomMagicConfig;

    /// <summary>g_Config.sCustomMagicClientConfigFileName（原文 :3630/:3636）。</summary>
    public static string sCustomMagicClientConfigFileName = "";

    /// <summary>g_Config.sCustomMagicDir（原文 uCustomMagicUtils.pas:775，非本单元，登记备查）。</summary>
    public static string sCustomMagicDir = "";

    // ---- Config: TIniFileEx（M2Share.pas 全局 Config）----

    /// <summary>Config.WriteString 接缝（原文 :3637/'Setup','CustomMagicClientConfigFileName'）。</summary>
    public static readonly List<(string Section, string Key, string Value)> ConfigStrings = new();
    /// <summary>Config.WriteBool 接缝（原文 :3668/'Setup','SendCustomMagicConfig'）。</summary>
    public static readonly List<(string Section, string Key, bool Value)> ConfigBools = new();

    /// <summary>原文 Config.WriteString。</summary>
    public static void ConfigWriteString(string section, string key, string value)
        => ConfigStrings.Add((section, key, value));

    /// <summary>原文 Config.WriteBool。</summary>
    public static void ConfigWriteBool(string section, string key, bool value)
        => ConfigBools.Add((section, key, value));

    // ---- g_EffectImageList（M2Share.pas；C# 正式归属 ViewList2State.g_EffectImageList）----

    /// <summary>g_EffectImageList（原文 :1710-1808 的文件下拉填充源）。</summary>
    public static Func<List<string>> EffectImageList = () => GXX.M2Server.Forms.ViewList2State.g_EffectImageList;

    // ---- UsrEngn.pas UserEngine ----

    /// <summary>UserEngine.m_CustomMagicList（TList of TCustomMagicConfig）（原文 :1652、:3538）。</summary>
    public static readonly List<TCustomMagicConfig> m_CustomMagicList = new();

    /// <summary>UserEngine.SendServerConfig 调用次数（原文 :3626）。</summary>
    public static int SendServerConfigCalls;

    /// <summary>原文 UserEngine.SendServerConfig（:3626）。</summary>
    public static void SendServerConfig()
    {
        SendServerConfigCalls++;
        Engine.GameConfigState.SendServerConfig();
    }

    /// <summary>ResetMagicCDList 调用次数（原文 :3625；未在本单元 uses 明列，属 UsrEngn）。</summary>
    public static int ResetMagicCDListCalls;

    /// <summary>原文 ResetMagicCDList（:3625）。</summary>
    public static void ResetMagicCDList() => ResetMagicCDListCalls++;

    // ---- M2Threads.pas ----

    /// <summary>g_MultiThreadRun（原文 :1648、:326、:3639）。</summary>
    public static bool g_MultiThreadRun;

    /// <summary>m_CustomMagicList.LockR / UnLockR 调用记录（把 LockR 的整数参数一并记录）。</summary>
    public static readonly List<(string Op, int Arg)> ListLockCalls = new();

    // ---- CheckUnit.pas / EDCode.pas ----

    /// <summary>g_CustomMagicListText（原文 :3594）。</summary>
    public static byte[]? g_CustomMagicListText;
    /// <summary>g_CustomMagicListTextLen（原文 :3593）。</summary>
    public static int g_CustomMagicListTextLen;
    /// <summary>g_CustomMagicListTextCRC（原文 :3595）。</summary>
    public static uint g_CustomMagicListTextCRC;

    /// <summary>BufferCrc 接缝（CheckUnit.pas:24-34；默认转调 GXX.Core 的 CheckUnit.BufferCrc）。</summary>
    public static Func<byte[], int, uint> BufferCrc = GXX.Core.Util.CheckUnit.BufferCrc;

    /// <summary>zLibCompressBuffer 接缝（EDCode.pas:535；默认转调 GXX.Core 的 EDcode.zLibCompressBuffer）。</summary>
    public static Func<byte[], int, byte[]> ZLibCompressBuffer =
        GXX.Core.Protocol.EDcode.zLibCompressBuffer;

    /// <summary>SaveCustomMagicClientConfigs 接缝（uCustomMagicUtils.pas:247-317；默认只记录）。</summary>
    public static Action<List<TCustomMagicConfig>, string>? SaveCustomMagicClientConfigs;

    /// <summary>SetCurrentDirectory / Application.ExeName 接缝（原文 :3633-3635）。</summary>
    public static string ApplicationExeName = "";
    /// <summary>SetCurrentDirectory 记录。</summary>
    public static readonly List<string> SetCurrentDirectoryCalls = new();
    /// <summary>原文 SetCurrentDirectory(dir)。</summary>
    public static void SetCurrentDirectory(string dir) => SetCurrentDirectoryCalls.Add(dir);

    /// <summary>ExtractFileDir 接缝（默认取目录部分）。</summary>
    public static Func<string, string> ExtractFileDir = path =>
    {
        int i = path.LastIndexOfAny(new[] { '\\', '/' });
        return i < 0 ? "" : path[..i];
    };

    /// <summary>ChangeFileExt 接缝（默认换扩展名）。</summary>
    public static Func<string, string, string> ChangeFileExt = (path, ext) =>
    {
        int i = path.LastIndexOf('.');
        int j = path.LastIndexOfAny(new[] { '\\', '/' });
        return i > j ? path[..i] + ext : path + ext;
    };

    // ---- 复位（测试用）----

    /// <summary>复位全部接缝静态量（xUnit 串行集合里逐测调用）。</summary>
    public static void Reset()
    {
        boSendCustomMagicConfig = false;
        sCustomMagicClientConfigFileName = "";
        sCustomMagicDir = "";
        ConfigStrings.Clear();
        ConfigBools.Clear();
        EffectImageList = () => GXX.M2Server.Forms.ViewList2State.g_EffectImageList;
        m_CustomMagicList.Clear();
        SendServerConfigCalls = 0;
        ResetMagicCDListCalls = 0;
        g_MultiThreadRun = false;
        ListLockCalls.Clear();
        g_CustomMagicListText = null;
        g_CustomMagicListTextLen = 0;
        g_CustomMagicListTextCRC = 0;
        BufferCrc = GXX.Core.Util.CheckUnit.BufferCrc;
        ZLibCompressBuffer = GXX.Core.Protocol.EDcode.zLibCompressBuffer;
        SaveCustomMagicClientConfigs = null;
        ApplicationExeName = "";
        SetCurrentDirectoryCalls.Clear();
        CustomMagicPostMessageSeam.Clear();
        CustomMagicMessageBoxSeam.ShownMessages.Clear();
        CustomMagicMessageBoxSeam.UiEnabled = true;
        CustomMagicMessageBoxSeam.ShowModalHandler = null;
    }
}
