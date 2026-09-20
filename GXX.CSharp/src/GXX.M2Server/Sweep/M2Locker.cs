// 源单元：Source/M2Engine/M2Locker.pas（417 行统计 / 实际 470 行）
// 本文件 = TSafeList / TSafeStringList / TM2CriticalSection + 自旋锁自由函数的 1:1 托管移植
//          （namespace GXX.M2Server.Sweep）。
//
// 【原文对照】
//   M2Locker.pas:8        {$DEFINE USE_SPINLOCK}   ← 生效分支（下面只移植这一支）
//   M2Locker.pas:11-29    TSafeList 声明
//   M2Locker.pas:31-49    TSafeStringList 声明
//   M2Locker.pas:51-72    TM2CriticalSection 声明
//   M2Locker.pas:74-83    自由函数声明：SpinLock/SpinUnLock/BeginRead/EndRead/BeginWrite/EndWrite
//   M2Locker.pas:95-171   TSafeList 实现（97-108 ctor、110-119 dtor、121-136 LockR、138-146 UnLockR、148-160 LockW、162-171 UnLockW）
//   M2Locker.pas:173-265  TSafeStringList 实现（175-186 ctor、188-197 dtor、199-209 注释掉的 TryLock、
//                         211-226 LockR、228-237 UnLockR、239-254 LockW、256-265 UnLockW）
//   M2Locker.pas:267-374  自由函数实现（268-284 SpinLock、286-302 SpinUnLock、304-324 BeginRead、
//                         326-329 EndRead、331-367 BeginWrite、369-372 EndWrite）
//   M2Locker.pas:376-468  TM2CriticalSection 实现（378-389 ctor、391-400 dtor、402-412 注释掉的 TryLock、
//                         414-429 LockR、431-440 UnLockR、442-457 LockW、459-468 UnLockW）
//
// 【依赖处理（任务书第 2 条：不顺手移植依赖）】
//   * TSafeStringList 的基类 TStringList —— 复用既有 GXX.Core.Util.TStringList（已移植）。
//   * TSafeList 的基类 TList —— **本仓库尚无 TList 移植**（GXX.Core 只有 TStringList；GXX.Client 里的
//     TList 属另一工程/另一车道）。按任务书第 2 条不顺手移植 TList，改为**内部组合** List<object?>，
//     并只在 TSafeList 上暴露原文调用方实际会用到的 TList 成员面（Count/Add/Delete/Clear/Items/IndexOf）。
//     接缝：待 TList 在 GXX.Core 归位后，TSafeList 改为 `: TList`（成员面已对齐，改继承即可）。
//   * MyGetTickCount —— 原文 M2Share.pas:3589 `external mmsyst name 'timeGetTime'` → 见 SweepSeams.cs。
//   * MainOutMessage —— 原文 M2Share.pas → 见 SweepSeams.cs。
//   * Sleep(0) —— Windows API，托管侧 Thread.Sleep(0)（语义一致：让出当前时间片）。
//
// 【原文缺陷登记（按原文保留，不修正）】
//   1. M2Locker.pas:354 `if not Result then` —— BeginWrite 里"等待所有读取"的守卫条件写反了：
//      第一段自旋成功时 Result 恒为 True，于是**永远不会等待读者**；只有第一段超时（Result=False）
//      才回头等 Target = 1。按原文逐字保留。
//   2. M2Locker.pas:138-146 TSafeList.UnLockR 只 `FIsLock := False`，**没有**像
//      TSafeStringList.UnLockR:228-237 / TM2CriticalSection.UnLockR:431-440 那样清 FLockerID
//      —— 三份实现不对称。逐字保留。
//   3. M2Locker.pas:199-209 / 402-412 两处 `TryLock` 整段被 (* *) 注释掉 —— 以注释形式保留。
//   4. M2Locker.pas:314/341 `CurrentReference := Target and $FFFFFFFC` 在读取与 CAS 之间无同步，
//      属原文设计（自旋 + 超时兜底），逐字保留。
//   5. M2Locker.pas:91-93 只为非 CPUX64 声明 InterlockedCompareExchange 外部函数（32 位导入）。
//      托管侧统一用 System.Threading.Interlocked.CompareExchange，无平台分支。
//
// 【路径隔离说明】放在 Sweep/ 只是为了与顺序会话的 src/GXX.M2Server/** 常驻区物理隔离；
//   实现复用 GXX.Core（TStringList/DelphiRTL），不复制第二份。

using System;
using System.Collections.Generic;
using System.Threading;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Sweep;

