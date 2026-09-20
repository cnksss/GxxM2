// ============================================================================
// 源单元：Source\M2Engine\ObjBase.pas（实测 43,480 LF，GBK）
// 本文件：**TCreature / TBaseObject 基类成员片**（切片 3 的基类部分）。
// 覆盖原文行号范围：
//   ObjBase.pas:101     m_btDirection: Byte;
//   ObjBase.pas:162     m_wAppr: Word;
//   ObjBase.pas:163     m_btRaceServer: Byte;   ← 托管侧经 MagicModel.cs:52 已存在，不重复声明
//   ObjBase.pas:164     m_btRaceImg: Byte;      ← 托管侧 ObjBase.cs:21 已存在，不重复声明
//   ObjBase.pas:293     m_LastHiter: TBaseObject;
//   ObjBase.pas:357     m_CurrTarget: TBaseObject;
//   ObjBase.pas:362     m_ActorIcons: TActorIconArray;
//   ObjBase.pas:645-647 function GetPoseCreate(): TBaseObject; overload ×3
//   ObjBase.pas:679-680 procedure TurnTo(nDir: Integer); / TurnToEx
//   ObjBase.pas:768     procedure Initialize(); virtual; // FFFE
//   ObjBase.pas:778     procedure RecalcAbilitys(); virtual;  // FFF7
//   ObjBase.pas:681     procedure FeatureChanged();
//   ObjBase.pas:586     procedure SendRefMsg(wIdent; wParam..nParam3: NativeInt; sMsg; dwDelay: LongWord = 0);
//   ObjBase.pas:11473-11478  m_ActorIcons 初始化（nFileIndex := -1; nIconCount := 1）
//   ObjBase.pas:26888-26894  TBaseObject.GetPoseCreate（无参）
//   ObjBase.pas:26896-26902  TBaseObject.GetPoseCreate(BaseObject)
//   ObjBase.pas:26904-...    TBaseObject.GetPoseCreate(AStep: Cardinal)
//   ObjBase.pas:32881-32906  TBaseObject.Initialize
//   ObjBase.pas:32929-32932  TBaseObject.FeatureChanged
//   ObjBase.pas:33382-33392  TBaseObject.TurnTo / TurnToEx
//   ObjBase.pas:30980-...    TBaseObject.SendRefMsg
//
// ⚠ 不重复声明的既有成员（git grep 证据见交付报告）：
//   · m_btRaceServer → Engine/MagicModel.cs:52（TCreature 经 TSpellCaster 继承面）
//   · m_btRaceImg    → Engine/ObjBase.cs:21
//   · m_btDirection  → Engine/ObjBase.cs:19
//   · m_PEnvir       → Engine/ObjBase.cs:24
// ============================================================================

using System;
using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>
/// 原文 `TActorIconArray`（Grobal2.pas:3284）= `array [0 .. MAX_ICON_COUNT - 1] of TActorIcon`，
/// `MAX_ICON_COUNT = 10`（Grobal2.pas:50「顶戴花翎数量」）。
/// 元素类型复用 `GXX.Core.Protocol.TActorIcon`（Grobal2.Types1.cs:213），**未造第二份**。
/// </summary>
public static class TActorIconArrayConst
{
    /// <summary>原文 `MAX_ICON_COUNT = 10;`（Grobal2.pas:50）。</summary>
    public const int MAX_ICON_COUNT = 10;
}

/// <summary>
/// `TBaseObject` 的接缝（原文成员在 `TBaseObject` / `TSmartObject` 上，托管侧尚未切出这两层，
/// 只存在 `TCreature` 这一层）。按任务书第 4 条，托管侧把成员落在 `TCreature` 上并在此登记偏差。
/// </summary>
public static class PlayerSurfaceBaseSeams
{
    /// <summary>
    /// `IsCopyMon` 等 `Self is TCopyMon` 判断（ObjBase.pas:32914 的 `not(Self is TCopyMon)`）。
    /// 托管侧暂无 `TCopyMon`。默认 false。
    /// </summary>
    public static Func<TCreature, bool> IsCopyMon { get; set; } = _ => false;

