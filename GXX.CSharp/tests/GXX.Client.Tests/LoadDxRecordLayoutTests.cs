using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// GuiRecords.g.cs 的**回读比对**测试。
//
// Full_Offset_Table_Matches_Source_Derived_Audit 里的期望表**不是手抄的**：
// 它等于 Tools/GenGuiRecords.ps1 跑完后 stdout 的 AUDIT 段（脚本从 DxComponents.pas 抽取
// 声明、按 Delphi {$A8}/{$MINENUMSIZE 1} 规则算偏移后打印）。
// 于是"生成器 → 生成的 C# → 运行时反射出来的布局"这条链被整表锁死：
// 任何人改生成器、改生成记录、或手工编辑 GuiRecords.g.cs 都会立刻红灯。
// =====================================================================================
public sealed class LoadDxRecordLayoutTests
{
    /// <summary>Generate/Dump 的格式：<c>类型名;SizeOf;Align;字段@偏移+宽度 ...</c></summary>
    private static string Dump(DelphiRecordLayout layout)
    {
        var sb = new StringBuilder();
        sb.Append(layout.Name).Append(';').Append(layout.Size).Append(';').Append(layout.Align).Append(';');
        for (int i = 0; i < layout.Fields.Length; i++)
        {
            if (i > 0) sb.Append(' ');
            sb.Append(layout.Fields[i].Name).Append('@').Append(layout.Fields[i].Offset).Append('+').Append(layout.Fields[i].Size);
        }
        return sb.ToString();
    }

    /// <summary>反射取出 GXX.Client.LoadDx 下所有记录的 <c>Layout</c> 静态字段，按类型名排序。</summary>
    internal static DelphiRecordLayout[] AllLayouts()
    {
        var assembly = typeof(TGuiHeader).Assembly;
        var list = new List<DelphiRecordLayout>();
        foreach (var type in assembly.GetTypes())
        {
            if (type.Namespace != "GXX.Client.LoadDx") continue;
            var field = type.GetField("Layout", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (field != null && field.FieldType == typeof(DelphiRecordLayout))
                list.Add((DelphiRecordLayout)field.GetValue(null));
        }
        return list.OrderBy(l => l.Name, StringComparer.Ordinal).ToArray();
    }

    [Fact]
    public void Full_Offset_Table_Matches_Source_Derived_Audit()
    {
        var expected = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in ExpectedAudit()) expected[line.Substring(0, line.IndexOf(';'))] = line;

        var actual = AllLayouts().ToDictionary(l => l.Name, Dump, StringComparer.Ordinal);

        Assert.Equal(expected.Count, actual.Count);
        foreach (var pair in expected)
        {
            Assert.True(actual.ContainsKey(pair.Key), $"missing generated record {pair.Key}");
            Assert.Equal(pair.Value, actual[pair.Key]);
        }
    }

