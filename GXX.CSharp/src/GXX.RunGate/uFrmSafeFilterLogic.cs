using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmSafeFilter.pas 1:1 转换（Source\RunGate\uFrmSafeFilter.pas，1497 行 / LF 1496）。
// 布局真源：Source\RunGate\uFrmSafeFilter.dfm（**同名 .dfm 存在**，共 1034 行）。本车道最大的一个。
//
// 窗体职责（网络安全过滤）：6 个列表 + 23 个参数控件 + 6 个右键菜单：
//   * 当前连接（lstActive）、动态过滤 IP（lstTemp）、永久过滤 IP（lstBlock）、过滤 IP 段（lstIpSection）、
//     动态/永久 MAC（lstTempMac/lstBlockMac）；
//   * 连接保护 / 攻击操作 / 流量控制 / 防 CC / 发言设置 / 防御设置 / 验证客户端 / 非法包检查；
//   * 确定时把 23 个参数写回全局量 + Config.ini 的 `[GameGate]` 27 个键。
//
// 接缝：
//   * ISafeFilterContextHost ← TIOCPClientContextPool / TMirClientContext（MirClientContext.pas 未移植）
//   * ISafeFilterListHost    ← g_TempIPList / g_BlockIPList / g_IPSectionList / g_TempMacList / g_BlockMacList
//   * ISafeFilterPersistence ← SaveBlockIPList / SaveIPSectionList / SaveBlockMacList / AddBlockIP / AddTempBlockIP ...
//   * MessageBoxSeam / InputQueryWithValue（原文用自建的 `InputQueryEx` 对话框，见下）
//
// ★ 原文要点与缺陷（照抄 + 差异断言）：
//   D1. `mniTempAddAllToBlockClick`（原 :537-554）**循环体用的是 `lstTemp.Items[lstTemp.ItemIndex]`
//       而不是 `[I]`** —— 即"全部加入永久过滤"实际上会把**当前选中项重复加 N 次**。
//       这是原文的真实缺陷，照抄并在测试中固定（不"顺手修正"）。
//   D2. `mniBlockClearClick`（原 :601-605）**没有 Lock/UnLock**（同族其它清空操作都有）。
//   D3. `mniTempMacAddToBlockClick`（原 :1326）用 `g_TempMacList.Delete(lstTempMac.ItemIndex)`
//       （**用 UI 下标删列表**），而同族的 `mniTempMacDeleteClick`（原 :1294-1296）用
//       `g_TempMacList.IndexOf(lstTempMac.Items[ItemIndex])` 后再 Delete —— 两者语义不同：
//       若列表与 UI 顺序不一致（例如 UI Sorted=True 后），前者会删错项。这是原文缺陷。
//   D4. `mniBlockMacAddToTempMacClick`（原 :1439）同样用 `g_BlockMacList.Delete(lstBlockMac.ItemIndex)`（同 D3）。
//   D5. 自建对话框 `InputQueryEx`（原 :230-325）：比 `Dialogs.InputQuery` 多一个 **Hint 标签**
//       （蓝色文字），宽度 FORM_WIDTH=280 对话框单位，Edit 的 MaxLength=255，默认选中全部文本。
//       本移植保留三参数接缝（Caption / Prompt / Hint），**不复刻自绘布局**（见接缝说明）。
//   D6. `GetIPAddrFromActiveItem`（原 :923-926）= `Trim(Copy(S, 1, 18))`；
//       `GetUserNameFromActiveItem`（原 :928-931）= `Copy(S, 19, MaxInt)`。
//       而 `Open`（原 :392-393）写入时用的是 `Format('%-18s', [sIPaddr]) + Context.sChrName`，
//       **没有** `%-20s` 的用户名前缀（那段被注释掉了，原 :382-387）。
//   D7. `mniIpSectionAddClick`（原 :1120-1152）：起始/结束 IP 任一为 INADDR_NONE 则弹错返回；
//       结束 < 起始时弹 '结束地址不能小于开始地址'（**不含**等于的情况 —— 等于允许）。
//   D8. `mniActiveAddToTempClick`（原 :945）用**裸** `Application.MessageBox(...)` 判定，
//       而 `mniActiveAddToBlockClick`（原 :991）用 `QuestionMessage`（标题 '' → '询问'）。
//       两者提示文本与标题构造不同：前者标题 = '确认信息 - ' + IP，后者也传了标题；
//       但 `mniActiveAddAllToTempClick`/`...ToBlock`/`...NoUserTo*`（原 :963/:1013/:1037/:1066）
//       只传文本（标题走 '询问'）。
//   D9. `lstActiveKeyDown`（原 :1210-1244）：仅 `Ctrl+F` 触发查找；对 `lstTempMac`/`lstBlockMac`
//       提示 '输入MAC'/'请输入要查找的MAC地址'，其它 listbox 提示 '输入IP'/'请输入要查找的IP地址'；
//       查找用 `SameText`（大小写不敏感 + 忽略空白），命中即设 ItemIndex 并 **Break**。
//   D10. `tfrmSafeFilter.btnOKClick`（原 :683-803）**没有 try..finally**（与 uFrmReadFileIP 同），
//        且末尾是 `Close`（不是 `ModalResult := mrOK`）—— 但 `ShowFrmSafeFilter` 判的是
//        `ShowModal = mrOk`（原 :337）→ **确定按钮永远返回 False**！这是原文缺陷，照抄并断言。
//   D11. `btnOKClick` 写了 27 个键，其中 `MaxClientMsgCount` 写的是全局常量
//        `nMaxClientMsgCount`（原 :740），**不是** `g_nMaxClientPacketCount`（那个是 :739 的
//        `MaxClientPacketCount`）。两个键名相似但值来源不同 —— 易错点。
//   D12. `UpdateHints`（原 :861-881）的 5 组 Hint 文本**成对赋值**（chk 与 se 共用同一串）。
// =====================================================================================

