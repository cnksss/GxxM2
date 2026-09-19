using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.DBServer;

// RouteEdit.pas (1-783) → RouteEdit.cs
// 网关路由编辑窗体（主游戏网关 1..8 + 备用 RunGate2 列表）。
//
// 接缝说明：
//   · VirtualTrees 的 `TVirtualStringTree` / `IVTEditLink`（RouteEdit.pas:99-347 TPropertyEditLink）
//     在 WinForms 无对应类型；节点模型（TRunGateNode）与"哪一列编辑什么、如何裁剪"的语义已 1:1 移植，
//     内联编辑器本身用 ListView + 单元格编辑接缝（PrepareEdit/EndEdit）表达。待 VirtualTrees 移植后接入。

/// <summary>RouteEdit.pas:10 `WM_STARTEDITING = WM_USER + 778`。</summary>
public static class RouteEditConst
{
    public const int WM_USER = 0x0400;
    public const int WM_STARTEDITING = WM_USER + 778;
}

/// <summary>VirtualTrees 的 TCheckState 子集（原文使用 csCheckedNormal / csUncheckedNormal）。</summary>
public static class TCheckState
{
    public const byte csUncheckedNormal = 0;
    public const byte csCheckedNormal = 1;
}

/// <summary>VirtualTrees 的一个节点：NodeData(TRunGateInfo 拷贝) + CheckState + Index。</summary>
public class TRunGateNode
{
    public TRunGateInfo Data = new TRunGateInfo();
    public byte CheckState = TCheckState.csCheckedNormal;
    public int Index;

    public TRunGateNode Clone()
        => new TRunGateNode
        {
            Index = Index,
            CheckState = CheckState,
            Data = new TRunGateInfo
            {
                Enabled = Data.Enabled,
                IP = Data.IP,
                Port = Data.Port,
                DBPort = Data.DBPort,
                Level = Data.Level,
                LastResponseTick = Data.LastResponseTick,
                IsConnect = Data.IsConnect
            }
        };
}

/// <summary>窗体内 8 组网关编辑框 + 备用网关开关的取值快照（等价 `edtGateIP1..8` 等控件）。</summary>
public class RouteEditInputs
{
    public string SelGate = "";
    public string[] GateIP = new string[8];
    public string[] GatePort = new string[8];
    public string[] DBPort = new string[8];
    public bool OpenRunGate2;
    public int GameGateDisconnectCount;

    public RouteEditInputs()
    {
        for (int i = 0; i < 8; i++) { GateIP[i] = ""; GatePort[i] = ""; DBPort[i] = ""; }
    }
}

/// <summary>RouteEdit.pas:447-637 `btnOKClick` 的执行结果（用于单测断言"弹了什么/焦点去哪/ModalResult"）。</summary>
public class BtnOKOutcome
{
    /// <summary>TModalResult.mrNone 表示原文走的是 `Exit`（未设置 ModalResult）。</summary>
    public int ModalResult = TModalResult.mrNone;

    public bool MessageBoxShown;
    public string MessageBoxText = "";
    public string MessageBoxCaption = "";
    public uint MessageBoxFlags;

    /// <summary>原文 Exit 前 SetFocus 的目标控件名（''=无）。</summary>
    public string FocusTarget = "";

    /// <summary>原文 `vstRunGate.Selected[Node] := True; FocusedNode := Node;` 的节点序号（-1=无）。</summary>
    public int FocusNodeIndex = -1;

    /// <summary>原文 `vstRunGate.EditNode(Node, Column)` 的列（-1=无）。</summary>
    public int EditNodeColumn = -1;
}

/// <summary>RouteEdit.pas 的非 UI 逻辑（可单测）。</summary>
public static class RouteEditLogic
{
    /// <summary>RouteEdit.pas:678 `if vstRunGate.RootNodeCount >= 100`。</summary>
    public const int MaxRunGateNodes = 100;

    /// <summary>
    /// RouteEdit.pas:375-445 `TfrmRouteEdit.DoOpen`：
    ///   ① 8 组编辑框 ← FRouteInfo；
    ///   ② 备用列表节点 ← FRouteInfo.RunGate2List（**逐字段拷贝**，NodeData^ := Item^）；
    ///   ③ CheckState ← NodeData.Enabled；
    ///   ④ NodeData.IsConnect := GetTickCount - LastResponseTick &lt;= 3000（原文如此，改写的是节点副本）；
    ///   ⑤ FIsChanged := False。
    /// </summary>
    public static void DoOpen(TRouteInfo FRouteInfo, RouteEditInputs inputs, List<TRunGateNode> nodes, ref byte FIsChanged)
    {
        inputs.SelGate = FRouteInfo.sSelGateIP;
        for (int i = 0; i < 8; i++)
        {
            inputs.GateIP[i] = FRouteInfo.sGameGateIP[i];
            inputs.GatePort[i] = DelphiRTL.IntToStr(FRouteInfo.nGameGatePort[i]);
            inputs.DBPort[i] = DelphiRTL.IntToStr(FRouteInfo.nGameGateDBPort[i]);
        }

        inputs.OpenRunGate2 = FRouteInfo.EnabledRunGate2List != 0;
        inputs.GameGateDisconnectCount = FRouteInfo.GameGateDisconnectCount;

        nodes.Clear();
        for (int I = 0; I <= FRouteInfo.RunGate2List.Count - 1; I++)
        {
            TRunGateInfo src = FRouteInfo.RunGate2List.Items(I);
            var node = new TRunGateNode
            {
                Index = I,
                Data = new TRunGateInfo
                {
                    Enabled = src.Enabled,
                    IP = src.IP,
                    Port = src.Port,
                    DBPort = src.DBPort,
                    Level = src.Level,
                    LastResponseTick = src.LastResponseTick,
                    IsConnect = src.IsConnect
                }
            };

            node.CheckState = node.Data.Enabled != 0 ? TCheckState.csCheckedNormal : TCheckState.csUncheckedNormal;

            node.Data.IsConnect = TBool.ToByte(DelphiTick.GetTickCount() - node.Data.LastResponseTick <= 3000);
            nodes.Add(node);
        }

        FIsChanged = 0;
    }

