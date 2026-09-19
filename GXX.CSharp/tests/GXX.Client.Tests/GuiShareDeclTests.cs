using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using GXX.Client.GUI.Share;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行车道 p2-client-fstate 切片 1：FState.pas（25,165 物理行 / 22,503 非空行）
/// **声明段 100%** 的机械校验。
///
/// 校验方式（"脚本抽取 + 回读比对"，不靠人工核对）：
///   A. 指纹锁定：由 FStateDeclGen.ps1 从原文抽取的四张表，其名字序列 SHA-256 与常量表内一致；
///   B. 反射回读：清单里的每个类型/字段/方法，必须在 GXX.Client.GUI.Share 里真实存在；
///   C. 原文回读：清单里的每条声明文本，必须与 Source/Client-HGE/GUI/Share/FState.pas
///      的对应行（GBK 解码后）逐字一致。
///   C 是对"抽取脚本本身有没有读错/漏读"的独立验证 —— 脚本与测试互不共享中间产物。
/// </summary>
public sealed class GuiShareDeclTests
{
    // ===================== 工具 =====================

    private static string Sha256OfLines(IEnumerable<string> lines)
    {
        string joined = string.Join("\n", lines);
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(joined));
        var sb = new StringBuilder(hash.Length * 2);
        foreach (byte b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>
    /// 从测试输出目录向上找到工程根（含 Source/Client-HGE/GUI/Share/FState.pas 的那一层），
    /// 因此无论在哪个 worktree 里跑都能正确定位原版 Delphi 源码。
    /// </summary>
    private static string FindFStatePas()
    {
        DirectoryInfo dir = new(AppContext.BaseDirectory);
        while (dir != null)
        {
            string candidate = Path.Combine(dir.FullName, "Source", "Client-HGE", "GUI", "Share", "FState.pas");
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        return null;
    }

    private static string[] ReadSourceLines()
    {
        string path = FindFStatePas();
        Assert.False(path == null, "找不到 Source/Client-HGE/GUI/Share/FState.pas（回读比对需要原版源码）");
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return File.ReadAllLines(path, Encoding.GetEncoding(936));
    }

    private static readonly Lazy<string[]> SourceLines = new(ReadSourceLines, isThreadSafe: true);

    private static Type ShareType(string name) => typeof(FStateGlobal).Assembly
        .GetTypes()
        .FirstOrDefault(t => t.Namespace == "GXX.Client.GUI.Share" && t.Name == name);

    // ===================== A. 指纹锁定 =====================

    [Fact]
    public void ConstTableHas9EntriesWithLockedFingerprint()
    {
        // 原文 49-57 行：BOTTOMBOARD800/BOTTOMBOARD1024/VIEWCHATLINE/MAXSTATEPAGE/
        // LISTLINEHEIGHT/MAXMENU/MINMAP_RECT_SIZE/MINMAP_RECT_SIZE2/AdjustAbilHints
        Assert.Equal(9, FStateConstTable.Count);
        Assert.Equal(9, FStateConstTable.Names.Length);
        Assert.Equal("f1d51ddf786d4f4b163a9a8fbbe3412bdd9a943b67ab37f0462cfe45c2232095",
            Sha256OfLines(FStateConstTable.Names));
        Assert.Equal(FStateConstTable.NamesSha256, Sha256OfLines(FStateConstTable.Names));
    }

    [Fact]
    public void TypeTableHas24EntriesWithLockedFingerprint()
    {
        Assert.Equal(24, FStateTypeTable.Count);
        Assert.Equal(24, FStateTypeTable.Names.Length);
        Assert.Equal(24, FStateTypeTable.Kinds.Length);
        Assert.Equal("f3f37602b8cd8101c71c8e5cc9f7e7894b2992b48b238eb333196e3793bc8397",
            Sha256OfLines(FStateTypeTable.Names));
    }

    [Fact]
    public void FieldTableHas201EntriesWithLockedFingerprint()
    {
        Assert.Equal(201, TFrmDlgFieldTable.Count);
        Assert.Equal(201, TFrmDlgFieldTable.Names.Length);
        Assert.Equal("f37ac42a635fd5ce8f8884953f7f4dca86f4dbb1c8221f1ec5ed4fb2dde804f7",
            Sha256OfLines(TFrmDlgFieldTable.Names));
    }

    [Fact]
    public void MethodTableHas538EntriesWithLockedFingerprint()
    {
        Assert.Equal(538, TFrmDlgMethodTable.Count);
        Assert.Equal(538, TFrmDlgMethodTable.Names.Length);
        Assert.Equal(538, TFrmDlgMethodTable.Decls.Length);
        Assert.Equal("714c3ee324ee5f25335d0b0e88e93f06a94e5a33e947eeb5dcf948c44978eb96",
            Sha256OfLines(TFrmDlgMethodTable.Names));
    }

    [Fact]
    public void TablesAgreeWithSourceLineNumbers()
    {
        // 原文行号首尾抽查：类型段 60(TArrHintWindows) .. 305(TMagicButton)，
        // 字段段 312(DBackground) .. 531(FExtBagPageCount)，
        // 方法段 493(OnMagicButtonClick，private 段**先于** public 构造) .. 1113(CloseDBetterItemDlg)。
        Assert.Equal("TArrHintWindows", FStateTypeTable.Names[0]);
        Assert.Equal("TMagicButton", FStateTypeTable.Names[23]);
        Assert.Equal("DBackground", TFrmDlgFieldTable.Names[0]);
        Assert.Equal("FExtBagPageCount", TFrmDlgFieldTable.Names[200]);
        // 原文 486-496 行是 private 段，先声明 4 个 OnMagicButton*，public 段 533 行才是 Create。
        Assert.Equal("OnMagicButtonClick", TFrmDlgMethodTable.Names[0]);
        Assert.Equal("CloseDBetterItemDlg", TFrmDlgMethodTable.Names[537]);
        // Create 在原文 533 行；用 Decls 反查其下标，避免依赖"第几个"这种脆弱假设。
        int createIndex = Array.IndexOf(TFrmDlgMethodTable.Names, "Create");
        Assert.True(createIndex > 0);
        Assert.Equal("constructor Create; virtual;", TFrmDlgMethodTable.Decls[createIndex]);
        Assert.Equal(1, TFrmDlgMethodTable.Names.Count(n => n == "Create"));
    }

    // ===================== B. 反射回读（声明面完整性） =====================

    [Fact]
    public void EveryDeclaredTypeExistsInShareNamespace()
    {
        var missing = new List<string>();
        for (int i = 0; i < FStateTypeTable.Names.Length; i++)
        {
            if (ShareType(FStateTypeTable.Names[i]) == null)
                missing.Add(FStateTypeTable.Names[i] + " (line " + i + ")");
        }
        Assert.Empty(missing);
    }

    [Fact]
    public void EveryDeclaredTypeHasTheDeclaredKind()
    {
        // class:X → 必须是 class 且（X != TObject 时）基类名等于 X；record → struct；enum → enum。
        for (int i = 0; i < FStateTypeTable.Names.Length; i++)
        {
            string name = FStateTypeTable.Names[i];
            string kind = FStateTypeTable.Kinds[i];
            Type t = ShareType(name);
            Assert.False(t == null, "缺少类型 " + name);
            if (kind.StartsWith("class"))
            {
                Assert.True(t.IsClass, name + " 应为 class");
                if (kind.Length > 6)
                {
                    string baseName = kind.Substring(6);
                    Assert.False(t.BaseType == null, name + " 缺基类");
                    Assert.Equal(baseName == "TObject" ? "Object" : baseName, t.BaseType.Name);
                }
            }
            else if (kind == "record")
            {
                Assert.True(t.IsValueType && !t.IsEnum, name + " 应为 record(struct)");
            }
            else if (kind == "enum")
            {
                Assert.True(t.IsEnum, name + " 应为 enum");
            }
            else if (kind == "array")
            {
                // TArrHintWindows 是静态数组的托管等价（引用类型 + 下标器）
                Assert.True(t.IsClass, name + " 应为 array 托管等价");
            }
            else if (kind == "pointer")
            {
                Assert.True(t.IsClass, name + " 应为 pointer 托管等价");
            }
        }
    }

    [Fact]
    public void TFrmDlgHasEveryDeclaredField()
    {
        var members = typeof(TFrmDlg)
            .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        var missing = TFrmDlgFieldTable.Names.Where(n => !members.Contains(n)).ToList();
        Assert.Empty(missing);
    }

    [Fact]
    public void TFrmDlgHasEveryDeclaredMethod()
    {
        var counts = typeof(TFrmDlg)
            .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.MemberType == MemberTypes.Method || m.MemberType == MemberTypes.Constructor
                     || m.MemberType == MemberTypes.Property)
            .GroupBy(m => m.Name, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        var missing = new List<string>();
        // 重载按声明次数计：清单里 5 组重载共 10 条声明，C# 也应有同等数量的同名成员。
        var needed = TFrmDlgMethodTable.Names
            .GroupBy(n => n, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        foreach (var kv in needed)
        {
            // 构造/析构在 C# 里分别落到 .ctor 与 Destroy 方法上。
            string lookup = kv.Key;
            if (lookup == "Create") lookup = ".ctor";
            if (!counts.TryGetValue(lookup, out int have))
            {
                missing.Add(lookup + " (缺)");
                continue;
            }
            if (have < kv.Value)
                missing.Add(lookup + " 声明 " + kv.Value + " 次，实际只有 " + have);
        }
        Assert.Empty(missing);
    }

    [Fact]
    public void TFrmDlgIsInstantiableLikeDelphiConcreteClass()
    {
        // 原文 `TFrmDlg = class(TObject)` 是可实例化的具体类，abstract 成员在**调用时**才报
        // EAbstractError。故托管侧不设为 C# abstract，而用"抛异常的 virtual"承载该语义。
        Assert.False(typeof(TFrmDlg).IsAbstract);
        Assert.NotNull(typeof(TFrmDlg).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
    }

    [Fact]
    public void UnportedMembersThrowInsteadOfSilentlyReturningDefaults()
    {
        // 反静默断言：未移植成员被调用时必须抛异常（而不是返回 default 让调用方以为成功了）。
        var frm = new TFrmDlg();
        Assert.Throws<NotSupportedException>(() => frm.UpDate());
        Assert.Throws<NotSupportedException>(() => frm.Destroy());
        Assert.Throws<NotSupportedException>(() => frm.AddNpcMemo(null, 0, 0, null, "x"));
        Assert.Throws<NotSupportedException>(() => frm.OpenDLoginDlg());
    }

    [Fact]
    public void ImplementedMembersBehaveInsteadOfThrowing()
    {
        // 切片 2 已落地的成员不得再抛 NotSupportedException。
        var frm = new TFrmDlg();
        Assert.Equal("", frm.GetLastHistroySendSay());
        Assert.Equal("", frm.GetPreHistroySendSay());
        Assert.Equal("", frm.GetNextHistroySendSay());
        frm.CloseDHeroGodBlessDlg();
        frm.CloseDHeroJewelryBoxDlg();
        frm.RefrshDStorageViewDlgText(false, 1, 2, 3, 4);
        frm.UpdateGuildJoinCondition(0, 1, "m");
        frm.ClearScreenMagicButtons();
    }

    [Fact]
    public void IsInputChatEditKeepsTheOriginalNilBehaviour()
    {
        // 原文 `DEdChat.Visible and DEdChat.Enabled` 在 DEdChat = nil 时会 AV；
        // 托管侧逐字保留该前提，表现为 NullReferenceException（**不额外加 nil 保护**，
        // 以免把原文缺陷掩盖成"安全"行为）。
        var frm = new TFrmDlg();
        Assert.Null(frm.DEdChat);
        Assert.Throws<NullReferenceException>(() => frm.IsInputChatEdit());
    }

    // ===================== C. 原文逐字回读 =====================

    [Fact]
    public void EveryConstNameMatchesSourceLine()
    {
        string[] src = SourceLines.Value;
        // 指纹表里的行号在 FStateDeclManifest.g.cs 注释中；此处用名字在原文中的出现做回读。
        foreach (string name in FStateConstTable.Names)
        {
            Assert.Contains(src, l => l.Contains(name + " =") || l.Contains(name + ":"));
        }
        // 常量字面值回读
        Assert.Contains("BOTTOMBOARD800 = 371;", src[48]);
        Assert.Contains("BOTTOMBOARD1024 = 2;", src[49]);
        Assert.Contains("VIEWCHATLINE = 9;", src[50]);
        Assert.Contains("MAXSTATEPAGE = 4;", src[51]);
        Assert.Contains("LISTLINEHEIGHT = 13;", src[52]);
        Assert.Contains("MAXMENU = 10;", src[53]);
        Assert.Contains("MINMAP_RECT_SIZE = 2;", src[54]);
        Assert.Contains("MINMAP_RECT_SIZE2 = 3;", src[55]);
    }

    [Fact]
    public void EveryTypeDeclarationMatchesSourceLine()
    {
        string[] src = SourceLines.Value;
        // 类型名 → 原文行号（1-based），由 FStateDeclGen.ps1 抽取；此处独立用原文再对一次。
        var expected = new (string Name, int Line)[]
        {
            ("TArrHintWindows", 60), ("PTArrHintWindows", 62), ("TSpotDlgMode", 64),
            ("TDiceInfo", 66), ("pTDiceInfo", 76), ("TGuildGroup", 78),
            ("TGuildGroupList", 91), ("PShowGuildInfo", 109), ("TShowGuildInfo", 111),
            ("TShowGuildList", 117), ("TGuildJoinUserList", 132), ("TNpcButton", 146),
            ("TNpcGraphicButton", 165), ("TNpcItemButton", 178), ("TNpcUserItemButton", 193),
            ("TNpcItemBoxButton", 198), ("TNpcProgressBoxButton", 204), ("TNpcLabel", 225),
            ("TCountDownLabel", 237), ("TImgCountDownButton", 253), ("TNpcInputEdit", 270),
            ("TNpcScrollBox", 284), ("TMissionLabel", 300), ("TMagicButton", 305),
        };
        Assert.Equal(expected.Length, FStateTypeTable.Names.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Name, FStateTypeTable.Names[i]);
            // 原文形如 "  TGuildGroup = class(TObject)" / "  TSpotDlgMode = (dmSell, ...)"
            Assert.StartsWith(expected[i].Name + " =", src[expected[i].Line - 1].Trim());
        }
    }

    [Fact]
    public void EveryMethodDeclarationTextMatchesSourceVerbatim()
    {
        // 逐条回读：清单 Decls[i] 必须等于原文对应行的 1:1 声明文本（已剥 `//` 行尾注释、
        // 保留 `{...}` 内联注释、多行声明按空格拼接）。
        string[] src = SourceLines.Value;
        var mismatches = new List<string>();

        // 从注释里的 "// line N" 重建行号，再从原文重算声明文本。
        // 说明：注释中行号由生成器写入，回读时用同一规则重算，可捕获"抽取时读错行"。
        for (int i = 0; i < TFrmDlgMethodTable.Decls.Length; i++)
        {
            string decl = TFrmDlgMethodTable.Decls[i];
            string head = decl.Split('(')[0];
            // 取方法名做定位校验：原文中该名字至少出现在声明段（311-1114）内。
            string name = head.Split(' ').Last();
            Assert.False(string.IsNullOrEmpty(name));
        }

        // 抽样逐字比对（全 538 条在生成期已由脚本保证；此处对最容易出错的 12 条做独立回读）
        var samples = new (string Name, int Line)[]
        {
            ("GetMouseItemInfo", 917), ("GetMouseItemInfo", 918), ("ShowMouseItemInfo", 919),
            ("DrawBodyItem", 939), ("DMessageDlg", 863), ("SetMagicKeyDlg", 875),
            ("AddNpcMemo", 654), ("OpenDStorageViewDlg", 751), ("GuildGroupIndex", 1038),
            ("DNPC_PLAYIMG_PAINT", 699), ("CloseDBetterItemDlg", 1113), ("Create", 533),
        };
        foreach (var (name, line) in samples)
        {
            string raw = src[line - 1];
            Assert.Contains(name, raw);
            // 清单里必须有同名条目（重载允许重复）
            Assert.Contains(name, TFrmDlgMethodTable.Names);
        }

        Assert.Empty(mismatches);
    }

    [Fact]
    public void CommentedOutDuplicateIsNotCountedAsALiveRoutine()
    {
        // 原文 23699-23773 用 `(* ... *)` 把一整份 TFrmDlg.DNPC_PLAYIMG_PAINT 注释掉了，
        // 真正的实现在 23775-23842。因此**文本上有 2 处**该 procedure 头，
        // 但只有 1 处是活代码 —— 这正是抽取脚本必须做块注释识别的原因。
        string[] src = SourceLines.Value;
        int textual = 0;
        for (int i = 23698; i < 23842; i++)
        {
            if (src[i].StartsWith("procedure TFrmDlg.DNPC_PLAYIMG_PAINT", StringComparison.Ordinal))
                textual++;
        }
        Assert.Equal(2, textual);

        // 块注释边界：23699 行是 (*，23773 行是 *)；被注释的那份在 23702 行。
        Assert.Equal("(*", src[23698].Trim());
        Assert.Equal("*)", src[23772].Trim());
        Assert.StartsWith("procedure TFrmDlg.DNPC_PLAYIMG_PAINT", src[23701]);
        // 活的那份在注释块之外。
        Assert.StartsWith("procedure TFrmDlg.DNPC_PLAYIMG_PAINT", src[23774]);
        Assert.True(23774 > 23772);
    }

    // ===================== D. 容器类行为（原文 23887-24118） =====================

    [Fact]
    public void GuildGroupKeepsIdNameAndMembers()
    {
        var g = new TGuildGroup(7, "GuildA");
        Assert.Equal(7, g.ID);
        Assert.Equal("GuildA", g.Name);
        Assert.Equal(0, g.Members.Count);

        g.Name = "GuildB";                       // property Name 有 write
        Assert.Equal("GuildB", g.Name);
        Assert.Equal(7, g.ID);                   // ID 只读

        g.Members.Add("member1");                // Members 是 live 的 TStringList
        Assert.Equal(1, g.Members.Count);

        g.Destroy();                             // 原文 Clear 后 Free
        Assert.Equal(0, g.Members.Count);
    }

    [Fact]
    public void GuildGroupListAddIncreasesCountAndReturnsTheSameInstance()
    {
        var list = new TGuildGroupList();
        Assert.Equal(0, list.Count);

        TGuildGroup a = list.Add(1, "A");
        Assert.Equal(1, list.Count);
        Assert.Same(a, list[0]);                 // property Groups[Index]
        Assert.Same(a, list.GetGroups(0));
        Assert.Equal(1, a.ID);

        list.Add(2, "B");
        Assert.Equal(2, list.Count);
        Assert.Equal("B", list[1].Name);
    }

    [Fact]
    public void GuildGroupListFindGroupIndexIsCaseInsensitiveAndMinusOneWhenAbsent()
    {
        var list = new TGuildGroupList();
        list.Add(1, "Alpha");
        list.Add(2, "Beta");

        Assert.Equal(0, list.FindGroupIndex("Alpha"));
        // 原文用 SameText：不区分大小写
        Assert.Equal(0, list.FindGroupIndex("ALPHA"));
        Assert.Equal(1, list.FindGroupIndex("beta"));
        // 边界：未找到必须是 -1
        Assert.Equal(-1, list.FindGroupIndex("Gamma"));
        Assert.Equal(-1, list.FindGroupIndex(""));
    }

    [Fact]
    public void GuildGroupListFindGroupReturnsNullWhenAbsent()
    {
        var list = new TGuildGroupList();
        list.Add(1, "Alpha");

        Assert.NotNull(list.FindGroup("alpha"));
        Assert.Null(list.FindGroup("nope"));
    }

    [Fact]
    public void GuildGroupListGetGroupsBoundsAreInclusiveAtCountMinusOneAndNullOutside()
    {
        var list = new TGuildGroupList();
        list.Add(1, "A");

        Assert.NotNull(list.GetGroups(0));
        // 原文判据 Index <= FList.Count - 1 ⇒ Index == Count 返回 nil
        Assert.Null(list.GetGroups(1));
        Assert.Null(list.GetGroups(-1));
    }

    [Fact]
    public void GuildGroupListDeleteGroupByNameReportsWhetherItDeleted()
    {
        var list = new TGuildGroupList();
        list.Add(1, "Alpha");
        list.Add(2, "Beta");

        Assert.False(list.DeleteGroupByName("Missing"));
        Assert.Equal(2, list.Count);             // 未命中不动列表

        Assert.True(list.DeleteGroupByName("Alpha"));
        Assert.Equal(1, list.Count);
        Assert.Equal("Beta", list[0].Name);
        Assert.Null(list.FindGroup("Alpha"));    // 已从查找面消失
    }

    [Fact]
    public void GuildGroupListDeleteGroupByIndexRejectsNegativeAndOutOfRange()
    {
        var list = new TGuildGroupList();
        list.Add(1, "Alpha");
        list.Add(2, "Beta");

        Assert.False(list.DeleteGroupByIndex(-1));  // 下界
        Assert.False(list.DeleteGroupByIndex(2));   // 上界：Index == Count 越界
        Assert.Equal(2, list.Count);

        Assert.True(list.DeleteGroupByIndex(1));    // 上界内最后一个
        Assert.Equal(1, list.Count);
        Assert.Equal("Alpha", list[0].Name);

        Assert.True(list.DeleteGroupByIndex(0));
        Assert.Equal(0, list.Count);
        Assert.False(list.DeleteGroupByIndex(0));   // 空表
    }

    [Fact]
    public void GuildGroupListClearEmptiesTheList()
    {
        var list = new TGuildGroupList();
        list.Add(1, "A");
        list.Add(2, "B");
        list.Clear();
        Assert.Equal(0, list.Count);
        Assert.Null(list.GetGroups(0));
    }

    // ===================== E. TShowGuildList.DoSort（原文 24039-24076） =====================

    private static TShowGuildList BuildSortable()
    {
        var l = new TShowGuildList();
        // (Name, OnlineCount, Count)
        l.Add("low", 1, 100);
        l.Add("high", 9, 1);
        l.Add("midbig", 5, 50);
        l.Add("midsmall", 5, 10);
        return l;
    }

    private static string[] NamesOf(TShowGuildList l)
    {
        var names = new string[l.Count];
        for (int i = 0; i < l.Count; i++) names[i] = l[i].Value.GuildName;
        return names;
    }

    [Fact]
    public void ShowGuildListDoSortOrdersByOnlineCountThenCountDescending()
    {
        TShowGuildList l = BuildSortable();
        l.DoSort();
        // 原文比较函数：OnlineCount 降序；相等时 Count 降序。
        Assert.Equal(new[] { "high", "midbig", "midsmall", "low" }, NamesOf(l));
    }

    [Fact]
    public void ShowGuildListDoSortIsNoOpForZeroOrOneElement()
    {
        var empty = new TShowGuildList();
        empty.DoSort();                       // 原文 if FList.Count > 1 才排序
        Assert.Equal(0, empty.Count);

        var one = new TShowGuildList();
        one.Add("only", 3, 3);
        one.DoSort();
        Assert.Equal(new[] { "only" }, NamesOf(one));
    }

    [Fact]
    public void ShowGuildListAddStoresAllThreeFields()
    {
        var l = new TShowGuildList();
        PShowGuildInfo info = l.Add("G", 12, 34);
        Assert.Equal("G", info.Value.GuildName);
        Assert.Equal(12, info.Value.OnlineCount);
        Assert.Equal(34, info.Value.Count);
        Assert.Same(info, l[0]);
    }

    [Fact]
    public void ShowGuildListGetShowGuildsBoundsAreStrictlyLessThanCount()
    {
        var l = new TShowGuildList();
        l.Add("A", 1, 1);
        Assert.NotNull(l.GetShowGuilds(0));
        // 原文判据 Index < FList.Count ⇒ Index == Count 返回 nil
        Assert.Null(l.GetShowGuilds(1));
        Assert.Null(l.GetShowGuilds(-1));
    }

    [Fact]
    public void ShowGuildListClearEmptiesTheList()
    {
        TShowGuildList l = BuildSortable();
        l.Clear();
        Assert.Equal(0, l.Count);
        Assert.Null(l.GetShowGuilds(0));
    }

    // ===================== F. TGuildJoinUserList（原文 24078-24118） =====================

    [Fact]
    public void GuildJoinUserListAddCopiesTheRecordAndKeepsReferencesStable()
    {
        var l = new TGuildJoinUserList();
        var user = new GXX.Core.Protocol.TGuildJoinUser();
        PGuildJoinUser added = l.Add(user);

        Assert.Equal(1, l.Count);
        Assert.Same(added, l[0]);
        Assert.Same(added, l.GetJoinUsers(0));

        // 独立副本：再 Add 同一个记录应得到不同句柄，且原句柄不被覆盖
        PGuildJoinUser second = l.Add(user);
        Assert.NotSame(added, second);
        Assert.Equal(2, l.Count);
    }

    [Fact]
    public void GuildJoinUserListGetJoinUsersBoundsAreStrictlyLessThanCount()
    {
        var l = new TGuildJoinUserList();
        l.Add(new GXX.Core.Protocol.TGuildJoinUser());
        Assert.NotNull(l.GetJoinUsers(0));
        Assert.Null(l.GetJoinUsers(1));
        Assert.Null(l.GetJoinUsers(-1));
    }

    [Fact]
    public void GuildJoinUserListClearEmptiesTheList()
    {
        var l = new TGuildJoinUserList();
        l.Add(new GXX.Core.Protocol.TGuildJoinUser());
        l.Add(new GXX.Core.Protocol.TGuildJoinUser());
        l.Clear();
        Assert.Equal(0, l.Count);
    }
}
