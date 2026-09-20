using GXX.Core.Protocol;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsForward.cs 测试 —— 覆盖 RunGateUtils.pas:926-1365（DoForwardToClientData 的 else-if 链）
/// 与 949-970（SM_CHECK_RUNGATE1 应答）。
/// </summary>
public class RunGateUtilsForwardTests
{
    private const int Data = 16;

    private static RunGateForwardPlan C(int ident, int len, bool recog = false)
        => RunGateForwardClassifier.Classify(ident, len, recog);

    // ---------------- 前置守卫 ----------------

    [Fact]
    public void ShortPacket_IsDroppedBeforeAnyIdentCheck()
    {
        // 原 944：BufferLen < SizeOf(TDefaultMessage) → Exit（先于 Ident 判定）
        Assert.Equal(RunGateForwardKind.TooShort, C(Grobal2Const.SM_LOGON, 15).Kind);
        Assert.Equal(RunGateForwardKind.TooShort, C(Grobal2Const.SM_CHECK_RUNGATE1, 15).Kind);
        // 原 979 的 `BufferLen <= 0` 是死代码（944 已拦），因此 0 也归 TooShort
        Assert.Equal(RunGateForwardKind.TooShort, C(9999, 0).Kind);
        Assert.Equal(RunGateForwardKind.TooShort, C(9999, -1).Kind);
        Assert.Equal(RunGateForwardKind.TooShort, C(9999, int.MinValue).Kind);
    }

    [Fact]
    public void CheckRunGate1_BypassesEverythingElse()
    {
        // 原 949-970：即使带附加数据也要走专用分支
        var p = C(Grobal2Const.SM_CHECK_RUNGATE1, Data + 100);
        Assert.Equal(RunGateForwardKind.CheckRunGate1, p.Kind);
        Assert.Equal(-1, p.CacheIndex);
    }

    // ---------------- 状态改写类 ----------------

    [Fact]
    public void Logon_Matches()
    {
        Assert.Equal(RunGateForwardKind.Logon, C(Grobal2Const.SM_LOGON, Data).Kind);
    }

