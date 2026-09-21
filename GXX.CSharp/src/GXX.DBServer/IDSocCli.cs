using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// Source\DBServer\IDSocCli.pas（464 行）→ IDSocCli.cs
//
// 它是 DBServer 与 **LoginSrv**（ID 服务器）之间的客户端：LoginSrv 用文本帧
//   `(<nIdent>/<payload>)`            例：`(1000/account/123/0/0/1.2.3.4)`
// 推送全局会话的开/关，DBServer 据此维护 `GlobaSessionList`，供选人端校验会话
// （`SelectClient.pas` 的 `CheckSession` / `SetGlobaSessionPlay` …）。
//
// 【本单元的边界】原文 `TFrmIDSoc = class(TForm)`，但其 DFM（IDSocCli.dfm）里**只有 3 个非可视组件**
//   （`IDSocket: TClientSocket`、`Timer1`/`KeepAliveTimer: TTimer`），**没有任何可视控件**。
//   托管侧这 3 个都必须是接缝（JSocket/TTimer 未移植）⇒ 见 `IDSocCli.Seams.cs`。
//
// 【原文陷阱（照抄标签，不照抄注释值）】
//   `ProcessSocketMsg` 的 case 写作 `SS_OPENSESSION {100}` / `SS_CLOSESESSION {101}` / `SS_KEEPALIVE {104}`
//   —— 那三个 `{…}` 注释是**陈旧**的，真值在 Common.pas:23-27 分别是 **1000 / 1010 / 1040**
//   （`SS_SERVERINFO = 1030`）。托管侧 case 标签照抄**常量名**，注释里写明"原文如此（注释值与真值不符）"，
//   否则照注释值写会把三条推送**全部漏掉**。
//
// 【`%X` 相关】`SelectClient.pas` 只用到本单元的 6 个方法；`FrmIDSoc` 这个全局的接线见
//   `DBServerService`（`DBServer.dpr:36 Application.CreateForm(TFrmIDSoc, FrmIDSoc)` 的对应物）。
// ============================================================================================

/// <summary>
/// IDSocCli.pas:9-53 `TFrmIDSoc`（1:1）。
/// 公开面同时实现 <see cref="ITFrmIDSoc"/>，以便直接赋给 <see cref="IDSocCliSeam.FrmIDSoc"/>。
/// </summary>
public class TFrmIDSoc : Form, ITFrmIDSoc
{
    // ==========================================================================================
    // 字段（原文 :24-38 / :56-65）
    // ==========================================================================================

    /// <summary>IDSocCli.pas:24 `GlobaSessionList: TList; //0x2D8`（存 `pTGlobaSessionInfo`）。</summary>
    private readonly List<TGlobaSessionInfo> GlobaSessionList = new List<TGlobaSessionInfo>();

    /// <summary>IDSocCli.pas:25 `m_sSockMsg: string; //0x2E4`（收包累计缓冲；AnsiString ⇒ latin-1 字节串）。</summary>
    private string m_sSockMsg = "";

    /// <summary>IDSocCli.pas:26 `m_dwKeepAlivePacketTick: LongWord;`</summary>
    private uint m_dwKeepAlivePacketTick;

    /// <summary>IDSocCli.pas:35 `m_Module: Pointer;`（`pTModuleInfo`）。</summary>
    public IntPtr m_Module;

    /// <summary>IDSocCli.pas:36 `m_dwCheckServerTimeMin: LongWord;`</summary>
    public uint m_dwCheckServerTimeMin;

    /// <summary>IDSocCli.pas:37 `m_dwCheckServerTimeMax: LongWord;`</summary>
    public uint m_dwCheckServerTimeMax;

    /// <summary>IDSocCli.pas:38 `m_dwCheckRecviceTick: LongWord;`（原文拼写 `Recvice` 逐字保留）。</summary>
    public uint m_dwCheckRecviceTick;

    /// <summary>只读视图：当前全局会话条数（原文无此成员；单测与诊断用，不改变任何行为）。</summary>
    public int GlobaSessionCount => GlobaSessionList.Count;

