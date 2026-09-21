// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：**例程覆盖登记表**（1:1 审计用，非可执行逻辑）。
// 数据来源：脚本从原文逐行抽取 `^(function|procedure|constructor|destructor)` 顶层例程，
//           行号区间 = [本例程起始行, 下一例程起始行 - 1]（末条止于 10544；原文 `end.` 在 10545）。
// 状态：
//   Covered - 已在本车道 Npc/ 内 1:1 实现
//   Seam    - 已声明最小接缝或虚方法外壳，未逐行移植；或例程**部分**落地（Home 里注明范围）
//   Missing - 未覆盖（原文在本车道未落地）
// 注：嵌套过程（如 UpgradeWapon 内的 sub_4A0218）不单独成条 —— 原文里它们不是顶层例程。
// ============================================================================

namespace GXX.M2Server.Npc;

/// <summary>ObjNpc.pas 例程覆盖登记（每条含原文行号区间、签名、状态、归属）。</summary>
public static class ObjNpcRoutineRegistry
{
    /// <summary>单条例程登记。</summary>
    public sealed class Entry
    {
        /// <summary>原文起始行（含）。</summary>
        public int StartLine;
        /// <summary>原文结束行（含）。</summary>
        public int EndLine;
        /// <summary>原文例程签名（逐字）。</summary>
        public string Signature = "";
        /// <summary>Covered / Seam / Missing。</summary>
        public string Status = "";
        /// <summary>归属文件或接缝名（Missing 时为空）。</summary>
        public string Home = "";
    }

