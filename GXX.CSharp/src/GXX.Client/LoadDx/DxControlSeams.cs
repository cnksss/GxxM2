using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GXX.Client.DxComponent;

// Delphi 的 TAlignment 在既有接缝里叫 TDxAlignment（DxComponents.pas 的 TAlignment 与 VCL 同序）。
using TAlignment = GXX.Client.DxComponent.TDxAlignment;
// 既有接缝把 TClickSound 声明成 TDxImageButton 的嵌套枚举（DxLabel.cs:22），故此处跟随。
using TClickSound = GXX.Client.DxComponent.TDxImageButton.TClickSound;

namespace GXX.Client.LoadDx;

// =====================================================================================
// DxComponent 控件族接缝。
//
// 【为什么是接缝而不是移植】
//   LoadDxControl*.pas 的 LoadComponent 需要 23 种 TDx* 控件作为赋值目标。
//   仓库现状（P1 车道5 已合并）只提供了 5 种：
//       TDxControl（基类）/ TDxImageButton / TDxLabel / TDxLine / TDxImageProgress / TDXTrackBar
//   （见 src/GXX.Client/DxComponent/*.cs）。其余 18 种本文件按"最小可用面"定义：
//   只声明 LoadComponent 真的会写的成员，**不做绘制/命中测试**，并在每处标注
//   // 接缝：待 <单元名> 移植后接入
//
// 【为什么不另造控件】
//   所有接缝类一律继承 GXX.Client.DxComponent.TDxControl，复用其
//   DxOwner / ImageIndex / BorderColor / CaptionColor(DrawCaptionFont) / 位置语义。
//   对照台账 §9.3：同一 Delphi 单元只能由一条车道负责接缝 —— 本文件**不重新定义**
//   TDxControl / TDxFont / TDxCaptionColor / TDxImageIndex / TDxImageButton / TDxLabel /
//   TDxLine / TDxImageProgress / TDXTrackBar，一律引用既有实现。
//
// 【差异落点】既有控件缺少的成员（如 TDxImageButton.Animation）挂在本文件的
//   TDxControlExtra 附带对象上（ConditionalWeakTable），不修改既有文件。
// =====================================================================================

/// <summary>
/// DxComponent 既有控件上**不存在**、但 LoadComponent 需要写入的成员的落点。
/// 键是被创建的控件实例；原文这些成员就在各控件类里，故此处只是"接缝寄存"。
/// </summary>
public sealed class TDxControlExtra
{
    /// <summary>原文 TDxControl.GuiType（DxControls.pas）。既有 TDxControl 基类未定义 → 寄存于此。</summary>
    public TGuiType GuiType = TGuiType.t_None;

    /// <summary>接缝：待 DxImageButton.pas 移植后接入（原文 TDxImageButton.Animation:TDxButtonAnimation）。</summary>
    public TGuiButtonAnimation Animation;

    /// <summary>原文 TDxControl.OnGetImage（LoadDxControl.pas:1611 的赋值目标）。</summary>
    public Action<TDxImageIndex, TImageType> OnGetImage;
}

/// <summary>TDxControlExtra 的挂号表（弱键，不阻止控件被回收）。</summary>
public static class TDxControlExtras
{
    private static readonly ConditionalWeakTable<TDxControl, TDxControlExtra> Table = new();

    public static TDxControlExtra Of(TDxControl control)
    {
        if (control == null) return null;
        return Table.GetValue(control, _ => new TDxControlExtra());
    }

    /// <summary>对应原文 <c>DxControl.GuiType := GuiHeader.Gui</c>（LoadDxControl.pas:1593）。</summary>
    public static void SetGuiType(TDxControl control, TGuiType gui)
    {
        Of(control).GuiType = gui;
        // 既有 TDxImageButton/TDxLabel 自带同名属性，一并同步，避免两处取值不一致。
        if (control is TDxImageButton button) button.GuiType = gui;
    }

    /// <summary>对应原文 <c>DxControl.GuiType</c> 读取。</summary>
    public static TGuiType GetGuiType(TDxControl control) => Of(control)?.GuiType ?? TGuiType.t_None;
}

