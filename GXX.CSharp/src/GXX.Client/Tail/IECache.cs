// 源单元：Source/Client-HGE/IECache.pas（原文 723 行 / CRLF 计入 1297 行）
// 原文 uses：wininet, Windows, Messages, SysUtils, Classes
// 原文无同名 .dfm（TIECache 是 TComponent 派生，不是窗体）。
//
// ⚠ 覆盖事实（已核）：IECache **不在 Client.dpr 的 uses 列表中**
//   （Client.dpr:77 有 `IECache in 'IECache.pas'`），且整棵源码树**没有任何 TIECache 的使用点**
//   （仅:77 的工程引用 + 本单元自身）。它是一份"引而不入"的第三方组件
//   （原文 1-25 行的版权头：Per Lindsø Larsen / Christian Lovis，"IE CACHE Component v1.02"）。
//
// 移植策略（任务书：解析/校验/状态机/字符串处理抽成可测函数；Win32 调用收敛到接缝）：
//   · 纯逻辑 1:1 移植（可单测）：
//       - CACHEGROUP_* / GROUPNAME_MAX_LENGTH 等常量（:41-77）
//       - TFilterOption 枚举与 UpdateFilterOptionValue 的**位值求和**（:1271-1282）
//       - TSearchPattern 与 FindFirstEntry 的**前缀模式表**（:1121）
//       - SetFilterOptions 的赋值语义（:1261-1265）
//       - TSockAddr 无关的 TGroupInfo/TEntryInfo/TContent 记录结构（:111-159）
//   · WinInet 调用（原文 :299-331 的动态绑定 + :431-1265 的 20 个方法）收敛为接缝
//     <see cref="IECacheSeam"/>：托管侧不加载 wininet.dll，方法保留签名并返回"未启用"，
//     由调用方改走 HttpClient/CookieContainer。映射表见 <see cref="IECacheSeam.FunctionMap"/>。
using System;
using System.Collections.Generic;

namespace GXX.Client.Tail;

/// <summary>原文 :39-77 的 WinInet 缓存组常量（1:1 保留）。</summary>
public static class IECacheConst
{
    /// <summary>原文 :41 — <c>CACHEGROUP_ATTRIBUTE_GET_ALL = $FFFFFFFF;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_GET_ALL = 0xFFFFFFFF;
    /// <summary>原文 :43 — <c>CACHEGROUP_ATTRIBUTE_BASIC = $00000001;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_BASIC = 0x00000001;
    /// <summary>原文 :45 — <c>CACHEGROUP_ATTRIBUTE_FLAG = $00000002;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_FLAG = 0x00000002;
    /// <summary>原文 :47 — <c>CACHEGROUP_ATTRIBUTE_TYPE = $00000004;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_TYPE = 0x00000004;
    /// <summary>原文 :49 — <c>CACHEGROUP_ATTRIBUTE_QUOTA = $00000008;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_QUOTA = 0x00000008;
    /// <summary>原文 :51 — <c>CACHEGROUP_ATTRIBUTE_GROUPNAME = $00000010;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_GROUPNAME = 0x00000010;
    /// <summary>原文 :53 — <c>CACHEGROUP_ATTRIBUTE_STORAGE = $00000020;</c></summary>
    public const uint CACHEGROUP_ATTRIBUTE_STORAGE = 0x00000020;

    /// <summary>原文 :55 — <c>CACHEGROUP_FLAG_NONPURGEABLE = $00000001;</c></summary>
    public const uint CACHEGROUP_FLAG_NONPURGEABLE = 0x00000001;
    /// <summary>原文 :57 — <c>CACHEGROUP_FLAG_GIDONLY = $00000004;</c></summary>
    public const uint CACHEGROUP_FLAG_GIDONLY = 0x00000004;
    /// <summary>原文 :59 — <c>CACHEGROUP_FLAG_FLUSHURL_ONDELETE = $00000002;</c></summary>
    public const uint CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 0x00000002;

    /// <summary>原文 :61 — <c>CACHEGROUP_SEARCH_ALL = $00000000;</c></summary>
    public const uint CACHEGROUP_SEARCH_ALL = 0x00000000;
    /// <summary>原文 :63 — <c>CACHEGROUP_SEARCH_BYURL = $00000001;</c></summary>
    public const uint CACHEGROUP_SEARCH_BYURL = 0x00000001;
    /// <summary>原文 :65 — <c>CACHEGROUP_TYPE_INVALID = $00000001;</c></summary>
    public const uint CACHEGROUP_TYPE_INVALID = 0x00000001;

