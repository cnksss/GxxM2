using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace GXX.Client.DxComponent;

// =====================================================================================
// DxControls.pas（4,147 行）余部 1:1 逐字移植。
//
// 上一波（车道5）已在 DxComponentCommon.cs 里建立了接缝，但**该接缝比台账 §9.3 描述的更薄**：
// 本轮逐条核对（read DxComponentCommon.cs 788-1409 + 全文 grep）后，实际只有这些成员可直接复用：
//   TDxFont / TDxCaptionColor / TDxBorderColor / TDxImageIndex / TDxRect / TDxPoint / TDxColor /
//   DxRectUtil；接口 IDxTexture / IDxImageLibrary / IDxSurfacePainter（+TDxNullPainter /
//   TDxRecordingPainter）/ TDxFontEnv / TDxTextImageInfo；TDxControl 的
//   Painter / FontEnv / DxOwner / VirtualRectOverride / Caption / AutoSize / ClientRect /
//   VisibleRect / VirtualRect / Left / Top / Width / Height / Visible / Enabled / Designing /
//   Center / OwnerMove / EnableFocus / EnableMouse / CanMouse / Transparent / BackgroundColor /
//   DrawBorder / Floating / MouseEvents / Align / ReferenceX / AdjustYByHeight / TopAlignment /
//   TabOrder / ControlID / ShowName / HintText / MouseDowned / MouseMoveed / Checked / BlendMode /
//   MouseDownBlendMode / MouseMoveBlendMode / ImageIndex / BorderColor / DrawCaptionFont / OnPaint /
//   OnInRealArea / AutoSizeSetFlag / SetOnGetImage / GetPosition / SetPosition / CheckAutoSizeBase /
//   ReallyRect / CanDraw / ReallyPaintRect / DrawCaption / GetImageInfos / FindFont / TextHeight /
//   DoPaint / InRange / DoOnInRealArea / Repaint / MouseDown / MouseMove / MouseUp / PopupMenuHook /
//   DoMouseUp / ImageIndexChange(sender) / DoCaptionChange / DoResize。
//
// **未**在上游接缝里、因而由本文件承接的 TDxControl 成员（原文行号见各方法注释）：
//   SetFocus / SetCapture / FocusSomething / GetRootCtrl / FindControl / GetCtrl / FindComponent /
//   Insert / Remove / InserComponent / RemoveComponent / DestroyComponents / Notification /
//   ComponentCount / Components[] / ComponentIndex / SeComponentIndex / Initialize / Finalize /
//   ToFront / ToBack / BringToFront / SentToBack / SetClientRect / Show / Hide / Close / SetEnabled /
//   SetOwner / MoveBy / ResizeBy / ApplyConstraint / Assign / FormatCaption / DoUpdate / Update /
//   ImageIndexChange / SetAlign / DoResize / SetAutoSize / SetCaptionA(V) / SetShowNameA /
//   SetReferenceX / SetAdjustYByHeight / SetTopAlignment / DoHide / DoDisable / DoEnable / DoShow /
//   SetMouseMoveed / SetMouseDowned / MouseDowned / MouseMoveed / Focused / SetCenter / WidthCenter /
//   HeightCenter / SetCenterA / SetScrollControl / ReleaseControl / SetDesigning / GetPaintRect /
//   FillRect(×2) / FillRectAlpha(×2) / FrameRect(×2) / DrawRect(×3) / DrawRectColor(×2) /
//   DrawRectColorAlpha(×2) / ReallyPaintRect 的 GetPaintRect 兄弟 / Paint / DoClick / DoDblClick /
//   DblClick / KeyDown / KeyPress / KeyUp / MouseWheelDown / MouseWheelUp / MouseDown / MouseMove /
//   MouseUp / CanMove / Move / SetMousePoint / FindActiveControl / GetAllSubComponents /
//   SetClipboardText / GetClipboardText / DebugOutStr / FindDxComponent / MakeGuiName。
//
// DFM: TDxControl / TDxControlEngine / TDxScrollControl 无同名 .dfm
//      （Source\Client-HGE 全树 glob 核实，0 命中），设计期取值落在各 .GUI 文件的 Tgui* 记录里
//      —— 不按 .dfm 对齐（沿用上一波的注释写法）。
//
// -------------------------------------------------------------------------------------
// 托管侧表达方式（**上游文件只读**是硬约束：DxComponentCommon.cs 的 TDxControl 不是 partial，
// 且本轮不许改它，故无法用 partial class 续写）：
//
//  1. 需要「成为 TDxControl 成员」的方法 → 收纳进静态类 DxControlOps，第一参数即原文 Self。
//     调用形态与原文逐字对应，例如原文 `Control[I].BringToFront` → `DxControlOps.BringToFront(c)`；
//     每个方法注明替代的原文成员名与行号。
//  2. 上游 protected 的虚方法（DoShow / DoHide / DoEnable / DoDisable / DoCaptionChange /
//     DoMouseDown / DoMouseMove / DoMouseUp / DoMouseEnter / DoMouseLeave / DoFocused / DoUnFocused /
//     DoUpdate / DoMove / DoClick / DoDblClick / DoResize(ref)）用 DxProtected 的「开实例委托」
//     承接：Delegate.CreateDelegate 绑到 MethodInfo，调用仍走**虚分派**（子类覆写有效），
//     且无每次调用的反射开销。DoResize 因 ref 参数改用缓存的 MethodInfo.Invoke。
//  3. FComponents（原文 TList）→ ConditionalWeakTable（键为父控件）。原文 Remove 在表空时
//     `FComponents.Free; FComponents := nil`，弱表不拦 GC，「无表 = ComponentCount 0」一致。
//  4. FRootCtrl / FName / FRawText / SpotX / SpotY / FMouseDownX / FMouseDownY / ModalControl /
//     FOnKeyDown… / FOnShow… 等上游未暴露的字段 → DxControlHooks 的弱表槽位 + DxRootRegistry 逆查表。
//     FOnKeyDown 等事件槽位在本文件中以 **同名公开字段** 暴露在 TDxControlHooks 上
//     （原文 `property OnKeyDown` 的等价落点），不叫 OnKeyDown 以免与 WinForms 的
//     `Control.KeyDown` 事件/`OnKeyDown` 保护方法撞名 —— 见第 5 条。
//  5. 命名冲突：TDxControl 派生自 WinForms Control，`KeyDown/KeyPress/KeyUp/DblClick/Update/
//     Paint/Initialize/Move/FindActiveControl` 等名字在 Control 上已存在（部分是事件、部分非虚），
//     无法覆写。故 TDxControlEngine 只对 **MouseDown/MouseMove/MouseUp**（上游确为 virtual）用
//     override，其余布尔入口统一命名为 `Port*`（原文同名方法的托管落点），返回值即原文的 Boolean。
//     上游接缝已沿用同样手法（HintText / DxOwner / FontEnv / Painter / CheckAutoSizeBase）。
// =====================================================================================

// -------------------------------------------------------------------------------------
// DxComponents.pas 49-72 里被 DxControls / DxImageButton / DxImageForm 引用、上一波未建立的类型
// -------------------------------------------------------------------------------------

/// <summary>DxComponents.pas 49 TDrawAligment。</summary>
public enum TDrawAligment { daFill, daBottom }

/// <summary>DxComponents.pas 54 TClickSound。</summary>
public enum TClickSound { csNone, csStone, csGlass, csNorm }

/// <summary>DxComponents.pas 56 TNotifyEvent（stdcall of object）。</summary>
public delegate void TNotifyEvent(object sender);

/// <summary>DxComponents.pas 57 TMouseEvent。</summary>
public delegate void TMouseEvent(object sender, TDxMouseButton button, TDxShiftState shift, int x, int y);

/// <summary>DxComponents.pas 58 TMouseMoveEvent。</summary>
public delegate void TMouseMoveEvent(object sender, TDxShiftState shift, int x, int y);

/// <summary>DxComponents.pas 59 TKeyEvent（var Key:Word）。</summary>
public delegate void TKeyEvent(object sender, UShortRef key, TDxShiftState shift);

/// <summary>DxComponents.pas 60 TKeyPressEvent（var Key:Char）。</summary>
public delegate void TKeyPressEvent(object sender, CharRef key);

/// <summary>DxComponents.pas 68 TAnimationFrameChangedEvent。</summary>
public delegate void TAnimationFrameChangedEvent(object sender, int animationIndex, int playCount, int frame);

/// <summary>DxComponents.pas 72 TOnClickSound。</summary>
public delegate void TOnClickSound(object sender, TClickSound clickSound);

/// <summary>DxControls.pas 32 TOnGetItem。</summary>
public delegate void TOnGetItem(object sender, TDxControlRef dxControl, string s, int fc, int bc);

/// <summary>DxControls.pas 28 TOnInsertControl。</summary>
public delegate void TOnInsertControl(object sender, TDxControl control);

/// <summary>DxControls.pas 29 TOnRemoveControl。</summary>
public delegate void TOnRemoveControl(object sender, TDxControl control);

/// <summary>DxControls.pas 30 TOnFindActiveControl。</summary>
public delegate TDxControl TOnFindActiveControl(object sender, int x, int y);

/// <summary>Delphi `var Key:Word`（16 位无符号）出参。</summary>
public sealed class UShortRef
{
    public ushort Value;
    public UShortRef(ushort value) { Value = value; }
}

/// <summary>Delphi `var Key:Char` 出参。</summary>
public sealed class CharRef
{
    public char Value;
    public CharRef(char value) { Value = value; }
}

/// <summary>Delphi `var DxControl:TDxControl` 出参（TOnGetItem 用）。</summary>
public sealed class TDxControlRef
{
    public TDxControl Value;
    public TDxControlRef(TDxControl value) { Value = value; }
}

/// <summary>Delphi Classes.TOperation。</summary>
public enum TOperation { opInsert, opRemove }

/// <summary>
/// 接缝：原文 DxControls.pas 1143 里 `Application.MainForm` 的托管落点。
/// 托管侧没有 VCL 的全局 Application 对象；引擎命名需要一个「主窗体」基准，
/// 故以本接缝承接（缺省 nil，等价于原文 MainForm 尚未创建的情形）。
/// </summary>
public static class TDxApplication
{
    /// <summary>原文 Application.MainForm（缺省 nil）。</summary>
    public static Control MainForm;

    /// <summary>供 TDxControlEngine 使用（避免与 WinForms Application 撞名而写全限定）。</summary>
    internal static object GetMainFormForNaming() => MainForm;
}

/// <summary>
/// 接缝：MyGetTickCount（原文 HUtil32 / SDK；DxImageButton / DxImageForm 的动画计时用）。
/// 缺省回落 Environment.TickCount —— 与 Win32 GetTickCount 同为无符号回绕语义。
/// </summary>
public static class DxTickCount
{
    /// <summary>注入点（原文 MyGetTickCount 的全局实现）。</summary>
    public static Func<uint> Provider;

    /// <summary>原文 MyGetTickCount。</summary>
    public static uint MyGetTickCount() => Provider != null ? Provider() : (uint)Environment.TickCount;
}

// -------------------------------------------------------------------------------------
// protected 虚方法承接层
// -------------------------------------------------------------------------------------

/// <summary>
/// 把上游 TDxControl 的 protected 虚方法绑成**开实例委托**（首参 Self）。
/// 绑定只做一次；调用点是普通委托调用，且仍走虚分派（子类 override 生效）。
///
/// 上游接缝实际提供的 protected virtual 只有这 5 个（逐条 grep 核实 DxComponentCommon.cs）：
///   DoCaptionChange（1077）/ DoResize(ref)（1080）/ DoPaint（1358）/ DoOnInRealArea（1377）/
///   CheckAutoSizeBase（1135），另有 public virtual Repaint / InRange / MouseDown / MouseMove / MouseUp
///   与 protected virtual bool CanMove（1395）。
/// 原文其余 Do*（DoShow/DoHide/DoEnable/DoDisable/DoMouseDown/DoMouseMove/DoMouseUp/DoMouseEnter/
/// DoMouseLeave/DoFocused/DoUnFocused/DoUpdate/DoMove/DoClick/DoDblClick）**不在接缝里**，
/// 故文件其余位置直接调用 DxControlOps 自己的实现（原文语义等价），不再经本层。
/// </summary>
internal static class DxProtected
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

    internal delegate void VoidD(TDxControl self);

    /// <summary>TDxControl.DoCaptionChange（1077，protected virtual）。</summary>
    internal static readonly VoidD DoCaptionChange =
        (VoidD)Delegate.CreateDelegate(typeof(VoidD), null, typeof(TDxControl).GetMethod(
            "DoCaptionChange", Flags) ?? throw new MissingMethodException("TDxControl", "DoCaptionChange"));

    /// <summary>TDxControl.DoPaint（1358，protected virtual）。</summary>
    internal static readonly VoidD DoPaint =
        (VoidD)Delegate.CreateDelegate(typeof(VoidD), null, typeof(TDxControl).GetMethod(
            "DoPaint", Flags) ?? throw new MissingMethodException("TDxControl", "DoPaint"));

    /// <summary>DoResize(var NewRect:TRect) —— ref 参数不能用开实例委托绑，故保留 MethodInfo。</summary>
    private static readonly MethodInfo DoResizeMi =
        typeof(TDxControl).GetMethod("DoResize", Flags)
        ?? throw new MissingMethodException(typeof(TDxControl).FullName, "DoResize");

    /// <summary>DoResize 调用（ref 语义）。</summary>
    internal static void CallDoResize(TDxControl self, ref TDxRect rect)
    {
        object[] args = { rect };
        DoResizeMi.Invoke(self, args);
        rect = (TDxRect)args[0];
    }
}

// -------------------------------------------------------------------------------------
// 接缝：绘图器的着色绘制扩展
// -------------------------------------------------------------------------------------

/// <summary>
/// 接缝扩展：GameCanvas.DrawColor / DrawColorAlpha / FillRectAlpha
/// （DxControls.pas 3286 / 3544 / 3600 / 3652 / 3704）。
/// 上游 IDxSurfacePainter 未含这三个方法，故以独立接口 + 判定承接 ——
/// 未实现时退化为无着色/无 alpha 版本，保证纯几何可单测。
/// </summary>
public interface IDxSurfacePainterExt
{
    /// <summary>GameCanvas.FillRectAlpha(Rect, Color, Alpha)。</summary>
    void FillRectAlpha(TDxRect destRect, TDxRect virtualRect, TDxRect visibleRect, int color, byte alpha);

    /// <summary>GameCanvas.DrawColor(x, y, SrcRect, Texture, Color, BlendMode)。</summary>
    void DrawColor(int x, int y, TDxRect srcRect, IDxTexture texture, int color, int blendMode);

    /// <summary>GameCanvas.DrawColorAlpha(x, y, SrcRect, Texture, Color, Alpha, BlendMode)。</summary>
    void DrawColorAlpha(int x, int y, TDxRect srcRect, IDxTexture texture, int color, byte alpha, int blendMode);

    /// <summary>GameCanvas.Draw(x, y, SrcRect, Texture, BlendMode)（DxImageForm 1240 用）。</summary>
    void DrawBlend(int x, int y, TDxRect srcRect, IDxTexture texture, int blendMode);

    /// <summary>GameCanvas.StretchDraw(DestRect, SrcRect, Texture, BlendMode)（DxImageForm 1237 用）。</summary>
    void StretchDraw(TDxRect destRect, TDxRect srcRect, IDxTexture texture, int blendMode);
}

/// <summary>IDxSurfacePainterExt 的转发落点（未实现时退化）。</summary>
public static class DxPainterExt
{
    public static void FillRectAlpha(IDxSurfacePainter p, TDxRect dest, TDxRect vt, TDxRect vb, int color, byte alpha)
    {
        if (p is IDxSurfacePainterExt e) e.FillRectAlpha(dest, vt, vb, color, alpha);
        else p.FillRect(dest, vt, vb, color);
    }

    public static void DrawColor(IDxSurfacePainter p, int x, int y, TDxRect src, IDxTexture t, int color, int blendMode)
    {
        if (p is IDxSurfacePainterExt e) e.DrawColor(x, y, src, t, color, blendMode);
        else p.Draw(x, y, src, t);
    }

    public static void DrawColorAlpha(IDxSurfacePainter p, int x, int y, TDxRect src, IDxTexture t, int color,
        byte alpha, int blendMode)
    {
        if (p is IDxSurfacePainterExt e) e.DrawColorAlpha(x, y, src, t, color, alpha, blendMode);
        else p.Draw(x, y, src, t);
    }

    /// <summary>带 BlendMode 的 5 参 GameCanvas.Draw（未实现时退化为 4 参 Draw）。</summary>
    public static void DrawBlend(IDxSurfacePainter p, int x, int y, TDxRect src, IDxTexture t, int blendMode)
    {
        if (p is IDxSurfacePainterExt e) e.DrawBlend(x, y, src, t, blendMode);
        else p.Draw(x, y, src, t);
    }

    /// <summary>GameCanvas.StretchDraw（未实现时退化为原尺寸 Draw）。</summary>
    public static void StretchDraw(IDxSurfacePainter p, TDxRect dest, TDxRect src, IDxTexture t, int blendMode)
    {
        if (p is IDxSurfacePainterExt e) e.StretchDraw(dest, src, t, blendMode);
        else p.Draw(dest.Left, dest.Top, src, t);
    }
}

// -------------------------------------------------------------------------------------
// FRootCtrl 逆查表 / 上游未暴露字段的槽位
// -------------------------------------------------------------------------------------

/// <summary>
/// FRootCtrl（DxControls.pas 179 / 509）的托管落点：控件 → 根引擎 的逆查表。
/// 原文每个 TDxControl 构造时按 Owner 推导 FRootCtrl；托管侧由引擎在挂接时登记
/// （构造推导同样保留在 TDxControlEngine 构造里）。
/// </summary>
public static class DxRootRegistry
{
    private static readonly ConditionalWeakTable<TDxControl, TDxControlEngine> Table = new();

