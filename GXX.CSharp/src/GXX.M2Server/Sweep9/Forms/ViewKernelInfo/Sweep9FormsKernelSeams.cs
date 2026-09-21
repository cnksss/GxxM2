// ============================================================================
// ViewKernelInfo.pas 的接缝层（`g_Config` 段内存视图 / `g_M2RunThreadMgr`）
//
// 原文 uses（:5-7）：Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls,
//   Forms, Dialogs, StdCtrls, ExtCtrls, ComCtrls, Grids, **M2Share, M2Threads**。
// 依赖缺口（全树 0 命中，见 docs\并行报告-p9-m2-forms.md §0.4）：
//   · `g_Config` 的 `nLoadDBCount/nLoadDBErrorCount/nSaveDBCount/nDBQueryID/nItemNumber/
//     nItemNumberEx/nWinLottery*/GlobalVal[0..999]` —— 托管侧 `M2Config`（Engine）**没有**这些字段
//     ⇒ 本文件给**只读读取面**接缝 `Sweep9FormsKernelConfig`。
//   · `g_Config` 的 `UserEngineThread/IDSocketThread/DBSOcketThread`（`pTThreadInfo` 指针）
//     及紧随其后的 6 个字段 —— 用 `TConfigThreadRegion` 做**段内存视图**，
//     1:1 复刻 `ViewKernelInfo.pas:116-133` 的 4 字节重叠写（偏离 **D-P9-01**）。
//   · `M2Threads.pas` 的 `TM2RunThread/TM2RunThreadMgr` —— 全单元未移植 ⇒
//     `Sweep9FormsRunThreadInfo` 载体 + `RunThreadMgrHandler` 接缝。
//
// ★ 接缝纪律：查询类接缝未接线时**显式抛异常**（不返回中性值），
//   理由见《并行派发台账》§25.2 —— 若"未接线"被静默当成"引擎里没有线程/计数为 0"，
//   本窗体（内核数据查看）会显示**一整屏虚假的 0**，而 0 在这里恰好是"合法读数"，
//   无法区分 ⇒ 属于"字段语义错被伪装成数据"，必须显式失败。
// ============================================================================

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// `M2Threads.pas:11-39 TM2RunThread` 的接缝载体（该单元尚未移植，全树 0 命中）。
/// <para>
/// 字段名与原文逐字一致；`Handle`/`ThreadID`/`RunTick` 一族按 §3.1 映射为无符号
/// （`THandle`/`LongWord`）。`ThreadCPUUsage` 在原文是**方法**（:130-154，含计时状态），
/// 故接缝以委托承载、未注入即抛（不返回 0——0 是合法读数，会变成假数据）。
/// </para>
/// </summary>
public sealed class Sweep9FormsRunThreadInfo
{
    /// <summary>原文 `property ThreadDesc: string read FThreadDesc`（:32）。</summary>
    public string ThreadDesc = "";
    /// <summary>原文 `TThread.Handle: THandle`（:183 用于 `IntToStr(Thread.Handle)`）。</summary>
    public uint Handle;
    /// <summary>原文 `TThread.ThreadID: LongWord`（:184）。</summary>
    public uint ThreadID;
    /// <summary>原文 `property RunTick: LongWord read FRunTick`（:33）。</summary>
    public uint RunTick;
    /// <summary>原文 `property MinRunTick: LongWord read FMinRunTick`（:34）。</summary>
    public uint MinRunTick;
    /// <summary>原文 `property MaxRunTick: LongWord read FMaxRunTick`（:35）。</summary>
    public uint MaxRunTick;
    /// <summary>原文 `property MaxThreadCPUUsage: Integer read/write`（:38）。</summary>
    public int MaxThreadCPUUsage;

    /// <summary>原文 `function ThreadCPUUsage: Integer`（M2Threads.pas:130-154）。未注入即抛。</summary>
    public Func<int>? ThreadCPUUsageFn;

    /// <summary>调用原文 `ThreadCPUUsage`；未注入即抛（§25.2：不返回可被误当真实读数的 0）。</summary>
    public int ThreadCPUUsage()
        => (ThreadCPUUsageFn ?? throw Sweep9FormsKit.NotWired(
            "Sweep9FormsRunThreadInfo.ThreadCPUUsage",
            "M2Threads.pas:130-154 TM2RunThread.ThreadCPUUsage"))();
}