/// <summary>接缝：待 `IocpTcpServer.pas` / `MirClientContext.pas` 移植后接入。
/// 原文用到 `TIOCPClientContextPool.Instance.GetOnlineContextList(List)` 与
/// `TMirClientContext.RemoteAddr` / `.sChrName` / `.Close`。</summary>
public interface ISafeFilterClient
{
    string RemoteAddr { get; }
    string ChrName { get; }
    void Close();
}

/// <summary>接缝：在线连接池。</summary>
public interface ISafeFilterClientPool
{
    /// <summary>原 :376 / :893 `GetOnlineContextList(List)`；返回顺序即原文 `List` 顺序（**倒序遍历**）。</summary>
    List<ISafeFilterClient> GetOnlineContextList();
}

/// <summary>接缝：GateShare.pas 的 5 个列表（`g_TempIPList` / `g_BlockIPList` / `g_IPSectionList` /
/// `g_TempMacList` / `g_BlockMacList`）+ 3 个 Save 函数 + Add/AddTemp 函数。</summary>
public interface ISafeFilterHost
{
    // --- 动态/永久 IP 列表（元素为 IP 字符串的托管替身；原文为 pTSockaddr）---
    TSafeHashStringList TempIPList { get; }
    TSafeHashStringList BlockIPList { get; }

    // --- IP 段列表（原文为 TList of PTIPSection）---
    List<IPSECTION> IPSectionList { get; }

    // --- MAC 列表 ---
    TSafeHashStringList TempMacList { get; }
    TSafeHashStringList BlockMacList { get; }

    // --- 原文 free 函数 ---
    void AddBlockIP(string ip);
    void AddTempBlockIP(string ip);
    void AddBlockMac(string mac);
    void AddTempBlockMac(string mac);
    void SaveBlockIPList();
    void SaveIPSectionList();
    void SaveBlockMacList();
}

/// <summary>原 GateShare.pas `TIPSection`（nBeginAddr / nEndAddr 两个 LongWord）。</summary>
public class IPSECTION
{
    public uint nBeginAddr;
    public uint nEndAddr;

    /// <summary>原 :405 `Long2IP(IPSection.nBeginAddr) + ' - ' + Long2IP(IPSection.nEndAddr)`。</summary>
    public string Display => RunGateNet.Long2IP(nBeginAddr) + " - " + RunGateNet.Long2IP(nEndAddr);   // 原 :1147
}

