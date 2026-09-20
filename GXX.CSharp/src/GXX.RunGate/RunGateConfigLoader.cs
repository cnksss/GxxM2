// =====================================================================================
// 源单元：Source\RunGate\uFrmMain.pas（Delphi 7，GBK）—— **S1：配置加载/校验/写回**
//   实测 LF = 4217 行。本文件覆盖（行号 = 物理 LF 行号；`Get-Content`/`Select-String` 会漂移 +17，勿混用）：
//     :1076-1507 `procedure TFrmMain.LoadConfig(Ports: TList);` 的**解析内核**
//                 （:1509-1855 是纯 UI 尾巴，本片不含）
//     :2760-2806 `btnSettingOKClick` 的**输入校验内核**（:2766-2806）
//     :2808-2935 `btnSettingOKClick` 的**全局赋值 + 48 个 INI 写回**（:2808-2869 赋值、:2872-2932 写回）
//
// ── DFM 常量（脚本抽取 + 回读，非手工转录）────────────────────────────────────────────
//   抽取命令（GBK 读入后按 `object <name>:` 定位，取 MinValue/MaxValue 与 Items.Strings 条数）：
//     $dfm=[IO.File]::ReadAllText('Source\RunGate\uFrmMain.dfm',[Text.Encoding]::GetEncoding(936))
//   回读结果（`RunGateConfigBounds` 的默认值即由此生成）：
//     seCheckServerTimeOutTime              MinValue=60    MaxValue=600      (dfm:839)
//     seClientSendBlockSize                 MinValue=1     MaxValue=8        (dfm:855)
//     sePreAllocatedCount                   MinValue=256   MaxValue=102400   (dfm:887)
//     seRecvAntiPlugHeartbeatTimeOutTime    MinValue=15    MaxValue=180      (dfm:970)
//     seAntiPlugStreamSendSpeed             MinValue=1     MaxValue=20       (dfm:982)
//     cbbShowLogLevel.Items.Count           = 11                             (dfm:715)
//     cbbAntiPlugStreamSendBlockSize.Items.Count = 4                         (dfm:997)
//   断言见 `RunGateConfigLoaderTests.DfmBounds_MatchScriptExtraction`。
//
// ── 原文缺陷 / 易错点登记（照抄语义 + 差异断言）───────────────────────────────────────
//   C1. [:1112-1113] `if g_btShowLogLevel > cbbShowLogLevel.Items.Count then := Items.Count`
//       —— 下拉框有 **11** 项（合法 ItemIndex 0..10），但钳位目标是 **11** → 等于 11 时
//       随后 `cbbShowLogLevel.ItemIndex := g_btShowLogLevel`（原 :1521）会**越界**。
//       同时 `<= 0 → 0` 会把负值抹成 0。本实现照抄钳位（含把 11 留给 UI 尾巴踩坑）。
//   C2. [:1195-1199] `if ReadInteger('AttackTick',-1) > 0 then g_dwAttackTick := ReadInteger('AttackTick', g_dwAttackTick)`
//       —— **同一个键被解析两次**（`AttackCount` 同型）。若两次之间文件被改动，两次读到的值会不一致。
//       本实现照抄"读两次"的顺序（不是语义等价地读一次再判断）。
//   C3. [:1205] `if g_nMaxClientPacketSize > 512 then g_nMaxClientPacketSize := 128;`
//       —— 越界回落目标是 **128**，**不是**原文自己的内联初值 512（GateShare.pas:1219）。静默、无日志。
//   C4. [:1202 / :1404] `g_BlockMethod := TBlockIPMethod(ReadInteger(...))` 与
//       `g_CheckClientFailBlockMethod := TBlockIPMethod(...)` —— **不做范围校验**（对比 :1234/:1242/:1331 都做了）。
//       越界 INI 值会产生非法枚举值。托管侧用 `unchecked((TBlockIPMethod)v)` 照抄。
//   C5. [:1448-1449] `g_wAntiPlugUpdateCheckInterval := ReadInteger(..., g_wAntiPlugUpdateCheckInterval); if < 1 then := 1;`
//       —— 只钳下界，无上界。
//   C6. [:1356-1367] `String/DisableSayMsg` 与 `String/DisableSayMsgBegin` 是**"缺键则写入默认"**
//       （`ReadString(...,'')` 为空 → `WriteString(..., 全局默认)`），而不是"从 INI 读默认"。即首次运行会**补键**。
//   C7. [:1419-1420 / :1427-1428] `FYDownDenyIPUrl` / `FYDownPassIPUrl` 用 `if ValueExists then ReadString(...,'')`
//       —— 键存在但值为空串时，会把全局**清成空串**；键不存在时保留全局默认。`FYDownDenyMACUrl`（:1435）**没有**这层保护。
//   C8. [:1281/:1284/...] `g_Config` 的 `dwUserShop_*`/`dwTakeOn_*`/`dwDealTry_*`/`dwBrutal_*` 等**没有**范围校验。
//   C9. [:1312-1315] `if g_Config.dwSpeedValue >= g_Config.dwCollectCount then dwSpeedValue := dwCollectCount - 1;`
//       —— 依赖"CollectCount 已被钳到 >= 5"，故不会出现 -1；但若 CollectCount 的 INI 值越界被拒
//       （保持默认 15）而 SpeedValue 合法（2..18）→ 仍可能满足 `>=` 并回落到 14。照抄。
//   C10.[:1485/:1497] `StrValue := IntToStr(I + 1)` → 键名是 `'1NormalHP'`（**前缀从 1 起、无分隔符**，
//       与前任报告 §5.24-15 的 `uFrmItemEatCD.pas:188` 同型）。照抄，不"顺手统一"。
//   C11.[:2794-2799] `IntSvrPort` 允许 **0**（`< 0` 才报错），而 `IntGatePort` 同样；`IntDBPort`
//       （:2771 读入，:2812 赋值）**完全没有校验**。
//   C12.[:2809] `g_wdGatePort := IntGatePort` —— `IntGatePort: Integer` 赋给 `Word` 全局，$R- 下按低 16 位截断。
//   C13.[:2894] 写回 `AntiplugAllLog`，但**读取侧**该行在 :1145 被注释掉 → **写入型设置**，重启后不生效（子代理 D18）。
//   C15.[:1109-1113] `g_btShowLogLevel: Byte`（GateShare.pas:1172）—— `ReadInteger` 的结果**先截断成 Byte**
//       再比较，故 `if g_btShowLogLevel <= 0` **不可达负值**（Byte 无负值）→ 实际等价于 `= 0`；
//       INI 里的 `-5` 会变成 **251**，随后命中上界钳位得 **11**。照抄（不"顺手修正"成 int 比较）。
//   C14.[:2887-2888] 写回 `ClientSendBlockSize`/`MaxPreallocatedMemorySize` 用的是**控件值**（`seClientSendBlockSize.Value`
//       /`sePreAllocatedCount.Value`），而不是 :1134/:1141 落到 `MAX_*` 的值 —— 两者在"未通过范围校验"时会分叉。
// =====================================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.RunGate;

/// <summary>`uFrmMain.dfm` 里参与 `LoadConfig` 钳位判定的控件边界（脚本抽取，见文件头）。
/// 测试可整体替换以驱动各分支。</summary>
public sealed class RunGateConfigBounds
{
    /// <summary>`seCheckServerTimeOutTime`（dfm:839）。</summary>
    public int CheckServerTimeOutTimeMin = 60;
    public int CheckServerTimeOutTimeMax = 600;

    /// <summary>`seClientSendBlockSize`（dfm:855）。</summary>
    public int ClientSendBlockSizeMin = 1;
    public int ClientSendBlockSizeMax = 8;

    /// <summary>`sePreAllocatedCount`（dfm:887）。</summary>
    public int PreAllocatedCountMin = 256;
    public int PreAllocatedCountMax = 102400;

    /// <summary>`seRecvAntiPlugHeartbeatTimeOutTime`（dfm:970）。</summary>
    public int RecvAntiPlugHeartbeatTimeOutTimeMin = 15;
    public int RecvAntiPlugHeartbeatTimeOutTimeMax = 180;

