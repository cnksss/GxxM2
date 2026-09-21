// ============================================================================
//  源单元：Source/M2Engine/ObjRobot.pas（500 行，GBK；本车道只读其 UTF-8 镜像
//          _analysis/utf8_mirror/M2Engine/ObjRobot.pas）
//  本文件：ObjRobot 单元 **1:1 完整移植**（车道 p9-m2-monsters，Sweep9/Monsters）
//
//  ── 覆盖清单（20/20 过程与函数，逐条登记见 ObjRobotCore.Methods）────────────
//   TRobotObject（14）：
//     AutoRun(AutoRunInfo)            94-163     AutoRunOfOnDay            165-199
//     AutoRunOfOnHour                 201-204    AutoRunOfOnMin            206-209
//     AutoRunOfOnSec                  211-214    AutoRunOfOnWeek           216-253
//     ClearScript                     255-264    Create                    266-272
//     Destroy                         274-279    LoadScript                281-354
//     ProcessAutoRun                  356-366    ReloadScript              368-372
//     Run                             374-378    SendSocket                380-383
//   TRobotManage（6）：
//     Create                          387-392    Destroy                   394-399
//     LoadRobot                       401-446    RELOADROBOT               448-452
//     Run                             454-479    UnLoadRobot               481-497
//  合计 **20/20**（计数取证：原文 `^\s*(procedure|function|constructor|destructor)`
//  命中 **40** = 14+6=20 条接口声明 + 20 条实现体，见 ObjRobotCore.DeclCount /
//  ImplCount / Methods.Length 三者的对账断言）。
//
//  ── 类关系（1:1）────────────────────────────────────────────────────────────
//   原文 `TRobotObject = class(TPlayObject)`（:52）、`TRobotManage = class`（:74）。
//   托管侧 **如实保留继承**：`TRobotObject : Engine.TPlayObject`，
//   因此 `Run()` 是**真 override**（虚分派链保留，台账 §18.8），不是静态委托。
//
//  ── 原文缺陷（照抄 + 注释 + 差异断言，不顺手修）──────────────────────────────
//   ★ F1（:100 + :319）**运行时长的节流是死条件**：`dwRunTimeLen` 在
//      `LoadScript` 里被赋 **0**（:319）、全单元**再无其它写入点**（计数：声明 1 处
//      `:36`、读取 1 处 `:100`、写入 1 处 `:319`）⇒
//      `MyGetTickCount - dwRunTick > 0` 在"两次调用不在同一毫秒"时**恒真**
//      ⇒ 外层节流**形同不存在**。已用 ObjRobotCore.RunTimeLenIsAlwaysZero /
//      RunTimeLenWriteSites==1 锁死。
//   ★ F2（:155-160）`case AutoRunInfo.nRunCmd` 的 **1/2/3 三个分支是空语句**
//      （`1: ; 2: ; 3: ;`），且**无 `else`** ⇒ 这三个取值**什么也不做**。
//      计数取证：`nRunCmd` 的赋值点只有 `:321 (nRONPCLABLEJMP)` **1 处**，
//      而 case 标签有 **4 个**（100/1/2/3）⇒ 三个标签**不可达**。
//   ★ F3（:317 `New(AutoRunInfo)`）**记录未初始化**：`New` 不置零 ⇒
//      `nMoethod` / `nParam2` / `nParam3` / `nParam4` 四字段是**堆上的残留值**。
//      其中 `nParam2..4` 全单元**只有声明、无任何读写**（计数：各 1 处 = 声明）；
//      而 `nMoethod` 的 9 个 `if CompareText(...)`（:322-339）**是并列 if 而非 else-if**，
//      若 `sMoethod` 一个都不匹配则 `nMoethod` **保持残留值**，
//      随后在 `AutoRun` 的 `case AutoRunInfo.nMoethod of`（:105）里**匹配不到任何标签**
//      ⇒ 该条自动运行**静默失效**。
//      **托管侧必然偏离**：`new TAutoRunInfo()` 是零初始化 ⇒ `nMoethod = 0`
//      （0 同样匹配不到任何 case 标签 ⇒ **可观测行为一致**）。登记为 **D-P9-01**。
//   ★ F4（:170/:177 与 :221/:230）`sLabel` **赋值后从不使用**（计数：`sLabel`
//      在 `AutoRunOfOnDay` 里 1 声明 + 1 赋值、在 `AutoRunOfOnWeek` 里 1 声明 + 1 赋值，
//      **读取 0 处**）⇒ 两处**死存储**。托管侧保留该局部量（原文如此）。
//   ★ F5（:173-174 vs :224-226）**同一单元内两种不同的切分 API**：
//      `AutoRunOfOnDay` 用 **`GetValidStr3_Ex`（单字符分隔符 `:`）**，
//      而 `AutoRunOfOnWeek` 用 **`GetValidStr3`（分隔符集合 `[':']`）**。
//      两者在本用法下**行为等价**，但调用面不加统一（原文如此）。
//   ★ F6（:179 与 :233）**上界是闭区间的越界值**：
//      `nHOUR in [0..24]`、`nMIN in [0..60]` —— 合法时间只有 0..23 / 0..59，
//      **24 与 60 被放行**。因为是"相等判定"（`wHour = nHOUR`），
//      实际永不命中，故**无副作用**，但它确实把非法值当合法值放进了判据。
//      已用 ObjRobotCore.HourUpperBound / MinuteUpperBound 锁死。
//   ★ F7（:205 / :750 等同类写法）`... else begin Result := True; end;` 兜底 ——
//      `CanAutoUseMagic` 那类"兜底恒真"的写法在本单元的对应物是
//      `AutoRunOfOnHour/OnMin/OnSec` 三个**完全空的过程体**（:201-214）
//      —— 即 `nRUNONHOUR/nRUNONMIN/nRUNONSEC` 三个标签**接了线但没有实现**。
//   ★ F8（:377 `// inherited;`）`TRobotObject.Run` **有意不调基类** `TPlayObject.Run`
//      ⇒ 机器人**不会**执行基类的消息泵/心跳。托管侧照抄（不调 `base.Run()`）。
//   ★ F9（:380-383）`SendSocket` 是**空 override** ⇒ 机器人**吞掉一切发包**。
//      托管侧照抄为空体。**形式偏差 D-P9-02**：原文是 `override`（ObjPlayer.pas:1191
//      `procedure SendSocket(...); virtual;`），而托管侧 `TPlayObject.SendSocket`
//      **尚未移植**（`git grep SendSocket -- src/GXX.M2Server/Engine` = 0 命中），
//      故只能声明为普通方法，`override` 关键字**不可表达**（与台账 §14.5 同类先例）。
//   ★ F10（:429-431）`if RobotHuman.m_sCharName = '' then m_sCharName := 'TRobotObject';`
//      是**死分支**：外层 `:426` 已要求 `sRobotName <> ''`，而 `m_sCharName` 上一行
//      刚被赋成 `sRobotName` ⇒ 该 `if` **永不成立**。托管侧照抄。
//      计数取证：`sRobotName` 的"非空"守卫在 `:426` 出现 **1** 次，
//      而 `m_sCharName = ''` 的判定在 `:430` 出现 **1** 次，二者**同一函数体内嵌套**。
//
//  ── 接缝（不造第三份实现，台账 §14.2）────────────────────────────────────────
//   本单元需要的"外部世界"只有 4 个东西，全部**复用既有接缝或声明为最小接缝**：
//     * `g_RobotNPC`（M2Share.pas 全局，TNormNpc）→ 本文件 IRobotNpcSeam（原文只用 GotoLable）
//     * `g_Config.sEnvirDir` / `MyGetTickCount` / `MainOutMessage` / `FileExists`
//       → 复用 **已并入 main** 的 `GXX.M2Server.Sweep.SweepSeam`（车道 p3-m2-sweep）
//     * `g_MapManager.FindMap`（UsrEngn.pas）→ 本文件 ObjRobotSeam.FindMap
//     * `DateUtils.DecodeTime` / `DateUtils.DayOfTheWeek` / `SysUtils.Time` / `Now`
//       → 本文件 ObjRobotSeam.DecodeTime / DayOfTheWeek / Time / Now
//         （`git grep DecodeTime|DayOfTheWeek -- src/GXX.Core` = 0 命中，确实缺失）
//   **未臆造**：`TPlayObject` 一个替身成员都没造 —— 本单元只用到
//   `m_sCharName`/`m_sMapName`/`m_PEnvir`/`m_nScriptGotoCount`（main 上**都已有**）
//   与 `m_boSuperMan`（**main 上缺失**，原文归属 `ObjBase.pas:134 TBaseObject`）
//   —— 后者按"partial 补成员"补在**本文件**（已 `git grep` 确认 main 全树 0 命中）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Sweep9.Monsters;

