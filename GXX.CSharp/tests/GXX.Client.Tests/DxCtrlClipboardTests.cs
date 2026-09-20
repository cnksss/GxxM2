using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Client.DxComponent;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// StreamClipbrd.pas（219 行）+ DxControlClpbrd.pas（166 行）的单元测试。
//
// 覆盖对象：
//   * src/GXX.Client/DxComponent/StreamClipbrd.cs（8 个过程 + IDxClipboard/IDxWriter/
//     IDxReader 接缝 + TDxMemoryClipboard 默认后端）
//   * src/GXX.Client/DxComponent/DxControlClpbrd.cs（6 个转发过程）
//
// 全部走注入的假后端，**不触碰 OS 剪贴板 / 不做任何 UI 调用**（无头安全）。
// 所有 `原文 NNN` 行号指 Source\Client-HGE\DxComponent\StreamClipbrd.pas。
// =====================================================================================
[Collection("dxctrl-serial")]
public class DxCtrlClipboardTests
{
    // ===============================================================================
    // 假后端
    // ===============================================================================

    private sealed class FakeClipboard : IDxClipboard
    {
        public readonly List<string> Log = new();
        public readonly Dictionary<long, byte[]> Blocks = new();
        public readonly Dictionary<uint, long> Handles = new();
        public readonly List<uint> FormatOrder = new();
        public readonly Dictionary<uint, string> Names = new();

        public bool FailAlloc;
        public bool FailLock;
        public bool ThrowOnSetAsHandle;

        private long _next = 1;
        private uint _nextFmt = 0xC000;

        public long GlobalAlloc(int size)
        {
            Log.Add("Alloc" + size);
            if (FailAlloc) return 0;
            long h = _next++;
            Blocks[h] = new byte[size];
            return h;
        }

        public byte[] GlobalLock(long handle)
        {
            Log.Add("Lock" + handle);
            if (FailLock) return null;
            return Blocks.TryGetValue(handle, out var b) ? b : null;
        }

        public void GlobalUnlock(long handle) => Log.Add("Unlock" + handle);

        public void GlobalFree(long handle)
        {
            Log.Add("Free" + handle);
            Blocks.Remove(handle);
        }

        public int GlobalSize(long handle) => Blocks.TryGetValue(handle, out var b) ? b.Length : 0;

        public void Open() => Log.Add("Open");
        public void Close() => Log.Add("Close");

        public void Clear()
        {
            Log.Add("Clear");
            Handles.Clear();
            FormatOrder.Clear();
        }

        public void SetAsHandle(uint fmt, long handle)
        {
            Log.Add($"SetAsHandle({fmt},{handle})");
            if (ThrowOnSetAsHandle) throw new InvalidOperationException("boom");
            if (!Handles.ContainsKey(fmt)) FormatOrder.Add(fmt);
            Handles[fmt] = handle;
        }

        public long GetAsHandle(uint fmt) => Handles.TryGetValue(fmt, out long h) ? h : 0;

        public int FormatCount => FormatOrder.Count;

        public uint Formats(int index) => FormatOrder[index];

        public string GetFormatName(uint fmt) => Names.TryGetValue(fmt, out string n) ? n : "";

        public uint RegisterFormat(string name)
        {
            Log.Add("Register" + name);
            if (string.IsNullOrEmpty(name)) return 0;
            foreach (var kv in Names) if (kv.Value == name) return kv.Key;
            uint id = _nextFmt++;
            Names[id] = name;
            return id;
        }

        /// <summary>测试助手：预置一个格式句柄 + 内容。</summary>
        public void Seed(uint fmt, byte[] data)
        {
            long h = _next++;
            Blocks[h] = data;
            if (!Handles.ContainsKey(fmt)) FormatOrder.Add(fmt);
            Handles[fmt] = h;
        }
    }

    private sealed class FakeWriter : IDxWriter
    {
        public readonly List<string> Ops = new();
        public void WriteInteger(int v) => Ops.Add("i:" + v);
        public void WriteString(string v) => Ops.Add("s:" + v);
        public void Write(byte[] buffer, int count) => Ops.Add("b:" + string.Join(",", buffer.Take(count)));
        public void WriteListBegin() => Ops.Add("LB");
        public void WriteListEnd() => Ops.Add("LE");
    }

    private sealed class FakeReader : IDxReader
    {
        public readonly List<string> Log = new();
        public readonly Queue<int> Ints = new();
        public readonly Queue<string> Strings = new();
        public readonly Queue<byte[]> Blobs = new();
        public readonly Queue<bool> EndFlags = new();

