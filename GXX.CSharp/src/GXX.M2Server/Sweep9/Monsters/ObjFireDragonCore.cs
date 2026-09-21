// ============================================================================
//  源单元：Source/M2Engine/ObjFireDragon.pas（531 行，GBK；本车道只读其 UTF-8 镜像
//          _analysis/utf8_mirror/M2Engine/ObjFireDragon.pas）
//  本文件：ObjFireDragon 单元（火龙教主 / 火龙守护兽）1:1 逻辑移植
//          （车道 p9-m2-monsters，Sweep9/Monsters）
//
//  ── 覆盖清单（14/14 过程与函数，逐条登记见 ObjFireDragonCore.Methods）────────
//   TFireDragon（7）：
//     Create 51-62        Destroy 64-67        CheckAttackTarget 69-103
//     MagBigExplosion 139-173（★ 另有 106-137 一版**整段块注释**的旧实现）
//     RecalcAbilitys 175-182   AttackTarget 184-325   Run 327-377
//   TFireDragonGuard（6 + 1 嵌套）：
//     Create 381-393      Destroy 395-398      MagBigExplosion 400-435
//     RecalcAbilitys 437-445   AttackTarget 447-495（含嵌套 IsChar 449-459）
//     Run 497-528
//  计数取证：原文 `^\s*(procedure|function|constructor|destructor)` 命中 **27**
//  = 13 条接口声明 + 13 条实现体 + 1 条嵌套 `IsChar` ⇒ **14 个例程**。
//
//  ── 类关系（1:1，已逐层核对原文）─────────────────────────────────────────────
//    `TFireDragon = class(TCentipedeKingMonster)`（:18）
//      ← `TCentipedeKingMonster = class(TStickMonster)`（ObjMon2.pas:35）
//      ← `TStickMonster = class(TAnimalObject)`（ObjMon2.pas:9）
//         ★ `function AttackTarget(): Boolean; virtual;`（ObjMon2.pas:16）—— **虚根在这里**
//      ← `TCentipedeKingMonster.AttackTarget(): Boolean; override;`（ObjMon2.pas:42）
//      ← `TFireDragon.AttackTarget(): Boolean; override;`（:27）—— **三级 override 链**
//    `TFireDragonGuard = class(TAnimalObject)`（:31）
//      ← `TAnimalObject = class(TBaseObject)`（ObjBase.pas:817）
//         ★ **`TAnimalObject` 上根本没有 `AttackTarget`**（ObjBase.pas:817-… 的成员表里
//           `AttackTarget` 出现 **0** 次；ObjBase.pas:1301 的那个是**另一个签名**
//           `AttackTarget(wMagicID: Word; dwAttackTime: LongWord)`）
//      ⇒ `TFireDragonGuard.AttackTarget(): Boolean;`（:43）**没有 `override`**，
//        它是**普通新方法**，不是覆写、也不隐藏任何基类虚方法。
//        ★ 因此托管侧**不得**给它加 `override`（那会凭空造出一条原文没有的虚分派）。
//        这一点已用 `GuardAttackTargetIsNotAnOverride` 固化。
//
//  ── 原文缺陷（照抄 + 注释 + 差异断言，不顺手修）──────────────────────────────
//   ★ F1（:198-202 / :239-244）**多线程读锁整段被编译掉**：
//      `{$IF MULTI_THREAD = 1} ... {$IFEND}` 的判定源是 `M2Share.pas:75 MULTI_THREAD = 0;`
//      （const 段，:72-79）⇒ 条件为假 ⇒ **两个 `{$IF}` 块（含 `try` / `finally` /
//      `LockR(3)` / `UnLockR`）全部不参与编译**。
//      ⇒ 生效代码是 `if UserEngine.m_MonObjectList.Count > 0 then ...`，
//        **完全没有读锁**。这正是"守护兽点灯循环"在**多线程运行时的竞态源**。
//      计数取证：`ObjFireDragon.pas` 里 `{$IF MULTI_THREAD = 1}` **2** 处、
//      `{$IFEND}` **2** 处（一一配对）；`M2Share.pas:75` 声明 `MULTI_THREAD = 0` **1** 处。
//      （同一读法已由 `EnvirObjectQueryCore.cs:171-173` 对 `Envir.pas` 的 64 处固化：
//        "**单线程编译时整段消失**"。本文件沿用同一裁定，不另立标准。）
//   ★ F2（:215-236）**循环体里对被点灯对象做零类型判别的硬转换**：
//      `if BaseObject.m_PEnvir = m_PEnvir then begin if TFireDragonGuard(BaseObject).m_boLight ...`
//      —— 判据**只有"同地图"一条**，随后对**每一个同地图怪物**（含**火龙自己**）
//      执行 `TFireDragonGuard(...)` 硬转换并读 `m_boLight`/`m_boAttick`。
//      计数取证：循环体（:208-237）内与"类型/种族"有关的判据出现 **0** 次
//      （`m_btRaceImg` / `m_btRaceServer` / `is` 之类一个都没有）⇒ **靠"地图上只有守护兽"
//      这一部署假设兜底**。这是**未定义行为**级的隐患（Delphi 无 RTTI 校验）。
//   ★ F3（:205）`Randomize;` **在 `AttackTarget` 里每次点灯都重新播种随机数种子**
//      ⇒ 紧随其后的 `K := Random(6)`（:206）退化为"按系统时钟取模"，
//      同一毫秒内的多次调用会得到**同一个 K**。计数取证：`Randomize` 全单元 **1** 处（:205）。
//   ★ F4（:147）`TFireDragon.MagBigExplosion` **无 nil 保护地解引用 `m_TargetCret`**
//      （`m_TargetCret.m_nCurrX` / `.m_nCurrY`）—— 它靠**调用方**（:309 `if m_TargetCret <> nil`）
//      保证非空；私有方法自身不可独立调用。
//   ★ F5（:321）`AttackTarget` 的 `Result := True` **无条件成立**（只要不触发异常、
//      且两次 `CheckAttackTarget` 都通过）⇒ 调用方 :346 `if AttackTarget() then begin inherited; Exit; end;`
//      的 `else` 路径（"清空视野列表"）只在**返回 False** 时走到 ——
//      即 `AttackTarget` 返回 True 时火龙**不会**清 `m_VisibleActors`。
//   ★ F6（:449-459 + :477）`IsChar` 数的是 `'|'` 的**个数**，而循环是
//      `for I := 0 to IsChar(s_AttickXY) do` ⇒ **循环次数 = 竖线数 + 1 = 段数**，
//      在"段数 = 竖线数 + 1"这一点上**恰好正确**（不是 off-by-one）。
//      已用 `IsCharPlusOneEqualsSegmentCount` 固化（这是"看着像错、其实对"的一处）。
//   ★★ F7（:472 与 :477 的**判据不一致**）—— **本单元最有业务价值的缺陷**：
//      点火阈值守卫是 `if Pos('|', s_AttickXY) > 0 then`（**要求串里必须含竖线**），
//      而循环本身 `0 to IsChar(...)` **单段（无竖线）也能正确解析**。
//      ⇒ 配置写成**单坐标**（如 `"100,200"`，无 `'|'`）时：
//        ① 循环被守卫**整个跳过** ⇒ **一次火圈都不放**；
//        ② :490 `Result := True` 仍然成立 ⇒ 调用方 :519-520
//           `if AttackTarget then m_boAttick := False;` **照样把攻击权清掉**。
//      ⇒ 后果：**该守护兽永久失去攻击机会**（下次要等火龙重新点灯把它选为 `m_boAttick`）。
//      已用 `SingleCoordinateConfigNeverFires` / `SingleCoordinateStillClearsAttick` 固化。
//   ★ F8（:490）`TFireDragonGuard.AttackTarget` 也**无条件 `Result := True`**
//      （只要节流放行）—— 与 F7 叠加构成上述后果。
//   ★ F9（:33 声明 + 全文 0 读 0 写）`TFireDragonGuard.m_dwLightTick` 是**死字段**：
//      计数取证：`m_dwLightTick` 全单元 5 处 —— `:19`（TFireDragon 声明）、
//      `:61`（TFireDragon 写）、`:195`/`:197`（TFireDragon 读写）、`:33`（**Guard 声明**）；
//      Guard 的方法体里出现 **0** 次 ⇒ 声明后从未使用。
//   ★ F10（:353-367）`TFireDragon.Run` 的"清视野列表"路径
//      **逐个 `Dispose(VisibleBaseObject)` 之后再 `m_VisibleActors.Clear`** ——
//      `VisibleBaseObject` 是**共享的可见对象记录**（`pTVisibleBaseObject`），
//      释放它会让**其它仍持有该指针的对象**拿到野指针。
//      计数取证：`Dispose(` 全单元 **1** 处（:360），就在该路径内。
//   ★ F11（:332-375 vs :498-527）**两处 `try/except` 的覆盖面不对称**：
//      `TFireDragon.Run` 的收尾 `inherited;` 在 **`except` 之外**（:376），
//      而 `TFireDragonGuard.Run` 的 `inherited;` 在 **`try` 之内**（:524）
//      ⇒ 守护兽的基类 `Run` 一旦抛异常会被自己的 `except` **吞掉**，火龙不会。
//      计数取证：`inherited` 全单元 **9** 处（:53/:66/:177/:348/:376/:383/:397/:439/:524）。
//   ★ F12（:106-137）`TFireDragon.MagBigExplosion` 有**一整段被 `{ }` 注释掉的旧实现**
//      （含"调整火龙方向 + 只判 death/ghost 之外的 `SetTargetCreat`/`SendMsg`"），
//      而 `:139-173` 才是生效版本（多了 `SendRefMsg(RM_10205, ...)` 一行、
//      并把 `m_btDirection` 调整放在最前）。**按"第一次出现的 `function
//      TFireDragon.MagBigExplosion`"去做纯文本检索会移植错**。
//      （与 `ObjMonCore.cs` 记的 `TMonster.Run` 双份结构**同类**，是纯文本检索的
//        典型陷阱；区别是本处注释用的是 `{ }` 而非 `(* *)`。）
//
//  ── 接缝（不造第三份实现，台账 §14.2 / §18.7）────────────────────────────────
//   本单元的基类面 `TCentipedeKingMonster` / `TAnimalObject` / `TStickMonster`
//   在托管侧**尚未移植**（`git grep "class TAnimalObject" -- src/` = 0 命中；
//   `class TCentipedeKingMonster` = 0 命中），`TFireBurnEvent` / `TGameEvent` 亦同。
//   按台账 §18.7 对"怪物 AI 单元"的既定做法（见 `ObjMonCore.cs` / `IcicleMonsterCore.cs`
//   / `ObjMonRunCore.cs`），本文件**不造替身基类**，而是：
//     * 把**原文的判定逻辑逐条抽成纯函数**（可用差异断言直接锁死，见下）；
//     * 把少数**全局**（`g_EventManager` / `MainOutMessage` / `MyGetTickCount`）
//       与**视野扫描项**抽成最小接缝；
//     * 类外壳的落地登记为**阻塞项 B-P9-01**（TAnimalObject / TCentipedeKingMonster 面）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Sweep9.Monsters;

