// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  本文件：窗体族所依赖的**单元级全局 + M2Share.pas 函数子集**
//    · M2Share.pas:3708      g_DummyDisableMoveMapList: TGStringList
//    · M2Share.pas:3709      g_DummyNoActiveAttackMonList: TGStringList
//    · M2Share.pas:3914      g_DummyNameList: TGStringList
//    · M2Share.pas:16640-16658 GetDummyNameList(sName): Boolean
//    · M2Share.pas:16660-16678 SaveDummyNameList
//    · M2Share.pas:12922-12934 SaveDummyDisableMoveMap
//    · M2Share.pas:12968-12980 SaveDummyNoActiveAttackMonList
//    · M2Share.pas:12890-12920 LoadDummyDisableMoveMap（**未移植**，见下）
//    · M2Share.pas:12936-12966 LoadDummyNoActiveAttackMonList（**未移植**）
//    · M2Share.pas:16607-16638 LoadDummyNameList（**未移植**）
//
//  ⚠ 接缝说明：
//    M2Share.pas 属**顺序会话常驻区**（`src/GXX.M2Server/**` 其余部分只读）。
//    `M2ShareGlobals`（隔离清单 `M2ConfigIsolationCoverage.cs:48` 提到的那个类型）
//    **当前在托管侧并不存在**（已 `git grep` 确认：main 全树 0 命中
//    `class M2ShareGlobals`），故三个全局列表**无法注入既有类型**，只能在本车道
//    独占区内自带一份，待 M2Share.pas 全量批次移植后由集成方合并
//    （同 `Forms/GamePets/GamePetsConfig.cs` 的先例）。
//
//  ★ 三个 `Load*` 为什么不移植（切片边界，**不是遗漏**）：
//    它们都不在 `uFrmDummySetting.pas` 里（原文只有 `Save*`），属 M2Share.pas 的
//    初始化段，且依赖 `g_Config.sEnvirDir` 与「文件不存在则**创建空文件**」的副作用语义。
//    本车道按任务书硬性要求 2「不要顺手移植依赖」只做最小接缝。
//    窗体侧 `FormCreate`（:320/:321）只**读**这两个列表 ⇒ 读空列表与原文
//    「Load 未跑」时的窗体行为不可区分（不构成语义偏离）。
//    接缝：待 M2Share.pas 移植后接入。
//
//  ★★ 本文件的两条**正式偏离**（台账 §26/§30.3 要求逐条编号登记）：
//    --- D-p8-01：`TGStringList` → `GXX.Core.Util.TStringList` ---
//      原文三个全局的类型是 `SDK.pas:80 TGStringList = class(TStringList)`
//      （只加临界区 + Up/Down，**不覆写 Sorted**）。
//      托管侧 `GXX.Core.Protocol.SDK.TGStringList` 是 SDK.cs 的**嵌套类**
//      （`SDK.TGStringList`）、且**只有 get-only 索引器**，**没有 `Sorted` 属性** ⇒
//      若照抄类型，`g_DummyDisableMoveMapList.Sorted := True`（原文 :524 / :988）
//      与 `g_DummyNoActiveAttackMonList.Sorted := True`（:542 / :1072）
//      **无法表达**（写不出、也测不出）。
//      托管行为：改用 `GXX.Core.Util.TStringList`（该类型有真 `Sorted` 语义，
//      置 true 会用 `CompareStrings` 就地把已有项排序 —— 与 Delphi 一致）。
//      恢复途径：若日后 `GXX.Core.Protocol.SDK.TGStringList` 补上 `Sorted`
//      （转发到底层 `TStringList`），本文件可改回该类型，行为不变。
//    --- D-p8-02：临界区改为**单元级**单把锁 ---
//      原文 `TGStringList` 的临界区是**每实例**（构造函数 `InitializeCriticalSection`）。
//      托管侧三个列表是 `static readonly` 单例 ⇒ 用一把 `static readonly object` 串行化。
//      行为差异：仅影响"并发访问同一列表"的粒度；本窗体的全部写入点都在
//      `ButtonDummySaveClick` / `*SaveClick`（单线程 UI 路径），**无可观测差异**。
//      恢复途径：无（这是托管侧的必要形式）。
// ============================================================================

using System;
using GXX.Core.Util;

namespace GXX.M2Server.Forms.DummySetting;

