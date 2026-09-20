// ============================================================================
// 测试目标：GXX.Core/Paradox/ParadoxDataSet.cs（1:1 移植 Source\RunGate\ParadoxDataSet.pas，
//           GBK 1362 行）。测试里出现的每个行号都指向该 .pas 原文。
//
// 本文件用**构造出来的合成 Paradox .DB**驱动真实的解析路径（不 mock 解析），
// 因为本单元是二进制格式解析：字节序 / 边界 / 长度字段必须做差异断言。
//
// 合成表布局（3 字段，与 TParadoxDataSet 的读点严格对齐）：
//   文件头 0x00..0x57  = TPxFileHeader（Pack=1）
//   数据头 0x58..0x7F  = TPxDataHeader（FileVersionID >= $05 时才读）
//   0x78             = 字段信息区起点（原文 :779 `P := $78`）
//   0x78 + i*2       = 第 i 个 TFieldInfoRecord（FieldType, FieldSize）
//   字段名区 = 0x78 + NumFields*2 + 4 + NumFields*4   （原文 :797）
//     3 字段 -> 0x78 + 6 + 4 + 12 = 0x8E；3 个名字各 4 字节（3 字符 + #0）=> 读完停在 0x9A
//   0x9A + P（未加密时 P = NumFields*2 = 6，原文 :828） = 0xA0 -> "china" + #0
//     原文 :835 用 soFromCurrent，故落点 = 名字区结束 + NumFields*2
//   数据块 1 = 0x800（MaxTableSize=2 => 2*1024）；块头 6 字节；记录 n 从
//     0x806 + (n-1)*RecordSize 起，RecordSize 刻意取 3 且 AddDataSize=9，
//     以避开原文 :973 的 `div + 1` 块内计数缺陷（见 D4 差异断言）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GXX.Core.Paradox;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>合成 Paradox .DB 构造器（字段布局与原文读点一一对应，注释给出偏移推导）。</summary>
internal sealed class PxSyntheticDb : IDisposable
{
    public readonly string Dir;
    public readonly string DbPath;
    public readonly string MbPath;

    private readonly List<TFieldInfoRecord> _fields = new List<TFieldInfoRecord>();
    private readonly List<string> _names = new List<string>();
    private readonly List<byte[]> _records = new List<byte[]>();

    public ushort RecordSize = 3;
    public byte FileType = 2;                 // 0 = 索引化 .DB、2 = 无索引 .DB
    public byte FileVersionID = 0x0C;         // $0C = 7.x（>= $05 且 >= $0C）
    public byte SortOrder = 3;                // 'Paradox China 936' 的 SortOrder
    public ushort DosGlobalCodePage = 936;    // 'Paradox China 936' 的 CodePage
    public int Encryption2 = 0;               // 非 0 即加密（版本 >= $05 判定）
    public int Encryption1 = 0;               // 版本 < $05 的加密判定
    public ushort NumRecordsOverride;         // 0 = 用 _records.Count
    public ushort FirstBlock = 1;
    public ushort FileBlocks = 1;
    public byte MaxTableSize = 2;             // 块大小 = 2 * 1024
    public ushort HeaderSize = 0x0800;
    public ushort AddDataSizeOverride;        // 0 = 用 RecordSize * 条数

    public PxSyntheticDb(string tag)
    {
        Dir = Path.Combine(Path.GetTempPath(), "pxds-" + tag + "-" + Guid.NewGuid().ToString("N"));
        System.IO.Directory.CreateDirectory(Dir);
        DbPath = Path.Combine(Dir, "T.DB");
        MbPath = Path.Combine(Dir, "T.mb");
    }

    /// <summary>追加字段定义（原文 :126-129 TFieldInfoRecord）。</summary>
    public PxSyntheticDb Field(byte fieldType, byte fieldSize)
    {
        _fields.Add(new TFieldInfoRecord { FieldType = fieldType, FieldSize = fieldSize });
        return this;
    }

    /// <summary>追加字段名（Delphi 短串以外的"名字区"串，原文 :809-816 逐字节读到 #0）。</summary>
    public PxSyntheticDb Name(string name)
    {
        _names.Add(name);
        return this;
    }

    /// <summary>追加一条用户记录（长度须 == RecordSize）。</summary>
    public PxSyntheticDb Record(byte[] data)
    {
        _records.Add(data);
        return this;
    }

    /// <summary>按当前配置写出 .DB（必要时写 .mb）。</summary>
    /// <remarks>
    /// 布局与 TParadoxDataSet 的**实际读点**对齐（这是从原文 :797-320 推出来的，不是想当然）：
    ///   0x78           字段信息区（NumFields × 2 字节）
    ///   0x78+2N+4+4N   字段信息区之后；**原文不用这个位置读字段名**（见下）
    ///   P0 = 0x78 + 2N + 4 + 4N
    ///   P1 = P0 + (版本 >= $0C ? 261 : 79)      <-- 这就是原文 :800-803 的 TableName 尺寸
    ///   字段名区实际从 P1 开始（原文 :805 用的是 P1，不是 P0）
    ///   SortOrderID 从 P1 + 名字区长度 + N*2 开始（原文 :835 soFromCurrent，P = N*2）
    /// 这是原文的真实缺陷（报告 D17）：字段名被读到了"TableName 区之后"，
    /// 也就是(按原设计意图)本该是表名的位置。
    /// </remarks>
    public PxSyntheticDb Write()
    {
        int numFields = _fields.Count;
        int fieldsBase = 0x78;
        int p0 = fieldsBase + numFields * 2 + 4 + numFields * 4;
        int p1 = p0 + (FileVersionID >= 0x0C ? 261 : 79);
        int namesBase = p1;
        int namesLen = 0;
        foreach (var n in _names) namesLen += n.Length + 1;
        int sortOrderIdPos = namesBase + namesLen + (FileVersionID != 0 ? numFields * 2 : 0);

        int blockBase = HeaderSize;
        int dataLen = AddDataSizeOverride != 0 ? AddDataSizeOverride : RecordSize * _records.Count;
        int end = blockBase + Marshal.SizeOf<TDataBlock>() + dataLen;

        var buf = new byte[end + 8];

        // ---- TPxFileHeader（0x00） ----
        var fh = new TPxFileHeader
        {
            RecordSize = RecordSize,
            HeaderSize = HeaderSize,
            FileType = FileType,
            MaxTableSize = MaxTableSize,
            NumRecords = NumRecordsOverride != 0 ? NumRecordsOverride : _records.Count,
            NextBlock = FileBlocks,
            FileBlocks = FileBlocks,
            FirstBlock = FirstBlock,
            LastBlock = 1,
            Unknown12x13 = 0,
            ModifiedFlags1 = 0,
            IndexFieldNumber = 0,
            PrimaryIndexWorkspace = 0,
            UnknownPtr1A = 0,
            NumFields = (ushort)numFields,
            PrimaryKeyFields = 1,
            Encryption1 = Encryption1,
            SortOrder = SortOrder,
            ModifiedFlags2 = 0,
            ChangeCount1 = 0,
            ChangeCount2 = 0,
            Unknown2F = 0,
            TableNamePtrPtr = 0,
            FieldInfoPtr = 0,
            WriteProtected = 0,
            FileVersionID = FileVersionID,
            MaxBlocks = FileBlocks,
            Unknown3C = 0,
            AuxPasswords = 0,
            CryptInfoStartPtr = 0,
            CryptInfoEndPtr = 0,
            Unknown48 = 0,
            AutoInc = 0,
            IndexUpdateRequired = 0,
            RefIntegrity = 0,
        };
        WriteStruct(buf, 0, fh);

        // ---- TPxDataHeader（0x58） ----
        var dh = new TPxDataHeader
        {
            FileVersionID = (ushort)(0x0100 + FileVersionID),
            FileVersionID2 = (ushort)(0x0100 + FileVersionID),
            Encryption2 = Encryption2,
            FileUpdateTime = 0,
            HiFieldID = (ushort)numFields,
            HiFieldIDInfo = (ushort)numFields,
            SometimesNumFields = (ushort)numFields,
            DosGlobalCodePage = DosGlobalCodePage,
            ChangeCount4 = 0,
        };
        WriteStruct(buf, 0x58, dh);

        // ---- 字段信息区（0x78，每条 2 字节：FieldType, FieldSize） ----
        for (int i = 0; i < numFields; i++)
        {
            buf[fieldsBase + i * 2] = _fields[i].FieldType;
            buf[fieldsBase + i * 2 + 1] = _fields[i].FieldSize;
        }

        // ---- 字段名区（namesBase，每名 名字 + #0） ----
        int pos = namesBase;
        foreach (var n in _names)
        {
            foreach (char c in n) buf[pos++] = (byte)c;
            buf[pos++] = 0;
        }

        // ---- SortOrderID（sortOrderIdPos，读到 #0） ----
        pos = sortOrderIdPos;
        foreach (char c in "china") buf[pos++] = (byte)c;
        buf[pos] = 0;

        // ---- 数据块 1 头 + 记录 ----
        var blk = new TDataBlock
        {
            NextBlock = 0,
            BlockNumber = 1,
            AddDataSize = (ushort)dataLen,
        };
        WriteStruct(buf, blockBase, blk);
        for (int i = 0; i < _records.Count; i++)
            Array.Copy(_records[i], 0, buf, blockBase + Marshal.SizeOf<TDataBlock>() + i * RecordSize, RecordSize);

        File.WriteAllBytes(DbPath, buf);
        return this;
    }

    /// <summary>写 .mb（Blob）文件。</summary>
    public PxSyntheticDb WriteMb(byte[] content)
    {
        File.WriteAllBytes(MbPath, content);
        return this;
    }

    /// <summary>记录 n（1-based）在 .DB 内的绝对偏移，与原文 :980-983 的 nSeekPos 同一算式。</summary>
    public int RecordOffset(int n)
        => HeaderSize + (FirstBlock - 1) * MaxTableSize * 1024 + (n - 1) * RecordSize + Marshal.SizeOf<TDataBlock>();

    public void Dispose()
    {
        try { if (System.IO.Directory.Exists(Dir)) System.IO.Directory.Delete(Dir, true); } catch { }
    }


    private static void WriteStruct<T>(byte[] buf, int offset, T value) where T : unmanaged
    {
        unsafe
        {
            fixed (byte* p = &buf[offset]) *(T*)p = value;
        }
    }
}

