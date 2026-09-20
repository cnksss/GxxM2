using System;
using System.Collections.Generic;
using System.IO;

namespace GXX.Client.DxComponent;

// =====================================================================================
// StreamClipbrd.pas（219 行，源：Source\Client-HGE\DxComponent\StreamClipbrd.pas）1:1 移植。
//
// 源单元行号范围（本文件逐条对应）：
//   * 1-5       unit 头 + interface uses（SysUtils, Classes, Windows, Clipbrd）
//   * 6-14      interface 段 8 个过程声明（6-11 的 6 个与 DxControlClpbrd.pas 逐字相同）
//   * 17-51     CopyStreamToClipboard
//   * 53-76     CopyStreamFromClipboard
//   * 78-99     SaveClipboardFormat
//   * 101-123   LoadClipboardFormat
//   * 125-145   SaveClipboard
//   * 147-167   LoadClipboard
//   * 170-171   const CF_MYFORMAT = 55555
//   * 174-193   StreamSaveToClipboard
//   * 200-217   StreamLoadFromClipboard
//
// 兄弟单元 DxControlClpbrd.pas 见 DxControlClpbrd.cs（**未被子系统引用的前缀重复**，
// 实现段 `StreamClipbrd.pas 15-167` ≡ `DxControlClpbrd.pas 12-164`，153 行逐字相同）。
//
// DFM: 无（纯过程单元）。
//
// -------------------------------------------------------------------------------------
// 托管侧接缝与偏差（逐条登记）：
//
//   1. **`Clipboard` / `GlobalAlloc` 家族整体接缝化**（原文 Windows + Clipbrd 单元）：
//      `IDxClipboard` 逐方法对应 `GlobalAlloc` / `GlobalLock` / `GlobalUnlock` /
//      `GlobalFree` / `GlobalSize` / `Clipboard.Open|Close|Clear|SetAsHandle|GetAsHandle|
//      FormatCount|Formats[]` / `GetClipboardFormatName` / `RegisterClipboardFormat`。
//      默认后端 `TDxMemoryClipboard` 是**进程内**等价物（Dictionary<uint,long> 承载
//      格式 → 句柄），**不触碰任何 OS/UI 资源** —— 无头 testhost 安全（硬性要求 §6）。
//      **偏差**：跨进程/跨应用剪贴板互操作需要宿主把 `DxClipboardBackend.Current`
//      替换为 Win32 实现（RegisterClipboardFormat/SetClipboardData/GetClipboardData）。
//      这是本单元最大的偏差，已在上方登记。
//
//   2. `GlobalLock` 在托管侧返回 `byte[]`（锁定块的可写视图），`GlobalUnlock` 之前
//      对它的写入即"写入被锁定的全局内存"—— 对应原文 `S.Read(pMem^, S.Size)` /
//      `S.Write(pMem^, GlobalSize(hMem))`。返回 null 表示原文的 `pMem = nil` 失败分支。
//
//   3. `TWriter` / `TReader`（Delphi Classes RTL）**尚未移植** → 以 `IDxWriter` / `IDxReader`
//      最小面接缝（原文用到的 5 + 5 个方法）。默认**未注入**：此时 `SaveClipboard` /
//      `LoadClipboard` 给出明确的 NotSupportedException，而不是静默失败。
//      工厂签名对应原文 `TWriter.Create(S, 4096)` / `TReader.Create(S, 4096)`
//      （第二形参 4096 是缓冲区大小，由实现方解释）。
//
//   4. `Assert(Assigned(S))`（原文 19 / 55 / 83 / 108 / 130 / 151）在 Delphi 里
//      仅 Debug 构建生效；托管侧统一抛 `ArgumentNullException` 并注释 `// 原文 Assert`。
//
//   5. `OutOfMemoryError`（SysUtils）→ `throw new OutOfMemoryException()`（原文 43/47）。
//
//   6. `S.Size` / `S.Position` / `S.Read` / `S.Write` → `Stream.Length` / `Position` /
//      `Read(byte[],int,int)` / `Write(byte[],int,int)`；均要求可寻址流（原文同）。
//
//   7. 原文 **`fmt: Word` 形参的静默窄化**（78 行）：`SaveClipboard` 用
//      `Clipboard.Formats[i]`（Cardinal）实参调用 `SaveClipboardFormat(fmt: Word, ...)`
//      —— Delphi 隐式窄化为 16 位，**格式 id ≥ 65536 时高位丢失**。托管侧用
//      `ushort` 形参 + 显式 `(ushort)` 转换逐字保留该行为（见 SaveClipboard 注释）。
//
//   8. 原文 `writer.WriteString(fmtname)`（92 行）的实参是 `array[0..128] of Char`
//      （NUL 结尾的格式名缓冲）而非 `string`；托管侧以 `GetFormatName` 返回的
//      托管字符串承接（已是到首个 NUL 为止的名字）。
//
//   9. 原文 174-193 `StreamSaveToClipboard` **不做** `Clipboard.Open/Close`、
//      也**不做** `S.Position := 0`（与 17-51 的 `CopyStreamToClipboard` 不同）；
//      异常时 `GlobalFree(hbuf)` 后 `raise`。以上均逐字保留。
//
//  10. 原文 210 用 `WriteBuffer`（写不满即抛），而 63 行用 `Write`（不校验）。
//      这是同一单元内的真实差异，托管侧均用 `Stream.Write`，由流的实现保证
//      `WriteBuffer` 的"全写或抛"语义（已注释在调用点）。
// =====================================================================================

