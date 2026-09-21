// ============================================================================
// 源单元：Source/M2Engine/uAliyunSendSMSThread.pas（320 行，GBK）
// 类型/全局/过程：
//   PSMSTask/TSMSTask（:9-14 record：Player/Npc: TBaseObject）
//   TAliyunSendSMSThread（:16-38）
//     Create :75-90   Destroy :92-99   Terminate :101-105   AddTask :107-125
//     Execute :127-140  GetSleeping :142-145  TriggerEvent :147-150
//     DoExecuteLoop :152-239  ClearTaskList :241-257
//   TSendSMSConfig（:41-49 record）
//   g_AliyunSendSMSThread（:52）/ g_SendSMSConfig（:53-61 typed const 初值）
//   LoadSendSMSConfig（:63 / :259-317）
//   ⇒ 方法合计 **10**（Create/Destroy/Terminate/AddTask/Execute/GetSleeping/TriggerEvent/
//     DoExecuteLoop/ClearTaskList/LoadSendSMSConfig）。
//
// 原文 uses（:6 / :68）：Windows, Classes, Forms, SysUtils, SyncObjs, ObjBase, M2Locker,
//   SendSmsRequest, acsUtils, IniFiles, acsParams；实现段 ObjPlayer, M2Share, ObjNpc, Grobal2。
//
// 依赖缺口（全树 0 命中，见 docs\并行报告-p9-m2-forms.md §0.4）：
//   · `TSendSmsRequest`（SendSmsRequest.pas）→ 接口接缝 `ISweep9FormsSendSmsRequest`。
//   · `g_AcsUtil`（acsUtils.pas 的 KeyID/KeySecret）→ `Sweep9FormsAcsUtil`。
//   · `g_RequestStr`（acsParams/acsUtils）→ `Sweep9FormsSmsGlobals.g_RequestStr`。
//   · `TEvent`（SyncObjs）→ `ISweep9FormsEvent` 接口（默认实现基于 AutoResetEvent；
//     原文 `TEvent.Create(nil, False, False, '')` = **自动重置、初始未触发**）。
//   · `UserEngine.GetPlayObject(Player)` → 实例接缝（未接线即抛，§25.2）。
//   · `g_FunctionNPC`（M2Share.pas）→ 接缝（未接线即抛）。
//   · `TPlayObject` 的 6 个成员 + `SendMsg(BaseObject, ...)` 重载在托管侧 0 命中
//     ⇒ 以 `ISweep9FormsSmsPlayer` 接缝接口承载（偏离 **D-P9-06**；**不** partial 补
//     `TPlayObject`，避免与未来 ObjPlayer 批次 CS0102）。
//   · `TBaseObject` → 托管侧基类 `GXX.M2Server.Engine.TCreature`（偏离 **D-P9-02**）。
//   · `TSafeList` → **既有** `GXX.M2Server.Sweep.TSafeList`（不新增替身）。
//
// ★ 原文行为要点（逐字保留 + 差异断言锁定）：
//   1. `Create`（:77）`inherited Create(False)` ⇒ **构造即起线程**（`Execute` 立刻跑到
//      `FEvent.WaitFor(INFINITE)` 停住）。托管侧保留该语义，但提供
//      `Sweep9FormsSmsSeams.StartThreadOnCreate`（默认 true）供无头测试关闭真实线程。
//   2. `TEvent.Create(nil, False, False, '')` ⇒ **自动重置**事件（WaitFor 返回后自动复位）。
//   3. `DoExecuteLoop` 的 `finally TriggerEvent`（:237）**只在取到任务时**执行
//      （`Player = nil` 的 `Exit` 在 `try` **之前**，:177-178）⇒ 空表时不会自旋。
//   4. `Execute`（:130-139）循环条件只在**顶部**判 `Terminated` ⇒ `Terminate` 后还会
//      再执行一轮 `DoExecuteLoop` 才退出。
//   5. `GetSleeping = not FInExecuteLoop` ⇒ **只在 `DoExecuteLoop` 执行期间**为 False。
//   6. `:191-194` 模板选择：`not m_boMobileBind` ⇒ **绑定**模板，否则 ⇒ **校验**模板
//      （名字与直觉相反，逐字保留）。
//   7. `:202` 只发 `{"code":"%s"}`（**不含角色名** —— 原文 :198 注释说明阿里不支持中文参数）。
//   8. `:209` `else if Npc <> nil` —— 只有带 NPC 的失败才输出日志/跳标签；
//      **不带 NPC 的失败被完全静默吞掉**（照抄）。
//   9. `:214-229` 一大段 `m_MerchantList` 遍历被 `{ }` 注释掉（照抄保留为注释）。
//  10. `:230-234` 用的是 **`g_FunctionNPC`**（全局功能 NPC）而非本次任务的 `Npc`
//      ⇒ 任务里传的 `Npc` 其实只用于 :209 的判空。原文如此。
//  11. `LoadSendSMSConfig`（:259-317）：先无条件 `DeleteKey('aliyun','Endpoint')` 与
//      `DeleteKey('aliyun','Topic')`（:268-269，**迁移残留清理**）；每个键都是
//      「不存在则 **写回** 默认值，存在则 **读入**」；**整数键用 `ReadInteger` 而不是
//      `ReadString`**（:299/304/309/314），与字符串键（:274/279/284/289/294）成对但不同型。
// ============================================================================