    /// <summary>期望值 = GenGuiRecords.ps1 的 AUDIT 输出（脚本从原文抽取 + 计算，非人工转录）。</summary>
    private static string[] ExpectedAudit() => new[]
        {
            "TBackgroundImage;16;4;ImageType@0+1 BlendMode@1+1 OutsideAreaDraw@2+1 Reserve@3+1 ImageIndex@4+4 OffsetX@8+4 OffsetY@12+4",
            "TGuiAnimation;32;4;ImageType@0+1 DrawBeforeDef@1+1 Reserve@2+2 StartIndex@4+4 EndIndex@8+4 FrameTime@12+4 PlayCount@16+4 OffsetX@20+4 OffsetY@24+4 UseImageOffset@28+1 OutsideAreaDraw@29+1 Draw@30+1 BlendDraw@31+1",
            "TGuiButtonAnimation;36;4;ImageType@0+1 DrawBeforeDef@1+1 Reserve@2+2 ShowType@4+1 StartIndex@8+4 EndIndex@12+4 FrameTime@16+4 PlayCount@20+4 OffsetX@24+4 OffsetY@28+4 UseImageOffset@32+1 OutsideAreaDraw@33+1 Draw@34+1 BlendDraw@35+1",
            "TGuiCaptionColor;96;4;Up@0+24 Hot@24+24 Down@48+24 Disabled@72+24",
            "TGuiComboBox;428;4;GuiPopupMenu@0+216 DrawBorder@216+1 ButtonColor@220+4 BackgroundColor@224+4 TextColor@228+96 BorderColor@324+96 TextLen@420+4 ItemLen@424+4",
            "TGuiEdit;160;4;DrawBorder@0+1 SelectedColor@4+4 SelBackColor@8+4 SelFontColor@12+4 BackgroundColor@16+4 FontColor@20+24 BorderColor@44+96 ReadOnly@140+1 MaxLength@144+4 InValue@148+1 PasswordChar@149+1 AllowSelect@150+1 AllowPaste@151+1 TabOrder@152+4 TextLen@156+4",
            "TGuiFileHeader;56;1;sDesc@0+36 ClientVersion@36+1 Reserve@37+1 GroupCount@38+2 nGuiVersion@40+4 nCount@44+4 dCreateDate@48+8",
            "TGuiFont;24;4;Color@0+4 BColor@4+4 Style@8+1 Size@12+4 Bold@16+1 NameLen@20+4",
            "TGuiFormShapeInfo;52;4;ImageType@0+1 ImageIndex@4+4 Draw@8+1 Stretch@9+1 Center@10+1 BlendMode@12+4 Align@16+1 SrcRect@20+16 DestRect@36+16",
            "TGuiGroupAttackProgress;204;4;ProgressAlignment@0+1 Settings@4+144 Reserve@148+56",
            "TGuiGroupAttackProgressSetting;48;4;ImageType@0+1 Background@4+4 Progress@8+4 FlashStart@12+4 FlashEnd@16+4 FlashInterval@20+4 OffsetX1@24+4 OffsetY1@28+4 OffsetX2@32+4 OffsetY2@36+4 OffsetX3@40+4 OffsetY3@44+4",
            "TGuiHeader;40;4;Gui@0+1 Left@4+4 Top@8+4 Width@12+4 Height@16+4 Enabled@20+1 Visible@21+1 Transparent@22+1 EnableFocus@23+1 Floating@24+1 OwnerMove@25+1 MouseEvents@26+1 NameLen@28+4 Background@32+4 Count@36+4",
            "TGuiHeaderAdd;48;4;ShowNameLen@0+4 ReferenceX@4+1 AdjustYByHeight@5+1 TopAlignment@6+1 Reserverd@7+1 HintTextLen@8+2 Reserverd2@10+2 Reserve@12+36",
            "TGuiImageButton;140;4;Alignment@0+1 ImageIndex@4+20 AutoSize@24+1 CaptionColor@28+96 Checked@124+1 ClickCount@125+1 Style@126+1 CaptionDownOffsetX@128+4 CaptionDownOffsetY@132+4 CaptionLen@136+4",
            "TGuiImageButton_New2;152;4;Alignment@0+1 ImageIndex@4+24 AutoSize@28+1 CaptionColor@32+96 Checked@128+1 ClickCount@129+1 Style@130+1 CaptionDownOffsetX@132+4 CaptionDownOffsetY@136+4 CaptionOffsetX@140+4 CaptionOffsetY@144+4 CaptionLen@148+4",
            "TGuiImageButton_New3;188;4;Alignment@0+1 ImageIndex@4+24 AutoSize@28+1 CaptionColor@32+96 Checked@128+1 ClickCount@129+1 Style@130+1 CaptionDownOffsetX@132+4 CaptionDownOffsetY@136+4 CaptionOffsetX@140+4 CaptionOffsetY@144+4 Animation@148+36 CaptionLen@184+4",
            "TGuiImageCheckBox;144;4;Button@0+140 Checked@140+1",
            "TGuiImageEdit;220;4;DrawBorder@0+1 SelectedColor@4+4 SelBackColor@8+4 SelFontColor@12+4 BackgroundColor@16+4 DisableBackgroundColor@20+4 HintTextFont@24+24 HintTextAlignment@48+1 FontColor@52+24 BorderColor@76+96 ReadOnly@172+1 MaxLength@176+4 InValue@180+1 PasswordChar@181+1 AllowSelect@182+1 AllowPaste@183+1 TabOrder@184+4 TextLen@188+4 HintTextLen@192+4 Reserve@196+24",
            "TGuiImageEdit_New;280;4;DrawBorder@0+1 SelectedColor@4+4 SelBackColor@8+4 SelFontColor@12+4 BackgroundColor@16+4 BackgroundColorAlpha@20+1 BackgroundImage@24+16 DisableHideCtrl@40+1 DisableBackgroundTransparent@41+1 DisableBackgroundColor@44+4 DisableBackgroundAlpha@48+1 DisableBackgroundImage@52+16 HintTextFont@68+24 HintTextAlignment@92+1 FontColor@96+24 BorderColor@120+96 ReadOnly@216+1 MaxLength@220+4 InValue@224+1 PasswordChar@225+1 AllowSelect@226+1 AllowPaste@227+1 TabOrder@228+4 TextLen@232+4 HintTextLen@236+4 Reserve@240+40",
            "TGuiImageForm;28;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1",
            "TGuiImageForm_New;48;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1 BackgroundAlpha@25+1 BackgroundColor@28+4 Reserve@32+16",
            "TGuiImageForm_New2;208;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1 BackgroundAlpha@25+1 BackgroundColor@28+4 Animation1@32+32 Animation2@64+32 Animation3@96+32 ImageOffsetX@128+4 ImageOffsetY@132+4 Reserve@136+72",
            "TGuiImageForm_New3;204;4;AutoSize@0+1 ImageIndex@4+16 Center@20+1 BackgroundAlpha@21+1 BackgroundColor@24+4 Animation1@28+32 Animation2@60+32 Animation3@92+32 Reserve@124+80",
            "TGuiImageFormShape;444;4;AutoSize@0+1 ImageIndex@4+20 Center@24+1 ImageIndexs@28+416",
            "TGuiImageGrid;20;4;ColCount@0+4 RowCount@4+4 ColWidth@8+4 RowHeight@12+4 ViewTopLine@16+4",
            "TGuiImageIndex;20;4;Image@0+1 Up@4+4 Hot@8+4 Down@12+4 Disabled@16+4",
            "TGuiImageIndex_Button;24;4;Image@0+1 Up@4+4 Hot@8+4 Down@12+4 Disabled@16+4 Checked@20+4",
            "TGuiImageIndex_Form;16;4;Image@0+1 Up@4+4 OffsetX@8+4 OffsetY@12+4",
            "TGuiImageProgress;236;4;AutoSize@0+1 ImageType@1+1 ImageBG@4+4 ImageProgress@8+4 ImageProgressX@12+4 ImageProgressY@16+4 ValueType@20+1 ValueSplite@21+21 ValueAlignment@42+1 ValuePrefix@43+61 ValueSuffix@104+61 Max@168+4 Min@172+4 Value@176+4 Font@180+24 Reserve@204+32",
            "TGuiLabel;216;4;AutoSize@0+1 DrawBorder@1+1 BackgroundColor@4+4 BorderColor@8+96 CaptionColor@104+96 ClickCount@200+1 Style@201+1 CaptionDownOffsetX@204+4 CaptionDownOffsetY@208+4 CaptionLen@212+4",
            "TGuiLabel_New;216;4;AutoSize@0+1 Alignment@1+1 DrawBorder@2+1 BackgroundColor@4+4 BorderColor@8+96 CaptionColor@104+96 ClickCount@200+1 Style@201+1 CaptionDownOffsetX@204+4 CaptionDownOffsetY@208+4 CaptionLen@212+4",
            "TGuiLine;100;4;LineColor@0+96 LineStyle@96+1",
            "TGuiMagicBall;80;4;BallType@0+1 ValueAlignment@1+1 Overall_ImageType@2+1 Overall_EmptyHPMP@4+4 Overall_FullHPMP@8+4 Overall_EmptyHP@12+4 Overall_FullHP@16+4 Overall_Splite@20+4 Overall_MiddleZoneWidth@24+4 Alone_ImageType@28+1 Alone_Empty@32+4 Alone_Full@36+4 Reserve@40+40",
            "TGuiMagicBall2;116;4;BallType@0+1 ValueAlignment@1+1 Overall_ImageType@2+1 Overall_EmptyHPMP@4+4 Overall_FullHPMP@8+4 Overall_EmptyHP@12+4 Overall_FullHP@16+4 Overall_Splite@20+4 Overall_MiddleZoneWidth@24+4 Overall_EffectDrawBlend@28+1 Overall_EffectImageType@29+1 Overall_EffectHPMPStart@32+4 Overall_EffectHPStart@36+4 Overall_EffectImageCount@40+4 Overall_EffectPlayInterval@44+4 Alone_ImageType@48+1 Alone_Empty@52+4 Alone_Full@56+4 Alone_EffectDrawBlend@60+1 Alone_EffectImageType@61+1 Alone_EffectStart@64+4 Alone_EffectImageCount@68+4 Alone_EffectPlayInterval@72+4 Reserve@76+40",
            "TGuiMainBottomCenter;100;4;Height@0+4 MinHeight@4+4 MaxHeight@8+4 OffsetLeft@12+4 OffsetRight@16+4 DragHeightOffsetY@20+4 DragHeightSize@24+4 AutoStretchSize@28+1 StretchImageFillCenterAlpha@29+1 StretchImageFillCenterColor@32+4 StretchImageFillCenterExpandHorz@36+4 StretchImageFillCenterExpandVert@40+4 StretchImageType@44+1 StretchImageUpLeft@48+4 StretchImageUp@52+4 StretchImageUpRight@56+4 StretchImageLeft@60+4 StretchImageRight@64+4 StretchImageDownLeft@68+4 StretchImageDown@72+4 StretchImageDownRight@76+4 FillImageType@80+1 FillImageIndex@84+4 Reserve@88+12",
            "TGuiMainBottomForm;132;4;LeftImageType@0+1 LeftImageIndex@4+4 RightImageType@8+1 RightImageIndex@12+4 Center@16+100 Reserve@116+16",
            "TGuiMainBottomFormAnimation;52;4;ImageType@0+1 DrawBeforeDef@1+1 Reserve@2+2 StartIndex@4+4 EndIndex@8+4 FrameTime@12+4 PlayCount@16+4 OffsetX@20+4 OffsetY@24+4 UseImageOffset@28+1 OutsideAreaDraw@29+1 Draw@30+1 BlendDraw@31+1 HorzAlignment@32+1 VertAlignment@33+1 AdjustYByHeight@34+1 Reseved@35+17",
            "TGuiMainBottomForm_New;340;4;LeftImageType@0+1 LeftImageIndex@4+4 RightImageType@8+1 RightImageIndex@12+4 Center@16+100 Animation1@116+52 Animation2@168+52 Animation3@220+52 Animation4@272+52 Reserve@324+16",
            "TGuiMainBottomForm_New2;348;4;LeftImageType@0+1 LeftImageIndex@4+4 RightImageType@8+1 RightImageIndex@12+4 BottomImageType@16+1 BottomImageIndex@20+4 Center@24+100 Animation1@124+52 Animation2@176+52 Animation3@228+52 Animation4@280+52 Reserve@332+16",
            "TGuiMemo;168;4;ImageIndex@0+20 ScrollImageIndex@20+20 PrevImageIndex@40+20 NextImageIndex@60+20 BarImageIndex@80+20 ShowScroll@100+1 ItemHeight@104+4 ItemIndex@108+4 ScrollBars@112+1 ScrollSize@116+4 ExpandSize@120+4 Position@124+4 VisibleItemCount@128+4 ShowButton@132+1 OffSetX@136+4 OffSetY@140+4 ColCount@144+4 ShowItemCount@148+4 ShowGridLine@152+1 GridLineColor@156+4 CheckItemControlSize@160+1 Reserve@164+4",
            "TGuiMemo_New;224;4;ImageIndex@0+20 ScrollImageIndex@20+20 PrevImageIndex@40+20 NextImageIndex@60+20 BarImageIndex@80+20 ShowScroll@100+1 ItemHeight@104+4 ItemIndex@108+4 ScrollBars@112+1 ScrollSize@116+4 ExpandSize@120+4 Position@124+4 VisibleItemCount@128+4 ShowButton@132+1 OffSetX@136+4 OffSetY@140+4 ColCount@144+4 ShowItemCount@148+4 ShowGridLine@152+1 GridLineColor@156+4 CheckItemControlSize@160+1 BackGroupColor@164+4 FontBackTransparent@168+1 FontLen@172+4 FontSize@176+4 FontStroke@180+1 Reserve@184+40",
            "TGuiPageControl;48;4;ShowButton@0+1 ClientLeft@4+4 ClientTop@8+4 ClientWidth@12+4 ClientHeight@16+4 TabPosition@20+1 PageCount@24+4 ActivePageIndex@28+4 ButtonWidth@32+4 ButtonHeight@36+4 OffSetX@40+4 OffSetY@44+4",
            "TGuiPageControl_New;144;4;ShowButton@0+1 ClientLeft@4+4 ClientTop@8+4 ClientWidth@12+4 ClientHeight@16+4 TabPosition@20+1 PageCount@24+4 ActivePageIndex@28+4 ButtonWidth@32+4 ButtonHeight@36+4 OffSetX@40+4 OffSetY@44+4 CaptionOffsetX@48+4 CaptionOffsetY@52+4 DownCaptionOffsetX@56+4 DownCaptionOffsetY@60+4 ReverseDrawButton@64+1 Reserve1@65+3 Reserve2@68+76",
            "TGuiPopupMenu;216;4;DrawBorder@0+1 SelectColor@4+4 BackgroundColor@8+4 ItemColor@12+96 BorderColor@108+96 ItemHeight@204+4 ItemIndex@208+4 ItemTextLen@212+4",
            "TGuiSexPanel;56;4;IsMale@0+1 UseSettign2@1+1 ImageType@2+1 Male@4+4 Female@8+4 ImageType2@12+1 Male2@16+4 Female2@20+4 Reserve@24+32",
            "TGuiSwitchButton;404;4;AutoSize@0+1 CloseSetting@4+184 OpenSetting@188+184 Reserve@372+32",
            "TGuiSwitchButtonSetting;184;4;ImageIndex@0+20 CaptionColor@20+96 ClickSound@116+1 Alignment@117+1 CaptionOffsetX@120+4 CaptionOffsetY@124+4 CaptionDownOffsetX@128+4 CaptionDownOffsetY@132+4 ButtonDownOffsetX@136+4 ButtonDownOffsetY@140+4 DrawAligment@144+1 CaptionLen@148+4 Reserve@152+32",
            "TGuiTabSheet;136;4;OffSetX@0+4 OffSetY@4+4 HideTable@8+1 Reserve1@9+3 Reserve2@12+4 CaptionColor@16+96 ImageIndex@112+20 CaptionLen@132+4",
            "TGuiTrackBar;56;4;ImageIndex@0+20 SliderIndex@20+20 AutoSize@40+1 Min@44+4 Max@48+4 Position@52+4",
            "TGuiViewField;104;4;Color@0+96 Alignment@96+1 CaptionLen@100+4",
            "TSaveUIColor;16;4;Up@0+4 Hot@4+4 Down@8+4 Disabled@12+4",
            "TSelection;8;4;StartPos@0+4 EndPos@4+4",
        };

