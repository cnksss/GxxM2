// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（Delphi 7，GBK）—— **名单加载/落盘 + 日志 + 进程黑名单重建**
//   实测 LF = 3595 行。本文件覆盖：
//     :1392-1400 AddTempBlockIP          :1402-1410 AddBlockIP
//     :1412-1421 AddTempBlockMac         :1423-1432 AddBlockMac
//     :1434-1451 AddMainLogMsg           :1466-1477 AddIOCPLogMsg
//     :1481-1504 AddToDefFilterSayMsgList（**VERSION_TYPE=2 → 死代码**，见下）
//     :1538-1553 LoadFilterSayMsgFile
//     :1555-1580 ReadFYDenyIPListFile    :1582-1607 ReadFYPassIPListFile
//     :1609-1634 ReadFYDenyMACListFile
//     :1636-1656 LoadBlockIPFile         :1658-1671 SaveBlockIPList
//     :1673-1688 LoadBlockMacFile        :1690-1693 SaveBlockMacList
//     :1695-1748 LoadIPSectionList       :1750-1775 SaveIPSectionList
//     :1777-1803 LoadDBAddressTable
//     :1866-1907 LoadProcessBlacklist    :1909-1954 RebuildProcessBlacklist
//     :1956-1979 SaveProcessBlacklist    :1981-2014 LoadNoVerifyChrList
//     :3376-3387 InitActionIntervalsFileNames
//   （依赖：MD5Util.pas:374-405 `StrToMD5Digest` —— GXX.Core 的 MD5Util 没有它，见 §依赖）
//
// ── 条件编译开关的**生效组合复核**（本切片新增一条，前任 §1 表里没有）─────────────────
//   `Grobal2_Ex.pas:15  VERSION_TYPE = 2;` —— 因此 GateShare.pas 里三处 `{$IF VERSION_TYPE = 1}`
//   **全部为假**，属死代码：`:436-438`（CheckInWhiteList 声明）、`:1479-1536`（其实现 +
//   AddToDefFilterSayMsgList 本体）、`:1549-1551`（LoadFilterSayMsgFile 里的调用）。
//   后果（本切片按**活代码**实现，死分支只保留可复核的形式）：
//     * `LoadFilterSayMsgFile` **不会**注入那 65 条默认全角/西里尔词表；
//     * `CheckInWhiteList`（登录器白名单 + DecryString_LF 解密）不可达 → 不移植，登记在报告里。
//   同一文件里 `{$IF VERSION_TYPE <> 0}` 为真（uFrmMain.pas:364/2413），属主窗体车道。
//   其余开关复核结论与前任 §1 一致：`CLIENT_ANTIPLUG=1`、`NEED_REGISTER=1`、`REGISTER_TEST=0`、
//   `MultiThreadRunContext=1`、`RungateLEG_IOCP=6`、`UseIocpClient=1`；
//   `Common/iocp.inc` 里 `{.$DEFINE USE_SPINLOCK}` **被注释掉** → `USE_SPINLOCK` 未定义，
//   `{$ELSE}` 的 `TRTLCriticalSection` 分支才是活代码（本车道的容器用 Monitor 等价实现）。
//
// ── 依赖说明 ────────────────────────────────────────────────────────────────────────
//   `StrToMD5Digest(S, var MD5): Boolean`（MD5Util.pas:374-405）在 C# 侧**没有对应实现**
//   （`GXX.Core.Crypto.MD5Util` 只有 MD5Buffer/MD5Print/MD5String(→hex 串)）。
//   本文件按 1:1 内联移植这一个函数（其余 MD5 一律用 GXX.Core 版），并在报告里登记为
//   "建议上移 GXX.Core"。`MD5String(m): MD5Digest`（原文返回**原始 16 字节**，而 GXX.Core 版返回 hex 串）
//   的语义差异由 `RebuildProcessBlacklist` 直接对**压缩后的原始字节**做 MD5 来对齐。
//
// ── 原文缺陷登记（照抄语义 + 差异断言）───────────────────────────────────────────────
//   A. [:1873 + :1963] `LoadProcessBlacklist` / `SaveProcessBlacklist` 的**第一行都是
//      `g_DBAddressList.Clear;`** —— 清的是 **DB 地址表**而不是进程黑名单（从 `LoadDBAddressTable` :1783
//      复制粘贴来的）。后果：加载/保存进程黑名单会把 `g_DBAddressList` 清空。照抄并断言。
//   B. [:1609-1620] `ReadFYDenyMACListFile` 的 `g_FYDenyMACList.Clear`（:1617）在
//      `if not FileExists(...) then Exit`（:1620）**之前**；而同族 `ReadFYDenyIPListFile`（:1564-1565）
//      与 `ReadFYPassIPListFile`（:1591-1592）是"先判存在、再 Clear"。三者行为**不一致**：MAC 名单
//      在文件缺失时会被清空，IP 名单不会。照抄并断言。
//   C. [:3382] `InitActionIntervalsFileNames` 的 `if Length(g_sActionIntervalsFileNames) > 0 then`
//      —— `g_sActionIntervalsFileNames` 是**静态数组**，`Length` 恒为 27 → 条件恒真。
//      后果：原本为 `''` 的 7 个槽位也会被拼上 exe 目录，变成**目录路径**而不是空串。照抄并断言。
//   D. [:1643 / :1677 / :1692 / :1713 / :1773 / :1784 / :1874 / :1964] 所有落盘/读盘路径都由
//      `ExtractFilePath(ParamStr(0))` 拼接（相对 exe 目录）。托管侧抽成 `GateSharePaths.ExeDir` 接缝。
//   E. [:1552 / :1655 / :1687 / :1747] 这 4 条 `AddMainLogMsg(..., 4)` 的等级是 **4**，而
//      `g_btShowLogLevel` 默认 **3** → `4 <= 3` 为假 → **一条都不会进日志**。照抄并断言。
//   F. [:1882-1899] `LoadProcessBlacklist` **不清空** `g_ProcessBlackList`（只靠 `Add` 的 MD5 去重）；
//      重复调用不会产生重复项，但也不会移除文件中已删除的项。
//   G. [:1483-1490] `DefStrs: array[0..64]` 共 65 槽、初始值恰好 **65** 个 → 无"多余空串"槽；
//      但表内**有重复**（'Ａ' 出现 2 次、'щ' 2 次、'Ｂ'/'Ｃ'/'Ｄ'/'Ｅ'/'Ｆ' 各 2 次），
//      靠 `IndexOf` 去重（:1498）。表由脚本从 GBK 原文抽取并回读比对（非手工转录）。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.RunGate;