/// <summary>`WinSock` 的 IP 转换（原 SafeFilter 依赖 WinSock.IP2Long / Long2IP / INADDR_NONE / inet_ntoa）。</summary>
public static class RunGateNet
{
    /// <summary>原 WinSock `INADDR_NONE`（= 255.255.255.255 的 32 位无符号值）；原文用 `LongWord(INADDR_NONE)` 比较。</summary>
    public const uint INADDR_NONE = 0xFFFFFFFF;

    /// <summary>原 WinSock `IP2Long`（解析失败返回 INADDR_NONE）。</summary>
    public static uint IP2Long(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return INADDR_NONE;
        string[] parts = ip.Trim().Split('.');
        if (parts.Length != 4) return INADDR_NONE;
        uint result = 0;
        foreach (string p in parts)
        {
            if (p.Length == 0) return INADDR_NONE;
            foreach (char c in p) if (c < '0' || c > '9') return INADDR_NONE;
            if (!int.TryParse(p, out int v) || v < 0 || v > 255) return INADDR_NONE;
            result = (result << 8) | (uint)v;
        }
        return result;
    }

    /// <summary>原 WinSock `Long2IP`。</summary>
    public static string Long2IP(uint value)
        => ((value >> 24) & 0xFF) + "." + ((value >> 16) & 0xFF) + "." + ((value >> 8) & 0xFF) + "." + (value & 0xFF);

    /// <summary>GateShare.pas `IsIPaddr`（GXX.Core.Util.HUtil32.IsIpaddr 的等价实现）。</summary>
    public static bool IsIPaddr(string ip) => IP2Long(ip) != INADDR_NONE || ip == "255.255.255.255";
}

/// <summary>uFrmSafeFilter.pas 的非 UI 逻辑（可单测）。</summary>
public static class SafeFilterLogic
{
    // ---------------------------------------------------------------------------------
    // 原 :923-931 的两个 Format 解析函数
    // ---------------------------------------------------------------------------------

    /// <summary>原 :923-926 `GetIPAddrFromActiveItem(S) = Trim(Copy(S, 1, 18))`。</summary>
    public static string GetIPAddrFromActiveItem(string s)
        => DelphiRTL.Trim(DelphiRTL.Copy(s ?? "", 1, 18));

    /// <summary>原 :928-931 `GetUserNameFromActiveItem(S) = Copy(S, 19, MaxInt)`。</summary>
    public static string GetUserNameFromActiveItem(string s)
        => DelphiRTL.Copy(s ?? "", 19, DelphiRTL.MaxInt);

    /// <summary>原 :392-393 `Format('%-18s', [sIPaddr]) + Context.sChrName`（左对齐 18 宽）。
    /// ★ `%-Ns` 在 Delphi 中是**右填充空格到 N 宽**（不足补空格，超出不截断）。</summary>
    public static string FormatActiveItem(string ipAddr, string chrName)
    {
        string padded = ipAddr ?? "";
        if (padded.Length < 18) padded = padded.PadRight(18);
        return padded + (chrName ?? "");                                   // 原 :390-393
    }

    /// <summary>原 :378-395 / :895-912 的 `lstActive` 填充（`for I := List.Count - 1 downto 0` → **倒序**）。
    /// 只保留 `RemoteAddr &lt;&gt; ''` 的连接。</summary>
    public static List<string> BuildActiveItems(IReadOnlyList<ISafeFilterClient> contexts)
    {
        var items = new List<string>();
        for (int i = contexts.Count - 1; i >= 0; i--)                       // 原 :378 / :895 倒序
        {
            string ip = contexts[i].RemoteAddr;                             // 原 :389 / :906
            if (!string.IsNullOrEmpty(ip))                                  // 原 :390 / :907
                items.Add(FormatActiveItem(ip, contexts[i].ChrName));       // 原 :392-393 / :909-910
        }
        return items;
    }

    // ---------------------------------------------------------------------------------
    // 原 :861-881 UpdateHints（5 组 Hint，逐字含中文）
    // ---------------------------------------------------------------------------------

    /// <summary>原 :863-864 `trckbrDefenseLevel.Hint`。</summary>
    public static string DefenseLevelHint(int defenseLevel)
        => "调整范围在：1-10。分别是：严格-宽松。" +
           "等级为0时关闭攻击防御！当前等级：" + DelphiRTL.IntToStr(defenseLevel);      // 原 :863-864

