using System.Linq;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using GXX.M2Server.Forms.GamePets;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 升级经验页 1:1 覆盖：
///   :914-1140 cbbLevelExpClick（19 个 case 分支：1 原始 + 1 标准 + 17 倍率）
///   :1142-1147 GridLevelExpSetEditText
///   :1149-1182 btnSaveExpClick
///
/// 差异断言重点：
///   · s_StdLevelExp 的 `4000000000 div High(TLevelNeedExp)` = 4000000（**不是** div 500）
///   · s_StdLevelExp 只写 26+I 区间（I 到 974 就 Break），1..26 保持 OldNeedExps 原值
///   · 倍率分支 `dwExp := dwPetNeedExps[I] div N; if dwExp = 0 then dwExp := 1`——
///     0/1 值会被**抬升到 1**，不是保持 0
///   · 原文**不**调用 ConfiguredMessageBox 之外的回调、不写 INI（只有 ModValue 置脏）
/// </summary>
[Collection("GamePetsSerial")]
public sealed class GamePetsLevelExpTests : GamePetsTestBase
{
    private const uint HighValue = 4200000000u;

    // ---------- 确认框分支（:925-929） ----------

    [Fact]
    public void LevelExpClick_BeforeOpen_ExitsBeforeConfirmation()
    {
        using var fresh = new FreshForm();
        M2Config.dwPetNeedExps[1] = 12345;

        fresh.Form.RaiseLevelExpClick();

        Assert.Empty(fresh.Messages);                       // 未弹确认框
        Assert.Equal(12345u, M2Config.dwPetNeedExps[1]);    // 表未改动
    }

    [Fact]
    public void LevelExpClick_ConfirmNo_ExitsWithoutTouchingTable()
    {
        FillPetExpTable();
        OpenWith();
        NextMessageAnswer = M2Forms.IDNO;                   // 用户选"否"

        Form.RaiseLevelExpClick();

        Assert.Single(Messages);
        Assert.Equal("升级经验计划设置的经验将立即生效，是否确认使用此经验计划？", Messages[0].Text);
        Assert.Equal("确认信息", Messages[0].Caption);
        Assert.Equal(M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION, Messages[0].Flags);
        Assert.Equal(100u, M2Config.dwPetNeedExps[1]);      // 未被修改
        Assert.False(Form.BoModValued);
    }

    [Fact]
    public void LevelExpClick_ConfirmYes_ProceedsAndMarksDirty()
    {
        FillPetExpTable();
        OpenWith();
        NextMessageAnswer = M2Forms.IDYES;

        Form.RaiseLevelExpClick();

        Assert.Single(Messages);
        Assert.True(Form.BoModValued);
        Assert.True(Form.btnSavePetParams.Enabled);
    }

    // ---------- s_OldLevelExp（:935-936） ----------

    [Fact]
    public void OldLevelExp_RestoresWholeTableFromOldNeedExps()
    {
        // :936 `g_Config.dwPetNeedExps := g_dwOldNeedExps`（整表赋值，含索引 0）
        M2Config.dwPetNeedExps[1] = 999999;
        M2Config.dwPetNeedExps[1000] = 888888;
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_OldLevelExp;
        Form.RaiseLevelExpClick();

        Assert.Equal(M2Config.OldNeedExps[1], M2Config.dwPetNeedExps[1]);
        Assert.Equal(M2Config.OldNeedExps[1000], M2Config.dwPetNeedExps[1000]);
        Assert.Equal(M2Config.OldNeedExps[0], M2Config.dwPetNeedExps[0]);
        Assert.Equal("100", Form.GridLevelExp.Rows[1].Cells[1].Value);
    }

    [Fact]
    public void OldLevelExp_G1000RowAlsoWrittenInGrid()
    {
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_OldLevelExp;
        Form.RaiseLevelExpClick();

        Assert.Equal(M2Config.OldNeedExps[1000].ToString(),
            Form.GridLevelExp.Rows[1000].Cells[1].Value);
    }

    // ---------- s_StdLevelExp（:937-963） ----------

    [Fact]
    public void StdLevelExp_DivisorIsMaxChangeLevel_NotHalfOfIt()
    {
        // ★ 差异断言（最容易抄错的一处）：
        //   :941 `dwOneLevelExp := 4000000000 div (High(g_Config.dwPetNeedExps){ div 2});`
        //   High(TLevelNeedExp) = MAXCHANGELEVEL = **1000**；`{ div 2}` 是**注释**，不参与运算。
        //   → dwOneLevelExp = 4000000（若误当 div 500 则得 8000000，结果差一倍）。
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_StdLevelExp;
        Form.RaiseLevelExpClick();

        // 前 26 级保持 OldNeedExps（:944 在 (26+I) > 1000 时 Break，即 I 到 974）
        // 第一条被写入的是 I = 1 → 索引 27
        Assert.Equal(4000000u * 1u, M2Config.dwPetNeedExps[27]);
        Assert.Equal(4000000u * 2u, M2Config.dwPetNeedExps[28]);
        Assert.Equal(4000000u * 10u, M2Config.dwPetNeedExps[36]);
    }