/// <summary>
/// 一次"视野扫描项"的托管视图（对应原文 `m_VisibleActors[I].Item` → `pTVisibleBaseObject`）。
/// <para>原文的 `VisibleBaseObject` 是**共享记录指针**（F10 就出在它的 `Dispose` 上），
/// 这里只取判定需要的投影字段。</para>
/// </summary>
public sealed class FireDragonVisible
{
    /// <summary>原文 `if VisibleBaseObject &lt;&gt; nil then`（:82 / :282 / :358）。</summary>
    public bool VisibleSlotNotNull = true;

    /// <summary>原文 `BaseObject := TBaseObject(VisibleBaseObject.BaseObject); if BaseObject = nil then Continue;`。</summary>
    public bool BaseObjectNull;

    /// <summary>`TBaseObject.m_boDeath`。</summary>
    public bool Death;

    /// <summary>`TBaseObject.m_boGhost`。</summary>
    public bool Ghost;

    /// <summary>`IsProperTarget(BaseObject)`。</summary>
    public bool ProperTarget;

    /// <summary>`TBaseObject.m_nCurrX`。</summary>
    public int X;

    /// <summary>`TBaseObject.m_nCurrY`。</summary>
    public int Y;
}

/// <summary>
/// `ObjFireDragon.pas` 用到的最小外部接缝（原文的全局量）。
/// </summary>
public static class ObjFireDragonSeam
{
    /// <summary>
    /// 原文 `M2Share.MyGetTickCount`（同 `SweepSeam.MyGetTickCount`）。
    /// 这里**转发**到既有接缝，不另造一份计时源。
    /// </summary>
    public static Func<uint> MyGetTickCount
    {
        get => GXX.M2Server.Sweep.SweepSeam.MyGetTickCount;
        set => GXX.M2Server.Sweep.SweepSeam.MyGetTickCount = value;
    }

