// ============================================================================
// 源单元：Source\M2Engine\NpcCommon.pas（GBK）
// 本文件：NPC 脚本命令**派发基础设施** 1:1 —— 原文三件套：
//   ① `nNF_*` 命令号常量（NpcCommon.pas:10-144，共 68 条，值 1..68）
//   ② `sNF_*` 标签常量（NpcCommon.pas:33-…，共 68 条）
//   ③ `g_NpcProcessCommand` 标签→命令号表（`:1899-1962` 的 68 条 `AddObject`）
//
// 它是 `TMerchant.UserSelect`(2087-2899) 派发体 2547-2696 的前置：
//   `nIndex := g_NpcProcessCommand.IndexOf(sLabel);`
//   `nIndex := Integer(g_NpcProcessCommand.Objects[nIndex]);`
//   `case nIndex of … nNF_XXX: …`
//
// ★ 归属说明（调度方裁定）：这套基础设施**就是 NPC 层的东西**，故落在 `Npc/` 下；
//   原文名 `g_NpcProcessCommand` / `nNF_*` / `sNF_*` **一律保留**，便于逐条回读比对。
// ============================================================================

using System;
using System.Collections.Generic;

namespace GXX.M2Server.Npc;

/// <summary>
/// 原文 `NpcCommon.pas` 的 `nNF_*`（命令号）与 `sNF_*`（标签）常量，以及
/// `g_NpcProcessCommand`（`TStringList`，`Objects[i]` 存 `TObject(nNF_指挥号)`）。
/// <para><b>照抄要点</b>：原文用 `TStringList.AddObject(sLabel, TObject(nNF))` —— 标签是
/// **有序**表，`IndexOf` 取到下标后经 `Objects[idx]` 还原命令号。托管侧用
/// `Dictionary&lt;string,int&gt;` + 一个 `List&lt;string&gt;` 保序（原文的 `IndexOf` 语义只需"查得到"，
/// 但保序可在将来需要 `Strings[i]` 时不受影响）。</para>
/// <para>⚠ 原文 `TObject(nNF)` 是把 **Integer 直接当指针**存（`NpcCommon.pas:1899-1962`），
/// 故托管用 `int` 存值即可 —— 这是原文的"指针即整数"用法，不是类型错误。</para>
/// </summary>
public static class NpcProcessCmd
{
    // -----------------------------------------------------------------------
    // ① nNF_* 命令号（NpcCommon.pas:10-144，值 1..68）—— 逐条标注原文行
    // -----------------------------------------------------------------------

