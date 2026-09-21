using System;
using System.Windows.Forms;
using GXX.Core.Rtl;

// 源单元：Source/DBServer/CreateChr.pas（69 行）→ 本文件（1:1 移植）
// DFM：Source/DBServer/CreateChr.dfm（944 字节，**二进制 DFM**）—— 按 §41.3-1 回读 `Source/**` 原始字节
//      手工解码（`_analysis/utf8_mirror` 里的副本已损坏，**不可用**）。
//      实测：23 字节头 `FF 0A 00` + 'TFRMCREATECHR' + 00 + `30 10` + Int32(921)，
//      流从偏移 23 的 'TPF0' 起、解到 **944 = 文件长度**（零残留）。
//      对账：DFM 控件 **8** / 托管字段 **8**（Label1 Label2 Label3 EdUserId EdChrName BitBtnOK BitBtnCancel EditSelectID）；
//      DFM 事件绑定 **1**（根 `OnShow = FormShow`）/ 托管 `+=` **1**。
// 方法对账：原文 **3**（FormShow / IncputChrInfo / GetInputInfo）→ 托管 **3**。
//
// 消费者取证（本单元到底还有没有人用）：
//   · `DBServer.dpr:14` uses + `:37 Application.CreateForm(TFrmCreateChr, FrmCreateChr)` ⇒ 由 .dpr 常驻实例化；
//   · 唯一**调用点**曾出现在 `LoginSrv/uFrmDataManager.pas:144-158`，但那段（:139-176）在原文里
//     被 `(* … *)` **整段注释**（托管 `uFrmDataManager.cs:184-188 BtnCreateChrClick` 已按"空实现"登记）
//     ⇒ 本单元目前是"被实例化但无人调用"的窗体，仍按 1:1 移植（.dpr 成员，不是死代码）。

namespace GXX.DBServer.Forms2;

/// <summary>
/// `CreateChr.pas:9-28 TFrmCreateChr`（DFM: CreateChr.dfm，二进制）。
/// </summary>
public class TFrmCreateChr : Form
{
    // DFM: FrmCreateChr Left=967 Top=489 BorderIcons=[biSystemMenu] BorderStyle=bsSingle
    //      Caption='创建新人物' ClientHeight=130 ClientWidth=250 Color=clBtnFace
    //      Font.Charset=ANSI_CHARSET Font.Name='宋体' Font.Height=-12 Font.Style=[]
    //      OldCreateOrder=False OnShow=FormShow PixelsPerInch=96 TextHeight=12
    public Label Label1 = null!;
    public Label Label2 = null!;
    public Label Label3 = null!;
    public TextBox EdUserId = null!;
    public TextBox EdChrName = null!;
    public Button BitBtnOK = null!;              // DFM: TBitBtn Kind=bkOK
    public Button BitBtnCancel = null!;          // DFM: TBitBtn Kind=bkCancel
    public TextBox EditSelectID = null!;

    // CreateChr.pas:23-25 三个 public 字段
    /// <summary>`CreateChr.pas:23 sUserId: string`。</summary>
    public string sUserId = "";

    /// <summary>`CreateChr.pas:24 sChrName: string`。</summary>
    public string sChrName = "";

    /// <summary>`CreateChr.pas:25 nSelectID: Integer`（Delphi 默认 0；取消时保持 0 不变）。</summary>
    public int nSelectID;

    /// <summary>
    /// `CreateChr.pas:31 var FrmCreateChr: TFrmCreateChr`（由 `DBServer.dpr:37 Application.CreateForm` 赋值）。
    /// 托管侧留同名静态字段供宿主接线（<c>Program.cs</c> 在分区外 ⇒ 登记跨区请求 X-P10-01）。
    /// </summary>
    public static TFrmCreateChr? FrmCreateChr;

