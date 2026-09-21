// ============================================================================================
// uFrmRoleDataEdit.pas (1-974) → uFrmRoleDataEdit.cs
//
// 人物 / 英雄数据编辑窗体（DBServer）。原文是**活代码**：
//   · DBServer.dpr:15 / DBServer.dproj:91 都编译它；
//   · LoginSrv/uFrmDataManager.pas:64 `uses ... uFrmRoleDataEdit`，并在 :199/:207/:454/:462 调
//     `ShowFrmRoleDataEdit(HumanID, HumData, nil)` / `(HeroID, nil, @HeroData)`。
//
// 原文 DFM：`Source\DBServer\uFrmRoleDataEdit.dfm`（**文本** DFM，GBK）。实测：
//   · `object` 节点 **88**（窗体 1 + 子节点 87，其中 SaveDialog/OpenDialog 是 TComponent，不在 Controls 里）；
//   · 事件绑定 **35** = OnCreate×1 + OnChange×29 + OnClick×2（edtHomeMap/chkIsMaster）
//                       + ButtonSaveData.OnClick×1 + ButtonExportDataClick×2（导出/导入两个按钮）。
//   （`_analysis/utf8_mirror/DBServer/uFrmRoleDataEdit.dfm` 是**损坏**副本，本文件一律以 Source 下的 GBK 文本 DFM 为准。）
//
// 命名空间：刻意用 `GXX.DBServer.Forms`。C# 简单名解析**先找外层命名空间**再看 using，
// 故 `GXX.DBServer` 里的 TSpinEdit/TSpinEditEx（SpinControls.cs）、IListViewSink/ListViewSink
// （ListViewSink.cs）、THumanDBBase/THeroDBBase（MySqlRoleDB.Base.cs）、UiSeam/TMsgBox/TBool
// （UiSeam.cs / DelphiRtlSeam.cs）、SelectClientRoleDbSeam（SelectClient.Seams.cs）在本命名空间内**天然可见**，
// 本文件**不重复声明**任何既有类型。
// ============================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.DBServer.Forms;

// ============================================================================================
// Grobal2.pas 的长度常量（本单元只用这几个；数组维度由 THumData 的内联数组类型固定）
// ============================================================================================

/// <summary>uFrmRoleDataEdit.pas 里各 `Length(FXxx)` / `Low..High(FXxx)` 对应的**静态**数组维度。</summary>
public static class RoleDataEditConst
{
    /// <summary>Grobal2.pas: `UValues: array[0..499] of Integer`（:206/:708/:948/:950）。</summary>
    public const int UValuesCount = 500;

    /// <summary>Grobal2.pas: `TValues: array[0..499] of string[100]`（:212/:713/:953/:955）。</summary>
    public const int TValuesCount = 500;

    /// <summary>Grobal2.pas:51 `MAX_USE_ITEM_COUNT = 30`（`THumanUseItems = array[0..29]`）。</summary>
    public const int HumItemsCount = 30;

    /// <summary>Grobal2.pas:4189 `THumanJewelryBoxItems = array[0..5]`。</summary>
    public const int JewelryBoxItemsCount = 6;

    /// <summary>Grobal2.pas:4195 `THumanGodBlessItems = array[0..11]`。</summary>
    public const int GodBlessItemsCount = 12;

    /// <summary>Grobal2.pas:4153 `TStorageItems = array[0..195]`。</summary>
    public const int StorageItemsCount = 196;

    /// <summary>Grobal2.pas:4203 `THumanFengHaoItems = array[0..59]`。</summary>
    public const int FengHaoItemsCount = 60;

    /// <summary>Grobal2.pas `THumData.Magics: array[0..47] of THumMagic`（:348/:364）。</summary>
    public const int MagicsCount = 48;

    /// <summary>
    /// uFrmRoleDataEdit.pas:131-162（implementation 段）：
    /// <code>const TItemWhereNames: array[Low(THumanUseItems)..High(THumanUseItems)] of string = (...)</code>
    ///
    /// ★ 实测该数组字面量**共 30 项**（:133-162），与 `Low..High(THumanUseItems) = 0..29` 一致。
    ///   （派发说明里写的"31 项"是笔误，以源文件计数为准 —— 见测试 `TItemWhereNames_30项且顺序与原文一致`。）
    /// 逐字照抄，**顺序不得调整**（下标即装备位序号）。
    /// </summary>
    public static readonly string[] TItemWhereNames =
    {
        "衣服",
        "武器",
        "照明物",
        "项链",
        "头盔",
        "左手镯",
        "右手镯",
        "左戒指",
        "右戒指",
        "符",
        "腰带",
        "鞋子",
        "宝石",
        "斗笠",
        "军鼓",
        "马牌",
        "盾牌",
        "灵玉",
        "时装衣服",
        "时装武器",
        "时装项链",
        "时装头盔",
        "时装左手镯",
        "时装右手镯",
        "时装左戒指",
        "时装右戒指",
        "时装照明物",
        "时装腰带",
        "时装鞋子",
        "时装宝石"
    };
}

// ============================================================================================
// 控件壳（原文用的是 Delphi 标准/自研控件，托管侧缺哪一个就在这里补齐）
// ============================================================================================

/// <summary>
/// `Component\Mir2Ctrls\source\SpinEditEx.pas:41-380` 的 `TSpinEditLongWord`
/// （`TCustomEdit` 派生，`Value: LongWord`，用**文本**承载数值；DFM 里 4 个实例：
/// `seGold` / `seGameGold` / `seGameDiamond` / `seGameGird`）。
///
/// 原文语义（逐行）：
///   · :186-203 构造：`Text := '0'`、`FIncrement := 1`、`FMinValue/FMaxValue` 由 DFM 赋 0/0；
///   · :344-354 GetValue：`I64 := StrToInt64(Text)`，再 `Max(FMinValue, Min(I64, High(LongWord)))`；
///     解析失败 → `FMinValue`；
///   · :356-359 SetValue：`Text := IntToStr(CheckValue(NewValue))`；
///   · :361-371 CheckValue：**仅当 `FMaxValue <> FMinValue` 才裁剪** ⇒ 本单元 4 个实例（0/0）**不裁剪**。
///
/// 托管映射：WinForms `NumericUpDown.Value` 是 `decimal`。为复刻"0/0 不裁剪"，
/// 底层范围固定为 **[0, High(LongWord)]** —— 这正是原文 GetValue 的钳制区间，
/// 于是 uint 全域的赋值/读取都无损，且与 :350 的裁剪语义逐字一致。
/// ★ 该类型在托管侧**不存在**（`SpinControls.cs` 只有 TSpinEdit/TSpinEditEx），故按车道纪律在本文件内声明。
/// </summary>
public class TSpinEditLongWord : NumericUpDown
{
    /// <summary>SpinEditEx.pas:43 `FMinValue: LongWord`（DFM `MinValue`）。</summary>
    public uint DfmMinValue;

    /// <summary>SpinEditEx.pas:44 `FMaxValue: LongWord`（DFM `MaxValue`）。</summary>
    public uint DfmMaxValue;

    public TSpinEditLongWord()
    {
        // SpinEditEx.pas:344-354：GetValue 的钳制区间 = [FMinValue, High(LongWord)] = [0, 4294967295]
        Minimum = 0;
        Maximum = uint.MaxValue;
    }

    /// <summary>SpinEditEx.pas:99 `property Value: LongWord read GetValue write SetValue;`。</summary>
    public new uint Value
    {
        get => (uint)base.Value;
        set => base.Value = value;
    }

    /// <summary>设置 DFM 的 MinValue/MaxValue（SpinEditEx.pas:87-88）。</summary>
    public void SetDfmRange(uint minValue, uint maxValue)
    {
        DfmMinValue = minValue;
        DfmMaxValue = maxValue;
    }
}

/// <summary>
/// Delphi VCL `TStringGrid`（`Grids.pas`）的最小托管等价物，承载 `strGridVarU` / `strGridVarT`。
///
/// ★ 越界语义**照抄 VCL**：`TStringGrid.GetEditText` / `SetEditText` 在
///   `(ACol &lt; 0) or (ARow &lt; 0) or (ACol &gt;= ColCount) or (ARow &gt;= RowCount)` 时**静默 Exit**
///   —— 读越界得空串、写越界**什么也不做**，**不抛异常**。
///   本单元依赖这一点：`ButtonSaveDataClick`（:950/:955）在**没走过 DoOpen** 时会读
///   `Cells[1, I+1]`（I 最大 499）而两张表的 RowCount 仍是 DFM 的 101 ⇒ 越界读取必须静默给空串。
///
/// ★ 与 `AddrEdit.cs` 的 `TStringGridModel` **不可合并**：那个单元的原文路径（AddrEdit.pas:190-193
///   写第 9 列）**必须抛出** `EInvalidGridIndex`，是它的可观察行为；本单元相反。两者放一起必然打架。
///
/// 行号口径：VCL 的 `RowCount` 含第 0 行（`FixedRows=1` 的固定表头行）；
/// WinForms `DataGridView` 没有固定行概念，故 `Rows[0]` 对应 VCL 的 row 0，`RowCount` 直接等价。
/// </summary>
public class TStringGrid : DataGridView
{
    public TStringGrid()
    {
        // VCL 默认 FixedRows=1/FixedCols=1；本单元只把第 0 行当表头用（DoOpen :193-197）。
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        RowHeadersVisible = false;
        ColumnHeadersVisible = false;
        // DFM: Options=[goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
        ReadOnly = false;
    }

    /// <summary>Delphi `TStringGrid.ColCount`（DFM `ColCount = 2`）。</summary>
    public int ColCount
    {
        get => ColumnCount;
        set => ColumnCount = value;
    }

    /// <summary>Delphi `TStringGrid.RowCount`（DFM `RowCount = 101`；`DoOpen` 再把人物侧改成 501）。</summary>
    public new int RowCount
    {
        get => base.RowCount;
        set => base.RowCount = value;
    }

    /// <summary>Delphi `TStringGrid.Cells[Col, Row]`（读）：越界**静默返回空串**（VCL GetEditText 语义）。</summary>
    public string Cells(int col, int row)
    {
        if (col < 0 || row < 0 || col >= ColCount || row >= RowCount) return "";
        return Convert.ToString(Rows[row].Cells[col].Value) ?? "";
    }

    /// <summary>Delphi `TStringGrid.Cells[Col, Row] := Value`（写）：越界**静默忽略**（VCL SetEditText 语义）。</summary>
    public void SetCells(int col, int row, string value)
    {
        if (col < 0 || row < 0 || col >= ColCount || row >= RowCount) return;
        Rows[row].Cells[col].Value = value;
    }
}

/// <summary>
/// Delphi `ComCtrls.TTabSheet` 的托管壳（`tsBase` … `tsVarT` 共 8 个）。
///
/// ★ 为什么需要它：WinForms `TabPage` **没有** `TabVisible`；`TabPage.Visible` 返回的是
///   **有效可见性**（窗体从未显示时恒为 False），不能承载原文 `TabVisible` 的**属性值**。
///   故本类单独承载 `TabVisible`，并由 <see cref="TFrmRoleDataEdit.ApplyTabVisible"/> 把它落成
///   "把页从 TabControl 的控件集合里摘除 / 按原索引插回"（与 p9-m2-forms 车道
///   `GHeroDBConfig.cs` 的 P10TabVisible 垫片同一处置）。
/// </summary>
public class TTabSheet : TabPage
{
    /// <summary>Delphi `TTabSheet.TabVisible`（页签是否出现在页签行）。DFM 默认 True。</summary>
    public bool TabVisible = true;

    /// <summary>DFM 里的原始页序（摘除后按它插回，保证 `PageControl.ActivePageIndex` 口径不变）。</summary>
    public int DfmIndex = -1;

    public TTabSheet(string caption) : base(caption) { }
}

// ============================================================================================
// 接缝（原文依赖而**未移植**的外部面；按台账 §25.2：有语义返回值的接缝默认**抛"未接线"**，绝不静默给中性值）
// ============================================================================================

/// <summary>
/// 接缝：`DBShare.pas` 的两个显示名查询函数（**未移植**）。
///   :104-105 声明；:268-279 `function GetStdItemName(nPosition: Integer): string;`（查 `g_StdItemList`）；
///   :291-307 `function GetMagicName(wMagicId: Word; MagicAttr: TMagicAttr): string;`（查 `g_MagicList`）。
/// 本单元的调用点：:356/:372（GetMagicName）、:400/:432/:463/:497/:528/:559/:603/:636/:679/:683（GetStdItemName）。
/// 两者都是"有语义返回值"，故默认实现**抛 NotSupportedException 并指名接入点**（台账 §25.2）。
/// </summary>
public static class RoleDataEditDbShareSeam
{
    /// <summary>DBShare.pas:291-307 `GetMagicName(wMagicId, MagicAttr)`。</summary>
    public static Func<ushort, TMagicAttr, string> GetMagicName = UnwiredGetMagicName;

    /// <summary>DBShare.pas:268-279 `GetStdItemName(nPosition)`（注意原文入参是 1 基：`g_StdItemList[nPosition - 1]`）。</summary>
    public static Func<int, string> GetStdItemName = UnwiredGetStdItemName;

    /// <summary>复位为"未接线"（单测用）。</summary>
    public static void Reset()
    {
        GetMagicName = UnwiredGetMagicName;
        GetStdItemName = UnwiredGetStdItemName;
    }

