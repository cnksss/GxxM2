using System;
using System.Windows.Forms;
using GXX.Core.Util;

// 接缝层（p10-m2-misc 车道）：GateSet.pas（302 行，源 `Source/LoginSrv/GateSet.pas`）所需的
// `LSShare.pas` 片段与 Delphi 控件语义修正。
// 本文件**不**移植 GateSet 本身（那是 GateSetForm.cs），只提供：
//   ① TGNet/TGateRoute 记录模型 + g_Config 的 nRouteCount/GateRoute 注入点；
//   ② LSShare.pas:507-535 `SaveGateConfig()` 的 1:1 落地（**应随 LSShare 整单元移植搬走**，见类注释）；
//   ③ `TComboBox.ItemIndex` 的 Delphi 语义安全赋值（§21.3 已知陷阱：空列表上赋 0 在 WinForms 会抛）；
//   ④ `Beep` 注入点（原文 `GateSet.pas:186`）。
// 登记见 docs/并行报告-p10-m2-misc.md（D-P10-12…D-P10-15 / X-P10-02）。

namespace GXX.LoginSrv.Forms2;

/// <summary>`LSShare.pas:140-144 TGateNet`（字段 1:1）。</summary>
public sealed class TGateNet
{
    public string sIPaddr = "";
    public int nPort;
    public bool boEnable;
}

/// <summary>`LSShare.pas:145-152 TGateRoute`（字段 1:1；`Gate: array[0..9] of TGateNet`）。</summary>
public sealed class TGateRoute
{
    public string sServerName = "";
    public string sTitle = "";
    public string sRemoteAddr = "";
    public string sPublicAddr = "";
    public int nSelIdx;

    /// <summary>`Gate: array[0..9] of TGateNet`（Delphi 的静态数组元素是**已初始化的记录** ⇒ 托管侧预建 10 个实例）。</summary>
    public TGateNet[] Gate = CreateGate();

    private static TGateNet[] CreateGate()
    {
        var a = new TGateNet[10];
        for (int i = 0; i < a.Length; i++) a[i] = new TGateNet();
        return a;
    }
}

/// <summary>
/// 接缝：`LSShare.pas:226 g_Config.nRouteCount` 与 `:240 g_Config.GateRoute[0..59]`。
///
/// <para>
/// 托管 `GXX.LoginSrv.LoginSrvShare.TConfig`（**本车道分区外，不可改**）目前**没有**这两个成员 ——
/// 它的类注释已写明「接缝：LSShare.pas 其余部分（… GateRoute …）待 LSShare 整单元移植后接入，
/// 本文件**不**代为移植」。而 `GateSet.pas` 的 5 个过程（CbGateListChange / BtnOkClick /
/// BtnChangeTitleClick / CbServerListChange / RefRouteList）**全都**直接读写它们。
/// </para>
/// <para>
/// ⇒ 本车道在自己分区内提供**同名同型**的注入点，并登记跨区请求 **X-P10-02**：
/// 待 `TConfig` 补上 `nRouteCount`/`GateRoute` 之后，把本类的两个静态成员改为**转调** `g_Config`
/// （只改 getter/setter，**不是**新增第二份实现，§14.2）。
/// </para>
/// </summary>
public static class GateSetConfigSeam
{
    /// <summary>`GateRoute: array[0..59]` 的元素个数（`High = 59`）。</summary>
    public const int GateRouteCount = 60;

    /// <summary>`Gate: array[0..9]` 的元素个数（`High = 9`）。</summary>
    public const int GateCount = 10;

    /// <summary>`g_Config.GateRoute`。</summary>
    public static TGateRoute[] GateRoute = CreateRoutes();

    /// <summary>`g_Config.nRouteCount`（原文由 `!addrtable.txt` 解析时填充）。</summary>
    public static int nRouteCount;

    /// <summary>`LSShare.pas:507-535 SaveGateConfig()`（可注入；默认写 `.\!addrtable.txt`）。</summary>
    public static Action SaveGateConfig = DefaultSaveGateConfig;

    /// <summary>`SaveGateConfig` 的落盘目标（原文硬编码 `'.\!addrtable.txt'`，相对**当前工作目录**）。</summary>
    public static string AddrTableFile = @".\!addrtable.txt";

    /// <summary>`GateSet.pas:186` 的 `Beep`（原文 Windows API `MessageBeep(0)`）。</summary>
    public static Action Beep = () => System.Media.SystemSounds.Beep.Play();

    private static TGateRoute[] CreateRoutes()
    {
        var a = new TGateRoute[GateRouteCount];
        for (int i = 0; i < a.Length; i++) a[i] = new TGateRoute();
        return a;
    }

