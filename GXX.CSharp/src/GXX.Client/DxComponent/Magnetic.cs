using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

// =====================================================================================
// Magnetic.pas（851 行）移植。
//
//   * 147-153   TWND_INFO（h_wnd / hWndParent / Glue）
//   * 155-156   TSubClass_Proc（Win32 子类化回调签名）
//   * 158-190   TMagnetic 声明（FSnapWidth / m_uWndInfo / m_rcWnd / m_lWndCount / 4 个光标点）
//   * 192-196   LB_RECT = 16、单元级全局 MagneticWnd
//   * 199-851   实现段
//
// -------------------------------------------------------------------------------------
// 托管侧落点与偏差（逐条登记）：
//
//   1. **HWND → int**（`TDxMagneticHandle` 只是 `int` 的类型别名，便于阅读）。
//      Win32 的 `IsWindow` / `IsWindowVisible` / `GetWindowRect` / `GetCursorPos` /
//      `BeginDeferWindowPos` / `DeferWindowPos` / `OffsetRect` 全部走接缝委托：
//        * `WindowValidator`   —— 原文 372 `IsWindow(Handle) And (IsWindow(hWndParent) Or hWndParent=0)`
//        * `WindowVisibility`  —— 原文 462/561/623/639/707/748/766/774 `IsWindowVisible`
//        * `WindowRectProvider`—— 原文 741 `GetWindowRect(...)`（写进 m_rcWnd[lc1]）
//        * `CursorPosProvider` —— 原文 546 `GetCursorPos(m_ptCurr)`
//        * `ChildMover`        —— 原文 702-726 的 DeferWindowPos 批量搬子窗口
//      缺省实现：验证恒 true、可见恒 true、矩形返回 Zero、光标恒 (0,0)、搬移只记录。
//      这样 `pvWndsConnected` / `pvCheckGlueing` / magnetism 计算**全部可 headless 单测**。
//
//   2. 原文数组是 **1-based**（`array of TWND_INFO` 用 `[1..m_lWndCount]`，
//      且 `m_rcWnd[0]` 被当作"桌面区域"）。托管侧用 `List<T>` + **在索引 0 放哨兵**，
//      使 `m_uWndInfo[1..n]` / `m_rcWnd[0..n]` 的原索引语义**一字不改**地保留。
//      原文 `AddWindow` 里 `SetLength(m_uWndInfo, m_lWndCount+1)` 正是这个 1-based + 哨兵布局。
//
//   3. `pvMoveRect` 里 `GetCursorPos` 之后用 `m_ptAnchor`/`m_ptOffset` 算位移；
//      本波把 **光标位置** 与 **锚点/偏移** 都做成可注入字段，故纯逻辑可测。
//      `pvMoveRect` 的 `DeferWindowPos` 段（702-726）走 `ChildMover` 接缝，其余全量落地。
//
//   4. `zSubclass_Proc`（248-355）是 Win32 窗口过程（WM_MOVING/WM_SIZING/WM_ENTERSIZEMOVE 等），
//      **本波不移植**：它依赖 `TMessage`/`WndProc` 链，属未移植的 Forms 域。
//      本波只把它的两个被调者（`pvMoveRect` / `pvSizeRect`）与 `pvCheckGlueing` 落地。
// =====================================================================================

/// <summary>原文 `HWND` 的托管别名（Win32 窗口句柄）。</summary>
public readonly struct TDxMagneticHandle : IEquatable<TDxMagneticHandle>
{
    /// <summary>句柄值（0 = 无窗口，对应原文 `hWndParent = 0` 的"无父"）。</summary>
    public readonly int Value;

    public TDxMagneticHandle(int value) { Value = value; }

    /// <summary>原文的 HWND 0（无父窗口哨兵）。</summary>
    public static readonly TDxMagneticHandle Null = new(0);

    public bool Equals(TDxMagneticHandle other) => Value == other.Value;
    public override bool Equals(object obj) => obj is TDxMagneticHandle h && Equals(h);
    public override int GetHashCode() => Value;
    public override string ToString() => Value == 0 ? "0" : $"0x{Value:X}";
}

/// <summary>Magnetic.pas 148-153 TWND_INFO：一个被磁吸管理的窗口。</summary>
public sealed class TDxWndInfo
{
    /// <summary>原文 h_wnd。</summary>
    public TDxMagneticHandle Handle;

