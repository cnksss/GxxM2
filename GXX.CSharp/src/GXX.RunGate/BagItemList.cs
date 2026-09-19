using System;
using System.Collections.Generic;
using System.Threading;

namespace GXX.RunGate;

/// <summary>TBagItem（BagItemList.pas packed 记录）。</summary>
public class TBagItem
{
    public int MakeIndex;
    public byte StdMode;
    public byte Shape;
    public ushort Reserved;
    public int AC1;
    public int MAC1;
}

/// <summary>
/// BagItemList.pas TBagItemList 1:1：按 MakeIndex 有序的背包物品索引表
/// （二分查找 + 插入排序语义 + 临界锁 + 可选重复项）。
/// </summary>
public class TBagItemList
{
    private readonly List<TBagItem> _list = new();
    private readonly object _locker = new();
    private bool _duplicates;

    public TBagItemList(int capacity)
    {
        _list.Capacity = Math.Max(capacity, 0);
    }

    public int Count
    {
        get { lock (_locker) return _list.Count; }
    }

    public int Capacity
    {
        get { lock (_locker) return _list.Capacity; }
        set { lock (_locker) _list.Capacity = value; }
    }

    public bool Duplicates { get => _duplicates; set => _duplicates = value; }

    public void Lock() => System.Threading.Monitor.Enter(_locker);
    public void UnLock() => System.Threading.Monitor.Exit(_locker);

    /// <summary>二分查找：返回是否存在与插入位置（原 Search 语义）。</summary>
    private bool Search(int makeIndex, out int index)
    {
        int lo = 0, hi = _list.Count - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            int midIdx = _list[mid].MakeIndex;
            if (makeIndex == midIdx) { index = mid; return true; }
            if (makeIndex < midIdx) hi = mid - 1;
            else lo = mid + 1;
        }
        index = lo;
        return false;
    }

    /// <summary>Add：已存在（且不允许重复）→ 返回原项；否则插入有序位置。</summary>
    public TBagItem Add(int makeIndex)
    {
        lock (_locker)
        {
            if (!_duplicates && Search(makeIndex, out int idx))
                return _list[idx];
            var item = new TBagItem { MakeIndex = makeIndex };
            Search(makeIndex, out int insertPos);
            _list.Insert(_duplicates ? Count : insertPos, item);
            return item;
        }
    }

    public TBagItem? Find(int makeIndex)
    {
        lock (_locker)
        {
            if (Search(makeIndex, out int idx))
                return _list[idx];
            return null;
        }
    }

    public void Clear()
    {
        lock (_locker) _list.Clear();
    }

    public void Delete(int index)
    {
        lock (_locker) _list.RemoveAt(index);
    }

    public void Remove(int makeIndex)
    {
        lock (_locker)
        {
            if (Search(makeIndex, out int idx))
                _list.RemoveAt(idx);
        }
    }

    public void Remove(TBagItem bagItem)
    {
        lock (_locker) _list.Remove(bagItem);
    }
}
