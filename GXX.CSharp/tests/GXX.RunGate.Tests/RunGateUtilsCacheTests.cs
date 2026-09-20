using System;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsCache.cs 测试 —— 覆盖 RunGateUtils.pas:1094-1231（14 类配置包的网关侧缓存）。
/// </summary>
public class RunGateUtilsCacheTests
{
    [Fact]
    public void Table_Has14Slots_AndCacheIdentsAreContiguous()
    {
        // 原 38：FCacheDatas: array[0..13] of string；原 1227 用 Ident - SM_MODULEMD5_CACHE 直接做下标
        Assert.Equal(14, RunGateCacheTable.SlotCount);
        Assert.Equal(14, RunGateCacheTable.Slots.Length);
        Assert.True(RunGateCacheTable.VerifyContiguity());
        Assert.Equal(Grobal2Const.SM_MODULEMD5_CACHE, RunGateCacheTable.MinCacheIdent);
        Assert.Equal(Grobal2Const.SM_SENDDROPITEMEFFECTLIST_CACHE, RunGateCacheTable.MaxCacheIdent);
        Assert.Equal(10104, RunGateCacheTable.MinCacheIdent);
        Assert.Equal(10117, RunGateCacheTable.MaxCacheIdent);
    }

    [Fact]
    public void Table_LiveIdentsMatchGrobal2Constants()
    {
        Assert.Equal(Grobal2Const.SM_MODULEMD5, RunGateCacheTable.Slots[0].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDCUSTOMMONSTERCONFIG, RunGateCacheTable.Slots[1].LiveIdent);
        Assert.Equal(Grobal2Const.SM_STDITEMLIST, RunGateCacheTable.Slots[2].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDITEMDESCLIST, RunGateCacheTable.Slots[3].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDTZITEMDESCLIST, RunGateCacheTable.Slots[4].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDFILTERITEMLIST, RunGateCacheTable.Slots[5].LiveIdent);
        Assert.Equal(Grobal2Const.SM_EFFECTIMAGELIST, RunGateCacheTable.Slots[6].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SPECIALCMD, RunGateCacheTable.Slots[7].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDCUSTOMMAGICCONFIG, RunGateCacheTable.Slots[8].LiveIdent);
        Assert.Equal(Grobal2Const.SM_PLUGFILE, RunGateCacheTable.Slots[9].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SERVERCONFIG, RunGateCacheTable.Slots[10].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDCUSTOMNPCCONFIG, RunGateCacheTable.Slots[11].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDITEMDESCTOPLIST, RunGateCacheTable.Slots[12].LiveIdent);
        Assert.Equal(Grobal2Const.SM_SENDDROPITEMEFFECTLIST, RunGateCacheTable.Slots[13].LiveIdent);
    }

    [Fact]
    public void LiveIdentToIndex_HitsAllSlots_AndMinusOneOtherwise()
    {
        for (int i = 0; i < RunGateCacheTable.Slots.Length; i++)
            Assert.Equal(i, RunGateCacheTable.LiveIdentToIndex(RunGateCacheTable.Slots[i].LiveIdent));

        Assert.Equal(-1, RunGateCacheTable.LiveIdentToIndex(Grobal2Const.SM_LOGON));
        Assert.Equal(-1, RunGateCacheTable.LiveIdentToIndex(0));
        Assert.Equal(-1, RunGateCacheTable.LiveIdentToIndex(Grobal2Const.SM_BLACKMODULEMD5));
    }

    [Fact]
    public void IsCacheIdent_BoundariesAreInclusive()
    {
        // 原 1175：(Ident >= SM_MODULEMD5_CACHE) and (Ident <= SM_SENDDROPITEMEFFECTLIST_CACHE)
        Assert.False(RunGateCacheTable.IsCacheIdent(10103));
        Assert.True(RunGateCacheTable.IsCacheIdent(10104));
        Assert.True(RunGateCacheTable.IsCacheIdent(10110));
        Assert.True(RunGateCacheTable.IsCacheIdent(10117));
        Assert.False(RunGateCacheTable.IsCacheIdent(10118));
        // 与表头/表尾常量一致
        Assert.True(RunGateCacheTable.IsCacheIdent(RunGateCacheTable.MinCacheIdent));
        Assert.True(RunGateCacheTable.IsCacheIdent(RunGateCacheTable.MaxCacheIdent));
    }

    [Fact]
    public void Remap_CacheToLive_MatchesForwardChain()
    {
        for (int i = 0; i < RunGateCacheTable.Slots.Length; i++)
        {
            Assert.True(RunGateCacheTable.TryRemapCacheToLive(RunGateCacheTable.Slots[i].CacheIdent, out int live));
            Assert.Equal(RunGateCacheTable.Slots[i].LiveIdent, live);
        }

        Assert.False(RunGateCacheTable.TryRemapCacheToLive(10103, out int nothing));
        Assert.Equal(0, nothing);
        Assert.False(RunGateCacheTable.TryRemapCacheToLive(10118, out _));
    }