    /// <summary>
    /// RouteEdit.pas:447-637 `btnOKClick` 的逐字移植。
    /// 原文缺陷（已保留并测试锁定）：
    ///   第 6/7/8 组的 IP 校验写成了 `IsIPaddr(FRouteInfo.sGameGateIP[5]/[6]/[7])`（尚未赋值的槽位），
    ///   而不是本组的局部 `sGameGateIP` → 新建路由到第 6 组必然提前 `ModalResult := mrOK; Exit`，
    ///   nGateCount 最多只能到 5（文件里已有第 6 条 IP 时才会"看起来正常"）。
    /// </summary>
    public static BtnOKOutcome BtnOK(RouteEditInputs inp, TRouteInfo FRouteInfo, List<TRunGateNode> nodes, ref byte FIsChanged)
    {
        var outcome = new BtnOKOutcome();
        string sSelGateIP, sGameGateIP;
        int nGameGatePort;

        sSelGateIP = DelphiRTL.Trim(inp.SelGate);
        if (!HUtil32.IsIPaddr(sSelGateIP))
        {
            outcome.MessageBoxShown = true;
            outcome.MessageBoxText = "角色网关输入错误！！！";
            outcome.MessageBoxCaption = "错误信息";
            outcome.MessageBoxFlags = TMsgBox.MB_OK + TMsgBox.MB_ICONERROR;
            UiSeam.MessageBox(outcome.MessageBoxText, outcome.MessageBoxCaption, outcome.MessageBoxFlags);
            outcome.FocusTarget = "EditSelGate";
            return outcome;
        }

        sGameGateIP = DelphiRTL.Trim(inp.GateIP[0]);
        nGameGatePort = DelphiRTL.StrToIntDef(inp.GatePort[0], 0);
        if (!HUtil32.IsIPaddr(sGameGateIP))
        {
            outcome.MessageBoxShown = true;
            outcome.MessageBoxText = "游戏网关一输入错误！！！";
            outcome.MessageBoxCaption = "错误信息";
            outcome.MessageBoxFlags = TMsgBox.MB_OK + TMsgBox.MB_ICONERROR;
            UiSeam.MessageBox(outcome.MessageBoxText, outcome.MessageBoxCaption, outcome.MessageBoxFlags);
            outcome.FocusTarget = "edtGateIP1";
            return outcome;
        }
        if (nGameGatePort <= 0)
        {
            outcome.MessageBoxShown = true;
            outcome.MessageBoxText = "游戏网关一输入错误！！！";
            outcome.MessageBoxCaption = "错误信息";
            outcome.MessageBoxFlags = TMsgBox.MB_OK + TMsgBox.MB_ICONERROR;
            UiSeam.MessageBox(outcome.MessageBoxText, outcome.MessageBoxCaption, outcome.MessageBoxFlags);
            outcome.FocusTarget = "edtGatePort1";
            return outcome;
        }

        // vstRunGate.EndEditNode;
        if (inp.OpenRunGate2)
        {
            for (int n = 0; n < nodes.Count; n++)
            {
                TRunGateNode node = nodes[n];

                if (!HUtil32.IsIPaddr(DelphiRTL.Trim(node.Data.IP)))
                {
                    outcome.MessageBoxShown = true;
                    outcome.MessageBoxText = "IP输入错误！";
                    outcome.MessageBoxCaption = "错误信息";
                    outcome.MessageBoxFlags = TMsgBox.MB_OK + TMsgBox.MB_ICONERROR;
                    UiSeam.MessageBox(outcome.MessageBoxText, outcome.MessageBoxCaption, outcome.MessageBoxFlags);
                    outcome.FocusNodeIndex = n;
                    outcome.EditNodeColumn = 1;
                    return outcome;
                }

                if (node.Data.Port <= 0)
                {
                    outcome.MessageBoxShown = true;
                    outcome.MessageBoxText = "端口输入错误！";
                    outcome.MessageBoxCaption = "错误信息";
                    outcome.MessageBoxFlags = TMsgBox.MB_OK + TMsgBox.MB_ICONERROR;
                    UiSeam.MessageBox(outcome.MessageBoxText, outcome.MessageBoxCaption, outcome.MessageBoxFlags);
                    outcome.FocusNodeIndex = n;
                    outcome.EditNodeColumn = 2;
                    return outcome;
                }

                for (int m = n + 1; m < nodes.Count; m++)
                {
                    TRunGateNode NextNodeData = nodes[m];

                    if (DelphiStrUtils.SameText(DelphiRTL.Trim(NextNodeData.Data.IP), DelphiRTL.Trim(node.Data.IP)) &&
                        ((NextNodeData.Data.Port == node.Data.Port) || (NextNodeData.Data.DBPort == node.Data.DBPort)))
                    {
                        outcome.MessageBoxShown = true;
                        outcome.MessageBoxText = "IP端口输入重复" + "[" + DelphiRTL.IntToStr(node.Index) + "]";
                        outcome.MessageBoxCaption = "错误信息";
                        outcome.MessageBoxFlags = TMsgBox.MB_OK + TMsgBox.MB_ICONERROR;
                        UiSeam.MessageBox(outcome.MessageBoxText, outcome.MessageBoxCaption, outcome.MessageBoxFlags);
                        outcome.FocusNodeIndex = m;
                        return outcome;
                    }
                }
            }
        }

        FRouteInfo.sSelGateIP = sSelGateIP;
        FRouteInfo.sGameGateIP[0] = sGameGateIP;
        FRouteInfo.nGameGatePort[0] = nGameGatePort;
        FRouteInfo.nGameGateDBPort[0] = DelphiRTL.StrToIntDef(DelphiRTL.Trim(inp.DBPort[0]), 0);
        FRouteInfo.nGateCount = 1;

        FRouteInfo.EnabledRunGate2List = TBool.ToByte(inp.OpenRunGate2);
        FRouteInfo.GameGateDisconnectCount = inp.GameGateDisconnectCount;

        if (FIsChanged != 0)
        {
            FRouteInfo.RunGate2List.Clear();
            // vstRunGate.EndEditNode;
            if (inp.OpenRunGate2)
            {
                for (int n = 0; n < nodes.Count; n++)
                {
                    TRunGateNode node = nodes[n];
                    bool IsEnable = node.CheckState == TCheckState.csCheckedNormal;
                    FRouteInfo.RunGate2List.Add(TBool.ToByte(IsEnable), node.Data.IP, node.Data.Port, node.Data.DBPort, node.Data.Level);
                }
            }
            FRouteInfo.RunGate2List.DoSort();
        }

        // ---- 第 2..8 组（注意第 6/7/8 组校验的是 FRouteInfo 槽位而非本组文本，原文如此） ----
        for (int g = 1; g <= 7; g++)
        {
            sGameGateIP = DelphiRTL.Trim(inp.GateIP[g]);
            nGameGatePort = DelphiRTL.StrToIntDef(inp.GatePort[g], 0);

            string checkIp;
            if (g <= 4)                       // 第2..5组：校验本组文本
                checkIp = sGameGateIP;
            else                              // 第6..8组：原文误用 FRouteInfo.sGameGateIP[g-1]（尚未赋值的槽位）
                checkIp = FRouteInfo.sGameGateIP[g - 1];

            if ((!HUtil32.IsIPaddr(checkIp)) || (nGameGatePort <= 0))
            {
                outcome.ModalResult = TModalResult.mrOK;
                return outcome;
            }
            FRouteInfo.sGameGateIP[g] = sGameGateIP;
            FRouteInfo.nGameGatePort[g] = nGameGatePort;
            FRouteInfo.nGameGateDBPort[g] = DelphiRTL.StrToIntDef(DelphiRTL.Trim(inp.DBPort[g]), 0);
            FRouteInfo.nGateCount = g + 1;
        }

        outcome.ModalResult = TModalResult.mrOK;
        return outcome;
    }