/// <summary>
/// 原文 `M2Share.pas` 里 `g_Config: TConfig` 的**本窗体读取面**（只读 + 可写测试注入）。
/// <para>
/// 字段名与原文逐字一致；`GlobalVal` 按原文 `array[0..999] of Integer` ⇒ 长度 1000。
/// </para>
/// </summary>
public sealed class Sweep9FormsKernelConfig
{
    /// <summary>原文 `M2Share.pas:1799 nLoadDBCount: Integer;`。</summary>
    public int nLoadDBCount;
    /// <summary>原文 `:1800 nLoadDBErrorCount: Integer;`。</summary>
    public int nLoadDBErrorCount;
    /// <summary>原文 `:1801 nSaveDBCount: Integer;`。</summary>
    public int nSaveDBCount;
    /// <summary>原文 `:1802 nDBQueryID: Integer;`。</summary>
    public int nDBQueryID;
    /// <summary>原文 `nItemNumber: Integer;`（"怪物掉落物品"序号）。</summary>
    public int nItemNumber;
    /// <summary>原文 `nItemNumberEx: Integer;`（"命令制造物品"序号）。</summary>
    public int nItemNumberEx;
    /// <summary>原文 `:1510 nWinLotteryCount: Integer;`。</summary>
    public int nWinLotteryCount;
    /// <summary>原文 `:1511 nNoWinLotteryCount: Integer;`。</summary>
    public int nNoWinLotteryCount;
    /// <summary>原文 `:1512-1517 nWinLotteryLevel1..6`。</summary>
    public int nWinLotteryLevel1;
    /// <summary>原文 `nWinLotteryLevel2`。</summary>
    public int nWinLotteryLevel2;
    /// <summary>原文 `nWinLotteryLevel3`。</summary>
    public int nWinLotteryLevel3;
    /// <summary>原文 `nWinLotteryLevel4`。</summary>
    public int nWinLotteryLevel4;
    /// <summary>原文 `nWinLotteryLevel5`。</summary>
    public int nWinLotteryLevel5;
    /// <summary>原文 `nWinLotteryLevel6`。</summary>
    public int nWinLotteryLevel6;

    /// <summary>原文 `M2Share.pas:1520 GlobalVal: array [0 .. 999] of Integer;`。</summary>
    public readonly int[] GlobalVal = new int[1000];

    /// <summary>原文 `g_Config` 里那三个线程指针字段起的**段内存视图**（见 <see cref="TConfigThreadRegion"/>）。</summary>
    public readonly TConfigThreadRegion ThreadRegion = new();
}

/// <summary>
/// `g_Config: TConfig` 中从 `UserEngineThread` 起 10 个 dword 的**段内存视图**。
/// <para>
/// 存在的唯一理由：`ViewKernelInfo.pas:116/122/128` 写的是
/// <c>ThreadInfo := @Config.UserEngineThread</c> —— **指针字段的地址**，
/// 随后按 `TThreadInfo` 的字段偏移往那块内存写 0。
/// `TThreadInfo`（M2Share.pas:558-568）8 个字段全是 4 字节（`dwRunTick`/`boActived`/
/// `nRunFlag`/`boTerminaled`/`nRunTime`/`nMaxRunTime`/`hThreadHandle`/`dwThreadID`），
/// 被写的 5 个字段偏移（dword 计）是 6/0/4/5/2 ⇒ 覆盖
/// <c>g_Config</c> 的 dword `base+0 … base+6`。用 dword 数组建模**精确等价**
/// （1 字节的 `boActived`/`boTerminaled` 从未被写）。
/// </para>
/// <para>
/// dword 索引 ↔ `g_Config` 字段（M2Share.pas:1808-1817）：
/// <code>
///   0 UserEngineThread(pTThreadInfo)   1 IDSocketThread   2 DBSOcketThread
///   3 nUserSellOffCount   4 nUserSellOffTax   5 nSkill69CD
///   6 nSkill69AddTime     7 nSkill69AddRange   8 boSkill69SameLevel(+3 字节填充)   9 nSkill70CD
/// </code>
/// </para>
/// </summary>
public sealed class TConfigThreadRegion
{
    /// <summary>本视图覆盖的 dword 数（0..9）。</summary>
    public const int DwordCount = 10;

    private readonly int[] _dword = new int[DwordCount];

