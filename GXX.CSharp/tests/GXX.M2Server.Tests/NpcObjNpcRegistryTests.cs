// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：ObjNpcRoutineRegistry（脚本从原文抽取的 112 条顶层例程登记表）
//   —— 这些是**审计守卫**：锁死条目数、行号区间连续性、状态取值域，
//      防止后续"顺手改一行"造成覆盖口径漂移。
// ============================================================================

using System.Collections.Generic;
using System.Linq;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcRegistryTests
{
    [Fact]
    public void Registry_Has112TopLevelRoutines()
    {
        Assert.Equal(112, ObjNpcRoutineRegistry.All.Length);
    }

    [Fact]
    public void Registry_LineRangesAreContiguous()
    {
        // 每条的上界 + 1 == 下一条的下界；末条止于 10544（原文 10545 是 `end.`）。
        var all = ObjNpcRoutineRegistry.All;
        Assert.Equal(504, all[0].StartLine);
        Assert.Equal(10544, all[^1].EndLine);
        for (int i = 1; i < all.Length; i++)
            Assert.Equal(all[i - 1].EndLine + 1, all[i].StartLine);
    }

    [Fact]
    public void Registry_StatusesAreFromFixedVocabulary()
    {
        var allowed = new HashSet<string> { "Covered", "Seam", "Missing" };
        foreach (var e in ObjNpcRoutineRegistry.All)
            Assert.Contains(e.Status, allowed);
    }

    [Fact]
    public void Registry_NonMissingEntriesHaveHome()
    {
        foreach (var e in ObjNpcRoutineRegistry.All.Where(x => x.Status != "Missing"))
            Assert.False(string.IsNullOrEmpty(e.Home), $"{e.StartLine} 缺少归属");
    }

    [Fact]
    public void Registry_CoverageCountsMatchReportedNumbers()
    {
        // 落地口径（切片 17 之后）：Covered 63 / Seam 6 / Missing 43 = 112。
        Assert.Equal(63, ObjNpcRoutineRegistry.All.Count(e => e.Status == "Covered"));
        Assert.Equal(6, ObjNpcRoutineRegistry.All.Count(e => e.Status == "Seam"));
        Assert.Equal(43, ObjNpcRoutineRegistry.All.Count(e => e.Status == "Missing"));
    }

    [Fact]
    public void Registry_SignaturesAreNonEmptyAndStartWithRoutineKeyword()
    {
        foreach (var e in ObjNpcRoutineRegistry.All)
        {
            Assert.False(string.IsNullOrEmpty(e.Signature));
            Assert.True(
                e.Signature.StartsWith("function ") || e.Signature.StartsWith("procedure ")
                || e.Signature.StartsWith("constructor ") || e.Signature.StartsWith("destructor "),
                $"{e.StartLine} 签名异常：{e.Signature}");
        }
    }

    [Theory]
    [InlineData(509, "LoadLevelScriptAction", "Covered")]
    [InlineData(596, "LoadLevelScriptCondition", "Covered")]
    [InlineData(682, "GetLevelBaseObjectCondition", "Covered")]
    [InlineData(857, "GetLevelBaseObjectAction", "Covered")]
    [InlineData(10406, "CheckStrIsVar", "Covered")]
    [InlineData(3272, "GetUserItemPrice", "Covered")]
    [InlineData(6011, "GetVariableText", "Seam")]
    [InlineData(3052, "LoadNPCData", "Covered")]
    [InlineData(3062, "SaveNPCData", "Covered")]
    [InlineData(3180, "LoadNpcScript", "Covered")]
    [InlineData(3234, "GetVariableText", "Covered")]
    [InlineData(3869, "AddItemToGoodsList", "Covered")]
    [InlineData(3798, "ClientSellItem", "Covered")]
    [InlineData(4164, "ClearScript", "Covered")]
    [InlineData(4196, "LoadUpgradeList", "Covered")]
    [InlineData(4241, "ClearData", "Covered")]
    [InlineData(1674, "SaveUpgradingList", "Covered")]
    [InlineData(9575, "LoadNpcScript", "Covered")]
    [InlineData(9593, "LoadNpcIconFile", "Covered")]
    [InlineData(10510, "TBoxMonster.Create", "Covered")]
    [InlineData(10527, "TBoxMonster.Operate", "Covered")]
    [InlineData(10534, "TBoxMonster.Run", "Covered")]
    [InlineData(4431, "Click", "Seam")]
    [InlineData(1107, "TCastleOfficial.Click", "Covered")]
    [InlineData(3228, "TMerchant.Click", "Covered")]
    [InlineData(10049, "TGuildOfficial.Click", "Covered")]
    [InlineData(10055, "TGuildOfficial.GetVariableText", "Covered")]
    [InlineData(10364, "TCastleOfficial.Create", "Covered")]
    [InlineData(10386, "TGuildOfficial.SendCustemMsg", "Covered")]
    [InlineData(10391, "TCastleOfficial.SendCustemMsg", "Covered")]
    [InlineData(10374, "TGuildOfficial.Create", "Covered")]
    [InlineData(9789, "SendMsgToUser", "Covered")]
    [InlineData(9800, "MessageBox", "Covered")]
    [InlineData(9837, "SendCustemMsg", "Covered")]
    [InlineData(10521, "TBoxMonster.Initialize", "Covered")]
    [InlineData(10091, "TGuildOfficial.Run", "Missing")]
    [InlineData(5326, "GetBoxItemValue", "Seam")]
    [InlineData(4645, "SetBoxItemValue", "Seam")]
    [InlineData(4935, "SetValNameValue", "Seam")]
    [InlineData(9263, "GotoLable", "Missing")]
    [InlineData(2087, "UserSelect", "Missing")]
    [InlineData(3367, "ClientBuyItem", "Missing")]
    [InlineData(1684, "UpgradeWapon", "Seam")]
    [InlineData(10516, "TBoxMonster.Destroy", "Missing")]
    public void Registry_KnownEntries(int startLine, string nameFragment, string status)
    {
        var e = ObjNpcRoutineRegistry.All.Single(x => x.StartLine == startLine);
        Assert.Contains(nameFragment, e.Signature);
        Assert.Equal(status, e.Status);
    }
}