    private static string UnwiredGetMagicName(ushort wMagicId, TMagicAttr MagicAttr)
        => throw new NotSupportedException(
            "接缝：DBShare.pas:291-307 GetMagicName 未接线。接入点：宿主把 g_MagicList 的查询挂到 RoleDataEditDbShareSeam.GetMagicName。");

    private static string UnwiredGetStdItemName(int nPosition)
        => throw new NotSupportedException(
            "接缝：DBShare.pas:268-279 GetStdItemName 未接线。接入点：宿主把 g_StdItemList 的查询挂到 RoleDataEditDbShareSeam.GetStdItemName。");
}

/// <summary>
/// 接缝：`TSaveDialog.Execute` / `TOpenDialog.Execute`（原文 :747 / :781）。
///
/// 契约刻意做成 **bool**（= 原文 Execute 的返回值），而不是"直接返回路径"：
/// 这样窗体侧的 `if not X.Execute then Exit; s := X.FileName;` 可以**逐字**保留
/// （FileName / InitialDir 仍由真实的 SaveFileDialog/OpenFileDialog 字段承载）。
/// 注入的替身只需"把 `frm.SaveDialog.FileName` 设成要用的路径，再返回 true/false"。
/// </summary>
public static class RoleDataEditFileDialogSeam
{
    /// <summary>默认实现弹真实保存对话框（单测**必须**注入，否则会阻塞）。</summary>
    public static Func<TFrmRoleDataEdit, bool> SaveDialogExecute = DefaultSaveDialogExecute;

    /// <summary>默认实现弹真实打开对话框（单测**必须**注入）。</summary>
    public static Func<TFrmRoleDataEdit, bool> OpenDialogExecute = DefaultOpenDialogExecute;

    /// <summary>复位为默认（弹真实对话框）。</summary>
    public static void Reset()
    {
        SaveDialogExecute = DefaultSaveDialogExecute;
        OpenDialogExecute = DefaultOpenDialogExecute;
    }

    private static bool DefaultSaveDialogExecute(TFrmRoleDataEdit frm)
        => frm.SaveDialog.ShowDialog() == DialogResult.OK;

    private static bool DefaultOpenDialogExecute(TFrmRoleDataEdit frm)
        => frm.OpenDialog.ShowDialog() == DialogResult.OK;
}

// ============================================================================================
// Delphi System.pas 的低级文件句柄族（ProcessSaveDataToFile / ProcessLoadDataformFile 的底座）
// ============================================================================================

/// <summary>
/// Delphi `System.pas` 的 `FileOpen` / `FileCreate` / `FileRead` / `FileWrite` / `FileClose`
/// 的托管等价物（只有本单元用到的模式）。
///
/// 语义对齐（Delphi 7 SysUtils 实现）：
///   · `FileOpen(Name, Mode)` → `CreateFile(..., OPEN_EXISTING, ...)`：**文件不存在返回 -1**（不创建）；
///     Mode 低半字节 = 访问模式（0=fmOpenRead / 1=fmOpenWrite / 2=fmOpenReadWrite），
///     高半字节 = 共享模式（$00=fmShareCompat/独占、$10=fmShareExclusive、$20=fmShareDenyWrite、
///     $30=fmShareDenyRead、$40=fmShareDenyNone）；
///   · `FileCreate(Name)` → `CreateFile(..., CREATE_ALWAYS, ...)`，**截断**已有文件，共享模式 0（独占）；
///   · `FileRead/FileWrite` 返回**实际字节数**（失败返回 0，不抛）；
///   · `FileClose` 关句柄；对无效句柄**静默返回**。
///
/// ★ 句柄用自增 int 模拟（Delphi 的 `THandle` 也是整数），`OpenHandleCount` 供单测断言"有没有漏关"。
/// </summary>
public static class DelphiFileIo
{
    public const int fmOpenRead = 0;
    public const int fmOpenWrite = 1;
    public const int fmOpenReadWrite = 2;
    public const int fmShareCompat = 0x00;
    public const int fmShareExclusive = 0x10;
    public const int fmShareDenyWrite = 0x20;
    public const int fmShareDenyRead = 0x30;
    public const int fmShareDenyNone = 0x40;

    private static readonly Dictionary<int, FileStream> Handles = new Dictionary<int, FileStream>();
    private static int _nextHandle = 1;

    /// <summary>Delphi `FileOpen(const FileName: string; Mode: LongWord): Integer`（失败 -1）。</summary>
    public static int FileOpen(string fileName, int mode)
    {
        try
        {
            FileShare share;
            switch (mode & 0xF0)
            {
                case fmShareDenyNone: share = FileShare.ReadWrite; break;
                case fmShareDenyWrite: share = FileShare.Read; break;
                case fmShareDenyRead: share = FileShare.Write; break;
                default: share = FileShare.None; break;      // fmShareCompat / fmShareExclusive
            }

            FileAccess access;
            switch (mode & 0x0F)
            {
                case fmOpenRead: access = FileAccess.Read; break;
                case fmOpenWrite: access = FileAccess.Write; break;
                default: access = FileAccess.ReadWrite; break;
            }

            return Register(new FileStream(fileName, FileMode.Open, access, share));
        }
        catch
        {
            return -1;      // 原文：CreateFile 失败 ⇒ 句柄 = INVALID_HANDLE_VALUE(-1)
        }
    }

