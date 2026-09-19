using System;
using System.Drawing;
using System.Windows.Forms;

namespace GXX.DBServer;

// CreateId.pas (1-35) → CreateId.cs
// 原文如此：本单元**只有窗体骨架**，没有任何 ID/帐号生成逻辑（不生成位数、不做唯一性/回绕/冲突处理），
// 也没有被 DBServer 任何其它单元引用（CreateId.pas:24 的 FrmCreateId 全局在 DBServer.dpr 之外无人使用）。
// 真正的角色 ID 分配在 RoleDB/SqliteRoleDB(DoGetID/NewHuman 的自增 ID) 一侧，属角色 DB 车道。
// 因此本文件 1:1 只移植窗体与 FormShow，不做任何"顺手补全"。

/// <summary>CreateId.pas:23-24 `var FrmCreateId: TFrmCreateId;`（Application.CreateForm 赋值）。</summary>
public static class CreateIdGlobal
{
    public static FrmCreateId FrmCreateId;
}

/// <summary>CreateId.pas:9-21 `TFrmCreateId`（DFM: CreateId.dfm，DeDe 二进制 DFM，脚本 DfmToText 还原后逐条对齐）。</summary>
public class FrmCreateId : Form
{
    // DFM: FrmCreateId Left=604 Top=256 BorderIcons=[biSystemMenu, biMinimize] BorderStyle=bsSingle
    //      Caption='新建帐号' ClientHeight=135 ClientWidth=237 Color=clBtnFace
    //      Font.Charset=ANSI_CHARSET Font.Color=clWindowText Font.Height=-12 Font.Name='宋体'
    //      OnShow=FormShow PixelsPerInch=96 TextHeight=12
    public TextBox EdId;
    public TextBox EdPasswd;
    public Label Label1;
    public Label Label2;
    public Button BitBtn1;   // DFM: Kind=bkOK
    public Button BitBtn2;   // DFM: Kind=bkCancel

    public FrmCreateId()
    {
        // DFM: FrmCreateId Left=604 Top=256 BorderStyle=bsSingle Caption='新建帐号' ClientHeight=135 ClientWidth=237
        Text = "新建帐号";
        StartPosition = FormStartPosition.Manual;
        Location = new Point(604, 256);
        ClientSize = new Size(237, 135);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;        // BorderIcons = [biSystemMenu, biMinimize] → 无 biMaximize
        MinimizeBox = true;
        ShowInTaskbar = true;

        // DFM: Label1 Left=23 Top=26 Width=30 Height=12 Caption='帐号:'
        Label1 = new Label { Left = 23, Top = 26, Width = 30, Height = 12, Text = "帐号:" };
        // DFM: Label2 Left=23 Top=56 Width=30 Height=12 Caption='密码:'
        Label2 = new Label { Left = 23, Top = 56, Width = 30, Height = 12, Text = "密码:" };
        // DFM: EdId Left=60 Top=22 Width=149 Height=20 TabOrder=0
        EdId = new TextBox { Left = 60, Top = 22, Width = 149, Height = 20, TabIndex = 0 };
        // DFM: EdPasswd Left=60 Top=51 Width=149 Height=20 TabOrder=1
        EdPasswd = new TextBox { Left = 60, Top = 51, Width = 149, Height = 20, TabIndex = 1 };
        // DFM: BitBtn1 Left=19 Top=88 Width=92 Height=27 Caption='确定(&O)' TabOrder=2 Kind=bkOK
        BitBtn1 = new Button { Left = 19, Top = 88, Width = 92, Height = 27, Text = "确定(&O)", TabIndex = 2, DialogResult = DialogResult.OK };
        // DFM: BitBtn2 Left=124 Top=88 Width=92 Height=27 TabOrder=3 Kind=bkCancel
        BitBtn2 = new Button { Left = 124, Top = 88, Width = 92, Height = 27, Text = "取消", TabIndex = 3, DialogResult = DialogResult.Cancel };

        // DFM: FrmCreateId OnShow=FormShow
        Shown += (s, e) => FormShow(s, e);

        Controls.Add(Label1);
        Controls.Add(Label2);
        Controls.Add(EdId);
        Controls.Add(EdPasswd);
        Controls.Add(BitBtn1);
        Controls.Add(BitBtn2);
    }

    /// <summary>CreateId.pas:30-33 `TFrmCreateId.FormShow`：EdId.SetFocus。</summary>
    public void FormShow(object Sender, EventArgs e)
    {
        EdId.Focus();
    }
}