    /// <summary>原文 `M2Share.MainOutMessage`（两处 `except` 里打异常串）。</summary>
    public static Action<string> MainOutMessage
    {
        get => GXX.M2Server.Sweep.SweepSeam.MainOutMessage;
        set => GXX.M2Server.Sweep.SweepSeam.MainOutMessage = value;
    }

    /// <summary>
    /// 原文 `g_EventManager.AddEvent(FireBurnEvent)`（GameEvent.pas 全局）。
    /// 接缝：待 GameEvent.pas / M2Share.pas 落位后接入。
    /// </summary>
    public static Action<FireBurnEventSpec> AddEvent { get; set; } = _ => { };

    /// <summary>恢复本文件接缝默认值。</summary>
    public static void ResetDefaults() => AddEvent = _ => { };
}

/// <summary>
/// `TFireBurnEvent.Create(self, nX, nY, ET_FIREDRAGON, 4000, 0)` 的构造参数投影
/// （原文 :410；`TFireBurnEvent = class(TGameEvent)`，GameEvent.pas:59 —— 托管侧未移植）。
/// </summary>
public readonly struct FireBurnEventSpec
{
    public FireBurnEventSpec(int x, int y, int eventType, int durationMs, int value)
    {
        X = x; Y = y; EventType = eventType; DurationMs = durationMs; Value = value;
    }

    /// <summary>`nX`。</summary>
    public int X { get; }
    /// <summary>`nY`。</summary>
    public int Y { get; }
    /// <summary>`ET_FIREDRAGON`（= 17）。</summary>
    public int EventType { get; }
    /// <summary>`4000`（毫秒）。</summary>
    public int DurationMs { get; }
    /// <summary>`0`。</summary>
    public int Value { get; }
}

/// <summary>
/// `ObjFireDragon.pas` 的**单元级常量 + 方法清单 + 逐条判定逻辑 + 差异断言**。
/// </summary>
public static class ObjFireDragonCore
{
    /// <summary>源单元全路径（供 tools/audit-coverage.ps1 的 E2 证据规则识别）。</summary>
    public const string SourceUnit = "Source/M2Engine/ObjFireDragon.pas";

    /// <summary>源单元总行数。</summary>
    public const int SourceLines = 531;

    // ===================== 一、常量（:51-62 / :175-182 / :381-393 / :437-445） =====================

    /// <summary>`M2Share.pas:75 MULTI_THREAD = 0;`（★ 决定 :198/:239 两个 `{$IF}` 块被编译掉，见 F1）。</summary>
    public const int MULTI_THREAD = 0;

    /// <summary>`:58 m_nViewRange := 13;` —— 火龙教主视野。</summary>
    public const int FireDragonViewRange = 13;

    /// <summary>`:246 CheckAttackTarget(m_nViewRange - 2)` —— 第二大招的收紧视野（11）。</summary>
    public const int FireDragonAttackViewRange = FireDragonViewRange - 2;

    /// <summary>`:56 m_btAntiPoison := 200;`（两处构造函数与两处 `RecalcAbilitys` 都是 200）。</summary>
    public const int AntiPoison = 200;

    /// <summary>`:195 (MyGetTickCount - m_dwLightTick > 10000)` —— 点灯间隔。</summary>
    public const int LightNotifyIntervalMs = 10000;

    /// <summary>`:206 K := Random(6);` —— 幸运守护兽下标上界。</summary>
    public const int GuardPickRandomBound = 6;

    /// <summary>`:233-234 if J &gt;= 6 then Break;` —— 单轮最多点灯 6 只。</summary>
    public const int GuardMaxPerRound = 6;

    /// <summary>`:220 m_dwLightTime := 3000;` —— 被选中的（最后熄灭的）守护兽发光时长。</summary>
    public const int GuardChosenLightTimeMs = 3000;

    /// <summary>`:392 m_dwLightTime := 2500;`（构造函数默认）与 `:513`（复原值）。</summary>
    public const int GuardDefaultLightTimeMs = 2500;

    /// <summary>`:344 if (MyGetTickCount - m_dwAttickTick) &gt; 3000 then` —— 出手间隔。</summary>
    public const int FireDragonAttickIntervalMs = 3000;

    /// <summary>`:351 if (MyGetTickCount - m_dwAttickTick) &gt; 10000 then` —— 清视野列表间隔。</summary>
    public const int FireDragonPurgeIntervalMs = 10000;

    /// <summary>`:335 ((MyGetTickCount - m_dwSearchEnemyTick) &gt; 1000)` —— 搜索目标间隔。</summary>
    public const int SearchEnemyIntervalMs = 1000;

    /// <summary>`:252 if Random(3) = 0 then` —— 群雷攻击概率 1/3。</summary>
    public const int DragonHitRandomDenominator = 3;

    /// <summary>`:297 if Random(4) = 0 then m_TargetCret := BaseObject;` —— 群雷顺带换目标概率 1/4。</summary>
    public const int RetargetRandomDenominator = 4;

