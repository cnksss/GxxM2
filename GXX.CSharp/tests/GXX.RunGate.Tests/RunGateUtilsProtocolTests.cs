using System;
using System.Runtime.InteropServices;
using GXX.Core;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsProtocol.cs 测试 —— 覆盖 Grobal2_Ex.pas:228-301（结构布局）与
/// GateShare.pas:3475-3494（EncodeRunGateMsg）、RunGateUtils.pas:314-357 / 402-437（帧构造）。
/// </summary>
public class RunGateUtilsProtocolTests
{
    // ---------------- 布局：Delphi SizeOf 必须逐字节一致 ----------------

    [Fact]
    public void Layout_TM2MsgHeader_Is20Bytes_AllOffsetsNatural()
    {
        // 原 Grobal2_Ex.pas:238-245：LongWord+Integer+Word+Word+LongWord+Integer
        Assert.Equal(20, Marshal.SizeOf<TM2MsgHeader>());
        Assert.Equal(RunGateUtilsConst.SizeOfTM2MsgHeader, Marshal.SizeOf<TM2MsgHeader>());
        Assert.Equal(20, RunGateFrameBuilder.BuildServerMsgHeaderOnly(0, 0, 0, 0, 0, 0).Length);
    }

    [Fact]
    public void Layout_TDefaultMessage_Is16Bytes()
    {
        Assert.Equal(16, Marshal.SizeOf<TDefaultMessage>());
        Assert.Equal(RunGateUtilsConst.SizeOfTDefaultMessage, Marshal.SizeOf<TDefaultMessage>());
    }

    [Fact]
    public void Layout_TRungateMsgHeader_Is24Bytes_MsgAtOffset8()
    {
        // TDefaultMessage 的 Int64 使 Msg 对齐到偏移 8 → 8 + 16 = 24
        Assert.Equal(24, Marshal.SizeOf<TRungateMsgHeader>());
        Assert.Equal(RunGateUtilsConst.SizeOfTRungateMsgHeader, Marshal.SizeOf<TRungateMsgHeader>());
        Assert.Equal(8, Marshal.OffsetOf<TRungateMsgHeader>(nameof(TRungateMsgHeader.Msg)).ToInt32());
    }

    [Fact]
    public void Layout_VerifyStructs_MatchDelphiPackedSizes()
    {
        Assert.Equal(12, Marshal.SizeOf<TRungateVerifyHeader>());       // LongWord×3
        Assert.Equal(51, Marshal.SizeOf<TRungateVerifyData>());         // 4 + 15 + 32
        Assert.Equal(59, Marshal.SizeOf<TRungateVerifyData_New>());     // 51 + 4 + 4
        Assert.Equal(RunGateUtilsConst.SizeOfTRungateVerifyData, Marshal.SizeOf<TRungateVerifyData>());
        Assert.Equal(RunGateUtilsConst.SizeOfTRungateVerifyData_New, Marshal.SizeOf<TRungateVerifyData_New>());
    }

    // ---------------- 常量：原文 Grobal2_Ex.pas:667-694 ----------------

    [Fact]
    public void Constants_MagicCodes_MatchGrobal2Ex()
    {
        Assert.Equal(0xAA55AA55u, RunGateUtilsConst.RUNGATECODE);
        Assert.Equal(0xAA9AAA9Au, RunGateUtilsConst.RUNGATECODEX);
        Assert.Equal(0xAABBCCDDu, RunGateUtilsConst.RUN_GATE_MSG_CODE);
        Assert.Equal(Grobal2Const.RUNGATECODE, RunGateUtilsConst.RUNGATECODE);
        Assert.Equal(Grobal2Const.RUNGATECODEX, RunGateUtilsConst.RUNGATECODEX);
        Assert.Equal(Grobal2Const.RUN_GATE_MSG_CODE, RunGateUtilsConst.RUN_GATE_MSG_CODE);
    }

