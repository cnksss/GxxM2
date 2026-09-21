// ============================================================================
// 车道 p9-m2-forms ｜ 跨单元**补成员**（按《并行派发台账》§19.6 的 `partial` 手法）
//
// 背景：`ConfigMerchant.pas:297` 写 `SelMerchant.m_boDenyRefStatus`，该字段原文声明在
// `ObjBase.pas:340`（`m_boDenyRefStatus: Boolean; // 是否刷新在地图上信息；`）⇒
// 属 `TBaseObject`/`TCreature` 一族。托管侧 `GXX.M2Server.Engine.TCreature` 里
// **全树 0 命中**（只有两处注释提及，见 SendRefMsgCore.cs:133 / ViewRangeMaintainCore.cs:294）。
//
// 处置（照 §19.6：在自己的新文件里 `partial` 补齐，**不改任何既有文件**）：
//   在本文件为 `TCreature` 补上该字段。
//
// ⚠ **交接事项（务必登记）**：若顺序会话/其它车道日后在 `ObjBase.pas` 批次里
//   **正式落地** `m_boDenyRefStatus`，**必须删掉本文件**，否则 **CS0102**（重复定义）。
//   已在 docs\并行报告-p9-m2-forms.md「跨区事项」登记。
//
// 另：`m_boArmRemoveStone`（`ObjNpcClasses.cs:167`）与 `m_sScript`/`m_nFlag`/`m_wAppr`/
//   `m_dwMoveTime`/`m_ItemTypeList` 等均**已存在**，故**不**在此重复声明（避免 CS0102）。
// ============================================================================

namespace GXX.M2Server.Engine;

/// <summary>
/// `TCreature` 的补成员（原文 `ObjBase.pas:340 m_boDenyRefStatus: Boolean;
/// // 是否刷新在地图上信息；`）。
/// <para>
/// 消费点：`ObjBase.pas:11456`（置 True）、`:21845`、`:31164`、`:31796`（读）；
/// `ObjPlayer.pas:3983`、`:15773`（读）；**本车道的 `ConfigMerchant.pas:297`（写）**。
/// </para>
/// </summary>
public abstract partial class TCreature
{
    /// <summary>原文 `ObjBase.pas:340 m_boDenyRefStatus: Boolean; // 是否刷新在地图上信息；</summary>
    public bool m_boDenyRefStatus;
}