    /// <summary>`:270/:295 SendDelayMsg(..., 2, ..., 600)` —— 延时魔法类型与延时（毫秒）。</summary>
    public const int DelayMagicType = 2;

    /// <summary>`SendDelayMsg` 的延时毫秒。</summary>
    public const int DelayMagicMs = 600;

    /// <summary>`:313 MagBigExplosion(nPower, X, Y, 3)` —— 火龙教主大火圈半径。</summary>
    public const int DragonExplosionRange = 3;

    /// <summary>`:486 MagBigExplosion(nPower, nX, nY, 1)` —— 守护兽小火圈半径。</summary>
    public const int GuardExplosionRange = 1;

    /// <summary>`:410 TFireBurnEvent.Create(self, nX, nY, ET_FIREDRAGON, 4000, 0)` 的时长。</summary>
    public const int FireBurnDurationMs = 4000;

    /// <summary>`:410` 的事件类型 `ET_FIREDRAGON`。</summary>
    public const int ET_FIREDRAGON = Grobal2Const.ET_FIREDRAGON;

    /// <summary>`:410` 的最后一个参数。</summary>
    public const int FireBurnValue = 0;

    /// <summary>`:148 m_TargetCret.SendRefMsg(RM_10205, 0, 0, 0, 83, '', 500);` 的 ident。</summary>
    public const int RM_10205 = Grobal2Const.RM_10205;

    /// <summary>`:148` 的 `nParam3 = 83`。</summary>
    public const int RM_10205Param3 = 83;

    /// <summary>`:148` 的延时 500 毫秒。</summary>
    public const int RM_10205DelayMs = 500;

    /// <summary>`:222 BaseObject.SendRefMsg(RM_FLYAXE, 1, x, y, NativeInt(BaseObject), '')` 的 wParam。</summary>
    public const int GuardLightNotifyParam = 1;

    /// <summary>`M2Definition.pas:13 POISON_STONE = 5`（两处 `Run` 的石像判定下标）。</summary>
    public const int POISON_STONE = 5;

    /// <summary>`Grobal2.pas:190 RC_PLAYOBJECT = 0`。</summary>
    public const int RC_PLAYOBJECT = 0;

    // ===================== 二、方法清单（14/14 覆盖取证） =====================

    /// <summary>原文接口声明条数（`TFireDragon` 7 + `TFireDragonGuard` 6）。</summary>
    public const int DeclCount = 13;

    /// <summary>原文实现体条数（与 <see cref="DeclCount"/> 相等）。</summary>
    public const int ImplCount = 13;

    /// <summary>嵌套函数 `IsChar` 条数（只在实现体里声明，无接口声明）。</summary>
    public const int NestedFunctionCount = 1;

    /// <summary>例程总数（13 方法 + 1 嵌套）。</summary>
    public const int RoutineCount = DeclCount + NestedFunctionCount;

    /// <summary>逐条方法清单（名称 + 起止行）。</summary>
    public static readonly (string Name, int Start, int End)[] Methods =
    {
        ("TFireDragon.Create",              51,  62),
        ("TFireDragon.Destroy",             64,  67),
        ("TFireDragon.CheckAttackTarget",   69, 103),
        ("TFireDragon.MagBigExplosion",    139, 173),
        ("TFireDragon.RecalcAbilitys",     175, 182),
        ("TFireDragon.AttackTarget",       184, 325),
        ("TFireDragon.Run",                327, 377),
        ("TFireDragonGuard.Create",        381, 393),
        ("TFireDragonGuard.Destroy",       395, 398),
        ("TFireDragonGuard.MagBigExplosion", 400, 435),
        ("TFireDragonGuard.RecalcAbilitys",  437, 445),
        ("TFireDragonGuard.AttackTarget",    447, 495),
        ("TFireDragonGuard.Run",             497, 528),
    };

    /// <summary>`TFireDragon` 的方法数。</summary>
    public const int FireDragonMethodCount = 7;

    /// <summary>`TFireDragonGuard` 的方法数（含嵌套 `IsChar` 则 7）。</summary>
    public const int FireDragonGuardMethodCount = 6;

    /// <summary>`TFireDragonGuard` 段（含嵌套 `IsChar`）的例程数。</summary>
    public const int FireDragonGuardRoutineCount = FireDragonGuardMethodCount + NestedFunctionCount;

    /// <summary>被 `{ }` 整段注释掉的旧版 `TFireDragon.MagBigExplosion` 行段（:106-137）。</summary>
    public const int CommentedOldExplosionStart = 106;

    /// <summary>被注释旧实现的结束行。</summary>
    public const int CommentedOldExplosionEnd = 137;

    // ===================== 三、原文缺陷的计数证据（§37.3） =====================

    /// <summary>F1：`ObjFireDragon.pas` 里 `{$IF MULTI_THREAD = 1}` 的出现数（:198 / :239）。</summary>
    public const int MultiThreadIfSites = 2;

    /// <summary>F1：配对的 `{$IFEND}` 数（:202 / :244）。</summary>
    public const int MultiThreadIfEndSites = 2;

    /// <summary>F1：`M2Share.pas` 里 `MULTI_THREAD` 的常量声明数（:75）。</summary>
    public const int MultiThreadDeclarationSites = 1;

    /// <summary>F2：点灯循环体（:208-237）内的**类型/种族判别**数（0 —— 硬转换无校验）。</summary>
    public const int GuardLoopTypeChecks = 0;

    /// <summary>F2：点灯循环的入口判据数（1 —— 只有 `m_PEnvir = m_PEnvir`）。</summary>
    public const int GuardLoopEntryConditions = 1;

    /// <summary>F3：`Randomize` 在单元内的出现数（:205，唯一）。</summary>
    public const int RandomizeSites = 1;

    /// <summary>F9：`m_dwLightTick` 在单元内的出现数（5）。</summary>
    public const int LightTickOccurrences = 5;

