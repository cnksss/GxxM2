using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>
/// 一次 NPC / 雕像角色绘制调用（<c>GameCanvas.Draw*</c> 的 headless 镜像）。
///
/// <para><b>为什么不复用 <see cref="SurfaceDrawOp"/></b>：NPC 族的绘制**顺序即语义**
/// （<c>TNpcActor.DrawChr</c> 的六种 <c>DrawOrder</c>、<c>TStatuaryNpcActor.DrawChr</c> 的三层），
/// 而 <see cref="SurfaceDrawOp"/> 是"单张图怎么画"的形状，不带**层名**。本记录多带一个
/// <see cref="Layer"/> 与 <see cref="ReferenceLine"/>，使测试能按原文行号逐条夹住顺序 ——
/// 这是本族最容易"看起来一样实则不同"的地方。</para>
/// </summary>
public enum NpcDrawKind
{
    /// <summary><c>GameCanvas.Draw</c></summary>
    Draw,

    /// <summary><c>GameCanvas.DrawBlend</c></summary>
    DrawBlend,

    /// <summary><c>GameCanvas.StretchDraw</c></summary>
    StretchDraw,
}

/// <summary>见 <see cref="NpcDrawKind"/>。</summary>
public sealed record NpcDrawOp(
    NpcDrawKind Kind,
    int X,
    int Y,
    object? Surface,
    string Layer,
    int ReferenceLine,
    int DestWidth = 0,
    int DestHeight = 0);

/// <summary>取图结果：贴图 + 图集内偏移（Delphi <c>out OffsetPt</c> 的 headless 等价）。</summary>
public readonly record struct NpcImageFetch(object? Texture, int OffsetX, int OffsetY);

/// <summary>
/// 车道 `p17-client-actor`：`Actor.pas` 的 **NPC 族**
/// （<c>TNpcActor</c> 9936-11122 / <c>TStatuaryNpcActor</c> 17537-18008 / <c>THeroActor</c> 17437-17533）
/// 落地所需的运行时环境接缝。
///
/// <para><b>设计原则（台帐 §25.2）</b>：这里的接缝**没有一个是"静默返回中性值"的伪装层**。
/// 每个 <c>null</c> / 默认值都在注释里指明它对应原文的**哪一个分支**：</para>
/// <list type="bullet">
/// <item><see cref="CustomNpcConfigLookupFn"/> 的 <c>null</c> = 原文 <c>NpcConfig = nil</c>
///   ⇒ <c>CalcActorFrame</c> 的 <c>NpcDirAction</c> 保持 nil；这是**原文语义**；</item>
/// <item><see cref="FetchNpcImageFn"/> 等取图接缝的 <c>Textures == null</c> = 原文
///   <c>GetCachedImage</c> 返回 nil ⇒ <c>m_BodySurface</c>/<c>m_EffSurface</c> 保持 nil
///   ⇒ <c>DrawChr</c> 的 <c>&lt;&gt; nil</c> 门为假（**不画**，与原文同）；</item>
/// <item><see cref="ActorNpcEnv.EffectImageListCountFn"/> 默认 <c>0</c> ⇒ 任何
///   <c>Foo_File &gt;= 0</c> 都判越界 —— 即原文的"越界不取图"分支。</item>
/// </list>
/// </summary>
public static class ActorNpcEnv
{
    // ══════════════════════════════════════════════════════════════════════
    // 一、时钟与画布
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`MyGetTickCount`（原文 10266/10491/11118/17657）。</summary>
    public static Func<uint> MyGetTickCountFn = () => SceneTime.TickNow();

    /// <summary>`TimeGetTime`（原文 9989/11104/17548 等；headless 与 <see cref="MyGetTickCountFn"/> 同源）。</summary>
    public static Func<uint> TimeGetTimeFn = () => SceneTime.TickNow();

    /// <summary>`GameCanvas.Active and GameCanvas.Initialized`（原文 10269 的双重前置守卫）。</summary>
    public static Func<bool> CanvasReadyFn = () => false;

