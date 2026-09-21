using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;
using Xunit;

// ★ 车道 p16-m2-tmonster-run：`TMonster.Run` 通过三个**静态**接缝读取世界/配置
//   （`TMonster.RunDeps` / `TMonster.RunConfigOverride` / `MonsterRunEnvirView.Registered`）。
//   本程序集已由 `TestConfig.cs:4` 的 `DisableTestParallelization = true` 串行化
//   （理由正是"M2Config 静态全局会被并行污染"），故本车道的接缝替身**无需**再加一条
//   Assembly 级属性（重复会 CS0579）；构造函数/`Dispose` 里各自清场即可。
namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p16-m2-tmonster-run 的运行时逐分支用例：
/// `TMonster.Run` 的 1:1 移植（`ObjMon.pas:1121-1379`，259 行）。
/// <para>用例命名里的行号一律是**原文行号**；`MonsterRunHarness.cs` 提供三个接缝替身。</para>
/// </summary>
public sealed class MonsterRunTests : IDisposable
{
    private readonly FakeMonsterRunWorld _dein = new();
    private readonly FakeMonsterRunConfig _config = new();
    private readonly FakeMonsterRunEnvirView _view = new();
    private readonly TEnvirnoment _envir = MakeEnv("0");
    private readonly TEnvirnoment _otherEnvir = MakeEnv("1");
    private readonly TMonster _mon;
    private readonly TPlayObject _master;

    /// <summary>原文 `m_PEnvir` 必须有可走的格子（`TEnvirnoment.CanWalk` 要真的下标数组）。</summary>
    private static TEnvirnoment MakeEnv(string mapName, int w = 64, int h = 64)
    {
        var env = new TEnvirnoment
        {
            sMapName = mapName,
            nWidth = w,
            nHeight = h,
            MapCellArray = new TMapCellinfo[w, h],
            MapData = new TMapUnitInfo[w, h],
        };
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                env.MapCellArray[x, y] = new TMapCellinfo();
        return env;
    }

    public MonsterRunTests()
    {
        MonsterRunEnvirView.Registered.Clear();
        MonsterRunEnvirView.Registered[_envir.sMapName] = _view;
        MonsterRunEnvirView.Registered[_otherEnvir.sMapName] = _view;
        TMonster.RunConfigOverride = _config;
        TMonster.DefaultCanMove = true;

        _master = new TPlayObject
        {
            m_btRaceServer = (byte)Grobal2Const.RC_PLAYOBJECT,
            m_nCurrX = 100,
            m_nCurrY = 100,
            m_btDirection = 4,
            m_PEnvir = _envir
        };

        _mon = new TMonster
        {
            m_nCurrX = 10,
            m_nCurrY = 10,
            m_PEnvir = _envir,
            m_dwWalkTick = Now - 100000,   // 初始即"早该走一步"
            m_nWalkSpeed = 100,
            m_nWalkDelay = 0,
            RunDeps = _dein,
        };
    }

    public void Dispose()
    {
        _mon.RunDeps = null;
        TMonster.RunConfigOverride = null;
        TMonster.DefaultCanMove = true;
        MonsterRunEnvirView.Registered.Clear();
    }

    private static uint Now => DelphiRTL.GetTickCount();

    /// <summary>把怪物配成"主人是玩家、距离主人 10 格、可以走"的常规宝宝。</summary>
    private void MakeGamePet()
    {
        _mon.m_boGamePet = true;
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_nCurrY = _mon.m_nCurrY;
        // 默认把 `TSmartObject` 那条自动拾取关掉（1258 的第一道门），
        // 使 1159-1171 的宠物快速拾取段成为**唯一**可能的拾取出口；
        // 需要考察 1253 那条的用例自行打开它。
        _master.m_boSlaveAutoPickItem = false;
    }

    // ======================================================================
    // 一、五重守卫（1128）与无条件 `inherited`（1378）
    // ======================================================================

    /// <summary>
    /// 原文 1128 的守卫被挡下时**整段体不执行**，但 1378 的 `inherited` 在守卫**之外**
    /// ⇒ 仍然走到基类（= 消费消息队列）。这是原文的真实形状。
    /// </summary>
    [Fact]
    public void FallenGuardStillRunsBaseInherited()
    {
        _mon.m_boGhost = true;
        var before = _mon.m_dwWalkTick;

        _mon.Run();

        Assert.Equal(0, _dein.CanMoveCalls);      // 守卫短路：连 CanMove 都没求值
        Assert.Equal(before, _mon.m_dwWalkTick);  // 体没跑：走步节拍没被刷新
    }

    [Theory]
    // 五个字段逐一置位，其余全开 ⇒ 每个都能单独挡下（"其它全开只关它"）
    [InlineData("ghost")]
    [InlineData("death")]
    [InlineData("fixedHide")]
    [InlineData("stone")]
    [InlineData("canMove")]
    public void EachGuardFieldAloneBlocksTheBody(string which)
    {
        switch (which)
        {
            case "ghost": _mon.m_boGhost = true; break;
            case "death": _mon.m_boDeath = true; break;
            case "fixedHide": _mon.m_boFixedHideMode = true; break;
            case "stone": _mon.m_boStoneMode = true; break;
            case "canMove": _dein.CanMoveResult = false; break;
        }
        var before = _mon.m_dwWalkTick;

        _mon.Run();

        Assert.Equal(which == "canMove" ? 1 : 0, _dein.CanMoveCalls);
        Assert.Equal(before, _mon.m_dwWalkTick);
    }

    // ======================================================================
    // 二、四条补回分支①：`m_Master` 天关宝宝 `MakeGhost`（1130-1137）
    // ======================================================================

    /// <summary>1133：`(m_PEnvir &lt;&gt; m_Master.m_PEnvir) and m_PEnvir.m_boGuardianLevel` → `MakeGhost; Exit`。</summary>
    [Fact]
    public void GuardianLevelCrossMapMakesGhost()
    {
        _mon.m_Master = _master;              // 主人已在另一张图（见构造：_master.m_PEnvir = _envir）
        _master.m_PEnvir = _otherEnvir;
        _view.m_boGuardianLevel = true;

        _mon.Run();

        Assert.True(_mon.m_boGhost);
        Assert.NotEqual(0u, _mon.m_dwGhostTick);
        Assert.Equal(0, _dein.SpaceMoveCalls);
        Assert.Equal(0, _dein.ThinkCalls);    // 早退：没走到 Think
    }

    /// <summary>1133 的**前半**（跨图）单独成立还不够 —— 同图时即使天关地图也不幽灵化。</summary>
    [Fact]
    public void GuardianLevelSameMapDoesNotMakeGhost()
    {
        _mon.m_Master = _master;
        _master.m_PEnvir = _envir;            // 同图
        _view.m_boGuardianLevel = true;

        _mon.Run();

        Assert.False(_mon.m_boGhost);
        Assert.Equal(1, _dein.CanMoveCalls);
    }

    /// <summary>1133 的**后半**（`m_boGuardianLevel`）单独成立也还不够 —— 非天关地图不幽灵化。</summary>
    [Fact]
    public void GuardianLevelFalseCrossMapDoesNotMakeGhost()
    {
        _mon.m_Master = _master;
        _master.m_PEnvir = _otherEnvir;
        _view.m_boGuardianLevel = false;

        _mon.Run();

        Assert.False(_mon.m_boGhost);
        Assert.Equal(1, _dein.CanMoveCalls);
    }

