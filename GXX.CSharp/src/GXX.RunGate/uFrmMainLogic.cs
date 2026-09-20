// =====================================================================================
// 源单元：Source\RunGate\uFrmMain.pas（Delphi 7，GBK）—— **主窗体纯逻辑切片**
//   实测 LF = 4216 行。本文件**只覆盖不依赖缺失接缝的纯函数**（其余 90+ 个例程见报告 §uFrmMain 处置表）：
//     :426-439   GetSizeString
//     :2188-2207 tmrRefreshInfoTimer 的**运行时长格式化**（纯内核，原文内联在定时器里）
//     :2490-2494 DllCheckMessage（**VERSION_TYPE = 2 → 死代码**，登记不移植）
//     :2942-2965 ExtractSelfTimeDateStamp（含嵌套 :2948-2954 UnixDateToDateTime）
//     :3413-3466 btnSearchClick / btnNextSearchClick 的判定内核
//     :3612-3766 btnSearchOnlineClick / btnSearchOnlineNextClick 的判定内核
//     :3777-3794 pmProcessListPopup 的可见性判定
//     :3837-3899 RecallPreAllocatedSize / lblRecommendPreAllocatedCountClick 的算式
//
//   行号口径：`[IO.File]::ReadAllText(p) -split "`n"` 得到的**物理 LF 行号**（与 read 工具一致）。
//   ★ 警告：`Get-Content`/`Select-String` 对该文件的行号会**漂移**（到 EOF 时 +17），不要混用。
//
//   条件编译：`uFrmMain.pas` 的 `uses` 里有 `Grobal2_Ex`（VERSION_TYPE/CLIENT_ANTIPLUG 的来源），
//   `VERSION_TYPE = 2` → `:359/:535/:2393/:2427` 的 `{$IF VERSION_TYPE = 1}` 段是死代码；
//   `:364/:2413` 的 `{$IF VERSION_TYPE <> 0}` 为真。`DllCheckMessage`（:2490）在死分支里 → 不移植。
//
// ── 原文缺陷登记（照抄语义 + 差异断言）───────────────────────────────────────────────
//   D1. [:3449] `if FSearchIndex >= lvContextProcessListInfo.Items.Count - 1 then FSearchIndex := 0;`
//       —— 应为 `>= Count`。于是 `FSearchIndex = Count - 1`（合法值）会被重置为 0，
//       "下一个"**永远搜不到最后一行**；而 `:3433/:3462` 存的正是 `I + 1`（可等于 Count）。
//   D2. [:3429 / :3458] `ListItem.Selected;` 是**空的属性读取**（应为 `Selected := True`），无副作用。
//   D3. [:3782-3785] 早退分支里 `ListItem.SubItems[0]`：当 `SubItems.Count = 0` 且 `ListItem <> nil` 时，
//       Delphi 的 `TListItem.SubItems[0]` 会抛 `EListError`（越界）。托管侧按安全语义返回 ""（已登记差异）。
//   D4. [:3627 / :3707] 模糊/精确匹配都以 `Item.SubItems.Count > 4` 为前置条件；
//       不满足时 `IsFound` 保持 False（**不报错、不提示**）。
//   D5. [:3630-3633] 模糊匹配用 `Pos(...) > 0`（**大小写敏感**），且 Delphi 的 `Pos('', S) = 1`
//       → **空关键字匹配一切**。而 `GXX.Core.Rtl.DelphiRTL.Pos("")` 返回 **0**（前任报告 §5.25-1 已登记该跨库差异）
//       → 本文件显式补偿，见 `PosDelphi`。
//   D6. [:3663-3666] 精确匹配用 `SameText`（大小写不敏感 + **全等**），与模糊匹配的语义**不同**。
//   D7. [:2188-2207] 运行时长格式化：`Day/Hour/Min/Sec` 的进位与"0 段省略"规则照抄
//       （`Day=0 and Hour=0 and Min=0` 时只显示秒）。原文用 `tick_diff` 先把 tick 差算出来。
//   D8. [:2952-2953] `UnixDateToDateTime` **硬编码 UTC+8**（`IncHour(Result, 8)`），无时区接缝。
//   D9. [:3842 / :3890] 循环上界是 **64**（不是 63），注释写"64 * 128 = 8K, 最大8K缓冲区"；
//       `Max := I shl 7` 与 `Break` 判据 `Max >= seClientSendBlockSize.Value shl 10` 照抄，
//       故 `seClientSendBlockSize.Value = 0` 时**第一轮就 Break**（Max=0 >= 0）。
//   D10.[:3853] `MemSize := MemSize + MAX_IOCP_CLIENT_RECV_BUFFER_SIZE * 60;` 在
//       `{$IF UseIocpClient <> 0}` 内，而 `UseIocpClient = 1`（IocpCommon.pas:14）→ **生效**
//       （+1 MiB × 60 = 62,914,560 字节）。
//   D11.[:3845] `SizeOf(OVERLAPPEDEx)` 依赖 Win32 结构布局：`OVERLAPPED`(20) + `TWSABUF`(8)
//       + `TIoType`(4) + `AllocSize`(4) = **36**（32 位对齐，无 packed）。标 **推断**（无法在本环境编译 Delphi 复核）。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GXX.RunGate;

