// ============================================================================
// 源单元：Source\RunGate\ParadoxDataSet.pas（GBK，1362 行）
//   记录布局部分：行 117-136（TFieldInfoRecord/TDataBlock）、行 425-517
//                 （TPxFileHeader 425-470 / TPxDataHeader 472-491 /
//                   TPxRecordHeader 493-497 / TPxBlob 499-503 /
//                   TPxBlobIdx 505-510 / TPxLang 512-517）
//   字段类型常量：行 97-115（pxfAlpha..pxfBytes）
// 本文件 = 记录布局 + 常量的 1:1 移植（纯布局，无逻辑）。
//
// 两个副本的裁定：Source\RunGate\ParadoxDataSet.pas 与
// Source\GameCenter\ParadoxDataSet.pas 归一化行尾后**逐字节相同**
// （各 62,954 字节 / SHA256 904B84DF8B40A1BF6211DF609D025540EFCEF57441D7B2C0D6530871B51D136F，
//  LF=CR=1362）。故只移植一份，落在 GXX.Core（RunGate 与 GameCenter 都引用 GXX.Core）。
//
// 保真要点（照抄原文，含原文缺陷）：
//  1) 全部 packed record → [StructLayout(LayoutKind.Sequential, Pack = 1)]；
//     固定数组（array[$001E..$0020] of Byte 等）→ C# fixed buffer，
//     用 Marshal.SizeOf/偏移断言锁死二进制布局（见 ParadoxDataSetRecordTests）。
//  2) 原文把指针字段声明成 Integer（PrimaryIndexWorkspace/UnknownPtr1A/
//     TableNamePtrPtr/FieldInfoPtr/CryptInfoStartPtr/CryptInfoEndPtr，注释里的
//     pointer/^pAnsichar 被注释掉）→ 托管侧同样用 int（原文从不解引用它们）。
//  3) TPxBlob 的 3 个字段共用标识符 "Length" 与 List<T>.Length 无关，
//     照抄字段名 Length（不要改成 Len）。TPxBlobIdx 另有 Len16/Len。
//  4) TPxLang.Name/SortOrderID 是 Delphi 短串 string[20]/string[8]
//     → 托管侧保留为 string + 容量常量（见 PxLangNameCapacity/PxLangSortOrderIdCapacity）。
// ============================================================================

using System.Runtime.InteropServices;

namespace GXX.Core.Paradox;

/// <summary>ParadoxDataSet.pas:97-115 Paradox 字段类型码（原文 const 区）。</summary>
public static class PxFieldType
{
    /// <summary>原文行 99：pxfAlpha = $01。</summary>
    public const byte pxfAlpha = 0x01;

    /// <summary>原文行 100：pxfDate = $02。</summary>
    public const byte pxfDate = 0x02;

    /// <summary>原文行 101：pxfShort = $03。</summary>
    public const byte pxfShort = 0x03;

    /// <summary>原文行 102：pxfLong = $04。</summary>
    public const byte pxfLong = 0x04;

    /// <summary>原文行 103：pxfCurrency = $05。</summary>
    public const byte pxfCurrency = 0x05;

    /// <summary>原文行 104：pxfNumber = $06。</summary>
    public const byte pxfNumber = 0x06;

    /// <summary>原文行 105：pxfLogical = $09。</summary>
    public const byte pxfLogical = 0x09;

    /// <summary>原文行 106：pxfMemoBLOB = $0C。</summary>
    public const byte pxfMemoBLOB = 0x0C;

    /// <summary>原文行 107：pxfBLOB = $0D。</summary>
    public const byte pxfBLOB = 0x0D;

    /// <summary>原文行 108：pxfFmtMemoBLOB = $0E。</summary>
    public const byte pxfFmtMemoBLOB = 0x0E;

    /// <summary>原文行 109：pxfOLE = $0F。</summary>
    public const byte pxfOLE = 0x0F;

    /// <summary>原文行 110：pxfGraphic = $10。</summary>
    public const byte pxfGraphic = 0x10;

    /// <summary>原文行 111：pxfTime = $14。</summary>
    public const byte pxfTime = 0x14;

    /// <summary>原文行 112：pxfTimestamp = $15。</summary>
    public const byte pxfTimestamp = 0x15;

