// ============================================================================
// 源单元：Source/M2Engine/Forms/uFrmClientPlugManager.pas（151 行，GBK）
// 同源 DFM：Source/M2Engine/Forms/uFrmClientPlugManager.dfm（文本，33 行）
// 类型/过程：
//   TFrmClientPlugManager（:10-18 声明；:122-134 Open、:136-149 ButtonRefClick）
//   单元级过程 LoadPlugClientFiles（:20 / :27-120，内含嵌套过程 SearchFiles :33-84
//                                 与嵌套函数 IsDir :38-42 / IsFile :44-47）
//
// DFM 实测（:1-32）：
//   窗体 Caption='客户端插件管理'（#23458#25143#31471#25554#20214#31649#29702）、
//   Left=726 Top=423、BorderStyle=bsDialog、ClientWidth=625 ClientHeight=243、
//   Position=poMainFormCenter、Font.Height=-11 Font.Name=Tahoma、**无 OnCreate**。
//   控件 2 个：ListBoxPlugin（TListBox 9,9 518×217，ItemHeight=13）、
//              ButtonRef（TButton 534,9 81×27，Caption='刷新(&R)'）。
//   事件绑定 **1** 个（ButtonRef.OnClick）。
//
// 原文 uses（:6-7）Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls,
//   Forms, Dialogs, StdCtrls, EDCode, CheckUnit；实现段（:24）M2Share, MD5Util。
//
// 依赖处置（见 docs\并行报告-p9-m2-forms.md §0.4）：
//   · `g_PlugClientList`（M2Share.pas:3971 `TList`，元素是指针 `pTPlugClientInfo`）、
//     `g_PlugFileMD5ListText{,Len,CRC}`（M2Share.pas:3804-3806）→ 本文件静态接缝类。
//   · `TPlugClientInfo`（M2Share.pas:458-461 record，两个短串）→ 托管侧 class（原文全靠
//     `New/Dispose` 指针 + `TObject(...)` 装箱传递 ⇒ 引用语义等价）。
//   · `zEncodeString(const S: AnsiString): AnsiString`（EDcode.pas:768）→ 既有
//     `GXX.Core.Protocol.EDcode.zEncodeString(string): byte[]`（AnsiString 按字节承载）。
//   · `BufferCrc(Buffer: PAnsiChar; nSize: Integer)`（CheckUnit.pas:16）→ 既有
//     `GXX.Core.Util.CheckUnit.BufferCrc(byte[], int)`。
//   · `RivestFile`（MD5Util.pas:334 邻域，= `MD5Print(MD5File(name))`）→ 默认实现用已移植的
//     `GXX.Core.Crypto.MD5Util.MD5Buffer/MD5Print` 复刻；**缺失文件时原文返回"空内容的 MD5"
//     而非空串**（MD5File 在 CreateFile 失败时跳过 try 直接 MD5Final ⇒ d41d8cd9…），已锁测试。
//   · `UserEngine.SendPlugClientList`（UsrEngn.pas）→ 实例接缝（未接线即抛，§25.2）。
//   · `FindFirst/FindNext/FindClose`（SysUtils/Win32）→ `ISweep9FormsFileSearch` 接缝
//     （默认实现基于 Directory 枚举；测试注入假实现 ⇒ 不碰磁盘）。
//   · `Application.ProcessMessages` → `Application.DoEvents`；`Application.Terminated` → 接缝。
//
// ★ 原文缺陷/怪癖（逐字保留 + 差异断言锁定）：
//   1. `:38-42 IsDir` 嵌套函数**声明了但全单元 0 处调用** ⇒ 死代码（照抄保留）。
//   2. `:50 nFileCount := 0` 的 100 计数**只在 `FindNext` 循环里 Inc**（:66）⇒
//      `FindFirst` 命中的第一个文件不计入；且 `:64` 的循环条件含 `not Application.Terminated`。
//   3. `:81-83 finally FindClose(Info)` 在 `FindFirst` 失败时**也会调用**
//      （Info 未初始化）—— 原文如此。
//   4. `:111` 行尾有**两个分号** `sLineBreak;;`（Pascal 空语句，合法）逐字保留为注释标注。
//   5. `:118 Length(g_PlugFileMD5ListText)` 用的是**编码后**串的长度（字节口径），
//      与 `:116 g_PlugFileMD5ListTextLen := Length(S)`（编码**前**）**不是同一个量**。
//   6. `:64`/`:69` `Application.ProcessMessages` 只在"每 100 个文件"时被调用，
//      且 `Sleep(1)` 被注释掉（原文 :70）—— 照抄。
// ============================================================================