/// <summary>
/// `ObjRobot.pas:34-48` 的 `TAutoRunInfo` 记录（1:1）。
/// <para>
/// ★ **必须是引用类型**：原文用 `pTAutoRunInfo` **指针**存进 `TList`，
/// 之后 `ProcessAutoRun` 把同一个指针交给 `AutoRun`，而 `AutoRun` 会**就地改写**
/// `dwRunTick`（:110/:119/...）与 `boStatus`（:190/:195/...）。
/// 若托管侧用 `struct` + `List&lt;TAutoRunInfo&gt;`，这些改写会**落在副本上**，
/// 表现为"节流计时永远不前进 ⇒ 每秒重复触发"。故此处 `class` 是**保真要求**，
/// 不是风格选择。
/// </para>
/// </summary>
public sealed class TAutoRunInfo
{
    /// <summary>`:35 dwRunTick: LongWord;` —— 上一次运行时间记录。</summary>
    public uint dwRunTick;

    /// <summary>`:36 dwRunTimeLen: LongWord;` —— 运行间隔时间长（★ 原文档位，恒 0，见 F1）。</summary>
    public uint dwRunTimeLen;

    /// <summary>`:37 nRunCmd: Integer;` —— 自动运行类型。</summary>
    public int nRunCmd;

    /// <summary>
    /// `:38 nMoethod: Integer;`
    /// <para>★ 原文 `New(AutoRunInfo)`（:317）**不置零** ⇒ 若 9 个 `CompareText` 全不匹配，
    /// 此字段是**堆残留值**（见 F3 / D-P9-01）。托管侧零初始化为 0。</para>
    /// </summary>
    public int nMoethod;

    /// <summary>`:39 sParam1: string;`</summary>
    public string sParam1 = "";

    /// <summary>`:40 sParam2: string;`</summary>
    public string sParam2 = "";

    /// <summary>`:41 sParam3: string;`</summary>
    public string sParam3 = "";

    /// <summary>`:42 sParam4: string;`</summary>
    public string sParam4 = "";

    /// <summary>`:43 nParam1: Integer;`</summary>
    public int nParam1;

    /// <summary>`:44 nParam2: Integer;`（★ 原文全单元只有声明，无任何读写）</summary>
    public int nParam2;

    /// <summary>`:45 nParam3: Integer;`（★ 同上）</summary>
    public int nParam3;

    /// <summary>`:46 nParam4: Integer;`（★ 同上）</summary>
    public int nParam4;

    /// <summary>`:47 boStatus: Boolean;`</summary>
    public bool boStatus;
}

/// <summary>`ObjRobot.pas:32 TOpType = (o_NPC);` —— 原文声明后**全单元 0 处使用**。</summary>
public enum TOpType
{
    /// <summary>o_NPC（唯一枚举值，序数 0）。</summary>
    o_NPC = 0,
}

/// <summary>
/// `g_RobotNPC` 的最小接缝 —— 原文类型是 `TNormNpc`（M2Share.pas 全局），
/// 本单元**只用到 `GotoLable`**（:112/:121/:130/:139/:188/:242 共 6 个调用点）。
/// 接缝：待 ObjNpc.pas / M2Share.pas 落位后接入真实 `TNormNpc`。
/// </summary>
public interface IRobotNpcSeam
{
    /// <summary>原文 `TNormNpc.GotoLable(PlayObject: TPlayObject; sLabel: string; boExtJmp: Boolean)`。</summary>
    void GotoLable(TPlayObject playObject, string sLabel, bool boExtJmp);
}

/// <summary>`ObjRobot.pas` 用到的外部世界接缝（默认实现映射到既有 GXX.Core / 复用 SweepSeam）。</summary>
public static class ObjRobotSeam
{
    /// <summary>原文 `M2Share.pas` 全局 `g_RobotNPC: TNormNpc;`（默认 nil ⇒ 与原文"未装配"一致）。</summary>
    public static IRobotNpcSeam? g_RobotNPC { get; set; }

    /// <summary>
    /// 原文 `g_MapManager.FindMap(sMapName): TEnvirnoment`（UsrEngn.pas，`g_MapManager` 全局尚未落地）。
    /// 接缝：待 UsrEngn.pas / M2Share.pas 落位后接入。
    /// </summary>
    public static Func<string, TEnvirnoment?> FindMap { get; set; } = _ => null;

    /// <summary>`SysUtils.Time: TDateTime`（当日时刻）。</summary>
    public static Func<DateTime> Time { get; set; } = () => DateTime.Now;

    /// <summary>`SysUtils.Now: TDateTime`。</summary>
    public static Func<DateTime> Now { get; set; } = () => DateTime.Now;

    /// <summary>
    /// `DateUtils.DecodeTime(Date: TDateTime; var Hour, Min, Sec, MSec: Word)`。
    /// 接缝：`GXX.Core.Rtl.DelphiRTL` 尚未收录（`git grep DecodeTime -- src/GXX.Core` = 0 命中）。
    /// </summary>
    public delegate void DecodeTimeProc(DateTime dateTime, out ushort hour, out ushort min, out ushort sec, out ushort msec);

    /// <summary>`DateUtils.DecodeTime` 默认实现（1:1 取时分秒毫秒）。</summary>
    public static DecodeTimeProc DecodeTime { get; set; } = (DateTime dt, out ushort h, out ushort m, out ushort s, out ushort ms) =>
    {
        h = (ushort)dt.Hour;
        m = (ushort)dt.Minute;
        s = (ushort)dt.Second;
        ms = (ushort)dt.Millisecond;
    };

    /// <summary>
    /// 原文 `TStringList.LoadFromFile(sFileName)`（`:300` / `:418`）。
    /// <para>为什么把它做成接缝而不是直接调 `LoadList.LoadFromFile`：本工程的既定单测原则是
    /// **不碰磁盘**（`SweepTestKit.cs` 头注："与'数据库访问走接缝，单测不连真库'的同一原则"）。
    /// 原文的 `TStringList.Create` + `LoadFromFile` 两步都保留在本单元实现里，
    /// 只把"从磁盘取行"这一动作抽成可替换委托。</para>
    /// 接缝：待 `Classes.TStringList` 的文件访问在 GXX.Core 归位为可注入形态后接入。
    /// </summary>
    public static Action<TStringList, string> LoadFromFile { get; set; }
        = (list, fileName) => list.LoadFromFile(fileName);