    /// <summary>
    /// 原文 :67-73 — <c>CACHEGROUP_READWRITE_MASK = CACHEGROUP_ATTRIBUTE_TYPE or
    /// CACHEGROUP_ATTRIBUTE_QUOTA or CACHEGROUP_ATTRIBUTE_GROUPNAME or CACHEGROUP_ATTRIBUTE_STORAGE;</c>
    /// </summary>
    public const uint CACHEGROUP_READWRITE_MASK =
        CACHEGROUP_ATTRIBUTE_TYPE | CACHEGROUP_ATTRIBUTE_QUOTA |
        CACHEGROUP_ATTRIBUTE_GROUPNAME | CACHEGROUP_ATTRIBUTE_STORAGE;

    /// <summary>原文 :75 — <c>GROUPNAME_MAX_LENGTH = 120;</c></summary>
    public const int GROUPNAME_MAX_LENGTH = 120;
    /// <summary>原文 :77 — <c>GROUP_OWNER_STORAGE_SIZE = 4;</c></summary>
    public const int GROUP_OWNER_STORAGE_SIZE = 4;
}

/// <summary>
/// 原文 :161-177 <c>TFilterOption</c> 枚举（顺序即原文声明的位值顺序，
/// 与 <see cref="IECache.FilterOptionValues"/> 逐项对应）。
/// </summary>
public enum TFilterOption
{
    /// <summary>原文 :161 — <c>NORMAL_ENTRY</c></summary>
    NORMAL_ENTRY = 0,
    /// <summary>原文 :163 — <c>STABLE_ENTRY</c></summary>
    STABLE_ENTRY = 1,
    /// <summary>原文 :165 — <c>STICKY_ENTRY</c></summary>
    STICKY_ENTRY = 2,
    /// <summary>原文 :167 — <c>COOKIE_ENTRY</c></summary>
    COOKIE_ENTRY = 3,
    /// <summary>原文 :169 — <c>URLHISTORY_ENTRY</c></summary>
    URLHISTORY_ENTRY = 4,
    /// <summary>原文 :171 — <c>TRACK_OFFLINE_ENTRY</c></summary>
    TRACK_OFFLINE_ENTRY = 5,
    /// <summary>原文 :173 — <c>TRACK_ONLINE_ENTRY</c></summary>
    TRACK_ONLINE_ENTRY = 6,
    /// <summary>原文 :175 — <c>SPARSE_ENTRY</c></summary>
    SPARSE_ENTRY = 7,
    /// <summary>原文 :177 — <c>OCX_ENTRY</c></summary>
    OCX_ENTRY = 8,
}

/// <summary>原文 :185 — <c>TSearchPattern = (spAll, spCookies, spHistory, spUrl);</c></summary>
public enum TSearchPattern
{
    /// <summary>原文 :185 — <c>spAll</c>（模式 nil）</summary>
    spAll = 0,
    /// <summary>原文 :185 — <c>spCookies</c>（模式 'Cookie:'）</summary>
    spCookies = 1,
    /// <summary>原文 :185 — <c>spHistory</c>（模式 'Visited:'）</summary>
    spHistory = 2,
    /// <summary>原文 :185 — <c>spUrl</c>（模式 ''）</summary>
    spUrl = 3,
}

/// <summary>原文 :141-151 <c>TGroupInfo</c>。</summary>
public struct TGroupInfo
{
    /// <summary>原文 :143 — <c>DiskUsage:DWORD;</c></summary>
    public uint DiskUsage;
    /// <summary>原文 :145 — <c>DiskQuota:DWORD;</c></summary>
    public uint DiskQuota;
    /// <summary>原文 :147 — <c>OwnerStorage:array[0..GROUP_OWNER_STORAGE_SIZE - 1] of DWORD;</c></summary>
    public uint[] OwnerStorage;
    /// <summary>原文 :149 — <c>GroupName:string;</c></summary>
    public string GroupName;
}

