using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

// 源：Source/RunGate/RunGateUtils.pas
//   TMirRemoteContext.DoCheckRecvBuffer      439-529（M2 链路收包重组：严格模式，坏帧即断）
//   TMirRemoteContext.ProcessDecompressPacket 747-924（解压后缓冲：宽松模式，坏字节跳 1 重同步）
//   TMirRemoteContext.DoRecvFullServiceMsg   1367-1378（全服消息切分）
// 这两个扫描循环**看起来一样实则不同**，差异见 RunGateUtilsFrameTests 的差异断言。

namespace GXX.RunGate;

/// <summary>帧扫描错误类别（对应原文里那几条 AddMainLogMsg 分支）。</summary>
public enum RunGateFrameError
{
    None = 0,
    /// <summary>RunGateUtils.pas:472 — 首帧魔术不符且 Ident 也不是 GM_RUN_GATE_VER。</summary>
    BadMagic = 1,
    /// <summary>RunGateUtils.pas:502 — GM_COMPDATA 的 nSocket != RUNGATECODEX。</summary>
    BadCompDataCode = 2,
    /// <summary>RunGateUtils.pas:458-467 — 魔术不符且 Ident == GM_RUN_GATE_VER：网关与 M2 不配套。</summary>
    VersionMismatch = 3,
}

/// <summary>一个完整帧的切片（Offset 指向 TM2MsgHeader 起始）。</summary>
public readonly struct RunGatePacketSlice
{
    public readonly int Offset;
    public readonly int Length;
    public readonly TM2MsgHeader Header;

    public RunGatePacketSlice(int offset, int length, in TM2MsgHeader header)
    {
        Offset = offset;
        Length = length;
        Header = header;
    }

    /// <summary>负载起始（= Offset + 20）。</summary>
    public int DataOffset => Offset + RunGateUtilsConst.SizeOfTM2MsgHeader;

    /// <summary>负载长度（Abs(nLength)）。</summary>
    public int DataLength => Math.Abs(Header.nLength);
}

/// <summary>扫描结果：完整帧 + 尚未完整、需要留在缓冲里的尾巴 + 首个错误。</summary>
public sealed class RunGateFrameScanResult
{
    public readonly List<RunGatePacketSlice> Packets = new();
    /// <summary>未消费的尾部字节数（原文把自己 Move 回 S 的那段）。</summary>
    public int LeftoverOffset;
    public int LeftoverLength;
    /// <summary>VersionMismatch 时携带对方版本号（原文用 MsgHeader.nSocket 拼提示串）。</summary>
    public int VersionMismatchVersion;
    public RunGateFrameError Error = RunGateFrameError.None;

    public bool HasError => Error != RunGateFrameError.None;
    public int LeftoverEnd => LeftoverOffset + LeftoverLength;
}

/// <summary>
/// RunGate 的两个收包扫描循环（纯函数，不碰 socket）。
/// </summary>
public static class RunGateFrameScanner
{
    /// <summary>
    /// RunGateUtils.pas:439-529 — <c>TMirRemoteContext.DoCheckRecvBuffer</c>（M2 → 网关方向）。
    /// <para>
    /// 语义（逐行对齐）：若缓冲 &lt; 20 直接返回（Result=False，缓冲原样保留）；
    /// 否则循环——魔术不符则整段作废（GM_RUN_GATE_VER 特判为版本不匹配）；<c>PacketLen = |nLength| + 20</c>
    /// 不足则把 [P, P+Len) 前移成新缓冲并退出；GM_COMPDATA 还要校验 <c>(uint)nSocket == RUNGATECODEX</c>；
    /// 每消费一帧后 Len==0 → 清空，Len &lt;= 20 → 留尾巴退出，否则继续循环。
    /// </para>
    /// 注意：原文用 <c>Len &lt; PacketLen</c>（严格小于）判“不完整”，因此**恰好等长**也算完整帧。
    /// </summary>
    public static RunGateFrameScanResult ScanM2Stream(byte[] buf, int len)
    {
        var r = new RunGateFrameScanResult();
        if (buf == null || len <= 0) return r;

        int p = 0;
        int remain = len;

        // 原 450：if Len >= SizeOf(TM2MsgHeader)
        if (remain < RunGateUtilsConst.SizeOfTM2MsgHeader) { r.LeftoverOffset = 0; r.LeftoverLength = remain; return r; }

        while (true)
        {
            var hdr = ReadHeader(buf, p);

            if (hdr.dwCode != RunGateUtilsConst.RUNGATECODE)
            {
                if (hdr.wIdent == RunGateUtilsConst.GM_RUN_GATE_VER)
                {
                    // 原 458-467：提示串写回 S 并打 5 次日志，然后把 S 清空（缓冲整体丢弃）
                    r.Error = RunGateFrameError.VersionMismatch;
                    r.VersionMismatchVersion = hdr.nSocket;
                }
                else
                {
                    // 原 469-474：S := ''; 日志 'error 1'
                    r.Error = RunGateFrameError.BadMagic;
                }
                r.LeftoverOffset = 0;
                r.LeftoverLength = 0;
                return r;
            }

            int packetLen = Math.Abs(hdr.nLength) + RunGateUtilsConst.SizeOfTM2MsgHeader;

            if (remain < packetLen)
            {
                // 原 481-490：不完整 —— 若 P 已前移则把尾巴前移，否则原样
                r.LeftoverOffset = p;
                r.LeftoverLength = remain;
                return r;
            }

            if (hdr.wIdent == RunGateUtilsConst.GM_COMPDATA)
            {
                if ((uint)hdr.nSocket != RunGateUtilsConst.RUNGATECODEX)
                {
                    // 原 501-505：error 2，整段作废
                    r.Error = RunGateFrameError.BadCompDataCode;
                    r.LeftoverOffset = 0;
                    r.LeftoverLength = 0;
                    return r;
                }
            }

            r.Packets.Add(new RunGatePacketSlice(p, packetLen, hdr));
            p += packetLen;
            remain -= packetLen;

            if (remain == 0)
            {
                // 原 514-518：S := ''
                r.LeftoverOffset = p;
                r.LeftoverLength = 0;
                return r;
            }
            if (remain <= RunGateUtilsConst.SizeOfTM2MsgHeader)
            {
                // 原 519-525：留下不足一帧的尾巴
                r.LeftoverOffset = p;
                r.LeftoverLength = remain;
                return r;
            }
            // 原 526：否则继续 while True
        }
    }

