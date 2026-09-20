using System;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmAddProcessBlack.pas 1:1 转换（Source\RunGate\uFrmAddProcessBlack.pas，107 行 / LF 106）。
// 布局真源：Source\RunGate\uFrmAddProcessBlack.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：录入「进程名 + MD5」并加入 `g_ProcessBlackList`。6 条校验分支，顺序即原文顺序：
//   1) Count >= 80               → '已经达到最多数量，无法添加'      （原 :53-58，**校验的是字面量 80，不是 MaxCount**）
//   2) Length(edtProcessName.Text) = 0 → '进程名不能为空'           （原 :60-65，**不 Trim**）
//   3) Length(Trim(edtProcessMD5.Text)) = 0 → '进程MD5不能为空'     （原 :67-73，**Trim 后判空**）
//   4) Length(MD5) <> 32         → '进程MD5长度不对'                 （原 :75-80）
//   5) not IsHexString(MD5)      → '进程MD5包含非法的字符串'         （原 :82-87）
//   6) Add 返回 nil              → '进程MD5已经存在于黑名单中'       （原 :96-101）
// 前 5 条与第 6 条都是 Exit（不置 ModalResult），只有全部通过才 `ModalResult := mrOK`。
//
// ★ 差异断言点（"看起来一样实则不同"）：
//   * 分支 2 用 `Length(edtProcessName.Text) = 0`（**原始文本**，含空格也算非空），
//     分支 3 用 `Length(Trim(edtProcessMD5.Text)) = 0`（**Trim 后**）。
//     因此"进程名 = 单个空格"会**通过**校验，而"MD5 = 单个空格"会被判空。
//   * 分支 4/5 只对 MD5 做 Trim（MD5 := Trim(edtProcessMD5.Text)，原 :67），进程名不 Trim，
//     最终 Add 用的是 `edtProcessName.Text`（原 :91）—— 带空格的进程名会原样入库。
// =====================================================================================

/// <summary>uFrmAddProcessBlack.pas 的校验结果（可单测，不依赖 WinForms）。</summary>
public enum AddProcessBlackResult
{
    OK = 0,                     // 全部通过 → ModalResult := mrOK
    MaxCountReached = 1,        // 原 :53-58
    EmptyProcessName = 2,       // 原 :60-65
    EmptyMD5 = 3,               // 原 :67-73
    BadMD5Length = 4,           // 原 :75-80
    NonHexMD5 = 5,              // 原 :82-87
    DuplicateMD5 = 6            // 原 :96-101
}

/// <summary>uFrmAddProcessBlack.pas 的非 UI 逻辑。</summary>
public static class AddProcessBlackLogic
{
    /// <summary>原 :53 的硬编码上限（**不是** g_ProcessBlackList.MaxCount）。</summary>
    public const int HardCodedMaxCount = 80;

    /// <summary>原 :55/62/70/77/84/98 的 6 条提示文本 + 标题（标题统一 '提示'）。</summary>
    public const string Caption = "提示";
    public const string MsgMaxCount = "已经达到最多数量，无法添加";
    public const string MsgEmptyProcessName = "进程名不能为空";
    public const string MsgEmptyMD5 = "进程MD5不能为空";
    public const string MsgBadMD5Length = "进程MD5长度不对";
    public const string MsgNonHexMD5 = "进程MD5包含非法的字符串";
    public const string MsgDuplicateMD5 = "进程MD5已经存在于黑名单中";

    /// <summary>原 :55/62/... 的 MessageBox 图标：`MB_OK + MB_ICONINFORMATION`。</summary>
    public static void ShowHint(AddProcessBlackResult result)
    {
        string text;
        switch (result)
        {
            case AddProcessBlackResult.MaxCountReached: text = MsgMaxCount; break;
            case AddProcessBlackResult.EmptyProcessName: text = MsgEmptyProcessName; break;
            case AddProcessBlackResult.EmptyMD5: text = MsgEmptyMD5; break;
            case AddProcessBlackResult.BadMD5Length: text = MsgBadMD5Length; break;
            case AddProcessBlackResult.NonHexMD5: text = MsgNonHexMD5; break;
            case AddProcessBlackResult.DuplicateMD5: text = MsgDuplicateMD5; break;
            default: return;
        }
        MessageBoxSeam.ShowInformation(text, Caption);
    }