    /// <summary>原文 TDxControl.RootCtrl（读 FRootCtrl）。根引擎的 RootCtrl 是它自己。</summary>
    public static TDxControlEngine Get(TDxControl c)
    {
        if (c is TDxControlEngine engine) return engine;
        return Table.TryGetValue(c, out var e) ? e : null;
    }

    /// <summary>设置某控件及其整棵已挂子树的 FRootCtrl。</summary>
    public static void Set(TDxControl c, TDxControlEngine engine)
    {
        if (Table.TryGetValue(c, out var old) && ReferenceEquals(old, engine)) return;
        Table.Remove(c);
        if (engine != null) Table.Add(c, engine);
        for (int i = 0; i < DxControlOps.ComponentCount(c); i++)
            Set(DxControlOps.Components(c, i), engine);
    }
}

/// <summary>
/// 原文 TDxControl 里上游接缝**未暴露**的字段与事件的托管槽位：
/// FName / FRawText / SpotX / SpotY / FMouseDownX / FMouseDownY / FModalControl /
/// FPopupMenu 及 FOnResize / FOnShow / FOnHide / FOnCreate / FOnDestroy / FOnStartPaint /
/// FOnStartSubPaint / FOnStopPaint / FOnFocused / FOnUpDate / FOnMove / FOnFindActiveControl /
/// FOnKeyDown / FOnKeyPress / FOnKeyUp / FOnClick / FOnDblClick / FOnMouseDown / FOnMouseMove /
/// FOnMouseUp。
///
/// 事件槽位用字段（不是属性）承接原文的 `property OnXxx`：读 `GetOnXxx` / 写 `SetOnXxx`，
/// 避免与 WinForms 的 `Control.OnXxx` 保护方法 / `Xxx` 事件撞名。
/// </summary>
public static class DxControlHooks
{
    private static readonly ConditionalWeakTable<TDxControl, object[]> Table = new();
        // ConditionalWeakTable.GetOrCreateValue 要求值类型有**无参构造函数** —— object[]/string[]/int[]
    // 都不满足（RuntimeType.ActivatorCache 会抛 MissingMethodException），故用 GetValue + 工厂。
    private static object[] HooksArr(TDxControl c) => Table.GetValue(c, _ => new object[32]);

    private const int IxOnResize = 0;
    private const int IxOnShow = 1;
    private const int IxOnHide = 2;
    private const int IxOnCreate = 3;
    private const int IxOnDestroy = 4;
    private const int IxOnStartPaint = 5;
    private const int IxOnStartSubPaint = 6;
    private const int IxOnStopPaint = 7;
    private const int IxOnFocused = 8;
    private const int IxOnUpdate = 9;
    private const int IxOnMove = 10;
    private const int IxOnFindActiveControl = 11;
    private const int IxRawText = 12;
    private const int IxOnKeyDown = 13;
    private const int IxOnKeyPress = 14;
    private const int IxOnKeyUp = 15;
    private const int IxOnClick = 16;
    private const int IxOnDblClick = 17;
    private const int IxOnMouseDown = 18;
    private const int IxOnMouseMove = 19;
    private const int IxOnMouseUp = 20;
    private const int IxModalControl = 21;
    private const int IxOnGetImage = 22;
    private const int IxMoveRange = 23;
    private const int IxOnMouseEnter = 25;
    private const int IxOnMouseLeave = 26;

    /// <summary>
    /// 原文 TDxControl.OnGetImage（494；读走 FImageIndex.OnGetImage，写走 FImageIndex.SetOnGetImage）。
    /// 上游接缝只暴露了 SetOnGetImage（无 getter），故补一个槽位镜像写入。
    /// </summary>
    public static Action<TDxImageIndex, TImageType> GetOnGetImage(TDxControl c)
        => (Action<TDxImageIndex, TImageType>)HooksArr(c)[IxOnGetImage];

    public static void SetOnGetImage(TDxControl c, Action<TDxImageIndex, TImageType> v)
    {
        HooksArr(c)[IxOnGetImage] = v;
        c.SetOnGetImage(v);                        // 同步到上游接缝（FImageIndex.OnGetImage）
    }

    /// <summary>
    /// 原文 3065-3078 TDxControl.DoPaint（`{$IF CLIENTEXE = 1} CheckAutoSize(); {$IFEND}`）的转发落点。
    /// 上游接缝的等价物是 protected CheckAutoSizeBase，故此处只驱动挂上的槽位。
    /// </summary>
    public static void DoPaint(TDxControl c)
    {
        var before = (Action<TDxControl>)HooksArr(c)[IxBeforeDefaultPaint];
        before?.Invoke(c);
    }

    /// <summary>
    /// 原文 TDxControl.MoveRange（506）。原文在构造里初始化为
    /// `if Owner &lt;&gt; nil then Bounds(0, 0, Owner.Width, Owner.Height) else Rect(0, 0, 800, 600)`
    /// （DxControls.pas 1844-1848）；该字段上游未暴露，故在首次读取时按同一公式求值。
    /// </summary>
    public static TDxRect GetMoveRange(TDxControl c)
    {
        if (HooksArr(c)[IxMoveRange] is TDxRect r) return r;
        var range = c.DxOwner != null
            ? TDxRect.Bounds(0, 0, c.DxOwner.Width, c.DxOwner.Height)
            : TDxRect.Rect(0, 0, 800, 600);
        HooksArr(c)[IxMoveRange] = range;
        return range;
    }

    /// <summary>原文 TDxControl.MoveRange 的显式写入。</summary>
    public static void SetMoveRange(TDxControl c, TDxRect value) => HooksArr(c)[IxMoveRange] = value;

    /// <summary>原文 TDxControl.OnResize（533 / 2370）。</summary>
    public static Action<TDxControl> GetOnResize(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnResize];
    public static void SetOnResize(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnResize] = v;

    /// <summary>原文 TDxControl.OnShow（535）。</summary>
    public static Action<TDxControl> GetOnShow(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnShow];
    public static void SetOnShow(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnShow] = v;

    /// <summary>原文 TDxControl.OnHide（535）。</summary>
    public static Action<TDxControl> GetOnHide(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnHide];
    public static void SetOnHide(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnHide] = v;

    /// <summary>原文 TDxControl.OnCreate（519）。</summary>
    public static Action<TDxControl> GetOnCreate(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnCreate];
    public static void SetOnCreate(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnCreate] = v;

    /// <summary>原文 TDxControl.OnDestroy（520）。</summary>
    public static Action<TDxControl> GetOnDestroy(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnDestroy];
    public static void SetOnDestroy(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnDestroy] = v;

    /// <summary>原文 TDxControl.OnStartPaint（526）。</summary>
    public static Action<TDxControl> GetOnStartPaint(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnStartPaint];
    public static void SetOnStartPaint(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnStartPaint] = v;

    /// <summary>原文 TDxControl.OnStartSubPaint（527）。</summary>
    public static Action<TDxControl> GetOnStartSubPaint(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnStartSubPaint];
    public static void SetOnStartSubPaint(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnStartSubPaint] = v;

    /// <summary>
    /// 原文 3065-3078 TDxControl.DoPaint（原文 `{$IF CLIENTEXE = 1} CheckAutoSize(); {$IFEND}`）。
    /// 上游接缝的 CheckAutoSize 叫 CheckAutoSizeBase 且是 protected，故以槽位承接「是否有前置绘制动作」。
    /// </summary>
    private const int IxBeforeDefaultPaint = 24;

    public static Action<TDxControl> GetBeforeDefaultPaint(TDxControl c)
        => (Action<TDxControl>)HooksArr(c)[IxBeforeDefaultPaint];
    public static void SetBeforeDefaultPaint(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxBeforeDefaultPaint] = v;

    /// <summary>原文 TDxControl.OnStopPaint（529）。</summary>
    public static Action<TDxControl> GetOnStopPaint(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnStopPaint];
    public static void SetOnStopPaint(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnStopPaint] = v;

    /// <summary>原文 TDxControl.OnFocused（530）。</summary>
    public static Action<TDxControl> GetOnFocused(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnFocused];
    public static void SetOnFocused(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnFocused] = v;

    /// <summary>原文 TDxControl.OnUpDate（531）。</summary>
    public static Action<TDxControl> GetOnUpdate(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnUpdate];
    public static void SetOnUpdate(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnUpdate] = v;

    /// <summary>原文 TDxControl.OnMove（504）。</summary>
    public static Action<TDxControl> GetOnMove(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnMove];
    public static void SetOnMove(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnMove] = v;

    /// <summary>原文 TDxControl.OnFindActiveControl（503）。</summary>
    public static TOnFindActiveControl GetOnFindActiveControl(TDxControl c)
        => (TOnFindActiveControl)HooksArr(c)[IxOnFindActiveControl];
    public static void SetOnFindActiveControl(TDxControl c, TOnFindActiveControl v)
        => HooksArr(c)[IxOnFindActiveControl] = v;

    /// <summary>原文 TDxControl.OnKeyDown（495，TKeyEvent）。</summary>
    public static TKeyEvent GetOnKeyDown(TDxControl c) => (TKeyEvent)HooksArr(c)[IxOnKeyDown];
    public static void SetOnKeyDown(TDxControl c, TKeyEvent v) => HooksArr(c)[IxOnKeyDown] = v;

    /// <summary>原文 TDxControl.OnKeyPress（496，TKeyPressEvent）。</summary>
    public static TKeyPressEvent GetOnKeyPress(TDxControl c) => (TKeyPressEvent)HooksArr(c)[IxOnKeyPress];
    public static void SetOnKeyPress(TDxControl c, TKeyPressEvent v) => HooksArr(c)[IxOnKeyPress] = v;

    /// <summary>原文 TDxControl.OnKeyUp（497，TKeyEvent）。</summary>
    public static TKeyEvent GetOnKeyUp(TDxControl c) => (TKeyEvent)HooksArr(c)[IxOnKeyUp];
    public static void SetOnKeyUp(TDxControl c, TKeyEvent v) => HooksArr(c)[IxOnKeyUp] = v;

    /// <summary>原文 TDxControl.OnClick（498，TOnClickEx = procedure(Sender; X,Y)）。</summary>
    public static Action<TDxControl, int, int> GetOnClick(TDxControl c)
        => (Action<TDxControl, int, int>)HooksArr(c)[IxOnClick];
    public static void SetOnClick(TDxControl c, Action<TDxControl, int, int> v) => HooksArr(c)[IxOnClick] = v;

    /// <summary>原文 TDxControl.OnDblClick（499，TOnClickEx）。</summary>
    public static Action<TDxControl, int, int> GetOnDblClick(TDxControl c)
        => (Action<TDxControl, int, int>)HooksArr(c)[IxOnDblClick];
    public static void SetOnDblClick(TDxControl c, Action<TDxControl, int, int> v) => HooksArr(c)[IxOnDblClick] = v;

    /// <summary>原文 TDxControl.OnMouseDown（500，TMouseEvent）。</summary>
    public static TMouseEvent GetOnMouseDown(TDxControl c) => (TMouseEvent)HooksArr(c)[IxOnMouseDown];
    public static void SetOnMouseDown(TDxControl c, TMouseEvent v) => HooksArr(c)[IxOnMouseDown] = v;

    /// <summary>原文 TDxControl.OnMouseMove（501，TMouseMoveEvent）。</summary>
    public static TMouseMoveEvent GetOnMouseMove(TDxControl c) => (TMouseMoveEvent)HooksArr(c)[IxOnMouseMove];
    public static void SetOnMouseMove(TDxControl c, TMouseMoveEvent v) => HooksArr(c)[IxOnMouseMove] = v;

    /// <summary>原文 TDxControl.OnMouseUp（502，TMouseEvent）。</summary>
    public static TMouseEvent GetOnMouseUp(TDxControl c) => (TMouseEvent)HooksArr(c)[IxOnMouseUp];
    public static void SetOnMouseUp(TDxControl c, TMouseEvent v) => HooksArr(c)[IxOnMouseUp] = v;

    /// <summary>
    /// 原文 TDxControl.OnMouseEnter（521）。上游接缝确实把该事件声明成
    /// `public Action<TDxControl> OnMouseEnter` 字段，但 WinForms `Control` 自带
    /// **同名 protected 事件** `OnMouseEnter`，从派生类/外部按名字访问一律绑定到 Control 的
    /// 那个事件（不可访问，CS0122）。故本槽位是托管侧唯一可用的落点 ——
    /// 接缝字段在派生类里实际不可达，已在报告登记。
    /// </summary>
    public static Action<TDxControl> GetOnMouseEnter(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnMouseEnter];
    public static void SetOnMouseEnter(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnMouseEnter] = v;

    /// <summary>原文 TDxControl.OnMouseLeave（522）；同上，接缝字段被 Control 同名 protected 事件遮蔽。</summary>
    public static Action<TDxControl> GetOnMouseLeave(TDxControl c) => (Action<TDxControl>)HooksArr(c)[IxOnMouseLeave];
    public static void SetOnMouseLeave(TDxControl c, Action<TDxControl> v) => HooksArr(c)[IxOnMouseLeave] = v;

    /// <summary>原文 TDxControl.RawText（481，只读；唯一写入者是 SetCaptionA）。</summary>
    public static string GetRawText(TDxControl c) => (string)HooksArr(c)[IxRawText] ?? "";
    public static void SetRawText(TDxControl c, string v) => HooksArr(c)[IxRawText] = v;

    /// <summary>
    /// 原文 FCaption 的**字段直写**（不经 setter）。
    /// 上游接缝的 `Caption` setter 自己会调 DoCaptionChange（这正是原文 SetCaptionA/V 的职责），
    /// 而原文里 `FCaption := Value` 是**裸字段写**、DoCaptionChange 在其后由 SetCaptionA/V 显式调用。
    /// 托管侧若走属性就会把 DoCaptionChange 触发两次（实测），故用私有字段直写保持一次。
    /// </summary>
    private static readonly System.Reflection.FieldInfo CaptionField =
        typeof(TDxControl).GetField("_caption",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new MissingFieldException(typeof(TDxControl).FullName, "_caption");

    public static void SetCaptionRaw(TDxControl c, string value) => CaptionField.SetValue(c, value);

    /// <summary>原文 TDxControl.ModalControl（572）。</summary>
    public static TDxControl GetModalControl(TDxControl c) => (TDxControl)HooksArr(c)[IxModalControl];

    /// <summary>
    /// 原文 3910-3914 TDxControl.DoMouseEnter 里 `OnMouseEnter(Self)` 的落点。
    /// 上游接缝把该事件声明成字段 `public Action<TDxControl> OnMouseEnter`，但它与
    /// WinForms `Control.OnMouseEnter(EventArgs)`（protected）同名 —— 在派生类里按名字访问会
    /// 绑定到 Control 的 protected 方法（不可访问）。故在此以 `TDxControl` 静态类型读取该字段。
    /// </summary>
    public static void DispatchMouseEnter(TDxControl c) => GetOnMouseEnter(c)?.Invoke(c);

    /// <summary>原文 3916-3920 TDxControl.DoMouseLeave 里 `OnMouseLeave(Self)` 的落点（同上）。</summary>
    public static void DispatchMouseLeave(TDxControl c) => GetOnMouseLeave(c)?.Invoke(c);
    public static void SetModalControl(TDxControl c, TDxControl v) => HooksArr(c)[IxModalControl] = v;

    // ---- 原文 Name（TComponentName，DxControls.pas 245 / 542）----
    // WinForms Control.Name **不是 virtual**（已实测），无法覆写；故独立槽位并同步到 Control.Name。
    private static readonly ConditionalWeakTable<TDxControl, string[]> NameTable = new();

    /// <summary>原文 TDxControl.Name（原文构造里 FName 未显式初始化 → ''）。</summary>
    public static string NameOf(TDxControl c) => NameTable.TryGetValue(c, out var s) ? s[0] : "";

    /// <summary>原文 TDxControl.Name 的初始化。</summary>
    public static void InitName(TDxControl c, string name)
    {
        NameTable.GetValue(c, _ => new string[1])[0] = name;
        if (!string.IsNullOrEmpty(name))
            c.Name = name;
    }

    /// <summary>DxControls.pas 2138-2143 TDxControl.SetName（同名不改）。</summary>
    public static void SetName(TDxControl self, string newName)
    {
        var slot = NameTable.GetValue(self, _ => new string[1]);
        if (slot[0] == newName) return;
        slot[0] = newName;
        if (!string.IsNullOrEmpty(newName))
            self.Name = newName;
    }

    // ---- 原文 SpotX / SpotY / FMouseDownX / FMouseDownY（DxControls.pas 224 / 303）----
    private static readonly ConditionalWeakTable<TDxControl, int[]> SpotTable = new();

    internal static int[] Spots(TDxControl c) => SpotTable.GetValue(c, _ => new int[4]);

    /// <summary>原文 SpotX。</summary>
    public static int SpotX(TDxControl c) => SpotTable.TryGetValue(c, out var s) ? s[0] : 0;

    /// <summary>原文 SpotY。</summary>
    public static int SpotY(TDxControl c) => SpotTable.TryGetValue(c, out var s) ? s[1] : 0;

    /// <summary>原文 FMouseDownX。</summary>
    public static int MouseDownX(TDxControl c) => SpotTable.TryGetValue(c, out var s) ? s[2] : 0;

    /// <summary>原文 FMouseDownY（DxControls.pas:4001 把它赋成了 X —— 见 DxControlOps.SetMousePoint）。</summary>
    public static int MouseDownY(TDxControl c) => SpotTable.TryGetValue(c, out var s) ? s[3] : 0;
}

// -------------------------------------------------------------------------------------
// DxControls.pas 655-768 单元级自由函数
// -------------------------------------------------------------------------------------

/// <summary>DxControls.pas 655-768 的单元级自由函数。</summary>
public static class DxControlsUnit
{
    /// <summary>
    /// DxControls.pas 673-694 SetClipboardText（WideString → CF_UNICODETEXT）。
    /// 原文走 GlobalAlloc/GlobalLock/Clipboard.SetAsHandle；托管侧等价物为 Clipboard.SetText。
    /// 原文失败时 RaiseLastOSError 并 GlobalFree —— 托管侧由 Clipboard 自身抛异常表达。
    /// </summary>
    public static void SetClipboardText(string text) => Clipboard.SetText(text ?? "");

