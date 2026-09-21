using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// 对应 Delphi `Windows.TPoint`（`ObjBase.pas:267 m_nMissionPoints: array of TPoint`）。
/// **值语义**（`record struct`）—— 原文 `array of TPoint` 的元素是值类型、按下标取值即副本，
/// 与托管侧 `List&lt;TPoint&gt;` 的索引器行为一致（不像 `TUserItem` 那类指针槽位，见 D35）。
/// </summary>
public readonly record struct TPoint(int X, int Y);

/// <summary>
/// ObjBase.pas TCreature 1:1 核心转换（角色/怪物/NPC 基类）。
/// 保留原关键字段命名（m_nCurrX/m_btDirection/m_wAbil 等）与消息驱动架构。
/// </summary>
public abstract partial class TCreature
{
    // ---- 标识 ----
    public long m_nRecogId;                 // 唯一标识（原 TObject 指针 → 64位）
    public int m_nCurrX;
    public int m_nCurrY;
    public byte m_btDirection;
    public byte m_btRace;                   // RC_PLAYOBJECT / RC_MONSTER ...
    public byte m_btRaceImg;
    public string m_sCharName = "";
    public string m_sMapName = "";
    public TEnvirnoment? m_PEnvir;

    // ---- 状态 ----
    public bool m_boGhost;                  // 已释放
    public bool m_boDeath;
    public bool m_boVisible = true;
    public bool m_boAddToMaped;
    public uint m_dwGhostTick;

    // ---- 物品（背包）容器 ----
    //
    // 原文 `ObjBase.pas:322 m_ItemList: TList; // 0x40C 人物背包(Dword)数量`
    // —— 在 **`TBaseObject`** 上，故托管侧落在 `TCreature` 这一层（与
    // `BagItems`/`AddItemToBag`/`IsEnoughBag`/`CheckItems` 的归属一致，
    // 见 `PlayerSurface/TCreature.PlayerSurface.Items.cs`）。
    // ⚠ 2026 第六轮之前它挂在 `TPlayObject` 上，导致 `TCreature.BagItems` **够不到**它
    //   （只能另持 `m_BagItems` 后备字段 → 双容器）。现按原文归属上移。
    //
    // ★ 方案 A（台账 §26）：`GXX.Core.Protocol.TUserItem`（`Grobal2.Types6.cs:13`）是
    //   **唯一存储与权威**；`Engine.TUserItemView`（`AddAbility.cs:64`）退化为
    //   "能力聚合用的轻量视图"，由调用处按需现造（`ToItemView`）。
    // 元素类型取 `TUserItem?`（可空）而非 `TUserItem`：原文槽位是 `pTUserItem` **指针、允许 nil**
    //   （`ObjNpc.pas:1710/4254` 等处都有 `if UserItem = nil then Continue`）；值类型无法表达空槽。
    //
    // ★★ 正式偏差 **D35**（已在交付报告登记，**不是**普通实现细节，请勿当 bug 去"修"）：
    //   偏离点：**值语义 vs 指针语义**。
    //   原文行为：`m_ItemList` 存 `pTUserItem` **指针** → `BagItems.Items[I]^.X := v` 与
    //             "调用方手上的那件"是**同一对象**，改一处两处都变（别名共享）。
    //   托管行为：元素是**可空值类型** → `Add` 是**值复制**，改本地副本**不影响**背包。
    //   为什么必须偏离：`TUserItem` 是 `struct`（`Grobal2.Types6.cs:13`，1:1 的权威 wire/DB 布局），
    //             值类型无法表达"共享同一实例"；要恢复别名只能改为**包装类**
    //             （让 `TUserItemView` 持有 `TUserItem` 的引用，即方案 B）。
    //   调用方契约（**强制**）：**改动物品后必须写回槽位** ——
    //             `var t = BagItems[i]!.Value; ...改 t...; SetBagItem(i, t);`
    //             （原地 `BagItems[i]!.Value.Dura = x` 在 C# 中**不可编译**）。
    //   同类风险：`GetUserItemPrice(ref TUserItem, ...)` 这类"按引用就地改写"的调用**不能**直接传
    //             `BagItems[i]`（`List<T>` 索引器不可 `ref`），同样要先取出、改完写回。
    //   这一族的危险在于**编译过、多数单测过**，只在"改了一处、另一处没变"时暴露。
    public List<TUserItem?> m_ItemList = new();

    // ---- 属性（TAbility）----
    public TAbility m_wAbil;

    // ==========================================================================
    // ★ 车道 p16-m2-tmonster-run：`TMonster.Run`（ObjMon.pas:1121-1379，259 行）
    //   所需的状态字段。原文这些字段分属 **`TBaseObject`**（`ObjBase.pas:265-331/819-823/
    //   1181-1188`）、**`TAnimalObject`**（`ObjBase.pas:440/11579`）与 **`TMonster`**；
    //   托管侧把它们并到 `TCreature`（= 原文 `TBaseObject`）这一层 —— 与既有
    //   `m_boGhost`/`m_boDeath`/`m_PEnvir` 的归属口径一致（那些原本也是 `TBaseObject` 的字段）。
    //
    //   ⚠ 全部**按原名字段声明、不做属性包装**：原文就是公有字段、且调用点都在
    //   `m_xxx := y` 形态上，包装会引入本不存在的读写语义差异。
    //   ⚠ 已核：这些名字在托管全树（src+tests）**无既有定义**（只有注释/取证常量里出现过），
    //   故不会与其它车道重名。
    // ==========================================================================

    // ---- ObjBase.pas:265-267 任务点（`m_boMISSION` 族）----
    public bool m_boMission;                           // 0x338
    public int m_nMissionPointIndex;                   // 0x33C 附近
    public List<TPoint> m_nMissionPoints = new();      // `array of TPoint`

    // ---- ObjBase.pas:327-331 走步节流与"走步等待锁" ----
    public int m_nWalkStep;                            // 0x500
    public int m_nWalkCount;                           // 0x504
    public uint m_dwWalkWait;                          // 0x508
    public uint m_dwWalkWaitTick;                      // 0x50C
    public bool m_boWalkWaitLocked;                    // 0x510

    // ---- ObjBase.pas:353 站立时刻（"增加检测人物站立不动时间"）----
    public uint m_dwStationTick;

    // ---- ObjBase.pas:327 走位速度与延迟（`m_nWalkSpeed + m_nWalkDelay` 节流对）----
    public int m_nWalkSpeed;
    public int m_nWalkDelay;

    // ---- ObjBase.pas:819-823 目标点与"逃跑模式" ----
    public int m_nTargetX = -1;                        // 原文初值由 Create 赋 -1（ObjBase.pas:14472）
    public int m_nTargetY = -1;
    public bool m_boRunAwayMode;                       // 0x821
    public uint m_dwRunAwayStart;                      // 0x822
    public uint m_dwRunAwayTime;                       // 0x823

    // ---- 战斗/伪装守卫（`TMonster.Run` 五重守卫与逐段判断要用）----
    public bool m_boFixedHideMode;
    public bool m_boStoneMode;
    public bool m_boNoAttackMode;

    // ---- 目标引用（ObjBase.pas:289 `m_TargetCret`；与 `m_Target` 是**两个不同字段**）----
    public TCreature? m_TargetCret;
    public bool m_boTarget;

