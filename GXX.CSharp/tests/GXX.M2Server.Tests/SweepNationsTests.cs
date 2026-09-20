// 测试：Source/M2Engine/Nations.pas → GXX.M2Server.Sweep.TNationManage（1:1）
//
// 覆盖策略（任务书第 3 条）：每个公开方法 ≥3 用例，含空/0/负/超界/异常路径；
// 对「看起来一样实则不同」的分支写差异断言：
//   * CompareText 只折 ASCII —— Kelvin 记号 'K'(U+212A) 不得匹配 'k'（OrdinalIgnoreCase 会匹配）；
//   * SendNationMsg 的 m_boBanNationChat 判定在原文里是**正逻辑**（只发给被禁言者）—— 差异断言；
//   * LoadConfig 的 FCount 只增不减 —— 重复调用累加（原文缺陷，断言现状）；
//   * Word/Byte 字段从 INI 读入时**截断**（M2Server.dpr 无 {$R+}）—— 边界断言。

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;
using Xunit;

namespace GXX.M2Server.Tests;

public class SweepNationsTests
{
    private const int Max = 1000; // Grobal2Const.MAXNATIONCOUNT

    private static string NationFile(string name) => ".\\Envir\\" + "\\Nations\\" + name + ".ini";

    private const string NationsIni = ".\\Envir\\" + "\\Nations\\Nations.ini";

    /// <summary>建一个「国家 1 = 甲国」的管理器（直接写 NationConfigList 只能经 LoadConfig，故用 LoadConfig 建）。</summary>
    private static TNationManage MakeWithNation(SweepTestEnv env, string nationName, int index = 1)
    {
        env.Fs.Seed(NationsIni, ("Names", "NationalNames" + index, nationName));
        env.Fs.Seed(NationFile(nationName));
        var mgr = new TNationManage();
        mgr.LoadConfig();
        return mgr;
    }

    // ------------------------------------------------------------------ 构造 / Count / Items

    [Fact]
    public void Ctor_CountZero_ItemsNull_And_ConfigDefaultsWritten()
    {
        using var env = new SweepTestEnv();
        var mgr = new TNationManage();
        Assert.Equal(0, mgr.Count);                 // FCount := 0
        Assert.Null(mgr[1]);                        // sName = '' → Get 返回 nil
        Assert.Null(mgr[0]);                        // 越界（原文 1..MAXNATIONCOUNT）
        Assert.Null(mgr[Max + 1]);                  // 越界
        Assert.Null(mgr[-1]);                       // 负数越界

        // 构造函数把 g_Config 的 Home* 默认值灌进每个槽位 → 用 SaveConfig 观测
        env.Fs.Seed(NationsIni, ("Names", "NationalNames1", "默认国"));
        var mgr2 = MakeWithNation(env, "默认国");
        mgr2.SaveConfig(1);
        string f = NationFile("默认国");
        Assert.Equal(M2Config.nRedHomeX.ToString(), env.Fs.Get(f, "Info", "nRedHomeX"));
        Assert.Equal(M2Config.nRedHomeY.ToString(), env.Fs.Get(f, "Info", "RedHomeY"));
        Assert.Equal(M2Config.nHomeX.ToString(), env.Fs.Get(f, "Info", "HomeX"));
        Assert.Equal(M2Config.nHomeY.ToString(), env.Fs.Get(f, "Info", "HomeY"));
        Assert.Equal(M2Config.sRedHomeMap, env.Fs.Get(f, "Info", "RedHomeMap"));
        Assert.Equal(M2Config.sHomeMap, env.Fs.Get(f, "Info", "HomeMap"));
        Assert.Equal("0", env.Fs.Get(f, "Info", "Peoples"));
        Assert.Equal("0", env.Fs.Get(f, "Info", "Gold"));
        Assert.Equal("", env.Fs.Get(f, "Info", "King"));
    }

    // ------------------------------------------------------------------ GetNationInfo / GetNationIndex

