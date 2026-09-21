using System;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// Source\DBServer\SelectClient.pas → SelectClient.cs
//
// 【编译开关 DBSUSETHREAD = 0】（DBShare.pas:19）
//   ⇒ SelectClient.pas:55  TSelectClient = class(TServerClientWinSocket)   ← {$ELSE} 分支生效
//   ⇒ 生效的构造是 :281-294 `Create(Socket: TSocket; ServerWinSocket: TServerWinSocket)`
//   ⇒ **未编译**：:263-278 的 Create(ASocket)、:303-348 的 Close/ClientExecute（{$IF DBSUSETHREAD = 1}）
//   ⇒ SendKeepAlivePacket/SendKickUser/SendUserSocket 走 {$ELSE}（直接 SendText，无 Connected 判定）
//   ⇒ ExecGateBuffers 的 'S' 分支取 `Self.RemoteAddress`
//   本文件按"生效分支"1:1 落地，未编译分支以注释保留（见各方法尾部）。
//
// 【接缝总览】见 SelectClient.Seams.cs；槽位表见 SelectClient.CharTable.cs。
// ============================================================================================

/// <summary>
/// SelectClient.pas:55-96 `TSelectClient`（选人会话层：一个 SelGate 连接 = 一个实例）。
///
/// ★ 原文 `TSelectClient` **本身就是 socket 对象**（VCL 的 OnGetSocket 造它），
///   所以 `OpenUser:697` 写的是 `UserInfo.Socket := Self`。
///   托管侧保留这一形状：<see cref="TSelectClient"/> 继承接缝基类 <see cref="TServerClientWinSocket"/>，
///   并把它唯一的两个真实出口（SendText / RemoteAddress）实现为可注入接缝。
/// </summary>
public class TSelectClient : TServerClientWinSocket
{
    /// <summary>SelectClient.pas:56 `m_dwKeepAliveTick: LongWord;`</summary>
    public uint m_dwKeepAliveTick;

    /// <summary>SelectClient.pas:57 `m_sReceiveText: string;`（AnsiString ⇒ latin-1 字节串）。</summary>
    public string m_sReceiveText = "";

    /// <summary>SelectClient.pas:58 `m_sGateaddr: string; //0x04`（AnsiString ⇒ latin-1 字节串）。</summary>
    public string m_sGateaddr = "";

    /// <summary>SelectClient.pas:59 `m_dwTick10: LongWord; //0x10`</summary>
    public uint m_dwTick10;

    /// <summary>SelectClient.pas:60 `m_nGateID: Integer; //网关ID`（全工程**只被赋 0**；见 OpenUser）。</summary>
    public int m_nGateID;

    /// <summary>SelectClient.pas:62 `m_Module: Pointer;`（原文恒 nil，见 SelectClientModuleSeam）。</summary>
    public IntPtr m_Module;

    /// <summary>SelectClient.pas:63 `m_dwCheckServerTimeMin: LongWord;`</summary>
    public uint m_dwCheckServerTimeMin;

    /// <summary>SelectClient.pas:64 `m_dwCheckServerTimeMax: LongWord;`</summary>
    public uint m_dwCheckServerTimeMax;

    /// <summary>SelectClient.pas:65 `m_dwCheckRecviceTick: LongWord;`（原文拼写 `Recvice` 逐字保留）。</summary>
    public uint m_dwCheckRecviceTick;

    /// <summary>SelectClient.pas:66 `SelectCharList: TSelectChar;`（1000 槽会话表）。</summary>
    public TSelectChar SelectCharList;

    /// <summary>Delphi 的 <c>Divider = ['/']</c> 实参（本单元所有 GetValidStr3 都用单分隔符 '/'）。</summary>
    private static readonly char[] Slash = { '/' };

    // ==========================================================================================
    // 出口接缝：TServerClientWinSocket 的 SendText / RemoteAddress
    // ==========================================================================================

    /// <summary>
    /// 原文 <c>SendText</c>（继承自 ScktComp 的 TServerClientWinSocket，真正写 socket）。
    /// 托管侧无头不可验证 ⇒ 收敛为可注入委托；**默认抛异常**（§25.2，绝不静默丢弃发包）。
    /// 接入点：GatewayKit 的 TcpLink.Send / SelGate 连接的发送缓冲。
    /// </summary>
    public static Action<TSelectClient, byte[]> SendTextSink = (_, __) => throw new NotSupportedException(
        "接缝：ServerClient.pas 的 TServerClientWinSocket.SendText 未接线。接入点：DBServerService 把 TcpLink.Send 赋给 TSelectClient.SendTextSink。");

    /// <summary>
    /// 原文 <c>Self.RemoteAddress</c>（SelectClient.pas:516，ExecGateBuffers 的 'S' 分支）。
    /// 默认抛异常（同上）。
    /// </summary>
    public static Func<TSelectClient, string> RemoteAddressSink = _ => throw new NotSupportedException(
        "接缝：ServerClient.pas 的 TServerClientWinSocket.RemoteAddress 未接线。接入点：DBServerService 把对端地址赋给 TSelectClient.RemoteAddressSink。");

    /// <inheritdoc />
    public override void SendText(byte[] sMsg) => SendTextSink(this, sMsg);

    /// <inheritdoc />
    public override string RemoteAddress => RemoteAddressSink(this);

    // ==========================================================================================
    // 生命周期
    // ==========================================================================================

    /// <summary>
    /// SelectClient.pas:281-294 `constructor TSelectClient.Create(Socket: TSocket; ServerWinSocket: TServerWinSocket)`。
    ///
    /// 【偏差 D-p7-1】原文首参是 WinSock 句柄、次参是 VCL 的 TServerWinSocket（本单元只把它们原样交给
    /// `inherited Create`）；托管侧这两个类型都不存在，且 <see cref="TSelectClient"/> 自身即 socket 接缝，
    /// 故收敛为**无参构造**。字段初始化顺序与初值逐字照抄（含 :274 `m_dwCheckServerTimeMax := 0; //GetTickCount;`）。
    /// </summary>
    public TSelectClient()
    {
        m_dwKeepAliveTick = DelphiTick.GetTickCount();
        m_sReceiveText = "";
        m_sGateaddr = "";
        m_dwTick10 = DelphiTick.GetTickCount();
        m_nGateID = 0;
        m_Module = IntPtr.Zero;
        m_dwCheckServerTimeMin = DelphiTick.GetTickCount();
        m_dwCheckServerTimeMax = 0;                                                                      //GetTickCount;
        m_dwCheckRecviceTick = DelphiTick.GetTickCount();
        SelectCharList = new TSelectChar();
    }

    /// <summary>
    /// SelectClient.pas:297-301 `destructor TSelectClient.Destroy`：`SelectCharList.Free; inherited;`
    /// </summary>
    public void Destroy()
    {
        SelectCharList.Destroy();
    }

    // 原文 :303-348 {$IF DBSUSETHREAD = 1} 的 `procedure Close` 与 `procedure ClientExecute`
    // 在 DBSUSETHREAD = 0 下**不编译**（Close 由 VCL 基类提供）。
    // 任务书列出的 :84/:85 即这两处 —— 本配置下不存在对应托管成员；线程分支的收包循环
    // （ClientExecute → ExecGateBuffers(@RecvBuffer, nMsgLen)）由 PChar 重载承接，见下。