    /// <summary>原文 hWndParent（0 = 无父；AddWindow 里"父=自己"会被归一成 0）。</summary>
    public TDxMagneticHandle ParentHandle;

    /// <summary>原文 Glue（是否与父窗口保持吸附）。</summary>
    public bool Glue;
}

/// <summary>Magnetic.pas 155-156 TSubClass_Proc（Win32 子类化回调）。</summary>
public delegate bool TDxSubClassProc(TDxMagneticHandle hWnd, int uMsg, ref int wParam, ref int lParam,
    ref int lReturn, ref bool handled);

/// <summary>
/// Magnetic.pas 158-190 / 199-851 TMagnetic：
/// 让一组窗口在拖动/缩放时互相"磁吸"到边缘，并在吸附后把子窗口一起搬走。
///
/// 已 1:1 落地的纯逻辑：
///   * 构造 / 析构（216-235）
///   * SnapWidth 属性（238-246，默认 10）
///   * `pvWndsConnected`（792-814）：并集矩形包围判定 + 8 条边重合判定
///   * `pvWndGetInfoIndex` / `pvWndParentGetInfoIndex`（816-847）
///   * `AddWindow`（356-396）/ `RemoveWindow`（399-437）
///   * `pvCheckGlueing`（731-790）：直接连接 + 多层间接连接传播
///   * `pvSizeRect`（446-532）：按 6 个缩放边（WMSZ_*）做 X/Y 单向吸附
///   * `pvMoveRect`（534-729）：四个锚点组合各自覆盖 lOffx/lOffy（**后判者胜**），
///     再对子窗口做同样的吸附，最后统一 OffsetRect + 搬子窗口
/// =====================================================================================
public class TDxMagnetic
{
    // 原文 471-527 的缩放边常量（Windows WMSZ_*）
    /// <summary>Windows WMSZ_LEFT = 1。</summary>
    public const int WMSZ_LEFT = 1;
    /// <summary>Windows WMSZ_RIGHT = 2。</summary>
    public const int WMSZ_RIGHT = 2;
    /// <summary>Windows WMSZ_TOP = 3。</summary>
    public const int WMSZ_TOP = 3;
    /// <summary>Windows WMSZ_TOPLEFT = 4。</summary>
    public const int WMSZ_TOPLEFT = 4;
    /// <summary>Windows WMSZ_TOPRIGHT = 5。</summary>
    public const int WMSZ_TOPRIGHT = 5;
    /// <summary>Windows WMSZ_BOTTOM = 6。</summary>
    public const int WMSZ_BOTTOM = 6;
    /// <summary>Windows WMSZ_BOTTOMLEFT = 7。</summary>
    public const int WMSZ_BOTTOMLEFT = 7;
    /// <summary>Windows WMSZ_BOTTOMRIGHT = 8。</summary>
    public const int WMSZ_BOTTOMRIGHT = 8;

    /// <summary>原文 193 `LB_RECT = 16`（TRECT 的字节长；托管侧的 CopyMemory 已换成结构赋值）。</summary>
    public const int LB_RECT = 16;

    private int _snapWidth;

    // 原文 m_uWndInfo: array of TWND_INFO / m_rcWnd: array of TRECT，**1-based + 索引 0 哨兵**
    // （原文 `m_rcWnd[0] has the window rect of Desktop area`，见 461/560 注释）
    private readonly List<TDxWndInfo> _wndInfo = new() { null };      // [0] 哨兵
    private readonly List<TDxRect> _rcWnd = new() { TDxRect.Empty };  // [0] = 桌面区域

    /// <summary>原文 m_lWndCount（已注册窗口数；不含索引 0 的哨兵）。</summary>
    public int WndCount => _wndInfo.Count > 0 ? _wndInfo.Count - 1 : 0;

    /// <summary>原文 m_ptAnchor（拖拽锚点）。</summary>
    public TDxPoint AnchorPoint;

    /// <summary>原文 m_ptOffset（拖拽偏移）。</summary>
    public TDxPoint OffsetPoint;

    /// <summary>原文 m_ptCurr（当前光标位置；由 CursorPosProvider 或直接写）。</summary>
    public TDxPoint CurrentPoint;