    /// <summary>
    /// RunGateUtils.pas:747-924 — <c>TMirRemoteContext.ProcessDecompressPacket</c>（GM_COMPDATA 解压后）。
    /// <para>
    /// 与 <see cref="ScanM2Stream"/> 的**四处实质差异**（务必按原文保留，勿"统一"）：
    /// (1) 魔术不符时 <b>前进 1 字节继续扫描</b>（重同步），不是整段作废；
    /// (2) 帧长不足时 <b>Break</b>，尾部直接丢弃（不产生 Leftover）；
    /// (3) <b>不处理 GM_COMPDATA</b>（避免递归解压），也不校验 RUNGATECODEX；
    /// (4) 循环条件在尾部，<c>nLen &lt; 20</c> 才退出，因此 0 长负载帧（恰好 20 字节）会被消费掉。
    /// </para>
    /// </summary>
    public static RunGateFrameScanResult ScanDecompressedStream(byte[] buf, int len)
    {
        var r = new RunGateFrameScanResult();
        if (buf == null || len <= 0) return r;

        int p = 0;
        int remain = len;
        if (remain < RunGateUtilsConst.SizeOfTM2MsgHeader) return r;

        while (true)
        {
            var hdr = ReadHeader(buf, p);

            if (hdr.dwCode == RunGateUtilsConst.RUNGATECODE)
            {
                int packetLen = Math.Abs(hdr.nLength) + RunGateUtilsConst.SizeOfTM2MsgHeader;
                if (packetLen > remain) break;              // 原 767：Break（丢弃尾部）

                r.Packets.Add(new RunGatePacketSlice(p, packetLen, hdr));
                p += packetLen;
                remain -= packetLen;
            }
            else
            {
                p += 1;                                     // 原 913-914：Inc(P); Dec(nLen)
                remain -= 1;
            }

            if (remain < RunGateUtilsConst.SizeOfTM2MsgHeader) break;   // 原 916
        }

        r.LeftoverOffset = p;
        r.LeftoverLength = 0;                               // 宽松模式不留尾巴（尾部已丢弃）
        return r;
    }

    /// <summary>
    /// RunGateUtils.pas:1367-1378 — <c>DoRecvFullServiceMsg</c>。
    /// 原文判据是 <c>BufferLen &gt; SizeOf(TDefaultMessage)</c>（**严格大于**），
    /// 因此恰好 16 字节的全服消息会被静默丢弃 —— 这是真实边界，勿改成 &gt;=。
    /// </summary>
    public static bool TrySplitFullServiceMsg(byte[] buf, int len, out TDefaultMessage defMsg, out int dataOffset, out int dataLen)
    {
        defMsg = default;
        dataOffset = 0;
        dataLen = 0;
        if (buf == null || len <= RunGateUtilsConst.SizeOfTDefaultMessage) return false;

        defMsg = StructBytes.FromBytes<TDefaultMessage>(buf, 0);
        dataOffset = RunGateUtilsConst.SizeOfTDefaultMessage;
        // 原 1376：BufferLen - SizeOf(TDefaultMessage)。注释说“后面有个 #0 这里干脆不要算了”，
        // 但代码并没有再减 1（消费侧的 OnTimerRunContext 才做 -1，见 RunGateUtilsFullServiceMsgQueue）。
        dataLen = len - RunGateUtilsConst.SizeOfTDefaultMessage;
        return true;
    }

    public static TM2MsgHeader ReadHeader(byte[] buf, int offset) => StructBytes.FromBytes<TM2MsgHeader>(buf, offset);

    /// <summary>把 Leftover 复制成独立数组（原文 SetLength(TempS, Len) + Move）。</summary>
    public static byte[] ExtractLeftover(byte[] buf, in RunGateFrameScanResult r)
    {
        if (r.LeftoverLength <= 0) return Array.Empty<byte>();
        var t = new byte[r.LeftoverLength];
        Array.Copy(buf, r.LeftoverOffset, t, 0, r.LeftoverLength);
        return t;
    }
}
