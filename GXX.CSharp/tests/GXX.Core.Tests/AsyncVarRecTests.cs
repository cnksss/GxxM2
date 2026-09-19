using System;
using GXX.Core.Async;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// TVarRec 判别式 / CopyVarRec 深拷贝决策 / array-of-const 压栈记账
/// （原文 AsyncCalls.pas:2284-2409）的测试。
/// </summary>
public class AsyncVarRecTests
{
    private static TVarRec Rec(byte vType, object payload, int padding = 0)
        => new TVarRec { VType = vType, VPointer = payload, VReservedPadding = padding };

    // ------------------------------------------------------------------
    // CopyVarRec（原文 2284-2313）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(TVarRecType.vtString)]
    [InlineData(TVarRecType.vtAnsiString)]
    [InlineData(TVarRecType.vtWideString)]
    [InlineData(TVarRecType.vtExtended)]
    [InlineData(TVarRecType.vtCurrency)]
    [InlineData(TVarRecType.vtInt64)]
    [InlineData(TVarRecType.vtVariant)]
    [InlineData(TVarRecType.vtInterface)]
    public void CopyVarRec_DeepCopyTypes_WithNonNullPointer_TakeFieldAssignBranch(byte vType)
    {
        TVarRec src = Rec(vType, new object(), padding: 7);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(vType, dst.VType);
        Assert.NotNull(dst.VPointer);
        // 差异断言：深拷贝分支只赋 VType 与指针槽，VType 之后的对齐填充保持默认值（原文：Result.VType := ...; Result.VPointer := nil;）
        Assert.Equal(0, dst.VReservedPadding);
        Assert.Equal(7, src.VReservedPadding); // 源未被改动
    }

    [Fact]
    public void CopyVarRec_NullPointer_FallsBackToWholeStructCopy()
    {
        // 边界：深拷贝类型 + 指针为 nil ⇒ 走 else 的“整体 Result := Data”
        TVarRec src = Rec(TVarRecType.vtInt64, null, padding: 7);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(TVarRecType.vtInt64, dst.VType);
        Assert.Null(dst.VPointer);
        Assert.Equal(7, dst.VReservedPadding); // 差异断言：整体复制把填充也搬过来了
    }

    [Theory]
    [InlineData(TVarRecType.vtInteger)]
    [InlineData(TVarRecType.vtBoolean)]
    [InlineData(TVarRecType.vtChar)]
    [InlineData(TVarRecType.vtPointer)]
    [InlineData(TVarRecType.vtObject)]
    [InlineData(TVarRecType.vtClass)]
    [InlineData(TVarRecType.vtWideChar)]
    [InlineData(TVarRecType.vtPWideChar)]
    [InlineData(TVarRecType.vtPChar)]
    public void CopyVarRec_NonDeepCopyTypes_AlwaysCopyWholeStruct(byte vType)
    {
        TVarRec src = Rec(vType, new object(), padding: 9);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(vType, dst.VType);
        Assert.Equal(9, dst.VReservedPadding);
    }

    [Fact]
    public void CopyVarRec_Int64Payload_AllocatesNewBox()
    {
        TVarRec src = Rec(TVarRecType.vtInt64, 1234567890123L);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(1234567890123L, dst.VInt64);
        // 原文：New(Result.VInt64); Result.VInt64^ := Data.VInt64^  ⇒ 新分配
        Assert.False(ReferenceEquals(src.VPointer, dst.VPointer));
    }

    [Fact]
    public void CopyVarRec_AnsiStringPayload_SharesReference()
    {
        var text = new string('x', 8);
        TVarRec src = Rec(TVarRecType.vtAnsiString, text);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(text, dst.VAnsiString);
        // 原文：AnsiString(Result.VAnsiString) := AnsiString(Data.VAnsiString) ⇒ 仅引用计数 +1
        Assert.Same(src.VPointer, dst.VPointer);
    }

