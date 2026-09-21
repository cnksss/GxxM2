// =====================================================================================
// 源单元：Source/RunGate/MirClientContext.pas（GBK，实测 11,125 LF / 11,126 物理行）
//
// 本文件覆盖的原文行号范围（1:1 逐字移植，行号以 `Get-Content -Encoding Default` 归一化
// CRLF→LF 后的物理行为准）：
//   * 类方法声明 1-301（uses / const / type / TClientMsg / TProcessMsg / TRecordActionInfo /
//     TMirClientContext 全部字段与属性 41-294 / 单元级函数声明 296-297 / MIRCONTEXT_RUN_THREAD_COUNT 300）
//   * UpdateLockUserList   308-351
//   * GetUserLockTime      353-387
//   * SubStringOccurences  389-412
//   * Create               416-443
//   * Destroy              445-470
//   * DoReset              473-679
//   * Run                  681-1886
//   * GetExVersionNO       1888-1901
//   * CheckRecvPacketSize  1904-1918
//   * ProcessClientMessage 2947-3020
//   * DelayClientMessage   3022-3054
//   * ClearClientMsgList   3056-3078
//   * GetClientMessage     3080-3120
//   * SendMessaggeToClient 3123-3143
//   * SendMessageToServer  3146-3228（3 个重载）
//   * SendActionRet        3230-3238
//   * LockUser / UnLockUser 3241-3279
//   * FilterSayMsg         3281-3408
//   * GetSpeedText         3410-3416
//   * ContinuousSpeed      3418-3455
//   * ProcessAssasinate    3457-3463
//   * GetConcurrentPacketCount / ClearConcurrentPacket 9689-9735
//   * SendWarnMsg          9737-9745
//   * GetRunGate           9747-9753
//   * AddServerMsg / AddServerText 9755-9844
//   * CloseContextSocket   9846-9881
//   * DoConnect            9883-9965
//   * DoDisconnect         10023-10249
//   * DelayClose           10892-10896
//
// 分工文件（同一 partial class TMirClientContext）：
//   * MirClientContext.Messages.cs  → DoCheckRecvBuffer 1920-2945 / ProcesssSendToClient* 10251-10646 /
//                                     DoLogClientPacket 10648-10797 / GenerateVerifyCode 10799-10890 /
//                                     SendAntiPlugStream* 10899-11064
//   * MirClientContext.CheckUsePlugin.cs → CheckUsePlugin 3465-9688
//   * MirClientContextSeams.cs      → 接缝（IocpCommon/IocpTcpServer/Grobal2_Ex/GateShare）
//
// 条件编译（依据 docs/并行报告-p2-rungate-impl.md §1，实测生效组合）：
//   CLIENT_ANTIPLUG = 1（Grobal2_Ex.pas:10）→ 反外挂分支为**活代码**
//   NEED_REGISTER   = 1（Grobal2_Ex.pas:18）→ `Dissconnect` 反调试函数为活代码，属 §2.3 不移植项
//   REGISTER_TEST   = 0 → 暗桩日志分支为死代码
//   MultiThreadRunContext = 1（Grobal2_Ex.pas:24）→ `Run` 的形参是 RunThread: TThread
//   UseIocpClient   = 1（IocpCommon.pas:14）→ 走 `RunGate.TcpClient.` 前缀
//   LOG_PLUG_DATA   = 1 → `LogPluginData` 为活代码
//   USE_SPINLOCK    未定义（Common/iocp.inc）→ 所有 `Create('xxxLocker')` 的字符串形参不存在
// =====================================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

// 使移植文本与原 Delphi 保持同形：全局量、RTL 函数、协议常量按原名裸用。
using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.RunGate.AnsiStrSeam;
using static GXX.Core.Rtl.DelphiRTL;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate;

// -------------------------------------------------------------------------------------
// 原文 :14-15
//   MAX_RECORD_ACTION_COUNT = 30;
// -------------------------------------------------------------------------------------
public static class MirClientContextConst
{
    public const int MAX_RECORD_ACTION_COUNT = 30;

    // 原文 :300
    public const int MIRCONTEXT_RUN_THREAD_COUNT = 8;
}

// -------------------------------------------------------------------------------------
// 原文 :18-25  PClientMsg / TClientMsg
//   pBuffer: PChar  → byte[]（承载二进制，见 MirClientContextSeams.cs 偏差 D3）
// -------------------------------------------------------------------------------------
public sealed class TClientMsg
{
    public TDefaultMessage DefMessage;
    public uint dwTimeTick;
    public bool boDelay;
    public byte[] pBuffer;
    public int nBufferLen;
}

// -------------------------------------------------------------------------------------
// 原文 :27-33  PProcessMsg / TProcessMsg
//   sMessage: string 语义上是**原始二进制**（来自 TClientMsg.pBuffer），C# 侧用 byte[]。
// -------------------------------------------------------------------------------------
public sealed class TProcessMsg
{
    public TDefaultMessage DefMessage;
    public uint dwTimeTick;
    public bool boDelay;
    public byte[] sMessage = Array.Empty<byte>();
}

// -------------------------------------------------------------------------------------
// 原文 :35-39  TRecordActionInfo
// -------------------------------------------------------------------------------------
public struct TRecordActionInfo
{
    public TBaseAction Action;
    public uint Tick;
    public TDefaultMessage DefMsg;
}

// =====================================================================================
// 原文 :41  TMirClientContext = class(TIocpClientContext)
// =====================================================================================
public partial class TMirClientContext : TIocpClientContext
{
    // ---- private 字段（原文 :43-63）----
    private TBaseAction FLastAction;                                   // :43

    private readonly TSafeList FClientMsgList;                         // :45

    private uint FDelayTick;                                           // :47

    private uint FLastSendMyHeartbeatTick;                             // :49

    private readonly TSafeMemoryStream FScreenshotStream;              // :51

    private readonly TSafeMemoryStream FClientResponseFileStream;      // :53
    private string FClientResponseFileName = "";                       // :54
    private uint FClientResponseFileTick;                              // :55
    private ushort FClientResponseFileIndex;                           // :56
    private ushort FClientResponseFileCount;                           // :57

    private byte[] FServerMsgStr = Array.Empty<byte>();                // :59
    private readonly TIocpCriticalSection FServerMsgLocker;            // :60

    private string FLogPakcetFileName = "";                            // :62
    private readonly TIocpCriticalSection FWriteLogLocker;             // :63

    // ---- public 字段（原文 :150-293）----
    public uint dwConnectTick;                                         // :150
    public int nUserListIndex;                                         // :151
    public int nPacketIndex;                                           // :152
    public int nPacketErrCount;                                        // :153
    public bool boStartLogon;                                          // :154

    public bool boSendGmClose;                                         // :156

    public bool boLoginNoticeOK;                                       // :158
    public uint dwSendDateToClientTick;                                // :159

    public string sAccount = "";                                       // :161
    public string sChrName = "";                                       // :162
    public string sLogFileChrName = "";                                // :163
    public int nSessionID;                                             // :164
    public string sVersion = "";                                       // :165
    public string sMachineID = "";                                     // :166
    public string sMapName = "";                                       // :167

    public bool boIsOldClient;                                         // :170

    public uint dwDisableSayMsgTick;                                   // :172
    public uint dwSayMsgTick;                                          // :173
    public uint dwSayMsgCount;                                         // :174

    public int nMoveSpeed;                                             // :176
    public int nAttackSpeed;                                           // :177
    public int nSpellSpeed;                                            // :178

    public long nRecogId;                                              // :180

    public bool boChangeMap;                                           // :182

    public byte btJob;                                                 // :185
    public byte btHeroJob;                                             // :186

    public bool boLocked;                                              // :189
    public uint dwUnLockTick;                                          // :190

    public bool boFirstClientQueryBagItems;                            // :192
    public bool boClientSoftClose;                                     // :193

    public int nClientLogoutDelay;                                     // :195
    public bool boDelayClientLogout;                                   // :196
    public uint dwDelayClientLogoutTick;                               // :197

    public bool boValidClose;                                          // :199
    public int nClientCloseDelay;                                      // :200
    public bool boDelayClientClose;                                    // :201
    public uint dwDelayClientCloseTick;                                // :202

    public TGameSpeed GameSpeed;                                       // :204

    public TAntiPlugActionMode LastLockAntiPlugActionMode;             // :206

    /// <summary>原文 dwCollectIntervalArr: array[TAntiPlugActionMode, 0..99] of Integer。</summary>
    public int[,] dwCollectIntervalArr = new int[ActionModeCount, 100];                 // :208
    public int[] nCollectIntervalIndexArr = new int[ActionModeCount];                   // :209

    public TRecordActionInfo[] RecordActionArr = new TRecordActionInfo[MirClientContextConst.MAX_RECORD_ACTION_COUNT]; // :211
    public int nRecordActionIndex;                                                      // :212

    public uint[,] SumSpeedProcessArr = new uint[ActionModeCount, 2];                   // :214
    public int[] nCompensationArr = new int[ActionModeCount];                           // :216

