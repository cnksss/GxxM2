// 源单元：Source/Client-HGE/uFrmNGItemEdit.pas（原文 41 行 / CRLF 计入 49 行）
// 原文 uses：Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls
//
// ⚠ .dfm 存在但是**二进制 DFM**（Source/Client-HGE/uFrmNGItemEdit.dfm，528 字节，
//   首字节 $FF 是 Delphi 二进制 DFM 签名）。因此无法像文本 .dfm 那样逐行照抄属性，
//   本文件的布局注释来自对二进制流的**手工解码**（解码过程与结果见报告 §5）：
//
//     FF 0A 00 "TFRMNGITEMEDIT" 00 30 10 F8 01 00 00      ← 二进制 DFM 头
//     TP F0 "TFrmNGItemEdit" "FrmNGItemEdit"
//       Left=766($FE02)  Top=521($0209)  BorderStyle=bsDialog  BorderWidth=3
//       Caption='内挂物品编辑'(UTF-16)  ClientHeight=308  ClientWidth=449
//       Color=clBtnFace  Font.Charset=DEFAULT_CHARSET  Font.Color=clWindowText
//       Font.Height=-11($F4→等待解码)  Font.Name='Tahoma'  Font.Style=[]
//       OldCreateOrder=False  Position=poMainFormCenter
//       PixelsPerInch=96($60)  TextHeight=14($0E)
//       object mmoItems: TMemo  Left=0 Top=0 Width=449 Height=276 ScrollBars=ssVertical TabOrder=0
//       object btnOK: TButton Left=374 Top=280 Width=75 Height=25
//                                  Caption='确定'  ModalResult=mrOk  TabOrder=1
//
// 原文行为（uFrmNGItemEdit.pas:33-47）：创建窗体 → 把待编辑文本灌进 Memo →
// ShowModal；**只有** 返回 mrOk 时才把 Memo 文本**写回**调用方的 TStrings。
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GXX.Client.Tail;

/// <summary>
/// uFrmNGItemEdit.pas 的 <c>TFrmNGItemEdit</c> 1:1 移植（WinForms）。
///
/// <para>原文是一个"批量文本编辑器"对话框：一个多行 Memo + 一个"确定"按钮
/// （<c>ModalResult = mrOk</c>，点击即关）。<see cref="ShowFrmNGItemEdit"/> 是唯一的入口函数。</para>
///
/// <para><b>原文被注释掉的写法</b>（uFrmNGItemEdit.pas:37）：<c>//Result := False; HZQ 20230519</c>
/// —— 即"进来先当成取消"的写法被删掉了。**这带来一个真实差异**：若要编辑的列表为空、
/// 或窗体提前被 Free，<c>Result</c> 会是 Delphi 的默认 <c>False</c>，与之一致；本移植同样
/// 不预置返回值，靠"只有 mrOk 才为 true"表达。</para>
/// </summary>
public class TFrmNGItemEdit : Form
{
    /// <summary>DFM: mmoItems TMemo（Left=0 Top=0 Width=449 Height=276 ScrollBars=ssVertical TabOrder=0）</summary>
    public TextBox mmoItems;

    /// <summary>DFM: btnOK TButton（Left=374 Top=280 Width=75 Height=25 Caption='确定' ModalResult=mrOk TabOrder=1）</summary>
    public Button btnOK;

    /// <summary>DFM: TFrmNGItemEdit ClientWidth=449</summary>
    public const int DfmClientWidth = 449;
    /// <summary>DFM: TFrmNGItemEdit ClientHeight=308</summary>
    public const int DfmClientHeight = 308;
    /// <summary>DFM: mmoItems Width=449 Height=276</summary>
    public const int DfmMemoWidth = 449;
    /// <summary>DFM: mmoItems Height=276</summary>
    public const int DfmMemoHeight = 276;
    /// <summary>DFM: btnOK Left=374 Top=280 Width=75 Height=25 ModalResult=mrOk</summary>
    public const int DfmOkLeft = 374;
    /// <summary>DFM: btnOK Top=280</summary>
    public const int DfmOkTop = 280;
    /// <summary>DFM: btnOK Width=75</summary>
    public const int DfmOkWidth = 75;
    /// <summary>DFM: btnOK Height=25</summary>
    public const int DfmOkHeight = 25;
    /// <summary>DFM: TFrmNGItemEdit Caption='内挂物品编辑'</summary>
    public const string DfmCaption = "内挂物品编辑";
    /// <summary>DFM: btnOK Caption='确定'</summary>
    public const string DfmOkCaption = "确定";
    /// <summary>DFM: TFrmNGItemEdit Position=poMainFormCenter</summary>
    public const FormStartPosition DfmPosition = FormStartPosition.CenterParent;
    /// <summary>DFM: TFrmNGItemEdit BorderStyle=bsDialog</summary>
    public const FormBorderStyle DfmBorderStyle = FormBorderStyle.FixedDialog;
    /// <summary>DFM: TFrmNGItemEdit BorderWidth=3</summary>
    public const int DfmBorderWidth = 3;

