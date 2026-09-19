using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace GXX.Core.IO;

/// <summary>
/// MapFiles.pas 1:1 转换：内存映射文件只读流（原 CreateFileMapping/MapViewOfFile → .NET MemoryMappedFile）。
/// 用于地图(.map)与 WIL 资源的随机读取。
/// </summary>
public class TMapStream : IDisposable
{
    private string _fileName = "";
    private MemoryMappedFile? _mmf;
    private MemoryMappedViewAccessor? _view;
    private long _size;
    private long _position;

    public long Position
    {
        get => _position;
        set { if (value <= _size) _position = value; }
    }

    public long Size => _size;

    public bool LoadFromFile(string fileName)
    {
        Dispose();
        _fileName = fileName;
        _size = 0;
        _position = 0;
        if (!File.Exists(fileName)) return false;
        try
        {
            _mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            _view = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _size = fs.Length;
            return true;
        }
        catch
        {
            Dispose();
            return false;
        }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        if (count <= 0 || _view == null) return 0;
        long avail = _size - _position;
        if (avail < count) return 0;
        _view.ReadArray(_position, buffer, offset, count);
        _position += count;
        return count;
    }

    public byte ReadByte()
    {
        byte v = 0;
        _view?.Read(_position, out v);
        _position += 1;
        return v;
    }

    public short ReadInt16()
    {
        short v = 0;
        _view?.Read(_position, out v);
        _position += 2;
        return v;
    }

    public int ReadInt32()
    {
        int v = 0;
        _view?.Read(_position, out v);
        _position += 4;
        return v;
    }

    public int Seek(int offset, SeekOrigin origin)
    {
        Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => _size + offset,
            _ => Position
        };
        return (int)Position;
    }

    public int Write(byte[] buffer, int offset, int count) => 0; // 原实现为空（只读）

    public void Dispose()
    {
        _view?.Dispose(); _view = null;
        _mmf?.Dispose(); _mmf = null;
    }
}
