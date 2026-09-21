using System;
using System.Collections.Generic;
using System.Linq;

// 源：Source/RunGate/Common/IODataPool.pas         （586 物理 LF）
//     Source/RunGate/Common/IocpTcpClient.pas      （340 物理 LF）
//     Source/RunGate/Common/Qos.pas                （293 物理 LF）
//     Source/RunGate/DllUpdateCommon.pas           （ 10 物理 LF）
// ============================================================================
// 车道 p9-rungate-rest 的裁定：**四个单元全部「不移植 + 证据」**，不新建任何
// 第二份实现（遵守台账 §14.2「不造第三份实现」与 §18.5 第 6 条的处置口径）。
//
// 口径（§18.5 第 6 条）：
//   被 .NET / GatewayKit 等价取代者 → 登记为「不移植 + 证据」；
//   否则 → 1:1 移植。
//
// 本文件**不含任何运行时算法**，只含「证据数据 + 纯判定函数」：
//   * 证据数据 = 从原文逐条抽取的成员/常量清单（抽取命令见各表注释）；
//   * 判定函数 = 把「为什么可以不移植」写成可断言的形式。
// 真正的取证在 tests/GXX.RunGate.Tests/Rest9NotPortedEvidenceTests.cs：
// 那些用例**直接读仓库里的 Delphi 原文**逐条重算，与这里的数据双向比对
// （不是 C# 自证），并锁定「C# 侧 0 命中」这一否定性断言（§37.3 计数取证）。
// ============================================================================

namespace GXX.RunGate.Rest9;

/// <summary>
/// 四个 RunGate/IOCP 单元「不移植」的可断言证据。
/// </summary>
public static class Rest9NotPortedEvidence
{
    /// <summary>本次裁定的四个源单元（相对仓库根的路径，一律用 '/' 分隔）。</summary>
    public static readonly IReadOnlyList<string> SourceUnits = new[]
    {
        "Source/RunGate/Common/IODataPool.pas",
        "Source/RunGate/Common/IocpTcpClient.pas",
        "Source/RunGate/Common/Qos.pas",
        "Source/RunGate/DllUpdateCommon.pas",
    };

    /// <summary>四个源单元的物理 LF 行数（实测口径，见并行报告 §0）。</summary>
    public static readonly IReadOnlyDictionary<string, int> PhysicalLineCounts =
        new Dictionary<string, int>
        {
            ["Source/RunGate/Common/IODataPool.pas"] = 586,
            ["Source/RunGate/Common/IocpTcpClient.pas"] = 340,
            ["Source/RunGate/Common/Qos.pas"] = 293,
            ["Source/RunGate/DllUpdateCommon.pas"] = 10,
        };

    // ------------------------------------------------------------------
    // 1) IODataPool.pas —— 被 GatewayKit 的托管 SAEA 链路取代
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>TIODataPool</c> 的全部实现体（11 个，按原文行号升序）。
    /// 抽取：<c>Select-String -Path Source\RunGate\Common\IODataPool.pas
    /// -Pattern '^(constructor|destructor|procedure|function|class function)\s+TIODataPool'</c>
    /// </summary>
    public static readonly IReadOnlyList<string> IODataPoolMembers = new[]
    {
        "constructor TIODataPool.Create",              // :82
        "destructor TIODataPool.Destroy",              // :174
        "procedure TIODataPool.Clear",                 // :200
        "function TIODataPool.GetNewIOData",           // :266
        "function TIODataPool.GetCount",               // :368
        "function TIODataPool.GetUseCount",            // :373
        "function TIODataPool.GetNoUseCount",          // :378
        "function TIODataPool.GiveBackIOData",         // :383
        "function TIODataPool.GetNewSendBufer",        // :489（原文整段在 { } 内 = 死代码）
        "function TIODataPool.GiveBackSendBuffer",     // :536（原文整段在 { } 内 = 死代码）
        "class function TIODataPool.Instance",         // :567（{$IFDEF SHARE_POOL_MODE}，iocp.inc 已定义）
    };