    // {$IF CLIENT_ANTIPLUG = 1} 活分支（原文 :218-236）
    public uint dwClientAntiPlugVersion;                               // :219
    public ushort wSendLoadAntiPlugCode;                               // :220
    public bool boSendLoadAntiPlug;                                    // :221
    public int nSendLoadAntiPlugIndex;                                 // :222
    public bool boSendLoadAntiPlugFinished;                            // :223
    public bool boWaitLoadAntiPlug;                                    // :224
    public uint dwWaitLoadAntiPlugTick;                                // :225

    public uint dwSendLoadAntiPlugTick;                                // :227
    public bool boRecvLoadAntiPlug;                                    // :228
    public uint dwRecvLoadAntiPlugTick;                                // :229

    public uint dwRecvClientAntiplugCRC;                               // :231

    public readonly TIocpCriticalSection ContextDataLocker;            // :233
    public byte[] pContextData;                                        // :234
    public uint nContextDataLen;                                       // :235

    public int nIllegalPacketCount;                                    // :237

    public byte[] SendProcessBlacklistMD5 = new byte[16];              // :239（MD5Digest = array[0..15] of Byte）
    public uint dwSendProcessBlacklistTick;                            // :240

    public uint dwLastRungateDoorTick;                                 // :242

    public bool boSendCheckCode;                                       // :244
    public int dwSendCheckCode;                                        // :245
    public uint dwSendCheckTick;                                       // :246
    public bool boRecvCheckCodeOK;                                     // :247

    public int nClientSendDate;                                        // :249
    public ushort wClinetSendHour;                                     // :250
    public int nClientSendRunGateIP;                                   // :251
    public uint dwClientSendDateTick;                                  // :252

    public bool boDelayClose;                                          // :254
    public uint dwDelayCloseTick;                                      // :255

    public readonly TSafeStringList ProcessList;                       // :257

    public readonly TMagicIntervalList MagicUseTickList;                // :259
    public bool[] MagicCDSpeed = new bool[10];                          // :260  array[0..9] of Boolean
    public int MagicCDSpeedCount;                                       // :261
    public int MagicCDSpeedIndex;                                       // :262

    public uint LastEatingItemTick;                                     // :264
    public uint LastHeroEatingItemTick;                                 // :265
    // EatingItemMakeIndex: Integer;                                    // :266 原文如此（已注释）
    public readonly TBagItemList HumBagItems;                           // :267
    public readonly TBagItemList HeroBagItems;                          // :268

    // {$IF MultiThreadRunContext <> 0} 活分支（原文 :270-272）
    public int RunThreadIndex;                                          // :271

    public uint dwVerifyInterval;                                       // :274

    public bool boSendVerifyCode;                                       // :276
    public uint dwSendVerifyCodeTick;                                   // :277
    public int nVerifyCodeErrCount;                                     // :278
    public int nVerifyCodeRefreshCount;                                 // :279
    public string sVerifyCode = "";                                     // :280
    public int nVerifySuccessCount;                                     // :281

    public bool boVerifyDisableAttack;                                  // :283

    public bool boEnableClientUploadPickItems;                          // :285
    public uint dwClientUploadPickItemsTick;                            // :286

    public string RequestClientFileRootPath = "";                       // :288
    public string SaveResponseClientFileRoot = "";                      // :289

    // 只读属性（原文 :291-293）
    public uint ClientResponseFileTick => FClientResponseFileTick;
    public ushort ClientResponseFileIndex => FClientResponseFileIndex;
    public ushort ClientResponseFileCount => FClientResponseFileCount;

    // 内部探针（**public**：测试工程是独立程序集，internal 不可见；
    // 与 uBuffer*.cs 的 `*Internal` 探针同一约定，生产语义不变）
    public byte[] ServerMsgStrProbe => FServerMsgStr;
    public TBaseAction LastActionProbe => FLastAction;

    /// <summary>
    /// **可写探针**（仅测试用；生产语义不变）。
    /// <para>
    /// <c>FLastAction</c>（原文 :43）是 private，而 `CheckUsePlugin` 的 CM_WALK / CM_RUN /
    /// CM_TURN / 攻击族 / CM_SPELL 五个 ident 族**尚未移植**，测试无法用"真的走一次走路包"
    /// 的方式把它设成 `baWalk`/`baRun`/`baCutMeat`，也就无法覆盖 CM_SITDOWN 分支里
    /// `:9076 FLastAction in [baWalk, baRun]`（移动到挖肉）与 `:9249 amCutMeat`
    /// 这两条互斥限速路径。故按 uBuffer*.cs 的探针约定公开一个可写入口
    /// （**只影响测试**，接缝层与生产代码都不写它）。
    /// </para>
    /// </summary>
    public TBaseAction LastActionForTest
    {
        get => FLastAction;
        set => FLastAction = value;
    }
    public uint DelayTickProbe => FDelayTick;
    public uint LastSendMyHeartbeatTickProbe => FLastSendMyHeartbeatTick;
    public TSafeMemoryStream ScreenshotStreamProbe => FScreenshotStream;
    public TSafeMemoryStream ClientResponseFileStreamProbe => FClientResponseFileStream;
    public string LogPacketFileNameProbe => FLogPakcetFileName;   // 原文拼写 Pakcet（:62）
    public TSafeList ClientMsgListProbe => FClientMsgList;
    public TIocpCriticalSection ServerMsgLockerProbe => FServerMsgLocker;
    public string ClientResponseFileNameProbe => FClientResponseFileName;

    // =================================================================================
    // 原文 :416-443  constructor TMirClientContext.Create(AIocpCore: TIocpCore; ASocket: TSocket = 0)
    // USE_SPINLOCK 未定义 → TSafeList.Create / TIocpCriticalSection.Create 无字符串形参。
    // =================================================================================
    public TMirClientContext(TIocpCore aIocpCore, uint aSocket = 0)
        : base(aIocpCore, aSocket)
    {
        FClientMsgList = new TSafeList();                    // :420
        ProcessList = new TSafeStringList();                 // :421
        FScreenshotStream = new TSafeMemoryStream();         // :422
        FClientResponseFileStream = new TSafeMemoryStream(); // :423
        FClientResponseFileTick = 0;                         // :424
        FClientResponseFileIndex = 0;                        // :425
        FClientResponseFileCount = 0;                        // :426

        FServerMsgLocker = new TIocpCriticalSection();       // :428
        FWriteLogLocker = new TIocpCriticalSection();        // :429

        MagicUseTickList = new TMagicIntervalList();         // :431

        LastEatingItemTick = 0;                              // :433
        LastHeroEatingItemTick = 0;                          // :434
        HumBagItems = new TBagItemList(46);                  // :435
        HeroBagItems = new TBagItemList(46);                 // :436

        // {$IF CLIENT_ANTIPLUG = 1}  原文 :438-442
        ContextDataLocker = new TIocpCriticalSection();      // :439
        nContextDataLen = 300;                               // :440
        pContextData = new byte[nContextDataLen];            // :441 GetMem(pContextData, nContextDataLen)
        // 原文 :416-443 里**没有** GameSpeed := ...；GameSpeed 是 Delphi record，对象字段天然全 0。
        // 托管侧为引用类型，必须显式构造（见 MirClientContextSeams.cs 偏差 D5），语义等价。
        GameSpeed = new TGameSpeed();                        // 偏差 D5（= Delphi 零值）
    }

    // =================================================================================
    // 原文 :445-470  destructor TMirClientContext.Destroy;
    // Delphi 的 Destroy 由调用方（TIocpClientContextPool）显式执行；
    // 托管侧保留同名显式方法（对应 Delphi 的 Free），终结器只做兜底。
    // =================================================================================
    ~TMirClientContext()
    {
        // 原文 445-470 的非托管资源（FClientMsgList 的 GetMem 块等）已由 ClearClientMsgList 归零。
    }

    /// <summary>原文 <c>destructor Destroy</c>（:445-470）。</summary>
    public void Destroy()
    {
        ClearClientMsgList();                                // :447
        // FClientMsgList.Free                                    :448 → GC（托管对象）
        // ProcessList.Free                                       :449 → GC
        FScreenshotStream.Dispose();                         // :450
        FClientResponseFileStream.Dispose();                 // :451
        // FServerMsgLocker.Free / FWriteLogLocker.Free           :453-454 → GC
        // MagicUseTickList.Free                                  :456 → GC
        // HumBagItems.Free / HeroBagItems.Free                   :458-459 → GC

        // {$IF CLIENT_ANTIPLUG = 1}  原文 :461-467
        if (nContextDataLen > 0 && pContextData != null)     // :462
        {
            pContextData = null;                             // :464 FreeMem(pContextData, nContextDataLen)
        }
        // ContextDataLocker.Free                                 :466 → GC
        // inherited;                                             :469
    }