    /// <summary>原文行 113：pxfAutoInc = $16。</summary>
    public const byte pxfAutoInc = 0x16;

    /// <summary>原文行 114：pxfBCD = $17。</summary>
    public const byte pxfBCD = 0x17;

    /// <summary>原文行 115：pxfBytes = $18。</summary>
    public const byte pxfBytes = 0x18;
}

/// <summary>ParadoxDataSet.pas:126-129 PFieldInfoRecord/^TFieldInfoRecord（2 字节）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TFieldInfoRecord
{
    /// <summary>原文行 127：FieldType: Byte。</summary>
    public byte FieldType;

    /// <summary>原文行 128：FieldSize: Byte。</summary>
    public byte FieldSize;
}

/// <summary>ParadoxDataSet.pas:132-136 PDataBlock（6 字节，数据块头）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TDataBlock
{
    /// <summary>原文行 133：NextBlock: Word。</summary>
    public ushort NextBlock;

    /// <summary>原文行 134：BlockNumber: Word。</summary>
    public ushort BlockNumber;

    /// <summary>原文行 135：AddDataSize: Word。</summary>
    public ushort AddDataSize;
}

/// <summary>
/// ParadoxDataSet.pas:428-470 PPxFileHeader（88 字节 = $58）。
/// 原文行 439-469 的字段顺序即二进制顺序；行 139-423 是原作者的偏移说明注释
/// （表头注释，纯文档，不产生代码）。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TPxFileHeader
{
    /// <summary>原文行 429：RecordSize: Word（偏移 $00）。</summary>
    public ushort RecordSize;

    /// <summary>原文行 430：HeaderSize: Word（$02，恒 $0800）。</summary>
    public ushort HeaderSize;

    /// <summary>原文行 431：FileType: Byte（$04；0=索引化 .DB、2=无索引 .DB）。</summary>
    public byte FileType;

    /// <summary>原文行 432：MaxTableSize: Byte（$05；块大小 = 值 × $0400）。</summary>
    public byte MaxTableSize;

    /// <summary>原文行 433：NumRecords: Integer（$06）。</summary>
    public int NumRecords;

    /// <summary>原文行 434：NextBlock: Word（$0A）。</summary>
    public ushort NextBlock;

    /// <summary>原文行 435：FileBlocks: Word（$0C）。</summary>
    public ushort FileBlocks;

    /// <summary>原文行 436：FirstBlock: Word（$0E；空表时为 0）。</summary>
    public ushort FirstBlock;

    /// <summary>原文行 437：LastBlock: Word（$10）。</summary>
    public ushort LastBlock;

    /// <summary>原文行 438：Unknown12x13: Word（$12）。</summary>
    public ushort Unknown12x13;

    /// <summary>原文行 439：ModifiedFlags1: Byte（$14）。</summary>
    public byte ModifiedFlags1;

    /// <summary>原文行 440：IndexFieldNumber: Byte（$15；原文此处用了一个制表符缩进）。</summary>
    public byte IndexFieldNumber;

    /// <summary>原文行 441：PrimaryIndexWorkspace: Integer（$16；原为 pointer）。</summary>
    public int PrimaryIndexWorkspace;

    /// <summary>原文行 442：UnknownPtr1A: Integer（$1A；原为 pointer）。</summary>
    public int UnknownPtr1A;

    /// <summary>原文行 443：Unknown1Ex20: array[$001E..$0020] of Byte（$1E，3 字节）。</summary>
    public fixed byte Unknown1Ex20[3];

    /// <summary>原文行 444：NumFields: Word（$21）。</summary>
    public ushort NumFields;

    /// <summary>原文行 445：PrimaryKeyFields: Word（$23）。</summary>
    public ushort PrimaryKeyFields;

    /// <summary>原文行 446：Encryption1: Integer（$25）。</summary>
    public int Encryption1;

    /// <summary>原文行 447：SortOrder: Byte（$29）。</summary>
    public byte SortOrder;

    /// <summary>原文行 448：ModifiedFlags2: Byte（$2A）。</summary>
    public byte ModifiedFlags2;

    /// <summary>原文行 449：Unknown2Bx2C: array[$002B..$002C] of Byte（$2B，2 字节）。</summary>
    public fixed byte Unknown2Bx2C[2];

    /// <summary>原文行 450：ChangeCount1: Byte（$2D）。</summary>
    public byte ChangeCount1;

    /// <summary>原文行 451：ChangeCount2: Byte（$2E）。</summary>
    public byte ChangeCount2;

    /// <summary>原文行 452：Unknown2F: Byte（$2F）。</summary>
    public byte Unknown2F;

    /// <summary>原文行 453：TableNamePtrPtr: Integer（$30；原注释 ^pAnsichar）。</summary>
    public int TableNamePtrPtr;

    /// <summary>原文行 454：FieldInfoPtr: Integer（$34；原注释 PFieldInfoRecord）。</summary>
    public int FieldInfoPtr;

    /// <summary>原文行 455：WriteProtected: Byte（$38）。</summary>
    public byte WriteProtected;

    /// <summary>原文行 456：FileVersionID: Byte（$39；$03/$04 = 3.x，$05..$09 = 4.x，$0A/$0B = 5.x，$0C = 7.x）。</summary>
    public byte FileVersionID;

    /// <summary>原文行 457：MaxBlocks: Word（$3A）。</summary>
    public ushort MaxBlocks;

    /// <summary>原文行 458：Unknown3C: Byte（$3C）。</summary>
    public byte Unknown3C;

    /// <summary>原文行 459：AuxPasswords: Byte（$3D）。</summary>
    public byte AuxPasswords;

    /// <summary>原文行 460：Unknown3Ex3F: array[$003E..$003F] of Byte（$3E，2 字节）。</summary>
    public fixed byte Unknown3Ex3F[2];

    /// <summary>原文行 461：CryptInfoStartPtr: Integer（$40；原为 pointer）。</summary>
    public int CryptInfoStartPtr;

    /// <summary>原文行 462：CryptInfoEndPtr: Integer（$42+2=... 见布局断言；原为 pointer）。</summary>
    public int CryptInfoEndPtr;

    /// <summary>原文行 463：Unknown48: Byte（$48）。</summary>
    public byte Unknown48;

    /// <summary>原文行 464：AutoInc: Integer（$49）。</summary>
    public int AutoInc;

    /// <summary>原文行 465：Unknown4Dx4E: array[$004D..$004E] of Byte（$4D，2 字节）。</summary>
    public fixed byte Unknown4Dx4E[2];

    /// <summary>原文行 466：IndexUpdateRequired: Byte（$4F）。</summary>
    public byte IndexUpdateRequired;

    /// <summary>原文行 467：Unknown50x54: array[$0050..$0054] of Byte（$50，5 字节）。</summary>
    public fixed byte Unknown50x54[5];

    /// <summary>原文行 468：RefIntegrity: Byte（$55；次索引文件此处为 inxDirection）。</summary>
    public byte RefIntegrity;

    /// <summary>原文行 469：Unknown56x57: array[$0056..$0057] of Byte（$56，2 字节）。</summary>
    public fixed byte Unknown56x57[2];
}