    /// <summary>原文 :489-564 两个 SendBuffer 成员整段处于 <c>{ }</c> 注释内 ⇒ 不是活代码。</summary>
    public const int IODataPoolCommentedOutMembers = 2;

    /// <summary>
    /// <c>TIODataPool</c> 里 <c>GetMem</c>/<c>FreeMem</c> 的出现处数——
    /// 即该池是**非托管内存池**的直接证据（含上面两个注释掉的成员）。
    /// 抽取：<c>git grep -n -I -E "\b(GetMem|FreeMem)\s*\(" -- Source/RunGate</c>。
    /// </summary>
    public const int IODataPoolUnmanagedAllocSites = 14;

    /// <summary>原文 :107/:112/:138/:217 等：池里存的是 <c>POVERLAPPEDEx</c> 原始指针。</summary>
    public const string IODataPoolPayloadType = "POVERLAPPEDEx";

    /// <summary>原文 :96 —— <c>Count := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10 shr 7 + 1;</c> = 5*1024/128+1 = 41 个尺寸档。</summary>
    public const int IODataPoolBucketCount = 41;

    /// <summary>原文 :105 —— 每档预分配 <c>MAX_PREALLOCATED_MEMORY_SIZE</c> = 1024 块。</summary>
    public const int IODataPoolPreallocPerBucket = 1024;

    /// <summary>
    /// IODataPool 在 RunGate 侧的**全部**引用点（单元名出现处）。
    /// 抽取：<c>git grep -n -I -w IODataPool -- Source/RunGate</c>。
    /// 注意：全部引用者本身已裁定为「IOCP 管道本体不移植」（见并行报告-p2-rungate-impl.md §2.2 第 20/21 行）。
    /// </summary>
    public static readonly IReadOnlyList<string> IODataPoolCalledFrom = new[]
    {
        "Source/RunGate/Common/IocpUtils.pas",       // uses（:9）+ TIODataPool.Instance/GetNewIOData（:370/389/447/466）
        "Source/RunGate/Common/IocpTcpServer.pas",   // uses（:9）+ FIODataPool: TIODataPool（:142/609/649）
        "Source/RunGate/Common/IocpTcpClient.pas",   // uses（:9）+ TIODataPool.Instance.GetNewIOData(0)（:215/217）
        "Source/RunGate/MirClientContext.pas",       // uses（:9）
        "Source/RunGate/uFrmMain.pas",               // uses（:18）+ 6 个统计标签（:2240/2241/2243/2244/2245）
        "Source/RunGate/Common/IODataPool.pas",      // 单元自身（:1 unit 名 + 单元内 10 处 TIODataPool）
    };

    /// <summary>
    /// 唯一一处**非** IOCP 管道内的使用：主窗体把池的统计量显示在 6 个标签上
    /// （<c>uFrmMain.pas:2240-2245</c>）。这是「已移植但零生产调用方」之外的另一类缺口，
    /// 一并登记，供主窗体车道决定是否用 GatewayKit 的计数器复刻。
    /// </summary>
    public static readonly IReadOnlyList<string> IODataPoolUiStatLabels = new[]
    {
        "Source/RunGate/uFrmMain.pas:2240  lblIODataUseCount.Caption := IntToStr(TIODataPool.Instance.UseCount);",
        "Source/RunGate/uFrmMain.pas:2241  lblIODataNoUseCount.Caption := IntToStr(TIODataPool.Instance.NoUseCount);",
        "Source/RunGate/uFrmMain.pas:2243  lblIODataMaxUseCount.Caption := IntToStr(TIODataPool.Instance.MaxUseCount);",
        "Source/RunGate/uFrmMain.pas:2244  lblIODataMaxMemCount.Caption := GetSizeString(TIODataPool.Instance.MaxAllocMemSize);",
        "Source/RunGate/uFrmMain.pas:2245  lblIODataMemCount.Caption := GetSizeString(TIODataPool.Instance.AllocMemSize);",
    };

