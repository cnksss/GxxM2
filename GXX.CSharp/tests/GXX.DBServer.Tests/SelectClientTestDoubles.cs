using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

// ============================================================================================
// p7-db-selectclient 车道的测试替身：**绝不连真库、绝不碰真 socket**。
//   · FakeFrmIDSoc      → IDSocCliSeam.FrmIDSoc
//   · FakeSelectHumanDB → SelectClientRoleDbSeam.HumanDB（THumanDBBase 的内存实现）
//   · FakeSelectHeroDB  → SelectClientRoleDbSeam.HeroDB （THeroDBBase 的内存实现）
//   · SelectClientTestBase：在 TempDirTest（TestReset.All）之上，额外复位本车道新增的接缝。
// ============================================================================================

/// <summary>内存版 TFrmIDSoc（IDSocCli.pas）。记录每一次调用，并允许钉死 CheckSession 结果。</summary>
public sealed class FakeFrmIDSoc : ITFrmIDSoc
{
    public bool CheckSessionResult = true;
    public bool SessionStatus = true;

    public readonly List<(string Account, string IP, int SessionID)> CheckSessionCalls = new();
    public readonly List<int> NoPlayCalls = new();
    public readonly List<int> PlayCalls = new();
    public readonly List<int> StatusCalls = new();
    public readonly List<(ushort Ident, string Msg)> SocketMsgs = new();
    public readonly List<(string Account, int SessionID)> ClosedSessions = new();

    public bool CheckSession(string sAccount, string sIPaddr, int nSessionID)
    {
        CheckSessionCalls.Add((sAccount, sIPaddr, nSessionID));
        return CheckSessionResult;
    }

    public void SetGlobaSessionNoPlay(int nSessionID) => NoPlayCalls.Add(nSessionID);

    public void SetGlobaSessionPlay(int nSessionID) => PlayCalls.Add(nSessionID);

    public bool GetGlobaSessionStatus(int nSessionID)
    {
        StatusCalls.Add(nSessionID);
        return SessionStatus;
    }

    public void SendSocketMsg(ushort wIdent, string sMsg) => SocketMsgs.Add((wIdent, sMsg));

    public void CloseSession(string sAccount, int nSessionID) => ClosedSessions.Add((sAccount, nSessionID));
}

/// <summary>
/// 内存版 THumanDB（RoleDB.pas:117-209）：只实现 SelectClient.pas 真正会调到的那几个 Do*，
/// 其余按"永不命中"实现（返回初值），并在被调用时记一笔以便"未预期调用"断言。
/// </summary>
public sealed class FakeSelectHumanDB : THumanDBBase
{
    /// <summary>角色名 → 编号（GetID 用）。</summary>
    public readonly Dictionary<string, int> Ids = new();

    /// <summary>账号 → 角色数（GetHumanCount 用）。</summary>
    public readonly Dictionary<string, int> HumanCounts = new();

    /// <summary>QueryHumans 的结果集。</summary>
    public readonly List<TQueryHumanData> Humans = new();

    /// <summary>QueryDeleteHumans 的结果集。</summary>
    public readonly List<TQueryHumanData> DeleteHumans = new();

    public bool GetBaseInfoResult = true;
    public int BaseInfoSex, BaseInfoJob, BaseInfoLevel, BaseInfoLastLogin;

    public bool AddResult = true;
    public bool DeleteResult = true;
    public bool DeleteRestoreResult = true;

    public readonly List<(string Account, string Name, bool IsSelect, byte Sex, byte Job, byte Hair)> AddCalls = new();
    public readonly List<(string Account, string Name)> DeleteCalls = new();
    public readonly List<(string Account, string Name)> DeleteRestoreCalls = new();
    public readonly List<(string Account, string Name)> SelectCalls = new();
    public readonly List<string> GetIdCalls = new();
    public readonly List<string> GetHumanCountCalls = new();
    public readonly List<string> QueryHumansCalls = new();
    public readonly List<string> QueryDeleteHumansCalls = new();

    public int LockCount, UnLockCount;

    protected override void OwnerLock() => LockCount++;
    protected override void OwnerUnLock() => UnLockCount++;

    protected override int DoGetID(string HumanName)
    {
        GetIdCalls.Add(HumanName);
        return Ids.TryGetValue(HumanName, out int id) ? id : RoleDbConst.NO_ID;
    }

    protected override int DoGetHumanCount(string Account)
    {
        GetHumanCountCalls.Add(Account);
        return HumanCounts.TryGetValue(Account, out int n) ? n : 0;
    }

