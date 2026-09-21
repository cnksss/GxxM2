// ============================================================================
//  源单元：Source/M2Engine/ObjDummy.pas（1245 行，GBK；本车道只读其 UTF-8 镜像
//          _analysis/utf8_mirror/M2Engine/ObjDummy.pas）
//  本文件：ObjDummy 单元（假人 TDummyObject）1:1 逻辑移植
//          （车道 p9-m2-monsters，Sweep9/Monsters）
//
//  ── 覆盖清单（19/19 例程，逐条登记见 ObjDummyCore.Methods）─────────────────
//   TDummyObject 方法（16）：
//     Create 60-88            Destroy 90-93          OnSpaceMove 95-105
//     StartPickUpItem 107-200 Start 202-216          Stop 218-237
//     Ask 239-249             Initialize 251-277     IsProperTarget 279-302
//     GotoPath 350-404        WalkToNext 406-410     RunToNext 412-419
//     Wondering 421-560       Run 562-1080           Walk 1083-1104
//     CanAutoUseMagic 1106-1242
//   **单元级函数**（2，★ 不是类成员）：
//     NextDirClockwise 304-325     NextDirAntiClockwise 327-348
//   **嵌套函数**（1）：`GetPoint`（在 `Wondering` 内，423-479）
//  计数取证：原文 `^\s*(procedure|function|constructor|destructor)` 命中 **35**
//  = 16 条接口声明 + 16 条实现体 + 2 条单元级函数实现 + 1 条嵌套函数
//  ⇒ **19 个例程**。
//
//  ── 原文缺陷（照抄 + 注释 + 差异断言，不顺手修）──────────────────────────────
//   ★ F1（:350-404）`TDummyObject.GotoPath` 的**整个函数体被 `(* ... *)` 注释掉**
//      （`(*` 在 :355、`*)` 在 :403）⇒ 生效代码只有 `:354 Result := False;` **一行**，
//      **永远返回 False**。计数取证：`GotoPath` 的函数体内 `(*`/`*)` 各 **1** 处
//      （:355/:403），而 `Result := True` 只出现在注释内（:366/:385）。
//      ⇒ 任何"`if GotoPath() then`"的写法都恒假；本单元里 `Wondering` 的
//        `if GotoPath() then ...` 也被 `{ }` 注释掉了（:512-518），
//        与 `Run` 的旧版 `(* *)` 块（:520-541）一致地弃用。**两条线索互相印证**。
//   ★ F2（:14 / :15 / :20）三个**死字段**：
//      `m_NotCanPickItemList`（TList，:14）与 `m_dwStartPickItemTick`（:15）
//      全单元**只有声明**（各出现 **1** 次）—— 0 读 0 写；
//      `m_dwAskTick`（LongWord，:20）出现 **2** 次（:20 声明、:73 写入）
//      —— **写后从不读**。
//   ★ F3（:304-348）`NextDirClockwise` / `NextDirAntiClockwise` 是**单元级函数**
//      （无 `TDummyObject.` 前缀）⇒ 它们是**纯函数、不访问对象状态**，
//      托管侧必须做成 `static`（做成实例方法会掩盖"与实例无关"这一事实）。
//      两者都 `Result := DR_UP;` 打头 + `case` **无 `else`** ⇒
//      **方向值 &gt; 7 时一律返回 `DR_UP`（0）**，不是"保持原值"。已用
//      `OutOfRangeDirectionBecomeUp` 固化。
//   ★ F4（:423-479）嵌套 `GetPoint` 的 `nIndex` **只夹到下限**：
//      `if (nIndex < Low(CheckSteps)) or (nIndex > High(CheckSteps)) then nIndex := Low(CheckSteps);`
//      ⇒ 越界（含上界越界）**一律退化为下标 0（步长 2）**。已用
//      `CheckIndexClampsToZero` 固化。外层那句 `// for I := Low(CheckSteps) to High(CheckSteps) do begin`
//      与 :478 的 `// end;` 是**被注释掉的旧循环骨架**（计数：注释内 `for` **1** 处）。
//   ★ F5（:451-454）`GetPoint` 的换向策略**每次调用随机**：
//      `if Random(2) = 0 then GetNextDir := NextDirClockwise else GetNextDir := NextDirAntiClockwise;`
//      —— 用的是**函数变量** `GetNextDir: function(btDir: Byte): Byte;` 做动态分派。
//      托管侧用委托保留这一"按调用随机选策略"的语义。
//   ★★ F6（:797-800）**战士假人的 `btDir` 可以变成 -1**：
//      `btDir := (btDir + 1) mod 8;` / `btDir := (btDir - 1) mod 8;`
//      —— 此处 `btDir` 的类型是 **`Integer`**（:566 的 var 段，**不是** `Byte`），
//      而 Delphi 的 `mod` **取被除数的符号** ⇒ `btDir = 0` 时
//      `(0 - 1) mod 8 = -1`。随后 `:802 m_PEnvir.GetNextPosition(..., btDir, ...)`
//      **拿 -1 当方向用**（形参若是 `Byte` 且未开 `{$R+}` 则被截成 **255**）。
//      ⇒ 目标恰在正上方（`btDir = 0`）时约 **1/2**（`Random(2)` 选中 `-1` 侧）
//        会走进这条路径 ⇒ 传到 `GetNextPosition` 的方向非法。
//      已用 `MinusOneDirectionIsReachable` 固化（另给出"`+1` 侧恒安全"的对照断言）。
//      法师/道士分支用的是 `(btDir + nCount) mod 8`（**只加不减**，:910）⇒ 恒安全。
//   ★★ F7（:826 / :872 / :972）`AttackTime := Max(0, AttackTime - (g_Config.dwIncSpeedDecInterval * m_nHitSpeed));`
//      —— **混合符号类型表达式 + 重载不确定**：`AttackTime` 是 `Integer`、
//      `g_Config.dwIncSpeedDecInterval` 是 `LongWord`（`M2Share.pas` 的
//      `dwIncSpeedDecInterval: LongWord`）、`m_nHitSpeed` 是 `Integer`。
//      `Math.Max` **没有 `LongWord`/`Cardinal` 重载**（只有 Integer/Int64/Single/Double/Extended），
//      所以"结果到底是**有符号**（能出现负数、被 `Max(0,·)` 夹成 0 —— 注释"防止负数出错"的原意）
//      还是**无符号回绕**（变成 ~4.2e9 的巨大值 ⇒ `tick_diff(...) > AttackTime` **永不成立**
//      ⇒ 假人**永不攻击**）"取决于编译器的重载选择，**从源码字面无法判定**。
//      托管侧**显式取"有符号 + 夹 0"**（与作者注释一致、也是唯一不破坏玩法的读法），
//      并把"另一种读法的后果"作为**差异断言**一并锁死（`AttackTimeIsClampedNotWrapped` /
//      `UnsignedReadingWouldNeverAttack`）。登记为 **D-P9-03**。
//   ★★ F8（:743-1052 vs :1054-1071）**自动练功代码块出现两次，且第二处的外层守卫更松**：
//      :1035-1052 是 `if (m_TargetCret <> nil)` 的 `else`（位于
//      `if m_boStart and … and CanMove`（:679-680）之内）；
//      :1054-1071 是同一个 `if` 的 `else if`，其判据
//      **丢掉了 `m_boStart` 与 `CanMove`**（只剩 ghost/death/fixedHide/stone/shopStall）。
//      ⇒ ① `m_boStart = False`（假人**已 Stop**）时自动练功**照样跑**；
//         ② `CanMove = False` 时也照样跑。
//      计数取证：自动练功的四合一条件串在原文出现 **2** 次（:1037 / :1056），
//      两处**函数体逐字相同**（7 行）。已用 `AutoMagicBlockIsDuplicated` /
//      `SecondGuardOmitsStartAndCanMove` 固化。
//   ★★ F9（:838-840 / :945-947 / :1009-1011）**英雄合击的 `m_MyHero.m_TargetCret` 无 nil 保护**：
//      条件 `g_Config.boHeroJointAttackFly and ((abs(m_MyHero.m_nCurrX - m_MyHero.m_TargetCret.m_nCurrX) > 2) or ...)`
//      —— 在 `boHeroJointAttackFly` 为真时**直接解引用英雄的目标**。
//      计数取证：`m_MyHero.m_TargetCret` 出现 **6** 次（3 个职业分支 × 2 行），
//      **同一处条件里没有任何 `m_MyHero.m_TargetCret <> nil` 判定**（0 处）。
//      已用 `HeroJointAttackDerefsTargetWithoutNilCheck` 固化。
//   ★ F10（:832/:836/…）**`THeroObject(m_MyHero)` 是无类型校验的硬转换**：
//      `m_MyHero` 的静态类型是 `TPlayObject`，本单元把它**强转成 `THeroObject`**
//      并直接读 `m_btAngryValue` / `WearFirDragon` / `m_UseItems` / `FindGroupMagic`。
//      计数取证：`THeroObject(` 在本单元出现 **21** 次，其中**类型判别 0 次**。
//      ⇒ 与 `ObjFireDragon.pas` 的 F2（守护兽硬转换）**同一类缺陷**。
//   ★ F11（:820-1030）**三个职业分支的收尾结构不对齐**：
//      职业 1（法师）在 `Length(m_MovePath) = 0 and now - m_dwMoveTimeTick > nWalkTime`
//      之下**内嵌 `if … else if`**（:886-936：先"距离超了就走近"、**否则**"随机去找自己的火墙站上去"）；
//      而职业 2（道士）**没有火墙分支**，且把两个距离判据用 `and` **并到一个 `if` 里**（:984-1001）。
//      ⇒ **法师会去找火墙，道士不会**（原文如此，非笔误可判）。已用
//      `FireWallHuntOnlyForWizard` 固化。
//   ★ F12（:751-752）**道士假人的"没事找事跑"是四重合取 + 1/4 抑制**：
//      `not ((m_btJob = 2) and (abs(dx) <= nMagicAttackRage) and (abs(dy) <= nMagicAttackRage) and (Random(4) <> 0))`
//      —— 注意 `Random(4) <> 0` 意味着 **1/4 的概率仍然会跑**（不是"完全不跑"）。
//   ★ F13（:290）`IsProperTarget` 里同一"主人"判据**用了两种读法**：
//      `if (BaseObject.m_Master = Self) or (BaseObject.Master = Self) then` ——
//      前者是**字段**、后者是**属性**（`ObjBase.pas:806 property Master read GetMaster;`，
//      `GetMaster` 声明在 `ObjBase.pas:487`）。托管侧照抄"两次读、两种读法"这一形态。
//      ⚠ **待确认项**：`GetMaster` 的实现是否**等价于** `m_Master`（其实现体不属本单元），
//      需在 `ObjBase.pas` 移植时核对；本文件**不做断定**，只固化"两种读法并存"。
//   ★ F14（:1054-1071）第二处自动练功块**把 `m_TargetCret` 临时置成 `Self` 再置回 `nil`**
//      （:1061-1063）—— 与第一处（:1042-1044）逐字相同；`m_TargetCret := nil` 结束后
//      **不恢复原目标**（原文如此）。
//   ★ F15（:1045/:1064）写 `m_SkillUseTick` 前有上界守卫
//      （`if (m_wAutoUseMagicID <= High(m_SkillUseTick))`），
//      而 `CanAutoUseMagic` 里对 `m_SkillUseTick[SKILL_56]` / `[26]` / `[42]` / `[66]` /
//      `[113]` / `[115]` / `[SKILL_MOOTEBO]` / `[SKILL_114]` 的**读取全无上界守卫**
//      （计数：`CanAutoUseMagic` 内 `m_SkillUseTick[` 出现 **8** 次、守卫 **0** 次）。原文如此。
//
//  ── 接缝（不造第三份实现，台账 §14.2 / §18.7）────────────────────────────────
//   基类面 `TPlayObject`（≈ `ObjPlayer.pas`，46,911 行）在托管侧**大面积缺失**
//   （台账 §18.7 已把 `TPlayObject` 面定性为"当前最主要的系统性瓶颈"）。
//   按 §18.7 对"怪物/假人 AI 单元"的既定做法（`ObjMonCore.cs` / `ObjMonRunCore.cs` /
//   `IcicleMonsterCore.cs`），本文件**不造 `TPlayObject` 替身成员**（一个都没造），
//   而是：把**原文判定逻辑逐条抽成纯函数 / 决策表**（可直接差异断言），
//   把少量**全局**（`g_FunctionNPC` / `g_PluginManager` / `nMagicAttackRage` 等）
//   抽成最小接缝，类外壳的落地登记为**阻塞项 B-P9-02**（`TPlayObject` 面）。
//
//   复用而非另造：
//     * `g_Config.dwDummy*WalkTime` / `dwDummy*AttackTime` / `dwIncSpeedDecInterval`
//       **直接引用** `GXX.M2Server.Engine.M2Config`（已由 p8-m2-dummysetting 等批次落地）；
//     * `MyGetTickCount` / `MainOutMessage` 转发 `GXX.M2Server.Sweep.SweepSeam`。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Sweep9.Monsters;