    /// <summary>
    /// 托管侧不需要该池的机制性理由（可断言：GatewayKit 公开 API 里没有等价物）。
    /// </summary>
    public static readonly IReadOnlyList<string> IODataPoolManagedReplacement = new[]
    {
        "GXX.GatewayKit.IocpManager",   // GatewayProtocol.cs:136 —— 每连接固定 DATA_BUFSIZE 缓冲，无尺寸档池
        "GXX.GatewayKit.GateSession",   // GatewayProtocol.cs:84  —— Buffer/EnqueueSend，缓冲随会话由 GC 回收
        "GXX.GatewayKit.TcpLink",       // TcpLink.cs:15         —— 主动连接侧，_recvBuffer/_accum 定长数组
    };

    /// <summary>托管替代设施的公开成员清单（用于断言「没有尺寸档池 API 可供复用」）。</summary>
    public static readonly IReadOnlyList<string> ManagedGatewayKitPublicApi = new[]
    {
        "IocpManager.Start", "IocpManager.StartListen", "IocpManager.Stop", "IocpManager.Send",
        "IocpManager.CloseSession", "IocpManager.Dispose", "IocpManager.SessionCount",
        "IocpManager.GetSession", "IocpManager.OnAccept", "IocpManager.OnReceive", "IocpManager.OnDisconnect",
        "GateSession.EnqueueSend", "GateSession.TryDequeueSend", "GateSession.AppendBuffer",
        "GateSession.ResetBuffer", "GateSession.PendingSendCount",
        "TcpLink.Connect", "TcpLink.Send", "TcpLink.Accumulate", "TcpLink.ConsumeAccum",
        "TcpLink.Close", "TcpLink.Dispose", "TcpLink.StartReceiveLoop",
    };

    // ------------------------------------------------------------------
    // 2) IocpTcpClient.pas —— 被 GatewayKit.TcpLink 取代
    // ------------------------------------------------------------------

    /// <summary>
    /// <c>TIocpRemoteContext</c> + <c>TIocpTcpClient</c> 的全部实现体（19 个，按原文行号升序）。
    /// 抽取：<c>Select-String -Path Source\RunGate\Common\IocpTcpClient.pas
    /// -Pattern '^(constructor|destructor|procedure|function)\s+T'</c>
    /// </summary>
    public static readonly IReadOnlyList<string> IocpTcpClientMembers = new[]
    {
        "constructor TIocpRemoteContext.Create",                       // :88
        "destructor TIocpRemoteContext.Destroy",                       // :94
        "procedure TIocpRemoteContext.CloseContextSocket",             // :100
        "procedure TIocpRemoteContext.DoConnect",                      // :105
        "procedure TIocpRemoteContext.DoDisconnect",                   // :111
        "procedure TIocpRemoteContext.DoReset",                        // :117
        "procedure TIocpRemoteContext.SetActive",                      // :123
        "constructor TIocpTcpClient.Create",                           // :241
        "destructor TIocpTcpClient.Destroy",                           // :254
        "procedure TIocpTcpClient.ClearContexts",                      // :265
        "function TIocpTcpClient.Add",                                 // :276
        "function TIocpTcpClient.GetCount",                            // :291
        "function TIocpTcpClient.GetItems",                            // :296
        "procedure TIocpTcpClient.SetOnContextConnect",                // :304
        "procedure TIocpTcpClient.SetOnContextDisconnect",             // :310
        "procedure TIocpTcpClient.SetOnError",                         // :316
        "procedure TIocpTcpClient.SetOnRecvDataBuffer",                // :322
        "procedure TIocpTcpClient.SetOnSendDataBuffer",                // :328
        "procedure TIocpTcpClient.RegisterContextClass",               // :334
    };

