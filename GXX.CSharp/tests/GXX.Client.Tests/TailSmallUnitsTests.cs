using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// 「小单元尾巴」里第一批 6 个单元（ClientBuff / uAntiPlug / DepUtils / Mpeg /
/// LanguagesDEPfix / uExceptionStruct）的 1:1 移植测试。
/// <para>每个公开成员 ≥3 个用例（含边界）；对"看起来一样实则不同"的分支写了差异断言。</para>
/// </summary>
public sealed class TailSmallUnitsTests
{
    // ══════════════════════════════════════════════════════════════════════
    // ClientBuff.pas — TClientBuffList（纯数据载体）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：两个字段默认为 0（原文无构造函数、无字段初始化）。</summary>
    [Fact]
    public void ClientBuffList_Defaults_AreBothZero()
    {
        var b = new TClientBuffList();
        Assert.Equal(0, b.m_nButtonTop);
        Assert.Equal(0, b.m_nClientBuffTop);
    }

    /// <summary>用例 2：两个字段可独立赋值（原文是 public 字段，不是只读属性）。</summary>
    [Fact]
    public void ClientBuffList_Fields_AreIndependentlyWritable()
    {
        var b = new TClientBuffList { m_nButtonTop = -120, m_nClientBuffTop = 65535 };
        Assert.Equal(-120, b.m_nButtonTop);
        Assert.Equal(65535, b.m_nClientBuffTop);
    }

    /// <summary>用例 3：边界 —— int.MinValue / int.MaxValue 原样保留（原文 Integer 语义）。</summary>
    [Fact]
    public void ClientBuffList_BoundaryValues_ArePreserved()
    {
        var b = new TClientBuffList
        {
            m_nButtonTop = int.MinValue,
            m_nClientBuffTop = int.MaxValue
        };
        Assert.Equal(int.MinValue, b.m_nButtonTop);
        Assert.Equal(int.MaxValue, b.m_nClientBuffTop);
    }

    /// <summary>用例 4：原文类**没有** IDisposable/析构语义（uses 里的 DxImageButton 是残留）。</summary>
    [Fact]
    public void ClientBuffList_HasNoDisposableContract()
        => Assert.DoesNotContain(typeof(IDisposable), typeof(TClientBuffList).GetInterfaces());

    // ══════════════════════════════════════════════════════════════════════
    // uAntiPlug.pas — EnableDebugPrivilege（§2.3 Stub）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：Stub 恒返回 false（原文失败路径取值）。</summary>
    [Fact]
    public void EnableDebugPrivilege_Stub_ReturnsFalse()
        => Assert.False(UAntiPlug.EnableDebugPrivilege());

    /// <summary>用例 2：幂等 —— 反复调用结果一致（原文每次都会重新打开令牌）。</summary>
    [Fact]
    public void EnableDebugPrivilege_IsIdempotent()
    {
        for (int i = 0; i < 3; i++) Assert.False(UAntiPlug.EnableDebugPrivilege());
    }

    /// <summary>用例 3：方法签名保持「无参 + 返回 bool」（原文 <c>function EnableDebugPrivilege:Boolean;</c>）。</summary>
    [Fact]
    public void EnableDebugPrivilege_Signature_MatchesOriginal()
    {
        var m = typeof(UAntiPlug).GetMethod("EnableDebugPrivilege",
            BindingFlags.Public | BindingFlags.Static);
        Assert.NotNull(m);
        Assert.Equal(typeof(bool), m.ReturnType);
        Assert.Empty(m.GetParameters());
    }

    // ══════════════════════════════════════════════════════════════════════
    // DepUtils.pas — DEP 常量表 + dep_flags 计算（§2.3 Stub + 纯逻辑抽取）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：原文常量取值逐条核对（DepUtils.pas:26-31）。</summary>
    [Fact]
    public void DepUtils_Constants_MatchSource()
    {
        Assert.Equal(0x00000001u, DepUtils.PROCESS_DEP_ENABLE);
        Assert.Equal(0x00000002u, DepUtils.PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION);
        Assert.Equal(1u, DepUtils.MEM_EXECUTE_OPTION_ENABLE);
        Assert.Equal(2u, DepUtils.MEM_EXECUTE_OPTION_DISABLE);
        Assert.Equal(4u, DepUtils.MEM_EXECUTE_OPTION_ATL7_THUNK_EMULATION);
        Assert.Equal(8u, DepUtils.MEM_EXECUTE_OPTION_PERMANENT);
        Assert.Equal(0x22, DepUtils.ProcessExecuteFlags);
    }