using System.Threading;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Sweep9.Forms;

// ---------------------------------------------------------------------------
// 原文记录 / 接口接缝
// ---------------------------------------------------------------------------

/// <summary>
/// 原文 `uAliyunSendSMSThread.pas:9-14`：
/// <code>
///   PSMSTask = ^TSMSTask;
///   TSMSTask = record Player: TBaseObject; Npc: TBaseObject; end;
/// </code>
/// <para>
/// 原文以 `New(SMSTask)` / `Dispose(SMSTask)` 的**指针**使用 ⇒ 托管侧用 class
/// （引用语义等价；回收交给 GC）。`TBaseObject` → <see cref="TCreature"/>（偏离 D-P9-02）。
/// </para>
/// </summary>
public sealed class TSMSTask
{
    /// <summary>原文 `Player: TBaseObject`。</summary>
    public TCreature? Player;
    /// <summary>原文 `Npc: TBaseObject`。</summary>
    public TCreature? Npc;
}

/// <summary>
/// 原文 `:41-49 TSendSMSConfig = record`（含 typed const 初值，见
/// <see cref="Sweep9FormsSmsGlobals.g_SendSMSConfig"/>）。
/// </summary>
public sealed class TSendSMSConfig
{
    /// <summary>原文 `SignName: string`（初值 `''`）。</summary>
    public string SignName = "";
    /// <summary>原文 `TemplateCodeBind: string`（初值 `''`）。</summary>
    public string TemplateCodeBind = "";
    /// <summary>原文 `TemplateCodeCheck: string`（初值 `''`）。</summary>
    public string TemplateCodeCheck = "";
    /// <summary>原文 `ResendVerifyCodeCount: Integer; // 重新发送验证码最大次数`（初值 **3**）。</summary>
    public int ResendVerifyCodeCount = 3;
    /// <summary>原文 `VerifySendInterval: Integer; // 验证码发送间隔 秒`（初值 **60**）。</summary>
    public int VerifySendInterval = 60;
    /// <summary>原文 `VerifyCodeTimeOutTime: Integer; // 验证码超时时间`（初值 **120**）。</summary>
    public int VerifyCodeTimeOutTime = 120;
    /// <summary>原文 `VerifyCodeErrorCount: Integer; // 验证码允许错误次数`（初值 **3**）。</summary>
    public int VerifyCodeErrorCount = 3;
}