/// <summary>`ExtractFilePath(ParamStr(0))` 的托管接缝 —— 所有"exe 目录相对路径"都走这里。
/// 测试把它指向临时目录即可完全避开真实工作目录（原文硬编码 `D:\MirServer\...` 的项除外，
/// 那些由 `GateShareGlobals`/`FormGlobals` 的字符串全局量承载，测试直接改字符串）。</summary>
public static class GateSharePaths
{
    /// <summary>`ParamStr(0)`（exe 全路径）。</summary>
    public static string ParamStr0 = DefaultParamStr0();

    /// <summary>`ExtractFilePath(ParamStr(0))`：**带尾随目录分隔符**。</summary>
    public static string ExeDir = DefaultExeDir();

    private static string DefaultParamStr0()
    {
        try
        {
            var p = Environment.ProcessPath;
            if (!string.IsNullOrEmpty(p)) return p;
        }
        catch { /* 非托管宿主下可能不可用 */ }
        return Path.Combine(AppContext.BaseDirectory, "RunGate.exe");
    }

    private static string DefaultExeDir()
    {
        string d = Path.GetDirectoryName(ParamStr0) ?? AppContext.BaseDirectory;
        if (d.Length == 0) return d;
        char last = d[^1];
        return last == Path.DirectorySeparatorChar || last == Path.AltDirectorySeparatorChar
            ? d
            : d + Path.DirectorySeparatorChar;
    }

    /// <summary>把 exe 目录换成给定目录（测试用）。传入目录**不必**带尾随分隔符。</summary>
    public static void SetExeDirForTest(string dir)
    {
        if (string.IsNullOrEmpty(dir)) { ParamStr0 = DefaultParamStr0(); ExeDir = DefaultExeDir(); return; }
        char last = dir[^1];
        ExeDir = last == Path.DirectorySeparatorChar || last == Path.AltDirectorySeparatorChar
            ? dir
            : dir + Path.DirectorySeparatorChar;
        ParamStr0 = Path.Combine(dir, "RunGate.exe");
    }

    /// <summary>复位到真实 exe 目录。</summary>
    public static void ResetForTest()
    {
        ParamStr0 = DefaultParamStr0();
        ExeDir = DefaultExeDir();
    }
}

/// <summary>MD5Util.pas 的最小内联移植（只补 C# 侧缺失的那一个函数）。</summary>
public static class GateShareMd5
{
    /// <summary>原文 MD5Util.pas:374-405 `function StrToMD5Digest(S: string; var MD5: MD5Digest): Boolean;`
    /// 长度必须恰为 32 且全为十六进制字符，否则 False 且 **MD5 出参不改动**。</summary>
    public static bool StrToMD5Digest(string S, out byte[] MD5)
    {
        MD5 = new byte[16];
        if (S == null || S.Length != 32) return false;      // 原 :380

        for (int I = 0; I < S.Length; I++)                  // 原 :382
        {
            char c = S[I];                                  // 原 :390 `S[I] in ['0'..'9','A'..'F','a'..'f']`
            bool ok = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
            if (!ok) return false;                          // 原 :387 `Exit`
        }

        // 原 :397-403
        for (int I = 0; I < 16; I++)
        {
            string TempS = S.Substring(I * 2, 2);           // 原 :401 `Copy(S, I*2+1, 2)`
            MD5[I] = (byte)Convert.ToInt32(TempS, 16);      // 原 :402 `StrToInt('$' + TempS)`
        }
        return true;                                        // 原 :397
    }

    /// <summary>原文 MD5Util.pas:285-292 `MD5String(m): MD5Digest` —— 对字符串**字节**求原始 16 字节摘要。
    /// 托管侧按 GBK 取字节（原文 `PChar(m)` + `Length(m)` 即 AnsiString 的字节缓冲）。</summary>
    public static byte[] MD5StringDigest(string m)
    {
        byte[] data = GXX.Core.EncodingInit.GBK.GetBytes(m ?? "");
        return MD5Bytes(data);
    }

    /// <summary>对**原始字节**求 MD5（等价于原文 `MD5String(二进制串)`）。</summary>
    public static byte[] MD5Bytes(byte[] data)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(data ?? Array.Empty<byte>());
    }

    /// <summary>原文 :334-344 `MD5Print`（小写十六进制）。</summary>
    public static string MD5Print(byte[] d) => BitConverter.ToString(d).Replace("-", "").ToLowerInvariant();
}

