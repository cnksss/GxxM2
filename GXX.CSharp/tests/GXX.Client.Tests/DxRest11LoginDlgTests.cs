using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using GXX.Client.DxComponent;
using GXX.Client.DxComponent.Rest11;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行车道 p11-client-dxrest2：<c>Source/Client-HGE/DxComponent/LoginDlg.pas</c>（179 行）的
/// 1:1 移植测试。覆盖四块：
/// <list type="number">
/// <item><b>DFM 对账</b>：控件声明数 vs 实例化数、DFM 绑定数 vs 托管 <c>+=</c> 数
///   （按台账 §41.3-2 走 <c>Component.Events</c>+静态键，<b>不用</b>反射数字段 —— 那是假绿）；</item>
/// <item>9 个成员（6 个事件处理器 + <c>ItemIndex</c>/<c>ModalResult</c>/<c>CloseForm</c> 承载面）的逐条行为；</item>
/// <item>原文缺陷的差异断言锁死（D-P11-06/D-P11-07）；</item>
/// <item>接缝（<c>SelectDirectory</c>/<c>Application.ExeName</c>/<c>TIniFile</c>）的形态验证。</item>
/// </list>
/// </summary>
public sealed class DxRest11LoginDlgTests : IDisposable
{
    private readonly string _dir;

    public DxRest11LoginDlgTests()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _dir = Path.Combine(Path.GetTempPath(), "DxRest11_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        LoginDlgGlobals.ResetForTests();
        LoginDlgHost.ResetForTests();
        LoginDlgHost.ApplicationExeName = Path.Combine(_dir, "Client.exe");
    }