/// <summary>
/// 原文 `SendSmsRequest.pas` 的 `TSendSmsRequest` 的接缝面（该单元尚未移植）。
/// </summary>
public interface ISweep9FormsSendSmsRequest
{
    /// <summary>原文 `property SignName: string`（构造后由 `Create` 从 `g_SendSMSConfig` 写入）。</summary>
    string SignName { get; set; }
    /// <summary>原文 `property TemplateCode: string`（每次发送前按 `m_boMobileBind` 选择）。</summary>
    string TemplateCode { get; set; }
    /// <summary>
    /// 原文 `function Request(MobilePhone, ParamStr: string; var ErrorCode, ErrorMsg: string): Boolean`。
    /// </summary>
    bool Request(string mobilePhone, string paramStr, out string errorCode, out string errorMsg);
}

/// <summary>
/// 原文 `acsUtils.pas` 的 `g_AcsUtil`（只取本单元用到的两个字段）。
/// </summary>
public sealed class Sweep9FormsAcsUtil
{
    /// <summary>原文 `g_AcsUtil.KeyID`。</summary>
    public string KeyID = "";
    /// <summary>原文 `g_AcsUtil.KeySecret`。</summary>
    public string KeySecret = "";
}

/// <summary>
/// 原文 `SyncObjs.TEvent` 的接缝面。
/// <para>
/// 原文用法：`TEvent.Create(nil, False, False, '')`（**自动重置、初始未触发**）、
/// `WaitFor(INFINITE)`、`SetEvent`。默认实现见 <see cref="Sweep9FormsAutoResetEvent"/>。
/// </para>
/// </summary>
public interface ISweep9FormsEvent : IDisposable
{
    /// <summary>原文 `function WaitFor(Timeout: LongWord): TWaitResult`（`INFINITE` = $FFFFFFFF）。</summary>
    uint WaitFor(uint timeout);
    /// <summary>原文 `procedure SetEvent`。</summary>
    void SetEvent();
}

/// <summary>`TEvent(nil, False, False, '')` 的默认托管实现（`AutoResetEvent`，uint 超时口径）。</summary>
public sealed class Sweep9FormsAutoResetEvent : ISweep9FormsEvent
{
    /// <summary>原文 `Windows.INFINITE`。</summary>
    public const uint INFINITE = 0xFFFFFFFF;

    private readonly AutoResetEvent _event = new(false);

    /// <inheritdoc />
    public uint WaitFor(uint timeout)
    {
        // 原文返回 TWaitResult（wrSignaled=0 / wrTimeout=1 / wrAbandoned=2 / wrError=3）；
        // 本单元丢弃返回值（:132），故这里只保留"是否等到"的语义（0=等到）。
        bool signaled = timeout == INFINITE
            ? WaitInfinite()
            : _event.WaitOne(unchecked((int)timeout));
        return signaled ? 0u : 1u;
    }

    private bool WaitInfinite()
    {
        // INFINITE 在托管侧用无限等待（-1），并**不**响应托管线程中断之外的取消。
        _event.WaitOne(Timeout.Infinite);
        return true;
    }

    /// <inheritdoc />
    public void SetEvent() => _event.Set();

    /// <inheritdoc />
    public void Dispose() => _event.Dispose();
}

/// <summary>
/// 原文 `TPlayObject` 在本单元被用到的**成员接缝**（托管侧 6 个成员 + 1 个重载 0 命中）。
/// <para>
/// 成员名与原文逐字一致（`m_boGhost` / `m_sMobileNumber` / `m_boMobileBind` /
/// `m_sMobileVerifyCode` / `m_dwMobileVerifyTick` / `m_nScriptGotoCount`），
/// `SendMsg` 是 `SendMsg(BaseObject, wIdent, ...)` 那个 **7 参重载**（发给指定对象）。
/// </para>
/// 偏离 **D-P9-06**：不 partial 补 `TPlayObject`（避免与 ObjPlayer 批次 CS0102）。
/// </summary>
public interface ISweep9FormsSmsPlayer
{
    /// <summary>原文 `TPlayObject.m_boGhost: Boolean`。</summary>
    bool m_boGhost { get; }
    /// <summary>原文 `TPlayObject.m_sMobileNumber: string`。</summary>
    string m_sMobileNumber { get; }
    /// <summary>原文 `TPlayObject.m_boMobileBind: Boolean`。</summary>
    bool m_boMobileBind { get; }
    /// <summary>原文 `TPlayObject.m_sMobileVerifyCode: string`（本单元写入生成的验证码）。</summary>
    string m_sMobileVerifyCode { get; set; }
    /// <summary>原文 `TPlayObject.m_dwMobileVerifyTick: LongWord`（写入 `MyGetTickCount`）。</summary>
    uint m_dwMobileVerifyTick { get; set; }
    /// <summary>原文 `TPlayObject.m_nScriptGotoCount: Integer`（失败路径清 0）。</summary>
    int m_nScriptGotoCount { get; set; }
    /// <summary>原文 `procedure SendMsg(BaseObject: TBaseObject; wIdent: Word; wParam, nParam1..3: Integer; sMsg: string)`。</summary>
    void SendMsg(TCreature baseObject, ushort wIdent, long wParam, long nParam1, long nParam2, long nParam3, string sMsg);
}

