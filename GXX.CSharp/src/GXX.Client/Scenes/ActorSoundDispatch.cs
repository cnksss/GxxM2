using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>声音播放请求（headless：替换 g_PlaySound.PlaySound / PlaySound）。</summary>
public readonly record struct SoundCue(int SoundId, string Kind);

/// <summary>
/// Actor.pas `TActor.RunSound`（6788-6901）与 `TActor.RunActSound`（6903-7200）的
/// **派发决策层** 1:1 移植（批次J94）。
///
/// 本批次**只移植"该播哪个声音、播完是否关掉 `m_boRunSound`"这两个决策**，
/// 不移植音频解码/播放本身（属 Bass 层，另行批次）。
/// 所有取声分支均通过返回 `SoundCue` 列表表达，不产生副作用。
///
/// **贯穿全族的原文语义**：
/// - 绝大多数分支都包在 `if frame = 2 then` 中（少数例外见各分支注释）；
/// - 播过声音后**几乎都置 `m_boRunSound := False`**，使该动作不再重复播音
///   （`SM_61HIT` 亦然；`SM_CUSTOM_HIT001` 范围支路只在真的取到配置并选定声音后才置 False）；
/// - `RunActSound` 仅对 **`m_btRace in [0, 1]`** 走这套武器音，其余种族走 7045 起的 else 分支。
/// </summary>
public static class ActorSoundDispatch
{
    /// <summary>6909：`RunActSound` 入口门。</summary>
    public static bool RunActSoundGate(bool m_boRunSound) => m_boRunSound;

    /// <summary>6910：武器音分支的种族门（`m_btRace in [0, 1]`）。</summary>
    public static bool IsWeaponSoundRace(int m_btRace) => m_btRace is 0 or 1;

    /// <summary>6919：自定义攻击动作码 → 自定义技能序号（`- SM_CUSTOM_HIT001 + CUSTOM_MAGIC_START_ID`）。</summary>
    public const int CustomMagicStartId = 1;

    /// <summary>7005/7011：断岳斩（`SM_102HIT`）触发的屏幕震动。</summary>
    public const int ScreenShakeAction = TActorCore.SM_102HIT;

    /// <summary>7005：`SM_100HIT..SM_103HIT` 范围的四个码。</summary>
    public static bool Is100To103Hit(int action)
        => action >= TActorCore.SM_100HIT && action <= TActorCore.SM_103HIT;

    /// <summary>
    /// `RunSound`（6788-6839）1:1：按 `m_nCurrentAction` 决定播放哪些声音。
    /// **该函数的返回值仅表示"要播的声音"，`m_boRunSound` 在入口被无条件置 True（6794）**。
    /// </summary>
    public static List<SoundCue> RunSound(
        int m_nCurrentAction,
        int m_nStruckWeaponSound, int m_nStruckSound, int m_nScreamSound,
        int m_nDieSound, bool m_boDeath, bool isMySelf,
        int m_nAttackSound, int m_nAppearSound, int m_nMagicStartSound,
        bool hasCustomMagicConfig, int newLevel,
        IReadOnlyList<int>? customUseMagicSounds)
    {
        var cues = new List<SoundCue>();

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_STRUCK:
                // 三个声音**各自独立判 >= 0**，故可同时播放
                if (m_nStruckWeaponSound >= 0) cues.Add(new SoundCue(m_nStruckWeaponSound, "StruckWeapon"));
                if (m_nStruckSound >= 0) cues.Add(new SoundCue(m_nStruckSound, "Struck"));
                if (m_nScreamSound >= 0) cues.Add(new SoundCue(m_nScreamSound, "Scream"));
                break;

            case TActorCore.SM_NOWDEATH:
                // 6803：额外要求 m_boDeath；自身死亡还要发延迟消息播 gameover BGM
                if (m_nDieSound >= 0 && m_boDeath)
                {
                    cues.Add(new SoundCue(m_nDieSound, "Die"));
                    if (isMySelf)
                        cues.Add(new SoundCue(0, "GameOverBgmDelay"));
                }
                break;

            case TActorCore.SM_THROW:
            case TActorCore.SM_HIT:
            case TActorCore.SM_FLYAXE:
            case TActorCore.SM_LIGHTING:
            case TActorCore.SM_DIGDOWN:
                if (m_nAttackSound >= 0) cues.Add(new SoundCue(m_nAttackSound, "Attack"));
                break;

            case TActorCore.SM_ALIVE:
            case TActorCore.SM_DIGUP:
                // 6815：**无 >= 0 判定**，appear 音一律播
                cues.Add(new SoundCue(m_nAppearSound, "Appear"));
                break;

            case TActorCore.SM_SPELL:
                if (hasCustomMagicConfig)
                {
                    var lvl = SelfMagicEffectRender.MagicPlusLevelOf(newLevel);
                    int snd = customUseMagicSounds != null && (int)lvl < customUseMagicSounds.Count
                        ? customUseMagicSounds[(int)lvl] : -1;
                    // 6832：仅有配置音**长度 > 0** 时才播（长度 0 表示未配置）
                    if (snd > 0)
                        cues.Add(new SoundCue(snd, "CustomUseMagic"));
                }
                else
                    cues.Add(new SoundCue(m_nMagicStartSound, "MagicStart"));
                break;
        }

