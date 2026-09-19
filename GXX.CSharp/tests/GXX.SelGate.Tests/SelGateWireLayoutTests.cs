using System;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>
/// ClientSession.pas SendDefMessage 的 wire 布局等价性（防止用 Marshal 与显式拼字节两条路径漂移）。
/// </summary>
public class SelGateWireLayoutTests
{
    [Fact]
    public void BytesOfWire_MatchesDefaultMessageSizeAndMarshalLayout()
    {
        Assert.Equal(16, Marshal.SizeOf<TDefaultMessage>());          // Protocol.pas:43-49 TCmdPack = 16 字节

        var m = default(TDefaultMessage);
        m.Recog = 0x1122334455667788L;
        m.Ident = 0x00AA;
        m.Param = 0x00BB;
        m.Tag = 0x00CC;
        m.Series = 0x00DD;

        byte[] wire = CSelSessionObj.BytesOfWire(m);
        Assert.Equal(16, wire.Length);
        // 小端逐字段
        Assert.Equal(new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 }, wire.AsSpan(0, 8).ToArray());
        Assert.Equal(new byte[] { 0xAA, 0x00 }, wire.AsSpan(8, 2).ToArray());
        Assert.Equal(new byte[] { 0xBB, 0x00 }, wire.AsSpan(10, 2).ToArray());
        Assert.Equal(new byte[] { 0xCC, 0x00 }, wire.AsSpan(12, 2).ToArray());
        Assert.Equal(new byte[] { 0xDD, 0x00 }, wire.AsSpan(14, 2).ToArray());
    }

    [Fact]
    public void BytesOfWire_RoundTripsThroughMarshalForFreshStruct()
    {
        // 对"逐字段一次性构造"的结构体，显式拼字节必须与 Marshal 完全一致
        var m = default(TDefaultMessage);
        m.Recog = 0x1122334455667788L;
        m.Ident = 100;
        m.Param = 33;
        m.Tag = 22;
        m.Series = 33;
        byte[] viaMarshal = StructBytes.BytesOf(m);
        byte[] viaWire = CSelSessionObj.BytesOfWire(m);
        Assert.Equal(viaMarshal, viaWire);
    }

    [Fact]
    public void BytesOfWire_AssignmentOrderOfOriginalCode_KeepsParamOverriddenBySeries()
    {
        // 复刻 ClientSession.pas:104-108 的赋值顺序：param 先写 nParam，再被 nSeries 覆盖
        ushort nParam = 11, nTag = 22, nSeries = 33;
        var m = default(TDefaultMessage);
        m.Recog = 0x1122334455667788L;
        m.Ident = 100;
        m.Param = nParam;
        m.Tag = nTag;
        m.Param = nSeries;
        byte[] wire = CSelSessionObj.BytesOfWire(m);
        Assert.Equal(new byte[] { 0x21, 0x00 }, wire.AsSpan(10, 2).ToArray()); // param = 33
        Assert.Equal(new byte[] { 0x16, 0x00 }, wire.AsSpan(12, 2).ToArray()); // tag = 22
    }
}