/// <summary>uFrmMain.pas 的纯逻辑切片（主窗体其余部分需要 TMirClientContext/TRunGateManager/TIODataPool 等缺失接缝）。</summary>
public static class RunGateMainLogic
{
    // ==================================================================================
    // :426-439 GetSizeString
    // ==================================================================================

    /// <summary>原文 :426-439 `function GetSizeString(B: Int64): string;`
    /// 1024 进制，阈值依次 TB/GB/MB/KB/B，`%.2f` 保留两位小数（`else` 分支是 `%dB` 整数）。
    /// 依据：`Format('%.2fTB', [B / 1099511627776])` 等（:429/:431/:433/:435/:437）。</summary>
    public static string GetSizeString(long B)
    {
        if (B >= 1099511627776L)                                    // 原 :428
            return (B / 1099511627776.0).ToString("F2", CultureInfo.InvariantCulture) + "TB";
        else if (B >= 1073741824L)                                  // 原 :430
            return (B / 1073741824.0).ToString("F2", CultureInfo.InvariantCulture) + "GB";
        else if (B >= 1048576L)                                     // 原 :432
            return (B / 1048576.0).ToString("F2", CultureInfo.InvariantCulture) + "MB";
        else if (B >= 1024L)                                        // 原 :434
            return (B / 1024.0).ToString("F2", CultureInfo.InvariantCulture) + "KB";
        else
            return B.ToString(CultureInfo.InvariantCulture) + "B";   // 原 :437 `Format('%dB', [B])`
    }

    // ==================================================================================
    // :2188-2207 运行时长格式化（原内联在 tmrRefreshInfoTimer 里）
    // ==================================================================================

    /// <summary>原文 :2188-2207 的 `SRunTime` 生成（`TimeInterval` 已经由 `tick_diff` 算好）。
    /// `MSecsPerDay = 86400000`、`MSecsPerSec * 60 * 60 = 3600000`、`MSecsPerSec * 60 = 60000`、`MSecsPerSec = 1000`。</summary>
    public static string FormatRunTime(uint TimeInterval)
    {
        uint Day = TimeInterval / 86400000u;                        // 原 :2189
        uint Remain = TimeInterval % 86400000u;                     // 原 :2190

        uint Hour = Remain / 3600000u;                              // 原 :2192
        Remain %= 3600000u;                                         // 原 :2193

        uint Min = Remain / 60000u;                                 // 原 :2195
        Remain %= 60000u;                                           // 原 :2196
        uint Sec = Remain / 1000u;                                  // 原 :2197

        if (Day > 0)                                                // 原 :2200-2201
            return Day + "天" + Hour + "时" + Min + "分" + Sec + "秒";
        else if (Hour > 0)                                          // 原 :2202-2203
            return Hour + "时" + Min + "分" + Sec + "秒";
        else if (Min > 0)                                           // 原 :2204-2205
            return Min + "分" + Sec + "秒";
        else
            return Sec + "秒";                                      // 原 :2206-2207
    }