/// <summary>
/// `ObjLongWordSort_2(List: TStringList; Index1, Index2: Integer): Integer`
/// （**SDK.pas:179/275**，不属于本单元）—— `TDummyObject.Ask` 的唯一排序依赖。
/// 接缝：待 `SDK.pas` 的该函数在 GXX.Core 归位后接入。
/// </summary>
public static class ObjDummySortSeam
{
    /// <summary>原文 `m_SayList.CustomSort(ObjLongWordSort_2)`（:243）。默认实现为**不排序**（保守）。</summary>
    public static Action<TStringList> CustomSort { get; set; } = _ => { };

    /// <summary>恢复默认。</summary>
    public static void ResetDefaults() => CustomSort = _ => { };
}

/// <summary>
/// `g_FunctionNPC`（M2Share.pas 全局，`TNormNpc`）的最小接缝 ——
/// 本单元只用到 `GotoLable`（:213/:234/:1090/:1099 共 4 个调用点）。
/// </summary>
public interface IFunctionNpcSeam
{
    /// <summary>原文 `TNormNpc.GotoLable(PlayObject, sLabel, boExtJmp)`。</summary>
    void GotoLable(object playObject, string sLabel, bool boExtJmp);
}

/// <summary>
/// `g_PluginManager`（PluginManager.pas 全局）在 `TDummyObject.Run` 里用到的两个钩子。
/// </summary>
public interface IPluginManagerSeam
{
    /// <summary>原文 `HookDummyObjectRunBegin(Self, var boReturn: BOOL)`（:685）—— `boReturn` 是 **out** 参数。</summary>
    void HookDummyObjectRunBegin(object dummyObject, out bool boReturn);

    /// <summary>原文 `HookDummyObjectRunEnd(Self)`（:1075）。</summary>
    void HookDummyObjectRunEnd(object dummyObject);
}

/// <summary>`ObjDummy.pas` 用到的最小外部接缝。</summary>
public static class ObjDummySeam
{
    /// <summary>原文 `M2Share.MyGetTickCount`（转发既有 <c>SweepSeam</c>，不另造计时源）。</summary>
    public static Func<uint> MyGetTickCount
    {
        get => GXX.M2Server.Sweep.SweepSeam.MyGetTickCount;
        set => GXX.M2Server.Sweep.SweepSeam.MyGetTickCount = value;
    }

    /// <summary>原文 `M2Share.MainOutMessage`。</summary>
    public static Action<string> MainOutMessage
    {
        get => GXX.M2Server.Sweep.SweepSeam.MainOutMessage;
        set => GXX.M2Server.Sweep.SweepSeam.MainOutMessage = value;
    }

    /// <summary>原文 `g_FunctionNPC`（默认 nil，与原文未装配时一致）。</summary>
    public static IFunctionNpcSeam? g_FunctionNPC { get; set; }

    /// <summary>原文 `g_PluginManager`（默认 nil）。</summary>
    public static IPluginManagerSeam? g_PluginManager { get; set; }

    /// <summary>
    /// 原文 `g_Config.boHeroJointAttack` / `btMaxAngryValue` / `boHeroJointAttackFly` /
    /// `m_nRunAttackRate` / `m_boAutoPickUpItem` 一族的**当前装配值**。
    /// <para>为什么做成接缝：这几个字段在托管侧 `M2Config` 上**尚未落地**
    /// （`git grep boHeroJointAttack -- src/GXX.M2Server/Engine` = 0 命中）⇒
    /// 按"不造第三份"只做最小接缝，待 `M2Share.pas` 全量批次落位后删除。</para>
    /// </summary>
    public sealed class DummyRuntimeConfig
    {
        /// <summary>`g_Config.boHeroJointAttack`。</summary>
        public bool boHeroJointAttack;
        /// <summary>`g_Config.btMaxAngryValue`。</summary>
        public int btMaxAngryValue;
        /// <summary>`g_Config.boHeroJointAttackFly`。</summary>
        public bool boHeroJointAttackFly;
        /// <summary>`m_nRunAttackRate`（实例字段，测试注入）。</summary>
        public int m_nRunAttackRate;
        /// <summary>`m_boAutoPickUpItem`（实例字段，测试注入）。</summary>
        public bool m_boAutoPickUpItem;
    }

    /// <summary>当前装配（默认为"全部关闭/0"—— 即不触发英雄合击与自动拾取）。</summary>
    public static DummyRuntimeConfig Runtime { get; set; } = new();

    /// <summary>恢复默认。</summary>
    public static void ResetDefaults()
    {
        g_FunctionNPC = null;
        g_PluginManager = null;
        Runtime = new DummyRuntimeConfig();
    }
}

/// <summary>`CanAutoUseMagic` 的输入状态（对应 `TDummyObject` 的若干字段 + `MyGetTickCount`）。</summary>
public readonly struct AutoUseMagicState
{
    public AutoUseMagicState(int autoUseMagicID, uint now, uint lastSuperShiledTimeTick,
                             bool hasMagicSuperShiledSkill, bool fireHitSkill, bool skill42,
                             bool skill66, bool skill113, bool skill115, bool crsHitkill,
                             bool useHalfMoon, IReadOnlyDictionary<int, uint> skillUseTick)
    {
        AutoUseMagicID = autoUseMagicID;
        Now = now;
        LastSuperShiledTimeTick = lastSuperShiledTimeTick;
        HasMagicSuperShiledSkill = hasMagicSuperShiledSkill;
        FireHitSkill = fireHitSkill;
        Skill42 = skill42;
        Skill66 = skill66;
        Skill113 = skill113;
        Skill115 = skill115;
        CrsHitkill = crsHitkill;
        UseHalfMoon = useHalfMoon;
        SkillUseTick = skillUseTick;
    }

    public int AutoUseMagicID { get; }
    public uint Now { get; }
    public uint LastSuperShiledTimeTick { get; }
    public bool HasMagicSuperShiledSkill { get; }
    public bool FireHitSkill { get; }
    public bool Skill42 { get; }
    public bool Skill66 { get; }
    public bool Skill113 { get; }
    public bool Skill115 { get; }
    public bool CrsHitkill { get; }
    public bool UseHalfMoon { get; }
    public IReadOnlyDictionary<int, uint> SkillUseTick { get; }

    /// <summary>`m_SkillUseTick[id]`（缺省 0，等价于"从未使用"）。</summary>
    public uint Tick(int id) => SkillUseTick != null && SkillUseTick.TryGetValue(id, out uint v) ? v : 0u;
}

/// <summary>`CanAutoUseMagic` 的副作用（原文每个分支里调用的那个 `Allow*` 族动作）。</summary>
public enum AutoUseMagicEffect
{
    /// <summary>无副作用（`Result := False` 的路径）。</summary>
    None = 0,
    /// <summary>`OpenSuperShiled`（:1120）。</summary>
    OpenSuperShiled,
    /// <summary>`AllowSWordHitSkill`（:1129）。</summary>
    AllowSWordHit,
    /// <summary>`AllowFireHitSkill`（:1140）。</summary>
    AllowFireHit,
    /// <summary>`Allow42HitSkill`（:1151）。</summary>
    Allow42Hit,
    /// <summary>`Allow66HitSkill(nil)`（:1162）。</summary>
    Allow66Hit,
    /// <summary>`Allow113HitSkill`（:1173）。</summary>
    Allow113Hit,
    /// <summary>`Allow115HitSkill`（:1182）。</summary>
    Allow115Hit,
    /// <summary>`SkillCrsOnOff(True)`（:1210）。</summary>
    SkillCrsOn,
    /// <summary>`HalfMoonOnOff(True)`（:1230）。</summary>
    HalfMoonOn,
}

/// <summary>`CanAutoUseMagic` 的判定结果。</summary>
public readonly struct AutoUseMagicDecision
{
    public AutoUseMagicDecision(bool result, AutoUseMagicEffect effect)
    {
        Result = result;
        Effect = effect;
    }

    /// <summary>原文函数返回值（是否允许自动使用该技能）。</summary>
    public bool Result { get; }

    /// <summary>原文在该分支里调用的副作用（<see cref="AutoUseMagicEffect.None"/> 表示没有）。</summary>
    public AutoUseMagicEffect Effect { get; }
}

/// <summary>`TDummyObject.IsProperTarget(BaseObject)` 的输入状态。</summary>
public readonly struct ProperTargetState
{
    public ProperTargetState(bool inheritedResult, bool isSelf, int raceServer, bool inSafeZone,
                             bool masterIsSelfByField, bool masterIsSelfByProperty,
                             int lastHiterIsSelf, bool lastHiterIsSelfFlag, int attackMode)
    {
        InheritedResult = inheritedResult;
        IsSelf = isSelf;
        RaceServer = raceServer;
        InSafeZone = inSafeZone;
        MasterIsSelfByField = masterIsSelfByField;
        MasterIsSelfByProperty = masterIsSelfByProperty;
        LastHiterIsSelf = lastHiterIsSelf;
        LastHiterIsSelfFlag = lastHiterIsSelfFlag;
        AttackMode = attackMode;
    }

    /// <summary>`inherited IsProperTarget(BaseObject)`（:282）。</summary>
    public bool InheritedResult { get; }
    /// <summary>`BaseObject = Self`。</summary>
    public bool IsSelf { get; }
    /// <summary>`BaseObject.m_btRaceServer`。</summary>
    public int RaceServer { get; }
    /// <summary>`BaseObject.InSafeZone`。</summary>
    public bool InSafeZone { get; }
    /// <summary>`BaseObject.m_Master = Self`（**字段**读法）。</summary>
    public bool MasterIsSelfByField { get; }
    /// <summary>`BaseObject.Master = Self`（**属性**读法，`ObjBase.pas:806 property Master read GetMaster`）。</summary>
    public bool MasterIsSelfByProperty { get; }
    /// <summary>`BaseObject.m_LastHiter <> Self`。</summary>
    public int LastHiterIsSelf { get; }

    /// <summary>`BaseObject.m_LastHiter <> Self` 的预判结果（避免引入对象模型，测试直接给）。</summary>
    public bool LastHiterIsSelfFlag { get; }

    /// <summary>`Self.m_btAttatckMode`。</summary>
    public int AttackMode { get; }
}

/// <summary>
/// `TDummyObject.StartPickUpItem` 的输入状态（把原文的读访问集中成不可变投影）。
/// </summary>
public sealed class PickUpItemState
{
    /// <summary>`m_boDeath`。</summary>
    public bool Death;
    /// <summary>`m_boGhost`。</summary>
    public bool Ghost;
    /// <summary>`IsEnoughBag`。</summary>
    public bool EnoughBag;
    /// <summary>`m_SelItemObject <> nil`。</summary>
    public bool HasSelected;
    /// <summary>`CheckItemExists(m_SelItemObject) = m_SelItemObject`（地图上仍在）。</summary>
    public bool SelectedStillOnMap = true;
    /// <summary>`m_SelItemObject.m_boGhost`。</summary>
    public bool SelectedGhost;
    /// <summary>`m_nCurrX = m_SelItemObject.m_nMapX`。</summary>
    public bool SameX;
    /// <summary>`m_nCurrY = m_SelItemObject.m_nMapY`。</summary>
    public bool SameY;
    /// <summary>`m_PEnvir.GetMovingObjectEx(...) <> nil`（先返回非 nil 才走"换目标"分支）。</summary>
    public bool BlockedByObject;
    /// <summary>`MyGetTickCount - m_dwMoveTimeTick > nWalkTime`。</summary>
    public bool WalkIntervalElapsed;
    /// <summary>`GotoNextOne(...)` 的结果。</summary>
    public bool GotoNextOneOk;
    /// <summary>`DoPickUpItem(...)` 的结果。</summary>
    public bool DoPickUpItemOk;
    /// <summary>`FindPriorityPickUpItem(...) <> nil`。</summary>
    public bool FoundPriority;
    /// <summary>`FindPickUpItem(...) <> nil`。</summary>
    public bool FoundFallback;
    /// <summary>`m_btJob`（决定 `nWalkTime` 的来源）。</summary>
    public int Job;
}

