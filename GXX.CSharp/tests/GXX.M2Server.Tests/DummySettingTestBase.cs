using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.DummySetting;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p8-m2-dummysetting：`uFrmDummySetting.pas`（1,081 行）→ `TFrmDummySetting` 1:1 转换测试。
///
/// ⚠ xUnit 串行化：本窗体族读写 `M2Config` 静态字段与
/// `DummySettingState.g_DummyNameList` / `g_DummyDisableMoveMapList` /
/// `g_DummyNoActiveAttackMonList` 三个 `static readonly` 列表（原文是单元级全局 var），
/// 与其它窗体族测试共享进程静态状态 ⇒ 归入独立集合 `DummySettingSerial`（DisableParallelization）。
/// </summary>
[CollectionDefinition("DummySettingSerial", DisableParallelization = true)]
public sealed class DummySettingSerialCollection
{
}

/// <summary>
/// 无头 UI 处置（任务书硬性要求 3）：
///   · 3 处 `Application.MessageBox`（原 :352/:437/:445）走 `MessageBoxHandler` 接缝
///     → 本基类统一注入 handler，**绝不**弹真实模态框（否则挂死 testhost）。
///   · `ShowModal`（原 :223）走 `DummySettingMessageBoxSeam.ShowModalHandler`，
///     每个用例前复位 `UiEnabled = false`。
///   · `SetFocus`（原 :353/:438/:446）走 `SetFocusProbe` 决策镜像。
///   · 全部事件处理器**直调**（它们已是 public），不需要消息泵。
/// </summary>
[Collection("DummySettingSerial")]
public abstract class DummySettingTestBase : IDisposable
{
    protected readonly string Dir;

    /// <summary>
    /// 当前窗体实例。**每个用例通过 <see cref="Recreate"/> 或 <see cref="OpenWith"/> 换新实例**
    /// —— 因为原文 `FormCreate`（:235/:301/:310/:320/:321）对列表框是**追加**语义：
    /// 原文每个窗体实例只 `Create` 一次，而 xUnit 每个用例都会跑一次 `FormCreate`
    /// ⇒ 复用实例会把上一个用例的列表项累积下来（**测试夹具问题，不是被测代码问题**）。
    /// </summary>
    protected TFrmDummySetting Form = null!;

    /// <summary>捕获的弹窗（文本, 标题, flags）。</summary>
    protected readonly List<(string Text, string Caption, int Flags)> Messages = new();

    /// <summary>捕获的 `Config.WriteBool/WriteInteger/WriteString`（section 恒 'Setup'）。</summary>
    protected readonly List<(string Key, bool Value)> WroteBool = new();
    protected readonly List<(string Key, int Value)> WroteInt = new();
    protected readonly List<(string Key, string Value)> WroteStr = new();

    /// <summary>捕获的 `SetFocus` 目标（无头不可断言 ⇒ 决策镜像）。</summary>
    protected readonly List<string> FocusProbes = new();

    /// <summary>捕获的 `AddDummyLogon(pTDummyLogon)` 调用。</summary>
    protected readonly List<TDummyLogon> LogonsAdded = new();

    /// <summary>捕获的写盘（文件, 内容行）—— `DummySettingState.SaveListToFile` 接缝。</summary>
    protected readonly List<(string FileName, string[] Lines)> WroteFiles = new();

    /// <summary>注入的 `FindMap` 结果（null = 原文 `nil`）。</summary>
    protected string[] KnownMaps = Array.Empty<string>();

    /// <summary>注入的 `GetPlayObject` 已存在名单。</summary>
    protected readonly HashSet<string> ExistingPlayObjects = new(StringComparer.Ordinal);

    /// <summary>注入的 `FindDummyLogon` 已登记名单。</summary>
    protected readonly HashSet<string> AlreadyQueuedLogons = new(StringComparer.Ordinal);

    /// <summary>注入的地图表 / 怪物表。</summary>
    protected List<TEnvirnoment> MapTable = new();
    protected List<(string sName, byte btRace)> MonsterTable = new();

