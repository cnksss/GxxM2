// 源单元：Source/Client-HGE/CheckProcessModules.pas（原文 362 行 / CRLF 计入 445 行）
// 原文 uses：Windows, Messages, SysUtils, Classes, Registry, tlHelp32, Dialogs, ShlObj,
//           HashList, MD5Util, CheckCrc, FastStrings, EncryptUnit（implementation 段另用 ZLibEx）
// 原文无同名 .dfm。
//
// 覆盖事实：CheckProcessModules **不在 Client.dpr 的 uses 列表中**
//   （Client.dpr:72 有 `CheckProcessModules in 'CheckProcessModules.pas'`），
//   源码树里 `CheckProcessModule` / `GetSpecialFolderDir` 也没有调用点。
//   它是一份"引而不入"的反外挂模块（对照模块名白名单/黑名单）。
//
// 移植策略：
//   · 纯逻辑 1:1 移植（可单测）：CompareLStr、ReadRegKey/WriteRegKey 的模式分发、
//     明文版权表 + DecryString 解密、CheckProcessModule 的**判定顺序**；
//   · Win32 → 托管映射：
//       HashList                 → Dictionary<string, object>
//       RivestFile（文件 MD5）    → MD5.HashData（GXX.Core.Crypto.MD5Util）
//       GetFileLegalCopyright    → System.Diagnostics.FileVersionInfo.LegalCopyright（**语义等价**）
//       FastPosNoCase            → 忽略大小写的子串查找（原文是 Boyer-Moore 加速版，结果相同）
//       GetSpecialFolderDir      → Environment.GetFolderPath（CSIDL 映射）
//       TRegistry                → Microsoft.Win32.Registry（托管侧用 RegistryKey）
//   · 崩溃风险点：原文 `GetFileVersionInfoSize(PChar(sFileName), InfoSize)` 把 **InfoSize 当出参**，
//     函数返回的"真实大小"被丢弃、InfoSize 被写成 GetLastError（0）⇒ InfoSize 恒为 0，
//     随后 `AllocMem(0)` 得到零长缓冲、`GetFileVersionInfo` 必然失败。
//     这是原文缺陷，见报告 §5；本移植用 FileVersionInfo 得到**原文想要的结果**，并登记差异。
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace GXX.Client.Tail;

