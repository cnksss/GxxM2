// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：切片 46 的"消息/输入"族（**切片 46 当时漏测，本片补齐**）
//   · RemoteMsg   2177-2204
//   · InPutInteger 2461-2484
//   · InPutString  2486-2506
// ★ 重点：四处"看起来一样实则不同"的**差异断言**，其中两处是**行内位移**（只差 1 个字符 /
//   一条语句的位置）—— 位移类差异**只断言成功/失败抓不到**，故用**交叉前缀**取证：
//   把 InPutInteger 的标签换成 `@@InputString7`（反之亦然），若两者位移抄成一样，交叉用例会**双双成功**。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectInputTests : IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _labels = new();
    private readonly List<string> _sys = new();
    private readonly List<string> _dbg = new();

    public NpcObjNpcUserSelectInputTests()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        NpcSeams.SysMsg = (t, msg, color, type) => _sys.Add(msg);
        NpcSeams.MainOutMessage = s => _dbg.Add(s);
        PlayerSurfaceNpcSeams.GotoLable = (npc, p, label, ext) => { _labels.Add(label); return true; };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
    }

    private static TMerchant M() => new() { m_sCharName = "商人" };
    private static TPlayObject P(string name = "玩家")
        => new() { m_sCharName = name };

    // =======================================================================
    // RemoteMsg（2177-2204）—— 三态 × Trim/空串门
    // =======================================================================

    [Fact]
    public void RemoteMsg_TrimmedEmpty_DoesNothing()
    {
        // 2182 Trim → 2183 `sMsg <> ''` 门：全空白 ⇒ 什么都不做（连查名字都不查）
        bool queried = false;
        ObjNpcInputSeams.GetPlayObject = _ => { queried = true; return null; };
        M().RemoteMsg(P(), "@@rmst", "   ");
        Assert.False(queried);
        Assert.Empty(_sys);
        Assert.Empty(_sent);
    }

    [Fact]
    public void RemoteMsg_TargetNotFound_UsesGlobalNotOnLineWithMsgPrefix()
    {
        // 2201：`User.SysMsg(sMsg + g_sUserNotOnLine, c_Red, t_Hint)` —— ★ 保留 sMsg 前缀
        ObjNpcInputSeams.GetPlayObject = _ => null;
        M().RemoteMsg(P(), "@@rmst", "  张三  ");
        Assert.Equal("张三" + ObjNpcInputSeams.g_sUserNotOnLine, Assert.Single(_sys));
    }

    [Fact]
    public void RemoteMsg_TargetRejects_UsesTargetNameWithMsgPrefix()
    {
        // 2196：**保留 sMsg 前缀** + 对方名 + 固定文案
        ObjNpcInputSeams.GetPlayObject = _ => P("李四");
        ObjNpcInputSeams.GetBoRemoteMsg = _ => false;
        M().RemoteMsg(P(), "@@rmst", "张三");
        Assert.Equal("张三你的好友 李四 拒绝接受歌曲！", Assert.Single(_sys));
        Assert.Empty(_sent);
    }

    [Fact]
    public void RemoteMsg_TargetAccepts_SendsMusicWithoutMsgPrefix()
    {
        // 2190-2192：sLabel 去掉**第 1 个字符**；提示串里**不含** sMsg 前缀
        ObjNpcInputSeams.GetPlayObject = _ => P("李四");
        ObjNpcInputSeams.GetBoRemoteMsg = _ => true;
        M().RemoteMsg(P("张三"), "@@rmst", "李四");
        Assert.Empty(_sys);
        string msg = Assert.Single(_sent);
        Assert.Contains("张三", msg);          // User.m_sCharName
        Assert.Contains("@rmst", msg);         // sLabel 去首字符
        Assert.DoesNotContain("李四 拒绝", msg);
    }

    [Fact]
    public void RemoteMsg_QueryUsesTrimmedName()
    {
        // 2185 查的是**已 Trim** 的 sMsg
        string? asked = null;
        ObjNpcInputSeams.GetPlayObject = n => { asked = n; return null; };
        M().RemoteMsg(P(), "@@rmst", "  王五  ");
        Assert.Equal("王五", asked);
    }

    [Fact]
    public void RemoteMsg_DropFirstCharOfLabel_IsObservable()
    {
        // ★ 位移取证：`Copy(sLabel, 2, Len-1)` 去掉的正是**第 2 个 `@`**
        ObjNpcInputSeams.GetPlayObject = _ => P("李四");
        ObjNpcInputSeams.GetBoRemoteMsg = _ => true;
        M().RemoteMsg(P(), "@@rmst", "李四");
        Assert.Contains("<播放歌曲/@rmst>", Assert.Single(_sent));
    }

    // =======================================================================
    // ★ 位移差异①：Copy 起点/长度 —— 用**交叉前缀**取证
    // =======================================================================

    [Fact]
    public void InPutInteger_OwnPrefix_ExtractsSeven()
    {
        var p = P();
        M().InPutInteger(p, "@@InputInteger7", "42");
        Assert.Equal(42, p.m_nInteger[7]);
    }

    [Fact]
    public void InPutInteger_WithStringPrefix_YieldsNothing()
    {
        // ★ 交叉取证：`@@InputString7`（14 字符）用**整数版**的 `Copy(…,15,…)` 取到空串 → nNo = -1 → 不写
        var p = P();
        M().InPutInteger(p, "@@InputString7", "42");
        Assert.Empty(_labels);
    }

    [Fact]
    public void InPutString_OwnPrefix_ExtractsSeven()
    {
        var p = P();
        M().InPutString(p, "@@InputString7", "hello");
        Assert.Equal("hello", p.m_sString[7]);
    }

    [Fact]
    public void InPutString_WithIntegerPrefix_YieldsNothing()
    {
        // ★ 交叉取证：`@@InputInteger7`（15 字符）用**字符串版**的 `Copy(…,14,2)` 取到 `"r7"` → nNo = -1 → 不写
        var p = P();
        M().InPutString(p, "@@InputInteger7", "hello");
        Assert.Empty(_labels);
    }

    [Fact]
    public void InPutInteger_CrossPrefix_ExtractsDifferentDigitsThanStringVersion()
    {
        // ★ 位移的**同一输入双向对照**：同一个 `"@@InputString700"`（16 字符；`@@InputString` 为 13 字符）
        //   · 整数版 `Copy(s, 15, 16-14=2)` → `"00"` → nNo = **0**
        //   · 字符串版 `Copy(s, 14, 16-13=3)` → `"700"` → nNo = **700**
        //   ⇒ 两者都落在有效范围，但**下标不同**。只断言"成功/失败"抓不到位移，必须断言**取到哪个下标**。
        var a = P();
        M().InPutInteger(a, "@@InputString700", "9");
        Assert.Equal(9, a.m_nInteger[0]);                            // 整数版 → 下标 0
        Assert.Equal("@InputString700", Assert.Single(_labels));     // 只去掉第 1 个 `@`

        var b = P();
        M().InPutString(b, "@@InputString700", "9");
        Assert.Equal("9", b.m_sString[700]);                         // 字符串版 → 下标 700
        Assert.Equal(2, _labels.Count);                              // a、b 各跳一次（_labels 不跨用例清空）
        Assert.Equal("@InputString700", _labels[1]);

        // ★ 差异断言：两者写的是**不同下标**
        Assert.True(string.IsNullOrEmpty(a.m_sString[0]));
        Assert.True(string.IsNullOrEmpty(b.m_sString[0]));
        Assert.Equal(0, b.m_nInteger[0]);
    }

    // =======================================================================
    // ★ 位移差异②：过滤检查的**位置**（整数版在范围门之前、字符串版在之后）
    // =======================================================================

    [Fact]
    public void InPutInteger_FilterHit_RedirectsToIntegerFilterLabel()
    {
        ObjNpcInputSeams.GetInputBoxFilterList = () => new object();
        ObjNpcInputSeams.GetInputBoxInFilterList = _ => true;
        var p = P();
        M().InPutInteger(p, "@@InputInteger3", "5");
        Assert.Equal("@InputIntegerFilter", Assert.Single(_labels));
        Assert.Equal(0, p.m_nInteger[3]);          // 未写入
    }

    [Fact]
    public void InPutString_FilterHit_RedirectsToStringFilterLabel()
    {
        ObjNpcInputSeams.GetInputBoxFilterList = () => new object();
        ObjNpcInputSeams.GetInputBoxInFilterList = _ => true;
        var p = P();
        M().InPutString(p, "@@InputString3", "x");
        Assert.Equal("@InputStringFilter", Assert.Single(_labels));
        Assert.True(string.IsNullOrEmpty(p.m_sString[3]));
    }

    [Fact]
    public void InPutInteger_FilterCheckedBeforeRangeGate_OutOfRangeStillRedirects()
    {
        // ★ 位置差异：整数版 2470 的过滤检查在 2478 范围门**之前**
        //   ⇒ nNo 越界时**仍会**跳 `@InputIntegerFilter`
        ObjNpcInputSeams.GetInputBoxFilterList = () => new object();
        ObjNpcInputSeams.GetInputBoxInFilterList = _ => true;
        M().InPutInteger(P(), "@@InputIntegerNotANumber", "5");   // nNo = -1（越界）
        Assert.Equal("@InputIntegerFilter", Assert.Single(_labels));
    }

    [Fact]
    public void InPutString_RangeGateBeforeFilterCheck_OutOfRangeSkipsFilter()
    {
        // ★ 位置差异：字符串版 2492 的范围门在 2494 过滤检查**之前**
        //   ⇒ nNo 越界时**不会**跳 `@InputStringFilter`（与整数版相反）
        ObjNpcInputSeams.GetInputBoxFilterList = () => new object();
        ObjNpcInputSeams.GetInputBoxInFilterList = _ => true;
        M().InPutString(P(), "@@InputStringX", "x");              // nNo = -1（越界）
        Assert.Empty(_labels);
    }

    // =======================================================================
    // ★ 差异③：写入目标不同
    // =======================================================================

    [Fact]
    public void InPutInteger_WritesNInteger_NotString()
    {
        var p = P();
        M().InPutInteger(p, "@@InputInteger5", "77");
        Assert.Equal(77, p.m_nInteger[5]);
        Assert.True(string.IsNullOrEmpty(p.m_sString[5]));
    }

    [Fact]
    public void InPutString_WritesString_NotNInteger()
    {
        var p = P();
        M().InPutString(p, "@@InputString5", "abc");
        Assert.Equal("abc", p.m_sString[5]);
        Assert.Equal(0, p.m_nInteger[5]);
    }

    // =======================================================================
    // ★ 默认行为忠实性：`g_InputBoxFilterList == null` ⇒ 整段过滤被跳过
    // =======================================================================

    [Fact]
    public void FilterListNull_SkipsEntireFilterCheck_ForBothProcedures()
    {
        // ★ 显式用例：默认 `null` 时**连 GetInputBoxInFilterList 都不调用**（与原文 `<> nil` 门一致）
        int calls = 0;
        ObjNpcInputSeams.GetInputBoxFilterList = () => null;
        ObjNpcInputSeams.GetInputBoxInFilterList = _ => { calls++; return true; };

        var p = P();
        M().InPutInteger(p, "@@InputInteger4", "8");
        M().InPutString(p, "@@InputString4", "s");

        Assert.Equal(0, calls);                    // 一次都没调
        // ★ 过滤被跳过后**两条都走到成功路径**（`GotoLable` 到 `Copy(sLabel,2,Len-1)`）——
        //   这正是"跳过过滤"的可观测后果
        Assert.Equal(2, _labels.Count);
        Assert.Equal("@InputInteger4", _labels[0]);
        Assert.Equal("@InputString4", _labels[1]);
        Assert.Equal(8, p.m_nInteger[4]);          // 直接写入成功
        Assert.Equal("s", p.m_sString[4]);
    }

    // =======================================================================
    // 其它门（非数字 / 范围 / 长度上限）
    // =======================================================================

    [Fact]
    public void InPutInteger_NonNumeric_DoesNothing()
    {
        // 原文 2466 `IsStringNumber(sMsg)`
        var p = P();
        M().InPutInteger(p, "@@InputInteger2", "abc");
        Assert.Empty(_labels);
        Assert.Equal(0, p.m_nInteger[2]);
    }

    [Theory]
    [InlineData(0)]      // 下界
    [InlineData(999)]    // 上界
    public void InPutInteger_RangeBoundsAreInclusive(int nNo)
    {
        var p = P();
        M().InPutInteger(p, "@@InputInteger" + nNo, "3");
        Assert.Equal(3, p.m_nInteger[nNo]);
    }

    [Fact]
    public void InPutInteger_AboveUpperBound_DoesNothing()
    {
        var p = P();
        M().InPutInteger(p, "@@InputInteger1000", "3");   // nNo = 1000 > 999
        Assert.Empty(_labels);
    }

    [Fact]
    public void InPutString_RangeBoundsAreInclusive()
    {
        var p = P();
        M().InPutString(p, "@@InputString999", "z");
        Assert.Equal("z", p.m_sString[999]);
    }

    [Fact]
    public void InPutInteger_NonNumericWithFilterListNull_StillNoWrite()
    {
        // 交叉：过滤被跳过 + 非数字 → 仍不写（两个门独立）
        ObjNpcInputSeams.GetInputBoxFilterList = () => null;
        var p = P();
        M().InPutInteger(p, "@@InputInteger9", "xyz");
        Assert.Equal(0, p.m_nInteger[9]);
    }
}