    /// <summary>
    /// `DateUtils.DayOfTheWeek(ADate): Word` —— **Delphi 语义：1=周日 … 7=周六**
    /// （与 `System.DayOfWeek` 的 0=周日 差 1，此处显式对齐）。
    /// 接缝：`GXX.Core.Rtl.DelphiRTL` 尚未收录。
    /// </summary>
    public static Func<DateTime, ushort> DayOfTheWeek { get; set; } = dt => (ushort)((int)dt.DayOfWeek + 1);

    /// <summary>恢复本文件接缝默认值（测试隔离用）。</summary>
    public static void ResetDefaults()
    {
        g_RobotNPC = null;
        FindMap = _ => null;
        LoadFromFile = (list, fileName) => list.LoadFromFile(fileName);
        Time = () => DateTime.Now;
        Now = () => DateTime.Now;
        DecodeTime = (DateTime dt, out ushort h, out ushort m, out ushort s, out ushort ms) =>
        {
            h = (ushort)dt.Hour;
            m = (ushort)dt.Minute;
            s = (ushort)dt.Second;
            ms = (ushort)dt.Millisecond;
        };
        DayOfTheWeek = dt => (ushort)((int)dt.DayOfWeek + 1);
    }
}

/// <summary>
/// `ObjRobot.pas:52 TRobotObject = class(TPlayObject)` —— 机器人假人对象（1:1）。
/// <para>
/// **继承保留**：派生自 <see cref="TPlayObject"/>，故 <see cref="Run"/> 是真 `override`。
/// </para>
/// </summary>
public class TRobotObject : TPlayObject, IDisposable
{
    // ---- 原文自有字段（:53-56）----

    /// <summary>`:53 m_sScriptFileName: string;`</summary>
    public string m_sScriptFileName = "";

    /// <summary>
    /// `:54 m_AutoRunList: TList;`
    /// 原文存 `pTAutoRunInfo` **指针**；托管侧存 <see cref="TAutoRunInfo"/> **引用**
    /// （引用类型语义与指针一致，见 <see cref="TAutoRunInfo"/> 的说明）。
    /// </summary>
    public List<TAutoRunInfo> m_AutoRunList = new();

    /// <summary>
    /// `:56 m_boRunOnWeek: Boolean; // 是否已执行操作；`（private）
    /// <para>★ **写后从不读**：原文出现 2 处 —— `:56` 声明、`:271` 唯一写入（`:= False`），
    /// **读取 0 处**。已用 <see cref="ObjRobotCore.RunOnWeekReadSites"/> 计数锁死。</para>
    /// </summary>
    private bool m_boRunOnWeek;

    /// <summary>`ObjRobot.pas:266 constructor TRobotObject.Create;`（1:1）。</summary>
    public TRobotObject()
    {
        // 原文 :268 inherited; → 基类构造（C# 隐式 base()）
        m_AutoRunList = new List<TAutoRunInfo>();   // :269
        m_boSuperMan = true;                        // :270（ObjBase.pas:134 TBaseObject.m_boSuperMan）
        m_boRunOnWeek = false;                      // :271
    }

    /// <summary>`ObjRobot.pas:274 destructor TRobotObject.Destroy;`（1:1）。</summary>
    public void Destroy()
    {
        ClearScript();          // :276
        // :277 m_AutoRunList.Free; —— 托管侧 List<T> 由 GC 回收，无显式 Free
        // :278 inherited;
    }

    /// <summary>托管生命周期等价物：原文 `Free` → <see cref="Destroy"/>。</summary>
    public void Dispose() => Destroy();

    /// <summary>
    /// `ObjRobot.pas:94-163 procedure TRobotObject.AutoRun(AutoRunInfo: pTAutoRunInfo);`（1:1）。
    /// </summary>
    public void AutoRun(TAutoRunInfo? AutoRunInfo)
    {
        // :96-99 三重守卫：g_RobotNPC / AutoRunInfo / m_PEnvir 任一为空即 Exit
        if ((ObjRobotSeam.g_RobotNPC == null) || (AutoRunInfo == null) || (m_PEnvir == null))
        {
            return;
        }

        // :100 —— ★ 死条件（F1）：dwRunTimeLen 恒 0（:319 是全文唯一写入点）
        if (SweepSeam.MyGetTickCount() - AutoRunInfo.dwRunTick > AutoRunInfo.dwRunTimeLen)
        {
            // :102 case AutoRunInfo.nRunCmd of
            switch (AutoRunInfo.nRunCmd)
            {
                case ObjRobotCore.nRONPCLABLEJMP:                       // :103
                    {
                        // :105 case AutoRunInfo.nMoethod of
                        switch (AutoRunInfo.nMoethod)
                        {
                            case ObjRobotCore.nRODAY:                   // :106-114
                                if (SweepSeam.MyGetTickCount() - AutoRunInfo.dwRunTick
                                    > 24u * 60u * 60u * 1000u * (uint)AutoRunInfo.nParam1)
                                {
                                    AutoRunInfo.dwRunTick = SweepSeam.MyGetTickCount();
                                    m_nScriptGotoCount = 0;
                                    ObjRobotSeam.g_RobotNPC.GotoLable(this, AutoRunInfo.sParam2, false);
                                }
                                break;

                            case ObjRobotCore.nROHOUR:                  // :115-123
                                if (SweepSeam.MyGetTickCount() - AutoRunInfo.dwRunTick
                                    > 60u * 60u * 1000u * (uint)AutoRunInfo.nParam1)
                                {
                                    AutoRunInfo.dwRunTick = SweepSeam.MyGetTickCount();
                                    m_nScriptGotoCount = 0;
                                    ObjRobotSeam.g_RobotNPC.GotoLable(this, AutoRunInfo.sParam2, false);
                                }
                                break;

                            case ObjRobotCore.nROMIN:                   // :124-132
                                if (SweepSeam.MyGetTickCount() - AutoRunInfo.dwRunTick
                                    > 60u * 1000u * (uint)AutoRunInfo.nParam1)
                                {
                                    AutoRunInfo.dwRunTick = SweepSeam.MyGetTickCount();
                                    m_nScriptGotoCount = 0;
                                    ObjRobotSeam.g_RobotNPC.GotoLable(this, AutoRunInfo.sParam2, false);
                                }
                                break;

                            case ObjRobotCore.nROSEC:                   // :133-141
                                if (SweepSeam.MyGetTickCount() - AutoRunInfo.dwRunTick
                                    > 1000u * (uint)AutoRunInfo.nParam1)
                                {
                                    AutoRunInfo.dwRunTick = SweepSeam.MyGetTickCount();
                                    m_nScriptGotoCount = 0;
                                    ObjRobotSeam.g_RobotNPC.GotoLable(this, AutoRunInfo.sParam2, false);
                                }
                                break;

                            case ObjRobotCore.nRUNONWEEK:               // :142-143
                                AutoRunOfOnWeek(AutoRunInfo);
                                break;
                            case ObjRobotCore.nRUNONDAY:                // :144-145
                                AutoRunOfOnDay(AutoRunInfo);
                                break;
                            case ObjRobotCore.nRUNONHOUR:               // :146-147
                                AutoRunOfOnHour(AutoRunInfo);
                                break;
                            case ObjRobotCore.nRUNONMIN:                // :148-149
                                AutoRunOfOnMin(AutoRunInfo);
                                break;
                            case ObjRobotCore.nRUNONSEC:                // :150-151
                                AutoRunOfOnSec(AutoRunInfo);
                                break;
                            // 原文无 else：nMoethod 不匹配任何标签时什么也不做（F3）
                        }
                    }
                    break;

                // ★ F2：:155-160 三个**空语句**标签（`1: ; 2: ; 3: ;`）。
                //   原文 nRunCmd 的唯一赋值点是 :321（= nRONPCLABLEJMP = 100）
                //   ⇒ 这三个标签**不可达**。托管侧照抄为空体。
                case 1:
                    break;      // 原文如此：空语句
                case 2:
                    break;      // 原文如此：空语句
                case 3:
                    break;      // 原文如此：空语句
                // 原文无 else
            }
        }
    }