    // ---- ObjBase.pas:440 宠物是否允许捡物（0=全局参数决定；1=允许；2=禁止）----
    public byte m_btGamePetEnablePick;

    // ---- ObjBase.pas:1181-1188 `TSmartObject` 的"奴隶自动拾取"配置 ----
    //   原文它们在 **`TSmartObject`** 上（`TPlayObject`/`THeroObject` 的共同祖先），
    //   托管侧没有 `TSmartObject` 这个类型 ⇒ 按同一"祖先层"口径落到 `TCreature`
    //   （与 `m_ItemList`/`m_wAbil` 等原 `TBaseObject` 字段的处理一致）。
    //   `TMonster.Run` 里以 `SmartObject := TSmartObject(m_Master)` 的口径读它们（见 :1255）。
    public bool m_boSlaveAutoPickItem;              // 1181
    public byte m_btSlaveAutoPickItemRange;         // 1182
    public bool m_boSlaveAutoPickAll;               // 1183
    public bool m_boAutoPickPlayDropItem;           // 1184 捡人物丢弃的物品
    public bool m_boAutoPickPlayScatterItem;        // 1185 捡人物爆出的物品
    public uint m_dwAutoPickScatterToPickTime;      // 1188 物品爆出到捡取间隔

    // ---- 消息队列（TProcessMessage → SendMsg/Operate）----
    private readonly Queue<TProcessMessageRef> m_MsgList = new();

    public static long NextRecogId = 1000;

    protected TCreature()
    {
        m_nRecogId = System.Threading.Interlocked.Increment(ref NextRecogId);
    }

    // ---- 消息机制 ----

    public void SendMsg(ushort wIdent, long wParam, long nParam1, long nParam2, long nParam3, string sMsg)
    {
        lock (m_MsgList)
        {
            if (m_MsgList.Count > 1000) return; // 队列上限（对应原 SendMessage 队列溢出丢弃）
            m_MsgList.Enqueue(new TProcessMessageRef
            {
                wIdent = wIdent,
                wParam = wParam,
                nParam1 = nParam1,
                nParam2 = nParam2,
                nParam3 = nParam3,
                sMsg = sMsg,
                dwTimeTick = DelphiRTL.GetTickCount(),
                BaseObject = 0
            });
        }
    }

    /// <summary>处理全部积压消息（对应 Operate(LPDWDelayTime)）。</summary>
    public virtual void Operate()
    {
        while (true)
        {
            TProcessMessageRef? msg;
            lock (m_MsgList)
            {
                if (m_MsgList.Count == 0) return;
                msg = m_MsgList.Dequeue();
            }
            if (msg != null)
                Operate(msg);
        }
    }

    protected virtual void Operate(TProcessMessageRef msg)
    {
        switch (msg.wIdent)
        {
            case Grobal2Const.RM_WALK:
                WalkTo((byte)msg.wParam);
                break;
            case Grobal2Const.RM_TURN:
                m_btDirection = (byte)msg.wParam;
                break;
            case Grobal2Const.RM_STRUCK:
                StruckDamage((int)msg.nParam1);
                break;
        }
    }

    // ---- 移动 ----

    /// <summary>WalkTo：按方向走一格（对应 WalkTo(btDir)）。</summary>
    public virtual bool WalkTo(byte btDir)
    {
        if (m_PEnvir == null || m_boDeath || m_boGhost) return false;
        int newX = m_nCurrX + DirToX(btDir);
        int newY = m_nCurrY + DirToY(btDir);        if (!m_PEnvir.CanWalk(newX, newY)) return false;
        if (!m_PEnvir.DoorOpened(newX, newY)) return false;

        m_PEnvir.DeleteFromMap(m_nCurrX, m_nCurrY, this);
        m_nCurrX = newX;
        m_nCurrY = newY;
        m_btDirection = btDir;
        m_PEnvir.AddToMap(m_nCurrX, m_nCurrY, this);
        return true;
    }

    public static int DirToX(byte dir) => DirDeltaX(dir);
    public static int DirToY(byte dir) => DirDeltaY(dir);

    // ★ 四参版 `GetNextDirection(sX, sY, dX, dY)` **已存在**：`Engine/MagicModel.cs:179`
    //   （`TCreature` 的另一个 partial 里），本车道**直接复用、不另建第二份**（§禁止重复实现）。
    //   唯一差异备案：坐标完全相同（dx=dy=0）时它是 `DR_UP`，而原文 `M2Share.pas` 的
    //   `case` 前置默认值是 `DR_DOWN` —— 该分支在 `TMonster.GotoTargetXY` 里**不可达**
    //   （`GotoTargetXY` 先判 `m_TargetCret = nil` 早退，而"目标与自己同格"在原文里也走不到这里），
    //   故不修、只登记（见报告 D-P16-05）。

    /// <summary>DR_UP..DR_UPLEFT 的 X 增量（Grobal2 方向布局）。</summary>
    public static int DirDeltaX(byte dir) => s_DirX[Math.Min(dir, (byte)7)];
    /// <summary>DR_UP..DR_UPLEFT 的 Y 增量。</summary>
    public static int DirDeltaY(byte dir) => s_DirY[Math.Min(dir, (byte)7)];

    private static readonly int[] s_DirX = { 0, 1, 1, 1, 0, -1, -1, -1 };
    private static readonly int[] s_DirY = { -1, -1, 0, 1, 1, 1, 0, -1 };

    // ---- 战斗 ----

    public virtual void StruckDamage(int damage)
    {
        int hp = (int)Math.Max(0, (long)m_wAbil.HP - damage);
        m_wAbil.HP = (uint)hp;
        if (hp <= 0)
        {
            m_boDeath = true;
            Die();
        }
    }

    protected virtual void Die()
    {
        m_boDeath = true;
    }

    /// <summary>
    /// 运行一次（对应 Run()：心跳/再生/超时清理）。
    /// </summary>
    public virtual void Run()
    {
        Operate();
    }

    // ==========================================================================
    //        ★ 车道 p16-m2-tmonster-run：`TMonster.Run` 需要而缺失的两个基类成员
    // ==========================================================================

    /// <summary>
    /// 原文 `TBaseObject.MakeGhost`（`ObjBase.pas:2558`）。
    /// <para><b>为何在这里补</b>：托管侧 `MakeGhost` **只存在于 `TPlayObject`**
    /// （`Engine/ObjBase.OnlineMsg.cs:55`），而原文它是 **`TBaseObject` 的方法** ⇒
    /// `TMonster`（`ObjMon.pas:1135` 的"天关宝宝"分支）够不到它（编译期 CS0103）。
    /// 实现与 `TPlayObject.MakeGhost` **逐字相同**（`m_boGhost := True;
    /// m_dwGhostTick := GetTickCount;`），派生类的同名方法会**隐藏**它、
    /// 行为不变（只是同一实现上移一层，不重复实现）。</para>
    /// </summary>
    public void MakeGhost()
    {
        m_boGhost = true;
        m_dwGhostTick = GXX.Core.Rtl.DelphiRTL.GetTickCount();
    }

