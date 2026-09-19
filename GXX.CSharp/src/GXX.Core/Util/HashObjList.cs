using System;
using System.Collections.Generic;

namespace GXX.Core.Util;

/// <summary>
/// HashObjList.pas 1:1 转换：CRC16 采样式哈希 + THashObjectList（名称 → 指针 的桶式哈希表）。
///
/// 覆盖审计（证据见 docs/并行报告-p2c-common-crypto.md）：
///  - 既有 <see cref="THashList"/>（Util/HashList.cs，对应 **HashList.pas**）是另一种算法
///    （哈希 = 逐字符 `(h&lt;&lt;5)^(h&gt;&gt;27)^byte`，并维护全局有序链 + 临界区）；
///    本文件的 THashObjectList 用 **CRC16（初值 $FFFF，多项式 $1021）** 且**无有序链**，
///    两者语义不同 → 属"有差异，需单独移植"，不能互相覆盖；
///  - 既有 <see cref="THashList.CRC16"/> 是"标准逐字节 CRC16"，与原文的**采样式** CRC16
///    （&gt;=32 字节时按 Step := Count div 32 + 1 跳字节）不同 → 本文件单独实现。
///
/// 原文照抄的缺陷（**保留**，见测试差异断言）：
///  1. `CRC16`（HashObjList.pas:85-113）先 `Result := Crc16Start`（$FFFF），
///     最后 2 轮又混入 `OldCRC`；&gt;=32 字节时**只采样** iCount div 32 + 1 个字节；
///  2. `THashObjectList.Add`（HashObjList.pas:205-217）**从不给 Result 赋值** →
///     在 Delphi 里返回 False（本移植返回 false 并注释）；
///  3. `THashObjectList.Modify`（HashObjList.pas:219-231）同样**从不给 Result 赋值**，
///     函数体只是 `P := Find(Name)^`（对 PPHashItem 解引用），实际什么也不改；
///  4. `Find` 返回 `PPHashItem`（指向链表指针的指针），Add 采用**头插**（新项插到桶头）。
/// </summary>
public static class HashObjListGlobal
{
    /// <summary>原文 HashObjList.pas:44 的 TABLE_SIZE。</summary>
    public const int TABLE_SIZE = 256;
}

/// <summary>
/// 原文 HashObjList.pas 的 CRC16 表（$1021 多项式，与 CCITT 一致）与 CRC16 函数。
/// 表由脚本/测试用多项式生成并与原文逐值比对（见 CommonTailTests）。
/// </summary>
public static class HashObjListCrc16
{
    /// <summary>原文 HashObjList.pas:81-83。</summary>
    public const uint Crc16Start = 0xFFFF;
    public const int Crc16Bytes = 2;
    public const int Crc16Bits = 16;

    /// <summary>CRC16 查找表：table[i] = i 的 16 位 CRC（多项式 $1021）。</summary>
    public static readonly ushort[] Crc16Table = BuildTable();

    private static ushort[] BuildTable()
    {
        var table = new ushort[HashObjListGlobal.TABLE_SIZE];
        for (int i = 0; i < 256; i++)
        {
            ushort crc = (ushort)(i << 8);
            for (int j = 0; j < 8; j++)
                crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
            table[i] = crc;
        }
        return table;
    }

    /// <summary>
    /// 对应原文 CRC16(s: PByteArray; iCount: Integer; OldCRC: Word = 0): Word。
    /// iCount &lt; 32 时逐字节；否则按 Step := iCount div 32 + 1 **采样**（原文如此）。
    /// </summary>
    public static ushort CRC16(byte[] s, int iCount, ushort oldCrc = 0)
    {
        ushort result = (ushort)Crc16Start;

        if (iCount < 32)
        {
            for (int i = 0; i < iCount; i++)
                result = (ushort)(Crc16Table[result >> (Crc16Bits - 8)] ^ (ushort)(result << 8) ^ s[i]);
        }
        else
        {
            int step = iCount / 32 + 1;
            int i = 0;
            int decCount = iCount - 1;
            while (i < decCount)
            {
                result = (ushort)(Crc16Table[result >> (Crc16Bits - 8)] ^ (ushort)(result << 8) ^ s[i]);
                i += step;
            }
        }

        for (int i = 0; i < Crc16Bytes; i++)
        {
            result = (ushort)(Crc16Table[result >> (Crc16Bits - 8)] ^ (ushort)(result << 8) ^ (oldCrc >> (Crc16Bits - 8)));
            oldCrc = (ushort)(oldCrc << 8);
        }
        return result;
    }

    /// <summary>
    /// 对应原文 HashIndex(Value: Integer): Integer —— `CRC16(IntToStr(Value), Length, 0) mod 2000`。
    /// 注意原文把 IntToStr 的结果当 PByteArray 传（AnsiString 的字节），本移植按 GBK 字节还原。
    /// </summary>
    public static int HashIndex(int value)
    {
        byte[] s = GXX.Core.EncodingInit.GBK.GetBytes(GXX.Core.Rtl.DelphiRTL.IntToStr(value));
        return CRC16(s, s.Length, 0) % 2000;
    }
}

