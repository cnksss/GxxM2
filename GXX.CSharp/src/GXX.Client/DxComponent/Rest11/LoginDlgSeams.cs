using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Rtl;
using GXX.Core.Util;

// =====================================================================================
// 并行车道 p11-client-dxrest2 —— LoginDlg.pas 的**分区外依赖**接缝。
//
// 归属说明（台账 §9.3「一个类型只有一个归属」）：
//   LoginDlg.pas 的 uses 段里有 4 个单元尚未移植到 `GXX.Client`：
//     * Share.pas      —— `g_sMirDataDirectory` / `g_ClientVersion` /
//                          `g_MirDataDirectoryList` / `g_FileNameList`（原文 :32/:34/:51/:52）
//     * IniFiles.pas   —— `TIniFile`（原文 :116/:147 `TIniFile.Create(... + 'Config.ini')`）
//     * ShlObj / ShellApi —— `SelectDirectory` 用的 `SHBrowseForFolder` 家族（原文 :37-95）
//     * GameImages.pas —— `g_boD3DFormat`（原文 :120/:150/:175）
//   这些类型**本车道的分区外**，故一律只做「引用/接缝」，**绝不在本文件重定义已归属的类型**：
//     * `TClientVersion`  → 直接引用 `GXX.Client.DxComponent.TClientVersion`（DxComponentCommon.cs:123，
//                            对应原文 DxComponents.pas:27；LoginDlg.pas 的 uses 段正含该单元）。
//     * `TStringList`     → 直接引用 `GXX.Core.Util.TStringList`（GXX.Core 已归属）。
//   本文件只声明「GXX.Client 里确实还不存在」的两类东西：
//     (1) Share.pas / GameImages.pas 的**单元级全局变量**（托管侧无单元概念，按 §3.3 落为静态类）；
//     (2) IniFiles.pas 的 `TIniFile` 与 shell32 的目录对话框这两处**宿主能力接缝**。
//
// 与 `GXX.Client.DxComponent` / `GXX.Client.LoadDx` 的既有类型**无同名冲突**
// （`LoadDxNamespaceCollisionTests` 只比较 `DxComponent` 与 `LoadDx` 两个命名空间的公开简单名；
//  `Rest11` 是 `DxComponent` 下的**子命名空间**，本文件全部类型都只出现在这里）。
// =====================================================================================

namespace GXX.Client.DxComponent.Rest11;

/// <summary>
/// 接缝：<c>Share.pas</c> 的单元级全局变量（原文 :32/:34/:51/:52）。
///
/// <para><b>为什么是接缝而不是移植</b>：<c>Share.pas</c>（63 行）尚未移植，
/// 它的 4 个全局被 <c>LoginDlg.pas</c> 读写。按 §3.3「全局变量 → public static class XxxGlobal」，
/// 这里给出一份**只含 LoginDlg 真正用到的那 4 个**的承载面；待 <c>Share.pas</c> 移植后，
/// 本类整体转调它的正式归属（登记在交付报告「跨区事项」）。</para>
///
/// <para><b>已登记的既有重复（D-P11-01）</b>：<c>g_boD3DFormat</c> 在
/// <c>GXX.Client.ReadResources.GameImagesBase.g_boD3DFormat</c>（GameImagesBase.cs:442）
/// 已有一份。两条车道各自按"单元级全局"落地，故存在两份同义字段。本车道**不修改**既有文件
/// （分区外，且 GameImages 侧属其它车道战场），只登记该重复与合并方向。</para>
/// </summary>
public static class LoginDlgGlobals
{
    /// <summary>Share.pas:32 <c>g_sMirDataDirectory: string = 'D:\热血传奇\';</c>。</summary>
    public static string g_sMirDataDirectory = @"D:\热血传奇\";

    /// <summary>Share.pas:34 <c>g_ClientVersion: TClientVersion = cvSerial;</c>。</summary>
    public static TClientVersion g_ClientVersion = TClientVersion.cvSerial;

    /// <summary>Share.pas:52 <c>g_MirDataDirectoryList: array[TClientVersion] of string;</c>。
    /// <para>原文是 <c>array[TClientVersion]</c> 枚举下标数组 ⇒ 长度 = <c>Ord(High(TClientVersion)) + 1</c> = 6
    /// （cv176..cvMirNewUI205）。<b>不含</b> <c>cvMirs / cvMirReturn / cvMirReturn2</c>
    /// （原文 DxComponents.pas:27 把它们注释掉了）。</para></summary>
    public static readonly string[] g_MirDataDirectoryList = new string[ClientVersionOrdinalCount];