    // =================================================================================
    // 关键偏移（挑最容易被"看起来一样"骗到的几处）
    // =================================================================================

    [Fact]
    public void TGuiHeader_FieldOffsets()
    {
        Assert.Equal(0, TGuiHeader.Layout.OffsetOf("Gui"));
        Assert.Equal(4, TGuiHeader.Layout.OffsetOf("Left"));
        Assert.Equal(20, TGuiHeader.Layout.OffsetOf("Enabled"));
        Assert.Equal(26, TGuiHeader.Layout.OffsetOf("MouseEvents"));
        Assert.Equal(28, TGuiHeader.Layout.OffsetOf("NameLen"));
        Assert.Equal(36, TGuiHeader.Layout.OffsetOf("Count"));
        Assert.Equal(40, TGuiHeader.SizeOf);
        Assert.Throws<ArgumentException>(() => TGuiHeader.Layout.OffsetOf("NoSuchField"));
    }

    [Fact]
    public void TGuiHeaderAdd_Reserverd_And_Reserve_Are_Real_Bytes()
    {
        // 原文如此（DxComponents.pas:174,176）：两个"笔误"字段名 Reserverd / Reserverd2 真实占 4 字节，
        // 不是可以省略的注释；它们在 TGuiHeader.SizeOf 之后紧邻。
        Assert.Equal(7, TGuiHeaderAdd.Layout.OffsetOf("Reserverd"));
        Assert.Equal(10, TGuiHeaderAdd.Layout.OffsetOf("Reserverd2"));
        Assert.Equal(12, TGuiHeaderAdd.Layout.OffsetOf("Reserve"));   // array[0..8] of Integer = 36
        Assert.Equal(48, TGuiHeaderAdd.SizeOf);
    }