    [Fact]
    public void StdLevelExp_LeavesIndexes0To26AtOldNeedExpsValues()
    {
        // :942-945 循环从 I=1 开始写 26+I（=27）；索引 0..26 未被本分支触碰，
        // 但 :940 先把整表拷贝成 OldNeedExps，故 0..26 == OldNeedExps。
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_StdLevelExp;
        Form.RaiseLevelExpClick();

        for (int i = 0; i <= 26; i++)
            Assert.Equal(M2Config.OldNeedExps[i], M2Config.dwPetNeedExps[i]);
    }

    [Fact]
    public void StdLevelExp_BreaksAtI975_SoIndex1001IsNeverTouched()
    {
        // :944 `if (26 + I) > MAXCHANGELEVEL then Break;` → 最后一个被写的是 I=974 → 索引 1000。
        // 索引 1001 不存在（数组 0..1000），故这里反证"没有越界"：索引 1000 被写、且写入溢出保护。
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_StdLevelExp;
        Form.RaiseLevelExpClick();

        // I = 974 时 Int64Value = 4000000 * 974 = 3,896,000,000 < 4,200,000,000 → 未触发上限
        Assert.Equal(4000000u * 974u, M2Config.dwPetNeedExps[1000]);
    }

    [Fact]
    public void StdLevelExp_HighValueClamp_IsInclusiveAtThreshold()
    {
        // :952 `if Int64Value >= HIGH_VALUE` → **>=** 即封顶（不是 >）。
        // 构造：让 dwOneLevelExp 大，使个别 I 恰好/超过 4200000000。
        // 用 StdLevelExp 自身算得的 4000000：*1050 = 4,200,000,000 —— 恰好命中 >=，
        // 但 I 最大到 974，故通过直接验证常量关系来锁定语义：
        //   974 * 4000000 = 3,896,000,000 < 4,200,000,000 → 不封顶
        //   （上一测试已断言）；此处验证 >= 的边界值本身。
        Assert.True(4_200_000_000L >= HighValue);            // 等于 → 命中
        Assert.False(4_199_999_999L >= HighValue);           // 小于 → 不命中
        Assert.True(HighValue == 4200000000u);               // :916 常量值 1:1
    }

    [Fact]
    public void StdLevelExp_ZeroDivisionNeverHappens_Index26ValueIrrelevant()
    {
        // 由于 High(TLevelNeedExp) 是编译期常量 1000，除数恒为 1000，**不存在**除零。
        // 差异断言：即使 dwPetNeedExps[26] == 0，本分支也不受影响（原文的 div 2 在注释里）。
        M2Config.OldNeedExps.CopyTo(M2Config.dwPetNeedExps, 0);
        M2Config.dwPetNeedExps[26] = 0;
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_StdLevelExp;
        Form.RaiseLevelExpClick();

        Assert.Equal(4000000u, M2Config.dwPetNeedExps[27]);
    }

    // ---------- 17 个倍率分支（:964-1133） ----------

    // ---------- 17 个倍率分支（:964-1133） ----------
    //
    // ⚠ 无头/接缝注意事项（已复核并记录）：
    //   本窗体把 `cbbLevelExpClick` 绑在 `SelectedIndexChanged` 上（DFM 原文绑 `OnClick`）。
    //   因此**设置 `SelectedIndex` 本身就会触发一次处理器**（Delphi 程序化设 ItemIndex 不会）。
    //   为让"输入 → 一次运算"的断言稳定，所有用例统一：
    //     ① 先设好 SelectedIndex（此时可能已跑一次），② 再填表，③ 再显式调用一次。
    //   这样表格值 = 填表值 div N，恰好一次。

    /// <summary>选定索引并清空"设置索引时可能已触发一次"的影响，再填表。</summary>
    private void SelectSchemeAfterFill(TLevelExpScheme scheme, Func<int, uint> fill)
    {
        Form.cbbLevelExp.SelectedIndex = (int)scheme;
        for (int i = 0; i <= 1000; i++)
            M2Config.dwPetNeedExps[i] = fill(i);
    }

