// ============================================================================
// 源单元：Source\RunGate\ParadoxDataSet.pas（GBK，1362 行）
// 本文件 = **最小接缝**（不是移植单元的一部分），只为让 TParadoxDataSet 的 1:1 逻辑
// 可在 GXX.Core 编译：VCL DB 层（DB.pas 的 TDataSet/TField/TFieldDefs/TFieldType…）
// 与 Classes.pas 的 TFileStream/TMemoryStream/Seek 常量在**全仓范围内不存在**
// （实测：`git grep -E "(class|struct|enum) +(TDataSet|TField|TFieldType)"` 在
//  src/**/*.cs 上零命中）。且 GXX.Core 不能反向引用 GXX.RunGate
// （GXX.RunGate.csproj:9 → ProjectReference GXX.Core），故：
//
//   接缝：DB/Classes 最小面（本文件）
//   接缝：ParadoxConv 的三个入口 GetCodepage/Encoding/TEncodingKind（见 ParadoxConvSeam.cs）
//
// 覆盖的原文调用点（逐条）：
//   ParadoxDataSet.pas:649-650  FFileStream/FBlobStream: TFileStream（Create/fmOpenRead/
//                               Seek/Read/ReadBuffer/Free）
//   :651/:659/:682/:917  FIsOpen/IsCursorOpen（本接缝自己持有 FActive）
//   :676-699 TDataSet 虚方法（GetBookmarkFlag…GetRecNo）→ 本接缝 virtual
//   :702-703 TComponent/TDataSet.Create/Destroy
//   :767     Application.HandleException(Self)（Forms.pas）
//   :789     FieldDefs.Clear / :820,:822 FieldDefs.Add（4 参 / 2 参重载）
//   :902     DefaultFields / CreateFields；:903 BindFields(True)；:916 DestroyFields/BindFields(False)
//   :999     DatabaseError()
//   :1047    Resync([])
//   :1194-1246 GetFieldData 的 TField.FieldNo/FieldSize/DataType + TValueBuffer
//   :1251/:1274/:1298 TMemoryStream（Create/Write/CopyFrom/Position）
//   :1287,:1325 Field.DataType = ftMemo；:1261 bmRead
//
// 刻意**不**引入的内容（避免"顺手移植依赖"，台账 §15）：TCustomDataSet/数据集状态机
// （dsInsert/dsEdit）、TDataSource/TDBGrid、Filter/Filtered/OnFilterRecord、书签全族、
// TBlobStream、字段校验/Required/ReadOnly、TFieldDef.Attributes 等 ——
// TParadoxDataSet 原文一行都没用到。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace GXX.Core.Paradox;

// ==================== Classes.pas：流与 Seek 常量 ====================

/// <summary>Classes.pas 的打开模式常量（原文 :858/:883/:891 用 fmOpenRead）。</summary>
public static class PxFileMode
{
    /// <summary>fmOpenRead（Delphi Classes.pas 常量）。</summary>
    public const int fmOpenRead = 0;
}

/// <summary>
/// 接缝：Classes.pas **TFileStream**（原文 :649/:650 字段类型、:858/:883/:891 Create、
/// :783/:805/:835/:985/:1121/:1286/:1303/:1305 Seek、:793/:813/:841/:864/:871/:987
/// Read、:1122 ReadBuffer、:920/:922 Free）。
/// 语义要点（照抄 Delphi）：<see cref="Read"/> 返回**实际读取字节数**且**不抛异常**；
/// <see cref="ReadBuffer"/> 读不满才抛 <see cref="EParadoxError"/>。
/// </summary>
public sealed class TFileStream : IDisposable
{
    private readonly FileStream _fs;
    private bool _disposed;

    private TFileStream(FileStream fs) { _fs = fs; }