    protected override bool DoGetBaseInfo(string HumanName, out int Sex, out int Job, out int Level, out int LastLogin)
    {
        Sex = BaseInfoSex; Job = BaseInfoJob; Level = BaseInfoLevel; LastLogin = BaseInfoLastLogin;
        return GetBaseInfoResult;
    }

    protected override int DoQueryHumans(string Account, TQueryHumanList HumanList)
    {
        QueryHumansCalls.Add(Account);
        foreach (var h in Humans) HumanList.Add(h);
        return Humans.Count;
    }

    protected override int DoQueryDeleteHumans(string Account, TQueryHumanList HumanList)
    {
        QueryDeleteHumansCalls.Add(Account);
        foreach (var h in DeleteHumans) HumanList.Add(h);
        return DeleteHumans.Count;
    }

    protected override bool DoSelect(string Account, string HumanName)
    {
        SelectCalls.Add((Account, HumanName));
        return SelectResult;
    }

    protected override bool DoAdd(string Account, string HumanName, bool IsSelect, byte Sex, byte Job, byte Hair)
    {
        AddCalls.Add((Account, HumanName, IsSelect, Sex, Job, Hair));
        return AddResult;
    }

    protected override bool DoDelete(string Account, string HumanName)
    {
        DeleteCalls.Add((Account, HumanName));
        return DeleteResult;
    }

    protected override bool DoDeleteRestore(string Account, string HumanName)
    {
        DeleteRestoreCalls.Add((Account, HumanName));
        return DeleteRestoreResult;
    }

    /// <summary>DoSelect 的返回值（原文 g_RoleDB.HumanDB.Select）。</summary>
    public bool SelectResult = true;

    // ---- 以下 Do* 在 SelectClient.pas 里**不会被调用**；被调到就说明接线错了 ----
    protected override bool DoCheckHumanExists(string Account, string HumanName) => throw new InvalidOperationException("SelectClient 不应调用 CheckHumanExists");
    protected override string DoGetOtherHumanName(string Account, string HumanName) => throw new InvalidOperationException("SelectClient 不应调用 GetOtherHumanName");
    protected override bool DoGetHumanHeroName(string Account, string HumanName, out string HeroName, out string DeputyHeroName) => throw new InvalidOperationException("SelectClient 不应调用 GetHumanHeroName");
    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("SelectClient 不应调用 SearchByAccount");
    protected override int DoSearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("SelectClient 不应调用 SearchByName");
    protected override int DoSearchByLevel(int LimitCount, int MinLevel, TSerarchRoleList RoleList) => throw new InvalidOperationException("SelectClient 不应调用 SearchByLevel");
    protected override int DoGetMobileNumbers(bool OnlyBindMobile, List<string> MobileNumberList) => throw new InvalidOperationException("SelectClient 不应调用 GetMobileNumbers");
    protected override bool DoGet(string Account, string HumanName, ref THumData HumData, out int HumanID) => throw new InvalidOperationException("SelectClient 不应调用 Get");
    protected override bool DoSetEnabled(string Account, string HumanName, int Enabled) => throw new InvalidOperationException("SelectClient 不应调用 SetEnabled");
    protected override bool DoErase(string Account, string HumanName) => throw new InvalidOperationException("SelectClient 不应调用 Erase");
    protected override bool DoRecordLoginTime(string Account, string HumanName) => throw new InvalidOperationException("SelectClient 不应调用 RecordLoginTime");
    protected override bool DoSave(int HumanID, ref THumData HumData) => throw new InvalidOperationException("SelectClient 不应调用 Save");
    protected override bool DoRename(string Account, string HumanName, int HumanID, string NewName) => throw new InvalidOperationException("SelectClient 不应调用 Rename");
    protected override bool DoChangedGold(string HumanName, TDBChangeGoldType ChangeType, int ChangedValue, out uint ResultValue) => throw new InvalidOperationException("SelectClient 不应调用 ChangedGold");
    protected override bool DoChangedCustomMoney(int HumanID, string CustomMoneyName, int ChangedValue, out uint ResultValue) => throw new InvalidOperationException("SelectClient 不应调用 ChangedCustomMoney");
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount, TRoleRankList HumanRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList, TRoleRankList TaoistRankList, TRoleRankList MasterRankList) => throw new InvalidOperationException("SelectClient 不应调用 GetRankData");
    protected override bool DoBuyPlayer(string sSellAccount, string sSellHumanName, string sBuyAccount, string sBuyHumanName) => throw new InvalidOperationException("SelectClient 不应调用 BuyPlayer");
}

/// <summary>内存版 THeroDB（RoleDB.pas:211-261）：SelectClient 只用它的 <c>GetID</c>（重名判定）。</summary>
public sealed class FakeSelectHeroDB : THeroDBBase
{
    public readonly Dictionary<string, int> Ids = new();
    public readonly List<string> GetIdCalls = new();