    /// <summary>DxControls.pas 696-711 GetClipboardText（无 CF_UNICODETEXT 时返回 ''）。</summary>
    public static string GetClipboardText() => Clipboard.ContainsText() ? Clipboard.GetText() : "";

    /// <summary>
    /// DxControls.pas 713-729 DebugOutStr：追加写 '.\Debug.txt'，行 = `TimeToStr(Time) + ' ' + Msg`。
    /// 原文不处理 IO 异常；托管侧吞掉 IO 失败（纯调试出口）—— 偏差已在报告登记。
    /// </summary>
    public static Action<string> DebugOutStrSink;

    public static void DebugOutStr(string msg)
    {
        if (DebugOutStrSink != null) { DebugOutStrSink(msg); return; }
        try
        {
            System.IO.File.AppendAllText(@".\Debug.txt",
                DateTime.Now.ToString("HH:mm:ss") + " " + msg + Environment.NewLine);
        }
        catch (System.IO.IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    /// <summary>
    /// DxControls.pas 731-748 FindDxComponent：深度优先在 Master.Components 里按名字找
    /// （CompareText = 大小写不敏感）。
    /// </summary>
    public static bool FindDxComponent(TDxControl master, string className)
    {
        if (master == null) return false;
        for (int i = 0; i < DxControlOps.ComponentCount(master); i++)
        {
            var child = DxControlOps.Components(master, i);
            if (string.Equals(DxControlHooks.NameOf(child), className, StringComparison.OrdinalIgnoreCase))
                return true;
            if (FindDxComponent(child, className))
                return true;
        }
        return false;
    }

    /// <summary>
    /// DxControls.pas 750-768 MakeGuiName：类名以 'tdx' 开头（Pos 结果 = 1，大小写不敏感）时去前 3 字符，
    /// 再从 1 开始找第一个不冲突的后缀编号。
    /// </summary>
    public static string MakeGuiName(TDxControl master, string className)
    {
        string result;
        if (Pos(LowerCase(className), "tdx") == 1)
            result = Copy(className, 4, className.Length - 3);
        else
            result = className;

        int num = 1;
        while (FindDxComponent(master, result + num.ToString()))
            num++;
        result = result + num.ToString();

        // 原文 765-767（被注释掉的旧实现，原文如此）：
        //   Num := 1;
        //   while (Master.FindComponent(Result + IntToStr(Num)) <> nil) do Inc(Num);
        //   Result := Result + IntToStr(Num);
        return result;
    }

    /// <summary>Delphi LowerCase。</summary>
    public static string LowerCase(string s) => s == null ? "" : s.ToLowerInvariant();

    /// <summary>Delphi Pos（1-based，找不到返回 0）。</summary>
    public static int Pos(string s, string sub)
    {
        if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(sub)) return 0;
        int i = s.IndexOf(sub, StringComparison.Ordinal);
        return i < 0 ? 0 : i + 1;
    }

    /// <summary>Delphi Copy（1-based 起点；越界返回空串或截断，与原文一致）。</summary>
    public static string Copy(string s, int index1Based, int count)
    {
        if (string.IsNullOrEmpty(s) || count <= 0) return "";
        int start = index1Based - 1;
        if (start < 0 || start >= s.Length) return "";
        if (start + count > s.Length) count = s.Length - start;
        return s.Substring(start, count);
    }
}

/// <summary>
/// HUtil32.ArrestStringEx（DxControls.pas 3052 调用）：取 AStart 与 AEnd 之间（不含两端）的子串。
/// 原文 1-based；找不到起止符时返回 0 并把 Dest 置空。
/// </summary>
public static class DxStringArrest
{
    public static int ArrestStringEx(ref string source, char aStart, char aEnd, out string dest)
    {
        dest = "";
        if (string.IsNullOrEmpty(source)) return 0;
        int s = source.IndexOf(aStart);
        if (s < 0) return 0;
        int e = source.IndexOf(aEnd, s + 1);
        if (e < 0) return 0;
        dest = source.Substring(s + 1, e - s - 1);
        return e + 2;
    }
}

// -------------------------------------------------------------------------------------
// TDxScrollControl（原文 DxControls.pas 1730 的 `is TDxScrollControl` 判定目标；
// 真类在 DxScrollBox / DxPageControl / DxMemo 里，属后续波次）
// -------------------------------------------------------------------------------------

/// <summary>
/// 滚动控件基类接缝。本波只保留「是不是滚动控件」这一判定所需的抽象面：
/// engine 的 MouseWheelDown/Up 与 SetScrollControl 只做 `is TDxScrollControl` 判定后转调这两个虚方法。
/// </summary>
public abstract class TDxScrollControl : TDxControl
{
    /// <summary>原文 TDxControl.MouseWheelDown（子类覆写）。</summary>
    public virtual void MouseWheelDown(TDxShiftState shift, TDxPoint mousePos) { }

    /// <summary>原文 TDxControl.MouseWheelUp（子类覆写）。</summary>
    public virtual void MouseWheelUp(TDxShiftState shift, TDxPoint mousePos) { }
}

// -------------------------------------------------------------------------------------
// DxControlOps：原文 TDxControl 的全部「非几何属性」成员
// -------------------------------------------------------------------------------------

/// <summary>
/// DxControls.pas 中属于 TDxControl、但无法写进 DxComponentCommon.cs（只读独占）的成员。
/// 第一参数即原文的 `Self`；每个方法注明替代的原文成员名与行号。
/// </summary>
public static class DxControlOps
{
    // 原文 FComponents:TList（每个控件一份，懒创建；表空即置 nil）
    private static readonly ConditionalWeakTable<TDxControl, List<TDxControl>> ComponentTable = new();

    private static List<TDxControl> ComponentsRaw(TDxControl c)
        => ComponentTable.TryGetValue(c, out var list) ? list : null;

    private static List<TDxControl> EnsureComponents(TDxControl c)
    {
        if (!ComponentTable.TryGetValue(c, out var list))
        {
            list = new List<TDxControl>();
            ComponentTable.Add(c, list);
        }
        return list;
    }

    /// <summary>
    /// 原文控制树挂接时同步 FRootCtrl
    /// （构造里 `if AOwner is TDxControlEngine then FRootCtrl := Engine else FRootCtrl := AOwner.RootCtrl`，
    /// DxControls.pas 1830-1835）。原文该推导只在 Create 时发生；托管侧在 Insert 时补登记，
    /// 否则「先建控件再挂树」这一常见用法永远拿不到 RootCtrl。
    /// </summary>
    private static void InheritRoot(TDxControl parent, TDxControl child)
    {
        var root = DxRootRegistry.Get(parent);
        if (root != null) DxRootRegistry.Set(child, root);
    }

    /// <summary>原文 TDxControl.RootCtrl（DxControls.pas 509）。</summary>
    public static TDxControlEngine RootCtrlOf(TDxControl self) => DxRootRegistry.Get(self);

    // ---- 原文 314-343 的 Do* 覆写点：上游接缝只保留了 DoCaptionChange / DoResize / DoPaint /
    //      DoOnInRealArea 四个（另有 public virtual Repaint/InRange/MouseDown/MouseMove/MouseUp
    //      与 protected virtual bool CanMove）。其余 Do*（DoShow/DoHide/DoEnable/DoDisable/
    //      DoMouseDown/DoMouseMove/DoMouseUp/DoMouseEnter/DoMouseLeave/DoFocused/DoUnFocused/
    //      DoUpdate/DoMove）在托管侧以这些 *Core 方法承接，基类实现按原文 2774-2830 / 3895-3930 /
    //      4103-4107 逐条移植。----

    /// <summary>原文 2818-2830 TDxControl.DoShow（基类实现）。</summary>
    public static void OnShownCore(TDxControl self)
    {
        SetFocus(self);
        var root = RootCtrlOf(self);
        if (root != null && !ReferenceEquals(root.FocusedControl, self))
        {
            FocusSomething(self);
            SyncIme(self);
        }
        self.Repaint();
    }

    /// <summary>原文 2774-2787 TDxControl.DoHide（基类实现）。</summary>
    public static void OnHiddenCore(TDxControl self)
    {
        ReleaseControl(self);
        if (self.DxOwner is TDxControl owner)
        {
            FocusSomething(owner);
            SyncIme(self);
        }
        self.Repaint();
    }

    /// <summary>原文 2807-2815 TDxControl.DoEnable（基类实现）。</summary>
    public static void OnEnabledCore(TDxControl self)
    {
        FocusSomething(self);
        SyncIme(self);
        self.Repaint();
    }

    /// <summary>原文 2790-2804 TDxControl.DoDisable（基类实现）。</summary>
    public static void OnDisabledCore(TDxControl self)
    {
        ReleaseControl(self);
        if (self.DxOwner is TDxControl owner)
        {
            // 设置控件可见时不要调用父控件的 FocusSomething；2019-09-06 18:11:48
            if (ReferenceEquals(RootCtrlOf(owner)?.FocusedControl, self) && !self.Visible)
                FocusSomething(owner);

            SyncIme(self);
        }
        self.Repaint();
    }

    /// <summary>原文 3895-3898 TDxControl.DoMouseDown（基类实现 = Repaint）。</summary>
    public static void OnMouseDownCore(TDxControl self) => self.Repaint();

    /// <summary>原文 3900-3903 TDxControl.DoMouseMove（基类实现 = Repaint）。</summary>
    public static void OnMouseMoveCore(TDxControl self) => self.Repaint();

    /// <summary>原文 3905-3908 TDxControl.DoMouseUp（基类实现 = Repaint）。</summary>
    public static void OnMouseUpCore(TDxControl self) => self.Repaint();

    /// <summary>原文 3910-3914 TDxControl.DoMouseEnter（转发 OnMouseEnter）。</summary>
    public static void OnMouseEnterCore(TDxControl self) => DxControlHooks.DispatchMouseEnter(self);

    /// <summary>原文 3916-3920 TDxControl.DoMouseLeave（转发 OnMouseLeave）。</summary>
    public static void OnMouseLeaveCore(TDxControl self) => DxControlHooks.DispatchMouseLeave(self);

    /// <summary>原文 3922-3925 TDxControl.DoFocused（空实现）。</summary>
    public static void OnFocusedCore(TDxControl self) { }

    /// <summary>原文 3927-3930 TDxControl.DoUnFocused（空实现）。</summary>
    public static void OnUnfocusedCore(TDxControl self) { }

    /// <summary>原文 4103-4107 TDxControl.DoMove（转发 OnMove）。</summary>
    public static void OnMovedCore(TDxControl self) => DxControlHooks.GetOnMove(self)?.Invoke(self);

    // ---- 原文 2036-2120：Insert / Remove / InserComponent / RemoveComponent / DestroyComponents /
    //                      Notification / FindComponent(名) / FindComponent(ID) ----

    /// <summary>DxControls.pas 2036-2041 TDxControl.Insert（表懒创建，并为子控件回填 Owner）。</summary>
    public static void Insert(TDxControl self, TDxControl aComponent)
    {
        EnsureComponents(self).Add(aComponent);
        aComponent.DxOwner = self;                 // 原文 `AComponent.FOwner := Self`
        InheritRoot(self, aComponent);
    }

    /// <summary>DxControls.pas 2043-2053 TDxControl.Remove：先清 Owner 再摘链；**表空即释放**。</summary>
    public static void Remove(TDxControl self, TDxControl aComponent)
    {
        aComponent.DxOwner = null;
        DxRootRegistry.Set(aComponent, null);
        var list = ComponentsRaw(self);
        if (list != null)
        {
            list.Remove(aComponent);
            if (list.Count == 0)
                ComponentTable.Remove(self);       // 原文 `FComponents.Free; FComponents := nil`
        }
    }

    /// <summary>DxControls.pas 2055-2059 TDxControl.InserComponent（原文如此：Inser，少一个 t）。</summary>
    public static void InserComponent(TDxControl self, TDxControl aComponent)
    {
        Insert(self, aComponent);
        Notification(self, aComponent, TOperation.opInsert);
    }

    /// <summary>DxControls.pas 2061-2065 TDxControl.RemoveComponent（先广播再摘链）。</summary>
    public static void RemoveComponent(TDxControl self, TDxControl aComponent)
    {
        Notification(self, aComponent, TOperation.opRemove);
        Remove(self, aComponent);
    }

    /// <summary>
    /// DxControls.pas 2067-2077 TDxControl.DestroyComponents：
    /// **while FComponents &lt;&gt; nil** 反复取 Last 摘除、递归销毁、再 Destroy（顺序不可换）。
    /// </summary>
    public static void DestroyComponents(TDxControl self)
    {
        while (ComponentsRaw(self) != null)
        {
            var list = ComponentsRaw(self);
            var instance = list[list.Count - 1];
            RemoveComponent(self, instance);
            DestroyComponents(instance);
            instance.Dispose();                    // 原文 Instance.Destroy
        }
    }

    /// <summary>DxControls.pas 2079-2089 TDxControl.Notification（递归向子控件广播；基类无副作用）。</summary>
    public static void Notification(TDxControl self, TDxControl aComponent, TOperation operation)
    {
        var list = ComponentsRaw(self);
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
            Notification(list[i], aComponent, operation);
    }

    /// <summary>
    /// DxControls.pas 2091-2101 TDxControl.FindComponent(AName)：**只找直接子**，SameText。
    /// 原文循环里 `Result := FComponents[I]` 会把**最后一个不匹配的**留在 Result，
    /// 但末尾 `Result := nil`（2100）会清掉，故等价于「命中即返回，否则 nil」。
    /// </summary>
    public static TDxControl FindComponent(TDxControl self, string aName)
    {
        var list = ComponentsRaw(self);
        if (!string.IsNullOrEmpty(aName) && list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(DxControlHooks.NameOf(list[i]), aName, StringComparison.OrdinalIgnoreCase))
                    return list[i];
            }
        }
        return null;
    }