    /// <summary>
    /// RouteEdit.pas:673-704 `btnAddClick`：根节点数 &gt;= 100 时提示并返回 false；
    /// 否则新增节点（CheckType=ctCheckBox / CheckState=csCheckedNormal / IP='' / Port=7200 / Level=10 /
    /// IsConnect=True / LastResponseTick=0），选中并进入第 1 列编辑，FIsChanged := True。
    /// </summary>
    public static bool BtnAdd(List<TRunGateNode> nodes, ref byte FIsChanged, out TRunGateNode newNode)
    {
        newNode = null;
        if (nodes.Count >= MaxRunGateNodes)
        {
            UiSeam.MessageBox("备用网关已达到最大数量,不能再增加！", "提示信息", TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION);
            return false;
        }

        newNode = new TRunGateNode { Index = nodes.Count, CheckState = TCheckState.csCheckedNormal };
        newNode.Data.Enabled = 1;
        newNode.Data.IP = "";
        newNode.Data.Port = 7200;
        newNode.Data.Level = 10;
        newNode.Data.IsConnect = 1;
        newNode.Data.LastResponseTick = 0;
        nodes.Add(newNode);

        FIsChanged = 1;
        return true;
    }

    /// <summary>
    /// RouteEdit.pas:722-743 `btnDelClick`：
    ///   取焦点节点 → 先取下一个、没有则取上一个作为新焦点 → 删除 → 选新焦点 → FIsChanged := True。
    /// 返回 true 表示确实删除了（原文 `if Node &lt;&gt; nil`），newFocus 为 -1 表示无后继焦点。
    /// </summary>
    public static bool BtnDel(List<TRunGateNode> nodes, int focusedIndex, ref byte FIsChanged, out int newFocus)
    {
        newFocus = -1;
        if (focusedIndex < 0 || focusedIndex >= nodes.Count) return false;

        int SelNode = focusedIndex + 1;                       // GetNext(Node)
        if (SelNode >= nodes.Count)                           // GetNext = nil
            SelNode = focusedIndex - 1;                       // GetPrevious(Node)
        if (SelNode < 0) SelNode = -1;

        nodes.RemoveAt(focusedIndex);
        for (int i = 0; i < nodes.Count; i++) nodes[i].Index = i;   // VirtualTrees 的 Node.Index 重排

        if (SelNode >= 0)
        {
            newFocus = SelNode;
        }

        FIsChanged = 1;
        return true;
    }