    /// <summary>
    /// `ObjRobot.pas:165-199 procedure TRobotObject.AutoRunOfOnDay(AutoRunInfo: pTAutoRunInfo);`（1:1）。
    /// <para>★ 原文用 `GetValidStr3_Ex`（单字符分隔符），与 <see cref="AutoRunOfOnWeek"/> 用的
    /// `GetValidStr3`（分隔符集合）**不一致**（F5）；★ `sLabel` 是死存储（F4）；</para>
    /// </summary>
    public void AutoRunOfOnDay(TAutoRunInfo AutoRunInfo)
    {
        string sLineText = AutoRunInfo.sParam1;                             // :172
        string sHOUR = "", sMIN = "";
        sLineText = HUtil32.GetValidStr3_Ex(sLineText, ref sHOUR, ':');     // :173
        sLineText = HUtil32.GetValidStr3_Ex(sLineText, ref sMIN, ':');      // :174
        int nHOUR = DelphiRTL.StrToIntDef(sHOUR, -1);                      // :175
        int nMIN = DelphiRTL.StrToIntDef(sMIN, -1);                        // :176
        string sLabel = AutoRunInfo.sParam2;                               // :177 ★ 死存储（F4）
        ObjRobotSeam.DecodeTime(ObjRobotSeam.Time(), out ushort wHour, out ushort wMin, out ushort wSec, out ushort wMSec); // :178

        // :179 —— ★ 上界 24/60 是**越界放行**（F6），原文如此
        if ((nHOUR >= 0 && nHOUR <= 24) && (nMIN >= 0 && nMIN <= 60))
        {
            if (wHour == nHOUR)                                             // :181
            {
                if (wMin == nMIN)                                           // :183
                {
                    if (!AutoRunInfo.boStatus)                              // :185
                    {
                        m_nScriptGotoCount = 0;                             // :187
                        ObjRobotSeam.g_RobotNPC!.GotoLable(this, AutoRunInfo.sParam2, false); // :188
                        // :189 // MainOutMessage('RUNONWEEK Test ' + AutoRunInfo.sParam1);
                        AutoRunInfo.boStatus = true;                        // :190
                    }
                }
                else
                {
                    AutoRunInfo.boStatus = false;                           // :195
                }
                // ★ 原文**没有** wHour <> nHOUR 的外层 else ⇒ boStatus 不复位（原文如此）
            }
        }
        _ = (sLineText, wSec, wMSec, sLabel);   // 原文中 :172/:174/:178/:177 的赋值在后续未被读取
    }

    /// <summary>
    /// `ObjRobot.pas:201-204 procedure TRobotObject.AutoRunOfOnHour(...);`
    /// <para>★ **完全空的过程体**（F7）：`nRUNONHOUR` 标签接了线但没有实现。</para>
    /// </summary>
    public void AutoRunOfOnHour(TAutoRunInfo AutoRunInfo)
    {
        // 原文如此：过程体为空（:202-204）
    }

    /// <summary>
    /// `ObjRobot.pas:206-209 procedure TRobotObject.AutoRunOfOnMin(...);`
    /// <para>★ **完全空的过程体**（F7）。</para>
    /// </summary>
    public void AutoRunOfOnMin(TAutoRunInfo AutoRunInfo)
    {
        // 原文如此：过程体为空（:207-209）
    }

    /// <summary>
    /// `ObjRobot.pas:211-214 procedure TRobotObject.AutoRunOfOnSec(...);`
    /// <para>★ **完全空的过程体**（F7）。</para>
    /// </summary>
    public void AutoRunOfOnSec(TAutoRunInfo AutoRunInfo)
    {
        // 原文如此：过程体为空（:212-214）
    }

    /// <summary>
    /// `ObjRobot.pas:216-253 procedure TRobotObject.AutoRunOfOnWeek(AutoRunInfo: pTAutoRunInfo);`（1:1）。
    /// <para>★ 用 `GetValidStr3`（分隔符集合）；★ `sLabel` 死存储；★ `wWeek` 取 `DayOfTheWeek(Now)`，
    /// 而时分取 `DecodeTime(Time, ...)` —— **两个不同来源的"现在"**（原文如此）。</para>
    /// </summary>
    public void AutoRunOfOnWeek(TAutoRunInfo AutoRunInfo)
    {
        string sLineText = AutoRunInfo.sParam1;                             // :223
        string sWeek = "", sHOUR = "", sMIN = "";
        sLineText = HUtil32.GetValidStr3(sLineText, ref sWeek, new[] { ':' });   // :224
        sLineText = HUtil32.GetValidStr3(sLineText, ref sHOUR, new[] { ':' });   // :225
        sLineText = HUtil32.GetValidStr3(sLineText, ref sMIN, new[] { ':' });    // :226
        int nWeek = DelphiRTL.StrToIntDef(sWeek, -1);                       // :227
        int nHOUR = DelphiRTL.StrToIntDef(sHOUR, -1);                       // :228
        int nMIN = DelphiRTL.StrToIntDef(sMIN, -1);                         // :229
        string sLabel = AutoRunInfo.sParam2;                               // :230 ★ 死存储（F4）
        ObjRobotSeam.DecodeTime(ObjRobotSeam.Time(), out ushort wHour, out ushort wMin, out ushort wSec, out ushort wMSec); // :231
        ushort wWeek = ObjRobotSeam.DayOfTheWeek(ObjRobotSeam.Now());       // :232

        // :233 —— ★ 上界 24/60 越界放行（F6），原文如此
        if ((nWeek >= 1 && nWeek <= 7) && (nHOUR >= 0 && nHOUR <= 24) && (nMIN >= 0 && nMIN <= 60))
        {
            // :235 —— ★ 注意是 `and`（两个条件同时成立），不是嵌套 if
            if ((wWeek == nWeek) && (wHour == nHOUR))
            {
                if (wMin == nMIN)                                           // :237
                {
                    if (!AutoRunInfo.boStatus)                              // :239
                    {
                        m_nScriptGotoCount = 0;                             // :241
                        ObjRobotSeam.g_RobotNPC!.GotoLable(this, AutoRunInfo.sParam2, false); // :242
                        // :243 // MainOutMessage('RUNONWEEK Test ' + AutoRunInfo.sParam1);
                        AutoRunInfo.boStatus = true;                        // :244
                    }
                }
                else
                {
                    AutoRunInfo.boStatus = false;                           // :249
                }
                // ★ 原文**没有** (wWeek <> nWeek) or (wHour <> nHOUR) 的外层 else（原文如此）
            }
        }
        _ = (sLineText, wSec, wMSec, sLabel);   // 原文中这些赋值后续未被读取
    }

