using System;
using System.Collections.Generic;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Core.Protocol;
using GXX.Core.Util;

namespace GXX.Client.GUI.Share;

// ============================================================================================
// 【接缝】FState.pas（22,503 非空行 / 25,165 物理行，全工程最大单元）声明面所需、
// 而工程中尚无实现的最小类型面。
//
// 分区裁定（沿用 docs/并行派发台账.md §9.3）：
//   * DxComponent 控件族（TDxControl/TDxImageButton/TDxLabel/TDxEdit/TDxImageForm/TDxImageGrid/
//     TDxScrollBox/TDxPopupMenu/TDxPageControl/TDxMemo/TRect/TTexture/TGameImages/TGuiImageIndex/
//     TClientVersion/TImageType/TClickSound/TOnClickEx）**复用车道1 的 `GXX.Client.GUI.DxComponent`
//     命名空间**，本文件不复刻（§9.3 的"同一 Delphi 单元两条车道各造一套接缝"事故不得重演）。
//   * GXX.Core.Protocol 已有的记录（TClientItem/TStdItem/TClientMagic/TGuildJoinUser/TUserEntry/
//     TStorageHeroInfo/TGuildMemeberInfo/TGuardianLevelItemCounts/TUserStateInfo）直接引用。
//   * MShare.pas 的全局（g_MySelf/g_UserState1/g_MagicList/GetRGB/MyGetTickCount…）复用车道1 的
//     `GXX.Client.GUI.Mir.MShareGlobals`（见本文件末尾 using static 说明），同样不复刻。
//   * 其余本单元专属的未移植依赖在下方逐个定义，全部标注原文出处。
// ============================================================================================

/// <summary>
/// FState.pas 用到、GXX.Core 中缺失的常量（原文出处逐条标注）。
/// 命名保留原文全大写，便于 .pas → .cs 对照。
/// </summary>
public static class FStateSeamConst
{
    /// <summary>SDK.pas:70 HINT_WIN_MIN = 0（提示窗数组下界）。</summary>
    public const int HINT_WIN_MIN = 0;

    /// <summary>SDK.pas:71 HINT_WIN_MAX = 2（提示窗数组上界，共 3 个）。</summary>
    public const int HINT_WIN_MAX = 2;

    /// <summary>MShare.pas:80 AUCTION_BAG_ONE_PAGE_COUNT = 10（拍卖中包裹物品每页显示 10 条）。</summary>
    public const int AUCTION_BAG_ONE_PAGE_COUNT = 10;
}

// ---------------------------------------------------------------------------------------------
// Delphi 基础类型接缝
// ---------------------------------------------------------------------------------------------

/// <summary>Delphi Windows.TPoint（32 位有符号）。</summary>
public struct TPoint
{
    public int X;
    public int Y;

    public TPoint(int x, int y) { X = x; Y = y; }

    public static TPoint Point(int x, int y) => new(x, y);
}

/// <summary>Delphi Controls.TShiftState（set of，[Flags] 对应）。</summary>
[Flags]
public enum TShiftState
{
    ssNone = 0,
    ssShift = 1,
    ssAlt = 2,
    ssCtrl = 4,
    ssLeft = 8,
    ssRight = 16,
    ssMiddle = 32,
    ssDouble = 64,
}

/// <summary>Delphi Controls.TMouseButton。</summary>
public enum TMouseButton
{
    mbLeft = 0,
    mbRight = 1,
    mbMiddle = 2,
}

/// <summary>Delphi Grids.TGridDrawState（set of，[Flags] 对应）。</summary>
[Flags]
public enum TGridDrawState
{
    gdSelected = 1,
    gdFocused = 2,
    gdFixed = 4,
    gdHot = 8,
}

/// <summary>Delphi Dialogs.TMsgDlgBtn 的集合（原文只按位传递）。</summary>
[Flags]
public enum TMsgDlgButtons
{
    mbNone = 0,
    mbYes = 1,
    mbNo = 2,
    mbOK = 4,
    mbCancel = 8,
    mbAbort = 16,
    mbRetry = 32,
    mbIgnore = 64,
    mbAll = 128,
    mbHelp = 256,
    mbClose = 512,
    mbYesNoCancel = mbYes | mbNo | mbCancel,
    mbOKCancel = mbOK | mbCancel,
    mbAbortRetryIgnore = mbAbort | mbRetry | mbIgnore,
}