/// <summary>原文 `M2Share.pas` 的 `g_FunctionNPC`（功能 NPC）在本单元被用到的面。</summary>
public interface ISweep9FormsFunctionNpc
{
    /// <summary>原文 `procedure GotoLable(PlayObject: TPlayObject; sLabel: string; boExt: Boolean)`。</summary>
    void GotoLable(ISweep9FormsSmsPlayer playObject, string sLabel, bool boExt);
}

/// <summary>
/// 原文 `IniFiles.TIniFile` 在本单元被用到的面（比既有 `ISweepIniFile` 多
/// `ValueExists` 与 `DeleteKey` 两个成员 —— 本单元 :268-271 用到，故自带一份最小面；
/// 默认实现包既有 `GXX.Core.Util.TFastIniFile`，不重写 INI 解析）。
/// </summary>
public interface ISweep9FormsSmsIniFile : IDisposable
{
    /// <summary>原文 `procedure DeleteKey(const Section, Ident: string)`。</summary>
    void DeleteKey(string section, string key);
    /// <summary>原文 `function ValueExists(const Section, Ident: string): Boolean`。</summary>
    bool ValueExists(string section, string key);
    /// <summary>原文 `function ReadString(const Section, Ident, Default: string): string`。</summary>
    string ReadString(string section, string key, string defaultValue);
    /// <summary>原文 `function ReadInteger(const Section, Ident: string; Default: Longint): Longint`。</summary>
    int ReadInteger(string section, string key, int defaultValue);
    /// <summary>原文 `procedure WriteString(const Section, Ident, Value: string)`。</summary>
    void WriteString(string section, string key, string value);
    /// <summary>原文 `procedure WriteInteger(const Section, Ident: string; Value: Longint)`。</summary>
    void WriteInteger(string section, string key, int value);
}

/// <summary>`ISweep9FormsSmsIniFile` 的默认实现（薄包既有 <c>GXX.Core.Util.TFastIniFile</c>）。</summary>
public sealed class Sweep9FormsSmsIniFile : ISweep9FormsSmsIniFile
{
    private readonly GXX.Core.Util.TFastIniFile _ini;

    /// <summary>构造（原文 `TIniFile.Create(FileName)`）。</summary>
    public Sweep9FormsSmsIniFile(string fileName) => _ini = new GXX.Core.Util.TFastIniFile(fileName);

    /// <inheritdoc />
    public void DeleteKey(string section, string key) => _ini.DeleteKey(section, key);
    /// <inheritdoc />
    public bool ValueExists(string section, string key) => _ini.ValueExists(section, key);
    /// <inheritdoc />
    public string ReadString(string section, string key, string defaultValue) => _ini.ReadString(section, key, defaultValue);
    /// <inheritdoc />
    public int ReadInteger(string section, string key, int defaultValue) => _ini.ReadInteger(section, key, defaultValue);
    /// <inheritdoc />
    public void WriteString(string section, string key, string value) => _ini.WriteString(section, key, value);
    /// <inheritdoc />
    public void WriteInteger(string section, string key, int value) => _ini.WriteInteger(section, key, value);
    /// <inheritdoc />
    public void Dispose() => _ini.Dispose();
}

