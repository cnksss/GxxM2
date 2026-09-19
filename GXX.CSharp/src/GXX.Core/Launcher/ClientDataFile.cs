using GXX.Core.Crypto;
using GXX.Core.Protocol;

namespace GXX.Core.Launcher;

/// <summary>
/// GXX 客户端配置数据文件（真机名 <c>ClientData.dat</c>，即 <c>TClientParam.sClientDataFile</c>；
/// 文档中也称 "Client.dat" 载荷）的读写信封。
///
/// <para>文件结构（依据 <c>Source\Client-HGE\ClMain.pas:2932-2968</c> 的 LoadConfig）：</para>
/// <code>
///   +-------------------------------+ 0
///   | payload 区（各段 zlib 数据）    |
///   +-------------------------------+ nSize
///   | TConfigClient 记录（DES 加密）  | SizeOf(TConfigClient)
///   +-------------------------------+ 文件末尾
/// </code>
/// <list type="bullet">
///   <item>记录的 <c>nSize</c> = payload 长度；<c>nCrc</c> = <see cref="CheckCrc.Crc32"/>（初值 $DBC66688）over 前 nSize 字节</item>
///   <item>整个记录用 <c>DesKey</c> 做 DES-CBC（BS=20）加解密</item>
/// </list>
///
/// <para>本类只负责"信封"（payload 切分 + 记录加解密 + CRC 重算），
/// 不假设记录内部字段布局：<see cref="Record"/> 原样保留，
/// 字段级编辑通过 <see cref="GetInt32"/>/<see cref="SetInt32"/> 等按偏移访问，
/// 这样**未映射的字段天然字节不变**，兼容性风险为零。</para>
/// </summary>
public sealed partial class ClientDataFile
{
    /// <summary>
    /// DES 密钥字符串 = <c>IntToStr(NewEncryKey^)</c>，其中 <c>Common\UnitDes.pas:1109</c> 定义
    /// <c>NewEncryKey^ := $0C08BE531</c>。
    /// <para>实测确认：走的是 **无符号 Int64 重载**（有符号值 "-1064573647" 解不开真机文件）。</para>
    /// </summary>
    public const string DesKey = "3230393649";

    /// <summary>最低可接受记录大小（防御性下界，真机 = 23324）。</summary>
    public const int MinRecordSize = 1024;

    private byte[] _payload;
    private readonly byte[] _record;

    private ClientDataFile(byte[] payload, byte[] record)
    {
        _payload = payload;
        _record = record;
    }

    /// <summary>payload 区（未被改动的原始字节）。</summary>
    public byte[] Payload => _payload;

    /// <summary>已解密的 TConfigClient 记录字节（长度即 SizeOf(TConfigClient)）。</summary>
    public byte[] Record => _record;

    /// <summary>记录在文件中的起始偏移（= payload.Length = nSize）。</summary>
    public int RecordOffset => _payload.Length;

    /// <summary>文件总长度。</summary>
    public int FileLength => _payload.Length + _record.Length;

    /// <summary>nSize 字段（记录偏移 0）。</summary>
    public int SizeField
    {
        get => GetInt32(0);
        set => SetInt32(0, value);
    }

    /// <summary>nCrc 字段（记录偏移 4）。</summary>
    public uint CrcField
    {
        get => GetUInt32(4);
        set => SetUInt32(4, value);
    }

    // ---------------------------------------------------------------- 解码 / 编码

    /// <summary>
    /// 从完整文件字节解码。
    /// 通过扫描候选记录起点定位：对偏移 S 处的第一个 20 字节块解密，
    /// 若解出的 <c>nSize == S</c> 即为命中（真机上唯一解，无歧义）。
    /// </summary>
    public static ClientDataFile Decode(byte[] file)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (file.Length < MinRecordSize + UnitDes.BS)
            throw new InvalidDataException($"文件太小 ({file.Length} B)，不可能是 GXX 配置数据文件");

        int s = FindRecordOffset(file);
        if (s < 0)
            throw new InvalidDataException(
                "未能定位 TConfigClient 记录（未找到满足 nSize==偏移 的位置）。" +
                $"密钥是否仍为 \"{DesKey}\"？或该文件不是 GXX 配置数据文件。");

        int n = file.Length - s;
        var enc = new byte[n];
        var rec = new byte[n];
        Buffer.BlockCopy(file, s, enc, 0, n);
        UnitDes.DecryptDes(enc, rec, n, DesKey);

        var payload = new byte[s];
        Buffer.BlockCopy(file, 0, payload, 0, s);