/// <summary>
/// 原文 :111-139 <c>TEntryInfo</c>。
/// <para><b>注意字段顺序</b>：原文里 <c>FSize</c> 在 <c>HitRate</c> 之后、时间字段之前
/// （不是按语义分组的），本移植按原文声明顺序保留。</para>
/// </summary>
public struct TEntryInfo
{
    /// <summary>原文 :113 — <c>SourceUrlName:string;</c></summary>
    public string SourceUrlName;
    /// <summary>原文 :115 — <c>LocalFileName:string;</c></summary>
    public string LocalFileName;
    /// <summary>原文 :117 — <c>EntryType:DWORD;</c></summary>
    public uint EntryType;
    /// <summary>原文 :119 — <c>UseCount:DWORD;</c></summary>
    public uint UseCount;
    /// <summary>原文 :121 — <c>HitRate:DWORD;</c></summary>
    public uint HitRate;
    /// <summary>原文 :123 — <c>FSize:DWORD;</c>（注意原文 <c>GetEntryValues</c> 里算的是
    /// <c>(dwSizeHigh shl 32) + dwSizeLow</c>，而 DWORD 是 32 位 ⇒ 高 32 位左移后必然为 0，
    /// 这是个原文缺陷，见报告 §5）</summary>
    public uint FSize;
    /// <summary>原文 :125 — <c>LastModifiedTime:TDateTime;</c></summary>
    public double LastModifiedTime;
    /// <summary>原文 :127 — <c>ExpireTime:TDateTime;</c></summary>
    public double ExpireTime;
    /// <summary>原文 :129 — <c>LastAccessTime:TDateTime;</c></summary>
    public double LastAccessTime;
    /// <summary>原文 :131 — <c>LastSyncTime:TDateTime;</c></summary>
    public double LastSyncTime;
    /// <summary>原文 :133 — <c>HeaderInfo:string;</c></summary>
    public string HeaderInfo;
    /// <summary>原文 :135 — <c>FileExtension:string;</c></summary>
    public string FileExtension;
    /// <summary>原文 :137 — <c>ExemptDelta:DWORD;</c></summary>
    public uint ExemptDelta;
}

/// <summary>原文 :153-159 <c>TContent</c>（缓冲区 + 长度）。</summary>
public struct TContent
{
    /// <summary>原文 :155 — <c>Buffer:Pointer;</c>（托管侧为 <c>byte[]</c>）</summary>
    public byte[] Buffer;
    /// <summary>原文 :157 — <c>BufferLength:Integer;</c></summary>
    public int BufferLength;
}

/// <summary>
/// IECache.pas 的 <c>TIECache</c> 1:1 移植（WinInet 部分为接缝）。
///
/// <para><b>已 1:1 移植的纯逻辑</b>：<c>FilterOptions</c> → <c>FFilterOptionValue</c>
/// 的位值求和（原文 :1269-1282）、<c>SearchPattern</c> → 前缀模式表（原文 :1121）、
/// 构造函数里的默认过滤集（原文 :445-447）。</para>
///
/// <para><b>接缝</b>：所有调用 wininet.dll 的方法保留原签名，统一返回
/// <c>ERROR_FILE_NOT_FOUND</c>（原文"库未找到"时的取值，见原文 :476 等），
/// 并置 <see cref="LibraryFound"/> 为 false。</para>
/// </summary>
public class TIECache
{
    /// <summary>
    /// 原文 :1271-1273 — <c>acardFilterOptionValues:array[TFilterOption] of Cardinal</c>
    /// <para><c>($00000001, $00000002, $00000004, $00100000, $00200000, $00000010,
    /// $00000020, $00010000, $00020000)</c>，按 <c>TFilterOption</c> 顺序。
    /// <b>注意顺序不是位序</b>：COOKIE/URLHISTORY 取 $00100000/$00200000，
    /// 而 TRACK_OFFLINE/ONLINE 才是 $00000010/$00000020。</para>
    /// </summary>
    public static readonly uint[] FilterOptionValues =
    {
        0x00000001, // NORMAL_ENTRY
        0x00000002, // STABLE_ENTRY
        0x00000004, // STICKY_ENTRY
        0x00100000, // COOKIE_ENTRY
        0x00200000, // URLHISTORY_ENTRY
        0x00000010, // TRACK_OFFLINE_ENTRY
        0x00000020, // TRACK_ONLINE_ENTRY
        0x00010000, // SPARSE_ENTRY
        0x00020000, // OCX_ENTRY
    };