    // =================================================================================
    // 原文 :473-679  procedure TMirClientContext.DoReset(IsClose: Boolean);
    // =================================================================================
    protected override void DoReset(bool IsClose)
    {
        base.DoReset(IsClose);                               // :477 inherited;
        if (!IsClose)                                        // :478
        {
            dwConnectTick = MyGetTickCount();                // :480

            nUserListIndex = -1;                             // :482
            nPacketIndex = -1;                               // :483
            nPacketErrCount = 0;                             // :484
            boStartLogon = true;                             // :485

            boLoginNoticeOK = false;                         // :487
            dwSendDateToClientTick = 0;                      // :488

            sAccount = "";                                   // :490
            sChrName = "";                                   // :491
            sLogFileChrName = "";                            // :492
            nSessionID = 0;                                  // :493
            sVersion = "";                                   // :494
            sMachineID = "";                                 // :495
            sMapName = "";                                   // :496
            boIsOldClient = true;                            // :497

            dwDisableSayMsgTick = 0;                         // :499
            dwSayMsgTick = 0;                                // :500
            dwSayMsgCount = 0;                               // :501

            nMoveSpeed = 0;                                  // :503
            nAttackSpeed = 0;                                // :504
            nSpellSpeed = 0;                                 // :505

            nRecogId = 0;                                    // :507
            boChangeMap = false;                             // :508

            btJob = 0;                                       // :511
            btHeroJob = 0;                                   // :512

            boLocked = false;                                // :514
            dwUnLockTick = 0;                                // :515

            boFirstClientQueryBagItems = false;              // :517
            boClientSoftClose = false;                       // :518

            nClientLogoutDelay = 0;                          // :520
            boDelayClientLogout = false;                     // :521
            dwDelayClientLogoutTick = MyGetTickCount();      // :522

            boValidClose = false;                            // :524
            nClientCloseDelay = 0;                           // :525
            boDelayClientClose = false;                      // :526
            dwDelayClientCloseTick = MyGetTickCount();       // :527

            FLastAction = TBaseAction.baOther;               // :529
            GameSpeed.Clear();                               // :530 FillChar(GameSpeed, SizeOf(GameSpeed), 0)

            LastLockAntiPlugActionMode = TAntiPlugActionMode.amHit;  // :532

            Array.Clear(dwCollectIntervalArr, 0, dwCollectIntervalArr.Length);          // :534 FillChar
            Array.Clear(nCollectIntervalIndexArr, 0, nCollectIntervalIndexArr.Length);  // :535 FillChar

            Array.Clear(RecordActionArr, 0, RecordActionArr.Length);                    // :537 FillChar
            nRecordActionIndex = 0;                                                     // :538

            Array.Clear(SumSpeedProcessArr, 0, SumSpeedProcessArr.Length);              // :540 FillChar

            // :542 `for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do`
            for (int modeIndex = (int)LowAntiPlugActionMode; modeIndex <= (int)HighAntiPlugActionMode; modeIndex++)
            {
                nCompensationArr[modeIndex] = 0;             // :544
            }

            FLastSendMyHeartbeatTick = MyGetTickCount();     // :547

            // {$IF CLIENT_ANTIPLUG = 1}  原文 :549-566
            Randomize();                                     // :550

            dwClientAntiPlugVersion = 0;                     // :552

            wSendLoadAntiPlugCode = 0;                       // :554
            boSendLoadAntiPlug = false;                      // :555
            nSendLoadAntiPlugIndex = 0;                      // :556
            boSendLoadAntiPlugFinished = false;              // :557
            dwSendLoadAntiPlugTick = MyGetTickCount();       // :558
            boRecvLoadAntiPlug = false;                      // :559
            dwRecvLoadAntiPlugTick = MyGetTickCount();       // :560

            boWaitLoadAntiPlug = false;                      // :562
            dwWaitLoadAntiPlugTick = MyGetTickCount();       // :563

            dwRecvClientAntiplugCRC = 0;                     // :565

            nIllegalPacketCount = 0;                         // :568

            Array.Clear(SendProcessBlacklistMD5, 0, SendProcessBlacklistMD5.Length);    // :570 FillChar
            dwSendProcessBlacklistTick = MyGetTickCount();   // :571

            dwLastRungateDoorTick = MyGetTickCount() - 30;   // :573

            boSendCheckCode = false;                         // :575
            dwSendCheckCode = 0;                             // :576
            dwSendCheckTick = MyGetTickCount();              // :577
            boRecvCheckCodeOK = false;                       // :578

            nClientSendDate = 0;                             // :580
            wClinetSendHour = 0;                             // :581
            nClientSendRunGateIP = 0;                        // :582
            dwClientSendDateTick = MyGetTickCount();         // :583

            boDelayClose = false;                            // :585
            dwDelayCloseTick = MyGetTickCount();             // :586

            // {$IF MultiThreadRunContext <> 0}  原文 :588-590
            RunThreadIndex = Random(MirClientContextConst.MIRCONTEXT_RUN_THREAD_COUNT);  // :589
        }

        // {$IF CLIENT_ANTIPLUG = 1}  原文 :593-603
        ContextDataLocker.Lock();                            // :594
        try
        {
            if (pContextData != null && nContextDataLen > 0)  // :596
            {
                Array.Clear(pContextData, 0, (int)nContextDataLen);  // :598 FillChar(pContextData^, nContextDataLen, 0)
            }
        }
        finally
        {
            ContextDataLocker.UnLock();                      // :601
        }

        boSendGmClose = true;                                // :605
        ClearClientMsgList();                                // :606

        ProcessList.Lock();                                  // :608
        try
        {
            ProcessList.Clear();                             // :610
        }
        finally
        {
            ProcessList.UnLock();                            // :612
        }

        MagicUseTickList.Lock();                             // :615
        try
        {
            MagicUseTickList.Clear();                        // :617
        }
        finally
        {
            MagicUseTickList.UnLock();                       // :619
        }

        Array.Clear(MagicCDSpeed, 0, MagicCDSpeed.Length);   // :622 FillChar
        MagicCDSpeedIndex = 0;                               // :623
        MagicCDSpeedCount = 0;                               // :624

        LastEatingItemTick = 0;                              // :626
        LastHeroEatingItemTick = 0;                          // :627

        HumBagItems.Lock();                                  // :629
        try
        {
            HumBagItems.Clear();                             // :631
        }
        finally
        {
            HumBagItems.UnLock();                            // :633
        }

        HeroBagItems.Lock();                                 // :636
        try
        {
            HeroBagItems.Clear();                            // :638
        }
        finally
        {
            HeroBagItems.UnLock();                           // :640
        }

        FScreenshotStream.Lock();                            // :643
        try
        {
            FScreenshotStream.ClearStream();                       // :645
        }
        finally
        {
            FScreenshotStream.UnLock();                      // :647
        }

        FClientResponseFileStream.Lock();                    // :650
        try
        {
            FClientResponseFileStream.ClearStream();               // :652
        }
        finally
        {
            FClientResponseFileStream.UnLock();              // :654
        }

        FClientResponseFileTick = 0;                         // :657
        FClientResponseFileIndex = 0;                        // :658
        FClientResponseFileCount = 0;                        // :659

        FServerMsgLocker.Lock();                             // :661
        try
        {
            FServerMsgStr = Array.Empty<byte>();             // :663
        }
        finally
        {
            FServerMsgLocker.UnLock();                       // :665
        }

        // 原文 :668 —— g_dwVerifyCodeInterval1/2 是**分钟**，乘 60000 得毫秒。
        // 原文缺陷（保留）：Interval2 < Interval1 时 `(2-1)*60000` 为负 → Delphi Random(负数)
        // 在 Range <= 0 时返回 0；此处 Random 接缝同样护栏（返回 0），语义一致。
        dwVerifyInterval = g_dwVerifyCodeInterval1 * 60000 +
            (uint)Random(unchecked((int)((g_dwVerifyCodeInterval2 - g_dwVerifyCodeInterval1) * 60000)));

        boSendVerifyCode = false;                            // :670
        dwSendVerifyCodeTick = MyGetTickCount();             // :671
        nVerifyCodeErrCount = 0;                             // :672
        nVerifyCodeRefreshCount = 0;                         // :673
        sVerifyCode = "";                                    // :674
        nVerifySuccessCount = 0;                             // :675
        boVerifyDisableAttack = false;                       // :676
        boEnableClientUploadPickItems = false;               // :677
        dwClientUploadPickItemsTick = 0;                     // :678
    }

    // =================================================================================
    // 原文 :1888-1901  function GetExVersionNO(nVersionDate: Integer; var nOldVerstionDate: Integer): Integer;
    // 原文缺陷（保留）：:1897 `Inc(Result, 100000000)` 在 nVersionDate 极大时整数溢出 → 照抄，
    // 由 MirClientContextTests 钉死边界（unchecked 回绕）。
    // =================================================================================
    public static int GetExVersionNO(int nVersionDate, out int nOldVerstionDate)
    {
        int Result = 0;                                      // :1890
        nOldVerstionDate = 0;                                // :1891
        if (nVersionDate > 100000000)                        // :1892
        {
            while (nVersionDate > 100000000)                 // :1894
            {
                nVersionDate -= 100000000;                   // :1896
                Result = unchecked(Result + 100000000);      // :1897
            }
        }
        nOldVerstionDate = nVersionDate;                     // :1900
        return Result;
    }

    /// <summary>原文调用点（:2211）只用返回值。</summary>
    public static int GetExVersionNO(int nVersionDate) => GetExVersionNO(nVersionDate, out _);