/// <summary>
/// 原文 <c>M2Locker.pas:11-29 TSafeList = class(TList)</c>（{$DEFINE USE_SPINLOCK} 分支）。
/// <para>字段可见性说明：原文为 private；托管侧放开为 public **仅为单测可直接观测锁状态**
/// （不新增任何行为）。原文 M2Locker.pas:14-17。</para>
/// </summary>
public class TSafeList : IDisposable
{
    public string FName;
    public bool FIsLock;
    public int FLocker;
    public int FLockerID;

    private readonly List<object?> _list = new();

    /// <summary>原文 97-108 <c>constructor TSafeList.Create(AName: string)</c>。</summary>
    public TSafeList(string AName)
    {
        FName = AName;
        FIsLock = false;
        FLocker = 0;
        FLockerID = 0;
    }

    /// <summary>原文 110-119 <c>destructor TSafeList.Destroy</c>（托管侧 List 由 GC 回收，仅复位标志）。</summary>
    public void Dispose()
    {
        FIsLock = false;
        FLocker = 0;
    }

    /// <summary>原文 121-136 <c>procedure TSafeList.LockR(LockID: Integer)</c>。</summary>
    public void LockR(int LockID)
    {
        FIsLock = true;
        if (M2Locker.BeginRead(ref FLocker))
        {
            FLockerID = LockID;
        }
        else
        {
            // 原文如此（M2Locker.pas:131）：发现死锁时**只注释掉输出**，不做任何处理
            //MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
        }
    }

    /// <summary>原文 138-146 <c>procedure TSafeList.UnLockR</c>（注意：**不清 FLockerID**，原文如此）。</summary>
    public void UnLockR()
    {
        M2Locker.EndRead(ref FLocker);
        FIsLock = false;
    }

    /// <summary>原文 148-160 <c>procedure TSafeList.LockW(LockID: Integer)</c>（失败时输出死锁日志）。</summary>
    public void LockW(int LockID)
    {
        FIsLock = true;
        if (M2Locker.BeginWrite(ref FLocker))
        {
            FLockerID = LockID;
        }
        else
        {
            SweepSeam.MainOutMessage("发现死锁:" + FName + "; " + DelphiRTL.IntToStr(FLockerID) + ", " + DelphiRTL.IntToStr(LockID));
        }
    }

    /// <summary>原文 162-171 <c>procedure TSafeList.UnLockW</c>。</summary>
    public void UnLockW()
    {
        M2Locker.EndWrite(ref FLocker);
        FLockerID = 0;
        FIsLock = false;
    }

    // ---- 原文继承自 TList 的成员面（只列原文调用方实际用到的） ----

    /// <summary>原文 <c>TList.Count</c>。</summary>
    public int Count => _list.Count;

    /// <summary>原文 <c>TList.Items[Index]: Pointer</c>（读取越界抛异常，同 TList）。</summary>
    public object? this[int Index]
    {
        get => _list[Index];
        set => _list[Index] = value;
    }

    /// <summary>原文 <c>TList.Add(Item): Integer</c>（返回新元素下标）。</summary>
    public int Add(object? Item)
    {
        _list.Add(Item);
        return _list.Count - 1;
    }

    /// <summary>原文 <c>TList.Delete(Index)</c>。</summary>
    public void Delete(int Index) => _list.RemoveAt(Index);

    /// <summary>原文 <c>TList.Clear</c>。</summary>
    public void Clear() => _list.Clear();

    /// <summary>原文 <c>TList.IndexOf(Item): Integer</c>（未找到返回 -1）。</summary>
    public int IndexOf(object? Item) => _list.IndexOf(Item);
}

/// <summary>
/// 原文 <c>M2Locker.pas:31-49 TSafeStringList = class(TStringList)</c>（{$DEFINE USE_SPINLOCK} 分支）。
/// 基类复用既有的 <see cref="TStringList"/>（GXX.Core.Util）。
/// </summary>
public class TSafeStringList : TStringList, IDisposable
{
    public string FName;
    public bool FIsLock;
    public int FLocker;
    public int FLockerID;

    /// <summary>原文 175-186 <c>constructor TSafeStringList.Create(AName: string)</c>。</summary>
    public TSafeStringList(string AName)
    {
        FName = AName;
        FIsLock = false;
        FLocker = 0;
        FLockerID = 0;
    }

    /// <summary>原文 188-197 <c>destructor TSafeStringList.Destroy</c>。</summary>
    public void Dispose()
    {
        FIsLock = false;
        FLocker = 0;
    }

    // 原文 199-209（整段被 (* *) 注释掉的 TryLock，逐字保留）：
    //
    // function TSafeStringList.TryLock: Boolean;
    // begin
    // {$IFDEF USE_SPINLOCK}
    //   Result := not FIsLock;
    //   if Result then Lock;
    // {$ELSE}
    //   Result := TryEnterCriticalSection(FCS);
    // {$ENDIF}
    // end;