/// <summary>Delphi Forms.TModalResult（mrNone = 0 起）。</summary>
public enum TModalResult
{
    mrNone = 0,
    mrOk = 1,
    mrCancel = 2,
    mrAbort = 3,
    mrRetry = 4,
    mrIgnore = 5,
    mrYes = 6,
    mrNo = 7,
    mrAll = 8,
    mrNoToAll = 9,
    mrYesToAll = 10,
}

/// <summary>Delphi Graphics.TGraphic 的最小占位（TNpcGraphicButton.SetGraphic 用）。</summary>
public class TGraphic
{
}

/// <summary>
/// Delphi Classes.TList（无泛型、Object 元素、0-based）。
/// 原文大量使用 Add/Count/Items/IndexOf/Delete/Clear，语义 1:1 保留。
/// </summary>
public class TList
{
    private readonly List<object> _items = new();

    public int Add(object item) { _items.Add(item); return _items.Count - 1; }

    public void Clear() => _items.Clear();

    public void Delete(int index) => _items.RemoveAt(index);

    /// <summary>Delphi TList.Remove（按值删首个匹配项，返回其下标，未找到返回 -1）。</summary>
    public int Remove(object item)
    {
        int index = _items.IndexOf(item);
        if (index >= 0) _items.RemoveAt(index);
        return index;
    }

    public int IndexOf(object item) => _items.IndexOf(item);

    public int Count => _items.Count;

    public object this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    /// <summary>Delphi TList.Exchange(I, J)：交换两个下标处的元素（DoSort 的快速排序用）。</summary>
    public void Exchange(int i, int j)
    {
        (this[i], this[j]) = (this[j], this[i]);
    }

    /// <summary>Delphi TList 的底层列表（供 Linq/枚举使用）。</summary>
    public IReadOnlyList<object> Items => _items;
}

/// <summary>Delphi Classes.TMemoryStream 的最小占位（LoadShareFromStream/LoadFromStream 用）。</summary>
public class TMemoryStream
{
    public byte[] Buffer = Array.Empty<byte>();
    public long Position;
    public long Size => Buffer.Length;
}

/// <summary>
/// Delphi IniFiles.THashedStringList 的最小占位。
/// 原文 MakeControlAddressList / MakeShareControlAddrList 返回它，键为 '@控件名'、值为控件地址。
/// </summary>
public class THashedStringList : TStringList
{
}

/// <summary>superobject.pas ISuperObject 的最小占位（LoadJsonControl 用）。</summary>
public interface ISuperObject
{
    /// <summary>superobject: ISuperObject.AsString。</summary>
    string AsString { get; }

    /// <summary>superobject: ISuperObject['name']（对象成员索引）。</summary>
    ISuperObject this[string name] { get; }

    /// <summary>superobject: ISuperObject.AsInteger。</summary>
    int AsInteger { get; }

    /// <summary>superobject: ISuperObject.AsBoolean。</summary>
    bool AsBoolean { get; }

    /// <summary>superobject: ISuperObject.AsArray。</summary>
    IReadOnlyList<ISuperObject> AsArray { get; }

    /// <summary>superobject: ISuperObject.Count。</summary>
    int Count { get; }
}

// ---------------------------------------------------------------------------------------------
// Delphi 指针类型接缝
// 原文用 ^T 指针把记录传出/传入（变更对调用方可见）。C# 用持有记录的引用类型 1:1 承载。
// ---------------------------------------------------------------------------------------------

/// <summary>FState.pas:76 pTDiceInfo = ^TDiceInfo。</summary>
public sealed class pTDiceInfo
{
    public TDiceInfo Value;
    public pTDiceInfo() { }
    public pTDiceInfo(TDiceInfo v) { Value = v; }
}

