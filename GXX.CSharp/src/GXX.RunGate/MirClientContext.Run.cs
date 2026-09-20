// =====================================================================================
// 源单元：Source/RunGate/MirClientContext.pas（GBK，实测 11,125 LF）
// 本文件覆盖：**Run 681-1886**（约 1,206 行，TMirClientContext 的主决策循环）
//
// 条件编译（活分支，详见 MirClientContext.cs 文件头）：
//   MultiThreadRunContext = 1 → 形参是 `RunThread: TThread`（**没有** AddFullServiceMsgText）
//   CLIENT_ANTIPLUG = 1       → 反外挂超时/插件回调分支为活代码
//   UseIocpClient = 1         → `RunGate.TcpClient.SendServerMsg(...)`
//
// 接缝点（socket/线程）：PostSendText/PostSendBuffer/Close → TIocpClientContext 的
//   IIocpTransportSeam；RunGate（TRunGate）/ TcpClient.SendServerMsg → MirClientContextSeams.cs。
//
// 原文缺陷 / 易错点（原样保留，测试见 MirClientContextRunTests）：
//   R1. :997 `if boLocked and (MyGetTickCount <= dwUnLockTick)` —— 锁定期间的丢弃判据用 `<=`
//       而 Run 前面的解锁判据（:916）用 `>=`；边界同一毫秒时两者同时成立（丢弃优先）。
//   R2. :1165-1167 `LockTime := GetUserLockTime(Self); if LockTime >= 0 then LockUser(LockTime);`
//       —— GetUserLockTime 恒返回 `>= 0`（:357 Result := 0），故 `LockTime >= 0` 永真；
//       实际效果靠 LockUser(0) 在 :3246 直接 Exit 兜住。
//   R3. :1067-1068 `if (not (MagicID in [7,12,25,26,40,42,43,56,66]))` 与 :1513 的
//       `if not ((Ident = CM_SPELL) and (MagicID in [7,12,25,40]))`、:1605 的第三个列表
//       **三处列表互不相同**（26/42/43/56/66 只在部分列表里）—— 照抄，不统一。
//   R4. :1530 `if TimeInterval <= MagicCDTime - 60` 在 MagicCDTime < 60 时按 LongWord 下溢成
//       巨大值 → 该分支恒成立（照抄）。
//   R5. :1575 `MakeWord(g_btMagicCDFColor, g_btMagicCDBColor)` 的参数顺序是 (F, B)。
// =====================================================================================

using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.RunGate.AnsiStrSeam;
using static GXX.Core.Rtl.DelphiRTL;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate;

public partial class TMirClientContext
{
    /// <summary>原文 :683 <c>SKILL_MOOTEBO = 27;</c>（野蛮冲撞）。</summary>
    private const int SKILL_MOOTEBO = 27;