    /// <summary>RouteEdit.pas:706-720 `vstRunGateGetText`：0=Index / 1=IP / 2=Port / 3=DBPort / 4=Level；其余列返回 ''。</summary>
    public static string GetText(TRunGateNode node, int Column)
    {
        switch (Column)
        {
            case 0: return DelphiRTL.IntToStr(node.Index);
            case 1: return node.Data.IP;
            case 2: return DelphiRTL.IntToStr(node.Data.Port);
            case 3: return DelphiRTL.IntToStr(node.Data.DBPort);
            case 4: return DelphiRTL.IntToStr(node.Data.Level);
        }
        return "";
    }

    /// <summary>RouteEdit.pas:765-776 `vstRunGatePaintText`：IsConnect 为假时字体红色（true=红）。</summary>
    public static bool PaintTextIsRed(TRunGateNode node) => node.Data.IsConnect == 0;

    /// <summary>RouteEdit.pas:745-755 `vstRunGateKeyDown`：Ctrl+Insert → btnAdd；Ctrl+Delete → btnDel。</summary>
    public static string KeyDownCommand(int key, bool ctrl)
    {
        if (!ctrl) return "";
        if (key == (int)Keys.Insert) return "btnAdd";
        if (key == (int)Keys.Delete) return "btnDel";
        return "";
    }

    /// <summary>RouteEdit.pas:272-325 `TPropertyEditLink.PrepareEdit` 的列 → 编辑器参数映射。</summary>
    public static bool PrepareEdit(int Column, TRunGateNode node, out int minValue, out int maxValue, out string text)
    {
        minValue = 0;
        maxValue = 0;
        text = "";
        switch (Column)
        {
            case 2:
            case 3:
            case 4:
                minValue = 0;
                maxValue = 65535;                 // High(Word)
                if (Column == 2) text = DelphiRTL.IntToStr(node.Data.Port);
                else if (Column == 3) text = DelphiRTL.IntToStr(node.Data.DBPort);
                else text = DelphiRTL.IntToStr(node.Data.Level);
                return true;
            case 1:
                text = node.Data.IP;              // MaxLength := 15
                maxValue = 15;
                return true;
            default:
                return false;                     // else Result := False
        }
    }

    /// <summary>
    /// RouteEdit.pas:200-261 `TPropertyEditLink.EndEdit`：把编辑结果写回节点。
    /// 原文如此：
    ///   · 第 1 列比较用的是 `Trim(Text)`，但写回的是**未 Trim 的原文本**；
    ///   · 第 2/3/4 列比较/写回 TSpinEditEx.Value（Port/DBPort 为 Word，赋 int 时由编译器范围裁剪）。
    /// 返回 IsChanged（原文据此把宿主窗体 FIsChanged 置 True）。
    /// </summary>
    public static bool EndEdit(int Column, bool isTEdit, string editText, int editValue, TRunGateNode node)
    {
        bool IsChanged = false;

        if (isTEdit)
        {
            if (Column == 1)
            {
                string TempString = DelphiRTL.Trim(editText);

                if (!DelphiStrUtils.SameText(TempString, node.Data.IP))
                {
                    node.Data.IP = editText;      // 原文如此：赋未 Trim 的原文本
                    IsChanged = true;
                }
            }
        }
        else
        {
            int TempValue = editValue;
            switch (Column)
            {
                case 2:
                    if (node.Data.Port != TempValue)
                    {
                        node.Data.Port = (ushort)TempValue;
                        IsChanged = true;
                    }
                    break;
                case 3:
                    if (node.Data.DBPort != TempValue)
                    {
                        node.Data.DBPort = (ushort)TempValue;
                        IsChanged = true;
                    }
                    break;
                case 4:
                    if (node.Data.Level != TempValue)
                    {
                        node.Data.Level = TempValue;
                        IsChanged = true;
                    }
                    break;
            }
        }

        return IsChanged;
    }

    /// <summary>RouteEdit.pas:645-649 `vstRunGateEditing`：Allowed := Node &lt;&gt; nil。</summary>
    public static bool Editing(int nodeIndex) => nodeIndex >= 0;
}

/// <summary>RouteEdit.pas:90 / 350-371 `ShowFrmRouteEdit`（+ SaveServerInfo）。</summary>
public static class RouteEditUnit
{
    /// <summary>接缝：测试可替换为"直接返回 bool 并落盘"的桩，避免弹真实模态窗体。</summary>
    public static Func<TRouteInfo, bool, bool> ShowFrmRouteEdit = DefaultShowFrmRouteEdit;