    /// <summary>Share.pas:51 <c>g_FileNameList: TStringList;</c>（原文在 initialization 段 Create、finalization 段 Free）。</summary>
    public static TStringList g_FileNameList = new TStringList();

    /// <summary>GameImages.pas:176 <c>g_boD3DFormat: Boolean = False;</c>（经 LoginDlg :120/:150/:175 读写）。</summary>
    public static bool g_boD3DFormat;

    /// <summary>原文 <c>High(TClientVersion) + 1</c>（DxComponents.pas:27 共 6 个枚举值）。</summary>
    public const int ClientVersionOrdinalCount = 6;

    /// <summary>
    /// <c>g_MirDataDirectoryList[TClientVersion(I)]</c> 的**越界安全**读写面。
    ///
    /// <para><b>为什么需要它（D-P11-09，原文缺陷的差异登记）</b>：DFM 的
    /// <c>RadioGroup.Items.Strings</c> 有 **7** 项（LoginDlg.dfm:51-58），而
    /// <c>TClientVersion</c> 只有 **6** 个值（DxComponents.pas:27）⇒ 第 7 项（'传奇归来'）
    /// 在 <c>RadioGroupClick</c>（原 :169-170）里会算出 <c>TClientVersion(6)</c> 并去读
    /// <c>g_MirDataDirectoryList[6]</c> —— Delphi 默认**不开范围检查**，这是越界读；
    /// 托管侧越界读会抛 <see cref="IndexOutOfRangeException"/>。
    /// 本访问器把越界定义为"返回空串 / 丢弃写入"，即"不崩但结果无意义"，与该越界读的
    /// **现象**一致（差异：原文读的是相邻内存，托管侧是空串）。</para>
    /// </summary>
    public static string GetDirectory(int index)
        => index >= 0 && index < g_MirDataDirectoryList.Length ? g_MirDataDirectoryList[index] : string.Empty;

    /// <summary><see cref="GetDirectory"/> 的写侧（越界丢弃）。</summary>
    public static void SetDirectory(int index, string value)
    {
        if (index >= 0 && index < g_MirDataDirectoryList.Length) g_MirDataDirectoryList[index] = value;
    }

    /// <summary>测试/复位用：回到 Share.pas 的 initialization 初值。</summary>
    public static void ResetForTests()
    {
        g_sMirDataDirectory = @"D:\热血传奇\";
        g_ClientVersion = TClientVersion.cvSerial;
        g_boD3DFormat = false;
        Array.Clear(g_MirDataDirectoryList, 0, g_MirDataDirectoryList.Length);
        g_FileNameList = new TStringList();
    }
}

/// <summary>
/// 接缝：<c>TFrmLogin</c> 的宿主能力（<c>Application.ExeName</c> 与 shell32 目录对话框）。
///
/// <para><b>为什么是接缝</b>：<c>Application.ExeName</c> 是 VCL 的宿主事实，
/// <c>SelectDirectory</c> 是<b>会弹模态框</b>的 shell 调用 —— 无头测试里一旦真弹框，
/// 模态消息循环会挂死 testhost（与本仓库既有的 <c>UiEnabled</c> 规程同源，
/// 见 <c>MirReturnMessageSeam</c> 的注释）。故两者都做成可注入接缝，默认值是生产行为。</para>
/// </summary>
public static class LoginDlgHost
{
    /// <summary>
    /// 接缝：<c>Application.ExeName</c>（原文 :116/:147 与 <c>ExtractFilePath</c> 联用）。
    /// 默认取托管宿主可执行文件的全路径。
    /// </summary>
    public static string ApplicationExeName = DefaultExeName();

    /// <summary>
    /// 接缝：原文 <c>SelectDirectory(const Caption: string; const Root: WideString;
    /// var Directory: string; Owner: THandle): Boolean</c>（LoginDlg.pas:44-95）。
    /// 默认实现是 1:1 的 shell32 <c>SHBrowseForFolder</c>（见本文件 §目录对话框）。
    /// </summary>
    public static Func<string, string, string, IntPtr, DirectoryPickResult> SelectDirectory = ShellDirectoryPicker.SelectDirectory;

    /// <summary>
    /// 对 <see cref="SelectDirectory"/> 的**薄包装**（等价于原文第二形参 <c>Root</c> 传 <c>''</c>
    /// 的调用形态，即 LoginDlg.pas:100 的写法）。表单代码只调这一个入口，
    /// 测试则可以直接替换 <see cref="SelectDirectory"/>。
    /// </summary>
    public static DirectoryPickResult PickDirectory(string caption, string root, string directory, IntPtr owner)
        => SelectDirectory(caption, root, directory, owner);

