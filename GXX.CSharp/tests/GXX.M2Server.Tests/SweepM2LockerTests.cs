// 测试：Source/M2Engine/M2Locker.pas → GXX.M2Server.Sweep 下的
//       M2Locker / TSafeList / TSafeStringList / TM2CriticalSection（1:1）
//
// 覆盖策略（任务书第 3 条）：每个公开方法 ≥3 用例，含空/0/负/边界/异常；
// 对「看起来一样实则不同」的分支写差异断言：
//   * TSafeList.UnLockR **不清 FLockerID**（M2Locker.pas:138-146），而
//     TSafeStringList.UnLockR:228-237 与 TM2CriticalSection.UnLockR:431-440 **清** —— 三份不对称，逐一钉死；
//   * BeginWrite 的 M2Locker.pas:354 `if not Result then` 把守卫写反：
//     第一段自旋成功时不等待读者 → 本测试用「第一段成功后第二段根本没进」证伪；
//   * EndWrite:369-372 是**非原子**直接赋值 0（不是 Interlocked），逐字保留；
//   * BeginRead/BeginWrite 的 CAS 目标值是 `CurrentReference + 2` / `+ 1`，低 2 位是写标志。
//
// 计时：SweepTestEnv 提供**每次调用自增 1** 的可控 SweepSeam.MyGetTickCount，
// 因此超时分支可以**确定性**触发，不需要真的 Sleep 3 秒。
// （注意：不能让默认时钟恒定 —— 自旋函数是无界循环，恒定时钟遇 CAS 永不成功会永久挂死。）

using System;
using GXX.M2Server.Sweep;
using Xunit;

namespace GXX.M2Server.Tests;

public class SweepM2LockerTests
{
    /// <summary>
    /// 安装**冻结时钟**（每次调用返回同一个非 0 值）。用于「CAS 一次成功」的常规路径：
    /// 此时自旋循环至多迭代一次，不需要真实的超时推进，读数也因此完全确定。
    /// <para>不要用它测超时路径 —— 恒定时钟永远不会推进超时，而自旋循环无界，会永久挂死。</para>
    /// </summary>
    private static void FreezeClock(SweepTestEnv env, uint value = 5u)
        => SweepSeam.MyGetTickCount = () => value;

    // ------------------------------------------------------------------ SpinLock / SpinUnLock

    [Fact]
    public void SpinLock_FreeTarget_AcquiresWithoutTicking()
    {
        using var env = new SweepTestEnv();
        int target = 0;
        Assert.True(M2Locker.SpinLock(ref target, "T"));
        Assert.Equal(1, target);
        Assert.Empty(SweepSeam.LoggedMessages);   // 原文超时输出被注释掉（:279）
    }

    [Fact]
    public void SpinLock_AlreadyHeld_TimesOutAfter3000ms_AndLeavesTargetHeld()
    {
        using var env = new SweepTestEnv();
        int target = 1;              // 已被占用
        // 起始 tick（1 次）+ 每次循环 1 次，直到累计 >= 3000 → 恰好 3001
        Assert.False(M2Locker.SpinLock(ref target, "T"));
        Assert.Equal(1, target);     // 超时后**不修改** Target（原文 Break 直出）
        Assert.Equal(3001u, env.Tick);
        Assert.Empty(SweepSeam.LoggedMessages);
    }

    [Fact]
    public void SpinUnLock_HeldTarget_Releases_And_TimesOutWhenNotHeld()
    {
        using var env = new SweepTestEnv();
        int target = 1;
        Assert.True(M2Locker.SpinUnLock(ref target, "T"));
        Assert.Equal(0, target);

        // Target 已是 0 → CAS(Target,0,1) 的返回值永不等于 1 → 自旋到 3000ms 超时
        Assert.False(M2Locker.SpinUnLock(ref target, "T"));
        Assert.Equal(0, target);
    }

    [Fact]
    public void SpinLock_ThenSpinUnLock_RoundTrip()
    {
        using var env = new SweepTestEnv();
        int target = 0;
        Assert.True(M2Locker.SpinLock(ref target, "T"));
        Assert.Equal(1, target);
        Assert.True(M2Locker.SpinUnLock(ref target, "T"));
        Assert.Equal(0, target);
        Assert.True(M2Locker.SpinLock(ref target, "T")); // 可再次获取
        Assert.Equal(1, target);
    }

