using System;

namespace GXX.Client.GUI.DxComponent;

// ============================================================================================
// 【接缝】DxControls/DxImageButton/DxImageForm/DxPageControl/DxLabel 控件族的托管最小面。
//
// 原实现（Source/Client-HGE/DxComponent/*.pas，DxControls.pas 117KB）以 HGE 自绘，控件不是
// WinForms 控件（TDxControl 直继承 TComponent，见 DxControls.pas:152 的 {$IF CLIENTEXE = 1}
// 分支）。本车道只移植 GUI/Mir 下 5 个窗口，故此处只复刻这些窗口直接触及的成员：
//   Left/Top/Width/Height/ClientRect/VirtualRect（DxControls.pas SetPosition/GetPosition 语义）
//   Visible/Enabled/BringToFront/Designing/Floating/Tag/CaptionColor/Caption
//   ImageIndex（TDxImageIndex：ImageType/Up/Hot/Down/Disabled）
//   OnClick / OnGetImage / OnPaint / OnClickSound / ClickCount / OnMouseMove
// 其余成员待 DxComponent 单元（另一车道/后续批次）移植后接入。
// ============================================================================================

/// <summary>DxComponents.pas TOnClickEx：procedure(Sender:TObject; X, Y:Integer)。</summary>
public delegate void TOnClickEx(object sender, int x, int y);

/// <summary>DxComponents.pas TOnClickSound：procedure(Sender:TObject; ClickSound:TClickSound)。</summary>
public delegate void TOnClickSound(object sender, TClickSound clickSound);

/// <summary>DxComponents.pas TOnGetImage：procedure(Sender:TObject; ImageType:TImageType; var AImage:TObject)。</summary>
public delegate void TOnGetImage(object sender, TImageType imageType, ref object image);
/// <summary>DxControls.pas TDxFont（Caption 用的字色/字体）：本车道只读 Color。</summary>
public sealed class TDxFont
{
    /// <summary>Delphi TColor（0x00BBGGRR 32 位值）。</summary>
    public int Value;
}

/// <summary>DxControls.pas TDxCaptionColor（Up/Hot/Down/Checked/Disabled 五态字色）。</summary>
public sealed class TDxCaptionColor
{
    public TDxFont Up { get; } = new();
    public TDxFont Hot { get; } = new();
    public TDxFont Down { get; } = new();
    public TDxFont Checked { get; } = new();
    public TDxFont Disabled { get; } = new();
}

/// <summary>
/// DxControls.pas TDxImageIndex：图库 + 四态图号。默认值照抄 TDxImageIndex.Create
/// （Prguse_wil / Up=-1 / Down=-1 / Hot=-1 / Disabled=-1）。
/// </summary>
public sealed class TDxImageIndex
{
    public TImageType ImageType { get; set; } = TImageType.Prguse_wil;
    public int Up { get; set; } = -1;
    public int Hot { get; set; } = -1;
    public int Down { get; set; } = -1;
    public int Checked { get; set; } = -1;
    public int Disabled { get; set; } = -1;

    /// <summary>DxControls.pas TDxImageIndex.Assign(Source:TPersistent)：整体拷贝图库与五态图号。</summary>
    public void Assign(TDxImageIndex Source)
    {
        ImageType = Source.ImageType;
        Up = Source.Up;
        Hot = Source.Hot;
        Down = Source.Down;
        Checked = Source.Checked;
        Disabled = Source.Disabled;
    }
}

/// <summary>
/// DxControls.pas:152 TDxControl 的最小托管等价（几何按 FClientRect 存储，与原实现一致：
/// Width/Height 由 Right-Left/Bottom-Top 派生，SetPosition 保持对角不动）。
/// </summary>
public class TDxControl
{
    private TRect _clientRect;

    /// <summary>DxControls.pas 构造：Owner 记录父控件（本车道只用于新建子控件）。</summary>
    public TDxControl(TDxControl owner = null)
    {
        Owner = owner;
        Name = string.Empty;
        Caption = string.Empty;
    }

    public TDxControl Owner { get; internal set; }

