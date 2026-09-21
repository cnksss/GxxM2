using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GXX.Core.Util;

// 源单元：Source/DBServer/uFrmHumanExport.pas（151 行）→ 本文件（1:1 移植）
// DFM：Source/DBServer/uFrmHumanExport.dfm（2,542 字节，**文本 DFM**，直接读原文即可；
//      `_analysis/utf8_mirror` 只对二进制 DFM 不可信 —— 见 §41.3-1）。
//      实测对账：DFM 控件 **10** / 托管字段 **10**；DFM 事件绑定 **2**（两个 OnClick）/ 托管 `+=` **2**。
// 方法对账：原文过程 **3**（ShowFrmHumanExport / btnHumanExportClick / btnMobileNumberExportClick）
//      → 托管 **3**（HumanExportUnit.ShowFrmHumanExport / TFrmHumanExport.btnHumanExportClick /
//        TFrmHumanExport.btnMobileNumberExportClick）。

namespace GXX.DBServer.Forms2;

/// <summary>
/// `uFrmHumanExport.pas:38-48 procedure ShowFrmHumanExport`。
/// 原文：Create(nil) → ShowModal → Free（返回值丢弃）。
/// </summary>
public static class HumanExportUnit
{
    /// <summary>
    /// `uFrmHumanExport.pas:42-46`：Create → ShowModal → Free。
    /// 闸门见 <see cref="DBServerForms2Ui.ShowModal"/>（默认弹真实模态框，测试注入即可脱 UI）。
    /// </summary>
    public static void ShowFrmHumanExport()
    {
        var FrmHumanExport = new TFrmHumanExport();
        try
        {
            DBServerForms2Ui.ShowModal(FrmHumanExport);
        }
        finally
        {
            FrmHumanExport.Dispose();
        }
    }
}

/// <summary>
/// `uFrmHumanExport.pas:11-28 TFrmHumanExport`（DFM: uFrmHumanExport.dfm）。
/// </summary>
public class TFrmHumanExport : Form
{
    // DFM: FrmHumanExport Left=752 Top=432 BorderStyle=bsDialog Caption='导出人物数据'
    //      ClientHeight=161 ClientWidth=299 Color=clBtnFace Font=Tahoma -11 PixelsPerInch=96
    public GroupBox GroupBox1 = null!;
    public Label Label1 = null!;
    public Label Label2 = null!;
    public TSpinEdit seLimitCount = null!;
    public TSpinEdit seMinLevel = null!;
    public Button btnHumanExport = null!;
    public GroupBox GroupBox2 = null!;
    public Button btnMobileNumberExport = null!;
    public RadioButton rbAllMobile = null!;
    public RadioButton rbBindMobile = null!;

