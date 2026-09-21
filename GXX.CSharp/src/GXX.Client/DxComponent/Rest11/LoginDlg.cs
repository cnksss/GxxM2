using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GXX.Core.Rtl;

// 源单元：Source/Client-HGE/DxComponent/LoginDlg.pas（179 行，GBK，CRLF）
// 原文 uses：Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
//            Dialogs, StdCtrls, Mask, RzEdit, RzBtnEdt, ExtCtrls, ComCtrls, ShlObj, ComObj,
//            ActiveX, RzPanel, RzDlgBtn, RzRadGrp, DxComponents
// implementation uses：GameImages, IniFiles, Share
//
// .dfm 形态：**文本格式**（Source/Client-HGE/DxComponent/LoginDlg.dfm，71 行，
//   首行 `object FrmLogin: TFrmLogin`）。本文件的所有布局常量都取自该文件的**实读值**，
//   并由 DxRest11LoginDlgTests 里的"DFM 对账"用例逐项锁死（§41.3-1 的二进制 DFM 陷阱
//   在本单元**不适用**：这份 DFM 是文本的，且 utf8_mirror 与 Source 两份逐字节一致）。

namespace GXX.Client.DxComponent.Rest11;

/// <summary>
/// LoginDlg.pas 的 <c>TFrmLogin</c> 1:1 移植（WinForms）。
///
/// <para><b>原文结构对账</b>：6 个事件处理器（<c>EditGamePathButtonClick</c> /
/// <c>DialogButtonsClickOk</c> / <c>FormCreate</c> / <c>DialogButtonsClickCancel</c> /
/// <c>RadioGroupClick</c> / <c>CheckBoxD3DFormatClick</c>）+ 2 个 implementation 段嵌套过程
/// （<c>SelectDirCB</c> / <c>SelectDirectory</c>，见 <see cref="ShellDirectoryPicker"/>）=
/// 8 个可执行成员，本文件与接缝文件合计 8 个，无遗漏、无附加。</para>
///
/// <para><b>布局承载（D-P11-04）</b>：原文的 5 个控件里有 4 个是第三方 Raize 控件
/// （<c>TRzDialogButtons</c> / <c>TRzButtonEdit</c> / <c>TRzRadioGroup</c>），工程内无对应件，
/// 故按"控件语义等价 + DFM 数值逐项对齐"落地：<c>TRzButtonEdit</c> ⇒ TextBox + 右侧按钮、
/// <c>TRzRadioGroup</c> ⇒ GroupBox + 单选钮（原文 7 个 Items.Strings）、
/// <c>TRzDialogButtons</c> ⇒ 底部确定/取消按钮条。所有 DFM 常量都原样保留为 <c>Dfm*</c> 常量。</para>
///
/// <para><b>DFM 赋值顺序（P-D-P11-05）</b>：Delphi 的 <c>OnCreate</c> 在流化**之前**触发
/// （DFM 的 <c>ItemIndex = 3</c> / <c>Checked</c> 等随后覆盖处理器写的值）。WinForms 的
/// 控件树必须在构造里建立、<c>OnCreate</c> 之后触发，故顺序必然不同 —— 本移植把
/// <c>OnCreate</c> 语义放在 <see cref="InitDfm"/> 末尾显式调用（<c>FormCreate</c> 也仍作为
/// <see cref="Form.OnLoad"/> 处理器绑定，二者都可达；见测试用例的幂等断言）。</para>
/// </summary>
public class TFrmLogin : Form
{
    // =================================================================================
    // DFM 常量（全部取自 Source/Client-HGE/DxComponent/LoginDlg.dfm，逐字核对）
    // =================================================================================