    /// <summary>只读视图：按索引取会话（越界返回 null；原文经 `GlobaSessionList.Items[I]` 直接取）。</summary>
    public TGlobaSessionInfo? GlobaSessionAt(int index)
        => (index >= 0 && index < GlobaSessionList.Count) ? GlobaSessionList[index] : null;

    /// <summary>
    /// IDSocCli.pas:64-75 `TFrmIDSoc.FormCreate` 中**不依赖宿主设施**的部分。
    /// （DFM 的 `OnCreate=FormCreate`；构造即执行。）
    ///
    /// ★ 原文 :66/:67 的两行 `Timer1.Enabled := False; KeepAliveTimer.Enabled := False;`
    ///   **不在这里**，见 <see cref="FormCreate"/> 与被登记的偏差 **D-p7-15**：
    ///   托管侧那两个定时器是**宿主设施接缝**（默认抛），而"构造时两个定时器都还没安装"
    ///   本身就已经是"Enabled = False"的同一状态 ⇒ 构造阶段调用接缝只会把
    ///   "宿主还没装定时器"变成"连窗体都建不出来"。
    /// </summary>
    public TFrmIDSoc()
    {
        // DFM: object TFrmIDSoc: TFrmIDSoc ... ClientHeight=121 ClientWidth=63 Caption='FrmIDSoc'
        Text = "FrmIDSoc";
        ClientSize = new System.Drawing.Size(63, 121);

        // :68  GlobaSessionList := TList.Create（字段初始化器已完成）
        m_sSockMsg = "";                                       // :69
        m_Module = IntPtr.Zero;                                // :70
        m_dwCheckServerTimeMin = DelphiTick.GetTickCount();    // :71
        m_dwCheckServerTimeMax = 0;                            // :72  //GetTickCount;
        m_dwCheckRecviceTick = DelphiTick.GetTickCount();      // :73
        m_dwKeepAlivePacketTick = DelphiTick.GetTickCount();   // :74
    }

    /// <summary>
    /// IDSocCli.pas:64-75 `TFrmIDSoc.FormCreate`（DFM 的 `OnCreate` 事件体）中
    /// **依赖宿主设施**的两行：`:66 Timer1.Enabled := False; :67 KeepAliveTimer.Enabled := False;`
    ///
    /// 托管侧调它＝要求宿主**已经装好这两个定时器**；未装则按接缝约定抛（§25.2）。
    /// `DBServerService` 当前**不调用**本方法（它只建窗体，不装定时器）—— 见 D-p7-15。
    /// </summary>
    public void FormCreate()
    {
        IDSocCliSeam.Timer1Enabled(false);                     // :66  Timer1.Enabled := False
        IDSocCliSeam.KeepAliveTimerEnabled(false);             // :67  KeepAliveTimer.Enabled := False
    }

    /// <summary>
    /// IDSocCli.pas:77-88 `TFrmIDSoc.FormDestroy`：逐个 `Dispose(pTGlobaSessionInfo)` 后 `Free` 列表。
    /// 托管侧 `TGlobaSessionInfo` 是 class ⇒ `Dispose` 即丢弃引用。
    /// </summary>
    public void FormDestroy()
    {
        // for I := 0 to GlobaSessionList.Count - 1 do Dispose(GlobaSessionList.Items[I]);
        GlobaSessionList.Clear();
    }

    // ==========================================================================================
    // 收包 / 解包（纯逻辑）
    // ==========================================================================================

    /// <summary>IDSocCli.pas:100-108 `IDSocketRead`：累计 + 只要有 ')' 就尝试解包。</summary>
    public void IDSocketRead()
    {
        m_sSockMsg = m_sSockMsg + IDSocCliSeam.RequireIDSocket.Socket.ReceiveText;   // :103
        if (DelphiRTL.Pos(")", m_sSockMsg) > 0)                                      // :104
        {
            ProcessSocketMsg();
        }
    }

