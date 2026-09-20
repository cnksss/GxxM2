using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.GamePets;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 开关 / Spin / 文本处理器族 1:1 覆盖（uFrmMainGamePets.pas:700-828、:1184-1350，
/// 共 36 个处理器）。每个处理器形状统一：`if not boOpened then Exit` → 写 g_Config → `ModValue`。
/// 差异断言重点：
///   · `boOpened` 闸门（未打开时既不写配置也不置脏）
///   · `chkPetPickupToMasterClick` 额外联动 `chkPetPickupFullToMaster.Enabled`
///   · `sePetNameColorChange` / `sePetPickupRangeChange` 的显式 `Byte(...)` 截断
///   · `edtPetSuffixNameChange` 的 `Trim`
///   · 每个处理器只写**自己那一个**字段（防错接线）
/// </summary>
[Collection("GamePetsSerial")]
public sealed class GamePetsHandlerTests : GamePetsTestBase
{
    /// <summary>处理器名 → (控件名, 读配置函数, 写标记函数, 期望值)。</summary>
    public static TheoryData<string, string, bool> BoolHandlerCases() => new()
    {
        { nameof(GamePetsForm.chkOpenGamePet), nameof(GamePetsForm.chkOpenGamePet), true },
        { nameof(GamePetsForm.chkEnabledPetAttack), nameof(GamePetsForm.chkEnabledPetAttack), true },
        { nameof(GamePetsForm.chkDisableMonAttackPet), nameof(GamePetsForm.chkDisableMonAttackPet), true },
        { nameof(GamePetsForm.chkDisableAllAttackPet), nameof(GamePetsForm.chkDisableAllAttackPet), true },
        { nameof(GamePetsForm.chkEnabledPetPickup), nameof(GamePetsForm.chkEnabledPetPickup), true },
        { nameof(GamePetsForm.chkPetOnlyPickMonsterItem), nameof(GamePetsForm.chkPetOnlyPickMonsterItem), true },
        { nameof(GamePetsForm.chkPetPickupToMaster), nameof(GamePetsForm.chkPetPickupToMaster), true },
        { nameof(GamePetsForm.chkPetPickupFullToMaster), nameof(GamePetsForm.chkPetPickupFullToMaster), true },
        { nameof(GamePetsForm.chkPetQuickPickup), nameof(GamePetsForm.chkPetQuickPickup), true },
        { nameof(GamePetsForm.chkPetRangePickup), nameof(GamePetsForm.chkPetRangePickup), true },
        { nameof(GamePetsForm.chkPetNoEntity), nameof(GamePetsForm.chkPetNoEntity), true },
        { nameof(GamePetsForm.chkPetSleepControlBySlave), nameof(GamePetsForm.chkPetSleepControlBySlave), true },
        { nameof(GamePetsForm.chkPetNoShowHPProgress), nameof(GamePetsForm.chkPetNoShowHPProgress), true },
        { nameof(GamePetsForm.chkEnablePetUseClientPickItems), nameof(GamePetsForm.chkEnablePetUseClientPickItems), true },
        { nameof(GamePetsForm.chkGamePetKillMonTrigger), nameof(GamePetsForm.chkGamePetKillMonTrigger), true },
        { nameof(GamePetsForm.chkCapturePetNeedItem), nameof(GamePetsForm.chkCapturePetNeedItem), true },
        { nameof(GamePetsForm.chkCaptureOKDecDura), nameof(GamePetsForm.chkCaptureOKDecDura), true },
        { nameof(GamePetsForm.chkPetShowMasterName), nameof(GamePetsForm.chkPetShowMasterName), true },
        { nameof(GamePetsForm.chkPetHPToMaster), nameof(GamePetsForm.chkPetHPToMaster), true },
        { nameof(GamePetsForm.chkPetDCToMaster), nameof(GamePetsForm.chkPetDCToMaster), true },
        { nameof(GamePetsForm.chkPetMCToMaster), nameof(GamePetsForm.chkPetMCToMaster), true },
        { nameof(GamePetsForm.chkPetSCToMaster), nameof(GamePetsForm.chkPetSCToMaster), true },
        { nameof(GamePetsForm.chkPetACToMaster), nameof(GamePetsForm.chkPetACToMaster), true },
        { nameof(GamePetsForm.chkPetMACToMaster), nameof(GamePetsForm.chkPetMACToMaster), true },
    };