    [Fact]
    public void ChangeSpeed_RequiresRecogMatch_Differential()
    {
        // 原 1011-1012：(Ident = SM_CHANGESPEED) and (Recog = Context.nRecogId)
        Assert.Equal(RunGateForwardKind.ChangeSpeed, C(Grobal2Const.SM_CHANGESPEED, Data, recog: true).Kind);
        // Recog 不匹配 → **不会**短路，继续走完整条链，最终落到 PlainForward
        Assert.Equal(RunGateForwardKind.PlainForward, C(Grobal2Const.SM_CHANGESPEED, Data, recog: false).Kind);
        // 带数据且 Recog 不匹配时，会被 ItemFamilyWithData 吃掉（因为它更靠前？不——它在更后面）
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData,
                     C(Grobal2Const.SM_CHANGESPEED, Data + 1, recog: false).Kind);
    }

    [Fact]
    public void AbilityAndHeroAbility_AreDistinctIdents()
    {
        Assert.NotEqual(Grobal2Const.SM_ABILITY, Grobal2Const.SM_HEROABILITY);
        Assert.Equal(RunGateForwardKind.Ability, C(Grobal2Const.SM_ABILITY, Data).Kind);
        Assert.Equal(RunGateForwardKind.HeroAbility, C(Grobal2Const.SM_HEROABILITY, Data).Kind);
    }

    [Fact]
    public void ChangeMapAndNewMap_AreDistinct()
    {
        Assert.Equal(RunGateForwardKind.ChangeMap, C(Grobal2Const.SM_CHANGEMAP, Data).Kind);
        Assert.Equal(RunGateForwardKind.NewMap, C(Grobal2Const.SM_NEWMAP, Data).Kind);
        Assert.NotEqual(Grobal2Const.SM_CHANGEMAP, Grobal2Const.SM_NEWMAP);
    }

    [Fact]
    public void MapMessages_HaveExtraDataFlagOnlyWhenLenGreaterThan16()
    {
        Assert.False(C(Grobal2Const.SM_CHANGEMAP, Data).HasExtraData);
        Assert.True(C(Grobal2Const.SM_CHANGEMAP, Data + 1).HasExtraData);
        Assert.False(C(Grobal2Const.SM_NEWMAP, Data).HasExtraData);
        Assert.True(C(Grobal2Const.SM_NEWMAP, Data + 5).HasExtraData);
    }

    // ---------------- 缓存 ----------------

    [Fact]
    public void CacheableIdents_MapToTheirSlotIndices()
    {
        for (int i = 0; i < RunGateCacheTable.Slots.Length; i++)
        {
            var p = C(RunGateCacheTable.Slots[i].LiveIdent, Data + 3);
            Assert.Equal(RunGateForwardKind.CacheStore, p.Kind);
            Assert.Equal(i, p.CacheIndex);
        }
    }

    [Fact]
    public void CacheRequestIdents_MapBackToSlots()
    {
        for (int i = 0; i < RunGateCacheTable.Slots.Length; i++)
        {
            var p = C(RunGateCacheTable.Slots[i].CacheIdent, Data + 3);
            Assert.Equal(RunGateForwardKind.CacheReply, p.Kind);
            Assert.Equal(i, p.CacheIndex);
        }
    }

    [Fact]
    public void CacheRangeBoundaries_AreExclusiveOfNeighbours()
    {
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(10103, Data + 3).Kind);
        Assert.Equal(RunGateForwardKind.CacheReply, C(10104, Data + 3).Kind);
        Assert.Equal(RunGateForwardKind.CacheReply, C(10117, Data + 3).Kind);
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(10118, Data + 3).Kind);
    }

    // ---------------- 其它分支 ----------------

    [Fact]
    public void BlackModuleMd5_And_SendNotice_AreDistinct()
    {
        Assert.Equal(RunGateForwardKind.BlackModuleMd5, C(Grobal2Const.SM_BLACKMODULEMD5, Data).Kind);
        Assert.Equal(RunGateForwardKind.SendNotice, C(Grobal2Const.SM_SENDNOTICE, Data).Kind);
        Assert.NotEqual(Grobal2Const.SM_BLACKMODULEMD5, Grobal2Const.SM_SENDNOTICE);
    }

    /// <summary>
    /// 差异断言：SM_SENDNOTICE 在原文里是**空分支**（1267-1270），**没有 Exit**，
    /// 因此它会继续执行到 1358 的 <c>Context.AddServerMsg</c>（即"不处理但仍要转发"）。
    /// 分类器把它单独列出正是为了提醒调用方：这一支必须转发，不能当作已处理。
    /// </summary>
    [Fact]
    public void SendNotice_IsAnEmptyBranch_ButStillForwards()
    {
        var p = C(Grobal2Const.SM_SENDNOTICE, Data);
        Assert.Equal(RunGateForwardKind.SendNotice, p.Kind);
        // 与之对照：CheckRunGate1 / CacheReply 是**有 Exit** 的分支（分类器语义上等于"已终结"）
        Assert.Equal(RunGateForwardKind.CheckRunGate1, C(Grobal2Const.SM_CHECK_RUNGATE1, Data).Kind);
        Assert.Equal(RunGateForwardKind.CacheReply, C(10104, Data).Kind);
    }

    [Fact]
    public void ItemFamily_OnlyWhenHasExtraData()
    {
        // 原 1273：(DataAdd <> nil) and (DataAddLen > 0)
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(Grobal2Const.SM_BAGITEMS, Data + 1).Kind);
        Assert.Equal(RunGateForwardKind.PlainForward, C(Grobal2Const.SM_BAGITEMS, Data).Kind);
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(Grobal2Const.SM_SENDMYMAGIC, Data + 1).Kind);
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(Grobal2Const.SM_ADDMAGIC, Data + 1).Kind);
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(Grobal2Const.SM_DROPITEM_SUCCESS, Data + 1).Kind);
    }

    [Fact]
    public void EatAndBagSwap_OnlyWithoutExtraData()
    {
        // 这些分支在 1273 的 `else if (DataAdd <> nil)...` 之后 → 有数据时被 ItemFamilyWithData 抢先
        Assert.Equal(RunGateForwardKind.EatOk, C(Grobal2Const.SM_EAT_OK, Data).Kind);
        Assert.Equal(RunGateForwardKind.EatOk, C(Grobal2Const.SM_AUTOEAT_OK, Data).Kind);
        Assert.Equal(RunGateForwardKind.HeroEatOk, C(Grobal2Const.SM_HEROEAT_OK, Data).Kind);
        Assert.Equal(RunGateForwardKind.HeroEatOk, C(Grobal2Const.SM_HEROAUTOEAT_OK, Data).Kind);
        Assert.Equal(RunGateForwardKind.MasterBagToHeroBagOk, C(Grobal2Const.SM_MASTERBAGTOHEROBAG_OK, Data).Kind);
        Assert.Equal(RunGateForwardKind.HeroBagToMasterBagOk, C(Grobal2Const.SM_HEROBAGTOMASTERBAG_OK, Data).Kind);
        Assert.Equal(RunGateForwardKind.EnableUploadPickItems, C(Grobal2Const.SM_ENABLE_UPLOAD_PICKITEMS, Data).Kind);

        // 带数据 → ItemFamilyWithData 抢走
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(Grobal2Const.SM_EAT_OK, Data + 1).Kind);
        Assert.Equal(RunGateForwardKind.ItemFamilyWithData, C(Grobal2Const.SM_ENABLE_UPLOAD_PICKITEMS, Data + 1).Kind);
    }

    [Fact]
    public void UnknownIdent_WithoutData_IsPlainForward()
    {
        Assert.Equal(RunGateForwardKind.PlainForward, C(9999, Data).Kind);
        Assert.Equal(RunGateForwardKind.PlainForward, C(0, Data).Kind);
        Assert.Equal(RunGateForwardKind.PlainForward, C(65535, Data).Kind);
    }

    // ---------------- 纯改写助手 ----------------

    [Fact]
    public void NormalizeJob_ClampsAboveTwoToZero()
    {
        // 原 1023-1024：btJob := LoByte(Param); if > 2 then 0
        Assert.Equal((byte)0, RunGateForwardClassifier.NormalizeJob(0x0000));
        Assert.Equal((byte)1, RunGateForwardClassifier.NormalizeJob(0x0001));
        Assert.Equal((byte)2, RunGateForwardClassifier.NormalizeJob(0x0202));   // LoByte = 2
        Assert.Equal((byte)0, RunGateForwardClassifier.NormalizeJob(0x0003));
        Assert.Equal((byte)0, RunGateForwardClassifier.NormalizeJob(0x00FF));
        Assert.Equal((byte)0, RunGateForwardClassifier.NormalizeJob(0xFF03));   // LoByte = 3 → 归 0
    }

    [Fact]
    public void NormalizeJob_OnlyLooksAtLowByte()
    {
        // 高字节是 0xFF 也不影响（LoByte 只取低 8 位）
        Assert.Equal((byte)0, RunGateForwardClassifier.NormalizeJob(0xFF00));
        Assert.Equal((byte)2, RunGateForwardClassifier.NormalizeJob(0xFF02));
        Assert.Equal((byte)0, RunGateForwardClassifier.NormalizeJob(0xFFFF));
    }

    [Fact]
    public void ToSpeed_InterpretsWordAsSmallInt()
    {
        // 原 1015-1017：nMoveSpeed := SmallInt(Param)
        Assert.Equal((short)0, RunGateForwardClassifier.ToSpeed(0));
        Assert.Equal((short)100, RunGateForwardClassifier.ToSpeed(100));
        Assert.Equal((short)-1, RunGateForwardClassifier.ToSpeed(0xFFFF));
        Assert.Equal((short)-32768, RunGateForwardClassifier.ToSpeed(0x8000));
        Assert.Equal((short)32767, RunGateForwardClassifier.ToSpeed(0x7FFF));
    }

    [Fact]
    public void StripMapNameHeader_CutsAtCrLfOnly()
    {
        Assert.Equal("MAPNAME", RunGateForwardClassifier.StripMapNameHeader("0\r\nMAPNAME"));
        Assert.Equal("MAPNAME", RunGateForwardClassifier.StripMapNameHeader("123\r\nMAPNAME"));
        Assert.Equal("", RunGateForwardClassifier.StripMapNameHeader("\r\n"));
        Assert.Equal("", RunGateForwardClassifier.StripMapNameHeader("X\r\n"));
    }

    [Fact]
    public void StripMapNameHeader_LoneCrOrLfIsNotACut_Differential()
    {
        // Delphi sLineBreak = #13#10 —— 单独 CR / 单独 LF 都不裁
        Assert.Equal("AB\rCD", RunGateForwardClassifier.StripMapNameHeader("AB\rCD"));
        Assert.Equal("AB\nCD", RunGateForwardClassifier.StripMapNameHeader("AB\nCD"));
        Assert.Equal("CD", RunGateForwardClassifier.StripMapNameHeader("AB\r\nCD"));
        // CR 在前但后随 LF 之外字符 → 也不裁
        Assert.Equal("AB\rXCD", RunGateForwardClassifier.StripMapNameHeader("AB\rXCD"));
    }

    [Fact]
    public void StripMapNameHeader_NullAndEmpty()
    {
        Assert.Equal("", RunGateForwardClassifier.StripMapNameHeader(null));
        Assert.Equal("", RunGateForwardClassifier.StripMapNameHeader(""));
        Assert.Equal("PLAINMAP", RunGateForwardClassifier.StripMapNameHeader("PLAINMAP"));
    }

    [Fact]
    public void ShouldResendAntiplugStream_TruthTable()
    {
        // 原 1238：(not g_boLogoutNoResendAntiplugStream) or (recv <> dllCrc)
        Assert.True(RunGateForwardClassifier.ShouldResendAntiplugStream(false, 5, 5));
        Assert.True(RunGateForwardClassifier.ShouldResendAntiplugStream(false, 5, 6));
        Assert.True(RunGateForwardClassifier.ShouldResendAntiplugStream(true, 5, 6));
        Assert.False(RunGateForwardClassifier.ShouldResendAntiplugStream(true, 5, 5));
    }

    [Fact]
    public void ShouldSendProcessBlacklist_RequiresNonEmptyString()
    {
        Assert.True(RunGateForwardClassifier.ShouldSendProcessBlacklist(1));
        Assert.False(RunGateForwardClassifier.ShouldSendProcessBlacklist(0));
        Assert.False(RunGateForwardClassifier.ShouldSendProcessBlacklist(-1));
    }

    // ---------------- SM_CHECK_RUNGATE1 应答 ----------------

    [Fact]
    public void CheckRunGate1_XorValueIsCrcXorLow32OfRecog()
    {
        RunGateCheckRunGate1.Compute(0x1234, 0x5678, 0x1122334455667788L, out uint crc, out uint xor);
        Assert.Equal(crc ^ 0x55667788u, xor);
    }

    [Fact]
    public void CheckRunGate1_RecogHighBitsAreTruncated()
    {
        // 原 953：dwValue := pDefMsg.Recog（Int64 → LongWord 截断）
        RunGateCheckRunGate1.Compute(1, 2, 0x00000000AAAAAAAAL, out uint c1, out uint x1);
        RunGateCheckRunGate1.Compute(1, 2, unchecked((long)0xFFFFFFFFAAAAAAAAL), out uint c2, out uint x2);
        Assert.Equal(c1, c2);
        Assert.Equal(x1, x2);
    }

    [Fact]
    public void CheckRunGate1_ParamAndTagBothAffectResult()
    {
        RunGateCheckRunGate1.Compute(0, 0, 0, out uint a, out _);
        RunGateCheckRunGate1.Compute(1, 0, 0, out uint b, out _);
        RunGateCheckRunGate1.Compute(0, 1, 0, out uint c, out _);
        Assert.NotEqual(a, b);
        Assert.NotEqual(a, c);
        Assert.NotEqual(b, c);
    }

    [Fact]
    public void CheckRunGate1_ParamTagSwappedIsDifferent()
    {
        // MakeLong(Param, Tag) = (Tag << 16) | Param → 交换两者一定不同
        RunGateCheckRunGate1.Compute(0x1111, 0x2222, 0, out uint a, out _);
        RunGateCheckRunGate1.Compute(0x2222, 0x1111, 0, out uint b, out _);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void CheckRunGate1_SeedIsUsed()
    {
        Assert.Equal(0x522582F4u, RunGateCheckRunGate1.Seed);
        // 手算：crc = seed；逐字节 crc = ((crc>>2) ^ (crc<<6)) ^ b
        uint crcField = (uint)((0x0002 << 16) | 0x0001);
        uint crc = RunGateCheckRunGate1.Seed;
        var s = new byte[8];
        System.BitConverter.GetBytes(crcField).CopyTo(s, 0);
        System.BitConverter.GetBytes(0u).CopyTo(s, 4);
        foreach (var b in s) crc = ((crc >> 2) ^ (crc << 6)) ^ b;

        RunGateCheckRunGate1.Compute(0x0001, 0x0002, 0, out uint got, out _);
        Assert.Equal(crc, got);
    }

    [Fact]
    public void CheckRunGate1_Deterministic()
    {
        RunGateCheckRunGate1.Compute(5, 7, 11, out uint c1, out uint x1);
        RunGateCheckRunGate1.Compute(5, 7, 11, out uint c2, out uint x2);
        Assert.Equal(c1, c2);
        Assert.Equal(x1, x2);
    }

    // ---------------- 会话下标派生的三处差异 ----------------

    /// <summary>
    /// 差异断言：GM_SERVERUSERINDEX / GM_KICK 用 <c>wGSocketIdx</c>（不加减），
    /// GM_CLOSE 用 <c>wUserListIndex - 1</c>。**同一份帧头，两个字段，两种算法**。
    /// </summary>
    [Fact]
    public void ContextLookup_TwoDifferentIndexDerivations()
    {
        Assert.Equal(0, RunGateContextLookup.ForSocketScoped(0));
        Assert.Equal(7, RunGateContextLookup.ForSocketScoped(7));
        Assert.Equal(65535, RunGateContextLookup.ForSocketScoped(65535));

        // 1-based → 0-based
        Assert.Equal(0, RunGateContextLookup.ForGmClose(1));
        Assert.Equal(6, RunGateContextLookup.ForGmClose(7));
    }

    /// <summary>
    /// 原文缺陷：GM_CLOSE 分支对 <c>wUserListIndex</c> 无范围校验，字段为 0 时下标为 -1。
    /// 原代码 <c>Contexts[MsgHeader.wUserListIndex - 1]</c>（RunGateUtils.pas:678）会越界。
    /// </summary>
    [Fact]
    public void ContextLookup_GmCloseWithZeroUserIndex_YieldsNegativeIndex()
    {
        int idx = RunGateContextLookup.ForGmClose(0);
        Assert.Equal(-1, idx);
        Assert.False(RunGateContextLookup.IsUsableIndex(idx));

        // 对照：同一个 0 走 wGSocketIdx 路线是合法的
        Assert.True(RunGateContextLookup.IsUsableIndex(RunGateContextLookup.ForSocketScoped(0)));
    }

    [Fact]
    public void ContextLookup_GmCloseHugeValue_WrapsInUnsignedDomain()
    {
        // 原文在 LongWord 域里减 1 再当 Integer 用：
        // 0x80000000 - 1 = 0x7FFFFFFF → +2147483647（"合法"但离谱的下标）
        Assert.Equal(2147483647, RunGateContextLookup.ForGmClose(0x80000000u));
        Assert.True(RunGateContextLookup.IsUsableIndex(RunGateContextLookup.ForGmClose(0x80000000u)));
        // 0 - 1 = 0xFFFFFFFF → -1
        Assert.Equal(-1, RunGateContextLookup.ForGmClose(0u));
        // uint.MaxValue - 1 = 0xFFFFFFFE → -2
        Assert.Equal(-2, RunGateContextLookup.ForGmClose(uint.MaxValue));
        Assert.False(RunGateContextLookup.IsUsableIndex(RunGateContextLookup.ForGmClose(uint.MaxValue)));
    }
}
