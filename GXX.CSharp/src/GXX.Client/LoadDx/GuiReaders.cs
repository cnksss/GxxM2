using System;
using System.IO;

namespace GXX.Client.LoadDx;

// =====================================================================================
// 本文件对应两个源单元里各自的 ReadMemory 原语：
//
//   * LoadDxControl.pas:61-83    ReadMemory(var Buffer; Count):Longint
//        —— 全局 MemoryData/MemorySize/MemoryPosition 三元组 + 裸指针 Move。
//   * LoadDxControlEx.pas:74-81  ReadMemory(streamUI:TStream; var Buffer; Count):Longint
//        —— 直接转发 TStream.Read。
//
// 两者语义**不同**（见报告"格式说明"节）：
//   - 内存版：Count<0 或 MemoryPosition<0 → 返回 0；剩余不足时只搬 nRem 字节并返回 nRem。
//   - 流版  ：Count<0 → 返回 0；其余交给 TStream.Read（读到流尾同样返回实际字节数）。
// 共同点：**短读不报错**。调用方若未检查返回值，记录尾部就是"上一次的残留值"。
// 托管侧无法复现栈上残留，统一以 0 填充并在 ReadRecord 处注释（// 原文如此）。
// =====================================================================================

/// <summary>
/// LoadDxControl*.pas 里 <c>ReadMemory</c> 的公共接缝。Count 为请求字节数，
/// 返回值为**实际**读到的字节数（原文语义：短读返回 nRem，绝不抛异常）。
/// </summary>
public interface IGuiReader
{
    /// <summary>对应 ReadMemory(Buffer, Count)：从当前位置读 Count 字节到 buffer[offset..]。</summary>
    int ReadMemory(byte[] buffer, int offset, int count);

    /// <summary>对应 TStream.Position / MemoryPosition（供 while 循环的结束判定）。</summary>
    long Position { get; }

    /// <summary>对应 TStream.Size / MemorySize。</summary>
    long Size { get; }
}

/// <summary>
/// LoadDxControl.pas:52-59 的单元级全局变量 + :61-83 的 ReadMemory。
/// <para>MemoryData/MemorySize/MemoryPosition 三件套原样保留为可读写字段，便于 1:1 对照。</para>
/// </summary>
public sealed class TDxMemoryReader : IGuiReader
{
    /// <summary>原文 MemoryData:Pointer（托管侧为受管缓冲）。</summary>
    public byte[] MemoryData;

    /// <summary>原文 MemorySize:Integer（调用方声明的长度）。</summary>
    public int MemorySize;

    /// <summary>原文 MemoryPosition:Integer（原文就是全局可变状态，故此处可写）。</summary>
    public int MemoryPosition;

    public TDxMemoryReader(byte[] memory, int size)
    {
        MemoryData = memory;
        MemorySize = size;
        MemoryPosition = 0;
    }

    /// <summary>
    /// 原文 LoadDxControl.pas:61-83 逐行。
    /// <para>
    /// 差异（// 原文如此（LoadDxControl.pas:70,74））：原文用裸指针
    /// <c>Move(Pointer(Longint(MemoryData) + MemoryPosition)^, Buffer, n)</c>，只受 MemorySize 约束；
    /// 若 MemorySize 大于真实缓冲长度会 AccessViolation。托管侧把
    /// <c>min(MemorySize, MemoryData.Length)</c> 当硬边界（既不越界也不抛异常）。
    /// </para>
    /// </summary>
    public int ReadMemory(byte[] buffer, int offset, int count)
    {
        if (MemoryPosition >= 0 && count >= 0)
        {
            int hardLimit = Math.Min(MemorySize, MemoryData?.Length ?? 0);
            int nRem = hardLimit - MemoryPosition;
            if (nRem > 0)
            {
                if (nRem >= count)
                {
                    Buffer.BlockCopy(MemoryData, MemoryPosition, buffer, offset, count);
                    MemoryPosition += count;
                    return count;
                }
                Buffer.BlockCopy(MemoryData, MemoryPosition, buffer, offset, nRem);
                MemoryPosition += nRem;
                return nRem;
            }
            return 0;
        }
        return 0;
    }

    public long Position => MemoryPosition;

    public long Size => MemorySize;

    /// <summary>原文 LoadDxControl.pas:1692-1694 的重新定位（LoadControlFromMemory 入口）。</summary>
    public void Reset(byte[] memory, int size)
    {
        MemoryData = memory;
        MemorySize = size;
        MemoryPosition = 0;
    }
}