using GXX.Core.Crypto;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Sweep9.Forms;

// ---------------------------------------------------------------------------
// 原文 M2Share.pas 的记录与全局（接缝）
// ---------------------------------------------------------------------------

/// <summary>
/// 原文 `M2Share.pas:458-461 TPlugClientInfo = record
/// sFileName: string[255]; sMD5: string[32]; end`。
/// <para>
/// 托管侧用 <c>class</c>：原文全程以 `New(PlugClientInfo)` / `Dispose(...)` 的**指针**传递，
/// 并 `TObject(PlugClientInfo)` 装箱进 `TList` / `Items.Objects[]` ⇒ 引用语义等价
/// （与既有 `TDummyLogon` 的处置同款）。
/// </para>
/// </summary>
public sealed class TPlugClientInfo
{
    /// <summary>原文 `sFileName: string[255]`（未移植短串类型，按 §3.1 用 string 承载）。</summary>
    public string sFileName = "";
    /// <summary>原文 `sMD5: string[32]`。</summary>
    public string sMD5 = "";
}

/// <summary>
/// 原文 `M2Share.pas:3971 g_PlugClientList: TList = nil;`（创建点 `svMain.pas:2040`，
/// 释放点 `:2369-2370` 逐项 Dispose + Free）与
/// `M2Share.pas:3804-3806 g_PlugFileMD5ListText: AnsiString; g_PlugFileMD5ListTextLen: Integer;
/// g_PlugFileMD5ListTextCRC: LongWord;`。
/// </summary>
public static class Sweep9FormsPlugClientGlobals
{
    /// <summary>原文 `g_PlugClientList: TList`（元素为 `pTPlugClientInfo`）。**初值空表**（原文 nil ⇒ 消费方须先 Create）。</summary>
    public static readonly List<TPlugClientInfo> g_PlugClientList = new();

    /// <summary>
    /// 原文 `g_PlugFileMD5ListText: AnsiString`。承载**编码后的字节**：
    /// 原文把它交给 `BufferCrc(PAnsiChar(...), Length(...))` ⇒ 字节口径
    /// （与既有 `g_ClientAntiPlugDllString` 的处置一致）。
    /// </summary>
    public static byte[] g_PlugFileMD5ListText = Array.Empty<byte>();

    /// <summary>原文 `g_PlugFileMD5ListTextLen: Integer`（= 编码**前** `S` 的长度）。</summary>
    public static int g_PlugFileMD5ListTextLen;

    /// <summary>原文 `g_PlugFileMD5ListTextCRC: LongWord`。</summary>
    public static uint g_PlugFileMD5ListTextCRC;

    /// <summary>测试隔离：复位到原文 initial 状态。</summary>
    public static void Reset()
    {
        g_PlugClientList.Clear();
        g_PlugFileMD5ListText = Array.Empty<byte>();
        g_PlugFileMD5ListTextLen = 0;
        g_PlugFileMD5ListTextCRC = 0;
    }
}

// ---------------------------------------------------------------------------
// FindFirst/FindNext/FindClose 接缝（SysUtils / Win32）
// ---------------------------------------------------------------------------

/// <summary>原文 `TSearchRec` 的最小面（SysUtils）。<c>faDirectory = $10</c>、<c>faAnyFile = $3F</c>。</summary>
public sealed class Sweep9FormsSearchRec
{
    /// <summary>原文 `TSearchRec.Name`。</summary>
    public string Name = "";
    /// <summary>原文 `TSearchRec.Attr`。</summary>
    public int Attr;
}