    // ==================================================================================
    // :2942-2965 ExtractSelfTimeDateStamp
    // ==================================================================================

    /// <summary>原文 :2948-2954 嵌套函数 `UnixDateToDateTime(const USec: Longint): TDateTime;`
    /// `Result := (USec / 86400) + 25569.0;` 再 `IncHour(Result, 8)`（★ 缺陷 D8：硬编码 UTC+8）。
    /// `25569.0` 是 1970-01-01 相对 Delphi `TDateTime` 纪元（1899-12-30）的天数。</summary>
    public static DateTime UnixDateToDateTime(int USec)
    {
        const double UnixStartDate = 25569.0;                       // 原 :2950
        double result = (USec / 86400.0) + UnixStartDate;            // 原 :2952
        return DateTime.FromOADate(result).AddHours(8);              // 原 :2953 `IncHour(Result, 8)`
    }

    /// <summary>原文 :2942-2965 `function TFrmMain.ExtractSelfTimeDateStamp: TDateTime;`
    /// 从 PE 镜像里取 `IMAGE_NT_HEADERS.FileHeader.TimeDateStamp`（Unix 秒）。
    /// 原文直接读 `ParamStr(0)`（自己的 exe）；托管侧取字节数组以便单测。
    /// 断言不足（DOS/NT 头越界）时返回 <see cref="DateTime.MinValue"/>（原文会 AV 或读垃圾 —— 已登记差异）。</summary>
    public static DateTime ExtractSelfTimeDateStamp(byte[] peImage)
    {
        // 原 :2960 `tmpDosHeader := Pointer(tmpMM.Memory);`
        if (peImage == null || peImage.Length < 0x40) return DateTime.MinValue;

        // 原 :2961 `tmpNtHeader := Pointer(NativeUInt(tmpDosHeader^._lfanew) + NativeUInt(tmpDosHeader));`
        int lfanew = BitConverter.ToInt32(peImage, 0x3C);
        // IMAGE_NT_HEADERS: Signature(4) + FileHeader.Machine(2) + NumberOfSections(2) → TimeDateStamp 在 NT + 8
        int tsOffset = lfanew + 8;
        if (lfanew < 0 || tsOffset + 4 > peImage.Length) return DateTime.MinValue;

        // 原 :2962 `Result := UnixDateToDateTime(tmpNtHeader.FileHeader.TimeDateStamp);`
        return UnixDateToDateTime(BitConverter.ToInt32(peImage, tsOffset));
    }

    /// <summary>便利重载：读文件后取时间戳（对应原文 `tmpMM.LoadFromFile(ParamStr(0))`）。</summary>
    public static DateTime ExtractSelfTimeDateStampFromFile(string fileName)
        => File.Exists(fileName) ? ExtractSelfTimeDateStamp(File.ReadAllBytes(fileName)) : DateTime.MinValue;

    // ==================================================================================
    // :3837-3899 预分配内存估算
    // ==================================================================================

    /// <summary>原文 :3845 的 `SizeOf(OVERLAPPEDEx)`（★ 缺陷 D11：按 Win32 布局推断 = 36）。</summary>
    public const int OVERLAPPED_EX_SIZE = 36;

    /// <summary>原文 :3853 的 `MAX_IOCP_CLIENT_RECV_BUFFER_SIZE`（IocpCommon.pas:17 `1 shl 20` = 1048576）。</summary>
    public const int MAX_IOCP_CLIENT_RECV_BUFFER_SIZE = 1 << 20;

    /// <summary>原文 :3853 的 `* 60`（60 个 FIODataLists_2）。</summary>
    public const int IOCP_CLIENT_RECV_BUFFER_COUNT = 60;

    /// <summary>原文 :3856 的告警阈值 `400 shl 20`。</summary>
    public const int PreAllocatedTooLargeThreshold = 400 << 20;

