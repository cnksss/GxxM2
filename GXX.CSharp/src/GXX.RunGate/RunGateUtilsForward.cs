using System;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

// 源：Source/RunGate/RunGateUtils.pas:926-1365 —— TMirRemoteContext.DoForwardToClientData
//     （M2 → 客户端方向的消息分类/改写；同一逻辑在 TRunGate.DoForwardToClientData 2785-3218 有一份
//      仅编译期开关不同的孪生副本，属 UseIocpClient=0 死分支）
// 只抽分类与纯改写；Context/socket 留接缝。

namespace GXX.RunGate;

/// <summary>RunGateUtils.pas:926-1365 的 else-if 链落到哪一支。</summary>
public enum RunGateForwardKind
{
    /// <summary>原 944：<c>BufferLen &lt; SizeOf(TDefaultMessage)</c> → 整段丢弃。</summary>
    TooShort = 0,
    /// <summary>原 949-970：SM_CHECK_RUNGATE1，自己应答后 Exit，不转发。</summary>
    CheckRunGate1,
    /// <summary>原 996-1001：SM_LOGON，记录 nRecogId 并清两个标志，**随后继续转发**。</summary>
    Logon,
    /// <summary>原 1011-1018：SM_CHANGESPEED 且 Recog 匹配，写三速，**随后继续转发**。</summary>
    ChangeSpeed,
    /// <summary>原 1020-1025：SM_ABILITY，写 btJob（&gt;2 归 0）。</summary>
    Ability,
    /// <summary>原 1027-1032：SM_HEROABILITY，写 btHeroJob（&gt;2 归 0）。</summary>
    HeroAbility,
    /// <summary>原 1034-1054：SM_CHANGEMAP。</summary>
    ChangeMap,
    /// <summary>原 1056-1092：SM_NEWMAP。</summary>
    NewMap,
    /// <summary>原 1095-1174：14 类大配置包，缓存进网关后回执 GM_DATA_CACHE。</summary>
    CacheStore,
    /// <summary>原 1175-1231：cache ident 命中，网关直发缓存并 Exit。</summary>
    CacheReply,
    /// <summary>原 1234-1264：SM_BLACKMODULEMD5（反外挂 + 进程黑名单 + 速度间隔）。</summary>
    BlackModuleMd5,
    /// <summary>原 1267-1270：SM_SENDNOTICE —— **空分支**，不 Exit，最终仍会转发。</summary>
    SendNotice,
    /// <summary>原 1273-1324：带数据时的魔法/包裹/物品子链。</summary>
    ItemFamilyWithData,
    /// <summary>原 1325-1329：SM_EAT_OK / SM_AUTOEAT_OK。</summary>
    EatOk,
    /// <summary>原 1330-1334：SM_HEROEAT_OK / SM_HEROAUTOEAT_OK。</summary>
    HeroEatOk,
    /// <summary>原 1336-1340：SM_MASTERBAGTOHEROBAG_OK。</summary>
    MasterBagToHeroBagOk,
    /// <summary>原 1342-1346：SM_HEROBAGTOMASTERBAG_OK。</summary>
    HeroBagToMasterBagOk,
    /// <summary>原 1349-1353：SM_ENABLE_UPLOAD_PICKITEMS。</summary>
    EnableUploadPickItems,
    /// <summary>走到原 1358：<c>Context.AddServerMsg</c> 原样转发。</summary>
    PlainForward,
}

/// <summary>分类结果。</summary>
public readonly struct RunGateForwardPlan
{
    public readonly RunGateForwardKind Kind;
    /// <summary>CacheStore 时的槽位下标，其余为 -1。</summary>
    public readonly int CacheIndex;
    /// <summary>原 987-993：<c>BufferLen &gt; SizeOf(TDefaultMessage)</c> 才有附加数据。</summary>
    public readonly bool HasExtraData;

    private RunGateForwardPlan(RunGateForwardKind kind, int cacheIndex, bool hasExtraData)
    {
        Kind = kind;
        CacheIndex = cacheIndex;
        HasExtraData = hasExtraData;
    }

    public static RunGateForwardPlan Make(RunGateForwardKind kind, int cacheIndex, bool hasExtraData)
        => new(kind, cacheIndex, hasExtraData);
}