    /// <summary>`nNF_ReclaimItem = 1`（NpcCommon.pas:10）</summary>
    public const int nNF_ReclaimItem = 1;
    /// <summary>`nNF_PlayDrink = 2`（NpcCommon.pas:12）</summary>
    public const int nNF_PlayDrink = 2;
    /// <summary>`nNF_PlayMakeWine = 3`（NpcCommon.pas:14）</summary>
    public const int nNF_PlayMakeWine = 3;
    /// <summary>`nNF_BuHero = 4`（NpcCommon.pas:16）</summary>
    public const int nNF_BuHero = 4;
    /// <summary>`nNF_CreateHero = 5`（NpcCommon.pas:18）</summary>
    public const int nNF_CreateHero = 5;
    /// <summary>`nNF_CreateDeputy = 6`（NpcCommon.pas:20）</summary>
    public const int nNF_CreateDeputy = 6;
    /// <summary>`nNF_UpgradeNew = 7`（NpcCommon.pas:22）</summary>
    public const int nNF_UpgradeNew = 7;
    /// <summary>`nNF_SendMsg = 8`（NpcCommon.pas:24）</summary>
    public const int nNF_SendMsg = 8;
    /// <summary>`nNF_SuperRepair = 9`（NpcCommon.pas:26）</summary>
    public const int nNF_SuperRepair = 9;
    /// <summary>`nNF_SuperRepairOK = 10`（NpcCommon.pas:28）</summary>
    public const int nNF_SuperRepairOK = 10;
    /// <summary>`nNF_SuperRepairFail = 11`（NpcCommon.pas:30）</summary>
    public const int nNF_SuperRepairFail = 11;
    /// <summary>`nNF_Repair = 12`（NpcCommon.pas:32）</summary>
    public const int nNF_Repair = 12;
    /// <summary>`nNF_RepairOK = 13`（NpcCommon.pas:34）</summary>
    public const int nNF_RepairOK = 13;
    /// <summary>`nNF_Buy = 14`（NpcCommon.pas:36）</summary>
    public const int nNF_Buy = 14;
    /// <summary>`nNF_Sell = 15`（NpcCommon.pas:38）</summary>
    public const int nNF_Sell = 15;
    /// <summary>`nNF_Trading = 16`（NpcCommon.pas:40）</summary>
    public const int nNF_Trading = 16;
    /// <summary>`nNF_MakedUrg = 17`（NpcCommon.pas:42）—— 注意原文拼写是 `MakedUrg`（不是 `MakeDrug`）</summary>
    public const int nNF_MakedUrg = 17;
    /// <summary>`nNF_Prices = 18`（NpcCommon.pas:44）</summary>
    public const int nNF_Prices = 18;
    /// <summary>`nNF_Storage = 19`（NpcCommon.pas:46）</summary>
    public const int nNF_Storage = 19;
    /// <summary>`nNF_Storage2 = 20`（NpcCommon.pas:48）</summary>
    public const int nNF_Storage2 = 20;
    /// <summary>`nNF_Storage3 = 21`（NpcCommon.pas:50）</summary>
    public const int nNF_Storage3 = 21;
    /// <summary>`nNF_Storage4 = 22`（NpcCommon.pas:52）</summary>
    public const int nNF_Storage4 = 22;
    /// <summary>`nNF_Getback = 23`（NpcCommon.pas:54）</summary>
    public const int nNF_Getback = 23;
    /// <summary>`nNF_Getback2 = 24`（NpcCommon.pas:56）</summary>
    public const int nNF_Getback2 = 24;
    /// <summary>`nNF_Getback3 = 25`（NpcCommon.pas:58）</summary>
    public const int nNF_Getback3 = 25;
    /// <summary>`nNF_Getback4 = 26`（NpcCommon.pas:60）</summary>
    public const int nNF_Getback4 = 26;
    /// <summary>`nNF_BigStorage = 27`（NpcCommon.pas:62）</summary>
    public const int nNF_BigStorage = 27;
    /// <summary>`nNF_BigGetback = 28`（NpcCommon.pas:64）</summary>
    public const int nNF_BigGetback = 28;
    /// <summary>`nNF_GetPreviousPage = 29`（NpcCommon.pas:66）</summary>
    public const int nNF_GetPreviousPage = 29;
    /// <summary>`nNF_GetNextPage = 30`（NpcCommon.pas:68）</summary>
    public const int nNF_GetNextPage = 30;
    /// <summary>`nNF_ArmRemoveStone = 31`（NpcCommon.pas:70）</summary>
    public const int nNF_ArmRemoveStone = 31;
    /// <summary>`nNF_UpgradeNow = 32`（NpcCommon.pas:72）</summary>
    public const int nNF_UpgradeNow = 32;
    /// <summary>`nNF_Upgradeing = 33`（NpcCommon.pas:74）—— 原文拼写 `Upgradeing`（多一个 e），照抄</summary>
    public const int nNF_Upgradeing = 33;
    /// <summary>`nNF_UpgradeOK = 34`（NpcCommon.pas:76）</summary>
    public const int nNF_UpgradeOK = 34;
    /// <summary>`nNF_UpgradeFail = 35`（NpcCommon.pas:78）</summary>
    public const int nNF_UpgradeFail = 35;
    /// <summary>`nNF_GetBackupgNow = 36`（NpcCommon.pas:80）</summary>
    public const int nNF_GetBackupgNow = 36;
    /// <summary>`nNF_GetBackupgOK = 37`（NpcCommon.pas:82）</summary>
    public const int nNF_GetBackupgOK = 37;
    /// <summary>`nNF_GetBackupgFail = 38`（NpcCommon.pas:84）</summary>
    public const int nNF_GetBackupgFail = 38;
    /// <summary>`nNF_GetBackupgFull = 39`（NpcCommon.pas:86）</summary>
    public const int nNF_GetBackupgFull = 39;
    /// <summary>`nNF_GetBackupging = 40`（NpcCommon.pas:88）</summary>
    public const int nNF_GetBackupging = 40;
    /// <summary>`nNF_Exit = 41`（NpcCommon.pas:90）</summary>
    public const int nNF_Exit = 41;
    /// <summary>`nNF_Back = 42`（NpcCommon.pas:92）</summary>
    public const int nNF_Back = 42;
    /// <summary>`nNF_Main = 43`（NpcCommon.pas:94）—— `@main`</summary>
    public const int nNF_Main = 43;
    /// <summary>`nNF_FailMain = 44`（NpcCommon.pas:96）—— `~@main`</summary>
    public const int nNF_FailMain = 44;
    /// <summary>`nNF_GetMaster = 45`（NpcCommon.pas:98）</summary>
    public const int nNF_GetMaster = 45;
    /// <summary>`nNF_GetMarry = 46`（NpcCommon.pas:100）</summary>
    public const int nNF_GetMarry = 46;
    /// <summary>`nNF_UseItemName = 47`（NpcCommon.pas:102）</summary>
    public const int nNF_UseItemName = 47;
    /// <summary>`nNF_Rmst = 48`（NpcCommon.pas:104，`// 接受歌曲`）</summary>
    public const int nNF_Rmst = 48;
    /// <summary>`nNF_OfflineMsg = 49`（NpcCommon.pas:106）</summary>
    public const int nNF_OfflineMsg = 49;
    /// <summary>`nNF_StartDealGold = 50`（NpcCommon.pas:108，`// 元宝转帐`）</summary>
    public const int nNF_StartDealGold = 50;
    /// <summary>`nNF_DealGold = 51`（NpcCommon.pas:110）</summary>
    public const int nNF_DealGold = 51;
    /// <summary>`nNF_InputInteger = 52`（NpcCommon.pas:112）</summary>
    public const int nNF_InputInteger = 52;
    /// <summary>`nNF_InputString = 53`（NpcCommon.pas:114）</summary>
    public const int nNF_InputString = 53;
    /// <summary>`nNF_BuildGuildNow = 54`（NpcCommon.pas:116）</summary>
    public const int nNF_BuildGuildNow = 54;
    /// <summary>`nNF_GuildWar = 55`（NpcCommon.pas:118）</summary>
    public const int nNF_GuildWar = 55;
    /// <summary>`nNF_Donate = 56`（NpcCommon.pas:120）</summary>
    public const int nNF_Donate = 56;
    /// <summary>`nNF_RequestCastleWar = 57`（NpcCommon.pas:122）</summary>
    public const int nNF_RequestCastleWar = 57;
    /// <summary>`nNF_CastleName = 58`（NpcCommon.pas:124）</summary>
    public const int nNF_CastleName = 58;
    /// <summary>`nNF_WithDrawal = 59`（NpcCommon.pas:126）</summary>
    public const int nNF_WithDrawal = 59;
    /// <summary>`nNF_Receipts = 60`（NpcCommon.pas:128）</summary>
    public const int nNF_Receipts = 60;
    /// <summary>`nNF_OpenMainDoor = 61`（NpcCommon.pas:130）</summary>
    public const int nNF_OpenMainDoor = 61;
    /// <summary>`nNF_CloseMainDoor = 62`（NpcCommon.pas:132）</summary>
    public const int nNF_CloseMainDoor = 62;
    /// <summary>`nNF_RepairDoorNow = 63`（NpcCommon.pas:134）</summary>
    public const int nNF_RepairDoorNow = 63;
    /// <summary>`nNF_RepairWallNow1 = 64`（NpcCommon.pas:136）</summary>
    public const int nNF_RepairWallNow1 = 64;
    /// <summary>`nNF_RepairWallNow2 = 65`（NpcCommon.pas:138）</summary>
    public const int nNF_RepairWallNow2 = 65;
    /// <summary>`nNF_RepairWallNow3 = 66`（NpcCommon.pas:140）</summary>
    public const int nNF_RepairWallNow3 = 66;
    /// <summary>`nNF_HirearcherNow = 67`（NpcCommon.pas:142）—— 原文拼写 `Hirearcher`（小写 a），照抄</summary>
    public const int nNF_HirearcherNow = 67;
    /// <summary>`nNF_HireguardNow = 68`（NpcCommon.pas:144）</summary>
    public const int nNF_HireguardNow = 68;