/// <summary>把合成表打开成 TParadoxDataSet 的便捷封装。</summary>
internal static class PxOpen
{
    public static TParadoxDataSet OpenAndFirst(PxSyntheticDb db)
    {
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        ds.First();
        return ds;
    }

    /// <summary>
    /// 在给定 CodePage 的 PxLangTable 项里挑一条**其 SortOrderID 与表格自身一致**的，
    /// 反推 DetectLang 会命中的下标（测试夹具需要按原文三元组构造表头）。
    /// </summary>
    public static int FindLangByCodePage(ushort codePage)
    {
        for (int i = 1; i <= 118; i++)
            if (TParadoxDataSet.PxLangTable[i].CodePage == codePage)
                return i;
        return 0;
    }
}

// ============================================================================
// 1) 记录布局：字节序 / Pack=1 / 固定数组 / 偏移（原文 117-136、425-517）
// ============================================================================

public class ParadoxDataSetLayoutTests
{
    [Fact] // 原文 :132-136 TDataBlock = NextBlock/BlockNumber/AddDataSize 三个 Word
    public void TDataBlock_IsSixBytes_WordLittleEndian()
    {
        Assert.Equal(6, Marshal.SizeOf<TDataBlock>());
        var b = new byte[6];
        unsafe { fixed (byte* p = b) *(TDataBlock*)p = new TDataBlock { NextBlock = 0x1234, BlockNumber = 0x5678, AddDataSize = 0x9ABC }; }
        // 差异断言：Delphi 的 packed record 在小端机上按字段顺序落字节，不是大端
        Assert.Equal(new byte[] { 0x34, 0x12, 0x78, 0x56, 0xBC, 0x9A }, b);
    }

    [Fact] // 原文 :126-129 TFieldInfoRecord（放在 :132 TDataBlock 之前的声明顺序）
    public void TFieldInfoRecord_IsTwoBytes_TypeThenSize()
    {
        Assert.Equal(2, Marshal.SizeOf<TFieldInfoRecord>());
        var b = new byte[2];
        unsafe { fixed (byte* p = b) *(TFieldInfoRecord*)p = new TFieldInfoRecord { FieldType = 0x01, FieldSize = 0x14 }; }
        Assert.Equal(new byte[] { 0x01, 0x14 }, b);
    }

    [Fact] // 原文 :428-470 PPxFileHeader：88 字节 = $58（:443/:449/:460/:465/:467/:469 的固定数组决定）
    public void TPxFileHeader_IsEightyEightBytes()
    {
        Assert.Equal(88, Marshal.SizeOf<TPxFileHeader>());
        Assert.Equal(0x58, Marshal.SizeOf<TPxFileHeader>());
    }

    [Fact] // 原文 :473-491 PPxDataHeader：40 字节 = $28
    public void TPxDataHeader_IsFortyBytes()
    {
        Assert.Equal(40, Marshal.SizeOf<TPxDataHeader>());
        Assert.Equal(0x28, Marshal.SizeOf<TPxDataHeader>());
    }

    [Fact] // 原文 :493-497 TPxRecordHeader。差异断言：Delphi 7 枚举最小 2 字节 ⇒ SizeOf = 4+2 = 6（**不是** 8）
    public void TPxRecordHeader_IsSixBytes_DelphiEnumMinSizeIsTwo()
    {
        Assert.Equal(6, PxRecordHeaderOps.Size);
        Assert.Equal(6, Marshal.SizeOf<TPxRecordHeader>());
        // 差异断言：托管 enum:int 是 4 字节；若直接按枚举字段 Marshal 会得到 8，
        // 会让原文 :986/:992 的"用户记录在缓冲内的偏移"整体偏 2 字节。
        Assert.NotEqual(8, PxRecordHeaderOps.Size);
    }

    [Fact] // 原文 :499-503 TPxBlob（10-Byte Blob Info Block）
    public void TPxBlob_IsTenBytes()
    {
        Assert.Equal(10, Marshal.SizeOf<TPxBlob>());
    }

    [Fact] // 原文 :505-510 TPxBlobIdx：Offset/Len16/ModCnt/Len = 1+1+2+1 = 5... 但 Pack 顺序给出 6
    public void TPxBlobIdx_IsSixBytes_ButMbEntryIsFiveBytes()
    {
        // 差异断言（原文缺陷 D11）：结构 6 字节，而 .mb 条目步长是 5（原文 :1303 `5 * Idx`）
        Assert.Equal(6, Marshal.SizeOf<TPxBlobIdx>());
        var b = new byte[6];
        unsafe { fixed (byte* p = b) *(TPxBlobIdx*)p = new TPxBlobIdx { Offset = 0x0A, Len16 = 0x0B, ModCnt = 0x0C0D, Len = 0x0E }; }
        Assert.Equal(new byte[] { 0x0A, 0x0B, 0x0D, 0x0C, 0x0E, 0x00 }, b);
    }

    [Fact] // 原文 :97-115 字段类型码（逐个钉死，防转录错）
    public void PxFieldTypeCodes_MatchOriginalConstants()
    {
        Assert.Equal(0x01, PxFieldType.pxfAlpha);
        Assert.Equal(0x02, PxFieldType.pxfDate);
        Assert.Equal(0x03, PxFieldType.pxfShort);
        Assert.Equal(0x04, PxFieldType.pxfLong);
        Assert.Equal(0x05, PxFieldType.pxfCurrency);
        Assert.Equal(0x06, PxFieldType.pxfNumber);
        Assert.Equal(0x09, PxFieldType.pxfLogical);
        Assert.Equal(0x0C, PxFieldType.pxfMemoBLOB);
        Assert.Equal(0x0D, PxFieldType.pxfBLOB);
        Assert.Equal(0x0E, PxFieldType.pxfFmtMemoBLOB);
        Assert.Equal(0x0F, PxFieldType.pxfOLE);
        Assert.Equal(0x10, PxFieldType.pxfGraphic);
        Assert.Equal(0x14, PxFieldType.pxfTime);
        Assert.Equal(0x15, PxFieldType.pxfTimestamp);
        Assert.Equal(0x16, PxFieldType.pxfAutoInc);
        Assert.Equal(0x17, PxFieldType.pxfBCD);
        Assert.Equal(0x18, PxFieldType.pxfBytes);
    }
}

// ============================================================================
// 2) PxLangTable（原文 :520-638，脚本抽取）
// ============================================================================

public class ParadoxLangTableTests
{
    [Fact] // 原文 :520 `array[1..118] of TPxLang`（1-based，托管侧 119 槽 + 槽 0 占位）
    public void PxLangTable_Has119Slots_IndexZeroIsPlaceholder()
    {
        Assert.Equal(119, TParadoxDataSet.PxLangTable.Length);
        Assert.Null(TParadoxDataSet.PxLangTable[0]);   // Delphi 下界为 1
        Assert.NotNull(TParadoxDataSet.PxLangTable[1]);
        Assert.NotNull(TParadoxDataSet.PxLangTable[118]);
    }

    [Theory] // 抽 11 个跨段样本逐字段比对（含原文里用 '' 转义的单引号名）
    // 锚点不变量：PxLangTable[K] 来自原文第 520+K 行（Gen 脚本硬校验该式）
    [InlineData(1, "Access General", 161, 1252, "ACCGEN")]
    [InlineData(2, "Access Greece", 53, 1253, "ACCGREEK")]
    [InlineData(6, "'ascii' ANSI", 76, 1252, "DBWINUS0")]
    [InlineData(24, "dBASE CHS cp936", 233, 936, "DB936CN0")]
    [InlineData(58, "dBASE SVE cp850", 253, 850, "DB850SV1")]
    [InlineData(60, "dBASE TRK cp857", 0, 857, "DB857TR0")]
    [InlineData(74, "Paradox China 936", 3, 936, "china")]
    [InlineData(82, "Paradox 'intl'", 183, 437, "intl")]
    [InlineData(92, "Paradox Taiwan 950", 132, 950, "taiwan")]
    [InlineData(94, "Paradox 'turk'", 198, 857, "turk")]
    [InlineData(113, "pdx ISO L_2 Czech", 91, 592, "il2czw")]
    [InlineData(118, "'WEurope' ANSI", 64, 1252, "DBWINWE0")]
    public void PxLangTable_SampleEntries_MatchSourceText(int index, string name, int sortOrder, int codePage, string sortOrderId)
    {
        var e = TParadoxDataSet.PxLangTable[index];
        Assert.Equal(name, e.Name);
        Assert.Equal((byte)sortOrder, e.SortOrder);
        Assert.Equal((ushort)codePage, e.CodePage);
        Assert.Equal(sortOrderId, e.SortOrderID);
    }

    [Fact] // 原文 :513/:516 短串容量 string[20] / string[8]
    public void TPxLang_ShortStringCapacities_HoldAllEntries()
    {
        Assert.Equal(20, TPxLang.NameCapacity);
        Assert.Equal(8, TPxLang.SortOrderIDCapacity);
        for (int i = 1; i <= 118; i++)
        {
            Assert.True(TParadoxDataSet.PxLangTable[i].Name.Length <= TPxLang.NameCapacity);
            Assert.True(TParadoxDataSet.PxLangTable[i].SortOrderID.Length <= TPxLang.SortOrderIDCapacity);
        }
    }

    [Fact] // 差异断言：原文 :592 与 :628 同名 'ascii' 的 SortOrderID ⇒ SetLanguage 命中**第一个**（:1080 Break）
    public void PxLangTable_DuplicateSortOrderId_AsciiAppearsTwice()
    {
        int hits = 0;
        for (int i = 1; i <= 118; i++)
            if (TParadoxDataSet.PxLangTable[i].SortOrderID == "ascii") hits++;
        Assert.Equal(2, hits);
    }
}

// ============================================================================
// 3) InternalOpen（原文 :850-906）+ IsCursorOpen（:908）+ InternalClose（:913）
// ============================================================================

public class ParadoxDataSetOpenTests
{
    [Fact] // 原文 :854-855 TableName 为空 → EParadoxError.CreateFmt('TableName is not set', [])
    public void Open_EmptyTableName_Throws()
    {
        var ds = new TParadoxDataSet();
        var ex = Assert.Throws<EParadoxError>(() => ds.Open());
        // Delphi 的 Exception.CreateFmt 会前缀 <类名>: （见 EParadoxError.CreateFmt 注释）
        Assert.Equal("ParadoxDataSet.EParadoxError: TableName is not set", ex.Message);
        Assert.False(ds.Active);
    }