    /// <summary>
    /// 原 uFrmAddProcessBlack.pas:49-104 的**全部校验分支**（纯函数，不动全局状态）。
    /// 输入分别是 `edtProcessName.Text`（**不 Trim**）与 `edtProcessMD5.Text`（内部 Trim）。
    /// </summary>
    public static AddProcessBlackResult Validate(string processNameText, string processMD5Text,
                                                 int currentCount, Func<string, string, bool> isDuplicate)
    {
        if (currentCount >= HardCodedMaxCount)                                            // 原 :53
            return AddProcessBlackResult.MaxCountReached;

        if ((processNameText ?? "").Length == 0)                                          // 原 :60（不 Trim）
            return AddProcessBlackResult.EmptyProcessName;

        string md5 = DelphiRTL.Trim(processMD5Text);                                      // 原 :67
        if (md5.Length == 0)                                                              // 原 :68
            return AddProcessBlackResult.EmptyMD5;

        if (md5.Length != 32)                                                             // 原 :75
            return AddProcessBlackResult.BadMD5Length;

        if (!GateShareHelper.IsHexString(md5))                                             // 原 :82
            return AddProcessBlackResult.NonHexMD5;

        if (isDuplicate != null && isDuplicate(processNameText ?? "", md5))                // 原 :96（Add 返回 nil）
            return AddProcessBlackResult.DuplicateMD5;

        return AddProcessBlackResult.OK;
    }
}

/// <summary>GateShare.pas:1805 `function IsHexString(S: string): Boolean;`（纯函数，1:1）。
/// 原文语义：空串返回 True；逐字符必须是 '0'..'9' 或 'A'..'F' 或 'a'..'f'。</summary>
public static class GateShareHelper
{
    public static bool IsHexString(string s)
    {
        s ??= "";
        if (s.Length == 0) return true;                 // 原文：Result := True; 空串直接通过
        foreach (char c in s)
        {
            bool ok = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
            if (!ok) return false;
        }
        return true;
    }
}

/// <summary>原 :32-47 `function ShowAddProcessBlack(var ProcessInfo: PTProcessInfo): Boolean;`。</summary>
public static class AddProcessBlackUnit
{
    /// <summary>成功时通过 <paramref name="processInfo"/> 返回新建的黑名单项。</summary>
    public static bool ShowAddProcessBlack(out TProcessInfo processInfo)
    {
        using var form = new FrmAddProcessBlack();
        processInfo = null;
        bool ok = form.ShowDialog() == DialogResult.OK;      // 原 :39
        if (ok) processInfo = form.FProcessInfo;             // 原 :40-42
        return ok;
    }
}

/// <summary>原 uFrmAddProcessBlack.pas:10-24 `TFrmAddProcessBlack`（DFM: uFrmAddProcessBlack.dfm）。</summary>
public class FrmAddProcessBlack : Form
{
    // DFM: FrmAddProcessBlack Left=271 Top=330 BorderStyle=bsDialog Caption='添加进程黑名单'
    //      ClientHeight=122 ClientWidth=365 Font.Charset=GB2312_CHARSET Font.Height=-13 Font.Name='宋体'
    //      Position=poMainFormCenter PixelsPerInch=96
    public GroupBox grp1;        // DFM: grp1 Left=6 Top=6 Width=353 Height=75 Caption='进程信息' TabOrder=0
    public Label lbl1;           // DFM: lbl1 Left=6 Top=22 Width=52 Height=13 Caption='进程名：'
    public Label Label1;         // DFM: Label1 Left=11 Top=48 Width=47 Height=13 Caption='MD5值：'
    public TextBox edtProcessName; // DFM: edtProcessName Left=54 Top=18 Width=291 Height=21 TabOrder=0
    public TextBox edtProcessMD5;  // DFM: edtProcessMD5 Left=54 Top=44 Width=291 Height=21 MaxLength=32 TabOrder=1
    public Button btnOK;         // DFM: btnOK Left=200 Top=88 Width=75 Height=25 Caption='确定' TabOrder=1 OnClick=btnOKClick
    public Button btnCancel;     // DFM: btnCancel Left=284 Top=88 Width=75 Height=25 Caption='取消' ModalResult=2 TabOrder=2

    /// <summary>原 :21 `FProcessInfo: PTProcessInfo;`（成功时非 nil）。</summary>
    public TProcessInfo FProcessInfo;

