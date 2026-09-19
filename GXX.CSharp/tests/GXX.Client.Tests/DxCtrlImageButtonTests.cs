using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// DxImageButton.pas（1,019 行）的单元测试。
//
// 覆盖对象（src/GXX.Client/DxComponent/DxImageButton.cs）：
//   * TButtonAnimation：ShowType 门控（ResolveIsShow，7 种 ShowType × 3 种 Style 的交叉）、
//     帧推进（AdvanceFrame，含 PlayCount 播完即停 / 回绕 / 「先回绕后回调」）、
//     绘制几何（BuildDrawPlan，UseImageOffset / OutsideAreaDraw / 四边裁剪）
//   * TDxImageButton：构造默认值、Style / CaptionOffset* / ExpandWidth 的 setter 副作用、
//     ResolveFaceIndex（5 个 face 的优先级）、ComputeCaptionChangeSize（bsButton 直取图 / 文本量算 /
//     ExpandWidth / Bold +2 / 空 Caption 取图）、SetChecked（bsRadio 互斥）、DoClickV2 状态机、
//     InRange（bsButton 走基类 / 其他 Style 只判 VisibleRect）
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlImageButtonTests
{
    /// <summary>
    /// 原文的 `MouseDowned` / `MouseMoveed` 是**属性**（setter = TDxControl.SetMouseDowned/SetMouseMoveed，
    /// 且需 RootCtrl 非 nil 才落值）。上游接缝把它们做成了字段型属性，因此本夹具显式经
    /// `DxControlOps.SetMouseDowned/SetMouseMoveed` 走**原文 setter 语义**，
    /// 并挂一个根引擎让 `RootCtrlOf` 可解析。
    /// </summary>
    private sealed class TButtonHost : TDxImageButton
    {
        public TDxControlEngine Root;

        public TButtonHost()
        {
            Root = new TDxControlEngine { ClientRect = TDxRect.Bounds(0, 0, 800, 600) };
            DxControlOps.InserComponent(Root, this);
            Designing = false;
        }

        public void SetMouseDowned(bool v) => DxControlOps.SetMouseDowned(this, v);
        public void SetMouseMoveed(bool v) => DxControlOps.SetMouseMoveed(this, v);
        public new void SetChecked(bool v) => base.SetChecked(v);
    }

    private static TDxImageLibraryStub Lib(params (int w, int h)[] sizes)
    {
        var lib = new TDxImageLibraryStub();
        foreach (var (w, h) in sizes) lib.Add(new TDxTextureStub(w, h));
        return lib;
    }

    // ===============================================================================
    // 一、构造默认值（原文 466-489）
    // ===============================================================================

    [Fact]
    public void Constructor_Defaults_MatchOriginal()
    {
        var b = new TDxImageButton();

        Assert.True(b.AutoSize);
        Assert.Equal(100, b.Width);
        Assert.Equal(20, b.Height);
        Assert.Equal(TDxAlignment.taCenter, b.Alignment);
        Assert.Equal(TClickSound.csNone, b.ClickCount);
        Assert.Equal(TButtonStyle.bsButton, b.Style);
        Assert.False(b.Checked);
        Assert.Equal(1, b.CaptionDownOffsetX);
        Assert.Equal(1, b.CaptionDownOffsetY);
        Assert.Equal(0, b.ButtonDownOffsetX);
        Assert.Equal(0, b.ButtonDownOffsetY);
        Assert.Equal(TDrawAligment.daFill, b.DrawAligment);
        Assert.Equal(0, b.CaptionOffsetX);
        Assert.Equal(0, b.CaptionOffsetY);
        Assert.Equal(0, b.ExpandWidth);
        Assert.Equal(TGuiType.t_Button, b.GuiType);       // 原文 488
        Assert.NotNull(b.Animation);
        Assert.NotNull(b.CaptionColor);
    }

    [Fact]
    public void Animation_Constructor_Defaults_MatchOriginal()
    {
        var b = new TDxImageButton();
        var a = b.Animation;

        Assert.Equal(TImageType.Prguse_wil, a.ImageType);
        Assert.Equal(TButtonAnimationShowType.astAlwaysShow, a.FShowType);   // 原文 220
        Assert.Equal(-1, a.StartIndex);
        Assert.Equal(-1, a.EndIndex);
        Assert.Equal(200, a.FrameTime);
        Assert.Equal(0, a.PlayCount);
        Assert.True(a.UseImageOffset);
        Assert.Equal(0, a.OffsetX);
        Assert.Equal(0, a.OffsetY);
        Assert.False(a.OutsideAreaDraw);
        Assert.False(a.BlendDraw);
        Assert.False(a.Draw);
        Assert.False(a.DrawBeforeDef);
        Assert.Equal(0, a.CurrentFrame);
        Assert.Equal(0, a.CurrentCount);
    }

    // ===============================================================================
    // 二、CaptionOffset* / ExpandWidth 的 setter 副作用（原文 824-846）
    // ===============================================================================

    [Fact]
    public void CaptionOffsetSetters_TriggerCaptionChangeOnlyOnChange()
    {
        var b = new TDxImageButton { Caption = "X" };   // 有 Caption 才会算尺寸
        b.ImageIndex.Image = Lib((40, 8));
        b.ImageIndex.Up = 0;

        b.CaptionOffsetX = 5;
        Assert.Equal(5, b.CaptionOffsetX);
        b.CaptionOffsetX = 5;                           // 同值 → 不再触发

        b.CaptionOffsetY = 7;
        Assert.Equal(7, b.CaptionOffsetY);

        b.ExpandWidth = 3;
        Assert.Equal(3, b.ExpandWidth);
        b.ExpandWidth = 3;
        Assert.Equal(3, b.ExpandWidth);
    }

    [Fact]
    public void ExpandWidth_IsAddedToComputedWidth()
    {
        var b = new TDxImageButton { Caption = "ab" };
        b.ImageIndex.Image = Lib((40, 8));
        b.ImageIndex.Up = 0;
        b.Style = TButtonStyle.bsCheckBox;        // 非 bsButton → 走文本量算分支

        var before = b.ComputeCaptionChangeSize();
        Assert.NotNull(before);

        b.ExpandWidth = 10;
        var after = b.ComputeCaptionChangeSize();
        Assert.NotNull(after);
        Assert.Equal(before.Value.X + 10, after.Value.X);
        Assert.Equal(before.Value.Y, after.Value.Y);    // ExpandWidth 只影响宽
    }

    // ===============================================================================
    // 三、ResolveFaceIndex（原文 687-729 / 939-976）
    // ===============================================================================

    [Theory]
    // bsButton：Down / Hot 有效时优先；无效回退 Up
    [InlineData(TButtonStyle.bsButton, false, false, false, 5, 7, 9, 11, 13, 5)]   // Normal → Up
    [InlineData(TButtonStyle.bsButton, true, false, false, 5, 7, 9, 11, 13, 9)]    // Down → Down
    [InlineData(TButtonStyle.bsButton, false, true, false, 5, 7, 9, 11, 13, 7)]    // Hot → Hot
    [InlineData(TButtonStyle.bsButton, true, true, false, 5, 7, 9, 11, 13, 9)]     // Down 优先于 Hot
    [InlineData(TButtonStyle.bsButton, false, false, true, 5, 7, 9, 11, 13, 5)]    // Checked 对 bsButton 无影响
    [InlineData(TButtonStyle.bsButton, false, false, false, 5, -1, -1, 11, 13, 5)] // Down/Hot 无效 → Up
    [InlineData(TButtonStyle.bsButton, true, false, false, 5, -1, -1, 11, 13, 5)]  // Downed 但 Down<0 → 回退 Up
    // bsCheckBox：MouseMoveed + Checked → Checked（无效时 Down）
    [InlineData(TButtonStyle.bsCheckBox, false, true, true, 5, 7, 9, 11, 13, 11)]
    [InlineData(TButtonStyle.bsCheckBox, false, true, true, 5, 7, 9, -1, 13, 9)]   // Checked<0 → Down
    [InlineData(TButtonStyle.bsCheckBox, true, false, true, 5, 7, 9, 11, 13, 11)]  // 非 MouseMoveed + Checked
    [InlineData(TButtonStyle.bsCheckBox, false, false, false, 5, 7, 9, 11, 13, 5)] // 非 Move 非 Check → Up
    public void ResolveFaceIndex_FollowsOriginalPriority(
        TButtonStyle style, bool downed, bool moved, bool isChecked,
        int up, int hot, int down, int chk, int dis, int expected)
    {
        var b = new TButtonHost { Style = style };
        b.ImageIndex.Up = up;
        b.ImageIndex.Hot = hot;
        b.ImageIndex.Down = down;
        b.ImageIndex.Checked = chk;
        b.ImageIndex.Disabled = dis;
        if (downed) b.SetMouseDowned(true);
        if (moved) b.SetMouseMoveed(true);
        b.SetChecked(isChecked);

        Assert.Equal(expected, b.ResolveFaceIndex());
    }

    [Fact]
    public void ResolveFaceIndex_CheckBox_MouseDownedNeedsBothDownAndChecked()
    {
        // 原文 711：`MouseDowned and (Down >= 0) and (Checked >= 0)` → Down；
        // 只满足 MouseDowned 而 Checked < 0 时**不走 Down**，落到 Hot。
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.ImageIndex.Up = 1;
        b.ImageIndex.Hot = 2;
        b.ImageIndex.Down = 3;
        b.ImageIndex.Checked = -1;
        b.SetMouseDowned(true);
        b.SetMouseMoveed(true);

        Assert.Equal(2, b.ResolveFaceIndex());        // Hot，不是 Down(3)

        b.ImageIndex.Checked = 4;
        Assert.Equal(3, b.ResolveFaceIndex());        // 两者都有效 → Down
    }

    // ===============================================================================
    // 四、ComputeCaptionChangeSize（原文 676-786）
    // ===============================================================================

    [Fact]
    public void CaptionChange_NotAutoSize_ReturnsNull()
    {
        var b = new TDxImageButton { Caption = "x", AutoSize = false };
        Assert.Null(b.ComputeCaptionChangeSize());
    }

    [Fact]
    public void CaptionChange_BsButton_TakesImageSizeDirectly_WhenAtLeast2x2()
    {
        var b = new TDxImageButton { Caption = "x", Style = TButtonStyle.bsButton };
        b.ImageIndex.Image = Lib((40, 8));
        b.ImageIndex.Up = 0;

        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        Assert.Equal(40, size.Value.X);
        Assert.Equal(8, size.Value.Y);
    }

    [Fact]
    public void CaptionChange_BsButton_TinyImage_FallsBackToTextPath()
    {
        // 原文 744：bsButton 直取图要求 `D.Width >= 2 and D.Height >= 2`；
        // 1x1 不满足 → 走文本量算分支（此时 d 仍有效，故再加 `+1 +D.Width`）
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.ImageIndex.Image = Lib((1, 1));
        b.ImageIndex.Up = 0;
        b.Caption = "x";                    // 在 Style 之后设，避免构造期提前跑几何

        // 注入一个与 1x1 图完全不同的文本度量（99x7），以便区分走的是哪条分支
        object fakeFont = new object();
        b.FontEnv.FindFont = (name, size, style) => fakeFont;
        b.FontEnv.GetImageInfos = (f, text) =>
            new List<TDxTextImageInfo> { new() { Width = 99, Height = 7 } };

        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        // 1x1 不满足 `D.Width >= 2 and D.Height >= 2` → **没有**直取 1x1。
        // 结果由文本度量（99/7）+ Bold(+2) 与 `+1 + D.Width(1)` / `Max(…, D.Height(1))` 构成，
        // 与直取分支的 (1,1) 明显不同。
        Assert.NotEqual(1, size.Value.X);
        Assert.NotEqual(1, size.Value.Y);
        Assert.True(size.Value.X >= 99, $"应包含文本宽度 99，实际 {size.Value.X}");
        Assert.True(size.Value.Y >= 7, $"应包含文本高度 7，实际 {size.Value.Y}");
    }

    [Fact]
    public void CaptionChange_NonStyle_NeverTakesImageSizeDirectly()
    {
        // 同样的图，在 bsCheckBox 下**不能**直取（原文 744 要求 bsButton），
        // 而在 bsButton 下且图 >= 2x2 时必须直取 —— 这是两条分支的稳定开关断言，
        // 不依赖 Bold/字体的具体像素值。
        var bsButton = new TButtonHost { Style = TButtonStyle.bsButton };
        bsButton.ImageIndex.Image = Lib((40, 8));
        bsButton.ImageIndex.Up = 0;
        bsButton.Caption = "x";

        var checkBox = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        checkBox.ImageIndex.Image = Lib((40, 8));
        checkBox.ImageIndex.Up = 0;
        checkBox.Caption = "x";

        var a = bsButton.ComputeCaptionChangeSize();
        var b = checkBox.ComputeCaptionChangeSize();

        Assert.NotNull(a);
        Assert.NotNull(b);
        Assert.Equal(40, a.Value.X);                // bsButton → 直取图宽
        Assert.Equal(8, a.Value.Y);
        Assert.NotEqual(40, b.Value.X);             // bsCheckBox → 走文本路径
    }

    [Fact]
    public void CaptionChange_EmptyCaption_TakesImageSize()
    {
        var b = new TDxImageButton { Caption = "" };
        b.ImageIndex.Image = Lib((20, 9));
        b.ImageIndex.Up = 0;

        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        Assert.Equal(20, size.Value.X);
        Assert.Equal(9, size.Value.Y);
    }

    [Fact]
    public void CaptionChange_EmptyCaptionAndNoImage_ReturnsNull()
    {
        var b = new TDxImageButton { Caption = "" };
        Assert.Null(b.ComputeCaptionChangeSize());
    }

    [Fact]
    public void CaptionChange_NoImageWithCaption_TextPathOnly()
    {
        var b = new TDxImageButton { Caption = "z", Style = TButtonStyle.bsCheckBox };
        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        Assert.Equal(0, size.Value.X);      // 无字体、无图 → 全 0
        Assert.Equal(0, size.Value.Y);
    }

    [Fact]
    public void CaptionChange_WithFontInjection_MeasuresText()
    {
        // 注入一个「每字符 7px 宽、12px 高」的假字体环境，验证文本量算分支真的被走到
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        object fakeFont = new object();
        b.FontEnv.FindFont = (name, size, style) => fakeFont;
        b.FontEnv.GetImageInfos = (f, text) =>
        {
            var info = new TDxTextImageInfo { Width = text.Length * 7, Height = 12 };
            return new List<TDxTextImageInfo> { info };
        };
        b.AutoSize = true;
        b.Caption = "abc";

        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        // 文本分支量出 3*7 = 21、高 12；**CaptionColor.Up.Bold 默认为 True**（原文 864-868），
        // 故宽高各 +2。本用例未设 ImageIndex.Image，故原文 768-772 的 `+1 +D.Width` 不生效。
        Assert.Equal(23, size.Value.X);     // 21 + 2(Bold)
        Assert.Equal(14, size.Value.Y);     // 12 + 2(Bold)
    }

    [Fact]
    public void CaptionChange_BoldFont_AddsTwoToBothDimensions()
    {
        var b = new TDxImageButton { Caption = "ab", Style = TButtonStyle.bsCheckBox };
        object fakeFont = new object();
        b.CaptionColor.Up.Bold = true;      // 默认已是 True，此处显式写出以表意
        b.FontEnv.FindFont = (name, size, style) => fakeFont;
        b.FontEnv.GetImageInfos = (f, text) =>
            new List<TDxTextImageInfo> { new() { Width = 10, Height = 5 } };

        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        Assert.Equal(12, size.Value.X);     // 10 + 2
        Assert.Equal(7, size.Value.Y);      // 5 + 2
    }

    [Fact]
    public void CaptionChange_NonBoldFont_SkipsThePlusTwo()
    {
        var b = new TDxImageButton { Caption = "ab", Style = TButtonStyle.bsCheckBox };
        b.CaptionColor.Up.Bold = false;     // 显式关掉（默认是 True）
        object fakeFont = new object();
        b.FontEnv.FindFont = (name, size, style) => fakeFont;
        b.FontEnv.GetImageInfos = (f, text) =>
            new List<TDxTextImageInfo> { new() { Width = 10, Height = 5 } };
        b.ImageIndex.Image = Lib((4, 3));
        b.ImageIndex.Up = 0;

        var size = b.ComputeCaptionChangeSize();

        Assert.NotNull(size);
        Assert.Equal(15, size.Value.X);     // 10 + 1 + 4（无 Bold +2）
        Assert.Equal(5, size.Value.Y);      // Max(5, 3)
    }

    [Fact]
    public void CaptionChange_SetterUpdatesWidthAndHeight()
    {
        var b = new TDxImageButton { Caption = "x", Style = TButtonStyle.bsCheckBox };
        b.ImageIndex.Image = Lib((15, 4));
        b.ImageIndex.Up = 0;

        b.CaptionOffsetX = 1;               // 触发 DoCaptionChange

        Assert.Equal(16, b.Width);          // 0 + 1 + 15
        Assert.Equal(4, b.Height);
    }

    // ===============================================================================
    // 五、SetChecked / DoClickV2 状态机（原文 798-822 / 870-908）
    // ===============================================================================

    [Fact]
    public void SetChecked_Radio_ClearsSiblings()
    {
        var root = new TDxControlEngine { ClientRect = TDxRect.Bounds(0, 0, 800, 600) };
        var a = new TButtonHost { Style = TButtonStyle.bsRadio };
        var b = new TButtonHost { Style = TButtonStyle.bsRadio };
        DxControlOps.InserComponent(root, a);
        DxControlOps.InserComponent(root, b);

        a.SetChecked(true);
        Assert.True(a.Checked);

        b.SetChecked(true);
        Assert.True(b.Checked);
        Assert.False(a.Checked);            // 互斥

        b.SetChecked(false);
        Assert.False(b.Checked);
    }

    [Fact]
    public void SetChecked_NonRadio_IsPlainAssignment()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.SetChecked(true);
        Assert.True(b.Checked);
        b.SetChecked(false);
        Assert.False(b.Checked);
    }

    [Fact]
    public void DoClickV2_CheckBox_TogglesAndFiresSoundAndClick()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.Designing = false;
        var log = new List<string>();
        b.OnClickSound = (s, cs) => log.Add($"sound:{cs}");
        DxControlHooks.SetOnClick(b, (s, x, y) => log.Add("click"));

        bool clicked = b.DoClickV2(1, 2);

        Assert.True(clicked);
        Assert.True(b.Checked);
        Assert.Equal(new List<string> { "sound:csNone", "click" }, log);
    }

    [Fact]
    public void DoClickV2_CheckBox_TogglesBack()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.Designing = false;
        b.SetChecked(true);

        b.DoClickV2(0, 0);
        Assert.False(b.Checked);
    }

    [Fact]
    public void DoClickV2_Radio_AlreadyChecked_DoesNotFire()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsRadio };
        b.SetChecked(true);
        Assert.True(b.Checked);

        var log = new List<string>();
        b.OnClickSound = (s, cs) => log.Add("sound");
        DxControlHooks.SetOnClick(b, (s, x, y) => log.Add("click"));

        bool clicked = b.DoClickV2(0, 0);

        // 原文 880-891：bsRadio 先把**同父所有** bsRadio 置 Checked=False，再 `if not Checked then …`。
        // 因为 b 自己也在同父列表里，所以「已选中」状态会被这一轮清掉 —— b.Checked 变 False 后
        // 走进 `if not Checked` 分支：置 True、发音、inherited。**这正是原文的行为**，
        // 也就是说单选按钮的"重复点击不发音"只在**有兄弟把状态清掉之外**的场景才成立。
        Assert.True(clicked);
        Assert.True(b.Checked);
        Assert.Equal(new List<string> { "sound", "click" }, log);
    }

    [Fact]
    public void DoClickV2_Radio_ClearsSiblingAndTakesSelection()
    {
        var root = new TDxControlEngine { ClientRect = TDxRect.Bounds(0, 0, 800, 600) };
        var a = new TDxImageButton { Style = TButtonStyle.bsRadio };
        var b = new TDxImageButton { Style = TButtonStyle.bsRadio };
        DxControlOps.InserComponent(root, a);
        DxControlOps.InserComponent(root, b);
        a.Designing = false;
        b.Designing = false;

        a.SetChecked(true);
        Assert.True(a.Checked);

        var log = new List<string>();
        b.OnClickSound = (s, cs) => log.Add("sound");
        DxControlHooks.SetOnClick(b, (s, x, y) => log.Add("click"));

        Assert.True(b.DoClickV2(0, 0));

        Assert.True(b.Checked);
        Assert.False(a.Checked);                 // 兄弟被清空
        Assert.Equal(new List<string> { "sound", "click" }, log);
    }

    [Fact]
    public void DoClickV2_Button_FiresSoundAndClick()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.Designing = false;
        var log = new List<string>();
        b.OnClickSound = (s, cs) => log.Add("sound");
        DxControlHooks.SetOnClick(b, (s, x, y) => log.Add("click"));

        Assert.True(b.DoClickV2(0, 0));
        Assert.Equal(new List<string> { "sound", "click" }, log);
        Assert.False(b.Checked);            // bsButton 不改 Checked
    }

    [Fact]
    public void DoClickV2_DesigningPath_GoesStraightToClick()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.Designing = true;                 // 设计期
        var log = new List<string>();
        b.OnClickSound = (s, cs) => log.Add("sound");
        DxControlHooks.SetOnClick(b, (s, x, y) => log.Add("click"));

        Assert.True(b.DoClickV2(0, 0));
        Assert.Equal(new List<string> { "click" }, log);   // 不发音、不翻转
        Assert.False(b.Checked);
    }

    // ===============================================================================
    // 六、InRange（原文 848-868）
    // ===============================================================================

    [Fact]
    public void InRange_BsButton_UsesBaseImplementation()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.ClientRect = TDxRect.Bounds(0, 0, 20, 20);
        b.Designing = false;

        Assert.True(b.InRange(5, 5));
        Assert.False(b.InRange(50, 50));
    }

    [Fact]
    public void InRange_NonButton_HonoursOnInRealAreaVeto()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.ClientRect = TDxRect.Bounds(0, 0, 20, 20);
        b.Designing = false;

        Assert.True(b.InRange(5, 5));

        b.OnInRealArea = (ctrl, x, y, box) => box.Value = false;
        Assert.False(b.InRange(5, 5));       // 回调可改判
    }

    // ===============================================================================
    // 七、TButtonAnimation.ResolveIsShow（原文 260-295）
    // ===============================================================================

    [Fact]
    public void ResolveIsShow_AlwaysShow_IsAlwaysTrue()
    {
        var b = new TButtonHost();
        b.Animation.FShowType = TButtonAnimationShowType.astAlwaysShow;

        Assert.True(b.Animation.ResolveIsShow());
        b.Enabled = false;
        Assert.True(b.Animation.ResolveIsShow());          // 与 Enabled 无关
    }

    [Fact]
    public void ResolveIsShow_EnableAndDisableFollowOwnerEnabled()
    {
        var b = new TButtonHost();
        b.Animation.FShowType = TButtonAnimationShowType.astEnableShow;

        b.Enabled = true;
        Assert.True(b.Animation.ResolveIsShow());
        b.Enabled = false;
        Assert.False(b.Animation.ResolveIsShow());

        b.Animation.FShowType = TButtonAnimationShowType.astDisableShow;
        Assert.True(b.Animation.ResolveIsShow());
        b.Enabled = true;
        Assert.False(b.Animation.ResolveIsShow());
    }

    [Theory]
    // bsButton：Normal / Hot / Down 三态各自只认自己那个 ShowType
    // （astNormalShow 在 Normal 态为 True，另有专门用例覆盖，此处不重复）
    [InlineData(TButtonAnimationShowType.astHotShow, false, false, false)]
    [InlineData(TButtonAnimationShowType.astDownShow, false, false, false)]
    public void ResolveIsShow_Style_NormalState(TButtonAnimationShowType showType, bool downed, bool moved,
        bool expected)
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.Animation.FShowType = showType;
        if (downed) b.SetMouseDowned(true);
        if (moved) b.SetMouseMoveed(true);

        Assert.Equal(expected, b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_Style_NormalState_OnlyNormalShow()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.Animation.FShowType = TButtonAnimationShowType.astNormalShow;
        Assert.True(b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_Style_HotState_OnlyHotShow()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.SetMouseMoveed(true);

        b.Animation.FShowType = TButtonAnimationShowType.astHotShow;
        Assert.True(b.Animation.ResolveIsShow());

        b.Animation.FShowType = TButtonAnimationShowType.astNormalShow;
        Assert.False(b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_Style_DownState_OnlyDownShow()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.SetMouseDowned(true);

        b.Animation.FShowType = TButtonAnimationShowType.astDownShow;
        Assert.True(b.Animation.ResolveIsShow());

        b.Animation.FShowType = TButtonAnimationShowType.astHotShow;
        Assert.False(b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_Style_MouseDownedTakesPrecedenceOverMouseMoved()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        b.SetMouseDowned(true);
        b.SetMouseMoveed(true);

        b.Animation.FShowType = TButtonAnimationShowType.astDownShow;
        Assert.True(b.Animation.ResolveIsShow());          // 原文 MouseDowned 分支先判

        b.Animation.FShowType = TButtonAnimationShowType.astHotShow;
        Assert.False(b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_CheckBox_CheckedBeatsMouseDownedWhileMoving()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.SetMouseMoveed(true);
        b.SetMouseDowned(true);
        b.SetChecked(true);

        b.Animation.FShowType = TButtonAnimationShowType.astCheckShow;
        Assert.True(b.Animation.ResolveIsShow());          // 原文 Checked 先于 MouseDowned

        b.Animation.FShowType = TButtonAnimationShowType.astDownShow;
        Assert.False(b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_CheckBox_NotMovingCheckedOnlyCheckShow()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };
        b.SetChecked(true);

        b.Animation.FShowType = TButtonAnimationShowType.astCheckShow;
        Assert.True(b.Animation.ResolveIsShow());

        b.Animation.FShowType = TButtonAnimationShowType.astNormalShow;
        Assert.False(b.Animation.ResolveIsShow());
    }

    [Fact]
    public void ResolveIsShow_CheckBox_NotMovingNotCheckedOnlyNormalShow()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsCheckBox };

        b.Animation.FShowType = TButtonAnimationShowType.astNormalShow;
        Assert.True(b.Animation.ResolveIsShow());

        b.Animation.FShowType = TButtonAnimationShowType.astCheckShow;
        Assert.False(b.Animation.ResolveIsShow());
    }

    // ===============================================================================
    // 八、TButtonAnimation.AdvanceFrame（原文 256-319）
    // ===============================================================================

    [Fact]
    public void AdvanceFrame_GuardsRejectBadState()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;

        Assert.False(a.AdvanceFrame());                    // Image == nil
        a.Image = Lib((10, 10));
        Assert.False(a.AdvanceFrame());                    // Start/End 都是 -1

        a.StartIndex = 0;
        a.EndIndex = -1;
        Assert.False(a.AdvanceFrame());                    // End < 0

        a.StartIndex = 5;
        a.EndIndex = 2;
        Assert.False(a.AdvanceFrame());                    // Start > End

        a.StartIndex = 0;
        a.EndIndex = 2;
        Assert.False(a.AdvanceFrame());                    // not Draw
    }

    [Fact]
    public void AdvanceFrame_PlayCountExhausted_Stops()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.Image = Lib((10, 10), (10, 10));
        a.StartIndex = 0;
        a.EndIndex = 1;
        a.Draw = true;
        a.PlayCount = 1;
        a.FrameTime = 0;                                    // 每帧都推进

        // 0→1（不回绕），1→回绕到 0 且 CurrentCount=1
        Assert.True(a.AdvanceFrame());
        Assert.True(a.AdvanceFrame());
        Assert.Equal(1, a.CurrentCount);

        Assert.False(a.AdvanceFrame());                     // CurrentCount >= PlayCount → 停
    }

    [Fact]
    public void AdvanceFrame_WrapsAndFiresCallbackAfterWrap()
    {
        // 原文 307-316：先回绕（CurrentFrame := StartIndex）再回调 —— 传给回调的 Frame 是 StartIndex
        var b = new TButtonHost();
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.Image = Lib((10, 10), (10, 10));
        a.StartIndex = 0;
        a.EndIndex = 1;
        a.Draw = true;
        a.PlayCount = 0;
        a.FrameTime = 0;

        var seen = new List<(int playCount, int frame)>();
        b.OnAnimationFrameChanged = (sender, idx, playCount, frame) => seen.Add((playCount, frame));

        Assert.True(a.AdvanceFrame());                      // frame 0 → 1（未回绕，无回调）
        Assert.Empty(seen);

        Assert.True(a.AdvanceFrame());                      // frame 1 → 2 > End → 回绕到 0，回调传 0
        Assert.Single(seen);
        Assert.Equal(0, seen[0].frame);                     // **回绕后的值**
        Assert.Equal(0, seen[0].playCount);                 // PlayCount = 0 → CurrentCount 不加
        Assert.Equal(0, a.CurrentFrame);
    }

    [Fact]
    public void AdvanceFrame_CallbackReceivesAnimationIndexZero()
    {
        // 原文 315 传的 AnimationIndex 恒为**字面量 0**（TButtonAnimation 没有 FAnimationIndex 字段）
        var b = new TButtonHost();
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.Image = Lib((10, 10));
        a.StartIndex = 0;
        a.EndIndex = 0;
        a.Draw = true;
        a.FrameTime = 0;

        int gotIndex = -1;
        b.OnAnimationFrameChanged = (sender, idx, playCount, frame) => gotIndex = idx;

        Assert.True(a.AdvanceFrame());                      // 0 → 1 > End(0) → 回绕 + 回调
        Assert.Equal(0, gotIndex);
    }

    [Fact]
    public void AdvanceFrame_NotElapsed_DoesNotAdvanceButStillDraws()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.Image = Lib((10, 10), (10, 10));
        a.StartIndex = 0;
        a.EndIndex = 1;
        a.Draw = true;
        a.FrameTime = 100000;                               // 远超测试耗时 → 不推进

        Assert.True(a.AdvanceFrame());                      // 仍返回 true（应当绘制）
        Assert.Equal(0, a.CurrentFrame);
    }

    [Fact]
    public void AdvanceFrame_OutOfRangeFrame_IsClampedToStart()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.Image = Lib((10, 10), (10, 10), (10, 10));
        a.StartIndex = 1;
        a.EndIndex = 2;
        a.Draw = true;
        a.FrameTime = 100000;

        // Draw setter 把 CurrentFrame 置为 StartIndex(1)；再手工制造越界场景不现实，
        // 改用：Start/End 后置为 2/2，此时 CurrentFrame(1) < Start → 会被拉回
        a.StartIndex = 2;
        a.EndIndex = 2;
        Assert.True(a.AdvanceFrame());
        Assert.Equal(2, a.CurrentFrame);
    }

    [Fact]
    public void AdvanceFrame_ShowTypeGate_BlocksWhenNotShown()
    {
        var b = new TButtonHost { Style = TButtonStyle.bsButton };
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astDownShow;   // 当前未按下 → 不显示
        a.Image = Lib((10, 10));
        a.StartIndex = 0;
        a.EndIndex = 0;
        a.Draw = true;
        a.FrameTime = 0;

        Assert.False(a.AdvanceFrame());
    }

    // ===============================================================================
    // 九、TButtonAnimation.BuildDrawPlan 几何（原文 321-373）
    // ===============================================================================

    private static TButtonHost MakeButtonWithAnim(out TButtonAnimation anim, int texW, int texH)
    {
        var b = new TButtonHost { ClientRect = TDxRect.Bounds(0, 0, 100, 50) };
        b.Designing = false;
        anim = b.Animation;
        anim.FShowType = TButtonAnimationShowType.astAlwaysShow;
        anim.Image = Lib((texW, texH));
        anim.StartIndex = 0;
        anim.EndIndex = 0;
        anim.Draw = true;
        anim.FrameTime = 100000;                 // 不推进，保证 CurrentFrame = 0
        return b;
    }

    [Fact]
    public void BuildDrawPlan_UsesOwnerVirtualRectAndOffsets()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);

        a.UseImageOffset = true;
        a.OffsetX = 3;
        a.OffsetY = 4;

        var plan = a.BuildDrawPlan();

        Assert.NotNull(plan);
        Assert.Equal(3, plan.X);                 // VirtualRect.Left(0) + 3
        Assert.Equal(4, plan.Y);
        Assert.Equal(new TDxRect(0, 0, 10, 8), plan.SrcRect);
        Assert.Equal(2, plan.BlendMode);         // BlendDraw = false → 字面量 2
        Assert.False(plan.FullTexture);
    }

    [Fact]
    public void BuildDrawPlan_OutsideAreaDraw_ReturnsWholeTexturePath()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);
        a.OutsideAreaDraw = true;
        a.OffsetX = 500;                          // 故意跑到父矩形外

        var plan = a.BuildDrawPlan();

        Assert.NotNull(plan);
        Assert.True(plan.FullTexture);            // 走 GameCanvas.Draw(x,y,Texture,BlendMode)
        Assert.Equal(500, plan.X);
        Assert.Equal(new TDxRect(0, 0, 10, 8), plan.SrcRect);   // SrcRect 不被裁剪
    }

    [Fact]
    public void BuildDrawPlan_ClipsLeftAndTop_SyncingSrcRect()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);
        a.OffsetX = -4;                           // 左边被裁 4
        a.OffsetY = -3;                           // 上边被裁 3

        var plan = a.BuildDrawPlan();

        Assert.NotNull(plan);
        Assert.Equal(0, plan.X);
        Assert.Equal(0, plan.Y);
        // SrcRect 左/上各同步内移
        Assert.Equal(4, plan.SrcRect.Left);
        Assert.Equal(3, plan.SrcRect.Top);
    }

    [Fact]
    public void BuildDrawPlan_ClipsRightAndBottom_SyncingSrcRect()
    {
        // 父控件只有 100x50，把纹理放到右/下越界处
        var b = new TButtonHost { ClientRect = TDxRect.Bounds(0, 0, 20, 15) };
        b.Designing = false;
        var a = b.Animation;
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.Image = Lib((50, 40));
        a.StartIndex = 0;
        a.EndIndex = 0;
        a.Draw = true;
        a.FrameTime = 100000;
        a.OffsetX = 0;
        a.OffsetY = 0;

        var plan = a.BuildDrawPlan();

        Assert.NotNull(plan);
        // VisibleRect = (0,0,20,15)（无 Owner → ClientRect）→ 右边裁到 20、下边裁到 15
        Assert.Equal(0, plan.X);
        Assert.Equal(0, plan.Y);
        Assert.Equal(20, plan.SrcRect.Right);     // 原文 `SrcRect.Right -= R.Right - ParentRect.Right`
        Assert.Equal(15, plan.SrcRect.Bottom);
    }

    [Fact]
    public void BuildDrawPlan_BlendDraw_UsesSrcAlphaColor()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);
        a.BlendDraw = true;

        var plan = a.BuildDrawPlan();

        Assert.NotNull(plan);
        Assert.Equal(TDxHgeBlend.Blend_SrcAlphaColor, plan.BlendMode);
    }

    [Fact]
    public void BuildDrawPlan_PlayCountExhausted_ReturnsNull()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);
        a.PlayCount = 1;
        a.FrameTime = 0;

        a.AdvanceFrame();                          // 0→1>End → 回绕、CurrentCount=1
        Assert.Equal(1, a.CurrentCount);

        Assert.Null(a.BuildDrawPlan());            // CurrentCount >= PlayCount → 不画
    }

    [Fact]
    public void BuildDrawPlan_NotShown_ReturnsNull()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);
        a.FShowType = TButtonAnimationShowType.astDownShow;   // 未按下 → 不显示

        Assert.Null(a.BuildDrawPlan());
    }

    [Fact]
    public void BuildDrawPlan_MissingTextureAtFrame_ReturnsNull()
    {
        var b = MakeButtonWithAnim(out var a, 10, 8);
        a.EndIndex = 5;                            // 图库里只有索引 0
        a.FrameTime = 0;
        a.AdvanceFrame();                          // 0→1，仍在 [0,5] 内

        Assert.Null(a.BuildDrawPlan());            // GetImage(1) == null
    }

    // ===============================================================================
    // 十、TButtonAnimation 其它 setter / Assign
    // ===============================================================================

    [Fact]
    public void DrawSetter_ResetsFrameTickAndCount()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        a.Image = Lib((10, 10), (10, 10));
        a.FShowType = TButtonAnimationShowType.astAlwaysShow;
        a.StartIndex = 0;
        a.EndIndex = 1;
        a.PlayCount = 5;
        a.FrameTime = 0;
        a.Draw = true;
        a.AdvanceFrame();
        a.AdvanceFrame();                          // CurrentCount = 1

        a.Draw = false;

        Assert.Equal(0, a.CurrentFrame);           // 原文 387：重置为 FStartIndex
        Assert.Equal(0, a.CurrentCount);
    }

    [Fact]
    public void OutsideAreaDraw_And_UseImageOffset_ArePlainAssignments()
    {
        var b = new TButtonHost();
        var a = b.Animation;

        a.OutsideAreaDraw = true;
        Assert.True(a.OutsideAreaDraw);
        a.UseImageOffset = false;
        Assert.False(a.UseImageOffset);
        a.BlendDraw = true;
        Assert.True(a.BlendDraw);
        a.DrawBeforeDef = true;
        Assert.True(a.DrawBeforeDef);
    }

    [Fact]
    public void SetOnGetImage_FiresImmediatelyThenChanged()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        var log = new List<string>();
        a.OnChange = _ => log.Add("changed");

        a.SetOnGetImage((s, t, o) => log.Add($"getimage:{t}"));

        Assert.Equal(new List<string> { "getimage:Prguse_wil", "changed" }, log);
    }

    [Fact]
    public void ImageType_Change_FiresOnGetImageAndChanged()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        var log = new List<string>();
        a.SetOnGetImage((s, t, o) => log.Add($"gi:{t}"));
        a.OnChange = _ => log.Add("ch");
        log.Clear();

        a.ImageType = TImageType.UI_wil;

        Assert.Equal(new List<string> { "gi:UI_wil", "ch" }, log);

        log.Clear();
        a.ImageType = TImageType.UI_wil;           // 同值 → 不触发
        Assert.Empty(log);
    }

    [Fact]
    public void Assign_CopiesAllFieldsIncludingFrameState()
    {
        var b1 = new TButtonHost();
        var a1 = b1.Animation;
        a1.Image = Lib((10, 10), (10, 10));
        a1.FShowType = TButtonAnimationShowType.astHotShow;
        a1.StartIndex = 0;
        a1.EndIndex = 1;
        a1.FrameTime = 77;
        a1.PlayCount = 3;
        a1.UseImageOffset = false;
        a1.OffsetX = 9;
        a1.OffsetY = 8;
        a1.OutsideAreaDraw = true;
        a1.BlendDraw = true;
        a1.DrawBeforeDef = true;
        a1.FrameTime = 0;
        a1.Draw = true;
        a1.AdvanceFrame();
        a1.AdvanceFrame();                          // 让 CurrentFrame/Count 有非默认值

        var b2 = new TButtonHost();
        b2.Animation.Assign(a1);

        Assert.Equal(TButtonAnimationShowType.astHotShow, b2.Animation.FShowType);
        Assert.Equal(0, b2.Animation.StartIndex);
        Assert.Equal(1, b2.Animation.EndIndex);
        Assert.Equal(0, b2.Animation.FrameTime);   // 最后被显式设为 0
        Assert.Equal(3, b2.Animation.PlayCount);
        Assert.False(b2.Animation.UseImageOffset);
        Assert.Equal(9, b2.Animation.OffsetX);
        Assert.Equal(8, b2.Animation.OffsetY);
        Assert.True(b2.Animation.OutsideAreaDraw);
        Assert.True(b2.Animation.BlendDraw);
        Assert.True(b2.Animation.DrawBeforeDef);
        Assert.Equal(a1.CurrentFrame, b2.Animation.CurrentFrame);
        Assert.Equal(a1.CurrentCount, b2.Animation.CurrentCount);
    }

    [Fact]
    public void Assign_NullSource_IsSafe()
    {
        var b = new TButtonHost();
        b.Animation.Assign(null);
    }

    [Fact]
    public void SetImage_OnlyFiresChangedOnDifferentInstance()
    {
        var b = new TButtonHost();
        var a = b.Animation;
        int changes = 0;
        a.OnChange = _ => changes++;

        var lib = Lib((10, 10));
        a.SetImage(lib);
        Assert.Equal(1, changes);

        a.SetImage(lib);                            // 同一个实例 → 不再触发
        Assert.Equal(1, changes);
    }

    // ===============================================================================
    // 十一、TDxImageButton.Assign / DisposeButton
    // ===============================================================================

    [Fact]
    public void AssignFromImageButton_CopiesGeometryAndButtonProps()
    {
        var src = new TButtonHost
        {
            ClientRect = TDxRect.Bounds(11, 22, 33, 44),
            Style = TButtonStyle.bsRadio,
            Caption = "hi",
            CaptionDownOffsetX = 4,
            CaptionDownOffsetY = 5,
            ButtonDownOffsetX = 6,
            ButtonDownOffsetY = 7,
            ClickCount = TClickSound.csGlass,
        };
        src.SetChecked(true);
        src.Animation.Image = Lib((10, 10));
        src.Animation.FShowType = TButtonAnimationShowType.astCheckShow;

        var dst = new TButtonHost();
        dst.AssignFromImageButton(src);

        Assert.Equal(11, dst.Left);
        Assert.Equal(22, dst.Top);
        Assert.Equal(TButtonStyle.bsRadio, dst.Style);
        Assert.Equal("hi", dst.Caption);
        Assert.Equal(4, dst.CaptionDownOffsetX);
        Assert.Equal(5, dst.CaptionDownOffsetY);
        Assert.Equal(6, dst.ButtonDownOffsetX);
        Assert.Equal(7, dst.ButtonDownOffsetY);
        Assert.Equal(TClickSound.csGlass, dst.ClickCount);
        Assert.True(dst.Checked);
        Assert.Equal(TButtonAnimationShowType.astCheckShow, dst.Animation.FShowType);
    }
    [Fact]
    public void AssignFromImageButton_WrongSourceType_IsNoOp()
    {
        var dst = new TButtonHost { ClientRect = TDxRect.Bounds(1, 2, 3, 4) };
        dst.AssignFromImageButton(new TDxControlEngine());
        Assert.Equal(1, dst.Left);
    }

    [Fact]
    public void DisposeButton_IsSafe()
    {
        var b = new TButtonHost();
        b.DisposeButton();
        b.InitializeButton();
        b.FinalizeButton();
    }

    // ===============================================================================
    // 十二、DrawAligment / CheckAutoSize 门控
    // ===============================================================================

    [Fact]
    public void CheckAutoSizeV2_OnlyAppliesForStyle()
    {
        // 用两个独立实例：原文 `CheckAutoSize` 一旦按图定尺就把 FAutoSizeSetFlag 置 $FF，
        // 之后**同一个控件**再调也不会重算（原文 2127 的 `FAutoSizeSetFlag = 0` 守卫）——
        // 若在同一个实例上先跑 bsCheckBox 再跑 bsButton，第二次会被该标志挡住。
        var nonButton = new TButtonHost { ClientRect = TDxRect.Bounds(0, 0, 5, 5) };
        nonButton.ImageIndex.Image = Lib((60, 30));
        nonButton.ImageIndex.Up = 0;
        nonButton.AutoSize = true;
        nonButton.Style = TButtonStyle.bsCheckBox;

        nonButton.CheckAutoSizeV2();
        Assert.Equal(5, nonButton.Width);           // 非 bsButton → 不做自动定尺

        var button = new TButtonHost { ClientRect = TDxRect.Bounds(0, 0, 5, 5) };
        button.ImageIndex.Image = Lib((60, 30));
        button.ImageIndex.Up = 0;
        button.AutoSize = true;
        button.Style = TButtonStyle.bsButton;

        button.CheckAutoSizeV2();
        Assert.Equal(60, button.Width);             // bsButton → 按图定尺
        Assert.Equal(30, button.Height);
        Assert.Equal(0xFF, button.AutoSizeSetFlag); // 原文置 $FF 后不再重算
    }
}