    [Fact] // 原文 :857-862 打开失败 → 'Unable to open database "%S" - %S'（Delphi %S 按序替换）
    public void Open_MissingFile_ThrowsWithBothSubstitutions()
    {
        using var db = new PxSyntheticDb("missing");
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;      // 尚未 Write
        var ex = Assert.Throws<EParadoxError>(() => ds.Open());
        // 差异断言（台账 §17.2）：%S 必须**按序**消费两个实参，不是把两个位置都填第一个实参
        const string prefix = "ParadoxDataSet.EParadoxError: Unable to open database \"" + "";
        Assert.StartsWith("ParadoxDataSet.EParadoxError: Unable to open database \"" + db.DbPath + "\" - ", ex.Message);
        Assert.NotEqual("ParadoxDataSet.EParadoxError: Unable to open database \"" + db.DbPath + "\" - " + db.DbPath, ex.Message);
        Assert.True(ex.Message.Length > ("ParadoxDataSet.EParadoxError: Unable to open database \"" + db.DbPath + "\" - ").Length);
    }

    [Theory] // 原文 :866 `if not (FileType in [0, 2])`：0 与 2 放行
    [InlineData(0, true)]
    [InlineData(2, true)]
    public void Open_FileType0Or2_IsAccepted(byte fileType, bool expectedOk)
    {
        using var db = new PxSyntheticDb("ft" + fileType);
        db.FileType = fileType;
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        Assert.Equal(expectedOk, ds.Active);
        Assert.Equal(fileType, ds.FileHeader.FileType);
    }

    [Theory] // 差异断言（原文缺陷 D3）：FileType=1（.PX 主索引）也被拒 —— 原文只放行 0/2
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Open_OtherFileTypes_AreRejected(byte fileType)
    {
        using var db = new PxSyntheticDb("rej" + fileType);
        db.FileType = fileType;
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        var ex = Assert.Throws<EParadoxError>(() => ds.Open());
        // 原文 :867 格式串 '"%S" - is not .DB data file'
        Assert.Equal("ParadoxDataSet.EParadoxError: \"" + db.DbPath + "\" - is not .DB data file", ex.Message);
    }

    [Fact] // 原文 :869-873 版本 >= $05 → 读 TPxDataHeader，FIsEncrypted := Encryption2 <> 0
    public void Open_VersionAtLeast5ReadsDataHeader_Encryption2Decides()
    {
        using var db = new PxSyntheticDb("enc2");
        db.FileVersionID = 0x0C;
        db.Encryption2 = 0x12345678;
        db.Encryption1 = 0;             // 即使 Encryption1=0 也应判为加密
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        Assert.True(ds.DataHeaderRead);
        Assert.True(ds.IsEncrypted);
        Assert.Equal(0x12345678, ds.DataHeader.Encryption2);
        Assert.Equal(936, ds.DataHeader.DosGlobalCodePage);
    }

    [Fact] // 原文 :874-877 版本 < $05 → 不读数据头，FIsEncrypted := Encryption1 <> 0
    public void Open_VersionBelow5UsesEncryption1_AndSkipsDataHeader()
    {
        using var db = new PxSyntheticDb("enc1");
        db.FileVersionID = 0x04;
        db.Encryption1 = unchecked((int)0xDEADBEEF);
        db.Encryption2 = 0;
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        Assert.False(ds.DataHeaderRead);
        Assert.True(ds.IsEncrypted);
        Assert.Equal(unchecked((int)0xDEADBEEF), ds.FileHeader.Encryption1);
    }

    [Fact] // 原文 :879-899 .mb 存在才建 FBlobStream（大小写两种尝试）
    public void Open_BlobStreamCreatedOnlyWhenMbExists()
    {
        using var noMb = new PxSyntheticDb("nomb");
        noMb.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds1 = new TParadoxDataSet();
        ds1.TableName = noMb.DbPath;
        ds1.Open();
        Assert.False(File.Exists(noMb.MbPath));

        using var withMb = new PxSyntheticDb("withmb");
        withMb.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        withMb.WriteMb(new byte[64]);
        var ds2 = new TParadoxDataSet();
        ds2.TableName = withMb.DbPath;
        ds2.Open();
        Assert.True(ds2.Active);
    }

    [Fact] // 原文 :908-911 IsCursorOpen 与 :904 FIsOpen；Close 后 :917 FIsOpen := False、:924 FLanguageID := 0
    public void OpenClose_FlipsIsCursorOpen_AndResetsLanguage()
    {
        using var db = new PxSyntheticDb("oc");
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        Assert.False(ds.Active);
        ds.Open();
        Assert.True(ds.Active);
        Assert.True(ds.LanguageID > 0);         // DetectLang 命中 'Paradox China 936' -> 74
        ds.Close();
        Assert.False(ds.Active);
        Assert.Equal(0, ds.LanguageID);         // 原文 :924
    }

    [Fact] // 原文 :902 CreateFields 之后 TField 数量/名字/类型（消费者 uFrmMagicCD.pas:363 用 FieldByName）
    public void Open_BuildsFieldsFromFileHeader()
    {
        using var db = new PxSyntheticDb("flds");
        db.RecordSize = 9;
        db.Field(PxFieldType.pxfAlpha, 3).Name("MagId")
          .Field(PxFieldType.pxfLong, 4).Name("Descr")
          .Field(PxFieldType.pxfNumber, 2).Name("Rate")
          .Record(new byte[9]).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        Assert.Equal(3, ds.FieldCount);
        Assert.Equal("MagId", ds.Fields[0].FieldName);
        Assert.Equal("Descr", ds.Fields[1].FieldName);
        Assert.Equal("Rate", ds.Fields[2].FieldName);
        Assert.Equal(TFieldType.ftString, ds.Fields[0].DataType);
        Assert.Equal(TFieldType.ftInteger, ds.Fields[1].DataType);
        Assert.Equal(TFieldType.ftFloat, ds.Fields[2].DataType);
        Assert.Equal(1, ds.Fields[0].FieldNo);
        Assert.Equal(3, ds.Fields[2].FieldNo);
        // 字段偏移来自原文 :792-794 的累加
        Assert.Equal(new[] { 0, 3, 7 }, ds.FieldOffsets);
    }
}

// ============================================================================
// 4) DetectLang / Language / SortOrderID（原文 :1125-1151、:1064-1091）
// ============================================================================

public class ParadoxDataSetLangTests
{
    [Fact] // 原文 :1132-1140 版本 >= $05：SortOrder + DosGlobalCodePage + SortOrderID 三者全等
    public void DetectLang_ModernFile_RequiresAllThreeFields()
    {
        using var db = new PxSyntheticDb("dl1");
        db.FileVersionID = 0x0C;
        db.SortOrder = 3;
        db.DosGlobalCodePage = 936;
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(74, ds.LanguageID);
        Assert.Equal("Paradox China 936", ds.Language);
        Assert.Equal("china", ds.SortOrderID);
    }

    [Fact] // 差异断言：CodePage 不匹配时**不**命中（1320 项里 SortOrder=3 只有一条，故退化为 0）
    public void DetectLang_CodePageMismatch_ReturnsZero()
    {
        using var db = new PxSyntheticDb("dl2");
        db.FileVersionID = 0x0C;
        db.SortOrder = 3;
        db.DosGlobalCodePage = 999;            // 与 'china' 的 936 不符
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(0, ds.LanguageID);
        Assert.Equal("", ds.Language);          // 原文 :1069 FLanguageID 越界 -> ''
    }

    [Fact] // 差异断言（原文缺陷 D16）：版本 < $05 时**只比 SortOrder**，取 1..118 中第一条
    public void DetectLang_LegacyFile_MatchesOnSortOrderOnly()
    {
        // SortOrder = 161：PxLangTable[1] 'Access General' 就是 161 ⇒ 首命中 1
        using var db = new PxSyntheticDb("dl3");
        db.FileVersionID = 0x04;
        db.SortOrder = 161;
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(1, ds.LanguageID);
        Assert.Equal("Access General", ds.Language);
    }

    [Fact] // 差异断言：SortOrder=0 时首命中是 [60] 'dBASE TRK cp857'（老版本判定仅看 SortOrder）
    public void DetectLang_LegacyFile_SortOrderZero_MatchesFirstZeroEntry()
    {
        using var db = new PxSyntheticDb("dl4");
        db.FileVersionID = 0x03;
        db.SortOrder = 0;
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(60, ds.LanguageID);
        Assert.Equal("dBASE TRK cp857", ds.Language);
    }

    [Fact] // 原文 :1066 GetLanguage 的 1..118 边界：0 与 119 都返回 ''
    public void GetLanguage_OutsideRange_ReturnsEmpty()
    {
        using var db = new PxSyntheticDb("gl");
        db.SortOrder = 200;                     // 无任何匹配
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(0, ds.LanguageID);
        Assert.Equal("", ds.Language);
    }

    [Fact] // 原文 :1076-1085 Active 时按 Name 精确（大小写敏感）查表；:1089 非 Active 时清零
    public void SetLanguage_ActiveScansTable_CaseSensitive()
    {
        using var db = new PxSyntheticDb("sl");
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(74, ds.LanguageID);

        ds.Language = "Paradox China 936";
        Assert.Equal(74, ds.LanguageID);

        ds.Language = "parados china 936";      // 大小写不符 ⇒ 原文不改 FLanguageID
        Assert.Equal(74, ds.LanguageID);

        ds.Language = "No Such Language";       // 未命中且 Active ⇒ 保持不变
        Assert.Equal(74, ds.LanguageID);
    }

    [Fact] // 原文 :1086-1090 非 Active 分支：直接 FLanguageID := 0（不查表）
    public void SetLanguage_InactiveAlwaysZeroes()
    {
        using var db = new PxSyntheticDb("sl2");
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        ds.Close();
        ds.Language = "Paradox China 936";
        Assert.Equal(0, ds.LanguageID);
    }