    /// <summary>
    /// <c>TIocpTcpClient</c> 的活调用方（唯一一个）。
    /// 抽取：<c>git grep -n -I -E "FIocpClient|RegisterContextClass" -- Source/RunGate</c>。
    /// </summary>
    public static readonly IReadOnlyList<string> IocpTcpClientCalledFrom = new[]
    {
        "Source/RunGate/RunGateUtils.pas:3509  FIocpClient := TIocpTcpClient.Create(nil);",
        "Source/RunGate/RunGateUtils.pas:3510  FIocpClient.RegisterContextClass(TMirRemoteContext);",
        "Source/RunGate/RunGateUtils.pas:1422  FTcpClient := TMirRemoteContext(AOnwer.FIocpClient.Add);",
        "Source/RunGate/RunGateUtils.pas:3621  for I := 0 to FIocpClient.Count - 1 do",
        "Source/RunGate/RunGateUtils.pas:3623  TIocpRemoteContext(FIocpClient.Items[I]).Active := False;",
        "Source/RunGate/RunGateUtils.pas:1502  FTcpClient.Active := True;   // -> SetActive",
    };

    /// <summary>
    /// 原文 <c>SetActive</c> 里用的 Win32 机械（:125-137/:155-229）——
    /// 这些是「整条自研 IOCP 栈」的入口，不是可孤立移植的算法。
    /// 括号内为**整个单元**的实测出现次数（本单元的其它部分都不碰这些 API）。
    /// </summary>
    public static readonly IReadOnlyDictionary<string, int> SetActiveWin32Mechanics =
        new Dictionary<string, int>
        {
            ["WSASocket"] = 2,
            ["Bind"] = 2,
            ["CreateIoCompletionPort"] = 1,
            ["SIO_GET_EXTENSION_FUNCTION_POINTER"] = 1,
            ["WSAID_CONNECTEX"] = 3,
            ["inet_addr"] = 1,
            ["htons"] = 2,
            ["htonl"] = 2,
            ["IocpConnectEx"] = 6,
        };

    // ------------------------------------------------------------------
    // 3) Qos.pas —— 第三方头文件翻译（JEDI/Microsoft qos.h），零可执行语句
    // ------------------------------------------------------------------

    /// <summary><c>{$EXTERNALSYM ...}</c> 条数 = 40，恰好等于「26 个常量的偶 + 14 个类型的偶」。</summary>
    public const int QosExternalsymCount = 40;

    /// <summary>Qos.pas <c>const</c> 段的 26 个常量名（抽取见测试）。</summary>
    public static readonly IReadOnlyList<string> QosConstantNames = new[]
    {
        "SERVICETYPE_NOTRAFFIC", "SERVICETYPE_BESTEFFORT", "SERVICETYPE_CONTROLLEDLOAD",
        "SERVICETYPE_GUARANTEED", "SERVICETYPE_NETWORK_UNAVAILABLE", "SERVICETYPE_GENERAL_INFORMATION",
        "SERVICETYPE_NOCHANGE", "SERVICETYPE_NONCONFORMING", "SERVICETYPE_NETWORK_CONTROL",
        "SERVICETYPE_QUALITATIVE",
        "SERVICE_BESTEFFORT", "SERVICE_CONTROLLEDLOAD", "SERVICE_GUARANTEED", "SERVICE_QUALITATIVE",
        "SERVICE_NO_TRAFFIC_CONTROL", "SERVICE_NO_QOS_SIGNALING",
        "QOS_NOT_SPECIFIED", "POSITIVE_INFINITY_RATE",
        "QOS_GENERAL_ID_BASE", "QOS_OBJECT_END_OF_LIST", "QOS_OBJECT_SD_MODE",
        "QOS_OBJECT_SHAPING_RATE", "QOS_OBJECT_DESTADDR",
        "TC_NONCONF_BORROW", "TC_NONCONF_SHAPE", "TC_NONCONF_DISCARD",
    };