    [Fact]
    public void TGuiFont_Style_Is_OneByte_Set_So_Size_Is_Padded()
    {
        Assert.Equal(8, TGuiFont.Layout.OffsetOf("Style"));
        Assert.Equal(12, TGuiFont.Layout.OffsetOf("Size"));    // 不是 9
        Assert.Equal(20, TGuiFont.Layout.OffsetOf("NameLen")); // 不是 17
        Assert.Equal(24, TGuiFont.SizeOf);
    }

    [Fact]
    public void TGuiImageFormShape_Holds_Eight_Shape_Items()
    {
        // array[0..8 - 1] of TGuiFormShapeInfo —— 原文写成 "8 - 1"，是 8 项而不是九宫格的 9 项。
        Assert.Equal(28, TGuiImageFormShape.Layout.OffsetOf("ImageIndexs"));
        Assert.Equal(416, TGuiImageFormShape.Layout.Fields[3].Size);
        Assert.Equal(28 + 8 * 52, TGuiImageFormShape.SizeOf);
    }

    [Fact]
    public void TGuiFileHeader_Is_Packed_So_No_Padding()
    {
        Assert.Equal(36, TGuiFileHeader.Layout.OffsetOf("ClientVersion"));
        Assert.Equal(38, TGuiFileHeader.Layout.OffsetOf("GroupCount"));   // packed → 紧贴 Reserve 之后
        Assert.Equal(40, TGuiFileHeader.Layout.OffsetOf("nGuiVersion"));
        Assert.Equal(48, TGuiFileHeader.Layout.OffsetOf("dCreateDate"));
        Assert.Equal(56, TGuiFileHeader.SizeOf);
        Assert.Equal(1, TGuiFileHeader.Layout.Align);
    }