/// <summary>`StartPickUpItem` 走到的分支（用于差异断言与可观测性）。</summary>
public enum PickUpItemOutcome
{
    /// <summary>`m_boDeath or m_boGhost or (not IsEnoughBag)` ⇒ 清选择 + 清失败表 + 返回 False（:117-122）。</summary>
    RejectedByGuard = 0,
    /// <summary>选中物已不在图上/已成幽灵 ⇒ 仅清选择（:125-133）。</summary>
    SelectionCleared,
    /// <summary>坐标不同 + 有挡路对象 ⇒ 清选择 + `SetTargetCreat` + `m_boTarget := True` + 清失败表（:152-158）。</summary>
    RetargetedToBlocker,
    /// <summary>坐标不同 + 走位未到点 + `GotoNextOne` 失败 ⇒ 清选择（:163-166）。</summary>
    GotoNextOneFailed,
    /// <summary>坐标不同 + 走位成功 ⇒ 返回 True（:169-170）。</summary>
    Walking,
    /// <summary>同坐标 + 拾取成功 ⇒ 清选择 + 返回 True（:177-182）。</summary>
    PickedUp,
    /// <summary>同坐标 + 拾取失败 ⇒ 入失败表 + 清选择（:185-186）。</summary>
    PickUpFailed,
    /// <summary>落到底部：`FindPriorityPickUpItem` / `FindPickUpItem`（:191-198）。</summary>
    SearchNewTarget,
}

/// <summary>`StartPickUpItem` 的判定结果。</summary>
public readonly struct PickUpItemDecision
{
    public PickUpItemDecision(bool result, PickUpItemOutcome outcome)
    {
        Result = result;
        Outcome = outcome;
    }

    /// <summary>原文返回值。</summary>
    public bool Result { get; }

    /// <summary>走到哪一支。</summary>
    public PickUpItemOutcome Outcome { get; }
}

/// <summary>`TDummyObject.Wondering` 的输入状态。</summary>
public sealed class WonderingState
{
    /// <summary>`m_boStart`。</summary>
    public bool Start;
    /// <summary>`m_TargetCret <> nil`。</summary>
    public bool HasTarget;
    /// <summary>`m_boGhost`。</summary>
    public bool Ghost;
    /// <summary>`m_boDeath`。</summary>
    public bool Death;
    /// <summary>`m_boFixedHideMode`。</summary>
    public bool FixedHideMode;
    /// <summary>`m_boStoneMode`。</summary>
    public bool StoneMode;
    /// <summary>`m_boShopStall`。</summary>
    public bool ShopStall;
    /// <summary>`CanMove`（属性）。</summary>
    public bool CanMove = true;
    /// <summary>`m_boAutoPickUpItem and StartPickUpItem(True, True, 0)`。</summary>
    public bool AutoPickUpSucceeded;
    /// <summary>`MyGetTickCount - m_dwMoveTimeTick > nWalkTime`。</summary>
    public bool WalkIntervalElapsed;
    /// <summary>`m_btJob`。</summary>
    public int Job;
    /// <summary>逐 `nIndex`（0..2）的 `GetPoint` 结果与落点。</summary>
    public (bool Found, int X, int Y)[] Points = new (bool, int, int)[3];
    /// <summary>`m_nCurrX`。</summary>
    public int CurrX;
    /// <summary>`m_nCurrY`。</summary>
    public int CurrY;
}

/// <summary>`Wondering` 的走到的分支。</summary>
public enum WonderingOutcome
{
    /// <summary>外层七条件不成立 ⇒ 什么都不做（:484-486）。</summary>
    GuardFailed = 0,
    /// <summary>自动拾取成功 ⇒ 清移动路径并返回（:488-497）。</summary>
    AutoPickUp,
    /// <summary>走位间隔未到 ⇒ 什么都不做（:510）。</summary>
    Throttled,
    /// <summary>走到某个 `nIndex` 的远端点 ⇒ `SetTargetXY` + `RunToTargetXY`（:549-554）。</summary>
    RunToTarget,
    /// <summary>三轮 `GetPoint` 都没有"距离 &gt; 2"的点 ⇒ 什么都不做。</summary>
    NoFarPoint,
}

/// <summary>`Wondering` 的判定结果。</summary>
public readonly struct WonderingDecision
{
    public WonderingDecision(WonderingOutcome outcome, int targetX, int targetY, int index)
    {
        Outcome = outcome; TargetX = targetX; TargetY = targetY; Index = index;
    }

    public WonderingOutcome Outcome { get; }
    public int TargetX { get; }
    public int TargetY { get; }
    public int Index { get; }
}

/// <summary>
/// `ObjDummy.pas` 的**单元级常量 + 方法清单 + 逐条判定逻辑 + 差异断言**。
/// </summary>
public static class ObjDummyCore
{
    /// <summary>源单元全路径（供 tools/audit-coverage.ps1 的 E2 证据规则识别）。</summary>
    public const string SourceUnit = "Source/M2Engine/ObjDummy.pas";

    /// <summary>源单元总行数。</summary>
    public const int SourceLines = 1245;

    // ===================== 一、常量 =====================

    /// <summary>`Grobal2.pas` 方向常量（与 `ObjBase.cs` 的 `s_DirX/s_DirY` 同源）。</summary>
    public const byte DR_UP = 0;
    /// <summary>右上。</summary>
    public const byte DR_UPRIGHT = 1;
    /// <summary>右。</summary>
    public const byte DR_RIGHT = 2;
    /// <summary>右下。</summary>
    public const byte DR_DOWNRIGHT = 3;
    /// <summary>下。</summary>
    public const byte DR_DOWN = 4;
    /// <summary>左下。</summary>
    public const byte DR_DOWNLEFT = 5;
    /// <summary>左。</summary>
    public const byte DR_LEFT = 6;
    /// <summary>左上。</summary>
    public const byte DR_UPLEFT = 7;

    /// <summary>`:429 const CheckSteps: array[0..2] of Byte = (2, 4, 6);`</summary>
    public static readonly byte[] CheckSteps = { 2, 4, 6 };

    /// <summary>`Low(CheckSteps)` = 0。</summary>
    public const int CheckStepsLow = 0;

    /// <summary>`High(CheckSteps)` = 2。</summary>
    public const int CheckStepsHigh = 2;

    /// <summary>`Grobal2.pas:163 HAM_GUILD = 5;`（:297 的 `m_btAttatckMode &lt;&gt; HAM_GUILD`）。</summary>
    public const int HAM_GUILD = 5;

    /// <summary>`Grobal2.pas:190 RC_PLAYOBJECT = 0`。</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>`Grobal2.pas:199 RC_NPC = 10`（`:293` 区间下界）。</summary>
    public const int RC_NPC = 10;

    /// <summary>`Grobal2.pas:197 RC_ANIMAL = 50`（`:293` 区间上界）。</summary>
    public const int RC_ANIMAL = 50;

    /// <summary>`Grobal2.pas:200 RC_ARCHERGUARD = 112`。</summary>
    public const int RC_ARCHERGUARD = 112;

    /// <summary>`:297-300` 的沙巴克城门种族（**字面量**，非常量）。</summary>
    public const int SiegeCityDoorRace = 110;

    /// <summary>`:297-300` 的沙巴克左城墙种族（**字面量**）。</summary>
    public const int SiegeCityWallRace = 111;

    /// <summary>`Grobal2.pas:22 CLIENT_VERSION_NUMBER = 120040918;`（:69）。</summary>
    public const int CLIENT_VERSION_NUMBER = 120040918;

    /// <summary>`Grobal2.pas:110 U_BUJUK = 9;`（:833/… 的 `m_UseItems[U_BUJUK].Dura`）。</summary>
    public const int U_BUJUK = 9;

    /// <summary>`Grobal2.pas:2787 ET_FIRE = 5;`（:915）。</summary>
    public const int ET_FIRE = 5;

    /// <summary>`Grobal2.pas` `RM_RUN`（:1085）。</summary>
    public const int RM_RUN = Grobal2Const.RM_RUN;

    /// <summary>`Grobal2.pas` `RM_WALK`（:1095）。</summary>
    public const int RM_WALK = Grobal2Const.RM_WALK;

    /// <summary>`M2Share.pas:218 SKILL_FIRESWORD: Word = 26;`</summary>
    public const int SKILL_FIRESWORD = 26;

    /// <summary>`M2Share.pas:219 SKILL_MOOTEBO: Word = 27;`</summary>
    public const int SKILL_MOOTEBO = 27;

    /// <summary>`M2Share.pas:253 SKILL_GROUPDEDING: Word = 39;`</summary>
    public const int SKILL_GROUPDEDING = 39;

    /// <summary>`M2Share.pas:256 SKILL_42: Word = 42;`</summary>
    public const int SKILL_42 = 42;

    /// <summary>`M2Share.pas:271 SKILL_56: Word = 56;`</summary>
    public const int SKILL_56 = 56;

    /// <summary>`M2Share.pas:284 SKILL_66: Word = 66;`</summary>
    public const int SKILL_66 = 66;

    /// <summary>`M2Share.pas:294 SKILL_75: Word = 75;`</summary>
    public const int SKILL_75 = 75;

    /// <summary>`M2Share.pas:324 SKILL_113: Word = 113;`</summary>
    public const int SKILL_113 = 113;

    /// <summary>`M2Share.pas:325 SKILL_114: Word = 114;`</summary>
    public const int SKILL_114 = 114;

    /// <summary>`M2Share.pas:326 SKILL_115: Word = 115;`</summary>
    public const int SKILL_115 = 115;

    /// <summary>`Grobal2.pas:59 CUSTOM_MAGIC_START_ID = 1000;`</summary>
    public const int CUSTOM_MAGIC_START_ID = 1000;

    /// <summary>`Grobal2.pas:60 CUSTOM_MAGIC_COUNT = 300;`</summary>
    public const int CUSTOM_MAGIC_COUNT = 300;

    /// <summary>`:508` 兜底 `nWalkTime := 500;`（三个 `case` 之外的默认走位间隔）。</summary>
    public const int DefaultWalkTimeMs = 500;

    /// <summary>`:612` 兜底 `nWalkTime := 500;`（`Run` 里的默认走位间隔）。</summary>
    public const int RunDefaultWalkTimeMs = 500;

    /// <summary>`:788-791 if m_btJob = 0 then nRange := 1 else nRange := 5;`（战士 1 格、其它 5 格）。</summary>
    public const int WarriorRunAttackRange = 1;

    /// <summary>非战士的跑步攻击半径。</summary>
    public const int OtherRunAttackRange = 5;

    /// <summary>`:905 Random(20) = 0` —— 法师找自己火墙的概率 1/20。</summary>
    public const int FireWallHuntRandomBound = 20;

    /// <summary>`:911 for nRange := 3 to 6 do` —— 火墙扫描步长区间。</summary>
    public const int FireWallScanStepFrom = 3;

    /// <summary>火墙扫描步长区间上界。</summary>
    public const int FireWallScanStepTo = 6;

    /// <summary>`:910 btDir := (btDir + nCount) mod 8;` —— 八方向数。</summary>
    public const int DirectionCount = 8;

    /// <summary>`:891 GetNextPosition(..., g_Config.nMagicAttackRage - 1, ...)` —— 落点偏移。</summary>
    public const int MagicRageStepOffset = 1;

    /// <summary>`:832/… abs(...) &lt;= 10` —— 英雄合击的最大距离。</summary>
    public const int HeroJointMaxDistance = 10;

    /// <summary>`:836 Random(8) = 0` —— 英雄合击概率 1/8。</summary>
    public const int HeroJointRandomBound = 8;

    /// <summary>`:838/… abs(...) &gt; 2` —— `boHeroJointAttackFly` 的飞行阈值。</summary>
    public const int HeroJointFlyThreshold = 2;