    // -----------------------------------------------------------------------
    // ② sNF_* 标签（NpcCommon.pas:33 起，与 ① 一一对应）
    // -----------------------------------------------------------------------

    /// <summary>`sNF_ReclaimItem = '@ReclaimItem'`</summary>
    public const string sNF_ReclaimItem = "@ReclaimItem";
    /// <summary>`sNF_PlayDrink = '@PlayDrink'`</summary>
    public const string sNF_PlayDrink = "@PlayDrink";
    /// <summary>`sNF_PlayMakeWine = '@PlayMakeWine'`（`// 酿酒 标识`）</summary>
    public const string sNF_PlayMakeWine = "@PlayMakeWine";
    /// <summary>`sNF_BuHero = '@BuHero'`</summary>
    public const string sNF_BuHero = "@BuHero";
    /// <summary>`sNF_CreateHero = '@@CreateHero'` —— 注意是**两个** `@`</summary>
    public const string sNF_CreateHero = "@@CreateHero";
    /// <summary>`sNF_CreateDeputy = '@@BuHero'`</summary>
    public const string sNF_CreateDeputy = "@@BuHero";
    /// <summary>`sNF_UpgradeNew = '@upgradenew'`</summary>
    public const string sNF_UpgradeNew = "@upgradenew";
    /// <summary>`sNF_SendMsg = '@@sendmsg'`</summary>
    public const string sNF_SendMsg = "@@sendmsg";
    /// <summary>`sNF_SuperRepair = '@s_repair'`</summary>
    public const string sNF_SuperRepair = "@s_repair";
    /// <summary>`sNF_SuperRepairOK = '~@s_repair'`</summary>
    public const string sNF_SuperRepairOK = "~@s_repair";
    /// <summary>`sNF_SuperRepairFail = '@fail_s_repair'`</summary>
    public const string sNF_SuperRepairFail = "@fail_s_repair";
    /// <summary>`sNF_Repair = '@repair'`</summary>
    public const string sNF_Repair = "@repair";
    /// <summary>`sNF_RepairOK = '~@repair'`</summary>
    public const string sNF_RepairOK = "~@repair";
    /// <summary>`sNF_Buy = '@buy'`</summary>
    public const string sNF_Buy = "@buy";
    /// <summary>`sNF_Sell = '@sell'`</summary>
    public const string sNF_Sell = "@sell";
    /// <summary>`sNF_Trading = '@trading'`</summary>
    public const string sNF_Trading = "@trading";
    /// <summary>`sNF_MakedUrg = '@makedrug'`</summary>
    public const string sNF_MakedUrg = "@makedrug";
    /// <summary>`sNF_Prices = '@prices'`</summary>
    public const string sNF_Prices = "@prices";
    /// <summary>`sNF_Storage = '@storage'`</summary>
    public const string sNF_Storage = "@storage";
    /// <summary>`sNF_Storage2 = '@storage2'`</summary>
    public const string sNF_Storage2 = "@storage2";
    /// <summary>`sNF_Storage3 = '@storage3'`</summary>
    public const string sNF_Storage3 = "@storage3";
    /// <summary>`sNF_Storage4 = '@storage4'`</summary>
    public const string sNF_Storage4 = "@storage4";
    /// <summary>`sNF_Getback = '@getback'`</summary>
    public const string sNF_Getback = "@getback";
    /// <summary>`sNF_Getback2 = '@getback2'`</summary>
    public const string sNF_Getback2 = "@getback2";
    /// <summary>`sNF_Getback3 = '@getback3'`</summary>
    public const string sNF_Getback3 = "@getback3";
    /// <summary>`sNF_Getback4 = '@getback4'`</summary>
    public const string sNF_Getback4 = "@getback4";
    /// <summary>`sNF_BigStorage = '@bigstorage'`</summary>
    public const string sNF_BigStorage = "@bigstorage";
    /// <summary>`sNF_BigGetback = '@biggetback'`</summary>
    public const string sNF_BigGetback = "@biggetback";
    /// <summary>`sNF_GetPreviousPage = '@getpreviouspage'`</summary>
    public const string sNF_GetPreviousPage = "@getpreviouspage";
    /// <summary>`sNF_GetNextPage = '@getnextpage'`</summary>
    public const string sNF_GetNextPage = "@getnextpage";
    /// <summary>`sNF_ArmRemoveStone = '@armremovestone'`</summary>
    public const string sNF_ArmRemoveStone = "@armremovestone";
    /// <summary>`sNF_UpgradeNow = '@upgradenow'`</summary>
    public const string sNF_UpgradeNow = "@upgradenow";
    /// <summary>`sNF_Upgradeing = '~@upgradenow_ing'`（NpcCommon.pas:75）</summary>
    public const string sNF_Upgradeing = "~@upgradenow_ing";
    /// <summary>`sNF_UpgradeOK = '~@upgradenow_ok'`（NpcCommon.pas:77）</summary>
    public const string sNF_UpgradeOK = "~@upgradenow_ok";
    /// <summary>`sNF_UpgradeFail = '~@upgradenow_fail'`（NpcCommon.pas:79）</summary>
    public const string sNF_UpgradeFail = "~@upgradenow_fail";
    /// <summary>`sNF_GetBackupgNow = '@getbackupgnow'`</summary>
    public const string sNF_GetBackupgNow = "@getbackupgnow";
    /// <summary>`sNF_GetBackupgOK = '~@getbackupgnow_ok'`</summary>
    public const string sNF_GetBackupgOK = "~@getbackupgnow_ok";
    /// <summary>`sNF_GetBackupgFail = '~@getbackupgnow_fail'`</summary>
    public const string sNF_GetBackupgFail = "~@getbackupgnow_fail";
    /// <summary>`sNF_GetBackupgFull = '~@getbackupgnow_bagfull'`</summary>
    public const string sNF_GetBackupgFull = "~@getbackupgnow_bagfull";
    /// <summary>`sNF_GetBackupging = '~@getbackupgnow_ing'`</summary>
    public const string sNF_GetBackupging = "~@getbackupgnow_ing";
    /// <summary>`sNF_Exit = '@exit'`</summary>
    public const string sNF_Exit = "@exit";
    /// <summary>`sNF_Back = '@back'`</summary>
    public const string sNF_Back = "@back";
    /// <summary>`sNF_Main = '@main'`</summary>
    public const string sNF_Main = "@main";
    /// <summary>`sNF_FailMain = '~@main'`</summary>
    public const string sNF_FailMain = "~@main";
    /// <summary>`sNF_GetMaster = '@@getmaster'`</summary>
    public const string sNF_GetMaster = "@@getmaster";
    /// <summary>`sNF_GetMarry = '@@getmarry'`</summary>
    public const string sNF_GetMarry = "@@getmarry";
    /// <summary>`sNF_UseItemName = '@@useitemname'`</summary>
    public const string sNF_UseItemName = "@@useitemname";
    /// <summary>`sNF_Rmst = '@@rmst'`</summary>
    public const string sNF_Rmst = "@@rmst";
    /// <summary>`sNF_OfflineMsg = '@@offlinemsg'`</summary>
    public const string sNF_OfflineMsg = "@@offlinemsg";
    /// <summary>`sNF_StartDealGold = '@startdealgold'`</summary>
    public const string sNF_StartDealGold = "@startdealgold";
    /// <summary>`sNF_DealGold = '@@dealgold'`</summary>
    public const string sNF_DealGold = "@@dealgold";
    /// <summary>`sNF_InputInteger = '@@InputInteger'` —— 注意 I 大写</summary>
    public const string sNF_InputInteger = "@@InputInteger";
    /// <summary>`sNF_InputString = '@@InputString'` —— 注意 I 大写</summary>
    public const string sNF_InputString = "@@InputString";
    /// <summary>`sNF_BuildGuildNow = '@@buildguildnow'`</summary>
    public const string sNF_BuildGuildNow = "@@buildguildnow";
    /// <summary>`sNF_GuildWar = '@@guildwar'`</summary>
    public const string sNF_GuildWar = "@@guildwar";
    /// <summary>`sNF_Donate = '@@donate'`</summary>
    public const string sNF_Donate = "@@donate";
    /// <summary>`sNF_RequestCastleWar = '@requestcastlewarnow'`</summary>
    public const string sNF_RequestCastleWar = "@requestcastlewarnow";
    /// <summary>`sNF_CastleName = '@@castlename'`</summary>
    public const string sNF_CastleName = "@@castlename";
    /// <summary>`sNF_WithDrawal = '@@withdrawal'`</summary>
    public const string sNF_WithDrawal = "@@withdrawal";
    /// <summary>`sNF_Receipts = '@@receipts'`</summary>
    public const string sNF_Receipts = "@@receipts";
    /// <summary>`sNF_OpenMainDoor = '@openmaindoor'`</summary>
    public const string sNF_OpenMainDoor = "@openmaindoor";
    /// <summary>`sNF_CloseMainDoor = '@closemaindoor'`</summary>
    public const string sNF_CloseMainDoor = "@closemaindoor";
    /// <summary>`sNF_RepairDoorNow = '@repairdoornow'`</summary>
    public const string sNF_RepairDoorNow = "@repairdoornow";
    /// <summary>`sNF_RepairWallNow1 = '@repairwallnow1'`</summary>
    public const string sNF_RepairWallNow1 = "@repairwallnow1";
    /// <summary>`sNF_RepairWallNow2 = '@repairwallnow2'`</summary>
    public const string sNF_RepairWallNow2 = "@repairwallnow2";
    /// <summary>`sNF_RepairWallNow3 = '@repairwallnow3'`</summary>
    public const string sNF_RepairWallNow3 = "@repairwallnow3";
    /// <summary>`sNF_HirearcherNow = '@hirearchernow'`</summary>
    public const string sNF_HirearcherNow = "@hirearchernow";
    /// <summary>`sNF_HireguardNow = '@hireguardnow'`</summary>
    public const string sNF_HireguardNow = "@hireguardnow";

