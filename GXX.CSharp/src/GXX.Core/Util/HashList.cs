using System;
using System.Collections.Generic;
using System.Threading;

namespace GXX.Core.Util;

/// <summary>
/// HashList.pas 1:1 语义转换：字符串键 → 指针 的桶式哈希表（含有序链 + 线程锁）。
/// Delphi 指针 Item → object（托管引用）；PHashItem → THashItem。
/// </summary>
public class THashList : IDisposable
{
    public class THashItem
    {
        public string Key = "";
        public object? Item;
        public THashItem? Prev;
        public THashItem? Next;
    }

    private readonly THashItem?[] _buckets;
    private THashItem? _first;
    private THashItem? _last;
    private int _count;
    private readonly object _critical = new();

    public THashList(uint size = 256)
    {
        uint real = 1;
        while (real < size) real <<= 1;
        _buckets = new THashItem[real];
    }

    public THashItem? First => _first;
    public THashItem? Last => _last;
    public int Count => _count;

    public static uint HashOf8(string key)
    {
        uint hash = 0;
        foreach (char c in key)
            hash = ((hash << 4) ^ (hash >> 28) ^ (uint)(c & 0xFF)) & 0xFFFFFFFF;
        return hash & 0xFF;
    }

    public static uint HashOf16(string key)
    {
        uint hash = 0;
        foreach (char c in key)
            hash = ((hash << 5) ^ (hash >> 27) ^ (uint)(c & 0xFF)) & 0xFFFFFFFF;
        return hash & 0xFFFF;
    }

    public static uint HashOf32(string key)
    {
        uint hash = 0;
        foreach (char c in key)
            hash = ((hash << 5) ^ (hash >> 27) ^ (uint)(c & 0xFF)) & 0xFFFFFFFF;
        return hash;
    }

    protected uint HashOf(string key) => HashOf32(key);

    public bool TryLock() => Monitor.TryEnter(_critical);
    public void Lock() => Monitor.Enter(_critical);
    public void UnLock() => Monitor.Exit(_critical);

    public void Add(string key, object item)
    {
        lock (_critical)
        {
            var item_ = new THashItem { Key = key, Item = item };
            uint bucket = HashOf(key) & (uint)(_buckets.Length - 1);
            item_.Next = _buckets[bucket];
            if (_buckets[bucket] != null) _buckets[bucket].Prev = item_;
            _buckets[bucket] = item_;
            item_.Prev = null;
            // 追加到全局有序链尾部
            if (_last != null) { _last.Next = item_; item_.Prev = _last; }
            else _first = item_;
            _last = item_;
            _count++;
        }
    }

    public void Insert(string key, object item) => Add(key, item);

    public THashItem? GetKey(string key)
    {
        uint bucket = HashOf(key) & (uint)(_buckets.Length - 1);
        var item = _buckets[bucket];
        while (item != null)
        {
            if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase)) return item;
            item = item.Next;
        }
        return null;
    }

    public object? Find(string key) => GetKey(key)?.Item;

    public int IndexOf(object item)
    {
        int i = 0;
        var it = _first;
        while (it != null)
        {
            if (ReferenceEquals(it.Item, item)) return i;
            it = it.Next;
            i++;
        }
        return -1;
    }

    public void Remove(string key)
    {
        var item = GetKey(key);
        if (item != null) RemoveItem(item);
    }

    private void RemoveItem(THashItem item)
    {
        uint bucket = HashOf(item.Key) & (uint)(_buckets.Length - 1);
        if (item.Prev != null) item.Prev.Next = item.Next;
        else _buckets[bucket] = item.Next;
        if (item.Next != null) item.Next.Prev = item.Prev;
        if (ReferenceEquals(_first, item)) _first = item.Next;
        if (ReferenceEquals(_last, item)) _last = item.Prev;
        _count--;
    }

    public bool Modify(string key, object item)
    {
        var it = GetKey(key);
        if (it == null) return false;
        it.Item = item;
        return true;
    }

    public bool Exists(string key) => GetKey(key) != null;

    public int ExistsPos(string key) => IndexOf(Find(key));

    public void Clear()
    {
        Array.Clear(_buckets, 0, _buckets.Length);
        _first = null;
        _last = null;
        _count = 0;
    }

    public void Dispose() => Clear();

    // ---- CRC 系列（UnitHash/CRC 帮助函数）----

    public static byte CRC8(string s)
    {
        byte crc = 0;
        foreach (char ch in s)
        {
            crc ^= (byte)(ch & 0xFF);
            for (int i = 0; i < 8; i++)
                crc = (crc & 1) != 0 ? (byte)((crc >> 1) ^ 0x8C) : (byte)(crc >> 1);
        }
        return crc;
    }

    public static ushort CRC16(byte[] s, int iCount, ushort oldCrc = 0)
    {
        ushort crc = oldCrc;
        for (int i = 0; i < iCount; i++)
        {
            crc ^= (ushort)(s[i] << 8);
            for (int j = 0; j < 8; j++)
                crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
        }
        return crc;
    }
}

/// <summary>HashTable.pas：键值双向泛型哈希（Delphi THashTable 语义）。</summary>
public class THashTable<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _map = new();

    public int Count => _map.Count;

    public void Put(TKey key, TValue value) => _map[key] = value;

    public TValue? Get(TKey key) => _map.TryGetValue(key, out var v) ? v : default;

    public bool Contains(TKey key) => _map.ContainsKey(key);

    public void Remove(TKey key) => _map.Remove(key);

    public void Clear() => _map.Clear();

    public IEnumerable<KeyValuePair<TKey, TValue>> Entries => _map;
}