/// <summary>GateShare.pas 的名单加载/落盘 + 日志族（1:1）。</summary>
public static class GateShareLists
{
    // ==================================================================================
    // 原文 :1481-1504 AddToDefFilterSayMsgList 的 65 条默认词表
    //   脚本抽取 + 回读比对（非手工转录）：
    //     $l=[IO.File]::ReadAllLines($normalized); $block=$l[1482..1489] -join "`n";
    //     [regex]::Matches($block,"'([^']*)'") | %{ $_.Groups[1].Value }   → count = 65（恰好填满 0..64）
    //   回读校验：见 GateShareListTests.DefStrs_Has65EntriesMatchingSourceExtraction
    // ==================================================================================
    private static readonly string[] DefStrs =
    {
        "М", "ω", "С", "Ａ", "Ｂ", "Ｃ", "ш", "щ", "щ", "Ａ", "Ｄ", "Ｅ", "Ｆ",
        "ｂ", "ｃ", "ｄ", "ｅ", "ｆ", "ｇ", "ｈ", "ｉ", "ｊ", "ｋ", "ｌ", "ｍ", "ｎ",
        "ｏ", "ｐ", "ｑ", "ｒ", "ｓ", "ｔ", "ｕ", "ｖ", "ｗ", "ｘ", "ｙ", "ｚ", "Ｂ",
        "Ｃ", "Ｄ", "Ｅ", "Ｆ", "Ｇ", "Ｈ", "Ｉ", "Ｊ", "Ｋ", "Ｌ", "Ｍ", "Ｎ", "Ｏ",
        "Ｐ", "Ｑ", "Ｒ", "Ｓ", "Ｔ", "Ｕ", "Ｖ", "Ｗ", "Ｘ", "Ｙ", "Ｚ", "c", "w",
    };

    /// <summary>原文 `DefStrs` 表的只读视图（供测试做抽取/回读比对）。</summary>
    public static IReadOnlyList<string> DefStrsForTest => DefStrs;

    /// <summary>原文 :1481-1504 `procedure AddToDefFilterSayMsgList;`
    /// ★ **死代码**：调用点 `:1550` 在 `{$IF VERSION_TYPE = 1}` 内，而 `VERSION_TYPE = 2`。
    /// 仍按 1:1 保留，以便开关切回 1 时可复核；`LoadFilterSayMsgFile` **不调用它**（见 :1549-1551）。</summary>
    public static void AddToDefFilterSayMsgList()
    {
        FormGlobals.g_WordFilterList.Lock();                                   // 原 :1494
        try
        {
            for (int I = 0; I < DefStrs.Length; I++)                           // 原 :1496 `Low..High`
            {
                if (FormGlobals.g_WordFilterList.IndexOf(DefStrs[I]) == -1)    // 原 :1498
                    FormGlobals.g_WordFilterList.Add(DefStrs[I]);              // 原 :1499
            }
        }
        finally
        {
            FormGlobals.g_WordFilterList.UnLock();                             // 原 :1502
        }
    }

    /// <summary>原文 :1538-1553 `procedure LoadFilterSayMsgFile();`
    /// ★ 活路径**不含**默认词表注入（:1549-1551 是死分支）；末尾日志等级 4 会被默认门限 3 滤掉（缺陷 E）。</summary>
    public static void LoadFilterSayMsgFile()
    {
        // 原 :1540 `if FileExists(g_sWordFilterFileName) then`
        if (File.Exists(FormGlobals.g_sWordFilterFileName))
        {
            FormGlobals.g_WordFilterList.Lock();                               // 原 :1542
            try
            {
                FormGlobals.g_WordFilterList.LoadFromFile(FormGlobals.g_sWordFilterFileName);   // 原 :1544
            }
            finally
            {
                FormGlobals.g_WordFilterList.UnLock();                          // 原 :1546
            }
        }

        // 原 :1549-1551：
        //   {$IF VERSION_TYPE = 1}
        //     AddToDefFilterSayMsgList;
        //   {$IFEND}
        //   VERSION_TYPE = 2（Grobal2_Ex.pas:15）→ 该分支不编译。此处显式保留"不调用"的事实，
        //   并由 LoadFilterSayMsgFile_DoesNotInjectDefaultWords_VersionTypeIs2 断言。

        AddMainLogMsg("加载文字过滤信息完成", 4);                                // 原 :1552
    }

    // ==================================================================================
    // 防御名单（FY）读取
    // ==================================================================================

    /// <summary>原文 :1555-1580 `function ReadFYDenyIPListFile(): Boolean;`
    /// 文件不存在 → **False 且不清空**既有名单。</summary>
    public static bool ReadFYDenyIPListFile()
    {
        bool Result = false;                                        // 原 :1562
        string sFileName = FormGlobals.g_sFYReadDenyIPFile;         // 原 :1563
        if (!File.Exists(sFileName)) return false;                  // 原 :1564 `Exit`
        GateShareGlobals.g_FYDenyIPList.Clear();                    // 原 :1565

        var LoadList = new TStringList();
        try
        {
            LoadList.LoadFromFile(sFileName);                       // 原 :1569
            for (int I = 0; I < LoadList.Count; I++)                // 原 :1570
            {
                string sIPaddr = DelphiRTL.Trim(LoadList[I]);       // 原 :1572
                if (!HUtil32.IsIPaddr(sIPaddr)) continue;           // 原 :1573 `if not IsIpaddr then Continue`
                GateShareGlobals.g_FYDenyIPList.Add(sIPaddr);       // 原 :1574
            }
            Result = true;                                          // 原 :1576
        }
        finally
        {
            // 原 :1577-1578 `finally LoadList.Free;`
        }
        return Result;
    }

