using System;
using GXX.Core.Protocol;

// 源：Source/RunGate/RunGateUtils.pas:1094-1231 —— TMirRemoteContext.DoForwardToClientData 的
// 「M2→网关数据缓存」段：把 14 类大配置包缓存到网关，之后客户端再要就由网关直发（提高吞吐）。
// 抽成纯表 + 纯函数，便于单测；不碰 socket / Context。

namespace GXX.RunGate;

/// <summary>一条缓存槽（live ident ↔ cache ident 的 1:1 对应）。</summary>
public readonly struct RunGateCacheSlot
{
    public readonly int Index;
    public readonly ushort LiveIdent;
    public readonly ushort CacheIdent;

    public RunGateCacheSlot(int index, ushort liveIdent, ushort cacheIdent)
    {
        Index = index;
        LiveIdent = liveIdent;
        CacheIdent = cacheIdent;
    }
}

/// <summary>
/// RunGateUtils.pas:1113-1163（正向：live→index）与 1179-1227（反向：cache→live）的映射表。
/// <para>
/// 关键不变量：14 个 cache ident 必须**连续**（10104..10117），因为原文用
/// <c>FCacheDatas[pDefMsg.Ident - SM_MODULEMD5_CACHE]</c> 直接做下标算术（RunGateUtils.pas:1227）。
/// 一旦出现空洞就会读错槽位 —— 测试里用 <see cref="VerifyContiguity"/> 把这条钉死。
/// </para>
/// </summary>
public static class RunGateCacheTable
{
    /// <summary><c>FCacheDatas: array[0..13] of string</c>（RunGateUtils.pas:38）—— 活分支的槽位数。</summary>
    public const int SlotCount = 14;

    /// <summary>
    /// 死分支 <c>TRunGate.FCacheDatas: array[0..12] of string</c>（RunGateUtils.pas:107）的槽位数。
    /// 该分支在 <c>UseIocpClient = 0</c> 时才编译，而 IocpCommon.pas:14 定死 <c>UseIocpClient = 1</c>，
    /// 故不会真的越界；但若有人切回 0，index 13（SM_SENDDROPITEMEFFECTLIST）就会写越界。
    /// 作为原文缺陷登记（见 RunGateUtilsCacheTests.DeadBranch_FCacheDatas_IsTooSmall）。
    /// </summary>
    public const int DeadBranchSlotCount = 13;

    public static readonly RunGateCacheSlot[] Slots =
    {
        new(0,  Grobal2Const.SM_MODULEMD5,                 Grobal2Const.SM_MODULEMD5_CACHE),
        new(1,  Grobal2Const.SM_SENDCUSTOMMONSTERCONFIG,   Grobal2Const.SM_SENDCUSTOMMONSTERCONFIG_CACHE),
        new(2,  Grobal2Const.SM_STDITEMLIST,               Grobal2Const.SM_STDITEMLIST_CACHE),
        new(3,  Grobal2Const.SM_SENDITEMDESCLIST,          Grobal2Const.SM_SENDITEMDESCLIST_CACHE),
        new(4,  Grobal2Const.SM_SENDTZITEMDESCLIST,        Grobal2Const.SM_SENDTZITEMDESCLIST_CACHE),
        new(5,  Grobal2Const.SM_SENDFILTERITEMLIST,        Grobal2Const.SM_SENDFILTERITEMLIST_CACHE),
        new(6,  Grobal2Const.SM_EFFECTIMAGELIST,           Grobal2Const.SM_EFFECTIMAGELIST_CACHE),
        new(7,  Grobal2Const.SM_SPECIALCMD,                Grobal2Const.SM_SPECIALCMD_CACHE),
        new(8,  Grobal2Const.SM_SENDCUSTOMMAGICCONFIG,     Grobal2Const.SM_SENDCUSTOMMAGICCONFIG_CACHE),
        new(9,  Grobal2Const.SM_PLUGFILE,                  Grobal2Const.SM_PLUGFILE_CACHE),
        new(10, Grobal2Const.SM_SERVERCONFIG,              Grobal2Const.SM_SERVERCONFIG_CACHE),
        new(11, Grobal2Const.SM_SENDCUSTOMNPCCONFIG,       Grobal2Const.SM_SENDCUSTOMNPCCONFIG_CACHE),
        new(12, Grobal2Const.SM_SENDITEMDESCTOPLIST,       Grobal2Const.SM_SENDITEMDESCTOPLIST_CACHE),
        new(13, Grobal2Const.SM_SENDDROPITEMEFFECTLIST,    Grobal2Const.SM_SENDDROPITEMEFFECTLIST_CACHE),
    };