    /// <summary>处理器名 → 该处理器写完后，只有它对应的 g_Config 字段变为 true。</summary>
    private static Func<bool> ReadFlag(string control) => control switch
    {
        nameof(GamePetsForm.chkOpenGamePet) => () => M2Config.boOpenGamePet,
        nameof(GamePetsForm.chkEnabledPetAttack) => () => M2Config.boEnabledPetAttack,
        nameof(GamePetsForm.chkDisableMonAttackPet) => () => M2Config.boDisableMonAttackPet,
        nameof(GamePetsForm.chkDisableAllAttackPet) => () => M2Config.boDisableAllAttackPet,
        nameof(GamePetsForm.chkEnabledPetPickup) => () => M2Config.boEnabledPetPickup,
        nameof(GamePetsForm.chkPetOnlyPickMonsterItem) => () => M2Config.boPetOnlyPickMonsterItem,
        nameof(GamePetsForm.chkPetPickupToMaster) => () => M2Config.boPetPickupToMaster,
        nameof(GamePetsForm.chkPetPickupFullToMaster) => () => M2Config.boPetPickupFullToMaster,
        nameof(GamePetsForm.chkPetQuickPickup) => () => M2Config.boPetQuickPickup,
        nameof(GamePetsForm.chkPetRangePickup) => () => M2Config.boPetRangePickup,
        nameof(GamePetsForm.chkPetNoEntity) => () => M2Config.boPetNoEntity,
        nameof(GamePetsForm.chkPetSleepControlBySlave) => () => M2Config.boPetSleepControlBySlave,
        nameof(GamePetsForm.chkPetNoShowHPProgress) => () => M2Config.boPetNoShowHPProgress,
        nameof(GamePetsForm.chkEnablePetUseClientPickItems) => () => M2Config.boEnablePetUseClientPickItems,
        nameof(GamePetsForm.chkGamePetKillMonTrigger) => () => M2Config.boGamePetKillMonTrigger,
        nameof(GamePetsForm.chkCapturePetNeedItem) => () => M2Config.boCapturePetNeedItem,
        nameof(GamePetsForm.chkCaptureOKDecDura) => () => M2Config.boCaptureOKDecDura,
        nameof(GamePetsForm.chkPetShowMasterName) => () => M2Config.boPetShowMasterName,
        nameof(GamePetsForm.chkPetHPToMaster) => () => M2Config.boPetHPToMaster,
        nameof(GamePetsForm.chkPetDCToMaster) => () => M2Config.boPetDCToMaster,
        nameof(GamePetsForm.chkPetMCToMaster) => () => M2Config.boPetMCToMaster,
        nameof(GamePetsForm.chkPetSCToMaster) => () => M2Config.boPetSCToMaster,
        nameof(GamePetsForm.chkPetACToMaster) => () => M2Config.boPetACToMaster,
        nameof(GamePetsForm.chkPetMACToMaster) => () => M2Config.boPetMACToMaster,
        _ => throw new ArgumentException(control),
    };

    [Theory]
    [MemberData(nameof(BoolHandlerCases))]
    public void BoolHandler_SetsItsOwnConfigFieldAndMarksDirty(string handler, string control, bool expected)
    {
        OpenWith();
        Assert.False(ReadFlag(control)());

        Form.RaiseParamHandler(handler, true);

        Assert.Equal(expected, ReadFlag(control)());
        Assert.True(Form.btnSavePet.Enabled);        // ModValue :667
        Assert.True(Form.btnSavePetParams.Enabled);  // ModValue :668
    }

    [Theory]
    [MemberData(nameof(BoolHandlerCases))]
    public void BoolHandler_TogglingBackToFalse_ClearsField(string handler, string control, bool _)
    {
        OpenWith();
        Form.RaiseParamHandler(handler, true);
        Form.RaiseParamHandler(handler, false);

        Assert.False(ReadFlag(control)());
    }

    [Theory]
    [MemberData(nameof(BoolHandlerCases))]
    public void BoolHandler_BeforeOpen_BoOpenedGateBlocksWriteAndModValue(string handler, string control, bool _)
    {
        // 未 Open 的窗体：boOpened == false → :702-703 早退，既不写配置也不置脏。
        using var fresh = new FreshForm();
        fresh.Form.RaiseParamHandler(handler, true);

        Assert.False(ReadFlag(control)());
        Assert.False(fresh.Form.btnSavePet.Enabled);
        Assert.False(fresh.Form.btnSavePetParams.Enabled);
    }

