// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（Delphi 7，GBK）—— **容器/记录类型族**
//   实测 LF = 3595 行。本文件覆盖：
//     :29-35    TSockaddr + pTSockaddr
//     :45-76    TAddressList（声明）
//     :78-88    TAddressInfo（record）
//     :90-115   TAddressListEx（声明）
//     :117-131  TSafeHashStringList（声明；**类体已在 uFrmGameSpeedLogic.cs 的既有接缝里**，
//               本轮按 §6.2「复用，不要另造」不重复定义，仅在本文件登记）
//     :133-147  TSafeStringList（声明）
//     :149-163  TSafeMemoryStream（声明）
//     :165-169  TProcessInfo（record）
//     :171-199  TProcessBlacklist（声明）
//     :211-215  TIPSection（record）
//     :2538-2704 TAddressList 实现
//     :2708-2834 TAddressListEx 实现
//     :2883-2924 TSafeStringList 实现
//     :2928-2969 TSafeMemoryStream 实现
//     :3261-3374 TProcessBlacklist 实现
//
// ★ 名字/语义对齐（本轮）：uFrmGameSpeedLogic.cs 里原有三个**接缝**同名类
//   `TSafeHashStringList` / `TProcessInfo` / `TProcessBlackList`。按车道纪律要求做了一次对齐：
//     * `TSafeHashStringList` —— 保留接缝（它已覆盖 GateShare 需要的全部子集），本文件不再定义。
//     * `TProcessInfo`       —— 接缝字段与原文 `:166-169` **逐字相同**，已移入本文件（唯一真源）。
//     * `TProcessBlackList`  —— 接缝多了大写 L，且 `MaxCount` 可写、`Add` 不做 `UpperCase`。
//                              已按原文类名 `TProcessBlacklist`（:171）与语义（:3293-3303）重写并替换；
//                              唯一保留的测试接缝是 `MaxCountForTest`（原文 `FMaxCount` 是 private、`MaxCount` 只读）。
//
// ★ 原文缺陷登记（照抄语义 + 差异断言；均未"顺手修正"）：
//   1. [:2619-2622] `TAddressList.Add` 与 [:2789-2792] `TAddressListEx.Add` 的
//      `if nIP = INADDR_NONE then Exit;` **是死代码**：
//      `nIP: Integer` 收下 `inet_addr` 的 $FFFFFFFF 后是 -1，而 `INADDR_NONE` 是无类型常量
//      $FFFFFFFF（Delphi 定型为 Cardinal）；`Integer = Cardinal` 的比较按 **Int64 提升**
//      → -1 vs 4294967295 恒不相等。后果：**非法 IP 字符串也会被加入名单**，
//      且 `nIPaddr` 存成 -1（等价于 255.255.255.255，见交接纪律里"不要用 -1 当哨兵"的告诫）。
//   2. [:2619 + :2627] `TAddressList.Add` 对同一 IP **调用两次 `inet_addr`**（原文如此）。
//   3. [:2613-2630] vs [:2783-2800] 两个 Add 的差别：后者把 `nIPaddr := nIP`（复用缓存），
//      前者重新 `inet_addr`；且只有后者保存 `sIPaddr` 字符串（TSockaddr 里没有该字段）。
//   4. [:2582 / :3350] `GetItems` 越界判据写法不一致：`TAddressList`/`TAddressListEx` 用
//      `(Index >= 0) and (Index <= Count - 1)`，`TProcessBlacklist` 用 `(Index >= 0) and (Index < Count)`。
//      两者语义相同，但按 1:1 照抄两种写法。
//   5. [:2670-2683 / :2821-2834 / :3316-3329] 三个 `Delete(x)` 都是**按引用相等**删除
//      （`FAddress.Items[I] = Sockaddr`），不是按值；`TAddressList` 另有 `Delete(IP: string)` 重载。
//   6. [:2883-2924] `TSafeStringList` 的 `Destroy` **不调用** `Clear`（对比 :2551-2562 的 `TAddressList.Destroy`
//      第一行是 `Clear`）——TStringList 自身会释放字符串，无害，但形态不一致。
//   7. [:2928-2969] `TSafeMemoryStream` 在整个 RunGate 里**没有任何使用点**（grep 全树只有声明/实现）；
//      属"备用设施"，本文件按 1:1 移植以便类型账目完整。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.RunGate;

