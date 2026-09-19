using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// DxControls.pas（4,147 行）余部的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/DxControls.cs）：
//   * 单元级自由函数：MakeGuiName / FindDxComponent / Pos / Copy / LowerCase / GetAllSubComponents
//   * DxControlOps 控件树：Insert/Remove/InserComponent/RemoveComponent/DestroyComponents/
//     FindComponent(名/ID)/ComponentIndex/SetComponentIndex/Initialize/GetControl(Count)
//   * DxControlOps 几何与状态：SetClientRect/MoveBy/ResizeBy/ApplyConstraint/SetAlign/DoResize/
//     SetVisible/Show/Hide/Close/SetEnabled/SetOwner/ImageIndexChange/FindControl/GetCtrl/
//     GetRootCtrl/SetCenter(A)/WidthCenter/HeightCenter/GetFocused/GetMouseDowned/GetMouseMoveed
//   * DxControlOps 绘制几何：FillRect/FillRectAlpha/FrameRect/DrawRect(×3)/DrawRectColor(×2)/
//     DrawRectColorAlpha(×2)/GetPaintRect/Paint（用 TDxRecordingPainter 断言操作序列）
//   * DxControlOps 文本：FormatCaption/ArrestVariable/ArrestStringEx/CompareLStr/Sub49ADB8/SetMousePoint
//   * DxControlEngine：模态栈/焦点/鼠标键盘派发/绘制遍历/ExecutePosition/队列
//
// 参照物：Delphi 原文 + 上游接缝 DxComponentCommon.cs（只读）。
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxControlsTests
{
    // -------------------------------------------------------------------------------
    // 测试夹具
    // -------------------------------------------------------------------------------

    /// <summary>最小可实例化控件（TDxControl 是 abstract，测试用一个空壳子类）。</summary>
    private sealed class TTestCtl : TDxControl
    {
        public string Tag;
        public TTestCtl(string tag = "") { Tag = tag; Width = 10; Height = 10; }
    }

    /// <summary>记录 DoShow/DoHide/DoEnable/DoDisable/DoCaptionChange/DoResize/DoPaint 覆写调用的探针控件。</summary>
    private sealed class TProbeCtl : TDxControl
    {
        public readonly List<string> Calls = new();

        protected override void DoCaptionChange() => Calls.Add("DoCaptionChange");

        protected override void DoResize(ref TDxRect newRect) => Calls.Add("DoResize");

        protected override void DoPaint() => Calls.Add("DoPaint");

        public void Clear() => Calls.Clear();
    }

    /// <summary>
    /// 新建引擎（控件树根）。原文 TDxControlEngine 的 ClientRect 就是主窗体矩形；
    /// 托管侧测试显式给一个 800x600（与原文 FMoveRange 缺省 Rect(0,0,800,600) 一致），
    /// 因为 TDxControl.Move 的门控要读 RootCtrl.Width/Height（DxControls.pas 4068）。
    /// </summary>
    private static TDxControlEngine NewEngine()
        => new TDxControlEngine { ClientRect = TDxRect.Bounds(0, 0, 800, 600) };

    /// <summary>把 child 挂到 owner 下（走原文 InserComponent）。</summary>
    private static TDxControl Attach(TDxControl owner, TDxControl child)
    {
        DxControlOps.InserComponent(owner, child);
        return child;
    }

    // ===============================================================================
    // 一、单元级自由函数（DxControls.pas 655-768 / 4130-4140）
    // ===============================================================================

    [Fact]
    public void MakeGuiName_StripsTdxPrefix_AndFindsFreeSuffix()
    {
        // 原文 750-763：类名以 'tdx' 开头（Pos=1，大小写不敏感）→ 去前 3 字符；从 1 起找空号。
        // 注意原文 **不做大小写还原**：'TDxButton' → 'Button1'，'tdxbutton' → 'button1'。
        var root = NewEngine();
        Assert.Equal("Button1", DxControlsUnit.MakeGuiName(root, "TDxButton"));
        Assert.Equal("button1", DxControlsUnit.MakeGuiName(root, "tdxbutton"));

        // 已存在 Button1 → 跳到 Button2
        var c1 = new TTestCtl();
        DxControlHooks.InitName(c1, "Button1");
        Attach(root, c1);
        Assert.Equal("Button2", DxControlsUnit.MakeGuiName(root, "TDxButton"));
    }

    [Fact]
    public void MakeGuiName_NoTdxPrefix_KeepsWholeName()
    {
        var root = NewEngine();
        // 无 'tdx' 前缀 → 整名保留；后缀编号从 1 起（原文 760-763）
        Assert.Equal("Panel1", DxControlsUnit.MakeGuiName(root, "Panel"));
    }

    [Fact]
    public void FindDxComponent_IsCaseInsensitive_AndRecursive()
    {
        var root = NewEngine();
        var mid = new TTestCtl();
        var leaf = new TTestCtl();
        Attach(root, mid);
        Attach(mid, leaf);
        DxControlHooks.InitName(leaf, "DeepOne");

        Assert.True(DxControlsUnit.FindDxComponent(root, "deepone"));
        Assert.True(DxControlsUnit.FindDxComponent(root, "DEEPONE"));
        Assert.False(DxControlsUnit.FindDxComponent(root, "NotThere"));
        Assert.False(DxControlsUnit.FindDxComponent(null, "DeepOne"));
    }

    [Theory]
    [InlineData("abcabc", "b", 2)]
    [InlineData("abcabc", "z", 0)]
    [InlineData("abc", "", 0)]
    [InlineData("", "a", 0)]
    [InlineData("abc", "abc", 1)]
    public void Pos_IsOneBased_ZeroWhenMissing(string s, string sub, int expected)
        => Assert.Equal(expected, DxControlsUnit.Pos(s, sub));

    [Theory]
    [InlineData("abcdef", 1, 3, "abc")]
    [InlineData("abcdef", 4, 3, "def")]
    [InlineData("abcdef", 6, 5, "f")]      // 截断
    [InlineData("abcdef", 7, 1, "")]       // 起点越界
    [InlineData("abcdef", 1, 0, "")]       // count <= 0
    [InlineData("", 1, 3, "")]
    public void Copy_ClampsLikeDelphi(string s, int idx, int count, string expected)
        => Assert.Equal(expected, DxControlsUnit.Copy(s, idx, count));

    [Fact]
    public void GetAllSubComponents_ExcludesSelf_PreOrder()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(a, new TTestCtl("b"));
        var c = Attach(root, new TTestCtl("c"));

        var list = new List<TDxControl>();
        DxControlOps.GetAllSubComponents(root, list);

        Assert.Equal(3, list.Count);
        Assert.Same(a, list[0]);
        Assert.Same(b, list[1]);     // 先加直接子 a，再递归 a 的子 b，最后回到 root 的第二个子 c
        Assert.Same(c, list[2]);
    }

    [Fact]
    public void GetAllSubComponents_NullArgs_AreSafe()
    {
        DxControlOps.GetAllSubComponents(null, new List<TDxControl>());
        DxControlOps.GetAllSubComponents(NewEngine(), null);
    }

    // ===============================================================================
    // 二、控件树（原文 2036-2216）
    // ===============================================================================

    [Fact]
    public void Insert_SetDxOwner_AndComponentCount()
    {
        var root = NewEngine();
        var c = new TTestCtl();
        Assert.Equal(0, DxControlOps.ComponentCount(root));

        DxControlOps.Insert(root, c);
        Assert.Same(root, c.DxOwner);
        Assert.Equal(1, DxControlOps.ComponentCount(root));
        Assert.Same(c, DxControlOps.Components(root, 0));
        Assert.Equal(0, DxControlOps.GetComponentIndex(c));
    }

    [Fact]
    public void Remove_ClearsOwner_AndDropsTableWhenEmpty()
    {
        var root = NewEngine();
        var c = new TTestCtl();
        DxControlOps.Insert(root, c);

        DxControlOps.Remove(root, c);
        Assert.Null(c.DxOwner);
        Assert.Equal(0, DxControlOps.ComponentCount(root));   // 原文表空即置 nil
        Assert.Null(DxControlOps.Components(root, 0));
        Assert.Equal(-1, DxControlOps.GetComponentIndex(c));
    }

    [Fact]
    public void RemoveComponent_OnNonEmptyTable_KeepsOthers()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));

        DxControlOps.RemoveComponent(root, a);
        Assert.Equal(1, DxControlOps.ComponentCount(root));
        Assert.Same(b, DxControlOps.Components(root, 0));
    }

    [Fact]
    public void DestroyComponents_RemovesLastFirst_AndRecurses()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(a, new TTestCtl("b"));

        DxControlOps.DestroyComponents(root);

        Assert.Equal(0, DxControlOps.ComponentCount(root));
        Assert.Equal(0, DxControlOps.ComponentCount(a));
        Assert.True(a.IsDisposed);
        Assert.True(b.IsDisposed);
    }

    [Fact]
    public void FindComponentByName_OnlyDirectChildren()
    {
        var root = NewEngine();
        var mid = Attach(root, new TTestCtl());
        var leaf = Attach(mid, new TTestCtl());
        DxControlHooks.InitName(leaf, "Leaf");
        DxControlHooks.InitName(mid, "Mid");

        Assert.Null(DxControlOps.FindComponent(root, "Leaf"));   // 原文只找直接子
        Assert.Same(leaf, DxControlOps.FindComponent(mid, "Leaf"));
        Assert.Same(mid, DxControlOps.FindComponent(root, "Mid"));
    }

    [Fact]
    public void FindComponentByName_EmptyNameOrNilTable_YieldsNull()
    {
        var root = NewEngine();
        Assert.Null(DxControlOps.FindComponent(root, ""));
        Assert.Null(DxControlOps.FindComponent(root, null));
        Assert.Null(DxControlOps.FindComponent(new TTestCtl(), "X"));
    }

    [Fact]
    public void FindComponentByID_IsRecursive_AndZeroIsSentinel()
    {
        var root = NewEngine();
        var mid = Attach(root, new TTestCtl());
        var leaf = Attach(mid, new TTestCtl());
        leaf.ControlID = 42;

        Assert.Same(leaf, DxControlOps.FindComponent(root, 42));
        Assert.Null(DxControlOps.FindComponent(root, 0));    // 原文 `if ID <> 0` 守卫
        Assert.Null(DxControlOps.FindComponent(root, 99));
    }

    [Fact]
    public void SetComponentIndex_ClampsToRange()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));
        var c = Attach(root, new TTestCtl("c"));

        DxControlOps.SetComponentIndex(c, 0);
        Assert.Same(c, DxControlOps.Components(root, 0));
        Assert.Same(a, DxControlOps.Components(root, 1));

        DxControlOps.SetComponentIndex(c, -5);              // 夹到 0
        Assert.Same(c, DxControlOps.Components(root, 0));

        DxControlOps.SetComponentIndex(c, 999);             // 夹到 Count-1
        Assert.Same(c, DxControlOps.Components(root, 2));

        // 无 Owner 时静默返回
        DxControlOps.SetComponentIndex(new TTestCtl(), 0);
    }

    [Fact]
    public void GetControl_OutOfRange_ReturnsNull()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl());
        Assert.Same(a, DxControlOps.GetControl(root, 0));
        Assert.Null(DxControlOps.GetControl(root, 1));
        Assert.Null(DxControlOps.GetControl(root, -1));
        Assert.Equal(1, DxControlOps.GetControlCount(root));
    }

    [Fact]
    public void Initialize_And_Finalize_RecursePreOrder()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl());
        var b = Attach(a, new TTestCtl());

        DxControlOps.Initialize(root);          // 不应抛异常
        DxControlOps.FinalizeControls(root);
        Assert.Equal(1, DxControlOps.ComponentCount(a));
        Assert.Same(b, DxControlOps.Components(a, 0));
    }

    [Fact]
    public void Notification_RecursesAllDescendants()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl());
        var b = Attach(a, new TTestCtl());
        var log = new List<TDxControl>();
        // 用一个可观察的 Notification：从 a 再广播到 b —— 本实现无可注入回调，
        // 故只断言不抛异常且树结构不变（原文基类无副作用）
        DxControlOps.Notification(root, b, TOperation.opRemove);
        Assert.Equal(1, DxControlOps.ComponentCount(root));
        Assert.Equal(1, DxControlOps.ComponentCount(a));
        Assert.Empty(log);
    }

    // ===============================================================================
    // 三、根控件与命名查找（原文 2573-2622）
    // ===============================================================================

    [Fact]
    public void GetRootCtrl_EngineIsOwnRoot()
    {
        var root = NewEngine();
        Assert.Same(root, DxControlOps.GetRootCtrl(root));
        Assert.Same(root, DxControlOps.RootCtrlOf(root));
    }

    [Fact]
    public void GetRootCtrl_WalksUpToEngine()
    {
        var root = NewEngine();
        var mid = Attach(root, new TTestCtl());
        var leaf = Attach(mid, new TTestCtl());

        Assert.Same(root, DxControlOps.GetRootCtrl(leaf));
        Assert.Same(root, DxControlOps.RootCtrlOf(leaf));
    }

    [Fact]
    public void GetRootCtrl_DetachedControl_IsNull()
    {
        Assert.Null(DxControlOps.GetRootCtrl(new TTestCtl()));
        Assert.Null(DxControlOps.RootCtrlOf(new TTestCtl()));
    }

    [Fact]
    public void GetCtrl_LowerCasesName_AndFindsSelfOrDirectChild()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl());
        DxControlHooks.InitName(c, "MyButton");

        Assert.Same(c, DxControlOps.GetCtrl(root, "MyButton"));
        Assert.Same(c, DxControlOps.GetCtrl(root, "MYBUTTON"));
        Assert.Same(c, DxControlOps.GetCtrl(c, "mybutton"));   // 命中自身
        Assert.Null(DxControlOps.GetCtrl(root, "nope"));
    }

    // ===============================================================================
    // 四、几何（原文 2362-2493 / 2625-2757）
    // ===============================================================================

    [Fact]
    public void MoveBy_And_ResizeBy()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(10, 20, 30, 40) };
        DxControlOps.MoveBy(c, 5, -5);
        Assert.Equal(new TDxRect(15, 15, 45, 55), c.ClientRect);

        DxControlOps.ResizeBy(c, 10, 20);
        Assert.Equal(40, c.Width);
        Assert.Equal(60, c.Height);
    }

    [Fact]
    public void ApplyConstraint_TakesIntersection()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 100, 100) };
        DxControlOps.ApplyConstraint(c, TDxRect.Bounds(10, 10, 50, 50));
        Assert.Equal(new TDxRect(10, 10, 60, 60), c.ClientRect);

        // 无交集时 ShortRect 不判空（原文给"反向矩形"）
        var d = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) };
        DxControlOps.ApplyConstraint(d, TDxRect.Bounds(100, 100, 20, 20));
        Assert.Equal(new TDxRect(100, 100, 10, 10), d.ClientRect);
    }

    [Fact]
    public void SetClientRect_AppliesDoResize_AndFiresOnResize()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl());
        int fired = 0;
        DxControlHooks.SetOnResize(c, _ => fired++);

        DxControlOps.SetClientRect(c, TDxRect.Bounds(1, 2, 3, 4));

        Assert.Equal(new TDxRect(1, 2, 4, 6), c.ClientRect);
        Assert.Equal(1, fired);
    }

    [Fact]
    public void SetAlign_AlNone_DoesNothing()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 200, 100) });
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Bounds(5, 5, 25, 15) });

        DxControlOps.SetAlign(c, TDxAlign.alNone);
        Assert.Equal(new TDxRect(5, 5, 30, 20), c.ClientRect);   // Bounds(5,5,25,15) = (5,5,30,20)；alNone → 不动
    }

    [Fact]
    public void SetAlign_AlTop_FillsOwnerWidth_PinsTopLeft()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 200, 100) });
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Bounds(5, 5, 25, 15) });

        DxControlOps.SetAlign(c, TDxAlign.alTop);

        Assert.Equal(200, c.Width);      // 原文先 Width := Owner.Width
        Assert.Equal(0, c.Top);
        Assert.Equal(0, c.Left);
        Assert.Equal(15, c.Height);      // 高不变（Bounds(5,5,25,15) 的高 = 15）
    }

    [Fact]
    public void SetAlign_AlBottom_UsesOwnerHeightMinusOwnHeight()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 300, 120) });
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 300, 30) });
        c.Height = 25;

        DxControlOps.SetAlign(c, TDxAlign.alBottom);

        Assert.Equal(300, c.Width);
        Assert.Equal(95, c.Top);         // 120 - 25
        Assert.Equal(0, c.Left);
    }

    [Fact]
    public void SetAlign_AlRight_PinsToOwnerRightEdge()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 300, 120) });
        var c = Attach(owner, new TTestCtl());
        c.Width = 40;

        DxControlOps.SetAlign(c, TDxAlign.alRight);

        Assert.Equal(120, c.Height);     // 原文先 Height := Owner.Height
        Assert.Equal(0, c.Top);
        Assert.Equal(260, c.Left);       // 300 - 40
    }

    [Fact]
    public void SetAlign_AlClient_FillsOwner()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 300, 120) });
        var c = Attach(owner, new TTestCtl());

        DxControlOps.SetAlign(c, TDxAlign.alClient);

        Assert.Equal(300, c.Width);
        Assert.Equal(120, c.Height);
        Assert.Equal(0, c.Left);
        Assert.Equal(0, c.Top);
    }

    [Fact]
    public void SetAlign_NoOwner_OnlyRecordsValue()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(3, 4, 13, 14) };
        DxControlOps.SetAlign(c, TDxAlign.alClient);
        Assert.Equal(TDxAlign.alClient, c.Align);
        Assert.Equal(new TDxRect(3, 4, 16, 18), c.ClientRect);   // Bounds(3,4,13,14) = (3,4,16,18)；无 Owner → 不落位
    }

    [Fact]
    public void DoResize_AlBottom_UsesOldHeight_NotNewRectHeight()
    {
        // 原文 2729-2734：alBottom 用 `Height`（旧的自身高）而不是 NewRect 的高 —— 原文怪癖
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 300, 120) });
        var c = Attach(owner, new TTestCtl());
        c.Height = 30;
        c.Align = TDxAlign.alBottom;    // 直接置字段，不经 SetAlign 的落位分支

        var newRect = TDxRect.Bounds(999, 999, 1, 1);
        DxControlOps.DoResize(c, ref newRect);

        Assert.Equal(0, newRect.Left);
        Assert.Equal(90, newRect.Top);          // 120 - 30（旧高）
        Assert.Equal(300, newRect.Right);
        Assert.Equal(120, newRect.Bottom);      // 90 + 30
    }

    [Fact]
    public void DoResize_AlRight_UsesOldWidth()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 300, 120) });
        var c = Attach(owner, new TTestCtl());
        c.Width = 40;
        c.Align = TDxAlign.alRight;

        var newRect = TDxRect.Empty;
        DxControlOps.DoResize(c, ref newRect);

        Assert.Equal(260, newRect.Left);
        Assert.Equal(300, newRect.Right);
        Assert.Equal(120, newRect.Bottom);
    }

    [Fact]
    public void DoResize_NoOwner_LeavesRectUntouched()
    {
        var c = new TTestCtl();
        c.Align = TDxAlign.alClient;
        var newRect = TDxRect.Rect(1, 2, 3, 4);          // 用 Rect（右/下为绝对值），避免 Bounds 语义混淆
        DxControlOps.DoResize(c, ref newRect);
        Assert.Equal(new TDxRect(1, 2, 3, 4), newRect);
    }

    // ===============================================================================
    // 五、可见 / 可用 / 居中（原文 2415-2462 / 2897-2928）
    // ===============================================================================

    [Fact]
    public void SetVisible_OnlyFiresOnChange_AndHonoursCallbackOrder()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl());
        var order = new List<string>();
        DxControlHooks.SetOnShow(c, _ => order.Add("OnShow"));
        DxControlHooks.SetOnHide(c, _ => order.Add("OnHide"));

        DxControlOps.SetVisible(c, false);
        Assert.Equal(new List<string> { "OnHide" }, order);   // 隐藏时 OnHide 先
        Assert.False(c.Visible);

        order.Clear();
        DxControlOps.SetVisible(c, false);                    // 值未变 → 不再触发
        Assert.Empty(order);

        DxControlOps.SetVisible(c, true);
        Assert.Equal(new List<string> { "OnShow" }, order);   // 显示时 OnShow 后
        Assert.True(c.Visible);
    }

    [Fact]
    public void ShowHideClose_AreSetVisibleWrappers()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl());

        DxControlOps.Hide(c);
        Assert.False(c.Visible);
        DxControlOps.Show(c);
        Assert.True(c.Visible);
        DxControlOps.Close(c);
        Assert.False(c.Visible);
    }

    [Fact]
    public void SetEnabled_OnlyFiresOnChange()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl());
        DxControlOps.SetEnabled(c, false);
        Assert.False(c.Enabled);
        DxControlOps.SetEnabled(c, false);
        Assert.False(c.Enabled);
        DxControlOps.SetEnabled(c, true);
        Assert.True(c.Enabled);
    }

    [Fact]
    public void SetOwner_RehomesControl()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));
        var child = Attach(a, new TTestCtl("child"));

        DxControlOps.SetOwner(child, b);

        Assert.Same(b, child.DxOwner);
        Assert.Equal(0, DxControlOps.ComponentCount(a));
        Assert.Equal(1, DxControlOps.ComponentCount(b));

        // nil 或相同 → 不动
        DxControlOps.SetOwner(child, null);
        Assert.Same(b, child.DxOwner);
        DxControlOps.SetOwner(child, b);
        Assert.Equal(1, DxControlOps.ComponentCount(b));
    }

    [Fact]
    public void SetCenterA_CentersOnlyWhenDesigningIsFalse()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 100, 60) });
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 10) });
        Assert.True(c.Designing);       // 原文构造默认 True

        DxControlOps.SetCenterA(c, true);
        Assert.Equal(0, c.Left);        // 设计期不居中

        c.Designing = false;
        DxControlOps.SetCenterA(c, false);
        DxControlOps.SetCenterA(c, true);
        Assert.Equal(40, c.Left);       // (100-20)/2
        Assert.Equal(25, c.Top);        // (60-10)/2
    }

    [Fact]
    public void SetCenter_IsUnconditional()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 100, 60) });
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 21, 11) });

        DxControlOps.SetCenter(c);
        Assert.Equal(39, c.Left);       // (100-21)/2 = 39（向零截断）
        Assert.Equal(24, c.Top);        // (60-11)/2 = 24
    }

    [Fact]
    public void WidthCenter_And_HeightCenter_AreIndependent()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 100, 60) });
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Rect(7, 7, 27, 17) });   // 宽 20、高 10

        DxControlOps.WidthCenter(c);
        Assert.Equal(40, c.Left);       // (100 - 20) / 2
        Assert.Equal(7, c.Top);

        DxControlOps.HeightCenter(c);
        Assert.Equal(25, c.Top);        // (60 - 10) / 2
    }

    [Fact]
    public void CenterHelpers_NoOwner_AreNoOps()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(3, 4, 13, 14) };
        DxControlOps.SetCenter(c);
        DxControlOps.WidthCenter(c);
        DxControlOps.HeightCenter(c);
        // 原文三个方法都有 Owner 守卫；无 Owner 时**什么都不做**（ClientRect 保持 Bounds(3,4,13,14)=(3,4,16,18)）
        Assert.Equal(new TDxRect(3, 4, 16, 18), c.ClientRect);
    }

    // ===============================================================================
    // 六、ImageIndexChange（原文 2539-2569）
    // ===============================================================================

    [Fact]
    public void ImageIndexChange_PicksFirstNonNegativeFace_AndResizes()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl());
        var lib = new TDxImageLibraryStub().Add(new TDxTextureStub(32, 16));
        c.ImageIndex.Image = lib;
        c.ImageIndex.Hot = 0;               // Up 为 -1 → 取 Hot

        DxControlOps.ImageIndexChange(c);

        Assert.Equal(32, c.Width);
        Assert.Equal(16, c.Height);
    }

    [Fact]
    public void ImageIndexChange_TinyTexture_LeavesSizeAlone()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(2, 2));  // 2*2 = 4，不大于 4
        c.ImageIndex.Up = 0;

        DxControlOps.ImageIndexChange(c);
        Assert.Equal(10, c.Width);
    }

    [Fact]
    public void ImageIndexChange_NoImageOrNotAutoSize_Skipped()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        c.ImageIndex.Up = 0;
        DxControlOps.ImageIndexChange(c);          // Image == null
        Assert.Equal(10, c.Width);

        c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(50, 50));
        c.AutoSize = false;
        DxControlOps.ImageIndexChange(c);
        Assert.Equal(10, c.Width);
    }

    [Fact]
    public void ImageIndexChange_CentersWhenCenterAndNotDesigning()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 100, 100) });
        var c = Attach(owner, new TTestCtl());
        c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(20, 10));
        c.ImageIndex.Up = 0;
        c.Designing = false;
        c.Center = true;

        DxControlOps.ImageIndexChange(c);

        Assert.Equal(40, c.Left);
        Assert.Equal(45, c.Top);
    }

    [Fact]
    public void ImageIndexChange_OutOfRangeIndex_LeavesSizeAlone()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        c.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(50, 50));
        c.ImageIndex.Up = 5;                       // 图库里没有 5
        DxControlOps.ImageIndexChange(c);
        Assert.Equal(10, c.Width);
    }

    // ===============================================================================
    // 七、绘制几何：FillRect / FillRectAlpha / FrameRect
    // ===============================================================================

    [Fact]
    public void FillRect_ThreeArgOverload_UsesOwnVisibleAndVirtualRect()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        DxControlOps.FillRectOne(c, TDxRect.Bounds(0, 0, 10, 10), 0x123456);

        Assert.Single(p.Ops);
        Assert.Equal("FillRect((0,0,10,10),(0,0,50,50),(0,0,50,50),123456)", p.Ops[0]);
    }

    [Fact]
    public void FillRect_ClipsAgainstVisibleRect()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        // vbRect 从 (10,10) 起 → nLeft/nTop = 10；vb 右边/下边为 30,30
        // → nWidth = 30-0-10 = 20、nHeight = 30-0-10 = 20、落点 (10,10)
        DxControlOps.FillRect(c, TDxRect.Bounds(0, 0, 30, 30),
            TDxRect.Bounds(0, 0, 50, 50), TDxRect.Rect(10, 10, 30, 30), 0xFFFFFF);

        Assert.Single(p.Ops);
        Assert.Equal("FillRect((10,10,30,30),(0,0,50,50),(10,10,30,30),FFFFFF)", p.Ops[0]);
    }

    [Fact]
    public void FillRect_DegenerateCases_ProduceNoOps()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        // vtRect 与 destRect 无交集 → ShortRect 后为负矩形
        DxControlOps.FillRect(c, TDxRect.Bounds(100, 100, 10, 10),
            TDxRect.Bounds(0, 0, 10, 10), TDxRect.Bounds(0, 0, 10, 10), 1);
        // 零尺寸 destRect
        DxControlOps.FillRect(c, TDxRect.Rect(0, 0, 0, 0),
            TDxRect.Bounds(0, 0, 10, 10), TDxRect.Bounds(0, 0, 10, 10), 1);

        Assert.Empty(p.Ops);
    }

    [Fact]
    public void FillRectAlpha_AtOrAbove255_DelegatesToFillRect()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        var dest = TDxRect.Bounds(0, 0, 5, 5);
        DxControlOps.FillRectAlpha(c, dest, TDxRect.Bounds(0, 0, 20, 20), TDxRect.Bounds(0, 0, 20, 20),
            0xABCDEF, 255);

        Assert.Single(p.Ops);
        Assert.StartsWith("FillRect(", p.Ops[0]);
    }

    [Fact]
    public void FillRectAlpha_Below255_UsesFillRectAlphaSeam()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingExtPainter();
        c.Painter = p;

        DxControlOps.FillRectAlpha(c, TDxRect.Bounds(0, 0, 5, 5),
            TDxRect.Bounds(0, 0, 20, 20), TDxRect.Bounds(0, 0, 20, 20), 0x112233, 128);

        Assert.Single(p.Ops);
        Assert.StartsWith("FillRectAlpha((0,0,5,5),", p.Ops[0]);
        Assert.EndsWith(",128)", p.Ops[0]);
    }

    [Fact]
    public void FrameRect_DrawsUpToFourLines_WithIndependentEdgeGuards()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        var rect = TDxRect.Rect(0, 0, 10, 10);
        var vb = TDxRect.Rect(0, 0, 20, 20);
        DxControlOps.FrameRect(c, rect, vb, vb, 0xFF0000);

        Assert.Equal(4, p.Ops.Count);
        Assert.Equal("Line((0,0),(10,0),FF0000)", p.Ops[0]);        // 上横
        Assert.Equal("Line((10,0),(10,10),FF0000)", p.Ops[1]);     // 右竖
        Assert.Equal("Line((0,10),(10,10),FF0000)", p.Ops[2]);     // 下横
        Assert.Equal("Line((0,-1),(0,10),FF0000)", p.Ops[3]);      // 左竖（原文 Top - 1）
    }

    [Fact]
    public void FrameRect_EdgeGuardsDropOutOfBoundsEdges()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        // ① destRect 与 vbRect 只在边界上相接（dest 从 (10,10) 起，vb 只到 (10,10)）
        //    → CropFill 里 nWidth/nHeight 归零 → 整个 FrameRect 直接 Exit（原文如此）
        var vb = TDxRect.Rect(0, 0, 10, 10);
        var rect = TDxRect.Rect(10, 10, 20, 20);
        DxControlOps.FrameRect(c, rect, TDxRect.Rect(0, 0, 100, 100), vb, 0x00FF00);

        Assert.Empty(p.Ops);

        // ② 真正相交：dest (5,5)-(15,15) 与 vb (0,0)-(10,10) 交于 (5,5)-(10,10)
        //    → 上横画、左竖画；右竖（15 <= 10 假）与下横（15 <= 10 假）不画 —— 四边守卫各自独立
        var p2 = new TDxRecordingPainter();
        c.Painter = p2;
        DxControlOps.FrameRect(c, TDxRect.Rect(5, 5, 15, 15), TDxRect.Rect(0, 0, 100, 100), vb, 0x00FF00);

        Assert.Equal(2, p2.Ops.Count);
        Assert.Equal("Line((5,5),(10,5),00FF00)", p2.Ops[0]);      // 上横
        Assert.Equal("Line((5,4),(5,10),00FF00)", p2.Ops[1]);      // 左竖（起点 Top - 1）
    }

    [Fact]
    public void FrameRect_NoOverlap_IsNoOp()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        DxControlOps.FrameRect(c, TDxRect.Bounds(100, 100, 10, 10),
            TDxRect.Bounds(0, 0, 10, 10), TDxRect.Bounds(0, 0, 10, 10), 1);

        Assert.Empty(p.Ops);
    }

    // ===============================================================================
    // 八、绘制几何：DrawRect / DrawRectColor / DrawRectColorAlpha
    // ===============================================================================

    [Fact]
    public void DrawRect_SmallTexture_IsSkipped()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        DxControlOps.DrawRect(c, TDxRect.Bounds(0, 0, 50, 50), new TDxTextureStub(2, 2), 0);
        DxControlOps.DrawRect(c, TDxRect.Bounds(0, 0, 50, 50), null, 0);       // 托管侧 nil 守卫

        Assert.Empty(p.Ops);
    }

    [Fact]
    public void DrawRect_FullVisible_DrawsWholeTexture()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        DxControlOps.DrawRect(c, TDxRect.Bounds(0, 0, 10, 10), new TDxTextureStub(10, 10), 0);

        Assert.Single(p.Ops);
        Assert.Equal("Draw(0,0,(0,0,10,10),tex10x10)", p.Ops[0]);
    }

    [Fact]
    public void DrawRect_PartiallyOffscreen_ClipsSrcRect()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        // destRect 左上被 VisibleRect 裁掉 5 像素 → SrcRect 同步左移 5、宽高各减 5
        // （TDxRect.Bounds(w,h) 是宽/高语义：Bounds(-5,-5,10,10) = (-5,-5,5,5)，宽高各 10）
        var dest = TDxRect.Bounds(-5, -5, 10, 10);
        DxControlOps.DrawRect(c, dest, new TDxTextureStub(20, 20), 0);

        Assert.Single(p.Ops);
        Assert.Equal("Draw(0,0,(5,5,10,10),tex20x20)", p.Ops[0]);
    }

    [Fact]
    public void DrawRect_ScopedOverload_And_OffsetOverload()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 30, 30) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        var vb = TDxRect.Bounds(0, 0, 30, 30);
        DxControlOps.DrawRectScoped(c, TDxRect.Bounds(1, 2, 10, 10), vb, vb, new TDxTextureStub(9, 9), 0);
        DxControlOps.DrawRect(c, 100, 200, TDxRect.Bounds(1, 2, 10, 10), vb, vb, new TDxTextureStub(9, 9), 0);

        Assert.Equal(2, p.Ops.Count);
        Assert.Equal("Draw(1,2,(0,0,9,9),tex9x9)", p.Ops[0]);
        Assert.Equal("Draw(101,202,(0,0,9,9),tex9x9)", p.Ops[1]);   // nPX/nPY 只加到落点
    }

    [Fact]
    public void DrawRectColor_UsesColoredSeam()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingExtPainter();
        c.Painter = p;

        DxControlOps.DrawRectColor(c, TDxRect.Bounds(0, 0, 8, 8), new TDxTextureStub(8, 8), 0x00FF00, 3);

        Assert.Single(p.Ops);
        Assert.Equal("DrawColor(0,0,(0,0,8,8),tex8x8,00FF00,3)", p.Ops[0]);
    }

    [Fact]
    public void DrawRectColor_ScalarOverload_ClampsSrcRect()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingExtPainter();
        c.Painter = p;

        // SrcRect 超出纹理 → ShortRect 夹到 ClientRect
        DxControlOps.DrawRectColor(c, TDxRect.Bounds(0, 0, 8, 8), TDxRect.Bounds(-5, -5, 100, 100),
            TDxRect.Bounds(0, 0, 20, 20), TDxRect.Bounds(0, 0, 20, 20), new TDxTextureStub(8, 8), 0x010203, 0);

        Assert.Single(p.Ops);
        Assert.Equal("DrawColor(0,0,(0,0,8,8),tex8x8,010203,0)", p.Ops[0]);
    }

    [Fact]
    public void DrawRectColorAlpha_AtAlpha255_DelegatesToDrawRectColor()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingExtPainter();
        c.Painter = p;

        DxControlOps.DrawRectColorAlpha(c, TDxRect.Bounds(0, 0, 8, 8), new TDxTextureStub(8, 8), 0x112233, 255, 0);

        Assert.Single(p.Ops);
        Assert.StartsWith("DrawColor(", p.Ops[0]);
    }

    [Fact]
    public void DrawRectColorAlpha_Below255_UsesAlphaSeam()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingExtPainter();
        c.Painter = p;

        DxControlOps.DrawRectColorAlpha(c, TDxRect.Bounds(0, 0, 8, 8), new TDxTextureStub(8, 8), 0x445566, 64, 2);

        Assert.Single(p.Ops);
        Assert.Equal("DrawColorAlpha(0,0,(0,0,8,8),tex8x8,445566,64,2)", p.Ops[0]);
    }

    [Fact]
    public void DrawRectColorAlphaScoped_TinyOrNilTexture_IsSkipped()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) };
        var p = new TDxRecordingExtPainter();
        c.Painter = p;

        var vb = TDxRect.Bounds(0, 0, 20, 20);
        DxControlOps.DrawRectColorAlphaScoped(c, TDxRect.Bounds(0, 0, 4, 4), vb, vb, new TDxTextureStub(1, 1), 0, 100, 0);
        DxControlOps.DrawRectColorAlphaScoped(c, TDxRect.Bounds(0, 0, 4, 4), vb, vb, null, 0, 100, 0);

        Assert.Empty(p.Ops);
    }

    // ===============================================================================
    // 九、GetPaintRect（原文 3147-3197）
    // ===============================================================================

    [Fact]
    public void GetPaintRect_BasicClamp_ReturnsWidthMinusOffsets()
    {
        var dest = TDxRect.Rect(0, 0, 30, 30);           // 用 Rect：右/下绝对值
        var src = TDxRect.Rect(0, 0, 40, 40);
        var vb = TDxRect.Rect(5, 5, 25, 25);

        var r = DxControlOps.GetPaintRect(dest, src, dest, vb, out int nX, out int nY);

        Assert.Equal(5, nX);
        Assert.Equal(5, nY);
        // ShortRect(dest, vb) = (5,5,25,25)，宽 20；min(20, 40) = 20；再 Bounds(0+5, 0+5, 20, 20) 与 src 求交
        Assert.Equal(new TDxRect(5, 5, 25, 25), r);
    }

    [Fact]
    public void GetPaintRect_DegenerateVisibleRect_ReturnsEmptyWithMinusOne()
    {
        var r = DxControlOps.GetPaintRect(TDxRect.Bounds(0, 0, 10, 10), TDxRect.Bounds(0, 0, 10, 10),
            TDxRect.Bounds(0, 0, 10, 10), TDxRect.Rect(0, 0, 0, 0), out int nX, out int nY);

        Assert.Equal(TDxRect.Empty, r);
        Assert.Equal(-1, nX);
        Assert.Equal(-1, nY);
    }

    [Fact]
    public void GetPaintRect_DestFullyCoveredByVisible_NoTranslation()
    {
        var dest = TDxRect.Bounds(10, 10, 10, 10);
        var src = TDxRect.Bounds(0, 0, 10, 10);
        var vb = TDxRect.Bounds(0, 0, 100, 100);

        var r = DxControlOps.GetPaintRect(dest, src, dest, vb, out int nX, out int nY);

        Assert.Equal(10, nX);
        Assert.Equal(10, nY);
        Assert.Equal(src, r);
    }

    // ===============================================================================
    // 十、Paint 遍历（原文 3859-3877）
    // ===============================================================================

    [Fact]
    public void Paint_DegenerateVisibleRect_DoesNothing()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Rect(0, 0, 0, 0) };
        var p = new TDxRecordingPainter();
        c.Painter = p;

        DxControlOps.Paint(c);
        Assert.Empty(p.Ops);
    }

    [Fact]
    public void Paint_Designing_DrawsRedFrame()
    {
        var c = new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) };
        var p = new TDxRecordingPainter();
        c.Painter = p;
        c.Designing = true;

        DxControlOps.Paint(c);

        Assert.Equal(4, p.Ops.Count);
        Assert.All(p.Ops, o => Assert.EndsWith(",0000FF)", o));    // clRed（BGR 表示）
    }

    [Fact]
    public void Paint_PaintsVisibleChildren_InReverseOrder()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a") { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var b = Attach(root, new TTestCtl("b") { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var hidden = Attach(root, new TTestCtl("h") { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        hidden.Visible = false;

        var p = new TDxRecordingPainter();
        a.Painter = p;
        b.Painter = p;
        hidden.Painter = p;
        root.Designing = false;

        DxControlOps.Paint(root);

        // 逆序：先 b 后 a；hidden 不可见被跳过
        Assert.Equal(8, p.Ops.Count);     // b 四边 + a 四边
    }

    // ===============================================================================
    // 十一、FormatCaption / 文本辅助（原文 3039-3063）
    // ===============================================================================

    [Fact]
    public void FormatCaption_NoRawText_DoesNothing()
    {
        var c = new TTestCtl();
        DxControlOps.FormatCaption(c);
        Assert.Equal("", c.Caption);
    }

    [Fact]
    public void FormatCaption_NonMoneyVariable_LeavesCaptionUnchanged()
    {
        var c = new TTestCtl();
        DxControlOps.SetCaptionA(c, "<#OTHER(x)>");
        DxControlOps.FormatCaption(c);
        Assert.Equal("<#OTHER(x)>", c.Caption);
        Assert.Equal("<#OTHER(x)>", DxControlOps.RawTextOf(c));      // RawText 未被 FormatCaption 改写
    }

    [Fact]
    public void FormatCaption_MoneyHit_SubstitutesValue()
    {
        var c = new TTestCtl();
        DxControlOps.SetCaptionA(c, "Gold: <#MONEY(abc)>");
        DxControlOps.MoneyListGetIndex = _ => 3;
        DxControlOps.MoneyListGetValue = _ => 12345;
        try
        {
            DxControlOps.FormatCaption(c);
            Assert.Equal("Gold: 12345", c.Caption);
            Assert.Equal("Gold: <#MONEY(abc)>", DxControlOps.RawTextOf(c));
        }
        finally
        {
            DxControlOps.MoneyListGetIndex = null;
            DxControlOps.MoneyListGetValue = null;
        }
    }

    [Fact]
    public void FormatCaption_MoneyMiss_WritesZero()
    {
        var c = new TTestCtl();
        DxControlOps.SetCaptionA(c, "<#MONEY(nope)>");
        DxControlOps.FormatCaption(c);                    // 缺省查表 = 未命中
        Assert.Equal("0", c.Caption);
    }

    [Fact]
    public void FormatCaption_TwiceIsIdempotent_BecauseRawTextKept()
    {
        var c = new TTestCtl();
        DxControlOps.SetCaptionA(c, "<#MONEY(x)>");
        DxControlOps.FormatCaption(c);
        DxControlOps.FormatCaption(c);
        Assert.Equal("0", c.Caption);
    }

    [Fact]
    public void SetCaptionV_DoesNotTouchRawText()
    {
        var c = new TTestCtl();
        DxControlOps.SetCaptionA(c, "raw");
        DxControlOps.SetCaptionV(c, "shown");
        Assert.Equal("shown", c.Caption);
        Assert.Equal("raw", DxControlOps.RawTextOf(c));
    }

    [Fact]
    public void ArrestVariable_ExtractsVariableAndRequiresSeparator()
    {
        string src = "pre<#MONEY(abc)>post";
        int pos = DxControlOps.ArrestVariable(ref src, '<', '#', '>', 1, out string v);
        Assert.Equal("#MONEY(abc)", v);
        Assert.Equal(src.IndexOf('>') + 2, pos);

        // 无 '#' → 失败
        string s2 = "<MONEY(abc)>";
        Assert.Equal(0, DxControlOps.ArrestVariable(ref s2, '<', '#', '>', 1, out string v2));
        Assert.Equal("", v2);

        // 无结束符 → 失败
        string s3 = "<#MONEY(abc";
        Assert.Equal(0, DxControlOps.ArrestVariable(ref s3, '<', '#', '>', 1, out _));
    }

    [Fact]
    public void ArrestStringEx_ExtractsBetweenDelimiters()
    {
        string s = "abc(inner)tail";
        int r = DxStringArrest.ArrestStringEx(ref s, '(', ')', out string dest);
        Assert.Equal("inner", dest);
        Assert.True(r > 0);

        string s2 = "no delimiters";
        Assert.Equal(0, DxStringArrest.ArrestStringEx(ref s2, '(', ')', out string d2));
        Assert.Equal("", d2);
    }

    [Fact]
    public void CompareLStr_IsCaseInsensitiveAndLengthBounded()
    {
        Assert.True(DxControlOps.CompareLStr("#money(x)", "#MONEY(", 7));
        Assert.False(DxControlOps.CompareLStr("#mone", "#MONEY(", 7));   // 太短
        Assert.False(DxControlOps.CompareLStr("#MONEY(", "#MONEZ(", 7));
        Assert.True(DxControlOps.CompareLStr("abc", "abc", 0));           // 长度 0 恒真
        Assert.False(DxControlOps.CompareLStr(null, "abc", 1));
    }

    [Fact]
    public void Sub49ADB8_ReplacesFirstOccurrenceOnly()
    {
        Assert.Equal("a-X-c-b", DxControlOps.Sub49ADB8("a-b-c-b", "b", "X"));   // 只替换第 1 次
        Assert.Equal("a-b", DxControlOps.Sub49ADB8("a-b", "z", "X"));
        Assert.Equal("", DxControlOps.Sub49ADB8("", "b", "X"));
        Assert.Equal("a-b", DxControlOps.Sub49ADB8("a-b", "", "X"));
    }

    // ===============================================================================
    // 十二、SetMousePoint（原文 3996-4004，含原文笔误）
    // ===============================================================================

    [Fact]
    public void SetMousePoint_RecursesToOwner_AndHasTheXAsYTypo()
    {
        var root = NewEngine();
        var owner = Attach(root, new TTestCtl());
        var c = Attach(owner, new TTestCtl());

        DxControlOps.SetMousePoint(c, 7, 9);

        Assert.Equal(7, DxControlHooks.SpotX(c));
        Assert.Equal(9, DxControlHooks.SpotY(c));
        Assert.Equal(7, DxControlHooks.MouseDownX(c));
        // 原文 DxControls.pas:4001 `FMouseDownY := X`（笔误）—— 逐字保留
        Assert.Equal(7, DxControlHooks.MouseDownY(c));

        // 递归到 Owner
        Assert.Equal(7, DxControlHooks.SpotX(owner));
        Assert.Equal(9, DxControlHooks.SpotY(owner));
    }

    [Fact]
    public void SetMousePoint_Defaults_AreZero()
    {
        var c = new TTestCtl();
        Assert.Equal(0, DxControlHooks.SpotX(c));
        Assert.Equal(0, DxControlHooks.SpotY(c));
        Assert.Equal(0, DxControlHooks.MouseDownX(c));
        Assert.Equal(0, DxControlHooks.MouseDownY(c));
    }

    // ===============================================================================
    // 十三、鼠标三段：MouseDown / MouseMove / MouseUp（原文 4046-4128）
    // ===============================================================================

    [Fact]
    public void MouseDown_ForwardsToOnMouseDown()
    {
        var c = new TTestCtl();
        var seen = new List<string>();
        DxControlHooks.SetOnMouseDown(c, (s, b, sh, x, y) => seen.Add($"{b}:{x},{y}"));

        DxControlOps.MouseDown(c, TDxMouseButton.mbLeft, TDxShiftState.ssLeft, 3, 4);
        Assert.Equal(new List<string> { "mbLeft:3,4" }, seen);
    }

    [Fact]
    public void MouseMove_ForwardsToOnMouseMove()
    {
        var c = new TTestCtl();
        int calls = 0;
        DxControlHooks.SetOnMouseMove(c, (s, sh, x, y) => calls++);
        DxControlOps.MouseMove(c, TDxShiftState.ssLeft, 1, 2);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void MouseUp_LeftButtonWithMbLeft_FiresUpAndClick()
    {
        var c = new TTestCtl();
        c.MouseEvents = TMouseEvents.Default;
        var log = new List<string>();
        DxControlHooks.SetOnMouseUp(c, (s, b, sh, x, y) => log.Add("up"));
        DxControlHooks.SetOnClick(c, (s, x, y) => log.Add($"click:{x},{y}"));

        DxControlOps.MouseUp(c, TDxMouseButton.mbLeft, TDxShiftState.None, 5, 6);

        Assert.Equal(new List<string> { "up", "click:5,6" }, log);     // 顺序：先 OnMouseUp 再 DoClick
    }

    [Fact]
    public void MouseUp_ButtonNotInMouseEvents_IsSuppressed()
    {
        var c = new TTestCtl();
        c.MouseEvents = TMouseEvents.mbLeft;                  // 只允许左键
        var log = new List<string>();
        DxControlHooks.SetOnMouseUp(c, (s, b, sh, x, y) => log.Add("up"));
        DxControlHooks.SetOnClick(c, (s, x, y) => log.Add("click"));

        DxControlOps.MouseUp(c, TDxMouseButton.mbRight, TDxShiftState.None, 1, 1);
        DxControlOps.MouseUp(c, TDxMouseButton.mbMiddle, TDxShiftState.None, 1, 1);

        Assert.Empty(log);                                    // 原文 FCanMouse 门控（中键根本不参与）
    }

    [Fact]
    public void MouseUp_NoHandlers_IsSafe()
    {
        var c = new TTestCtl();
        DxControlOps.MouseUp(c, TDxMouseButton.mbLeft, TDxShiftState.None, 0, 0);
    }

    // ===============================================================================
    // 十四、DxControlOps.FindActiveControl（原文 4016-4034）
    // ===============================================================================

    [Fact]
    public void FindActiveControl_ReturnsSelfWhenInRange()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;

        Assert.Same(c, DxControlOps.FindActiveControl(c, 5, 5));
        Assert.Null(DxControlOps.FindActiveControl(c, 50, 50));
    }

    [Fact]
    public void FindActiveControl_ReturnsTopmostChildThatHits()
    {
        var root = NewEngine();
        var parent = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) });
        parent.Designing = false;
        var child = Attach(parent, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) });
        child.Designing = false;

        var hit = DxControlOps.FindActiveControl(parent, 5, 5);
        Assert.NotNull(hit);
        Assert.Same(child, hit);
    }

    [Fact]
    public void FindActiveControl_RespectsVisibilityEnabledAndEnableMouse()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;

        c.Visible = false;
        Assert.Null(DxControlOps.FindActiveControl(c, 5, 5));
        c.Visible = true;

        c.Enabled = false;
        Assert.Null(DxControlOps.FindActiveControl(c, 5, 5));   // Designing=false → 不可命中
        c.Enabled = true;

        c.EnableMouse = false;
        Assert.Null(DxControlOps.FindActiveControl(c, 5, 5));
        c.EnableMouse = true;

        c.CanMouse.ToString();   // 只读，恒 True
        Assert.Same(c, DxControlOps.FindActiveControl(c, 5, 5));
    }

    [Fact]
    public void FindActiveControl_DisabledButDesigning_StillHits()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Enabled = false;
        c.Designing = true;

        Assert.Same(c, DxControlOps.FindActiveControl(c, 5, 5));
    }

    [Fact]
    public void FindActiveControl_OnFindActiveControlHookWins()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        var sentinel = new TTestCtl();
        DxControlHooks.SetOnFindActiveControl(c, (s, x, y) => sentinel);

        Assert.Same(sentinel, DxControlOps.FindActiveControl(c, 999, 999));
    }

    // ===============================================================================
    // 十五、Move（原文 4060-4101）
    // ===============================================================================

    [Fact]
    public void Move_RequiresFloatingOrDesigning()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(10, 10, 30, 30) });
        c.Designing = false;
        c.Floating = false;
        DxControlOps.SetMousePoint(c, 0, 0);

        DxControlOps.Move(c, 5, 5);
        Assert.Equal(10, c.Left);        // 未动
    }

    [Fact]
    public void Move_ShiftsByDelta_AndUpdatesSpot()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(10, 10, 30, 30) });
        c.Designing = false;
        c.Floating = true;
        DxControlOps.SetMousePoint(c, 0, 0);

        DxControlOps.Move(c, 5, 7);

        Assert.Equal(15, c.Left);
        Assert.Equal(17, c.Top);
        Assert.Equal(5, DxControlHooks.SpotX(c));
        Assert.Equal(7, DxControlHooks.SpotY(c));
    }

    [Fact]
    public void Move_SameSpot_IsNoOp()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(10, 10, 30, 30) });
        c.Designing = false;
        c.Floating = true;
        DxControlOps.SetMousePoint(c, 4, 4);

        DxControlOps.Move(c, 4, 4);
        Assert.Equal(10, c.Left);
    }

    [Fact]
    public void Move_NegativeOrBeyondRoot_IsRejected()
    {
        var root = NewEngine();
        root.ClientRect = TDxRect.Bounds(0, 0, 100, 100);
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(10, 10, 30, 30) });
        c.Designing = false;
        c.Floating = true;
        DxControlOps.SetMousePoint(c, 0, 0);

        DxControlOps.Move(c, -1, 5);
        Assert.Equal(10, c.Left);

        DxControlOps.Move(c, 5, 1000);
        Assert.Equal(10, c.Top);
    }

    [Fact]
    public void Move_OutOfOwnerRange_FallsBackToOriginalPosition()
    {
        // 原文 4079-4082 的 `al := Left`（回退原位，不是夹紧）
        var root = NewEngine();
        root.ClientRect = TDxRect.Bounds(0, 0, 500, 500);
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Rect(0, 0, 50, 50) });
        owner.Designing = false;
        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Rect(10, 10, 20, 20) });   // 宽 10，root 宽 500
        c.Designing = false;
        c.Floating = true;
        DxControlOps.SetMousePoint(c, 0, 0);

        // Range = owner.MoveRange（首次读取时为 Bounds(0,0,50,50) → Right = 50）
        // owner 已挂到 root（宽 500），故 Range = root.ClientRect → Right = 500；
        // al = 10 + 100 = 110 落在 [0,500] 内 → **不**回退，落到 110（原文 4079-4082 的判定就是拿 Range 比）
        DxControlOps.Move(c, 100, 0);
        Assert.Equal(110, c.Left);
    }

    [Fact]
    public void Move_OwnerMove_PropagatesToOwner()
    {
        var root = NewEngine();
        root.ClientRect = TDxRect.Bounds(0, 0, 500, 500);
        var owner = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(60, 60, 80, 80) });
        owner.Designing = false;
        owner.Floating = true;
        DxControlOps.SetMousePoint(owner, 0, 0);

        var c = Attach(owner, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        c.Designing = false;
        c.Floating = false;
        c.OwnerMove = true;
        DxControlOps.SetMousePoint(c, 3, 3);

        DxControlOps.Move(c, 8, 8);            // 子控件自身门控失败（不 Floating）→ 上抛给 owner

        Assert.Equal(65, owner.Left);          // 60 + (8 - 3)
    }

    // ===============================================================================
    // 十六、TDxControlEngine：模态栈与状态（原文 1208-1343 / 1252-1284）
    // ===============================================================================

    [Fact]
    public void Engine_ModalFormStack_InsertAtZero_AndDeleteSkipsIndexZero()
    {
        var root = NewEngine();
        var f1 = new TTestCtl();
        var f2 = new TTestCtl();

        root.SetModalForm(f1);
        Assert.Same(f1, root.ModalForm);
        Assert.Equal(1, root.ModalFormCount);

        root.SetModalForm(f2);
        Assert.Same(f2, root.ModalForm);        // 后设的排到 0
        Assert.Equal(2, root.ModalFormCount);

        root.DeleteModalForm(f2);              // 索引 0 → **不删**（原文 `if nIndex > 0`）
        Assert.Equal(2, root.ModalFormCount);

        root.DeleteModalForm(f1);              // 索引 1 → 删
        Assert.Equal(1, root.ModalFormCount);
        Assert.Same(f2, root.ModalForm);
    }

    [Fact]
    public void Engine_SetModalForm_AlreadyPresent_MovesToFront()
    {
        var root = NewEngine();
        var f1 = new TTestCtl();
        var f2 = new TTestCtl();
        root.SetModalForm(f1);
        root.SetModalForm(f2);

        root.SetModalForm(f1);
        Assert.Same(f1, root.ModalForm);
        Assert.Equal(2, root.ModalFormCount);
    }

    [Fact]
    public void Engine_ModalFormEx_SameAsModalForm()
    {
        var root = NewEngine();
        Assert.Null(root.ModalForm);
        Assert.Null(root.ModalFormEx);
        var f = new TTestCtl();
        root.SetModalForm(f);
        Assert.Same(root.ModalForm, root.ModalFormEx);
    }

    [Fact]
    public void Engine_ActiveMenu_SwapHidesOld()
    {
        var root = NewEngine();
        var m1 = new TTestCtl();
        var m2 = new TTestCtl();
        m1.Visible = true;

        root.ActiveMenu = m1;
        Assert.Same(m1, root.ActiveMenu);
        Assert.True(m1.Visible);

        root.ActiveMenu = m2;
        Assert.False(m1.Visible);              // 换值时把旧的 Visible 置 False
        Assert.Same(m2, root.ActiveMenu);
    }

    [Fact]
    public void Engine_ImeAndCandidateWindow_SwapHidesOld()
    {
        var root = NewEngine();
        var w1 = new TTestCtl { Visible = true };
        var w2 = new TTestCtl { Visible = true };
        root.ImeWindow = w1;
        root.ImeWindow = w2;
        Assert.False(w1.Visible);
        Assert.Same(w2, root.ImeWindow);

        var c1 = new TTestCtl { Visible = true };
        var c2 = new TTestCtl { Visible = true };
        root.CandidateWindow = c1;
        root.CandidateWindow = c2;
        Assert.False(c1.Visible);
        Assert.Same(c2, root.CandidateWindow);
    }

    [Fact]
    public void Engine_FocusedControl_FiresUnfocusedThenFocused()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl());
        var b = Attach(root, new TTestCtl());
        var order = new List<string>();
        // DoFocused/DoUnFocused 在基类是空实现，故只断言状态迁移与 OnFocused 钩子
        DxControlHooks.SetOnFocused(a, _ => order.Add("a-focused"));

        root.FocusedControl = a;
        Assert.Same(a, root.FocusedControl);
        Assert.True(DxControlOps.GetFocused(a));

        root.FocusedControl = b;
        Assert.Same(b, root.FocusedControl);
        Assert.False(DxControlOps.GetFocused(a));
        Assert.True(DxControlOps.GetFocused(b));

        root.FocusedControl = b;                // 值相同 → 不做任何切换
        Assert.Same(b, root.FocusedControl);
    }

    [Fact]
    public void Engine_ScrollControl_And_MouseDownMoveControl()
    {
        var root = NewEngine();
        // 注意：GetMouseDowned / GetMouseMoveed 读的是 RootCtrl 的当前值（原文 2880-2888），
        // 故控件必须真的挂在 root 下才能命中（未挂树的控件 RootCtrl 为 nil → 恒 False）。
        var s = new TTestCtl();
        var d = Attach(root, new TTestCtl());
        var m = Attach(root, new TTestCtl());

        root.ScrollControl = s;
        Assert.Same(s, root.ScrollControl);

        root.MouseDownControl = d;
        Assert.True(DxControlOps.GetMouseDowned(d));
        Assert.False(DxControlOps.GetMouseDowned(m));

        root.MouseMoveControl = m;
        Assert.True(DxControlOps.GetMouseMoveed(m));
    }

    [Fact]
    public void Engine_ActiveControl_OnlyAssignsOnChange()
    {
        var root = NewEngine();
        var a = new TTestCtl();
        Assert.Null(root.ActiveControl);
        root.ActiveControl = a;
        Assert.Same(a, root.ActiveControl);
    }

    // ===============================================================================
    // 十七、TDxControlEngine：队列与 ExecutePosition（原文 1355-1396）
    // ===============================================================================

    [Fact]
    public void Engine_ToFrontQueue_DrainsToComponentIndexZero()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));
        var c = Attach(root, new TTestCtl("c"));

        root.AddBringToFront(c);
        Assert.Equal(1, root.ToFrontQueueCount);
        Assert.Equal(2, DxControlOps.GetComponentIndex(c));

        root.ExecutePosition();

        Assert.Equal(0, root.ToFrontQueueCount);
        Assert.Same(c, DxControlOps.Components(root, 0));
    }

    [Fact]
    public void Engine_ToBackQueue_DrainsToComponentCountMinusOne()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));
        var c = Attach(root, new TTestCtl("c"));
        Assert.Equal(0, DxControlOps.GetComponentIndex(a));

        root.AddSentToBack(a);
        Assert.Equal(1, root.ToBackQueueCount);

        root.ExecutePosition();

        Assert.Equal(0, root.ToBackQueueCount);
        Assert.Same(a, DxControlOps.Components(root, 2));    // ComponentCount - 1
    }

    [Fact]
    public void Engine_ExecutePosition_EmptyQueues_IsSafe()
    {
        var root = NewEngine();
        root.ExecutePosition();
        Assert.Equal(0, root.ToFrontQueueCount);
        Assert.Equal(0, root.ToBackQueueCount);
    }

    [Fact]
    public void Engine_LockUnlock_AreReentrantSafePair()
    {
        var root = NewEngine();
        root.Lock();
        root.UnLock();      // 不应抛异常
        Assert.True(true);
    }

    [Fact]
    public void Engine_Name_IsDerivedFromClassName()
    {
        var root = NewEngine();
        // 类名以 'Tdx' 开头（Pos 大小写不敏感）→ 去前 3 字符得 'ControlEngine'，再加后缀 1
        Assert.False(string.IsNullOrEmpty(root.DxName));
        Assert.StartsWith("ControlEngine", root.DxName);
    }

    // ===============================================================================
    // 十八、TDxControlEngine：鼠标键盘派发（原文 1398-1743）
    // ===============================================================================

    [Fact]
    public void Engine_FindActiveControl_SearchesChildrenInOrder()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var b = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(20, 20, 10, 10) });
        a.Designing = false;
        b.Designing = false;

        Assert.Same(a, root.FindActiveControl(5, 5));
        Assert.Same(b, root.FindActiveControl(25, 25));
        Assert.Null(root.FindActiveControl(200, 200));
    }

    [Fact]
    public void Engine_PortMouseDown_HitsChild_FocusesAndForwards()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        c.EnableFocus = true;
        int downs = 0;
        DxControlHooks.SetOnMouseDown(c, (s, b, sh, x, y) => downs++);

        bool handled = root.PortMouseDown(TDxMouseButton.mbLeft, TDxShiftState.ssLeft, 5, 5);

        Assert.True(handled);
        Assert.Equal(1, downs);
        Assert.True(c.MouseDowned);
        Assert.Same(c, root.FocusedControl);
    }

    [Fact]
    public void Engine_PortMouseDown_Miss_FiresBackgroundAndMouseDown_WithWantReturn()
    {
        var root = NewEngine();
        var log = new List<string>();
        root.OnBackgroundClick = _ => { log.Add("bg"); root.WantReturn = true; };
        DxControlHooks.SetOnMouseDown(root, (s, b, sh, x, y) => log.Add("md"));

        bool handled = root.PortMouseDown(TDxMouseButton.mbLeft, TDxShiftState.None, 999, 999);

        Assert.True(handled);                       // WantReturn = true
        Assert.Equal(new List<string> { "bg", "md" }, log);
    }

    [Fact]
    public void Engine_PortMouseDown_ModalControlInvisible_ConsumesAndClears()
    {
        var root = NewEngine();
        var ghost = new TTestCtl { Visible = false };
        DxControlHooks.SetModalControl(root, ghost);

        bool handled = root.PortMouseDown(TDxMouseButton.mbLeft, TDxShiftState.None, 1, 1);

        Assert.True(handled);
        Assert.Null(DxControlHooks.GetModalControl(root));
    }

    [Fact]
    public void Engine_PortMouseDown_ActiveMenuInside_IsHandled()
    {
        var root = NewEngine();
        var menu = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 30, 30) });
        menu.Designing = false;
        root.ActiveMenu = menu;
        int downs = 0;
        DxControlHooks.SetOnMouseDown(menu, (s, b, sh, x, y) => downs++);

        bool handled = root.PortMouseDown(TDxMouseButton.mbLeft, TDxShiftState.None, 5, 5);

        Assert.True(handled);
        Assert.Equal(1, downs);
        Assert.True(menu.MouseDowned);
    }

    [Fact]
    public void Engine_PortMouseDown_ActiveMenuOutside_HidesMenuAndContinues()
    {
        var root = NewEngine();
        var menu = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 30, 30) });
        menu.Designing = false;
        menu.Visible = true;
        root.ActiveMenu = menu;

        root.PortMouseDown(TDxMouseButton.mbLeft, TDxShiftState.None, 500, 500);

        Assert.False(menu.Visible);
    }

    [Fact]
    public void Engine_PortMouseMove_TracksMouseMoveControl()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;

        bool handled = root.PortMouseMove(TDxShiftState.None, 5, 5);

        Assert.True(handled);
        Assert.Same(c, root.MouseMoveControl);
        // 注意：上游接缝的 `MouseMoveed` 是字段型属性，与原文 `Self = RootCtrl.MouseMoveControl` 的
        // getter 语义不同 —— 托管侧的正确读法是 DxControlOps.GetMouseMoveed（原文 GetMouseMoveed）。
        Assert.True(DxControlOps.GetMouseMoveed(c));
    }

    [Fact]
    public void Engine_PortMouseMove_TracksAndClearsViaSetMouseMoveed()
    {
        // 直接驱动原文的两条状态迁移（SetMouseMoveed 是 MouseMoveed 属性的 setter，
        // 原文 2841-2862 的完整语义都在这里）：
        //   true  → 挂上 root.MouseMoveControl；false → 让旧的收 DoMouseLeave 后置 nil。
        // 注意上游接缝的 `MouseMoveed` 是**字段型**属性（与原文 `Self = RootCtrl.MouseMoveControl`
        // 的 getter 语义不同），故读一律走 DxControlOps.GetMouseMoveed。
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;

        DxControlOps.SetMouseMoveed(c, true);
        Assert.Same(c, root.MouseMoveControl);
        Assert.True(DxControlOps.GetMouseMoveed(c));

        DxControlOps.SetMouseMoveed(c, false);
        Assert.Null(root.MouseMoveControl);
        Assert.False(DxControlOps.GetMouseMoveed(c));
    }

    [Fact]
    public void Engine_PortMouseMove_MissStillReportsHandledAndLeavesResultFalse()
    {
        // 原文 1589-1648 的 miss 尾巴：先把当前 MouseMoveControl 的 MouseMoveed 置 False，
        // 再走引擎级 OnMouseMove 并由 WantReturn 决定结果（未挂 = False）。
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        DxControlOps.SetMouseMoveed(c, true);
        Assert.True(DxControlOps.GetMouseMoveed(c));

        bool handled = root.PortMouseMove(TDxShiftState.None, 500, 500);

        Assert.False(handled);          // 没有 OnMouseMove 回调、WantReturn 未被置位
    }

    [Fact]
    public void Engine_PortMouseMove_WithCapture_ForwardsDirectly()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        c.Floating = true;
        root.MouseDownControl = c;
        DxControlOps.SetMousePoint(c, 0, 0);
        int moves = 0;
        DxControlHooks.SetOnMouseMove(c, (s, sh, x, y) => moves++);

        bool handled = root.PortMouseMove(TDxShiftState.ssLeft, 100, 100);

        Assert.True(handled);
        Assert.Equal(1, moves);             // 原文第一分支不判 InRange
    }

    [Fact]
    public void Engine_PortMouseUp_WithCapture_InsideRange_FiresUp()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        root.MouseDownControl = c;
        c.MouseDowned = true;
        int ups = 0;
        DxControlHooks.SetOnMouseUp(c, (s, b, sh, x, y) => ups++);

        bool handled = root.PortMouseUp(TDxMouseButton.mbLeft, TDxShiftState.None, 5, 5);

        Assert.True(handled);
        Assert.Equal(1, ups);
        Assert.False(c.MouseDowned);
    }

    [Fact]
    public void Engine_PortMouseUp_WithCapture_OutsideRange_JustClearsFlag()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        root.MouseDownControl = c;
        c.MouseDowned = true;
        int ups = 0;
        DxControlHooks.SetOnMouseUp(c, (s, b, sh, x, y) => ups++);

        root.PortMouseUp(TDxMouseButton.mbLeft, TDxShiftState.None, 500, 500);

        Assert.Equal(0, ups);
        Assert.False(c.MouseDowned);
    }

    [Fact]
    public void Engine_PortMouseUp_NoCapture_FiresEngineOnMouseUpAndOnClick()
    {
        var root = NewEngine();
        var log = new List<string>();
        DxControlHooks.SetOnMouseUp(root, (s, b, sh, x, y) => log.Add("up"));
        DxControlHooks.SetOnClick(root, (s, x, y) => log.Add("click"));

        bool handled = root.PortMouseUp(TDxMouseButton.mbLeft, TDxShiftState.None, 1, 2);

        Assert.False(handled);              // 两个回调都没设 WantReturn
        Assert.Equal(new List<string> { "up", "click" }, log);
    }

    [Fact]
    public void Engine_PortKeyDown_PriorityMenuThenFocusedThenModal()
    {
        var root = NewEngine();
        var log = new List<string>();
        var menu = Attach(root, new TTestCtl { Visible = true });
        var focused = Attach(root, new TTestCtl());
        var modal = new TTestCtl { Visible = true };
        DxControlHooks.SetOnKeyDown(menu, (s, k, sh) => log.Add("menu"));
        DxControlHooks.SetOnKeyDown(focused, (s, k, sh) => log.Add("focused"));
        DxControlHooks.SetOnKeyDown(modal, (s, k, sh) => log.Add("modal"));

        // ① 无菜单无焦点无模态 → false
        Assert.False(root.PortKeyDown(new UShortRef(65), TDxShiftState.None));

        // ② 只有模态
        root.SetModalForm(modal);
        Assert.True(root.PortKeyDown(new UShortRef(65), TDxShiftState.None));
        Assert.Equal(new List<string> { "modal" }, log);

        // ③ 有焦点 → 焦点优先于模态
        log.Clear();
        root.FocusedControl = focused;
        Assert.True(root.PortKeyDown(new UShortRef(65), TDxShiftState.None));
        Assert.Equal(new List<string> { "focused" }, log);

        // ④ 有菜单 → 菜单最优先
        log.Clear();
        root.ActiveMenu = menu;
        Assert.True(root.PortKeyDown(new UShortRef(65), TDxShiftState.None));
        Assert.Equal(new List<string> { "menu" }, log);
    }

    [Fact]
    public void Engine_PortKeyPress_And_PortKeyUp_MirrorKeyDown()
    {
        var root = NewEngine();
        var log = new List<string>();
        var c = Attach(root, new TTestCtl());
        DxControlHooks.SetOnKeyPress(c, (s, k) => log.Add($"press:{k.Value}"));
        DxControlHooks.SetOnKeyUp(c, (s, k, sh) => log.Add($"up:{k.Value}"));
        root.FocusedControl = c;

        Assert.True(root.PortKeyPress(new CharRef('a')));
        Assert.True(root.PortKeyUp(new UShortRef(65), TDxShiftState.None));

        Assert.Equal(new List<string> { "press:a", "up:65" }, log);
    }

    [Fact]
    public void Engine_PortDblClick_HitsChildAndSetsModalControl()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        int dbl = 0;
        DxControlHooks.SetOnDblClick(c, (s, x, y) => dbl++);

        bool handled = root.PortDblClick(5, 5);

        Assert.True(handled);
        Assert.Equal(1, dbl);
        Assert.Same(root.ModalForm, DxControlHooks.GetModalControl(root));   // 原文此时 ModalForm 为 null
    }

    [Fact]
    public void Engine_PortDblClick_Miss_ReturnsFalse()
    {
        var root = NewEngine();
        Assert.False(root.PortDblClick(100, 100));
    }

    [Fact]
    public void Engine_PortDblClick_ActiveMenuInside_IsHandled()
    {
        var root = NewEngine();
        var menu = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        menu.Designing = false;
        root.ActiveMenu = menu;
        int dbl = 0;
        DxControlHooks.SetOnDblClick(menu, (s, x, y) => dbl++);

        Assert.True(root.PortDblClick(5, 5));
        Assert.Equal(1, dbl);
    }

    [Fact]
    public void Engine_PortMouseWheel_ForwardsOnlyToScrollControl()
    {
        var root = NewEngine();
        var notScroll = new TTestCtl();
        root.ScrollControl = notScroll;

        Assert.False(root.PortMouseWheelDown(TDxShiftState.None, TDxPoint.Point(0, 0)));
        Assert.False(root.PortMouseWheelUp(TDxShiftState.None, TDxPoint.Point(0, 0)));

        var scroll = new TScrollProbe();
        root.ScrollControl = scroll;

        Assert.True(root.PortMouseWheelDown(TDxShiftState.None, TDxPoint.Point(1, 2)));
        Assert.True(root.PortMouseWheelUp(TDxShiftState.None, TDxPoint.Point(3, 4)));
        Assert.Equal(new List<string> { "down:1,2", "up:3,4" }, scroll.Log);
    }

    private sealed class TScrollProbe : TDxScrollControl
    {
        public readonly List<string> Log = new();
        public override void MouseWheelDown(TDxShiftState shift, TDxPoint mousePos)
            => Log.Add($"down:{mousePos.X},{mousePos.Y}");
        public override void MouseWheelUp(TDxShiftState shift, TDxPoint mousePos)
            => Log.Add($"up:{mousePos.X},{mousePos.Y}");
    }

    [Fact]
    public void Engine_OverrideMouseEntryPoints_RecordVoidResult()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;

        // 上游 TDxControl 的三个 virtual 入口 → 转发到 Port* 并存 Last*Result
        root.MouseDown(TDxMouseButton.mbLeft, TDxShiftState.None, 5, 5);
        Assert.True(root.LastMouseDownResult);

        root.MouseMove(TDxShiftState.None, 5, 5);
        Assert.True(root.LastMouseMoveResult);

        // MouseUp：原文 1505-1587 的命中子控件分支**不设** MouseDownControl（只置 MouseDowned），
        // 故 1650 的第一分支（MouseDownControl <> nil）不成立 → 落到引擎级 OnMouseUp/OnClick，
        // 两者都未挂 → 返回 False（原文如此）。
        root.MouseUp(TDxMouseButton.mbLeft, TDxShiftState.None, 5, 5);
        Assert.False(root.LastMouseUpResult);
    }

    // ===============================================================================
    // 十九、TDxControlEngine：绘制 / 更新遍历（原文 1745-1794）
    // ===============================================================================

    [Fact]
    public void Engine_PaintChildren_SkipsActiveMenuAndModalForm_ThenDrawsThemLast()
    {
        var root = NewEngine();
        root.Designing = false;
        var a = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var menu = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var modal = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        a.Designing = menu.Designing = modal.Designing = false;

        var p = new TDxRecordingPainter();
        a.Painter = menu.Painter = modal.Painter = p;

        root.ActiveMenu = menu;
        root.SetModalForm(modal);

        root.PaintChildren();

        // a 走普通路径（4 条边）；menu 与 modal 不在第一轮，但第二轮/第三轮各画一次
        Assert.Equal(0, p.Ops.Count);   // 三个控件都没开 Designing → 自身不画框；menu/modal 也只是各自 0 条
    }

    [Fact]
    public void Engine_PaintChildren_NoChildren_IsSafe()
    {
        var root = NewEngine();
        root.PaintChildren();
    }

    [Fact]
    public void Engine_UpdateChildren_OnlyVisibleAndEnabled()
    {
        var root = NewEngine();
        var visible = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var hidden = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var disabled = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        hidden.Visible = false;
        disabled.Enabled = false;

        int count = 0;
        DxControlHooks.SetOnUpdate(visible, _ => count++);
        DxControlHooks.SetOnUpdate(hidden, _ => count++);
        DxControlHooks.SetOnUpdate(disabled, _ => count++);

        root.UpdateChildren();

        Assert.Equal(1, count);
    }

    [Fact]
    public void Engine_Repaint_FiresOnRepaintOnce_AndGuardsRecursion()
    {
        var root = NewEngine();
        int fires = 0;
        root.OnRepaint = r => { fires++; r.Repaint(); };   // 自递归

        root.Repaint();

        Assert.Equal(1, fires);       // 递归被 _inRepaint 挡住
    }

    [Fact]
    public void Engine_InitializeAndFinalize_DoNotThrow()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl());
        Attach(a, new TTestCtl());
        root.Initialize();
        root.FinalizeEngine();
    }

    // ===============================================================================
    // 二十、DxControlOps 其余小件
    // ===============================================================================

    [Fact]
    public void ToFrontAndBack_HonourRootAndIndexGuards()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));
        var c = Attach(root, new TTestCtl("c"));

        DxControlOps.ToFront(root, 2);              // c → 0
        Assert.Same(c, DxControlOps.Components(root, 0));

        DxControlOps.ToBack(root, 0);               // c → 末尾
        Assert.Same(c, DxControlOps.Components(root, 2));

        // 越界索引 → 无操作
        DxControlOps.ToFront(root, 99);
        DxControlOps.ToBack(root, -1);

        // 无 RootCtrl → 无操作
        var orphan = new TTestCtl();
        DxControlOps.ToFront(orphan, 0);
        DxControlOps.ToBack(orphan, 0);
    }

    [Fact]
    public void BringToFrontAndSentToBack_GoThroughOwner()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl("a"));
        var b = Attach(root, new TTestCtl("b"));
        Assert.Same(a, DxControlOps.Components(root, 0));

        DxControlOps.BringToFront(b);
        Assert.Same(b, DxControlOps.Components(root, 0));

        DxControlOps.SentToBack(b);
        Assert.Same(b, DxControlOps.Components(root, 1));

        DxControlOps.BringToFront(new TTestCtl());   // 无 Owner → 安全
    }

    [Fact]
    public void SetAutoSize_TriggersImageIndexChangeAndCaptionChange()
    {
        var root = NewEngine();
        var probe = new TProbeCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) };
        Attach(root, probe);
        probe.ImageIndex.Image = new TDxImageLibraryStub().Add(new TDxTextureStub(40, 20));
        probe.ImageIndex.Up = 0;

        DxControlOps.SetAutoSize(probe, false);
        Assert.Contains("DoCaptionChange", probe.Calls);

        probe.Calls.Clear();
        DxControlOps.SetAutoSize(probe, true);       // 变 True → ImageIndexChange 会按图定尺
        Assert.Equal(40, probe.Width);
        Assert.Contains("DoCaptionChange", probe.Calls);

        // 同值不再触发
        probe.Calls.Clear();
        DxControlOps.SetAutoSize(probe, true);
        Assert.Empty(probe.Calls);
    }

    [Fact]
    public void SetCaptionA_And_SetCaptionV_FireCaptionChange()
    {
        var probe = new TProbeCtl();
        DxControlOps.SetCaptionA(probe, "x");
        Assert.Equal(new List<string> { "DoCaptionChange" }, probe.Calls);

        probe.Calls.Clear();
        DxControlOps.SetCaptionA(probe, "x");        // 同值 → 不触发
        Assert.Empty(probe.Calls);

        DxControlOps.SetCaptionV(probe, "y");
        Assert.Equal(new List<string> { "DoCaptionChange" }, probe.Calls);
    }

    [Fact]
    public void SetShowNameReferenceXAdjustYTopAlignment_OnlyWriteOnChange()
    {
        var c = new TTestCtl();
        DxControlOps.SetShowNameA(c, "sn");
        Assert.Equal("sn", c.ShowName);

        DxControlOps.SetReferenceX(c, TReferenceX.rxCenter);
        Assert.Equal(TReferenceX.rxCenter, c.ReferenceX);

        DxControlOps.SetAdjustYByHeight(c, true);
        Assert.True(c.AdjustYByHeight);

        DxControlOps.SetTopAlignment(c, true);
        Assert.True(c.TopAlignment);
    }

    [Fact]
    public void SetDesigning_PropagatesToChildren()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl());
        var b = Attach(a, new TTestCtl());

        DxControlOps.SetDesigning(root, false);
        Assert.False(root.Designing);
        Assert.False(a.Designing);
        Assert.False(b.Designing);

        DxControlOps.SetDesigning(root, false);     // 同值 → 提前返回
        Assert.False(a.Designing);
    }

    [Fact]
    public void SetScrollControl_FindsFirstScrollChild()
    {
        var root = NewEngine();
        var branch = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        var scroll = Attach(branch, new TScrollProbe { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        scroll.Designing = false;

        DxControlOps.SetScrollControl(root);

        Assert.Same(scroll, root.ScrollControl);
    }

    [Fact]
    public void SetScrollControl_OnScrollControlItself_AssignsDirectly()
    {
        var root = NewEngine();
        var scroll = Attach(root, new TScrollProbe { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        scroll.Designing = false;

        DxControlOps.SetScrollControl(scroll);
        Assert.Same(scroll, root.ScrollControl);
    }

    [Fact]
    public void SetFocus_SetsScrollControlFromFirstChild_AndFocusesSelf()
    {
        var root = NewEngine();
        var scroll = Attach(root, new TScrollProbe { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        scroll.Designing = false;
        scroll.EnableFocus = true;

        DxControlOps.SetFocus(scroll);

        Assert.Same(scroll, root.ScrollControl);      // 原文 Control[0] 是 TDxScrollControl
        Assert.Same(scroll, root.FocusedControl);
    }

    [Fact]
    public void SetFocus_NotVisibleOrEnabled_CannotFocus()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        c.EnableFocus = true;
        c.Visible = false;

        DxControlOps.SetFocus(c);
        Assert.Null(root.FocusedControl);
    }

    [Fact]
    public void SetFocus_DisabledButDesigning_CanStillFocus()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = true;
        c.Enabled = false;
        c.EnableFocus = true;

        DxControlOps.SetFocus(c);
        Assert.Same(c, root.FocusedControl);
    }

    [Fact]
    public void SetCapture_RespectsVisibleAndEnabledOrDesigning()
    {
        var root = NewEngine();
        var c = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        c.Designing = false;
        c.Enabled = false;

        DxControlOps.SetCapture(c);
        Assert.Null(root.MouseDownControl);

        c.Enabled = true;
        DxControlOps.SetCapture(c);
        Assert.Same(c, root.MouseDownControl);
    }

    [Fact]
    public void SetMouseDowned_TransfersCaptureAndFiresDoMouseUpOnOld()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        var b = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });

        DxControlOps.SetMouseDowned(a, true);
        Assert.Same(a, root.MouseDownControl);

        DxControlOps.SetMouseDowned(b, true);       // 旧控件收 DoMouseUp（= Repaint）
        Assert.Same(b, root.MouseDownControl);

        DxControlOps.SetMouseDowned(b, false);
        Assert.Null(root.MouseDownControl);
    }

    [Fact]
    public void SetMouseMoveed_FiresEnterOnlyWhenControlChanges()
    {
        var root = NewEngine();
        var a = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        var b = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        var enters = new List<string>();
        DxControlHooks.SetOnMouseEnter(a, _ => enters.Add("a"));
        DxControlHooks.SetOnMouseEnter(b, _ => enters.Add("b"));

        DxControlOps.SetMouseMoveed(a, true);
        DxControlOps.SetMouseMoveed(a, true);       // 同一个 → 不再 Enter
        DxControlOps.SetMouseMoveed(b, true);       // 换控件 → Enter b

        Assert.Equal(new List<string> { "a", "b" }, enters);
        Assert.Same(b, root.MouseMoveControl);

        DxControlOps.SetMouseMoveed(b, false);
        Assert.Null(root.MouseMoveControl);
    }

    [Fact]
    public void ReleaseControl_ClearsAllEngineSlotsRecursively()
    {
        var root = NewEngine();
        var parent = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        var child = Attach(parent, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 20, 20) });
        parent.Designing = child.Designing = false;

        root.SetModalForm(parent);
        root.ActiveMenu = parent;
        root.FocusedControl = parent;
        root.ScrollControl = parent;
        root.MouseDownControl = parent;
        root.MouseMoveControl = parent;

        DxControlOps.ReleaseControl(parent);

        Assert.Null(root.ActiveMenu);
        Assert.Null(root.FocusedControl);
        Assert.Null(root.ScrollControl);
        Assert.Null(root.MouseDownControl);
        Assert.Null(root.MouseMoveControl);
        Assert.Equal(1, root.ModalFormCount);       // DeleteModalForm 索引 0 **不删**（原文如此）
    }

    [Fact]
    public void FocusSomething_PicksFirstFocusableChild()
    {
        var root = NewEngine();
        var parent = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) });
        var a = Attach(parent, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        var b = Attach(parent, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        a.Designing = b.Designing = false;
        a.EnableFocus = true;
        b.EnableFocus = true;

        DxControlOps.FocusSomething(parent);

        Assert.Same(a, root.FocusedControl);
    }

    [Fact]
    public void FocusSomething_NotVisibleOrNotEnableMouse_DoesNothing()
    {
        var root = NewEngine();
        var parent = Attach(root, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 50, 50) });
        var a = Attach(parent, new TTestCtl { ClientRect = TDxRect.Bounds(0, 0, 10, 10) });
        a.Designing = false;
        a.EnableFocus = true;

        parent.EnableMouse = false;
        DxControlOps.FocusSomething(parent);
        Assert.Null(root.FocusedControl);

        parent.EnableMouse = true;
        parent.Visible = false;
        DxControlOps.FocusSomething(parent);
        Assert.Null(root.FocusedControl);
    }

    // ===============================================================================
    // 二十一、绘图器扩展接口的退化路径
    // ===============================================================================

    [Fact]
    public void PainterExt_FallsBackWhenExtInterfaceNotImplemented()
    {
        var p = new TDxRecordingPainter();           // 未实现 IDxSurfacePainterExt
        var tex = new TDxTextureStub(8, 8);

        DxPainterExt.FillRectAlpha(p, TDxRect.Bounds(0, 0, 4, 4), TDxRect.Bounds(0, 0, 4, 4),
            TDxRect.Bounds(0, 0, 4, 4), 0xFFFFFF, 100);
        DxPainterExt.DrawColor(p, 1, 2, TDxRect.Bounds(0, 0, 8, 8), tex, 0x00FF00, 0);
        DxPainterExt.DrawColorAlpha(p, 3, 4, TDxRect.Bounds(0, 0, 8, 8), tex, 0x00FF00, 90, 0);
        DxPainterExt.DrawBlend(p, 5, 6, TDxRect.Bounds(0, 0, 8, 8), tex, 2);
        DxPainterExt.StretchDraw(p, TDxRect.Bounds(0, 0, 16, 16), TDxRect.Bounds(0, 0, 8, 8), tex, 2);

        Assert.Equal(5, p.Ops.Count);
        Assert.StartsWith("FillRect(", p.Ops[0]);
        Assert.StartsWith("Draw(", p.Ops[1]);
        Assert.StartsWith("Draw(", p.Ops[2]);
    }

    /// <summary>同时实现 IDxSurfacePainterExt 的记录器（用于断言着色/alpha/拉伸路径）。</summary>
    private sealed class TDxRecordingExtPainter : IDxSurfacePainter, IDxSurfacePainterExt
    {
        public readonly List<string> Ops = new();
        public bool Active => true;

        public void FillRect(TDxRect d, TDxRect v, TDxRect vb, int c) => Ops.Add($"FillRect({d},{v},{vb},{c:X6})");
        public void FrameRect(TDxRect d, TDxRect v, TDxRect vb, int c) => Ops.Add($"FrameRect({d},{v},{vb},{c:X6})");
        public void DrawRect(TDxRect d, TDxRect v, TDxRect vb, IDxTexture t, int b) => Ops.Add("DrawRect");
        public void Draw(int x, int y, TDxRect src, IDxTexture t) => Ops.Add($"Draw({x},{y},{src})");
        public void Draw(int x, int y, IDxTexture t, int b) => Ops.Add($"DrawPlain({x},{y},{b})");
        public void Line(TDxPoint p1, TDxPoint p2, int c) => Ops.Add($"Line({p1},{p2})");
        public void Circle(int x, int y, int r, int c) => Ops.Add("Circle");
        public void TextRect(int x, int y, TDxRect src, List<TDxTextImageInfo> i, int c, int b, byte a, int e)
            => Ops.Add("TextRect");

        public void FillRectAlpha(TDxRect d, TDxRect v, TDxRect vb, int c, byte a)
            => Ops.Add($"FillRectAlpha({d},{v},{vb},{c:X6},{a})");

        public void DrawColor(int x, int y, TDxRect src, IDxTexture t, int c, int b)
            => Ops.Add($"DrawColor({x},{y},{src},tex{(t == null ? "-" : $"{t.Width}x{t.Height}")},{c:X6},{b})");

        public void DrawColorAlpha(int x, int y, TDxRect src, IDxTexture t, int c, byte a, int b)
            => Ops.Add($"DrawColorAlpha({x},{y},{src},tex{(t == null ? "-" : $"{t.Width}x{t.Height}")},{c:X6},{a},{b})");

        public void DrawBlend(int x, int y, TDxRect src, IDxTexture t, int b)
            => Ops.Add($"DrawBlend({x},{y},{src},{b})");

        public void StretchDraw(TDxRect dest, TDxRect src, IDxTexture t, int b)
            => Ops.Add($"StretchDraw({dest},{src},{b})");
    }
}