    /// <summary>原文 <c>DirectoryExists(Directory)</c>（LoginDlg.pas:56）。</summary>
    public static Func<string, bool> DirectoryExists = Directory.Exists;

    /// <summary>
    /// 接缝：VCL <c>Application.Handle</c>（原文 :65 <c>IDesktopFolder.ParseDisplayName(Application.Handle, ...)</c>）。
    /// 默认 <see cref="IntPtr.Zero"/>（无宿主时为 0，与 VCL 无主窗体时的取值同形）。
    /// </summary>
    public static IntPtr OwnerWindow = IntPtr.Zero;

    /// <summary>
    /// 原文 <c>ExtractFilePath(Application.ExeName) + 'Config.ini'</c>（LoginDlg.pas:116/:147）。
    /// Delphi 的 <c>ExtractFilePath</c> 取到最后一个路径分隔符（含），故与 <c>AddTrailingSeparator</c> 等价。
    /// </summary>
    public static string ConfigIniPath() => AddTrailingSeparator(ApplicationExeName) + "Config.ini";

    /// <summary>GameImages.pas:257-263 <c>ExtractFilePath</c>：<c>Copy(FileName, 1, LastDelimiter(PathDelim + DriveDelim, FileName))</c>。</summary>
    public static string AddTrailingSeparator(string fileName)
    {
        int i = LastDelimiter("\\:", fileName);
        return DelphiRTL.Copy(fileName, 1, i);
    }

    /// <summary>Delphi <c>SysUtils.LastDelimiter</c>（1-based；未命中返回 0）。</summary>
    private static int LastDelimiter(string delimiters, string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (delimiters.IndexOf(s[i]) >= 0) return i + 1;
        }
        return 0;
    }

    /// <summary>接缝默认值的来源（<c>Application.ExeName</c> 的托管对应）。</summary>
    private static string DefaultExeName()
    {
        try
        {
            return Environment.ProcessPath ?? AppContext.BaseDirectory;
        }
        catch (Exception)
        {
            // 原文如此：Application.ExeName 在任何情况下都返回串（VCL 内部缓存 ParamStr(0)）。
            return AppContext.BaseDirectory;
        }
    }

    /// <summary>测试/复位用。</summary>
    public static void ResetForTests()
    {
        ApplicationExeName = DefaultExeName();
        SelectDirectory = ShellDirectoryPicker.SelectDirectory;
        DirectoryExists = Directory.Exists;
        OwnerWindow = IntPtr.Zero;
    }
}

// -------------------------------------------------------------------------------------
// IniFiles.pas 的 TIniFile
//
// 归属与去重：`GXX.Core.Util` 现有 `TFastIniFile`（对应 FastIniFile.pas，**内存缓存 + 显式落盘**），
// 而 LoginDlg 用的是 RTL 的 IniFiles.TIniFile（**写穿**：每次 Write* 立即落盘）。
// GXX.Client 侧目前**没有**任何 TIniFile 实现；工程内另有两份（DBServer/IniFiles.cs、
// GameCenter/GameCenterIniFile.cs，均为**跨工程私有副本**）。本文件按"最小面"再造一份
// 落在本车道分区内，供 LoginDlg 与其测试使用 —— **不去改那两个文件、也不复制它们的代码**。
// 已登记（D-P11-02）：待 TIniFile 在 GXX.Core 归位后，本类应删除并改为引用。
// -------------------------------------------------------------------------------------

/// <summary>
/// Delphi <c>IniFiles.TIniFile</c> 在 <c>LoginDlg.pas</c> 用到的**最小面**（原文 :116-158）：
/// <c>ReadSection / ReadString / ReadInteger / ReadBool / WriteString / WriteInteger / WriteBool / Free</c>。
///
/// <para>类名不叫 <c>TIniFile</c>：工程内已有两份异命名空间同义类，且 GXX.Client 的
/// 多个既有文件带 <c>using GXX.Client.GUI.GameConfig;</c>；为免 CS0104（§「一个类型只有一个归属」），
/// 这里带项目前缀。</para>
///
/// <para><b>语义照抄（RTL IniFiles.pas）</b>：</para>
/// <list type="number">
/// <item><c>Create</c> 时文件不存在 → 视作空 INI（<b>不抛异常</b>）；落盘时才建文件/建目录。</item>
/// <item><c>ReadSection</c>：值列表先 <c>Clear</c>（原文如此，RTL 版清空；<c>ReadSectionValues</c> 才不清）。</item>
/// <item><c>WriteString</c> 对**空值不写入**（原文如此）；<c>WriteBool</c> 走
///   <c>WriteInteger(Ord(Value))</c> ⇒ 落盘为 <c>'1'/'0'</c>，**不是** <c>BoolToStr</c> 的 <c>'-1'/'0'</c>。</item>
/// <item>每次 <c>Write*</c> 立即落盘（写穿）。</item>
/// <item>落盘编码 GBK（原文 IniFiles 走 <c>WritePrivateProfileString</c> 的 ANSI 代码页 = CP936）。</item>
/// </list>
/// </summary>
public sealed class TLoginIniFile : IDisposable
{
    private sealed class Entry
    {
        public string Section = string.Empty;
        public string Ident = string.Empty;
        public string Value = string.Empty;
    }