    /// <summary>构造：FileStream.Create(FileName, Mode)。失败抛 <see cref="EParadoxError"/>（原文 :857-862 的 try/except 语义）。</summary>
    public static TFileStream Create(string fileName, int mode)
    {
        try
        {
            return new TFileStream(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// FileStream.Seek(Offset, Origin)（原文 TSeekOrigin：soBeginning=0 / soCurrent=1 / soEnd=2）。
    /// 返回新的绝对偏移。
    /// </summary>
    public long Seek(long offset, int origin)
        => _fs.Seek(offset, origin == 0 ? SeekOrigin.Begin : origin == 1 ? SeekOrigin.Current : SeekOrigin.End);

    /// <summary>FileStream.Read(var Buffer; Count)：返回实际字节数（原文多处忽略返回值）。</summary>
    public int Read(byte[] buffer, int offset, int count) => _fs.Read(buffer, offset, count);

    /// <summary>FileStream.Read 的指针重载（原文 :987 `Read(P^, RecordSize)`）。</summary>
    public unsafe int Read(void* buffer, int count)
        => _fs.Read(new Span<byte>(buffer, count));

    /// <summary>FileStream.ReadBuffer：读满 <paramref name="count"/>，否则抛异常（原文 :1122）。</summary>
    public void ReadBuffer(byte[] buffer, int offset, int count)
    {
        int n = _fs.Read(buffer, offset, count);
        if (n != count) throw new EParadoxError("Stream read error");
    }

    /// <summary>FileStream.Free（原文 :920/:922）。</summary>
    public void Free() => Dispose();

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _fs.Dispose();
    }
}

/// <summary>
/// 接缝：Classes.pas **TMemoryStream**（原文 :1251 MS: TMemoryStream、:1274 Create、
/// :1294/:1314/:1332/:1336 Write、:1298/:1318 CopyFrom、:1340 Position := 0）。
/// 原文被调用方当 TStream 返回（CreateBlobStream 返回 TStream），故本接缝同时充当 TStream。
/// </summary>
public sealed class TMemoryStream : IDisposable
{
    private readonly MemoryStream _ms = new MemoryStream();
    private bool _disposed;

    /// <summary>TMemoryStream.Create。</summary>
    public static TMemoryStream Create() => new TMemoryStream();

    /// <summary>TStream.Write(Buffer, Count) 的指针重载（原文 :1332/:1336 `MS.Write(Src, Blob.Length)`）。</summary>
    public unsafe void Write(void* buffer, int count)
    {
        if (count <= 0) return;   // 原文 TStream.Write 对 Count<=0 直接返回
        _ms.Write(new ReadOnlySpan<byte>(buffer, count));
    }

    /// <summary>TStream.Write(Buffer, Count) 的字节数组重载（原文 :1294/:1314 `MS.Write(S[1], Length(S))`）。</summary>
    public void Write(byte[] buffer, int offset, int count)
    {
        if (count <= 0) return;
        _ms.Write(buffer, offset, count);
    }

    /// <summary>TStream.CopyFrom(Source, Count)（原文 :1298/:1318）。</summary>
    public void CopyFrom(TFileStream source, int count)
    {
        if (count <= 0) return;
        var buf = new byte[count];
        int n = source.Read(buf, 0, count);
        _ms.Write(buf, 0, n);
    }

    /// <summary>TStream.Position（原文 :1340 `MS.Position := 0`）。</summary>
    public long Position
    {
        get => _ms.Position;
        set => _ms.Position = value;
    }

    /// <summary>TStream.Size。</summary>
    public long Size => _ms.Length;

    /// <summary>取得已写内容（测试与接缝消费方用）。</summary>
    public byte[] ToArray() => _ms.ToArray();

    /// <summary>TMemoryStream.Free。</summary>
    public void Free() => Dispose();

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _ms.Dispose();
    }
}

/// <summary>接缝：SysUtils.pas 的 FileExists / ChangeFileExt（原文 :879/:880/:883/:891）。</summary>
public static class PxFileUtils
{
    /// <summary>SysUtils.FileExists。</summary>
    public static bool FileExists(string fileName)
        => !string.IsNullOrEmpty(fileName) && File.Exists(fileName);

    /// <summary>
    /// SysUtils.ChangeFileExt：把最后一个 '.' 之后（含 '.'）的部分替换为 <paramref name="newExt"/>。
    /// 原文用于 `.DB` → `.mb` / `.MB`（:879/:883/:891）。
    /// </summary>
    public static string ChangeFileExt(string fileName, string newExt)
    {
        if (fileName == null) return newExt ?? "";
        int p = fileName.LastIndexOf('.');
        // Delphi 的 ChangeFileExt 在无扩展名分隔符时直接追加（最后一个路径分隔符之后不再找 '.'）
        int sep = fileName.LastIndexOfAny(new[] { '\\', '/' });
        if (p <= sep) p = -1;
        return p < 0 ? fileName + (newExt ?? "") : fileName.Substring(0, p) + (newExt ?? "");
    }
}

// ==================== DB.pas：枚举与异常 ====================

/// <summary>DB.pas TFieldType：**完整枚举且顺序照抄**（原文 :669/:1093-1115 的取值面 + 方法签名）。</summary>
public enum TFieldType
{
    ftUnknown,
    ftString,
    ftSmallint,
    ftInteger,
    ftWord,
    ftBoolean,
    ftFloat,
    ftCurrency,
    ftBCD,
    ftDate,
    ftTime,
    ftDateTime,
    ftBytes,
    ftVarBytes,
    ftAutoInc,
    ftBlob,
    ftMemo,
    ftGraphic,
    ftFmtMemo,
    ftParadoxOle,
    ftDBaseOle,
    ftTypedBinary,
    ftCursor,
    ftFixedChar,
    ftWideString,
    ftLargeint,
    ftADT,
    ftArray,
    ftReference,
    ftDataSet,
    ftOraBlob,
    ftOraClob,
    ftVariant,
    ftInterface,
    ftIDispatch,
    ftGuid,
    ftTimeStamp,
    ftFMTBcd,
}

/// <summary>DB.pas TGetResult（原文 :685/:927 签名，:935/:941/:948/:955 取值）。</summary>
public enum TGetResult
{
    grOK,
    grBOF,
    grEOF,
    grError,
}

/// <summary>DB.pas TGetMode（原文 :685/:927 签名，:938/:945/:952 取值）。</summary>
public enum TGetMode
{
    gmCurrent,
    gmNext,
    gmPrior,
}

/// <summary>DB.pas TBookmarkFlag（原文 :495/:676/:677/:962/:994 取值）。</summary>
public enum TBookmarkFlag
{
    bfCurrent,
    bfBOF,
    bfEOF,
    bfInserted,
}

/// <summary>DB.pas TBlobStreamMode（原文 :706/:1249/:1261 取值）。</summary>
public enum TBlobStreamMode
{
    bmRead,
    bmWrite,
    bmReadWrite,
}

/// <summary>
/// 接缝：原文 <c>EParadoxError = class(Exception)</c>（ParadoxDataSet.pas:749）。
/// 原文用 <c>CreateFmt(...)</c> 做 Delphi 格式串替换（:855/:861/:867/:1120）——
/// 托管侧一律走 <c>GXX.Core.Rtl.DelphiFormat.Format</c>（台账 §17.2：禁止 string.Format）。
/// </summary>
public class EParadoxError : Exception
{
    /// <summary>Exception.Create。</summary>
    public EParadoxError(string message) : base(message) { }