    // ==========================================================================================
    // 发包族
    // ==========================================================================================

    /// <summary>
    /// SelectClient.pas:350-382 `procedure SendKeepAlivePacket();`
    ///
    /// {$ELSE}（生效）分支只有一行本体 <c>SendText('%++$')</c>（原文把 Connected 判定注释掉了）。
    /// {$IF DBSUSETHREAD = 1} 分支是 `if ClientSocket.Connected then try ClientSocket.SendText('%++$') except Close end;`
    /// —— 未编译。
    ///
    /// ★ 注意 <c>%++$</c> 这个字节序列：对端按 <c>%&lt;c&gt;…$</c> 解帧后 <c>c = '+'</c>，
    ///   落进 SelectClient.pas:466 的 **case 默认分支**（不是 '-'），这是一个**极其容易误读**的原文细节。
    /// </summary>
    public void SendKeepAlivePacket()
    {
        SendText(SelectClientAnsi.BytesOf("%++$"));
        m_dwKeepAliveTick = DelphiTick.GetTickCount();
        m_dwCheckServerTimeMin = DelphiTick.GetTickCount() - m_dwCheckRecviceTick;
        if (m_dwCheckServerTimeMin > m_dwCheckServerTimeMax) m_dwCheckServerTimeMax = m_dwCheckServerTimeMin;
        m_dwCheckRecviceTick = DelphiTick.GetTickCount();
        if (m_Module != IntPtr.Zero)
            SelectClientModuleSeam.UpdateModuleBuffer(m_Module,
                DelphiFormat.Format("%d/%d", m_dwCheckServerTimeMin, m_dwCheckServerTimeMax));  // §17.2：Delphi 格式串
    }

    /// <summary>
    /// SelectClient.pas:384-412 `procedure SendKickUser(SocketHandle: string; nKickType: Integer);`
    ///
    /// <c>case nKickType of 0/1/2</c> —— **没有 else**：其它取值**什么都不发**（原文如此）。
    /// {$ELSE} 分支同样把 try/except/Connected 判定注释掉了。
    /// </summary>
    public void SendKickUser(string SocketHandle, int nKickType)
    {
        switch (nKickType)
        {
            case 0: SendText(SelectClientAnsi.BytesOf("%+-" + SocketHandle + "$")); break;
            case 1: SendText(SelectClientAnsi.BytesOf("%+T" + SocketHandle + "$")); break;
            case 2: SendText(SelectClientAnsi.BytesOf("%+B" + SocketHandle + "$")); break;
        }
    }

    /// <summary>
    /// SelectClient.pas:414-434 `procedure SendUserSocket(sSessionID, sSendMsg: string);`
    /// 原文 <c>SendText('%' + sSessionID + '/#' + sSendMsg + '!$')</c>。
    ///
    /// 【偏差 D-p7-2】<c>sSendMsg</c> 由 Delphi AnsiString 改为 <see cref="byte"/>[]：
    /// 它承载的是 6-bit 编码后的**协议字节**，托管侧按转换开发文档 §3.1「协议层一律 byte[]」表达，
    /// 单测因此能逐字节锁定组包结果（原文侧的拼接顺序/分隔符一个不改）。
    /// </summary>
    public void SendUserSocket(string sSessionID, byte[] sSendMsg)
    {
        SendText(SelectClientAnsi.Concat(
            SelectClientAnsi.BytesOf("%" + sSessionID + "/#"),
            sSendMsg,
            SelectClientAnsi.BytesOf("!$")));
    }

    /// <summary>
    /// SelectClient.pas:436-442 `procedure OutOfConnect(sSessionID: string);`
    /// <c>Msg := MakeDefaultMsg(SM_OUTOFCONNECTION, 0,0,0,0); SendUserSocket(sSessionID, EncodeMessage(Msg));</c>
    /// </summary>
    public void OutOfConnect(string sSessionID)
    {
        TDefaultMessage Msg = MakeDefaultMsg(Grobal2Const.SM_OUTOFCONNECTION, 0, 0, 0, 0);
        SendUserSocket(sSessionID, EncodeMessage(Msg));
    }

    // ==========================================================================================
    // 收包 / 解帧
    // ==========================================================================================