    /// <summary>原文 m_ptLast（上一次光标位置）。</summary>
    public TDxPoint LastPoint;

    // ---- 接缝（Win32）----

    /// <summary>原文 372 `IsWindow(Handle) And (IsWindow(hWndParent) Or (hWndParent = 0))`。缺省恒 true。</summary>
    public Func<TDxMagneticHandle, TDxMagneticHandle, bool> WindowValidator;

    /// <summary>原文 `IsWindowVisible(...)`。缺省恒 true。</summary>
    public Func<TDxMagneticHandle, bool> WindowVisibility;

    /// <summary>原文 741 `GetWindowRect(hwnd, rect)`。缺省从注册表里已存的矩形回读（即"不动"）。</summary>
    public Func<TDxMagneticHandle, TDxRect> WindowRectProvider;

    /// <summary>原文 546 `GetCursorPos(m_ptCurr)`。缺省返回 CurrentPoint（便于 headless 驱动）。</summary>
    public Func<TDxPoint> CursorPosProvider;

    /// <summary>
    /// 原文 702-726 的 `BeginDeferWindowPos`/`DeferWindowPos` 批量搬子窗口。
    /// 参数：(子窗口句柄, 新 x, 新 y)。缺省只记录到 <see cref="MovedChildren"/>。
    /// </summary>
    public Action<TDxMagneticHandle, int, int> ChildMover;

    /// <summary>缺省 ChildMover 的记录落点（测试可断言）。</summary>
    public readonly List<(TDxMagneticHandle handle, int x, int y)> MovedChildren = new();

    /// <summary>原文 195-196 `Var MagneticWnd: TMagnetic` 的单例落点。</summary>
    public static TDxMagnetic MagneticWnd;

    /// <summary>原文 216-224 TMagnetic.create：`SnapWidth := 10; m_lWndCount := 0;`</summary>
    public TDxMagnetic()
    {
        _snapWidth = 10;
        // m_lWndCount := 0（由 _wndInfo 的哨兵布局天然表达）
    }

    /// <summary>原文 227-235 Destroy（把单元级全局清空并缩短数组）。</summary>
    public void Destroy()
    {
        if (ReferenceEquals(MagneticWnd, this)) MagneticWnd = null;
        _wndInfo.Clear();
        _rcWnd.Clear();
    }

    /// <summary>原文 238-241 GetSnapWidth。</summary>
    public int SnapWidth
    {
        get => _snapWidth;
        set => _snapWidth = value;      // 原文 243-246 setter 是裸赋值（无范围校验）
    }

    // ---- 注册表访问（原文的 1-based 数组）----

    /// <summary>原文 `m_uWndInfo[i]`（i 为 1-based；越界返回 null）。</summary>
    public TDxWndInfo GetWndInfo(int index)
        => (index >= 1 && index <= WndCount) ? _wndInfo[index] : null;

    /// <summary>原文 `m_rcWnd[i]`（i 为 0-based；索引 0 是桌面哨兵）。</summary>
    public TDxRect GetWndRect(int index)
        => (index >= 0 && index < _rcWnd.Count) ? _rcWnd[index] : TDxRect.Empty;

    /// <summary>原文 `m_rcWnd[i] := value`（供接缝与测试设置窗口矩形）。</summary>
    public void SetWndRect(int index, TDxRect value)
    {
        while (_rcWnd.Count <= index) _rcWnd.Add(TDxRect.Empty);
        _rcWnd[index] = value;
    }

    private bool IsVisible(TDxMagneticHandle handle)
        => WindowVisibility == null || WindowVisibility(handle);

    // ---- 原文 356-396 AddWindow ----