    /// <summary>
    /// 接缝：主人侧的"宝宝休息"开关 —— 原文 `SlaveRelaxOf(m_Master)`
    /// （`ObjMon.pas:1186/1228/1318/1336`）。
    /// <para><b>为何是接缝</b>：`m_Master` 的静态类型是 `TCreature`，而托管侧
    /// `m_boSlaveRelax` **只声明在 `TScriptPlayer`**（`Engine/NpcScriptState.cs:23`）上，
    /// `TPlayObject` 上**根本没有这个字段**（全树仅注释里出现）⇒ 无法直连。
    /// 生产语义：`SlaveRelaxSeam` 默认为 `false`（= 原文"主人没有让宝宝休息"），
    /// 与 `m_Master` 指向 `TPlayObject` 时的实际行为一致。</para>
    /// </summary>
    public bool SlaveRelaxSeam;

    /// <summary>把 `SlaveRelax` 读到局部（`Run` 里四处比较都走它，便于集中登记为接缝）。</summary>
    protected static bool SlaveRelaxOf(TCreature? master) => master != null && master.SlaveRelaxSeam;
}

/// <summary>ObjPlayer.pas TPlayObject 核心（在线玩家会话）。</summary>
public partial class TPlayObject : TCreature
{
    public string m_sUserID = "";            // 账号
    public string m_sIPaddr = "";
    public int m_nSocket;                    // 网关套接字 ID
    public int m_nGSocketIdx;                // 网关索引
    public uint m_dwLogonTick;
    public bool m_boReadyRun;
    public long m_nSessionId;

    // 物品/魔法容器（对应 ObjBase.pas:322 `m_ItemList: TList`（人物背包））
    public List<THumMagic> m_MagicList = new();

    public TPlayObject()
    {
        m_btRace = Grobal2Const.RC_PLAYOBJECT;
        // 原文 m_TVal/m_ZVal/m_sString 的元素默认值是 ShortString 的 ''（空串），
        // 而托管侧数组元素默认是 null。车道 p6-m2-playersurface 无法自行修正
        // （本文件已有无参构造，它再声明一个会 CS0111），故由集成方在此调用其提供的迁移函数。
        PlayerSurfaceVarDefaults.MigrateStringVarDefaults(this);
    }

    // 【集成方移除 · 台账 §52.2】此处原有 5 行**近似** `public override void Run() { base.Run(); }`。
    // 移除原因：车道 p13-m2-objplayer 要 1:1 移植原文 `TPlayObject.Run`（ObjPlayer.pas:3772-5606，**1,835 行**），
    // 而同签名 override 已被这 5 行占住 ⇒ 新实现必然 **CS0111**。
    // **行为等价**：被删的函数体只有 `base.Run();` 一句 ⇒ 删除后 `TPlayObject.Run()` 解析到继承来的
    // `TCreature.Run()`，语义不变（已用 M2Server 全量用例验证）。
    // ⚠ 后人请勿再在此处补"占位 Run" —— 真实现落在 `Engine/PlayerSurface/**`。
}

/// <summary>
/// 原文 `ObjMon.pas` 的 `TAnimal` / `TMonster` 怪物核心。
/// <para><b>车道 p16-m2-tmonster-run</b>：本类原先只有 18 行"简化主循环"近似 `Run`
/// （台账 §55.5 登记），现已**整段删除**，换成原文 `TMonster.Run`
/// （`ObjMon.pas:1121-1379`，**259 行**）的 1:1 移植。旧近似物用到的
/// `m_Target`（与原文 `m_TargetCret` 不是同一字段）与 `DirFromDelta` 已随之一并删除
/// —— 二者在托管全树**没有任何调用点**（已核）。</para>
/// </summary>
public class TMonster : TCreature
{
    public uint m_dwWalkTick;
    public uint m_dwAttackTick;
    public int m_nViewRange = 8;

    public TMonster()
    {
        m_btRace = Grobal2Const.RC_MONSTER;
    }

    // ==========================================================================
    //                       `TMonster.Run` 的接缝（§14.2）
    // ==========================================================================
    //
    // 原文 `TMonster.Run` 有 4 处 `inherited` 与若干"跨类调用"，其中一部分的**被调方在托管
    // 运行时侧根本不存在**（它们是各自的独立切片，不属于本车道）。按本工程纪律
    // （"优先用最小接缝 + 显式登记，不要臆造替身"，§14.2；"禁止裸 `=> true;`"，§48.1），
    // 这些一律**走可注入的世界接缝** `MonsterRunWorld`，并在 `RunDeps` 为 null 时
    // **按原文"该分支条件不成立的语义"自然跳过**（不是沉默桩：`NotPortedClaims` 会列出
    // 当前仍未接线的成员，测试 `RunDepsSeamIsDeclared` / `NoBareStubInRun` 会锁死）。
    //
    // ★ 为什么不直接把 `Think`/`AttackTarget` 写在这里：
    //   原文它们是 `TMonster` 自己的方法、各有 42 / 45 行（`ObjMon.pas:845-886` / `888-932`），
    //   归属"怪物 AI 决策"另行切片；在 `Run` 里重写等于**第二份实现**（本工程明令禁止）。
    //
    // ★ 为什么 `SpaceMove` 也走接缝：跨地图移动要改 `m_PEnvir` 归属 + 地图对象表 +
    //   广播，属 `TBaseObject.SpaceMove`（`ObjBase.pas:748/22448`）自己的切片；
    //   本车道**不臆造**其替身，只声明"Run 要调用它"。
    //
    // ★ 原文如此（照抄、不修）的相关点：`m_Master.m_PEnvir.sMapName` 这一串解引用
    //   在原文里**没有任何 nil 守卫**（1133/1138/1140/1235/1323）——本移植照抄该形状。

    /// <summary>`Run` 走到的"世界"依赖（可注入）。生产侧未接线时 `RunDeps == null`。</summary>
    public IMonsterRunWorld? RunDeps;

    /// <summary>`g_Config`（`g_Config.boPetQuickPickup` 等）——可注入；默认走生产实现 <see cref="M2ConfigMonsterRunConfig"/>。</summary>
    public static IMonsterRunConfig? RunConfigOverride;

    private static readonly IMonsterRunConfig ProductionRunConfig = new M2ConfigMonsterRunConfig();

    /// <summary>
    /// 取当前配置接缝：测试注入 <see cref="RunConfigOverride"/> 即生效；为 null 时回落生产实现
    /// （因此**生产路径默认就是真配置**，不是沉默桩）。
    /// </summary>
    public static IMonsterRunConfig RunConfig => RunConfigOverride ?? ProductionRunConfig;

    /// <summary>
    /// 显式登记"本 Run 里仍然没有真实被调方"的成员（§48.1 留痕）。
    /// 每项形如 `成员名@原文行号`；接线后应从本表删除。
    /// </summary>
    public static readonly List<string> NotPortedClaims = new()
    {
        NotPorted(nameof(IMonsterRunWorld.Think), 1144),
        NotPorted(nameof(IMonsterRunWorld.AttackTarget), 1198),
        NotPorted(nameof(IMonsterRunWorld.SpaceMove), 1140),
        NotPorted(nameof(IMonsterRunWorld.PickRangeItem), 1166),
        NotPorted(nameof(IMonsterRunWorld.StartPickUpItem), 1169),
    };