    /// <summary>全部 112 条顶层例程（原文 implementation 段 496-10544）。</summary>
    public static readonly Entry[] All =
    {
        new() { StartLine = 504, EndLine = 508, Signature = "constructor TConditionList.Create;", Status = "Covered", Home = "ObjNpcTypes.cs" },
        new() { StartLine = 509, EndLine = 595, Signature = "function LoadLevelScriptAction(QuestActionInfo: pTQuestActionInfo; sCmd: string): string;", Status = "Covered", Home = "ObjNpcUnitFuncs.cs" },
        new() { StartLine = 596, EndLine = 681, Signature = "function LoadLevelScriptCondition(QuestConditionInfo: pTQuestConditionInfo; sCmd: string): string;", Status = "Covered", Home = "ObjNpcUnitFuncs.cs" },
        new() { StartLine = 682, EndLine = 856, Signature = "function GetLevelBaseObjectCondition(Npc: TNormNpc; PlayObject: TPlayObject; QuestConditionInfo: pTQuestConditionInfo): TBaseObject;", Status = "Covered", Home = "ObjNpcUnitFuncs.cs" },
        new() { StartLine = 857, EndLine = 1106, Signature = "function GetLevelBaseObjectAction(Npc: TNormNpc; PlayObject: TPlayObject; QuestActionInfo: pTQuestActionInfo): TBaseObject;", Status = "Covered", Home = "ObjNpcUnitFuncs.cs" },
        new() { StartLine = 1107, EndLine = 1117, Signature = "procedure TCastleOfficial.Click(PlayObject: TPlayObject);", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 1118, EndLine = 1185, Signature = "function TCastleOfficial.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean;", Status = "Missing", Home = "" },
        new() { StartLine = 1186, EndLine = 1334, Signature = "procedure TCastleOfficial.UserSelect(PlayObject: TPlayObject; sData: string);", Status = "Missing", Home = "" },
        new() { StartLine = 1335, EndLine = 1389, Signature = "procedure TCastleOfficial.HireGuard(sIndex: string; PlayObject: TPlayObject);", Status = "Missing", Home = "" },
        new() { StartLine = 1390, EndLine = 1445, Signature = "procedure TCastleOfficial.HireArcher(sIndex: string; PlayObject: TPlayObject);", Status = "Missing", Home = "" },
        new() { StartLine = 1446, EndLine = 1456, Signature = "procedure TMerchant.AddItemPrice(nIndex: Integer; nPrice: Integer);", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 1457, EndLine = 1487, Signature = "procedure TMerchant.CheckItemPrice(nIndex: Integer);", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 1488, EndLine = 1511, Signature = "function TMerchant.GetRefillList(nIndex: Integer): TList;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 1512, EndLine = 1629, Signature = "procedure TMerchant.RefillGoods;", Status = "Missing", Home = "" },
        new() { StartLine = 1630, EndLine = 1644, Signature = "function TMerchant.CheckItemType(nStdMode: Integer): Boolean;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 1645, EndLine = 1673, Signature = "function TMerchant.GetItemPrice(nIndex: Integer): Integer;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 1674, EndLine = 1683, Signature = "procedure TMerchant.SaveUpgradingList();", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 1684, EndLine = 1902, Signature = "procedure TMerchant.UpgradeWapon(User: TPlayObject); // 004A0920", Status = "Seam", Home = "嵌套过程 sub_4A0218(1686-1828) 已 1:1 in ObjNpcMerchant.cs；外层体 1830-1901 阻塞未做" },
        new() { StartLine = 1903, EndLine = 2051, Signature = "procedure TMerchant.GetBackupgWeapon(User: TPlayObject); // 004A0CB8", Status = "Missing", Home = "" },
        new() { StartLine = 2052, EndLine = 2086, Signature = "function TMerchant.GetUserPrice(PlayObject: TPlayObject; nPrice: Integer): Integer; // 0049F6E0", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 2087, EndLine = 2900, Signature = "procedure TMerchant.UserSelect(PlayObject: TPlayObject; sData: string);", Status = "Missing", Home = "" },
        new() { StartLine = 2901, EndLine = 3046, Signature = "procedure TMerchant.Run();", Status = "Missing", Home = "" },
        new() { StartLine = 3047, EndLine = 3051, Signature = "function TMerchant.Operate(ProcessMsg: pTProcessMessage): Boolean;", Status = "Missing", Home = "" },
        new() { StartLine = 3052, EndLine = 3061, Signature = "procedure TMerchant.LoadNPCData;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3062, EndLine = 3070, Signature = "procedure TMerchant.SaveNPCData;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3071, EndLine = 3123, Signature = "constructor TMerchant.Create; // 0049EC70", Status = "Missing", Home = "" },
        new() { StartLine = 3124, EndLine = 3159, Signature = "destructor TMerchant.Destroy; // 0049ED70", Status = "Missing", Home = "" },
        new() { StartLine = 3160, EndLine = 3179, Signature = "procedure TMerchant.ClearExpreUpgradeListData; // 004A01A0", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3180, EndLine = 3205, Signature = "procedure TMerchant.LoadNpcScript(IsAddMapName: Boolean = True);", Status = "Covered", Home = "ObjNpcPersistence.cs" },
        new() { StartLine = 3206, EndLine = 3227, Signature = "procedure TMerchant.LoadNpcIconFile(IsAddMapName: Boolean = True);", Status = "Covered", Home = "ObjNpcPersistence.cs" },
        new() { StartLine = 3228, EndLine = 3233, Signature = "procedure TMerchant.Click(PlayObject: TPlayObject); // 0049FF24", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 3234, EndLine = 3271, Signature = "function TMerchant.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean; // 0049FD04", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3272, EndLine = 3366, Signature = "function TMerchant.GetUserItemPrice(UserItem: pTUserItem; IsSellToNpc: Boolean): Integer;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3367, EndLine = 3689, Signature = "procedure TMerchant.ClientBuyItem(PlayObject: TPlayObject; sItemName: string; nCount, nInt: Integer; IsFromTradingDlg: Boolean);", Status = "Covered", Home = "ObjNpcMerchantBuy.cs" },
        new() { StartLine = 3690, EndLine = 3752, Signature = "procedure TMerchant.ClientGetDetailGoodsList(PlayObject: TPlayObject; sItemName: string; nInt: Integer; IsFromTradingDlg: Boolean);", Status = "Missing", Home = "" },
        new() { StartLine = 3753, EndLine = 3792, Signature = "procedure TMerchant.ClientQuerySellPrice(PlayObject: TPlayObject; UserItem: pTUserItem; IsFromTradingDlg: Boolean; WaitSetIndex: Integer);", Status = "Missing", Home = "" },
        new() { StartLine = 3793, EndLine = 3797, Signature = "function TMerchant.GetSellItemPrice(nPrice: Integer): Integer;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3798, EndLine = 3868, Signature = "function TMerchant.ClientSellItem(PlayObject: TPlayObject; UserItem: pTUserItem; IsFromTradingDlg: Boolean): Boolean;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3869, EndLine = 3893, Signature = "function TMerchant.AddItemToGoodsList(UserItem: pTUserItem): Boolean;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 3894, EndLine = 4029, Signature = "procedure TMerchant.ClientMakeDrugItem(PlayObject: TPlayObject; sItemName: string);", Status = "Missing", Home = "" },
        new() { StartLine = 4030, EndLine = 4064, Signature = "procedure TMerchant.ClientQueryRepairCost(PlayObject: TPlayObject; UserItem: pTUserItem);", Status = "Missing", Home = "" },
        new() { StartLine = 4065, EndLine = 4163, Signature = "function TMerchant.ClientRepairItem(PlayObject: TPlayObject; UserItem: pTUserItem): Boolean;", Status = "Missing", Home = "" },
        new() { StartLine = 4164, EndLine = 4195, Signature = "procedure TMerchant.ClearScript;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 4196, EndLine = 4234, Signature = "procedure TMerchant.LoadUpgradeList;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 4235, EndLine = 4240, Signature = "procedure TMerchant.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 4241, EndLine = 4281, Signature = "procedure TMerchant.ClearData;", Status = "Covered", Home = "ObjNpcMerchant.cs" },
        new() { StartLine = 4282, EndLine = 4334, Signature = "procedure TMerchant.ChangeUseItemName(PlayObject: TPlayObject; sLabel, sItemName: string);", Status = "Missing", Home = "" },
        new() { StartLine = 4335, EndLine = 4342, Signature = "constructor TTrainer.Create; // 004A385C", Status = "Missing", Home = "" },
        new() { StartLine = 4343, EndLine = 4347, Signature = "destructor TTrainer.Destroy;", Status = "Missing", Home = "" },
        new() { StartLine = 4348, EndLine = 4365, Signature = "function TTrainer.Operate(ProcessMsg: pTProcessMessage): Boolean; // 004A38C4", Status = "Missing", Home = "" },
        new() { StartLine = 4366, EndLine = 4382, Signature = "procedure TTrainer.Run;", Status = "Missing", Home = "" },
        new() { StartLine = 4383, EndLine = 4430, Signature = "procedure TNormNpc.ClearScript;", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 4431, EndLine = 4442, Signature = "procedure TNormNpc.Click(PlayObject: TPlayObject); // 0049EC18", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4443, EndLine = 4456, Signature = "procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var nValue: Integer);", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4457, EndLine = 4466, Signature = "procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string);", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4467, EndLine = 4481, Signature = "procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var sValue: string; var nValue: Integer; var IsBreakParseVar: Boolean);", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4482, EndLine = 4494, Signature = "procedure TNormNpc.GetVarValue(PlayObject: TPlayObject; sData: string; var sVar, sValue: string; var nValue: Integer);", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4495, EndLine = 4511, Signature = "function TNormNpc.SetVarValue(PlayObject: TPlayObject; const sData, sValue: string; const nValue: Integer): Boolean;", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4512, EndLine = 4575, Signature = "function TNormNpc.GetDynamicValue(PlayObject: TPlayObject; sVar: string; var sValue: string; var nValue: Integer): Boolean;", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4576, EndLine = 4644, Signature = "function TNormNpc.SetDynamicValue(PlayObject: TPlayObject; sVar: string; sValue: string; nValue: Integer): Boolean;", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 4645, EndLine = 4934, Signature = "function SetBoxItemValue(sVar: string; PlayObject: TPlayObject; sValue: string; nValue: Integer): Boolean;", Status = "Seam", Home = "NpcSeams.SetBoxItemValue" },
        new() { StartLine = 4935, EndLine = 5325, Signature = "function TNormNpc.SetValNameValue(PlayObject: TPlayObject; sVar: string; sValue: string; nValue: Integer): Boolean;", Status = "Seam", Home = "NpcSeams.SetValNameValue" },
        new() { StartLine = 5326, EndLine = 5689, Signature = "function GetBoxItemValue(sVar: string; PlayObject: TPlayObject; var Ret: string): Boolean;", Status = "Seam", Home = "NpcSeams.GetBoxItemValue" },
        new() { StartLine = 5690, EndLine = 5878, Signature = "function TNormNpc.GetValNameValue(PlayObject: TPlayObject; sVar: string; var sValue: string; var nValue: Integer): Boolean;", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 5879, EndLine = 5916, Signature = "constructor TNormNpc.Create; // 0049AA38", Status = "Missing", Home = "" },
        new() { StartLine = 5917, EndLine = 5933, Signature = "destructor TNormNpc.Destroy; // 0049AAE4", Status = "Missing", Home = "" },
        new() { StartLine = 5934, EndLine = 5952, Signature = "function TNormNpc.AllowSelect(sLabel: string): Boolean;", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 5953, EndLine = 5966, Signature = "procedure TNormNpc.AddSelectLable(sLabel: string);", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 5967, EndLine = 5980, Signature = "procedure TNormNpc.DeleteSelectLable(sLabel: string);", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 5981, EndLine = 6010, Signature = "function TNormNpc.GetLineVariableText(PlayObject: TPlayObject; sMsg: string; var IsBreakParseVar: Boolean): string;", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 6011, EndLine = 9262, Signature = "function TNormNpc.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean;", Status = "Seam", Home = "NpcSeams.GetVariableText + ObjNpcVars.cs 虚方法外壳" },
        new() { StartLine = 9263, EndLine = 9574, Signature = "function TNormNpc.GotoLable(Player: TPlayObject; sLabel: string; boExtJmp: Boolean; UseParams: Boolean): Boolean;", Status = "Missing", Home = "" },
        new() { StartLine = 9575, EndLine = 9592, Signature = "procedure TNormNpc.LoadNpcScript;", Status = "Covered", Home = "ObjNpcPersistence.cs" },
        new() { StartLine = 9593, EndLine = 9603, Signature = "procedure TNormNpc.LoadNpcIconFile;", Status = "Covered", Home = "ObjNpcPersistence.cs" },
        new() { StartLine = 9604, EndLine = 9608, Signature = "function TNormNpc.Operate(ProcessMsg: pTProcessMessage): Boolean;", Status = "Missing", Home = "" },
        new() { StartLine = 9609, EndLine = 9626, Signature = "function TNormNpc.GetShowName(boSuperUser: Boolean): string;", Status = "Missing", Home = "" },
        new() { StartLine = 9627, EndLine = 9744, Signature = "procedure TNormNpc.Run;", Status = "Missing", Home = "" },
        new() { StartLine = 9745, EndLine = 9766, Signature = "procedure TNormNpc.ScriptActionError(BaseObject: TBaseObject; sErrMsg: string; QuestActionInfo: pTQuestActionInfo);", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 9767, EndLine = 9788, Signature = "procedure TNormNpc.ScriptConditionError(BaseObject: TBaseObject; QuestConditionInfo: pTQuestConditionInfo);", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 9789, EndLine = 9799, Signature = "procedure TNormNpc.SendMsgToUser(PlayObject: TPlayObject; sMsg: string; boShowNPCName: Boolean); // 0049AD14", Status = "Covered", Home = "ObjNpcConversation.cs" },
        new() { StartLine = 9800, EndLine = 9806, Signature = "procedure TNormNpc.MessageBox(PlayObject: TPlayObject; sMsg: string);", Status = "Covered", Home = "ObjNpcConversation.cs" },
        new() { StartLine = 9807, EndLine = 9836, Signature = "procedure TNormNpc.UserSelect(PlayObject: TPlayObject; sData: string);", Status = "Missing", Home = "" },
        new() { StartLine = 9837, EndLine = 9863, Signature = "procedure TNormNpc.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);", Status = "Covered", Home = "ObjNpcConversation.cs" },
        new() { StartLine = 9864, EndLine = 9876, Signature = "procedure TNormNpc.Initialize;", Status = "Missing", Home = "" },
        new() { StartLine = 9877, EndLine = 9899, Signature = "function TNormNpc.GetDynamicVarList(PlayObject: TPlayObject; sType: string; var sName: string): TList;", Status = "Covered", Home = "ObjNpcVars.cs" },
        new() { StartLine = 9900, EndLine = 9925, Signature = "procedure TNormNpc.LoadAddData;", Status = "Missing", Home = "" },
        new() { StartLine = 9926, EndLine = 9954, Signature = "procedure TNormNpc.SaveAddData;", Status = "Missing", Home = "" },
        new() { StartLine = 9955, EndLine = 9995, Signature = "procedure TNormNpc.QuickSortRecordList(List: TList; L, R: Integer);", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 9996, EndLine = 10018, Signature = "procedure TNormNpc.DoSort;", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 10019, EndLine = 10048, Signature = "function TNormNpc.GetSayingRecordFromRecordList(List: TList; sLabel: string): pTSayingRecord;", Status = "Covered", Home = "ObjNpcLabels.cs" },
        new() { StartLine = 10049, EndLine = 10054, Signature = "procedure TGuildOfficial.Click(PlayObject: TPlayObject); // 004A30F4", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 10055, EndLine = 10090, Signature = "function TGuildOfficial.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string; var IsBreakParseVar: Boolean; nPos: Integer): Boolean;", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 10091, EndLine = 10100, Signature = "procedure TGuildOfficial.Run; // 004A37F0", Status = "Missing", Home = "" },
        new() { StartLine = 10101, EndLine = 10152, Signature = "procedure TGuildOfficial.UserSelect(PlayObject: TPlayObject; sData: string);", Status = "Missing", Home = "" },
        new() { StartLine = 10153, EndLine = 10230, Signature = "function TGuildOfficial.ReQuestBuildGuild(PlayObject: TPlayObject; sGuildName: string): Integer; // 004A3124", Status = "Missing", Home = "" },
        new() { StartLine = 10231, EndLine = 10251, Signature = "function TGuildOfficial.ReQuestGuildWar(PlayObject: TPlayObject; sGuildName: string): Integer; // 004A3368", Status = "Missing", Home = "" },
        new() { StartLine = 10252, EndLine = 10256, Signature = "procedure TGuildOfficial.DoNate(PlayObject: TPlayObject); // 004A346C", Status = "Missing", Home = "" },
        new() { StartLine = 10257, EndLine = 10291, Signature = "procedure TGuildOfficial.ReQuestCastleWar(PlayObject: TPlayObject; sIndex: string); // 004A3498", Status = "Missing", Home = "" },
        new() { StartLine = 10292, EndLine = 10327, Signature = "procedure TCastleOfficial.RepairDoor(PlayObject: TPlayObject); // 004A3FB8", Status = "Missing", Home = "" },
        new() { StartLine = 10328, EndLine = 10363, Signature = "procedure TCastleOfficial.RepairWallNow(nWallIndex: Integer; PlayObject: TPlayObject); // 004A4074", Status = "Missing", Home = "" },
        new() { StartLine = 10364, EndLine = 10368, Signature = "constructor TCastleOfficial.Create;", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 10369, EndLine = 10373, Signature = "destructor TCastleOfficial.Destroy;", Status = "Missing", Home = "" },
        new() { StartLine = 10374, EndLine = 10380, Signature = "constructor TGuildOfficial.Create;", Status = "Covered", Home = "ObjNpcConversation.cs" },
        new() { StartLine = 10381, EndLine = 10385, Signature = "destructor TGuildOfficial.Destroy;", Status = "Missing", Home = "" },
        new() { StartLine = 10386, EndLine = 10390, Signature = "procedure TGuildOfficial.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 10391, EndLine = 10405, Signature = "procedure TCastleOfficial.SendCustemMsg(PlayObject: TPlayObject; sMsg: string);", Status = "Covered", Home = "ObjNpcGuildCastle.cs" },
        new() { StartLine = 10406, EndLine = 10509, Signature = "function CheckStrIsVar(sText: string): Boolean;", Status = "Covered", Home = "ObjNpcUnitFuncs.cs" },
        new() { StartLine = 10510, EndLine = 10515, Signature = "constructor TBoxMonster.Create;", Status = "Covered", Home = "ObjNpcBoxMonster.cs" },
        new() { StartLine = 10516, EndLine = 10520, Signature = "destructor TBoxMonster.Destroy;", Status = "Missing", Home = "" },
        new() { StartLine = 10521, EndLine = 10526, Signature = "procedure TBoxMonster.Initialize;", Status = "Covered", Home = "ObjNpcConversation.cs" },
        new() { StartLine = 10527, EndLine = 10533, Signature = "function TBoxMonster.Operate(ProcessMsg: pTProcessMessage): Boolean;", Status = "Covered", Home = "ObjNpcBoxMonster.cs" },
        new() { StartLine = 10534, EndLine = 10544, Signature = "procedure TBoxMonster.Run;", Status = "Covered", Home = "ObjNpcBoxMonster.cs" },
    };
}
