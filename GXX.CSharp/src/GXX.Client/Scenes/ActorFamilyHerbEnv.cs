using System;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// 车道 `p7-client-actor-family`：移植 <c>HerbActor.pas</c> 族子类时**新需要的字段与成员**
/// （以 <c>partial</c> 扩展 <c>TActorCore</c>，零改既有文件），以及各子类依赖的**运行时接缝**
/// （放在同一个类里，便于一处审阅）。
///
/// <para>字段部分在原文里都是 <c>TActor</c> 的实例成员，但托管侧此前不需要它们
/// （要么没移植、要么由别的接缝承载），故集中在此登记，避免散落。</para>
/// </summary>
public static partial class ActorFamilyHerbEnv
{
    // ──────────────────────────────────────────────────────────────────────
    // 地图 / 场景写入口接缝（HerbActor 族的 Run 与 ApplyDoorState 依赖）
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`g_MySelf.m_nCurrX - Map.m_nBlockLeft`（`TCastleDoor.Run` 713-722 需要格坐标）。
    /// <para><b>接缝：待 <c>Map.m_nCurUnitX/m_nCurUnitY</c>（GXX.Client 地图层）移植后接入</b>；
    /// 默认 <c>-1/-1</c> 表示"地图未接线"—— 与"格坐标恰好为 -1"无法区分，
    /// 故改动检测在未接线时**总会命中**（等价于原文"地图变了"），这是保守且不静默的方向。</para>
    /// </summary>
    public static Func<int> MapCurUnitXFn = () => -1;

    /// <summary>见 <see cref="MapCurUnitXFn"/>。</summary>
    public static Func<int> MapCurUnitYFn = () => -1;

    /// <summary>`Map.MarkCanWalk(x, y, walk)`（`TCastleDoor.ApplyDoorState` 537-559 /
    /// `TWallStructure.Run` 956/962 的唯一地图写入口）。
    /// <para><b>接缝：待 <c>Map.MarkCanWalk</c>（GXX.Client 地图层）移植后接入</b>。</para>
    /// </summary>
    public static Action<int, int, bool> MapMarkCanWalkFn = (_, _, _) => { };

    /// <summary>`PlayScene.SetActorDrawLevel(Self, 0)`（`TWallStructure.Run` 966 / `TNewWallStructure.Run` 1160）。</summary>
    public static Action<TActorCore, int> SetActorDrawLevelFn = (_, _) => { };

    /// <summary>
    /// `PlayScene.LoadSurface(LoadSurface)`（原文 7645 / 7718 / 8001 / 8070 / 8352 等）——
    /// 即"把本角色的 <c>LoadSurface</c> 方法交给场景做**延迟装载**"。
    /// <para><b>接缝：待 <c>PlayScene.LoadSurface</c> 的延迟装载队列移植后接入</b>
    /// （托管侧 <c>PlaySceneNewActor.cs</c> 有独立的 <c>LoadSurfaceCount</c> 计数型承载）。</para>
    /// </summary>
    public static Action<TActorCore, Action> RequestLoadSurfaceFn = (_, _) => { };

    // ──────────────────────────────────────────────────────────────────────
    // 取图接缝（原文都用 `g_WMonImages[...]` 或 `g_WDragonImg` 的 GetCached*）
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`DOORDEATHEFFECTBASE = 120;`（原文 `HerbActor.pas:21`）。</summary>
    public const int DoorDeathEffectBase = 120;

    /// <summary>`WALLLEFTBROKENEFFECTBASE = 224;`（原文 `HerbActor.pas:22`）。</summary>
    public const int WallLeftBrokenEffectBase = 224;

    /// <summary>`WALLRIGHTBROKENEFFECTBASE = 240;`（原文 `HerbActor.pas:23`）。
    /// <para>★ 注意：本单元**没有任何地方**使用它（原文 22/23 两个常量都定义了，
    /// 但 832-1162 只用到 <c>WALLLEFTBROKENEFFECTBASE</c>）—— <c>HerbActor.cs</c> 已记载该常量，
    /// 此处保留常数以对齐常量表，**不虚构使用点**。</para>
    /// </summary>
    public const int WallRightBrokenEffectBase = 240;

    /// <summary>`TCastleDoor.LoadSurface` 575-578 的 `mimg.GetCached*(DOORDEATHEFFECTBASE + …)`。</summary>
    public static Func<int, int, int, int, string, object?> FetchDoorEffectFn = (_, _, _, _, _) => null;

