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
// DrawScrn.pas 的提示行/提示窗
//
// ★ D-P10-06 已结清（车道 p14-client-fstate，台账 §47.5）：原先这里的
//   `THintLines`（旧 407-452）与 `THintWindows` + `DrawScrn` 静态接缝（旧 599-612）
//   三个接缝段**已删除**，正式归属是 `GXX.Client.Scenes`
//   （`Scenes/DrawScrn/HintMessageFamily.cs:1046` 的 THintLines、
//     `Scenes/DrawScrn/HintWindowFamily.cs:451` 的 THintWindows + `DrawScrnEnv.HintWindows`）。
//   删除条件（须核对 `FStatePure.cs:75 GetHitLines` 是否用了接缝 `Add` 的 int 返回值）：
//   **未使用** —— 原文三处 `HintLines.Add` 全丢弃返回值，核对记录见交付报告 §D-P10-06。
// ---------------------------------------------------------------------------------------------

/// <summary>
/// 原文 `MyGetTickCount`（= Windows GetTickCount，MShare.pas）的可用性接缝。
///
/// **为什么要有这一层**：FState.pas 的倒计时控件（TCountDownLabel / TImgCountDownButton）与
/// NPC 动画控件（TNpcButton / TNpcLabel）的分支判据全部以 tick 差值作边界，其中
/// `>= 1000`（TCountDownLabel）与 `> 1000`（TImgCountDownButton）**只差一个等号**。
/// 用真实时钟无法确定性地命中"恰好等于 1000"这一格，也就无法对这两条分支写出可靠断言。
/// 因此把时钟收敛到一个可注入的点：默认直通 <see cref="Environment.TickCount"/>（与原文
/// `GetTickCount` 同语义，含 uint 回绕），测试可注入假时钟以精确落点。
/// 调用点仍保持 `MyGetTickCount` 的取值语义 —— 这是本波次唯一为"可验证性"引入的接缝。
/// </summary>
public static class FStateSeamClock
{
    /// <summary>测试注入点；为 null 时直通 Environment.TickCount。</summary>
    public static Func<uint> NowHandler;

    /// <summary>原文 MyGetTickCount（Windows GetTickCount，uint 毫秒，会回绕）。</summary>
    public static uint Now => NowHandler != null ? NowHandler() : (uint)Environment.TickCount;

    /// <summary>测试复位。</summary>
    public static void ResetForTests() => NowHandler = null;
}

/// <summary>Delphi Graphics.TFontStyles（set of，[Flags] 对应）。</summary>
[Flags]
public enum TFontStyles
{
    fsNone = 0,
    fsBold = 1,
    fsItalic = 2,
    fsUnderline = 4,
    fsStrikeOut = 8,
}

/// <summary>
/// MShare.pas 的提示字体三函数：GetHintFontSize / GetHintFontStyle / GetHintFontStroke。
/// 【接缝：待 MShare.pas 移植后接入真实实现（MShare.pas:11735-11760）】
/// 注意：这三个函数**不在 FState.pas** —— 既往车道报告把它们记在 FState 名下，本车道已更正
/// （见交付报告"既往事实更正"）。此处按原文调用形态做注入点，GetHitLines 原样传入这三个值。
/// </summary>
public static class MShareHintFont
{
    /// <summary>MShare.pas:11735 function GetHintFontSize:Integer（默认 9）。</summary>
    public static Func<int> GetHintFontSizeHandler;
    public static int GetHintFontSize() => GetHintFontSizeHandler?.Invoke() ?? 9;

    /// <summary>MShare.pas:11740 function GetHintFontStyle(FontStyles:TFontStyles):TFontStyles。</summary>
    public static Func<TFontStyles, TFontStyles> GetHintFontStyleHandler;
    public static TFontStyles GetHintFontStyle(TFontStyles FontStyles)
        => GetHintFontStyleHandler?.Invoke(FontStyles) ?? FontStyles;

    /// <summary>MShare.pas:11752 function GetHintFontStroke(IsStroke:Boolean = False):Boolean。</summary>
    public static Func<bool, bool> GetHintFontStrokeHandler;
    public static bool GetHintFontStroke(bool IsStroke = false)
        => GetHintFontStrokeHandler?.Invoke(IsStroke) ?? IsStroke;

    /// <summary>测试复位。</summary>
    public static void ResetForTests()
    {
        GetHintFontSizeHandler = null;
        GetHintFontStyleHandler = null;
        GetHintFontStrokeHandler = null;
    }
}