    private readonly string _fileName;
    private readonly List<string> _sectionOrder = new List<string>();
    private readonly List<Entry> _entries = new List<Entry>();

    /// <summary>原文 <c>TIniFile.Create(FileName)</c>。</summary>
    public TLoginIniFile(string fileName)
    {
        _fileName = fileName ?? string.Empty;
        // 原文 TIniFile.Create → TMemIniFile.Create 时即建立空文件（IniFiles.pas）。
        //  托管侧照做：否则"同一目录两次 Create"的可见性会与原文不同。
        if (_fileName.Length != 0 && !File.Exists(_fileName))
        {
            string createDir = Path.GetDirectoryName(_fileName);
            if (!string.IsNullOrEmpty(createDir) && !Directory.Exists(createDir))
                Directory.CreateDirectory(createDir);
            File.WriteAllText(_fileName, string.Empty, EncodingInit.GBK);
        }
        Load();
    }

    public string FileName => _fileName;

    /// <summary>
    /// ★ 语义关键：原文 <c>TIniFile</c> 在 <c>Create</c> 时把文件**整体读进缓存**，
    /// 之后的 <c>ReadString</c> 只查缓存（写穿只写出去，不回流进同实例的读缓存）。
    /// 故本类构造时读一次，<c>Write*</c> 只更新缓存 + 落盘，**不再从磁盘重读**。
    /// 否则"构造 → 写 → 读"会读到别处写过的值，与原文不同。
    /// </summary>
    private void Load()
    {
        _entries.Clear();
        _sectionOrder.Clear();
        if (_fileName.Length == 0 || !File.Exists(_fileName)) return;

        string text;
        try
        {
            byte[] buffer = File.ReadAllBytes(_fileName);
            text = EncodingInit.GBK.GetString(buffer);
        }
        catch (IOException)
        {
            return;   // 原文 IniFiles 对不可读文件按"空 INI"处理（GetPrivateProfileString 的默认行为）
        }

        string current = string.Empty;
        foreach (string raw in SplitLines(text))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#")) continue;
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                current = line.Substring(1, line.Length - 2).Trim();
                if (!_sectionOrder.Contains(current)) _sectionOrder.Add(current);
                continue;
            }
            int eq = line.IndexOf('=');
            if (eq > 0 && current.Length > 0)
            {
                _entries.Add(new Entry
                {
                    Section = current,
                    Ident = line.Substring(0, eq).Trim(),
                    Value = line.Substring(eq + 1).Trim(),
                });
            }
        }
    }

    /// <summary>原文 <c>TStrings.SetTextStr</c> 的行切分（#13 / #10 / #13#10 三种都认）。</summary>
    private static IEnumerable<string> SplitLines(string text)
    {
        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != '\r' && text[i] != '\n') continue;
            yield return text.Substring(start, i - start);
            if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
            start = i + 1;
        }
        if (start < text.Length) yield return text.Substring(start);
    }

    private Entry Find(string section, string ident)
    {
        foreach (var e in _entries)
        {
            if (string.Equals(e.Section, section, StringComparison.OrdinalIgnoreCase)
                && string.Equals(e.Ident, ident, StringComparison.OrdinalIgnoreCase))
                return e;
        }
        return null;
    }

    /// <summary>原文 <c>TCustomIniFile.ReadSection(Section, Strings)</c>：先 <c>Strings.Clear</c> 再逐条加入（按文件出现顺序）。</summary>
    public void ReadSection(string section, List<string> strings)
    {
        strings.Clear();
        foreach (var e in _entries)
        {
            if (string.Equals(e.Section, section, StringComparison.OrdinalIgnoreCase))
                strings.Add(e.Ident);
        }
    }

    /// <summary>原文 <c>TCustomIniFile.ReadSection</c> 的 <c>TStrings</c> 重载（g_FileNameList 形态）。</summary>
    public void ReadSection(string section, TStringList strings)
    {
        strings.Clear();
        var keys = new List<string>();
        ReadSection(section, keys);
        foreach (string k in keys) strings.Add(k);
    }

    public string ReadString(string section, string ident, string defaultValue)
    {
        var e = Find(section, ident);
        return e == null ? defaultValue : e.Value;
    }

    /// <summary>
    /// 原文 <c>TCustomIniFile.ReadInteger</c>：先接受 <c>0x</c>/<c>0X</c> 前缀（改写为 <c>$</c>），
    /// 再按 <c>$</c> 十六进制 / 十进制解析，失败回退 Default。
    /// 与 <c>GXX.Core.Util.TFastIniFile.ReadInteger</c>（FastIniFile.cs:131-141）逐字同源。
    /// </summary>
    public int ReadInteger(string section, string ident, int defaultValue)
    {
        string s = ReadString(section, ident, string.Empty);
        if (s.Length == 0) return defaultValue;
        if (s.Length > 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X')) s = "$" + s.Substring(2);
        if (s[0] == '$')
        {
            return int.TryParse(s.Substring(1), System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out int hex) ? hex : defaultValue;
        }
        return int.TryParse(s, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out int v) ? v : defaultValue;
    }

    /// <summary>原文 <c>TCustomIniFile.ReadBool</c>：<c>ReadInteger(...Ord(Default)) &lt;&gt; 0</c> —— **任何非 0 都算 True**。</summary>
    public bool ReadBool(string section, string ident, bool defaultValue)
        => ReadInteger(section, ident, defaultValue ? 1 : 0) != 0;

    /// <summary>原文 <c>TCustomIniFile.WriteString</c>：**空值直接跳过**（不落盘），非空立即写穿。</summary>
    public void WriteString(string section, string ident, string value)
    {
        if (string.IsNullOrEmpty(value)) return;   // 原文如此：TIniFile.WriteString 对空串不写入
        var e = Find(section, ident);
        if (e == null)
        {
            if (!_sectionOrder.Contains(section)) _sectionOrder.Add(section);
            _entries.Add(new Entry { Section = section, Ident = ident, Value = value });
        }
        else
        {
            e.Value = value;
        }
        UpdateFile();
    }

    /// <summary>原文 <c>TCustomIniFile.WriteInteger</c>：<c>WriteString(Section, Ident, IntToStr(Value))</c>。</summary>
    public void WriteInteger(string section, string ident, int value)
        => WriteString(section, ident, DelphiRTL.IntToStr(value));

    /// <summary>原文 <c>TCustomIniFile.WriteBool</c>：<c>WriteInteger(Section, Ident, Ord(Value))</c> ⇒ <c>'1'/'0'</c>。</summary>
    public void WriteBool(string section, string ident, bool value)
        => WriteInteger(section, ident, value ? 1 : 0);

    public bool SectionExists(string section)
    {
        foreach (string s in _sectionOrder)
        {
            if (string.Equals(s, section, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    public bool ValueExists(string section, string ident) => Find(section, ident) != null;

    /// <summary>落盘（GBK、CRLF；节内保持首次写入顺序，节间一个空行 —— 与 Delphi TIniFile 输出同形）。</summary>
    public void UpdateFile()
    {
        if (_fileName.Length == 0) return;
        var sb = new StringBuilder();
        foreach (string section in _sectionOrder)
        {
            sb.Append('[').Append(section).Append(']').Append("\r\n");
            foreach (var e in _entries)
            {
                if (string.Equals(e.Section, section, StringComparison.OrdinalIgnoreCase))
                    sb.Append(e.Ident).Append('=').Append(e.Value).Append("\r\n");
            }
            sb.Append("\r\n");
        }
        string dir = Path.GetDirectoryName(_fileName);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_fileName, sb.ToString(), EncodingInit.GBK);
    }

    /// <summary>原文 <c>IniFile.Free</c>（写入已即时落盘，故此处无需再写）。</summary>
    public void Dispose() { }
}

/// <summary>接缝：一次目录选择的结果（原文 <c>var Directory: string</c> 是 in/out 参数，故用结构承载）。</summary>
public readonly struct DirectoryPickResult
{
    /// <summary>原文函数返回值（用户确认选中为 True）。</summary>
    public readonly bool Result;

    /// <summary>原文 <c>Directory</c> 变量的回写值（<c>Result=False</c> 时保持调用方原值）。</summary>
    public readonly string Directory;

    public DirectoryPickResult(bool result, string directory)
    {
        Result = result;
        Directory = directory ?? string.Empty;
    }
}