        public int ReadInteger() { Log.Add("ri"); return Ints.Dequeue(); }
        public string ReadString() { Log.Add("rs"); return Strings.Dequeue(); }

        public void Read(byte[] buffer, int count)
        {
            Log.Add("r:" + count);
            var blob = Blobs.Count > 0 ? Blobs.Dequeue() : Array.Empty<byte>();
            Array.Copy(blob, buffer, Math.Min(blob.Length, count));
        }

        public void ReadListBegin() => Log.Add("LB");
        public void ReadListEnd() => Log.Add("LE");
        public bool EndOfList
        {
            get { bool v = EndFlags.Count > 0 ? EndFlags.Dequeue() : true; Log.Add("eol:" + v); return v; }
        }
    }

    /// <summary>替换静态接缝并在作用域结束时还原。</summary>
    private sealed class Scope : IDisposable
    {
        public readonly FakeClipboard Clip = new();
        public readonly List<Stream> RequestedStreams = new();
        public FakeWriter Writer;
        public FakeReader Reader;

        public Scope(bool withFactories = false)
        {
            DxClipboardBackend.Current = Clip;
            if (withFactories)
            {
                DxClipboardBackend.CreateWriter = (s, n) => { RequestedStreams.Add(s); Assert.Equal(4096, n); return Writer; };
                DxClipboardBackend.CreateReader = (s, n) => { RequestedStreams.Add(s); Assert.Equal(4096, n); return Reader; };
            }
        }

        public void Dispose() => DxClipboardBackend.Reset();
    }

    private static MemoryStream Ms(params byte[] data) => new(data);

    // ===============================================================================
    // 一、CopyStreamToClipboard（原文 17-51）
    // ===============================================================================

    [Fact]
    public void CopyStreamToClipboard_CopiesWholeStreamAndResetsPosition()
    {
        using var scope = new Scope();
        var s = Ms(1, 2, 3);

        StreamClipbrd.CopyStreamToClipboard(0x1234, s);

        Assert.Equal(new byte[] { 1, 2, 3 }, scope.Clip.Blocks[scope.Clip.Handles[0x1234]]);
        Assert.Equal(0, s.Position);
        // 原文 21-38 的调用顺序
        Assert.Equal(new[] { "Alloc3", "Lock1", "Unlock1", "Open", "SetAsHandle(4660,1)", "Close" },
            scope.Clip.Log.ToArray());
    }

    [Fact]
    public void CopyStreamToClipboard_ReadsFromStartEvenIfPositionAdvanced()
    {
        using var scope = new Scope();
        var s = Ms(1, 2, 3);
        s.Position = 3;                                             // 原文 20 `S.Position := 0`

        StreamClipbrd.CopyStreamToClipboard(0x1234, s);

        Assert.Equal(new byte[] { 1, 2, 3 }, scope.Clip.Blocks[scope.Clip.Handles[0x1234]]);
    }

    [Fact]
    public void CopyStreamToClipboard_EmptyStream_StillSetsHandle()
    {
        using var scope = new Scope();
        var s = Ms();

        StreamClipbrd.CopyStreamToClipboard(7, s);

        Assert.Contains("Alloc0", scope.Clip.Log);
        Assert.Equal(0, scope.Clip.GlobalSize(scope.Clip.Handles[7]));
    }

    [Fact]
    public void CopyStreamToClipboard_AllocFailure_ThrowsOutOfMemory()
    {
        using var scope = new Scope();
        scope.Clip.FailAlloc = true;

        Assert.Throws<OutOfMemoryException>(() => StreamClipbrd.CopyStreamToClipboard(1, Ms(1, 2)));

        Assert.DoesNotContain("Open", scope.Clip.Log);              // 原文 46-47 直接抛
    }

    [Fact]
    public void CopyStreamToClipboard_LockFailure_FreesHandleThenThrows()
    {
        using var scope = new Scope();
        scope.Clip.FailLock = true;

        Assert.Throws<OutOfMemoryException>(() => StreamClipbrd.CopyStreamToClipboard(1, Ms(1, 2)));

        Assert.Contains("Free1", scope.Clip.Log);                   // 原文 42 GlobalFree
        Assert.DoesNotContain("Open", scope.Clip.Log);
    }