/// <summary>原文 :26-153 的 <c>_CSIDL_*</c> 常量（1:1 保留）。</summary>
public static class CheckProcessModulesConst
{
    /// <summary>原文 :28 — <c>_CSIDL_DESKTOP = $0000;</c></summary>
    public const int _CSIDL_DESKTOP = 0x0000;
    /// <summary>原文 :30 — <c>_CSIDL_INTERNET = $0001;</c></summary>
    public const int _CSIDL_INTERNET = 0x0001;
    /// <summary>原文 :32 — <c>_CSIDL_PROGRAMS = $0002;</c></summary>
    public const int _CSIDL_PROGRAMS = 0x0002;
    /// <summary>原文 :34 — <c>_CSIDL_CONTROLS = $0003;</c></summary>
    public const int _CSIDL_CONTROLS = 0x0003;
    /// <summary>原文 :36 — <c>_CSIDL_PRINTERS = $0004;</c></summary>
    public const int _CSIDL_PRINTERS = 0x0004;
    /// <summary>原文 :38 — <c>_CSIDL_PERSONAL = $0005;</c></summary>
    public const int _CSIDL_PERSONAL = 0x0005;
    /// <summary>原文 :40 — <c>_CSIDL_FAVORITES = $0006;</c></summary>
    public const int _CSIDL_FAVORITES = 0x0006;
    /// <summary>原文 :42 — <c>_CSIDL_STARTUP = $0007;</c></summary>
    public const int _CSIDL_STARTUP = 0x0007;
    /// <summary>原文 :44 — <c>_CSIDL_RECENT = $0008;</c></summary>
    public const int _CSIDL_RECENT = 0x0008;
    /// <summary>原文 :46 — <c>_CSIDL_SENDTO = $0009;</c></summary>
    public const int _CSIDL_SENDTO = 0x0009;
    /// <summary>原文 :48 — <c>_CSIDL_BITBUCKET = $000A;</c></summary>
    public const int _CSIDL_BITBUCKET = 0x000A;
    /// <summary>原文 :50 — <c>_CSIDL_STARTMENU = $000B;</c></summary>
    public const int _CSIDL_STARTMENU = 0x000B;
    /// <summary>原文 :52 — <c>_CSIDL_MYDOCUMENTS = _CSIDL_PERSONAL;</c></summary>
    public const int _CSIDL_MYDOCUMENTS = _CSIDL_PERSONAL;
    /// <summary>原文 :54 — <c>_CSIDL_MYMUSIC = $000D;</c></summary>
    public const int _CSIDL_MYMUSIC = 0x000D;
    /// <summary>原文 :56 — <c>_CSIDL_MYVIDEO = $000E;</c></summary>
    public const int _CSIDL_MYVIDEO = 0x000E;
    /// <summary>原文 :58 — <c>_CSIDL_DESKTOPDIRECTORY = $0010;</c></summary>
    public const int _CSIDL_DESKTOPDIRECTORY = 0x0010;
    /// <summary>原文 :60 — <c>_CSIDL_DRIVES = $0011;</c></summary>
    public const int _CSIDL_DRIVES = 0x0011;
    /// <summary>原文 :62 — <c>_CSIDL_NETWORK = $0012;</c></summary>
    public const int _CSIDL_NETWORK = 0x0012;
    /// <summary>原文 :64 — <c>_CSIDL_NETHOOD = $0013;</c></summary>
    public const int _CSIDL_NETHOOD = 0x0013;
    /// <summary>原文 :66 — <c>_CSIDL_FONTS = $0014;</c></summary>
    public const int _CSIDL_FONTS = 0x0014;
    /// <summary>原文 :68 — <c>_CSIDL_TEMPLATES = $0015;</c></summary>
    public const int _CSIDL_TEMPLATES = 0x0015;
    /// <summary>原文 :70 — <c>_CSIDL_COMMON_STARTMENU = $0016;</c></summary>
    public const int _CSIDL_COMMON_STARTMENU = 0x0016;
    /// <summary>原文 :72 — <c>_CSIDL_COMMON_PROGRAMS = $0017;</c></summary>
    public const int _CSIDL_COMMON_PROGRAMS = 0x0017;
    /// <summary>原文 :74 — <c>_CSIDL_COMMON_STARTUP = $0018;</c></summary>
    public const int _CSIDL_COMMON_STARTUP = 0x0018;
    /// <summary>原文 :76 — <c>_CSIDL_COMMON_DESKTOPDIRECTORY = $0019;</c></summary>
    public const int _CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019;
    /// <summary>原文 :78 — <c>_CSIDL_APPDATA = $001A;</c></summary>
    public const int _CSIDL_APPDATA = 0x001A;
    /// <summary>原文 :80 — <c>_CSIDL_PRINTHOOD = $001B;</c></summary>
    public const int _CSIDL_PRINTHOOD = 0x001B;
    /// <summary>原文 :82 — <c>_CSIDL_LOCAL_APPDATA = $001C;</c></summary>
    public const int _CSIDL_LOCAL_APPDATA = 0x001C;
    /// <summary>原文 :84 — <c>_CSIDL_ALTSTARTUP = $001D;</c></summary>
    public const int _CSIDL_ALTSTARTUP = 0x001D;
    /// <summary>原文 :86 — <c>_CSIDL_COMMON_ALTSTARTUP = $001E;</c></summary>
    public const int _CSIDL_COMMON_ALTSTARTUP = 0x001E;
    /// <summary>原文 :88 — <c>_CSIDL_COMMON_FAVORITES = $001F;</c></summary>
    public const int _CSIDL_COMMON_FAVORITES = 0x001F;
    /// <summary>原文 :90 — <c>_CSIDL_INTERNET_CACHE = $0020;</c></summary>
    public const int _CSIDL_INTERNET_CACHE = 0x0020;
    /// <summary>原文 :92 — <c>_CSIDL_COOKIES = $0021;</c></summary>
    public const int _CSIDL_COOKIES = 0x0021;
    /// <summary>原文 :94 — <c>_CSIDL_HISTORY = $0022;</c></summary>
    public const int _CSIDL_HISTORY = 0x0022;
    /// <summary>原文 :96 — <c>_CSIDL_COMMON_APPDATA = $0023;</c></summary>
    public const int _CSIDL_COMMON_APPDATA = 0x0023;
    /// <summary>原文 :98 — <c>_CSIDL_WINDOWS = $0024;</c></summary>
    public const int _CSIDL_WINDOWS = 0x0024;
    /// <summary>原文 :100 — <c>_CSIDL_SYSTEM = $0025;</c></summary>
    public const int _CSIDL_SYSTEM = 0x0025;
    /// <summary>原文 :102 — <c>_CSIDL_PROGRAM_FILES = $0026;</c></summary>
    public const int _CSIDL_PROGRAM_FILES = 0x0026;
    /// <summary>原文 :104 — <c>_CSIDL_MYPICTURES = $0027;</c></summary>
    public const int _CSIDL_MYPICTURES = 0x0027;
    /// <summary>原文 :106 — <c>_CSIDL_PROFILE = $0028;</c></summary>
    public const int _CSIDL_PROFILE = 0x0028;
    /// <summary>原文 :108 — <c>_CSIDL_SYSTEMX86 = $0029;</c>（64 位系统上的 SysWOW64）</summary>
    public const int _CSIDL_SYSTEMX86 = 0x0029;
    /// <summary>原文 :110 — <c>_CSIDL_PROGRAM_FILESX86 = $002A;</c></summary>
    public const int _CSIDL_PROGRAM_FILESX86 = 0x002A;
    /// <summary>原文 :112 — <c>_CSIDL_PROGRAM_FILES_COMMON = $002B;</c></summary>
    public const int _CSIDL_PROGRAM_FILES_COMMON = 0x002B;
    /// <summary>原文 :114 — <c>_CSIDL_PROGRAM_FILES_COMMONX86 = $002C;</c></summary>
    public const int _CSIDL_PROGRAM_FILES_COMMONX86 = 0x002C;
    /// <summary>原文 :116 — <c>_CSIDL_COMMON_TEMPLATES = $002D;</c></summary>
    public const int _CSIDL_COMMON_TEMPLATES = 0x002D;
    /// <summary>原文 :118 — <c>_CSIDL_COMMON_DOCUMENTS = $002E;</c></summary>
    public const int _CSIDL_COMMON_DOCUMENTS = 0x002E;
    /// <summary>原文 :120 — <c>_CSIDL_COMMON_ADMINTOOLS = $002F;</c></summary>
    public const int _CSIDL_COMMON_ADMINTOOLS = 0x002F;
    /// <summary>原文 :122 — <c>_CSIDL_ADMINTOOLS = $0030;</c></summary>
    public const int _CSIDL_ADMINTOOLS = 0x0030;
    /// <summary>原文 :124 — <c>_CSIDL_CONNECTIONS = $0031;</c></summary>
    public const int _CSIDL_CONNECTIONS = 0x0031;
    /// <summary>原文 :126 — <c>_CSIDL_COMMON_MUSIC = $0035;</c></summary>
    public const int _CSIDL_COMMON_MUSIC = 0x0035;
    /// <summary>原文 :128 — <c>_CSIDL_COMMON_PICTURES = $0036;</c></summary>
    public const int _CSIDL_COMMON_PICTURES = 0x0036;
    /// <summary>原文 :130 — <c>_CSIDL_COMMON_VIDEO = $0037;</c></summary>
    public const int _CSIDL_COMMON_VIDEO = 0x0037;
    /// <summary>原文 :132 — <c>_CSIDL_RESOURCES = $0038;</c></summary>
    public const int _CSIDL_RESOURCES = 0x0038;
    /// <summary>原文 :134 — <c>_CSIDL_RESOURCES_LOCALIZED = $0039;</c></summary>
    public const int _CSIDL_RESOURCES_LOCALIZED = 0x0039;
    /// <summary>原文 :136 — <c>_CSIDL_COMMON_OEM_LINKS = $003A;</c></summary>
    public const int _CSIDL_COMMON_OEM_LINKS = 0x003A;
    /// <summary>原文 :138 — <c>_CSIDL_CDBURN_AREA = $003B;</c></summary>
    public const int _CSIDL_CDBURN_AREA = 0x003B;
    /// <summary>原文 :141 — <c>_CSIDL_COMPUTERSNEARME = $003D;</c></summary>
    public const int _CSIDL_COMPUTERSNEARME = 0x003D;