    /// <summary>
    /// SelectClient.pas:444-538 `procedure ExecGateBuffers(Buffer: string); overload;`
    /// —— **本配置下真正生效的收包入口**（uFrmMain.pas:340-353 `SelectSocketClientRead` → `ExecGateBuffers(sReceiveText)`）。
    ///
    /// 帧格式 <c>%&lt;c&gt;&lt;body&gt;$</c>，<c>c</c> 的取值见 <see cref="DeCodeUserMsg"/> 前的分派说明：
    ///   '-' 心跳 / 'A' 用户数据 / 'O' 用户接入 / 'X' 用户离开 / 'S' 网关自报地址 / 其它 = 未知（防溢出计数）。
    /// </summary>
    public void ExecGateBuffers(string Buffer)
    {
        int nCount = 0;
        //SetLength(sReceiveText, BufLen);
        //Move(Buffer^, sReceiveText[1], BufLen);
        m_sReceiveText = m_sReceiveText + Buffer;
        //MainOutMessage('m_sReceiveText:'+m_sReceiveText);
        while (true)
        {
            if (DelphiRTL.Pos("$", m_sReceiveText) <= 0) break;
            string s0C = "";
            string s10 = "";
            m_sReceiveText = HUtil32.ArrestStringEx(m_sReceiveText, '%', '$', ref s10);
            if (s10 != "")
            {
                char s19 = s10[0];
                s10 = DelphiRTL.Copy(s10, 2, s10.Length - 1);
                bool boBreakLoop = false;
                switch (s19)
                {
                    case '-':
                        {
                            SendKeepAlivePacket();
                            //MainOutMessage('SendKeepAlivePacket');
                            break;
                        }
                    case 'A':
                        {
                            //MainOutMessage('A');
                            s10 = HUtil32Seam.GetValidStr3(s10, ref s0C, Slash);
                            // ★ Delphi `for I := 0 to Expr` 的 Expr **只求值一次** ⇒ 先取值再循环。
                            int nOnLineCount = SelectCharList.OnLineCount;
                            for (int I = 0; I <= nOnLineCount - 1; I++)
                            {
                                TUserInfo? UserInfo = SelectCharList.OnLineItems(I);
                                if (UserInfo != null)
                                {
                                    if (UserInfo.sConnID == s0C)
                                    {
                                        UserInfo.sReceiveText = UserInfo.sReceiveText + s10;
                                        // ★★ 原文 :484 的 `Continue` 作用在 **for** 上（不是退出 switch），
                                        //    即"没有 '!' 时继续扫描**后续**槽位"；只有命中 '!' 才 Break 退出 for。
                                        //    C# 里这两个 break/continue 同样绑定**最内层**的 for（switch 在 for 之外）。
                                        if (DelphiRTL.Pos("!", s10) < 1) continue;
                                        ProcessUserMsg(UserInfo);
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    case 'O':
                        {
                            //MainOutMessage('O '+s10);
                            s10 = HUtil32Seam.GetValidStr3(s10, ref s0C, Slash);
                            OpenUser(s0C, s10);
                            break;
                        }
                    case 'X':
                        {
                            CloseUser(s10);
                            //MainOutMessage('X');
                            break;
                        }
                    case 'S':
                        {
                            if (HUtil32.CompareLStr(s10, "127.0.0.", "127.0.0.".Length))
                            {
                                m_sGateaddr = s10;
                                MainOutMessage(m_sGateaddr + " 角色网关连接");
                            }
                            else
                            {
                                // {$IF DBSUSETHREAD = 1}: m_sGateaddr := ClientSocket.RemoteAddress;
                                // {$ELSE}（生效）:
                                m_sGateaddr = this.RemoteAddress;
                            }
                            break;
                        }
                    default:
                        {
                            if (nCount >= 1)
                            {
                                m_sReceiveText = "";                                                          //防止DBS溢出攻击
                                boBreakLoop = true;                                                           // 原文 :525 Break（跳出 while，非跳出 case）
                                break;
                            }
                            nCount++;
                            break;
                        }
                }
                if (boBreakLoop) break;
            }
            else
            {
                //MainOutMessage('防止DBS溢出攻击');
                m_sReceiveText = "";                                                                      //防止DBS溢出攻击
                break;
            }
        }
    }

    /// <summary>
    /// SelectClient.pas:540-636 `procedure ExecGateBuffers(Buffer: PChar; BufLen: Integer); overload;`
    ///
    /// 【可达性】原文**唯一**调用点是 :330（<c>ClientExecute</c>，{$IF DBSUSETHREAD = 1}）⇒ 在
    /// DBSUSETHREAD = 0 下**不可达**。托管侧仍保留该入口，供线程化（把 DBSUSETHREAD 打开）时使用。
    ///
    /// 【偏差 D-p7-4】原文把 :458-537 的整段解帧循环**复制**了一份到 :555-635（两段仅注释不同）。
    /// 托管侧改为转调 string 重载，**行为等价**（左侧那句 `m_sReceiveText := m_sReceiveText + sReceiveText`
    /// 与 string 重载的第一句完全同型），不复制 80 行。
    /// `SetLength + Move` 的语义 = 恰好追加 BufLen 字节；原文 BufLen &gt; 缓冲长度时是越界读(UB)，托管侧钳到实际长度。
    /// </summary>
    public void ExecGateBuffers(byte[] Buffer, int BufLen)
    {
        string sReceiveText = SelectClientAnsi.StrOf(Buffer, 0, BufLen);
        ExecGateBuffers(sReceiveText);
    }

    /// <summary>
    /// SelectClient.pas:638-667 `procedure ProcessUserMsg(UserInfo: pTUserInfo);`
    /// 从该槽的 <c>sReceiveText</c> 里按 <c>#…!</c> 逐帧取出，长度 ≥ DEF_BLOCK_SIZE(22) 才交给
    /// <see cref="DeCodeUserMsg"/>。
    /// </summary>
    private void ProcessUserMsg(TUserInfo UserInfo)
    {
        string s10 = "";
        int nC = 0;
        while (true)
        {
            if (HUtil32.TagCount(UserInfo.sReceiveText, '!') <= 0) break;
            UserInfo.sReceiveText = HUtil32.ArrestStringEx(UserInfo.sReceiveText, '#', '!', ref s10);
            if (s10 != "")
            {
                s10 = DelphiRTL.Copy(s10, 2, s10.Length - 1);
                if (s10.Length >= Grobal2Const.DEF_BLOCK_SIZE)
                {
                    DeCodeUserMsg(s10, UserInfo);
                    //MainOutMessage('DeCodeUserMsg');
                }                                                                                         // else Inc(n4ADC20);
            }
            else
            {
                //Inc(n4ADC1C);
                if (nC >= 1)
                {
                    UserInfo.sReceiveText = "";
                }
                nC++;
            }
        }
    }

    // ==========================================================================================
    // 用户接入 / 离开
    // ==========================================================================================

    /// <summary>
    /// SelectClient.pas:669-702 `procedure OpenUser(sID, sIP: string);`
    /// 帧：<c>%O&lt;socket&gt;/&lt;ip&gt;/&lt;localIP&gt;$</c>（SelGate ClientSession.pas:289-290）。
    /// 已在线（同 sConnID）时**直接 Exit**，不重复占槽。
    /// </summary>
    private void OpenUser(string sID, string sIP)
    {
        int nIndex;
        TUserInfo? UserInfo;
        string sUserIPaddr = "";
        string sGateIPaddr = HUtil32Seam.GetValidStr3(sIP, ref sUserIPaddr, Slash);
        // ★ for 上界只求值一次
        int nOnLineCount = SelectCharList.OnLineCount;
        for (int I = 0; I <= nOnLineCount - 1; I++)
        {
            UserInfo = SelectCharList.OnLineItems(I);
            if ((UserInfo != null) && (UserInfo.sConnID == sID))
            {
                return;                                                                                // 原文 Exit
            }
        }
        nIndex = SelectCharList.Add();
        if (nIndex >= 0)
        {
            SelectCharList.Initialize(nIndex);
            UserInfo = SelectCharList.Items(nIndex);
            UserInfo!.nIndex = nIndex;
            UserInfo.sUserIPaddr = sUserIPaddr;
            UserInfo.sGateIPaddr = sGateIPaddr;
            UserInfo.sConnID = sID;
            // {$IF DBSUSETHREAD = 1}: UserInfo.Socket := ClientSocket;
            // {$ELSE}（生效）:
            UserInfo.Socket = this;
            // ★ 原文 :24 字段是 ShortInt（有符号字节），:699 从 Integer 赋值；
            //   Delphi 默认 {$R-} ⇒ **静默取低 8 位**。全工程 m_nGateID 只被赋 0，故实际恒 0。
            UserInfo.nSelGateID = unchecked((sbyte)m_nGateID);
            //MainOutMessage('OpenUser2');
        }
    }

    /// <summary>
    /// SelectClient.pas:704-723 `procedure CloseUser(sID: string);`
    /// 会话仍在（<c>GetGlobaSessionStatus</c> 为真）时**不动** IDSocCli；否则发 SS_SOFTOUTSESSION 并关闭会话。
    /// 随后 <c>SelectCharList.Finalize(UserInfo.nIndex)</c> 并 **Break**（只处理第一个匹配槽）。
    /// </summary>
    private void CloseUser(string sID)
    {
        // ★ for 上界只求值一次
        int nOnLineCount = SelectCharList.OnLineCount;
        for (int I = 0; I <= nOnLineCount - 1; I++)
        {
            TUserInfo? UserInfo = SelectCharList.OnLineItems(I);
            if ((UserInfo != null) && (UserInfo.sConnID == sID))
            {
                // 原文 :714-718：
                //   if not FrmIDSoc.GetGlobaSessionStatus(nSessionID) then begin
                //     FrmIDSoc.SendSocketMsg(SS_SOFTOUTSESSION, sAccount + '/' + IntToStr(nSessionID));
                //     FrmIDSoc.CloseSession(sAccount, nSessionID);
                //   end;
                //
                // ★ 走**窄口子**（偏差 D-p7-13，语义与理由见 IDSocCliSeam.CloseUser_ShouldCloseSession）：
                //   只有这一步允许"未接线时记日志 + 跳过清理"；其余全部接缝保持抛异常。
                if (IDSocCliSeam.CloseUser_ShouldCloseSession(UserInfo.nSessionID))
                {
                    ITFrmIDSoc FrmIDSoc = IDSocCliSeam.Require;
                    FrmIDSoc.SendSocketMsg((ushort)CommonConst.SS_SOFTOUTSESSION,
                        UserInfo.sAccount + "/" + DelphiRTL.IntToStr(UserInfo.nSessionID));
                    FrmIDSoc.CloseSession(UserInfo.sAccount, UserInfo.nSessionID);
                }
                SelectCharList.Finalize(UserInfo.nIndex);
                break;
            }
        }
    }

    // ==========================================================================================
    // 消息分发内核
    // ==========================================================================================

    /// <summary>
    /// SelectClient.pas:725-885 `procedure DeCodeUserMsg(sData: string; UserInfo: pTUserInfo);`
    /// —— **消息分发内核**。分派表（命令 → 原文行号 → 语义）：
    ///
    /// | 命令（值） | case 行 | 语义 | 节流 |
    /// |---|---|---|---|
    /// | CM_QUERYCHR (100)      | :735-752 | 查该账号角色列表 → SM_QUERYCHR | <c>not boChrQueryed or tick&gt;200</c>，否则 "[Hacker Attack] _QUERYCHR" |
    /// | CM_RANDOMNAME (106)    | :753-776 | 随机生成一个角色名 → SM_RANDOMNAME | **无**（原文把 tick 判定整段注释掉了 :756-758/:770-775） |
    /// | CM_NEWCHR (101)        | :777-800 | 建号 → SM_NEWCHR_SUCCESS/FAIL | <c>tick&gt;1000</c>，否则 "[Hacker Attack] _NEWCHR" |
    /// | CM_DELCHR (102)        | :801-823 | 删号 → SM_DELCHR_SUCCESS/FAIL | <c>tick&gt;1000</c>，否则 "[Hacker Attack] _DELCHR" |
    /// | CM_SELCHR (103)        | :824-847 | 选号 → SM_STARTPLAY/STARTFAIL | <c>not boChrQueryed</c>，否则 "Double send _SELCHR" |
    /// | CM_QUERYDELCHR (105)   | :849-862 | 查已删角色（≤10 条）→ SM_QUERYDELCHR | <c>tick&gt;200</c>，否则 "[Hacker Attack] _QUERYDELCHR" |
    /// | CM_GETBACKDELCHR (3006)| :863-876 | 找回已删角色 → SM_GETBAKCHAR_* | <c>tick&gt;200</c>，否则 "[Hacker Attack] _GETBACKDELCHR" |
    /// | 其它                   | :877-883 | 回 SM_CHECKISMYSELFSERVER（"检测是否为我们自己的服务端"）| — |
    ///
    /// ★ 除 CM_RANDOMNAME / CM_QUERYDELCHR / CM_GETBACKDELCHR 外，其余四个分支都先做
    ///   `(sAccount &lt;&gt; '') and FrmIDSoc.CheckSession(...)`；不通过则 `OutOfConnect(sConnID)` + 记日志。
    ///   CM_QUERYCHR / CM_QUERYDELCHR / CM_GETBACKDELCHR **不做** CheckSession（只做时间节流）。
    /// ★ CM_QUERYDELCHR / CM_GETBACKDELCHR 的**返回值被丢弃**（原文如此）。
    /// </summary>
    private void DeCodeUserMsg(string sData, TUserInfo UserInfo)
    {
        string sDefMsg, s18;
        TDefaultMessage Msg, DefMsg;

        sDefMsg = DelphiRTL.Copy(sData, 1, Grobal2Const.DEF_BLOCK_SIZE);
        s18 = DelphiRTL.Copy(sData, Grobal2Const.DEF_BLOCK_SIZE + 1, sData.Length - Grobal2Const.DEF_BLOCK_SIZE);
        Msg = EDcode.DecodeMessage(SelectClientAnsi.BytesOf(sDefMsg));

        switch (Msg.Ident)
        {
            case Grobal2Const.CM_QUERYCHR:
                {
                    //MainOutMessage('CM_QUERYCHR');
                    if (!UserInfo.boChrQueryed || ((DelphiTick.GetTickCount() - UserInfo.dwChrTick) > 200))
                    {
                        UserInfo.dwChrTick = DelphiTick.GetTickCount();
                        if (QueryChr(s18, UserInfo))
                        {
                            UserInfo.boChrQueryed = true;
                        }
                    }
                    else
                    {
                        //Inc(g_nQueryChrCount);
                        MainOutMessage("[Hacker Attack] _QUERYCHR " + UserInfo.sUserIPaddr);
                    }
                    break;
                }
            case Grobal2Const.CM_RANDOMNAME:
                {
                    //MainOutMessage('CM_NEWCHR');
                    //        if (GetTickCount - UserInfo.dwChrTick) > 1000 then
                    //        begin
                    //          UserInfo.dwChrTick := GetTickCount();
                    if ((UserInfo.sAccount != "")
                      && IDSocCliSeam.Require.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID))
                    {
                        RandomName(UserInfo);
                        //            UserInfo.boChrQueryed := False;
                    }
                    else
                    {
                        OutOfConnect(UserInfo.sConnID);
                        MainOutMessage("[ERROR] _NEWCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                    }
                    //        end
                    //        else
                    //        begin
                    //          //Inc(nHackerNewChrCount);
                    //          MainOutMessage('[Hacker Attack] _NEWCHR ' + UserInfo.sAccount + '/' + UserInfo.sUserIPaddr);
                    //        end;
                    break;
                }
            case Grobal2Const.CM_NEWCHR:
                {
                    //MainOutMessage('CM_NEWCHR');
                    if ((DelphiTick.GetTickCount() - UserInfo.dwChrTick) > 1000)
                    {
                        UserInfo.dwChrTick = DelphiTick.GetTickCount();
                        if ((UserInfo.sAccount != "")
                          && IDSocCliSeam.Require.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID))
                        {
                            NewChr(s18, UserInfo);
                            UserInfo.boChrQueryed = false;
                        }
                        else
                        {
                            OutOfConnect(UserInfo.sConnID);
                            MainOutMessage("[ERROR] _NEWCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                        }
                    }
                    else
                    {
                        //Inc(nHackerNewChrCount);
                        MainOutMessage("[Hacker Attack] _NEWCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                    }
                    break;
                }
            case Grobal2Const.CM_DELCHR:
                {
                    if ((DelphiTick.GetTickCount() - UserInfo.dwChrTick) > 1000)
                    {
                        UserInfo.dwChrTick = DelphiTick.GetTickCount();
                        if ((UserInfo.sAccount != "")
                          && IDSocCliSeam.Require.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID))
                        {
                            DelChr(s18, UserInfo);
                            UserInfo.boChrQueryed = false;
                        }
                        else
                        {
                            OutOfConnect(UserInfo.sConnID);
                            MainOutMessage("[ERROR] _DELCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                        }
                    }
                    else
                    {
                        //Inc(nHackerDelChrCount);
                        MainOutMessage("[Hacker Attack] _DELCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                    }
                    break;
                }
            case Grobal2Const.CM_SELCHR:
                {
                    if (!UserInfo.boChrQueryed)
                    {
                        if ((UserInfo.sAccount != "")
                          && IDSocCliSeam.Require.CheckSession(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID))
                        {
                            if (SelectChr(s18, UserInfo))
                            {
                                UserInfo.boChrSelected = true;
                            }
                        }
                        else
                        {
                            OutOfConnect(UserInfo.sConnID);
                            MainOutMessage("[ERROR] _SELCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                        }
                    }
                    else
                    {
                        //Inc(nHackerSelChrCount);
                        MainOutMessage("Double send _SELCHR " + UserInfo.sAccount + "/" + UserInfo.sUserIPaddr);
                    }
                    break;
                }

            case Grobal2Const.CM_QUERYDELCHR:
                {
                    //查询删除的人物
                    //MainOutMessage('CM_QUERYDELCHR');
                    if (DelphiTick.GetTickCount() - UserInfo.dwChrTick > 200)
                    {
                        UserInfo.dwChrTick = DelphiTick.GetTickCount();
                        QueryDelChr(s18, UserInfo);                    // ★ 返回值被丢弃（原文如此）
                    }
                    else
                    {
                        //Inc(g_nQueryChrCount);
                        MainOutMessage("[Hacker Attack] _QUERYDELCHR " + UserInfo.sUserIPaddr);
                    }
                    break;
                }
            case Grobal2Const.CM_GETBACKDELCHR:
                {
                    //找回人物
                    //MainOutMessage('CM_GETBACKDELCHR');
                    if (DelphiTick.GetTickCount() - UserInfo.dwChrTick > 200)
                    {
                        UserInfo.dwChrTick = DelphiTick.GetTickCount();
                        GetBackDelChr(s18, UserInfo);                  // ★ 返回值被丢弃（原文如此）
                    }
                    else
                    {
                        //Inc(g_nQueryChrCount);
                        MainOutMessage("[Hacker Attack] _GETBACKDELCHR " + UserInfo.sUserIPaddr);
                    }
                    break;
                }
            default:
                {
                    //Inc(n4ADC24);
                    // 检测是否为我们自己的服务端
                    DefMsg = MakeDefaultMsg(Grobal2Const.SM_CHECKISMYSELFSERVER, 0, 0, 0, 0);
                    SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
                    break;
                }
        }
    }

    // ==========================================================================================
    // 角色族
    // ==========================================================================================

    /// <summary>
    /// SelectClient.pas:889-901 `procedure RandomName(UserInfo: pTUserInfo);`
    ///
    /// <code>
    /// I := Random(g_FirstName.Count - 1);  sName := g_FirstName.Strings[I];
    /// I := Random(g_LastName.Count  - 1);  sName := sName + g_LastName.Strings[I];
    /// DefMsg := MakeDefaultMsg(SM_RANDOMNAME, 0,0,0,0);
    /// SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg) + EncodeString(sName));
    /// </code>
    ///
    /// ★★ 原文如此（**不是** <c>Random(Count)</c>）：参数是 <c>Count - 1</c> ⇒
    ///   ① 名单有 N ≥ 2 项时，**最后一项永远取不到**（取值域 0..N-2）；
    ///   ② N = 1 时 <c>Random(0) = 0</c> ⇒ 恰好取到唯一那项；
    ///   ③ N = 0 时 <c>Random(-1) = Trunc(Random * -1) = 0</c> ⇒ 去索引**空列表的 [0]** ⇒ 抛异常。
    /// </summary>
    private void RandomName(TUserInfo UserInfo)
    {
        int I;
        string sName;
        TDefaultMessage DefMsg;

        I = SelectClientRandom.Random(SelectClientGlobals.g_FirstName.Count - 1);
        sName = SelectClientGlobals.g_FirstName[I];
        I = SelectClientRandom.Random(SelectClientGlobals.g_LastName.Count - 1);
        sName = sName + SelectClientGlobals.g_LastName[I];
        DefMsg = MakeDefaultMsg(Grobal2Const.SM_RANDOMNAME, 0, 0, 0, 0);
        SendUserSocket(UserInfo.sConnID, SelectClientAnsi.Concat(EncodeMessage(DefMsg), EncodeString(sName)));
    }

    /// <summary>
    /// SelectClient.pas:903-998 `procedure NewChr(sData: string; UserInfo: pTUserInfo);`
    ///
    /// 载荷：<c>DecodeString(sData)</c> 后的 <c>账号/角色名/发型/职业/性别/&lt;多余字段&gt;</c>（'/' 分隔）。
    /// nCode 语义（原文注释）：-1 初值；0 名字非法/过长/多余字段；2 名字被拒或已存在；3 数量超限；
    /// 5 建号功能关闭；6 含数字；7 全英文；8 命中非法字符过滤表；1 成功。
    ///
    /// ★★ **全是字节语义**（见 <see cref="SelectClientAnsi"/>）：<c>Length(sChrName)</c> 是**字节数**
    ///   （两个汉字 = 4 ⇒ 通过 MIN_CHAR_NAME_LEN=4），<c>sChrName[I] in TextChars</c> 也是按**字节**过滤。
    /// ★ 顺序关键：先 CheckDenyChrName/CheckChrName（**原始名**），再按字节删非法字符，
    ///   然后才用**过滤后的名**做 CheckFilterNewHumanChrName / CheckSpecialChar / 重名 / 入库。
    /// ★ <c>Imp(g_nCreateHumCount)</c> 只在入表成功（nCode := 1）时自增。
    /// </summary>
    private void NewChr(string sData, TUserInfo UserInfo)
    {
        string Data, sAccount, sChrName, sHair, sJob, sSex;
        int nCode;

        int I;
        TDefaultMessage DefMsg;

        int nHumanCount;
        if (!TBool.ToBool(DBShareSeam.g_boCanCreateHuman))
        {
            DefMsg = MakeDefaultMsg(Grobal2Const.SM_NEWCHR_FAIL, 5, 0, 0, 0);
            SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
            return;                                                                                    // 原文 Exit
        }

        nCode = -1;

        Data = DecodeString(sData);
        sAccount = ""; sChrName = ""; sHair = ""; sJob = ""; sSex = "";    // C# ref 形参需先定值（原文无此要求）
        Data = HUtil32Seam.GetValidStr3(Data, ref sAccount, Slash);
        Data = HUtil32Seam.GetValidStr3(Data, ref sChrName, Slash);
        Data = HUtil32Seam.GetValidStr3(Data, ref sHair, Slash);
        Data = HUtil32Seam.GetValidStr3(Data, ref sJob, Slash);
        Data = HUtil32Seam.GetValidStr3(Data, ref sSex, Slash);
        sChrName = DelphiRTL.Trim(sChrName);

        if (DelphiRTL.Trim(Data) != "") nCode = 0;

        if (sChrName.Length < SelectClientDbShareSeam.MIN_CHAR_NAME_LEN) nCode = 0;
        if (!SelectClientDbShareSeam.CheckDenyChrName(sChrName)) nCode = 2;
        if (!SelectClientDbShareSeam.CheckChrName(sChrName)) nCode = 0;
        for (I = sChrName.Length; I >= 1; I--)
        {
            if (!(InTextChars(sChrName[I - 1]))) sChrName = sChrName.Remove(I - 1, 1);
        }

        if (SelectClientDbShareSeam.CheckFilterNewHumanChrName(sChrName)) nCode = 8;                    //2;

        if (!TBool.ToBool(DBShareSeam.g_boDenyChrName) && (nCode == -1))
        {
            if (!SelectClientDbShareSeam.CheckSpecialChar(SelectClientAnsi.AnsiTextOf(sChrName)))
            {
                nCode = 0;
            }
        }

        if ((nCode == -1) && TBool.ToBool(DBShareSeam.g_boForbidNumberName) && SelectClientDbShareSeam.CheckNumberName(sChrName))  //检测是否有数字
            nCode = 6;

        if ((nCode == -1) && TBool.ToBool(DBShareSeam.g_boForbidLetterName) && SelectClientDbShareSeam.CheckLetterName(sChrName))  //检测是否全部是英文
            nCode = 7;


        if ((nCode == -1) && (sChrName.Length > SelectClientDbShareSeam.MAX_CHAR_NAME_LEN))
        {
            nCode = 0;
        }

        if (nCode == -1)
        {
            string sChrNameText = SelectClientAnsi.AnsiTextOf(sChrName);
            if ((SelectClientRoleDbSeam.RequireHuman.GetID(sChrNameText) != RoleDbConst.NO_ID)
              || (SelectClientRoleDbSeam.RequireHero.GetID(sChrNameText) != RoleDbConst.NO_ID))
            {
                nCode = 2;
            }
            else
            {
                nHumanCount = SelectClientRoleDbSeam.RequireHuman.GetHumanCount(SelectClientAnsi.AnsiTextOf(sAccount));
                if (nHumanCount < DBShareSeam.g_nCreateChrNameCount)
                {
                    if (SelectClientRoleDbSeam.RequireHuman.Add(SelectClientAnsi.AnsiTextOf(sAccount), sChrNameText, nHumanCount == 0,
                            (byte)DelphiRTL.StrToIntDef(sSex, 0), (byte)DelphiRTL.StrToIntDef(sJob, 0), (byte)DelphiRTL.StrToIntDef(sHair, 0)))
                    {
                        nCode = 1;
                        SelectClientGlobals.g_nCreateHumCount++;
                    }
                    else
                        nCode = 2;
                }
                else
                {
                    nCode = 3;
                }
            }
        }

        if (nCode == 1)
        {
            DefMsg = MakeDefaultMsg(Grobal2Const.SM_NEWCHR_SUCCESS, 0, 0, 0, 0);
        }
        else
        {
            DefMsg = MakeDefaultMsg(Grobal2Const.SM_NEWCHR_FAIL, nCode, DBShareSeam.g_nCreateChrNameCount, 0, 0);
        }

        SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
    }

    /// <summary>
    /// SelectClient.pas:1000-1041 `function QueryDelChr(sData: string; UserInfo: pTUserInfo): Boolean;`
    /// 取该账号**已删除**的角色，**最多 10 条**（<c>if nChrCount &gt;= 10 then break</c>，:1032）。
    /// 每条 21 字节 <c>TDeleteHumanInfo</c>（6-bit 编码后）以 '/' 串接在 SM_QUERYDELCHR 之后。
    /// ★ 只有 <c>QueryDeleteHumans(...) &gt; 0</c> 时才 <c>Result := True</c>。
    /// ★ <c>if Length(sData) &gt; 0</c> 判的是**编码后的入参**（不是解码后的账号）。
    /// </summary>
    private bool QueryDelChr(string sData, TUserInfo UserInfo)
    {
        string sAccount, S;
        int nChrCount;
        int I;
        // 原文是未初始化的栈上 record；GXX 的 ShortStr.Set **零填充**余下字节，故 default 等价（见报告）。
        TDeleteHumanInfo DeleteHumanInfo = default;
        TQueryHumanList HumanList;
        TQueryHumanData? HumData;

        bool Result = false;
        sAccount = SelectClientAnsi.AnsiTextOf(DecodeString(sData));
        nChrCount = 0;
        S = "";

        if (sData.Length > 0)
        {
            HumanList = new TQueryHumanList();
            try
            {
                if (SelectClientRoleDbSeam.RequireHuman.QueryDeleteHumans(sAccount, HumanList) > 0)
                {
                    Result = true;

                    for (I = 0; I <= HumanList.Count - 1; I++)
                    {
                        HumData = HumanList.Items(I);

                        // 原文 `DeleteHumanInfo.sChrName := HumData.HumanName;`
                        // （托管侧 TDeleteHumanInfo 的 sChrName 是 fixed 缓冲，只能经 ChrName 属性写入）
                        DeleteHumanInfo.ChrName = HumData!.HumanName;
                        DeleteHumanInfo.nLevel = HumData.Level;
                        DeleteHumanInfo.btJob = (byte)HumData.Job;
                        DeleteHumanInfo.btSex = (byte)HumData.Sex;
                        S = S + SelectClientAnsi.StrOf(EncodeBuffer(StructBytes.BytesOf(DeleteHumanInfo),
                                                                   StructBytes.SizeOf<TDeleteHumanInfo>())) + "/";
                        nChrCount++;
                        if (nChrCount >= 10) break;
                    }
                }
            }
            finally
            {
                // HumanList.Free
            }
        }

        SendUserSocket(UserInfo.sConnID, SelectClientAnsi.Concat(
            EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_QUERYDELCHR, nChrCount, 0, 0, 0)),
            SelectClientAnsi.BytesOf(S)));
        return Result;
    }

    /// <summary>
    /// SelectClient.pas:1043-1081 `function GetBackDelChr(sData: string; UserInfo: pTUserInfo): Boolean;`
    /// 找回已删角色。<c>nCode</c>：-5 功能关闭；-1 角色已满；-2 找回失败；1 成功；0 参数不全。
    /// ★★ **原文缺陷（逐字保留）**：<c>Result</c> 在 :1048 置 False 后**再无赋值** ⇒
    ///    本函数**恒返回 False**，即使 <c>nCode = 1</c>（成功）也一样。
    ///    调用点 <c>DeCodeUserMsg:869</c> 丢弃了返回值，故当前**不产生行为差异**；差异断言见测试。
    /// </summary>
    private bool GetBackDelChr(string sData, TUserInfo UserInfo)
    {
        string sAccount, sChrName;
        int nChrCount, nCode;
        bool Result = false;
        if (TBool.ToBool(DBShareSeam.g_boCanGetBackDeleteHuman))
        {
            sData = DecodeString(sData);
            sAccount = ""; sChrName = "";
            sData = HUtil32Seam.GetValidStr3(sData, ref sAccount, Slash);
            sData = HUtil32Seam.GetValidStr3(sData, ref sChrName, Slash);

            nCode = 0;

            if ((sAccount.Length > 0) && (sChrName.Length > 0))
            {
                nChrCount = SelectClientRoleDbSeam.RequireHuman.GetHumanCount(SelectClientAnsi.AnsiTextOf(sAccount));
                if (nChrCount >= DBShareSeam.g_nCreateChrNameCount)
                    nCode = -1;
                else if (SelectClientRoleDbSeam.RequireHuman.DeleteRestore(SelectClientAnsi.AnsiTextOf(sAccount), SelectClientAnsi.AnsiTextOf(sChrName)))
                {
                    nCode = 1;
                }
                else
                {
                    nCode = -2;
                }
            }
        }
        else
        {
            nCode = -5;
        }

        if (nCode == 1)
            SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_GETBAKCHAR_SUCCESS, nCode, 0, 0, 0)));
        else
            SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_GETBAKCHAR_FAIL, nCode, 0, 0, DBShareSeam.g_nCreateChrNameCount)));

        return Result;
    }

    /// <summary>
    /// SelectClient.pas:1083-1118 `procedure DelChr(sData: string; UserInfo: pTUserInfo);`
    /// <c>nCode</c>：0 初值（=名字不存在/未删）；-2 删除功能关闭；-3 等级 &gt; g_nCanDeleteHumanLowLevel；1 成功。
    /// ★ <c>g_nCanDeleteHumanLowLevel</c> 默认 45：**等级 &gt; 45 不可删**（边界是 &gt;，不是 &gt;=）。
    /// </summary>
    private void DelChr(string sData, TUserInfo UserInfo)
    {
        string sChrName, S;
        TDefaultMessage Msg;
        int nCode;                                                                                     //{, nHumanID}: Integer;
        int Sex, Job, Level, LastLogin;
        nCode = 0;
        if (TBool.ToBool(DBShareSeam.g_boCanDeleteHuman))
        {
            sChrName = DecodeString(sData);

            if (SelectClientRoleDbSeam.RequireHuman.GetBaseInfo(SelectClientAnsi.AnsiTextOf(sChrName), out Sex, out Job, out Level, out LastLogin))
            {
                if (Level > DBShareSeam.g_nCanDeleteHumanLowLevel)
                    nCode = -3;
                else
                {
                    if (SelectClientRoleDbSeam.RequireHuman.Delete(UserInfo.sAccount, SelectClientAnsi.AnsiTextOf(sChrName)))
                    {
                        nCode = 1;
                    }
                }
            }
        }
        else
            nCode = -2;

        if (nCode == 1)
            Msg = MakeDefaultMsg(Grobal2Const.SM_DELCHR_SUCCESS, 0, 0, 0, 0);
        else
            Msg = MakeDefaultMsg(Grobal2Const.SM_DELCHR_FAIL, nCode, 0, 0, 0);

        S = SelectClientAnsi.StrOf(EncodeMessage(Msg));
        SendUserSocket(UserInfo.sConnID, SelectClientAnsi.BytesOf(S));
    }

    /// <summary>
    /// SelectClient.pas:1120-1180 `function SelectChr(sData: string; UserInfo: pTUserInfo): Boolean;`
    /// 两条路由：
    ///   · <c>not g_boUseActiveRunGage</c>（默认）：<c>DBShare.GateRouteIP(m_sGateaddr, ...)</c>，
    ///     动态 IP 模式（<c>g_boDynamicIPMode</c>）直接改用 <c>UserInfo.sGateIPaddr</c>；
    ///   · 否则：<c>DBShare.GateActiveRouteIP(...)</c>；动态 IP 模式且 <c>CheckActiveRunGate</c> 不通过 ⇒ 置空 ⇒ SM_STARTFAIL。
    /// 成功路径都发 <c>SM_STARTPLAY + EncodeString(routeIP + '/' + IntToStr(port + nMapIndex))</c>
    /// 并调 <c>FrmIDSoc.SetGlobaSessionPlay</c>。
    /// ★ 原文 :1152 写的是 <c>m_sGateAddr</c>（大写 A）而字段声明在 :58 是 <c>m_sGateaddr</c>
    ///   —— Delphi 标识符**大小写不敏感**，是同一字段；托管侧（大小写敏感）必须写成同一个成员。
    /// </summary>
    private bool SelectChr(string sData, TUserInfo UserInfo)
    {
        string sAccount, sChrName;
        int nMapIndex;
        bool boDataOK;
        byte[] sDefMsg;
        byte[] sRouteMsg;
        string sRouteIP;
        int nRoutePort;
        bool Result = false;
        sAccount = "";
        sChrName = HUtil32Seam.GetValidStr3(DecodeString(sData), ref sAccount, Slash);
        boDataOK = SelectClientRoleDbSeam.RequireHuman.Select(SelectClientAnsi.AnsiTextOf(sAccount), SelectClientAnsi.AnsiTextOf(sChrName));

        if (boDataOK)
        {
            nMapIndex = 0;
            sDefMsg = EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_STARTPLAY, 0, 0, 0, 0));
            if (!TBool.ToBool(DBShareSeam.g_boUseActiveRunGage))
            {
                sRouteIP = DBShareSeam.GateRouteIP(m_sGateaddr, out nRoutePort);
                // 修复支持动态IP piaoyun 2013-08-29
                if (TBool.ToBool(SelectClientGlobals.g_boDynamicIPMode)) sRouteIP = UserInfo.sGateIPaddr;   //使用动态IP

                sRouteMsg = EncodeString(sRouteIP + "/" + DelphiRTL.IntToStr(nRoutePort + nMapIndex));
                SendUserSocket(UserInfo.sConnID, SelectClientAnsi.Concat(sDefMsg, sRouteMsg));
                IDSocCliSeam.Require.SetGlobaSessionPlay(UserInfo.nSessionID);
                Result = true;
            }
            else
            // 只取可以连接的网关 chongchong 2015-07-22
            {
                sRouteIP = SelectClientDbShareSeam.GateActiveRouteIP(m_sGateaddr, out nRoutePort);

                // 修复支持动态IP piaoyun 2013-08-29
                if (TBool.ToBool(SelectClientGlobals.g_boDynamicIPMode))
                {
                    sRouteIP = UserInfo.sGateIPaddr;
                    if (!SelectClientDbShareSeam.CheckActiveRunGate(sRouteIP, nRoutePort))
                        sRouteIP = "";
                }

                if (sRouteIP.Length == 0)
                {
                    Result = false;
                    SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_STARTFAIL, 0, 0, 0, 0)));
                }
                else
                {
                    sRouteMsg = EncodeString(sRouteIP + "/" + DelphiRTL.IntToStr(nRoutePort + nMapIndex));
                    SendUserSocket(UserInfo.sConnID, SelectClientAnsi.Concat(sDefMsg, sRouteMsg));
                    IDSocCliSeam.Require.SetGlobaSessionPlay(UserInfo.nSessionID);
                    Result = true;
                }
            }
        }
        else
        {
            SendUserSocket(UserInfo.sConnID, EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_STARTFAIL, 0, 0, 0, 0)));
        }
        return Result;
    }

    /// <summary>
    /// SelectClient.pas:1182-1237 `function QueryChr(sData: string; UserInfo: pTUserInfo): Boolean;`
    /// 载荷 <c>账号/会话号</c>；<c>nSessionID := StrToIntDef(sSessionID, -2)</c>（**默认 -2**，不是 0）。
    /// 体例 <c>[*]名/职业/发型/等级/性别/</c> 串接在 SM_QUERYCHR 之后（<c>*</c> 标记"上次选中的那个"）。
    /// 角色条数上限 <c>g_nCreateChrNameCount</c>。
    /// 会话校验失败 ⇒ 发 SM_QUERYCHR_FAIL 后 **CloseUser**（把用户踢下线）。
    ///
    /// ★★ **原文缺陷（逐字保留）**：<c>Result</c> 在 :1190 置 False 后**再无赋值** ⇒
    ///    **恒返回 False**；于是 DeCodeUserMsg:742-745 的 <c>boChrQueryed := True</c> 永不执行
    ///    ⇒ CM_QUERYCHR 的 `not boChrQueryed` 门永远为真（200ms 节流始终生效），
    ///      且 CM_SELCHR 的 `if not boChrQueryed` 也**永远进得去**（"Double send _SELCHR" 分支不可达）。
    /// </summary>
    private bool QueryChr(string sData, TUserInfo UserInfo)
    {
        string sAccount, sSessionID, S;
        int nSessionID;
        int I, nChrCount;
        TQueryHumanList HumanList;
        TQueryHumanData? HumanData;
        bool Result = false;
        sAccount = "";
        sSessionID = HUtil32Seam.GetValidStr3(DecodeString(sData), ref sAccount, Slash);
        nSessionID = DelphiRTL.StrToIntDef(sSessionID, -2);
        UserInfo.nSessionID = nSessionID;

        S = "";
        nChrCount = 0;

        if (IDSocCliSeam.Require.CheckSession(SelectClientAnsi.AnsiTextOf(sAccount), UserInfo.sUserIPaddr, nSessionID))
        {
            IDSocCliSeam.Require.SetGlobaSessionNoPlay(nSessionID);
            UserInfo.sAccount = SelectClientAnsi.AnsiTextOf(sAccount);

            if (TBool.ToBool(SelectClientGlobals.g_boShowQuryChrLog))
            {
                MainOutMessage("查询帐户角色信息:" + SelectClientAnsi.AnsiTextOf(sAccount));
            }

            HumanList = new TQueryHumanList();
            try
            {
                if (SelectClientRoleDbSeam.RequireHuman.QueryHumans(SelectClientAnsi.AnsiTextOf(sAccount), HumanList) > 0)
                {
                    nChrCount = HumanList.Count;

                    if (nChrCount > DBShareSeam.g_nCreateChrNameCount)
                        nChrCount = DBShareSeam.g_nCreateChrNameCount;

                    for (I = 0; I <= nChrCount - 1; I++)
                    {
                        HumanData = HumanList.Items(I);
                        if (HumanData!.IsSelect) S = S + "*";
                        S = S + HumanData.HumanName + "/" + DelphiRTL.IntToStr(HumanData.Job) + "/" + DelphiRTL.IntToStr(HumanData.Hair) + "/" + DelphiRTL.IntToStr(HumanData.Level) + "/" + DelphiRTL.IntToStr(HumanData.Sex) + "/";
                    }
                }
            }
            finally
            {
                // HumanList.Free
            }

            SendUserSocket(UserInfo.sConnID, SelectClientAnsi.Concat(
                EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_QUERYCHR, nChrCount, 0, 1, 0)), EncodeString(S)));
            //*ChrName/sJob/sHair/sLevel/sSex/
        }
        else
        {
            SendUserSocket(UserInfo.sConnID,
                EncodeMessage(MakeDefaultMsg(Grobal2Const.SM_QUERYCHR_FAIL, nChrCount, 0, 1, 0)));
            CloseUser(UserInfo.sConnID);
        }
        return Result;
    }

    // ==========================================================================================
    // 原文同名转发（只为让调用点与 .pas 逐字可对照）
    // ==========================================================================================

    /// <summary>EDcode.pas:620-632 `MakeDefaultMsg(wIdent: Word; nRecog: Int64; wParam, wTag, wSeries: Word)`。</summary>
    private static TDefaultMessage MakeDefaultMsg(int wIdent, long nRecog, int wParam, int wTag, int wSeries)
        => TDefaultMessage.Make((ushort)wIdent, nRecog, (ushort)wParam, (ushort)wTag, (ushort)wSeries);

    /// <summary>EDcode.pas `function EncodeMessage(Msg: TDefaultMessage): AnsiString;`</summary>
    private static byte[] EncodeMessage(in TDefaultMessage Msg) => EDcode.EncodeMessage(Msg);

    /// <summary>EDcode.pas `function EncodeString(const S: AnsiString): AnsiString;`（入参是 GBK 文本）。</summary>
    private static byte[] EncodeString(string S) => EDcode.EncodeString(S);

    /// <summary>EDcode.pas `function EncodeBuffer(Src: PAnsiChar; SrcLen: Integer): AnsiString;`</summary>
    private static byte[] EncodeBuffer(byte[] Src, int SrcLen) => EDcode.EncodeBuffer(Src, SrcLen);

    /// <summary>
    /// EDcode.pas `function DecodeString(const S: AnsiString): AnsiString;`
    /// —— 入参是承载 6-bit 编码字节的 AnsiString，出参是**GBK 文本的字节串**。
    /// 全程走字节（<see cref="SelectClientAnsi"/>），返回值仍是字节串（调用方按需再 AnsiTextOf）。
    /// </summary>
    private static string DecodeString(string S)
        => SelectClientAnsi.StrOf(EDcode.DecodeString(SelectClientAnsi.BytesOf(S)));

    /// <summary>DBShare.pas:16 `TextChars = [#32..#255]`（AnsiChar 集合 ⇒ 按字节判定）。</summary>
    private static bool InTextChars(char c)
        => ((byte)c >= SelectClientDbShareSeam.TextCharsFirst) && ((byte)c <= SelectClientDbShareSeam.TextCharsLast);

    /// <summary>DBShare.pas:82 `procedure MainOutMessage(sMsg: string);`（原文只写主消息队列，不重抛）。</summary>
    private static void MainOutMessage(string sMsg) => RoleDbSeam.MainOutMessage(sMsg);

    /// <summary>复位本类的两处静态出站接缝（单测用）。</summary>
    public static void ResetSeams()
    {
        SendTextSink = (_, __) => throw new NotSupportedException(
            "接缝：ServerClient.pas 的 TServerClientWinSocket.SendText 未接线。接入点：DBServerService 把 TcpLink.Send 赋给 TSelectClient.SendTextSink。");
        RemoteAddressSink = _ => throw new NotSupportedException(
            "接缝：ServerClient.pas 的 TServerClientWinSocket.RemoteAddress 未接线。接入点：DBServerService 把对端地址赋给 TSelectClient.RemoteAddressSink。");
    }
}