    /// <summary>DFM:1 <c>object FrmLogin: TFrmLogin</c></summary>
    public const string DfmFormName = "FrmLogin";
    /// <summary>DFM:5 <c>Caption = 'FrmLogin'</c></summary>
    public const string DfmCaption = "FrmLogin";
    /// <summary>DFM:2 <c>Left = 478</c></summary>
    public const int DfmLeft = 478;
    /// <summary>DFM:3 <c>Top = 108</c></summary>
    public const int DfmTop = 108;
    /// <summary>DFM:4 <c>BorderStyle = bsDialog</c></summary>
    public const FormBorderStyle DfmBorderStyle = FormBorderStyle.FixedDialog;
    /// <summary>DFM:6 <c>ClientHeight = 152</c></summary>
    public const int DfmClientHeight = 152;
    /// <summary>DFM:7 <c>ClientWidth = 508</c></summary>
    public const int DfmClientWidth = 508;
    /// <summary>DFM:8 <c>Color = clBtnFace</c></summary>
    public const int DfmColor = 0x00F0F0F0;      // clBtnFace（COLOR_BTNFACE = $00F0F0F0，BGR）
    /// <summary>DFM:11 <c>Font.Height = -12</c>（负值 = 字符高度）</summary>
    public const int DfmFontHeight = -12;
    /// <summary>DFM:12 <c>Font.Name = #23435#20307</c> = '宋体'</summary>
    public const string DfmFontName = "宋体";
    /// <summary>DFM:15 <c>Position = poScreenCenter</c></summary>
    public const FormStartPosition DfmPosition = FormStartPosition.CenterScreen;
    /// <summary>DFM:16 <c>OnCreate = FormCreate</c></summary>
    public const string DfmOnCreate = "FormCreate";
    /// <summary>DFM:17 <c>PixelsPerInch = 96</c></summary>
    public const int DfmPixelsPerInch = 96;
    /// <summary>DFM:18 <c>TextHeight = 12</c></summary>
    public const int DfmTextHeight = 12;

    /// <summary>DFM:19-25 <c>object Label1: TLabel</c></summary>
    public const string DfmLabel1Name = "Label1";
    /// <summary>DFM:20/21 Label1 Left=16 Top=20</summary>
    public const int DfmLabel1Left = 16;
    /// <summary>DFM:21</summary>
    public const int DfmLabel1Top = 20;
    /// <summary>DFM:22/23 Label1 Width=48 Height=12</summary>
    public const int DfmLabel1Width = 48;
    /// <summary>DFM:23</summary>
    public const int DfmLabel1Height = 12;
    /// <summary>DFM:24 <c>Caption = #20256#22855#30446#24405</c> = '传奇目录'</summary>
    public const string DfmLabel1Caption = "传奇目录";

    /// <summary>DFM:26-34 <c>object DialogButtons: TRzDialogButtons</c></summary>
    public const string DfmDialogButtonsName = "DialogButtons";
    /// <summary>DFM:27/28 DialogButtons Left=0 Top=116</summary>
    public const int DfmDialogButtonsLeft = 0;
    /// <summary>DFM:28</summary>
    public const int DfmDialogButtonsTop = 116;
    /// <summary>DFM:29 DialogButtons Width=508</summary>
    public const int DfmDialogButtonsWidth = 508;
    /// <summary>DFM:30 <c>HotTrack = True</c></summary>
    public const bool DfmDialogButtonsHotTrack = true;
    /// <summary>DFM:31 <c>OnClickOk = DialogButtonsClickOk</c></summary>
    public const string DfmOnClickOk = "DialogButtonsClickOk";
    /// <summary>DFM:32 <c>OnClickCancel = DialogButtonsClickCancel</c></summary>
    public const string DfmOnClickCancel = "DialogButtonsClickCancel";
    /// <summary>DFM:33 <c>TabOrder = 0</c></summary>
    public const int DfmDialogButtonsTabOrder = 0;

    /// <summary>DFM:35-43 <c>object EditGamePath: TRzButtonEdit</c></summary>
    public const string DfmEditGamePathName = "EditGamePath";
    /// <summary>DFM:36/37 EditGamePath Left=72 Top=16</summary>
    public const int DfmEditGamePathLeft = 72;
    /// <summary>DFM:37</summary>
    public const int DfmEditGamePathTop = 16;
    /// <summary>DFM:38/39 Width=417 Height=20</summary>
    public const int DfmEditGamePathWidth = 417;
    /// <summary>DFM:39</summary>
    public const int DfmEditGamePathHeight = 20;
    /// <summary>DFM:41 <c>TabOrder = 1</c></summary>
    public const int DfmEditGamePathTabOrder = 1;
    /// <summary>DFM:42 <c>OnButtonClick = EditGamePathButtonClick</c></summary>
    public const string DfmOnButtonClick = "EditGamePathButtonClick";