/// <summary>
/// MShare.pas TConfigClient 中 FState.pas 用到的、车道1 接缝未承载的成员。
/// 【接缝：待 MShare.pas 移植后并入车道1 的 TConfigClient】
/// </summary>
public static class ConfigClientExt
{
    /// <summary>MShare.pas ItemHintTextConfig 的项类型（httNeedJob*）。</summary>
    public enum TItemHintTextType
    {
        /// <summary>httNeedJobWarr（战士）</summary>
        httNeedJobWarr = 0,
        /// <summary>httNeedJobWizard（法师）</summary>
        httNeedJobWizard = 1,
        /// <summary>httNeedJobTaos（道士）</summary>
        httNeedJobTaos = 2,
    }

    /// <summary>MShare.pas TItemHintTextInfo.Text（职业需求提示文本）。</summary>
    public sealed class TItemHintTextInfo
    {
        public string Text = "";
    }

    /// <summary>MShare.pas g_ConfigClient.ItemHintTextConfig（按 TItemHintTextType 取项）。</summary>
    public static TItemHintTextInfo[] ItemHintTextConfig =
    {
        new(), new(), new(),
    };

    /// <summary>MShare.pas g_ConfigClient.boDisableDrogMagicIcon（禁止拖动技能图标）。</summary>
    public static byte boDisableDrogMagicIcon;

    /// <summary>MShare.pas g_ConfigClient.boSaveMagicIconPosition（保存技能图标位置）。</summary>
    public static byte boSaveMagicIconPosition;

    /// <summary>测试复位。</summary>
    public static void ResetForTests()
    {
        ItemHintTextConfig = new[] { new TItemHintTextInfo(), new TItemHintTextInfo(), new TItemHintTextInfo() };
        boDisableDrogMagicIcon = 0;
        boSaveMagicIconPosition = 0;
    }
}

/// <summary>
/// MShare.pas / ClMain.pas 中 TFrmDlg.SaveMagicButtons / LoadMagicButtons 需要而尚未移植的
/// 路径与 INI 面。
/// 【接缝：待 MShare.pas（g_sSelfFilePath / g_sPlugServerName / g_sPlugUserName /
///   MAGIC_ICONS_INI_FILE）与 FastIniFile 接入后，把 Save/LoadMagicButtons 的 INI 主体补成 1:1】
/// </summary>
public static class MagicButtonIniSeam
{
    /// <summary>MShare.pas g_sSelfFilePath（客户端自身目录）。</summary>
    public static string g_sSelfFilePath = ".\\";

    /// <summary>MShare.pas g_sPlugServerName（当前服务器名，用于 INI 文件名）。</summary>
    public static string g_sPlugServerName = "";

    /// <summary>MShare.pas g_sPlugUserName（当前用户名经 ProcessFileNameSpecialChar 过滤后）。</summary>
    public static string g_sPlugUserName = "";

    /// <summary>MShare.pas MAGIC_ICONS_INI_FILE 的 Format 模板。</summary>
    public static string MAGIC_ICONS_INI_FILE = "Config\\{0}_{1}_magic.ini";

    /// <summary>
    /// 真正的 INI 写盘。【接缝：待 FastIniFile 接入】
    /// 参数为 (文件名, 段名/键名/值 三元组序列)。
    /// </summary>
    public static Action<string, IReadOnlyList<(string Section, string Key, int Value)>> WriteIniHandler;

    /// <summary>测试复位。</summary>
    public static void ResetForTests()
    {
        g_sSelfFilePath = ".\\";
        g_sPlugServerName = "";
        g_sPlugUserName = "";
        WriteIniHandler = null;
    }
}