    /// <summary>Delphi `FileCreate(const FileName: string): Integer`（CREATE_ALWAYS，失败 -1）。</summary>
    public static int FileCreate(string fileName)
    {
        try
        {
            return Register(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None));
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>Delphi `FileWrite(Handle: Integer; const Buffer; Count: Integer): Integer`（返回写入字节数）。</summary>
    public static int FileWrite(int handle, byte[] buffer, int offset, int count)
    {
        if (!Handles.TryGetValue(handle, out FileStream stream)) return 0;
        try
        {
            stream.Write(buffer, offset, count);
            stream.Flush();
            return count;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>Delphi `FileRead(Handle: Integer; var Buffer; Count: Integer): Integer`（返回读出字节数）。</summary>
    public static int FileRead(int handle, byte[] buffer, int offset, int count)
    {
        if (!Handles.TryGetValue(handle, out FileStream stream)) return 0;
        try
        {
            return stream.Read(buffer, offset, count);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>Delphi `FileClose(Handle: Integer)`（无效句柄静默）。</summary>
    public static void FileClose(int handle)
    {
        if (!Handles.TryGetValue(handle, out FileStream stream)) return;
        Handles.Remove(handle);
        try { stream.Dispose(); } catch { /* 原文 CloseHandle 失败不抛 */ }
    }

    /// <summary>当前仍未关闭的句柄数（单测断言"原文的 FileClose 有没有被 Exit 跳过"）。</summary>
    public static int OpenHandleCount => Handles.Count;

    /// <summary>强制清空全部句柄（单测隔离用）。</summary>
    public static void ResetAll()
    {
        foreach (KeyValuePair<int, FileStream> kv in Handles)
        {
            try { kv.Value.Dispose(); } catch { }
        }
        Handles.Clear();
        _nextHandle = 1;
    }

    private static int Register(FileStream stream)
    {
        int handle = _nextHandle++;
        Handles[handle] = stream;
        return handle;
    }
}

// ============================================================================================
// 单元级全局过程（原文 :125 / :164-187）
// ============================================================================================

/// <summary>uFrmRoleDataEdit.pas:125 的单元级 `procedure ShowFrmRoleDataEdit(...)`。</summary>
public static class RoleDataEditUnit
{
    /// <summary>
    /// uFrmRoleDataEdit.pas:164-187 `procedure ShowFrmRoleDataEdit(ID: Integer; HumData: PTHumData; HeroData: PTHeroData);`
    ///
    /// 原文形状（逐行）：建窗体 → `FID := ID` → `edtID.Text := IntToStr(ID)` →
    /// `if HumData &lt;&gt; nil then`（FIsHuman := True; FHumData := HumData^）
    /// `else`（FIsHuman := False; FHeroData := HeroData^）→ `DoOpen` → `ShowModal` → `finally Free`。
    ///
    /// ★ 接缝（与 LoginSrv 侧的跨模块类型冲突详见车道报告"跨区阻塞项"）：
    ///   托管侧 `GXX.LoginSrv/RoleDBSeam.cs:80` 已有一个同名接缝
    ///   `Action&lt;int, THumData?, THeroData?&gt;`，但那里的 `THumData`/`THeroData` 是**两个空类**
    ///   （仅作不透明句柄），与 `GXX.Core.Protocol.THumData/THeroData`（unsafe struct）不是同一个类型。
    ///   本方法的形参一律用 **真身** `GXX.Core.Protocol.THumData?` / `THeroData?`
    ///   （`nil` ↔ 原文的 nil 指针）；两侧统一由集成方裁定，本车道**不改** LoginSrv 的文件。
    /// </summary>
    public static void ShowFrmRoleDataEdit(int ID, THumData? HumData, THeroData? HeroData)
    {
        var FrmRoleDataEdit = new TFrmRoleDataEdit();
        try
        {
            FrmRoleDataEdit.FID = ID;
            FrmRoleDataEdit.edtID.Text = DelphiRTL.IntToStr(ID);
            if (HumData != null)
            {
                FrmRoleDataEdit.FIsHuman = true;
                FrmRoleDataEdit.FHumData = HumData.Value;
            }
            else
            {
                FrmRoleDataEdit.FIsHuman = false;
                // 原文 `FHeroData := HeroData^`：HeroData 为 nil 时原文是访问违例，
                // 托管侧 Nullable 取值抛 InvalidOperationException（等价"炸给你看"）。
                FrmRoleDataEdit.FHeroData = HeroData.Value;
            }

            FrmRoleDataEdit.DoOpen();
            FrmRoleDataEdit.ShowModalEquivalent();      // 原文 ShowModal
        }
        finally
        {
            FrmRoleDataEdit.Dispose();                  // 原文 finally FrmRoleDataEdit.Free
        }
    }
}

// ============================================================================================
// 窗体本体
// ============================================================================================

/// <summary>
/// uFrmRoleDataEdit.pas:11-123 `TFrmRoleDataEdit = class(TForm)`
/// （DFM: Source\DBServer\uFrmRoleDataEdit.dfm，88 个 object 节点 / 35 条事件绑定）。
/// </summary>
public class TFrmRoleDataEdit : Form
{
    // ---------------- DFM 的 88 个 object 节点：字段名与 DFM **逐字同名** ----------------

    /// <summary>DFM: `lbl11111: TLabel`（红色告警行）。</summary>
    public Label lbl11111;

    public TabControl PageControl;

    // DFM: PageControl 下的 8 个 TTabSheet（顺序即 DFM/页序）
    public TTabSheet tsBase;
    public TTabSheet tsInfo;
    public TTabSheet tsMagic;
    public TTabSheet tsUserItem;
    public TTabSheet tsFenghao;
    public TTabSheet tsSorage;
    public TTabSheet tsVarU;
    public TTabSheet tsVarT;

    // DFM: tsBase 上的 10 个标签（顺序即 DFM）
    public Label lbl2;       // 人物名称:
    public Label lbl3;       // 登录帐号:
    public Label lbl4;       // 仓库密码:
    public Label lbl5;       // 配偶名称:
    public Label lbl6;       // 师徒名称:
    public Label lbl1;       // 索引号码:
    public Label lbl7;       // 当前地图:
    public Label lbl8;       // 当前座标:
    public Label lbl9;       // 回城地图:
    public Label lbl10;      // 回城座标:

    // DFM: tsBase 上的 7 个 TEdit + edtHomeMap
    public TextBox edtChrName;
    public TextBox edtAccount;
    public TextBox edtPassword;
    public TextBox edtDearName;
    public TextBox edtMasterName;
    public TextBox edtID;
    public TextBox edtCurMap;
    public TextBox edtHomeMap;

    // DFM: tsBase 上的 4 个 TSpinEditEx
    public TSpinEditEx seCurX;
    public TSpinEditEx seCurY;
    public TSpinEditEx seHomeX;
    public TSpinEditEx seHomeY;

    public CheckBox chkIsMaster;

    // DFM: tsInfo 上的 10 个标签
    public Label lbl11;      // 等  级:
    public Label lbl12;      // 金  币:
    public Label lbl13;      // 元  宝:
    public Label lbl14;      // 游戏点:
    public Label lbl18;      // 声望点:
    public Label lbl17;      // 充值点:
    public Label lbl19;      // PK点:
    public Label lbl20;      // 贡献度:
    public Label lbl15;      // 金刚石:
    public Label lbl16;      // 灵  符:

    // DFM: tsInfo 上的 8 个 TSpinEditEx
    public TSpinEditEx seLevel;
    public TSpinEditEx seGamePoint;
    public TSpinEditEx seCreditPoint;
    public TSpinEditEx sePayPoint;
    public TSpinEditEx sePKPoint;
    public TSpinEditEx seContribution;

    // DFM: tsInfo 上的 4 个 TSpinEditLongWord（托管侧本文件自带实现）
    public TSpinEditLongWord seGold;
    public TSpinEditLongWord seGameGold;
    public TSpinEditLongWord seGameDiamond;
    public TSpinEditLongWord seGameGird;

    public GroupBox GroupBox6;

    // DFM: GroupBox6 上的 11 个标签
    public Label lbl22;      // DC:
    public Label lbl23;      // MC:
    public Label lbl24;      // SC:
    public Label lbl25;      // AC:
    public Label lbl26;      // MAC:
    public Label lbl27;      // HP:
    public Label lbl28;      // MP:
    public Label lbl29;      // Hit:
    public Label lbl30;      // Speed:
    public Label lbl31;      // X2:
    public Label lbl21;      // 可用属性点:

    // DFM: GroupBox6 上的 11 个 TSpinEditEx（10 个属性点 + seBonusPoint）
    public TSpinEditEx EditDC;
    public TSpinEditEx EditMC;
    public TSpinEditEx EditSC;
    public TSpinEditEx EditAC;
    public TSpinEditEx EditMAC;
    public TSpinEditEx EditHP;
    public TSpinEditEx EditMP;
    public TSpinEditEx EditHit;
    public TSpinEditEx EditSpeed;
    public TSpinEditEx EditX2;
    public TSpinEditEx seBonusPoint;

    // DFM: 4 张 TListView
    public ListView lvMagic;
    public ListView lvUserItem;
    public ListView lvFenghaoItem;
    public ListView lvStorage;

    // DFM: 2 张 TStringGrid
    public TStringGrid strGridVarU;
    public TStringGrid strGridVarT;

    // DFM: 3 个 TButton（都在窗体上，不在 PageControl 里）
    public Button ButtonSaveData;
    public Button ButtonExportData;
    public Button ButtonImportData;

    // DFM: 2 个非可视组件（TComponent，不在 Controls 树里 —— 对账时要单独 +2）
    public SaveFileDialog SaveDialog;
    public OpenFileDialog OpenDialog;

    // ---------------- uFrmRoleDataEdit.pas:104-107 的私有字段 ----------------
    // 原文是 `private`（Delphi 的 private = **单元内可见**，故单元级 ShowFrmRoleDataEdit 能直接写它们）。
    // C# 没有"同单元友元"，按本仓窗体惯例（RouteEdit.cs 的 FRouteInfo/FIsChanged）提升为 public。

    /// <summary>uFrmRoleDataEdit.pas:104 `FIsHuman: Boolean;`。</summary>
    public bool FIsHuman;

    /// <summary>uFrmRoleDataEdit.pas:105 `FHumData: THumData;`（值语义，等价原文的记录字段）。</summary>
    public THumData FHumData;

    /// <summary>uFrmRoleDataEdit.pas:106 `FID: Integer;`。</summary>
    public int FID;

    /// <summary>uFrmRoleDataEdit.pas:107 `FHeroData: THeroData;`。</summary>
    public THeroData FHeroData;

    // ---------------- 显示接缝（TListView → IListViewSink；复用既有 ListViewSink） ----------------

    /// <summary>`lvMagic` 的显示接缝（DFM 的 TListView → 既有 ListViewSink；测试可换成内存实现）。</summary>
    public IListViewSink MagicSink;

    /// <summary>`lvUserItem` 的显示接缝。</summary>
    public IListViewSink UserItemSink;

    /// <summary>`lvFenghaoItem` 的显示接缝。</summary>
    public IListViewSink FenghaoSink;

    /// <summary>`lvStorage` 的显示接缝。</summary>
    public IListViewSink StorageSink;

    // ---------------- ShowModal 接缝（原文 :183 `FrmRoleDataEdit.ShowModal`） ----------------

    /// <summary>
    /// 接缝：原文 :183 `ShowModal`。默认 **null ⇒ 不阻塞、直接返回 false**（由派发约定要求）：
    /// 本车道的单测与无头环境不能真弹模态窗体，宿主若要真显示需自行接线。
    /// </summary>
    public static Func<TFrmRoleDataEdit, bool> ShowModalHandler;

    /// <summary>等价于原文的 `ShowModal`（返回值 = 模态结果；未接线时 false）。</summary>
    public bool ShowModalEquivalent() => ShowModalHandler != null && ShowModalHandler(this);

    // ---------------- 原文 resourcestring（:386/:590/:667 三处**同名同值**声明） ----------------

    /// <summary>原文 `resourcestring sItemValue = '%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d-%d';`（14 项 btValue）。</summary>
    public const string sItemValue = "{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}-{10}-{11}-{12}-{13}";

    public TFrmRoleDataEdit()
    {
        InitializeComponent();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SaveDialog?.Dispose();
            OpenDialog?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ========================================================================================
    // InitializeComponent：DFM 逐条落地（每个控件一行，注里给 DFM 的几何/属性）
    // ========================================================================================
    private void InitializeComponent()
    {
        // DFM: FrmRoleDataEdit Left=373 Top=215 BorderIcons=[biSystemMenu] BorderStyle=bsSingle
        //      Caption='编辑人物数据' ClientHeight=380 ClientWidth=522 Color=clBtnFace
        //      Font.Charset=ANSI_CHARSET Font.Height=-12 Font.Name='宋体' Position=poDesktopCenter
        //      OnCreate=FormCreate PixelsPerInch=96
        Text = "编辑人物数据";
        Name = "FrmRoleDataEdit";
        StartPosition = FormStartPosition.CenterScreen;   // DFM: poDesktopCenter（本仓既有映射，见 uFrmDataManager.cs:65）
        Location = new System.Drawing.Point(373, 215);
        ClientSize = new System.Drawing.Size(522, 380);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: lbl11111 Left=275 Top=353 Width=240 Height=12 Font.Color=clRed
        //      Caption='修改时人物不能在线，否则数据回档或出错！'
        lbl11111 = new Label { Left = 275, Top = 353, Width = 240, Height = 12, ForeColor = System.Drawing.Color.Red, Text = "修改时人物不能在线，否则数据回档或出错！" };

        // DFM: PageControl Left=9 Top=8 Width=504 Height=328 ActivePage=tsBase TabOrder=0
        PageControl = new TabControl { Left = 9, Top = 8, Width = 504, Height = 328, TabIndex = 0 };

        // ---------------- PageControl 的 8 个 TabSheet（DFM 顺序） ----------------
        // DFM: tsBase Caption='普通'
        tsBase = new TTabSheet("普通");
        // DFM: tsInfo Caption='信息' ImageIndex=1
        tsInfo = new TTabSheet("信息");
        // DFM: tsMagic BorderWidth=6 Caption='技能' ImageIndex=2
        tsMagic = new TTabSheet("技能");
        // DFM: tsUserItem BorderWidth=6 Caption='装备' ImageIndex=3
        tsUserItem = new TTabSheet("装备");
        // DFM: tsFenghao BorderWidth=6 Caption='称号' ImageIndex=7
        tsFenghao = new TTabSheet("称号");
        // DFM: tsSorage BorderWidth=6 Caption='仓库' ImageIndex=4
        tsSorage = new TTabSheet("仓库");
        // DFM: tsVarU BorderWidth=6 Caption='U变量' ImageIndex=5
        tsVarU = new TTabSheet("U变量");        // DFM: Caption = 'U'#21464#37327 → 'U' + '变量'
        // DFM: tsVarT BorderWidth=6 Caption='T变量' ImageIndex=6
        tsVarT = new TTabSheet("T变量");        // DFM: Caption = 'T'#21464#37327 → 'T' + '变量'

        TTabSheet[] tabs = { tsBase, tsInfo, tsMagic, tsUserItem, tsFenghao, tsSorage, tsVarU, tsVarT };
        for (int i = 0; i < tabs.Length; i++) tabs[i].DfmIndex = i;
        PageControl.TabPages.AddRange(tabs);

        // ---------------- tsBase ----------------
        // DFM: lbl2 Left=10 Top=36 Width=54 Caption='人物名称:'
        lbl2 = new Label { Left = 10, Top = 36, Width = 54, Height = 12, Text = "人物名称:" };
        // DFM: lbl3 Left=10 Top=60 Caption='登录帐号:'
        lbl3 = new Label { Left = 10, Top = 60, Width = 54, Height = 12, Text = "登录帐号:" };
        // DFM: lbl4 Left=10 Top=84 Caption='仓库密码:'
        lbl4 = new Label { Left = 10, Top = 84, Width = 54, Height = 12, Text = "仓库密码:" };
        // DFM: lbl5 Left=10 Top=108 Caption='配偶名称:'
        lbl5 = new Label { Left = 10, Top = 108, Width = 54, Height = 12, Text = "配偶名称:" };
        // DFM: lbl6 Left=10 Top=132 Caption='师徒名称:'
        lbl6 = new Label { Left = 10, Top = 132, Width = 54, Height = 12, Text = "师徒名称:" };
        // DFM: lbl1 Left=10 Top=12 Caption='索引号码:'
        lbl1 = new Label { Left = 10, Top = 12, Width = 54, Height = 12, Text = "索引号码:" };
        // DFM: lbl7 Left=202 Top=12 Caption='当前地图:'
        lbl7 = new Label { Left = 202, Top = 12, Width = 54, Height = 12, Text = "当前地图:" };
        // DFM: lbl8 Left=202 Top=36 Caption='当前座标:'
        lbl8 = new Label { Left = 202, Top = 36, Width = 54, Height = 12, Text = "当前座标:" };
        // DFM: lbl9 Left=202 Top=60 Caption='回城地图:'
        lbl9 = new Label { Left = 202, Top = 60, Width = 54, Height = 12, Text = "回城地图:" };
        // DFM: lbl10 Left=202 Top=84 Caption='回城座标:'
        lbl10 = new Label { Left = 202, Top = 84, Width = 54, Height = 12, Text = "回城座标:" };

        // DFM: edtChrName Left=66 Top=32 Width=97 Height=20 Color=cl3DLight ReadOnly=True TabOrder=0
        edtChrName = new TextBox { Left = 66, Top = 32, Width = 97, Height = 20, BackColor = System.Drawing.SystemColors.Info, ReadOnly = true, TabIndex = 0 };
        // DFM: edtAccount Left=66 Top=56 Width=97 Height=20 Color=cl3DLight ReadOnly=True TabOrder=1
        edtAccount = new TextBox { Left = 66, Top = 56, Width = 97, Height = 20, BackColor = System.Drawing.SystemColors.Info, ReadOnly = true, TabIndex = 1 };
        // DFM: edtPassword Left=66 Top=80 Width=97 Height=20 TabOrder=2 OnChange=edtPasswordChange
        edtPassword = new TextBox { Left = 66, Top = 80, Width = 97, Height = 20, TabIndex = 2 };
        // DFM: edtDearName Left=66 Top=104 Width=97 Height=20 TabOrder=3 OnChange=edtPasswordChange
        edtDearName = new TextBox { Left = 66, Top = 104, Width = 97, Height = 20, TabIndex = 3 };
        // DFM: edtMasterName Left=66 Top=128 Width=97 Height=20 TabOrder=4 OnChange=edtPasswordChange
        edtMasterName = new TextBox { Left = 66, Top = 128, Width = 97, Height = 20, TabIndex = 4 };
        // DFM: edtID Left=66 Top=8 Width=97 Height=20 Color=cl3DLight ReadOnly=True TabOrder=5
        edtID = new TextBox { Left = 66, Top = 8, Width = 97, Height = 20, BackColor = System.Drawing.SystemColors.Info, ReadOnly = true, TabIndex = 5 };
        // DFM: edtCurMap Left=258 Top=8 Width=97 Height=20 TabOrder=6 OnChange=edtPasswordChange
        edtCurMap = new TextBox { Left = 258, Top = 8, Width = 97, Height = 20, TabIndex = 6 };
        // DFM: seCurX Left=258 Top=32 Width=49 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=7 OnChange=edtPasswordChange
        seCurX = new TSpinEditEx { Left = 258, Top = 32, Width = 49, Height = 21, TabIndex = 7 };
        seCurX.SetDfmRange(0, 0);
        // DFM: seCurY Left=306 Top=32 Width=49 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=8 OnChange=edtPasswordChange
        seCurY = new TSpinEditEx { Left = 306, Top = 32, Width = 49, Height = 21, TabIndex = 8 };
        seCurY.SetDfmRange(0, 0);
        // DFM: edtHomeMap Left=258 Top=56 Width=97 Height=20 TabOrder=9 OnClick=edtPasswordChange
        edtHomeMap = new TextBox { Left = 258, Top = 56, Width = 97, Height = 20, TabIndex = 9 };
        // DFM: seHomeX Left=258 Top=80 Width=49 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=10 OnChange=edtPasswordChange
        seHomeX = new TSpinEditEx { Left = 258, Top = 80, Width = 49, Height = 21, TabIndex = 10 };
        seHomeX.SetDfmRange(0, 0);
        // DFM: seHomeY Left=306 Top=80 Width=49 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=11 OnChange=edtPasswordChange
        seHomeY = new TSpinEditEx { Left = 306, Top = 80, Width = 49, Height = 21, TabIndex = 11 };
        seHomeY.SetDfmRange(0, 0);
        // DFM: chkIsMaster Left=66 Top=152 Width=57 Height=17 Caption='师父' TabOrder=12 OnClick=edtPasswordChange
        chkIsMaster = new CheckBox { Left = 66, Top = 152, Width = 57, Height = 17, Text = "师父", TabIndex = 12 };

        // ---------------- tsInfo ----------------
        // DFM: lbl11 Left=10 Top=12 Width=42 Caption='等  级:'
        lbl11 = new Label { Left = 10, Top = 12, Width = 42, Height = 12, Text = "等  级:" };
        // DFM: lbl12 Left=10 Top=36 Width=42 Caption='金  币:'
        lbl12 = new Label { Left = 10, Top = 36, Width = 42, Height = 12, Text = "金  币:" };
        // DFM: lbl13 Left=10 Top=60 Width=42 Caption='元  宝:'
        lbl13 = new Label { Left = 10, Top = 60, Width = 42, Height = 12, Text = "元  宝:" };
        // DFM: lbl14 Left=10 Top=84 Width=42 Caption='游戏点:'
        lbl14 = new Label { Left = 10, Top = 84, Width = 42, Height = 12, Text = "游戏点:" };
        // DFM: lbl18 Left=10 Top=184 Width=42 Caption='声望点:'
        lbl18 = new Label { Left = 10, Top = 184, Width = 42, Height = 12, Text = "声望点:" };
        // DFM: lbl17 Left=10 Top=160 Width=42 Caption='充值点:'
        lbl17 = new Label { Left = 10, Top = 160, Width = 42, Height = 12, Text = "充值点:" };
        // DFM: lbl19 Left=10 Top=208 Width=30 Caption='PK点:'
        lbl19 = new Label { Left = 10, Top = 208, Width = 30, Height = 12, Text = "PK点:" };
        // DFM: lbl20 Left=10 Top=232 Width=42 Caption='贡献度:'
        lbl20 = new Label { Left = 10, Top = 232, Width = 42, Height = 12, Text = "贡献度:" };
        // DFM: lbl15 Left=10 Top=111 Width=42 Caption='金刚石:'
        lbl15 = new Label { Left = 10, Top = 111, Width = 42, Height = 12, Text = "金刚石:" };
        // DFM: lbl16 Left=10 Top=136 Width=42 Caption='灵  符:'
        lbl16 = new Label { Left = 10, Top = 136, Width = 42, Height = 12, Text = "灵  符:" };

        // DFM: seLevel Left=54 Top=8 Width=80 Height=21 MaxValue=65535 MinValue=0 Value=0 TabOrder=0 OnChange=edtPasswordChange
        seLevel = new TSpinEditEx { Left = 54, Top = 8, Width = 80, Height = 21, TabIndex = 0 };
        seLevel.SetDfmRange(0, 65535);
        // DFM: seGold Left=54 Top=32 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=1 OnChange=edtPasswordChange
        seGold = new TSpinEditLongWord { Left = 54, Top = 32, Width = 80, Height = 21, TabIndex = 1 };
        seGold.SetDfmRange(0, 0);
        // DFM: seGameGold Left=54 Top=56 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=2 OnChange=edtPasswordChange
        seGameGold = new TSpinEditLongWord { Left = 54, Top = 56, Width = 80, Height = 21, TabIndex = 2 };
        seGameGold.SetDfmRange(0, 0);
        // DFM: seGamePoint Left=54 Top=80 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=3 OnChange=edtPasswordChange
        seGamePoint = new TSpinEditEx { Left = 54, Top = 80, Width = 80, Height = 21, TabIndex = 3 };
        seGamePoint.SetDfmRange(0, 0);
        // DFM: seCreditPoint Left=54 Top=179 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=4 OnChange=edtPasswordChange
        seCreditPoint = new TSpinEditEx { Left = 54, Top = 179, Width = 80, Height = 21, TabIndex = 4 };
        seCreditPoint.SetDfmRange(0, 0);
        // DFM: sePayPoint Left=54 Top=155 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=5 OnChange=edtPasswordChange
        sePayPoint = new TSpinEditEx { Left = 54, Top = 155, Width = 80, Height = 21, TabIndex = 5 };
        sePayPoint.SetDfmRange(0, 0);
        // DFM: sePKPoint Left=54 Top=203 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=6 OnChange=edtPasswordChange
        sePKPoint = new TSpinEditEx { Left = 54, Top = 203, Width = 80, Height = 21, TabIndex = 6 };
        sePKPoint.SetDfmRange(0, 0);
        // DFM: seContribution Left=54 Top=227 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=7 OnChange=edtPasswordChange
        seContribution = new TSpinEditEx { Left = 54, Top = 227, Width = 80, Height = 21, TabIndex = 7 };
        seContribution.SetDfmRange(0, 0);
        // DFM: seGameDiamond Left=54 Top=107 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=9 OnChange=edtPasswordChange
        seGameDiamond = new TSpinEditLongWord { Left = 54, Top = 107, Width = 80, Height = 21, TabIndex = 9 };
        seGameDiamond.SetDfmRange(0, 0);
        // DFM: seGameGird Left=54 Top=131 Width=80 Height=21 MaxValue=0 MinValue=0 Value=0 TabOrder=10 OnChange=edtPasswordChange
        seGameGird = new TSpinEditLongWord { Left = 54, Top = 131, Width = 80, Height = 21, TabIndex = 10 };
        seGameGird.SetDfmRange(0, 0);

        // DFM: GroupBox6 Left=162 Top=4 Width=195 Height=163 Caption='属性点' TabOrder=8
        GroupBox6 = new GroupBox { Left = 162, Top = 4, Width = 195, Height = 163, Text = "属性点", TabIndex = 8 };

        // DFM: lbl22 Left=11 Top=45 Width=18 Caption='DC:'
        lbl22 = new Label { Left = 11, Top = 45, Width = 18, Height = 12, Text = "DC:" };
        // DFM: lbl23 Left=11 Top=67 Width=18 Caption='MC:'
        lbl23 = new Label { Left = 11, Top = 67, Width = 18, Height = 12, Text = "MC:" };
        // DFM: lbl24 Left=11 Top=90 Width=18 Caption='SC:'
        lbl24 = new Label { Left = 11, Top = 90, Width = 18, Height = 12, Text = "SC:" };
        // DFM: lbl25 Left=11 Top=112 Width=18 Caption='AC:'
        lbl25 = new Label { Left = 11, Top = 112, Width = 18, Height = 12, Text = "AC:" };
        // DFM: lbl26 Left=11 Top=136 Width=24 Caption='MAC:'
        lbl26 = new Label { Left = 11, Top = 136, Width = 24, Height = 12, Text = "MAC:" };
        // DFM: lbl27 Left=95 Top=45 Width=18 Caption='HP:'
        lbl27 = new Label { Left = 95, Top = 45, Width = 18, Height = 12, Text = "HP:" };
        // DFM: lbl28 Left=95 Top=68 Width=18 Caption='MP:'
        lbl28 = new Label { Left = 95, Top = 68, Width = 18, Height = 12, Text = "MP:" };
        // DFM: lbl29 Left=95 Top=90 Width=24 Caption='Hit:'
        lbl29 = new Label { Left = 95, Top = 90, Width = 24, Height = 12, Text = "Hit:" };
        // DFM: lbl30 Left=95 Top=112 Width=36 Caption='Speed:'
        lbl30 = new Label { Left = 95, Top = 112, Width = 36, Height = 12, Text = "Speed:" };
        // DFM: lbl31 Left=95 Top=136 Width=18 Caption='X2:'
        lbl31 = new Label { Left = 95, Top = 136, Width = 18, Height = 12, Text = "X2:" };
        // DFM: lbl21 Left=11 Top=21 Width=66 Caption='可用属性点:'
        lbl21 = new Label { Left = 11, Top = 21, Width = 66, Height = 12, Text = "可用属性点:" };

        // DFM: EditDC Left=35 Top=41 Width=54 Height=21 Enabled=False MaxValue=0 MinValue=0 Value=0 TabOrder=0 OnChange=edtPasswordChange
        EditDC = new TSpinEditEx { Left = 35, Top = 41, Width = 54, Height = 21, Enabled = false, TabIndex = 0 };
        EditDC.SetDfmRange(0, 0);
        // DFM: EditMC Left=35 Top=63 Width=54 Height=21 Enabled=False TabOrder=1 OnChange=edtPasswordChange
        EditMC = new TSpinEditEx { Left = 35, Top = 63, Width = 54, Height = 21, Enabled = false, TabIndex = 1 };
        EditMC.SetDfmRange(0, 0);
        // DFM: EditSC Left=35 Top=85 Width=54 Height=21 Enabled=False TabOrder=2 OnChange=edtPasswordChange
        EditSC = new TSpinEditEx { Left = 35, Top = 85, Width = 54, Height = 21, Enabled = false, TabIndex = 2 };
        EditSC.SetDfmRange(0, 0);
        // DFM: EditAC Left=35 Top=109 Width=54 Height=21 Enabled=False TabOrder=3 OnChange=edtPasswordChange
        EditAC = new TSpinEditEx { Left = 35, Top = 109, Width = 54, Height = 21, Enabled = false, TabIndex = 3 };
        EditAC.SetDfmRange(0, 0);
        // DFM: EditMAC Left=35 Top=133 Width=54 Height=21 Enabled=False TabOrder=4 OnChange=edtPasswordChange
        EditMAC = new TSpinEditEx { Left = 35, Top = 133, Width = 54, Height = 21, Enabled = false, TabIndex = 4 };
        EditMAC.SetDfmRange(0, 0);
        // DFM: EditHP Left=130 Top=41 Width=54 Height=21 Enabled=False TabOrder=5 OnChange=edtPasswordChange
        EditHP = new TSpinEditEx { Left = 130, Top = 41, Width = 54, Height = 21, Enabled = false, TabIndex = 5 };
        EditHP.SetDfmRange(0, 0);
        // DFM: EditMP Left=130 Top=63 Width=54 Height=21 Enabled=False TabOrder=6 OnChange=edtPasswordChange
        EditMP = new TSpinEditEx { Left = 130, Top = 63, Width = 54, Height = 21, Enabled = false, TabIndex = 6 };
        EditMP.SetDfmRange(0, 0);
        // DFM: EditHit Left=130 Top=85 Width=54 Height=21 Enabled=False TabOrder=7 OnChange=edtPasswordChange
        EditHit = new TSpinEditEx { Left = 130, Top = 85, Width = 54, Height = 21, Enabled = false, TabIndex = 7 };
        EditHit.SetDfmRange(0, 0);
        // DFM: EditSpeed Left=130 Top=109 Width=54 Height=21 Enabled=False TabOrder=8 OnChange=edtPasswordChange
        EditSpeed = new TSpinEditEx { Left = 130, Top = 109, Width = 54, Height = 21, Enabled = false, TabIndex = 8 };
        EditSpeed.SetDfmRange(0, 0);
        // DFM: EditX2 Left=130 Top=133 Width=54 Height=21 Enabled=False TabOrder=9 OnChange=edtPasswordChange
        EditX2 = new TSpinEditEx { Left = 130, Top = 133, Width = 54, Height = 21, Enabled = false, TabIndex = 9 };
        EditX2.SetDfmRange(0, 0);
        // DFM: seBonusPoint Left=78 Top=16 Width=106 Height=21 TabOrder=10 OnChange=edtPasswordChange
        seBonusPoint = new TSpinEditEx { Left = 78, Top = 16, Width = 106, Height = 21, TabIndex = 10 };
        seBonusPoint.SetDfmRange(0, 0);

        GroupBox6.Controls.Add(lbl21);
        GroupBox6.Controls.Add(lbl22);
        GroupBox6.Controls.Add(lbl23);
        GroupBox6.Controls.Add(lbl24);
        GroupBox6.Controls.Add(lbl25);
        GroupBox6.Controls.Add(lbl26);
        GroupBox6.Controls.Add(lbl27);
        GroupBox6.Controls.Add(lbl28);
        GroupBox6.Controls.Add(lbl29);
        GroupBox6.Controls.Add(lbl30);
        GroupBox6.Controls.Add(lbl31);
        GroupBox6.Controls.Add(EditDC);
        GroupBox6.Controls.Add(EditMC);
        GroupBox6.Controls.Add(EditSC);
        GroupBox6.Controls.Add(EditAC);
        GroupBox6.Controls.Add(EditMAC);
        GroupBox6.Controls.Add(EditHP);
        GroupBox6.Controls.Add(EditMP);
        GroupBox6.Controls.Add(EditHit);
        GroupBox6.Controls.Add(EditSpeed);
        GroupBox6.Controls.Add(EditX2);
        GroupBox6.Controls.Add(seBonusPoint);

        // ---------------- 4 张 TListView ----------------
        // DFM: lvMagic Left=0 Top=0 Width=484 Height=289 Align=alClient GridLines=True ReadOnly=True RowSelect=True
        //      ViewStyle=vsReport Columns: 序号(40) 技能() 技能名称(100) 等级(40) 修炼点(60) 快捷键()
        lvMagic = MakeListView(484, 289,
            "序号", 40, "技能", -1, "技能名称", 100, "等级", 40, "修炼点", 60, "快捷键", -1);
        // DFM: lvUserItem Columns: 序号(40) 装备位置(76) 装备名称(76) Idx() 序列号(80) 持久(90,taCenter) 参数(220)
        lvUserItem = MakeListView(484, 289,
            "序号", 40, "装备位置", 76, "装备名称", 76, "Idx", -1, "序列号", 80, "持久", 90, "参数", 220);
        // DFM: lvFenghaoItem Columns: 序号(40) 装备名称(76) Idx() 序列号(80) 持久(90,taCenter) 参数(220)
        lvFenghaoItem = MakeListView(484, 289,
            "序号", 40, "装备名称", 76, "Idx", -1, "序列号", 80, "持久", 90, "参数", 220);
        // DFM: lvStorage Columns: 序号(40) 装备名称(76) Idx() 序列号(80) 持久(90,taCenter) 参数(220)
        lvStorage = MakeListView(484, 289,
            "序号", 40, "装备名称", 76, "Idx", -1, "序列号", 80, "持久", 90, "参数", 220);

        // DFM: 三张物品表的「持久」列 `Alignment = taCenter`
        lvUserItem.Columns[5].TextAlign = HorizontalAlignment.Center;
        lvFenghaoItem.Columns[4].TextAlign = HorizontalAlignment.Center;
        lvStorage.Columns[4].TextAlign = HorizontalAlignment.Center;

        // ---------------- 2 张 TStringGrid ----------------
        // DFM: strGridVarU Left=0 Top=0 Width=484 Height=289 Align=alClient ColCount=2 DefaultRowHeight=20 RowCount=101
        strGridVarU = new TStringGrid { Left = 0, Top = 0, Width = 484, Height = 289, Dock = DockStyle.Fill, ColCount = 2, RowCount = 101, TabIndex = 0 };
        // DFM: strGridVarT 同上
        strGridVarT = new TStringGrid { Left = 0, Top = 0, Width = 484, Height = 289, Dock = DockStyle.Fill, ColCount = 2, RowCount = 101, TabIndex = 0 };

        // ---------------- 3 个 TButton（DFM 里挂在窗体上） ----------------
        // DFM: ButtonSaveData Left=8 Top=346 Width=81 Height=25 Caption='保存修改(&S)' TabOrder=1 OnClick=ButtonSaveDataClick
        ButtonSaveData = new Button { Left = 8, Top = 346, Width = 81, Height = 25, Text = "保存修改(&S)", TabIndex = 1 };
        // DFM: ButtonExportData Left=95 Top=346 Width=81 Height=25 Caption='导出数据(&E)' TabOrder=2 OnClick=ButtonExportDataClick
        ButtonExportData = new Button { Left = 95, Top = 346, Width = 81, Height = 25, Text = "导出数据(&E)", TabIndex = 2 };
        // DFM: ButtonImportData Left=182 Top=346 Width=81 Height=25 Caption='导入数据(&I)' TabOrder=3 OnClick=ButtonExportDataClick
        //   ★ 导入按钮在 DFM 里绑的就是 **ButtonExportDataClick**（不是笔误：该处理过程 :726 明确分支到 ProcessLoadDataformFile）
        ButtonImportData = new Button { Left = 182, Top = 346, Width = 81, Height = 25, Text = "导入数据(&I)", TabIndex = 3 };

        // DFM: SaveDialog DefaultExt='hum' Filter='人物数据 (*.hum)|*.hum' Options=[ofOverwritePrompt, ofHideReadOnly, ofEnableSizing]
        SaveDialog = new SaveFileDialog
        {
            DefaultExt = "hum",
            Filter = "人物数据 (*.hum)|*.hum",
            OverwritePrompt = true      // DFM: ofOverwritePrompt（ofHideReadOnly/ofEnableSizing 在 WinForms 无对应项）
        };
        // DFM: OpenDialog DefaultExt='hum' Filter='人物数据 (*.hum)|*.hum'
        OpenDialog = new OpenFileDialog { DefaultExt = "hum", Filter = "人物数据 (*.hum)|*.hum" };

        // ---------------- 控件树装配（DFM 的父子关系；对账计数依赖它） ----------------
        tsBase.Controls.Add(lbl1);
        tsBase.Controls.Add(lbl2);
        tsBase.Controls.Add(lbl3);
        tsBase.Controls.Add(lbl4);
        tsBase.Controls.Add(lbl5);
        tsBase.Controls.Add(lbl6);
        tsBase.Controls.Add(lbl7);
        tsBase.Controls.Add(lbl8);
        tsBase.Controls.Add(lbl9);
        tsBase.Controls.Add(lbl10);
        tsBase.Controls.Add(edtID);
        tsBase.Controls.Add(edtChrName);
        tsBase.Controls.Add(edtAccount);
        tsBase.Controls.Add(edtPassword);
        tsBase.Controls.Add(edtDearName);
        tsBase.Controls.Add(edtMasterName);
        tsBase.Controls.Add(edtCurMap);
        tsBase.Controls.Add(seCurX);
        tsBase.Controls.Add(seCurY);
        tsBase.Controls.Add(edtHomeMap);
        tsBase.Controls.Add(seHomeX);
        tsBase.Controls.Add(seHomeY);
        tsBase.Controls.Add(chkIsMaster);

        tsInfo.Controls.Add(lbl11);
        tsInfo.Controls.Add(lbl12);
        tsInfo.Controls.Add(lbl13);
        tsInfo.Controls.Add(lbl14);
        tsInfo.Controls.Add(lbl15);
        tsInfo.Controls.Add(lbl16);
        tsInfo.Controls.Add(lbl17);
        tsInfo.Controls.Add(lbl18);
        tsInfo.Controls.Add(lbl19);
        tsInfo.Controls.Add(lbl20);
        tsInfo.Controls.Add(seLevel);
        tsInfo.Controls.Add(seGold);
        tsInfo.Controls.Add(seGameGold);
        tsInfo.Controls.Add(seGamePoint);
        tsInfo.Controls.Add(seCreditPoint);
        tsInfo.Controls.Add(sePayPoint);
        tsInfo.Controls.Add(sePKPoint);
        tsInfo.Controls.Add(seContribution);
        tsInfo.Controls.Add(GroupBox6);
        tsInfo.Controls.Add(seGameDiamond);
        tsInfo.Controls.Add(seGameGird);

        tsMagic.Controls.Add(lvMagic);
        tsUserItem.Controls.Add(lvUserItem);
        tsFenghao.Controls.Add(lvFenghaoItem);
        tsSorage.Controls.Add(lvStorage);
        tsVarU.Controls.Add(strGridVarU);
        tsVarT.Controls.Add(strGridVarT);

        Controls.Add(lbl11111);
        Controls.Add(PageControl);
        Controls.Add(ButtonSaveData);
        Controls.Add(ButtonExportData);
        Controls.Add(ButtonImportData);

        // DFM 的 `object` 名 = 托管字段名（DFM 对账工具按 `Control.Name` 计数/查找，必须逐字赋值）
        SetDfmNames();

        // DFM: FrmRoleDataEdit OnCreate=FormCreate → Load += ...
        Load += (s, e) => FormCreate(s);

        // DFM: 29 个 OnChange 都绑 edtPasswordChange
        // （4 个 TEdit：edtPassword/edtDearName/edtMasterName/edtCurMap 用 OnChange）
        edtPassword.TextChanged += (s, e) => edtPasswordChange(s);
        edtDearName.TextChanged += (s, e) => edtPasswordChange(s);
        edtMasterName.TextChanged += (s, e) => edtPasswordChange(s);
        edtCurMap.TextChanged += (s, e) => edtPasswordChange(s);
        // （25 个 Spinned：OnChange → ValueChanged）
        seCurX.ValueChanged += (s, e) => edtPasswordChange(s);
        seCurY.ValueChanged += (s, e) => edtPasswordChange(s);
        seHomeX.ValueChanged += (s, e) => edtPasswordChange(s);
        seHomeY.ValueChanged += (s, e) => edtPasswordChange(s);
        seLevel.ValueChanged += (s, e) => edtPasswordChange(s);
        seGold.ValueChanged += (s, e) => edtPasswordChange(s);
        seGameGold.ValueChanged += (s, e) => edtPasswordChange(s);
        seGamePoint.ValueChanged += (s, e) => edtPasswordChange(s);
        seCreditPoint.ValueChanged += (s, e) => edtPasswordChange(s);
        sePayPoint.ValueChanged += (s, e) => edtPasswordChange(s);
        sePKPoint.ValueChanged += (s, e) => edtPasswordChange(s);
        seContribution.ValueChanged += (s, e) => edtPasswordChange(s);
        EditDC.ValueChanged += (s, e) => edtPasswordChange(s);
        EditMC.ValueChanged += (s, e) => edtPasswordChange(s);
        EditSC.ValueChanged += (s, e) => edtPasswordChange(s);
        EditAC.ValueChanged += (s, e) => edtPasswordChange(s);
        EditMAC.ValueChanged += (s, e) => edtPasswordChange(s);
        EditHP.ValueChanged += (s, e) => edtPasswordChange(s);
        EditMP.ValueChanged += (s, e) => edtPasswordChange(s);
        EditHit.ValueChanged += (s, e) => edtPasswordChange(s);
        EditSpeed.ValueChanged += (s, e) => edtPasswordChange(s);
        EditX2.ValueChanged += (s, e) => edtPasswordChange(s);
        seBonusPoint.ValueChanged += (s, e) => edtPasswordChange(s);
        seGameDiamond.ValueChanged += (s, e) => edtPasswordChange(s);
        seGameGird.ValueChanged += (s, e) => edtPasswordChange(s);
        // DFM: 2 个 OnClick 也绑 edtPasswordChange（edtHomeMap、chkIsMaster）
        edtHomeMap.Click += (s, e) => edtPasswordChange(s);
        chkIsMaster.Click += (s, e) => edtPasswordChange(s);

        // DFM: ButtonSaveData.OnClick / ButtonExportData.OnClick / ButtonImportData.OnClick
        ButtonSaveData.Click += (s, e) => ButtonSaveDataClick(s);
        ButtonExportData.Click += (s, e) => ButtonExportDataClick(s);
        ButtonImportData.Click += (s, e) => ButtonExportDataClick(s);

        // TListView → 既有 ListViewSink（不新造适配器）
        MagicSink = new ListViewSink(lvMagic);
        UserItemSink = new ListViewSink(lvUserItem);
        FenghaoSink = new ListViewSink(lvFenghaoItem);
        StorageSink = new ListViewSink(lvStorage);
    }

    /// <summary>
    /// 把 DFM 的 `object` 名逐字赋给 `Control.Name`。
    /// ★ 对账工具（P10FormReconcile，台账 §37.3）只数**有名字**的控件（WinForms 复合控件的匿名内部
    ///   子控件不算 DFM 节点），故这一步是"88 个 object 节点 = 88 个具名对象"对账的前提。
    ///   用 `nameof` 而不是字符串字面量：字段改名即编译失败，杜绝手工漏字。
    /// </summary>
    private void SetDfmNames()
    {
        lbl11111.Name = nameof(lbl11111);
        PageControl.Name = nameof(PageControl);

        tsBase.Name = nameof(tsBase);
        tsInfo.Name = nameof(tsInfo);
        tsMagic.Name = nameof(tsMagic);
        tsUserItem.Name = nameof(tsUserItem);
        tsFenghao.Name = nameof(tsFenghao);
        tsSorage.Name = nameof(tsSorage);
        tsVarU.Name = nameof(tsVarU);
        tsVarT.Name = nameof(tsVarT);

        lbl1.Name = nameof(lbl1);
        lbl2.Name = nameof(lbl2);
        lbl3.Name = nameof(lbl3);
        lbl4.Name = nameof(lbl4);
        lbl5.Name = nameof(lbl5);
        lbl6.Name = nameof(lbl6);
        lbl7.Name = nameof(lbl7);
        lbl8.Name = nameof(lbl8);
        lbl9.Name = nameof(lbl9);
        lbl10.Name = nameof(lbl10);
        lbl11.Name = nameof(lbl11);
        lbl12.Name = nameof(lbl12);
        lbl13.Name = nameof(lbl13);
        lbl14.Name = nameof(lbl14);
        lbl15.Name = nameof(lbl15);
        lbl16.Name = nameof(lbl16);
        lbl17.Name = nameof(lbl17);
        lbl18.Name = nameof(lbl18);
        lbl19.Name = nameof(lbl19);
        lbl20.Name = nameof(lbl20);
        lbl21.Name = nameof(lbl21);
        lbl22.Name = nameof(lbl22);
        lbl23.Name = nameof(lbl23);
        lbl24.Name = nameof(lbl24);
        lbl25.Name = nameof(lbl25);
        lbl26.Name = nameof(lbl26);
        lbl27.Name = nameof(lbl27);
        lbl28.Name = nameof(lbl28);
        lbl29.Name = nameof(lbl29);
        lbl30.Name = nameof(lbl30);
        lbl31.Name = nameof(lbl31);

        edtChrName.Name = nameof(edtChrName);
        edtAccount.Name = nameof(edtAccount);
        edtPassword.Name = nameof(edtPassword);
        edtDearName.Name = nameof(edtDearName);
        edtMasterName.Name = nameof(edtMasterName);
        edtID.Name = nameof(edtID);
        edtCurMap.Name = nameof(edtCurMap);
        edtHomeMap.Name = nameof(edtHomeMap);

        seCurX.Name = nameof(seCurX);
        seCurY.Name = nameof(seCurY);
        seHomeX.Name = nameof(seHomeX);
        seHomeY.Name = nameof(seHomeY);
        seLevel.Name = nameof(seLevel);
        seGold.Name = nameof(seGold);
        seGameGold.Name = nameof(seGameGold);
        seGamePoint.Name = nameof(seGamePoint);
        seCreditPoint.Name = nameof(seCreditPoint);
        sePayPoint.Name = nameof(sePayPoint);
        sePKPoint.Name = nameof(sePKPoint);
        seContribution.Name = nameof(seContribution);
        seGameDiamond.Name = nameof(seGameDiamond);
        seGameGird.Name = nameof(seGameGird);
        seBonusPoint.Name = nameof(seBonusPoint);

        EditDC.Name = nameof(EditDC);
        EditMC.Name = nameof(EditMC);
        EditSC.Name = nameof(EditSC);
        EditAC.Name = nameof(EditAC);
        EditMAC.Name = nameof(EditMAC);
        EditHP.Name = nameof(EditHP);
        EditMP.Name = nameof(EditMP);
        EditHit.Name = nameof(EditHit);
        EditSpeed.Name = nameof(EditSpeed);
        EditX2.Name = nameof(EditX2);

        chkIsMaster.Name = nameof(chkIsMaster);
        GroupBox6.Name = nameof(GroupBox6);

        lvMagic.Name = nameof(lvMagic);
        lvUserItem.Name = nameof(lvUserItem);
        lvFenghaoItem.Name = nameof(lvFenghaoItem);
        lvStorage.Name = nameof(lvStorage);

        strGridVarU.Name = nameof(strGridVarU);
        strGridVarT.Name = nameof(strGridVarT);

        ButtonSaveData.Name = nameof(ButtonSaveData);
        ButtonExportData.Name = nameof(ButtonExportData);
        ButtonImportData.Name = nameof(ButtonImportData);

        // ★ DFM: SaveDialog / OpenDialog 是 **TComponent**（WinForms 的 SaveFileDialog/OpenFileDialog
        //   继承链上没有 `Name`，也不在 Controls 树里）⇒ 对账时它们按"字段存在"单独 +2，不计入 Name 计数。
    }

    /// <summary>
    /// 原文 `:938 seLevel.MaxValue := High(Word)` 的上界（`High(Word)` = 65535）。
    /// </summary>
    public const int SeLevelMaxValue = 65535;

    /// <summary>
    /// 复刻原文 `TSpinEditEx.CheckValue`（`SpinEditEx.pas:126-136`）的**裁剪**语义：
    /// <c>if MaxValue &lt;&gt; MinValue then</c> 把值夹到 <c>[MinValue, MaxValue]</c>；
    /// 本单元 `seLevel` 的 DFM 是 `MaxValue=65535 MinValue=0` ⇒ 裁剪区间 `[0, 65535]`。
    ///
    /// ★ D-P10-29（本车道引入；**不是**修正原文，而是**还原**原文语义）：
    ///   托管 `TSpinEdit`（`SpinControls.cs:29-33`）的 `Value` setter 直接转 `NumericUpDown.Value`，
    ///   而后者把 `Minimum/Maximum` 当**硬边界** —— 越界赋值会抛
    ///   `ArgumentOutOfRangeException`（实测：`Maximum=65535` 后赋 70000 即抛），
    ///   与原文"静默裁剪"不同。`RefreshBaseInfo`（:287/:320）正是拿**数据库记录**去喂它，
    ///   而 GXX 是"21 亿"改版、`TOAbility.Level` 完全可能 > 65535 ⇒ 托管侧会**抛异常**（Delphi 只是夹到 65535）。
    ///   故在此显式裁剪，保持原文的可观察行为。
    ///   ⇒ 根治点在 `SpinControls.cs` 的属主（把 setter 改成裁剪），见跨区项 **B-P10-20**。
    /// </summary>
    private static int ClampSeLevel(int value)
        => value < 0 ? 0 : (value > SeLevelMaxValue ? SeLevelMaxValue : value);

    /// <summary>
    /// 造一张 DFM 口径的 TListView：`ViewStyle=vsReport` + `GridLines` + `RowSelect` + `ReadOnly`。
    /// `headerPairs` 为 (标题, 宽度) 对；宽度传 -1 表示 DFM 未写 Width（保留赢控默认宽度）。
    /// </summary>
    private static ListView MakeListView(int width, int height, params object[] headerPairs)
    {
        var lv = new ListView
        {
            Left = 0,
            Top = 0,
            Width = width,
            Height = height,
            Dock = DockStyle.Fill,          // DFM: Align=alClient
            GridLines = true,               // DFM: GridLines=True
            FullRowSelect = true,           // DFM: RowSelect=True
            View = View.Details,            // DFM: ViewStyle=vsReport
            MultiSelect = false,
            HideSelection = false
        };
        for (int i = 0; i + 1 < headerPairs.Length; i += 2)
        {
            int w = (int)headerPairs[i + 1];
            if (w >= 0) lv.Columns.Add((string)headerPairs[i], w);
            else lv.Columns.Add((string)headerPairs[i]);
        }
        return lv;
    }

    /// <summary>
    /// Delphi `TTabSheet.TabVisible := Value` 的托管落地：
    /// 属性值单独承载在 <see cref="TTabSheet.TabVisible"/>，效果落成"摘除/按原索引插回 TabControl"。
    /// </summary>
    public void ApplyTabVisible(TTabSheet page, bool value)
    {
        page.TabVisible = value;
        if (value)
        {
            if (!PageControl.TabPages.Contains(page))
            {
                int at = page.DfmIndex < 0 ? PageControl.TabPages.Count : Math.Min(page.DfmIndex, PageControl.TabPages.Count);
                PageControl.TabPages.Insert(at, page);
            }
        }
        else if (PageControl.TabPages.Contains(page))
        {
            PageControl.TabPages.Remove(page);
        }
    }

    /// <summary>原文 `Format(sItemValue, [UserItem.btValue[0] ... btValue[13]])`（:405/:437/:468/:502/:533/:564/:608/:641/:684）。</summary>
    private static string ItemValueText(TUserItem UserItem)
        => string.Format(sItemValue,
            UserItem.GetBtValue(0), UserItem.GetBtValue(1), UserItem.GetBtValue(2), UserItem.GetBtValue(3),
            UserItem.GetBtValue(4), UserItem.GetBtValue(5), UserItem.GetBtValue(6), UserItem.GetBtValue(7),
            UserItem.GetBtValue(8), UserItem.GetBtValue(9), UserItem.GetBtValue(10), UserItem.GetBtValue(11),
            UserItem.GetBtValue(12), UserItem.GetBtValue(13));

    // ========================================================================================
    // 原文过程逐条移植
    // ========================================================================================

    /// <summary>uFrmRoleDataEdit.pas:189-225 <c>procedure TFrmRoleDataEdit.DoOpen;</c></summary>
    public void DoOpen()
    {
        strGridVarU.SetCells(0, 0, "变量名");
        strGridVarU.SetCells(1, 0, "变量值");

        strGridVarT.SetCells(0, 0, "变量名");
        strGridVarT.SetCells(1, 0, "变量值");

        ApplyTabVisible(tsVarU, FIsHuman);       // 原文 :199 tsVarU.TabVisible := FIsHuman
        ApplyTabVisible(tsVarT, FIsHuman);       // 原文 :200 tsVarT.TabVisible := FIsHuman

        if (FIsHuman)
        {
            Text = string.Format("编辑人物数据 [{0}]", FHumData.ChrName);       // 原文 :204

            // 原文 :206 `strGridVarU.RowCount := Length(FHumData.UValues) + 1;`
            //   ★ Length 作用于**静态数组** ⇒ 恒为 500（不是"已用变量个数"）—— 原文如此。
            strGridVarU.RowCount = RoleDataEditConst.UValuesCount + 1;
            for (int I = 0; I < RoleDataEditConst.UValuesCount; I++)
            {
                // 原文 :209 `strGridVarU.Cells[0, I + 1] := 'U' + IntToStr(I);`
                strGridVarU.SetCells(0, I + 1, "U" + DelphiRTL.IntToStr(I));
            }

            // 原文 :212 `strGridVarT.RowCount := Length(FHumData.TValues) + 1;`
            strGridVarT.RowCount = RoleDataEditConst.TValuesCount + 1;
            for (int I = 0; I < RoleDataEditConst.TValuesCount; I++)
            {
                // 原文 :215 `strGridVarT.Cells[0, I + 1] := 'T' + IntToStr(I);`
                strGridVarT.SetCells(0, I + 1, "T" + DelphiRTL.IntToStr(I));
            }
        }
        else
        {
            Text = string.Format("编辑英雄数据 [{0}]", FHeroData.ChrName);     // 原文 :220（原文该行无分号）
        }

        RefreshShow();                          // 原文 :223
        PageControl.SelectedIndex = 0;          // 原文 :224 `PageControl.ActivePageIndex := 0;`（WinForms 等价属性）
    }

    /// <summary>uFrmRoleDataEdit.pas:227-323 <c>procedure TFrmRoleDataEdit.RefreshBaseInfo;</c></summary>
    public void RefreshBaseInfo()
    {
        //------------------------------------------------------------
        // 原文 :230-248 先整体清零。
        // ★ 这些赋值在原文里**会触发 OnChange**（TEdit.Text / TSpinEdit.Value 变化时），
        //   即清零块本身会写回 FHumData（edtPassword→sStoragePwd、edtDearName→sDearName …）。
        //   托管侧 TextChanged/ValueChanged 的触发条件与 Delphi 一致（值真的变了才触发），故照抄即可。
        edtPassword.Text = "";
        edtDearName.Text = "";
        edtMasterName.Text = "";
        chkIsMaster.Checked = false;

        edtHomeMap.Text = "";
        seHomeX.Value = 0;
        seHomeY.Value = 0;

        seGold.Value = 0;
        seGameGold.Value = 0;
        seGamePoint.Value = 0;
        sePayPoint.Value = 0;
        seCreditPoint.Value = 0;

        seContribution.Value = 0;
        seBonusPoint.Value = 0;
        seGameDiamond.Value = 0;
        seGameGird.Value = 0;

        edtPassword.Enabled = FIsHuman;
        edtDearName.Enabled = FIsHuman;
        edtMasterName.Enabled = FIsHuman;
        chkIsMaster.Enabled = FIsHuman;

        edtHomeMap.Enabled = FIsHuman;
        seHomeX.Enabled = FIsHuman;
        seHomeY.Enabled = FIsHuman;

        seGold.Enabled = FIsHuman;
        seGameGold.Enabled = FIsHuman;
        seGamePoint.Enabled = FIsHuman;
        sePayPoint.Enabled = FIsHuman;
        seCreditPoint.Enabled = FIsHuman;

        seContribution.Enabled = FIsHuman;
        seBonusPoint.Enabled = FIsHuman;
        seGameDiamond.Enabled = FIsHuman;
        seGameGird.Enabled = FIsHuman;

        if (FIsHuman)
        {
            edtChrName.Text = FHumData.ChrName;                 // :272 sChrName
            edtAccount.Text = FHumData.Account;                 // :273 sAccount
            edtPassword.Text = FHumData.StoragePwd;             // :274 sStoragePwd
            edtDearName.Text = FHumData.DearName;               // :275 sDearName
            edtMasterName.Text = FHumData.MasterName;           // :276 sMasterName
            chkIsMaster.Checked = TBool.ToBool(FHumData.boMaster);   // :277 boMaster

            edtCurMap.Text = FHumData.CurMap;                   // :279 sCurMap
            seCurX.Value = FHumData.wCurX;                      // :280
            seCurY.Value = FHumData.wCurY;                      // :281

            edtHomeMap.Text = FHumData.HomeMap;                 // :283 sHomeMap
            seHomeX.Value = FHumData.wHomeX;                    // :284
            seHomeY.Value = FHumData.wHomeY;                    // :285

            seLevel.Value = ClampSeLevel(FHumData.Abil.Level);  // :287（★ D-P10-29：托管壳越界会抛，原文是裁剪）
            seGold.Value = FHumData.nGold;                      // :288
            seGameGold.Value = FHumData.nGameGold;              // :289
            seGamePoint.Value = (int)FHumData.nGamePoint;       // :290（原文 LongWord→Integer 隐式同宽转换）
            sePayPoint.Value = FHumData.nPayMentPoint;          // :291
            seCreditPoint.Value = FHumData.Abil.CreditPoint;    // :292
            sePKPoint.Value = FHumData.nPKPoint;                // :293
            seContribution.Value = FHumData.wContribution;      // :294

            seBonusPoint.Value = FHumData.nBonusPoint;          // :296
            seGameDiamond.Value = FHumData.nGameDiamond;        // :297
            seGameGird.Value = FHumData.nGameGird;              // :298

            EditDC.Value = FHumData.BonusAbil.DC;               // :300
            EditMC.Value = FHumData.BonusAbil.MC;               // :301
            EditSC.Value = FHumData.BonusAbil.SC;               // :302
            EditAC.Value = FHumData.BonusAbil.AC;               // :303
            EditMAC.Value = FHumData.BonusAbil.MAC;             // :304
            EditHP.Value = FHumData.BonusAbil.HP;               // :305
            EditMP.Value = FHumData.BonusAbil.MP;               // :306
            EditHit.Value = FHumData.BonusAbil.Hit;             // :307
            EditSpeed.Value = FHumData.BonusAbil.Speed;         // :308
            EditX2.Value = FHumData.BonusAbil.X2;               // :309
        }
        else
        {
            edtChrName.Text = FHeroData.ChrName;                // :313 sChrName
            //edtAccount.Text := FHeroData.sAccount;            // :314 原文**被注释掉**

            edtCurMap.Text = FHeroData.CurMap;                  // :316 sCurMap
            seCurX.Value = FHeroData.wCurX;                     // :317
            seCurY.Value = FHeroData.wCurY;                     // :318

            seLevel.Value = ClampSeLevel(FHeroData.Abil.Level);  // :320（★ D-P10-29 同上）
            sePKPoint.Value = FHeroData.nPKPoint;               // :321
        }
    }

    /// <summary>uFrmRoleDataEdit.pas:325-337 <c>procedure TFrmRoleDataEdit.RefreshShow;</c></summary>
    public void RefreshShow()
    {
        RefreshBaseInfo();
        RefreshMagicInfo();
        RefreshUserItems();
        RefreshFenghaoItems();
        RefreshStorages();

        if (FIsHuman)
        {
            RefreshUserVar();
        }
    }

    /// <summary>uFrmRoleDataEdit.pas:339-378 <c>procedure TFrmRoleDataEdit.RefreshMagicInfo;</c></summary>
    public void RefreshMagicInfo()
    {
        MagicSink.Clear();                      // :345 lvMagic.Clear
        if (FIsHuman)
        {
            for (int I = 0; I < RoleDataEditConst.MagicsCount; I++)     // :348 Low..High(FHumData.Magics) = 0..47
            {
                THumMagic MagicInfo = FHumData.Magics[I];               // :350
                if (MagicInfo.wMagIdx == 0) break;                      // :351

                MagicSink.AddRow(DelphiRTL.IntToStr(I), null,           // :353-354
                    DelphiRTL.IntToStr(MagicInfo.wMagIdx),              // :355
                    RoleDataEditDbShareSeam.GetMagicName(MagicInfo.wMagIdx, MagicInfo.MagicAttr),   // :356
                    DelphiRTL.IntToStr(MagicInfo.btLevel),              // :357
                    DelphiRTL.IntToStr(MagicInfo.nTranPoint),           // :358
                    DelphiRTL.IntToStr(MagicInfo.btKey));               // :359
            }
        }
        else
        {
            for (int I = 0; I < RoleDataEditConst.MagicsCount; I++)     // :364
            {
                THumMagic MagicInfo = FHeroData.Magics[I];              // :366
                if (MagicInfo.wMagIdx == 0) break;                      // :367

                MagicSink.AddRow(DelphiRTL.IntToStr(I), null,           // :369-370
                    DelphiRTL.IntToStr(MagicInfo.wMagIdx),              // :371
                    RoleDataEditDbShareSeam.GetMagicName(MagicInfo.wMagIdx, MagicInfo.MagicAttr),   // :372
                    DelphiRTL.IntToStr(MagicInfo.btLevel),              // :373
                    DelphiRTL.IntToStr(MagicInfo.nTranPoint),           // :374
                    DelphiRTL.IntToStr(MagicInfo.btKey));               // :375
            }
        }
    }

    /// <summary>
    /// uFrmRoleDataEdit.pas:380-582 <c>procedure TFrmRoleDataEdit.RefreshUserItems;</c>
    /// （人物：HumItems → JewelryBoxItems → GodBlessItems；英雄：同一顺序读 FHeroData）。
    /// </summary>
    public void RefreshUserItems()
    {
        UserItemSink.Clear();                   // :389 lvUserItem.Clear
        if (FIsHuman)
        {
            for (int I = 0; I < RoleDataEditConst.HumItemsCount; I++)       // :392
            {
                TUserItem UserItem = FHumData.HumItems[I];                  // :394
                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :395

                UserItemSink.AddRow(DelphiRTL.IntToStr(I), null,            // :397-398
                    RoleDataEditConst.TItemWhereNames[I],                   // :399
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),// :400
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                // :401
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                 // :402
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),   // :403
                    ItemValueText(UserItem));                               // :405-420
            }

            for (int I = 0; I < RoleDataEditConst.JewelryBoxItemsCount; I++)   // :424
            {
                TUserItem UserItem = FHumData.JewelryBoxItems[I];              // :426
                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :427

                UserItemSink.AddRow(
                    // :430 `IntToStr(I + Length(FHumData.HumItems))`（Length 静态 = 30）
                    DelphiRTL.IntToStr(I + RoleDataEditConst.HumItemsCount), null,
                    "首饰盒" + DelphiRTL.IntToStr(I + 1),                       // :431
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),    // :432
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                   // :433
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                    // :434
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),  // :435
                    ItemValueText(UserItem));                                  // :437-452
            }

            for (int I = 0; I < RoleDataEditConst.GodBlessItemsCount; I++)      // :455
            {
                TUserItem UserItem = FHumData.GodBlessItems[I];                 // :457
                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :458

                UserItemSink.AddRow(
                    // :461 `IntToStr(I + Length(HumItems) + Length(JewelryBoxItems))`（30 + 6）
                    DelphiRTL.IntToStr(I + RoleDataEditConst.HumItemsCount + RoleDataEditConst.JewelryBoxItemsCount), null,
                    "神佑盒" + DelphiRTL.IntToStr(I + 1),                       // :462
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),    // :463
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                   // :464
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                    // :465
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),  // :466
                    ItemValueText(UserItem));                                  // :468-483
            }
        }
        else
        {
            for (int I = 0; I < RoleDataEditConst.HumItemsCount; I++)       // :488
            {
                TUserItem UserItem = FHeroData.HumItems[I];                 // :490
                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :492

                UserItemSink.AddRow(DelphiRTL.IntToStr(I), null,            // :494-495
                    RoleDataEditConst.TItemWhereNames[I],                   // :496
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),// :497
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                // :498
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                 // :499
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),   // :500
                    ItemValueText(UserItem));                               // :502-517
            }