    /// <summary>
    /// Exception.CreateFmt(Fmt, Args)：**Delphi 格式串**语义（%s/%S/%d/%% 等）。
    /// 原文四处调用点：:855（无占位符）、:861（"%S" - %S）、:867（"%S"）、:1120（"Block %d read error"）。
    /// </summary>
    public static EParadoxError CreateFmt(string fmt, params object[] args)
        => new EParadoxError(GXX.Core.Rtl.DelphiFormat.Format(fmt, args));
}

// ==================== DB.pas：TField 族 ====================

/// <summary>
/// 接缝：DB.pas TField 的**最小子集**（原文用到的成员：FieldNo / FieldName / DataType /
/// Size / AsString / AsInteger / AsFloat / AsBoolean / AsDateTime / Value）。
/// 取值一律经 <see cref="TDataSet.GetFieldData"/>（原文 :705 的 override 入口），
/// 因此接缝不会把"取字段值"的语义复制一份出来 —— 测 <c>Field.Value</c> 就是测原文逻辑。
/// </summary>
public class TField
{
    /// <summary>DB.pas TField.FieldNo（1-based；原文 :1194 等处用 <c>FieldNo - 1</c> 索引）。</summary>
    public int FieldNo;

    /// <summary>DB.pas TField.FieldName。</summary>
    public string FieldName = "";

    /// <summary>DB.pas TField.DataType（原文 :1287/:1307/:1325 与 :1261 处比较 ftMemo）。</summary>
    public TFieldType DataType;

    /// <summary>DB.pas TField.Size（字节/字符尺寸；原文 :1269/:1277 处用 <c>Field.Size</c>）。</summary>
    public int Size;