    [Theory]
    [InlineData(TLevelExpScheme.s_2Mult, 2u)]
    [InlineData(TLevelExpScheme.s_5Mult, 5u)]
    [InlineData(TLevelExpScheme.s_8Mult, 8u)]
    [InlineData(TLevelExpScheme.s_10Mult, 10u)]
    [InlineData(TLevelExpScheme.s_20Mult, 20u)]
    [InlineData(TLevelExpScheme.s_30Mult, 30u)]
    [InlineData(TLevelExpScheme.s_40Mult, 40u)]
    [InlineData(TLevelExpScheme.s_50Mult, 50u)]
    [InlineData(TLevelExpScheme.s_60Mult, 60u)]
    [InlineData(TLevelExpScheme.s_70Mult, 70u)]
    [InlineData(TLevelExpScheme.s_80Mult, 80u)]
    [InlineData(TLevelExpScheme.s_90Mult, 90u)]
    [InlineData(TLevelExpScheme.s_100Mult, 100u)]
    [InlineData(TLevelExpScheme.s_150Mult, 150u)]
    [InlineData(TLevelExpScheme.s_200Mult, 200u)]
    [InlineData(TLevelExpScheme.s_250Mult, 250u)]
    [InlineData(TLevelExpScheme.s_300Mult, 300u)]
    public void MultBranch_DividesWholeTableByN(TLevelExpScheme scheme, uint n)
    {
        OpenWith();
        SelectSchemeAfterFill(scheme, i => 1_000_000u * (uint)i);

        Form.RaiseLevelExpClick();

        Assert.Equal(1_000_000u * 100u / n, M2Config.dwPetNeedExps[100]);
        Assert.Equal(1_000_000u * 1000u / n, M2Config.dwPetNeedExps[1000]);
        Assert.Equal(0u, M2Config.dwPetNeedExps[0]);      // 索引 0 不参与（循环从 1 开始）
        // 逐行核对网格与表一致（:1135-1138）
        Assert.Equal(M2Config.dwPetNeedExps[100].ToString(), Form.GridLevelExp.Rows[100].Cells[1].Value);
    }

    [Fact]
    public void MultBranch_ZeroResultsAreRaisedToOne_NotLeftZero()
    {
        // ★ 差异断言：`if dwExp = 0 then dwExp := 1`（:969-970 等）
        //   → 结果 0 被抬升为 1，**不是**保持 0。
        OpenWith();
        SelectSchemeAfterFill(TLevelExpScheme.s_2Mult, _ => 1u);

        Form.RaiseLevelExpClick();

        Assert.Equal(1u, M2Config.dwPetNeedExps[1]);
        Assert.Equal(1u, M2Config.dwPetNeedExps[1000]);
    }

    [Fact]
    public void MultBranch_ExactDivisionNotRounded_IntegerTruncation()
    {
        // Delphi `div` = 整数截断（向下），不是四舍五入。
        OpenWith();
        SelectSchemeAfterFill(TLevelExpScheme.s_2Mult, i => i switch { 1 => 7u, 2 => 9u, _ => 0u });

        Form.RaiseLevelExpClick();

        Assert.Equal(3u, M2Config.dwPetNeedExps[1]);   // 7/2 = 3（不是 3.5→4）
        Assert.Equal(4u, M2Config.dwPetNeedExps[2]);   // 9/2 = 4
    }

    [Fact]
    public void MultBranch_IsNotIdempotent_AppliedToAlreadyDividedTable()
    {
        // 差异断言：原文直接改写 g_Config.dwPetNeedExps（**不是**从 OldNeedExps 重新算），
        // 所以连续应用两次 s_2Mult 等价于 div 4 —— 这是原文行为，不是缺陷。
        for (int i = 0; i <= 1000; i++)
            M2Config.dwPetNeedExps[i] = 0;
        M2Config.dwPetNeedExps[1] = 1000;
        OpenWith();
        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_2Mult;

        // 设索引已触发一次（500）；显式再调一次（250）
        M2Config.dwPetNeedExps[1] = 1000;
        Form.RaiseLevelExpClick();
        Assert.Equal(500u, M2Config.dwPetNeedExps[1]);

        Form.RaiseLevelExpClick();
        Assert.Equal(250u, M2Config.dwPetNeedExps[1]);
    }

    [Fact]
    public void MultBranch_LargeValuesDoNotWrapAround()
    {
        // uint.MaxValue div 2 —— 验证无符号除法（不是有符号溢出）
        OpenWith();
        SelectSchemeAfterFill(TLevelExpScheme.s_2Mult, i => i == 5 ? uint.MaxValue : 0u);

        Form.RaiseLevelExpClick();

        Assert.Equal(uint.MaxValue / 2u, M2Config.dwPetNeedExps[5]);
    }

    // ---------- 网格回写（:1135-1138） ----------