    /// <summary>DxControls.pas TDxControl.ClientRect（FClientRect）。</summary>
    public TRect ClientRect
    {
        get => _clientRect;
        set { _clientRect = value; Repaint(); OnResize?.Invoke(this); }
    }

    /// <summary>DxControls.pas GetVirtualRect（无父控件裁剪时即 ClientRect）。</summary>
    public TRect VirtualRect => _clientRect;

    /// <summary>DxControls.pas GetVisibleRect。</summary>
    public TRect VisibleRect => _clientRect;

    public int Left
    {
        get => _clientRect.Left;
        set
        {
            int aux = _clientRect.Right - _clientRect.Left;
            _clientRect.Left = value;
            _clientRect.Right = value + aux;
            Repaint();
        }
    }

    public int Top
    {
        get => _clientRect.Top;
        set
        {
            int aux = _clientRect.Bottom - _clientRect.Top;
            _clientRect.Top = value;
            _clientRect.Bottom = value + aux;
            Repaint();
        }
    }

    public int Width
    {
        get => _clientRect.Right - _clientRect.Left;
        set { _clientRect.Right = _clientRect.Left + value; Repaint(); }
    }

    public int Height
    {
        get => _clientRect.Bottom - _clientRect.Top;
        set { _clientRect.Bottom = _clientRect.Top + value; Repaint(); }
    }

    public bool Visible { get; set; } = true;
    public bool Enabled { get; set; } = true;
    public bool Designing { get; set; } = true;
    public bool Floating { get; set; }
    public int Tag { get; set; }
    public int TabOrder { get; set; }
    public string Name { get; set; }
    public string Caption { get; set; }
    public string Hint { get; set; } = string.Empty;
    public object Data { get; set; }

    public TDxControl PopupMenu { get; set; }

    /// <summary>DxControls.pas TDxControl.ImageIndex。</summary>
    public TDxImageIndex ImageIndex { get; } = new();

    /// <summary>DxControls.pas TDxControl.CaptionColor（TDxImageButton/TDxLabel 用于变色）。</summary>
    public TDxCaptionColor CaptionColor { get; } = new();

    // ---- 事件（DxControls.pas published property，均为 procedure of object） ----
    public TOnClickEx OnClick { get; set; }
    public TOnClickEx OnDblClick { get; set; }
    public TOnClickSound OnClickSound { get; set; }
    public Action<object> OnPaint { get; set; }
    public Action<object> OnShow { get; set; }
    public Action<object> OnHide { get; set; }
    public Action<object> OnResize { get; set; }
    public Action<object> OnMouseMoveEx { get; set; }
    public TOnGetImage OnGetImage { get; set; }

    /// <summary>DxControls.pas TDxControl.Repaint（本通道无绘制后端，仅占位保持调用点结构）。</summary>
    public virtual void Repaint() { }

    /// <summary>DxControls.pas TDxControl.BringToFront（Z 序调整；本通道无绘制后端，占位）。</summary>
    public virtual void BringToFront() { }

    /// <summary>DxControls.pas TDxControl.SentToBack。</summary>
    public virtual void SentToBack() { }

    /// <summary>DxControls.pas TDxControl.VisibleRect/VirtualRect 之外的 MoveBy。</summary>
    public void MoveBy(int dx, int dy)
    {
        _clientRect.Left += dx; _clientRect.Right += dx;
        _clientRect.Top += dy; _clientRect.Bottom += dy;
    }
}

/// <summary>DxControls.pas TDxImageButton（本车道只用 ImageIndex/OnClick/OnGetImage/OnClickSound/ClickCount）。</summary>
public class TDxImageButton : TDxControl
{
    public TDxImageButton(TDxControl owner = null) : base(owner) { }

    /// <summary>DxImageButton.pas TDxImageButton.ClickCount:TClickSound。</summary>
    public TClickSound ClickCount { get; set; } = TClickSound.csNone;

    /// <summary>DxImageButton.pas TDxImageButton.Checked。</summary>
    public bool Checked { get; set; }
}

/// <summary>DxLabel.pas TDxLabel : TDxImageButton（Caption + CaptionColor）。</summary>
public class TDxLabel : TDxImageButton
{
    public TDxLabel(TDxControl owner = null) : base(owner) { }
}