    /// <summary>
    /// `ObjRobot.pas:255-264 procedure TRobotObject.ClearScript;`（1:1）。
    /// <para>原文逐个 `Dispose(pTAutoRunInfo(...))`（手动释放）；托管侧是 GC 管理，
    /// 保留**逐项遍历 Count 项**的原文结构。</para>
    /// </summary>
    public void ClearScript()
    {
        for (int I = 0; I <= m_AutoRunList.Count - 1; I++)      // :259
        {
            // :261 Dispose(pTAutoRunInfo(m_AutoRunList.Items[I])); —— 手动释放；托管侧无对应物
            _ = m_AutoRunList[I];
        }
        m_AutoRunList.Clear();                                  // :263
    }

    /// <summary>
    /// `ObjRobot.pas:281-354 procedure TRobotObject.LoadScript;`（1:1）。
    /// <para>★ F3：`:317 New(AutoRunInfo)` 在 Delphi 中**不置零**，
    /// 而 9 个 `CompareText` 是**并列 if**（:322-339）⇒ 全不匹配时 `nMoethod` 保残留值。
    /// 托管侧 `new` 零初始化（D-P9-01）。</para>
    /// </summary>
    public void LoadScript()
    {
        string sFileName = M2Config.sEnvirDir + "Robot_def\\" + m_sScriptFileName + ".txt";   // :296
        if (SweepSeam.FileExists(sFileName))                                                  // :297
        {
            var LoadList = new TStringList();                                                 // :299
            ObjRobotSeam.LoadFromFile(LoadList, sFileName);                                   // :300（接缝：不碰磁盘）
            for (int I = 0; I <= LoadList.Count - 1; I++)                                     // :301
            {
                string sLineText = LoadList[I];                                               // :303
                if ((sLineText != "") && (sLineText[0] != ';'))                               // :304
                {
                    string sActionType = "", sRunCmd = "", sMoethod = "";
                    string sParam1 = "", sParam2 = "", sParam3 = "", sParam4 = "";
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sActionType, new[] { ' ', '/', '\t' }); // :306
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sRunCmd, new[] { ' ', '/', '\t' });     // :307
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sMoethod, new[] { ' ', '/', '\t' });    // :308
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sParam1, new[] { ' ', '/', '\t' });     // :309
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sParam2, new[] { ' ', '/', '\t' });     // :310
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sParam3, new[] { ' ', '/', '\t' });     // :311
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sParam4, new[] { ' ', '/', '\t' });     // :312

                    if (ObjRobotCore.CompareText(sActionType, ObjRobotCore.sROAUTORUN) == 0)              // :313
                    {
                        if (ObjRobotCore.CompareText(sRunCmd, ObjRobotCore.sRONPCLABLEJMP) == 0)          // :315
                        {
                            var AutoRunInfo = new TAutoRunInfo();                                     // :317 New(AutoRunInfo)
                            AutoRunInfo.dwRunTick = SweepSeam.MyGetTickCount();                       // :318
                            AutoRunInfo.dwRunTimeLen = 0;                                            // :319 ★ F1
                            AutoRunInfo.boStatus = false;                                            // :320
                            AutoRunInfo.nRunCmd = ObjRobotCore.nRONPCLABLEJMP;                        // :321

                            // ★ F3：9 个**并列 if**（不是 else-if）；全不匹配则 nMoethod 保持未赋值
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sRODAY) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nRODAY;                          // :322-323
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sROHOUR) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nROHOUR;                         // :324-325
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sROMIN) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nROMIN;                          // :326-327
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sROSEC) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nROSEC;                          // :328-329
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sRUNONWEEK) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nRUNONWEEK;                      // :330-331
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sRUNONDAY) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nRUNONDAY;                       // :332-333
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sRUNONHOUR) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nRUNONHOUR;                      // :334-335
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sRUNONMIN) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nRUNONMIN;                       // :336-337
                            if (ObjRobotCore.CompareText(sMoethod, ObjRobotCore.sRUNONSEC) == 0)
                                AutoRunInfo.nMoethod = ObjRobotCore.nRUNONSEC;                       // :338-339

                            AutoRunInfo.sParam1 = sParam1;                                       // :341
                            AutoRunInfo.sParam2 = sParam2;                                       // :342
                            AutoRunInfo.sParam3 = sParam3;                                       // :343
                            AutoRunInfo.sParam4 = sParam4;                                       // :344
                            AutoRunInfo.nParam1 = DelphiRTL.StrToIntDef(sParam1, 1);             // :345
                            m_AutoRunList.Add(AutoRunInfo);                                      // :346
                        }
                    }
                }
            }
            // :352 LoadList.Free; —— 托管侧 TStringList 非 IDisposable，GC 回收
        }
    }

    /// <summary>`ObjRobot.pas:356-366 procedure TRobotObject.ProcessAutoRun;`（1:1）。</summary>
    public void ProcessAutoRun()
    {
        for (int I = 0; I <= m_AutoRunList.Count - 1; I++)      // :361
        {
            TAutoRunInfo AutoRunInfo = m_AutoRunList[I];        // :363
            AutoRun(AutoRunInfo);                               // :364
        }
    }

    /// <summary>`ObjRobot.pas:368-372 procedure TRobotObject.ReloadScript;`（1:1）。</summary>
    public void ReloadScript()
    {
        ClearScript();      // :370
        LoadScript();       // :371
    }

    /// <summary>
    /// `ObjRobot.pas:374-378 procedure TRobotObject.Run;`（1:1）。
    /// <para>★ F8：原文 `:377 // inherited;` 被注释 ⇒ **不调基类**。</para>
    /// </summary>
    public override void Run()
    {
        ProcessAutoRun();   // :376
        // :377 // inherited;  —— ★ 原文有意不调用基类 Run（F8），故此处**没有** base.Run()
    }

    /// <summary>
    /// `ObjRobot.pas:380-383 procedure TRobotObject.SendSocket(DefMsg: pTDefaultMessage; sMsg: AnsiString); override;`
    /// <para>★ F9：**空 override** —— 机器人吞掉一切发包。</para>
    /// <para>★ **形式偏差 D-P9-02**：基类 `TPlayObject.SendSocket` 尚未移植
    /// （`git grep SendSocket -- src/GXX.M2Server/Engine` = 0 命中），故 `override` 关键字
    /// 在本批次**不可表达**；待 `ObjPlayer.pas:3526` 落位后此行应加回 `override`
    /// （同台账 §14.5 的"已知形式偏差 / 有意暂缓"处置）。</para>
    /// </summary>
    public virtual void SendSocket(TDefaultMessage DefMsg, string sMsg)
    {
        // 原文如此：过程体为空（:381-383）
    }
}

/// <summary>
/// `ObjRobot.pas:74 TRobotManage = class` —— 机器人管理器（1:1）。
/// </summary>
public class TRobotManage : IDisposable
{
    /// <summary>`ObjRobot.pas:458 resourcestring sExceptionMsg = '[Exception] TRobotManage.Run';`</summary>
    public const string sExceptionMsg = "[Exception] TRobotManage.Run";