    /// <summary>
    /// `m_PEnvir.GetMovingObject(nX, nY, boFlag)`（ObjBase.pas:26893/26901）——
    /// `Envir.cs` 目前只有 `GetObjects(nX,nY)`（**没有 `boFlag`/`BaseObject` 过滤重载**）。
    /// 接缝：待 `TEnvirnoment.GetMovingObject` 移植后接入。
    /// 默认实现用既有的 `GetObjects` 找第一个 `TCreature`。
    /// </summary>
    public static Func<TEnvirnoment, int, int, TCreature?, bool, TCreature?> GetMovingObject { get; set; }
        = (env, x, y, exclude, _) =>
        {
            foreach (var o in env.GetObjects(x, y))
            {
                if (o is TCreature c && !ReferenceEquals(c, exclude)) return c;
            }
            return null;
        };

    /// <summary>`m_PEnvir.AddToMap(m_nCurrX, m_nCurrY, Self)` 的返回（ObjBase.pas:32899）。</summary>
    public static Func<TEnvirnoment, int, int, TCreature, bool> AddToMap { get; set; }
        = (env, x, y, obj) => env.AddToMap(x, y, obj);

    /// <summary>`GetCharStatus()`（ObjBase.pas:32901）——未移植；接缝返回 0。</summary>
    public static Func<TCreature, int> GetCharStatus { get; set; } = _ => 0;

    /// <summary>`AddBodyLuck(0)`（ObjBase.pas:32902）——未移植；接缝默认空操作。</summary>
    public static Action<TCreature, int> AddBodyLuck { get; set; } = (_, _) => { };

    /// <summary>`LoadSayMsg()`（ObjBase.pas:32903 / 32910-32922）——依赖 `g_MonSayMsgList`；默认空操作。</summary>
    public static Action<TCreature> LoadSayMsg { get; set; } = _ => { };

    /// <summary>`MonsterSayMsg(nil, s_MonGen)`（ObjBase.pas:32905）——默认空操作。</summary>
    public static Action<TCreature> MonsterSayMsg { get; set; } = _ => { };

    /// <summary>`SendRefMsg` 的实际下发（ObjBase.pas:30980）。默认空操作（无宿主）。</summary>
    public static Action<TCreature, int, long, long, long, long, string, uint> SendRefMsg { get; set; }
        = (_, _, _, _, _, _, _, _) => { };

    /// <summary>`WeightChanged()`（ObjBase.pas:35605）——未移植；默认空操作。</summary>
    public static Action<TCreature> WeightChanged { get; set; } = _ => { };

    /// <summary>`GetMaxBagCount()`（ObjBase.pas:26738）默认 `DEF_MAX_BAG_ITEM`；
    /// `TPlayObject` 覆写它（ObjPlayer.pas:1264）。未移植配置时返回 0，由 <see cref="PlayerSurfaceConst.DEF_MAX_BAG_ITEM"/> 兜底。</summary>
    public static Func<TCreature, int> GetMaxBagCount { get; set; } = _ => PlayerSurfaceConst.DEF_MAX_BAG_ITEM;

    /// <summary>`AbilCopyToWAbil()`（ObjBase.pas:32876-32879 `m_WAbil := m_Abil;`）——
    /// 托管侧尚无 `m_Abil`，接缝默认空操作。</summary>
    public static Action<TCreature> AbilCopyToWAbil { get; set; } = _ => { };

    /// <summary>`TBaseObject.RecalcAbilitys`（ObjBase.pas:18178 起）的实现落点。
    /// 托管侧已有 1:1 实现：`Engine/RecalcChain.cs:32` 的扩展方法
    /// `RecalcAbilitysChain.RecalcAbilitys(this TPlayObject)`（等级基础 + 装备槽遍历 + 聚合并入 + clamp）。
    /// 默认实现即**转调该既有实现**（不复制、不重复）。</summary>
    public static Action<TCreature> RecalcAbilitys { get; set; }
        = self => { if (self is TPlayObject po) RecalcAbilitysChain.RecalcAbilitys(po); };