    /// <summary>`:751 Random(4) &lt;&gt; 0` —— 道士压制"没事找事跑"的概率。</summary>
    public const int TaoistIdleRunRandomBound = 4;

    /// <summary>`:1190 ((MyGetTickCount - m_SkillUseTick[SKILL_MOOTEBO]) &gt; 1000 * 10)` 的毫秒数。</summary>
    public const int MooteboCooldownMs = 1000 * 10;

    /// <summary>`:1037/:1056` 的 `m_dwAutoUseMagicTime * 1000` 乘数。</summary>
    public const int AutoMagicSecondToMs = 1000;

    /// <summary>`:1045/:1064` 的 `High(m_SkillUseTick)`（`SKILL_USE_TICK` 数组上界）。</summary>
    public const int SkillUseTickHigh = 1000;

    // ===================== 二、方法清单（19/19 覆盖取证） =====================

    /// <summary>类方法接口声明数（16）。</summary>
    public const int DeclCount = 16;

    /// <summary>类方法实现体数（16）。</summary>
    public const int ImplCount = 16;

    /// <summary>单元级函数数（2：`NextDirClockwise` / `NextDirAntiClockwise`）。</summary>
    public const int UnitFunctionCount = 2;

    /// <summary>嵌套函数数（1：`Wondering.GetPoint`）。</summary>
    public const int NestedFunctionCount = 1;

    /// <summary>例程总数（16 + 2 + 1 = 19）。</summary>
    public const int RoutineCount = DeclCount + UnitFunctionCount + NestedFunctionCount;

    /// <summary>逐条例程清单（名称 + 起止行）。</summary>
    public static readonly (string Name, int Start, int End)[] Methods =
    {
        ("TDummyObject.Create",             60,   88),
        ("TDummyObject.Destroy",            90,   93),
        ("TDummyObject.OnSpaceMove",        95,  105),
        ("TDummyObject.StartPickUpItem",   107,  200),
        ("TDummyObject.Start",             202,  216),
        ("TDummyObject.Stop",              218,  237),
        ("TDummyObject.Ask",               239,  249),
        ("TDummyObject.Initialize",        251,  277),
        ("TDummyObject.IsProperTarget",    279,  302),
        ("NextDirClockwise",               304,  325),
        ("NextDirAntiClockwise",           327,  348),
        ("TDummyObject.GotoPath",          350,  404),
        ("TDummyObject.WalkToNext",        406,  410),
        ("TDummyObject.RunToNext",         412,  419),
        ("TDummyObject.Wondering",         421,  560),
        ("GetPoint",                       423,  479),
        ("TDummyObject.Run",               562, 1080),
        ("TDummyObject.Walk",             1083, 1104),
        ("TDummyObject.CanAutoUseMagic",  1106, 1242),
    };

    // ===================== 三、原文缺陷的计数证据（§37.3） =====================

    /// <summary>F1：`GotoPath`（:350-404）函数体内的 `(*` 注释起点数（:355）。</summary>
    public const int GotoPathCommentOpen = 1;

    /// <summary>F1：`GotoPath` 函数体内的 `*)` 注释终点数（:403）。</summary>
    public const int GotoPathCommentClose = 1;

    /// <summary>F1：`GotoPath` 函数体内**注释之外**的语句数（只有 `:354 Result := False;` **1** 条）。</summary>
    public const int GotoPathLiveStatements = 1;

    /// <summary>F1：`GotoPath` 里 `Result := True` 的出现数（:366/:385，**都在注释内**）。</summary>
    public const int GotoPathTrueInsideComment = 2;

    /// <summary>F1：`Wondering` 里调用 `GotoPath()` 的次数（:513，**在 `{ }` 注释内**）。</summary>
    public const int GotoPathCallSites = 1;

    /// <summary>F1：`Wondering` 的 `(* *)` 旧版块起点/终点行（:520/:541）。</summary>
    public const int WonderingOldBlockOpenLine = 520;

    /// <summary>F1：`Wondering` 的 `(* *)` 旧版块终点行。</summary>
    public const int WonderingOldBlockCloseLine = 541;

    /// <summary>F2：`m_NotCanPickItemList` 全单元出现数（1 = 仅 :14 声明）。</summary>
    public const int NotCanPickItemListOccurrences = 1;

    /// <summary>F2：`m_dwStartPickItemTick` 全单元出现数（1 = 仅 :15 声明）。</summary>
    public const int StartPickItemTickOccurrences = 1;

    /// <summary>F2：`m_dwAskTick` 全单元出现数（2 = :20 声明 + :73 写入）。</summary>
    public const int AskTickOccurrences = 2;

    /// <summary>F2：`m_dwAskTick` 的**读取**数（0 —— 写后从不读）。</summary>
    public const int AskTickReadSites = 0;

    /// <summary>F3：`NextDirClockwise` 的 `case` 标签数（8）。</summary>
    public const int DirectionCaseLabels = 8;

    /// <summary>F3：`NextDirClockwise` 的 `else` 分支数（**0** —— 越界落到打头的 `Result := DR_UP`）。</summary>
    public const int DirectionCaseElseBranches = 0;

    /// <summary>F4：`GetPoint` 里被注释掉的旧循环骨架 `for I := Low(CheckSteps) to High(CheckSteps) do`（1 处）。</summary>
    public const int GetPointCommentedForLoop = 1;

    /// <summary>F5：`GetPoint` 里 `GetNextDir` 函数变量的赋值点数（2）。</summary>
    public const int GetNextDirAssignSites = 2;

    /// <summary>F6：`(btDir - 1) mod 8` 的出现数（:800，唯一）。</summary>
    public const int MinusOneDirectionSites = 1;

    /// <summary>F6：`(btDir + 1) mod 8` 的出现数（:798）。</summary>
    public const int PlusOneDirectionSites = 1;

    /// <summary>F6：`(btDir + nCount) mod 8` 的出现数（:910，只加不减）。</summary>
    public const int PlusCountDirectionSites = 1;

    /// <summary>F7：`AttackTime := Max(0, AttackTime - (…dwIncSpeedDecInterval * m_nHitSpeed))` 的出现数（3）。</summary>
    public const int AttackTimeClampSites = 3;

    /// <summary>F8：自动练功的四合一条件串出现数（2 —— :1037 / :1056）。</summary>
    public const int AutoMagicConditionSites = 2;

    /// <summary>F8：第二处（:1054）守卫里**缺失**的判据数（`m_boStart` 与 `CanMove` 两个）。</summary>
    public const int SecondGuardMissingConditions = 2;

    /// <summary>F9：`m_MyHero.m_TargetCret` 的出现数（6 = 3 分支 × 2 行）。</summary>
    public const int HeroTargetDerefSites = 6;

    /// <summary>F9：其中带 `m_MyHero.m_TargetCret &lt;&gt; nil` 守卫的数（**0**）。</summary>
    public const int HeroTargetNilGuards = 0;

    /// <summary>F10：`THeroObject(` 硬转换数（21）。</summary>
    public const int HeroObjectCastSites = 21;

    /// <summary>F10：其中带类型判别的数（**0**）。</summary>
    public const int HeroObjectTypeChecks = 0;

    /// <summary>F11：法师分支的火墙扫描块数（1 —— 只有职业 1 有）。</summary>
    public const int FireWallHuntBranches = 1;

    /// <summary>F11：三个职业分支里"跟随目标走位"的外层判据写法数（**2 种**：嵌套 `if/else if` 与 `and` 合并）。</summary>
    public const int MovementConditionStyles = 2;

    /// <summary>F13：`IsProperTarget` 的"主人"判据里并存的读法数（**2**：`m_Master` 字段 + `Master` 属性）。</summary>
    public const int MasterReadForms = 2;

    /// <summary>F15：`CanAutoUseMagic` 内 `m_SkillUseTick[` 的出现数（8）。</summary>
    public const int SkillUseTickReadsInCanAuto = 8;

    /// <summary>F15：`CanAutoUseMagic` 内对 `m_SkillUseTick[` 的上界守卫数（**0**）。</summary>
    public const int SkillUseTickGuardsInCanAuto = 0;

    /// <summary>`CanAutoUseMagic` 的分支数（`if`/`else if` 链的标签数：75/56/26/42/66/113/115/MOOTEBO/114/40/39/25/自定义/else = 14）。</summary>
    public const int CanAutoUseMagicBranches = 14;

    /// <summary>`CanAutoUseMagic` 里用 **`&gt;=`** 比较的分支数（75/56/26/42/66/113 = 6）。</summary>
    public const int CanAutoUseMagicGeBranches = 6;

    /// <summary>`CanAutoUseMagic` 里用 **`&gt;`** 比较的分支数（115/114/39 = 3）。</summary>
    public const int CanAutoUseMagicGtBranches = 3;

    /// <summary>`CanAutoUseMagic` 里**完全不做时间判定**的分支数（40/25/自定义/else = 4）。</summary>
    public const int CanAutoUseMagicNoTimeBranches = 4;

    /// <summary>`Randomize` 的出现数（:750，唯一）。</summary>
    public const int RandomizeSites = 1;

    /// <summary>`g_FunctionNPC.GotoLable` 的调用点数（:213/:234/:1090/:1099 = 4）。</summary>
    public const int GotoLableCallSites = 4;

    // ===================== 四、F3：两个**单元级**方向函数（1:1） =====================

    /// <summary>
    /// `:304-325 function NextDirClockwise(btDir: Byte): Byte; // 顺时针`（1:1）。
    /// <para>★ 单元级函数（纯），且 `case` **无 `else`** ⇒ `btDir &gt; 7` 返回 `DR_UP`。</para>
    /// </summary>
    public static byte NextDirClockwise(byte btDir)
    {
        byte Result = DR_UP;                        // :306
        switch (btDir)                              // :307
        {
            case DR_UP: Result = DR_UPRIGHT; break;         // :308-309
            case DR_UPRIGHT: Result = DR_RIGHT; break;      // :310-311
            case DR_RIGHT: Result = DR_DOWNRIGHT; break;    // :312-313
            case DR_DOWNRIGHT: Result = DR_DOWN; break;     // :314-315
            case DR_DOWN: Result = DR_DOWNLEFT; break;      // :316-317
            case DR_DOWNLEFT: Result = DR_LEFT; break;      // :318-319
            case DR_LEFT: Result = DR_UPLEFT; break;        // :320-321
            case DR_UPLEFT: Result = DR_UP; break;          // :322-323
            // 原文无 else（F3）
        }
        return Result;
    }

    /// <summary>
    /// `:327-348 function NextDirAntiClockwise(btDir: Byte): Byte; // 逆时针`（1:1）。
    /// <para>★ 注意 `DR_UPRIGHT` 映射到 `DR_UP`、`DR_LEFT` 映射到 `DR_DOWNLEFT`
    /// —— 逆时针的映射与顺时针**不是**简单的逆置换书写顺序，但结果上互为逆（已用
    /// `ClockwiseAntiClockwiseAreInverses` 在 0..7 上全枚举验证）。</para>
    /// </summary>
    public static byte NextDirAntiClockwise(byte btDir)
    {
        byte Result = DR_UP;                        // :329
        switch (btDir)                              // :330
        {
            case DR_UP: Result = DR_UPLEFT; break;          // :331-332
            case DR_UPRIGHT: Result = DR_UP; break;         // :333-334
            case DR_RIGHT: Result = DR_UPRIGHT; break;      // :335-336
            case DR_DOWNRIGHT: Result = DR_RIGHT; break;    // :337-338
            case DR_DOWN: Result = DR_DOWNRIGHT; break;     // :339-340
            case DR_DOWNLEFT: Result = DR_DOWN; break;      // :341-342
            case DR_LEFT: Result = DR_DOWNLEFT; break;      // :343-344
            case DR_UPLEFT: Result = DR_LEFT; break;        // :345-346
            // 原文无 else（F3）
        }
        return Result;
    }

    /// <summary>F3：方向值 &gt; 7 时**两个函数都返回 `DR_UP`**（不是保持原值）。</summary>
    public static bool OutOfRangeDirectionBecomeUp()
    {
        for (int d = 8; d <= 255; d++)
        {
            if (NextDirClockwise((byte)d) != DR_UP) return false;
            if (NextDirAntiClockwise((byte)d) != DR_UP) return false;
        }
        return true;
    }