    /// <summary>`TWallStructure/TNewWallStructure.LoadSurface` 856-859 / 1073-1076 的死亡帧主体图。</summary>
    public static Func<int, int, int, int, string, object?> FetchWallBodyFn = (_, _, _, _, _) => null;

    /// <summary>`…LoadSurface` 882-885 / 1089-1092 的破碎图。</summary>
    public static Func<int, int, int, int, string, object?> FetchWallBrokenFn = (_, _, _, _, _) => null;

    /// <summary>`…LoadSurface` 903-906 / 1098-1101 的破碎特效图。</summary>
    public static Func<int, int, int, int, string, object?> FetchWallEffectFn = (_, _, _, _, _) => null;

    /// <summary>
    /// `g_WMonImages.Indexs[15]` 的取图（`TCentipedeKingMon.LoadEffect` 1189-1198 的图库）。
    /// <para><b>接缝：待 <c>g_WMonImages.Indexs[15]</c> 图库接缝接入</b> ——
    /// 既有 <c>ActorFamilyEnv.FetchBodySurfaceFn</c> 只按 <c>ActorBodyImage</c> 取图，
    /// 不表达"图集下标"这一维度。返回 <c>null</c> = 未接线（⇒ <c>AttackEffectSurface</c> 保持 nil，
    /// 与原文 `d = nil` 的存在性门同形）。</para>
    /// </summary>
    public static Func<int, int, int, string, object?> FetchMonImagesIndex15Fn = (_, _, _, _) => null;

    /// <summary>
    /// `g_WDragonImg.GetCachedImage/Gray/Bright(idx, m_nPx, m_nHpy)`（`TDragonBody.LoadSurface`
    /// 1331-1334）。
    /// <para><b>接缝：待 <c>g_WDragonImg</c> 图库接入</b>。返回 <c>null</c> = 未接线
    /// （⇒ <c>m_BodySurface</c> 保持 nil，与原文 <c>mimg = nil</c> 时"不赋图"同形）。</para>
    /// </summary>
    public static Func<int, int, int, string, object?> FetchDragonImgFn = (_, _, _, _) => null;

    /// <summary>
    /// `PlayScene.NewMagic(Self, 111, m_nEffectNum, m_nCurrX, m_nCurrY, m_nTargetX, m_nTargetY,
    /// m_nTargetRecog, m_MagicType, True, 0, bofly)`（原文 211-222 / 347-358 / 474-485）+
    /// 随后的 `if bofly then PlaySound(m_nMagicFireSound) else PlaySound(m_nMagicExplosionSound)`。
    ///
    /// <para><b>接缝：待 <c>PlayScene.NewMagic</c>（魔法特效实例化）移植后接入</b> ——
    /// 该函数会按飞行/爆裂两态返回 <c>bofly</c> 并同时产生特效入列副作用，
    /// 托管侧尚未有等价物（<c>MagicEffects*</c> 族是另一批）。
    /// 返回 <c>null</c> = 未接线（此时**不画特效也不播音**，等价于"原文走不到该段"）。</para>
    /// </summary>
    public static Func<TActorCore, bool?>? PlaySceneNewMagicFn;

    // 本类只承载**静态接缝与常量**；实例字段在下方 TActorCore 的 partial 扩展里。
}

/// <summary>
/// `HerbActor.pas` 族子类本体需要的**实例字段与虚成员**（partial 扩展 <c>TActorCore</c>）。
/// </summary>
public partial class TActorCore
{
    // ──────────────────────────────────────────────────────────────────────
    // HerbActor 族 CalcActorFrame / LoadSurface 直接读写
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// `m_nHpy`（`TDragonBody.LoadSurface` 1317-1339 的取图 y 偏移；
    /// 与既有 `m_nPy` 成对，原文里是两个独立字段）。
    /// </summary>
    public int m_nHpy;

    /// <summary>`m_nEffectNum`（`TKillingHerb` 211 / `TBeeQueen` 347 / `TCentipedeKingMon` 474 的
    /// `PlayScene.NewMagic` 第 3 实参）。</summary>
    public int m_nEffectNum;

    /// <summary>
    /// `m_EffectFrame` / `BoUseDieEffect` / `m_nEffectStart`（`TCentipedeKingMon` 的
    /// 464-468 / 498-502 死亡特效三段）。
    /// <para><c>m_nEffectStart</c> 已在 <c>ActorFamilyEnv.cs</c> 声明，此处只补另两个。</para>
    /// </summary>
    public bool BoUseDieEffect;
    public int m_EffectFrame;