    // =================================================================================
    // 原文 :681-1886  procedure TMirClientContext.Run(
    //                   {$IF MultiThreadRunContext = 0}const AddFullServiceMsgText: string
    //                   {$ELSE}RunThread: TThread{$IFEND});
    // 活分支（MultiThreadRunContext = 1）形参是 RunThread: TThread。
    // =================================================================================
    public void Run(object RunThread)
    {
        // ---- 局部变量（原文 :685-710）----
        TProcessMsg ProcessMsg = new TProcessMsg();              // :685
        int I, LockTime;                                         // :686
        string sDataText = "", sHumName = "";                    // :687
        byte[] sDataTextBin = Array.Empty<byte>();               // :687 sDataText 在 :1853 被当二进制用
        ushort Ident;                                            // :688
        int Len, MaxLen, Count;                                  // :689
        object RunGateObj;                                       // :690
        TRunGate RunGate;                                        // :691
        int P;                                                   // :692 P: PChar（用 0-based 偏移表示）
        TDefaultMessage TheDefMsg = default;                     // :693
        ushort MagicID;                                          // :694
        int MagicCDSppedCount;                                   // :695 CD超速次数
        bool MagicCDSppedPass;                                   // :696 CD超速放行

        TMagicInterval CurMagicUseTick, SysMagicCD;              // :698
        uint TimeInterval;                                       // :699
        uint MagicCDTime;                                        // :700

        TDefaultMessage DefMsg = default;                        // :702
        byte[] sSendText = Array.Empty<byte>();                   // :703
        TBagItem BagItem;                                        // :705
        bool boWantVerify;                                       // :707
        string sTemp;                                            // :709
        bool IsSendToM2;                                         // :710 BOOL

        if (IsPostedCloseQuest || IsWaitingGiveBack) return;     // :712

        RunGate = null;                                          // :714
        RunGateObj = GetRunGate();                               // :715
        if (RunGateObj != null && RunGateObj is TRunGate)        // :716
        {
            RunGate = (TRunGate)RunGateObj;                      // :718
        }

        if (RunGate == null) return;                             // :721

        if (RunGate.dwCurDefenseLevel != 0 &&                    // :723
            boStartLogon &&                                      // :724
            RunGateTiming.TickDiff(dwConnectTick, MyGetTickCount()) >=
                g_dwKeepConnectTimeOut * 1000 * RunGate.dwCurDefenseLevel)   // :725
        {
            AddMainLogMsg("[空连接超时]: " + RemoteAddr, 9);     // :727
            Close();                                             // :728
        }

        else if (RunGateTiming.TickDiff(LastRecvDataTick, MyGetTickCount()) >= 1000 * 60)   // :731
        {
            AddMainLogMsg("[客户端响应超时]: " + RemoteAddr + "; " + sChrName, 9);          // :733
            Close();                                                                        // :734
        }

        // {$IF CLIENT_ANTIPLUG = 1}  原文 :737-753
        else if (boDelayClose && (MyGetTickCount() >= dwDelayCloseTick))                     // :739
        {
            Close();                                                                        // :741
        }

        // 超时时间 90000 -> 10000 2020-01-07
        else if (!boDelayClose && boSendLoadAntiPlug && boSendLoadAntiPlugFinished && !boRecvLoadAntiPlug &&
            RunGateTiming.TickDiff(dwSendLoadAntiPlugTick, MyGetTickCount()) >= 40000)       // :745-746
        {
            AddMainLogMsg("[反外挂模块加载失败 - 超时]: " + RemoteAddr + "; " + sChrName, 4);  // :748
            SendMessaggeToClient("反外挂模块加载失败(超时)，请重新进入游戏", 1, 0, 0);          // :749
            DelayClose(100);                                                                // :750
        }
        // {$IFEND}

        else if (g_boOpenCheckClient && boSendCheckCode &&                                   // :755
            !boRecvCheckCodeOK &&                                                            // :756
            RunGateTiming.TickDiff(dwSendCheckTick, MyGetTickCount()) >= 60000)              // :757
        {
            AddMainLogMsg("[客户端验证失败]: " + RemoteAddr + "; " + sChrName, 4);            // :759
            if (g_CheckClientFailBlockMethod == TBlockIPMethod.bmBlockList)                  // :760
                AddBlockIP(RemoteAddr);                                                      // :761
            else if (g_CheckClientFailBlockMethod == TBlockIPMethod.bmTempBlock)             // :762
                AddTempBlockIP(RemoteAddr);                                                  // :763

            Close();                                                                         // :765
        }

        else if (boSendVerifyCode &&
            RunGateTiming.TickDiff(dwSendVerifyCodeTick, MyGetTickCount()) >= (uint)(g_nVerifyCodeWaitTime * 1000))  // :768
        {
            if (g_boOpenVerifyCode)                                                          // :770
            {
                AddMainLogMsg("[客户端验证码超时]: " + RemoteAddr, 4);                        // :772

                if (g_boVerifyFailLoginVerify)                                               // :774
                {
                    g_VerifyFailUserList.Lock();                                             // :776
                    try
                    {
                        if (g_VerifyFailUserList.IndexOf(sChrName) < 0)                       // :778
                            g_VerifyFailUserList.Add(sChrName);                              // :779
                    }
                    finally
                    {
                        g_VerifyFailUserList.UnLock();                                        // :781
                    }
                }

                if (!g_boVerifyFailTriggerScript)                                            // :785
                {
                    SendMessaggeToClient("验证码超时，断开连接", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :787
                    DelayClose(1000);                                                        // :788
                }
                else if (RunGate != null)                                                    // :790
                {
                    DefMsg = MakeDefaultMsg(CM_SENDUSERVERIFYFAIL, 0, 0, 0, 0);              // :792
                    RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                        StructBytes.BytesOf(DefMsg), TDefaultMessage.SizeOf);                // :793
                }
            }

            boSendVerifyCode = false;                                                        // :797
        }

        // 如果客户端长时间没发消息过来，则发个心跳过去，看客户端死了没 chongchong 2014-07-04
        else if (RunGateTiming.TickDiff(LastRecvDataTick, MyGetTickCount()) >= 1000 * 30 &&
                 RunGateTiming.TickDiff(FLastSendMyHeartbeatTick, MyGetTickCount()) >= 1000 * 30)   // :801
        {
            FLastSendMyHeartbeatTick = MyGetTickCount();                                     // :803
            DefMsg = MakeDefaultMsg(SM_RUNGATE_HEARTBEAT, 0, 0, 0, 0);                       // :804
            sSendText = EncodeRunGateMsg(DefMsg, null, 0);                                   // :805
            PostSendTextBytes(sSendText);                                                    // :806
        }

        else if (g_boOpenVerifyCode && boFirstClientQueryBagItems && !boSendVerifyCode &&
                 RunGateTiming.TickDiff(dwSendVerifyCodeTick, MyGetTickCount()) >= dwVerifyInterval)   // :809
        {
            // {要查询包裹之后，表示人物完全登录}
            g_VerifyCodeMapList.Lock();                                                      // :811
            try
            {
                if (g_boVerifyCodeExcludeMap)                                                // :813
                    boWantVerify = g_VerifyCodeMapList.IndexOf(sMapName) < 0;                // :814
                else
                    boWantVerify = g_VerifyCodeMapList.IndexOf(sMapName) >= 0;               // :816
            }
            finally
            {
                g_VerifyCodeMapList.UnLock();                                                // :818
            }

            if (boWantVerify)                                                                // :821
            {
                g_LoadNoVerifyChrList.Lock();                                                // :823
                try
                {
                    boWantVerify = g_LoadNoVerifyChrList.IndexOf(sChrName) < 0;              // :825
                }
                finally
                {
                    g_LoadNoVerifyChrList.UnLock();                                          // :827
                }

                if (boWantVerify)                                                            // :830
                {
                    nVerifyCodeErrCount = 0;                                                 // :832
                    nVerifyCodeRefreshCount = 0;                                             // :833
                    sVerifyCode = "";                                                        // :834

                    if (GenerateVerifyCode())                                                // :836
                    {
                        // 先改下时间，怕在线程中反复执行。导致执行多次 GenerateVerifyCode chongchong 2016-12-11
                        dwSendVerifyCodeTick = MyGetTickCount();                             // :839

                        boSendVerifyCode = true;                                             // :841
                    }
                }
            }
            else
            {
                // 10秒后再判断，不要不停的判断
                dwSendVerifyCodeTick = MyGetTickCount();                                     // :848
                dwVerifyInterval = 10000;                                                    // :849
            }
        }

        if (boDelayClientLogout)                                                             // :853
        {
            if (RunGateTiming.TickDiff(dwDelayClientLogoutTick, MyGetTickCount()) >= 1000)   // :855
            {
                dwDelayClientLogoutTick = MyGetTickCount();                                  // :857
                if (nClientLogoutDelay > 0)                                                  // :858
                {
                    nClientLogoutDelay--;                                                    // :860 Dec
                }

                if (nClientLogoutDelay > 0)                                                  // :863
                {
                    sTemp = "本服已开启小退延时功能，正在小退..." + IntToStr(nClientLogoutDelay) + "s";  // :865
                    SendMessaggeToClient(sTemp, 0, 249, 255);                                // :866
                }
            }

            if (nClientLogoutDelay == 0)                                                     // :870
            {
                boValidClose = true;                                                         // :872
                boDelayClientLogout = false;                                                 // :873
                TheDefMsg = MakeDefaultMsg(CM_SOFTCLOSE, 0, 0, 0, 0);                        // :874
                ProcessClientMessage(TheDefMsg, Array.Empty<byte>());                        // :875 ProcessClientMessage(@TheDefMsg, '')
            }
        }

        if (boDelayClientClose)                                                              // :879
        {
            if (RunGateTiming.TickDiff(dwDelayClientCloseTick, MyGetTickCount()) >= 1000)    // :881
            {
                dwDelayClientCloseTick = MyGetTickCount();                                   // :883
                if (nClientCloseDelay > 0)                                                   // :884
                {
                    nClientCloseDelay--;                                                     // :886 Dec
                }

                if (nClientCloseDelay > 0)                                                   // :889
                {
                    sTemp = "本服已开启大退延时功能，正在大退..." + IntToStr(nClientCloseDelay) + "s";  // :891
                    SendMessaggeToClient(sTemp, 0, 249, 255);                                // :892
                }
            }

            if (nClientCloseDelay == 0)                                                      // :896
            {
                boValidClose = true;                                                         // :898
                boDelayClientClose = false;                                                  // :899
                TheDefMsg = MakeDefaultMsg(CM_IOCP_APPEXIT, 0, 0, 0, 0);                     // :900
                ProcessClientMessage(TheDefMsg, Array.Empty<byte>());                        // :901
            }
        }

        // (* 原文 :905-912 整段被注释：GetNoSendCacheSize 堆积检测 *)

        // 检查待解锁的角色 chongchong 2014-12-16
        if (boLocked && (MyGetTickCount() >= dwUnLockTick))                                  // :916
        {
            UnLockUser();                                                                    // :918
        }

        // 将服M2收到的封包往客户端转发 chongchong 2016-06-25
        // :923 原文的 if 条件被注释掉，只剩 `begin ... end;`（无条件执行）
        {
            FServerMsgLocker.Lock();                                                         // :925
            try
            {
                // {$IF MultiThreadRunContext = 0} 死分支（活分支下 Run 没有 AddFullServiceMsgText 形参）
                //   if Length(AddFullServiceMsgText) > 0 then
                //     FServerMsgStr := FServerMsgStr + AddFullServiceMsgText;

                Len = FServerMsgStr.Length;                                                  // :932
                if (Len > 0)                                                                 // :933
                {
                    dwSendDateToClientTick = MyGetTickCount();                               // :935

                    MaxLen = (int)(IocpSendCachePolicy.MaxOverlappedExBufferSizeKb << 10);   // :937  MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10
                    if (Len <= MaxLen)                                                       // :938
                    {
                        PostSendTextBytes(FServerMsgStr);                                    // :940 PostSendText(FServerMsgStr)
                        FServerMsgStr = Array.Empty<byte>();                                 // :941
                    }
                    else
                    {
                        // {
                        // P := PChar(FServerMsgStr);
                        // PostSendBuffer(P, MaxLen);
                        // FServerMsgStr := Copy(FServerMsgStr, MaxLen + 1, MaxInt);
                        // }

                        Count = Len / MaxLen;                                                // :951 整数除

                        P = 0;                                                               // :953 P := PChar(FServerMsgStr)
                        for (I = 1; I <= Count; I++)                                         // :954
                        {
                            PostSendBuffer(FServerMsgStr, P, MaxLen);                        // :956
                            P += MaxLen;                                                     // :957 Inc(P, MaxLen)
                        }

                        Count = Len % MaxLen;                                                // :960
                        if (Count > 0)                                                       // :961
                        {
                            PostSendBuffer(FServerMsgStr, P, Count);                         // :963
                        }

                        FServerMsgStr = Array.Empty<byte>();                                 // :966
                    }
                }
                else
                {
                    if (AnsiLen(g_ProcessBlacklistStr) > 0 &&                                            // :971
                        RunGateTiming.TickDiff(dwSendProcessBlacklistTick, MyGetTickCount()) >= 60000 &&  // :972
                        !MD5Match(SendProcessBlacklistMD5, g_ProcessBlacklistMD5) &&                      // :973
                        SM_PROCESSBLACKLIST > 0)                                                          // :974
                    {
                        dwSendProcessBlacklistTick = MyGetTickCount();                       // :976
                        SendProcessBlacklistMD5 = (byte[])g_ProcessBlacklistMD5.Clone();     // :977
                        TheDefMsg = MakeDefaultMsg(SM_PROCESSBLACKLIST, AnsiLen(g_ProcessBlacklistStr), 0, 0, 0);  // :978

                        sDataTextBin = EncodeRunGateMsg(TheDefMsg, GbkBytes(g_ProcessBlacklistStr),
                            AnsiLen(g_ProcessBlacklistStr));                                 // :980
                        FServerMsgStr = Concat(FServerMsgStr, sDataTextBin);                 // :981
                    }
                }
            }
            finally
            {
                FServerMsgLocker.UnLock();                                                   // :985
            }
        }

        // 将客户端收到的封包往M2转发 chongchong 2016-06-25
        if (GetClientMessage(ProcessMsg))                                                    // :990
        {
            Ident = ProcessMsg.DefMessage.Ident;                                             // :992

            if (boDelayClose) return;                                                        // :994

            // 如果在锁定时间段内，数据包丢弃 chongchong 2014-12-16
            if (boLocked && (MyGetTickCount() <= dwUnLockTick))                              // :997
            {
                if (Ident != CM_SOFTCLOSE)                                                   // :999
                    return;                                                                  // :1000
            }

            if (Ident == CM_WALK || Ident == CM_RUN || Ident == CM_TURN)                     // :1003
            {
                if ((boDelayClientLogout || boDelayClientClose) && g_boDelayCloseDisableMove)  // :1005
                {
                    if (boDelayClientClose)                                                  // :1007
                    {
                        if (g_boBreakClientCloseHint && g_sBreakClientCloseHint != "")       // :1009
                        {
                            SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);      // :1011
                        }
                    }
                    else if (boDelayClientLogout)                                            // :1014
                    {
                        if (g_boBreakClientLogoutHint && g_sBreakClientLogoutHint != "")     // :1016
                        {
                            SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);     // :1018
                        }
                    }
                    boDelayClientLogout = false;                                             // :1021
                    boDelayClientClose = false;                                              // :1022

                    TheDefMsg = MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);                    // :1024
                    sDataTextBin = EncodeRunGateMsg(TheDefMsg, null, 0);                     // :1025
                    FServerMsgStr = Concat(FServerMsgStr, sDataTextBin);                     // :1026
                }
            }
            else if (Ident == CM_HIT || Ident == CM_HEAVYHIT || Ident == CM_BIGHIT ||         // :1029
                Ident == CM_POWERHIT || Ident == CM_LONGHIT || Ident == CM_WIDEHIT ||         // :1030
                Ident == CM_FIREHIT || Ident == CM_CRSHIT || Ident == CM_TWNHIT ||            // :1031
                Ident == CM_SWORDHIT || Ident == CM_43HIT ||                                  // :1032
                Ident == CM_66HIT || Ident == CM_66HIT1 ||                                    // :1033
                Ident == CM_101HIT || Ident == CM_102HIT || Ident == CM_103HIT ||             // :1034
                Ident == CM_113HIT || Ident == CM_115HIT ||                                   // :1035
                (Ident >= CM_CUSTOM_HIT001 && Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT) ||   // :1036
                (Ident == CM_SPELL && ProcessMsg.DefMessage.Tag == SKILL_MOOTEBO))            // :1037
            {
                if ((boDelayClientLogout || boDelayClientClose) && g_boDelayCloseDisableAttack)   // :1039
                {
                    if (boDelayClientClose)                                                  // :1041
                    {
                        if (g_boBreakClientCloseHint && g_sBreakClientCloseHint != "")       // :1043
                        {
                            SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);      // :1045
                        }
                    }
                    else if (boDelayClientLogout)                                            // :1048
                    {
                        if (g_boBreakClientLogoutHint && g_sBreakClientLogoutHint != "")     // :1050
                        {
                            SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);     // :1052
                        }
                    }

                    boDelayClientLogout = false;                                             // :1056
                    boDelayClientClose = false;                                              // :1057

                    TheDefMsg = MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);                    // :1059
                    sDataTextBin = EncodeRunGateMsg(TheDefMsg, null, 0);                     // :1060
                    FServerMsgStr = Concat(FServerMsgStr, sDataTextBin);                     // :1061
                }
            }
            else if (Ident == CM_SPELL)                                                      // :1064
            {
                MagicID = ProcessMsg.DefMessage.Tag;                                         // :1066
                // :1067-1068 `if (not (MagicID in [7{攻杀}, 12{刺杀}, 25{半月}, 26{烈火},
                //   40{双龙斩}, 42{龙影}, 43{雷霆剑法}, 56{逐日剑法}, 66{开天斩}])) then`
                if (!(MagicID == 7 || MagicID == 12 || MagicID == 25 || MagicID == 26 ||
                      MagicID == 40 || MagicID == 42 || MagicID == 43 || MagicID == 56 || MagicID == 66))
                {
                    // 不是开关技能，自定义开关技能开关，在这里不好搞。先这样判断
                    if ((boDelayClientLogout || boDelayClientClose) && g_boDelayCloseDisableSpell)   // :1071
                    {
                        if (!(ProcessMsg.DefMessage.Recog == 0 && ProcessMsg.DefMessage.Param == 0 &&
                              ProcessMsg.DefMessage.Series == 0))                            // :1073
                        {
                            if (boDelayClientClose)                                              // :1075
                            {
                                if (g_boBreakClientCloseHint && g_sBreakClientCloseHint != "")   // :1077
                                {
                                    SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);  // :1079
                                }
                            }
                            else if (boDelayClientLogout)                                        // :1082
                            {
                                if (g_boBreakClientLogoutHint && g_sBreakClientLogoutHint != "") // :1084
                                {
                                    SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252); // :1086
                                }
                            }

                            boDelayClientLogout = false;                                      // :1090
                            boDelayClientClose = false;                                       // :1091

                            TheDefMsg = MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);             // :1093
                            sDataTextBin = EncodeRunGateMsg(TheDefMsg, null, 0);              // :1094
                            FServerMsgStr = Concat(FServerMsgStr, sDataTextBin);              // :1095
                        }
                    }
                }
            }
            else if (Ident == CM_EAT)                                                        // :1100
            {
                if ((boDelayClientLogout || boDelayClientClose) && g_boDelayCloseDisableUseItem)  // :1102
                {
                    if (boDelayClientClose)                                                  // :1104
                    {
                        if (g_boBreakClientCloseHint && g_sBreakClientCloseHint != "")       // :1106
                        {
                            SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);      // :1108
                        }
                    }
                    else if (boDelayClientLogout)                                            // :1111
                    {
                        if (g_boBreakClientLogoutHint && g_sBreakClientLogoutHint != "")     // :1113
                        {
                            SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);     // :1115
                        }
                    }

                    boDelayClientLogout = false;                                             // :1119
                    boDelayClientClose = false;                                              // :1120

                    TheDefMsg = MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);                    // :1122
                    sDataTextBin = EncodeRunGateMsg(TheDefMsg, null, 0);                     // :1123
                    FServerMsgStr = Concat(FServerMsgStr, sDataTextBin);                     // :1124
                }
            }

            // 消息过滤 chongchong 2014-12-27
            if (Ident == CM_SAY)                                                             // :1129
            {
                if (boChangeMap) boChangeMap = false;                                        // :1131

                if (ProcessMsg.sMessage != null && ProcessMsg.sMessage.Length > 0)           // :1133 Length(ProcessMsg.sMessage) > 0
                {
                    sDataText = DecodeBuffer(ProcessMsg.sMessage, ProcessMsg.sMessage.Length);   // :1135 DecodeString(ProcessMsg.sMessage)
                    if (AnsiLen(sDataText) > 0)                                              // :1136
                    {
                        if (sDataText[0] == '/')                                             // :1138 sDataText[1]
                        {
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sHumName, new char[] { ' ' });   // :1140
                            if (FilterSayMsg(ref sDataText)) return;                         // :1141
                            sDataText = sHumName + " " + sDataText;                          // :1142
                        }
                        else
                        {
                            if (sDataText[0] != '@')                                         // :1146
                            {
                                if (FilterSayMsg(ref sDataText)) return;                     // :1148
                            }
                        }

                        // 将改变的数据发出去
                        ProcessMsg.sMessage = EncodeString(sDataText);                   // :1153
                    }
                }
            }

            // 在客户端第一次请求包裹列表时，返回锁定状态及文字 chongchong 2014-12-16
            else if (Ident == CM_QUERYBAGITEMS)                                              // :1159
            {
                if (boChangeMap) boChangeMap = false;                                        // :1161

                if (!boFirstClientQueryBagItems)                                             // :1163
                {
                    LockTime = MirClientContextUnit.GetUserLockTime(this);                   // :1165
                    if (LockTime >= 0)                                                       // :1166 恒真（见 R2）
                        LockUser(LockTime);                                                  // :1167

                    if (g_boLogoutNoResendAntiplugStream && dwRecvClientAntiplugCRC == g_ClientAntiPlugDllStringCRC)  // :1169
                    {
                        // 网关插件加载才让客户端插件加载 2020-01-02 00:40:00
                        if (g_RunGatePlugDllHandle != 0)                                     // :1172
                        {
                            SendAntiPlugStreamLoadCache();                                   // :1174
                        }
                    }

                    boFirstClientQueryBagItems = true;                                       // :1178
                }
            }

            else if (Ident == CM_SOFTCLOSE)                                                  // :1182
            {
                boClientSoftClose = true;                                                    // :1184
            }

            else if (Ident == CM_IOCP_APPEXIT)                                               // :1187
            {
                boClientSoftClose = true;                                                    // :1189

                DefMsg = MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 0);                           // :1191
                sSendText = EncodeRunGateMsg(DefMsg, null, 0);                               // :1192
                PostSendTextBytes(sSendText);                                                // :1193

                return;                                                                      // :1195
            }

            // 请求交易
            else if (Ident == CM_DEALTRY)                                                    // :1199
            {
                if (boChangeMap) boChangeMap = false;                                        // :1201

                // 攻击到交易的间隔
                if (RunGateTiming.TickDiff(GameSpeed.dwAttackTick, MyGetTickCount()) <
                    g_Config.dwDealTry_Attack_Interval)                                      // :1204
                {
                    if (g_Config.boDealTry_Attack_ShowHint)                                  // :1206
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1207
                    return;                                                                  // :1208
                }

                // 交易到交易的间隔
                if (RunGateTiming.TickDiff(GameSpeed.dwDealTryTick, MyGetTickCount()) <
                    g_Config.dwDealTry_Attack_Interval)                                      // :1212
                {
                    if (g_Config.boDealTry_Attack_ShowHint)                                  // :1214
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1215
                    return;                                                                  // :1216
                }

                GameSpeed.dwDealTryTick = MyGetTickCount();                                  // :1219
            }

            // 请求挑战
            else if (Ident == CM_CHALLENGETRY)                                               // :1223
            {
                if (boChangeMap) boChangeMap = false;                                        // :1225

                // 交易到挑战的间隔
                if (RunGateTiming.TickDiff(GameSpeed.dwDealTryTick, MyGetTickCount()) <
                    g_Config.dwDealTry_Attack_Interval)                                      // :1228
                {
                    if (g_Config.boDealTry_Attack_ShowHint)                                  // :1230
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1231
                    return;                                                                  // :1232
                }

                // 攻击或挑战 到 挑战的间隔
                if (RunGateTiming.TickDiff(GameSpeed.dwAttackTick, MyGetTickCount()) <
                    g_Config.dwDealTry_Attack_Interval)                                      // :1236
                {
                    if (g_Config.boDealTry_Attack_ShowHint)                                  // :1238
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1239
                    return;                                                                  // :1240
                }

                GameSpeed.dwAttackTick = MyGetTickCount();                                   // :1243
            }

            else if (Ident == CM_SEARCHSHOPITEMS)                                            // :1246  {CM_QUERYUSERSHOPS,} 被注释
            {
                if (boChangeMap) boChangeMap = false;                                        // :1248

                // 搜索个人商店物品
                if (RunGateTiming.TickDiff(GameSpeed.dwShopItemSearchTick, MyGetTickCount()) <
                    g_Config.dwUserShop_Search_Interval)                                     // :1251
                {
                    if (g_Config.boUserShop_Search_ShowHint)                                 // :1253
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1254
                    return;                                                                  // :1255
                }

                GameSpeed.dwShopItemSearchTick = MyGetTickCount();                           // :1258
            }

            else if (Ident == CM_QUERYUSERSHOPITEMS)                                         // :1261  {CM_QUERYUSERSHOPS,} 被注释
            {
                if (boChangeMap) boChangeMap = false;                                        // :1263

                // 搜索个人商店物品
                if (RunGateTiming.TickDiff(GameSpeed.dwUserShopItemSearchTick, MyGetTickCount()) <
                    g_Config.dwUserShop_Search_Interval)                                     // :1266
                {
                    if (g_Config.boUserShop_Search_ShowHint)                                 // :1268
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1269
                    return;                                                                  // :1270
                }

                GameSpeed.dwUserShopItemSearchTick = MyGetTickCount();                       // :1273
            }

            else if (Ident == CM_SENDBUYUSERSHOPITEM)                                        // :1276
            {
                if (boChangeMap) boChangeMap = false;                                        // :1278

                // 购买个人商店物品
                if (RunGateTiming.TickDiff(GameSpeed.dwUserShopBuyTick, MyGetTickCount()) <
                    g_Config.dwUserShop_Buy_Interval)                                        // :1281
                {
                    if (g_Config.boUserShop_Buy_ShowHint)                                    // :1283
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1284
                    return;                                                                  // :1285
                }

                GameSpeed.dwUserShopBuyTick = MyGetTickCount();                              // :1288
            }

            else if (Ident == CM_TAKEONITEM)                                                 // :1291
            {
                if (boChangeMap) boChangeMap = false;                                        // :1293

                // 人物穿戴装备
                if (RunGateTiming.TickDiff(GameSpeed.dwTakeOnItemTick, MyGetTickCount()) <
                    g_Config.dwTakeOn_Item_Interval)                                         // :1296
                {
                    if (g_Config.boTakeOn_Item_ShowHint)                                     // :1298
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1299

                    DefMsg = MakeDefaultMsg(SM_TAKEON_FAIL, 0, 0, 0, 0);                     // :1301

                    sSendText = EncodeRunGateMsg(DefMsg, null, 0);                           // :1303
                    PostSendTextBytes(sSendText);                                            // :1304

                    return;                                                                  // :1306
                }

                GameSpeed.dwTakeOnItemTick = MyGetTickCount();                               // :1309
            }

            else if (Ident == CM_HEROTAKEONITEM)                                             // :1312
            {
                if (boChangeMap) boChangeMap = false;                                        // :1314

                // 英雄穿戴装备
                if (RunGateTiming.TickDiff(GameSpeed.dwHeroTakeOnItemTick, MyGetTickCount()) <
                    g_Config.dwTakeOn_Item_Interval)                                         // :1317
                {
                    if (g_Config.boTakeOn_Item_ShowHint)                                     // :1319
                        SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1320

                    DefMsg = MakeDefaultMsg(SM_HEROTAKEON_FAIL, 0, 0, 0, 0);                 // :1322
                    sSendText = EncodeRunGateMsg(DefMsg, null, 0);                           // :1323
                    PostSendTextBytes(sSendText);                                            // :1324

                    return;                                                                  // :1326
                }

                GameSpeed.dwHeroTakeOnItemTick = MyGetTickCount();                           // :1329
            }

            else if (Ident == CM_GETRUNGATEVERIFYCODE)                                       // :1332
            {
                if (boSendVerifyCode)                                                        // :1334
                {
                    if (nVerifyCodeRefreshCount < g_nVerifyCodeRefreshCount)                 // :1336
                    {
                        if (GenerateVerifyCode())                                            // :1338
                        {
                            //nVerifyCodeErrCount := 0;                                      // :1340 原文如此（已注释）
                            nVerifyCodeRefreshCount = nVerifyCodeRefreshCount + 1;           // :1341
                            SendMessaggeToClient("刷新验证码成功，还可以再刷新" +
                                IntToStr(g_nVerifyCodeRefreshCount - nVerifyCodeRefreshCount) + "次",
                                0, g_Config.btMsgFColor, g_Config.btMsgBColor);              // :1342
                        }
                    }
                    else
                    {
                        SendMessaggeToClient("刷新验证码失败，因为你已经刷新了太多次",
                            0, g_Config.btMsgFColor, g_Config.btMsgBColor);                  // :1347
                    }
                }
                return;                                                                      // :1350
            }

            else if (Ident == CM_CHECKRUNGATEVERIFYCODE)                                     // :1353
            {
                if (boSendVerifyCode)                                                        // :1355
                {
                    sDataText = DecodeBuffer(ProcessMsg.sMessage, ProcessMsg.sMessage.Length);   // :1357
                    if (SameText(sVerifyCode, sDataText))                                    // :1358
                    {
                        boSendVerifyCode = false;                                            // :1360
                        dwSendVerifyCodeTick = MyGetTickCount();                             // :1361
                        nVerifyCodeErrCount = 0;                                             // :1362
                        nVerifyCodeRefreshCount = 0;                                         // :1363

                        DefMsg = MakeDefaultMsg(SM_RUNGATE_VERIFYCODE_CHECK_RET, 0, 1, 0, 0);  // :1365
                        sSendText = EncodeRunGateMsg(DefMsg, null, 0);                       // :1366
                        PostSendTextBytes(sSendText);                                        // :1367

                        nVerifySuccessCount++;                                               // :1369 Inc

                        dwVerifyInterval = (g_dwVerifyCodeInterval1 +
                            g_dwVerifySuccessAddInterval * (uint)nVerifySuccessCount) * 60000 +
                            (uint)Random(unchecked((int)((g_dwVerifyCodeInterval2 - g_dwVerifyCodeInterval1) * 60000)));  // :1371-1372

                        if (g_boVerifyFailLoginVerify)                                       // :1374
                        {
                            boVerifyDisableAttack = false;                                   // :1376
                            g_VerifyFailUserList.Lock();                                     // :1377
                            try
                            {
                                I = g_VerifyFailUserList.IndexOf(sChrName);                  // :1379
                                if (I >= 0)                                                  // :1380
                                    g_VerifyFailUserList.Delete(I);                          // :1381
                            }
                            finally
                            {
                                g_VerifyFailUserList.UnLock();                               // :1383
                            }
                        }
                    }
                    else
                    {
                        nVerifyCodeErrCount++;                                               // :1389 Inc
                        if (nVerifyCodeErrCount >= g_nVerifyCodeErrCount)                   // :1390
                        {
                            if (g_boVerifyFailLoginVerify)                                   // :1392
                            {
                                g_VerifyFailUserList.Lock();                                 // :1394
                                try
                                {
                                    if (g_VerifyFailUserList.IndexOf(sChrName) < 0)          // :1396
                                        g_VerifyFailUserList.Add(sChrName);                  // :1397
                                }
                                finally
                                {
                                    g_VerifyFailUserList.UnLock();                           // :1399
                                }
                            }

                            if (!g_boVerifyFailTriggerScript)                                // :1403
                            {
                                SendMessaggeToClient("连续输入验证码失败，断开连接", 1,
                                    g_Config.btMsgFColor, g_Config.btMsgBColor);             // :1405
                                DelayClose(1000);                                            // :1406
                            }
                            else if (RunGate != null)                                        // :1408
                            {
                                DefMsg = MakeDefaultMsg(CM_SENDUSERVERIFYFAIL, 0, 0, 0, 0);  // :1410
                                RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                                    StructBytes.BytesOf(DefMsg), TDefaultMessage.SizeOf);    // :1411
                            }
                        }
                        else
                        {
                            DefMsg = MakeDefaultMsg(SM_RUNGATE_VERIFYCODE_CHECK_RET,
                                g_nVerifyCodeErrCount - nVerifyCodeErrCount, 0, 0, 0);       // :1416
                            sSendText = EncodeRunGateMsg(DefMsg, null, 0);                   // :1417
                            PostSendTextBytes(sSendText);                                    // :1418
                        }
                    }
                }

                return;                                                                      // :1423
            }

            // 其他数据包
            else                                                                             // :1427
            {
                if (Ident == CM_SPELL ||                                                     // :1429 魔法攻击
                    Ident == CM_HIT || Ident == CM_HEAVYHIT || Ident == CM_BIGHIT ||
                    Ident == CM_POWERHIT || Ident == CM_LONGHIT || Ident == CM_WIDEHIT ||
                    Ident == CM_FIREHIT || Ident == CM_CRSHIT || Ident == CM_TWNHIT ||
                    Ident == CM_SWORDHIT || Ident == CM_43HIT ||
                    Ident == CM_66HIT || Ident == CM_66HIT1 ||
                    Ident == CM_101HIT || Ident == CM_102HIT || Ident == CM_103HIT ||
                    Ident == CM_113HIT || Ident == CM_115HIT ||
                    (Ident >= CM_CUSTOM_HIT001 && Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT))   // :1429-1437
                {
                    if (boChangeMap) boChangeMap = false;                                    // :1439

                    // 验证失败重进游戏立即验证后，不让攻击
                    if (boSendVerifyCode && boVerifyDisableAttack)                           // :1442
                    {
                        if (Ident == CM_SPELL)                                               // :1444
                        {
                            DefMsg = MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);   // :1446
                            sSendText = EncodeRunGateMsg(DefMsg, null, 0);                   // :1447
                            PostSendTextBytes(sSendText);                                    // :1448
                        }

                        SendActionRet(false);                                                // :1451

                        return;                                                              // :1453
                    }

                    // 技能冷确时间判断 chongchong 2016-10-08
                    MagicID = 0;                                                             // :1457
                    if (Ident == CM_POWERHIT)                                                // :1458 攻杀
                        MagicID = 7;
                    else if (Ident == CM_LONGHIT)                                            // :1460 刺杀
                        MagicID = 12;
                    else if (Ident == CM_WIDEHIT)                                            // :1462 半月
                        MagicID = 25;
                    else if (Ident == CM_FIREHIT)                                            // :1464 烈火
                        MagicID = 26;
                    else if (Ident == CM_CRSHIT)                                             // :1466 双龙斩
                        MagicID = 40;
                    else if (Ident == CM_TWNHIT)                                             // :1468 龙影
                        MagicID = 42;
                    else if (Ident == CM_43HIT)                                              // :1470 雷霆剑法
                        MagicID = 43;
                    else if (Ident == CM_SWORDHIT)                                           // :1472 逐日剑法
                        MagicID = 56;
                    else if (Ident == CM_66HIT)                                              // :1474 开天斩
                        MagicID = 66;
                    else if (Ident == CM_66HIT1)                                             // :1476 开天斩轻击
                        MagicID = 66;
                    else if (Ident == CM_101HIT)                                             // :1478 三绝杀
                        MagicID = 101;
                    else if (Ident == CM_102HIT)                                             // :1480 断岳斩
                        MagicID = 102;
                    else if (Ident == CM_103HIT)                                             // :1482 横扫千军
                        MagicID = 103;
                    else if (Ident == CM_113HIT)                                             // :1484 断空斩
                        MagicID = 113;
                    else if (Ident == CM_115HIT)                                             // :1486 血魂一击
                        MagicID = 115;
                    else if (Ident >= CM_CUSTOM_HIT001 && Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)   // :1488
                        MagicID = (ushort)(Ident - CM_CUSTOM_HIT001 + 1000);                 // :1489
                    else if (Ident == CM_SPELL)                                              // :1490
                    {
                        MagicID = ProcessMsg.DefMessage.Tag;                                 // :1492
                        //AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~' + IntToStr(MagicID), 0);    // :1493 原文如此（已注释）
                    }

                    if (MagicID > 0)                                                         // :1496
                    {
                        TimeInterval = uint.MaxValue;                                        // :1498 High(LongWord)

                        MagicUseTickList.Lock();                                             // :1500
                        try
                        {
                            CurMagicUseTick = MagicUseTickList.Find(unchecked((ushort)MagicID));   // :1502

                            if (CurMagicUseTick != null)                                     // :1504
                            {
                                TimeInterval = RunGateTiming.TickDiff(CurMagicUseTick.Interval, MyGetTickCount());  // :1506
                            }
                        }
                        finally
                        {
                            MagicUseTickList.UnLock();                                       // :1509
                        }

                        // 这几个战士技能开关不用关心开关时间
                        // :1513 `if not ((Ident = CM_SPELL) and (MagicID in [7,12,25,40])) then`
                        if (!(Ident == CM_SPELL && (MagicID == 7 || MagicID == 12 || MagicID == 25 || MagicID == 40)))
                        {
                            if (TimeInterval != uint.MaxValue)                               // :1515
                            {
                                MagicCDTime = 0;                                             // :1517
                                g_MagicCDList.Lock();                                        // :1518
                                try
                                {
                                    SysMagicCD = g_MagicCDList.Find(unchecked((ushort)MagicID));   // :1520
                                    if (SysMagicCD != null)                                  // :1521
                                        MagicCDTime = SysMagicCD.Interval;                   // :1522
                                }
                                finally
                                {
                                    g_MagicCDList.UnLock();                                  // :1524
                                }

                                if (MagicCDTime != 0)                                        // :1527
                                {
                                    // :1529 OutputDebugString(PChar(IntToStr(TimeInterval))); —— 调试输出，不移植
                                    // 原文缺陷（R4）：MagicCDTime < 60 时按 LongWord 下溢 → 恒成立
                                    if (TimeInterval <= unchecked(MagicCDTime - 60))         // :1530
                                    {
                                        MagicCDSppedPass = false;                            // :1532

                                        MagicCDSpeed[MagicCDSpeedIndex] = true;              // :1534
                                        if (MagicCDSpeedCount < MagicCDSpeed.Length)         // :1535 Length(MagicCDSpeed) = 10
                                            MagicCDSpeedCount++;

                                        MagicCDSpeedIndex++;                                 // :1537 Inc
                                        if (MagicCDSpeedIndex >= MagicCDSpeed.Length)        // :1538
                                            MagicCDSpeedIndex = 0;

                                        // 有一定的几率放行极小超速的技能CD chongchong 2016-10-16
                                        if (TimeInterval >= unchecked(MagicCDTime - 200))    // :1541
                                        {
                                            MagicCDSppedCount = 0;                           // :1543
                                            for (I = 0; I <= MagicCDSpeed.Length - 1; I++)   // :1544 `for I := 0 to Length(MagicCDSpeed) - 1`
                                            {
                                                if (MagicCDSpeed[I])                         // :1546
                                                    MagicCDSppedCount++;
                                            }

                                            if (MagicCDSpeedCount >= 6)                      // :1550
                                                MagicCDSppedPass = MagicCDSppedCount <= 2;
                                            else
                                                MagicCDSppedPass = MagicCDSppedCount <= 1;
                                        }

                                        if (!MagicCDSppedPass)                               // :1556
                                        {
                                            DefMsg = MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);   // :1558
                                            sSendText = EncodeRunGateMsg(DefMsg, null, 0);       // :1559
                                            PostSendTextBytes(sSendText);                        // :1560
                                            SendActionRet(true);                                 // :1561

                                            if (AnsiLen(g_sMagicCDMsgText) > 0)                  // :1563
                                            {
                                                TimeInterval = (MagicCDTime - TimeInterval + 999) / 1000;   // :1565

                                                sDataText = StringReplace(g_sMagicCDMsgText, "%time",
                                                    IntToStr((int)TimeInterval), true);          // :1567 [rfIgnoreCase]

                                                if (g_btMagicCDMsgType == 0)                     // :1569
                                                {
                                                    SendMessaggeToClient(sDataText, 0,
                                                        g_btMagicCDFColor, g_btMagicCDBColor);   // :1571
                                                }
                                                else if (g_btMagicCDMsgType == 1)            // :1573
                                                {
                                                    DefMsg = MakeDefaultMsg(SM_SCREENMESSAGE, 0,
                                                        MakeWord(g_btMagicCDFColor, g_btMagicCDBColor),
                                                        (ushort)g_nMagicCDShowX, (ushort)g_nMagicCDShowY);   // :1575
                                                    sSendText = EncodeRunGateMsg(DefMsg, GbkBytes(sDataText),
                                                        AnsiLen(sDataText));                     // :1576
                                                    PostSendTextBytes(sSendText);                // :1577
                                                }
                                            }

                                            return;                                          // :1581
                                        }
                                    }
                                }
                                else
                                {
                                    MagicCDSpeed[MagicCDSpeedIndex] = false;                 // :1587
                                    if (MagicCDSpeedCount < MagicCDSpeed.Length)             // :1588
                                        MagicCDSpeedCount++;

                                    MagicCDSpeedIndex++;                                     // :1590
                                    if (MagicCDSpeedIndex >= MagicCDSpeed.Length)            // :1591
                                        MagicCDSpeedIndex = 0;
                                }
                            }
                            else
                            {
                                MagicCDSpeed[MagicCDSpeedIndex] = false;                     // :1596
                                if (MagicCDSpeedCount < MagicCDSpeed.Length)                 // :1597
                                    MagicCDSpeedCount++;

                                MagicCDSpeedIndex++;                                         // :1599
                                if (MagicCDSpeedIndex >= MagicCDSpeed.Length)                // :1600
                                    MagicCDSpeedIndex = 0;
                            }
                        }

                        // 开关战士技能不要记录技能时间
                        // :1605-1606 `if not ((Ident = CM_SPELL) and (MagicID in [7,12,25,26,40,42,43,56,66])) then`
                        if (!(Ident == CM_SPELL && (MagicID == 7 || MagicID == 12 || MagicID == 25 || MagicID == 26 ||
                              MagicID == 40 || MagicID == 42 || MagicID == 43 || MagicID == 56 || MagicID == 66)))
                        {
                            MagicUseTickList.Lock();                                     // :1608
                            try
                            {
                                CurMagicUseTick = MagicUseTickList.Find(unchecked((ushort)MagicID));   // :1610

                                if (CurMagicUseTick == null)                             // :1612
                                {
                                    CurMagicUseTick = MagicUseTickList.Add(MagicID);     // :1614
                                    if (CurMagicUseTick != null)                         // :1615
                                    {
                                        CurMagicUseTick.Interval = MyGetTickCount();     // :1617
                                    }
                                }
                                else
                                {
                                    CurMagicUseTick.Interval = MyGetTickCount();         // :1622
                                }
                            }
                            finally
                            {
                                MagicUseTickList.UnLock();                               // :1625
                            }
                        }
                    }

                    // 交易到攻击的间隔
                    if (RunGateTiming.TickDiff(GameSpeed.dwDealTryTick, MyGetTickCount()) <
                        g_Config.dwDealTry_Attack_Interval)                                  // :1631
                    {
                        if (g_Config.boDealTry_Attack_ShowHint)                              // :1633
                            SendMessaggeToClient("您的操作速度过快", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1634

                        // 返回包处理失败到客户端
                        SendActionRet(false);                                                // :1637

                        return;                                                              // :1639
                    }

                    // 野蛮到攻击的间隔
                    if (!(Ident == CM_SPELL && ProcessMsg.DefMessage.Tag == SKILL_MOOTEBO) &&    // :1643
                        RunGateTiming.TickDiff(GameSpeed.dwMooteboTick, MyGetTickCount()) <
                            g_Config.dwBrutal_Attack_Interval)                               // :1644
                    {
                        if (g_Config.boBrutal_Attack_ShowHint)                               // :1646
                            SendMessaggeToClient("您的操作速度过快", 0, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1647

                        // 返回包处理失败到客户端
                        SendActionRet(false);                                                // :1650

                        return;                                                              // :1652
                    }

                    // 野蛮冲撞
                    if (Ident == CM_SPELL && ProcessMsg.DefMessage.Tag == SKILL_MOOTEBO)     // :1656
                    {
                        // 转向到野蛮 2020-02-01 19:21:38
                        if (RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn],
                                MyGetTickCount()) < g_Config.dwBrutal_Attack_Interval)      // :1659
                        {
                            if (g_Config.boBrutal_Attack_ShowHint)                           // :1661
                                SendMessaggeToClient("您的操作速度过快", 0, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1662

                            // 返回包处理失败到客户端
                            SendActionRet(false);                                            // :1665

                            return;                                                          // :1667
                        }

                        GameSpeed.dwMooteboTick = MyGetTickCount();                          // :1670
                    }

                    GameSpeed.dwAttackTick = MyGetTickCount();                               // :1673
                }
                else if (Ident == CM_WALK || Ident == CM_RUN)                                // :1675
                {
                    // 野蛮到移动的间隔
                    if (RunGateTiming.TickDiff(GameSpeed.dwMooteboTick, MyGetTickCount()) <
                        g_Config.dwBrutal_Attack_Interval)                                   // :1678
                    {
                        if (g_Config.boBrutal_Attack_ShowHint)                               // :1680
                            SendMessaggeToClient("您的操作速度过快", 0, g_Config.btMsgFColor, g_Config.btMsgBColor);  // :1681

                        // 返回包处理失败到客户端
                        SendActionRet(false);                                                // :1684

                        return;                                                              // :1686
                    }
                }

                // 收到客户端发来的时间验证包，不用往服务器发了
                else if (Ident == CM_RUNGATE_CHECK_INFO)                                     // :1691
                {
                    nClientSendDate = MakeLong(ProcessMsg.DefMessage.Tag, ProcessMsg.DefMessage.Param);  // :1693
                    wClinetSendHour = ProcessMsg.DefMessage.Series;                          // :1694
                    nClientSendRunGateIP = unchecked((int)ProcessMsg.DefMessage.Recog);      // :1695
                    dwClientSendDateTick = MyGetTickCount();                                 // :1696

                    return;                                                                  // :1698
                }

                // 吃药
                else if (Ident == CM_EAT || Ident == CM_AUTOEAT)                             // :1702
                {
                    HumBagItems.Lock();                                                      // :1704
                    try
                    {
                        BagItem = HumBagItems.Find(unchecked((int)ProcessMsg.DefMessage.Recog));   // :1706

                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].Other;            // :1708
                        if (BagItem != null)                                                 // :1709
                        {
                            if (BagItem.StdMode == 0)                                        // :1711
                            {
                                if (BagItem.Shape != 1)                                      // :1713 普通药物
                                {
                                    if (BagItem.AC1 > 0 && BagItem.MAC1 > 0)                 // :1715
                                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].NormalHPMP;   // :1716
                                    else if (BagItem.MAC1 > 0)                               // :1717
                                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].NormalMP;     // :1718
                                    else
                                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].NormalHP;     // :1720
                                }
                                else                                                         // :1722 特殊药物
                                {
                                    if (BagItem.AC1 > 0 && BagItem.MAC1 > 0)                 // :1724
                                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].SpecialHPMP;  // :1725
                                    else if (BagItem.MAC1 > 0)                               // :1726
                                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].SpecialMP;    // :1727
                                    else
                                        TimeInterval = (uint)g_EatItemCDConfig.Hum[btJob].SpecialHP;    // :1729
                                }
                            }
                            else if ((BagItem.StdMode == 2 && (BagItem.Shape == 1 || BagItem.Shape == 2 || BagItem.Shape == 3)) ||   // :1732 传送石系列
                                     (BagItem.StdMode == 3 && (BagItem.Shape == 1 || BagItem.Shape == 2 ||
                                      BagItem.Shape == 3 || BagItem.Shape == 5)))            // :1733 传送卷系列
                            {
                                TimeInterval = 0;                                            // :1735
                            }
                        }
                    }
                    finally
                    {
                        HumBagItems.UnLock();                                                // :1739
                    }

                    if (RunGateTiming.TickDiff(LastEatingItemTick, MyGetTickCount()) < TimeInterval)   // :1742
                    {
                        if (Ident == CM_EAT)                                                 // :1744
                            DefMsg = MakeDefaultMsg(SM_EAT_FAIL, 0, 0, 0, 0);                // :1745
                        else
                            DefMsg = MakeDefaultMsg(SM_AUTOEAT_FAIL, 0, 0, 0, 0);            // :1747

                        sSendText = EncodeRunGateMsg(DefMsg, null, 0);                       // :1749
                        PostSendTextBytes(sSendText);                                        // :1750

                        return;                                                              // :1752
                    }

                    LastEatingItemTick = MyGetTickCount();                                   // :1755
                }

                // 吃药
                else if (Ident == CM_HEROEAT)                                                // :1759
                {
                    HeroBagItems.Lock();                                                     // :1761
                    try
                    {
                        BagItem = HeroBagItems.Find(unchecked((int)ProcessMsg.DefMessage.Recog));   // :1763

                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].Other;       // :1765
                        if (BagItem != null)                                                 // :1766
                        {
                            if (BagItem.StdMode == 0)                                        // :1768
                            {
                                if (BagItem.Shape != 1)                                      // :1770 普通药物
                                {
                                    if (BagItem.AC1 > 0 && BagItem.MAC1 > 0)                 // :1772
                                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].NormalHPMP;  // :1773
                                    else if (BagItem.MAC1 > 0)                               // :1774
                                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].NormalMP;    // :1775
                                    else
                                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].NormalHP;    // :1777
                                }
                                else                                                         // :1779 特殊药物
                                {
                                    if (BagItem.AC1 > 0 && BagItem.MAC1 > 0)                 // :1781
                                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].SpecialHPMP; // :1782
                                    else if (BagItem.MAC1 > 0)                               // :1783
                                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].SpecialMP;   // :1784
                                    else
                                        TimeInterval = (uint)g_EatItemCDConfig.Hero[btHeroJob].SpecialHP;   // :1786
                                }
                            }
                            else if ((BagItem.StdMode == 2 && (BagItem.Shape == 1 || BagItem.Shape == 2 || BagItem.Shape == 3)) ||   // :1789 传送石系列
                                     (BagItem.StdMode == 3 && (BagItem.Shape == 1 || BagItem.Shape == 2 ||
                                      BagItem.Shape == 3 || BagItem.Shape == 5)))            // :1790 传送卷系列
                            {
                                TimeInterval = 0;                                            // :1792
                            }
                        }
                    }
                    finally
                    {
                        HeroBagItems.UnLock();                                               // :1796
                    }

                    if (RunGateTiming.TickDiff(LastHeroEatingItemTick, MyGetTickCount()) < TimeInterval)   // :1799
                    {
                        DefMsg = MakeDefaultMsg(SM_HEROEAT_FAIL, 0, 0, 0, 0);                // :1801
                        sSendText = EncodeRunGateMsg(DefMsg, null, 0);                       // :1802
                        PostSendTextBytes(sSendText);                                        // :1803

                        return;                                                              // :1805
                    }

                    LastHeroEatingItemTick = MyGetTickCount();                               // :1808
                }

                // 检测是否有外挂
                if (!(
                    ProcessMsg.DefMessage.Ident == CM_SPELL &&
                    (
                      ProcessMsg.DefMessage.Tag == 7 || ProcessMsg.DefMessage.Tag == 12 ||
                      ProcessMsg.DefMessage.Tag == 25 || ProcessMsg.DefMessage.Tag == 26 ||
                      ProcessMsg.DefMessage.Tag == 40 || ProcessMsg.DefMessage.Tag == 42 ||
                      ProcessMsg.DefMessage.Tag == 43 || ProcessMsg.DefMessage.Tag == 56 ||
                      ProcessMsg.DefMessage.Tag == 66
                    )
                  ))                                                                         // :1812-1818
                {
                    if (CheckUsePlugin(ProcessMsg))                                          // :1820
                    {
                        if (boChangeMap) boChangeMap = false;                                // :1822
                        return;                                                              // :1823
                    }
                }

                if (boChangeMap) boChangeMap = false;                                        // :1827
            }

            // {$IF CLIENT_ANTIPLUG = 1}  原文 :1830-1873
            IsSendToM2 = true;                                                               // :1831

            if (ProcessMsg.DefMessage.Ident > 10000)                                         // :1833
                IsSendToM2 = false;                                                          // :1834

            if (ProcessMsg.DefMessage.Ident != CM_DISABLECONNECT)                            // :1836
            {
                lock (g_CSRunGatePlug)                                                       // :1838 EnterCriticalSection
                {
                    // :1840-1845
                    if (g_rgpRecvPacket != null &&
                        // 插件未更新完成时，不转发数据包 chongchong 2018-11-21 23:05:35
                        boSendLoadAntiPlug && boSendLoadAntiPlugFinished &&                  // 未加载也处理，不然有问题 boRecvLoadAntiPlug &&
                        dwClientAntiPlugVersion == g_ClientAntiPlugVersion)
                        //(tick_diff(dwRecvLoadAntiPlugTick, MyGetTickCount) >= 2000) then   // 将10秒(10000) -> 换成了2秒(2000)
                    {
                        // {$IF LOG_PLUG_DATA = 1}  :1848 AddMainLogMsg('客户端数据包: ' + IntToStr(...), 0);

                        try
                        {
                            DefMsg = ProcessMsg.DefMessage;                                  // :1852
                            sDataTextBin = ProcessMsg.sMessage;                              // :1853
                            g_rgpRecvPacket(ContextID, DefMsg, sDataTextBin, sDataTextBin.Length, IsSendToM2);  // :1854
                        }
                        catch (Exception E)
                        {
                            AddMainLogMsg("GxxRunGate.RecvPacket error 1, " + E.Message, 1);      // :1858
                        }
                    }
                }
            }

            if (boDelayClose) return;                                                        // :1867

            if (IsSendToM2)                                                                  // :1869
            {
                // 将客户端发来的消息转发到M2Server chongchong 2014-12-27
                SendMessageToServer(ProcessMsg.DefMessage, ProcessMsg.sMessage);             // :1872
            }
            // {$ELSE} 死分支（CLIENT_ANTIPLUG = 0 时才会走 :1876）
        }
    }

    /// <summary>原文 :956/:963 的 <c>PostSendBuffer(P, N)</c>（P 是 PChar 游标）。</summary>
    private void PostSendBuffer(byte[] buf, int offset, int len)
    {
        if (len <= 0) return;
        byte[] slice = new byte[len];
        AnsiBufferSeam.Move(buf, offset, slice, 0, len);
        PostSendBuffer(slice, len);
    }
}
