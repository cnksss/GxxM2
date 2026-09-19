using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// Magnetic.pas（851 行）的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/Magnetic.cs）：
//   * 构造默认值（SnapWidth = 10 / 窗口数 0 / 四个光标点）
//   * pvWndsConnected（并集包围判定 + 8 条边重合判定的**交叉表**）
//   * pvWndGetInfoIndex / pvWndParentGetInfoIndex
//   * AddWindow（重复拒绝 / 验证失败拒绝 / **父=自己归一成 0** / 成功后立刻 CheckGlueing）
//   * RemoveWindow（覆盖式前移 / 解除父子关系 / 未找到返回 false）
//   * pvCheckGlueing（直接连接 + 多层间接传播）
//   * pvSizeRect（6 个 WMSZ_* 边 → 改 Left/Right/Top/Bottom；以及"不吸附"的三种情形）
//   * pvMoveRect（四锚点覆写"后判者胜" / 平移 / 子窗口搬移）
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlMagneticTests
{
    private static TDxMagneticHandle H(int v) => new(v);

    private static TDxRect R(int l, int t, int r, int b) => TDxRect.Rect(l, t, r, b);

    /// <summary>构造一个带 3 个窗口（含索引 0 桌面哨兵）的磁场，并把矩形预置好。</summary>
    private static TDxMagnetic Make(params (int handle, int parent, TDxRect rect)[] windows)
    {
        var m = new TDxMagnetic();
        foreach (var (handle, parent, rect) in windows)
        {
            m.AddWindow(H(handle), H(parent));
        }
        // AddWindow 里会调 pvCheckGlueing（用缺省 WindowRectProvider = 回读），此处再显式落矩形
        for (int i = 0; i < windows.Length; i++)
            m.SetWndRect(i + 1, windows[i].rect);
        return m;
    }

    // ===============================================================================
    // 一、构造与属性（原文 216-246）
    // ===============================================================================

    [Fact]
    public void Constructor_SnapWidthIsTen_AndNoWindows()
    {
        var m = new TDxMagnetic();

        Assert.Equal(10, m.SnapWidth);        // 原文 220
        Assert.Equal(0, m.WndCount);          // 原文 223
        Assert.Equal(0, m.AnchorPoint.X);
        Assert.Equal(0, m.OffsetPoint.X);
        Assert.Equal(0, m.CurrentPoint.X);
        Assert.Equal(0, m.LastPoint.X);
        Assert.Equal(16, TDxMagnetic.LB_RECT); // 原文 193
    }

    [Fact]
    public void SnapWidth_IsPlainAssignment()
    {
        var m = new TDxMagnetic();
        m.SnapWidth = 25;
        Assert.Equal(25, m.SnapWidth);
        m.SnapWidth = -3;                     // 原文无范围校验
        Assert.Equal(-3, m.SnapWidth);
    }

    [Fact]
    public void WmszConstants_MatchWindows()
    {
        Assert.Equal(1, TDxMagnetic.WMSZ_LEFT);
        Assert.Equal(2, TDxMagnetic.WMSZ_RIGHT);
        Assert.Equal(3, TDxMagnetic.WMSZ_TOP);
        Assert.Equal(4, TDxMagnetic.WMSZ_TOPLEFT);
        Assert.Equal(5, TDxMagnetic.WMSZ_TOPRIGHT);
        Assert.Equal(6, TDxMagnetic.WMSZ_BOTTOM);
        Assert.Equal(7, TDxMagnetic.WMSZ_BOTTOMLEFT);
        Assert.Equal(8, TDxMagnetic.WMSZ_BOTTOMRIGHT);
    }

    [Fact]
    public void Destroy_ClearsSingletonAndArrays()
    {
        var m = new TDxMagnetic();
        TDxMagnetic.MagneticWnd = m;
        m.AddWindow(H(1), H(0));

        m.Destroy();

        Assert.Null(TDxMagnetic.MagneticWnd);
        // 托管侧 Destroy 把两个 List 全清（原文 `SetLength(..., 0)`），
        // 故 WndCount 必须回到 0 —— 与原文"清空后逻辑窗口数 0"一致。
        Assert.Equal(0, m.WndCount);
    }

    // ===============================================================================
    // 二、pvWndsConnected（原文 792-814）
    // ===============================================================================

    [Fact]
    public void PvWndsConnected_EdgeTouchingHorizontally_IsConnected()
    {
        // 并集宽 = 30 = 10 + 20（<= 和）→ 包围成立；rcWnd1.Right(10) == rcWnd2.Left(10) → 边重合
        Assert.True(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(10, 0, 30, 10)));
    }

    [Fact]
    public void PvWndsConnected_EdgeTouchingVertically_IsConnected()
    {
        Assert.True(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(0, 10, 10, 30)));
    }

    [Fact]
    public void PvWndsConnected_SameTopEdge_ButTooFarApart_IsNotConnected()
    {
        // 虽然 Top=Top 重合，但并集宽 30 > 10 + 10 = 20 → **包围判定先失败** → 不连接
        // （原文 802-804 的包围判定是 8 条边判定的前置条件）
        Assert.False(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(20, 0, 30, 10)));
    }

    [Fact]
    public void PvWndsConnected_SameTopEdge_AndCloseEnough_IsConnected()
    {
        // 并集宽 15 <= 10 + 10 = 20 → 包围成立；Top=Top 重合 → 连接（原文把 Top=Top 也算重合）
        Assert.True(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(5, 0, 15, 10)));
    }

    [Fact]
    public void PvWndsConnected_SeparatedDiagonally_IsNotConnected()
    {
        // 既不满足包围（并集 25 > 10+10=20），也不共享任何边
        Assert.False(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(15, 15, 25, 25)));
    }

    [Fact]
    public void PvWndsConnected_StrictlyContained_IsNotConnected()
    {
        // 严格包含：并集宽 30 <= 30 + 10 成立（包围过），
        // 但 X 四值 {0,30,10,20} 两两不等、Y 四值 {0,30,10,20} 也两两不等 → 8 条边全不重合 → 不连接
        // （原文 807-810 只比 4 条边是否**数值相等**，不看几何相邻）
        Assert.False(TDxMagnetic.PvWndsConnected(R(0, 0, 30, 30), R(10, 10, 20, 20)));
    }

    [Fact]
    public void PvWndsConnected_ContainedWithSharedLeftTop_IsConnected()
    {
        // 同左上角 (0,0) 的包含关系 → Left=Left 且 Top=Top 都成立 → 连接
        Assert.True(TDxMagnetic.PvWndsConnected(R(0, 0, 30, 30), R(0, 0, 10, 10)));
    }

    [Fact]
    public void PvWndsConnected_BothAxesOffset_NoEdgeCoincidence_IsNotConnected()
    {
        // 并集宽 15 <= 10 + 10 = 20、并集高 13 <= 10 + 10 = 20 → 包围成立；
        // 但 X 四值 {0,10,5,15} 两两不相等、Y 四值 {0,10,3,13} 也两两不相等 → 8 条边全不重合 → 不连接
        Assert.False(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(5, 3, 15, 13)));
    }

    [Fact]
    public void PvWndsConnected_IdenticalRects_IsConnected()
    {
        Assert.True(TDxMagnetic.PvWndsConnected(R(5, 5, 15, 15), R(5, 5, 15, 15)));
    }

    [Fact]
    public void PvWndsConnected_XAlignedButYOverlapping_NoEdgeCoincidence_IsNotConnected()
    {
        // 并集宽 10 <= 10+10；但 X 相同(Y 不同) 时 Left=Left 成立 → 连接
        // 用"X 与 Y 都不同、但 Y 区间重叠"构造真正的"既不满足包围也不共边"
        Assert.False(TDxMagnetic.PvWndsConnected(R(0, 0, 10, 10), R(5, 3, 15, 13)));
    }

    [Fact]
    public void PvWndsConnected_OnlyBottomTopTouching_MeetsWidthBound()
    {
        Assert.True(TDxMagnetic.PvWndsConnected(R(100, 100, 110, 110), R(100, 110, 120, 130)));
    }

    // ===============================================================================
    // 三、AddWindow（原文 356-396）
    // ===============================================================================

    [Fact]
    public void AddWindow_AddsAndNormalisesSelfParentToZero()
    {
        var m = new TDxMagnetic();

        Assert.True(m.AddWindow(H(7), H(0)));
        Assert.Equal(1, m.WndCount);
        Assert.Equal(7, m.GetWndInfo(1).Handle.Value);
        Assert.Equal(0, m.GetWndInfo(1).ParentHandle.Value);
        Assert.False(m.GetWndInfo(1).Glue);

        // 原文 383-386：父 = 自己 → 归一成 0
        Assert.True(m.AddWindow(H(8), H(8)));
        Assert.Equal(0, m.GetWndInfo(2).ParentHandle.Value);
    }

    [Fact]
    public void AddWindow_DuplicateHandle_IsRejected()
    {
        var m = new TDxMagnetic();
        Assert.True(m.AddWindow(H(1), H(0)));
        Assert.False(m.AddWindow(H(1), H(0)));      // 原文 365-369 Exit，Result 仍 False
        Assert.Equal(1, m.WndCount);
    }

    [Fact]
    public void AddWindow_ValidatorRejects_IsRejected()
    {
        var m = new TDxMagnetic { WindowValidator = (h, p) => false };
        Assert.False(m.AddWindow(H(1), H(0)));
        Assert.Equal(0, m.WndCount);
    }

    [Fact]
    public void AddWindow_ValidatorSeesBothHandles()
    {
        var seen = new List<string>();
        var m = new TDxMagnetic
        {
            WindowValidator = (h, p) => { seen.Add($"{h.Value}/{p.Value}"); return true; },
        };

        m.AddWindow(H(3), H(9));

        Assert.Equal(new List<string> { "3/9" }, seen);
    }

    [Fact]
    public void AddWindow_TriggersCheckGlueingImmediately()
    {
        // 原文 389：AddWindow 尾部立刻 `pvCheckGlueing`
        var m = new TDxMagnetic { WindowRectProvider = h => R(0, 0, 10, 10) };
        m.AddWindow(H(1), H(0));
        m.AddWindow(H(2), H(1));

        // 父(1) 与子(2) 矩形完全相同 → pvWndsConnected 为真 → Glue = True
        Assert.True(m.GetWndInfo(2).Glue);
    }

    // ===============================================================================
    // 四、RemoveWindow（原文 399-437）
    // ===============================================================================

    [Fact]
    public void RemoveWindow_ShiftsRemainingEntriesDown()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(20, 0, 30, 10)), (3, 0, R(40, 0, 50, 10)));

        Assert.True(m.RemoveWindow(H(2)));

        Assert.Equal(2, m.WndCount);
        Assert.Equal(1, m.GetWndInfo(1).Handle.Value);
        Assert.Equal(3, m.GetWndInfo(2).Handle.Value);      // 3 前移到索引 2
        Assert.Equal(40, m.GetWndRect(2).Left);             // 矩形一起前移
    }

    [Fact]
    public void RemoveWindow_ClearsParentRelationships()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 1, R(10, 0, 20, 10)));
        Assert.Equal(1, m.GetWndInfo(2).ParentHandle.Value);

        m.RemoveWindow(H(1));

        Assert.Equal(0, m.GetWndInfo(1).ParentHandle.Value);   // 2 现在是索引 1
        Assert.Equal(2, m.GetWndInfo(1).Handle.Value);
    }

    [Fact]
    public void RemoveWindow_UnknownHandle_ReturnsFalse()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        Assert.False(m.RemoveWindow(H(99)));
        Assert.Equal(1, m.WndCount);
    }

    [Fact]
    public void RemoveWindow_LastEntry()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(20, 0, 30, 10)));
        Assert.True(m.RemoveWindow(H(2)));
        Assert.Equal(1, m.WndCount);
        Assert.Equal(1, m.GetWndInfo(1).Handle.Value);
    }

    [Fact]
    public void CheckGlueing_PublicEntry_DelegatesToPvCheckGlueing()
    {
        var calls = 0;
        var m = new TDxMagnetic { WindowRectProvider = h => { calls++; return R(0, 0, 10, 10); } };
        m.AddWindow(H(1), H(0));
        int before = calls;

        m.CheckGlueing();

        Assert.True(calls > before);
    }

    // ===============================================================================
    // 五、pvCheckGlueing（原文 731-790）
    // ===============================================================================

    [Fact]
    public void PvCheckGlueing_NoParent_NoGlue()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(10, 0, 20, 10)));

        m.PvCheckGlueing();

        Assert.False(m.GetWndInfo(1).Glue);     // 无父 → 不吸附
        Assert.False(m.GetWndInfo(2).Glue);
    }

    [Fact]
    public void PvCheckGlueing_DirectConnection_SetsGlue()
    {
        // 2 的父是 1；两者右边/左边相接 → 连接
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 1, R(10, 0, 20, 10)));

        m.PvCheckGlueing();

        Assert.True(m.GetWndInfo(2).Glue);
    }

    [Fact]
    public void PvCheckGlueing_DirectConnection_FailsWhenSeparated()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 1, R(100, 100, 110, 110)));

        m.PvCheckGlueing();

        Assert.False(m.GetWndInfo(2).Glue);
    }

    [Fact]
    public void PvCheckGlueing_ResetsGlueBeforeRechecking()
    {
        var rect1 = R(0, 0, 10, 10);
        var rect2 = R(10, 0, 20, 10);
        var m = new TDxMagnetic { WindowRectProvider = h => h.Value == 1 ? rect1 : rect2 };
        m.AddWindow(H(1), H(0));
        m.AddWindow(H(2), H(1));
        Assert.True(m.GetWndInfo(2).Glue);

        // 把子窗口挪远 → 重新检查后 Glue 应被清掉（原文 742 先 Reset）
        rect2 = R(200, 200, 210, 210);
        m.PvCheckGlueing();

        Assert.False(m.GetWndInfo(2).Glue);
    }

    [Fact]
    public void PvCheckGlueing_IndirectPropagation_SameParent()
    {
        // 1 与 2 同父(0)，2 与 3 同父(1)…
        // 构造：w1(父0) — w2(父1, 与 w1 相接) ; w3(父1, 与 w2 相接但自身与 w1 不相接)
        // 直接连接轮：w2.Glue = connected(w1,w2) = True
        // 间接轮：w2.Glue 真 且 w2.Parent == w3.Parent(都是 1) → w3.Glue := connected(w2,w3)
        var m = Make(
            (1, 0, R(0, 0, 10, 10)),
            (2, 1, R(10, 0, 20, 10)),
            (3, 1, R(20, 0, 30, 10)));

        m.PvCheckGlueing();

        Assert.True(m.GetWndInfo(2).Glue);
        Assert.True(m.GetWndInfo(3).Glue);      // 间接传播（w1 与 w3 并不相接）
    }

    [Fact]
    public void PvCheckGlueing_IndirectPropagation_RequiresSameParent()
    {
        // w3 的父是 5（与 w2 的父 1 不同）→ 间接轮不该把它连上
        var m = Make(
            (1, 0, R(0, 0, 10, 10)),
            (2, 1, R(10, 0, 20, 10)),
            (3, 5, R(20, 0, 30, 10)));

        m.PvCheckGlueing();

        Assert.True(m.GetWndInfo(2).Glue);
        Assert.False(m.GetWndInfo(3).Glue);     // 父不同 → 不传播
    }

    [Fact]
    public void PvCheckGlueing_HiddenWindowIsSkipped()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 1, R(10, 0, 20, 10)));
        m.WindowVisibility = h => h.Value != 2;      // w2 隐藏

        m.PvCheckGlueing();

        Assert.False(m.GetWndInfo(2).Glue);
    }

    [Fact]
    public void PvCheckGlueing_ParentNotRegistered_DoesNotThrowAndNoGlue()
    {
        // 原文不判 lWId = -1（会用 m_rcWnd[-1]，Delphi 下是未定义内存）；托管侧守卫 → 不吸附
        var m = Make((2, 999, R(10, 0, 20, 10)));

        m.PvCheckGlueing();

        Assert.False(m.GetWndInfo(1).Glue);
    }

    // ===============================================================================
    // 六、pvSizeRect（原文 446-532）
    // ===============================================================================

    [Fact]
    public void PvSizeRect_LeftEdge_SnapsToTargetLeft()
    {
        // 目标窗 m_rcWnd[0]（桌面哨兵）设为 (100,0,200,100)；左边界 103 距 100 仅 3 < 10 → 吸附到 100
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 0, 200, 100));

        var r = m.PvSizeRect(H(1), R(103, 20, 150, 60), TDxMagnetic.WMSZ_LEFT);

        Assert.Equal(100, r.Left);
        Assert.Equal(150, r.Right);          // 右边界不变
    }

    [Fact]
    public void PvSizeRect_LeftEdge_SnapsToTargetRight()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 0, 200, 100));

        // 左边界 197 距目标右边 200 差 3 → 吸附到 200（原文 480-483）
        var r = m.PvSizeRect(H(1), R(197, 20, 250, 60), TDxMagnetic.WMSZ_LEFT);

        Assert.Equal(200, r.Left);
    }

    [Fact]
    public void PvSizeRect_RightEdge_SnapsToTargetLeftAndRight()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 0, 200, 100));

        var toLeft = m.PvSizeRect(H(1), R(50, 20, 103, 60), TDxMagnetic.WMSZ_RIGHT);
        Assert.Equal(100, toLeft.Right);     // 右边 103 距目标左 100 差 3

        var toRight = m.PvSizeRect(H(1), R(50, 20, 197, 60), TDxMagnetic.WMSZ_RIGHT);
        Assert.Equal(200, toRight.Right);    // 右边 197 距目标右 200 差 3
    }

    [Fact]
    public void PvSizeRect_TopEdge_SnapsTopOnly()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(0, 100, 100, 200));

        var r = m.PvSizeRect(H(1), R(20, 103, 60, 150), TDxMagnetic.WMSZ_TOP);

        Assert.Equal(100, r.Top);
        Assert.Equal(150, r.Bottom);
    }

    [Fact]
    public void PvSizeRect_BottomEdge_SnapsBottomOnly()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(0, 100, 100, 200));

        var r = m.PvSizeRect(H(1), R(20, 50, 60, 103), TDxMagnetic.WMSZ_BOTTOM);
        Assert.Equal(100, r.Bottom);

        var r2 = m.PvSizeRect(H(1), R(20, 50, 60, 197), TDxMagnetic.WMSZ_BOTTOM);
        Assert.Equal(200, r2.Bottom);
    }

    [Fact]
    public void PvSizeRect_OutOfSnapRange_DoesNotSnap()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 0, 200, 100));

        // 左边界 120 距目标左 100 与目标右 200 都 >= 10 → 不吸附
        var r = m.PvSizeRect(H(1), R(120, 20, 250, 60), TDxMagnetic.WMSZ_LEFT);

        Assert.Equal(120, r.Left);
    }

    [Fact]
    public void PvSizeRect_NoOverlapOnTheOtherAxis_DoesNotSnap()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 0, 200, 100));

        // Y 方向完全不重叠（Top=200 >= 目标 Bottom 100 + 10）→ X 吸附不执行
        var r = m.PvSizeRect(H(1), R(103, 200, 150, 250), TDxMagnetic.WMSZ_LEFT);

        Assert.Equal(103, r.Left);
    }

    [Fact]
    public void PvSizeRect_SkipsItsOwnWindow()
    {
        var m = Make((1, 0, R(103, 20, 150, 60)));
        m.SetWndRect(0, R(100, 0, 200, 100));

        // 候选里 lc==1 就是自己 → 跳过；只剩桌面哨兵(100..200) → 仍吸附到 100
        var r = m.PvSizeRect(H(1), R(103, 20, 150, 60), TDxMagnetic.WMSZ_LEFT);
        Assert.Equal(100, r.Left);
    }

    [Fact]
    public void PvSizeRect_HiddenTargetIsSkipped()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(0, 0, 10, 10)));
        m.SetWndRect(1, R(100, 0, 200, 100));       // 让窗口 1 当"目标"
        m.SetWndRect(0, R(1000, 0, 2000, 1000));    // 桌面哨兵远置
        m.WindowVisibility = h => h.Value != 1;     // 窗口 1 隐藏

        var r = m.PvSizeRect(H(2), R(103, 20, 150, 60), TDxMagnetic.WMSZ_LEFT);

        Assert.Equal(103, r.Left);                  // 隐藏目标不参与吸附
    }

    [Fact]
    public void PvSizeRect_TopLeftEdge_SnapsBothAxes()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 100, 200, 200));

        var r = m.PvSizeRect(H(1), R(103, 103, 150, 150), TDxMagnetic.WMSZ_TOPLEFT);

        Assert.Equal(100, r.Left);
        Assert.Equal(100, r.Top);
    }

    [Fact]
    public void PvSizeRect_UnknownEdge_ChangesNothing()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(100, 100, 200, 200));

        var r = m.PvSizeRect(H(1), R(103, 103, 150, 150), 999);

        Assert.Equal(R(103, 103, 150, 150), r);
    }

    [Fact]
    public void PvSizeRect_UsesEnteringRectSnapshot_NotTheMutatingOne()
    {
        // 原文 453 先 CopyMemory 出 rcTmp，之后所有 Abs 比较都拿 rcTmp；
        // 若用被改写的 rcWnd 比较，连续吸附会串味。这里验证"只吸附一次"：
        // 目标 (100,0,200,100)，左边界 103 → 吸附到 100；若错用 result.Left(100) 再比一次，
        // 会因 |100-100|<10 再"吸附"一次（结果仍是 100，无法区分），故改用目标右边：
        // 目标 (96,0,100,100)，左边界 99 → 距 96 差 3（吸到 96），距 100 差 1（再吸到 100，后判者胜）
        var m = Make((1, 0, R(0, 0, 10, 10)));
        m.SetWndRect(0, R(96, 0, 100, 100));

        var r = m.PvSizeRect(H(1), R(99, 20, 150, 60), TDxMagnetic.WMSZ_LEFT);

        Assert.Equal(100, r.Left);      // 两个 case 都命中 → 后一个（Right）胜
    }

    // ===============================================================================
    // 七、pvMoveRect（原文 534-729）
    // ===============================================================================

    [Fact]
    public void PvMoveRect_TranslatesToCursorPlusOffset()
    {
        // 没有目标窗时 lOffx/lOffy 保持 0 → 只做"移到光标 + 偏移"
        var m = new TDxMagnetic();
        m.CurrentPoint = TDxPoint.Point(50, 60);
        m.OffsetPoint = TDxPoint.Point(3, 4);
        m.WndCount.ToString();

        var r = m.PvMoveRect(H(1), R(0, 0, 10, 10));

        // OffsetRect(rcWnd, (50-0)+3, 0) → Left 53；再 OffsetRect(0, (60-0)+4) → Top 64
        Assert.Equal(53, r.Left);
        Assert.Equal(64, r.Top);
        Assert.Equal(63, r.Right);
        Assert.Equal(74, r.Bottom);
    }

    [Fact]
    public void PvMoveRect_UsesCursorPosProvider()
    {
        // 把桌面哨兵挪远，隔离"吸附"对落点的影响，只看平移
        var m = new TDxMagnetic { CursorPosProvider = () => TDxPoint.Point(20, 30) };
        m.SetWndRect(0, R(1000, 1000, 2000, 2000));
        var r = m.PvMoveRect(H(1), R(0, 0, 10, 10));

        Assert.Equal(20, r.Left);
        Assert.Equal(30, r.Top);
        Assert.Equal(20, m.CurrentPoint.X);     // 接缝返回的是 (20,30)
        Assert.Equal(30, m.CurrentPoint.Y);
    }

    [Fact]
    public void PvMoveRect_SnapsLeftToTargetLeft()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(100, 0, 200, 100)));
        m.CurrentPoint = TDxPoint.Point(103, 30);  // 移动窗左边界将落到 103

        // handle = 1；候选 0(桌面=0,0,0,0) 与 2(100..200)
        var r = m.PvMoveRect(H(1), R(0, 0, 40, 40));

        // 平移后 Left=103；对目标 2：|103-100|=3<10 → lOffx = 100-103 = -3 → Left = 100
        Assert.Equal(100, r.Left);
    }

    [Fact]
    public void PvMoveRect_SnapsRightToTargetLeft()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(100, 0, 200, 100)));
        // 移动窗右边界将落到 103：rcWnd 宽 40，Left=63 → Right=103
        m.CurrentPoint = TDxPoint.Point(63, 30);

        var r = m.PvMoveRect(H(1), R(0, 0, 40, 40));

        // 对目标 2：|Left(63)-100|=37 不命中；|Left-Right(200)|=137 不命中；
        // |Right(103)-Left(100)|=3 → lOffx = 100-103 = -3 → Left 60, Right 100
        Assert.Equal(100, r.Right);
    }

    [Fact]
    public void PvMoveRect_SnapsTopToTargetTop()
    {
        var m = Make((1, 0, R(0, 0, 200, 10)), (2, 0, R(0, 100, 200, 200)));
        m.CurrentPoint = TDxPoint.Point(10, 103);

        var r = m.PvMoveRect(H(1), R(0, 0, 40, 40));

        // 平移后 Top=103；X 方向与目标 2 重叠（10..50 与 0..200）→ Y 吸附 |103-100|=3 → Top=100
        Assert.Equal(100, r.Top);
    }

    [Fact]
    public void PvMoveRect_LaterCaseWins_WhenBothEdgesHit()
    {
        // 目标窗很窄：(100,0,106,100)。移动窗 Right 将落到 103：
        //   |Right(103) - Left(100)| = 3   → lOffx = 100-103 = -3
        //   |Right(103) - Right(106)| = 3  → lOffx = 106-103 = +3  ← 后判者胜
        var m = Make((1, 0, R(0, 0, 300, 10)), (2, 0, R(100, 0, 106, 100)));
        m.CurrentPoint = TDxPoint.Point(63, 30);   // rcWnd 宽 40 → Right=103

        var r = m.PvMoveRect(H(1), R(0, 0, 40, 40));

        Assert.Equal(106, r.Right);                // 证明了"后判者胜"
    }

    [Fact]
    public void PvMoveRect_SkipsItselfAndHiddenTargets()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)), (2, 0, R(100, 0, 200, 100)));
        m.WindowVisibility = h => h.Value != 2;    // 目标 2 隐藏
        m.CurrentPoint = TDxPoint.Point(103, 30);

        var r = m.PvMoveRect(H(1), R(0, 0, 40, 40));

        Assert.Equal(103, r.Left);                 // 不吸附
    }

    [Fact]
    public void PvMoveRect_MovesGluedChildren()
    {
        // 2 是 1 的已吸附子窗口：先把 Glue 建立起来，再拖 1
        var m = Make((1, 0, R(0, 0, 100, 100)), (2, 1, R(100, 0, 200, 100)));
        m.PvCheckGlueing();
        Assert.True(m.GetWndInfo(2).Glue);

        m.CurrentPoint = TDxPoint.Point(20, 0);
        m.MovedChildren.Clear();

        var r = m.PvMoveRect(H(1), R(0, 0, 100, 100));

        // 移动窗左上从 (0,0) → (20,0)；子窗 2 的 Left(100) - (自身旧 Left(0) - rcWnd.Left(20)) = 120
        Assert.Equal(20, r.Left);
        Assert.Single(m.MovedChildren);
        Assert.Equal(2, m.MovedChildren[0].handle.Value);
        Assert.Equal(120, m.MovedChildren[0].x);
        Assert.Equal(0, m.MovedChildren[0].y);
    }

    [Fact]
    public void PvMoveRect_ChildMoverSeamReceivesRequests()
    {
        var moves = new List<(int h, int x, int y)>();
        var m = new TDxMagnetic { ChildMover = (h, x, y) => moves.Add((h.Value, x, y)) };
        m.AddWindow(H(1), H(0));
        m.AddWindow(H(2), H(1));
        m.SetWndRect(1, R(0, 0, 100, 100));
        m.SetWndRect(2, R(100, 0, 200, 100));
        m.PvCheckGlueing();

        m.CurrentPoint = TDxPoint.Point(10, 5);
        m.PvMoveRect(H(1), R(0, 0, 100, 100));

        Assert.Single(moves);
        Assert.Equal(2, moves[0].h);
        Assert.Equal(110, moves[0].x);
        Assert.Equal(5, moves[0].y);
        Assert.Empty(m.MovedChildren);          // 走接缝时不再记录
    }

    [Fact]
    public void PvMoveRect_DoesNotMoveUnGluedChildren()
    {
        var m = Make((1, 0, R(0, 0, 100, 100)), (2, 1, R(500, 500, 600, 600)));   // 不相接
        m.PvCheckGlueing();
        Assert.False(m.GetWndInfo(2).Glue);

        m.CurrentPoint = TDxPoint.Point(20, 0);
        m.MovedChildren.Clear();
        m.PvMoveRect(H(1), R(0, 0, 100, 100));

        Assert.Empty(m.MovedChildren);
    }

    [Fact]
    public void PvMoveRect_StoresLastPoint()
    {
        var m = new TDxMagnetic();
        m.CurrentPoint = TDxPoint.Point(11, 22);

        m.PvMoveRect(H(1), R(0, 0, 10, 10));

        Assert.Equal(11, m.LastPoint.X);
        Assert.Equal(22, m.LastPoint.Y);
    }

    [Fact]
    public void PvMoveRect_NoWindows_SnapsToDesktopSentinel()
    {
        // 注意：原文 557 的循环是 `lc1 := 0 to m_lWndCount` —— **索引 0 的桌面哨兵也参与吸附**，
        // 而它初始是 (0,0,0,0)。所以"没有注册窗口"时仍然会往原点吸附：
        //   平移后 Left=6：|6-0|=6 < SnapWidth(10) → lOffx = 0-6 = -6
        //   平移后 Top=9 ：|9-0|=9 < 10            → lOffy = 0-9 = -9
        // → 结果回到 (0,0,20,20)。
        var m = new TDxMagnetic { OffsetPoint = TDxPoint.Point(1, 2) };
        m.CurrentPoint = TDxPoint.Point(5, 7);

        var r = m.PvMoveRect(H(1), R(10, 20, 30, 40));

        Assert.Equal(R(0, 0, 20, 20), r);
    }

    [Fact]
    public void PvMoveRect_NoWindows_FarFromSentinel_OnlyTranslates()
    {
        // 把桌面哨兵挪远，就只剩"移到光标 + 偏移"这一步
        var m = new TDxMagnetic { OffsetPoint = TDxPoint.Point(1, 2) };
        m.SetWndRect(0, R(1000, 1000, 2000, 2000));
        m.CurrentPoint = TDxPoint.Point(5, 7);

        var r = m.PvMoveRect(H(1), R(10, 20, 30, 40));

        Assert.Equal(10 + ((5 - 10) + 1), r.Left);   // 6
        Assert.Equal(20 + ((7 - 20) + 2), r.Top);    // 9
    }

    // ===============================================================================
    // 八、辅助访问器
    // ===============================================================================

    [Fact]
    public void GetWndInfo_And_GetWndRect_OutOfRange_AreSafe()
    {
        var m = Make((1, 0, R(0, 0, 10, 10)));

        Assert.Null(m.GetWndInfo(0));
        Assert.Null(m.GetWndInfo(2));
        Assert.Equal(TDxRect.Empty, m.GetWndRect(99));
        Assert.Equal(TDxRect.Empty, m.GetWndRect(-1));
    }

    [Fact]
    public void PvWndGetInfoIndex_FindsAndReturnsMinusOne()
    {
        var m = Make((5, 0, R(0, 0, 10, 10)), (9, 0, R(20, 0, 30, 10)));

        Assert.Equal(1, m.PvWndGetInfoIndex(H(5)));
        Assert.Equal(2, m.PvWndGetInfoIndex(H(9)));
        Assert.Equal(-1, m.PvWndGetInfoIndex(H(7)));
    }

    [Fact]
    public void PvWndParentGetInfoIndex_LooksUpByWindowHandle()
    {
        var m = Make((5, 0, R(0, 0, 10, 10)), (9, 5, R(20, 0, 30, 10)));

        Assert.Equal(1, m.PvWndParentGetInfoIndex(H(5)));
        Assert.Equal(-1, m.PvWndParentGetInfoIndex(H(99)));
    }

    [Fact]
    public void MagneticHandle_EqualityAndToString()
    {
        Assert.True(H(7).Equals(H(7)));
        Assert.False(H(7).Equals(H(8)));
        Assert.Equal("0x7", H(7).ToString());
        Assert.Equal("0", TDxMagneticHandle.Null.ToString());
    }

    [Fact]
    public void SetWndRect_GrowsArrayAsNeeded()
    {
        var m = new TDxMagnetic();
        m.SetWndRect(5, R(1, 2, 3, 4));

        Assert.Equal(R(1, 2, 3, 4), m.GetWndRect(5));
        Assert.Equal(TDxRect.Empty, m.GetWndRect(4));
    }
}
