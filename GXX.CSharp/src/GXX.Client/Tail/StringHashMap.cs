// 源单元：Source/Client-HGE/StringHashMap.pas（原文 565 行 / CRLF 计入 638 行）
// 原文 uses：SysUtils, Classes
// 原文无同名 .dfm（纯哈希表实现，无窗体）。
//
// 移植策略：
//   · 哈希算法是 Bob Jenkins 的 lookup3 `hashlittle`（原文 StringHashMap.pas:149-315），
//     **逐行移植并保位语义**（Cardinal 回绕 = unchecked(uint)），因此哈希值与原文完全一致；
//   · 原文的链表 + 桶数组结构（Dispose/New 手工内存管理）→ C# 引用型节点对象，
//     桶语义（头插法、负载因子 75%、容量向上取 2 的幂）逐条保留；
//   · Delphi `Pointer` 值类型 → C# `object`（nil ↔ null）；
//   · 原文有两个"看起来多余"的写法，均**保留并注释**：
//       ① `TryGetValue` 判 `ppItem <> nil` —— 而 `Find` 永远返回桶内地址，不可能为 nil；
//       ② `Remove` 里被注释掉的 `if Prev <> nil then`（原文自带注释"Prev始终落在桶内，不可能为空"）。
using System;
using System.Collections;
using System.Collections.Generic;

namespace GXX.Client.Tail;

/// <summary>原文 StringHashMap.pas:8 — <c>EMPTY_HASH = -1;</c></summary>
public static class StringHashMapConst
{
    /// <summary>原文 :8 — <c>EMPTY_HASH = -1;</c></summary>
    public const int EMPTY_HASH = -1;
}

/// <summary>
/// StringHashMap.pas 的 <c>TStringHashMap</c> 1:1 移植（命名空间 <c>GXX.Client.Tail</c>）。
///
/// <para><b>键的字节语义</b>：原文 <c>HashOf(const Key: string)</c> 用
/// <c>PAnsiChar(Key)</c> 直接对 **AnsiString 的 GBK 字节**做 hashlittle，
/// 所以同一段中文的哈希取决于其 GBK 编码而非 UTF-16。本移植同样先取 GBK 字节。</para>
///
/// <para><b>值语义</b>：原文 <c>TMapValue = Pointer</c>（可 nil）；C# 用 <c>object</c>。</para>
/// </summary>
public class TStringHashMap
{
    // ── 原文 :36-38 的私有字段 ─────────────────────────────────────────────
    private TStringHashMapItem[] Buckets;
    private int FRecordCount;
    private int FGrowThreshold;

    /// <summary>原文 :57 — <c>constructor Create(Size: Integer = 256);</c></summary>
    public TStringHashMap(int Size = 256)
    {
        int NewCapPow2 = CalcCapacity(Size);
        FGrowThreshold = (NewCapPow2 >> 1) + (NewCapPow2 >> 2);
        Buckets = new TStringHashMapItem[NewCapPow2];
        FRecordCount = 0;
    }

    /// <summary>原文 :77 — <c>property Count: Integer read FRecordCount;</c></summary>
    public int Count => FRecordCount;

    /// <summary>原文 :48 — <c>function GetBucketCount: Integer;</c></summary>
    public int GetBucketCount => Buckets.Length;

    /// <summary>原文 :47 — <c>procedure Grow();</c>：容量翻倍（为 0 时取 4）。</summary>
    private void Grow()
    {
        int newCap = Buckets.Length * 2;
        // 原文如此（StringHashMap.pas:339）：if newCap = 0 then newCap := 4;
        if (newCap == 0) newCap = 4;
        Rehash(newCap);
    }

