// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  本文件：窗体族所依赖的 `g_Config` 字段子集（原文均属 M2Share.pas 的 `TM2Config`）
//    · M2Share.pas:1143-1153   boDiableDummyRun / boDummyRun* / boDummyWar* /
//                              boDummySafeAreaLimited / boDummySafeAreaDisNpcRun /
//                              boSafeAreaDisShopStallDummyRun / boSafeAreaDisOffLineDummyRun
//    · M2Share.pas:2248-2262   boDummyLogonRand / nDummyLogonTime / sDummyHomeMap /
//                              nDummyHomeX / nDummyHomeY / boDummyAutoRepairItem /
//                              boDummyAutoRecallHero / dwDummy*AttackTime / dwDummy*WalkTime
//    · M2Share.pas:2930-2952   boDummyAutoAdd* / nDummyAdd*Percent / nDummy*HPTime_* /
//                              nDummy*HPBase_* / nDummy*MPTime_* / nDummy*MPBase_* /
//                              nDummyHero*
//
//  ⚠ 接缝说明：
//    M2Share.pas 属**顺序会话常驻区**（`src/GXX.M2Server/**` 其余部分只读），
//    本车道不修改同类既有文件。这些字段在托管侧尚无定义（已 git grep main 确认：
//    只有注释提及、无声明），故在此以**本单元依赖子集**形式落地，
//    待 M2Share.pas 全量批次移植后由集成方合并（届时**必须删掉本文件里的重复声明**，
//    否则 CS0102）。这与既有 `Forms/GamePets/GamePetsConfig.cs`、`Forms/CustomMagic/**`
//    的做法一致（同型接缝，非本车道发明）。
//
//  类型选择：原文字段分属 4 个不同段落的 `TM2Config` 记录，但托管侧 `M2Config`
//  本就是**扁平 static 类**（见既有 M2Config.*.cs 共 21 份 partial），故按同一口径落为
//  同类 static 字段。字段名、类型、默认值逐字照抄 M2Share.pas typed-constant 初始值
//  （:4268-4272 / :4931-4940 / :5507-5529）。
// ============================================================================

namespace GXX.M2Server.Engine;

/// <summary>
/// `uFrmDummySetting.pas` 依赖的 `g_Config`（M2Share.pas `TM2Config`）字段子集 1:1。
/// 默认值取自 M2Share.pas typed-constant 段（行号见文件头）。
/// </summary>
public static partial class M2Config
{
    // ------------------------------------------------------------------
    //  M2Share.pas:1143-1153（跑动段）
    // ------------------------------------------------------------------

    /// <summary>`M2Share.pas:1143 boDiableDummyRun: Boolean`（默认 True，见 :4268）。</summary>
    public static bool boDiableDummyRun = true;

    /// <summary>`M2Share.pas:1144 boDummyRunHum: Boolean`（默认 False，见 :4268）。</summary>
    public static bool boDummyRunHum;

    /// <summary>`M2Share.pas:1145 boDummyRunMon: Boolean`（默认 False，见 :4268）。</summary>
    public static bool boDummyRunMon;

    /// <summary>`M2Share.pas:1146 boDummyRunNpc: Boolean`（默认 False，见 :4268）。</summary>
    public static bool boDummyRunNpc;

    /// <summary>`M2Share.pas:1147 boDummyRunGuard: Boolean`（默认 False，见 :4268）。</summary>
    public static bool boDummyRunGuard;

    /// <summary>`M2Share.pas:1148 boDummyWarDisHumRun: Boolean`（默认 False，见 :4269）。</summary>
    public static bool boDummyWarDisHumRun;

    /// <summary>`M2Share.pas:1149 boDummyWarHreoRun: Boolean` —— 原文注释「攻城区域允许穿英雄 piaoyun 2013-07-17」。</summary>
    public static bool boDummyWarHreoRun;