/// <summary>原文 `SysUtils.FindFirst/FindNext/FindClose` 三件套的接缝。</summary>
public interface ISweep9FormsFileSearch
{
    /// <summary>原文 `FindFirst(const Path: string; Attr: Integer; var F: TSearchRec): Integer`（0 = 找到）。</summary>
    int FindFirst(string searchPath, int attr, Sweep9FormsSearchRec info);
    /// <summary>原文 `FindNext(var F: TSearchRec): Integer`（0 = 还有下一条）。</summary>
    int FindNext(Sweep9FormsSearchRec info);
    /// <summary>原文 `FindClose(var F: TSearchRec)`。</summary>
    void FindClose(Sweep9FormsSearchRec info);
}

/// <summary>
/// `FindFirst/FindNext` 的**托管默认实现**（基于 <see cref="Directory"/> 枚举，
/// 单层、非递归；掩码走 `Directory.GetFileSystemEntries` 的同一语义）。
/// <para>枚举顺序 = 文件系统返回顺序（原文同样不排序）。</para>
/// </summary>
public sealed class Sweep9FormsFileSearchDefault : ISweep9FormsFileSearch
{
    /// <summary>原文 `faDirectory = $10`。</summary>
    public const int faDirectory = 0x10;
    /// <summary>原文 `faAnyFile = $3F`。</summary>
    public const int faAnyFile = 0x3F;

    private readonly Dictionary<Sweep9FormsSearchRec, IEnumerator<string>> _open = new();

    /// <inheritdoc />
    public int FindFirst(string searchPath, int attr, Sweep9FormsSearchRec info)
    {
        string dir = Path.GetDirectoryName(searchPath) ?? "";
        string mask = Path.GetFileName(searchPath);
        if (dir.Length == 0) dir = ".";
        string[] entries;
        try
        {
            entries = Directory.GetFileSystemEntries(dir, mask);
        }
        catch
        {
            return 1;   // 原文：目录不存在等 ⇒ 返回非 0
        }
        var e = ((IEnumerable<string>)entries).GetEnumerator();
        _open[info] = e;
        if (!e.MoveNext())
        {
            _open.Remove(info);
            return 1;
        }
        Fill(info, e.Current);
        return 0;
    }

    /// <inheritdoc />
    public int FindNext(Sweep9FormsSearchRec info)
    {
        if (!_open.TryGetValue(info, out var e)) return 1;
        if (!e.MoveNext())
        {
            _open.Remove(info);
            return 1;
        }
        Fill(info, e.Current);
        return 0;
    }

    /// <inheritdoc />
    public void FindClose(Sweep9FormsSearchRec info) => _open.Remove(info);

    private static void Fill(Sweep9FormsSearchRec info, string fullPath)
    {
        info.Name = Path.GetFileName(fullPath);
        try { info.Attr = Directory.Exists(fullPath) ? faDirectory : 0; }
        catch { info.Attr = 0; }
    }
}

// ---------------------------------------------------------------------------
// 本单元的接缝袋
// ---------------------------------------------------------------------------

/// <summary>
/// `LoadPlugClientFiles`（:27-120）所需的全部接缝。
/// <para>
/// **默认值 = 生产行为**（真实文件系统 / 真实 MD5）；测试换入假实现 ⇒ 不碰磁盘。
/// 每个字段都注明对应的原文位置。
/// </para>
/// </summary>
public sealed class Sweep9FormsClientPlugSeams
{
    /// <summary>原文 `FindFirst/FindNext/FindClose`（:53/:64/:82）。</summary>
    public ISweep9FormsFileSearch FileSearch = new Sweep9FormsFileSearchDefault();

    /// <summary>原文 `RivestFile(...)`（:59/:77，MD5Util.pas）。默认 = `MD5Print(MD5File(name))` 复刻。</summary>
    public Func<string, string> RivestFile = Sweep9FormsMD5.RivestFile;