    public TFrmHumanExport()
    {
        // DFM: FrmHumanExport Left=752 Top=432 BorderStyle=bsDialog Caption='导出人物数据'
        //      ClientHeight=161 ClientWidth=299 Position=poMainFormCenter PixelsPerInch=96 TextHeight=13
        Text = "导出人物数据";
        StartPosition = FormStartPosition.CenterParent;                 // Position=poMainFormCenter
        Location = new System.Drawing.Point(752, 432);
        ClientSize = new System.Drawing.Size(299, 161);
        FormBorderStyle = FormBorderStyle.FixedDialog;                  // BorderStyle=bsDialog
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: GroupBox1 Left=11 Top=8 Width=278 Height=73 Caption='导出离线挂机人物' TabOrder=0
        GroupBox1 = new GroupBox { Left = 11, Top = 8, Width = 278, Height = 73, Text = "导出离线挂机人物", TabIndex = 0 };
        // DFM: Label1 Left=12 Top=20 Width=84 Height=13 Caption='限制导出数量：'
        Label1 = new Label { Left = 12, Top = 20, Width = 84, Height = 13, Text = "限制导出数量：" };
        // DFM: Label2 Left=12 Top=44 Width=84 Height=13 Caption='最低人物等级：'
        Label2 = new Label { Left = 12, Top = 44, Width = 84, Height = 13, Text = "最低人物等级：" };
        // DFM: seLimitCount Left=94 Top=16 Width=88 Height=22 MaxValue=0 MinValue=0 TabOrder=0 Value=100
        seLimitCount = new TSpinEdit { Left = 94, Top = 16, Width = 88, Height = 22, TabIndex = 0 };
        seLimitCount.SetDfmRange(0, 0);
        seLimitCount.Value = 100;
        // DFM: seMinLevel Left=94 Top=40 Width=88 Height=22 MaxValue=0 MinValue=0 TabOrder=1 Value=20
        seMinLevel = new TSpinEdit { Left = 94, Top = 40, Width = 88, Height = 22, TabIndex = 1 };
        seMinLevel.SetDfmRange(0, 0);
        seMinLevel.Value = 20;
        // DFM: btnHumanExport Left=193 Top=14 Width=75 Height=25 Caption='导出' TabOrder=2 OnClick=btnHumanExportClick
        btnHumanExport = new Button { Left = 193, Top = 14, Width = 75, Height = 25, Text = "导出", TabIndex = 2 };

        // DFM: GroupBox2 Left=11 Top=88 Width=278 Height=65 Caption='导出人物手机号' TabOrder=1
        GroupBox2 = new GroupBox { Left = 11, Top = 88, Width = 278, Height = 65, Text = "导出人物手机号", TabIndex = 1 };
        // DFM: btnMobileNumberExport Left=193 Top=17 Width=75 Height=25 Caption='导出' TabOrder=0 OnClick=btnMobileNumberExportClick
        btnMobileNumberExport = new Button { Left = 193, Top = 17, Width = 75, Height = 25, Text = "导出", TabIndex = 0 };
        // DFM: rbAllMobile Left=8 Top=21 Width=113 Height=17 Caption='导出所有手机号' TabOrder=1
        rbAllMobile = new RadioButton { Left = 8, Top = 21, Width = 113, Height = 17, Text = "导出所有手机号", TabIndex = 1 };
        // DFM: rbBindMobile Left=8 Top=42 Width=137 Height=17
        //      Hint='绑定的手机号是经过验证码验证的' Caption='导出已绑定的手机号' Checked=True
        //      ParentShowHint=False ShowHint=True TabOrder=2 TabStop=True
        rbBindMobile = new RadioButton
        {
            Left = 8,
            Top = 42,
            Width = 137,
            Height = 17,
            Text = "导出已绑定的手机号",
            Checked = true,
            TabIndex = 2,
            TabStop = true,
        };
        var toolTip = new ToolTip();
        toolTip.SetToolTip(rbBindMobile, "绑定的手机号是经过验证码验证的");

        GroupBox1.Controls.Add(Label1);
        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(seLimitCount);
        GroupBox1.Controls.Add(seMinLevel);
        GroupBox1.Controls.Add(btnHumanExport);

        GroupBox2.Controls.Add(btnMobileNumberExport);
        GroupBox2.Controls.Add(rbAllMobile);
        GroupBox2.Controls.Add(rbBindMobile);

        Controls.Add(GroupBox1);
        Controls.Add(GroupBox2);

        // DFM 绑定（2/2）：
        btnHumanExport.Click += (s, e) => btnHumanExportClick(s);
        btnMobileNumberExport.Click += (s, e) => btnMobileNumberExportClick(s);
    }

