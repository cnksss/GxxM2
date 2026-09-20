using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsFrame.cs 测试 —— 覆盖 RunGateUtils.pas:439-529（严格模式 DoCheckRecvBuffer）
/// 与 747-924（宽松模式 ProcessDecompressPacket）、1367-1378（全服消息切分）。
/// </summary>
public class RunGateUtilsFrameTests
{
    // ------------------------------------------------------------------
    // 测试用帧构造（与生产代码无关，避免"用被测代码造测试数据"）
    // ------------------------------------------------------------------

    private static byte[] Frame(int ident, byte[] payload, uint code = RunGateUtilsConst.RUNGATECODE,
                               int socketValue = 0, ushort wSocketIdx = 0, int? lengthOverride = null)
    {
        payload ??= Array.Empty<byte>();
        var buf = new byte[20 + payload.Length];
        BitConverter.GetBytes(code).CopyTo(buf, 0);
        BitConverter.GetBytes(socketValue).CopyTo(buf, 4);
        BitConverter.GetBytes(wSocketIdx).CopyTo(buf, 8);
        BitConverter.GetBytes((ushort)ident).CopyTo(buf, 10);
        BitConverter.GetBytes(0u).CopyTo(buf, 12);
        BitConverter.GetBytes(lengthOverride ?? payload.Length).CopyTo(buf, 16);
        payload.CopyTo(buf, 20);
        return buf;
    }

    private static byte[] Concat(params byte[][] parts)
    {
        int n = 0;
        foreach (var p in parts) n += p.Length;
        var r = new byte[n];
        int o = 0;
        foreach (var p in parts) { p.CopyTo(r, o); o += p.Length; }
        return r;
    }

    // ------------------------------------------------------------------
    // ScanM2Stream（严格模式）
    // ------------------------------------------------------------------

    [Fact]
    public void ScanM2_BufferShorterThanHeader_ReturnsNoPacketsAndKeepsEverything()
    {
        // 原 450：if Len >= SizeOf(TM2MsgHeader) —— 不足 20 直接返回，缓冲原样保留
        var buf = new byte[] { 1, 2, 3, 4, 5 };
        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Empty(r.Packets);
        Assert.False(r.HasError);
        Assert.Equal(0, r.LeftoverOffset);
        Assert.Equal(5, r.LeftoverLength);
        Assert.Equal(buf, RunGateFrameScanner.ExtractLeftover(buf, r));
    }

    [Fact]
    public void ScanM2_SingleCompleteFrame_ConsumedWithNoLeftover()
    {
        var f = Frame(RunGateUtilsConst.GM_CHECKSERVER, Array.Empty<byte>());
        var r = RunGateFrameScanner.ScanM2Stream(f, f.Length);
        Assert.False(r.HasError);
        Assert.Single(r.Packets);
        Assert.Equal(0, r.Packets[0].Offset);
        Assert.Equal(20, r.Packets[0].Length);
        Assert.Equal(0, r.Packets[0].DataLength);
        Assert.Equal(0, r.LeftoverLength);
    }

    [Fact]
    public void ScanM2_TwoFrames_LeftoverIsExactTail()
    {
        // 第二帧不完整（只给 10 字节头）→ 第一帧被消费，剩下 10 字节作为尾巴
        var f1 = Frame(RunGateUtilsConst.GM_DATA, new byte[6]);
        var f2 = Frame(RunGateUtilsConst.GM_CHECKCLIENT, Array.Empty<byte>());
        var buf = Concat(f1, f2.AsSpan(0, 10).ToArray());

        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Single(r.Packets);
        Assert.Equal(26, r.Packets[0].Length);
        Assert.Equal(26, r.LeftoverOffset);
        Assert.Equal(10, r.LeftoverLength);
        Assert.Equal(f2.AsSpan(0, 10).ToArray(), RunGateFrameScanner.ExtractLeftover(buf, r));
    }

    [Fact]
    public void ScanM2_BadMagic_AbortsWholeBuffer()
    {
        // 原 469-474：整段作废（S := ''），已消费的帧仍然有效但尾巴不再保留。
        // 注意：坏帧必须让"消费完第一帧后剩余 > 20"，否则会先命中 `Len <= 20 → 留尾巴` 而看不到坏帧。
        var good = Frame(RunGateUtilsConst.GM_DATA, new byte[2]);          // 22 字节
        var bad = Frame(0, new byte[4], code: 0x12345678);                 // 24 字节
        var buf = Concat(good, bad);                                       // 46 字节

        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(RunGateFrameError.BadMagic, r.Error);
        Assert.Single(r.Packets);                      // 坏帧之前的那一帧已被处理
        Assert.Equal(0, r.LeftoverLength);
    }