    [Fact]
    public void CopyStreamToClipboard_NullStream_Throws()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => StreamClipbrd.CopyStreamToClipboard(1, null));
    }

    // ===============================================================================
    // 二、CopyStreamFromClipboard（原文 53-76）
    // ===============================================================================

    [Fact]
    public void CopyStreamFromClipboard_WritesGlobalSizeAndResetsPosition()
    {
        using var scope = new Scope();
        scope.Clip.Seed(0x20, new byte[] { 9, 8, 7, 6 });
        var s = new MemoryStream();

        StreamClipbrd.CopyStreamFromClipboard(0x20, s);

        Assert.Equal(new byte[] { 9, 8, 7, 6 }, s.ToArray());
        Assert.Equal(0, s.Position);
    }

    [Fact]
    public void CopyStreamFromClipboard_MissingFormat_DoesNothing()
    {
        using var scope = new Scope();
        var s = Ms(1, 2, 3);
        s.Position = 2;

        StreamClipbrd.CopyStreamFromClipboard(0x99, s);             // 原文 57 hMem = 0 → 整段跳过

        Assert.Equal(new byte[] { 1, 2, 3 }, s.ToArray());
        Assert.Equal(2, s.Position);                                // 连 Position 都不动
        Assert.DoesNotContain("Lock0", scope.Clip.Log);
    }

    [Fact]
    public void CopyStreamFromClipboard_LockFailure_ThrowsOriginalMessage()
    {
        using var scope = new Scope();
        scope.Clip.Seed(0x20, new byte[] { 1 });
        scope.Clip.FailLock = true;

        var ex = Assert.Throws<Exception>(() => StreamClipbrd.CopyStreamFromClipboard(0x20, new MemoryStream()));
        Assert.Equal("CopyStreamFromClipboard: could not lock global handle obtained from clipboard!", ex.Message);
    }

    [Fact]
    public void CopyStreamFromClipboard_NullStream_Throws()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => StreamClipbrd.CopyStreamFromClipboard(1, null));
    }

    // ===============================================================================
    // 三、SaveClipboardFormat（原文 78-99）
    // ===============================================================================

    [Fact]
    public void SaveClipboardFormat_WritesFourItemsWhenDataPresent()
    {
        using var scope = new Scope();
        scope.Clip.Seed(0x10, new byte[] { 5, 6 });
        scope.Clip.Names[0x10] = "MyFormat";
        var w = new FakeWriter();

        StreamClipbrd.SaveClipboardFormat(0x10, w);

        Assert.Equal(new[] { "i:16", "s:MyFormat", "i:2", "b:5,6" }, w.Ops.ToArray());   // 原文 91-94
    }

    [Fact]
    public void SaveClipboardFormat_EmptyClipboardData_WritesNothing()
    {
        using var scope = new Scope();
        var w = new FakeWriter();

        StreamClipbrd.SaveClipboardFormat(0x10, w);                 // 无句柄 → ms.Size = 0

        Assert.Empty(w.Ops);                                        // 原文 89 的 `ms.Size > 0` 门控
    }

    [Fact]
    public void SaveClipboardFormat_UnregisteredFormat_WritesEmptyName()
    {
        using var scope = new Scope();
        scope.Clip.Seed(0x10, new byte[] { 1 });
        var w = new FakeWriter();

        StreamClipbrd.SaveClipboardFormat(0x10, w);

        Assert.Equal("s:", w.Ops[1]);                               // 原文 84-85 GetClipboardFormatName 失败 → ''
    }

    [Fact]
    public void SaveClipboardFormat_NullWriter_Throws()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => StreamClipbrd.SaveClipboardFormat(1, null));
    }

    // ===============================================================================
    // 四、LoadClipboardFormat（原文 101-123）
    // ===============================================================================

    [Fact]
    public void LoadClipboardFormat_NoName_CopiesUnderReadFormat()
    {
        using var scope = new Scope();
        var r = new FakeReader();
        r.Ints.Enqueue(0x30);
        r.Strings.Enqueue("");
        r.Ints.Enqueue(3);
        r.Blobs.Enqueue(new byte[] { 4, 5, 6 });

        StreamClipbrd.LoadClipboardFormat(r);

        Assert.Equal(new byte[] { 4, 5, 6 }, scope.Clip.Blocks[scope.Clip.Handles[0x30]]);
        Assert.DoesNotContain(scope.Clip.Log, x => x.StartsWith("Register"));
    }

    [Fact]
    public void LoadClipboardFormat_WithName_RegistersFormatFirst()
    {
        using var scope = new Scope();
        var r = new FakeReader();
        r.Ints.Enqueue(0x30);                                       // 读到的 fmt 被注册结果**覆盖**
        r.Strings.Enqueue("PkgFmt");
        r.Ints.Enqueue(2);
        r.Blobs.Enqueue(new byte[] { 7, 8 });

        StreamClipbrd.LoadClipboardFormat(r);

        Assert.Contains("RegisterPkgFmt", scope.Clip.Log);          // 原文 117
        Assert.False(scope.Clip.Handles.ContainsKey(0x30));
        Assert.Equal(new byte[] { 7, 8 }, scope.Clip.Blocks[scope.Clip.Handles[0xC000]]);
    }

    [Fact]
    public void LoadClipboardFormat_ZeroFormatAndNoName_SkipsCopy()
    {
        using var scope = new Scope();
        var r = new FakeReader();
        r.Ints.Enqueue(0);
        r.Strings.Enqueue("");
        r.Ints.Enqueue(2);
        r.Blobs.Enqueue(new byte[] { 1, 2 });

        StreamClipbrd.LoadClipboardFormat(r);

        Assert.Empty(scope.Clip.Handles);                           // 原文 118 `if fmt <> 0`
    }

    [Fact]
    public void LoadClipboardFormat_ZeroSize_StillCopiesEmptyBlock()
    {
        using var scope = new Scope();
        var r = new FakeReader();
        r.Ints.Enqueue(0x40);
        r.Strings.Enqueue("");
        r.Ints.Enqueue(0);
        r.Blobs.Enqueue(Array.Empty<byte>());

        StreamClipbrd.LoadClipboardFormat(r);

        Assert.Equal(0, scope.Clip.GlobalSize(scope.Clip.Handles[0x40]));
    }

    [Fact]
    public void LoadClipboardFormat_NullReader_Throws()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => StreamClipbrd.LoadClipboardFormat(null));
    }

    // ===============================================================================
    // 五、SaveClipboard（原文 125-145）
    // ===============================================================================

    [Fact]
    public void SaveClipboard_WithoutWriterFactory_ThrowsNotSupported()
    {
        using var scope = new Scope();
        var ex = Assert.Throws<NotSupportedException>(() => StreamClipbrd.SaveClipboard(new MemoryStream()));
        Assert.Contains("TWriter", ex.Message);
    }

    [Fact]
    public void SaveClipboard_WrapsFormatsInListAndOpensClipboard()
    {
        using var scope = new Scope(withFactories: true) { Writer = new FakeWriter() };
        scope.Clip.Seed(0x10, new byte[] { 1 });
        var s = new MemoryStream();

        StreamClipbrd.SaveClipboard(s);

        Assert.Same(s, scope.RequestedStreams[0]);
        Assert.Equal("Open", scope.Clip.Log[0]);
        Assert.Equal("Close", scope.Clip.Log[^1]);
        Assert.Equal("LB", scope.Writer.Ops[0]);                    // 原文 135
        Assert.Equal("LE", scope.Writer.Ops[^1]);                   // 原文 138
        Assert.Contains("i:16", scope.Writer.Ops);
    }

    [Fact]
    public void SaveClipboard_FormatIdAbove65535_IsSilentlyTruncatedToWord()
    {
        // 原文 78 的 `fmt: Word` 形参：`SaveClipboard` 用 Cardinal 实参调用 → Delphi 隐式窄化。
        // 差异断言：0x12345 被截成 0x2345，于是 GetAsHandle(0x2345) 落空 → 该格式的数据被**静默丢弃**。
        using var scope = new Scope(withFactories: true) { Writer = new FakeWriter() };
        scope.Clip.Seed(0x10, new byte[] { 1 });
        scope.Clip.Seed(0x12345, new byte[] { 2 });

        StreamClipbrd.SaveClipboard(new MemoryStream());

        // 只有 0x10 一项被写出；0x12345 那项因截断后取不到句柄而完全消失
        Assert.Equal(new[] { "LB", "i:16", "s:", "i:1", "b:1", "LE" }, scope.Writer.Ops.ToArray());
    }

    [Fact]
    public void SaveClipboard_ClosesClipboardEvenWhenNoFormats()
    {
        using var scope = new Scope(withFactories: true) { Writer = new FakeWriter() };

        StreamClipbrd.SaveClipboard(new MemoryStream());

        Assert.Equal(new[] { "Open", "Close" }, scope.Clip.Log.ToArray());
        Assert.Equal(new[] { "LB", "LE" }, scope.Writer.Ops.ToArray());
    }

    [Fact]
    public void SaveClipboard_NullStream_Throws()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => StreamClipbrd.SaveClipboard(null));
    }

    [Fact]
    public void SaveClipboard_ClosesClipboardWhenWriterThrows()
    {
        using var scope = new Scope(withFactories: true);
        scope.Writer = new FakeWriter();
        scope.Clip.Seed(0x10, new byte[] { 1 });
        var failing = new ThrowingWriter();
        DxClipboardBackend.CreateWriter = (s, n) => failing;

        Assert.Throws<InvalidOperationException>(() => StreamClipbrd.SaveClipboard(new MemoryStream()));
        Assert.Equal("Close", scope.Clip.Log[^1]);                  // 原文 139-140 finally
    }

    private sealed class ThrowingWriter : IDxWriter
    {
        public void WriteInteger(int value) => throw new InvalidOperationException("w");
        public void WriteString(string value) => throw new InvalidOperationException("w");
        public void Write(byte[] buffer, int count) => throw new InvalidOperationException("w");
        public void WriteListBegin() => throw new InvalidOperationException("w");
        public void WriteListEnd() => throw new InvalidOperationException("w");
    }

    // ===============================================================================
    // 六、LoadClipboard（原文 147-167）
    // ===============================================================================

    [Fact]
    public void LoadClipboard_WithoutReaderFactory_ThrowsNotSupported()
    {
        using var scope = new Scope();
        var ex = Assert.Throws<NotSupportedException>(() => StreamClipbrd.LoadClipboard(new MemoryStream()));
        Assert.Contains("TReader", ex.Message);
    }

    [Fact]
    public void LoadClipboard_ClearsFirstThenReadsListUntilEndOfList()
    {
        using var scope = new Scope(withFactories: true) { Reader = new FakeReader() };
        scope.Reader.EndFlags.Enqueue(false);
        scope.Reader.EndFlags.Enqueue(true);
        scope.Reader.Ints.Enqueue(0x50); scope.Reader.Strings.Enqueue(""); scope.Reader.Ints.Enqueue(1);
        scope.Reader.Blobs.Enqueue(new byte[] { 3 });
        var s = new MemoryStream();

        StreamClipbrd.LoadClipboard(s);

        Assert.Same(s, scope.RequestedStreams[0]);
        Assert.Equal("Open", scope.Clip.Log[0]);
        Assert.Equal("Clear", scope.Clip.Log[1]);                   // 原文 156 先清空
        Assert.Equal("Close", scope.Clip.Log[^1]);
        Assert.Equal(new[] { "LB", "eol:False", "ri", "rs", "ri", "r:1", "eol:True", "LE" },
            scope.Reader.Log.ToArray());
        Assert.Equal(new byte[] { 3 }, scope.Clip.Blocks[scope.Clip.Handles[0x50]]);
    }

    [Fact]
    public void LoadClipboard_EmptyList_CopiesNothing()
    {
        using var scope = new Scope(withFactories: true) { Reader = new FakeReader() };
        scope.Reader.EndFlags.Enqueue(true);

        StreamClipbrd.LoadClipboard(new MemoryStream());

        Assert.Empty(scope.Clip.Handles);
        Assert.Equal(new[] { "LB", "eol:True", "LE" }, scope.Reader.Log.ToArray());
    }

    [Fact]
    public void LoadClipboard_ClosesClipboardWhenReaderThrows()
    {
        using var scope = new Scope(withFactories: true) { Reader = new FakeReader() };
        var s = new MemoryStream();
        DxClipboardBackend.CreateReader = (st, n) => throw new InvalidOperationException("r");

        Assert.Throws<InvalidOperationException>(() => StreamClipbrd.LoadClipboard(s));
        // 工厂在 Open 之前抛 → 没有 Open/Close 日志
        Assert.Empty(scope.Clip.Log);
    }

    [Fact]
    public void LoadClipboard_NullStream_Throws()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => StreamClipbrd.LoadClipboard(null));
    }

    // ===============================================================================
    // 七、StreamSaveToClipboard（原文 174-193）
    // ===============================================================================

    [Fact]
    public void StreamSaveToClipboard_UsesCfMyFormatAndSkipsOpenClose()
    {
        using var scope = new Scope();
        var s = Ms(1, 2, 3);

        StreamClipbrd.StreamSaveToClipboard(s);

        Assert.Equal(55555u, StreamClipbrd.CF_MYFORMAT);            // 原文 171
        Assert.Equal(new byte[] { 1, 2, 3 }, scope.Clip.Blocks[scope.Clip.Handles[StreamClipbrd.CF_MYFORMAT]]);
        Assert.DoesNotContain("Open", scope.Clip.Log);              // 原文 174-193 不 Open/Close
        Assert.DoesNotContain("Close", scope.Clip.Log);
    }

    [Fact]
    public void StreamSaveToClipboard_DoesNotResetPosition_DifferentialVsCopyStreamToClipboard()
    {
        // 差异断言：原文 184 直接 `S.Read` 而不先 `S.Position := 0`（对比 20-29 的 CopyStreamToClipboard）
        using var scope = new Scope();
        var s = Ms(1, 2, 3);
        s.Position = 1;

        StreamClipbrd.StreamSaveToClipboard(s);

        Assert.Equal(3, s.Position);                                // 未复位（读到末尾）
        Assert.Equal(new byte[] { 2, 3, 0 },                        // 只有后 2 字节被填入
            scope.Clip.Blocks[scope.Clip.Handles[StreamClipbrd.CF_MYFORMAT]]);
    }

    [Fact]
    public void StreamSaveToClipboard_FreesHandleWhenSetAsHandleThrows()
    {
        using var scope = new Scope();
        scope.Clip.ThrowOnSetAsHandle = true;

        var ex = Assert.Throws<InvalidOperationException>(() => StreamClipbrd.StreamSaveToClipboard(Ms(1)));
        Assert.Equal("boom", ex.Message);
        Assert.Contains("Free1", scope.Clip.Log);                   // 原文 189-191 except → GlobalFree → raise
        Assert.Contains("Unlock1", scope.Clip.Log);
    }

    [Fact]
    public void StreamSaveToClipboard_LockFailure_FreesHandle()
    {
        using var scope = new Scope();
        scope.Clip.FailLock = true;

        Assert.ThrowsAny<Exception>(() => StreamClipbrd.StreamSaveToClipboard(Ms(1, 2)));
        Assert.Contains("Free1", scope.Clip.Log);
    }

    // ===============================================================================
    // 八、StreamLoadFromClipboard（原文 200-217）
    // ===============================================================================

    [Fact]
    public void StreamLoadFromClipboard_WritesGlobalSizeAndResetsPosition()
    {
        using var scope = new Scope();
        scope.Clip.Seed(StreamClipbrd.CF_MYFORMAT, new byte[] { 1, 2, 3, 4 });
        var s = new MemoryStream();

        StreamClipbrd.StreamLoadFromClipboard(s);

        Assert.Equal(new byte[] { 1, 2, 3, 4 }, s.ToArray());
        Assert.Equal(0, s.Position);
    }

    [Fact]
    public void StreamLoadFromClipboard_MissingFormat_DoesNothing()
    {
        using var scope = new Scope();
        var s = Ms(9);
        s.Position = 1;

        StreamClipbrd.StreamLoadFromClipboard(s);

        Assert.Equal(1, s.Position);
        Assert.DoesNotContain(scope.Clip.Log, x => x.StartsWith("Lock"));
    }

    [Fact]
    public void StreamLoadFromClipboard_LockFailure_DoesNotThrow()
    {
        using var scope = new Scope();
        scope.Clip.Seed(StreamClipbrd.CF_MYFORMAT, new byte[] { 1 });
        scope.Clip.FailLock = true;
        var s = new MemoryStream();

        StreamClipbrd.StreamLoadFromClipboard(s);                   // 原文 208 `if bufptr <> nil` → 静默跳过

        Assert.Equal(0, s.Length);
    }

    // ===============================================================================
    // 九、DxControlClpbrd 转发（DxControlClpbrd.pas 14-164）
    // ===============================================================================

    [Fact]
    public void DxControlClpbrd_CopyStreamToClipboard_Forwards()
    {
        using var scope = new Scope();
        DxControlClpbrd.CopyStreamToClipboard(0x11, Ms(1, 2));
        Assert.Equal(new byte[] { 1, 2 }, scope.Clip.Blocks[scope.Clip.Handles[0x11]]);
    }

    [Fact]
    public void DxControlClpbrd_CopyStreamFromClipboard_Forwards()
    {
        using var scope = new Scope();
        scope.Clip.Seed(0x11, new byte[] { 7 });
        var s = new MemoryStream();
        DxControlClpbrd.CopyStreamFromClipboard(0x11, s);
        Assert.Equal(new byte[] { 7 }, s.ToArray());
    }

    [Fact]
    public void DxControlClpbrd_SaveAndLoadClipboardFormat_Forward()
    {
        using var scope = new Scope();
        scope.Clip.Seed(0x12, new byte[] { 5 });
        var w = new FakeWriter();
        DxControlClpbrd.SaveClipboardFormat(0x12, w);
        Assert.Equal(new[] { "i:18", "s:", "i:1", "b:5" }, w.Ops.ToArray());

        var r = new FakeReader();
        r.Ints.Enqueue(0x13); r.Strings.Enqueue(""); r.Ints.Enqueue(2); r.Blobs.Enqueue(new byte[] { 8, 9 });
        DxControlClpbrd.LoadClipboardFormat(r);
        Assert.Equal(new byte[] { 8, 9 }, scope.Clip.Blocks[scope.Clip.Handles[0x13]]);
    }

    [Fact]
    public void DxControlClpbrd_SaveClipboard_Forwards()
    {
        using var scope = new Scope(withFactories: true) { Writer = new FakeWriter() };
        scope.Clip.Seed(0x14, new byte[] { 1 });
        DxControlClpbrd.SaveClipboard(new MemoryStream());
        Assert.Equal(new[] { "LB", "i:20", "s:", "i:1", "b:1", "LE" }, scope.Writer.Ops.ToArray());
    }

    [Fact]
    public void DxControlClpbrd_LoadClipboard_Forwards()
    {
        using var scope = new Scope(withFactories: true) { Reader = new FakeReader() };
        scope.Reader.EndFlags.Enqueue(true);
        DxControlClpbrd.LoadClipboard(new MemoryStream());
        Assert.Equal(new[] { "LB", "eol:True", "LE" }, scope.Reader.Log.ToArray());
    }

    [Fact]
    public void DxControlClpbrd_NullArguments_ThrowSameAsStreamClipbrd()
    {
        using var scope = new Scope();
        Assert.Throws<ArgumentNullException>(() => DxControlClpbrd.CopyStreamToClipboard(1, null));
        Assert.Throws<ArgumentNullException>(() => DxControlClpbrd.CopyStreamFromClipboard(1, null));
        Assert.Throws<ArgumentNullException>(() => DxControlClpbrd.SaveClipboard(null));
        Assert.Throws<ArgumentNullException>(() => DxControlClpbrd.LoadClipboard(null));
    }

    // ===============================================================================
    // 十、TDxMemoryClipboard 默认后端（无 OS/UI 调用）
    // ===============================================================================

    [Fact]
    public void MemoryClipboard_AllocLockUnlockFreeSize()
    {
        var c = new TDxMemoryClipboard();

        long h = c.GlobalAlloc(4);
        Assert.NotEqual(0, h);
        byte[] block = c.GlobalLock(h);
        Assert.Equal(4, block.Length);
        Assert.Equal(4, c.GlobalSize(h));
        c.GlobalUnlock(h);
        c.GlobalFree(h);
        Assert.Equal(0, c.GlobalSize(h));
        Assert.Null(c.GlobalLock(h));
    }

    [Fact]
    public void MemoryClipboard_NegativeAllocReturnsZero()
    {
        Assert.Equal(0, new TDxMemoryClipboard().GlobalAlloc(-1));
    }

    [Fact]
    public void MemoryClipboard_SetGetHandleAndFormatOrder()
    {
        var c = new TDxMemoryClipboard();
        long h1 = c.GlobalAlloc(1);
        long h2 = c.GlobalAlloc(1);

        Assert.Equal(0, c.GetAsHandle(0x10));
        c.SetAsHandle(0x10, h1);
        c.SetAsHandle(0x20, h2);
        c.SetAsHandle(0x10, h2);                                    // 覆盖不改变顺序

        Assert.Equal(h2, c.GetAsHandle(0x10));
        Assert.Equal(2, c.FormatCount);
        Assert.Equal(0x10u, c.Formats(0));
        Assert.Equal(0x20u, c.Formats(1));
    }

    [Fact]
    public void MemoryClipboard_ClearEmptiesFormatsButKeepsBlocks()
    {
        var c = new TDxMemoryClipboard();
        long h = c.GlobalAlloc(2);
        c.SetAsHandle(0x10, h);

        c.Clear();

        Assert.Equal(0, c.FormatCount);
        Assert.Equal(0, c.GetAsHandle(0x10));
        Assert.Equal(2, c.GlobalSize(h));                           // 原文 Clear 不动全局内存
    }

    [Fact]
    public void MemoryClipboard_RegisterFormatIsStableAndNamed()
    {
        var c = new TDxMemoryClipboard();

        uint a = c.RegisterFormat("FmtA");
        uint b = c.RegisterFormat("FmtB");

        Assert.Equal(0xC000u, a);                                   // Delphi 从 0xC000 起
        Assert.Equal(0xC001u, b);
        Assert.Equal(a, c.RegisterFormat("FmtA"));                  // 同名同 id
        Assert.Equal("FmtA", c.GetFormatName(a));
        Assert.Equal("", c.GetFormatName(0x9999));
        Assert.Equal(0u, c.RegisterFormat(null));
        Assert.Equal(0u, c.RegisterFormat(""));
    }

    [Fact]
    public void MemoryClipboard_FormatsOutOfRange_ReturnsZero()
    {
        var c = new TDxMemoryClipboard();
        Assert.Equal(0u, c.Formats(0));
        Assert.Equal(0u, c.Formats(-1));
        Assert.Equal(0u, c.Formats(5));
    }

    [Fact]
    public void BackendDefaults_AreInjectedFreeAndHeadlessSafe()
    {
        DxClipboardBackend.Reset();

        Assert.IsType<TDxMemoryClipboard>(DxClipboardBackend.Current);
        Assert.Null(DxClipboardBackend.CreateWriter);               // Classes.TWriter 未移植
        Assert.Null(DxClipboardBackend.CreateReader);
        Assert.Equal(4096, DxClipboardBackend.StreamBufferSize);    // 原文 131/152
    }

    // ===============================================================================
    // 十一、端到端：写入 → 读回（进程内往返）
    // ===============================================================================

    [Fact]
    public void RoundTrip_StreamSaveThenLoadFromClipboard()
    {
        DxClipboardBackend.Reset();
        using var s1 = Ms(10, 20, 30, 40);
        StreamClipbrd.StreamSaveToClipboard(s1);

        using var s2 = new MemoryStream();
        StreamClipbrd.StreamLoadFromClipboard(s2);

        Assert.Equal(new byte[] { 10, 20, 30, 40 }, s2.ToArray());
    }

    [Fact]
    public void RoundTrip_SaveClipboardThenLoadClipboard_WithPseudoWriterReader()
    {
        // 用一对进程内的"伪 TWriter/TReader"（内存 list-编码）验证 Save/Load 的往返
        DxClipboardBackend.Reset();
        var sink = new PseudoStore();
        DxClipboardBackend.CreateWriter = (s, n) => new PseudoWriter(sink);
        DxClipboardBackend.CreateReader = (s, n) => new PseudoReader(sink);

        using (var src = Ms(1, 2, 3))
            StreamClipbrd.CopyStreamToClipboard(0xD0, src);
        StreamClipbrd.SaveClipboard(new MemoryStream());

        DxClipboardBackend.Current = new TDxMemoryClipboard();       // 换成空剪贴板
        StreamClipbrd.LoadClipboard(new MemoryStream());

        using var back = new MemoryStream();
        StreamClipbrd.CopyStreamFromClipboard(0xD0, back);
        Assert.Equal(new byte[] { 1, 2, 3 }, back.ToArray());
    }

    /// <summary>Save/Load 往返用的最小 list 容器（模拟 TWriter/TReader 的流）。</summary>
    private sealed class PseudoStore
    {
        public readonly List<(int Fmt, string Name, byte[] Data)> Entries = new();
    }

    private sealed class PseudoWriter : IDxWriter
    {
        private readonly PseudoStore _store;
        private readonly List<object> _pending = new();

        public PseudoWriter(PseudoStore store) => _store = store;

        public void WriteListBegin() => _pending.Clear();
        public void WriteListEnd() { }

        public void WriteInteger(int value) => _pending.Add(value);
        public void WriteString(string value) => _pending.Add(value ?? "");

        public void Write(byte[] buffer, int count)
        {
            _pending.Add(buffer.Take(count).ToArray());
            _store.Entries.Add(((int)_pending[0], (string)_pending[1], (byte[])_pending[3]));
            _pending.Clear();
        }
    }

    private sealed class PseudoReader : IDxReader
    {
        private readonly PseudoStore _store;
        private int _index = -1;
        private bool _consumed = true;

        public PseudoReader(PseudoStore store) => _store = store;

        public void ReadListBegin() { _index = -1; _consumed = true; }
        public void ReadListEnd() { }
        public bool EndOfList => _consumed && _index + 1 >= _store.Entries.Count;

        /// <summary>第一次调用返回条目格式，之后返回数据长度（对应原文 109/111 两次 ReadInteger）。</summary>
        public int ReadInteger()
        {
            if (_consumed)
            {
                _index++;
                _consumed = false;
                return _store.Entries[_index].Fmt;
            }
            _consumed = true;
            return _store.Entries[_index].Data.Length;
        }

        public string ReadString() => _store.Entries[_index].Name;

        public void Read(byte[] buffer, int count)
        {
            var data = _store.Entries[_index].Data;
            Array.Copy(data, buffer, Math.Min(data.Length, count));
        }
    }
}