    /// <summary><c>TC_NONCONF_BORROW_PLUS = 3;</c>（原文 :268，「not supported currently」）也是常量，共 27 条。</summary>
    public const int QosConstantCount = 27;

    /// <summary>原文 <c>type</c> 段声明/别名的 22 个名字。</summary>
    public static readonly IReadOnlyList<string> QosTypeNames = new[]
    {
        "SERVICETYPE", "TServiceType", "PServiceType",
        "_flowspec", "FLOWSPEC", "PFLOWSPEC", "LPFLOWSPEC", "TFlowSpec",
        "QOS_OBJECT_HDR", "LPQOS_OBJECT_HDR", "TQOSObjectHdr", "PQOSObjectHdr",
        "_QOS_SD_MODE", "QOS_SD_MODE", "LPQOS_SD_MODE", "TQOSSDMode", "PQOSSDMode",
        "_QOS_SHAPING_RATE", "QOS_SHAPING_RATE", "LPQOS_SHAPING_RATE",
        "TQOSShapingRate", "PQOSShapingRate",
    };

    public const string QosTCNonconfBorrowPlus = "TC_NONCONF_BORROW_PLUS";

    /// <summary>原文 <c>type</c> 段的 5 处（:75/:145/:199/:248/:278），<c>const</c> 段 5 处。</summary>
    public const int QosTypeSectionCount = 5;

    /// <summary>原文 <c>const</c> 段处数。</summary>
    public const int QosConstSectionCount = 5;

    /// <summary>
    /// Qos.pas 在整棵树里的**唯一**引用点。
    /// 抽取：<c>git grep -n -I -E "Qos\s*(,|;|in )" -- "*.pas" "*.dpr" "*.dproj"</c>。
    /// </summary>
    public static readonly IReadOnlyList<string> QosCalledFrom = new[]
    {
        "Source/RunGate/Common/IocpWinsock2.pas:79  Windows, Qos;",
    };

    /// <summary>
    /// ★ 引用方 <c>IocpWinsock2.pas</c> **真正用到**的 Qos 符号只有两个：
    /// <c>FLOWSPEC</c>（7 处，:1603 注释 1 + :1620/:1621 两个字段 + 别处）与
    /// <c>SERVICETYPE</c>（2 处，其中 :1411 是 <c>WSA_QOS_ESERVICETYPE</c> 的一部分）。
    /// 其余 38 个符号在该单元里 **0 命中**（下述清单已按实测校正——初版曾错记
    /// 为「0 命中」，被本车道的取证用例当场否掉，见并行报告 §4 自我纠错）。
    /// </summary>
    public static readonly IReadOnlyList<string> QosSymbolsActuallyUsedByIocpWinsock2 = new[]
    {
        "FLOWSPEC",     // 7
        "SERVICETYPE",  // 2（子串命中 WSA_QOS_ESERVICETYPE）
    };

    /// <summary>上述两个符号在 IocpWinsock2.pas 里的出现次数（逐条实测）。</summary>
    public static readonly IReadOnlyDictionary<string, int> QosSymbolUseCountsInIocpWinsock2 =
        new Dictionary<string, int>
        {
            ["FLOWSPEC"] = 7,
            ["SERVICETYPE"] = 2,
        };

    /// <summary>
    /// 未被引用方动用的相关符号（在 IocpWinsock2.pas 里必须 0 命中）——
    /// 即 Qos.pas 的「QOS 对象族 / 整形（shaping）族」整族都没有下游。
    /// </summary>
    public static readonly IReadOnlyList<string> QosSymbolsAbsentFromIocpWinsock2 = new[]
    {
        "QOS_OBJECT_HDR", "QOS_SD_MODE", "QOS_SHAPING_RATE", "TC_NONCONF_",
        "QOS_GENERAL_ID_BASE", "QOS_NOT_SPECIFIED", "POSITIVE_INFINITY_RATE",
        "TFlowSpec", "TServiceType", "PServiceType",
    };