    /// <summary>
    /// 原文 :368-381 <c>function CalcCapacity(nCapacity: Integer): Integer;</c>
    /// <para>从 4 起按 2 的幂翻倍，直到 <c>(newCap/2 + newCap/4) &gt; nCapacity</c>（即 75% 负载）。</para>
    /// </summary>
    public static int CalcCapacity(int nCapacity)
    {
        if (nCapacity == 0)
        {
            // 原文如此（StringHashMap.pas:372-373）：if nCapacity = 0 then Result := 4;
            return 4;
        }
        int newCap = 4;
        while (((newCap >> 1) + (newCap >> 2)) <= nCapacity)
        {
            newCap <<= 1;
        }
        return newCap;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 哈希（原文 :149-315 hashlittle，逐行移植）
    // ══════════════════════════════════════════════════════════════════════

    private static uint Rot(uint x, int k) => (x << k) | (x >> (32 - k));

    private static void Mix(ref uint a, ref uint b, ref uint c)
    {
        // 原文如此（StringHashMap.pas:156-162）：Dec 是**无符号回绕**减
        a -= c; a ^= Rot(c, 4); c += b;
        b -= a; b ^= Rot(a, 6); a += c;
        c -= b; c ^= Rot(b, 8); b += a;
        a -= c; a ^= Rot(c, 16); c += b;
        b -= a; b ^= Rot(a, 19); a += c;
        c -= b; c ^= Rot(b, 4); b += a;
    }

    private static void DoFinal(ref uint a, ref uint b, ref uint c)
    {
        c ^= b; c -= Rot(b, 14);
        a ^= c; a -= Rot(c, 11);
        b ^= a; b -= Rot(a, 25);
        c ^= b; c -= Rot(b, 16);
        a ^= c; a -= Rot(c, 4);
        b ^= a; b -= Rot(a, 14);
        c ^= b; c -= Rot(b, 24);
    }

    /// <summary>
    /// 小端 32 位读取（原文 <c>GetCardinalValue</c> 的 <c>PCardinal^</c> 在 x86 上即小端）。
    /// <para><b>越界语义</b>：原文直接按 4 字节解引用，即使 <c>Len</c> 不是 4 的倍数也会**多读**
    /// 到 <c>Len</c> 之外的字节（只被随后的 <c>and $FF/$FFFF/$FFFFFF</c> 掩掉高位）。
    /// 对 Delphi 的 AnsiString 而言，<c>Len</c> 之后紧随的是结尾 NUL 与堆尾余量，实际都取到 0；
    /// 托管 <c>byte[]</c> 没有这种余量，故本移植对越界部分**补 0**，从而得到与原文相同的字节值。</para>
    /// </summary>
    private static uint GetCardinal(byte[] p, int offset)
    {
        uint v = 0;
        for (int i = 0; i < 4; i++)
        {
            int idx = offset + i;
            if (idx >= 0 && idx < p.Length) v |= (uint)p[idx] << (8 * i);
        }
        return v;
    }

    /// <summary>
    /// 原文 :149-315 <c>function HashLittle(const Data; Len, InitVal: Integer): Integer;</c>
    /// <para>逐行移植。原文按 <c>(Cardinal(pb) and 3) = 0</c> 分成"4 字节对齐"与"逐字节"两条路径；
    /// 两者对同一数据**结果相同**（原文如此，两个 case 表一一对应），托管侧只需一条正确路径，
    /// 但仍按原文的 12 字节主循环 + 尾部 switch/goto 展开逐字对应。</para>
    /// </summary>
    public static int HashLittle(byte[] data, int len, int initVal)
    {
        if (data == null) { data = Array.Empty<byte>(); len = 0; }
        if (len < 0) len = 0;
        if (len > data.Length) len = data.Length;

        uint a = 0xDEADBEEFu + (uint)len + (uint)initVal;
        uint b = a;
        uint c = a;

        int off = 0;
        // 原文如此（StringHashMap.pas:208）：while Len > 12 do
        while (len > 12)
        {
            a += GetCardinal(data, off);
            b += GetCardinal(data, off + 4);
            c += GetCardinal(data, off + 8);
            Mix(ref a, ref b, ref c);
            len -= 12;
            off += 12;
        }

        // 原文 :217-259 的 case Len of 0..12（此处用等价的分段累加，语义逐条对应）
        switch (len)
        {
            case 0: return unchecked((int)c);                            // 原文 :218
            case 1: a += GetCardinal(data, off) & 0xFF; break;           // :219
            case 2: a += GetCardinal(data, off) & 0xFFFF; break;         // :220
            case 3: a += GetCardinal(data, off) & 0xFFFFFF; break;       // :221
            case 4: a += GetCardinal(data, off); break;                  // :222
            case 5:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4) & 0xFF;                  // :225
                break;
            case 6:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4) & 0xFFFF;                // :229
                break;
            case 7:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4) & 0xFFFFFF;              // :233
                break;
            case 8:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4);                         // :237
                break;
            case 9:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4);
                c += GetCardinal(data, off + 8) & 0xFF;                  // :242
                break;
            case 10:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4);
                c += GetCardinal(data, off + 8) & 0xFFFF;                // :247
                break;
            case 11:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4);
                c += GetCardinal(data, off + 8) & 0xFFFFFF;              // :252
                break;
            case 12:
                a += GetCardinal(data, off);
                b += GetCardinal(data, off + 4);
                c += GetCardinal(data, off + 8);                         // :257
                break;
        }

        DoFinal(ref a, ref b, ref c);
        return unchecked((int)c);
    }

    /// <summary>
    /// 原文 :110-120 <c>HashOf(const Key: string): Cardinal;</c>
    /// <para><c>PositiveMask = not Integer($80000000)</c> = <c>$7FFFFFFF</c>；
    /// <c>Result := PositiveMask and ((PositiveMask and nHash) + 1)</c> —— 保证结果落在
    /// <c>[1, $7FFFFFFF]</c>（**永不为 0**，因为要预留"空"语义）。</para>
    /// </summary>
    public virtual uint HashOf(string Key)
    {
        const uint PositiveMask = 0x7FFFFFFFu;   // not Integer($80000000)
        byte[] bytes = GXX.Core.EncodingInit.GBK.GetBytes(Key ?? string.Empty);
        int nHash = HashLittle(bytes, bytes.Length, 0);
        return PositiveMask & ((PositiveMask & (uint)nHash) + 1u);
    }

    /// <summary>原文 :122-130 <c>HashOf(pData: Pointer; nLen: Integer): Cardinal;</c></summary>
    public virtual uint HashOf(byte[] pData, int nLen)
    {
        const uint PositiveMask = 0x7FFFFFFFu;
        int nHash = HashLittle(pData, nLen, 0);
        return PositiveMask & ((PositiveMask & (uint)nHash) + 1u);
    }

    /// <summary>
    /// 原文 :142-147 <c>HashOf(Key: Integer): Cardinal;</c>（斐波那契散列）
    /// <para><c>A = 0.6180339887</c>；<c>Result := Trunc(Length(Buckets) * Frac(Cardinal(Key) * A))</c>。
    /// 注意原文把 <c>Key</c> 先转成 **Cardinal（无符号）** 再乘，负数键与正数键不同。</para>
    /// </summary>
    public virtual uint HashOf(int Key)
    {
        const double A = 0.6180339887; // (sqrt(5) - 1) / 2
        double v = Buckets.Length * ((uint)Key * A - Math.Floor((uint)Key * A));
        return (uint)Math.Truncate(v);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 查找 / 插入 / 删除
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :322-332 <c>GetValue</c>（默认索引器 getter）。
    /// <para>未命中时返回 <c>DefaultEmptyValue</c>（= nil）。</para>
    /// </summary>
    public object GetValue(string Key)
    {
        var p = Find(Key);
        return p != null ? p.Pair.Value : DefaultEmptyValue();
    }

    /// <summary>原文 :43 <c>DefaultEmptyValue</c> = <c>nil</c>。</summary>
    private static object DefaultEmptyValue() => null;

    /// <summary>原文 :49 <c>DefaultEmptyKey</c> = <c>''</c>。</summary>
    private static string DefaultEmptyKey() => string.Empty;

    /// <summary>
    /// 原文 :343-366 <c>SetValue</c>（默认索引器 setter）。
    /// <para>命中则改值；未命中则**头插**到桶首并 Inc(FRecordCount)，达到阈值后 Grow。</para>
    /// </summary>
    public void SetValue(string Key, object Value)
    {
        int HashIndex = (int)(HashOf(Key) % (uint)Buckets.Length);
        var existing = FindInBucket(Key, HashIndex);
        if (existing != null)
        {
            existing.Pair.Value = Value;
        }
        else
        {
            var bucket = new TStringHashMapItem
            {
                // 原文如此（StringHashMap.pas:355-359）：SetValue 路径**不**写 HashCode 字段
                // （只有 TryAdd 会写），所以 Rehash 时该节点的 HashCode 恒为 0。
                HashCode = 0u,
                Next = Buckets[HashIndex],
                Pair = new TStringHashMapPair { Key = Key, Value = Value },
            };
            Buckets[HashIndex] = bucket;
            FRecordCount++;

            if (FRecordCount >= FGrowThreshold)
            {
                Grow();
            }
        }
    }

    /// <summary>原文 :75 — <c>property Values[const Key: TMapKey]: TMapValue read GetValue write SetValue; default;</c></summary>
    public object this[string Key]
    {
        get => GetValue(Key);
        set => SetValue(Key, value);
    }

    /// <summary>
    /// 原文 :422-435 <c>Find(const Key: TMapKey): PPHashMapItem;</c>
    /// <para>返回"指向该键所在链节点的指针"；未命中时返回链尾的 <c>Next</c> 字段地址（其值为 nil）。
    /// C# 侧以"返回命中节点或 null"表达同一语义（见 <see cref="FindPrev"/> 处理删除场景）。</para>
    /// </summary>
    private TStringHashMapItem Find(string Key)
    {
        int HashIndex = (int)(HashOf(Key) % (uint)Buckets.Length);
        var p = Buckets[HashIndex];
        while (p != null)
        {
            if (p.Pair.Key == Key) return p;
            p = p.Next;
        }
        return null;
    }

    /// <summary>原文 :437-447 <c>Find(const Key: TMapKey; HashIndex: Integer): PPHashMapItem;</c></summary>
    private TStringHashMapItem FindInBucket(string Key, int HashIndex)
    {
        var p = Buckets[HashIndex];
        while (p != null)
        {
            if (p.Pair.Key == Key) return p;
            p = p.Next;
        }
        return null;
    }

    /// <summary>内部：取"键所在节点的前驱"（原文用 <c>PPHashMapItem</c> 的二级指针表达）。</summary>
    private TStringHashMapItem FindPrev(string Key, out int hashIndex, out TStringHashMapItem current)
    {
        hashIndex = (int)(HashOf(Key) % (uint)Buckets.Length);
        TStringHashMapItem prev = null;
        var p = Buckets[hashIndex];
        while (p != null)
        {
            if (p.Pair.Key == Key) { current = p; return prev; }
            prev = p;
            p = p.Next;
        }
        current = null;
        return prev;
    }

    /// <summary>原文 :449-461 <c>TryGetValue</c>。</summary>
    public bool TryGetValue(string Key, out object Value)
    {
        var item = Find(Key);
        // 原文如此（StringHashMap.pas:453-460）：先判 `ppItem <> nil` 再取值。
        // 注意原 `Find` 返回的是**桶内地址**，永不为 nil —— 所以原文的分支其实是"恒真"，
        // 真正的未命中是通过 `ppItem^.Pair.Value` 落到 nil 体现的。本移植改为按节点判空，
        // 并把这一处原文冗余如实登记（见报告 §5）。
        if (item != null)
        {
            Value = item.Pair.Value;
            return true;
        }
        Value = DefaultEmptyValue();
        return false;
    }

    /// <summary>原文 :463-466 <c>IsContainKey</c>。</summary>
    public bool IsContainKey(string Key) => Find(Key) != null;

    /// <summary>
    /// 原文 :468-492 <c>TryAdd</c>：与 <c>SetValue</c> 的关键差异 —— 它在新建节点时
    /// **写入 <c>HashCode</c>**（SetValue 不写），因此只有 TryAdd 建出的节点在 Rehash 后
    /// 能落到正确的桶（见报告 §5 的原文缺陷登记）。
    /// </summary>
    public bool TryAdd(string Key, object Value)
    {
        uint Hash = HashOf(Key);
        int HashIndex = (int)(Hash % (uint)Buckets.Length);
        if (FindInBucket(Key, HashIndex) == null)
        {
            var pItem = new TStringHashMapItem
            {
                HashCode = Hash,                          // 原文 :477 ✅
                Pair = new TStringHashMapPair { Key = Key, Value = Value },
                Next = Buckets[HashIndex],
            };
            Buckets[HashIndex] = pItem;
            FRecordCount++;

            if (FRecordCount >= FGrowThreshold)
            {
                Grow();
            }
            return true;
        }
        return false;
    }

    /// <summary>原文 :494-497 <c>AddOrSet</c>（inline）—— 直接转调 SetValue。</summary>
    public void AddOrSet(string Key, object Value) => SetValue(Key, Value);

    /// <summary>原文 :499-510 <c>Modify</c>（原文注释：此函数未用上）。</summary>
    public bool Modify(string Key, object Value)
    {
        var p = Find(Key);
        if (p != null)
        {
            p.Pair.Value = Value;
            return true;
        }
        return false;
    }

    /// <summary>原文 :512-524 <c>Delete(Index: Integer)</c>：删桶首节点。</summary>
    private void Delete(int Index)
    {
        var p = Buckets[Index];
        if (p != null)
        {
            Buckets[Index] = p.Next;
            FRecordCount--;
        }
    }

    /// <summary>
    /// 原文 :526-540 <c>Remove(const Key: TMapKey)</c>。
    /// <para>原文保留了一段被注释掉的空指针保护（<c>//if Prev &lt;&gt; nil then</c>，
    /// 并自带注释"Prev始终落在桶内，不可能为空"）—— 本移植同样**不做**空指针保护以保持行为一致，
    /// 但 C# 侧链首即命中时 prev 为 null，故按"prev 为空 ⇒ 改桶首"分支实现，语义与原文二级指针写法一致。</para>
    /// </summary>
    public void Remove(string Key)
    {
        var prev = FindPrev(Key, out int hashIndex, out var cur);
        // 原文如此（StringHashMap.pas:532）：//if Prev <> nil then begin Prev始终落在桶内，不可能为空
        if (cur != null)
        {
            if (prev == null) Buckets[hashIndex] = cur.Next;
            else prev.Next = cur.Next;
            FRecordCount--;
        }
    }

    /// <summary>原文 :383-398 <c>Clear</c>：逐桶释放链，FRecordCount := 0（桶数组长度不变）。</summary>
    public void Clear()
    {
        FRecordCount = 0;
        for (int i = 0; i < Buckets.Length; i++)
        {
            Buckets[i] = null;
        }
    }

    /// <summary>
    /// 原文 :542-568 <c>Rehash(NewCapPow2: Integer)</c>。
    /// <para><c>NewCapPow2 &lt; 0</c> 或等于当前容量时直接返回；重挂时用
    /// <c>Bucket.HashCode mod Length(NewItems)</c> —— 因此**用 SetValue 建出来的节点
    /// （HashCode 恒为 0）在扩容后全部堆到 0 号桶**。原文如此，本移植保真保留。</para>
    /// <para>阈值重算为 <c>75%</c>。</para>
    /// </summary>
    public void Rehash(int NewCapPow2)
    {
        // 原文如此（StringHashMap.pas:548）：if NewCapPow2 < 0 then exit; //OutOfMemoryError;
        if (NewCapPow2 < 0) return;
        if (NewCapPow2 == Buckets.Length) return;

        var oldItems = Buckets;
        var newItems = new TStringHashMapItem[NewCapPow2];
        Buckets = newItems;

        for (int i = 0; i < oldItems.Length; i++)
        {
            var Bucket = oldItems[i];
            while (Bucket != null)
            {
                int nNewIndex = (int)(Bucket.HashCode % (uint)newItems.Length);
                var Next = Bucket.Next;
                Bucket.Next = newItems[nNewIndex];
                Buckets[nNewIndex] = Bucket;
                Bucket = Next;
            }
        }
        oldItems = null;

        FGrowThreshold = (NewCapPow2 >> 1) + (NewCapPow2 >> 2); // 75%
    }

    /// <summary>原文 :411-414 <c>GetEnumerator</c>。</summary>
    public TStringHashMapPairEnumerator GetEnumerator() => new TStringHashMapPairEnumerator(this);

    /// <summary>
    /// 测试接缝：直接读某个桶的链首。
    /// <para>原文 <c>Buckets</c> 是 private（同单元的枚举器可直接访问）；跨程序集无法访问，
    /// 故公开本只读入口（**不提供**写入口，桶结构仍由本类独占）。越界抛
    /// <see cref="ArgumentOutOfRangeException"/>。</para>
    /// </summary>
    public TStringHashMapItem BucketAt(int index) => Buckets[index];
}