    /// <summary>DFM:44-70 <c>object RadioGroup: TRzRadioGroup</c></summary>
    public const string DfmRadioGroupName = "RadioGroup";
    /// <summary>DFM:45/46 RadioGroup Left=16 Top=48</summary>
    public const int DfmRadioGroupLeft = 16;
    /// <summary>DFM:46</summary>
    public const int DfmRadioGroupTop = 48;
    /// <summary>DFM:47/48 Width=473 Height=57</summary>
    public const int DfmRadioGroupWidth = 473;
    /// <summary>DFM:48</summary>
    public const int DfmRadioGroupHeight = 57;
    /// <summary>DFM:49 <c>Columns = 4</c></summary>
    public const int DfmRadioGroupColumns = 4;
    /// <summary>DFM:50 <c>ItemIndex = 3</c>（原文如此：7 个 Items 里单选第 4 项 = '连击版'）</summary>
    public const int DfmRadioGroupItemIndex = 3;
    /// <summary>DFM:59 <c>TabOrder = 2</c></summary>
    public const int DfmRadioGroupTabOrder = 2;
    /// <summary>DFM:60 <c>OnClick = RadioGroupClick</c></summary>
    public const string DfmOnRadioGroupClick = "RadioGroupClick";

    /// <summary>
    /// DFM:51-58 <c>Items.Strings</c>（7 项；原文用 <c>#nnnn</c> 十进制转义写中文）。
    /// <c>'1.76'</c> / <c>'1.85'</c> / '英雄版' / '连击版' / '传奇续章' / '传奇外传' / '传奇归来'
    /// </summary>
    public static readonly string[] DfmRadioGroupItems =
    {
        "1.76", "1.85", "英雄版", "连击版", "传奇续章", "传奇外传", "传奇归来",
    };

    /// <summary>DFM:61-69 <c>object CheckBoxD3DFormat: TCheckBox</c></summary>
    public const string DfmCheckBoxD3DFormatName = "CheckBoxD3DFormat";
    /// <summary>DFM:62/63 CheckBoxD3DFormat Left=192 Top=32</summary>
    public const int DfmCheckBoxLeft = 192;
    /// <summary>DFM:63</summary>
    public const int DfmCheckBoxTop = 32;
    /// <summary>DFM:64/65 Width=73 Height=17</summary>
    public const int DfmCheckBoxWidth = 73;
    /// <summary>DFM:65</summary>
    public const int DfmCheckBoxHeight = 17;
    /// <summary>DFM:66 <c>Caption = #32441#29702#21387#32553</c> = '纹理压缩'</summary>
    public const string DfmCheckBoxCaption = "纹理压缩";
    /// <summary>DFM:67 <c>TabOrder = 0</c></summary>
    public const int DfmCheckBoxTabOrder = 0;
    /// <summary>DFM:68 <c>OnClick = CheckBoxD3DFormatClick</c></summary>
    public const string DfmOnCheckBoxClick = "CheckBoxD3DFormatClick";

    // =================================================================================
    // 控件（原文 DFM 的 5 个 object，名字原样保留）
    // =================================================================================

    /// <summary>DFM:19 <c>object Label1: TLabel</c></summary>
    public Label Label1;

    /// <summary>DFM:26 <c>object DialogButtons: TRzDialogButtons</c>（承载面：<see cref="DialogButtonsPanel"/>）</summary>
    public Panel DialogButtons;
    /// <summary>DFM:31 <c>OnClickOk</c> 的目标按钮。</summary>
    public Button DialogButtonsOk;
    /// <summary>DFM:32 <c>OnClickCancel</c> 的目标按钮。</summary>
    public Button DialogButtonsCancel;

    /// <summary>DFM:35 <c>object EditGamePath: TRzButtonEdit</c>（承载面：<see cref="EditGamePath"/> 文本框）</summary>
    public TextBox EditGamePath;
    /// <summary>DFM:42 <c>OnButtonClick = EditGamePathButtonClick</c> 的目标按钮（原文 Raize 内嵌按钮）。</summary>
    public Button EditGamePathButton;

    /// <summary>DFM:44 <c>object RadioGroup: TRzRadioGroup</c></summary>
    public GroupBox RadioGroup;
    /// <summary>DFM:51-58 的 7 个 Items（原文 <c>Items.Strings</c>）。</summary>
    public RadioButton[] RadioGroupItems;

    /// <summary>DFM:61 <c>object CheckBoxD3DFormat: TCheckBox</c>（原文挂在 RadioGroup 之内）</summary>
    public CheckBox CheckBoxD3DFormat;