/// <summary>原文 :29-34 `TSockaddr = record nIPaddr: Integer; dwStartAttackTick: LongWord;
/// nAttackCount: Integer; nSocketHandle: Integer; end;`（另见 :35 `pTSockaddr = ^TSockaddr`）。</summary>
public class TSockaddr
{
    public int nIPaddr;               // inet_addr 的**小端打包**值（见 GateShareInet 文件头）
    public uint dwStartAttackTick;
    public int nAttackCount;
    public int nSocketHandle;
}

/// <summary>原文 :78-88 `TAddressInfo = record`（另见 :77 `pTAddressInfo = ^TAddressInfo` 与
/// :108/:110 的 `PTAddressInfo` —— 原文两种大小写混用，Delphi 不敏感故等价）。</summary>
public class TAddressInfo
{
    public string sIPaddr = "";
    public int nIPaddr;
    public int nCount;
    public uint dwIPCountTick1;
    public int nIPCount1;
    public uint dwIPCountTick2;
    public int nIPCount2;
    public uint dwDenyTick;
    public int nIPDenyCount;
}

/// <summary>原文 :211-215 `TIPSection = record nBeginAddr: LongWord; nEndAddr: LongWord; end;`
/// （另见 :211 `PTIPSection = ^TIPSection`）。字段存的是 **IP2Long 的大端数值**。</summary>
public class TIPSection
{
    public uint nBeginAddr;
    public uint nEndAddr;
}

/// <summary>原文 :165-169 `TProcessInfo = record ProcessName: string; ProcessMD5: string; end;`
/// （另见 :165 `PTProcessInfo = ^TProcessInfo`）。</summary>
public class TProcessInfo
{
    public string ProcessName = "";
    public string ProcessMD5 = "";
}

/// <summary>原文 :45-76 声明 / :2538-2704 实现的 `TAddressList = class(TObject)`。
/// 元素为 `pTSockaddr`（托管侧引用类型 `TSockaddr`）。
///
/// 原文的 `{$IFDEF USE_SPINLOCK}` 分支在整个工程未定义 → 走 `TRTLCriticalSection` 分支；
/// 托管侧用 `Monitor`（Windows CRITICAL_SECTION 是**可重入**的，`lock` 语句块不满足"Lock/UnLock 分离 + 可重入"）。</summary>
public class TAddressList
{
    private readonly object FCS = new object();          // 原 :53 `FCS: TRTLCriticalSection`
    private readonly List<TSockaddr> FAddress = new List<TSockaddr>();

    /// <summary>原 :2538-2549 `constructor Create`。</summary>
    public TAddressList()
    {
        // 原 :2541-2547：USE_SPINLOCK 未定义 → InitializeCriticalSection(FCS)
        FAddress = new List<TSockaddr>();                // 原 :2548
    }

    /// <summary>原 :2551-2562 `destructor Destroy`：`Clear; FAddress.Free; ...; inherited Destroy;`。</summary>
    public void Destroy() => Clear();

    /// <summary>原 :2564-2578 `procedure Clear`。</summary>
    public void Clear()
    {
        Lock();                                          // 原 :2568
        try
        {
            FAddress.Clear();                            // 原 :2574（Dispose 逐项 → GC 等价）
        }
        finally
        {
            UnLock();                                    // 原 :2576
        }
    }

    /// <summary>原 :2580-2586 `function GetItems(Index: Integer): pTSockaddr`（默认属性 Items）。
    /// 越界（含负数、含 Index = Count）→ nil。</summary>
    public TSockaddr this[int Index]
        => (Index >= 0) && (Index <= FAddress.Count - 1) ? FAddress[Index] : null;   // 原 :2582

    /// <summary>原 :2588-2596 `procedure Lock`。</summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原 :2598-2606 `procedure UnLock`。</summary>
    public void UnLock() => Monitor.Exit(FCS);

    /// <summary>原 :2608-2611 `function GetCount: Integer`（属性 Count）。</summary>
    public int Count => FAddress.Count;

