using System.Runtime.InteropServices;
using GXX.Core.Protocol;

namespace GXX.Client.GUI.Mir;

// ============================================================================================
// 【P17 切片3】MShare.pas `g_ClientConfig` 中本车道需要的三个字段 —— **有界跨区 seam**
//
// 为什么需要这个 seam：
//   原文这三处读的是 `g_ClientConfig:TConfigClient`（`MShare.pas:2180`，记录体声明在
//   `MShare.pas:493-703`）：
//     · `12079` `if not g_ClientConfig.boDisableWarrContinueHit then Exit;`
//     · `12088/12112` `for I := 0 to Length(g_ClientConfig.ArrDisableWarrContinueHitIDs) - 1`
//     · `12101` `... >= g_ClientConfig.nWarrContinueHitMinInterval + 100`
//   托管侧的 `g_ClientConfig` 由 **车道1 的 `MirForms.cs:23 TConfigClient`** 承载（**不在我的分区**，
//   本切片**不越区**），而它目前**没有**这三个字段。
//   这三个字段的**同名字段真身**其实已存在于 `GXX.Core.Protocol.TClientConfig`
//   （`Grobal2.Types5.cs:341-343`：`boDisableWarrContinueHit` / `nWarrContinueHitMinInterval` /
//   `ArrDisableWarrContinueHitIDs`），但那是另一个类型。
//
// ⇒ 本类只承载「这三个字段」，字段名与类型与 `Grobal2.Types5.cs:341-343` **逐字一致**，
//   使将来把 `MShareGlobals.g_ConfigClient` 收敛为 `GXX.Core.Protocol.TClientConfig` 时，
//   把 `MShareWarrConfigSeam.X` 改成 `MShareGlobals.g_ClientConfig.X` 即可**机械替换**。
// 【退役条件】见报告 CR-6：`MirForms.TConfigClient` 补上这三个字段（或 `g_ClientConfig` 收敛为
//   `Core.Protocol.TClientConfig`），随后删除本类。
//
// 计数对账（本文件）：真实体 4 / NotPorted 0 / 原文如此 0 = 4（3 字段 + 1 Reset）。
// ============================================================================================

/// <summary>
/// `g_ClientConfig` 中 `TWarrContinueHitManager` 需要的三个字段的**有界 seam**。
/// 字段名/类型与 `GXX.Core.Protocol.TClientConfig`（`Grobal2.Types5.cs:341-343`）逐字一致。
/// </summary>
public static class MShareWarrConfigSeam
{
    /// <summary>
    /// MShare.pas:12079 `g_ClientConfig.boDisableWarrContinueHit:Boolean`（禁用战士连击技能）。
    /// 原文是 `Boolean`；托管侧沿用 Core 对 `TClientConfig` 的既有承载（`byte`，0=False）。
    /// </summary>
    public static byte boDisableWarrContinueHit;

    /// <summary>
    /// MShare.pas:12101 `g_ClientConfig.nWarrContinueHitMinInterval`（连击最小间隔，毫秒）。
    /// 原文声明为 `Integer`，但 Core 的 `TClientConfig` 承载为 `uint`（`Grobal2.Types5.cs:342`）；
    /// 与 `tick_diff` 的 `Cardinal` 比较时按无符号语义，故此处跟 `uint`。
    /// </summary>
    public static uint nWarrContinueHitMinInterval;

    /// <summary>
    /// MShare.pas:12010/12088/12112 `g_ClientConfig.ArrDisableWarrContinueHitIDs`。
    /// 原文记录里是 `array[0..9] of Word`；Core 的 `TClientConfig` 用 `WordArray10`
    /// （`Grobal2.Types4.cs` 的 `[InlineArray(10)]`）。元素 0 = 表尾。
    /// </summary>
    public static WordArray10 ArrDisableWarrContinueHitIDs;

    /// <summary>测试用复位（回到原文的"配置未装载"状态）。</summary>
    public static void ResetForTests()
    {
        boDisableWarrContinueHit = 0;
        nWarrContinueHitMinInterval = 0;
        for (int i = 0; i < 10; i++)
            ArrDisableWarrContinueHitIDs[i] = 0;
    }
}