    protected override void OwnerLock() { }
    protected override void OwnerUnLock() { }

    protected override int DoGetID(string HeroName)
    {
        GetIdCalls.Add(HeroName);
        return Ids.TryGetValue(HeroName, out int id) ? id : RoleDbConst.NO_ID;
    }

    protected override int DoSearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.SearchByAccount");
    protected override int DoSearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.SearchByName");
    protected override bool DoGet(string HeroName, ref THeroData HeroData, out int HeroID) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.Get");
    protected override bool DoAdd(string Account, string HumanName, int HumanID, string HeroName, byte Sex, byte Job, byte Hair, bool IsDeputyHero) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.Add");
    protected override bool DoErase(string HeroName) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.Erase");
    protected override bool DoSave(int HeroID, ref THeroData HeroData) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.Save");
    protected override bool DoRename(int HeroID, string HeroName, string NewName) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.Rename");
    protected override bool DoAssess(int HeroID, string HeroName, string DeputyHeroName) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.Assess");
    protected override void DoGetRankData(uint MinLevel, uint MaxLevel, uint TopCount, TRoleRankList HeroRankList, TRoleRankList WarriorRankList, TRoleRankList WizardRankList, TRoleRankList TaoistRankList) => throw new InvalidOperationException("SelectClient 不应调用 HeroDB.GetRankData");
}

/// <summary>一条被捕获的出站帧（已按 <c>%&lt;sid&gt;/#…!$</c> 拆好）。</summary>
public sealed class SentFrame
{
    public string SessionID = "";
    /// <summary>完整原始字节（含前后缀），用于逐字节断言。</summary>
    public byte[] Raw = Array.Empty<byte>();
    /// <summary>是否匹配 <c>%&lt;sid&gt;/#…!$</c>（心跳 <c>%++$</c> 等为 false）。</summary>
    public bool IsUserSocket;
    /// <summary>载荷 = 22 字节编码头 + 编码体。</summary>
    public byte[] Payload = Array.Empty<byte>();
    public TDefaultMessage Msg;
    /// <summary>头部之后的编码体（原文 <c>s18</c>）。</summary>
    public byte[] Body = Array.Empty<byte>();

    /// <summary>原文意义的 AnsiString 文本（latin-1 逐字节）。</summary>
    public string RawAnsi => System.Text.Encoding.Latin1.GetString(Raw);
}

/// <summary>
/// SelectClient 车道的测试基类：TempDirTest（已做 TestReset.All）+ 本车道新增接缝的复位。
/// </summary>
public abstract class SelectClientTestBase : TempDirTest
{
    protected readonly List<SentFrame> Sent = new();
    protected readonly List<string> Logs = new();
    protected readonly FakeFrmIDSoc IdSoc = new();
    protected readonly FakeSelectHumanDB HumanDb = new();
    protected readonly FakeSelectHeroDB HeroDb = new();

    protected SelectClientTestBase()
    {
        TSelectClient.ResetSeams();
        SelectClientModuleSeam.Reset();
        SelectClientDbShareSeam.Reset();
        SelectClientRoleDbSeam.Reset();
        SelectClientGlobals.Reset();
        SelectClientRandom.Reset();
        IDSocCliSeam.Reset();

        TSelectClient.SendTextSink = (_, buf) => Sent.Add(ParseFrame(buf));
        TSelectClient.RemoteAddressSink = _ => "10.1.2.3";

        IDSocCliSeam.FrmIDSoc = IdSoc;
        SelectClientRoleDbSeam.HumanDB = HumanDb;
        SelectClientRoleDbSeam.HeroDB = HeroDb;

        RoleDbSeam.MainOutMessage = s => Logs.Add(s);

        // 默认放行全部人物名校验接缝（各用例按需覆盖）
        SelectClientDbShareSeam.CheckChrName = _ => true;
        SelectClientDbShareSeam.CheckSpecialChar = _ => true;
        SelectClientDbShareSeam.CheckDenyChrName = _ => true;
        SelectClientDbShareSeam.CheckNumberName = _ => false;
        SelectClientDbShareSeam.CheckLetterName = _ => false;
        SelectClientDbShareSeam.CheckFilterNewHumanChrName = _ => false;
    }