// -------------------------------------------------------------------------------------
// 通用子结构
// -------------------------------------------------------------------------------------

/// <summary>接缝：待 DxControls.pas 移植后接入。<c>TDxControl.PopupMenu</c> 是 TDxPopupMenu，
/// 既有 TDxComboBox 未移植 → 这里作为 ComboBox 的内嵌弹出菜单承载面。</summary>
public sealed class TDxGuiStrings
{
    /// <summary>对应 TStrings.Text（Delphi 的 TStrings.Text 以换行连接各项）。</summary>
    public string Text = string.Empty;
}

/// <summary>Delphi DxControls.pas 的 TDxImageInfo 级别接缝（TDxImageFormShape.Items[] 元素）。</summary>
public sealed class TDxFormShapeItem
{
    public TImageType ImageType;
    public int ImageIndex;
    public TAlignEx Align;
    public bool Draw;
    public bool Stretch;
    public bool Center;
    public int BlendMode;
    public TDxRect SourceRect;
    public TDxRect DestRect;
}

/// <summary>Delphi DxControls.pas 的 TDxViewField（TDxListView.Fields[] 元素）。</summary>
public sealed class TDxListViewField
{
    public TDxCaptionColor Color = new TDxCaptionColor();
    public TDxAlignment Alignment;
    public string Caption = string.Empty;
}

/// <summary>接缝：待 DxImageProgress.pas 已移植 → 复用 TProgressSetting，此处仅给动画子块。</summary>
public sealed class TDxGuiAnimation
{
    public TImageType ImageType;
    public int StartIndex;
    public int EndIndex;
    public int FrameTime;
    public int PlayCount;
    public int OffsetX;
    public int OffsetY;
    public bool UseImageOffset;
    public bool OutsideAreaDraw;
    public bool Draw;
    public bool BlendDraw;
    public bool DrawBeforeDef;
    public TAlignment HorzAlignment;
    public TVerticalAlignment VertAlignment;
    public bool AdjustYByHeight;
}

// -------------------------------------------------------------------------------------
// 12 个待移植控件的最小接缝
// -------------------------------------------------------------------------------------

/// <summary>接缝：待 DxImageForm.pas 移植后接入（原文 TDxImageForm:TDxControl）。</summary>
public class TDxImageForm : TDxControl
{
    public int BackgroundAlpha;
    public TDxGuiAnimation Animation1 = new TDxGuiAnimation();
    public TDxGuiAnimation Animation2 = new TDxGuiAnimation();
    public TDxGuiAnimation Animation3 = new TDxGuiAnimation();
}

/// <summary>接缝：待 DxImageForm.pas 移植后接入（原文 TDxImageFormShape:TDxImageForm）。</summary>
public sealed class TDxImageFormShape : TDxImageForm
{
    /// <summary>原文 ImageCount 恒为 9（TGuiFormShapeInfoArray = array[0..8-1]）。</summary>
    public int ImageCount = 8;

    public readonly TDxFormShapeItem[] Items = CreateItems();

    private static TDxFormShapeItem[] CreateItems()
    {
        var items = new TDxFormShapeItem[8];
        for (int i = 0; i < items.Length; i++) items[i] = new TDxFormShapeItem();
        return items;
    }
}

/// <summary>接缝：待 DxEdit.pas 移植后接入（原文 TDxEdit:TDxControl）。</summary>
public class TDxEdit : TDxControl
{
    public TDxFont Font = new TDxFont();
    public int SelectedColor;
    public int SelBackColor;
    public int SelFontColor;
    public TInValue InValue;
    public byte PasswordChar;
    public bool AllowSelect;
    public bool AllowPaste;
    public int MaxLength;
    public bool ReadOnly;
    public string Text = string.Empty;

    public TDxEdit() { TabOrder = 0; }
}