    /// <summary>
    /// 差异断言（跨文件契约缺陷）：<c>GXX.GatewayKit/GatewayProtocol.cs:22</c> 把
    /// <c>RUNGATECODEX</c> 映射成了 <c>Grobal2Const.RUN_GATE_MSG_CODE</c>（$AABBCCDD），
    /// 而 Delphi <c>Grobal2_Ex.pas:694</c> 的 <c>RUNGATECODEX</c> 是 <b>$AA9AAA9A</b>。
    /// 本车道无权改 GatewayKit，这里把差异钉死以防被"顺手统一"。
    /// </summary>
    [Fact]
    public void Differential_GatewayKit_RUNGATECODEX_IsWrongConstant()
    {
        Assert.NotEqual(Grobal2Const.RUN_GATE_MSG_CODE, RunGateUtilsConst.RUNGATECODEX);
        Assert.Equal(0xAA9AAA9Au, RunGateUtilsConst.RUNGATECODEX);
        // GatewayKit 的写法（错误）：
        Assert.Equal(0xAABBCCDDu, Grobal2Const.RUN_GATE_MSG_CODE);
        // 三者在 Delphi 里是三个不同的常量，不得互相顶替
        Assert.NotEqual(RunGateUtilsConst.RUNGATECODE, RunGateUtilsConst.RUNGATECODEX);
        Assert.NotEqual(RunGateUtilsConst.RUNGATECODEX, RunGateUtilsConst.RUN_GATE_MSG_CODE);
    }

    [Fact]
    public void Constants_Gm_EffectiveValues_AreDeclarationInitialisers()
    {
        // Grobal2_Ex.pas:667-686：NEED_REGISTER=1 时它们是 var，但全树无运行期赋值
        //（唯一赋值点在注释块内）→ 取值恒等于初值。
        Assert.Equal(1, RunGateUtilsConst.GM_OPEN);
        Assert.Equal(2, RunGateUtilsConst.GM_CLOSE);
        Assert.Equal(3, RunGateUtilsConst.GM_CHECKSERVER);
        Assert.Equal(4, RunGateUtilsConst.GM_CHECKCLIENT);
        Assert.Equal(5, RunGateUtilsConst.GM_DATA);
        Assert.Equal(6, RunGateUtilsConst.GM_SERVERUSERINDEX);
        Assert.Equal(7, RunGateUtilsConst.GM_RECEIVE_OK);
        Assert.Equal(9, RunGateUtilsConst.GM_COMPDATA);
        Assert.Equal(10, RunGateUtilsConst.GM_KICK);
        Assert.Equal(11, RunGateUtilsConst.GM_DATA_CACHE);
        Assert.Equal(12, RunGateUtilsConst.GM_NO_CERTIFICATION);
        Assert.Equal(13, RunGateUtilsConst.GM_FULL_SERVICE_MSG);
        Assert.Equal(15, RunGateUtilsConst.GM_RUN_GATE_VER);
        Assert.Equal(16, RunGateUtilsConst.GM_RUN_GATE_MAGICS);
        Assert.Equal(20, RunGateUtilsConst.GM_DELAY_CLOSE);
    }

    // ---------------- 帧构造 ----------------

    [Fact]
    public void BuildServerMsg_NullData_ProducesBareHeader()
    {
        var buf = RunGateFrameBuilder.BuildServerMsg(RunGateUtilsConst.RUNGATECODE, 0x11223344, 5,
                                                     RunGateUtilsConst.GM_CHECKSERVER, 0, null, 0);
        Assert.Equal(20, buf.Length);
        var h = StructBytes.FromBytes<TM2MsgHeader>(buf, 0);
        Assert.Equal(RunGateUtilsConst.RUNGATECODE, h.dwCode);
        Assert.Equal(0x11223344, h.nSocket);
        Assert.Equal(5, h.wGSocketIdx);
        Assert.Equal((ushort)RunGateUtilsConst.GM_CHECKSERVER, h.wIdent);
        Assert.Equal(0u, h.wUserListIndex);
        Assert.Equal(0, h.nLength);
    }