    /// <summary>原文 :143 — <c>_CSIDL_FLAG_CREATE = $8000;</c></summary>
    public const int _CSIDL_FLAG_CREATE = 0x8000;
    /// <summary>原文 :145 — <c>_CSIDL_FLAG_DONT_VERIFY = $4000;</c></summary>
    public const int _CSIDL_FLAG_DONT_VERIFY = 0x4000;
    /// <summary>原文 :147 — <c>_CSIDL_FLAG_DONT_UNEXPAND = $2000;</c></summary>
    public const int _CSIDL_FLAG_DONT_UNEXPAND = 0x2000;
    /// <summary>原文 :149 — <c>_CSIDL_FLAG_NO_ALIAS = $1000;</c></summary>
    public const int _CSIDL_FLAG_NO_ALIAS = 0x1000;
    /// <summary>原文 :151 — <c>_CSIDL_FLAG_PER_USER_INIT = $0800;</c></summary>
    public const int _CSIDL_FLAG_PER_USER_INIT = 0x0800;
    /// <summary>原文 :153 — <c>_CSIDL_FLAG_MASK = $FF00;</c></summary>
    public const int _CSIDL_FLAG_MASK = 0xFF00;

    /// <summary>原文 :22 — <c>ServerModuleMD5List:THashList;</c>（桶容量 65536，原文 :356）</summary>
    public const int HashListCapacity = 65536;
}

/// <summary>
/// CheckProcessModules.pas 1:1 移植（Win32 部分收敛为托管等价 API）。
/// </summary>
public class CheckProcessModules
{
    // ── 原文 :22-25 的全局哈希表（托管侧为 Dictionary）──────────────────────
    /// <summary>原文 :22 — <c>ServerModuleMD5List:THashList;</c></summary>
    public readonly HashSet<string> ServerModuleMD5List = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :23 — <c>BlackModuleMD5List:THashList; // 黑名单 MD5</c></summary>
    public readonly HashSet<string> BlackModuleMD5List = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :24 — <c>BlackModuleList:THashList; // 黑名单</c></summary>
    public readonly HashSet<string> BlackModuleList = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :25 — <c>AddModuleMD5List:THashList;</c></summary>
    public readonly HashSet<string> AddModuleMD5List = new HashSet<string>(StringComparer.Ordinal);