    protected DummySettingTestBase()
    {
        Dir = Path.Combine(Path.GetTempPath(), "gxx_p8dummy_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        M2Config.sEnvirDir = Dir + "\\";

        ResetStatics();
        Form = StaRunner.New(() => new TFrmDummySetting());
        Wire(Form);
    }

    /// <summary>换一个全新的窗体实例（等价于原文一次 `TFrmDummySetting.Create(nil)`）。</summary>
    protected void Recreate()
    {
        StaRunner.New(() => Form.Dispose());
        Form = StaRunner.New(() => new TFrmDummySetting());
        Wire(Form);
    }

    protected void Wire(TFrmDummySetting form)
    {
        form.MessageBoxHandler = (text, caption, flags) =>
        {
            Messages.Add((text, caption, flags));
            return M2Forms.IDOK;
        };
        form.WriteBoolHandler = (k, v) => WroteBool.Add((k, v));
        form.WriteIntegerHandler = (k, v) => WroteInt.Add((k, v));
        form.WriteStringHandler = (k, v) => WroteStr.Add((k, v));
        form.SetFocusProbe = t => FocusProbes.Add(t);
        form.FindMapHandler = name => MapTable.Find(e => e.sMapName == name);
        form.MapListHandler = () => MapTable;
        form.MonsterListHandler = () => MonsterTable;
        form.GetPlayObjectHandler = name => ExistingPlayObjects.Contains(name) ? new object() : null;
        form.FindDummyLogonHandler = name => AlreadyQueuedLogons.Contains(name);
        form.AddDummyLogonHandler = d => LogonsAdded.Add(d);
    }

    /// <summary>把全部假人配置字段与三个全局列表复位到"确定初值"，避免测试间串味。</summary>
    protected static void ResetStatics()
    {
        DummySettingState.g_DummyNameList.Clear();
        DummySettingState.g_DummyDisableMoveMapList.Clear();
        DummySettingState.g_DummyNoActiveAttackMonList.Clear();
        DummySettingState.SaveListToFile = null;

        // 跑动段（M2Share.pas:4268-4272）
        M2Config.boDiableDummyRun = true;
        M2Config.boDummyRunHum = false;
        M2Config.boDummyRunMon = false;
        M2Config.boDummyRunNpc = false;
        M2Config.boDummyRunGuard = false;
        M2Config.boDummyWarDisHumRun = false;
        M2Config.boDummyWarHreoRun = false;
        M2Config.boDummySafeAreaLimited = false;
        M2Config.boDummySafeAreaDisNpcRun = false;
        M2Config.boSafeAreaDisShopStallDummyRun = false;
        M2Config.boSafeAreaDisOffLineDummyRun = false;

        // 登录段（M2Share.pas:4931-4940）
        M2Config.boDummyLogonRand = false;
        M2Config.nDummyLogonTime = 3;
        M2Config.sDummyHomeMap = "3";
        M2Config.nDummyHomeX = 330;
        M2Config.nDummyHomeY = 330;
        M2Config.boDummyAutoRepairItem = true;
        M2Config.boDummyAutoRecallHero = true;
        M2Config.dwDummyWarrorAttackTime = 1200;
        M2Config.dwDummyWizardAttackTime = 1200;
        M2Config.dwDummyTaoistAttackTime = 1200;
        M2Config.dwDummyWarrorWalkTime = 500;
        M2Config.dwDummyWizardWalkTime = 500;
        M2Config.dwDummyTaoistWalkTime = 500;

        // 回血/回蓝段（M2Share.pas:5507-5529）
        M2Config.boDummyAutoAddHP = true;
        M2Config.nDummyAddHPPercent = 60;
        M2Config.boDummyAutoAddMP = true;
        M2Config.nDummyAddMPPercent = 60;
        M2Config.nDummyHPTime_Warrior = 350;
        M2Config.nDummyHPTime_DF = 350;
        M2Config.nDummyHeroHPTime_Warrior = 350;
        M2Config.nDummyHeroHPTime_DF = 350;
        M2Config.nDummyMPTime_Warrior = 800;
        M2Config.nDummyMPTime_DF = 800;
        M2Config.nDummyHeroMPTime_Warrior = 800;
        M2Config.nDummyHeroMPTime_DF = 800;
        M2Config.nDummyHPBase_Warrior = 75;
        M2Config.nDummyHPBase_DF = 75;
        M2Config.nDummyHeroHPBase_Warrior = 75;
        M2Config.nDummyHeroHPBase_DF = 75;
        M2Config.nDummyMPBase_Warrior = 18;
        M2Config.nDummyMPBase_DF = 18;
        M2Config.nDummyHeroMPBase_Warrior = 18;
        M2Config.nDummyHeroMPBase_DF = 18;

        // 无头 UI 接缝复位
        DummySettingMessageBoxSeam.UiEnabled = false;
        DummySettingMessageBoxSeam.ShowModalHandler = null;
        DummySettingMessageBoxSeam.ShowModalCount = 0;
        DummySettingMessageBoxSeam.LastModalResult = 0;

        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
    }

    public void Dispose()
    {
        StaRunner.New(() => Form.Dispose());
        ResetStatics();
        try { Directory.Delete(Dir, true); } catch { /* best effort */ }
    }

    /// <summary>
    /// 调用 `FormCreate`（原文 `OnCreate`）并在调用前清掉"回填触发的连带写入"，
    /// 便于断言 `FormCreate` **本身**的行为。
    /// </summary>
    protected void DoFormCreate()
    {
        Form.FormCreate();
    }

    protected void ClearCaptures()
    {
        Messages.Clear();
        WroteBool.Clear();
        WroteInt.Clear();
        WroteStr.Clear();
        FocusProbes.Clear();
        LogonsAdded.Clear();
        WroteFiles.Clear();
    }

    /// <summary>设置 `lstDummyList` 的选中集合（WinForms ListBox 的选中 API）。</summary>
    protected void SelectDummyList(params int[] indices)
    {
        Form.Ct.lstDummyList.ClearSelected();
        foreach (int i in indices)
            Form.Ct.lstDummyList.SetSelected(i, true);
    }

    protected void SelectMapList(params int[] indices)
    {
        Form.Ct.lstMapList.ClearSelected();
        foreach (int i in indices)
            Form.Ct.lstMapList.SetSelected(i, true);
    }

    protected void SelectMonList(params int[] indices)
    {
        Form.Ct.lstMonList.ClearSelected();
        foreach (int i in indices)
            Form.Ct.lstMonList.SetSelected(i, true);
    }

    /// <summary>构造一张地图（原文 `TEnvirnoment.sMapName`）。</summary>
    protected static TEnvirnoment Env(string name) => new() { sMapName = name };

    /// <summary>
    /// 装载窗体：换新实例 + 给定地图/怪物表 + 跑 `FormCreate`。
    /// </summary>
    protected void OpenWith(IEnumerable<string>? maps = null, IEnumerable<string>? monsters = null)
    {
        Recreate();
        MapTable = new List<TEnvirnoment>();
        foreach (var m in maps ?? Array.Empty<string>())
            MapTable.Add(Env(m));
        MonsterTable = new List<(string, byte)>();
        foreach (var m in monsters ?? Array.Empty<string>())
            MonsterTable.Add((m, 0));
        Form.FindMapHandler = name => MapTable.Find(e => e.sMapName == name);
        Form.MapListHandler = () => MapTable;
        Form.MonsterListHandler = () => MonsterTable;
        DoFormCreate();
    }
}
