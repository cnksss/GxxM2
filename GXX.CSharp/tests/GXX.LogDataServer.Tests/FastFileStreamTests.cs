using System;
using System.IO;
using GXX.LogDataServer;
using Xunit;

namespace GXX.LogDataServer.Tests;

/// <summary>
/// uFastFileStream.pas TFastFileStream 1:1 测试。
/// 分配粒度用 provider 注入为 64 以获得确定性（Windows 实际为 65536）。
/// </summary>
public sealed class FastFileStreamTests : IDisposable
{
    private const long G = 64;

    private readonly string _dir;

    public FastFileStreamTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_lane6_ffs_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        TFastFileStream.AllocationGranularityProvider = () => G;
    }

    public void Dispose()
    {
        TFastFileStream.AllocationGranularityProvider = () => 65536;
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private string P(string name) => Path.Combine(_dir, name);

    private string NewFile(string name, byte[] content)
    {
        string p = P(name);
        File.WriteAllBytes(p, content);
        return p;
    }

    private static byte[] Seq(int n, byte start = 0)
    {
        var b = new byte[n];
        for (int i = 0; i < n; i++) b[i] = (byte)((start + i) & 0xFF);
        return b;
    }

    // ------------------------------------------------------------------
    // 构造 / 属性
    // ------------------------------------------------------------------

    [Fact]
    public void Create_SmallFile_DefaultsTo16GranularityBuffer()
    {
        string p = NewFile("a.bin", Seq(100));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);

        Assert.Equal(p, s.FileName);
        Assert.Equal(16 * G, s.BufferSize);
        Assert.Equal(100, s.Length);
        Assert.Equal(100, s.RealSize);
        Assert.Equal(0, s.BufferPosition);
        Assert.Equal(0, s.PositionInBuffer);
        Assert.True(s.ReadOnly);
        Assert.False(s.CanWrite);
        Assert.True(s.CanRead);
        Assert.True(s.CanSeek);
    }

    /// <summary>FRealSize >= 224 * 粒度 时缓冲升级到 224 倍粒度。</summary>
    [Fact]
    public void Create_LargeFile_Uses224GranularityBuffer()
    {
        string p = NewFile("big.bin", Seq((int)(224 * G)));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        Assert.Equal(224 * G, s.BufferSize);

        string p2 = NewFile("small.bin", Seq((int)(224 * G - 1)));
        using var s2 = new TFastFileStream(p2, TFastFileStream.fmOpenRead);
        Assert.Equal(16 * G, s2.BufferSize);
    }

    [Fact]
    public void Create_EmptyFile_BufferIsSetButNoView_ReadReturnsZero()
    {
        string p = NewFile("empty.bin", Array.Empty<byte>());
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);

        Assert.Equal(0, s.Length);
        Assert.Equal(16 * G, s.BufferSize);
        Assert.Equal(0, s.CurrentBufferSize);   // ReInitView: FRealSize(0) - FBufferPos(0) = 0
        var buf = new byte[8];
        Assert.Equal(0, s.Read(buf, 0, 8));
    }

    [Fact]
    public void Create_MissingFile_ThrowsIOException()
    {
        Assert.Throws<IOException>(() => new TFastFileStream(P("nope.bin"), TFastFileStream.fmOpenRead));
    }

    [Fact]
    public void Create_RightsOverload_IsEquivalent()
    {
        string p = NewFile("r.bin", Seq(10));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead, 0);
        Assert.Equal(10, s.Length);
        Assert.True(s.ReadOnly);
    }

    [Fact]
    public void Create_FmCreate_TruncatesAndIsWritable()
    {
        string p = P("created.bin");
        using (var s = new TFastFileStream(p, TFastFileStream.fmCreate))
        {
            Assert.False(s.ReadOnly);
            Assert.True(s.CanWrite);
            Assert.Equal(0, s.Length);
        }
        Assert.True(File.Exists(p));
    }

    [Fact]
    public void Handle_IsValidAndFileNamePreserved()
    {
        string p = NewFile("h.bin", Seq(4));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        Assert.NotEqual(IntPtr.Zero, s.Handle);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    public void Create_ReadOnlyFlagFollowsModeAndThree(int mode, bool readOnly)
    {
        string p = NewFile("ro.bin", Seq(8));
        using var s = new TFastFileStream(p, mode);
        Assert.Equal(readOnly, s.ReadOnly);
        Assert.Equal(!readOnly, s.CanWrite);
    }

    // ------------------------------------------------------------------
    // Read / Seek
    // ------------------------------------------------------------------

    [Fact]
    public void Read_SequentialAcrossBufferBoundaries_ReturnsAllBytes()
    {
        var data = Seq(1000);
        string p = NewFile("seq.bin", data);
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead)
        {
            BufferSize = G,   // 让窗口频繁切换
        };

        var buf = new byte[1000];
        Assert.Equal(1000, s.Read(buf, 0, 1000));
        Assert.Equal(data, buf);
        Assert.Equal(1000, s.Position);
    }

    [Fact]
    public void Read_PastEnd_ReturnsShortCount()
    {
        string p = NewFile("sh.bin", Seq(20));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.Seek(15, SeekOrigin.Begin);

        var buf = new byte[20];
        Assert.Equal(5, s.Read(buf, 0, 20));
        Assert.Equal(15, buf[0]);
        Assert.Equal(19, buf[4]);
    }

    [Fact]
    public void Read_AtExactEnd_ReturnsZero()
    {
        string p = NewFile("e.bin", Seq(20));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.Seek(20, SeekOrigin.Begin);
        var buf = new byte[4];
        Assert.Equal(0, s.Read(buf, 0, 4));
    }

    /// <summary>
    /// 只有新位置落在当前缓冲窗口之外才重新按分配粒度对齐窗口
    /// （注意 BufferSize setter 是 `Succ(Value div 粒度) * 粒度`，故 64 → 128）。
    /// </summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(127, 127)]
    [InlineData(128, 0)]
    [InlineData(200, 8)]
    [InlineData(500, 52)]
    public void Seek_RealignsBufferAtAllocationGranularity(long target, long expectedInBuffer)
    {
        string p = NewFile("s.bin", Seq(1000));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead) { BufferSize = G };
        Assert.Equal(2 * G, s.BufferSize);   // 窗口实际为 128

        long pos = s.Seek(target, SeekOrigin.Begin);

        Assert.Equal(target, pos);
        Assert.Equal(target - expectedInBuffer, s.BufferPosition);
        Assert.Equal(expectedInBuffer, s.PositionInBuffer);
    }

    /// <summary>Delphi 允许把位置移到 Size 之外（Seek 不做范围校验）。</summary>
    [Fact]
    public void Seek_BeyondEnd_IsAllowed()
    {
        string p = NewFile("be.bin", Seq(20));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);

        Assert.Equal(500, s.Seek(500, SeekOrigin.Begin));
        Assert.Equal(500, s.Position);
        Assert.Equal(520, s.Seek(20, SeekOrigin.Current));
        Assert.Equal(30, s.Seek(10, SeekOrigin.End));   // Size(20) + 10
    }

    [Fact]
    public void Seek_WithinCurrentBuffer_OnlyMovesOffset()
    {
        string p = NewFile("inb.bin", Seq(1000));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);

        s.Seek(10, SeekOrigin.Begin);
        long bufferPos = s.BufferPosition;
        s.Seek(50, SeekOrigin.Begin);

        Assert.Equal(bufferPos, s.BufferPosition);   // 未重映射
        Assert.Equal(50, s.PositionInBuffer);
        Assert.Equal(50, s.Position);
    }

    [Fact]
    public void Position_SetterUsesSeekFromBeginning()
    {
        string p = NewFile("pos.bin", Seq(200));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.Position = 123;
        Assert.Equal(123, s.Position);
        // 123 仍在初始窗口 [0, 200) 内 → 不重映射
        Assert.Equal(0, s.BufferPosition);
        Assert.Equal(123, s.PositionInBuffer);
    }

    // ------------------------------------------------------------------
    // BufferSize
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(0, 1 * G)]
    [InlineData(1, 1 * G)]
    [InlineData(G, 2 * G)]
    [InlineData(G + 1, 2 * G)]
    public void BufferSizeSetter_RoundsUpSuccOfDiv(long value, long expected)
    {
        string p = NewFile("bs.bin", Seq(1000));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.BufferSize = value;
        Assert.Equal(expected, s.BufferSize);
    }

    [Fact]
    public void BufferSizeSetter_NegativeIsIgnored()
    {
        string p = NewFile("bsn.bin", Seq(1000));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        long before = s.BufferSize;
        s.BufferSize = -1;
        Assert.Equal(before, s.BufferSize);
    }

    // ------------------------------------------------------------------
    // Write / SetLength
    // ------------------------------------------------------------------

    [Fact]
    public void Write_GrowsVirtualSize_AndFlushesToDiskOnDispose()
    {
        string p = P("w.bin");
        using (var s = new TFastFileStream(p, TFastFileStream.fmCreate))
        {
            var data = Seq(300, 1);
            s.Write(data, 0, data.Length);
            Assert.Equal(300, s.Length);
            // 内部真实文件已被扩容（FVirtualSize + FBufferSize*4 向上取整）
            Assert.True(s.RealSize > 300);
        }
        // 析构时 SetFileSize(FVirtualSize, false) → 落盘长度 = 虚拟长度
        Assert.Equal(300, new FileInfo(p).Length);
        Assert.Equal(Seq(300, 1), File.ReadAllBytes(p));
    }

    [Fact]
    public void Write_AtOffsetBeyondEnd_FillsGapWithZeros()
    {
        string p = P("gap.bin");
        using (var s = new TFastFileStream(p, TFastFileStream.fmCreate))
        {
            s.Seek(10, SeekOrigin.Begin);
            var data = new byte[] { 0xAA, 0xBB };
            s.Write(data, 0, 2);
            Assert.Equal(12, s.Length);
        }
        byte[] read = File.ReadAllBytes(p);
        Assert.Equal(12, read.Length);
        Assert.All(read[..10], b => Assert.Equal(0, b));
        Assert.Equal(0xAA, read[10]);
        Assert.Equal(0xBB, read[11]);
    }

    [Fact]
    public void Write_ReadOnly_IsIgnoredAndSetsLastError()
    {
        string p = NewFile("wro.bin", Seq(10));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.Write(new byte[] { 1, 2, 3 }, 0, 3);
        Assert.Equal(10, s.Length);
        Assert.Equal(TFastFileStream.ERROR_ACCESS_DENIED, s.LastError);
    }

    [Fact]
    public void SetLength_ExtendsVirtualSize_AndPersistsOnDispose()
    {
        string p = P("len.bin");
        using (var s = new TFastFileStream(p, TFastFileStream.fmCreate))
        {
            s.SetLength(500);
            Assert.Equal(500, s.Length);
        }
        Assert.Equal(500, new FileInfo(p).Length);
    }

    [Fact]
    public void SetLength_ShrinksVirtualSize()
    {
        string p = P("shrink.bin");
        using (var s = new TFastFileStream(p, TFastFileStream.fmCreate))
        {
            s.Write(Seq(1000), 0, 1000);
            s.SetLength(100);
            Assert.Equal(100, s.Length);
        }
        Assert.Equal(100, new FileInfo(p).Length);
    }

    [Fact]
    public void SetLength_ReadOnly_IsDeniedAndKeepsSize()
    {
        string p = NewFile("lro.bin", Seq(50));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.SetLength(10);
        Assert.Equal(50, s.Length);
        Assert.Equal(TFastFileStream.ERROR_ACCESS_DENIED, s.LastError);
    }

    /// <summary>SetLength 到相同值不触发任何变化（原文 `if NewSize &lt;&gt; FVirtualSize`）。</summary>
    [Fact]
    public void SetLength_SameValue_IsNoOp_ButStillRepositions()
    {
        string p = NewFile("same.bin", Seq(100));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenReadWrite);
        s.Seek(0, SeekOrigin.Begin);
        s.SetLength(100);
        Assert.Equal(100, s.Position);   // Seek(NewSize, soBeginning)
    }

    // ------------------------------------------------------------------
    // 边界：文件被截断
    // ------------------------------------------------------------------

    /// <summary>
    /// 外部把文件截断后，映射视图内的字节仍可读（托管侧补 0）——
    /// 保证 Read 的"消费 iRemain 字节"调用约定与原文一致。
    /// 只读打开以避免托管 FileStream 的写缓冲回写干扰外部截断。
    /// </summary>
    [Fact]
    public void Read_TruncatedFile_PadsWithZerosToBufferWindow()
    {
        string p = NewFile("trunc.bin", Seq(100, 0x10));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);

        File.WriteAllBytes(p, Seq(100, 0x10)[..10]);   // 外部截断到 10 字节

        var buf = new byte[100];
        int n = s.Read(buf, 0, 100);

        Assert.Equal(100, n);
        Assert.Equal(Seq(10, 0x10), buf[..10]);
        Assert.All(buf[10..], b => Assert.Equal(0, b));
    }

    [Fact]
    public void Flush_IsNoOp()
    {
        string p = NewFile("f.bin", Seq(4));
        using var s = new TFastFileStream(p, TFastFileStream.fmOpenRead);
        s.Flush();
        Assert.Equal(4, s.Length);
    }

    [Fact]
    public void Dispose_TruncatesRealFileToVirtualSize_WhenRealIsLarger()
    {
        string p = NewFile("d.bin", Seq(1000));
        var s = new TFastFileStream(p, TFastFileStream.fmOpenReadWrite);
        s.SetLength(40);
        Assert.True(s.RealSize >= 40);
        s.Dispose();
        Assert.Equal(40, new FileInfo(p).Length);
    }
}