    /// <summary>原 :2613-2630 `function Add(IP: string): pTSockaddr`。</summary>
    public TSockaddr Add(string IP)
    {
        TSockaddr Result = null;                                     // 原 :2617
        if (string.IsNullOrEmpty(IP)) return null;                    // 原 :2618 `if Length(IP) = 0 then Exit;`

        int nIP = unchecked((int)GateShareInet.InetAddr(IP));         // 原 :2619（inet_addr → Integer 截断）
        // 原 :2620 `if nIP = INADDR_NONE then Exit;` —— ★ 死代码，见文件头缺陷 1。
        //   Delphi: Integer(-1) = Cardinal($FFFFFFFF) 经 Int64 提升后恒不相等。
        //   这里显式写成 Int64 比较以**保留**原文（失效的）语义。
        if ((long)nIP == (long)GateShareInet.INADDR_NONE) return null;

        Result = Find(IP);                                            // 原 :2622
        if (Result != null) return null;                              // 原 :2623

        Result = new TSockaddr();                                     // 原 :2625 `New(Result);`
        // 原 :2626 ZeroMemory(Result, SizeOf(TSockaddr)) —— new 已完成清零
        Result.nIPaddr = unchecked((int)GateShareInet.InetAddr(IP));  // 原 :2627（★ 第二次 inet_addr，见文件头缺陷 2）
        Result.nAttackCount = 0;                                      // 原 :2628
        FAddress.Add(Result);                                         // 原 :2629
        return Result;
    }

    /// <summary>原 :2632-2649 `function Find(IP: string): pTSockaddr`（按 nIPaddr 数值匹配，非字符串）。</summary>
    public TSockaddr Find(string IP)
    {
        int nIP = unchecked((int)GateShareInet.InetAddr(IP));         // 原 :2639
        for (int I = 0; I < FAddress.Count; I++)                      // 原 :2640
        {
            var Sockaddr = FAddress[I];                               // 原 :2642
            if (Sockaddr.nIPaddr == nIP) return Sockaddr;             // 原 :2643-2646
        }
        return null;                                                  // 原 :2638 `Result := nil;`
    }

    /// <summary>原 :2651-2668 `function FindIndex(IP: string): Integer`；未命中返回 **-1**。</summary>
    public int FindIndex(string IP)
    {
        int nIP = unchecked((int)GateShareInet.InetAddr(IP));         // 原 :2658
        for (int I = 0; I < FAddress.Count; I++)                      // 原 :2659
        {
            if (FAddress[I].nIPaddr == nIP) return I;                 // 原 :2662-2665
        }
        return -1;                                                    // 原 :2657
    }

    /// <summary>原 :2670-2683 `procedure Delete(Sockaddr: pTSockaddr); overload;`（**按引用**相等）。</summary>
    public void Delete(TSockaddr Sockaddr)
    {
        for (int I = 0; I < FAddress.Count; I++)                      // 原 :2674
        {
            if (ReferenceEquals(FAddress[I], Sockaddr))               // 原 :2676 `FAddress.Items[I] = Sockaddr`
            {
                FAddress.RemoveAt(I);                                 // 原 :2679
                return;                                               // 原 :2680
            }
        }
    }

    /// <summary>原 :2685-2692 `procedure Delete(IP: string); overload;`。</summary>
    public void Delete(string IP)
    {
        int I = FindIndex(IP);                                        // 原 :2689
        if (I != -1) DeleteIndex(I);                                  // 原 :2690-2691
    }

    /// <summary>原 :2694-2704 `procedure DeleteIndex(Index: Integer)`。</summary>
    public void DeleteIndex(int Index)
    {
        if ((Index >= 0) && (Index <= FAddress.Count - 1))            // 原 :2698
        {
            FAddress.RemoveAt(Index);                                 // 原 :2702
        }
    }
}

/// <summary>原文 :90-115 声明 / :2708-2834 实现的 `TAddressListEx = class(TObject)`。
/// 与 <see cref="TAddressList"/> 的差异：元素是 `pTAddressInfo`（带 sIPaddr 字符串与计数域），
/// 且 `Add` 保存 `sIPaddr`。</summary>
public class TAddressListEx
{
    private readonly object FCS = new object();           // 原 :98
    private readonly List<TAddressInfo> FAddress = new List<TAddressInfo>();

