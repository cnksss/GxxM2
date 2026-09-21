using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GXX.Core.Rtl;
using GXX.Core.Util;

// 源单元：Source/Common/MemoryIniFiles.pas（888 行）→ 本文件（1:1 移植）
//
// 归属裁决（派发要求 #2「先判是否已被取代 / 死代码」）——结论 = **② 仍有引用 + 有独立语义 ⇒ 1:1 移植**：
//   · `uses` 命中：Source/M2Engine/NpcActionCmd.pas:8 `MemoryIniFiles, …`（**唯一**引用点，实活代码）；
//   · 调用点命中：NpcActionCmd.pas:19215/19255、:19339/:19374、:19467/:19502
//     三处 `MemoryIniFile := TMemoryIniFile.Create(LoadList);`（ActionOfSortVarToList 等 3 个脚本动作）；
//   · `.dpr/.dproj/.dpk` 命中 0（`inDpr=False` 属实）—— 但它由 NpcActionCmd.pas **间接**编入 M2Server.dpr；
//   · 与 `GXX.Core/Util/FastIniFile.cs`（`FastIniFile.pas`，86,353 字节）**不是同一单元、也不重叠**：
//       FastIniFile = 文件后端（`TFastIniFile = class(TCustomIniFile)`，字典存储 + 编码嗅探 + 写盘）；
//       本单元     = 内存后端（**从 TStrings / 文本 / 裸字节缓冲构造**，节表是 `TQuickSortList` 快排 + 二分查节，
//                    值表是 `TValueList`/`TIniValueList` 的有序插入，另有 `SaveToList` 与 `OnChange` 通知）。
//     两者的公开面交集只有 Read/Write(String|Integer|Bool) 的**语义约定**（`WriteBool` = '1'/'0'、
//     `ReadInteger` 的 `0x→$` 改写），不存在"同一能力的第二份实现"⇒ 不违反 §14.2。
//
// 差异/接缝登记见 docs/并行报告-p10-m2-misc.md（D-P10-01…）。

namespace GXX.Core.MemoryIni;

/// <summary>
/// Delphi `SysUtils.EConvertError` 的托管等价。
///
/// <para>
/// 接缝（登记 D-P10-01）：本单元 8 个 <c>Read*</c>（Date/DateTime/Float/Time × string/Integer 节重载）
/// 都写成 <c>try … except on EConvertError do {忽略} else raise; end</c>，
/// 必须要有一个**按类型**可捕获的异常；`GXX.Core.Rtl`（分区外，本车道不可改）目前没有
/// `EConvertError`，也没有 `StrToDate/StrToDateTime/StrToFloat/StrToTime`。
/// 故本单元自带最小实现，**只服务本单元**，待 `GXX.Core.Rtl` 补齐后改调那里并删除本类。
/// </para>
/// </summary>
public class EConvertError : Exception
{
    public EConvertError(string message) : base(message) { }
}

/// <summary>
/// 本单元用到的 Delphi RTL 文本/数值转换（SysUtils）最小实现。
/// 逐条对应原文调用点，故意**不**做"宽松兜底"（失败一律抛 <see cref="EConvertError"/>，
/// 与原 `StrToDate/StrToDateTime/StrToFloat/StrToTime` 一致）。
///
/// 偏离 D-P10-02：原文按**当前区域设置**（`ShortDateFormat`/`DateSeparator`/`DecimalSeparator`）解析，
/// 结果随机器变化、不可复现；本实现固定 `InvariantCulture`（与 `TIniFixedDateTime.StrToDateDef`
/// 的 D-P8-4 同一口径）。往返用例只使用 `yyyy-MM-dd` / `HH:mm:ss` / `G17` 可解析形态。
/// </summary>
internal static class MemoryIniRtl
{
    // TDateTime = OLE 自动化双精度天数（1899-12-30 为 0）；GXX.Core 无统一垫片，
    // 直接用 DateTime.ToOADate（与 Util/FastIniFile.cs:315/:329 的既有口径一致）。
    public static double StrToDate(string s)
    {
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            return dt.Date.ToOADate();
        throw new EConvertError("'" + s + "' is not a valid date");
    }

    public static double StrToDateTime(string s)
    {
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            return dt.ToOADate();
        throw new EConvertError("'" + s + "' is not a valid date and time");
    }

    public static double StrToTime(string s)
    {
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            return dt.TimeOfDay.TotalDays;
        throw new EConvertError("'" + s + "' is not a valid time");
    }

    public static double StrToFloat(string s)
    {
        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
            return v;
        throw new EConvertError("'" + s + "' is not a valid floating point value");
    }

    /// <summary>
    /// Delphi `SysUtils.StrToIntDef`：**支持 `'$'` 十六进制前缀**（内部走 `Val`），
    /// 否则十进制，失败返回 Def。
    ///
    /// <para>
    /// 为什么不直接用 <see cref="DelphiRTL.StrToIntDef"/>：后者用
    /// `long.TryParse(NumberStyles.Integer)` ⇒ **不认 `'$'`**，会让
    /// `ReadInteger` 的 `'0x1F' → '$1F'` 改写结果恒回退默认值（真实保真缺口，登记 D-P10-03）。
    /// 原文依据：MemoryIniFiles.pas:657-666 / :723-732 的改写分支本身就说明后续 `StrToIntDef` 认 `'$'`。
    /// </para>
    /// </summary>
    public static int StrToIntDef(string s, int def)
    {
        s = (s ?? "").Trim();
        if (s.Length == 0) return def;
        try
        {
            if (s[0] == '$')
            {
                return s.Length > 1
                    ? Convert.ToInt32(s.Substring(1), 16)
                    : def;
            }
            if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && s.Length > 2)
                return Convert.ToInt32(s.Substring(2), 16);
            long v = long.Parse(s, NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            return v is >= int.MinValue and <= int.MaxValue ? (int)v : def;
        }
        catch (FormatException) { return def; }
        catch (OverflowException) { return def; }
        catch (ArgumentException) { return def; }
    }
}