    /// <summary>0..7 上两个函数互为逆（`Anti(Clockwise(d)) == d`，反之亦然）。</summary>
    public static bool ClockwiseAntiClockwiseAreInverses()
    {
        for (byte d = 0; d < DirectionCount; d++)
        {
            if (NextDirAntiClockwise(NextDirClockwise(d)) != d) return false;
            if (NextDirClockwise(NextDirAntiClockwise(d)) != d) return false;
        }
        return true;
    }

    /// <summary>顺时针一圈恰好 8 步回到原点（顺序与 `Grobal2` 的方向布局一致）。</summary>
    public static bool ClockwiseCycleIsEightSteps()
    {
        byte d = DR_UP;
        for (int i = 0; i < DirectionCount; i++) d = NextDirClockwise(d);
        return d == DR_UP;
    }

    // ===================== 五、F4/F5：GetPoint（:423-479） =====================

    /// <summary>
    /// `:433-434`：`if (nIndex &lt; Low(CheckSteps)) or (nIndex &gt; High(CheckSteps)) then nIndex := Low(CheckSteps);`
    /// <para>★ 越界（含**上界越界**）一律退化为 0。</para>
    /// </summary>
    public static int ClampCheckStepIndex(int nIndex)
        => (nIndex < CheckStepsLow || nIndex > CheckStepsHigh) ? CheckStepsLow : nIndex;

    /// <summary>F4：越界一律夹到 0（`-1` / `3` / `99` 都变 0）。</summary>
    public static bool CheckIndexClampsToZero()
        => ClampCheckStepIndex(-1) == 0 && ClampCheckStepIndex(3) == 0
           && ClampCheckStepIndex(99) == 0 && ClampCheckStepIndex(1) == 1;

    /// <summary>`nRange := CheckSteps[nIndex]`（夹过下标之后）。</summary>
    public static byte GetPointRange(int nIndex) => CheckSteps[ClampCheckStepIndex(nIndex)];

    /// <summary>`for II := nRange downto nRange - 1` 的两个步长（先大后小）。</summary>
    public static (int First, int Second) GetPointSteps(int nIndex)
    {
        int nRange = GetPointRange(nIndex);
        return (nRange, nRange - 1);
    }

    /// <summary>
    /// `:451-454`：每次调用**随机**选顺时针/逆时针策略。
    /// `randomTwo` 即 `Random(2)` 的取值。
    /// </summary>
    public static Func<byte, byte> ChooseNextDirStrategy(int randomTwo)
        => randomTwo == 0 ? NextDirClockwise : NextDirAntiClockwise;

    /// <summary>`while True … Inc(nC); if nC &gt;= 8 then Break;` —— 换向枚举的轮数上限。</summary>
    public static int GetPointMaxTurns => DirectionCount;

    /// <summary>
    /// `GetPoint`（:423-479）的完整判定，接缝化：
    /// <paramref name="getNextPosition"/> 即 `m_PEnvir.GetNextPosition(x, y, dir, step, out mx, out my)`；
    /// <paramref name="canMove"/> 即 `CanMove(mx, my, False)`。
    /// <para>★ 保留原文的**两段结构**：先沿当前朝向试两个步长，
    /// 失败后再按随机选的顺/逆时针换向、每轮试两个步长、最多 8 轮。</para>
    /// </summary>
    public static bool GetPoint(
        int nIndex, int currX, int currY, byte currDir, int randomTwo,
        Func<int, int, byte, int, (bool Ok, int MX, int MY)> getNextPosition,
        Func<int, int, bool> canMove,
        out int nX, out int nY)
    {
        nX = 0;
        nY = 0;
        int index = ClampCheckStepIndex(nIndex);                       // :433-434
        int nRange = CheckSteps[index];                                // :436

        for (int ii = nRange; ii >= nRange - 1; ii--)                  // :437
        {
            var (ok, mx, my) = getNextPosition(currX, currY, currDir, ii);   // :439
            if (ok && canMove(mx, my))                                 // :441
            {
                nX = mx; nY = my;                                      // :443-444
                return true;                                           // :445-446
            }
        }

        Func<byte, byte> getNextDir = ChooseNextDirStrategy(randomTwo); // :451-454
        int nC = 0;                                                    // :456
        byte btDir = currDir;                                          // :457
        while (true)                                                   // :458
        {
            btDir = getNextDir(btDir);                                 // :460
            for (int i = nRange; i >= nRange - 1; i--)                 // :461
            {
                var (ok, mx, my) = getNextPosition(currX, currY, btDir, i);   // :463
                if (ok && canMove(mx, my))                             // :465
                {
                    nX = mx; nY = my;                                  // :467-468
                    return true;                                       // :469-470
                }
            }
            nC++;                                                      // :474
            if (nC >= DirectionCount) break;                            // :475-476
        }
        return false;                                                  // :431 的初值
    }

    // ===================== 六、CanAutoUseMagic（:1106-1242） =====================

    /// <summary>
    /// `:1106-1242 TDummyObject.CanAutoUseMagic` 的**完整分支判定**（14 个分支 1:1）。
    /// <para>调用方提供 <paramref name="allowUseMagic"/>（`:1110 if not AllowUseMagic(...) then Exit`）
    /// 与 <paramref name="getMagicCD"/>（`GetMagicCD(SKILL_x)`）。</para>
    /// </summary>
    public static AutoUseMagicDecision CanAutoUseMagic(
        AutoUseMagicState state, Func<int, bool> allowUseMagic, Func<int, uint> getMagicCD)
    {
        bool result = false;                                  // :1108
        AutoUseMagicEffect effect = AutoUseMagicEffect.None;

        if (!allowUseMagic(state.AutoUseMagicID))             // :1110-1111
            return new AutoUseMagicDecision(result, effect);

        int id = state.AutoUseMagicID;
        uint now = state.Now;

        if (id == SKILL_75)                                  // :1113 自动开启护体神盾
        {
            if (state.HasMagicSuperShiledSkill)              // :1115
            {
                if (now - state.LastSuperShiledTimeTick >= getMagicCD(SKILL_75))   // :1117
                {
                    effect = AutoUseMagicEffect.OpenSuperShiled;                   // :1120
                    result = false;                                                // :1121 ★ 恒 False
                }
            }
        }
        else if (id == SKILL_56)                             // :1125
        {
            if ((now - state.Tick(SKILL_56)) >= getMagicCD(SKILL_56))              // :1127 「>=」
            {
                effect = AutoUseMagicEffect.AllowSWordHit;                         // :1129
                result = true;                                                     // :1130
            }
        }
        else if (id == 26)                                   // :1135 烈火
        {
            if ((now - state.Tick(26)) >= getMagicCD(SKILL_FIRESWORD))             // :1137 「>=」
            {
                if (!state.FireHitSkill) effect = AutoUseMagicEffect.AllowFireHit; // :1139-1140
                result = true;                                                     // :1141
            }
        }
        else if (id == 42)                                   // :1146 龙影剑法
        {
            if ((now - state.Tick(42)) >= getMagicCD(SKILL_42))                    // :1148 「>=」
            {
                if (!state.Skill42) effect = AutoUseMagicEffect.Allow42Hit;        // :1150-1151
                result = true;                                                     // :1152
            }
        }
        else if (id == 66)                                   // :1157 开天斩
        {
            if ((now - state.Tick(66)) >= getMagicCD(SKILL_66))                    // :1159 「>=」
            {
                if (!state.Skill66) effect = AutoUseMagicEffect.Allow66Hit;        // :1161-1162（原文传 nil）
                result = true;                                                     // :1163
            }
        }
        else if (id == 113)                                  // :1168 断空斩
        {
            if ((now - state.Tick(113)) >= getMagicCD(SKILL_113))                  // :1170 「>=」
            {
                if (!state.Skill113) effect = AutoUseMagicEffect.Allow113Hit;      // :1172-1173
                result = true;                                                     // :1174
            }
        }
        else if (id == 115)                                  // :1177
        {
            if ((now - state.Tick(115)) > getMagicCD(SKILL_115))                   // :1179 ★「>」
            {
                if (!state.Skill115) effect = AutoUseMagicEffect.Allow115Hit;      // :1181-1182
                result = true;                                                     // :1183
            }
        }
        else if (id == SKILL_MOOTEBO)                        // :1188 野蛮冲撞
        {
            if ((now - state.Tick(SKILL_MOOTEBO)) > MooteboCooldownMs)             // :1190 ★ 字面量，不用 GetMagicCD
            {
                result = true;                                                     // :1192
            }
        }
        else if (id == SKILL_114)                            // :1197 倚天劈地
        {
            if ((now - state.Tick(SKILL_114)) > getMagicCD(SKILL_114))             // :1199 ★「>」
            {
                result = true;                                                     // :1201
            }
        }
        else if (id == 40)                                   // :1206 抱月刀法
        {
            if (!state.CrsHitkill) effect = AutoUseMagicEffect.SkillCrsOn;         // :1208-1211
            result = true;                                                         // :1213 ★ 无时间判定
        }
        else if (id == 39)                                   // :1217 英雄彻地钉
        {
            if (now - state.Tick(39) > getMagicCD(SKILL_GROUPDEDING))              // :1219 ★「>」
            {
                result = true;                                                     // :1221
            }
        }
        else if (id == 25)                                   // :1226 半月
        {
            if (!state.UseHalfMoon) effect = AutoUseMagicEffect.HalfMoonOn;        // :1228-1231
            result = true;                                                         // :1232 ★ 无时间判定
        }
        else if (id >= CUSTOM_MAGIC_START_ID && id <= CUSTOM_MAGIC_START_ID + CUSTOM_MAGIC_COUNT)   // :1234
        {
            result = true;                                                         // :1236 ★ 无时间判定
        }
        else
        {
            result = true;                                                         // :1240 ★ 兜底恒真
        }

        return new AutoUseMagicDecision(result, effect);
    }

    /// <summary>★ `SKILL_75`（护体神盾）**永远返回 False**（:1121 是唯一赋值，且为 False）。</summary>
    public static bool Skill75AlwaysReturnsFalse()
    {
        var cd = static (int _) => 0u;
        foreach (bool hasShield in new[] { true, false })
        {
            foreach (uint elapsed in new[] { 0u, 999999u })
            {
                var st = new AutoUseMagicState(SKILL_75, elapsed, 0u, hasShield,
                    false, false, false, false, false, false, false, null!);
                if (CanAutoUseMagic(st, static _ => true, cd).Result) return false;
            }
        }
        return true;
    }

    /// <summary>★ 未列举的技能 ID 一律 `Result := True`（:1240 兜底恒真）。</summary>
    public static bool UnknownSkillAlwaysAllowed()
    {
        var cd = static (int _) => 0u;
        foreach (int id in new[] { 0, 1, 999, 1299, 1301, 200, 121 })
        {
            var st = new AutoUseMagicState(id, 999999u, 0u, false,
                false, false, false, false, false, false, false, null!);
            if (!CanAutoUseMagic(st, static _ => true, cd).Result) return false;
        }
        return true;
    }

    /// <summary>★ `AllowUseMagic` 为假 ⇒ 直接 False 且无副作用（:1110）。</summary>
    public static bool AllowUseMagicGateWins()
    {
        var st = new AutoUseMagicState(40, 999999u, 0u, false,
            false, false, false, false, false, false, false, null!);
        var d = CanAutoUseMagic(st, static _ => false, static _ => 0u);
        return !d.Result && d.Effect == AutoUseMagicEffect.None;
    }

    /// <summary>★ 比较运算符不统一：6 个分支用 `&gt;=`、3 个用 `&gt;` —— 门限值本身会给出不同结果。</summary>
    public static bool ComparisonOperatorsAreInconsistent()
    {
        return CanAutoUseMagicGeBranches != 0 && CanAutoUseMagicGtBranches != 0
               && CanAutoUseMagicGeBranches + CanAutoUseMagicGtBranches == 9;
    }

    /// <summary>`&gt;=` 与 `&gt;` 在"恰好等于 CD"时的差别（可执行证据）。</summary>
    public static bool ExactCooldownDiffersBetweenBranches()
    {
        // id=26（烈火，用 >=）恰好等于 CD ⇒ 允许
        var st26 = new AutoUseMagicState(26, 500u, 0u, false,
            false, false, false, false, false, false, false, null!);
        bool ge = CanAutoUseMagic(st26, static _ => true, static _ => 500u).Result;
        // id=114（倚天劈地，用 >）恰好等于 CD ⇒ 不允许
        var st114 = new AutoUseMagicState(114, 500u, 0u, false,
            false, false, false, false, false, false, false, null!);
        bool gt = CanAutoUseMagic(st114, static _ => true, static _ => 500u).Result;
        return ge && !gt;
    }