    [Fact]
    public void ScanM2_BadMagicInTailExactly20Bytes_IsNotDetected_LeftoverWinsFirst()
    {
        // 差异边界：若消费完最后一帧后恰好剩 20 字节，`Len <= SizeOf(TM2MsgHeader)` 先命中，
        // 坏帧被当作"未处理尾巴"原样留着，**不会**报 BadMagic。这是原文的判定顺序，勿"修正"。
        var good = Frame(RunGateUtilsConst.GM_DATA, new byte[2]);          // 22 字节
        var bad = Frame(0, Array.Empty<byte>(), code: 0x12345678);         // 20 字节
        var buf = Concat(good, bad);

        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(RunGateFrameError.None, r.Error);
        Assert.Single(r.Packets);
        Assert.Equal(22, r.LeftoverOffset);
        Assert.Equal(20, r.LeftoverLength);
    }

    [Fact]
    public void ScanM2_VersionMismatch_IsDistinctErrorCarryingM2Version()
    {
        // 原 458-467：魔术不符 **且** wIdent 恰好等于 GM_RUN_GATE_VER → 版本不匹配提示
        var buf = new byte[20];
        BitConverter.GetBytes(0x0BADF00Du).CopyTo(buf, 0);
        BitConverter.GetBytes(20260920).CopyTo(buf, 4);                        // nSocket = M2 版本
        BitConverter.GetBytes((ushort)RunGateUtilsConst.GM_RUN_GATE_VER).CopyTo(buf, 10);

        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(RunGateFrameError.VersionMismatch, r.Error);
        Assert.Equal(20260920, r.VersionMismatchVersion);
        Assert.Equal(0, r.LeftoverLength);
    }

    [Fact]
    public void ScanM2_CompDataWithWrongGateCode_IsRejected()
    {
        // 原 492-505：GM_COMPDATA 的 LongWord(nSocket) 必须等于 RUNGATECODEX
        var buf = Frame(RunGateUtilsConst.GM_COMPDATA, new byte[4], code: RunGateUtilsConst.RUNGATECODE,
                        socketValue: unchecked((int)RunGateUtilsConst.RUN_GATE_MSG_CODE));
        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(RunGateFrameError.BadCompDataCode, r.Error);
        Assert.Empty(r.Packets);
    }

    [Fact]
    public void ScanM2_CompDataWithRightGateCode_IsAccepted()
    {
        var buf = Frame(RunGateUtilsConst.GM_COMPDATA, new byte[4], code: RunGateUtilsConst.RUNGATECODE,
                        socketValue: unchecked((int)RunGateUtilsConst.RUNGATECODEX));
        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.False(r.HasError);
        Assert.Single(r.Packets);
        Assert.Equal((ushort)RunGateUtilsConst.GM_COMPDATA, r.Packets[0].Header.wIdent);
    }

    [Fact]
    public void ScanM2_Exactly20BytesLeftover_IsRetainedNotParsed()
    {
        // 原 519：Len <= SizeOf(TM2MsgHeader) → 留尾巴。恰好 20 字节也必须留（而非当帧解析）
        var f1 = Frame(RunGateUtilsConst.GM_DATA, new byte[1]);
        var f2 = Frame(RunGateUtilsConst.GM_CHECKSERVER, Array.Empty<byte>());
        var buf = Concat(f1, f2);                       // 21 + 20 = 41

        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Single(r.Packets);
        Assert.Equal(21, r.LeftoverOffset);
        Assert.Equal(20, r.LeftoverLength);
    }

    [Fact]
    public void ScanM2_LengthExactlyPacketLen_IsComplete()
    {
        // 原 481 用 `<`（严格小于）→ 等长算完整帧
        var buf = Frame(RunGateUtilsConst.GM_DATA, new byte[4]);
        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Single(r.Packets);
        Assert.Equal(24, r.Packets[0].Length);
        Assert.Equal(0, r.LeftoverLength);
    }

    [Fact]
    public void ScanM2_NegativeLength_UsesAbsoluteValue()
    {
        // 原 478/908：Abs(MsgHeader.nLength)
        var buf = new byte[24];
        BitConverter.GetBytes(RunGateUtilsConst.RUNGATECODE).CopyTo(buf, 0);
        BitConverter.GetBytes((ushort)RunGateUtilsConst.GM_DATA).CopyTo(buf, 10);
        BitConverter.GetBytes(-4).CopyTo(buf, 16);

        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Single(r.Packets);
        Assert.Equal(24, r.Packets[0].Length);
        Assert.Equal(4, r.Packets[0].DataLength);
    }