    /// <summary>原文 :3837-3877 `procedure TFrmMain.RecallPreAllocatedSize;` 的结果。</summary>
    public struct PreAllocatedSizeResult
    {
        /// <summary>循环累加值（原文 `Count`，即 :3845 的 `Count := Count + Max + SizeOf(OVERLAPPEDEx)`）。</summary>
        public int Count;
        /// <summary>`Count * sePreAllocatedCount.Value` 再按需加 60 MiB（原文 `MemSize`）。</summary>
        public int MemSize;
        /// <summary>`MemSize >= 400 shl 20`（原文 :3856）。</summary>
        public bool IsTooLarge;
        /// <summary>`edtPreAllocatedSize.Text`（原文 :3871-3876，`%.2fMBytes/KBytes/Bytes`）。</summary>
        public string Text;
    }

    /// <summary>原文 :3837-3877 `RecallPreAllocatedSize` 的纯算式部分
    /// （原文尾部还会写控件颜色/提示文字；本方法只返回算式结果与文本）。
    /// ★ 循环上界 **64**（含）；`Break` 判据是 `Max &gt;= blockSizeKB shl 10`（★ 缺陷 D9）。</summary>
    public static PreAllocatedSizeResult RecallPreAllocatedSize(
        int clientSendBlockSizeValue, int preAllocatedCountValue,
        int overlappedExSize = OVERLAPPED_EX_SIZE,
        bool useIocpClient = true,
        int maxIocpClientRecvBufferSize = MAX_IOCP_CLIENT_RECV_BUFFER_SIZE)
    {
        int Count = 0;                                              // 原 :3841
        for (int I = 0; I <= 64; I++)                               // 原 :3842 `for I := 0 to 64`
        {
            int Max = I << 7;                                       // 原 :3844 `Max := I shl 7`
            Count = Count + Max + overlappedExSize;                 // 原 :3845
            if (Max >= clientSendBlockSizeValue << 10) break;       // 原 :3846
        }

        int MemSize = Count * preAllocatedCountValue;               // 原 :3849

        if (useIocpClient)                                          // 原 :3851 `{$IF UseIocpClient <> 0}`（= 1 → 生效）
            MemSize = MemSize + maxIocpClientRecvBufferSize * IOCP_CLIENT_RECV_BUFFER_COUNT;   // 原 :3853

        bool tooLarge = MemSize >= PreAllocatedTooLargeThreshold;   // 原 :3856

        string text;
        if (MemSize >= 1048576)                                     // 原 :3871
            text = (MemSize / 1048576.0).ToString("F2", CultureInfo.InvariantCulture) + "MBytes";
        else if (MemSize >= 1024)                                   // 原 :3873
            text = (MemSize / 1024.0).ToString("F2", CultureInfo.InvariantCulture) + "KBytes";
        else
            text = MemSize.ToString("F2", CultureInfo.InvariantCulture) + "Bytes";   // 原 :3876

        return new PreAllocatedSizeResult
        {
            Count = Count,
            MemSize = MemSize,
            IsTooLarge = tooLarge,
            Text = text,
        };
    }

    /// <summary>原文 :3885-3899 `procedure TFrmMain.lblRecommendPreAllocatedCountClick(Sender: TObject);`
    /// 用同一个累加器（初值 0）算 `MemSize`，再 `Count := 300 shl 20 div MemSize`。
    /// ★ 不含 `UseIocpClient` 的 60 MiB 项（原文这里没有那段 `{$IF}`）。</summary>
    public static int RecommendPreAllocatedCount(
        int clientSendBlockSizeValue, int overlappedExSize = OVERLAPPED_EX_SIZE)
    {
        int MemSize = 0;                                            // 原 :3889
        for (int I = 0; I <= 64; I++)                               // 原 :3890
        {
            int Max = I << 7;                                       // 原 :3892
            MemSize = MemSize + Max + overlappedExSize;              // 原 :3893
            if (Max >= clientSendBlockSizeValue << 10) break;       // 原 :3894
        }

        return (300 << 20) / MemSize;                               // 原 :3897 `300 shl 20 div MemSize`
    }