    public static int MinCacheIdent => Slots[0].CacheIdent;
    public static int MaxCacheIdent => Slots[SlotCount - 1].CacheIdent;

    /// <summary>RunGateUtils.pas:1113-1163 —— live ident → 槽位下标；未命中返回 -1（原文 <c>Index := -1</c>）。</summary>
    public static int LiveIdentToIndex(int liveIdent)
    {
        for (int i = 0; i < Slots.Length; i++)
            if (Slots[i].LiveIdent == (ushort)liveIdent) return i;
        return -1;
    }

    /// <summary>RunGateUtils.pas:1175 —— 外层范围守卫 <c>(Ident &gt;= _CACHE_min) and (Ident &lt;= _CACHE_max)</c>。</summary>
    public static bool IsCacheIdent(int ident) => ident >= MinCacheIdent && ident <= MaxCacheIdent;

    /// <summary>RunGateUtils.pas:1197-1224 —— cache ident → live ident；不在范围内返回 false。</summary>
    public static bool TryRemapCacheToLive(int cacheIdent, out int liveIdent)
    {
        liveIdent = 0;
        if (!IsCacheIdent(cacheIdent)) return false;
        liveIdent = Slots[cacheIdent - MinCacheIdent].LiveIdent;
        return true;
    }

    /// <summary>RunGateUtils.pas:1227 —— <c>FCacheDatas[pDefMsg.Ident - SM_MODULEMD5_CACHE]</c> 的槽位下标。</summary>
    public static int CacheIdentToIndex(int cacheIdent) => cacheIdent - MinCacheIdent;

    /// <summary>校验 14 个 cache ident 严格连续（原文下标算术成立的前提）。</summary>
    public static bool VerifyContiguity()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].Index != i) return false;
            if (Slots[i].CacheIdent != MinCacheIdent + i) return false;
        }
        return true;
    }
}

/// <summary>
/// 网关侧 14 槽配置缓存（RunGateUtils.pas:38 <c>FCacheDatas</c> 的托管等价）。
/// <para>
/// 原文语义：命中缓存分支时**无条件**用缓存内容回发（RunGateUtils.pas:1227-1228），
/// 不检查该槽是否已经装载 → 客户端在 M2 首次推送前就请求会拿到空负载。
/// 这是原设计行为，照原样保留（测试 <c>CacheMiss_ReturnsEmptyButStillSends</c> 固化）。
/// </para>
/// </summary>
public sealed class RunGateDataCache
{
    private readonly byte[][] _data = new byte[RunGateCacheTable.SlotCount][];

    /// <summary>某槽当前装载的字节数（0 表示未装载或装载了 0 字节）。</summary>
    public int LoadedLength(int index)
    {
        if (index < 0 || index >= _data.Length) throw new ArgumentOutOfRangeException(nameof(index));
        return _data[index]?.Length ?? 0;
    }

    /// <summary>RunGateUtils.pas:1167-1168 —— <c>SetLength(FCacheDatas[i], DataAddLen); Move(...)</c>。</summary>
    public void Store(int index, byte[] data, int len)
    {
        if (index < 0 || index >= _data.Length) throw new ArgumentOutOfRangeException(nameof(index));
        if (data == null || len <= 0) { _data[index] = null; return; }
        var copy = new byte[len];
        Array.Copy(data, 0, copy, 0, len);
        _data[index] = copy;
    }

    /// <summary>按 live ident 装载；ident 不在表内返回 false（原文 Index = -1 时不写缓存）。</summary>
    public bool StoreByLiveIdent(int liveIdent, byte[] data, int len)
    {
        int idx = RunGateCacheTable.LiveIdentToIndex(liveIdent);
        if (idx < 0) return false;
        Store(idx, data, len);
        return true;
    }

    /// <summary>RunGateUtils.pas:1227-1228 —— 按 cache ident 取缓存内容（未装载 → 空数组，但仍然回发）。</summary>
    public byte[] ReadByCacheIdent(int cacheIdent)
    {
        if (!RunGateCacheTable.IsCacheIdent(cacheIdent)) return null;
        return _data[RunGateCacheTable.CacheIdentToIndex(cacheIdent)] ?? Array.Empty<byte>();
    }

    public void Clear()
    {
        for (int i = 0; i < _data.Length; i++) _data[i] = null;
    }
}