    /// <summary>`TBaseObject.Initialize` 内对 `m_MagicList` 的 `btLevel` 裁决
    /// （ObjBase.pas:32890-32895）——依赖 `TUserMagic.MagicInfo.TrainLevel`（未移植）。
    /// ⚠ 原文把这段写在 `TBaseObject.Initialize` 内部（用 `TSmartObject(Self).m_MagicList` 强转），
    /// 托管侧 `m_MagicList` 落在 `TPlayObject` 上，故本接缝由 `TPlayObject.Initialize`
    /// **覆写**基类后调用 —— 保持「原文那段代码在虚分派链上」这一事实。</summary>
    public static Action<TCreature> InitializeMagicLevelClamp { get; set; } = _ => { };

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        AbilCopyToWAbil = _ => { };
        RecalcAbilitys = self => { if (self is TPlayObject po) RecalcAbilitysChain.RecalcAbilitys(po); };
        InitializeMagicLevelClamp = _ => { };
        IsCopyMon = _ => false;
        GetMovingObject = (env, x, y, exclude, _) =>
        {
            foreach (var o in env.GetObjects(x, y))
            {
                if (o is TCreature c && !ReferenceEquals(c, exclude)) return c;
            }
            return null;
        };
        AddToMap = (env, x, y, obj) => env.AddToMap(x, y, obj);
        GetCharStatus = _ => 0;
        AddBodyLuck = (_, _) => { };
        LoadSayMsg = _ => { };
        MonsterSayMsg = _ => { };
        SendRefMsg = (_, _, _, _, _, _, _, _) => { };
        WeightChanged = _ => { };
        GetMaxBagCount = _ => PlayerSurfaceConst.DEF_MAX_BAG_ITEM;
    }
}

/// <summary>PlayerSurface 片用到的原文常量。</summary>
public static class PlayerSurfaceConst
{
    /// <summary>原文 `DEF_MAX_BAG_ITEM`（M2Share/Common 侧）——`TBaseObject.GetMaxBagCount` 的默认返回（ObjBase.pas:26740）。</summary>
    public const int DEF_MAX_BAG_ITEM = 48;

    /// <summary>原文 `U_WEAPON = 1; // 武器`（Grobal2.pas:102）。</summary>
    public const int U_WEAPON = 1;

    /// <summary>原文 `MAX_USE_ITEM_COUNT = 30; // 身上装备数量`（Grobal2.pas:51）。</summary>
    public const int MAX_USE_ITEM_COUNT = 30;

    /// <summary>原文 `RC_PLAYOBJECT`（Grobal2.pas）——托管侧同名常量。</summary>
    public const byte RC_PLAYOBJECT = Grobal2Const.RC_PLAYOBJECT;

    /// <summary>原文 `RC_HEROOBJECT`（Grobal2.pas）——托管侧同名常量。</summary>
    public const byte RC_HEROOBJECT = Grobal2Const.RC_HEROOBJECT;

    /// <summary>原文 `RC_PLAYMOSTER`（Grobal2.pas）——托管侧同名常量。</summary>
    public const byte RC_PLAYMOSTER = Grobal2Const.RC_PLAYMOSTER;
}

/// <summary>
/// `TCreature` 的**基类成员片**（寄存器/字段 + `Initialize` / `GetPoseCreate` / `TurnTo` /
/// `SendRefMsg` / `FeatureChanged` / `RecalcAbilitys`）。
/// </summary>
/// <remarks>
/// ★ 虚分派：原文 `Initialize`（ObjBase.pas:768）与 `RecalcAbilitys`（:778）都是 **`virtual`**，
/// `FeatureChanged`（:681）原文**没有** `virtual` 关键字（但它被 `TBaseObject` 直接实现，
/// 且 `ObjPlayer.pas:10452` 等处直接调用）。台账 §18.8 的要求是「原文 virtual ⇒ 托管 virtual」，
/// 反向**不**自动成立 —— 故此处：
/// <list type="bullet">
///   <item><description><c>Initialize</c> → <c>public virtual void</c>（原文 :768 `virtual`）</description></item>
///   <item><description><c>RecalcAbilitys</c> → <c>public virtual void</c>（原文 :778 `virtual`）</description></item>
///   <item><description><c>FeatureChanged</c> → <c>public void</c>（原文 :681 **无** `virtual`，照抄不升级）</description></item>
///   <item><description><c>GetPoseCreate</c> → 非虚（原文 :645-647 无 `virtual`）</description></item>
///   <item><description><c>TurnTo</c>/<c>TurnToEx</c> → 非虚（原文 :679-680 无 `virtual`）</description></item>
/// </list>
/// </remarks>
public abstract partial class TCreature
{
    // ------------------------------------------------------------------
    // 字段
    // ------------------------------------------------------------------

    /// <summary>原文 `m_wAppr: Word;`（ObjBase.pas:162）——外形。</summary>
    public ushort m_wAppr;

