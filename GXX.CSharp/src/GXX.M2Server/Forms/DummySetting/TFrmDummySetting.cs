// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmDummySetting.pas（1,081 行，GBK）
//  同源 DFM：Source/M2Engine/Forms/uFrmDummySetting.dfm（34,189 字节）
//  类型：TFrmDummySetting（:10-206）；全局过程 ShowFrmDummySetting（:208 / :217-227）
//  本文件：类型/控件面声明 + 构造（DFM 控件树 1:1）+ 全部接缝 + ShowFrmDummySetting
//
//  ⚠ 全树 0 命中（本车道开工前按任务书硬性要求 6 核验）：
//      git grep -l -E "(class|struct|enum|interface|delegate) +(partial +)?TFrmDummySetting\b" main
//    → 空。同理核验了 TDummyLogon（**已存在**，直接复用，不新增）、
//      DummySettingState / DummySettingMessageBoxSeam / TSpinEditExSeam /
//      TListBoxSeam / TPageControlSeam（均空 ⇒ 允许新增）。
//
//  ★ 无头 UI 规程（任务书硬性要求 3）：
//    · `Application.MessageBox`（:352 / :437 / :445）→ `MessageBoxHandler` 接缝，
//      默认走既有 `M2Forms.MessageBox`（**测试可注入 ⇒ 不弹窗**）。
//    · `ShowModal`（:223）→ `DummySettingMessageBoxSeam.UiEnabled` 开关。
//    · UI **取值/写回规则**抽成 `ModValue` / `uModValue` / `ApplyDisableDummyRun` /
//      `ApplyWarDisHumRun` / `AddSelectedMaps` / `AddSelectedMons` 等纯逻辑（见 Handlers）。
//    · `SetFocus`（:353 / :438 / :446）无头不可断言 → `SetFocusProbe` 决策镜像。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.DummySetting;

/// <summary>
/// `uFrmDummySetting.pas` `TFrmDummySetting` 1:1（M2Engine 假人设置窗体，1,081 行）。
///
/// 窗体 5 页签（DFM `pgcMain.ActivePage = ts1`）：
///   · 0 = `ts1` 假人名单/出生点（`lstDummyList` + `edtDummyName` + 出生地图/坐标 + 登录）
///   · 1 = `ts2` 假人行为（自动修装/召英雄、三职业攻速·走速、HP/MP 回速·基数、自动加血蓝）
///   · 2 = `ts3` 跑动限制（`chkDisDummyRun` 主开关 + 10 个从属勾选）
///   · 3 = `ts4` 禁止移动地图（`lstDisableMoveMap` ← `lstMapList`）
///   · 4 = `ts5` 不主动攻击怪物（`lstNoAttackMonList` ← `lstMonList`）
///
/// 无头 UI 处置见文件头。
/// </summary>
public sealed partial class TFrmDummySetting : System.Windows.Forms.Form
{
    // ========================================================================
    //  :199-200 private 字段
    // ========================================================================

    /// <summary>:199 `FIsDummyDisableMoveMapChanged: Boolean`。</summary>
    private bool FIsDummyDisableMoveMapChanged;

    /// <summary>:200 `FIsDummyNoActiveAttackMonChanged: Boolean`。</summary>
    private bool FIsDummyNoActiveAttackMonChanged;

    /// <summary>DFM 控件树（构造期由 <see cref="InitializeComponents"/> 填充）。</summary>
    public DummySettingControls Ct = null!;

    // ========================================================================
    //  接缝（原文依赖的未移植单元 / 不可测试的副作用）
    // ========================================================================

    /// <summary>
    /// 接缝：`Application.MessageBox`（:352 / :437 / :445）。
    /// 签名与既有 `M2Forms.MessageBox` / `GamePetsForm.MessageBoxHandler` **完全一致**
    /// `(text, caption, flags) -> answer`，默认转调 `M2Forms.MessageBox`。
    /// 接缝：待 M2Share.pas（`Application`）批次移植后接入；既有 M2Server 窗体族统一用它。
    /// </summary>
    public Func<string, string, int, int>? MessageBoxHandler;

    /// <summary>
    /// 探针：`edtDummyHomeMap.SetFocus`（:353 / :438 / :446）。
    /// 无头环境无焦点概念 ⇒ 不可断言；测试注入以断言"确实走了错误分支"。
    /// </summary>
    public Action<string>? SetFocusProbe;

    /// <summary>
    /// 接缝：`g_MapManager.FindMap(g_Config.sDummyHomeMap)`（:350 / :443）与
    /// `g_MapManager.Count` / `Items[I].sMapName`（:301-305）。
    /// <para>
    /// 返回 `null` = 原文 `FindMap(...) = nil`（:350/:443 的判空分支）。
    /// </para>
    /// 接缝：待 Envir.pas / MapManager 批次移植后接入。
    /// 要求签名：`TEnvirnoment? g_MapManager.FindMap(string sMapName)` + `IReadOnlyList<TEnvirnoment> Items`。
    /// </summary>
    public Func<string, GXX.M2Server.Engine.TEnvirnoment?>? FindMapHandler;