    // -----------------------------------------------------------------------
    // ③ g_NpcProcessCommand —— 标签→命令号表（NpcCommon.pas:1899-1962 的 68 条 AddObject）
    // -----------------------------------------------------------------------

    /// <summary>
    /// 原文 `g_NpcProcessCommand: TStringList`（`NpcCommon.pas` 单元级变量，
    /// 在 `InitNpcProcessCommand`（约 `:1899-1962`）里逐条 `AddObject(sLabel, TObject(nNF))`）。
    /// <para>托管侧：`Values` 保标签→命令号；<see cref="Order"/> 保原文的加入顺序
    /// （原文是 `TStringList`，`Strings[i]` 有序 —— 将来若需按序枚举可用）。</para>
    /// <para><b>查表语义</b>：<see cref="IndexOf"/> 找不到返回 `-1`（对应原文 `IndexOf` 返回 -1
    /// 时 `nIndex &gt;= 0` 为假）；<see cref="GetCommand"/> 组合了原文的
    /// `nIndex := ...IndexOf(sLabel); nIndex := Integer(...Objects[nIndex]);` 两步
    /// （`ObjNpc.pas:2691-2694`），未命中同样返回 `-1`（原文此时 `nIndex` 保持 -1 以外的
    /// 初值路径见 `UserSelect` 的 `nCode` 逻辑，届时按原文处理）。</para>
    /// </summary>
    public static class g_NpcProcessCommand
    {
        private static readonly Dictionary<string, int> Values = new(StringComparer.Ordinal);
        private static readonly List<string> OrderList = new();

