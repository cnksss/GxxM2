using System;
using System.Linq;
using System.Reflection;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// SelectClient.pas:889-1237 —— 角色族：RandomName / NewChr / QueryDelChr / GetBackDelChr /
/// DelChr / SelectChr / QueryChr。
/// 这些方法都是 private，测试统一从 <c>ExecGateBuffers</c> 帧入口驱动（端到端）；
/// 只有"函数返回值本身是缺陷"的两处（QueryChr / GetBackDelChr 恒返回 False）才用反射直调以便锁定。
/// </summary>
public class SelectClientRoleTests : SelectClientTestBase
{
    private static TDefaultMessage Cmd(int ident)
        => TDefaultMessage.Make((ushort)ident, 0, 0, 0, 0);

    private TSelectClient Client(out TUserInfo slot, string connID = "1", string ip = "1.1.1.1/2.2.2.2")
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O" + connID + "/" + ip + "$");
        slot = c.SelectCharList.OnLineItems(0)!;
        return c;
    }

    /// <summary>
    /// 过掉节流窗口后投递一条命令帧。
    /// <paramref name="account"/> 会写进槽位的 <c>sAccount</c>：原文 :759/:783/:806/:828 都先判
    /// <c>UserInfo.sAccount &lt;&gt; ''</c> 才走 CheckSession，否则直接 OutOfConnect。
    /// </summary>
    private void Send(TSelectClient c, int ident, string arg, uint tick = 100000, string? account = "acct")
    {
        var slot = c.SelectCharList.OnLineItems(0)!;
        if (account != null) slot.sAccount = account;
        slot.dwChrTick = 0;
        DelphiTick.GetTickCount = () => tick;
        c.ExecGateBuffers(UserDataFrame("1", Cmd(ident), arg));
    }

    /// <summary>
    /// 原文 <c>MakeDefaultMsg(wIdent, nRecog: Int64, wParam, wTag, wSeries)</c> 的**第 2 个**实参是 Int64 Recog，
    /// SelectClient.pas 把 nCode / nChrCount 全塞在这里 ⇒ 断言一律读 <c>Msg.Recog</c>。
    /// </summary>
    private static int RCode(SentFrame f) => (int)f.Msg.Recog;

    /// <summary>原文 <c>wParam</c>（SelectClient.pas 只在 SM_NEWCHR_FAIL 里用它传角色数上限）。</summary>
    private static int PMsg(SentFrame f) => f.Msg.Param;

    private static T InvokePrivate<T>(TSelectClient c, string name, params object[] args)
        => (T)typeof(TSelectClient)
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(c, args)!;

    /// <summary>
    /// 构造 <c>DeCodeUserMsg</c> 交给各角色方法的**帧体**（原文 <c>s18</c>）：
    /// 只有 <c>EncodeString(文本)</c>，**不含** 22 字节消息头。反射直调时必须用它。
    /// </summary>
    private static string BodyArg(string argText)
        => System.Text.Encoding.Latin1.GetString(EDcode.EncodeString(argText));

    private static TQueryHumanData Human(string name, int job, int hair, int level, int sex, bool isSelect = false)
        => new TQueryHumanData { HumanName = name, Job = job, Hair = hair, Level = level, Sex = sex, IsSelect = isSelect };

    // =====================================================================================
    // QueryChr（CM_QUERYCHR = 100，节流 200ms）
    // =====================================================================================

    [Fact]
    public void QueryChr_会话通过_回SM_QUERYCHR且Param是角色数Tag是1()
    {
        var c = Client(out var slot);
        HumanDb.Humans.Add(Human("Hero1", 1, 2, 3, 4, isSelect: true));

        Send(c, Grobal2Const.CM_QUERYCHR, "acct/42");

        Assert.Single(Sent);
        Assert.Equal(Grobal2Const.SM_QUERYCHR, Sent[0].Msg.Ident);
        Assert.Equal(520, Sent[0].Msg.Ident);
        Assert.Equal(1, RCode(Sent[0]));
        Assert.Equal(1, Sent[0].Msg.Tag);
        Assert.Equal(0, Sent[0].Msg.Series);
        Assert.Equal("*Hero1/1/2/3/4/", DecodeBody(Sent[0].Body));                // 原文 :1220-1221
    }

    [Fact]
    public void QueryChr_多条角色按Select标记串接()
    {
        var c = Client(out _);
        HumanDb.Humans.Add(Human("Aaaa", 0, 1, 10, 0));
        HumanDb.Humans.Add(Human("Bbbb", 1, 2, 20, 1, isSelect: true));

        Send(c, Grobal2Const.CM_QUERYCHR, "acct/7");

        Assert.Equal("Aaaa/0/1/10/0/Bbbb/1/2/20/1/", DecodeBody(Sent[0].Body).Replace("*", ""));
    }

    [Fact]
    public void QueryChr_会话不通过_回SM_QUERYCHR_FAIL并CloseUser()
    {
        var c = Client(out var slot);
        IdSoc.CheckSessionResult = false;
        slot.sAccount = "acct";

        Send(c, Grobal2Const.CM_QUERYCHR, "acct/42");

        Assert.Single(Sent);
        Assert.Equal(Grobal2Const.SM_QUERYCHR_FAIL, Sent[0].Msg.Ident);
        Assert.Equal(0, RCode(Sent[0]));                                       // nChrCount 停在 0
        Assert.Equal(0, c.SelectCharList.OnLineCount);                            // CloseUser → Finalize
    }

    [Fact]
    public void QueryChr_会话号解析失败时默认负二()
    {
        var c = Client(out var slot);

        Send(c, Grobal2Const.CM_QUERYCHR, "acct/notanumber");

        Assert.Equal(-2, slot.nSessionID);                                        // 原文 :1192 StrToIntDef(..., -2)
        Assert.Equal(-2, IdSoc.CheckSessionCalls[0].SessionID);
    }

    [Fact]
    public void QueryChr_会话号正常解析并写入槽位()
    {
        var c = Client(out var slot);
        Send(c, Grobal2Const.CM_QUERYCHR, "acct/12345");
        Assert.Equal(12345, slot.nSessionID);
        Assert.Equal("acct", slot.sAccount);
        Assert.Equal("1.1.1.1", IdSoc.CheckSessionCalls[0].IP);
    }

    [Fact]
    public void QueryChr_角色数超过上限被截断()
    {
        var c = Client(out _);
        DBShareSeam.g_nCreateChrNameCount = 2;
        HumanDb.Humans.Add(Human("Aaaa", 0, 0, 1, 0));
        HumanDb.Humans.Add(Human("Bbbb", 0, 0, 1, 0));
        HumanDb.Humans.Add(Human("Cccc", 0, 0, 1, 0));

        Send(c, Grobal2Const.CM_QUERYCHR, "acct/1");

        Assert.Equal(2, RCode(Sent[0]));
        Assert.Equal("Aaaa/0/0/1/0/Bbbb/0/0/1/0/", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void QueryChr_无角色时Param为0且体为空()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_QUERYCHR, "acct/1");
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(Sent[0].Body);
    }

    [Fact]
    public void QueryChr_ShowQuryChrLog为真时记日志()
    {
        var c = Client(out _);
        SelectClientGlobals.g_boShowQuryChrLog = 1;
        Send(c, Grobal2Const.CM_QUERYCHR, "acct/1");
        Assert.Contains("查询帐户角色信息:acct", Logs);
    }

    [Fact]
    public void QueryChr_恒返回False_故boChrQueryed永远不可能被置真()
    {
        // ★★ 原文缺陷（:1182-1237：Result 只在 :1190 赋过 False）：
        //     DeCodeUserMsg:742-745 的 `if QueryChr(...) then boChrQueryed := True` **永不成立**。
        var c = Client(out var slot, "1", "1.1.1.1/2.2.2.2");
        HumanDb.Humans.Add(Human("Hero1", 1, 2, 3, 4));

        Send(c, Grobal2Const.CM_QUERYCHR, "acct/42");

        Assert.Equal(Grobal2Const.SM_QUERYCHR, Sent[0].Msg.Ident);                // 业务上"成功了"
        Assert.False(slot.boChrQueryed);                                          // 但标志位没置上
        bool ret = InvokePrivate<bool>(c, "QueryChr", BodyArg("acct/42"), slot);
        Assert.False(ret);
    }

    [Fact]
    public void QueryChr_恒返回False的连带后果_CM_SELCHR的not_boChrQueryed门永远为真()
    {
        var c = Client(out var slot);
        Send(c, Grobal2Const.CM_QUERYCHR, "acct/42");
        Assert.False(slot.boChrQueryed);

        Sent.Clear();
        slot.sAccount = "acct";
        DBShareSeam.g_RouteInfo[0].sSelGateIP = "127.0.0.1";
        DBShareSeam.g_RouteInfo[0].nGateCount = 1;
        DBShareSeam.g_RouteInfo[0].sGameGateIP[0] = "10.0.0.1";
        DBShareSeam.g_RouteInfo[0].nGameGatePort[0] = 7200;
        c.m_sGateaddr = "127.0.0.1";
        DelphiRandom.Next = _ => 0;

        Send(c, Grobal2Const.CM_SELCHR, "acct/Hero1");

        Assert.Equal(Grobal2Const.SM_STARTPLAY, Sent[0].Msg.Ident);               // 没有被 Double send 拦下
        Assert.DoesNotContain(Logs, s => s.StartsWith("Double send _SELCHR", StringComparison.Ordinal));
    }

    // =====================================================================================
    // NewChr（CM_NEWCHR = 101，节流 1000ms）
    // =====================================================================================

    [Fact]
    public void NewChr_建号功能关闭_回FAIL且Param为5()
    {
        var c = Client(out _);
        DBShareSeam.g_boCanCreateHuman = 0;

        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");

        Assert.Single(Sent);
        Assert.Equal(Grobal2Const.SM_NEWCHR_FAIL, Sent[0].Msg.Ident);
        Assert.Equal(5, RCode(Sent[0]));
        Assert.Empty(HumanDb.AddCalls);
    }

    [Fact]
    public void NewChr_成功路径_回SUCCESS且入库参数正确()
    {
        var c = Client(out _);
        HumanDb.HumanCounts["acct"] = 0;                                          // 第一个角色 ⇒ IsSelect = True

        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/3/2/1");

        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
        Assert.Single(HumanDb.AddCalls);
        Assert.Equal(("acct", "Aaaa", true, (byte)1, (byte)2, (byte)3), HumanDb.AddCalls[0]);
        Assert.Equal(1, SelectClientGlobals.g_nCreateHumCount);
    }

    [Fact]
    public void NewChr_非首个角色时IsSelect为假()
    {
        var c = Client(out _);
        HumanDb.HumanCounts["acct"] = 1;

        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");

        Assert.False(HumanDb.AddCalls[0].IsSelect);
    }

    [Fact]
    public void NewChr_名字短于4字节被判非法()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/abc/1/1/1");                        // 3 字节
        Assert.Equal(Grobal2Const.SM_NEWCHR_FAIL, Sent[0].Msg.Ident);
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(HumanDb.AddCalls);
    }

    [Fact]
    public void NewChr_两个汉字是4字节_通过长度校验()
    {
        // ★★ 字节语义关键用例：按 UTF-16 计长只有 2，会误判为"名字过短"；
        //    原文 Length(AnsiString) 是 **GBK 字节数** ⇒ 恰好 4，通过。
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/战士/1/1/1");
        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
        Assert.Equal("战士", HumanDb.AddCalls[0].Name);
    }

    [Fact]
    public void NewChr_非TextChars字节被逐字节删除后再入库()
    {
        // 0x01 不属于 [#32..#255] ⇒ 被 Delete 掉；剩下 6 字节 "abcdef"
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/abc\u0001def/1/1/1");
        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
        Assert.Equal("abcdef", HumanDb.AddCalls[0].Name);
    }

    [Fact]
    public void NewChr_多余字段导致nCode为0()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1/EXTRA");
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(HumanDb.AddCalls);
    }

    [Fact]
    public void NewChr_名字超过14字节被判非法()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/abcdefghijklmno/1/1/1");            // 15 字节
        Assert.Equal(0, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_恰好14字节通过()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/abcdefghijklmn/1/1/1");
        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
    }

    [Fact]
    public void NewChr_CheckDenyChrName为假时nCode为2()
    {
        var c = Client(out _);
        SelectClientDbShareSeam.CheckDenyChrName = _ => false;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(2, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_CheckChrName为假时nCode为0()
    {
        var c = Client(out _);
        SelectClientDbShareSeam.CheckChrName = _ => false;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(0, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_命中非法字符过滤表时nCode为8()
    {
        var c = Client(out _);
        SelectClientDbShareSeam.CheckFilterNewHumanChrName = _ => true;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(8, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_CheckSpecialChar为假且未禁特殊字符时nCode为0()
    {
        var c = Client(out _);
        DBShareSeam.g_boDenyChrName = 0;
        SelectClientDbShareSeam.CheckSpecialChar = _ => false;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aa/a/1/1/1");
        Assert.Equal(0, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_已禁特殊字符时_不调用CheckSpecialChar()
    {
        var c = Client(out _);
        DBShareSeam.g_boDenyChrName = 1;
        bool called = false;
        SelectClientDbShareSeam.CheckSpecialChar = _ => { called = true; return true; };
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.False(called);
        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
    }

    [Fact]
    public void NewChr_CheckSpecialChar收的是GBK文本不是字节串()
    {
        // 原文 sign：CheckSpecialChar(sChrName: WideString) —— 唯一一个收 WideString 的校验函数
        var c = Client(out _);
        string seen = "";
        SelectClientDbShareSeam.CheckSpecialChar = s => { seen = s; return true; };
        Send(c, Grobal2Const.CM_NEWCHR, "acct/战士/1/1/1");
        Assert.Equal("战士", seen);
    }

    [Fact]
    public void NewChr_CheckChrName收的是字节串()
    {
        // 原文 sign：CheckChrName(sChrName: string = AnsiString)，函数体按**字节**比较 #$81..#$FE
        var c = Client(out _);
        string seen = "";
        SelectClientDbShareSeam.CheckChrName = s => { seen = s; return true; };
        Send(c, Grobal2Const.CM_NEWCHR, "acct/战士/1/1/1");
        Assert.Equal(4, seen.Length);                                             // 两个汉字 = 4 个字节
        Assert.Equal(0xD5, (byte)seen[0]);                                        // "战" 的 GBK 首字节
    }

    [Fact]
    public void NewChr_禁止数字名时nCode为6()
    {
        var c = Client(out _);
        DBShareSeam.g_boForbidNumberName = 1;
        SelectClientDbShareSeam.CheckNumberName = _ => true;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaa1/1/1/1");
        Assert.Equal(6, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_禁止全英文名时nCode为7()
    {
        var c = Client(out _);
        DBShareSeam.g_boForbidLetterName = 1;
        SelectClientDbShareSeam.CheckLetterName = _ => true;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(7, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_数字名检查不通过时不落6()
    {
        var c = Client(out _);
        DBShareSeam.g_boForbidNumberName = 1;
        SelectClientDbShareSeam.CheckNumberName = _ => false;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(Grobal2Const.SM_NEWCHR_SUCCESS, Sent[0].Msg.Ident);
    }

    [Fact]
    public void NewChr_HumanDB已有同名时nCode为2()
    {
        var c = Client(out _);
        HumanDb.Ids["Aaaa"] = 77;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(2, RCode(Sent[0]));
        Assert.Empty(HumanDb.AddCalls);
        Assert.Empty(HeroDb.GetIdCalls);                                          // or 短路：命中 HumanDB 就不问 HeroDB
    }

    [Fact]
    public void NewChr_HeroDB已有同名时nCode为2()
    {
        var c = Client(out _);
        HeroDb.Ids["Aaaa"] = 88;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(2, RCode(Sent[0]));
        Assert.Single(HeroDb.GetIdCalls);
    }

    [Fact]
    public void NewChr_角色数达上限时nCode为3()
    {
        var c = Client(out _);
        DBShareSeam.g_nCreateChrNameCount = 5;
        HumanDb.HumanCounts["acct"] = 5;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(3, RCode(Sent[0]));
        Assert.Empty(HumanDb.AddCalls);
    }

    [Fact]
    public void NewChr_入库失败时nCode为2()
    {
        var c = Client(out _);
        HumanDb.AddResult = false;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        Assert.Equal(2, RCode(Sent[0]));
        Assert.Equal(0, SelectClientGlobals.g_nCreateHumCount);                   // 失败时不自增
    }

    [Fact]
    public void NewChr_FAIL的Param是角色数上限()
    {
        var c = Client(out _);
        DBShareSeam.g_nCreateChrNameCount = 17;
        HumanDb.AddResult = false;
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/1/1/1");
        // 原文 MakeDefaultMsg(SM_NEWCHR_FAIL, nCode, g_nCreateChrNameCount, 0, 0) ⇒ 上限落在 wParam
        Assert.Equal(17, PMsg(Sent[0]));
        Assert.Equal(2, RCode(Sent[0]));
    }

    [Fact]
    public void NewChr_性别职业发型按Byte截断()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_NEWCHR, "acct/Aaaa/300/257/-1");
        // 载荷顺序是 账号/角色名/发型/职业/性别（原文 :923-927）
        // StrToIntDef → Integer，再隐式截低 8 位（Delphi {$R-} 语义）
        Assert.Equal((unchecked((byte)-1), unchecked((byte)257), unchecked((byte)300)),
                     (HumanDb.AddCalls[0].Sex, HumanDb.AddCalls[0].Job, HumanDb.AddCalls[0].Hair));
    }

    // =====================================================================================
    // DelChr（CM_DELCHR = 102，节流 1000ms）
    // =====================================================================================

    [Fact]
    public void DelChr_删除功能关闭_回nCode负2()
    {
        var c = Client(out var slot);
        slot.sAccount = "acct";
        DBShareSeam.g_boCanDeleteHuman = 0;
        Send(c, Grobal2Const.CM_DELCHR, "Aaaa");
        Assert.Equal(Grobal2Const.SM_DELCHR_FAIL, Sent[0].Msg.Ident);
        Assert.Equal(-2, RCode(Sent[0]));
        Assert.Empty(HumanDb.DeleteCalls);
    }

    [Fact]
    public void DelChr_等级高于45不可删()
    {
        var c = Client(out var slot);
        slot.sAccount = "acct";
        HumanDb.BaseInfoLevel = 46;
        Send(c, Grobal2Const.CM_DELCHR, "Aaaa");
        Assert.Equal(-3, RCode(Sent[0]));
        Assert.Empty(HumanDb.DeleteCalls);
    }

    [Fact]
    public void DelChr_等级恰为45可以删()
    {
        var c = Client(out var slot);
        slot.sAccount = "acct";
        HumanDb.BaseInfoLevel = 45;
        Send(c, Grobal2Const.CM_DELCHR, "Aaaa");
        Assert.Equal(Grobal2Const.SM_DELCHR_SUCCESS, Sent[0].Msg.Ident);
        Assert.Equal(("acct", "Aaaa"), HumanDb.DeleteCalls[0]);
    }

    [Fact]
    public void DelChr_取不到基础信息时nCode停0()
    {
        var c = Client(out var slot);
        slot.sAccount = "acct";
        HumanDb.GetBaseInfoResult = false;
        Send(c, Grobal2Const.CM_DELCHR, "Aaaa");
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(HumanDb.DeleteCalls);
    }

    [Fact]
    public void DelChr_删除失败时nCode停0()
    {
        var c = Client(out var slot);
        slot.sAccount = "acct";
        HumanDb.DeleteResult = false;
        Send(c, Grobal2Const.CM_DELCHR, "Aaaa");
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Single(HumanDb.DeleteCalls);
    }

    [Fact]
    public void DelChr_不使用消息里的账号_而用槽位上的账号()
    {
        var c = Client(out var slot);
        Send(c, Grobal2Const.CM_DELCHR, "Aaaa", account: "slotacct");
        Assert.Equal("slotacct", HumanDb.DeleteCalls[0].Account);
    }

    // =====================================================================================
    // RandomName（CM_RANDOMNAME = 106，无节流）
    // =====================================================================================

    private TSelectClient RandomNameClient(uint tick = 5000)
    {
        var c = Client(out var slot);
        slot.sAccount = "acct";
        DelphiTick.GetTickCount = () => tick;
        return c;
    }

    [Fact]
    public void RandomName_取首个名字段与首个性氏段()
    {
        var c = RandomNameClient();
        SelectClientGlobals.g_FirstName.Add("Abc");
        SelectClientGlobals.g_FirstName.Add("Def");
        SelectClientGlobals.g_LastName.Add("Xyz");
        SelectClientGlobals.g_LastName.Add("Uvw");
        SelectClientRandom.NextDouble = () => 0.0;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        Assert.Equal(Grobal2Const.SM_RANDOMNAME, Sent[0].Msg.Ident);
        Assert.Equal("AbcXyz", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void RandomName_最后一项永远取不到_因为参数是Count减1()
    {
        var c = RandomNameClient();
        SelectClientGlobals.g_FirstName.Add("AAA");
        SelectClientGlobals.g_FirstName.Add("BBB");
        SelectClientGlobals.g_FirstName.Add("ZZZ");                              // 永远取不到
        SelectClientGlobals.g_LastName.Add("ll");
        SelectClientRandom.NextDouble = () => 0.999999;                           // 逼近上界 [0,1)

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        // Random(2) 的取值域是 {0,1} ⇒ 取到 BBB（索引 1），永远到不了 ZZZ（索引 2）
        Assert.Equal("BBBll", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void RandomName_名单只有一项时取到唯一那项()
    {
        var c = RandomNameClient();
        SelectClientGlobals.g_FirstName.Add("Only");
        SelectClientGlobals.g_LastName.Add("One");
        SelectClientRandom.NextDouble = () => 0.5;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        Assert.Equal("OnlyOne", DecodeBody(Sent[0].Body));                        // Random(0) = 0
    }

    [Fact]
    public void RandomName_名单为空时抛异常_与原文空表索引等价()
    {
        // 原文：Random(0 - 1) = Trunc(Random * -1) = 0 ⇒ g_FirstName.Strings[0] 在空 TStringList 上抛 EStringListError
        var c = RandomNameClient();
        SelectClientRandom.NextDouble = () => 0.5;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME))));
    }

    [Fact]
    public void RandomName_不刷新dwChrTick()
    {
        var c = RandomNameClient(5000);
        var slot = c.SelectCharList.OnLineItems(0)!;
        slot.dwChrTick = 1234;
        SelectClientGlobals.g_FirstName.Add("Abc");
        SelectClientGlobals.g_LastName.Add("Def");
        SelectClientRandom.NextDouble = () => 0.0;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_RANDOMNAME)));

        Assert.Equal(1234u, slot.dwChrTick);                                      // 原文 :756-758 被注释掉
    }

    // =====================================================================================
    // SelectChr（CM_SELCHR = 103，无时间节流、只有 boChrQueryed 门）
    // =====================================================================================

    private TSelectClient SelectChrClient(out TUserInfo slot)
    {
        var c = Client(out slot);
        slot.sAccount = "acct";
        slot.nSessionID = 99;
        c.m_sGateaddr = "127.0.0.1";
        DBShareSeam.g_RouteInfo[0].sSelGateIP = "127.0.0.1";
        DBShareSeam.g_RouteInfo[0].nGateCount = 1;
        DBShareSeam.g_RouteInfo[0].sGameGateIP[0] = "10.0.0.1";
        DBShareSeam.g_RouteInfo[0].nGameGatePort[0] = 7200;
        DelphiRandom.Next = _ => 0;
        return c;
    }

    [Fact]
    public void SelectChr_选角失败_回SM_STARTFAIL且不碰会话()
    {
        var c = SelectChrClient(out _);
        HumanDb.SelectResult = false;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal(Grobal2Const.SM_STARTFAIL, Sent[0].Msg.Ident);
        Assert.Empty(IdSoc.PlayCalls);
    }

    [Fact]
    public void SelectChr_成功_回SM_STARTPLAY加路由串并置会话为游玩()
    {
        var c = SelectChrClient(out var slot);

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal(Grobal2Const.SM_STARTPLAY, Sent[0].Msg.Ident);
        Assert.Equal("10.0.0.1/7200", DecodeBody(Sent[0].Body));
        Assert.Equal(new[] { 99 }, IdSoc.PlayCalls.ToArray());
        Assert.True(slot.boChrSelected);
        Assert.Equal(("acct", "Hero1"), HumanDb.SelectCalls[0]);
    }

    [Fact]
    public void SelectChr_动态IP模式改用槽位上的GateIP()
    {
        var c = SelectChrClient(out var slot);
        SelectClientGlobals.g_boDynamicIPMode = 1;
        slot.sGateIPaddr = "192.168.9.9";

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal("192.168.9.9/7200", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void SelectChr_未命中路由时返回空IP与端口0()
    {
        var c = SelectChrClient(out _);
        DBShareSeam.g_RouteInfo[0].sSelGateIP = "1.2.3.4";                         // 不再匹配 m_sGateaddr
        c.m_sGateaddr = "127.0.0.1";

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        // 原文把命中不了的结果照样发出去：EncodeString("/0")
        Assert.Equal(Grobal2Const.SM_STARTPLAY, Sent[0].Msg.Ident);
        Assert.Equal("/0", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void SelectChr_主动网关模式_走GateActiveRouteIP接缝()
    {
        var c = SelectChrClient(out _);
        DBShareSeam.g_boUseActiveRunGage = 1;
        int port;
        SelectClientDbShareSeam.GateActiveRouteIP = (string ip, out int p) => { p = 7300; return "10.0.0.9"; };
        port = 0;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal("10.0.0.9/7300", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void SelectChr_主动网关模式_路由为空时回SM_STARTFAIL()
    {
        var c = SelectChrClient(out _);
        DBShareSeam.g_boUseActiveRunGage = 1;
        SelectClientDbShareSeam.GateActiveRouteIP = (string ip, out int p) => { p = 0; return ""; };

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal(Grobal2Const.SM_STARTFAIL, Sent[0].Msg.Ident);
        Assert.Empty(IdSoc.PlayCalls);
    }

    [Fact]
    public void SelectChr_主动网关加动态IP_CheckActiveRunGate失败则回SM_STARTFAIL()
    {
        var c = SelectChrClient(out var slot);
        DBShareSeam.g_boUseActiveRunGage = 1;
        SelectClientGlobals.g_boDynamicIPMode = 1;
        slot.sGateIPaddr = "192.168.9.9";
        SelectClientDbShareSeam.GateActiveRouteIP = (string ip, out int p) => { p = 7300; return "10.0.0.9"; };
        SelectClientDbShareSeam.CheckActiveRunGate = (ip, p) => false;

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal(Grobal2Const.SM_STARTFAIL, Sent[0].Msg.Ident);
    }

    [Fact]
    public void SelectChr_主动网关加动态IP_CheckActiveRunGate通过则用该IP()
    {
        var c = SelectChrClient(out var slot);
        DBShareSeam.g_boUseActiveRunGage = 1;
        SelectClientGlobals.g_boDynamicIPMode = 1;
        slot.sGateIPaddr = "192.168.9.9";
        SelectClientDbShareSeam.GateActiveRouteIP = (string ip, out int p) => { p = 7300; return "10.0.0.9"; };
        SelectClientDbShareSeam.CheckActiveRunGate = (ip, p) => ip == "192.168.9.9";

        c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1"));

        Assert.Equal("192.168.9.9/7300", DecodeBody(Sent[0].Body));
    }

    [Fact]
    public void SelectChr_主动网关接缝未接线时抛异常而不是静默()
    {
        var c = SelectChrClient(out _);
        DBShareSeam.g_boUseActiveRunGage = 1;
        Assert.Throws<NotSupportedException>(
            () => c.ExecGateBuffers(UserDataFrame("1", Cmd(Grobal2Const.CM_SELCHR), "acct/Hero1")));
    }

    // =====================================================================================
    // QueryDelChr（CM_QUERYDELCHR = 105，节流 200ms）
    // =====================================================================================

    [Fact]
    public void QueryDelChr_有已删角色时Result为真且逐条串接()
    {
        var c = Client(out _);
        HumanDb.DeleteHumans.Add(Human("Gone1", 1, 2, 30, 1));
        HumanDb.DeleteHumans.Add(Human("Gone2", 0, 0, 5, 0));

        Send(c, Grobal2Const.CM_QUERYDELCHR, "acct");

        Assert.Equal(Grobal2Const.SM_QUERYDELCHR, Sent[0].Msg.Ident);
        Assert.Equal(2, RCode(Sent[0]));
        // S 里累积的是**已编码**的 21 字节 TDeleteHumanInfo + '/'（原文不再整体重编码）
        Assert.Equal(2 * (EDcode.GetEncodeSize(21) + 1), Sent[0].Body.Length);
        Assert.Equal(2, Sent[0].Body.Count(b => b == (byte)'/'));
        // 逐段 6-bit 解回结构体
        var h0 = StructBytes.FromBytes<TDeleteHumanInfo>(EDcode.DecodeBuffer(Sent[0].Body, EDcode.GetEncodeSize(21)));
        Assert.Equal("Gone1", h0.ChrName);
        Assert.Equal(30, h0.nLevel);
    }

    [Fact]
    public void QueryDelChr_最多取10条()
    {
        var c = Client(out _);
        for (int i = 0; i < 13; i++) HumanDb.DeleteHumans.Add(Human("Gone" + i, 0, 0, 1, 0));

        Send(c, Grobal2Const.CM_QUERYDELCHR, "acct");

        Assert.Equal(10, RCode(Sent[0]));                                      // 原文 :1032 `if nChrCount >= 10 then break`
    }

    [Fact]
    public void QueryDelChr_无记录时Param为0()
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_QUERYDELCHR, "acct");
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(Sent[0].Body);
        Assert.Single(HumanDb.QueryDeleteHumansCalls);                            // 库**被查了**，只是返回 0 ⇒ 不遍历
    }

    [Fact]
    public void QueryDelChr_入参为空时不查库直接回包()
    {
        var c = Client(out _);
        HumanDb.DeleteHumans.Add(Human("Gone1", 1, 2, 30, 1));

        Send(c, Grobal2Const.CM_QUERYDELCHR, "");                                  // 编码体为空

        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(HumanDb.QueryDeleteHumansCalls);                             // 原文 :1014 `if Length(sData) > 0`
    }

    [Fact]
    public void QueryDelChr_每条记录按21字节结构编码()
    {
        var c = Client(out _);
        HumanDb.DeleteHumans.Add(Human("Gone1", 3, 4, 55, 1));

        Send(c, Grobal2Const.CM_QUERYDELCHR, "acct");

        byte[] decoded = EDcode.DecodeString(Sent[0].Body.AsSpan(0, Sent[0].Body.Length - 1).ToArray());
        var info = StructBytes.FromBytes<TDeleteHumanInfo>(decoded);
        Assert.Equal("Gone1", info.ChrName);
        Assert.Equal(55, info.nLevel);
        Assert.Equal(3, info.btJob);
        Assert.Equal(1, info.btSex);
    }

    [Fact]
    public void QueryDelChr_恒返回真当库有记录()
    {
        var c = Client(out _);
        HumanDb.DeleteHumans.Add(Human("Gone1", 1, 2, 30, 1));
        bool ret = InvokePrivate<bool>(c, "QueryDelChr",
            BodyArg("acct"), c.SelectCharList.OnLineItems(0)!);
        Assert.True(ret);
    }

    // =====================================================================================
    // GetBackDelChr（CM_GETBACKDELCHR = 3006，节流 200ms）
    // =====================================================================================

    [Fact]
    public void GetBackDelChr_找回功能关闭_nCode为负5()
    {
        var c = Client(out _);
        DBShareSeam.g_boCanGetBackDeleteHuman = 0;

        Send(c, Grobal2Const.CM_GETBACKDELCHR, "acct/Aaaa");

        Assert.Equal(Grobal2Const.SM_GETBAKCHAR_FAIL, Sent[0].Msg.Ident);
        Assert.Equal(-5, RCode(Sent[0]));
        Assert.Equal(DBShareSeam.g_nCreateChrNameCount, Sent[0].Msg.Series);      // 原文第 5 个实参 = wSeries
    }

    [Fact]
    public void GetBackDelChr_角色已满_nCode为负1()
    {
        var c = Client(out _);
        DBShareSeam.g_nCreateChrNameCount = 3;
        HumanDb.HumanCounts["acct"] = 3;

        Send(c, Grobal2Const.CM_GETBACKDELCHR, "acct/Aaaa");

        Assert.Equal(-1, RCode(Sent[0]));
        Assert.Empty(HumanDb.DeleteRestoreCalls);
    }

    [Fact]
    public void GetBackDelChr_成功_回SUCCESS且nCode为1()
    {
        var c = Client(out _);
        HumanDb.DeleteRestoreResult = true;

        Send(c, Grobal2Const.CM_GETBACKDELCHR, "acct/Aaaa");

        Assert.Equal(Grobal2Const.SM_GETBAKCHAR_SUCCESS, Sent[0].Msg.Ident);
        Assert.Equal(1, RCode(Sent[0]));
        Assert.Equal(("acct", "Aaaa"), HumanDb.DeleteRestoreCalls[0]);
    }

    [Fact]
    public void GetBackDelChr_找回失败_nCode为负2()
    {
        var c = Client(out _);
        HumanDb.DeleteRestoreResult = false;

        Send(c, Grobal2Const.CM_GETBACKDELCHR, "acct/Aaaa");

        Assert.Equal(Grobal2Const.SM_GETBAKCHAR_FAIL, Sent[0].Msg.Ident);
        Assert.Equal(-2, RCode(Sent[0]));
    }

    [Theory]
    [InlineData("")]
    [InlineData("acct")]
    [InlineData("acct/")]
    [InlineData("/Aaaa")]
    public void GetBackDelChr_参数不全时nCode为0(string arg)
    {
        var c = Client(out _);
        Send(c, Grobal2Const.CM_GETBACKDELCHR, arg);
        Assert.Equal(0, RCode(Sent[0]));
        Assert.Empty(HumanDb.DeleteRestoreCalls);
    }

    [Fact]
    public void GetBackDelChr_恒返回False_即使成功也是如此()
    {
        // ★★ 原文缺陷（:1043-1081：Result 只在 :1048 赋过 False，成功路径 :1064 只写 nCode）。
        //    调用点 DeCodeUserMsg:869 丢弃返回值 ⇒ 目前无行为差异；一旦有人去用返回值就会踩到。
        var c = Client(out _);
        HumanDb.DeleteRestoreResult = true;
        var slot = c.SelectCharList.OnLineItems(0)!;

        bool ret = InvokePrivate<bool>(c, "GetBackDelChr", BodyArg("acct/Aaaa"), slot);

        Assert.False(ret);
        Assert.Equal(Grobal2Const.SM_GETBAKCHAR_SUCCESS, Sent[0].Msg.Ident);      // 但协议上确实回了"成功"
    }

    [Fact]
    public void GetBackDelChr_失败时同样恒返回False()
    {
        var c = Client(out _);
        HumanDb.DeleteRestoreResult = false;
        var slot = c.SelectCharList.OnLineItems(0)!;
        bool ret = InvokePrivate<bool>(c, "GetBackDelChr", BodyArg("acct/Aaaa"), slot);
        Assert.False(ret);
    }
}