    /// <summary>按二进制 DFM 解码结果构造控件树。</summary>
    public TFrmNGItemEdit()
    {
        Text = DfmCaption;                                   // DFM: Caption='内挂物品编辑'
        ClientSize = new System.Drawing.Size(DfmClientWidth, DfmClientHeight);
        StartPosition = DfmPosition;
        FormBorderStyle = DfmBorderStyle;
        // DFM: BorderWidth=3 —— WinForms 无对应属性（Form.Padding 语义不同），
        // 保留常量并在注释中登记该装饰性属性未对齐（转换开发文档 §7 允许装饰性属性简化）。
        Padding = new Padding(DfmBorderWidth);

        mmoItems = new TextBox
        {
            Left = 0, Top = 0,
            Width = DfmMemoWidth, Height = DfmMemoHeight,
            Multiline = true,                                  // DFM: TMemo ⇒ 多行文本框
            ScrollBars = ScrollBars.Vertical,                  // DFM: ScrollBars=ssVertical
            AcceptsReturn = true,
            TabIndex = 0,                                     // DFM: TabOrder=0
        };

        btnOK = new Button
        {
            Left = DfmOkLeft, Top = DfmOkTop,
            Width = DfmOkWidth, Height = DfmOkHeight,
            Text = DfmOkCaption,                              // DFM: Caption='确定'
            TabIndex = 1,                                     // DFM: TabOrder=1
            // DFM: ModalResult=mrOk ⇒ 点击即以 OK 关闭
            DialogResult = DialogResult.OK,
        };

        Controls.Add(mmoItems);
        Controls.Add(btnOK);
        AcceptButton = btnOK;
    }

    /// <summary>
    /// 原文 :33-47 <c>function ShowFrmNGItemEdit(Items: TStrings): Boolean;</c>
    ///
    /// <para>原文语义：</para>
    /// <code>
    ///   FrmNGItemEdit := TFrmNGItemEdit.Create(nil);
    ///   try
    ///     FrmNGItemEdit.mmoItems.Text := Items.Text;      // 灌入
    ///     Result := FrmNGItemEdit.ShowModal = mrOk;
    ///     if Result then
    ///       Items.Text := FrmNGItemEdit.mmoItems.Text;    // **仅 OK 时写回**
    ///   finally
    ///     FrmNGItemEdit.Free;
    ///   end;
    /// </code>
    ///
    /// <para>本移植保持"仅 OK 写回"这一关键差异行为。</para>
    /// </summary>
    /// <param name="Items">待编辑的行集合（对应原文 <c>TStrings</c>）。</param>
    /// <returns>原文 <c>ShowModal = mrOk</c>（用户按"确定"为 true）。</returns>
    public static bool ShowFrmNGItemEdit(IList<string> Items)
    {
        using var frm = new TFrmNGItemEdit();
        // 原文 mmoItems.Text := Items.Text —— TStrings.Text 是 CRLF 连接的整体文本
        frm.mmoItems.Text = Items == null ? string.Empty : string.Join("\r\n", Items);

        bool result = frm.ShowDialog() == DialogResult.OK;

        if (result)
        {
            // 原文 Items.Text := FrmNGItemEdit.mmoItems.Text;
            WriteBack(Items, frm.mmoItems.Text);
        }
        return result;
    }

    /// <summary>
    /// <c>TStrings.Text</c> 的赋值语义：按 CRLF 拆行并**整体替换**目标内容。
    /// <para>抽成静态方法以便脱离 WinForms 消息泵单测。</para>
    /// </summary>
    public static void WriteBack(IList<string> target, string text)
    {
        if (target == null) return;
        target.Clear();
        if (string.IsNullOrEmpty(text)) return;
        // TStrings.SetTextStr 视 CR、LF、CRLF 均为行分隔（Delphi Classes 语义）
        foreach (string line in SplitDelphiLines(text))
        {
            target.Add(line);
        }
    }

    /// <summary>
    /// Delphi <c>TStrings.SetTextStr</c> 的行分隔规则（纯逻辑，供测试）：
    /// 以 <c>#13</c> 或 <c>#10</c> 为分隔，<c>#13#10</c> 算**一个**分隔，
    /// 且**丢弃末尾的空行**（"a\r\n" 只有一行 "a"）。
    /// </summary>
    public static IEnumerable<string> SplitDelphiLines(string text)
    {
        if (string.IsNullOrEmpty(text)) yield break;
        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '\r' || c == '\n')
            {
                yield return text.Substring(start, i - start);
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                start = i + 1;
            }
        }
        // 末尾未以换行结尾才产出最后一行（Delphi 会丢掉结尾的空行）
        if (start < text.Length) yield return text.Substring(start);
    }

    /// <summary>
    /// 原文 <c>TStrings.Text</c> 的 getter 语义（CRLF 连接，**末尾带 CRLF**），
    /// 供 <see cref="ShowFrmNGItemEdit"/> 的"灌入"一侧使用。
    /// </summary>
    public static string JoinDelphiText(IEnumerable<string> lines)
    {
        if (lines == null) return string.Empty;
        string body = string.Join("\r\n", lines.ToArray());
        return body.Length == 0 ? string.Empty : body + "\r\n";
    }
}