    /// <summary>
    /// 原文 :1121 — <c>Pattern:array[TSearchPattern] of PChar = (nil, 'Cookie:', 'Visited:', '');</c>
    /// <para>托管侧以 <c>null</c> 表示原文的 <c>nil</c>。</para>
    /// </summary>
    public static readonly string[] SearchPatterns = { null, "Cookie:", "Visited:", "" };

    private TFilterOption[] FFilterOptions = Array.Empty<TFilterOption>();
    private uint FFilterOptionValue;
    private TSearchPattern fSearchPattern;

    /// <summary>原文 :219 — <c>GroupInfo:TGroupInfo;</c>（public 字段）</summary>
    public TGroupInfo GroupInfo;
    /// <summary>原文 :221 — <c>EntryInfo:TEntryInfo;</c>（public 字段）</summary>
    public TEntryInfo EntryInfo;
    /// <summary>原文 :223 — <c>Content:TContent;</c>（public 字段）</summary>
    public TContent Content;

    /// <summary>
    /// 原文 :431-449 <c>constructor TIECache.Create(AOwner:TComponent);</c>
    /// <para>默认过滤集 = <c>[NORMAL_ENTRY, COOKIE_ENTRY, URLHISTORY_ENTRY,
    /// TRACK_OFFLINE_ENTRY, TRACK_ONLINE_ENTRY, STICKY_ENTRY]</c>
    /// （原文注释：Identical to URLCACHE_FIND_DEFAULT_FILTER）。</para>
    /// <para>注意原文构造函数**没有**调用 <c>UpdateFilterOptionValue</c>，
    /// 所以初值下 <c>FFilterOptionValue = 0</c> —— 只有 setter 才会算。本移植保真。</para>
    /// </summary>
    public TIECache()
    {
        Content.Buffer = null;
        ClearEntryValues();
        FFilterOptions = new[]
        {
            TFilterOption.NORMAL_ENTRY, TFilterOption.COOKIE_ENTRY, TFilterOption.URLHISTORY_ENTRY,
            TFilterOption.TRACK_OFFLINE_ENTRY, TFilterOption.TRACK_ONLINE_ENTRY, TFilterOption.STICKY_ENTRY,
        };
        // 原文如此（IECache.pas:431-449）：构造函数**不**调用 UpdateFilterOptionValue
    }

    /// <summary>原文 :275 — <c>property FilterOptions:TFilterOptions read FFilterOptions write SetFilterOptions;</c></summary>
    public TFilterOption[] FilterOptions
    {
        get => FFilterOptions;
        set
        {
            FFilterOptions = value ?? Array.Empty<TFilterOption>();
            UpdateFilterOptionValue();
        }
    }

    /// <summary>原文 :277 — <c>property SearchPattern:TSearchpattern read FSearchpattern write FSearchPattern;</c></summary>
    public TSearchPattern SearchPattern
    {
        get => fSearchPattern;
        set => fSearchPattern = value;
    }

    /// <summary>原文 :453-461 <c>getLibraryFound</c> —— 原为 <c>initializeWinInet</c> 的结果。</summary>
    public bool LibraryFound => IECacheSeam.initializeWinInet();

    /// <summary>原文 <c>FFilterOptionValue</c>（测试可见的只读视图）。</summary>
    public uint FilterOptionValue => FFilterOptionValue;

    /// <summary>
    /// 原文 :1269-1282 <c>procedure TIECache.UpdateFilterOptionValue;</c>
    /// <para><c>FFilterOptionValue := 0; if FFilterOptions &lt;&gt; [] then for i := Low..High do
    /// if i in FFilterOptions then Inc(FFilterOptionValue, acardFilterOptionValues[i]);</c></para>
    /// <para><b>差异断言要点</b>：是**加法**（Inc）而不是按位或 —— 因为每一项的位互不重叠，
    /// 结果与 <c>or</c> 相同；但重复项会被重复累加（本移植的原生列表允许重复，故保真用加法）。</para>
    /// </summary>
    public void UpdateFilterOptionValue()
    {
        FFilterOptionValue = 0;
        if (FFilterOptions != null && FFilterOptions.Length > 0)
        {
            for (int i = 0; i <= (int)TFilterOption.OCX_ENTRY; i++)
            {
                if (Array.IndexOf(FFilterOptions, (TFilterOption)i) >= 0)
                {
                    FFilterOptionValue += FilterOptionValues[i];
                }
            }
        }
    }

