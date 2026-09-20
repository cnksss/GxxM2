// =====================================================================================
// 源单元：Source/RunGate/MirClientContext.pas（GBK，实测 11,125 LF）
//
// 本文件覆盖的原文行号范围（1:1 逐字移植）：
//   * DoCheckRecvBuffer            1920-2945（约 1,026 行，纯解析主干）
//   * ProcesssSendToClientDelItem  10453-10481
//   * ProcesssSendToClientDelItems 10483-10520
//   * ProcesssSendToClientDropItem 10522-10543
//   * ProcesssSendToClientEatItemOK 10545-10566
//   * ProcesssSendToClientMasterBagToHeroBagOK 10568-10606
//   * ProcesssSendToClientHeroBagToMasterBagOK 10608-10646
//   * DoLogClientPacket            10648-10797
//   * GenerateVerifyCode           10799-10890
//   * SendAntiPlugStreamInfo       10899-10948
//   * SendAntiPlugStreamUnload     10950-10989
//   * SendAntiPlugStreamLoadCache  10991-11021
//   * SendAntiPlugStream           11024-11064
//   * LogPluginData                11069-11120
//
// **未覆盖（接缝，登记在本车道报告 §未覆盖）**：
//   * ProcesssSendToClientSendMyMagic 10251-10303  ┐ 需要 Grobal2_Ex.pas 的 packed record
//   * ProcesssSendToClientSendAddMagic 10305-10340 ┘ TClientMagic / TMagic_C（含 string[N]），
//     属 Grobal2_Ex 车道的类型面，本车道不重复定义（避免跨车道类型重名事故）。
//   * ProcesssSendToClientBagItems     10342-10407  ┐ 需要 TClientItem / TStdItem（同样属
//   * ProcesssSendToClientAddItem      10409-10451  ┘ Grobal2_Ex.pas 的类型面）。
//   以上四个方法按接缝保留签名 + 空实现，注释 `// 接缝：待 Grobal2_Ex.pas 的 ... 移植后接入`。
//
// 原文缺陷 / 易错点（原样保留；测试见 MirClientContextMessagesTests）：
//   M1. :2025 `nPacketIndex := IntValue;` —— IntValue 的唯一赋值点 :2013 处于被注释的 `{ }` 块内，
//       故此处读到的是**未初始化**的栈值。C# 侧确定性置 0，属"原文未定义行为"的可复现化。
//   M2. :2031 `Format('数据包长度错误！%s; %s', [Self.RemoteAddr, Self.sChrName])` —— 判据用的是
//       `Length(sData) < DEFBLOCKSIZE + 1`（23），而进入该分支前已在 :1994 判过 `Length(sData) <= 2`。
//   M3. :2299 `CheckRecvPacketSize(Length(sData), g_nMaxClientPacketSize, DefMsg.Ident)` 在
//       CM_RECVRUNGATE_CHECKCODE 分支**先于**校验码比对执行 —— 超长包会 DelayClose 但仍继续跑比对。
//   M4. :2372/:2344 `ProcessList.Text := sDataText`（两个分支完全相同，只有 RefreshContextProcessList
//       的第二实参不同：True / False）。
//   M5. :2489 `Copy(FClientResponseFileName, 1, Length(RequestClientFileRootPath))` ——
//       RequestClientFileRootPath 为空串时 `Copy(..., 1, 0)` 返回空串，`SameText('') = True`
//       → 任意文件名都通过前缀校验（照抄）。
//   M6. :2496/:2498 使用 `ExtractFilePath(ParamStr(0)) + 'ClientFile\'`（**硬编码反斜杠**，
//       与 sysutils 风格混用）。
//   M7. :2624-2631 小退延时提示连发 3+1 次；:2648-2655 大退延时提示连发 4+1 次（次数不同，照抄）。
//   M8. :2820 `(not ((DefMsg.Ident >= 41000) and (DefMsg.Ident < 50000)))` —— 非法包计数
//       不含插件区间；而 :2896 又单独把 41000..49999 限长 1024。
//   M9. :2875 `tick_diff(dwClientUploadPickItemsTick, MyGetTickCount) < g_Config.dwClientUploadPickItemsTime * 1000`
//       —— 两个 tick_diff 调用之间没取同一个 tick（照抄，可能导致日志值与判据不一致）。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.RunGate.FrmMainSeam;
using static GXX.RunGate.AnsiStrSeam;
using static GXX.Core.Rtl.DelphiRTL;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate;