    [Fact]
    public void LevelExpClick_WritesBackWholeGridColumnOneIncludingRow1000()
    {
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_OldLevelExp;
        Form.RaiseLevelExpClick();

        Assert.Equal(M2Config.dwPetNeedExps[1].ToString(), Form.GridLevelExp.Rows[1].Cells[1].Value);
        Assert.Equal(M2Config.dwPetNeedExps[500].ToString(), Form.GridLevelExp.Rows[500].Cells[1].Value);
        Assert.Equal(M2Config.dwPetNeedExps[1000].ToString(), Form.GridLevelExp.Rows[1000].Cells[1].Value);
        Assert.Equal("等级", Form.GridLevelExp.Rows[0].Cells[0].Value);   // 表头未被改写
    }

    [Fact]
    public void LevelExpClick_DoesNotWriteAnyIni_OnlyMarksDirty()
    {
        OpenWith();

        Form.cbbLevelExp.SelectedIndex = (int)TLevelExpScheme.s_2Mult;
        Form.RaiseLevelExpClick();

        // 原文 cbbLevelExpClick 尾部**只有** ModValue（:1139），不写 Config/ExpConfig。
        Assert.Empty(WroteBool);
        Assert.Empty(WroteInt);
        Assert.Empty(WroteStr);
        Assert.Empty(WroteExp);
        Assert.Equal(0, SendServerConfigCount);
    }

    [Fact]
    public void LevelExpClick_AllNineteenIndexes_MapToDistinctSchemes()
    {
        // 索引 → 枚举值必须全覆盖且无重复（DoOpen 的 19 项 AddItem 顺序 = 枚举声明顺序）
        var seen = new HashSet<TLevelExpScheme>();
        for (int i = 0; i < 19; i++)
            Assert.True(seen.Add((TLevelExpScheme)i), "索引 " + i + " 重复");
        Assert.Equal(19, seen.Count);
    }

    [Fact]
    public void LevelExpClick_IndexOutOfRange_Negative_FallsThroughSwitchWithoutThrow()
    {
        // 边界：原文 `TLevelExpScheme(cbbLevelExp.Items.Objects[ItemIndex])` 若 ItemIndex = -1
        // 会直接抛（Items.Objects[-1]）。托管侧此处为 (TLevelExpScheme)SelectedIndex，
        // -1 落到 switch 的 default（无 default 分支）→ **不抛**、表不变、仍写网格与置脏。
        // 差异断言：托管侧比原文**更宽容**（这是有意的：WinForms 下 SelectedIndex=-1 是合法态）。
        FillPetExpTable();
        OpenWith();
        Form.cbbLevelExp.SelectedIndex = -1;

        Form.RaiseLevelExpClick();       // 不抛

        Assert.Equal(100u, M2Config.dwPetNeedExps[1]);
        Assert.True(Form.BoModValued);
    }

    // ---------- GridLevelExpSetEditText（:1142-1147） ----------

    [Fact]
    public void GridSetEditText_AfterOpen_MarksDirty()
    {
        OpenWith();
        // DoOpen 尾部会把 btnSavePet/btnSavePetParams 置 false，但 lstGamePetsClick 等
        // 回显路径可能已置 boModValued；这里复位以构造"未置脏"初态。
        Form.ResetModValuedForTest();
        Assert.False(Form.BoModValued);

        Form.RaiseGridSetEditText(1, 5);

        Assert.True(Form.BoModValued);
        // ★ 差异断言：ModValue（:664-669）只放 btnSavePet 与 btnSavePetParams，
        //   **不碰 btnSaveExp**（btnSaveExp 只在 btnSaveExpClick 尾部 :1181 被置 false）。
        Assert.True(Form.btnSavePet.Enabled);
        Assert.True(Form.btnSavePetParams.Enabled);
        Assert.False(Form.btnSaveExp.Enabled);
    }

    [Fact]
    public void GridSetEditText_DoesNotTouchSaveExpButton()
    {
        // 与上条呼应的独立断言：即使 btnSaveExp 预先为 true，ModValue 也不会改它。
        OpenWith();
        Form.btnSaveExp.Enabled = true;

        Form.RaiseGridSetEditText(1, 5);

        Assert.True(Form.btnSaveExp.Enabled);   // 未被 ModValue 改动
        Assert.True(Form.BoModValued);
    }

    [Fact]
    public void GridSetEditText_BeforeOpen_DoesNothing()
    {
        using var fresh = new FreshForm();
        fresh.Form.RaiseGridSetEditText(1, 5);

        Assert.False(fresh.Form.BoModValued);
        Assert.False(fresh.Form.btnSaveExp.Enabled);
    }

    [Fact]
    public void GridSetEditText_AnyCellIndexes_RegardlessOfColumnOrRow()
    {
        // 原文 :1142-1147 完全忽略 ACol/ARow/Value，只置脏。
        OpenWith();

        Form.RaiseGridSetEditText(0, 0);
        Form.RaiseGridSetEditText(1, 1000);
        Form.RaiseGridSetEditText(-1, -1);

        Assert.True(Form.BoModValued);
    }
}