    // =================================================================================
    // 原文 :1904-1918  function TMirClientContext.CheckRecvPacketSize(
    //                    const PacketLen, MaxLen, MsgIdent: Integer): Boolean;
    // case 无 else：g_BlockMethod 为 bmDisconnect 时两个分支都不执行（原文如此）。
    // =================================================================================
    public bool CheckRecvPacketSize(int PacketLen, int MaxLen, int MsgIdent)
    {
        bool Result = true;                                  // :1906
        // 如果客户端发来的包超过限定的包长度
        if (PacketLen > MaxLen && g_boKickOverPacketSize)    // :1908
        {
            switch (g_BlockMethod)                           // :1910
            {
                case TBlockIPMethod.bmTempBlock:             // :1911
                    AddTempBlockIP(RemoteAddr);
                    break;
                case TBlockIPMethod.bmBlockList:             // :1912
                    AddBlockIP(RemoteAddr);
                    break;
                // 无 else（原文如此，MirClientContext.pas:1910-1913）
            }
            AddMainLogMsg(Format("数据超长，踢除连接; 长度:%d; 用户:%s; IP:%s; 错误代码:%d",
                PacketLen, sChrName, RemoteAddr, MsgIdent), 1);   // :1914
            DelayClose(100);                                 // :1915  // Close;
            Result = false;                                  // :1916
        }
        return Result;
    }

    // =================================================================================
    // 原文 :2947-3020  procedure TMirClientContext.ProcessClientMessage(DefMsg; DefMsgData);
    // 原文缺陷（保留）：:2990 的判定在锁内、:3006 的判定在锁外；若 :2957 的条件不成立，
    //   ClientPacketCount 保持 0 → :3006 恒假（客户端包堆积检测被整体跳过）。照抄。
    // =================================================================================
    public void ProcessClientMessage(in TDefaultMessage DefMsg, byte[] DefMsgData)
    {
        if (boDelayClose) return;                            // :2953
        if (IsPostedCloseQuest) return;                      // :2954
        List<string> ClientList = null;                      // :2955
        int ClientPacketCount = 0;                           // :2956
        if (MyGetTickCount() > FDelayTick + 50)              // :2957
        {
            FClientMsgList.Lock();                           // :2959
            try
            {
                TClientMsg ClientMsg = new TClientMsg();     // :2961 GetMem(ClientMsg, SizeOf(TClientMsg))
                FClientMsgList.Add(ClientMsg);               // :2962
                ClientMsg.DefMessage = DefMsg;               // :2963
                ClientMsg.dwTimeTick = MyGetTickCount();     // :2964
                ClientMsg.boDelay = false;                   // :2965
                ClientMsg.pBuffer = null;                    // :2966
                ClientMsg.nBufferLen = 0;                    // :2967
                if (DefMsgData != null && DefMsgData.Length > 0)   // :2968 Length(DefMsgData) > 0
                {
                    ClientMsg.pBuffer = new byte[DefMsgData.Length + 1];     // :2970 GetMem(..., Length+1)
                    AnsiBufferSeam.Move(DefMsgData, 0, ClientMsg.pBuffer, 0, DefMsgData.Length);  // :2971
                    ClientMsg.nBufferLen = DefMsgData.Length + 1;             // :2972

                    ClientMsg.pBuffer[DefMsgData.Length] = 0;                 // :2974 PChar(...+Length)^ := #0
                }

                // {$IF CLIENT_ANTIPLUG = 1}  原文 :2978-2989（排除掉客户端插件发来的数据）
                for (int I = 0; I < FClientMsgList.Count; I++)   // :2979
                {
                    ClientMsg = (TClientMsg)FClientMsgList[I];    // :2981
                    if (ClientMsg.DefMessage.Ident != 41002)      // :2982
                    {
                        ClientPacketCount++;                      // :2984
                    }
                }
                if (ClientPacketCount >= g_nMaxClientPacketCount && g_boKickOverPacketSize)  // :2990
                {
                    ClientList = new List<string>();             // :2991
                    int nMin = Math.Min(100, ClientPacketCount); // :2992
                    for (int I = 0; I < nMin; I++)               // :2993
                    {
                        ClientMsg = (TClientMsg)FClientMsgList[I];                     // :2995
                        ClientList.Add(Format("%d-%d-%d-%d-%d",                         // :2996
                            ClientMsg.DefMessage.Recog, ClientMsg.DefMessage.Ident,
                            ClientMsg.DefMessage.Param, ClientMsg.DefMessage.Tag,
                            ClientMsg.DefMessage.Series));
                    }
                }
            }
            finally
            {
                FClientMsgList.UnLock();                     // :3002
            }
        }

        if (ClientPacketCount >= g_nMaxClientPacketCount && g_boKickOverPacketSize)  // :3006
        {
            if (ClientList != null)                          // :3008
            {
                for (int I = 0; I < ClientList.Count; I++)   // :3009
                {
                    ClientList[I] = Format("输出日志[%d]:DefMessage:%s;", I, ClientList[I]);  // :3011
                }
                ClientList.Add("堆积消息数量:" + IntToStr(ClientPacketCount));               // :3013
                // :3014 ClientList.SaveToFile(Format('.\log\%s-%s.txt',[sChrName,FormatDateTime('yyyy-mm-dd', Now)]))
                ClientList = null;                           // :3015 FreeAndNil(ClientList)
            }
            AddMainLogMsg(Format("收到客户端的包堆积太多，断开连接; 包数量:%d; 用户:%s; IP:%s",
                ClientPacketCount, sChrName, RemoteAddr), 0);   // :3017
            Close();                                         // :3018
        }
    }

    // =================================================================================
    // 原文 :3022-3054  procedure TMirClientContext.DelayClientMessage(Msg: PProcessMsg; DelayTime: DWORD);
    // =================================================================================
    public void DelayClientMessage(TProcessMsg Msg, uint DelayTime)
    {
        FClientMsgList.Lock();                               // :3027
        try
        {
            for (int I = 0; I < FClientMsgList.Count; I++)   // :3029
            {
                TClientMsg ClientMsg = (TClientMsg)FClientMsgList[I];   // :3031
                ClientMsg.dwTimeTick += DelayTime;           // :3032
            }

            TClientMsg NewMsg = new TClientMsg();            // :3035 GetMem(ClientMsg, SizeOf(TClientMsg))
            FClientMsgList.Insert(0, NewMsg);                // :3036
            NewMsg.DefMessage = Msg.DefMessage;              // :3037
            NewMsg.dwTimeTick = MyGetTickCount() + DelayTime;// :3038
            NewMsg.boDelay = true;                           // :3039
            NewMsg.pBuffer = null;                           // :3040
            NewMsg.nBufferLen = 0;                           // :3041
            if (Msg.sMessage != null && Msg.sMessage.Length > 0)   // :3042
            {
                NewMsg.nBufferLen = Msg.sMessage.Length + 1;                  // :3044
                NewMsg.pBuffer = new byte[NewMsg.nBufferLen];                 // :3045 GetMem(..., nBufferLen)
                AnsiBufferSeam.Move(Msg.sMessage, 0, NewMsg.pBuffer, 0, Msg.sMessage.Length);  // :3046
                NewMsg.pBuffer[Msg.sMessage.Length] = 0;                      // :3047
            }
        }
        finally
        {
            FClientMsgList.UnLock();                         // :3050
        }

        FDelayTick = MyGetTickCount() + DelayTime;           // :3053
    }

    // =================================================================================
    // 原文 :3056-3078  procedure TMirClientContext.ClearClientMsgList;
    // =================================================================================
    public void ClearClientMsgList()
    {
        FClientMsgList.Lock();                               // :3061
        try
        {
            for (int I = FClientMsgList.Count - 1; I >= 0; I--)   // :3063
            {
                TClientMsg ClientMsg = (TClientMsg)FClientMsgList[I];   // :3065
                if (ClientMsg.pBuffer != null && ClientMsg.nBufferLen > 0)   // :3066
                {
                    ClientMsg.pBuffer = null;                // :3068 FreeMem(ClientMsg.pBuffer, ClientMsg.nBufferLen)
                }
                // :3071 FreeMem(ClientMsg)
            }

            FClientMsgList.Clear();                          // :3074
        }
        finally
        {
            FClientMsgList.UnLock();                         // :3076
        }
    }

    // =================================================================================
    // 原文 :3080-3120  function TMirClientContext.GetClientMessage(Msg: PProcessMsg): Boolean;
    // =================================================================================
    public bool GetClientMessage(TProcessMsg Msg)
    {
        bool Result = false;                                 // :3084

        FClientMsgList.Lock();                               // :3086
        try
        {
            while (FClientMsgList.Count > 0)                 // :3088
            {
                TClientMsg ClientMsg = (TClientMsg)FClientMsgList[0];   // :3090
                if (ClientMsg == null)                       // :3091
                {
                    FClientMsgList.Delete(0);                // :3093
                    continue;                                // :3094
                }

                FClientMsgList.Delete(0);                    // :3097
                Msg.DefMessage = ClientMsg.DefMessage;       // :3098
                Msg.dwTimeTick = ClientMsg.dwTimeTick;       // :3099
                Msg.boDelay = ClientMsg.boDelay;             // :3100

                if (ClientMsg.pBuffer != null && ClientMsg.nBufferLen > 0)   // :3102
                {
                    Msg.sMessage = new byte[ClientMsg.nBufferLen - 1];       // :3104 SetLength(Msg.sMessage, nBufferLen-1)
                    AnsiBufferSeam.Move(ClientMsg.pBuffer, 0, Msg.sMessage, 0, ClientMsg.nBufferLen - 1);  // :3105
                    ClientMsg.pBuffer = null;                                // :3106 FreeMem(ClientMsg.pBuffer, nBufferLen)
                }
                else
                {
                    Msg.sMessage = Array.Empty<byte>();                      // :3110
                }
                // :3112 FreeMem(ClientMsg, SizeOf(TClientMsg))

                Result = true;                               // :3114
                break;                                       // :3115
            }
        }
        finally
        {
            FClientMsgList.UnLock();                         // :3118
        }
        return Result;
    }