    /// <summary>
    /// DxControls.pas 2103-2120 TDxControl.FindComponent(ID:Integer)：深度优先、命中即返回；
    /// 未命中返回 nil（原文末尾的 `Result := nil` 逐字保留）。
    /// </summary>
    public static TDxControl FindComponent(TDxControl self, int id)
    {
        var list = ComponentsRaw(self);
        if (id != 0 && list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];
                if (d.ControlID == id) return d;
                var r = FindComponent(d, id);
                if (r != null) return r;
            }
        }
        return null;
    }

    // ---- 原文 2145-2186：GeComponentIndex / GeComponent / GeComponentCount / SeComponentIndex ----

    /// <summary>DxControls.pas 2145-2151 GeComponentIndex（原文成员名如此：Ge）。</summary>
    public static int GetComponentIndex(TDxControl self)
    {
        if (self.DxOwner != null && ComponentsRaw(self.DxOwner) != null)
            return ComponentsRaw(self.DxOwner).IndexOf(self);
        return -1;
    }

    /// <summary>DxControls.pas 2153-2160 GeComponent（表为 nil 时返回 nil，HZQ 20230525）。</summary>
    public static TDxControl Components(TDxControl self, int index)
    {
        var list = ComponentsRaw(self);
        return list != null ? list[index] : null;
    }

    /// <summary>DxControls.pas 2162-2168 GeComponentCount。</summary>
    public static int ComponentCount(TDxControl self)
    {
        var list = ComponentsRaw(self);
        return list != null ? list.Count : 0;
    }

    /// <summary>DxControls.pas 2170-2186 SeComponentIndex：Value 先夹到 [0, Count-1]，与当前位置相同则不动。</summary>
    public static void SetComponentIndex(TDxControl self, int value)
    {
        if (self.DxOwner == null) return;
        var list = ComponentsRaw(self.DxOwner);
        if (list == null) return;                  // 原文未判 nil（AV 前提）；托管侧防护
        int i = list.IndexOf(self);
        if (i < 0) return;
        int count = list.Count;
        if (value < 0) value = 0;
        if (value >= count) value = count - 1;
        if (value != i)
        {
            list.RemoveAt(i);
            list.Insert(value, self);
        }
    }

    // ---- 原文 2189-2216：Initialize / Finalize / GetControl / GetControlCount ----

    /// <summary>DxControls.pas 2189-2195 TDxControl.Initialize（前序：先全部子控件）。
    /// 上游接缝的 TDxControl 未提供 Initialize（WinForms Control.Initialize 也不是可覆写的虚方法），
    /// 故子控件的 Initialize 由各控件自身在需要时实现 —— 本方法只做递归下钻。</summary>
    public static void Initialize(TDxControl self)
    {
        for (int i = 0; i < ComponentCount(self); i++)
            Initialize(Components(self, i));
    }

    /// <summary>DxControls.pas 2197-2203 TDxControl.Finalize（原文成员名 Finalize）。</summary>
    public static void FinalizeControls(TDxControl self)
    {
        for (int i = 0; i < ComponentCount(self); i++)
            FinalizeControls(Components(self, i));
    }

    /// <summary>DxControls.pas 2205-2211 GetControl：越界返回 nil。</summary>
    public static TDxControl GetControl(TDxControl self, int index)
        => index >= 0 && index < ComponentCount(self) ? Components(self, index) : null;

    /// <summary>DxControls.pas 2213-2216 GetControlCount。</summary>
    public static int GetControlCount(TDxControl self) => ComponentCount(self);

    // ---- 原文 2265-2290：FocusSomething ----

    /// <summary>OpenIme / CloseIme 的接缝（原文为 imm.pas 的全局过程）。</summary>
    public static Action<TDxControl> OpenImeSink;
    public static Action<TDxControl> CloseImeSink;

    private static void OpenIme(TDxControl c) => OpenImeSink?.Invoke(c);
    private static void CloseIme(TDxControl c) => CloseImeSink?.Invoke(c);

    /// <summary>
    /// 原文 `Control[I] is TDxEdit / TDxImageEdit` 的判定（两个类属后续波次）。
    /// 缺省按类型名包含 "Edit"/"Memo" 判定 —— 该启发式可被 MatchesTextInput 覆盖。
    /// </summary>
    public static Func<TDxControl, bool> MatchesTextInput =
        c => c != null && (c.GetType().Name.Contains("Edit") || c.GetType().Name.Contains("Memo"));

    /// <summary>
    /// DxControls.pas 2265-2290 TDxControl.FocusSomething：
    /// ① 门控 `Visible and Enabled and EnableMouse`；
    /// ② 第一轮只认 TDxEdit / TDxImageEdit；
    /// ③ 第二轮认任意 可见/可用/EnableFocus 的子控件；
    /// ④ 第三轮递归下钻（原文 2283-2288 **未判** 子控件的 Visible/Enabled，直接递归）。
    /// </summary>
    public static void FocusSomething(TDxControl self)
    {
        if (!(self.Visible && self.Enabled && self.EnableMouse)) return;

        int count = ComponentCount(self);
        var root = RootCtrlOf(self);

        for (int i = 0; i < count; i++)
        {
            var c = Components(self, i);
            if (MatchesTextInput(c) && c.Visible && c.Enabled && c.EnableFocus)
            {
                SetFocus(c);
                if (root?.FocusedControl != null) break;
            }
        }

        if (root?.FocusedControl == null)
        {
            for (int i = 0; i < count; i++)
            {
                var c = Components(self, i);
                if (c.Visible && c.Enabled && c.EnableFocus)
                {
                    SetFocus(c);
                    if (root?.FocusedControl != null) break;
                }
            }
        }

        if (root?.FocusedControl == null)
        {
            for (int i = 0; i < count; i++)
            {
                FocusSomething(Components(self, i));
                if (root?.FocusedControl != null) return;
            }
        }
    }

    /// <summary>
    /// 原文 2781 / 2798 / 2810 / 2824 的公共尾巴：
    /// `if (FocusedCtrl is TDxEdit/TDxImageEdit) and FocusedCtrl.Visible and FocusedCtrl.Enabled
    ///     and FocusedCtrl.EnableFocus then OpenIme else CloseIme`。
    /// FocusedCtrl 属性 = `RootCtrl.FocusedControl`（DxControls.pas 2834-2837）。
    /// </summary>
    private static void SyncIme(TDxControl self)
    {
        var fc = RootCtrlOf(self)?.FocusedControl;
        bool open = fc != null && MatchesTextInput(fc) && fc.Visible && fc.Enabled && fc.EnableFocus;
        if (open) OpenIme(self); else CloseIme(self);
    }

    // ---- 原文 2293-2358：ToFront / ToBack / BringToFront / SentToBack ----

    /// <summary>
    /// DxControls.pas 2293-2314 TDxControl.ToFront(Index)：取 **{$ELSE} 非多线程分支**
    /// （直接 `Aux.ComponentIndex := 0`）。原文 IsMultiThreadRender=1 时改为 `RootCtrl.AddBringToFront(Aux)`
    /// 入队、由 TDxControlEngine.ExecutePosition 消化 —— 该路径同样已移植，只是默认走非多线程分支，
    /// 与 P1 车道5 的移植选择一致。
    /// </summary>
    public static void ToFront(TDxControl self, int index)
    {
        if (RootCtrlOf(self) == null) return;
        if (index >= 0 && index < ComponentCount(self))
        {
            var aux = Components(self, index);
            if (aux != null && aux.DxOwner != null && GetComponentIndex(aux) != 0)
            {
                // 原文两个分支（Aux.Owner = RootCtrl / else）此处动作相同，保留结构以便对照
                if (ReferenceEquals(aux.DxOwner, RootCtrlOf(self)))
                    SetComponentIndex(aux, 0);
                else
                    SetComponentIndex(aux, 0);
                self.Repaint();
            }
        }
    }

    /// <summary>
    /// DxControls.pas 2318-2339 TDxControl.ToBack(Index)：两分支目标索引**不同** ——
    /// RootCtrl 直属时用 `Aux.Owner.ComponentCount - 1`，否则用 `self.ComponentCount - 1`。逐字照抄。
    /// </summary>
    public static void ToBack(TDxControl self, int index)
    {
        if (RootCtrlOf(self) == null) return;
        if (index >= 0 && index < ComponentCount(self))
        {
            var aux = Components(self, index);
            if (aux != null && aux.DxOwner != null && GetComponentIndex(aux) != ComponentCount(self) - 1)
            {
                if (ReferenceEquals(aux.DxOwner, RootCtrlOf(self)))
                    SetComponentIndex(aux, ComponentCount(aux.DxOwner) - 1);
                else
                    SetComponentIndex(aux, ComponentCount(self) - 1);
                self.Repaint();
            }
        }
    }

    /// <summary>DxControls.pas 2342-2348 TDxControl.BringToFront。</summary>
    public static void BringToFront(TDxControl self)
    {
        if (self.DxOwner is TDxControl owner)
        {
            ToFront(owner, GetComponentIndex(self));
            self.Repaint();
        }
    }

    /// <summary>DxControls.pas 2352-2358 TDxControl.SentToBack（原文成员名如此：Sent）。</summary>
    public static void SentToBack(TDxControl self)
    {
        if (self.DxOwner is TDxControl owner)
        {
            ToBack(owner, GetComponentIndex(self));
            self.Repaint();
        }
    }

    // ---- 原文 2362-2371：SetClientRect ----

    /// <summary>
    /// DxControls.pas 2362-2371 TDxControl.SetClientRect：
    /// `NewRect := Value; DoResize(NewRect); FClientRect := NewRect; Repaint; if OnResize then OnResize(Self)`。
    /// 托管侧：DoResize 走 DxProtected，落值走公有 ClientRect setter（它内部还会再跑一次 DoResize ——
    /// 对 alNone 无影响，对其他对齐是幂等重算）。
    /// </summary>
    public static void SetClientRect(TDxControl self, TDxRect value)
    {
        var newRect = value;
        DxProtected.CallDoResize(self, ref newRect);
        self.ClientRect = newRect;
        self.Repaint();
        DxControlHooks.GetOnResize(self)?.Invoke(self);
    }

    // ---- 原文 2415-2470：SetVisible / Show / Hide / Close / SetEnabled / SetOwner ----

    /// <summary>
    /// DxControls.pas 2415-2429 TDxControl.SetVisible：
    /// 仅当值变化才触发；显示时 `DoShow` 先于 OnShow，隐藏时 `OnHide` 先于 `DoHide`（顺序不可换）。
    /// </summary>
    public static void SetVisible(TDxControl self, bool value)
    {
        if (self.Visible == value) return;
        self.Visible = value;
        if (value)
        {
            OnShownCore(self);
            DxControlHooks.GetOnShow(self)?.Invoke(self);
        }
        else
        {
            DxControlHooks.GetOnHide(self)?.Invoke(self);
            OnHiddenCore(self);
        }
    }

    /// <summary>DxControls.pas 2433-2436 Show。</summary>
    public static void Show(TDxControl self) => SetVisible(self, true);

    /// <summary>DxControls.pas 2440-2443 Hide。</summary>
    public static void Hide(TDxControl self) => SetVisible(self, false);

    /// <summary>DxControls.pas 2446-2449 Close（与 Hide 完全等价）。</summary>
    public static void Close(TDxControl self) => SetVisible(self, false);

    /// <summary>DxControls.pas 2453-2462 SetEnabled。</summary>
    public static void SetEnabled(TDxControl self, bool value)
    {
        if (self.Enabled == value) return;
        self.Enabled = value;
        if (value) OnEnabledCore(self);
        else OnDisabledCore(self);
    }

    /// <summary>
    /// DxControls.pas 2464-2470 TDxControl.SetOwner：Value 非 nil 且不同才摘链换爹。
    /// 原文 `FOwner.Remove(Self)` 未判 nil（隐含前提：已被 Insert 过），托管侧加 nil 守卫。
    /// </summary>
    public static void SetOwner(TDxControl self, TDxControl value)
    {
        if (value != null && !ReferenceEquals(value, self.DxOwner))
        {
            if (self.DxOwner != null) Remove(self.DxOwner, self);
            Insert(value, self);
        }
    }

    // ---- 原文 2474-2536：MoveBy / ResizeBy / ApplyConstraint / Assign ----

    /// <summary>DxControls.pas 2474-2478 TDxControl.MoveBy。</summary>
    public static void MoveBy(TDxControl self, int dx, int dy)
    {
        self.Left = self.ClientRect.Left + dx;
        self.Top = self.ClientRect.Top + dy;
    }

    /// <summary>DxControls.pas 2482-2486 TDxControl.ResizeBy。</summary>
    public static void ResizeBy(TDxControl self, int dx, int dy)
    {
        self.Width = self.Width + dx;
        self.Height = self.Height + dy;
    }

    /// <summary>DxControls.pas 2490-2493 TDxControl.ApplyConstraint（交集，不判空）。</summary>
    public static void ApplyConstraint(TDxControl self, TDxRect constraint)
        => self.ClientRect = DxRectUtil.ShortRect(self.ClientRect, constraint);

    /// <summary>
    /// DxControls.pas 2495-2536 TDxControl.Assign(Source:TDxControl)。
    /// 原文还拷贝 OnCreate/OnDestroy/OnStartPaint/OnStopPaint，落在 DxControlHooks 槽位上。
    /// </summary>
    public static void AssignFrom(TDxControl self, TDxControl source)
    {
        self.Left = source.Left;
        self.Top = source.Top;
        self.Width = source.Width;
        self.Height = source.Height;
        self.Visible = source.Visible;
        self.Enabled = source.Enabled;
        self.EnableMouse = source.EnableMouse;

        DxControlHooks.SetOnGetImage(self, DxControlHooks.GetOnGetImage(source));        DxControlHooks.SetOnKeyDown(self, DxControlHooks.GetOnKeyDown(source));
        DxControlHooks.SetOnKeyPress(self, DxControlHooks.GetOnKeyPress(source));
        DxControlHooks.SetOnKeyUp(self, DxControlHooks.GetOnKeyUp(source));
        DxControlHooks.SetOnClick(self, DxControlHooks.GetOnClick(source));
        DxControlHooks.SetOnDblClick(self, DxControlHooks.GetOnDblClick(source));
        DxControlHooks.SetOnMouseDown(self, DxControlHooks.GetOnMouseDown(source));
        DxControlHooks.SetOnMouseMove(self, DxControlHooks.GetOnMouseMove(source));
        DxControlHooks.SetOnMouseUp(self, DxControlHooks.GetOnMouseUp(source));

        DxControlHooks.SetOnCreate(self, DxControlHooks.GetOnCreate(source));
        DxControlHooks.SetOnDestroy(self, DxControlHooks.GetOnDestroy(source));
        DxControlHooks.SetOnFocused(self, DxControlHooks.GetOnFocused(source));
        DxControlHooks.SetOnUpdate(self, DxControlHooks.GetOnUpdate(source));
        DxControlHooks.SetOnMove(self, DxControlHooks.GetOnMove(source));

        self.OnInRealArea = source.OnInRealArea;
        self.OnPaint = source.OnPaint;
        DxControlHooks.SetOnStartPaint(self, DxControlHooks.GetOnStartPaint(source));
        DxControlHooks.SetOnStartSubPaint(self, DxControlHooks.GetOnStartSubPaint(source));
        DxControlHooks.SetOnStopPaint(self, DxControlHooks.GetOnStopPaint(source));

        DxControlHooks.SetOnResize(self, DxControlHooks.GetOnResize(source));
        DxControlHooks.SetOnFindActiveControl(self, DxControlHooks.GetOnFindActiveControl(source));

        self.MouseEvents = source.MouseEvents;
        self.ImageIndex.Assign(source.ImageIndex);
        self.BorderColor.Assign(source.BorderColor);
        self.Transparent = source.Transparent;

        self.Designing = source.Designing;
        self.EnableFocus = source.EnableFocus;
        self.Floating = source.Floating;
        self.OwnerMove = source.OwnerMove;
    }

    // ---- 原文 2539-2569：ImageIndexChange ----

    /// <summary>
    /// DxControls.pas 2539-2569 TDxControl.ImageIndexChange：
    /// Up→Hot→Down→Checked→Disabled 取**第一个 >= 0** 的索引；纹理存在且 `Width*Height > 4` 才按图定尺
    /// （注意与 CheckAutoSize 的 `>= 4` 判定不同）；Center 且非设计期再居中（div 2 向零截断）。
    /// </summary>
    public static void ImageIndexChange(TDxControl self)
    {
        if (!self.AutoSize || self.ImageIndex.Image == null) return;

        int nIndex = -1;
        if (self.ImageIndex.Up >= 0) nIndex = self.ImageIndex.Up;
        else if (self.ImageIndex.Hot >= 0) nIndex = self.ImageIndex.Hot;
        else if (self.ImageIndex.Down >= 0) nIndex = self.ImageIndex.Down;
        else if (self.ImageIndex.Checked >= 0) nIndex = self.ImageIndex.Checked;
        else if (self.ImageIndex.Disabled >= 0) nIndex = self.ImageIndex.Disabled;

        if (nIndex >= 0)
        {
            var texture = self.ImageIndex.Image.GetImage(nIndex);
            if (texture != null && texture.Width * texture.Height > 4)
            {
                self.Width = texture.Width;
                self.Height = texture.Height;
                if (self.Center && self.DxOwner != null && !self.Designing)
                {
                    self.Left = (self.DxOwner.Width - self.Width) / 2;
                    self.Top = (self.DxOwner.Height - self.Height) / 2;
                }
            }
        }
    }

    // ---- 原文 2573-2591：FindControl / GetCtrl ----

    /// <summary>
    /// DxControls.pas 2573-2583 FindControl：先比 `LowerCase(Self.Name)`（**原文只 LowerCase 了 Self.Name**；
    /// AName 由 GetCtrl 预先转小写），否则落到 FindComponent（直接子，SameText）。
    /// </summary>
    public static TDxControl FindControl(TDxControl self, string aName)
    {
        if (DxControlsUnit.LowerCase(DxControlHooks.NameOf(self)) == aName)
            return self;
        return FindComponent(self, aName);
    }

    /// <summary>DxControls.pas 2587-2591 GetCtrl：AName 先转小写。</summary>
    public static TDxControl GetCtrl(TDxControl self, string aName)
        => FindControl(self, DxControlsUnit.LowerCase(aName));

    // ---- 原文 2611-2622：GetRootCtrl ----

    /// <summary>
    /// DxControls.pas 2611-2622（HZQ 20230520 NEW FUNCTION）：
    /// 沿 Owner 上溯；Owner 是 TDxControlEngine 即返回，否则递归。
    /// 原文 2594-2607 的旧实现（可能不返回值）以注释照抄留档：
    ///   function TDxControl.GetRootCtrl(): TDxControlEngine;
    ///   begin
    ///     if ((Owner &lt;&gt; nil) and (Owner is TDxControlEngine)) then begin
    ///       Result := TDxControlEngine(Owner); Exit;
    ///     end;
    ///     while (Owner &lt;&gt; nil) and (Owner is TDxControl) do
    ///       Result := TDxControl(Owner).GetRootCtrl;
    ///   end;
    /// </summary>
    public static TDxControlEngine GetRootCtrl(TDxControl self)
    {
        // 引擎自身就是根：`FRootCtrl := Self`（DxControls.pas 1142）→ RootCtrl 返回自己
        if (self is TDxControlEngine selfEngine) return selfEngine;

        var owner = self.DxOwner;
        while (owner != null)
        {
            if (owner is TDxControlEngine engine) return engine;
            return GetRootCtrl(owner);
        }
        return null;
    }

    // ---- 原文 2625-2757：SetAlign / DoResize / SetAutoSize / SetCaptionA(V) / SetXxx 属性 ----

    /// <summary>
    /// DxControls.pas 2625-2662 TDxControl.SetAlign：
    /// 值变化才落位；六个分支各自读写 Width/Height/Top/Left（**读写顺序原文如此**，会相互影响 ——
    /// 例如 alTop 先 `Width := Owner.Width` 再 `Top := 0` 再 `Left := 0`）。
    /// </summary>
    public static void SetAlign(TDxControl self, TDxAlign value)
    {
        if (self.Align == value) return;
        self.Align = value;
        var owner = self.DxOwner;
        if (owner == null) return;
        switch (value)
        {
            case TDxAlign.alNone:
                break;
            case TDxAlign.alTop:
                self.Width = owner.Width;
                self.Top = 0;
                self.Left = 0;
                break;
            case TDxAlign.alBottom:
                self.Width = owner.Width;
                self.Top = owner.Height - self.Height;
                self.Left = 0;
                break;
            case TDxAlign.alLeft:
                self.Height = owner.Height;
                self.Top = 0;
                self.Left = 0;
                break;
            case TDxAlign.alRight:
                self.Height = owner.Height;
                self.Top = 0;
                self.Left = owner.Width - self.Width;
                break;
            case TDxAlign.alClient:
                self.Height = owner.Height;
                self.Width = owner.Width;
                self.Top = 0;
                self.Left = 0;
                break;
            case TDxAlign.alCustom:
                break;
        }
    }

    /// <summary>
    /// DxControls.pas 2718-2757 TDxControl.DoResize（用户显式入口）：
    /// 与 SetAlign 有别 —— alTop/alBottom 用 **NewRect.Left** 起算、且用**旧 Width/Height**
    /// （不是 NewRect 的），最后**无条件 Repaint**。原文如此，逐字照抄。
    /// </summary>
    public static void DoResize(TDxControl self, ref TDxRect newRect)
    {
        var owner = self.DxOwner;
        if (owner != null)
        {
            switch (self.Align)
            {
                case TDxAlign.alNone:
                    break;
                case TDxAlign.alTop:
                    newRect.Left = 0;
                    newRect.Top = 0;
                    newRect.Right = newRect.Left + owner.Width;
                    newRect.Bottom = newRect.Top + self.Height;
                    break;
                case TDxAlign.alBottom:
                    newRect.Left = 0;
                    newRect.Top = owner.Height - self.Height;
                    newRect.Right = newRect.Left + owner.Width;
                    newRect.Bottom = newRect.Top + self.Height;
                    break;
                case TDxAlign.alLeft:
                    newRect.Left = 0;
                    newRect.Top = 0;
                    newRect.Right = newRect.Left + self.Width;
                    newRect.Bottom = newRect.Top + owner.Height;
                    break;
                case TDxAlign.alRight:
                    newRect.Left = owner.Width - self.Width;
                    newRect.Top = 0;
                    newRect.Right = newRect.Left + self.Width;
                    newRect.Bottom = newRect.Top + owner.Height;
                    break;
                case TDxAlign.alClient:
                    newRect.Left = 0;
                    newRect.Top = 0;
                    newRect.Right = newRect.Left + owner.Width;
                    newRect.Bottom = newRect.Top + owner.Height;
                    break;
                case TDxAlign.alCustom:
                    break;
            }
        }
        self.Repaint();
    }

    /// <summary>DxControls.pas 2665-2672 TDxControl.SetAutoSize。</summary>
    public static void SetAutoSize(TDxControl self, bool value)
    {
        if (self.AutoSize == value) return;
        self.AutoSize = value;
        ImageIndexChange(self);
        DxProtected.DoCaptionChange(self);
    }

    /// <summary>DxControls.pas 2676-2682 TDxControl.SetCaptionV：**不动 FRawText**（故 FormatCaption 可反复跑）。</summary>
    public static void SetCaptionV(TDxControl self, string value)
    {
        if (self.Caption == value) return;
        DxControlHooks.SetCaptionRaw(self, value);   // 原文 `FCaption := Value`（裸字段写）
        DxProtected.DoCaptionChange(self);
    }

    /// <summary>DxControls.pas 2684-2691 TDxControl.SetCaptionA：**同时写 FRawText**。</summary>
    public static void SetCaptionA(TDxControl self, string value)
    {
        if (self.Caption == value) return;
        DxControlHooks.SetCaptionRaw(self, value);   // 原文 `FCaption := Value`
        DxControlHooks.SetRawText(self, value);
        DxProtected.DoCaptionChange(self);
    }

    /// <summary>DxControls.pas 2693-2698 TDxControl.SetShowNameA（同名不改）。</summary>
    public static void SetShowNameA(TDxControl self, string value)
    {
        if (self.ShowName != value)
            self.ShowName = value;
    }

    /// <summary>DxControls.pas 2700-2704 SetReferenceX。</summary>
    public static void SetReferenceX(TDxControl self, TReferenceX value)
    {
        if (self.ReferenceX != value)
            self.ReferenceX = value;
    }

    /// <summary>DxControls.pas 2706-2710 SetAdjustYByHeight。</summary>
    public static void SetAdjustYByHeight(TDxControl self, bool value)
    {
        if (self.AdjustYByHeight != value)
            self.AdjustYByHeight = value;
    }

    /// <summary>DxControls.pas 2712-2716 SetTopAlignment。</summary>
    public static void SetTopAlignment(TDxControl self, bool value)
    {
        if (self.TopAlignment != value)
            self.TopAlignment = value;
    }

    // ---- 原文 2774-2830：DoHide / DoDisable / DoEnable / DoShow ----

    /// <summary>DxControls.pas 2774-2787 TDxControl.DoHide。</summary>
    public static void DoHide(TDxControl self)
    {
        ReleaseControl(self);
        if (self.DxOwner is TDxControl owner)
        {
            FocusSomething(owner);
            SyncIme(self);
        }
        self.Repaint();
    }

    /// <summary>DxControls.pas 2790-2804 TDxControl.DoDisable。</summary>
    public static void DoDisable(TDxControl self)
    {
        ReleaseControl(self);
        if (self.DxOwner is TDxControl owner)
        {
            // 设置控件可见时不要调用父控件的 FocusSomething；2019-09-06 18:11:48
            if (ReferenceEquals(RootCtrlOf(owner)?.FocusedControl, self) && !self.Visible)
                FocusSomething(owner);

            SyncIme(self);
        }
        self.Repaint();
    }

    /// <summary>DxControls.pas 2807-2815 TDxControl.DoEnable。</summary>
    public static void DoEnable(TDxControl self)
    {
        FocusSomething(self);
        SyncIme(self);
        self.Repaint();
    }

    /// <summary>DxControls.pas 2818-2830 TDxControl.DoShow。</summary>
    public static void DoShow(TDxControl self)
    {
        SetFocus(self);
        var root = RootCtrlOf(self);
        if (root != null && !ReferenceEquals(root.FocusedControl, self))
        {
            FocusSomething(self);
            SyncIme(self);
        }
        self.Repaint();
    }

    // ---- 原文 2841-2893：MouseMoveed / MouseDowned / Focused ----

    /// <summary>
    /// DxControls.pas 2841-2862 TDxControl.SetMouseMoveed：
    /// True → 旧 MouseMoveControl 收 Leave；挂上 Self；若控制权换了控件则发 Enter；最后 DoMouseMove。
    /// False → 当前 MouseMoveControl 收 Leave 后置 nil。
    /// </summary>
    public static void SetMouseMoveed(TDxControl self, bool value)
    {
        var root = RootCtrlOf(self);
        if (root == null) return;                  // 原文此处会 AV；托管侧守卫
        if (value)
        {
            if (root.MouseMoveControl != null && !ReferenceEquals(root.MouseMoveControl, self))
                OnMouseLeaveCore(root.MouseMoveControl);

            var previous = root.MouseMoveControl;
            root.MouseMoveControl = self;

            if (!ReferenceEquals(previous, self))
                OnMouseEnterCore(self);

            OnMouseMoveCore(self);
        }
        else
        {
            if (root.MouseMoveControl != null)
                OnMouseLeaveCore(root.MouseMoveControl);
            root.MouseMoveControl = null;
        }
    }

    /// <summary>
    /// DxControls.pas 2864-2878 TDxControl.SetMouseDowned：
    /// True → 旧 MouseDownControl 收 DoMouseUp；挂上 Self；DoMouseDown。
    /// </summary>
    public static void SetMouseDowned(TDxControl self, bool value)
    {
        var root = RootCtrlOf(self);
        if (root == null) return;
        if (value)
        {
            if (root.MouseDownControl != null && !ReferenceEquals(root.MouseDownControl, self))
                OnMouseUpCore(root.MouseDownControl);

            root.MouseDownControl = self;
            OnMouseDownCore(self);
        }
        else
        {
            if (root.MouseDownControl != null)
                OnMouseUpCore(root.MouseDownControl);
            root.MouseDownControl = null;
        }
    }

    /// <summary>DxControls.pas 2880-2883 GetMouseDowned（与 root 的当前值比较）。</summary>
    public static bool GetMouseDowned(TDxControl self) => ReferenceEquals(self, RootCtrlOf(self)?.MouseDownControl);

    /// <summary>DxControls.pas 2885-2888 GetMouseMoveed。</summary>
    public static bool GetMouseMoveed(TDxControl self) => ReferenceEquals(self, RootCtrlOf(self)?.MouseMoveControl);

    /// <summary>DxControls.pas 2890-2893 GetFocused。</summary>
    public static bool GetFocused(TDxControl self) => ReferenceEquals(self, RootCtrlOf(self)?.FocusedControl);

    // ---- 原文 2897-2928：SetCenterA / SetCenter / WidthCenter / HeightCenter ----

    /// <summary>DxControls.pas 2897-2906 SetCenterA（置 True 且非设计期且有 Owner 才居中）。</summary>
    public static void SetCenterA(TDxControl self, bool value)
    {
        if (self.Center == value) return;
        self.Center = value;
        if (value && self.DxOwner != null && !self.Designing)
        {
            self.Left = (self.DxOwner.Width - self.Width) / 2;
            self.Top = (self.DxOwner.Height - self.Height) / 2;
        }
    }

    /// <summary>DxControls.pas 2908-2914 SetCenter()（无条件居中，不判 Designing）。</summary>
    public static void SetCenter(TDxControl self)
    {
        if (self.DxOwner != null)
        {
            self.Left = (self.DxOwner.Width - self.Width) / 2;
            self.Top = (self.DxOwner.Height - self.Height) / 2;
        }
    }

    /// <summary>DxControls.pas 2916-2921 WidthCenter。</summary>
    public static void WidthCenter(TDxControl self)
    {
        if (self.DxOwner != null)
            self.Left = (self.DxOwner.Width - self.Width) / 2;
    }

    /// <summary>DxControls.pas 2923-2928 HeightCenter。</summary>
    public static void HeightCenter(TDxControl self)
    {
        if (self.DxOwner != null)
            self.Top = (self.DxOwner.Height - self.Height) / 2;
    }

    // ---- 原文 2930-3035：SetFocus / SetCapture / SetScrollControl / ReleaseControl / SetDesigning ----

    /// <summary>
    /// DxControls.pas 2930-2967 TDxControl.SetFocus：
    /// ① 沿 Owner 链上溯，任一环 `not (Visible and (Enabled or (not Enabled and Designing)))` → 不可聚焦；
    /// ② 上溯到 `Owner = RootCtrl` 时：若 Self 是 TDxImageForm 且可见可用 → BringToFront，然后 break；
    /// ③ 可聚焦且 root 有子控件：`Control[0]` 满足 可见/可用/CanDraw 且是 TDxScrollControl →
    ///    直接设 root.ScrollControl，否则调它的 SetScrollControl；
    /// ④ 自身 可见 + 可用 + EnableFocus → `root.FocusedControl := Self` 并触发 OnFocused。
    /// </summary>
    public static void SetFocus(TDxControl self)
    {
        var root = RootCtrlOf(self);
        bool boCanFocus = true;
        var d = self;
        while (true)
        {
            if (!(d.Visible && (d.Enabled || (!d.Enabled && d.Designing))))
            {
                boCanFocus = false;
                break;
            }

            if (ReferenceEquals(d.DxOwner, root))
            {
                if (d is TDxImageForm && d.Visible && (d.Enabled || (!d.Enabled && d.Designing)))
                    BringToFront(d);
                break;
            }
            d = d.DxOwner;
            if (d == null) break;                  // 托管侧防护：原文隐式依赖 Owner 链终到 RootCtrl
        }

        if (boCanFocus && root != null && ComponentCount(root) > 0)
        {
            var c0 = Components(root, 0);
            if (c0.Visible &&
                (c0.Enabled || (!c0.Enabled && c0.Designing)) &&
                c0.CanDraw() &&
                c0 is TDxScrollControl)
                root.ScrollControl = c0;
            else
                SetScrollControl(c0);
        }

        if (boCanFocus && self.Visible && (self.Enabled || (!self.Enabled && self.Designing)) && self.EnableFocus)
        {
            if (root != null)
            {
                root.FocusedControl = self;
                DxControlHooks.GetOnFocused(self)?.Invoke(self);
            }
        }
    }

    /// <summary>DxControls.pas 2969-2974 TDxControl.SetCapture。</summary>
    public static void SetCapture(TDxControl self)
    {
        if (self.Visible && (self.Enabled || self.Designing))
        {
            var root = RootCtrlOf(self);
            if (root != null) root.MouseDownControl = self;
        }
    }

    /// <summary>DxControls.pas 2976-2990 TDxControl.SetScrollControl（递归下钻找第一个 TDxScrollControl）。</summary>
    public static void SetScrollControl(TDxControl self)
    {
        if (self.Visible && (self.Enabled || (!self.Enabled && self.Designing)) && self.CanDraw())
        {
            var root = RootCtrlOf(self);
            if (self is TDxScrollControl)
            {
                if (root != null) root.ScrollControl = self;
            }
            else
            {
                for (int i = 0; i < ComponentCount(self); i++)
                {
                    var c = Components(self, i);
                    SetScrollControl(c);
                    if (ReferenceEquals(root?.ScrollControl, c)) break;
                }
            }
        }
    }

    /// <summary>DxControls.pas 2992-3023 TDxControl.ReleaseControl（后序递归到全部子控件）。</summary>
    public static void ReleaseControl(TDxControl self)
    {
        var root = RootCtrlOf(self);
        if (root != null)
        {
            root.DeleteModalForm(self);

            if (ReferenceEquals(root.ActiveMenu, self)) root.ActiveMenu = null;
            if (ReferenceEquals(root.FocusedControl, self)) root.FocusedControl = null;
            if (ReferenceEquals(root.ScrollControl, self)) root.ScrollControl = null;

            if (ReferenceEquals(root.MouseDownControl, self)) SetMouseDowned(self, false);
            if (ReferenceEquals(root.MouseMoveControl, self)) SetMouseMoveed(self, false);
        }

        for (int i = 0; i < ComponentCount(self); i++)
            ReleaseControl(Components(self, i));
    }

    /// <summary>DxControls.pas 3025-3035 TDxControl.SetDesigning（递归下传）。</summary>
    public static void SetDesigning(TDxControl self, bool value)
    {
        if (self.Designing == value) return;
        self.Designing = value;
        for (int i = 0; i < ComponentCount(self); i++)
        {
            // 原文 3031-3032 是 `Control[Index].Designing := Value` —— 属性写入会虚分派到
            // TDxControlEngine 的覆写（DxControls.pas 612-613 只有 inherited），而引擎覆写不再下钻。
            // 托管侧引擎的覆写点落在本方法，故这里直接递归下钻，语义与原文一致。
            SetDesigning(Components(self, i), value);
        }
    }

    // ---- 原文 3039-3091：FormatCaption / DoUpdate / Update ----

    /// <summary>
    /// g_MoneyList 的查表接缝（原文 MShare.g_MoneyList，属未移植单元）。
    /// 第一项 = GetIndex，第二项 = Objects[nIdx]；缺省两者为 null（等价于 GetIndex 恒返回 -1）。
    /// </summary>
    public static Func<string, int> MoneyListGetIndex;
    public static Func<int, long> MoneyListGetValue;

    /// <summary>原文 TDxControl.RawText（481；唯一写入者是 SetCaptionA）。</summary>
    public static string RawTextOf(TDxControl self) => DxControlHooks.GetRawText(self);

    /// <summary>
    /// DxControls.pas 3039-3063 TDxControl.FormatCaption：
    /// RawText 同时含 '&lt;' 与 '#' 时取**一次** `&lt;...&gt;` 变量；仅处理 `#MONEY(` 前缀；
    /// 查表命中取真值、未命中填 '0'；最后 SetCaptionV（**不动 FRawText**，可反复调用）。
    /// 原文引用的 ArrestVariable / CompareLStr / ArrestStringEx / sub_49ADB8 一并移植。
    /// </summary>
    public static void FormatCaption(TDxControl self)
    {
        var rawText = RawTextOf(self);
        if (rawText == "") return;
        string sCaption = rawText;
        string s14 = sCaption;
        if (DxControlsUnit.Pos(rawText, "<") > 0 && DxControlsUnit.Pos(rawText, "#") > 0)
        {
            ArrestVariable(ref s14, '<', '#', '>', 1, out string sVariable);
            if (sVariable == "") return;
            if (CompareLStr(sVariable, "#MONEY(", 7))
            {
                DxStringArrest.ArrestStringEx(ref sVariable, '(', ')', out string sName);
                int nIdx = MoneyListGetIndex != null ? MoneyListGetIndex(sName) : -1;
                string repl = nIdx >= 0
                    ? (MoneyListGetValue != null ? MoneyListGetValue(nIdx) : 0).ToString()
                    : "0";
                sCaption = Sub49ADB8(sCaption, "<" + sVariable + ">", repl);
            }
        }
        SetCaptionV(self, sCaption);
    }

    /// <summary>HUtil32.CompareLStr：长度受限、大小写不敏感的「前 Len 个字符相同」判定。</summary>
    public static bool CompareLStr(string s1, string s2, int len)
    {
        if (s1 == null || s2 == null) return false;
        if (len < 0) return false;
        if (s1.Length < len || s2.Length < len) return false;
        for (int i = 0; i < len; i++)
        {
            if (char.ToUpperInvariant(s1[i]) != char.ToUpperInvariant(s2[i])) return false;
        }
        return true;
    }

    /// <summary>
    /// HUtil32.ArrestVariable：取第 AIndex 个 AStart 起、到第一个 AEndChar 为止的内容，
    /// 要求其中含 ASep（原文以 '#' 作此判定）。返回结束符之后的 1-based 位置，失败返回 0。
    /// </summary>
    public static int ArrestVariable(ref string source, char aStart, char aSep, char aEndChar, int aIndex,
        out string variable)
    {
        variable = "";
        if (string.IsNullOrEmpty(source)) return 0;

        int counting = 0;
        int start = -1;
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == aStart)
            {
                counting++;
                if (counting >= aIndex) { start = i; break; }
            }
        }
        if (start < 0) return 0;

        int end = source.IndexOf(aEndChar, start + 1);
        if (end < 0) return 0;

        string candidate = source.Substring(start + 1, end - start - 1);
        if (candidate.IndexOf(aSep) < 0) return 0;

        variable = candidate;
        return end + 2;      // 1-based 且指向结束符之后
    }

    /// <summary>
    /// 原文 sub_49ADB8：把 sSource 中**第 1 次**出现的 sOld 替换成 sNew，返回整串。
    /// （原文的 nPos 形参在此实现里未参与定位，逐字保留形参。）
    /// </summary>
    public static string Sub49ADB8(string sSource, string sOld, string sNew)
    {
        if (string.IsNullOrEmpty(sSource) || string.IsNullOrEmpty(sOld)) return sSource;
        int idx = sSource.IndexOf(sOld, StringComparison.Ordinal);
        if (idx < 0) return sSource;
        return sSource.Substring(0, idx) + sNew + sSource.Substring(idx + sOld.Length);
    }

    /// <summary>DxControls.pas 3065-3069 TDxControl.DoUpdate。</summary>
    public static void DoUpdate(TDxControl self)
    {
        FormatCaption(self);
        DxControlHooks.GetOnUpdate(self)?.Invoke(self);
    }

    /// <summary>
    /// DxControls.pas 3082-3091 TDxControl.Update：
    /// **只对自己**判 `(Enabled or (not Enabled and Designing)) and Visible` 才 DoUpdate，
    /// 但**无条件**递归全部子控件（原文如此 —— 子控件没有 visible/enabled 门控）。
    /// </summary>
    public static void Update(TDxControl self)
    {
        if ((self.Enabled || (!self.Enabled && self.Designing)) && self.Visible)
            DoUpdate(self);

        for (int i = 0; i < ComponentCount(self); i++)
            Update(Components(self, i));
    }

    // ---- 原文 3147-3197：GetPaintRect ----

    /// <summary>
    /// DxControls.pas 3147-3197 TDxControl.GetPaintRect 1:1。
    /// 与 ReallyPaintRect 的差别：宽高取 `Min(ShortRect(DestRect, vbRect) 的宽, SrcRect 的宽)`，
    /// **不**再减 nLeft/nTop。原文 3177-3188 被注释掉的旧分支照抄留档。
    /// </summary>
    public static TDxRect GetPaintRect(TDxRect destRect, TDxRect srcRect, TDxRect vtRect, TDxRect vbRect,
        out int nX, out int nY)
    {
        nX = -1;
        nY = -1;
        var result = TDxRect.Rect(0, 0, 0, 0);
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return result;

        int nLeft = vbRect.Left > destRect.Left ? vbRect.Left - destRect.Left : 0;
        nX = destRect.Left + nLeft;

        int nTop = vbRect.Top > destRect.Top ? vbRect.Top - destRect.Top : 0;
        nY = destRect.Top + nTop;

        if (destRect.Bottom - destRect.Top <= nTop || destRect.Right - destRect.Left <= nLeft) return result;

        var paintRect = DxRectUtil.ShortRect(destRect, vbRect);
        int nWidth = Math.Min(paintRect.Right - paintRect.Left, srcRect.Right - srcRect.Left);
        int nHeight = Math.Min(paintRect.Bottom - paintRect.Top, srcRect.Bottom - srcRect.Top);

        // 原文 3177-3188（被注释掉的旧分支，原文如此）：
        //   if vbRect.Right < DestRect.Right then nWidth := vbRect.Right - DestRect.Left
        //   else nWidth := DestRect.Right - DestRect.Left;
        //   if DestRect.Bottom > vbRect.Bottom then nHeight := vbRect.Bottom - DestRect.Top
        //   else nHeight := DestRect.Bottom - DestRect.Top;
        //   nWidth := nWidth - nLeft;  nHeight := nHeight - nTop;

        if (nHeight <= 0 || nWidth <= 0) return result;

        paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(srcRect.Left + nLeft, srcRect.Top + nTop, nWidth, nHeight), srcRect);

        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return result;

        return paintRect;
    }

    // ---- 原文 3227-3392：FillRect / FillRectAlpha / FrameRect ----

    /// <summary>DxControls.pas 3227-3230 FillRect(DestRect, Color) → 自取 VirtualRect/VisibleRect。</summary>
    public static void FillRectOne(TDxControl self, TDxRect destRect, int color)
        => FillRect(self, destRect, self.VirtualRect, self.VisibleRect, color);

    /// <summary>DxControls.pas 3233-3236 FillRectAlpha(DestRect, Color, Alpha)。</summary>
    public static void FillRectAlphaOne(TDxControl self, TDxRect destRect, int color, byte alpha)
        => FillRectAlpha(self, destRect, self.VirtualRect, self.VisibleRect, color, alpha);

    /// <summary>DxControls.pas 3239-3242 FrameRect(DestRect, Color)。</summary>
    public static void FrameRectOne(TDxControl self, TDxRect destRect, int color)
        => FrameRect(self, destRect, self.VirtualRect, self.VisibleRect, color);

    /// <summary>
    /// DxControls.pas 3246-3284 TDxControl.FillRect(DestRect, vtRect, vbRect, Color)：
    /// 纯几何裁剪，最终 `GameCanvas.FillRect(PaintRect, Color)` → IDxSurfacePainter.FillRect。
    /// </summary>
    public static void FillRect(TDxControl self, TDxRect destRect, TDxRect vtRect, TDxRect vbRect, int color)
    {
        if (!CropFill(self, destRect, vtRect, vbRect, out var paintRect)) return;
        self.Painter.FillRect(paintRect, vtRect, vbRect, color);
    }

    /// <summary>
    /// DxControls.pas 3286-3328 TDxControl.FillRectAlpha：
    /// **`Alpha &gt;= 255` 时退化为 FillRect**（原文判定是 &gt;=，不是 =），
    /// 否则 `GameCanvas.FillRectAlpha(PaintRect, Color, Alpha)`。
    /// </summary>
    public static void FillRectAlpha(TDxControl self, TDxRect destRect, TDxRect vtRect, TDxRect vbRect,
        int color, byte alpha)
    {
        if (alpha >= 255)
        {
            FillRect(self, destRect, vtRect, vbRect, color);
            return;
        }

        if (!CropFill(self, destRect, vtRect, vbRect, out var paintRect)) return;
        DxPainterExt.FillRectAlpha(self.Painter, paintRect, vtRect, vbRect, color, alpha);
    }

    /// <summary>
    /// DxControls.pas 3330-3392 TDxControl.FrameRect(DestRect, vtRect, vbRect, Color)：
    /// 四条边**各自独立判定**（上/右/下/左），左竖线起点是 `PaintRect.Top - 1`（原文如此）。
    /// 四次 GameCanvas.Line 的顺序不可换。
    /// </summary>
    public static void FrameRect(TDxControl self, TDxRect destRect, TDxRect vtRect, TDxRect vbRect, int color)
    {
        var ok = CropFill(self, destRect, vtRect, vbRect, out var paintRect);
        if (!ok) return;

        var p = self.Painter;

        if (destRect.Top >= vbRect.Top)        // 上横
            p.Line(TDxPoint.Point(paintRect.Left, paintRect.Top),
                   TDxPoint.Point(paintRect.Right, paintRect.Top), color);

        if (destRect.Right <= vbRect.Right)    // 右竖
            p.Line(TDxPoint.Point(paintRect.Right, paintRect.Top),
                   TDxPoint.Point(paintRect.Right, paintRect.Bottom), color);

        if (destRect.Bottom <= vbRect.Bottom)  // 下横
            p.Line(TDxPoint.Point(paintRect.Left, paintRect.Bottom),
                   TDxPoint.Point(paintRect.Right, paintRect.Bottom), color);

        if (destRect.Left >= vbRect.Left)      // 左竖
            p.Line(TDxPoint.Point(paintRect.Left, paintRect.Top - 1),
                   TDxPoint.Point(paintRect.Left, paintRect.Bottom), color);
    }

    /// <summary>
    /// DxControls.pas 3246-3284 / 3286-3328 / 3330-3392 三处**完全同形**的填充类裁剪前导：
    /// `PaintRect := ShortRect(DestRect, vtRect)` → nLeft/nX → nTop/nY → 退化判定 → nWidth/nHeight
    /// → 减 nLeft/nTop → 再判一次 → `Bounds(nX, nY, nWidth, nHeight)`。返回 false 表示原文会 Exit。
    /// （原文三段各自展开了一遍；此处收拢，逐条判定顺序与原文字面一致。）
    /// </summary>
    private static bool CropFill(TDxControl self, TDxRect destRect, TDxRect vtRect, TDxRect vbRect,
        out TDxRect paintRect)
    {
        paintRect = DxRectUtil.ShortRect(destRect, vtRect);

        int nLeft = vbRect.Left > paintRect.Left ? vbRect.Left - paintRect.Left : 0;
        int nX = paintRect.Left + nLeft;

        int nTop = vbRect.Top > paintRect.Top ? vbRect.Top - paintRect.Top : 0;
        int nY = paintRect.Top + nTop;

        if (paintRect.Bottom - paintRect.Top <= nTop || paintRect.Right - paintRect.Left <= nLeft) return false;

        int nWidth = vbRect.Right < paintRect.Right ? vbRect.Right - paintRect.Left : paintRect.Right - paintRect.Left;
        int nHeight = paintRect.Bottom > vbRect.Bottom ? vbRect.Bottom - paintRect.Top : paintRect.Bottom - paintRect.Top;

        nWidth -= nLeft;
        nHeight -= nTop;

        if (nHeight <= 0 || nWidth <= 0) return false;

        paintRect = TDxRect.Bounds(nX, nY, nWidth, nHeight);
        return true;
    }

    // ---- 原文 3394-3750：DrawRect / DrawRectColor / DrawRectColorAlpha ----

    /// <summary>DxControls.pas 3394-3443 DrawRect(DestRect, Texture, ABlendMode)（自取 vb/vt）。</summary>
    public static void DrawRect(TDxControl self, TDxRect destRect, IDxTexture texture, int blendMode)
        => DrawRectScoped(self, destRect, self.VirtualRect, self.VisibleRect, texture, blendMode);

    /// <summary>DxControls.pas 3445-3451 DrawRect(DestRect, vtRect, vbRect, Texture, ABlendMode) → nPX=nPY=0 重载。</summary>
    public static void DrawRectScoped(TDxControl self, TDxRect destRect, TDxRect vtRect, TDxRect vbRect,
        IDxTexture texture, int blendMode)
    {
        // 原文 3447-3492（被注释掉的展开实现，原文如此）：
        //   var PaintRect:TRect; nX, nY, nLeft, nTop, nWidth, nHeight:Integer;
        //   ... 与 3495 重载同形的整套裁剪 ...
        //   // GameCanvas.Draw(nX, nY, PaintRect, Texture, ABlendMode);
        DrawRect(self, 0, 0, destRect, vtRect, vbRect, texture, blendMode);
    }

    /// <summary>DxControls.pas 3495-3542 DrawRect(nPX, nPY, DestRect, vtRect, vbRect, Texture, ABlendMode)。</summary>
    public static void DrawRect(TDxControl self, int nPX, int nPY, TDxRect destRect, TDxRect vtRect, TDxRect vbRect,
        IDxTexture texture, int blendMode)
    {
        if (texture == null) return;               // 原文未判 nil（Texture.Width 会 AV）；托管侧守卫
        if (texture.Width * texture.Height <= 4) return;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;

        if (!CropDraw(destRect, vbRect, out int nLeft, out int nTop, out int nX, out int nY,
                out int nWidth, out int nHeight)) return;

        var src = texture.ClientRect;
        var paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(src.Left + nLeft, src.Top + nTop, nWidth, nHeight), src);
        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return;

        self.Painter.Draw(nX + nPX, nY + nPY, paintRect, texture);

        // 原文 ABlendMode 形参在 3495 重载里**未被使用**（GameCanvas.Draw 只有 4 参重载被调用），逐字保留形参。
        _ = blendMode;
    }

    /// <summary>
    /// DxControls.pas 3544-3598 DrawRectColorAlpha(DestRect, Texture, Color, Alpha, ABlendMode)。
    /// 注意此处退化为 DrawRectColor 的判定是 `Alpha = 255`（与 FillRectAlpha 的 `>=` 不同）。
    /// </summary>
    public static void DrawRectColorAlpha(TDxControl self, TDxRect destRect, IDxTexture texture, int color,
        byte alpha, int blendMode)
    {
        if (texture == null) return;
        if (texture.Width * texture.Height <= 4) return;
        if (alpha == 255)
        {
            DrawRectColor(self, destRect, texture, color, blendMode);
            return;
        }

        var vbRect = self.VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;

        if (!CropDraw(destRect, vbRect, out int nLeft, out int nTop, out int nX, out int nY,
                out int nWidth, out int nHeight)) return;

        var src = texture.ClientRect;
        var paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(src.Left + nLeft, src.Top + nTop, nWidth, nHeight), src);
        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return;

        // 原文 3596 的调用形参顺序为 (nX, nY, PaintRect, Texture, Alpha, Color, ABlendMode)
        // —— Alpha/Color 位置与 3648 行相反（原文如此，逐字保留）。
        DxPainterExt.DrawColorAlpha(self.Painter, nX, nY, paintRect, texture, color, alpha, blendMode);
    }

    /// <summary>
    /// DxControls.pas 3600-3650 DrawRectColorAlpha(DestRect, vtRect, vbRect, Texture, Color, Alpha, ABlendMode)。
    /// 原文 3608 的 `= 255` 分支把 `Texture.ClientRect` 当 SrcRect 传下去。
    /// </summary>
    public static void DrawRectColorAlphaScoped(TDxControl self, TDxRect destRect, TDxRect vtRect, TDxRect vbRect,
        IDxTexture texture, int color, byte alpha, int blendMode)
    {
        if (texture == null) return;
        if (texture.Width * texture.Height <= 4) return;
        if (alpha == 255)
        {
            DrawRectColor(self, destRect, texture.ClientRect, vtRect, vbRect, texture, color, blendMode);
            return;
        }
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;

        if (!CropDraw(destRect, vbRect, out int nLeft, out int nTop, out int nX, out int nY,
                out int nWidth, out int nHeight)) return;

        var src = texture.ClientRect;
        var paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(src.Left + nLeft, src.Top + nTop, nWidth, nHeight), src);
        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return;

        DxPainterExt.DrawColorAlpha(self.Painter, nX, nY, paintRect, texture, color, alpha, blendMode);
    }

    /// <summary>DxControls.pas 3652-3702 DrawRectColor(DestRect, Texture, Color, ABlendMode)（自取 vb/vt）。</summary>
    public static void DrawRectColor(TDxControl self, TDxRect destRect, IDxTexture texture, int color, int blendMode)
    {
        if (texture == null) return;
        if (texture.Width * texture.Height <= 4) return;

        var vbRect = self.VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;

        if (!CropDraw(destRect, vbRect, out int nLeft, out int nTop, out int nX, out int nY,
                out int nWidth, out int nHeight)) return;

        var src = texture.ClientRect;
        var paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(src.Left + nLeft, src.Top + nTop, nWidth, nHeight), src);
        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return;

        DxPainterExt.DrawColor(self.Painter, nX, nY, paintRect, texture, color, blendMode);
    }

    /// <summary>DxControls.pas 3704-3750 DrawRectColor(DestRect, SrcRect, vtRect, vbRect, Texture, Color, ABlendMode)。</summary>
    public static void DrawRectColor(TDxControl self, TDxRect destRect, TDxRect srcRect, TDxRect vtRect,
        TDxRect vbRect, IDxTexture texture, int color, int blendMode)
    {
        if (texture == null) return;
        if (texture.Width * texture.Height <= 4) return;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;

        if (!CropDraw(destRect, vbRect, out int nLeft, out int nTop, out int nX, out int nY,
                out int nWidth, out int nHeight)) return;

        srcRect = DxRectUtil.ShortRect(srcRect, texture.ClientRect);
        var paintRect = DxRectUtil.ShortRect(
            TDxRect.Bounds(srcRect.Left + nLeft, srcRect.Top + nTop, nWidth, nHeight), srcRect);
        if (paintRect.Bottom <= paintRect.Top || paintRect.Right <= paintRect.Left) return;

        DxPainterExt.DrawColor(self.Painter, nX, nY, paintRect, texture, color, blendMode);
    }

    /// <summary>
    /// 原文 3106-3135 / 3421-3436 / 3506-3535 / 3560-3589 / 3612-3641 / 3665-3694 / 3713-3742
    /// **完全同形**的绘制类裁剪前导（调用方已先判过 vbRect 退化）：
    /// nLeft/nX → nTop/nY → `尺寸 <= nTop/nLeft` → nWidth/nHeight → 减 nLeft/nTop → `<= 0` 判定。
    /// 返回 false 表示原文会 Exit。
    /// </summary>
    private static bool CropDraw(TDxRect destRect, TDxRect vbRect,
        out int nLeft, out int nTop, out int nX, out int nY, out int nWidth, out int nHeight)
    {
        nLeft = vbRect.Left > destRect.Left ? vbRect.Left - destRect.Left : 0;
        nX = destRect.Left + nLeft;

        nTop = vbRect.Top > destRect.Top ? vbRect.Top - destRect.Top : 0;
        nY = destRect.Top + nTop;

        nWidth = 0;
        nHeight = 0;

        if (destRect.Bottom - destRect.Top <= nTop || destRect.Right - destRect.Left <= nLeft) return false;

        nWidth = vbRect.Right < destRect.Right ? vbRect.Right - destRect.Left : destRect.Right - destRect.Left;
        nHeight = destRect.Bottom > vbRect.Bottom ? vbRect.Bottom - destRect.Top : destRect.Bottom - destRect.Top;

        nWidth -= nLeft;
        nHeight -= nTop;

        return nHeight > 0 && nWidth > 0;
    }

    // ---- 原文 3852-3877：Repaint / Paint ----

    /// <summary>DxControls.pas 3852-3855 TDxControl.Repaint（转发给 RootCtrl 的 FOnRepaint）。</summary>
    public static void Repaint(TDxControl self) => RootCtrlOf(self)?.Repaint();

    /// <summary>
    /// DxControls.pas 3859-3877 TDxControl.Paint：
    /// vbRect 退化直接 Exit；设计期先画红框；随后**逆序**（最后一个子控件往前）绘制可见子控件。
    /// 原文 3869 注释掉的那行照抄留档。
    /// </summary>
    public static void Paint(TDxControl self)
    {
        var vbRect = self.VisibleRect;
        if (vbRect.Bottom <= vbRect.Top || vbRect.Right <= vbRect.Left) return;
        var vtRect = self.VirtualRect;

        if (self.Designing)
        {
            // 原文 3869（被注释掉）：// Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
            FrameRect(self, vtRect, vtRect, vbRect, TDxColor.clRed);
        }

        for (int i = ComponentCount(self) - 1; i >= 0; i--)
        {
            var c = Components(self, i);
            if (c.Visible) Paint(c);
        }
    }

    // ---- 原文 3880-3954 / 4036-4044：事件入口 ----

    /// <summary>DxControls.pas 3880-3885 DoClick（原文另有一行注释掉的 DebugOut）。</summary>
    public static void DoClick(TDxControl self, int x, int y)
    {
        DxControlHooks.GetOnClick(self)?.Invoke(self, x, y);
    }

    /// <summary>DxControls.pas 3889-3893 DoDblClick。</summary>
    public static void DoDblClick(TDxControl self, int x, int y)
    {
        DxControlHooks.GetOnDblClick(self)?.Invoke(self, x, y);
    }

    /// <summary>DxControls.pas 3933-3936 DblClick（原文走 DoDblClick）。</summary>
    public static void DblClick(TDxControl self, int x, int y) => DoDblClick(self, x, y);

    /// <summary>DxControls.pas 3938-3942 KeyDown。</summary>
    public static void KeyDown(TDxControl self, UShortRef key, TDxShiftState shift)
        => DxControlHooks.GetOnKeyDown(self)?.Invoke(self, key, shift);

    /// <summary>DxControls.pas 3944-3948 KeyPress。</summary>
    public static void KeyPress(TDxControl self, CharRef key)
        => DxControlHooks.GetOnKeyPress(self)?.Invoke(self, key);

    /// <summary>DxControls.pas 3950-3954 KeyUp。</summary>
    public static void KeyUp(TDxControl self, UShortRef key, TDxShiftState shift)
        => DxControlHooks.GetOnKeyUp(self)?.Invoke(self, key, shift);

    /// <summary>DxControls.pas 4036-4039 MouseWheelDown（基类空实现）。</summary>
    public static void MouseWheelDown(TDxControl self, TDxShiftState shift, TDxPoint mousePos) { }

    /// <summary>DxControls.pas 4041-4044 MouseWheelUp（基类空实现）。</summary>
    public static void MouseWheelUp(TDxControl self, TDxShiftState shift, TDxPoint mousePos) { }

    // ---- 原文 3996-4004：SetMousePoint ----

    /// <summary>
    /// DxControls.pas 3996-4004 TDxControl.SetMousePoint：
    /// `SpotX := X; SpotY := Y; FMouseDownX := X; FMouseDownY := X;` ——
    /// **FMouseDownY 被赋成 X**（原文笔误，DxControls.pas:4001），逐字照抄；随后递归到 Owner。
    /// </summary>
    public static void SetMousePoint(TDxControl self, int x, int y)
    {
        var slot = DxControlHooks.Spots(self);
        slot[0] = x;    // SpotX
        slot[1] = y;    // SpotY
        slot[2] = x;    // FMouseDownX
        slot[3] = x;    // FMouseDownY —— 原文如此（用的是 X 不是 Y）
        if (self.DxOwner is TDxControl owner)
            SetMousePoint(owner, x, y);
    }

    // ---- 原文 4046-4128：MouseDown / MouseMove / MouseUp ----

    /// <summary>DxControls.pas 4046-4053 TDxControl.MouseDown（FCanMouse 判定被原文注释掉）。</summary>
    public static void MouseDown(TDxControl self, TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        // 原文 4048-4049（被注释掉）：
        //   // FCanMouse := ((Button = mbLeft) and (mbLeft in FMouseEvents)) or
        //   //   ((Button = mbRight) and (mbRight in FMouseEvents));
        DxControlHooks.GetOnMouseDown(self)?.Invoke(self, button, shift, x, y);
    }

    /// <summary>DxControls.pas 4109-4116 TDxControl.MouseMove（FCanMouse 判定被原文注释掉）。</summary>
    public static void MouseMove(TDxControl self, TDxShiftState shift, int x, int y)
    {
        // 原文 4111-4112（被注释掉）：
        //   // FCanMouse := ((ssLeft in Shift) and (mbLeft in FMouseEvents)) or
        //   //   ((ssRight in Shift) and (mbRight in FMouseEvents));
        DxControlHooks.GetOnMouseMove(self)?.Invoke(self, shift, x, y);
    }

    /// <summary>
    /// DxControls.pas 4118-4128 TDxControl.MouseUp：
    /// **先重算 FCanMouse** = (左键 and mbLeft in MouseEvents) or (右键 and mbRight in MouseEvents)；
    /// OnMouseUp 与 DoClick 都受它门控。
    /// 上游接缝的 CanMouse 是只读属性（构造恒 True 且原文除此处外无写入者），
    /// 故此处只保留门控变量计算（差异已登记）。
    /// </summary>
    public static void MouseUp(TDxControl self, TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        bool canMouse = (button == TDxMouseButton.mbLeft && self.MouseEvents.HasFlag(TMouseEvents.mbLeft))
                     || (button == TDxMouseButton.mbRight && self.MouseEvents.HasFlag(TMouseEvents.mbRight));

        if (canMouse)
            DxControlHooks.GetOnMouseUp(self)?.Invoke(self, button, shift, x, y);

        if (canMouse)
            DoClick(self, x, y);
    }

    /// <summary>DxControls.pas 4055-4058 TDxControl.CanMove（基类恒 True）。</summary>
    public static bool CanMove(TDxControl self) => CanMoveSink != null ? CanMoveSink(self) : true;

    /// <summary>
    /// 原文 TDxControl.CanMove 的可覆写落点（上游接缝的 CanMove 是 protected 且无参，
    /// 无法覆写；TDxImageForm 用 `not FNoMove` 覆写它）。缺省 null = 基类恒 True。
    /// </summary>
    public static Func<TDxControl, bool> CanMoveSink;

    /// <summary>
    /// DxControls.pas 4060-4101 TDxControl.Move（拖拽）：
    /// 门控 `CanMove and (Floating or Designing) and (SpotX&lt;&gt;X or SpotY&lt;&gt;Y) and X/Y&gt;=0
    ///       and X&lt;=RootCtrl.Width and Y&lt;=RootCtrl.Height`；
    /// 目标越界时**回退为原位**（`al := Left`，不是夹紧到边界）；
    /// 原文 4084-4087 的注释版本照抄留档；第一分支未进且 OwnerMove 时把 Move 上抛给父控件。
    /// </summary>
    public static void Move(TDxControl self, int x, int y)
    {
        var root = RootCtrlOf(self);
        if (CanMove(self) && (self.Floating || self.Designing) &&
            (DxControlHooks.SpotX(self) != x || DxControlHooks.SpotY(self) != y) &&
            x >= 0 && y >= 0 &&
            (root == null || (x <= root.Width && y <= root.Height)))
        {
            int al = self.Left + (x - DxControlHooks.SpotX(self));
            int at = self.Top + (y - DxControlHooks.SpotY(self));

            if (self.DxOwner != null)
            {
                TDxRect range = ReferenceEquals(self.DxOwner, root)
                    ? root.ClientRect
                    : DxControlHooks.GetMoveRange(self.DxOwner);

                if (al + self.Width < range.Left) al = self.Left;
                if (al > range.Right) al = self.Left;
                if (at + self.Height < range.Top) at = self.Top;
                if (at > range.Bottom) at = self.Top;

                // 原文 4084-4087（被注释掉的旧实现，原文如此）：
                //   if al + Width < Range.Left then al := Range.Left - Width;
                //   if al > Range.Right then al := Range.Right;
                //   if at + Height < Range.Top then at := Range.Top - Height;
                //   if at + Height > Range.Bottom then at := Range.Bottom - Height;
            }

            self.Left = al;
            self.Top = at;
            var slot = DxControlHooks.Spots(self);
            slot[0] = x;
            slot[1] = y;

            OnMovedCore(self);
            return;
        }

        if (!self.Designing && CanMove(self) && self.OwnerMove && self.DxOwner is TDxControl owner)
            Move(owner, x, y);
    }

    // ---- 原文 4016-4034：FindActiveControl（基类形态；引擎版本另行覆写语义） ----

    /// <summary>
    /// DxControls.pas 4016-4034 TDxControl.FindActiveControl。
    /// 上游接缝未提供该成员（不是 virtual），故落在这里。
    /// 语义：先看 OnFindActiveControl 回调；否则要求
    /// `Visible and EnableMouse and (Enabled or (not Enabled and Designing)) and CanDraw
    ///  and PointInRect(Point(X,Y), VisibleRect)`，再 `InRange` 命中取自身，
    /// 随后**逆序**遍历子控件、只要子控件返回非 nil 就覆盖 Result（**不 break**，
    /// 于是最终 Result 是**最靠前**（索引最小）的命中子控件）。
    /// </summary>
    public static TDxControl FindActiveControl(TDxControl self, int x, int y)
    {
        var onFind = DxControlHooks.GetOnFindActiveControl(self);
        if (onFind != null)
            return onFind(self, x, y);

        TDxControl result = null;
        if (self.Visible && self.EnableMouse &&
            (self.Enabled || (!self.Enabled && self.Designing)) &&
            self.CanDraw() &&
            DxRectUtil.PointInRect(TDxPoint.Point(x, y), self.VisibleRect))
        {
            if (self.InRange(x, y)) result = self;
            for (int i = ComponentCount(self) - 1; i >= 0; i--)
            {
                var d = FindActiveControl(Components(self, i), x, y);
                if (d != null)
                    result = d;
            }
        }
        return result;
    }

    /// <summary>DxControls.pas 4130-4140 GetAllSubComponents：**不含** Ctrl 自身；先加直接子再递归。</summary>
    public static void GetAllSubComponents(TDxControl ctrl, List<TDxControl> list)
    {
        if (ctrl == null || list == null) return;
        for (int i = 0; i < ComponentCount(ctrl); i++)
        {
            var child = Components(ctrl, i);
            list.Add(child);
            if (ComponentCount(child) > 0)
                GetAllSubComponents(child, list);
        }
    }
}

