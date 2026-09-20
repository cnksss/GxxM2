using System;
using System.Collections.Generic;

namespace GXX.Client.DxComponent;

// =============================================================================================
// DxMemo.pas（Source\Client-HGE\DxComponent\DxMemo.pas，5,877 行）1:1 移植 —— 第 1 片：
// TDxScrollControl（滚动控件基类）声明 + 实现。
//
//   覆盖行号：声明 44-192（含 82-129 私有字段、109-163 方法面、164-191 published 属性）
//             实现 670-733（Create/Destroy）+ 738-1490（全部方法体）
//
// 分片清单（全部落在 DxComponent/DxMemo*.cs 独占区）：
//   DxMemo.cs       ← 本文件：TScrollStyle + TDxScrollControl                    （DxMemo.pas 44-192 / 670-1490）
//   DxMemo.Box.cs   ← TDxScrollBox + TLineColor + TDxLines                       （DxMemo.pas 194-236 / 1494-1852）
//   DxMemo.Text.cs  ← TTokenType/PStringToken/TStringToken/TStringLineEx + 自由函数 （DxMemo.pas 238-266 / 659-668 / 2202-2222 / 5820-5877）
//   DxMemo.Chat.cs  ← TDxChatMemo                                                （DxMemo.pas 268-375 / 1855-3647）
//   DxMemo.List.cs  ← TViewItem + TDxListItem + TViewField + TDxListView          （DxMemo.pas 22-37 / 486-657 / 2231-2525 / 3648-5048）
//   DxMemo.Tree.cs  ← TDxTreeNode + TDxTreeView                                  （DxMemo.pas 377-484 / 5050-5819）
//
// TScrollStyle 归属说明（跨车道接缝）：
//   原文依赖 VCL `Controls.TScrollStyle`（DxMemo.pas 47/125/172）。本命名空间
//   `GXX.Client.DxComponent` 内**尚无**该枚举；同解决方案内的唯一同名定义在
//   `GXX.Client.LoadDx.TScrollStyle`（LoadDx/GuiRecords.g.cs:133）。为避免第五次
//   「重复接缝」碰撞，本片**不另造枚举**，直接引用 `LoadDx.TScrollStyle`（见 FScrollBars）。
//   若集成方希望 DxComponent 自持该枚举，需先撤销 LoadDx 侧定义 —— 已上报调度方。
// =============================================================================================

/// <summary>
/// DxMemo.pas 44-192 —— TDxScrollControl（滚动控件基类）。
///
/// 原文继承 `TDxControl`（DxControls.pas）。本片按台账 §12.8 裁定成为该类型的**正式归属**：
/// `DxComponent/DxControls.cs` 里那份「最小接缝」已由调度方撤销，本类即唯一真源。
///
/// 托管偏差（唯一一处，已在方法处注明）：原文 `MouseWheelDown`/`MouseWheelUp` 是
/// `override`（基类 `TDxControl` 在 DxControls.pas 4036-4044 有同名空虚方法）；
/// 托管侧上游 `TDxControl`（DxComponentCommon.cs，只读）**没有**声明这两个虚方法，
/// 故本类以 `virtual` 而非 `override` 落地 —— 调用点（`TDxControlEngine.PortMouseWheelDown`
/// 的 `sc.MouseWheelDown(...)`）行为与原文一致。
/// </summary>
public abstract class TDxScrollControl : TDxControl
{
    // ---- 原文私有字段（DxMemo.pas 46-108）---------------------------------------------------

    private bool FShowScroll;                 // 46
    private LoadDx.TScrollStyle FScrollBars;  // 47（原文 TScrollStyle）
    private int FScrollSize;                  // 48

    private int FItemHeight;                  // 50
    private int FItemIndex;                   // 51
    private int FHotItemIndex;                // 52

    private int FPosition;                    // 54

    private int FVisibleItemCount;            // 56

    private TDxImageIndex FScrollImageIndex;  // 58
    private TDxImageIndex FPrevImageIndex;    // 60
    private TDxImageIndex FNextImageIndex;    // 61
    private TDxImageIndex FBarImageIndex;     // 62

    private int FPrevImageSize;               // 64
    private int FNextImageSize;               // 65
    private int FBarImageSize;                // 66

    private bool FPrevMouseDown;              // 68
    private bool FNextMouseDown;              // 69
    private bool FBarMouseDown;               // 70

    private bool FPrevMouseMove;              // 72
    private bool FNextMouseMove;              // 73
    private bool FBarMouseMove;               // 74

    private bool FScrollMouseDown;            // 76

    private Action<TDxScrollControl> FOnScroll; // 78（原文 TNotifyEvent）

    private int FBarTop;                      // 80

    private int FOffSetX, FOffSetY;           // 82
    private int FShowItemCount;               // 83
    private uint FMouseMoveTick;              // 84（原文 LongWord）

    private TDxRect FMaxRect;                 // 86

    private int FExpandSize;                  // 87
    private bool FCanMouseWheel;              // 88

    private bool FAutoShowScroll;             // 90

    private bool FMouseHorizontal;            // 92  True Left False Top

    private bool FMouseScroll;                // 94  所有区域都能拖拽移动
    private int FMouseX;                      // 95
    private int FMouseY;                      // 96
    private bool FMouseScrollDown;            // 97

    // 99: {$MESSAGE HINT 'FMouseSpring 变量没用到，但是不能删，可能会影响插件API的内存结构'}
    // 原文如此（DxMemo.pas:99-100）：FMouseSpring 声明后从未被读写，但因「会影响插件 API 的
    // 内存结构」而**不可删除**。CS0169「字段从未使用」是本移植的预期警告，保留字段以维持结构。
    private bool FMouseSpring;                // 100

    private ushort FSpringStep;               // 101
    private int FDX;                          // 102  按下坐标
    private int FDY;                          // 103
    private double FfSpeedX;                  // 104
    private double FfSpeedY;                  // 105
    private uint FDTime;                      // 106
    private uint FMTime;                      // 107
    private bool FStartSpring;                // 108

    // ---- 原文私有方法（109-129）-------------------------------------------------------------
    // InPrevRange / InNextRange / InBarRange / GetVisibleHeight 见下；
    // ScrollImageIndexChange / PrevImageIndexChange / NextImageIndexChange / BarImageIndexChange
    // 为 stdcall 回调（托管侧即 Action<TDxImageIndex>，见 Create 的挂接）。

    // =========================================================================================
    // DxMemo.pas 670-724 —— constructor Create
    // =========================================================================================