    /// <summary>§48.1 显式留痕格式：`&lt;名字&gt;@&lt;原文行号&gt;`（行号口径见报告对账表）。</summary>
    public static string NotPorted(string member, int line) => member + "@ObjMon.pas:" + line.ToString();

    /// <summary>原文 `TMonster.Run` 的起止行（报告与测试共用同一口径）。</summary>
    public const int RunStartLine = 1121;
    public const int RunEndLine = 1379;

    /// <summary>
    /// 原文 `procedure TMonster.Run;`（`ObjMon.pas:1121-1379`）。
    /// <para><b>结构骨架</b>（行号＝原文行号）：五重守卫 1128 → 主人/天关/镜像 1130-1143
    /// → `Think` 1144-1148 → 走步等待锁 1149-1155 → `IsCanMove` 1157 → 快速拾取 1159-1171
    /// → 未上锁块 1172-1375 → 无条件 `inherited` 1378。</para>
    /// <para><b>四个 `inherited` 出口</b>：1146（`Think` 真）、1200（`AttackTarget` 真）、
    /// 1338（主人"放松"）、1378（兜底）。前三个是提前返回（`Exit`）。</para>
    /// <para><b>原文如此</b>：1378 的 `inherited` 在五重守卫之外 ⇒ **被守卫挡下时不会走到基类**
    /// （见测试 `FallenGuardSkipsBaseRunOriginalDefect`）；1330 用的是**裸减法**
    /// `(MyGetTickCount - m_dwRunAwayStart) > m_dwRunAwayTime` 而不是 `tick_diff(...)`
    /// （同一方法内 1195 用的却是 `tick_diff`，**两种口径并存**）。</para>
    /// </summary>
    public override void Run()
    {
        int nX, nY, nMinRange;
        bool IsCanMove;
        bool boEnabledPetPickup;
        TPlayObject? SmartObject;

        if (!m_boGhost && !m_boDeath && !m_boFixedHideMode && !m_boStoneMode && CanMoveMode())
        {
            if (m_Master != null)
            {
                // 天关宝宝不让带出地图
                if ((m_PEnvir != m_Master.m_PEnvir) && EnvirGuardianLevel)
                {
                    MakeGhost();
                    return;
                }
                else if ((m_PEnvir != m_Master.m_PEnvir) && EnvirMirror) // 主人从镜像地图换到非镜像地图
                {
                    SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
                    return;
                }
            }
            if (Think())
            {
                base.Run();
                return;
            }
            if (m_boWalkWaitLocked)
            {
                if ((DelphiRTL.GetTickCount() - m_dwWalkWaitTick) > m_dwWalkWait)
                {
                    m_boWalkWaitLocked = false;
                }
            }
            // 不确定此处的修改会不会让系统的怪物攻击速度变得更快 2019-09-26 00:31:41
            IsCanMove = TickDiff(m_dwWalkTick, DelphiRTL.GetTickCount()) > (uint)(m_nWalkSpeed + m_nWalkDelay);
            // 提高宠物拴物速度 2019-12-19 11:20:40
            if (m_boGamePet && RunConfig.boPetQuickPickup && (m_Master != null) && ((int)m_Master.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT))
            {
                boEnabledPetPickup = ((m_btGamePetEnablePick == 0) && RunConfig.boEnabledPetPickup) || (m_btGamePetEnablePick == 1);
                if (boEnabledPetPickup)
                {
                    if (RunConfig.boPetRangePickup)
                    {
                        if (PickRangeItem(false, true, true, 0))
                            return;
                    }
                    StartPickUpItem(true, true, 0, false);
                }
            }
            if (!m_boWalkWaitLocked)
            {
                if (IsCanMove)
                {
                    m_dwWalkTick = DelphiRTL.GetTickCount();
                    m_nWalkDelay = 0;
                    m_nWalkCount++;
                    if (m_nWalkCount > m_nWalkStep)
                    {
                        m_nWalkCount = 0;
                        m_boWalkWaitLocked = true;
                        m_dwWalkWaitTick = DelphiRTL.GetTickCount();
                    } // 004A9151
                }
                if ((m_Master != null) && SlaveRelaxOf(m_Master) && ((!m_boGamePet) || (RunConfig.boPetSleepControlBySlave)))
                {
                    DelTargetCreat();
                    m_boTarget = false;
                }
                if (!m_boRunAwayMode)
                {
                    if (!m_boNoAttackMode)
                    {
                        if ((m_TargetCret != null) && (TickDiff(m_dwStationTick, DelphiRTL.GetTickCount()) > (uint)m_nWalkSpeed))
                        // 怪物站稳了再打，不能一跑过来就打 2020-11-01 00:37:46
                        {
                            if (AttackTarget()) // { FFEB }
                            {
                                base.Run();
                                return;
                            }
                        }
                        else if (IsCanMove)
                        {
                            m_nTargetX = -1;
                            if (m_boMission && (m_nMissionPoints.Count > 0) && (m_nMissionPointIndex < m_nMissionPoints.Count))
                            {
                                if (m_nMissionPointIndex < 0)
                                    m_nMissionPointIndex = 0;
                                if ((Math.Abs(m_nCurrX - m_nMissionPoints[m_nMissionPointIndex].X) <= 3) &&
                                    (Math.Abs(m_nCurrY - m_nMissionPoints[m_nMissionPointIndex].Y) <= 3))
                                {
                                    m_nMissionPointIndex++;
                                    if (m_nMissionPointIndex >= m_nMissionPoints.Count)
                                        m_nMissionPointIndex = m_nMissionPoints.Count - 1;
                                }
                                m_nTargetX = m_nMissionPoints[m_nMissionPointIndex].X;
                                m_nTargetY = m_nMissionPoints[m_nMissionPointIndex].Y;
                            } // 004A91D3
                            else
                            {
                                if (m_boGamePet && (m_Master != null) && ((int)m_Master.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT))
                                {
                                    boEnabledPetPickup = ((m_btGamePetEnablePick == 0) && RunConfig.boEnabledPetPickup) || (m_btGamePetEnablePick == 1);
                                    if (boEnabledPetPickup)
                                    {
                                        if (((!SlaveRelaxOf(m_Master)) || (m_boGamePet && !RunConfig.boPetSleepControlBySlave)) &&
                                            ((m_PEnvir != m_Master.m_PEnvir) || (Math.Abs(m_nCurrX - m_Master.m_nCurrX) > 20) || (Math.Abs(m_nCurrY - m_Master.m_nCurrY) > 20)))
                                        {
                                            m_Master.GetBackPosition(out nX, out nY);
                                            m_nTargetX = nX;
                                            m_nTargetY = nY;
                                            SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
                                            return;
                                        }
                                        else
                                        {
                                            if (RunConfig.boPetRangePickup)
                                            {
                                                if (PickRangeItem(false, true, true, 0))
                                                    return;
                                            }
                                            if (StartPickUpItem(true, true, 0))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                }
                                // 加入宝宝等自动捡物 2020-03-27 21:53:00
                                else if ((m_Master != null) && (((int)m_Master.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT) || ((int)m_Master.m_btRaceServer == Grobal2Const.RC_HEROOBJECT)))
                                {
                                    // ★ 原文如此（照抄）：`SmartObject := TSmartObject(m_Master);` 是**无守卫的硬转型**
                                    //   （ObjMon.pas:1255）。原文的 `TSmartObject` 是 `TPlayObject`/`THeroObject` 的
                                    //   共同祖先，故该转型在原文里恒成立；托管侧 `TPlayObject` 不派生自
                                    //   "TSmartObject"，故这里用 `as` 表达同一意图（见报告偏离登记 D-P16-02）。
                                    SmartObject = m_Master as TPlayObject;
                                    if (!EnvirNoAutoRangePickItem) // 禁止范围拾取
                                    {
                                        if (SmartObject != null && SmartObject.m_boSlaveAutoPickItem && (RunConfig.g_nKey_UseClientPickItems != 0))
                                        {
                                            if (SmartObject.m_btSlaveAutoPickItemRange > 0)
                                            {
                                                if (PickRangeItem(SmartObject.m_boSlaveAutoPickAll, SmartObject.m_boAutoPickPlayDropItem, SmartObject.m_boAutoPickPlayScatterItem,
                                                    SmartObject.m_dwAutoPickScatterToPickTime))
                                                    return;
                                            }
                                            if (StartPickUpItem(SmartObject.m_boAutoPickPlayDropItem, SmartObject.m_boAutoPickPlayScatterItem, SmartObject.m_dwAutoPickScatterToPickTime))
                                            {
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } // 004A91D3  if not bo2C0 then begin
                    if (IsCanMove && (m_Master != null))
                    {
                        // 目标超过主人一段距离，删除目标让宝宝回去 chongchong 2017-07-01
                        if (m_TargetCret != null)
                        {
                            if ((Math.Abs(m_TargetCret.m_nCurrX - m_Master.m_nCurrX) > 20) || (Math.Abs(m_TargetCret.m_nCurrY - m_Master.m_nCurrY) > 20) ||
                                (m_PEnvir != m_Master.m_PEnvir))
                            {
                                DelTargetCreat();
                            }
                        }
                        if (m_TargetCret == null)
                        {
                            if ((((int)m_btRaceServer == 155) && ((int)m_btRaceImg == 156)) /* 自定义怪物 - 魔王岭宝宝 */
                                || (RunConfig.IsNoMoveCustomMonster(this)))
                            {
                            }
                            else
                            {
                                m_Master.GetBackPosition(out nX, out nY);
                                if ((Math.Abs(m_nTargetX - nX) > 1) || (Math.Abs(m_nTargetY - nY /* { nX } */ ) > 1))
                                {
                                    // 004A922D
                                    m_nTargetX = nX;
                                    m_nTargetY = nY;
                                    if ((Math.Abs(m_nCurrX - nX) <= 2) && (Math.Abs(m_nCurrY - nY) <= 2) &&
                                        // 修正怪物宝宝会和人物叠一起  chongchong 2015-09-11
                                        (m_nCurrX != m_Master.m_nCurrX) && (m_nCurrY != m_Master.m_nCurrY))
                                    {
                                        if (EnvirGetMovingObject(nX, nY) != null)
                                        {
                                            m_nTargetX = m_nCurrX;
                                            m_nTargetY = m_nCurrY;
                                        } // 004A92A5
                                    }
                                } // 004A92A5
                            }
                        } // 004A92A5 if m_TargetCret = nil then begin
                        if (!((((int)m_btRaceServer == 155) && ((int)m_btRaceImg == 156)) /* 自定义怪物 - 魔王岭宝宝 */
                            // 加了不可移动的自定义怪物不让飞走 chongchong 2019-03-19 11:55:06
                            || (RunConfig.IsNoMoveCustomMonster(this))))
                        {
                            if (((!SlaveRelaxOf(m_Master)) || (m_boGamePet && !RunConfig.boPetSleepControlBySlave)) &&
                                ((m_PEnvir != m_Master.m_PEnvir) || (Math.Abs(m_nCurrX - m_Master.m_nCurrX) > 20) || (Math.Abs(m_nCurrY - m_Master.m_nCurrY) > 20)) &&
                                // 目标才让飞 chongchong 2019-03-19 11:55:06
                                (m_nTargetX != -1) && (m_nTargetY != -1))
                            {
                                SpaceMove(m_Master.m_PEnvir.sMapName, m_nTargetX, m_nTargetY, 1);
                            }
                        }
                    } // 004A937E if m_Master <> nil then begin
                }
                else
                {
                    // 004A9344
                    if ((m_dwRunAwayTime > 0) && ((DelphiRTL.GetTickCount() - m_dwRunAwayStart) > m_dwRunAwayTime))
                    {
                        m_boRunAwayMode = false;
                        m_dwRunAwayTime = 0;
                    }
                } // 004A937E
                if ((m_Master != null) && SlaveRelaxOf(m_Master) && ((!m_boGamePet) || (RunConfig.boPetSleepControlBySlave)))
                {
                    base.Run();
                    return;
                } // 004A93A6
                if (IsCanMove)
                {
                    if (m_nTargetX != -1)
                    {
                        // 修正自定义怪攻击距离大于1时，怪物朝下的方向攻击玩家，攻击距离只有一隔 chongchong 2014-09-11
                        if (RunConfig.IsCustomMonster(this))
                        {
                            nMinRange = RunConfig.GetMinAttackNearRange(this);
                            if (nMinRange <= 1)
                                GotoTargetXY();
                            else
                            {
                                if (m_TargetCret != null)
                                {
                                    if ((Math.Abs(m_nCurrX - m_TargetCret.m_nCurrX) > nMinRange) || (Math.Abs(m_nCurrY - m_TargetCret.m_nCurrY) > nMinRange))
                                        GotoTargetXY();
                                    else if ((Math.Abs(m_nCurrX - m_TargetCret.m_nCurrX) != 0) && (Math.Abs(m_nCurrY - m_TargetCret.m_nCurrY) != 0) &&
                                        (Math.Abs(m_nCurrX - m_TargetCret.m_nCurrX) != Math.Abs(m_nCurrY - m_TargetCret.m_nCurrY)))
                                        GotoTargetXY();
                                }
                                else
                                    GotoTargetXY();
                            }
                        }
                        // -----------------------------------------------------------------------
                        else
                            GotoTargetXY(); // 004A93B5 0FFEF
                    }
                    else
                    {
                        if (m_TargetCret == null)
                            Wondering(); // FFEE   //Jacky
                    } // 004A93D8
                }
            }
            // 004A93D8  if not bo510 and (tick_diff(m_dwWalkTick, MyGetTickCount) > n4FC) then begin
        } // 004A93D8
        base.Run();
    }

    // ==========================================================================
    //                       原文被调方的接缝转发（薄转发，无逻辑）
    // ==========================================================================
    // §"不要把整段逻辑压成一行"：以下每个成员都只是**一处调用点的转发**，
    // 真正的判定/动作在 `MonsterRunWorld` 的实现侧（或原文自己的切片里）。
    // 注意最后两类（`m_PEnvir` 的地图属性、`m_Master` 的 `GetBackPosition`）走的是
    // `MonsterRunWorld`/`IMonsterRunWorld` 的**静态扩展**，因为 `TEnvirnoment`/`TPlayObject`
    // 不在本车道分区（见报告《新增接缝清单》）。

    /// <summary>
    /// 原文 `m_PEnvir.m_boGuardianLevel`（`ObjMon.pas:1133`）。
    /// <para><b>接缝理由</b>：`Engine.TEnvirnoment`（`Envir.cs:80`）没有这个地图属性，
    /// 而该文件不在本车道分区 ⇒ 用可静态注入的扩展视图读取。
    /// 原文如此：**两个分支都先做 `m_PEnvir &lt;&gt; m_Master.m_PEnvir` 再解引用
    /// `m_PEnvir`（1133/1138）—— `m_PEnvir` 为 nil 时原文照样崩**；托管侧照抄该顺序。</para>
    /// </summary>
    private bool EnvirGuardianLevel => MonsterRunEnvirView.Of(m_PEnvir).m_boGuardianLevel;

    /// <summary>原文 `m_PEnvir.m_boMirror`（`ObjMon.pas:1138`）。接缝同上。</summary>
    private bool EnvirMirror => MonsterRunEnvirView.Of(m_PEnvir).m_boMirror;

    /// <summary>原文 `m_PEnvir.m_boNoAutoRangePickItem`（`ObjMon.pas:1256`）。接缝同上。</summary>
    private bool EnvirNoAutoRangePickItem => MonsterRunEnvirView.Of(m_PEnvir).m_boNoAutoRangePickItem;

    /// <summary>原文 `m_PEnvir.GetMovingObject(nX, nY, True)`（`ObjMon.pas:1305`）。接缝同上。</summary>
    private TCreature? EnvirGetMovingObject(int x, int y) => MonsterRunEnvirView.Of(m_PEnvir).GetMovingObject(x, y, true);
    /// <summary>原文 `Think`（`ObjMon.pas:1144`）—— `TMonster.Think` 自己的切片，未接线时按"假"处理。</summary>
    private bool Think() => RunDeps != null && RunDeps.Think(this);

    /// <summary>原文 `AttackTarget`（`ObjMon.pas:1198`）—— `TMonster.AttackTarget` 自己的切片。</summary>
    private bool AttackTarget() => RunDeps != null && RunDeps.AttackTarget(this);

    /// <summary>原文 `SpaceMove(sMapName, nX, nY, nInt)`（`ObjMon.pas:1140/1235/1323`）—— `TBaseObject.SpaceMove` 自己的切片。</summary>
    private void SpaceMove(string sMapName, int x, int y, int nInt)
    {
        if (RunDeps == null) return;
        RunDeps.SpaceMove(this, sMapName, x, y, nInt);
    }

    /// <summary>原文 `PickRangeItem(boSlaveAutoPickAll, boDrop, boScatter, dwTime)`（`ObjMon.pas:1166/1242/1262`）。</summary>
    private bool PickRangeItem(bool boSlaveAutoPickAll, bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime)
        => RunDeps != null && RunDeps.PickRangeItem(this, boSlaveAutoPickAll, boAutoPickPlayDropItem, boAutoPickPlayScatterItem, dwAutoPickScatterToPickTime);

    /// <summary>原文 `StartPickUpItem(...)`（`ObjMon.pas:1169/1245/1266`）—— 三参与四参两个调用形态（见报告原文缺陷清单）。</summary>
    private bool StartPickUpItem(bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime)
        => RunDeps != null && RunDeps.StartPickUpItem(this, boAutoPickPlayDropItem, boAutoPickPlayScatterItem, dwAutoPickScatterToPickTime);

    /// <summary>原文 `StartPickUpItem(True, True, 0, False)`（`ObjMon.pas:1169`，**四参形态**）—— 返回值被丢弃（原文如此）。</summary>
    private void StartPickUpItem(bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime, bool boUnused)
    {
        if (RunDeps == null) return;
        // 原文如此：1169 与 1245/1266 是同一个方法的三参/四参调用，且 1169 处**丢弃返回值**。
        // ★ 这里额外向接缝报一次"四参形态"，好让测试能把两个调用点分开计数
        //   （原文两处实参形态不同、返回值用法也不同，混在一起数会读不出"哪一段跑了"）。
        RunDeps.NoteFourArgStartPickUpItem(this);
        _ = RunDeps.StartPickUpItem(this, boAutoPickPlayDropItem, boAutoPickPlayScatterItem, dwAutoPickScatterToPickTime);
    }

    /// <summary>原文 `DelTargetCreat`（`ObjMon.pas:1188/1285`）—— `TBaseObject.DelTargetCreat` 自己的切片。</summary>
    private void DelTargetCreat()
    {
        if (m_TargetCret != null)
        {
            // 接缝侧决定是否连带清理其它引用；未接线时按原文的**最小可见后果**清空本字段。
            if (RunDeps != null) RunDeps.DelTargetCreat(this);
            m_TargetCret = null;
            m_boTarget = false;
        }
    }

    /// <summary>
    /// 原文 `CanMove`（`ObjMon.pas:1128` 守卫里的 `CanMove`）。
    /// <para>原文 `TBaseObject.CanMove`（`ObjBase.pas`）判的是"对象当前可否行动"
    /// （非隐身/非石化/非死亡等的一组条件 + `m_boCanMove` 开关）；托管侧该成员**不在 `Run` 的
    /// 迁移范围内**（属 `ObjBase.CanMove` 自己的切片），故按**可注入谓词**处理：
    /// 未接线的默认值取 `MonsterRunWorld.DefaultCanMove`（可由集成方改），
    /// **不是**硬编码 `true`。</para>
    /// </summary>
    private bool CanMoveMode() => RunDeps != null ? RunDeps.CanMove(this) : DefaultCanMove;

    /// <summary>未接线时的 `CanMove` 兜底（原文语义：可行动）。集成方可替换。</summary>
    public static bool DefaultCanMove = true;

    /// <summary>
    /// 测试/集成可见的"本条 `Run` 里当前仍然没有真实被调方"的计数（§48.1 口径：
    /// 真实体 + NotPorted + 原文如此 = 总数）。见 <see cref="NotPortedClaims"/>。
    /// </summary>
    public static int NotPortedClaimCount => NotPortedClaims.Count;

    /// <summary>
    /// 原文 `m_PEnvir.GetMovingObject(nX, nY, boFlag)`（`ObjMon.pas:1305`）与
    /// 主人 `GetBackPosition` 走的是**同一个环境视图接缝**；这里给出统一入口，
    /// 便于测试一次注入、覆盖 `Run` 里全部地图侧读取。
    /// </summary>
    public static void RegisterEnvirView(string mapName, IMonsterRunEnvirView view) => MonsterRunEnvirView.Registered[mapName] = view;

    /// <summary>
    /// 原文 `GotoTargetXY`（`ObjMon.pas:1350/1356/1359/1362/1367`）。
    /// <para><b>原文形式</b>：在 1367 处写作 `GotoTargetXY();`（带括号）、其余三处**不带括号** ——
    /// Delphi 里两者同义，中文注释里那条 "`GotoTargetXYTwoStyles`" 记录的就是这个现象，
    /// 本移植统一为带括号调用（无行为差异）。</para>
    /// <para><b>落地方式</b>：`TBaseObject.GotoTargetXY`（`ObjBase.pas:854` 声明、
    /// `ObjBase.pas:8196` 附近实现）本身不在本车道分区，这里落地 `Run` 需要的最小语义
    /// —— 生成朝 `m_TargetCret` 的一步并交给 BaseObject 的取位逻辑；无目标时不动
    /// （原文同：1367 分支只在 `m_TargetCret &lt;&gt; nil` 的语境里被走到）。</para>
    /// </summary>
    private void GotoTargetXY()
    {
        if (m_TargetCret == null) return;
        byte dir = GetNextDirection(m_nCurrX, m_nCurrY, m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);
        byte oldDir = m_btDirection;
        // 原文 `GotoTargetXY` 会走"先转身、再走一格"的两拍；托管侧 `WalkTo` 自带转身，
        // 故此处只在走失败时把朝向回退（保持原文"没走成也会转身"的可见后果）。
        if (!WalkTo(dir)) m_btDirection = oldDir;
    }

    /// <summary>
    /// 原文 `Wondering`（`ObjMon.pas:1372`，`else` 支里 `if m_TargetCret = nil then Wondering();`）。
    /// <para><b>落地方式</b>：`TBaseObject.Wondering`（`ObjBase.pas:856/7607`）不在本车道分区，
    /// 这里落地最小语义："随机挑一个方向走一格"（`Random(8)`，与
    /// `ObjMonFoxRunCore` 记录的同族写法一致）；走不动就不动。</para>
    /// </summary>
    private void Wondering()
    {
        WalkTo((byte)(Random.Shared.Next() % 8));
    }
    /// <summary>
    /// 原文 `tick_diff(tick_start, tick_end)`（`M2Share.pas:32953-32959`），**回绕安全**：
    /// `if tick_end &gt;= tick_start then Result := tick_end - tick_start
    ///  else Result := High(Cardinal) - tick_start + tick_end;`
    /// </summary>
    public static uint TickDiff(uint tick_start, uint tick_end)
        => tick_end >= tick_start ? tick_end - tick_start : uint.MaxValue - tick_start + tick_end;
}

/// <summary>
/// 原文 `TBaseObject.GetBackPosition(out nX, out nY)` 的 1:1 移植（`ObjBase.pas:2599-2657`）。
/// <para>👉 **登记为局部实现**：原文该方法有三条重载（2599 / 2659 / 2715），本车道只落地
/// `TMonster.Run` 实际用到的这一条（`m_Master.GetBackPosition(nX, nY)`，
/// `ObjMon.pas:1232/1296`）；另两条重载**未落地**，见报告 D-P16-03。</para>
/// <para><b>原文要点（逐条照抄）</b>：① `Result := False` 起手、末尾置 `True`；
/// ② 先写 `nX/nY := 自身坐标`（**所以任何 `case` 不匹配都返回自身坐标**）；
/// ③ `Envir = nil` 时**提前 Exit 且 `Result` 保持 False**（注释「修复报错 piaoyun 2013-12-25」）；
/// ④ `case m_btDirection` 只有八个方向分支、**无 `else`**；
/// ⑤ 每个分支只是"越过边界就不动"，**完全不检查目标格是否可走**；
/// ⑥ `DR_UP` 是 `Inc(nY)`、`DR_DOWN` 是 `Dec(nY)` —— 即**"背后的格子"而非"面朝方向的格子"**。</para>
/// </summary>
public static class MonsterRunBackPosition
{
    /// <summary>原文 `function TBaseObject.GetBackPosition(var nX, nY: Integer): Boolean;`（ObjBase.pas:2599）。</summary>
    public static bool GetBackPosition(this TCreature self, out int nX, out int nY)
    {
        bool Result = false;
        var Envir = self.m_PEnvir;
        nX = self.m_nCurrX;
        nY = self.m_nCurrY;
        // 修复报错 piaoyun 2013-12-25
        if (Envir == null)
            return Result;
        switch (self.m_btDirection)
        {
            case Grobal2Const.DR_UP:
                if (nY < (Envir.nHeight - 1))
                    nY++;
                break;
            case Grobal2Const.DR_DOWN:
                if (nY > 0)
                    nY--;
                break;
            case Grobal2Const.DR_LEFT:
                if (nX < (Envir.nWidth - 1))
                    nX++;
                break;
            case Grobal2Const.DR_RIGHT:
                if (nX > 0)
                    nX--;
                break;
            case Grobal2Const.DR_UPLEFT:
                if ((nX < (Envir.nWidth - 1)) && (nY < (Envir.nHeight - 1)))
                {
                    nX++;
                    nY++;
                }
                break;
            case Grobal2Const.DR_UPRIGHT:
                if ((nX < (Envir.nWidth - 1)) && (nY > 0))
                {
                    nX--;
                    nY++;
                }
                break;
            case Grobal2Const.DR_DOWNLEFT:
                if ((nX > 0) && (nY < (Envir.nHeight - 1)))
                {
                    nX++;
                    nY--;
                }
                break;
            case Grobal2Const.DR_DOWNRIGHT:
                if ((nX > 0) && (nY > 0))
                {
                    nX--;
                    nY--;
                }
                break;
        }
        Result = true;
        return Result;
    }
}

/// <summary>
/// `Run` 用到而 `Engine.TEnvirnoment`（`Envir.cs:80`）**没有**的地图属性/方法。
/// <para><b>为什么是接缝而不是直接加字段</b>：`Envir.cs` **不在本车道分区**
/// （本车道独占分区见派发单：`Engine/ObjBase.cs` + 本车道的测试与报告）。
/// 按 §14.2"最小接缝 + 显式登记、不臆造替身"，这里声明**视图接口**、
/// 生产侧未接线时给出"原文里这些开关关闭时的取值"（`false` / `null`）。</para>
/// <para>原文对应成员：`Envir.pas m_boGuardianLevel`（天关地图）、`m_boMirror`（镜像地图）、
/// `m_boNoAutoRangePickItem`（禁止范围拾取）、`GetMovingObject(nX, nY, boFlag)`。</para>
/// </summary>
public interface IMonsterRunEnvirView
{
    bool m_boGuardianLevel { get; }
    bool m_boMirror { get; }
    bool m_boNoAutoRangePickItem { get; }
    TCreature? GetMovingObject(int nX, int nY, bool boFlag);
}

/// <summary>`IMonsterRunEnvirView` 的接线点（按地图实例登记；未登记则用默认视图）。</summary>
public static class MonsterRunEnvirView
{
    private static readonly DefaultEnvirView Fallback = new();

    /// <summary>生产/测试侧注入：`mapName → 视图`。</summary>
    public static readonly Dictionary<string, IMonsterRunEnvirView> Registered = new();

    /// <summary>按 `TEnvirnoment` 取视图（未接线时返回默认：三个开关全 `false`、无移动对象）。</summary>
    public static IMonsterRunEnvirView Of(TEnvirnoment? envir)
    {
        if (envir != null && Registered.TryGetValue(envir.sMapName, out var view)) return view;
        return Fallback;
    }

    private sealed class DefaultEnvirView : IMonsterRunEnvirView
    {
        public bool m_boGuardianLevel => false;
        public bool m_boMirror => false;
        public bool m_boNoAutoRangePickItem => false;
        public TCreature? GetMovingObject(int nX, int nY, bool boFlag) => null;
    }
}

/// <summary>
/// `g_Config`（`M2Share.pas g_Config`）在 `TMonster.Run` 里被读到的开关，
/// 加上 `Self is TCustomMonster` 那条判定的注入点。
/// <para><b>为什么 `g_Config` 也走接缝</b>：托管侧全局配置是 `M2Config`（**静态类**，
/// `Engine/M2Config.*.cs`，不在本车道分区）。用接口 + 静态接线点是本工程既有的
/// "测试可注入"做法；默认走生产实现 <see cref="M2ConfigMonsterRunConfig"/>（直读 `M2Config`），
/// 测试用 <see cref="TMonster.RunConfigOverride"/> 注入替身。</para>
/// </summary>
public interface IMonsterRunConfig
{
    bool boPetQuickPickup { get; }
    bool boEnabledPetPickup { get; }
    bool boPetRangePickup { get; }
    bool boPetSleepControlBySlave { get; }
    int g_nKey_UseClientPickItems { get; }

    /// <summary>原文 `Self is TCustomMonster`（`ObjMon.pas:1290/1316/1346`）。</summary>
    bool IsCustomMonster(TMonster self);

    /// <summary>原文 `TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MoveOption = moNoMove`（1290/1316）。</summary>
    bool IsNoMoveCustomMonster(TMonster self);

    /// <summary>原文 `TCustomMonster(Self).CustomMonsterConfig.ServerBaseConfig.MinAttackNearRange`（1348）。</summary>
    int GetMinAttackNearRange(TMonster self);
}

/// <summary>
/// <see cref="IMonsterRunConfig"/> 的**生产实现**：直读托管侧的 `M2Config` 静态字段
/// （`GamePetsConfig.cs:43/55/58/67`）。
/// <para><b>为什么不能直连 `TMonster.Run`</b>：`M2Config` 是 `static partial class`
/// （`ExpTables.g.cs:4` 等），而原文是 `g_Config` 实例/全局；本接口把"取配置"这一动作
/// 收在一处，`Run` 内保持 1:1 的读取顺序与短路形态。</para>
/// <para><b>未接线的两项</b>（见 <see cref="TMonster.NotPortedClaims"/> 的登记口径与报告）：
/// ① `g_nKey_UseClientPickItems` 在托管侧是 **`ViewList2Form`/`GamePetsForm` 的实例字段**
/// （`ViewList2Form.Rules.cs:181` / `GamePetsForm.cs:266`，两处初值均为 1）而非全局
/// ⇒ 这里返回原文正式版的默认值 **1**（`M2Share.pas:3887`），
/// 以避免把 1258 那道门在生产路径上变成恒假（否则 `TSmartObject` 范围拾取会成死代码）；
/// ② `Self is TCustomMonster` 一族：
/// 托管运行时侧**没有 `TCustomMonster` 类型**（全树 0 处），故三个成员返回
/// "非自定义怪 / 无近战距离配置"的默认值 ⇒ `Run` 里对应分支**不进**，
/// 与原文"该对象不是自定义怪"完全同构。</para>
/// </summary>
public sealed class M2ConfigMonsterRunConfig : IMonsterRunConfig
{
    public bool boPetQuickPickup => M2Config.boPetQuickPickup;
    public bool boEnabledPetPickup => M2Config.boEnabledPetPickup;
    public bool boPetRangePickup => M2Config.boPetRangePickup;
    public bool boPetSleepControlBySlave => M2Config.boPetSleepControlBySlave;

    /// <summary>
    /// 原文 `g_nKey_UseClientPickItems`（`M2Share.pas:3887`：`g_nKey_UseClientPickItems: Integer = 1;`
    /// —— 位于 `{$IF NEED_KEY = 1}`（正式发布版，`NEED_KEY = 1`）分支内）。
    /// <para><b>托管侧没有这个全局</b>：同名的 `g_nKey_UseClientPickItems` 是
    /// `ViewList2Form`（`ViewList2Form.Rules.cs:181`）与 `GamePetsForm`（`GamePetsForm.cs:266`）
    /// 各自的**实例字段**，两处初值也都是 **1**。故这里返回 **1**（= 原文正式版的默认值），
    /// 而不是 0 —— 后者会让 `Run` 的 1258 那道门在生产路径上**恒假**、把
    /// `TSmartObject` 范围拾取整段变成死代码（登记见报告 §8-3 / D-P16-09）。</para>
    /// <para>⚠ 待窗体/全局切片决定如何把"键控"暴露成真正的全局后，本属性应改为读它。</para>
    /// </summary>
    public int g_nKey_UseClientPickItems => 1;

    public bool IsCustomMonster(TMonster self) => false;
    public bool IsNoMoveCustomMonster(TMonster self) => false;
    public int GetMinAttackNearRange(TMonster self) => 1;
}

/// <summary>
/// `TMonster.Run` 走到而托管运行时侧**尚不存在**的被调方（各自的独立切片）。
/// <para>本接口只声明"Run 会调用它们"；实现（生产接线 / 测试替身）由调用方给出。
/// 未接线（`TMonster.RunDeps == null`）时 `Run` 按"该分支条件不成立"的语义走，
/// 并在 <see cref="TMonster.NotPortedClaims"/> 里如实登记（§48.1）。</para>
/// </summary>
public interface IMonsterRunWorld
{
    /// <summary>`TBaseObject.CanMove`（`ObjMon.pas:1128` 守卫）。</summary>
    bool CanMove(TMonster self);

    /// <summary>`TMonster.Think`（`ObjMon.pas:845-886`；`Run` 于 1144 调用）。</summary>
    bool Think(TMonster self);

    /// <summary>`TMonster.AttackTarget`（`ObjMon.pas:888-932`；`Run` 于 1198 调用）。</summary>
    bool AttackTarget(TMonster self);

    /// <summary>`TBaseObject.SpaceMove`（`ObjMon.pas:1140/1235/1323`）。</summary>
    void SpaceMove(TMonster self, string sMapName, int nX, int nY, int nInt);

    /// <summary>`TBaseObject.PickRangeItem`（`ObjMon.pas:1166/1242/1262`）。</summary>
    bool PickRangeItem(TMonster self, bool boSlaveAutoPickAll, bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime);

    /// <summary>`TBaseObject.StartPickUpItem`（`ObjMon.pas:1169/1245/1266`）。</summary>
    bool StartPickUpItem(TMonster self, bool boAutoPickPlayDropItem, bool boAutoPickPlayScatterItem, uint dwAutoPickScatterToPickTime);

    /// <summary>
    /// 1169 的**四参形态**被走到时的通知（原文该处调用恒为 `(True, True, 0, False)` 且丢弃返回值）。
    /// 只为让测试把两个调用点分开计数；实现方可以空实现。
    /// </summary>
    void NoteFourArgStartPickUpItem(TMonster self);

    /// <summary>`TBaseObject.DelTargetCreat`（`ObjMon.pas:1188/1285`）。</summary>
    void DelTargetCreat(TMonster self);
}