    /// <summary>`seAntiPlugStreamSendSpeed`（dfm:982）。</summary>
    public int AntiPlugStreamSendSpeedMin = 1;
    public int AntiPlugStreamSendSpeedMax = 20;

    /// <summary>`cbbShowLogLevel.Items.Count`（dfm:715）= 11。</summary>
    public int ShowLogLevelItemCount = 11;

    /// <summary>`cbbAntiPlugStreamSendBlockSize.Items.Count`（dfm:997）= 4。</summary>
    public int AntiPlugStreamSendBlockSizeItemCount = 4;

    /// <summary>缺省边界（= 脚本抽取结果）。</summary>
    public static RunGateConfigBounds FromDfm() => new RunGateConfigBounds();
}

/// <summary>`uFrmMain.pas` 的配置加载/校验/写回（S1）。</summary>
public static class RunGateConfigLoader
{
    /// <summary>`AddMainLogMsg` 接缝（原文 :1115 `AddMainLogMsg('正在加载配置信息...', 3)`）。
    /// 默认接到 `GateShareLists.AddMainLogMsg`（本轮产物）。</summary>
    public static Action<string, int> LogSink = (msg, level) => GateShareLists.AddMainLogMsg(msg, level);

    // ==================================================================================
    // :1076-1507 LoadConfig 解析内核
    // ==================================================================================