/// <summary>
/// 原文 HashObjList.pas 的 THashObjectList：名称 → 对象 的桶式哈希表（头插 + CRC16 哈希）。
/// Delphi 的 Pointer 在此映射为 object?（托管引用）。
/// </summary>
public class THashObjectList
{
    /// <summary>原文 PHashItem/THashItem：单向链节点（Next + Key + Item）。</summary>
    public sealed class THashItem
    {
        public THashItem? Next;
        public string Key = "";
        public object? Item;
    }

    private readonly int _size;
    private readonly THashItem?[] _buckets;
    private int _recordCount;

    /// <summary>对应原文 constructor THashObjectList.Create(Size: Integer = 256)。</summary>
    public THashObjectList(int size = 256)
    {
        _size = size;
        _buckets = new THashItem?[size];
        _recordCount = 0;
    }

    /// <summary>原文 property MaxCount: Integer read GetCount —— 返回 Buckets 的长度。</summary>
    public int MaxCount => _buckets.Length;

    /// <summary>原文 property Count: Integer read FRecordCount。</summary>
    public int Count => _recordCount;

    /// <summary>原文 property Objects[Index]: Pointer read Get write Put。</summary>
    public object? this[int index]
    {
        get => _buckets[index] != null ? _buckets[index]!.Item : null;
        set
        {
            if (_buckets[index] != null)
                _buckets[index]!.Item = value;
        }
    }

    /// <summary>原文 virtual function HashOf(const Name: string): Cardinal —— CRC16(Name)。</summary>
    public virtual uint HashOf(string name)
    {
        byte[] s = GXX.Core.EncodingInit.GBK.GetBytes(name ?? "");
        return HashObjListCrc16.CRC16(s, s.Length, 0);
    }

    /// <summary>原文 IndexOf(const Name): Integer = HashOf(Name) mod Length(Buckets)。</summary>
    public int IndexOf(string name)
        => (int)(HashOf(name) % (uint)_buckets.Length);

    /// <summary>
    /// 原文 protected function Find(const Name: string): PPHashItem —— 返回**指向链指针的引用**。
    /// C# 无指针引用返回，这里返回 (bucketIndex, previousNode) 二元组：
    /// previousNode == null 表示命中的是桶头（对应原文 @Buckets[Hash]）。
    /// </summary>
    protected (int Bucket, THashItem? Prev, THashItem? Node) Find(string name)
    {
        int hash = (int)(HashOf(name) % (uint)_buckets.Length);
        THashItem? prev = null;
        THashItem? cur = _buckets[hash];
        while (cur != null)
        {
            if (cur.Key == name)
                return (hash, prev, cur);
            prev = cur;
            cur = cur.Next;
        }
        return (hash, prev, null);
    }

    /// <summary>
    /// 原文 function Add(const Name: string; Item: Pointer): Boolean —— **头插**。
    /// 原文从不给 Result 赋值（HashObjList.pas:205-217）→ Delphi 下恒返回 False；
    /// 本移植照抄该行为并返回 false（测试差异断言锁定）。
    /// </summary>
    public bool Add(string name, object? item)
    {
        int hash = (int)(HashOf(name) % (uint)_buckets.Length);
        var bucket = new THashItem { Key = name, Next = _buckets[hash], Item = item };
        _buckets[hash] = bucket;
        _recordCount++;
        return false;   // 原文如此：Result 从未被赋值
    }

    /// <summary>
    /// 原文 function Modify(const Name: string; Value: string): Boolean —— 函数体只有
    /// `P := Find(Name)^`，既不写 Value 也不给 Result 赋值 → Delphi 下恒返回 False 且**无副作用**。
    /// 本移植照抄：只查找，不修改，返回 false。
    /// </summary>
    public bool Modify(string name, string value)
    {
        _ = Find(name);
        return false;   // 原文如此：Result 从未被赋值
    }

    /// <summary>原文 procedure Delete(Index: Integer)：删掉该桶的**头节点**并 Dec(FRecordCount)。</summary>
    public void Delete(int index)
    {
        var p = _buckets[index];
        if (p != null)
        {
            _buckets[index] = p.Next;
            _recordCount--;
        }
    }

    /// <summary>原文 procedure Remove(const Name: string)：找到后从链上摘除并 Dec(FRecordCount)。</summary>
    public void Remove(string name)
    {
        var (bucket, prev, node) = Find(name);
        if (node == null) return;
        if (prev == null)
            _buckets[bucket] = node.Next;
        else
            prev.Next = node.Next;
        _recordCount--;
    }

    /// <summary>原文 procedure Clear：释放所有节点、桶置 nil、FRecordCount := 0。</summary>
    public void Clear()
    {
        _recordCount = 0;
        for (int i = 0; i < _buckets.Length; i++)
            _buckets[i] = null;
    }

    /// <summary>测试辅助：返回桶内链长（用于验证头插顺序）。</summary>
    public int BucketChainLength(int bucket)
    {
        int n = 0;
        for (var p = _buckets[bucket]; p != null; p = p.Next) n++;
        return n;
    }

    /// <summary>测试辅助：按插入顺序（从桶头往后）枚举某桶的 Key。</summary>
    public List<string> BucketKeys(int bucket)
    {
        var list = new List<string>();
        for (var p = _buckets[bucket]; p != null; p = p.Next) list.Add(p.Key);
        return list;
    }
}