/// <summary>FState.pas:109 PShowGuildInfo = ^TShowGuildInfo。</summary>
public sealed class PShowGuildInfo
{
    public TShowGuildInfo Value;
    public PShowGuildInfo() { }
    public PShowGuildInfo(TShowGuildInfo v) { Value = v; }
}

/// <summary>Grobal2.pas ^TClientItem（pTClientItem）。</summary>
public sealed class PTClientItem
{
    public TClientItem Value;
    public PTClientItem() { }
    public PTClientItem(TClientItem v) { Value = v; }
}

/// <summary>Grobal2.pas ^TClientMagic（PTClientMagic）。</summary>
public sealed class PTClientMagic
{
    public TClientMagic Value;
    public PTClientMagic() { }
    public PTClientMagic(TClientMagic v) { Value = v; }
}

/// <summary>Grobal2.pas ^TStdItem（pTStdItem）。</summary>
public sealed class pTStdItem
{
    public TStdItem Value;
    public pTStdItem() { }
    public pTStdItem(TStdItem v) { Value = v; }
}

/// <summary>Grobal2.pas ^TClientUserShop（PClientUserShop）。</summary>
public sealed class PClientUserShop
{
    public TClientUserShop Value;
    public PClientUserShop() { }
    public PClientUserShop(TClientUserShop v) { Value = v; }
}

/// <summary>Grobal2.pas ^TGuildJoinUser（PGuildJoinUser）。</summary>
public sealed class PGuildJoinUser
{
    public TGuildJoinUser Value;
    public PGuildJoinUser() { }
    public PGuildJoinUser(TGuildJoinUser v) { Value = v; }
}

/// <summary>Grobal2.pas ^TUserEntry（pTUserEntry）。</summary>
public sealed class pTUserEntry
{
    public TUserEntry Value;
    public pTUserEntry() { }
    public pTUserEntry(TUserEntry v) { Value = v; }
}

/// <summary>Grobal2.pas ^TUserEntryAdd（pTUserEntryAdd）。</summary>
public sealed class pTUserEntryAdd
{
    public TUserEntryAdd Value;
    public pTUserEntryAdd() { }
    public pTUserEntryAdd(TUserEntryAdd v) { Value = v; }
}

/// <summary>Grobal2.pas ^TUserCharacterInfo（pTUserCharacterInfo）。</summary>
public sealed class pTUserCharacterInfo
{
    public TUserCharacterInfo Value;
    public pTUserCharacterInfo() { }
    public pTUserCharacterInfo(TUserCharacterInfo v) { Value = v; }
}

/// <summary>
/// 【接缝：待 Grobal2.pas 的 TSellPlayerMoney 移植后接入】
/// 原文 TSellPlayerMoney 是打包记录；OpenViewSellPlayerBagItems 只把它当只读句柄传给子类实现，
/// 本单元不解引用任何字段，故此处以不透明句柄承载并登记缺口。
/// </summary>
public sealed class PSellPlayerMoney
{
    public object Value;
}

/// <summary>【接缝：待 Grobal2.pas 的 TSellPlayerStorageViewHeader 移植后接入】</summary>
public sealed class PTSellPlayerStorageViewHeader
{
    public object Value;
}

/// <summary>【接缝：待 Grobal2.pas 的 TSellPlayerStorageExtViewHeader 移植后接入】</summary>
public sealed class PTSellPlayerStorageExtViewHeader
{
    public object Value;
}

// ---------------------------------------------------------------------------------------------
// DxComponent 控件接缝（车道1 DxComponent 命名空间未覆盖的类型）
// ---------------------------------------------------------------------------------------------

/// <summary>
/// DxControls.pas TDxControlEngine：控件树根 + 焦点/输入派发/绘制遍历。
/// 【接缝：待 DxComponent 全量移植后接入】本波次只复刻 TFrmDlg.Create 用到的成员。
/// </summary>
public class TDxControlEngine : GXX.Client.GUI.DxComponent.TDxControl
{
    public TDxControlEngine() : base(null) { }