    /// <summary>原 :2708-2719 `constructor Create`。</summary>
    public TAddressListEx()
    {
        FAddress = new List<TAddressInfo>();              // 原 :2718
    }

    /// <summary>原 :2721-2732 `destructor Destroy`。</summary>
    public void Destroy() => Clear();

    /// <summary>原 :2734-2748 `procedure Clear`。</summary>
    public void Clear()
    {
        Lock();                                           // 原 :2738
        try
        {
            FAddress.Clear();                             // 原 :2744
        }
        finally
        {
            UnLock();                                     // 原 :2746
        }
    }

    /// <summary>原 :2750-2756 `function GetItems(Index: Integer): pTAddressInfo`（默认属性 Items）。</summary>
    public TAddressInfo this[int Index]
        => (Index >= 0) && (Index <= FAddress.Count - 1) ? FAddress[Index] : null;   // 原 :2752

    /// <summary>原 :2758-2766 `procedure Lock`。</summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原 :2768-2776 `procedure UnLock`。</summary>
    public void UnLock() => Monitor.Exit(FCS);

    /// <summary>原 :2778-2781 `function GetCount: Integer`（属性 Count）。</summary>
    public int Count => FAddress.Count;

    /// <summary>原 :2783-2800 `function Add(IP: string): PTAddressInfo`。</summary>
    public TAddressInfo Add(string IP)
    {
        TAddressInfo Result = null;                                   // 原 :2787
        if (string.IsNullOrEmpty(IP)) return null;                     // 原 :2788
        int nIP = unchecked((int)GateShareInet.InetAddr(IP));          // 原 :2789
        // 原 :2790 `if nIP = INADDR_NONE then Exit;` —— ★ 同为死代码，见文件头缺陷 1
        if ((long)nIP == (long)GateShareInet.INADDR_NONE) return null;

        Result = Find(IP);                                             // 原 :2792
        if (Result != null) return null;                               // 原 :2793

        Result = new TAddressInfo();                                   // 原 :2795
        Result.nIPaddr = nIP;                                          // 原 :2797（★ 复用缓存，对比 TAddressList 的二次 inet_addr）
        Result.sIPaddr = IP;                                           // 原 :2798
        FAddress.Add(Result);                                          // 原 :2799
        return Result;
    }

    /// <summary>原 :2802-2819 `function Find(IP: string): PTAddressInfo`。</summary>
    public TAddressInfo Find(string IP)
    {
        int nIP = unchecked((int)GateShareInet.InetAddr(IP));           // 原 :2809
        for (int I = 0; I < FAddress.Count; I++)                        // 原 :2810
        {
            var AddressInfo = FAddress[I];                              // 原 :2812
            if (AddressInfo.nIPaddr == nIP) return AddressInfo;         // 原 :2813-2816
        }
        return null;                                                    // 原 :2808
    }

    /// <summary>原 :2821-2834 `procedure Delete(AddressInfo: pTAddressInfo)`（按引用）。</summary>
    public void Delete(TAddressInfo AddressInfo)
    {
        for (int I = 0; I < FAddress.Count; I++)                        // 原 :2825
        {
            if (ReferenceEquals(FAddress[I], AddressInfo))              // 原 :2827
            {
                FAddress.RemoveAt(I);                                   // 原 :2830
                return;                                                 // 原 :2831
            }
        }
    }
}

/// <summary>原文 :133-147 声明 / :2883-2924 实现的 `TSafeStringList = class(TStringList)`。
/// 只是给 `TStringList` 加一把锁；托管侧复用 `GXX.Core.Util.TStringList`（已 1:1 覆盖
/// Delphi `TStrings` 的 Text/Add/IndexOf/LoadFromFile/SaveToFile 语义）。</summary>
public class TSafeStringList
{
    private readonly object FCS = new object();           // 原 :141
    private readonly TStringList Inner = new TStringList();

    /// <summary>原 :2883-2893 `constructor Create`。</summary>
    public TSafeStringList()
    {
        Inner = new TStringList();
    }