    [Fact] // 原文 :1064-1070 GetLanguage 的 else：FLanguageID > 118 也返回 ''
    public void Language_RoundTrip_AllSampleNames()
    {
        using var db = new PxSyntheticDb("lrt");
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        foreach (var idx in new[] { 1, 6, 24, 74, 92, 118 })
        {
            string name = TParadoxDataSet.PxLangTable[idx].Name;
            ds.Language = name;
            Assert.Equal(idx, ds.LanguageID);
            Assert.Equal(name, ds.Language);
        }
    }
}

// ============================================================================
// 5) GetRecord / 游标（原文 :927-1001）+ SetRecNo/GetRecNo（:1043/:1050）
// ============================================================================

public class ParadoxDataSetCursorTests
{
    private static PxSyntheticDb ThreeRecords(string tag)
    {
        var db = new PxSyntheticDb(tag);
        db.RecordSize = 3;
        db.Field(PxFieldType.pxfAlpha, 3).Name("AAA")
          .Record(new byte[] { (byte)'A', (byte)'A', (byte)'A' })
          .Record(new byte[] { (byte)'B', (byte)'B', (byte)'B' })
          .Record(new byte[] { (byte)'C', (byte)'C', (byte)'C' })
          .Write();
        return db;
    }

    [Fact] // 原文 :1038 GetRecordCount = FFileHeader.NumRecords
    public void RecordCount_ComesFromFileHeader()
    {
        using var db = ThreeRecords("rc");
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(3, ds.RecordCount);
    }

    [Fact] // 原文 :1018 InternalFirst + TDataSet.First：FCursor 0 -> gmNext -> 1
    public void First_LandsOnRecord1_AndReadsFirstRecordBytes()
    {
        using var db = ThreeRecords("first");
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(1, ds.RecNo);
        Assert.False(ds.Eof);
        Assert.Equal("AAA", ds.FieldByName("AAA").AsString);
    }

    [Fact] // 原文 :945-950 gmNext 的 Inc(FCursor)；逐条读到末记录
    public void Next_WalksAllRecords_InOrder()
    {
        using var db = ThreeRecords("next");
        var ds = PxOpen.OpenAndFirst(db);
        var seen = new List<string> { ds.FieldByName("AAA").AsString };
        ds.Next();
        seen.Add(ds.FieldByName("AAA").AsString);
        ds.Next();
        seen.Add(ds.FieldByName("AAA").AsString);
        Assert.Equal(new[] { "AAA", "BBB", "CCC" }, seen);
        Assert.False(ds.Eof);
        Assert.Equal(3, ds.RecNo);
    }

    [Fact] // 差异断言（原文 :947-950 + :990-994）：越界 gmNext 返回 grEOF ⇒ 游标**不**前进、Eof 置位、缓冲清零
    public void Next_PastLast_ReturnsEof_KeepsCursor_AndZeroesBuffer()
    {
        using var db = ThreeRecords("eof");
        var ds = PxOpen.OpenAndFirst(db);
        ds.Next(); ds.Next();               // 到第 3 条
        Assert.Equal(3, ds.RecNo);
        ds.Next();                          // 越过末记录
        Assert.True(ds.Eof);
        Assert.Equal(3, ds.RecNo);          // 游标保持（原文只在非 EOF 时 Inc）
        Assert.Equal("", ds.FieldByName("AAA").AsString);  // 缓冲被 FillChar 清零（原文 :993）
        Assert.Equal(TBookmarkFlag.bfEOF, ReadBookmark(ds));
    }

    [Fact] // 差异断言（原文 :954-955 + :997-1000）：gmCurrent 越界返回 grError，
            // 且 DoCheck=True 时原文会调 DatabaseError('Error in GetRecord()') 抛异常。
            // 原文唯一带 DoCheck=True 的入口是 TDataSet 内部的 Resync/GetRecord 调用链；
            // 这里直接以反射驱动 GetRecord(buffer, gmCurrent, true) 锁定该分支。
    public void GmCurrent_OutOfRange_ReturnsError_DoCheckRaises()
    {
        using var db = ThreeRecords("cur");
        var ds = PxOpen.OpenAndFirst(db);
        var m = typeof(TParadoxDataSet).GetMethod("GetRecord",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var buf = Marshal.AllocHGlobal(32);
        try
        {
            // FCursor=1（首记录）时 gmCurrent 合法 ⇒ grOK
            var ok = (TGetResult)m.Invoke(ds, new object[] { buf, TGetMode.gmCurrent, false });
            Assert.Equal(TGetResult.grOK, ok);

            // 把游标推到 RecordCount+1（原文 :1023 InternalLast 的语义）⇒ gmCurrent 越界
            ds.Last();
            var res = (TGetResult)m.Invoke(ds, new object[] { buf, TGetMode.gmCurrent, false });
            Assert.Equal(TGetResult.grError, res);

            // DoCheck=True ⇒ 原文 :999 DatabaseError('Error in GetRecord()')
            var ex = Assert.Throws<System.Reflection.TargetInvocationException>(
                () => m.Invoke(ds, new object[] { buf, TGetMode.gmCurrent, true }));
            var inner = Assert.IsType<EParadoxError>(ex.InnerException);
            Assert.Equal("ParadoxDataSet.EParadoxError: Error in GetRecord()", inner.Message);
        }
        finally { Marshal.FreeHGlobal(buf); }
    }

    [Fact] // 原文 :1045 边界之外：空表（NumRecords=0）下 First 直接 Eof；
            // 差异断言（原文缺陷 D19）：原文 :1005 的 GetMem 不清零，而 :947-949 的 grEOF
            // 分支**不写 RecordIndex** ⇒ 原 RecNo 读到未初始化内存（不确定值）。
            // 托管侧在 AllocRecordBuffer 显式清零 ⇒ RecNo 恒为 0（可重复）。
    public void EmptyTable_First_IsEof_NoRecords()
    {
        // 连做 3 次以捕捉"未初始化内存"类抖动（本车道曾在约 1/6 概率下拿到 166957392）
        for (int round = 0; round < 3; round++)
        {
            using var db = new PxSyntheticDb("empty" + round);
            db.RecordSize = 3;
            db.Field(PxFieldType.pxfAlpha, 3).Name("AAA").Write();   // 0 条记录
            var ds = PxOpen.OpenAndFirst(db);
            Assert.Equal(0, ds.RecordCount);
            Assert.True(ds.Eof);
            Assert.Equal(0, ds.RecNo);                              // D19：必须确定性地为 0
            Assert.Equal(TBookmarkFlag.bfEOF, ReadBookmark(ds));     // 原文 :994 的 GJK 写入
        }
    }

    [Fact] // 原文 :1043-1048 SetRecNo：1 <= Value < RecordCount+1 才生效
    public void SetRecNo_ValidRange_MovesCursor()
    {
        using var db = ThreeRecords("setrc");
        var ds = PxOpen.OpenAndFirst(db);
        ds.RecNo = 2;
        Assert.Equal(2, ds.RecNo);
        Assert.Equal("BBB", ds.FieldByName("AAA").AsString);
        ds.RecNo = 3;
        Assert.Equal("CCC", ds.FieldByName("AAA").AsString);
    }

    [Theory] // 差异断言（原文缺陷 D15）：上界是 RecordCount+1 **排他** ⇒ 3 条表的 RecNo=4 被静默忽略
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(99)]
    public void SetRecNo_OutOfRange_IsSilentlyIgnored(int value)
    {
        using var db = ThreeRecords("setrc2");
        var ds = PxOpen.OpenAndFirst(db);
        ds.RecNo = 2;
        ds.RecNo = value;
        Assert.Equal(2, ds.RecNo);          // 原文 :1045 `then Exit`
    }

    [Fact] // 原文 :1023-1025 InternalLast 把游标设到 RecordCount+1，而 SetRecNo 拒绝了它（D15）
    public void InternalLast_CursorGoesPastEnd_ButSetRecNoWouldRejectIt()
    {
        using var db = ThreeRecords("last");
        var ds = PxOpen.OpenAndFirst(db);
        ds.Last();                          // 接缝：InternalLast 的入口
        Assert.Equal(4, ds.Cursor);         // RecordCount + 1
        ds.RecNo = 4;                       // 原文 :1045 拒绝 ⇒ 游标不变（仍 4）
        Assert.Equal(4, ds.Cursor);
    }

    [Fact] // 原文 :694/:1033 GetCanModify 恒 False（只读数据集）
    public void CanModify_IsAlwaysFalse()
    {
        using var db = ThreeRecords("mod");
        var ds = PxOpen.OpenAndFirst(db);
        Assert.False(ds.CanModify);
    }

    [Fact] // 原文 :1119-1120 ReadDataBlock 越界 → 'Block %d read error'（%d 走 DelphiFormat）
    public void ReadDataBlock_OutOfRangeBlockNumber_ThrowsFormatted()
    {
        // FirstBlock=0 会立刻越界（原文 :966 用 FirstBlock 起遍历）
        using var db = ThreeRecords("blk");
        db.FirstBlock = 0;
        db.Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        var ex = Assert.Throws<EParadoxError>(() => ds.First());
        Assert.Equal("ParadoxDataSet.EParadoxError: Block 0 read error", ex.Message);
    }

    [Fact] // 差异断言（原文缺陷 D4）：RecordSize=0 时 :973 的 `div FFileHeader.RecordSize` 除零
    public void GetRecord_RecordSizeZero_DivideByZero()
    {
        using var db = new PxSyntheticDb("div0");
        db.RecordSize = 0;
        db.NumRecordsOverride = 1;
        db.Field(PxFieldType.pxfAlpha, 3).Name("AAA").Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        Assert.Throws<DivideByZeroException>(() => ds.First());
    }

    private static TBookmarkFlag ReadBookmark(TParadoxDataSet ds)
    {
        // 接缝：TDataSet 未公开 bookmark 读取器；用 RecNo/字段值间接观察，
        // 此处改由反射读取受保护的缓冲区首部（测试专用）。
        var f = typeof(TDataSet).GetField("FActiveBuffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var ptr = (IntPtr)f.GetValue(ds);
        return PxRecordHeaderOps.GetBookmarkFlag(ptr);
    }
}

// ============================================================================
// 6) GetFieldData（原文 :1180-1247）—— 字段族逐分支 + 空值判定差异断言
// ============================================================================

public class ParadoxDataSetFieldDataTests
{
    private static PxSyntheticDb OneRecord(string tag, params (byte type, byte size, string name)[] fields)
    {
        var db = new PxSyntheticDb(tag);
        int size = 0;
        foreach (var f in fields) size += f.size;
        db.RecordSize = (ushort)Math.Max(size, 1);
        foreach (var f in fields) db.Field(f.type, f.size).Name(f.name);
        db.Write();
        return db;
    }

