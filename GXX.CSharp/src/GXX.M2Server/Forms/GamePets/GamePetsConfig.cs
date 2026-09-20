// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmMainGamePets.pas（1,353 行，GBK）
//  本文件：`g_Config`（M2Share.pas 全局配置对象）的**宠物字段子集**
//    · M2Share.pas:961-963  dwNeedExps / dwHeroNeedExps / dwPetNeedExps
//    · M2Share.pas 宠物开关族（uFrmMainGamePets.pas 直接读写的全部 g_Config 字段）
//
//  ⚠ 接缝说明（重要）：
//    `g_Config` 在托管侧是 `GXX.M2Server.Engine.M2Config`（static partial class）。
//    该类型所在的 `src/GXX.M2Server/Engine/**` 属**顺序会话常驻区（只读）**，
//    因此本车道**不修改** Engine 下任何既有文件：缺失的宠物字段以本 partial 分支追加，
//    对 Engine 侧既有引用（如 `AbilRecalc.cs:33` 的 `M2Config.dwPetNeedExps`）完全透明。
// ============================================================================

namespace GXX.M2Server.Engine;

/// <summary>
/// `M2Share.pas` 中 `g_Config` 的**宠物配置字段子集**（uFrmMainGamePets.pas 直接读写的全部字段）。
/// 字段名与 Delphi 逐字一致（含匈牙利前缀 bt/bo/n/dw/s）。
/// </summary>
/// <remarks>
/// 已由 Engine 侧既有文件提供的字段（**不在此重复声明**）：
///   `dwPetNeedExps` / `boPetUseFixExp` / `nPetAddExp` / `nPetBaseExp` /
///   `boPetHPToMaster` / `boPetDCToMaster` / `nPetAbilToMasterRate`
///   —— 见 `Engine/M2Config.ServerValue.cs:36-53`。
/// </remarks>
public static partial class M2Config
{
    // ---- DoOpen / 处理器族读写（uFrmMainGamePets.pas 用到、Engine 侧尚无的字段）----

    /// <summary>开启宠物系统。原 g_Config.boOpenGamePet（DoOpen :325 / chkOpenGamePetClick :704）。</summary>
    public static bool boOpenGamePet;

    /// <summary>允许宠物攻击（:326 / :736）。</summary>
    public static bool boEnabledPetAttack;

    /// <summary>禁止怪物攻击宠物（:327 / :1300）。</summary>
    public static bool boDisableMonAttackPet;

    /// <summary>禁止所有攻击宠物（:328 / :1308）。</summary>
    public static bool boDisableAllAttackPet;

    /// <summary>允许宠物捡物（:329 / :744）。</summary>
    public static bool boEnabledPetPickup;

    /// <summary>宠物只捡怪物掉落物（:330 / :752）。</summary>
    public static bool boPetOnlyPickMonsterItem;

    /// <summary>宠物直接捡物到主人背包（:331 / :760）。</summary>
    public static bool boPetPickupToMaster;

    /// <summary>宠物包满时捡到物品放主人包裹（:332 / :770）。</summary>
    public static bool boPetPickupFullToMaster;

    /// <summary>宠物快速捡物（:334 / :1316）。</summary>
    public static bool boPetQuickPickup;

    /// <summary>宠物范围捡物（:335 / :1324）。</summary>
    public static bool boPetRangePickup;

    /// <summary>宠物范围捡物半径（Byte；:336 / :1332）。</summary>
    public static byte btPetPickupRange;

    /// <summary>宝宝无实体（:338 / :712）。</summary>
    public static bool boPetNoEntity;

    /// <summary>宠物休息受宝宝控制（:339 / :728）。</summary>
    public static bool boPetSleepControlBySlave;

    /// <summary>宠物不显示 HP 进度条（:340 / :720）。</summary>
    public static bool boPetNoShowHPProgress;

    /// <summary>允许宠物使用客户端捡物列表（VMProtect 键控；:348/:355 / :1340）。</summary>
    public static bool boEnablePetUseClientPickItems;

    /// <summary>叠加魔法给主人（:363 / :802）。</summary>
    public static bool boPetMCToMaster;

    /// <summary>叠加道术给主人（:365 / :810）。</summary>
    public static bool boPetSCToMaster;

    /// <summary>叠加防御给主人（:366 / :818）。</summary>
    public static bool boPetACToMaster;

    /// <summary>叠加魔防给主人（:367 / :826）。</summary>
    public static bool boPetMACToMaster;

    /// <summary>宠物使用物品间隔时间（LongWord；:369 / :1252）。</summary>
    public static uint dwPetUseItemIntervalTime;

    /// <summary>捕捉宠物需要物品（:370 / :1260）。</summary>
    public static bool boCapturePetNeedItem;

    /// <summary>捕捉成功后扣物品持久（:371 / :1268）。</summary>
    public static bool boCaptureOKDecDura;

    /// <summary>宠物显示主人名字（:373 / :1228）。</summary>
    public static bool boPetShowMasterName;

    /// <summary>宠物名字颜色索引（Byte；:374 / :1236）。</summary>
    public static byte btPetNameColor;

    /// <summary>宠物后缀名（:375 / :1244 Trim 后写入）。</summary>
    public static string sPetSuffixName = "";

    /// <summary>游戏宠物数量上限（:377 / :1276）。</summary>
    public static int nGamePetMaxCount;

    /// <summary>游戏宠物名字数量上限（:378 / :1284）。</summary>
    public static int nGamePetNameCount;

    /// <summary>游戏宠物召回时间（:379 / :1292）。</summary>
    public static int nGamePetRecallTime;

    /// <summary>杀怪触发宠物（:381 / :1348）。</summary>
    public static bool boGamePetKillMonTrigger;

    // ---- 经验页字段（Experience 页 / btnSaveExp :1169-1173）----

    /// <summary>宠物升级经验点数（:310 / :1188）。</summary>
    public static int nPetHighLevel;

    /// <summary>宠物升级获得经验（:311 / :1196）。</summary>
    public static int nPetHighLevelGetExp;
}