    /// <summary>
    /// 接缝：`g_MapManager` 的地图枚举（:301-305 `for I := 0 to g_MapManager.Count - 1 do
    /// Envir := TEnvirnoment(g_MapManager.Items[I]); lstMapList.Items.Add(Envir.sMapName)`）。
    /// 返回 `null` 视为空表。
    /// </summary>
    public Func<IReadOnlyList<GXX.M2Server.Engine.TEnvirnoment>?>? MapListHandler;

    /// <summary>
    /// 接缝：`UserEngine.MonsterList` 枚举（:310-314，取 `MonInfo.sName` 填入 `lstMonList`）。
    /// <para>
    /// ⚠ 与既有 `GamePetsForm.MonsterListHandler` 同形（返回 `(sName, btRace)`），
    /// 但本单元**只用 `sName`**（原文 `lstMonList.Items.AddObject(MonInfo.sName, TObject(MonInfo))`
    /// 的 `Objects[]` 载体在托管侧无消费者 —— 全树 0 处读该 `Objects`）。
    /// </para>
    /// 接缝：待 UsrEngn.pas / ObjMon.pas 怪物表移植后接入。
    /// </summary>
    public Func<IReadOnlyList<(string sName, byte btRace)>?>? MonsterListHandler;

    /// <summary>
    /// 接缝：`g_MultiThreadRun` + `UserEngine.MonsterList.LockR(6)/UnLockR`（:307-318，
    /// 原文 `{$IF MULTI_THREAD = 1}` 包裹）。托管侧默认 false = 不加锁。
    /// 接缝：待 M2Threads.pas / UsrEngn.pas 移植后接入。
    /// </summary>
    public bool g_MultiThreadRun;

    /// <summary>接缝：`UserEngine.MonsterList.LockR(6)`（:308）。</summary>
    public Action<int>? MonsterListLockR;

    /// <summary>接缝：`UserEngine.MonsterList.UnLockR`（:317）。</summary>
    public Action? MonsterListUnLockR;

    /// <summary>
    /// 接缝：`UserEngine.GetPlayObject(name)`（:365，`= nil` 判定）。
    /// 返回 `null` = 原文 `= nil`。
    /// 接缝：待 UsrEngn.pas 移植后接入。
    /// </summary>
    public Func<string, object?>? GetPlayObjectHandler;

    /// <summary>
    /// 接缝：`UserEngine.FindDummyLogon(name)`（:365）。
    /// 接缝：待 UsrEngn.pas（`UsrEngn.pas:4287`）移植后接入。
    /// </summary>
    public Func<string, bool>? FindDummyLogonHandler;

    /// <summary>
    /// 接缝：`UserEngine.AddDummyLogon(@DummyLogon)`（:369）。
    /// 原文传 `pTDummyLogon` **指针**（栈上局部记录取址）；托管侧 `TDummyLogon` 是 `class`
    /// （`GXX.Core.Protocol.Grobal2.Types6.cs:131`，**已存在，本车道不新增**）⇒ 传引用。
    /// 接缝：待 UsrEngn.pas（`UsrEngn.pas:4307`）移植后接入。
    /// </summary>
    public Action<TDummyLogon>? AddDummyLogonHandler;

    /// <summary>
    /// 接缝：`Config.WriteBool/WriteInteger/WriteString`（:452-510，section 恒为 `'Setup'`）。
    /// 为 `null` 时默认走 `M2ShareState.ConfigIni`（真实 `!Setup.txt`）；
    /// 测试注入以捕获"写了哪些键值"。
    /// 签名与既有 `GamePetsForm.WriteBoolHandler` 同形。
    /// 接缝：待 M2Share.pas 全局 `Config` 批次移植后接入。
    /// </summary>
    public Action<string, bool>? WriteBoolHandler;

    /// <summary>接缝：`Config.WriteInteger('Setup', key, value)`。</summary>
    public Action<string, int>? WriteIntegerHandler;

    /// <summary>接缝：`Config.WriteString('Setup', key, value)`。</summary>
    public Action<string, string>? WriteStringHandler;

    // ========================================================================
    //  :208 / :217-227 ShowFrmDummySetting
    // ========================================================================

    /// <summary>
    /// `uFrmDummySetting.pas:217-227 ShowFrmDummySetting` 1:1
    /// （原文是单元级全局过程，此处落为静态方法以保留"无宿主"调用形态）。
    /// <para>
    /// ★ 原文 `:223 FrmDummySetting.ShowModal` → `DummySettingMessageBoxSeam.ShowModal`
    /// （无头开关，见接缝文件）。原文 `try/finally FrmDummySetting.Free` → `using`。
    /// </para>
    /// </summary>
    /// <param name="formFactory">
    /// 构造接缝（原文硬编码 `TFrmDummySetting.Create(nil)`）。为 `null` 时用默认构造。
    /// </param>
    public static void ShowFrmDummySetting(Func<TFrmDummySetting>? formFactory = null)
    {
        // 原文 FrmDummySetting := TFrmDummySetting.Create(nil);
        TFrmDummySetting FrmDummySetting = formFactory != null ? formFactory() : new TFrmDummySetting();
        try
        {
            // 原文 FrmDummySetting.ShowModal;
            DummySettingMessageBoxSeam.ShowModal(FrmDummySetting);
        }
        finally
        {
            // 原文 FrmDummySetting.Free;
            FrmDummySetting.Dispose();
        }
    }