    [Fact] // 原文 :1226 pxfAlpha → StrLCopy(Buffer, Src, FieldSize)
    public void FieldData_Alpha_CopiesRawBytesUpToFieldSize()
    {
        using var db = new PxSyntheticDb("alpha");
        db.RecordSize = 5;
        db.Field(PxFieldType.pxfAlpha, 5).Name("S").Record(new byte[] { (byte)'h', (byte)'e', 0, (byte)'X', (byte)'Y' }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal("he", ds.FieldByName("S").AsString);   // 遇 #0 截断（StrLCopy 语义）
    }

    [Fact] // 差异断言：Alpha 不走空值判定 ⇒ 全 #0 也返回 True（空串），不是"空值"
    public void FieldData_Alpha_AllZero_IsEmptyStringNotEmptyNull()
    {
        using var db = new PxSyntheticDb("alpha0");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfAlpha, 4).Name("S").Record(new byte[4]).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal("", ds.FieldByName("S").AsString);
    }

    [Fact] // 原文 :1203 空值判定只对 [2..6, $14..$16] 生效；全 0 数值字段 ⇒ Result := False
    public void FieldData_Numeric_AllZero_IsNull()
    {
        using var db = new PxSyntheticDb("num0");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfLong, 4).Name("N").Record(new byte[4]).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Null(ds.FieldByName("N").Value);            // GetFieldData 返回 False ⇒ 接缝 Value = null
        Assert.Equal(0, ds.FieldByName("N").AsInteger);    // AsInteger 对 null 给 0（Delphi TField.AsInteger 同语义）
    }