    // ── 原文 :158-162 的 implementation 段私有哈希表 ───────────────────────
    /// <summary>原文 :158 — <c>SystemDllList:THashList;</c></summary>
    public readonly HashSet<string> SystemDllList = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :159 — <c>ModulePathList:THashList;</c></summary>
    public readonly HashSet<string> ModulePathList = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :160 — <c>ModuleMD5List:THashList;</c></summary>
    public readonly HashSet<string> ModuleMD5List = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :161 — <c>ModuleFileList:THashList;</c>（**已判定为可信**的模块）</summary>
    public readonly HashSet<string> ModuleFileList = new HashSet<string>(StringComparer.Ordinal);
    /// <summary>原文 :162 — <c>UnKnowModuleFileList:THashList;</c>（**已判定为不可信**的模块）</summary>
    public readonly HashSet<string> UnKnowModuleFileList = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>原文 :164 — <c>SysWOW64:string = '';</c>（64 位系统的 SysWOW64 目录；与 System32 相同时被清空）</summary>
    public string SysWOW64 = string.Empty;

    /// <summary>原文 :166-227 — <c>CopyrightArray:array[0..59] of string</c>（**加密态**，60 条）。</summary>
    public static readonly string[] CopyrightArrayEncrypted =
    {
        "cQHSXsW?PoUSQfMRTsbJ",            // 'Microsoft', //微软
        "ZpVaIgHPDgXBaaXcUekY",           // 'Sogou.com', //搜狗
        "OwFri=XQYnNbU]UopyK@",           // 'Tencent', //腾讯
        "UnUpmRZbeJJPqaYopytb",           // 'Thunder', //迅雷
        "veW@ydP_MRNpHsa",                // '360.cn', //360
        "pPOa]sZaUCWraTMp@yVi",           // 'Kingsoft', //金山
        "gaUBQrLoAfZ@ESOSTnwN",           // 'Kaspersky', //卡巴
        "czHbEmYPmPYCH<L",                // 'Rising', //瑞星
        "mIWbqmZ^yLI`IjQ?<yW>",           // 'Symantec', //诺顿
        "xCJ?=NRQduWny]QOIdFsXyKLxw",     // 'Micropoint', //微点
        "WCFr]aXrUVQPpqMSXyeg",           // 'Jiangmin',  //江民
        "XtIa=RZcdpTopLk",                // 'Baidu',百度
        "GTU@icUsAcKOpxv",                // '百度'
        "kaUsdoNPAqNOpdO",                // 'Apple', //苹果
        "kuW_PlTPyIPBabMOLyAc",           // 'Broadcom',
        "y[PCa@ZaQCJ?plG",                // 'AVAST',
        "ZiYbQ>JSYpYPPHk",                // 'McAfee',
        "VINSajUbuMKOpvU",                // 'ESET',
        "iRRA]KI@qNHQQEUQMoGrQ=KLZA",     // 'Trend Micro', //趋势科技
        "@lMqerIp]lMRpkYopyxm",           // 'Lingoes', //灵格斯词霸
        "uPJ?USLReINB]GQsDkX_]fYOQqPBduUopyjZ", // 'Jean-loup Gailly' Jean-loup Gailly & Mark Adler
        "rmTRMqZcAcV@hOz",                // NVIDIA
        "PSPS=aTc]dJ@EvN?YdI@yLUoUKTc@kLOpywT", // ATI Technologies
        "TxI@Q_WBEuVa]nRRxyxb",           // Advanced
        "NtOs=RNsErIRApNBtpWrQvUoMcKOpgR", // www.52hxw.com
        "nnIbMGYCAlZ_EVPOpysW",           // Feitian
        "C>YPygXRiSZBpVZ",                // Andrea
        "epWBxnXrMnNOpKh",                // Adobe
        "JrGrygLQ]^YpYgPOpyI<",           // Brother
        "WNTNypYbaMKOpHj",                // DTS.
        "dWWQXoLo=DRcaLJ@miIrXyKL=G",     // Fortemedia
        "CBVcaSW_=qWqXUG",                // Radius
        "ciY@a]N`dpGq=TLOXuI`ynJLqh",     // Heidelberger
        "BAPs<gIRa]OQXrTNxuHpxsX?UmNCHpOp`yHI", // www.EnterSafe.com
        "hOWcUsMA=gMOpLU",                // Intel
        "OZIaaIGrenY_=LYopy[I",           // Knowles
        "L<XPiKHSQQJ?p=C",                // Waves
        "fPIoYKJButTpLuPOpygW",           // Infosec
        "CaOAesHS]MKOpUa",                // Real
        "cpQCIoZBQ=ZRILUrupIA<oYNhgM?`qYSDtIF", // Dolby Laboratories
        "ErZaacQbUtVBlkYopyRl",           // Realtek
        "bnRbAFRBtmRQYoM@@yQa",           // Synopsys
        "gnMC]NTaEsKOpnj",                // Sony
        "OKMryVTOY^PSTlMo<yjZ",           // SRS Labs
        "eGWrMiVoLrTsEjP?HtCw",           // Synaptics
        "mHPQ]]IrAuZA]cPOpyjT",           // TOSHIBA
        "ujW_=vYbPuRRxix",                // VMware
        "UPN_PrXaekYqEBPOpyoa",           // GameCap
        "ASTbyoIbYfFoL[k",                // Lenovo
        "KeM@MQNoIqNpusTbuoFnxoUoMaTcHQH", // Microelectronic
        "dRNRyaJOAhJ@HoOp@yxO",           // nianqing
        "fjQoLoQpEMKOpsC",                // 念青
        "WXMAetIbaVPO@ef",                // '新浪网',
        "fdYQXqUceSNPtms",                // '易语言',
        "YAROYQPoAfJPxrn",                // '奇虎网',
        "wHMpaBPRqsKOpKP",                // '巨盾'
        "XHURhkR_YsKOpt<",                // '极点五笔'
        "QLOsaDXC]>PC]mQPPyXh",           // '恒信科技'
        "BQHbdsR@AsWSA?NBXyyp",           // 飞天诚信
        "m[TrYdWs]?IPy@Q@pyuS",           // 农业银行
    };

