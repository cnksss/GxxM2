using System;
using System.Collections.Generic;
using GXX.Client.DxComponent;

namespace GXX.Client.LoadDx;

// =====================================================================================
// LoadDxControl.pas:85-1558 与 LoadDxControlEx.pas:83-1550 的 1:1 移植。
//
// 【为什么两个单元共用一份】
//   两份 LoadComponent 主体是**同一段代码**，唯一差别是读字节的原语
//   （内存版 ReadMemory(var Buffer; Count) / 流版 ReadMemory(streamUI, var Buffer; Count)），
//   而这一差别已被 IGuiReader 抽象吸收。用脚本做过的规范化逐行比对结论（见报告 §差异）：
//   把两边的 "streamUI, " 参数抹掉后，两份 1437 行主体的差异**只有空白与
//   `end else` 换行方式**（A/B diff 输出 8 处纯格式差异 + 1 处多余空行），
//   没有任何语义差异。因此这里只保留一份实现，分支顺序与原文完全一致。
//
// 行号对照（Delphi 原文 → 本文件段落）：
//   LoadDxControl.pas  116-1558   ≡  LoadDxControlEx.pas 114-1550   ≡  GuiComponentLoader.LoadComponent
//   LoadDxControl.pas  104-114    ≡  LoadDxControlEx.pas 102-112    ≡  GuiReaderExtensions.ReadGuiFontName
//   LoadDxControl.pas   85-92     ≡  LoadDxControlEx.pas  83-90     ≡  DxGuiFonts.DxFontAssign
//   LoadDxControl.pas   94-102    ≡  LoadDxControlEx.pas  92-100    ≡  DxGuiFonts.GuiFontAssign
//   LoadDxControl.pas 1560-1619   ≡  LoadDxControlEx.pas 1551-1610 (差异见 DxControlFactory)
// =====================================================================================

/// <summary>
/// LoadDxControl.pas:85-102 / LoadDxControlEx.pas:83-100 的 DxFontAssign / GuiFontAssign。
/// </summary>
public static class DxGuiFonts
{
    /// <summary>
    /// Delphi <c>TFontStyles</c>（set of TFontStyle）位序：fsBold=1, fsItalic=2, fsUnderline=4, fsStrikeOut=8。
    /// 既有 <see cref="TDxFont.Style"/> 是字符串集合（车道5 的接缝形态），故这里做一次位 → 名映射。
    /// </summary>
    private static readonly string[] StyleNames = { "Bold", "Italic", "Underline", "StrikeOut" };

    /// <summary>原文 LoadDxControl.pas:85-92：DxFont 五字段（Name 由 ReadGuiFontName 单独消费）。</summary>
    public static void DxFontAssign(TDxFont dxFont, TGuiFont guiFont)
    {
        dxFont.Color = guiFont.Color;
        dxFont.BColor = guiFont.BColor;
        dxFont.Size = guiFont.Size;
        dxFont.Bold = guiFont.Bold;
        dxFont.StyleSet = StyleToNames(guiFont.Style);
    }

    /// <summary>
    /// 原文 LoadDxControl.pas:94-102：反向赋值 + <c>GuiFont.NameLen := Length(DxFont.Name)</c>。
    /// 原文的 Length(AnsiString) 是**字节**长度，故此处按 GBK 字节计数（非 UTF-16 字符数）。
    /// </summary>
    public static void GuiFontAssign(TGuiFont guiFont, TDxFont dxFont)
    {
        guiFont.Color = dxFont.Color;
        guiFont.BColor = dxFont.BColor;
        guiFont.Size = dxFont.Size;
        guiFont.Bold = dxFont.Bold;
        guiFont.Style = NamesToStyle(dxFont.Style);
        guiFont.NameLen = dxFont.Name == null ? 0 : GXX.Core.EncodingInit.GBK.GetByteCount(dxFont.Name);
    }

    public static List<string> StyleToNames(byte style)
    {
        var list = new List<string>();
        for (int i = 0; i < StyleNames.Length; i++)
        {
            if ((style & (1 << i)) != 0) list.Add(StyleNames[i]);
        }
        return list;
    }

    public static byte NamesToStyle(IReadOnlyCollection<string> names)
    {
        byte style = 0;
        if (names == null) return 0;
        for (int i = 0; i < StyleNames.Length; i++)
        {
            foreach (var n in names)
            {
                if (string.Equals(n, StyleNames[i], StringComparison.OrdinalIgnoreCase))
                {
                    style |= (byte)(1 << i);
                    break;
                }
            }
        }
        return style;
    }
}

/// <summary>
/// LoadDxControl.pas:1560-1619 / LoadDxControlEx.pas:1551-1610 的 NewDxControl。
///
/// <para>两个版本的差异（用 diff 核过，是本车道最重要的"看似一样实则不同"点之一）：</para>
/// <list type="bullet">
/// <item>内存版 LoadDxControl.pas:1593-1596 在建好控件后**立刻**把它写进
///   <c>PControlAddress^</c> 并前移指针；流版 LoadDxControlEx.pas:1583-1584 只设 GuiType，
///   登记动作留给 LoadSubComponent / LoadControlFromStream（因为要按 Name 查表）。</item>
/// </list>
/// </summary>
public static class DxControlFactory
{
    /// <summary>原文两个 NewDxControl 的公共部分（case 顺序、AOwner 归属、12 项基础属性赋值顺序）。</summary>
    public static TDxControl NewDxControl(TGuiHeader guiHeader, TDxControl aOwner)
    {
        TDxControl dxControl = null;
        switch (guiHeader.Gui)
        {
            // 原文如此（LoadDxControl.pas:1566-1590）：case 无 else，t_None / 越界枚举值 → 返回 nil 且不消费任何字节。
            case TGuiType.t_Form: dxControl = new TDxImageForm(); break;
            case TGuiType.t_FormShape: dxControl = new TDxImageFormShape(); break;
            case TGuiType.t_Button: dxControl = new TDxImageButton(); break;
            case TGuiType.t_Edit: dxControl = new TDxEdit(); break;
            case TGuiType.t_ImageEdit: dxControl = new TDxImageEdit(); break;
            case TGuiType.t_Label: dxControl = new TDxLabel(); break;
            case TGuiType.t_Grid: dxControl = new TDxImageGrid(); break;
            case TGuiType.t_ScrollBox: dxControl = new TDxScrollBox(); break;
            case TGuiType.t_ChatMemo: dxControl = new TDxChatMemo(); break;
            case TGuiType.t_ListView: dxControl = new TDxListView(); break;
            case TGuiType.t_TreeView: dxControl = new TDxTreeView(); break;
            case TGuiType.t_PopupMenu: dxControl = new TDxPopupMenu(); break;
            case TGuiType.t_TabSheet: dxControl = new TDxTabSheet(); break;
            case TGuiType.t_PageControl: dxControl = new TDxPageControl(); break;
            case TGuiType.t_ComboBox: dxControl = new TDxComboBox(); break;
            case TGuiType.t_Line: dxControl = new TDxLine(); break;
            case TGuiType.t_TrackBar: dxControl = new TDXTrackBar(); break;
            case TGuiType.t_MainBottomForm: dxControl = new TDxMainBottomForm(); break;
            case TGuiType.t_MagicBall: dxControl = new TDxMagicBall(); break;
            case TGuiType.t_SexPanel: dxControl = new TDxSexPanel(); break;
            case TGuiType.t_GroupAttackProgress: dxControl = new TDxGroupAttackProgress(); break;
            case TGuiType.t_ImageProgress: dxControl = new TDxImageProgress(); break;
            case TGuiType.t_SwitchButton: dxControl = new TDxSwitchButton(); break;
        }

        if (dxControl != null)
        {
            dxControl.DxOwner = aOwner;                       // 原文 Create(AOwner)
            TDxControlExtras.SetGuiType(dxControl, guiHeader.Gui);
            dxControl.Left = guiHeader.Left;
            dxControl.Top = guiHeader.Top;
            dxControl.Width = guiHeader.Width;
            dxControl.Height = guiHeader.Height;
            dxControl.Enabled = guiHeader.Enabled;
            dxControl.Visible = guiHeader.Visible;
            dxControl.Transparent = guiHeader.Transparent;
            dxControl.EnableFocus = guiHeader.EnableFocus;
            dxControl.Floating = guiHeader.Floating;
            dxControl.OwnerMove = guiHeader.OwnerMove;
            dxControl.Designing = false;
            dxControl.SetOnGetImage(aOwner?.ImageIndex?.OnGetImage);   // 原文 DxControl.OnGetImage := AOwner.OnGetImage
            TDxControlExtras.Of(dxControl).OnGetImage = aOwner == null ? null : TDxControlExtras.Of(aOwner).OnGetImage;
            dxControl.MouseEvents = (TMouseEvents)guiHeader.MouseEvents;
        }

        return dxControl;
    }
}

/// <summary>
/// LoadDxControl.pas:116-1558 / LoadDxControlEx.pas:114-1550 的 LoadComponent 1:1。
/// 分支顺序、版本分界、字段赋值顺序全部照抄。
/// </summary>
public static class GuiComponentLoader
{
    /// <summary>版本分界常量（原文里是字面量日期，这里保持字面量以便逐行对照）。</summary>
    public const int Ver20160409 = 20160409;
    public const int Ver20160430 = 20160430;
    public const int Ver20160508 = 20160508;
    public const int Ver20160514 = 20160514;
    public const int Ver20160818 = 20160818;
    public const int Ver20170226 = 20170226;
    public const int Ver20171106 = 20171106;
    public const int Ver20180619 = 20180619;
    public const int Ver20190724 = 20190724;
    public const int Ver20190729 = 20190729;
    public const int Ver20211120 = 20211120;