    /// <summary>RouteEdit.pas:350-371 `ShowFrmRouteEdit` 默认实现。</summary>
    public static bool DefaultShowFrmRouteEdit(TRouteInfo RouteInfo, bool IsEdit)
    {
        var frmRouteEdit = new FrmRouteEdit();
        try
        {
            if (IsEdit)
                frmRouteEdit.Text = "修改网关路由";
            else
                frmRouteEdit.Text = "增加网关路由";
            frmRouteEdit.FRouteInfo = RouteInfo;
            frmRouteEdit.DoOpen();
            bool Result = (frmRouteEdit.ShowDialog() == DialogResult.OK) && (frmRouteEdit.FIsChanged != 0);

            if (Result)
            {
                DBShareSeam.SaveServerInfo();
            }
            return Result;
        }
        finally
        {
            frmRouteEdit.Dispose();
        }
    }
}

/// <summary>RouteEdit.pas:13-88 `TfrmRouteEdit`（DFM: RouteEdit.dfm）。</summary>
public class FrmRouteEdit : Form
{
    // DFM: frmRouteEdit Left=409 Top=188 BorderIcons=[biSystemMenu] BorderStyle=bsSingle Caption='修改网关路由'
    //      ClientHeight=461 ClientWidth=451 Position=poMainFormCenter OnCreate=FormCreate
    public Label Label1;
    public TextBox EditSelGate;
    public GroupBox GroupBox1;
    public Label Label2, Label3, Label4, Label5, Label6, Label7, Label8, Label9;
    public TextBox edtGateIP1, edtGateIP2, edtGateIP3, edtGateIP4, edtGateIP5, edtGateIP6, edtGateIP7, edtGateIP8;
    public TextBox edtGatePort1, edtGatePort2, edtGatePort3, edtGatePort4, edtGatePort5, edtGatePort6, edtGatePort7, edtGatePort8;
    public TextBox edtDBPort1, edtDBPort2, edtDBPort3, edtDBPort4, edtDBPort5, edtDBPort6, edtDBPort7, edtDBPort8;
    public GroupBox grp1;
    public Button btnCancel;
    public Button btnOK;
    public CheckBox chkOpenRunGate2;
    public TSpinEditEx seGameGateDisconnectCount;
    public Label lbl1;
    public Label lbl2;
    public ListView vstRunGate;      // DFM: TVirtualStringTree → WinForms ListView(CheckBoxes, 5 列)
    public Button btnAdd;
    public Button btnDel;

    /// <summary>RouteEdit.pas:80 `FRouteInfo: PTRouteInfo`。</summary>
    public TRouteInfo FRouteInfo;

    /// <summary>RouteEdit.pas:82 `FIsChanged: Boolean`。</summary>
    public byte FIsChanged;

    /// <summary>RouteEdit.pas:126-266 `TPropertyEditLink` 的节点模型（数据源）。</summary>
    public readonly List<TRunGateNode> Nodes = new List<TRunGateNode>();

    private TextBox[] _gateIPs;
    private TextBox[] _gatePorts;
    private TextBox[] _dbPorts;