        /// <summary>原文标签的加入顺序（`TStringList.Strings` 的托管等价物，只读视图）。</summary>
        public static IReadOnlyList<string> Order => OrderList;

        /// <summary>原文 `g_NpcProcessCommand.AddObject(sLabel, TObject(nNF))`。</summary>
        public static void AddObject(string sLabel, int nNF)
        {
            Values[sLabel] = nNF;
            OrderList.Add(sLabel);
        }

        /// <summary>
        /// 原文 `g_NpcProcessCommand.IndexOf(sLabel)`（`ObjNpc.pas:2691`）。
        /// <para>⚠ `TStringList.IndexOf` **不区分大小写**（Delphi `TStringList` 默认
        /// `CaseSensitive = False`）→ 用 `OrdinalIgnoreCase`。</para>
        /// </summary>
        public static int IndexOf(string sLabel)
        {
            foreach (var kv in Values)
            {
                if (string.Equals(kv.Key, sLabel, StringComparison.OrdinalIgnoreCase))
                    return OrderList.IndexOf(kv.Key);
            }
            return -1;
        }

        /// <summary>
        /// 原文 `nIndex := g_NpcProcessCommand.IndexOf(sLabel);`
        /// `nIndex := Integer(g_NpcProcessCommand.Objects[nIndex]);`（`ObjNpc.pas:2691-2694`）。
        /// 未命中返回 `-1`。
        /// </summary>
        public static int GetCommand(string sLabel)
        {
            int index = IndexOf(sLabel);
            if (index < 0 || index >= OrderList.Count)
                return -1;
            return Values[OrderList[index]];
        }