    /// <summary>原文 `m_LastHiter: TBaseObject;`（ObjBase.pas:293）——最后攻击者。
    /// 托管侧 `TBaseObject` 未切出，用最薄的 <see cref="TCreature"/> 代表（同 ObjNpc 车道 §6.3 的口径）。</summary>
    public TCreature? m_LastHiter;

    /// <summary>原文 `m_CurrTarget: TBaseObject; // 当前对象`（ObjBase.pas:357）。</summary>
    public TCreature? m_CurrTarget;

    /// <summary>
    /// 原文 `m_ActorIcons: TActorIconArray;`（ObjBase.pas:362）= `array[0..9] of TActorIcon`
    /// （`MAX_ICON_COUNT = 10`，Grobal2.pas:50/:3284）。
    /// 原文在 `ClearObject` 里（ObjBase.pas:11473-11478）：
    /// `FillChar(m_ActorIcons, SizeOf(m_ActorIcons), 0)`，然后逐元素
    /// `nFileIndex := -1; nIconCount := 1;`。
    /// </summary>
    public TActorIcon[] m_ActorIcons = NewDefaultActorIcons();

    /// <summary>`m_boAddtoMapFail`（ObjBase.pas:32898/:32900）——`Initialize` 的落地标志。</summary>
    public bool m_boAddtoMapFail;

    /// <summary>`m_nCharStatus`（ObjBase.pas:32901 由 `GetCharStatus()` 赋值）。</summary>
    public int m_nCharStatus;