    public FrmRouteEdit()
    {
        // DFM: frmRouteEdit Left=409 Top=188 BorderStyle=bsSingle ClientHeight=461 ClientWidth=451 Position=poMainFormCenter
        Text = "修改网关路由";
        StartPosition = FormStartPosition.CenterParent;
        Location = new System.Drawing.Point(409, 188);
        ClientSize = new System.Drawing.Size(451, 461);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: Label1 Left=8 Top=12 Width=54 Height=12 Caption='角色网关:'
        Label1 = new Label { Left = 8, Top = 12, Width = 54, Height = 12, Text = "角色网关:" };
        // DFM: EditSelGate Left=64 Top=8 Width=97 Height=20 TabOrder=0 OnChange=EditSelGateChange
        EditSelGate = new TextBox { Left = 64, Top = 8, Width = 97, Height = 20, TabIndex = 0 };

        // DFM: GroupBox1 Left=8 Top=32 Width=436 Height=117
        //      Caption='主游戏网关 （网关IP - 网关连接端口 - 网关检测端口）' TabOrder=1
        GroupBox1 = new GroupBox
        {
            Left = 8,
            Top = 32,
            Width = 436,
            Height = 117,
            Text = "主游戏网关 （网关IP - 网关连接端口 - 网关检测端口）",
            TabIndex = 1
        };

        // DFM: Label2..Label5 Left=8 Top=24/47/70/93 Caption='一:'..'四:'
        Label2 = new Label { Left = 8, Top = 24, Width = 18, Height = 12, Text = "一:" };
        Label3 = new Label { Left = 8, Top = 47, Width = 18, Height = 12, Text = "二:" };
        Label4 = new Label { Left = 8, Top = 70, Width = 18, Height = 12, Text = "三:" };
        Label5 = new Label { Left = 8, Top = 93, Width = 18, Height = 12, Text = "四:" };
        // DFM: Label6..Label9 Left=224 Top=24/48/72/93 Caption='五:'..'八:'
        Label6 = new Label { Left = 224, Top = 24, Width = 18, Height = 12, Text = "五:" };
        Label7 = new Label { Left = 224, Top = 48, Width = 18, Height = 12, Text = "六:" };
        Label8 = new Label { Left = 224, Top = 72, Width = 18, Height = 12, Text = "七:" };
        Label9 = new Label { Left = 224, Top = 93, Width = 18, Height = 12, Text = "八:" };

        _gateIPs = new[] { new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox() };
        _gatePorts = new[] { new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox() };
        _dbPorts = new[] { new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox(), new TextBox() };

        // DFM（脚本 dfm_compact 抽取，Left/Top/Width=97(IP) 或 41(端口)/Height=20，OnChange=EditSelGateChange）：
        //   第1组 edtGateIP1(28,20)   edtGatePort1(127,20)  edtDBPort1(170,20)   TabOrder 0/1/16
        //   第2组 edtGateIP2(28,43)   edtGatePort2(127,43)  edtDBPort2(170,43)   TabOrder 2/3/17
        //   第3组 edtGateIP3(28,66)   edtGatePort3(127,66)  edtDBPort3(170,66)   TabOrder 4/5/18
        //   第4组 edtGateIP4(28,89)   edtGatePort4(127,89)  edtDBPort4(170,89)   TabOrder 6/7/19
        //   第5组 edtGateIP5(244,20)  edtGatePort5(343,20)  edtDBPort5(387,20)   TabOrder 8/9/20
        //   第6组 edtGateIP6(244,43)  edtGatePort6(343,43)  edtDBPort6(387,43)   TabOrder 10/11/21
        //   第7组 edtGateIP7(244,66)  edtGatePort7(343,66)  edtDBPort7(387,66)   TabOrder 12/13/22
        //   第8组 edtGateIP8(244,89)  edtGatePort8(343,89)  edtDBPort8(387,89)   TabOrder 14/15/23
        int[] ipLeft = { 28, 28, 28, 28, 244, 244, 244, 244 };
        int[] ipTop = { 20, 43, 66, 89, 20, 43, 66, 89 };
        int[] portLeft = { 127, 127, 127, 127, 343, 343, 343, 343 };
        int[] dbLeft = { 170, 170, 170, 170, 387, 387, 387, 387 };
        int[] tabBase = { 0, 2, 4, 6, 8, 10, 12, 14 };
        int[] dbTabBase = { 16, 17, 18, 19, 20, 21, 22, 23 };
        for (int i = 0; i < 8; i++)
        {
            _gateIPs[i] = new TextBox { Left = ipLeft[i], Top = ipTop[i], Width = 97, Height = 20, TabIndex = tabBase[i] };
            _gatePorts[i] = new TextBox { Left = portLeft[i], Top = ipTop[i], Width = 41, Height = 20, TabIndex = tabBase[i] + 1 };
            _dbPorts[i] = new TextBox { Left = dbLeft[i], Top = ipTop[i], Width = 41, Height = 20, TabIndex = dbTabBase[i] };
        }
        edtGateIP1 = _gateIPs[0]; edtGateIP2 = _gateIPs[1]; edtGateIP3 = _gateIPs[2]; edtGateIP4 = _gateIPs[3];
        edtGateIP5 = _gateIPs[4]; edtGateIP6 = _gateIPs[5]; edtGateIP7 = _gateIPs[6]; edtGateIP8 = _gateIPs[7];
        edtGatePort1 = _gatePorts[0]; edtGatePort2 = _gatePorts[1]; edtGatePort3 = _gatePorts[2]; edtGatePort4 = _gatePorts[3];
        edtGatePort5 = _gatePorts[4]; edtGatePort6 = _gatePorts[5]; edtGatePort7 = _gatePorts[6]; edtGatePort8 = _gatePorts[7];
        edtDBPort1 = _dbPorts[0]; edtDBPort2 = _dbPorts[1]; edtDBPort3 = _dbPorts[2]; edtDBPort4 = _dbPorts[3];
        edtDBPort5 = _dbPorts[4]; edtDBPort6 = _dbPorts[5]; edtDBPort7 = _dbPorts[6]; edtDBPort8 = _dbPorts[7];

        // DFM: grp1 Left=8 Top=152 Width=436 Height=273 TabOrder=2
        grp1 = new GroupBox { Left = 8, Top = 152, Width = 436, Height = 273, TabIndex = 2 };
        // DFM: lbl1 Left=8 Top=21 Width=84 Height=12 Caption='当主游戏网关有'
        lbl1 = new Label { Left = 8, Top = 21, Width = 84, Height = 12, Text = "当主游戏网关有" };
        // DFM: lbl2 Left=148 Top=21 Width=192 Height=12 Caption='个不能连接时，不再分配主游戏网关'
        lbl2 = new Label { Left = 148, Top = 21, Width = 192, Height = 12, Text = "个不能连接时，不再分配主游戏网关" };
        // DFM: chkOpenRunGate2 Left=8 Top=-2 Width=117 Height=17 Caption='启用备用游戏网关' TabOrder=0 OnClick=chkOpenRunGate2Click
        chkOpenRunGate2 = new CheckBox { Left = 8, Top = -2, Width = 117, Height = 17, Text = "启用备用游戏网关", TabIndex = 0 };
        // DFM: seGameGateDisconnectCount Left=96 Top=16 Width=49 Height=21 Enabled=False MaxValue=8 MinValue=1 TabOrder=1 Value=1
        seGameGateDisconnectCount = new TSpinEditEx { Left = 96, Top = 16, Width = 49, Height = 21, TabIndex = 1, Enabled = false };
        seGameGateDisconnectCount.SetDfmRange(1, 8);
        seGameGateDisconnectCount.Value = 1;

        // DFM: vstRunGate Left=8 Top=40 Width=417 Height=204 Enabled=False TabOrder=2
        //      Header 列: 编号(60) / 网关IP(113) / 连接端口(80) / 检测端口(80) / 优先级别(80)
        //      TreeOptions.MiscOptions 含 toCheckSupport / toEditable；OnGetText/OnPaintText/OnKeyDown/OnNodeClick
        vstRunGate = new ListView
        {
            Left = 8,
            Top = 40,
            Width = 417,
            Height = 204,
            TabIndex = 2,
            Enabled = false,
            CheckBoxes = true,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false,
            View = View.Details
        };
        vstRunGate.Columns.Add("编号", 60);
        vstRunGate.Columns.Add("网关IP", 113);
        vstRunGate.Columns.Add("连接端口", 80);
        vstRunGate.Columns.Add("检测端口", 80);
        vstRunGate.Columns.Add("优先级别", 80);

        // DFM: btnAdd Left=309 Top=247 Width=56 Height=20 Caption='新增' Enabled=False TabOrder=3 OnClick=btnAddClick
        btnAdd = new Button { Left = 309, Top = 247, Width = 56, Height = 20, Text = "新增", TabIndex = 3, Enabled = false };
        // DFM: btnDel Left=368 Top=247 Width=56 Height=20 Caption='删除' Enabled=False TabOrder=4 OnClick=btnDelClick
        btnDel = new Button { Left = 368, Top = 247, Width = 56, Height = 20, Text = "删除", TabIndex = 4, Enabled = false };
        // DFM: btnCancel Left=369 Top=430 Width=73 Height=25 Cancel=True Caption='取消(&C)' ModalResult=2 TabOrder=3
        btnCancel = new Button { Left = 369, Top = 430, Width = 73, Height = 25, Text = "取消(&C)", TabIndex = 3, DialogResult = DialogResult.Cancel };
        // DFM: btnOK Left=289 Top=430 Width=73 Height=25 Caption='确定(&O)' TabOrder=4 OnClick=btnOKClick
        btnOK = new Button { Left = 289, Top = 430, Width = 73, Height = 25, Text = "确定(&O)", TabIndex = 4 };

        GroupBox1.Controls.Add(Label2);
        GroupBox1.Controls.Add(Label3);
        GroupBox1.Controls.Add(Label4);
        GroupBox1.Controls.Add(Label5);
        GroupBox1.Controls.Add(Label6);
        GroupBox1.Controls.Add(Label7);
        GroupBox1.Controls.Add(Label8);
        GroupBox1.Controls.Add(Label9);
        foreach (var t in _gateIPs) GroupBox1.Controls.Add(t);
        foreach (var t in _gatePorts) GroupBox1.Controls.Add(t);
        foreach (var t in _dbPorts) GroupBox1.Controls.Add(t);

        grp1.Controls.Add(lbl1);
        grp1.Controls.Add(lbl2);
        grp1.Controls.Add(chkOpenRunGate2);
        grp1.Controls.Add(seGameGateDisconnectCount);
        grp1.Controls.Add(vstRunGate);
        grp1.Controls.Add(btnAdd);
        grp1.Controls.Add(btnDel);

        Controls.Add(Label1);
        Controls.Add(EditSelGate);
        Controls.Add(GroupBox1);
        Controls.Add(grp1);
        Controls.Add(btnCancel);
        Controls.Add(btnOK);

        // DFM: frmRouteEdit OnCreate=FormCreate
        Load += (s, e) => FormCreate(s, e);
        btnOK.Click += (s, e) => btnOKClick(s, e);
        btnAdd.Click += (s, e) => btnAddClick(s, e);
        btnDel.Click += (s, e) => btnDelClick(s, e);
        chkOpenRunGate2.Click += (s, e) => chkOpenRunGate2Click(s, e);

        // DFM: EditSelGate 与全部 edtGateIP*/edtGatePort*/edtDBPort* 的 OnChange 都是 EditSelGateChange
        EditSelGate.TextChanged += (s, e) => EditSelGateChange(s, e);
        foreach (var t in _gateIPs) t.TextChanged += (s, e) => EditSelGateChange(s, e);
        foreach (var t in _gatePorts) t.TextChanged += (s, e) => EditSelGateChange(s, e);
        foreach (var t in _dbPorts) t.TextChanged += (s, e) => EditSelGateChange(s, e);

        vstRunGate.ItemChecked += (s, e) =>
        {
            int i = vstRunGate.Items.IndexOf(e.Item);
            if (i >= 0 && i < Nodes.Count)
                Nodes[i].CheckState = e.Item.Checked ? TCheckState.csCheckedNormal : TCheckState.csUncheckedNormal;
        };
    }

