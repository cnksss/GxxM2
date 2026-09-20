using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// 源：Source/RunGate/RunGateUtils.pas:3816-3968
//   TIPCheckInfo / TCheckItemInfo 记录定义（3817-3829）
//   GetClientRunGateIP（3831-3899，**整段被 (* *) 注释掉 → 死代码**）
//   GetClientDate      （3901-3967，NEED_REGISTER=1 下是活代码）
// 两者结构完全相同、只有被统计的字段不同（RunGateIP vs Date）——典型的“看起来一样实则不同”，
// 移植为同一算法 + 两个字段选择器，并用差异断言把“孪生但只有一个活着”这件事钉死。

namespace GXX.RunGate;

/// <summary>TIPCheckInfo（RunGateUtils.pas:3818-3823，packed = 12 字节）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TIPCheckInfo
{
    public uint IP;
    public int Date;
    public int RunGateIP;
}

/// <summary>TCheckItemInfo（RunGateUtils.pas:3825-3829，非 packed = 8 字节）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TCheckItemInfo
{
    public int Value;
    public int Count;
}

/// <summary>被统计的字段（对应原文那对孪生函数）。</summary>
public enum TIPCheckField
{
    /// <summary>GetClientDate（活）。</summary>
    Date = 0,
    /// <summary>GetClientRunGateIP（原文已注释，保留仅为对照）。</summary>
    RunGateIP = 1,
}

/// <summary>
/// RunGateUtils.pas:3901-3967 —— 客户端时间“多数表决”。
/// </summary>
public static class RunGateClientStat
{
    /// <summary>RunGateUtils.pas:3910 / 3841 —— 样本数下限，低于此值直接返回 0。</summary>
    public const int MinSampleCount = 20;

    /// <summary>RunGateUtils.pas:3946 / 3877 —— 有效样本（字段非 0）下限。</summary>
    public const int MinValidCount = 20;

    /// <summary>RunGateUtils.pas:3901-3967 —— GetClientDate（等价于 <c>Majority(list, Date)</c>）。</summary>
    public static int GetClientDate(IReadOnlyList<TIPCheckInfo> list) => Majority(list, TIPCheckField.Date);

    /// <summary>
    /// RunGateUtils.pas:3831-3899 —— GetClientRunGateIP。
    /// <b>原文该函数整体处于 <c>(* ... *)</c> 注释块内（3831 行开、3899 行闭）</b>，
    /// 唯一的调用点也被注释（RunGateUtils.pas:4035）。故这里只作对照实现，**不接入任何活路径**。
    /// </summary>
    public static int GetClientRunGateIP(IReadOnlyList<TIPCheckInfo> list) => Majority(list, TIPCheckField.RunGateIP);

    /// <summary>
    /// 表决本体。
    /// <para>
    /// 逐行要点：① <c>list.Count &lt; 20</c> 立即返回 0；② 只统计所选字段 <c>&lt;&gt; 0</c> 的项；
    /// ③ 按**首次出现顺序**建桶、桶内计数；④ 只有有效数 <c>&gt;= 20</c> 才表决；
    /// ⑤ 取**第一个**计数 <c>&gt;= AllCount div 2</c> 的桶的值 —— 不是取最大，也不是取占比最高。
    /// </para>
    /// </summary>
    public static int Majority(IReadOnlyList<TIPCheckInfo> list, TIPCheckField field)
    {
        if (list == null) return 0;
        if (list.Count < MinSampleCount) return 0;

        var buckets = new List<TCheckItemInfo>();
        int allCount = 0;

        for (int i = 0; i < list.Count; i++)
        {
            int value = field == TIPCheckField.Date ? list[i].Date : list[i].RunGateIP;
            if (value == 0) continue;                       // 原 3919：if IPCheckInfo.Date <> 0

            allCount++;

            bool found = false;
            for (int j = 0; j < buckets.Count; j++)
            {
                if (buckets[j].Value == value)
                {
                    var b = buckets[j];
                    b.Count++;
                    buckets[j] = b;                         // 原 3931：Inc(ItemInfo.Count)（指针写回）
                    found = true;
                    break;
                }
            }

            if (!found)
                buckets.Add(new TCheckItemInfo { Value = value, Count = 1 });
        }

        if (allCount >= MinValidCount)
        {
            int half = allCount / 2;                        // 原 3951：AllCount div 2（整数除，向下取整）
            for (int i = 0; i < buckets.Count; i++)
            {
                if (buckets[i].Count >= half)
                    return buckets[i].Value;                // 原 3953：Break，取首个达标者
            }
        }

        return 0;
    }
}