    // ------------------------------------------------------------------ BeginRead / EndRead

    /// <summary>
    /// **差异断言（原文设计限制）**：<c>BeginRead</c> 的 CAS 期望值是 <c>Target and $FFFFFFFC</c>
    /// （<c>M2Locker.pas:314</c>），而 CAS 比较的是**未掩码**的 Target。于是只要已有读者持锁
    /// （Target=2），第二位读者的 <c>CurrentReference</c> 仍是 0 → <c>CAS(Target, 2, 0)</c> 永不成功
    /// → 自旋到 1000ms 超时返回 False。
    /// <para>即：<b>读锁实际不可重入</b>（同一时刻只能有一位读者），第二读者要等第一读者释放后才能进。
    /// 这不是移植错误 —— Delphi 原文 <c>InterlockedCompareExchange(Target, CurrentReference + 2, CurrentReference)</c>
    /// 同样如此；本用例把这个「看起来像读写锁、实则读也不并发」的行为钉死。</para>
    /// </summary>
    [Fact]
    public void BeginRead_SecondConcurrentReader_TimesOut_ReadLockNotReentrant()
    {
        using var env = new SweepTestEnv();
        int t = 0;
        Assert.True(M2Locker.BeginRead(ref t));     // 第一位读者
        Assert.Equal(2, t);

        Assert.False(M2Locker.BeginRead(ref t));    // 第二位读者：CAS 期望值 0 ≠ Target 2 → 自旋超时
        Assert.Equal(2, t);                         // 计数未被破坏
        Assert.True(env.Tick >= 1000u, "第二读者应自旋到超时，实测 tick=" + env.Tick);

        // 第一读者释放后，读者位置让出 → 下一位读者可进（冻结时钟，保证「一次成功」且读数确定）
        M2Locker.EndRead(ref t);
        Assert.Equal(0, t);
        FreezeClock(env);
        Assert.True(M2Locker.BeginRead(ref t));
        Assert.Equal(2, t);
        M2Locker.EndRead(ref t);
        Assert.Equal(0, t);
        Assert.True(M2Locker.BeginRead(ref t));     // 再次进入仍可（顺序获取正常）
        Assert.Equal(2, t);
        M2Locker.EndRead(ref t);
        Assert.Equal(0, t);
    }

    [Fact]
    public void BeginRead_PreservesUpperBits_ClearsWriteFlag()
    {
        using var env = new SweepTestEnv();
        // 原文 BeginRead（M2Locker.pas:313-323）：CAS 目标 = (Target & $FFFFFFFC) + 2。
        // 目标位模式必须满足 (Target & $FFFFFFFC) == Target —— 否则 CAS 永不成功，只会自旋到超时。
        // 因此「写标志」（Bit0）原文本例只要为 1 就必然超时；能成功的是**只有高位计数**的情形。
        int t = 4;                                // 二进制 …0100 → 掩码后仍是 4
        Assert.True(M2Locker.BeginRead(ref t));
        Assert.Equal(6, t);                       // 4 + 2
        M2Locker.EndRead(ref t);
        Assert.Equal(4, t);                       // 减 2 回到高位基准
        M2Locker.EndRead(ref t);
        Assert.Equal(2, t);
        M2Locker.EndRead(ref t);
        Assert.Equal(0, t);
    }

    [Fact]
    public void BeginRead_WriteFlagSet_TimesOutAsDocumented()
    {
        using var env = new SweepTestEnv();
        // Bit0 = 1（写者持锁）→ 掩码后 CurrentReference=0，而 Target=5 → CAS 永不成功 → 1000ms 超时。
        // 这正是原文注释「等待写入器复位写入标志，因此 Target.Bit0 必须为 0」的字面含义。
        int t = unchecked((int)0x00000004) | 1;   // = 5
        Assert.False(M2Locker.BeginRead(ref t));
        Assert.Equal(5, t);                       // 超时不改 Target（原文如此）
        Assert.True(env.Tick >= 1000u, "应至少自旋 1000 次后超时，实测 " + env.Tick);
    }