    [Fact]
    public void BoolHandlers_AreNotCrossWired_EachWritesOnlyItsOwnField()
    {
        // 防错接线：改一个开关，只有它对应的字段变化（其余 23 个必须保持 false）。
        OpenWith();
        Form.RaiseParamHandler(nameof(Form.chkPetMACToMaster), true);

        foreach (string c in AllBoolControls())
        {
            bool v = ReadFlag(c)();
            if (c == nameof(Form.chkPetMACToMaster))
                Assert.True(v, c + " 应被置为 true");
            else
                Assert.False(v, c + " 不应被改动");
        }
    }

    /// <summary>全部 24 个布尔开关控件名（来自 DFM 事件绑定核对）。</summary>
    private static List<string> AllBoolControls() => BoolHandlerCases()
        .Select(r => (string)r[0]!)
        .ToList();

    // ---------- chkPetPickupToMasterClick 的额外联动（:756-764） ----------

    [Fact]
    public void PickupToMasterClick_AlsoSetsFullToMasterEnabledToInverse()
    {
        OpenWith();
        Form.RaiseParamHandler(nameof(Form.chkPetPickupToMaster), true);

        Assert.True(M2Config.boPetPickupToMaster);
        Assert.False(Form.chkPetPickupFullToMaster.Enabled);     // :763 not True

        Form.RaiseParamHandler(nameof(Form.chkPetPickupToMaster), false);

        Assert.False(M2Config.boPetPickupToMaster);
        Assert.True(Form.chkPetPickupFullToMaster.Enabled);      // :763 not False
    }

    [Fact]
    public void PickupFullToMasterClick_DoesNotTouchTheOtherFlag()
    {
        // 差异断言：FullToMaster 处理器（:766-772）**没有**反向联动。
        OpenWith();
        Form.RaiseParamHandler(nameof(Form.chkPetPickupFullToMaster), true);

        Assert.True(M2Config.boPetPickupFullToMaster);
        Assert.False(M2Config.boPetPickupToMaster);
        Assert.True(Form.chkPetPickupFullToMaster.Enabled);       // 未被改成 false
    }

    // ---------- Spin 处理器族 ----------

    [Theory]
    [InlineData(nameof(GamePetsForm.sePetAbilToMasterRate), 123)]
    [InlineData(nameof(GamePetsForm.seGamePetMaxCount), 7)]
    [InlineData(nameof(GamePetsForm.seGamePetNameCount), 8)]
    [InlineData(nameof(GamePetsForm.seGamePetRecallTime), 9)]
    [InlineData(nameof(GamePetsForm.sePetHighLevel), 11)]
    [InlineData(nameof(GamePetsForm.sePetHighLevelGetExp), 12)]
    [InlineData(nameof(GamePetsForm.sePetBaseExp), 13)]
    [InlineData(nameof(GamePetsForm.sePetAddExp), 14)]
    public void IntSpinHandlers_WriteTheirOwnConfigFieldAndMarkDirty(string control, int value)
    {
        OpenWith();

        Form.RaiseSpinHandler(control, value);

        Assert.Equal(value, ReadIntFlag(control));
        Assert.True(Form.btnSavePetParams.Enabled);
    }

    private static int ReadIntFlag(string control) => control switch
    {
        nameof(GamePetsForm.sePetAbilToMasterRate) => M2Config.nPetAbilToMasterRate,
        nameof(GamePetsForm.seGamePetMaxCount) => M2Config.nGamePetMaxCount,
        nameof(GamePetsForm.seGamePetNameCount) => M2Config.nGamePetNameCount,
        nameof(GamePetsForm.seGamePetRecallTime) => M2Config.nGamePetRecallTime,
        nameof(GamePetsForm.sePetHighLevel) => M2Config.nPetHighLevel,
        nameof(GamePetsForm.sePetHighLevelGetExp) => M2Config.nPetHighLevelGetExp,
        nameof(GamePetsForm.sePetBaseExp) => M2Config.nPetBaseExp,
        nameof(GamePetsForm.sePetAddExp) => M2Config.nPetAddExp,
        _ => throw new ArgumentException(control),
    };

    [Fact]
    public void UseItemIntervalTime_ZeroAndMaxAndOverflowBoundary()
    {
        OpenWith();

        Form.RaiseSpinHandler(nameof(Form.sePetUseItemIntervalTime), 0);
        Assert.Equal(0u, M2Config.dwPetUseItemIntervalTime);

        Form.RaiseSpinHandler(nameof(Form.sePetUseItemIntervalTime), int.MaxValue);
        Assert.Equal((uint)int.MaxValue, M2Config.dwPetUseItemIntervalTime);  // 只到 int.MaxValue 上限
    }