    /// <summary>`M2Share.pas:1150 boDummySafeAreaLimited: Boolean`（默认 False，见 :4270）。</summary>
    public static bool boDummySafeAreaLimited;

    /// <summary>`M2Share.pas:1151 boDummySafeAreaDisNpcRun: Boolean` —— 原文注释「安全区域禁止穿NPC piaoyun 2013-07-17」。</summary>
    public static bool boDummySafeAreaDisNpcRun;

    /// <summary>`M2Share.pas:1152 boSafeAreaDisShopStallDummyRun: Boolean` —— 原文注释「安全区域禁止穿摆摊人物 piaoyun 2013-07-17」。</summary>
    public static bool boSafeAreaDisShopStallDummyRun;

    /// <summary>`M2Share.pas:1153 boSafeAreaDisOffLineDummyRun: Boolean` —— 原文注释「安全区域禁止穿离线人物 piaoyun 2013-07-17」。</summary>
    public static bool boSafeAreaDisOffLineDummyRun;

    // ------------------------------------------------------------------
    //  M2Share.pas:2248-2262（假人登录段）
    // ------------------------------------------------------------------

    /// <summary>`M2Share.pas:2248 boDummyLogonRand: Boolean`（默认 False，见 :4931）。</summary>
    public static bool boDummyLogonRand;

    /// <summary>`M2Share.pas:2249 nDummyLogonTime: Integer`（默认 3，见 :4931）。</summary>
    public static int nDummyLogonTime = 3;

    /// <summary>`M2Share.pas:2252 sDummyHomeMap: string`（默认 '3'，见 :4934）。</summary>
    public static string sDummyHomeMap = "3";

    /// <summary>`M2Share.pas:2253 nDummyHomeX: Integer`（默认 330，见 :4934）。</summary>
    public static int nDummyHomeX = 330;

    /// <summary>`M2Share.pas:2254 nDummyHomeY: Integer`（默认 330，见 :4934）。</summary>
    public static int nDummyHomeY = 330;

    /// <summary>`M2Share.pas:2255 boDummyAutoRepairItem: Boolean`（默认 True，见 :4936）。</summary>
    public static bool boDummyAutoRepairItem = true;

    /// <summary>`M2Share.pas:2256 boDummyAutoRecallHero: Boolean`（默认 True，见 :4936）。</summary>
    public static bool boDummyAutoRecallHero = true;

    /// <summary>`M2Share.pas:2257 dwDummyWarrorAttackTime: Integer`（默认 1200，见 :4938）。</summary>
    /// <remarks>原文拼写 `Warror`（非 `Warrior`）—— 照抄。</remarks>
    public static int dwDummyWarrorAttackTime = 1200;

    /// <summary>`M2Share.pas:2258 dwDummyWizardAttackTime: Integer`（默认 1200，见 :4938）。</summary>
    public static int dwDummyWizardAttackTime = 1200;

    /// <summary>`M2Share.pas:2259 dwDummyTaoistAttackTime: Integer`（默认 1200，见 :4938）。</summary>
    public static int dwDummyTaoistAttackTime = 1200;

    /// <summary>`M2Share.pas:2260 dwDummyWarrorWalkTime: Integer`（默认 500，见 :4940）。</summary>
    /// <remarks>原文拼写 `Warror` —— 照抄。</remarks>
    public static int dwDummyWarrorWalkTime = 500;

    /// <summary>`M2Share.pas:2261 dwDummyWizardWalkTime: Integer`（默认 500，见 :4940）。</summary>
    public static int dwDummyWizardWalkTime = 500;

    /// <summary>`M2Share.pas:2262 dwDummyTaoistWalkTime: Integer`（默认 500，见 :4940）。</summary>
    public static int dwDummyTaoistWalkTime = 500;

    // ------------------------------------------------------------------
    //  M2Share.pas:2930-2952（假人自动加血/回血回蓝段）
    // ------------------------------------------------------------------

