// ============================================================================
// 源单元：Source\RunGate\ParadoxDataSet.pas（GBK，1362 行）
//   本文件覆盖：行 640-749（TParadoxDataSet 声明 + TEncodeEvent + EParadoxError）
//               行 751-1361（implementation 全部方法）
//   同族记录布局/常量在 ParadoxDataSet.Records.cs（原文 97-136、425-517），
//   PxLangTable 在 ParadoxDataSet.Tables.g.cs（原文 520-638，脚本抽取）。
//
// 两个副本裁定：RunGate\ParadoxDataSet.pas 与 GameCenter\ParadoxDataSet.pas
// 归一化行尾后逐字节相同（62,954 字节 / SHA256 904B84DF…136F / LF=CR=1362）
// ⇒ **只移植一份**，落在 GXX.Core（两个工程都引用 GXX.Core）。
//
// 逐方法对照（原文行号 → 本文件方法）：
//   755 GetBookmarkFlag            760 SetBookmarkFlag        765 InternalHandleException
//   771 InternalInitFieldDefs      850 InternalOpen           908 IsCursorOpen
//   913 InternalClose              927 GetRecord             1003 AllocRecordBuffer
//  1008 FreeRecordBuffer          1013 InternalInitRecord    1018 InternalFirst
//  1023 InternalLast              1028 InternalSetToRecord   1033 GetCanModify
//  1038 GetRecordCount            1043 SetRecNo              1050 GetRecNo
//  1055 SetTableName              1064 GetLanguage           1072 SetLanguage
//  1093 NativeToFieldType         1117 ReadDataBlock         1125 DetectLang
//  1153 EncodingField             1168 Create                1175 Destroy
//  1180 GetFieldData              1249 CreateBlobStream      1343 EncodingString
//
// 原文缺陷/易错点（全部**照抄**，并在 ParadoxDataSetTests 里用差异断言锁定；
// 详见 docs/并行报告-p6-core-paradox.md）：
//  D1  :809-816 / :837-844 的 repeat/until 只在 B<>0 时写入 ⇒ 文件截断时**死循环**
//      （Delphi 下 FFileStream.Read(B,1) 读不到不改 B）。
//  D2  :826-835 的 P 分支：未加密时 `P := FFileHeader.NumFields * 2`（**覆盖**了 :797/:800-803
//      算出的字段名区终点），:835 再以 soFromCurrent 相对定位 ⇒ 落点取决于 NumFields，
//      与"字段名区之后"无关。
//  D3  :866 `not (FileType in [0, 2])` ⇒ **FileType=1（.PX 主索引）也会被拒**；
//      .DB 无索引表是 2。
//  D4  :969-978 的块遍历按 `AddDataSize div RecordSize` 累加 ⇒ **RecordSize=0 时除零异常**；
//      且 TotalRecords 的累加含 `+ 1`，与 :973 的 div 结果共同决定 RecordStart。
//  D5  :980-983 nSeekPos 用 `MaxTableSize * 1024`（不是 *1024*? ——原文即 1024 字节/单位），
//      并以 `FCursor - RecordStart - 1` 计条内偏移。
//  D6  :1203 空值判定只对 `[2..6, $14..$16]` 生效；:1216 对**逆序读入**的最后一个字节异或 $80。
//  D7  :1207 `P[I] := PAnsiChar(Src + FieldSize - I - 1)^`：逆序读，I=FieldSize-1 时取 Src^。
//  D8  :1225-1246 case 里**没有** pxfMemoBLOB/pxfBLOB/pxfFmtMemoBLOB/pxfOLE/pxfGraphic/
//      pxfBCD/pxfBytes 分支（原文 :1237-1243 以注释列出）⇒ 这些类型 Result := False。
//  D9  :1269 `Header := Src + Field.Size - SizeOf(TPxBlob)` 在 Field.Size < 10 时
//      **向前越界读**（读的是该字段之前的字节）。
//  D10 :1277 `if Blob.Length > Field.Size - SizeOf(TPxBlob)`：长度字段与"是否为行内
//      Blob"的判定用同一表达式，短 Blob 走 :1323 分支。
//  D11 :1303-1305 读 5 字节步长（5 * Idx）却按 `SizeOf(TPxBlobIdx)`=6 读结构 ⇒ ModCnt 被污染。
//  D12 :1346 `case PxLangTable[FLanguageID].CodePage` **无 0 号保护**：FLanguageID=0 时
//      访问 PxLangTable[0]（托管侧为 null，会 NullReferenceException；Delphi 侧读的是
//      数组下界以外的内存）。唯一调用者 EncodingField (:1163) 有 `FLanguageID < 1` 前置退出，
//      但 CreateBlobStream (:1292/:1312) 与 EncodingMemo 组合**可以**到达。
//  D13 :1349-1356 的 Codepage 常量 `'UTF-8'/'KOI8R'/'CP1251'` 与构造函数 :1171 的
//      `GetCodepage`（Windows 下为 `'CP' + GetACP`，如 'CP936'）**永不相等**
//      ⇒ Windows 上 EncodingString 恒等返回入参。
//  D14 :855 `CreateFmt('TableName is not set', [])` 无占位符（原文如此）。
//  D15 :1045 `if (Value < 1) or (Value >= RecordCount + 1)` ⇒ RecNo := RecordCount+1 被拒，
//      即 InternalLast (:1025) 造的 FCursor 无法经 SetRecNo 复现。
//  D16 :666/:1072 GetLanguage/SetLanguage 的 `PxLangTable[I].Name = Value` 是**大小写敏感**
//      的短串比较（Delphi `=`），与 :1066 的 1..118 边界检查不对称（Set 只扫 1..118 无 else 报错）。
// ============================================================================