    /// <summary>`m_nState` 的 `STATE_STONE_MODE` 位（原文 5512 的石化门）。</summary>
    // （StateStoneMode 已在 ActorFamilyEnv.cs 定义，此处不重复）

    // ──────────────────────────────────────────────────────────────────────
    // 子类 LoadSurface / DrawChr / Run 需要的**纹理槽**与**格坐标缓存**
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`EffectSurface`（`TCastleDoor` 76 / `TWallStructure` 94 的死亡/破碎特效图）。</summary>
    public object? EffectSurface;

    /// <summary>`BrokenSurface`（`TWallStructure` 94 的破碎图）。</summary>
    public object? BrokenSurface;

    /// <summary>`AttackEffectSurface`（`TCentipedeKingMon` 52 的攻击特效图）。</summary>
    public object? AttackEffectSurface;

    /// <summary>`deathframe`（`TWallStructure` 100 / `TNewWallStructure` 117）。</summary>
    public int deathframe;

    /// <summary>`bomarkpos`（`TWallStructure` 100 / `TNewWallStructure` 117 的可走标记位）。</summary>
    public bool bomarkpos;

    /// <summary>`ax` / `ay`（墙系与门的特效绘制偏移；`TCentipedeKingMon` 亦用）。</summary>
    public int ax;
    public int ay;

    /// <summary>`bx` / `by`（墙系破碎图绘制偏移）。</summary>
    public int bx;
    public int by;

    /// <summary>`oldunitx` / `oldunity`（`TCastleDoor.Run` 713-722 的格坐标变化检测）。</summary>
    public int oldunitx;
    public int oldunity;

    /// <summary>`BoDoorOpen`（`TCastleDoor` public 字段；583-699 多处读写）。</summary>
    public bool BoDoorOpen;

    // 注意：`m_sUserName` 已在 PlaySceneActors.cs 声明（原文 605/785/1012 置 ' '）—— 不重复声明。

    // ──────────────────────────────────────────────────────────────────────
    // 本体依赖的成员（原文 TActor 的方法，托管侧此前以别的名字/接缝存在）
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// `TActor.ActionEnded`（原文 1822 声明 `<c>procedure ActionEnded; virtual;</c>`，实体 7106-7118）。
    /// <para>托管侧已有 <c>RunActionEnded()</c>（ActorMotion.cs:310，1:1 语义）承载同一实体；
    /// 本虚成员只是把"原文名"接上，供 <c>TCastleDoor.ActionEnded</c>（701-709）覆写。</para>
    /// <para><b>注意与 <c>TCustomActor</c> 的关系</b>：它走的是自己的 <c>OnActionEnded</c> 钩子
    /// （<c>CustomActor.cs</c>），与本虚成员无冲突。</para>
    /// </summary>
    public virtual void ActionEnded() => RunActionEnded();

    /// <summary>
    /// `TActor.Finalize`（原文 1848 声明；`TCastleDoor.Finalize` 527 / `TWallStructure.Finalize` 756
    /// 等覆写它）。
    /// <para>基类为空实现（原文的 <c>TActor.Finalize</c> 释放纹理槽，托管侧无纹理对象）。
    /// 注意 <c>TCustomActor</c> 另有自己的 <c>Finalize</c>（它继承自 <c>TGameConfigObject</c> 那个名字，
    /// 属不同族），故此处<b>不</b>把它做成会冲突的名字，而是照原文名 <c>Finalize</c> 落在 <c>TActorCore</c> 上
    /// —— 经核对，<c>TActorCore</c> 链上没有同名成员。</para>
    /// </summary>
    public virtual void Finalize() { }

    /// <summary>
    /// `TActor.LoadActorIcons`（原文 1839 声明；`TDragonBody.LoadSurface` 1337 调它）。
    /// <para><b>接缝：待 `TActor.LoadActorIcons`（顶戴花翎纹理装载）移植后接入</b> ——
    /// 它依赖 <c>m_ActorIcons</c>/<c>m_ActorIconIndexs</c> 数组与 <c>g_EffectImageList</c>。</para>
    /// </summary>
    public virtual void LoadActorIcons() { }

    /// <summary>
    /// `TActor.ActionChanged`（原文 7101-7104，**函数体为空**的虚钩子）。
    /// <para>托管侧已有 <c>ComputeActionChanged</c>（ActorMotion.cs:304，同样空实现）承载同一语义；
    /// 本属性只是把"原文名"接上，避免子类覆写时找不到对应成员。</para>
    /// </summary>
    public virtual void ActionChanged() => ComputeActionChanged();