    /// <summary>把 <c>%&lt;sid&gt;/#&lt;payload&gt;!$</c> 拆成结构化帧。</summary>
    protected static SentFrame ParseFrame(byte[] raw)
    {
        var f = new SentFrame { Raw = raw };
        int start = -1;
        for (int i = 0; i + 1 < raw.Length; i++)
        {
            if (raw[i] == (byte)'/' && raw[i + 1] == (byte)'#') { start = i + 2; break; }
        }
        int end = raw.Length;
        for (int i = raw.Length - 2; i >= 0; i--)
        {
            if (raw[i] == (byte)'!' && raw[i + 1] == (byte)'$') { end = i; break; }
        }
        if (start < 0)
        {
            f.SessionID = "";
            return f;                                                             // 非 %<sid>/#…!$ 形态（如 %++$）
        }
        f.IsUserSocket = true;
        f.SessionID = System.Text.Encoding.Latin1.GetString(raw, 1, Math.Max(0, start - 3));
        int len = Math.Max(0, end - start);
        f.Payload = new byte[len];
        Array.Copy(raw, start, f.Payload, 0, len);
        if (len >= Grobal2Const.DEF_BLOCK_SIZE)
        {
            byte[] head = new byte[Grobal2Const.DEF_BLOCK_SIZE];
            Array.Copy(f.Payload, 0, head, 0, Grobal2Const.DEF_BLOCK_SIZE);
            f.Msg = EDcode.DecodeMessage(head);
            f.Body = new byte[len - Grobal2Const.DEF_BLOCK_SIZE];
            Array.Copy(f.Payload, Grobal2Const.DEF_BLOCK_SIZE, f.Body, 0, f.Body.Length);
        }
        return f;
    }

    /// <summary>构造一条 <c>%&lt;cmd&gt;…$</c> 收包文本（latin-1 字节串）。</summary>
    protected static string GateFrame(string cmd, string body) => "%" + cmd + body + "$";

    /// <summary>
    /// 构造一帧用户数据的**编码载荷文本**（latin-1 逐字节），即 <c>%A&lt;connID&gt;/#1</c> 之后、
    /// <c>!</c> 之前那一段：22 字节编码头 + 可选的 <c>EncodeString(参数)</c>。
    ///
    /// ★ <c>#</c> 之后那个 <c>1</c> 不是装饰：SelGate 侧原文是
    ///   <c>StrFmt(@pszBuf[1], 'A%d/#1%s!$', [Socket, PAnsiChar(Addr)])</c>（ClientSession.pas:253-254），
    ///   而 DBServer 的 <c>ProcessUserMsg:650</c> 正是用 <c>Copy(s10, 2, Length(s10) - 1)</c> 把它剥掉。
    /// </summary>
    protected static string EncodedPayload(TDefaultMessage msg, string nameArg = "")
        => System.Text.Encoding.Latin1.GetString(nameArg == ""
            ? EDcode.EncodeMessage(msg)
            : Concat(EDcode.EncodeMessage(msg), EDcode.EncodeString(nameArg)));

    /// <summary>构造一帧用户数据：<c>%A&lt;connID&gt;/#1&lt;encoded&gt;!$</c>。</summary>
    protected static string UserDataFrame(string connID, TDefaultMessage msg, string nameArg = "")
        => "%A" + connID + "/#1" + EncodedPayload(msg, nameArg) + "!$";

    protected static byte[] Concat(byte[] a, byte[] b)
    {
        byte[] r = new byte[a.Length + b.Length];
        Array.Copy(a, 0, r, 0, a.Length);
        Array.Copy(b, 0, r, a.Length, b.Length);
        return r;
    }

    /// <summary>把编码体解回 GBK 文本（原文 EncodeString 的逆）。</summary>
    protected static string DecodeBody(byte[] body) => System.Text.Encoding.GetEncoding(936).GetString(EDcode.DecodeString(body));

    /// <summary>接线一个已接纳的槽位（等价于收到一条 <c>%O</c>）。</summary>
    protected TSelectClient MakeClientWithSlot(string connID, out TUserInfo slot, string ip = "1.2.3.4/5.6.7.8")
    {
        var c = new TSelectClient();
        c.ExecGateBuffers("%O" + connID + "/" + ip + "$");
        slot = c.SelectCharList.OnLineItems(0)!;
        return c;
    }

    /// <summary>
    /// 把槽 <paramref name="index"/> 放进 OnLineList（等价于"先被 Add 选中过"）。
    /// 做法：① 把 [0, index) 的 Socket 都置为已占用；② Add 一次（应得 index）；③ 再占用 index。
    /// 第 ③ 步模拟 OpenUser:688-699 —— 原文 Add 本身**不写** Socket，不补这一步就无法连续 AddAt。
    /// </summary>
    protected static void AddAt(TSelectChar t, int index)
    {
        var c = new TSelectClient();
        for (int i = 0; i < index; i++) t.Items(i)!.Socket = c;
        int got = t.Add();
        Assert.Equal(index, got);
        t.Items(index)!.Socket = c;
    }
}
