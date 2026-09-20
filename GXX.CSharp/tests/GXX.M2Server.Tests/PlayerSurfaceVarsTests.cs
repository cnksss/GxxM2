// ============================================================================
// 测试：本车道（p6-m2-playersurface）**变量容器片**（切片 1 / 4）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Vars.cs
//             GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.VarDefaults.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:119 / 123 / 125 / 127 / 260 /
//           1627 / 1833 / 1835 / 3849；ObjNpc.pas:9394 / 9417
// 用例 ≥3/方法：空 / 0 / 边界 / 超界 / 差异断言。
// ============================================================================

using System;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class PlayerSurfaceVarsTests
{
    // ---------------------------------------------------------------
    // 字段形状（原文 array 上下界）
    // ---------------------------------------------------------------

    [Fact]
    public void NVal_Has1000Elements_DefaultZero()
    {
        // 原文 ObjPlayer.pas:119 m_nVal: array [0 .. 999] of Integer
        var p = new TPlayObject();
        Assert.Equal(1000, p.m_nVal.Length);
        Assert.All(p.m_nVal, v => Assert.Equal(0, v));
        Assert.Equal(1000, PlayerVarSurfaceConst.NATIVE_VAR_COUNT);
    }

    [Fact]
    public void TVal_Has500Elements()
    {
        // 原文 ObjPlayer.pas:123 m_TVal: array [0 .. 499] of string[100]
        var p = new TPlayObject();
        Assert.Equal(500, p.m_TVal.Length);
        Assert.Equal(500, PlayerVarSurfaceConst.PRIVATE_VAR_COUNT);
    }

    [Fact]
    public void SString_Has1000Elements()
    {
        // 原文 ObjPlayer.pas:127 m_sString: array [0 .. 999] of string
        var p = new TPlayObject();
        Assert.Equal(1000, p.m_sString.Length);
    }

    [Fact]
    public void ArrayList_IsReusedExistingValueListStub_NotASecondCopy()
    {
        // 原文 ObjPlayer.pas:260 m_ArrayList: TValueList —— 复用 CombatPower.cs:567 的 TValueListStub
        var p = new TPlayObject();
        Assert.NotNull(p.m_ArrayList);
        Assert.IsType<TValueListStub>(p.m_ArrayList);
    }

    // ---------------------------------------------------------------
    // 差异断言：m_TVal / m_ZVal 的 ShortString(100) 截断
    // ---------------------------------------------------------------

    [Fact]
    public void TVal_ShortStringTruncation_DiffersFromPlainString()
    {
        // 原文 ObjPlayer.pas:123 是 string[100]：超过 100 字符**静默截断**。
        var p = new TPlayObject();
        string long101 = new string('x', 101);

        // ① 走 1:1 的 SetTVal（带截断）
        p.SetTVal(7, long101);
        Assert.Equal(100, p.GetTVal(7).Length);
        Assert.Equal(new string('x', 100), p.GetTVal(7));

        // ② 直接写数组（C# 原生赋值）**不截断** —— 这正是必须用 SetTVal 的原因
        p.m_TVal[8] = long101;
        Assert.Equal(101, p.m_TVal[8]!.Length);
        // → 差异断言：两条路径长度不同，说明「看起来一样实则不同」
        Assert.NotEqual(p.m_TVal[7]!.Length, p.m_TVal[8]!.Length);
    }

    [Fact]
    public void TVal_TruncShortString100_Boundary()
    {
        Assert.Equal("", TPlayObject.TruncShortString100(null!));
        Assert.Equal("", TPlayObject.TruncShortString100(""));
        Assert.Equal(100, TPlayObject.TruncShortString100(new string('a', 100)).Length);
        Assert.Equal(100, TPlayObject.TruncShortString100(new string('a', 1000)).Length);
        Assert.Equal(100, TPlayObject.SHORTSTRING_100_MAX);
    }

    [Fact]
    public void SString_IsNotShortString_HasNoTruncation()
    {
        // 原文 ObjPlayer.pas:127 是**无上限的 AnsiString**（与 m_TVal 的 string[100] 不同）
        var p = new TPlayObject();
        string long101 = new string('y', 101);
        p.m_sString[3] = long101;
        Assert.Equal(101, p.m_sString[3]!.Length);
    }

    // ---------------------------------------------------------------
    // 差异断言：m_ZVal 元素默认 null（托管）vs ''（原文）
    // ---------------------------------------------------------------

    [Fact]
    public void ZVal_ElementDefaultIsEmptyString_MatchingDelphi()
    {
        // 原文 ObjPlayer.pas:125 是 string[100] → 元素默认 ''；托管字段是 string[] → 元素默认 null。
        // 本用例原为"差异锁定"（断言锁定 null 这一缺陷本身）。
        // 集成方已在 `TPlayObject` 构造函数里调用
        // `PlayerSurfaceVarDefaults.MigrateStringVarDefaults(this)`（车道无法自行补：`ObjBase.cs:169`
        // 已有无参构造，它再声明一个会 CS0111），故此处改为断言**修正后的原文语义**。
        var p = new TPlayObject();

        // 直读字段即空串，不再是 null
        Assert.Equal("", p.m_ZVal[0]);
        Assert.Equal(0, p.m_ZVal[0]!.Length);   // 与原文一致后可直接取 Length，不抛
        // GetZVal 归一后同为 ''
        Assert.Equal("", p.GetZVal(0));
    }

    [Fact]
    public void MigrateStringVarDefaults_NormalizesNullToEmpty_Idempotent()
    {
        var p = new TPlayObject();
        long before = PlayerSurfaceVarDefaults.MigratedInstanceCount;

        PlayerSurfaceVarDefaults.MigrateStringVarDefaults(p);
        Assert.Equal(before + 1, PlayerSurfaceVarDefaults.MigratedInstanceCount);
        Assert.All(p.m_ZVal, v => Assert.Equal("", v));
        Assert.All(p.m_TVal, v => Assert.Equal("", v));
        Assert.All(p.m_sString, v => Assert.Equal("", v));

        // 幂等：再跑一次，已有值不被破坏
        p.SetTVal(1, "keep");
        PlayerSurfaceVarDefaults.MigrateStringVarDefaults(p);
        Assert.Equal("keep", p.GetTVal(1));
        Assert.All(p.m_ZVal, v => Assert.Equal("", v));
    }

    [Fact]
    public void ReadVar_NormalizesArbitraryStringContainer()
    {
        var arr = new string[3];
        Assert.Equal("", PlayerSurfaceVarDefaults.ReadVar(arr, 0));
        arr[1] = "v";
        Assert.Equal("v", PlayerSurfaceVarDefaults.ReadVar(arr, 1));
    }

    // ---------------------------------------------------------------
    // 整块清零（FillChar 等价）
    // ---------------------------------------------------------------

    [Fact]
    public void ClearNValues_WholeBlockZero_MatchesFillChar()
    {
        // 原文 ObjNpc.pas:9394/9417 FillChar(Player.m_nVal[0], SizeOf(Player.m_nVal), 0)
        var p = new TPlayObject();
        p.m_nVal[0] = -1;
        p.m_nVal[999] = 12345;
        p.ClearNValues();
        Assert.All(p.m_nVal, v => Assert.Equal(0, v));
    }

    [Fact]
    public void ClearTValues_And_ClearZValues_And_ClearSStrings()
    {
        // 原文 ObjPlayer.pas:1833 / 1835 / 3849 / 1627
        var p = new TPlayObject();
        p.m_TVal[0] = "t";
        p.m_ZVal[0] = "z";
        p.m_sString[0] = "s";

        p.ClearTValues();
        Assert.Null(p.m_TVal[0]);           // 原文是 ''；托管字段被 Array.Clear 置 null
        Assert.Equal("", p.GetTVal(0));     // 读取口归一

        p.ClearZValues();
        Assert.Null(p.m_ZVal[0]);
        Assert.Equal("", p.GetZVal(0));

        p.ClearSStrings();
        Assert.Null(p.m_sString[0]);
    }

    [Fact]
    public void SetZVal_AppliesShortStringTruncation()
    {
        var p = new TPlayObject();
        p.SetZVal(9, new string('z', 250));
        Assert.Equal(100, p.GetZVal(9).Length);
    }

    // ---------------------------------------------------------------
    // 存在性判定（S$ / N$ 具名表 —— 与 m_sString 数组是两套东西）
    // ---------------------------------------------------------------

    [Fact]
    public void HasStringVar_And_HasIntegerVar_AreDistinctFromIndexedArrays()
    {
        var p = new TPlayObject();
        Assert.False(p.HasStringVar("FOO"));
        Assert.False(p.HasIntegerVar("BAR"));

        p.m_StringList.Add("FOO", "1");
        p.m_IntegerList.Add("BAR", 7);

        Assert.True(p.HasStringVar("FOO"));
        Assert.True(p.HasIntegerVar("BAR"));
        // GetIndex 是 UpperCompare 语义（不区分大小写）
        Assert.True(p.HasStringVar("foo"));

        // 具名表与定长数组无关：m_sString[0] 写值不影响 HasStringVar
        p.m_sString[0] = "x";
        Assert.False(p.HasStringVar("0"));
    }
}