    protected TDxScrollControl()
    {
        // 672 inherited Create(AOwner)
        AutoSize = false;                                        // 673
        FShowScroll = true;                                      // 674
        FScrollSize = 16;                                        // 675
        FScrollBars = LoadDx.TScrollStyle.ssHorizontal;          // 676

        FAutoShowScroll = false;                                 // 678
        FMouseHorizontal = false;                                // 679
        Transparent = true;                                      // 680

        Width = 200;                                             // 682
        Height = 100;                                            // 683
        FScrollImageIndex = new TDxImageIndex();                 // 684
        FPrevImageIndex = new TDxImageIndex();                   // 685
        FNextImageIndex = new TDxImageIndex();                   // 686
        FBarImageIndex = new TDxImageIndex();                    // 687

        FScrollImageIndex.OnChange = ScrollImageIndexChange;     // 689
        FPrevImageIndex.OnChange = PrevImageIndexChange;         // 690
        FNextImageIndex.OnChange = NextImageIndexChange;         // 691
        FBarImageIndex.OnChange = BarImageIndexChange;           // 692

        // 694-697：`FScrollImageIndex.OnGetImage := ImageIndex.OnGetImage`
        // 原文此处是把基类 ImageIndex 上**当时**挂着的 OnGetImage 拷过去。托管侧
        // TDxImageIndex.OnGetImage 是委托字段，故直接取字段引用（语义等价）。
        FScrollImageIndex.OnGetImage = ImageIndex.OnGetImage;    // 694
        FPrevImageIndex.OnGetImage = ImageIndex.OnGetImage;      // 695
        FNextImageIndex.OnGetImage = ImageIndex.OnGetImage;      // 696
        FBarImageIndex.OnGetImage = ImageIndex.OnGetImage;       // 697

        FPrevMouseDown = false;                                  // 699
        FNextMouseDown = false;                                  // 700
        FBarMouseDown = false;                                   // 701
        FScrollMouseDown = false;                                // 702

        FPrevMouseMove = false;                                  // 704
        FNextMouseMove = false;                                  // 705
        FBarMouseMove = false;                                   // 706

        FItemIndex = -1;                                         // 708
        FPosition = 0;                                           // 709
        FItemHeight = 12;                                        // 710
        FOnScroll = null;                                        // 711

        FVisibleItemCount = 0;                                   // 713
        FExpandSize = 0;                                         // 714
        FBarTop = 0;                                             // 715
        FOffSetX = 0;                                            // 716
        FOffSetY = 0;                                            // 717
        FShowItemCount = 0;                                      // 718
        FMouseMoveTick = DxMemoClock.MyGetTickCount();           // 719
        OwnerMove = true;                                        // 720
        FCanMouseWheel = true;                                   // 721

        FSpringStep = 50;                                        // 723
    }

    // =========================================================================================
    // DxMemo.pas 726-733 —— destructor Destroy
    // =========================================================================================

    /// <summary>
    /// DxMemo.pas 726-733 1:1。
    /// 原文只 Free 四个 ImageIndex（TDxImageIndex 是 Delphi 对象，需显式释放）；
    /// 托管侧为普通引用类型，置 null 即为等价处置（无 GC 语义差异可观察）。
    /// </summary>
    public void Destroy()
    {
        FScrollImageIndex = null;   // 728 FScrollImageIndex.Free
        FPrevImageIndex = null;     // 729 FPrevImageIndex.Free
        FNextImageIndex = null;     // 730 FNextImageIndex.Free
        FBarImageIndex = null;      // 731 FBarImageIndex.Free
        // 732 inherited Destroy（托管侧由 GC 承接）
    }

    // =========================================================================================
    // DxMemo.pas 738-745 —— SetOnGetImage
    // =========================================================================================

    /// <summary>
    /// DxMemo.pas 738-745 1:1。
    /// 原文 `inherited SetOnGetImage(Value)` 后把同一个回调转挂到四个 ImageIndex 上。
    /// 托管侧基类方法是 `TDxControl.SetOnGetImage`。
    /// </summary>
    public void SetOnGetImage(Action<TDxImageIndex, TImageType> Value)
    {
        base.SetOnGetImage(Value);                       // 740
        FScrollImageIndex.OnGetImage = Value;            // 741
        FPrevImageIndex.OnGetImage = Value;              // 742
        FNextImageIndex.OnGetImage = Value;              // 743
        FBarImageIndex.OnGetImage = Value;               // 744
    }

    // =========================================================================================
    // DxMemo.pas 747-825 —— 四个 ImageIndex 变更回调
    // =========================================================================================

    /// <summary>
    /// DxMemo.pas 747-766 1:1。
    /// 原文笔误（DxMemo.pas:758/762）：`nIndex` 只在三个分支都命中时被赋值，
    /// 若无任何分支命中则 `nIndex` 保持**上一轮/未初始化**的值（原文无 nIndex := -1 初值）；
    /// 但 760 的 `if (nIndex >= 0)` 使得未命中分支通常被挡住。逐字保留该结构。
    /// 另：本方法写 FScrollSize = Texture.Width（**宽**），而其余三个回调写的是 Texture.Height。
    /// </summary>
    public void ScrollImageIndexChange(TDxImageIndex Sender)
    {
        int nIndex = 0;   // 原文 nIndex 无初值，托管侧必须给 0 以避免「未赋值局部变量」编译错

        if (FScrollImageIndex.Image != null)              // 752
        {
            if (FScrollImageIndex.Up >= 0)                // 753
                nIndex = FScrollImageIndex.Up;            // 754
            else if (FScrollImageIndex.Hot >= 0)          // 755
                nIndex = FScrollImageIndex.Hot;           // 756
            else if (FScrollImageIndex.Down >= 0)         // 757
                nIndex = FScrollImageIndex.Down;          // 758

            if (nIndex >= 0)                              // 760
            {
                var Texture = FScrollImageIndex.Image.GetImage(nIndex);      // 761
                if (Texture != null && Texture.Width * Texture.Height > 4)   // 762
                    FScrollSize = Texture.Width;                             // 763
            }
        }
    }

    /// <summary>DxMemo.pas 768-787 1:1（注意 773/782 用的是 **PrevImageIndex** 属性而非 FPrevImageIndex）。</summary>
    public void PrevImageIndexChange(TDxImageIndex Sender)
    {
        int nIndex = 0;

        if (PrevImageIndex.Image != null)                 // 773
        {
            if (FPrevImageIndex.Up >= 0)                  // 774
                nIndex = FPrevImageIndex.Up;              // 775
            else if (FPrevImageIndex.Hot >= 0)            // 776
                nIndex = FPrevImageIndex.Hot;             // 777
            else if (FPrevImageIndex.Down >= 0)           // 778
                nIndex = FPrevImageIndex.Down;            // 779

            if (nIndex >= 0)                              // 781
            {
                var Texture = PrevImageIndex.Image.GetImage(nIndex);          // 782
                if (Texture != null && Texture.Width * Texture.Height > 4)    // 783
                    FPrevImageSize = Texture.Height;                          // 784
            }
        }
    }