    [Fact]
    public void BeginRead_PlainWriteFlag_TimesOutAt1000ms()
    {
        using var env = new SweepTestEnv();
        // Target = 1（纯写标志）→ 掩码后 CurrentReference 恒为 0，
        // CAS(0, 2) 在 Target=1 时失败 → 自旋 → 1000ms 超时。
        int t = 1;
        Assert.False(M2Locker.BeginRead(ref t));
        Assert.Equal(1, t);                       // 超时不改 Target（原文如此）
        Assert.True(env.Tick >= 1000u, "应至少自旋 1000 次后超时，实测 " + env.Tick);
    }

    [Fact]
    public void EndRead_NegativeTarget_WrapsAsTwosComplement()
    {
        using var env = new SweepTestEnv();
        int t = 0;
        M2Locker.EndRead(ref t);          // 无守卫：直接从 0 减 2（原文如此）
        Assert.Equal(-2, t);
        M2Locker.EndRead(ref t);
        Assert.Equal(-4, t);
    }

    // ------------------------------------------------------------------ BeginWrite / EndWrite（原文缺陷断言）

    [Fact]
    public void BeginWrite_IncrementsByOne_AndEndWrite_ResetsToZero()
    {
        using var env = new SweepTestEnv();
        int t = 0;
        Assert.True(M2Locker.BeginWrite(ref t));
        Assert.Equal(1, t);              // CurrentReference(0) + 1
        M2Locker.EndWrite(ref t);
        Assert.Equal(0, t);              // 原文 369-372：非原子直接赋值 0
    }

    /// <summary>
    /// **差异/缺陷断言**：原文 M2Locker.pas:354 是 <c>if not Result then</c>（守卫写反）。
    /// 第一段 CAS 成功时 Result 仍为 True → **第二段「等待所有读取」被整段跳过**，写者不等读者。
    /// <para>本用例用 tick 消耗量把两段区分开：第一段每次迭代取 1 次 tick，
    /// 若守卫正确，第一段失败后第二段「等 Target=1」还会再消耗约 1000 次 tick 才返回。</para>
    /// </summary>
    [Fact]
    public void BeginWrite_DoesNotWaitForActiveReader_OriginalInvertedGuard()
    {
        using var env = new SweepTestEnv();
        int t = 0;
        Assert.True(M2Locker.BeginRead(ref t));     // 一位读者持锁 → t = 2
        Assert.Equal(2, t);

        // 第一段：掩码后 CurrentReference=0，CAS(0,1) 对 Target=2 永不成功 → 自旋到 1000ms 超时。
        SweepSeam.MyGetTickCount = () => env.Tick++;
        bool failed = M2Locker.BeginWrite(ref t);
        Assert.False(failed);
        Assert.Equal(2, t);                         // 读者计数未被破坏（超时不改 Target）

        // 对照：读者释放后，第一段一次 CAS 即成功；第二段因 Result==True 被跳过 →
        // BeginWrite 立即返回 True，没有任何 1000ms 自旋（冻结时钟下若进第二段会永久阻塞）。
        M2Locker.EndRead(ref t);
        Assert.Equal(0, t);
        FreezeClock(env);
        Assert.True(M2Locker.BeginWrite(ref t));
        Assert.Equal(1, t);
    }

    /// <summary>
    /// 守卫写反的补充证明：第一段一次 CAS 成功（<c>Result==True</c>）时，
    /// 原文第 352 行重置 <c>StartTick</c> 后紧接 <c>if not Result</c> → 条件为假 → 第二段整段跳过。
    /// 冻结时钟下整个 <c>BeginWrite</c> 立刻返回 True，<b>不会</b>发生任何 1000ms 自旋。
    /// </summary>
    [Fact]
    public void BeginWrite_FirstSegmentSucceeds_SecondGuardNeverEntered()
    {
        using var env = new SweepTestEnv();
        int t = 0;
        FreezeClock(env);
        Assert.True(M2Locker.BeginWrite(ref t));
        Assert.Equal(1, t);
        Assert.Equal(0u, env.Tick);       // 时钟被冻结且未见自旋（若第二段执行，这里会长时间阻塞）
    }