    [Fact]
    public void TGuiAnimation_And_TGuiButtonAnimation_Differ_In_Shape()
    {
        // 差异断言：两者都带 DrawBeforeDef@1，但 TGuiButtonAnimation 多了 ShowType（枚举），
        // 于是 StartIndex 从 4 被挤到 8，总长 32 → 36。
        Assert.Equal(1, TGuiAnimation.Layout.OffsetOf("DrawBeforeDef"));
        Assert.Equal(4, TGuiAnimation.Layout.OffsetOf("StartIndex"));
        Assert.Equal(12, TGuiAnimation.Layout.OffsetOf("FrameTime"));
        Assert.Equal(32, TGuiAnimation.SizeOf);

        Assert.Equal(1, TGuiButtonAnimation.Layout.OffsetOf("DrawBeforeDef"));
        Assert.Equal(4, TGuiButtonAnimation.Layout.OffsetOf("ShowType"));
        Assert.Equal(8, TGuiButtonAnimation.Layout.OffsetOf("StartIndex"));
        Assert.Equal(16, TGuiButtonAnimation.Layout.OffsetOf("FrameTime"));
        Assert.Equal(36, TGuiButtonAnimation.SizeOf);
    }

    [Fact]
    public void TGuiMainBottomFormAnimation_Has_Alignment_Tail_That_TGuiAnimation_Lacks()
    {
        // 差异断言：只有 MainBottomForm 的动画带 HorzAlignment/VertAlignment/AdjustYByHeight
        // 与 17 字节 Reseved 尾巴（32+3+17 = 52）。
        Assert.Equal(32, TGuiMainBottomFormAnimation.Layout.OffsetOf("HorzAlignment"));
        Assert.Equal(33, TGuiMainBottomFormAnimation.Layout.OffsetOf("VertAlignment"));
        Assert.Equal(34, TGuiMainBottomFormAnimation.Layout.OffsetOf("AdjustYByHeight"));
        Assert.Equal(35, TGuiMainBottomFormAnimation.Layout.OffsetOf("Reseved"));
        Assert.Equal(52, TGuiMainBottomFormAnimation.SizeOf);
        Assert.Throws<ArgumentException>(() => TGuiAnimation.Layout.OffsetOf("HorzAlignment"));
    }