    /// <summary>用例 2：枚举顺序即取值（DEP_DISABLED=0 / DEP_ENABLED=1 / DEP_ENABLED_ATL7_COMPAT=2）。</summary>
    [Fact]
    public void DepEnforcement_Values_MatchDeclarationOrder()
    {
        Assert.Equal(0, (int)DepEnforcement.DEP_DISABLED);
        Assert.Equal(1, (int)DepEnforcement.DEP_ENABLED);
        Assert.Equal(2, (int)DepEnforcement.DEP_ENABLED_ATL7_COMPAT);
    }

    /// <summary>
    /// 用例 3：SetProcessDEPPolicy 分支（DepUtils.pas:57-63）的 dep_flags。
    /// 差异断言：DEP_ENABLED 与 DEP_ENABLED_ATL7_COMPAT 在**这条分支上取值不同**
    /// （3 vs 1），而在 ntdll 分支上又是另外两个值（见用例 4）。
    /// </summary>
    [Fact]
    public void ComputeDepFlags_SetProcessDEPPolicyBranch_MatchesSource()
    {
        Assert.Equal(0u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_DISABLED, false));
        Assert.Equal(3u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_ENABLED, false));       // 1 or 2
        Assert.Equal(1u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_ENABLED_ATL7_COMPAT, false));
        Assert.NotEqual(DepUtils.ComputeDepFlags(DepEnforcement.DEP_ENABLED, false),
                        DepUtils.ComputeDepFlags(DepEnforcement.DEP_ENABLED_ATL7_COMPAT, false));
    }

    /// <summary>用例 4：NtSetInformationProcess 回退分支（:75-85）的 dep_flags。</summary>
    [Fact]
    public void ComputeDepFlags_NtdllFallbackBranch_MatchesSource()
    {
        Assert.Equal(2u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_DISABLED, true));        // MEM_EXECUTE_OPTION_DISABLE
        Assert.Equal(9u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_ENABLED, true));         // 8 or 1
        Assert.Equal(13u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_ENABLED_ATL7_COMPAT, true)); // 8 or 1 or 4
    }

    /// <summary>
    /// 用例 5：边界 —— 枚举越界对应原文的 <c>else Exit;</c>（返回 null，不产出任何 flags）。
    /// 注意 <c>DEP_DISABLED</c> 在 SetProcessDEPPolicy 分支上是 <b>0</b>，
    /// 这正是"不要用 0 当哨兵判失败"的典型：必须用可空类型区分 0 与"无值"。
    /// </summary>
    [Fact]
    public void ComputeDepFlags_OutOfRangeEnum_ReturnsNullLikeOriginalElseExit()
    {
        Assert.Null(DepUtils.ComputeDepFlags((DepEnforcement)99, false));
        Assert.Null(DepUtils.ComputeDepFlags((DepEnforcement)99, true));
        Assert.Null(DepUtils.ComputeDepFlags((DepEnforcement)(-1), true));
        // 差异断言：DEP_DISABLED 的 0 **不等于** null
        Assert.Equal(0u, DepUtils.ComputeDepFlags(DepEnforcement.DEP_DISABLED, false));
    }

    /// <summary>用例 6：Stub 入口恒返回 false（原文"未设置成功"取值），且不抛异常。</summary>
    [Theory]
    [InlineData(DepEnforcement.DEP_DISABLED)]
    [InlineData(DepEnforcement.DEP_ENABLED)]
    [InlineData(DepEnforcement.DEP_ENABLED_ATL7_COMPAT)]
    [InlineData((DepEnforcement)99)]
    public void SetCurrentProcessDEP_Stub_ReturnsFalse(DepEnforcement e)
        => Assert.False(DepUtils.SetCurrentProcessDEP(e));

    // ══════════════════════════════════════════════════════════════════════
    // Mpeg.pas — TMPEG（§2.3 Stub，但保留原文状态机的可观察取值）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：构造后 boInit=false、boPlay=false（原文 :55 的 Init 调用被注释掉了）。</summary>
    [Fact]
    public void Mpeg_Constructor_LeavesBothFlagsFalse()
    {
        using var m = new TMPEG(null);
        Assert.False(m.BoInit);
        Assert.False(m.BoPlay);
    }

    /// <summary>
    /// 用例 2：Init 在原文是 <c>private</c>；本移植把它提为 <c>public</c> 只是为了能直接驱动它，
    /// Stub 恒返回 false（等价于原文 <c>CoInitialize</c> 失败的出口）。
    /// 这里把「已提升为 public」这件事显式登记，以免被误当成原文签名。
    /// </summary>
    [Fact]
    public void Mpeg_Init_StubReturnsFalse_AndIsPromotedToPublicForTestability()
    {
        using var m = new TMPEG(null);
        Assert.False(m.Init());
        // 原文 :18 声明为 private；C# 侧提为 public（差异已登记）
        Assert.NotNull(typeof(TMPEG).GetMethod("Init", BindingFlags.Public | BindingFlags.Instance));
        // 但 Play/Pause/Stop/Create/Destroy 才是原文的公开面，Init 不在其中
        Assert.NotNull(typeof(TMPEG).GetMethod("Play", new[] { typeof(string) }));
    }

    /// <summary>用例 3：Play 在 Stub 下返回 false，且不修改 boInit/boPlay。</summary>
    [Fact]
    public void Mpeg_Play_StubReturnsFalse_AndLeavesFlagsUntouched()
    {
        using var m = new TMPEG(null);
        Assert.False(m.Play("intro.mpg"));
        Assert.False(m.BoInit);
        Assert.False(m.BoPlay);
        Assert.Equal("intro.mpg", m.FileName);
    }

    /// <summary>用例 4：Play(null) 的边界 —— 原文 <c>MultiByteToWideChar(PChar(sFileName),…)</c> 对 nil 会得到空串。</summary>
    [Fact]
    public void Mpeg_Play_NullFileName_DoesNotThrow()
    {
        using var m = new TMPEG(null);
        Assert.False(m.Play(null));
        Assert.Null(m.FileName);
    }

    /// <summary>用例 5：Stop 在未 Init 时**什么都不做**（原文 :108 <c>if not boInit then Exit;</c>，
    /// 注意它连 Close 都不调 —— 与 Close 的行为形成差异断言）。</summary>
    [Fact]
    public void Mpeg_Stop_WhenNotInitialized_IsNoOp_UnlikeClose()
    {
        using var m = new TMPEG(null);
        m.Stop();                       // 不应抛
        Assert.False(m.BoInit);
        Assert.False(m.BoPlay);

        // Close 会把 boInit 置回 false（已经是 false），但**不动 boPlay**；
        // 无论哪条路径，boPlay 都不会被 Stop/Close 改变。
        m.Close();
        Assert.False(m.BoPlay);
    }

    /// <summary>用例 6：构造时保留 PlayWindow 引用（原文 MovieWindow := PlayWindow）。</summary>
    [Fact]
    public void Mpeg_KeepsPlayWindowReference_EvenWhenNull()
    {
        using var m1 = new TMPEG(null);
        Assert.Null(m1.PlayWindowControl);

        using var panel = new Panel();
        using var m2 = new TMPEG(panel);
        Assert.Same(panel, m2.PlayWindowControl);
    }

    /// <summary>用例 7：公开成员面与原文一致（Create/Destroy/Play/Pause/Stop + IDisposable 表达 Destroy）。</summary>
    [Fact]
    public void Mpeg_PublicSurface_MatchesOriginal()
    {
        var t = typeof(TMPEG);
        Assert.NotNull(t.GetConstructor(new[] { typeof(Control) }));
        Assert.NotNull(t.GetMethod("Play", new[] { typeof(string) }));
        Assert.NotNull(t.GetMethod("Pause", Type.EmptyTypes));
        Assert.NotNull(t.GetMethod("Stop", Type.EmptyTypes));
        Assert.Contains(typeof(IDisposable), t.GetInterfaces());
    }

    // ══════════════════════════════════════════════════════════════════════
    // LanguagesDEPfix.pas — ParseLocaleIdHex（纯逻辑）+ §2.3 Stub
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：原文 <c>StrToInt('$' + Copy(LocaleID, 5, 4))</c> —— 取第 5..8 位十六进制。</summary>
    [Theory]
    [InlineData("00000409", 0x0409u)]   // en-US
    [InlineData("00000804", 0x0804u)]   // zh-CN
    [InlineData("00000411", 0x0411u)]   // ja-JP
    [InlineData("0000FFFF", 0xFFFFu)]
    public void ParseLocaleIdHex_TakesChar5To8(string locale, uint expected)
        => Assert.Equal(expected, LanguagesDEPfix.ParseLocaleIdHex(locale));

    /// <summary>用例 2：大小写混写都要能解析（原文 StrToInt 的 '$' 前缀接受 a-f 与 A-F）。</summary>
    [Fact]
    public void ParseLocaleIdHex_IsCaseInsensitive()
    {
        Assert.Equal(0x0409u, LanguagesDEPfix.ParseLocaleIdHex("00000409"));
        Assert.Equal(0x0abcdu, LanguagesDEPfix.ParseLocaleIdHex("0000abcd"));
        Assert.Equal(0x0abcdu, LanguagesDEPfix.ParseLocaleIdHex("0000ABCD"));
    }

    /// <summary>
    /// 用例 3：边界 —— 串长不足 8 时原文的 <c>Copy</c> 会返回空串 → <c>StrToInt('$')</c> 抛
    /// <c>EConvertError</c>。本移植以 <c>Convert.ToUInt32</c> 抛出等价异常。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("00")]
    [InlineData("0000")]
    public void ParseLocaleIdHex_TooShort_Throws(string locale)
        => Assert.ThrowsAny<Exception>(() => LanguagesDEPfix.ParseLocaleIdHex(locale));

    /// <summary>用例 4：串长 &gt;8 时只取第 5..8 位（原文 Copy 的定长语义）。</summary>
    [Fact]
    public void ParseLocaleIdHex_LongerInput_IgnoresTail()
        => Assert.Equal(0x0409u, LanguagesDEPfix.ParseLocaleIdHex("00000409XYZ"));

    /// <summary>用例 5：null 入参边界（C# 侧的额外防御，原文对 nil PChar 会拿不到字符）。</summary>
    [Fact]
    public void ParseLocaleIdHex_Null_Throws()
        => Assert.ThrowsAny<Exception>(() => LanguagesDEPfix.ParseLocaleIdHex(null));

    /// <summary>用例 6：Stub —— ApplyLanguagesDEPfix 不执行代码改写，返回 false。</summary>
    [Fact]
    public void ApplyLanguagesDEPfix_Stub_DoesNothing_AndReturnsFalse()
        => Assert.False(LanguagesDEPfix.ApplyLanguagesDEPfix());

    /// <summary>用例 7：跳转补丁常量与原文一致（$E9 近跳转、5 字节 TJumpRec、6 字节 TLongJumpRec、$25FF 包跳板）。</summary>
    [Fact]
    public void PatchConstants_MatchSource()
    {
        Assert.Equal(0xE9, LanguagesDEPfix.PATCH_OPCODE_JMP_NEAR32);
        Assert.Equal(0x25FF, LanguagesDEPfix.RUNTIME_PACKAGE_THUNK_OPCODE);
        Assert.Equal(5, LanguagesDEPfix.JumpRecSize);         // 1 字节 opcode + 4 字节 rel32
        Assert.Equal(6, LanguagesDEPfix.LongJumpRecSize);     // 2 字节 opcode + 4 字节 addr
        Assert.Equal(0u, LanguagesDEPfix.LCID_SUPPORTED);
    }

    /// <summary>用例 8：Languages() 惰性单例 —— 多次调用返回同一实例，且 Stub 下为空表。</summary>
    [Fact]
    public void Languages_IsLazySingleton_AndEmptyInStub()
    {
        var a = LanguagesDEPfix.Languages();
        var b = LanguagesDEPfix.Languages();
        Assert.Same(a, b);
        Assert.Empty(a);
    }

    /// <summary>用例 9：GetLocaleData Stub 返回空串（W/A 两个变体一致），且不抛。</summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetLocaleData_Stub_ReturnsEmpty(bool wide)
    {
        Assert.Equal(string.Empty, LanguagesDEPfix.GetLocaleData(0x0409u, 0x00000002u, wide));
        Assert.Equal(string.Empty, LanguagesDEPfix.GetLocaleData(0u, 0u, wide));
    }

    // ══════════════════════════════════════════════════════════════════════
    // uExceptionStruct.pas — SEH/VEH 结构常量（§2.3 Stub：仅声明）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：常量指纹（9 条，名与值逐条核对；含唯一的负值 -1）。</summary>
    [Fact]
    public void ExceptionStruct_Constants_Fingerprint()
    {
        var fp = UExceptionStruct.ConstantFingerprint;
        Assert.Equal(9, fp.Length);
        Assert.Equal(new[]
        {
            ("EXCEPTION_EXECUTE_HANDLER", 1L),
            ("EXCEPTION_CONTINUE_SEARCH", 0L),
            ("EXCEPTION_CONTINUE_EXECUTION", -1L),
            ("EH_NONE", 0L),
            ("EH_NONCONTINUABLE", 1L),
            ("EH_UNWINDING", 2L),
            ("EH_EXIT_UNWIND", 4L),
            ("EH_STACK_INVALID", 8L),
            ("EH_NESTED_CALL", 16L),
        }, fp);
    }

    /// <summary>
    /// 用例 2：差异断言 —— <c>EXCEPTION_CONTINUE_SEARCH = 0</c> 与
    /// <c>EXCEPTION_CONTINUE_EXECUTION = -1</c> 是"看起来像布尔实则不是"的经典陷阱，
    /// 用 <c>!= -1</c>/<c>== 0</c> 当成功判据都会误判，故单独锁住。
    /// </summary>
    [Fact]
    public void ExceptionDisposition_Constants_AreNotBooleanLike()
    {
        Assert.Equal(0, UExceptionStruct.EXCEPTION_CONTINUE_SEARCH);
        Assert.Equal(-1, UExceptionStruct.EXCEPTION_CONTINUE_EXECUTION);
        Assert.Equal(1, UExceptionStruct.EXCEPTION_EXECUTE_HANDLER);
        // 三者互不相等，且没有一个等于 C# 的 bool 默认值语义
        Assert.NotEqual(UExceptionStruct.EXCEPTION_CONTINUE_SEARCH, UExceptionStruct.EXCEPTION_CONTINUE_EXECUTION);
        Assert.NotEqual(UExceptionStruct.EXCEPTION_EXECUTE_HANDLER, UExceptionStruct.EXCEPTION_CONTINUE_SEARCH);
    }

    /// <summary>用例 3：EH_* 位标志是 2 的幂且互不重叠（原文 :144-149）。</summary>
    [Fact]
    public void EhFlags_ArePowersOfTwo_AndDistinct()
    {
        uint[] flags =
        {
            UExceptionStruct.EH_NONCONTINUABLE, UExceptionStruct.EH_UNWINDING,
            UExceptionStruct.EH_EXIT_UNWIND, UExceptionStruct.EH_STACK_INVALID,
            UExceptionStruct.EH_NESTED_CALL,
        };
        Assert.Equal(5, flags.Distinct().Count());
        foreach (uint f in flags)
        {
            Assert.True(f != 0 && (f & (f - 1)) == 0, $"0x{f:X} 不是 2 的幂");
        }
        Assert.Equal(0u, UExceptionStruct.EH_NONE);
    }

    /// <summary>用例 4：EXCEPTION_DISPOSITION 枚举顺序即取值（原文注释四条）。</summary>
    [Fact]
    public void ExceptionDisposition_Enum_OrderMatchesSource()
    {
        Assert.Equal(0, (int)EXCEPTION_DISPOSITION.ExceptionContinueExecution);
        Assert.Equal(1, (int)EXCEPTION_DISPOSITION.ExceptionContinueSearch);
        Assert.Equal(2, (int)EXCEPTION_DISPOSITION.ExceptionNestedException);
        Assert.Equal(3, (int)EXCEPTION_DISPOSITION.ExceptionCollidedUnwind);
    }

    /// <summary>用例 5：packed 布局 —— TJmpInstruction = 5 字节（1 + 4）。</summary>
    [Fact]
    public void TJmpInstruction_IsPacked5Bytes()
        => Assert.Equal(5, Marshal.SizeOf<TJmpInstruction>());

    /// <summary>用例 6：TExcDesc 的柔性数组偏移常量与原文变体声明一致。</summary>
    [Fact]
    public void TExcDescLayout_Offsets_MatchVariantDeclaration()
    {
        Assert.Equal(0, TExcDescLayout.JmpOffset);
        Assert.Equal(5, TExcDescLayout.JmpSize);
        Assert.Equal(5, TExcDescLayout.CntOffset);              // 紧跟 jmp
        Assert.Equal(5, TExcDescLayout.InstructionsOffset);     // 变体 0 与变体 1 的 cnt 同址
        Assert.Equal(9, TExcDescLayout.ExcTabOffset);           // cnt(4) 之后
    }

    /// <summary>用例 7：TExcFrame 的三个具名字段顺序（next/desc/hEBP）。</summary>
    [Fact]
    public void TExcFrame_FieldOrder_MatchesSource()
    {
        var names = typeof(TExcFrame).GetFields().Select(f => f.Name).ToArray();
        Assert.Equal(new[] { "next", "desc", "hEBP", "Union" }, names);
    }

    /// <summary>用例 8：_ExceptionRegistration / _ExceptionHandler 的字段名与原文一致（含前导下划线类型名）。</summary>
    [Fact]
    public void ExceptionRegistrationAndHandler_FieldNames_MatchSource()
    {
        Assert.Equal(new[] { "Prev", "Handler" }, typeof(_ExceptionRegistration).GetFields().Select(f => f.Name));
        Assert.Equal(new[] { "ExceptionRecord", "SEH", "Context", "DispatcherContext" },
            typeof(_ExceptionHandler).GetFields().Select(f => f.Name));
    }
}