    /// <summary>
    /// 原文 <c>LoadComponent(streamUI, Gui, DxControl, GuiVersion)</c>。
    /// <paramref name="gui"/> 即 <c>GuiHeader.Gui</c>。
    /// </summary>
    public static void LoadComponent(IGuiReader reader, TGuiType gui, TDxControl dxControl, int guiVersion)
    {
        string sText;
        int i;

        switch (gui)
        {
            case TGuiType.t_Form:
                {
                    var dxImageForm = (TDxImageForm)dxControl;
                    if (guiVersion < Ver20160409)
                    {
                        // 原文如此（LoadDxControl.pas:184 / LoadDxControlEx.pas:182）：两个单元此分支的界都是 < 20160409。
                        reader.ReadRecord(TGuiImageForm.SizeOf, TGuiImageForm.ReadAt, out var g);
                        dxImageForm.AutoSize = g.AutoSize;
                        dxImageForm.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageForm.ImageIndex.Up = g.ImageIndex.Up;
                        dxImageForm.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxImageForm.ImageIndex.Down = g.ImageIndex.Down;
                        dxImageForm.ImageIndex.Disabled = g.ImageIndex.Disabled;
                        dxImageForm.Center = g.Center;
                    }
                    else if (guiVersion < Ver20160514)
                    {
                        reader.ReadRecord(TGuiImageForm_New.SizeOf, TGuiImageForm_New.ReadAt, out var g);
                        dxImageForm.AutoSize = g.AutoSize;
                        dxImageForm.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageForm.ImageIndex.Up = g.ImageIndex.Up;
                        dxImageForm.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxImageForm.ImageIndex.Down = g.ImageIndex.Down;
                        dxImageForm.ImageIndex.Disabled = g.ImageIndex.Disabled;
                        dxImageForm.Center = g.Center;
                        dxImageForm.BackgroundColor = g.BackgroundColor;
                        dxImageForm.BackgroundAlpha = g.BackgroundAlpha;
                    }
                    else if (guiVersion < Ver20171106)
                    {
                        reader.ReadRecord(TGuiImageForm_New2.SizeOf, TGuiImageForm_New2.ReadAt, out var g);
                        dxImageForm.AutoSize = g.AutoSize;
                        dxImageForm.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageForm.ImageIndex.Up = g.ImageIndex.Up;
                        dxImageForm.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxImageForm.ImageIndex.Down = g.ImageIndex.Down;
                        dxImageForm.ImageIndex.Disabled = g.ImageIndex.Disabled;
                        dxImageForm.Center = g.Center;
                        dxImageForm.BackgroundColor = g.BackgroundColor;
                        dxImageForm.BackgroundAlpha = g.BackgroundAlpha;

                        AssignAnimation(dxImageForm.Animation1, g.Animation1);
                        AssignAnimation(dxImageForm.Animation2, g.Animation2);
                        AssignAnimation(dxImageForm.Animation3, g.Animation3);
                    }
                    else
                    {
                        reader.ReadRecord(TGuiImageForm_New3.SizeOf, TGuiImageForm_New3.ReadAt, out var g);
                        dxImageForm.AutoSize = g.AutoSize;
                        dxImageForm.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageForm.ImageIndex.Up = g.ImageIndex.Up;
                        // 原文如此（LoadDxControl.pas:263-264）：New3 的 ImageIndex 是 TGuiImageIndex_Form，
                        // 只有 Up/OffsetX/OffsetY，没有 Hot/Down/Disabled。
                        dxImageForm.ImageIndex.OffsetX = g.ImageIndex.OffsetX;
                        dxImageForm.ImageIndex.OffsetY = g.ImageIndex.OffsetY;
                        dxImageForm.Center = g.Center;
                        dxImageForm.BackgroundColor = g.BackgroundColor;
                        dxImageForm.BackgroundAlpha = g.BackgroundAlpha;

                        AssignAnimation(dxImageForm.Animation1, g.Animation1);
                        AssignAnimation(dxImageForm.Animation2, g.Animation2);
                        AssignAnimation(dxImageForm.Animation3, g.Animation3);
                    }
                }
                break;

            case TGuiType.t_FormShape:
                {
                    reader.ReadRecord(TGuiImageFormShape.SizeOf, TGuiImageFormShape.ReadAt, out var g);
                    var dxFormShape = (TDxImageFormShape)dxControl;
                    dxFormShape.AutoSize = g.AutoSize;
                    dxFormShape.ImageIndex.ImageType = g.ImageIndex.Image;
                    dxFormShape.ImageIndex.Up = g.ImageIndex.Up;
                    dxFormShape.ImageIndex.Hot = g.ImageIndex.Hot;
                    dxFormShape.ImageIndex.Down = g.ImageIndex.Down;
                    dxFormShape.ImageIndex.Disabled = g.ImageIndex.Disabled;
                    dxFormShape.Center = g.Center;
                    for (i = 0; i <= dxFormShape.ImageCount - 1; i++)
                    {
                        dxFormShape.Items[i].ImageType = g.ImageIndexs[i].ImageType;
                        dxFormShape.Items[i].ImageIndex = g.ImageIndexs[i].ImageIndex;
                        dxFormShape.Items[i].Align = g.ImageIndexs[i].Align;
                        dxFormShape.Items[i].Draw = g.ImageIndexs[i].Draw;
                        dxFormShape.Items[i].Stretch = g.ImageIndexs[i].Stretch;
                        dxFormShape.Items[i].Center = g.ImageIndexs[i].Center;
                        dxFormShape.Items[i].BlendMode = g.ImageIndexs[i].BlendMode;
                        dxFormShape.Items[i].SourceRect = g.ImageIndexs[i].SrcRect;
                        dxFormShape.Items[i].DestRect = g.ImageIndexs[i].DestRect;
                    }
                }
                break;

            case TGuiType.t_Button:
                {
                    var dxImageButton = (TDxImageButton)dxControl;
                    if (guiVersion < Ver20171106)
                    {
                        reader.ReadRecord(TGuiImageButton.SizeOf, TGuiImageButton.ReadAt, out var g);
                        dxImageButton.AutoSize = g.AutoSize;
                        dxImageButton.Alignment = g.Alignment;
                        dxImageButton.CaptionDownOffsetX = g.CaptionDownOffsetX;
                        dxImageButton.CaptionDownOffsetY = g.CaptionDownOffsetY;
                        dxImageButton.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageButton.ImageIndex.Up = g.ImageIndex.Up;
                        dxImageButton.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxImageButton.ImageIndex.Down = g.ImageIndex.Down;
                        dxImageButton.ImageIndex.Disabled = g.ImageIndex.Disabled;

                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Up, g.CaptionColor.Up);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Hot, g.CaptionColor.Hot);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Down, g.CaptionColor.Down);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Disabled, g.CaptionColor.Disabled);

                        dxImageButton.Checked = g.Checked;
                        dxImageButton.ClickSound = g.ClickCount;
                        dxImageButton.Style = g.Style;
                        dxImageButton.Caption = string.Empty;
                        dxImageButton.CaptionColor.Up.Name = reader.ReadGuiFontName(g.CaptionColor.Up);
                        dxImageButton.CaptionColor.Hot.Name = reader.ReadGuiFontName(g.CaptionColor.Hot);
                        dxImageButton.CaptionColor.Down.Name = reader.ReadGuiFontName(g.CaptionColor.Down);
                        dxImageButton.CaptionColor.Disabled.Name = reader.ReadGuiFontName(g.CaptionColor.Disabled);