using System;
using System.Runtime.InteropServices;

namespace GXX.Core.Paradox;

/// <summary>
/// ParadoxDataSet.pas:642 <c>TEncodeEvent = function(Sender: TObject; Field: TField; S: AnsiString): AnsiString of object</c>。
/// 托管侧 AnsiString 用 Latin-1 透明 string 表示（见 <see cref="PxAnsi"/>）。
/// </summary>
public delegate string TEncodeEvent(object Sender, TField Field, string S);

/// <summary>
/// ParadoxDataSet.pas:646-747 <c>TParadoxDataSet = class(TDataSet)</c> 1:1 移植。
/// 只读的 Paradox .DB 数据集；Blob 需同名 .mb/.MB 文件；不做索引。
/// 字段名/方法名/局部变量名/分支顺序/边界行为照抄原文。
/// </summary>
public sealed partial class TParadoxDataSet : TDataSet
{
    // ---- 原文 :648-664 私有字段（逐字保留 F 前缀与拼写） ----

    /// <summary>原文 :648 FTableName: string。</summary>
    private string FTableName;

    /// <summary>原文 :649 FFileStream: TFileStream（.DB 主文件）。</summary>
    private TFileStream FFileStream;

    /// <summary>原文 :650 FBlobStream: TFileStream（.mb/.MB Blob 文件，可为 nil）。</summary>
    private TFileStream FBlobStream;

    /// <summary>原文 :651 FIsOpen: boolean。</summary>
    private bool FIsOpen;

    /// <summary>原文 :652 FCursor: Integer（记录号，1-based；0 表示"首记录之前"）。</summary>
    private int FCursor;

    /// <summary>原文 :653 FFileHeader: TPxFileHeader。</summary>
    private TPxFileHeader FFileHeader;

    /// <summary>原文 :654 FDataHeader: TPxDataHeader。</summary>
    private TPxDataHeader FDataHeader;

    /// <summary>原文 :655 FFields: array of TFieldInfoRecord。</summary>
    private TFieldInfoRecord[] FFields;

    /// <summary>原文 :656 FFieldOffsets: array of Integer（各字段在用户记录内的字节偏移）。</summary>
    private int[] FFieldOffsets;

    /// <summary>原文 :658-659 FIsEncrypted: Boolean（注释：Показывает зашифрован ли файл）。</summary>
    private bool FIsEncrypted;

    /// <summary>原文 :660 FSortOrderID: AnsiString（从字段名区之后读到的排序规则短串）。</summary>
    private string FSortOrderID;

    /// <summary>原文 :661 FLanguageID: Integer（PxLangTable 的 1-based 下标，0 = 未识别）。</summary>
    private int FLanguageID;

    /// <summary>原文 :662 FCodepage: string（published 属性 Codepage 的后备字段）。</summary>
    private string FCodepage;

    /// <summary>原文 :663 FEncodingMemo: Boolean（published 属性 EncodingMemo 的后备字段）。</summary>
    private bool FEncodingMemo;

    /// <summary>原文 :664 FOnEncode: TEncodeEvent（published 属性 OnEncode 的后备字段）。</summary>
    private TEncodeEvent FOnEncode;

    // ---- 原文 :666-674 私有方法 ----

    /// <summary>原文 :666/:1064 GetLanguage。</summary>
    private string GetLanguage()
    {
        if ((FLanguageID > 0) && (FLanguageID <= 118))
            return PxLangTable[FLanguageID].Name;
        else
            return "";
    }

    /// <summary>原文 :667/:1072 SetLanguage（大小写敏感、只扫 1..118、未命中不报错）。</summary>
    private void SetLanguage(string Value)
    {
        if (Active)
        {
            for (int I = 1; I <= 118; I++)
            {
                if (PxLangTable[I].Name == Value)
                {
                    FLanguageID = I;
                    break;
                }
            }
        }
        else
        {
            FLanguageID = 0;
        }
    }

    /// <summary>原文 :668/:1055 SetTableName。</summary>
    private void SetTableName(string Value)
    {
        if (FTableName != Value)
        {
            if (Active) Close();
            FTableName = Value;
        }
    }