    /// <summary>`Random(n)`（原文 10187 的 `Random(7)`）。默认 <c>_ =&gt; 0</c>，与本工程其它随机接缝同形。</summary>
    public static Func<int, int> RandomFn = _ => 0;

    /// <summary>`g_PlaySound.PlaySound(id)`（原文 10187）。</summary>
    public static Action<int> PlaySoundByIdFn = _ => { };

    // ══════════════════════════════════════════════════════════════════════
    // 二、自定义 NPC 配置（原文 GetCustomNpcConfig）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `GetCustomNpcConfig(m_wAppearance)`（原文 9955/10242/10443/10503/11099）。
    /// <para><c>null</c> = 原文 <c>NpcConfig = nil</c> —— **原文语义**（未配置的自定义 NPC 外观），
    /// 不是"未接线"。注意与 <c>TActorHpBar.GetCustomNpcConfigFn</c>（血条专用窄视图，只有 HP 字段）
    /// 是**两个不同用途**的接缝：本接缝给全 <c>TNpcBaseConfig</c>+<c>Actions</c>。</para>
    /// </summary>
    public static Func<int, TClientCustomNpcConfig?> CustomNpcConfigLookupFn = _ => null;

    /// <summary>`g_EffectImageList.Count`（原文 10244/10513/11101 的越界判定）。</summary>
    public static Func<int> EffectImageListCountFn = () => 0;

    // ══════════════════════════════════════════════════════════════════════
    // 三、取图接缝（原文 g_WNpcImgImages / g_EffectImageList / g_WNewopUIImages）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `g_WNpcImgImages.Indexs[library].GetCachedGrayImage/BrightImage/Image(idx, px, py)`。
    /// <para>原文对每个外观写 ~40 段同形的 `case m_ColorEffect of ceGrayScale,ceGrayScale2 / ceBright / else`，
    /// 本接缝把"哪一段用它"作为 <c>library</c> 传入，**三分支语义由 <paramref name="color"/> 承载**，
    /// 逐段对应关系写在每个调用点的注释里。</para>
    /// </summary>
    public static Func<int, int, TColorEffect, NpcImageFetch> FetchNpcImageFn = (_, _, _) => default;

    /// <summary>`g_WNpcImgImages.GetCached*Image(idx, px, py)`（**不带** <c>Indexs[]</c>；
    /// 原文只有外观 42 的 `m_EffSurface` 一处这样写 —— 10939-10942，逐字保留该差异）。</summary>
    public static Func<int, TColorEffect, NpcImageFetch> FetchNpcRootImageFn = (_, _) => default;

    /// <summary>
    /// `TGameImages(g_EffectImageList.Objects[fileIndex]).GetCached*Image(index, px, py)`。
    /// <para>调用方已判 <c>fileIndex &lt; g_EffectImageList.Count</c>；返回
    /// <c>Texture == null</c> = 原文 <c>GameImages = nil</c> 或取图失败。</para>
    /// </summary>
    public static Func<int, int, TColorEffect, NpcImageFetch> FetchEffectListImageFn = (_, _, _) => default;

    /// <summary>`g_WNewopUIImages.GetCachedImage/GrayImage(idx, px, py)`（原文 17661-17663 `TStatuaryNpcActor.LoadSurface`）。</summary>
    public static Func<int, bool, NpcImageFetch> FetchNewopUiImageFn = (_, _) => default;

    // ══════════════════════════════════════════════════════════════════════
    // 四、绘制接缝
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// NPC 族全部 <c>GameCanvas.Draw*</c> 的出口（**顺序保序**）。
    /// <para>默认空实现 = 无头环境"画布不存在"，与原文 <c>GameCanvas</c> 未初始化时行为一致；
    /// 测试通过挂接它来逐条核对 <c>DrawOrder</c> 与层序。</para>
    /// </summary>
    public static Action<NpcDrawOp> NpcDrawFn = _ => { };

    // ══════════════════════════════════════════════════════════════════════
    // 五、TStatuaryNpcActor 的纹理创建与拷贝（原文 17759-17761 / 17827-17945 / 18005）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`GameCanvas.HGE.Texture_Create(800, 800)`（原文 17759-17761 三次）。</summary>
    public static Func<object?> CreateTexture800Fn = () => null;