    /// <summary>DxControls.pas TDxControlEngine.OnBackgroundClick:procedure(Sender:TObject)。</summary>
    public Action<object> OnBackgroundClick { get; set; }

    /// <summary>DxControls.pas TDxControlEngine.OnMouseDown（Button/Shift/X/Y + var IsRealArea 形态的简化）。</summary>
    public Action<object, TMouseButton, TShiftState, int, int> OnMouseDown { get; set; }
}

/// <summary>DxMemo.pas TDxChatMemo（聊天/M语记录框）。【接缝：待 DxMemo.pas 移植】</summary>
public class TDxChatMemo : GXX.Client.GUI.DxComponent.TDxMemo
{
    public TDxChatMemo(GXX.Client.GUI.DxComponent.TDxControl owner = null) : base(owner) { }
}

/// <summary>DxListView.pas TDxListView。【接缝：待 DxListView.pas 移植】</summary>
public class TDxListView : GXX.Client.GUI.DxComponent.TDxControl
{
    public TDxListView(GXX.Client.GUI.DxComponent.TDxControl owner = null) : base(owner) { }
}

/// <summary>DxImageEdit.pas TDxImageEdit（聊天输入框）。【接缝：待 DxImageEdit.pas 移植】</summary>
public class TDxImageEdit : GXX.Client.GUI.DxComponent.TDxEdit
{
    public TDxImageEdit(GXX.Client.GUI.DxComponent.TDxControl owner = null) : base(owner) { }
}

/// <summary>DxTreeView.pas TDxTreeNode（任务树节点）。【接缝：待 TDxTreeView 移植】</summary>
public class TDxTreeNode : GXX.Client.GUI.DxComponent.TDxControl
{
    public TDxTreeNode(GXX.Client.GUI.DxComponent.TDxControl owner = null) : base(owner) { }
}

/// <summary>DxTreeView.pas TDxTreeView（任务/帮会树）。【接缝：待 TDxTreeView.pas 移植】</summary>
public class TDxTreeView : GXX.Client.GUI.DxComponent.TDxControl
{
    public TDxTreeView(GXX.Client.GUI.DxComponent.TDxControl owner = null) : base(owner) { }
}

/// <summary>
/// WinForms TMemo（Delphi StdCtrls.TMemo）的最小接缝。
/// 原文 TFrmDlg.Create（FState.pas:1535-1544）只做 Parent/Color/Font/Ctl3D/BorderStyle/Visible 赋值。
/// 【接缝：待 ClMain.pas frmMain 移植后接入真实 WinForms 宿主】
/// </summary>
public class TMemo
{
    /// <summary>原文 TMemo.Create(frmMain.Owner)。</summary>
    public TMemo(object owner) { Owner = owner; }

    public object Owner { get; }

    /// <summary>原文 Memo.Parent := frmMain。</summary>
    public object Parent { get; set; }

    /// <summary>原文 Memo.Color := clblack。</summary>
    public int Color { get; set; }

    /// <summary>原文 Memo.Font.Color / Memo.Font.Size。</summary>
    public sealed class TFontSeam
    {
        public int Color { get; set; }
        public int Size { get; set; }
    }

    public TFontSeam Font { get; } = new();

    /// <summary>原文 Memo.Ctl3D := False。</summary>
    public bool Ctl3D { get; set; }

    /// <summary>原文 Memo.BorderStyle := bsSingle（Delphi Forms.TBorderStyle，bsSingle = 1）。</summary>
    public int BorderStyle { get; set; }

    /// <summary>原文 Memo.Visible := False。</summary>
    public bool Visible { get; set; }
}

// ---------------------------------------------------------------------------------------------
// DrawScrn.pas 的提示行/提示窗（GetHitLines 与 FSayItemHintWin 依赖）
// ---------------------------------------------------------------------------------------------

