using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsFullServiceMsg.cs 测试 —— 覆盖 RunGateUtils.pas:4478-4500（入队）、
/// 4502-4524（清空）、4345-4417（排空，含"只发 10 条但全部出队"的有损语义）。
/// </summary>
public class RunGateUtilsFullServiceMsgTests
{
    private static TDefaultMessage Msg(ushort ident, long recog = 0)
        => TDefaultMessage.Make(ident, recog, 0, 0, 0);

    [Fact]
    public void Add_WithPayload_CopiesBuffer()
    {
        var q = new RunGateFullServiceMsgQueue();
        var buf = new byte[] { 1, 2, 3, 0 };
        q.Add(Msg(658), buf, buf.Length);
        buf[0] = 0xFF;

        Assert.Equal(1, q.Count);
        var encoded = q.Drain();
        Assert.Single(encoded);
        // 编码长度 = 24 头 + (BufSize - 1) = 24 + 3
        Assert.Equal(27, encoded[0].Length);
        Assert.Equal(3u, BitConverter.ToUInt32(encoded[0], 4));   // DataLen
        Assert.Equal(new byte[] { 1, 2, 3 }, encoded[0][24..27]);
    }

    [Fact]
    public void Add_NonPositiveBufSize_KeepsNullPayload()
    {
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(1), null, 0);
        q.Add(Msg(2), new byte[4], 0);
        q.Add(Msg(3), new byte[4], -5);
        Assert.Equal(3, q.Count);

        var encoded = q.Drain();
        Assert.Empty(encoded);                                   // 三个都无负载 → 都丢弃
    }

    [Fact]
    public void Add_NullBufferWithPositiveSize_DoesNotThrow()
    {
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(1), null, 100);
        Assert.Equal(1, q.Count);
        Assert.Empty(q.Drain());
    }

    [Fact]
    public void Drain_TrimsTrailingNul_FromBufferLen()
    {
        // 原 4402：EncodeRunGateMsg(@DefMessage, pBuffer, nBufferLen - 1)
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(658), new byte[] { 0xAA, 0xBB, 0x00 }, 3);
        var encoded = q.Drain();
        Assert.Single(encoded);
        Assert.Equal(24 + 2, encoded[0].Length);
        Assert.Equal(2u, BitConverter.ToUInt32(encoded[0], 4));
    }

    [Fact]
    public void Drain_SingleBytePayload_BecomesZeroLengthData()
    {
        // nBufferLen = 1 → 1 - 1 = 0 → DataLen 0（不是负数/下溢）
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(1), new byte[] { 0 }, 1);
        var encoded = q.Drain();
        Assert.Single(encoded);
        Assert.Equal(24, encoded[0].Length);
        Assert.Equal(0u, BitConverter.ToUInt32(encoded[0], 4));
    }

    [Fact]
    public void Drain_AtMostTenMessages_ButAllAreDequeued()
    {
        // 原 4363-4365：Count < 10 才编码；但 while 循环把**所有**条目都出队
        var q = new RunGateFullServiceMsgQueue();
        for (int i = 0; i < 12; i++) q.Add(Msg((ushort)i), new byte[] { (byte)i, 0 }, 2);

        int total = q.Drain(new List<byte[]>(), out int dropped);
        Assert.Equal(0, q.Count);
        Assert.Equal(12, total);
        Assert.Equal(2, dropped);
    }

    [Fact]
    public void Drain_TenMessages_NoneDropped()
    {
        var q = new RunGateFullServiceMsgQueue();
        for (int i = 0; i < 10; i++) q.Add(Msg((ushort)i), new byte[] { (byte)i, 0 }, 2);

        var list = new List<byte[]>();
        int total = q.Drain(list, out int dropped);
        Assert.Equal(10, total);
        Assert.Equal(0, dropped);
        Assert.Equal(10, list.Count);
    }

    [Fact]
    public void Drain_PayloadLessEntriesConsumeTheTenSlotWindow()
    {
        // 前 10 条里第 0 条无负载 → 它占用配额但被丢弃；只有第 1..9 条被编码；第 10/11 条丢弃
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(0), null, 0);
        for (int i = 1; i < 12; i++) q.Add(Msg((ushort)i), new byte[] { (byte)i, 0 }, 2);

        var list = new List<byte[]>();
        int total = q.Drain(list, out int dropped);
        Assert.Equal(12, total);
        Assert.Equal(9, list.Count);
        Assert.Equal(3, dropped);
    }

    [Fact]
    public void Drain_PreservesInsertionOrder()
    {
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(11), new byte[] { 0x11, 0 }, 2);
        q.Add(Msg(22), new byte[] { 0x22, 0 }, 2);
        q.Add(Msg(33), new byte[] { 0x33, 0 }, 2);

        var list = q.Drain();
        Assert.Equal(3, list.Count);
        Assert.Equal((ushort)11, BitConverter.ToUInt16(list[0], 16));
        Assert.Equal((ushort)22, BitConverter.ToUInt16(list[1], 16));
        Assert.Equal((ushort)33, BitConverter.ToUInt16(list[2], 16));
    }

    [Fact]
    public void Drain_EmptiesQueue()
    {
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(1), new byte[] { 1, 0 }, 2);
        q.Drain();
        Assert.Equal(0, q.Count);
        Assert.Empty(q.Drain());
    }

    [Fact]
    public void Clear_DropsEverything()
    {
        var q = new RunGateFullServiceMsgQueue();
        for (int i = 0; i < 5; i++) q.Add(Msg((ushort)i), new byte[] { 1, 0 }, 2);
        q.Clear();
        Assert.Equal(0, q.Count);
        Assert.Empty(q.Drain());
    }

    [Fact]
    public void Drain_OnEmptyQueue_IsNoOp()
    {
        var q = new RunGateFullServiceMsgQueue();
        var list = new List<byte[]>();
        Assert.Equal(0, q.Drain(list, out int dropped));
        Assert.Equal(0, dropped);
        Assert.Empty(list);
    }

    [Fact]
    public void Drain_NullTargetList_StillDequeues()
    {
        var q = new RunGateFullServiceMsgQueue();
        q.Add(Msg(1), new byte[] { 1, 0 }, 2);
        Assert.Equal(1, q.Drain(null, out _));
        Assert.Equal(0, q.Count);
    }

    [Fact]
    public void MaxMessagesPerDrain_IsTen()
    {
        Assert.Equal(10, RunGateFullServiceMsgQueue.MaxMessagesPerDrain);
    }

    [Fact]
    public void Drain_IsThreadSafeUnderConcurrentAdd()
    {
        var q = new RunGateFullServiceMsgQueue();
        var tasks = new List<System.Threading.Tasks.Task>();
        for (int t = 0; t < 8; t++)
            tasks.Add(System.Threading.Tasks.Task.Run(() =>
            {
                for (int i = 0; i < 100; i++) q.Add(Msg(1), new byte[] { 1, 0 }, 2);
            }));
        System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        Assert.Equal(800, q.Count);

        int total = q.Drain(new List<byte[]>(), out _);
        Assert.Equal(800, total);
        Assert.Equal(0, q.Count);
    }
}

/// <summary>RunGateFullServiceMsg 值对象的边界。</summary>
public class RunGateFullServiceMsgValueTests
{
    [Fact]
    public void BufferLen_NullBuffer_IsZero()
    {
        var m = new RunGateFullServiceMsg();
        Assert.Equal(0, m.BufferLen);
    }

    [Fact]
    public void BufferLen_ReflectsArrayLength()
    {
        var m = new RunGateFullServiceMsg { Buffer = new byte[7] };
        Assert.Equal(7, m.BufferLen);
    }
}