    // =================================================================================
    // 原文 :3123-3143  procedure SendMessaggeToClient(Msg; MsgType; btFColor, btBColor);
    // 注意原文方法名拼写就是三个 g（Messagge）—— 保留。
    // =================================================================================
    public void SendMessaggeToClient(string Msg, int MsgType, byte btFColor, byte btBColor)
    {
        if (AnsiLen(Msg) == 0) return;                       // :3128 Length(Msg) = 0

        byte[] sSendText;
        TDefaultMessage DefMsg;
        if (MsgType == 0)                                    // :3130
        {
            DefMsg = MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord(btFColor, btBColor), 0, 1);  // :3132
            sSendText = EncodeRunGateMsg(DefMsg, GbkBytes(Msg), AnsiLen(Msg));              // :3133
        }
        else
        {
            DefMsg = MakeDefaultMsg(SM_MENU_OK, 0, 0, 0, 0);                                // :3137
            sSendText = EncodeString(Msg);                                                  // :3138
            sSendText = EncodeRunGateMsg(DefMsg, sSendText, sSendText.Length);               // :3139
        }

        PostSendTextBytes(sSendText);                        // :3142
    }

    // =================================================================================
    // 原文 :3146-3177  procedure SendMessageToServer(DefMsg: TDefaultMessage; Msg: string);
    // Msg 是**原始二进制**（AnsiString 承载）→ C# byte[]。
    // =================================================================================
    public void SendMessageToServer(in TDefaultMessage DefMsg, byte[] Msg)
    {
        TRunGate RunGate = GetRunGateAsTRunGate();           // :3153-3158

        if (RunGate != null)                                 // :3160
        {
            if (Msg == null || Msg.Length == 0)              // :3162 Length(Msg) = 0
            {
                RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                    StructBytes.BytesOf(DefMsg), TDefaultMessage.SizeOf);          // :3164
            }
            else
            {
                int Len = TDefaultMessage.SizeOf + Msg.Length;                     // :3168
                byte[] S = new byte[Len];                                          // :3169 SetLength(S, Len)

                AnsiBufferSeam.Move(StructBytes.BytesOf(DefMsg), 0, S, 0, TDefaultMessage.SizeOf);   // :3171
                AnsiBufferSeam.Move(Msg, 0, S, TDefaultMessage.SizeOf, Msg.Length);                  // :3172

                RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex, S, Len); // :3174
            }
        }
    }

    // 原文 :3179-3210  procedure SendMessageToServer(DefMsg: TDefaultMessage; Msg: PAnsiChar; MsgLen: Integer);
    public void SendMessageToServer(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen)
    {
        TRunGate RunGate = GetRunGateAsTRunGate();           // :3186-3191

        if (RunGate != null)                                 // :3193
        {
            if (MsgLen == 0)                                 // :3195
            {
                RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                    StructBytes.BytesOf(DefMsg), TDefaultMessage.SizeOf);          // :3197
            }
            else
            {
                int Len = TDefaultMessage.SizeOf + MsgLen;                         // :3201
                byte[] S = new byte[Len];                                          // :3202

                AnsiBufferSeam.Move(StructBytes.BytesOf(DefMsg), 0, S, 0, TDefaultMessage.SizeOf);   // :3204
                AnsiBufferSeam.Move(Msg, 0, S, TDefaultMessage.SizeOf, MsgLen);                      // :3205

                RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex, S, Len); // :3207
            }
        }
    }

    // 原文 :3212-3228  procedure SendMessageToServer(Msg: PAnsiChar; MsgLen: Integer);
    public void SendMessageToServer(byte[] Msg, int MsgLen)
    {
        TRunGate RunGate = GetRunGateAsTRunGate();           // :3217-3222

        if (RunGate != null)                                 // :3224
        {
            RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex, Msg, MsgLen); // :3226
        }
    }

    // =================================================================================
    // 原文 :3230-3238  procedure TMirClientContext.SendActionRet(IsGood: Boolean);
    // =================================================================================
    public void SendActionRet(bool IsGood)
    {
        TDefaultMessage DefMsg = MakeDefaultMsg(SM_ACTION_RET, 0, (ushort)(IsGood ? 1 : 0), 0, 0);   // :3235 Integer(IsGood)
        byte[] sSendText = EncodeRunGateMsg(DefMsg, null, 0);                              // :3236
        PostSendTextBytes(sSendText);                                                      // :3237
    }

    // =================================================================================
    // 原文 :3241-3262  procedure TMirClientContext.LockUser(LockTime: Integer);
    // =================================================================================
    public void LockUser(int LockTime)
    {
        if (LockTime == 0) return;                           // :3246

        // 向客户端发送锁定消息
        string sSendText = DecodeString(StringReplace(g_Config.sShowLockMsg, "%d", IntToStr(LockTime), false)); // :3249 StringReplace(..., [])
        TDefaultMessage DefMsg = MakeDefaultMsg(SM_LOCK_USER, 0, 1, 0, 0);              // :3250
        byte[] enc = EncodeRunGateMsg(DefMsg, GbkBytes(sSendText), AnsiLen(sSendText)); // :3251
        PostSendTextBytes(enc);                                                         // :3252

        // 记录日志
        if (g_Config.boShowLockLog)                          // :3255
        {
            AddMainLogMsg(Format("角色【%s】被锁定%d秒", sChrName, LockTime), 2);         // :3257
        }

        dwUnLockTick = MyGetTickCount() + (uint)(LockTime * 1000);   // :3260
        boLocked = true;                                     // :3261
    }

    // =================================================================================
    // 原文 :3265-3279  procedure TMirClientContext.UnLockUser;
    // =================================================================================
    public void UnLockUser()
    {
        if (boLocked)                                        // :3270
        {
            boLocked = false;                                // :3272
            TDefaultMessage DefMsg = MakeDefaultMsg(SM_LOCK_USER, 0, 0, 0, 0);          // :3273
            byte[] sSendText = EncodeRunGateMsg(DefMsg, null, 0);                        // :3274
            PostSendTextBytes(sSendText);                                                // :3275

            MirClientContextUnit.UpdateLockUserList(this);   // :3277 UpdateLockUserList(Self)
        }
    }

    // =================================================================================
    // 原文 :3281-3408  function TMirClientContext.FilterSayMsg(var sMsg: string): Boolean;
    // =================================================================================
    public bool FilterSayMsg(ref string sMsg)
    {
        bool Result = false;                                 // :3294
        if (sMsg == "OoOoOoOoOoQ")                           // :3295
        {
            // CloseAllUser();                               // :3297 原文如此（该行已被注释）
        }

        if (g_boSayMsgControl)                               // :3300
        {
            uint CurTick = MyGetTickCount();                 // :3302

            // { 禁言时间内 }                                 // :3304
            if (CurTick < dwDisableSayMsgTick)               // :3305
            {
                sMsg = "";                                   // :3307
                Result = true;                               // :3308
                SendWarnMsg(g_sDisableSayMsg);               // :3309
                return Result;                               // :3310
            }

            if (unchecked(CurTick - dwSayMsgTick) < g_dwSayTime)   // :3313
            {
                if (dwSayMsgCount >= g_dwSayMaxCount)        // :3315
                {
                    dwDisableSayMsgTick = CurTick + g_dwSayDisableTime * 1000;                      // :3317
                    sMsg = "";                                                                       // :3318
                    Result = true;                                                                   // :3319
                    string S = StringReplace(g_sDisableSayMsgBegin, "%d", IntToStr((int)g_dwSayDisableTime), true);  // :3320 [rfIgnoreCase]
                    SendWarnMsg(S);                                                                  // :3321
                    return Result;                                                                   // :3322
                }
                else
                    dwSayMsgCount++;                         // :3325
            }
            else
                dwSayMsgCount = 0;                           // :3328

            dwSayMsgTick = CurTick;                          // :3330

            // { 发言长度 }                                   // :3332
            // 原文 :3333-3334 —— 判据与截断都是 **AnsiString 字节**口径（GBK 下 != 字符数）
            if (g_dwSayMaxLen > 0 && (uint)AnsiLen(sMsg) > g_dwSayMaxLen)
                sMsg = AnsiCopyPrefix(sMsg, (int)g_dwSayMaxLen);              // :3334 Copy(sMsg, 1, g_dwSayMaxLen)
        }

        // { 发言文字过滤 }                                    // :3337
        if (!g_boFilterSayMsg) return Result;                // :3338

        // {$IFDEF CUSTOM_VER} —— 未定义 → 死代码                // :3340-3342

        g_WordFilterList.Lock();                             // :3344
        try
        {
            for (int I = 0; I < g_WordFilterList.Count; I++) // :3346
            {
                string sFilterText = g_WordFilterList[I];    // :3348
                string sReplaceText = "";                    // :3349
                if (AnsiContainsText(sMsg, sFilterText))     // :3350
                {
                    TRunGate RunGate = GetRunGateAsTRunGate();   // :3352-3357

                    if (RunGate != null && g_FilterSayMsgMode != TFilterSayMsgMode.fsmmClose &&
                        g_boFilterSayTriggerScript)          // :3359-3360
                    {
                        TDefaultMessage DefMsg = MakeDefaultMsg(CM_RUNGATE_SENDFILTERMSG, 0, 0, 0, 0);   // :3364
                        RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                            StructBytes.BytesOf(DefMsg), TDefaultMessage.SizeOf);                        // :3366
                    }

                    bool breakFilterLoop = false;
                    switch (g_FilterSayMsgMode)              // :3370
                    {
                        case TFilterSayMsgMode.fsmmAllBlock:         // :3371
                            sMsg = g_WarnSayMsg;                     // :3373
                            if (AnsiLen(g_WarnSayMsg) == 0) Result = true;   // :3374
                            breakFilterLoop = true;                  // :3375 Break
                            break;
                        case TFilterSayMsgMode.fsmmSelfBolck:        // :3377
                            sReplaceText = StringOfChar(g_sReplaceWord, AnsiLen(sFilterText));  // :3379
                            sMsg = AnsiReplaceText(sMsg, sFilterText, sReplaceText);            // :3380
                            break;
                        case TFilterSayMsgMode.fsmmClose:            // :3382
                            Close();                                 // :3384
                            Result = true;                           // :3385
                            breakFilterLoop = true;                  // :3386
                            break;
                        case TFilterSayMsgMode.fsmmDisMsg:           // :3388
                            sMsg = "";                               // :3390
                            Result = true;                           // :3391
                            breakFilterLoop = true;                  // :3392
                            break;
                        case TFilterSayMsgMode.fsmmDisMsgorSys:      // :3394
                            sMsg = "";                               // :3396
                            Result = true;                           // :3397
                            if (AnsiLen(g_WarnSayMsg) > 0)           // :3398
                                SendWarnMsg(g_WarnSayMsg);           // :3399
                            breakFilterLoop = true;                  // :3400
                            break;
                    }
                    if (breakFilterLoop) break;
                }
            }
        }
        finally
        {
            g_WordFilterList.UnLock();                       // :3406
        }
        return Result;
    }

    // =================================================================================
    // 原文 :3410-3416  function GetSpeedText(Value: Integer): string;
    // =================================================================================
    public static string GetSpeedText(int Value)
    {
        if (Value < 0)                                       // :3412
            return IntToStr(Value);                          // :3413
        else
            return "+" + IntToStr(Value);                    // :3415
    }

    // =================================================================================
    // 原文 :3418-3455  procedure TMirClientContext.ContinuousSpeed(ActionMode; Interval);
    // 原文用 AntiPlugActionModeNames_3 / AntiPlugActionModeNames（带下划线后缀）；
    // uFrmGameSpeedLogic.cs 的 RunGateConst 里对应成员名是 AntiPlugActionModeNames3 /
    // AntiPlugActionModeNames（去掉了下划线）—— 已核对内容与顺序一致（uFrmGameSpeedLogic.cs:262/298）。
    // =================================================================================
    public void ContinuousSpeed(TAntiPlugActionMode ActionMode, uint Interval)
    {
        switch (ActionMode)                                  // :3421
        {
            case TAntiPlugActionMode.amWalk:
            case TAntiPlugActionMode.amRun:                  // :3422
                AddMainLogMsg(Format("【速度异常】%s:%d; [移动速度%s]; 用户:%s",
                    AntiPlugActionModeNames3[(int)ActionMode], Interval, GetSpeedText(nMoveSpeed), sChrName), 0);  // :3424-3426
                break;
            case TAntiPlugActionMode.amHit:                  // :3428
                AddMainLogMsg(Format("【速度异常】%s:%d; [攻击速度%s]; 用户:%s",
                    AntiPlugActionModeNames[(int)ActionMode], Interval, GetSpeedText(nAttackSpeed), sChrName), 0); // :3430-3434
                break;
            case TAntiPlugActionMode.amSpell:                // :3436
                AddMainLogMsg(Format("【速度异常】%s:%d; [魔法速度%s]; 用户:%s",
                    AntiPlugActionModeNames3[(int)ActionMode], Interval, GetSpeedText(nSpellSpeed), sChrName), 0); // :3438-3442
                break;
            default:                                         // :3444-3450
                AddMainLogMsg(Format("【速度异常】%s:%d; 用户:%s",
                    AntiPlugActionModeNames3[(int)ActionMode], Interval, sChrName), 0);
                break;
        }

        SendMessaggeToClient("请关闭非法外挂后重新登陆!", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :3453
        DelayClose(1000);                                    // :3454
    }

    // =================================================================================
    // 原文 :3457-3463  procedure TMirClientContext.ProcessAssasinate;   // 检查到暗杀
    // =================================================================================
    public void ProcessAssasinate()
    {
        AddMainLogMsg(Format("【速度异常】; 用户:%s", sChrName), 0);   // :3459

        SendMessaggeToClient("请关闭非法外挂后重新登陆!", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :3461
        DelayClose(100);                                     // :3462
    }

    // =================================================================================
    // 原文 :9689-9708  function GetConcurrentPacketCount(DefMsg: pTDefaultMessage): Integer;
    // =================================================================================
    public int GetConcurrentPacketCount(in TDefaultMessage DefMsg)
    {
        int Result = 0;                                      // :9694
        FClientMsgList.Lock();                               // :9695
        try
        {
            for (int I = 0; I < FClientMsgList.Count; I++)   // :9697
            {
                TClientMsg ClientMsg = (TClientMsg)FClientMsgList[I];   // :9699
                if (ClientMsg != null && ClientMsg.DefMessage.Ident == DefMsg.Ident)   // :9700
                {
                    Result++;                                // :9702
                }
            }
        }
        finally
        {
            FClientMsgList.UnLock();                         // :9706
        }
        return Result;
    }

    // =================================================================================
    // 原文 :9710-9735  function ClearConcurrentPacket(DefMsg: pTDefaultMessage): Integer;
    // 原文缺陷（保留并注释）：
    //   * :9715 `Result := 0;` 之后**从未对 Result 赋值**，恒返回 0（与 GetConcurrentPacketCount 不对称）；
    //   * :9721 对 ClientMsg **没有 nil 检查**（GetConcurrentPacketCount :9700 有）；
    //   * :9725 释放长度用 `nBufferLen + 1`，而分配长度是 `nBufferLen`（多算 1 字节）。
    // =================================================================================
    public int ClearConcurrentPacket(in TDefaultMessage DefMsg)
    {
        int Result = 0;                                      // :9715
        FClientMsgList.Lock();                               // :9716
        try
        {
            for (int I = FClientMsgList.Count - 1; I >= 0; I--)   // :9718
            {
                TClientMsg ClientMsg = (TClientMsg)FClientMsgList[I];   // :9720
                // 原文如此（MirClientContext.pas:9721）：此处不做 nil 检查
                if (ClientMsg.DefMessage.Ident == DefMsg.Ident)  // :9721
                {
                    if (ClientMsg.pBuffer != null && ClientMsg.nBufferLen > 0)   // :9723
                    {
                        ClientMsg.pBuffer = null;             // :9725 FreeMem(ClientMsg.pBuffer, ClientMsg.nBufferLen + 1)
                    }
                    // :9727 FreeMem(ClientMsg)

                    FClientMsgList.Delete(I);                 // :9729
                }
            }
        }
        finally
        {
            FClientMsgList.UnLock();                         // :9733
        }
        return Result;
    }

    // =================================================================================
    // 原文 :9737-9745  procedure TMirClientContext.SendWarnMsg(WarnMsg: string);
    // 颜色固定为 MakeWord($38, $FF)。
    // =================================================================================
    public void SendWarnMsg(string WarnMsg)
    {
        TDefaultMessage DefMsg = MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord((byte)0x38, (byte)0xFF), 0, 1);  // :9742
        byte[] sSendText = EncodeRunGateMsg(DefMsg, GbkBytes(WarnMsg), AnsiLen(WarnMsg));        // :9743
        PostSendTextBytes(sSendText);                                                            // :9744
    }

    // =================================================================================
    // 原文 :9747-9753  function TMirClientContext.GetRunGate: TObject;
    // =================================================================================
    public object GetRunGate()
    {
        if (IocpCore != null && IocpCore.Owner != null && IocpCore.Owner is TIocpTcpServer)  // :9749
            return ((TIocpTcpServer)IocpCore.Owner).BindObject;                              // :9750
        else
            return null;                                     // :9752
    }

    /// <summary>原文各处重复的取用模式（:715-719 / :2005-2009 / :3153-3158 / …）：
    /// <c>if (RunGateObj &lt;&gt; nil) and (RunGateObj is TRunGate) then RunGate := TRunGate(RunGateObj);</c>
    /// </summary>
    public TRunGate GetRunGateAsTRunGate() => GetRunGate() as TRunGate;

    // =================================================================================
    // 原文 :9755-9800  procedure AddServerMsg(DefMsg: PTDefaultMessage; DataAdd; DataAddLen);
    // ErrorNum 是原文的"断点标记"，只用于异常日志，保留。
    // =================================================================================
    public void AddServerMsg(in TDefaultMessage DefMsg, byte[] DataAdd, uint DataAddLen)
    {
        // 2019-03-21 20:55:46
        if (IsPostedCloseQuest || IsWaitingGiveBack) return;  // :9762

        byte[] S = EncodeRunGateMsg(DefMsg, DataAdd, (int)DataAddLen);   // :9764

        int ErrorNum = 0;                                    // :9766
        try
        {
            bool IsCloseContext = false;                     // :9768

            ErrorNum = 1;                                    // :9770
            FServerMsgLocker.Lock();                         // :9771
            try
            {
                ErrorNum = 2;                                // :9773
                // 原文 :9774 —— `if Length(FServerMsgStr) + Length(S) >= g_dwClientAccumulateMaxSize shl 10{500K} then`
                // Delphi 里 shl 优先级低于 +，等价于 `(Len + Len) >= (g_dw... shl 10)`。
                if (FServerMsgStr.Length + S.Length >= g_dwClientAccumulateMaxSize << 10)   // :9774
                {
                    ErrorNum = 3;                            // :9776
                    AddMainLogMsg(Format("发往客户端的包堆积太多，断开连接; 用户:%s; IP:%s", sChrName, RemoteAddr), 0);  // :9777
                    IsCloseContext = true;                   // :9778
                }
                else
                {
                    ErrorNum = 4;                            // :9782
                    FServerMsgStr = Concat(FServerMsgStr, S); // :9783
                }
            }
            finally
            {
                FServerMsgLocker.UnLock();                   // :9786
            }

            if (IsCloseContext)                              // :9789
            {
                ErrorNum = 5;                                // :9791
                CloseContextSocket(0 /*ERROR_SUCCESS*/, TCloseFrom.cfOther);  // :9792
            }
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.AddServerMsg, ErrorNum = " + IntToStr(ErrorNum) + ", " + E.Message, 0);  // :9797
        }
    }

    // =================================================================================
    // 原文 :9802-9844  procedure AddServerText(const S: string);
    // 原文缺陷（保留）：except 里的日志文字仍是 "AddServerMsg"（从 AddServerMsg 复制粘贴遗留）。
    // =================================================================================
    public void AddServerText(byte[] S)
    {
        // 2019-03-21 20:55:46
        if (IsPostedCloseQuest || IsWaitingGiveBack) return;  // :9808

        int ErrorNum = 0;                                    // :9810
        try
        {
            bool IsCloseContext = false;                     // :9812

            ErrorNum = 1;                                    // :9814
            FServerMsgLocker.Lock();                         // :9815
            try
            {
                ErrorNum = 2;                                // :9817
                if (FServerMsgStr.Length + S.Length >= g_dwClientAccumulateMaxSize << 10)   // :9818
                {
                    ErrorNum = 3;                            // :9820
                    AddMainLogMsg(Format("发往客户端的包堆积太多，断开连接; 用户:%s; IP:%s", sChrName, RemoteAddr), 0);  // :9821
                    IsCloseContext = true;                   // :9822
                }
                else
                {
                    ErrorNum = 4;                            // :9826
                    FServerMsgStr = Concat(FServerMsgStr, S); // :9827
                }
            }
            finally
            {
                FServerMsgLocker.UnLock();                   // :9830
            }

            if (IsCloseContext)                              // :9833
            {
                ErrorNum = 5;                                // :9835
                CloseContextSocket(0, TCloseFrom.cfOther);    // :9836
            }
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.AddServerMsg, ErrorNum = " + IntToStr(ErrorNum) + ", " + E.Message, 0);  // :9841
        }
    }

    // =================================================================================
    // 原文 :9846-9881  procedure TMirClientContext.CloseContextSocket(ErrCode; CloseFrom);
    // =================================================================================
    protected override void CloseContextSocket(int ErrCode, TCloseFrom CloseFrom)
    {
        TRunGate RunGate = GetRunGateAsTRunGate();           // :9852-9857

        if (RunGate == null) return;                         // :9859

        RunGate.OnlineUser.Lock();                           // :9861
        try
        {
            RunGate.OnlineUser.Remove(this);                 // :9863
        }
        finally
        {
            RunGate.OnlineUser.UnLock();                     // :9865
        }

        if (ErrCode != 0 /*ERROR_SUCCESS*/)                  // :9868
        {
            string sCloseFrom;
            switch (CloseFrom)                               // :9870
            {
                case TCloseFrom.cfPostWSASendCache1: sCloseFrom = "PostWSASendCache1"; break;   // :9871
                case TCloseFrom.cfPostWSASendCache2: sCloseFrom = "PostWSASendCache2"; break;   // :9872
                case TCloseFrom.cfProcessIOQueued: sCloseFrom = "ProcessIOQueued"; break;       // :9873
                default: sCloseFrom = "无"; break;                                              // :9875
            }
            AddMainLogMsg(Format("[%d]%s 用户:%s; 来源:%s; 断开客户端连接: %s",
                ErrCode, SysErrorMessageSeam(ErrCode), sChrName, sCloseFrom, RemoteAddr), 10);   // :9877
        }

        base.CloseContextSocket(ErrCode, CloseFrom);          // :9880 inherited
    }

    internal static string SysErrorMessageSeam(int errCode)
    {
        try { return new System.ComponentModel.Win32Exception(errCode).Message; }
        catch { return errCode.ToString(); }
    }

    // =================================================================================
    // 原文 :9883-9965  procedure TMirClientContext.DoConnect;
    // =================================================================================
    protected override void DoConnect()
    {
        base.DoConnect();                                    // :9896 inherited
        TRunGate RunGate = GetRunGateAsTRunGate();           // :9897-9902

        if (RunGate == null) return;                         // :9904

        int ErrorNum = 0;                                    // :9906
        try
        {
            int OnlineUserCount;
            RunGate.OnlineUser.Lock();                       // :9908
            try
            {
                RunGate.OnlineUser.Add(this);                // :9910
                OnlineUserCount = RunGate.OnlineUser.Count;  // :9911
                if (RunGate.nMaxOnlineUserCount < OnlineUserCount)      // :9912
                    RunGate.nMaxOnlineUserCount = OnlineUserCount;      // :9913
            }
            finally
            {
                RunGate.OnlineUser.UnLock();                 // :9915 原文写的是 Unlock（大小写不敏感）
            }

            // { :9918-9925 原文整段被注释 —— 测试插件限定 10 个人 }

            ErrorNum = 11;                                   // :9927
            RunGate.SendServerMsg(GM_OPEN, (ushort)ContextID, unchecked((int)Socket), 0,
                GbkBytes(RemoteAddr), AnsiLen(RemoteAddr));  // :9928
            AddMainLogMsg("开始连接: " + RemoteAddr, 5);      // :9929

            nUserListIndex = 0;                              // :9931

            if (g_boOpenCheckClient)                         // :9933
            {
                Randomize();                                 // :9935
                boSendCheckCode = true;                      // :9936
                dwSendCheckTick = MyGetTickCount();          // :9937
                dwSendCheckCode = Random(int.MaxValue);      // :9938 Random(High(Integer))

                TDefaultMessage DefMsg = MakeDefaultMsg(SM_SENDRUNGATE_CHECKCODE, dwSendCheckCode, 0, 0, 0);  // :9940
                byte[] sSendText = EncodeRunGateMsg(DefMsg, null, 0);   // :9941
                PostSendTextBytes(sSendText);                            // :9942
            }

            ErrorNum = 14;                                   // :9945
            if (g_boAddAllToTemp && OnlineUserCount > (int)g_dwAddAllToTemp)   // :9946
            {
                RunGate.OnlineUser.Lock();                   // :9948
                try
                {
                    for (int I = 0; I < RunGate.OnlineUser.Count; I++)   // :9950
                    {
                        TMirClientContext Context = (TMirClientContext)RunGate.OnlineUser[I];  // :9952
                        AddTempBlockIP(Context.RemoteAddr);  // :9953
                    }
                }
                finally
                {
                    RunGate.OnlineUser.UnLock();             // :9956
                }

                Close();                                     // :9959
            }
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.DoConnect Error, ErrorNum = " + IntToStr(ErrorNum) + ", " + E.Message, 0);  // :9963
        }
    }

    // 原文 :9967-10021  {$IF NEED_REGISTER = 1} function Dissconnect: Boolean;
    //   —— 读自身 exe 的 PE 头（TImageDosHeader/TImageNtHeaders）+ 与
    //   FileHeader.NumberOfSymbols 比对的反调试暗桩。按 docs/转换开发文档.md §2.3
    //   （Windows 原生防护不移植）**未移植**；本车道报告 §未覆盖 登记。

    // =================================================================================
    // 原文 :10023-10249  procedure TMirClientContext.DoDisconnect(ASocket: TSocket);
    // =================================================================================
    protected override void DoDisconnect(uint aSocket)
    {
        int AContextID = ContextID;                          // :10032
        base.DoDisconnect(aSocket);                          // :10033 inherited

        TRunGate RunGate = GetRunGateAsTRunGate();           // :10035-10040

        if (RunGate == null) return;                         // :10042

        int ErrorNum = 0;                                    // :10044
        try
        {
            ErrorNum = 3;                                    // :10046
            if (g_CurrIPList != null)                        // :10047
            {
                g_CurrIPList.Lock();                         // :10049
                try
                {
                    TAddressInfo AddressInfo = g_CurrIPList.Find(RemoteAddr);   // :10051
                    if (AddressInfo != null && AddressInfo.nCount > 0)          // :10052
                    {
                        AddressInfo.nCount -= 1;             // :10054
                        if (AddressInfo.nCount <= 0)         // :10055
                            g_CurrIPList.Delete(AddressInfo); // :10056
                    }
                }
                finally
                {
                    g_CurrIPList.UnLock();                   // :10059
                }
            }

            if (AnsiLen(sMachineID) > 0)                     // :10063 Length(sMachineID) > 0
            {
                g_LoginMACPlayerList.Lock();                 // :10065
                try
                {
                    int Index = g_LoginMACPlayerList.IndexOf(sMachineID);   // :10067
                    if (Index >= 0)                          // :10068
                    {
                        object stored = g_LoginMACPlayerList.GetObject(Index);   // :10070 Integer(Objects[Index])
                        int Value = stored == null ? 0 : (int)stored;
                        if (Value > 0)                       // :10071
                            Value -= 1;                      // :10072
                        else
                            Value = 0;                       // :10074

                        if (Value == 0)                      // :10076
                            g_LoginMACPlayerList.Delete(Index);              // :10077
                        else
                            g_LoginMACPlayerList.SetObject(Index, Value);    // :10079
                    }
                }
                finally
                {
                    g_LoginMACPlayerList.UnLock();           // :10082
                }
            }

            ErrorNum = 4;                                    // :10086

            if (boSendGmClose)                               // :10088
            {
                if (boDelayClientClose && nClientCloseDelay > 0 && boFirstClientQueryBagItems)       // :10090
                    RunGate.SendServerMsg(GM_DELAY_CLOSE, (ushort)nSessionID, unchecked((int)aSocket), nUserListIndex,
                        StructBytes.BytesOf(nClientCloseDelay), sizeof(int));                        // :10091
                else if (!boValidClose && g_nClientCloseDelay > 0 && boFirstClientQueryBagItems)     // :10092
                    RunGate.SendServerMsg(GM_DELAY_CLOSE, (ushort)nSessionID, unchecked((int)aSocket), nUserListIndex,
                        StructBytes.BytesOf(g_nClientCloseDelay), sizeof(int));                      // :10093
                else
                    RunGate.SendServerMsg(GM_CLOSE, (ushort)nSessionID, unchecked((int)aSocket), nUserListIndex, null, 0); // :10095
            }

            ErrorNum = 5;                                    // :10098

            AddMainLogMsg("断开连接: " + RemoteAddr /* + '; ' + self.sChrName */, 5);   // :10100

            // { :10102-10248 原文整段被注释（g_boAntiUseException / g_rgpEndContext 插件回调） }
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.DoDisconnect Error, ErrorNum = " + IntToStr(ErrorNum) + ", " + E.Message, 0);
        }
    }

    // =================================================================================
    // 原文 :10892-10896  procedure TMirClientContext.DelayClose(DelayTime: LongWord);
    // =================================================================================
    public void DelayClose(uint DelayTime)
    {
        dwDelayCloseTick = MyGetTickCount() + DelayTime;     // :10894
        boDelayClose = true;                                 // :10895
    }

    // =================================================================================
    // 内部辅助（非原文方法）：把"AnsiString 当字节缓冲"的落点收口，见偏差 D3。
    // =================================================================================
    internal void PostSendTextBytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return;
        PostSendBuffer(bytes, bytes.Length);                 // 原文 PostSendText(AnsiString)
    }
}