    /// <summary>原文 :1582-1607 `function ReadFYPassIPListFile(): Boolean;`（与 Deny 版同构）。</summary>
    public static bool ReadFYPassIPListFile()
    {
        bool Result = false;                                        // 原 :1589
        string sFileName = FormGlobals.g_sFYReadPassIPFile;         // 原 :1590
        if (!File.Exists(sFileName)) return false;                  // 原 :1591
        GateShareGlobals.g_FYPassIPList.Clear();                    // 原 :1592

        var LoadList = new TStringList();
        LoadList.LoadFromFile(sFileName);                           // 原 :1596
        for (int I = 0; I < LoadList.Count; I++)                    // 原 :1597
        {
            string sIPaddr = DelphiRTL.Trim(LoadList[I]);           // 原 :1599
            if (!HUtil32.IsIPaddr(sIPaddr)) continue;               // 原 :1600
            GateShareGlobals.g_FYPassIPList.Add(sIPaddr);           // 原 :1601
        }
        return true;                                                // 原 :1603
    }

    /// <summary>原文 :1609-1634 `function ReadFYDenyMACListFile(): Boolean;`
    /// ★ 缺陷 B：`Clear`（:1617）在 `FileExists` 判据（:1620）**之前** → 文件缺失也清空；
    /// 且**没有** `IsIpaddr` 过滤（这是 MAC 名单）。</summary>
    public static bool ReadFYDenyMACListFile()
    {
        bool Result = false;                                        // 原 :1616
        GateShareGlobals.g_FYDenyMACList.Clear();                   // 原 :1617（★ 在存在性判据之前）

        string sFileName = FormGlobals.g_sFYReadDenyMACFile;        // 原 :1619
        if (!File.Exists(sFileName)) return false;                  // 原 :1620 `Exit`（此时已清空）

        var LoadList = new TStringList();
        LoadList.LoadFromFile(sFileName);                           // 原 :1624
        for (int I = 0; I < LoadList.Count; I++)                    // 原 :1625
        {
            string sIPaddr = DelphiRTL.Trim(LoadList[I]);           // 原 :1627
            GateShareGlobals.g_FYDenyMACList.Add(sIPaddr);          // 原 :1628（无 IsIpaddr 过滤）
        }
        return true;                                                // 原 :1630
    }

    // ==================================================================================
    // 禁止名单（BlockIP / BlockMac）读写
    // ==================================================================================

    /// <summary>原文 :1636-1656 `procedure LoadBlockIPFile();`
    /// ★ 无 `IsIpaddr` 过滤（与 ReadFYDenyIPListFile 不同），且 `Add` 的 INADDR_NONE 守卫是死代码
    /// （见 GateShareContainers.cs 缺陷 1）→ 非法行会以 `nIPaddr = -1` 入表。
    /// ★ 原文**不先 Clear**（重复调用靠 Add 的去重）。</summary>
    public static void LoadBlockIPFile()
    {
        string sFileName = GateSharePaths.ExeDir + "BlockIPList.txt";       // 原 :1643
        if (File.Exists(sFileName))                                        // 原 :1644
        {
            var LoadList = new TStringList();                              // 原 :1646
            LoadList.LoadFromFile(sFileName);                              // 原 :1647
            for (int I = 0; I < LoadList.Count; I++)                       // 原 :1648
            {
                string sIPaddr = DelphiRTL.Trim(LoadList[I]);              // 原 :1650
                GateShareGlobals.g_BlockIPList.Add(sIPaddr);               // 原 :1651
            }
        }
        AddMainLogMsg("加载IP过滤列表完成", 4);                               // 原 :1655
    }

    /// <summary>原文 :1658-1671 `procedure SaveBlockIPList();`
    /// 元素 → `StrPas(inet_ntoa(TInAddr(nIPaddr)))`（**这里不做 ReverseBytes**，与 `Long2IP` 不同）。</summary>
    public static void SaveBlockIPList()
    {
        var SaveList = new TStringList();                                  // 原 :1663
        for (int I = 0; I < GateShareGlobals.g_BlockIPList.Count; I++)     // 原 :1664
        {
            // 原 :1666 `SaveList.Add(StrPas(inet_ntoa(TInAddr(g_BlockIPList.Items[I].nIPaddr))));`
            SaveList.Add(GateShareInet.InetNtoa(unchecked((uint)GateShareGlobals.g_BlockIPList[I].nIPaddr)));
        }
        SaveList.SaveToFile(GateSharePaths.ExeDir + "BlockIPList.txt");    // 原 :1669
    }

    /// <summary>原文 :1673-1688 `procedure LoadBlockMacFile();`</summary>
    public static void LoadBlockMacFile()
    {
        string sFileName = GateSharePaths.ExeDir + "BlockMacList.txt";      // 原 :1677
        if (File.Exists(sFileName))                                        // 原 :1678
        {
            GateShareGlobals.g_BlockMacList.Lock();                        // 原 :1680
            try
            {
                GateShareGlobals.g_BlockMacList.LoadFromFile(sFileName);   // 原 :1682
            }
            finally
            {
                GateShareGlobals.g_BlockMacList.UnLock();                  // 原 :1684
            }
        }
        AddMainLogMsg("加载Mac过滤列表完成", 4);                              // 原 :1687
    }

    /// <summary>原文 :1690-1693 `procedure SaveBlockMacList();`
    /// ★ 原文**没有** Lock/UnLock（同族 SaveBlockIPList 也没有）—— 照抄。</summary>
    public static void SaveBlockMacList()
    {
        GateShareGlobals.g_BlockMacList.SaveToFile(GateSharePaths.ExeDir + "BlockMacList.txt");   // 原 :1692
    }