    [Fact]
    public void TGuiLabel_And_TGuiLabel_New_Are_Same_Size_But_Alignment_Shifts()
    {
        // 差异断言：TGuiLabel_New 多一个 Alignment（1 字节），恰好吃掉 DrawBorder 后面的填充，
        // 总长仍是 216 —— 只比对 SizeOf 的测试会漏掉它，所以必须断言具体偏移。
        Assert.Equal(216, TGuiLabel.SizeOf);
        Assert.Equal(216, TGuiLabel_New.SizeOf);
        Assert.Equal(1, TGuiLabel.Layout.OffsetOf("DrawBorder"));
        Assert.Equal(1, TGuiLabel_New.Layout.OffsetOf("Alignment"));
        Assert.Equal(2, TGuiLabel_New.Layout.OffsetOf("DrawBorder"));
        Assert.Equal(104, TGuiLabel.Layout.OffsetOf("CaptionColor"));
        Assert.Equal(104, TGuiLabel_New.Layout.OffsetOf("CaptionColor"));
        Assert.Throws<ArgumentException>(() => TGuiLabel.Layout.OffsetOf("Alignment"));
    }

    [Fact]
    public void TGuiEdit_And_TGuiImageEdit_Differ_From_Offset_20_Onwards()
    {
        Assert.Equal(TGuiEdit.Layout.OffsetOf("FontColor"), TGuiImageEdit.Layout.OffsetOf("FontColor") - 32);
        Assert.Equal(160, TGuiEdit.SizeOf);
        Assert.Equal(220, TGuiImageEdit.SizeOf);
        Assert.Equal(192, TGuiImageEdit.Layout.OffsetOf("HintTextLen"));
    }