    /// <summary>原文 211-226 <c>procedure TSafeStringList.LockR(LockID: Integer)</c>。</summary>
    public void LockR(int LockID)
    {
        FIsLock = true;
        if (M2Locker.BeginRead(ref FLocker))
        {
            FLockerID = LockID;
        }
        else
        {
            SweepSeam.MainOutMessage("发现死锁:" + FName + "; " + DelphiRTL.IntToStr(FLockerID) + ", " + DelphiRTL.IntToStr(LockID));
        }
    }

    /// <summary>原文 228-237 <c>procedure TSafeStringList.UnLockR</c>（与 TSafeList 不同：**清 FLockerID**）。</summary>
    public void UnLockR()
    {
        M2Locker.EndRead(ref FLocker);
        FLockerID = 0;
        FIsLock = false;
    }

    /// <summary>原文 239-254 <c>procedure TSafeStringList.LockW(LockID: Integer)</c>。</summary>
    public void LockW(int LockID)
    {
        FIsLock = true;
        if (M2Locker.BeginWrite(ref FLocker))
        {
            FLockerID = LockID;
        }
        else
        {
            SweepSeam.MainOutMessage("发现死锁:" + FName + "; " + DelphiRTL.IntToStr(FLockerID) + ", " + DelphiRTL.IntToStr(LockID));
        }
    }

    /// <summary>原文 256-265 <c>procedure TSafeStringList.UnLockW</c>。</summary>
    public void UnLockW()
    {
        M2Locker.EndWrite(ref FLocker);
        FLockerID = 0;
        FIsLock = false;
    }
}

/// <summary>
/// 原文 <c>M2Locker.pas:51-72 TM2CriticalSection = class(TObject)</c>（{$DEFINE USE_SPINLOCK} 分支）。
/// </summary>
public class TM2CriticalSection : IDisposable
{
    public string FName;
    public bool FIsLock;
    public int FLockerID;

    /// <summary>原文 60 <c>FSection: Integer</c>（protected）。</summary>
    protected int FSection;

    /// <summary>原文 378-389 <c>constructor TM2CriticalSection.Create(AName: string)</c>。</summary>
    public TM2CriticalSection(string AName)
    {
        FName = AName;
        FIsLock = false;
        FSection = 0;
        FLockerID = 0;
    }

    /// <summary>原文 391-400 <c>destructor TM2CriticalSection.Destroy</c>。</summary>
    public void Dispose()
    {
        FIsLock = false;
        FSection = 0;
    }

    // 原文 402-412（整段被 (* *) 注释掉的 TryLock，逐字保留）：
    //
    // function TM2CriticalSection.TryLock: Boolean;
    // begin
    // {$IFDEF USE_SPINLOCK}
    //   Result := not FIsLock;
    //   if Result then Lock;
    // {$ELSE}
    //   Result := TryEnterCriticalSection(FSection);
    // {$ENDIF}
    // end;

    /// <summary>原文 414-429 <c>procedure TM2CriticalSection.LockR(LockID: Integer)</c>。</summary>
    public void LockR(int LockID)
    {
        FIsLock = true;
        if (M2Locker.BeginRead(ref FSection))
        {
            FLockerID = LockID;
        }
        else
        {
            SweepSeam.MainOutMessage("发现死锁:" + FName + "; " + DelphiRTL.IntToStr(FLockerID) + ", " + DelphiRTL.IntToStr(LockID));
        }
    }

    /// <summary>原文 431-440 <c>procedure TM2CriticalSection.UnLockR</c>。</summary>
    public void UnLockR()
    {
        M2Locker.EndRead(ref FSection);
        FLockerID = 0;
        FIsLock = false;
    }

    /// <summary>原文 442-457 <c>procedure TM2CriticalSection.LockW(LockID: Integer)</c>。</summary>
    public void LockW(int LockID)
    {
        FIsLock = true;
        if (M2Locker.BeginWrite(ref FSection))
        {
            FLockerID = LockID;
        }
        else
        {
            SweepSeam.MainOutMessage("发现死锁:" + FName + "; " + DelphiRTL.IntToStr(FLockerID) + ", " + DelphiRTL.IntToStr(LockID));
        }
    }

    /// <summary>原文 459-468 <c>procedure TM2CriticalSection.UnLockW</c>。</summary>
    public void UnLockW()
    {
        M2Locker.EndWrite(ref FSection);
        FLockerID = 0;
        FIsLock = false;
    }

    /// <summary>
    /// <b>仅测试用</b>：把原文 protected 的 <c>FSection</c>（M2Locker.pas:60）置成指定位模式，
    /// 用于确定性触发死锁/超时分支（不新增任何行为，仅供单测预置锁状态；
    /// 与 <c>FName/FIsLock/FLockerID</c> 的 public 化同样的测试可观测性理由）。
    /// </summary>
    public void SetSectionForTest(int value) => FSection = value;
}