    // ==================================================================================
    // IP 段过滤列表（IPSectionList）：字段分隔符是 **#8（TAB）**
    // ==================================================================================

    /// <summary>原文 :1695-1748 `procedure LoadIPSectionList();`
    /// 行格式：`IP1 #8 IP2`。`Pos(#8, S) &lt;= 1` 的行跳过（无 TAB 或 TAB 在首列）。
    /// 守卫 `nBeginaddr &lt;&gt; INADDR_NONE` 在 **LongWord 域**比较（与 TAddressList 的死守卫不同）→ 有效。</summary>
    public static void LoadIPSectionList()
    {
        GateShareGlobals.LockIPSectionList();                                   // 原 :1704
        try
        {
            GateShareGlobals.ClearIPSectionList();                              // 原 :1708
        }
        finally
        {
            GateShareGlobals.UnLockIPSectionList();                             // 原 :1710
        }

        string sFileName = GateSharePaths.ExeDir + "IPSectionList.txt";         // 原 :1713
        if (File.Exists(sFileName))                                            // 原 :1714
        {
            var LoadList = new TStringList();                                  // 原 :1716
            LoadList.LoadFromFile(sFileName);                                  // 原 :1717
            for (int I = 0; I < LoadList.Count; I++)                           // 原 :1718
            {
                string S = DelphiRTL.Trim(LoadList[I]);                        // 原 :1720
                if (S.Length == 0) continue;                                   // 原 :1721

                int Index = S.IndexOf('\u0008') + 1;                           // 原 :1723 `Pos(#8, S)`（1-based，0 = 未找到）
                if (Index <= 1) continue;                                      // 原 :1724

                string IP1 = S.Substring(0, Index - 1);                        // 原 :1726 `Copy(S, 1, Index-1)`
                string IP2 = S.Substring(Index);                               // 原 :1727 `Copy(S, Index+1, MaxInt)`

                uint nBeginaddr = GateShareInet.IP2Long(IP1);                  // 原 :1729
                uint nEndaddr = GateShareInet.IP2Long(IP2);                    // 原 :1730

                GateShareGlobals.LockIPSectionList();                          // 原 :1732
                try
                {
                    // 原 :1734（LongWord 域比较，这里 INADDR_NONE 的守卫是**有效**的）
                    if ((nBeginaddr != GateShareInet.INADDR_NONE)
                        && (nEndaddr != GateShareInet.INADDR_NONE)
                        && (nBeginaddr <= nEndaddr))
                    {
                        GateShareGlobals.AddIPSection(new TIPSection          // 原 :1736-1739
                        {
                            nBeginAddr = nBeginaddr,
                            nEndAddr = nEndaddr,
                        });
                    }
                }
                finally
                {
                    GateShareGlobals.UnLockIPSectionList();                    // 原 :1742
                }
            }
        }
        AddMainLogMsg("加载IP段过滤列表完成", 4);                                 // 原 :1747
    }

    /// <summary>原文 :1750-1775 `procedure SaveIPSectionList();`</summary>
    public static void SaveIPSectionList()
    {
        var SaveList = new TStringList();                                      // 原 :1757

        GateShareGlobals.LockIPSectionList();                                  // 原 :1759
        try
        {
            for (int I = 0; I < GateShareGlobals.IPSectionListCount; I++)      // 原 :1761
            {
                var IPSection = GateShareGlobals.GetIPSection(I);              // 原 :1763
                string IP1 = GateShareInet.Long2IP(IPSection.nBeginAddr);      // 原 :1764
                string IP2 = GateShareInet.Long2IP(IPSection.nEndAddr);        // 原 :1765
                SaveList.Add(IP1 + "\u0008" + IP2);                            // 原 :1767
            }
        }
        finally
        {
            GateShareGlobals.UnLockIPSectionList();                            // 原 :1770
        }

        SaveList.SaveToFile(GateSharePaths.ExeDir + "IPSectionList.txt");      // 原 :1773
    }

    // ==================================================================================
    // !addrtable.txt（DB 地址表）
    // ==================================================================================

    /// <summary>原文 :1777-1803 `procedure LoadDBAddressTable();`
    /// 空行跳过、`;` 开头的行跳过、去重后追加。**先 Clear**。</summary>
    public static void LoadDBAddressTable()
    {
        GateShareGlobals.g_DBAddressList.Clear();                              // 原 :1783
        string FileName = GateSharePaths.ExeDir + "!addrtable.txt";            // 原 :1784
        if (File.Exists(FileName))                                             // 原 :1785
        {
            var LoadList = new TStringList();                                  // 原 :1787
            LoadList.LoadFromFile(FileName);                                   // 原 :1789
            for (int I = 0; I < LoadList.Count; I++)                           // 原 :1790
            {
                string sLineText = DelphiRTL.Trim(LoadList[I]);                // 原 :1792
                // 原 :1793 `if (sLineText <> '') and (sLineText[1] <> ';') then`
                if (sLineText.Length != 0 && sLineText[0] != ';')
                {
                    if (GateShareGlobals.g_DBAddressList.IndexOf(sLineText) < 0)   // 原 :1795
                        GateShareGlobals.g_DBAddressList.Add(sLineText);           // 原 :1796
                }
            }
        }
    }

    // ==================================================================================
    // 进程黑名单
    // ==================================================================================