    /// <summary>
    /// 原文 356-396 AddWindow 1:1：
    /// ① 已在表中 → 返回 false（原文 `Exit` 时 Result 仍是初始的 false）；
    /// ② `IsWindow(Handle) and (IsWindow(hWndParent) or hWndParent = 0)` 才继续；
    /// ③ `Inc(m_lWndCount)` + `SetLength(..., m_lWndCount+1)`（1-based + 哨兵）；
    /// ④ **`if hWndParent = Handle then 父 := 0`**（原文 383-386 的 "Parent window is Self window ?"）；
    /// ⑤ 立刻 `pvCheckGlueing`，返回 true。
    /// 原文的出参 `FuncPointer := Subclass_Proc` 在托管侧由调用方自行接
    /// <see cref="TDxSubClassProc"/>（本波不移植 zSubclass_Proc，见文件头第 4 条）。
    /// </summary>
    public bool AddWindow(TDxMagneticHandle handle, TDxMagneticHandle parentHandle)
    {
        for (int lc = 1; lc <= WndCount; lc++)
        {
            if (_wndInfo[lc].Handle.Equals(handle)) return false;
        }

        if (!(WindowValidator == null || WindowValidator(handle, parentHandle))) return false;

        _wndInfo.Add(new TDxWndInfo());
        _rcWnd.Add(TDxRect.Empty);

        var info = _wndInfo[WndCount];
        info.Handle = handle;
        if (parentHandle.Equals(handle))
            info.ParentHandle = TDxMagneticHandle.Null;
        else
            info.ParentHandle = parentHandle;

        PvCheckGlueing();
        return true;
    }

    /// <summary>
    /// 原文 399-437 RemoveWindow 1:1：
    /// ① 找到句柄 → 把后面的项**依次前移**（`m_uWndInfo[lc2] := m_uWndInfo[lc2+1]`，覆盖式搬移）；
    /// ② `Dec(m_lWndCount)` + 缩短两个数组；
    /// ③ 把所有 `hWndParent = Handle` 的项改成 0（解除父子关系）；
    /// ④ `pvCheckGlueing`，返回 true。
    /// </summary>
    public bool RemoveWindow(TDxMagneticHandle handle)
    {
        for (int lc1 = 1; lc1 <= WndCount; lc1++)
        {
            if (!_wndInfo[lc1].Handle.Equals(handle)) continue;

            for (int lc2 = lc1; lc2 <= WndCount - 1; lc2++)
            {
                _wndInfo[lc2] = _wndInfo[lc2 + 1];
                _rcWnd[lc2] = _rcWnd[lc2 + 1];
            }

            _wndInfo.RemoveAt(WndCount);
            _rcWnd.RemoveAt(_rcWnd.Count - 1);

            for (int lc2 = 1; lc2 <= WndCount; lc2++)
            {
                if (_wndInfo[lc2].ParentHandle.Equals(handle))
                    _wndInfo[lc2].ParentHandle = TDxMagneticHandle.Null;
            }

            PvCheckGlueing();
            return true;
        }
        return false;
    }

    /// <summary>原文 439-443 CheckGlueing（转调 pvCheckGlueing）。</summary>
    public void CheckGlueing() => PvCheckGlueing();

    // ---- 原文 792-814 pvWndsConnected ----