    /// <summary>`g_MySelf.m_nCurrX - Map.m_nBlockLeft`（`TCastleDoor.Run` 713-722 需要格坐标）。
    /// <para><b>接缝：待 <c>Map.m_nCurUnitX/m_nCurUnitY</c>（GXX.Client 地图层）移植后接入</b>；
    /// 默认 <c>-1/-1</c> 表示"地图未接线"—— 与"格坐标恰好为 -1"无法区分，
    /// 故改动检测在未接线时**总会命中**（等价于原文"地图变了"），这是保守且不静默的方向。</para>
    /// </summary>
    public static Func<int> MapCurUnitXFn = () => -1;
    /// <summary>见 <see cref="MapCurUnitXFn"/>。</summary>
    public static Func<int> MapCurUnitYFn = () => -1;

    /// <summary>`SetMagicSound(m_nMagicNum)`（原文 210/346/473）——
    /// 计算 <c>m_nMagicStartSound/Fire/Explosion</c> 三元组并写回本实例。</summary>
    public void SetMagicSound()
    {
        var ids = ActorDrawDispatch.SetMagicSound(m_nMagicNum);
        if (ids == null)
            return;                                  // 原文：wMagicID <= 0 ⇒ 不设值

        m_nMagicStartSound = ids.Start;
        m_nMagicFireSound = ids.Fire;
        m_nMagicExplosionSound = ids.Explosion;
    }

    /// <summary>
    /// `TActor.DrawEff(dx, dy)`（原文 1833 声明 `<c>procedure DrawEff(dx, dy:Integer); virtual;</c>`）。
    /// <para>基类为空实现（原文 <c>TActor.DrawEff</c> 的实体在更下方，托管侧未移植）；
    /// `TCentipedeKingMon.DrawEff`（1177-1182）与 `TDragonBody.DrawEff`（1310-1315）覆写它。</para>
    /// </summary>
    public virtual void DrawEff(int dx, int dy) { }

    /// <summary>
    /// `TCentipedeKingMon.LoadEffect`（**1184-1200**）的基类槽位。
    /// <para>原文里它是 `TCentipedeKingMon` 的 **private** 方法（不是虚方法）；
    /// 托管侧以 virtual 落位，以便 `Run`/`LoadSurface` 能在基类里调用到子类实现。</para>
    /// </summary>
    public virtual void LoadEffect() { }

    /// <summary>
    /// `m_CustomMagicStatusEffect.m_nStruck := 0`（原文 245 / 381 / 624）。
    /// <para><c>TCustomActor</c> 用自己的 <c>m_CustomMagicStatusEffect_Struck</c>（同名字段隐藏），
    /// 那是它的私有状态；本字段供 HerbActor 族使用。</para>
    /// </summary>
    public int m_CustomMagicStatusEffect_Struck;

    /// <summary>`m_nMagicNum`（原文 209/345/472 的施法序号门）。</summary>
    public int m_nMagicNum;

    /// <summary>`m_MagicType`（原文 219/355/482 的 EffectType 实参）。</summary>
    public int m_MagicType;

    /// <summary>`m_nTargetX` / `m_nTargetY`（原文 216-217 等）。</summary>
    public int m_nTargetX;
    public int m_nTargetY;

    /// <summary>`m_nMagicFireSound` / `m_nMagicExplosionSound`（原文 224-226 的两态取声）。</summary>
    public int m_nMagicFireSound = -1;
    public int m_nMagicExplosionSound = -1;

    /// <summary>`m_nTargetRecog`（原文 218/354/481 的 <c>PlayScene.NewMagic</c> 实参）。
    /// <para>注意 <c>TCustomActor</c> 另有一份同名字段（<c>CustomActor.cs:1629</c>），它只在本类内用；
    /// 基类这一份只被 HerbActor 族读取，两者无交集，故保留同名字段（无需消重）。</para></summary>
    public long m_nTargetRecog;

    /// <summary>`m_btRace` 的只读镜像（原文 5500-5506 的线性查找按外观号，不涉种族）。</summary>
    // （无需额外成员）

    /// <summary>`GetOffset(appr)`（原文 176 / 323 / 449 / 602 / 682 / 782 / 923 / 1009 / 1117 等
    /// 的 <c>m_nBodyOffset</c> 与死亡帧基址来源）。</summary>
    public static int GetOffset(int appearance) => ActorOffsets.GetOffset(appearance);
}