    /// <summary>原文 :1866-1907 `procedure LoadProcessBlacklist();`
    /// ★ 缺陷 A：第一行清的是 `g_DBAddressList`（不是进程黑名单）。
    /// ★ 缺陷 F：不清空 `g_ProcessBlackList`，只靠 `Add` 去重。</summary>
    public static void LoadProcessBlacklist()
    {
        GateShareGlobals.g_DBAddressList.Clear();                              // 原 :1873（★ 缺陷 A）
        string FileName = GateSharePaths.ExeDir + "ProcessBlacklist.txt";      // 原 :1874
        if (File.Exists(FileName))                                             // 原 :1875
        {
            FormGlobals.g_ProcessBlackList.Lock();                             // 原 :1877

            var LoadList = new TStringList();                                  // 原 :1879
            try
            {
                LoadList.LoadFromFile(FileName);                               // 原 :1881
                for (int I = 0; I < LoadList.Count; I++)                       // 原 :1882
                {
                    string sLineText = DelphiRTL.Trim(LoadList[I]);            // 原 :1884
                    if (sLineText.Length != 0 && sLineText[0] != ';')          // 原 :1885
                    {
                        int Index = sLineText.IndexOf('|') + 1;                // 原 :1887 `Pos('|', sLineText)`
                        if (Index > 0)                                         // 原 :1888
                        {
                            string ProcessName = sLineText.Substring(0, Index - 1);   // 原 :1890
                            string ProcessMD5 = sLineText.Substring(Index);           // 原 :1891

                            // 原 :1893
                            if (ProcessMD5.Length == 32 && GateShareInet.IsHexString(ProcessMD5))
                            {
                                FormGlobals.g_ProcessBlackList.Add(ProcessName, ProcessMD5);   // 原 :1895
                            }
                        }
                    }
                }

                RebuildProcessBlacklist();                                     // 原 :1901
            }
            finally
            {
                FormGlobals.g_ProcessBlackList.UnLock();                       // 原 :1904
            }
        }
    }

    /// <summary>原文 :1956-1979 `procedure SaveProcessBlacklist();`
    /// ★ 缺陷 A：第一行同样清 `g_DBAddressList`。落盘格式 `ProcessName|ProcessMD5`。</summary>
    public static void SaveProcessBlacklist()
    {
        GateShareGlobals.g_DBAddressList.Clear();                              // 原 :1963（★ 缺陷 A）
        string FileName = GateSharePaths.ExeDir + "ProcessBlacklist.txt";      // 原 :1964
        FormGlobals.g_ProcessBlackList.Lock();                                 // 原 :1965
        var SaveList = new TStringList();                                      // 原 :1966
        try
        {
            for (int I = 0; I < FormGlobals.g_ProcessBlackList.Count; I++)     // 原 :1968
            {
                var ProcessInfo = FormGlobals.g_ProcessBlackList[I];           // 原 :1970
                SaveList.Add(ProcessInfo.ProcessName + "|" + ProcessInfo.ProcessMD5);   // 原 :1971
            }
            SaveList.SaveToFile(FileName);                                     // 原 :1974
        }
        finally
        {
            FormGlobals.g_ProcessBlackList.UnLock();                           // 原 :1977
        }
    }

    /// <summary>原文 :1909-1954 `procedure RebuildProcessBlacklist;` 的 RSA 步骤接缝。
    /// 原文用 `LbRSA.pas` 的 `TLbRSA`（`aks128` + 硬编码 128 位公钥 `597A…`/`CF2C…`）加密
    /// `Count × 16` 字节的 MD5 串联。**本仓库未移植 LbRSA**（属许可/暗桩件，DoD §2.3 不移植项），
    /// 故保留可注入接缝；默认实现为**恒等 + 记录**（不伪造加密结果）。</summary>
    public interface IProcessBlacklistCipher
    {
        /// <summary>`RSA.EncryptBuffer(Buf[0], Count * SizeOf(MD5), OutBuf[0])` → 返回加密后字节与长度。</summary>
        byte[] EncryptBuffer(byte[] buffer, int length);
    }

    private sealed class IdentityProcessBlacklistCipher : IProcessBlacklistCipher
    {
        public int CallCount;
        public int LastLength;
        public byte[] EncryptBuffer(byte[] buffer, int length)
        {
            CallCount++;
            LastLength = length;
            var outp = new byte[length];
            Array.Copy(buffer, outp, length);
            return outp;
        }
    }

    private static readonly IdentityProcessBlacklistCipher DefaultCipher = new IdentityProcessBlacklistCipher();

    /// <summary>可注入的 RSA 步骤（默认恒等实现；生产接线由集成者替换）。</summary>
    public static IProcessBlacklistCipher ProcessBlacklistCipher = DefaultCipher;

    /// <summary>默认恒等实现被调用的次数（测试探针；生产无意义）。</summary>
    public static int DefaultCipherCallCount => DefaultCipher.CallCount;

    /// <summary>默认恒等实现最后一次收到的长度（测试探针）。</summary>
    public static int DefaultCipherLastLength => DefaultCipher.LastLength;

    /// <summary>`RebuildProcessBlacklist` 的 zLib 压缩接缝（默认走 `GXX.Core.Protocol.EDcode.zLibCompressBuffer`）。</summary>
    public static Func<byte[], int, byte[]> ZlibCompressBuffer = (src, len) => EDcode.zLibCompressBuffer(src, len);

    /// <summary>原文 :1943 的硬编码 RSA 参数（登记备查，供集成者接线 LbRSA 时使用）。</summary>
    public const string ProcessBlacklistRsaKeySize = "aks128";
    public const string ProcessBlacklistRsaModulus = "597A185BA5F22A014F50B453E647C0C5";
    public const string ProcessBlacklistRsaExponent = "CF2C34C6204626E70E493F5B37930363";