    /// <summary>原文 `g_Config.UserEngineThread: pTThreadInfo`（0 表示 nil）。</summary>
    public int UserEngineThread { get => _dword[0]; set => _dword[0] = value; }
    /// <summary>原文 `g_Config.IDSocketThread: pTThreadInfo`（0 表示 nil）。</summary>
    public int IDSocketThread { get => _dword[1]; set => _dword[1] = value; }
    /// <summary>原文 `g_Config.DBSOcketThread: pTThreadInfo`（0 表示 nil）。</summary>
    public int DBSOcketThread { get => _dword[2]; set => _dword[2] = value; }
    /// <summary>原文 `g_Config.nUserSellOffCount: Integer`。</summary>
    public int nUserSellOffCount { get => _dword[3]; set => _dword[3] = value; }
    /// <summary>原文 `g_Config.nUserSellOffTax: Integer`。</summary>
    public int nUserSellOffTax { get => _dword[4]; set => _dword[4] = value; }
    /// <summary>原文 `g_Config.nSkill69CD: Integer`。</summary>
    public int nSkill69CD { get => _dword[5]; set => _dword[5] = value; }
    /// <summary>原文 `g_Config.nSkill69AddTime: Integer`。</summary>
    public int nSkill69AddTime { get => _dword[6]; set => _dword[6] = value; }
    /// <summary>原文 `g_Config.nSkill69AddRange: Integer`。</summary>
    public int nSkill69AddRange { get => _dword[7]; set => _dword[7] = value; }
    /// <summary>原文 `g_Config.boSkill69SameLevel: Boolean`（dword 8 的低字节）。</summary>
    public bool boSkill69SameLevel
    {
        get => (_dword[8] & 0xFF) != 0;
        set => _dword[8] = (_dword[8] & ~0xFF) | (value ? 1 : 0);
    }
    /// <summary>原文 `g_Config.nSkill70CD: Integer`（dword 9）。</summary>
    public int nSkill70CD { get => _dword[9]; set => _dword[9] = value; }

    /// <summary>裸 dword 读（越界抛，同数组）。</summary>
    public int GetDword(int index) => _dword[index];

    /// <summary>裸 dword 写（**4 字节整写** —— 本视图的存在意义就是复刻这种重叠写）。</summary>
    public void SetDword(int index, int value) => _dword[index] = value;

    /// <summary>
    /// 复刻 `ViewKernelInfo.pas:116-133` 的五句写：
    /// <code>
    ///   ThreadInfo := @Config.&lt;Xxx&gt;Thread;   // 基址 = 该**指针字段**的地址
    ///   ThreadInfo.hThreadHandle := 0;        // TThreadInfo 偏移 6 dword
    ///   ThreadInfo.dwRunTick     := 0;        // 偏移 0 dword
    ///   ThreadInfo.nRunTime      := 0;        // 偏移 4 dword
    ///   ThreadInfo.nMaxRunTime   := 0;        // 偏移 5 dword
    ///   ThreadInfo.nRunFlag      := 0;        // 偏移 2 dword
    /// </code>
    /// </summary>
    /// <param name="baseDwordIndex">`@Config.XxxThread` 对应的 dword 索引（0/1/2）。</param>
    public void WriteThreadInfoFieldsAt(int baseDwordIndex)
    {
        // 写入顺序逐字照抄原文（:117-121），每句都是一次 4 字节写。
        SetDword(baseDwordIndex + 6, 0);      // hThreadHandle := 0
        SetDword(baseDwordIndex + 0, 0);      // dwRunTick     := 0
        SetDword(baseDwordIndex + 4, 0);      // nRunTime      := 0
        SetDword(baseDwordIndex + 5, 0);      // nMaxRunTime   := 0
        SetDword(baseDwordIndex + 2, 0);      // nRunFlag      := 0
    }

    /// <summary>把全部 dword 置为"非零哨兵"（测试用：让"被写坏"的字段可辨识）。</summary>
    public void FillSentinels(int baseValue = 100)
    {
        for (int i = 0; i < DwordCount; i++)
            _dword[i] = baseValue + i;
    }
}

/// <summary>
/// 原文 `ViewKernelInfo.pas:85 var frmViewKernelInfo: TfrmViewKernelInfo;`（单元级全局）。
/// <para>创建点 `svMain.pas:2768`、`Open` `:2770`、释放 `:2772`（均不在本单元内）。</para>
/// </summary>
public static class Sweep9FormsKernelInfoGlobals
{
    /// <summary>原文 `frmViewKernelInfo`（未创建时为 <c>null</c> == 原文 nil）。</summary>
    public static TfrmViewKernelInfo? frmViewKernelInfo;

    /// <summary>测试隔离。</summary>
    public static void Reset() => frmViewKernelInfo = null;
}