// =====================================================================================
// 单元级函数（原文 :308-412）—— 原文是**单元级**（非类方法），C# 侧用静态类保持同名/同序。
// =====================================================================================
public static class MirClientContextUnit
{
    // ---------------------------------------------------------------------------------
    // 原文 :308-351  procedure UpdateLockUserList(Context: TMirClientContext);
    // 原文缺陷（保留）：:336/:346 `TObject(nTime)` 把**秒数**当指针存进 TStringList.Objects；
    //   nTime = 0 时 Delphi 的 TObject(0) 就是 nil。C# 侧存 boxed int（0 仍是 0，不是 null），
    //   故 GetUserLockTime 的判断 (:377 `Integer(Objects[Index]) > 0`) 结果一致，但
    //   `Objects[Index] = nil` 的**可观测差异**由测试 `LockUser_NilObjectVsBoxedZero` 钉死。
    // ---------------------------------------------------------------------------------
    public static void UpdateLockUserList(TMirClientContext Context)
    {
        if (!g_Config.boSaveLockStatus)                      // :314
        {
            g_LockUserList.Lock();                           // :316
            try
            {
                if (g_LockUserList.Count > 0)                // :318
                    g_LockUserList.Clear();                  // :319
            }
            finally
            {
                g_LockUserList.UnLock();                     // :321
            }
            return;                                          // :323
        }

        g_LockUserList.Lock();                               // :326
        try
        {
            int Index = g_LockUserList.IndexOf(Context.sChrName);    // :328
            uint CurTick = MyGetTickCount();                         // :329

            if (Index >= 0)                                  // :331
            {
                if (Context.boLocked && Context.dwUnLockTick > CurTick)   // :333
                {
                    int nTime = (int)((Context.dwUnLockTick - CurTick) / 1000);   // :335
                    g_LockUserList.SetObject(Index, nTime);                       // :336
                }
                else
                {
                    g_LockUserList.Delete(Index);            // :340
                }
            }
            else if (Context.boLocked && Context.dwUnLockTick > CurTick)      // :343
            {
                int nTime = (int)((Context.dwUnLockTick - CurTick) / 1000);   // :345
                g_LockUserList.AddObject(Context.sChrName, nTime);            // :346
            }
        }
        finally
        {
            g_LockUserList.UnLock();                         // :349
        }
    }