/// <summary>
/// Delphi `AnsiCompareStr` / `AnsiCompareText`（按 **ANSI/GBK 字节**比较，不是 UTF-16 码元序）。
///
/// <para>
/// 本单元所有节名/键名的比较都走 `CompareStr`/`CompareText`；托管字符串是 UTF-16，
/// 直接用 `string.CompareOrdinal` 会在非 ASCII 名上给出与原文不同的**排序**（进而改变
/// `SortString` 的快排结果与二分查找是否命中）。故按 GBK 字节比较（纯 ASCII 名两者等价）。
/// </para>
/// </summary>
internal static class DelphiAnsiCompare
{
    private static byte[] Bytes(string s, bool caseSensitive)
    {
        string t = caseSensitive ? (s ?? "") : (s ?? "").ToUpperInvariant();
        return EncodingInit.GBK.GetBytes(t);
    }

    private static int Compare(string a, string b, bool caseSensitive)
    {
        byte[] x = Bytes(a, caseSensitive);
        byte[] y = Bytes(b, caseSensitive);
        int n = Math.Min(x.Length, y.Length);
        for (int i = 0; i < n; i++)
            if (x[i] != y[i]) return x[i] < y[i] ? -1 : 1;
        if (x.Length == y.Length) return 0;
        return x.Length < y.Length ? -1 : 1;
    }

    /// <summary>Delphi `SysUtils.CompareStr`（大小写敏感）。</summary>
    public static int CompareStr(string a, string b) => Compare(a, b, caseSensitive: true);

    /// <summary>Delphi `SysUtils.CompareText`（大小写不敏感）。</summary>
    public static int CompareText(string a, string b) => Compare(a, b, caseSensitive: false);
}

/// <summary>
/// 值表的查找算法选择。
///
/// <para>
/// ★ 原文有**两条**值表实现，`TMemoryIniFile` 同时用到（<c>MemoryIniFiles.pas:24</c> 与 <c>:649</c>）：
/// </para>
/// <list type="bullet">
/// <item><c>TIniValueList</c>（<c>:24-27</c> + <c>:84-97</c>）：**重写** `GetIndex` 为
///   线性 + `CompareStr`（**大小写敏感**、与列表是否有序无关）—— 由 `Get`（解析）创建；</item>
/// <item><c>TValueList</c>（`SDK.pas:113`，`GetIndex` 见 `SDK.pas:702-792`）：**二分**查找，
///   `Sorted=True` 用 `CompareStr`、否则用 `CompareText` —— 由 `WriteString` 在"节不存在"时创建。</item>
/// </list>
/// <para>
/// 托管侧 `GXX.Core.Protocol.TValueList`（`SDK.pas` 的既有权宜移植）**没有值写入口**
/// （`Strings[i] := …` 无从表达）、`GetIndex` 也非虚，而该文件在分区外不可改
/// ⇒ 本单元用一个类 + 本枚举还原两条查找路径，**不新建第二个 `TValueList`**（§14.2）。
/// 两套查找在"键名大小写不同"时结论相反，已用差异断言锁死（见测试）。
/// </para>
/// </summary>
public enum TIniValueLookup
{
    /// <summary>`TIniValueList.GetIndex`（MemoryIniFiles.pas:84-97）：线性 + CompareStr。</summary>
    LinearCaseSensitive = 0,

    /// <summary>`TValueList.GetIndex`（SDK.pas:702-792）：二分；Sorted→CompareStr，否则 CompareText。</summary>
    Binary = 1,
}

/// <summary>
/// `MemoryIniFiles.pas` 的值表（等价于 `TValueList` / `TIniValueList` 两者在本单元用到的并集）。
/// 存储 = (Name, Value, Object) 三元组，与 `SDK.pas:102-107 TValueItem` 同构。
/// </summary>
public class TIniValueList
{
    private sealed class TValueItem
    {
        public string Name = "";
        public string Value = "";
        public object? Obj;
    }

    private readonly List<TValueItem> FList = new();

    /// <summary>原文 `TValueList.FSorted`（`SDK.pas:118/166`；`SetSorted` 见 `:647-654`：置 True 时先排序）。</summary>
    private bool FSorted;

    /// <summary>原文 `TValueList.FCaseSensitive`（`SDK.pas:120/167`）。</summary>
    public bool CaseSensitive { get; set; }

    /// <summary>查找路径选择（默认 = `TIniValueList` 的 override 语义）。</summary>
    public TIniValueLookup Lookup = TIniValueLookup.LinearCaseSensitive;

    /// <summary>原文 `TValueList.OnChange`（`SDK.pas:171`）。</summary>
    public Action<TIniValueList>? OnChange;

    public int Count => FList.Count;

    /// <summary>Delphi `TValueList.Sorted`（`SDK.pas:647-654`）：置 True 时按 `Names` 排序（CompareStr）。</summary>
    public bool Sorted
    {
        get => FSorted;
        set
        {
            if (FSorted != value)
            {
                if (value) Sort();
                FSorted = value;
            }
        }
    }

    /// <summary>Delphi `Names[Index]`（`SDK.pas:169` 的默认属性；`GetName` = `:524-528`）。</summary>
    public string Names(int Index) => FList[Index].Name;

    /// <summary>Delphi `Strings[Index]`（`SDK.pas:502-506 Get`）。</summary>
    public string Strings(int Index) => FList[Index].Value;

    /// <summary>
    /// Delphi `Strings[Index] := Value`（`SDK.pas:586-593 Put`：`Changing` → 写 → `Changed`）。
    /// 注：原文在 `Sorted=True` 时会先 `Error(@SSortedListError)`；本单元从不在 Sorted=True 下写入
    /// ⇒ 该守卫未复刻（登记 D-P10-06）。
    /// </summary>
    public void SetStrings(int Index, string Value)
    {
        FList[Index].Value = Value;
        RaiseChanged();
    }