    /// <summary>
    /// Magnetic.pas 792-814 pvWndsConnected 1:1（纯函数）。
    /// `UnionRect(rcUnion, rcWnd1, rcWnd2)` 后：
    ///   ① 包围判定：`(并集宽 &lt;= 两窗宽之和) and (并集高 &lt;= 两窗高之和)`；
    ///   ② 8 条边重合判定（4 条左/右 × 4 条上/下 的**任何一种相等**）—— 注意原文把
    ///      `Left=Left` / `Right=Right` / `Top=Top` / `Bottom=Bottom` 也算作重合（即两窗同位置也算）。
    /// 两条都成立才返回 True。
    /// </summary>
    public static bool PvWndsConnected(TDxRect rcWnd1, TDxRect rcWnd2)
    {
        // UnionRect：原文是 Windows.UnionRect（min Left/Top、max Right/Bottom）
        var rcUnion = new TDxRect(
            Math.Min(rcWnd1.Left, rcWnd2.Left),
            Math.Min(rcWnd1.Top, rcWnd2.Top),
            Math.Max(rcWnd1.Right, rcWnd2.Right),
            Math.Max(rcWnd1.Bottom, rcWnd2.Bottom));

        if ((rcUnion.Right - rcUnion.Left) <= (rcWnd1.Right - rcWnd1.Left) + (rcWnd2.Right - rcWnd2.Left)
            && (rcUnion.Bottom - rcUnion.Top) <= (rcWnd1.Bottom - rcWnd1.Top) + (rcWnd2.Bottom - rcWnd2.Top))
        {
            if (rcWnd1.Left == rcWnd2.Left || rcWnd1.Left == rcWnd2.Right
                || rcWnd1.Right == rcWnd2.Left || rcWnd1.Right == rcWnd2.Right
                || rcWnd1.Top == rcWnd2.Top || rcWnd1.Top == rcWnd2.Bottom
                || rcWnd1.Bottom == rcWnd2.Top || rcWnd1.Bottom == rcWnd2.Bottom)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>原文 816-831 pvWndGetInfoIndex（找不到返回 -1）。</summary>
    public int PvWndGetInfoIndex(TDxMagneticHandle handle)
    {
        for (int lc = 1; lc <= WndCount; lc++)
        {
            if (_wndInfo[lc].Handle.Equals(handle)) return lc;
        }
        return -1;
    }

    /// <summary>
    /// 原文 833-847 pvWndParentGetInfoIndex：**注意原文比较的是 `h_wnd = hWndParent`**
    /// （拿"父句柄"去当"某个窗口自己的句柄"找），名字虽叫 Parent 但查的就是 h_wnd —— 逐字照抄。
    /// </summary>
    public int PvWndParentGetInfoIndex(TDxMagneticHandle parentHandle)
    {
        for (int lc = 1; lc <= WndCount; lc++)
        {
            if (_wndInfo[lc].Handle.Equals(parentHandle)) return lc;
        }
        return -1;
    }

    // ---- 原文 731-790 pvCheckGlueing ----

    /// <summary>
    /// Magnetic.pas 731-790 pvCheckGlueing 1:1：
    /// ① 重置：逐窗 `GetWindowRect` 刷新 m_rcWnd 并把 Glue 置 False（741-742）；
    /// ② **直接连接**：对有父的窗口，取父的 info 索引 `lWId`，则
    ///    `Glue := pvWndsConnected(m_rcWnd[lWId], m_rcWnd[lc1])`
    ///    —— **注意原文不判 `lWId` 是否 -1**（父不在表里时会用 `m_rcWnd[-1]`，Delphi 下是未定义内存；
    ///    托管侧加 -1 守卫，等价于"该窗口不吸附"）；
    /// ③ **间接连接**（多层吸附传播）：`m_lWndCount` 轮 × 逐窗 `lc1`：
    ///    若 `lc1.Glue` 为真，则对每个可见的 `lc2 <> lc1`，
    ///    若两者 **hWndParent 相同** 且 `lc2.Glue = False`，
    ///    则 `lc2.Glue := pvWndsConnected(m_rcWnd[lc1], m_rcWnd[lc2])`。
    /// </summary>
    public void PvCheckGlueing()
    {
        // ① 刷新矩形 + 重置 Glue
        for (int lc1 = 1; lc1 <= WndCount; lc1++)
        {
            var rect = WindowRectProvider != null
                ? WindowRectProvider(_wndInfo[lc1].Handle)
                : GetWndRect(lc1);
            SetWndRect(lc1, rect);
            _wndInfo[lc1].Glue = false;
        }

        // ② 直接连接
        for (int lc1 = 1; lc1 <= WndCount; lc1++)
        {
            if (!IsVisible(_wndInfo[lc1].Handle)) continue;

            if (!_wndInfo[lc1].ParentHandle.Equals(TDxMagneticHandle.Null))
            {
                int lWId = PvWndParentGetInfoIndex(_wndInfo[lc1].ParentHandle);
                if (lWId >= 0)     // 原文未判（会用 m_rcWnd[-1]）；托管侧守卫
                {
                    _wndInfo[lc1].Glue = PvWndsConnected(GetWndRect(lWId), GetWndRect(lc1));
                }
            }
        }

        // ③ 间接连接（多层传播）
        for (int lcMain = 1; lcMain <= WndCount; lcMain++)
        {
            for (int lc1 = 1; lc1 <= WndCount; lc1++)
            {
                if (!IsVisible(_wndInfo[lc1].Handle)) continue;

                if (_wndInfo[lc1].Glue)
                {
                    for (int lc2 = 1; lc2 <= WndCount; lc2++)
                    {
                        if (!IsVisible(_wndInfo[lc2].Handle)) continue;
                        if (lc1 == lc2) continue;

                        if (_wndInfo[lc1].ParentHandle.Equals(_wndInfo[lc2].ParentHandle))
                        {
                            if (!_wndInfo[lc2].Glue)
                            {
                                _wndInfo[lc2].Glue =
                                    PvWndsConnected(GetWndRect(lc1), GetWndRect(lc2));
                            }
                        }
                    }
                }
            }
        }
    }

    // ---- 原文 446-532 pvSizeRect ----

    /// <summary>
    /// Magnetic.pas 446-532 pvSizeRect 1:1（纯几何，返回吸附后的矩形）。
    /// `rcTmp` 是**进入时的原矩形快照**（原文 453 的 CopyMemory），后续所有判定都拿它比对；
    /// 循环 `lc := 0 to m_lWndCount`（**含索引 0 的桌面哨兵**），跳过当前窗口，
    /// X 吸附只在 `lfEdge` 属于 {LEFT, TOPLEFT, BOTTOMLEFT}（改 Left）或
    /// {RIGHT, TOPRIGHT, BOTTOMRIGHT}（改 Right）时生效，
    /// 且要求 `rcWnd.Top &lt; 该窗.Bottom + SnapWidth and rcWnd.Bottom &gt; 该窗.Top - SnapWidth`；
    /// Y 吸附同理（{TOP,TOPLEFT,TOPRIGHT} 改 Top，{BOTTOM,BOTTOMLEFT,BOTTOMRIGHT} 改 Bottom）。
    /// 每个方向内是两个**同字段的连续 case**（原文 476-483 / 487-494 / 506-513 / 518-525），
    /// 后一个命中会**覆盖**前一个 —— 逐字保留该"后判者胜"语义。
    /// </summary>
    public TDxRect PvSizeRect(TDxMagneticHandle handle, TDxRect rcWnd, int lfEdge)
    {
        var rcTmp = rcWnd;      // 原文 453 CopyMemory(@rcTmp, @rcWnd, LB_RECT)
        var result = rcWnd;

        for (int lc = 0; lc <= WndCount; lc++)
        {
            var target = GetWndRect(lc);

            // 原文 461-463：索引 0 是桌面哨兵，不查可见性
            if (lc != 0)
            {
                var info = GetWndInfo(lc);
                if (info != null && !IsVisible(info.Handle)) continue;
            }

            var cur = GetWndInfo(lc);
            if (cur != null && cur.Handle.Equals(handle)) continue;    // 原文 466

            // X magnetism
            if (result.Top < target.Bottom + _snapWidth && result.Bottom > target.Top - _snapWidth)
            {
                switch (lfEdge)
                {
                    case WMSZ_LEFT:
                    case WMSZ_TOPLEFT:
                    case WMSZ_BOTTOMLEFT:
                        if (Math.Abs(rcTmp.Left - target.Left) < _snapWidth) result.Left = target.Left;
                        if (Math.Abs(rcTmp.Left - target.Right) < _snapWidth) result.Left = target.Right;
                        break;

                    case WMSZ_RIGHT:
                    case WMSZ_TOPRIGHT:
                    case WMSZ_BOTTOMRIGHT:
                        if (Math.Abs(rcTmp.Right - target.Left) < _snapWidth) result.Right = target.Left;
                        if (Math.Abs(rcTmp.Right - target.Right) < _snapWidth) result.Right = target.Right;
                        break;
                }
            }

            // Y magnetism
            if (result.Left < target.Right + _snapWidth && result.Right > target.Left - _snapWidth)
            {
                switch (lfEdge)
                {
                    case WMSZ_TOP:
                    case WMSZ_TOPLEFT:
                    case WMSZ_TOPRIGHT:
                        if (Math.Abs(rcTmp.Top - target.Top) < _snapWidth) result.Top = target.Top;
                        if (Math.Abs(rcTmp.Top - target.Bottom) < _snapWidth) result.Top = target.Bottom;
                        break;

                    case WMSZ_BOTTOM:
                    case WMSZ_BOTTOMLEFT:
                    case WMSZ_BOTTOMRIGHT:
                        if (Math.Abs(rcTmp.Bottom - target.Top) < _snapWidth) result.Bottom = target.Top;
                        if (Math.Abs(rcTmp.Bottom - target.Bottom) < _snapWidth) result.Bottom = target.Bottom;
                        break;
                }
            }
        }
        return result;
    }

    // ---- 原文 534-729 pvMoveRect ----

    /// <summary>
    /// Magnetic.pas 534-729 pvMoveRect 的**计算部分** 1:1（返回吸附后的矩形，
    /// 并把子窗口搬移记进 <see cref="MovedChildren"/> 或交给 <see cref="ChildMover"/>）。
    ///
    /// 语义：
    ///   ① `GetCursorPos(m_ptCurr)`；
    ///   ② 先把 rcWnd 平移到光标处：`OffsetRect(rcWnd, (curr.x - rcWnd.Left) + offset.x, 0)`
    ///      再 `OffsetRect(rcWnd, 0, (curr.y - rcWnd.Top) + offset.y)`（**X 先于 Y**，顺序不可换）；
    ///   ③ 第一轮 `lc1 := 0 to m_lWndCount`（含桌面哨兵）：跳过当前窗口、跳过隐藏窗口、
    ///      跳过"已吸附且父是当前窗口"的子窗口；
    ///      对每个候选窗，X 方向 4 个命中组合（Left-Left / Left-Right / Right-Left / Right-Right）
    ///      依次覆写 `lOffx`，Y 方向同理覆写 `lOffy` —— **后判者胜**；
    ///   ④ 第二轮 `lc1 := 1 to m_lWndCount`：对"已吸附且父是当前窗口"的子窗口，
    ///      先按 `curr - anchor` 平移它的矩形副本，再对每个 `lc2`（跳过自己、跳过隐藏、
    ///      跳过"已吸附且句柄≠当前窗口"的）做同样的 4+4 覆写，**仍然写同一对 lOffx/lOffy**（原文如此）；
    ///   ⑤ `OffsetRect(rcWnd, lOffx, lOffy)`；
    ///   ⑥ 搬子窗口：对每个"父=当前窗口 且 Glue"的可见窗口，
    ///      `newLeft = 子.Left - (当前窗口.旧Left - rcWnd.Left)`、`newTop` 同理。
    /// </summary>
    public TDxRect PvMoveRect(TDxMagneticHandle handle, TDxRect rcWnd)
    {
        // ① 当前光标位置
        CurrentPoint = CursorPosProvider != null ? CursorPosProvider() : CurrentPoint;

        var result = rcWnd;

        // ② 先平移到光标处（X 先于 Y）
        OffsetRect(ref result, (CurrentPoint.X - result.Left) + OffsetPoint.X, 0);
        OffsetRect(ref result, 0, (CurrentPoint.Y - result.Top) + OffsetPoint.Y);

        int lOffx = 0;
        int lOffy = 0;

        // ③ 第一轮：对非子窗口的吸附
        for (int lc1 = 0; lc1 <= WndCount; lc1++)
        {
            if (lc1 != 0)
            {
                var info1 = GetWndInfo(lc1);
                if (info1 != null && !IsVisible(info1.Handle)) continue;
            }

            var cur1 = GetWndInfo(lc1);
            if (cur1 != null && cur1.Handle.Equals(handle)) continue;

            // 原文 568-569：跳过"已吸附且父就是当前窗口"的子窗口
            if (cur1 != null && cur1.Glue && cur1.ParentHandle.Equals(handle)) continue;

            var target = GetWndRect(lc1);

            // X magnetism（四个组合依次覆写）
            if (result.Top < target.Bottom + _snapWidth && result.Bottom > target.Top - _snapWidth)
            {
                if (Math.Abs(result.Left - target.Left) < _snapWidth) lOffx = target.Left - result.Left;
                if (Math.Abs(result.Left - target.Right) < _snapWidth) lOffx = target.Right - result.Left;
                if (Math.Abs(result.Right - target.Left) < _snapWidth) lOffx = target.Left - result.Right;
                if (Math.Abs(result.Right - target.Right) < _snapWidth) lOffx = target.Right - result.Right;
            }

            // Y magnetism
            if (result.Left < target.Right + _snapWidth && result.Right > target.Left - _snapWidth)
            {
                if (Math.Abs(result.Top - target.Top) < _snapWidth) lOffy = target.Top - result.Top;
                if (Math.Abs(result.Top - target.Bottom) < _snapWidth) lOffy = target.Bottom - result.Top;
                if (Math.Abs(result.Bottom - target.Top) < _snapWidth) lOffy = target.Top - result.Bottom;
                if (Math.Abs(result.Bottom - target.Bottom) < _snapWidth) lOffy = target.Bottom - result.Bottom;
            }

        }

        // ④ 第二轮：对"已吸附的子窗口"再算一遍（仍然覆写同一对 lOffx/lOffy）
        for (int lc1 = 1; lc1 <= WndCount; lc1++)
        {
            var child = GetWndInfo(lc1);
            if (child == null || !IsVisible(child.Handle)) continue;
            if (!(child.Glue && child.ParentHandle.Equals(handle))) continue;

            var rcTmp = GetWndRect(lc1);
            OffsetRect(ref rcTmp, CurrentPoint.X - AnchorPoint.X, 0);
            OffsetRect(ref rcTmp, 0, CurrentPoint.Y - AnchorPoint.Y);

            for (int lc2 = 0; lc2 <= WndCount; lc2++)
            {
                if (lc1 == lc2) continue;

                var other = GetWndInfo(lc2);
                if (other != null && !IsVisible(other.Handle)) continue;

                // 原文 643-644：跳过"已吸附 且 句柄≠当前窗口"的
                if (other != null && !other.Glue && !other.Handle.Equals(handle))
                {
                    var target = GetWndRect(lc2);

                    if (rcTmp.Top < target.Bottom + _snapWidth && rcTmp.Bottom > target.Top - _snapWidth)
                    {
                        if (Math.Abs(rcTmp.Left - target.Left) < _snapWidth) lOffx = target.Left - rcTmp.Left;
                        if (Math.Abs(rcTmp.Left - target.Right) < _snapWidth) lOffx = target.Right - rcTmp.Left;
                        if (Math.Abs(rcTmp.Right - target.Left) < _snapWidth) lOffx = target.Left - rcTmp.Right;
                        if (Math.Abs(rcTmp.Right - target.Right) < _snapWidth) lOffx = target.Right - rcTmp.Right;
                    }

                    if (rcTmp.Left < target.Right + _snapWidth && rcTmp.Right > target.Left - _snapWidth)
                    {
                        if (Math.Abs(rcTmp.Top - target.Top) < _snapWidth) lOffy = target.Top - rcTmp.Top;
                        if (Math.Abs(rcTmp.Top - target.Bottom) < _snapWidth) lOffy = target.Bottom - rcTmp.Top;
                        if (Math.Abs(rcTmp.Bottom - target.Top) < _snapWidth) lOffy = target.Top - rcTmp.Bottom;
                        if (Math.Abs(rcTmp.Bottom - target.Bottom) < _snapWidth) lOffy = target.Bottom - rcTmp.Bottom;
                    }
                }
            }
        }

        // ⑤ 应用偏移
        var beforeOffset = result;
        OffsetRect(ref result, lOffx, lOffy);

        // ⑥ 搬子窗口（原文 702-726 的 DeferWindowPos）
        for (int lc1 = 1; lc1 <= WndCount; lc1++)
        {
            var child = GetWndInfo(lc1);
            if (child == null || !IsVisible(child.Handle)) continue;
            if (!(child.ParentHandle.Equals(handle) && child.Glue)) continue;

            int lWId = PvWndGetInfoIndex(handle);
            if (lWId < 0) continue;        // 原文未判（会用 m_rcWnd[-1]）；托管侧守卫

            var childRect = GetWndRect(lc1);
            var selfOld = GetWndRect(lWId);

            int nx = childRect.Left - (selfOld.Left - result.Left);
            int ny = childRect.Top - (selfOld.Top - result.Top);

            if (ChildMover != null) ChildMover(child.Handle, nx, ny);
            else MovedChildren.Add((child.Handle, nx, ny));
        }

        LastPoint = CurrentPoint;
        _ = beforeOffset;    // 原文 727-728 只存光标；此处保留局部名以对照原文行号
        return result;
    }

    /// <summary>Windows.OffsetRect（就地平移）。</summary>
    private static void OffsetRect(ref TDxRect rect, int dx, int dy)
    {
        rect.Left += dx;
        rect.Right += dx;
        rect.Top += dy;
        rect.Bottom += dy;
    }
}