        var doc = new ClientDataFile(payload, rec);
        doc.Verify();
        return doc;
    }

    /// <summary>扫描记录起点；返回 -1 表示未找到。</summary>
    public static int FindRecordOffset(byte[] file)
    {
        var block = new byte[UnitDes.BS];
        var plain = new byte[UnitDes.BS];
        for (int s = 0; s + UnitDes.BS <= file.Length; s++)
        {
            Buffer.BlockCopy(file, s, block, 0, UnitDes.BS);
            UnitDes.DecryptDes(block, plain, UnitDes.BS, DesKey);
            if (BitConverter.ToInt32(plain, 0) == s) return s;
        }
        return -1;
    }

    /// <summary>重算 nSize/nCrc 并加密记录，产出完整文件字节。</summary>
    public byte[] Encode(bool recomputeCrc = true)
    {
        if (recomputeCrc)
        {
            SizeField = _payload.Length;
            CrcField = CheckCrc.Crc32(_payload, _payload.Length);
        }

        var enc = new byte[_record.Length];
        if (_record.Length > 0)
            UnitDes.EncryptDes(_record, enc, _record.Length, DesKey);

        var outBuf = new byte[_payload.Length + enc.Length];
        Buffer.BlockCopy(_payload, 0, outBuf, 0, _payload.Length);
        Buffer.BlockCopy(enc, 0, outBuf, _payload.Length, enc.Length);
        return outBuf;
    }

    /// <summary>校验 nSize/nCrc 与 payload 是否自洽。</summary>
    public void Verify()
    {
        int ns = SizeField;
        if (ns < 0 || ns != _payload.Length)
            throw new InvalidDataException($"nSize={ns} 与 payload 长度 {_payload.Length} 不一致");
        uint expect = CheckCrc.Crc32(_payload, _payload.Length);
        uint actual = CrcField;
        if (expect != actual)
            throw new InvalidDataException($"nCrc 校验失败：文件内 0x{actual:X8}，实算 0x{expect:X8}");
    }

    // ---------------------------------------------------------------- 记录字段访问

    private void CheckRange(int offset, int size)
    {
        if (offset < 0 || size < 0 || offset + size > _record.Length)
            throw new ArgumentOutOfRangeException(nameof(offset),
                $"偏移 {offset}+{size} 超出记录范围 (0..{_record.Length})");
    }

    public int GetInt32(int offset)
    {
        CheckRange(offset, 4);
        return BitConverter.ToInt32(_record, offset);
    }

    public void SetInt32(int offset, int value)
    {
        CheckRange(offset, 4);
        BitConverter.TryWriteBytes(_record.AsSpan(offset, 4), value);
    }

    public uint GetUInt32(int offset)
    {
        CheckRange(offset, 4);
        return BitConverter.ToUInt32(_record, offset);
    }

    public void SetUInt32(int offset, uint value)
    {
        CheckRange(offset, 4);
        BitConverter.TryWriteBytes(_record.AsSpan(offset, 4), value);
    }

    public ushort GetWord(int offset)
    {
        CheckRange(offset, 2);
        return BitConverter.ToUInt16(_record, offset);
    }

    public void SetWord(int offset, ushort value)
    {
        CheckRange(offset, 2);
        BitConverter.TryWriteBytes(_record.AsSpan(offset, 2), value);
    }

    public bool GetBool(int offset)
    {
        CheckRange(offset, 1);
        return _record[offset] != 0;
    }

    public void SetBool(int offset, bool value)
    {
        CheckRange(offset, 1);
        _record[offset] = value ? (byte)1 : (byte)0;
    }

    public byte GetByte(int offset)
    {
        CheckRange(offset, 1);
        return _record[offset];
    }

    public void SetByte(int offset, byte value)
    {
        CheckRange(offset, 1);
        _record[offset] = value;
    }

    /// <summary>读 Delphi ShortString（<c>string[N]</c>）：首字节长度 + GBK 内容。</summary>
    public string GetShortString(int offset, int capacity)
    {
        CheckRange(offset, 1 + capacity);
        return ShortStr.Get(_record, offset, capacity);
    }

    /// <summary>写 Delphi ShortString（<c>string[N]</c>）：超长按容量截断，长度字节 = 实际字节数。</summary>
    public void SetShortString(int offset, int capacity, string value)
    {
        CheckRange(offset, 1 + capacity);
        ShortStr.Set(_record, offset, capacity, value ?? "");
    }

    /// <summary>记录原始字节的只读视图（用于调试/比对）。</summary>
    public ReadOnlySpan<byte> RecordSpan => _record;

    public static ClientDataFile FromParts(byte[] payload, byte[] record)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(record);
        if (record.Length < MinRecordSize)
            throw new ArgumentException($"记录过小 ({record.Length})", nameof(record));
        return new ClientDataFile((byte[])payload.Clone(), (byte[])record.Clone());
    }
}
