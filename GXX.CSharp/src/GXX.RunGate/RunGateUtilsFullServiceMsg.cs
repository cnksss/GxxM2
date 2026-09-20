using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

// 源：Source/RunGate/RunGateUtils.pas
//   TRunGateManager.AddFullServiceMsg      4478-4500
//   TRunGateManager.ClearFullServiceMsgList 4502-4524
//   TRunGateManager.OnTimerRunContext       4331-4476（**死分支**：MultiThreadRunContext=0 才编译，
//       而 Grobal2_Ex.pas:24 定死 MultiThreadRunContext=1；其排空逻辑被下面这个队列忠实保留，
//       因为 MultiThreadRunContext=1 时排空移到了 TFullServiceMsgProcessThread.Execute（3256-3381））
// 全服消息 = 「TDefaultMessage 头 + 负载（尾部含一个 #0）」，由 TClientMsg/PCientMsg 链表承载。

namespace GXX.RunGate;

/// <summary>TClientMsg（RunGateUtils.pas 引用的 PClientMsg 记录）在托管侧的值语义表示。</summary>
public sealed class RunGateFullServiceMsg
{
    public TDefaultMessage DefMessage;
    /// <summary>原文 pBuffer 的字节数（含尾部 #0）。</summary>
    public byte[] Buffer;

    public int BufferLen => Buffer?.Length ?? 0;
}

/// <summary>
/// 全服消息队列（RunGateUtils.pas:210 <c>FFullServiceMsgList: TSafeList</c> 的托管等价）。
/// </summary>
public sealed class RunGateFullServiceMsgQueue
{
    /// <summary>RunGateUtils.pas:4363 —— 一次最多编码 10 条。</summary>
    public const int MaxMessagesPerDrain = 10;

    private readonly List<RunGateFullServiceMsg> _list = new();
    private readonly object _lock = new();

    public int Count { get { lock (_lock) return _list.Count; } }

    /// <summary>
    /// RunGateUtils.pas:4478-4500 —— 入队。
    /// <c>BufSize &lt;= 0</c> 时不分配负载（pBuffer 保持 nil）。
    /// 原文**没有队列长度上限**：M2 持续灌全服消息而排空线程停摆时会无限增长。
    /// </summary>
    public void Add(in TDefaultMessage defMsg, byte[] buffer, int bufSize)
    {
        var item = new RunGateFullServiceMsg { DefMessage = defMsg, Buffer = null };
        if (bufSize > 0 && buffer != null)
        {
            var copy = new byte[bufSize];
            Array.Copy(buffer, 0, copy, 0, bufSize);
            item.Buffer = copy;
        }

        lock (_lock) _list.Add(item);
    }

    /// <summary>RunGateUtils.pas:4502-4524 —— ClearFullServiceMsgList：全部释放并清空。</summary>
    public void Clear()
    {
        lock (_lock) _list.Clear();
    }

    /// <summary>
    /// RunGateUtils.pas:4345-4417 —— 排空一轮。
    /// <para>
    /// <b>原文语义（有损，照原样保留）</b>：整个 while 循环把所有条目都出队并释放，
    /// 但只有**前 <see cref="MaxMessagesPerDrain"/> 条且带负载**的会被编码进待发文本；
    /// 其余条目直接丢弃（第 11 条起、以及 pBuffer=nil / nBufferLen&lt;=0 的条目）。
    /// </para>
    /// <para>
    /// 编码入参为 <c>nBufferLen - 1</c>（RunGateUtils.pas:4402）—— 去掉尾部那个 #0。
    /// 因此 <c>nBufferLen == 1</c> 的负载编码出 0 长度数据（不是负长度）。
    /// </para>
    /// </summary>
    /// <param name="encoded">按顺序编码出的帧（每条 = TRungateMsgHeader + 负载）。</param>
    /// <param name="dropped">被直接丢弃的条目数。</param>
    /// <returns>本轮出队的条目总数。</returns>
    public int Drain(List<byte[]> encoded, out int dropped)
    {
        encoded ??= new List<byte[]>();
        dropped = 0;

        List<RunGateFullServiceMsg> snapshot;
        lock (_lock)
        {
            snapshot = new List<RunGateFullServiceMsg>(_list);
            _list.Clear();
        }

        int count = 0;
        foreach (var item in snapshot)
        {
            bool hasPayload = item.Buffer != null && item.Buffer.Length > 0;
            if (count < MaxMessagesPerDrain)
            {
                if (hasPayload)
                    encoded.Add(RunGateFrameBuilder.EncodeRunGateMsg(item.DefMessage, item.Buffer, item.BufferLen - 1));
                else
                    dropped++;
            }
            else
            {
                dropped++;
            }
            count++;
        }

        return count;
    }

    /// <summary>便捷重载：只关心编码结果。</summary>
    public List<byte[]> Drain()
    {
        var list = new List<byte[]>();
        Drain(list, out _);
        return list;
    }
}
