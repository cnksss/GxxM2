using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GXX.LogDataServer;

/// <summary>
/// uFastFileStream.pas TFastFileStream 1:1 移植（源码 317 行，MMF 快速文件流）。
/// 原文用 CreateFileMapping/MapViewOfFile 做窗口映射；托管侧以"同一窗口算法 + 定位读写"等价实现
/// （缓冲窗口 FBufferPos/FPosInBuffer/FCurBufferSize/nBuffSize 的推进与原文逐行一致），
/// 因此 SetSizeInternal 的扩容策略（FVirtualSize + FBufferSize*4 向上取整到分配粒度）
/// 与析构时的 SetFileSize(FVirtualSize, false) 截断语义都保持不变。
/// </summary>
public sealed class TFastFileStream : Stream
{
    // ---- SysUtils 文件模式（1:1）----
    public const int fmCreate = 0xFFFF;
    public const int fmOpenRead = 0x0000;
    public const int fmOpenWrite = 0x0001;
    public const int fmOpenReadWrite = 0x0002;
    public const int fmShareCompat = 0x0000;
    public const int fmShareExclusive = 0x0010;
    public const int fmShareDenyWrite = 0x0020;
    public const int fmShareDenyRead = 0x0030;
    public const int fmShareDenyNone = 0x0040;

    public const int ERROR_ACCESS_DENIED = 5;

    // ---- 字段（对应 uFastFileStream.pas:52-62）----
    private bool FViewValid;              // 原文 FPointer <> nil
    private FileStream FFile = null!;     // 原文 FFile: THandle
    private bool FMappingValid;           // 原文 FMapping <> INVALID_HANDLE_VALUE
    private long FRealSize;               // Real size of the file
    private long FVirtualSize;            // Current virtual size of the file (<=FRealSize)
    private long FBufferPos;              // Pos of Buffer in file
    private long FBufferSize;             // Size of Buffer wanted
    private long FCurBufferSize;          // current size of Buffer
    private long FAllocationGranularity;  // used for efficient alignment
    private long FPosInBuffer;            // current position in buffer (can be >=F(Cur)BufferSize)
    private bool FReadOnly;
    private string FFileName = "";

    /// <summary>nBuffSize（ReInitView 计算的映射窗口大小）。</summary>
    private long FViewSize;

    /// <summary>对应 Windows SetLastError/GetLastError（只读拒绝等场合）。</summary>
    public int LastError { get; private set; }

    /// <summary>
    /// 分配粒度来源。默认 P/Invoke GetSystemInfo（SysInfo.dwAllocationGranularity，通常 65536）；
    /// 测试可注入以获得确定性。
    /// </summary>
    public static Func<long> AllocationGranularityProvider = DefaultAllocationGranularity;

