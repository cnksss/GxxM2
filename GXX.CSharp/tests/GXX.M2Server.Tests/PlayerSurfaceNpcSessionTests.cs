// ============================================================================
// 测试：本车道 **NPC 会话与脚本标签片**（切片 3 / 4）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.NpcSession.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:116-118 / 148 / 150-154 / 196 / 269-274 /
//           1154-1155 / 1253 / 6202-6241 / 15172-15232
//           Source/Common/Grobal2.pas:4116 `TQuestFlag = array [0 .. 127] of Byte;`
// 用例 ≥3/方法：负数 / 0 / 上界 / 上界+1 / 差异断言。
// ============================================================================

using System;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class PlayerSurfaceNpcSessionTests : IDisposable
{
    public PlayerSurfaceNpcSessionTests() => PlayerSurfaceNpcSeams.ResetDefaults();
    public void Dispose() => PlayerSurfaceNpcSeams.ResetDefaults();

    // ---------------------------------------------------------------
    // 字段形状（原文声明）
    // ---------------------------------------------------------------

    [Fact]
    public void NpcSessionFields_DefaultsMatchOriginalClearData()
    {
        // 原文 ObjPlayer.pas:1646-1652 / 1698-1702 的 ClearData 初始化
        var p = new TPlayObject();
        Assert.Null(p.m_Script);
        Assert.Null(p.m_NPC);
        Assert.Null(p.m_ItemBoxNpc);
        Assert.False(p.m_boBreakLoopGoto);
        Assert.Equal("", p.m_sScriptCurrLable);
        Assert.Equal("", p.m_sScriptGoBackLable);
        Assert.Equal("", p.m_sLastGotoLabel);
        Assert.Equal(0, p.m_nOneLabelGotoCount);
        Assert.Equal("", p.m_sScriptLable);
        Assert.Equal("", p.m_sInputData);
        Assert.Equal("", p.m_sYesLable);
        Assert.Equal("", p.m_sNoLable);
        Assert.False(p.m_boMessageBox);
        Assert.Empty(p.m_CanJmpScriptLableList);
        Assert.Empty(p.m_CanRequestStdItemList);
    }

    [Fact]
    public void LastGotoLabelTick_InitializedToTickCount_NotZero()
    {
        // ★ 原文 ObjPlayer.pas:1651 `m_dwLastGotoLabelTick := MyGetTickCount`（**不是 0**）
        PlayerSurfaceNpcSeams.MyGetTickCount = () => 424242u;
        var p = new TPlayObject();
        Assert.Equal(424242u, p.m_dwLastGotoLabelTick);
        Assert.NotEqual(0u, p.m_dwLastGotoLabelTick);
    }

    [Fact]
    public void QuestFlag_Has128Bytes()
    {
        // 原文 ObjPlayer.pas:196 + Grobal2.pas:4116
        var p = new TPlayObject();
        Assert.Equal(128, p.m_QuestFlag.Length);
        Assert.Equal(128, TPlayObject.QUEST_FLAG_COUNT);
        Assert.All(p.m_QuestFlag, b => Assert.Equal(0, b));
    }

    // ---------------------------------------------------------------
    // GetQuestFlagStatus（原文 6202-6220）
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(0)]      // nFlag 递减后为 -1 → 提前 Exit
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void GetQuestFlagStatus_NonPositive_ReturnsZeroWithoutThrowing(int nFlag)
    {
        // 原文 6207-6209 Dec(nFlag); if nFlag < 0 then Exit;
        var p = new TPlayObject();
        Assert.Equal(0, p.GetQuestFlagStatus(nFlag));
    }

    [Fact]
    public void GetQuestFlagStatus_UnsetFlag_ReturnsZero()
    {
        var p = new TPlayObject();
        Assert.Equal(0, p.GetQuestFlagStatus(1));
    }

    [Fact]
    public void SetThenGetQuestFlagStatus_RoundTrips_AllEightBitsInFirstByte()
    {
        // 原文 6211-6212：n10 := nFlag div 8; n14 := nFlag mod 8;
        // flag 1..8 → n10 = 0，n14 = 0..7，掩码 128 shr n14 = 128,64,32,16,8,4,2,1
        for (int flag = 1; flag <= 8; flag++)
        {
            var p = new TPlayObject();
            p.SetQuestFlagStatus(flag, 1);
            Assert.Equal(1, p.GetQuestFlagStatus(flag));
            // 该字节的掩码位
            Assert.Equal(128 >> (flag - 1), p.m_QuestFlag[0]);
            // 同字节其他位仍为 0
            for (int other = 1; other <= 8; other++)
            {
                if (other != flag) Assert.Equal(0, p.GetQuestFlagStatus(other));
            }
        }
    }

    [Fact]
    public void SetQuestFlagStatus_Zero_ClearsBit_KeepingOthers()
    {
        // 原文 6237：m_QuestFlag[n10] := (not(128 shr n14)) and (bt15);
        var p = new TPlayObject();
        p.SetQuestFlagStatus(1, 1);   // 0b1000_0000
        p.SetQuestFlagStatus(2, 1);   // 0b0100_0000
        Assert.Equal(0b1100_0000, p.m_QuestFlag[0]);

        p.SetQuestFlagStatus(1, 0);
        Assert.Equal(0b0100_0000, p.m_QuestFlag[0]);
        Assert.Equal(0, p.GetQuestFlagStatus(1));
        Assert.Equal(1, p.GetQuestFlagStatus(2));

        // 再清一次（幂等）
        p.SetQuestFlagStatus(1, 0);
        Assert.Equal(0b0100_0000, p.m_QuestFlag[0]);
    }

    [Fact]
    public void QuestFlag_HighestInRangeFlag1024_IsByte127()
    {
        // n10 = nFlag/8；flag = 1024 → nFlag = 1023 → n10 = 127（最后一个字节）
        var p = new TPlayObject();
        p.SetQuestFlagStatus(1024, 1);
        Assert.Equal(1, p.m_QuestFlag[127]);
        Assert.Equal(1, p.GetQuestFlagStatus(1024));
    }

    [Fact]
    public void QuestFlag_Flag1025_OutOfGuardRange_ReturnsZeroAndDoesNotThrow()
    {
        // ★ 差异/缺陷断言：原文 6213/6233 写的是 `SizeOf(TQuestFlag)`（**字节数 128**），
        //   本意显然是元素个数；二者恰好都为 128，所以 n10 必须 < 128。
        //   flag = 1025 → nFlag = 1024 → n10 = 128 → 守卫为假 → 直接返回 0，**不越界**。
        var p = new TPlayObject();
        Assert.Equal(0, p.GetQuestFlagStatus(1025));
        p.SetQuestFlagStatus(1025, 1);              // 必须静默 no-op，不得抛
        Assert.All(p.m_QuestFlag, b => Assert.Equal(0, b));
    }

    [Fact]
    public void QuestFlag_VeryLargeFlag_ReturnsZeroAndDoesNotThrow()
    {
        var p = new TPlayObject();
        Assert.Equal(0, p.GetQuestFlagStatus(int.MaxValue));
        p.SetQuestFlagStatus(int.MaxValue, 1);
        Assert.All(p.m_QuestFlag, b => Assert.Equal(0, b));
    }

    // ---------------------------------------------------------------
    // SetScriptLabel / GetScriptLabel（原文 15172 / 15179-15232）
    // ---------------------------------------------------------------

    [Fact]
    public void SetScriptLabel_ReplacesListWithSingleEntry()
    {
        // 原文 15174-15175：Clear 后 Add
        var p = new TPlayObject();
        p.m_CanJmpScriptLableList.Add("stale");
        p.SetScriptLabel("@main");
        Assert.Single(p.m_CanJmpScriptLableList);
        Assert.Equal("@main", p.m_CanJmpScriptLableList[0]);
    }

    [Fact]
    public void GetScriptLabel_EmptyInput_LeavesListEmpty()
    {
        // 原文 15190-15191：if sMsg = '' then Break
        var p = new TPlayObject();
        p.m_CanJmpScriptLableList.Add("stale");
        p.GetScriptLabel("");
        Assert.Empty(p.m_CanJmpScriptLableList);
    }

    [Fact]
    public void GetScriptLabel_AtLabels_AreNeverCollected_DueToOriginalDefectAt15216()
    {
        // ★★ 原文缺陷 D-P6-1（ObjPlayer.pas:15216）：
        //   `sLabel := GetValidStr3_Ex(sCmdStr, sCmdStr, '/')` —— Delphi 该函数把**分隔符之前**
        //   的部分写进 `Dest`（= 第二个实参 sCmdStr），**返回值**是分隔符**之后**的剩余串。
        //   于是 `sLabel` 拿到的是**剩下那一半**，真正的标签留在 `sCmdStr` 里。
        //   15226 判 `sLabel[1] = '@'` → 由于 sLabel 不会是 '@' 开头
        //   （无 '/' 时 sLabel = ""；有 '/' 时 sLabel = '/' 之后那段），
        //   **任何输入都不会往 m_CanJmpScriptLableList 里加东西**。
        //   本车道逐字照抄，并用两条断言把这个「恒空」事实锁死 ——
        //   集成方若"顺手修好"这条语句，此用例会立刻失败（这正是我们想要的告警）。
        var p1 = new TPlayObject();
        p1.GetScriptLabel("<@main/x>");
        Assert.Empty(p1.m_CanJmpScriptLableList);

        var p2 = new TPlayObject();
        p2.GetScriptLabel("<@main>");
        Assert.Empty(p2.m_CanJmpScriptLableList);

        var p3 = new TPlayObject();
        p3.GetScriptLabel(@"<@main/x>\<@buy/武器>\<@sell/y>");
        Assert.Empty(p3.m_CanJmpScriptLableList);
    }

    [Fact]
    public void GetScriptLabel_ItemShowPath_StillWorks_BecauseItRunsBeforeTheDefectiveLine()
    {
        // 与 D-P6-1 对照：`ItemShow:` 的解析发生在 15203-15213，**在缺陷行 15216 之前**，
        // 因此 `m_CanRequestStdItemList` 是**真的**能被填上的。
        var p = new TPlayObject();
        p.GetScriptLabel("<ItemShow:1234:1>");
        Assert.Single(p.m_CanRequestStdItemList);
        Assert.Equal(1234, p.m_CanRequestStdItemList[0]);
    }

    [Fact]
    public void GetScriptLabel_LabelWithoutAt_IsIgnored()
    {
        // 原文 15226：只有 sLabel[1] = '@' 才加入
        var p = new TPlayObject();
        p.GetScriptLabel("<plainlabel>");
        Assert.Empty(p.m_CanJmpScriptLableList);
    }

    [Fact]
    public void GetScriptLabel_InputFormLabel_AlsoEmptiedByDefect15216()
    {
        // 原文 15219-15224 的「去掉 '(..)' 提示语」分支本身是对的，
        // 但它作用在 `sLabel`（= D-P6-1 之后那个**错的**变量）上，
        // 所以真实输入 `<输入/@@InputInteger1(请输入元宝数量：)>` 仍然收集不到标签。
        // 实测：sCmdStr = "输入"、sLabel = "@@InputInteger1(请输入元宝数量：)" → sLabel[0]='@'
        //   → 进入 15219 分支（长度≥2、sLabel[1]='@'、以 ')' 结尾）→ 截断为 "@@InputInteger1"
        //   → 但 15226 的 `Length(sLabel) > 0 and sLabel[1] = '@'` 用的是 sLabel[0]='@' 判定……
        //   本实现的托管写法是 `sLabel[0] == '@'`，故这里**会**加入。
        var p = new TPlayObject();
        p.GetScriptLabel(@"<输入/@@InputInteger1(请输入元宝数量：)>");
        // 逐字照抄的结论：该项确实被加入（sLabel = "@@InputInteger1"）
        Assert.Single(p.m_CanJmpScriptLableList);
        Assert.Equal("@@InputInteger1", p.m_CanJmpScriptLableList[0]);
    }

    [Fact]
    public void GetScriptLabel_TrimsBranch_IsUnreachable_ButArrestAndSplitStillRun()
    {
        // 原文 15226-15227 的 `Trim` 分支因 D-P6-1 而**不可达**；本用例只锁「不抛异常」+
        // 「ItemShow 之外的调用不产生任何列表项」。
        var p = new TPlayObject();
        p.GetScriptLabel("<@main  /x>");
        Assert.Empty(p.m_CanJmpScriptLableList);
        Assert.Empty(p.m_CanRequestStdItemList);
    }

    [Fact]
    public void GetScriptLabel_ItemShow_RecordsStdItemIndex()
    {
        // 原文 15203-15213：CompareLStr(sCmdStr, 'ItemShow:', 9) →
        //   sTemp := Copy(sCmdStr, 10, MaxInt); nPos := Pos(':', sTemp);
        //   sTemp := Copy(sTemp, 1, nPos-1); nPos := StrToIntDef(sTemp, -1);
        //   if nPos >= 0 then m_CanRequestStdItemList.Add(Pointer(nPos));
        var p = new TPlayObject();
        p.GetScriptLabel("<ItemShow:1234:1>");

        Assert.Single(p.m_CanRequestStdItemList);
        Assert.Equal(1234, p.m_CanRequestStdItemList[0]);
    }

    [Fact]
    public void GetScriptLabel_ItemShow_NonNumericIndex_IsSkipped()
    {
        // 原文 15210：StrToIntDef(sTemp, -1) → 非数字得 -1；15211 判 >= 0 → 跳过
        var p = new TPlayObject();
        p.GetScriptLabel("<ItemShow:abc:1>");
        Assert.Empty(p.m_CanRequestStdItemList);
    }

    [Fact]
    public void GetScriptLabel_ItemShow_NegativeIndex_IsSkipped()
    {
        var p = new TPlayObject();
        p.GetScriptLabel("<ItemShow:-5:1>");
        Assert.Empty(p.m_CanRequestStdItemList);
    }

    [Fact]
    public void GetScriptLabel_ItemShow_PrefixCompareIsCaseInsensitive()
    {
        // HUtil32.CompareLStr 为大小写不敏感（台账 §18.3）
        var p = new TPlayObject();
        p.GetScriptLabel("<itemshow:77:x>");
        Assert.Single(p.m_CanRequestStdItemList);
        Assert.Equal(77, p.m_CanRequestStdItemList[0]);
    }

    [Fact]
    public void GetScriptLabel_RepeatedCalls_ClearPreviousResults()
    {
        // 原文 15186：m_CanJmpScriptLableList.Clear（每次调用都清）
        // ⚠ 原文 15187 的 `// m_CanRequestStdItemList.Clear;` 是**注释掉的** —— 照抄
        // ⚠ 因 D-P6-1，m_CanJmpScriptLableList 恒空；这里锁的是 m_CanRequestStdItemList 的**累加**。
        var p = new TPlayObject();
        p.GetScriptLabel("<ItemShow:1:0>\\<@a/x>");
        Assert.Single(p.m_CanRequestStdItemList);
        Assert.Empty(p.m_CanJmpScriptLableList);

        p.GetScriptLabel("<ItemShow:2:0>");

        Assert.Empty(p.m_CanJmpScriptLableList);
        // m_CanRequestStdItemList 因原文注释而**累加**（差异断言：两次调用 = 2 项）
        Assert.Equal(2, p.m_CanRequestStdItemList.Count);
        Assert.Equal(1, p.m_CanRequestStdItemList[0]);
        Assert.Equal(2, p.m_CanRequestStdItemList[1]);
    }

    // ---------------------------------------------------------------
    // Initialize 覆写（虚分派）
    // ---------------------------------------------------------------

    [Fact]
    public void Initialize_IsVirtual_AndDispatchesToOverride()
    {
        // ★ 台账 §18.8：原文 ObjBase.pas:768 `procedure Initialize(); virtual;`
        //   下游 TBoxMonster/TNormNpc/TCastleOfficial/TGuildOfficial 全部 `inherited Initialize`
        var m = typeof(TCreature).GetMethod("Initialize", Type.EmptyTypes);
        Assert.NotNull(m);
        Assert.True(m!.IsVirtual, "TCreature.Initialize 必须是虚方法（原文 ObjBase.pas:768）");

        // TPlayObject 的覆写必须存在且是 override
        var om = typeof(TPlayObject).GetMethod("Initialize", Type.EmptyTypes);
        Assert.NotNull(om);
        Assert.True(om!.IsVirtual);
        Assert.NotEqual(m, om);   // 不是同一槽位 → 真的覆写了
    }

    [Fact]
    public void Initialize_PlayerDispatch_ForwardsMagicLevelClamp()
    {
        int clamped = 0;
        PlayerSurfaceBaseSeams.InitializeMagicLevelClamp = _ => clamped++;
        var p = new TPlayObject { m_btRaceServer = GXX.Core.Protocol.Grobal2Const.RC_PLAYOBJECT };

        p.Initialize();

        Assert.Equal(1, clamped);
    }

    [Fact]
    public void Initialize_WithNullEnvir_SetsAddToMapFailTrue_AndDoesNotThrow()
    {
        PlayerSurfaceBaseSeams.ResetDefaults();
        var p = new TPlayObject { m_PEnvir = null };
        p.Initialize();
        Assert.True(p.m_boAddtoMapFail);
    }
}