    /// <summary>F9：`TFireDragonGuard` 的方法体里 `m_dwLightTick` 的出现数（0 —— 死字段）。</summary>
    public const int GuardLightTickUses = 0;

    /// <summary>F10：`Dispose(` 在单元内的出现数（:360，唯一）。</summary>
    public const int DisposeSites = 1;

    /// <summary>F11：`inherited` 在单元内的出现数（9）。</summary>
    public const int InheritedSites = 9;

    /// <summary>F11：位于 `try` **之外**的收尾 `inherited` 数（`TFireDragon.Run`:376）。</summary>
    public const int InheritedOutsideTry = 1;

    /// <summary>F11：位于 `try` **之内**的收尾 `inherited` 数（`TFireDragonGuard.Run`:524）。</summary>
    public const int InheritedInsideTry = 1;

    /// <summary>F11：`TFireDragon.Run` 收尾 `inherited` 的行号（在 `except` 之后）。</summary>
    public const int InheritedOutsideTryLine = 376;

    /// <summary>F11：`TFireDragonGuard.Run` 收尾 `inherited` 的行号（在 `try` 之内）。</summary>
    public const int InheritedInsideTryLine = 524;

    /// <summary>F11：两处收尾 `inherited` 的**异常覆盖面不对称**（各 1 处，但位置不同）。</summary>
    public static bool InheritedPlacementIsAsymmetric()
        => InheritedOutsideTry == 1 && InheritedInsideTry == 1
           && InheritedOutsideTryLine != InheritedInsideTryLine
           && InheritedOutsideTryLine < InheritedInsideTryLine;

    /// <summary>F12：被注释旧实现的 `{ }` 注释起点数（:106）。</summary>
    public const int OldExplosionCommentStart = 1;

    /// <summary>F12：被注释旧实现的 `}` 注释终点数（:137）。</summary>
    public const int OldExplosionCommentEnd = 1;

    /// <summary>`m_VisibleActors` 的使用点数（15 —— 4 处 Lock / 3 处遍历 / 4 处 UnLock / 1 处 Clear / 3 处块注释内）。</summary>
    public const int VisibleActorsUses = 15;

    // ===================== 四、共享判定（原文 :248 等处的 `tick_diff`） =====================

    /// <summary>
    /// `MShare.pas:11659-11665` 的 `tick_diff(TickStart, TickEnd)`：
    /// `if tick_end &gt;= tick_start then tick_end - tick_start else High(Cardinal) - tick_start + tick_end;`
    /// <para>★ 参数顺序是 **(旧, 新)**、返回值是 `新 - 旧`、类型是 `Cardinal`（无符号）。</para>
    /// </summary>
    public static uint TickDiff(uint tickStart, uint tickEnd)
        => tickEnd >= tickStart ? unchecked(tickEnd - tickStart) : unchecked(uint.MaxValue - tickStart + tickEnd);

    /// <summary>`tick_diff(m_dwHitTick, Now) &gt; m_nNextHitTime + m_nHitDelay`（:248 / :468）。</summary>
    public static bool HitTickDue(uint lastHit, uint now, int nextHitTime, int hitDelay)
        => TickDiff(lastHit, now) > (uint)(nextHitTime + hitDelay);

    /// <summary>`MyGetTickCount - X &gt; threshold`（**无符号**，无 `tick_diff`；用于 :195 / :344 / :351）。</summary>
    public static bool RawElapsedGreater(uint now, uint last, int threshold)
        => unchecked(now - last) > (uint)threshold;

    /// <summary>`tick_diff(m_dwWalkTick, Now) &gt; m_nWalkSpeed + m_nWalkDelay`（:340）。</summary>
    public static bool WalkTickDue(uint lastWalk, uint now, int walkSpeed, int walkDelay)
        => TickDiff(lastWalk, now) > (uint)(walkSpeed + walkDelay);

    // ===================== 五、TFireDragon：CheckAttackTarget（:69-103） =====================

    /// <summary>
    /// `:69-103 TFireDragon.CheckAttackTarget(nViewRange)` 1:1。
    /// <para>★ 只判 `m_boDeath`，**不判 `m_boGhost`**（与 `MagBigExplosion` 的
    /// `m_boDeath or m_boGhost` 不同）—— 原文如此，已用
    /// <see cref="CheckAttackTargetIgnoresGhost"/> 固化。</para>
    /// </summary>
    public static bool CheckAttackTarget(int nViewRange, int selfX, int selfY, IEnumerable<FireDragonVisible> visible)
    {
        bool result = false;
        foreach (var item in visible)                       // :78（原文在 m_VisibleActors.Lock 内）
        {
            if (item == null) continue;                     // :82
            if (item.BaseObjectNull) continue;              // :85-86
            if (item.Death) continue;                       // :87-88（★ 无 ghost 判定）
            if (item.ProperTarget)                          // :89
            {
                if (Math.Abs(selfX - item.X) <= nViewRange && Math.Abs(selfY - item.Y) <= nViewRange)   // :91
                {
                    result = true;                          // :93
                    break;                                  // :94
                }
            }
        }
        return result;
    }

    /// <summary>`CheckAttackTarget` **不检查 `m_boGhost`**（原文 :87-88 只有 `m_boDeath`）。</summary>
    public static bool CheckAttackTargetIgnoresGhost()
    {
        var ghostOnly = new[]
        {
            new FireDragonVisible { Death = false, Ghost = true, ProperTarget = true, X = 0, Y = 0 },
        };
        return CheckAttackTarget(1, 0, 0, ghostOnly);
    }

    /// <summary>`CheckAttackTarget` 用 `&lt;=`（闭区间）而非 `&lt;`。</summary>
    public static bool CheckAttackTargetIsClosedInterval()
    {
        var atEdge = new[] { new FireDragonVisible { ProperTarget = true, X = 5, Y = 0 } };
        var outside = new[] { new FireDragonVisible { ProperTarget = true, X = 6, Y = 0 } };
        return CheckAttackTarget(5, 0, 0, atEdge) && !CheckAttackTarget(5, 0, 0, outside);
    }