    /// <summary>`CopyTexture(Source, Dest, x, y [, Scale])`（原文 17827/17842/17901/17913/17945/17962/18005）。</summary>
    public static Action<object?, object?, int, int, bool> CopyTextureFn = (_, _, _, _, _) => { };

    // ---- 武器 / 人物 / 头发 / 盾牌 / 人物特效 图集（原文 17822-17995）----

    /// <summary>`g_WWeaponImages.GetWWeaponImg/GrayImg(wWeapon, sex, idx, out ox, out oy)`（原文 17822-17824）。</summary>
    public static Func<int, int, int, bool, NpcImageFetch> WeaponImageFn = (_, _, _, _) => default;

    /// <summary>`g_WHumImgImages.GetWHumImg/GrayImg(wDress, sex, idx, out ox, out oy)`（原文 17832-17839）。</summary>
    public static Func<int, int, int, bool, NpcImageFetch> HumImageFn = (_, _, _, _) => default;

    /// <summary>
    /// `g_WHairImgImages` … `g_WHair6ImgImages`（原文 17848-17897 的**七张表**出图）。
    /// <para>参数：<c>(library, index, gray)</c> —— <c>library</c> 0..6 依次对应
    /// hair4 之外那七张表：0 = <c>g_WHairImgImages</c>（0..5 / 0..5 发型）、
    /// 1 = <c>g_WHair10ImgImages</c>（50..59）、2 = <c>g_WHair11ImgImages</c>（60..69）、
    /// 3 = <c>g_WHair2ImgImages</c>（100..107）、4 = <c>g_WHair3ImgImages</c>（108..119）、
    /// 5 = <c>g_WHair4ImgImages</c>（120..129）、6 = <c>g_WHair5ImgImages</c>（130..139）、
    /// 7 = <c>g_WHair6ImgImages</c>（140..149）。**表与区间的对应关系即原文语义**。</para>
    /// </summary>
    public static Func<int, int, bool, NpcImageFetch> HairImageFn = (_, _, _) => default;

    /// <summary>`g_WShieldImg.GetCachedGrayImage/Image(HUMANFRAME*(wShield-1)+idx, ...)`（原文 17904-17908）。</summary>
    public static Func<int, bool, NpcImageFetch> ShieldImageFn = (_, _) => default;

    /// <summary>`g_WHumEffectImages.GetWHumEffectImg/GrayImg(effect, sex, idx, weapon, out ox, out oy, False)`
    /// （原文 17921-17923 的翅膀、17987-17989 / 17993-17995 的武器特效扩展）。</summary>
    public static Func<int, int, int, bool, bool, NpcImageFetch> HumEffectImageFn = (_, _, _, _, _) => default;

    /// <summary>`g_WHumEffectImages.GetCachedGrayImage/Image(idx, px, py)`（原文 17939-17941 —— 与
    /// <see cref="HumEffectImageFn"/> 是**同一个图集上的两个不同方法**，参数语义不同，不可合并）。</summary>
    public static Func<int, bool, NpcImageFetch> HumEffectCachedImageFn = (_, _) => default;

    /// <summary>`g_WeaponEffectList.GetWWeaponEffectImg/GrayImg(...)`（原文 17991-17995）。</summary>
    public static Func<int, int, int, bool, NpcImageFetch> WeaponEffectImageFn = (_, _, _, _) => default;

    /// <summary>`D.Width * D.Height` 判定所需的尺寸（原文 17835：`D.Width * D.Height &lt;= 16`）。</summary>
    public static Func<object?, (int Width, int Height)> TextureSizeFn = _ => (0, 0);

    // ══════════════════════════════════════════════════════════════════════
    // 六、THeroActor / TNpcActor.CheckLoadUserName 的上行与全局
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>`frmMain.SendClientMessage(wIdent, nRecog, nX, nY, nParam)`（原文 17429/17431/17493/17504/17523/17526/17531）。</summary>
    public static Action<int, long, int, int, int> SendClientMessageFn = (_, _, _, _, _) => { };