    [Fact]
    public void TGuiImageForm_New3_Shrinks_ImageIndex_And_Drops_Hot_Down_Disabled()
    {
        // 差异断言：New3 用 TGuiImageIndex_Form（无 Hot/Down/Disabled）→ 20 字节变 16 字节，
        // 于是 Animation1 从 32 前移到 28。
        Assert.Equal(32, TGuiImageForm_New2.Layout.OffsetOf("Animation1"));
        Assert.Equal(28, TGuiImageForm_New3.Layout.OffsetOf("Animation1"));
        Assert.Equal(4, TGuiImageForm_New3.Layout.OffsetOf("ImageIndex"));
        Assert.Equal(20, TGuiImageForm_New2.Layout.Fields[1].Size);
        Assert.Equal(16, TGuiImageForm_New3.Layout.Fields[1].Size);
        Assert.Equal(208, TGuiImageForm_New2.SizeOf);
        Assert.Equal(204, TGuiImageForm_New3.SizeOf);
    }

    // =================================================================================
    // 往返：WriteAt → ReadAt 必须逐字段一致（合成字节的自洽性锁）
    // =================================================================================

    [Fact]
    public void RoundTrip_TGuiImageButton_New3()
    {
        var rec = new TGuiImageButton_New3
        {
            Alignment = TDxAlignment.taCenter,
            AutoSize = true,
            Checked = true,
            CaptionDownOffsetX = -3,
            CaptionDownOffsetY = 4,
            CaptionOffsetX = 5,
            CaptionOffsetY = 6,
            CaptionLen = 7,
        };
        rec.ImageIndex.Checked = 99;
        rec.CaptionColor.Up.Color = 0x00112233;
        rec.CaptionColor.Up.Style = 0b0101;         // fsBold | fsUnderline
        rec.Animation.ShowType = TButtonAnimationShowType.astHotShow;
        rec.Animation.OffsetY = -12;

        var bytes = rec.ToBytes();
        Assert.Equal(TGuiImageButton_New3.SizeOf, bytes.Length);

        var back = TGuiImageButton_New3.FromBytes(bytes);
        Assert.Equal(TDxAlignment.taCenter, back.Alignment);
        Assert.True(back.AutoSize);
        Assert.True(back.Checked);
        Assert.Equal(-3, back.CaptionDownOffsetX);
        Assert.Equal(4, back.CaptionDownOffsetY);
        Assert.Equal(5, back.CaptionOffsetX);
        Assert.Equal(6, back.CaptionOffsetY);
        Assert.Equal(7, back.CaptionLen);
        Assert.Equal(99, back.ImageIndex.Checked);
        Assert.Equal(0x00112233, back.CaptionColor.Up.Color);
        Assert.Equal(0b0101, back.CaptionColor.Up.Style);
        Assert.Equal(TButtonAnimationShowType.astHotShow, back.Animation.ShowType);
        Assert.Equal(-12, back.Animation.OffsetY);
    }

    [Fact]
    public void RoundTrip_TGuiImageProgress_Gbk_ShortStrings()
    {
        var rec = new TGuiImageProgress
        {
            ValueType = TProgressValueType.vtPercentage,
            ValueSplite = "/",
            ValuePrefix = "HP ",
            ValueSuffix = " %",
            Max = 100,
            Min = 1,
            Value = 42,
            ValueAlignment = TDxAlignment.taRightJustify,
        };
        rec.Font.Bold = true;
        rec.Font.NameLen = 6;

        var back = TGuiImageProgress.FromBytes(rec.ToBytes());
        Assert.Equal("/", back.ValueSplite);
        Assert.Equal("HP ", back.ValuePrefix);
        Assert.Equal(" %", back.ValueSuffix);
        Assert.Equal(100, back.Max);
        Assert.Equal(1, back.Min);
        Assert.Equal(42, back.Value);
        Assert.True(back.Font.Bold);
        Assert.Equal(6, back.Font.NameLen);
    }

    [Fact]
    public void RoundTrip_TGuiImageProgress_ShortString_Overflow_Is_Truncated()
    {
        // Delphi string[20] 的赋值语义：超出部分被截断，且长度字节 = 截断后的字节数。
        var rec = new TGuiImageProgress { ValueSplite = new string('A', 50) };
        var bytes = rec.ToBytes();
        Assert.Equal(20, bytes[21]);                                    // 长度字节
        Assert.Equal(new string('A', 20), TGuiImageProgress.FromBytes(bytes).ValueSplite);
    }

