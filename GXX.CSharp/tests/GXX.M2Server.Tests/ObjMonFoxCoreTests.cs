using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J215：`ObjMon.pas` 中 `TFoxMonster`（狐狸）**前四个方法**的 1:1 测试
/// （`Create` / `Think` / `MagicAttackTarget` / `AttackTarget`，合计 205 行）。
/// 后两个方法（`WonderingEx` / `Run`）留待批次J216。
/// **本批最有价值的发现**：
/// ① 共享外层模板**第四次逐字确认**（30 行零差异）、且**跨了不同继承分支**；
/// ② `// 004A8B74` / `// 004A8E54` 这类反编译地址注释**并不唯一**、
///    同一地址被贴在三处不同类的方法上 => 不能当身份键。
/// </summary>
public sealed class ObjMonFoxCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5530, ObjMonFoxCore.CreateStart);
        Assert.Equal(5542, ObjMonFoxCore.CreateEnd);
        Assert.Equal(13, ObjMonFoxCore.CreateLines);
        Assert.Equal(5544, ObjMonFoxCore.ThinkStart);
        Assert.Equal(5579, ObjMonFoxCore.ThinkEnd);
        Assert.Equal(36, ObjMonFoxCore.ThinkLines);
        Assert.Equal(5580, ObjMonFoxCore.MagicStart);
        Assert.Equal(5680, ObjMonFoxCore.MagicEnd);
        Assert.Equal(101, ObjMonFoxCore.MagicLines);
        Assert.Equal(5682, ObjMonFoxCore.AttackStart);
        Assert.Equal(5736, ObjMonFoxCore.AttackEnd);
        Assert.Equal(55, ObjMonFoxCore.AttackLines);
        Assert.Equal(205, ObjMonFoxCore.TotalLines);
        Assert.Equal(400, ObjMonFoxCore.ClassTotalLines);

        Assert.Equal(5651, ObjMonFoxCore.OuterStart);
        Assert.Equal(5680, ObjMonFoxCore.OuterEnd);
        Assert.Equal(30, ObjMonFoxCore.OuterLines);
        Assert.Equal(4847, ObjMonFoxCore.J207OuterStart);
        Assert.Equal(4876, ObjMonFoxCore.J207OuterEnd);
        Assert.Equal(0, ObjMonFoxCore.TemplateDiffLines);
        Assert.Equal(4, ObjMonFoxCore.TemplateConfirmations);
        Assert.Equal(5582, ObjMonFoxCore.NestedStart);
        Assert.Equal(5649, ObjMonFoxCore.NestedEnd);
        Assert.Equal(68, ObjMonFoxCore.NestedLines);

        Assert.Equal(3, ObjMonFoxCore.LabelOccurrences);
        Assert.Equal(5563, ObjMonFoxCore.VmtLabelThisBatch);
        Assert.Equal(5856, ObjMonFoxCore.VmtLabelJ216);
        Assert.Equal(781, ObjMonFoxCore.Ffff4DeclLine);
        Assert.Equal(20, ObjMonFoxCore.FfebDeclLine);
        Assert.Equal(35478, ObjMonFoxCore.IsProperTargetImpl);
        Assert.Equal(35486, ObjMonFoxCore.IsProperTargetNilLine);

        Assert.Equal(5549, ObjMonFoxCore.ThinkGateLine);
        Assert.Equal(3000, ObjMonFoxCore.ThinkGateMs);
        Assert.Equal(5555, ObjMonFoxCore.ObjCountLine);
        Assert.Equal(2, ObjMonFoxCore.ObjCountThreshold);
        Assert.Equal(5559, ObjMonFoxCore.NpcCountLine);
        Assert.Equal(1, ObjMonFoxCore.NpcCountThreshold);
        Assert.Equal(5571, ObjMonFoxCore.WalkToLine);
        Assert.Equal(8, ObjMonFoxCore.WalkToRandomBound);
        Assert.Equal(1238, ObjMonFoxCore.AnimalThinkDeclLine);
        Assert.Equal(14, ObjMonFoxCore.MonsterThinkDeclLine);
        Assert.Equal(186, ObjMonFoxCore.FoxThinkDeclLine);

        Assert.Equal(5689, ObjMonFoxCore.MagicFlagLine);
        Assert.Equal(5693, ObjMonFoxCore.MagicDelegateLine);
        Assert.Equal(5712, ObjMonFoxCore.PhysCooldownLine);
        Assert.Equal(5716, ObjMonFoxCore.PhysFocusLine);
        Assert.Equal(5696, ObjMonFoxCore.MagicElseLine);
        Assert.Equal(5723, ObjMonFoxCore.PhysElseLine);
        Assert.Equal(4, ObjMonFoxCore.ElseOffset);
        Assert.Equal(8, ObjMonFoxCore.ElseBlockLines);

        Assert.Equal(5536, ObjMonFoxCore.ViewRangeLine);
        Assert.Equal(5, ObjMonFoxCore.ViewRange);
        Assert.Equal(250, ObjMonFoxCore.RunTime);
        Assert.Equal(3000, ObjMonFoxCore.SearchTimeBase);
        Assert.Equal(2000, ObjMonFoxCore.SearchTimeBound);
        Assert.Equal(5540, ObjMonFoxCore.RaceLine);
        Assert.Equal(80, ObjMonFoxCore.RaceServer);
        Assert.Equal(182, ObjMonFoxCore.FoxBo554DeclLine);
        Assert.Equal(19, ObjMonFoxCore.ClassesCovered);
        Assert.Equal(35, ObjMonFoxCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonFoxCore.SpanMatches());
        Assert.True(ObjMonFoxCore.TotalLinesAddUp());
        Assert.True(ObjMonFoxCore.MethodsAscending());
        Assert.True(ObjMonFoxCore.MethodsContiguous());
        Assert.True(ObjMonFoxCore.MonsterCreateSpanMatches());
        Assert.True(ObjMonFoxCore.TwoBatchesAddUp());
        Assert.True(ObjMonFoxCore.SplitAcrossTwoBatches());
        Assert.True(ObjMonFoxCore.WithinUnit());
        Assert.True(ObjMonFoxCore.NoInstrumentation());
    }

    // ===================== 一、共享外层模板 =====================

    [Fact]
    public void OuterTemplateFacts()
    {
        Assert.True(ObjMonFoxCore.OuterTemplateVerbatim());
        Assert.True(ObjMonFoxCore.ThirtyLinesZeroDiff());
        Assert.True(ObjMonFoxCore.FourthConfirmation());
        Assert.True(ObjMonFoxCore.SpansDifferentBases());
        Assert.True(ObjMonFoxCore.TemplateIsGeneric());
        Assert.True(ObjMonFoxCore.OuterTemplateExtracted());
        Assert.True(ObjMonFoxCore.OuterLengthsMatch());
        Assert.True(ObjMonFoxCore.BothAreThirty());
        Assert.True(ObjMonFoxCore.MagicDecompositionAddsUp());

        // **本类外层体与 J207 各 30 行、零差异**
        Assert.Equal(30, ObjMonFoxCore.OuterLines);
        Assert.Equal(30, ObjMonFoxCore.J207OuterEnd - ObjMonFoxCore.J207OuterStart + 1);
        Assert.Equal(0, ObjMonFoxCore.TemplateDiffLines);

        Assert.Equal(6, ObjMonFoxCore.OuterTemplate.Length);
    }

    // ===================== 二、反编译标签非唯一 =====================

    [Fact]
    public void AddressLabelFacts()
    {
        Assert.True(ObjMonFoxCore.AddressLabelNotUnique());
        Assert.True(ObjMonFoxCore.SameLabelThreeCreates());
        Assert.True(ObjMonFoxCore.SameLabelThreeThinks());
        Assert.True(ObjMonFoxCore.CopyPropagatedComments());
        Assert.True(ObjMonFoxCore.EvidenceOfSameOrigin());
        Assert.True(ObjMonFoxCore.NotAnIdentityKey());
        Assert.True(ObjMonFoxCore.CreateLabelTableExtracted());
        Assert.True(ObjMonFoxCore.ThinkLabelTableExtracted());
        Assert.True(ObjMonFoxCore.SameAddressDifferentClasses());

        // **`// 004A8B74` 贴在三处 Create 上**
        Assert.Equal(3, ObjMonFoxCore.Label004A8B74Lines.Length);
        Assert.Equal(551, ObjMonFoxCore.Label004A8B74Lines[0]);
        Assert.Equal(788, ObjMonFoxCore.Label004A8B74Lines[1]);
        Assert.Equal(5530, ObjMonFoxCore.Label004A8B74Lines[2]);

        // **`// 004A8E54` 贴在三处 Think 上**
        Assert.Equal(3, ObjMonFoxCore.Label004A8E54Lines.Length);
        Assert.Equal(577, ObjMonFoxCore.Label004A8E54Lines[0]);
        Assert.Equal(845, ObjMonFoxCore.Label004A8E54Lines[1]);
        Assert.Equal(5544, ObjMonFoxCore.Label004A8E54Lines[2]);
    }

    [Fact]
    public void VmtLabelFacts()
    {
        Assert.True(ObjMonFoxCore.VmtLabelInExpression());
        Assert.True(ObjMonFoxCore.SixOccurrences());
        Assert.True(ObjMonFoxCore.OriginatesFromDeclaration());
        Assert.True(ObjMonFoxCore.Ffff4ComesFromObjBase781());
        Assert.True(ObjMonFoxCore.FfebComesFromObjMon20());
        Assert.True(ObjMonFoxCore.VmtLabelTableExtracted());
        Assert.True(ObjMonFoxCore.SplitAcrossThisAndJ216());

        Assert.Equal(6, ObjMonFoxCore.VmtLabelLines.Length);
        Assert.Equal(587, ObjMonFoxCore.VmtLabelLines[0]);
        Assert.Equal(872, ObjMonFoxCore.VmtLabelLines[1]);
        Assert.Equal(992, ObjMonFoxCore.VmtLabelLines[2]);
        Assert.Equal(1198, ObjMonFoxCore.VmtLabelLines[3]);
        Assert.Equal(5563, ObjMonFoxCore.VmtLabelLines[4]);
        Assert.Equal(5856, ObjMonFoxCore.VmtLabelLines[5]);
    }

    [Fact]
    public void NilSafetyFacts()
    {
        Assert.True(ObjMonFoxCore.NoNilCheckAtCallSite());
        Assert.True(ObjMonFoxCore.SafeBecauseCalleeChecks());
        Assert.True(ObjMonFoxCore.CalleeGuardWithComment());
        Assert.True(ObjMonFoxCore.ContractSplitAcrossCallChain());
        Assert.True(ObjMonFoxCore.OppositeShapeToJ213());

        // **被调方 `IsProperTarget` 自带空指针与自指判断**
        Assert.True(ObjMonFoxCore.IsProperTargetSafe(false, false));
        Assert.False(ObjMonFoxCore.IsProperTargetSafe(true, false));
        Assert.False(ObjMonFoxCore.IsProperTargetSafe(false, true));
        Assert.True(ObjMonFoxCore.NullIsNotProper());
        Assert.True(ObjMonFoxCore.SelfIsNotProper());
    }

    // ===================== 三、Think 的实现族 =====================

    [Fact]
    public void ThinkDeclarationFacts()
    {
        Assert.True(ObjMonFoxCore.DeclarationWithoutBody());
        Assert.True(ObjMonFoxCore.StaticSoNoLinkDemand());
        Assert.True(ObjMonFoxCore.NeverCalledThroughBase());
        Assert.True(ObjMonFoxCore.VestigialDeclaration());
        Assert.True(ObjMonFoxCore.ThreeSameNamedStatics());
        Assert.True(ObjMonFoxCore.NotVirtualSoNoOverride());
        Assert.True(ObjMonFoxCore.NoVmtDispatch());
        Assert.True(ObjMonFoxCore.CallerGetsItsOwn());
        Assert.True(ObjMonFoxCore.VisibilityNarrows());
        Assert.True(ObjMonFoxCore.DeclLinesAscending());
    }

    [Fact]
    public void ThinkImplementationFamily()
    {
        Assert.True(ObjMonFoxCore.FiveThinkImplementations());
        Assert.True(ObjMonFoxCore.SameThreeSecondGate());
        Assert.True(ObjMonFoxCore.UnfoldedArithmetic());
        Assert.True(ObjMonFoxCore.CopiedNotSimplified());
        Assert.True(ObjMonFoxCore.ThinkImplsExtracted());

        Assert.Equal(5, ObjMonFoxCore.ThinkImpls.Length);
        Assert.Equal(3856, ObjMonFoxCore.ThinkImpls[0].Line);
        Assert.Contains("TSmartObject", ObjMonFoxCore.ThinkImpls[0].Owner);
        Assert.Equal(582, ObjMonFoxCore.ThinkImpls[1].Line);
        Assert.Equal("TDevilkingMonster", ObjMonFoxCore.ThinkImpls[1].Owner);
        Assert.Equal(850, ObjMonFoxCore.ThinkImpls[2].Line);
        Assert.Equal("TMonster", ObjMonFoxCore.ThinkImpls[2].Owner);
        Assert.Equal(5549, ObjMonFoxCore.ThinkImpls[3].Line);
        Assert.Equal("TFoxMonster", ObjMonFoxCore.ThinkImpls[3].Owner);
        Assert.Equal(2790, ObjMonFoxCore.ThinkImpls[4].Line);
    }

    [Fact]
    public void ThinkGateBoundaries()
    {
        Assert.True(ObjMonFoxCore.JustRefreshedBlocked());
        Assert.True(ObjMonFoxCore.ExactlyThreeSecondsBlocked());
        Assert.True(ObjMonFoxCore.OneOverPasses());

        // **严格大于 3000**
        Assert.False(ObjMonFoxCore.ThinkGateElapsed(1000, 1000));
        Assert.False(ObjMonFoxCore.ThinkGateElapsed(1000, 4000));
        Assert.True(ObjMonFoxCore.ThinkGateElapsed(1000, 4001));
    }

    [Fact]
    public void MixedTimeIdiomFacts()
    {
        Assert.True(ObjMonFoxCore.RawSubtractForThinkGate());
        Assert.True(ObjMonFoxCore.TickDiffForHitCooldown());
        Assert.True(ObjMonFoxCore.TwoIdiomsInSameClass());
    }

    [Fact]
    public void ThinkStructureFacts()
    {
        Assert.True(ObjMonFoxCore.GateCoversFirstStageOnly());
        Assert.True(ObjMonFoxCore.SecondStageEveryFrame());
        Assert.True(ObjMonFoxCore.ReasonableSplit());
        Assert.True(ObjMonFoxCore.ResultOnlyFromDisplacement());
        Assert.True(ObjMonFoxCore.Stage2OutsideGate());
    }

    [Fact]
    public void DupRouteFacts()
    {
        Assert.True(ObjMonFoxCore.TwoMutuallyExclusiveRoutes());
        Assert.True(ObjMonFoxCore.ObjectCountThresholdTwo());
        Assert.True(ObjMonFoxCore.NpcCountThresholdOne());
        Assert.True(ObjMonFoxCore.TwoCommentsExplain());
        Assert.True(ObjMonFoxCore.CompletesJ159LockThirty());
        Assert.True(ObjMonFoxCore.DupRoutesExtracted());

        Assert.Equal(2, ObjMonFoxCore.DupRoutes.Length);
        Assert.Contains("安全区", ObjMonFoxCore.DupRoutes[0].Comment);
        Assert.Contains("NPC", ObjMonFoxCore.DupRoutes[1].Comment);
    }

    [Fact]
    public void DupRouteBoundaries()
    {
        Assert.True(ObjMonFoxCore.PetInSafeZoneGoesRoute2());
        Assert.True(ObjMonFoxCore.NoMasterGoesRoute1());
        Assert.True(ObjMonFoxCore.NotInSafeZoneGoesRoute1());
        Assert.True(ObjMonFoxCore.NonPlayerMasterGoesRoute1());

        Assert.Equal("route2", ObjMonFoxCore.PickDupRoute(true, true, 0));
        Assert.Equal("route1", ObjMonFoxCore.PickDupRoute(false, true, 0));
        Assert.Equal("route1", ObjMonFoxCore.PickDupRoute(true, false, 0));
        Assert.Equal("route1", ObjMonFoxCore.PickDupRoute(true, true, 80));

        // **对象数阈值 2、NPC 数阈值 1**
        Assert.True(ObjMonFoxCore.NeedsDupByObjCount(2));
        Assert.False(ObjMonFoxCore.NeedsDupByObjCount(1));
        Assert.True(ObjMonFoxCore.NeedsDupByNpcCount(1));
        Assert.False(ObjMonFoxCore.NeedsDupByNpcCount(0));
        Assert.True(ObjMonFoxCore.TwoObjectsNeedsDup());
        Assert.True(ObjMonFoxCore.OneObjectDoesNot());
        Assert.True(ObjMonFoxCore.OneNpcNeedsDup());
        Assert.True(ObjMonFoxCore.ZeroNpcDoesNot());
    }

    [Fact]
    public void MasterSpellingFacts()
    {
        Assert.True(ObjMonFoxCore.MasterVsMMaster());
        Assert.True(ObjMonFoxCore.SameThingTwoSpellings());
        Assert.True(ObjMonFoxCore.MixedWithinOneLine());
    }

    [Fact]
    public void StickyFlagFacts()
    {
        Assert.True(ObjMonFoxCore.StickyFlag());
        Assert.True(ObjMonFoxCore.ClearedOnlyOnSuccess());
        Assert.True(ObjMonFoxCore.RetriesUntilDisplaced());
        Assert.True(ObjMonFoxCore.NotOneShot());
        Assert.True(ObjMonFoxCore.InitialisedFalse());
        Assert.True(ObjMonFoxCore.MovedReturnsTrue());
        Assert.True(ObjMonFoxCore.NotMovedReturnsFalse());

        Assert.True(ObjMonFoxCore.DidMove(1, 1, 2, 1));
        Assert.True(ObjMonFoxCore.DidMove(1, 1, 1, 2));
        Assert.False(ObjMonFoxCore.DidMove(1, 1, 1, 1));
    }

    [Fact]
    public void WalkDirectionFacts()
    {
        Assert.True(ObjMonFoxCore.Random8IsCorrect());
        Assert.True(ObjMonFoxCore.CoversAllEight());
        Assert.True(ObjMonFoxCore.ContrastWithWonderingExRandom9());
        Assert.True(ObjMonFoxCore.SameClassTwoSpellings());
        Assert.True(ObjMonFoxCore.AllEightValid());

        // **`Random(8)` 的值域 0..7 全部合法**
        for (int r = 0; r < 8; r++)
            Assert.True(ObjMonFoxCore.Random8InRange(r));

        Assert.False(ObjMonFoxCore.Random8InRange(8));
        Assert.False(ObjMonFoxCore.Random8InRange(-1));
    }

    // ===================== 四、AttackTarget =====================

    [Fact]
    public void AttackTargetFacts()
    {
        Assert.True(ObjMonFoxCore.ChecksMagicFlag());
        Assert.True(ObjMonFoxCore.TwoBranches());
        Assert.True(ObjMonFoxCore.OppositeOfJ212());
        Assert.True(ObjMonFoxCore.SelfImplementedDispatch());
        Assert.True(ObjMonFoxCore.SameElseTwice());
        Assert.True(ObjMonFoxCore.EightLinesDuplicated());
        Assert.True(ObjMonFoxCore.OnlyAttackActionDiffers());
        Assert.True(ObjMonFoxCore.ElseOffsetsMatch());
    }

    [Fact]
    public void DispatchBoundaries()
    {
        Assert.True(ObjMonFoxCore.FlagTrueMagic());
        Assert.True(ObjMonFoxCore.FlagFalsePhysical());
        Assert.Equal("magic", ObjMonFoxCore.Dispatch(true));
        Assert.Equal("physical", ObjMonFoxCore.Dispatch(false));
    }

    [Fact]
    public void ElseBlockStructure()
    {
        // **两处 `else` 到 `SetTargetXY` 的结构偏移都是 4**
        Assert.Equal(4,
            ObjMonFoxCore.ElseTargetLines[0] - ObjMonFoxCore.MagicElseLine);
        Assert.Equal(4,
            ObjMonFoxCore.ElseTargetLines[1] - ObjMonFoxCore.PhysElseLine);
        Assert.True(ObjMonFoxCore.ElseOffsetsMatch());

        Assert.Equal(5700, ObjMonFoxCore.ElseTargetLines[0]);
        Assert.Equal(5727, ObjMonFoxCore.ElseTargetLines[1]);
    }

    [Fact]
    public void CooldownLayerFacts()
    {
        Assert.True(ObjMonFoxCore.PhysicalHasOwnCooldown());
        Assert.True(ObjMonFoxCore.MagicDelegatesCooldown());
        Assert.True(ObjMonFoxCore.CooldownAtDifferentLayers());
        Assert.True(ObjMonFoxCore.MagicBranchNoCooldown());
        Assert.True(ObjMonFoxCore.PhysicalCooldownTriple());
        Assert.True(ObjMonFoxCore.FocusAfterTriple());
        Assert.True(ObjMonFoxCore.AttackAfterFocus());
    }

    [Fact]
    public void PhysicalCooldownBoundaries()
    {
        Assert.True(ObjMonFoxCore.PhysJustResetFalse());
        Assert.True(ObjMonFoxCore.PhysExactlyAtThresholdFalse());
        Assert.True(ObjMonFoxCore.PhysAfterThresholdTrue());

        Assert.False(ObjMonFoxCore.PhysCooldownElapsed(1000, 1000, 500, 0));
        Assert.False(ObjMonFoxCore.PhysCooldownElapsed(1000, 1500, 500, 0));
        Assert.True(ObjMonFoxCore.PhysCooldownElapsed(1000, 1501, 500, 0));
        Assert.True(ObjMonFoxCore.PhysCooldownElapsed(1000, 1600, 500, 0));
    }

    [Fact]
    public void WonderingFlagFacts()
    {
        Assert.True(ObjMonFoxCore.TwoSetters());
        Assert.True(ObjMonFoxCore.ThreeClearers());
        Assert.True(ObjMonFoxCore.SettersInThisBatch());
        Assert.True(ObjMonFoxCore.ClearersSplit());
        Assert.True(ObjMonFoxCore.EnablesJ216());
        Assert.True(ObjMonFoxCore.MagicSetAfterDelegate());
        Assert.True(ObjMonFoxCore.PhysSetAfterTriple());
        Assert.True(ObjMonFoxCore.SetPositionsDiffer());

        Assert.Equal(2, ObjMonFoxCore.WonderingSetLines.Length);
        Assert.Equal(3, ObjMonFoxCore.WonderingClearLines.Length);
        Assert.Equal(5694, ObjMonFoxCore.WonderingSetLines[0]);
        Assert.Equal(5720, ObjMonFoxCore.WonderingSetLines[1]);
        Assert.Equal(5541, ObjMonFoxCore.WonderingClearLines[0]);
    }

    [Fact]
    public void DeclarationFormFacts()
    {
        Assert.True(ObjMonFoxCore.DeclaredVirtualNotOverride());
        Assert.True(ObjMonFoxCore.VirtualIsCorrectHere());
        Assert.True(ObjMonFoxCore.TwoIndependentChains());
        Assert.True(ObjMonFoxCore.TMonsterCommentsOutMagicAttackTarget());
        Assert.True(ObjMonFoxCore.NotAHidingBug());
    }

    [Fact]
    public void FocusTickFacts()
    {
        Assert.True(ObjMonFoxCore.SetsTargetFocusTick());
        Assert.True(ObjMonFoxCore.SameAsJ212());
        Assert.True(ObjMonFoxCore.AbsentInJ207J210J211());
    }

    // ===================== 五、Create =====================

    [Fact]
    public void CreateFacts()
    {
        Assert.True(ObjMonFoxCore.DuplicatesTMonsterCreate());
        Assert.True(ObjMonFoxCore.OnlyOneExtraLine());
        Assert.True(ObjMonFoxCore.NotInheritingTMonster());
        Assert.True(ObjMonFoxCore.CopyWasNecessary());
        Assert.True(ObjMonFoxCore.StructuralReason());
        Assert.True(ObjMonFoxCore.LineCountDiffersByOne());
        Assert.True(ObjMonFoxCore.ViewRangeFive());
        Assert.True(ObjMonFoxCore.FourthOccurrence());
        Assert.True(ObjMonFoxCore.ViewRangeTableExtracted());
        Assert.True(ObjMonFoxCore.CommonValues());

        // **本类 Create 13 行、TMonster.Create 12 行、差 1**
        Assert.Equal(13, ObjMonFoxCore.CreateLines);
        Assert.Equal(12, ObjMonFoxCore.MonsterCreateLines);
        Assert.Equal(3, ObjMonFoxCore.ViewRangeFiveOtherLines.Length);
    }

    [Fact]
    public void RaceAndBo554Facts()
    {
        Assert.True(ObjMonFoxCore.RaceIsEighty());
        Assert.True(ObjMonFoxCore.SameAsTMonster());
        Assert.True(ObjMonFoxCore.ContrastWithJ214Truck());
        Assert.True(ObjMonFoxCore.Bo554SharedField());
        Assert.True(ObjMonFoxCore.AssignedEvenWithoutInheritingTMonster());
        Assert.True(ObjMonFoxCore.CompletesJ206Finding());
        Assert.True(ObjMonFoxCore.ThreeDeclarations());
        Assert.True(ObjMonFoxCore.FoxDeclarationIs182());

        Assert.Equal(80, ObjMonFoxCore.RaceServer);
        Assert.Equal(new[] { 11, 182, 507 }, ObjMonFoxCore.Bo554DeclLines);
    }

    [Fact]
    public void SearchTimeBoundaries()
    {
        Assert.True(ObjMonFoxCore.SearchTime3000To4999());
        Assert.True(ObjMonFoxCore.ContrastJ213());
        Assert.True(ObjMonFoxCore.OperandOrderDiffers());
        Assert.True(ObjMonFoxCore.ConstantFirstHere());

        // **范围 3000..4999**
        Assert.Equal(3000, ObjMonFoxCore.SearchTimeInit(0));
        Assert.Equal(4999, ObjMonFoxCore.SearchTimeInit(1999));
    }

    [Fact]
    public void RuntimeFacts()
    {
        Assert.True(ObjMonFoxCore.RunTime250());
        Assert.Equal(250, ObjMonFoxCore.RunTime);
    }

    [Fact]
    public void J216HandoffFacts()
    {
        Assert.True(ObjMonFoxCore.J216CoversRest());
        Assert.Equal(68, ObjMonFoxCore.J216WonderingLines);
        Assert.Equal(127, ObjMonFoxCore.J216RunLines);
        Assert.Equal(205 + 68 + 127, ObjMonFoxCore.ClassTotalLines);
    }
}