    /// <summary>原文 :669/:1093 NativeToFieldType。</summary>
    private TFieldType NativeToFieldType(byte NativeType)
    {
        TFieldType Result = TFieldType.ftUnknown;
        switch (NativeType)
        {
            case PxFieldType.pxfAlpha: Result = TFieldType.ftString; break;
            case PxFieldType.pxfDate: Result = TFieldType.ftDate; break;
            case PxFieldType.pxfShort: Result = TFieldType.ftSmallint; break;
            case PxFieldType.pxfLong: Result = TFieldType.ftInteger; break;
            case PxFieldType.pxfCurrency: Result = TFieldType.ftCurrency; break;
            case PxFieldType.pxfNumber: Result = TFieldType.ftFloat; break;
            case PxFieldType.pxfLogical: Result = TFieldType.ftBoolean; break;
            case PxFieldType.pxfMemoBLOB: Result = TFieldType.ftMemo; break;
            case PxFieldType.pxfBLOB: Result = TFieldType.ftBlob; break;
            case PxFieldType.pxfFmtMemoBLOB: Result = TFieldType.ftFmtMemo; break;
            case PxFieldType.pxfOLE: Result = TFieldType.ftParadoxOle; break;
            case PxFieldType.pxfGraphic: Result = TFieldType.ftGraphic; break;
            case PxFieldType.pxfTime: Result = TFieldType.ftTime; break;
            case PxFieldType.pxfTimestamp: Result = TFieldType.ftDateTime; break;
            case PxFieldType.pxfAutoInc: Result = TFieldType.ftAutoInc; break;
            case PxFieldType.pxfBCD: Result = TFieldType.ftBCD; break;
            case PxFieldType.pxfBytes: Result = TFieldType.ftBytes; break;
        }
        return Result;
    }

    /// <summary>原文 :670/:1117 ReadDataBlock（块号 1-based；越界抛 EParadoxError）。</summary>
    private unsafe TDataBlock ReadDataBlock(ushort BlockNum)
    {
        if ((BlockNum < 1) || (BlockNum > FFileHeader.FileBlocks))
            throw EParadoxError.CreateFmt("Block %d read error", BlockNum);
        FFileStream.Seek(FFileHeader.HeaderSize + (BlockNum - 1) * FFileHeader.MaxTableSize * 1024, 0 /*soFromBeginning*/);
        var buf = new byte[Marshal.SizeOf<TDataBlock>()];
        FFileStream.ReadBuffer(buf, 0, buf.Length);
        fixed (byte* pb = buf) return *(TDataBlock*)pb;
    }