    /// <summary>原文 :1076-1507 `TFrmMain.LoadConfig` 的解析内核（不含 :1509-1855 的 UI 尾巴）。
    /// `ports` 对应原文的 `Ports: TList`（:1408-1413 追加受校验的端口号）。</summary>
    public static void LoadConfig(TCustomIniFileEx IniFile, IList<int> ports, RunGateConfigBounds bounds = null)
    {
        if (IniFile == null) throw new ArgumentNullException(nameof(IniFile));
        bounds ??= RunGateConfigBounds.FromDfm();
        ports ??= new List<int>();

        string GateClass = RunGateConst.GateClass;                                   // 原 :448 `GateClass = 'GameGate'`

        // 原 :1097 `g_DefaultConfig := g_Config;` —— 用**当前**全局量快照当"默认值"
        //   ★ 子代理 D6：二次调用会把上一次的运行值当默认值。照抄。
        FormGlobals.g_DefaultConfig.CopyFrom(FormGlobals.g_Config);

        // ---- :1101-1107 身份/端口 ----
        GateShareGlobals.g_sTitleName = IniFile.ReadString(GateClass, "Title", GateShareGlobals.g_sTitleName);
        GateShareGlobals.g_sServerAddr = IniFile.ReadString(GateClass, "Server1", GateShareGlobals.g_sServerAddr);
        GateShareGlobals.g_wdServerPort = unchecked((ushort)IniFile.ReadInteger(GateClass, "ServerPort", GateShareGlobals.g_wdServerPort));
        GateShareGlobals.g_sGateAddr = IniFile.ReadString(GateClass, "GateAddr", GateShareGlobals.g_sGateAddr);
        GateShareGlobals.g_wdGatePort = unchecked((ushort)IniFile.ReadInteger(GateClass, "GatePort", GateShareGlobals.g_wdGatePort));
        GateShareGlobals.g_wdDBPort = unchecked((ushort)IniFile.ReadInteger(GateClass, "DBPort", GateShareGlobals.g_wdDBPort));

        // ---- :1109-1113 ShowLogLevel（★ C1：上界钳到 Items.Count=11，合法 ItemIndex 只到 10） ----
        GateShareGlobals.g_btShowLogLevel = unchecked((byte)IniFile.ReadInteger(GateClass, "ShowLogLevel", GateShareGlobals.g_btShowLogLevel));
        if (GateShareGlobals.g_btShowLogLevel <= 0)
            GateShareGlobals.g_btShowLogLevel = 0;
        else if (GateShareGlobals.g_btShowLogLevel > bounds.ShowLogLevelItemCount)
            GateShareGlobals.g_btShowLogLevel = unchecked((byte)bounds.ShowLogLevelItemCount);

        LogSink("正在加载配置信息...", 3);                                            // 原 :1115

        GateShareGlobals.g_boMinimize = IniFile.ReadBool(GateClass, "Minimize", GateShareGlobals.g_boMinimize ? (byte)1 : (byte)0) != 0;   // 原 :1117

        // ---- :1120-1122 网关密码 ----
        // 原文这里写的是 `g_boCheckClientPassWord`（小写 p），而 GateShare.pas:1201 声明名是
        // `g_boCheckClientPassword`（大写 P）—— Delphi 大小写不敏感，托管侧按**声明名**保留（子代理 D2）。
        GateShareSeam.g_boCheckClientPassword = IniFile.ReadBool(GateClass, "CheckClientPassWord", GateShareSeam.g_boCheckClientPassword ? (byte)1 : (byte)0) != 0;
        GateShareSeam.g_sClientPassWord = IniFile.ReadString(GateClass, "ClientPassWord", GateShareSeam.g_sClientPassWord);
        if (GateShareSeam.g_sClientPassWord == "") GateShareSeam.g_sClientPassWord = "BmM2";   // 原 :1122

        // ---- :1124-1128 CheckM2ServerTimeOut ----
        GateShareGlobals.g_dwCheckServerTimeOutTime = unchecked((uint)IniFile.ReadInteger(GateClass, "CheckM2ServerTimeOut", unchecked((int)GateShareGlobals.g_dwCheckServerTimeOutTime)));
        if (GateShareGlobals.g_dwCheckServerTimeOutTime <= unchecked((uint)bounds.CheckServerTimeOutTimeMin))
            GateShareGlobals.g_dwCheckServerTimeOutTime = unchecked((uint)bounds.CheckServerTimeOutTimeMin);
        else if (GateShareGlobals.g_dwCheckServerTimeOutTime >= unchecked((uint)bounds.CheckServerTimeOutTimeMax))
            GateShareGlobals.g_dwCheckServerTimeOutTime = unchecked((uint)bounds.CheckServerTimeOutTimeMax);

        // ---- :1130-1135 ClientSendBlockSize → MAX_OVERLAPPEDEX_BUFFER_SIZE ----
        int IntValue = IniFile.ReadInteger(GateClass, "ClientSendBlockSize", unchecked((int)GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE));
        if ((IntValue >= bounds.ClientSendBlockSizeMin) && (IntValue <= bounds.ClientSendBlockSizeMax))
        {
            GateShareGlobals.MAX_OVERLAPPEDEX_BUFFER_SIZE = unchecked((uint)IntValue);   // 原 :1134
        }

        // ---- :1137-1142 MaxPreallocatedMemorySize → MAX_PREALLOCATED_MEMORY_SIZE ----
        IntValue = IniFile.ReadInteger(GateClass, "MaxPreallocatedMemorySize", GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE);
        if ((IntValue >= bounds.PreAllocatedCountMin) && (IntValue <= bounds.PreAllocatedCountMax))
        {
            GateShareGlobals.MAX_PREALLOCATED_MEMORY_SIZE = IntValue;                    // 原 :1141
        }

        GateShareSeam.g_boLogoutNoResendAntiplugStream = IniFile.ReadBool(GateClass, "LogoutNoResendAntiplugStream", GateShareSeam.g_boLogoutNoResendAntiplugStream ? (byte)1 : (byte)0) != 0;   // 原 :1144
        // 原 :1145 `//g_boAntiplugAllLog := IniFile.ReadBool(GateClass, 'AntiplugAllLog', g_boAntiplugAllLog);`（★ C13）

        // ---- :1147-1152 RecvAntiPlugHeartbeatTimeOutTime ----
        IntValue = IniFile.ReadInteger(GateClass, "RecvAntiPlugHeartbeatTimeOutTime", GateShareGlobals.g_nRecvAntiPlugHeartbeatTimeOutTime);
        if ((IntValue >= bounds.RecvAntiPlugHeartbeatTimeOutTimeMin) && (IntValue <= bounds.RecvAntiPlugHeartbeatTimeOutTimeMax))
        {
            GateShareGlobals.g_nRecvAntiPlugHeartbeatTimeOutTime = IntValue;
        }

        // ---- :1154-1159 AntiPlugStreamSendSpeed ----
        IntValue = IniFile.ReadInteger(GateClass, "AntiPlugStreamSendSpeed", GateShareGlobals.g_nAntiPlugStreamSendSpeed);
        if ((IntValue >= bounds.AntiPlugStreamSendSpeedMin) && (IntValue <= bounds.AntiPlugStreamSendSpeedMax))
        {
            GateShareGlobals.g_nAntiPlugStreamSendSpeed = IntValue;
        }

        // ---- :1161-1166 AntiPlugStreamSendBlockSize ----
        IntValue = IniFile.ReadInteger(GateClass, "AntiPlugStreamSendBlockSize", GateShareGlobals.g_nAntiPlugStreamSendBlockSize);
        if ((IntValue >= 0) && (IntValue < bounds.AntiPlugStreamSendBlockSizeItemCount))
        {
            GateShareGlobals.g_nAntiPlugStreamSendBlockSize = IntValue;
        }

        // ---- :1168-1191 CLIENT_ANTIPLUG = 1 → 由块大小档位推出"发送间隔 + 每块字节数" ----
        //   注意 `case` **没有 else**：档位越界（不可能，已钳到 0..3）时两者都不改。
        switch (GateShareGlobals.g_nAntiPlugStreamSendBlockSize)
        {
            case 0:   // 原 :1170-1174 128K 计算方式
                GateShareGlobals.g_ClientAntiPlugDllSendInterval = 100;
                GateShareGlobals.g_ClientAntiPlugDllBlockSize = 128 * 1024 * GateShareGlobals.g_nAntiPlugStreamSendSpeed / 10;
                break;
            case 1:   // 原 :1175-1179 96K
                GateShareGlobals.g_ClientAntiPlugDllSendInterval = 75;
                GateShareGlobals.g_ClientAntiPlugDllBlockSize = 128 * 1024 * GateShareGlobals.g_nAntiPlugStreamSendSpeed / 13;
                break;
            case 2:   // 原 :1180-1184 64K
                GateShareGlobals.g_ClientAntiPlugDllSendInterval = 50;
                GateShareGlobals.g_ClientAntiPlugDllBlockSize = 128 * 1024 * GateShareGlobals.g_nAntiPlugStreamSendSpeed / 20;
                break;
            case 3:   // 原 :1185-1189 32K
                GateShareGlobals.g_ClientAntiPlugDllSendInterval = 25;
                GateShareGlobals.g_ClientAntiPlugDllBlockSize = 128 * 1024 * GateShareGlobals.g_nAntiPlugStreamSendSpeed / 40;
                break;
        }

        GateShareSeam.g_dwClientAccumulateMaxSize = IniFile.ReadInteger(GateClass, "ClientAccumulateMaxSize", GateShareSeam.g_dwClientAccumulateMaxSize);   // 原 :1193

        // ---- :1195-1199 ★ C2：同一个键读两次 ----
        if (IniFile.ReadInteger(GateClass, "AttackTick", -1) > 0)
            FormGlobals.g_dwAttackTick = unchecked((uint)IniFile.ReadInteger(GateClass, "AttackTick", unchecked((int)FormGlobals.g_dwAttackTick)));
        if (IniFile.ReadInteger(GateClass, "AttackCount", -1) > 0)
            FormGlobals.g_nAttackCount = IniFile.ReadInteger(GateClass, "AttackCount", FormGlobals.g_nAttackCount);

        FormGlobals.g_nMaxConnOfIPaddr = IniFile.ReadInteger(GateClass, "MaxConnOfIPaddr", FormGlobals.g_nMaxConnOfIPaddr);          // 原 :1201
        // 原 :1202 ★ C4：不做范围校验
        FormGlobals.g_BlockMethod = unchecked((TBlockIPMethod)IniFile.ReadInteger(GateClass, "BlockMethod", (int)FormGlobals.g_BlockMethod));

        // ---- :1204-1208 ★ C3 ----
        FormGlobals.g_nMaxClientPacketSize = IniFile.ReadInteger(GateClass, "MaxClientPacketSize", FormGlobals.g_nMaxClientPacketSize);
        if (FormGlobals.g_nMaxClientPacketSize > 512) FormGlobals.g_nMaxClientPacketSize = 128;
        FormGlobals.g_nMaxClientPacketCount = IniFile.ReadInteger(GateClass, "MaxClientPacketCount", FormGlobals.g_nMaxClientPacketCount);
        FormGlobals.nMaxClientMsgCount = IniFile.ReadInteger(GateClass, "MaxClientMsgCount", FormGlobals.nMaxClientMsgCount);
        FormGlobals.g_boKickOverPacketSize = IniFile.ReadBool(GateClass, "kickOverPacket", FormGlobals.g_boKickOverPacketSize ? (byte)1 : (byte)0) != 0;

        FormGlobals.g_boCheckClientPacketLegal = IniFile.ReadBool(GateClass, "CheckClientPacketLegal", FormGlobals.g_boCheckClientPacketLegal ? (byte)1 : (byte)0) != 0;   // 原 :1210
        FormGlobals.g_nCheckClientPacketCount = IniFile.ReadInteger(GateClass, "CheckClientPacketCount", FormGlobals.g_nCheckClientPacketCount);        // 原 :1211

        // ---- :1213-1268 按 27 个动作模式读 Enabled/Interval/... ----
        for (int ActionMode = 0; ActionMode < RunGateConst.ActionModeCount; ActionMode++)                              // 原 :1213
        {
            string Section = RunGateConst.AntiPlugActionModeSections[ActionMode];                                      // 原 :1215

            // 原 :1217 `if ActionModeUseSpeedIntervals(ActionMode) and (not (ActionMode in [amHit, amSpell, amWalk, amRun])) then`
            //   ★ 子代理 D12：`ActionModeUseSpeedIntervals` 已经把 amHit/amSpell/amWalk/amRun 包含在内，
            //   所以第二个合取项对这四个恒为假 —— 整条件等价于"是这 18 个非基础模式之一"。
            if (FormGlobals.ActionModeUseSpeedIntervals((TAntiPlugActionMode)ActionMode)
                && !(ActionMode == (int)TAntiPlugActionMode.amHit
                  || ActionMode == (int)TAntiPlugActionMode.amSpell
                  || ActionMode == (int)TAntiPlugActionMode.amWalk
                  || ActionMode == (int)TAntiPlugActionMode.amRun))
            {
                FormGlobals.g_boSendSpeedIntervalsToClient[ActionMode] =
                    IniFile.ReadBool(Section, "SendSpeedIntervalsToClient", FormGlobals.g_boSendSpeedIntervalsToClient[ActionMode] ? (byte)1 : (byte)0) != 0;   // 原 :1219
            }

            var Action = FormGlobals.g_Config.ActionList[ActionMode];                                                  // 原 :1222 `Action := @g_Config.ActionList[ActionMode];`

            if (ActionMode >= (int)TAntiPlugActionMode.amHitConcurrent)                                                // 原 :1224
            {
                Action.boEnabled = true;                                                                              // 原 :1226
            }
            else
            {
                Action.boEnabled = IniFile.ReadBool(Section, "Enabled", Action.boEnabled ? (byte)1 : (byte)0) != 0;    // 原 :1230
                Action.nInterval = unchecked((uint)IniFile.ReadInteger(Section, "Interval", unchecked((int)Action.nInterval)));   // 原 :1231

                IntValue = IniFile.ReadInteger(Section, "ProcessMode", (int)Action.ProcessMode);                      // 原 :1233
                if ((IntValue >= (int)TActionProcessMode.apmDelay) && (IntValue <= (int)TActionProcessMode.apmNoProcess))
                    Action.ProcessMode = (TActionProcessMode)IntValue;                                                // 原 :1235
                else
                    Action.ProcessMode = TActionProcessMode.apmDelay;                                                 // 原 :1237 `Low(TActionProcessMode)`

                Action.boProcessScript = IniFile.ReadBool(Section, "ProcessScript", Action.boProcessScript ? (byte)1 : (byte)0) != 0;   // 原 :1239

                IntValue = IniFile.ReadInteger(Section, "SumProcessMode", (int)Action.SumProcessMode);                // 原 :1241
                if ((IntValue >= (int)TSumActionProcessMode.sapmNone) && (IntValue <= (int)TSumActionProcessMode.sampLockUser))
                    Action.SumProcessMode = (TSumActionProcessMode)IntValue;                                          // 原 :1243
                else
                    Action.SumProcessMode = TSumActionProcessMode.sapmNone;                                           // 原 :1245

                // 原 :1246 `//Action.nFloatingInterval := ...`（整行注释）
                Action.boShowHint = IniFile.ReadBool(Section, "ShowHint", Action.boShowHint ? (byte)1 : (byte)0) != 0;   // 原 :1247
                Action.sHintText = IniFile.ReadString(Section, "HintText", Action.sHintText);                          // 原 :1248
                Action.nCompensationValue = IniFile.ReadInteger(Section, "CompensationValue", Action.nCompensationValue);   // 原 :1249
                Action.boDebug = IniFile.ReadBool(Section, "Debug", Action.boDebug ? (byte)1 : (byte)0) != 0;          // 原 :1250
            }

            // 原 :1254-1267 是被 `{ }` 整段注释掉的 14 行重复代码（★ 子代理 D11）——不移植，注释保留于报告。
        }

        // ---- :1270-1319 Setup 段 ----
        var cfg = FormGlobals.g_Config;
        cfg.nLockTime = IniFile.ReadInteger("Setup", "LockTime", cfg.nLockTime);                                       // 原 :1270
        cfg.boSaveLockStatus = IniFile.ReadBool("Setup", "SaveLockStatus", cfg.boSaveLockStatus ? (byte)1 : (byte)0) != 0;   // 原 :1271
        cfg.boShowLockLog = IniFile.ReadBool("Setup", "ShowLockLog", cfg.boShowLockLog ? (byte)1 : (byte)0) != 0;      // 原 :1272
        cfg.sShowLockMsg = IniFile.ReadString("Setup", "ShowLockMsg", cfg.sShowLockMsg);                               // 原 :1273

        cfg.boSpeedClearData = IniFile.ReadBool("Setup", "SpeedClearData", cfg.boSpeedClearData ? (byte)1 : (byte)0) != 0;   // 原 :1275

        cfg.btMsgType = unchecked((byte)IniFile.ReadInteger("Setup", "MsgType", cfg.btMsgType));                       // 原 :1277
        cfg.btMsgFColor = unchecked((byte)IniFile.ReadInteger("Setup", "MsgFColor", cfg.btMsgFColor));                 // 原 :1278
        cfg.btMsgBColor = unchecked((byte)IniFile.ReadInteger("Setup", "MsgBColor", cfg.btMsgBColor));                 // 原 :1279

        cfg.dwUserShop_Search_Interval = unchecked((uint)IniFile.ReadInteger("Setup", "UserShopSearchInterval", unchecked((int)cfg.dwUserShop_Search_Interval)));   // 原 :1281
        cfg.boUserShop_Search_ShowHint = IniFile.ReadBool("Setup", "UserShopSearchShowHint", cfg.boUserShop_Search_ShowHint ? (byte)1 : (byte)0) != 0;               // 原 :1282

        cfg.dwUserShop_Buy_Interval = unchecked((uint)IniFile.ReadInteger("Setup", "UserShopBuyInterval", unchecked((int)cfg.dwUserShop_Buy_Interval)));            // 原 :1284
        cfg.boUserShop_Buy_ShowHint = IniFile.ReadBool("Setup", "UserShopBuyShowHint", cfg.boUserShop_Buy_ShowHint ? (byte)1 : (byte)0) != 0;                        // 原 :1285

        cfg.dwTakeOn_Item_Interval = unchecked((uint)IniFile.ReadInteger("Setup", "TakeOnItemInterval", unchecked((int)cfg.dwTakeOn_Item_Interval)));                // 原 :1287
        cfg.boTakeOn_Item_ShowHint = IniFile.ReadBool("Setup", "TakeOnItemShowHint", cfg.boTakeOn_Item_ShowHint ? (byte)1 : (byte)0) != 0;                            // 原 :1288

        cfg.dwDealTry_Attack_Interval = unchecked((uint)IniFile.ReadInteger("Setup", "DealTryAttackInterval", unchecked((int)cfg.dwDealTry_Attack_Interval)));       // 原 :1290
        cfg.boDealTry_Attack_ShowHint = IniFile.ReadBool("Setup", "DealTryAttackShowHint", cfg.boDealTry_Attack_ShowHint ? (byte)1 : (byte)0) != 0;                   // 原 :1291

        cfg.dwBrutal_Attack_Interval = unchecked((uint)IniFile.ReadInteger("Setup", "BrutalAttackInterval", unchecked((int)cfg.dwBrutal_Attack_Interval)));          // 原 :1293
        cfg.boBrutal_Attack_ShowHint = IniFile.ReadBool("Setup", "BrutalAttackShowHint", cfg.boBrutal_Attack_ShowHint ? (byte)1 : (byte)0) != 0;                      // 原 :1294

        cfg.boShowAttackLog = IniFile.ReadBool("Setup", "ShowAttackLog", cfg.boShowAttackLog ? (byte)1 : (byte)0) != 0;   // 原 :1296
        // 原 :1297 `//g_Config.boShowDropConcurrentLog := ...`（整行注释）
        cfg.dwContinueSpeedPassIncTime = unchecked((uint)IniFile.ReadInteger("Setup", "ContinueSpeedPassIncTime", unchecked((int)cfg.dwContinueSpeedPassIncTime)));   // 原 :1298

        IntValue = IniFile.ReadInteger("Setup", "CollectCount", cfg.dwCollectCount);                                   // 原 :1300
        if ((IntValue >= 5) && (IntValue <= 20)) cfg.dwCollectCount = IntValue;                                        // 原 :1301-1303

        IntValue = IniFile.ReadInteger("Setup", "SpeedValue", cfg.dwSpeedValue);                                       // 原 :1306
        if ((IntValue >= 2) && (IntValue <= 18)) cfg.dwSpeedValue = IntValue;                                          // 原 :1307-1309

        if (cfg.dwSpeedValue >= cfg.dwCollectCount) cfg.dwSpeedValue = cfg.dwCollectCount - 1;                          // 原 :1312-1315（★ C9）

        cfg.boZeroCompensationValueClearPool = IniFile.ReadBool("Setup", "ZeroCompensationValueClearPool", cfg.boZeroCompensationValueClearPool ? (byte)1 : (byte)0) != 0;   // 原 :1317

        cfg.dwClientUploadPickItemsTime = unchecked((ushort)IniFile.ReadInteger("Setup", "ClientUploadPickItemsTime", cfg.dwClientUploadPickItemsTime));   // 原 :1319

        cfg.boContinueSpeedCloseSocket = IniFile.ReadBool("Setup", "ContinueSpeedCloseSocket", cfg.boContinueSpeedCloseSocket ? (byte)1 : (byte)0) != 0;   // 原 :1321
        IntValue = IniFile.ReadInteger("Setup", "ContinueSpeedCount", cfg.nContinueSpeedCount);                        // 原 :1322
        if ((IntValue >= 2) && (IntValue <= 8)) cfg.nContinueSpeedCount = IntValue;                                    // 原 :1323-1324

        cfg.nSumSpeedCheckTime = IniFile.ReadInteger("Setup", "SumSpeedCheckTime", cfg.nSumSpeedCheckTime);           // 原 :1326
        cfg.nSumSpeedMaxCount = IniFile.ReadInteger("Setup", "SumSpeedMaxCount", cfg.nSumSpeedMaxCount);              // 原 :1327

        // ---- :1329-1335 消息过滤 ----
        FormGlobals.g_boFilterSayMsg = IniFile.ReadBool(GateClass, "FilterSayMsg", FormGlobals.g_boFilterSayMsg ? (byte)1 : (byte)0) != 0;   // 原 :1329
        IntValue = IniFile.ReadInteger(GateClass, "FilterSayMsgMode", (int)FormGlobals.g_FilterSayMsgMode);            // 原 :1330
        if ((IntValue >= (int)TFilterSayMsgMode.fsmmAllBlock) && (IntValue <= (int)TFilterSayMsgMode.fsmmDisMsgorSys))
            FormGlobals.g_FilterSayMsgMode = (TFilterSayMsgMode)IntValue;                                              // 原 :1332
        // 原 :1333 —— **默认值是 ''，不是 g_WarnSayMsg**（键缺失会把警告文字清空！）
        FormGlobals.g_WarnSayMsg = IniFile.ReadString(GateClass, "WarnSayMsg", "");

        FormGlobals.g_boFilterSayTriggerScript = IniFile.ReadBool(GateClass, "FilterSayTriggerScript", FormGlobals.g_boFilterSayTriggerScript ? (byte)1 : (byte)0) != 0;   // 原 :1335

        // ---- :1338-1353 发言控制 ----
        FormGlobals.g_dwKeepConnectTimeOut = unchecked((uint)IniFile.ReadInteger(GateClass, "KeepConnectTimeOut", unchecked((int)FormGlobals.g_dwKeepConnectTimeOut)));   // 原 :1338
        FormGlobals.g_boSayMsgControl = IniFile.ReadBool(GateClass, "SayMsgControl", FormGlobals.g_boSayMsgControl ? (byte)1 : (byte)0) != 0;                              // 原 :1341
        FormGlobals.g_dwSayMaxLen = unchecked((uint)IniFile.ReadInteger(GateClass, "SayMaxLen", unchecked((int)FormGlobals.g_dwSayMaxLen)));                              // 原 :1344
        FormGlobals.g_dwSayTime = unchecked((uint)IniFile.ReadInteger(GateClass, "SayTime", unchecked((int)FormGlobals.g_dwSayTime)));                                    // 原 :1347
        FormGlobals.g_dwSayMaxCount = unchecked((uint)IniFile.ReadInteger(GateClass, "SayMaxCount", unchecked((int)FormGlobals.g_dwSayMaxCount)));                        // 原 :1350
        FormGlobals.g_dwSayDisableTime = unchecked((uint)IniFile.ReadInteger(GateClass, "SayDisableTime", unchecked((int)FormGlobals.g_dwSayDisableTime)));                // 原 :1353

        // ---- :1356-1367 ★ C6：缺键则**写入**默认 ----
        string StrValue = IniFile.ReadString("String", "DisableSayMsg", "");
        if (StrValue.Length == 0)
            IniFile.WriteString("String", "DisableSayMsg", GateShareSeam.g_sDisableSayMsg);                       // 原 :1358
        else
            GateShareSeam.g_sDisableSayMsg = StrValue;                                                            // 原 :1360

        StrValue = IniFile.ReadString("String", "DisableSayMsgBegin", "");
        if (StrValue.Length == 0)
            IniFile.WriteString("String", "DisableSayMsgBegin", GateShareSeam.g_sDisableSayMsgBegin);             // 原 :1365
        else
            GateShareSeam.g_sDisableSayMsgBegin = StrValue;                                                       // 原 :1367

        // ---- :1369-1372 IP 频率限制 ----
        FormGlobals.g_dwIPCountLimitTime1 = unchecked((uint)IniFile.ReadInteger(GateClass, "IPCountLimitTime1", unchecked((int)FormGlobals.g_dwIPCountLimitTime1)));
        FormGlobals.g_dwIPCountLimit1 = unchecked((uint)IniFile.ReadInteger(GateClass, "IPCountLimit1", unchecked((int)FormGlobals.g_dwIPCountLimit1)));
        FormGlobals.g_dwIPCountLimitTime2 = unchecked((uint)IniFile.ReadInteger(GateClass, "IPCountLimitTime2", unchecked((int)FormGlobals.g_dwIPCountLimitTime2)));
        FormGlobals.g_dwIPCountLimit2 = unchecked((uint)IniFile.ReadInteger(GateClass, "IPCountLimit2", unchecked((int)FormGlobals.g_dwIPCountLimit2)));

        // ---- :1375-1404 防御设置 ----
        FormGlobals.g_dwDefenseLevel = unchecked((uint)IniFile.ReadInteger(GateClass, "DefenseLevel", unchecked((int)FormGlobals.g_dwDefenseLevel)));       // 原 :1375
        FormGlobals.g_boDefenseToLevel1 = IniFile.ReadBool(GateClass, "IsDefenseToLevel1", FormGlobals.g_boDefenseToLevel1 ? (byte)1 : (byte)0) != 0;        // 原 :1378
        FormGlobals.g_dwDefenseToLevel1 = unchecked((uint)IniFile.ReadInteger(GateClass, "DefenseToLevel1", unchecked((int)FormGlobals.g_dwDefenseToLevel1)));   // 原 :1381
        FormGlobals.g_boResotreDefense = IniFile.ReadBool(GateClass, "IsResotreDefense", FormGlobals.g_boResotreDefense ? (byte)1 : (byte)0) != 0;            // 原 :1384
        FormGlobals.g_dwResotreDefense = unchecked((uint)IniFile.ReadInteger(GateClass, "ResotreDefense", unchecked((int)FormGlobals.g_dwResotreDefense)));    // 原 :1387
        FormGlobals.g_boAutoClearTemp = IniFile.ReadBool(GateClass, "IsAutoClearTemp", FormGlobals.g_boAutoClearTemp ? (byte)1 : (byte)0) != 0;              // 原 :1390
        FormGlobals.g_dwAutoClearTemp = unchecked((uint)IniFile.ReadInteger(GateClass, "AutoClearTemp", unchecked((int)FormGlobals.g_dwAutoClearTemp)));       // 原 :1393
        FormGlobals.g_boAddAllToTemp = IniFile.ReadBool(GateClass, "IsAddAllToTemp", FormGlobals.g_boAddAllToTemp ? (byte)1 : (byte)0) != 0;                  // 原 :1396
        FormGlobals.g_dwAddAllToTemp = unchecked((uint)IniFile.ReadInteger(GateClass, "AddAllToTemp", unchecked((int)FormGlobals.g_dwAddAllToTemp)));           // 原 :1399
        FormGlobals.g_boOpenCheckClient = IniFile.ReadBool(GateClass, "OpenCheckClient", FormGlobals.g_boOpenCheckClient ? (byte)1 : (byte)0) != 0;            // 原 :1402
        // 原 :1404 ★ C4：不做范围校验
        FormGlobals.g_CheckClientFailBlockMethod = unchecked((TBlockIPMethod)IniFile.ReadInteger(GateClass, "CheckClientFailBlockMethod", (int)FormGlobals.g_CheckClientFailBlockMethod));

        // ---- :1407-1413 多网关端口 ----
        int Count = IniFile.ReadInteger("GameGates", "Count", 0);                                                      // 原 :1407
        for (int I = 1; I <= Count; I++)                                                                               // 原 :1408
        {
            IntValue = IniFile.ReadInteger("GameGates", "Port" + I, 0);                                                // 原 :1410
            if ((IntValue > 0) && (IntValue <= 65535)) ports.Add(IntValue);                                            // 原 :1411-1412
        }

        // ---- :1416-1438 防御设置（FY） ----
        FormGlobals.g_sFYReadDenyIPFile = IniFile.ReadString(GateClass, "FYReadDenyIPFile", FormGlobals.g_sFYReadDenyIPFile);        // 原 :1416
        FormGlobals.g_dwFYReadDenyIPTime = unchecked((uint)IniFile.ReadInteger(GateClass, "FYReadDenyIPTime", unchecked((int)FormGlobals.g_dwFYReadDenyIPTime)));   // 原 :1417

        if (IniFile.ValueExists(GateClass, "FYDownDenyIPUrl"))                                                                     // 原 :1419 ★ C7
            FormGlobals.g_sFYDownDenyIPUrl = IniFile.ReadString(GateClass, "FYDownDenyIPUrl", "");

        FormGlobals.g_dwFYDownDenyIPTime = unchecked((uint)IniFile.ReadInteger(GateClass, "FYDownDenyIPTime", unchecked((int)FormGlobals.g_dwFYDownDenyIPTime)));   // 原 :1422

        FormGlobals.g_sFYReadPassIPFile = IniFile.ReadString(GateClass, "FYReadPassIPFile", FormGlobals.g_sFYReadPassIPFile);        // 原 :1424
        FormGlobals.g_dwFYReadPassIPTime = unchecked((uint)IniFile.ReadInteger(GateClass, "FYReadPassIPTime", unchecked((int)FormGlobals.g_dwFYReadPassIPTime)));   // 原 :1425

        if (IniFile.ValueExists(GateClass, "FYDownPassIPUrl"))                                                                     // 原 :1427 ★ C7
            FormGlobals.g_sFYDownPassIPUrl = IniFile.ReadString(GateClass, "FYDownPassIPUrl", "");

        FormGlobals.g_dwFYDownPassIPTime = unchecked((uint)IniFile.ReadInteger(GateClass, "FYDownPassIPTime", unchecked((int)FormGlobals.g_dwFYDownPassIPTime)));   // 原 :1430

        FormGlobals.g_sFYReadDenyMACFile = IniFile.ReadString(GateClass, "FYReadDenyMACFile", FormGlobals.g_sFYReadDenyMACFile);     // 原 :1432
        FormGlobals.g_dwFYReadDenyMACTime = unchecked((uint)IniFile.ReadInteger(GateClass, "FYReadDenyMACTime", unchecked((int)FormGlobals.g_dwFYReadDenyMACTime)));   // 原 :1433

        // 原 :1435 ★ C7：**没有** ValueExists 保护（与上面两个 URL 不一致）
        FormGlobals.g_sFYDownDenyMACUrl = IniFile.ReadString(GateClass, "FYDownDenyMACUrl", FormGlobals.g_sFYDownDenyMACUrl);
        FormGlobals.g_dwFYDownDenyMACTime = unchecked((uint)IniFile.ReadInteger(GateClass, "FYDownDenyMACTime", unchecked((int)FormGlobals.g_dwFYDownDenyMACTime)));   // 原 :1436

        FormGlobals.g_OnlyWhiteListLink = IniFile.ReadBool(GateClass, "OnlyWhiteListLink", FormGlobals.g_OnlyWhiteListLink ? (byte)1 : (byte)0) != 0;   // 原 :1438

        FormGlobals.g_boLogClientPacket = IniFile.ReadBool(GateClass, "LogClientPacket", FormGlobals.g_boLogClientPacket ? (byte)1 : (byte)0) != 0;   // 原 :1440
        FormGlobals.g_nLogClientPacketType = IniFile.ReadInteger(GateClass, "LogClientPacketType", FormGlobals.g_nLogClientPacketType);               // 原 :1441

        // ---- :1443-1451 CLIENT_ANTIPLUG = 1 ----
        GateShareGlobals.g_ClientAntiPlugStream ??= new System.IO.MemoryStream();                                     // 原 :1444 `g_ClientAntiPlugStream := TMemoryStream.Create;`
        // 原 :1445 `InitializeCriticalSection(g_CSRunGatePlug);` → 托管侧 `GateShareSeam.g_CSRunGatePlug` 是现成的 lock 对象
        FormGlobals.g_boAntiPlugAutoUpdateCheck = IniFile.ReadBool(GateClass, "AntiPlugAutoUpdateCheck", FormGlobals.g_boAntiPlugAutoUpdateCheck ? (byte)1 : (byte)0) != 0;   // 原 :1447
        // ★ 注意：原文 :1448 用的默认值是 `g_wAntiPlugUpdateCheckInterval`（var 段初值 **5**，GateShare.pas:609），
        //   而 `FormGlobals.g_wAntiPlugUpdateCheckInterval` 被接缝置成 **1** → 键缺失时结果不同。差异见报告。
        FormGlobals.g_wAntiPlugUpdateCheckInterval = IniFile.ReadInteger(GateClass, "AntiPlugUpdateCheckInterval", FormGlobals.g_wAntiPlugUpdateCheckInterval);   // 原 :1448
        if (FormGlobals.g_wAntiPlugUpdateCheckInterval < 1) FormGlobals.g_wAntiPlugUpdateCheckInterval = 1;             // 原 :1449（★ C5：只钳下界）
        // 原 :1450 —— 注意键名末尾的 **5**（`AntiPlugUpdateConfigUrl5`），与前任报告 §5.24-15 同型
        FormGlobals.g_sAntiPlugUpdateConfigUrl = IniFile.ReadString(GateClass, "AntiPlugUpdateConfigUrl5", FormGlobals.g_sAntiPlugUpdateConfigUrl);

        // ---- :1453-1467 单机限制 / 退出延时 ----
        GateShareGlobals.g_boOneMACLimitePlayer = IniFile.ReadBool(GateClass, "OneMACLimitePlayer", GateShareGlobals.g_boOneMACLimitePlayer ? (byte)1 : (byte)0) != 0;   // 原 :1453
        GateShareGlobals.g_nOneMACLimitePlayerCount = IniFile.ReadInteger(GateClass, "OneMACLimitePlayerCount", GateShareGlobals.g_nOneMACLimitePlayerCount);              // 原 :1454

        GateShareSeam.g_nClientLogoutDelay = IniFile.ReadInteger(GateClass, "ClientLogoutDelay", GateShareSeam.g_nClientLogoutDelay);   // 原 :1456
        GateShareSeam.g_nClientCloseDelay = IniFile.ReadInteger(GateClass, "ClientCloseDelay", GateShareSeam.g_nClientCloseDelay);      // 原 :1457

        GateShareSeam.g_boDelayCloseDisableMove = IniFile.ReadBool(GateClass, "DelayCloseDisableMove", GateShareSeam.g_boDelayCloseDisableMove ? (byte)1 : (byte)0) != 0;         // 原 :1459
        GateShareSeam.g_boDelayCloseDisableSpell = IniFile.ReadBool(GateClass, "DelayCloseDisableSpell", GateShareSeam.g_boDelayCloseDisableSpell ? (byte)1 : (byte)0) != 0;      // 原 :1460
        GateShareSeam.g_boDelayCloseDisableAttack = IniFile.ReadBool(GateClass, "DelayCloseDisableAttack", GateShareSeam.g_boDelayCloseDisableAttack ? (byte)1 : (byte)0) != 0;   // 原 :1461
        GateShareSeam.g_boDelayCloseDisableUseItem = IniFile.ReadBool(GateClass, "DelayCloseDisableUseItem", GateShareSeam.g_boDelayCloseDisableUseItem ? (byte)1 : (byte)0) != 0;   // 原 :1462

        GateShareSeam.g_boBreakClientLogoutHint = IniFile.ReadBool(GateClass, "ShowBreakClientLogoutHint", GateShareSeam.g_boBreakClientLogoutHint ? (byte)1 : (byte)0) != 0;   // 原 :1464
        GateShareSeam.g_sBreakClientLogoutHint = IniFile.ReadString(GateClass, "BreakClientLogoutHint", GateShareSeam.g_sBreakClientLogoutHint);                                  // 原 :1465
        GateShareSeam.g_boBreakClientCloseHint = IniFile.ReadBool(GateClass, "ShowBreakClientCloseHint", GateShareSeam.g_boBreakClientCloseHint ? (byte)1 : (byte)0) != 0;       // 原 :1466
        GateShareSeam.g_sBreakClientCloseHint = IniFile.ReadString(GateClass, "BreakClientCloseHint", GateShareSeam.g_sBreakClientCloseHint);                                     // 原 :1467

        // ---- :1470-1480 MagicCD ----
        IntValue = IniFile.ReadInteger("MagicCD", "MsgType", FormGlobals.g_btMagicCDMsgType);                          // 原 :1470
        if ((IntValue >= 0) && (IntValue <= 2)) FormGlobals.g_btMagicCDMsgType = unchecked((byte)IntValue);            // 原 :1471-1473

        FormGlobals.g_sMagicCDMsgText = IniFile.ReadString("MagicCD", "MsgText", FormGlobals.g_sMagicCDMsgText);      // 原 :1476
        FormGlobals.g_btMagicCDFColor = unchecked((byte)IniFile.ReadInteger("MagicCD", "FColor", FormGlobals.g_btMagicCDFColor));   // 原 :1477
        FormGlobals.g_btMagicCDBColor = unchecked((byte)IniFile.ReadInteger("MagicCD", "BColor", FormGlobals.g_btMagicCDBColor));   // 原 :1478
        FormGlobals.g_nMagicCDShowX = IniFile.ReadInteger("MagicCD", "ShowX", FormGlobals.g_nMagicCDShowX);            // 原 :1479
        FormGlobals.g_nMagicCDShowY = IniFile.ReadInteger("MagicCD", "ShowY", FormGlobals.g_nMagicCDShowY);            // 原 :1480

        // ---- :1483-1505 吃药 CD（★ C10：键名前缀从 1 起、无分隔符） ----
        for (int I = 0; I < FormGlobals.g_EatItemCDConfig.Hum.Length; I++)                                              // 原 :1483
        {
            StrValue = (I + 1).ToString();                                                                             // 原 :1485 `IntToStr(I + 1)`
            var h = FormGlobals.g_EatItemCDConfig.Hum[I];
            h.NormalHP = IniFile.ReadInteger("HumanItemEatCD", StrValue + "NormalHP", h.NormalHP);         // 原 :1486
            h.NormalMP = IniFile.ReadInteger("HumanItemEatCD", StrValue + "NormalMP", h.NormalMP);         // 原 :1487
            h.NormalHPMP = IniFile.ReadInteger("HumanItemEatCD", StrValue + "NormalHPMP", h.NormalHPMP);   // 原 :1488
            h.SpecialHP = IniFile.ReadInteger("HumanItemEatCD", StrValue + "SpecialHP", h.SpecialHP);       // 原 :1489
            h.SpecialMP = IniFile.ReadInteger("HumanItemEatCD", StrValue + "SpecialMP", h.SpecialMP);       // 原 :1490
            h.SpecialHPMP = IniFile.ReadInteger("HumanItemEatCD", StrValue + "SpecialHPMP", h.SpecialHPMP); // 原 :1491
            h.Other = IniFile.ReadInteger("HumanItemEatCD", StrValue + "Other", h.Other);                   // 原 :1492
        }

        for (int I = 0; I < FormGlobals.g_EatItemCDConfig.Hero.Length; I++)                                             // 原 :1495
        {
            StrValue = (I + 1).ToString();                                                                             // 原 :1497
            var h = FormGlobals.g_EatItemCDConfig.Hero[I];
            h.NormalHP = IniFile.ReadInteger("HeroItemEatCD", StrValue + "NormalHP", h.NormalHP);         // 原 :1498
            h.NormalMP = IniFile.ReadInteger("HeroItemEatCD", StrValue + "NormalMP", h.NormalMP);         // 原 :1499
            h.NormalHPMP = IniFile.ReadInteger("HeroItemEatCD", StrValue + "NormalHPMP", h.NormalHPMP);   // 原 :1500
            h.SpecialHP = IniFile.ReadInteger("HeroItemEatCD", StrValue + "SpecialHP", h.SpecialHP);       // 原 :1501
            h.SpecialMP = IniFile.ReadInteger("HeroItemEatCD", StrValue + "SpecialMP", h.SpecialMP);       // 原 :1502
            h.SpecialHPMP = IniFile.ReadInteger("HeroItemEatCD", StrValue + "SpecialHPMP", h.SpecialHPMP); // 原 :1503
            h.Other = IniFile.ReadInteger("HeroItemEatCD", StrValue + "Other", h.Other);                   // 原 :1504
        }

        // 原 :1507 `IniFile.Free;`（托管侧由调用方 using/Dispose）
        // 原 :1509-1855 是纯 UI 尾巴（把全局量回填控件 + 建插件菜单），属 S2/UI 片，本片不含。
    }