    // ------------------------------------------------------------------
    // 差异断言：严格模式 vs 宽松模式
    // ------------------------------------------------------------------

    [Fact]
    public void Differential_BadMagic_StrictAbortsButLooseResyncs()
    {
        var junk = new byte[] { 0x11, 0x22, 0x33 };                   // 3 字节垃圾
        var good = Frame(RunGateUtilsConst.GM_CHECKSERVER, new byte[2]);
        var buf = Concat(junk, good);

        var strict = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(RunGateFrameError.BadMagic, strict.Error);
        Assert.Empty(strict.Packets);

        var loose = RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length);
        Assert.False(loose.HasError);
        Assert.Single(loose.Packets);
        Assert.Equal(3, loose.Packets[0].Offset);                    // 逐字节重同步后找到真帧
        Assert.Equal(good.Length, loose.Packets[0].Length);
    }

    [Fact]
    public void Differential_IncompleteFrame_StrictKeepsLeftoverButLooseDropsIt()
    {
        var f1 = Frame(RunGateUtilsConst.GM_DATA, new byte[4]);
        var partial = Frame(RunGateUtilsConst.GM_CHECKCLIENT, new byte[50]).AsSpan(0, 30).ToArray();
        var buf = Concat(f1, partial);

        var strict = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Single(strict.Packets);
        Assert.Equal(30, strict.LeftoverLength);

        var loose = RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length);
        Assert.Single(loose.Packets);
        Assert.Equal(0, loose.LeftoverLength);                       // Break → 尾巴直接丢
        Assert.Equal(24, loose.LeftoverOffset);
    }

    [Fact]
    public void Differential_LooseMode_DoesNotApplyCompDataGateCodeCheck()
    {
        // 严格模式会因为 nSocket != RUNGATECODEX 整段作废；
        // 宽松模式（ProcessDecompressPacket）**根本没有 COMPDATA 分支**，照收。
        var buf = Frame(RunGateUtilsConst.GM_COMPDATA, new byte[4], code: RunGateUtilsConst.RUNGATECODE,
                        socketValue: 0x12345678);
        Assert.Equal(RunGateFrameError.BadCompDataCode,
                     RunGateFrameScanner.ScanM2Stream(buf, buf.Length).Error);
        Assert.Single(RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length).Packets);
    }

    [Fact]
    public void Boundary_Exactly20BytesWithZeroLengthPayload_IsACompleteFrameForBothScanners()
    {
        // 严格模式判据是 `remain <= 20 → 留尾巴`（原 519），宽松模式是 `remain < 20 → Break`（原 916）。
        // 差异只在下一次迭代才体现；对"缓冲恰好 20 字节且正好是一个 0 长负载帧"这一情形，
        // 两者都必须把它当**完整帧**消费掉（这是 GM_CHECKCLIENT/心跳帧的常见形态）。
        var buf = Frame(RunGateUtilsConst.GM_CHECKSERVER, Array.Empty<byte>());
        var strict = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        var loose = RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length);
        Assert.Single(strict.Packets);
        Assert.Equal(0, strict.LeftoverLength);
        Assert.Single(loose.Packets);
        Assert.Equal(0, loose.LeftoverLength);
    }

    [Fact]
    public void Differential_LooseMode_Treats20ByteRemainderAsScannable_StrictRetainsIt()
    {
        // 真正的边界差异：缓冲尾部剩 20 字节。
        // 严格模式（原 519 `Len <= 20`）保留为尾巴并退出；宽松模式（原 916 `nLen < 20`）继续迭代。
        // 用一个 21 字节帧 + 尾部 20 字节垃圾来观察：宽松模式会逐字节把垃圾耗掉，
        // 严格模式则原样留 20 字节尾巴。
        var f1 = Frame(RunGateUtilsConst.GM_DATA, new byte[1]);      // 21 字节
        var junk = new byte[20];
        for (int i = 0; i < junk.Length; i++) junk[i] = (byte)(i + 1);
        var buf = Concat(f1, junk);

        var strict = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Single(strict.Packets);
        Assert.Equal(20, strict.LeftoverLength);
        Assert.Equal(junk, RunGateFrameScanner.ExtractLeftover(buf, strict));

        var loose = RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length);
        Assert.Single(loose.Packets);
        Assert.Equal(0, loose.LeftoverLength);                       // 尾部 20 字节被丢弃（只重同步了 1 字节）
        Assert.Equal(22, loose.LeftoverOffset);                      // 21（帧）+ 1（重同步）
    }

    [Fact]
    public void Differential_LooseMode_StopsOnHugeDeclaredLength()
    {
        // 声明长度大于剩余 → 严格模式留尾巴，宽松模式 Break 丢弃
        var buf = new byte[24];
        BitConverter.GetBytes(RunGateUtilsConst.RUNGATECODE).CopyTo(buf, 0);
        BitConverter.GetBytes((ushort)RunGateUtilsConst.GM_DATA).CopyTo(buf, 10);
        BitConverter.GetBytes(1000).CopyTo(buf, 16);

        Assert.Equal(24, RunGateFrameScanner.ScanM2Stream(buf, buf.Length).LeftoverLength);
        Assert.Empty(RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length).Packets);
        Assert.Equal(0, RunGateFrameScanner.ScanDecompressedStream(buf, buf.Length).LeftoverLength);
    }

    // ------------------------------------------------------------------
    // 全服消息切分
    // ------------------------------------------------------------------

    [Fact]
    public void FullServiceMsg_Exactly16Bytes_IsDropped()
    {
        // 原 1371：`BufferLen > SizeOf(TDefaultMessage)` 是**严格大于**
        var buf = new byte[16];
        Assert.False(RunGateFrameScanner.TrySplitFullServiceMsg(buf, 16, out _, out _, out _));
    }

    [Fact]
    public void FullServiceMsg_17Bytes_YieldsOneBytePayload()
    {
        var buf = new byte[17];
        BitConverter.GetBytes((ushort)1234).CopyTo(buf, 8);
        buf[16] = 0x5A;

        Assert.True(RunGateFrameScanner.TrySplitFullServiceMsg(buf, buf.Length, out var msg, out int off, out int len));
        Assert.Equal((ushort)1234, msg.Ident);
        Assert.Equal(16, off);
        Assert.Equal(1, len);
        Assert.Equal(0x5A, buf[off]);
    }

    [Fact]
    public void FullServiceMsg_PayloadKeepsTrailingNul()
    {
        // 原 1375 注释"后面有个 #0 这里干脆不要算了"，但代码并**没有**减 1 —— 减去动作在消费侧
        var buf = new byte[20];
        buf[19] = 0;
        Assert.True(RunGateFrameScanner.TrySplitFullServiceMsg(buf, buf.Length, out _, out _, out int len));
        Assert.Equal(4, len);
    }

    [Fact]
    public void FullServiceMsg_NullOrTooShort_ReturnsFalse()
    {
        Assert.False(RunGateFrameScanner.TrySplitFullServiceMsg(null, 0, out _, out _, out _));
        Assert.False(RunGateFrameScanner.TrySplitFullServiceMsg(new byte[4], 4, out _, out _, out _));
        Assert.False(RunGateFrameScanner.TrySplitFullServiceMsg(new byte[32], 0, out _, out _, out _));
    }

    // ------------------------------------------------------------------
    // 结果对象基本不变量
    // ------------------------------------------------------------------

    [Fact]
    public void ScanResult_LeftoverEnd_IsConsistent()
    {
        var f1 = Frame(RunGateUtilsConst.GM_DATA, new byte[3]);
        var buf = Concat(f1, new byte[7]);
        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(23, r.LeftoverOffset);
        Assert.Equal(7, r.LeftoverLength);
        Assert.Equal(30, r.LeftoverEnd);
    }

    [Fact]
    public void ExtractLeftover_ZeroLength_ReturnsEmptyArray()
    {
        var r = new RunGateFrameScanResult();
        Assert.Empty(RunGateFrameScanner.ExtractLeftover(new byte[4], r));
    }

    [Fact]
    public void Scan_EmptyOrNullBuffer_IsNoOp()
    {
        Assert.Empty(RunGateFrameScanner.ScanM2Stream(null, 0).Packets);
        Assert.Empty(RunGateFrameScanner.ScanM2Stream(new byte[0], 0).Packets);
        Assert.Empty(RunGateFrameScanner.ScanDecompressedStream(new byte[0], 0).Packets);
        Assert.Empty(RunGateFrameScanner.ScanDecompressedStream(null, 10).Packets);
    }

    [Fact]
    public void ScanM2_ManyFrames_AllConsumed()
    {
        var parts = new List<byte[]>();
        for (int i = 0; i < 5; i++) parts.Add(Frame(RunGateUtilsConst.GM_DATA, new byte[i]));
        var buf = Concat(parts.ToArray());
        var r = RunGateFrameScanner.ScanM2Stream(buf, buf.Length);
        Assert.Equal(5, r.Packets.Count);
        Assert.Equal(0, r.LeftoverLength);
        for (int i = 0; i < 5; i++) Assert.Equal(i, r.Packets[i].DataLength);
    }
}