    /// <summary>
    /// IDSocCli.pas:110-132 `ProcessSocketMsg`：按 `(ident/body)` 逐帧解，再按 ident 分派。
    /// ★ case 标签是 `SS_OPENSESSION`/`SS_CLOSESESSION`/`SS_KEEPALIVE`，真值 **1000/1010/1040**
    ///   （原文注释写的 `{100}`/`{101}`/`{104}` 是陈旧的）。
    /// </summary>
    public void ProcessSocketMsg()
    {
        string sScoketText = m_sSockMsg;                                             // :118
        string sData = "";
        while (DelphiRTL.Pos(")", sScoketText) > 0)                                  // :119
        {
            sScoketText = HUtil32.ArrestStringEx(sScoketText, '(', ')', ref sData);  // :121
            if (sData == "") break;                                                  // :122
            string sCode = "";
            string sBody = HUtil32Seam.GetValidStr3(sData, ref sCode, Slash);        // :123
            int nIdent = DelphiRTL.StrToIntDef(sCode, 0);                            // :124
            switch (nIdent)                                                          // :125
            {
                case CommonConst.SS_OPENSESSION:                                     // 原 :126 `SS_OPENSESSION {100}`
                    ProcessAddSession(sBody);
                    break;
                case CommonConst.SS_CLOSESESSION:                                    // 原 :127 `SS_CLOSESESSION {101}`
                    ProcessDelSession(sBody);
                    break;
                case CommonConst.SS_KEEPALIVE:                                       // 原 :128 `SS_KEEPALIVE {104}`
                    ProcessGetOnlineCount(sBody);
                    break;
            }
        }
        m_sSockMsg = sScoketText;                                                    // :131
    }

    /// <summary>
    /// IDSocCli.pas:328-349 `ProcessAddSession`：`账号/会话号/n24/忽略/网关IP` 五段。
    ///
    /// ★ 原文 `New(GlobaSessionInfo)` 后**没有**给 `bo28` 赋值（record 其余 9 个字段都赋了）
    ///   —— Delphi 的 `New` 给**未初始化**内存 ⇒ `bo28` 是垃圾值（偏差 D-p7-14）；
    ///   托管侧 `new TGlobaSessionInfo()` 零初始化 ⇒ `bo28 = false`。
    /// </summary>
    public void ProcessAddSession(string sData)
    {
        string sAccount = "", s10 = "", s14 = "", s18 = "", sIPaddr = "";
        sData = HUtil32Seam.GetValidStr3(sData, ref sAccount, Slash);                // :333
        sData = HUtil32Seam.GetValidStr3(sData, ref s10, Slash);                     // :334
        sData = HUtil32Seam.GetValidStr3(sData, ref s14, Slash);                     // :335
        sData = HUtil32Seam.GetValidStr3(sData, ref s18, Slash);                     // :336
        sData = HUtil32Seam.GetValidStr3(sData, ref sIPaddr, Slash);                 // :337
        var GlobaSessionInfo = new TGlobaSessionInfo();                              // :338 New(...)
        GlobaSessionInfo.sAccount = sAccount;                                        // :339
        GlobaSessionInfo.sIPaddr = sIPaddr;                                          // :340
        GlobaSessionInfo.nSessionID = DelphiRTL.StrToIntDef(s10, 0);                 // :341
        GlobaSessionInfo.n24 = DelphiRTL.StrToIntDef(s14, 0);                        // :342
        GlobaSessionInfo.boStartPlay = false;                                        // :343
        GlobaSessionInfo.boLoadRcd = false;                                          // :344
        GlobaSessionInfo.boHeroLoadRcd = false;                                      // :345
        //  原文 :345 前多一个空格，无行为差异
        GlobaSessionInfo.dwAddTick = DelphiTick.GetTickCount();                      // :346
        GlobaSessionInfo.dAddDate = DelphiDate.Now();                                // :347
        GlobaSessionList.Add(GlobaSessionInfo);                                      // :348
    }