    /// <summary>
    /// DxMemo.pas 789-806 1:1。
    /// 原文如此：与 Prev 版不同，这里把 `nIndex` 的三段判定提到了 `if (NextImageIndex.Image <> nil)`
    /// **之外**（794-799），即无图时也会先算 nIndex；但 801 仍要求 Image 非 nil 才使用。
    /// </summary>
    public void NextImageIndexChange(TDxImageIndex Sender)
    {
        int nIndex = 0;

        if (FNextImageIndex.Up >= 0)                      // 794
            nIndex = FNextImageIndex.Up;                  // 795
        else if (FNextImageIndex.Hot >= 0)                // 796
            nIndex = FNextImageIndex.Hot;                 // 797
        else if (FNextImageIndex.Down >= 0)               // 798
            nIndex = FNextImageIndex.Down;                // 799

        if (NextImageIndex.Image != null && nIndex >= 0)  // 801
        {
            var Texture = NextImageIndex.Image.GetImage(nIndex);              // 802
            if (Texture != null && Texture.Width * Texture.Height > 4)        // 803
                FNextImageSize = Texture.Height;                              // 804
        }
    }

    /// <summary>DxMemo.pas 808-825 1:1（结构同 Next 版：nIndex 判定在 Image 判空之外）。</summary>
    public void BarImageIndexChange(TDxImageIndex Sender)
    {
        int nIndex = 0;

        if (FBarImageIndex.Up >= 0)                       // 813
            nIndex = FBarImageIndex.Up;                   // 814
        else if (FBarImageIndex.Hot >= 0)                 // 815
            nIndex = FBarImageIndex.Hot;                  // 816
        else if (FBarImageIndex.Down >= 0)                // 817
            nIndex = FBarImageIndex.Down;                 // 818

        if (BarImageIndex.Image != null && nIndex >= 0)   // 820
        {
            var Texture = BarImageIndex.Image.GetImage(nIndex);               // 821
            if (Texture != null && Texture.Width * Texture.Height > 4)        // 822
                FBarImageSize = Texture.Height;                               // 823
        }
    }

    // =========================================================================================
    // DxMemo.pas 827-869 —— 命中判定
    // =========================================================================================

    /// <summary>DxMemo.pas 827-833 1:1。</summary>
    public int GetVisibleHeight()
    {
        if (FMouseHorizontal)          // 829
            return Width - OffSetX;    // 830 原文 `Width - OffSetX`
        return Height - OffSetY;       // 832
    }

    /// <summary>DxMemo.pas 835-838 1:1 —— 检测鼠标点在「向上」的按钮。</summary>
    public bool InPrevRange(int X, int Y, TDxRect vRect)
        => (X >= vRect.Right - FScrollSize) && (Y <= vRect.Top + FPrevImageSize);

    /// <summary>
    /// DxMemo.pas 840-843 1:1 —— 检测鼠标点在「向下」的按钮。
    /// 原文笔误（DxMemo.pas:842）：用的是 `Height`（**控件自身高度**）而不是 `vRect.Bottom`。
    /// </summary>
    public bool InNextRange(int X, int Y, TDxRect vRect)
        => (X >= vRect.Right - FScrollSize) && (Y >= vRect.Top + (Height - FNextImageSize));

    /// <summary>DxMemo.pas 845-869 1:1 —— 检测鼠标点在滚动条滑块上。</summary>
    public bool InBarRange(int X, int Y, TDxRect vRect)
    {
        int nHeight;
        int nMaxValue;
        int nBarTop;

        bool Result = false;                                            // 851
        if (FShowScroll)                                                // 852
        {
            if ((X >= vRect.Right - FScrollSize) && (Y < vRect.Bottom - FNextImageSize) &&
                (Y > vRect.Top + FPrevImageSize))                       // 853-854
            {
                if (FPosition > 0)                                      // 855
                {
                    nMaxValue = MaxValue();                             // 856
                    nHeight = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 857
                    if ((nMaxValue > 0) && (nHeight > 0) && (nMaxValue > VisibleHeight))  // 858
                        nBarTop = (int)Math.Round(FPosition * (double)nHeight / (nMaxValue - VisibleHeight));  // 859
                    else
                        nBarTop = 0;                                    // 861
                    Result = (Y >= vRect.Top + FPrevImageSize + nBarTop) &&
                             (Y <= vRect.Top + FPrevImageSize + nBarTop + FBarImageSize);      // 862
                }
                else
                {
                    Result = (Y <= vRect.Top + FPrevImageSize + FBarImageSize);   // 865
                }
            }
        }
        return Result;
    }

    // =========================================================================================
    // DxMemo.pas 871-891 —— DoScroll / AutoCalcShowItemCount
    // =========================================================================================

    /// <summary>
    /// DxMemo.pas 871-886 1:1 —— 滚动（把全部子控件按 Value 位移）。
    /// 原文 `Control[I]` → 托管侧 `DxControlOps.GetControl`（子控件表由 DxControlOps 承接，见 DxControls.cs）。
    /// </summary>
    public virtual void DoScroll(int Value)
    {
        if (Value != 0)                                                  // 876
        {
            int count = DxControlOps.GetControlCount(this);               // 877 ControlCount
            for (int I = 0; I < count; I++)
            {
                var D = DxControlOps.GetControl(this, I);                 // 878 Control[I]
                if (D == null) continue;
                if (FMouseHorizontal)                                     // 879
                    D.Left = D.Left + Value;                              // 880
                else
                    D.Top = D.Top + Value;                                // 882
            }
        }
        FOnScroll?.Invoke(this);                                          // 885
    }

    /// <summary>DxMemo.pas 888-891 1:1（VisibleHeight div ItemHeight）。</summary>
    public void AutoCalcShowItemCount()
        => ShowItemCount = VisibleHeight / ItemHeight;                    // 890

    #region 原文 893-1338：与「滚动条几何 / 位置夹取」相关的同一段收尾逻辑
    //
    // 原文在 SetExpandSize / SetVisibleItemCount / SetPosition / Next / Previous / First / Last /
    // DoResize 共 7 处**逐字重复**了同一段「按 FPosition 反算 FBarTop」的代码，差别只在
    // 一处比较基准（`Height` 还是 `VisibleHeight`）与一处赋值（`nMaxScrollValue` 还是
    // `Height - FPrevImageSize - FNextImageSize - FBarImageSize`）。
    // 为保持 1:1 且便于逐处对照，此处**不抽公共方法**，每处照抄原文。
    #endregion

    /// <summary>DxMemo.pas 893-918 1:1。</summary>
    public void SetExpandSize(int Value)
    {
        int nMaxValue, nMaxScrollValue;

        if (FExpandSize != Value)                                        // 897
        {
            FExpandSize = Value;                                         // 898
            if (FShowScroll)                                             // 899
            {
                nMaxValue = MaxValue();                                  // 900
                nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 901

                if ((nMaxValue > 0) && (nMaxScrollValue > 0))             // 903
                    FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 904
                else
                    FBarTop = 0;                                          // 906

                if (nMaxValue > Height)                                   // 908
                {
                    if (FPosition + VisibleHeight >= nMaxValue)           // 909
                        FBarTop = nMaxScrollValue;                        // 910
                    else if (FPosition == 0)                              // 911
                        FBarTop = 0;                                      // 912
                }
                else
                    FBarTop = 0;                                          // 915
            }
        }
    }

