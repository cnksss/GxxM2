using System.Linq;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.GamePets;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p6-m2-pets：`uFrmMainGamePets.pas`（1,353 行）→ `GamePetsForm` 1:1 转换测试。
///
/// ⚠ xUnit 串行化：本窗体族读写 `M2Config` 静态字段与 `GamePetsState.g_GamePetConfigList`
/// 静态列表（原文是单元级全局 var），与其它窗体族测试共享进程静态状态 →
/// 归入独立集合 `GamePetsSerial`（DisableParallelization）。
/// </summary>
[CollectionDefinition("GamePetsSerial", DisableParallelization = true)]
public sealed class GamePetsSerialCollection
{
}

/// <summary>
/// 无头 UI 处置：
///   · 所有 `Application.MessageBox` 走 `M2Forms.MessageBox`（既有 M2Server 接缝），
///     本测试基类统一注入 handler → **绝不** 弹真实模态框（否则挂死 testhost）。
///   · `Dialogs.InputQuery` 走 `InputQueryHandler` 注入。
///   · `ShowModal` 走 `DoOpen(showModal: false)`。
///   · 无法断言的焦点/光标定位走 `SetFocusProbe` / `GridLevelExpLocateRowHandler` 决策镜像。
/// </summary>
[Collection("GamePetsSerial")]
public abstract class GamePetsTestBase : IDisposable
{
    protected readonly string Dir;
    protected readonly GamePetsForm Form;

    /// <summary>捕获的弹窗（文本, 标题, flags）。</summary>
    protected readonly List<(string Text, string Caption, int Flags)> Messages = new();

    /// <summary>捕获的 `Config.WriteBool/WriteInteger/WriteString`（section 恒 'Setup'）。</summary>
    protected readonly List<(string Key, bool Value)> WroteBool = new();
    protected readonly List<(string Key, int Value)> WroteInt = new();
    protected readonly List<(string Key, string Value)> WroteStr = new();

    /// <summary>捕获的 `ExpConfig.WriteString(section, key, value)`。</summary>
    protected readonly List<(string Section, string Key, string Value)> WroteExp = new();

    /// <summary>捕获的 SetFocus 目标（原文无头不可断言）。</summary>
    protected readonly List<string> FocusProbes = new();

    /// <summary>捕获的 GridLevelExp 定位行（原文 :1161）。</summary>
    protected readonly List<int> GridLocatedRows = new();

    /// <summary>注入的下一次 InputQuery 结果。</summary>
    protected (bool Ok, string Value)? NextInputQuery;

    /// <summary>注入的下一次 MessageBox 返回值（null = 走默认 IDOK）。</summary>
    protected int? NextMessageAnswer;

    protected int SendServerConfigCount;