    [Fact]
    public void GetNationInfo_And_Index_EmptyUnknownCaseInsensitive()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni, ("Names", "NationalNames3", "BlueLand"));
        var mgr = new TNationManage();
        mgr.LoadConfig();

        Assert.Null(mgr.GetNationInfo(""));                     // 空串直接 nil（不进入循环）
        Assert.Equal(0, mgr.GetNationIndex(""));                // 空串 → 0
        Assert.Null(mgr.GetNationInfo("NoSuchNation"));         // 未找到
        Assert.Equal(0, mgr.GetNationIndex("NoSuchNation"));
        Assert.NotNull(mgr.GetNationInfo("BlueLand"));          // 精确
        Assert.Equal(3, mgr.GetNationIndex("BlueLand"));        // 下标 3
        Assert.NotNull(mgr.GetNationInfo("bLuElAnD"));          // CompareText 大小写不敏感
        Assert.Equal(3, mgr.GetNationIndex("BLUELAND"));
        Assert.Null(mgr.GetNationInfo("BlueLan"));              // 前缀不算匹配
        Assert.Null(mgr.GetNationInfo(" BlueLand"));            // 原文**不** Trim 查找键
    }

    /// <summary>差异断言：原文 System.pas 的 CompareText 只折 ASCII（UpCase 表），
    /// 不做 Unicode 区域折叠。用 Kelvin 记号 U+212A 当查找键必须**找不到**；
    /// 而 .NET 区域敏感比较（CurrentCulture/InvariantCulture）会把它折叠成 'k' —— 差异断言的意义所在。</summary>
    [Fact]
    public void GetNationIndex_CompareText_FoldsAsciiOnly_Not_CultureAware()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni, ("Names", "NationalNames5", "k"));
        var mgr = new TNationManage();
        mgr.LoadConfig();

        // 差异断言：原文 System.pas 的 CompareText 只折 ASCII（UpCase 表）。
        // 用 Kelvin 记号 U+212A 当查找键必须**找不到** —— 注意 .NET 的 OrdinalIgnoreCase 恰好也不折它，
        // 真正会误命中的是**区域敏感**的 CurrentCulture/InvariantCulture 比较（见下面的交叉验证）。
        Assert.Null(mgr.GetNationInfo("\u212A"));
        Assert.Equal(0, mgr.GetNationIndex("\u212A"));
        // 交叉验证：证明这条差异断言有意义 —— 区域敏感比较确实把 U+212A 与 'k' 视为相同
        Assert.True(string.Equals("\u212A", "k", StringComparison.InvariantCultureIgnoreCase));
        Assert.True(string.Equals("\u212A", "k", StringComparison.CurrentCultureIgnoreCase));
        // 中文按码位比较：不同字不命中，同字命中
        Assert.Null(mgr.GetNationInfo("国"));
    }

    [Fact]
    public void GetNationIndex_LastSlotBoundary()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni, ("Names", "NationalNames" + Max, "末位国"));
        var mgr = new TNationManage();
        mgr.LoadConfig();
        Assert.Equal(Max, mgr.GetNationIndex("末位国"));    // 第 1000 槽可用
        Assert.NotNull(mgr[Max]);
        Assert.Null(mgr[Max + 1]);                          // 越界仍 nil
    }

    // ------------------------------------------------------------------ GetNationName / 成员管理

    [Fact]
    public void GetNationName_ZeroNationAndEmptyName_ReturnEmpty()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        var p = new FakeNationPlayer { m_btNation = 0 };     // 未入国
        Assert.Equal("", mgr.GetNationName(p));
        Assert.False(mgr.IsMember(p));

        var p2 = new FakeNationPlayer { m_btNation = 2 };    // 2 号槽没有名字
        Assert.Equal("", mgr.GetNationName(p2));
        Assert.False(mgr.IsMember(p2));

        var p3 = new FakeNationPlayer { m_btNation = 1 };
        Assert.Equal("甲国", mgr.GetNationName(p3));
        Assert.True(mgr.IsMember(p3), "GetNationName 含 AddMember 副作用（原文 Nations.pas:120）");
    }

    [Fact]
    public void AddMember_Idempotent_And_PerNationIsolation()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        env.Fs.Seed(NationsIni, ("Names", "NationalNames2", "乙国"));
        mgr.LoadConfig();

        var a = new FakeNationPlayer { m_btNation = 1 };
        var b = new FakeNationPlayer { m_btNation = 1 };
        mgr.AddMember(a);
        mgr.AddMember(a);        // 重复 → 原文 Exit，不重复加入
        mgr.AddMember(b);
        Assert.True(mgr.IsMember(a));
        Assert.True(mgr.IsMember(b));

        var c = new FakeNationPlayer { m_btNation = 2 };
        Assert.False(mgr.IsMember(c));      // 国别隔离
        mgr.AddMember(c);
        Assert.True(mgr.IsMember(c));

        // 未入国（0）加入无效
        var z = new FakeNationPlayer { m_btNation = 0 };
        mgr.AddMember(z);
        Assert.False(mgr.IsMember(z));
    }

    [Fact]
    public void DeleteMember_RemovesFirstMatch_And_NoOpWhenAbsent()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        var a = new FakeNationPlayer { m_btNation = 1 };
        mgr.AddMember(a);
        Assert.True(mgr.IsMember(a));
        mgr.DeleteMember(a);
        Assert.False(mgr.IsMember(a));
        mgr.DeleteMember(a);                                // 再删一次 → 无操作
        Assert.False(mgr.IsMember(a));

        var z = new FakeNationPlayer { m_btNation = 0 };
        mgr.DeleteMember(z);                                // 越界国别 → 无操作，不抛
        Assert.False(mgr.IsMember(z));

        var p = new FakeNationPlayer { m_btNation = 1 };
        mgr.GetNationName(p);                               // 入国
        mgr.DeleteMember(p);
        Assert.False(mgr.IsMember(p));
    }

    [Fact]
    public void IsMember_EmptyNationList_ReturnsFalse()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        var p = new FakeNationPlayer { m_btNation = 1 };
        Assert.False(mgr.IsMember(p));      // 列表为空
        p.m_btNation = 0;
        Assert.False(mgr.IsMember(p));
    }

    // ------------------------------------------------------------------ SendNationMsg

    [Fact]
    public void SendNationMsg_OnlyBanNationChatReceivers_OriginalInvertedLogic()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        M2Config.boShowPreFixMsg = true;
        M2Config.sNationMsgPreFix = "〖国家〗";

        var banned = new FakeNationPlayer { m_btNation = 1, BanNationChat = true };
        var normal = new FakeNationPlayer { m_btNation = 1, BanNationChat = false };
        mgr.AddMember(banned);
        mgr.AddMember(normal);

        mgr.SendNationMsg(1, "开战了");

        // 差异断言：原文 Nations.pas:198 是 `if PlayObject.m_boBanNationChat then`
        // → **只有被禁言者**收到消息；未被禁言者收不到（语义可疑，逐字保留）。
        Assert.Single(banned.Sent);
        Assert.Empty(normal.Sent);
        Assert.Equal((ushort)Grobal2Const.RM_NATIONMESSAGE, banned.Sent[0].wIdent);
        Assert.Equal(0L, banned.Sent[0].wParam);
        Assert.Equal((long)M2Config.btNationMsgFColor, banned.Sent[0].nParam1);
        Assert.Equal((long)M2Config.btNationMsgBColor, banned.Sent[0].nParam2);
        Assert.Equal(0L, banned.Sent[0].nParam3);
        Assert.Equal("〖国家〗开战了", banned.Sent[0].sMsg);   // 前缀已拼接
    }

    /// <summary>
    /// 差异断言（上一轮实测抓出的真实缺陷）：<c>m_btNation</c> 在原文里是 **Word**（<c>ObjBase.pas:408</c>），
    /// 不是 Byte。<c>MAXNATIONCOUNT = 1000</c> 超出 Byte 上界 —— 若按 Byte 建模，
    /// 1000 号国家的成员会被悄悄塞进 1 号列表，<c>SendNationMsg(1000, …)</c> 永远发不出去且无任何报错。
    /// 这里钉死：槽位 1000 的成员必须能被 <c>AddMember</c>/<c>IsMember</c> 正确定位与投递。
    /// </summary>
    [Fact]
    public void MaxNationSlot_MemberRouting_WordNotByte()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni, ("Names", "NationalNames" + Max, "末位国"));
        env.Fs.Seed(NationFile("末位国"));
        var mgr = new TNationManage();
        mgr.LoadConfig();
        Assert.Equal(1, mgr.Count);
        Assert.Equal(Max, mgr.GetNationIndex("末位国"));

        var last = new FakeNationPlayer { m_btNation = Max, BanNationChat = true };
        mgr.AddMember(last);
        Assert.True(mgr.IsMember(last));

        mgr.SendNationMsg(Max, "末位");
        Assert.Single(last.Sent);

        // 999 与 1 号槽都没有成员 → 不得误投（若 m_btNation 被截成 Byte，1 号槽会误命中）
        mgr.SendNationMsg((ushort)999, "九九九");
        mgr.SendNationMsg((ushort)1, "一");
        Assert.Single(last.Sent);

        // 超上界（1001）与 0：AddMember 不生效
        var over = new FakeNationPlayer { m_btNation = (ushort)(Max + 1) };
        mgr.AddMember(over);
        Assert.False(mgr.IsMember(over));
    }

    [Fact]
    public void SendNationMsg_PrefixDisabled_And_OutOfRange()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        M2Config.boShowPreFixMsg = false;
        M2Config.sNationMsgPreFix = "〖国家〗";

        var p = new FakeNationPlayer { m_btNation = 1, BanNationChat = true };
        mgr.AddMember(p);

        mgr.SendNationMsg(1, "原始文本");
        Assert.Equal("原始文本", p.Sent[0].sMsg);              // 不加前缀

        mgr.SendNationMsg(0, "越界");                          // btNation=0 → 不进入循环
        mgr.SendNationMsg((ushort)(Max + 1), "越界");          // 1001 → 不进入循环
        Assert.Single(p.Sent);

        // MAX 槽位可用：m_btNation 原文是 **Word**（ObjBase.pas:408），1000 号国家完全可达。
        var mgrMax = MakeWithNation(env, "末位国", Max);
        var last = new FakeNationPlayer { m_btNation = Max, BanNationChat = true };
        mgrMax.AddMember(last);
        mgrMax.SendNationMsg(Max, "末位");
        Assert.Single(last.Sent);
        Assert.Equal("末位", last.Sent[0].sMsg);
    }

    [Fact]
    public void SendNationMsg_ExceptionPath_LogsCheckCodeAndMessage()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        M2Config.boShowPreFixMsg = false;
        SweepSeam.LoggedMessages.Clear();

        var bad = new FakeNationPlayer
        {
            m_btNation = 1,
            BanNationChat = true,
            ThrowOnSend = new InvalidOperationException("socket broken")
        };
        var after = new FakeNationPlayer { m_btNation = 1, BanNationChat = true };
        mgr.AddMember(bad);
        mgr.AddMember(after);

        mgr.SendNationMsg(1, "内容");

        Assert.Equal(2, SweepSeam.LoggedMessages.Count);
        Assert.Equal("[Exceptiion] TNationManage.SendNationMsg CheckCode: 5 Msg = 内容",
            SweepSeam.LoggedMessages[0]);                      // 原文 'Exceptiion' 拼写
        Assert.Equal("socket broken", SweepSeam.LoggedMessages[1]);
        Assert.Empty(after.Sent);                              // 异常中断了整个循环（原文如此）
    }

    [Fact]
    public void SendNationMsg_ExceptionInBanGetter_CheckCode4()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        M2Config.boShowPreFixMsg = true;
        M2Config.sNationMsgPreFix = "P:";
        SweepSeam.LoggedMessages.Clear();

        var bad = new FakeNationPlayer
        {
            m_btNation = 1,
            ThrowOnBanGet = new InvalidOperationException("field boom")
        };
        mgr.AddMember(bad);
        mgr.SendNationMsg(1, "M");

        Assert.Equal("[Exceptiion] TNationManage.SendNationMsg CheckCode: 4 Msg = P:M",
            SweepSeam.LoggedMessages[0]);
    }

    // ------------------------------------------------------------------ SaveConfig

    [Fact]
    public void SaveConfig_Single_OutOfRangeAndEmptyName_NoFile()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");
        var info = mgr[1];
        info.nPeoples = 7;
        info.sKingName = "王";
        info.nGold = 123456;
        info.wBuilding = 65535;
        info.wArm = 1;
        info.wEconomy = 2;
        info.wPolitics = 3;
        info.wContribution = 4;
        info.btMaps = 255;

        mgr.SaveConfig(1);
        string f = NationFile("甲国");
        Assert.True(env.Fs.FileExists(f));
        Assert.Equal("7", env.Fs.Get(f, "Info", "Peoples"));
        Assert.Equal("王", env.Fs.Get(f, "Info", "King"));
        Assert.Equal("123456", env.Fs.Get(f, "Info", "Gold"));
        Assert.Equal("65535", env.Fs.Get(f, "Info", "Building"));    // Word 上界
        Assert.Equal("1", env.Fs.Get(f, "Info", "Arm"));
        Assert.Equal("2", env.Fs.Get(f, "Info", "Economy"));
        Assert.Equal("3", env.Fs.Get(f, "Info", "Politics"));
        Assert.Equal("4", env.Fs.Get(f, "Info", "Contribution"));
        Assert.Equal("255", env.Fs.Get(f, "Info", "Maps"));          // Byte 上界
        Assert.Equal("甲国", info.sName);

        mgr.SaveConfig(0);                                          // 越界 → 不写
        mgr.SaveConfig((ushort)(Max + 1));
        Assert.Equal(2, env.Fs.Files.Count);                        // 仍只有 Nations.ini + 甲国.ini

        // 名字为空的国家：SaveConfig 不落盘
        var empty = new TNationManage();
        empty.SaveConfig(1);
        empty.SaveConfig();
        Assert.Equal(2, env.Fs.Files.Count);
    }

    [Fact]
    public void SaveConfig_All_WritesEveryNamedNation_AndCreatesDir()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni,
            ("Names", "NationalNames1", "甲国"),
            ("Names", "NationalNames2", "乙国"));
        env.Fs.Seed(NationFile("甲国"));
        env.Fs.Seed(NationFile("乙国"));
        var mgr = new TNationManage();
        mgr.LoadConfig();
        Assert.Equal(2, mgr.Count);

        env.Fs.Dirs.Clear();                     // 模拟目录不存在
        mgr.SaveConfig();

        Assert.True(env.Fs.DirectoryExists(".\\Envir\\" + "\\Nations\\"), "ForceDirectories 应被调用");
        Assert.True(env.Fs.FileExists(NationFile("甲国")));
        Assert.True(env.Fs.FileExists(NationFile("乙国")));
        Assert.True(env.Fs.Has(NationFile("甲国"), "Info", "Peoples"));
        Assert.True(env.Fs.Has(NationFile("乙国"), "Info", "Peoples"));
    }

    // ------------------------------------------------------------------ LoadConfig

    [Fact]
    public void LoadConfig_NoFile_DoesNothing()
    {
        using var env = new SweepTestEnv();
        var mgr = new TNationManage();
        mgr.LoadConfig();
        Assert.Equal(0, mgr.Count);
        Assert.Empty(env.Fs.Files);              // 只读用法不得创建文件
        Assert.Null(mgr[1]);
    }

    [Fact]
    public void LoadConfig_TrimsNames_And_CountsOnlyExistingNationFiles()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni,
            ("Names", "NationalNames1", "  甲国  "),   // 前后空白 → Trim
            ("Names", "NationalNames2", "乙国"),       // 无窗口文件 → 不计数
            ("Names", "NationalNames3", ""));          // 空名 → 跳过
        env.Fs.Seed(NationFile("甲国"),
            ("Info", "Peoples", "5"),
            ("Info", "nRedHomeX", "9"),
            ("Info", "King", "老王"),
            ("Info", "Building", "70000"),             // 超 Word 上界 → 截断
            ("Info", "Maps", "300"));                  // 超 Byte 上界 → 截断
        var mgr = new TNationManage();
        mgr.LoadConfig();

        Assert.Equal(1, mgr.Count);                   // 只有"甲国"的 ini 存在
        Assert.Equal(1, mgr.GetNationIndex("甲国"));   // 名字已 Trim
        var info = mgr[1];
        Assert.Equal(5, info.nPeoples);
        Assert.Equal(9, info.nRedHomeX);
        Assert.Equal("老王", info.sKingName);
        // 原文 (Word)ReadInteger / (Byte)ReadInteger 的截断语义：只保低 16 / 低 8 位。
        Assert.Equal(unchecked((ushort)70000), info.wBuilding); // 70000 & 0xFFFF = 4464
        Assert.Equal(4464, info.wBuilding);
        Assert.Equal(unchecked((byte)300), info.btMaps);        // 300 & 0xFF = 44
        Assert.Equal(44, info.btMaps);
    }

    [Fact]
    public void LoadConfig_MissingKeys_FallBackToConfigDefaults()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni, ("Names", "NationalNames1", "甲国"));
        env.Fs.Seed(NationFile("甲国"));               // 空文件 → 全部走默认值
        var mgr = new TNationManage();
        mgr.LoadConfig();
        var info = mgr[1];
        Assert.Equal(0, info.nPeoples);
        Assert.Equal(M2Config.sRedHomeMap, info.sRedHomeMap);
        Assert.Equal(M2Config.nRedHomeX, info.nRedHomeX);
        Assert.Equal(M2Config.nRedHomeY, info.nRedHomeY);
        Assert.Equal(M2Config.sHomeMap, info.sHomeMap);
        Assert.Equal(M2Config.nHomeX, info.nHomeX);
        Assert.Equal(M2Config.nHomeY, info.nHomeY);
        Assert.Equal("", info.sKingName);             // 默认是 NationInfo.sKingName（ctor 置 ''）
        Assert.Equal(0, info.nGold);
        Assert.Equal(0, info.btMaps);
    }

    /// <summary>差异/缺陷断言：原文 Nations.pas:328 <c>Inc(FCount)</c> 不重置 → 重复 LoadConfig 累加。</summary>
    [Fact]
    public void LoadConfig_RepeatedCall_AccumulatesCount_OriginalDefect()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni, ("Names", "NationalNames1", "甲国"));
        env.Fs.Seed(NationFile("甲国"));
        var mgr = new TNationManage();
        mgr.LoadConfig();
        Assert.Equal(1, mgr.Count);
        mgr.LoadConfig();
        Assert.Equal(2, mgr.Count);   // 原文如此（不重置 FCount）
        mgr.LoadConfig();
        Assert.Equal(3, mgr.Count);
    }

    // ------------------------------------------------------------------ RenameNationName

    [Fact]
    public void RenameNationName_RejectsExistingOrSameName()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni,
            ("Names", "NationalNames1", "甲国"),
            ("Names", "NationalNames2", "乙国"));
        env.Fs.Seed(NationFile("甲国"));
        env.Fs.Seed(NationFile("乙国"));
        var mgr = new TNationManage();
        mgr.LoadConfig();

        mgr.RenameNationName(1, "乙国");                       // 重名 → Exit
        Assert.Equal("甲国", mgr[1].sName);

        mgr.RenameNationName(1, "甲国");                       // 改成同名 → GetNationIndex>0 → Exit
        Assert.Equal("甲国", mgr[1].sName);

        mgr.RenameNationName(1, "bLuElAnD");                   // 不存在的名字？注意 CompareText：'乙国' ≠ 'bLuElAnD'
        Assert.Equal("bLuElAnD", mgr[1].sName);                // 允许改名
    }

    [Fact]
    public void RenameNationName_RenamesFile_Members_AndNationsIni()
    {
        using var env = new SweepTestEnv();
        env.Fs.Seed(NationsIni,
            ("Names", "NationalNames1", "甲国"),
            ("Names", "NationalNames2", ""));
        env.Fs.Seed(NationFile("甲国"), ("Info", "King", "老王"));
        var mgr = new TNationManage();
        mgr.LoadConfig();
        var member = new FakeNationPlayer { m_btNation = 1, m_sNationaName = "甲国" };
        mgr.AddMember(member);

        mgr.RenameNationName(1, "新甲国");

        Assert.Equal("新甲国", mgr[1].sName);
        Assert.Equal("新甲国", member.m_sNationaName);          // 成员名字同步
        Assert.False(env.Fs.FileExists(NationFile("甲国")));     // 旧文件被 RenameFile 移走
        Assert.True(env.Fs.FileExists(NationFile("新甲国")));
        Assert.Equal("老王", env.Fs.Get(NationFile("新甲国"), "Info", "King"));  // 内容随文件搬走
        Assert.Equal("新甲国", env.Fs.Get(NationsIni, "Names", "NationalNames1")); // Nations.ini 重写
        Assert.Equal("", env.Fs.Get(NationsIni, "Names", "NationalNames2"));
    }

    [Fact]
    public void RenameNationName_OutOfRange_NoChange_ButExistingNameStillExits()
    {
        using var env = new SweepTestEnv();
        var mgr = MakeWithNation(env, "甲国");

        mgr.RenameNationName(0, "无关名");                  // 越界 → 无操作
        Assert.Equal("甲国", mgr[1].sName);
        Assert.False(env.Fs.FileExists(NationFile("无关名")));

        mgr.RenameNationName((ushort)(Max + 1), "无关名2");
        Assert.False(env.Fs.FileExists(NationFile("无关名2")));

        // 越界但新名已存在 → 先 Exit（原文 :365 的检查在边界检查之前）
        mgr.RenameNationName(0, "甲国");
        Assert.Equal("甲国", mgr[1].sName);
    }
}