    /// <summary>原 :2895-2904 `destructor Destroy`（★ 不调用 Clear，见文件头缺陷 6）。</summary>
    public void Destroy() { }

    /// <summary>原 :2906-2914 `procedure Lock`。</summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原 :2916-2924 `procedure UnLock`。</summary>
    public void UnLock() => Monitor.Exit(FCS);

    /// <summary>`TStringList.Count`（原 `TStrings.Count`）。</summary>
    public int Count => Inner.Count;

    /// <summary>`TStrings.Strings[Index]`。</summary>
    public string this[int Index]
    {
        get => Inner[Index];
        set => Inner[Index] = value;
    }

    /// <summary>`TStrings.Add`。</summary>
    public int Add(string s) => Inner.Add(s);

    /// <summary>`TStrings.Clear`。</summary>
    public void Clear() => Inner.Clear();

    /// <summary>`TStrings.IndexOf`（Delphi 为**大小写不敏感**；`TStringList` 默认 CaseSensitive=False）。</summary>
    public int IndexOf(string s)
    {
        for (int i = 0; i < Inner.Count; i++)
            if (string.Equals(Inner[i], s, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    /// <summary>`TStrings.Delete(Index)`。</summary>
    public void Delete(int Index) => Inner.Delete(Index);

    /// <summary>`TStrings.Text`（读 = CRLF 连接；写 = SetTextStr 语义）。</summary>
    public string Text
    {
        get
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < Inner.Count; i++)
            {
                if (i > 0) sb.Append("\r\n");
                sb.Append(Inner[i]);
            }
            return sb.ToString();
        }
        set
        {
            Inner.Clear();
            if (string.IsNullOrEmpty(value)) return;
            var lines = value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            int n = lines.Length;
            if (n > 0 && lines[n - 1].Length == 0) n--;   // 末尾换行不产生额外空行
            for (int i = 0; i < n; i++) Inner.Add(lines[i]);
        }
    }

    /// <summary>`TStrings.SaveToFile`（GBK + CRLF）。</summary>
    public void SaveToFile(string FileName) => Inner.SaveToFile(FileName);

    /// <summary>`TStrings.LoadFromFile`（GBK）。</summary>
    public void LoadFromFile(string FileName) => Inner.LoadFromFile(FileName);

    /// <summary>枚举用快照（托管侧便利方法）。</summary>
    public string[] Lines
    {
        get
        {
            var a = new string[Inner.Count];
            for (int i = 0; i < Inner.Count; i++) a[i] = Inner[i];
            return a;
        }
    }
}

/// <summary>原文 :149-163 声明 / :2928-2969 实现的 `TSafeMemoryStream = class(TMemoryStream)`。
/// ★ 原文全树无使用点（见文件头缺陷 7）；此处只为类型账目 1:1 保留。</summary>
public class TSafeMemoryStream : MemoryStream
{
    private readonly object FCS = new object();           // 原 :157

    /// <summary>原 :2928-2938 `constructor Create`。</summary>
    public TSafeMemoryStream() : base() { }

    /// <summary>原 :2940-2949 `destructor Destroy`。</summary>
    public void Destroy() { }

    /// <summary>原 :2951-2959 `procedure Lock`。</summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原 :2961-2969 `procedure UnLock`。</summary>
    public void UnLock() => Monitor.Exit(FCS);
}

/// <summary>原文 :171-199 声明 / :3261-3374 实现的 `TProcessBlacklist = class(TObject)`。
/// `FMaxCount := 80`（:3271）；`Add` 会在**满 80** 或 **MD5 已存在**（`SameText`）时返回 nil，
/// 并把存入的 `ProcessMD5` **转成大写**（:3301 `UpperCase(ProcessMD5)`）。</summary>
public class TProcessBlacklist
{
    private int FMaxCount;                                // 原 :173（private，只读属性 MaxCount）
    private readonly List<TProcessInfo> FList = new List<TProcessInfo>();
    private readonly object FCS = new object();           // 原 :181

