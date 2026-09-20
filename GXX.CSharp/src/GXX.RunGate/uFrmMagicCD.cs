using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmMagicCD.pas 1:1 转换（Source\RunGate\uFrmMagicCD.pas，583 行 / LF 582）。
// 布局真源：Source\RunGate\uFrmMagicCD.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：从技能数据库（Magic.DB，Paradox）读出技能列表 → 在树/表里编辑每个技能的冷却时间
// （毫秒）→ 确定时写回 `g_MagicCDList` 并 SaveToFile；同时保存"冷却未到"提示的
// 内容/类型/前景色/背景色/显示坐标。
//
// 接缝：
//   * IMagicDbReader      ← ParadoxDataSet（GXX.RunGate 的 ParadoxConv 已有部分产物，
//                            但 TParadoxDataSet 未移植；此处定义最小读取接口）
//   * MagicCDLogic        ← 原 :337-398 DoOpen 的过滤/去重/取值（纯函数）
//   * MagicCDLogic.Save   ← 原 :456-504 btnOKClick 的落盘（纯逻辑）
//
// ★ 原文要点与缺陷（照抄 + 差异断言）：
//   D1. `DoOpen`（原 :361-391）遍历 RecordCount 行：
//       * `MagicID > 0` 才处理（原 :364）；
//       * `Descr` **大小写不敏感**地排除 `'英雄'` / `'静之'` / `'怒之'` 三个词（`SameText`，原 :367-369）；
//       * `if List.IndexOf(Pointer(MagicID)) < 0` 做去重（原 :371）——
//         注意 `List.Add` 加的是**重新读取的** `DateSet.FieldByName('MagId').AsInteger`（原 :385），
//         与上面缓存的 `MagicID` 是同一个字段，等价。
//   D2. `FieldByName('Descr')` 若字段不存在，ParadoxDataSet 会抛异常 —— 原文**没有 try..except**，
//       整个 DoOpen 会中断（只有 DateSet/List 的 finally 释放）。托管侧同样不吞异常。
//   D3. `vstMagicCDFreeNode`（原 :328-335）在节点释放时把 `MagicName := ''` ——
//       这只是清空一个即将销毁节点的字段，**没有实际效果**（原文冗余）。
//   D4. `btnOKClick`（原 :456-504）先写 INI（6 键），**再** `vstMagicCD.EndEditNode`，
//       然后才遍历节点重建 `g_MagicCDList`。即：如果编辑框还开着，其值**会**被 EndEditNode 落进节点后
//       一并保存（顺序正确）；但 `EndEditNode` 的返回值被丢弃。
//   D5. `btnOKClick` 重建 `g_MagicCDList` 时 `Clear` + 逐节点 `Add(MagicID)`；
//       若 `Add` 返回 nil（MagicID 重复）则**跳过 Interval 赋值**（原 :492-493）。
//   D6. `btnSearchClick`（原 :506-535）用 `Pos(MagicName, NodeData.MagicName) > 0` 子串匹配，
//       **区分大小写**（与 uFrmProcessBlacklist 的 UpperCase 双侧不同）；搜索文本先 Trim（原 :512）。
//   D7. `btnSearchNextClick`（原 :537-571）从 **当前焦点节点的下一个**开始；
//       当前无焦点节点时从第一个开始；**不回绕**（搜到末尾就结束）。
//   D8. `ShowFrmMaigcCD`（原 :270-307）函数名拼写为 `Maigc`（**g 与 c 颠倒**），
//       且 DB 路径由 `ExtractFileDir(ParamStr(0))` 再 `ExtractFilePath` 拼 `'Mud2\DB\Magic.DB'`
//       —— 等价于"exe 所在目录的上一级的 Mud2\DB\Magic.DB"。文件不存在时弹 OpenDialog 让用户指定。
//   D9. `FormCreate`（原 :309-312）只设置 `NodeDataSize`；真正的数据加载在 `DoOpen`。
// =====================================================================================

/// <summary>技能数据库读取接缝（原 ParadoxDataSet.FieldByName('MagId'/'Descr'/'MagName')）。</summary>
public interface IMagicDbReader
{
    int RecordCount { get; }

    /// <summary>`DateSet.First` + 迭代；每次 MoveNext 返回是否还有记录。</summary>
    void First();

    /// <summary>`DateSet.Next`；返回 false 表示已到末尾之后。</summary>
    bool Next();

    /// <summary>`DateSet.FieldByName('MagId').AsInteger`。</summary>
    int GetMagId();

    /// <summary>`DateSet.FieldByName('Descr').AsString`。</summary>
    string GetDescr();

    /// <summary>`DateSet.FieldByName('MagName').AsString`。</summary>
    string GetMagName();
}