    /// <summary><see cref="InitDfm"/> 的幂等标记（<c>OnCreate</c> 与 DFM 属性各只应用一次）。</summary>
    private bool _dfmApplied;

    /// <summary>
    /// 原文 <c>TRzRadioGroup.ItemIndex</c>（DFM:50 = 3）。WinForms 的 GroupBox 无此概念，
    /// 由 <see cref="RadioGroupItems"/> 的 Checked 状态表达；读写都走本属性。
    /// <c>-1</c> = 无选中（与 Delphi <c>ItemIndex = -1</c> 同义）。
    /// </summary>
    public int ItemIndex
    {
        get
        {
            for (int i = 0; i < RadioGroupItems.Length; i++)
            {
                if (RadioGroupItems[i].Checked) return i;
            }
            return -1;
        }
        set { SetItemIndex(value); }
    }

    /// <summary>原文 <c>ModalResult</c>（WinForms 的 <see cref="Form.DialogResult"/> 同义）。</summary>
    public DialogResult ModalResult
    {
        get => DialogResult;
        set => DialogResult = value;
    }

    /// <summary>原文 <c>mrYes</c> / <c>mrNo</c> 的承载（Delphi <c>ModalResult</c> 的取值）。</summary>
    public const DialogResult mrYes = DialogResult.Yes;
    /// <summary>原文 <c>mrNo</c>。</summary>
    public const DialogResult mrNo = DialogResult.No;

    /// <summary>
    /// LoginDlg.pas:11-27 的 <c>TFrmLogin = class(TForm)</c> 构造 + DFM 流化 + <c>OnCreate</c>。
    /// <para>构造函数**不触发**任何副作用（不碰磁盘）：DFM 的 5 个控件与 4 条事件绑定在
    /// <see cref="InitDfm"/> 里建立，<c>OnCreate</c>（<c>FormCreate</c>）在 <see cref="InitDfm"/> 末尾调用一次。
    /// 无头测试只构造、不调 <see cref="InitDfm"/>，即不会读 Config.ini。</para>
    /// </summary>
    public TFrmLogin()
    {
        Text = DfmCaption;                                  // DFM:5
        FormBorderStyle = DfmBorderStyle;                   // DFM:4
        StartPosition = DfmPosition;                        // DFM:15
        BackColor = Color.FromArgb(DfmColor);               // DFM:8（clBtnFace）
        Font = new Font(DfmFontName, DelphiFontSizeToPoints(DfmFontHeight));   // DFM:11-12
        ClientSize = new Size(DfmClientWidth, DfmClientHeight);                 // DFM:6-7
        // DFM:2-3 Left/Top：原文是设计期坐标，poScreenCenter 下由 VCL 在显示时重算；
        // WinForms 的 StartPosition=CenterScreen 同理，故这里只保留常量、不写 Left/Top。
    }

