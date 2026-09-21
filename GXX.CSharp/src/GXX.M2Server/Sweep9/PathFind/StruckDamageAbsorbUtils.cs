// ============================================================================
// 车道 p9-m2-pathfind —— 切片 2
// 源单元：Source/M2Engine/StruckDamageAbsorbUtils.pas（194 行，
//          镜像 _analysis/utf8_mirror/M2Engine/StruckDamageAbsorbUtils.pas）
// 1:1 托管移植。
//
// 【本单元在服务端的位置】
//   ObjBase.pas 的 `TBaseObject._Attack` 在"目标是人类以外的对象"那一层调用
//   `SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nPower)`
//   （注释「伤害吸收百分比 2020-09-17 20:11:44」）。该调用点已在
//   `src/GXX.M2Server/AttackCore.cs`（J143）与 `ExplosionMonsterCore.cs`（J143）中被**登记为未移植**；
//   本切片补上的正是那个 `.GetStruckDamage` 本体（台账 J143/J144/J146 的"尚未移植"清单里
//   连列三次的同一项）。
//
// ---------------------------------------------------------------------------
// 【类型映射决策（登记偏离 D-P9-04）】
//   原文 `TMonterStruckDamageAbsorb = record` + `PMonterStruckDamageAbsorb = ^TMonterStruckDamageAbsorb`，
//   生命周期由 `New(Result)` / `Dispose(Item)` 手工管理，且**列表里存的是指针**、
//   `Add` 返回的也是指针（调用方可能长期持有）。Delphi 记录的**值语义**在这里毫无用处、
//   而引用语义是必需的 ⇒ 托管侧落为 **class**（`TMonterStruckDamageAbsorb`），
//   `New`/`Dispose` 落为 `new` / 交给 GC（`DisposeItem` 注释点标明对应的原文位置）。
//
// 【依赖接缝】
//   * `MyGetTickCount`（原文 uses M2Share；M2Share.pas:3589 = winmm `timeGetTime`）
//     → 复用既有接缝 `GXX.M2Server.Sweep.SweepSeam.MyGetTickCount`（不另造第四份）。
//   * `tick_diff`（M2Share.pas:32953-32959）
//     → 复用既有 `GXX.M2Server.MonsterListBuildCore.TickDiff`（同为 32953 的 1:1，且已登记
//       `High(Cardinal)` 回绕偏差）。
//   * `AnsiCompareText`
//     → 复用既有 `GXX.M2Server.MonGenLoadCore.AnsiCompareText`（工程内既有的唯一处置）。
//   * `Random(100)`（Delphi RTL，`0..99`）
//     → 本单元自建接缝 `PathFindDamageSeam.Random`（M2Server 侧没有全局 Random 接缝；
//       `Npc.ObjNpcSeams.Random` 属 Npc 命名空间、不跨用）。
//
// 【原文缺陷（照抄 + 差异断言锁定）】
//   ① `Search` 命中后先 `H := I - 1` 再 `L := I` ⇒ 循环立即结束，**不会继续向左找重复名**。
//   ② `Add` 里把"率和值都 > 0"当唯一有效条件，**只裁剪上界 100、不裁剪下界**：
//      `AbsorbDamageRate = 0` 或 `AbsorbDamageValue = 0` ⇒ 删除已有项 / 不新增（返回 nil）。
//      ⇒ **无法用 `Add` 把某怪设成"吸收 0%"**，只能整条移除。
//   ③ `Add` 对**已存在**的项：先 `Result := FList.Items[Index]` 再判"要删除"，
//      删除分支里 `Dispose(Result); FList.Delete(Index); Result := nil; Exit;` —— 正确；
//      但**不删除**的分支**不会**把 `Index` 处的旧对象换掉，而是**原地改写**（保留 `StartTime` 被刷新）。
//   ④ `Run` 的 `EffectiveTime * 1000` 在 Delphi 里是 `Cardinal * Integer → Int64`（不溢出）；
//      托管侧用 `(long)` 提升，保留该语义。
//   ⑤ `Run` 在循环体内**每条各调一次** `MyGetTickCount`（不是循环外取一次）。
//   ⑥ `GetStruckDamage` 的兜底：先按怪名精确查，查不到再按通配名 `'*'` 查。
//      `'*'` 是**普通字符串**参与同一张有序表 —— 它必须真的存在于表里才生效。
//   ⑦ `AbsorbDamageValue` 的语义是"吸收掉原伤害的百分比"（`nDamage - Round(nDamage/100*Value)`），
//      不是"吸收掉固定点数"。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Sweep9.PathFind.Damage;

/// <summary>
/// 原文 <c>StruckDamageAbsorbUtils.pas:11-17 TMonterStruckDamageAbsorb</c>
/// （record + <c>PMonterStruckDamageAbsorb = ^...</c>）。
/// <para>托管侧落为 class（引用语义），理由见文件头 D-P9-04。</para>
/// </summary>
public class TMonterStruckDamageAbsorb
{
    /// <summary>原文 <c>:12 MonsterName: string</c>（怪物名；<c>'*'</c> 为通配项）。</summary>
    public string MonsterName = "";