    public TFrmCreateChr()
    {
        // DFM: Caption='创建新人物' ClientHeight=130 ClientWidth=250 BorderStyle=bsSingle Position 未设（默认 poDesigned）
        Text = "创建新人物";
        ClientSize = new System.Drawing.Size(250, 130);
        Location = new System.Drawing.Point(967, 489);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;                     // BorderIcons=[biSystemMenu] ⇒ 无最大化/最小化按钮
        MinimizeBox = false;
        // 装饰性属性按 §7「装饰性属性允许简化但控件与行为保留」：Font.Charset/Name/Height 未复刻
        // （WinForms 用系统默认 UI 字体；原文 '宋体'/-12 与 TextHeight=12 等价于 9pt@96dpi）。

        // DFM: Label1 Left=18 Top=18 Width=54 Height=12 Caption='登录帐号:'
        Label1 = new Label { Left = 18, Top = 18, Width = 54, Height = 12, Text = "登录帐号:" };
        // DFM: Label2 Left=18 Top=42 Width=54 Height=12 Caption='人物名称:'
        Label2 = new Label { Left = 18, Top = 42, Width = 54, Height = 12, Text = "人物名称:" };
        // DFM: Label3 Left=18 Top=66 Width=42 Height=12 Caption='选择ID:'（原文为 vaUTF8String）
        Label3 = new Label { Left = 18, Top = 66, Width = 42, Height = 12, Text = "选择ID:" };
        // DFM: EdUserId Left=80 Top=14 Width=149 Height=20 TabOrder=0
        EdUserId = new TextBox { Left = 80, Top = 14, Width = 149, Height = 20, TabIndex = 0 };
        // DFM: EdChrName Left=80 Top=37 Width=149 Height=20 TabOrder=1
        EdChrName = new TextBox { Left = 80, Top = 37, Width = 149, Height = 20, TabIndex = 1 };
        // DFM: BitBtnOK Left=23 Top=90 Width=93 Height=31 Caption='确定(&O)' TabOrder=2 Kind=bkOK
        BitBtnOK = new Button { Left = 23, Top = 90, Width = 93, Height = 31, Text = "确定(&O)", TabIndex = 2, DialogResult = DialogResult.OK };
        // DFM: BitBtnCancel Left=137 Top=90 Width=92 Height=31 Caption='取消(&C)' TabOrder=3 Kind=bkCancel
        BitBtnCancel = new Button { Left = 137, Top = 90, Width = 92, Height = 31, Text = "取消(&C)", TabIndex = 3, DialogResult = DialogResult.Cancel };
        // DFM: EditSelectID Left=80 Top=61 Width=149 Height=20 TabOrder=4 Text='0'
        EditSelectID = new TextBox { Left = 80, Top = 61, Width = 149, Height = 20, TabIndex = 4, Text = "0" };

        Controls.Add(Label1);
        Controls.Add(Label2);
        Controls.Add(Label3);
        Controls.Add(EdUserId);
        Controls.Add(EdChrName);
        Controls.Add(BitBtnOK);
        Controls.Add(BitBtnCancel);
        Controls.Add(EditSelectID);

        AcceptButton = BitBtnOK;                 // TBitBtn Kind=bkOK 默认按钮语义
        CancelButton = BitBtnCancel;             // TBitBtn Kind=bkCancel

        // DFM 绑定（1/1）：根 OnShow = FormShow
        Shown += (s, e) => FormShow(s);

        // 接缝默认值：Delphi `Self.ShowModal` ⇒ 弹本窗体
        ShowModalHandler = () => ShowDialog();
    }

    /// <summary>`CreateChr.pas:39-42 procedure TFrmCreateChr.FormShow`：`EdUserId.SetFocus;`。</summary>
    public void FormShow(object? Sender)
    {
        EdUserId.Focus();                        // SetFocus（无窗口句柄时 WinForms 安全返回 false）
    }

    /// <summary>
    /// `CreateChr.pas:44-49 function TFrmCreateChr.IncputChrInfo: Boolean`（原文注释 `//0x0049C65C`）1:1。
    /// 先清空两个 public 字段，再交给 <see cref="GetInputInfo"/>。
    /// </summary>
    public bool IncputChrInfo()
    {
        sUserId = "";
        sChrName = "";
        return GetInputInfo();
    }

    /// <summary>
    /// `CreateChr.pas:51-68 function TFrmCreateChr.GetInputInfo(): Boolean` 1:1（原文 private）。
    ///
    /// <para>
    /// 原文逐句：`Result := False` → 用两个字段回填编辑框（**注意 `EditSelectID` 不回填**）
    /// → `if Self.ShowModal = mrOK then` → `Trim` 两个文本框写回字段 →
    /// `nSelectID := StrToIntDef(Trim(EditSelectID.Text), -1)` →
    /// `nSelectID &lt; 0` 时 `MessageBox(Handle, '选择ID输入不正确！！！', '确认信息', MB_OK + MB_ICONEXCLAMATION)` 后 **Exit**
    /// （返回值保持 False，且**不会重新弹出对话框**）→ 否则 `Result := True`。
    /// </para>
    /// <para>
    /// 托管差异 D-P10-10：`Self.ShowModal` 换成可注入闸门 <see cref="ShowModalHandler"/>
    /// （默认转调 <see cref="Form.ShowDialog()"/>，返回值与原文 `mrOK`/`mrCancel` 数值一致：
    /// <see cref="DialogResult.OK"/>=1 / <see cref="DialogResult.Cancel"/>=2）。
    /// </para>
    /// </summary>
    private bool GetInputInfo()
    {
        bool Result = false;
        EdUserId.Text = sUserId;
        EdChrName.Text = sChrName;
        if (ShowModalHandler() == DialogResult.OK)
        {
            sUserId = DelphiRTL.Trim(EdUserId.Text);
            sChrName = DelphiRTL.Trim(EdChrName.Text);
            nSelectID = DelphiRTL.StrToIntDef(DelphiRTL.Trim(EditSelectID.Text), -1);
            if (nSelectID < 0)
            {
                UiSeam.MessageBox("选择ID输入不正确！！！", "确认信息",
                    TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);
                return Result;
            }
            Result = true;
        }
        return Result;
    }

    /// <summary>
    /// 接缝：Delphi `Self.ShowModal`（`CreateChr.pas:56`）。
    /// 默认弹**本窗体**的模态框；测试注入即返回 <see cref="DialogResult.OK"/> / <see cref="DialogResult.Cancel"/>。
    /// </summary>
    public Func<DialogResult> ShowModalHandler;
}