            for (int I = 0; I < RoleDataEditConst.JewelryBoxItemsCount; I++)   // :520
            {
                TUserItem UserItem = FHeroData.JewelryBoxItems[I];             // :522
                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :523

                UserItemSink.AddRow(
                    // :526 `IntToStr(I + Length(FHeroData.HumItems))`
                    DelphiRTL.IntToStr(I + RoleDataEditConst.HumItemsCount), null,
                    "首饰盒" + DelphiRTL.IntToStr(I + 1),                       // :527
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),    // :528
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                   // :529
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                    // :530
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),  // :531
                    ItemValueText(UserItem));                                  // :533-548
            }

            for (int I = 0; I < RoleDataEditConst.GodBlessItemsCount; I++)      // :551
            {
                TUserItem UserItem = FHeroData.GodBlessItems[I];                // :553
                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :554

                UserItemSink.AddRow(
                    // :557 `IntToStr(I + Length(FHeroData.HumItems) + Length(FHeroData.JewelryBoxItems))`
                    DelphiRTL.IntToStr(I + RoleDataEditConst.HumItemsCount + RoleDataEditConst.JewelryBoxItemsCount), null,
                    "神佑盒" + DelphiRTL.IntToStr(I + 1),                       // :558
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),    // :559
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                   // :560
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                    // :561
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),  // :562
                    ItemValueText(UserItem));                                  // :564-579
            }
        }
    }

    /// <summary>uFrmRoleDataEdit.pas:584-659 <c>procedure TFrmRoleDataEdit.RefreshFenghaoItems;</c></summary>
    public void RefreshFenghaoItems()
    {
        FenghaoSink.Clear();                    // :592 lvFenghaoItem.Clear
        if (FIsHuman)
        {
            for (int I = 0; I < RoleDataEditConst.FengHaoItemsCount; I++)   // :595
            {
                TUserItem UserItem = FHumData.FengHaoItems[I];              // :597

                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :599

                FenghaoSink.AddRow(DelphiRTL.IntToStr(I), null,             // :601-602
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),// :603
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                // :604
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                 // :605
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),   // :606
                    ItemValueText(UserItem));                               // :608-623
            }
        }
        else
        {
            for (int I = 0; I < RoleDataEditConst.FengHaoItemsCount; I++)   // :628
            {
                TUserItem UserItem = FHeroData.FengHaoItems[I];             // :630

                if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :632

                FenghaoSink.AddRow(DelphiRTL.IntToStr(I), null,             // :634-635
                    RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),// :636
                    DelphiRTL.IntToStr(UserItem.wIndex - 1),                // :637
                    DelphiRTL.IntToStr(UserItem.MakeIndex),                 // :638
                    string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),   // :639
                    ItemValueText(UserItem));                               // :641-656
            }
        }
    }

    /// <summary>
    /// uFrmRoleDataEdit.pas:661-701 <c>procedure TFrmRoleDataEdit.RefreshStorages;</c>
    /// ★ 原文**没有** `if FIsHuman` 分支：恒读 `FHumData.StorageItems`（英雄模式也一样）—— 原文如此，
    /// 因为 `THeroData`（Grobal2.pas）里根本没有 `StorageItems` 字段。
    /// </summary>
    public void RefreshStorages()
    {
        StorageSink.Clear();                    // :669 lvStorage.Clear

        for (int I = 0; I < RoleDataEditConst.StorageItemsCount; I++)   // :671
        {
            TUserItem UserItem = FHumData.StorageItems[I];              // :673

            if ((UserItem.wIndex == 0) || (UserItem.MakeIndex == 0)) continue;   // :675

            StorageSink.AddRow(DelphiRTL.IntToStr(I), null,             // :677-678
                RoleDataEditDbShareSeam.GetStdItemName(UserItem.wIndex),// :679
                DelphiRTL.IntToStr(UserItem.wIndex - 1),                // :680
                DelphiRTL.IntToStr(UserItem.MakeIndex),                 // :681
                string.Format("{0}/{1}", UserItem.Dura, UserItem.DuraMax),   // :682
                ItemValueText(UserItem));                               // :684-699
        }
    }

    /// <summary>uFrmRoleDataEdit.pas:703-718 <c>procedure TFrmRoleDataEdit.RefreshUserVar();</c></summary>
    public void RefreshUserVar()
    {
        for (int I = 0; I < RoleDataEditConst.UValuesCount; I++)        // :708
        {
            strGridVarU.SetCells(1, I + 1, DelphiRTL.IntToStr(FHumData.UValues[I]));   // :710
        }

        for (int I = 0; I < RoleDataEditConst.TValuesCount; I++)        // :713
        {
            string S = FHumData.TValues[I].Value;                       // :715（string[100] → 托管文本）
            strGridVarT.SetCells(1, I + 1, S);                          // :716
        }
    }

    /// <summary>
    /// uFrmRoleDataEdit.pas:720-734 <c>procedure TFrmRoleDataEdit.ButtonExportDataClick(Sender: TObject);</c>
    ///
    /// ★ 判定结论（DFM `ButtonImportData.OnClick = ButtonExportDataClick` 是否为原始缺陷）：
    ///   **不是缺陷** —— 该处理过程 :722/:726 明确按 `Sender` 分支：
    ///     `Sender = ButtonExportData` → ProcessSaveDataToFile；
    ///     `Sender = ButtonImportData` → ProcessLoadDataformFile。
    ///   导入按钮绑到本过程是**有意复用**（DFM 与 :726 的分支互证）。
    /// ★ 但 :730-733 的 `else if Sender = ButtonSaveData then begin end;` 是**空分支**，
    ///   而 DFM 里 ButtonSaveData 绑的是 ButtonSaveDataClick ⇒ 该分支**永不执行**（死代码，原文如此）。
    /// </summary>
    public void ButtonExportDataClick(object? Sender)
    {
        if (Sender == ButtonExportData)                 // :722
        {
            ProcessSaveDataToFile();
        }
        else if (Sender == ButtonImportData)            // :726
        {
            ProcessLoadDataformFile();
        }
        else if (Sender == ButtonSaveData)              // :730（空体；DFM 不会走到这里 ⇒ 死分支）
        {
        }
    }

    /// <summary>uFrmRoleDataEdit.pas:736-767 <c>procedure TFrmRoleDataEdit.ProcessSaveDataToFile;</c></summary>
    public void ProcessSaveDataToFile()
    {
        if (FIsHuman)
            SaveDialog.FileName = FHumData.ChrName;         // :742
        else
            SaveDialog.FileName = FHeroData.ChrName;        // :744

        SaveDialog.InitialDirectory = ".\\";                // 原文 :746 `SaveDialog.InitialDir := '.\';`
        if (!RoleDataEditFileDialogSeam.SaveDialogExecute(this)) return;   // :747 `if not SaveDialog.Execute then Exit`
        string sSaveFileName = SaveDialog.FileName;         // :748
        int nFileHandle = File.Exists(sSaveFileName)
            ? DelphiFileIo.FileOpen(sSaveFileName, DelphiFileIo.fmOpenReadWrite | DelphiFileIo.fmShareDenyNone)   // :750
            : DelphiFileIo.FileCreate(sSaveFileName);                                                             // :752

        if (nFileHandle <= 0)                               // :754
        {
            UiSeam.MessageBox("保存文件出现错误！！！", "错误信息", TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);   // :756
            return;                                         // :757 Exit
        }

        // :760-763 `FileWrite(h, FHumData/FHeroData, SizeOf(...))`
        if (FIsHuman)
            DelphiFileIo.FileWrite(nFileHandle, StructBytes.BytesOf(FHumData), 0, StructBytes.SizeOf<THumData>());
        else
            DelphiFileIo.FileWrite(nFileHandle, StructBytes.BytesOf(FHeroData), 0, StructBytes.SizeOf<THeroData>());

        DelphiFileIo.FileClose(nFileHandle);                // :765
        UiSeam.MessageBox("角色数据导出成功！！！", "提示信息", TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION);       // :766
    }

    /// <summary>
    /// uFrmRoleDataEdit.pas:769-840 <c>procedure TFrmRoleDataEdit.ProcessLoadDataformFile;</c>
    ///
    /// 关键语义（已逐行核对 :798-835）：
    ///   · **人类分支**：整体读入 `SizeOf(THumData)` 字节后，只用**内存里的 FHumData** 覆写读入缓冲的
    ///     **5 个字段** —— `sAccount` / `sChrName` / `sDearName` / `sHeroName` / `sDeputyHeroName`
    ///     （:808-812），即这 5 个字段**保留数据库里的值**，其余字段一律取**文件**里的值；随后
    ///     `FHumData := pTHumData(ReadBuf)^`（:814）。
    ///   · **英雄分支**：同样只覆写 **2 个字段** `sAccount` / `sChrName`（:829-830），再 `FHeroData := ...`（:831）。
    ///   · :821 英雄分支也按 `SizeOf(THumData)` 申请缓冲（大于所需，无害）—— 原文如此。
    ///   · :802/:823 的短读检查 `if not FileRead(...) = SizeOf(...) then` 是**死分支**（见下方注释与测试）。
    /// </summary>
    public void ProcessLoadDataformFile()
    {
        if (FIsHuman)
            OpenDialog.FileName = FHumData.ChrName;         // :776
        else
            OpenDialog.FileName = FHeroData.ChrName;        // :778

        OpenDialog.InitialDirectory = ".\\";                // 原文 :780 `OpenDialog.InitialDir := '.\';`
        if (!RoleDataEditFileDialogSeam.OpenDialogExecute(this)) return;   // :781 `if not OpenDialog.Execute then Exit`
        string sLoadFileName = OpenDialog.FileName;         // :782

        if (!File.Exists(sLoadFileName))                    // :784
        {
            UiSeam.MessageBox("指定的文件未找到！！！", "错误信息", TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);   // :786
            return;                                         // :787 Exit
        }

        int nFileHandle = DelphiFileIo.FileOpen(sLoadFileName, DelphiFileIo.fmOpenReadWrite | DelphiFileIo.fmShareDenyNone);   // :790
        if (nFileHandle <= 0)                               // :791
        {
            UiSeam.MessageBox("打开文件出现错误！！！", "错误信息", TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);    // :793
            return;                                         // :794 Exit
        }

        if (FIsHuman)
        {
            // :800 GetMem(ReadBuf, SizeOf(THumData))
            // ★ 托管侧用**零填充** byte[] 代替 GetMem 的**未初始化**内存（见车道报告 D-P10-17）。
            byte[] ReadBuf = new byte[StructBytes.SizeOf<THumData>()];
            // 原文 :801 try / :815 finally FreeMem(ReadBuf) —— 托管侧 GC 承担，语义等价
            {
                int nRead = DelphiFileIo.FileRead(nFileHandle, ReadBuf, 0, StructBytes.SizeOf<THumData>());   // :802

                // :802 `if not FileRead(nFileHandle, ReadBuf^, SizeOf(THumData)) = SizeOf(THumData) then`
                // 原文如此（缺陷）：Delphi 的一元 `not` 优先级为**第 1 级**（最高），`=` 为**第 4 级**（最低），
                // 故该式实际解析为 `(not nRead) = SizeOf(THumData)`；`not nRead` 是**按位取反**（= -nRead-1，
                // 恒为负），而 `SizeOf(THumData)` 恒为正 ⇒ **条件恒 False，整个"读取文件出现错误"分支不可达**。
                // 逐字复刻：C# 的 `~nRead` 同为按位取反。
                if ((~nRead) == StructBytes.SizeOf<THumData>())
                {
                    UiSeam.MessageBox("读取文件出现错误！！！\r\r文件格式可能不正确", "错误信息", TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);   // :804
                    return;                                 // :805 Exit（原文在 try..finally 内 ⇒ 仍 FreeMem，但跳过 :837 FileClose）
                }

                THumData ReadData = StructBytes.FromBytes<THumData>(ReadBuf);   // :802 FileRead 的落点
                ReadData.Account = FHumData.Account;                            // :808 sAccount ← 内存值
                ReadData.ChrName = FHumData.ChrName;                            // :809 sChrName ← 内存值
                ReadData.DearName = FHumData.DearName;                          // :810 sDearName ← 内存值
                ReadData.HeroName = FHumData.HeroName;                          // :811 sHeroName ← 内存值
                ReadData.DeputyHeroName = FHumData.DeputyHeroName;              // :812 sDeputyHeroName ← 内存值

                FHumData = ReadData;                                            // :814 FHumData := pTHumData(ReadBuf)^
            }
        }
        else
        {
            // :821 GetMem(ReadBuf, SizeOf(THumData)) —— 原文如此：英雄分支也按 THumData 申请
            byte[] ReadBuf = new byte[StructBytes.SizeOf<THumData>()];
            {
                int nRead = DelphiFileIo.FileRead(nFileHandle, ReadBuf, 0, StructBytes.SizeOf<THeroData>());   // :823

                // :823 同 :802 的 `not ... = ...` 死分支（原文如此）
                if ((~nRead) == StructBytes.SizeOf<THeroData>())
                {
                    UiSeam.MessageBox("读取文件出现错误！！！\r\r文件格式可能不正确", "错误信息", TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);   // :825
                    return;                                 // :826 Exit
                }

                THeroData ReadData = StructBytes.FromBytes<THeroData>(ReadBuf);
                ReadData.Account = FHeroData.Account;                           // :829 sAccount ← 内存值
                ReadData.ChrName = FHeroData.ChrName;                           // :830 sChrName ← 内存值

                FHeroData = ReadData;                                           // :831 FHeroData := PTHeroData(ReadBuf)^
            }
        }

        DelphiFileIo.FileClose(nFileHandle);                // :837
        RefreshShow();                                     // :838
        UiSeam.MessageBox("角色数据导入成功！！！", "提示信息", TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION);       // :839
    }

    /// <summary>
    /// uFrmRoleDataEdit.pas:842-934 <c>procedure TFrmRoleDataEdit.edtPasswordChange(Sender: TObject);</c>
    /// （DFM 里共 **31** 个控件绑到它：29×OnChange + 2×OnClick）
    ///
    /// ★ 判定结论（派发说明怀疑"不按 Sender 分支 ⇒ 31 个控件每次都跑完整段"）：**该怀疑不成立**。
    ///   本过程是一条 **`if Sender = X` / `else if` 长链**，每次只命中一个分支。
    /// ★ 但链本身有 3 处原始缺陷（逐条见各分支注释 + 测试）：
    ///   1. :880 第二个 `else if Sender = seCurY`（:868 已判过）⇒ **死分支**，且 `seHomeY` 没有任何分支
    ///      ⇒ 界面上改"回城座标 Y"**永远写不回** `FHumData.wHomeY`；
    ///   2. EditDC/EditMC/EditSC/EditAC/EditMAC/EditHP/EditMP/EditHit/EditSpeed/EditX2（DFM 里 10 个
    ///      `Enabled=False`）与 `seHomeY` 共 **11 个**已绑定控件**没有对应分支** ⇒ 落到链尾无操作；
    ///   3. :891-932 的 `seGold`/`seGameGold`/`seGamePoint`/`sePayPoint`/`seCreditPoint`/`seContribution`/
    ///      `seBonusPoint`/`seGameDiamond`/`seGameGird` 分支**没有 `if FIsHuman` 守卫**，
    ///      英雄模式下仍写 `FHumData`（原文如此；因这些控件在英雄模式被 `Enabled := False`，用户改不到）。
    /// </summary>
    public void edtPasswordChange(object? Sender)
    {
        if (Sender == edtPassword)                                      // :844
        {
            FHumData.StoragePwd = DelphiRTL.Trim(edtPassword.Text);     // :846 sStoragePwd
        }
        else if (Sender == edtDearName)                                 // :848
        {
            FHumData.DearName = DelphiRTL.Trim(edtDearName.Text);       // :850 sDearName
        }
        else if (Sender == edtMasterName)                               // :852
        {
            FHumData.MasterName = DelphiRTL.Trim(edtMasterName.Text);   // :854 sMasterName
        }
        else if (Sender == chkIsMaster)                                 // :856
        {
            FHumData.boMaster = TBool.ToByte(chkIsMaster.Checked);      // :858 boMaster
        }
        else if (Sender == edtCurMap)                                   // :860
        {
            FHumData.CurMap = DelphiRTL.Trim(edtCurMap.Text);           // :862 sCurMap
        }
        else if (Sender == seCurX)                                      // :864
        {
            FHumData.wCurX = (ushort)seCurX.Value;                      // :866（Integer → Word 截断，原文 $R- 默认关）
        }
        else if (Sender == seCurY)                                      // :868
        {
            FHumData.wCurY = (ushort)seCurY.Value;                      // :870
        }
        else if (Sender == edtHomeMap)                                  // :872（DFM 用 OnClick 绑定）
        {
            FHumData.HomeMap = DelphiRTL.Trim(edtHomeMap.Text);         // :874 sHomeMap
        }
        else if (Sender == seHomeX)                                     // :876
        {
            FHumData.wHomeX = (ushort)seHomeX.Value;                    // :878
        }
        else if (Sender == seCurY)                                      // :880
        {
            // ★★ 原文如此（原始缺陷）：:868 已判过 `Sender = seCurY`，本分支**永不执行**（死代码）；
            //    而 DFM 里 `seHomeY.OnChange = edtPasswordChange` 却不匹配任何分支
            //    ⇒ 界面上修改"回城座标 Y"**不会**写回 FHumData.wHomeY。
            //    逐字保留（含 `seHomeY.Value` 这个"看起来对"的右值），并加锁测试。
            FHumData.wHomeY = (ushort)seHomeY.Value;                    // :882
        }
        else if (Sender == seLevel)                                     // :884
        {
            if (FIsHuman)
                FHumData.Abil.Level = seLevel.Value;                    // :887
            else
                FHeroData.Abil.Level = seLevel.Value;                   // :889
        }
        else if (Sender == seGold)                                      // :891（无 FIsHuman 守卫，原文如此）
        {
            FHumData.nGold = seGold.Value;                              // :893
        }
        else if (Sender == seGameGold)                                  // :895（无守卫）
        {
            FHumData.nGameGold = seGameGold.Value;                      // :897
        }
        else if (Sender == seGamePoint)                                 // :899（无守卫）
        {
            FHumData.nGamePoint = (uint)seGamePoint.Value;              // :901（Integer → LongWord 隐式同宽）
        }
        else if (Sender == sePayPoint)                                  // :903（无守卫）
        {
            FHumData.nPayMentPoint = sePayPoint.Value;                  // :905
        }
        else if (Sender == seCreditPoint)                               // :907（无守卫）
        {
            FHumData.Abil.CreditPoint = seCreditPoint.Value;            // :909
        }
        else if (Sender == sePKPoint)                                   // :911
        {
            if (FIsHuman)
                FHumData.nPKPoint = sePKPoint.Value;                    // :914
            else
                FHeroData.nPKPoint = sePKPoint.Value;                   // :916
        }
        else if (Sender == seContribution)                              // :918（无守卫）
        {
            FHumData.wContribution = (ushort)seContribution.Value;      // :920
        }
        else if (Sender == seBonusPoint)                                // :922（无守卫）
        {
            FHumData.nBonusPoint = seBonusPoint.Value;                  // :924
        }
        else if (Sender == seGameDiamond)                               // :926（无守卫）
        {
            FHumData.nGameDiamond = seGameDiamond.Value;                // :928（原文此行无分号）
        }
        else if (Sender == seGameGird)                                  // :930（无守卫）
        {
            FHumData.nGameGird = seGameGird.Value;                      // :932
        }
        // ★ 链尾没有 else：EditDC/EditMC/EditSC/EditAC/EditMAC/EditHP/EditMP/EditHit/EditSpeed/EditX2
        //   与 seHomeY 共 11 个已绑定控件落到这里，**什么都不做**（原文如此）。
    }

    /// <summary>
    /// uFrmRoleDataEdit.pas:936-939 <c>procedure TFrmRoleDataEdit.FormCreate(Sender: TObject);</c>
    /// （原文只有一行 `seLevel.MaxValue := High(Word);`）
    /// </summary>
    public void FormCreate(object? Sender)
    {
        // :938 `seLevel.MaxValue := High(Word);` = 65535
        // ★ 原文如此：DFM 里 seLevel 已经是 `MaxValue = 65535`，本行只是**重复设置同一个值**（无实际效果）。
        //   托管侧照抄；本仓 SpinControls.cs 的 TSpinEdit **没有**改属性名，
        //   故 Delphi `TSpinEditEx.MaxValue` ≡ WinForms `NumericUpDown.Maximum`。
        seLevel.Maximum = 65535;
    }

    /// <summary>uFrmRoleDataEdit.pas:941-972 <c>procedure TFrmRoleDataEdit.ButtonSaveDataClick(Sender: TObject);</c></summary>
    public void ButtonSaveDataClick(object? Sender)
    {
        if (FIsHuman)
        {
            for (int I = 0; I < RoleDataEditConst.UValuesCount; I++)            // :948
            {
                // :950 `StrToIntDef(strGridVarU.Cells[1, I + 1], 0)`
                FHumData.UValues[I] = DelphiRTL.StrToIntDef(strGridVarU.Cells(1, I + 1), 0);
            }

            for (int I = 0; I < RoleDataEditConst.TValuesCount; I++)            // :953
            {
                // :955 `FHumData.TValues[I] := strGridVarT.Cells[1, I + 1];`（string[100] 截断）
                FHumData.TValues[I].Value = strGridVarT.Cells(1, I + 1);
            }
        }

        bool IsOK;                                                              // :944
        if (FIsHuman)
        {
            // :961 `IsOK := g_RoleDB.HumanDB.Save(FID, @FHumData);`
            //   ★ THumanDBBase.Save 的公开包装**会吞异常**并保留初值 False（MySqlRoleDB.Base.cs:352-363）⇒ 照抄即可。
            IsOK = SelectClientRoleDbSeam.RequireHuman.Save(FID, ref FHumData);
        }
        else
        {
            // :965 `IsOK := g_RoleDB.HeroDB.Save(FID, @FHeroData);`
            IsOK = SelectClientRoleDbSeam.RequireHero.Save(FID, ref FHeroData);
        }

        if (IsOK)
            UiSeam.MessageBox("角色数据保存成功！！！", "提示信息", TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION);   // :969
        else
            UiSeam.MessageBox("角色数据保存失败！！！", "错误信息", TMsgBox.MB_OK + TMsgBox.MB_ICONEXCLAMATION);   // :971
    }
}
