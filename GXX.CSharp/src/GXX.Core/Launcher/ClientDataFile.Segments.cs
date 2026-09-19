using System.IO.Compression;
using GXX.Core.Crypto;

namespace GXX.Core.Launcher;

/// <summary>
/// <see cref="ClientDataFile"/> 的 payload 段落编辑。
///
/// <para>段落语义（已对真机文件验证）：</para>
/// <list type="bullet">
///   <item>段落数据 = <b>zlib 压缩</b>后的字节，由记录的 (Offset,Size) 指向 payload 内</item>
///   <item>带 Crc 的段落，其 Crc = <see cref="CheckCrc.Crc32"/> over <b>解压后</b>的数据
///         （已实测 <c>nBackBmpCrc = 0xD4535EA7</c> 与解压后图片的 CheckCrc32 完全一致）</item>
/// </list>
///
/// <para>替换策略：把新数据<b>追加到 payload 末尾</b>（4 字节对齐）并改写 (Offset,Size,Crc)。
/// 旧数据成为不参与引用的死区，客户端按 (Offset,Size) 读取，不受影响。
/// 这样无需重组整个 payload，未映射段落 100% 不受影响。</para>
/// </summary>
public sealed partial class ClientDataFile
{
    /// <summary>整块替换 payload（会破坏所有既有段落指针，仅内部使用）。</summary>
    private void ReplacePayloadRaw(byte[] payload) => _payload = payload;

    /// <summary>把原始（未压缩）数据作为一个段落写入，并更新 (Offset,Size[,Crc])。</summary>
    /// <param name="offsetField">段落的 Offset 字段偏移</param>
    /// <param name="sizeField">段落的 Size 字段偏移</param>
    /// <param name="crcField">段落 Crc 字段偏移；&lt;0 表示该段落没有 Crc 字段</param>
    /// <param name="rawData">未压缩数据；null 或空 = 清空该段落</param>
    /// <returns>压缩后的字节数</returns>
    public int SetSegment(int offsetField, int sizeField, int crcField, byte[] rawData)
    {
        if (rawData == null || rawData.Length == 0)
        {
            SetInt32(offsetField, _payload.Length);
            SetInt32(sizeField, 0);
            if (crcField >= 0) SetUInt32(crcField, 0);
            return 0;
        }

        byte[] comp = ZlibCompress(rawData);

        // 追加到 payload 末尾，4 字节对齐
        int pad = (4 - (_payload.Length % 4)) % 4;
        var np = new byte[_payload.Length + pad + comp.Length];
        Buffer.BlockCopy(_payload, 0, np, 0, _payload.Length);
        Buffer.BlockCopy(comp, 0, np, _payload.Length + pad, comp.Length);
        _payload = np;

        SetInt32(offsetField, _payload.Length - comp.Length);
        SetInt32(sizeField, comp.Length);
        if (crcField >= 0) SetUInt32(crcField, CheckCrc.Crc32(rawData, rawData.Length));
        return comp.Length;
    }

    /// <summary>读取一个段落的未压缩数据；不存在时返回 null。</summary>
    public byte[] GetSegmentRaw(int offsetField, int sizeField)
    {
        int off = GetInt32(offsetField), size = GetInt32(sizeField);
        if (size <= 0 || off < 0 || (long)off + size > _payload.Length) return null;
        using var ms = new MemoryStream(_payload, off, size, false);
        using var z = new ZLibStream(ms, CompressionMode.Decompress);
        using var outp = new MemoryStream();
        z.CopyTo(outp);
        return outp.ToArray();
    }

    /// <summary>读出当前记录的已知段落清单（供 GUI 展示）。</summary>
    public IReadOnlyList<(string Name, int Offset, int Size, bool HasCrc)> ListSegments()
    {
        var list = new List<(string, int, int, bool)>();
        void Add(string n, int offF, int sizeF, bool crc) => list.Add((n, GetInt32(offF), GetInt32(sizeF), crc));
        Add("基础游戏 UI", ClientDataFields.OffBaseUIOffset, ClientDataFields.OffBaseUISize, false);
        Add("共享游戏 UI(个人商店)", ClientDataFields.OffShareUIOffset, ClientDataFields.OffShareUISize, false);
        Add("连击新属性 UI", ClientDataFields.OffNewStateWindowUIOffset, ClientDataFields.OffNewStateWindowUISize, false);
        Add("内挂 UI", ClientDataFields.OffConfigDlgUIOffset, ClientDataFields.OffConfigDlgUISize, false);
        Add("及时雨 UI", ClientDataFields.OffJSYUIOffset, ClientDataFields.OffJSYUISize, false);
        Add("游戏背景图", ClientDataFields.OffBackBmpOffset, ClientDataFields.OffBackBmpSize, true);
        Add("鼠标光标", ClientDataFields.OffCursorDefOffset, ClientDataFields.OffCursorDefSize, true);
        Add("镶嵌光标", ClientDataFields.OffCursorMountOffset, ClientDataFields.OffCursorMountSize, true);
        Add("拆卸光标", ClientDataFields.OffCursorUnmountOffset, ClientDataFields.OffCursorUnmountSize, true);
        return list;
    }

    /// <summary>payload 尾部的死区大小（上次替换后残留，仅统计信息）。</summary>
    public int DeadSpaceHint
    {
        get
        {
            int max = 0;
            foreach (var (_, off, size, _) in ListSegments())
                if (size > 0 && off + size > max) max = off + size;
            return Math.Max(0, _payload.Length - max);
        }
    }

    public static byte[] ZlibCompress(byte[] raw)
    {
        using var ms = new MemoryStream();
        using (var z = new ZLibStream(ms, CompressionLevel.Optimal, true))
            z.Write(raw, 0, raw.Length);
        return ms.ToArray();
    }
}