    [Fact]
    public void NameColor_ByteTruncation_255Boundary()
    {
        // 差异断言：:1236 `Byte(sePetNameColor.Value)` —— 显式截断为 byte。
        // 控件最大值也是 255，故边界即 255；断言类型与值都对。
        OpenWith();

        Form.RaiseSpinHandler(nameof(Form.sePetNameColor), 0);
        Assert.Equal((byte)0, M2Config.btPetNameColor);

        Form.RaiseSpinHandler(nameof(Form.sePetNameColor), 255);
        Assert.Equal((byte)255, M2Config.btPetNameColor);
        Assert.IsType<byte>(M2Config.btPetNameColor);
    }

    [Fact]
    public void PickupRange_ByteTruncation_255Boundary()
    {
        // :1332 `btPetPickupRange := sePetPickupRange.Value`（字段是 Byte）
        OpenWith();

        Form.RaiseSpinHandler(nameof(Form.sePetPickupRange), 255);
        Assert.Equal((byte)255, M2Config.btPetPickupRange);

        Form.RaiseSpinHandler(nameof(Form.sePetPickupRange), 0);
        Assert.Equal((byte)0, M2Config.btPetPickupRange);
    }

    [Fact]
    public void SpinHandler_BeforeOpen_IsBlocked()
    {
        using var fresh = new FreshForm();
        fresh.Form.RaiseSpinHandler(nameof(GamePetsForm.seGamePetMaxCount), 50);

        Assert.Equal(0, M2Config.nGamePetMaxCount);
        Assert.False(fresh.Form.btnSavePetParams.Enabled);
    }

    // ---------- edtPetSuffixNameChange（:1240-1246） ----------

    [Fact]
    public void SuffixName_IsTrimmedBeforeStoring()
    {
        OpenWith();

        Form.RaiseSuffixNameChanged("  的宝宝  ");

        Assert.Equal("的宝宝", M2Config.sPetSuffixName);
        Assert.True(Form.btnSavePetParams.Enabled);
    }

    [Fact]
    public void SuffixName_WhitespaceOnly_BecomesEmpty()
    {
        OpenWith();

        Form.RaiseSuffixNameChanged("    ");

        Assert.Equal("", M2Config.sPetSuffixName);
    }

    [Fact]
    public void SuffixName_Empty_StaysEmpty()
    {
        M2Config.sPetSuffixName = "旧值";
        OpenWith();

        Form.RaiseSuffixNameChanged("");

        Assert.Equal("", M2Config.sPetSuffixName);
    }

    [Fact]
    public void SuffixName_ChineseAndMixedCase_PreservedAfterTrim()
    {
        OpenWith();

        Form.RaiseSuffixNameChanged(" 的Baby宝宝 ");

        Assert.Equal("的Baby宝宝", M2Config.sPetSuffixName);
    }

    [Fact]
    public void SuffixName_BeforeOpen_IsBlocked()
    {
        M2Config.sPetSuffixName = "原值";
        using var fresh = new FreshForm();
        fresh.Form.RaiseSuffixNameChanged("新值");

        Assert.Equal("原值", M2Config.sPetSuffixName);
    }

    // ---------- 未知控件名防御 ----------

    [Fact]
    public void RaiseParamHandler_UnknownName_Throws()
    {
        OpenWith();
        Assert.Throws<ArgumentException>(() => Form.RaiseParamHandler("不存在的控件", true));
    }

    [Fact]
    public void RaiseSpinHandler_UnknownName_Throws()
    {
        OpenWith();
        Assert.Throws<ArgumentException>(() => Form.RaiseSpinHandler("不存在的控件", 1));
    }
}

/// <summary>辅助：一个未 Open（boOpened == false）的窗体，用于断言闸门分支。</summary>
public sealed class FreshForm : IDisposable
{
    public readonly GamePetsForm Form;

    public FreshForm()
    {
        Form = StaRunner.New(() => new GamePetsForm());
        Form.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        Form.WriteBoolHandler = (_, _) => { };
        Form.WriteIntegerHandler = (_, _) => { };
        Form.WriteStringHandler = (_, _) => { };
        Form.ExpConfigWriteStringHandler = (_, _, _) => { };
        Form.SetFocusProbe = _ => { };
    }

    public void Dispose() => StaRunner.New(() => Form.Dispose());
}