    /// <summary>原文 `ExtractFilePath(Application.ExeName)`（:96）⇒ 复用 `M2ShareState.g_sSelfFilePath`。</summary>
    public Func<string> SelfFilePath = () => M2ShareState.g_sSelfFilePath;

    /// <summary>原文 `DirectoryExists(sFileName)`（:97）⇒ 既有 `Sweep.Seam` 之外的直连（默认 Directory.Exists）。</summary>
    public Func<string, bool> DirectoryExists = Directory.Exists;

    /// <summary>原文 `CreateDir(sFileName)`（:99，SysUtils.CreateDir）。</summary>
    public Action<string> CreateDir = dir => Directory.CreateDirectory(dir);

    /// <summary>原文 `Application.ProcessMessages`（:69）。</summary>
    public Action ProcessMessages = () => System.Windows.Forms.Application.DoEvents();

    /// <summary>原文 `Application.Terminated`（:64）。生产 = 宿主退出标志；接缝：待 `svMain` 批次接入。</summary>
    public Func<bool> ApplicationTerminated = () => false;

    /// <summary>生产默认单例（`Application.Terminated` 等宿主钩子由接线方替换）。</summary>
    public static readonly Sweep9FormsClientPlugSeams Production = new();
}

// ---------------------------------------------------------------------------
// uFrmClientPlugManager.dfm 控件树
// ---------------------------------------------------------------------------

/// <summary>
/// 原文 `uFrmClientPlugManager.pas:10-18 TFrmClientPlugManager = class(TForm)` 1:1。
/// </summary>
public sealed class TFrmClientPlugManager : System.Windows.Forms.Form
{
    // ---- DFM 控件 ----
    /// <summary>DFM :16 `ListBoxPlugin: TListBox`（9,9 518×217）。</summary>
    public System.Windows.Forms.ListBox ListBoxPlugin = null!;
    /// <summary>DFM :24 `ButtonRef: TButton`（534,9 81×27，`OnClick = ButtonRefClick`）。</summary>
    public System.Windows.Forms.Button ButtonRef = null!;

    /// <summary>`Items.AddObject(..., TObject(PlugClientInfo))`（:131/:146）的载体镜像（同序同长）。</summary>
    public readonly List<object?> ItemObjects = new();

    /// <summary>本实例使用的接缝袋（默认生产行为；测试换入假实现）。</summary>
    public Sweep9FormsClientPlugSeams PlugSeams = Sweep9FormsClientPlugSeams.Production;

    /// <summary>
    /// 接缝：`UserEngine.SendPlugClientList()`（:148）。接缝：待 `UsrEngn.pas` 移植后接入。
    /// <para>未接线 = 静默跳过（与 `TFrmDummySetting`/`GamePetsForm` 的既有形态一致）；
    /// 已在报告登记为**待接线项**（不臆造中性实现）。</para>
    /// </summary>
    public Action? SendPlugClientListHandler;

    /// <summary>构造 + 装载 DFM 控件树（原文 DFM 未绑定 `OnCreate`）。</summary>
    public TFrmClientPlugManager()
    {
        InitializeComponents();
    }