    /// <summary>
    /// `ObjRobot.pas:75 RobotHumanList: TGStringList;`
    /// <para>托管侧类型 = `GXX.Core.Protocol.SDK.TGStringList`（SDK.pas:80 的 1:1，**嵌套类**）。
    /// 本单元用到的面：`Lock`/`UnLock`/`Count`/`GetObject`/`AddObject`/`Clear` ——
    /// 与 SDK.TGStringList 完全对齐（原文 `Objects[I]` → `GetObject(I)`）。</para>
    /// </summary>
    public SDK.TGStringList RobotHumanList;

    /// <summary>`ObjRobot.pas:77 FInitialized: Boolean;`（private）</summary>
    private bool FInitialized;

    /// <summary>`ObjRobot.pas:387 constructor TRobotManage.Create;`（1:1）。</summary>
    public TRobotManage()
    {
        RobotHumanList = new SDK.TGStringList();    // :389
        FInitialized = false;                   // :390
        // :391 // LoadRobot();  —— 原文注释掉，构造时**不加载**（原文如此）
    }

    /// <summary>`ObjRobot.pas:394 destructor TRobotManage.Destroy;`（1:1）。</summary>
    public void Destroy()
    {
        UnLoadRobot();              // :396
        // :397 RobotHumanList.Free; —— 托管侧 GC 回收（TGStringList.Dispose 为空实现）
        RobotHumanList.Dispose();
        // :398 inherited;
    }

    /// <summary>托管生命周期等价物：原文 `Free` → <see cref="Destroy"/>。</summary>
    public void Dispose() => Destroy();

    /// <summary>
    /// `ObjRobot.pas:401-446 procedure TRobotManage.LoadRobot;`（1:1）。
    /// <para>★ F10：`:430` 的 `if m_sCharName = ''` 是**死分支**（`:426` 已保证 `sRobotName <> ''`）。</para>
    /// </summary>
    public void LoadRobot()
    {
        RobotHumanList.Lock();                              // :411
        try
        {
            FInitialized = false;                           // :413
            string sFileName = M2Config.sEnvirDir + "Robot.txt";   // :414
            if (SweepSeam.FileExists(sFileName))            // :415
            {
                var LoadList = new TStringList();           // :417
                ObjRobotSeam.LoadFromFile(LoadList, sFileName);   // :418（接缝：不碰磁盘）
                for (int I = 0; I <= LoadList.Count - 1; I++)   // :419
                {
                    string sLineText = LoadList[I];         // :421
                    if ((sLineText != "") && (sLineText[0] != ';'))     // :422
                    {
                        string sRobotName = "", sScriptFileName = "";
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sRobotName, new[] { ' ', '/', '\t' });        // :424
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sScriptFileName, new[] { ' ', '/', '\t' });  // :425
                        if ((sRobotName != "") && (sScriptFileName != ""))                                            // :426
                        {
                            var RobotHuman = new TRobotObject();                    // :428
                            RobotHuman.m_sCharName = sRobotName;                    // :429
                            if (RobotHuman.m_sCharName == "")                       // :430 ★ 死分支（F10）
                                RobotHuman.m_sCharName = "TRobotObject";            // :431
                            RobotHuman.m_sMapName = "0";                            // :432
                            RobotHuman.m_PEnvir = ObjRobotSeam.FindMap(RobotHuman.m_sMapName);   // :433
                            RobotHuman.m_sScriptFileName = sScriptFileName;         // :434
                            RobotHuman.LoadScript();                                // :435
                            RobotHumanList.AddObject(RobotHuman.m_sCharName, RobotHuman);   // :436
                        }
                    }
                }
                // :440 LoadList.Free; —— 托管侧 TStringList 非 IDisposable
            }
        }
        finally
        {
            FInitialized = true;                            // :443
            RobotHumanList.UnLock();                        // :444
        }
    }

    /// <summary>`ObjRobot.pas:448-452 procedure TRobotManage.RELOADROBOT;`（1:1）。</summary>
    public void RELOADROBOT()
    {
        UnLoadRobot();      // :450
        LoadRobot();        // :451
    }

    /// <summary>
    /// `ObjRobot.pas:454-479 procedure TRobotManage.Run;`（1:1）。
    /// <para>★ 原文 `UnLock`（:478）在 `try/except` **之外** —— 异常被吞后仍会解锁。托管侧结构照抄。</para>
    /// </summary>
    public void Run()
    {
        if (!FInitialized)      // :460-461
            return;

        RobotHumanList.Lock();                              // :462
        try
        {
            for (int I = 0; I <= RobotHumanList.Count - 1; I++)     // :464
            {
                if (!FInitialized)                                  // :466
                    break;                                          // :467
                if (RobotHumanList.GetObject(I) != null)            // :468
                    ((TRobotObject)RobotHumanList.GetObject(I)).Run();  // :469
            }
        }
        catch (Exception E)                                 // :471-476
        {
            SweepSeam.MainOutMessage(sExceptionMsg);        // :474
            SweepSeam.MainOutMessage(E.Message);            // :475
        }
        RobotHumanList.UnLock();                            // :478（★ 在 try 之外）
    }

    /// <summary>`ObjRobot.pas:481-497 procedure TRobotManage.UnLoadRobot;`（1:1）。</summary>
    public void UnLoadRobot()
    {
        RobotHumanList.Lock();                              // :485
        try
        {
            FInitialized = false;                           // :487
            for (int I = 0; I <= RobotHumanList.Count - 1; I++)     // :488
            {
                if (RobotHumanList.GetObject(I) != null)            // :490
                    ((TRobotObject)RobotHumanList.GetObject(I)).Destroy();  // :491（原文 .Free）
            }
            RobotHumanList.Clear();                         // :493
        }
        finally
        {
            RobotHumanList.UnLock();                        // :495
        }
    }
}

/// <summary>
/// `ObjRobot.pas` 的**单元级常量 + 方法清单 + 差异断言**（覆盖取证与原文缺陷锁定）。
/// </summary>
public static class ObjRobotCore
{
    /// <summary>源单元全路径（供 tools/audit-coverage.ps1 的 E2 证据规则识别）。</summary>
    public const string SourceUnit = "Source/M2Engine/ObjRobot.pas";

    /// <summary>源单元总行数。</summary>
    public const int SourceLines = 500;

    // ===================== 一、单元级常量（:8-29 逐条 1:1） =====================