// -------------------------------------------------------------------------------------
// 引擎（控件树根容器）
// -------------------------------------------------------------------------------------

/// <summary>
/// DxControls.pas 576-653 / 1096-1794 TDxControlEngine
/// （控件树根：模态窗体栈、焦点、鼠标键盘派发、绘制遍历）。
///
/// 与原文的签名差异：原文 TDxControlEngine 把 DblClick/KeyDown/KeyPress/KeyUp/MouseDown/MouseMove/
/// MouseUp/MouseWheelDown/MouseWheelUp 都改成了 **Boolean** 返回；托管侧 TDxControl 只有
/// MouseDown/MouseMove/MouseUp 是 virtual（其余名字与 WinForms Control 冲突且非虚），
/// 故统一以 `Port*` 命名承接（返回值即原文的 Boolean），并对那三个可覆写的加同签名 override。
/// </summary>
public class TDxControlEngine : TDxControl
{
    private static readonly List<TDxControlEngine> ControlEngineList = new();

    private readonly List<TDxControl> _modalForms = new();      // FModalForms
    private readonly List<TDxControl> _topForms = new();         // FTopForms（原文创建后未被读写）
    private readonly List<TDxControl> _toFrontList = new();      // FToFrontList
    private readonly List<TDxControl> _toBackList = new();       // FToBackList
    private readonly object _gate = new();                       // FCriticalSection