    /// <summary>DFM 控件树 1:1 实例化（属性逐条取自 uFrmClientPlugManager.dfm）。</summary>
    private void InitializeComponents()
    {
        // ---- DFM :1-15 窗体自身 ----
        Name = "FrmClientPlugManager";                                // DFM object 名
        Text = "客户端插件管理";                                        // DFM Caption
        Left = 726;                                                   // DFM Left
        Top = 423;                                                    // DFM Top
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;  // DFM BorderStyle = bsDialog
        ClientSize = new System.Drawing.Size(625, 243);                // DFM ClientWidth/ClientHeight
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // DFM Position = poMainFormCenter
        MaximizeBox = false;                                          // bsDialog ⇒ 无最大化
        MinimizeBox = false;                                          // bsDialog ⇒ 无最小化
        Font = new System.Drawing.Font("Tahoma", 8.25F);              // DFM Font.Height = -11 / Name = Tahoma

        // ---- DFM :16-23 ListBoxPlugin ----
        ListBoxPlugin = new System.Windows.Forms.ListBox
        {
            Name = "ListBoxPlugin",                                    // DFM object 名
            Left = 9,
            Top = 9,
            Width = 518,
            Height = 217,
            IntegralHeight = false,                                    // DFM ItemHeight = 13
            TabIndex = 0,
        };
        Controls.Add(ListBoxPlugin);

        // ---- DFM :24-32 ButtonRef ----
        ButtonRef = new System.Windows.Forms.Button
        {
            Name = "ButtonRef",                                        // DFM object 名
            Left = 534,
            Top = 9,
            Width = 81,
            Height = 27,
            Text = "刷新(&R)",                                          // DFM Caption = '刷新(&R)'
            TabIndex = 1,
        };
        ButtonRef.Click += (_, _) => ButtonRefClick(ButtonRef);
        Controls.Add(ButtonRef);
    }

    /// <summary>
    /// 原文 `:122-134 procedure TFrmClientPlugManager.Open`。
    /// </summary>
    public void Open()
    {
        // 原文 ListBoxPlugin.Clear;
        ListBoxPlugin.Items.Clear();
        ItemObjects.Clear();

        // 原文 for I := 0 to g_PlugClientList.Count - 1 do ... AddObject(...)
        for (int I = 0; I < Sweep9FormsPlugClientGlobals.g_PlugClientList.Count; I++)
        {
            TPlugClientInfo PlugClientInfo = Sweep9FormsPlugClientGlobals.g_PlugClientList[I];
            ListBoxPlugin.Items.Add(PlugClientInfo.sFileName);
            ItemObjects.Add(PlugClientInfo);
        }
        // 原文 ShowModal;（:133，返回值被丢弃）
        Sweep9FormsMessageBoxSeam.ShowModal(this);
    }

    /// <summary>
    /// 原文 `:136-149 procedure TFrmClientPlugManager.ButtonRefClick(Sender: TObject)`。
    /// <para>原文 `Sender` 未被使用（保留形参以对齐签名）。</para>
    /// </summary>
    public void ButtonRefClick(object? Sender)
    {
        // 原文 LoadPlugClientFiles();
        LoadPlugClientFiles(PlugSeams);

        // 原文 ListBoxPlugin.Clear; for I := 0 to ... AddObject(...)
        ListBoxPlugin.Items.Clear();
        ItemObjects.Clear();
        for (int I = 0; I < Sweep9FormsPlugClientGlobals.g_PlugClientList.Count; I++)
        {
            TPlugClientInfo PlugClientInfo = Sweep9FormsPlugClientGlobals.g_PlugClientList[I];
            ListBoxPlugin.Items.Add(PlugClientInfo.sFileName);
            ItemObjects.Add(PlugClientInfo);
        }

        // 原文 UserEngine.SendPlugClientList();（:148）
        SendPlugClientListHandler?.Invoke();
    }