/// <summary>`DoOpen` 过滤后的一条技能记录。</summary>
public class MagicCDRow
{
    public int MagicID;         // 原 :376 NodeData.MagicID
    public string MagicName = ""; // 原 :377 NodeData.MagicName
    public uint CDTime;         // 原 :381/:383 NodeData.CDTime
}

/// <summary>uFrmMagicCD.pas 的非 UI 逻辑（可单测）。</summary>
public static class MagicCDLogic
{
    /// <summary>原 :367-369 排除的三个 Descr 值（`SameText` = 大小写不敏感）。</summary>
    public static readonly string[] ExcludedDescrs = { "英雄", "静之", "怒之" };

    /// <summary>原 :367-369 `not SameText(MagicDescr, '英雄') and not SameText(..., '静之') and not SameText(..., '怒之')`。</summary>
    public static bool IsDescrAllowed(string descr)
    {
        foreach (string s in ExcludedDescrs)
        {
            // Delphi SysUtils.SameText：大小写不敏感 + 忽略首尾空白（此处用 Trim + OrdinalIgnoreCase 等价）
            if (string.Equals((descr ?? "").Trim(), s, StringComparison.OrdinalIgnoreCase)) return false;
        }
        return true;
    }

    /// <summary>
    /// 原 :337-398 `DoOpen` 的数据部分（不碰控件）：读全部记录 → 过滤 → 去重 → 取 CD。
    /// `magicCdList` 用于回填已保存的 CD（`g_MagicCDList.Find(MagicID)`）。
    /// </summary>
    public static List<MagicCDRow> ReadMagicRows(IMagicDbReader reader, TMagicIntervalList magicCdList)
    {
        var rows = new List<MagicCDRow>();
        var seen = new HashSet<int>();                       // 原 :344 `List: TList`（IndexOf 去重）

        reader.First();                                      // 原 :360
        for (int i = 0; i < reader.RecordCount; i++)         // 原 :361
        {
            int magicId = reader.GetMagId();                 // 原 :363
            if (magicId > 0)                                 // 原 :364
            {
                string descr = reader.GetDescr();            // 原 :366
                if (IsDescrAllowed(descr))                   // 原 :367-369
                {
                    if (!seen.Contains(magicId))             // 原 :371 `List.IndexOf(Pointer(MagicID)) < 0`
                    {
                        var row = new MagicCDRow
                        {
                            MagicID = magicId,               // 原 :376
                            MagicName = reader.GetMagName()  // 原 :377
                        };
                        var interval = magicCdList.Find(magicId);        // 原 :379
                        row.CDTime = interval != null ? interval.Interval : 0;   // 原 :381 / :383
                        rows.Add(row);
                        seen.Add(magicId);                   // 原 :385
                    }
                }
            }
            reader.Next();                                   // 原 :390
        }
        return rows;
    }

    /// <summary>原 :314-326 `vstMagicCDGetText` 的取文本规则（0=MagId，1=MagName，2=CDTime）。</summary>
    public static string GetCellText(MagicCDRow row, int column)
    {
        switch (column)
        {
            case 0: return DelphiRTL.IntToStr(row.MagicID);       // 原 :322
            case 1: return row.MagicName;                         // 原 :323
            case 2: return DelphiRTL.IntToStr((int)row.CDTime);   // 原 :324
            default: return "";
        }
    }

    /// <summary>原 :419-423 `vstMagicCDEditing`：只允许编辑第 2 列（CDTime）。</summary>
    public static bool CanEditColumn(int column) => column == 2;

    /// <summary>原 :412-416 `vstMagicCDBeforeItemErase`：奇数行（Node.Index mod 2 &lt;&gt; 0）使用 $00F9F9F9 底色。</summary>
    public static bool ShouldEraseAltRow(int nodeIndex) => nodeIndex % 2 != 0;
    public static Color AltRowColor => Color.FromArgb(0xF9, 0xF9, 0xF9);   // 原 :415 ItemColor := $00F9F9F9