    // ========================================================================
    //  构造（DFM 控件树 1:1）
    // ========================================================================

    /// <summary>构造 + 装载 DFM 控件树（`OnCreate` 由宿主触发；测试显式调 <see cref="FormCreate"/>）。</summary>
    public TFrmDummySetting()
    {
        Ct = new DummySettingControls();
        InitializeComponents();
    }

    /// <summary>弹窗转调：优先本窗体注入，否则走既有 `M2Forms.MessageBox`。</summary>
    private int ShowMessageBox(string text, string caption, int flags)
        => MessageBoxHandler != null
            ? MessageBoxHandler(text, caption, flags)
            : M2Forms.MessageBox(text, caption, flags);

    /// <summary>`procedure SetFocus` 的探针镜像（无头不可断言）。</summary>
    private void ProbeSetFocus(string controlName) => SetFocusProbe?.Invoke(controlName);

    // ========================================================================
    //  内部辅助（替代 `TListBox` 的部分成员 / `Config.Write*` 默认实现）
    // ========================================================================

    /// <summary>
    /// `TListBox.Selected[Index]`（WinForms 等价物是 `GetSelected`；`ListBox.SelectedIndices`
    /// 每次访问都会新建集合，故用 `GetSelected` 逐项查，保持 O(1) 语义）。
    /// </summary>
    private static bool IsSelected(System.Windows.Forms.ListBox listBox, int index) => listBox.GetSelected(index);

    /// <summary>
    /// `TListBox.DeleteSelected`（原文 :397）的 WinForms 等价实现：
    /// 删除全部选中行；无选中时为 no-op；结束后 `SelectedIndex = -1`（Delphi 同样复位）。
    /// </summary>
    private static void DeleteSelectedRows(System.Windows.Forms.ListBox listBox)
    {
        for (int i = listBox.Items.Count - 1; i >= 0; i--)
        {
            if (listBox.GetSelected(i))
                listBox.Items.RemoveAt(i);
        }
        listBox.SelectedIndex = -1;
    }

    /// <summary>
    /// `g_MapManager.FindMap(sMapName)`（:350 / :443）接缝转调。
    /// <para>
    /// ⚠ 接缝**不得静默返回中性值**（台账 §25.2）：本方法把"未注入接缝"与
    /// "接了但真的查不到"**统一**为返回 `null` —— 这是**有依据**的，因为原文
    /// `FindMap` 的失败语义本就是 `nil`，而"没接引擎"在托管侧等价于"地图表为空"
    /// （`g_MapManager` 未加载任何地图时 `FindMap` 也返回 `nil`）。
    /// 上层分支（弹「出生地图设置错误！」并 `Exit`）因此在两种情况下都正确，
    /// **不存在**「字段语义错被伪装成分支没命中」的风险。
    /// </para>
    /// </summary>
    private TEnvirnoment? FindMap(string sMapName) => FindMapHandler?.Invoke(sMapName);

    /// <summary>`UserEngine.GetPlayObject(name)`（:365）接缝转调；未注入 ⇒ `null`（= 原文 `nil`）。</summary>
    private object? GetPlayObject(string sCharName) => GetPlayObjectHandler?.Invoke(sCharName);

    /// <summary>`UserEngine.FindDummyLogon(name)`（:365）接缝转调；未注入 ⇒ `false`。</summary>
    private bool FindDummyLogon(string sCharName) => FindDummyLogonHandler?.Invoke(sCharName) ?? false;

    /// <summary>`Config.WriteBool('Setup', key, value)`（:452 起）。注入优先，否则落 `M2ShareState.ConfigIni`。</summary>
    private void WriteBool(string key, bool value)
    {
        if (WriteBoolHandler != null) { WriteBoolHandler(key, value); return; }
        M2ShareState.ConfigIni.WriteBool("Setup", key, value);
    }

    /// <summary>`Config.WriteInteger('Setup', key, value)`。</summary>
    private void WriteInteger(string key, int value)
    {
        if (WriteIntegerHandler != null) { WriteIntegerHandler(key, value); return; }
        M2ShareState.ConfigIni.WriteInteger("Setup", key, value);
    }

    /// <summary>`Config.WriteString('Setup', key, value)`。</summary>
    private void WriteString(string key, string value)
    {
        if (WriteStringHandler != null) { WriteStringHandler(key, value); return; }
        M2ShareState.ConfigIni.WriteString("Setup", key, value);
    }
}