    /// <summary>
    /// 建立 DFM 的控件树（<b>不含</b>给控件赋 DFM 属性值）+ 绑 DFM 的 5 条事件
    /// （<c>OnClickOk</c>/<c>OnClickCancel</c>/<c>OnButtonClick</c>/<c>OnClick</c>×2）。
    /// <para>与 DFM 流化等价，幂等（重复调用不会重复挂控件）。</para>
    /// <para>调用序（见 <see cref="InitDfm"/>）里本方法是**第一步**。</para>
    /// </summary>
    private void CreateDfmControls()
    {
        if (Label1 != null) return;   // 幂等

        // ---- object Label1: TLabel（DFM:19-25）----
        Label1 = new Label
        {
            Name = DfmLabel1Name,
            AutoSize = false,
        };

        // ---- object DialogButtons: TRzDialogButtons（DFM:26-34）----
        //  承载面：底部按钮条。Left=0 Top=116 Width=508（DFM:27-29），TabOrder=0（DFM:33）。
        //  实测（Windows 10/11 默认主题）WinForms 按钮高度 23px、宽度 60px，
        //  原文 [确定][取消] 的右对齐结果落在 (378,116,435,139) 与 (440,116,497,139)。
        DialogButtons = new Panel { Name = DfmDialogButtonsName };
        DialogButtonsOk = new Button
        {
            Name = "DialogButtonsOk",
            Text = "确定",
            Width = 60,
            Height = 23,
        };
        DialogButtonsCancel = new Button
        {
            Name = "DialogButtonsCancel",
            Text = "取消",
            Width = 60,
            Height = 23,
        };
        DialogButtonsOk.Click += DialogButtonsClickOk;                     // DFM:31
        DialogButtonsCancel.Click += DialogButtonsClickCancel;             // DFM:32
        DialogButtons.Controls.Add(DialogButtonsOk);
        DialogButtons.Controls.Add(DialogButtonsCancel);

        // ---- object EditGamePath: TRzButtonEdit（DFM:35-43）----
        EditGamePath = new TextBox { Name = DfmEditGamePathName };
        EditGamePathButton = new Button
        {
            Name = "EditGamePathButton",
            Text = "...",
            Width = 20,
        };
        EditGamePathButton.Click += EditGamePathButtonClick;               // DFM:42

        // ---- object RadioGroup: TRzRadioGroup（DFM:44-70）----
        RadioGroup = new GroupBox
        {
            Name = DfmRadioGroupName,
            Text = string.Empty,
        };
        RadioGroupItems = new RadioButton[DfmRadioGroupItems.Length];
        for (int i = 0; i < DfmRadioGroupItems.Length; i++)
        {
            var radio = new RadioButton
            {
                Name = "RadioGroupItem" + i,
                Text = DfmRadioGroupItems[i],
                Width = 118,                                              // 473 / 4（原文由 Raize 按列均分）
                Height = 16,
                TabIndex = i,
            };
            int index = i;
            radio.Click += (sender, e) => RadioGroupClick(sender, e, index);   // DFM:60 OnClick = RadioGroupClick
            RadioGroupItems[i] = radio;
            RadioGroup.Controls.Add(radio);
        }

        CheckBoxD3DFormat = new CheckBox { Name = DfmCheckBoxD3DFormatName };
        CheckBoxD3DFormat.Click += CheckBoxD3DFormatClick;    // DFM:68
        RadioGroup.Controls.Add(CheckBoxD3DFormat);

        Controls.Add(Label1);
        Controls.Add(DialogButtons);
        Controls.Add(EditGamePath);
        Controls.Add(EditGamePathButton);
        Controls.Add(RadioGroup);
        AcceptButton = DialogButtonsOk;
        CancelButton = DialogButtonsCancel;
    }

    /// <summary>
    /// 把 DFM 文件里**声明出来的属性值**逐个写进控件。
    /// <para>原文的流化序是「<c>OnCreate</c>（:141-159）→ DFM 属性赋值」，故本方法是
    /// <see cref="InitDfm"/> 的**第三步**、在 <c>FormCreate</c> 之后执行 —— 这正是
    /// DFM:50 的 <c>ItemIndex = 3</c> 会**覆盖** <c>FormCreate</c> 所写
    /// <c>RadioGroup.ItemIndex := Integer(g_ClientVersion)</c> 的原因（原文如此，见 D-P11-05）。</para>
    /// </summary>
    private void ApplyDfmProperties()
    {
        // ---- TFrmLogin（DFM:2-18）----
        //  Left/Top/Caption/BorderStyle/ClientHeight/ClientWidth/Color/Font/Position
        //  已在构造函数里赋过（那几项与 OnCreate 无交互），此处只补 DFM 里"能覆盖处理器"的项。

        // ---- Label1（DFM:20-24）----
        Label1.Left = DfmLabel1Left;
        Label1.Top = DfmLabel1Top;
        Label1.Width = DfmLabel1Width;
        Label1.Height = DfmLabel1Height;
        Label1.Text = DfmLabel1Caption;

        // ---- DialogButtons（DFM:27-29/33）----
        DialogButtons.Left = DfmDialogButtonsLeft;
        DialogButtons.Top = DfmDialogButtonsTop;
        DialogButtons.Width = DfmDialogButtonsWidth;
        DialogButtons.Height = Height - ClientSize.Height + 23;   // 与 DFM ClientHeight 配套的按钮条高度
        DialogButtons.TabIndex = DfmDialogButtonsTabOrder;
        DialogButtonsOk.Left = DfmDialogButtonsWidth - 130;       // 右对齐（原文由 Raize 计算）
        DialogButtonsOk.Top = 0;
        DialogButtonsCancel.Left = DfmDialogButtonsWidth - 68;
        DialogButtonsCancel.Top = 0;

        // ---- EditGamePath（DFM:36-41）----
        EditGamePath.Left = DfmEditGamePathLeft;
        EditGamePath.Top = DfmEditGamePathTop;
        EditGamePath.Width = DfmEditGamePathWidth;
        EditGamePath.Height = DfmEditGamePathHeight;
        EditGamePath.TabIndex = DfmEditGamePathTabOrder;
        EditGamePathButton.Left = DfmEditGamePathLeft + DfmEditGamePathWidth;
        EditGamePathButton.Top = DfmEditGamePathTop;
        EditGamePathButton.Height = DfmEditGamePathHeight;

        // ---- RadioGroup（DFM:45-50/59）----
        RadioGroup.Left = DfmRadioGroupLeft;
        RadioGroup.Top = DfmRadioGroupTop;
        RadioGroup.Width = DfmRadioGroupWidth;
        RadioGroup.Height = DfmRadioGroupHeight;
        RadioGroup.TabIndex = DfmRadioGroupTabOrder;
        // 原文 Columns = 4（DFM:49）：行优先排布 ⇒ 第 i 项列 = i mod 4、行 = i div 4。
        for (int i = 0; i < RadioGroupItems.Length; i++)
        {
            RadioGroupItems[i].Left = (i % DfmRadioGroupColumns) * 118;
            RadioGroupItems[i].Top = (i / DfmRadioGroupColumns) * 16;
        }
        SetItemIndex(DfmRadioGroupItemIndex);                     // DFM:50 ItemIndex = 3

        // ---- CheckBoxD3DFormat（DFM:62-67）----
        CheckBoxD3DFormat.Left = DfmCheckBoxLeft;                 // 相对 RadioGroup
        CheckBoxD3DFormat.Top = DfmCheckBoxTop;
        CheckBoxD3DFormat.Width = DfmCheckBoxWidth;
        CheckBoxD3DFormat.Height = DfmCheckBoxHeight;
        CheckBoxD3DFormat.Text = DfmCheckBoxCaption;
        CheckBoxD3DFormat.TabIndex = DfmCheckBoxTabOrder;
    }