/// <summary>LoadDxControlEx.pas:74-81 的 ReadMemory(streamUI, ...) 1:1。</summary>
public sealed class TDxStreamReader : IGuiReader
{
    private readonly Stream _stream;

    public TDxStreamReader(Stream stream) => _stream = stream ?? throw new ArgumentNullException(nameof(stream));

    /// <summary>
    /// 原文 LoadDxControlEx.pas:74-81：<c>if Count &gt;= 0 then Result := streamUI.read(Buffer, Count) else Result := 0;</c>
    /// <para>
    /// 原文如此（LoadDxControlEx.pas:74）：负数长度返回 0，其余一律不判错。
    /// 托管侧 Stream.Read 亦不抛（除流已关闭），短读返回实际字节数，与 TStream.Read 语义一致。
    /// </para>
    /// </summary>
    public int ReadMemory(byte[] buffer, int offset, int count)
    {
        if (count >= 0) return _stream.Read(buffer, offset, count);
        return 0;
    }

    public long Position => _stream.Position;

    public long Size => _stream.Length;
}

/// <summary>LoadDxControl*.pas 里对各记录/标量的读取包装（原文是裸 ReadMemory 调用）。</summary>
public static class GuiReaderExtensions
{
    /// <summary>
    /// 对应 <c>ReadMemory(x, SizeOf(TXxx))</c> 后按字段解码。
    /// <para>
    /// 原文如此（LoadDxControl.pas:185 等）：LoadComponent 内的记录读取**不检查返回值**；
    /// 短读时 Delphi 局部记录变量尾部是未初始化的栈残留，托管侧统一以 0 填充后解码。
    /// 返回值为实际读到的字节数，调用方可据此断言异常路径。
    /// </para>
    /// </summary>
    public static int ReadRecord<T>(this IGuiReader reader, int sizeOf, Func<byte[], int, T> decoder, out T value)
    {
        var buf = new byte[sizeOf];
        int n = reader.ReadMemory(buf, 0, sizeOf);
        value = decoder(buf, 0);
        return n;
    }

    /// <summary>对应 <c>ReadMemory(GuiHeader, SizeOf(TGuiHeader)) = SizeOf(TGuiHeader)</c> 这类"整块读"。</summary>
    public static bool ReadExact<T>(this IGuiReader reader, int sizeOf, Func<byte[], int, T> decoder, out T value)
    {
        int n = reader.ReadRecord(sizeOf, decoder, out value);
        return n == sizeOf;
    }

    /// <summary>对应 <c>ReadMemory(Len, SizeOf(Len))</c>（LoadDxControl.pas:1703）。</summary>
    public static bool ReadInt32(this IGuiReader reader, out int value)
    {
        var buf = new byte[4];
        int n = reader.ReadMemory(buf, 0, 4);
        value = GuiCodec.ReadInt32(buf, 0);
        return n == 4;
    }

    /// <summary>
    /// 对应 <c>SetLength(sText, Len); ReadMemory(sText[1], Len);</c>
    /// —— 逐字节 GBK 解码，长度固定为 Len（Delphi 的 AnsiString 就是字节串）。
    /// </summary>
    public static string ReadFixedString(this IGuiReader reader, int len)
    {
        if (len <= 0) return string.Empty;
        var buf = new byte[len];
        // 原文如此（LoadDxControl.pas:1726-1728）：短读不报错；Delphi 的 SetLength 把未填字节留成 #0，
        // 托管侧 new byte[len] 本就是 0 填充，故读多少都按 len 解码，行为一致。返回值未使用。
        reader.ReadMemory(buf, 0, len);
        return GXX.Core.EncodingInit.GBK.GetString(buf);
    }

    /// <summary>
    /// 对应 LoadDxControl.pas:104-114 / LoadDxControlEx.pas:102-112 的 ReadGuiFontName：
    /// 仅当 NameLen&gt;0 时才消费 NameLen 字节。
    /// </summary>
    public static string ReadGuiFontName(this IGuiReader reader, TGuiFont font)
    {
        string result = string.Empty;
        if (font.NameLen > 0)
        {
            int len = font.NameLen;
            var buf = new byte[len];
            reader.ReadMemory(buf, 0, len);
            result = GXX.Core.EncodingInit.GBK.GetString(buf);
        }
        return result;
    }
}