/// <summary>接缝：待 DxImageEdit.pas 移植后接入（原文 TDxImageEdit:TDxEdit）。</summary>
public sealed class TDxImageEdit : TDxEdit
{
    public byte BackgroundColorAlpha;
    public TDxGuiBackgroundImage BackgroundImage = new TDxGuiBackgroundImage();
    public bool DisableHideCtrl;
    public bool DisableBackgroundTransparent;
    public int DisableBackgroundColor;
    public int DisableBackgroundAlpha;
    public TDxGuiBackgroundImage DisableBackgroundImage = new TDxGuiBackgroundImage();
    public TDxFont HintTextFont = new TDxFont();
    public TAlignment HintTextAlignment;
    public string HintText = string.Empty;
}

/// <summary>接缝：待 DxImageEdit.pas 移植后接入（原文 TBackgroundImage）。</summary>
public sealed class TDxGuiBackgroundImage
{
    public TImageType ImageType;
    public bool BlendDraw;
    public bool OutsideAreaDraw;
    public int ImageIndex;
    public int OffsetX;
    public int OffsetY;
}

/// <summary>接缝：待 DxImageGrid.pas 移植后接入（原文 TDxImageGrid:TDxControl）。</summary>
public sealed class TDxImageGrid : TDxControl
{
    public int ColCount;
    public int RowCount;
    public int ColWidth;
    public int RowHeight;
    public int ViewTopLine;
}

/// <summary>接缝：待 DxMemo.pas 移植后接入（原文 TDxScrollControl:TDxControl，TDxScrollBox/TDxChatMemo/TDxListView/TDxTreeView 皆其子类）。</summary>
public class TDxScrollControl : TDxControl
{
    public TDxImageIndex ScrollImageIndex = new TDxImageIndex();
    public TDxImageIndex PrevImageIndex = new TDxImageIndex();
    public TDxImageIndex NextImageIndex = new TDxImageIndex();
    public TDxImageIndex BarImageIndex = new TDxImageIndex();

    public bool ShowScroll;
    public int ItemHeight;
    public int ItemIndex;
    public TScrollStyle ScrollBars;
    public int ScrollSize;
    public int ExpandSize;
    public int Position;
    public int VisibleItemCount;
    public int OffSetX;
    public int OffSetY;
    public int ShowItemCount;
}

/// <summary>接缝：待 DxMemo.pas 移植后接入（原文 TDxScrollBox:TDxScrollControl）。</summary>
public sealed class TDxScrollBox : TDxScrollControl { }

/// <summary>接缝：待 DxMemo.pas 移植后接入（原文 TDxChatMemo:TDxScrollControl）。</summary>
public sealed class TDxChatMemo : TDxScrollControl
{
    public string FontName = string.Empty;
    public int FontSize;
    public bool FontStroke;
    public bool FontBackTransparent;
}

/// <summary>接缝：待 DxMemo.pas / DxListView.pas 移植后接入（原文 TDxListView:TDxScrollControl）。</summary>
public sealed class TDxListView : TDxScrollControl
{
    public int ColCount;
    public bool ShowGridLine;
    public int GridLineColor;
    public bool CheckItemControlSize;

    /// <summary>原文 ColRects:array of TRect（SaveComponent 里按 ColCount 逐条写出）。</summary>
    public TDxRect[] ColRects = new TDxRect[64];

    /// <summary>原文 Fields:array of TDxViewField。</summary>
    public TDxListViewField[] Fields = CreateFields();

    private static TDxListViewField[] CreateFields()
    {
        var f = new TDxListViewField[64];
        for (int i = 0; i < f.Length; i++) f[i] = new TDxListViewField();
        return f;
    }
}

/// <summary>接缝：待 DxMemo.pas 移植后接入（原文 TDxTreeView:TDxScrollControl）。</summary>
public sealed class TDxTreeView : TDxScrollControl
{
    public bool ShowButton;
}

/// <summary>接缝：待 DxPopupMenu.pas 移植后接入（原文 TDxPopupMenu:TDxControl）。</summary>
public sealed class TDxPopupMenu : TDxControl
{
    public TDxCaptionColor ItemColor = new TDxCaptionColor();
    public int SelectColor;
    public int ItemHeight;
    public int ItemIndex;
    public TDxGuiStrings Items = new TDxGuiStrings();
}