    /// <summary>IDSocCli.pas:351-373 `ProcessDelSession`：`账号/会话号`。</summary>
    public void ProcessDelSession(string sData)
    {
        string sAccount = "";
        sData = HUtil32Seam.GetValidStr3(sData, ref sAccount, Slash);                // :357
        int nSessionID = DelphiRTL.StrToIntDef(sData, 0);                            // :358
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :359
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :361
            if (GlobaSessionInfo != null)                                            // :362
            {
                if ((GlobaSessionInfo.nSessionID == nSessionID)
                    && DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount)) // :364
                {
                    GlobaSessionList.RemoveAt(I);                                    // :366
                    // Dispose(GlobaSessionInfo) → 托管侧丢弃引用
                    //MainOutMessage('ProcessDelSession:'+sAccount+' '+IntToStr(nSessionID));
                    break;                                                           // :369
                }
            }
        }
    }

    /// <summary>
    /// IDSocCli.pas:429-436 `ProcessGetOnlineCount`：刷新 `m_dwCheckServerTimeMin/Max` 与模块缓冲。
    /// `sData` **未被使用**（原文如此）。
    /// </summary>
    public void ProcessGetOnlineCount(string sData)
    {
        m_dwCheckServerTimeMin = DelphiTick.GetTickCount() - m_dwCheckRecviceTick;   // :431
        if (m_dwCheckServerTimeMin > m_dwCheckServerTimeMax)                         // :432
            m_dwCheckServerTimeMax = m_dwCheckServerTimeMin;
        m_dwCheckRecviceTick = DelphiTick.GetTickCount();                            // :433
        if (m_Module != IntPtr.Zero)                                                 // :434
            SelectClientModuleSeam.UpdateModuleBuffer(m_Module,
                DelphiFormat.Format("%d/%d", m_dwCheckServerTimeMin, m_dwCheckServerTimeMax));  // :435
    }

    // ==========================================================================================
    // 组包 / 发包
    // ==========================================================================================

    /// <summary>
    /// IDSocCli.pas:134-143 `SendSocketMsg`：`Format('(%d/%s)', [wIdent, sMsg])`，**仅在已连接时**发送。
    /// ★ 未连接时**静默不发送** —— 原文如此（不是接缝的中性值，是原文的真实分支）。
    /// </summary>
    public void SendSocketMsg(ushort wIdent, string sMsg)
    {
        // resourcestring sFormatMsg = '(%d/%s)';
        string sSendText = DelphiFormat.Format("(%d/%s)", wIdent, sMsg);             // :140
        if (IDSocCliSeam.RequireIDSocket.Socket.Connected)                           // :141
            IDSocCliSeam.RequireIDSocket.Socket.SendText(sSendText);                 // :142
    }

    /// <summary>
    /// IDSocCli.pas:375-386 `SendKeepAlivePacket`：**3000ms 节流**，发
    /// `(<SS_SERVERINFO=1030>/<g_sServerName>/99/<FrmMain.GetSelectCharCount>)`。
    /// 原文 :385 的注释 `//(103/翎风世界/0/0)` 里的 103 是陈旧值（真值 1030）。
    /// </summary>
    public void SendKeepAlivePacket()
    {
        if (DelphiTick.GetTickCount() - m_dwKeepAlivePacketTick > 3000)              // :377
        {
            m_dwKeepAlivePacketTick = DelphiTick.GetTickCount();                     // :379
            if (IDSocCliSeam.RequireIDSocket.Socket.Connected)                       // :380
            {
                IDSocCliSeam.RequireIDSocket.Socket.SendText(
                    "(" + DelphiRTL.IntToStr(CommonConst.SS_SERVERINFO)
                    + "/" + IDSocCliSeam.g_sServerName
                    + "/" + "99"
                    + "/" + DelphiRTL.IntToStr(IDSocCliSeam.GetSelectCharCount()) + ")");   // :382
            }
        }
    }

    // ==========================================================================================
    // 9 个会话查询 / 变更（纯逻辑；SelectClient.pas 用到其中 6 个）
    // ==========================================================================================

    /// <summary>
    /// IDSocCli.pas:145-164 `CheckSession`。
    /// ★ `sIPaddr` 形参在函数体里**从未被使用**（原文如此，只比对 `sAccount` 与 `nSessionID`）。
    /// </summary>
    public bool CheckSession(string sAccount, string sIPaddr, int nSessionID)
    {
        bool Result = false;                                                         // :151
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :152
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :154
            if (GlobaSessionInfo != null)                                            // :155
            {
                if (DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount)
                    && (GlobaSessionInfo.nSessionID == nSessionID))                  // :157
                {
                    Result = true;                                                   // :159
                    break;                                                           // :160
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// IDSocCli.pas:166-190 `CheckSessionLoadRcd`：命中后在**首次**调用时把 `boLoadRcd` 置真并返回真
    /// （"这条会话的存档只加载一次"的闩锁）。
    /// </summary>
    public bool CheckSessionLoadRcd(string sAccount, string sIPaddr, int nSessionID, ref bool boFoundSession)
    {
        bool Result = false;                                                         // :171
        boFoundSession = false;                                                      // :172
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :173
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :175
            if (GlobaSessionInfo != null)                                            // :176
            {
                if (DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount)
                    && (GlobaSessionInfo.nSessionID == nSessionID))                  // :178
                {
                    boFoundSession = true;                                           // :180
                    if (!GlobaSessionInfo.boLoadRcd)                                 // :181
                    {
                        GlobaSessionInfo.boLoadRcd = true;                           // :183
                        Result = true;                                               // :184
                    }
                    break;                                                           // :186
                }
            }
        }
        return Result;
    }

    /// <summary>IDSocCli.pas:192-216 `CheckSessionHeroLoadRcd`：同上，闩锁是 `boHeroLoadRcd`。</summary>
    public bool CheckSessionHeroLoadRcd(string sAccount, string sIPaddr, int nSessionID, ref bool boFoundSession)
    {
        bool Result = false;
        boFoundSession = false;
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);
            if (GlobaSessionInfo != null)
            {
                if (DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount)
                    && (GlobaSessionInfo.nSessionID == nSessionID))
                {
                    boFoundSession = true;
                    if (!GlobaSessionInfo.boHeroLoadRcd)
                    {
                        GlobaSessionInfo.boHeroLoadRcd = true;
                        Result = true;
                    }
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// IDSocCli.pas:218-236 `SetSessionSaveRcd`：把该账号的**全部**会话 `boLoadRcd := False`。
    /// ★ 原文**没有 Break**（与相邻的 `CheckSessionLoadRcd` 不同），且只要命中一次就返回 True。
    /// </summary>
    public bool SetSessionSaveRcd(string sAccount)
    {
        bool Result = false;                                                         // :223
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :224
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :226
            if (GlobaSessionInfo != null)                                            // :227
            {
                if (DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount))    // :229
                {
                    GlobaSessionInfo.boLoadRcd = false;                              // :231
                    Result = true;                                                   // :232
                }
            }
        }
        return Result;
    }

    /// <summary>IDSocCli.pas:238-255 `SetGlobaSessionNoPlay`：按 `nSessionID`，命中即 `boStartPlay := False` 并 Break。</summary>
    public void SetGlobaSessionNoPlay(int nSessionID)
    {
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :243
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :245
            if (GlobaSessionInfo != null)                                            // :246
            {
                if (GlobaSessionInfo.nSessionID == nSessionID)                       // :248
                {
                    GlobaSessionInfo.boStartPlay = false;                            // :250
                    break;                                                           // :251
                }
            }
        }
    }

    /// <summary>IDSocCli.pas:257-274 `SetGlobaSessionPlay`：同上，`boStartPlay := True`。</summary>
    public void SetGlobaSessionPlay(int nSessionID)
    {
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :262
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :264
            if (GlobaSessionInfo != null)                                            // :265
            {
                if (GlobaSessionInfo.nSessionID == nSessionID)                       // :267
                {
                    GlobaSessionInfo.boStartPlay = true;                             // :269
                    break;                                                           // :270
                }
            }
        }
    }

    /// <summary>
    /// IDSocCli.pas:276-294 `GetGlobaSessionStatus`：返回该会话的 `boStartPlay`；
    /// **找不到该会话时返回 False**（这与"会话存在但未开始玩"返回 False 是同一结果 —— 原文如此）。
    /// </summary>
    public bool GetGlobaSessionStatus(int nSessionID)
    {
        bool Result = false;                                                         // :281
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :282
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :284
            if (GlobaSessionInfo != null)                                            // :285
            {
                if (GlobaSessionInfo.nSessionID == nSessionID)                       // :287
                {
                    Result = GlobaSessionInfo.boStartPlay;                           // :289
                    break;                                                           // :290
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// IDSocCli.pas:296-318 `CloseSession`：按 `nSessionID` **且** 账号同名，删除该条并 Break。
    /// ★ 原文的 `if` 是**两层嵌套**（先 `nSessionID` 再 `SameText`）—— 与 `ProcessDelSession` 的
    ///   `and` 写法等价，但分支形态不同；这里照抄嵌套形态。
    /// </summary>
    public void CloseSession(string sAccount, int nSessionID)
    {
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :301
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :303
            if (GlobaSessionInfo != null)                                            // :304
            {
                if (GlobaSessionInfo.nSessionID == nSessionID)                       // :306
                {
                    if (DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount))// :308
                    {
                        GlobaSessionList.RemoveAt(I);                                // :311  Dispose + Delete(I)
                        //MainOutMessage('CloseSession:'+sAccount+' '+IntToStr(nSessionID));
                        break;                                                       // :313
                    }
                }
            }
        }
    }

    /// <summary>IDSocCli.pas:395-413 `GetSession`：按账号 **且** `sIPaddr` 精确同名。</summary>
    public bool GetSession(string sAccount, string sIPaddr)
    {
        bool Result = false;                                                         // :400
        for (int I = 0; I <= GlobaSessionList.Count - 1; I++)                        // :401
        {
            TGlobaSessionInfo? GlobaSessionInfo = GlobaSessionAt(I);                 // :403
            if (GlobaSessionInfo != null)                                            // :404
            {
                if (DelphiStrUtils.SameText(GlobaSessionInfo.sAccount, sAccount)
                    && (GlobaSessionInfo.sIPaddr == sIPaddr))                        // :406
                {
                    Result = true;                                                   // :408
                    break;                                                           // :409
                }
            }
        }
        return Result;
    }

    // ==========================================================================================
    // 连接生命周期（宿主面接缝）
    // ==========================================================================================

    /// <summary>
    /// IDSocCli.pas:90-98 `Timer1Timer`：未激活就（重新）指向 ID 服务器并激活。
    /// 原文顺序：**先赋值 Address/Port，再 Active := True**。
    /// </summary>
    public void Timer1Timer()
    {
        if (!IDSocCliSeam.RequireIDSocket.Active)                                    // :92
        {
            IDSocCliSeam.RequireIDSocket.Address = IDSocCliSeam.g_sIDServerAddr;     // :94
            IDSocCliSeam.RequireIDSocket.Port = IDSocCliSeam.g_nIDServerPort;        // :95
            IDSocCliSeam.RequireIDSocket.Active = true;                              // :96
        }
    }

    /// <summary>IDSocCli.pas:415-422 `OpenConnect`：开定时器 + 先 `Active := False` 再按配置重新激活。</summary>
    public void OpenConnect()
    {
        IDSocCliSeam.Timer1Enabled(true);                                            // :417
        IDSocCliSeam.RequireIDSocket.Active = false;                                 // :418
        IDSocCliSeam.RequireIDSocket.Address = IDSocCliSeam.g_sIDServerAddr;         // :419
        IDSocCliSeam.RequireIDSocket.Port = IDSocCliSeam.g_nIDServerPort;            // :420
        IDSocCliSeam.RequireIDSocket.Active = true;                                  // :421
    }

    /// <summary>IDSocCli.pas:388-393 `CloseConnect`：关定时器 + `Active := False` + `m_Module := nil`。</summary>
    public void CloseConnect()
    {
        IDSocCliSeam.Timer1Enabled(false);                                           // :390
        IDSocCliSeam.RequireIDSocket.Active = false;                                 // :391
        m_Module = IntPtr.Zero;                                                      // :392
    }

    /// <summary>IDSocCli.pas:424-427 `KeepAliveTimerTimer` → `SendKeepAlivePacket()`。</summary>
    public void KeepAliveTimerTimer() => SendKeepAlivePacket();

    /// <summary>IDSocCli.pas:320-326 `IDSocketError`：`ErrorCode := 0`（吞掉错误）+ 强制关 socket。</summary>
    public void IDSocketError(out int ErrorCode)
    {
        ErrorCode = 0;                                                               // :324
        IDSocCliSeam.RequireIDSocket.Socket.Close();                                 // :325
    }

    /// <summary>
    /// IDSocCli.pas:438-454 `IDSocketConnect`：清零统计、记远端地址、开保活定时器、登记模块表。
    ///
    /// 原文 :449-453 先把 `ModuleInfo.Module := Self` / `ModuleName := g_sServerName` /
    /// `Address := Format('%s:%d → %s:%d', …)` / `Buffer := '0/0'` 装进局部 `TModuleInfo`，
    /// 再 `m_Module := AddModule(@ModuleInfo)`。模块表的**身份键是 `Self`**（DBShare.pas:316/334）。
    /// </summary>
    public void IDSocketConnect()
    {
        m_dwCheckServerTimeMin = DelphiTick.GetTickCount();                          // :444
        m_dwCheckServerTimeMax = 0;                                                  // :445 //GetTickCount;
        m_dwCheckRecviceTick = DelphiTick.GetTickCount();                            // :446
        string sRemoteAddress = IDSocCliSeam.RequireIDSocket.Socket.RemoteAddress;   // :447
        IDSocCliSeam.KeepAliveTimerEnabled(true);                                    // :448
        // :449 ModuleInfo.Module := Self（身份键 = 本窗体）
        // :450 ModuleInfo.ModuleName := g_sServerName
        // :451 ModuleInfo.Address := Format('%s:%d → %s:%d', [sRemoteAddress, Socket.LocalPort,
        //                                                      sRemoteAddress, Socket.RemotePort])
        // :452 ModuleInfo.Buffer := '0/0'
        // :453 m_Module := AddModule(@ModuleInfo)
        string address = DelphiFormat.Format("%s:%d → %s:%d",
            sRemoteAddress, IDSocCliSeam.RequireIDSocket.Socket.LocalPort,
            sRemoteAddress, IDSocCliSeam.RequireIDSocket.Socket.RemotePort);
        m_Module = SelectClientModuleSeam.AddModule(
            this, IDSocCliSeam.g_sServerName, address, "0/0");
    }

    /// <summary>
    /// IDSocCli.pas:456-462 `IDSocketDisconnect`：清模块句柄、关保活定时器、从模块表移除。
    /// ★ 原文 :461 传的是 **`Self`**（身份键），不是 `m_Module` 句柄（:459 已把它置 nil）。
    /// </summary>
    public void IDSocketDisconnect()
    {
        m_Module = IntPtr.Zero;                                                      // :459
        IDSocCliSeam.KeepAliveTimerEnabled(false);                                   // :460
        SelectClientModuleSeam.RemoveModule(this);                                   // :461 RemoveModule(Self)
    }

    /// <summary>`GetValidStr3` 的 `['/']` 实参（本单元所有调用都用单分隔符 '/'）。</summary>
    private static readonly char[] Slash = { '/' };
}