    /// <summary>RouteEdit.pas:667-671 `FormCreate`：FIsChanged := False；vstRunGate.NodeDataSize := SizeOf(TRunGateInfo)。</summary>
    public void FormCreate(object Sender, EventArgs e)
    {
        FIsChanged = 0;
        // vstRunGate.NodeDataSize := SizeOf(TRunGateInfo)  → 托管侧由 TRunGateNode 承载，无对应设置
    }

    /// <summary>RouteEdit.pas:375-445 `DoOpen`（控件 ← FRouteInfo，节点 ← RunGate2List）。</summary>
    public void DoOpen()
    {
        var inputs = new RouteEditInputs();
        RouteEditLogic.DoOpen(FRouteInfo, inputs, Nodes, ref FIsChanged);

        EditSelGate.Text = inputs.SelGate;
        for (int i = 0; i < 8; i++)
        {
            _gateIPs[i].Text = inputs.GateIP[i];
            _gatePorts[i].Text = inputs.GatePort[i];
            _dbPorts[i].Text = inputs.DBPort[i];
        }
        chkOpenRunGate2.Checked = inputs.OpenRunGate2;
        seGameGateDisconnectCount.Value = inputs.GameGateDisconnectCount;

        SyncTreeView();
        SetRunGateControlsEnabled(inputs.OpenRunGate2);

        FIsChanged = 0;
    }