/// <summary>接缝：待 DxComboBox.pas 移植后接入（原文 TDxComboBox:TDxControl）。</summary>
public sealed class TDxComboBox : TDxControl
{
    public TDxPopupMenu PopupMenu = new TDxPopupMenu();
    public TDxCaptionColor TextColor = new TDxCaptionColor();
    public int ButtonColor;
    public string Text = string.Empty;
    public TDxGuiStrings Items = new TDxGuiStrings();
}

/// <summary>接缝：待 DxPageControl.pas 移植后接入（原文 TDxPageControl:TDxControl）。</summary>
public class TDxPageControl : TDxControl
{
    public int ClientLeft;
    public int ClientTop;
    public int ClientWidth;
    public int ClientHeight;
    public TTabPosition TabPosition;
    public int ButtonWidth;
    public int ButtonHeight;
    public bool ShowButton;
    public int OffSetX;
    public int OffSetY;
    public int CaptionOffsetX;
    public int CaptionOffsetY;
    public int DownCaptionOffsetX;
    public int DownCaptionOffsetY;
    public bool ReverseDrawButton;
    public int ActivePageIndex;
    public int PageCount;
}

/// <summary>接缝：待 DxPageControl.pas 移植后接入（原文 TDxTabSheet:TDxControl）。</summary>
public sealed class TDxTabSheet : TDxControl
{
    public int OffSetX;
    public int OffSetY;
    public bool TabVisible;
    public TDxCaptionColor CaptionColor = new TDxCaptionColor();

    /// <summary>
    /// 原文 LoadDxControl.pas:1072 用 <c>TDxPageControl(DxTabSheet.Owner).ActivePageIndex := 0</c>。
    /// 既有 TDxControl.DxOwner 是同一语义落点（原文 TComponent.Owner）。
    /// </summary>
    public TDxPageControl Owner => DxOwner as TDxPageControl;
}

/// <summary>接缝：待 DxMainBottomForm.pas 移植后接入。</summary>
public sealed class TDxMainBottomImage
{
    public TImageType ImageType;
    public int Index;
}

/// <summary>接缝：待 DxMainBottomForm.pas 移植后接入（原文 TDxMainBottomForm.CenterSetting）。</summary>
public sealed class TDxMainBottomCenterSetting
{
    public int OffsetLeft;
    public int OffsetRight;
    public bool AutoStretchSize;
    public TDxMainBottomStretchImage StretchImage = new TDxMainBottomStretchImage();
    public TDxMainBottomImage FillImage = new TDxMainBottomImage();
}

/// <summary>接缝：待 DxMainBottomForm.pas 移植后接入（原文 CenterSetting.StretchImage）。</summary>
public sealed class TDxMainBottomStretchImage
{
    public int MinHeight;
    public int MaxHeight;
    public int Height;
    public int DragHeightOffsetY;
    public int DragHeightSize;
    public byte FillCenterAlpha;
    public int FillCenterColor;
    public int FillCenterExpandHorz;
    public int FillCenterExpandVert;
    public TImageType ImageType;
    public int UpLeft;
    public int Up;
    public int UpRight;
    public int Left;
    public int Right;
    public int DownLeft;
    public int Down;
    public int DownRight;
}

/// <summary>接缝：待 DxMainBottomForm.pas 移植后接入（原文 TDxMainBottomForm:TDxControl）。</summary>
public sealed class TDxMainBottomForm : TDxControl
{
    public TDxMainBottomImage LeftImage = new TDxMainBottomImage();
    public TDxMainBottomImage RightImage = new TDxMainBottomImage();
    public TDxMainBottomImage BottomImage = new TDxMainBottomImage();
    public TDxMainBottomCenterSetting CenterSetting = new TDxMainBottomCenterSetting();
    public TDxGuiAnimation Animation1 = new TDxGuiAnimation();
    public TDxGuiAnimation Animation2 = new TDxGuiAnimation();
    public TDxGuiAnimation Animation3 = new TDxGuiAnimation();
    public TDxGuiAnimation Animation4 = new TDxGuiAnimation();
}

