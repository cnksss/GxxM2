// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：`TMerchant.UserSelect` 的**解析段 + 门控链**（原文 2527-2596）
//   —— 落在 `TMerchant.UserSelectPrepare`（等派发体落地后内联进 UserSelect 并删除）
// 五个易抄错点均写**差异断言**：ClassNameIs 精确类名 / 2533 优先级 / 2538 两出口 /
//   2542 Pos 返回 0 / 2578 用 Length(sLabel)。
// ⚠ 切分处按纪律写差异断言（空串 / 仅分隔符 / 连续分隔符 / 首尾分隔符）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectPrepareTests : IDisposable
{
    private readonly List<string> _msg = new();
    private readonly List<string> _labels = new();

    public NpcObjNpcUserSelectPrepareTests()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        NpcSeams.MainOutMessage = s => _msg.Add(s);
        PlayerSurfaceNpcSeams.GotoLable = (npc, p, label, ext) => { _labels.Add(label); return true; };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
    }

    /// <summary>用于验证 `ClassNameIs` 是**精确类名**比较的派生类。</summary>
    private sealed class DerivedMerchant : TMerchant { }

    private static TMerchant M() => new() { m_sCharName = "商人" };
    private static TPlayObject P() => new() { m_sCharName = "玩家" };

    private static bool Prep(TMerchant m, TPlayObject p, string sData,
        out string sLabel, out string sMsg, out bool allow, out bool canGoto, out bool canJmp)
        => m.UserSelectPrepare(p, sData, out sLabel, out sMsg, out allow, out canGoto, out canJmp);

    // -----------------------------------------------------------------------
    // ★ 易抄错点 ①：ClassNameIs 是**精确类名**（不是 is / 派生判定）
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_DerivedClassInstance_ReturnsFalse_LikeClassNameIs()
    {
        // ★ 差异断言：`is TMerchant` 会**通过**，而原文 `ClassNameIs(TMerchant.ClassName)` **不通过**
        var d = new DerivedMerchant();
        Assert.True(d is TMerchant);                       // C# 的 is：派生类也满足
        Assert.False(Prep(d, P(), "@a", out _, out _, out _, out _, out _));   // 原文：精确类名 → Exit
    }

    [Fact]
    public void Prepare_ExactClassInstance_Continues()
        => Assert.True(Prep(M(), P(), "@a", out _, out _, out _, out _, out _));

    // -----------------------------------------------------------------------
    // 2535 / 2533：数据门与城堡门
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_EmptyData_ContinuesButLeavesLabelStackUntouched()
    {
        var p = P();
        Assert.True(Prep(M(), p, "", out string l, out string s, out _, out _, out _));
        Assert.Equal("", l);
        Assert.Equal("", s);
        Assert.Equal("", p.m_sScriptLable);      // 2565 未执行
    }

    [Fact]
    public void Prepare_NonAtData_ContinuesWithoutParsing()
    {
        var p = P();
        Assert.True(Prep(M(), p, "plain", out string l, out _, out _, out _, out _));
        Assert.Equal("", l);
        Assert.Equal("", p.m_sScriptLable);
    }

    [Fact]
    public void Prepare_CastleNpcWithUnderWar_DoesNotEnterParseBranch()
    {
        // ★ 2533 优先级差异断言：`m_boCastle=true` 且 underWar=true → 整段**不执行**
        //   （若误抄成 `(not m_boCastle or not X) and Y`，结果会相反）
        var m = M();
        m.m_boCastle = true;
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.GetCastleUnderWar = _ => true;

        Assert.True(Prep(m, P(), "@a", out string l, out _, out _, out _, out _));
        Assert.Equal("", l);                     // 未解析
    }

    [Fact]
    public void Prepare_CastleNpcWithoutUnderWar_EntersParseBranch()
    {
        // 对照：underWar=false → `(!true) || ((!false) && true)` = true → 进入
        var m = M();
        m.m_boCastle = true;
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.GetCastleUnderWar = _ => false;

        Assert.True(Prep(m, P(), "@a", out string l, out _, out _, out _, out _));
        Assert.Equal("@a", l);
    }

    [Fact]
    public void Prepare_CastleNpcWithoutCastle_EntersParseBranch()
    {
        // m_Castle = nil → castleUnderWar 保持 false → 进入（且不触碰 D37 接缝）
        var m = M();
        m.m_boCastle = true;
        NpcSeams.GetNpcCastle = _ => null;

        Assert.True(Prep(m, P(), "@a", out string l, out _, out _, out _, out _));
        Assert.Equal("@a", l);
    }

    // -----------------------------------------------------------------------
    // ★ 易抄错点 ③：2538 GetValidStr3_Ex 两个出口（ref sLabel + 返回剩余串）
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_BothOutletsArePopulated()
    {
        Assert.True(Prep(M(), P(), "@标签\r剩余串", out string l, out string s, out _, out _, out _));
        Assert.Equal("@标签", l);       // ref 出口
        Assert.Equal("剩余串", s);      // 返回串出口
    }

    // ⚠ 切分差异断言（纪律要求：空串 / 仅分隔符 / 连续分隔符 / 首尾分隔符）
    [Fact]
    public void Prepare_Split_OnlySeparator_YieldsEmptyLabel()
    {
        Assert.True(Prep(M(), P(), "\r", out string l, out _, out _, out _, out _));
        Assert.Equal("", l);
    }

    [Fact]
    public void Prepare_Split_LeadingSeparator_YieldsEmptyLabel()
    {
        // 首分隔符 → 第一段为空；注意 sData[0] 是 '\r' 而非 '@' ⇒ 2535 门不通过（走不到切分）
        var p = P();
        Assert.True(Prep(M(), p, "\r@a", out string l, out _, out _, out _, out _));
        Assert.Equal("", l);
        Assert.Equal("", p.m_sScriptLable);
    }

    [Fact]
    public void Prepare_Split_ConsecutiveSeparators_KeepsRestRawInMsg()
    {
        // `@a\r\rb`：sLabel = "@a"，剩余串 = "\rb"（**原样**，未再切）
        Assert.True(Prep(M(), P(), "@a\r\rb", out string l, out string s, out _, out _, out _));
        Assert.Equal("@a", l);
        Assert.Equal("\rb", s);
    }

    [Fact]
    public void Prepare_Split_TrailingSeparator_YieldsEmptyRest()
    {
        Assert.True(Prep(M(), P(), "@a\r", out string l, out string s, out _, out _, out _));
        Assert.Equal("@a", l);
        Assert.Equal("", s);
    }

    // -----------------------------------------------------------------------
    // ★ 易抄错点 ④：2542 Pos('(') 找不到返回 0
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_ParenthesisedLabel_IsTruncatedBeforeParen()
    {
        // 2540 三重条件：第 2 字符 '@' + 末字符 ')' → 再按 '(' 截断
        Assert.True(Prep(M(), P(), "@@InputInteger1(请输入)", out string l, out _, out _, out _, out _));
        Assert.Equal("@@InputInteger1", l);
    }

    [Fact]
    public void Prepare_LabelWithParenButNoClosingParen_IsNotTruncated()
    {
        // 末字符不是 ')' → 2540 条件为假 → **不截断**（差异断言）
        Assert.True(Prep(M(), P(), "@@InputInteger1(请输入", out string l, out _, out _, out _, out _));
        Assert.Equal("@@InputInteger1(请输入", l);
    }

    [Fact]
    public void Prepare_LabelWithoutSecondAt_IsNotTruncated()
    {
        // 第 2 字符不是 '@' → 不截断
        Assert.True(Prep(M(), P(), "@x(abc)", out string l, out _, out _, out _, out _));
        Assert.Equal("@x(abc)", l);
    }

    // -----------------------------------------------------------------------
    // 2549-2562：FOUNDRY / SHOWITEM 归一 + m_sScriptLable/m_sInputData
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_FoundryItem_StripsPrefixIntoSelectItemName()
    {
        var p = P();
        Assert.True(Prep(M(), p, "@FOUNDRYITEM_屠龙", out string l, out _, out _, out _, out _));
        Assert.Equal("@FOUNDRYITEM_", l);
        Assert.Equal("屠龙", p.m_sNpcSelectItemName);
    }

    [Fact]
    public void Prepare_ShowItem_StripsPrefixIntoSelectItemName()
    {
        var p = P();
        Assert.True(Prep(M(), p, "@SHOWITEM_戒指", out string l, out _, out _, out _, out _));
        Assert.Equal("@SHOWITEM_", l);
        Assert.Equal("戒指", p.m_sNpcSelectItemName);
    }

    [Fact]
    public void Prepare_OtherLabel_ClearsSelectItemName()
    {
        var p = P();
        p.m_sNpcSelectItemName = "残留";
        Assert.True(Prep(M(), p, "@other", out _, out _, out _, out _, out _));
        Assert.Equal("", p.m_sNpcSelectItemName);
    }

    [Fact]
    public void Prepare_WritesScriptLableAndInputData()
    {
        var p = P();
        Assert.True(Prep(M(), p, "@a\rmsg", out _, out _, out _, out _, out _));
        Assert.Equal("@a\rmsg", p.m_sScriptLable);   // 2565：原始 sData（**不是** sLabel）
        Assert.Equal("msg", p.m_sInputData);         // 2566：剩余串
    }

    // -----------------------------------------------------------------------
    // ★ 易抄错点 ⑤：2578 长度参数是 Length(sLabel)
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_MessageBoxBranch_UsesLabelLengthAsPrefixLength()
    {
        // `m_boMessageBox=true` + Yes 标签 → boCanGoto 转真（前缀比较用 Length(sLabel)）
        var p = P();
        p.m_boMessageBox = true;
        p.m_sYesLable = "@yes_very_long_suffix";
        Assert.True(Prep(M(), p, "@yes", out _, out _, out _, out bool canGoto, out _));
        Assert.True(canGoto);
    }

    [Fact]
    public void Prepare_MessageBoxOff_DoesNotUseYesNo()
    {
        var p = P();
        p.m_boMessageBox = false;
        p.m_sYesLable = "@yes";
        Assert.True(Prep(M(), p, "@yes", out _, out _, out _, out bool canGoto, out _));
        Assert.False(canGoto);   // LableIsCanJmp 默认 false，且不走 Yes/No
    }

    [Fact]
    public void Prepare_LableIsCanJmpSeamDrivesCanGoto()
    {
        NpcSeams.LableIsCanJmp = (_, l) => l == "@ok";
        Assert.True(Prep(M(), P(), "@ok", out _, out _, out _, out bool g1, out _));
        Assert.True(g1);
        Assert.True(Prep(M(), P(), "@no", out _, out _, out _, out bool g2, out _));
        Assert.False(g2);
    }

    [Fact]
    public void Prepare_IdentityMatch_AndsAllowSelectIntoCanGoto()
    {
        // 2572-2573：三全局之一时 `boCanGoto := LableIsCanJmp(...) and boAllowSelect`
        NpcSeams.LableIsCanJmp = (_, _) => true;
        var m = M();
        m.m_NoUserSelectList.Add("@blocked");   // AllowSelect → false

        // 非身份 NPC：不加 allowSelect → true
        Assert.True(Prep(m, P(), "@blocked", out _, out _, out _, out bool g1, out _));
        Assert.True(g1);

        // 身份 NPC 命中：true and false → false
        NpcSeams.IsManageNpc = n => true;
        Assert.True(Prep(m, P(), "@blocked", out _, out _, out _, out bool g2, out _));
        Assert.False(g2);
    }

    // -----------------------------------------------------------------------
    // 早退分支（2586 / 2595）
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_FunctionNpcWithDisallowedSelect_ExitsAndMessages()
    {
        // 2581-2588：函数 NPC 且 not boAllowSelect → MainOutMessage + 早退
        NpcSeams.IsFunctionNpc = n => true;
        var m = M();
        m.m_NoUserSelectList.Add("@blocked");

        Assert.False(Prep(m, P(), "@blocked", out _, out _, out _, out _, out _));
        Assert.Single(_msg);
        Assert.Contains("禁止点用该NPC触发字段", _msg[0]);
    }

    [Fact]
    public void Prepare_SendMsgLabelWithEmptyMsg_Exits()
    {
        // 2592-2596：sLabel = sNF_SendMsg 且 sMsg = '' → 早退
        Assert.False(Prep(M(), P(), "@@sendmsg", out _, out string s, out _, out _, out _));
        Assert.Equal("", s);
    }

    [Fact]
    public void Prepare_SendMsgLabelWithNonEmptyMsg_Continues()
    {
        Assert.True(Prep(M(), P(), "@@sendmsg\rhello", out _, out string s, out _, out _, out _));
        Assert.Equal("hello", s);
    }

    // -----------------------------------------------------------------------
    // 2527：inherited 落到基类（TNormNpc.UserSelect）—— m_nScriptGotoCount 归零
    // -----------------------------------------------------------------------

    [Fact]
    public void Prepare_CallsBaseUserSelect_ResettingGotoCount()
    {
        var p = P();
        p.m_nScriptGotoCount = 7;
        Prep(M(), p, "@a", out _, out _, out _, out _, out _);
        Assert.Equal(0, p.m_nScriptGotoCount);
    }

    [Fact]
    public void Prepare_DerivedClass_StillCallsBaseBeforeExiting()
    {
        // 派生类实例：2529 的 Exit 在 2527 之后 → 基类**已经被调用**
        var p = P();
        p.m_nScriptGotoCount = 7;
        Assert.False(Prep(new DerivedMerchant(), p, "@a", out _, out _, out _, out _, out _));
        Assert.Equal(0, p.m_nScriptGotoCount);
    }

    [Fact]
    public void LableIsCanJmp_DefaultIsFalse_FaithfulToOriginalEmptyTable()
    {
        NpcSeams.ResetDefaults();
        Assert.False(NpcSeams.LableIsCanJmp(P(), "@anything"));
    }
}