    /// <summary>
    /// 原文 `:27-120 procedure LoadPlugClientFiles();`（**单元级**过程，托管侧为静态方法）。
    /// </summary>
    /// <param name="seams">接缝袋；为 <c>null</c> 时用生产默认（真实文件系统）。</param>
    public static void LoadPlugClientFiles(Sweep9FormsClientPlugSeams? seams = null)
    {
        seams ??= Sweep9FormsClientPlugSeams.Production;

        // 原文 var I: Integer; sFileName, S: string; PlugClientInfo: pTPlugClientInfo;
        int I;
        string sFileName, S;
        TPlugClientInfo PlugClientInfo;

        // 原文 for I := 0 to g_PlugClientList.Count - 1 do Dispose(pTPlugClientInfo(g_PlugClientList.Items[I]));
        Sweep9FormsPlugClientGlobals.g_PlugClientList.Clear();

        // 原文 g_PlugClientList.Clear;（托管侧上面一句已清，此处保留原文顺序语义）
        Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText = Array.Empty<byte>();   // 原文 := ''（AnsiString 空）
        Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen = 0;
        Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextCRC = 0;

        // 原文 sFileName := ExtractFilePath(Application.ExeName) + 'PlugClient';
        sFileName = seams.SelfFilePath() + "PlugClient";

        // 原文 if not DirectoryExists(sFileName) then CreateDir(sFileName);
        if (!seams.DirectoryExists(sFileName))
            seams.CreateDir(sFileName);

        // 原文 SearchFiles(sFileName);
        SearchFiles(sFileName, seams);

        // 原文 S := ''; for I := 0 to g_PlugClientList.Count - 1 do ...
        S = "";
        for (I = 0; I < Sweep9FormsPlugClientGlobals.g_PlugClientList.Count; I++)
        {
            PlugClientInfo = Sweep9FormsPlugClientGlobals.g_PlugClientList[I];
            // 原文 if I = g_PlugClientList.Count - 1 then S := S + PlugClientInfo.sMD5
            //      else S := S + PlugClientInfo.sMD5 + sLineBreak;;
            //      ↑ 原文 :111 行尾是**两个分号**（Pascal 空语句）—— 逐字保留该事实。
            if (I == Sweep9FormsPlugClientGlobals.g_PlugClientList.Count - 1)
                S = S + PlugClientInfo.sMD5;
            else
                S = S + PlugClientInfo.sMD5 + Sweep9FormsKit.sLineBreak;
        }

        // 原文 if Length(S) > 0 then
        if (S.Length > 0)
        {
            Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextLen = S.Length;      // 原文 :116（编码前长度）
            Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText = EDcode.zEncodeString(S);
            // 原文 :118 BufferCrc(PAnsiChar(g_PlugFileMD5ListText), Length(g_PlugFileMD5ListText))
            //   —— 这里的 Length 是**编码后**的字节长度（与 :116 不是同一个量，原文如此）。
            Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListTextCRC =
                CheckUnit.BufferCrc(Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText,
                    Sweep9FormsPlugClientGlobals.g_PlugFileMD5ListText.Length);
        }
    }

    /// <summary>
    /// 原文 `:33-84 procedure SearchFiles(SearchDirectory: string);`（嵌套过程）。
    /// </summary>
    private static void SearchFiles(string SearchDirectory, Sweep9FormsClientPlugSeams seams)
    {
        // 原文 var Info: TSearchRec; nFileCount: Integer;
        var Info = new Sweep9FormsSearchRec();
        int nFileCount;

        // 原文 :50 nFileCount := 0;
        nFileCount = 0;
        // 原文 :51 SearchDirectory := IncludeTrailingPathDelimiter(SearchDirectory);
        SearchDirectory = IncludeTrailingPathDelimiter(SearchDirectory);

        // 原文 try ... finally FindClose(Info); end;（:52/:81-83）
        try
        {
            // 原文 if FindFirst(SearchDirectory + '*.dll', faAnyFile, Info) = 0 then
            if (seams.FileSearch.FindFirst(SearchDirectory + "*.dll", Sweep9FormsFileSearchDefault.faAnyFile, Info) == 0)
            {
                // 原文 if IsFile then（:55）—— IsFile = 非目录，且名字不是 '.'/'..'
                if (IsFile(Info))
                {
                    // 原文 New(PlugClientInfo); PlugClientInfo.sFileName := ...; sMD5 := RivestFile(...); Add(...)
                    var PlugClientInfo = new TPlugClientInfo();
                    PlugClientInfo.sFileName = SearchDirectory + Info.Name;
                    PlugClientInfo.sMD5 = seams.RivestFile(PlugClientInfo.sFileName);
                    Sweep9FormsPlugClientGlobals.g_PlugClientList.Add(PlugClientInfo);
                }
            }

            // 原文 while (FindNext(Info) = 0) and (not Application.Terminated) do
            while (seams.FileSearch.FindNext(Info) == 0 && !seams.ApplicationTerminated())
            {
                // 原文 Inc(nFileCount);（:66）—— 注意 FindFirst 命中的那条**不计入**
                nFileCount++;
                // 原文 if nFileCount mod 100 = 0 then begin Application.ProcessMessages; // Sleep(1); end;
                if (nFileCount % 100 == 0)
                {
                    seams.ProcessMessages();
                    // Sleep(1);   ← 原文 :70 被注释掉，照抄保留
                }

                // 原文 if IsFile then ...
                if (IsFile(Info))
                {
                    var PlugClientInfo = new TPlugClientInfo();
                    PlugClientInfo.sFileName = SearchDirectory + Info.Name;
                    PlugClientInfo.sMD5 = seams.RivestFile(PlugClientInfo.sFileName);
                    Sweep9FormsPlugClientGlobals.g_PlugClientList.Add(PlugClientInfo);
                }
            }
        }
        finally
        {
            // 原文 FindClose(Info);（:82 —— FindFirst 失败时也照样调用，原文如此）
            seams.FileSearch.FindClose(Info);
        }
    }