    private TDxControl _activeControl;      // FActiveControl
    private TDxControl _activeMenu;         // FActiveMenu
    private TDxControl _imeWindow;          // FImeWindow
    private TDxControl _candidateWindow;    // FCandidateWindow
    private TDxControl _focusedControl;     // FFocusedControl
    private TDxControl _mouseDownControl;   // FMouseDownControl
    private TDxControl _mouseMoveControl;   // FMouseMoveControl
    private TDxControl _scrollControl;      // FScrollControl
    private bool _inRepaint;                // 托管侧：防 Repaint 自递归

    /// <summary>原文 578 FOnBackgroundClick。</summary>
    public Action<TDxControlEngine> OnBackgroundClick;

    /// <summary>原文 590 FOnRepaint。</summary>
    public Action<TDxControlEngine> OnRepaint;

    /// <summary>原文 614 WantReturn（MouseDown / MouseUp / MouseMove 事件回调的返回值门）。</summary>
    public bool WantReturn;

    /// <summary>原文 1143 `Name := MakeName(Application.MainForm, ClassName)` 的结果。</summary>
    public string DxName { get; private set; } = "";

    public TDxControlEngine()
    {
        // 原文 1140-1155 逐条（InitializeCriticalSection → inherited Create(nil) → FRootCtrl := Self → …）
        DxRootRegistry.Set(this, this);            // FRootCtrl := Self
        DxName = MakeName(TDxApplication.GetMainFormForNaming(), GetType().Name);
        DxControlHooks.InitName(this, DxName);
        OnRepaint = null;
        _activeMenu = null;
        _focusedControl = null;
        _mouseDownControl = null;
        _mouseMoveControl = null;
        _scrollControl = null;
        _activeControl = null;
        lock (ControlEngineList) ControlEngineList.Add(this);
    }

