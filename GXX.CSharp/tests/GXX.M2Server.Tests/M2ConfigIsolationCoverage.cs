// ============================================================================
// 车道 p6-test-isolation：静态全局「快照/还原」覆盖清单
//
// 口径：这里列出的**每个类型**的全部静态成员（见 M2ConfigIsolationState 的枚举规则）
// 都会在**每个测试用例前后**被快照/还原。
//
// 选取依据（按优先级）：
//   1. 任务点名的：M2Config（Delphi g_Config）、Grobal2/M2Share/GameConfigState 一类；
//   2. 测试里**实测被赋值且不还原**的静态全局（`Select-String -Pattern '^\s*<Type>\.\w+\s*=[^=]'`）：
//      M2Config 417 处、M2Forms 139、GameConfigState 10、DropLimitGlobals 7、InterServerState 7、
//      M2ShareAbilConfig 6、ViewList2State 5、M2ShareLimits 4、DbLayerGlobals 3、SndaShopEnv 2、
//      ViewListGroups 2、GHeroDBConfig 1、M2ShareState 1（外加 `using static` 写法）；
//   3. 同类的静态全局配置/状态持有者（同一机制顺手纳入，零额外成本）。
//
// 明确**排除**的（理由见 docs/并行报告-p6-test-isolation.md）：
//   * 纯常量/查找表类（DropLimitConsts / ObjNpcConst / NpcCmdCodes / DataItemTypes /
//     SqliteCodes / Sqlite*Sql / MySql*Sql / GameCommandTables / ItemSetAddValueTables …）——
//     只有 const / static readonly，没有可赋值静态状态；
//   * 测试自己的接缝类（NpcSeams / AuctionDbRunSeam / CustomMagicFormGlobals /
//     CustomMagicMessageBoxSeam / TVtEditorFactory / DbLayerTestKit …）—— 属于各在飞车道
//     的独占文件，且它们的测试基类已经自己 Reset（本车道无权改，也不应越界覆盖）；
//   * 静态窗体注册表（M2Forms 已单独纳入，M2FormRegistry 一类持有的是窗体实例而不是配置值）。
// ============================================================================

using System;
using GXX.M2Server.DbLayer;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.GameCenter;

namespace GXX.M2Server.Tests;

/// <summary>被隔离机制覆盖的静态全局类型清单（反射口径，非手写成员清单）。</summary>
internal static class M2ConfigIsolationCoverage
{
    internal static readonly Type[] Types =
    {
        // --- 1. 主目标：Delphi g_Config ---
        typeof(M2Config),

        // --- 2. 实测被测试改写且不还原的静态全局 ---
        typeof(M2ShareState),        // FormGeneralConfigTests:489-491 用 using static 写 g_sDBName 等
        typeof(M2ShareGlobals),      // M2Share.pas 全局名单/表
        typeof(M2ShareLimits),
        typeof(M2ShareAbilConfig),
        typeof(M2ShareFuncs),
        typeof(GameConfigState),     // GameSpeed/SpeedControl 全局
        typeof(DropLimitGlobals),    // FormJ40/J41/J42 写
        typeof(InterServerState),
        typeof(ViewList2State),      // FormJ57 等写
        typeof(ViewListGroups),
        typeof(DbLayerGlobals),
        typeof(SndaShopEnv),
        typeof(GHeroDBConfig),
        typeof(M2Forms),             // Application.MessageBox 注入/捕获（MessageBoxHandler/NextAnswer/LastMessage）

        // --- 3. 同类的静态全局状态持有者（顺手纳入） ---
        typeof(ViewListState3),
        typeof(CastleState),
        typeof(MissionPageState),
        typeof(ClientModuleState),
        typeof(IdSocState),
        typeof(CustomHeroMagicState),
        typeof(PluginManagerState),
        typeof(M2ServerLog),
    };
}