    /// <summary>
    /// 原文 :342-370 <c>InitModules</c> 里的 <c>CopyrightArray[I] := DecryString(CopyrightArray[I])</c>
    /// 结果（明文版权串）。原文在单元 initialization 段（:432-433）调用 <c>InitModules</c>，
    /// 即**进程启动时就把 60 条全部解密**。
    /// </summary>
    public string[] CopyrightArray { get; private set; }

    /// <summary>
    /// 原文 :365 — <c>sSystem32 := IncludeTrailingPathDelimiter(LowerCase(GetSpecialFolderDir(_CSIDL_SYSTEM)));</c>
    /// </summary>
    public string System32 { get; private set; }

    /// <summary>原文 :435-444 的 finalization：托管侧由 GC 承担，本方法用于显式复位。</summary>
    public void FinalizeLists()
    {
        SystemDllList.Clear();
        ModulePathList.Clear();
        ModuleMD5List.Clear();
        ModuleFileList.Clear();
        UnKnowModuleFileList.Clear();
        ServerModuleMD5List.Clear();
        AddModuleMD5List.Clear();
        BlackModuleMD5List.Clear();
        BlackModuleList.Clear();
    }

    /// <summary>
    /// 原文 :342-370 <c>procedure InitModules;</c>
    /// <para>注意原文 :362-363 的循环用 <c>Length(CopyrightArray) - 1</c>（0..59），
    /// 且把解密结果**写回原数组**（就地替换）。</para>
    /// <para>原文 :368-369：<c>SysWOW64 := IncludeTrailingPathDelimiter(LowerCase(GetSpecialFolderDir(_CSIDL_SYSTEMX86)));</c>
    /// 紧接着 <c>if (SysWOW64 = sSystem32) then SysWOW64 := '';</c> —— **32 位系统上两者相同，
    /// 于是 SysWOW64 被清空**（这是原文区分 32/64 位的方式）。</para>
    /// </summary>
    /// <param name="decrypt">原文的 <c>DecryString</c>；默认转调 <c>GXX.Core.Crypto.EncryptUnit.DecryString</c>。</param>
    public void InitModules(Func<string, string> decrypt = null)
    {
        decrypt ??= GXX.Core.Crypto.EncryptUnit.DecryString;

        CopyrightArray = new string[CopyrightArrayEncrypted.Length];
        for (int I = 0; I <= CopyrightArrayEncrypted.Length - 1; I++)
        {
            // 原文如此（CheckProcessModules.pas:363）：CopyrightArray[I] := DecryString(CopyrightArray[I]);
            CopyrightArray[I] = decrypt(CopyrightArrayEncrypted[I]);
        }

        System32 = IncludeTrailingPathDelimiter(LowerCaseSafe(GetSpecialFolderDir(
            CheckProcessModulesConst._CSIDL_SYSTEM)));

        // C:\Windows\SysWOW64
        SysWOW64 = IncludeTrailingPathDelimiter(LowerCaseSafe(GetSpecialFolderDir(
            CheckProcessModulesConst._CSIDL_SYSTEMX86)));
        if (SysWOW64 == System32) SysWOW64 = string.Empty;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 纯逻辑
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :372-384 <c>function CompareLStr(Src, targ: string; compn: Integer): Boolean;</c>
    /// <para>逐字符忽略大小写比较**前 compn 个字符**；<c>compn &lt;= 0</c> 或任一串短于 compn
    /// 时直接返回 False（原文 <c>Exit</c> 时 Result 仍是入口赋的 False）。</para>
    /// </summary>
    public static bool CompareLStr(string Src, string targ, int compn)
    {
        bool Result = false;
        // 原文如此（CheckProcessModules.pas:377）：if (compn <= 0) or (Length(Src) < compn) or (Length(targ) < compn) then Exit;
        if (compn <= 0 || (Src ?? string.Empty).Length < compn || (targ ?? string.Empty).Length < compn)
            return false;

        Result = true;
        for (int I = 0; I < compn; I++)
        {
            // 原文 :380：if UpCase(Src[I]) <> UpCase(targ[I])
            if (char.ToUpperInvariant(Src[I]) != char.ToUpperInvariant(targ[I]))
            {
                Result = false;
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 :229-255 <c>ReadRegKey</c> 的**模式分发**（纯逻辑，供测试）。
    /// <para>模式 1 = 读字符串后 <c>LowerCase(Trim(...))</c>；模式 2 = 读整数转字符串；
    /// 其它模式不改动 <c>sResult</c>。空结果按失败处理。</para>
    /// </summary>
    public static (bool Ok, string Result) ReadRegKeyDispatch(int iMode, Func<string> readString, Func<int> readInteger)
    {
        string sResult = string.Empty;
        switch (iMode)
        {
            case 1:
                // 原文 :241：sResult := LowerCase(Trim(ReadString(sKeyName)));
                sResult = LowerCaseSafe((readString() ?? string.Empty).Trim());
                break;
            case 2:
                // 原文 :242：sResult := IntToStr(ReadInteger(sKeyName));
                sResult = readInteger().ToString(System.Globalization.CultureInfo.InvariantCulture);
                break;
            // 原文如此（CheckProcessModules.pas:243）：// 3: sResult := ReadBinaryData(sKeyName, Buffer, BufSize);
        }

        // 原文 :245-248：if sResult = '' then Result := False else Result := True;
        return (sResult != string.Empty, sResult);
    }

    /// <summary>
    /// 原文 :258-283 <c>WriteRegKey</c> 的**模式分发**（纯逻辑，供测试）。
    /// <para>模式 3（二进制）原文写的是**未初始化的局部变量 <c>bData:Byte</c>**
    /// 且长度恒为 1 —— 即写入 1 个**未定义值**的字节。这是原文缺陷（见报告 §5）；
    /// 本移植保留"长度 1"这一形状，但把值参数化以便测试。</para>
    /// </summary>
    public static void WriteRegKeyDispatch(int iMode, string sKeyValue,
        Action<string> writeString, Action<int> writeInteger, Action<byte> writeBinary)
    {
        switch (iMode)
        {
            case 1:
                // 原文 :270：WriteString(sKeyName, sKeyValue);
                writeString(sKeyValue);
                break;
            case 2:
                // 原文 :271：WriteInteger(sKeyName, StrToInt(sKeyValue));
                writeInteger(int.Parse(sKeyValue, System.Globalization.CultureInfo.InvariantCulture));
                break;
            case 3:
                // 原文 :272：WriteBinaryData(sKeyName, bData, 1);  ← bData **未初始化**
                writeBinary(0);
                break;
        }
    }

    /// <summary>
    /// 原文 :285-312 <c>function GetSpecialFolderDir(mFolder: Integer): string;</c>
    /// （<c>SHGetSpecialFolderLocation</c> + <c>SHGetPathFromIDList</c>）
    /// <para>托管侧用 <c>Environment.GetFolderPath</c> 表达同一 CSIDL 语义；
    /// 未映射的 CSIDL 返回空串（原文该情形下 <c>vBuffer</c> 保持未初始化，
    /// 结果不确定，托管侧取空串）。</para>
    /// </summary>
    public static string GetSpecialFolderDir(int mFolder)
    {
        switch (mFolder)
        {
            case CheckProcessModulesConst._CSIDL_DESKTOP:
            case CheckProcessModulesConst._CSIDL_DESKTOPDIRECTORY:
                return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            case CheckProcessModulesConst._CSIDL_PERSONAL:      // _CSIDL_MYDOCUMENTS 同值
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            case CheckProcessModulesConst._CSIDL_FAVORITES:
                return Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            case CheckProcessModulesConst._CSIDL_STARTUP:
                return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            case CheckProcessModulesConst._CSIDL_RECENT:
                return Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            case CheckProcessModulesConst._CSIDL_SENDTO:
                return Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
            case CheckProcessModulesConst._CSIDL_STARTMENU:
                return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            case CheckProcessModulesConst._CSIDL_MYMUSIC:
                return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            case CheckProcessModulesConst._CSIDL_MYVIDEO:
                return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            case CheckProcessModulesConst._CSIDL_NETWORK:
                return Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts);
            case CheckProcessModulesConst._CSIDL_FONTS:
                return Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            case CheckProcessModulesConst._CSIDL_TEMPLATES:
                return Environment.GetFolderPath(Environment.SpecialFolder.Templates);
            case CheckProcessModulesConst._CSIDL_COMMON_STARTMENU:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            case CheckProcessModulesConst._CSIDL_COMMON_PROGRAMS:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
            case CheckProcessModulesConst._CSIDL_COMMON_STARTUP:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
            case CheckProcessModulesConst._CSIDL_COMMON_DESKTOPDIRECTORY:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            case CheckProcessModulesConst._CSIDL_APPDATA:
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            case CheckProcessModulesConst._CSIDL_PRINTHOOD:
                return string.Empty; // 原文 _CSIDL_PRINTHOOD：.NET 无对应 SpecialFolder（已登记）
            case CheckProcessModulesConst._CSIDL_LOCAL_APPDATA:
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            case CheckProcessModulesConst._CSIDL_COMMON_FAVORITES:
                return string.Empty; // 原文 _CSIDL_COMMON_FAVORITES：.NET 无对应 SpecialFolder（已登记）
            case CheckProcessModulesConst._CSIDL_INTERNET_CACHE:
                return Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            case CheckProcessModulesConst._CSIDL_COOKIES:
                return Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
            case CheckProcessModulesConst._CSIDL_HISTORY:
                return Environment.GetFolderPath(Environment.SpecialFolder.History);
            case CheckProcessModulesConst._CSIDL_COMMON_APPDATA:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            case CheckProcessModulesConst._CSIDL_WINDOWS:
                return Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            case CheckProcessModulesConst._CSIDL_SYSTEM:
                return Environment.GetFolderPath(Environment.SpecialFolder.System);
            case CheckProcessModulesConst._CSIDL_PROGRAM_FILES:
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            case CheckProcessModulesConst._CSIDL_MYPICTURES:
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            case CheckProcessModulesConst._CSIDL_PROFILE:
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            case CheckProcessModulesConst._CSIDL_SYSTEMX86:
                return Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            case CheckProcessModulesConst._CSIDL_PROGRAM_FILESX86:
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            case CheckProcessModulesConst._CSIDL_PROGRAM_FILES_COMMON:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            case CheckProcessModulesConst._CSIDL_PROGRAM_FILES_COMMONX86:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
            case CheckProcessModulesConst._CSIDL_COMMON_TEMPLATES:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates);
            case CheckProcessModulesConst._CSIDL_COMMON_DOCUMENTS:
                return Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            case CheckProcessModulesConst._CSIDL_COMMON_ADMINTOOLS:
                return string.Empty; // 原文 _CSIDL_COMMON_ADMINTOOLS：.NET 无对应 SpecialFolder（已登记）
            case CheckProcessModulesConst._CSIDL_ADMINTOOLS:
                return string.Empty; // 原文 _CSIDL_ADMINTOOLS：.NET 无对应 SpecialFolder（已登记）
            case CheckProcessModulesConst._CSIDL_CDBURN_AREA:
                return Environment.GetFolderPath(Environment.SpecialFolder.CDBurning);
            case CheckProcessModulesConst._CSIDL_COMPUTERSNEARME:
                return Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts);
            default:
                // 原文对未映射 CSIDL 会返回未初始化的 vBuffer（不确定值）——托管侧返回空串
                return string.Empty;
        }
    }

    /// <summary>
    /// 原文 :314-340 <c>function GetFileLegalCopyright(sFileName: string): string;</c>
    ///
    /// <para><b>原文缺陷（见报告 §5）</b>：第 325 行
    /// <c>InfoSize := GetFileVersionInfoSize(PChar(sFileName), InfoSize);</c>
    /// 把 <c>InfoSize</c> 同时当**出参**传入 —— 该 API 会用 <c>GetLastError</c> 覆写它（成功时 0），
    /// 而函数的**返回值**（真实版本资源大小）被丢弃 ⇒ <c>InfoSize</c> 恒为 0，
    /// 随后 <c>AllocMem(0)</c> 得到零长缓冲、<c>GetFileVersionInfo</c> 必然失败。
    /// 换言之原文这个函数**实际上永远返回空串**。</para>
    ///
    /// <para>本移植用 <c>FileVersionInfo</c> 取得**原文注释所意图的结果**
    /// （<c>Trim(LegalCopyright)</c>），差异已登记。</para>
    /// </summary>
    public static string GetFileLegalCopyright(string sFileName)
    {
        if (string.IsNullOrEmpty(sFileName) || !File.Exists(sFileName)) return string.Empty;
        try
        {
            var vi = FileVersionInfo.GetVersionInfo(sFileName);
            // 原文 :334：Result := Trim(string(PChar(InfoPointer)));
            return (vi.LegalCopyright ?? string.Empty).Trim();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 原文 <c>MD5Util.RivestFile</c>（文件 MD5 十六进制）。原文 <c>RivestFile</c> 返回 32 位小写 hex。
    /// <para>托管侧用 <c>MD5.HashData</c> 流式读取，签名与原文一致。
    /// 文件不存在时返回空串（原文后续 <c>if (sMD5 &lt;&gt; '') and ...</c> 依赖该取值）。</para>
    /// </summary>
    public static string RivestFile(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return string.Empty;
        try
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(fs);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 原文 <c>FastStrings.FastPosNoCase</c>（忽略大小写的子串查找）。
    /// <para>原文是 Boyer-Moore 加速版，**返回值语义**是 1-based 位置（未命中 0）。
    /// 调用点只判 <c>&gt; 0</c>，故托管侧用 <c>IndexOf(OrdinalIgnoreCase)</c> +
    /// 1-based 换算即可保持该判据。</para>
    /// </summary>
    public static int FastPosNoCase(string src, string pattern)
    {
        if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(pattern)) return 0;
        int idx = src.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
        return idx < 0 ? 0 : idx + 1;
    }

    /// <summary>
    /// 原文 :386-430 <c>function CheckProcessModule(const sMoudle: string): Boolean;</c>
    /// <para>判定顺序（原文逐条）：</para>
    /// <list type="number">
    /// <item>（原文第 391-394 行有一段**被注释掉**的 <c>BlackModuleList.Exists</c> 前置检查 —— 保留注释，不生效）</item>
    /// <item>已在白名单 <c>ModuleFileList</c> ⇒ True</item>
    /// <item>已在黑名单文件表 <c>UnKnowModuleFileList</c> ⇒ False</item>
    /// <item>MD5 命中 <c>BlackModuleMD5List</c> ⇒ 记入 <c>BlackModuleList</c> 并 False</item>
    /// <item>版权串包含 <c>CopyrightArray</c> 中任一子串 ⇒ 记入白名单并 True</item>
    /// <item>MD5 命中 <c>ModuleMD5List</c> 或 <c>ServerModuleMD5List</c> ⇒ 记入白名单并 True</item>
    /// <item>否则记入 <c>UnKnowModuleFileList</c> 并 False</item>
    /// </list>
    /// </summary>
    public bool CheckProcessModule(string sMoudle)
    {
        /* 原文如此（CheckProcessModules.pas:391-394）：以下黑名单前置检查被注释掉
           if BlackModuleList.Exists(sMoudle) then begin
              Result := False;
              Exit;
           end; */

        bool Result = ModuleFileList.Contains(sMoudle);
        if (!Result)
        {
            if (UnKnowModuleFileList.Contains(sMoudle)) return false;

            string sMD5 = RivestFile(sMoudle);
            if (BlackModuleMD5List.Contains(sMD5))
            {
                BlackModuleList.Add(sMoudle);
                // 原文如此（CheckProcessModules.pas:403-404）：Result := False; Exit;
                return false;
            }

            string sCopyright = GetFileLegalCopyright(sMoudle);
            if (sCopyright != string.Empty && CopyrightArray != null)
            {
                for (int I = 0; I <= CopyrightArray.Length - 1; I++)
                {
                    // 原文 :410：FastPosNoCase(sCopyright, CopyrightArray[I], Length(sCopyright), Length(CopyrightArray[I]), 1) > 0
                    if (FastPosNoCase(sCopyright, CopyrightArray[I]) > 0)
                    {
                        ModuleFileList.Add(sMoudle);
                        // 原文如此（CheckProcessModules.pas:412-413）：Result := True; Exit;
                        return true;
                    }
                }
            }

            if (sMD5 != string.Empty && ModuleMD5List.Contains(sMD5))
            {
                ModuleFileList.Add(sMoudle);
                Result = true;
            }
            else if (sMD5 != string.Empty && ServerModuleMD5List.Contains(sMD5))
            {
                ModuleFileList.Add(sMoudle);
                Result = true;
            }
            else
            {
                UnKnowModuleFileList.Add(sMoudle);
            }
        }
        return Result;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 托管辅助（原文由 Windows/SysUtils 提供）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>原文 <c>SysUtils.IncludeTrailingPathDelimiter</c>：补尾部分隔符（已有则不动）。</summary>
    public static string IncludeTrailingPathDelimiter(string s)
    {
        if (string.IsNullOrEmpty(s)) return Path.DirectorySeparatorChar.ToString();
        return s.EndsWith(Path.DirectorySeparatorChar) || s.EndsWith(Path.AltDirectorySeparatorChar)
            ? s
            : s + Path.DirectorySeparatorChar;
    }

    private static string LowerCaseSafe(string s) => (s ?? string.Empty).ToLowerInvariant();

    /// <summary>原文 <c>GetSpecialFolderDir</c> 的注册表辅助（原文用 TRegistry，见 :229-283）。</summary>
    public static bool ReadRegKey(RegistryHive rootHive, int iMode, string sPath, string sKeyName,
        out string sResult)
    {
        sResult = string.Empty;
        try
        {
            using var root = RegistryKey.OpenBaseKey(rootHive, RegistryView.Default);
            using var key = root.OpenSubKey(sPath, false);
            if (key == null) return false;

            var r = ReadRegKeyDispatch(iMode,
                () => key.GetValue(sKeyName) as string ?? (key.GetValue(sKeyName)?.ToString() ?? string.Empty),
                () => key.GetValue(sKeyName) is int i ? i : Convert.ToInt32(key.GetValue(sKeyName) ?? 0));
            sResult = r.Result;
            return r.Ok;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>原文 <c>WriteRegKey</c>（:258-283）。</summary>
    public static bool WriteRegKey(RegistryHive rootHive, int iMode, string sPath, string sKeyName,
        string sKeyValue)
    {
        try
        {
            using var root = RegistryKey.OpenBaseKey(rootHive, RegistryView.Default);
            using var key = root.CreateSubKey(sPath, true);
            if (key == null) return false;

            WriteRegKeyDispatch(iMode, sKeyValue,
                v => key.SetValue(sKeyName, v, RegistryValueKind.String),
                v => key.SetValue(sKeyName, v, RegistryValueKind.DWord),
                v => key.SetValue(sKeyName, new byte[] { v }, RegistryValueKind.Binary));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>原文 <c>ReadRegKey</c> 的 HKEY_LOCAL_MACHINE 便捷重载（原文注释：// HKEY_LOCAL_MACHINE）。</summary>
    public static bool ReadRegKey(int RegRootKey, int iMode, string sPath, string sKeyName, out string sResult)
        => ReadRegKey(RegistryHive.LocalMachine, iMode, sPath, sKeyName, out sResult);

    /// <summary>原文 <c>WriteRegKey</c> 的便捷重载。</summary>
    public static bool WriteRegKey(int RegRootKey, int iMode, string sPath, string sKeyName, string sKeyValue)
        => WriteRegKey(RegistryHive.LocalMachine, iMode, sPath, sKeyName, sKeyValue);
}