/// <summary>
/// ParadoxDataSet.pas:473-491 PPxDataHeader（40 字节 = $28，紧跟在 TPxFileHeader 之后）。
/// 原文 474-477 是俄文注释（版本号含义），479/481 同为注释。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TPxDataHeader
{
    /// <summary>原文行 478：FileVersionID: Word（$0105..$010C）。</summary>
    public ushort FileVersionID;

    /// <summary>原文行 480：FileVersionID2: Word（与 FileVersionID 同值）。</summary>
    public ushort FileVersionID2;

    /// <summary>原文行 482：Encryption2: Integer（0 = 未加密）。</summary>
    public int Encryption2;

    /// <summary>原文行 483：FileUpdateTime: Integer。</summary>
    public int FileUpdateTime;

    /// <summary>原文行 484：HiFieldID: Word。</summary>
    public ushort HiFieldID;

    /// <summary>原文行 485：HiFieldIDInfo: Word。</summary>
    public ushort HiFieldIDInfo;

    /// <summary>原文行 486：SometimesNumFields: Word。</summary>
    public ushort SometimesNumFields;

    /// <summary>原文行 487：DosGlobalCodePage: Word。</summary>
    public ushort DosGlobalCodePage;

    /// <summary>原文行 488：Unknown6Cx6F: array[$006C..$006F] of Byte（4 字节）。</summary>
    public fixed byte Unknown6Cx6F[4];

    /// <summary>原文行 489：ChangeCount4: Word。</summary>
    public ushort ChangeCount4;

    /// <summary>原文行 490：Unknown72x77: array[$0072..$0077] of Byte（6 字节）。</summary>
    public fixed byte Unknown72x77[6];
}