    /// <summary>
    /// C# 侧**已存在**的 Qos 常量（别名复用，不重复造）：来自另一份同源翻译
    /// <c>Source/Client-HGE/WinSock2.pas</c> 的移植物。
    /// </summary>
    public const string QosAlreadyPortedInCSharpWhere =
        "GXX.CSharp/src/GXX.Client/Tail/WinSock2Constants.cs";

    /// <summary>已在 <see cref="QosAlreadyPortedInCSharpWhere"/> 落地的 8 条 SERVICETYPE_* 常量。</summary>
    public static readonly IReadOnlyList<string> QosAlreadyPortedInCSharp = new[]
    {
        "SERVICETYPE_NOTRAFFIC", "SERVICETYPE_BESTEFFORT", "SERVICETYPE_CONTROLLEDLOAD",
        "SERVICETYPE_GUARANTEED", "SERVICETYPE_NETWORK_UNAVAILABLE",
        "SERVICETYPE_GENERAL_INFORMATION", "SERVICETYPE_NOCHANGE",
    };

    /// <summary>Qos.pas 独有、C# 侧 0 命中的 32 条符号（40 − 8 已落地）。</summary>
    public const int QosSymbolsWithZeroCSharpHits = 32;

    // ------------------------------------------------------------------
    // 4) DllUpdateCommon.pas —— 空壳孤儿单元
    // ------------------------------------------------------------------

    /// <summary>原文里 <c>interface</c> 与 <c>implementation</c> 之间的字节数（含全部空白）。</summary>
    public const int DllUpdateCommonInterfaceSectionLength = 0;

    /// <summary>原文声明的类型/常量/变量/函数/过程数（必须为 0）。</summary>
    public const int DllUpdateCommonDeclarationCount = 0;

    /// <summary>
    /// 引用了 DllUpdateCommon 的 <c>*.dpr</c>/<c>*.dproj</c>/<c>*.dpk</c> 数（必须为 0）。
    /// 抽取：<c>git grep -n -I -E "DllUpdateCommon" -- "*.dpr" "*.dproj" "*.dpk"</c> → 0 命中。
    /// </summary>
    public const int DllUpdateCommonProjectReferences = 0;

    /// <summary>
    /// 把 <c>uses</c> 子句里的单元名列表与某个单元名做**精确**匹配（Delphi 的 uses 可跨行、逗号分隔）。
    /// 仅用于证据判定，不参与任何生产逻辑。
    /// </summary>
    public static bool UsesClauseContainsUnit(string usesClause, string unitName)
    {
        if (string.IsNullOrEmpty(usesClause) || string.IsNullOrEmpty(unitName)) return false;
        return usesClause
            .Split(new[] { ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Any(t => string.Equals(t, unitName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 「块注释内成员」判定：把源文本按 <c>{ }</c> / <c>(* *)</c> 注释剥离后，
    /// 再看某个方法签名是否仍出现。用于把 IODataPool 的 2 个 SendBuffer 成员
    /// 与 Qos 的注释块剥离干净。
    /// </summary>
    public static string StripPascalComments(string source)
    {
        if (source == null) return string.Empty;
        var sb = new System.Text.StringBuilder(source.Length);
        int i = 0;
        while (i < source.Length)
        {
            if (source[i] == '{')
            {
                int end = source.IndexOf('}', i + 1);
                i = end < 0 ? source.Length : end + 1;
                continue;
            }
            if (source[i] == '(' && i + 1 < source.Length && source[i + 1] == '*')
            {
                int end = source.IndexOf("*)", i + 2, StringComparison.Ordinal);
                i = end < 0 ? source.Length : end + 2;
                continue;
            }
            if (source[i] == '/' && i + 1 < source.Length && source[i + 1] == '/')
            {
                int end = source.IndexOf('\n', i + 1);
                i = end < 0 ? source.Length : end + 1;
                continue;
            }
            sb.Append(source[i]);
            i++;
        }
        return sb.ToString();
    }
}