    protected GamePetsTestBase()
    {
        Dir = Path.Combine(Path.GetTempPath(), "gxx_p6pets_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        M2Config.sEnvirDir = Dir + "\\";

        ResetStatics();
        Form = StaRunner.New(() => new GamePetsForm());
        Wire(Form);
    }

    protected void Wire(GamePetsForm form)
    {
        form.MessageBoxHandler = (text, caption, flags) =>
        {
            Messages.Add((text, caption, flags));
            if (NextMessageAnswer.HasValue)
            {
                int a = NextMessageAnswer.Value;
                NextMessageAnswer = null;
                return a;
            }
            return M2Forms.IDOK;
        };
        form.WriteBoolHandler = (k, v) => WroteBool.Add((k, v));
        form.WriteIntegerHandler = (k, v) => WroteInt.Add((k, v));
        form.WriteStringHandler = (k, v) => WroteStr.Add((k, v));
        form.ExpConfigWriteStringHandler = (s, k, v) => WroteExp.Add((s, k, v));
        form.SendServerConfigHandler = () => SendServerConfigCount++;
        form.SetFocusProbe = t => FocusProbes.Add(t);
        form.GridLevelExpLocateRowHandler = r => GridLocatedRows.Add(r);
        form.InputQueryHandler = (_, _, _) => NextInputQuery ?? (false, "");
    }

    /// <summary>把全部 g_Config 宠物字段与经验表复位到"确定初值"，避免测试间串味。</summary>
    protected static void ResetStatics()
    {
        GamePetsState.ClearGamePetsConfig();

        M2Config.dwPetNeedExps = new uint[1001];
        M2Config.boPetUseFixExp = false;
        M2Config.nPetAddExp = 0;
        M2Config.nPetBaseExp = 0;
        M2Config.boPetHPToMaster = false;
        M2Config.boPetDCToMaster = false;
        M2Config.nPetAbilToMasterRate = 0;

        M2Config.boOpenGamePet = false;
        M2Config.boEnabledPetAttack = false;
        M2Config.boDisableMonAttackPet = false;
        M2Config.boDisableAllAttackPet = false;
        M2Config.boEnabledPetPickup = false;
        M2Config.boPetOnlyPickMonsterItem = false;
        M2Config.boPetPickupToMaster = false;
        M2Config.boPetPickupFullToMaster = false;
        M2Config.boPetQuickPickup = false;
        M2Config.boPetRangePickup = false;
        M2Config.btPetPickupRange = 0;
        M2Config.boPetNoEntity = false;
        M2Config.boPetSleepControlBySlave = false;
        M2Config.boPetNoShowHPProgress = false;
        M2Config.boEnablePetUseClientPickItems = false;
        M2Config.boPetMCToMaster = false;
        M2Config.boPetSCToMaster = false;
        M2Config.boPetACToMaster = false;
        M2Config.boPetMACToMaster = false;
        M2Config.dwPetUseItemIntervalTime = 0;
        M2Config.boCapturePetNeedItem = false;
        M2Config.boCaptureOKDecDura = false;
        M2Config.boPetShowMasterName = false;
        M2Config.btPetNameColor = 0;
        M2Config.sPetSuffixName = "";
        M2Config.nGamePetMaxCount = 0;
        M2Config.nGamePetNameCount = 0;
        M2Config.nGamePetRecallTime = 0;
        M2Config.boGamePetKillMonTrigger = false;
        M2Config.nPetHighLevel = 0;
        M2Config.nPetHighLevelGetExp = 0;
    }

    public void Dispose()
    {
        StaRunner.New(() => Form.Dispose());
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        ResetStatics();
        try { Directory.Delete(Dir, true); } catch { /* best effort */ }
    }

    /// <summary>把经验表填成可辨识值：dwPetNeedExps[i] = i * 100。</summary>
    protected static void FillPetExpTable()
    {
        for (int i = 0; i <= 1000; i++)
            M2Config.dwPetNeedExps[i] = (uint)(i * 100);
    }

    protected static (string sName, byte btRace) Mon(string name, byte race) => (name, race);

    /// <summary>造一个 DoOpen 用的怪物表：覆盖全部过滤分支。</summary>
    protected static List<(string sName, byte btRace)> MonsterTable() => new()
    {
        Mon("低于动物", Grobal2Const.RC_ANIMAL - 1),   // 49 → 被 btRace >= RC_ANIMAL 挡掉
        Mon("普通动物", Grobal2Const.RC_ANIMAL),        // 50 → 保留
        Mon("人形怪", Grobal2Const.RC_PLAYMOSTER),      // 150 → 排除
        Mon("弓箭手", Grobal2Const.RC_ARCHERGUARD),     // 112 → 排除
        Mon("巡回弓", Grobal2Const.RC_MOVE_ARCHERGUARD),// 142 → 排除
        Mon("押镖车", Grobal2Const.RC_TRUCKOBJECT),     // 128 → 排除
        Mon("练功师", 55),                              // 55 → 排除
        Mon("怪110", 110),                              // 110 → 排除
        Mon("沙巴克墙", 111),                            // 111 → 排除
        Mon("大怪", 200),                               // 200 → 保留
    };
}