    /// <summary>原 :866-868 `chkDefenseToLevel1.Hint` 与 `seDefenseToLevel1.Hint`（同一串）。</summary>
    public static string DefenseToLevel1Hint(int defenseToLevel1)
        => "被攻击" + DelphiRTL.IntToStr(defenseToLevel1) +
           "次后，如果你的防御等级不是1级，程序将自动调整你的防御等级为1级";              // 原 :866-867

    /// <summary>原 :870-872 `seResotreDefense.Hint` 与 `chkResotreDefense.Hint`（同一串）。</summary>
    public static string ResotreDefenseHint(int resotreDefense)
        => "每隔" + DelphiRTL.IntToStr(resotreDefense) +
           "秒自动清除动态过滤列表中的IP";                                              // 原 :870-871

    /// <summary>原 :874-876 `chkAutoClearTemp.Hint` 与 `seAutoClearTemp.Hint`（同一串）。</summary>
    public static string AutoClearTempHint(int autoClearTemp)
        => "在没有攻击后，等待" + DelphiRTL.IntToStr(autoClearTemp) +
           "秒程序将防御等级还原成你最初的设置";                                         // 原 :874-875

    /// <summary>原 :878-880 `chkAddAllToTemp.Hint` 与 `seAddAllToTemp.Hint`（同一串）。</summary>
    public static string AddAllToTempHint(int addAllToTemp)
        => "当连接数达到" + DelphiRTL.IntToStr(addAllToTemp) +
           "时，将链接列表的所有IP加入动态过滤";                                        // 原 :878-879

    /// <summary>把 5 组 Hint 汇总成"控件名 → 文本"（便于测试逐条断言，也便于窗体统一赋值）。</summary>
    public static Dictionary<string, string> BuildHints(int defenseLevel, int defenseToLevel1, int resotreDefense,
                                                        int autoClearTemp, int addAllToTemp)
    {
        var d = new Dictionary<string, string>();
        string h1 = DefenseLevelHint(defenseLevel);
        string h2 = DefenseToLevel1Hint(defenseToLevel1);
        string h3 = ResotreDefenseHint(resotreDefense);
        string h4 = AutoClearTempHint(autoClearTemp);
        string h5 = AddAllToTempHint(addAllToTemp);

        d["trckbrDefenseLevel"] = h1;        // 原 :863
        d["chkDefenseToLevel1"] = h2;        // 原 :866
        d["seDefenseToLevel1"] = h2;         // 原 :868
        d["seResotreDefense"] = h3;          // 原 :870
        d["chkResotreDefense"] = h3;         // 原 :872
        d["chkAutoClearTemp"] = h4;          // 原 :874
        d["seAutoClearTemp"] = h4;           // 原 :876
        d["chkAddAllToTemp"] = h5;           // 原 :878
        d["seAddAllToTemp"] = h5;            // 原 :880
        return d;
    }

    // ---------------------------------------------------------------------------------
    // 原 :687-692 / :731 的 BlockMethod 单选 → 枚举
    // ---------------------------------------------------------------------------------

    /// <summary>原 :687-692：先判 rbDisConnect，再 rbAddTempList，**else 一律 bmBlockList**
    /// （即 rbAddBlockList 未选中时也落到 bmBlockList —— 与"三个都未选"等价）。</summary>
    public static TBlockIPMethod ResolveBlockMethod(bool rbDisConnectChecked, bool rbAddTempListChecked)
    {
        if (rbDisConnectChecked) return TBlockIPMethod.bmDisconnect;      // 原 :688
        if (rbAddTempListChecked) return TBlockIPMethod.bmTempBlock;      // 原 :690
        return TBlockIPMethod.bmBlockList;                                // 原 :692
    }

    // ---------------------------------------------------------------------------------
    // 原 :1197-1208 的两个弹窗封装
    // ---------------------------------------------------------------------------------

    /// <summary>原 :1197-1200 `ErrMessage`：标题固定 '错误'，图标 MB_ICONERROR。</summary>
    public static void ErrMessage(string msgStr)
        => MessageBoxSeam.ShowError(msgStr, "错误");                        // 原 :1199

    /// <summary>原 :1202-1208 `QuestionMessage`：标题为空时用 '询问'，MB_OKCANCEL + MB_ICONQUESTION，返回是否 IDOK。</summary>
    public static bool QuestionMessage(string msgStr, string msgTitle)
    {
        if (string.IsNullOrEmpty(msgTitle)) msgTitle = "询问";              // 原 :1204-1205
        return MessageBoxSeam.Ask(msgStr, msgTitle);                       // 原 :1206-1207
    }