    /// <summary>
    /// 原文 `ClearObject` 里的 `m_ActorIcons` 初始化（ObjBase.pas:11473-11478）。
    /// 原文先 `FillChar(...,0)` 再逐元素设 `nFileIndex := -1; nIconCount := 1` ——
    /// 注意 `FillChar` 会把 `nFileIndex` 清零，随后的 `-1` 才是**有效默认值**（$FFFF）。
    /// </summary>
    public static TActorIcon[] NewDefaultActorIcons()
    {
        // 原文 11473：FillChar(m_ActorIcons, SizeOf(m_ActorIcons), 0);
        var icons = new TActorIcon[TActorIconArrayConst.MAX_ICON_COUNT];
        // 原文 11474-11478：for I := 0 to Length(m_ActorIcons) - 1 do ... -1 / 1
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].nFileIndex = -1;   // 原文 11476
            icons[i].nIconCount = 1;    // 原文 11477
        }
        return icons;
    }

    // ------------------------------------------------------------------
    // 虚方法：Initialize / RecalcAbilitys（原文 virtual）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `procedure TBaseObject.Initialize(); virtual; // FFFE`（ObjBase.pas:768 声明，
    /// :32881-32906 实现）。
    /// ★ **原文是虚方法**（台账 §18.8）——下游 `TBoxMonster.Initialize`(ObjNpc.pas:10521-10525)、
    /// `TNormNpc.Initialize`(:9864-9875)、`TCastleOfficial`/`TGuildOfficial` 的 `Initialize`
    /// 全部走 `inherited Initialize;`，故托管侧**必须**是 `virtual`。
    /// </summary>
    /// <remarks>
    /// 逐行对照 ObjBase.pas:32886-32905：
    /// <code>
    ///   AbilCopyToWAbil();                                  // 32886
    ///   if m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then  // 32888
    ///     ... m_MagicList 逐条裁决 btLevel ...              // 32890-32895
    ///   m_boAddtoMapFail := True;                            // 32898
    ///   if m_PEnvir.CanWalk(m_nCurrX, m_nCurrY, True) and AddToMap() then  // 32899
    ///     m_boAddtoMapFail := False;                         // 32900
    ///   m_nCharStatus := GetCharStatus();                    // 32901
    ///   AddBodyLuck(0);                                      // 32902
    ///   LoadSayMsg();                                        // 32903
    ///   if g_Config.boMonSayMsg then MonsterSayMsg(nil, s_MonGen);  // 32904-32905
    /// </code>
    /// **接缝**（未移植 / 未切层的依赖）：`AbilCopyToWAbil`、`m_MagicList` 的 `pTUserMagic`
    /// 细节、`CanWalk(...,True)` 的三参重载、`AddToMap()`、`GetCharStatus`、`AddBodyLuck`、
    /// `LoadSayMsg`、`MonsterSayMsg`。结构（分支顺序、默认值、布尔初值）逐条保留，
    /// 宿主能力经 <see cref="PlayerSurfaceBaseSeams"/> 注入；默认实现即「无宿主」。
    /// ⚠ 原文 32899 **不判 `m_PEnvir = nil`** —— 托管 `m_PEnvir` 是可空引用，
    /// 此处按「原文语义优先」在 `m_PEnvir == nil` 时只置 `m_boAddtoMapFail = True` 并跳过
    /// `CanWalk`/`AddToMap`（避免把原文的空指针 AV 变成托管 NRE 打断整个 Initialize 流程；
    /// 该偏差已登记在报告「原文缺陷 / 易错点」）。
    /// </remarks>
    public virtual void Initialize()
    {
        // 原文 32886：AbilCopyToWAbil();
        AbilCopyToWAbil();

        // 原文 32888：if m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
        if (m_btRaceServer == Grobal2Const.RC_PLAYOBJECT
            || m_btRaceServer == Grobal2Const.RC_HEROOBJECT
            || m_btRaceServer == Grobal2Const.RC_PLAYMOSTER)
        {
            // 原文 32890-32895：遍历 m_MagicList，把 btLevel 超过 MagicInfo.TrainLevel 上限者归零。
            // ⚠ 原文用 `TSmartObject(Self).m_MagicList` 在**基类内部**强转访问，说明该段语义
            // 属于基类流程；但托管 `m_MagicList` 落在 `TPlayObject`（`ObjBase.cs:167`）。
            // 为**不重复执行**，该段只在 `TPlayObject.Initialize` 的 override 里执行一次
            // （见 TPlayObject.PlayerSurface.NpcSession.cs）—— 此处保留 `if` 结构以标记位置，
            // 不再调用接缝。这样对 `TCreature` 的其他派生（怪/NPC）行为不变（原文对它们也是空转），
            // 对 `TPlayObject` 恰好执行一次（原文语义）。
            //
            // 接缝：PlayerSurfaceBaseSeams.InitializeMagicLevelClamp（由 TPlayObject 覆写调用）
        }

        // 原文 32898：m_boAddtoMapFail := True;
        m_boAddtoMapFail = true;

        // 原文 32899：if m_PEnvir.CanWalk(m_nCurrX, m_nCurrY, True) and AddToMap() then
        if (m_PEnvir != null)
        {
            bool canWalk = m_PEnvir.CanWalk(m_nCurrX, m_nCurrY);   // 接缝：三参重载 (nX,nY,boFlag)
            bool added = canWalk && PlayerSurfaceBaseSeams.AddToMap(m_PEnvir, m_nCurrX, m_nCurrY, this);
            if (added)
                m_boAddtoMapFail = false;                          // 原文 32900
        }

        // 原文 32901：m_nCharStatus := GetCharStatus();
        m_nCharStatus = PlayerSurfaceBaseSeams.GetCharStatus(this);

        // 原文 32902：AddBodyLuck(0);
        PlayerSurfaceBaseSeams.AddBodyLuck(this, 0);

        // 原文 32903：LoadSayMsg();
        PlayerSurfaceBaseSeams.LoadSayMsg(this);

        // 原文 32904-32905：if g_Config.boMonSayMsg then MonsterSayMsg(nil, s_MonGen);
        if (M2Config.boMonSayMsg)
            PlayerSurfaceBaseSeams.MonsterSayMsg(this);
    }

    /// <summary>
    /// 原文 `AbilCopyToWAbil()`（ObjBase.pas:32876-32879：`m_WAbil := m_Abil;`）。
    /// 托管侧只有 `m_wAbil`（`ObjBase.cs:34`），`m_Abil` 未切出 —— 接缝为「不动作」，
    /// 由宿主在切出 `m_Abil` 后替换。
    /// </summary>
    public virtual void AbilCopyToWAbil()
    {
        // 原文 32878：m_WAbil := m_Abil;
        PlayerSurfaceBaseSeams.AbilCopyToWAbil(this);
    }

    /// <summary>
    /// 原文 `procedure TBaseObject.RecalcAbilitys(); virtual; // FFF7`（ObjBase.pas:778 声明）。
    /// 托管侧已有 `Engine/RecalcAbilitys.cs:11` 的**静态** `RecalcAbilitys` 组件与
    /// `Engine/RecalcChain.cs:98` 的 `TPlayObject` 扩展 —— 本车道**不重复实现**，
    /// 只把原文的**虚方法位置**补齐（否则下游 `User.RecalcAbilitys()` 无处可调）。
    /// </summary>
    /// <remarks>
    /// ★ 保留虚分派：原文 `TPlayObject.RecalcAbilitys`（ObjPlayer.pas:1225）是 `override`。
    /// 基类实现在原文 :18178-... 是 1000+ 行的装备/组套装聚合（尚未移植），
    /// 此处**只落虚方法外壳**并转发接缝；这是「接缝」而非「纯委托降级」——
    /// 子类 `override` 后 `inherited RecalcAbilitys()` 仍能回到这层。
    /// </remarks>
    public virtual void RecalcAbilitys()
    {
        // 原文 ObjBase.pas:18178 起的实现未移植（依赖 m_UseItems 全量 GetAccessory + g_Config）。
        // 接缝：宿主把它接到 `Engine.RecalcChain` 的既有实现上。
        PlayerSurfaceBaseSeams.RecalcAbilitys(this);
    }

    // ------------------------------------------------------------------
    // 位置/朝向
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `function TBaseObject.GetPoseCreate: TBaseObject;`（ObjBase.pas:645 声明，
    /// :26888-26894 实现）：
    /// <code>
    ///   GetFrontPosition(nX, nY);
    ///   Result := m_PEnvir.GetMovingObject(nX, nY, True);
    /// </code>
    /// ⚠ 原文在 :645-647 有**三个重载**且**都不是 `virtual`**；本车道落其中被 ObjNpc 四优先方法
    /// 需要的无参版本 + `BaseObject` 版本 + `AStep` 版本（三个都照抄签名）。
    /// </summary>
    public TCreature? GetPoseCreate()
    {
        // 原文 26892：GetFrontPosition(nX, nY);
        GetFrontPosition(out int nX, out int nY);
        // 原文 26893：Result := m_PEnvir.GetMovingObject(nX, nY, True);
        if (m_PEnvir == null) return null;
        return PlayerSurfaceBaseSeams.GetMovingObject(m_PEnvir, nX, nY, null, true);
    }

    /// <summary>原文 `function TBaseObject.GetPoseCreate(BaseObject: TBaseObject): TBaseObject;`（ObjBase.pas:646/:26896-26902）。</summary>
    public TCreature? GetPoseCreate(TCreature? baseObject)
    {
        // 原文 26900：GetFrontPosition(nX, nY);
        GetFrontPosition(out int nX, out int nY);
        // 原文 26901：Result := m_PEnvir.GetMovingObject(nX, nY, BaseObject, True);
        if (m_PEnvir == null) return null;
        return PlayerSurfaceBaseSeams.GetMovingObject(m_PEnvir, nX, nY, baseObject, true);
    }

    /// <summary>
    /// 原文 `function TBaseObject.GetPoseCreate(AStep: Cardinal): TBaseObject;`
    /// （ObjBase.pas:647 声明，:26904 起实现「增加指定步数内获取对象 Cursor 2023-07-03」）。
    /// </summary>
    /// <remarks>
    /// 原文 :26909 先置 `Result := nil`，随后 `tmpX/tmpY := m_nCurrX/m_nCurrY`，
    /// 再 `for I := 0 to AStep - 1 do` 沿朝向逐步前进取 `m_PEnvir.GetMovingObject(...)`。
    /// ⚠ `AStep = 0` 时 Delphi 的 `for I := 0 to -1` **一次都不执行**，直接返回 `nil` ——
    /// 托管 `for (int i = 0; i &lt; 0; i++)` 同样零次，**等价**（差异断言已锁）。
    /// 接缝：`GetFrontPosition` 的单步版本依赖 `m_btDirection` 方向表，已有（`ObjBase.cs:122-127`）。
    /// </remarks>
    public TCreature? GetPoseCreate(uint aStep)
    {
        // 原文 26909：Result := nil;
        TCreature? result = null;
        // 原文 26910-26911：tmpX := m_nCurrX; tmpY := m_nCurrY;
        int tmpX = m_nCurrX;
        int tmpY = m_nCurrY;
        // 原文 26912：for I := 0 to AStep - 1 do
        for (int i = 0; i < (long)aStep; i++)
        {
            // 原文循环体：按 m_btDirection 前进一步后取 m_PEnvir.GetMovingObject
            tmpX += DirToX(m_btDirection);
            tmpY += DirToY(m_btDirection);
            if (m_PEnvir == null) break;
            var hit = PlayerSurfaceBaseSeams.GetMovingObject(m_PEnvir, tmpX, tmpY, null, true);
            if (hit != null)
            {
                result = hit;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 原文 `GetFrontPosition(var nX, nY: Integer)`（ObjBase.pas 内 TBaseObject）——
    /// 按 `m_btDirection` 取身前一格。托管侧未切出该方法，此处由 `DirToX/DirToY` 组合出等价语义。
    /// </summary>
    public void GetFrontPosition(out int nX, out int nY)
    {
        nX = m_nCurrX + DirToX(m_btDirection);
        nY = m_nCurrY + DirToY(m_btDirection);
    }

    /// <summary>
    /// 原文 `procedure TBaseObject.TurnTo(nDir: Integer);`（ObjBase.pas:679 声明，
    /// :33382-33386 实现）：
    /// <code>
    ///   m_btDirection := nDir;
    ///   SendRefMsg(RM_TURN, nDir, m_nCurrX, m_nCurrY, 0, '');
    /// </code>
    /// ⚠ 原文**没有** `virtual`；`nDir` 是 `Integer` 而 `m_btDirection` 是 `Byte`
    /// —— 原文靠 Delphi 隐式窄化（>`255` 会静默截断），托管侧用 `unchecked((byte)nDir)` 复刻。
    /// </summary>
    public void TurnTo(int nDir)
    {
        // 原文 33384：m_btDirection := nDir;
        m_btDirection = unchecked((byte)nDir);
        // 原文 33385：SendRefMsg(RM_TURN, nDir, m_nCurrX, m_nCurrY, 0, '');
        SendRefMsg(Grobal2Const.RM_TURN, nDir, m_nCurrX, m_nCurrY, 0, "", 0);
    }

    /// <summary>原文 `procedure TBaseObject.TurnToEx(nDir: Integer);`（ObjBase.pas:680/:33388-33392）。</summary>
    public void TurnToEx(int nDir)
    {
        // 原文 33390：m_btDirection := nDir;
        m_btDirection = unchecked((byte)nDir);
        // 原文 33391：SendRefMsg(RM_TURN_EX, nDir, m_nCurrX, m_nCurrY, 0, '');
        SendRefMsg(Grobal2Const.RM_TURN_EX, nDir, m_nCurrX, m_nCurrY, 0, "", 0);
    }

    // ------------------------------------------------------------------
    // 消息
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `procedure TBaseObject.SendRefMsg(wIdent: Integer; wParam, nParam1, nParam2, nParam3:
    /// NativeInt; sMsg: AnsiString; dwDelay: LongWord = 0);`（ObjBase.pas:586 声明，:30980 实现）。
    /// 实现体（RM_FEATURECHANGED / RM_ITEMSHOW 的参数改写 + 按视野逐个玩家下发）依赖
    /// `TEnvirnoment` 视野遍历与 `TDefaultMessage` 编码，**未移植** → 落最小接缝。
    /// ★ 签名按原文：`Integer, NativeInt×4, AnsiString, LongWord = 0`。
    /// 托管侧 `NativeInt` → `long`、`AnsiString` → `string`。
    /// </summary>
    public void SendRefMsg(int wIdent, long wParam, long nParam1, long nParam2, long nParam3,
        string sMsg, uint dwDelay = 0)
    {
        PlayerSurfaceBaseSeams.SendRefMsg(this, wIdent, wParam, nParam1, nParam2, nParam3, sMsg, dwDelay);
    }

    /// <summary>
    /// 原文 `procedure TBaseObject.FeatureChanged;`（ObjBase.pas:681 声明，:32929-32932 实现）：
    /// `SendRefMsg(RM_FEATURECHANGED, 0, 0, 0, 0, '');`
    /// ⚠ 原文 :681 **没有** `virtual` —— 托管侧同样**不标** `virtual`（照抄可见性/虚分派）。
    /// </summary>
    public void FeatureChanged()
    {
        // 原文 32931
        SendRefMsg(Grobal2Const.RM_FEATURECHANGED, 0, 0, 0, 0, "", 0);
    }
}
