// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// **复核守卫（verification guard）** —— 目的不是新增功能覆盖，而是把
// "已覆盖 57 条例程" 依赖的**底层语义**逐条钉死，使任何未来改动（含 GXX.Core 的
// DelphiRTL/HUtil32 修正）都会立刻在这组用例上暴露。
//
// 本文件针对复核中发现的 **两类真实语义偏差** 建守卫：
//   V1 `DelphiRTL.Trim` 比 Delphi 的 `Trim` **窄**（只去 #9/#10/#11/#12/#13/#32，
//      原文 `Trim` 去所有 `<= ' '`（#0..#32））—— 影响 LoadLevelScript{Action,Condition} 的分段判定。
//   V2 `DelphiRTL.StrToInt64Def` 会 **Trim**，而 Delphi 的 `StrToInt64Def` 走 `Val`：
//      `Val` 只跳**前导**空白，**尾随**空白导致解析失败 → 返回 Default。
//      影响 GetValNameValue / GetVarValue 系列。
// 另有 V3：`TGroupItems.ExtractStrings` 的"跳空串"语义（原文 `ExtractStrings(['.'], [], ...)`），
//   本文件用 ObjNpc 的公开入口把它钉死，避免该 helper 被改动后静默改变脚本分段。
//
// ⚠ 本文件**只断言当前托管行为**，并在注释里标注"与原文不同"。Core 侧修好后，
//   这些用例会失败 —— 那正是它们的作用：提醒本车道的调用点需要重新回读原文。
// ============================================================================