        return cues;
    }

    /// <summary>
    /// `RunActSound` 武器音族（6911-7043）1:1。所有分支都要求 `frame = 2`。
    /// 返回的是"要播的声音"与"是否应关闭 m_boRunSound"。
    /// </summary>
    public static List<SoundCue> RunActSoundWar(
        int m_nCurrentAction, int frame, int m_btSex,
        int m_nWeaponSound,
        int sYedoMan, int sYedoWoman, int sLonghit, int sWidehit, int sFirehit, int sPhz,
        bool hasCustomMagicConfig, int newLevel,
        Func<int, int, int>? customSoundLookup,
        bool sceneShakeChecked,
        out bool closeRunSound)
    {
        var cues = new List<SoundCue>();
        closeRunSound = false;

        // 全族共同门：只有第 2 帧才出声
        if (frame != 2)
            return cues;

        bool close = false;

        void Weapon(string kind)
        {
            cues.Add(new SoundCue(m_nWeaponSound, kind));
            close = true;
        }

        switch (m_nCurrentAction)
        {
            case TActorCore.SM_THROW:
            case TActorCore.SM_HIT:
            case TActorCore.SM_HIT + 1:
            case TActorCore.SM_HIT + 2:
                Weapon("Weapon");
                break;

            case int a when a >= TActorCore.SM_CUSTOM_HIT001
                && a < TActorCore.SM_CUSTOM_HIT001 + TActorCore.CustomMagicCount:
            {
                // 6919：自定义动作码 → 技能序号
                int serial = m_nCurrentAction - TActorCore.SM_CUSTOM_HIT001 + CustomMagicStartId;

                if (hasCustomMagicConfig)
                {
                    var lvl = SelfMagicEffectRender.MagicPlusLevelOf(newLevel);
                    int manWarr = customSoundLookup?.Invoke(serial, (int)lvl) ?? -1;

                    // 6932/6936：男号优先 cmstManWarr、女号 cmstWomanWarr，都未配置才退回武器音
                    if (m_btSex == 0 && manWarr > 0)
                    {
                        cues.Add(new SoundCue(manWarr, "CustomManWarr"));
                        close = true;
                    }
                    else if (m_btSex == 1 && manWarr > 0)
                    {
                        cues.Add(new SoundCue(manWarr, "CustomWomanWarr"));
                        close = true;
                    }
                    else
                    {
                        Weapon("CustomFallbackWeapon");
                    }
                }
                // 取不到配置时**不播也不关**
                break;
            }

            case TActorCore.SM_POWERHIT:
                cues.Add(new SoundCue(m_nWeaponSound, "Weapon"));
                cues.Add(new SoundCue(m_btSex == 0 ? sYedoMan : sYedoWoman, "Yedo"));
                close = true;
                break;

            case TActorCore.SM_LONGHIT:
                Weapon("Weapon");
                cues.Add(new SoundCue(sLonghit, "LongHit"));
                break;

            case TActorCore.SM_WIDEHIT:
                Weapon("Weapon");
                cues.Add(new SoundCue(sWidehit, "WideHit"));
                break;

            case TActorCore.SM_FIREHIT:
                Weapon("Weapon");
                cues.Add(new SoundCue(sFirehit, "FireHit"));
                break;

            case TActorCore.SM_TWNHIT:
            case TActorCore.SM_CRSHIT:
            case TActorCore.SM_43HIT:
                cues.Add(new SoundCue(m_nWeaponSound, "Weapon"));
                // 6977：仅 SM_TWNHIT 用专用音 11058，其余用 s_widehit
                cues.Add(new SoundCue(
                    m_nCurrentAction == TActorCore.SM_TWNHIT ? 11058 : sWidehit,
                    m_nCurrentAction == TActorCore.SM_TWNHIT ? "TwnHit" : "WideHit"));
                close = true;
                break;

            case TActorCore.SM_60HIT:
                Weapon("Weapon");
                cues.Add(new SoundCue(sPhz, "Phz"));
                break;

            case TActorCore.SM_61HIT:
                // 6990-6997：**不播武器音**，只播 124 与 10512
                cues.Add(new SoundCue(124, "Hit61A"));
                cues.Add(new SoundCue(10512, "Hit61B"));
                close = true;
                break;

            case TActorCore.SM_SWORDHIT:
                Weapon("Weapon");
                cues.Add(new SoundCue(sFirehit, "FireHit"));
                break;

            case int a when Is100To103Hit(a):
            {
                // 7005-7015：武器音 + firehit；**仅 SM_102HIT 且勾选场景震动时追加一次震动**
                Weapon("Weapon");
                cues.Add(new SoundCue(sFirehit, "FireHit"));
                if (m_nCurrentAction == ScreenShakeAction && sceneShakeChecked)
                    cues.Add(new SoundCue(0, "SceneShake"));
                break;
            }

            case TActorCore.SM_66HIT:
            case TActorCore.SM_66HIT1:      // 开天斩重击
                // 7016-7020：**不播武器音**，只播 11056
                cues.Add(new SoundCue(11056, "Hit66"));
                close = true;
                break;

            case TActorCore.SM_113HIT:      // 断空斩
                cues.Add(new SoundCue(m_btSex == 0 ? 11030 : 11031, "Hit113"));
                close = true;
                break;

            case TActorCore.SM_115HIT:      // 血魄一击
                cues.Add(new SoundCue(m_btSex == 0 ? 11033 : 11034, "Hit115"));
                close = true;
                break;
        }

        closeRunSound = close;
        return cues;
    }

    /// <summary>
    /// `RunActSound` 非武器种族 else 分支（**7045-7093**）的**决策部分** 1:1。
    ///
    /// <para><b>结构（四段，注意嵌套层级）</b>：</para>
    /// <list type="number">
    /// <item><b>7046-7047</b>：`m_btRace = 50` → **空实现**（既不播音、也**不消费随机数**），且
    ///   **连带跳过 7076-7092 的 Mon36 段**（该段在 7074 的 `end;` **之后**，仍属 7045 的
    ///   `else begin`，故 race 50 走不到它 —— race 50 与 `[202..209]` 本就无交集，但结构如此）。</item>
    /// <item><b>7051-7056</b>：`SM_TURN` 且 `frame = 1` 且 `Random(8) = 1` → 播 `m_nNormalSound`。</item>
    /// <item><b>7057-7062</b>：`SM_HIT` 且 `frame = 3` 且 `m_nAttackSound &gt;= 0` → 播 `m_nWeaponSound`。
    ///   ★ 与上一条是**两个独立 `if`**（不是 `else if`）。</item>
    /// <item><b>7063-7072</b>：`case m_wAppearance of 80:` —— **仅 `SM_NOWDEATH` 且 `frame = 2`** →
    ///   播 `m_nDie2Sound`（原文唯一的 appearance 分支）。</item>
    /// <item><b>7076-7092</b>：`m_btRace in [202..209]`（Mon36_X 族）—— 三个动作各播**硬编码音**
    ///   （`SM_TURN→542` / `SM_STRUCK→495` / `SM_NOWDEATH→496`），且**三个分支都不判 `frame`**。</item>
    /// </list>
    ///
    /// <para><b>★ 车道 `p7-client-actor-family` 的更正记录</b>：本函数**原先只落了 7051-7062 两段**，
    /// 漏掉 7063-7072 与 7076-7092；而本函数的 XML 注释当时自称是"7045-7110ish 的决策部分 1:1"
    /// —— 即**注释与实现不符**。已按原文**逐字补齐这两段**（父 agent 裁定：在**源头**修，
    /// 不允许在别处留第二份"同义但内容不同"的派发实现，台账 §25.2/§26.2/§31.4）。
    /// 改动前后行为差异：`m_wAppearance = 80` 的死亡音与 Mon36_X 族的三种音
    /// **从"静默丢失"变为正常播放**。</para>
    ///
    /// <para><b>参数位置说明</b>：<paramref name="m_wAppearance"/> 与 <paramref name="m_nDie2Sound"/>
    /// 是本次补齐所需的两个新增入参，为**不破坏既有调用点**而追加在 <paramref name="rand8"/> **之后**
    /// （它们的原文使用点 7063-7072 在逻辑上也位于 7051-7062 **之后**，故这个顺序同样忠于原文流程）。</para>
    /// </summary>
    public static List<SoundCue> RunActSoundOther(
        int m_btRace, int m_nCurrentAction, int frame,
        int m_nNormalSound, int m_nAttackSound, int m_nWeaponSound,
        Func<int> rand8,
        int m_wAppearance, int m_nDie2Sound,
        out bool closeRunSound)
    {
        var cues = new List<SoundCue>();
        closeRunSound = false;

        // 7046：race 50 空实现
        if (m_btRace == 50)
            return cues;

        bool close = false;

        // 7051-7056：仅转身动作、第 1 帧、且 Random(8) = 1 时才出声
        if (m_nCurrentAction == TActorCore.SM_TURN)
        {
            if (frame == 1 && rand8() == 1)
            {
                cues.Add(new SoundCue(m_nNormalSound, "NormalTurn"));
                close = true;
            }
        }

        // 7057-7062：攻击动作第 3 帧且攻击音有效
        if (m_nCurrentAction == TActorCore.SM_HIT)
        {
            if (frame == 3 && m_nAttackSound >= 0)
            {
                cues.Add(new SoundCue(m_nWeaponSound, "HitWeapon"));
                close = true;
            }
        }

        // 7063-7072：appearance 80 专用死亡音（原文唯一的 appearance 分支）
        //   case m_wAppearance of 80: if m_nCurrentAction = SM_NOWDEATH then if (frame = 2) ...
        if (m_wAppearance == 80)
        {
            if (m_nCurrentAction == TActorCore.SM_NOWDEATH)
            {
                if (frame == 2)
                {
                    cues.Add(new SoundCue(m_nDie2Sound, "Die2"));   // 7067
                    close = true;                                   // 7068
                }
            }
        }

        // 7076-7092：Mon36_X 族（race 202..209）—— 三分支**都判 action 而不判 frame**
        if (m_btRace is >= 202 and <= 209)
        {
            if (m_nCurrentAction == TActorCore.SM_TURN)             // 7080
            {
                cues.Add(new SoundCue(542, "Mon36Turn"));           // 7081
                close = true;                                       // 7082
            }

            if (m_nCurrentAction == TActorCore.SM_STRUCK)           // 7084
            {
                cues.Add(new SoundCue(495, "Mon36Struck"));         // 7085
                close = true;                                       // 7086
            }

            if (m_nCurrentAction == TActorCore.SM_NOWDEATH)         // 7088
            {
                cues.Add(new SoundCue(496, "Mon36NowDeath"));       // 7089
                close = true;                                       // 7090
            }
        }

        closeRunSound = close;
        return cues;
    }
}