    // ---------------------------------------------------------------------------------
    // 原 :482-649 / :1469-1493 的右键菜单使能规则（6 组，结构完全同形）
    // ---------------------------------------------------------------------------------

    /// <summary>菜单使能规则（原各 `pm*Popup` 的共同形状）：
    /// * 排序/清空/"全部加入"三项 = `Items.Count &gt; 0`；
    /// * 删除/单项加入两项 = `ItemIndex in [0, Count)`。</summary>
    public static void ComputePopupState(int itemCount, int itemIndex,
                                        out bool sortClearAddAllEnabled, out bool itemSpecificEnabled)
    {
        sortClearAddAllEnabled = itemCount > 0;                                         // 原 :629-631 等
        itemSpecificEnabled = itemIndex >= 0 && itemIndex < itemCount;                   // 原 :633 等
    }

    /// <summary>原 :625-636 `pmTempPopup`（4 个菜单项的使能）。</summary>
    public static void PopupStateTemp(int itemCount, int itemIndex,
                                     out bool sort, out bool clear, out bool addAll, out bool del, out bool addTo)
    {
        ComputePopupState(itemCount, itemIndex, out sort, out bool itemSpecific);
        clear = sort;      // 原 :630
        addAll = sort;     // 原 :631
        del = itemSpecific;     // 原 :634
        addTo = itemSpecific;   // 原 :635
    }

    /// <summary>原 :1105-1118 `pmActiveChange`（**注意**：没有"清空"项，只有 sort/addAll×2/单项×3）。</summary>
    public static void PopupStateActive(int itemCount, int itemIndex,
                                       out bool sort, out bool addAllToTemp, out bool addAllToBlock,
                                       out bool addToTemp, out bool addToBlock, out bool kick)
    {
        sort = itemCount > 0;                                    // 原 :1110
        addAllToTemp = sort;                                     // 原 :1111
        addAllToBlock = sort;                                    // 原 :1112
        bool itemSpecific = itemIndex >= 0 && itemIndex < itemCount;   // 原 :1114
        addToTemp = itemSpecific;                                // 原 :1115
        addToBlock = itemSpecific;                               // 原 :1116
        kick = itemSpecific;                                     // 原 :1117
    }

    /// <summary>原 :1187-1195 `pmIpSectionPopup`（**只有** sort 与 del 两项）。</summary>
    public static void PopupStateIpSection(int itemCount, int itemIndex, out bool sort, out bool del)
    {
        sort = itemCount > 0;                                                       // 原 :1191
        del = itemIndex >= 0 && itemIndex < itemCount;                              // 原 :1194
    }

    // ---------------------------------------------------------------------------------
    // 原 :1120-1152 mniIpSectionAddClick 的校验分支
    // ---------------------------------------------------------------------------------

    /// <summary>IP 段校验结果。</summary>
    public enum IPSectionResult
    {
        OK = 0,
        BeginInvalid = 1,     // 原 :1129-1133
        EndInvalid = 2,       // 原 :1136-1140
        EndLessThanBegin = 3  // 原 :1150-1151
    }

    /// <summary>原 :1126-1151 的三条校验（注意 `nEndaddr >= nBeginaddr` 才通过 —— **等于允许**）。</summary>
    public static IPSectionResult ValidateIPSection(string beginText, string endText,
                                                    out uint beginAddr, out uint endAddr)
    {
        beginAddr = RunGateNet.IP2Long(beginText);                          // 原 :1128
        if (beginAddr == RunGateNet.INADDR_NONE) { endAddr = 0; return IPSectionResult.BeginInvalid; }   // 原 :1129

        endAddr = RunGateNet.IP2Long(endText);                              // 原 :1135
        if (endAddr == RunGateNet.INADDR_NONE) return IPSectionResult.EndInvalid;                        // 原 :1136

        if (endAddr >= beginAddr) return IPSectionResult.OK;                // 原 :1141（等于也算通过）
        return IPSectionResult.EndLessThanBegin;                            // 原 :1150-1151
    }