    /// <summary>原 :3261-3272 `constructor Create`。</summary>
    public TProcessBlacklist()
    {
        FList = new List<TProcessInfo>();                 // 原 :3263
        // 原 :3264-3270：USE_SPINLOCK 未定义 → InitializeCriticalSection(FCS)
        FMaxCount = 80;                                   // 原 :3271
    }

    /// <summary>原 :3274-3286 `destructor Destroy`。</summary>
    public void Destroy() => Clear();

    /// <summary>原 :189 `property MaxCount: Integer read FMaxCount;` —— **只读**。</summary>
    public int MaxCount => FMaxCount;

    /// <summary>测试接缝（原文无写属性；窗体测试用它把 80 调小以走"已满"分支）。
    /// 生产路径只走构造函数里的 80。</summary>
    public int MaxCountForTest { set => FMaxCount = value; }

    /// <summary>原 :3288-3291 `function GetCount: Integer`（属性 Count）。</summary>
    public int Count => FList.Count;

    /// <summary>原 :3348-3354 `function GetItems(Index: Integer): PTProcessInfo`（默认属性 Items）。
    /// 判据是 `(Index >= 0) and (Index < FList.Count)`（对比 TAddressList 的 `&lt;= Count-1`）。</summary>
    public TProcessInfo this[int Index]
        => (Index >= 0) && (Index < FList.Count) ? FList[Index] : null;   // 原 :3350

    /// <summary>窗体族便利属性：全部元素的快照（原文通过 `Items[I]` 逐个取）。</summary>
    public TProcessInfo[] Items => FList.ToArray();

    /// <summary>原 :3293-3303 `function Add(const ProcessName, ProcessMD5: string): PTProcessInfo`。</summary>
    public TProcessInfo Add(string ProcessName, string ProcessMD5)
    {
        // 原 :3295 `Result := nil;`
        if (FList.Count >= FMaxCount) return null;                 // 原 :3296
        if (Find(ProcessMD5) != null) return null;                 // 原 :3297

        var Result = new TProcessInfo();                           // 原 :3299
        Result.ProcessName = ProcessName;                          // 原 :3300
        Result.ProcessMD5 = DelphiRTL.UpperCase(ProcessMD5);       // 原 :3301（★ 接缝版本漏了 UpperCase）
        FList.Add(Result);                                         // 原 :3302
        return Result;
    }

    /// <summary>原 :3305-3314 `procedure Clear`。</summary>
    public void Clear() => FList.Clear();                          // 原 :3313

    /// <summary>原 :3316-3329 `procedure Delete(ProcessInfo: PTProcessInfo)`（按引用相等，命中即 Break）。</summary>
    public void Delete(TProcessInfo ProcessInfo)
    {
        for (int I = 0; I < FList.Count; I++)                      // 原 :3320
        {
            if (ReferenceEquals(FList[I], ProcessInfo))            // 原 :3322
            {
                FList.RemoveAt(I);                                 // 原 :3325
                return;                                            // 原 :3326 `Break`
            }
        }
    }

    /// <summary>原 :3331-3346 `function Find(const MD5: string): PTProcessInfo`（`SameText` = 大小写不敏感）。</summary>
    public TProcessInfo Find(string MD5)
    {
        for (int I = 0; I < FList.Count; I++)                      // 原 :3337
        {
            var ProcessInfo = FList[I];                            // 原 :3339
            // 原 :3340 `if SameText(ProcessInfo.ProcessMD5, MD5) then`
            //   Delphi `SameText` = `CompareText(...) = 0`（大小写不敏感）。GXX.Core 未提供 SameText，
            //   此处用 OrdinalIgnoreCase 等价实现（调用方语义只涉及十六进制 ASCII）。
            if (SameText(ProcessInfo.ProcessMD5, MD5))              // 原 :3340
                return ProcessInfo;                                // 原 :3342-3343
        }
        return null;                                               // 原 :3336
    }

    /// <summary>Delphi `SysUtils.SameText`：大小写不敏感比较。</summary>
    private static bool SameText(string a, string b)
        => string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>原 :3356-3364 `procedure Lock`。</summary>
    public void Lock() => Monitor.Enter(FCS);

    /// <summary>原 :3366-3374 `procedure UnLock`。</summary>
    public void UnLock() => Monitor.Exit(FCS);
}