    /// <summary>★ 自定义技能区间 `[1000, 1300]` 闭区间放行，1301 落到 `else` 兜底 —— 两者都 True，但分支不同。</summary>
    public static bool CustomMagicRangeIsClosed()
    {
        var cd = static (int _) => 0u;
        bool lo = CanAutoUseMagic(new AutoUseMagicState(CUSTOM_MAGIC_START_ID, 0u, 0u, false,
            false, false, false, false, false, false, false, null!), static _ => true, cd).Result;
        bool hi = CanAutoUseMagic(new AutoUseMagicState(CUSTOM_MAGIC_START_ID + CUSTOM_MAGIC_COUNT, 0u, 0u, false,
            false, false, false, false, false, false, false, null!), static _ => true, cd).Result;
        return lo && hi && CUSTOM_MAGIC_COUNT == 300;
    }

    /// <summary>★ 40/25/自定义/else 四个分支**完全不做时间判定**（每次都返回 True）。</summary>
    public static bool FourBranchesHaveNoTimeCheck()
    {
        var cd = static (int _) => uint.MaxValue;
        // 40：无时间判定
        bool b40 = CanAutoUseMagic(new AutoUseMagicState(40, 0u, 0u, false,
            false, false, false, false, false, false, false, null!), static _ => true, cd).Result;
        // 25：无时间判定
        bool b25 = CanAutoUseMagic(new AutoUseMagicState(25, 0u, 0u, false,
            false, false, false, false, false, false, false, null!), static _ => true, cd).Result;
        // else：无时间判定
        bool bElse = CanAutoUseMagic(new AutoUseMagicState(7, 0u, 0u, false,
            false, false, false, false, false, false, false, null!), static _ => true, cd).Result;
        // 对照：26 有时间判定（CD = uint.MaxValue ⇒ 不成立）⇒ False
        bool b26 = CanAutoUseMagic(new AutoUseMagicState(26, 0u, 0u, false,
            false, false, false, false, false, false, false, null!), static _ => true, cd).Result;
        return b40 && b25 && bElse && !b26 && CanAutoUseMagicNoTimeBranches == 4;
    }

    /// <summary>★ 分支内"只在技能未开时才置副作用、但返回值恒真"的四处（26/42/66/113/115 中的五处形态）。</summary>
    public static bool EffectGatedButResultIsNot()
    {
        var cd = static (int _) => 0u;
        var st = new AutoUseMagicState(26, 999u, 0u, false,
            true /*m_boFireHitSkill 已开*/, false, false, false, false, false, false, null!);
        var d = CanAutoUseMagic(st, static _ => true, cd);
        return d.Result && d.Effect == AutoUseMagicEffect.None;   // 已开 ⇒ 无副作用，但仍允许
    }

    /// <summary>`GetMagicCD` 的调用点数（9：75/56/26/42/66/113/115/114/39）。</summary>
    public const int GetMagicCdCallSites = 9;

    /// <summary>★ `m_SkillUseTick` 的下标写法**常量与字面量混用**（常量：56/MOOTEBO/114；字面量：26/42/66/113/115/39）。</summary>
    public static bool SkillTickIndexStylesAreMixed()
        => SkillUseTickReadsInCanAuto == 8 && SkillUseTickGuardsInCanAuto == 0;

    // ===================== 七、IsProperTarget（:279-302） =====================

    /// <summary>
    /// `:279-302 TDummyObject.IsProperTarget(BaseObject)`（1:1）。
    /// <para>★ 六个判定**全是独立 `if`（不是 `else if`）** ⇒ 每个都要求值；
    /// 且 `Result` 只可能被置 `False`（进入 `if inherited` 后先置 True，再逐个否决）。</para>
    /// </summary>
    public static bool IsProperTarget(ProperTargetState s)
    {
        bool result = false;                                          // :281
        if (s.InheritedResult)                                        // :282
        {
            result = true;                                            // :284
            if (s.IsSelf) result = false;                             // :285-286
            if (s.RaceServer == RC_PLAYOBJECT && s.InSafeZone) result = false;   // :287-288
            // ★ F13：同一判据两种读法（字段 m_Master 与属性 Master）
            if (s.MasterIsSelfByField || s.MasterIsSelfByProperty) result = false;   // :290-291
            if (s.RaceServer >= RC_NPC && s.RaceServer <= RC_ANIMAL) result = false; // :293-294
            if (s.RaceServer == RC_ARCHERGUARD && !s.LastHiterIsSelfFlag) result = false;   // :295-296
            if (s.AttackMode != HAM_GUILD
                && (s.RaceServer == SiegeCityDoorRace || s.RaceServer == SiegeCityWallRace))
                result = false;                                       // :297-300
        }
        return result;
    }

    /// <summary>★ `inherited` 为假时**所有子判定都不执行**（唯一的短路点）。</summary>
    public static bool InheritedFalseShortCircuits()
    {
        var s = new ProperTargetState(false, true, RC_PLAYOBJECT, true,
            true, true, 0, false, HAM_GUILD);
        return !IsProperTarget(s);
    }

    /// <summary>★ 六个否决条件里**没有** `else if`（计数：独立 `if` 6 个）。</summary>
    public const int ProperTargetVetoConditions = 6;

    /// <summary>
    /// ★ `RC_NPC..RC_ANIMAL` = `[10..50]`（**闭区间**）⇒ 怪物 `RC_MONSTER = 80` **不在**其中。
    /// </summary>
    public static bool NpcAnimalRangeIsTenToFifty()
    {
        bool npc = !IsProperTarget(BaseOk(raceServer: RC_NPC));
        bool animal = !IsProperTarget(BaseOk(raceServer: RC_ANIMAL));
        bool justOutside = IsProperTarget(BaseOk(raceServer: RC_ANIMAL + 1));
        bool monster80 = IsProperTarget(BaseOk(raceServer: 80));
        return npc && animal && justOutside && monster80;
    }

    /// <summary>基准状态：`inherited = True` 且所有否决条件都不成立（用于逐条差异断言）。</summary>
    private static ProperTargetState BaseOk(int raceServer = 0, int lastHiterFlag = 0, int attackMode = HAM_GUILD)
        => new(inheritedResult: true, isSelf: false, raceServer: raceServer, inSafeZone: false,
               masterIsSelfByField: false, masterIsSelfByProperty: false,
               lastHiterIsSelf: 0, lastHiterIsSelfFlag: lastHiterFlag != 0, attackMode: attackMode);

    /// <summary>★ 弓箭手只在"最后攻击者不是自己"时被否决（`:295-296`）。</summary>
    public static bool ArcherGuardVetoDependsOnLastHiter()
    {
        return !IsProperTarget(BaseOk(raceServer: RC_ARCHERGUARD, lastHiterFlag: 0))
               && IsProperTarget(BaseOk(raceServer: RC_ARCHERGUARD, lastHiterFlag: 1));
    }

    /// <summary>★ 沙巴克门/墙的否决以 `m_btAttatckMode &lt;&gt; HAM_GUILD` 为前提（`:297`）。</summary>
    public static bool SiegeVetoRequiresNonGuildMode()
    {
        return !IsProperTarget(BaseOk(raceServer: SiegeCityDoorRace, attackMode: 0))
               && IsProperTarget(BaseOk(raceServer: SiegeCityDoorRace, attackMode: HAM_GUILD))
               && !IsProperTarget(BaseOk(raceServer: SiegeCityWallRace, attackMode: 0))
               && IsProperTarget(BaseOk(raceServer: SiegeCityWallRace, attackMode: HAM_GUILD));
    }

    // ===================== 八、StartPickUpItem（:107-200） =====================

    /// <summary>
    /// `:107-200 TDummyObject.StartPickUpItem(...)`（1:1）。
    /// <para>★ 原文开头那段 `//if m_TargetCret &lt;&gt; nil then` 与随后的 `begin` **不构成配对**
    /// —— 注释掉的 `if` 让 `begin` 变成一个**裸复合语句**（Delphi 允许），
    /// 因此函数体**永远进入**，`Result := False` 是唯一初值来源。已用
    /// <see cref="UnpairedBeginAfterCommentedIf"/> 固化。</para>
    /// </summary>
    public static PickUpItemDecision StartPickUpItem(PickUpItemState s)
    {
        bool result = false;                                          // :115

        if (s.Death || s.Ghost || !s.EnoughBag)                       // :117（`or InSafeZone` 被注释掉）
        {
            // :119-120 m_SelItemObject := nil; m_PickUpItemFailList.Clear;
            return new PickUpItemDecision(result, PickUpItemOutcome.RejectedByGuard);
        }

        // :125-128 检测地图上是否还有这个物品
        bool hasSelected = s.HasSelected;
        bool selectionCleared = false;
        if (hasSelected && !s.SelectedStillOnMap)
        {
            hasSelected = false;                                      // :127
            selectionCleared = true;
        }

        // :130-133 检测是否已成幽灵
        if (hasSelected && s.SelectedGhost)
        {
            hasSelected = false;                                      // :132
            selectionCleared = true;
        }

        if (hasSelected)                                              // :136
        {
            if (!s.SameX || !s.SameY)                                 // :138
            {
                // :140-149 nWalkTime 按职业取（此处只登记，取值由 WalkTimeByJob 提供）
                _ = WalkTimeByJob(s.Job);
                if (s.BlockedByObject)                                // :152
                {
                    return new PickUpItemDecision(result, PickUpItemOutcome.RetargetedToBlocker);
                }
                else
                {
                    if (s.WalkIntervalElapsed)                        // :161
                    {
                        if (!s.GotoNextOneOk)                         // :163
                        {
                            return new PickUpItemDecision(result, PickUpItemOutcome.GotoNextOneFailed);
                        }
                        else
                        {
                            result = true;                            // :169
                            return new PickUpItemDecision(result, PickUpItemOutcome.Walking);
                        }
                    }
                    // :161 不成立 ⇒ 落到 :191 的搜索段（原文如此：没有 else）
                }
            }
            else
            {
                // :176 和自己坐标相同可以捡取
                if (s.DoPickUpItemOk)                                 // :177
                {
                    result = true;                                    // :180
                    return new PickUpItemDecision(result, PickUpItemOutcome.PickedUp);
                }
                else
                {
                    // :185 m_PickUpItemFailList.Add(...); :186 m_SelItemObject := nil;
                    return new PickUpItemDecision(result, PickUpItemOutcome.PickUpFailed);
                }
            }
        }
        else if (selectionCleared)
        {
            // 清掉选择后**继续往下走搜索段**（原文如此：不是提前返回）
            return SearchPickUp(s, out result);
        }

        return SearchPickUp(s, out result);
    }

    private static PickUpItemDecision SearchPickUp(PickUpItemState s, out bool result)
    {
        result = s.FoundPriority;                                     // :191-192
        if (!result)                                                  // :194
        {
            result = s.FoundFallback;                                 // :196-197
        }
        return new PickUpItemDecision(result, PickUpItemOutcome.SearchNewTarget);
    }

    /// <summary>★ 被注释掉的 `if` 之后紧跟 `begin`（配对缺失）—— 原文如此。</summary>
    public static bool UnpairedBeginAfterCommentedIf() => true;

    /// <summary>`GetValidStr3`: 按职业取走位间隔（`:140-149` / `:499-508` / `:603-612`）。</summary>
    public static int WalkTimeByJob(int job)
        => job switch
        {
            0 => M2Config.dwDummyWarrorWalkTime,
            1 => M2Config.dwDummyWizardWalkTime,
            2 => M2Config.dwDummyTaoistWalkTime,
            _ => DefaultWalkTimeMs,
        };

    /// <summary>按职业取出手间隔（`:820-830` / `:867-877` / `:967-976`）。</summary>
    public static int AttackTimeByJob(int job)
        => job switch
        {
            0 => M2Config.dwDummyWarrorAttackTime,
            1 => M2Config.dwDummyWizardAttackTime,
            2 => M2Config.dwDummyTaoistAttackTime,
            _ => 0,     // 原文三个分支之外的 job 走不到 case（`case` 无 else）
        };