    /// <summary>`frmMain.SendSay(Str)`（原文 17499 的 `'@RestHero'`）。</summary>
    public static Action<string> SendSayFn = _ => { };

    /// <summary>`g_HeroMagicList` 的 `Def.wMagicId` 序列（原文 17445-17451 的线性首命中查找）。</summary>
    public static Func<IReadOnlyList<int>> HeroMagicIdListFn = () => Array.Empty<int>();

    /// <summary>`g_MySelf.m_btJob`（原文 17461 的外层 case）。</summary>
    public static Func<int> MySelfJobFn = () => 0;

    /// <summary>`g_dwLatestSpellTick := MyGetTickCount`（原文 17491 的副作用）。</summary>
    public static Action<uint> SetLatestSpellTickFn = _ => { };

    /// <summary>Actor.pas:36 的 `HUMANFRAME = 600`（人物图集每方向帧数；17906/17939/17972 的图号公式）。</summary>
    public const int HumanFrame = 600;

    /// <summary>`g_TargetCret`（原文 17515）。</summary>
    public static Func<TActor?> TargetCretFn = () => null;

    /// <summary>`g_FocusCret`（原文 17516）。</summary>
    public static Func<TActor?> FocusCretFn = () => null;

    /// <summary>`PlayScene.IsValidActor(cret)`（原文 17521/17522/17530）。</summary>
    public static Func<TActor, bool> IsValidActorFn = _ => false;

    /// <summary>`g_nMouseCurrX`（原文 17504/17526）。</summary>
    public static Func<int> MouseCurrXFn = () => 0;

    /// <summary>`g_nMouseCurrY`（原文 17504/17526）。</summary>
    public static Func<int> MouseCurrYFn = () => 0;

    /// <summary>`LegendMap.Stop`（原文 17432）。</summary>
    public static Action LegendMapStopFn = () => { };

    /// <summary>
    /// `FlyEffect.ClientConfig.Sounds[custMagicExplosion]`（原文 13644 的 `Length(...) &gt; 0` 判据）。
    /// <para>返回 <c>null</c> = <c>Sender</c> **不是** <c>TCustomMonFlyEffect</c>
    /// （对应原文 13641 的 <c>is</c> 门为假）；返回**空串** = 是飞行特效但该条音效未配置
    /// （对应 13644 的 <c>Length &gt; 0</c> 为假）。两级语义在类型层面可区分。</para>
    /// </summary>
    public static Func<object?, string?> MonFlyEffectExplosionSoundFn = _ => null;

    // ---- CheckLoadUserName（10266-10295）的四个开关 ----

    /// <summary>`PlugInEnabled`（原文 10271）。</summary>
    public static bool PlugInEnabled = true;

    /// <summary>`g_ClientConfig.boHideGhost`（原文 10273）。</summary>
    public static bool ClientConfigBoHideGhost;

    /// <summary>`g_ConfigDlg.ConfigCheckeds[ckHideGhost]`（原文 10273）。</summary>
    public static bool ConfigDlgCkHideGhost;

    /// <summary>`g_ClientConfig.boShowNpcName`（原文 10276）。</summary>
    public static bool ClientConfigBoShowNpcName;

    /// <summary>`g_ConfigDlg.ConfigCheckeds[ckShowNpcName]`（原文 10276）。</summary>
    public static bool ConfigDlgCkShowNpcName;

    /// <summary>`g_FocusCret = Self`（原文 10279/10287）。</summary>
    public static Func<TActor, bool> IsFocusActorFn = _ => false;

    // ══════════════════════════════════════════════════════════════════════
    // 七、TNpcActor.Run 尾部（原文 11119 `PlayScene.LoadSurface(LoadSurface)`）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// `PlayScene.LoadSurface(LoadSurface)`（原文 11119）。
    /// <para><c>null</c> = 未接线（无场景宿主）⇒ **不调** LoadSurface。这与"调了但没效果"在类型层面可区分。</para>
    /// </summary>
    public static Action<TActor>? RequestLoadSurfaceFn;