using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcVerificationGuardTests : System.IDisposable
{
    public NpcObjNpcVerificationGuardTests() => NpcSeams.ResetDefaults();

    public void Dispose() => NpcSeams.ResetDefaults();

    // =======================================================================
    // V1：Trim 宽度差异（原文 `Trim` 去所有 <= ' ' 的字符）
    // =======================================================================

    [Fact]
    public void V1_DelphiRtlTrim_StripsOnlyTheSixCommonWhitespaceChars()
    {
        // 原文 SysUtils.Trim 会去掉的字符集（#0..#32）里，DelphiRTL 只覆盖 6 个。
        Assert.Equal("A", DelphiRTL.Trim(" \t\r\n\f\vA \t\r\n\f\v"));
        // 这些控制字符 DelphiRTL **不去**（原文会去）—— 差异锁定点
        Assert.Equal("\u0001A", DelphiRTL.Trim("\u0001A"));
        Assert.Equal("\u0014A", DelphiRTL.Trim("\u0014A"));
        Assert.Equal("A\u001f", DelphiRTL.Trim("A\u001f"));
    }

    [Fact]
    public void V1_LoadLevelScriptAction_SegmentWithControlCharDoesNotMatchPrefix()
    {
        // 可达后果：原文 `Upper(Trim('HERO' + #20))` = 'HERO' → CMD_RACE_1；
        //          托管侧 DelphiRTL.Trim 不去 #20 → 'HERO\u0014' ≠ 'HERO' → 落 else = CMD_RACE_5。
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, "HERO\u0014.CHECKITEM");
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_5 }, info.ScriptCmd);
        Assert.Equal(new[] { "SELF", "HERO\u0014" }, info.ScriptList);
    }

    [Fact]
    public void V1_LoadLevelScriptAction_OrdinaryWhitespaceStillBehavesLikeOriginal()
    {
        // 对照：普通空格/Tab 两侧都被去掉 —— 原文一致。
        var a = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(a, " HERO . CHECKITEM ");
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_1 }, a.ScriptCmd);

        var b = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(b, "\tHERO\t.\tCHECKITEM\t");
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_1 }, b.ScriptCmd);
    }

    // =======================================================================
    // V2：StrToInt64Def 的 Trim 差异（原文 `Val` 只跳前导空白）
    // =======================================================================

    [Fact]
    public void V2_StrToInt64Def_LeadingSpaceParses()
    {
        // 两个语义在此**一致**：`Val(' 123')` 跳前导空格 → 123。
        Assert.Equal(123L, DelphiRTL.StrToInt64Def(" 123", -1));
    }

    [Fact]
    public void V2_StrToInt64Def_TrailingSpaceIsCurrentlyAccepted()
    {
        // ⚠ 差异锁定点：原文 `Val('123 ')` 因尾随空白而解析失败 → 返回 **Default**；
        //   托管侧 `s?.Trim()` 先去掉尾随空白 → 返回 **123**。
        // Core 修好后本断言会失败，届时应改为 Assert.Equal(-1L, ...)。
        Assert.Equal(123L, DelphiRTL.StrToInt64Def("123 ", -1));
    }

    [Fact]
    public void V2_GetValNameValue_TrailingSpaceInStringVariableIsParsedNumeric()
    {
        // 可达后果：S$ 变量值为 "77 " 时，原文 GetValNameValue 里
        // `nValue := StrToInt64Def(sValue, nValue)` 会保留 nValue（Default），
        // 而托管侧解析出 77。已锁定托管行为。
        var npc = new TNormNpc();
        var player = new GXX.M2Server.Engine.TPlayObject();
        player.m_StringList.Add("S$V", "77 ");
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "S$V", ref sValue, ref nValue));
        Assert.Equal("77 ", sValue);   // sValue 原样保留尾随空白
        Assert.Equal(77, nValue);      // ⚠ 原文此处应为 0（Default）
    }

    // =======================================================================
    // V3：ExtractStrings(['.'], [], ...) 的"跳空串"语义（经 ObjNpc 公开入口钉死）
    // =======================================================================

    [Theory]
    [InlineData("..A.B", new[] { "SELF", "A" })]           // 前导连续点 → 空段被跳过；末段 "B" 是**命令名**被摘掉
    [InlineData("A..B.C", new[] { "SELF", "A", "B" })]     // 中间连续点 → 空段被跳过；末段 "C" 被摘掉
    [InlineData(".A.B.", new[] { "SELF", "A" })]           // 尾点被 Action 的尾点保护挡住 → 不分段
    [InlineData("A.B..", new[] { "SELF", "A" })]           // 同上
    public void V3_ExtractStrings_SkipsEmptySegments(string cmd, string[] expectedList)
    {
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, cmd);
        if (cmd.EndsWith("."))
        {
            // 尾点保护（原文 518）→ 完全不分段
            Assert.Empty(info.ScriptList);
        }
        else
        {
            Assert.Equal(expectedList, info.ScriptList);
        }
    }

    [Fact]
    public void V3_ExtractStrings_LeadingDotsAreSkippedAndSelfIsInserted()
    {
        var info = new TQuestActionInfo();
        // "..A.B" → ExtractStrings 得 ['A','B']；末段 'B' 被当命令名摘掉；余 ['A']；首段 'A' ≠ SELF → 插 SELF
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, "..A.B");
        Assert.Equal("B", r);
        Assert.Equal(new[] { "SELF", "A" }, info.ScriptList);
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_5 }, info.ScriptCmd);
    }

    [Fact]
    public void V3_ExtractStrings_DoesNotTrimSegments()
    {
        // 原文 ExtractStrings 不做 Trim（Trim 是调用点自己做的）→ 空段判定只看长度。
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, "   .B");
        // 首段是三个空格：非空段 → 保留；Trim 后为 '' ≠ 'SELF' → 插入 SELF；ScriptList[1] = ''
        Assert.Equal(new[] { "SELF", "" }, info.ScriptList);
    }

    // =======================================================================
    // V4：整数除法 / 取模 / 无符号截断（三个重灾区）
    // =======================================================================

    [Fact]
    public void V4_GetUserPrice_CastleMemberBranchUsesIntegerDivisionInParens()
    {
        // 原文 2073：`Max(60, Round(m_nPriceRate * (g_Config.nCastleMemberPriceRate / 100)))`
        // 括号内是**整数除法** → 80/100 = 0 → n14 = Max(60, 0) = 60（与 rate 无关）
        var m = new TMerchant { m_boCastle = true, m_nPriceRate = 100 };
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.IsMasterGuild = (_, _) => true;
        Assert.Equal(60, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));

        // ⚠ 关键：`n14` 恒为 60 **与 m_nPriceRate 无关** —— 这就是 D9 缺陷的可观测后果。
        m.m_nPriceRate = 1;
        Assert.Equal(60, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));
        m.m_nPriceRate = 999;
        Assert.Equal(60, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));
    }

    [Fact]
    public void V4_Sub4A0218_ByteOutParamsTruncateLikeDelphiRangeCheckOff()
    {
        // 原文 `btDc: Byte` 接收 Integer 表达式（范围检查关闭）→ 按低 8 位截断。
        var m = new TMerchant();
        NpcSeams.sBlackStone = "黑铁矿";
        NpcSeams.GetStdItemName = _ => "NOTBLACK";
        // nDc = DC2(0) + DC1(2000) = 2000 → btDc = 2000 div 5 + 0 div 3 = 400 → (byte)400 = 144
        NpcSeams.GetStdItem = _ => new GXX.Core.Protocol.TStdItem { StdMode = 19, DC1 = 2000 };
        var list = new List<object> { new GXX.Core.Protocol.TUserItem { wIndex = 1 } };
        m.sub_4A0218(new GXX.M2Server.Engine.TPlayObject(), list, out byte dc, out _, out _, out _);
        Assert.Equal(unchecked((byte)400), dc);   // 400 & 0xFF == 144
        Assert.Equal(144, dc);
    }

    [Fact]
    public void V4_GetSellItemPrice_BankerRoundingBoundaries()
    {
        var m = new TMerchant();
        // Round(0.5)=0 / Round(1.5)=2 / Round(2.5)=2 / Round(-0.5)=0 / Round(-1.5)=-2
        Assert.Equal(0, m.GetSellItemPrice(1));
        Assert.Equal(2, m.GetSellItemPrice(3));
        Assert.Equal(2, m.GetSellItemPrice(5));
        Assert.Equal(0, m.GetSellItemPrice(-1));
        Assert.Equal(-2, m.GetSellItemPrice(-3));
    }

    // =======================================================================
    // V5：虚分派链（三条：GetVariableText / SendCustemMsg / Click）
    // =======================================================================

    private sealed class ProbeNpc : TNormNpc
    {
        public int VarTextBaseCalls;
        public int SendCustemBaseCalls;
        public int ClickBaseCalls;

        public override bool GetVariableText(GXX.M2Server.Engine.TPlayObject p, ref string sMsg, string sVariable,
            ref bool IsBreakParseVar, int nPos)
        {
            VarTextBaseCalls++;
            return base.GetVariableText(p, ref sMsg, sVariable, ref IsBreakParseVar, nPos);
        }

        public override void SendCustemMsg(GXX.M2Server.Engine.TPlayObject p, string sMsg)
        {
            SendCustemBaseCalls++;
            base.SendCustemMsg(p, sMsg);
        }

        public override void Click(GXX.M2Server.Engine.TPlayObject p)
        {
            ClickBaseCalls++;
            base.Click(p);
        }
    }

    [Fact]
    public void V5_ThreeVirtualChains_DispatchThroughOverride()
    {
        var npc = new ProbeNpc();
        var player = new GXX.M2Server.Engine.TPlayObject();

        bool brk = false;
        npc.GetLineVariableText(player, "<$ANY>", ref brk);   // → 虚调用 GetVariableText
        Assert.True(npc.VarTextBaseCalls >= 1);

        npc.SendCustemMsg(player, "x");
        Assert.Equal(1, npc.SendCustemBaseCalls);

        npc.Click(player);
        Assert.Equal(1, npc.ClickBaseCalls);
    }

    [Fact]
    public void V5_MerchantClickSeamReceivesConcreteNpcInstance()
    {
        TNormNpc seen = null;
        NpcSeams.Click = (n, p) => seen = n;
        var m = new TMerchant();
        m.Click(new GXX.M2Server.Engine.TPlayObject());
        Assert.Same(m, seen);
    }

    // =======================================================================
    // V6：Pos / Copy 的 1-based 与越界语义（ObjNpc 覆盖路径实际用到的形态）
    // =======================================================================

    [Fact]
    public void V6_Pos_IsOneBasedAndZeroWhenMissing()
    {
        Assert.Equal(1, DelphiRTL.Pos(".", ".A"));
        Assert.Equal(2, DelphiRTL.Pos(".", "A.B"));
        Assert.Equal(0, DelphiRTL.Pos(".", "AB"));
        Assert.Equal(0, DelphiRTL.Pos(".", ""));
    }

    [Fact]
    public void V6_Copy_ClampsCountAndReturnsEmptyWhenOutOfRange()
    {
        Assert.Equal("ABC", DelphiRTL.Copy("ABC", 1, 10));   // Count 超长 → 截断
        Assert.Equal("", DelphiRTL.Copy("ABC", 4, 1));       // Index > Length → 空
        Assert.Equal("", DelphiRTL.Copy("ABC", 1, 0));       // Count = 0 → 空
        Assert.Equal("", DelphiRTL.Copy("", 1, 10));         // 源为空 → 空
    }

    // =======================================================================
    // V7：常量值逐条对照原文（防止转录错位）
    // =======================================================================

    [Fact]
    public void V7_CmdRaceConstantsMatchOriginalLines10To22()
    {
        Assert.Equal(0, ObjNpcConst.CMD_RACE_0);
        Assert.Equal(1, ObjNpcConst.CMD_RACE_1);
        Assert.Equal(2, ObjNpcConst.CMD_RACE_2);
        Assert.Equal(3, ObjNpcConst.CMD_RACE_3);
        Assert.Equal(4, ObjNpcConst.CMD_RACE_4);
        Assert.Equal(5, ObjNpcConst.CMD_RACE_5);
        Assert.Equal(6, ObjNpcConst.CMD_RACE_6);
        Assert.Equal(7, ObjNpcConst.CMD_RACE_7);
        Assert.Equal(8, ObjNpcConst.CMD_RACE_8);
        Assert.Equal(9, ObjNpcConst.CMD_RACE_9);
        Assert.Equal(10, ObjNpcConst.CMD_RACE_10);
        Assert.Equal(11, ObjNpcConst.CMD_RACE_11);
        Assert.Equal(12, ObjNpcConst.CMD_RACE_12);
        // M2Share.pas:87 / :96
        Assert.Equal(0, ObjNpcConst.LOG_ActionNone);
        Assert.Equal(9, ObjNpcConst.LOG_ItemDisappear);
    }

    [Fact]
    public void V7_M2ShareDirectoryConstantsMatchOriginalLines378To381()
    {
        Assert.Equal(@"Market_Def\", NpcSeams.sMarket_Def);  // M2Share.pas:378
        Assert.Equal(@"Npc_def\", NpcSeams.sNpc_def);        // M2Share.pas:379
        Assert.Equal(@"NpcIcons\", NpcSeams.sNpcIcons);      // M2Share.pas:381
    }

    [Fact]
    public void V7_BlackStoneDefaultNameMatchesM2ShareLine4142()
    {
        Assert.Equal("黑铁矿", NpcSeams.sBlackStone);
    }

    [Fact]
    public void V7_RcBoxValueIsThirty()
    {
        Assert.Equal(30, GXX.Core.Protocol.Grobal2Const.RC_BOX);
    }

    [Fact]
    public void V7_HeroExtOffLeavesZeroWhichEqualsCmdRaceZero()
    {
        // `CMD_RACE_0 = 0` 与"未赋值"的零初始化**不可区分** —— 这是最容易把
        // "HM 分支没生效"误判成"映射成了 self"的一处，用常量关系显式钉住。
        NpcSeams.g_nKey_HeroExt = 0;
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, "HM.X");
        Assert.Equal(ObjNpcConst.CMD_RACE_0, info.ScriptCmd[1]);
    }
}