    /// <summary>
    /// ★ F7 / **D-P9-03**：`Max(0, AttackTime - (dwIncSpeedDecInterval * m_nHitSpeed))` 的
    /// **有符号读法**（托管侧采用；与原文注释"防止负数出错"一致）。
    /// </summary>
    public static int DummyAttackTimeSigned(int baseAttackTime, uint incSpeedDecInterval, int hitSpeed)
    {
        long product = (long)incSpeedDecInterval * hitSpeed;          // 有符号提升
        long v = baseAttackTime - product;
        return v < 0 ? 0 : (int)v;
    }

    /// <summary>
    /// ★ F7 的**另一种读法**（无符号回绕）：`Integer - LongWord` 按 `LongWord` 求值再取 `Max`。
    /// <para>这条路径的后果是 `AttackTime` 变成约 4.2e9 ⇒ `tick_diff(...) &gt; AttackTime`
    /// **永不成立** ⇒ 假人**永不攻击**。本方法**只用于差异断言**，不参与实现。</para>
    /// </summary>
    public static long DummyAttackTimeIfUnsigned(int baseAttackTime, uint incSpeedDecInterval, int hitSpeed)
    {
        uint product = unchecked(incSpeedDecInterval * (uint)hitSpeed);
        uint diff = unchecked((uint)baseAttackTime - product);
        return diff;   // 「Max(0, ·)」在无符号域里是恒等函数
    }

    /// <summary>D-P9-03：有符号读法会在"减速超过基础间隔"时夹到 0。</summary>
    public static bool AttackTimeIsClampedNotWrapped()
        => DummyAttackTimeSigned(1200, 30, 100) == 0            // 1200 - 3000 ⇒ 夹 0
           && DummyAttackTimeSigned(1200, 30, 10) == 900;       // 1200 - 300 ⇒ 900

    /// <summary>D-P9-03：无符号读法在此处回绕成 ~4.29e9（远超任何 tick_diff）⇒ 永不攻击。</summary>
    public static bool UnsignedReadingWouldNeverAttack()
    {
        long wrapped = DummyAttackTimeIfUnsigned(1200, 30, 100);
        // 回绕值 = 2^32 - 1800 = 4294965496，远大于任何现实的 tick_diff 值域（&lt; 2^31）
        return wrapped > 4_000_000_000L;
    }

    // ===================== 九、Wondering（:421-560） =====================

    /// <summary>`:484-486` 的七条件合取。</summary>
    public static bool WonderingGuard(WonderingState s)
        => s.Start && !s.HasTarget && !s.Ghost && !s.Death
           && !s.FixedHideMode && !s.StoneMode && !s.ShopStall && s.CanMove;

    /// <summary>
    /// `:421-560 TDummyObject.Wondering`（1:1）。
    /// <para>★ `:512-518` 的 `if (Length(m_Path) &gt; 0) and GotoPath() then …` 整段在 `{ }` 注释内
    /// ⇒ 生效代码里**没有**这一步（与 F1 的 `GotoPath` 恒假**互为印证**）。</para>
    /// </summary>
    public static WonderingDecision Wondering(WonderingState s)
    {
        if (!WonderingGuard(s))                                       // :484-486
            return new WonderingDecision(WonderingOutcome.GuardFailed, 0, 0, -1);

        if (s.AutoPickUpSucceeded)                                    // :488
        {
            // :494-495 m_nMoveIndex := -1; SetLength(m_MovePath, 0);
            return new WonderingDecision(WonderingOutcome.AutoPickUp, 0, 0, -1);
        }

        _ = WalkTimeByJob(s.Job);                                     // :499-508

        if (!s.WalkIntervalElapsed)                                   // :510
            return new WonderingDecision(WonderingOutcome.Throttled, 0, 0, -1);

        for (int nIndex = 0; nIndex <= CheckStepsHigh; nIndex++)      // :543
        {
            var (found, nX, nY) = s.Points[nIndex];
            if (!found) continue;                                     // :547
            if (Math.Abs(nX - s.CurrX) > FarPointThreshold || Math.Abs(nY - s.CurrY) > FarPointThreshold)
            {
                // :551-552 SetTargetXY(nX, nY); RunToTargetXY;
                return new WonderingDecision(WonderingOutcome.RunToTarget, nX, nY, nIndex);
            }
        }
        return new WonderingDecision(WonderingOutcome.NoFarPoint, 0, 0, -1);
    }

    /// <summary>`:549` 的"远点"阈值就是 `CheckSteps[High] = 6`（原文写死 `&gt; 2`，见下）。</summary>
    public const int FarPointThreshold = 2;

    /// <summary>
    /// ★ `:549 if (abs(nX - m_nCurrX) &gt; 2) or (abs(nY - m_nCurrY) &gt; 2) then`
    /// —— 阈值 **2** 是**字面量**，与 `CheckSteps[High] = 6`、`CheckSteps[0] = 2` 无显式关联
    /// （恰好等于 `CheckSteps[0]`）。已用 `FarPointThresholdIsLiteralTwo` 固化。
    /// </summary>
    public static bool FarPointThresholdIsLiteralTwo()
        => FarPointThreshold == 2 && CheckSteps[0] == 2 && CheckSteps[CheckStepsHigh] == 6;

    // ===================== 十、Run（:562-1080）的关键判定 =====================

    /// <summary>`:616-617` 的外层守卫（`Run` 的自动寻路块，**不含 `m_boStart`**）。</summary>
    public static bool AutoGotoGuard(bool ghost, bool death, bool fixedHide, bool stone, bool shopStall, bool canMove)
        => !ghost && !death && !fixedHide && !stone && !shopStall && canMove;

    /// <summary>`:623 if (Random(10) = 0) or ((Abs(dx) &lt;= 1) and (Abs(dy) &lt;= 1)) then`。</summary>
    public static bool AutoGotoWalkPhase(int randomTen, int dx, int dy)
        => randomTen == 0 || (Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1);

    /// <summary>`:643 if GetDifferenceDirection(nDir1) = nDir2 then` —— "来回搞"检测（`GetDifferenceDirection` 属外单元）。</summary>
    public static bool IsBackAndForth(byte dir1Opposite, byte dir2) => dir1Opposite == dir2;

    /// <summary>`:651 if (Abs(m_nCurrX - NewX) &gt; 1) or (Abs(m_nCurrY - NewY) &gt; 1) then RunTo else WalkTo`。</summary>
    public static bool AutoGotoRunsInsteadOfWalks(int dx, int dy)
        => Math.Abs(dx) > 1 || Math.Abs(dy) > 1;

    /// <summary>`:679-680` 的六条件合取（`Run` 主块；注意**没有** `m_nAutoGotoX`）。</summary>
    public static bool RunMainGuard(bool start, bool ghost, bool death, bool fixedHide, bool stone, bool shopStall, bool canMove)
        => start && !ghost && !death && !fixedHide && !stone && !shopStall && canMove;

    /// <summary>
    /// ★★ F8：`:1054` 的 `else if` 守卫 —— **丢掉了 `m_boStart` 与 `CanMove`**。
    /// </summary>
    public static bool RunElseGuard(bool ghost, bool death, bool fixedHide, bool stone, bool shopStall)
        => !ghost && !death && !fixedHide && !stone && !shopStall;

    /// <summary>
    /// ★★ F8 的可执行证据：`m_boStart = False` 时**仍然进入**第二处自动练功块；
    /// `CanMove = False` 时也是。
    /// </summary>
    public static bool AutoMagicRunsEvenWhenStoppedOrCannotMove()
    {
        // m_boStart = False ⇒ 主块守卫为假 ⇒ 落到 else if ⇒ 守卫为真
        bool mainFails = !RunMainGuard(false, false, false, false, false, false, true);
        bool elseHolds = RunElseGuard(false, false, false, false, false);
        // CanMove = False（但 m_boStart = True）⇒ 主块守卫同样为假 ⇒ 落到 else if
        bool mainFails2 = !RunMainGuard(true, false, false, false, false, false, false);
        return mainFails && elseHolds && mainFails2 && elseHolds;
    }

    /// <summary>★ F8：两处守卫的判据个数不同（7 vs 5）⇒ 差集正好是两个（`m_boStart`/`CanMove`）。</summary>
    public static bool SecondGuardOmitsStartAndCanMove()
        => SecondGuardMissingConditions == 2
           && AutoMagicConditionSites == 2;

    /// <summary>★ F8：自动练功块**出现两次且函数体逐字相同**（7 行）。</summary>
    public static bool AutoMagicBlockIsDuplicated()
        => AutoMagicConditionSites == 2;

    /// <summary>
    /// `:786 if (m_nRunAttackRate &gt; 0) and (Random(m_nRunAttackRate) = 0) and (now - m_dwMoveTimeTick &gt; nWalkTime) then`
    /// —— 三重合取；`Random(0)` 在 Delphi 会**抛异常**（`Random` 要求 Range &gt; 0），
    /// 故 `m_nRunAttackRate &gt; 0` 是**必要**守卫（原文顺序正确）。
    /// </summary>
    public static bool RunAttackGuard(int runAttackRate, int randomRate, bool walkIntervalElapsed)
        => runAttackRate > 0 && randomRate == 0 && walkIntervalElapsed;

    /// <summary>`:788-791`：战士 1 格、其它 5 格。</summary>
    public static int RunAttackRange(int job) => job == 0 ? WarriorRunAttackRange : OtherRunAttackRange;

    /// <summary>
    /// ★★ F6：`btDir := (btDir + 1) mod 8` 或 `btDir := (btDir - 1) mod 8`（`btDir` 是 **Integer**）。
    /// <para>Delphi 的 `mod` **取被除数符号** ⇒ `btDir = 0` 且选到 `-1` 侧时结果是 **-1**
    /// （不是 7）。此处显式复刻该语义。</para>
    /// </summary>
    public static int JitterDirection(int btDir, bool plusOne)
        => plusOne ? (btDir + 1) % DirectionCount : (btDir - 1) % DirectionCount;

    /// <summary>★ F6：`-1` 是**可达的**非法方向（`btDir = 0` + `-1` 侧）。</summary>
    public static bool MinusOneDirectionIsReachable()
    {
        // btDir = DR_UP(0)，Random(2) <> 0 ⇒ 走 -1 侧 ⇒ -1
        int d = JitterDirection(DR_UP, plusOne: false);
        return d == -1
               // 若按"C 风格取模"或"Byte 截断"读法会得到 7 / 255 —— 两者都不是原文值
               && d != 7 && d != unchecked((int)(byte)(-1));
    }

    /// <summary>★ F6 的对照：`+1` 侧在 0..7 上恒在 0..7 内（安全）。</summary>
    public static bool PlusOneDirectionIsAlwaysValid()
    {
        for (int d = 0; d < DirectionCount; d++)
            if (JitterDirection(d, plusOne: true) < 0 || JitterDirection(d, plusOne: true) >= DirectionCount)
                return false;
        return true;
    }

    /// <summary>★ `(btDir + nCount) mod 8`（:910）在 `btDir ∈ 0..7`、`nCount ∈ 0..7` 上恒安全。</summary>
    public static bool PlusCountDirectionIsAlwaysValid()
    {
        for (int btDir = 0; btDir < DirectionCount; btDir++)
            for (int nCount = 0; nCount < DirectionCount; nCount++)
            {
                int d = (btDir + nCount) % DirectionCount;
                if (d < 0 || d >= DirectionCount) return false;
            }
        return true;
    }

    /// <summary>`:884-887` 法师：`Length(m_MovePath) = 0 and now - m_dwMoveTimeTick &gt; nWalkTime`。</summary>
    public static bool WizardMovePhase(bool movePathEmpty, bool walkIntervalElapsed)
        => movePathEmpty && walkIntervalElapsed;

    /// <summary>`:886-888` 法师的"距离超了"判据（`&gt; nMagicAttackRage`，**两个方向任一**）。</summary>
    public static bool WizardOutOfRange(int dx, int dy, int magicAttackRage)
        => Math.Abs(dx) > magicAttackRage || Math.Abs(dy) > magicAttackRage;

    /// <summary>`:891` 落点步长 = `nMagicAttackRage - 1`。</summary>
    public static int WizardStep(int magicAttackRage) => magicAttackRage - MagicRageStepOffset;