/// <summary>接缝：待 DxMagicBall.pas 移植后接入（原文 TDxMagicBall.OverallSetting）。</summary>
public sealed class TDxMagicBallOverallSetting
{
    public TImageType ImageType;
    public int EmptyHPMP;
    public int FullHPMP;
    public int EmptyHP;
    public int FullHP;
    public int Splite;
    public int MiddleZoneWidth;
    public bool EffectDrawBlend;
    public TImageType EffectImageType;
    public int EffectHPMPStart;
    public int EffectHPStart;
    public int EffectImageCount;
    public int EffectPlayInterval;
}

/// <summary>接缝：待 DxMagicBall.pas 移植后接入（原文 TDxMagicBall.AloneSetting）。</summary>
public sealed class TDxMagicBallAloneSetting
{
    public TImageType ImageType;
    public int Empty;
    public int Full;
    public bool EffectDrawBlend;
    public TImageType EffectImageType;
    public int EffectStart;
    public int EffectImageCount;
    public int EffectPlayInterval;
}

/// <summary>接缝：待 DxMagicBall.pas 移植后接入（原文 TDxMagicBall:TDxControl）。</summary>
public sealed class TDxMagicBall : TDxControl
{
    public TMagicBallType BallType;
    public TMagicBallValueAlignment ValueAlignment;
    public TDxMagicBallOverallSetting OverallSetting = new TDxMagicBallOverallSetting();
    public TDxMagicBallAloneSetting AloneSetting = new TDxMagicBallAloneSetting();
}

/// <summary>接缝：待 DxSexPanel.pas 移植后接入（原文 TDxSexPanel.SexImageSetting）。</summary>
public sealed class TDxSexImageSetting
{
    public TImageType ImageType;
    public int Male;
    public int Female;
}

/// <summary>接缝：待 DxSexPanel.pas 移植后接入（原文 TDxSexPanel:TDxControl）。</summary>
public sealed class TDxSexPanel : TDxControl
{
    public bool IsMale;
    public bool UseSetting2;
    public TDxSexImageSetting SexImageSetting = new TDxSexImageSetting();
    public TDxSexImageSetting SexImageSetting2 = new TDxSexImageSetting();
}

/// <summary>接缝：待 DxGroupAttackProgress.pas 移植后接入（原文 ContinueSetting/GroupSetting/ContinueAndGroupSetting）。</summary>
public sealed class TDxGroupAttackProgressSetting
{
    public TImageType ImageType;
    public int Background;
    public int Progress;
    public int FlashStart;
    public int FlashEnd;
    public int FlashInterval;
    public int BgOffsetX;
    public int BgOffsetY;
    public int PgOffsetX;
    public int PgOffsetY;
    public int ContinueOffsetX;
    public int ContinueOffsetY;
    public int FlashOffsetX;
    public int FlashOffsetY;
}

/// <summary>接缝：待 DxGroupAttackProgress.pas 移植后接入（原文 TDxGroupAttackProgress:TDxControl）。</summary>
public sealed class TDxGroupAttackProgress : TDxControl
{
    public TMagicBallValueAlignment ProgressAlignment;
    public TDxGroupAttackProgressSetting ContinueSetting = new TDxGroupAttackProgressSetting();
    public TDxGroupAttackProgressSetting GroupSetting = new TDxGroupAttackProgressSetting();
    public TDxGroupAttackProgressSetting ContinueAndGroupSetting = new TDxGroupAttackProgressSetting();
}

/// <summary>接缝：待 DxSwitchButton.pas 移植后接入（原文 TDxSwitchButton.CloseSetting/OpenSetting）。</summary>
public sealed class TDxSwitchButtonSetting
{
    public TDxImageIndex ImageIndex = new TDxImageIndex();
    public TDxCaptionColor CaptionColor = new TDxCaptionColor();
    public TClickSound ClickSound;
    public TAlignment Alignment;
    public int CaptionOffsetX;
    public int CaptionOffsetY;
    public int CaptionDownOffsetX;
    public int CaptionDownOffsetY;
    public int ButtonDownOffsetX;
    public int ButtonDownOffsetY;
    public TDrawAligment DrawAligment;
    public string Caption = string.Empty;
}