    /// <summary>Delphi `Objects[Index]`（`SDK.pas:518-522`）。</summary>
    public object? Objects(int Index) => FList[Index].Obj;

    private void RaiseChanged() => OnChange?.Invoke(this);

    /// <summary>Delphi `TValueList.Sort`（`SDK.pas:670-674`）：`QuickSort` + `Changed`。</summary>
    public void Sort()
    {
        // 原文走 QuickSort(0, FCount-1, StringListCompareStrings)：按 **S（值）** 比较。
        // 见 SDK.pas:664-668 StringListCompareStrings → TValueList.CompareStrings（CaseSensitive 决定）。
        // 与 C# 的稳定排序结果在等值元素上可能不同，但 Sort 在本单元内只在用户显式置 Sorted=True 时触发。
        FList.Sort((x, y) => CompareStrings(x.Value, y.Value));
        RaiseChanged();
    }

    private int CompareStrings(string s1, string s2)
        => CaseSensitive ? DelphiAnsiCompare.CompareStr(s1, s2) : DelphiAnsiCompare.CompareText(s1, s2);

    /// <summary>
    /// `TValueList.GetIndex`（`SDK.pas:702-792`）1:1（二分版）。
    /// 逐分支保留原文的 `nLow/nHigh/nMed` 推进与"相邻即比对两端"的收口条件。
    /// </summary>
    private int GetIndexBinary(string AName)
    {
        int Result = -1;
        if (Count != 0)
        {
            if (Sorted)
            {
                if (Count == 1)
                {
                    if (DelphiAnsiCompare.CompareStr(AName, Names(0)) == 0) Result = 0;
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            if (DelphiAnsiCompare.CompareStr(AName, Names(nHigh)) == 0) Result = nHigh;
                            if (DelphiAnsiCompare.CompareStr(AName, Names(nLow)) == 0) Result = nLow;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareStr(AName, Names(nMed));
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = nMed;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Count == 1)
                {
                    if (DelphiAnsiCompare.CompareText(AName, Names(0)) == 0) Result = 0;
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            if (DelphiAnsiCompare.CompareText(AName, Names(nHigh)) == 0) Result = nHigh;
                            if (DelphiAnsiCompare.CompareText(AName, Names(nLow)) == 0) Result = nLow;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareText(AName, Names(nMed));
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = nMed;
                            break;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// `TIniValueList.GetIndex`（`MemoryIniFiles.pas:84-97`）1:1：**线性 + CompareStr**（大小写敏感），
    /// 与 `Self.Sorted` 无关、与 `CaseSensitive` 也无关（原文硬编码 `CompareStr`）。
    /// </summary>
    private int GetIndexLinear(string AName)
    {
        int Result = -1;
        for (int I = 0; I <= Count - 1; I++)
        {
            if (DelphiAnsiCompare.CompareStr(AName, Names(I)) == 0)
            {
                Result = I;
                break;
            }
        }
        return Result;
    }

    /// <summary>Delphi `TValueList.GetIndex`（virtual）→ 本类按 <see cref="Lookup"/> 分派。</summary>
    public int GetIndex(string AName)
        => Lookup == TIniValueLookup.Binary ? GetIndexBinary(AName) : GetIndexLinear(AName);

    /// <summary>Delphi `TValueList.AddObject(AName, S, AObject)`（`SDK.pas:398-408`）：不排序则追加。</summary>
    public int AddObject(string AName, string S, object? AObject)
    {
        int Result = FList.Count;
        FList.Add(new TValueItem { Name = AName, Value = S, Obj = AObject });
        RaiseChanged();
        return Result;
    }

    /// <summary>Delphi `TValueList.InsertObject`（`SDK.pas:556-562`）→ 在 Index 处插入。</summary>
    public void InsertObject(int Index, string AName, string S, object? AObject)
    {
        FList.Insert(Index, new TValueItem { Name = AName, Value = S, Obj = AObject });
        RaiseChanged();
    }

    /// <summary>Delphi `TValueList.Delete`（`SDK.pas:434-…`）。</summary>
    public void Delete(int Index)
    {
        FList.RemoveAt(Index);
        RaiseChanged();
    }

    /// <summary>Delphi `TValueList.Clear`（`SDK.pas:422-432`）：仅非空时通知。</summary>
    public void Clear()
    {
        if (FList.Count != 0)
        {
            FList.Clear();
            RaiseChanged();
        }
    }

    /// <summary>
    /// `TValueList.AddRecord(AName, S, Value)`（`SDK.pas:837-…`）1:1 —— 按 `Sorted` 选择
    /// `CompareStr` / `CompareText` 做**二分插入**，键已存在则返回 False 且**不覆盖**。
    /// 本单元两条路径（`TQuickSortList` 的节表用的同名算法见 `MemoryIniFiles.pas:223-377`）都用它。
    /// </summary>
    public bool AddRecord(string AName, string S)
    {
        bool Result = true;
        if (Count == 0)
        {
            AddObject(AName, S, null);
        }
        else
        {
            if (Sorted)
            {
                if (Count == 1)
                {
                    // ★ 原文如此（SDK.pas:850-859）：Count=1 且**同名**时既不插入、**也不置 Result := False**
                    //   ⇒ 返回 True 但列表未变（Count>1 的重复路径才返回 False）。照抄 + 差异断言锁定。
                    int nMed = DelphiAnsiCompare.CompareStr(AName, Names(0));
                    if (nMed > 0)
                        AddObject(AName, S, null);
                    else if (nMed < 0)
                        InsertObject(0, AName, S, null);
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            nMed = DelphiAnsiCompare.CompareStr(AName, Names(nHigh));
                            if (nMed > 0)
                            {
                                InsertObject(nHigh + 1, AName, S, null);
                                break;
                            }
                            nMed = DelphiAnsiCompare.CompareStr(AName, Names(nLow));
                            if (nMed > 0)
                            {
                                InsertObject(nLow + 1, AName, S, null);
                                break;
                            }
                            if (nMed < 0)
                            {
                                InsertObject(nLow, AName, S, null);
                                break;
                            }
                            Result = false;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareStr(AName, Names(nMed));
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Count == 1)
                {
                    // ★ 同 Sorted 分支：Count=1 且同名时返回 True 但不插入（原文如此，SDK.pas:921-930）
                    int nMed = DelphiAnsiCompare.CompareText(AName, Names(0));
                    if (nMed > 0)
                        AddObject(AName, S, null);
                    else if (nMed < 0)
                        InsertObject(0, AName, S, null);
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            nMed = DelphiAnsiCompare.CompareText(AName, Names(nHigh));
                            if (nMed > 0)
                            {
                                InsertObject(nHigh + 1, AName, S, null);
                                break;
                            }
                            nMed = DelphiAnsiCompare.CompareText(AName, Names(nLow));
                            if (nMed > 0)
                            {
                                InsertObject(nLow + 1, AName, S, null);
                                break;
                            }
                            if (nMed < 0)
                            {
                                InsertObject(nLow, AName, S, null);
                                break;
                            }
                            Result = false;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareText(AName, Names(nMed));
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = false;
                            break;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>Delphi `TStrings.Free`（`MemoryIniFiles.pas:464/:482` 的释放点）。</summary>
    public void Free() { }
}

/// <summary>
/// `MemoryIniFiles.pas:7-22 TQuickSortList = class(TStringList)` 1:1。
///
/// <para>
/// 节表：存节名（`Strings[]`），`Objects[]` 挂值表。差异只在比较函数与**手工快排**
/// （`SortString`，而不是继承的 `Sort`），并在 `OnChange` 上向 `TMemoryIniFile.Changed` 冒泡。
/// </para>
/// <para>
/// 偏离 D-P10-04：原文的 `OnChange` 由 `TStrings.Changed` 在基类内部触发；托管基类
/// <see cref="TStringList"/>（分区外）方法非虚、无 `OnChange`，故本类用 `new` **遮蔽**全部变更方法，
/// 在转调基类后自行触发。签名与触发时机按 `TStrings` 对齐；`Sort()`（继承）不触发，原文亦然。
/// </para>
/// </summary>
public class TQuickSortList : TStringList
{
    private readonly object CriticalSection = new();

    /// <summary>`MemoryIniFiles.pas:418/:426/:573/:650` 的 `OnChange := Changed` 落点。</summary>
    public Action<object>? OnChange;

    private void RaiseChanged() => OnChange?.Invoke(this);

    // ---- 遮蔽变更方法以复刻 TStrings.Changed（见 D-P10-04） ----

    public new int Add(string s)
    {
        int r = base.Add(s);
        RaiseChanged();
        return r;
    }

    public new int AddObject(string s, object? obj)
    {
        int r = base.AddObject(s, obj);
        RaiseChanged();
        return r;
    }

    public new void Insert(int index, string s)
    {
        base.Insert(index, s);
        RaiseChanged();
    }

    public new void InsertObject(int index, string s, object? obj)
    {
        base.InsertObject(index, s, obj);
        RaiseChanged();
    }

    public new void Delete(int index)
    {
        base.Delete(index);
        RaiseChanged();
    }

    public new void Exchange(int i, int j)
    {
        base.Exchange(i, j);
        RaiseChanged();
    }

    public new void Clear()
    {
        bool wasEmpty = Count == 0;
        base.Clear();
        if (!wasEmpty) RaiseChanged();
    }

    // ---- 原文成员 ----

    /// <summary>`MemoryIniFiles.pas:20-21` 的 `boCaseSensitive`（`Get/SetCaseSensitive` 见 `:379-387`）。</summary>
    public bool boCaseSensitive
    {
        get => CaseSensitive;
        set => CaseSensitive = value;
    }

    /// <summary>
    /// `MemoryIniFiles.pas:101-189 TQuickSortList.GetIndex` 1:1（二分查找节名）。
    /// `Sorted=False`（本单元的实际取值）→ 走 `CompareText` 分支；两条分支都保留。
    /// </summary>
    public int GetIndex(string S)
    {
        int Result = -1;
        if (Count != 0)
        {
            if (Sorted)
            {
                if (Count == 1)
                {
                    if (DelphiAnsiCompare.CompareStr(S, this[0]) == 0) Result = 0;
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            if (DelphiAnsiCompare.CompareStr(S, this[nHigh]) == 0) Result = nHigh;
                            if (DelphiAnsiCompare.CompareStr(S, this[nLow]) == 0) Result = nLow;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareStr(S, this[nMed]);
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = nMed;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Count == 1)
                {
                    if (DelphiAnsiCompare.CompareText(S, this[0]) == 0) Result = 0;
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            if (DelphiAnsiCompare.CompareText(S, this[nHigh]) == 0) Result = nHigh;
                            if (DelphiAnsiCompare.CompareText(S, this[nLow]) == 0) Result = nLow;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareText(S, this[nMed]);
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = nMed;
                            break;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// `MemoryIniFiles.pas:191-221 TQuickSortList.SortString` 1:1（Houre 快排的原地变体）。
    /// 逐字保留原文的 `s18 := Strings[(nMIN + nMax) shr 1]`（枢轴一次取定）与
    /// 末尾 `nMIN := ntMin; if ntMin >= nMax then Break` 的循环收口。
    /// </summary>
    public void SortString(int nMIN, int nMax)
    {
        if (Count > 0)
        {
            while (true)
            {
                int ntMin = nMIN;
                int ntMax = nMax;
                string s18 = this[(nMIN + nMax) >> 1];
                while (true)
                {
                    while (DelphiAnsiCompare.CompareText(this[ntMin], s18) < 0)
                        ntMin++;
                    while (DelphiAnsiCompare.CompareText(this[ntMax], s18) > 0)
                        ntMax--;
                    if (ntMin <= ntMax)
                    {
                        Exchange(ntMin, ntMax);
                        ntMin++;
                        ntMax--;
                    }
                    if (ntMin > ntMax)
                        break;
                }
                if (nMIN < ntMax) SortString(nMIN, ntMax);
                nMIN = ntMin;
                if (ntMin >= nMax) break;
            }
        }
    }

    /// <summary>
    /// `MemoryIniFiles.pas:223-377 TQuickSortList.AddRecord` 1:1：
    /// 按 `Sorted` 选 `CompareStr`/`CompareText` 做二分插入；`AObject` 为节表的值表实例。
    /// 键已存在时返回 False 且不插入（原文字典序去重）。
    /// </summary>
    public bool AddRecord(string S, object? AObject)
    {
        bool Result = true;
        if (Count == 0)
        {
            AddObject(S, AObject);
        }
        else
        {
            if (Sorted)
            {
                if (Count == 1)
                {
                    // ★ 原文如此（MemoryIniFiles.pas:236-245）：Count=1 且**同名**时既不插入、
                    //   **也不置 Result := False** ⇒ 返回 True 但列表未变（Count>1 的重复路径才返回 False）。
                    int nMed = DelphiAnsiCompare.CompareStr(S, this[0]);
                    if (nMed > 0)
                        AddObject(S, AObject);
                    else if (nMed < 0)
                        InsertObject(0, S, AObject);
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            nMed = DelphiAnsiCompare.CompareStr(S, this[nHigh]);
                            if (nMed > 0)
                            {
                                InsertObject(nHigh + 1, S, AObject);
                                break;
                            }
                            nMed = DelphiAnsiCompare.CompareStr(S, this[nLow]);
                            if (nMed > 0)
                            {
                                InsertObject(nLow + 1, S, AObject);
                                break;
                            }
                            if (nMed < 0)
                            {
                                InsertObject(nLow, S, AObject);
                                break;
                            }
                            Result = false;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareStr(S, this[nMed]);
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Count == 1)
                {
                    // ★ 同 Sorted 分支：Count=1 且同名时返回 True 但不插入（原文如此，MemoryIniFiles.pas:307-316）
                    int nMed = DelphiAnsiCompare.CompareText(S, this[0]);
                    if (nMed > 0)
                        AddObject(S, AObject);
                    else if (nMed < 0)
                        InsertObject(0, S, AObject);
                }
                else
                {
                    int nLow = 0;
                    int nHigh = Count - 1;
                    int nMed = (nHigh - nLow) / 2 + nLow;
                    while (true)
                    {
                        if ((nHigh - nLow) == 1)
                        {
                            nMed = DelphiAnsiCompare.CompareText(S, this[nHigh]);
                            if (nMed > 0)
                            {
                                InsertObject(nHigh + 1, S, AObject);
                                break;
                            }
                            nMed = DelphiAnsiCompare.CompareText(S, this[nLow]);
                            if (nMed > 0)
                            {
                                InsertObject(nLow + 1, S, AObject);
                                break;
                            }
                            if (nMed < 0)
                            {
                                InsertObject(nLow, S, AObject);
                                break;
                            }
                            Result = false;
                            break;
                        }
                        else
                        {
                            int nCompareVal = DelphiAnsiCompare.CompareText(S, this[nMed]);
                            if (nCompareVal > 0)
                            {
                                nLow = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            if (nCompareVal < 0)
                            {
                                nHigh = nMed;
                                nMed = (nHigh - nLow) / 2 + nLow;
                                continue;
                            }
                            Result = false;
                            break;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:389-392 TQuickSortList.Lock`（EnterCriticalSection）。</summary>
    public void Lock() => System.Threading.Monitor.Enter(CriticalSection);

    /// <summary>`MemoryIniFiles.pas:394-397 TQuickSortList.UnLock`（LeaveCriticalSection）。</summary>
    public void UnLock() => System.Threading.Monitor.Exit(CriticalSection);
}

/// <summary>
/// `MemoryIniFiles.pas:29-78 / 413-886 TMemoryIniFile` 1:1。
///
/// <para>
/// ★ 原文两处**有意保留的缺陷/不对称**（照抄 + 差异断言锁死，见测试）：
/// </para>
/// <list type="number">
/// <item>**节查找大小写不敏感、键查找大小写敏感**：解析出的节由 `Get` 用 `Sections.SortString`
///   按 `CompareText` 排好，而 `TQuickSortList.GetIndex` 在 `Sorted=False` 下也用 `CompareText`；
///   但键查找走 `TIniValueList.GetIndex` = **`CompareStr`**（`MemoryIniFiles.pas:91`）。
///   ⇒ `ReadString('sec','KEY')` 与 `ReadString('SEC','key')` 结果不同。</item>
/// <item>**`WriteString` 新建的节用的是 `TValueList`（不是 `TIniValueList`）**（`:649`）
///   ⇒ 该节的键查找变成 `SDK.pas:702` 的**二分 + CompareText**（大小写不敏感），
///   与"解析出来的节"行为相反。这就是本类 <see cref="TIniValueLookup"/> 存在的唯一原因。</item>
/// <item>`SaveToList`/`SaveToFile` 的 `else SaveList.Add(Section)` 分支
///   （`:515`/`:544`）会写出**原样的节名行**：空节名 `[]` 会写成**空行**，`[;x]` 会写成 `;x`
///   （丢掉方括号）—— 原文如此。</item>
/// <item>`FChanged` 实际**只写不读**（`SaveToFile` 开头 `:528` 把它清掉，那行
///   `// if FChanged and FLoadOK then begin` 是注释）⇒ 该字段对行为无影响；
///   但 `OnChange`（public 属性）会经 `Changed` 冒泡，是本类唯一可观察的副作用。</item>
/// </list>
/// </summary>
public class TMemoryIniFile : IDisposable
{
    private bool FLoadOK;
    private bool FChanged;
    private Action<TMemoryIniFile>? FOnChange;

    /// <summary>`MemoryIniFiles.pas:38 Sections: TQuickSortList`。</summary>
    public TQuickSortList Sections = null!;

    /// <summary>原文 `FChanged`（private，只写不读）；托管侧公开只读以便单测观察 `Changed` 是否触发。</summary>
    public bool ChangedFlag => FChanged;

    /// <summary>原文 `FLoadOK`（private）：`Get` 成功收尾置 True，`LoadFromFile` 开头置 False。</summary>
    public bool LoadOKFlag => FLoadOK;

    /// <summary>`MemoryIniFiles.pas:77 property OnChange`。</summary>
    public Action<TMemoryIniFile>? OnChange
    {
        get => FOnChange;
        set => FOnChange = value;
    }

    /// <summary>`MemoryIniFiles.pas:413-419 constructor Create()`。</summary>
    public TMemoryIniFile()
    {
        FChanged = false;
        FLoadOK = false;
        Sections = new TQuickSortList();
        Sections.OnChange = Changed;
    }

    /// <summary>`MemoryIniFiles.pas:421-428 constructor Create(FileList: TStrings)`。</summary>
    public TMemoryIniFile(TStringList FileList)
    {
        FChanged = false;
        FLoadOK = false;
        Sections = new TQuickSortList();
        Sections.OnChange = Changed;
        Get(FileList);
    }

    /// <summary>
    /// `MemoryIniFiles.pas:430-441 constructor Create(Text: string)` 1:1。
    /// 原文在体内再调 `Create(Strings)`（Delphi 允许同对象再跑一次构造器）；
    /// 托管侧用私有 `ConstructFrom` 表达同一语义（字段初值顺序一致）。
    /// 原文的临时 `TStringList.OnChange := Changed` 在此**无观察者**（对象尚未构造完就被覆写为 False），
    /// 故托管侧不复制该接线（登记 D-P10-05）。
    /// </summary>
    public TMemoryIniFile(string Text)
    {
        FChanged = false;
        FLoadOK = false;
        var Strings = new TStringList();
        Strings.Text = Text;
        ConstructFrom(Strings);
    }

    /// <summary>
    /// `MemoryIniFiles.pas:443-457 constructor Create(Data: Pointer; Size: Integer)`。
    /// 原文 `SetLength(Text, Size); Move(Data^, Text[1], Size);` 即"把 Size 个**裸字节**当成
    /// AnsiString 内容"（不做任何编码嗅探）；托管等价 = <c>byte[]</c> 按 GBK 解码（本工程 ANSI 约定）。
    /// </summary>
    public TMemoryIniFile(byte[] Data, int Size)
    {
        FChanged = false;
        FLoadOK = false;
        var text = EncodingInit.GBK.GetString(Data, 0, Size);
        var Strings = new TStringList();
        Strings.Text = text;
        ConstructFrom(Strings);
    }

    /// <summary>`Create(FileList: TStrings)` 的构造体（供 Text/Data 重载复用）。</summary>
    private void ConstructFrom(TStringList FileList)
    {
        FChanged = false;
        FLoadOK = false;
        Sections = new TQuickSortList();
        Sections.OnChange = Changed;
        Get(FileList);
    }

    /// <summary>`MemoryIniFiles.pas:459-467 destructor Destroy`：逐个释放节的值表后释放节表。</summary>
    public void Dispose()
    {
        for (int I = 0; I <= Sections.Count - 1; I++)
            ((TIniValueList)Sections.GetObject(I)).Free();
        Sections = null!;
    }

    /// <summary>`MemoryIniFiles.pas:469-473 procedure Changed(Sender: TObject)`。</summary>
    public void Changed(object Sender)
    {
        FChanged = true;
        FOnChange?.Invoke(this);
    }

    /// <summary>
    /// `MemoryIniFiles.pas:475-492 LoadFromFile` 1:1。
    /// 原文 `Strings.LoadFromFile(FileName)` 抛异常由空 `except` 吞掉 ⇒ 文件缺失/不可读时留下**空表**。
    /// （托管 `TStringList.LoadFromFile` 对缺失文件本身就不抛，见 D-P8-12 —— 可观察结果相同。）
    /// </summary>
    public void LoadFromFile(string FileName)
    {
        FChanged = false;
        for (int I = 0; I <= Sections.Count - 1; I++)
            ((TIniValueList)Sections.GetObject(I)).Free();
        Sections.Clear();

        var Strings = new TStringList();
        try
        {
            Strings.LoadFromFile(FileName);
        }
        catch
        {
            // 原文如此：空 except
        }
        Get(Strings);
    }

    /// <summary>
    /// `MemoryIniFiles.pas:494-518 SaveToList` 1:1。
    /// 节名非空且首字符不是 `';'` ⇒ 写 `[节名]` + 逐键 `Name=Value`；
    /// 否则原样写节名（空节名 ⇒ 空行；`[;x]` ⇒ `;x`）—— 见类注释第 3 条。
    /// </summary>
    public void SaveToList(TStringList SaveList)
    {
        FChanged = false;
        for (int I = 0; I <= Sections.Count - 1; I++)
        {
            string Section = Sections[I];
            if ((Section != "") && (Section[0] != ';'))
            {
                SaveList.Add("[" + Sections[I] + "]");
                var Strings = (TIniValueList)Sections.GetObject(I);
                for (int II = 0; II <= Strings.Count - 1; II++)
                {
                    SaveList.Add(Strings.Names(II) + "=" + Strings.Strings(II));
                }
            }
            else
            {
                SaveList.Add(Section);
            }
        }
    }

    /// <summary>`MemoryIniFiles.pas:520-554 SaveToFile`：先拼 `SaveList`，再 `SaveToFile`（异常吞掉）。</summary>
    public void SaveToFile(string FileName)
    {
        // if FChanged and FLoadOK then begin   ← 原文如此：该保护被注释掉（`:527`/`:553`）
        FChanged = false;
        var SaveList = new TStringList();
        for (int I = 0; I <= Sections.Count - 1; I++)
        {
            string Section = Sections[I];
            if ((Section != "") && (Section[0] != ';'))
            {
                SaveList.Add("[" + Sections[I] + "]");
                var Strings = (TIniValueList)Sections.GetObject(I);
                for (int II = 0; II <= Strings.Count - 1; II++)
                {
                    SaveList.Add(Strings.Names(II) + "=" + Strings.Strings(II));
                }
            }
            else
            {
                SaveList.Add(Section);
            }
        }
        try
        {
            SaveList.SaveToFile(FileName);
        }
        catch
        {
            // 原文如此：空 except
        }
    }

    /// <summary>
    /// `MemoryIniFiles.pas:556-592 Get` 1:1。
    /// · `LineText := TrimLeft(原文行)`；`nPos := Pos(']', LineText)`（**注意：不要求 `]` 在末尾**）；
    /// · `LineText[1]='['` 且 `nPos>0` ⇒ 节名 = `Copy(LineText, 2, nPos-2)`，值表用
    ///   `TIniValueList`（线性 CompareStr 查找）；**同名节不去重**（`AddObject` 追加）；
    /// · 否则在"已有当前节"且行非空时按**第一个** `'='` 切 `Name`/`Value`；
    /// · 收尾 `Sections.SortString(0, Count-1)`（按 CompareText 手工快排）。
    ///   ⇒ 节名在表内有序，但 `Sections.Sorted` 仍是 **False** ⇒ `GetIndex` 走 CompareText 分支。
    /// </summary>
    public void Get(TStringList FileList)
    {
        TIniValueList? Strings = null;
        FLoadOK = false;
        for (int I = 0; I <= FileList.Count - 1; I++)
        {
            string LineText = DelphiRTL.TrimLeft(FileList[I]);
            int nPos = DelphiRTL.Pos("]", LineText);
            if ((LineText != "") && (LineText[0] == '[') && (nPos > 0))
            {
                Strings = new TIniValueList { Lookup = TIniValueLookup.LinearCaseSensitive };
                Strings.OnChange = _ => Changed(this);
                Sections.AddObject(DelphiRTL.Copy(LineText, 2, nPos - 2), Strings);
            }
            else
            {
                if ((Strings != null) && (LineText != ""))
                {
                    nPos = DelphiRTL.Pos("=", LineText);
                    if (nPos > 0)
                    {
                        string sName = DelphiRTL.Copy(LineText, 1, nPos - 1);
                        string sValue = DelphiRTL.Copy(LineText, nPos + 1, LineText.Length);
                        Strings.AddObject(sName, sValue, null);   // 原文 TObject(0)
                    }
                }
            }
        }
        Sections.SortString(0, Sections.Count - 1);
        FLoadOK = true;
    }

    /// <summary>`MemoryIniFiles.pas:594-605 ReadSection`：`AddStrings` 逐项按 `Strings[i]` 追加。</summary>
    public void ReadSection(string Section, TStringList Strings)
    {
        int nIndex = Sections.GetIndex(Section);
        if (nIndex >= 0)
        {
            var ValueList = (TIniValueList)Sections.GetObject(nIndex);
            // Delphi TStrings.AddStrings：逐项 AddObject(Source[i], Source.Objects[i])
            for (int i = 0; i <= ValueList.Count - 1; i++)
                Strings.AddObject(ValueList.Strings(i), ValueList.Objects(i));
        }
    }

    // ---------------------------------------------------------------- 字符串节名重载

    /// <summary>`MemoryIniFiles.pas:607-623 ReadString(const Section, Ident, Default: string)`。</summary>
    public string ReadString(string Section, string Ident, string Default)
    {
        string Result = Default;
        int nIndex = Sections.GetIndex(Section);
        if (nIndex >= 0)
        {
            var Strings = (TIniValueList)Sections.GetObject(nIndex);
            nIndex = Strings.GetIndex(Ident);
            if (nIndex >= 0)
            {
                Result = Strings.Strings(nIndex);
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:625-655 WriteString(const Section, Ident, Value: string)`。</summary>
    public void WriteString(string Section, string Ident, string Value)
    {
        FChanged = true;
        int nIndex = Sections.GetIndex(Section);
        if (nIndex >= 0)
        {
            var Strings = (TIniValueList)Sections.GetObject(nIndex);
            nIndex = Strings.GetIndex(Ident);
            if (nIndex >= 0)
            {
                Strings.SetStrings(nIndex, Value);
            }
            else
            {
                Strings.AddRecord(Ident, Value);
            }
        }
        else
        {
            // 原文 TValueList.Create（**不是** TIniValueList）⇒ 该节的键查找走二分版（见类注释第 2 条）
            var Strings = new TIniValueList { Lookup = TIniValueLookup.Binary };
            Strings.OnChange = _ => Changed(this);
            Sections.AddRecord(Section, Strings);
            Strings.AddRecord(Ident, Value);
        }
    }

    /// <summary>`MemoryIniFiles.pas:657-666 ReadInteger(const Section, Ident: string; Default: Longint)`。</summary>
    public int ReadInteger(string Section, string Ident, int Default)
    {
        string IntStr = ReadString(Section, Ident, "");
        if ((IntStr.Length > 2) && (IntStr[0] == '0') && ((IntStr[1] == 'X') || (IntStr[1] == 'x')))
            IntStr = "$" + DelphiRTL.Copy(IntStr, 3, DelphiRTL.MaxInt);
        return MemoryIniRtl.StrToIntDef(IntStr, Default);
    }

    /// <summary>`MemoryIniFiles.pas:668-671 WriteInteger(const Section, Ident: string; Value: Longint)`。</summary>
    public void WriteInteger(string Section, string Ident, int Value)
        => WriteString(Section, Ident, DelphiRTL.IntToStr(Value));

    /// <summary>`MemoryIniFiles.pas:673-676 ReadBool`：`ReadInteger(Section, Ident, Ord(Default)) &lt;&gt; 0`。</summary>
    public bool ReadBool(string Section, string Ident, bool Default)
        => ReadInteger(Section, Ident, Default ? 1 : 0) != 0;

    /// <summary>`MemoryIniFiles.pas:678-683 WriteBool`：`Values: array[Boolean] of string = ('0','1')`。</summary>
    public void WriteBool(string Section, string Ident, bool Value)
        => WriteString(Section, Ident, Value ? "1" : "0");

    /// <summary>`MemoryIniFiles.pas:752-767 ReadDate(const Section, Name: string; Default: TDateTime)`。</summary>
    public double ReadDate(string Section, string Name, double Default)
    {
        string DateStr = ReadString(Section, Name, "");
        double Result = Default;
        if (DateStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToDate(DateStr);
            }
            catch (EConvertError)
            {
                // Ignore EConvertError exceptions（原文如此；其它异常继续上抛 = 原文 else raise）
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:769-784 ReadDateTime`。</summary>
    public double ReadDateTime(string Section, string Name, double Default)
    {
        string DateStr = ReadString(Section, Name, "");
        double Result = Default;
        if (DateStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToDateTime(DateStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:786-801 ReadFloat`。</summary>
    public double ReadFloat(string Section, string Name, double Default)
    {
        string FloatStr = ReadString(Section, Name, "");
        double Result = Default;
        if (FloatStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToFloat(FloatStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:803-818 ReadTime`。</summary>
    public double ReadTime(string Section, string Name, double Default)
    {
        string TimeStr = ReadString(Section, Name, "");
        double Result = Default;
        if (TimeStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToTime(TimeStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }

    // ---------------------------------------------------------------- 整数节序号重载（`Section: Integer`）

    /// <summary>
    /// `MemoryIniFiles.pas:686-701 ReadString(Section: Integer; …)`：**索引越界直接返回 Default**
    /// （与字符串重载的按名查找是两条路）。
    /// </summary>
    public string ReadString(int Section, string Ident, string Default)
    {
        string Result = Default;
        if ((Section >= 0) && (Section < Sections.Count))
        {
            var Strings = (TIniValueList)Sections.GetObject(Section);
            int nIndex = Strings.GetIndex(Ident);
            if (nIndex >= 0)
            {
                Result = Strings.Strings(nIndex);
            }
        }
        return Result;
    }

    /// <summary>
    /// `MemoryIniFiles.pas:703-721 WriteString(Section: Integer; …)`：
    /// ★ 原文如此 —— **不置 `FChanged`**，且节序号越界时**静默什么都不做**（不会新建节）。
    /// </summary>
    public void WriteString(int Section, string Ident, string Value)
    {
        if ((Section >= 0) && (Section < Sections.Count))
        {
            var Strings = (TIniValueList)Sections.GetObject(Section);
            int nIndex = Strings.GetIndex(Ident);
            if (nIndex >= 0)
            {
                Strings.SetStrings(nIndex, Value);
            }
            else
            {
                Strings.AddRecord(Ident, Value);
            }
        }
    }

    /// <summary>`MemoryIniFiles.pas:723-732 ReadInteger(Section: Integer; …)`。</summary>
    public int ReadInteger(int Section, string Ident, int Default)
    {
        string IntStr = ReadString(Section, Ident, "");
        if ((IntStr.Length > 2) && (IntStr[0] == '0') && ((IntStr[1] == 'X') || (IntStr[1] == 'x')))
            IntStr = "$" + DelphiRTL.Copy(IntStr, 3, DelphiRTL.MaxInt);
        return MemoryIniRtl.StrToIntDef(IntStr, Default);
    }

    /// <summary>`MemoryIniFiles.pas:734-737 WriteInteger(Section: Integer; …)`。</summary>
    public void WriteInteger(int Section, string Ident, int Value)
        => WriteString(Section, Ident, DelphiRTL.IntToStr(Value));

    /// <summary>`MemoryIniFiles.pas:739-742 ReadBool(Section: Integer; …)`。</summary>
    public bool ReadBool(int Section, string Ident, bool Default)
        => ReadInteger(Section, Ident, Default ? 1 : 0) != 0;

    /// <summary>`MemoryIniFiles.pas:744-749 WriteBool(Section: Integer; …)`。</summary>
    public void WriteBool(int Section, string Ident, bool Value)
        => WriteString(Section, Ident, Value ? "1" : "0");

    /// <summary>`MemoryIniFiles.pas:820-835 ReadDate(Section: Integer; …)`。</summary>
    public double ReadDate(int Section, string Name, double Default)
    {
        string DateStr = ReadString(Section, Name, "");
        double Result = Default;
        if (DateStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToDate(DateStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:837-852 ReadDateTime(Section: Integer; …)`。</summary>
    public double ReadDateTime(int Section, string Name, double Default)
    {
        string DateStr = ReadString(Section, Name, "");
        double Result = Default;
        if (DateStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToDateTime(DateStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:854-869 ReadFloat(Section: Integer; …)`。</summary>
    public double ReadFloat(int Section, string Name, double Default)
    {
        string FloatStr = ReadString(Section, Name, "");
        double Result = Default;
        if (FloatStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToFloat(FloatStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }

    /// <summary>`MemoryIniFiles.pas:871-886 ReadTime(Section: Integer; …)`。</summary>
    public double ReadTime(int Section, string Name, double Default)
    {
        string TimeStr = ReadString(Section, Name, "");
        double Result = Default;
        if (TimeStr != "")
        {
            try
            {
                Result = MemoryIniRtl.StrToTime(TimeStr);
            }
            catch (EConvertError)
            {
            }
        }
        return Result;
    }
}