// -------------------------------------------------------------------------------------
// 接缝：Windows 全局内存 + 系统剪贴板（原文 uses Windows, Clipbrd）
// -------------------------------------------------------------------------------------

/// <summary>
/// StreamClipbrd.pas 用到的 `GlobalAlloc`/`GlobalLock`/`Clipboard.*` 最小面。
/// 每个方法都逐条对应原文的一次调用。
/// </summary>
public interface IDxClipboard
{
    /// <summary>`GlobalAlloc(GHND or GMEM_DDESHARE, Size)`；返回 0 表示失败（原文 21/179）。</summary>
    long GlobalAlloc(int size);

    /// <summary>`GlobalLock(hMem)`；返回 null 表示失败（原文 24/59/180/207）。</summary>
    byte[] GlobalLock(long handle);

    /// <summary>`GlobalUnlock(hMem)`（原文 31/66/187/213）。</summary>
    void GlobalUnlock(long handle);

    /// <summary>`GlobalFree(hMem)`（原文 42/190）。</summary>
    void GlobalFree(long handle);

    /// <summary>`GlobalSize(hMem)`（原文 63/210）。</summary>
    int GlobalSize(long handle);

    /// <summary>`Clipboard.Open`（原文 33/133/154）。</summary>
    void Open();

    /// <summary>`Clipboard.Close`（原文 37/140/162）。</summary>
    void Close();

    /// <summary>`Clipboard.Clear`（原文 156）。</summary>
    void Clear();

    /// <summary>`Clipboard.SetAsHandle(fmt, hMem)`（原文 35/185）。</summary>
    void SetAsHandle(uint fmt, long handle);

    /// <summary>`Clipboard.GetAsHandle(fmt)`；返回 0 表示该格式不存在（原文 56/205）。</summary>
    long GetAsHandle(uint fmt);

    /// <summary>`Clipboard.FormatCount`（原文 136）。</summary>
    int FormatCount { get; }

    /// <summary>`Clipboard.Formats[i]`（原文 137）。</summary>
    uint Formats(int index);

    /// <summary>`GetClipboardFormatName(fmt, buf, size)`；返回 "" 表示失败（原文 84）。</summary>
    string GetFormatName(uint fmt);

    /// <summary>`RegisterClipboardFormat(PChar(name))`（原文 117）。</summary>
    uint RegisterFormat(string name);
}

/// <summary>
/// 默认剪贴板后端：**进程内**等价物（无 OS/UI 调用，无头 testhost 安全）。
/// 跨进程互操作需宿主替换 <see cref="DxClipboardBackend.Current"/>（见文件头第 1 条）。
/// </summary>
public sealed class TDxMemoryClipboard : IDxClipboard
{
    private sealed class Block
    {
        public byte[] Data;
        public bool Locked;
    }