/// <summary>
/// 原文 <c>M2Locker.pas:74-83 / 267-374</c> 的单元级自由函数（SpinLock/SpinUnLock/BeginRead/EndRead/
/// BeginWrite/EndWrite）。托管侧收进静态类；<c>var Target: Integer</c> 参数 → <c>ref int</c>。
/// </summary>
public static class M2Locker
{
    /// <summary>原文 268-284 <c>function SpinLock(var Target: Integer; const LockName: string): Boolean</c>。</summary>
    public static bool SpinLock(ref int Target, string LockName)
    {
        bool Result = true;
        uint StartTick = SweepSeam.MyGetTickCount();
        while (Interlocked.CompareExchange(ref Target, 1, 0) != 0)
        {
            Thread.Sleep(0);
            if ((uint)(SweepSeam.MyGetTickCount() - StartTick) >= 3000)
            {
                // 原文如此（M2Locker.pas:279）：超时输出被注释掉
                //MainOutMessage('发现死锁:' + LockName);
                Result = false;
                break;
            }
        }
        return Result;
    }

    /// <summary>原文 286-302 <c>function SpinUnLock(var Target: Integer; const LockName: string): Boolean</c>。</summary>
    public static bool SpinUnLock(ref int Target, string LockName)
    {
        bool Result = true;
        uint StartTick = SweepSeam.MyGetTickCount();
        while (Interlocked.CompareExchange(ref Target, 0, 1) != 1)
        {
            Thread.Sleep(0);
            if ((uint)(SweepSeam.MyGetTickCount() - StartTick) >= 3000)
            {
                // 原文如此（M2Locker.pas:297）
                //MainOutMessage('发现死锁:' + LockName);
                Result = false;
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 304-324 <c>function BeginRead(var Target: Integer): Boolean</c>。
    /// 超时 1000ms；CAS 把 <c>Target</c> 的计数 +2（低 2 位为写标志）。
    /// </summary>
    public static bool BeginRead(ref int Target)
    {
        bool Result = true;
        uint StartTick = SweepSeam.MyGetTickCount();

        // 等待写入器复位写入标志，因此 Target.Bit0 必须为 0
        int CurrentReference = 0;
        do
        {
            CurrentReference = Target & unchecked((int)0xFFFFFFFC);

            if ((uint)(SweepSeam.MyGetTickCount() - StartTick) >= 1000)
            {
                Result = false;
                break;
            }

            Thread.Sleep(0);
        } while (Interlocked.CompareExchange(ref Target, unchecked(CurrentReference + 2), CurrentReference) != CurrentReference);

        return Result;
    }

    /// <summary>原文 326-329 <c>procedure EndRead(var Target: Integer)</c>（计数 -2）。</summary>
    public static void EndRead(ref int Target)
    {
        Interlocked.Add(ref Target, -2);
    }

    /// <summary>
    /// 原文 331-367 <c>function BeginWrite(var Target: Integer): Boolean</c>。
    /// <para>**原文 354 的守卫写反**：<c>if not Result then</c> 只在第一段自旋超时后才等待读者；
    /// 第一段成功时不会等待。逐字保留（见文件头缺陷登记第 1 条）。</para>
    /// </summary>
    public static bool BeginWrite(ref int Target)
    {
        bool Result = true;
        uint StartTick = SweepSeam.MyGetTickCount();

        // 等待写入器复位写入标志，因此 Target.Bit0 必须为 0，然后设置 Target.Bit0
        int CurrentReference = 0;
        do
        {
            CurrentReference = Target & unchecked((int)0xFFFFFFFC);

            if ((uint)(SweepSeam.MyGetTickCount() - StartTick) >= 1000)
            {
                Result = false;
                break;
            }

            Thread.Sleep(0);
        } while (Interlocked.CompareExchange(ref Target, unchecked(CurrentReference + 1), CurrentReference) != CurrentReference);

        StartTick = SweepSeam.MyGetTickCount();

        if (!Result) // 原文如此（M2Locker.pas:354）—— 条件可疑，逐字保留
        {
            // 等待所有读取
            do
            {
                if ((uint)(SweepSeam.MyGetTickCount() - StartTick) >= 1000)
                {
                    Result = false;
                    break;
                }

                Thread.Sleep(0);
            } while (Target != 1);
        }

        return Result;
    }

    /// <summary>原文 369-372 <c>procedure EndWrite(var Target: Integer)</c>（**非原子**直接置 0，原文如此）。</summary>
    public static void EndWrite(ref int Target)
    {
        Target = 0;
    }
}