    /// <summary>
    /// 原文 :1271-1282 的**纯函数**版本（不改实例状态，供直接单测）。
    /// </summary>
    public static uint ComputeFilterOptionValue(IEnumerable<TFilterOption> options)
    {
        uint result = 0;
        if (options == null) return result;
        var set = new HashSet<TFilterOption>(options);
        if (set.Count == 0) return result;
        for (int i = 0; i <= (int)TFilterOption.OCX_ENTRY; i++)
        {
            if (set.Contains((TFilterOption)i))
            {
                result += FilterOptionValues[i];
            }
        }
        return result;
    }

    /// <summary>
    /// 原文 :1121 + 原文 :1146 的 <c>Pattern[SearchPattern]</c> 取值（纯逻辑，供测试）。
    /// </summary>
    public static string SearchPatternOf(TSearchPattern p)
    {
        int i = (int)p;
        if (i < 0 || i >= SearchPatterns.Length) return null;
        return SearchPatterns[i];
    }

    /// <summary>
    /// 原文 :1027-1069 <c>procedure TIECache.ClearEntryValues;</c>
    /// <para>把 <c>Content</c> 与 <c>EntryInfo</c> 的全部字段复位。原文首行有
    /// <c>if not initializeWinInet then Exit;</c>（:1033）—— 即在库不可用时**什么都不清**。
    /// 本移植保留该短路（用 <see cref="IECacheSeam.initializeWinInet"/> 判定），
    /// 但额外提供 <paramref name="force"/> 供构造期使用（原文构造期调用它时库还没探测过）。</para>
    /// </summary>
    public void ClearEntryValues(bool force = true)
    {
        if (!force && !IECacheSeam.initializeWinInet()) return;

        Content.Buffer = null;
        Content.BufferLength = 0;

        EntryInfo.SourceUrlName = string.Empty;
        EntryInfo.LocalFileName = string.Empty;
        EntryInfo.EntryType = 0;
        EntryInfo.UseCount = 0;
        EntryInfo.HitRate = 0;
        EntryInfo.LastModifiedTime = 0;
        EntryInfo.ExpireTime = 0;
        EntryInfo.LastAccessTime = 0;
        EntryInfo.LastSyncTime = 0;
        EntryInfo.FileExtension = string.Empty;
        EntryInfo.FSize = 0;
        EntryInfo.HeaderInfo = string.Empty;
        EntryInfo.ExemptDelta = 0;
    }

    /// <summary>
    /// 原文 :1073-1112 <c>procedure TIECache.GetEntryValues(Info:PInternetCacheEntryInfo);</c>
    /// 的**字段映射部分**（已抽成可测的纯逻辑重载）。
    /// <para>原文 <c>FSize := (info^.dwSizeHigh shl 32) + info^.dwSizeLow;</c> 里
    /// <c>dwSizeHigh</c> 是 DWORD（32 位），<c>shl 32</c> 在 Delphi 32 位里结果恒为 0
    /// —— 于是 <c>FSize</c> **只等于 dwSizeLow**，>4GB 的缓存条目会被错报。
    /// 本移植在 <paramref name="sizeHigh"/> 重载里保留**与原文字节级一致**的行为
    /// （忽略高位），并单独登记该缺陷。</para>
    /// </summary>
    public void GetEntryValues(string sourceUrlName, string localFileName, uint cacheEntryType,
        uint dwUseCount, uint dwHitRate, uint dwSizeLow, double lastModified, double expire,
        double lastAccess, double lastSync, string fileExtension, string headerInfo,
        uint dwReserved)
    {
        EntryInfo.SourceUrlName = sourceUrlName;
        EntryInfo.LocalFileName = localFileName;
        EntryInfo.EntryType = cacheEntryType;
        EntryInfo.UseCount = dwUseCount;
        EntryInfo.HitRate = dwHitRate;
        EntryInfo.LastModifiedTime = lastModified;
        EntryInfo.ExpireTime = expire;
        EntryInfo.LastAccessTime = lastAccess;
        EntryInfo.LastSyncTime = lastSync;
        EntryInfo.FileExtension = fileExtension;
        // 原文如此（IECache.pas:1104）：(info^.dwSizeHigh shl 32) + info^.dwSizeLow
        EntryInfo.FSize = dwSizeLow;
        EntryInfo.HeaderInfo = headerInfo;
        EntryInfo.ExemptDelta = dwReserved;
    }