    public FrmAddProcessBlack()
    {
        // DFM: FrmAddProcessBlack Caption='添加进程黑名单' BorderStyle=bsDialog
        Text = "添加进程黑名单";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;      // Position=poMainFormCenter
        Location = new Point(271, 330);
        ClientSize = new Size(365, 122);
        Font = new Font("宋体", 9.75F);                      // Font.Height=-13 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: grp1 Left=6 Top=6 Width=353 Height=75 Caption='进程信息' TabOrder=0
        grp1 = new GroupBox { Left = 6, Top = 6, Width = 353, Height = 75, Text = "进程信息", TabIndex = 0 };
        // DFM: lbl1 Left=6 Top=22 Width=52 Height=13 Caption='进程名：'
        lbl1 = new Label { Left = 6, Top = 22, Width = 52, Height = 13, Text = "进程名：" };
        // DFM: Label1 Left=11 Top=48 Width=47 Height=13 Caption='MD5值：'
        Label1 = new Label { Left = 11, Top = 48, Width = 47, Height = 13, Text = "MD5值：" };
        // DFM: edtProcessName Left=54 Top=18 Width=291 Height=21 TabOrder=0
        edtProcessName = new TextBox { Left = 54, Top = 18, Width = 291, Height = 21, TabIndex = 0 };
        // DFM: edtProcessMD5 Left=54 Top=44 Width=291 Height=21 MaxLength=32 TabOrder=1
        edtProcessMD5 = new TextBox { Left = 54, Top = 44, Width = 291, Height = 21, MaxLength = 32, TabIndex = 1 };

        // DFM: btnOK Left=200 Top=88 Width=75 Height=25 Caption='确定' TabOrder=1 OnClick=btnOKClick
        btnOK = new Button { Left = 200, Top = 88, Width = 75, Height = 25, Text = "确定", TabIndex = 1 };
        // DFM: btnCancel Left=284 Top=88 Width=75 Height=25 Caption='取消' ModalResult=2 TabOrder=2
        btnCancel = new Button { Left = 284, Top = 88, Width = 75, Height = 25, Text = "取消", TabIndex = 2,
                                 DialogResult = DialogResult.Cancel };

        grp1.Controls.Add(lbl1);
        grp1.Controls.Add(Label1);
        grp1.Controls.Add(edtProcessName);
        grp1.Controls.Add(edtProcessMD5);
        Controls.Add(grp1);
        Controls.Add(btnOK);
        Controls.Add(btnCancel);

        // DFM: btnOK OnClick = btnOKClick
        btnOK.Click += (s, e) => btnOK_Click(s, e);
    }

    /// <summary>原 uFrmAddProcessBlack.pas:49-104 `TFrmAddProcessBlack.btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        if (FormGlobals.g_ProcessBlackList.Count >= AddProcessBlackLogic.HardCodedMaxCount)   // 原 :53
        {
            AddProcessBlackLogic.ShowHint(AddProcessBlackResult.MaxCountReached);             // 原 :55
            edtProcessName.Focus();                                                          // 原 :56
            return;                                                                          // 原 :57 Exit
        }

        if (edtProcessName.Text.Length == 0)                                                 // 原 :60（不 Trim）
        {
            AddProcessBlackLogic.ShowHint(AddProcessBlackResult.EmptyProcessName);            // 原 :62
            edtProcessName.Focus();                                                          // 原 :63
            return;                                                                          // 原 :64 Exit
        }

        string md5 = DelphiRTL.Trim(edtProcessMD5.Text);                                     // 原 :67
        if (md5.Length == 0)                                                                 // 原 :68
        {
            AddProcessBlackLogic.ShowHint(AddProcessBlackResult.EmptyMD5);                    // 原 :70
            edtProcessMD5.Focus();                                                           // 原 :71
            return;
        }

        if (md5.Length != 32)                                                                // 原 :75
        {
            AddProcessBlackLogic.ShowHint(AddProcessBlackResult.BadMD5Length);                // 原 :77
            edtProcessMD5.Focus();                                                           // 原 :78
            return;
        }

        if (!GateShareHelper.IsHexString(md5))                                                // 原 :82
        {
            AddProcessBlackLogic.ShowHint(AddProcessBlackResult.NonHexMD5);                   // 原 :84
            edtProcessMD5.Focus();                                                           // 原 :85
            return;
        }

        // 原 :89-94
        FormGlobals.g_ProcessBlackList.Lock();
        try
        {
            FProcessInfo = FormGlobals.g_ProcessBlackList.Add(edtProcessName.Text, md5);      // 原 :91
        }
        finally
        {
            FormGlobals.g_ProcessBlackList.UnLock();                                          // 原 :93
        }

        if (FProcessInfo == null)                                                             // 原 :96
        {
            AddProcessBlackLogic.ShowHint(AddProcessBlackResult.DuplicateMD5);                // 原 :98
            edtProcessMD5.Focus();                                                            // 原 :99
            return;                                                                           // 原 :100 Exit
        }

        DialogResult = DialogResult.OK;                                                       // 原 :103 ModalResult := mrOK
    }
}