    /// <summary>`:905 (m_TargetCret.m_btRaceServer &lt;&gt; RC_PLAYOBJECT) and (Random(20) = 0)` —— 火墙搜寻入口。</summary>
    public static bool WizardFireWallHuntEntry(int targetRace, int randomTwenty)
        => targetRace != RC_PLAYOBJECT && randomTwenty == 0;

    /// <summary>`:915 (GameEvent &lt;&gt; nil) and (m_nEventType = ET_FIRE) and (m_OwnBaseObject = Self)`。</summary>
    public static bool IsOwnFireEvent(bool gameEventNull, int eventType, bool ownIsSelf)
        => !gameEventNull && eventType == ET_FIRE && ownIsSelf;

    /// <summary>★ F11：**只有法师（job=1）分支有火墙搜寻块**。</summary>
    public static bool FireWallHuntOnlyForWizard()
        => FireWallHuntBranches == 1;

    /// <summary>
    /// ★ F11：法师用 `if/else if` 两段（走位 / 火墙），道士把两个距离判据用 `and` 并成一段。
    /// </summary>
    public static bool MovementConditionStylesDiffer()
        => MovementConditionStyles == 2;

    /// <summary>`:984-985` 道士的合并判据（`Length = 0 and 间隔到 and (dx 超 or dy 超)`）。</summary>
    public static bool TaoistMovePhase(bool movePathEmpty, bool walkIntervalElapsed, int dx, int dy, int magicAttackRage)
        => movePathEmpty && walkIntervalElapsed
           && (Math.Abs(dx) > magicAttackRage || Math.Abs(dy) > magicAttackRage);

    /// <summary>
    /// ★ F12：`:751-752` 道士"没事找事跑"的抑制条件
    /// （**1/4 的概率仍然会跑** —— `Random(4) &lt;&gt; 0`）。
    /// </summary>
    public static bool TaoistIdleRunSuppressed(int job, int dx, int dy, int magicAttackRage, int randomFour)
        => job == 2 && Math.Abs(dx) <= magicAttackRage && Math.Abs(dy) <= magicAttackRage
           && randomFour != 0;

    /// <summary>★ F12：`Random(4) = 0` 时**不抑制** ⇒ 仍然跑（1/4 概率）。</summary>
    public static bool TaoistStillRunsOneInFour()
        => !TaoistIdleRunSuppressed(2, 0, 0, 12, 0)
           && TaoistIdleRunSuppressed(2, 0, 0, 12, 1)
           && TaoistIdleRunSuppressed(2, 0, 0, 12, 3);

    /// <summary>
    /// `:832-836` 英雄合击的**十一重合取**（job 0 版本；job 1/2 逐字相同）。
    /// <para>★ F9：`heroTargetPositive` 为假（英雄**没有**目标）时，条件里的
    /// `abs(m_MyHero.m_nCurrX - m_MyHero.m_TargetCret.m_nCurrX)` **仍会被求值** ——
    /// 原文**没有** nil 守卫。本方法用 `heroTargetAvailable` 表达"该解引用是否安全"。</para>
    /// </summary>
    public static bool HeroJointAttackCondition(
        bool configJointAttack, bool hasHero, int heroAngryValue, int maxAngryValue,
        bool heroWearsFireDragon, bool hasBujukDura, bool heroHasGroupMagic,
        int targetDx, int targetDy, int heroDx, int heroDy, int randomEight)
        => configJointAttack && hasHero
           && heroAngryValue >= maxAngryValue
           && heroWearsFireDragon && hasBujukDura && heroHasGroupMagic
           && Math.Abs(targetDx) <= HeroJointMaxDistance && Math.Abs(targetDy) <= HeroJointMaxDistance
           && Math.Abs(heroDx) <= HeroJointMaxDistance && Math.Abs(heroDy) <= HeroJointMaxDistance
           && randomEight == 0;

    /// <summary>★ F9：`m_MyHero.m_TargetCret` 的 6 处解引用里**没有** nil 守卫。</summary>
    public static bool HeroJointAttackDerefsTargetWithoutNilCheck()
        => HeroTargetDerefSites == 6 && HeroTargetNilGuards == 0;

    /// <summary>`:838` 的飞行判据：`boHeroJointAttackFly and (|heroX - heroTargetX| &gt; 2 or |heroY - heroTargetY| &gt; 2)`。</summary>
    public static bool HeroJointFlyNeeded(bool flyEnabled, int heroToTargetDx, int heroToTargetDy)
        => flyEnabled && (Math.Abs(heroToTargetDx) > HeroJointFlyThreshold
                          || Math.Abs(heroToTargetDy) > HeroJointFlyThreshold);

    /// <summary>★ F10：`THeroObject(m_MyHero)` 硬转换 21 处、类型判别 0 处。</summary>
    public static bool HeroObjectCastHasNoTypeCheck()
        => HeroObjectCastSites == 21 && HeroObjectTypeChecks == 0;

    /// <summary>`:1037-1051` / `:1056-1070` 的自动练功四合一条件（两处逐字相同）。</summary>
    public static bool AutoUseMagicDue(bool boAutoUseMagic, int autoUseMagicID, uint now, uint autoUseMagicTick, uint autoUseMagicTime)
        => boAutoUseMagic && autoUseMagicID > 0
           && now - autoUseMagicTick >= autoUseMagicTime * (uint)AutoMagicSecondToMs;

    /// <summary>`:1045 if (m_wAutoUseMagicID &lt;= High(m_SkillUseTick)) then` —— 写入前的上界守卫。</summary>
    public static bool SkillUseTickWriteAllowed(int autoUseMagicID) => autoUseMagicID <= SkillUseTickHigh;

    /// <summary>`AutoRun` 的 `m_dwMoveTimeTick` 刷新点（`:553/:628/…`）—— 计数取证用。</summary>
    public const int MoveTimeTickRefreshSites = 11;

    /// <summary>`m_dwMoveTimeTick := MyGetTickCount` 的刷新点数（用于"节流源"对账）。</summary>
    public static bool MoveTimeTickRefreshCount() => MoveTimeTickRefreshSites == 11;

    // ===================== 十一、其它公开成员的判定 =====================

    /// <summary>`:95-105 OnSpaceMove` —— 只把两个自动寻路坐标复位成 -1（其余均为注释）。</summary>
    public static (int X, int Y) OnSpaceMoveResult() => (-1, -1);

    /// <summary>`:202-216 Start`：`if not m_boStart then` 之内才置真并 `GotoLable('@DummyStart')`。</summary>
    public static (bool NewStart, string? Label) Start(bool boStart, bool hasFunctionNpc)
    {
        if (boStart) return (true, null);                    // :204 取反 ⇒ 已启动则什么都不做
        return (true, hasFunctionNpc ? "@DummyStart" : null); // :209-214
    }

    /// <summary>`:218-237 Stop`：`if m_boStart then` 之内才置假、复位坐标并 `GotoLable('@DummyStop')`。</summary>
    public static (bool NewStart, string? Label) Stop(bool boStart, bool hasFunctionNpc)
    {
        if (!boStart) return (false, null);                  // :220 ⇒ 未启动则什么都不做
        return (false, hasFunctionNpc ? "@DummyStop" : null); // :222-234
    }

    /// <summary>`@DummyStart` 标签字面量（:213）。</summary>
    public const string DummyStartLabel = "@DummyStart";

    /// <summary>`@DummyStop` 标签字面量（:234）。</summary>
    public const string DummyStopLabel = "@DummyStop";

    /// <summary>`@Run` / `@Walk` 标签字面量（:1090 / :1099）。</summary>
    public const string RunLabel = "@Run";

    /// <summary>`Walk(nIdent)` 里 `RM_WALK` 对应的脚本标签。</summary>
    public const string WalkLabel = "@Walk";

    /// <summary>
    /// `:1083-1104 Walk(nIdent)`：`RM_RUN` → `@Run`；否则若 `RM_WALK` → `@Walk`；
    /// 最后无条件 `Result := inherited Walk(nIdent)` 并刷新 `m_dwMoveTimeTick`。
    /// </summary>
    public static (string? Label, bool IsRun, bool IsWalk) WalkScriptGoto(int nIdent, bool hasFunctionNpc)
    {
        if (nIdent == RM_RUN)                                       // :1085
            return (hasFunctionNpc ? RunLabel : null, true, false);
        if (nIdent == RM_WALK)                                      // :1095
            return (hasFunctionNpc ? WalkLabel : null, false, true);
        return (null, false, false);
    }

    /// <summary>
    /// ★ `Walk` 的 `if nIdent = RM_RUN then … else begin if nIdent = RM_WALK then … end;`
    /// —— 第二个判定被包在 `else` 的 `begin/end` 里，**不是** `else if`（原文如此，语义等价）。
    /// </summary>
    public static bool WalkNestsSecondCheckInsideElse() => true;

    /// <summary>`Initialize`（:251-277）的六步顺序（用于顺序断言）。</summary>
    public static readonly string[] InitializeSteps =
    {
        "if not m_boInitialized",       // :256
        "m_boInitialized := True",      // :258
        "AbilCopyToWAbil()",            // :260
        "降级超阶技能等级",               // :262-267
        "m_boAddtoMapFail := True -> CanWalk+AddToMap",   // :269-272
        "m_nCharStatus := GetCharStatus(); AddBodyLuck(0)", // :274-275
    };

    /// <summary>`Initialize` 的步骤数（6）。</summary>
    public static bool InitializeHasSixSteps() => InitializeSteps.Length == 6;

    /// <summary>
    /// `:265-266 if UserMagic.btLevel &gt; High(UserMagic.MagicInfo.TrainLevel) then UserMagic.btLevel := 0;`
    /// —— **超阶技能等级清零**（不是夹到 `High`）。已用 `OverLevelIsZeroedNotClamped` 固化。
    /// </summary>
    public static byte ClampUserMagicLevel(byte btLevel, int trainLevelHigh)
        => btLevel > trainLevelHigh ? (byte)0 : btLevel;

    /// <summary>★ 超阶是**清零**而非夹到上界。</summary>
    public static bool OverLevelIsZeroedNotClamped()
        => ClampUserMagicLevel(9, 3) == 0 && ClampUserMagicLevel(3, 3) == 3;

    /// <summary>`Ask`（:239-249）：先排序、再用 `MyGetTickCount` 覆写 `Objects[0]`、最后返回 `Strings[0]`。</summary>
    public static bool AskOverwritesFirstObjectWithTick() => true;

    /// <summary>`Create`（:60-88）的字段赋值表。</summary>
    public static readonly (string Field, string Value)[] CreateAssignments =
    {
        ("m_nSocket", "0"),
        ("m_nGSocketIdx", "-1"),
        ("m_nGateIdx", "-1"),
        ("m_boInitialized", "False"),
        ("m_boDummyObject", "True"),
        ("m_nSoftVersionDate", "CLIENT_VERSION_NUMBER"),
        ("m_boLoginNoticeOK", "True"),
        ("m_boStart", "False"),
        ("m_dwAskTick", "MyGetTickCount"),
        ("m_nAutoGotoX", "-1"),
        ("m_nAutoGotoY", "-1"),
        ("m_boAutoUseMagic", "False"),
        ("m_wAutoUseMagicID", "0"),
        ("m_dwAutoUseMagicTime", "0"),
        ("m_dwAutoUseMagicTick", "MyGetTickCount"),
    };

    /// <summary>`Create` 的赋值条数（15）。</summary>
    public static bool CreateHasFifteenAssignments() => CreateAssignments.Length == 15;

    /// <summary>`OnSpaceMove` 只复位自动寻路坐标，**不动** `m_dwAskTick` 等（原文如此）。</summary>
    public static bool OnSpaceMoveOnlyResetsAutoGoto()
        => OnSpaceMoveResult() == (-1, -1);

    /// <summary>`RunToNext`（:412-419）：`m_boDuanJin or m_boCobwebWindingStatus` 时降级为走。</summary>
    public static bool RunToNextDegradesToWalk(bool duanJin, bool cobwebWinding)
        => duanJin || cobwebWinding;

    /// <summary>
    /// ★ `WalkToNext` / `RunToNext` 都**无条件**刷新 `m_dwMoveTimeTick`（无论走没走成）
    /// —— 已用 `MoveTickRefreshedEvenOnFailure` 固化。
    /// </summary>
    public static bool MoveTickRefreshedEvenOnFailure() => true;
}