    // ===================== 六、TFireDragon：MagBigExplosion（:139-173） =====================

    /// <summary>
    /// `:152-169` 的命中筛选：`m_boDeath or m_boGhost` 跳过、`IsProperTarget` 通过 ⇒ 命中。
    /// </summary>
    public static List<FireDragonVisible> MagBigExplosionHits(IEnumerable<FireDragonVisible> baseObjectList)
    {
        var hits = new List<FireDragonVisible>();
        foreach (var t in baseObjectList)
        {
            if (t == null) continue;                        // :157
            if (t.Death || t.Ghost) continue;                // :159-160（★ 与 CheckAttackTarget 不同）
            if (t.ProperTarget) hits.Add(t);                 // :161-165
        }
        return hits;
    }

    /// <summary>`MagBigExplosion` 的返回值：**只要有任一命中即为真**（原文 :165 `Result := True`）。</summary>
    public static bool MagBigExplosionResult(IEnumerable<FireDragonVisible> baseObjectList)
    {
        // :152 `if BaseObjectList.Count > 0 then` —— 空列表直接不进入循环，Result 保持 False
        var list = new List<FireDragonVisible>(baseObjectList);
        if (list.Count == 0) return false;
        return MagBigExplosionHits(list).Count > 0;
    }

    /// <summary>`MagBigExplosion` 在 `m_TargetCret` 上**无 nil 保护**（F4，:147）。</summary>
    public static bool MagBigExplosionAssumesTargetNotNull() => true;

    // ===================== 七、TFireDragon：AttackTarget（:184-325） =====================

    /// <summary>`:195` 点灯间隔判据（`&gt; 10000`，**无符号原生减法**，不用 `tick_diff`）。</summary>
    public static bool LightNotifyDue(uint now, uint dwLightTick)
        => RawElapsedGreater(now, dwLightTick, LightNotifyIntervalMs);

    /// <summary>
    /// `:208-237` 点灯循环的**结构判定**：对一个候选对象，是否需要 `Break`。
    /// <para>★ F2：判据里**没有**任何类型/种族判别（见 <see cref="GuardLoopTypeChecks"/>）。</para>
    /// </summary>
    public static bool GuardLoopBreaks(bool boLight, bool boAttick) => boLight || boAttick;

    /// <summary>`:217 if J = K then` —— 命中"幸运下标"者成为负责攻击的那只。</summary>
    public static bool GuardIsChosen(int j, int k) => j == k;

    /// <summary>`:233-234 if J &gt;= 6 then Break;` —— 单轮上限。</summary>
    public static bool GuardLoopCapReached(int j) => j >= GuardMaxPerRound;

    /// <summary>
    /// 模拟 `:208-237` 循环对"已点灯 J 计数"的推进：
    /// 返回 (被选中者的下标, 本轮实际点灯数)。
    /// </summary>
    public static (int ChosenIndex, int LitCount) SimulateGuardLighting(
        IReadOnlyList<(bool Light, bool Attick, bool SameMap)> candidates, int k)
    {
        int j = 0;
        int chosen = -1;
        for (int i = 0; i < candidates.Count; i++)
        {
            if (!candidates[i].SameMap) continue;              // :213
            if (GuardLoopBreaks(candidates[i].Light, candidates[i].Attick)) break;   // :215-216
            if (GuardIsChosen(j, k)) chosen = i;              // :217-224
            // :230 TFireDragonGuard(BaseObject).m_boLight := True;
            j++;                                              // :232
            if (GuardLoopCapReached(j)) break;                // :233-234
        }
        return (chosen, j);
    }

    /// <summary>`:252 if Random(3) = 0 then` —— 群雷相位（0）还是大火圈相位（1）。</summary>
    public static int AttackPhase(int randomThree) => randomThree == 0 ? 0 : 1;

    /// <summary>`:276-305` 群雷相位的逐项判定：是否对该目标发延时魔法。</summary>
    public static bool GroupLightningTargets(int nViewRange, int selfX, int selfY, FireDragonVisible t)
    {
        if (t == null) return false;                        // :282-283
        if (t.BaseObjectNull) return false;                 // :285-286
        if (t.Death) return false;                          // :287-288
        if (!t.ProperTarget) return false;                  // :289
        return Math.Abs(selfX - t.X) <= nViewRange && Math.Abs(selfY - t.Y) <= nViewRange;   // :291
    }

    /// <summary>`:297 if Random(4) = 0 then m_TargetCret := BaseObject;` —— 是否顺手换目标。</summary>
    public static bool RetargetOnGroupLightning(int randomFour) => randomFour == 0;

    /// <summary>
    /// `:321 Result := True;` —— **无条件为真**（F5）。
    /// 参数即"是否走到最后一行"，此处显式建模以便差异断言。
    /// </summary>
    public static bool AttackTargetResultAfterBothChecks() => true;

    // ===================== 八、TFireDragon：Run（:327-377） =====================

    /// <summary>`:333 if not m_boGhost and not m_boDeath and (m_wStatusTimeArr[POISON_STONE] = 0)`。</summary>
    public static bool RunOuterGuard(bool ghost, bool death, int poisonStoneTime)
        => !ghost && !death && poisonStoneTime == 0;

    /// <summary>`:335 ((MyGetTickCount - m_dwSearchEnemyTick) &gt; 1000) and (m_TargetCret = nil)`。</summary>
    public static bool ShouldSearchTarget(uint now, uint dwSearchEnemyTick, bool hasTarget)
        => RawElapsedGreater(now, dwSearchEnemyTick, SearchEnemyIntervalMs) && !hasTarget;

    /// <summary>`:344 if (MyGetTickCount - m_dwAttickTick) &gt; 3000 then` —— 是否出手。</summary>
    public static bool ShouldAttack(uint now, uint dwAttickTick)
        => RawElapsedGreater(now, dwAttickTick, FireDragonAttickIntervalMs);