/// <summary>
/// `M2Share.pas` 假人相关的三个单元级 `TGStringList` 全局（行号见文件头）。
/// <para>
/// ⚠ 这三个是 `static readonly` 引用。程序集级测试隔离机制
/// （`M2ConfigIsolationState`）对 **非数组的 `static readonly` 引用只做引用比对、
/// 不做内容深拷贝**（该文件 :16-18 明确登记）⇒ **测试基类必须自行 `Clear()`**
/// （见 `DummySettingTestBase.ResetStatics`）。
/// </para>
/// </summary>
public static class DummySettingState
{
    /// <summary>`M2Share.pas:3914 g_DummyNameList: TGStringList`。</summary>
    public static readonly TStringList g_DummyNameList = new();

    /// <summary>`M2Share.pas:3708 g_DummyDisableMoveMapList: TGStringList`。</summary>
    public static readonly TStringList g_DummyDisableMoveMapList = new();

    /// <summary>`M2Share.pas:3709 g_DummyNoActiveAttackMonList: TGStringList`。</summary>
    public static readonly TStringList g_DummyNoActiveAttackMonList = new();

    // ------------------------------------------------------------------
    //  临界区（见文件头 D-p8-02）
    // ------------------------------------------------------------------

    private static readonly object LockObj = new();

    /// <summary>`TGStringList.Lock`（Delphi 临界区进入；托管侧见文件头 D-p8-02）。</summary>
    public static void Lock() => System.Threading.Monitor.Enter(LockObj);

    /// <summary>`TGStringList.UnLock`。</summary>
    public static void UnLock() => System.Threading.Monitor.Exit(LockObj);

    // ------------------------------------------------------------------
    //  写盘接缝（原文 `TGStringList.SaveToFile`）
    // ------------------------------------------------------------------

    /// <summary>
    /// 接缝：`TGStringList.SaveToFile(sFileName)`（原文 :12929 / :12975）。
    /// 默认走真实 `TStringList.SaveToFile`（GBK）；测试注入以断言
    /// "写到了哪个文件、写了哪些行、顺序如何"。
    /// <para>
    /// ⚠ **只覆盖另外两个 `Save*`**：`SaveDummyNameList`（:16660-16678）原文走的是
    /// **普通 `TStringList` 的中转 `SaveList.SaveToFile`**，**不经过本接缝**（形态差异，
    /// 1:1 保留；测试 `SaveDummyNameList_UsesPlainListPath_NotSeam` 锁定该差异）。
    /// </para>
    /// 接缝：待 M2Share.pas 移植后接入（届时本接缝可删）。
    /// </summary>
    public static Action<TStringList, string>? SaveListToFile;

    /// <summary>`g_Config.sEnvirDir`（原文 `M2Share.pas` 全局 `g_Config` 字段）。</summary>
    private static string EnvirDir => GXX.M2Server.Engine.M2Config.sEnvirDir;

    // ------------------------------------------------------------------
    //  M2Share.pas:16640-16658 GetDummyNameList — 1:1
    // ------------------------------------------------------------------

    /// <summary>
    /// `M2Share.pas:16640-16658 GetDummyNameList(sName: string): Boolean` 1:1。
    /// <para>
    /// 原文 `:16649 if CompareText(sName, g_DummyNameList.Strings[i]) = 0 then` ——
    /// **大小写无关**、首个命中即 `Result := True` 并 `Break`，未命中 `False`。
    /// </para>
    /// <para>
    /// ⚠ 与 `GamePetsState.GetGamePetConfig`（`SameText`）同族；此处用
    /// `string.Equals(..., StringComparison.OrdinalIgnoreCase)`（BCL 等价）。
    /// Delphi 7 `CompareText` 是 ANSI 序的不区分大小写比较；对中文名等价
    /// （中文无大小写折叠）。
    /// </para>
    /// </summary>
    public static bool GetDummyNameList(string sName)
    {
        bool Result = false;                                    // :16643
        Lock();                                                 // :16645
        try
        {
            for (int i = 0; i <= g_DummyNameList.Count - 1; i++)    // :16647
            {
                if (string.Equals(sName ?? "", g_DummyNameList[i] ?? "", StringComparison.OrdinalIgnoreCase))
                {
                    Result = true;                              // :16651
                    break;                                      // :16652
                }
            }
        }
        finally
        {
            UnLock();                                           // :16656
        }
        return Result;
    }