        /// <summary>表内条目数（原文 68 条；供测试与回读比对）。</summary>
        public static int Count => OrderList.Count;

        /// <summary>
        /// 原文 `InitNpcProcessCommand`（`NpcCommon.pas:1899-1962`）的 68 条注册，**顺序照抄**。
        /// <para>由静态构造调用一次；<see cref="Reset"/> 供单测隔离。</para>
        /// </summary>
        public static void Init()
        {
            AddObject(sNF_ReclaimItem, nNF_ReclaimItem);
            AddObject(sNF_PlayDrink, nNF_PlayDrink);
            AddObject(sNF_PlayMakeWine, nNF_PlayMakeWine);
            AddObject(sNF_BuHero, nNF_BuHero);
            AddObject(sNF_CreateHero, nNF_CreateHero);
            AddObject(sNF_CreateDeputy, nNF_CreateDeputy);
            AddObject(sNF_UpgradeNew, nNF_UpgradeNew);
            AddObject(sNF_SendMsg, nNF_SendMsg);
            AddObject(sNF_SuperRepair, nNF_SuperRepair);
            AddObject(sNF_SuperRepairOK, nNF_SuperRepairOK);
            AddObject(sNF_SuperRepairFail, nNF_SuperRepairFail);
            AddObject(sNF_Repair, nNF_Repair);
            AddObject(sNF_RepairOK, nNF_RepairOK);
            AddObject(sNF_Buy, nNF_Buy);
            AddObject(sNF_Trading, nNF_Trading);
            AddObject(sNF_Sell, nNF_Sell);
            AddObject(sNF_MakedUrg, nNF_MakedUrg);
            AddObject(sNF_Prices, nNF_Prices);
            AddObject(sNF_Storage, nNF_Storage);
            AddObject(sNF_Storage2, nNF_Storage2);
            AddObject(sNF_Storage3, nNF_Storage3);
            AddObject(sNF_Storage4, nNF_Storage4);
            AddObject(sNF_Getback, nNF_Getback);
            AddObject(sNF_Getback2, nNF_Getback2);
            AddObject(sNF_Getback3, nNF_Getback3);
            AddObject(sNF_Getback4, nNF_Getback4);
            AddObject(sNF_BigStorage, nNF_BigStorage);
            AddObject(sNF_BigGetback, nNF_BigGetback);
            AddObject(sNF_GetPreviousPage, nNF_GetPreviousPage);
            AddObject(sNF_GetNextPage, nNF_GetNextPage);
            AddObject(sNF_ArmRemoveStone, nNF_ArmRemoveStone);
            AddObject(sNF_UpgradeNow, nNF_UpgradeNow);
            AddObject(sNF_Upgradeing, nNF_Upgradeing);
            AddObject(sNF_UpgradeOK, nNF_UpgradeOK);
            AddObject(sNF_UpgradeFail, nNF_UpgradeFail);
            AddObject(sNF_GetBackupgNow, nNF_GetBackupgNow);
            AddObject(sNF_GetBackupgOK, nNF_GetBackupgOK);
            AddObject(sNF_GetBackupgFail, nNF_GetBackupgFail);
            AddObject(sNF_GetBackupgFull, nNF_GetBackupgFull);
            AddObject(sNF_GetBackupging, nNF_GetBackupging);
            AddObject(sNF_Exit, nNF_Exit);
            AddObject(sNF_Back, nNF_Back);
            AddObject(sNF_Main, nNF_Main);
            AddObject(sNF_FailMain, nNF_FailMain);
            AddObject(sNF_GetMaster, nNF_GetMaster);
            AddObject(sNF_GetMarry, nNF_GetMarry);
            AddObject(sNF_UseItemName, nNF_UseItemName);
            AddObject(sNF_Rmst, nNF_Rmst);
            AddObject(sNF_OfflineMsg, nNF_OfflineMsg);
            AddObject(sNF_StartDealGold, nNF_StartDealGold);
            AddObject(sNF_DealGold, nNF_DealGold);
            AddObject(sNF_InputInteger, nNF_InputInteger);
            AddObject(sNF_InputString, nNF_InputString);
            AddObject(sNF_BuildGuildNow, nNF_BuildGuildNow);
            AddObject(sNF_GuildWar, nNF_GuildWar);
            AddObject(sNF_Donate, nNF_Donate);
            AddObject(sNF_RequestCastleWar, nNF_RequestCastleWar);
            AddObject(sNF_CastleName, nNF_CastleName);
            AddObject(sNF_WithDrawal, nNF_WithDrawal);
            AddObject(sNF_Receipts, nNF_Receipts);
            AddObject(sNF_OpenMainDoor, nNF_OpenMainDoor);
            AddObject(sNF_CloseMainDoor, nNF_CloseMainDoor);
            AddObject(sNF_RepairDoorNow, nNF_RepairDoorNow);
            AddObject(sNF_RepairWallNow1, nNF_RepairWallNow1);
            AddObject(sNF_RepairWallNow2, nNF_RepairWallNow2);
            AddObject(sNF_RepairWallNow3, nNF_RepairWallNow3);
            AddObject(sNF_HirearcherNow, nNF_HirearcherNow);
            AddObject(sNF_HireguardNow, nNF_HireguardNow);
        }

        /// <summary>清空并重新注册（单测隔离用；原文是单元级全局表，进程内只初始化一次）。</summary>
        public static void Reset()
        {
            Values.Clear();
            OrderList.Clear();
            Init();
        }

        static g_NpcProcessCommand() => Init();
    }
}