    /// <summary>原 :1128-1133 的弹窗文本。</summary>
    public const string MsgIPFormatError = "输入的地址格式不正确！";
    public const string MsgIPFormatErrorTitle = "提示信息";
    public const string MsgEndLessThanBegin = "结束地址不能小于开始地址";

    /// <summary>原 :810/831 的两个"永久IP过滤/永久IP过滤"对话框标题与提示。</summary>
    public const string CaptionIpFilter = "永久IP过滤";
    public const string PromptIpInput = "请输入一个新的IP地址: ";
    public const string HintIpExample = "如：202.103.100.20";

    /// <summary>原 :1127/1134 的 IP 段对话框标题与提示。</summary>
    public const string CaptionIpSection = "过滤IP段信息";
    public const string PromptIpSectionBegin = "请输入起始IP地址: ";
    public const string PromptIpSectionEnd = "请输入结束IP地址: ";
    public const string HintIpSectionBegin = "如：202.103.100.1";
    public const string HintIpSectionEnd = "如：202.103.100.100";

    /// <summary>原 :812 `ErrMessage('输入的地址格式错误！')` 与 :834 `ErrMessage('输入的IP地址错误')`
    /// —— ★ 两处文本**不同**（同一功能的两个菜单项），差异断言点。</summary>
    public const string MsgTempAddBadIp = "输入的地址格式错误！";
    public const string MsgBlockAddBadIp = "输入的IP地址错误";

    /// <summary>原 :1272/1385 的 MAC 对话框标题与提示。</summary>
    public const string CaptionTempMac = "动态MAC过滤";
    public const string CaptionBlockMac = "永久MAC过滤";
    public const string PromptMacInput = "请输入一个新的MAC地址: ";

    /// <summary>原 :1224-1230 `lstActiveKeyDown` 的对话框标题/提示（按控件区分 MAC 与 IP）。</summary>
    public static void InputQueryTitlesFor(bool isMacList, out string caption, out string prompt)
    {
        if (isMacList) { caption = "输入MAC"; prompt = "请输入要查找的MAC地址"; }   // 原 :1224-1225
        else { caption = "输入IP"; prompt = "请输入要查找的IP地址"; }               // 原 :1229-1230
    }

    /// <summary>原 :1234-1241：在 listbox 里按 `SameText` 找第一条命中项，返回下标（未命中 -1）。</summary>
    public static int FindListItem(IReadOnlyList<string> items, string input)
    {
        for (int i = 0; i < items.Count; i++)                               // 原 :1234
        {
            if (string.Equals((items[i] ?? ""), (input ?? ""), StringComparison.OrdinalIgnoreCase))   // 原 :1236 SameText
                return i;
        }
        return -1;
    }

    /// <summary>原 :190-196 / :266-270 / :272-276 等"加入永久/动态列表并 Save"的核心动作。
    /// 返回加入后的列表内容快照（便于断言）。</summary>
    public static List<string> AddAllToBlockWithBug(IReadOnlyList<string> tempItems, int selectedIndex,
                                                    Action<string> addBlockIp)
    {
        // ★ D1 复刻：原 :542-548 用 `lstTemp.Items[lstTemp.ItemIndex]`（**当前选中项**）而不是 `[I]`。
        var added = new List<string>();
        for (int i = 0; i < tempItems.Count; i++)                           // 原 :542
        {
            string ip = selectedIndex >= 0 && selectedIndex < tempItems.Count
                ? tempItems[selectedIndex] : "";                            // 原 :544（原文是 Items[ItemIndex]）
            added.Add(ip);
            addBlockIp?.Invoke(ip);                                         // 原 :547
        }
        return added;
    }
}

/// <summary>原 :328-341 `function ShowFrmSafeFilter(MainForm: TForm): Boolean;`。</summary>
public static class SafeFilterUnit
{
    public static bool ShowFrmSafeFilter(Form mainForm)
    {
        using var form = new FrmSafeFilter();
        if (mainForm != null)                                                        // 原 :334-335
        {
            form.Left = mainForm.Left + (mainForm.Width - form.Width) / 2;
            form.Top = mainForm.Top + (mainForm.Height - form.Height) / 2;
        }
        form.Open();                                                                 // 原 :336
        return form.ShowDialog() == DialogResult.OK;                                 // 原 :337 ShowModal = mrOk
    }
}