    // ── 以下为 WinInet 接缝（原签名保留，统一走 IECacheSeam）────────────────

    /// <summary>原文 :465-486 <c>RemoveUrlFromGroup</c></summary>
    public uint RemoveUrlFromGroup(long GroupID, string Url) => IECacheSeam.NotAvailable();

    /// <summary>原文 :488-507 <c>AddUrlToGroup</c></summary>
    public uint AddUrlToGroup(long GroupID, string Url) => IECacheSeam.NotAvailable();

    /// <summary>原文 :511-541 <c>CopyFileToCache</c></summary>
    public uint CopyFileToCache(string Url, string FileName, uint CacheType, double Expire)
        => IECacheSeam.NotAvailable();

    /// <summary>原文 :545-559 <c>CreateEntry</c>（FName 是 var 参数）</summary>
    public uint CreateEntry(string Url, string FileExtension, uint ExpectedFileSize, out string FName)
    {
        FName = null;
        return IECacheSeam.NotAvailable();
    }

    /// <summary>原文 :563-603 <c>GetGroupInfo</c></summary>
    public uint GetGroupInfo(long GroupID) => IECacheSeam.NotAvailable();

    /// <summary>原文 :607-644 <c>SetGroupInfo</c></summary>
    public uint SetGroupInfo(long GroupID) => IECacheSeam.NotAvailable();

    /// <summary>原文 :648-665 <c>CreateGroup</c></summary>
    public long CreateGroup() => 0;

    /// <summary>原文 :669-690 <c>DeleteGroup</c></summary>
    public uint DeleteGroup(long GroupID) => IECacheSeam.NotAvailable();

    /// <summary>原文 :694-753 <c>SetEntryInfo</c></summary>
    public uint SetEntryInfo(string Url) => IECacheSeam.NotAvailable();

    /// <summary>原文 :757-799 <c>GetEntryInfo</c></summary>
    public uint GetEntryInfo(string Url) => IECacheSeam.NotAvailable();

    /// <summary>原文 :803-864 <c>GetEntryContent</c></summary>
    public uint GetEntryContent(string Url) => IECacheSeam.NotAvailable();

    /// <summary>原文 :868-887 <c>FindNextGroup</c></summary>
    public bool FindNextGroup(ref long GroupID) => false;

    /// <summary>原文 :891-916 <c>FindFirstGroup</c></summary>
    public uint FindFirstGroup(ref long GroupID) => IECacheSeam.NotAvailable();

    /// <summary>原文 :920-972 <c>RetrieveGroups</c></summary>
    public uint RetrieveGroups() => IECacheSeam.NotAvailable();

    /// <summary>原文 :976-993 <c>DeleteEntry</c></summary>
    public uint DeleteEntry(string Url) => IECacheSeam.NotAvailable();

    /// <summary>原文 :997-1023 <c>ClearAllEntries</c></summary>
    public void ClearAllEntries() { /* 接缝：见 IECacheSeam */ }

    /// <summary>原文 :1115-1165 <c>FindFirstEntry</c></summary>
    public uint FindFirstEntry(long GroupID) => IECacheSeam.NotAvailable();

    /// <summary>原文 :1169-1212 <c>FindNextEntry</c></summary>
    public uint FindNextEntry() => IECacheSeam.NotAvailable();

    /// <summary>原文 :1250-1257 <c>CloseFindEntry</c></summary>
    public bool CloseFindEntry() => false;

    /// <summary>原文 :1216-1246 <c>RetrieveEntries</c>（原文用 FOnEntry 事件回调）</summary>
    public void RetrieveEntries(long GroupID) { /* 接缝：见 IECacheSeam */ }

    /// <summary>原文 :289-1291 <c>procedure Register;</c>（VCL 设计期注册，托管侧无对应）</summary>
    public static void Register() { /* 原文：RegisterComponents('Internet', [TIECache]); —— 托管侧无设计期注册 */ }
}