/// <summary>
/// DrawScrn.pas:318 THintLines（提示文本行容器）。
/// 【接缝：待 DrawScrn.pas 移植后接入】本波次只复刻 GetHitLines 与解构循环用到的成员。
/// 注意：车道8 已在其 TStateWindowsText.cs 中登记同一缺口，但归属为 DrawScrn.pas，
/// **不是** FState.pas —— 见本车道交付报告的"既往车道事实更正"一节。
/// </summary>
public class THintLines
{
    /// <summary>DrawScrn.pas THintLines.Add(Text:string; Color:TColor)。</summary>
    public int Add(string text, TColor color)
    {
        Lines.Add(new HintLine(text, color));
        return Lines.Count - 1;
    }

    /// <summary>DrawScrn.pas THintLines.Count。</summary>
    public int Count => Lines.Count;

    /// <summary>逐行的文本与颜色（对应原文 THintLines 内部的记录项）。</summary>
    public readonly List<HintLine> Lines = new();

    /// <summary>DrawScrn.pas THintLines 内一项：Text + Color。</summary>
    public readonly struct HintLine
    {
        public readonly string Text;
        public readonly TColor Color;

        public HintLine(string text, TColor color)
        {
            Text = text;
            Color = color;
        }
    }
}

/// <summary>DrawScrn.pas:408 THintWindows。【接缝：待 DrawScrn.pas 移植】</summary>
public class THintWindows
{
}

/// <summary>
/// DrawScrn.pas 单元级命名空间接缝：原文 TFrmDlg.Create 写的是 `DrawScrn.THintWindows.Create`。
/// 【接缝：待 DrawScrn.pas 移植】
/// </summary>
public static class DrawScrn
{
    /// <summary>DrawScrn.pas:408 THintWindows（原文以 `DrawScrn.THintWindows` 限定引用）。</summary>
    public static THintWindows CreateHintWindows() => new();
}

/// <summary>
/// MShare.pas 中 FState.pas 引用、而车道1 的 `GXX.Client.GUI.Mir.MShareGlobals` 尚未承载的全局。
/// 【接缝：待 MShare.pas 移植后与车道1 的 MShareGlobals 合并为单一类型（已登记在交付报告的
/// "接缝合并建议"）。此处只做最小定义，命名与原文一致。】
/// </summary>
public static class FStateMShareSeam
{
    /// <summary>MShare.pas:2008 g_SellDlgItem:TClientItem（出售对话框中被选中的物品）。</summary>
    public static TClientItem g_SellDlgItem;

    /// <summary>MShare.pas:1879 g_ExtBagOpenItemCount:Word = 0（扩展背包已开格数）。</summary>
    public static ushort g_ExtBagOpenItemCount;

    /// <summary>测试用复位。</summary>
    public static void ResetForTests()
    {
        g_SellDlgItem = default;
        g_ExtBagOpenItemCount = 0;
    }
}

/// <summary>
/// DxControls.pas TDxPopupMenu / TDxControl 上 FState.pas 用到的、车道1 接缝未覆盖的属性。
/// 【接缝：待 DxControls.pas 全量移植后删除本类，改用真实属性】
/// 用 ConditionalWeakTable 外挂，避免修改车道1 的 DxControls.cs（分区铁律）。
/// </summary>
public static class DxPopupMenuExt
{
    /// <summary>DxControls.pas TGuiType 枚举中本单元用到的取值。</summary>
    public enum TGuiType
    {
        /// <summary>普通控件</summary>
        t_Normal = 0,
        /// <summary>弹出菜单（原文 TDxPopupMenu.Create 里写死 t_PopupMenu）</summary>
        t_PopupMenu = 1,
    }

    /// <summary>DxControls.pas TDxControl 的五态字色（与车道1 的 TDxCaptionColor 同构）。</summary>
    public sealed class TFiveStateColor
    {
        public TDxFont Up { get; } = new();
        public TDxFont Hot { get; } = new();
        public TDxFont Down { get; } = new();
        public TDxFont Disabled { get; } = new();
        public TDxFont Checked { get; } = new();

        /// <summary>Delphi 的 `with ItemColor do begin Up.Color := X; Hot.Color := X; ... end` 惯用法。</summary>
        public void SetAll(int color)
        {
            Up.Value = color;
            Hot.Value = color;
            Down.Value = color;
            Disabled.Value = color;
            Checked.Value = color;
        }
    }