    // ---------------------------------------------------------------------------------
    // 原文 :353-387  function GetUserLockTime(Context: TMirClientContext): Integer;
    // ---------------------------------------------------------------------------------
    public static int GetUserLockTime(TMirClientContext Context)
    {
        int Result = 0;                                      // :357

        if (!g_Config.boSaveLockStatus)                      // :359
        {
            g_LockUserList.Lock();                           // :361
            try
            {
                if (g_LockUserList.Count > 0)                // :363
                    g_LockUserList.Clear();                  // :364
            }
            finally
            {
                g_LockUserList.UnLock();                     // :366
            }

            return Result;                                   // :369
        }

        g_LockUserList.Lock();                               // :372
        try
        {
            int Index = g_LockUserList.IndexOf(Context.sChrName);   // :374
            if (Index >= 0)                                  // :375
            {
                object stored = g_LockUserList.GetObject(Index);    // :377 Integer(Objects[Index]) > 0
                int value = stored == null ? 0 : (int)stored;
                if (value > 0)                               // :377
                {
                    Result = value;                          // :379
                    if (Result > g_Config.nLockTime)         // :380
                        Result = g_Config.nLockTime;         // :381
                }
            }
        }
        finally
        {
            g_LockUserList.UnLock();                         // :385
        }
        return Result;
    }