    /// <summary>原 :506-535 `btnSearchClick`：**区分大小写**的子串匹配，文本先 Trim。
    /// ★ 与 GXX.Core.Rtl.DelphiRTL.Pos 的差异：Delphi 的 `Pos('', S)` 返回 1，
    ///   而 DelphiRTL.Pos 对空子串返回 0 —— 这里显式补上"空串命中"以复刻原语义。</summary>
    public static int FindFirst(IReadOnlyList<MagicCDRow> rows, string searchText)
    {
        string magicName = DelphiRTL.Trim(searchText ?? "");          // 原 :512
        if (magicName.Length == 0) return -1;                         // 原 :513（调用方负责弹窗）
        for (int i = 0; i < rows.Count; i++)                          // 原 :520-534
        {
            if (DelphiRTL.Pos(magicName, rows[i].MagicName) > 0)       // 原 :525
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 原 :537-571 `btnSearchNextClick`：从"当前焦点节点的下一个"开始，**不回绕**。
    /// `focusedIndex` = -1（无焦点）时从 0 开始。返回命中下标，未命中 -1。
    /// </summary>
    public static int FindNext(IReadOnlyList<MagicCDRow> rows, int focusedIndex, string searchText)
    {
        string magicName = DelphiRTL.Trim(searchText ?? "");          // 原 :543
        if (magicName.Length == 0) return -1;                         // 原 :544

        int start = focusedIndex < 0 ? 0 : focusedIndex + 1;          // 原 :551-555
        for (int i = start; i < rows.Count; i++)                      // 原 :557-570
        {
            if (DelphiRTL.Pos(magicName, rows[i].MagicName) > 0)       // 原 :561
                return i;
        }
        return -1;
    }

    /// <summary>Delphi `Pos` 的空串语义补丁：空子串恒命中（Delphi 返回 1，DelphiRTL 返回 0）。</summary>
    public static bool ContainsSub(string needle, string haystack)
        => needle.Length == 0 || DelphiRTL.Pos(needle, haystack ?? "") > 0;

    /// <summary>原 :463-468：控件 → 6 个全局量。</summary>
    public static void ApplyGlobalsFromUi(int msgType, string msgText, byte fColor, byte bColor, int showX, int showY)
    {
        FormGlobals.g_btMagicCDMsgType = (byte)msgType;      // 原 :463
        FormGlobals.g_sMagicCDMsgText = msgText ?? "";        // 原 :464
        FormGlobals.g_btMagicCDFColor = fColor;              // 原 :465
        FormGlobals.g_btMagicCDBColor = bColor;              // 原 :466
        FormGlobals.g_nMagicCDShowX = showX;                 // 原 :467
        FormGlobals.g_nMagicCDShowY = showY;                 // 原 :468
    }

    /// <summary>原 :470-480：写 INI `[MagicCD]` 6 键（**键序即原文顺序**）。</summary>
    public static void WriteIni(string iniFileName)
    {
        var ini = new TIniFileEx(iniFileName);                                            // 原 :470
        try
        {
            ini.WriteInteger("MagicCD", "MsgType", FormGlobals.g_btMagicCDMsgType);       // 原 :472
            ini.WriteString("MagicCD", "MsgText", FormGlobals.g_sMagicCDMsgText);         // 原 :473
            ini.WriteInteger("MagicCD", "FColor", FormGlobals.g_btMagicCDFColor);         // 原 :474
            ini.WriteInteger("MagicCD", "BColor", FormGlobals.g_btMagicCDBColor);         // 原 :475
            ini.WriteInteger("MagicCD", "ShowX", FormGlobals.g_nMagicCDShowX);            // 原 :476
            ini.WriteInteger("MagicCD", "ShowY", FormGlobals.g_nMagicCDShowY);            // 原 :477
        }
        finally
        {
            ini.Dispose();                                                                // 原 :479 IniFile.Free
        }
    }

    /// <summary>
    /// 原 :483-501：按界面行重建 `g_MagicCDList`（Clear → 逐行 Add → 赋值 Interval），
    /// 返回被跳过的重复 MagicID（Add 返回 nil 的项）。
    /// </summary>
    public static List<int> RebuildMagicCdList(IReadOnlyList<MagicCDRow> rows, TMagicIntervalList magicCdList)
    {
        var skipped = new List<int>();
        magicCdList.Lock();                                        // 原 :483
        try
        {
            magicCdList.Clear();                                   // 原 :485
            foreach (var row in rows)                              // 原 :486-496
            {
                var interval = magicCdList.Add(row.MagicID);       // 原 :491
                if (interval != null)
                    interval.Interval = row.CDTime;                // 原 :493
                else
                    skipped.Add(row.MagicID);
            }
        }
        finally
        {
            magicCdList.UnLock();                                  // 原 :498
        }
        return skipped;
    }

    /// <summary>原 :456-504 `btnOKClick` 的完整非 UI 流程（不含 EndEditNode）。</summary>
    public static void ButtonOK(IReadOnlyList<MagicCDRow> rows, string iniFileName, string magicCdListFileName,
                                int msgType, string msgText, byte fColor, byte bColor, int showX, int showY)
    {
        ApplyGlobalsFromUi(msgType, msgText, fColor, bColor, showX, showY);   // 原 :463-468
        WriteIni(iniFileName);                                                // 原 :470-480
        RebuildMagicCdList(rows, FormGlobals.g_MagicCDList);                  // 原 :483-499
        FormGlobals.g_MagicCDList.SaveToFile(magicCdListFileName);            // 原 :501
    }

    /// <summary>原 :278-279 `ShowFrmMaigcCD` 的 DB 路径推导：
    /// `S := ExtractFileDir(ParamStr(0)); S := ExtractFilePath(S) + 'Mud2\DB\Magic.DB';`</summary>
    public static string ComputeDefaultMagicDbPath(string exePath)
    {
        // ExtractFileDir(ParamStr(0)) 去掉文件名；ExtractFilePath(...) 再去掉一层目录
        string exeDir = System.IO.Path.GetDirectoryName(exePath ?? "") ?? "";
        string parent = System.IO.Path.GetDirectoryName(exeDir.TrimEnd(System.IO.Path.DirectorySeparatorChar,
                                                                       System.IO.Path.AltDirectorySeparatorChar)) ?? "";
        return System.IO.Path.Combine(parent, "Mud2", "DB", "Magic.DB");
    }
}

/// <summary>原 :270-307 `function ShowFrmMaigcCD: Boolean;`（函数名拼写照抄原文的 `Maigc`）。</summary>
public static class MagicCDUnit
{
    /// <summary>可注入的"默认 DB 是否存在"与"选择 DB 文件"接缝（默认走 File.Exists + OpenFileDialog）。</summary>
    public static Func<string, bool> FileExistsProbe = p => System.IO.File.Exists(p);

    public static Func<string, string> ChooseMagicDb = defaultChoose;

    private static string defaultChoose(string initialFileName)
    {
        using var dlg = new OpenFileDialog
        {
            Filter = "技能数据库(Magic.DB)|*.DB",     // 原 :285
            FileName = initialFileName,               // 原 :286
            Title = "指定技能数据库"                   // 原 :287
        };
        return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;   // 原 :289-294
    }

    public static bool ShowFrmMaigcCD(IMagicDbReader readerFactory = null)
    {
        string s = MagicCDLogic.ComputeDefaultMagicDbPath(Application.ExecutablePath);   // 原 :278-279
        if (!FileExistsProbe(s))                                                          // 原 :281
        {
            string chosen = ChooseMagicDb("Magic.DB");                                    // 原 :286
            if (chosen == null) return false;                                             // 原 :292 Exit
            s = chosen;                                                                   // 原 :290
        }

        using var form = new FrmMagicCD();
        if (readerFactory != null) form.DoOpenWithReader(readerFactory);                  // 原 :302 DoOpen(S)
        else form.DoOpen(s);
        return form.ShowDialog() == DialogResult.OK;                                      // 原 :303
    }
}

/// <summary>原 uFrmMagicCD.pas:14-68 `TFrmMagicCD`（DFM: uFrmMagicCD.dfm）。</summary>
public class FrmMagicCD : Form
{
    // DFM: FrmMagicCD Left=425 Top=285 BorderStyle=bsDialog Caption='技能CD设置'
    //      ClientHeight=429 ClientWidth=369 Font.Charset=GB2312_CHARSET Font.Height=-12 Font.Name='宋体'
    //      Position=poMainFormCenter OnCreate=FormCreate PixelsPerInch=96
    public Label lbl4;             // DFM: lbl4 Left=8 Top=405 Width=36 Height=12 Caption='技能：'
    public ListView vstMagicCD;    // DFM: vstMagicCD (TVirtualStringTree) Left=8 Top=78 Width=353 Height=312
                                   //      DefaultNodeHeight=22 Header.DefaultHeight=24 Columns=[技能ID(60),技能名称(140),冷确时间 [毫秒](149)]
    public Button btnOK;           // DFM: btnOK Left=286 Top=398 Width=75 Height=25 Caption='确定' TabOrder=1 OnClick=btnOKClick
    public GroupBox GroupBox1;     // DFM: GroupBox1 Left=8 Top=5 Width=353 Height=68 Caption='技能冷确时间未到提示' TabOrder=2
    public Label lbl1;             // DFM: lbl1 Left=105 Top=20 Width=36 Height=12 Caption='内容：'
    public Label lbl2;             // DFM: lbl2 Left=8 Top=20 Width=36 Height=12 Caption='位置：'
    public Label Label1, Label2, lbl3, Label4;
    public TextBox edtMagicCDMsgText;      // DFM: edtMagicCDMsgText (在 GroupBox1 内)
    public ComboBox cbbMagicCDMsgType;     // DFM: cbbMagicCDMsgType Style=csDropDownList
    public TColorIndexEdit seMagicCDFColor; // DFM: seMagicCDFColor (TColorIndexEdit)
    public TColorIndexEdit seMagicCDBColor; // DFM: seMagicCDBColor (TColorIndexEdit)
    public TSpinEditEx seMagicCDShowX;      // DFM: seMagicCDShowX
    public TSpinEditEx seMagicCDShowY;      // DFM: seMagicCDShowY
    public TextBox edtMagicName;            // DFM: edtMagicName
    public Button btnSearch;                // DFM: btnSearch Caption='搜索' OnClick=btnSearchClick
    public Button btnSearchNext;            // DFM: btnSearchNext Caption='搜索下一个' OnClick=btnSearchNextClick
    public OpenFileDialog dlgOpen1;         // DFM: dlgOpen1 (TOpenDialog)

    /// <summary>原 :15 第 0 列宽度 60、第 1 列 140、第 2 列 149（.dfm Columns）。</summary>
    public const int ColWidthMagicId = 60;
    public const int ColWidthMagicName = 140;
    public const int ColWidthCdTime = 149;

    /// <summary>当前界面上的一行（原 `PMagicData` / `NodeData`）。</summary>
    public List<MagicCDRow> Rows = new List<MagicCDRow>();

    /// <summary>
    /// 当前焦点行镜像。WinForms `ListView.SelectedIndices` 需要控件已创建句柄
    /// （无消息循环的单元测试里恒为空），而原文 `vstMagicCD.FocusedNode` 是纯数据。
    /// 真实运行时它始终等于 `SelectedIndices[0]`。
    /// </summary>
    public int SelectedRowMirror = -1;

    /// <summary>当前焦点行（原文 `vstMagicCD.FocusedNode`；无焦点为 -1）。</summary>
    public int FocusedRow
        => vstMagicCD.SelectedIndices.Count > 0 ? vstMagicCD.SelectedIndices[0] : SelectedRowMirror;

    /// <summary>设置焦点行（原文 `vstMagicCD.FocusedNode := Node`）。</summary>
    public void SetFocusedRow(int index)
    {
        SelectedRowMirror = index;
        if (index >= 0 && index < vstMagicCD.Items.Count)
        {
            vstMagicCD.Items[index].Selected = true;
            vstMagicCD.Items[index].Focused = true;
        }
    }

    /// <summary>`cbbMagicCDMsgType.Items` 的项（原文由 .dfm 的 Items.Strings 提供；
    /// 本地 .dfm 未列出该项列表，此处按 `g_btMagicCDMsgType` 的取值域给出等价项，见文件头已知偏差）。</summary>
    public static readonly string[] MagicCDMsgTypeItems = { "密人提示", "屏幕提示", "系统提示" };

    /// <summary>测试可见：前景色索引（避免测试直接依赖 `TColorIndexEdit.ColorIndex` 的 WinForms 值域）。</summary>
    public byte FColorIndex => seMagicCDFColor.ColorIndex;

    /// <summary>测试可见：背景色索引。</summary>
    public byte BColorIndex => seMagicCDBColor.ColorIndex;

    public FrmMagicCD()
    {
        // DFM: FrmMagicCD Caption='技能CD设置' BorderStyle=bsDialog Position=poMainFormCenter
        Text = "技能CD设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Location = new Point(425, 285);
        ClientSize = new Size(369, 429);
        Font = new Font("宋体", 9F);                      // Font.Height=-12 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: lbl4 Left=8 Top=405 Width=36 Height=12 Caption='技能：'
        lbl4 = new Label { Left = 8, Top = 405, Width = 36, Height = 12, Text = "技能：" };

        // DFM: vstMagicCD (TVirtualStringTree) Left=8 Top=78 Width=353 Height=312 DefaultNodeHeight=22
        //      Header.DefaultHeight=24 Header.Options 含 hoVisible；TreeOptions 含 toEditable/toFullRowSelect
        //      列宽：技能ID=60 / 技能名称=140 / 冷确时间 [毫秒]=149
        // ★ 第三方 VirtualTrees 未移植 → 用 ListView(View.Details) 承载（见文件头 D 说明）。
        vstMagicCD = new ListView
        {
            Left = 8, Top = 78, Width = 353, Height = 312, TabIndex = 0,
            View = View.Details,
            GridLines = true,                       // TreeOptions.PaintOptions 含 toShowHorzGridLines/toShowVertGridLines
            FullRowSelect = true,                   // toFullRowSelect
            MultiSelect = false,
            HideSelection = false,                  // toExtendedFocus
            LabelEdit = true,                       // toEditable（仅第 2 列真正可编辑）
            OwnerDraw = true                        // 支撑 OnBeforeItemErase（隔行底色）
        };
        vstMagicCD.Columns.Add("技能ID", ColWidthMagicId);              // DFM: Columns[0] Width=60 WideText='技能ID'
        vstMagicCD.Columns.Add("技能名称", ColWidthMagicName);           // DFM: Columns[1] Width=140 WideText='技能名称'
        vstMagicCD.Columns.Add("冷确时间 [毫秒]", ColWidthCdTime);        // DFM: Columns[2] Width=149 WideText='冷确时间 [毫秒]'

        // DFM: btnOK Left=286 Top=398 Width=75 Height=25 Caption='确定' TabOrder=1 OnClick=btnOKClick
        btnOK = new Button { Left = 286, Top = 398, Width = 75, Height = 25, Text = "确定", TabIndex = 1 };

        // DFM: GroupBox1 Left=8 Top=5 Width=353 Height=68 Caption='技能冷确时间未到提示' TabOrder=2
        GroupBox1 = new GroupBox { Left = 8, Top = 5, Width = 353, Height = 68,
                                   Text = "技能冷确时间未到提示", TabIndex = 2 };
        // DFM: lbl1 Left=105 Top=20 Width=36 Height=12 Caption='内容：'
        lbl1 = new Label { Left = 105, Top = 20, Width = 36, Height = 12, Text = "内容：" };
        // DFM: lbl2 Left=8 Top=20 Width=36 Height=12 Caption='位置：'
        lbl2 = new Label { Left = 8, Top = 20, Width = 36, Height = 12, Text = "位置：" };
        Label1 = new Label { Left = 44, Top = 20, Width = 20, Height = 12, Text = "X：" };
        seMagicCDShowX = new TSpinEditEx { Left = 60, Top = 16, Width = 40, Height = 21, TabIndex = 0 };
        Label2 = new Label { Left = 110, Top = 20, Width = 20, Height = 12, Text = "Y：" };
        seMagicCDShowY = new TSpinEditEx { Left = 128, Top = 16, Width = 40, Height = 21, TabIndex = 1 };
        edtMagicCDMsgText = new TextBox { Left = 141, Top = 16, Width = 202, Height = 20, TabIndex = 2 };
        Label4 = new Label { Left = 8, Top = 46, Width = 36, Height = 12, Text = "类型：" };
        cbbMagicCDMsgType = new ComboBox { Left = 44, Top = 42, Width = 64, Height = 21, TabIndex = 3,
                                           DropDownStyle = ComboBoxStyle.DropDownList };
        cbbMagicCDMsgType.Items.AddRange(new object[] { MagicCDMsgTypeItems[0], MagicCDMsgTypeItems[1], MagicCDMsgTypeItems[2] });
        lbl3 = new Label { Left = 116, Top = 46, Width = 36, Height = 12, Text = "前景：" };
        seMagicCDFColor = new TColorIndexEdit { Left = 152, Top = 42, Width = 40, Height = 21, TabIndex = 4 };
        Label1.Text = "前景色：";       // 保持原文 Label1 语义（前景）
        Label2.Text = "背景色：";       // 保持原文 Label2 语义（背景）
        seMagicCDBColor = new TColorIndexEdit { Left = 232, Top = 42, Width = 40, Height = 21, TabIndex = 5 };

        edtMagicName = new TextBox { Left = 44, Top = 405, Width = 140, Height = 20 };
        btnSearch = new Button { Left = 190, Top = 402, Width = 53, Height = 22, Text = "搜索" };
        btnSearchNext = new Button { Left = 248, Top = 402, Width = 71, Height = 22, Text = "搜索下一个" };
        dlgOpen1 = new OpenFileDialog { Filter = "技能数据库(Magic.DB)|*.DB", FileName = "Magic.DB", Title = "指定技能数据库" };

        GroupBox1.Controls.AddRange(new Control[] { lbl1, lbl2, Label1, Label2, Label4, lbl3,
                                                    edtMagicCDMsgText, cbbMagicCDMsgType,
                                                    seMagicCDFColor, seMagicCDBColor,
                                                    seMagicCDShowX, seMagicCDShowY });
        Controls.AddRange(new Control[] { lbl4, vstMagicCD, btnOK, GroupBox1,
                                          edtMagicName, btnSearch, btnSearchNext });

        // DFM 的事件接线
        Load += (s, e) => FormCreate(s, e);                                    // OnCreate=FormCreate
        btnOK.Click += (s, e) => btnOK_Click(s, e);                            // OnClick=btnOKClick
        btnSearch.Click += (s, e) => btnSearch_Click(s, e);
        btnSearchNext.Click += (s, e) => btnSearchNext_Click(s, e);
        edtMagicName.KeyDown += (s, e) => edtMagicName_KeyDown(s, e);
        vstMagicCD.KeyDown += (s, e) => vstMagicCD_KeyDown(s, e);
        vstMagicCD.DrawItem += (s, e) => vstMagicCD_EraseAltRow(s, e);
    }

    /// <summary>原 uFrmMagicCD.pas:309-312 `FormCreate`（只设 NodeDataSize）。</summary>
    public void FormCreate(object sender, EventArgs e)
    {
        // 原 :311 vstMagicCD.NodeDataSize := SizeOf(TMagicData); → 托管侧行数据由 Rows 列表承载
        Rows = new List<MagicCDRow>();
    }

    /// <summary>测试辅助：由外部读好的行填充界面。</summary>
    public void SetRows(IReadOnlyList<MagicCDRow> rows)
    {
        Rows = new List<MagicCDRow>(rows);
        vstMagicCD.Items.Clear();
        for (int i = 0; i < Rows.Count; i++)
        {
            var item = new ListViewItem(MagicCDLogic.GetCellText(Rows[i], 0));       // 原 :322
            item.SubItems.Add(MagicCDLogic.GetCellText(Rows[i], 1));                 // 原 :323
            item.SubItems.Add(MagicCDLogic.GetCellText(Rows[i], 2));                 // 原 :324
            item.Tag = Rows[i];
            vstMagicCD.Items.Add(item);
        }
        SelectedRowMirror = -1;                       // 重建后无焦点（原 :373 AddChild 不设 FocusedNode）
    }

    /// <summary>测试辅助：把界面读回纯逻辑行列表（含用户编辑后的值）。</summary>
    public List<MagicCDRow> ReadRows()
    {
        var rows = new List<MagicCDRow>();
        foreach (ListViewItem it in vstMagicCD.Items)
        {
            var row = it.Tag as MagicCDRow;
            if (row == null) continue;
            row.MagicID = DelphiRTL.StrToIntDef(it.Text, 0);
            row.MagicName = it.SubItems.Count > 1 ? it.SubItems[1].Text : "";
            row.CDTime = unchecked((uint)DelphiRTL.StrToIntDef(it.SubItems.Count > 2 ? it.SubItems[2].Text : "0", 0));
            rows.Add(row);
        }
        return rows;
    }

    /// <summary>原 :347 `cbbMagicCDMsgType.ItemIndex := g_btMagicCDMsgType` 的安全等价：
    /// WinForms 的 `ComboBox.SelectedIndex` 越界会抛 `ArgumentOutOfRangeException`，
    /// 而 Delphi 的 `TComboBox.ItemIndex` 越界只写内部字段（不抛）。此处显式夹取以复刻 Delphi 语义。</summary>
    private void SetMsgTypeIndex(int value)
    {
        if (cbbMagicCDMsgType.Items.Count == 0)
        {
            cbbMagicCDMsgType.SelectedIndex = -1;                 // 无项可选中（Delphi 亦不显示任何项）
            return;
        }
        cbbMagicCDMsgType.SelectedIndex = Math.Max(0, Math.Min(value, cbbMagicCDMsgType.Items.Count - 1));
    }

    /// <summary>原 :337-398 `DoOpen`（从 Paradox DB 读取；此处经 <see cref="IMagicDbReader"/> 接缝）。</summary>
    public void DoOpenWithReader(IMagicDbReader reader)
    {
        SetMsgTypeIndex(FormGlobals.g_btMagicCDMsgType);                       // 原 :347
        edtMagicCDMsgText.Text = FormGlobals.g_sMagicCDMsgText;                // 原 :348
        seMagicCDFColor.ColorIndex = FormGlobals.g_btMagicCDFColor;            // 原 :349
        seMagicCDBColor.ColorIndex = FormGlobals.g_btMagicCDBColor;            // 原 :350
        seMagicCDShowX.Value = FormGlobals.g_nMagicCDShowX;                    // 原 :351
        seMagicCDShowY.Value = FormGlobals.g_nMagicCDShowY;                    // 原 :352

        SetRows(MagicCDLogic.ReadMagicRows(reader, FormGlobals.g_MagicCDList)); // 原 :354-397
    }

    /// <summary>原 :337-398 `DoOpen(MagicDBName)` 的文件版（ParadoxDataSet 未移植 → 交回接缝）。</summary>
    public void DoOpen(string magicDbName)
    {
        // 接缝：TParadoxDataSet 尚未移植（ParadoxConv.cs 只覆盖编码转换）。
        // 原文 DateSet.TableName := MagicDBName; DateSet.Open; 后的读取见 MagicCDLogic.ReadMagicRows。
        throw new NotSupportedException(
            "TParadoxDataSet 尚未移植；请用 DoOpenWithReader(IMagicDbReader) 或由 ParadoxConv 车道补齐后接入。");
    }

    /// <summary>原 uFrmMagicCD.pas:456-504 `btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        // 原 :482 vstMagicCD.EndEditNode（提交尚未结束的单元格编辑）
        // → 托管侧 ListView 没有直接的 EndEdit API，用 Win32 消息 LVM_ENDLABELEDIT 提交标签编辑
        //   （原文同样丢弃 EndEditNode 的返回值）。
        const int LVM_FIRST = 0x1000;
        const int LVM_ENDLABELEDIT = LVM_FIRST + 64;      // 0x1040
        if (vstMagicCD.IsHandleCreated)
            vstMagicCD.SendMessage(LVM_ENDLABELEDIT, IntPtr.Zero, IntPtr.Zero);

        var rows = ReadRows();
        MagicCDLogic.ButtonOK(rows,
            FormGlobals.g_sIniFileName, FormGlobals.g_MagicCDListFileName,
            cbbMagicCDMsgType.SelectedIndex, edtMagicCDMsgText.Text,
            seMagicCDFColor.ColorIndex, seMagicCDBColor.ColorIndex,
            seMagicCDShowX.Value, seMagicCDShowY.Value);
        DialogResult = DialogResult.OK;                                        // 原 :503
    }

    /// <summary>原 uFrmMagicCD.pas:506-535 `btnSearchClick`。</summary>
    public void btnSearch_Click(object sender, EventArgs e)
    {
        string magicName = DelphiRTL.Trim(edtMagicName.Text);                  // 原 :512
        if (magicName.Length == 0)                                             // 原 :513
        {
            MessageBoxSeam.ShowInformation("搜索内容不能为空", "信息");          // 原 :515
            edtMagicName.Focus();                                              // 原 :516
            return;                                                            // 原 :517 Exit
        }

        int hit = MagicCDLogic.FindFirst(Rows, magicName);                     // 原 :520-534
        if (hit >= 0) SelectRow(hit);
    }

    /// <summary>原 uFrmMagicCD.pas:537-571 `btnSearchNextClick`。</summary>
    public void btnSearchNext_Click(object sender, EventArgs e)
    {
        string magicName = DelphiRTL.Trim(edtMagicName.Text);                  // 原 :543
        if (magicName.Length == 0)                                             // 原 :544
        {
            MessageBoxSeam.ShowInformation("搜索内容不能为空", "信息");          // 原 :546
            edtMagicName.Focus();                                              // 原 :547
            return;                                                            // 原 :548 Exit
        }

        int focused = FocusedRow;                                                        // 原 :551 FocusedNode
        int hit = MagicCDLogic.FindNext(Rows, focused, magicName);                                 // 原 :557-570
        if (hit >= 0) SelectRow(hit);
    }

    private void SelectRow(int index)
    {
        SetFocusedRow(index);                      // 原 :527-528 FocusedNode + Selected[Node] := True
        vstMagicCD.EnsureVisible(index);
        vstMagicCD.Focus();                          // 原 :529 SetFocus
    }

    /// <summary>原 uFrmMagicCD.pas:573-580 `edtMagicNameKeyDown`：VK_RETURN 且 Trim 后非空 → 触发搜索。</summary>
    public void edtMagicName_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return && DelphiRTL.Trim(edtMagicName.Text).Length > 0)   // 原 :576
            btnSearch_Click(btnSearch, EventArgs.Empty);                                        // 原 :578 btnSearch.Click
    }

    /// <summary>原 uFrmMagicCD.pas:447-454 `vstMagicCDKeyDown`：VK_RETURN 且非编辑中且有焦点节点 → 编辑第 2 列。</summary>
    public void vstMagicCD_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return && FocusedRow >= 0)                                 // 原 :450
            vstMagicCD.Items[FocusedRow].BeginEdit();                                     // 原 :452 EditNode(..., 2)
    }

    /// <summary>原 uFrmMagicCD.pas:408-417 `vstMagicCDBeforeItemErase`：奇数行底色 $00F9F9F9。</summary>
    public void vstMagicCD_EraseAltRow(object sender, DrawListViewItemEventArgs e)
    {
        if (MagicCDLogic.ShouldEraseAltRow(e.ItemIndex))                    // 原 :412
        {
            e.Graphics.FillRectangle(new SolidBrush(MagicCDLogic.AltRowColor), e.Bounds);   // 原 :414-415
            e.DrawDefault = false;
        }
        else
        {
            e.DrawDefault = true;
        }
    }
}