    private readonly Dictionary<long, Block> _blocks = new();
    private readonly Dictionary<uint, long> _clipboard = new();
    private readonly List<uint> _formatOrder = new();
    private readonly Dictionary<string, uint> _registered = new(StringComparer.Ordinal);
    private readonly Dictionary<uint, string> _names = new();
    private long _nextHandle = 1;
    private uint _nextFormat = 0xC000;      // Delphi RegisterClipboardFormat 从 0xC000 起分配

    public long GlobalAlloc(int size)
    {
        if (size < 0) return 0;
        long handle = _nextHandle++;
        _blocks[handle] = new Block { Data = new byte[size] };
        return handle;
    }

    public byte[] GlobalLock(long handle)
    {
        if (!_blocks.TryGetValue(handle, out var block)) return null;
        block.Locked = true;
        return block.Data;
    }

    public void GlobalUnlock(long handle)
    {
        if (_blocks.TryGetValue(handle, out var block)) block.Locked = false;
    }

    public void GlobalFree(long handle) => _blocks.Remove(handle);

    public int GlobalSize(long handle) => _blocks.TryGetValue(handle, out var block) ? block.Data.Length : 0;

    public void Open() { }

    public void Close() { }

    public void Clear()
    {
        _clipboard.Clear();
        _formatOrder.Clear();
    }

    public void SetAsHandle(uint fmt, long handle)
    {
        if (!_clipboard.ContainsKey(fmt)) _formatOrder.Add(fmt);
        _clipboard[fmt] = handle;
    }

    public long GetAsHandle(uint fmt) => _clipboard.TryGetValue(fmt, out long handle) ? handle : 0;

    public int FormatCount => _formatOrder.Count;

    public uint Formats(int index) => index >= 0 && index < _formatOrder.Count ? _formatOrder[index] : 0u;

    /// <summary>已注册格式名；未注册（含标准剪贴板格式）返回 ""，对应原文 `GetClipboardFormatName` 失败。</summary>
    public string GetFormatName(uint fmt) => _names.TryGetValue(fmt, out string name) ? name : "";

    public uint RegisterFormat(string name)
    {
        if (string.IsNullOrEmpty(name)) return 0;
        if (_registered.TryGetValue(name, out uint existing)) return existing;
        uint id = _nextFormat++;
        _registered[name] = id;
        _names[id] = name;
        return id;
    }
}

/// <summary>接缝：原文 `Classes.TWriter`（本单元用到的 5 个方法）。</summary>
public interface IDxWriter
{
    /// <summary>`TWriter.WriteInteger(Value: Integer)`（原文 91/93）。</summary>
    void WriteInteger(int value);

    /// <summary>`TWriter.WriteString(const Value: string)`（原文 92）。</summary>
    void WriteString(string value);

    /// <summary>`TWriter.Write(Buffer, Count)`（原文 94：`writer.Write(ms.Memory^, ms.Size)`）。</summary>
    void Write(byte[] buffer, int count);

    /// <summary>`TWriter.WriteListBegin`（原文 135）。</summary>
    void WriteListBegin();

    /// <summary>`TWriter.WriteListEnd`（原文 138）。</summary>
    void WriteListEnd();
}

/// <summary>接缝：原文 `Classes.TReader`（本单元用到的 6 个成员）。</summary>
public interface IDxReader
{
    /// <summary>`TReader.ReadInteger`（原文 109）。</summary>
    int ReadInteger();

    /// <summary>`TReader.ReadString`（原文 110）。</summary>
    string ReadString();

    /// <summary>`TReader.Read(Buffer, Count)`（原文 115）。</summary>
    void Read(byte[] buffer, int count);

    /// <summary>`TReader.ReadListBegin`（原文 157）。</summary>
    void ReadListBegin();

    /// <summary>`TReader.ReadListEnd`（原文 160）。</summary>
    void ReadListEnd();

    /// <summary>`TReader.EndOfList`（原文 158）。</summary>
    bool EndOfList { get; }
}