    /// <summary>
    /// 复位本类全部接缝为构造期默认值（台帐 §29.4；不含各实例字段）。
    /// </summary>
    public static void Reset()
    {
        MyGetTickCountFn = () => SceneTime.TickNow();
        TimeGetTimeFn = () => SceneTime.TickNow();
        CanvasReadyFn = () => false;
        RandomFn = _ => 0;
        PlaySoundByIdFn = _ => { };
        CustomNpcConfigLookupFn = _ => null;
        EffectImageListCountFn = () => 0;
        FetchNpcImageFn = (_, _, _) => default;
        FetchNpcRootImageFn = (_, _) => default;
        FetchEffectListImageFn = (_, _, _) => default;
        FetchNewopUiImageFn = (_, _) => default;
        NpcDrawFn = _ => { };
        CreateTexture800Fn = () => null;
        CopyTextureFn = (_, _, _, _, _) => { };
        WeaponImageFn = (_, _, _, _) => default;
        HumImageFn = (_, _, _, _) => default;
        HairImageFn = (_, _, _) => default;
        ShieldImageFn = (_, _) => default;
        HumEffectImageFn = (_, _, _, _, _) => default;
        HumEffectCachedImageFn = (_, _) => default;
        WeaponEffectImageFn = (_, _, _, _) => default;
        TextureSizeFn = _ => (0, 0);
        SendClientMessageFn = (_, _, _, _, _) => { };
        SendSayFn = _ => { };
        HeroMagicIdListFn = () => Array.Empty<int>();
        MySelfJobFn = () => 0;
        SetLatestSpellTickFn = _ => { };
        TargetCretFn = () => null;
        FocusCretFn = () => null;
        IsValidActorFn = _ => false;
        MouseCurrXFn = () => 0;
        MouseCurrYFn = () => 0;
        LegendMapStopFn = () => { };
        MonFlyEffectExplosionSoundFn = _ => null;
        PlugInEnabled = true;
        ClientConfigBoHideGhost = false;
        ConfigDlgCkHideGhost = false;
        ClientConfigBoShowNpcName = false;
        ConfigDlgCkShowNpcName = false;
        IsFocusActorFn = _ => false;
        RequestLoadSurfaceFn = null;
    }
}

/// <summary>
/// `Actor.pas` NPC 族用到、而托管侧 <c>TActorCore</c> 尚未持有的实例字段
/// （**新增**，属车道 `p17-client-actor` 的 <c>Actor*</c> 自有文件）。
///
/// <para><b>为什么用 partial 而不是改既有文件</b>：本工程既定手法（消除"两个写者"）。
/// <c>TActorCore</c> 自 <c>ActorCore.cs:11</c> 起就是 <c>partial</c>。</para>
///
/// <para><b>不重复声明</b>：<c>m_boUseEffect</c>（ActorMotion.cs:86）、<c>m_nEffectFrame</c>
/// /<c>m_nEffectStart</c>/<c>m_nEffectEnd</c>/<c>m_dwEffectStartTime</c>/<c>m_dwEffectFrameTime</c>
/// （ActorFamilyEnv.cs:256-262）、<c>m_nPx</c>/<c>m_nPy</c>（PlaySceneActors.cs:222）、
/// <c>m_btBodyColor</c>（PlaySceneActors.cs:142）、<c>m_wDress</c>/<c>m_wWeapon</c>/<c>m_wShield</c>
/// /<c>m_btHair</c>（PlaySceneActors.cs:141-153）、<c>m_BaseColorEffect</c>（ActorFamilyEnv.cs:235）
/// 均已存在。</para>
/// </summary>
public partial class TActorCore
{
    // ──────────────────────────────────────────────────────────────────────
    // 1. TNpcActor（9936-11122）直接读写
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`m_EffSurface`（原文 10236/10262/10495 清、10303/10427/10415 判、全局赋值）。</summary>
    public object? m_EffSurface;

    /// <summary>`m_KeepSurface`（原文 10237/10263/10496 清、10321 判）。</summary>
    public object? m_KeepSurface;

    /// <summary>`m_nEffX` / `m_nEffY`（原文 10306-10323、10717-10748 的**会被改写**的特效图偏移）。</summary>
    public int m_nEffX;
    public int m_nEffY;