    /// <summary>
    /// DxMemo.pas 920-933 1:1。
    /// 原文 `Min(nMinValue, D.Top)` —— 取所有子控件 Top 的**最小值**（无子控件时恒 0）。
    /// </summary>
    public virtual int MinValue()
    {
        int nMinValue = 0;                                                // 926
        int count = DxControlOps.GetControlCount(this);                    // 927
        for (int I = 0; I < count; I++)
        {
            var D = DxControlOps.GetControl(this, I);                      // 928
            if (D == null) continue;
            nMinValue = Min(nMinValue, D.Top);                             // 929
        }
        return nMinValue;                                                  // 932
    }

    /// <summary>
    /// DxMemo.pas 935-966 1:1。
    /// 先按 `LongRect` 求所有子控件 ClientRect 的并集 FMaxRect，再按
    /// VisibleItemCount/VisibleHeight/ItemHeight 做三段修正，最后加 FExpandSize。
    /// </summary>
    public virtual int MaxValue()
    {
        int nMaxValue;

        FMaxRect = TDxRect.Rect(int.MaxValue, int.MaxValue, 0, 0);         // 941 High(Integer)
        int count = DxControlOps.GetControlCount(this);                     // 942
        for (int I = 0; I < count; I++)
        {
            var D = DxControlOps.GetControl(this, I);                       // 943
            if (D == null) continue;
            FMaxRect = DxRectUtil.LongRect(FMaxRect, D.ClientRect);         // 944
        }

        if (FMouseHorizontal)                                               // 947
            nMaxValue = Max(FMaxRect.Right - Min(FMaxRect.Left, 0), 0);     // 948
        else
            nMaxValue = Max(FMaxRect.Bottom - Min(FMaxRect.Top, 0), 0);     // 950

        if (VisibleItemCount > 0)                                           // 952
        {
            if (nMaxValue > VisibleHeight)                                  // 953
            {
                if ((nMaxValue % VisibleHeight) > VisibleItemCount * ItemHeight)   // 954
                    nMaxValue = nMaxValue + (VisibleHeight - VisibleItemCount * ItemHeight);  // 955
            }
            else
            {
                if (nMaxValue > VisibleItemCount * ItemHeight)              // 959
                    nMaxValue = nMaxValue + (VisibleHeight - VisibleItemCount * ItemHeight);  // 960
            }
        }

        return nMaxValue + FExpandSize;                                     // 965
    }