public partial class TMirClientContext
{
    // =================================================================================
    // 原文 :1920-2945  function TMirClientContext.DoCheckRecvBuffer(var S: string): Boolean;
    //   —— 常量 SE_* 是 :1922-1926 的（Windows 提权权限号，只在反调试分支用；本移植未用到）。
    // 返回语义：true = "已成功解出一个包"（原文 :2000 `Result := True`），
    //   与基类 IocpUtils.DoCheckRecvBuffer 的"恒 False"（:595-598）完全不同。
    // =================================================================================
    protected override bool DoCheckRecvBuffer(ref string S)
    {
        int ErrCode = 0;                                     // :1964
        bool Result = false;                                 // :1965

        // 当延时关闭过程中，不再接受内核发来的数据包 chongchong 2016-12-16
        if (boDelayClose)                                    // :1968
        {
            S = "";                                          // :1970
            return Result;                                   // :1971
        }

        if (AnsiLen(S) == 0) return Result;                  // :1974 Length(S) = 0

        try
        {
            // 手动管理心跳 chongchong 2014-07-04
            // 心跳包不用处理 chongchong 2014-07-04
            int Index = Pos("*", S);                         // :1979
            string sTemp;
            if (Index > 0)                                   // :1980
                sTemp = Copy(S, 1, Index - 1) + Copy(S, Index + 1, AnsiLen(S));   // :1981
            else
                sTemp = S;                                   // :1983

            if (AnsiLen(sTemp) == 0)                         // :1985
            {
                S = "";                                      // :1987
                return Result;                               // :1988
            }

            ErrCode = 1;                                     // :1991

            string sData = "";
            sTemp = HUtil32.ArrestStringEx_Ansi(sTemp, '#', '!', ref sData);   // :1993
            if (AnsiLen(sData) <= 2)                         // :1994 Length(sData) <= 2
            {
                sTemp = "";                                  // :1996
            }
            else
            {
                Result = true;                               // :2000

                ErrCode = 2;                                 // :2002

                TRunGate RunGate = GetRunGateAsTRunGate();   // :2004-2009

                // { 原文 :2011-2021 整段被注释：
                //   ErrCode := 3;
                //   IntValue := Str_ToInt(sData[1], 99);;
                //   if IntValue = nPacketIndex then begin Inc(nPacketErrCount); S := sTemp;
                //     AddMainLogMsg(...); Exit; end; }

                ErrCode = 4;                                 // :2023
                // 修改包编号
                // 原文缺陷 M1：IntValue 的唯一赋值点在注释块内 → 此处是未初始化值，C# 侧确定性置 0。
                int IntValue = 0;                            // 原文 :2025 nPacketIndex := IntValue;
                nPacketIndex = IntValue;                     // :2025

                // Len包含前分隔标记 #序号!
                if (AnsiLen(sData) < DEFBLOCKSIZE + 1)       // :2028
                {
                    S = sTemp;                               // :2030
                    AddMainLogMsg(Format("数据包长度错误！%s; %s", RemoteAddr, sChrName), 1);   // :2031
                    return Result;                           // :2032
                }

                ErrCode = 5;                                 // :2035
                S = sTemp;                                   // :2036

                ErrCode = 6;                                 // :2038

                string sDataText, sDataMsg;
                TDefaultMessage DefMsg;

                // 客户端发来第一个数据包
                if (boStartLogon)                            // :2041
                {
                    ErrCode = 7;                             // :2043
                    if (!CheckRecvPacketSize(AnsiLen(sData), 384, 0)) return Result;   // :2044

                    ErrCode = 8;                             // :2046
                    boStartLogon = false;                    // :2047
                    sDataText = Copy(sData, 2, int.MaxValue);   // :2048 Copy(sData, 2, MaxInt)
                    if (AnsiLen(sDataText) > 0)              // :2049
                    {
                        ErrCode = 9;                         // :2051
                        sDataText = DecodeString(sDataText); // :2052

                        // {$IF REGISTER_TEST = 1} 死代码（REGISTER_TEST = 0）：:2054-2058 的 3 条日志

                        ErrCode = 10;                        // :2060
                        IntValue = MirClientContextUnit.SubStringOccurences("/", sDataText, false);   // :2061
                        string sAccount = "";
                        string sChrName = "";
                        string sSessionID = "";
                        string sClientVersion = "";
                        string sMachineID = "";
                        if (IntValue <= 5)                   // :2062
                        {
                            ErrCode = 11;                    // :2064
                            if (g_boCheckClientPassword)     // :2065 原文写的是 g_boCheckClientPassWord
                            {
                                ErrCode = 12;                // :2067
                                //老端直接中断
                                AddMainLogMsg("网关启用了密码检测--老客户端，无法登录！", 1);   // :2069
                                DelayClose(100);             // :2070 // Close
                                return Result;               // :2071
                            }
                            else
                            {
                                ErrCode = 13;                // :2075
                                sDataText = Copy(sDataText, 3, AnsiLen(sDataText) - 2);   // :2076
                                sDataText = HUtil32.GetValidStr3(sDataText, ref sAccount, new char[] { '/' });       // :2077
                                sDataText = HUtil32.GetValidStr3(sDataText, ref sChrName, new char[] { '/' });       // :2078
                                sDataText = HUtil32.GetValidStr3(sDataText, ref sSessionID, new char[] { '/' });     // :2079
                                sDataText = HUtil32.GetValidStr3(sDataText, ref sClientVersion, new char[] { '/' }); // :2080
                            }
                        }
                        else
                        {
                            ErrCode = 14;                    // :2085
                            // 我们的端进行密码判断
                            sDataText = Copy(sDataText, 3, AnsiLen(sDataText) - 2);   // :2087
                            string sClientPassWord = "", sKey = "", sCheckKey = "", sRunLoginCode = "";
                            string sUserMachineID = "", sScreenWidth = "", sScreenHeight = "";
                            string sGameLoginConfigUrlMD5 = "";
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sClientPassWord, new char[] { '/' });       // :2088
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sAccount, new char[] { '/' });              // :2089
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sChrName, new char[] { '/' });              // :2090
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sSessionID, new char[] { '/' });            // :2091
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sClientVersion, new char[] { '/' });        // :2092
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sKey, new char[] { '/' });                  // :2093
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sCheckKey, new char[] { '/' });             // :2094
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sRunLoginCode, new char[] { '/' });         // :2095
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sMachineID, new char[] { '/' });            // :2096
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sUserMachineID, new char[] { '/' });        // :2097

                            sDataText = HUtil32.GetValidStr3(sDataText, ref sScreenWidth, new char[] { '/' });          // :2099
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sScreenHeight, new char[] { '/' });         // :2100
                            sDataText = HUtil32.GetValidStr3(sDataText, ref sGameLoginConfigUrlMD5, new char[] { '/' });// :2101

                            ErrCode = 15;                    // :2104
                            // EncryString_LF(IntToStr(g_PKey^)) —— 解密口令密钥
                            IntValue = StrToIntDef(EncryptUnitLf.DecryString_LF(sKey), 0);   // :2105
                            sClientPassWord = GXX.Core.Crypto.EncryptUnit.DecryStringK(sClientPassWord,
                                GXX.Core.Crypto.EncryptUnit.GetKeyValue(IntValue));          // :2106

                            ErrCode = 16;                    // :2108
                            if (g_boCheckClientPassword && !SameText(sClientPassWord, g_sClientPassWord))   // :2109
                            {
                                AddMainLogMsg(Format("登录密码错误; Account:%s; ChrName:%s; Password:%s; IP:%s",
                                    sAccount, sChrName, sClientPassWord, RemoteAddr), 1);   // :2111
                                DelayClose(100);             // :2113 // Close
                                return Result;               // :2114
                            }

                            ErrCode = 17;                    // :2117
                            if (IsBlockMac(sMachineID) && RunGate != null)   // :2118
                            {
                                ErrCode = 18;                // :2120
                                RunGate.nTotalAttackCount = unchecked(RunGate.nTotalAttackCount + 1);   // :2121 InterlockedIncrement
                                RunGate.dwClearTempTick = MyGetTickCount();      // :2122
                                RunGate.dwResotreDefenseTick = MyGetTickCount(); // :2123

                                ErrCode = 19;                // :2125
                                // { TODO -ochongchong -c增加 : 受攻击防御调为1级 【2013-08-30】 }
                                if (g_boDefenseToLevel1 && (uint)RunGate.nTotalAttackCount >= g_dwDefenseToLevel1)   // :2127
                                {
                                    if (RunGate.dwCurDefenseLevel != 1)   // :2129
                                    {
                                        RunGate.dwCurDefenseLevel = 1;    // :2131
                                        AddMainLogMsg("自动调整防御等级为1级", 1);   // :2132
                                    }
                                }

                                ErrCode = 20;                // :2136
                                AddMainLogMsg("过滤MAC: " + sMachineID, 1);   // :2137
                                DelayClose(100);             // :2138 // Close
                                return Result;               // :2139
                            }

                            ErrCode = 21;                    // :2142
                            // { 原文 :2143-2173 整段被注释：VMProtect + 登录器 MD5 授权校验 }
                        }

                        // 登录 MAC 表计数
                        g_LoginMACPlayerList.Lock();                     // :2176
                        try
                        {
                            Index = g_LoginMACPlayerList.IndexOf(sMachineID);   // :2178
                            if (Index >= 0)                              // :2179
                            {
                                object stored = g_LoginMACPlayerList.GetObject(Index);
                                int TempValue = stored == null ? 0 : (int)stored;   // :2181
                                if (g_boOneMACLimitePlayer && TempValue >= g_nOneMACLimitePlayerCount)   // :2182
                                {
                                    DefMsg = MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);        // :2184

                                    sDataText = AnsiStringFromBytes(EncodeString("单台机器登录达到最大数量限制!"));   // :2186
                                    var enc = EncodeRunGateMsg(DefMsg, EncodeString("单台机器登录达到最大数量限制!"),
                                        EncodeString("单台机器登录达到最大数量限制!").Length);            // :2187
                                    PostSendTextBytes(enc);                                              // :2188

                                    DelayClose(200);                         // :2190
                                    return Result;                           // :2191
                                }

                                TempValue = TempValue + 1;                   // :2194
                                g_LoginMACPlayerList.SetObject(Index, TempValue);   // :2195 TObject(TempValue)
                            }
                            else
                            {
                                g_LoginMACPlayerList.AddObject(sMachineID, 1);   // :2199 TObject(1)
                            }
                        }
                        finally
                        {
                            g_LoginMACPlayerList.UnLock();               // :2202
                        }

                        ErrCode = 25;                            // :2205
                        this.sAccount = sAccount;                // :2206
                        this.sChrName = sChrName;                // :2207
                        nSessionID = StrToIntDef(sSessionID, 0); // :2208
                        sVersion = sClientVersion;               // :2209
                        this.sMachineID = sMachineID;            // :2210
                        boIsOldClient = GetExVersionNO(StrToIntDef(sClientVersion, 0), out IntValue) == 0;   // :2211

                        ErrCode = 26;                            // :2213
                        if (AnsiLen(sChrName) > 0)               // :2214
                        {
                            ErrCode = 27;                        // :2216

                            string WS = sChrName;                // :2218
                            for (int I = 0; I < WS.Length; I++)  // :2219 `for I := 1 to Length(WS)`
                            {
                                if (WS[I] == '\\' || WS[I] == '/' || WS[I] == ':' ||
                                    WS[I] == '*' || WS[I] == '?' || WS[I] == '"' ||
                                    WS[I] == '<' || WS[I] == '>' || WS[I] == '|')   // :2221-2223
                                {
                                    WS = WS.Substring(0, I) + "-" + WS.Substring(I + 1);   // :2225 WS[I] := '-'
                                }
                            }

                            sLogFileChrName = WS;                // :2229

                            ErrCode = 28;                        // :2231
                            FWriteLogLocker.Lock();              // :2232
                            try
                            {
                                ErrCode = 29;                    // :2234
                                string sFilePath = g_sLogClientPacketDir + FormatDateTime("yyyy-mm-dd", Now()) + "\\";   // :2235
                                FLogPakcetFileName = sFilePath + sLogFileChrName + ".txt";                              // :2236
                                if (!DirectoryExists(sFilePath))                                        // :2237
                                    ForceDirectories(sFilePath);                                        // :2238
                            }
                            finally
                            {
                                FWriteLogLocker.UnLock();        // :2240
                            }
                        }
                    }

                    ErrCode = 30;                                // :2245
                    // 第一个包和后面的包不一样，第一个包是原样转发到服务器
                    if (RunGate != null)                         // :2247
                    {
                        string sData2 = "#" + sData + "!";       // :2249
                        RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                            GbkBytes(sData2), AnsiLen(sData2));  // :2250
                    }

                    // (* 原文 :2254-2272 整段被注释：g_rgpStartContext 插件回调（另一处在 :2911 是活的）*)
                    // g_PluginManager.HookContextStart(Self);
                }
                // ---------------------------------普通数据包 else if boStartLogon
                else if (RunGate != null && RunGate.TcpClient.Active)   // :2275  UseIocpClient = 1 活分支
                {
                    ErrCode = 31;                            // :2277
                    string sDefMsg = Copy(sData, 2, DEFBLOCKSIZE);   // :2278-2279 SetLength + Move

                    ErrCode = 32;                            // :2281
                    // 包附加文字信息
                    sDataMsg = "";                           // :2283
                    IntValue = AnsiLen(sData) - DEFBLOCKSIZE - 1;   // :2284
                    if (IntValue > 0)                        // :2285
                    {
                        sDataMsg = Copy(sData, 2 + DEFBLOCKSIZE, IntValue);   // :2287-2288
                    }

                    ErrCode = 33;                            // :2291
                    // 包头
                    DefMsg = EDcode.DecodeMessage(GbkBytes(sDefMsg), DEFBLOCKSIZE);   // :2293

                    ErrCode = 34;                            // :2295
                    if (DefMsg.Ident == CM_RECVRUNGATE_CHECKCODE)     // :2296
                    {
                        ErrCode = 35;                        // :2298
                        if (!CheckRecvPacketSize(AnsiLen(sData), g_nMaxClientPacketSize, DefMsg.Ident)) return Result;   // :2299

                        ErrCode = 36;                        // :2301
                        // {$I VMProtectBegin.inc} —— 原文 :2303-2322 是 djb2 变体散列（**不是** VMProtect 代码）
                        {
                            sDataText = this.sAccount + this.sChrName + IntToStr(dwSendCheckCode);   // :2303

                            uint dwHashCode = 0;             // :2305
                            int P2 = 0;                      // :2306 P := PChar(sDataText)
                            byte[] hashIn = GbkBytes(sDataText);
                            int Len2 = hashIn.Length;        // :2308
                            while (Len2 > 0)                 // :2309
                            {
                                dwHashCode = unchecked((dwHashCode << 4) + hashIn[P2]);           // :2311
                                uint Temp = dwHashCode & 0xF0000000;                              // :2312
                                if (Temp != 0)                                                    // :2313
                                    dwHashCode = unchecked((dwHashCode ^ (Temp >> 24)) & ~0xF0000000u);   // :2314
                                Len2--;                                                           // :2315
                                P2++;                                                             // :2316
                            }

                            if (unchecked((uint)DefMsg.Recog) == dwHashCode)   // :2319
                            {
                                boRecvCheckCodeOK = true;        // :2321
                            }
                        }
                        // {$I VMProtectEnd.inc}
                    }

                    else if (DefMsg.Ident == CM_SENDPROCESS_LIST ||
                             DefMsg.Ident == CM_IOCP_SENDPROCESS_LIST ||
                             DefMsg.Ident == CM_IOCP_SENDPROCESS_LIST2)   // :2326
                    {
                        ErrCode = 40;                        // :2328
                        if (!CheckRecvPacketSize(AnsiLen(sData), 10000, DefMsg.Ident)) return Result;   // :2329

                        ErrCode = 41;                        // :2331
                        try
                        {
                            sDataText = zLibDecodeString(sDataMsg);   // :2333
                        }
                        catch
                        {
                            return Result;                   // :2335
                        }

                        ErrCode = 42;                        // :2338
                        if (AnsiLen(sDataText) == DefMsg.Recog)   // :2339 Length(sDataText) = DefMsg.Recog
                        {
                            ErrCode = 44;                    // :2341
                            ProcessList.Lock();              // :2342
                            try
                            {
                                ProcessList.Text = sDataText;   // :2344
                            }
                            finally
                            {
                                ProcessList.UnLock();        // :2346
                            }

                            ErrCode = 44;                    // :2349 原文如此（ErrCode 重复赋值 44）
                            if (FrmMain != null)             // :2350 Assigned(FrmMain)
                            {
                                FrmMain.RefreshContextProcessList(this, true);   // :2352
                            }
                        }
                    }

                    else if (DefMsg.Ident == CM_IOCP_SENDDIR_LIST)   // :2357
                    {
                        ErrCode = 45;                        // :2359
                        try
                        {
                            sDataText = zLibDecodeString(sDataMsg);   // :2361
                        }
                        catch
                        {
                            return Result;                   // :2363
                        }

                        ErrCode = 46;                        // :2366
                        if (AnsiLen(sDataText) == DefMsg.Recog)   // :2367
                        {
                            ErrCode = 47;                    // :2369
                            ProcessList.Lock();              // :2370
                            try
                            {
                                ProcessList.Text = sDataText;   // :2372
                            }
                            finally
                            {
                                ProcessList.UnLock();        // :2374
                            }

                            ErrCode = 48;                    // :2377
                            if (FrmMain != null)             // :2378
                            {
                                FrmMain.RefreshContextProcessList(this, false);   // :2380
                            }
                        }
                    }

                    else if (DefMsg.Ident == CM_SENDSCREENSHOT_GAME ||
                             DefMsg.Ident == CM_IOCP_SENDSCREENSHOT_GAME)   // :2385
                    {
                        ErrCode = 50;                        // :2387
                        if (!CheckRecvPacketSize(AnsiLen(sData), 10000, DefMsg.Ident)) return Result;   // :2388

                        ErrCode = 51;                        // :2390
                        if (AnsiLen(sDataMsg) == DefMsg.Recog)   // :2391
                        {
                            ErrCode = 52;                    // :2393
                            FScreenshotStream.Lock();        // :2394
                            try
                            {
                                ErrCode = 53;                // :2396
                                if (DefMsg.Series == 0)      // :2397
                                {
                                    FScreenshotStream.Clear();   // :2399
                                }

                                ErrCode = 54;                // :2402
                                if (FScreenshotStream.Size > 4 << 20)   // :2403 `4 shl 20`
                                    FScreenshotStream.Clear();          // :2404

                                ErrCode = 55;                // :2406
                                byte[] P = new byte[DefMsg.Param + 1];   // :2407 GetMem(P, DefMsg.Param + 1)
                                try
                                {
                                    EDcode.DecodeBuffer(GbkBytes(sDataMsg), AnsiLen(sDataMsg), P, DefMsg.Param);   // :2409
                                    FScreenshotStream.Write(P, 0, DefMsg.Param);                                   // :2410
                                }
                                finally
                                {
                                    // :2412 FreeMem(P)
                                }

                                ErrCode = 56;                // :2415
                                if (DefMsg.Series == DefMsg.Tag - 1)   // :2416
                                {
                                    // 如果角色名包含 /\:*?<>| 这些符号，创建目录都不行
                                    string sDir = g_ScreenshotPath;   // :2419 { + Self.sChrName + '\'}
                                    if (!DirectoryExists(sDir))       // :2420
                                        ForceDirectories(sDir);       // :2421

                                    ErrCode = 57;                // :2423
                                    if (DirectoryExists(sDir))   // :2424
                                    {
                                        ErrCode = 58;            // :2426
                                        string sFileName = sDir + FormatDateTime("yyyymmddhhnnss", Now()) + ".jpg";   // :2427
                                        FScreenshotStream.Position = 0;
                                        FScreenshotStream.SaveToFileSafe(sFileName);   // :2428 FScreenshotStream.SaveToFile

                                        ErrCode = 59;            // :2430
                                        if (FrmMain != null)     // :2431
                                        {
                                            FrmMain.RefreshContextStatusText("保存游戏快照到文件 " + sFileName);   // :2433
                                        }
                                    }

                                    ErrCode = 60;                // :2437
                                    FScreenshotStream.Clear();   // :2438
                                }
                            }
                            finally
                            {
                                FScreenshotStream.UnLock();  // :2441
                            }
                        }
                    }

                    else if (DefMsg.Ident == CM_IOCP_RESPONSE_FILE ||
                             DefMsg.Ident == CM_IOCP_RESPONSE_FILE2)   // :2446
                    {
                        ErrCode = 61;                        // :2448
                        if (!CheckRecvPacketSize(AnsiLen(sData), 100000, DefMsg.Ident)) return Result;   // :2449

                        ErrCode = 62;                        // :2451
                        if (AnsiLen(sDataMsg) == DefMsg.Recog)   // :2452
                        {
                            FClientResponseFileTick = MyGetTickCount();       // :2454
                            FClientResponseFileIndex = DefMsg.Series;         // :2455
                            FClientResponseFileCount = DefMsg.Tag;            // :2456

                            ErrCode = 63;                    // :2458
                            FClientResponseFileStream.Lock();   // :2459
                            try
                            {
                                ErrCode = 63;                // :2461 原文如此（ErrCode 重复赋值 63）
                                if (DefMsg.Series <= 1)      // :2462
                                {
                                    FClientResponseFileStream.Clear();   // :2464

                                    if (DefMsg.Series == 0)  // :2466
                                    {
                                        FClientResponseFileName = DecodeString(sDataMsg);   // :2468
                                        return Result;       // :2469
                                    }
                                }

                                ErrCode = 64;                // :2473
                                if (FClientResponseFileStream.Size > 300 << 20)   // :2474 `300 shl 20`
                                    FClientResponseFileStream.Clear();            // :2475

                                ErrCode = 65;                // :2477
                                byte[] P = new byte[DefMsg.Param + 1];   // :2478 GetMem(P, DefMsg.Param + 1)
                                try
                                {
                                    EDcode.DecodeBuffer(GbkBytes(sDataMsg), AnsiLen(sDataMsg), P, DefMsg.Param);   // :2480
                                    FClientResponseFileStream.Write(P, 0, DefMsg.Param);                           // :2481
                                }
                                finally
                                {
                                    // :2483 FreeMem(P)
                                }

                                ErrCode = 66;                // :2486
                                if (DefMsg.Series == DefMsg.Tag)   // :2487
                                {
                                    string sFileName;
                                    // 原文缺陷 M5：RequestClientFileRootPath 为空时 Copy(...,1,0) = '' → SameText('') = True
                                    if (SameText(RequestClientFileRootPath,
                                            Copy(FClientResponseFileName, 1, AnsiLen(RequestClientFileRootPath))))   // :2489
                                    {
                                        FClientResponseFileTick = 0;     // :2491
                                        FClientResponseFileIndex = 0;    // :2492
                                        FClientResponseFileCount = 0;    // :2493

                                        if (SaveResponseClientFileRoot == "")   // :2495
                                            sFileName = ExtractFilePath(ParamStr0) + "ClientFile\\" +
                                                Copy(FClientResponseFileName, AnsiLen(RequestClientFileRootPath) + 1, int.MaxValue);   // :2496
                                        else
                                            sFileName = ExtractFilePath(ParamStr0) + "ClientFile\\" + SaveResponseClientFileRoot + "\\" +
                                                Copy(FClientResponseFileName, AnsiLen(RequestClientFileRootPath) + 1, int.MaxValue);   // :2498

                                        string sDir = ExtractFilePath(sFileName);   // :2500

                                        if (!DirectoryExists(sDir))     // :2502
                                            ForceDirectories(sDir);     // :2503

                                        if (DirectoryExists(sDir))      // :2505
                                        {
                                            if (DefMsg.Ident == CM_IOCP_RESPONSE_FILE)   // :2507
                                            {
                                                try
                                                {
                                                    FClientResponseFileStream.Position = 0;   // :2512
                                                    byte[] raw = FClientResponseFileStream.ToArray();
                                                    byte[] ms = ZDecompressStream(raw);       // :2513 ZDecompressStream(FClientResponseFileStream, MS)

                                                    WriteAllBytesSafe(sFileName, ms);         // :2515 MS.SaveToFile(sFileName)

                                                    if (FrmMain != null)                      // :2517
                                                    {
                                                        FrmMain.RefreshContextStatusText("保存客户端文件 " + sFileName);   // :2519
                                                    }
                                                }
                                                catch
                                                {
                                                    // :2521 原文为空 except
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    FClientResponseFileStream.Position = 0;   // :2530

                                                    WriteAllBytesSafe(sFileName, FClientResponseFileStream.ToArray());   // :2532

                                                    if (FrmMain != null)                      // :2534
                                                    {
                                                        FrmMain.RefreshContextStatusText("保存客户端文件 " + sFileName);   // :2536
                                                    }
                                                }
                                                catch
                                                {
                                                    // :2538 原文为空 except
                                                }
                                            }
                                        }
                                    }

                                    ErrCode = 690;                   // :2544
                                    FClientResponseFileStream.Clear();   // :2545
                                }
                            }
                            finally
                            {
                                FClientResponseFileStream.UnLock();   // :2548
                            }
                        }
                    }

                    else if (DefMsg.Ident == CM_SENDSCREENSHOT ||
                             DefMsg.Ident == CM_IOCP_SENDSCREENSHOT)   // :2553
                    {
                        ErrCode = 70;                        // :2555
                        if (!CheckRecvPacketSize(AnsiLen(sData), 10000, DefMsg.Ident)) return Result;   // :2556
                        ErrCode = 71;                        // :2557
                        if (AnsiLen(sDataMsg) == DefMsg.Recog)   // :2558
                        {
                            ErrCode = 72;                    // :2560
                            FScreenshotStream.Lock();        // :2561
                            try
                            {
                                ErrCode = 73;                // :2563
                                if (DefMsg.Series == 0)      // :2564
                                {
                                    FScreenshotStream.Clear();   // :2566
                                }

                                ErrCode = 74;                // :2569
                                if (FScreenshotStream.Size > 4 << 20)   // :2570
                                    FScreenshotStream.Clear();          // :2571

                                ErrCode = 75;                // :2573
                                byte[] P = new byte[DefMsg.Param + 1];   // :2574
                                try
                                {
                                    EDcode.DecodeBuffer(GbkBytes(sDataMsg), AnsiLen(sDataMsg), P, DefMsg.Param);   // :2576
                                    FScreenshotStream.Write(P, 0, DefMsg.Param);                                   // :2577
                                }
                                finally
                                {
                                    // :2579 FreeMem(P)
                                }

                                ErrCode = 76;                // :2582
                                if (DefMsg.Series == DefMsg.Tag - 1)   // :2583
                                {
                                    // 如果角色名包含 /\:*?<>| 这些符号，创建目录都不行
                                    string sDir = g_ScreenshotPath;   // :2586 { + Self.sChrName + '\'}
                                    if (!DirectoryExists(sDir))       // :2587
                                        ForceDirectories(sDir);       // :2588

                                    ErrCode = 77;                // :2590
                                    if (DirectoryExists(sDir))   // :2591
                                    {
                                        string sFileName = sDir + FormatDateTime("yyyymmddhhnnss", Now()) + ".jpg";   // :2593
                                        FScreenshotStream.Position = 0;
                                        FScreenshotStream.SaveToFileSafe(sFileName);   // :2594

                                        if (FrmMain != null)     // :2596
                                        {
                                            FrmMain.RefreshContextStatusText("保存桌面快照到文件 " + sFileName);   // :2598
                                        }
                                    }

                                    ErrCode = 78;                // :2602
                                    FScreenshotStream.Clear();   // :2603
                                }
                            }
                            finally
                            {
                                FScreenshotStream.UnLock();  // :2606
                            }
                        }
                    }

                    else if (DefMsg.Ident == CM_SOFTCLOSE)   // :2611
                    {
                        if (g_nClientLogoutDelay <= 0)       // :2613
                        {
                            boValidClose = true;             // :2615
                            ProcessClientMessage(DefMsg, GbkBytes(sDataMsg));   // :2616
                        }
                        else
                        {
                            dwDelayClientLogoutTick = MyGetTickCount();   // :2620
                            boDelayClientLogout = true;                   // :2621
                            nClientLogoutDelay = g_nClientLogoutDelay;    // :2622

                            string sTemp2 = "[提示] 角色小退被延时到 " + IntToStr(nClientLogoutDelay) + " 秒后，请等待……";   // :2624
                            // 原文缺陷 M7：小退提示连发 3 次 + 1 次
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2625
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2626
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2627
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2628

                            sTemp2 = "本服已开启小退延时功能，正在小退..." + IntToStr(nClientLogoutDelay) + "s";   // :2630
                            SendMessaggeToClient(sTemp2, 0, 249, 255);    // :2631
                        }
                    }

                    else if (DefMsg.Ident == CM_IOCP_APPEXIT)   // :2635
                    {
                        if (g_nClientCloseDelay <= 0)           // :2637
                        {
                            boValidClose = true;                // :2639
                            ProcessClientMessage(DefMsg, GbkBytes(sDataMsg));   // :2640
                        }
                        else
                        {
                            dwDelayClientCloseTick = MyGetTickCount();   // :2644
                            boDelayClientClose = true;                   // :2645
                            nClientCloseDelay = g_nClientCloseDelay;     // :2646

                            string sTemp2 = "[提示] 角色大退被延时到 " + IntToStr(nClientCloseDelay) + " 秒后，请等待……";   // :2648
                            // 原文缺陷 M7：大退提示连发 4 次 + 1 次（与小退的 3 次不同）
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2649
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2650
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2651
                            SendMessaggeToClient(sTemp2, 0, 0, 255);      // :2652

                            sTemp2 = "本服已开启大退延时功能，正在大退..." + IntToStr(nClientCloseDelay) + "s";   // :2654
                            SendMessaggeToClient(sTemp2, 0, 249, 255);    // :2655
                        }
                    }

                    // 网关后门代码 chongchong 2016-08-01
                    else if (DefMsg.Ident == CM_RUNGATEDOOR)   // :2660
                    {
                        // {$IF NEED_REGISTER <> 0}  原文 :2662-2740（**活代码**）
                        //   原文用 TLbRSA(1024) 解 sDataMsg，按 sTemp[20] 的 #1/#3/#7 分别：
                        //     #1 → 覆写全局常量 RUNGATECODE（Move(sTemp[10], RUNGATECODE, SizeOf)）
                        //     #3 → ntdll!RtlAdjustPrivilege + NtSetInformationProcess(ProcessBreakOnTermination) + ExitProcess
                        //     #7 → RunGate.TcpClient.SendServerMsg(GM_RANDOM_DATA, 0, 0, 0, nil, 0)
                        //   按 docs/转换开发文档.md §2.3（VMProtect/提权/反调试均不移植）**本车道未移植**；
                        //   仅保留 :2741 的 Exit 语义（该行在 {$IF} 块**之外**，是活代码）。
                        // {$IFEND}
                        return Result;   // :2741 Exit
                    }

                    // {$IF CLIENT_ANTIPLUG = 1}  原文 :2744-2796
                    else if (DefMsg.Ident == CM_IOCP_ANTIPLUG_CRC)   // :2745
                    {
                        dwRecvClientAntiplugCRC = unchecked((uint)DefMsg.Recog);   // :2747
                    }

                    else if (DefMsg.Ident == CM_DISABLECONNECT)   // :2750
                    {
                        ErrCode = 89;                        // :2752
                        AddMainLogMsg(Format("反外挂模块检测到非法外挂！; 用户:%s; 代码:%d", this.sChrName, DefMsg.Param), 1);   // :2753
                        SendMessaggeToClient("请关闭非法外挂后重新登陆!", 1, g_Config.btMsgFColor, g_Config.btMsgBColor);   // :2754

                        // (* 原文 :2756-2788 整段被注释：
                        //    g_boAutoAddToDisableChrLoginList / g_DisableChrLoginList /
                        //    g_boCheckPluginTriggerScript → CM_SENDCHECKPLUGIN *)

                        if (!boDelayClose)                   // :2790
                        {
                            DelayClose(3000);                // :2792
                        }
                    }
                    // {$IFEND}

                    // 客户端发来心跳
                    else if (DefMsg.Ident == CM_RUNGATE_HEARTBEAT)   // :2799
                    {
                        return Result;                       // :2801 Exit
                    }

                    // 不让客户端发些消息，防止别人刷包 2020-03-23 22:22:16
                    else if (DefMsg.Ident == CM_RUNGATE_SENDFILTERMSG)   // :2805
                    {
                        return Result;                       // :2807 Exit
                    }

                    // 处理用户消息
                    else                                     // :2811
                    {
                        ErrCode = 90;                        // :2813

                        // {$IF LOG_PLUG_DATA = 1}  原文 :2815-2817
                        LogPluginData(false, false, DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag,
                            DefMsg.Series, "其他");

                        if (g_boCheckClientPacketLegal && g_nCheckClientPacketCount > 0 && DefMsg.Ident >= 20000 &&
                            !(DefMsg.Ident >= 41000 && DefMsg.Ident < 50000))   // :2819-2820
                        {
                            nIllegalPacketCount++;               // :2822 Inc

                            ErrCode = 91;                        // :2824
                            if (nIllegalPacketCount >= g_nCheckClientPacketCount)   // :2825
                            {
                                switch (g_BlockMethod)           // :2827
                                {
                                    case TBlockIPMethod.bmTempBlock: AddTempBlockIP(RemoteAddr); break;   // :2828
                                    case TBlockIPMethod.bmBlockList: AddBlockIP(RemoteAddr); break;       // :2829
                                }
                                AddMainLogMsg(Format("非法数据包，踢除连接; 用户:%s; IP:%s; 错误代码:%d",
                                    this.sChrName, RemoteAddr, DefMsg.Ident), 1);   // :2831
                                DelayClose(100);                 // :2832 // Close
                            }

                            return Result;                       // :2835 Exit
                        }

                        // 如果客户端发来的包超过限定的包长度
                        ErrCode = 92;                        // :2839
                        if (DefMsg.Ident == CM_CLIENTDATAFILE)   // :2840
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 2048, DefMsg.Ident)) return Result;   // :2842
                        }
                        else if (DefMsg.Ident == CM_GUILDUPDATENOTICE || DefMsg.Ident == CM_GUILDUPDATERANKINFO)   // :2844
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 4096, DefMsg.Ident)) return Result;   // :2846
                        }
                        // 可能有输入框的内容 2019-08-02
                        else if (DefMsg.Ident == CM_MERCHANTDLGSELECT)   // :2849
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 2048, DefMsg.Ident)) return Result;   // :2851
                        }
                        else if (DefMsg.Ident == 3030 /*CM_SAY*/)        // :2853
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 256, DefMsg.Ident)) return Result;    // :2855
                        }
                        // 交易市场可能卖多个东西，这里限制改掉
                        else if (DefMsg.Ident == CM_TradingSELLITEMS)   // :2858 原文写的是 CM_TRADINGSELLITEMS
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 8192, DefMsg.Ident)) return Result;   // :2860
                        }
                        // 元宝交易出售物品
                        else if (DefMsg.Ident == CM_SENDSELLGAMEGOLDDALITEM)   // :2863
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 512, DefMsg.Ident)) return Result;    // :2865
                        }

                        // 上传客户端内挂捡物设置 2020-03-08 22:38:29
                        else if (DefMsg.Ident == CM_UPLOAD_PICK_ITMES)   // :2869
                        {
                            // 如果未开启上传功能 2020-03-08 23:14:55
                            if (!boEnableClientUploadPickItems) return Result;   // :2872

                            // 发送间隔过低
                            // 原文缺陷 M9：两次 tick_diff 用了不同的 MyGetTickCount 采样
                            if (RunGateTiming.TickDiff(dwClientUploadPickItemsTick, MyGetTickCount()) <
                                g_Config.dwClientUploadPickItemsTime * 1000)     // :2875
                            {
                                AddMainLogMsg(Format("上传内挂捡物配置间隔过短 [%d毫秒]; IP:%s",
                                    RunGateTiming.TickDiff(dwClientUploadPickItemsTick, MyGetTickCount()), RemoteAddr), 5);   // :2877
                                return Result;                   // :2878
                            }

                            if (DefMsg.Param > MAX_UPLOAD_PICKITEMS) return Result;   // :2881

                            dwClientUploadPickItemsTick = MyGetTickCount();   // :2883

                            // MAX_UPLOAD_PICKITEMS * 2 + 5120
                            if (!CheckRecvPacketSize(AnsiLen(sData), MAX_UPLOAD_PICKITEMS * 2 + 5120, DefMsg.Ident))
                                return Result;                   // :2886
                        }

                        // {$IF CLIENT_ANTIPLUG = 1}
                        // 反外挂模块发来的IP列表
                        else if (DefMsg.Ident == 41002)          // :2891
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 81920, DefMsg.Ident)) return Result;   // :2893
                        }
                        // {$IFEND}
                        else if (DefMsg.Ident >= 41000 && DefMsg.Ident < 50000)   // :2896
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), 1024, DefMsg.Ident)) return Result;   // :2898
                        }
                        else
                        {
                            if (!CheckRecvPacketSize(AnsiLen(sData), g_nMaxClientPacketSize, DefMsg.Ident))
                                return Result;                   // :2902
                        }

                        ErrCode = 920;                       // :2905
                        if (DefMsg.Ident == CM_LOGINNOTICEOK)   // :2906
                        {
                            boLoginNoticeOK = true;          // :2908

                            // {$IF CLIENT_ANTIPLUG = 1}  原文 :2910-2925
                            lock (g_CSRunGatePlug)           // :2911 EnterCriticalSection
                            {
                                if (g_rgpStartContext != null)   // :2913
                                {
                                    try
                                    {
                                        g_rgpStartContext(ContextID);   // :2916
                                    }
                                    catch (Exception E)
                                    {
                                        AddMainLogMsg("TMirClientContext.StartContext Error, Code = " +
                                            IntToStr(ErrCode) + " ," + E.Message, 0);   // :2919
                                    }
                                }
                            }

                            // g_PluginManager.HookStartContext(Self);   // :2927
                        }

                        ErrCode = 93;                        // :2930
                        if (g_boLogClientPacket && DefMsg.Ident < 30000)   // :2931
                        {
                            DoLogClientPacket(DefMsg, GbkBytes(sDataMsg));   // :2933
                        }

                        ErrCode = 95;                        // :2936
                        ProcessClientMessage(DefMsg, GbkBytes(sDataMsg));   // :2937
                    }
                }
            }
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.DoCheckRecvBuffer Error, Code = " + IntToStr(ErrCode) + " ," + E.Message, 0);   // :2943
        }

        return Result;
    }

    /// <summary>原文 :2065-2209 的登录信息落点（两条分支共用的赋值序列）。</summary>
    private void ApplyLoginInfo(string sAccount, string sChrName, string sSessionID, string sClientVersion)
        => ApplyLoginInfo(sAccount, sChrName, sSessionID, sClientVersion, "");

    private void ApplyLoginInfo(string sAccount, string sChrName, string sSessionID, string sClientVersion, string sMachineID)
    {
        this.sAccount = sAccount;
        this.sChrName = sChrName;
        nSessionID = StrToIntDef(sSessionID, 0);
        sVersion = sClientVersion;
        this.sMachineID = sMachineID;
    }

    /// <summary>原文 <c>EncodeString(S)</c> 的 AnsiString 文字形态（GBK 承载）。</summary>
    private static string AnsiStringFromBytes(byte[] bytes) =>
        bytes == null || bytes.Length == 0 ? "" : Gbk.GetString(bytes);

    private static void WriteAllBytesSafe(string fileName, byte[] data)
    {
        try
        {
            string dir = ExtractFilePath(fileName);
            if (!string.IsNullOrEmpty(dir) && !DirectoryExists(dir)) ForceDirectories(dir);
            File.WriteAllBytes(fileName, data ?? Array.Empty<byte>());
        }
        catch
        {
            // 原文 :2521/:2538 都是空 except
        }
    }

    // =================================================================================
    // 原文 :10251-10340  ProcesssSendToClientSendMyMagic / SendAddMagic
    // 接缝：待 Grobal2_Ex.pas 的 packed record `TClientMagic` / `TMagic_C`（含 string[N] 短串）
    //       移植后接入。本车道不定义这两个类型（跨车道类型重名事故已发生 4 次）。
    // 原文语义（已侦察，供接管者 1:1 落地）：
    //   SendMyMagic : zLibDecompressBuffer → 长度 = SizeOf(TClientMagic) * DefMsg.Series 时，
    //                 逐条用 g_MagicCDList.Find(Def.wMagicId) 覆写 dwInterval/dwRealInterval，
    //                 有改动则 zLibCompressBuffer 回写并 AddServerMsg，DefMsg.Param := 压缩后长度。
    //   SendAddMagic: MsgLen = SizeOf(TClientMagic) 时对单条做同样的覆写 + AddServerMsg。
    // =================================================================================
    public bool ProcesssSendToClientSendMyMagic(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen)
    {
        // 接缝：待 Grobal2_Ex.pas 的 TClientMagic / TMagic_C 移植后接入
        return false;
    }

    public bool ProcesssSendToClientSendAddMagic(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen)
    {
        // 接缝：待 Grobal2_Ex.pas 的 TClientMagic / TMagic_C 移植后接入
        return false;
    }

    // =================================================================================
    // 原文 :10342-10451  ProcesssSendToClientBagItems / ProcesssSendToClientAddItem
    // 接缝：待 Grobal2_Ex.pas 的 packed record `TClientItem` / `TStdItem` 移植后接入。
    // 原文语义（已侦察）：MsgLen/DefMsg.Param 与 DefMsg.Series * SizeOf(TClientItem) 比对通过后，
    //   按 ClientItem.s.StdMode in [0,2,3] 把 MakeIndex/StdMode/Shape/AC1/MAC1 填进
    //   HumBagItems 或 HeroBagItems（BagItems 先 Clear，AddItem 不 Clear）。
    // =================================================================================
    public void ProcesssSendToClientBagItems(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        // 接缝：待 Grobal2_Ex.pas 的 TClientItem / TStdItem 移植后接入
    }

    public void ProcesssSendToClientAddItem(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        // 接缝：待 Grobal2_Ex.pas 的 TClientItem / TStdItem 移植后接入
    }

    // =================================================================================
    // 原文 :10453-10481  procedure ProcesssSendToClientDelItem(DefMsg; Msg; MsgLen; IsHuman);
    // =================================================================================
    public void ProcesssSendToClientDelItem(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        if (!boIsOldClient)                                  // :10458
        {
            int MakeIndex = unchecked((int)DefMsg.Recog);    // :10460

            if (IsHuman)                                     // :10462
            {
                HumBagItems.Lock();                          // :10464
                try
                {
                    HumBagItems.Remove(MakeIndex);           // :10466
                }
                finally
                {
                    HumBagItems.UnLock();                    // :10468
                }
            }
            else
            {
                HeroBagItems.Lock();                         // :10473
                try
                {
                    HeroBagItems.Remove(MakeIndex);          // :10475
                }
                finally
                {
                    HeroBagItems.UnLock();                   // :10477
                }
            }
        }
    }

    // =================================================================================
    // 原文 :10483-10520  procedure ProcesssSendToClientDelItems(DefMsg; Msg; MsgLen; IsHuman);
    // =================================================================================
    public void ProcesssSendToClientDelItems(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        string sTemp = DecodeBuffer(Msg, MsgLen);            // :10489
        while (AnsiLen(sTemp) > 0)                           // :10490
        {
            string sName = "", sIndex = "";
            sTemp = HUtil32.GetValidStr3_Ex(sTemp, ref sName, '/');    // :10492
            sTemp = HUtil32.GetValidStr3_Ex(sTemp, ref sIndex, '/');   // :10493
            if (sName != "" && sIndex != "")                 // :10494
            {
                int nMakeIndex = StrToIntDef(sIndex, 0);     // :10496

                if (IsHuman)                                 // :10498
                {
                    HumBagItems.Lock();                      // :10500
                    try
                    {
                        HumBagItems.Remove(nMakeIndex);      // :10502
                    }
                    finally
                    {
                        HumBagItems.UnLock();                // :10504
                    }
                }
                else
                {
                    HeroBagItems.Lock();                     // :10509
                    try
                    {
                        HeroBagItems.Remove(nMakeIndex);     // :10511
                    }
                    finally
                    {
                        HeroBagItems.UnLock();               // :10513
                    }
                }
            }
            else
                break;                                       // :10518
        }
    }

    // =================================================================================
    // 原文 :10522-10543  procedure ProcesssSendToClientDropItem(...);
    // 注意：原文 DropItem **没有** `if not boIsOldClient` 前置判断（与 DelItem 不同）。
    // =================================================================================
    public void ProcesssSendToClientDropItem(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        if (IsHuman)                                         // :10525
        {
            HumBagItems.Lock();                              // :10527
            try
            {
                HumBagItems.Remove(unchecked((int)DefMsg.Recog));   // :10529
            }
            finally
            {
                HumBagItems.UnLock();                        // :10531
            }
        }
        else
        {
            HeroBagItems.Lock();                             // :10536
            try
            {
                HeroBagItems.Remove(unchecked((int)DefMsg.Recog));  // :10538
            }
            finally
            {
                HeroBagItems.UnLock();                       // :10540
            }
        }
    }

    // =================================================================================
    // 原文 :10545-10566  procedure ProcesssSendToClientEatItemOK(...);
    // 同样没有 `if not boIsOldClient`（与 DropItem 同族）。
    // =================================================================================
    public void ProcesssSendToClientEatItemOK(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        if (IsHuman)                                         // :10548
        {
            HumBagItems.Lock();                              // :10550
            try
            {
                HumBagItems.Remove(unchecked((int)DefMsg.Recog));   // :10552
            }
            finally
            {
                HumBagItems.UnLock();                        // :10554
            }
        }
        else
        {
            HeroBagItems.Lock();                             // :10559
            try
            {
                HeroBagItems.Remove(unchecked((int)DefMsg.Recog));  // :10561
            }
            finally
            {
                HeroBagItems.UnLock();                       // :10563
            }
        }
    }

    // =================================================================================
    // 原文 :10568-10606  procedure ProcesssSendToClientMasterBagToHeroBagOK(...);
    // 注意：原文 `Item := BagItem^` 是**值拷贝**（packed record）；C# 的 TBagItem 是引用类型，
    //   故此处显式逐字段拷贝，否则 Remove 之后 Item 会指向已移除对象（语义差异已注释）。
    // =================================================================================
    public void ProcesssSendToClientMasterBagToHeroBagOK(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        bool IsFound = false;                                // :10575
        TBagItem Item = null;                                // 原文 :10573 Item: TBagItem（值类型）
        HumBagItems.Lock();                                  // :10576
        try
        {
            TBagItem BagItem = HumBagItems.Find(unchecked((int)DefMsg.Recog));   // :10578
            if (BagItem != null)                             // :10579
            {
                Item = CopyBagItem(BagItem);                 // :10581 Item := BagItem^（值拷贝）
                IsFound = true;                              // :10582
                HumBagItems.Remove(BagItem.MakeIndex);       // :10583
            }
        }
        finally
        {
            HumBagItems.UnLock();                            // :10586
        }

        if (IsFound)                                         // :10589
        {
            HeroBagItems.Lock();                             // :10591
            try
            {
                TBagItem BagItem = HeroBagItems.Add(Item.MakeIndex);   // :10593
                if (BagItem != null)                         // :10594
                {
                    BagItem.StdMode = Item.StdMode;          // :10596
                    BagItem.Shape = Item.Shape;              // :10597
                    // BagItem.Name := Item.Name;            // :10598 原文如此（已注释）
                    BagItem.AC1 = Item.AC1;                  // :10599
                    BagItem.MAC1 = Item.MAC1;                // :10600
                }
            }
            finally
            {
                HeroBagItems.UnLock();                       // :10602
            }
        }
    }

    // =================================================================================
    // 原文 :10608-10646  procedure ProcesssSendToClientHeroBagToMasterBagOK(...);
    // =================================================================================
    public void ProcesssSendToClientHeroBagToMasterBagOK(in TDefaultMessage DefMsg, byte[] Msg, int MsgLen, bool IsHuman)
    {
        bool IsFound = false;                                // :10615
        TBagItem Item = null;
        HeroBagItems.Lock();                                 // :10616
        try
        {
            TBagItem BagItem = HeroBagItems.Find(unchecked((int)DefMsg.Recog));   // :10618
            if (BagItem != null)                             // :10619
            {
                Item = CopyBagItem(BagItem);                 // :10621 Item := BagItem^
                IsFound = true;                              // :10622
                HeroBagItems.Remove(BagItem.MakeIndex);      // :10623
            }
        }
        finally
        {
            HeroBagItems.UnLock();                           // :10626
        }

        if (IsFound)                                         // :10629
        {
            HumBagItems.Lock();                              // :10631
            try
            {
                TBagItem BagItem = HumBagItems.Add(Item.MakeIndex);   // :10633
                if (BagItem != null)                         // :10634
                {
                    BagItem.StdMode = Item.StdMode;          // :10636
                    BagItem.Shape = Item.Shape;              // :10637
                    // BagItem.Name := Item.Name;            // :10638 原文如此（已注释）
                    BagItem.AC1 = Item.AC1;                  // :10639
                    BagItem.MAC1 = Item.MAC1;                // :10640
                }
            }
            finally
            {
                HumBagItems.UnLock();                        // :10642
            }
        }
    }

    private static TBagItem CopyBagItem(TBagItem src) => new TBagItem
    {
        MakeIndex = src.MakeIndex,
        StdMode = src.StdMode,
        Shape = src.Shape,
        Reserved = src.Reserved,
        AC1 = src.AC1,
        MAC1 = src.MAC1,
    };

    // =================================================================================
    // 原文 :10648-10797  procedure TMirClientContext.DoLogClientPacket(DefMsg; Msg: string);
    // Msg 语义是**原始二进制**（AnsiString）→ C# byte[]。
    // =================================================================================
    public void DoLogClientPacket(in TDefaultMessage DefMsg, byte[] Msg)
    {
        bool boLog = true;                                   // :10655

        g_LogClientPacketUser.Lock();                        // :10657
        try
        {
            if (g_LogClientPacketUser.Count > 0)             // :10659
            {
                boLog = g_LogClientPacketUser.IndexOf(this.sChrName) >= 0;   // :10661
            }
        }
        finally
        {
            g_LogClientPacketUser.UnLock();                  // :10664
        }

        if (boLog)                                           // :10667
        {
            if (DefMsg.Ident == 3010 ||                      // :10669 转身(方向改变)
                DefMsg.Ident == 3011 ||                      // :10670 走
                DefMsg.Ident == 3013)                        // :10671 跑
            {
                boLog = (g_nLogClientPacketType & CPT_MOVE) == CPT_MOVE;      // :10673
            }

            else if (DefMsg.Ident == 3014 ||                 // :10676 普通物理近身攻击
                DefMsg.Ident == 3015 ||                      // :10677 跳起来打的动作
                DefMsg.Ident == 3016 ||                      // :10678 强攻
                DefMsg.Ident == 3018 ||                      // :10679 攻杀
                DefMsg.Ident == 3019 ||                      // :10680 刺杀
                DefMsg.Ident == 3024 ||                      // :10681 半月
                DefMsg.Ident == 3025 ||                      // :10682 烈火
                DefMsg.Ident == 3036 ||                      // :10683 抱月刀 双龙斩
                DefMsg.Ident == 3037 ||                      // :10684 龙影剑法
                DefMsg.Ident == 3043 ||                      // :10685 雷霆剑法
                DefMsg.Ident == 3056 ||                      // :10686 逐日剑法
                DefMsg.Ident == 3066 ||                      // :10687 开天斩
                DefMsg.Ident == 3166 ||                      // :10688
                DefMsg.Ident == 3101 ||                      // :10689 三绝杀
                DefMsg.Ident == 3102 ||                      // :10690 断岳斩
                DefMsg.Ident == 3103 ||                      // :10691 横扫千军
                (DefMsg.Ident >= 5127 && DefMsg.Ident <= 5226))   // :10692 自定义技能
            {
                boLog = (g_nLogClientPacketType & CPT_HIT) == CPT_HIT;        // :10694
            }

            else if (DefMsg.Ident == 3017)                   // :10697 施魔法
            {
                boLog = (g_nLogClientPacketType & CPT_SPELL) == CPT_SPELL;    // :10699
            }

            else if (DefMsg.Ident == 81 ||                   // :10702 查询包裹
                DefMsg.Ident == 97 ||                        // :10703 排行榜
                DefMsg.Ident == 98 ||                        // :10704 自己排行榜
                DefMsg.Ident == 109 ||                       // :10705 搜索传奇店铺
                DefMsg.Ident == 110 ||                       // :10706 传奇店铺
                DefMsg.Ident == 111 ||                       // :10707 搜索指定用户店铺物品
                DefMsg.Ident == 112 ||                       // :10708 搜索指定用户店铺物品
                DefMsg.Ident == 113 ||                       // :10709 搜索用户店铺物品
                DefMsg.Ident == 114 ||                       // :10710 搜索指定物品
                DefMsg.Ident == 115 ||                       // :10711 搜索我的店铺正在物品
                DefMsg.Ident == 116 ||                       // :10712 搜索我的店铺已经物品
                DefMsg.Ident == 117 ||                       // :10713 搜索我的店铺仓库物品
                DefMsg.Ident == 118 ||                       // :10714 搜索我的店铺物品
                DefMsg.Ident == 122 ||                       // :10715 查看选中物品信息
                DefMsg.Ident == 5125 ||                      // :10716 请求可视仓库换页
                DefMsg.Ident == 5126 ||                      // :10717 刷新英雄包裹
                DefMsg.Ident == CM_CLIENTDATAFILE ||         // :10719 请求相关资源
                DefMsg.Ident == CM_PLUGINCONFIG)             // :10720 向服务器发送内挂配置信息
            {
                boLog = (g_nLogClientPacketType & CPT_QUERY) == CPT_QUERY;    // :10722
            }

            else if (DefMsg.Ident == 1020 ||                 // :10725 新建组队
                DefMsg.Ident == 1021 ||                      // :10726 组内添人
                DefMsg.Ident == 1022)                        // :10727 组内删人
            {
                boLog = (g_nLogClientPacketType & CPT_TEAM) == CPT_TEAM;      // :10729
            }

            else if ((DefMsg.Ident >= 1036 && DefMsg.Ident <= 1041) ||        // :10732
                (DefMsg.Ident >= 5227 && DefMsg.Ident <= 5248))               // :10733
            {
                boLog = (g_nLogClientPacketType & CPT_GUILD) == CPT_GUILD;    // :10735
            }

            else if (DefMsg.Ident == 95 ||                   // :10738 商铺相关
                DefMsg.Ident == 9002 ||                      // :10739
                DefMsg.Ident == 9006 ||                      // :10740
                DefMsg.Ident == 163 ||                       // :10741 开始摆摊
                DefMsg.Ident == 164 ||                       // :10742 停止摆摊
                DefMsg.Ident == 165 ||                       // :10743 购买摆摊物品
                DefMsg.Ident == 166 ||                       // :10744 增加摆摊物品
                DefMsg.Ident == 167 ||                       // :10745 删除摆摊物品
                DefMsg.Ident == 168)                         // :10746 关闭购买摆摊物品窗口
            {
                boLog = (g_nLogClientPacketType & CPT_SHOP) == CPT_SHOP;      // :10748
            }

            if (boLog)                                       // :10751
            {
                if (AnsiLen(FLogPakcetFileName) > 0)         // :10753
                {
                    FWriteLogLocker.Lock();                  // :10755
                    try
                    {
                        string sTemp;
                        if (DefMsg.Ident == CM_LOGINNOTICEOK)   // :10768
                        {
                            sTemp = sLineBreak +                  // :10770
                                "----------------------------------------------------------------------------" +
                                "----------------------------------------------------------------------------" +
                                "----------------------------------------------------------------------------" + sLineBreak;   // :10771-10773
                        }
                        else if (DefMsg.Ident == CM_SAY || DefMsg.Ident == CM_MERCHANTDLGSELECT)   // :10775
                        {
                            sTemp = Format("%s%s%-30s%s%-56s%s%s",                    // :10777
                                FormatDateTime("mm-dd hh:nn:ss", Now()), "\t", this.sChrName, "\t",
                                Format("%d, %d, %d, %d, %d", DefMsg.Ident, DefMsg.Recog, DefMsg.Param,
                                    DefMsg.Tag, DefMsg.Series), "\t", DecodeBuffer(Msg, Msg == null ? 0 : Msg.Length));
                        }
                        else
                        {
                            sTemp = Format("%s%s%-30s%s%-56s%s%s",                    // :10782
                                FormatDateTime("mm-dd hh:nn:ss", Now()), "\t", this.sChrName, "\t",
                                Format("%d, %d, %d, %d, %d", DefMsg.Ident, DefMsg.Recog, DefMsg.Param,
                                    DefMsg.Tag, DefMsg.Series), "\t", AnsiStringFromBytes(Msg));
                        }

                        try
                        {
                            // 原文 :10759-10766 的 Rewrite/Append → 追加一行
                            AppendTextLine(FLogPakcetFileName, sTemp);   // :10787 Writeln
                        }
                        catch
                        {
                            // :10789 原文为空 except
                        }
                    }
                    finally
                    {
                        FWriteLogLocker.UnLock();            // :10792
                    }
                }
            }
        }
    }

    // =================================================================================
    // 原文 :10799-10890  function TMirClientContext.GenerateVerifyCode(): Boolean;
    // 图形渲染走 VerifyCodeUtils.cs（既有真实现），zLib 压缩走 EDcode。
    // 原文缺陷（保留）：:10815 `Chr(65 + Random(27))` —— Random(27) ∈ [0,26]，65+26 = '[' 越出 A..Z。
    // =================================================================================
    public bool GenerateVerifyCode()
    {
        bool Result = false;                                 // :10809

        Randomize();                                         // :10811
        int Num = Random(3);                                 // :10812
        string sTempVerifyCode;
        if (Num == 0)                                        // :10813
        {
            sVerifyCode = ((char)(65 + Random(27))).ToString() + ((char)(65 + Random(27))).ToString() +
                ((char)(65 + Random(27))).ToString() + ((char)(65 + Random(27))).ToString() +
                ((char)(65 + Random(27))).ToString() + ((char)(65 + Random(27))).ToString();   // :10815-10816

            sTempVerifyCode = sVerifyCode;                   // :10818
        }
        else if (Num == 1)                                   // :10820
        {
            int Num1 = 10 + Random(40);                      // :10822
            int Num2 = 10 + Random(40);                      // :10823
            sVerifyCode = IntToStr(Num1 + Num2);             // :10824

            sTempVerifyCode = IntToStr(Num1) + "+" + IntToStr(Num2) + "=";   // :10826
        }
        else
        {
            int Num1 = 10 + Random(90);                      // :10830
            int Num2 = 10 + Random(90);                      // :10831

            if (Num1 >= Num2)                                // :10833
            {
                sVerifyCode = IntToStr(Num1 - Num2);         // :10835
                sTempVerifyCode = IntToStr(Num1) + "-" + IntToStr(Num2) + "=";   // :10836
            }
            else
            {
                sVerifyCode = IntToStr(Num2 - Num1);         // :10840
                sTempVerifyCode = IntToStr(Num2) + "-" + IntToStr(Num1) + "=";   // :10841
            }
        }

        int ErrCode = 1;                                     // :10845
        try
        {
            using (System.Drawing.Bitmap Bitmap = new System.Drawing.Bitmap(170, 70))   // :10847 TBitmap.Create
            {
                ErrCode = 2;                                 // :10849
                ErrCode = 3;                                 // :10852
                // :10853-10854 Bitmap.Width := 170; Bitmap.Height := 70;（GDI+ 由构造参数给定）
                using (System.Drawing.Font font = new System.Drawing.Font("Arial", 32))   // :10855 Bitmap.Canvas.Font.Size := 32
                {
                    // :10856 Bitmap.PixelFormat := pf24bit —— GDI+ 下由 Bitmap 构造参数决定，无需另设

                    ErrCode = 4;                             // :10858

                    VerifyCodeUtils.MakeVerifyCode(sTempVerifyCode, Bitmap, font, -(5 + Random(5)), false);   // :10860

                    ErrCode = 5;                             // :10862
                    using (MemoryStream MS = new MemoryStream())   // :10863 TMemoryStream.Create
                    {
                        ErrCode = 6;                         // :10865
                        Bitmap.Save(MS, System.Drawing.Imaging.ImageFormat.Png);   // :10866 Bitmap.SaveToStream(MS)

                        ErrCode = 7;                         // :10868
                        byte[] sSendText = EDcode.zLibCompressBuffer(MS.ToArray(), (int)MS.Length);   // :10869
                        TDefaultMessage DefMsg = MakeDefaultMsg(SM_RUNGATE_VERIFYCODE,
                            sSendText.Length, (ushort)(MS.Length & 0xFFFF), (ushort)((MS.Length >> 16) & 0xFFFF), 0);   // :10870 LoWord/HiWord
                        byte[] enc = EncodeRunGateMsg(DefMsg, sSendText, sSendText.Length);   // :10871

                        ErrCode = 8;                         // :10873
                        PostSendTextBytes(enc);              // :10874

                        Result = true;                       // :10876
                    }
                }
            }
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.GenerateVerifyCode Error, Code = " + IntToStr(ErrCode) + "," + E.Message, 0);   // :10888
        }

        return Result;
    }

    // =================================================================================
    // 原文 :10899-10948  procedure TMirClientContext.SendAntiPlugStreamInfo;
    // =================================================================================
    public void SendAntiPlugStreamInfo()
    {
        if (g_ClientAntiPlugDllSize > 0)                     // :10905
        {
            int Len = g_ClientAntiPlugDllString.Length;      // :10907
            ushort W1 = (ushort)(Len & 0xFFFF);              // :10908 LoWord
            ushort W2 = (ushort)((Len >> 16) & 0xFFFF);      // :10909 HiWord

            dwClientAntiPlugVersion = g_ClientAntiPlugVersion;   // :10911

            dwSendLoadAntiPlugTick = MyGetTickCount();       // :10913
            boRecvLoadAntiPlug = false;                      // :10914
            dwRecvLoadAntiPlugTick = MyGetTickCount();       // :10915

            boWaitLoadAntiPlug = false;                      // :10917
            dwWaitLoadAntiPlugTick = MyGetTickCount();       // :10918

            Randomize();                                     // :10920
            wSendLoadAntiPlugCode = (ushort)Random(ushort.MaxValue);   // :10921 Random(High(WORD))

            TDefaultMessage DefMsg = MakeDefaultMsg(SM_ANTIPLUGSTREAM_IOCP2, g_ClientAntiPlugDllSize, W1, W2,
                wSendLoadAntiPlugCode);                      // :10923
            AddServerMsg(DefMsg, null, 0);                   // :10924

            boSendLoadAntiPlug = true;                       // :10926
            nSendLoadAntiPlugIndex = 0;                      // :10927
            boSendLoadAntiPlugFinished = false;              // :10928

            // 应插件要求 只要是重新发模块出去就要抹掉数据 2019-12-12 15:52:33
            ContextDataLocker.Lock();                        // :10933
            try
            {
                if (pContextData != null && nContextDataLen > 0)   // :10935
                {
                    Array.Clear(pContextData, 0, (int)nContextDataLen);   // :10937
                }
            }
            finally
            {
                ContextDataLocker.UnLock();                  // :10940
            }

            // {$IF LOG_PLUG_DATA = 1}  :10945 AddMainLogMsg('[准备发送插件数据]: ' + ...)
        }
    }

    // =================================================================================
    // 原文 :10950-10989  procedure TMirClientContext.SendAntiPlugStreamUnload(IsWaitLoad: Boolean);
    // =================================================================================
    public void SendAntiPlugStreamUnload(bool IsWaitLoad)
    {
        if (g_ClientAntiPlugDllSize > 0)                     // :10954
        {
            dwSendLoadAntiPlugTick = MyGetTickCount();       // :10956
            boRecvLoadAntiPlug = false;                      // :10957
            dwRecvLoadAntiPlugTick = MyGetTickCount();       // :10958

            TDefaultMessage DefMsg = MakeDefaultMsg(SM_IOCP_ANTIPLUG_UNLOAD_IOCP2, 0, 0, 0, 0);   // :10960
            AddServerMsg(DefMsg, null, 0);                   // :10961

            boSendLoadAntiPlug = false;                      // :10963
            boSendLoadAntiPlugFinished = false;              // :10964

            if (IsWaitLoad)                                  // :10966
            {
                boWaitLoadAntiPlug = true;                   // :10968
                dwWaitLoadAntiPlugTick = MyGetTickCount();   // :10969
            }

            // 应插件要求 只要是重新发模块出去就要抹掉数据 2019-12-12 15:52:33
            ContextDataLocker.Lock();                        // :10974
            try
            {
                if (pContextData != null && nContextDataLen > 0)   // :10976
                {
                    Array.Clear(pContextData, 0, (int)nContextDataLen);   // :10978
                }
            }
            finally
            {
                ContextDataLocker.UnLock();                  // :10981
            }

            // {$IF LOG_PLUG_DATA = 1}  :10986 AddMainLogMsg('[通知客户端卸载插件]: ' + ...)
        }
    }

    // =================================================================================
    // 原文 :10991-11021  procedure TMirClientContext.SendAntiPlugStreamLoadCache;
    // =================================================================================
    public void SendAntiPlugStreamLoadCache()
    {
        if (g_ClientAntiPlugDllSize > 0)                     // :10995
        {
            dwClientAntiPlugVersion = g_ClientAntiPlugVersion;   // :10997

            dwSendLoadAntiPlugTick = MyGetTickCount();       // :10999
            boRecvLoadAntiPlug = false;                      // :11000
            dwRecvLoadAntiPlugTick = MyGetTickCount();       // :11001

            boWaitLoadAntiPlug = false;                      // :11003
            dwWaitLoadAntiPlugTick = MyGetTickCount();       // :11004

            Randomize();                                     // :11006
            wSendLoadAntiPlugCode = (ushort)Random(ushort.MaxValue);   // :11007

            TDefaultMessage DefMsg = MakeDefaultMsg(SM_IOCP_ANTIPLUGSTREAM_CACHE_IOCP2,
                g_ClientAntiPlugDllSize, 0, 0, wSendLoadAntiPlugCode);   // :11009
            AddServerMsg(DefMsg, null, 0);                   // :11010

            boSendLoadAntiPlug = true;                       // :11012
            nSendLoadAntiPlugIndex = 0;                      // :11013
            boSendLoadAntiPlugFinished = true;               // :11014

            if (g_boAntiplugAllLog)                          // :11016
            {
                AddMainLogMsg("[发送加载插件缓存]: " + RemoteAddr + "; " + sChrName, 9);   // :11018
            }
        }
    }

    // =================================================================================
    // 原文 :11024-11064  function TMirClientContext.SendAntiPlugStream: Boolean;
    // =================================================================================
    public bool SendAntiPlugStream()
    {
        bool Result = false;                                 // :11031
        // 增加(g_RunGatePlugDllHandle <> 0)网关插件加载才下发模块 2019-12-16 19:53:52
        if (g_ClientAntiPlugDllSize > 0 && g_RunGatePlugDllHandle != 0 &&
            nSendLoadAntiPlugIndex < g_ClientAntiPlugDllBlockCount)   // :11033
        {
            if (nSendLoadAntiPlugIndex == g_ClientAntiPlugDllBlockCount - 1 && !boFirstClientQueryBagItems)
                return Result;                               // :11035 Exit

            byte[] S = SliceAt(g_ClientAntiPlugDllString,
                nSendLoadAntiPlugIndex * g_ClientAntiPlugDllBlockSize, g_ClientAntiPlugDllBlockSize);   // :11037 Copy(...)
            int Len = S.Length;                              // :11038
            ushort W1 = (ushort)(Len & 0xFFFF);              // :11039 LoWord
            ushort W2 = (ushort)((Len >> 16) & 0xFFFF);      // :11040 HiWord

            dwSendLoadAntiPlugTick = MyGetTickCount();       // :11042

            TDefaultMessage DefMsg = MakeDefaultMsg(SM_CONTINUEANTIPLUGSTREAM_IOCP2,
                nSendLoadAntiPlugIndex + 1, W1, W2, (ushort)g_ClientAntiPlugDllBlockCount);   // :11044
            AddServerMsg(DefMsg, S, (uint)S.Length);         // :11045

            nSendLoadAntiPlugIndex++;                        // :11047 Inc

            // {$IF LOG_PLUG_DATA = 1}  :11050 AddMainLogMsg('[发送插件数据...]: ' + ...)

            Result = true;                                   // :11053
            if (nSendLoadAntiPlugIndex >= g_ClientAntiPlugDllBlockCount)   // :11054
            {
                boSendLoadAntiPlugFinished = true;           // :11056

                if (g_boAntiplugAllLog)                      // :11058
                {
                    AddMainLogMsg("[插件数据发送完成]: " + RemoteAddr + "; " + sChrName, 9);   // :11060
                }
            }
        }
        return Result;
    }

    private static byte[] SliceAt(byte[] src, int offset, int len)
    {
        if (src == null || offset >= src.Length || len <= 0) return Array.Empty<byte>();
        int n = Math.Min(len, src.Length - offset);
        byte[] r = new byte[n];
        Buffer.BlockCopy(src, offset, r, 0, n);
        return r;
    }

    // =================================================================================
    // 原文 :11069-11120  {$IF LOG_PLUG_DATA = 1} procedure TMirClientContext.LogPluginData(...);
    // =================================================================================
    public void LogPluginData(bool IsSplite, bool IsSend, ushort wIdent, long nRecog,
        ushort wParam, ushort wTag, ushort wSeries, string DataDesc)
    {
        FWriteLogLocker.Lock();                              // :11076
        try
        {
            string sPath = ExtractFilePath(ParamStr0) + "plugdata\\" + sLogFileChrName;   // :11078
            string LogFileName = sPath + "\\" + FormatDateTime("yyyy-mm-dd hh", Now()) + ".txt";   // :11079

            if (!DirectoryExists(sPath))                     // :11081
                ForceDirectories(sPath);                     // :11082

            bool boWriteLog = true;                          // :11084
            try
            {
                // 原文 :11086-11095 是 AssignFile + Rewrite/Append；托管侧由 AppendTextLine 接缝承担。
                // FileExists 判定保留（原文用它选择 Rewrite 还是 Append）。
                _ = FileExists(LogFileName);
            }
            catch
            {
                boWriteLog = false;                          // :11097
                AddMainLogMsg("保存插件封包日志信息出错！", 0);   // :11098
            }

            if (boWriteLog)                                  // :11101
            {
                string sData;
                if (IsSplite)                                // :11103
                    sData = sLineBreak;                      // :11104
                else
                {
                    if (IsSend)                              // :11107
                        sData = Format("%-12s [发, %s]; %x, %d, %d, %d, %d",
                            DateTime.Now.ToString("HH:mm:ss"), DataDesc, wIdent, nRecog, wParam, wTag, wSeries);   // :11108
                    else
                        sData = Format("%-12s [收, %s]; %x, %d, %d, %d, %d",
                            DateTime.Now.ToString("HH:mm:ss"), DataDesc, wIdent, nRecog, wParam, wTag, wSeries);   // :11110
                }

                AppendTextLine(LogFileName, sData);          // :11113 Writeln(LogFile, sData)
                // :11115 CloseFile(LogFile)
            }
        }
        finally
        {
            FWriteLogLocker.UnLock();                        // :11118
        }
    }
}

// -------------------------------------------------------------------------------------
// TSafeMemoryStream 的落盘助手（原文 TMemoryStream.SaveToFile）。
// -------------------------------------------------------------------------------------
internal static class TSafeMemoryStreamExtensions
{
    public static void SaveToFileSafe(this TSafeMemoryStream stream, string fileName)
    {
        try
        {
            string dir = GateShareSeam.ExtractFilePath(fileName);
            if (!string.IsNullOrEmpty(dir) && !GateShareSeam.DirectoryExists(dir))
                GateShareSeam.ForceDirectories(dir);
            System.IO.File.WriteAllBytes(fileName, stream.ToArray());
        }
        catch
        {
            // 原文 :2428/:2521/:2538/:2594 的失败都被 except 吞掉
        }
    }
}