    private sealed class Extra
    {
        public TGuiType GuiType;
        public int Alpha;
        public readonly TFiveStateColor ItemColor = new();
        public readonly TFiveStateColor BorderColor = new();
        public int BackgroundColor;
        public bool DrawBorder = true;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<
        GXX.Client.GUI.DxComponent.TDxControl, Extra> Extras = new();

    private static Extra Of(GXX.Client.GUI.DxComponent.TDxControl c) => Extras.GetOrCreateValue(c);

    /// <summary>DxControls.pas TDxControl.GuiType。</summary>
    public static TGuiType GetGuiType(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).GuiType;
    public static void SetGuiType(GXX.Client.GUI.DxComponent.TDxControl c, TGuiType v) => Of(c).GuiType = v;

    /// <summary>DxControls.pas TDxControl.Alpha（0..255）。</summary>
    public static int GetAlpha(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).Alpha;
    public static void SetAlpha(GXX.Client.GUI.DxComponent.TDxControl c, int v) => Of(c).Alpha = v;

    /// <summary>DxControls.pas TDxControl.ItemColor。</summary>
    public static TFiveStateColor GetItemColor(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).ItemColor;

    /// <summary>DxControls.pas TDxControl.BorderColor。</summary>
    public static TFiveStateColor GetBorderColor(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).BorderColor;

    /// <summary>DxControls.pas TDxControl.BackgroundColor。</summary>
    public static int GetBackgroundColor(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).BackgroundColor;
    public static void SetBackgroundColor(GXX.Client.GUI.DxComponent.TDxControl c, int v) => Of(c).BackgroundColor = v;

    /// <summary>DxControls.pas TDxControl.DrawBorder。</summary>
    public static bool GetDrawBorder(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).DrawBorder;
    public static void SetDrawBorder(GXX.Client.GUI.DxComponent.TDxControl c, bool v) => Of(c).DrawBorder = v;
}

/// <summary>
/// FState.pas:60 TArrHintWindows = array[HINT_WIN_MIN..HINT_WIN_MAX] of TList。
/// Delphi 的**静态数组**按值传参会复制；此处以引用类型承载。
/// 原文所有使用点（GetMouseItemInfo/ShowMouseItemInfo/GetMouseItemInfoWindow）都只是把
/// 3 个 TList 句柄读出/写入，复制与引用在"容器本身不被重新赋值"的前提下等价；
/// 该前提由调用点保证，若后续发现依赖复制语义需改为 struct + 按值传递。
/// </summary>
public sealed class TArrHintWindows
{
    /// <summary>数组元素个数（HINT_WIN_MIN..HINT_WIN_MAX ⇒ 3）。</summary>
    public const int Length = FStateSeamConst.HINT_WIN_MAX - FStateSeamConst.HINT_WIN_MIN + 1;

    private readonly TList[] _items = new TList[Length];

    /// <summary>原文下标（HINT_WIN_MIN=0 起，含上界）。</summary>
    public TList this[int index]
    {
        get => _items[index - FStateSeamConst.HINT_WIN_MIN];
        set => _items[index - FStateSeamConst.HINT_WIN_MIN] = value;
    }
}

/// <summary>FState.pas:62 PTArrHintWindows = ^TArrHintWindows。</summary>
public sealed class PTArrHintWindows
{
    public TArrHintWindows Value;
}

// ---------------------------------------------------------------------------------------------
// DxControls.pas 中车道1 接缝尚未覆盖的 TDxControl 成员
// 【接缝：待 DxControls.pas 全量移植后，把 TDxControl 上的这三个属性补进正式类型并删除本类】
// 用 ConditionalWeakTable 挂在既有实例上，避免复制/修改车道1 的 DxControls.cs（分区铁律）。
// ---------------------------------------------------------------------------------------------

/// <summary>DxControls.pas TDxControl 的 AddData1/AddData2/DisableHideCtrl 三个成员的外挂承载。</summary>
public static class DxControlExt
{
    private sealed class Extra
    {
        public int AddData1;
        public int AddData2;
        public bool DisableHideCtrl;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<
        GXX.Client.GUI.DxComponent.TDxControl, Extra> Extras = new();