    // ==================================================================================
    // :2766-2806 btnSettingOKClick 的输入校验
    // ==================================================================================

    /// <summary>`btnSettingOKClick` 的校验结果（原 :2773-2806 的 5 条 `ErrMessage` + `Exit`）。</summary>
    public enum SettingValidation
    {
        /// <summary>全部通过（原 :2808 起才赋值）。</summary>
        Ok = 0,
        /// <summary>原 :2775 `'网关地址设置错误！'`。</summary>
        GateAddrInvalid,
        /// <summary>原 :2782 `'网关端口设置错误！'`。</summary>
        GatePortInvalid,
        /// <summary>原 :2789 `'服务器地址设置错误！'`。</summary>
        ServerAddrInvalid,
        /// <summary>原 :2796 `'服务器端口设置错误！'`。</summary>
        ServerPortInvalid,
        /// <summary>原 :2803 `'应用程序标题不能为空！'`。</summary>
        TitleEmpty,
    }

    /// <summary>原文 :2766-2806 的校验顺序（逐条 `Exit`，顺序即优先级）。</summary>
    public static SettingValidation ValidateSettingInputs(
        string gateIpText, string gatePortText, string serverIpText, string serverPortText, string titleText,
        out string gateIp, out int gatePort, out string serverIp, out int serverPort, out string title)
    {
        gateIp = (gateIpText ?? "").Trim();                                          // 原 :2766 `Trim(edtGateIPaddr.Text)`
        gatePort = DelphiRTL.StrToIntDef((gatePortText ?? "").Trim(), -1);            // 原 :2767
        serverIp = (serverIpText ?? "").Trim();                                      // 原 :2768
        serverPort = DelphiRTL.StrToIntDef((serverPortText ?? "").Trim(), -1);        // 原 :2769
        title = (titleText ?? "").Trim();                                            // 原 :2770

        if (!HUtil32.IsIPaddr(gateIp)) return SettingValidation.GateAddrInvalid;      // 原 :2773-2778
        if ((gatePort < 0) || (gatePort > 65535)) return SettingValidation.GatePortInvalid;   // 原 :2780-2785（★ C11：允许 0）
        if (!HUtil32.IsIPaddr(serverIp)) return SettingValidation.ServerAddrInvalid;  // 原 :2787-2792
        if ((serverPort < 0) || (serverPort > 65535)) return SettingValidation.ServerPortInvalid;   // 原 :2794-2799
        if (title.Length == 0) return SettingValidation.TitleEmpty;                   // 原 :2801-2806
        return SettingValidation.Ok;
    }