    /// <summary>原文 :672/:1125 DetectLang（1..118 首个命中；版本 ≥ $05 需 SortOrder+CodePage+SortOrderID 三者相等）。</summary>
    private int DetectLang()
    {
        int Result = 0;
        for (int I = 1; I <= 118; I++)
        {
            if ((FFileHeader.FileVersionID >= 0x05))
            {
                if ((PxLangTable[I].SortOrder == FFileHeader.SortOrder) &&
                    (PxLangTable[I].CodePage == FDataHeader.DosGlobalCodePage) &&
                    (PxLangTable[I].SortOrderID == FSortOrderID))
                {
                    Result = I;
                    break;
                }
            }
            else
            {
                if ((PxLangTable[I].SortOrder == FFileHeader.SortOrder))
                {
                    Result = I;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 :673/:1343 EncodingString（原文缺陷 D12/D13：无 0 号保护 + Codepage 常量永不相等）。</summary>
    private string EncodingString(string S)
    {
        string Result = S;
        switch (PxLangTable[FLanguageID].CodePage)
        {
            case 1251:
                {
                    if (Codepage == "UTF-8") Result = ParadoxConvSeam.Encoding(TEncodingKind.CP1251, TEncodingKind.UTF8, S);
                    if (Codepage == "KOI8R") Result = ParadoxConvSeam.Encoding(TEncodingKind.CP1251, TEncodingKind.KOI8R, S);
                    break;
                }
            case 866:
                {
                    if (Codepage == "UTF-8") Result = ParadoxConvSeam.Encoding(TEncodingKind.CP866, TEncodingKind.UTF8, S);
                    if (Codepage == "KOI8R") Result = ParadoxConvSeam.Encoding(TEncodingKind.CP866, TEncodingKind.KOI8R, S);
                    if (Codepage == "CP1251") Result = ParadoxConvSeam.Encoding(TEncodingKind.CP866, TEncodingKind.CP1251, S);
                    break;
                }
        }
        return Result;
    }

    // ---- 原文 :675-699 protected override ----

    /// <summary>原文 :676/:755 GetBookmarkFlag。</summary>
    protected override unsafe TBookmarkFlag GetBookmarkFlag(IntPtr Buffer)
    {
        return ((TPxRecordHeader*)Buffer)->BookmarkFlag;
    }

    /// <summary>原文 :677/:760 SetBookmarkFlag。</summary>
    protected override void SetBookmarkFlag(IntPtr Buffer, TBookmarkFlag Value)
    {
        unsafe
        {
            var p = (TPxRecordHeader*)Buffer;
            p->BookmarkFlag = Value;
        }
    }

    /// <summary>原文 :679/:765 InternalHandleException → Application.HandleException(Self)。</summary>
    protected override void InternalHandleException()
    {
        ParadoxApplication.HandleException(this);
    }

    /// <summary>
    /// 原文 :680/:771 InternalInitFieldDefs（行 770 注释：载入所有字段名 chongchong 2016-07-31）。
    /// P 的演变完全照抄（含缺陷 D2）：:778-781 起点、:797 加字段区尺寸、:800-803 加表名区、
    /// :828 未加密时**覆盖**为 NumFields*2、:835 以 soFromCurrent 相对定位。
    /// </summary>
    protected override unsafe void InternalInitFieldDefs()
    {
        int I;
        int P, Offset;
        byte B;
        string S;

        if ((FFileHeader.FileVersionID >= 0x05))
            P = 0x78;
        else
            P = 0x58;

        FFileStream.Seek(P, 0 /*soFromBeginning*/);

        FFields = new TFieldInfoRecord[FFileHeader.NumFields];
        FFieldOffsets = new int[FFileHeader.NumFields];

        Offset = 0;
        FieldDefs.Clear();
        for (I = 0; I <= FFileHeader.NumFields - 1; I++)
        {
            FFieldOffsets[I] = Offset;
            var fld = new byte[Marshal.SizeOf<TFieldInfoRecord>()];
            FFileStream.Read(fld, 0, fld.Length);
            fixed (byte* pf = fld) FFields[I] = *(TFieldInfoRecord*)pf;
            Offset = Offset + FFields[I].FieldSize;
        }

        P = P + FFileHeader.NumFields * Marshal.SizeOf<TFieldInfoRecord>() + 4 + FFileHeader.NumFields * 4;

        // TableName size
        if ((FFileHeader.FileVersionID >= 0x0C))
            P = P + 261;
        else
            P = P + 79;

        FFileStream.Seek(P, 0 /*soFromBeginning*/);
        for (I = 0; I <= FFileHeader.NumFields - 1; I++)
        {
            S = "";
            do
            {
                // 原文字段名逐字节读（FPC 分支用 ReadByte，非 FPC 分支用 Read(B,1)）
                var one = new byte[1];
                FFileStream.Read(one, 0, 1);
                B = one[0];
                if ((B != 0)) S = S + (char)B;
            }
            while (B != 0);

            // 原文 :818-823：字符串/Blob 族带 FieldSize，其余不带
            switch (FFields[I].FieldType)
            {
                case PxFieldType.pxfAlpha:
                case PxFieldType.pxfMemoBLOB:
                case PxFieldType.pxfBLOB:
                case PxFieldType.pxfFmtMemoBLOB:
                case PxFieldType.pxfOLE:
                case PxFieldType.pxfGraphic:
                    FieldDefs.Add(S, NativeToFieldType(FFields[I].FieldType), FFields[I].FieldSize, false);
                    break;
                default:
                    FieldDefs.Add(S, NativeToFieldType(FFields[I].FieldType));
                    break;
            }
        }

        if (!FIsEncrypted)
        {
            P = FFileHeader.NumFields * 2;
        }
        else
        {
            // Здесь нужно рассчитать смещение для зашифрованного файла（原文注释：此处需计算加密文件的偏移）
        }

        FFileStream.Seek(P, 1 /*soFromCurrent*/);
        S = "";
        do
        {
            var one = new byte[1];
            FFileStream.Read(one, 0, 1);
            B = one[0];
            if ((B != 0)) S = S + (char)B;
        }
        while (B != 0);
        FSortOrderID = S;

        FLanguageID = DetectLang();
    }

    /// <summary>
    /// 原文 :681/:850 InternalOpen。
    /// 注意原文缺陷 D3：`not (FileType in [0, 2])` 会把 FileType=1（.PX 主索引）也拒掉。
    /// </summary>
    protected override unsafe void InternalOpen()
    {
        if (TableName == "")
            throw EParadoxError.CreateFmt("TableName is not set", Array.Empty<object>());

        try
        {
            FFileStream = TFileStream.Create(TableName, PxFileMode.fmOpenRead);
        }
        catch (Exception E)
        {
            throw EParadoxError.CreateFmt("Unable to open database \"%S\" - %S", FTableName, E.Message);
        }

        var fh = new byte[Marshal.SizeOf<TPxFileHeader>()];
        FFileStream.Read(fh, 0, fh.Length);
        fixed (byte* pfh = fh) FFileHeader = *(TPxFileHeader*)pfh;

        if (!(FFileHeader.FileType == 0 || FFileHeader.FileType == 2))
            throw EParadoxError.CreateFmt("\"%S\" - is not .DB data file", FTableName);

        if ((FFileHeader.FileVersionID >= 0x05))
        {
            var dh = new byte[Marshal.SizeOf<TPxDataHeader>()];
            FFileStream.Read(dh, 0, dh.Length);
            fixed (byte* pdh = dh) FDataHeader = *(TPxDataHeader*)pdh;
            FIsEncrypted = FDataHeader.Encryption2 != 0;
        }
        else
        {
            FIsEncrypted = FFileHeader.Encryption1 != 0;
        }

        string FileName = PxFileUtils.ChangeFileExt(TableName, ".mb");
        if (PxFileUtils.FileExists(FileName))
        {
            try
            {
                FBlobStream = TFileStream.Create(PxFileUtils.ChangeFileExt(TableName, ".mb"), PxFileMode.fmOpenRead);
            }
            catch (Exception)
            {
                FBlobStream = null;
            }

            if (FBlobStream == null)
            {
                try
                {
                    FBlobStream = TFileStream.Create(PxFileUtils.ChangeFileExt(TableName, ".MB"), PxFileMode.fmOpenRead);
                }
                catch (Exception)
                {
                    // remark on 2010/02/18 by bruce0829@yahoo.com.tw
                    // write('Blob file is not open: ');
                    // writeln(ChangeFileExt(TableName, '.MB'));
                    FBlobStream = null;
                }
            }
        }

        InternalInitFieldDefs();
        // 原文 :902-903：`if DefaultFields then CreateFields; BindFields(True);`
        // 托管侧 CreateFields/BindFields(true) 由 TDataSet.Open 在 InternalOpen 之后统一执行
        // （接缝：原文在 InternalOpen 内部做，顺序等价 —— 见报告接缝清单）。
        FIsOpen = true;
        FCursor = 0;
    }

    /// <summary>原文 :682/:908 IsCursorOpen。</summary>
    protected override bool IsCursorOpen => FIsOpen;

    /// <summary>原文 :683/:913 InternalClose。</summary>
    protected override void InternalClose()
    {
        // 原文 :915-916 BindFields(False) + DestroyFields 由 TDataSet.Close 统一执行（接缝）
        FIsOpen = false;

        if (FBlobStream != null)
            FBlobStream.Free();

        FFileStream.Free();

        FLanguageID = 0;
    }

    /// <summary>
    /// 原文 :685/:927 GetRecord（行 965 注释：修正读取有问题 chongchong 2016-07-30）。
    /// 循环/边界/异常行为逐行照抄；原文缺陷 D4（RecordSize=0 除零）保留。
    /// </summary>
    protected override TGetResult GetRecord(IntPtr Buffer, TGetMode GetMode, bool DoCheck)
    {
        ushort BlockIndex, RecordStart;
        int TotalRecords;
        TDataBlock DataBlock;
        IntPtr P;
        int nSeekPos;

        var Result = TGetResult.grOK;

        switch (GetMode)
        {
            case TGetMode.gmPrior:
                {
                    if (FCursor <= 1)
                        Result = TGetResult.grBOF;
                    else
                        FCursor = FCursor - 1;   // Dec(FCursor)
                    break;
                }
            case TGetMode.gmNext:
                {
                    if (FCursor >= RecordCount)
                        Result = TGetResult.grEOF;
                    else
                        FCursor = FCursor + 1;   // Inc(FCursor)
                    break;
                }
            case TGetMode.gmCurrent:
                {
                    if ((FCursor < 1) || (FCursor > RecordCount))
                        Result = TGetResult.grError;
                    break;
                }
        }

        if (Result == TGetResult.grOK)
        {
            unsafe
            {
                ((TPxRecordHeader*)Buffer)->RecordIndex = FCursor;
                ((TPxRecordHeader*)Buffer)->BookmarkFlag = TBookmarkFlag.bfCurrent; // GJK
            }

            // Находим позицию записи в файле
            BlockIndex = FFileHeader.FirstBlock;
            TotalRecords = 0;
            RecordStart = 0;
            while ((TotalRecords < FCursor))
            {
                DataBlock = ReadDataBlock(BlockIndex);
                RecordStart = (ushort)TotalRecords;
                TotalRecords = TotalRecords + (DataBlock.AddDataSize / FFileHeader.RecordSize) + 1;
                if (TotalRecords >= FCursor)
                    break;
                else
                    BlockIndex = DataBlock.NextBlock;
            }

            nSeekPos = FFileHeader.HeaderSize +
                (BlockIndex - 1) * FFileHeader.MaxTableSize * 1024 +
                (FCursor - RecordStart - 1) * FFileHeader.RecordSize +
                Marshal.SizeOf<TDataBlock>();

            FFileStream.Seek(nSeekPos, 0 /*soFromBeginning*/);
            P = Buffer + Marshal.SizeOf<TPxRecordHeader>();
            unsafe
            {
                FFileStream.Read((void*)P, FFileHeader.RecordSize);
            }
        }
        else
        {
            // This prevents garbage in datagrid when the last record was deleted
            P = Buffer + Marshal.SizeOf<TPxRecordHeader>();      // GJK
            PxBuffer.FillChar(P, FFileHeader.RecordSize, 0);      // GJK
            unsafe
            {
                ((TPxRecordHeader*)Buffer)->BookmarkFlag = TBookmarkFlag.bfEOF; // GJK
            }
        }

        if (DoCheck && (Result == TGetResult.grError))
        {
            DatabaseError("Error in GetRecord()");
        }

        return Result;
    }

    /// <summary>原文 :686/:1003 AllocRecordBuffer。</summary>
    protected override IntPtr AllocRecordBuffer()
    {
        return Marshal.AllocHGlobal(Marshal.SizeOf<TPxRecordHeader>() + FFileHeader.RecordSize);
    }

    /// <summary>原文 :687/:1008 FreeRecordBuffer。</summary>
    protected override void FreeRecordBuffer(IntPtr Buffer)
    {
        Marshal.FreeHGlobal(Buffer);
    }

    /// <summary>原文 :688/:1013 InternalInitRecord（原文为**空实现**，照抄）。</summary>
    protected override void InternalInitRecord(IntPtr Buffer)
    {
    }

    /// <summary>原文 :690/:1018 InternalFirst（FCursor := 0）。</summary>
    protected override void InternalFirst()
    {
        FCursor = 0;
    }

    /// <summary>原文 :691/:1023 InternalLast（FCursor := RecordCount + 1）。</summary>
    protected override void InternalLast()
    {
        FCursor = RecordCount + 1;
    }

    /// <summary>原文 :692/:1028 InternalSetToRecord（FCursor := PPxRecordHeader(Buffer)^.RecordIndex）。</summary>
    protected override unsafe void InternalSetToRecord(IntPtr Buffer)
    {
        FCursor = ((TPxRecordHeader*)Buffer)->RecordIndex;
    }

    /// <summary>原文 :694/:1033 GetCanModify（恒 False）。</summary>
    protected override bool GetCanModify => false;

    /// <summary>原文 :696/:1038 GetRecordCount（= FFileHeader.NumRecords）。</summary>
    protected override int GetRecordCount => FFileHeader.NumRecords;

    /// <summary>原文 :698/:1043 SetRecNo（越界即 Exit；合法则 Resync([])）。</summary>
    protected override void SetRecNo(int Value)
    {
        if ((Value < 1) || (Value >= RecordCount + 1)) return;
        FCursor = Value;
        Resync(Array.Empty<TField>());
    }

    /// <summary>原文 :699/:1050 GetRecNo。</summary>
    protected override unsafe int GetRecNo => ((TPxRecordHeader*)ActiveBuffer)->RecordIndex;

    // ---- 原文 :700-709 public ----

    /// <summary>原文 :702/:1168 constructor Create(AOwner: TComponent)。</summary>
    public TParadoxDataSet()
    {
        // inherited Create(AOwner)（原文 :1170）
        FCodepage = ParadoxConvSeam.GetCodepage();   // 原文 :1171
        FEncodingMemo = true;                        // 原文 :1172
    }

    /// <summary>原文 :703/:1175 destructor Destroy（仅 inherited Destroy，原文如此）。</summary>
    public void Destroy()
    {
        // inherited Destroy（原文 :1177）
    }

    /// <summary>
    /// 原文 :705/:1180 GetFieldData（TValueBuffer 指针版）。
    /// 逐行照抄：空缓冲直接 False；逆序读入 + 末字节 xor $80 的空值判定（原文缺陷 D6/D7）；
    /// case 分支不含 Blob 族（D8）。
    /// </summary>
    public override unsafe bool GetFieldData(TField Field, void* Buffer)
    {
        if (Buffer == null)
        {
            return false;
        }

        var Result = true;
        var P = new byte[FFields[Field.FieldNo - 1].FieldSize];
        IntPtr Src;

        Src = ActiveBuffer + Marshal.SizeOf<TPxRecordHeader>() + FFieldOffsets[Field.FieldNo - 1];

        bool IsNull = true;
        byte ftype = FFields[Field.FieldNo - 1].FieldType;
        int fsize = FFields[Field.FieldNo - 1].FieldSize;
        if ((ftype >= 2 && ftype <= 6) || (ftype >= 0x14 && ftype <= 0x16))
        {
            for (int I = 0; I <= fsize - 1; I++)
            {
                P[I] = PxBuffer.ReadByteAt(Src, fsize - I - 1);
                if (P[I] != 0)
                {
                    IsNull = false;
                }
            }

            // GJK:Using a loop var outside the loop can cause (in Delphi) strange behavior
            // P[I] := Chr(Ord(P[I]) xor $80);
            P[fsize - 1] = (byte)(P[fsize - 1] ^ 0x80);

            if (IsNull)
            {
                return false;
            }
        }

        switch (ftype)
        {
            case PxFieldType.pxfAlpha:
                PxAnsi.StrLCopy((byte*)Buffer, (byte*)Src, fsize);
                break;
            // 原文 :1227-1235 把 P 当作"逆序读入后的本地缓冲"再按目标类型解引用；
            // 托管侧用同一 byte[] 的 fixed 地址（同一内存、同一语义）。
            case PxFieldType.pxfDate:
                fixed (byte* pp = P) *(int*)Buffer = *(int*)pp;
                break;
            case PxFieldType.pxfShort:
                fixed (byte* pp = P) *(short*)Buffer = *(short*)pp;
                break;
            case PxFieldType.pxfLong:
                fixed (byte* pp = P) *(int*)Buffer = *(int*)pp;
                break;
            case PxFieldType.pxfCurrency:
                fixed (byte* pp = P) *(double*)Buffer = *(double*)pp;
                break;
            case PxFieldType.pxfNumber:
                fixed (byte* pp = P) *(double*)Buffer = *(double*)pp;
                break;
            case PxFieldType.pxfLogical:
                // 原文 :1232 `PWordbool(Buffer)^ := (Ord(Src^) = $80)`：直接看**源字节**，
                // 与上面 P 的逆序/xor 无关；PWordBool 为 2 字节（False=0 / True=$FFFF）。
                *(ushort*)Buffer = (ushort)(PxBuffer.ReadByte(Src) == 0x80 ? 0xFFFF : 0x0000);
                break;
            case PxFieldType.pxfTime:
                fixed (byte* pp = P) *(double*)Buffer = *(double*)pp;
                break;
            case PxFieldType.pxfTimestamp:
                fixed (byte* pp = P) *(double*)Buffer = *(double*)pp;
                break;
            case PxFieldType.pxfAutoInc:
                fixed (byte* pp = P) *(int*)Buffer = *(int*)pp;
                break;

            // pxfMemoBLOB     = $0C;
            // pxfBLOB         = $0D;
            // pxfFmtMemoBLOB  = $0E;
            // pxfOLE          = $0F;
            // pxfGraphic      = $10;
            // pxfBCD          = $17;
            // pxfBytes        = $18;
            default:
                Result = false;
                break;
        }

        return Result;
    }

    /// <summary>GetFieldData 的托管缓冲重载（接缝；语义等价于指针版）。</summary>
    public override bool GetFieldData(TField Field, byte[] Buffer)
    {
        unsafe
        {
            fixed (byte* pb = Buffer)
            {
                return GetFieldData(Field, pb);
            }
        }
    }

    /// <summary>
    /// 原文 :706/:1249 CreateBlobStream。
    /// Blob 信息块取 `Src + Field.Size - SizeOf(TPxBlob)`（原文缺陷 D9/D10/D11 照抄）；
    /// 单块走 `.mb` 偏移 Loc+9，多块索引走 Loc+12+5*Idx 再按 Loc+16*Offset 取数据。
    /// 原文 :1327-1331 的注释代码块逐字保留为注释。
    /// </summary>
    public override unsafe TMemoryStream CreateBlobStream(TField Field, TBlobStreamMode Mode)
    {
        TMemoryStream MS;
        IntPtr Src, Header;
        TPxBlob Blob;
        TPxBlobIdx BlobIdx;
        string S;
        byte Idx;
        int Loc;
        // Buffer: PAnsiChar;

        TMemoryStream Result = null;
        if ((Mode != TBlobStreamMode.bmRead)) return Result;

        Src = ActiveBuffer + Marshal.SizeOf<TPxRecordHeader>() + FFieldOffsets[Field.FieldNo - 1];

        Header = Src + Field.Size - Marshal.SizeOf<TPxBlob>();
        Blob = *(TPxBlob*)Header;

        if (Blob.Length == 0) return Result;

        MS = TMemoryStream.Create();
        Result = MS;

        if (Blob.Length > Field.Size - Marshal.SizeOf<TPxBlob>())
        {
            if (FBlobStream != null)
            {
                Idx = (byte)(Blob.FileLoc & 0xFF);
                Loc = Blob.FileLoc & unchecked((int)0xFFFFFF00);

                if (Idx == 0xFF)
                {   // Read from a Single Blob Block
                    FBlobStream.Seek(Loc + 9, 0 /*soFromBeginning*/);
                    if (Field.DataType == TFieldType.ftMemo)
                    {
                        var sBuf = new byte[Blob.Length];
                        FBlobStream.Read(sBuf, 0, Blob.Length);
                        S = PxAnsi.FromBytes(sBuf, sBuf.Length);

                        if (EncodingMemo) S = EncodingField(S, Field);

                        MS.Write(PxAnsi.ToBytes(S), 0, S.Length);
                    }
                    else
                    {
                        MS.CopyFrom(FBlobStream, Blob.Length);
                    }
                }
                else
                {
                    FBlobStream.Seek(Loc + 12 + 5 * Idx, 0 /*soFromBeginning*/);
                    var biBuf = new byte[Marshal.SizeOf<TPxBlobIdx>()];
                    FBlobStream.Read(biBuf, 0, biBuf.Length);
                    fixed (byte* pbi = biBuf) BlobIdx = *(TPxBlobIdx*)pbi;
                    FBlobStream.Seek(Loc + 16 * BlobIdx.Offset, 0 /*soFromBeginning*/);

                    if (Field.DataType == TFieldType.ftMemo)
                    {
                        var sBuf = new byte[Blob.Length];
                        FBlobStream.Read(sBuf, 0, Blob.Length);
                        S = PxAnsi.FromBytes(sBuf, sBuf.Length);

                        if (EncodingMemo) S = EncodingField(S, Field);

                        MS.Write(PxAnsi.ToBytes(S), 0, S.Length);
                    }
                    else
                    {
                        MS.CopyFrom(FBlobStream, Blob.Length);
                    }
                }
            }
        }
        else
        {
            if (Field.DataType == TFieldType.ftMemo)
            {
                {
                    // StrLCopy(Buffer, PAnsiChar(Src), Blob.Length);
                    // S := Buffer;
                    // MS.Write(S[1], Length(S));
                    MS.Write((void*)Src, Blob.Length);
                }
            }
            else
            {
                MS.Write((void*)Src, Blob.Length);
            }
        }

        MS.Position = 0;
        return Result;
    }

    /// <summary>原文 :673/:1153 EncodingField（OnEncode 优先；否则 FLanguageID&lt;1 直接 Exit；否则 EncodingString）。</summary>
    private string EncodingField(string S, TField Field)
    {
        string Result = S;

        if (FOnEncode != null)
        {
            Result = FOnEncode(this, Field, S);
        }
        else
        {
            if (FLanguageID < 1) return Result;
            Result = EncodingString(S);
        }
        return Result;
    }

    // ---- 原文 :707-746 属性（published/public） ----

    /// <summary>原文 :707 <c>property FileHeader: TPxFileHeader read FFileHeader</c>。</summary>
    public TPxFileHeader FileHeader => FFileHeader;

    /// <summary>原文 :708 <c>property DataHeader: TPxDataHeader read FDataHeader</c>。</summary>
    public TPxDataHeader DataHeader => FDataHeader;

    /// <summary>原文 :709 <c>property SortOrderID: AnsiString read FSortOrderID</c>。</summary>
    public string SortOrderID => FSortOrderID;

    /// <summary>原文 :711 <c>property TableName: string read FTableName write SetTableName</c>。</summary>
    public string TableName
    {
        get => FTableName;
        set => SetTableName(value);
    }

    /// <summary>原文 :712 <c>property Language: AnsiString read GetLanguage write SetLanguage</c>。</summary>
    public string Language
    {
        get => GetLanguage();
        set => SetLanguage(value);
    }

    /// <summary>原文 :713 <c>property Codepage: string read FCodepage write FCodepage</c>。</summary>
    public string Codepage
    {
        get => FCodepage;
        set => FCodepage = value;
    }

    /// <summary>原文 :714 <c>property EncodingMemo: Boolean read FEncodingMemo write FEncodingMemo</c>。</summary>
    public bool EncodingMemo
    {
        get => FEncodingMemo;
        set => FEncodingMemo = value;
    }

    /// <summary>原文 :746 <c>property OnEncode: TEncodeEvent read FOnEncode write FOnEncode</c>。</summary>
    public TEncodeEvent OnEncode
    {
        get => FOnEncode;
        set => FOnEncode = value;
    }

    /// <summary>接缝：原文 :716 <c>property Active</c>（转发 DB.pas TDataSet.Active；由基类公开）。</summary>

    // ---- 接缝：内部可观察状态（供测试与集成方读取；原文无对应属性） ----

    /// <summary>接缝：原文私有 FIsEncrypted（:659）的只读视图。</summary>
    public bool IsEncrypted => FIsEncrypted;

    /// <summary>接缝：原文私有 FLanguageID（:661）的只读视图。</summary>
    public int LanguageID => FLanguageID;

    /// <summary>接缝：原文私有 FCursor（:652）的只读视图。</summary>
    public int Cursor => FCursor;

    /// <summary>接缝：原文私有 FFields（:655）的只读视图。</summary>
    public TFieldInfoRecord[] FieldInfo => FFields;

    /// <summary>接缝：原文私有 FFieldOffsets（:656）的只读视图。</summary>
    public int[] FieldOffsets => FFieldOffsets;

    /// <summary>接缝：原文私有 FDataHeader 是否已读（版本 &lt; $05 时不读）。</summary>
    public bool DataHeaderRead => FFileHeader.FileVersionID >= 0x05;

    // ---- 接缝内部工具（把 Delphi 的非托管类型转换映射到托管） ----
}