    /// <summary>RouteEdit.pas:447-637 `btnOKClick`。</summary>
    public void btnOKClick(object Sender, EventArgs e)
    {
        var inputs = CollectInputs();
        BtnOKOutcome outcome = RouteEditLogic.BtnOK(inputs, FRouteInfo, Nodes, ref FIsChanged);
        if (outcome.FocusTarget == "EditSelGate") EditSelGate.Focus();
        else if (outcome.FocusTarget == "edtGateIP1") edtGateIP1.Focus();
        else if (outcome.FocusTarget == "edtGatePort1") edtGatePort1.Focus();
        if (outcome.FocusNodeIndex >= 0)
        {
            if (outcome.FocusNodeIndex < vstRunGate.Items.Count)
            {
                vstRunGate.Items[outcome.FocusNodeIndex].Selected = true;
                vstRunGate.FocusedItem = vstRunGate.Items[outcome.FocusNodeIndex];
            }
        }
        if (outcome.ModalResult != TModalResult.mrNone)
        {
            DialogResult = DialogResult.OK;   // ModalResult := mrOK
        }
    }

    /// <summary>RouteEdit.pas:673-704 `btnAddClick`。</summary>
    public void btnAddClick(object Sender, EventArgs e)
    {
        TRunGateNode node;
        if (!RouteEditLogic.BtnAdd(Nodes, ref FIsChanged, out node)) return;
        SyncTreeView();
        if (vstRunGate.Items.Count > 0)
        {
            vstRunGate.Items[vstRunGate.Items.Count - 1].Selected = true;
            vstRunGate.FocusedItem = vstRunGate.Items[vstRunGate.Items.Count - 1];
        }
    }

    /// <summary>RouteEdit.pas:722-743 `btnDelClick`。</summary>
    public void btnDelClick(object Sender, EventArgs e)
    {
        int focused = vstRunGate.FocusedItem != null ? vstRunGate.FocusedItem.Index : -1;
        int newFocus;
        if (!RouteEditLogic.BtnDel(Nodes, focused, ref FIsChanged, out newFocus)) return;
        SyncTreeView();
        if (newFocus >= 0 && newFocus < vstRunGate.Items.Count)
        {
            vstRunGate.Items[newFocus].Selected = true;
            vstRunGate.FocusedItem = vstRunGate.Items[newFocus];
        }
    }

    /// <summary>RouteEdit.pas:757-763 `chkOpenRunGate2Click`。</summary>
    public void chkOpenRunGate2Click(object Sender, EventArgs e)
    {
        SetRunGateControlsEnabled(chkOpenRunGate2.Checked);
    }

    /// <summary>RouteEdit.pas:778-781 `EditSelGateChange`：任何编辑框变更都置 FIsChanged := True。</summary>
    public void EditSelGateChange(object Sender, EventArgs e)
    {
        FIsChanged = 1;
    }

    private void SetRunGateControlsEnabled(bool enabled)
    {
        seGameGateDisconnectCount.Enabled = enabled;
        vstRunGate.Enabled = enabled;
        btnAdd.Enabled = enabled;
        btnDel.Enabled = enabled;
    }

    private RouteEditInputs CollectInputs()
    {
        var inputs = new RouteEditInputs
        {
            SelGate = EditSelGate.Text,
            OpenRunGate2 = chkOpenRunGate2.Checked,
            GameGateDisconnectCount = seGameGateDisconnectCount.Value
        };
        for (int i = 0; i < 8; i++)
        {
            inputs.GateIP[i] = _gateIPs[i].Text;
            inputs.GatePort[i] = _gatePorts[i].Text;
            inputs.DBPort[i] = _dbPorts[i].Text;
        }
        return inputs;
    }

    /// <summary>RouteEdit.pas:706-720/765-776 的 ListView 呈现（GetText + IsConnect 红字）。</summary>
    public void SyncTreeView()
    {
        vstRunGate.BeginUpdate();
        try
        {
            vstRunGate.Items.Clear();
            for (int i = 0; i < Nodes.Count; i++)
            {
                TRunGateNode node = Nodes[i];
                node.Index = i;
                var it = new ListViewItem(RouteEditLogic.GetText(node, 0)) { Tag = node, Checked = node.CheckState == TCheckState.csCheckedNormal };
                it.SubItems.Add(RouteEditLogic.GetText(node, 1));
                it.SubItems.Add(RouteEditLogic.GetText(node, 2));
                it.SubItems.Add(RouteEditLogic.GetText(node, 3));
                it.SubItems.Add(RouteEditLogic.GetText(node, 4));
                if (RouteEditLogic.PaintTextIsRed(node)) it.ForeColor = System.Drawing.Color.Red;
                vstRunGate.Items.Add(it);
            }
        }
        finally
        {
            vstRunGate.EndUpdate();
        }
    }
}