    // ==================================================================================
    // :3413-3466 进程列表搜索
    // ==================================================================================

    /// <summary>Delphi `Pos(SubStr, S)` 的等价物：**大小写敏感**，返回值 1-based，未找到为 0。
    /// ★ 缺陷 D5：`GXX.Core.Rtl.DelphiRTL.Pos("")` 返回 **0**，而 Delphi 的 `Pos('', S)` 返回 **1**
    /// → 这里显式补偿（空 needle 返回 1，表示"命中任何串"）。</summary>
    public static int PosDelphi(string needle, string haystack)
    {
        if (string.IsNullOrEmpty(needle)) return 1;                 // Delphi: Pos('', S) = 1
        int i = (haystack ?? "").IndexOf(needle, StringComparison.Ordinal);
        return i < 0 ? 0 : i + 1;
    }

    /// <summary>原文 :3419-3421 / :3445-3447 的关键字预处理：`Trim` → `UpperCase`；
    /// 结果为空串时**调用方应直接 Exit**（原文 :3420/:3446）。返回 null 表示"应 Exit"。</summary>
    public static string PrepareProcessSearchKey(string searchText)
    {
        string s = (searchText ?? "").Trim();                       // 原 :3419 `Trim(edtSearch.Text)`
        if (s.Length == 0) return null;                             // 原 :3420 `if Length(StrSearch) = 0 then Exit;`
        return s.ToUpperInvariant();                                // 原 :3421 `SysUtils.UpperCase(StrSearch)`
    }

    /// <summary>原文 :3426-3427（及 :3455-3456）：`S := UpperCase(ListItem.Caption); Pos(StrSearch, S) &gt; 0`。</summary>
    public static bool ProcessRowMatches(string caption, string preparedUpperKey)
        => PosDelphi(preparedUpperKey, (caption ?? "").ToUpperInvariant()) > 0;   // 原 :3426-3427

    /// <summary>原文 :3449-3450 的起始下标修正。★ 缺陷 D1：判据是 `>= Count - 1`（应为 `>= Count`），
    /// 于是 `FSearchIndex = Count - 1` 时被重置为 0 → 最后一行永远搜不到。</summary>
    public static int ProcessNextSearchStartIndex(int searchIndex, int itemCount)
        => searchIndex >= itemCount - 1 ? 0 : searchIndex;          // 原 :3449-3450

    // ==================================================================================
    // :3612-3766 在线用户搜索
    // ==================================================================================

    /// <summary>`cbbSearchOnlineField` 的 4 个选项（原文 :3629-3633 的 `case` 分支）。</summary>
    public const int OnlineFieldAccount = 0;    // 帐户 → SubItems[2]
    public const int OnlineFieldName = 1;       // 名称 → SubItems[3]
    public const int OnlineFieldIP = 2;         // IP   → SubItems[0]
    public const int OnlineFieldMac = 3;        // MAC  → SubItems[4]

    /// <summary>原文 :3629-3633 / :3662-3666 的 `case cbbSearchOnlineField.ItemIndex of` 到 `SubItems` 下标的映射。
    /// 越界/未知字段返回 **-1**（原文 `case` 无 `else` → `IsFound` 保持 False）。</summary>
    public static int OnlineSubItemIndexForField(int fieldIndex) => fieldIndex switch
    {
        OnlineFieldAccount => 2,                                 // 原 :3630 / :3663
        OnlineFieldName => 3,                                    // 原 :3631 / :3664
        OnlineFieldIP => 0,                                      // 原 :3632 / :3665
        OnlineFieldMac => 4,                                     // 原 :3633 / :3666
        _ => -1,
    };