    /// <summary>原文 :1909-1954 `procedure RebuildProcessBlacklist;`
    /// 把 `g_ProcessBlackList` 里前 80 个**合法 MD5**（`StrToMD5Digest` 成功者）串成 16×N 字节 →
    /// RSA → zLib 压缩 → 存 `g_ProcessBlacklistStr` + MD5 摘要。
    /// Count = 0 时两者都清空（`:1946-1950`）。
    ///
    /// ★ 托管侧表示差异：原文 `g_ProcessBlacklistStr` 是**二进制字符串**（zLib 输出装进 string）。
    ///   本实现的权威表示是 `ProcessBlacklistBytes`（byte[]），同时把 `FormGlobals.g_ProcessBlacklistStr`
    ///   按 Latin-1（逐字节保真）同步，以免破坏既有窗体族对该字段的引用。
    ///   `g_ProcessBlacklistMD5` 对**压缩后的原始字节**求 MD5（等价于原文 `MD5String(二进制串)`）。</summary>
    public static void RebuildProcessBlacklist()
    {
        // 原 :1915 `Buf: array[0..2048] of Byte;`（16 × 80 = 1280 ≤ 2048）
        var Buf = new byte[2048];
        int Count = 0;                                                     // 原 :1922

        for (int I = 0; I < FormGlobals.g_ProcessBlackList.Count; I++)     // 原 :1923
        {
            var ProcessInfo = FormGlobals.g_ProcessBlackList[I];           // 原 :1925

            if (GateShareMd5.StrToMD5Digest(ProcessInfo.ProcessMD5, out byte[] MD5))   // 原 :1927
            {
                Array.Copy(MD5, 0, Buf, Count * 16, 16);                   // 原 :1929 `Move(MD5[0], P^, SizeOf(MD5))`
                Count++;                                                   // 原 :1931

                if (Count >= 80) break;                                    // 原 :1933
            }
        }

        if (Count > 0)                                                     // 原 :1937
        {
            // 原 :1939-1941 `RSA.KeySize := aks128; ModulusAsString := '597A…'; ExponentAsString := 'CF2C…';`
            byte[] enc = ProcessBlacklistCipher.EncryptBuffer(Buf, Count * 16);   // 原 :1943

            // 原 :1943 `g_ProcessBlacklistStr := zLibCompressBuffer(PChar(@OutBuf[0]), OutBufSize);`
            ProcessBlacklistBytes = ZlibCompressBuffer(enc, enc.Length);
            FormGlobals.g_ProcessBlacklistStr = BytesToBinaryString(ProcessBlacklistBytes);

            // 原 :1944 `g_ProcessBlacklistMd5 := MD5String(g_ProcessBlacklistStr);`
            //   ★ 原文的 `g_ProcessBlacklistStr` 是**二进制** AnsiString，`MD5String` 对它的**字节缓冲**求摘要；
            //     托管侧的等价物就是对压缩后的**原始字节**求 MD5（不能对 Latin-1 字符串再做 GBK 编码，
            //     那会把 0x80-0xFF 变成多字节 → 摘要不一致）。
            FormGlobals.g_ProcessBlacklistMD5 = GateShareMd5.MD5Bytes(ProcessBlacklistBytes);
        }
        else
        {
            ProcessBlacklistBytes = Array.Empty<byte>();                   // 原 :1948
            FormGlobals.g_ProcessBlacklistStr = "";
            FormGlobals.g_ProcessBlacklistMD5 = new byte[16];              // 原 :1949 `FillChar(..., 0)`
        }
    }

    /// <summary>`RebuildProcessBlacklist` 产出的压缩字节（托管侧权威表示）。</summary>
    public static byte[] ProcessBlacklistBytes { get; private set; } = Array.Empty<byte>();

    /// <summary>byte[] → 逐字节保真的字符串（等价于 Delphi 把二进制装进 AnsiString）。</summary>
    private static string BytesToBinaryString(byte[] b)
    {
        var chars = new char[b.Length];
        for (int i = 0; i < b.Length; i++) chars[i] = (char)b[i];
        return new string(chars);
    }

    // ==================================================================================
    // 免验证角色名单
    // ==================================================================================

    /// <summary>原文 :1981-2014 `function LoadNoVerifyChrList: Boolean;`
    /// **先清空**（在路径判据之前）；文件不存在或路径为空 → False 但已清空。
    /// `IndexOf` 是大小写不敏感（TStringList 默认）→ 去重也大小写不敏感。</summary>
    public static bool LoadNoVerifyChrList()
    {
        bool Result = false;                                                          // 原 :1986

        GateShareGlobals.g_LoadNoVerifyChrList.Lock();                                // 原 :1988
        try
        {
            GateShareGlobals.g_LoadNoVerifyChrList.Clear();                           // 原 :1990

            // 原 :1992
            if (GateShareGlobals.g_sLoadNoVerifyChrListFile.Length > 0
                && File.Exists(GateShareGlobals.g_sLoadNoVerifyChrListFile))
            {
                var SL = new TStringList();                                           // 原 :1994
                SL.LoadFromFile(GateShareGlobals.g_sLoadNoVerifyChrListFile);          // 原 :1996

                for (int I = 0; I < SL.Count; I++)                                    // 原 :1998
                {
                    if (GateShareGlobals.g_LoadNoVerifyChrList.IndexOf(SL[I]) < 0)     // 原 :2000
                        GateShareGlobals.g_LoadNoVerifyChrList.Add(SL[I]);            // 原 :2002
                }
                Result = true;                                                        // 原 :2006
            }
        }
        finally
        {
            GateShareGlobals.g_LoadNoVerifyChrList.UnLock();                          // 原 :2012
        }
        return Result;
    }