    /// <summary>原文 :2808-2809 `g_sGateAddr := StrGateIP; g_wdGatePort := IntGatePort;` 等 5 条赋值。
    /// `dbPort` 对应原 :2771/:2812 —— ★ C11：**原文不校验 DBPort**。</summary>
    public static void ApplySettingGlobals(string gateIp, int gatePort, string serverIp, int serverPort, string title, int dbPort)
    {
        GateShareGlobals.g_sGateAddr = gateIp;                                        // 原 :2808
        GateShareGlobals.g_wdGatePort = unchecked((ushort)gatePort);                  // 原 :2809（★ C12：Integer → Word 截断）
        GateShareGlobals.g_sServerAddr = serverIp;                                    // 原 :2810
        GateShareGlobals.g_wdServerPort = unchecked((ushort)serverPort);              // 原 :2811
        GateShareGlobals.g_wdDBPort = unchecked((ushort)dbPort);                      // 原 :2812
        GateShareGlobals.g_sTitleName = title;                                       // 原 :2813（原文此处为 `g_sTitleName := StrTitleName`）
    }

    // ==================================================================================
    // :2872-2932 btnSettingOKClick 的 48 个 INI 写回
    // ==================================================================================

    /// <summary>原文 :2872-2932 的写回（`TIniFile.Create(g_sIniFileName)` → 48 个 `Write*` → `Free`）。
    /// `clientSendBlockSizeControlValue` / `preAllocatedCountControlValue` /
    /// `antiPlugStreamSendBlockSizeItemIndex` 对应原文用的**控件值**（★ C14）。</summary>
    public static void WriteConfig(
        TCustomIniFileEx IniFile,
        int clientSendBlockSizeControlValue,
        int preAllocatedCountControlValue,
        int antiPlugStreamSendBlockSizeItemIndex)
    {
        if (IniFile == null) throw new ArgumentNullException(nameof(IniFile));
        string G = RunGateConst.GateClass;

        IniFile.WriteString(G, "GateAddr", GateShareGlobals.g_sGateAddr);                                              // 原 :2874
        IniFile.WriteInteger(G, "GatePort", GateShareGlobals.g_wdGatePort);                                             // 原 :2875
        IniFile.WriteString(G, "Server1", GateShareGlobals.g_sServerAddr);                                              // 原 :2876
        IniFile.WriteInteger(G, "ServerPort", GateShareGlobals.g_wdServerPort);                                         // 原 :2877
        IniFile.WriteBool(G, "CheckClientPassword", GateShareSeam.g_boCheckClientPassword ? (byte)1 : (byte)0);         // 原 :2878（键名是 Password 大写 P，读取侧是 PassWord）
        IniFile.WriteString(G, "ClientPassWord", GateShareSeam.g_sClientPassWord);                                       // 原 :2879

        IniFile.WriteString(G, "Title", GateShareGlobals.g_sTitleName);                                                 // 原 :2881

        IniFile.WriteInteger(G, "ShowLogLevel", GateShareGlobals.g_btShowLogLevel);                                     // 原 :2883
        IniFile.WriteBool(G, "Minimize", GateShareGlobals.g_boMinimize ? (byte)1 : (byte)0);                            // 原 :2884

        IniFile.WriteInteger(G, "CheckM2ServerTimeOut", unchecked((int)GateShareGlobals.g_dwCheckServerTimeOutTime));   // 原 :2886
        IniFile.WriteInteger(G, "ClientSendBlockSize", clientSendBlockSizeControlValue);                                // 原 :2887（★ C14）
        IniFile.WriteInteger(G, "MaxPreallocatedMemorySize", preAllocatedCountControlValue);                            // 原 :2888（★ C14）
        IniFile.WriteInteger(G, "ClientAccumulateMaxSize", GateShareSeam.g_dwClientAccumulateMaxSize);                  // 原 :2889

        IniFile.WriteInteger(G, "DBPort", GateShareGlobals.g_wdDBPort);                                                 // 原 :2891

        IniFile.WriteBool(G, "LogoutNoResendAntiplugStream", GateShareSeam.g_boLogoutNoResendAntiplugStream ? (byte)1 : (byte)0);   // 原 :2893
        IniFile.WriteBool(G, "AntiplugAllLog", GateShareSeam.g_boAntiplugAllLog ? (byte)1 : (byte)0);                    // 原 :2894（★ C13：写入型设置）

        IniFile.WriteInteger(G, "RecvAntiPlugHeartbeatTimeOutTime", GateShareGlobals.g_nRecvAntiPlugHeartbeatTimeOutTime);   // 原 :2896
        IniFile.WriteInteger(G, "AntiPlugStreamSendSpeed", GateShareGlobals.g_nAntiPlugStreamSendSpeed);                 // 原 :2897
        IniFile.WriteInteger(G, "AntiPlugStreamSendBlockSize", antiPlugStreamSendBlockSizeItemIndex);                    // 原 :2898（★ C14）

        IniFile.WriteBool(G, "OpenVerifyCode", GateShareGlobals.g_boOpenVerifyCode ? (byte)1 : (byte)0);                 // 原 :2900
        IniFile.WriteInteger(G, "VerifyCodeErrCount", GateShareGlobals.g_nVerifyCodeErrCount);                          // 原 :2901
        IniFile.WriteInteger(G, "VerifyCodeRefreshCount", GateShareGlobals.g_nVerifyCodeRefreshCount);                  // 原 :2902
        IniFile.WriteInteger(G, "VerifyCodeWaitTime", GateShareGlobals.g_nVerifyCodeWaitTime);                          // 原 :2903
        IniFile.WriteInteger(G, "VerifyCodeInterval1", unchecked((int)GateShareGlobals.g_dwVerifyCodeInterval1));        // 原 :2904
        IniFile.WriteInteger(G, "VerifyCodeInterval2", unchecked((int)GateShareGlobals.g_dwVerifyCodeInterval2));        // 原 :2905
        IniFile.WriteInteger(G, "VerifySuccessAddInterval", unchecked((int)GateShareGlobals.g_dwVerifySuccessAddInterval));   // 原 :2906
        IniFile.WriteBool(G, "VerifyFailTriggerScript", GateShareGlobals.g_boVerifyFailTriggerScript ? (byte)1 : (byte)0);   // 原 :2907
        IniFile.WriteBool(G, "VerifyFailLoginVerify", GateShareGlobals.g_boVerifyFailLoginVerify ? (byte)1 : (byte)0);   // 原 :2908
        IniFile.WriteBool(G, "VerifyCodeExcludeMap", GateShareGlobals.g_boVerifyCodeExcludeMap ? (byte)1 : (byte)0);     // 原 :2909

        IniFile.WriteBool(G, "AutoLoadNoVerifyChrList", GateShareGlobals.g_boAutoLoadNoVerifyChrList ? (byte)1 : (byte)0);   // 原 :2911
        IniFile.WriteString(G, "LoadNoVerifyChrListFile", GateShareGlobals.g_sLoadNoVerifyChrListFile);                  // 原 :2912
        IniFile.WriteInteger(G, "AutoLoadNoVerifyChrListInterval", GateShareGlobals.g_nAutoLoadNoVerifyChrListInterval);  // 原 :2913

        IniFile.WriteBool(G, "OneMACLimitePlayer", GateShareGlobals.g_boOneMACLimitePlayer ? (byte)1 : (byte)0);         // 原 :2915
        IniFile.WriteInteger(G, "OneMACLimitePlayerCount", GateShareGlobals.g_nOneMACLimitePlayerCount);                 // 原 :2916

        IniFile.WriteInteger(G, "ClientLogoutDelay", GateShareSeam.g_nClientLogoutDelay);                               // 原 :2918
        IniFile.WriteInteger(G, "ClientCloseDelay", GateShareSeam.g_nClientCloseDelay);                                 // 原 :2919

        IniFile.WriteBool(G, "DelayCloseDisableMove", GateShareSeam.g_boDelayCloseDisableMove ? (byte)1 : (byte)0);      // 原 :2921
        IniFile.WriteBool(G, "DelayCloseDisableSpell", GateShareSeam.g_boDelayCloseDisableSpell ? (byte)1 : (byte)0);    // 原 :2922
        IniFile.WriteBool(G, "DelayCloseDisableAttack", GateShareSeam.g_boDelayCloseDisableAttack ? (byte)1 : (byte)0);  // 原 :2923
        IniFile.WriteBool(G, "DelayCloseDisableUseItem", GateShareSeam.g_boDelayCloseDisableUseItem ? (byte)1 : (byte)0);   // 原 :2924

        IniFile.WriteBool(G, "ShowBreakClientLogoutHint", GateShareSeam.g_boBreakClientLogoutHint ? (byte)1 : (byte)0);  // 原 :2926
        IniFile.WriteString(G, "BreakClientLogoutHint", GateShareSeam.g_sBreakClientLogoutHint);                        // 原 :2927
        IniFile.WriteBool(G, "ShowBreakClientCloseHint", GateShareSeam.g_boBreakClientCloseHint ? (byte)1 : (byte)0);    // 原 :2928
        IniFile.WriteString(G, "BreakClientCloseHint", GateShareSeam.g_sBreakClientCloseHint);                          // 原 :2929
    }
}