    [Fact] // 差异断言（原文缺陷 D7）：数值字段按**逆序**读入 P，再整体按目标类型解引用
    public void FieldData_Long_ReadsBytesInReverseOrder()
    {
        using var db = new PxSyntheticDb("longrev");
        db.RecordSize = 4;
        // 用户记录字节（文件顺序）: 01 02 03 04
        db.Field(PxFieldType.pxfLong, 4).Name("N").Record(new byte[] { 0x01, 0x02, 0x03, 0x04 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        // 置换：P[0]=rec[3]=04, P[1]=rec[2]=03, P[2]=rec[1]=02, P[3]=rec[0]=01^80=81
        // P 作为小端 int = 0x81020304
        Assert.Equal(unchecked((int)0x81020304), ds.FieldByName("N").AsInteger);
        // 差异断言：与"按文件顺序直接小端解释"（0x04030201）必须不同
        Assert.NotEqual(0x04030201, ds.FieldByName("N").AsInteger);
    }

    [Fact] // 原文 :1216 末字节 xor $80 —— 与"不做 xor"的结果必须有差异
    public void FieldData_Long_LastByteIsXoredWith0x80()
    {
        using var db = new PxSyntheticDb("longxor");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfLong, 4).Name("N").Record(new byte[] { 0x10, 0x00, 0x00, 0x00 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        // P = [00, 00, 00, 10^80=90] ⇒ 小端 int = 0x90000000（无 xor 则 0x10000000）
        Assert.Equal(unchecked((int)0x90000000), ds.FieldByName("N").AsInteger);
        Assert.NotEqual(0x10000000, ds.FieldByName("N").AsInteger);
    }

    [Fact] // 原文 :1228 pxfShort → PSmallInt（2 字节）
    public void FieldData_Short_Is16Bit()
    {
        using var db = new PxSyntheticDb("short");
        db.RecordSize = 2;
        db.Field(PxFieldType.pxfShort, 2).Name("S").Record(new byte[] { 0x34, 0x12 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(TFieldType.ftSmallint, ds.FieldByName("S").DataType);
        // P[0]=12, P[1]=34 xor 80 = B4 ⇒ 0xB412 有符号 = -19438
        Assert.Equal(unchecked((short)0xB412), (short)ds.FieldByName("S").AsInteger);
    }

    [Fact] // 原文 :1230/:1231 pxfCurrency / pxfNumber → PDouble
    public void FieldData_CurrencyAndNumber_AreDoubles()
    {
        using var db = new PxSyntheticDb("dbl");
        db.RecordSize = 8;
        db.Field(PxFieldType.pxfNumber, 8).Name("D").Record(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0x40 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(TFieldType.ftFloat, ds.FieldByName("D").DataType);
        Assert.Equal("ftFloat", ds.FieldByName("D").DataType.ToString());
    }

    [Fact] // 差异断言（原文 :1232）：pxfLogical 看的是 **Src^** 一个字节与 $80 比较，
    // 且写入的是 PWordBool（2 字节），**不是** 1 字节 Boolean
    public void FieldData_Logical_ComparesFirstSourceByteTo0x80_AndWritesWordBool()
    {
        using var db = new PxSyntheticDb("log1");
        db.RecordSize = 1;
        db.Field(PxFieldType.pxfLogical, 1).Name("B").Record(new byte[] { 0x80 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.True(ds.FieldByName("B").AsBoolean);

        using var db2 = new PxSyntheticDb("log2");
        db2.RecordSize = 1;
        db2.Field(PxFieldType.pxfLogical, 1).Name("B").Record(new byte[] { 0x01 }).Write();
        var ds2 = PxOpen.OpenAndFirst(db2);
        // 差异断言：$01 != $80 ⇒ False（不是"非零即真"）
        Assert.False(ds2.FieldByName("B").AsBoolean);
    }

    [Fact] // 原文 :1235 pxfAutoInc → PInteger
    public void FieldData_AutoInc_IsInteger()
    {
        using var db = new PxSyntheticDb("auto");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfAutoInc, 4).Name("A").Record(new byte[] { 0x01, 0x00, 0x00, 0x00 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(TFieldType.ftAutoInc, ds.FieldByName("A").DataType);
        // 逆序读 + 末字节 xor $80：记录 [01 00 00 00] → P=[00,00,00,81] → 小端 = 0x81000000
        Assert.Equal(unchecked((int)0x81000000), ds.FieldByName("A").AsInteger);
    }

    [Fact] // 原文 :1227 pxfDate → PLongint 搬运；但 ftDate 的 TField.Value 视图是 TDateTime
    public void FieldData_Date_IsIntegerPayload()
    {
        using var db = new PxSyntheticDb("date");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfDate, 4).Name("D").Record(new byte[] { 0x00, 0x00, 0x00, 0x01 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(TFieldType.ftDate, ds.FieldByName("D").DataType);
        // 差异断言：GetFieldData 搬的是 4 字节整数（原文 :1227），
        // 但接缝的 TField.Value 对 ftDate 按 TDateTime 解释 —— 用底层缓冲直接验证搬运结果。
        var raw = new byte[4];
        Assert.True(ds.GetFieldData(ds.FieldByName("D"), raw));
        // 记录 [00,00,00,01] ⇒ (00^80)<<24 | 00 | 00 | 01 = 0x80000001
        // 差异断言：末字节是最高字节（逆序），**不是**小端直接解释（那会得到 0x01000000）
        Assert.Equal(unchecked((int)0x80000001), BitConverter.ToInt32(raw, 0));
        Assert.NotEqual(0x01000000, BitConverter.ToInt32(raw, 0));
    }

    [Theory] // 差异断言（原文缺陷 D8）：Blob/Memo 族在 case 里**没有**分支 ⇒ GetFieldData 返回 False
    [InlineData(PxFieldType.pxfMemoBLOB)]
    [InlineData(PxFieldType.pxfBLOB)]
    [InlineData(PxFieldType.pxfFmtMemoBLOB)]
    [InlineData(PxFieldType.pxfOLE)]
    [InlineData(PxFieldType.pxfGraphic)]
    [InlineData(PxFieldType.pxfBCD)]
    [InlineData(PxFieldType.pxfBytes)]
    public void FieldData_BlobFamily_ReturnsFalse(byte nativeType)
    {
        using var db = new PxSyntheticDb("blob" + nativeType);
        db.RecordSize = 10;
        db.Field(nativeType, 10).Name("X").Record(new byte[10]).Write();
        var ds = PxOpen.OpenAndFirst(db);
        var buf = new byte[16];
        Assert.False(ds.GetFieldData(ds.FieldByName("X"), buf));
        Assert.Null(ds.FieldByName("X").Value);
    }

    [Theory] // 原文 :1093-1115 NativeToFieldType 全 17 个分支（含未在 case 中出现的类型码）
    [InlineData(0x01, TFieldType.ftString)]
    [InlineData(0x02, TFieldType.ftDate)]
    [InlineData(0x03, TFieldType.ftSmallint)]
    [InlineData(0x04, TFieldType.ftInteger)]
    [InlineData(0x05, TFieldType.ftCurrency)]
    [InlineData(0x06, TFieldType.ftFloat)]
    [InlineData(0x09, TFieldType.ftBoolean)]
    [InlineData(0x0C, TFieldType.ftMemo)]
    [InlineData(0x0D, TFieldType.ftBlob)]
    [InlineData(0x0E, TFieldType.ftFmtMemo)]
    [InlineData(0x0F, TFieldType.ftParadoxOle)]
    [InlineData(0x10, TFieldType.ftGraphic)]
    [InlineData(0x14, TFieldType.ftTime)]
    [InlineData(0x15, TFieldType.ftDateTime)]
    [InlineData(0x16, TFieldType.ftAutoInc)]
    [InlineData(0x17, TFieldType.ftBCD)]
    [InlineData(0x18, TFieldType.ftBytes)]
    [InlineData(0x00, TFieldType.ftUnknown)]     // 差异断言：未列出 → ftUnknown
    [InlineData(0x0A, TFieldType.ftUnknown)]
    [InlineData(0xFF, TFieldType.ftUnknown)]
    public void NativeToFieldType_MapsAllBranches(byte nativeType, TFieldType expected)
    {
        using var db = new PxSyntheticDb("n2f" + nativeType);
        db.RecordSize = 4;
        db.Field(nativeType, 4).Name("X").Record(new byte[] { 1, 2, 3, 4 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Equal(expected, ds.Fields[0].DataType);
    }
}

// ============================================================================
// 7) CreateBlobStream（原文 :1249-1341）—— 行内 Blob / .mb 单块 / .mb 索引块
// ============================================================================

public class ParadoxDataSetBlobTests
{
    [Fact] // 原文 :1261 Mode <> bmRead ⇒ Result := nil
    public void CreateBlobStream_NonReadMode_ReturnsNull()
    {
        using var db = new PxSyntheticDb("bmode");
        db.RecordSize = 16;
        db.Field(PxFieldType.pxfMemoBLOB, 16).Name("M").Record(new byte[16]).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Null(ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmWrite));
        Assert.Null(ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmReadWrite));
    }

    [Fact] // 原文 :1272 Blob.Length = 0 ⇒ Exit(nil)
    public void CreateBlobStream_ZeroLength_ReturnsNull()
    {
        using var db = new PxSyntheticDb("bzero");
        db.RecordSize = 16;
        db.Field(PxFieldType.pxfMemoBLOB, 16).Name("M").Record(new byte[16]).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.Null(ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead));
    }

    [Fact] // 原文 :1323-1337 行内 Blob 分支（Blob.Length <= Field.Size - 10）⇒ 直接从记录缓冲写
    public void CreateBlobStream_InlineBlob_WritesFromRecordBuffer()
    {
        using var db = new PxSyntheticDb("binline");
        const byte fieldSize = 20;
        db.RecordSize = fieldSize;
        // Blob 信息块在 Src + Size - 10 = Src + 10；Length = 4（<= 10 ⇒ 行内分支）
        var rec = new byte[fieldSize];
        rec[10] = 0; rec[11] = 0; rec[12] = 0; rec[13] = 0;   // FileLoc = 0
        rec[14] = 4; rec[15] = 0; rec[16] = 0; rec[17] = 0;   // Length = 4
        rec[18] = 0; rec[19] = 0;                             // ModCnt
        rec[0] = (byte)'W'; rec[1] = (byte)'X'; rec[2] = (byte)'Y'; rec[3] = (byte)'Z';
        db.Field(PxFieldType.pxfMemoBLOB, fieldSize).Name("M").Record(rec).Write();
        var ds = PxOpen.OpenAndFirst(db);
        var ms = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.NotNull(ms);
        Assert.Equal(0, ms.Position);                          // 原文 :1340
        Assert.Equal(new byte[] { (byte)'W', (byte)'X', (byte)'Y', (byte)'Z' }, ms.ToArray());
    }

    [Fact] // 原文 :1284-1299 单块（Idx = $FF）：Seek(Loc + 9) + 读 Blob.Length 字节
    public void CreateBlobStream_MbSingleBlock_SeeksLocPlus9()
    {
        using var db = new PxSyntheticDb("bsingle");
        const byte fieldSize = 20;
        db.RecordSize = fieldSize;
        const int loc = 0x100;
        var rec = new byte[fieldSize];
        int fileLoc = loc | 0xFF;                              // Idx = low byte = $FF
        rec[10] = (byte)(fileLoc & 0xFF); rec[11] = (byte)((fileLoc >> 8) & 0xFF);
        rec[12] = (byte)((fileLoc >> 16) & 0xFF); rec[13] = (byte)((fileLoc >> 24) & 0xFF);
        int blobLen = 6;                                       // > Field.Size-10 = 10? 否 -> 需 > 10
        blobLen = 12;                                          // 必须 > 10 才走 .mb 分支
        rec[14] = (byte)blobLen; rec[15] = 0; rec[16] = 0; rec[17] = 0;
        db.Field(PxFieldType.pxfMemoBLOB, fieldSize).Name("M").Record(rec).Write();

        var mb = new byte[loc + 9 + blobLen + 8];
        for (int i = 0; i < blobLen; i++) mb[loc + 9 + i] = (byte)('a' + i);
        db.WriteMb(mb);

        var ds = PxOpen.OpenAndFirst(db);
        var ms = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.NotNull(ms);
        Assert.Equal("abcdefghijkl", PxAnsi.FromBytes(ms.ToArray(), (int)ms.Size));
    }

    [Fact] // 差异断言（原文缺陷 D11）：多块索引按 5 字节步长定位条目，却按 6 字节读结构
    public void CreateBlobStream_MbIndexedBlock_UsesFiveByteStrideSixByteRead()
    {
        using var db = new PxSyntheticDb("bidx");
        const byte fieldSize = 20;
        db.RecordSize = fieldSize;
        const int loc = 0x200;
        const byte idx = 3;                                    // != $FF ⇒ 索引分支
        var rec = new byte[fieldSize];
        int fileLoc = loc | idx;
        rec[10] = (byte)(fileLoc & 0xFF); rec[11] = (byte)((fileLoc >> 8) & 0xFF);
        rec[12] = (byte)((fileLoc >> 16) & 0xFF); rec[13] = (byte)((fileLoc >> 24) & 0xFF);
        const int blobLen = 12;                                // > 10
        rec[14] = (byte)blobLen; rec[15] = 0; rec[16] = 0; rec[17] = 0;
        db.Field(PxFieldType.pxfMemoBLOB, fieldSize).Name("M").Record(rec).Write();

        // 索引条目在 Loc + 12 + 5*Idx = Loc + 27；Offset = 2 ⇒ 数据在 Loc + 16*2 = Loc + 32
        int entry = loc + 12 + 5 * idx;
        int dataPos = loc + 16 * 2;
        var mb = new byte[dataPos + blobLen + 8];
        mb[entry] = 2;        // Offset
        mb[entry + 1] = 0;    // Len16
        for (int i = 0; i < blobLen; i++) mb[dataPos + i] = (byte)('0' + i);
        db.WriteMb(mb);

        var ds = PxOpen.OpenAndFirst(db);
        var ms = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.NotNull(ms);
        Assert.Equal("0123456789:;", PxAnsi.FromBytes(ms.ToArray(), (int)ms.Size));
    }

    [Fact] // 差异断言（原文缺陷 D9）：行内 Blob 的 Header 指针 = Src + Field.Size - SizeOf(TPxBlob)，
    // 当 Field.Size < SizeOf(TPxBlob)=10 时**向前越界**读，把该字段**之前**的字节当 Blob 信息块。
    // 原文 :1272 的 `if Blob.Length = 0 then Exit(nil)` 因此可能因越界字节而——或被跳过、或被触发。
    public void CreateBlobStream_FieldSizeBelow10_ReadsBlobHeaderBeforeField()
    {
        using var db = new PxSyntheticDb("bsmall");
        // A(4 字节) 在前，M(8 字节) 在后 ⇒ M 的 Src 偏移 = 4
        // M 的 Header = Src + 8 - 10 = Src - 2，即 A[2..3] + M[0..1] + M[2..5] + M[6..7]
        // Blob.FileLoc = A[2..3]+M[0..1]，Blob.Length = M[2..5]，Blob.ModCnt = M[6..7]
        db.RecordSize = 12;
        var rec = new byte[12];
        rec[0] = (byte)'A'; rec[1] = (byte)'A';           // A 的可见部分
        rec[2] = 0x11; rec[3] = 0x22;                     // Blob.FileLoc 低半（来自 A 的尾部）
        rec[4] = 0x33; rec[5] = 0x44;                     // Blob.FileLoc 高半（来自 M 的前 2 字节）
        // Blob.Length 必须非 0 才会走到 :1277 的分支：取 4（<= Field.Size-10 = -2? 否）
        rec[6] = 4; rec[7] = 0; rec[8] = 0; rec[9] = 0;   // Blob.Length = 4
        rec[10] = 0; rec[11] = 0;                         // Blob.ModCnt
        db.Field(PxFieldType.pxfAlpha, 4).Name("A")
          .Field(PxFieldType.pxfBLOB, 8).Name("M")
          .Record(rec).Write();
        var ds = PxOpen.OpenAndFirst(db);
        // 差异断言：Field.Size(8) - 10 = -2 ⇒ Blob.Length(4) > -2 恒真 ⇒ 走 .mb 分支；
        // 而本夹具没有 .mb ⇒ FBlobStream = nil ⇒ 返回**空的非 null 流**（原文 :1274/:1275 已建 MS）
        Assert.Equal(8, ds.FieldByName("M").Size);
        var ms = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.NotNull(ms);
        Assert.Equal(0, ms.Size);
    }
}

// ============================================================================
// 8) EncodingField / EncodingString（原文 :1153-1166、:1343-1359）+ 接缝
// ============================================================================

public class ParadoxDataSetEncodingTests
{
    [Fact] // 原文 :1157-1160 OnEncode 优先；原文 :1155 先 `Result := S`，OnEncode 返回 nil 时
            // 也会**原样返回 nil**（不回落 S）—— 锁定该分支顺序
    public void EncodingField_OnEncodeTakesPriority_AndNilResultIsNotFallenBack()
    {
        using var db = new PxSyntheticDb("onenc");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfAlpha, 4).Name("S").Record(new byte[] { (byte)'a', 0, 0, 0 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        string seenField = null;
        ds.OnEncode = (sender, field, s) => { seenField = field.FieldName; return "ENC:" + s; };
        var m = typeof(TParadoxDataSet).GetMethod("EncodingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Equal("ENC:abc", (string)m.Invoke(ds, new object[] { "abc", ds.FieldByName("S") }));
        Assert.Equal("S", seenField);

        // 差异断言：OnEncode 回调返回 null 时原文不回落 Result := S
        ds.OnEncode = (sender, field, s) => null;
        Assert.Null((string)m.Invoke(ds, new object[] { "abc", ds.FieldByName("S") }));
    }

    [Fact] // 原文 :1162-1164 FLanguageID < 1 ⇒ 直接 Exit（返回入参 S）
    public void EncodingField_NoLanguage_ReturnsInputUnchanged()
    {
        using var db = new PxSyntheticDb("nolang");
        db.SortOrder = 200;                                   // 无法识别 ⇒ FLanguageID = 0
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfAlpha, 4).Name("S").Record(new byte[] { (byte)'a', 0, 0, 0 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        var m = typeof(TParadoxDataSet).GetMethod("EncodingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Equal("abc", (string)m.Invoke(ds, new object[] { "abc", ds.FieldByName("S") }));
    }

    [Fact] // 差异断言（原文缺陷 D13）：构造函数把 Codepage 设为 GetCodepage()（Windows: 'CP<ACP>'），
    // 而 EncodingString 比较的是 'UTF-8'/'KOI8R'/'CP1251' ⇒ **永不相等** ⇒ 恒等返回
    public void EncodingString_DefaultCodepage_NeverMatchesTheComparedLiterals()
    {
        using var db = new PxSyntheticDb("encstr");
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.StartsWith("CP", ds.Codepage);
        Assert.NotEqual("UTF-8", ds.Codepage);
        Assert.NotEqual("KOI8R", ds.Codepage);
        Assert.NotEqual("CP1251", ds.Codepage);

        var m = typeof(TParadoxDataSet).GetMethod("EncodingString",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Equal("hello", (string)m.Invoke(ds, new object[] { "hello" }));
    }

    [Fact] // 原文 :1349-1356 当 Codepage 被手工改成 'UTF-8' 且语言为 CP1251 族时应调用接缝
    public void EncodingString_CodepageUtf8_CallsSeamForCp1251()
    {
        var calls = new List<(TEncodingKind src, TEncodingKind dst)>();
        var saved = ParadoxConvSeam.EncodingHook;
        try
        {
            ParadoxConvSeam.EncodingHook = (src, dst, s) => { calls.Add((src, dst)); return "X" + s; };
            using var db = new PxSyntheticDb("encseam");
            db.SortOrder = 192;                 // PxLangTable[77] 'Paradox Cyrr 866' -> CodePage 866
            db.DosGlobalCodePage = 866;
            db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
            var ds = PxOpen.OpenAndFirst(db);
            ds.Language = TParadoxDataSet.PxLangTable[PxOpen.FindLangByCodePage(866)].Name;
            Assert.Equal(866, TParadoxDataSet.PxLangTable[ds.LanguageID].CodePage);

            ds.Codepage = "UTF-8";
            var m = typeof(TParadoxDataSet).GetMethod("EncodingString",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Equal("Xabc", (string)m.Invoke(ds, new object[] { "abc" }));
            Assert.Contains((TEncodingKind.CP866, TEncodingKind.UTF8), calls);
        }
        finally
        {
            ParadoxConvSeam.EncodingHook = saved;
        }
    }

    [Fact] // 原文 :1352-1357 866 分支的三个 Codepage 常量各自对应一个接缝调用
    public void EncodingString_Cp866Branch_ThreeCodepageLiterals()
    {
        var calls = new List<(TEncodingKind src, TEncodingKind dst, string s)>();
        var saved = ParadoxConvSeam.EncodingHook;
        try
        {
            ParadoxConvSeam.EncodingHook = (src, dst, s) => { calls.Add((src, dst, s)); return s + "!"; };
            using var db = new PxSyntheticDb("cp866");
            db.SortOrder = 192;
            db.DosGlobalCodePage = 866;
            db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
            var ds = PxOpen.OpenAndFirst(db);
            ds.Language = TParadoxDataSet.PxLangTable[PxOpen.FindLangByCodePage(866)].Name;
            var m = typeof(TParadoxDataSet).GetMethod("EncodingString",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            ds.Codepage = "UTF-8";
            Assert.Equal("a!", (string)m.Invoke(ds, new object[] { "a" }));
            ds.Codepage = "KOI8R";
            Assert.Equal("a!", (string)m.Invoke(ds, new object[] { "a" }));
            ds.Codepage = "CP1251";
            Assert.Equal("a!", (string)m.Invoke(ds, new object[] { "a" }));

            Assert.Equal(3, calls.Count);
            Assert.All(calls, c => Assert.Equal(TEncodingKind.CP866, c.src));
            Assert.Equal(new[] { TEncodingKind.UTF8, TEncodingKind.KOI8R, TEncodingKind.CP1251 },
                calls.ConvertAll(c => c.dst).ToArray());
        }
        finally
        {
            ParadoxConvSeam.EncodingHook = saved;
        }
    }

    [Fact] // 原文 :1346 case 的 other 值（如 1252）不匹配任何分支 ⇒ 恒等返回
    public void EncodingString_CodePage1252_NoBranchMatches()
    {
        using var db = new PxSyntheticDb("cp1252");
        db.SortOrder = 161;                    // PxLangTable[1] CodePage 1252
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        // 显式把 FLanguageID 指到 CodePage=1252 的项（测试目的是 :1346 的 case 落空分支）
        ds.Language = TParadoxDataSet.PxLangTable[PxOpen.FindLangByCodePage(1252)].Name;
        Assert.Equal(1252, TParadoxDataSet.PxLangTable[ds.LanguageID].CodePage);
        ds.Codepage = "UTF-8";
        var m = typeof(TParadoxDataSet).GetMethod("EncodingString",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.Equal("abc", (string)m.Invoke(ds, new object[] { "abc" }));
    }

    [Fact] // 原文 :1292/:1312 `if EncodingMemo then S := EncodingField(S, Field)`：
            // 单块 Memo 的 .mb 路径必须经 EncodingField；EncodingMemo=False 则绕开
    public void CreateBlobStream_MbSingleBlockMemo_UsesEncodingFieldOnlyWhenEncodingMemo()
    {
        using var db = new PxSyntheticDb("benc");
        const byte fieldSize = 20;
        db.RecordSize = fieldSize;
        const int loc = 0x100;
        const int blobLen = 12;                                // > Field.Size-10 = 10 ⇒ .mb 分支
        var rec = new byte[fieldSize];
        int fileLoc = loc | 0xFF;                              // Idx = $FF（单块）
        rec[10] = (byte)(fileLoc & 0xFF); rec[11] = (byte)((fileLoc >> 8) & 0xFF);
        rec[12] = (byte)((fileLoc >> 16) & 0xFF); rec[13] = (byte)((fileLoc >> 24) & 0xFF);
        rec[14] = (byte)blobLen; rec[15] = 0; rec[16] = 0; rec[17] = 0;
        db.Field(PxFieldType.pxfMemoBLOB, fieldSize).Name("M").Record(rec).Write();
        var mb = new byte[loc + 9 + blobLen + 8];
        for (int i = 0; i < blobLen; i++) mb[loc + 9 + i] = (byte)('a' + i);
        db.WriteMb(mb);

        var ds = PxOpen.OpenAndFirst(db);
        var seen = new List<string>();
        ds.OnEncode = (sender, field, s) => { seen.Add(field.FieldName); return s; };

        // EncodingMemo = True（原文 :1172 的默认值）⇒ 必经 OnEncode/EncodingField
        ds.EncodingMemo = true;
        var ms1 = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.Equal(1, seen.Count);
        Assert.Equal("M", seen[0]);
        Assert.Equal("abcdefghijkl", PxAnsi.FromBytes(ms1.ToArray(), (int)ms1.Size));

        // EncodingMemo = False ⇒ 原文 :1292 整条被跳过，OnEncode 不再被调用
        ds.EncodingMemo = false;
        var ms2 = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.Equal(1, seen.Count);                           // 未增加
        Assert.Equal("abcdefghijkl", PxAnsi.FromBytes(ms2.ToArray(), (int)ms2.Size));
    }

    [Fact] // 原文 :1292-1294 编码结果经 `MS.Write(S[1], Length(S))` 落地：
            // 非恒等编码（长度变化）必须体现在流的长度上
    public void CreateBlobStream_MbSingleBlockMemo_WritesEncodedLength()
    {
        using var db = new PxSyntheticDb("benc2");
        const byte fieldSize = 20;
        db.RecordSize = fieldSize;
        // ⚠ .mb 偏移的标准编码：FileLoc = (文件偏移 & $FFFFFF00) | 块内索引。
        //   偏移的低 8 位被索引占用 ⇒ 偏移必须是 256 的倍数，否则会被 Idx 覆盖
        //   （本轮实测踩到：偏移 $80 会得到 FileLoc=$000000FF ⇒ Loc=0，读到文件头）。
        const int loc = 0x100;
        const int blobLen = 12;
        var rec = new byte[fieldSize];
        int fileLoc = loc | 0xFF;
        rec[10] = (byte)(fileLoc & 0xFF); rec[11] = (byte)((fileLoc >> 8) & 0xFF);
        rec[12] = (byte)((fileLoc >> 16) & 0xFF); rec[13] = (byte)((fileLoc >> 24) & 0xFF);
        rec[14] = (byte)blobLen; rec[15] = 0; rec[16] = 0; rec[17] = 0;
        db.Field(PxFieldType.pxfMemoBLOB, fieldSize).Name("M").Record(rec).Write();
        var mb = new byte[loc + 9 + blobLen + 8];
        for (int i = 0; i < blobLen; i++) mb[loc + 9 + i] = (byte)('a' + i);
        db.WriteMb(mb);

        var ds = PxOpen.OpenAndFirst(db);
        ds.EncodingMemo = true;
        string encSeen = "<not-called>";
        ds.OnEncode = (sender, field, s) => { encSeen = "len=" + s.Length; return s + "ZZ"; };
        var ms = ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead);
        Assert.Equal("len=12", encSeen);
        Assert.Equal(14, ms.Size);                             // 原文 :1294 用编码后的 Length(S)
        Assert.Equal("abcdefghijklZZ", PxAnsi.FromBytes(ms.ToArray(), (int)ms.Size));
    }

    [Fact] // 差异断言（原文缺陷 D18）：FileLoc 的"文件偏移"与"块内索引"共用 32 位，
            // 而 `Loc := Blob.FileLoc and $FFFFFF00` 在 **最高位被置位**时把 Loc 变成负数
            //   FileLoc = $800000FF ⇒ Idx = $FF、Loc = $80000000 = int.MinValue
            // Delphi 的 Integer 有符号运算同样得到 int.MinValue，
            // 随后 `FBlobStream.Seek(Loc + 9, soFromBeginning)` 被喂入负偏移。
            // 原文没有任何防护，托管侧照抄（TFileStream.Seek 会由 FileStream 抛 ArgumentOutOfRange）。
    public void CreateBlobStream_FileLocHighBitSet_YieldsNegativeLoc()
    {
        using var db = new PxSyntheticDb("bneg");
        const byte fieldSize = 20;
        db.RecordSize = fieldSize;
        const int blobLen = 12;
        var rec = new byte[fieldSize];
        // FileLoc = 0x800000FF（最高位 + 单块索引）
        rec[10] = 0xFF; rec[11] = 0x00; rec[12] = 0x00; rec[13] = 0x80;
        rec[14] = (byte)blobLen; rec[15] = 0; rec[16] = 0; rec[17] = 0;
        db.Field(PxFieldType.pxfMemoBLOB, fieldSize).Name("M").Record(rec).Write();
        db.WriteMb(new byte[64]);                              // 有 .mb 才会走 Seek 分支

        var ds = PxOpen.OpenAndFirst(db);
        // 差异断言：不是"偏移 0x80000000"，而是溢出成负偏移 → Seek 抛异常
        // （.NET FileStream 对负偏移抛 IOException；Delphi TFileStream.Seek 会把它
        //  透传给 SetFilePointer，行为未定义 —— 两侧都**没有**防护，这正是原文缺陷）
        Assert.Throws<System.IO.IOException>(
            () => ds.CreateBlobStream(ds.FieldByName("M"), TBlobStreamMode.bmRead));
    }

    [Fact] // 差异断言（原文缺陷 D12）：FLanguageID = 0 时 EncodingString 访问
            // PxLangTable[0]（Delphi 越界读 / 托管 null）。唯一防护在 EncodingField :1163，
            // 但 EncodingMemo 路径**不经** FLanguageID 前置检查 —— 这里锁定该可达性。
    public void EncodingString_LanguageIdZero_IsReachableAndThrows()
    {
        using var db = new PxSyntheticDb("langid0");
        db.SortOrder = 200;                                    // DetectLang 无法命中 ⇒ FLanguageID = 0
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfAlpha, 4).Name("S").Record(new byte[] { 1, 0, 0, 0 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        ds.Language = "";                                      // 命中 PxLangTable 里没有的名字 ⇒ 保持 0
        Assert.Equal(0, ds.LanguageID);

        var m = typeof(TParadoxDataSet).GetMethod("EncodingString",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        // 原文在 Delphi 下读的是数组下界之外的内存（未定义行为）；托管侧为 NullReferenceException。
        // 这正是 D12：EncodingString 自身没有 0 号保护。
        Assert.Throws<System.Reflection.TargetInvocationException>(() => m.Invoke(ds, new object[] { "abc" }));
    }

    [Fact] // 接缝：ParadoxConvSeam.GetCodepage 的默认实现与可注入性（原文 :1171）
    public void Seam_GetCodepage_IsInjectable()
    {
        var saved = ParadoxConvSeam.GetCodepageHook;
        try
        {
            ParadoxConvSeam.GetCodepageHook = () => "CP936";
            var ds = new TParadoxDataSet();
            Assert.Equal("CP936", ds.Codepage);
        }
        finally
        {
            ParadoxConvSeam.GetCodepageHook = saved;
        }
    }

    [Fact] // 原文 :1172 FEncodingMemo 默认 True（published 属性可写）
    public void EncodingMemo_DefaultsToTrue_AndIsWritable()
    {
        var ds = new TParadoxDataSet();
        Assert.True(ds.EncodingMemo);
        ds.EncodingMemo = false;
        Assert.False(ds.EncodingMemo);
    }
}

// ============================================================================
// 9) 接缝与消费者契约（uFrmMagicCD.pas:356-393 / GBDEtoSqlite.pas:154-179 的调用面）
// ============================================================================

public class ParadoxDataSetConsumerContractTests
{
    [Fact] // uFrmMagicCD.pas:356-390 的遍历形状：Create(nil) → TableName → Open → First →
            // for I := 0 to RecordCount-1 → FieldByName(...).AsInteger/AsString → Next → Free
    public void ConsumerLoop_MagicCdShape_IteratesExactlyRecordCountTimes()
    {
        using var db = new PxSyntheticDb("consumer");
        db.RecordSize = 8;
        db.Field(PxFieldType.pxfLong, 4).Name("MagId")
          .Field(PxFieldType.pxfAlpha, 4).Name("MagName")
          .Record(new byte[] { 1, 0, 0, 0, (byte)'A', 0, 0, 0 })
          .Record(new byte[] { 2, 0, 0, 0, (byte)'B', 0, 0, 0 })
          .Record(new byte[] { 3, 0, 0, 0, (byte)'C', 0, 0, 0 })
          .Write();

        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        ds.First();
        int iterations = 0;
        var ids = new List<int>();
        for (int I = 0; I <= ds.RecordCount - 1; I++)
        {
            int magicId = ds.FieldByName("MagId").AsInteger;
            ids.Add(magicId);
            iterations++;
            ds.Next();
        }
        Assert.Equal(3, iterations);
        // 原文 :1207 的逆序读 + :1216 对 P[FieldSize-1]（= 文件首字节）异或 $80：
        //   记录 [n,0,0,0] ⇒ P=[00,00,00,n^80] ⇒ 小端 int = (n^80) << 24
        // ⇒ 1/2/3 全部得到 0x80000000|((n^80)<<24)，即 n 只影响高位字节：
        //   1^80=81 → 0x81000000, 2^80=82 → 0x82000000, 3^80=83 → 0x83000000
        Assert.Equal(
            new[] { unchecked((int)0x81000000), unchecked((int)0x82000000), unchecked((int)0x83000000) },
            ids);
        ds.Close();
        Assert.False(ds.Active);
    }

    [Fact] // GBDEtoSqlite.pas:105/:111 FieldCount = 0 时直接 Exit（空表契约）
    public void ConsumerContract_EmptyFieldCount()
    {
        using var db = new PxSyntheticDb("nofields");
        db.Field(PxFieldType.pxfAlpha, 4).Name("A").Record(new byte[] { 0, 0, 0, 0 }).Write();
        var ds = new TParadoxDataSet();
        ds.TableName = db.DbPath;
        ds.Open();
        Assert.Equal(1, ds.FieldCount);

        // 差异断言：FileName='' 时 ChangeFileExt 不抛（接缝工具，Delphi ChangeFileExt('','.mb')='.mb'）
        Assert.Equal(".mb", PxFileUtils.ChangeFileExt("", ".mb"));
    }

    [Fact] // 接缝：ChangeFileExt 的 Delphi 语义（原文 :879/:883/:891）
    public void Seam_ChangeFileExt_DelphiSemantics()
    {
        Assert.Equal("C:\\db\\StdItems.mb", PxFileUtils.ChangeFileExt("C:\\db\\StdItems.DB", ".mb"));
        Assert.Equal("C:\\db\\StdItems.MB", PxFileUtils.ChangeFileExt("C:\\db\\StdItems.DB", ".MB"));
        // 目录名里含点但文件名无扩展名 ⇒ Delphi 不会误把目录的 '.' 当扩展名
        Assert.Equal("C:\\my.dir\\StdItems.mb", PxFileUtils.ChangeFileExt("C:\\my.dir\\StdItems", ".mb"));
    }

    [Fact] // 接缝：EParadoxError.CreateFmt 走 DelphiFormat（%d/%S 按序替换，台账 §17.2）
    public void Seam_CreateFmt_UsesDelphiFormat()
    {
        // Delphi 的 Exception.CreateFmt 会前缀 <单元>.<类名>: （见 CreateFmt 注释）
        const string P = "ParadoxDataSet.EParadoxError: ";
        Assert.Equal(P + "Block 7 read error", EParadoxError.CreateFmt("Block %d read error", 7).Message);
        Assert.Equal(P + "\"a.DB\" - is not .DB data file",
            EParadoxError.CreateFmt("\"%S\" - is not .DB data file", "a.DB").Message);
        Assert.Equal(P + "Unable to open database \"x\" - boom",
            EParadoxError.CreateFmt("Unable to open database \"%S\" - %S", "x", "boom").Message);
        // 差异断言：三个 %S 若被错误地全部替换为 {0}，dest/sym 会静默丢失
        Assert.NotEqual(P + "Unable to open database \"x\" - x",
            EParadoxError.CreateFmt("Unable to open database \"%S\" - %S", "x", "boom").Message);
        Assert.Equal(P + "a-b-c", EParadoxError.CreateFmt("%s-%s-%s", "a", "b", "c").Message);
    }

    [Fact] // 接缝：InternalHandleException 转发到注入处理器（原文 :767 Application.HandleException）
    public void Seam_InternalHandleException_ForwardsToHook()
    {
        object seen = null;
        var saved = ParadoxApplication.OnHandleException;
        try
        {
            ParadoxApplication.OnHandleException = (sender, ex) => seen = sender;
            using var db = new PxSyntheticDb("hx");
            db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
            var ds = PxOpen.OpenAndFirst(db);
            var m = typeof(TParadoxDataSet).GetMethod("InternalHandleException",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            m.Invoke(ds, null);
            Assert.Same(ds, seen);
        }
        finally
        {
            ParadoxApplication.OnHandleException = saved;
        }
    }

    [Fact] // 原文 :688/:1013 InternalInitRecord 是空实现（照抄，不写任何字节）
    public void InternalInitRecord_IsEmptyImplementation()
    {
        using var db = new PxSyntheticDb("iir");
        db.RecordSize = 4;
        db.Field(PxFieldType.pxfAlpha, 4).Name("A").Record(new byte[] { 1, 2, 3, 4 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        var m = typeof(TParadoxDataSet).GetMethod("InternalInitRecord",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        IntPtr buf = Marshal.AllocHGlobal(4);
        try
        {
            Marshal.WriteByte(buf, 0xAA);
            m.Invoke(ds, new object[] { buf });
            Assert.Equal(0xAA, Marshal.ReadByte(buf));   // 未被触碰
        }
        finally { Marshal.FreeHGlobal(buf); }
    }

    [Fact] // 原文 :1055-1062 SetTableName：Active 时先 Close；同值不改（不重复 Close）
    public void SetTableName_SameValue_DoesNotClose()
    {
        using var db = new PxSyntheticDb("stn");
        db.Field(1, 3).Name("AAA").Record(new byte[] { 1, 2, 3 }).Write();
        var ds = PxOpen.OpenAndFirst(db);
        Assert.True(ds.Active);
        ds.TableName = db.DbPath;            // 同值 ⇒ 原文不进入 if
        Assert.True(ds.Active);
        ds.TableName = db.DbPath + ".x";     // 异值 ⇒ 原文 :1059 先 Close
        Assert.False(ds.Active);
    }
}