/// <summary>
/// ParadoxDataSet.pas:493-497 TPxRecordHeader（8 字节）。
/// 该记录**不在文件中**：它是托管/原文为每条记录缓冲区加的头部
/// （RecordIndex + BookmarkFlag），GetRecord 写入、GetRecNo 读取。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TPxRecordHeader
{
    /// <summary>原文行 494：RecordIndex: integer。</summary>
    public int RecordIndex;

    /// <summary>原文行 495：BookmarkFlag: TBookmarkFlag。</summary>
    public TBookmarkFlag BookmarkFlag;
}

/// <summary>
/// ParadoxDataSet.pas:499-503 TPxBlob（10 字节 = 10-Byte Blob Info Block）。
/// 注意：字段名 <c>Length</c> 与 <c>ModCnt</c> 逐字照抄。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TPxBlob
{
    /// <summary>原文行 500：FileLoc: Integer（低字节 = 块内索引，高 24 位 = .mb 文件偏移）。</summary>
    public int FileLoc;

    /// <summary>原文行 501：Length: Integer。</summary>
    public int Length;

    /// <summary>原文行 502：ModCnt: Word。</summary>
    public ushort ModCnt;
}

/// <summary>
/// ParadoxDataSet.pas:505-510 TPxBlobIdx（6 字节 = Blob Pointer Array Entry）。
/// 原文缺陷（照抄）：CreateBlobStream 第 1304 行按 <c>SizeOf(TPxBlobIdx)</c> 读 6 字节，
/// 但 .mb 里该条目实为 5 字节（原文行 1303 的 <c>5 * Idx</c> 步长可证），
/// 故 <c>ModCnt</c> 恒为文件中的下一个字节。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TPxBlobIdx
{
    /// <summary>原文行 506：Offset: Byte。</summary>
    public byte Offset;

    /// <summary>原文行 507：Len16: Byte。</summary>
    public byte Len16;

    /// <summary>原文行 508：ModCnt: Word。</summary>
    public ushort ModCnt;

    /// <summary>原文行 509：Len: Byte。</summary>
    public byte Len;
}

/// <summary>
/// ParadoxDataSet.pas:512-517 TPxLang。
/// <c>Name: string[20]</c> / <c>SortOrderID: string[8]</c> 是 Delphi **短串**
/// （长度字节 + GBK 字节）；托管侧保留为 <see cref="string"/> 并给出容量常量，
/// 原文 118 项表中 Name 最长 19 字符、SortOrderID 恰 8 字符（见布局断言测试）。
/// </summary>
public sealed class TPxLang
{
    /// <summary>原文行 513：Name: string[20] 的容量。</summary>
    public const int NameCapacity = 20;

    /// <summary>原文行 516：SortOrderID: string[8] 的容量。</summary>
    public const int SortOrderIDCapacity = 8;

    /// <summary>原文行 513：Name: string[20]。</summary>
    public string Name;

    /// <summary>原文行 514：SortOrder: Byte。</summary>
    public byte SortOrder;

    /// <summary>原文行 515：CodePage: Word。</summary>
    public ushort CodePage;

    /// <summary>原文行 516：SortOrderID: string[8]。</summary>
    public string SortOrderID;

    /// <summary>按原文 520-638 的 `(Name: ...; SortOrder: ...; CodePage: ...; SortOrderID: ...)` 顺序构造。</summary>
    public TPxLang(string Name, byte SortOrder, ushort CodePage, string SortOrderID)
    {
        this.Name = Name;
        this.SortOrder = SortOrder;
        this.CodePage = CodePage;
        this.SortOrderID = SortOrderID;
    }
}