/// <summary>接缝：待 DxSwitchButton.pas 移植后接入（原文 TDxSwitchButton:TDxControl）。</summary>
public sealed class TDxSwitchButton : TDxControl
{
    public TDxSwitchButtonSetting CloseSetting = new TDxSwitchButtonSetting();
    public TDxSwitchButtonSetting OpenSetting = new TDxSwitchButtonSetting();
}

// -------------------------------------------------------------------------------------
// 名字表（LoadDxControlEx 的 ControlAddrList:THashedStringList）
// -------------------------------------------------------------------------------------

/// <summary>
/// 对应 <c>^TDxControl</c>（Delphi 的"控件指针的指针"）。
/// 原文 LoadSubComponent 里 <c>pControlAddr^ := DxControl</c> 就是往这个槽里写。
/// </summary>
public sealed class TDxControlRef
{
    public TDxControl Value;
    public TDxControlRef(TDxControl value = null) => Value = value;
    public static implicit operator TDxControlRef(TDxControl c) => new TDxControlRef(c);
}

/// <summary>
/// 对应 <c>THashedStringList</c> 的最小接缝（Delphi 侧来自 HashList/MShare）。
/// 只保留 LoadDxControlEx 用到的 IndexOf / Objects[] 语义。
/// <para>接缝：待 HashList.pas 移植后接入（若 GXX.Core 已提供同名类型则改为引用）。</para>
/// </summary>
public sealed class THashedStringList
{
    private readonly List<string> _keys = new();

    public int Count => _keys.Count;

    public string this[int index] => _keys[index];

    /// <summary>
    /// 对应 TStringList.Objects[]（原文是属性数组）。这里用 List&lt;object&gt; 表达，
    /// 于是 <c>ControlAddrList.Objects[n]</c> 与 Delphi 写法逐字一致；
    /// LoadDxControlEx 往里面存的是 <see cref="TDxControlRef"/>。
    /// </summary>
    public List<object> Objects { get; } = new();

    public int AddObject(string name, object value)
    {
        _keys.Add(name);
        Objects.Add(value);
        return _keys.Count - 1;
    }

    /// <summary>对应 THashedStringList.IndexOf：区分大小写，未命中返回 -1。</summary>
    public int IndexOf(string name) => _keys.IndexOf(name);

    /// <summary>测试/构建用：把一个控件指针槽登记进表（相当于原文 objRootControl 的 RTTI 字段表）。</summary>
    public TDxControlRef Register(string name)
    {
        var reference = new TDxControlRef();
        AddObject(name, reference);
        return reference;
    }
}

/// <summary>
/// 对应 LoadDxControl.pas:53 的 <c>PControlAddress:Pointer</c> 及其
/// <c>TDxControl(PControlAddress^) := DxControl; Inc(PInteger(PControlAddress));</c> 用法
/// （LoadDxControl.pas:1594-1596）。
/// </summary>
public sealed class TDxControlAddressList
{
    private readonly TDxControl[] _slots;
    private int _index;

    public TDxControlAddressList(TDxControl[] slots)
    {
        _slots = slots ?? Array.Empty<TDxControl>();
    }

    /// <summary>当前写入位置（对应 PControlAddress 的字节偏移 / SizeOf(Pointer)）。</summary>
    public int Index => _index;

    /// <summary>
    /// 原文如此（LoadDxControl.pas:1594）：<c>TDxControl(PControlAddress^) := DxControl; Inc(PInteger(PControlAddress));</c>
    /// —— 原文不判越界，写出数组即内存越界；托管侧越界则丢弃（返回 false）并在测试中锁定该差异。
    /// </summary>
    public bool Store(TDxControl control)
    {
        if (_index < 0 || _index >= _slots.Length)
        {
            _index++;
            return false;
        }
        _slots[_index++] = control;
        return true;
    }
}
