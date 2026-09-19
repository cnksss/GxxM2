using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Rtl;

namespace GXX.DBServer;

// uRunGateList.pas (1-142) → uRunGateList.cs
// 备用（RunGate2）网关列表。Delphi 用 New(Result)/Dispose 堆分配 TRunGateInfo 并以指针共享，
// 故 C# 侧用具引用语义的 class 表达（等价 pTRunGateInfo）。

/// <summary>
/// Delphi `string[N]`（AnsiShortString）语义：GBK 字节容量 15，超长按字节截断。
/// uRunGateList.pas:12 `IP: string[15]`、DBShare.pas:28 `sSelGateIP: string[15]`。
/// </summary>
public readonly struct TShortString15
{
    public const int Capacity = 15;

    private readonly string _value;

    public TShortString15(string value) => _value = Truncate(value, Capacity);

    public string Value => _value ?? "";

    /// <summary>按 GBK 字节截断到 cap 字节（Delphi ShortString 赋值语义）。</summary>
    public static string Truncate(string s, int cap)
    {
        if (string.IsNullOrEmpty(s)) return "";
        byte[] bytes = EncodingInit.GBK.GetBytes(s);
        if (bytes.Length <= cap) return s;
        return EncodingInit.GBK.GetString(bytes, 0, cap);
    }

    public static implicit operator TShortString15(string v) => new TShortString15(v);
    public static implicit operator string(TShortString15 v) => v.Value;

    public static bool operator ==(TShortString15 a, TShortString15 b) => a.Value == b.Value;
    public static bool operator !=(TShortString15 a, TShortString15 b) => a.Value != b.Value;

    public override bool Equals(object obj) => obj is TShortString15 o && o.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}

/// <summary>uRunGateList.pas:10-18 `TRunGateInfo`（Delphi record，堆分配 + 指针共享 → C# class）。</summary>
public class TRunGateInfo
{
    /// <summary>uRunGateList.pas:11 `Enabled: Boolean` → byte（与 0 比较）。</summary>
    public byte Enabled;

    /// <summary>uRunGateList.pas:12 `IP: string[15]`。</summary>
    public TShortString15 IP;

    /// <summary>uRunGateList.pas:13 `Port: Word`。</summary>
    public ushort Port;

    /// <summary>uRunGateList.pas:14 `DBPort: Word`。</summary>
    public ushort DBPort;

    /// <summary>uRunGateList.pas:15 `Level: Integer`。</summary>
    public int Level;

    /// <summary>uRunGateList.pas:16 `LastResponseTick: LongWord`（最后响应时间 chongchong 2015-07-27）。</summary>
    public uint LastResponseTick;

    /// <summary>uRunGateList.pas:17 `IsConnect: Boolean` → byte。</summary>
    public byte IsConnect;
}

/// <summary>
/// uRunGateList.pas:21-142 `TRunGateList`。
/// FList 为插入序；FSortList 为 DoSort 后的 Level 降序镜像（原文 QuickSort 逐字移植）。
/// </summary>
public class TRunGateList
{
    private readonly List<TRunGateInfo> FList = new List<TRunGateInfo>();
    private readonly List<TRunGateInfo> FSortList = new List<TRunGateInfo>();

    /// <summary>uRunGateList.pas:137-140 `GetCount`。</summary>
    public int Count => FList.Count;

    public TRunGateList()
    {
        // uRunGateList.pas:44-48 constructor：仅创建两个空 TList。
    }

    /// <summary>uRunGateList.pas:119-126 `GetItems`：越界返回 nil。</summary>
    public TRunGateInfo Items(int Index)
    {
        TRunGateInfo Result = null;
        if (Index >= 0 && Index < FList.Count)
        {
            Result = FList[Index];
        }
        return Result;
    }

    /// <summary>uRunGateList.pas:128-135 `GetSortItems`：越界返回 nil。</summary>
    public TRunGateInfo SortItems(int Index)
    {
        TRunGateInfo Result = null;
        if (Index >= 0 && Index < FSortList.Count)
        {
            Result = FSortList[Index];
        }
        return Result;
    }

    /// <summary>
    /// uRunGateList.pas:58-69 `Add`。
    /// 原文如此：只写入 FList，不写 FSortList，也**不**初始化 IsConnect（保持 New 出来的 0）。
    /// </summary>
    public TRunGateInfo Add(byte AEnabled, string AIP, ushort APort, ushort ADBPort, int ALevel)
    {
        var Result = new TRunGateInfo();
        FList.Add(Result);
        Result.Enabled = AEnabled;
        Result.IP = AIP;
        Result.Port = APort;
        Result.DBPort = ADBPort;
        Result.Level = ALevel;
        Result.LastResponseTick = DelphiTick.GetTickCount();
        return Result;
    }

    /// <summary>uRunGateList.pas:71-81 `Clear`。</summary>
    public void Clear()
    {
        for (int I = 0; I < FList.Count; I++)
        {
            // 原文 Dispose(PRunGateInfo(FList.Items[I]))：托管侧交给 GC，仅断开引用。
        }
        FList.Clear();
        FSortList.Clear();
    }

    /// <summary>
    /// uRunGateList.pas:83-117 `DoSort`：Level **降序**（逐字移植原文 QuickSort 的边界写法）。
    /// 原文如此：FList.Count &lt;= 1 时**不重建** FSortList（保留上一次的陈旧内容）。
    /// </summary>
    public void DoSort()
    {
        void QuickSort(int L, int R)
        {
            int I, J;
            int nLevel;
            do
            {
                I = L;
                J = R;
                nLevel = FSortList[L + (R - L) / 2].Level;
                do
                {
                    while (FSortList[I].Level > nLevel)
                        I++;
                    while (FSortList[J].Level < nLevel)
                        J--;

                    if (I <= J)
                    {
                        Exchange(I, J);
                        I++;
                        J--;
                    }
                } while (I <= J);
                if (L < J) QuickSort(L, J);
                L = I;
            } while (I < R);
        }

        if (FList.Count > 1)
        {
            FSortList.Clear();
            FSortList.AddRange(FList);      // TList.Assign(FList) 语义：浅拷贝元素引用
            QuickSort(0, FSortList.Count - 1);
        }
    }

    /// <summary>Delphi `TList.Exchange`（原文 FSortList.Exchange(I, J)）。</summary>
    private void Exchange(int i, int j)
    {
        (FSortList[i], FSortList[j]) = (FSortList[j], FSortList[i]);
    }
}