    [Fact]
    public void RoundTrip_TGuiImageProgress_ShortString_MultiByte_Gbk_Counts_Bytes()
    {
        // 中文在 GBK 里是 2 字节：string[20] 只能装 10 个汉字。
        var rec = new TGuiImageProgress { ValueSplite = new string('中', 12) };
        var bytes = rec.ToBytes();
        Assert.Equal(20, bytes[21]);
        Assert.Equal(new string('中', 10), TGuiImageProgress.FromBytes(bytes).ValueSplite);
    }

    [Fact]
    public void RoundTrip_TGuiMemo_New_Reserve_Byte_Array()
    {
        var rec = new TGuiMemo_New { ShowScroll = true, ItemHeight = 16, ItemIndex = 3, FontLen = 8, FontSize = 12 };
        rec.Reserve[0] = 0xAA;
        rec.Reserve[9] = 0x55;

        var bytes = rec.ToBytes();
        var back = TGuiMemo_New.FromBytes(bytes);
        Assert.True(back.ShowScroll);
        Assert.Equal(16, back.ItemHeight);
        Assert.Equal(3, back.ItemIndex);
        Assert.Equal(8, back.FontLen);
        Assert.Equal(12, back.FontSize);
        Assert.Equal(0xAA, back.Reserve[0]);
        Assert.Equal(0x55, back.Reserve[9]);
        Assert.Equal(184, TGuiMemo_New.Layout.OffsetOf("Reserve"));
        Assert.Equal(0xAA, bytes[184]);
    }

    [Fact]
    public void RoundTrip_TGuiGroupAttackProgress_Array_Of_Records()
    {
        var rec = new TGuiGroupAttackProgress { ProgressAlignment = TMagicBallValueAlignment.mbaBottom };
        rec.Settings[0].ImageType = TImageType.UI1_wil;
        rec.Settings[1].Progress = 77;
        rec.Settings[2].OffsetY3 = -9;

        var back = TGuiGroupAttackProgress.FromBytes(rec.ToBytes());
        Assert.Equal(TMagicBallValueAlignment.mbaBottom, back.ProgressAlignment);
        Assert.Equal(TImageType.UI1_wil, back.Settings[0].ImageType);
        Assert.Equal(77, back.Settings[1].Progress);
        Assert.Equal(-9, back.Settings[2].OffsetY3);
    }

    [Fact]
    public void RoundTrip_TGuiImageFormShape_Rect_Array()
    {
        var rec = new TGuiImageFormShape();
        rec.ImageIndexs[3].SrcRect = TDxRect.Rect(1, 2, 3, 4);
        rec.ImageIndexs[3].DestRect = TDxRect.Rect(5, 6, 7, 8);
        rec.ImageIndexs[7].Align = TAlignEx.alxBottomRight;

        var back = TGuiImageFormShape.FromBytes(rec.ToBytes());
        Assert.Equal(TDxRect.Rect(1, 2, 3, 4), back.ImageIndexs[3].SrcRect);
        Assert.Equal(TDxRect.Rect(5, 6, 7, 8), back.ImageIndexs[3].DestRect);
        Assert.Equal(TAlignEx.alxBottomRight, back.ImageIndexs[7].Align);
    }

    [Fact]
    public void RoundTrip_TGuiFileHeader_Desc_Truncates_At_35_Bytes()
    {
        var rec = new TGuiFileHeader { sDesc = new string('X', 40), nGuiVersion = 20171106, nCount = 12, GroupCount = 2 };
        var bytes = rec.ToBytes();
        Assert.Equal(56, bytes.Length);
        Assert.Equal(35, bytes[0]);
        var back = TGuiFileHeader.FromBytes(bytes);
        Assert.Equal(new string('X', 35), back.sDesc);
        Assert.Equal(20171106, back.nGuiVersion);
        Assert.Equal(12, back.nCount);
        Assert.Equal(2, back.GroupCount);
    }

    [Fact]
    public void ReadAt_With_NonZero_Base_Offset()
    {
        // LoadControlFromMemory 的 "at" 参数必须真正生效（不是永远从 0 读）。
        var rec = new TGuiLabel { BackgroundColor = 0x123456, ClickCount = (GXX.Client.DxComponent.TDxImageButton.TClickSound)2 };
        var bytes = new byte[TGuiLabel.SizeOf + 8];
        rec.WriteAt(bytes, 8);
        var back = TGuiLabel.ReadAt(bytes, 8);
        Assert.Equal(0x123456, back.BackgroundColor);
        Assert.Equal(2, (int)back.ClickCount);
    }
}