    [Fact]
    public void BuildServerMsg_WithData_AppendsTrailingNul_AndKeepsRealLength()
    {
        // 原 418/429-430：nLen = BufferLen + 20 + 1，最后一位清 0；但 nLength = BufferLen
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var buf = RunGateFrameBuilder.BuildServerMsg(RunGateUtilsConst.RUNGATECODE, 7, 1,
                                                     RunGateUtilsConst.GM_DATA, 3, data, 5);
        Assert.Equal(20 + 5 + 1, buf.Length);
        var h = StructBytes.FromBytes<TM2MsgHeader>(buf, 0);
        Assert.Equal(5, h.nLength);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, buf[20..25]);
        Assert.Equal(0, buf[25]);
    }

    [Fact]
    public void BuildServerMsg_ZeroLengthDataStillGetsTerminator()
    {
        // Buffer <> nil 但 BufferLen = 0 → 仍然多分配 1 字节并写 #0（原 414-432 的 else 分支）
        var buf = RunGateFrameBuilder.BuildServerMsg(RunGateUtilsConst.RUNGATECODE, 0, 0, 5, 0, Array.Empty<byte>(), 0);
        Assert.Equal(21, buf.Length);
        Assert.Equal(0, buf[20]);
        Assert.Equal(0, StructBytes.FromBytes<TM2MsgHeader>(buf, 0).nLength);
    }

    [Fact]
    public void EncodeRunGateMsg_Layout_CodeAndDataLenAndPayload()
    {
        var msg = TDefaultMessage.Make(1003, -1L, 7, 8, 9);
        var data = new byte[] { 0xAA, 0xBB, 0xCC };
        var buf = RunGateFrameBuilder.EncodeRunGateMsg(msg, data, 3);

        Assert.Equal(24 + 3, buf.Length);
        Assert.Equal(RunGateUtilsConst.RUN_GATE_MSG_CODE, BitConverter.ToUInt32(buf, 0));
        Assert.Equal(3u, BitConverter.ToUInt32(buf, 4));
        // Msg 从偏移 8 起
        Assert.Equal(-1L, BitConverter.ToInt64(buf, 8));
        Assert.Equal((ushort)1003, BitConverter.ToUInt16(buf, 16));
        Assert.Equal((ushort)7, BitConverter.ToUInt16(buf, 18));
        Assert.Equal(new byte[] { 0xAA, 0xBB, 0xCC }, buf[24..27]);
    }

    [Fact]
    public void EncodeRunGateMsg_NullOrNonPositiveData_DataLenZero()
    {
        // 原 3482-3485：(DataAdd <> nil) and (DataAddLen > 0) 才写长度
        var msg = TDefaultMessage.Make(1, 2, 3, 4, 5);
        Assert.Equal(24, RunGateFrameBuilder.EncodeRunGateMsg(msg, null, 0).Length);
        Assert.Equal(24, RunGateFrameBuilder.EncodeRunGateMsg(msg, Array.Empty<byte>(), 0).Length);
        Assert.Equal(24, RunGateFrameBuilder.EncodeRunGateMsg(msg, new byte[4], -1).Length);
        Assert.Equal(0u, BitConverter.ToUInt32(RunGateFrameBuilder.EncodeRunGateMsg(msg, new byte[4], -1), 4));
    }

    [Fact]
    public void MakeVersionNumber_UsesYearMonthDayPacking()
    {
        // 原 330：(Y * 10000) + (M * 100) + D
        Assert.Equal(20260920, RunGateFrameBuilder.MakeVersionNumber(2026, 9, 20));
        Assert.Equal(20000101, RunGateFrameBuilder.MakeVersionNumber(2000, 1, 1));
        Assert.Equal(19991231, RunGateFrameBuilder.MakeVersionNumber(1999, 12, 31));
    }

    [Fact]
    public void BuildRunGateVersionMsg_UsesGSocketIdx5_AndVersionInSocketField()
    {
        // 原 333-339：nSocket := IntVer; wGSocketIdx := 5
        var buf = RunGateFrameBuilder.BuildRunGateVersionMsg(20260920);
        var h = StructBytes.FromBytes<TM2MsgHeader>(buf, 0);
        Assert.Equal(0xAA55AA55u, h.dwCode);
        Assert.Equal(20260920, h.nSocket);
        Assert.Equal(5, h.wGSocketIdx);
        Assert.Equal((ushort)RunGateUtilsConst.GM_RUN_GATE_VER, h.wIdent);
        Assert.Equal(0, h.nLength);
    }

    [Fact]
    public void BuildRequestMagicListMsg_AllZeroSocketFields()
    {
        var buf = RunGateFrameBuilder.BuildRequestMagicListMsg();
        var h = StructBytes.FromBytes<TM2MsgHeader>(buf, 0);
        Assert.Equal(0xAA55AA55u, h.dwCode);
        Assert.Equal(0, h.nSocket);
        Assert.Equal(0, h.wGSocketIdx);
        Assert.Equal((ushort)RunGateUtilsConst.GM_RUN_GATE_MAGICS, h.wIdent);
        Assert.Equal(0u, h.wUserListIndex);
        Assert.Equal(0, h.nLength);
    }
}