/// <summary>DxEdit.pas TDxEdit（本车道只用几何/Visible）。</summary>
public class TDxEdit : TDxControl
{
    public TDxEdit(TDxControl owner = null) : base(owner) { }

    public string Text { get; set; } = string.Empty;
    public int MaxLength { get; set; }
}

/// <summary>DxImageGrid.pas TDxImageGrid（本车道只用几何）。</summary>
public class TDxImageGrid : TDxControl
{
    public TDxImageGrid(TDxControl owner = null) : base(owner) { }
}

/// <summary>DxMemo.pas TDxScrollBox 族的公有基（本车道只用 Width/Height）。</summary>
public class TDxScrollBox : TDxControl
{
    public TDxScrollBox(TDxControl owner = null) : base(owner) { }
}

/// <summary>DxPageControl.pas TDxTabSheet（TabVisible 控制页签可见性）。</summary>
public class TDxTabSheet : TDxControl
{
    public TDxTabSheet(TDxControl owner = null) : base(owner) { }

    public bool TabVisible { get; set; } = true;
}

/// <summary>DxPageControl.pas TDxPageControl（本车道只用 ActivePageIndex）。</summary>
public class TDxPageControl : TDxControl
{
    public TDxPageControl(TDxControl owner = null) : base(owner) { }

    public int ActivePageIndex { get; set; } = -1;
}

/// <summary>DxImageForm.pas TDxImageForm（本车道只用几何/Visible/Floating/OnPaint/BringToFront/ImageIndex）。</summary>
public class TDxImageForm : TDxControl
{
    public TDxImageForm(TDxControl owner = null) : base(owner) { }

    /// <summary>DxImageForm.pas TDxImageForm.ImageType（单图形态用于 DLogin/DSelectChr 等）。</summary>
    public TImageType ImageType { get; set; } = TImageType.Prguse_wil;

    /// <summary>DxImageForm.pas TDxImageForm.ImageIndex:Integer（单图形态图号）。</summary>
    public int ImageIndexValue { get; set; } = -1;
}

/// <summary>
/// DxSexPanel.pas TDxSexPanel : TDxImageForm（人物形象面板）。
/// 本车道只用到 IsMale（性别图片组选择）与 UseSetting2（1.76 传统样式设定）。
/// </summary>
public class TDxSexPanel : TDxImageForm
{
    public TDxSexPanel(TDxControl owner = null) : base(owner) { }

    /// <summary>DxSexPanel.pas TDxSexPanel.IsMale：true=男（图片组 0），false=女。</summary>
    public bool IsMale { get; set; }

    /// <summary>DxSexPanel.pas TDxSexPanel.UseSetting2：使用第 2 套布局设定。</summary>
    public bool UseSetting2 { get; set; }
}

/// <summary>DxComboBox.pas TDxComboBox（本车道只用几何）。</summary>
public class TDxComboBox : TDxControl
{
    public TDxComboBox(TDxControl owner = null) : base(owner) { }
}

/// <summary>DxPopupMenu.pas TDxPopupMenu（本车道只用几何）。</summary>
public class TDxPopupMenu : TDxControl
{
    public TDxPopupMenu(TDxControl owner = null) : base(owner) { }
}

/// <summary>DxMemo.pas TDxMemo（聊天框基类：Prev/Next/Bar/Scroll 四组 ImageIndex）。</summary>
public class TDxMemo : TDxControl
{
    public TDxMemo(TDxControl owner = null) : base(owner) { }

    public TDxImageIndex PrevImageIndex { get; } = new();
    public TDxImageIndex NextImageIndex { get; } = new();
    public TDxImageIndex BarImageIndex { get; } = new();
    public TDxImageIndex ScrollImageIndex { get; } = new();
}

/// <summary>SerialWindowsDlg.pas DServerDlg:TDxImageForm（选服对话框，续章版设图号）。</summary>
public class TDxServerDlg : TDxImageForm
{
    public TDxServerDlg(TDxControl owner = null) : base(owner) { }
}