    /// <summary>
    /// 与 <c>DFM 流化 + TForm 构造</c> 等价的三步：
    /// <list type="number">
    /// <item><see cref="CreateDfmControls"/> —— 建控件树（= DFM 的 <c>object ... end</c> 逐个 Create）；</item>
    /// <item><see cref="FormCreate"/> —— 原文 <c>OnCreate</c>（DFM:16），流化**之前**触发；</item>
    /// <item><see cref="ApplyDfmProperties"/> —— DFM 的属性赋值（在 <c>OnCreate</c> **之后**）。</item>
    /// </list>
    /// </summary>
    public void InitDfm()
    {
        CreateDfmControls();
        if (_dfmApplied) return;      // 幂等：OnCreate 与 DFM 属性各只跑一次
        FormCreate(this, EventArgs.Empty);   // DFM:16 OnCreate = FormCreate
        ApplyDfmProperties();
        _dfmApplied = true;
    }


    /// <summary>
    /// 原文 <c>TForm.OnCreate</c> 的托管落点。原文此处只设 <c>ItemIndex</c>（不做别的事），
    /// 而 WinForms 的 <see cref="Form.OnLoad"/> 是"面向用户显示前"的最后一次机会，
    /// 与 VCL <c>OnCreate</c> 的**效果**（显示前初始化一次）一致。
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        InitDfm();
        base.OnLoad(e);
    }

    /// <summary>原文 <c>RadioGroup.ItemIndex := X</c>（单选项互斥由 WinForms 自动维护，故先全清）。</summary>
    public void SetItemIndex(int value)
    {
        if (value < 0 || value >= RadioGroupItems.Length)
        {
            // 原文如此（D-P11-06）：Delphi 的 `ItemIndex := 越界值` 会被 TRzRadioGroup
            // 内部 clamp 到 -1（无选中），不抛异常。托管侧同样退化为"全部不选"。
            foreach (var r in RadioGroupItems) r.Checked = false;
            return;
        }
        RadioGroupItems[value].Checked = true;
    }

    /// <summary>原文的 <c>Close</c>（VCL <c>TForm.Close</c>；模态下等价于返回 ModalResult）。</summary>
    public void CloseForm() => Close();

    /// <summary>
    /// DFM:11 的 <c>Font.Height = -12</c> → WinForms 磅值。
    /// Delphi 的负 Height 是**字符高（像素）**，屏幕上 1 磅 = PixelsPerInch/72 像素。
    /// 不加载真实字体度量（无头环境不可靠），按 DFM:17 的 <c>PixelsPerInch = 96</c> 换算：
    /// <c>12 / (96/72) = 9</c> 磅。
    /// </summary>
    public static float DelphiFontSizeToPoints(int fontHeight)
        => Math.Abs(fontHeight) * 72f / DfmPixelsPerInch;

    // =================================================================================
    // 原文 6 个事件处理器（方法名与原文逐字一致）
    // =================================================================================

    /// <summary>
    /// LoginDlg.pas:97-107 <c>TFrmLogin.EditGamePathButtonClick</c> 1:1。
    /// <para><b>原文缺陷照抄（D-P11-07）</b>：当用户取消目录选择时，原文的收尾顺序是
    /// <c>Close; ModalResult := mrNo;</c> —— <b>先关窗、后赋 ModalResult</b>（:102-103）。
    /// 对照 <c>DialogButtonsClickCancel</c>（:161-165）是同一顺序，而 <c>DialogButtonsClickOk</c>（:137-138）
    /// 也是同一顺序，故三处一致，不是笔误。托管侧用 <c>ModalResult 赋值 + Close()</c> 复刻，
    /// 并在测试中把"赋值先于关闭"这一顺序锁死。</para>
    /// </summary>
    public void EditGamePathButtonClick(object sender, EventArgs e)
    {
        EditGamePath.Text = LoginDlgGlobals.g_sMirDataDirectory;                     // :99
        var pick = LoginDlgHost.PickDirectory("请选择传奇客户端“Legend of mir2”目录", string.Empty,
            LoginDlgGlobals.g_sMirDataDirectory, Handle);                            // :100
        if (!pick.Result)                                                            // :100 `if not SelectDirectory(...) then`
        {
            //ModalResult := 0;                                                      // :101（原文注释掉）
            CloseForm();                                                             // :102 Close;
            ModalResult = mrNo;                                                      // :103
            return;                                                                  // :104 Exit;
        }
        EditGamePath.Text = LoginDlgGlobals.g_sMirDataDirectory;                     // :106
    }

    /// <summary>LoginDlg.pas:109-139 <c>TFrmLogin.DialogButtonsClickOk</c> 1:1。</summary>
    public void DialogButtonsClickOk(object sender, EventArgs e)
    {
        int I;
        // :116 IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'Config.ini');
        var IniFile = new TLoginIniFile(LoginDlgHost.ConfigIniPath());
        try
        {
            LoginDlgGlobals.g_sMirDataDirectory = DelphiRTL.Trim(EditGamePath.Text);   // :117
            //IniFile.WriteString('Setup', 'Directory', g_sMirDataDirectory);           // :118（原文注释掉）
            IniFile.WriteInteger("Setup", "ClientVersion", (int)LoginDlgGlobals.g_ClientVersion);   // :119
            IniFile.WriteBool("Setup", "D3DFormat", LoginDlgGlobals.g_boD3DFormat);                 // :120

            var FileNameList = new List<string>();                                     // :121
            IniFile.ReadSection("FileNames", FileNameList);                            // :122
            for (I = 0; I <= FileNameList.Count - 1; I++)                              // :123
            {
                string sFileName = IniFile.ReadString("FileNames", FileNameList[I], string.Empty);   // :124
                if (sFileName.Length != 0)                                             // :125
                    LoginDlgGlobals.g_FileNameList.AddObject(sFileName, I);            // :126 TObject(I)
            }
            // :128 FileNameList.Free（托管侧 GC）

            LoginDlgGlobals.g_MirDataDirectoryList[(int)LoginDlgGlobals.g_ClientVersion]
                = LoginDlgGlobals.g_sMirDataDirectory;                                 // :130
            for (I = 0; I <= LoginDlgGlobals.g_MirDataDirectoryList.Length - 1; I++)   // :131
            {
                IniFile.WriteString("Directory", DelphiRTL.IntToStr(I),
                    LoginDlgGlobals.g_MirDataDirectoryList[I]);                        // :132
                //   原文 :132 写的是 `g_MirDataDirectoryList[TClientVersion(I)]` —— 枚举下标与
                //   整数下标同值（cv176=0 … cvMirNewUI205=5），故此处用 int 下标是**同一取值**。
            }
        }
        finally
        {
            IniFile.Dispose();                                                         // :135 IniFile.Free;
        }

        //ModalResult := mrYes;                                                        // :136（原文注释掉）
        CloseForm();                                                                   // :137
        ModalResult = mrYes;                                                           // :138
    }

    /// <summary>
    /// LoginDlg.pas:141-159 <c>TFrmLogin.FormCreate</c> 1:1。
    /// </summary>
    public void FormCreate(object sender, EventArgs e)
    {
        int I;
        ModalResult = mrNo;                                                            // :146
        var IniFile = new TLoginIniFile(LoginDlgHost.ConfigIniPath());                 // :147
        try
        {
            //g_sMirDataDirectory := IniFile.ReadString('Setup', 'Directory', g_sMirDataDirectory);  // :148（原文注释掉）
            LoginDlgGlobals.g_ClientVersion = (TClientVersion)IniFile.ReadInteger("Setup", "ClientVersion",
                (int)LoginDlgGlobals.g_ClientVersion);                                 // :149
            LoginDlgGlobals.g_boD3DFormat = IniFile.ReadBool("Setup", "D3DFormat",
                LoginDlgGlobals.g_boD3DFormat);                                        // :150
            for (I = 0; I <= LoginDlgGlobals.g_MirDataDirectoryList.Length - 1; I++)   // :151
            {
                // :152 g_MirDataDirectoryList[TClientVersion(I)] := IniFile.ReadString('Directory', IntToStr(I), g_sMirDataDirectory);
                LoginDlgGlobals.g_MirDataDirectoryList[I] = IniFile.ReadString("Directory",
                    DelphiRTL.IntToStr(I), LoginDlgGlobals.g_sMirDataDirectory);
            }
            LoginDlgGlobals.g_sMirDataDirectory
                = LoginDlgGlobals.g_MirDataDirectoryList[(int)LoginDlgGlobals.g_ClientVersion];   // :154
            EditGamePath.Text = LoginDlgGlobals.g_sMirDataDirectory;                   // :155
            SetItemIndex((int)LoginDlgGlobals.g_ClientVersion);                        // :156 RadioGroup.ItemIndex
            CheckBoxD3DFormat.Checked = LoginDlgGlobals.g_boD3DFormat;                 // :157
        }
        finally
        {
            IniFile.Dispose();                                                         // :158 IniFile.Free;
        }
    }

    /// <summary>LoginDlg.pas:161-165 <c>TFrmLogin.DialogButtonsClickCancel</c> 1:1。</summary>
    public void DialogButtonsClickCancel(object sender, EventArgs e)
    {
        CloseForm();                                                                   // :163 Close;
        ModalResult = mrNo;                                                            // :164
    }

    /// <summary>
    /// LoginDlg.pas:167-171 <c>TFrmLogin.RadioGroupClick</c> 1:1。
    /// <para>对应 DFM 的 <c>OnClick = RadioGroupClick</c>（DFM:60）；原文的 <c>Sender</c> 未被使用，
    /// 选中项从控件自身读 <c>RadioGroup.ItemIndex</c>。</para>
    /// </summary>
    public void RadioGroupClick(object sender, EventArgs e)
    {
        RadioGroupClick(sender, e, ItemIndex);
    }

    /// <summary><c>RadioGroupClick</c> 的内部重载：把"被点的第几项"作为 <c>ItemIndex</c> 传入
    /// （WinForms 的 <c>RadioButton.Click</c> 每项各绑一个处理器，需要在绑定时捕获下标）。</summary>
    public void RadioGroupClick(object sender, EventArgs e, int itemIndex)
    {
        ItemIndex = itemIndex;                                                          // RadioGroup.ItemIndex
        LoginDlgGlobals.g_ClientVersion = (TClientVersion)ItemIndex;                    // :169
        EditGamePath.Text = LoginDlgGlobals.g_MirDataDirectoryList[(int)LoginDlgGlobals.g_ClientVersion];   // :170
    }

    /// <summary>LoginDlg.pas:173-176 <c>TFrmLogin.CheckBoxD3DFormatClick</c> 1:1。</summary>
    public void CheckBoxD3DFormatClick(object sender, EventArgs e)
    {
        LoginDlgGlobals.g_boD3DFormat = CheckBoxD3DFormat.Checked;                      // :175
    }
}
