using System;
using System.Collections.Generic;

namespace GXX.M2Server.Engine;

/// <summary>uMagicACUtils.pas TMagicACInfo record 1:1：单技能攻防百分比配置。</summary>
public sealed class TMagicACInfo
{
    public ushort wMagicId;
    public bool boEnabled;
    public byte btHum;             // 对人攻击百分比
    public byte btMon;             // 对怪攻击百分比
    public byte btHero;            // 对英雄攻击百分比
    public byte btDefenceHum;      // 对人防御百分比
    public byte btDefenceMon;      // 对怪防御百分比
    public byte btDefenceHero;     // 对英雄防御百分比
    public string sMagicName = "";
}

/// <summary>
/// uMagicACUtils.pas TMagicACList 1:1（批次J59）：技能攻防百分比表。
/// Add 按 wMagicId 去重（重复直接返回既有记录）；唯一的特例分支是 MagicID = 12
/// （原文魔术数字，构造时 boEnabled=True 且六项百分比全 100）；
/// Sort 后 Get 走二分（wMagicId - MagicID），未排序走线性；
/// QuickSort 为原文的三数取中 Hoare 分区原地实现（含 P 随交换迁移）。
/// FIsSort 一旦置真不再重排（原文 CustomSort 的 if not FIsSort 门控）。
/// </summary>
public sealed class TMagicACList
{
    private readonly object _lock = new();
    private readonly List<TMagicACInfo> _list = new();
    private bool _isSort;

    public string LockName { get; }

    public TMagicACList(string aLockName = "") => LockName = aLockName;

    public int Count => _list.Count;

    public bool IsSorted => _isSort;

    /// <summary>Items[index]：越界返回 null（Delphi GetItems 1:1）。</summary>
    public TMagicACInfo? this[int index]
        => index >= 0 && index <= _list.Count - 1 ? _list[index] : null;

    /// <summary>Add（63-109）：按 wMagicId 查重；MagicID=12 特例初始化。</summary>
    public TMagicACInfo Add(ushort magicId, string magicName)
    {
        lock (_lock)
        {
            foreach (var existing in _list)
            {
                if (existing.wMagicId == magicId)
                    return existing;
            }

            var info = new TMagicACInfo { wMagicId = magicId, sMagicName = magicName };
            if (magicId == 12)
            {
                info.boEnabled = true;
                info.btHum = 100;
                info.btMon = 100;
                info.btHero = 100;
                info.btDefenceHum = 0;
                info.btDefenceMon = 0;
                info.btDefenceHero = 0;
            }
            else
            {
                info.boEnabled = false;
                info.btHum = 0;
                info.btMon = 0;
                info.btHero = 0;
                info.btDefenceHum = 0;
                info.btDefenceMon = 0;
                info.btDefenceHero = 0;
            }

            _list.Add(info);
            return info;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _list.Clear();
            // 原文 Clear 未复位 FIsSort（Delphi 1:1 保留）
        }
    }

    /// <summary>Sort（150-153）：按 wMagicId 升序（MagicACListCompareMagicID 减法）。</summary>
    public void Sort() => CustomSort((a, b) => a.wMagicId - b.wMagicId);

    /// <summary>CustomSort（155-167）：仅首次（!FIsSort 且 Count &gt; 1）真正排序。</summary>
    public void CustomSort(Func<TMagicACInfo, TMagicACInfo, int> compare)
    {
        lock (_lock)
        {
            if (!_isSort && _list.Count > 1)
            {
                QuickSort(0, _list.Count - 1, compare);
                _isSort = true;
            }
        }
    }

    /// <summary>QuickSort（169-195）：原文 Hoare 分区 + 三数取中，P 随交换迁移。</summary>
    private void QuickSort(int l, int r, Func<TMagicACInfo, TMagicACInfo, int> compare)
    {
        int i, j, p;
        do
        {
            i = l;
            j = r;
            p = (l + r) >> 1;
            do
            {
                while (compare(_list[i], _list[p]) < 0) i++;
                while (compare(_list[j], _list[p]) > 0) j--;
                if (i <= j)
                {
                    (_list[i], _list[j]) = (_list[j], _list[i]);
                    if (p == i)
                        p = j;
                    else if (p == j)
                        p = i;
                    i++;
                    j--;
                }
            }
            while (i <= j);

            if (l < j)
                QuickSort(l, j, compare);
            l = i;
        }
        while (i < r);
    }

    /// <summary>Get（197-240）：已排序走二分，否则线性；未命中返回 null。</summary>
    public TMagicACInfo? Get(ushort magicId)
    {
        lock (_lock)
        {
            if (_isSort)
            {
                int l = 0;
                int h = _list.Count - 1;
                while (l <= h)
                {
                    int i = l + (h - l) / 2;
                    var info = _list[i];
                    int c = info.wMagicId - magicId;
                    if (c < 0)
                    {
                        l = i + 1;
                    }
                    else
                    {
                        h = i - 1;
                        if (c == 0)
                            return info;
                    }
                }
                return null;
            }

            foreach (var info in _list)
            {
                if (info.wMagicId == magicId)
                    return info;
            }
            return null;
        }
    }

    /// <summary>复位 FIsSort（原文 Clear 不复位，此处仅供测试隔离使用）。</summary>
    public void ResetSortFlag()
    {
        lock (_lock)
        {
            _isSort = false;
        }
    }

    /// <summary>遍历快照（窗体层用）。</summary>
    public IEnumerable<TMagicACInfo> All()
    {
        lock (_lock)
        {
            foreach (var info in _list)
                yield return info;
        }
    }
}

/// <summary>uMagicACUtils.pas 单元级（批次J59）：g_MagicACList（技能攻防百分比表）。</summary>
public static class MagicACUtils
{
    /// <summary>g_MagicACList。</summary>
    public static readonly TMagicACList MagicACList = new("MagicACList");

    /// <summary>测试隔离：清空并复位排序标志。</summary>
    public static void ResetForTests()
    {
        MagicACList.Clear();
        MagicACList.ResetSortFlag();
    }
}