                        if (g.CaptionLen > 0)
                        {
                            sText = reader.ReadFixedString(g.CaptionLen);
                            dxImageButton.Caption = sText;
                        }
                    }
                    else if (guiVersion < Ver20180619)
                    {
                        reader.ReadRecord(TGuiImageButton_New2.SizeOf, TGuiImageButton_New2.ReadAt, out var g);
                        dxImageButton.AutoSize = g.AutoSize;
                        dxImageButton.Alignment = g.Alignment;
                        dxImageButton.CaptionDownOffsetX = g.CaptionDownOffsetX;
                        dxImageButton.CaptionDownOffsetY = g.CaptionDownOffsetY;
                        dxImageButton.CaptionOffsetX = g.CaptionOffsetX;
                        dxImageButton.CaptionOffsetY = g.CaptionOffsetY;
                        dxImageButton.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageButton.ImageIndex.Up = g.ImageIndex.Up;
                        dxImageButton.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxImageButton.ImageIndex.Down = g.ImageIndex.Down;
                        dxImageButton.ImageIndex.Disabled = g.ImageIndex.Disabled;
                        dxImageButton.ImageIndex.Checked = g.ImageIndex.Checked;

                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Up, g.CaptionColor.Up);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Hot, g.CaptionColor.Hot);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Down, g.CaptionColor.Down);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Disabled, g.CaptionColor.Disabled);

                        dxImageButton.Checked = g.Checked;
                        dxImageButton.ClickSound = g.ClickCount;
                        dxImageButton.Style = g.Style;
                        dxImageButton.Caption = string.Empty;
                        dxImageButton.CaptionColor.Up.Name = reader.ReadGuiFontName(g.CaptionColor.Up);
                        dxImageButton.CaptionColor.Hot.Name = reader.ReadGuiFontName(g.CaptionColor.Hot);
                        dxImageButton.CaptionColor.Down.Name = reader.ReadGuiFontName(g.CaptionColor.Down);
                        dxImageButton.CaptionColor.Disabled.Name = reader.ReadGuiFontName(g.CaptionColor.Disabled);

                        if (g.CaptionLen > 0)
                        {
                            sText = reader.ReadFixedString(g.CaptionLen);
                            dxImageButton.Caption = sText;
                        }
                    }
                    else
                    {
                        reader.ReadRecord(TGuiImageButton_New3.SizeOf, TGuiImageButton_New3.ReadAt, out var g);
                        dxImageButton.AutoSize = g.AutoSize;
                        dxImageButton.Alignment = g.Alignment;
                        dxImageButton.CaptionDownOffsetX = g.CaptionDownOffsetX;
                        dxImageButton.CaptionDownOffsetY = g.CaptionDownOffsetY;
                        dxImageButton.CaptionOffsetX = g.CaptionOffsetX;
                        dxImageButton.CaptionOffsetY = g.CaptionOffsetY;
                        dxImageButton.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxImageButton.ImageIndex.Up = g.ImageIndex.Up;
                        dxImageButton.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxImageButton.ImageIndex.Down = g.ImageIndex.Down;
                        dxImageButton.ImageIndex.Disabled = g.ImageIndex.Disabled;
                        dxImageButton.ImageIndex.Checked = g.ImageIndex.Checked;

                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Up, g.CaptionColor.Up);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Hot, g.CaptionColor.Hot);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Down, g.CaptionColor.Down);
                        DxGuiFonts.DxFontAssign(dxImageButton.CaptionColor.Disabled, g.CaptionColor.Disabled);

                        dxImageButton.Checked = g.Checked;
                        dxImageButton.ClickSound = g.ClickCount;
                        dxImageButton.Style = g.Style;
                        dxImageButton.Caption = string.Empty;

                        // 接缝：待 DxImageButton.pas 移植后接入（原文 TDxImageButton.Animation := ...）
                        TDxControlExtras.Of(dxImageButton).Animation = g.Animation;

                        dxImageButton.CaptionColor.Up.Name = reader.ReadGuiFontName(g.CaptionColor.Up);
                        dxImageButton.CaptionColor.Hot.Name = reader.ReadGuiFontName(g.CaptionColor.Hot);
                        dxImageButton.CaptionColor.Down.Name = reader.ReadGuiFontName(g.CaptionColor.Down);
                        dxImageButton.CaptionColor.Disabled.Name = reader.ReadGuiFontName(g.CaptionColor.Disabled);

                        if (g.CaptionLen > 0)
                        {
                            sText = reader.ReadFixedString(g.CaptionLen);
                            dxImageButton.Caption = sText;
                        }
                    }
                }
                break;

            case TGuiType.t_Edit:
                {
                    reader.ReadRecord(TGuiEdit.SizeOf, TGuiEdit.ReadAt, out var g);
                    var dxEdit = (TDxEdit)dxControl;
                    dxEdit.Text = string.Empty;
                    dxEdit.BackgroundColor = g.BackgroundColor;
                    dxEdit.DrawBorder = g.DrawBorder;

                    DxGuiFonts.DxFontAssign(dxEdit.Font, g.FontColor);
                    DxGuiFonts.DxFontAssign(dxEdit.BorderColor.Up, g.BorderColor.Up);
                    DxGuiFonts.DxFontAssign(dxEdit.BorderColor.Hot, g.BorderColor.Hot);
                    DxGuiFonts.DxFontAssign(dxEdit.BorderColor.Down, g.BorderColor.Down);
                    DxGuiFonts.DxFontAssign(dxEdit.BorderColor.Disabled, g.BorderColor.Disabled);

                    dxEdit.ReadOnly = g.ReadOnly;
                    dxEdit.MaxLength = g.MaxLength;
                    dxEdit.SelectedColor = g.SelectedColor;
                    dxEdit.SelBackColor = g.SelBackColor;
                    dxEdit.SelFontColor = g.SelFontColor;
                    dxEdit.InValue = g.InValue;
                    dxEdit.PasswordChar = g.PasswordChar;
                    dxEdit.AllowSelect = g.AllowSelect;
                    dxEdit.AllowPaste = g.AllowPaste;
                    dxEdit.TabOrder = g.TabOrder;
                    dxEdit.Font.Name = reader.ReadGuiFontName(g.FontColor);

                    if (g.TextLen > 0)
                    {
                        sText = reader.ReadFixedString(g.TextLen);
                        dxEdit.Text = sText;
                    }
                }
                break;

            case TGuiType.t_ImageEdit:
                {
                    var dxImageEdit = (TDxImageEdit)dxControl;
                    if (guiVersion < Ver20160430)
                    {
                        // 原文如此（LoadDxControl.pas:497）：此分支复用的是 TGuiEdit 布局，不是 TGuiImageEdit。
                        reader.ReadRecord(TGuiEdit.SizeOf, TGuiEdit.ReadAt, out var g);
                        dxImageEdit.Text = string.Empty;
                        dxImageEdit.BackgroundColor = g.BackgroundColor;
                        dxImageEdit.DrawBorder = g.DrawBorder;

                        DxGuiFonts.DxFontAssign(dxImageEdit.Font, g.FontColor);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Up, g.BorderColor.Up);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Hot, g.BorderColor.Hot);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Down, g.BorderColor.Down);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Disabled, g.BorderColor.Disabled);

                        dxImageEdit.ReadOnly = g.ReadOnly;
                        dxImageEdit.MaxLength = g.MaxLength;
                        dxImageEdit.SelectedColor = g.SelectedColor;
                        dxImageEdit.SelBackColor = g.SelBackColor;
                        dxImageEdit.SelFontColor = g.SelFontColor;
                        dxImageEdit.InValue = g.InValue;
                        dxImageEdit.PasswordChar = g.PasswordChar;
                        dxImageEdit.AllowSelect = g.AllowSelect;
                        dxImageEdit.AllowPaste = g.AllowPaste;
                        dxImageEdit.TabOrder = g.TabOrder;
                        dxImageEdit.Font.Name = reader.ReadGuiFontName(g.FontColor);

                        if (g.TextLen > 0)
                        {
                            sText = reader.ReadFixedString(g.TextLen);
                            dxImageEdit.Text = sText;
                        }
                    }
                    else if (guiVersion < Ver20190724)
                    {
                        reader.ReadRecord(TGuiImageEdit.SizeOf, TGuiImageEdit.ReadAt, out var g);
                        dxImageEdit.Text = string.Empty;
                        dxImageEdit.BackgroundColor = g.BackgroundColor;
                        dxImageEdit.DrawBorder = g.DrawBorder;
                        DxGuiFonts.DxFontAssign(dxImageEdit.Font, g.FontColor);

                        dxImageEdit.DisableBackgroundColor = g.DisableBackgroundColor;
                        DxGuiFonts.DxFontAssign(dxImageEdit.HintTextFont, g.HintTextFont);
                        dxImageEdit.HintTextAlignment = g.HintTextAlignment;

                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Up, g.BorderColor.Up);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Hot, g.BorderColor.Hot);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Down, g.BorderColor.Down);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Disabled, g.BorderColor.Disabled);

                        dxImageEdit.ReadOnly = g.ReadOnly;
                        dxImageEdit.MaxLength = g.MaxLength;
                        dxImageEdit.SelectedColor = g.SelectedColor;
                        dxImageEdit.SelBackColor = g.SelBackColor;
                        dxImageEdit.SelFontColor = g.SelFontColor;
                        dxImageEdit.InValue = g.InValue;
                        dxImageEdit.PasswordChar = g.PasswordChar;
                        dxImageEdit.AllowSelect = g.AllowSelect;
                        dxImageEdit.AllowPaste = g.AllowPaste;
                        dxImageEdit.TabOrder = g.TabOrder;

                        dxImageEdit.Font.Name = reader.ReadGuiFontName(g.FontColor);
                        dxImageEdit.HintTextFont.Name = reader.ReadGuiFontName(g.HintTextFont);

                        if (g.TextLen > 0)
                        {
                            sText = reader.ReadFixedString(g.TextLen);
                            dxImageEdit.Text = sText;
                        }

                        if (g.HintTextLen > 0)
                        {
                            sText = reader.ReadFixedString(g.HintTextLen);
                            dxImageEdit.HintText = sText;
                        }
                    }
                    else
                    {
                        reader.ReadRecord(TGuiImageEdit_New.SizeOf, TGuiImageEdit_New.ReadAt, out var g);
                        dxImageEdit.Text = string.Empty;
                        dxImageEdit.DrawBorder = g.DrawBorder;
                        DxGuiFonts.DxFontAssign(dxImageEdit.Font, g.FontColor);

                        dxImageEdit.BackgroundColor = g.BackgroundColor;
                        dxImageEdit.BackgroundColorAlpha = g.BackgroundColorAlpha;
                        dxImageEdit.BackgroundImage.ImageType = g.BackgroundImage.ImageType;
                        dxImageEdit.BackgroundImage.BlendDraw = g.BackgroundImage.BlendMode;
                        dxImageEdit.BackgroundImage.OutsideAreaDraw = g.BackgroundImage.OutsideAreaDraw;
                        dxImageEdit.BackgroundImage.ImageIndex = g.BackgroundImage.ImageIndex;
                        dxImageEdit.BackgroundImage.OffsetX = g.BackgroundImage.OffsetX;
                        dxImageEdit.BackgroundImage.OffsetY = g.BackgroundImage.OffsetY;

                        dxImageEdit.DisableHideCtrl = g.DisableHideCtrl;
                        dxImageEdit.DisableBackgroundTransparent = g.DisableBackgroundTransparent;
                        dxImageEdit.DisableBackgroundColor = g.DisableBackgroundColor;
                        dxImageEdit.DisableBackgroundAlpha = g.DisableBackgroundAlpha;
                        dxImageEdit.DisableBackgroundImage.ImageType = g.DisableBackgroundImage.ImageType;
                        dxImageEdit.DisableBackgroundImage.BlendDraw = g.DisableBackgroundImage.BlendMode;
                        dxImageEdit.DisableBackgroundImage.OutsideAreaDraw = g.DisableBackgroundImage.OutsideAreaDraw;
                        dxImageEdit.DisableBackgroundImage.ImageIndex = g.DisableBackgroundImage.ImageIndex;
                        dxImageEdit.DisableBackgroundImage.OffsetX = g.DisableBackgroundImage.OffsetX;
                        dxImageEdit.DisableBackgroundImage.OffsetY = g.DisableBackgroundImage.OffsetY;

                        DxGuiFonts.DxFontAssign(dxImageEdit.HintTextFont, g.HintTextFont);
                        dxImageEdit.HintTextAlignment = g.HintTextAlignment;

                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Up, g.BorderColor.Up);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Hot, g.BorderColor.Hot);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Down, g.BorderColor.Down);
                        DxGuiFonts.DxFontAssign(dxImageEdit.BorderColor.Disabled, g.BorderColor.Disabled);

                        dxImageEdit.ReadOnly = g.ReadOnly;
                        dxImageEdit.MaxLength = g.MaxLength;
                        dxImageEdit.SelectedColor = g.SelectedColor;
                        dxImageEdit.SelBackColor = g.SelBackColor;
                        dxImageEdit.SelFontColor = g.SelFontColor;
                        dxImageEdit.InValue = g.InValue;
                        dxImageEdit.PasswordChar = g.PasswordChar;
                        dxImageEdit.AllowSelect = g.AllowSelect;
                        dxImageEdit.AllowPaste = g.AllowPaste;
                        dxImageEdit.TabOrder = g.TabOrder;

                        dxImageEdit.Font.Name = reader.ReadGuiFontName(g.FontColor);
                        dxImageEdit.HintTextFont.Name = reader.ReadGuiFontName(g.HintTextFont);

                        if (g.TextLen > 0)
                        {
                            sText = reader.ReadFixedString(g.TextLen);
                            dxImageEdit.Text = sText;
                        }

                        if (g.HintTextLen > 0)
                        {
                            sText = reader.ReadFixedString(g.HintTextLen);
                            dxImageEdit.HintText = sText;
                        }
                    }
                }
                break;

            case TGuiType.t_Label:
                {
                    var dxLabel = (TDxLabel)dxControl;
                    if (guiVersion < Ver20160409)
                    {
                        reader.ReadRecord(TGuiLabel.SizeOf, TGuiLabel.ReadAt, out var g);
                        dxLabel.AutoSize = g.AutoSize;
                        dxLabel.BackgroundColor = g.BackgroundColor;
                        dxLabel.DrawBorder = g.DrawBorder;
                        dxLabel.CaptionDownOffsetX = g.CaptionDownOffsetX;
                        dxLabel.CaptionDownOffsetY = g.CaptionDownOffsetY;
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Up, g.CaptionColor.Up);
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Hot, g.CaptionColor.Hot);
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Down, g.CaptionColor.Down);
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Disabled, g.CaptionColor.Disabled);

                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Up, g.BorderColor.Up);
                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Hot, g.BorderColor.Hot);
                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Down, g.BorderColor.Down);
                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Disabled, g.BorderColor.Disabled);

                        dxLabel.ClickSound = g.ClickCount;
                        dxLabel.Style = g.Style;
                        dxLabel.Caption = string.Empty;

                        dxLabel.CaptionColor.Up.Name = reader.ReadGuiFontName(g.CaptionColor.Up);
                        dxLabel.CaptionColor.Hot.Name = reader.ReadGuiFontName(g.CaptionColor.Hot);
                        dxLabel.CaptionColor.Down.Name = reader.ReadGuiFontName(g.CaptionColor.Down);
                        dxLabel.CaptionColor.Disabled.Name = reader.ReadGuiFontName(g.CaptionColor.Disabled);

                        if (g.CaptionLen > 0)
                        {
                            sText = reader.ReadFixedString(g.CaptionLen);
                            dxLabel.Caption = sText;
                        }
                    }
                    else
                    {
                        reader.ReadRecord(TGuiLabel_New.SizeOf, TGuiLabel_New.ReadAt, out var g);
                        dxLabel.AutoSize = g.AutoSize;
                        // 原文如此（LoadDxControl.pas:681）：Alignment 只在 _New 布局里存在，
                        // 旧布局（TGuiLabel）没有该字段，故旧分支不赋值。
                        dxLabel.Alignment = g.Alignment;
                        dxLabel.BackgroundColor = g.BackgroundColor;
                        dxLabel.DrawBorder = g.DrawBorder;
                        dxLabel.CaptionDownOffsetX = g.CaptionDownOffsetX;
                        dxLabel.CaptionDownOffsetY = g.CaptionDownOffsetY;
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Up, g.CaptionColor.Up);
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Hot, g.CaptionColor.Hot);
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Down, g.CaptionColor.Down);
                        DxGuiFonts.DxFontAssign(dxLabel.CaptionColor.Disabled, g.CaptionColor.Disabled);

                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Up, g.BorderColor.Up);
                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Hot, g.BorderColor.Hot);
                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Down, g.BorderColor.Down);
                        DxGuiFonts.DxFontAssign(dxLabel.BorderColor.Disabled, g.BorderColor.Disabled);

                        dxLabel.ClickSound = g.ClickCount;
                        dxLabel.Style = g.Style;
                        dxLabel.Caption = string.Empty;

                        dxLabel.CaptionColor.Up.Name = reader.ReadGuiFontName(g.CaptionColor.Up);
                        dxLabel.CaptionColor.Hot.Name = reader.ReadGuiFontName(g.CaptionColor.Hot);
                        dxLabel.CaptionColor.Down.Name = reader.ReadGuiFontName(g.CaptionColor.Down);
                        dxLabel.CaptionColor.Disabled.Name = reader.ReadGuiFontName(g.CaptionColor.Disabled);

                        if (g.CaptionLen > 0)
                        {
                            sText = reader.ReadFixedString(g.CaptionLen);
                            dxLabel.Caption = sText;
                        }
                    }
                }
                break;

            case TGuiType.t_Grid:
                {
                    reader.ReadRecord(TGuiImageGrid.SizeOf, TGuiImageGrid.ReadAt, out var g);
                    var dxImageGrid = (TDxImageGrid)dxControl;
                    dxImageGrid.ColCount = g.ColCount;
                    dxImageGrid.RowCount = g.RowCount;
                    dxImageGrid.ColWidth = g.ColWidth;
                    dxImageGrid.RowHeight = g.RowHeight;
                    dxImageGrid.ViewTopLine = g.ViewTopLine;
                }
                break;

            case TGuiType.t_ScrollBox:
            case TGuiType.t_ChatMemo:
            case TGuiType.t_ListView:
            case TGuiType.t_TreeView:
                {
                    var dxScrollControl = (TDxScrollControl)dxControl;
                    if (guiVersion < Ver20160508)
                    {
                        reader.ReadRecord(TGuiMemo.SizeOf, TGuiMemo.ReadAt, out var g);
                        dxScrollControl.ShowScroll = g.ShowScroll;
                        dxScrollControl.ItemHeight = g.ItemHeight;
                        dxScrollControl.ItemIndex = g.ItemIndex;
                        dxScrollControl.ScrollBars = g.ScrollBars;
                        dxScrollControl.ScrollSize = g.ScrollSize;

                        dxScrollControl.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxScrollControl.ImageIndex.Up = g.ImageIndex.Up;
                        dxScrollControl.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxScrollControl.ImageIndex.Down = g.ImageIndex.Down;
                        dxScrollControl.ImageIndex.Disabled = g.ImageIndex.Disabled;

                        dxScrollControl.ScrollImageIndex.ImageType = g.ScrollImageIndex.Image;
                        dxScrollControl.ScrollImageIndex.Up = g.ScrollImageIndex.Up;
                        dxScrollControl.ScrollImageIndex.Hot = g.ScrollImageIndex.Hot;
                        dxScrollControl.ScrollImageIndex.Down = g.ScrollImageIndex.Down;
                        dxScrollControl.ScrollImageIndex.Disabled = g.ScrollImageIndex.Disabled;

                        dxScrollControl.PrevImageIndex.ImageType = g.PrevImageIndex.Image;
                        dxScrollControl.PrevImageIndex.Up = g.PrevImageIndex.Up;
                        dxScrollControl.PrevImageIndex.Hot = g.PrevImageIndex.Hot;
                        dxScrollControl.PrevImageIndex.Down = g.PrevImageIndex.Down;
                        dxScrollControl.PrevImageIndex.Disabled = g.PrevImageIndex.Disabled;

                        dxScrollControl.NextImageIndex.ImageType = g.NextImageIndex.Image;
                        dxScrollControl.NextImageIndex.Up = g.NextImageIndex.Up;
                        dxScrollControl.NextImageIndex.Hot = g.NextImageIndex.Hot;
                        dxScrollControl.NextImageIndex.Down = g.NextImageIndex.Down;
                        dxScrollControl.NextImageIndex.Disabled = g.NextImageIndex.Disabled;

                        dxScrollControl.BarImageIndex.ImageType = g.BarImageIndex.Image;
                        dxScrollControl.BarImageIndex.Up = g.BarImageIndex.Up;
                        dxScrollControl.BarImageIndex.Hot = g.BarImageIndex.Hot;
                        dxScrollControl.BarImageIndex.Down = g.BarImageIndex.Down;
                        dxScrollControl.BarImageIndex.Disabled = g.BarImageIndex.Disabled;

                        dxScrollControl.ExpandSize = g.ExpandSize;
                        dxScrollControl.Position = g.Position;
                        dxScrollControl.VisibleItemCount = g.VisibleItemCount;
                        dxScrollControl.OffSetX = g.OffSetX;
                        dxScrollControl.OffSetY = g.OffSetY;

                        dxScrollControl.ShowItemCount = g.ShowItemCount;
                        if (dxScrollControl is TDxTreeView treeView)
                            treeView.ShowButton = g.ShowButton;

                        if (dxScrollControl is TDxListView listView)
                        {
                            listView.ColCount = g.ColCount;
                            listView.ShowGridLine = g.ShowGridLine;
                            listView.GridLineColor = g.GridLineColor;
                            listView.CheckItemControlSize = g.CheckItemControlSize;

                            for (i = 0; i <= listView.ColCount - 1; i++)
                            {
                                reader.ReadRecord(16, ReadRect, out var colRect);
                                listView.ColRects[i] = colRect;
                            }

                            for (i = 0; i <= listView.ColCount - 1; i++)
                            {
                                reader.ReadRecord(TGuiViewField.SizeOf, TGuiViewField.ReadAt, out var guiViewField);
                                listView.Fields[i].Alignment = guiViewField.Alignment;
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Up, guiViewField.Color.Up);
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Hot, guiViewField.Color.Hot);
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Down, guiViewField.Color.Down);
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Disabled, guiViewField.Color.Disabled);

                                listView.Fields[i].Color.Up.Name = reader.ReadGuiFontName(guiViewField.Color.Up);
                                listView.Fields[i].Color.Hot.Name = reader.ReadGuiFontName(guiViewField.Color.Hot);
                                listView.Fields[i].Color.Down.Name = reader.ReadGuiFontName(guiViewField.Color.Down);
                                listView.Fields[i].Color.Disabled.Name = reader.ReadGuiFontName(guiViewField.Color.Disabled);

                                if (guiViewField.CaptionLen > 0)
                                {
                                    sText = reader.ReadFixedString(guiViewField.CaptionLen);
                                    listView.Fields[i].Caption = sText;
                                }
                            }
                        }
                    }
                    else
                    {
                        reader.ReadRecord(TGuiMemo_New.SizeOf, TGuiMemo_New.ReadAt, out var g);
                        dxScrollControl.ShowScroll = g.ShowScroll;
                        dxScrollControl.ItemHeight = g.ItemHeight;
                        dxScrollControl.ItemIndex = g.ItemIndex;
                        dxScrollControl.ScrollBars = g.ScrollBars;
                        dxScrollControl.ScrollSize = g.ScrollSize;

                        dxScrollControl.ImageIndex.ImageType = g.ImageIndex.Image;
                        dxScrollControl.ImageIndex.Up = g.ImageIndex.Up;
                        dxScrollControl.ImageIndex.Hot = g.ImageIndex.Hot;
                        dxScrollControl.ImageIndex.Down = g.ImageIndex.Down;
                        dxScrollControl.ImageIndex.Disabled = g.ImageIndex.Disabled;

                        dxScrollControl.ScrollImageIndex.ImageType = g.ScrollImageIndex.Image;
                        dxScrollControl.ScrollImageIndex.Up = g.ScrollImageIndex.Up;
                        dxScrollControl.ScrollImageIndex.Hot = g.ScrollImageIndex.Hot;
                        dxScrollControl.ScrollImageIndex.Down = g.ScrollImageIndex.Down;
                        dxScrollControl.ScrollImageIndex.Disabled = g.ScrollImageIndex.Disabled;

                        dxScrollControl.PrevImageIndex.ImageType = g.PrevImageIndex.Image;
                        dxScrollControl.PrevImageIndex.Up = g.PrevImageIndex.Up;
                        dxScrollControl.PrevImageIndex.Hot = g.PrevImageIndex.Hot;
                        dxScrollControl.PrevImageIndex.Down = g.PrevImageIndex.Down;
                        dxScrollControl.PrevImageIndex.Disabled = g.PrevImageIndex.Disabled;

                        dxScrollControl.NextImageIndex.ImageType = g.NextImageIndex.Image;
                        dxScrollControl.NextImageIndex.Up = g.NextImageIndex.Up;
                        dxScrollControl.NextImageIndex.Hot = g.NextImageIndex.Hot;
                        dxScrollControl.NextImageIndex.Down = g.NextImageIndex.Down;
                        dxScrollControl.NextImageIndex.Disabled = g.NextImageIndex.Disabled;

                        dxScrollControl.BarImageIndex.ImageType = g.BarImageIndex.Image;
                        dxScrollControl.BarImageIndex.Up = g.BarImageIndex.Up;
                        dxScrollControl.BarImageIndex.Hot = g.BarImageIndex.Hot;
                        dxScrollControl.BarImageIndex.Down = g.BarImageIndex.Down;
                        dxScrollControl.BarImageIndex.Disabled = g.BarImageIndex.Disabled;

                        dxScrollControl.ExpandSize = g.ExpandSize;
                        dxScrollControl.Position = g.Position;
                        dxScrollControl.VisibleItemCount = g.VisibleItemCount;
                        dxScrollControl.OffSetX = g.OffSetX;
                        dxScrollControl.OffSetY = g.OffSetY;

                        dxScrollControl.ShowItemCount = g.ShowItemCount;

                        dxScrollControl.BackgroundColor = g.BackGroupColor;

                        if (dxScrollControl is TDxTreeView treeView)
                            treeView.ShowButton = g.ShowButton;

                        if (g.FontLen > 0)
                        {
                            sText = reader.ReadFixedString(g.FontLen);
                        }
                        else
                            sText = string.Empty;

                        if (dxScrollControl is TDxChatMemo chatMemo)
                        {
                            chatMemo.FontName = sText;
                            chatMemo.FontSize = g.FontSize;
                            chatMemo.FontStroke = g.FontStroke;
                            chatMemo.FontBackTransparent = g.FontBackTransparent;
                        }

                        if (dxScrollControl is TDxListView listView)
                        {
                            listView.ColCount = g.ColCount;
                            listView.ShowGridLine = g.ShowGridLine;
                            listView.GridLineColor = g.GridLineColor;
                            listView.CheckItemControlSize = g.CheckItemControlSize;
                            for (i = 0; i <= listView.ColCount - 1; i++)
                            {
                                reader.ReadRecord(16, ReadRect, out var colRect);
                                listView.ColRects[i] = colRect;
                            }

                            for (i = 0; i <= listView.ColCount - 1; i++)
                            {
                                reader.ReadRecord(TGuiViewField.SizeOf, TGuiViewField.ReadAt, out var guiViewField);
                                listView.Fields[i].Alignment = guiViewField.Alignment;
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Up, guiViewField.Color.Up);
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Hot, guiViewField.Color.Hot);
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Down, guiViewField.Color.Down);
                                DxGuiFonts.DxFontAssign(listView.Fields[i].Color.Disabled, guiViewField.Color.Disabled);

                                listView.Fields[i].Color.Up.Name = reader.ReadGuiFontName(guiViewField.Color.Up);
                                listView.Fields[i].Color.Hot.Name = reader.ReadGuiFontName(guiViewField.Color.Hot);
                                listView.Fields[i].Color.Down.Name = reader.ReadGuiFontName(guiViewField.Color.Down);
                                listView.Fields[i].Color.Disabled.Name = reader.ReadGuiFontName(guiViewField.Color.Disabled);

                                if (guiViewField.CaptionLen > 0)
                                {
                                    sText = reader.ReadFixedString(guiViewField.CaptionLen);
                                    listView.Fields[i].Caption = sText;
                                }
                            }
                        }
                    }
                }
                break;

            case TGuiType.t_PopupMenu:
                {
                    reader.ReadRecord(TGuiPopupMenu.SizeOf, TGuiPopupMenu.ReadAt, out var g);
                    var dxPopupMenu = (TDxPopupMenu)dxControl;
                    dxPopupMenu.BackgroundColor = g.BackgroundColor;
                    dxPopupMenu.DrawBorder = g.DrawBorder;

                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Up, g.ItemColor.Up);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Hot, g.ItemColor.Hot);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Down, g.ItemColor.Down);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Disabled, g.ItemColor.Disabled);

                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Up, g.BorderColor.Up);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Hot, g.BorderColor.Hot);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Down, g.BorderColor.Down);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Disabled, g.BorderColor.Disabled);

                    dxPopupMenu.SelectColor = g.SelectColor;
                    dxPopupMenu.ItemHeight = g.ItemHeight;
                    dxPopupMenu.ItemIndex = g.ItemIndex;

                    dxPopupMenu.ItemColor.Up.Name = reader.ReadGuiFontName(g.ItemColor.Up);
                    dxPopupMenu.ItemColor.Hot.Name = reader.ReadGuiFontName(g.ItemColor.Hot);
                    dxPopupMenu.ItemColor.Down.Name = reader.ReadGuiFontName(g.ItemColor.Down);
                    dxPopupMenu.ItemColor.Disabled.Name = reader.ReadGuiFontName(g.ItemColor.Disabled);

                    if (g.ItemTextLen > 0)
                    {
                        sText = reader.ReadFixedString(g.ItemTextLen);
                        dxPopupMenu.Items.Text = sText;
                    }
                }
                break;

            case TGuiType.t_PageControl:
                {
                    var dxPageControl = (TDxPageControl)dxControl;
                    if (guiVersion < Ver20160514)
                    {
                        reader.ReadRecord(TGuiPageControl.SizeOf, TGuiPageControl.ReadAt, out var g);
                        dxPageControl.ClientLeft = g.ClientLeft;
                        dxPageControl.ClientTop = g.ClientTop;
                        dxPageControl.ClientWidth = g.ClientWidth;
                        dxPageControl.ClientHeight = g.ClientHeight;
                        dxPageControl.TabPosition = g.TabPosition;
                        dxPageControl.ButtonWidth = g.ButtonWidth;
                        dxPageControl.ButtonHeight = g.ButtonHeight;
                        dxPageControl.ShowButton = g.ShowButton;
                        dxPageControl.OffSetX = g.OffSetX;
                        dxPageControl.OffSetY = g.OffSetY;
                    }
                    else
                    {
                        reader.ReadRecord(TGuiPageControl_New.SizeOf, TGuiPageControl_New.ReadAt, out var g);
                        dxPageControl.ClientLeft = g.ClientLeft;
                        dxPageControl.ClientTop = g.ClientTop;
                        dxPageControl.ClientWidth = g.ClientWidth;
                        dxPageControl.ClientHeight = g.ClientHeight;
                        dxPageControl.TabPosition = g.TabPosition;
                        dxPageControl.ButtonWidth = g.ButtonWidth;
                        dxPageControl.ButtonHeight = g.ButtonHeight;
                        dxPageControl.ShowButton = g.ShowButton;
                        dxPageControl.OffSetX = g.OffSetX;
                        dxPageControl.OffSetY = g.OffSetY;
                        // 原文如此（LoadDxControl.pas:970-974）：_New 布局多出的 5 个字段旧布局没有。
                        dxPageControl.CaptionOffsetX = g.CaptionOffsetX;
                        dxPageControl.CaptionOffsetY = g.CaptionOffsetY;
                        dxPageControl.DownCaptionOffsetX = g.DownCaptionOffsetX;
                        dxPageControl.DownCaptionOffsetY = g.DownCaptionOffsetY;
                        dxPageControl.ReverseDrawButton = g.ReverseDrawButton;
                    }
                }
                break;

            case TGuiType.t_ComboBox:
                {
                    reader.ReadRecord(TGuiComboBox.SizeOf, TGuiComboBox.ReadAt, out var g);
                    var dxComboBox = (TDxComboBox)dxControl;
                    var dxPopupMenu = dxComboBox.PopupMenu;

                    sText = string.Empty;
                    dxPopupMenu.BackgroundColor = g.GuiPopupMenu.BackgroundColor;
                    dxPopupMenu.DrawBorder = g.GuiPopupMenu.DrawBorder;

                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Up, g.GuiPopupMenu.ItemColor.Up);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Hot, g.GuiPopupMenu.ItemColor.Hot);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Down, g.GuiPopupMenu.ItemColor.Down);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.ItemColor.Disabled, g.GuiPopupMenu.ItemColor.Disabled);

                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Up, g.GuiPopupMenu.BorderColor.Up);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Hot, g.GuiPopupMenu.BorderColor.Hot);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Down, g.GuiPopupMenu.BorderColor.Down);
                    DxGuiFonts.DxFontAssign(dxPopupMenu.BorderColor.Disabled, g.GuiPopupMenu.BorderColor.Disabled);

                    dxPopupMenu.SelectColor = g.GuiPopupMenu.SelectColor;
                    dxPopupMenu.ItemHeight = g.GuiPopupMenu.ItemHeight;
                    dxPopupMenu.ItemIndex = g.GuiPopupMenu.ItemIndex;

                    dxPopupMenu.ItemColor.Up.Name = reader.ReadGuiFontName(g.GuiPopupMenu.ItemColor.Up);
                    dxPopupMenu.ItemColor.Hot.Name = reader.ReadGuiFontName(g.GuiPopupMenu.ItemColor.Hot);
                    dxPopupMenu.ItemColor.Down.Name = reader.ReadGuiFontName(g.GuiPopupMenu.ItemColor.Down);
                    dxPopupMenu.ItemColor.Disabled.Name = reader.ReadGuiFontName(g.GuiPopupMenu.ItemColor.Disabled);

                    dxComboBox.BackgroundColor = g.BackgroundColor;
                    dxComboBox.DrawBorder = g.DrawBorder;
                    dxComboBox.ButtonColor = g.ButtonColor;

                    DxGuiFonts.DxFontAssign(dxComboBox.TextColor.Up, g.TextColor.Up);
                    DxGuiFonts.DxFontAssign(dxComboBox.TextColor.Hot, g.TextColor.Hot);
                    DxGuiFonts.DxFontAssign(dxComboBox.TextColor.Down, g.TextColor.Down);
                    DxGuiFonts.DxFontAssign(dxComboBox.TextColor.Disabled, g.TextColor.Disabled);

                    DxGuiFonts.DxFontAssign(dxComboBox.BorderColor.Up, g.BorderColor.Up);
                    DxGuiFonts.DxFontAssign(dxComboBox.BorderColor.Hot, g.BorderColor.Hot);
                    DxGuiFonts.DxFontAssign(dxComboBox.BorderColor.Down, g.BorderColor.Down);
                    DxGuiFonts.DxFontAssign(dxComboBox.BorderColor.Disabled, g.BorderColor.Disabled);

                    dxComboBox.TextColor.Up.Name = reader.ReadGuiFontName(g.TextColor.Up);
                    dxComboBox.TextColor.Hot.Name = reader.ReadGuiFontName(g.TextColor.Hot);
                    dxComboBox.TextColor.Down.Name = reader.ReadGuiFontName(g.TextColor.Down);
                    dxComboBox.TextColor.Disabled.Name = reader.ReadGuiFontName(g.TextColor.Disabled);

                    if (g.TextLen > 0)
                    {
                        sText = reader.ReadFixedString(g.TextLen);
                        dxComboBox.Text = sText;
                    }
                    if (g.ItemLen > 0)
                    {
                        sText = reader.ReadFixedString(g.ItemLen);
                        dxComboBox.Items.Text = sText;
                    }
                }
                break;

            case TGuiType.t_TabSheet:
                {
                    reader.ReadRecord(TGuiTabSheet.SizeOf, TGuiTabSheet.ReadAt, out var g);
                    var dxTabSheet = (TDxTabSheet)dxControl;
                    dxTabSheet.OffSetX = g.OffSetX;
                    dxTabSheet.OffSetY = g.OffSetY;
                    dxTabSheet.TabVisible = !g.HideTable;

                    dxTabSheet.Caption = string.Empty;
                    dxTabSheet.ImageIndex.ImageType = g.ImageIndex.Image;
                    dxTabSheet.ImageIndex.Up = g.ImageIndex.Up;
                    dxTabSheet.ImageIndex.Hot = g.ImageIndex.Hot;
                    dxTabSheet.ImageIndex.Down = g.ImageIndex.Down;
                    dxTabSheet.ImageIndex.Disabled = g.ImageIndex.Disabled;

                    DxGuiFonts.DxFontAssign(dxTabSheet.CaptionColor.Up, g.CaptionColor.Up);
                    DxGuiFonts.DxFontAssign(dxTabSheet.CaptionColor.Hot, g.CaptionColor.Hot);
                    DxGuiFonts.DxFontAssign(dxTabSheet.CaptionColor.Down, g.CaptionColor.Down);
                    DxGuiFonts.DxFontAssign(dxTabSheet.CaptionColor.Disabled, g.CaptionColor.Disabled);

                    dxTabSheet.CaptionColor.Up.Name = reader.ReadGuiFontName(g.CaptionColor.Up);
                    dxTabSheet.CaptionColor.Hot.Name = reader.ReadGuiFontName(g.CaptionColor.Hot);
                    dxTabSheet.CaptionColor.Down.Name = reader.ReadGuiFontName(g.CaptionColor.Down);
                    dxTabSheet.CaptionColor.Disabled.Name = reader.ReadGuiFontName(g.CaptionColor.Disabled);

                    if (g.CaptionLen > 0)
                    {
                        sText = reader.ReadFixedString(g.CaptionLen);
                        dxTabSheet.Caption = sText;
                    }

                    // 原文如此（LoadDxControl.pas:1072）：无条件把宿主 PageControl 的 ActivePageIndex 归零。
                    // 原文在 Owner 不是 TDxPageControl 时会硬类型转换失败（EInvalidCast）；
                    // 托管侧 as-转换得到 null 则跳过（差异见报告"缺陷"节）。
                    if (dxTabSheet.Owner != null) dxTabSheet.Owner.ActivePageIndex = 0;
                }
                break;

            case TGuiType.t_Line:
                {
                    reader.ReadRecord(TGuiLine.SizeOf, TGuiLine.ReadAt, out var g);
                    var dxLine = (TDxLine)dxControl;
                    dxLine.Style = g.LineStyle;
                    DxGuiFonts.DxFontAssign(dxLine.LineColor.Up, g.LineColor.Up);
                    DxGuiFonts.DxFontAssign(dxLine.LineColor.Hot, g.LineColor.Hot);
                    DxGuiFonts.DxFontAssign(dxLine.LineColor.Down, g.LineColor.Down);
                    DxGuiFonts.DxFontAssign(dxLine.LineColor.Disabled, g.LineColor.Disabled);
                }
                break;

            case TGuiType.t_TrackBar:
                {
                    // 原文如此（LoadDxControl.pas:1084 与 LoadDxControlEx.pas:1082）：
                    // 唯一一处写成 SizeOf(GuiTrackBar)（变量）而不是 SizeOf(TGuiTrackBar)，结果相同。
                    reader.ReadRecord(TGuiTrackBar.SizeOf, TGuiTrackBar.ReadAt, out var g);
                    var dxTrackBar = (TDXTrackBar)dxControl;
                    dxTrackBar.AutoSize = g.AutoSize;

                    dxTrackBar.ImageIndex.ImageType = g.ImageIndex.Image;
                    dxTrackBar.ImageIndex.Up = g.ImageIndex.Up;
                    dxTrackBar.ImageIndex.Hot = g.ImageIndex.Hot;
                    dxTrackBar.ImageIndex.Down = g.ImageIndex.Down;
                    dxTrackBar.ImageIndex.Disabled = g.ImageIndex.Disabled;

                    dxTrackBar.SliderIndex.ImageType = g.SliderIndex.Image;
                    dxTrackBar.SliderIndex.Up = g.SliderIndex.Up;
                    dxTrackBar.SliderIndex.Hot = g.SliderIndex.Hot;
                    dxTrackBar.SliderIndex.Down = g.SliderIndex.Down;
                    dxTrackBar.SliderIndex.Disabled = g.SliderIndex.Disabled;

                    dxTrackBar.Min = g.Min;
                    dxTrackBar.Max = g.Max;
                    dxTrackBar.Position = g.Position;
                }
                break;

            case TGuiType.t_MainBottomForm:
                {
                    var dxMainBottomForm = (TDxMainBottomForm)dxControl;
                    if (guiVersion < Ver20190729)
                    {
                        reader.ReadRecord(TGuiMainBottomForm.SizeOf, TGuiMainBottomForm.ReadAt, out var g);
                        dxMainBottomForm.LeftImage.ImageType = g.LeftImageType;
                        dxMainBottomForm.LeftImage.Index = g.LeftImageIndex;

                        dxMainBottomForm.RightImage.ImageType = g.RightImageType;
                        dxMainBottomForm.RightImage.Index = g.RightImageIndex;

                        AssignMainBottomCenter(dxMainBottomForm.CenterSetting, g.Center);
                        // 原文如此（LoadDxControl.pas:1141-1142）：旧布局没有 BottomImage。
                    }
                    else if (guiVersion < Ver20211120)
                    {
                        reader.ReadRecord(TGuiMainBottomForm_New.SizeOf, TGuiMainBottomForm_New.ReadAt, out var g);
                        dxMainBottomForm.LeftImage.ImageType = g.LeftImageType;
                        dxMainBottomForm.LeftImage.Index = g.LeftImageIndex;

                        dxMainBottomForm.RightImage.ImageType = g.RightImageType;
                        dxMainBottomForm.RightImage.Index = g.RightImageIndex;

                        AssignMainBottomCenter(dxMainBottomForm.CenterSetting, g.Center);

                        AssignMainBottomAnimation(dxMainBottomForm.Animation1, g.Animation1);
                        AssignMainBottomAnimation(dxMainBottomForm.Animation2, g.Animation2);
                        AssignMainBottomAnimation(dxMainBottomForm.Animation3, g.Animation3);
                        AssignMainBottomAnimation(dxMainBottomForm.Animation4, g.Animation4);
                    }
                    else
                    {
                        reader.ReadRecord(TGuiMainBottomForm_New2.SizeOf, TGuiMainBottomForm_New2.ReadAt, out var g);
                        dxMainBottomForm.LeftImage.ImageType = g.LeftImageType;
                        dxMainBottomForm.LeftImage.Index = g.LeftImageIndex;

                        dxMainBottomForm.RightImage.ImageType = g.RightImageType;
                        dxMainBottomForm.RightImage.Index = g.RightImageIndex;

                        dxMainBottomForm.BottomImage.ImageType = g.BottomImageType;
                        dxMainBottomForm.BottomImage.Index = g.BottomImageIndex;

                        AssignMainBottomCenter(dxMainBottomForm.CenterSetting, g.Center);

                        AssignMainBottomAnimation(dxMainBottomForm.Animation1, g.Animation1);
                        AssignMainBottomAnimation(dxMainBottomForm.Animation2, g.Animation2);
                        AssignMainBottomAnimation(dxMainBottomForm.Animation3, g.Animation3);
                        AssignMainBottomAnimation(dxMainBottomForm.Animation4, g.Animation4);
                    }
                }
                break;

            case TGuiType.t_MagicBall:
                {
                    var dxMagicBall = (TDxMagicBall)dxControl;
                    if (guiVersion < Ver20160818)
                    {
                        reader.ReadRecord(TGuiMagicBall.SizeOf, TGuiMagicBall.ReadAt, out var g);
                        dxMagicBall.BallType = g.BallType;
                        dxMagicBall.ValueAlignment = g.ValueAlignment;
                        dxMagicBall.OverallSetting.ImageType = g.Overall_ImageType;
                        dxMagicBall.OverallSetting.EmptyHPMP = g.Overall_EmptyHPMP;
                        dxMagicBall.OverallSetting.FullHPMP = g.Overall_FullHPMP;
                        dxMagicBall.OverallSetting.EmptyHP = g.Overall_EmptyHP;
                        dxMagicBall.OverallSetting.FullHP = g.Overall_FullHP;
                        dxMagicBall.OverallSetting.Splite = g.Overall_Splite;
                        dxMagicBall.OverallSetting.MiddleZoneWidth = g.Overall_MiddleZoneWidth;
                        dxMagicBall.AloneSetting.ImageType = g.Alone_ImageType;
                        dxMagicBall.AloneSetting.Empty = g.Alone_Empty;
                        dxMagicBall.AloneSetting.Full = g.Alone_Full;
                    }
                    else
                    {
                        reader.ReadRecord(TGuiMagicBall2.SizeOf, TGuiMagicBall2.ReadAt, out var g);
                        dxMagicBall.BallType = g.BallType;
                        dxMagicBall.ValueAlignment = g.ValueAlignment;
                        dxMagicBall.OverallSetting.ImageType = g.Overall_ImageType;
                        dxMagicBall.OverallSetting.EmptyHPMP = g.Overall_EmptyHPMP;
                        dxMagicBall.OverallSetting.FullHPMP = g.Overall_FullHPMP;
                        dxMagicBall.OverallSetting.EmptyHP = g.Overall_EmptyHP;
                        dxMagicBall.OverallSetting.FullHP = g.Overall_FullHP;
                        dxMagicBall.OverallSetting.Splite = g.Overall_Splite;
                        dxMagicBall.OverallSetting.MiddleZoneWidth = g.Overall_MiddleZoneWidth;

                        // 原文如此（LoadDxControl.pas:1392-1397）：Effect* 五件套只在 MagicBall2 布局里。
                        dxMagicBall.OverallSetting.EffectDrawBlend = g.Overall_EffectDrawBlend;
                        dxMagicBall.OverallSetting.EffectImageType = g.Overall_EffectImageType;
                        dxMagicBall.OverallSetting.EffectHPMPStart = g.Overall_EffectHPMPStart;
                        dxMagicBall.OverallSetting.EffectHPStart = g.Overall_EffectHPStart;
                        dxMagicBall.OverallSetting.EffectImageCount = g.Overall_EffectImageCount;
                        dxMagicBall.OverallSetting.EffectPlayInterval = g.Overall_EffectPlayInterval;

                        dxMagicBall.AloneSetting.ImageType = g.Alone_ImageType;
                        dxMagicBall.AloneSetting.Empty = g.Alone_Empty;
                        dxMagicBall.AloneSetting.Full = g.Alone_Full;
                        dxMagicBall.AloneSetting.EffectDrawBlend = g.Alone_EffectDrawBlend;
                        dxMagicBall.AloneSetting.EffectImageType = g.Alone_EffectImageType;
                        dxMagicBall.AloneSetting.EffectStart = g.Alone_EffectStart;
                        dxMagicBall.AloneSetting.EffectImageCount = g.Alone_EffectImageCount;
                        dxMagicBall.AloneSetting.EffectPlayInterval = g.Alone_EffectPlayInterval;
                    }
                }
                break;

            case TGuiType.t_SexPanel:
                {
                    reader.ReadRecord(TGuiSexPanel.SizeOf, TGuiSexPanel.ReadAt, out var g);
                    var dxSexPanel = (TDxSexPanel)dxControl;
                    dxSexPanel.IsMale = g.IsMale;
                    // 原文如此（LoadDxControl.pas:1416）：记录字段拼写是 UseSettign2（原文笔误），
                    // 控件属性是 UseSetting2。
                    dxSexPanel.UseSetting2 = g.UseSettign2;

                    dxSexPanel.SexImageSetting.ImageType = g.ImageType;
                    dxSexPanel.SexImageSetting.Male = g.Male;
                    dxSexPanel.SexImageSetting.Female = g.Female;

                    dxSexPanel.SexImageSetting2.ImageType = g.ImageType2;
                    dxSexPanel.SexImageSetting2.Male = g.Male2;
                    dxSexPanel.SexImageSetting2.Female = g.Female2;
                }
                break;

            case TGuiType.t_GroupAttackProgress:
                {
                    reader.ReadRecord(TGuiGroupAttackProgress.SizeOf, TGuiGroupAttackProgress.ReadAt, out var g);
                    var dxGroupAttackProgress = (TDxGroupAttackProgress)dxControl;
                    dxGroupAttackProgress.ProgressAlignment = g.ProgressAlignment;

                    dxGroupAttackProgress.ContinueSetting.ImageType = g.Settings[0].ImageType;
                    dxGroupAttackProgress.ContinueSetting.Background = g.Settings[0].Background;
                    dxGroupAttackProgress.ContinueSetting.FlashStart = g.Settings[0].FlashStart;
                    dxGroupAttackProgress.ContinueSetting.FlashEnd = g.Settings[0].FlashEnd;
                    dxGroupAttackProgress.ContinueSetting.FlashInterval = g.Settings[0].FlashInterval;
                    dxGroupAttackProgress.ContinueSetting.BgOffsetX = g.Settings[0].OffsetX1;
                    dxGroupAttackProgress.ContinueSetting.BgOffsetY = g.Settings[0].OffsetY1;
                    dxGroupAttackProgress.ContinueSetting.FlashOffsetX = g.Settings[0].OffsetX2;
                    dxGroupAttackProgress.ContinueSetting.FlashOffsetY = g.Settings[0].OffsetY2;

                    dxGroupAttackProgress.GroupSetting.ImageType = g.Settings[1].ImageType;
                    dxGroupAttackProgress.GroupSetting.Background = g.Settings[1].Background;
                    dxGroupAttackProgress.GroupSetting.Progress = g.Settings[1].Progress;
                    dxGroupAttackProgress.GroupSetting.FlashStart = g.Settings[1].FlashStart;
                    dxGroupAttackProgress.GroupSetting.FlashEnd = g.Settings[1].FlashEnd;
                    dxGroupAttackProgress.GroupSetting.FlashInterval = g.Settings[1].FlashInterval;
                    dxGroupAttackProgress.GroupSetting.BgOffsetX = g.Settings[1].OffsetX1;
                    dxGroupAttackProgress.GroupSetting.BgOffsetY = g.Settings[1].OffsetY1;
                    dxGroupAttackProgress.GroupSetting.PgOffsetX = g.Settings[1].OffsetX2;
                    dxGroupAttackProgress.GroupSetting.PgOffsetY = g.Settings[1].OffsetY2;
                    dxGroupAttackProgress.GroupSetting.ContinueOffsetX = g.Settings[1].OffsetX3;
                    dxGroupAttackProgress.GroupSetting.ContinueOffsetY = g.Settings[1].OffsetY3;

                    dxGroupAttackProgress.ContinueAndGroupSetting.ImageType = g.Settings[2].ImageType;
                    dxGroupAttackProgress.ContinueAndGroupSetting.Background = g.Settings[2].Background;
                    dxGroupAttackProgress.ContinueAndGroupSetting.Progress = g.Settings[2].Progress;
                    dxGroupAttackProgress.ContinueAndGroupSetting.FlashStart = g.Settings[2].FlashStart;
                    dxGroupAttackProgress.ContinueAndGroupSetting.FlashEnd = g.Settings[2].FlashEnd;
                    dxGroupAttackProgress.ContinueAndGroupSetting.FlashInterval = g.Settings[2].FlashInterval;
                    dxGroupAttackProgress.ContinueAndGroupSetting.BgOffsetX = g.Settings[2].OffsetX1;
                    dxGroupAttackProgress.ContinueAndGroupSetting.BgOffsetY = g.Settings[2].OffsetY1;
                    dxGroupAttackProgress.ContinueAndGroupSetting.PgOffsetX = g.Settings[2].OffsetX2;
                    dxGroupAttackProgress.ContinueAndGroupSetting.PgOffsetY = g.Settings[2].OffsetY2;
                    dxGroupAttackProgress.ContinueAndGroupSetting.ContinueOffsetX = g.Settings[2].OffsetX3;
                    dxGroupAttackProgress.ContinueAndGroupSetting.ContinueOffsetY = g.Settings[2].OffsetY3;
                }
                break;

            case TGuiType.t_ImageProgress:
                {
                    reader.ReadRecord(TGuiImageProgress.SizeOf, TGuiImageProgress.ReadAt, out var g);
                    var dxImageProgress = (TDxImageProgress)dxControl;
                    var setting = dxImageProgress.ProgressSetting;
                    dxImageProgress.AutoSize = g.AutoSize;

                    setting.ImageType = g.ImageType;
                    setting.ImageBG = g.ImageBG;
                    setting.ImageProgress = g.ImageProgress;
                    setting.ImageProgressX = g.ImageProgressX;
                    setting.ImageProgressY = g.ImageProgressY;
                    setting.ValueType = g.ValueType;
                    setting.ValueSplite = g.ValueSplite;
                    setting.ValueAlignment = g.ValueAlignment;
                    setting.ValuePrefix = g.ValuePrefix;
                    setting.ValueSuffix = g.ValueSuffix;
                    // 原文如此（LoadDxControl.pas:1485-1487）：记录里是 LongWord，控件属性是 uint；
                    // 逐位同宽，这里显式转换以保持 C# 类型系统可编译。
                    setting.Max = (uint)g.Max;
                    setting.Min = (uint)g.Min;
                    setting.Value = (uint)g.Value;

                    DxGuiFonts.DxFontAssign(setting.Font, g.Font);
                    setting.Font.Name = reader.ReadGuiFontName(g.Font);
                }
                break;

            case TGuiType.t_SwitchButton:
                {
                    reader.ReadRecord(TGuiSwitchButton.SizeOf, TGuiSwitchButton.ReadAt, out var g);
                    var dxSwitchButton = (TDxSwitchButton)dxControl;
                    dxSwitchButton.AutoSize = g.AutoSize;

                    dxSwitchButton.CloseSetting.ImageIndex.ImageType = g.CloseSetting.ImageIndex.Image;
                    dxSwitchButton.CloseSetting.ImageIndex.Up = g.CloseSetting.ImageIndex.Up;
                    dxSwitchButton.CloseSetting.ImageIndex.Hot = g.CloseSetting.ImageIndex.Hot;
                    dxSwitchButton.CloseSetting.ImageIndex.Down = g.CloseSetting.ImageIndex.Down;
                    dxSwitchButton.CloseSetting.ImageIndex.Disabled = g.CloseSetting.ImageIndex.Disabled;

                    DxGuiFonts.DxFontAssign(dxSwitchButton.CloseSetting.CaptionColor.Up, g.CloseSetting.CaptionColor.Up);
                    DxGuiFonts.DxFontAssign(dxSwitchButton.CloseSetting.CaptionColor.Hot, g.CloseSetting.CaptionColor.Hot);
                    DxGuiFonts.DxFontAssign(dxSwitchButton.CloseSetting.CaptionColor.Down, g.CloseSetting.CaptionColor.Down);
                    DxGuiFonts.DxFontAssign(dxSwitchButton.CloseSetting.CaptionColor.Disabled, g.CloseSetting.CaptionColor.Disabled);

                    dxSwitchButton.CloseSetting.ClickSound = g.CloseSetting.ClickSound;
                    dxSwitchButton.CloseSetting.Alignment = g.CloseSetting.Alignment;
                    dxSwitchButton.CloseSetting.CaptionOffsetX = g.CloseSetting.CaptionOffsetX;
                    dxSwitchButton.CloseSetting.CaptionOffsetY = g.CloseSetting.CaptionOffsetY;
                    dxSwitchButton.CloseSetting.CaptionDownOffsetX = g.CloseSetting.CaptionDownOffsetX;
                    dxSwitchButton.CloseSetting.CaptionDownOffsetY = g.CloseSetting.CaptionDownOffsetY;
                    dxSwitchButton.CloseSetting.ButtonDownOffsetX = g.CloseSetting.ButtonDownOffsetX;
                    dxSwitchButton.CloseSetting.ButtonDownOffsetY = g.CloseSetting.ButtonDownOffsetY;
                    dxSwitchButton.CloseSetting.DrawAligment = g.CloseSetting.DrawAligment;

                    dxSwitchButton.OpenSetting.ImageIndex.ImageType = g.OpenSetting.ImageIndex.Image;
                    dxSwitchButton.OpenSetting.ImageIndex.Up = g.OpenSetting.ImageIndex.Up;
                    dxSwitchButton.OpenSetting.ImageIndex.Hot = g.OpenSetting.ImageIndex.Hot;
                    dxSwitchButton.OpenSetting.ImageIndex.Down = g.OpenSetting.ImageIndex.Down;
                    dxSwitchButton.OpenSetting.ImageIndex.Disabled = g.OpenSetting.ImageIndex.Disabled;

                    DxGuiFonts.DxFontAssign(dxSwitchButton.OpenSetting.CaptionColor.Up, g.OpenSetting.CaptionColor.Up);
                    DxGuiFonts.DxFontAssign(dxSwitchButton.OpenSetting.CaptionColor.Hot, g.OpenSetting.CaptionColor.Hot);
                    DxGuiFonts.DxFontAssign(dxSwitchButton.OpenSetting.CaptionColor.Down, g.OpenSetting.CaptionColor.Down);
                    DxGuiFonts.DxFontAssign(dxSwitchButton.OpenSetting.CaptionColor.Disabled, g.OpenSetting.CaptionColor.Disabled);

                    dxSwitchButton.OpenSetting.ClickSound = g.OpenSetting.ClickSound;
                    dxSwitchButton.OpenSetting.Alignment = g.OpenSetting.Alignment;
                    dxSwitchButton.OpenSetting.CaptionOffsetX = g.OpenSetting.CaptionOffsetX;
                    dxSwitchButton.OpenSetting.CaptionOffsetY = g.OpenSetting.CaptionOffsetY;
                    dxSwitchButton.OpenSetting.CaptionDownOffsetX = g.OpenSetting.CaptionDownOffsetX;
                    dxSwitchButton.OpenSetting.CaptionDownOffsetY = g.OpenSetting.CaptionDownOffsetY;
                    dxSwitchButton.OpenSetting.ButtonDownOffsetX = g.OpenSetting.ButtonDownOffsetX;
                    dxSwitchButton.OpenSetting.ButtonDownOffsetY = g.OpenSetting.ButtonDownOffsetY;
                    dxSwitchButton.OpenSetting.DrawAligment = g.OpenSetting.DrawAligment;

                    // 原文如此（LoadDxControl.pas:1545-1555）：标题在两组设置都赋完之后才读，
                    // 顺序为 Close 在前、Open 在后，与记录里 CaptionLen 的存放顺序一致。
                    if (g.CloseSetting.CaptionLen > 0)
                    {
                        sText = reader.ReadFixedString(g.CloseSetting.CaptionLen);
                        dxSwitchButton.CloseSetting.Caption = sText;
                    }

                    if (g.OpenSetting.CaptionLen > 0)
                    {
                        sText = reader.ReadFixedString(g.OpenSetting.CaptionLen);
                        dxSwitchButton.OpenSetting.Caption = sText;
                    }
                }
                break;

            default:
                // 原文如此（LoadDxControl.pas:182 / LoadDxControlEx.pas:180）：case 无 else 分支。
                // t_None 及任何越界枚举值 → 一个字节都不消费，直接返回。
                break;
        }
    }

    /// <summary>原文 LoadDxControl.pas:783/882：<c>ReadMemory(ColRect, SizeOf(TRect))</c>。</summary>
    private static TDxRect ReadRect(byte[] b, int at)
        => TDxRect.Rect(GuiCodec.ReadInt32(b, at + 0), GuiCodec.ReadInt32(b, at + 4),
                        GuiCodec.ReadInt32(b, at + 8), GuiCodec.ReadInt32(b, at + 12));

    private static void AssignAnimation(TDxGuiAnimation target, TGuiAnimation source)
    {
        target.ImageType = source.ImageType;
        target.StartIndex = source.StartIndex;
        target.EndIndex = source.EndIndex;
        target.FrameTime = source.FrameTime;
        target.PlayCount = source.PlayCount;
        target.OffsetX = source.OffsetX;
        target.OffsetY = source.OffsetY;
        target.UseImageOffset = source.UseImageOffset;
        target.OutsideAreaDraw = source.OutsideAreaDraw;
        target.Draw = source.Draw;
        target.BlendDraw = source.BlendDraw;
        target.DrawBeforeDef = source.DrawBeforeDef;
        // 原文如此（LoadDxControl.pas:219-230）：TGuiAnimation 没有 HorzAlignment/VertAlignment/AdjustYByHeight，
        // 只有 TGuiMainBottomFormAnimation 才有；故这三个字段在此不赋值（保持控件默认）。
    }

    private static void AssignMainBottomAnimation(TDxGuiAnimation target, TGuiMainBottomFormAnimation source)
    {
        target.ImageType = source.ImageType;
        target.StartIndex = source.StartIndex;
        target.EndIndex = source.EndIndex;
        target.FrameTime = source.FrameTime;
        target.PlayCount = source.PlayCount;
        target.OffsetX = source.OffsetX;
        target.OffsetY = source.OffsetY;
        target.UseImageOffset = source.UseImageOffset;
        target.OutsideAreaDraw = source.OutsideAreaDraw;
        target.Draw = source.Draw;
        target.BlendDraw = source.BlendDraw;
        target.DrawBeforeDef = source.DrawBeforeDef;
        target.HorzAlignment = source.HorzAlignment;
        target.VertAlignment = source.VertAlignment;
        target.AdjustYByHeight = source.AdjustYByHeight;
    }

    private static void AssignMainBottomCenter(TDxMainBottomCenterSetting target, TGuiMainBottomCenter source)
    {
        target.AutoStretchSize = source.AutoStretchSize;
        target.OffsetLeft = source.OffsetLeft;
        target.OffsetRight = source.OffsetRight;

        target.StretchImage.MinHeight = source.MinHeight;
        target.StretchImage.MaxHeight = source.MaxHeight;
        target.StretchImage.Height = source.Height;

        target.StretchImage.DragHeightOffsetY = source.DragHeightOffsetY;
        target.StretchImage.DragHeightSize = source.DragHeightSize;

        target.StretchImage.FillCenterAlpha = source.StretchImageFillCenterAlpha;
        target.StretchImage.FillCenterColor = source.StretchImageFillCenterColor;
        target.StretchImage.FillCenterExpandHorz = source.StretchImageFillCenterExpandHorz;
        target.StretchImage.FillCenterExpandVert = source.StretchImageFillCenterExpandVert;

        target.StretchImage.ImageType = source.StretchImageType;
        target.StretchImage.UpLeft = source.StretchImageUpLeft;
        target.StretchImage.Up = source.StretchImageUp;
        target.StretchImage.UpRight = source.StretchImageUpRight;
        target.StretchImage.Left = source.StretchImageLeft;
        target.StretchImage.Right = source.StretchImageRight;
        target.StretchImage.DownLeft = source.StretchImageDownLeft;
        target.StretchImage.Down = source.StretchImageDown;
        target.StretchImage.DownRight = source.StretchImageDownRight;

        target.FillImage.ImageType = source.FillImageType;
        target.FillImage.Index = source.FillImageIndex;
    }
}