    [Fact]
    public void EndWrite_NonAtomicDirectAssign_ClearsReaderCountToo()
    {
        using var env = new SweepTestEnv();
        int t = 3;                    // 原文 EndWrite 不关心计数，直接清 0
        M2Locker.EndWrite(ref t);
        Assert.Equal(0, t);
        M2Locker.EndWrite(ref t);
        Assert.Equal(0, t);
        M2Locker.EndWrite(ref t);
        Assert.Equal(0, t);
    }

    // ------------------------------------------------------------------ TSafeList

    [Fact]
    public void SafeList_Ctor_FieldsAndTListSurface()
    {
        using var env = new SweepTestEnv();
        using var list = new TSafeList("L");
        Assert.Equal("L", list.FName);
        Assert.False(list.FIsLock);
        Assert.Equal(0, list.FLocker);
        Assert.Equal(0, list.FLockerID);
        Assert.Equal(0, list.Count);
        Assert.Equal(-1, list.IndexOf(null));   // TList.IndexOf 未找到 = -1
    }

    [Fact]
    public void SafeList_AddItemsDeleteClear_ReturnsIndexAndThrowsOnBadIndex()
    {
        using var env = new SweepTestEnv();
        using var list = new TSafeList("L");
        Assert.Equal(0, list.Add("a"));
        Assert.Equal(1, list.Add("b"));
        Assert.Equal(2, list.Add(null));
        Assert.Equal(3, list.Count);
        Assert.Equal("a", list[0]);
        Assert.Null(list[2]);
        Assert.Equal(1, list.IndexOf("b"));
        Assert.Equal(-1, list.IndexOf("z"));

        list[1] = "B";
        Assert.Equal("B", list[1]);
        list.Delete(1);
        Assert.Equal(2, list.Count);
        Assert.Equal("a", list[0]);
        Assert.Null(list[1]);

        Assert.Throws<ArgumentOutOfRangeException>(() => list[5]);   // TList 越界读抛异常
        Assert.Throws<ArgumentOutOfRangeException>(() => list.Delete(5));
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void SafeList_LockR_SetsFIsLockAndLockerID_AndUnLockR_KeepsLockerID_OriginalAsymmetry()
    {
        using var env = new SweepTestEnv();
        using var list = new TSafeList("L");
        list.LockR(42);
        Assert.True(list.FIsLock);
        Assert.Equal(2, list.FLocker);          // BeginRead 计数
        Assert.Equal(42, list.FLockerID);

        list.UnLockR();
        Assert.False(list.FIsLock);
        Assert.Equal(0, list.FLocker);
        // **差异断言**：原文 M2Locker.pas:138-146 的 TSafeList.UnLockR **没有**清 FLockerID
        // （TSafeStringList/TM2CriticalSection 的同名方法都清了）—— 逐字保留。
        Assert.Equal(42, list.FLockerID);
    }

    [Fact]
    public void SafeList_LockW_SuccessAndDeadlockLog()
    {
        using var env = new SweepTestEnv();
        using var list = new TSafeList("名单甲");
        list.LockW(7);
        Assert.True(list.FIsLock);
        Assert.Equal(1, list.FLocker);
        Assert.Equal(7, list.FLockerID);
        list.UnLockW();
        Assert.Equal(0, list.FLocker);
        Assert.Equal(0, list.FLockerID);        // UnLockW 清 FLockerID（与 UnLockR 不对称）
        Assert.False(list.FIsLock);

        // 死锁路径：预置 FLocker 的 Bit0 已被占 → BeginWrite 第一段自旋超时 → 输出「发现死锁」
        SweepSeam.LoggedMessages.Clear();
        var victim = new TSafeList("名单丙") { FLocker = 1 };
        env.Tick = 0;
        SweepSeam.MyGetTickCount = () => env.Tick++;
        victim.LockW(99);
        Assert.True(victim.FIsLock);             // 原文先 FIsLock := True，再尝试
        Assert.Equal(0, victim.FLockerID);       // 失败 → 不写 FLockerID
        Assert.Single(SweepSeam.LoggedMessages);
        Assert.Equal("发现死锁:名单丙; 0, 99", SweepSeam.LoggedMessages[0]);
        victim.Dispose();
    }

    [Fact]
    public void SafeList_LockR_DeadlockPath_LogsNothing_OriginalCommentedOut()
    {
        using var env = new SweepTestEnv();
        SweepSeam.LoggedMessages.Clear();
        using var list = new TSafeList("L") { FLocker = 1 };   // Bit0 被占 → BeginRead 永不成功
        env.Tick = 0;
        SweepSeam.MyGetTickCount = () => env.Tick++;
        list.LockR(5);
        Assert.True(list.FIsLock);
        Assert.Equal(0, list.FLockerID);
        // 原文 M2Locker.pas:131 把 MainOutMessage 整行注释掉（只留空 else）→ 不产生任何日志
        Assert.Empty(SweepSeam.LoggedMessages);
    }

    [Fact]
    public void SafeList_Dispose_ResetsLockState()
    {
        using var env = new SweepTestEnv();
        var list = new TSafeList("L");
        list.LockW(3);
        list.Dispose();
        Assert.False(list.FIsLock);
        Assert.Equal(0, list.FLocker);
        // 原文 destructor 不动 FLockerID
        Assert.Equal(3, list.FLockerID);
    }

    // ------------------------------------------------------------------ TSafeStringList

    [Fact]
    public void SafeStringList_InheritsTStringList_AndLockState()
    {
        using var env = new SweepTestEnv();
        using var list = new TSafeStringList("S");
        Assert.Equal("S", list.FName);
        Assert.False(list.FIsLock);
        Assert.Equal(0, list.FLocker);
        Assert.Equal(0, list.FLockerID);
        Assert.Equal(0, list.Count);              // 继承自 GXX.Core.Util.TStringList

        list.LockW(11);
        Assert.True(list.FIsLock);
        Assert.Equal(1, list.FLocker);
        Assert.Equal(11, list.FLockerID);
        list.UnLockW();
        Assert.False(list.FIsLock);
        Assert.Equal(0, list.FLockerID);
    }

    [Fact]
    public void SafeStringList_UnLockR_ClearsLockerID_UnlikeSafeList()
    {
        using var env = new SweepTestEnv();
        using var list = new TSafeStringList("S");
        list.LockR(8);
        Assert.Equal(2, list.FLocker);
        Assert.Equal(8, list.FLockerID);
        list.UnLockR();
        Assert.Equal(0, list.FLocker);
        Assert.False(list.FIsLock);
        // **差异断言**：这里**清** FLockerID（M2Locker.pas:228-237），与 TSafeList.UnLockR 相反
        Assert.Equal(0, list.FLockerID);
    }

    [Fact]
    public void SafeStringList_LockWDeadlock_LogsAndKeepsState()
    {
        using var env = new SweepTestEnv();
        SweepSeam.LoggedMessages.Clear();
        using var list = new TSafeStringList("串表") { FLocker = 1 };
        env.Tick = 0;
        SweepSeam.MyGetTickCount = () => env.Tick++;
        list.LockW(6);
        Assert.True(list.FIsLock);
        Assert.Equal(0, list.FLockerID);
        Assert.Equal("发现死锁:串表; 0, 6", SweepSeam.LoggedMessages[0]);
    }

    [Fact]
    public void SafeStringList_Dispose_ResetsLockState()
    {
        using var env = new SweepTestEnv();
        var list = new TSafeStringList("S");
        list.LockW(4);
        list.Dispose();
        Assert.False(list.FIsLock);
        Assert.Equal(0, list.FLocker);
        Assert.Equal(4, list.FLockerID);          // 原文 destructor 不动 FLockerID
    }

    // ------------------------------------------------------------------ TM2CriticalSection

    [Fact]
    public void M2CriticalSection_CtorAndDispose()
    {
        using var env = new SweepTestEnv();
        var cs = new TM2CriticalSection("C");
        Assert.Equal("C", cs.FName);
        Assert.False(cs.FIsLock);
        Assert.Equal(0, cs.FLockerID);
        cs.LockW(2);
        Assert.True(cs.FIsLock);
        cs.Dispose();
        Assert.False(cs.FIsLock);
        Assert.Equal(2, cs.FLockerID);            // 原文 destructor 不动 FLockerID
    }

    [Fact]
    public void M2CriticalSection_LockRUnLockR_ClearsLockerID()
    {
        using var env = new SweepTestEnv();
        using var cs = new TM2CriticalSection("C");
        cs.LockR(1);
        Assert.True(cs.FIsLock);
        Assert.Equal(1, cs.FLockerID);
        cs.UnLockR();
        Assert.Equal(0, cs.FLockerID);
        Assert.False(cs.FIsLock);

        cs.LockW(2);
        Assert.Equal(2, cs.FLockerID);
        cs.UnLockW();
        Assert.Equal(0, cs.FLockerID);
        Assert.False(cs.FIsLock);
    }

    /// <summary>
    /// **差异断言（三份实现不对称）**：同样走死锁/超时失败路径，
    /// <list type="bullet">
    /// <item><c>TSafeList.LockR</c>（M2Locker.pas:121-136）—— <c>MainOutMessage</c> 整行被注释掉 → **静默**；</item>
    /// <item><c>TSafeStringList.LockR</c>（:211-226）与 <c>TM2CriticalSection.LockR</c>（:414-429）—— **照常输出**。</item>
    /// </list>
    /// </summary>
    [Fact]
    public void M2CriticalSection_DeadlockPaths_LogForBothReadAndWrite()
    {
        using var env = new SweepTestEnv();
        SweepSeam.LoggedMessages.Clear();

        using var wr = new TM2CriticalSection("写者");
        wr.SetSectionForTest(1);                  // 原文 protected FSection（M2Locker.pas:60）
        wr.LockW(21);
        Assert.Equal("发现死锁:写者; 0, 21", SweepSeam.LoggedMessages[0]);

        using var rd = new TM2CriticalSection("读者");
        rd.SetSectionForTest(1);
        rd.LockR(22);
        // 与 TSafeList.LockR 不同：这里的失败路径**会**输出（原文如此）
        Assert.Equal(2, SweepSeam.LoggedMessages.Count);
        Assert.Equal("发现死锁:读者; 0, 22", SweepSeam.LoggedMessages[1]);
    }

    /// <summary>
    /// 与 <see cref="SweepM2LockerTests.BeginRead_SecondConcurrentReader_TimesOut_ReadLockNotReentrant"/>
    /// 同一根因：TM2CriticalSection 的 <c>FSection</c> 也是同一个「掩码后 CAS」手法 →
    /// 第二次并发 <c>LockR</c> 必然自旋超时，<c>FLockerID</c> 不会被第二次覆盖。
    /// </summary>
    [Fact]
    public void M2CriticalSection_SecondConcurrentReader_TimesOut()
    {
        using var env = new SweepTestEnv();
        using var cs = new TM2CriticalSection("C");
        cs.LockR(1);
        Assert.Equal(1, cs.FLockerID);
        cs.LockR(2);                              // 第二次：CAS 期望值 0 ≠ FSection 2 → 超时
        Assert.Equal(1, cs.FLockerID);            // 未被覆盖（原文失败路径不写 FLockerID）
        cs.LockR(3);
        Assert.Equal(1, cs.FLockerID);
        cs.UnLockR();
        Assert.Equal(0, cs.FLockerID);
    }

    [Fact]
    public void M2CriticalSection_SequentialReadersAreFine()
    {
        using var env = new SweepTestEnv();
        using var cs = new TM2CriticalSection("C");
        cs.LockR(1);                                  // 无并发读者 → 一次 CAS 成功
        Assert.Equal(1, cs.FLockerID);
        cs.UnLockR();
        Assert.Equal(0, cs.FLockerID);
        cs.LockR(5);                                  // 顺序（非并发）读者正常
        Assert.Equal(5, cs.FLockerID);
        cs.UnLockR();
        Assert.Equal(0, cs.FLockerID);
        cs.LockW(9);                                  // 写锁同样正常
        Assert.Equal(9, cs.FLockerID);
        cs.UnLockW();
        Assert.Equal(0, cs.FLockerID);
    }
}