    [Fact]
    public void CacheIdentToIndex_IsAPlainSubtraction()
    {
        for (int i = 0; i < 14; i++)
            Assert.Equal(i, RunGateCacheTable.CacheIdentToIndex(10104 + i));
    }

    /// <summary>
    /// 原文缺陷登记：RunGateUtils.pas:107 的死分支声明 <c>FCacheDatas: array[0..12]</c>（13 槽），
    /// 而索引映射会走到 13。若把 <c>UseIocpClient</c> 切回 0，写槽 13 就数组越界
    /// （Delphi 默认关范围检查 → 静默内存破坏）。活分支（:38）是 <c>array[0..13]</c>，安全。
    /// </summary>
    [Fact]
    public void DeadBranch_FCacheDatas_IsTooSmall()
    {
        Assert.Equal(13, RunGateCacheTable.DeadBranchSlotCount);
        Assert.True(RunGateCacheTable.SlotCount > RunGateCacheTable.DeadBranchSlotCount);
        // 最高下标 13 在死分支下越界
        Assert.Equal(13, RunGateCacheTable.Slots[13].Index);
        Assert.True(RunGateCacheTable.Slots[13].Index >= RunGateCacheTable.DeadBranchSlotCount);
    }

    // ---------------- 缓存体 ----------------

    [Fact]
    public void DataCache_StoreAndReadRoundTrip()
    {
        var cache = new RunGateDataCache();
        var payload = new byte[] { 1, 2, 3, 4, 5 };
        Assert.True(cache.StoreByLiveIdent(Grobal2Const.SM_STDITEMLIST, payload, payload.Length));

        var read = cache.ReadByCacheIdent(Grobal2Const.SM_STDITEMLIST_CACHE);
        Assert.Equal(payload, read);
        Assert.Equal(5, cache.LoadedLength(RunGateCacheTable.LiveIdentToIndex(Grobal2Const.SM_STDITEMLIST)));
    }

    [Fact]
    public void DataCache_StoreUnknownLiveIdent_ReturnsFalseAndStoresNothing()
    {
        var cache = new RunGateDataCache();
        Assert.False(cache.StoreByLiveIdent(Grobal2Const.SM_LOGON, new byte[4], 4));
        for (int i = 0; i < 14; i++) Assert.Equal(0, cache.LoadedLength(i));
    }

    [Fact]
    public void DataCache_MissReturnsEmptyArray_ButStillCountsAsAReply()
    {
        // 原 1227-1228：命中缓存分支时**无条件**回发，不检查槽是否装载 → 空负载
        var cache = new RunGateDataCache();
        var read = cache.ReadByCacheIdent(Grobal2Const.SM_SERVERCONFIG_CACHE);
        Assert.NotNull(read);
        Assert.Empty(read);
    }

    [Fact]
    public void DataCache_OutOfRangeCacheIdent_ReturnsNull()
    {
        // 与"在范围内但空"区分开：越界返回 null，空槽返回空数组
        var cache = new RunGateDataCache();
        Assert.Null(cache.ReadByCacheIdent(10103));
        Assert.Null(cache.ReadByCacheIdent(10118));
        Assert.Empty(cache.ReadByCacheIdent(10104));
    }

    [Fact]
    public void DataCache_StoreTruncatesToGivenLength()
    {
        var cache = new RunGateDataCache();
        cache.Store(0, new byte[] { 9, 9, 9, 9 }, 2);
        Assert.Equal(2, cache.LoadedLength(0));
        Assert.Equal(new byte[] { 9, 9 }, cache.ReadByCacheIdent(10104));
    }

    [Fact]
    public void DataCache_StoreNonPositiveLength_ClearsSlot()
    {
        var cache = new RunGateDataCache();
        cache.Store(0, new byte[] { 1, 2 }, 2);
        Assert.Equal(2, cache.LoadedLength(0));
        cache.Store(0, new byte[] { 1, 2 }, 0);
        Assert.Equal(0, cache.LoadedLength(0));
        Assert.Empty(cache.ReadByCacheIdent(10104));
    }

    [Fact]
    public void DataCache_StoreCopiesBuffer_NotAliases()
    {
        var cache = new RunGateDataCache();
        var src = new byte[] { 1, 2, 3 };
        cache.Store(0, src, 3);
        src[0] = 0xFF;
        Assert.Equal(new byte[] { 1, 2, 3 }, cache.ReadByCacheIdent(10104));
    }

    [Fact]
    public void DataCache_Clear_EmptiesAllSlots()
    {
        var cache = new RunGateDataCache();
        for (int i = 0; i < 14; i++) cache.Store(i, new byte[] { (byte)i }, 1);
        cache.Clear();
        for (int i = 0; i < 14; i++) Assert.Equal(0, cache.LoadedLength(i));
    }

    [Fact]
    public void DataCache_OutOfRangeIndex_Throws()
    {
        var cache = new RunGateDataCache();
        Assert.Throws<ArgumentOutOfRangeException>(() => cache.Store(14, new byte[1], 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => cache.Store(-1, new byte[1], 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => cache.LoadedLength(14));
    }
}