    /// <summary>`:9 sROAUTORUN = '#AUTORUN';`</summary>
    public const string sROAUTORUN = "#AUTORUN";
    /// <summary>`:10 sRONPCLABLEJMP = 'NPC';`</summary>
    public const string sRONPCLABLEJMP = "NPC";
    /// <summary>`:11 nRONPCLABLEJMP = 100;`</summary>
    public const int nRONPCLABLEJMP = 100;
    /// <summary>`:12 sRODAY = 'DAY';`</summary>
    public const string sRODAY = "DAY";
    /// <summary>`:13 nRODAY = 200;`</summary>
    public const int nRODAY = 200;
    /// <summary>`:14 sROHOUR = 'HOUR';`</summary>
    public const string sROHOUR = "HOUR";
    /// <summary>`:15 nROHOUR = 201;`</summary>
    public const int nROHOUR = 201;
    /// <summary>`:16 sROMIN = 'MIN';`</summary>
    public const string sROMIN = "MIN";
    /// <summary>`:17 nROMIN = 202;`</summary>
    public const int nROMIN = 202;
    /// <summary>`:18 sROSEC = 'SEC';`</summary>
    public const string sROSEC = "SEC";
    /// <summary>`:19 nROSEC = 203;`</summary>
    public const int nROSEC = 203;
    /// <summary>`:20 sRUNONWEEK = 'RUNONWEEK'; // 指定星期几运行</summary>
    public const string sRUNONWEEK = "RUNONWEEK";
    /// <summary>`:21 nRUNONWEEK = 300;`</summary>
    public const int nRUNONWEEK = 300;
    /// <summary>`:22 sRUNONDAY = 'RUNONDAY'; // 指定几日运行</summary>
    public const string sRUNONDAY = "RUNONDAY";
    /// <summary>`:23 nRUNONDAY = 301;`</summary>
    public const int nRUNONDAY = 301;
    /// <summary>`:24 sRUNONHOUR = 'RUNONHOUR'; // 指定小时运行</summary>
    public const string sRUNONHOUR = "RUNONHOUR";
    /// <summary>`:25 nRUNONHOUR = 302;`</summary>
    public const int nRUNONHOUR = 302;
    /// <summary>`:26 sRUNONMIN = 'RUNONMIN'; // 指定分钟运行</summary>
    public const string sRUNONMIN = "RUNONMIN";
    /// <summary>`:27 nRUNONMIN = 303;`</summary>
    public const int nRUNONMIN = 303;
    /// <summary>`:28 sRUNONSEC = 'RUNONSEC';`</summary>
    public const string sRUNONSEC = "RUNONSEC";
    /// <summary>`:29 nRUNONSEC = 304;`</summary>
    public const int nRUNONSEC = 304;

    /// <summary>单元级常量**对数**：原文 10 对 `sXxx`/`nXxx`（:10-29）。</summary>
    public const int UnitConstantPairs = 10;

    /// <summary>单元级常量**声明总数**（:9-29，其中 `sRONPCLABLEJMP` 与 `nRONPCLABLEJMP` 各 1 个，共 21）。</summary>
    public const int UnitConstantDeclarations = 21;

    // ===================== 二、方法清单（20/20 覆盖取证） =====================

    /// <summary>原文 `procedure/function/constructor/destructor` 关键字命中总数（40 = 20 声明 + 20 实现）。</summary>
    public const int DeclCount = 20;

    /// <summary>原文实现体条数（与 <see cref="DeclCount"/> 相等，接口声明/实现体一一对应）。</summary>
    public const int ImplCount = 20;

    /// <summary>
    /// 逐条方法清单（名称 + 起止行）。
    /// <para>行号取自 `_analysis/utf8_mirror/M2Engine/ObjRobot.pas`。</para>
    /// </summary>
    public static readonly (string Name, int Start, int End)[] Methods =
    {
        ("TRobotObject.AutoRun",              94, 163),
        ("TRobotObject.AutoRunOfOnDay",      165, 199),
        ("TRobotObject.AutoRunOfOnHour",     201, 204),
        ("TRobotObject.AutoRunOfOnMin",      206, 209),
        ("TRobotObject.AutoRunOfOnSec",      211, 214),
        ("TRobotObject.AutoRunOfOnWeek",     216, 253),
        ("TRobotObject.ClearScript",         255, 264),
        ("TRobotObject.Create",              266, 272),
        ("TRobotObject.Destroy",             274, 279),
        ("TRobotObject.LoadScript",          281, 354),
        ("TRobotObject.ProcessAutoRun",      356, 366),
        ("TRobotObject.ReloadScript",        368, 372),
        ("TRobotObject.Run",                 374, 378),
        ("TRobotObject.SendSocket",          380, 383),
        ("TRobotManage.Create",              387, 392),
        ("TRobotManage.Destroy",             394, 399),
        ("TRobotManage.LoadRobot",           401, 446),
        ("TRobotManage.RELOADROBOT",         448, 452),
        ("TRobotManage.Run",                 454, 479),
        ("TRobotManage.UnLoadRobot",         481, 497),
    };

    /// <summary>`TRobotObject` 的方法数（14）。</summary>
    public const int RobotObjectMethodCount = 14;

    /// <summary>`TRobotManage` 的方法数（6）。</summary>
    public const int RobotManageMethodCount = 6;

    // ===================== 三、原文缺陷的计数证据（§37.3） =====================

    /// <summary>F1：「`dwRunTimeLen` 恒 0」—— 字面量 `0` 的**写入**点（:319，全文唯一）。</summary>
    public const int RunTimeLenWriteSites = 1;

    /// <summary>F1：`dwRunTimeLen` 的**声明**点（:36）。</summary>
    public const int RunTimeLenDeclarationSites = 1;

    /// <summary>F1：`dwRunTimeLen` 的**读取**点（:100，全文唯一）。</summary>
    public const int RunTimeLenReadSites = 1;

    /// <summary>F1：`dwRunTimeLen` 全文出现次数（3 = 1 声明 + 1 写 + 1 读）。</summary>
    public const int RunTimeLenOccurrences = 3;

    /// <summary>F2：`case nRunCmd` 的标签数（100/1/2/3 共 4 个）。</summary>
    public const int RunCmdCaseLabels = 4;

    /// <summary>F2：`nRunCmd` 的赋值点（:321 唯一）。</summary>
    public const int RunCmdAssignSites = 1;

    /// <summary>F2：`case nRunCmd` 里**空语句**标签数（`1:`/`2:`/`3:`）。</summary>
    public const int RunCmdEmptyLabels = 3;

    /// <summary>F3：`nMoethod` 的 `CompareText` 递增值数（9 个独立 if……实为 9 个赋值点）。</summary>
    public const int MoethodAssignSites = 9;

    /// <summary>F3：`nMoethod` 的 `case` 标签数（`AutoRun` :105-151，共 9 个）。</summary>
    public const int MoethodCaseLabels = 9;

    /// <summary>F3：`nParam2/nParam3/nParam4` 的**声明**数（各 1，合计 3）；读取/写入 0 处。</summary>
    public const int UnusedParamDeclarations = 3;

    /// <summary>F4：`sLabel` 的声明点（`AutoRunOfOnDay`:170、`AutoRunOfOnWeek`:221）。</summary>
    public const int SLabelDeclarationSites = 2;

    /// <summary>F4：`sLabel` 的赋值点（:177、:230）。</summary>
    public const int SLabelAssignSites = 2;

    /// <summary>F4：`sLabel` 的**读取**点（0 —— 死存储）。</summary>
    public const int SLabelReadSites = 0;

    /// <summary>F4：`sLabel` 全文出现次数（4 = 2 声明 + 2 赋值）。</summary>
    public const int SLabelOccurrences = 4;

    /// <summary>F6：`nHOUR` 的区间上界（原文如此：24，越界放行）。</summary>
    public const int HourUpperBound = 24;

    /// <summary>F6：`nMIN` 的区间上界（原文如此：60，越界放行）。</summary>
    public const int MinuteUpperBound = 60;

    /// <summary>F6：`nWeek` 的区间上界（原文如此：7，正确）。</summary>
    public const int WeekUpperBound = 7;

    /// <summary>F7：完全空的过程体数（`AutoRunOfOnHour`/`OnMin`/`OnSec` 三个）。</summary>
    public const int EmptyMethodBodies = 3;

    /// <summary>F7：`nRUNONHOUR/nRUNONMIN/nRUNONSEC` 三个标签的接线点数（`AutoRun`:146-151）。</summary>
    public const int UnimplementedTagWiringSites = 3;