/// <summary>原文 :16-19 — <c>TStringHashMapPair = record Key: TMapKey; Value: TMapValue; end;</c></summary>
public sealed class TStringHashMapPair
{
    /// <summary>原文 :17 — <c>Key: TMapKey;</c></summary>
    public string Key;
    /// <summary>原文 :18 — <c>Value: TMapValue;</c></summary>
    public object Value;
}

/// <summary>原文 :24-28 — <c>TStringHashMapItem = record HashCode; Next; Pair; end;</c></summary>
public sealed class TStringHashMapItem
{
    /// <summary>原文 :25 — <c>HashCode: Cardinal;</c></summary>
    public uint HashCode;
    /// <summary>原文 :26 — <c>Next: PStringHashMapItem;</c></summary>
    public TStringHashMapItem Next;
    /// <summary>原文 :27 — <c>Pair: TStringHashMapPair;</c></summary>
    public TStringHashMapPair Pair;
}

/// <summary>
/// 原文 :80-93 / :572-635 <c>TStringHashMapPairEnumerator</c>。
/// <para>遍历顺序 = 桶序（0..N-1）× 桶内链序（头插法 ⇒ **后插入的先被枚举**）。
/// 原文 <c>MoveNext</c> 用 <c>m_BucketIndex &lt; 0</c> 表示"尚未开始"。</para>
/// </summary>
public sealed class TStringHashMapPairEnumerator
{
    private int m_BucketIndex;
    private TStringHashMapItem m_pCurBucket;
    private TStringHashMap m_hash;