    /// <summary>
    /// `uFrmHumanExport.pas:50-106 btnHumanExportClick` 1:1。
    ///
    /// <para>逐段保真：</para>
    /// <list type="number">
    /// <item>先 `btnHumanExport.Enabled := False`，整段包在 `try … finally Enabled := True` 里；</item>
    /// <item>`SL` 与 `TempRoleList` 两层 try/finally（TempRoleList 先 Free）；</item>
    /// <item>`SearchByLevel(seLimitCount.Value, seMinLevel.Value, TempRoleList)` —— **形参顺序就是
    ///   (限制数量, 最低等级)**（RoleDB.pas:714-730 的形参名是 `LimitCount, MinLevel`，与窗体控件同名同序）；</item>
    /// <item>每行 = `RoleName + #9 + Account`（制表符分隔）；</item>
    /// <item>保存对话框：Title='导出离线挂机人物'、Filter='AutoLoadOffline|*.txt'、FileName='AutoLoadOffline.txt'；
    ///   ★ 原文缺陷：`Filter` 缺 `|描述|模式` 的两段式写法中的**描述段**（'AutoLoadOffline' 就是描述，模式是 '*.txt'），
    ///   实际可用，但标题/过滤串都是写死的英文字面量（照抄，不改）；</item>
    /// <item>Execute 为真时：`ExtractFileExt(FileName) &lt;&gt; '.txt'` ⇒ `ChangeFileExt(FileName, '.txt')`
    ///   （大小写敏感：`.TXT` 也会被改成 `.txt`）；</item>
    /// <item>`SL.SaveToFile(FileName)` 外层 try/except：成功弹 `提示信息`（MB_ICONQUESTION）、
    ///   异常弹 `错误信息`（MB_ICONERROR）并**吞掉**异常（不中断 finally 链）。</item>
    /// </list>
    /// <para>
    /// 托管差异 D-P10-07：`SaveDialog` 用可注入替身 <see cref="TSaveDialogSeam"/>
    /// （原文 `TSaveDialog.Create(nil)` + `Execute`；托管 `SaveFileDialog.ShowDialog`）。
    /// </para>
    /// </summary>
    public void btnHumanExportClick(object? Sender)
    {
        btnHumanExport.Enabled = false;

        var SL = new TStringList();
        try
        {
            var TempRoleList = new TSerarchRoleList();
            try
            {
                SelectClientRoleDbSeam.RequireHuman.SearchByLevel(seLimitCount.Value, seMinLevel.Value, TempRoleList);
                for (int I = 0; I <= TempRoleList.Count - 1; I++)
                {
                    TSerarchRoleData? RoleData = TempRoleList.Items(I);

                    SL.Add(RoleData!.RoleName + "\t" + RoleData.Account);   // #9 = TAB
                }
            }
            finally
            {
                // TempRoleList.Free（托管由 GC 接管）
            }

            TSaveDialogSeam SaveDialog = DBServerForms2Ui.CreateSaveDialog();
            SaveDialog.Title = "导出离线挂机人物";
            SaveDialog.Filter = "AutoLoadOffline|*.txt";
            SaveDialog.FileName = "AutoLoadOffline.txt";
            if (SaveDialog.Execute())
            {
                string FileName = SaveDialog.FileName;
                if (P10bFileUtils.ExtractFileExt(FileName) != ".txt")
                    FileName = P10bFileUtils.ChangeFileExt(FileName, ".txt");

                try
                {
                    SL.SaveToFile(FileName);
                    UiSeam.MessageBox(FileName, "提示信息", TMsgBox.MB_ICONQUESTION);
                }
                catch (Exception e)
                {
                    UiSeam.MessageBox(e.Message, "错误信息", TMsgBox.MB_ICONERROR);
                }
            }
        }
        finally
        {
            btnHumanExport.Enabled = true;
        }
    }

    /// <summary>
    /// `uFrmHumanExport.pas:108-149 btnMobileNumberExportClick` 1:1。
    ///
    /// <para>
    /// 与上一个处理器的三处差异（原文如此）：
    /// ① 没有 `TempRoleList` 这一层（直接把 `SL` 交给 DB）；
    /// ② Filter 是两段式 `'文本文件(*.txt)|*.txt'`、FileName=`'手机号码.txt'`、Title=`'导出人物手机号码'`；
    /// ③ 传给 DB 的是 `rbBindMobile.Checked`（**已绑定**为真 ⇒ 只导已绑定）。
    /// 原文残留：`:110` 的 `//I: Integer;` 是被注释掉的声明（无行为）。
    /// </para>
    /// <para>
    /// 托管差异 D-P10-09：`THumanDBBase.GetMobileNumbers(bool, List&lt;string&gt;)` 的托管签名收
    /// `List&lt;string&gt;`（原文是 `TStrings`）；本方法用临时 `List&lt;string&gt;` 承接后逐条 `SL.Add`，
    /// 落地字节与原文一致（每行一条号码，CRLF）。
    /// </para>
    /// </summary>
    public void btnMobileNumberExportClick(object? Sender)
    {
        btnMobileNumberExport.Enabled = false;

        var SL = new TStringList();
        try
        {
            var numbers = new List<string>();
            SelectClientRoleDbSeam.RequireHuman.GetMobileNumbers(rbBindMobile.Checked, numbers);
            foreach (string number in numbers) SL.Add(number);

            TSaveDialogSeam SaveDialog = DBServerForms2Ui.CreateSaveDialog();
            SaveDialog.Title = "导出人物手机号码";
            SaveDialog.Filter = "文本文件(*.txt)|*.txt";
            SaveDialog.FileName = "手机号码.txt";
            if (SaveDialog.Execute())
            {
                string FileName = SaveDialog.FileName;
                if (P10bFileUtils.ExtractFileExt(FileName) != ".txt")
                {
                    FileName = P10bFileUtils.ChangeFileExt(FileName, ".txt");
                }

                try
                {
                    SL.SaveToFile(FileName);
                    UiSeam.MessageBox(FileName, "提示信息", TMsgBox.MB_ICONQUESTION);
                }
                catch (Exception e)
                {
                    UiSeam.MessageBox(e.Message, "错误信息", TMsgBox.MB_ICONERROR);
                }
            }
        }
        finally
        {
            btnMobileNumberExport.Enabled = true;
        }
    }
}