    /// <summary>原文 :3622-3635（模糊）/ :3655-3668（精确）的单行判定：
    /// 前置条件 `Item.SubItems.Count &gt; 4`（★ 缺陷 D4：不满足时判为不匹配）；
    /// 模糊 = `Pos(needle, sub) &gt; 0`（大小写**敏感**，空 needle 命中一切，★ 缺陷 D5）；
    /// 精确 = `SameText(needle, sub)`（大小写**不敏感**全等，★ 缺陷 D6）。</summary>
    public static bool OnlineRowMatches(
        IReadOnlyList<string> subItems, string searchText, int fieldIndex, bool fuzzy)
    {
        if (subItems == null || subItems.Count <= 4) return false;   // 原 :3627 / :3660
        int idx = OnlineSubItemIndexForField(fieldIndex);
        if (idx < 0) return false;                                  // 原 `case` 无 else

        string sub = subItems[idx] ?? "";
        if (fuzzy)
            return PosDelphi(searchText ?? "", sub) > 0;             // 原 :3630-3633（大小写敏感）
        else
            return string.Equals(searchText ?? "", sub, StringComparison.OrdinalIgnoreCase);   // 原 :3663-3666 `SameText`
    }

    /// <summary>原文 :3694-3698 `StartIndex := lvOnLine.ItemIndex + 1;` 及其两次重置。
    /// `itemIndex = -1`（无选中）→ `0 + ...`：`0 &lt; 0` 为假、`0 &gt;= count` 仅在 count = 0 时为真 → 通常返回 0。</summary>
    public static int OnlineNextSearchStartIndex(int itemIndex, int itemCount)
    {
        int StartIndex = itemIndex + 1;                             // 原 :3694
        if (StartIndex < 0) return 0;                               // 原 :3695-3696
        if (StartIndex >= itemCount) return 0;                      // 原 :3697-3698
        return StartIndex;
    }

    // ==================================================================================
    // :3777-3794 pmProcessListPopup 可见性
    // ==================================================================================

    /// <summary>原文 :3777-3794 `pmProcessListPopup` 的三个菜单项可见性（无 ItemIndex 依赖，纯数据判定）。
    /// ★ 缺陷 D3：早退分支读 `SubItems[0]`，当 `SubItems.Count = 0` 时 Delphi 会抛 `EListError`；
    /// 托管侧用 <see cref="SubItemOrEmpty"/> 返回 ""（已登记差异）。</summary>
    public struct ProcessPopupVisibility
    {
        public bool AddBlackProcess;
        public bool SendFileToRungate;
        public bool SetRoot;
    }

    /// <summary>原文 :3777-3794 的判定。`hasSelection` = `ListItem &lt;&gt; nil`；
    /// `subItems` 为选中行的 `SubItems`；`isProcessList` = `FIsProcessList`。</summary>
    public static ProcessPopupVisibility GetProcessPopupVisibility(
        bool hasSelection, IReadOnlyList<string> subItems, bool isProcessList)
    {
        string sub0 = SubItemOrEmpty(subItems, 0);
        string sub1 = SubItemOrEmpty(subItems, 1);
        int subCount = subItems?.Count ?? 0;

        // 原 :3782 `if (ListItem = nil) or (ListItem.SubItems.Count < 2) or (Length(ListItem.SubItems[1]) = 0) then`
        if (!hasSelection || subCount < 2 || sub1.Length == 0)
        {
            return new ProcessPopupVisibility
            {
                AddBlackProcess = false,                                        // 原 :3784
                SetRoot = hasSelection && sub0 != "",                           // 原 :3785
                SendFileToRungate = false,                                      // 原 :3786
            };
        }

        return new ProcessPopupVisibility
        {
            AddBlackProcess = isProcessList,                                    // 原 :3791
            SendFileToRungate = !isProcessList,                                 // 原 :3792
            SetRoot = !isProcessList && sub0 != "",                             // 原 :3793
        };
    }

    /// <summary>`ListItem.SubItems[i]` 的安全等价物（越界返回 ""）。
    /// ★ 与 Delphi 的差异：`TListItem.SubItems[i]` 越界抛 `EListError`（缺陷 D3）。</summary>
    public static string SubItemOrEmpty(IReadOnlyList<string> subItems, int index)
        => (subItems != null && index >= 0 && index < subItems.Count) ? (subItems[index] ?? "") : "";
}