    // ---- 原文 1098-1138：FindComponentName / MakeName ----

    /// <summary>
    /// DxControls.pas 1098-1121 FindComponentName（Master 的子控件树 + 全局引擎表）。
    /// 原文形参是 `Master:TComponent`；托管侧接缝控件派生自 WinForms Control，故接 object 后逐类型判定。
    /// </summary>
    private static bool FindComponentName(object master, string className)
    {
        if (master is TDxControl dx)
        {
            for (int i = 0; i < DxControlOps.ComponentCount(dx); i++)
            {
                var child = DxControlOps.Components(dx, i);
                if (string.Equals(DxControlHooks.NameOf(child), className, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (FindComponentName(child, className))
                    return true;
            }
        }
        else if (master is Control wf && wf.Controls != null)
        {
            foreach (Control comp in wf.Controls)
            {
                if (comp is TDxControl c &&
                    string.Equals(DxControlHooks.NameOf(c), className, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (FindComponentName(comp, className))
                    return true;
            }
        }

        lock (ControlEngineList)
        {
            for (int i = 0; i < ControlEngineList.Count; i++)
            {
                if (string.Equals(ControlEngineList[i].DxName, className, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }

    /// <summary>DxControls.pas 1123-1138 MakeName。</summary>
    private static string MakeName(object master, string className)
    {
        string result;
        if (DxControlsUnit.Pos(DxControlsUnit.LowerCase(className), "tdx") == 1)
            result = DxControlsUnit.Copy(className, 4, className.Length - 3);
        else
            result = className;

        int num = 1;
        while (master != null && FindComponentName(master, result + num.ToString()))
            num++;

        return result + num.ToString();
    }

    // ---- 原文 1208-1343：状态属性 ----

    /// <summary>DxControls.pas 1208-1213 SetActiveControl。</summary>
    public TDxControl ActiveControl
    {
        get => _activeControl;
        set { if (!ReferenceEquals(_activeControl, value)) _activeControl = value; }
    }

    /// <summary>DxControls.pas 1304-1311 SetActiveMenu（换值时把旧的 Visible 置 False）。</summary>
    public TDxControl ActiveMenu
    {
        get => _activeMenu;
        set
        {
            if (ReferenceEquals(_activeMenu, value)) return;
            if (_activeMenu != null) _activeMenu.Visible = false;
            _activeMenu = value;
        }
    }

    /// <summary>DxControls.pas 1286-1293 SetImeWindow（换值时把旧的 Visible 置 False）。</summary>
    public TDxControl ImeWindow
    {
        get => _imeWindow;
        set
        {
            if (ReferenceEquals(_imeWindow, value)) return;
            if (_imeWindow != null) _imeWindow.Visible = false;
            _imeWindow = value;
        }
    }

    /// <summary>DxControls.pas 1295-1302 SetCandidateWindow。</summary>
    public TDxControl CandidateWindow
    {
        get => _candidateWindow;
        set
        {
            if (ReferenceEquals(_candidateWindow, value)) return;
            if (_candidateWindow != null) _candidateWindow.Visible = false;
            _candidateWindow = value;
        }
    }

    /// <summary>DxControls.pas 1313-1322 SetFocusedControl（旧的 DoUnFocused → 新的 DoFocused）。</summary>
    public TDxControl FocusedControl
    {
        get => _focusedControl;
        set
        {
            if (ReferenceEquals(_focusedControl, value)) return;
            if (_focusedControl != null) DxControlOps.OnUnfocusedCore(_focusedControl);
            _focusedControl = value;
            if (_focusedControl != null) DxControlOps.OnFocusedCore(_focusedControl);
        }
    }

    /// <summary>DxControls.pas 1324-1329 SetMouseDownControl（裸赋值）。</summary>
    public TDxControl MouseDownControl
    {
        get => _mouseDownControl;
        set { if (!ReferenceEquals(_mouseDownControl, value)) _mouseDownControl = value; }
    }

    /// <summary>DxControls.pas 1331-1336 SetMouseMoveControl（裸赋值）。</summary>
    public TDxControl MouseMoveControl
    {
        get => _mouseMoveControl;
        set { if (!ReferenceEquals(_mouseMoveControl, value)) _mouseMoveControl = value; }
    }

    /// <summary>DxControls.pas 1338-1343 SetScrollControl。</summary>
    public TDxControl ScrollControl
    {
        get => _scrollControl;
        set { if (!ReferenceEquals(_scrollControl, value)) _scrollControl = value; }
    }

    /// <summary>DxControls.pas 1228-1238 GetModalForm（取 FModalForms[0]）。</summary>
    public TDxControl ModalForm => _modalForms.Count > 0 ? _modalForms[0] : null;

    /// <summary>DxControls.pas 1240-1250 GetModalFormEx（HZQ 20230525；与 ModalForm 同体）。</summary>
    public TDxControl ModalFormEx => _modalForms.Count > 0 ? _modalForms[0] : null;

    /// <summary>DxControls.pas 1252-1269 SetModalForm：已在表中则先摘再插到 0；否则直接插到 0。</summary>
    public void SetModalForm(TDxControl value)
    {
        int nIndex = _modalForms.IndexOf(value);
        if (nIndex >= 0)
        {
            _modalForms.RemoveAt(nIndex);
            _modalForms.Insert(0, value);
        }
        else
        {
            _modalForms.Insert(0, value);
        }
    }

    /// <summary>
    /// DxControls.pas 1271-1284 DeleteModalForm：
    /// **`if nIndex &gt; 0`** —— 索引 0（当前模态窗体）**永不**被删除（原文如此）。
    /// </summary>
    public void DeleteModalForm(TDxControl d)
    {
        int nIndex = _modalForms.IndexOf(d);
        if (nIndex > 0)
            _modalForms.RemoveAt(nIndex);
    }

    /// <summary>原文 FModalForms.Count（测试/诊断用）。</summary>
    public int ModalFormCount { get { lock (_modalForms) return _modalForms.Count; } }

    // ---- 原文 1345-1396：Lock / UnLock / ExecutePostion / AddBringToFront / AddSentToBack ----

    /// <summary>DxControls.pas 1345-1348 Lock。</summary>
    public void Lock() => System.Threading.Monitor.Enter(_gate);

    /// <summary>DxControls.pas 1350-1353 UnLock。</summary>
    public void UnLock() => System.Threading.Monitor.Exit(_gate);

    /// <summary>
    /// DxControls.pas 1355-1376 ExecutePostion（原文成员名如此：Postion）：
    /// 先清 FToBackList（逐项 `ComponentIndex := ComponentCount - 1`），再清 FToFrontList（逐项 0）。
    /// </summary>
    public void ExecutePosition()
    {
        while (true)
        {
            TDxControl item;
            lock (_toBackList)
            {
                if (_toBackList.Count == 0) break;
                item = _toBackList[0];
                _toBackList.RemoveAt(0);
            }
            DxControlOps.SetComponentIndex(item, DxControlOps.ComponentCount(this) - 1);
        }

        while (true)
        {
            TDxControl item;
            lock (_toFrontList)
            {
                if (_toFrontList.Count == 0) break;
                item = _toFrontList[0];
                _toFrontList.RemoveAt(0);
            }
            DxControlOps.SetComponentIndex(item, 0);
        }
    }

    /// <summary>DxControls.pas 1378-1386 AddBringToFront（原文成员名如此：Bring）。</summary>
    public void AddBringToFront(TDxControl d) { lock (_toFrontList) _toFrontList.Add(d); }

    /// <summary>DxControls.pas 1388-1396 AddSentToBack。</summary>
    public void AddSentToBack(TDxControl d) { lock (_toBackList) _toBackList.Add(d); }

    /// <summary>原文 FToFrontList.Count（测试/诊断用）。</summary>
    public int ToFrontQueueCount { get { lock (_toFrontList) return _toFrontList.Count; } }

    /// <summary>原文 FToBackList.Count（测试/诊断用）。</summary>
    public int ToBackQueueCount { get { lock (_toBackList) return _toBackList.Count; } }

    // ---- 原文 1215-1226：FindActiveControl ----

    /// <summary>
    /// DxControls.pas 1215-1226 TDxControlEngine.FindActiveControl：
    /// **正序**遍历子控件、取**第一个**非 nil 结果（命中即 break）。
    /// 上游接缝的 TDxControl.FindActiveControl 不是 virtual，故本方法用 `new` 屏蔽。
    /// </summary>
    public new TDxControl FindActiveControl(int x, int y)
    {
        TDxControl d = null;
        for (int i = 0; i < DxControlOps.ComponentCount(this); i++)
        {
            d = DxControlOps.FindActiveControl(DxControlOps.Components(this, i), x, y);
            if (d != null) break;
        }
        return d;
    }

    // ---- 原文 1174-1206：Initialize / Finalize / Repaint ----

    /// <summary>DxControls.pas 1174-1186 TDxControlEngine.Initialize（原文在 IsMultiThreadRender=1 时加锁）。</summary>
    public void Initialize()
    {
        lock (_gate) DxControlOps.Initialize(this);
    }

    /// <summary>DxControls.pas 1188-1200 TDxControlEngine.Finalize。</summary>
    public void FinalizeEngine()
    {
        lock (_gate) DxControlOps.FinalizeControls(this);
    }

    /// <summary>DxControls.pas 1202-1206 TDxControlEngine.Repaint（只转 FOnRepaint，自身不绘制）。</summary>
    public override void Repaint()
    {
        if (_inRepaint) return;
        _inRepaint = true;
        try { OnRepaint?.Invoke(this); }
        finally { _inRepaint = false; }
    }

    /// <summary>
    /// DxControls.pas 1745-1761 TDxControlEngine.Update：
    /// 只对 `Visible and Enabled` 的子控件调 Update（**引擎自身不做 DoUpdate**）。
    /// </summary>
    public void UpdateChildren()
    {
        lock (_gate)
        {
            for (int i = 0; i < DxControlOps.ComponentCount(this); i++)
            {
                var c = DxControlOps.Components(this, i);
                if (c.Visible && c.Enabled)
                    DxControlOps.Update(c);
            }
        }
    }

    /// <summary>
    /// DxControls.pas 1763-1794 TDxControlEngine.Paint：
    /// ① **逆序**绘制可见子控件，跳过 ActiveMenu / ModalForm；
    /// ② 再单独画 ModalForm（若可见）；③ 最后画 ActiveMenu（若可见）。
    /// 原文在 IsMultiThreadRender=1 时先 ExecutePostion 再 Lock。
    /// </summary>
    public void PaintChildren()
    {
        ExecutePosition();
        lock (_gate)
        {
            for (int i = DxControlOps.ComponentCount(this) - 1; i >= 0; i--)
            {
                var c = DxControlOps.Components(this, i);
                if (!c.Visible) continue;
                if (ReferenceEquals(c, _activeMenu) || ReferenceEquals(c, ModalForm)) continue;
                DxControlOps.Paint(c);
            }

            var d = ModalForm;
            if (d != null && d.Visible)
                DxControlOps.Paint(d);

            if (_activeMenu != null && _activeMenu.Visible)
                DxControlOps.Paint(_activeMenu);
        }
    }

    // ---- 原文 1398-1743：事件派发（Boolean 版本）----

    /// <summary>DxControls.pas 1398-1435 TDxControlEngine.DblClick。</summary>
    public bool PortDblClick(int x, int y)
    {
        if (_activeMenu != null && _activeMenu.Visible && _activeMenu.InRange(x, y))
        {
            DxControlOps.DblClick(_activeMenu, x, y);
            return true;
        }

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            var d = DxControlOps.FindActiveControl(aModalForm, x, y);
            if (d != null)
            {
                DxControlOps.DblClick(d, x, y);
                if (DxControlHooks.GetModalControl(d) == null && !ReferenceEquals(aModalForm, DxControlHooks.GetModalControl(this)))
                    DxControlHooks.SetModalControl(d, aModalForm);   // 检测双击后 显示模式窗体 会产生重复 MouseDown
                return true;
            }

            if (aModalForm.InRange(x, y))
            {
                DxControlOps.DblClick(aModalForm, x, y);
                return true;
            }
            return false;
        }

        var e = FindActiveControl(x, y);
        if (e != null)
        {
            DxControlOps.DblClick(e, x, y);
            if (DxControlHooks.GetModalControl(this) == null)
                DxControlHooks.SetModalControl(this, ModalForm);     // 检测双击后 显示模式窗体 会产生重复 MouseDown
            return true;
        }
        return false;
    }

    /// <summary>DxControls.pas 1437-1458 TDxControlEngine.KeyDown。</summary>
    public bool PortKeyDown(UShortRef key, TDxShiftState shift)
    {
        if (_activeMenu != null && _activeMenu.Visible)
        {
            DxControlOps.KeyDown(_activeMenu, key, shift);
            return true;
        }

        if (_focusedControl != null)
        {
            DxControlOps.KeyDown(_focusedControl, key, shift);
            return true;
        }

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            DxControlOps.KeyDown(aModalForm, key, shift);
            return true;
        }
        return false;
    }

    /// <summary>DxControls.pas 1460-1480 TDxControlEngine.KeyPress。</summary>
    public bool PortKeyPress(CharRef key)
    {
        if (_activeMenu != null && _activeMenu.Visible)
        {
            DxControlOps.KeyPress(_activeMenu, key);
            return true;
        }

        if (_focusedControl != null)
        {
            DxControlOps.KeyPress(_focusedControl, key);
            return true;
        }

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            DxControlOps.KeyPress(aModalForm, key);
            return true;
        }
        return false;
    }

    /// <summary>DxControls.pas 1482-1503 TDxControlEngine.KeyUp。</summary>
    public bool PortKeyUp(UShortRef key, TDxShiftState shift)
    {
        if (_activeMenu != null && _activeMenu.Visible)
        {
            DxControlOps.KeyUp(_activeMenu, key, shift);
            return true;
        }

        if (_focusedControl != null)
        {
            DxControlOps.KeyUp(_focusedControl, key, shift);
            return true;
        }

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            DxControlOps.KeyUp(aModalForm, key, shift);
            return true;
        }
        return false;
    }

    /// <summary>
    /// DxControls.pas 1505-1587 TDxControlEngine.MouseDown。
    /// 分支优先级：① 双击残留的 ModalControl 消耗 → ② ActiveMenu（内 → 处理；外 → 隐藏）
    /// → ③ ModalForm（内 → FindActiveControl 或窗体本体；外 → 清 MouseDownControl）
    /// → ④ FindActiveControl；否则 OnBackgroundClick / OnMouseDown 由 WantReturn 决定结果。
    /// </summary>
    public bool PortMouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        var engineModal = DxControlHooks.GetModalControl(this);
        if (engineModal != null && !engineModal.Visible)
        {
            // 双击产生的重复 MouseDown
            DxControlHooks.SetModalControl(this, null);
            return true;
        }

        if (_activeMenu != null && _activeMenu.Visible)
        {
            if (_activeMenu.InRange(x, y))
            {
                _activeMenu.MouseDowned = true;
                DxControlOps.SetMousePoint(_activeMenu, x, y);
                DxControlOps.SetFocus(_activeMenu);
                DxControlOps.MouseDown(_activeMenu, button, shift, x, y);
                return true;
            }
            _activeMenu.Visible = false;
        }

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            var d = DxControlOps.FindActiveControl(aModalForm, x, y);
            if (d != null)
            {
                var dm = DxControlHooks.GetModalControl(d);
                if (dm != null && !dm.Visible)
                {
                    // 双击产生的重复 MouseDown
                    DxControlHooks.SetModalControl(d, null);
                    return true;
                }

                d.MouseDowned = true;
                DxControlOps.SetMousePoint(d, x, y);
                DxControlOps.SetFocus(d);
                DxControlOps.MouseDown(d, button, shift, x, y);
                return true;
            }

            if (aModalForm.InRange(x, y))
            {
                aModalForm.MouseDowned = true;
                DxControlOps.SetMousePoint(aModalForm, x, y);
                DxControlOps.SetFocus(aModalForm);
                DxControlOps.MouseDown(aModalForm, button, shift, x, y);
            }
            else
            {
                if (MouseDownControl != null)
                    MouseDownControl.MouseDowned = false;
            }
            return true;
        }

        var e = FindActiveControl(x, y);
        if (e != null)
        {
            e.MouseDowned = true;
            DxControlOps.SetMousePoint(e, x, y);
            DxControlOps.SetFocus(e);
            DxControlOps.MouseDown(e, button, shift, x, y);
            return true;
        }

        bool result = false;
        if (MouseDownControl != null)
            MouseDownControl.MouseDowned = false;

        if (OnBackgroundClick != null)
        {
            WantReturn = false;
            OnBackgroundClick(this);
            if (WantReturn) result = true;
        }

        var onMouseDown = DxControlHooks.GetOnMouseDown(this);
        if (onMouseDown != null)
        {
            WantReturn = false;
            onMouseDown(this, button, shift, x, y);
            if (WantReturn) result = true;
        }
        return result;
    }

    /// <summary>
    /// DxControls.pas 1589-1648 TDxControlEngine.MouseMove。
    /// 原文第一分支：只要 MouseDownControl 非 nil 就直接 `Move`+`MouseMove` 并返回 True
    /// （**不判 InRange**，拖拽中移出也继续收 Move）。
    /// </summary>
    public bool PortMouseMove(TDxShiftState shift, int x, int y)
    {
        if (MouseDownControl != null)
        {
            DxControlOps.Move(MouseDownControl, x, y);
            DxControlOps.MouseMove(MouseDownControl, shift, x, y);
            return true;
        }

        if (_activeMenu != null && _activeMenu.Visible)
        {
            if (_activeMenu.InRange(x, y))
            {
                _activeMenu.MouseMoveed = true;
                DxControlOps.MouseMove(_activeMenu, shift, x, y);
                return true;
            }
        }

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            var d = DxControlOps.FindActiveControl(aModalForm, x, y);
            if (d != null)
            {
                d.MouseMoveed = true;
                DxControlOps.MouseMove(d, shift, x, y);
                return true;
            }

            if (aModalForm.InRange(x, y))
            {
                aModalForm.MouseMoveed = true;
                DxControlOps.MouseMove(aModalForm, shift, x, y);
            }
            else
            {
                if (MouseMoveControl != null)
                    MouseMoveControl.MouseMoveed = false;
            }
            return true;
        }

        var e = FindActiveControl(x, y);
        if (e != null)
        {
            DxControlOps.SetMouseMoveed(e, true);
            DxControlOps.MouseMove(e, shift, x, y);
            return true;
        }

        if (MouseMoveControl != null)
            MouseMoveControl.MouseMoveed = false;

        bool result = false;
        var onMouseMove = DxControlHooks.GetOnMouseMove(this);
        if (onMouseMove != null)
        {
            WantReturn = false;
            onMouseMove(this, shift, x, y);
            if (WantReturn) result = true;
        }
        return result;
    }

    /// <summary>
    /// DxControls.pas 1650-1725 TDxControlEngine.MouseUp。
    /// 原文 1671-1702 的大段注释（旧实现）照抄留档。
    /// 1705-1709 的 `D := MouseDownControl` 在此时**恒为 nil**（上面 1656 分支已处理非 nil 情况）
    /// —— 原文在此会 AV，托管侧按 nil-safe 表达。
    /// </summary>
    public bool PortMouseUp(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        if (MouseDownControl != null)
        {
            var captured = MouseDownControl;
            if (captured.InRange(x, y))
            {
                // 原文 1658-1659（被注释掉）：
                //   // MouseDownControl.MouseUp(Button, Shift, X, Y);
                //   // MouseDownControl.MouseDowned := False;
                captured.MouseDowned = false;
                DxControlOps.MouseUp(captured, button, shift, x, y);
            }
            else
            {
                captured.MouseDowned = false;
            }
            return true;
        }

        // 原文 1671-1702（整段被注释掉的旧实现，原文如此）：
        //   if (ActiveMenu <> nil) and ActiveMenu.Visible then begin
        //     if ActiveMenu.InRange(X, Y) and ActiveMenu.MouseDowned then ActiveMenu.MouseUp(Button, Shift, X, Y);
        //     Result := True; Exit;
        //   end;
        //   if MouseDownControl <> nil then begin
        //     if MouseDownControl.InRange(X, Y) then MouseDownControl.MouseUp(Button, Shift, X, Y)
        //     else MouseDownControl.MouseDowned := False;
        //     Result := True; Exit;
        //   end;
        //   if (ModalForm <> nil) and ModalForm.Visible then begin
        //     if ModalForm.InRange(X, Y) and ModalForm.MouseDowned then ModalForm.MouseUp(Button, Shift, X, Y);
        //     Result := True; Exit;
        //   end;

        var aModalForm = ModalForm;
        if (aModalForm != null && aModalForm.Visible)
        {
            if (aModalForm.InRange(x, y) && aModalForm.MouseDowned)
            {
                var d = MouseDownControl;                  // 原文如此：此处恒为 nil
                if (MouseDownControl != null)
                    MouseDownControl.MouseDowned = false;
                if (d != null) DxControlOps.MouseUp(d, button, shift, x, y);
            }
            return true;
        }

        bool result = false;

        var onMouseUp = DxControlHooks.GetOnMouseUp(this);
        if (onMouseUp != null)
        {
            WantReturn = false;
            onMouseUp(this, button, shift, x, y);
            if (WantReturn) result = true;
        }

        var onClick = DxControlHooks.GetOnClick(this);
        if (onClick != null)
        {
            WantReturn = false;
            onClick(this, x, y);
            if (WantReturn) result = true;
        }

        return result;
    }

    /// <summary>DxControls.pas 1727-1734 MouseWheelDown：只有 `FScrollControl is TDxScrollControl` 才转发。</summary>
    public bool PortMouseWheelDown(TDxShiftState shift, TDxPoint mousePos)
    {
        if (_scrollControl is TDxScrollControl sc)
        {
            sc.MouseWheelDown(shift, mousePos);
            return true;
        }
        return false;
    }

    /// <summary>DxControls.pas 1736-1743 MouseWheelUp。</summary>
    public bool PortMouseWheelUp(TDxShiftState shift, TDxPoint mousePos)
    {
        if (_scrollControl is TDxScrollControl sc)
        {
            sc.MouseWheelUp(shift, mousePos);
            return true;
        }
        return false;
    }

    // ---- 与上游 TDxControl 的三个 virtual 对齐（结果存 Last*Result）----

    /// <summary>原文 TDxControlEngine.MouseDown 的返回值。</summary>
    public bool LastMouseDownResult { get; private set; }

    /// <summary>原文 TDxControlEngine.MouseMove 的返回值。</summary>
    public bool LastMouseMoveResult { get; private set; }

    /// <summary>原文 TDxControlEngine.MouseUp 的返回值。</summary>
    public bool LastMouseUpResult { get; private set; }

    /// <summary>转发到 PortMouseDown（结果见 LastMouseDownResult）。</summary>
    public override void MouseDown(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        LastMouseDownResult = PortMouseDown(button, shift, x, y);
    }

    /// <summary>转发到 PortMouseMove（结果见 LastMouseMoveResult）。</summary>
    public override void MouseMove(TDxShiftState shift, int x, int y)
    {
        LastMouseMoveResult = PortMouseMove(shift, x, y);
    }

    /// <summary>转发到 PortMouseUp（结果见 LastMouseUpResult）。</summary>
    public override void MouseUp(TDxMouseButton button, TDxShiftState shift, int x, int y)
    {
        LastMouseUpResult = PortMouseUp(button, shift, x, y);
    }
}
