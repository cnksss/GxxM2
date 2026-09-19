using System;
using System.Collections.Generic;

namespace GXX.Core.Util;

/// <summary>TQuickID（MudUtil.pas）。</summary>
public class TQuickID
{
    public string sAccount = "";
    public string sChrName = "";
    public int nIndex;          // NativeInt → int（原用于对象指针槽位）
    public long nIndexPtr;      // 原始 NativeInt 槽（TObject(nIndex) 指针语义保留）
    public int nSelectID;
    public bool boIsHero;
}

/// <summary>TQuickList（带锁的名称→整数索引表，二分查找语义 1:1）。</summary>
public class TQuickList
{
    private readonly TStringList _list = new();
    private readonly object _critical = new();
    private bool _sorted = true;

    public int Count => _list.Count;
    public bool boCaseSensitive { get => _list.CaseSensitive; set => _list.CaseSensitive = value; }

    public string GetString(int index) => _list[index];

    public void Lock() => System.Threading.Monitor.Enter(_critical);
    public void UnLock() => System.Threading.Monitor.Exit(_critical);

    /// <summary>排序（原 SortString 快速排序，此处保序语义一致）。</summary>
    public void SortString(int nMin, int nMax)
    {
        _list.Sort();
        _sorted = true;
    }

    public int GetIndex(string sName)
    {
        int result = -1;
        if (_list.Count == 0) return result;
        int nLow = 0, nHigh, nMed, nCompareVal;
        if (_sorted)
        {
            if (_list.Count == 1)
            {
                if (string.CompareOrdinal(sName, _list[0]) == 0) result = 0;
            }
            else
            {
                nHigh = _list.Count - 1;
                nMed = (nHigh - nLow) / 2 + nLow;
                while (true)
                {
                    if (nHigh - nLow == 1)
                    {
                        if (string.CompareOrdinal(sName, _list[nHigh]) == 0) result = nHigh;
                        if (string.CompareOrdinal(sName, _list[nLow]) == 0) result = nLow;
                        break;
                    }
                    nCompareVal = string.CompareOrdinal(sName, _list[nMed]);
                    if (nCompareVal > 0)
                    {
                        nLow = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    if (nCompareVal < 0)
                    {
                        nHigh = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    result = nMed;
                    break;
                }
            }
        }
        else
        {
            if (_list.Count == 1)
            {
                if (string.Equals(sName, _list[0], StringComparison.OrdinalIgnoreCase)) result = 0;
            }
            else
            {
                nHigh = _list.Count - 1;
                nMed = (nHigh - nLow) / 2 + nLow;
                while (true)
                {
                    if (nHigh - nLow == 1)
                    {
                        if (string.Equals(sName, _list[nHigh], StringComparison.OrdinalIgnoreCase)) result = nHigh;
                        if (string.Equals(sName, _list[nLow], StringComparison.OrdinalIgnoreCase)) result = nLow;
                        break;
                    }
                    nCompareVal = string.Compare(sName, _list[nMed], StringComparison.OrdinalIgnoreCase);
                    if (nCompareVal > 0)
                    {
                        nLow = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    if (nCompareVal < 0)
                    {
                        nHigh = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    result = nMed;
                    break;
                }
            }
        }
        return result;
    }

    public bool AddRecord(string sName, int nIndex)
    {
        bool result = true;
        if (_list.Count == 0)
        {
            _list.AddObject(sName, _list.Count >= 0 ? new IntPtrBox(nIndex) : null);
        }
        else if (_sorted)
        {
            if (_list.Count == 1)
            {
                int nMed = string.CompareOrdinal(sName, _list[0]);
                if (nMed > 0) _list.AddObject(sName, new IntPtrBox(nIndex));
                else if (nMed < 0) _list.InsertObject(0, sName, new IntPtrBox(nIndex));
            }
            else
            {
                int nLow = 0, nHigh = _list.Count - 1;
                int nMed = (nHigh - nLow) / 2 + nLow;
                while (true)
                {
                    if (nHigh - nLow == 1)
                    {
                        nMed = string.CompareOrdinal(sName, _list[nHigh]);
                        if (nMed > 0) { _list.InsertObject(nHigh + 1, sName, new IntPtrBox(nIndex)); break; }
                        nMed = string.CompareOrdinal(sName, _list[nLow]);
                        if (nMed > 0) { _list.InsertObject(nLow + 1, sName, new IntPtrBox(nIndex)); break; }
                        if (nMed < 0) { _list.InsertObject(nLow, sName, new IntPtrBox(nIndex)); break; }
                        result = false;
                        break;
                    }
                    int nCompareVal = string.CompareOrdinal(sName, _list[nMed]);
                    if (nCompareVal > 0)
                    {
                        nLow = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    if (nCompareVal < 0)
                    {
                        nHigh = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    result = false;
                    break;
                }
            }
        }
        else
        {
            if (_list.Count == 1)
            {
                int nMed = string.Compare(sName, _list[0], StringComparison.OrdinalIgnoreCase);
                if (nMed > 0) _list.AddObject(sName, new IntPtrBox(nIndex));
                else if (nMed < 0) _list.InsertObject(0, sName, new IntPtrBox(nIndex));
            }
            else
            {
                int nLow = 0, nHigh = _list.Count - 1;
                int nMed = (nHigh - nLow) / 2 + nLow;
                while (true)
                {
                    if (nHigh - nLow == 1)
                    {
                        nMed = string.Compare(sName, _list[nHigh], StringComparison.OrdinalIgnoreCase);
                        if (nMed > 0) { _list.InsertObject(nHigh + 1, sName, new IntPtrBox(nIndex)); break; }
                        nMed = string.Compare(sName, _list[nLow], StringComparison.OrdinalIgnoreCase);
                        if (nMed > 0) { _list.InsertObject(nLow + 1, sName, new IntPtrBox(nIndex)); break; }
                        if (nMed < 0) { _list.InsertObject(nLow, sName, new IntPtrBox(nIndex)); break; }
                        result = false;
                        break;
                    }
                    int nCompareVal = string.Compare(sName, _list[nMed], StringComparison.OrdinalIgnoreCase);
                    if (nCompareVal > 0)
                    {
                        nLow = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    if (nCompareVal < 0)
                    {
                        nHigh = nMed;
                        nMed = (nHigh - nLow) / 2 + nLow;
                        continue;
                    }
                    result = false;
                    break;
                }
            }
        }
        return result;
    }

    public int GetValue(int index) => _list.GetObject(index) is IntPtrBox b ? b.Value : 0;

    private sealed class IntPtrBox
    {
        public readonly int Value;
        public IntPtrBox(int v) => Value = v;
    }
}

/// <summary>TQuickIDList：账号 → 角色列表的有序映射（二分插入语义 1:1）。</summary>
public class TQuickIDList
{
    private readonly TStringList _list = new();

    public int Count => _list.Count;

    public void AddRecord(string sAccount, string sChrName, int nIndex, int nSelIndex, bool boIsHero)
    {
        var quickId = new TQuickID
        {
            sAccount = sAccount,
            sChrName = sChrName,
            nIndex = nIndex,
            nSelectID = nSelIndex,
            boIsHero = boIsHero
        };
        List<TQuickID> chrList;
        if (_list.Count == 0)
        {
            chrList = new List<TQuickID> { quickId };
            _list.AddObject(sAccount, chrList);
            return;
        }
        if (_list.Count == 1)
        {
            int nMed = string.CompareOrdinal(sAccount, _list[0]);
            if (nMed > 0)
            {
                chrList = new List<TQuickID> { quickId };
                _list.AddObject(sAccount, chrList);
            }
            else if (nMed < 0)
            {
                chrList = new List<TQuickID> { quickId };
                _list.InsertObject(0, sAccount, chrList);
            }
            else
            {
                chrList = (List<TQuickID>)_list.GetObject(0);
                chrList.Add(quickId);
            }
            return;
        }
        int nLow = 0, nHigh = _list.Count - 1;
        int nMed2 = (nHigh - nLow) / 2 + nLow;
        while (true)
        {
            if (nHigh - nLow == 1)
            {
                int n20 = string.CompareOrdinal(sAccount, _list[nHigh]);
                if (n20 > 0)
                {
                    chrList = new List<TQuickID> { quickId };
                    _list.InsertObject(nHigh + 1, sAccount, chrList);
                    break;
                }
                if (string.CompareOrdinal(sAccount, _list[nHigh]) == 0)
                {
                    chrList = (List<TQuickID>)_list.GetObject(nHigh);
                    chrList.Add(quickId);
                    break;
                }
                n20 = string.CompareOrdinal(sAccount, _list[nLow]);
                if (n20 > 0)
                {
                    chrList = new List<TQuickID> { quickId };
                    _list.InsertObject(nLow + 1, sAccount, chrList);
                    break;
                }
                if (n20 < 0)
                {
                    chrList = new List<TQuickID> { quickId };
                    _list.InsertObject(nLow, sAccount, chrList);
                    break;
                }
                chrList = (List<TQuickID>)_list.GetObject(n20 >= 0 ? nLow : nLow);
                chrList.Add(quickId);
                break;
            }
            int n1C = string.CompareOrdinal(sAccount, _list[nMed2]);
            if (n1C > 0)
            {
                nLow = nMed2;
                nMed2 = (nHigh - nLow) / 2 + nLow;
                continue;
            }
            if (n1C < 0)
            {
                nHigh = nMed2;
                nMed2 = (nHigh - nLow) / 2 + nLow;
                continue;
            }
            chrList = (List<TQuickID>)_list.GetObject(nMed2);
            chrList.Add(quickId);
            break;
        }
    }

    public void DelRecord(int nIndex, string sChrName)
    {
        if (_list.Count - 1 < nIndex) return;
        var chrList = (List<TQuickID>)_list.GetObject(nIndex);
        for (int i = 0; i < chrList.Count; i++)
        {
            if (chrList[i].sChrName == sChrName)
            {
                chrList.RemoveAt(i);
                break;
            }
        }
        if (chrList.Count <= 0)
            _list.Delete(nIndex);
    }

    public int GetChrList(string sAccount, ref List<TQuickID> chrNameList)
    {
        int result = -1;
        if (_list.Count == 0) return -1;
        if (_list.Count == 1)
        {
            if (string.CompareOrdinal(sAccount, _list[0]) == 0)
            {
                chrNameList = (List<TQuickID>)_list.GetObject(0);
                result = 0;
            }
            return result;
        }
        int nLow = 0, nHigh = _list.Count - 1;
        int nMed = (nHigh - nLow) / 2 + nLow;
        int n24 = -1;
        while (true)
        {
            if (nHigh - nLow == 1)
            {
                if (string.CompareOrdinal(sAccount, _list[nHigh]) == 0) n24 = nHigh;
                if (string.CompareOrdinal(sAccount, _list[nLow]) == 0) n24 = nLow;
                break;
            }
            int n20 = string.CompareOrdinal(sAccount, _list[nMed]);
            if (n20 > 0)
            {
                nLow = nMed;
                nMed = (nHigh - nLow) / 2 + nLow;
                continue;
            }
            if (n20 < 0)
            {
                nHigh = nMed;
                nMed = (nHigh - nLow) / 2 + nLow;
                continue;
            }
            n24 = nMed;
            break;
        }
        if (n24 != -1)
            chrNameList = (List<TQuickID>)_list.GetObject(n24);
        return n24;
    }
}