    /// <summary>
    /// `LSShare.pas:507-535 SaveGateConfig()` 1:1。
    ///
    /// <para>
    /// ★ 原文要点（逐条保留）：
    /// ① 头两行是固定注释行（`';No space allowed'` 与带 `GenSpaceString` 列宽的列头）；
    /// ② 逐路由拼一行：4 个字段各按 15/15/17/17 补空格，再按顺序追加各网关 `IP:Port`
    ///   （**`boEnable` 为假时在最前面加 `'*'`**），每个网关字段按 17 补空格；
    /// ③ 网关列的循环以 `sIPaddr = ''` 为**终止条件** ⇒ 中间出现空槽会**截断**其后所有网关
    ///   （原文如此，不是"跳过空槽"）；
    /// ④ 落盘 `'.\!addrtable.txt'`（**无 try/except** ⇒ 写失败会抛给调用方 `BtnOkClick`）。
    /// </para>
    /// </summary>
    public static void DefaultSaveGateConfig()
    {
        var SaveList = new GXX.Core.Util.TStringList();
        SaveList.Add(";No space allowed");
        SaveList.Add(LoginSrvShare.GenSpaceString(";Server", 15)
            + LoginSrvShare.GenSpaceString("Title", 15)
            + LoginSrvShare.GenSpaceString("Remote", 17)
            + LoginSrvShare.GenSpaceString("Public", 17)
            + "Gate...");
        for (int i = 0; i <= nRouteCount - 1; i++)
        {
            string sC = LoginSrvShare.GenSpaceString(GateRoute[i].sServerName, 15)
                + LoginSrvShare.GenSpaceString(GateRoute[i].sTitle, 15)
                + LoginSrvShare.GenSpaceString(GateRoute[i].sRemoteAddr, 17)
                + LoginSrvShare.GenSpaceString(GateRoute[i].sPublicAddr, 17);
            int n8 = 0;
            while (true)
            {
                string s10 = GateRoute[i].Gate[n8].sIPaddr;
                if (s10 == "") break;
                if (!GateRoute[i].Gate[n8].boEnable)
                    s10 = "*" + s10;
                s10 = s10 + ":" + GXX.Core.Rtl.DelphiRTL.IntToStr(GateRoute[i].Gate[n8].nPort);
                sC = sC + LoginSrvShare.GenSpaceString(s10, 17);
                n8++;
                if (n8 >= GateCount) break;
            }
            SaveList.Add(sC);
        }
        SaveList.SaveToFile(AddrTableFile);
    }

    /// <summary>测试/宿主复位（对应 Delphi 单元 initialization 的全局初值）。</summary>
    public static void ResetForTests()
    {
        GateRoute = CreateRoutes();
        nRouteCount = 0;
        SaveGateConfig = DefaultSaveGateConfig;
        AddrTableFile = @".\!addrtable.txt";
        Beep = () => System.Media.SystemSounds.Beep.Play();
        GateSetUi.ResetForTests();
    }
}

/// <summary>
/// 接缝：Delphi `Form.ShowModal`（`GateSet.pas:298 Self.ShowModal = mrOK`）。
/// 默认弹真实模态框；测试注入即返回 <see cref="DialogResult.OK"/> / <see cref="DialogResult.Cancel"/>
/// （`mrOk = 1 = DialogResult.OK`）。
/// </summary>
public static class GateSetUi
{
    public static Func<Form, DialogResult> ShowModal = f => f.ShowDialog();

    public static void ResetForTests() => ShowModal = f => f.ShowDialog();
}

/// <summary>
/// `TComboBox.ItemIndex` 的 Delphi 语义安全赋值（§21.3 已知陷阱）。
///
/// <para>
/// Delphi `TComboBox.SetItemIndex` 内部走 `CB_SETCURSEL`：**越界/空列表时返回 CB_ERR，
/// ItemIndex 原样不变**（不抛异常）。WinForms `ComboBox.SelectedIndex = n`
/// 在 `n >= Items.Count` 时抛 <see cref="ArgumentOutOfRangeException"/>。
/// `GateSet.pas:266/:289` 就在"下拉框可能为空"的路径上赋 0（`CbGateList.ItemIndex := 0`
/// 紧跟 `CbGateList.Clear` 之后）⇒ 必须按 Delphi 语义钳制，否则**空路由配置打开窗体即抛**。
/// </para>
/// </summary>
public static class P10cComboBox
{
    /// <summary>Delphi `ItemIndex := Value`：`Value &lt; -1` 先钳到 -1；越界则**不改动**（CB_ERR）。</summary>
    public static void SetItemIndex(ComboBox combo, int value)
    {
        if (value < -1) value = -1;
        if (value == -1)
        {
            combo.SelectedIndex = -1;
            return;
        }
        if (value >= combo.Items.Count) return;      // Delphi：CB_SETCURSEL 失败 ⇒ ItemIndex 不变
        combo.SelectedIndex = value;
    }

    /// <summary>Delphi `ItemIndex` 读值（WinForms `SelectedIndex` 未选中时为 -1，语义一致）。</summary>
    public static int GetItemIndex(ComboBox combo) => combo.SelectedIndex;
}