    // ------------------------------------------------------------------
    //  M2Share.pas:16660-16678 SaveDummyNameList — 1:1
    // ------------------------------------------------------------------

    /// <summary>
    /// `M2Share.pas:16660-16678 SaveDummyNameList: Boolean` 1:1。
    /// <para>
    /// ★ 与另外两个 `Save*` **形态不同**：本函数**经中转 `SaveList`**
    /// （`:16672 SaveList.Add(g_DummyNameList.Strings[i])` 后 `:16674 SaveList.SaveToFile`），
    /// 而 `SaveDummyDisableMoveMap` / `SaveDummyNoActiveAttackMonList` 是
    /// **直接在 `TGStringList` 上调 `SaveToFile`**（:12929 / :12975）。原文如此。
    /// 行为等价（都是逐行 GBK 落盘），但 `SaveListToFile` 接缝**只拦得住后两者**。
    /// </para>
    /// <para>原文无失败路径，恒返回 True（:16677 `Result := True`）。</para>
    /// </summary>
    public static bool SaveDummyNameList()
    {
        string sFileName = EnvirDir + "DummyNameList.txt";      // :16666
        var SaveList = new TStringList();                       // :16667
        Lock();                                                 // :16668
        try
        {
            for (int i = 0; i <= g_DummyNameList.Count - 1; i++)    // :16670
            {
                SaveList.Add(g_DummyNameList[i]);               // :16672
            }
            // :16674 原文 SaveList.SaveToFile(sFileName) —— SaveList 是**普通 TStringList**
            SaveList.SaveToFile(sFileName);
        }
        finally
        {
            UnLock();                                           // :16676
        }
        return true;                                            // :16677
    }

    // ------------------------------------------------------------------
    //  M2Share.pas:12922-12934 SaveDummyDisableMoveMap — 1:1
    // ------------------------------------------------------------------

    /// <summary>
    /// `M2Share.pas:12922-12934 SaveDummyDisableMoveMap: Boolean` 1:1。
    /// 直接在 `g_DummyDisableMoveMapList` 上调 `SaveToFile`（与 NameList 版不同，见上）。
    /// </summary>
    public static bool SaveDummyDisableMoveMap()
    {
        string sFileName = EnvirDir + "DummyDisableMoveMap.txt";    // :12926
        Lock();                                                     // :12927
        try
        {
            // :12929 原文 g_DummyDisableMoveMapList.SaveToFile(sFileName)
            (SaveListToFile ?? DefaultSaveListToFile)(g_DummyDisableMoveMapList, sFileName);
        }
        finally
        {
            UnLock();                                               // :12931
        }
        return true;                                                // :12933
    }

    // ------------------------------------------------------------------
    //  M2Share.pas:12968-12980 SaveDummyNoActiveAttackMonList — 1:1
    // ------------------------------------------------------------------

    /// <summary>
    /// `M2Share.pas:12968-12980 SaveDummyNoActiveAttackMonList: Boolean` 1:1。
    /// </summary>
    public static bool SaveDummyNoActiveAttackMonList()
    {
        string sFileName = EnvirDir + "DummyNoActiveAttackMonList.txt";   // :12972
        Lock();                                                          // :12973
        try
        {
            // :12975 原文 g_DummyNoActiveAttackMonList.SaveToFile(sFileName)
            (SaveListToFile ?? DefaultSaveListToFile)(g_DummyNoActiveAttackMonList, sFileName);
        }
        finally
        {
            UnLock();                                                    // :12977
        }
        return true;                                                     // :12979
    }

    // ------------------------------------------------------------------
    //  内部：写盘默认实现
    // ------------------------------------------------------------------

    /// <summary>`TStringList.SaveToFile` 的直接转发（GBK，与 `GXX.Core.Util.TStringList` 一致）。</summary>
    private static void DefaultSaveListToFile(TStringList list, string fileName) => list.SaveToFile(fileName);

    /// <summary>测试辅助：把列表快照成 `string[]`（原文 `Strings[i]` 遍历）。</summary>
    public static string[] Snapshot(TStringList list)
    {
        var result = new string[list.Count];
        for (int i = 0; i <= list.Count - 1; i++)
            result[i] = list[i];
        return result;
    }
}