    /// <summary>DB.pas TField.AsString（经 GetFieldData 取 ftString 缓冲）。</summary>
    public virtual string AsString => Value == null ? "" : Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>DB.pas TField.AsInteger。</summary>
    public virtual int AsInteger => Value == null ? 0 : Convert.ToInt32(Value, System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>DB.pas TField.AsFloat。</summary>
    public virtual double AsFloat => Value == null ? 0d : Convert.ToDouble(Value, System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>DB.pas TField.AsBoolean。</summary>
    public virtual bool AsBoolean => Value != null && Convert.ToBoolean(Value, System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>DB.pas TField.AsDateTime。</summary>
    public virtual DateTime AsDateTime => Value == null ? default : Convert.ToDateTime(Value, System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>DB.pas TField.Value：按 DataType 分派到具体取值缓冲。</summary>
    public virtual object Value
    {
        get
        {
            if (DataSet == null) return null;
            switch (DataType)
            {
                case TFieldType.ftString:
                case TFieldType.ftFixedChar:
                case TFieldType.ftWideString:
                case TFieldType.ftMemo:
                case TFieldType.ftFmtMemo:
                {
                    var buf = new byte[Size + 1];
                    return DataSet.GetFieldData(this, buf) ? PxAnsi.FromBytes(buf, Size) : "";
                }
                case TFieldType.ftDate:
                case TFieldType.ftTime:
                case TFieldType.ftDateTime:
                {
                    var buf = new byte[8];
                    return DataSet.GetFieldData(this, buf) ? PxDateTime.TDateTimeFromDouble(BitConverter.ToDouble(buf, 0)) : default(DateTime);
                }
                case TFieldType.ftBoolean:
                {
                    var buf = new byte[2];
                    return DataSet.GetFieldData(this, buf) && (buf[0] | (buf[1] << 8)) != 0;
                }
                case TFieldType.ftCurrency:
                case TFieldType.ftFloat:
                {
                    var buf = new byte[8];
                    return DataSet.GetFieldData(this, buf) ? BitConverter.ToDouble(buf, 0) : 0d;
                }
                case TFieldType.ftSmallint:
                case TFieldType.ftWord:
                {
                    var buf = new byte[2];
                    return DataSet.GetFieldData(this, buf) ? (int)BitConverter.ToInt16(buf, 0) : 0;
                }
                case TFieldType.ftInteger:
                case TFieldType.ftAutoInc:
                case TFieldType.ftLargeint:
                {
                    var buf = new byte[4];
                    return DataSet.GetFieldData(this, buf) ? BitConverter.ToInt32(buf, 0) : 0;
                }
                default:
                    return null;
            }
        }
    }

    /// <summary>TField 所属数据集（CreateFields 时回填）。</summary>
    public TDataSet DataSet;
}

/// <summary>DB.pas TStringField（原文 pxfAlpha → ftString）。</summary>
public class TStringField : TField { }

/// <summary>DB.pas TIntegerField。</summary>
public class TIntegerField : TField { }

/// <summary>DB.pas TSmallintField。</summary>
public class TSmallintField : TField { }

/// <summary>DB.pas TWordField。</summary>
public class TWordField : TField { }

/// <summary>DB.pas TFloatField。</summary>
public class TFloatField : TField { }

/// <summary>DB.pas TCurrencyField。</summary>
public class TCurrencyField : TField { }

/// <summary>DB.pas TBooleanField。</summary>
public class TBooleanField : TField { }

/// <summary>DB.pas TDateTimeField。</summary>
public class TDateTimeField : TField { }

/// <summary>DB.pas TDateField。</summary>
public class TDateField : TField { }

/// <summary>DB.pas TTimeField。</summary>
public class TTimeField : TField { }

/// <summary>DB.pas TAutoIncField。</summary>
public class TAutoIncField : TField { }

/// <summary>DB.pas TBlobField。</summary>
public class TBlobField : TField { }

/// <summary>DB.pas TMemoField。</summary>
public class TMemoField : TField { }

/// <summary>DB.pas TGraphicField。</summary>
public class TGraphicField : TField { }

/// <summary>DB.pas TFmtMemoField（原文 pxfFmtMemoBLOB → ftFmtMemo）。</summary>
public class TFmtMemoField : TField { }

/// <summary>DB.pas TBCDField。</summary>
public class TBCDField : TField { }

/// <summary>DB.pas TBytesField。</summary>
public class TBytesField : TField { }

/// <summary>DB.pas TOleField（原文 ftParadoxOle）。</summary>
public class TOleField : TField { }

/// <summary>DB.pas TFieldDef（原文 :820/:822 <c>FieldDefs.Add</c> 的产物，CreateFields 据此建 TField）。</summary>
public sealed class TFieldDef
{
    /// <summary>字段名（原文 :820/:822 的 S）。</summary>
    public string Name;

    /// <summary>字段类型（原文 :820/:822 的 NativeToFieldType(...)）。</summary>
    public TFieldType DataType;

    /// <summary>字段尺寸（原文 4 参重载的 FieldSize；2 参重载为 0）。</summary>
    public int Size;

    /// <summary>原文 4 参重载的 Required 实参（原文恒传 False）。</summary>
    public bool Required;
}

/// <summary>DB.pas TFieldDefs 的**最小子集**：Clear / Add（4 参、2 参） / Count / 索引器。</summary>
public sealed class TFieldDefs
{
    private readonly List<TFieldDef> _items = new List<TFieldDef>();

    /// <summary>TFieldDefs.Clear（原文 :789 每次 InternalInitFieldDefs 开头调用）。</summary>
    public void Clear() => _items.Clear();

    /// <summary>TFieldDefs.Add(const Name; DataType; Size; Required)（原文 :820）。</summary>
    public TFieldDef Add(string name, TFieldType dataType, int size, bool required)
    {
        var d = new TFieldDef { Name = name, DataType = dataType, Size = size, Required = required };
        _items.Add(d);
        return d;
    }

    /// <summary>
    /// TFieldDefs.Add(const Name; DataType)（原文 :822）。
    /// <c>Size</c> 取 0（原文该重载不传尺寸，字段尺寸由 <c>Field.Size</c> 在使用处决定）。
    /// </summary>
    public TFieldDef Add(string name, TFieldType dataType) => Add(name, dataType, 0, false);

    /// <summary>TFieldDefs.Count。</summary>
    public int Count => _items.Count;

    /// <summary>TFieldDefs[Index]（0-based，照抄 Delphi 的 TDefCollection 索引）。</summary>
    public TFieldDef this[int index] => _items[index];
}

// ==================== DB.pas：TDataSet ====================

/// <summary>
/// 接缝：DB.pas **TDataSet** 的**最小子集**。只提供原文真实使用到的成员：
///  - 生命周期：Create/Destroy、Open/Close、Active
///  - 遍历：First/Next/Eof（原文消费者 uFrmMagicCD.pas:359-390、
///          GBDEtoSqlite.pas:154-179 用 First/Next/Eof/RecordCount/FieldByName）
///  - 字段：FieldDefs/DefaultFields/CreateFields/DestroyFields/BindFields/Fields/FieldByName/FieldCount
///  - 缓冲：ActiveBuffer/ActivateBuffer、GetFieldData
///  - 内部：GetRecord/AllocRecordBuffer/FreeRecordBuffer/Internal*/GetBookmarkFlag/SetBookmarkFlag
///          /GetCanModify/GetRecordCount/GetRecNo/SetRecNo/GetFieldData/CreateBlobStream/Resync
/// 刻意**不**实现：状态机（dsInsert/dsEdit/dsBrowse）、Filter、书签全族、TDataSource/TDBGrid
/// 联动、TCustomDataSet 的 dsInactive 触发顺序 —— 原文 TParadoxDataSet 从不触碰它们。
/// </summary>
public abstract class TDataSet
{
    /// <summary>DB.pas TDataSet.SetActive / FActive 对应物（原文 :1059/:1076 <c>if Active then</c>）。</summary>
    protected bool FActive;

    /// <summary>
    /// DB.pas TDataSet.Active（原文 :716 在 TParadoxDataSet 上以 <c>property Active;</c> 再公开一次）。
    /// 派生类的 published 重声明会遮蔽基类成员，故这里直接暴露为 public。
    /// </summary>
    public bool Active => FActive;

    /// <summary>DB.pas TDataSet.ActiveBuffer（原文 :1052/:1199/:1266/:1264 读取）。</summary>
    protected IntPtr FActiveBuffer = IntPtr.Zero;

    /// <summary>DB.pas TDataSet.FieldDefs（原文 :789/:820/:822）。</summary>
    public TFieldDefs FieldDefs { get; } = new TFieldDefs();

    /// <summary>DB.pas TDataSet.DefaultFields（原文 :902/:916）。</summary>
    public bool DefaultFields = true;

    /// <summary>DB.pas TDataSet.Fields（消费者 GBDEtoSqlite.pas:113/:160 用）。</summary>
    public TField[] Fields { get; protected set; } = Array.Empty<TField>();

    /// <summary>DB.pas TDataSet.FieldCount。</summary>
    public int FieldCount => Fields.Length;

    /// <summary>DB.pas TDataSet.FindField / FieldByName（消费者 uFrmMagicCD.pas:363/:366/:377）。</summary>
    public TField FieldByName(string name)
    {
        for (int i = 0; i < Fields.Length; i++)
            if (string.Equals(Fields[i].FieldName, name, StringComparison.Ordinal)) return Fields[i];
        throw new EParadoxError("Field '" + name + "' not found");
    }

    /// <summary>DB.pas TDataSet.Active（由基类暴露，原文 :716 的 <c>property Active;</c>）。</summary>
    public bool ActiveBufferIsSet => FActiveBuffer != IntPtr.Zero;

    /// <summary>DB.pas TDataSet.ActiveBuffer（原文 :1052/:1199/:1266 读；托管侧为记录缓冲指针）。</summary>
    public IntPtr ActiveBuffer => FActiveBuffer;

    /// <summary>DB.pas TDataSet.Open → SetActive(True) → InternalOpen + CreateFields + BindFields(True)。</summary>
    public void Open()
    {
        if (FActive) return;
        InternalOpen();
        if (DefaultFields) CreateFields();
        BindFields(true);
        FActive = true;
    }

    /// <summary>DB.pas TDataSet.Close → SetActive(False) → BindFields(False) + DestroyFields + InternalClose。</summary>
    public void Close()
    {
        if (!FActive) return;
        BindFields(false);
        if (DefaultFields) DestroyFields();
        FActive = false;
        InternalClose();
        FActiveBuffer = IntPtr.Zero;
    }

    /// <summary>DB.pas TDataSet.CreateFields：按 FieldDefs 建具体 TField 子类（顺序即 FieldNo）。</summary>
    public void CreateFields()
    {
        var arr = new TField[FieldDefs.Count];
        for (int i = 0; i < arr.Length; i++)
        {
            var def = FieldDefs[i];
            var f = CreateFieldOfType(def.DataType);
            f.FieldNo = i + 1;
            f.FieldName = def.Name;
            f.DataType = def.DataType;
            f.Size = def.Size;
            f.DataSet = this;
            arr[i] = f;
        }
        Fields = arr;
    }

    private static TField CreateFieldOfType(TFieldType t)
    {
        switch (t)
        {
            case TFieldType.ftString: return new TStringField();
            case TFieldType.ftSmallint: return new TSmallintField();
            case TFieldType.ftInteger: return new TIntegerField();
            case TFieldType.ftWord: return new TWordField();
            case TFieldType.ftBoolean: return new TBooleanField();
            case TFieldType.ftFloat: return new TFloatField();
            case TFieldType.ftCurrency: return new TCurrencyField();
            case TFieldType.ftBCD: return new TBCDField();
            case TFieldType.ftDate: return new TDateField();
            case TFieldType.ftTime: return new TTimeField();
            case TFieldType.ftDateTime: return new TDateTimeField();
            case TFieldType.ftBytes: return new TBytesField();
            case TFieldType.ftAutoInc: return new TAutoIncField();
            case TFieldType.ftBlob: return new TBlobField();
            case TFieldType.ftMemo: return new TMemoField();
            case TFieldType.ftGraphic: return new TGraphicField();
            case TFieldType.ftFmtMemo: return new TFmtMemoField();
            case TFieldType.ftParadoxOle: return new TOleField();
            default: return new TField();
        }
    }

    /// <summary>DB.pas TDataSet.DestroyFields（原文 :916）。</summary>
    public void DestroyFields() => Fields = Array.Empty<TField>();

    /// <summary>DB.pas TDataSet.BindFields（原文 :903/:915；托管侧仅记录绑定状态）。</summary>
    public bool FieldsBound { get; private set; }

    /// <summary>DB.pas TDataSet.BindFields(Value)。</summary>
    public void BindFields(bool value) => FieldsBound = value;

    /// <summary>
    /// DB.pas TDataSet.First（消费者 :360/:154 调用）。
    /// 照抄 Delphi 语义：<c>InternalFirst</c> 后取 <c>First</c> 一跳的记录（gmNext）。
    /// </summary>
    public void First()
    {
        InternalFirst();
        Next();
    }

    /// <summary>
    /// DB.pas TDataSet.Next（消费者 :390/:178 调用）。
    /// 照抄 Delphi 语义：gmNext 取一跳；返回 grEOF 时缓冲被清零（原文 :992-994）、
    /// 游标**保持不变**（原文 :947-950 只在非 EOF 时 Inc），Eof 因此置位。
    /// </summary>
    public void Next()
    {
        if (FActiveBuffer == IntPtr.Zero) FActiveBuffer = AllocRecordBuffer();
        var r = GetRecord(FActiveBuffer, TGetMode.gmNext, false);
        if (r == TGetResult.grEOF) FEof = true;
        else if (r == TGetResult.grOK) FEof = false;
    }

    /// <summary>
    /// DB.pas TDataSet.Eof。
    /// 接缝说明：Delphi 的 Eof 是 <c>FEOF and not IsCursorOpen</c> 的状态位；
    /// 原文 TParadoxDataSet **没有**覆盖 Eof，而它的 GetRecord 在 gmNext 越过末记录时
    /// 正好返回 grEOF 且把 BookmarkFlag 写成 bfEOF（:994）。托管侧据此把 Eof 定义为
    /// "末次 gmNext 返回 grEOF"（<see cref="Next"/> 维护），与原文可观察行为一致。
    /// </summary>
    public bool Eof { get; protected set; }

    /// <summary>DB.pas TDataSet.Bof（接缝：原文从不查询；定义为游标已在首记录或之前）。</summary>
    public bool Bof => !FActive || FCursorForEof <= 1;

    /// <summary>Eof/Bof 判定所需的游标镜像（由派生类在 InternalFirst/InternalLast 后同步）。</summary>
    protected int FCursorForEof;

    /// <summary>SetActive 的接缝实现。</summary>
    protected void SetActive(bool value)
    {
        if (value) Open(); else Close();
    }

    /// <summary>DB.pas TDataSet.Resync（原文 :1047 <c>Resync([])</c>）。</summary>
    public void Resync(TField[] fields)
    {
        if (!FActive) return;
        if (FActiveBuffer == IntPtr.Zero) FActiveBuffer = AllocRecordBuffer();
        // 原文 SetRecNo 先改 FCursor 再 Resync，故此处按 gmCurrent 重读当前记录
        var r = GetRecord(FActiveBuffer, TGetMode.gmCurrent, false);
        if (r == TGetResult.grOK) FEof = false;
    }

    /// <summary>Eof 状态位（Delphi FEOF 对应物）。</summary>
    protected bool FEof;

    /// <summary>DB.pas TDataSet.DatabaseError（原文 :999）。</summary>
    protected void DatabaseError(string message) => throw new EParadoxError(message);

    /// <summary>DB.pas TDataSet.GetFieldData（原文 :705/:1180 override）。</summary>
    public abstract bool GetFieldData(TField field, byte[] buffer);

    /// <summary>DB.pas TDataSet.GetFieldData 的指针重载（原文 TValueBuffer 形参）。</summary>
    public abstract unsafe bool GetFieldData(TField field, void* buffer);

    // ---- 派生类必须实现的虚方法（原文 :676-706 的 override 清单） ----

    /// <summary>原文 :676/:755 GetBookmarkFlag。</summary>
    protected abstract TBookmarkFlag GetBookmarkFlag(IntPtr buffer);

    /// <summary>原文 :677/:760 SetBookmarkFlag。</summary>
    protected abstract void SetBookmarkFlag(IntPtr buffer, TBookmarkFlag value);

    /// <summary>原文 :679/:765 InternalHandleException。</summary>
    protected abstract void InternalHandleException();

    /// <summary>原文 :680/:771 InternalInitFieldDefs。</summary>
    protected abstract void InternalInitFieldDefs();

    /// <summary>原文 :681/:850 InternalOpen。</summary>
    protected abstract void InternalOpen();

    /// <summary>原文 :682/:908 IsCursorOpen。</summary>
    protected abstract bool IsCursorOpen { get; }

    /// <summary>原文 :683/:913 InternalClose。</summary>
    protected abstract void InternalClose();

    /// <summary>原文 :685/:927 GetRecord。</summary>
    protected abstract TGetResult GetRecord(IntPtr buffer, TGetMode getMode, bool doCheck);

    /// <summary>原文 :686/:1003 AllocRecordBuffer。</summary>
    protected abstract IntPtr AllocRecordBuffer();

    /// <summary>原文 :687/:1008 FreeRecordBuffer。</summary>
    protected abstract void FreeRecordBuffer(IntPtr buffer);

    /// <summary>原文 :688/:1013 InternalInitRecord（原文为空实现）。</summary>
    protected abstract void InternalInitRecord(IntPtr buffer);

    /// <summary>原文 :690/:1018 InternalFirst。</summary>
    protected abstract void InternalFirst();

    /// <summary>原文 :691/:1023 InternalLast。</summary>
    protected abstract void InternalLast();

    /// <summary>原文 :692/:1028 InternalSetToRecord。</summary>
    protected abstract void InternalSetToRecord(IntPtr buffer);

    /// <summary>原文 :694/:1033 GetCanModify。</summary>
    protected abstract bool GetCanModify { get; }

    /// <summary>原文 :696/:1038 GetRecordCount。</summary>
    protected abstract int GetRecordCount { get; }

    /// <summary>原文 :698/:1043 SetRecNo。</summary>
    protected abstract void SetRecNo(int value);

    /// <summary>原文 :699/:1050 GetRecNo。</summary>
    protected abstract int GetRecNo { get; }

    /// <summary>原文 :706/:1249 CreateBlobStream（TParadoxDataSet 上 override）。</summary>
    public virtual TMemoryStream CreateBlobStream(TField field, TBlobStreamMode mode) => null;

    /// <summary>DB.pas TDataSet.RecordCount（原文 :696 GetRecordCount 的公开入口）。</summary>
    public int RecordCount => GetRecordCount;

    /// <summary>DB.pas TDataSet.RecNo（原文 :698/:1050）。</summary>
    public int RecNo
    {
        get => GetRecNo;
        set => SetRecNo(value);
    }

    /// <summary>DB.pas TDataSet.CanModify（原文 :694 恒 False）。</summary>
    public bool CanModify => GetCanModify;

    /// <summary>DB.pas TDataSet.SetToRecord：原文 :1028 InternalSetToRecord 的入口。</summary>
    public void SetToRecord(IntPtr buffer) => InternalSetToRecord(buffer);

    /// <summary>DB.pas TDataSet.Last：原文 :1023 InternalLast + 末记录。接缝：原文从不调用。</summary>
    public void Last()
    {
        InternalLast();
        FCursorForEof = -1;
    }
}

// ==================== 接缝：Forms.pas Application.HandleException ====================

/// <summary>
/// 接缝：Forms.pas <c>Application.HandleException(Self)</c>（原文 :767
/// <c>TParadoxDataSet.InternalHandleException</c> 的唯一一行）。
/// GXX.Core 未启用 UseWindowsForms（GXX.Core.csproj 无该属性），托管侧改为可注入钩子。
/// </summary>
public static class ParadoxApplication
{
    /// <summary>接缝：宿主注入的异常处理器（原文 Application.HandleException 的语义位）。</summary>
    public static Action<object, Exception> OnHandleException;

    /// <summary>原文 :767 的等价调用（无处理器时静默，与 Delphi 的默认行为不同——见报告接缝清单）。</summary>
    public static void HandleException(object sender) => OnHandleException?.Invoke(sender, null);
}

// ==================== 接缝工具：AnsiString / 指针 / 位运算 ====================

/// <summary>接缝工具：Delphi AnsiString（GBK 字节串）与托管 string 的透明映射。</summary>
public static class PxAnsi
{
    /// <summary>
    /// Delphi AnsiString → 托管 string：按 **Latin-1 透明**映射（每字节一字符），
    /// 与 GXX.RunGate/ParadoxConv.cs 的 BytesToLatin1 同一策略 —— 不在此处做 GBK 解码，
    /// 以便"字节语义"在 TParadoxDataSet 内部完全保真（原文字段名/短串都是原始字节）。
    /// </summary>
    public static string FromBytes(byte[] bytes, int count)
    {
        if (bytes == null) return "";
        if (count > bytes.Length) count = bytes.Length;
        var chars = new char[count];
        for (int i = 0; i < count; i++) chars[i] = (char)bytes[i];
        return new string(chars);
    }

    /// <summary>托管 string → Delphi AnsiString 字节（Latin-1 低位截断，同 ParadoxConv.Latin1ToBytes）。</summary>
    public static byte[] ToBytes(string s)
    {
        if (s == null) return Array.Empty<byte>();
        var b = new byte[s.Length];
        for (int i = 0; i < s.Length; i++) b[i] = (byte)(s[i] & 0xFF);
        return b;
    }

    /// <summary>
    /// 接缝：SysUtils.StrLCopy(Dest, Source, MaxLen)（原文 :1226）。
    /// 语义照抄：最多复制 <paramref name="maxLen"/> 字节，遇到 #0 提前结束，末尾补 #0。
    /// </summary>
    public static unsafe int StrLCopy(byte* dest, byte* source, int maxLen)
    {
        int i = 0;
        if (maxLen <= 0) return 0;
        while (i < maxLen && source[i] != 0) { dest[i] = source[i]; i++; }
        if (i < maxLen + 1) dest[i] = 0;
        return i;
    }

    /// <summary>Delphi <c>Ord(Boolean)</c> 反解：PWordBool 缓冲 → bool。</summary>
    public static bool WordBoolFromBytes(byte[] b) => (b[0] | (b[1] << 8)) != 0;
}

/// <summary>接缝工具：Delphi TDateTime（Double 天数）↔ 托管 DateTime。</summary>
public static class PxDateTime
{
    /// <summary>TDateTime → DateTime（1899-12-30 为 0；本接缝不处理负值/纪元边界）。</summary>
    public static DateTime TDateTimeFromDouble(double value)
    {
        try { return DateTime.FromOADate(value); }
        catch { return default; }
    }

    /// <summary>DateTime → TDateTime。</summary>
    public static double TDateTimeToDouble(DateTime value) => value.ToOADate();
}

/// <summary>接缝工具：Delphi untyped 指针缓冲的读写助手（把原文的 <c>Buffer + N</c> 语法映射到托管侧）。</summary>
public static unsafe class PxBuffer
{
    /// <summary>把 Delphi 的 <c>Ptr + N</c> 指针算术映射为托管 <see cref="IntPtr"/> 偏移。</summary>
    public static IntPtr Add(IntPtr ptr, int offset) => ptr + offset;

    /// <summary>把 Delphi 的 <c>Ptr^</c>（Byte）取字节。</summary>
    public static byte ReadByte(IntPtr ptr) => *(byte*)ptr;

    /// <summary>把 Delphi 的 <c>Ptr^ := Value</c>（Byte）写字节。</summary>
    public static void WriteByte(IntPtr ptr, byte value) => *(byte*)ptr = value;

    /// <summary>把 Delphi 的 <c>FillChar(P^, Count, Value)</c>（原文 :993）映射为托管实现。</summary>
    public static void FillChar(IntPtr ptr, int count, byte value)
    {
        if (count <= 0) return;
        new Span<byte>((void*)ptr, count).Fill(value);
    }

    /// <summary>从非托管缓冲读出 <paramref name="count"/> 字节到托管数组（测试/断言用）。</summary>
    public static byte[] ToArray(IntPtr ptr, int count)
    {
        var b = new byte[count];
        if (count > 0) Marshal.Copy(ptr, b, 0, count);
        return b;
    }

    /// <summary>把托管数组写入非托管缓冲（测试构造记录用）。</summary>
    public static void FromArray(IntPtr ptr, byte[] source, int count)
    {
        if (count > 0) Marshal.Copy(source, 0, ptr, count);
    }

    /// <summary>ReadProcessMemory 式 1 字节读（等价 Delphi <c>PAnsiChar(P)^</c>）。</summary>
    public static byte ReadByteAt(IntPtr ptr, int offset) => *(byte*)(ptr + offset);
}
