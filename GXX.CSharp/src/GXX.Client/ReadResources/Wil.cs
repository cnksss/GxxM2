using System;
using System.IO;
using GXX.Core;

namespace GXX.Client.ReadResources;

/// <summary>
/// Wil.pas 1:1 核心：WIL/WIS/WZL 资源文件读取（8bit 调色板图 → 位图）。
/// 文件头：#ImageCount(DWord) + #Palette(256*4)；索引表：每图 nPosition(DWord) + wWidth(Word) + wHeight(Word) + sColor(Word)。
/// </summary>
public unsafe class TWil : IDisposable
{
    public struct TWilImageInfo
    {
        public uint nPosition;
        public ushort wWidth;
        public ushort wHeight;
        public ushort sColor;
    }

    public string FileName = "";
    public int ImageCount;
    public int CurrentIndex = -1;
    public TWilImageInfo CurrentInfo;
    public byte[] CurrentData = Array.Empty<byte>();
    public int[] CurrentPixels = Array.Empty<int>(); // ARGB
    public int Version = -1;

    private FileStream? _fs;
    private BinaryReader? _br;
    private readonly int[] _palette = new int[256];

    public bool Open(string fileName, int version = -1)
    {
        Close();
        try
        {
            if (!File.Exists(fileName)) return false;
            _fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _br = new BinaryReader(_fs);
            Version = version;
            ImageCount = _br.ReadInt32();
            // 256 色调色板（BGRA → ARGB）
            for (int i = 0; i < 256; i++)
            {
                byte b = _br.ReadByte();
                byte g = _br.ReadByte();
                byte r = _br.ReadByte();
                _br.ReadByte(); // 0
                _palette[i] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
            FileName = fileName;
            return true;
        }
        catch
        {
            Close();
            return false;
        }
    }

    public void Close()
    {
        _br?.Dispose(); _br = null;
        _fs?.Dispose(); _fs = null;
        ImageCount = 0;
        CurrentIndex = -1;
        FileName = "";
    }

    public bool LoadImage(int index)
    {
        if (_br == null || index < 0 || index >= ImageCount) return false;
        try
        {
            _br.BaseStream.Position = 4 + 256 * 4 + index * 8;
            uint nPosition = _br.ReadUInt32();
            ushort w = _br.ReadUInt16();
            ushort h = _br.ReadUInt16();
            CurrentInfo = new TWilImageInfo { nPosition = nPosition, wWidth = w, wHeight = h };
            if (w == 0 || h == 0) return false;
            _br.BaseStream.Position = nPosition;
            CurrentData = _br.ReadBytes((int)w * h);
            CurrentPixels = new int[w * h];
            for (int i = 0; i < CurrentData.Length; i++)
                CurrentPixels[i] = _palette[CurrentData[i]];
            CurrentIndex = index;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose() => Close();
}