    /// <summary>`M2Share.pas:2930 boDummyAutoAddHP: Boolean`（默认 True，见 :5507）。</summary>
    public static bool boDummyAutoAddHP = true;

    /// <summary>`M2Share.pas:2931 nDummyAddHPPercent: Integer`（默认 60，见 :5507）。</summary>
    public static int nDummyAddHPPercent = 60;

    /// <summary>`M2Share.pas:2932 boDummyAutoAddMP: Boolean`（默认 True，见 :5509）。</summary>
    public static bool boDummyAutoAddMP = true;

    /// <summary>`M2Share.pas:2933 nDummyAddMPPercent: Integer`（默认 60，见 :5509）。</summary>
    public static int nDummyAddMPPercent = 60;

    /// <summary>`M2Share.pas:2934 nDummyHPTime_Warrior: Integer`（默认 350，见 :5511）。</summary>
    public static int nDummyHPTime_Warrior = 350;

    /// <summary>`M2Share.pas:2935 nDummyHPTime_DF: Integer`（默认 350，见 :5512）。</summary>
    public static int nDummyHPTime_DF = 350;

    /// <summary>`M2Share.pas:2936 nDummyHeroHPTime_Warrior: Integer`（默认 350，见 :5513）。</summary>
    public static int nDummyHeroHPTime_Warrior = 350;

    /// <summary>`M2Share.pas:2937 nDummyHeroHPTime_DF: Integer`（默认 350，见 :5514）。</summary>
    public static int nDummyHeroHPTime_DF = 350;

    /// <summary>`M2Share.pas:2939 nDummyMPTime_Warrior: Integer`（默认 800，见 :5516）。</summary>
    public static int nDummyMPTime_Warrior = 800;

    /// <summary>`M2Share.pas:2940 nDummyMPTime_DF: Integer`（默认 800，见 :5517）。</summary>
    public static int nDummyMPTime_DF = 800;

    /// <summary>`M2Share.pas:2941 nDummyHeroMPTime_Warrior: Integer`（默认 800，见 :5518）。</summary>
    public static int nDummyHeroMPTime_Warrior = 800;

    /// <summary>`M2Share.pas:2942 nDummyHeroMPTime_DF: Integer`（默认 800，见 :5519）。</summary>
    public static int nDummyHeroMPTime_DF = 800;

    /// <summary>`M2Share.pas:2944 nDummyHPBase_Warrior: Integer`（默认 75，见 :5521）。</summary>
    public static int nDummyHPBase_Warrior = 75;

    /// <summary>`M2Share.pas:2945 nDummyHPBase_DF: Integer`（默认 75，见 :5522）。</summary>
    public static int nDummyHPBase_DF = 75;

    /// <summary>`M2Share.pas:2946 nDummyHeroHPBase_Warrior: Integer`（默认 75，见 :5523）。</summary>
    public static int nDummyHeroHPBase_Warrior = 75;

    /// <summary>`M2Share.pas:2947 nDummyHeroHPBase_DF: Integer`（默认 75，见 :5524）。</summary>
    public static int nDummyHeroHPBase_DF = 75;

    /// <summary>`M2Share.pas:2949 nDummyMPBase_Warrior: Integer`（默认 18，见 :5526）。</summary>
    public static int nDummyMPBase_Warrior = 18;

    /// <summary>`M2Share.pas:2950 nDummyMPBase_DF: Integer`（默认 18，见 :5527）。</summary>
    public static int nDummyMPBase_DF = 18;

    /// <summary>`M2Share.pas:2951 nDummyHeroMPBase_Warrior: Integer`（默认 18，见 :5528）。</summary>
    public static int nDummyHeroMPBase_Warrior = 18;

    /// <summary>`M2Share.pas:2952 nDummyHeroMPBase_DF: Integer`（默认 18，见 :5529）。</summary>
    public static int nDummyHeroMPBase_DF = 18;
}