    // ==================================================================================
    // 按模式的间隔文件名（初始化用）
    // ==================================================================================

    /// <summary>原文 :3376-3387 `procedure InitActionIntervalsFileNames;`
    /// ★ 缺陷 C：`if Length(g_sActionIntervalsFileNames) > 0` 判的是**数组长度（恒 27）**，
    /// 不是元素字符串 → 7 个空串槽位也会被拼成 exe 目录路径。</summary>
    public static void InitActionIntervalsFileNames()
    {
        for (int ActionMode = 0; ActionMode < FormGlobals.g_sActionIntervalsFileNames.Length; ActionMode++)   // 原 :3380
        {
            // 原 :3382 `if Length(g_sActionIntervalsFileNames) > 0 then` —— 静态数组，Length 恒为 27
            if (FormGlobals.g_sActionIntervalsFileNames.Length > 0)
            {
                FormGlobals.g_sActionIntervalsFileNames[ActionMode] =
                    GateSharePaths.ExeDir + FormGlobals.g_sActionIntervalsFileNames[ActionMode];   // 原 :3384
            }
        }
    }

    // ==================================================================================
    // 日志与名单增补
    // ==================================================================================

    /// <summary>`Now` / `TimeToStr(Now)` 的接缝（测试注入固定时间）。</summary>
    public static Func<DateTime> NowProvider = () => DateTime.Now;

    /// <summary>原文 :1434-1451 `procedure AddMainLogMsg(Msg: string; nLevel: Integer; AddTime: Boolean = True);`
    /// 门限 `nLevel &lt;= g_btShowLogLevel`；`AddTime` 为真时前缀 `[HH:MM:SS] `。
    /// ★ Lock 在 try **之外**（若 Lock 抛异常则不会 UnLock）—— 照抄结构。</summary>
    public static void AddMainLogMsg(string Msg, int nLevel, bool AddTime = true)
    {
        GateShareGlobals.g_MainLogStrings.Lock();                        // 原 :1439
        try
        {
            if (nLevel <= GateShareGlobals.g_btShowLogLevel)             // 原 :1440
            {
                string sMsg;
                if (AddTime)
                    sMsg = "[" + NowProvider().ToString("HH:mm:ss") + "] " + Msg;   // 原 :1443 `'[' + TimeToStr(Now) + '] ' + Msg`
                else
                    sMsg = Msg;                                          // 原 :1445
                GateShareGlobals.g_MainLogStrings.Add(sMsg);             // 原 :1446
            }
        }
        finally
        {
            GateShareGlobals.g_MainLogStrings.UnLock();                  // 原 :1449
        }
    }

    /// <summary>原文 :1466-1477 `procedure AddIOCPLogMsg(Msg: string);`（**总是**带时间前缀，无等级门限）。</summary>
    public static void AddIOCPLogMsg(string Msg)
    {
        GateShareGlobals.g_IOCPLogStrings.Lock();                        // 原 :1470
        try
        {
            string sMsg = "[" + NowProvider().ToString("HH:mm:ss") + "] " + Msg;   // 原 :1472
            GateShareGlobals.g_IOCPLogStrings.Add(sMsg);                 // 原 :1473
        }
        finally
        {
            GateShareGlobals.g_IOCPLogStrings.UnLock();                  // 原 :1475
        }
    }

    /// <summary>原文 :1392-1400 `procedure AddTempBlockIP(sIPaddr: string);`（**不去重**，靠 TAddressList.Add 去重）。</summary>
    public static void AddTempBlockIP(string sIPaddr)
    {
        GateShareGlobals.g_TempIPList.Lock();                            // 原 :1394
        try
        {
            GateShareGlobals.g_TempIPList.Add(sIPaddr);                  // 原 :1396
        }
        finally
        {
            GateShareGlobals.g_TempIPList.UnLock();                      // 原 :1398
        }
    }

    /// <summary>原文 :1402-1410 `procedure AddBlockIP(sIPaddr: string);`</summary>
    public static void AddBlockIP(string sIPaddr)
    {
        GateShareGlobals.g_BlockIPList.Lock();                           // 原 :1404
        try
        {
            // 原 :1406（原文此处多一个前导空格，属排版）
            GateShareGlobals.g_BlockIPList.Add(sIPaddr);
        }
        finally
        {
            GateShareGlobals.g_BlockIPList.UnLock();                     // 原 :1408
        }
    }

    /// <summary>原文 :1412-1421 `procedure AddTempBlockMac(sMac: string);`（**显式 IndexOf 去重**）。</summary>
    public static void AddTempBlockMac(string sMac)
    {
        GateShareGlobals.g_TempMacList.Lock();                           // 原 :1414
        try
        {
            if (GateShareGlobals.g_TempMacList.IndexOf(sMac) < 0)        // 原 :1416
                GateShareGlobals.g_TempMacList.Add(sMac);                // 原 :1417
        }
        finally
        {
            GateShareGlobals.g_TempMacList.UnLock();                     // 原 :1419
        }
    }

    /// <summary>原文 :1423-1432 `procedure AddBlockMac(sMac: string);`</summary>
    public static void AddBlockMac(string sMac)
    {
        GateShareGlobals.g_BlockMacList.Lock();                          // 原 :1425
        try
        {
            if (GateShareGlobals.g_BlockMacList.IndexOf(sMac) < 0)       // 原 :1427
                GateShareGlobals.g_BlockMacList.Add(sMac);               // 原 :1428
        }
        finally
        {
            GateShareGlobals.g_BlockMacList.UnLock();                    // 原 :1430
        }
    }
}