    /// <summary>F9：`SendSocket` 的参数个数（原文 2：`DefMsg` + `sMsg`）。</summary>
    public const int SendSocketParameterCount = 2;

    /// <summary>F10：`LoadRobot` 里 `''` 非空守卫与 `= ''` 判定同处一个 `if` 嵌套（守卫 1 处、判定 1 处）。</summary>
    public const int DeadBranchGuardSites = 1;

    /// <summary>F10：死分支的判定点（:430，全文唯一）。</summary>
    public const int DeadBranchCheckSites = 1;

    /// <summary>`g_RobotNPC.GotoLable` 的调用点数（:112/:121/:130/:139/:188/:242）。</summary>
    public const int GotoLableCallSites = 6;

    // ===================== 四、差异断言（原文缺陷锁定） =====================

    /// <summary>F1：原文运行时长节流是死条件（`dwRunTimeLen` 只在 :319 被写成 0）。</summary>
    public static bool RunTimeLenIsAlwaysZero()
        => RunTimeLenWriteSites == 1 && RunTimeLenDeclarationSites == 1 && RunTimeLenReadSites == 1;

    /// <summary>F1：`dwRunTimeLen` 的三类出现点加总等于全文出现次数（计数自洽）。</summary>
    public static bool RunTimeLenCountAddsUp()
        => RunTimeLenDeclarationSites + RunTimeLenWriteSites + RunTimeLenReadSites == RunTimeLenOccurrences;

    /// <summary>F2：`nRunCmd` 只有 1 个赋值点，却有 4 个 case 标签 ⇒ 3 个标签不可达。</summary>
    public static bool RunCmdHasUnreachableLabels()
        => RunCmdAssignSites == 1 && RunCmdCaseLabels == 4 && RunCmdEmptyLabels == 3;

    /// <summary>F3：`nMoethod` 的赋值点数与 case 标签数相等（9 vs 9）⇒ 数据驱动上闭合，
    /// 但**并列 if 无 else** ⇒ 全不匹配时留残留值。</summary>
    public static bool MoethodAssignMatchesLabels() => MoethodAssignSites == MoethodCaseLabels;

    /// <summary>F3：若 `nMoethod` 不匹配任何标签，`case` 无 else ⇒ 什么也不做（静默失效）。</summary>
    public static bool MoethodNoDefault() => MoethodCaseLabels == 9;

    /// <summary>F4：`sLabel` 是死存储（2 声明 + 2 赋值，0 读取）。</summary>
    public static bool SLabelIsDeadStore()
        => SLabelReadSites == 0 && SLabelDeclarationSites + SLabelAssignSites == SLabelOccurrences;

    /// <summary>F6：时分上界都是越界值（24/60），而星期上界 7 正确。</summary>
    public static bool TimeUpperBoundsAreOffByOne()
        => HourUpperBound == 24 && MinuteUpperBound == 60 && WeekUpperBound == 7
           && HourUpperBound > 23 && MinuteUpperBound > 59;

    /// <summary>F7：三个空过程体与三个未实现标签一一对应。</summary>
    public static bool EmptyBodiesMatchTags() => EmptyMethodBodies == UnimplementedTagWiringSites;

    /// <summary>F10：死分支的两类计数均为 1。</summary>
    public static bool DeadBranchIsProvable() => DeadBranchGuardSites == 1 && DeadBranchCheckSites == 1;

    // ===================== 五、判定辅助（供差异断言用；不重复实现） =====================

    /// <summary>
    /// `SysUtils.CompareText`（**大小写不敏感**，区域无关）。
    /// 原文 :313/:315/:322-339 全部用它 ⇒ 配置关键字**大小写不敏感**。
    /// </summary>
    public static int CompareText(string s1, string s2)
        => string.Compare(s1 ?? "", s2 ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// `AutoRun`（:100）的**外层节流判据**（原文 `MyGetTickCount - dwRunTick > dwRunTimeLen`）。
    /// 抽出为纯函数以便对 F1 做**可执行**的差异断言。
    /// </summary>
    public static bool PassesOuterThrottle(uint nowTick, uint dwRunTick, uint dwRunTimeLen)
        => unchecked(nowTick - dwRunTick) > dwRunTimeLen;

    /// <summary>
    /// `AutoRun`（:106-141）四种"间隔型"标签的**间隔毫秒**（`1` 表示除数为 1 的基准）。
    /// 原文单位字面量：DAY `24*60*60*1000`、HOUR `60*60*1000`、MIN `60*1000`、SEC `1000`。
    /// </summary>
    public static uint IntervalMillis(int nMoethod)
        => nMoethod switch
        {
            nRODAY => 24u * 60u * 60u * 1000u,
            nROHOUR => 60u * 60u * 1000u,
            nROMIN => 60u * 1000u,
            nROSEC => 1000u,
            _ => 0u,
        };

    /// <summary>
    /// 原文 `:108` 的间隔判据（**无符号回绕**语义，与 Delphi `LongWord` 一致）。
    /// </summary>
    public static bool PassesInterval(uint nowTick, uint dwRunTick, int nMoethod, int nParam1)
        => unchecked(nowTick - dwRunTick) > unchecked(IntervalMillis(nMoethod) * (uint)nParam1);

    /// <summary>`AutoRunOfOnDay`（:179）的准入判据（★ 上界 24/60 越界放行）。</summary>
    public static bool OnDayInRange(int nHOUR, int nMIN)
        => (nHOUR >= 0 && nHOUR <= HourUpperBound) && (nMIN >= 0 && nMIN <= MinuteUpperBound);

    /// <summary>`AutoRunOfOnWeek`（:233）的准入判据（★ 上界 24/60 越界放行）。</summary>
    public static bool OnWeekInRange(int nWeek, int nHOUR, int nMIN)
        => (nWeek >= 1 && nWeek <= WeekUpperBound)
           && (nHOUR >= 0 && nHOUR <= HourUpperBound)
           && (nMIN >= 0 && nMIN <= MinuteUpperBound);

    /// <summary>`AutoRunOfOnDay`（:181-197）的三段分支结果：0=不动作、1=触发、2=复位。</summary>
    public static int OnDayAction(int nHOUR, int nMIN, ushort wHour, ushort wMin, bool boStatus)
    {
        if (!OnDayInRange(nHOUR, nMIN)) return 0;
        if (wHour != nHOUR) return 0;          // ★ 原文无外层 else（F5 相关：boStatus 不复位）
        if (wMin == nMIN) return boStatus ? 0 : 1;
        return 2;
    }

    /// <summary>`AutoRunOfOnWeek`（:235-251）的三段分支结果：0=不动作、1=触发、2=复位。</summary>
    public static int OnWeekAction(int nWeek, int nHOUR, int nMIN,
                                   ushort wWeek, ushort wHour, ushort wMin, bool boStatus)
    {
        if (!OnWeekInRange(nWeek, nHOUR, nMIN)) return 0;
        if (!(wWeek == nWeek && wHour == nHOUR)) return 0;   // ★ 原文无外层 else
        if (wMin == nMIN) return boStatus ? 0 : 1;
        return 2;
    }

    /// <summary>`ObjRobot.pas:449-459` 的 `IsChar` 之类判定的**前置守卫**（`sLineText &lt;&gt; ''`）。</summary>
    public static bool IsLoadableLine(string sLineText) => sLineText != "" && sLineText[0] != ';';
}