    /// <summary>
    /// 原文 `:44-47 function IsFile: Boolean`（嵌套函数）：
    /// `(not((Info.Attr and faDirectory) = faDirectory)) and (Info.Name &lt;&gt; '.') and (Info.Name &lt;&gt; '..')`。
    /// </summary>
    private static bool IsFile(Sweep9FormsSearchRec Info)
        => !((Info.Attr & Sweep9FormsFileSearchDefault.faDirectory) == Sweep9FormsFileSearchDefault.faDirectory)
           && Info.Name != "." && Info.Name != "..";

    /// <summary>
    /// 原文 `:38-42 function IsDir: Boolean`（嵌套函数）—— **全单元 0 处调用**（死代码）。
    /// <para>照抄保留为方法并在报告登记；测试用反射锁定"实现存在但零调用点"的事实。</para>
    /// </summary>
    public static bool IsDir(Sweep9FormsSearchRec Info)
        => Info.Name != "." && Info.Name != ".."
           && (Info.Attr & Sweep9FormsFileSearchDefault.faDirectory) == Sweep9FormsFileSearchDefault.faDirectory;

    /// <summary>原文 `SysUtils.IncludeTrailingPathDelimiter`（:51）。</summary>
    private static string IncludeTrailingPathDelimiter(string path)
        => path.EndsWith("\\", StringComparison.Ordinal) || path.EndsWith("/", StringComparison.Ordinal)
            ? path
            : path + "\\";
}

/// <summary>
/// `MD5Util.pas` 的 `RivestFile`（源 `Common\MD5Util.pas`：`Result := MD5Print(MD5File(FileName))`）。
/// <para>
/// ⚠ 本类型是**本车道的接缝实现**（`MD5Util.pas` 整体未移植，只移植了被 `GXX.Core` 收录的
/// 那一部分）。它由**已移植的** `MD5Util.MD5Buffer` + `MD5Util.MD5Print` 组合而成，
/// **不是替身语义**：缺文件时原文 `MD5File` 因 `CreateFile` 失败而跳过读取、直接 `MD5Final`
/// ⇒ 返回**空内容的 MD5**（`d41d8cd98f00b204e9800998ecf8427e`），本实现逐字对齐该行为。
/// </para>
/// </summary>
public static class Sweep9FormsMD5
{
    /// <summary>原文 `MD5Util.pas RivestFile(FileName: string): string`（16 字节摘要的小写十六进制）。</summary>
    public static string RivestFile(string fileName)
    {
        byte[] data;
        try
        {
            data = File.ReadAllBytes(fileName);
        }
        catch
        {
            // 原文 CreateFile 失败 ⇒ 不读入任何字节 ⇒ MD5Final 对空上下文收尾。
            data = Array.Empty<byte>();
        }
        return MD5Util.MD5Print(MD5Util.MD5Buffer(data, 0, data.Length));
    }
}