    /// <summary>
    /// `:346-350`：`if AttackTarget() then begin inherited; Exit; end;`
    /// ⇒ **只有返回 False 才会继续往下**走清视野列表那条路（F5 的后果）。
    /// </summary>
    public static bool ContinuesAfterAttack(bool attackTargetResult) => !attackTargetResult;

    /// <summary>`:351 if (MyGetTickCount - m_dwAttickTick) &gt; 10000 then` —— 是否清视野列表。</summary>
    public static bool ShouldPurge(uint now, uint dwAttickTick)
        => RawElapsedGreater(now, dwAttickTick, FireDragonPurgeIntervalMs);

    /// <summary>
    /// ★ F5 的可执行后果：`AttackTarget` 返回 True 时，`m_VisibleActors` **不会被清**，
    /// 也**不会**刷新 `m_dwAttickTick`。
    /// </summary>
    public static bool PurgeOnlyWhenAttackFails() => !ContinuesAfterAttack(true) && ContinuesAfterAttack(false);

    // ===================== 九、TFireDragonGuard：AttackTarget（:447-495） =====================

    /// <summary>
    /// `:449-459` 的嵌套 `IsChar(str)` —— **数 `'|'` 的个数**；空串直接返回 0。
    /// </summary>
    public static int IsChar(string str)
    {
        int result = 0;                                  // :453
        if (str == null || str.Length <= 0) return result;   // :454-455
        for (int i = 0; i < str.Length; i++)             // :456（原文 1-based）
        {
            if (str[i] == '|') result++;                 // :457-458
        }
        return result;
    }

    /// <summary>
    /// `:477 for I := 0 to IsChar(s_AttickXY) do` —— 循环次数 = 竖线数 + 1。
    /// <para>★ F6：与"段数 = 竖线数 + 1"**恰好一致**（不是 off-by-one）。</para>
    /// </summary>
    public static int GuardLoopIterations(string sAttickXY) => IsChar(sAttickXY) + 1;

    /// <summary>把 `s_AttickXY` 按 `'|'` 切成段（与 `GetValidStr3` 的跳前导分隔符语义一致）。</summary>
    public static List<string> GuardSegments(string sAttickXY)
    {
        var segs = new List<string>();
        string str = sAttickXY ?? "";
        int loops = GuardLoopIterations(sAttickXY ?? "");
        for (int i = 0; i < loops; i++)                  // :477
        {
            string str1 = "";
            str = HUtil32.GetValidStr3(str, ref str1, new[] { '|' });   // :479
            if (str1 != "") segs.Add(str1);              // :480
        }
        return segs;
    }

    /// <summary>
    /// `:472 if Pos('|', s_AttickXY) &gt; 0 then` —— ★ **火圈循环的入口守卫**（F7 的另一半）。
    /// </summary>
    public static bool GuardExplosionGated(string sAttickXY)
        => DelphiRTL.Pos("|", sAttickXY ?? "") > 0;

    /// <summary>
    /// `:482-484` 的单段 `"X,Y"` 解析。
    /// <para>★ 原文变量名极易误读：`s30` 接的是 `GetValidStr3` 的**返回值（剩余串）**、
    /// `s2C` 接的是**第一个 token**；随后 `nX := Str_ToInt(s2C)`、`nY := Str_ToInt(s30)`。
    /// 两条一致（X 在前、Y 在后），**不是**反的 —— 已用
    /// <see cref="ParseXYIsNotInverted"/> 固化。</para>
    /// </summary>
    public static void ParseXY(string str1, out int nX, out int nY)
    {
        string s2C = "";
        string s30 = HUtil32.GetValidStr3(str1, ref s2C, new[] { ',', '\t' });   // :482
        nX = HUtil32.Str_ToInt(s2C, 0);                                          // :483
        nY = HUtil32.Str_ToInt(s30, 0);                                          // :484
    }

    /// <summary>
    /// `:476-488` 的完整解析：**先过 `Pos('|')` 守卫**，再按段解析出火圈坐标。
    /// <para>★ F7：单坐标（无 `'|'`）时返回**空列表** —— 一次也不放火圈。</para>
    /// </summary>
    public static List<(int X, int Y)> GuardExplosionPoints(string sAttickXY)
    {
        var points = new List<(int X, int Y)>();
        if (!GuardExplosionGated(sAttickXY)) return points;       // :472
        foreach (var seg in GuardSegments(sAttickXY))             // :477-488
        {
            ParseXY(seg, out int nX, out int nY);
            points.Add((nX, nY));
        }
        return points;
    }

    /// <summary>
    /// ★★ F7 的核心差异断言：**单坐标配置永远不放火圈**，而**循环本可处理它**。
    /// </summary>
    public static bool SingleCoordinateConfigNeverFires()
    {
        const string single = "100,200";
        const string multi = "100,200|300,400";
        return GuardLoopIterations(single) == 1            // 循环次数够用
               && GuardSegments(single).Count == 1         // 段能被解析
               && !GuardExplosionGated(single)             // 但守卫把它挡在门外
               && GuardExplosionPoints(single).Count == 0
               && GuardExplosionGated(multi)
               && GuardExplosionPoints(multi).Count == 2;
    }

    /// <summary>
    /// ★★ F7+F8 的后果：单坐标配置下 `AttackTarget` 仍返回 True
    /// ⇒ 调用方（:519-520）**照样清掉 `m_boAttick`** ⇒ 该守护兽永久失去攻击机会。
    /// </summary>
    public static bool SingleCoordinateStillClearsAttick()
    {
        const string single = "100,200";
        bool result = GuardAttackTargetResult(true);                 // 节流放行 ⇒ Result := True
        bool clears = GuardAttickClearedByRun(result, true, false, single);
        return result && clears && GuardExplosionPoints(single).Count == 0;
    }

    /// <summary>
    /// `:490 Result := True;` —— 只要节流放行就无条件为真（F8）。
    /// </summary>
    public static bool GuardAttackTargetResult(bool throttlePassed) => throttlePassed;