// ★ D-P10-06（续）：原 `THintWindows`（旧 599-602）与 `DrawScrn.CreateHintWindows`（旧 604-612）
//   两个接缝段已删除；`TFrmDlg.Create` 的 `FSayItemHintWin := DrawScrn.THintWindows.Create`
//   改指 `GXX.Client.Scenes.THintWindows`（字段类型同步由 object 改为 THintWindows）。

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

    /// <summary>
    /// MShare.pas `g_SelDeleteHumanInfo:TUserCharacterInfo`（"找回角色"对话框里选中的角色）。
    /// 原文 20594 只读它的 `sChrName`（Delphi `string[19]` 短串）；Core 里的 TUserCharacterInfo
    /// 是定长缓冲版，没有可直读的短串访问器，而本单元只用到这一个字段，
    /// 因此托管侧以 `g_SelDeleteHumanInfo_sChrName` 承载该单字段（命名带后缀以免被误当成整个记录）。
    /// 【接缝：待 MShare.pas 落地后换成真实 TUserCharacterInfo.sChrName】
    /// </summary>
    public static string g_SelDeleteHumanInfo_sChrName = "";

    /// <summary>
    /// MShare.pas `g_dwQueryMsgTick:LongWord`（"查询"类动作的 3 秒节流时间戳）。
    /// 原文在 17876/17885/18906/18914 等处以 `if MyGetTickCount &gt; g_dwQueryMsgTick` 判据使用。
    /// 注：Scenes 车道的 `MiniMapMessageState.QueryMsgTick`（MiniMapRender.cs:225）是同一全局的
    /// 另一份承载，那是**跨分区**的（不在本车道），故此处按本单元用到的形态独立承载。
    /// 【接缝：待 MShare.pas 落地后与 Scenes 的那份合并为单一全局】
    /// </summary>
    public static uint g_dwQueryMsgTick;

    /// <summary>MShare.pas `g_dwDealActionTick:LongWord`（交易动作节流时间戳；原文 17535 判据）。</summary>
    public static uint g_dwDealActionTick;

    /// <summary>MShare.pas `g_dwChallengeActionTick:LongWord`（挑战动作节流时间戳；原文 20815 判据）。</summary>
    public static uint g_dwChallengeActionTick;

    /// <summary>MShare.pas `g_boDealEnd:Boolean`（交易已结束；原文 17748 判据，**先判它**）。</summary>
    public static bool g_boDealEnd;

    /// <summary>MShare.pas `g_nDealGold:Integer`（交易中的金币数；原文 17748 判据）。</summary>
    public static int g_nDealGold;

    /// <summary>MShare.pas `g_boChallengeEnd:Boolean`（挑战已结束；原文 20791 判据）。</summary>
    public static bool g_boChallengeEnd;

    /// <summary>MShare.pas `g_nChallengeGold:Integer`（挑战中的金币数；原文 20791 判据）。</summary>
    public static int g_nChallengeGold;

    /// <summary>MShare.pas `g_dwChangeGroupModeTick:LongWord`（切换组队模式节流时间戳；原文 18923/18942 判据）。</summary>
    public static uint g_dwChangeGroupModeTick;

    /// <summary>MShare.pas `g_boAllowGroup:Boolean`（是否允许组队；原文 18924/18943 **取反**后回写并上报）。</summary>
    public static bool g_boAllowGroup;

    /// <summary>MShare.pas `g_DealDlgItem:TClientItem`（交易对话框当前物品；原文 17620 赋值）。</summary>
    public static TClientItem g_DealDlgItem;

    /// <summary>
    /// MShare.pas `g_GameGoldDealRemoteItems:array[0..8] of TClientItem`（元宝交易菜单的 9 个远端物品栏）。
    /// 原文 18663 的 `SafeFillChar(..., SizeOf(TClientItem) * 9, #0)` 在托管侧等价于 `Array.Clear`。
    /// </summary>
    public static TClientItem[] g_GameGoldDealRemoteItems = new TClientItem[9];

    /// <summary>
    /// MShare.pas `g_GameGoldDeal:TGameGoldDeal`（元宝交易菜单状态）。
    /// 原文 18664 的 `SafeFillChar(..., SizeOf(TGameGoldDeal), #0)` 在托管侧等价于 `= default`。
    /// </summary>
    public static TGameGoldDeal g_GameGoldDeal;

    /// <summary>MShare.pas `g_nMinMapX:Integer`（小地图上鼠标 X；原文 18224 赋值）。</summary>
    public static int g_nMinMapX;

    /// <summary>MShare.pas `g_nMinMapY:Integer`（小地图上鼠标 Y；原文 18225 赋值）。</summary>
    public static int g_nMinMapY;

    /// <summary>
    /// MShare.pas `g_MouseUserStateItem:TClientItem`（人物状态窗口上鼠标所指的物品）。
    /// 原文 17802 / 2612 只做 `g_MouseUserStateItem.S.Name := ''`（清空短串名）。
    /// 这里只暴露**被用到的那一个字段**，而不是整份 645 KB 级结构体
    /// （**禁止按值传递巨型结构体** —— 台账本轮新规程）。
    /// 【接缝：待 MShare.pas 落地后换成真实 TClientItem 的 s.Name】
    /// </summary>
    public static string g_MouseUserStateItem_sName = "";

    /// <summary>测试用复位。</summary>
    public static void ResetForTests()
    {
        g_SellDlgItem = default;
        g_ExtBagOpenItemCount = 0;
        g_SelDeleteHumanInfo_sChrName = "";
        g_dwQueryMsgTick = 0;
        g_dwDealActionTick = 0;
        g_dwChallengeActionTick = 0;
        g_boDealEnd = false;
        g_nDealGold = 0;
        g_boChallengeEnd = false;
        g_nChallengeGold = 0;
        g_dwChangeGroupModeTick = 0;
        g_boAllowGroup = false;
        g_DealDlgItem = default;
        g_GameGoldDealRemoteItems = new TClientItem[9];
        g_GameGoldDeal = default;
        g_nMinMapX = 0;
        g_nMinMapY = 0;
        g_MouseUserStateItem_sName = "";
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
    /// <summary>DxControls.pas TDxControl.OnMouseMove 的签名（Sender/Shift/X/Y）。</summary>
    public delegate void TDxMouseMoveEvent(object sender, TShiftState shift, int x, int y);

    /// <summary>DxControls.pas TDxControl.OnMove 的签名（Sender）。</summary>
    public delegate void TDxMoveEvent(object sender);

    private sealed class Extra
    {
        public int AddData1;
        public int AddData2;
        public bool DisableHideCtrl;
        public TDxMouseMoveEvent OnMouseMove;
        public TDxMoveEvent OnMove;
        public Action SetFocusHandler;
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

    /// <summary>DxControls.pas TDxControl.OnMouseMove（车道1 接缝只提供了 OnMouseMoveEx）。</summary>
    public static TDxMouseMoveEvent GetOnMouseMove(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).OnMouseMove;
    public static void SetOnMouseMove(GXX.Client.GUI.DxComponent.TDxControl c, TDxMouseMoveEvent v) => Of(c).OnMouseMove = v;

    /// <summary>DxControls.pas TDxControl.OnMove（控件被拖动后触发，用于保存图标位置）。</summary>
    public static TDxMoveEvent GetOnMove(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).OnMove;
    public static void SetOnMove(GXX.Client.GUI.DxComponent.TDxControl c, TDxMoveEvent v) => Of(c).OnMove = v;

    /// <summary>DxControls.pas TDxControl.SetFocus。【接缝：待 DxControls.pas 全量移植】</summary>
    public static void SetFocus(GXX.Client.GUI.DxComponent.TDxControl c) => Of(c).SetFocusHandler?.Invoke();
    public static void SetSetFocusHandler(GXX.Client.GUI.DxComponent.TDxControl c, Action v) => Of(c).SetFocusHandler = v;
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

    // ============================================================================================
    // 切片 3（调度方授权 B-2 增量补充权）：补 `frmMain` 上 FState.pas 真正调用到的成员。
    //
    // ★ 为什么放在**本类**而不是另开 `Scenes/FStateClMainSeam.cs`：
    //   调度方授权的分区含 `!GXX.CSharp/src/GXX.Client/Scenes/FStateClMainSeam.cs`，
    //   但本类**已经叫** `FStateClMainSeam`（在 `GXX.Client.GUI.Share`）。
    //   若在同一程序集里再造一个**同名**类型，凡同时 `using` 两个命名空间的文件都会 CS0104，
    //   正是本工程反复登记过的"同一 Delphi 单元两条车道各造一套接缝"事故。
    //   ⇒ 因此**沿用既有类**做增量补充，不新建同名类（用不到那份授权，也不产生二义性）。
    //
    // 逐条计数（本切片新补 4 个成员 + 1 个转接方法）：
    //   1) Navigate(string)              —— 原文 20579 `frmMain.Navigate(g_ClientConfig.sHomePage)`
    //   2) SendDActionLogClick()         —— 原文 20584 `frmMain.SendDActionLogClick`
    //   3) SendGetBackDeleteChr(string)  —— 原文 20595 `frmMain.SendGetBackDeleteChr(...)`
    //   4) SendClientMessage(int,int,int,int,int,string)
    //                                    —— 原文 24920 `FrmMain.SendClientMessage(CM_CUSTOM_BUTTON_CLICK, Tag, 0,0,0, '')`
    //   5) TakeHorse(TActor)             —— 原文 18845/20573 `g_MySelf.TakeHorse`（见下方说明）
    // 仍缺（本轮**未**补，后续车道注意）：`Close`（原文 2249/2297）、
    //   `ReConnectClientSocketGate`（24471）、`SendSay`（17931）、`SendGuildHome`（17878）、
    //   `SendGuildMemberList`（17887）、`SendGuildAddMem`/`SendGuildDelMem`（17896/17903）、
    //   `SendAdjustBonus`（17987）、`SendCancelGameGoldDealItem`（18702/18706）、
    //   `SendGetShopItems`（18094）等约 12 条 —— 它们的调用点**不在本切片**。
    // ============================================================================================

    /// <summary>ClMain.pas `frmMain.Navigate(sUrl:string)`（打开官网/公告页）。</summary>
    public static Action<string> NavigateHandler;

    /// <summary>
    /// MShare.pas `g_ClientConfig.sHomePage:string`（官网地址）。
    /// 【接缝：车道1 的 `GXX.Client.GUI.Mir.TConfigClient`（MirForms.cs:23）目前**没有**这个字段，
    ///  而它不在本车道分区，故把该字段的最小承载放在这里；待 TConfigClient 补齐后改回
    ///  `g_ClientConfig.sHomePage`】默认值与 M2 端 `M2Config.sHomePage` 一致
    ///  （`GXX.M2Server/Engine/M2Config.ClientConf.cs:128` = "http://www.gxxm2.com"）。
    /// </summary>
    public static string sHomePage = "http://www.gxxm2.com";

    /// <summary>ClMain.pas frmMain.Navigate（原文 20579 调用）。</summary>
    public static void Navigate(string sUrl) => NavigateHandler?.Invoke(sUrl);

    /// <summary>ClMain.pas `frmMain.SendDActionLogClick`（无参动作日志上报）。</summary>
    public static Action SendDActionLogClickHandler;

    /// <summary>ClMain.pas frmMain.SendDActionLogClick（原文 20584 调用，原文**无括号**）。</summary>
    public static void SendDActionLogClick() => SendDActionLogClickHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendGetBackDeleteChr(sChrName:string)`（找回已删除角色）。</summary>
    public static Action<string> SendGetBackDeleteChrHandler;

    /// <summary>ClMain.pas frmMain.SendGetBackDeleteChr（原文 20595 调用）。</summary>
    public static void SendGetBackDeleteChr(string sChrName)
        => SendGetBackDeleteChrHandler?.Invoke(sChrName);

    /// <summary>
    /// ClMain.pas `frmMain.SendClientMessage(wIdent:Word; nRecog, nParam, nParam2, nParam3:Integer; sMsg:string)`。
    /// 原文 24920 用 `(CM_CUSTOM_BUTTON_CLICK, Tag, 0, 0, 0, '')` 六参形态。
    /// </summary>
    public static Action<int, int, int, int, int, string> SendClientMessageHandler;

    /// <summary>ClMain.pas frmMain.SendClientMessage（原文 24920 调用）。</summary>
    public static void SendClientMessage(int wIdent, int nRecog, int nParam, int nParam2, int nParam3, string sMsg)
        => SendClientMessageHandler?.Invoke(wIdent, nRecog, nParam, nParam2, nParam3, sMsg);

    /// <summary>
    /// Actor.pas `TActor.TakeHorse`（原文 18845 `g_MySelf.TakeHorse`、20573 同形）。
    /// Actor.pas 的托管承接方是 `GXX.Client.Scenes.TActor`，车道1 的 TActor 上尚无该方法；
    /// 原文它只发一个 `CM_TAKEHORSE` 客户端消息，故此处按同形转发到 SendClientMessage。
    /// 【接缝：待 Actor.pas 落地 TakeHorse 后，把 DDownHorseClick/DBotHorseClick 改调真实方法】
    /// </summary>
    public static Action<object> TakeHorseHandler;

    /// <summary>Actor.pas TActor.TakeHorse（原文 18845 / 20573 调用）。</summary>
    public static void TakeHorse(object actor) => TakeHorseHandler?.Invoke(actor);

    /// <summary>ClMain.pas `frmMain.SendGuildHome`（原文 17878 调用，无参）。</summary>
    public static Action SendGuildHomeHandler;

    /// <summary>ClMain.pas frmMain.SendGuildHome（原文 17878 调用）。</summary>
    public static void SendGuildHome() => SendGuildHomeHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendGuildMemberList`（原文 17887 调用，无参）。</summary>
    public static Action SendGuildMemberListHandler;

    /// <summary>ClMain.pas frmMain.SendGuildMemberList（原文 17887 调用）。</summary>
    public static void SendGuildMemberList() => SendGuildMemberListHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendCancelDeal`（原文 17537 调用，无参）。</summary>
    public static Action SendCancelDealHandler;

    /// <summary>ClMain.pas frmMain.SendCancelDeal（原文 17537 调用）。</summary>
    public static void SendCancelDeal() => SendCancelDealHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendChangeDealGold(nGold:Integer)`（原文 17750 调用）。</summary>
    public static Action<int> SendChangeDealGoldHandler;

    /// <summary>ClMain.pas frmMain.SendChangeDealGold（原文 17750 调用）。</summary>
    public static void SendChangeDealGold(int nGold) => SendChangeDealGoldHandler?.Invoke(nGold);

    /// <summary>ClMain.pas `frmMain.SendCancelChallenge`（原文 20817 调用，无参）。</summary>
    public static Action SendCancelChallengeHandler;

    /// <summary>ClMain.pas frmMain.SendCancelChallenge（原文 20817 调用）。</summary>
    public static void SendCancelChallenge() => SendCancelChallengeHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendChangeChallengeGold(nGold:Integer)`（原文 20793 调用）。</summary>
    public static Action<int> SendChangeChallengeGoldHandler;

    /// <summary>ClMain.pas frmMain.SendChangeChallengeGold（原文 20793 调用）。</summary>
    public static void SendChangeChallengeGold(int nGold) => SendChangeChallengeGoldHandler?.Invoke(nGold);

    /// <summary>ClMain.pas `frmMain.SendDealTry`（原文 18916 调用，无参）。</summary>
    public static Action SendDealTryHandler;

    /// <summary>ClMain.pas frmMain.SendDealTry（原文 18916 调用）。</summary>
    public static void SendDealTry() => SendDealTryHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendChallengeTry`（原文 18908 调用，无参）。</summary>
    public static Action SendChallengeTryHandler;

    /// <summary>ClMain.pas frmMain.SendChallengeTry（原文 18908 调用）。</summary>
    public static void SendChallengeTry() => SendChallengeTryHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.ReConnectClientSocketGate`（原文 24471 调用，无参）。</summary>
    public static Action ReConnectClientSocketGateHandler;

    /// <summary>ClMain.pas frmMain.ReConnectClientSocketGate（原文 24471 调用）。</summary>
    public static void ReConnectClientSocketGate() => ReConnectClientSocketGateHandler?.Invoke();

    /// <summary>ClMain.pas `frmMain.SendGroupMode(boAllowGroup:Boolean)`（原文 18926/18945 调用）。</summary>
    public static Action<bool> SendGroupModeHandler;

    /// <summary>ClMain.pas frmMain.SendGroupMode（原文 18926/18945 调用）。</summary>
    public static void SendGroupMode(bool boAllowGroup) => SendGroupModeHandler?.Invoke(boAllowGroup);

    /// <summary>ClMain.pas `frmMain.SendDelDealItem(Item:TClientItem)`（原文 17621 调用）。</summary>
    public static Action<TClientItem> SendDelDealItemHandler;

    /// <summary>ClMain.pas frmMain.SendDelDealItem（原文 17621 调用）。</summary>
    public static void SendDelDealItem(TClientItem item) => SendDelDealItemHandler?.Invoke(item);

    /// <summary>ClMain.pas `frmMain.Close`（原文 2249 / 2297 调用，无参）。</summary>
    public static Action CloseHandler;

    /// <summary>ClMain.pas frmMain.Close（原文 2297 调用）。</summary>
    public static void Close() => CloseHandler?.Invoke();

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
        NavigateHandler = null;
        sHomePage = "http://www.gxxm2.com";
        SendDActionLogClickHandler = null;
        SendGetBackDeleteChrHandler = null;
        SendClientMessageHandler = null;
        TakeHorseHandler = null;
        SendGuildHomeHandler = null;
        SendGuildMemberListHandler = null;
        SendCancelDealHandler = null;
        SendChangeDealGoldHandler = null;
        SendCancelChallengeHandler = null;
        SendChangeChallengeGoldHandler = null;
        SendDealTryHandler = null;
        SendChallengeTryHandler = null;
        ReConnectClientSocketGateHandler = null;
        SendGroupModeHandler = null;
        SendDelDealItemHandler = null;
        CloseHandler = null;
        FStateMShareSeam.ResetForTests();
        MShareGlobalsReset.ResetForTests();
    }
}