/// <summary>
/// StreamClipbrd.pas 的四类外部依赖的托管落点：剪贴板后端 + `TWriter`/`TReader` 工厂。
/// 全部可注入；默认后端是进程内实现（无头安全），默认工厂为 null（未移植 RTL）。
/// </summary>
public static class DxClipboardBackend
{
    /// <summary>剪贴板 + 全局内存后端（原文 Windows/Clipbrd 单元）。</summary>
    public static IDxClipboard Current = new TDxMemoryClipboard();

    /// <summary>
    /// 原文 `TWriter.Create(S, 4096)` 的工厂（第二形参是缓冲区大小）。
    /// 默认 null —— `Classes.TWriter` 尚未移植；`SaveClipboard` 会给出明确异常。
    /// </summary>
    public static Func<Stream, int, IDxWriter> CreateWriter;

    /// <summary>原文 `TReader.Create(S, 4096)` 的工厂。默认 null，理由同上。</summary>
    public static Func<Stream, int, IDxReader> CreateReader;

    /// <summary>原文 `TWriter.Create` / `TReader.Create` 的 4096 缓冲区常量（原文 131/152）。</summary>
    public const int StreamBufferSize = 4096;

    /// <summary>恢复默认后端（测试夹具用）。</summary>
    public static void Reset()
    {
        Current = new TDxMemoryClipboard();
        CreateWriter = null;
        CreateReader = null;
    }
}

// -------------------------------------------------------------------------------------
// StreamClipbrd.pas 17-217：8 个单元级过程
// -------------------------------------------------------------------------------------

/// <summary>StreamClipbrd.pas（219 行）的 8 个单元级过程 1:1 移植。</summary>
public static class StreamClipbrd
{
    /// <summary>原文 171 `CF_MYFORMAT = 55555;`。</summary>
    public const uint CF_MYFORMAT = 55555;

    private static IDxClipboard Clipboard => DxClipboardBackend.Current;