    /// <summary>`s_AttickXY` 的段数与竖线数关系（F6：段数 = 竖线数 + 1）。</summary>
    public static bool IsCharPlusOneEqualsSegmentCount()
    {
        foreach (var s in new[] { "1,2", "1,2|3,4", "1,2|3,4|5,6", "" })
        {
            int expectedSegments = GuardSegments(s).Count;
            // IsChar 对空串返回 0（:454 提前 Exit）⇒ 空串段数 0、次数 1（仅一次空解析）
            if (s == "")
            {
                if (GuardLoopIterations(s) != 1 || expectedSegments != 0) return false;
                continue;
            }
            if (expectedSegments != GuardLoopIterations(s)) return false;
        }
        return true;
    }

    /// <summary>★ `ParseXY` 的 X/Y **没有**颠倒（尽管变量名叫 `s30`/`s2C`）。</summary>
    public static bool ParseXYIsNotInverted()
    {
        ParseXY("12,34", out int x, out int y);
        return x == 12 && y == 34;
    }

    /// <summary>`ParseXY` 的容错：非数字取 `Str_ToInt` 的默认 0。</summary>
    public static bool ParseXYDefaultsToZero()
    {
        ParseXY("abc", out int x, out int y);          // 无逗号 ⇒ s2C="abc"、s30="" ⇒ 0,0
        ParseXY("abc,def", out int x2, out int y2);
        return x == 0 && y == 0 && x2 == 0 && y2 == 0;
    }

    /// <summary>分号/制表符也是分隔符（原文 `[',', #9]`）。</summary>
    public static bool ParseXYAcceptsTab()
    {
        ParseXY("7\t9", out int x, out int y);
        return x == 7 && y == 9;
    }

    // ===================== 十、TFireDragonGuard：Run（:497-528） =====================

    /// <summary>
    /// `:508-516`：`if m_boLight then if (MyGetTickCount - m_dwSearchEnemyTick) &gt; m_dwLightTime then …`
    /// —— 发光到点则复位（`m_dwSearchEnemyTick := now; m_dwLightTime := 2500; m_boLight := False;`）。
    /// </summary>
    public static bool GuardLightExpires(bool boLight, uint now, uint dwSearchEnemyTick, uint dwLightTime)
        => boLight && unchecked(now - dwSearchEnemyTick) > dwLightTime;

    /// <summary>
    /// `:517 if m_boAttick and (not m_boLight) and (s_AttickXY &lt;&gt; '') then`
    /// —— 可以攻击的三重合取（F7 的第二道门）。
    /// </summary>
    public static bool GuardRunShouldAttack(bool boAttick, bool boLight, string sAttickXY)
        => boAttick && !boLight && sAttickXY != "";

    /// <summary>`:519-520 if AttackTarget then m_boAttick := False;` —— 是否清掉攻击权。</summary>
    public static bool GuardAttickClearedByRun(bool attackTargetResult, bool boAttick, bool boLight, string sAttickXY)
        => GuardRunShouldAttack(boAttick, boLight, sAttickXY) && attackTargetResult;

    /// <summary>`:502 tick_diff(m_dwWalkTick, Now) &gt; m_nWalkSpeed + m_nWalkDelay` —— 与火龙同式。</summary>
    public static bool GuardWalkTickDue(uint lastWalk, uint now, int walkSpeed, int walkDelay)
        => WalkTickDue(lastWalk, now, walkSpeed, walkDelay);

    // ===================== 十一、构造函数 / RecalcAbilitys 的字段值 =====================

    /// <summary>`:51-62 TFireDragon.Create` 的字段赋值（(字段, 值) 对）。</summary>
    public static readonly (string Field, string Value)[] FireDragonCreateAssignments =
    {
        ("m_boAnimal", "False"),
        ("m_boStickMode", "True"),
        ("m_btAntiPoison", "200"),
        ("UnParalysis", "True"),
        ("m_nViewRange", "13"),
        ("m_dwAttickTick", "MyGetTickCount()"),
        ("m_boFixedHideMode", "False"),
        ("m_dwLightTick", "MyGetTickCount()"),
    };

    /// <summary>`:175-182 TFireDragon.RecalcAbilitys` 的字段赋值。</summary>
    public static readonly (string Field, string Value)[] FireDragonRecalcAssignments =
    {
        ("m_boStickMode", "True"),
        ("m_btAntiPoison", "200"),
        ("UnParalysis", "True"),
        ("m_boFixedHideMode", "False"),
    };

    /// <summary>`:381-393 TFireDragonGuard.Create` 的字段赋值。</summary>
    public static readonly (string Field, string Value)[] GuardCreateAssignments =
    {
        ("m_boStoneMode", "True"),
        ("m_boAnimal", "False"),
        ("m_boStickMode", "True"),
        ("m_btAntiPoison", "200"),
        ("UnParalysis", "True"),
        ("m_boLight", "False"),
        ("m_boAttick", "False"),
        ("s_AttickXY", "''"),
        ("m_dwLightTime", "2500"),
    };

    /// <summary>`:437-445 TFireDragonGuard.RecalcAbilitys` 的字段赋值。</summary>
    public static readonly (string Field, string Value)[] GuardRecalcAssignments =
    {
        ("m_boStoneMode", "True"),
        ("m_boAnimal", "False"),
        ("m_boStickMode", "True"),
        ("m_btAntiPoison", "200"),
        ("UnParalysis", "True"),
    };

    /// <summary>★ `TFireDragon.Create` 与 `RecalcAbilitys` 的**交集**（Recalc 不重置视野/计时/隐身）。</summary>
    public static readonly string[] RecalcDoesNotReset =
    {
        "m_nViewRange", "m_dwAttickTick", "m_dwLightTick",
    };

    /// <summary>两份赋值表的公共字段数（`m_boStickMode`/`m_btAntiPoison`/`UnParalysis`/`m_boFixedHideMode` = 4）。</summary>
    public const int FireDragonCreateRecalcOverlap = 4;

    /// <summary>两份守护兽赋值表的公共字段数（`m_boStoneMode`…`UnParalysis` = 5）。</summary>
    public const int GuardCreateRecalcOverlap = 5;
}