/// <summary>DoForwardToClientData 的纯分类器。</summary>
public static class RunGateForwardClassifier
{
    /// <summary>
    /// 判定 926-1365 的 else-if 链走向。
    /// <paramref name="recogMatches"/> 用于 SM_CHANGESPEED 的 <c>Recog = Context.nRecogId</c> 条件
    /// （该条件不满足时**不会**落到 PlainForward 之外的短路，而是继续往下走完整条链）。
    /// </summary>
    public static RunGateForwardPlan Classify(int ident, int bufferLen, bool recogMatches = false)
    {
        // 原 944
        if (bufferLen < RunGateUtilsConst.SizeOfTDefaultMessage)
            return RunGateForwardPlan.Make(RunGateForwardKind.TooShort, -1, false);
        // 原 949
        if (ident == Grobal2Const.SM_CHECK_RUNGATE1)
            return RunGateForwardPlan.Make(RunGateForwardKind.CheckRunGate1, -1, false);
        // 原 979-982 的 `if BufferLen <= 0 then Exit` 是**死代码**：原 944 已经保证 BufferLen >= 16。
        // 这里刻意不给它单独分支，测试 RunGateUtilsForwardTests.ShortPacket_IsDroppedBeforeAnyIdentCheck
        // 断言 bufferLen <= 0 也归入 TooShort。

        bool hasExtra = bufferLen > RunGateUtilsConst.SizeOfTDefaultMessage;   // 原 987

        if (ident == Grobal2Const.SM_LOGON) return RunGateForwardPlan.Make(RunGateForwardKind.Logon, -1, hasExtra);
        if (ident == Grobal2Const.SM_CHANGESPEED && recogMatches)
            return RunGateForwardPlan.Make(RunGateForwardKind.ChangeSpeed, -1, hasExtra);
        if (ident == Grobal2Const.SM_ABILITY) return RunGateForwardPlan.Make(RunGateForwardKind.Ability, -1, hasExtra);
        if (ident == Grobal2Const.SM_HEROABILITY) return RunGateForwardPlan.Make(RunGateForwardKind.HeroAbility, -1, hasExtra);
        if (ident == Grobal2Const.SM_CHANGEMAP) return RunGateForwardPlan.Make(RunGateForwardKind.ChangeMap, -1, hasExtra);
        if (ident == Grobal2Const.SM_NEWMAP) return RunGateForwardPlan.Make(RunGateForwardKind.NewMap, -1, hasExtra);

        int cacheIndex = RunGateCacheTable.LiveIdentToIndex(ident);
        if (cacheIndex >= 0)
            return RunGateForwardPlan.Make(RunGateForwardKind.CacheStore, cacheIndex, hasExtra);

        if (RunGateCacheTable.IsCacheIdent(ident))
            return RunGateForwardPlan.Make(RunGateForwardKind.CacheReply,
                                           RunGateCacheTable.CacheIdentToIndex(ident), hasExtra);

        if (ident == Grobal2Const.SM_BLACKMODULEMD5)
            return RunGateForwardPlan.Make(RunGateForwardKind.BlackModuleMd5, -1, hasExtra);
        if (ident == Grobal2Const.SM_SENDNOTICE)
            return RunGateForwardPlan.Make(RunGateForwardKind.SendNotice, -1, hasExtra);

        if (hasExtra)
            return RunGateForwardPlan.Make(RunGateForwardKind.ItemFamilyWithData, -1, true);

        if (ident == Grobal2Const.SM_EAT_OK || ident == Grobal2Const.SM_AUTOEAT_OK)
            return RunGateForwardPlan.Make(RunGateForwardKind.EatOk, -1, false);
        if (ident == Grobal2Const.SM_HEROEAT_OK || ident == Grobal2Const.SM_HEROAUTOEAT_OK)
            return RunGateForwardPlan.Make(RunGateForwardKind.HeroEatOk, -1, false);
        if (ident == Grobal2Const.SM_MASTERBAGTOHEROBAG_OK)
            return RunGateForwardPlan.Make(RunGateForwardKind.MasterBagToHeroBagOk, -1, false);
        if (ident == Grobal2Const.SM_HEROBAGTOMASTERBAG_OK)
            return RunGateForwardPlan.Make(RunGateForwardKind.HeroBagToMasterBagOk, -1, false);
        if (ident == Grobal2Const.SM_ENABLE_UPLOAD_PICKITEMS)
            return RunGateForwardPlan.Make(RunGateForwardKind.EnableUploadPickItems, -1, false);

        return RunGateForwardPlan.Make(RunGateForwardKind.PlainForward, -1, hasExtra);
    }

    /// <summary>
    /// 原 1234-1264 之前的守卫：SM_BLACKMODULEMD5 分支里是否要重发反外挂流。
    /// <c>(not g_boLogoutNoResendAntiplugStream) or (dwRecvClientAntiplugCRC &lt;&gt; g_ClientAntiPlugDllStringCRC)</c>
    /// </summary>
    public static bool ShouldResendAntiplugStream(bool logoutNoResendAntiplugStream, uint recvCrc, uint dllStringCrc)
        => !logoutNoResendAntiplugStream || recvCrc != dllStringCrc;

    /// <summary>
    /// 原 1245 —— 是否发送进程黑名单：<c>(Length(g_ProcessBlacklistStr) &gt; 0) and (SM_PROCESSBLACKLIST &gt; 0)</c>。
    /// 第二个条件是对**消息号本身**取正（反注册版可能把 SM_PROCESSBLACKLIST 改写为 0）。
    /// </summary>
    public static bool ShouldSendProcessBlacklist(int blacklistStrLength)
        => blacklistStrLength > 0 && Grobal2Const.SM_PROCESSBLACKLIST > 0;

    /// <summary>原 1023-1024/1030-1031 —— <c>btJob := LoByte(Param); if &gt; 2 then 0</c>。</summary>
    public static byte NormalizeJob(ushort param)
    {
        byte job = DelphiRTL.LoByte(param);
        return job > 2 ? (byte)0 : job;
    }