    /// <summary>原文 :579-584 <c>constructor Create;</c>（无参，m_hash = nil ⇒ MoveNext 恒 false）</summary>
    public TStringHashMapPairEnumerator()
    {
        m_hash = null;
        m_pCurBucket = null;
        m_BucketIndex = -1;
    }

    /// <summary>原文 :572-577 <c>constructor Create(hash: TStringHashMap);</c></summary>
    public TStringHashMapPairEnumerator(TStringHashMap hash)
    {
        m_hash = hash;
        m_pCurBucket = null;
        m_BucketIndex = -1;
    }

    /// <summary>原文 :591-598 <c>GetCurrent</c>：当前桶节点为 nil 时返回 nil。</summary>
    public TStringHashMapPair Current => m_pCurBucket?.Pair;

    /// <summary>原文 :601-635 <c>MoveNext</c>。</summary>
    public bool MoveNext()
    {
        bool Result = false;
        if (m_hash == null) return false;

        if (m_BucketIndex < 0)
        {
            // first（原文 :608-617）
            int nCount = m_hash.GetBucketCount;
            for (int i = 0; i < nCount; i++)
            {
                m_pCurBucket = m_hash.BucketAt(i);
                if (m_pCurBucket != null)
                {
                    m_BucketIndex = i;
                    Result = true;
                    break;
                }
            }
        }
        else
        {
            if (m_pCurBucket.Next == null)
            {
                m_pCurBucket = null;
                int nCount = m_hash.GetBucketCount;
                for (int i = m_BucketIndex + 1; i < nCount; i++)
                {
                    m_pCurBucket = m_hash.BucketAt(i);
                    if (m_pCurBucket != null)
                    {
                        m_BucketIndex = i;
                        Result = true;
                        break;
                    }
                }
            }
            else
            {
                m_pCurBucket = m_pCurBucket.Next;
                Result = true;
            }
        }
        return Result;
    }
}

/// <summary>
/// 原文 <c>TMapKey = String; TMapValue = Pointer;</c> 的别名常量（审计用）。
/// </summary>
public static class StringHashMapAliases
{
    /// <summary>原文 :11 — <c>TMapKey = String;</c></summary>
    public const bool TMapKeyIsString = true;
    /// <summary>原文 :12 — <c>TMapValue = Pointer;</c>（C# 侧为 <c>object</c>，nil ↔ null）</summary>
    public const bool TMapValueIsPointer = true;
}