    [Fact]
    public void CopyVarRec_ShortStringPayload_ClonesString()
    {
        var text = new string('y', 5);
        TVarRec src = Rec(TVarRecType.vtString, text);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(text, dst.VString);
        // 原文：New(Result.VString); Result.VString^ := Data.VString^ ⇒ 新的 PShortString
        Assert.False(ReferenceEquals(src.VPointer, dst.VPointer));
    }

    [Fact]
    public void CopyVarRec_EmptyStringPayload_IsNotNull_SoTakesDeepCopyBranch()
    {
        // 边界：空串不是 nil
        TVarRec src = Rec(TVarRecType.vtAnsiString, string.Empty, padding: 3);

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(string.Empty, dst.VAnsiString);
        Assert.Equal(0, dst.VReservedPadding); // 深拷贝分支
    }

    [Fact]
    public void CopyVarRec_DefaultStruct_StaysDefault()
    {
        TVarRec src = default;

        TVarRecMarshal.CopyVarRec(in src, out TVarRec dst);

        Assert.Equal(0, dst.VType);
        Assert.Null(dst.VPointer);
    }

    [Fact]
    public void IsDeepCopyType_MatchesOriginalSetExactly()
    {
        byte[] deep =
        {
            TVarRecType.vtString, TVarRecType.vtAnsiString, TVarRecType.vtWideString,
            TVarRecType.vtExtended, TVarRecType.vtCurrency, TVarRecType.vtInt64,
            TVarRecType.vtVariant, TVarRecType.vtInterface,
        };
        for (byte t = 0; t <= 20; t++)
        {
            bool expected = Array.IndexOf(deep, t) >= 0;
            Assert.Equal(expected, TVarRecMarshal.IsDeepCopyType(t));
        }

        // 差异断言：原文 {$IFDEF UNICODE} 下的 vtUnicodeString 在 Delphi 7 基线下不属于深拷贝集合
        Assert.False(TVarRecMarshal.IsDeepCopyType(TVarRecType.vtUnicodeString));
    }

    // ------------------------------------------------------------------
    // ReleaseVarRec（原文 2258-2282 析构分支）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(TVarRecType.vtString)]
    [InlineData(TVarRecType.vtAnsiString)]
    [InlineData(TVarRecType.vtWideString)]
    [InlineData(TVarRecType.vtExtended)]
    [InlineData(TVarRecType.vtCurrency)]
    [InlineData(TVarRecType.vtInt64)]
    [InlineData(TVarRecType.vtVariant)]
    [InlineData(TVarRecType.vtInterface)]
    public void ReleaseVarRec_ClearsPayloadForReleasedTypes(byte vType)
    {
        TVarRec rec = Rec(vType, new object());

        TVarRecMarshal.ReleaseVarRec(ref rec);

        Assert.Null(rec.VPointer);
        Assert.Equal(vType, rec.VType); // VType 不动
    }

    [Theory]
    [InlineData(TVarRecType.vtInteger)]
    [InlineData(TVarRecType.vtObject)]
    [InlineData(TVarRecType.vtPointer)]
    public void ReleaseVarRec_LeavesOtherTypesUntouched(byte vType)
    {
        var payload = new object();
        TVarRec rec = Rec(vType, payload);

        TVarRecMarshal.ReleaseVarRec(ref rec);

        Assert.Same(payload, rec.VPointer);
    }

    // ------------------------------------------------------------------
    // GetPushKind（原文 2337-2401 的 asm case 分组）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(TVarRecType.vtInteger, TVarRecPushKind.Integer)]
    [InlineData(TVarRecType.vtBoolean, TVarRecPushKind.BooleanOrChar)]
    [InlineData(TVarRecType.vtChar, TVarRecPushKind.BooleanOrChar)]
    [InlineData(TVarRecType.vtWideChar, TVarRecPushKind.WideChar)]
    [InlineData(TVarRecType.vtExtended, TVarRecPushKind.Extended)]
    [InlineData(TVarRecType.vtCurrency, TVarRecPushKind.CurrencyOrInt64)]
    [InlineData(TVarRecType.vtInt64, TVarRecPushKind.CurrencyOrInt64)]
    [InlineData(TVarRecType.vtString, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtPointer, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtPChar, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtObject, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtClass, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtAnsiString, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtPWideChar, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtVariant, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtInterface, TVarRecPushKind.Pointer)]
    [InlineData(TVarRecType.vtWideString, TVarRecPushKind.Pointer)]
    public void GetPushKind_MatchesOriginalClassification(byte vType, TVarRecPushKind expected)
    {
        Assert.Equal(expected, TVarRecMarshal.GetPushKind(vType));
    }