    private static long DefaultAllocationGranularity()
    {
        try
        {
            GetSystemInfo(out SYSTEM_INFO info);
            if (info.dwAllocationGranularity != 0) return info.dwAllocationGranularity;
        }
        catch
        {
            // 非 Windows 或 P/Invoke 不可用 → 回退到 Windows 常规值
        }
        return 65536;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEM_INFO
    {
        public ushort wProcessorArchitecture;
        public ushort wReserved;
        public uint dwPageSize;
        public IntPtr lpMinimumApplicationAddress;
        public IntPtr lpMaximumApplicationAddress;
        public IntPtr dwActiveProcessorMask;
        public uint dwNumberOfProcessors;
        public uint dwProcessorType;
        public uint dwAllocationGranularity;
        public ushort wProcessorLevel;
        public ushort wProcessorRevision;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

    /// <summary>uFastFileStream.pas:86 构造函数。</summary>
    public TFastFileStream(string AFileName, int Mode)
    {
        if (Mode == fmCreate)
        {
            try
            {
                FFile = new FileStream(AFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                LastError = Marshal.GetLastWin32Error();
                throw new IOException("无法创建文件 " + Path.GetFullPath(AFileName) + ". " + ex.Message, ex);
            }
            FReadOnly = false;
        }
        else
        {
            try
            {
                FFile = new FileStream(AFileName, FileMode.Open, AccessOf(Mode), ShareOf(Mode));
            }
            catch (Exception ex)
            {
                LastError = Marshal.GetLastWin32Error();
                throw new IOException("无法打开文件 " + Path.GetFullPath(AFileName) + ". " + ex.Message, ex);
            }
            FReadOnly = (Mode & 3) == 0;
        }
        FFileName = AFileName;

        FRealSize = FFile.Length;                    // 原文 PCardinal(@FRealSize)^ := GetFileSize(...)
        FMappingValid = FRealSize != 0;              // 原文 FRealSize = 0 → FMapping := INVALID_HANDLE_VALUE

        FAllocationGranularity = AllocationGranularityProvider();
        FBufferPos = 0;
        if (FRealSize >= 224 * FAllocationGranularity)
            FBufferSize = 224 * FAllocationGranularity;
        else
            FBufferSize = 16 * FAllocationGranularity;
        FCurBufferSize = FBufferSize;
        FVirtualSize = FRealSize;
        FPosInBuffer = 0;
        ReInitView();
    }

    /// <summary>uFastFileStream.pas:158 构造函数（Rights 重载，原文直接转调）。</summary>
    public TFastFileStream(string AFileName, int Mode, uint Rights) : this(AFileName, Mode)
    {
    }

    private static FileAccess AccessOf(int Mode) => (Mode & 3) switch
    {
        0 => FileAccess.Read,          // fmOpenRead
        1 => FileAccess.Write,         // fmOpenWrite
        _ => FileAccess.ReadWrite,     // fmOpenReadWrite
    };

    private static FileShare ShareOf(int Mode) => (Mode & 0xF0) switch
    {
        fmShareExclusive => FileShare.None,
        fmShareDenyWrite => FileShare.Read,
        fmShareDenyRead => FileShare.Write,
        fmShareDenyNone => FileShare.ReadWrite,
        _ => FileShare.ReadWrite,      // fmShareCompat / 未指定
    };

    /// <summary>uFastFileStream.pas:131 SetFileSize。</summary>
    private void SetFileSize(long NewSize, bool reMap = true)
    {
        if (FReadOnly && reMap) return;
        if (FViewValid)
        {
            // 原文 FlushViewOfFile(FPointer, 0)（托管侧每次写直达文件，无需 flush）
            FViewValid = false;
        }
        if (FMappingValid)
        {
            FMappingValid = false;
        }
        if (FReadOnly) return;
        if (NewSize < FRealSize)
        {
            FFile.SetLength(NewSize);      // 原文 FileSeek + SetEndOfFile
        }
        FRealSize = NewSize;
        if (reMap)
        {
            FMappingValid = true;
            // 原文 CreateFileMapping(..., FRealSize, ...) 会把文件扩展到 FRealSize
            if (FRealSize > 0 && FFile.Length != FRealSize) FFile.SetLength(FRealSize);
            ReInitView();
        }
    }

    /// <summary>uFastFileStream.pas:171 ReInitView。</summary>
    private void ReInitView()
    {
        if (FViewValid)
        {
            FViewValid = false;
        }
        long nBuffSize;
        if (FVirtualSize < FBufferPos + FBufferSize)
        {
            if (FVirtualSize < FBufferPos) FCurBufferSize = 0;
            else FCurBufferSize = FVirtualSize - FBufferPos;
        }
        else
        {
            FCurBufferSize = FBufferSize;
        }
        if (FRealSize < FBufferPos + FBufferSize)
        {
            if (FRealSize < FBufferPos) nBuffSize = 0;
            else nBuffSize = FRealSize - FBufferPos;
        }
        else
        {
            nBuffSize = FBufferSize;
        }
        if (nBuffSize > 0)
        {
            FViewValid = true;
            FViewSize = nBuffSize;
        }
        else
        {
            FViewSize = 0;
        }
    }

    /// <summary>uFastFileStream.pas:196 Seek。</summary>
    public override long Seek(long Offset, SeekOrigin Origin)
    {
        long newPos;
        switch (Origin)
        {
            case SeekOrigin.Begin: newPos = Offset; break;
            case SeekOrigin.Current: newPos = FBufferPos + FPosInBuffer + Offset; break;
            case SeekOrigin.End: newPos = Size + Offset; break;
            default: newPos = -1; break;
        }
        if (newPos >= 0)
        {
            if (newPos < FBufferPos || newPos >= FBufferPos + FCurBufferSize)
            {
                FBufferPos = newPos - newPos % FAllocationGranularity;
                FPosInBuffer = newPos % FAllocationGranularity;
                ReInitView();
            }
            else
            {
                FPosInBuffer = newPos - FBufferPos;
            }
        }
        return FBufferPos + FPosInBuffer;
    }

    /// <summary>uFastFileStream.pas:216 SetBufferSize（向上取整到分配粒度）。</summary>
    private void SetBufferSize(long Value)
    {
        if (Value < 0) return;
        FBufferSize = (Value / FAllocationGranularity + 1) * FAllocationGranularity;
        ReInitView();
    }

    /// <summary>uFastFileStream.pas:223 GetSize。</summary>
    public override long Length => FVirtualSize;

    /// <summary>Delphi TStream.Size（= GetSize = FVirtualSize），Seek/Read/Write 内部使用。</summary>
    private long Size => FVirtualSize;

    /// <summary>uFastFileStream.pas:228 SetSize。</summary>
    public override void SetLength(long value) => SetSizeInternal(value);

    /// <summary>uFastFileStream.pas:233 SetSizeInternal。</summary>
    private void SetSizeInternal(long NewSize, bool setPosition = true)
    {
        // size changed?
        if (NewSize != FVirtualSize)
        {
            if (FReadOnly)
            {
                LastError = ERROR_ACCESS_DENIED;
                return;
            }
            FVirtualSize = NewSize;
            long newSizeWanted = FVirtualSize + FBufferSize * 4;
            newSizeWanted = (newSizeWanted / FAllocationGranularity + 1) * FAllocationGranularity;
            if (FVirtualSize > FRealSize || newSizeWanted - FBufferSize * 2 > FRealSize)
                SetFileSize(newSizeWanted);
        }
        if (setPosition) Seek(NewSize, SeekOrigin.Begin);
    }

    private void ReadAt(long fileOffset, byte[] buffer, int offset, int count)
    {
        int n = count;
        if (FViewValid)
        {
            long avail = FViewSize - FPosInBuffer;
            if (n > avail) n = (int)avail;
        }
        FFile.Position = fileOffset;
        int read = n > 0 ? FFile.Read(buffer, offset, n) : 0;
        if (read < count)
        {
            // 映射视图语义：视图内的字节恒可读；文件被中途截断时补 0（等价于原文映射页补齐）
            Array.Clear(buffer, offset + read, count - read);
        }
    }

    private void WriteAt(long fileOffset, byte[] buffer, int offset, int count)
    {
        FFile.Position = fileOffset;
        FFile.Write(buffer, offset, count);
    }

    /// <summary>uFastFileStream.pas:252 Read。</summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        int start = offset;
        while (count > 0)
        {
            long iRemain = FCurBufferSize - FPosInBuffer;
            if (iRemain < count)
            {
                if (Position >= Size) break;
                if (iRemain > 0)
                {
                    ReadAt(FBufferPos + FPosInBuffer, buffer, offset, (int)iRemain);
                    offset += (int)iRemain;
                    count -= (int)iRemain;
                }
                Seek(iRemain, SeekOrigin.Current);
            }
            else
            {
                ReadAt(FBufferPos + FPosInBuffer, buffer, offset, count);
                Seek(count, SeekOrigin.Current);
                offset += count;
                count = 0;
            }
        }
        return offset - start;     // 原文 Result := Cardinal(pTarget) - Cardinal(@Buffer)
    }

    /// <summary>uFastFileStream.pas:279 Write。</summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        if (FReadOnly)
        {
            LastError = ERROR_ACCESS_DENIED;
            return;
        }

        // Resize if needed
        long curPos = Position;
        if (curPos + count > Size)
        {
            SetSizeInternal(curPos + count, false);
        }

        int start = offset;
        while (count > 0)
        {
            long iRemain = FCurBufferSize - FPosInBuffer;
            if (iRemain < count)
            {
                if (iRemain > 0)
                {
                    WriteAt(FBufferPos + FPosInBuffer, buffer, offset, (int)iRemain);
                    offset += (int)iRemain;
                    count -= (int)iRemain;
                }
                Seek(iRemain, SeekOrigin.Current);
            }
            else
            {
                WriteAt(FBufferPos + FPosInBuffer, buffer, offset, count);
                Seek(count, SeekOrigin.Current);
                offset += count;
                count = 0;
            }
        }
        _ = start;
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => !FReadOnly;

    /// <summary>Delphi TStream.Position（读=Seek(0, soCurrent)，写=Seek(v, soBeginning)）。</summary>
    public override long Position
    {
        get => Seek(0, SeekOrigin.Current);
        set => Seek(value, SeekOrigin.Begin);
    }

    /// <summary>原文无 Flush；写入每次直达文件，此处为空实现。</summary>
    public override void Flush()
    {
    }

    /// <summary>uFastFileStream.pas:164 析构（SetFileSize(FVirtualSize, false) → 把文件截到虚拟长度）。</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && FFile != null)
        {
            SetFileSize(FVirtualSize, false);
            FFile.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>uFastFileStream.pas:77 property BufferSize。</summary>
    public long BufferSize
    {
        get => FBufferSize;
        set => SetBufferSize(value);
    }

    /// <summary>uFastFileStream.pas:78 property FileName。</summary>
    public string FileName => FFileName;

    /// <summary>uFastFileStream.pas:79 property Handle（THandle）。</summary>
    public IntPtr Handle => FFile.SafeFileHandle.DangerousGetHandle();

    // ---- 测试/审计可见的内部状态（与原文私有字段同源）----
    public long RealSize => FRealSize;
    public long VirtualSize => FVirtualSize;
    public long BufferPosition => FBufferPos;
    public long PositionInBuffer => FPosInBuffer;
    public long CurrentBufferSize => FCurBufferSize;
    public long AllocationGranularity => FAllocationGranularity;
    public bool ReadOnly => FReadOnly;
}