    /// <summary>
    /// 原文 17-51 <c>CopyStreamToClipboard(fmt: Cardinal; S: TStream)</c>：
    /// 把整条流拷进一张新分配的全局内存块，并把该句柄以 `fmt` 放到剪贴板上。
    /// 分配失败 / 锁定失败都 `OutOfMemoryError`；锁定失败时先 `GlobalFree` 再抛。
    /// 成功路径会把 `S.Position` 复位两次（读之前与读之后）。
    /// </summary>
    public static void CopyStreamToClipboard(uint fmt, Stream s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));   // 原文 19 Assert(Assigned(S))
        s.Position = 0;

        long hMem = Clipboard.GlobalAlloc((int)s.Length);
        if (hMem != 0)
        {
            byte[] pMem = Clipboard.GlobalLock(hMem);
            if (pMem != null)
            {
                try
                {
                    s.Read(pMem, 0, (int)s.Length);
                    s.Position = 0;
                }
                finally
                {
                    Clipboard.GlobalUnlock(hMem);
                }

                Clipboard.Open();
                try
                {
                    Clipboard.SetAsHandle(fmt, hMem);
                }
                finally
                {
                    Clipboard.Close();
                }
            }
            else
            {
                Clipboard.GlobalFree(hMem);
                throw new OutOfMemoryException();                     // 原文 43 OutOfMemoryError
            }
        }
        else
        {
            throw new OutOfMemoryException();                         // 原文 47 OutOfMemoryError
        }
    }

    /// <summary>
    /// 原文 53-76 <c>CopyStreamFromClipboard(fmt: Cardinal; S: TStream)</c>：
    /// 从剪贴板取 `fmt` 的句柄，**按 `GlobalSize`（不是流原长）**写入 `S`，最后 `S.Position := 0`。
    /// 句柄不存在（0）时**什么都不做**（连 Position 都不动）。
    /// 锁定失败抛原文的英文消息（原文 73-74）。
    /// </summary>
    public static void CopyStreamFromClipboard(uint fmt, Stream s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));   // 原文 55 Assert(Assigned(S))
        long hMem = Clipboard.GetAsHandle(fmt);
        if (hMem != 0)
        {
            byte[] pMem = Clipboard.GlobalLock(hMem);
            if (pMem != null)
            {
                try
                {
                    // 原文 63 是 `S.Write`（不校验写入量），与 210 行的 `WriteBuffer` 不同
                    s.Write(pMem, 0, Clipboard.GlobalSize(hMem));
                    s.Position = 0;
                }
                finally
                {
                    Clipboard.GlobalUnlock(hMem);
                }
            }
            else
            {
                throw new Exception("CopyStreamFromClipboard: could not lock global handle " +
                    "obtained from clipboard!");                      // 原文 73-74 逐字
            }
        }
    }

    /// <summary>
    /// 原文 78-99 <c>SaveClipboardFormat(fmt: Word; writer: TWriter)</c>：
    /// 取 `fmt` 的格式名（失败则空串），把剪贴板内容读进内存流；
    /// **仅当 `ms.Size &gt; 0`** 时写 4 项：`Integer(fmt)` / `String(fmtname)` /
    /// `Integer(ms.Size)` / `Write(ms.Memory^, ms.Size)`。
    /// 注意形参是 **Word**（见文件头第 7 条的静默窄化）。
    /// </summary>
    public static void SaveClipboardFormat(ushort fmt, IDxWriter writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));  // 原文 83 Assert
        string fmtname = Clipboard.GetFormatName(fmt) ?? "";                  // 原文 84-85
        using var ms = new MemoryStream();
        CopyStreamFromClipboard(fmt, ms);
        if (ms.Length > 0)                                                    // 原文 89
        {
            writer.WriteInteger(fmt);                                         // 原文 91
            writer.WriteString(fmtname);                                      // 原文 92
            writer.WriteInteger((int)ms.Length);                              // 原文 93
            writer.Write(ms.ToArray(), (int)ms.Length);                       // 原文 94
        }
    }

    /// <summary>
    /// 原文 101-123 <c>LoadClipboardFormat(reader: TReader)</c>：
    /// 读 `Integer(fmt)` / `String(fmtname)` / `Integer(Size)` / `Size` 字节；
    /// **`fmtname` 非空时用 `RegisterCLipboardFormat` 覆盖 fmt**（原文 117 拼写 CLipboard）；
    /// `fmt &lt;&gt; 0` 才写回剪贴板。`fmt` 是 **Integer**（后续调用处再隐式放宽为 Cardinal）。
    /// </summary>
    public static void LoadClipboardFormat(IDxReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));   // 原文 108 Assert
        int fmt = reader.ReadInteger();
        string fmtname = reader.ReadString();
        int size = reader.ReadInteger();

        // 原文 112-115：`ms := TMemoryStream.Create; ms.Size := Size; reader.Read(ms.memory^, Size)`
        // 托管侧 `MemoryStream(capacity)` 的缓冲区是公开可取的（`GetBuffer()` 可用）。
        using var ms = new MemoryStream(size < 0 ? 0 : size);
        if (size > 0) ms.SetLength(size);
        reader.Read(ms.GetBuffer(), size);                                   // 原文 115
        if (!string.IsNullOrEmpty(fmtname))                                   // 原文 116 Length(fmtname) > 0
            fmt = unchecked((int)Clipboard.RegisterFormat(fmtname));          // 原文 117
        if (fmt != 0)                                                         // 原文 118
            CopyStreamToClipboard((uint)fmt, ms);                             // 原文 119
    }

    /// <summary>
    /// 原文 125-145 <c>SaveClipboard(S: TStream)</c>：
    /// `TWriter.Create(S, 4096)` → `Clipboard.Open` → `WriteListBegin` →
    /// 对**每一个**剪贴板格式调 `SaveClipboardFormat(Formats[i], writer)` → `WriteListEnd`
    /// → `Clipboard.Close` → `writer.Free`。
    /// `Formats[i]` 是 Cardinal，传入 `Word` 形参时静默截断（文件头第 7 条）。
    /// </summary>
    public static void SaveClipboard(Stream s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));   // 原文 130 Assert(Assigned(S))
        IDxWriter writer = DxClipboardBackend.CreateWriter != null
            ? DxClipboardBackend.CreateWriter(s, DxClipboardBackend.StreamBufferSize)
            : throw new NotSupportedException(
                "StreamClipbrd.SaveClipboard 需要 TWriter：请注入 DxClipboardBackend.CreateWriter（Classes.TWriter 尚未移植）。");
        // 原文 143 `writer.Free`：托管侧 IDxWriter 由工厂创建并持有，无显式释放点。
        Clipboard.Open();
        try
        {
            writer.WriteListBegin();                             // 原文 135
            for (int i = 0; i < Clipboard.FormatCount; i++)       // 原文 136-137
                SaveClipboardFormat(unchecked((ushort)Clipboard.Formats(i)), writer);
            writer.WriteListEnd();                               // 原文 138
        }
        finally
        {
            Clipboard.Close();
        }
    }

    /// <summary>
    /// 原文 147-167 <c>LoadClipboard(S: TStream)</c>：
    /// `TReader.Create(S, 4096)` → `Clipboard.Open` → **`clipboard.Clear`** →
    /// `ReadListBegin` → `while not EndOfList do LoadClipboardFormat(reader)` →
    /// `ReadListEnd` → `Clipboard.Close` → `reader.Free`。
    /// 注意原文 156 的 `clipboard.Clear` 是**先清空再导入**。
    /// </summary>
    public static void LoadClipboard(Stream s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));   // 原文 151 Assert(Assigned(S))
        IDxReader reader = DxClipboardBackend.CreateReader != null
            ? DxClipboardBackend.CreateReader(s, DxClipboardBackend.StreamBufferSize)
            : throw new NotSupportedException(
                "StreamClipbrd.LoadClipboard 需要 TReader：请注入 DxClipboardBackend.CreateReader（Classes.TReader 尚未移植）。");
        // 原文 165 `reader.Free`：托管侧 IDxReader 由工厂创建并持有，无显式释放点。
        Clipboard.Open();
        try
        {
            Clipboard.Clear();                                   // 原文 156
            reader.ReadListBegin();                              // 原文 157
            while (!reader.EndOfList)                            // 原文 158
                LoadClipboardFormat(reader);
            reader.ReadListEnd();                                // 原文 160
        }
        finally
        {
            Clipboard.Close();
        }
    }

    /// <summary>
    /// 原文 174-193 <c>StreamSaveToClipboard(S: TStream)</c>：
    /// `GlobalAlloc(GMEM_MOVEABLE, S.Size)` → 锁定 → `S.Read` → `SetAsHandle(CF_MYFORMAT, hbuf)`；
    /// **不 `Open`/`Close`**、**不 `S.Position := 0`**（与 <see cref="CopyStreamToClipboard"/> 不同）；
    /// 任何异常都先 `GlobalFree(hbuf)` 再 `raise`。原文不检查 alloc/lock 失败。
    /// </summary>
    public static void StreamSaveToClipboard(Stream s)
    {
        long hbuf = Clipboard.GlobalAlloc((int)s.Length);
        try
        {
            byte[] bufptr = Clipboard.GlobalLock(hbuf);
            try
            {
                s.Read(bufptr, 0, (int)s.Length);                    // 原文 184
                Clipboard.SetAsHandle(CF_MYFORMAT, hbuf);            // 原文 185
            }
            finally
            {
                Clipboard.GlobalUnlock(hbuf);
            }
        }
        catch
        {
            Clipboard.GlobalFree(hbuf);                              // 原文 190
            throw;
        }
    }

    /// <summary>
    /// 原文 200-217 <c>StreamLoadFromClipboard(S: TStream)</c>：
    /// 取 `CF_MYFORMAT` 句柄；存在且能锁定时按 `GlobalSize` **`WriteBuffer`**（写不满即抛）
    /// 写入 `S`，最后 `S.Position := 0`。两者任一失败都不抛、什么都不做。
    /// </summary>
    public static void StreamLoadFromClipboard(Stream s)
    {
        long hbuf = Clipboard.GetAsHandle(CF_MYFORMAT);
        if (hbuf != 0)
        {
            byte[] bufptr = Clipboard.GlobalLock(hbuf);
            if (bufptr != null)
            {
                try
                {
                    // 原文 210 是 `S.WriteBuffer`（与 63 行的 `S.Write` 不同）
                    s.Write(bufptr, 0, Clipboard.GlobalSize(hbuf));
                    s.Position = 0;
                }
                finally
                {
                    Clipboard.GlobalUnlock(hbuf);
                }
            }
        }
    }
}