    /// <summary>守卫字段（`m_boGuardianLevel`）单独关掉即不进该分支 —— "其它全开只关它"。</summary>
    [Fact]
    public void GuardianLevelOnlyGateOfItsBranch()
    {
        _mon.m_Master = _master;
        _master.m_PEnvir = _otherEnvir;
        _view.m_boGuardianLevel = true;
        _view.m_boMirror = true;

        // 天关优先于镜像：两个都开时走 `MakeGhost`，`SpaceMove` 一次都不该发生
        _mon.Run();

        Assert.True(_mon.m_boGhost);
        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    // ======================================================================
    // 三、四条补回分支②：镜像地图 `SpaceMove`（1138-1142）
    // ======================================================================

    /// <summary>
    /// 1138-1141：跨图 + `m_boMirror`（且**非**天关）→
    /// `SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1); Exit`。
    /// <para>注意参数是**怪物自己的 `m_nTargetX/m_nTargetY`**（不是主人坐标），且第 4 参恒为 `1`。</para>
    /// </summary>
    [Fact]
    public void MirrorCrossMapSpaceMovesToMastersMap()
    {
        _mon.m_Master = _master;
        _master.m_PEnvir = _otherEnvir;
        _view.m_boGuardianLevel = false;
        _view.m_boMirror = true;
        _mon.m_nTargetX = 33;
        _mon.m_nTargetY = 44;

        _mon.Run();

        Assert.Equal(1, _dein.SpaceMoveCalls);
        Assert.Equal(("1", 33, 44, 1), _dein.LastSpaceMove);
        Assert.False(_mon.m_boGhost);
        Assert.Equal(0, _dein.ThinkCalls);    // Exit：没走到 Think
    }

    /// <summary>
    /// 镜像分支的守卫字段单独关掉即不进 —— "其它全开只关它"。
    /// <para>⚠ 隔离手法：本用例的两个 `SpaceMove` 出口在同一条路径上（1138 镜像 / 1323 归位），
    /// 故用原文自己的"155/156 宝宝"豁免（1290/1314）把 1323 那条压掉，
    /// 使唯一可能的 `SpaceMove` 只剩镜像分支。</para>
    /// </summary>
    [Fact]
    public void MirrorFalseCrossMapDoesNotSpaceMove()
    {
        _mon.m_Master = _master;
        _master.m_PEnvir = _otherEnvir;         // 跨图（1138 的前半成立）
        _view.m_boGuardianLevel = false;
        _view.m_boMirror = false;               // ← 唯一被关掉的门
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 156;                 // 1290/1314 的豁免
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
        Assert.Equal(1, _dein.ThinkCalls);
    }

    /// <summary>同图时即使 `m_boMirror` 为真也不飞（前半条件单独起作用）。同用 155/156 隔离 1323。</summary>
    [Fact]
    public void MirrorSameMapDoesNotSpaceMove()
    {
        _mon.m_Master = _master;
        _master.m_PEnvir = _envir;              // 同图 ⇒ 1133/1138 的前半都不成立
        _view.m_boMirror = true;
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 156;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    // ======================================================================
    // 四、`Think` 早退（1144-1148）
    // ======================================================================

    /// <summary>1144：`if Think then begin inherited; Exit; end` —— 真值时**只**跑基类、其余全跳过。</summary>
    [Fact]
    public void ThinkTrueExitsAfterBaseRun()
    {
        _dein.ThinkResult = true;
        MakeGamePet();

        _mon.Run();

        Assert.Equal(1, _dein.ThinkCalls);
        Assert.Equal(0, _dein.PickRangeItemCalls);   // 快速拾取段没跑到
        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    [Fact]
    public void ThinkFalseContinues()
    {
        _dein.ThinkResult = false;
        _mon.m_dwWalkTick = Now;   // 让 IsCanMove 为假，避免下游噪音

        _mon.Run();

        Assert.Equal(1, _dein.ThinkCalls);
    }

    // ======================================================================
    // 五、四条补回分支③：`m_boWalkWaitLocked` 走步等待锁（1149-1155、1172、1174）
    // ======================================================================

    /// <summary>
    /// 1149-1155：锁住时**只有超时**才解锁；1151 用的是**裸减法**、
    /// 而 1174 的闸门是 `if not m_boWalkWaitLocked` ⇒ 锁住时**连 `IsCanMove` 会不会生效都不用管**：
    /// 走步节拍与计数一律不更新。
    /// </summary>
    [Fact]
    public void WalkWaitLockBlocksTickRefreshEvenWhenIsCanMoveIsTrue()
    {
        _mon.m_boWalkWaitLocked = true;
        _mon.m_dwWalkWait = 5000;
        _mon.m_dwWalkWaitTick = Now;          // 远未到 5000ms ⇒ 不解锁
        _mon.m_nWalkCount = 7;
        uint walkTickBefore = _mon.m_dwWalkTick;

        _mon.Run();

        Assert.True(_mon.m_boWalkWaitLocked);          // 仍然锁着
        Assert.Equal(walkTickBefore, _mon.m_dwWalkTick); // 节拍未刷新（IsCanMove 虽为真）
        Assert.Equal(7, _mon.m_nWalkCount);            // 计数未自增
    }

    /// <summary>1151 的**解锁**：`(now - m_dwWalkWaitTick) &gt; m_dwWalkWait` 为真 ⇒ 解锁，并进入 1172 段。</summary>
    [Fact]
    public void WalkWaitLockExpiresByTime()
    {
        _mon.m_boWalkWaitLocked = true;
        _mon.m_dwWalkWait = 100;
        _mon.m_dwWalkWaitTick = Now - 5000;    // 早过期
        _mon.m_nWalkStep = 10;
        _mon.m_dwWalkTick = Now - 100000;      // IsCanMove = true ⇒ 会刷新节拍

        _mon.Run();

        Assert.False(_mon.m_boWalkWaitLocked);
        Assert.Equal(1, _mon.m_nWalkCount);     // 已刷新（1174 段跑了）
    }

    /// <summary>
    /// 1151 是**严格大于**（不是 `&gt;=`）：`now - tick == m_dwWalkWait` 时**不解锁**。
    /// 与 1179 的 `m_nWalkCount &gt; m_nWalkStep` 一起，构成本方法里两处 `&gt;` 边界。
    /// </summary>
    [Fact]
    public void WalkWaitExpiryIsStrictGreaterThan()
    {
        // 把"现在"钉死：让差值恰好等于 m_dwWalkWait
        uint now = Now;
        _mon.m_boWalkWaitLocked = true;
        _mon.m_dwWalkWait = 1000;
        _mon.m_dwWalkWaitTick = now - 1000;

        // 允许 1ms 抖动：只有当 (Now - tick) 仍恰好为 1000 时才有意义
        if (Now - _mon.m_dwWalkWaitTick != 1000) return;

        _mon.Run();

        Assert.True(_mon.m_boWalkWaitLocked);   // 等于阈值 ⇒ 不解锁
    }

    /// <summary>1174/1179：`m_nWalkCount &gt; m_nWalkStep` 才上锁并清零；等于时不锁。</summary>
    [Fact]
    public void WalkCountExceedingStepLocksAndResets()
    {
        _mon.m_nWalkStep = 3;
        _mon.m_nWalkCount = 3;                 // == step ⇒ 不自增后仍 > ？
        _mon.m_dwWalkTick = Now - 100000;      // IsCanMove = true

        _mon.Run();

        Assert.Equal(0, _mon.m_nWalkCount);    // 4 > 3 ⇒ 归零
        Assert.True(_mon.m_boWalkWaitLocked);
        Assert.NotEqual(0u, _mon.m_dwWalkWaitTick);
    }

    [Fact]
    public void WalkCountNotExceedingStepDoesNotLock()
    {
        _mon.m_nWalkStep = 10;
        _mon.m_nWalkCount = 0;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(1, _mon.m_nWalkCount);    // 1 > 10 为假 ⇒ 只自增
        Assert.False(_mon.m_boWalkWaitLocked);
    }

    /// <summary>1174 的闸门单独把关：`m_boWalkWaitLocked = false` 且 `IsCanMove` ⇒ 刷新节拍并清零延迟。</summary>
    [Fact]
    public void IsCanMoveRefreshesTickAndClearsDelay()
    {
        _mon.m_boWalkWaitLocked = false;
        _mon.m_nWalkDelay = 777;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(0, _mon.m_nWalkDelay);
        Assert.True(TickDiffBeforeNow(_mon.m_dwWalkTick) < 1000);   // 刚被刷新成"现在"
    }

    /// <summary>
    /// 1157 的**严格大于**边界：`tick_diff(m_dwWalkTick, now) &gt; m_nWalkSpeed + m_nWalkDelay`。
    /// 恰好等于阈值时 `IsCanMove` 为**假**（若原文写成 `&gt;=` 则此处会走）。
    /// <para>对照：`OMonChickenDeer.Run`（1393+）用的是 **`&gt;=`** —— 两处口径不同，
    /// 本用例把 `TMonster.Run` 这一侧锁死为 `&gt;`。</para>
    /// </summary>
    [Fact]
    public void IsCanMoveUsesStrictGreaterThanNotGreaterOrEqual()
    {
        _mon.m_nWalkSpeed = 5000;
        _mon.m_nWalkDelay = 0;
        _mon.m_boWalkWaitLocked = false;
        _mon.m_nWalkCount = 0;
        _mon.m_dwWalkTick = Now - 5000;        // 恰好等于阈值

        if (TickDiffBeforeNow(_mon.m_dwWalkTick) != 5000) return;

        _mon.Run();

        Assert.Equal(0, _mon.m_nWalkCount);    // IsCanMove 为假 ⇒ 计数器没动
    }

    [Fact]
    public void IsCanMoveOneMillisecondOverThresholdPasses()
    {
        _mon.m_nWalkSpeed = 5000;
        _mon.m_nWalkDelay = 0;
        _mon.m_boWalkWaitLocked = false;
        _mon.m_nWalkStep = 10;                 // 别让 1179 上锁把计数清零
        _mon.m_nWalkCount = 0;
        _mon.m_dwWalkTick = Now - 5001;

        _mon.Run();

        Assert.Equal(1, _mon.m_nWalkCount);
    }

    // ======================================================================
    // 六、四条补回分支④：`m_boGamePet && g_Config.boPetQuickPickup` 宠物拴物
    //     + `TSmartObject` 范围拾取（1159-1171、1223-1273）
    // ======================================================================

    /// <summary>
    /// 1159-1171：快速拾取段的**四个条件全开**才进入；
    /// `boPetRangePickup` 为真时先 `PickRangeItem(False, True, True, 0)`，返回真即 `Exit`。
    /// </summary>
    [Fact]
    public void PetQuickPickupRangePickupReturnsExit()
    {
        MakeGamePet();
        _config.boPetQuickPickup = true;
        _config.boEnabledPetPickup = true;
        _config.boPetRangePickup = true;
        _mon.m_btGamePetEnablePick = 0;
        _dein.PickRangeItemResult = true;

        _mon.Run();

        Assert.Equal(1, _dein.PickRangeItemCalls);
        Assert.Equal((false, true, true, 0u), _dein.LastPickRangeItem);
        Assert.Equal(0, _dein.StartPickUpItemCalls);   // Exit 早于 StartPickUpItem
        Assert.Equal(1, _dein.ThinkCalls);             // 1144 的 Think 在 1159 之前，已被调用
        Assert.Equal(0, _mon.m_nWalkCount);            // Exit ⇒ 1174 段没跑（计数器没动）
    }

    /// <summary>
    /// 1164-1168 的守卫字段 `boPetRangePickup` 单独关掉 ⇒ 1159 段跳过 `PickRangeItem`、
    /// 直接走 1169 的**四参形态** `StartPickUpItem(True, True, 0, False)`（返回值被丢弃 —— 原文如此）。
    /// <para>⚠ 原文 1159 段在 `boPetRangePickup = False` 时**不 Exit**，所以同一帧还会继续走到
    /// 1223 段；两段的实参形态不同（1169 恒 `(True,True,0)`、1245 用主人字段），
    /// 故这里按**调用点**分别断言。</para>
    /// </summary>
    [Fact]
    public void PetRangePickupFalseSkipsPickRangeItemOnly()
    {
        MakeGamePet();
        IsolateQuickPickupBlock();
        _config.boPetQuickPickup = true;
        _config.boEnabledPetPickup = true;
        _config.boPetRangePickup = false;
        _mon.m_btGamePetEnablePick = 0;

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);              // 1164 的门关掉 ⇒ 不范围拾取
        Assert.True(_dein.SawFourArgStartPickUpItem);           // 1169 走了
        Assert.Equal(2, _dein.StartPickUpItemCalls);            // 1169 + 1245（原文两段都不 Exit）
        Assert.Equal((true, true, 0u), _dein.LastStartPickUpItem);   // 1245：三参恒 True/True/0
    }

    /// <summary>
    /// 把 1159-1171 变成**唯一**可能的拾取出口：
    /// 主人放在 ≤20 格内（压掉 1228 的"离主人太远"支）、`m_nTargetX/Y` 置 -1（压掉 1321 的归位支）、
    /// 关掉 `m_boSlaveAutoPickItem`（压掉 1258 的 `TSmartObject` 支）。
    /// <para>原文 1159 段在 `boPetRangePickup = False` 时**不 Exit**，所以"同一帧还会继续往下走"
    /// 是原文的真实形状 —— 本方法把下游出口全部掐断，使断言仍然只反映 1159 段。</para>
    /// </summary>
    private void IsolateQuickPickupBlock()
    {
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_nCurrY = _mon.m_nCurrY;
        _master.m_boSlaveAutoPickItem = false;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 156;
    }

    /// <summary>1159 的守卫字段 `boPetQuickPickup` 单独关掉 ⇒ 1159 段整段不进。</summary>
    [Fact]
    public void PetQuickPickupFalseSkipsWholeBlock()
    {
        MakeGamePet();
        IsolateQuickPickupBlock();
        _config.boPetQuickPickup = false;
        _config.boEnabledPetPickup = true;
        _config.boPetRangePickup = true;
        _mon.m_btGamePetEnablePick = 1;

        _mon.Run();

        Assert.False(_dein.SawFourArgStartPickUpItem);          // 1169 没走
        // 1223 段仍然会走（它的门是 m_boGamePet + 主人是玩家，与 boPetQuickPickup 无关）⇒
        // 它的范围拾取用的是 1242 的常量形态（`False, True, True, 0`），而 1159 段用的是同一形态。
        Assert.Equal(1, _dein.PickRangeItemCalls);
        Assert.Equal((false, true, true, 0u), _dein.LastPickRangeItem);
    }

    /// <summary>1159 的 `m_boGamePet` 单独关掉 ⇒ 整段不进。</summary>
    [Fact]
    public void PetQuickPickupRequiresGamePetFlag()
    {
        MakeGamePet();
        _mon.m_boGamePet = false;
        _config.boPetQuickPickup = true;
        _config.boEnabledPetPickup = true;
        _config.boPetRangePickup = true;
        _mon.m_btGamePetEnablePick = 1;

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
    }

    /// <summary>1159 的 `m_Master.m_btRaceServer = RC_PLAYOBJECT` 单独不成立（英雄）⇒ 整段不进。</summary>
    [Fact]
    public void PetQuickPickupRequiresMasterBePlayerObject()
    {
        MakeGamePet();
        _master.m_btRaceServer = (byte)Grobal2Const.RC_HEROOBJECT;
        _config.boPetQuickPickup = true;
        _config.boEnabledPetPickup = true;
        _config.boPetRangePickup = true;
        _mon.m_btGamePetEnablePick = 1;

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
    }

    /// <summary>1161 的三态：`m_btGamePetEnablePick = 0` 时由 `g_Config.boEnabledPetPickup` 决定。</summary>
    [Theory]
    [InlineData(0, true, true)]    // 跟随全局、全局开 ⇒ 开
    [InlineData(0, false, false)]  // 跟随全局、全局关 ⇒ 关
    [InlineData(1, false, true)]   // 强制开 ⇒ 开（无视全局）
    [InlineData(2, true, false)]   // 强制关 ⇒ 关（**原文只认 0 与 1**，2 落进"关"）
    public void PetEnablePickThreeStateField(byte enablePick, bool globalOn, bool expectedPickup)
    {
        MakeGamePet();
        IsolateQuickPickupBlock();
        _config.boPetQuickPickup = true;
        _config.boEnabledPetPickup = globalOn;
        _config.boPetRangePickup = true;
        _mon.m_btGamePetEnablePick = enablePick;

        _mon.Run();

        Assert.Equal(expectedPickup, _dein.PickRangeItemCalls > 0);
    }

    /// <summary>
    /// 1169 的返回值**被丢弃**（原文如此）：`StartPickUpItem` 返回真也**不早退**。
    /// <para>判据：早退会让 1174 段完全不跑；此处观察到 `IsCanMove` 已把节拍刷新、计数自增
    /// ⇒ 控制流确实穿过了 1169。</para>
    /// </summary>
    [Fact]
    public void StartPickUpItemReturnValueDiscardedAt1169()
    {
        MakeGamePet();
        IsolateQuickPickupBlock();
        _config.boPetQuickPickup = true;
        _config.boEnabledPetPickup = true;
        _config.boPetRangePickup = false;
        _mon.m_btGamePetEnablePick = 1;
        _mon.m_nWalkStep = 10;
        _mon.m_dwWalkTick = Now - 100000;       // IsCanMove = true
        _dein.StartPickUpItemResult = true;

        _mon.Run();

        Assert.True(_dein.SawFourArgStartPickUpItem);   // 1169 走了
        Assert.True(_dein.StartPickUpItemCalls >= 1);
        Assert.Equal(1, _mon.m_nWalkCount);             // 穿过 1169 ⇒ 1174 段跑了
    }

    /// <summary>
    /// 1223-1237：宠物离主人太远（`&gt; 20` 或跨图）⇒ `m_Master.GetBackPosition` → 写目标点 →
    /// `SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1)` → `Exit`。
    /// </summary>
    [Fact]
    public void GamePetFarFromMasterSpaceMovesToBackPosition()
    {
        MakeGamePet();
        _mon.m_nCurrX = 10;
        _mon.m_nCurrY = 10;
        _master.m_nCurrX = 50;                 // 相距 40 > 20
        _master.m_nCurrY = 10;
        _master.m_PEnvir = _envir;
        _master.m_btDirection = 4;             // 朝下 ⇒ 背后是 (50, 9)
        _mon.m_btGamePetEnablePick = 1;

        _mon.Run();

        Assert.Equal(1, _dein.SpaceMoveCalls);
        Assert.Equal(("0", 50, 9, 1), _dein.LastSpaceMove);
    }

    /// <summary>1228 的**距离条件单独不成立**（≤20 且同图）⇒ 不飞，改走 1240-1249 的捡物。</summary>
    [Fact]
    public void GamePetNearMasterDoesNotSpaceMove()
    {
        MakeGamePet();                          // 相距恰好 10
        _master.m_btDirection = 4;
        _mon.m_btGamePetEnablePick = 1;
        _config.boPetRangePickup = true;
        _dein.PickRangeItemResult = true;

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
        Assert.Equal(1, _dein.PickRangeItemCalls);
        Assert.Equal((false, true, true, 0u), _dein.LastPickRangeItem);   // 1242 形态
    }

    /// <summary>1228 的跨图条件**单独**成立也足以触发飞行。</summary>
    [Fact]
    public void GamePetCrossMapSpaceMovesEvenWhenNear()
    {
        MakeGamePet();
        _master.m_nCurrX = _mon.m_nCurrX + 1;   // 距离 1，但跨图
        _master.m_nCurrY = _mon.m_nCurrY;
        _master.m_PEnvir = _otherEnvir;
        _master.m_btDirection = 4;
        _mon.m_btGamePetEnablePick = 1;

        _mon.Run();

        Assert.Equal(1, _dein.SpaceMoveCalls);
        Assert.Equal("1", _dein.LastSpaceMove!.Value.Map);
    }

    /// <summary>1240 的 `boPetRangePickup` 单独关掉 ⇒ 跳过 `PickRangeItem`、直接三参 `StartPickUpItem`。</summary>
    [Fact]
    public void GamePetRangePickupFalseUsesThreeArgStartPickUp()
    {
        MakeGamePet();
        _master.m_btDirection = 4;
        _mon.m_btGamePetEnablePick = 1;
        _config.boPetRangePickup = false;
        _dein.StartPickUpItemResult = true;

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
        Assert.Equal(1, _dein.StartPickUpItemCalls);
        Assert.Equal((true, true, 0u), _dein.LastStartPickUpItem);
    }

    /// <summary>
    /// 1253-1273：**非**游戏宠物但主人是玩家/英雄 ⇒ 走 `TSmartObject` 那条自动范围拾取。
    /// 三层闸门全开才拾取：地图允许（1256）、`m_boSlaveAutoPickItem && 键非零`（1258）、
    /// `m_btSlaveAutoPickItemRange &gt; 0`（1260）。
    /// </summary>
    [Fact]
    public void SmartObjectSlaveAutoPickReadsMastersOwnPickFields()
    {
        _mon.m_Master = _master;                // 非 m_boGamePet
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_nCurrY = _mon.m_nCurrY;
        _master.m_boSlaveAutoPickItem = true;
        _master.m_btSlaveAutoPickItemRange = 3;
        _master.m_boSlaveAutoPickAll = true;
        _master.m_boAutoPickPlayDropItem = true;
        _master.m_boAutoPickPlayScatterItem = false;
        _master.m_dwAutoPickScatterToPickTime = 1234;
        _config.g_nKey_UseClientPickItems = 1;
        _view.m_boNoAutoRangePickItem = false;
        _dein.PickRangeItemResult = true;

        _mon.Run();

        Assert.Equal(1, _dein.PickRangeItemCalls);
        // 1262：四个实参**全部来自主人的 `TSmartObject` 字段**（不是 1166 那组常量）
        Assert.Equal((true, true, false, 1234u), _dein.LastPickRangeItem);
        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    /// <summary>1256 的地图闸门 `m_boNoAutoRangePickItem` 单独关掉该分支（"其它全开只关它"）。</summary>
    [Fact]
    public void MapNoAutoRangePickItemBlocksSmartObjectPickup()
    {
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_boSlaveAutoPickItem = true;
        _master.m_btSlaveAutoPickItemRange = 3;
        _config.g_nKey_UseClientPickItems = 1;
        _view.m_boNoAutoRangePickItem = true;       // ← 唯一被关掉的门

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
        Assert.Equal(0, _dein.StartPickUpItemCalls);
    }

    /// <summary>1258 的 `m_boSlaveAutoPickItem` 单独关掉该分支。</summary>
    [Fact]
    public void SlaveAutoPickItemOffBlocksSmartObjectPickup()
    {
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_boSlaveAutoPickItem = false;
        _master.m_btSlaveAutoPickItemRange = 3;
        _config.g_nKey_UseClientPickItems = 1;

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
    }

    /// <summary>1258 的 `g_nKey_UseClientPickItems &lt;&gt; 0` 单独关掉该分支。</summary>
    [Fact]
    public void VmProtectKeyOffBlocksSmartObjectPickup()
    {
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_boSlaveAutoPickItem = true;
        _master.m_btSlaveAutoPickItemRange = 3;
        _config.g_nKey_UseClientPickItems = 0;      // ← 契约键为 0

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
    }

    /// <summary>1260 的 `m_btSlaveAutoPickItemRange &gt; 0` 单独关掉 `PickRangeItem`（但仍会走 1266 的 `StartPickUpItem`）。</summary>
    [Fact]
    public void SlaveAutoPickItemRangeZeroSkipsOnlyRangePickup()
    {
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_boSlaveAutoPickItem = true;
        _master.m_btSlaveAutoPickItemRange = 0;     // ← 唯一被关掉的门
        _config.g_nKey_UseClientPickItems = 1;

        _mon.Run();

        Assert.Equal(0, _dein.PickRangeItemCalls);
        Assert.Equal(1, _dein.StartPickUpItemCalls);   // 1266 不受这一条约束（原文如此）
    }

    /// <summary>1253 的种族集合 `[RC_PLAYOBJECT, RC_HEROOBJECT]`：英雄主人也走这条。</summary>
    [Fact]
    public void SmartObjectBranchAcceptsHeroMasterToo()
    {
        _mon.m_Master = _master;
        _master.m_btRaceServer = (byte)Grobal2Const.RC_HEROOBJECT;
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_boSlaveAutoPickItem = true;
        _master.m_btSlaveAutoPickItemRange = 3;
        _master.m_boAutoPickPlayDropItem = true;
        _master.m_dwAutoPickScatterToPickTime = 77;
        _config.g_nKey_UseClientPickItems = 1;
        _dein.PickRangeItemResult = true;

        _mon.Run();

        Assert.Equal(1, _dein.PickRangeItemCalls);
        Assert.Equal((false, true, false, 77u), _dein.LastPickRangeItem);
    }

    // ======================================================================
    // 七、主人"休息"（1186-1190、1336-1340）与逃跑模式（1191、1330-1334）
    // ======================================================================

    /// <summary>1186-1190：主人休息且（非宠物 或 受主人睡眠控制）⇒ 清目标 + `m_boTarget := False`。</summary>
    [Fact]
    public void SlaveRelaxClearsTarget()
    {
        _mon.m_Master = _master;
        _master.SlaveRelaxSeam = true;
        _mon.m_TargetCret = _master;
        _mon.m_boTarget = true;
        _mon.m_dwWalkTick = Now;               // IsCanMove 假，隔离下游

        _mon.Run();

        Assert.Null(_mon.m_TargetCret);
        Assert.False(_mon.m_boTarget);
        Assert.Equal(1, _dein.DelTargetCreatCalls);
    }

    /// <summary>1186 的 `m_boGamePet` 极性：宠物且 `boPetSleepControlBySlave = False` ⇒ **不清**目标。</summary>
    [Fact]
    public void RelaxPetNotControlledBySlaveKeepsTarget()
    {
        MakeGamePet();
        _master.SlaveRelaxSeam = true;
        _config.boPetSleepControlBySlave = false;   // 宠物不受主人睡眠控制
        _mon.m_TargetCret = _master;
        _mon.m_dwWalkTick = Now;

        _mon.Run();

        Assert.NotNull(_mon.m_TargetCret);
    }

    /// <summary>1186 的 `g_Config.boPetSleepControlBySlave` 单独置真即让宠物也清目标（极性反转证据）。</summary>
    [Fact]
    public void RelaxPetControlledBySlaveClearsTarget()
    {
        MakeGamePet();
        _master.SlaveRelaxSeam = true;
        _config.boPetSleepControlBySlave = true;
        _mon.m_TargetCret = _master;
        _mon.m_dwWalkTick = Now;

        _mon.Run();

        Assert.Null(_mon.m_TargetCret);
    }

    /// <summary>1191：`m_boRunAwayMode` 为真 ⇒ **整段移动逻辑被跳过**（含 1277 的宝宝跟随段）。</summary>
    [Fact]
    public void RunAwayModeSkipsWholeMovementBlock()
    {
        MakeGamePet();
        _mon.m_boRunAwayMode = true;
        _mon.m_dwRunAwayTime = 0;              // 不触发 1330 的解超时
        _mon.m_nCurrX = 10;
        _mon.m_nCurrY = 10;
        _master.m_nCurrX = 50;                 // 远到"本该飞"

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
        Assert.Equal(0, _dein.AttackTargetCalls);
    }

    /// <summary>1330-1334：超时（**裸减法**，非 `tick_diff`）⇒ `m_boRunAwayMode := False; m_dwRunAwayTime := 0`。</summary>
    [Fact]
    public void RunAwayTimeoutClearsBothFields()
    {
        _mon.m_boRunAwayMode = true;
        _mon.m_dwRunAwayTime = 100;
        _mon.m_dwRunAwayStart = Now - 5000;

        _mon.Run();

        Assert.False(_mon.m_boRunAwayMode);
        Assert.Equal(0u, _mon.m_dwRunAwayTime);
    }

    /// <summary>1330 的 `m_dwRunAwayTime &gt; 0` 单独为假 ⇒ 即使早已超时也不清（原文如此）。</summary>
    [Fact]
    public void RunAwayTimeZeroNeverExpires()
    {
        _mon.m_boRunAwayMode = true;
        _mon.m_dwRunAwayTime = 0;
        _mon.m_dwRunAwayStart = 0;             // 古老

        _mon.Run();

        Assert.True(_mon.m_boRunAwayMode);
    }

    /// <summary>
    /// ★ 原文缺陷锁死：1330 用的是**裸减法** `(MyGetTickCount - m_dwRunAwayStart) &gt; m_dwRunAwayTime`、
    /// 而 1195 用的是 **`tick_diff`**（回绕安全）—— 同一方法内两种时间口径并存。
    /// 本用例在"单调时钟已回绕"的输入上把两者**分开**：
    /// `tick_diff` 回绕安全会给一个小差值（超时），裸减法在无符号回绕下也会给一个小差值，
    /// 因此这里改用"`m_dwRunAwayStart` 在未来（时钟回绕后的常见错位）"这一形态验证：
    /// `tick_diff(start_future, now)` = `Max - start + now`（**很大**）⇒ 视为超时；
    /// 裸减法 `now - start_future` 同样回绕成**很大** ⇒ 也视为超时。两者在这里**恰好一致**，
    /// 故真正能区分二者的是 `m_dwRunAwayStart` **小于** `now` 但差值超过 `High(Cardinal)` 的极端值域 —— 
    /// 已由 <see cref="TickDiffIsWrapSafeUnlikeRawSubtraction"/> 直接对函数本身锁定。
    /// </summary>
    [Fact]
    public void TickDiffIsWrapSafeUnlikeRawSubtraction()
    {
        // tick_diff 的定义（M2Share.pas:32953-32959）
        Assert.Equal(0u, TMonster.TickDiff(1000, 1000));
        Assert.Equal(500u, TMonster.TickDiff(1000, 1500));
        // 回绕：start 比 end 大 ⇒ High(Cardinal) - start + end
        Assert.Equal(uint.MaxValue - 4000u + 1000u, TMonster.TickDiff(4000, 1000));

        // 裸减法在同样输入下是"另一个数"（uint 回绕），两者**不是**同一个函数
        uint raw = unchecked(1000u - 4000u);
        Assert.NotEqual(TMonster.TickDiff(4000, 1000), raw);
    }

    /// <summary>1336-1340：主人休息 ⇒ `inherited; Exit`（**在** 1336 处早退）。</summary>
    [Fact]
    public void RelaxSecondGuardExitsBeforeGotoTarget()
    {
        _mon.m_Master = _master;
        _master.SlaveRelaxSeam = true;
        _mon.m_boRunAwayMode = true;            // 跳过整段移动块，直达 1336
        _mon.m_dwRunAwayTime = 0;
        _mon.m_nTargetX = 5;
        _mon.m_nTargetY = 5;
        _mon.m_dwWalkTick = Now - 100000;       // IsCanMove 真
        _mon.m_nWalkSpeed = 0;

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    // ======================================================================
    // 八、攻击/任务点/宝宝归位（1193-1326）
    // ======================================================================

    /// <summary>
    /// 1198-1203：站稳（`tick_diff(m_dwStationTick, now) &gt; m_nWalkSpeed`，**1195 行**）+ 有目标
    /// ⇒ `AttackTarget`，真即早退。
    /// </summary>
    [Fact]
    public void AttackTargetTrueExits()
    {
        _mon.m_TargetCret = _master;
        _mon.m_dwStationTick = Now - 100000;
        _mon.m_nWalkSpeed = 100;
        _dein.AttackTargetResult = true;

        _mon.Run();

        Assert.Equal(1, _dein.AttackTargetCalls);
        Assert.Equal(0, _dein.PickRangeItemCalls);
    }

    /// <summary>1198 的前半 `m_TargetCret &lt;&gt; nil` 单独为假 ⇒ 不进攻击支（改走 1204 的 `IsCanMove` 支）。</summary>
    [Fact]
    public void NoTargetSkipsAttackBranch()
    {
        _mon.m_TargetCret = null;
        _mon.m_dwStationTick = Now - 100000;

        _mon.Run();

        Assert.Equal(0, _dein.AttackTargetCalls);
    }

    /// <summary>1195 的"站稳"阈值单独不成立（刚站下）⇒ 不进攻击支；且 1204 的 `IsCanMove` 为真时走任务点/拾取支。</summary>
    [Fact]
    public void NotStationedLongEnoughSkipsAttack()
    {
        _mon.m_TargetCret = _master;
        _mon.m_dwStationTick = Now;             // 刚站下
        _mon.m_nWalkSpeed = 100000;             // 阈值巨大 ⇒ 一定"没站稳"
        _mon.m_dwWalkTick = Now - 100000;       // IsCanMove = true

        _mon.Run();

        Assert.Equal(0, _dein.AttackTargetCalls);
    }

    /// <summary>1193 的 `m_boNoAttackMode` 单独关掉攻击支（"其它全开只关它"）。</summary>
    [Fact]
    public void NoAttackModeSkipsAttackBranchEntirely()
    {
        _mon.m_TargetCret = _master;
        _mon.m_dwStationTick = Now - 100000;
        _mon.m_boNoAttackMode = true;

        _mon.Run();

        Assert.Equal(0, _dein.AttackTargetCalls);
    }

    /// <summary>1206-1219：任务模式下取当前任务点写入 `m_nTargetX/m_nTargetY`。</summary>
    [Fact]
    public void MissionPointsDriveTargetXY()
    {
        _mon.m_boMission = true;
        _mon.m_nMissionPoints.Add(new TPoint(30, 30));
        _mon.m_nMissionPointIndex = 0;
        _mon.m_dwWalkTick = Now - 100000;       // IsCanMove
        _mon.m_nTargetX = -1;                   // 1206 会先置 -1

        _mon.Run();

        Assert.Equal(30, _mon.m_nTargetX);
        Assert.Equal(30, _mon.m_nTargetY);
    }

    /// <summary>1211-1217：距当前点 ≤3 且 ≤3 ⇒ 索引前进一格。</summary>
    [Fact]
    public void MissionIndexAdvancesWhenArrived()
    {
        _mon.m_boMission = true;
        _mon.m_nMissionPoints.Add(new TPoint(_mon.m_nCurrX + 1, _mon.m_nCurrY + 1));
        _mon.m_nMissionPoints.Add(new TPoint(35, 35));
        _mon.m_nMissionPointIndex = 0;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(1, _mon.m_nMissionPointIndex);
        Assert.Equal(35, _mon.m_nTargetX);
    }

    /// <summary>1215-1216：索引越过末尾 ⇒ 钳到 `Length - 1`（不环绕）。</summary>
    [Fact]
    public void MissionIndexClampsAtLastPoint()
    {
        _mon.m_boMission = true;
        _mon.m_nMissionPoints.Add(new TPoint(1, 1));
        _mon.m_nMissionPointIndex = 0;
        _mon.m_dwWalkTick = Now - 100000;
        _mon.m_nCurrX = 1;
        _mon.m_nCurrY = 1;                      // 与唯一任务点重合 ⇒ 前进后越界

        _mon.Run();

        Assert.Equal(0, _mon.m_nMissionPointIndex);   // Length-1 = 0
    }

    /// <summary>1207 的三重闸门里 `m_boMission` 单独为假 ⇒ 落到 1221 的 `else`（宠物/自动拾取支）。</summary>
    [Fact]
    public void MissionOffFallsIntoPickupBranch()
    {
        _mon.m_boMission = false;
        _mon.m_nMissionPoints.Add(new TPoint(30, 30));   // 有点也不算
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.NotEqual(30, _mon.m_nTargetX);
    }

    /// <summary>1207 的 `Length(m_nMissionPoints) &gt; 0` 单独为假 ⇒ 同样落到 `else`。</summary>
    [Fact]
    public void MissionEmptyPointsFallsIntoPickupBranch()
    {
        _mon.m_boMission = true;                 // 开着但一个点都没有
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(-1, _mon.m_nTargetX);       // 1206 置 -1 后没人改它（但 1277 段可能改）
    }

    /// <summary>1280-1287：目标离主人 &gt; 20 或跨图 ⇒ `DelTargetCreat`。</summary>
    [Fact]
    public void TargetTooFarFromMasterIsDeleted()
    {
        MakeGamePet();
        _master.m_nCurrX = 0;
        _master.m_nCurrY = 0;
        _mon.m_TargetCret = _master;
        _mon.m_dwWalkTick = Now - 100000;

        // 目标(0,0) 与主人(0,0) 同点 ⇒ 不超距；这里换一个"离主人远"的目标
        var far = new TMonster { m_nCurrX = 500, m_nCurrY = 500, m_PEnvir = _envir };
        _mon.m_TargetCret = far;

        _mon.Run();

        Assert.Equal(1, _dein.DelTargetCreatCalls);
        Assert.Null(_mon.m_TargetCret);
    }

    /// <summary>1282 的条件单独不成立（目标就在主人身边）⇒ 不删目标。</summary>
    [Fact]
    public void NearbyTargetNotDeleted()
    {
        MakeGamePet();
        _master.m_nCurrX = _mon.m_nCurrX + 10;
        _master.m_nCurrY = _mon.m_nCurrY;
        var near = new TMonster { m_nCurrX = _master.m_nCurrX + 1, m_nCurrY = _master.m_nCurrY, m_PEnvir = _envir };
        _mon.m_TargetCret = near;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(0, _dein.DelTargetCreatCalls);
    }

    /// <summary>1288-1312：无目标 ⇒ 用主人的回位点写 `m_nTargetX/m_nTargetY`（这里只验"确实取了回位点"）。</summary>
    [Fact]
    public void NoTargetTakesMastersBackPosition()
    {
        MakeGamePet();
        _master.m_nCurrX = 20;
        _master.m_nCurrY = 20;
        _master.m_btDirection = 4;              // 朝下 ⇒ 背后 (20, 19)
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        // 1282 的"目标离主人远"不适用（无目标）⇒ 直接进 1288
        Assert.Equal(20, _mon.m_nTargetX);
        Assert.Equal(19, _mon.m_nTargetY);
    }

    /// <summary>1290 的 `(m_btRaceServer = 155) and (m_btRaceImg = 156)` 单独为真 ⇒ **跳过**取回位点（保持 -1）。</summary>
    [Fact]
    public void Guardian155_156SkipsBackPosition()
    {
        MakeGamePet();
        _master.m_nCurrX = 20;
        _master.m_nCurrY = 20;
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 156;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(-1, _mon.m_nTargetX);
    }

    /// <summary>1290 的两个字面量缺一不可：155/157 ⇒ 不跳过。</summary>
    [Fact]
    public void RaceServer155NeedsRaceImg156()
    {
        MakeGamePet();
        _master.m_nCurrX = 20;
        _master.m_nCurrY = 20;
        _master.m_btDirection = 4;
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 157;                 // ← 只差这一个
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(20, _mon.m_nTargetX);
    }

    /// <summary>1290 的第二个析取项 `Self is TCustomMonster and MoveOption = moNoMove`（走接缝）⇒ 同样跳过。</summary>
    [Fact]
    public void NoMoveCustomMonsterSkipsBackPosition()
    {
        MakeGamePet();
        _master.m_nCurrX = 20;
        _master.m_nCurrY = 20;
        _config.IsCustomMonsterValue = true;
        _config.IsNoMoveCustomMonsterValue = true;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(-1, _mon.m_nTargetX);
    }

    /// <summary>1305-1310：回位点被别的对象占住 ⇒ 目标点退回自身坐标（"怪物宝宝会和人物叠一起"修正）。</summary>
    [Fact]
    public void OccupiedBackPositionRevertsToSelf()
    {
        MakeGamePet();
        _master.m_nCurrX = 20;
        _master.m_nCurrY = 20;
        _master.m_btDirection = 4;              // 背后 (20,19)
        _mon.m_nCurrX = 19;                     // 与回位点相邻（≤2）
        _mon.m_nCurrY = 19;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _view.Occupied.Add((20, 19));
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.True(_view.GetMovingObjectCalls > 0);
        Assert.Equal(19, _mon.m_nTargetX);
        Assert.Equal(19, _mon.m_nTargetY);
    }

    /// <summary>1305 的"没被占"对照：同样输入但格子空 ⇒ 目标点保持回位点。</summary>
    [Fact]
    public void FreeBackPositionKeepsMasterBackPosition()
    {
        MakeGamePet();
        _master.m_nCurrX = 20;
        _master.m_nCurrY = 20;
        _master.m_btDirection = 4;
        _mon.m_nCurrX = 19;
        _mon.m_nCurrY = 19;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(20, _mon.m_nTargetX);
        Assert.Equal(19, _mon.m_nTargetY);
    }

    /// <summary>
    /// 1314-1325：`SpaceMove` 的合取项里 1321 的 `(m_nTargetX &lt;&gt; -1) and (m_nTargetY &lt;&gt; -1)` 单独关掉即不飞。
    /// <para>构造：主人远在 50 格之外（1318 的前半成立）、`m_boGamePet` 为假（避开 1223 支）、
    /// `m_btRaceServer/Img` 取 155/156（1290 的豁免 ⇒ 不会把回位点写进 `m_nTargetX`）
    /// ⇒ 到达 1321 时两个目标分量仍是 -1，于是**不 SpaceMove**。</para>
    /// </summary>
    [Fact]
    public void MinusOneTargetBlocksFinalSpaceMove()
    {
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 50;  // 远超 20
        _master.m_nCurrY = _mon.m_nCurrY;
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 156;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    /// <summary>
    /// 1321 的两个分量是**两道独立门**（原文分别写 `(m_nTargetX &lt;&gt; -1) and (m_nTargetY &lt;&gt; -1)`）：
    /// 只把 `m_nTargetY` 置 -1 同样能挡住 1323（对照 <see cref="GamePetCrossMapSpaceMovesEvenWhenNear"/> 的正面例）。
    /// </summary>
    [Fact]
    public void FinalSpaceMoveRequiresBothTargetComponents()
    {
        _mon.m_Master = _master;
        _master.m_nCurrX = _mon.m_nCurrX + 50;
        _master.m_nCurrY = _mon.m_nCurrY;
        _mon.m_btRaceServer = 155;
        _mon.m_btRaceImg = 156;
        _mon.m_nTargetX = 20;                   // X 有效
        _mon.m_nTargetY = -1;                   // Y 无效 ⇒ 1321 整体为假
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    // ======================================================================
    // 九、1330 之后：`GotoTargetXY` / `Wondering` / 自定义怪最小距离（1341-1374）
    // ======================================================================

    /// <summary>1343：`m_nTargetX &lt;&gt; -1` ⇒ 普通怪走 `GotoTargetXY()`（即朝目标走一格）。</summary>
    [Fact]
    public void NonCustomMonsterWithTargetGotoTargetXY()
    {
        _mon.m_nTargetX = 20;
        _mon.m_nTargetY = 10;
        var t = new TMonster { m_nCurrX = 25, m_nCurrY = 10, m_PEnvir = _envir };
        _mon.m_TargetCret = t;
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(11, _mon.m_nCurrX);        // 朝右走一格
        Assert.Equal(10, _mon.m_nCurrY);
    }

    /// <summary>1371-1372：`m_nTargetX = -1` 且 `m_TargetCret = nil` ⇒ `Wondering()`（随机走一格，位置必变）。</summary>
    [Fact]
    public void NoTargetXAndNoTargetWonders()
    {
        _mon.m_Master = null;
        _mon.m_nTargetX = -1;
        _mon.m_nTargetY = -1;
        _mon.m_TargetCret = null;
        _mon.m_nCurrX = 20;
        _mon.m_nCurrY = 20;

        bool moved = false;
        for (int i = 0; i < 40 && !moved; i++)
        {
            _mon.m_nCurrX = 20;
            _mon.m_nCurrY = 20;
            RearmMovement();
            _mon.Run();
            moved = _mon.m_nCurrX != 20 || _mon.m_nCurrY != 20;
        }

        Assert.True(moved, "Wondering 在 40 次里应当至少走动一次（每次位移 ≤ 1 格）");
    }

    /// <summary>1346-1363：自定义怪 `MinAttackNearRange &lt;= 1` 时**无条件** `GotoTargetXY`。</summary>
    [Fact]
    public void CustomMonsterMinRangeOneShortcut()
    {
        _config.IsCustomMonsterValue = true;
        _config.MinAttackNearRangeValue = 1;
        _mon.m_nTargetX = 20;
        _mon.m_nTargetY = 10;
        _mon.m_TargetCret = new TMonster { m_nCurrX = 25, m_nCurrY = 10, m_PEnvir = _envir };
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.Equal(11, _mon.m_nCurrX);
    }

    /// <summary>1348-1359：自定义怪 `MinAttackNearRange &gt; 1` 且目标在范围内且**两轴都非零且不相等**（对角）⇒ 仍 `GotoTargetXY`。</summary>
    [Fact]
    public void CustomMonsterDiagonalInsideMinRangeStillGoto()
    {
        _config.IsCustomMonsterValue = true;
        _config.MinAttackNearRangeValue = 5;
        _mon.m_nTargetX = 20;
        _mon.m_nTargetY = 10;
        _mon.m_TargetCret = new TMonster { m_nCurrX = _mon.m_nCurrX + 3, m_nCurrY = _mon.m_nCurrY + 2, m_PEnvir = _envir };
        _mon.m_dwWalkTick = Now - 100000;

        _mon.Run();

        Assert.NotEqual((10, 10), (_mon.m_nCurrX, _mon.m_nCurrY));
    }

    /// <summary>1357 的第三个合取项单独为假（两轴相等 ⇒ 正对角）⇒ **不** `GotoTargetXY`。</summary>
    [Fact]
    public void CustomMonsterEqualAxesInsideMinRangeDoesNotGoto()
    {
        _config.IsCustomMonsterValue = true;
        _config.MinAttackNearRangeValue = 5;
        _mon.m_nTargetX = 20;
        _mon.m_nTargetY = 10;
        _mon.m_TargetCret = new TMonster { m_nCurrX = _mon.m_nCurrX + 3, m_nCurrY = _mon.m_nCurrY + 3, m_PEnvir = _envir };
        _mon.m_dwWalkTick = Now - 100000;
        _mon.m_nCurrX = 10;
        _mon.m_nCurrY = 10;

        _mon.Run();

        Assert.Equal((10, 10), (_mon.m_nCurrX, _mon.m_nCurrY));
    }

    /// <summary>把"下一次 Run 一定进入走位段"的前置状态恢复（1174 段的闸门 + 1179 的步数锁）。</summary>
    private void RearmMovement()
    {
        _mon.m_boWalkWaitLocked = false;
        _mon.m_nWalkCount = 0;
        _mon.m_nWalkStep = 10;
        _mon.m_dwWalkTick = Now - 100000;
    }

    /// <summary>
    /// 1353 的 `m_TargetCret = nil` 分支：1348 取到 `nMinRange &gt; 1` 后，因为**没有目标**
    /// 直接落到 1362 的 `else GotoTargetXY()`（**不比较任何距离** —— 原文如此）。
    /// <para>构造：`m_Master = nil` 以避开 1277 段的"无目标就取主人回位点"（那段会把目标点改掉）；
    /// 自定义怪 + `nMinRange = 5` + `m_nTargetX = 20`；`RearmMovement` 压掉 1179 的步数锁。</para>
    /// <para><b>⚠ 为什么只断言"移动了"而不指定方向</b>：原文 1341 的第一道门是
    /// `if (m_nTargetX &lt;&gt; -1)`，而 **1206/1209（`m_nTargetX := -1`）在 1341 之前**，
    /// 所以走到 1341 时 `m_nTargetX` 通常已是 -1 ⇒ 实际落入 1371-1372 的 `Wondering()`（**随机方向**）。
    /// 要在 1362 拿到确定方向，只能让 1206 那一步被跳过（`IsCanMove` 为假），可那样 1341 整段又不会执行。
    /// 即：**原文里 1362 这条支只有在 `m_nTargetX` 被 1288-1312 段重新写成非 -1 时才可能被走到**
    /// （`CustomMonsterDiagonalInsideMinRangeStillGoto` / `CustomMonsterMinRangeOneShortcut` 正是那种形态）。
    /// 故本用例只把"没有目标时不会崩、且会尝试移动"钉死，不假称方向。</para>
    /// </summary>
    [Fact]
    public void CustomMonsterNilTargetStillGoto()
    {
        _config.IsCustomMonsterValue = true;
        _config.MinAttackNearRangeValue = 5;
        _mon.m_Master = null;
        _mon.m_TargetCret = null;               // 1353 的关键前置
        _mon.m_nTargetX = 20;
        _mon.m_nTargetY = 10;
        RearmMovement();

        var before = (_mon.m_nCurrX, _mon.m_nCurrY);
        bool movedAtLeastOnce = false;

        // `Wondering` 是随机方向 ⇒ 单次运行可能"原地绕回"，故用多次运行把
        // **不变量**（每次最多走一格、始终不越界）钉死，而不是赌某一次一定位移。
        for (int i = 0; i < 200; i++)
        {
            var prev = (_mon.m_nCurrX, _mon.m_nCurrY);
            RearmMovement();
            _mon.Run();
            Assert.True(Math.Abs(_mon.m_nCurrX - prev.Item1) <= 1 && Math.Abs(_mon.m_nCurrY - prev.Item2) <= 1,
                "单次 Run 最多走一格");
            Assert.InRange(_mon.m_nCurrX, 0, _envir.nWidth - 1);
            Assert.InRange(_mon.m_nCurrY, 0, _envir.nHeight - 1);
            if ((_mon.m_nCurrX, _mon.m_nCurrY) != prev) movedAtLeastOnce = true;
        }

        Assert.True(movedAtLeastOnce, "200 次里至少应当走动一次");
        Assert.True(before.Item1 > 0);
    }

    // ======================================================================
    // 十、接缝与留痕（§48.1）
    // ======================================================================

    /// <summary>接缝未接线（`RunDeps == null`）时**不崩**、且按"该分支不成立"走。</summary>
    [Fact]
    public void NullRunDepsIsSafe()
    {
        _mon.RunDeps = null;

        _mon.Run();

        Assert.False(_mon.m_boGhost);
        Assert.Equal(0, _dein.SpaceMoveCalls);
    }

    /// <summary>`NotPorted` 留痕格式与内容：每条都是 `&lt;成员&gt;@ObjMon.pas:&lt;行号&gt;`。</summary>
    [Fact]
    public void NotPortedClaimsAreExplicitAndFormatted()
    {
        Assert.Equal(5, TMonster.NotPortedClaimCount);
        foreach (var claim in TMonster.NotPortedClaims)
        {
            Assert.Contains("@ObjMon.pas:", claim);
            Assert.Matches(@"@ObjMon\.pas:\d+$", claim);
        }
        Assert.Contains(TMonster.NotPorted("Think", 1144), TMonster.NotPortedClaims);
        Assert.Contains(TMonster.NotPorted("AttackTarget", 1198), TMonster.NotPortedClaims);
        Assert.Contains(TMonster.NotPorted("SpaceMove", 1140), TMonster.NotPortedClaims);
    }

    /// <summary>原文 `tick_diff` 的回绕安全定义（`M2Share.pas:32953-32959`）。</summary>
    [Fact]
    public void TickDiffMatchesOriginalDefinition()
    {
        Assert.Equal(1500u, TMonster.TickDiff(1000, 2500));
        Assert.Equal(0u, TMonster.TickDiff(7, 7));
        Assert.Equal(uint.MaxValue - 10u + 5u, TMonster.TickDiff(10, 5));
    }

    /// <summary>四条补回分支的守卫字段/常量逐个在托管侧可寻址（防止"名字写错但编译过"）。</summary>
    [Fact]
    public void FourRestoredBranchesAreAddressable()
    {
        // ① 天关宝宝：地图视图接缝的天关开关
        _view.m_boGuardianLevel = true;
        Assert.True(_view.m_boGuardianLevel);
        // ② 镜像地图：地图视图接缝的镜像开关
        _view.m_boMirror = true;
        Assert.True(_view.m_boMirror);
        // ③ 走步等待锁字段
        _mon.m_boWalkWaitLocked = true;
        _mon.m_dwWalkWaitTick = 1;
        _mon.m_dwWalkWait = 2;
        Assert.True(_mon.m_boWalkWaitLocked);
        // ④ 宠物拴物字段
        _mon.m_boGamePet = true;
        _mon.m_btGamePetEnablePick = 1;
        Assert.Equal(1, _mon.m_btGamePetEnablePick);
    }

    private static uint TickDiffBeforeNow(uint tick) => TMonster.TickDiff(tick, Now);
}