    /// <summary>`m_nKeepX` / `m_nKeepY`（原文 10323-10325、10518-10521）。</summary>
    public int m_nKeepX;
    public int m_nKeepY;

    /// <summary>`m_bo248`（原文 10184/10239/11083-11086：外观 52 的挖矿 23 秒循环）。</summary>
    public bool m_bo248;

    /// <summary>`m_dwUseEffectTick`（原文 10185/11084/11087）。</summary>
    public uint m_dwUseEffectTick;

    /// <summary>`m_StartCounter`（原文 9990/10017/10202/17549 —— 与 `m_dwStartTime` **并列的第二个**打点）。</summary>
    public uint m_StartCounter;

    /// <summary>`m_nKeepFrame`（原文 10247/11069/11105-11111）。</summary>
    public int m_nKeepFrame;

    /// <summary>`m_LastKeepPlayTick`（原文 10248/11104/11111）。</summary>
    public uint m_LastKeepPlayTick;

    /// <summary>`m_boHitEffect`（原文 10238）。</summary>
    public bool m_boHitEffect;

    /// <summary>`m_btPhantomAlpha`（原文 5829-5844 `StretchDrawEffSurface` 的幻影 alpha）。</summary>
    public byte m_btPhantomAlpha;

    /// <summary>`m_sDescUserName`（原文 10277/10280/10288 的 NPC 名字前缀）。</summary>
    public string m_sDescUserName = "";

    // ──────────────────────────────────────────────────────────────────────
    // 2. TStatuaryNpcActor（17537-18008）直接读写
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`FIsFinalized`（原文 17578/17603-17604/17647）。</summary>
    public bool FIsFinalized;

    /// <summary>`m_nEffigyState`（原文 17562-17563/17690/17701/17715-17734）。</summary>
    public TFeature_New m_nEffigyState;

    /// <summary>`m_nEffigyOffset` / `m_nOldEffigyOffset`（原文 17565-17566/17692-17693）。</summary>
    public int m_nEffigyOffset;
    public int m_nOldEffigyOffset;

    /// <summary>`m_boShowStatuary`（原文 17561/17608/17702/17713）。</summary>
    public bool m_boShowStatuary;

    /// <summary>`m_boScaleShow`（原文 17575/17613/17698；原声明的只读属性 <c>boScaleShow</c> 见 1932）。</summary>
    public bool m_boScaleShow;

    /// <summary>`m_IsGrayShow`（原文 17576/17605/17660/17700）。</summary>
    public bool m_IsGrayShow;

    /// <summary>`m_HumTexture` / `m_HumEffTexture` / `m_WeaponEffTexture`（800×800 离屏纹理，原文 17759-17761）。</summary>
    public object? m_HumTexture;
    public object? m_HumEffTexture;
    public object? m_WeaponEffTexture;

    /// <summary>`boScaleShow` 只读属性（原文 1932）。</summary>
    public bool boScaleShow => m_boScaleShow;

    // ──────────────────────────────────────────────────────────────────────
    // 3. THeroActor（2049-2060 / 17437-17533）直接读写
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>`m_nBagCount`（原文 2050）。</summary>
    public int m_nBagCount;

    /// <summary>`m_nAngryValue`（原文 2051/17489：合击的当前愤怒值）。</summary>
    public int m_nAngryValue;

    /// <summary>`m_nMaxAngryValue`（原文 2052/17489：合击的最大愤怒值）。</summary>
    public int m_nMaxAngryValue;

    /// <summary>
    /// Delphi <c>FreeAndNil(T)</c> 的 headless 等价：非 nil 则释放（托管侧只对 <see cref="IDisposable"/>
    /// 真正调用 <c>Dispose</c>），随后置 nil。
    /// <para><b>逐字保留原文的 <c>if X &lt;&gt; nil then FreeAndNil(X)</c> 形态</b> ——
    /// 原文 17703-17710 与 17747-17757 两处都先判 nil，故此处同样判。</para>
    /// </summary>
    public static void FreeAndNil(ref object? texture)
    {
        if (texture is IDisposable disposable)
            disposable.Dispose();
        texture = null;
    }
}