    /// <summary>原文 <c>:13 AbsorbDamageRate: Byte</c>（触发机率，0..100，<c>Add</c> 里上界裁剪到 100）。</summary>
    public byte AbsorbDamageRate;

    /// <summary>原文 <c>:14 AbsorbDamageValue: Byte</c>（吸收百分比，0..100，<c>Add</c> 里上界裁剪到 100）。</summary>
    public byte AbsorbDamageValue;

    /// <summary>原文 <c>:15 StartTime: LongWord</c>（写入时刻 = <c>MyGetTickCount</c>）。</summary>
    public uint StartTime;

    /// <summary>原文 <c>:16 EffectiveTime: LongWord</c>（有效秒数；0 = 永不过期）。</summary>
    public uint EffectiveTime;
}

/// <summary>
/// 本单元的可替换接缝。默认实现对齐 Delphi RTL。
/// </summary>
public static class PathFindDamageSeam
{
    private static readonly Random Rnd = new();

    /// <summary>
    /// 原文 <c>Random(n)</c>（Delphi RTL System.pas）：返回 <c>0..n-1</c>，
    /// 且 <c>Random(0) = 0</c>（不抛异常）。接缝存在的意义是让单测可确定性注入。
    /// </summary>
    public static Func<int, int> Random { get; set; } = DelphiRandom;

    /// <summary>Delphi <c>Random(n)</c> 的默认实现（<c>n &lt;= 0</c> 返回 0）。</summary>
    public static int DelphiRandom(int range)
    {
        if (range <= 0) return 0;
        return Rnd.Next(range);
    }

    /// <summary>恢复接缝默认值（测试隔离用）。</summary>
    public static void ResetDefaults()
    {
        Random = DelphiRandom;
    }
}

/// <summary>
/// 原文 <c>StruckDamageAbsorbUtils.pas:19-37 TStruckDamageAbsorbMgr = class(TObject)</c>。
/// <para>按怪物名**有序**（<c>AnsiCompareText</c> 二分）维护一张伤害吸收表。</para>
/// </summary>
public class TStruckDamageAbsorbMgr : IDisposable
{
    /// <summary>原文 <c>:21 FList: TList</c>（元素是 <c>PMonterStruckDamageAbsorb</c>）。</summary>
    private readonly List<TMonterStruckDamageAbsorb> FList = new();

    /// <summary>原文 <c>:46-49 constructor TStruckDamageAbsorbMgr.Create</c>（<c>FList := TList.Create</c>）。</summary>
    public TStruckDamageAbsorbMgr()
    {
    }

    /// <summary>原文 <c>:51-56 destructor TStruckDamageAbsorbMgr.Destroy</c>（<c>Clear</c> 后 <c>FList.Free</c>）。</summary>
    public void Dispose()
    {
        Clear();
        FList.Clear();
    }

    /// <summary>原文 <c>:35 property Count: Integer read GetCount</c>。</summary>
    public int Count => GetCount();

    /// <summary>原文 <c>:36 property Items[Index: Integer]: PMonterStruckDamageAbsorb read GetItems</c>。</summary>
    public TMonterStruckDamageAbsorb this[int Index] => GetItems(Index);

    /// <summary>
    /// 原文 <c>:111-114 function TStruckDamageAbsorbMgr.GetCount</c>
    /// （<c>Result := FList.Count</c>）。
    /// </summary>
    private int GetCount()
    {
        return FList.Count;
    }

    /// <summary>
    /// 原文 <c>:116-122 function TStruckDamageAbsorbMgr.GetItems(Index)</c>。
    /// 越界返回 nil（原文：<c>Index &gt;= 0</c> 且 <c>Index &lt;= FList.Count - 1</c>）。
    /// </summary>
    private TMonterStruckDamageAbsorb GetItems(int Index)
    {
        if (Index >= 0 && Index <= FList.Count - 1)
            return FList[Index];
        return null;
    }

    /// <summary>
    /// 原文 <c>:58-69 procedure TStruckDamageAbsorbMgr.Clear</c>。
    /// 正序遍历并逐个 <c>Dispose</c>，最后 <c>FList.Clear</c>。
    /// </summary>
    public void Clear()
    {
        for (int I = 0; I <= FList.Count - 1; I++)
        {
            // 原文 Dispose(Item)：托管侧交给 GC（见 D-P9-04）。
        }

        FList.Clear();
    }