    private static Extra Of(GXX.Client.GUI.DxComponent.TDxControl c) => Extras.GetOrCreateValue(c);

    /// <summary>DxControls.pas TDxControl.AddData1（NPC 播放图片的循环次数门限）。</summary>
    public static int GetAddData1(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).AddData1;
    public static void SetAddData1(GXX.Client.GUI.DxComponent.TDxControl c, int v) => Of(c).AddData1 = v;

    /// <summary>DxControls.pas TDxControl.AddData2（NPC 播放图片的已循环次数）。</summary>
    public static int GetAddData2(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).AddData2;
    public static void SetAddData2(GXX.Client.GUI.DxComponent.TDxControl c, int v) => Of(c).AddData2 = v;

    /// <summary>DxControls.pas TDxControl.DisableHideCtrl（HideChatEdit 用：为真才真正隐藏）。</summary>
    public static bool GetDisableHideCtrl(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).DisableHideCtrl;
    public static void SetDisableHideCtrl(GXX.Client.GUI.DxComponent.TDxControl c, bool v) => Of(c).DisableHideCtrl = v;
}

/// <summary>
/// ClMain.pas `frmMain` 中 FState.pas 引用、而车道1 的 `GXX.Client.GUI.Mir.frmMain` 尚未承载的成员。
/// 【接缝：待 ClMain.pas 移植后与车道1 的 frmMain 合并为单一类型（已登记在交付报告的"接缝合并建议"）】
/// 此处只做转发，调用点保留原文函数名。
/// </summary>
public static class FStateClMainSeam
{
    /// <summary>ClMain.pas frmMain.SendMerchantDlgSelect(nMerchant:Integer; sCmd:string)。</summary>
    public static Action<int, string> SendMerchantDlgSelectHandler;

    /// <summary>ClMain.pas frmMain.SendMerchantDlgSelect（原文如此调用）。</summary>
    public static void SendMerchantDlgSelect(int nMerchant, string sCmd)
        => SendMerchantDlgSelectHandler?.Invoke(nMerchant, sCmd);

    /// <summary>MShare.pas g_nCurMerchant（当前商人识别号）。</summary>
    public static int g_nCurMerchant;

    /// <summary>ClMain.pas frmMain.UseMagic(nX, nY:Integer; Magic:PTClientMagic)。</summary>
    public static Action<int, int, PTClientMagic> UseMagicHandler;

    /// <summary>ClMain.pas frmMain.UseMagic（原文如此调用）。</summary>
    public static void UseMagic(int nX, int nY, PTClientMagic Magic)
        => UseMagicHandler?.Invoke(nX, nY, Magic);

    /// <summary>MShare.pas g_nMouseX / g_nMouseY（当前鼠标坐标）。</summary>
    public static int g_nMouseX;
    public static int g_nMouseY;

    /// <summary>MShare.pas g_nTargetX / g_nTargetY（当前锁定目标坐标）。</summary>
    public static int g_nTargetX;
    public static int g_nTargetY;

    /// <summary>ClMain.pas frmMain.Owner（TMemo 宿主）。</summary>
    public static object Owner;

    /// <summary>
    /// ClMain.pas `frmMain` 本体（TFrmDlg.Create 里 `Memo.Parent := frmMain` 的宿主窗体）。
    /// 【接缝：待 ClMain.pas 移植后换成真实 frmMain 实例】
    /// </summary>
    public static object FrmMainPlaceholder;

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        SendMerchantDlgSelectHandler = null;
        UseMagicHandler = null;
        g_nCurMerchant = 0;
        g_nMouseX = 0;
        g_nMouseY = 0;
        g_nTargetX = 0;
        g_nTargetY = 0;
        Owner = null;
        FrmMainPlaceholder = null;
        FStateMShareSeam.ResetForTests();
        MShareGlobalsReset.ResetForTests();
    }
}