    /// <summary>DxMemo.pas 968-995 1:1。</summary>
    public void SetVisibleItemCount(int Value)
    {
        int nMaxValue, nMaxScrollValue;

        if (VisibleHeight / ItemHeight < Value)                             // 972
            FVisibleItemCount = 0;                                          // 973
        else
            FVisibleItemCount = Value;                                      // 975

        if (FShowScroll)                                                    // 977
        {
            nMaxValue = MaxValue();                                         // 978
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 979

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 981
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 982
            else
                FBarTop = 0;                                                // 984

            if (nMaxValue > Height)                                         // 986
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 987
                    FBarTop = nMaxScrollValue;                              // 988
                else if (FPosition == 0)                                    // 989
                    FBarTop = 0;                                            // 990
            }
            else
                FBarTop = 0;                                                // 993
        }
    }

    /// <summary>
    /// DxMemo.pas 997-1035 1:1 —— 设置滚动指针。
    /// 夹取顺序：先 P&lt;0→0；再「VisibleHeight &gt; nMaxValue ⇒ 0」；
    /// 否则「P + VisibleHeight &gt; nMaxValue ⇒ nMaxValue - VisibleHeight」。
    /// 注意 1014 的 `DoScroll(FPosition - P)`：传的是**旧减新**（即反向位移量）。
    /// </summary>
    public void SetPosition(int Value)
    {
        int P, nMaxValue, nMaxScrollValue;

        nMaxValue = MaxValue();                                             // 1001
        if (FPosition != Value)                                             // 1002
        {
            P = Value;                                                      // 1003
            if (P < 0) P = 0;                                               // 1004

            if (VisibleHeight > nMaxValue)                                  // 1006
            {
                P = 0;                                                      // 1007
            }
            else
            {
                if (P + VisibleHeight > nMaxValue)                          // 1010
                    P = nMaxValue - VisibleHeight;                          // 1011
            }

            DoScroll(FPosition - P);                                        // 1014
            FPosition = P;                                                  // 1015
        }

        if (FShowScroll)                                                    // 1018
        {
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1019

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 1021
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 1022
            else
                FBarTop = 0;                                                // 1024

            if (nMaxValue > VisibleHeight)                                  // 1026
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 1027
                    FBarTop = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1028
                else if (FPosition == 0)                                    // 1029
                    FBarTop = 0;                                            // 1030
            }
            else
                FBarTop = 0;                                                // 1033
        }
    }

    /// <summary>DxMemo.pas 1037-1083 1:1 —— 向下滚动。</summary>
    public void Next()
    {
        int P;
        int nMaxScrollValue;
        int nMaxValue;

        nMaxValue = MaxValue();                                             // 1043
        if ((nMaxValue > 0) && (nMaxValue > VisibleHeight))                 // 1044
        {
            if (FPosition + VisibleHeight < nMaxValue)                      // 1045
            {
                if (FPosition + VisibleHeight + FItemHeight <= nMaxValue)   // 1046
                {
                    P = FPosition + FItemHeight;                            // 1047
                    DoScroll(FPosition - P);                                // 1048
                    FPosition = P;                                          // 1049
                }
                else
                {
                    P = nMaxValue - VisibleHeight;                          // 1052
                    DoScroll(FPosition - P);                                // 1053
                    FPosition = P;                                          // 1054
                }
            }
        }
        else
        {
            if (FPosition > 0)                                              // 1059
            {
                P = 0;                                                      // 1060
                DoScroll(FPosition - P);                                    // 1061
                FPosition = P;                                              // 1062
            }
        }

        if (FShowScroll)                                                    // 1066
        {
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1067

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 1069
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 1070
            else
                FBarTop = 0;                                                // 1072

            if (nMaxValue > Height)                                         // 1074
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 1075
                    FBarTop = nMaxScrollValue;                              // 1076
                else if (FPosition == 0)                                    // 1077
                    FBarTop = 0;                                            // 1078
            }
            else
                FBarTop = 0;                                                // 1081
        }
    }

    /// <summary>DxMemo.pas 1085-1129 1:1 —— 向上滚动（与 Next 对偶；比较基准是 VisibleHeight）。</summary>
    public void Previous()
    {
        int P;
        int nMaxScrollValue;
        int nMaxValue;

        nMaxValue = MaxValue();                                             // 1091
        if (FPosition > 0)                                                  // 1092
        {
            if ((nMaxValue > 0) && (nMaxValue > VisibleHeight))             // 1093
            {
                if (FPosition - FItemHeight >= 0)                           // 1094
                {
                    P = FPosition - FItemHeight;                            // 1095
                    DoScroll(FPosition - P);                                // 1096
                    FPosition = P;                                          // 1097
                }
                else
                {
                    P = 0;                                                  // 1100
                    DoScroll(FPosition - P);                                // 1101
                    FPosition = P;                                          // 1102
                }
            }
            else
            {
                P = 0;                                                      // 1106
                DoScroll(FPosition - P);                                    // 1107
                FPosition = P;                                              // 1108
            }
        }

        if (FShowScroll)                                                    // 1112
        {
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1113

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 1115
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 1116
            else
                FBarTop = 0;                                                // 1118

            if (nMaxValue > VisibleHeight)                                  // 1120
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 1121
                    FBarTop = nMaxScrollValue;                              // 1122
                else if (FPosition == 0)                                    // 1123
                    FBarTop = 0;                                            // 1124
            }
            else
                FBarTop = 0;                                                // 1127
        }
    }

    /// <summary>
    /// DxMemo.pas 1131-1166 1:1 —— 滚动条回到顶部。
    /// 原文笔误（DxMemo.pas:1139）：`DoScroll(Max(abs(nMinValue), FPosition))` ——
    /// Max 的**两个参数顺序**与常规相反（先 abs(MinValue) 再 FPosition），
    /// 且 abs 用的是 System.Abs（Integer 版）。逐字保留。
    /// </summary>
    public void First()
    {
        int P;
        int nMaxScrollValue;
        int nMaxValue, nMinValue;

        nMinValue = MinValue();                                             // 1137
        if (nMinValue < 0)                                                  // 1138
        {
            DoScroll(Max(Math.Abs(nMinValue), FPosition));                   // 1139
            FPosition = 0;                                                  // 1140
        }
        if (FPosition > 0)                                                  // 1142
        {
            P = 0;                                                          // 1143
            DoScroll(FPosition - P);                                        // 1144
            FPosition = P;                                                  // 1145
        }

        if (FShowScroll)                                                    // 1148
        {
            nMaxValue = MaxValue();                                         // 1149
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1150

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 1152
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 1153
            else
                FBarTop = 0;                                                // 1155

            if (nMaxValue > VisibleHeight)                                  // 1157
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 1158
                    FBarTop = nMaxScrollValue;                              // 1159
                else if (FPosition == 0)                                    // 1160
                    FBarTop = 0;                                            // 1161
            }
            else
                FBarTop = 0;                                                // 1164
        }
    }

    /// <summary>DxMemo.pas 1168-1207 1:1 —— 滚动条回到底部。</summary>
    public void Last()
    {
        int P;
        int nMaxScrollValue;
        int nMaxValue;

        nMaxValue = MaxValue();                                             // 1174
        if ((nMaxValue > 0) && (nMaxValue > VisibleHeight))                 // 1175
        {
            if (FPosition + VisibleHeight < nMaxValue)                      // 1176
            {
                P = nMaxValue - VisibleHeight;                              // 1177
                DoScroll(FPosition - P);                                    // 1178
                FPosition = P;                                              // 1179
            }
        }
        else
        {
            if (FPosition > 0)                                              // 1183
            {
                P = 0;                                                      // 1184
                DoScroll(FPosition - P);                                    // 1185
                FPosition = P;                                              // 1186
            }
        }

        if (FShowScroll)                                                    // 1190
        {
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1191

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 1193
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 1194
            else
                FBarTop = 0;                                                // 1196

            if (nMaxValue > VisibleHeight)                                  // 1198
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 1199
                    FBarTop = nMaxScrollValue;                              // 1200
                else if (FPosition == 0)                                    // 1201
                    FBarTop = 0;                                            // 1202
            }
            else
                FBarTop = 0;                                                // 1205
        }
    }

    /// <summary>DxMemo.pas 1209-1215 1:1。</summary>
    public void SetShowItemCount(int Value)
    {
        if (FShowItemCount != Value)                                        // 1211
        {
            FShowItemCount = Value;                                         // 1212
            ChangeShowItemCount();                                          // 1213
        }
    }

    /// <summary>DxMemo.pas 1217-1225 1:1（仅在置 True 时调 AutoSetShowScroll）。</summary>
    public void SetAutoShowScroll(bool Value)
    {
        if (FAutoShowScroll != Value)                                       // 1219
        {
            FAutoShowScroll = Value;                                        // 1220
            if (Value)                                                      // 1221
                AutoSetShowScroll();                                        // 1222
        }
    }

    /// <summary>DxMemo.pas 1227-1230 1:1 —— 空体（原文如此）。</summary>
    protected virtual void ChangeShowItemCount() { }

    /// <summary>DxMemo.pas 1232-1235 1:1（只转基类）。</summary>
    protected virtual void DoClick(int X, int Y)
    {
        // 1234 inherited → 托管侧基类对应 DxControlOps.DoClick
        DxControlOps.DoClick(this, X, Y);
    }

    /// <summary>
    /// DxMemo.pas 1237-1243 1:1。
    /// 托管偏差：见类头说明 —— 上游 TDxControl 未声明 MouseWheelDown 虚方法，故用 virtual 落地。
    /// 原文 1240 `inherited MouseWheelDown(Shift, MousePos)` 是基类**空实现**，
    /// 托管侧等价物即 `DxControlOps.MouseWheelDown`（空体）。
    /// </summary>
    public virtual void MouseWheelDown(TDxShiftState Shift, TDxPoint MousePos)
    {
        if (FCanMouseWheel)                                                 // 1239
        {
            DxControlOps.MouseWheelDown(this, Shift, MousePos);              // 1240 inherited（空体）
            Next();                                                         // 1241
        }
    }

    /// <summary>DxMemo.pas 1245-1251 1:1（偏差同上）。</summary>
    public virtual void MouseWheelUp(TDxShiftState Shift, TDxPoint MousePos)
    {
        if (FCanMouseWheel)                                                 // 1247
        {
            DxControlOps.MouseWheelUp(this, Shift, MousePos);                // 1248 inherited（空体）
            Previous();                                                     // 1249
        }
    }

    /// <summary>
    /// DxMemo.pas 1253-1265 1:1。
    /// 原文用 `FItemHeight`（**字段**）做除数，而 SetItemHeight 保证它 ≥1（1274），故不会除零。
    /// </summary>
    public void SetItemIndex(int Value)
    {
        int nItemCount;

        if (FItemIndex != Value)                                            // 1257
        {
            FItemIndex = Value;                                             // 1258
            nItemCount = (MaxValue() - VisibleItemCount * ItemHeight) / FItemHeight;   // 1259
            if (FItemIndex >= nItemCount) FItemIndex = -1;                   // 1260
            if (FItemIndex >= 0)                                            // 1261
                Position = FItemIndex * FItemHeight;                        // 1262
        }
    }

    /// <summary>DxMemo.pas 1267-1270 1:1 —— 空体（原文如此）。</summary>
    protected virtual void AutoSetShowScroll() { }

    /// <summary>DxMemo.pas 1272-1275 1:1（下限夹到 1）。</summary>
    public void SetItemHeight(int Value) => FItemHeight = Max(Value, 1);

    /// <summary>DxMemo.pas 1277-1282 1:1。</summary>
    public void SetScrollSize(int Value)
    {
        if (FScrollSize != Value)                                           // 1279
            FScrollSize = Value;                                            // 1280
    }

    /// <summary>DxMemo.pas 1284-1289 1:1。</summary>
    public void SetScrollBars(LoadDx.TScrollStyle Value)
    {
        if (FScrollBars != Value)                                           // 1286
            FScrollBars = Value;                                            // 1287
    }

    /// <summary>DxMemo.pas 1291-1298 1:1。</summary>
    protected virtual void DoMouseUp()
    {
        FBarMouseDown = false;                                              // 1293
        FPrevMouseDown = false;                                             // 1294
        FNextMouseDown = false;                                             // 1295
        FScrollMouseDown = false;                                           // 1296
        FMouseScrollDown = false;                                           // 1297
    }

    /// <summary>DxMemo.pas 1300-1303 1:1（只转基类）。</summary>
    protected virtual void DoMouseEnter()
    {
        DxControlOps.OnMouseEnterCore(this);                                // 1302 inherited
    }

    /// <summary>DxMemo.pas 1305-1311 1:1。</summary>
    protected virtual void DoMouseLeave()
    {
        FPrevMouseMove = false;                                             // 1307
        FNextMouseMove = false;                                             // 1308
        FBarMouseMove = false;                                              // 1309
        DxControlOps.OnMouseLeaveCore(this);                                // 1310 inherited
    }

    /// <summary>
    /// DxMemo.pas 1313-1339 1:1。
    /// 原文笔误（DxMemo.pas:1336）：`else` 分支里额外把 `Position := 0` ——
    /// 该赋值会经属性 setter 递归回 SetPosition（但此时 FPosition 已是 0，故不再递归）。
    /// </summary>
    protected override void DoResize(ref TDxRect NewRect)
    {
        int nMaxScrollValue;
        int nMaxValue;

        // 1318 inherited
        base.DoResize(ref NewRect);

        if (FShowScroll)                                                    // 1319
        {
            nMaxValue = MaxValue();                                         // 1320
            nMaxScrollValue = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1321

            if ((nMaxValue > 0) && (nMaxScrollValue > 0))                    // 1323
                FBarTop = Max((int)Math.Round(FPosition * (double)nMaxScrollValue / (nMaxValue - VisibleHeight)), 0);  // 1324
            else
                FBarTop = 0;                                                // 1326

            if (nMaxValue > VisibleHeight)                                  // 1328
            {
                if (FPosition + VisibleHeight >= nMaxValue)                 // 1329
                    FBarTop = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1330
                else if (FPosition == 0)                                    // 1331
                    FBarTop = 0;                                            // 1332
            }
            else
            {
                FBarTop = 0;                                                // 1335
                Position = 0;                                               // 1336
            }
        }
    }

    /// <summary>DxMemo.pas 1341-1344 1:1。</summary>
    protected override bool CanMove()
        => !(FScrollMouseDown || FPrevMouseDown || FNextMouseDown || FBarMouseDown);

    // =========================================================================================
    // DxMemo.pas 1346-1490 —— 鼠标三段（滚动条交互）
    // =========================================================================================

    /// <summary>
    /// DxMemo.pas 1346-1404 1:1。
    /// 注意 1390/1397 两处 `FItemIndex := (Y - vtRect.Top - OffSetY) div FItemHeight + FPosition div FItemHeight;`
    /// 完全相同（原文 HZQ 20230525 的注释说明：不显示滚动条时也要算 ItemIndex）。
    /// 原文 `Button = mbLeft` 用的是 VCL `TMouseButton`；托管侧同值枚举为 `TDxMouseButton.mbLeft`。
    /// </summary>
    protected bool ScrollMouseDown(TDxMouseButton Button, int X, int Y)
    {
        int nHeight;
        int nMaxValue;
        int nBarTop;
        TDxRect vRect;
        TDxRect vtRect;

        bool Result = false;                                                // 1354
        vtRect = VirtualRect;                                               // 1355
        vRect = vtRect;                                                     // 1356
        FMouseMoveTick = DxMemoClock.MyGetTickCount() + 600;                 // 1357
        if (FShowScroll)                                                    // 1358
        {
            if (Button == TDxMouseButton.mbLeft)                            // 1359
            {
                FScrollMouseDown = (X >= vRect.Right - FScrollSize);        // 1360
                Result = FScrollMouseDown;                                  // 1361
                if (!(FPrevMouseDown || FNextMouseDown || FBarMouseDown))   // 1362
                {
                    FPrevMouseDown = InPrevRange(X, Y, vRect);              // 1363
                    FNextMouseDown = InNextRange(X, Y, vRect);              // 1364
                    FBarMouseDown = InBarRange(X, Y, vRect);                // 1365
                }

                if (!(FPrevMouseDown || FNextMouseDown || FBarMouseDown))   // 1368
                {
                    vRect.Left = vRect.Right - FScrollSize;                 // 1369
                    vRect.Right = vRect.Left + FScrollSize;                 // 1370
                    if (DxRectUtil.PointInRect(new TDxPoint(X, Y), vRect))  // 1371
                    {
                        nMaxValue = MaxValue();                             // 1372
                        nHeight = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1373
                        if (((nMaxValue > 0) && (nMaxValue > VisibleHeight) && (nHeight > 0)) || (Position > 0))   // 1374
                        {
                            nBarTop = Max(Y - vRect.Top - FPrevImageSize - FBarImageSize / 2, 0);   // 1375
                            Position = (int)Math.Round(nBarTop * (double)(nMaxValue - VisibleHeight) / nHeight);   // 1376
                        }
                        else
                        {
                            Position = 0;                                   // 1378
                        }
                        // 1380 Exit;
                    }
                }
                else
                {
                    if (FPrevMouseDown && InPrevRange(X, Y, vRect))          // 1383
                        Previous();                                         // 1384
                    else if (FNextMouseDown && InNextRange(X, Y, vRect))     // 1385
                        Next();                                             // 1386
                }
            }
            FItemIndex = (Y - vtRect.Top - FOffSetY) / FItemHeight + FPosition / FItemHeight;   // 1390
            // 1391-1394 原文注释掉的 FItemIndex 夹取：
            //   if FItemIndex >= (MaxValue - VisibleItemCount * ItemHeight) div FItemHeight then
            //     FItemIndex := -1;
        }
        else
        {
            // 1396 HZQ 20230525 解决在不显示滚动条时，计算 ItemIndex，否则 ItemIndex 永远为 -1
            FItemIndex = (Y - vtRect.Top - FOffSetY) / FItemHeight + FPosition / FItemHeight;   // 1397
        }
        if (FMouseScroll)                                                   // 1399
        {
            FMouseScrollDown = true;                                        // 1400
            FMouseX = X;                                                    // 1401
            FMouseY = Y;                                                    // 1402
        }
        return Result;                                                      // 1404（Result 隐式返回）
    }

    /// <summary>
    /// DxMemo.pas 1406-1480 1:1。
    /// 原文笔误（DxMemo.pas:1461-1478）：FMouseScroll 的拖拽分支只比较
    /// `FMouseX &lt;&gt; X` / `FMouseY &lt;&gt; Y`，且**未使用 Shift 参数**；
    /// `Position := Position + (FMouseX - X)` 的下标方向与注释掉的两分支版本不同。
    /// </summary>
    protected bool ScrollMouseMove(TDxShiftState Shift, int X, int Y)
    {
        int nHeight;
        int nMaxValue;
        int nBarTop;
        TDxRect vtRect = default;
        TDxRect vRect = default;

        bool Result = false;                                                // 1414
        if (FShowScroll)                                                    // 1415
        {
            vtRect = VirtualRect;                                           // 1416
            vRect = vtRect;                                                 // 1417

            if (!(FPrevMouseDown || FNextMouseDown || FBarMouseDown))       // 1419
            {
                FPrevMouseMove = InPrevRange(X, Y, vRect);                  // 1420
                FNextMouseMove = InNextRange(X, Y, vRect);                  // 1421
                FBarMouseMove = InBarRange(X, Y, vRect);                    // 1422
            }

            // 1425 Result := (FPrevMouseDown or FNextMouseDown or FBarMouseDown or FScrollMouseDown);
        }
        if (FBarMouseDown && FShowScroll)                                   // 1427
        {
            FPrevMouseMove = false;                                         // 1428
            FNextMouseMove = false;                                         // 1429

            FNextMouseDown = false;                                         // 1431
            FPrevMouseDown = false;                                         // 1432

            nMaxValue = MaxValue();                                         // 1434
            nHeight = Height - FPrevImageSize - FNextImageSize - FBarImageSize;   // 1435
            if (((nMaxValue > 0) && (nMaxValue > VisibleHeight) && (nHeight > 0)) || (Position > 0))   // 1436
            {
                nBarTop = Max(Y - vRect.Top - FPrevImageSize - FBarImageSize / 2, 0);   // 1437
                Position = (int)Math.Round(nBarTop * (double)(nMaxValue - VisibleHeight) / nHeight);   // 1438
            }
            else
            {
                Position = 0;                                               // 1441
            }
        }
        else
        {
            if (FPrevMouseDown)                                             // 1445
            {
                if (unchecked((int)(DxMemoClock.MyGetTickCount() - FMouseMoveTick)) > 100)   // 1446 Longint(...)
                {
                    FMouseMoveTick = DxMemoClock.MyGetTickCount();           // 1447
                    Previous();                                             // 1448
                }
            }
            else if (FNextMouseDown)                                        // 1451
            {
                if (unchecked((int)(DxMemoClock.MyGetTickCount() - FMouseMoveTick)) > 100)   // 1452
                {
                    FMouseMoveTick = DxMemoClock.MyGetTickCount();           // 1453
                    Next();                                                 // 1454
                }
            }
            vRect = VirtualRect;                                            // 1457
            FHotItemIndex = (Y - vRect.Top - FOffSetY) / FItemHeight + FPosition / FItemHeight;   // 1458
        }

        if (FMouseScrollDown)                                               // 1461
        {
            if ((FMouseX != X) && FMouseHorizontal)                         // 1462
            {
                // 1463-1466 原文注释掉的两分支版本：
                //   if FMouseX > X then Position := Position + 1 else Position := Position - 1;
                Position = Position + (FMouseX - X);                        // 1467

                FMouseX = X;                                                // 1469
            }
            if ((FMouseY != Y) && !FMouseHorizontal)                        // 1471
            {
                // 1472-1474 原文注释掉的两分支版本（且缺 else，注释形态本身不合法）：
                //   if FMouseY > Y then Position := Position + (FMouseY - Y) else
                Position = Position + (FMouseY - Y);                        // 1475

                FMouseY = Y;                                                // 1477
            }
        }
        return Result;                                                      // 1480
    }

    /// <summary>DxMemo.pas 1482-1490 1:1（恒返回 True）。</summary>
    protected bool ScrollMouseUp()
    {
        // 1484 Result := True;
        FBarMouseDown = false;                                              // 1485
        FPrevMouseDown = false;                                             // 1486
        FNextMouseDown = false;                                             // 1487
        FScrollMouseDown = false;                                           // 1488
        FMouseScrollDown = false;                                           // 1489
        return true;                                                        // 1484
    }

    // =========================================================================================
    // 原文 145-163 / 164-191 —— 方法面与 published 属性
    // =========================================================================================

    /// <summary>DxMemo.pas 158 MaxRect（read/write FMaxRect）。</summary>
    public TDxRect MaxRect { get => FMaxRect; set => FMaxRect = value; }

    /// <summary>DxMemo.pas 159 OnScroll（TNotifyEvent）。</summary>
    public Action<TDxScrollControl> OnScroll { get => FOnScroll; set => FOnScroll = value; }

    /// <summary>DxMemo.pas 160 VisibleHeight（read GetVisibleHeight）。</summary>
    public int VisibleHeight => GetVisibleHeight();

    /// <summary>DxMemo.pas 161 HotItemIndex。</summary>
    public int HotItemIndex { get => FHotItemIndex; set => FHotItemIndex = value; }

    /// <summary>DxMemo.pas 168 CanMouseWheel。</summary>
    public bool CanMouseWheel { get => FCanMouseWheel; set => FCanMouseWheel = value; }

    /// <summary>DxMemo.pas 169 ShowScroll。</summary>
    public bool ShowScroll { get => FShowScroll; set => FShowScroll = value; }

    /// <summary>DxMemo.pas 170 ItemHeight（write SetItemHeight，下限 1）。</summary>
    public int ItemHeight { get => FItemHeight; set => SetItemHeight(value); }

    /// <summary>DxMemo.pas 171 ItemIndex（write SetItemIndex）。</summary>
    public int ItemIndex { get => FItemIndex; set => SetItemIndex(value); }

    /// <summary>DxMemo.pas 172 ScrollBars（write SetScrollBars）。</summary>
    public LoadDx.TScrollStyle ScrollBars { get => FScrollBars; set => SetScrollBars(value); }

    /// <summary>DxMemo.pas 173 ScrollSize（write SetScrollSize）。</summary>
    public int ScrollSize { get => FScrollSize; set => SetScrollSize(value); }

    /// <summary>DxMemo.pas 174 ScrollImageIndex。</summary>
    public TDxImageIndex ScrollImageIndex => FScrollImageIndex;

    /// <summary>DxMemo.pas 175 PrevImageIndex。</summary>
    public TDxImageIndex PrevImageIndex => FPrevImageIndex;

    /// <summary>DxMemo.pas 176 NextImageIndex。</summary>
    public TDxImageIndex NextImageIndex => FNextImageIndex;

    /// <summary>DxMemo.pas 177 BarImageIndex。</summary>
    public TDxImageIndex BarImageIndex => FBarImageIndex;

    /// <summary>DxMemo.pas 179 Position（write SetPosition）。</summary>
    public int Position { get => FPosition; set => SetPosition(value); }

    /// <summary>DxMemo.pas 180 VisibleItemCount（write SetVisibleItemCount）。</summary>
    public int VisibleItemCount { get => FVisibleItemCount; set => SetVisibleItemCount(value); }

    /// <summary>DxMemo.pas 181 ExpandSize（write SetExpandSize）。</summary>
    public int ExpandSize { get => FExpandSize; set => SetExpandSize(value); }

    /// <summary>DxMemo.pas 183 OffSetX。原文是**两个独立 Integer 字段** FOffSetX/FOffSetY，此处按原名暴露。</summary>
    public int OffSetX { get => FOffSetX; set => FOffSetX = value; }

    /// <summary>DxMemo.pas 184 OffSetY。</summary>
    public int OffSetY { get => FOffSetY; set => FOffSetY = value; }

    /// <summary>DxMemo.pas 185 ShowItemCount（write SetShowItemCount；TDxListView 有效）。</summary>
    public int ShowItemCount { get => FShowItemCount; set => SetShowItemCount(value); }

    /// <summary>DxMemo.pas 187 AutoShowScroll（write SetAutoShowScroll）。</summary>
    public bool AutoShowScroll { get => FAutoShowScroll; set => SetAutoShowScroll(value); }

    /// <summary>DxMemo.pas 188 MouseHorizontal（True = 水平拖动，False = 垂直）。</summary>
    public bool MouseHorizontal { get => FMouseHorizontal; set => FMouseHorizontal = value; }

    /// <summary>DxMemo.pas 189 MouseScroll。</summary>
    public bool MouseScroll { get => FMouseScroll; set => FMouseScroll = value; }

    /// <summary>DxMemo.pas 190 MouseScrollDown。</summary>
    public bool MouseScrollDown { get => FMouseScrollDown; set => FMouseScrollDown = value; }

    // ---- 内部只读观测点（测试/诊断用；原文相应字段为 private，无公开读取途径）-----------------

    /// <summary>原文 80 FBarTop（private）。测试/诊断用只读视图。</summary>
    public int BarTop => FBarTop;

    /// <summary>原文 64-66 FPrevImageSize/FNextImageSize/FBarImageSize（private）。</summary>
    public int PrevImageSize => FPrevImageSize;

    /// <summary>原文 65 FNextImageSize。</summary>
    public int NextImageSize => FNextImageSize;

    /// <summary>原文 66 FBarImageSize。</summary>
    public int BarImageSize => FBarImageSize;

    /// <summary>原文 68-70 三个 *MouseDown 标志（private）。</summary>
    public bool PrevMouseDown => FPrevMouseDown;

    /// <summary>原文 69 FNextMouseDown。</summary>
    public bool NextMouseDown => FNextMouseDown;

    /// <summary>原文 70 FBarMouseDown。</summary>
    public bool BarMouseDown => FBarMouseDown;

    /// <summary>原文 72-74 三个 *MouseMove 标志（private）。</summary>
    public bool PrevMouseMove => FPrevMouseMove;

    /// <summary>原文 73 FNextMouseMove。</summary>
    public bool NextMouseMove => FNextMouseMove;

    /// <summary>原文 74 FBarMouseMove。</summary>
    public bool BarMouseMove => FBarMouseMove;

    /// <summary>原文 76 FScrollMouseDown（private）。</summary>
    public bool IsScrollMouseDown => FScrollMouseDown;

    /// <summary>原文 101 FSpringStep（private，默认 50）。</summary>
    public ushort SpringStep => FSpringStep;

    // ---- 原文未使用的弹簧字段（100/102-108）：保留以维持「插件 API 内存结构」-------------------

    /// <summary>
    /// 原文 99-100 的 {$MESSAGE HINT}：FMouseSpring **声明后从未读写**，但注释明确
    /// 「不能删，可能会影响插件API的内存结构」。托管侧无内存布局约束，仍按 1:1 保留字段。
    /// 同理保留 FDX/FDY/FfSpeedX/FfSpeedY/FDTime/FMTime/FStartSpring（102-108，原文亦未使用）。
    /// </summary>
    public bool MouseSpringUnused => FMouseSpring;

    /// <summary>原文 102 FDX（按下坐标，未被读写）。</summary>
    public int SpringDX => FDX;

    /// <summary>原文 103 FDY（未被读写）。</summary>
    public int SpringDY => FDY;

    /// <summary>原文 106 FDTime（未被读写）。</summary>
    public uint SpringDTime => FDTime;

    /// <summary>原文 107 FMTime（未被读写）。</summary>
    public uint SpringMTime => FMTime;

    /// <summary>原文 108 FStartSpring（未被读写）。</summary>
    public bool StartSpring => FStartSpring;

    // =========================================================================================
    // 私有工具（Min/Max）—— 原文用 Delphi 的 System.Max/Min（Integer 版）。
    // 本命名空间没有同名工具，故在此实现；语义与 System 版一致（返回两值中的大/小者）。
    // =========================================================================================

    private static int Max(int a, int b) => a >= b ? a : b;

    private static int Min(int a, int b) => a <= b ? a : b;
}

/// <summary>
/// 原文 `MyGetTickCount`（DxMemo.pas:719/1357/1446…；定义见 HUtil32.pas）。
/// 语义 = Windows.GetTickCount：**毫秒、无符号 32 位、约 49.7 天回绕**。
/// 托管侧用 `Environment.TickCount` 的 uint 视图保持同一回绕行为。
/// </summary>
internal static class DxMemoClock
{
    /// <summary>HUtil32.MyGetTickCount 1:1（uint 回绕）。</summary>
    public static uint MyGetTickCount() => unchecked((uint)Environment.TickCount);
}