    // ---------------------------------------------------------------------------------
    // 原文 :389-412  function SubStringOccurences(const subString, sourceString: string;
    //                                            caseSensitive: boolean): integer;
    // 使用 PosEx（1-based，与 Delphi 一致）。
    // 原文缺陷（保留）：`pEx := PosEx(sub, source, pEx + Length(sub));` 当 sub 为空串时
    //   Length(sub) = 0 → pEx 不前进 → **死循环**（Delphi 同）。见下方 Bounded 版本。
    // ---------------------------------------------------------------------------------
    public static int SubStringOccurences(string subString, string sourceString, bool caseSensitive)
    {
        string sub, source;
        if (caseSensitive)                                   // :394
        {
            sub = subString;                                 // :396
            source = sourceString;                           // :397
        }
        else
        {
            sub = LowerCase(subString);                      // :401
            source = LowerCase(sourceString);                // :402
        }

        int result = 0;                                      // :405
        int pEx = PosEx(sub, source, 1);                     // :406
        while (pEx != 0)                                     // :407
        {
            result++;                                        // :409
            pEx = PosEx(sub, source, pEx + sub.Length);      // :410
        }
        return result;
    }

    /// <summary>
    /// 与 <see cref="SubStringOccurences"/> 同算法，但加了迭代上限 —— 用于测试观察
    /// "空子串会无限循环" 这一原文缺陷（否则测试进程挂死）。
    /// </summary>
    public static int SubStringOccurencesBounded(string subString, string sourceString, bool caseSensitive, int maxIterations)
    {
        string sub = caseSensitive ? subString : LowerCase(subString);
        string source = caseSensitive ? sourceString : LowerCase(sourceString);

        int result = 0;
        int pEx = PosEx(sub, source, 1);
        int guard = 0;
        while (pEx != 0)
        {
            result++;
            if (++guard >= maxIterations) break;
            pEx = PosEx(sub, source, pEx + sub.Length);
        }
        return result;
    }
}