/// <summary>
/// 原文 `:52-61` 的两个单元级全局（`g_AliyunSendSMSThread` 与 `g_SendSMSConfig` 的 typed const 初值）。
/// </summary>
public static class Sweep9FormsSmsGlobals
{
    /// <summary>原文 `:52 g_AliyunSendSMSThread: TAliyunSendSMSThread = nil;`。</summary>
    public static TAliyunSendSMSThread? g_AliyunSendSMSThread;

    /// <summary>
    /// 原文 `:53-61 g_SendSMSConfig: TSendSMSConfig = (SignName: ''; TemplateCodeBind: '';
    /// TemplateCodeCheck: ''; ResendVerifyCodeCount: 3; VerifySendInterval: 60;
    /// VerifyCodeTimeOutTime: 120; VerifyCodeErrorCount: 3;);`
    /// </summary>
    public static readonly TSendSMSConfig g_SendSMSConfig = new();

    /// <summary>原文 `acsUtils/acsParams` 的 `g_RequestStr`（`MainOutMessage(g_RequestStr)`，:213）。</summary>
    public static string g_RequestStr = "";

    /// <summary>原文 `g_AcsUtil`（acsUtils.pas）。</summary>
    public static readonly Sweep9FormsAcsUtil g_AcsUtil = new();

    /// <summary>测试隔离：复位到原文 initial 状态（typed const 初值）。</summary>
    public static void Reset()
    {
        g_AliyunSendSMSThread = null;
        g_SendSMSConfig.SignName = "";
        g_SendSMSConfig.TemplateCodeBind = "";
        g_SendSMSConfig.TemplateCodeCheck = "";
        g_SendSMSConfig.ResendVerifyCodeCount = 3;
        g_SendSMSConfig.VerifySendInterval = 60;
        g_SendSMSConfig.VerifyCodeTimeOutTime = 120;
        g_SendSMSConfig.VerifyCodeErrorCount = 3;
        g_RequestStr = "";
        g_AcsUtil.KeyID = "";
        g_AcsUtil.KeySecret = "";
    }
}

/// <summary>本单元的无头/宿主接缝（静态）。</summary>
public static class Sweep9FormsSmsSeams
{
    /// <summary>
    /// 原文 `:77 inherited Create(False)` ⇒ **构造即起真实线程**（线程随即阻塞在
    /// `FEvent.WaitFor(INFINITE)`）。无头测试置 **false** 以避免真实线程（默认 true = 生产）。
    /// </summary>
    public static bool StartThreadOnCreate = true;

    /// <summary>原文 `IniFiles.TIniFile.Create(FileName)`（:266）。</summary>
    public static Func<string, ISweep9FormsSmsIniFile> CreateIniFile = fileName => new Sweep9FormsSmsIniFile(fileName);

    /// <summary>
    /// 原文 `:81 FSendSmsRequest := TSendSmsRequest.Create;`（SendSmsRequest.pas）。
    /// <para>
    /// **静态**接缝：原文在**构造期**就创建它（并立刻写 `SignName`）⇒ 注入必须早于 `new`，
    /// 故不能做成实例字段（实例字段要到对象初始化器才赋值，晚于构造函数）。
    /// </para>
    /// </summary>
    public static Func<ISweep9FormsSendSmsRequest> CreateSendSmsRequest
        = () => new Sweep9FormsNullSendSmsRequest();

    /// <summary>
    /// 原文 `:89 FEvent := TEvent.Create(nil, False, False, '');`（SyncObjs）。
    /// **静态**接缝，理由同上（构造期创建）。
    /// </summary>
    public static Func<ISweep9FormsEvent> CreateEvent = () => new Sweep9FormsAutoResetEvent();

    /// <summary>测试隔离。</summary>
    public static void Reset()
    {
        StartThreadOnCreate = true;
        CreateIniFile = fileName => new Sweep9FormsSmsIniFile(fileName);
        CreateSendSmsRequest = () => new Sweep9FormsNullSendSmsRequest();
        CreateEvent = () => new Sweep9FormsAutoResetEvent();
    }
}