    [Fact]
    public void GetPushKind_UnicodeString_ThrowsInDelphi7Baseline()
    {
        // 差异断言：vtUnicodeString(17) 在原文受 {$IFDEF UNICODE} 保护，D7 下落入 else → UnknownVarRecType
        var ex = Assert.Throws<TAsyncCallError>(() => TVarRecMarshal.GetPushKind(TVarRecType.vtUnicodeString));
        Assert.Contains("17", ex.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData((byte)200)]
    [InlineData((byte)255)]
    public void GetPushKind_UnknownType_ThrowsWithTypeNumber(byte vType)
    {
        var ex = Assert.Throws<TAsyncCallError>(() => TVarRecMarshal.GetPushKind(vType));
        Assert.Contains(vType.ToString(), ex.Message, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------------
    // 栈字节记账（原文 2321、2363、2375）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(0, 0x40)]
    [InlineData(1, 0x44)]
    [InlineData(3, 0x4C)]
    [InlineData(16, 0x80)]
    public void GetStackBufferByteCount_IsArgCountTimesFourPlusReserve(int argCount, int expected)
    {
        Assert.Equal(expected, TVarRecMarshal.GetStackBufferByteCount(argCount));
    }

    [Fact]
    public void GetStackBufferByteCount_NegativeArgCount_HasNoLowerBoundCheck()
    {
        // 原文如此（AsyncCalls.pas:2321）：Length(FArgs) 不可能为负，故无下界检查；此处保留该无检查行为
        Assert.Equal(0x3C, TVarRecMarshal.GetStackBufferByteCount(-1));
    }

    [Theory]
    [InlineData(TVarRecType.vtExtended, 8)]
    [InlineData(TVarRecType.vtCurrency, 4)]
    [InlineData(TVarRecType.vtInt64, 4)]
    [InlineData(TVarRecType.vtInteger, 0)]
    [InlineData(TVarRecType.vtPointer, 0)]
    public void GetExtraStackByteCount_MatchesAsmAccumulation(byte vType, int expected)
    {
        Assert.Equal(expected, TVarRecMarshal.GetExtraStackByteCount(vType));
    }

    [Fact]
    public void ComputeStackByteCount_SumsBaseAndExtras()
    {
        var args = new[]
        {
            Rec(TVarRecType.vtInteger, 1),
            Rec(TVarRecType.vtExtended, 1.5),
            Rec(TVarRecType.vtInt64, 7L),
        };

        // 3 * 4 + 0x40 + 8 (Extended) + 4 (Int64) = 12 + 64 + 12 = 88
        Assert.Equal(88, TVarRecMarshal.ComputeStackByteCount(args));
    }

    [Fact]
    public void ComputeStackByteCount_EmptyArgs_IsJustTheReserve()
    {
        Assert.Equal(0x40, TVarRecMarshal.ComputeStackByteCount(Array.Empty<TVarRec>()));
    }

    [Fact]
    public void ComputeStackByteCount_NullArgs_TreatedAsEmpty()
    {
        Assert.Equal(0x40, TVarRecMarshal.ComputeStackByteCount(null));
    }

    // ------------------------------------------------------------------
    // BuildPushSequence（原文 2334-2402；cdecl ⇒ 自右向左）
    // ------------------------------------------------------------------

    [Fact]
    public void BuildPushSequence_IsRightToLeft()
    {
        var args = new[]
        {
            Rec(TVarRecType.vtInteger, 1),
            Rec(TVarRecType.vtAnsiString, "a"),
            Rec(TVarRecType.vtWideChar, 'w'),
        };

        TVarRecPushStep[] steps = TVarRecMarshal.BuildPushSequence(args);

        Assert.Equal(3, steps.Length);
        Assert.Equal(2, steps[0].ArgIndex);
        Assert.Equal(TVarRecPushKind.WideChar, steps[0].Kind);
        Assert.Equal(1, steps[1].ArgIndex);
        Assert.Equal(TVarRecPushKind.Pointer, steps[1].Kind);
        Assert.Equal(0, steps[2].ArgIndex);
        Assert.Equal(TVarRecPushKind.Integer, steps[2].Kind);
    }

    [Fact]
    public void BuildPushSequence_EmptyArgs_IsEmpty()
    {
        Assert.Empty(TVarRecMarshal.BuildPushSequence(Array.Empty<TVarRec>()));
    }

    [Fact]
    public void BuildPushSequence_NullArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TVarRecMarshal.BuildPushSequence(null));
    }

    [Fact]
    public void BuildPushSequence_UnsupportedType_ThrowsAtThatArgument()
    {
        var args = new[]
        {
            Rec(TVarRecType.vtInteger, 1),
            Rec(TVarRecType.vtUnicodeString, "u"),
            Rec(TVarRecType.vtInteger, 3),
        };

        // 自右向左：先处理下标 2（合法），再到下标 1（非法）→ 抛错
        var ex = Assert.Throws<TAsyncCallError>(() => TVarRecMarshal.BuildPushSequence(args));
        Assert.Contains("17", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildPushSequence_RecordsPerTypeExtraBytes()
    {
        var args = new[]
        {
            Rec(TVarRecType.vtExtended, 1.0),
            Rec(TVarRecType.vtInteger, 1),
        };

        TVarRecPushStep[] steps = TVarRecMarshal.BuildPushSequence(args);

        Assert.Equal(0, steps[0].ExtraStackBytes); // 下标 1：Integer
        Assert.Equal(8, steps[1].ExtraStackBytes); // 下标 0：Extended
    }

    // ------------------------------------------------------------------
    // TAsyncCallArrayOfConst（原文 2231-2256 的 Self 插入）
    // ------------------------------------------------------------------

    [Fact]
    public void ArrayOfConst_FunctionForm_KeepsArgCountAndDeepCopies()
    {
        var args = new[]
        {
            Rec(TVarRecType.vtAnsiString, "hello"),
            Rec(TVarRecType.vtInteger, 42, padding: 5),
        };

        var call = new TAsyncCallArrayOfConst(0x1234, args, new FakeRuntime().Runtime);

        Assert.Equal(2, call.Args.Length);
        Assert.Equal(TVarRecType.vtAnsiString, call.Args[0].VType);
        Assert.Equal("hello", call.Args[0].VAnsiString);
        // Integer 属浅拷贝集合 → 整体复制，填充一并带走（差异断言）
        Assert.Equal(5, call.Args[1].VReservedPadding);
        Assert.Equal(42, call.Args[1].VInteger);
    }

    [Fact]
    public void ArrayOfConst_MethodForm_InsertsSelfAsFirstArg()
    {
        var methodData = new object();
        var args = new[] { Rec(TVarRecType.vtInteger, 7) };

        var call = new TAsyncCallArrayOfConst(0x2000, methodData, args, new FakeRuntime().Runtime);

        Assert.Equal(2, call.Args.Length);
        Assert.Equal(TVarRecType.vtObject, call.Args[0].VType);
        Assert.Same(methodData, call.Args[0].VObject);
        Assert.Equal(7, call.Args[1].VInteger);
    }

    [Fact]
    public void ArrayOfConst_MethodForm_WithNoArgs_StillHasSelf()
    {
        var call = new TAsyncCallArrayOfConst(0x2000, new object(), Array.Empty<TVarRec>(), new FakeRuntime().Runtime);

        Assert.Single(call.Args);
        Assert.Equal(TVarRecType.vtObject, call.Args[0].VType);
    }
}