/// <summary>
/// IECache 的 WinInet 接缝。
/// <para>托管侧**不**加载 wininet.dll：.NET 的 <c>System.Net.Http.HttpClient</c> +
/// <c>CookieContainer</c> + <c>HttpClientHandler</c> 覆盖了 IE 缓存组件的实际用途
/// （Cookie 管理 / 缓存条目枚举），而 WinINet 的缓存 API 没有托管投影。</para>
/// </summary>
public static class IECacheSeam
{
    /// <summary>原文在"库未找到"时返回的错误码（如 :476 <c>Result := ERROR_FILE_NOT_FOUND;</c>）。</summary>
    public const uint ERROR_FILE_NOT_FOUND = 2;

    /// <summary>原文 <c>S_OK</c>（Windows 单元定义）。</summary>
    public const uint S_OK = 0;

    /// <summary>原文 <c>ERROR_NO_MORE_ITEMS</c>（Windows 单元定义；:1017 用它作循环终止）。</summary>
    public const uint ERROR_NO_MORE_ITEMS = 259;

    /// <summary>
    /// 原文 :343-377 <c>function initializeWinInet:boolean;</c>
    /// <para>原文逐个 <c>LoadLibrary + GetProcAddress</c> 检查 5 个入口
    /// （FindFirstUrlCacheGroup / FindNextUrlCacheGroup / GetUrlCacheGroupAttributeA /
    /// SetUrlCacheGroupAttributeA / FindFirstUrlCacheEntryExA），**缺一即 false**
    /// 并缓存到 <c>winInetLibFound</c>。</para>
    /// <para>托管侧不加载 wininet，故恒返回 false（原文"库不可用"分支）。</para>
    /// </summary>
    public static bool initializeWinInet() => false;

    /// <summary>接缝统一返回：原文"库未找到"取值。</summary>
    public static uint NotAvailable() => ERROR_FILE_NOT_FOUND;

    /// <summary>
    /// 原文 wininet.dll 入口 → 托管等价物。
    /// <para>每行 <c>(原文入口, 托管等价, 备注)</c>。<b>本表是文档而非代码。</b></para>
    /// </summary>
    public static readonly (string WinInet, string Managed, string Note)[] FunctionMap = new[]
    {
        ("FindFirstUrlCacheGroup",     "（无托管等价）", "CookieContainer 无「缓存组」概念"),
        ("FindNextUrlCacheGroup",      "（无托管等价）", ""),
        ("GetUrlCacheGroupAttributeA", "（无托管等价）", ""),
        ("SetUrlCacheGroupAttributeA", "（无托管等价）", ""),
        ("FindFirstUrlCacheEntryExA",  "（无托管等价）", "HttpClientHandler 不暴露磁盘缓存"),
        ("FindNextUrlCacheEntryEx",    "（无托管等价）", ""),
        ("CreateUrlCacheGroup",        "（无托管等价）", ""),
        ("DeleteUrlCacheGroup",        "（无托管等价）", ""),
        ("SetUrlCacheEntryGroup",      "（无托管等价）", ""),
        ("CreateUrlCacheEntry",        "（无托管等价）", "缓存文件落盘由 HttpClientHandler 内部完成"),
        ("CommitUrlCacheEntry",        "（无托管等价）", ""),
        ("SetUrlCacheEntryInfo",       "（无托管等价）", ""),
        ("GetUrlCacheEntryInfoEx",     "（无托管等价）", ""),
        ("DeleteUrlCacheEntry",        "（无托管等价）", ""),
        ("RetrieveUrlCacheEntryStream", "（无托管等价）", ""),
        ("ReadUrlCacheEntryStream",    "（无托管等价）", ""),
        ("UnLockUrlCacheEntryStream",  "（无托管等价）", ""),
        ("FindCloseUrlCache",          "（无托管等价）", ""),
        ("FileTimeToLocalFiletime",    "DateTime.ToLocalTime",   "FILETIME↔DateTime 换算"),
        ("FileTimeToSystemTime",       "DateTime 构造",           ""),
        ("SystemTimeToFileTime",       "DateTime 构造",           ""),
        ("LocalFileTimeToFileTime",    "DateTime.ToUniversalTime", ""),
        ("GetLastError",               "Marshal.GetLastWin32Error / SocketException.ErrorCode", ""),
    };

    /// <summary>原文 :335 — <c>winetdll = 'wininet.dll';</c>（登记用；托管侧不加载）</summary>
    public const string winetdll = "wininet.dll";
}