    /// <summary>
    /// 原文 <c>:71-109 function TStruckDamageAbsorbMgr.Add(MonsterName; AbsorbDamageRate, AbsorbDamageValue: Byte;
    /// EffectiveTime: LongWord): PMonterStruckDamageAbsorb</c>。
    /// <para>返回 nil 表示"该条不生效"（新增被拒 / 已有项被移除）。</para>
    /// </summary>
    public TMonterStruckDamageAbsorb Add(string MonsterName, byte AbsorbDamageRate, byte AbsorbDamageValue,
        uint EffectiveTime)
    {
        TMonterStruckDamageAbsorb Result;
        int Index;

        // 原文 :76-79：只裁上界，不裁下界（byte 无负值，故"下界"只剩 0）。
        if (AbsorbDamageRate > 100)
            AbsorbDamageRate = 100;
        if (AbsorbDamageValue > 100)
            AbsorbDamageValue = 100;

        if (Search(MonsterName, out Index))
        {
            Result = FList[Index];
            if (AbsorbDamageRate <= 0 || AbsorbDamageValue <= 0)
            {
                // 原文 :86-89 Dispose(Result); FList.Delete(Index); Result := nil; Exit;
                FList.RemoveAt(Index);
                return null;
            }
        }
        else
        {
            if (AbsorbDamageRate <= 0 || AbsorbDamageValue <= 0)
            {
                // 原文 :94-98：插入点已算出，但不新增。
                return null;
            }

            // 原文 :100-101 New(Result); FList.Insert(Index, Result)（Index 即插入点）。
            Result = new TMonterStruckDamageAbsorb();
            FList.Insert(Index, Result);
        }

        Result.MonsterName = MonsterName;
        Result.AbsorbDamageRate = AbsorbDamageRate;
        Result.AbsorbDamageValue = AbsorbDamageValue;
        Result.EffectiveTime = EffectiveTime;
        Result.StartTime = SweepSeam.MyGetTickCount();

        return Result;
    }

    /// <summary>
    /// 原文 <c>:124-138 procedure TStruckDamageAbsorbMgr.Run</c>。
    /// <para>**倒序**遍历；仅当 <c>EffectiveTime &gt; 0</c> 且
    /// <c>tick_diff(StartTime, MyGetTickCount) &gt;= EffectiveTime * 1000</c> 时移除
    /// （即"到点或超过"都移除）。</para>
    /// <para><c>MyGetTickCount</c> 在循环体内逐条调用（原文缺陷 ⑤）。</para>
    /// </summary>
    public void Run()
    {
        for (int I = FList.Count - 1; I >= 0; I--)
        {
            TMonterStruckDamageAbsorb Item = FList[I];
            if (Item.EffectiveTime > 0 &&
                (long)MonsterListBuildCore.TickDiff(Item.StartTime, SweepSeam.MyGetTickCount()) >=
                (long)Item.EffectiveTime * 1000)
            {
                // 原文 :134-135 Dispose(Item); FList.Delete(I);
                FList.RemoveAt(I);
            }
        }
    }

    /// <summary>
    /// 原文 <c>:140-167 function TStruckDamageAbsorbMgr.Search(MonsterName: string; var Index: Integer): Boolean</c>。
    /// <para><c>var Index</c> → <c>out int Index</c>：**命中时**是命中下标，
    /// **未命中时**是插入点（原文 <c>Index := L</c> 在循环外统一赋值）。</para>
    /// <para><b>命中即终止</b>：命中分支先 <c>H := I - 1</c> 再 <c>L := I</c> ⇒
    /// 循环条件立刻变假，不会继续向左找同名项（原文缺陷 ①）。</para>
    /// </summary>
    private bool Search(string MonsterName, out int Index)
    {
        bool Result = false;

        int L = 0;
        int H = FList.Count - 1;
        while (L <= H)
        {
            int I = (L + H) >> 1;
            TMonterStruckDamageAbsorb Item = FList[I];
            int C = MonGenLoadCore.AnsiCompareText(Item.MonsterName, MonsterName);
            if (C < 0)
            {
                L = I + 1;
            }
            else
            {
                H = I - 1;
                if (C == 0)
                {
                    Result = true;
                    L = I;
                }
            }
        }

        Index = L;
        return Result;
    }

    /// <summary>
    /// 原文 <c>:169-192 function TStruckDamageAbsorbMgr.GetStruckDamage(MonsterName: string; nDamage: Integer): Integer</c>。
    /// <para>先按怪名精确查；查不到再按通配名 <c>'*'</c> 查（<c>'*'</c> 必须真的在表里，
    /// 原文缺陷 ⑥）。命中后 <c>Random(100) &lt; AbsorbDamageRate</c> 才生效。</para>
    /// <para>吸收公式 <c>nDamage - Round(nDamage / 100 * AbsorbDamageValue)</c>
    /// （Delphi <c>/</c> 是浮点除法 ⇒ 托管侧 <c>/ 100.0</c>；<c>Round</c> 用
    /// <c>MidpointRounding.ToEven</c> 对齐 Delphi）。结果下限钳 0。</para>
    /// </summary>
    public int GetStruckDamage(string MonsterName, int nDamage)
    {
        int Result = nDamage;

        TMonterStruckDamageAbsorb Item;
        int Index;
        if (Search(MonsterName, out Index))
            Item = FList[Index];
        else if (Search("*", out Index))
            Item = FList[Index];
        else
            Item = null;

        if (Item != null)
        {
            if (PathFindDamageSeam.Random(100) < Item.AbsorbDamageRate)
            {
                Result = nDamage - (int)Math.Round(nDamage / 100.0 * Item.AbsorbDamageValue,
                    MidpointRounding.ToEven);
                if (Result < 0)
                    Result = 0;
            }
        }

        return Result;
    }
}