    /// <summary>原 1015-1017 —— SM_CHANGESPEED 的三个速度按 <c>SmallInt</c> 解释（可为负）。</summary>
    public static short ToSpeed(ushort value) => unchecked((short)value);

    /// <summary>
    /// 原 1038-1043 / 1060-1065 —— 地图名清洗：<c>Index := Pos(sLineBreak, sMapName);
    /// if Index &gt; 0 then sMapName := Copy(sMapName, Index + 2, MaxInt)</c>。
    /// 只认 <c>#13#10</c>（Delphi <c>sLineBreak</c>），单独 CR 或单独 LF 都不裁。
    /// </summary>
    public static string StripMapNameHeader(string decoded)
    {
        if (string.IsNullOrEmpty(decoded)) return decoded ?? string.Empty;
        int idx = decoded.IndexOf("\r\n", StringComparison.Ordinal);
        if (idx < 0) return decoded;
        int start = idx + 2;                       // 原 Copy(s, Index + 2, MaxInt)
        return start >= decoded.Length ? string.Empty : decoded.Substring(start);
    }
}

/// <summary>
/// 原 949-970 —— SM_CHECK_RUNGATE1 的应答散列（VMProtect 包裹内，纯算术）。
/// </summary>
public static class RunGateCheckRunGate1
{    /// <summary>原 959 —— 散列初值。</summary>
    public const uint Seed = 0x522582F4u;

    /// <summary>
    /// 计算应答。返回的 <c>Crc</c> 与 <c>XorValue</c> 分别对应原文
    /// <c>SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, CRC, CRC xor dwValue, nil, 0)</c> 的
    /// nSocket 与 nUserListIndex（注意 nUserListIndex 形参是 Integer，<c>CRC xor dwValue</c> 会以 32 位传）。
    /// </summary>
    public static void Compute(ushort param, ushort tag, long recog, out uint crc, out uint xorValue)
    {
        uint crcField = unchecked((uint)DelphiRTL.MakeLong(param, tag));    // 原 952
        uint dwValue = unchecked((uint)recog);                              // 原 953：Int64 → LongWord 截断

        Span<byte> s = stackalloc byte[8];
        WriteLe32(s, 0, crcField);                                          // 原 956
        WriteLe32(s, 4, dwValue);                                           // 原 957

        crc = Seed;
        for (int i = 0; i < s.Length; i++)                                  // 原 960-963
            crc = ((crc >> 2) ^ (crc << 6)) ^ s[i];

        xorValue = crc ^ dwValue;                                           // 原 965
    }

    private static void WriteLe32(Span<byte> b, int i, uint v)
    {
        b[i] = (byte)v;
        b[i + 1] = (byte)(v >> 8);
        b[i + 2] = (byte)(v >> 16);
        b[i + 3] = (byte)(v >> 24);
    }
}

/// <summary>
/// 原文里三种"按帧头字段找会话"的**不同**下标派生方式（这是本单元最容易翻错的一处）。
/// <para>
/// <c>RunGateUtils.pas:582</c>（GM_SERVERUSERINDEX）与 <c>:658</c>（GM_KICK）都用
/// <c>Contexts[MsgHeader.wGSocketIdx]</c>（**直接用**，不加减）；
/// 而 <c>:678</c>（GM_CLOSE）用的是 <c>Contexts[MsgHeader.wUserListIndex - 1]</c>（**减 1**）。
/// </para>
/// <para>
/// 缺陷：GM_CLOSE 分支里 <c>wUserListIndex</c> 并未做任何范围校验，
/// 若 M2 发来的帧里该字段为 0（GM_* 帧常常不填用户序号），则下标为 <b>-1</b>；
/// Delphi 默认关范围检查时 <c>List[-1]</c> 会读到数组首元素**之前**的内存并强转成对象引用 → 访问违例/野指针。
/// 移植时**不得**擅自"顺手修正"为 0 或加保护，只能把它登记为缺陷（见报告 §5.22）。
/// </para>
/// </summary>
public static class RunGateContextLookup
{
    /// <summary>原 582 / 658 —— 直接用 <c>wGSocketIdx</c>。</summary>
    public static int ForSocketScoped(ushort wGSocketIdx) => wGSocketIdx;

    /// <summary>
    /// 原 678 —— <c>wUserListIndex - 1</c>（1-based → 0-based）。
    /// 原文先在 <c>LongWord</c> 域里做减法，再把结果当 <c>Integer</c> 下标用；
    /// 因此输入 0 得到 <b>-1</b>（越界），输入 <c>&gt;= $80000001</c> 会回绕成大正数。
    /// </summary>
    public static int ForGmClose(uint wUserListIndex) => unchecked((int)(wUserListIndex - 1u));

    /// <summary>该下标是否可用（&gt;= 0）。原文**没有**这个检查，本方法只用于测试表达缺陷边界。</summary>
    public static bool IsUsableIndex(int index) => index >= 0;
}