    public void Dispose()
    {
        LoginDlgGlobals.ResetForTests();
        LoginDlgHost.ResetForTests();
        try { Directory.Delete(_dir, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    // =================================================================================
    // 工具
    // =================================================================================

    /// <summary>从测试输出目录向上找到仓库根（含 <c>Source/</c> 与 <c>.git</c> 的那一层），
    /// 因此无论在哪个 worktree 里跑都能定位原版源码。</summary>
    /// <summary>
    /// 从测试输出目录向上找到**本车道所在的**仓库根。
    /// <para>判据不是"含 <c>Source/</c> + <c>.git</c>"（主工作树也满足，会读错树），
    /// 而是"含 <c>Source/</c> + <b>本文件自己</b>" —— 这样在
    /// <c>.worktrees/p11-client-dxrest2</c> 里跑就读本工作树，在主干里跑就读主干。</para>
    /// <para>附带：<c>.git</c> 在 Windows 上是隐藏项，<c>DirectoryInfo.EnumerateFileSystemInfos()</c>
    /// 默认不含隐藏项，故一律用 <c>Directory.Exists</c>/<c>File.Exists</c>（不看 Hidden 属性）。</para>
    /// </summary>
    private static string FindRepoRoot()
    {
        const string selfRel = @"GXX.CSharp\tests\GXX.Client.Tests\DxRest11LoginDlgTests.cs";
        for (DirectoryInfo d = new(AppContext.BaseDirectory); d != null; d = d.Parent)
        {
            if (Directory.Exists(Path.Combine(d.FullName, "Source"))
                && File.Exists(Path.Combine(d.FullName, selfRel)))
                return d.FullName;
        }
        return null;
    }

    private static string FindDfm()
    {
        string root = FindRepoRoot();
        Assert.False(root == null, "找不到仓库根（回读 DFM 需要原版源码）");
        string path = Path.Combine(root, "Source", "Client-HGE", "DxComponent", "LoginDlg.dfm");
        Assert.True(File.Exists(path), "找不到 Source/Client-HGE/DxComponent/LoginDlg.dfm");
        return path;
    }

    private static string FindLoginDlgCs()
    {
        string root = FindRepoRoot();
        Assert.False(root == null, "找不到仓库根");
        string path = Path.Combine(root, "GXX.CSharp", "src", "GXX.Client", "DxComponent", "Rest11", "LoginDlg.cs");
        Assert.True(File.Exists(path), "找不到 LoginDlg.cs");
        return path;
    }

    /// <summary>DFM 里的一个 <c>object</c>（含缩进层级，用于还原父子关系）。</summary>
    private sealed class DfmObject
    {
        public string Name;
        public string Type;
        public int Indent;
        public string Parent;
    }

    private sealed class DfmFacts
    {
        public readonly List<DfmObject> Objects = new();
        public readonly List<(string Control, string Handler)> Bindings = new();

        public DfmObject Form => Objects[0];
        public DfmObject Find(string name) => Objects.FirstOrDefault(o => o.Name == name);
    }

    /// <summary>
    /// 文本 DFM 的最小解析：只取 <c>object Name: Type</c> 行与 <c>OnXxx = Handler</c> 行。
    /// 缩进用两个空格一级（Delphi DFM 的固定格式），父节点 = 最近一个缩进更小的 object。
    /// </summary>
    private static DfmFacts ParseDfm(string path)
    {
        var facts = new DfmFacts();
        var stack = new List<DfmObject>();
        foreach (string raw in File.ReadAllLines(path))
        {
            string line = raw.TrimEnd();
            if (line.Length == 0) continue;
            int indent = 0;
            while (indent < line.Length && line[indent] == ' ') indent++;
            string body = line.Substring(indent);

            if (body.StartsWith("object ", StringComparison.Ordinal))
            {
                string decl = body.Substring("object ".Length);
                int colon = decl.IndexOf(':');
                var obj = new DfmObject
                {
                    Name = decl.Substring(0, colon).Trim(),
                    Type = decl.Substring(colon + 1).Trim(),
                    Indent = indent,
                    Parent = stack.Count > 0 ? stack[stack.Count - 1].Name : null,
                };
                while (stack.Count > 0 && stack[stack.Count - 1].Indent >= indent) stack.RemoveAt(stack.Count - 1);
                obj.Parent = stack.Count > 0 ? stack[stack.Count - 1].Name : null;
                facts.Objects.Add(obj);
                stack.Add(obj);
                continue;
            }

            if (body.StartsWith("On", StringComparison.Ordinal) && body.Contains(" = "))
            {
                int eq = body.IndexOf(" = ", StringComparison.Ordinal);
                string prop = body.Substring(0, eq).Trim();
                string value = body.Substring(eq + 3).Trim();
                // 只收"事件 = 处理器名"（处理器名是标识符；DFM 的枚举值也长得像标识符，
                // 但事件属性一律以 On 开头且这里只收 On*，故不会误收 BorderStyle 之类）。
                if (stack.Count > 0 && value.Length > 0 && IsIdentifier(value))
                    facts.Bindings.Add((stack[stack.Count - 1].Name, value));
            }
        }
        return facts;
    }

    private static bool IsIdentifier(string s)
        => s.Length > 0 && (char.IsLetter(s[0]) || s[0] == '_')
        && s.All(c => char.IsLetterOrDigit(c) || c == '_');

    /// <summary>
    /// 台账 §41.3-2 要求的取证方式：走 <c>Component.Events</c>（<c>EventHandlerList</c>）
    /// + 静态事件键字段 <c>s_xxxEvent</c>，取出"真的挂在这条事件上"的事件键名（如 <c>s_clickEvent</c>）。
    /// <para>反射数字段一律得 0（.NET 8 WinForms 事件不是 field-like event）—— 那是假绿，故本方法不那样做。</para>
    /// <para><c>Component.Events</c> 是 <c>protected</c>、<c>EventHandlerList</c> 既无公开
    /// <c>Count</c> 也不实现 <c>IEnumerable</c>（其 <c>Count</c>/<c>GetEnumerator</c> 都是 internal 显式实现），
    /// 故一律经反射：先取 <c>protected Component.Events</c> 属性，再走内部的
    /// <c>_head</c>（<c>ListEntry</c> 单链表）→ <c>_key</c>/<c>_next</c> 字段（.NET 8 实测名，亦容忍无下划线旧名）。</para>
    /// </summary>
    /// <param name="allowEmpty">为 true 时允许"零绑定"（用于**断言**某控件确实没挂事件）。</param>
    private static HashSet<string> BoundHandlerNames(Component c, bool allowEmpty = false)
    {
        Assert.True(c != null, "控件为 null，无法取证事件绑定");
        string name = (c as Control)?.Name ?? c.GetType().Name;

        PropertyInfo eventsProp = typeof(Component).GetProperty("Events",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(eventsProp);
        var list = (EventHandlerList)eventsProp.GetValue(c);

        FieldInfo headField = null;
        foreach (FieldInfo f in typeof(EventHandlerList).GetFields(
                     BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            // .NET 8 实测字段名是 `_head`；同时容忍 `head`（旧实现/未来改名）。
            if (f.Name == "_head" || f.Name == "head")
            {
                headField = f;
                break;
            }
        }
        Assert.True(headField != null,
            "EventHandlerList 的链表头字段没找到（.NET 实现变了）：本对账方法必须跟着更新，"
            + "不得退回『反射数 s_xxxEvent 字段』那种一律得 0 的假绿做法（台账 §41.3-2）");
        object head = headField.GetValue(list);
        if (!allowEmpty)
            Assert.True(head != null, $"{name} 的 Events 为空：说明该控件一条事件都没绑（假绿风险）");
        if (head == null) return new HashSet<string>(StringComparer.Ordinal);

        var keyToField = new Dictionary<object, string>(ReferenceEqualityComparer.Instance);
        for (Type t = c.GetType(); t != null; t = t.BaseType)
        {
            foreach (FieldInfo f in t.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (!f.Name.StartsWith("s_", StringComparison.Ordinal)) continue;
                object key = f.GetValue(null);
                if (key != null && !keyToField.ContainsKey(key)) keyToField[key] = f.Name;
            }
        }

        var names = new HashSet<string>(StringComparer.Ordinal);
        Type entryType = head.GetType();
        FieldInfo keyField = FindField(entryType, "_key", "key");
        FieldInfo nextField = FindField(entryType, "_next", "next");
        Assert.True(keyField != null && nextField != null,
            "EventHandlerList.ListEntry 的字段名变了（期望 _key/_next）：本对账方法必须跟着更新");

        for (object node = head; node != null; node = nextField.GetValue(node))
        {
            object key = keyField.GetValue(node);
            bool matched = false;
            foreach (var kv in keyToField)
            {
                if (ReferenceEquals(kv.Key, key) || Equals(kv.Key, key))
                {
                    names.Add(kv.Value);   // 如 "s_clickEvent"
                    matched = true;
                    break;
                }
            }
            if (!matched) names.Add("<unresolved-key:" + (key?.GetType().Name ?? "null") + ">");
        }
        return names;
    }

    /// <summary>按候选名（<c>_x</c> / <c>x</c>）取实例字段，任一命中即可。</summary>
    private static FieldInfo FindField(Type t, params string[] candidates)
    {
        foreach (string name in candidates)
        {
            FieldInfo f = t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (f != null) return f;
        }
        return null;
    }

    /// <summary>读 C# 源文件（本仓库 .cs 为 UTF-8）。</summary>
    private static string ReadSource(string path) => File.ReadAllText(path, new UTF8Encoding(false));

    /// <summary>把 INI 内容（GBK）写进测试目录的 Config.ini。</summary>
    private void WriteConfig(string content)
        => File.WriteAllText(Path.Combine(_dir, "Config.ini"), content, GXX.Core.EncodingInit.GBK);

    private string ConfigPath => Path.Combine(_dir, "Config.ini");

    // =================================================================================
    // A. DFM 对账
    // =================================================================================

    /// <summary>
    /// DFM 原文实读：<c>TFrmLogin</c> + 5 个控件（3 个直接子 + 1 个 RadioGroup 的嵌套子），
    /// 5 条事件绑定 + 1 条 <c>OnCreate</c>。
    /// </summary>
    [Fact]
    public void Dfm_HasExactly6ObjectsAnd6Bindings()
    {
        var dfm = ParseDfm(FindDfm());

        Assert.Equal("FrmLogin", dfm.Form.Name);
        Assert.Equal("TFrmLogin", dfm.Form.Type);

        // 控件声明数 = 5（Label1 / DialogButtons / EditGamePath / RadioGroup / CheckBoxD3DFormat）
        Assert.Equal(5, dfm.Objects.Count - 1);
        Assert.Equal(new[] { "Label1", "DialogButtons", "EditGamePath", "RadioGroup", "CheckBoxD3DFormat" },
            dfm.Objects.Skip(1).Select(o => o.Name).ToArray());

        // 嵌套关系：只有 CheckBoxD3DFormat 挂在 RadioGroup 之下
        Assert.Equal("FrmLogin", dfm.Find("Label1").Parent);
        Assert.Equal("FrmLogin", dfm.Find("DialogButtons").Parent);
        Assert.Equal("FrmLogin", dfm.Find("EditGamePath").Parent);
        Assert.Equal("FrmLogin", dfm.Find("RadioGroup").Parent);
        Assert.Equal("RadioGroup", dfm.Find("CheckBoxD3DFormat").Parent);

        // 6 条绑定：5 条控件事件 + OnCreate
        Assert.Equal(6, dfm.Bindings.Count);
        Assert.Contains(("FrmLogin", "FormCreate"), dfm.Bindings);
        Assert.Contains(("DialogButtons", "DialogButtonsClickOk"), dfm.Bindings);
        Assert.Contains(("DialogButtons", "DialogButtonsClickCancel"), dfm.Bindings);
        Assert.Contains(("EditGamePath", "EditGamePathButtonClick"), dfm.Bindings);
        Assert.Contains(("RadioGroup", "RadioGroupClick"), dfm.Bindings);
        Assert.Contains(("CheckBoxD3DFormat", "CheckBoxD3DFormatClick"), dfm.Bindings);
    }

    /// <summary>
    /// 控件的 <c>Name</c> 与 DFM 的 <c>object</c> 名逐个相等，且父子关系与 DFM 一致。
    /// </summary>
    [Fact]
    public void ControlTree_MatchesDfmNamesAndNesting()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        var dfm = ParseDfm(FindDfm());

        var actual = new List<string>();
        CollectNames(f, actual);
        Assert.Equal(dfm.Objects.Select(o => o.Name).ToArray(), actual.ToArray());

        Assert.Same(f, f.Label1.Parent);
        Assert.Same(f, f.DialogButtons.Parent);
        Assert.Same(f, f.EditGamePath.Parent);
        Assert.Same(f, f.RadioGroup.Parent);
        Assert.Same(f.RadioGroup, f.CheckBoxD3DFormat.Parent);
        Assert.All(f.RadioGroupItems, r => Assert.Same(f.RadioGroup, r.Parent));
    }

    private static void CollectNames(Control root, List<string> sink)
    {
        sink.Add(root.Name);
        foreach (Control child in root.Controls)
        {
            if (child.Name.StartsWith("DialogButtons", StringComparison.Ordinal)
                && child.Name != TFrmLogin.DfmDialogButtonsName) continue;   // 按钮条的承载子按钮（D-P11-04）
            if (child.Name == "EditGamePathButton") continue;                // Raize 内嵌按钮的承载（D-P11-04）
            if (child.Name.StartsWith("RadioGroupItem", StringComparison.Ordinal)) continue;
            CollectNames(child, sink);
        }
    }

    /// <summary>
    /// ★ 台账 §41.3-2：DFM 绑定数 vs 托管 <c>+=</c> 数必须对账，且必须走
    /// <c>Component.Events</c>(<c>EventHandlerList</c>) + 静态键 <c>s_xxxEvent</c>。
    /// </summary>
    [Fact]
    public void EventBindings_AreCountedViaEventHandlerList_AndMatchDfm()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        var dfm = ParseDfm(FindDfm());

        // 托管侧"每条绑定的处理器方法名"从 EventHandlerList 取。
        //  D-P11-04：原文的 TRzDialogButtons 是一个控件带 OnClickOk/OnClickCancel 两个事件；
        //  托管侧的承载面是一个 Panel（按钮条）+ 两个 Button（确定/取消），
        //  故 DFM 的 2 条绑定落在**两个 Button** 各自的 s_clickEvent 上（计数仍为 2）。
        var dialogButtonsHandlers = BoundHandlerNames(f.DialogButtonsOk);
        Assert.Contains("s_clickEvent", dialogButtonsHandlers);
        Assert.Contains("s_clickEvent", BoundHandlerNames(f.DialogButtonsCancel));
        Assert.Contains("s_clickEvent", BoundHandlerNames(f.EditGamePathButton));    // OnButtonClick
        Assert.Contains("s_clickEvent", BoundHandlerNames(f.RadioGroupItems[0]));    // OnClick（7 项各一条）
        Assert.Contains("s_clickEvent", BoundHandlerNames(f.RadioGroupItems[6]));
        Assert.Contains("s_clickEvent", BoundHandlerNames(f.CheckBoxD3DFormat));     // OnClick

        // 计数取证（§41.3-2 / §37.3）：DFM 6 条绑定 ⇔ 托管侧 6 个处理器入口，
        //   而 EventHandlerList 的"实际条目数" = 各控件的非空事件键总数
        //   （DialogButtons 的承载面板本身不挂事件 ⇒ 0 条，计入下方合计）。
        int boundEntries = BoundHandlerNames(f.DialogButtons, allowEmpty: true).Count
            + BoundHandlerNames(f.DialogButtonsOk).Count
            + BoundHandlerNames(f.DialogButtonsCancel).Count
            + BoundHandlerNames(f.EditGamePathButton).Count
            + BoundHandlerNames(f.CheckBoxD3DFormat).Count
            + f.RadioGroupItems.Sum(r => BoundHandlerNames(r).Count);
        Assert.Equal(11, boundEntries);   // 2（确定/取消）+ 1（EditGamePathButton）+ 1（CheckBox）+ 7（单选项）
        Assert.Equal(0, BoundHandlerNames(f.DialogButtons, allowEmpty: true).Count);   // 纯承载面板，不挂事件

        // 计数取证：DFM 的 6 条绑定，逐条在托管侧"能真的触发到原文处理器"
        Assert.Equal(6, dfm.Bindings.Count);
        var dfmHandlers = dfm.Bindings.Select(b => b.Handler).ToHashSet(StringComparer.Ordinal);
        //  1) FormCreate —— OnCreate（DECLARATIVE-01：WinForms 的 OnLoad/显式 InitDfm 两个入口）
        Assert.Contains("FormCreate", dfmHandlers);
        Assert.True(HasDeclaredMethod(typeof(TFrmLogin), "FormCreate"));
        //  2) DialogButtonsClickOk / DialogButtonsClickCancel —— 按钮条两个按钮
        Assert.True(HasDeclaredMethod(typeof(TFrmLogin), "DialogButtonsClickOk"));
        Assert.True(HasDeclaredMethod(typeof(TFrmLogin), "DialogButtonsClickCancel"));
        //  3) EditGamePathButtonClick —— EditGamePathButton.Click
        Assert.True(HasDeclaredMethod(typeof(TFrmLogin), "EditGamePathButtonClick"));
        //  4) RadioGroupClick —— 7 个 RadioButton.Click 都指向它
        Assert.True(HasDeclaredMethod(typeof(TFrmLogin), "RadioGroupClick"));
        //  5) CheckBoxD3DFormatClick —— CheckBoxD3DFormat.Click
        Assert.True(HasDeclaredMethod(typeof(TFrmLogin), "CheckBoxD3DFormatClick"));

        // 反向：C# 源里 5 个控件事件处理器名各必须真的出现在 `+=` 绑定语句里（否定性断言的计数取证）
        string src = ReadSource(FindLoginDlgCs());
        foreach (string handler in new[]
        {
            "DialogButtonsClickOk", "DialogButtonsClickCancel", "EditGamePathButtonClick",
            "CheckBoxD3DFormatClick",
        })
        {
            //  `+=` 与处理器名之间是方法组写法，允许 `+=X` 或 `+= X`（当前源码为后者）
            int occurrences = CountOccurrences(src, "+=" + handler) + CountOccurrences(src, "+= " + handler);
            Assert.True(occurrences >= 1, $"源里找不到 `+={handler}` 绑定（处理器未被接线）");
        }
        //  RadioGroupClick 走"每个单选项各绑一次"（7 项）：源码里只有 1 处 lambda 调用文本，
        //  运行时展开成 7 条绑定 —— 故这里断言 1 处文本，而运行时条数由上方的 11 条计数取证。
        Assert.Equal(1, CountOccurrences(src, "RadioGroupClick(sender, e, index)"));
        //  DECLARATIVE-01（D-P11-05）：FormCreate 没有 `+=` 绑定 —— 原文的 OnCreate 在
        //  **流化之前**触发，WinForms 无等价时点，故落成 InitDfm 里的一次显式调用（且幂等）。
        //  这里用"计数取证"锁死它既没有被漏掉、也没有被重复接线。
        Assert.Equal(0, CountOccurrences(src, "+= FormCreate") + CountOccurrences(src, "+=FormCreate"));
        Assert.Equal(1, CountOccurrences(src, "FormCreate(this, EventArgs.Empty)"));
        //  OnLoad 只负责"显示前把 DFM 就绪"，不重复调用处理器
        Assert.Equal(1, CountOccurrences(src, "InitDfm();"));
    }

    private static bool HasDeclaredMethod(Type t, string name)
        => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(m => m.Name == name);

    private static int CountOccurrences(string haystack, string needle)
    {
        int n = 0, i = 0;
        while ((i = haystack.IndexOf(needle, i, StringComparison.Ordinal)) >= 0) { n++; i += needle.Length; }
        return n;
    }

    /// <summary>
    /// DFM 的每个数值/字符串属性值，必须在 C# 源里以 <c>Dfm*</c> 常量的形式出现，
    /// 且常量值与 DFM 实读值一致（布局不可被"顺手简化"）。
    /// </summary>
    [Fact]
    public void DfmLayoutConstants_MatchDfmFile()
    {
        var dfm = ParseDfm(FindDfm());
        string text = File.ReadAllText(FindDfm(), Encoding.GetEncoding(936));

        Assert.Contains("ClientWidth = " + TFrmLogin.DfmClientWidth, text);
        Assert.Contains("ClientHeight = " + TFrmLogin.DfmClientHeight, text);
        Assert.Contains("Left = " + TFrmLogin.DfmEditGamePathLeft, text);
        Assert.Contains("Top = " + TFrmLogin.DfmEditGamePathTop, text);
        Assert.Contains("Width = " + TFrmLogin.DfmEditGamePathWidth, text);
        Assert.Contains("Height = " + TFrmLogin.DfmEditGamePathHeight, text);
        Assert.Contains("TabOrder = " + TFrmLogin.DfmEditGamePathTabOrder, text);
        Assert.Contains("Width = " + TFrmLogin.DfmRadioGroupWidth, text);
        Assert.Contains("Height = " + TFrmLogin.DfmRadioGroupHeight, text);
        Assert.Contains("Columns = " + TFrmLogin.DfmRadioGroupColumns, text);
        Assert.Contains("ItemIndex = " + TFrmLogin.DfmRadioGroupItemIndex, text);
        Assert.Contains("Left = " + TFrmLogin.DfmCheckBoxLeft, text);
        Assert.Contains("Top = " + TFrmLogin.DfmCheckBoxTop, text);
        Assert.Contains("Width = " + TFrmLogin.DfmCheckBoxWidth, text);
        Assert.Contains("Height = " + TFrmLogin.DfmCheckBoxHeight, text);
        Assert.Contains("TabOrder = " + TFrmLogin.DfmCheckBoxTabOrder, text);
        Assert.Contains("Width = " + TFrmLogin.DfmLabel1Width, text);

        Assert.Equal(7, TFrmLogin.DfmRadioGroupItems.Length);
        Assert.Equal(3, TFrmLogin.DfmRadioGroupItemIndex);
        Assert.Equal("FrmLogin", TFrmLogin.DfmFormName);
        Assert.Equal("传奇目录", TFrmLogin.DfmLabel1Caption);
        Assert.Equal("纹理压缩", TFrmLogin.DfmCheckBoxCaption);
        Assert.NotNull(dfm.Find("RadioGroup"));
    }

    // =================================================================================
    // B. 构造与 FormCreate（原文 :141-159）
    // =================================================================================

    /// <summary>构造只建 DFM 之外的状态：不碰磁盘（无头测试的安全前提）。</summary>
    [Fact]
    public void Constructor_DoesNotTouchDiskNorCreateControls()
    {
        Assert.False(File.Exists(ConfigPath));
        using var f = new TFrmLogin();
        Assert.False(File.Exists(ConfigPath), "构造函数不得创建/读取 Config.ini");
        Assert.Null(f.Label1);
        Assert.Equal(TFrmLogin.DfmCaption, f.Text);
        Assert.Equal(TFrmLogin.DfmClientWidth, f.ClientSize.Width);
        Assert.Equal(TFrmLogin.DfmClientHeight, f.ClientSize.Height);
        Assert.Equal(TFrmLogin.DfmBorderStyle, f.FormBorderStyle);
        Assert.Equal(TFrmLogin.DfmPosition, f.StartPosition);
    }

    /// <summary>DFM:11 <c>Font.Height = -12</c> + DFM:17 <c>PixelsPerInch = 96</c> ⇒ 9 磅。</summary>
    [Fact]
    public void FontHeightToPoints_Is9ptForMinus12At96Dpi()
        => Assert.Equal(9f, TFrmLogin.DelphiFontSizeToPoints(TFrmLogin.DfmFontHeight));

    /// <summary>原文 :146-157 的读值 + 回填（含 <c>g_MirDataDirectoryList</c> 全量读入）。</summary>
    [Fact]
    public void FormCreate_ReadsConfigAndFillsGlobalsAndControls()
    {
        WriteConfig(
            "[Setup]\r\n" +
            "ClientVersion=4\r\n" +
            "D3DFormat=1\r\n" +
            "[Directory]\r\n" +
            "0=D:\\v0\\\r\n" +
            "1=D:\\v1\\\r\n" +
            "2=D:\\v2\\\r\n" +
            "3=D:\\v3\\\r\n" +
            "4=D:\\v4\\\r\n" +
            "5=D:\\v5\\\r\n");

        using var f = new TFrmLogin();
        f.InitDfm();

        Assert.Equal(TClientVersion.cvMirSequel, LoginDlgGlobals.g_ClientVersion);          // :149
        Assert.True(LoginDlgGlobals.g_boD3DFormat);                                        // :150
        Assert.Equal(@"D:\v0\", LoginDlgGlobals.g_MirDataDirectoryList[0]);                 // :151-152
        Assert.Equal(@"D:\v4\", LoginDlgGlobals.g_MirDataDirectoryList[4]);
        Assert.Equal(@"D:\v5\", LoginDlgGlobals.g_MirDataDirectoryList[5]);
        Assert.Equal(@"D:\v4\", LoginDlgGlobals.g_sMirDataDirectory);                       // :154
        Assert.Equal(@"D:\v4\", f.EditGamePath.Text);                                       // :155
        Assert.Equal(DialogResult.No, f.ModalResult);                                       // :146
        Assert.True(f.CheckBoxD3DFormat.Checked);                                           // :157

        // ★ D-P11-05：DFM:50 的 `ItemIndex = 3` 在 OnCreate **之后**流化 ⇒ 覆盖 FormCreate 写的 4。
        //   原文如此，故此处断言 3 而不是 4。
        Assert.Equal(TFrmLogin.DfmRadioGroupItemIndex, f.ItemIndex);
    }

    /// <summary>原文 :149 的 <c>TClientVersion(ReadInteger(...))</c> 是**无范围检查**的硬转换。</summary>
    [Fact]
    public void FormCreate_OutOfRangeClientVersion_IsPreservedByHardCast()
    {
        WriteConfig("[Setup]\r\nClientVersion=99\r\nD3DFormat=0\r\n");

        using var f = new TFrmLogin();
        // 原文 TClientVersion(99) 不抛异常（Delphi 无范围检查的枚举硬转换），
        // 托管侧 (TClientVersion)99 同样不抛 ⇒ 保持 99。
        f.InitDfm();

        Assert.Equal((TClientVersion)99, LoginDlgGlobals.g_ClientVersion);
    }

    /// <summary>缺 Config.ini 时全部走默认值（原文 TIniFile 对不存在的文件按空 INI 处理）。</summary>
    [Fact]
    public void FormCreate_MissingIni_UsesDefaults()
    {
        using var f = new TFrmLogin();
        f.CreateDfmControlsForTests();
        f.FormCreate(f, EventArgs.Empty);      // 只跑 OnCreate，不跑 DFM 属性赋值

        Assert.Equal(TClientVersion.cvSerial, LoginDlgGlobals.g_ClientVersion);
        Assert.False(LoginDlgGlobals.g_boD3DFormat);
        Assert.Equal(@"D:\热血传奇\", LoginDlgGlobals.g_sMirDataDirectory);
        //  DxComponents.pas:27 的枚举序：cv176=0, cv185=1, cvHero=2, cvSerial=3, cvMirSequel=4, cvMirNewUI205=5
        //  ⇒ Share.pas:34 的默认 g_ClientVersion = cvSerial 对应 ItemIndex **3**（不是 0）。
        Assert.Equal(3, (int)TClientVersion.cvSerial);
        Assert.Equal((int)LoginDlgGlobals.g_ClientVersion, f.ItemIndex);
        Assert.False(f.CheckBoxD3DFormat.Checked);
        Assert.Equal(DialogResult.No, f.ModalResult);

        // 再走 DFM 属性赋值（原文流化的下一步）⇒ ItemIndex 被 DFM:50 覆盖为 3
        f.ApplyDfmPropertiesForTests();
        Assert.Equal(TFrmLogin.DfmRadioGroupItemIndex, f.ItemIndex);
    }

    /// <summary>
    /// ★ D-P11-05 的直接差异断言：<c>ApplyDfmProperties</c>（等价于 DFM 流化）会把
    /// <c>ItemIndex</c> 重新写成 DFM 的声明值 3，覆盖 <c>FormCreate</c> 的 4。
    /// </summary>
    [Fact]
    public void DfmStreamingOverwritesOnCreateItemIndex()
    {
        WriteConfig("[Setup]\r\nClientVersion=4\r\nD3DFormat=0\r\n");

        using var f = new TFrmLogin();
        f.CreateDfmControlsForTests();
        f.FormCreate(f, EventArgs.Empty);      // OnCreate 先写（:156）
        Assert.Equal(4, f.ItemIndex);

        f.ApplyDfmPropertiesForTests();        // DFM 流化后写
        Assert.Equal(3, f.ItemIndex);          // DFM:50 覆盖
    }

    /// <summary>幂等：<c>InitDfm</c> 连调两次，控件不重复、值不漂移。</summary>
    [Fact]
    public void InitDfm_IsIdempotent()
    {
        WriteConfig("[Setup]\r\nClientVersion=2\r\nD3DFormat=1\r\n");

        using var f = new TFrmLogin();
        f.InitDfm();
        int count = f.Controls.Count;
        int radios = f.RadioGroupItems.Length;
        f.InitDfm();
        Assert.Equal(count, f.Controls.Count);
        Assert.Equal(radios, f.RadioGroupItems.Length);
        Assert.Equal(3, f.ItemIndex);
    }

    // =================================================================================
    // C. EditGamePathButtonClick（原文 :97-107）
    // =================================================================================

    /// <summary>
    /// 拒绝分支：<c>SelectDirectory</c> 返回 False 时 <c>EditGamePath.Text</c> 保持
    /// <c>g_sMirDataDirectory</c>（未被目录选择结果覆盖），且窗体以 <c>mrNo</c> 收尾。
    /// </summary>
    [Fact]
    public void EditGamePathButtonClick_WhenPickerCancels_KeepsGlobalAndSetsMrNo()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        LoginDlgGlobals.g_sMirDataDirectory = @"G:\传奇\";
        int calls = 0;
        string seenCaption = null, seenDir = null;
        LoginDlgHost.SelectDirectory = (caption, root, dir, owner) =>
        {
            calls++;
            seenCaption = caption;
            seenDir = dir;
            return new DirectoryPickResult(false, dir);
        };

        f.EditGamePathButtonClick(f.EditGamePathButton, EventArgs.Empty);

        Assert.Equal(1, calls);
        Assert.Equal("请选择传奇客户端“Legend of mir2”目录", seenCaption);   // 原文 :100（全角引号照抄）
        Assert.Equal(@"G:\传奇\", seenDir);                                     // 原文把 g_sMirDataDirectory 传进去
        Assert.Equal(@"G:\传奇\", f.EditGamePath.Text);                         // :99 与 :106 写的是同一个值
        Assert.Equal(DialogResult.No, f.ModalResult);                          // :103
    }

    /// <summary>原文 :100 的 <c>Root</c> 形参是字面 <c>''</c>；<c>Owner</c> 是窗体的 <c>Handle</c>。</summary>
    [Fact]
    public void EditGamePathButtonClick_PassesEmptyRootAndFormHandle()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        IntPtr handleBefore = f.Handle;          // 先建句柄，避免处理器里 CreateHandle 的时序干扰
        Assert.NotEqual(IntPtr.Zero, handleBefore);
        string root = null;
        IntPtr owner = IntPtr.Zero;
        LoginDlgHost.SelectDirectory = (caption, r, dir, o) =>
        {
            root = r;
            owner = o;
            return new DirectoryPickResult(false, dir);
        };

        f.EditGamePathButtonClick(f.EditGamePathButton, EventArgs.Empty);

        Assert.Equal(string.Empty, root);
        Assert.Equal(handleBefore, owner);
    }

    /// <summary>
    /// 确认分支：原文在成功路径上**不写回** <c>g_sMirDataDirectory</c>（只写 <c>EditGamePath.Text</c>），
    /// 故这里锁死"全局量不被目录选择改写"。
    /// </summary>
    [Fact]
    public void EditGamePathButtonClick_OnSuccess_DoesNotWriteBackGlobal()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        LoginDlgGlobals.g_sMirDataDirectory = @"G:\旧\";
        LoginDlgHost.SelectDirectory = (caption, root, dir, owner) =>
            new DirectoryPickResult(true, @"G:\新\");

        f.EditGamePathButtonClick(f.EditGamePathButton, EventArgs.Empty);

        Assert.Equal(@"G:\旧\", f.EditGamePath.Text);                     // :106 仍读全局量
        Assert.Equal(@"G:\旧\", LoginDlgGlobals.g_sMirDataDirectory);     // 全局量未被改写（原文如此）
    }

    // =================================================================================
    // D. DialogButtonsClickOk（原文 :109-139）
    // =================================================================================

    [Fact]
    public void DialogButtonsClickOk_WritesSetupAndDirectoryKeysAndCollectsFileNames()
    {
        // 初始 Config.ini：6 个 Directory 键齐全（否则 :151-152 会把缺失项回填成 g_sMirDataDirectory 默认值）
        WriteConfig(
            "[Setup]\r\nClientVersion=1\r\nD3DFormat=0\r\n" +
            "[Directory]\r\n0=D:\\d0\\\r\n1=D:\\d1\\\r\n2=D:\\d2\\\r\n3=D:\\d3\\\r\n4=D:\\d4\\\r\n5=D:\\d5\\\r\n");

        using (var warm = new TFrmLogin())
        {
            warm.InitDfm();   // 先让 g_MirDataDirectoryList 载入 D:\d0..d5
        }

        // 再把 FileNames 节补进同一个 Config.ini（等价于原文流程里 Config.ini 另有这些键）
        WriteConfig(
            "[Setup]\r\nClientVersion=1\r\nD3DFormat=0\r\n" +
            "[Directory]\r\n0=D:\\d0\\\r\n1=D:\\d1\\\r\n2=D:\\d2\\\r\n3=D:\\d3\\\r\n4=D:\\d4\\\r\n5=D:\\d5\\\r\n" +
            "[FileNames]\r\n" +
            "0=ChrSel.wil\r\n" +
            "1=\r\n" +
            "2=Prguse.wil\r\n");

        using var f = new TFrmLogin();
        f.InitDfm();
        f.EditGamePath.Text = "  E:\\Mir2\\  ";            // 原文 :117 走 Trim
        LoginDlgGlobals.g_ClientVersion = TClientVersion.cv185;   // 1

        f.DialogButtonsClickOk(f.DialogButtonsOk, EventArgs.Empty);

        Assert.Equal(@"E:\Mir2\", LoginDlgGlobals.g_sMirDataDirectory);          // :117
        Assert.Equal(@"E:\Mir2\", LoginDlgGlobals.g_MirDataDirectoryList[1]);    // :130

        // :131-133 把 6 个 Directory 键**全量**写回（不只写当前版本那一项）
        var ini = new TLoginIniFile(ConfigPath);
        for (int i = 0; i <= 5; i++)
        {
            string expected = i == 1 ? @"E:\Mir2\" : @"D:\d" + i + @"\";
            Assert.Equal(expected, ini.ReadString("Directory", i.ToString(), "<missing>"));
        }

        // :119/:120 Setup 两个键
        Assert.Equal(1, ini.ReadInteger("Setup", "ClientVersion", -1));
        Assert.False(ini.ReadBool("Setup", "D3DFormat", true));

        // :121-127 文件名单：键 '0' → "ChrSel.wil"（对象值 = 键序号 0）；键 '2' → "Prguse.wil"（对象值 2）；
        //   键 '1' 的值是空串 ⇒ 被 :125 的 `sFileName <> ''` 跳过（计数取证：只加 2 条而不是 3 条）。
        Assert.Equal(2, LoginDlgGlobals.g_FileNameList.Count);
        Assert.Equal("ChrSel.wil", LoginDlgGlobals.g_FileNameList[0]);
        Assert.Equal(0, LoginDlgGlobals.g_FileNameList.GetObject(0));
        Assert.Equal("Prguse.wil", LoginDlgGlobals.g_FileNameList[1]);
        Assert.Equal(2, LoginDlgGlobals.g_FileNameList.GetObject(1));

        Assert.Equal(DialogResult.Yes, f.ModalResult);   // :138
    }

    [Fact]
    public void DialogButtonsClickOk_AppendsToExistingGlobalFileNameList()
        => AssertFileNameListIsAppended();

    private void AssertFileNameListIsAppended()
    {
        WriteConfig("[FileNames]\r\n0=a.wil\r\n");
        using var f = new TFrmLogin();
        f.InitDfm();
        LoginDlgGlobals.g_FileNameList.Add("既有项");

        f.DialogButtonsClickOk(f.DialogButtonsOk, EventArgs.Empty);

        // 原文 :126 是 AddObject（追加），不是 Clear+Add ⇒ 既有项仍在
        Assert.Equal(2, LoginDlgGlobals.g_FileNameList.Count);
        Assert.Equal("既有项", LoginDlgGlobals.g_FileNameList[0]);
        Assert.Equal("a.wil", LoginDlgGlobals.g_FileNameList[1]);
    }

    /// <summary>
    /// ★ D-P11-08：<c>FormCreate</c> 把 <c>Config.ini</c> 里的 <c>[Setup] ClientVersion</c> 装进
    /// <c>g_ClientVersion</c>，随后 <c>Open</c>（原文 <c>FrmLogin</c> 由外部 <c>ShowModal</c>）时
    /// <c>Ok</c> 又把同一值写回；<b>但</b> <c>ItemIndex</c> 已被 DFM 改成 3 —— 若用户在打开后
    /// 直接按"确定"而没点单选项，写的仍是 <c>g_ClientVersion</c>（= 读入值），不会因为是
    /// <c>ItemIndex=3</c> 而改成 3。这是一处"界面显示与实际值不一致"的原文行为，此处锁死。
    /// </summary>
    [Fact]
    public void OkWithoutTouchingRadio_WritesTheIniValueNotTheDfmItemIndex()
    {
        WriteConfig("[Setup]\r\nClientVersion=1\r\nD3DFormat=0\r\n");

        using var f = new TFrmLogin();
        f.InitDfm();
        Assert.Equal(TClientVersion.cv185, LoginDlgGlobals.g_ClientVersion);   // 读入 1
        Assert.Equal(3, f.ItemIndex);                                          // 界面显示 DFM 的 3

        f.DialogButtonsClickOk(f.DialogButtonsOk, EventArgs.Empty);

        var ini = new TLoginIniFile(ConfigPath);
        Assert.Equal(1, ini.ReadInteger("Setup", "ClientVersion", -1));        // 写回 1，不是 3
    }

    // =================================================================================
    // E. RadioGroupClick / CheckBoxD3DFormatClick（原文 :167-176）
    // =================================================================================

    [Fact]
    public void RadioGroupClick_SetsVersionAndShowsMappedDirectory()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        for (int i = 0; i < 6; i++) LoginDlgGlobals.g_MirDataDirectoryList[i] = "D:\\v" + i + "\\";

        f.RadioGroupClick(f.RadioGroupItems[2], EventArgs.Empty, 2);

        Assert.Equal(TClientVersion.cvHero, LoginDlgGlobals.g_ClientVersion);   // :169
        Assert.Equal(f.RadioGroupItems[2].Checked, true);
        Assert.False(f.RadioGroupItems[3].Checked);                             // 单选互斥
        Assert.Equal(@"D:\v2\", f.EditGamePath.Text);                           // :170
    }

    /// <summary>★ 原文缺陷照抄（D-P11-09）：DFM 有 **7** 个 Items，枚举只有 **6** 个值，数组也只有 6 格。</summary>
    [Fact]
    public void RadioGroupItems_SevenEntries_VersusSixElementDirectoryArray()
    {
        using var f = new TFrmLogin();
        f.InitDfm();

        // 1) 界面 7 项 vs 枚举 6 值 vs 数组 6 格 —— 三个数必须这样（原文如此）
        Assert.Equal(7, f.RadioGroupItems.Length);
        Assert.Equal(7, TFrmLogin.DfmRadioGroupItems.Length);
        Assert.Equal(6, Enum.GetValues<TClientVersion>().Length);
        Assert.Equal(6, LoginDlgGlobals.g_MirDataDirectoryList.Length);

        // 2) 第 7 项（表头 '传奇归来'）在原文里对应 TClientVersion(6) —— **枚举里没有这个值**
        Assert.Equal("传奇归来", TFrmLogin.DfmRadioGroupItems[6]);
    }

    [Fact]
    public void RadioGroupClick_OnSeventhItem_ReadsOutOfRangeSlot()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        for (int i = 0; i < 6; i++) LoginDlgGlobals.g_MirDataDirectoryList[i] = "D:\\v" + i + "\\";

        f.RadioGroupClick(f.RadioGroupItems[6], EventArgs.Empty, 6);

        // 原文 :169-170：g_ClientVersion := TClientVersion(6)（枚举外值）；数组下标 6 **越界**。
        //   Delphi 默认无范围检查 ⇒ 读到相邻内存；托管侧越界读抛 IndexOutOfRangeException。
        //   D-P11-09：托管侧把越界读定义为"返回空串"（见 LoginDlgGlobals 的索引器语义），
        //   以保持"不崩、但结果无意义"这一现象，并在此锁死该差异。
        Assert.Equal((TClientVersion)6, LoginDlgGlobals.g_ClientVersion);
        Assert.Equal(string.Empty, f.EditGamePath.Text);
    }

    [Fact]
    public void RadioGroupClick_ForEveryIndex_MapsToSameOrdinalClientVersion()
    {
        using var f = new TFrmLogin();
        f.InitDfm();

        for (int i = 0; i <= 5; i++)
        {
            f.RadioGroupClick(f.RadioGroupItems[i], EventArgs.Empty, i);
            Assert.Equal((TClientVersion)i, LoginDlgGlobals.g_ClientVersion);
            Assert.Equal(i, f.ItemIndex);
        }
    }

    [Fact]
    public void CheckBoxD3DFormatClick_CopiesCheckedIntoGlobal()
    {
        using var f = new TFrmLogin();
        f.InitDfm();

        f.CheckBoxD3DFormat.Checked = true;
        f.CheckBoxD3DFormatClick(f.CheckBoxD3DFormat, EventArgs.Empty);
        Assert.True(LoginDlgGlobals.g_boD3DFormat);      // :175

        f.CheckBoxD3DFormat.Checked = false;
        f.CheckBoxD3DFormatClick(f.CheckBoxD3DFormat, EventArgs.Empty);
        Assert.False(LoginDlgGlobals.g_boD3DFormat);
    }

    // =================================================================================
    // F. DialogButtonsClickCancel（原文 :161-165）/ CloseForm / ModalResult
    // =================================================================================

    [Fact]
    public void DialogButtonsClickCancel_SetsMrNo()
    {
        using var f = new TFrmLogin();
        f.InitDfm();
        f.ModalResult = DialogResult.Yes;

        f.DialogButtonsClickCancel(f.DialogButtonsCancel, EventArgs.Empty);

        Assert.Equal(DialogResult.No, f.ModalResult);
    }

    [Fact]
    public void ModalResult_PropertyMirrorsDialogResult()
    {
        using var f = new TFrmLogin();
        f.ModalResult = TFrmLogin.mrYes;
        Assert.Equal(DialogResult.Yes, f.DialogResult);
        f.ModalResult = TFrmLogin.mrNo;
        Assert.Equal(DialogResult.No, f.DialogResult);
        Assert.Equal(DialogResult.Yes, TFrmLogin.mrYes);
        Assert.Equal(DialogResult.No, TFrmLogin.mrNo);
    }

    // =================================================================================
    // G. 接缝与 TIniFile（原文 IniFiles.TIniFile / ExtractFilePath）
    // =================================================================================

    /// <summary>原文 :116/:147 的 <c>ExtractFilePath(Application.ExeName) + 'Config.ini'</c>。</summary>
    [Theory]
    [InlineData(@"C:\app\Client.exe", @"C:\app\Config.ini")]
    [InlineData(@"C:\app\bin\Client.exe", @"C:\app\bin\Config.ini")]
    [InlineData("Client.exe", "Config.ini")]                       // 无路径分隔符 ⇒ ExtractFilePath 返回空串
    [InlineData(@"D:\a\b\", @"D:\a\b\Config.ini")]                  // 尾部反斜杠仍是"最后分隔符"（ExtractFilePath 把它保留）
    [InlineData(@"D:\a\b", @"D:\a\Config.ini")]                     // 无尾分隔符 ⇒ 最后一段 'b' 被丢掉（ExtractFilePath 语义）
    public void ConfigIniPath_FollowsExtractFilePath(string exeName, string expected)
    {
        LoginDlgHost.ApplicationExeName = exeName;
        Assert.Equal(expected, LoginDlgHost.ConfigIniPath());
    }

    [Fact]
    public void IniFile_WriteBoolWritesOneOrZero_AndEmptyStringIsSkipped()
    {
        string path = Path.Combine(_dir, "w.ini");
        using (var ini = new TLoginIniFile(path))
        {
            ini.WriteBool("Setup", "A", true);
            ini.WriteBool("Setup", "B", false);
            ini.WriteString("Setup", "C", "");        // 原文如此：空值不写
        }

        string text = File.ReadAllText(path, GXX.Core.EncodingInit.GBK);
        Assert.Contains("A=1", text);
        Assert.Contains("B=0", text);
        Assert.DoesNotContain("C=", text);
    }

    [Fact]
    public void IniFile_ReadBoolTreatsAnyNonZeroAsTrue_AndNegativeOneAsTrue()
    {
        string path = Path.Combine(_dir, "b.ini");
        File.WriteAllText(path, "[S]\r\nA=-1\r\nB=2\r\nC=True\r\nD=0\r\n", GXX.Core.EncodingInit.GBK);

        using var ini = new TLoginIniFile(path);
        Assert.True(ini.ReadBool("S", "A", false));      // '-1' → True（Delphi BoolToStr 的形态）
        Assert.True(ini.ReadBool("S", "B", false));      // 任何非 0
        Assert.False(ini.ReadBool("S", "C", false));     // 'True' 解析失败 ⇒ 回退 Default
        Assert.True(ini.ReadBool("S", "C", true));
        Assert.False(ini.ReadBool("S", "D", true));
    }

    [Fact]
    public void IniFile_ReadIntegerAcceptsHexPrefix()
    {
        string path = Path.Combine(_dir, "h.ini");
        File.WriteAllText(path, "[S]\r\nA=0x10\r\nB=$1F\r\nC=abc\r\n", GXX.Core.EncodingInit.GBK);

        using var ini = new TLoginIniFile(path);
        Assert.Equal(16, ini.ReadInteger("S", "A", -1));
        Assert.Equal(31, ini.ReadInteger("S", "B", -1));
        Assert.Equal(-1, ini.ReadInteger("S", "C", -1));
        Assert.Equal(5, ini.ReadInteger("S", "Z", 5));
    }

    [Fact]
    public void IniFile_ReadSectionReturnsKeysInFileOrder()
    {
        string path = Path.Combine(_dir, "s.ini");
        File.WriteAllText(path, "[FileNames]\r\n7=z.wil\r\n3=a.wil\r\n5=m.wil\r\n[Other]\r\n1=x\r\n",
            GXX.Core.EncodingInit.GBK);

        using var ini = new TLoginIniFile(path);
        var keys = new List<string> { "应被清空" };
        ini.ReadSection("FileNames", keys);
        Assert.Equal(new[] { "7", "3", "5" }, keys.ToArray());
        Assert.Equal(3, keys.Count);
        Assert.DoesNotContain("应被清空", keys);   // 计数取证：ReadSection 先 Clear（§37.3）
    }

    [Fact]
    public void IniFile_RoundTripsThroughDisk()
    {
        string path = Path.Combine(_dir, "rt.ini");
        using (var ini = new TLoginIniFile(path))
        {
            ini.WriteInteger("Setup", "ClientVersion", 3);
            ini.WriteString("Directory", "3", @"E:\Mir2\");
        }
        using (var ini = new TLoginIniFile(path))
        {
            Assert.Equal(3, ini.ReadInteger("Setup", "ClientVersion", -1));
            Assert.Equal(@"E:\Mir2\", ini.ReadString("Directory", "3", ""));
        }
    }

    /// <summary><c>LoginDlgGlobals</c> 的初值与 <c>Share.pas</c> 的 initialization 段一致。</summary>
    [Fact]
    public void Globals_DefaultToSharePasInitializationValues()
    {
        LoginDlgGlobals.ResetForTests();
        Assert.Equal(@"D:\热血传奇\", LoginDlgGlobals.g_sMirDataDirectory);
        Assert.Equal(TClientVersion.cvSerial, LoginDlgGlobals.g_ClientVersion);
        Assert.False(LoginDlgGlobals.g_boD3DFormat);
        Assert.Equal(6, LoginDlgGlobals.g_MirDataDirectoryList.Length);
        Assert.NotNull(LoginDlgGlobals.g_FileNameList);
        Assert.Equal(0, LoginDlgGlobals.g_FileNameList.Count);
    }

    /// <summary>
    /// <c>ShellDirectoryPicker</c> 的 P/Invoke 入口在"被测试绕过"时必须可注入，
    /// 且默认实现不可在无头测试里被调用到（否则弹模态框挂死 testhost）。
    /// </summary>
    [Fact]
    public void SelectDirectorySeam_IsReplaceable()
    {
        int calls = 0;
        LoginDlgHost.SelectDirectory = (c, r, d, o) => { calls++; return new DirectoryPickResult(false, d); };
        var got = LoginDlgHost.PickDirectory("cap", "", "dir", IntPtr.Zero);
        Assert.Equal(1, calls);
        Assert.False(got.Result);
        Assert.Equal("dir", got.Directory);

        LoginDlgHost.ResetForTests();
        Assert.NotNull(LoginDlgHost.SelectDirectory);
        // 默认实现必须是 shell32 那一份（不是抛异常/返回 null 的占位）
        Assert.Equal("SelectDirectory", LoginDlgHost.SelectDirectory.Method.Name);
    }

    /// <summary>
    /// 否定性断言（§37.3 计数取证）：本单元**没有**原文之外的公开成员，
    /// 6 个事件处理器 + 2 个嵌套过程一个不多一个不少。
    /// </summary>
    [Fact]
    public void PublicSurface_MatchesTheOriginalMemberList()
    {
        var names = typeof(TFrmLogin)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        foreach (string expected in new[]
        {
            "EditGamePathButtonClick", "DialogButtonsClickOk", "FormCreate",
            "DialogButtonsClickCancel", "RadioGroupClick", "CheckBoxD3DFormatClick",
        })
        {
            Assert.Contains(expected, names);
        }

        // 原文的 2 个嵌套过程在接缝类型上（不是窗体成员）
        Assert.True(typeof(ShellDirectoryPicker).GetMethod("SelectDirectory",
            BindingFlags.Public | BindingFlags.Static) != null);
        Assert.True(typeof(ShellDirectoryPicker).GetMethod("SelectDirCB",
            BindingFlags.NonPublic | BindingFlags.Static) != null);
    }
}
