// 测试：Source\RunGate\GateShare.pas（实测 LF 3595）的**名单加载/落盘 + 日志 + 进程黑名单重建**族
//   → src/GXX.RunGate/GateShareLists.cs
//
// 覆盖策略：每个公开成员 ≥3 用例；原文缺陷写成差异断言并标注 `原文缺陷 X`（编号与 GateShareLists.cs 文件头一致）。
// 文件系统用例一律用 `Path.GetTempPath()` 下的独立临时目录，Dispose 时删除。
using System;
using System.IO;
using System.Linq;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

[Collection("RunGateFormLane")]      // 共享 FormGlobals / GateShareGlobals / GateSharePaths 静态量，必须串行
public sealed class GateShareListTests : IDisposable
{
    private readonly string _dir;

    public GateShareListTests()
    {
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        _dir = Path.Combine(Path.GetTempPath(), "p2rg-lists-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        GateSharePaths.SetExeDirForTest(_dir);
    }

    public void Dispose()
    {
        GateSharePaths.ResetForTest();
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { /* 清理失败不影响断言 */ }
    }

    private string P(string name) => Path.Combine(_dir, name);
    private static readonly System.Text.Encoding GBK = System.Text.Encoding.GetEncoding(936);
    private static void WriteGb(string path, string text) => File.WriteAllText(path, text, GBK);

    // ================= GateSharePaths =================

    [Fact]
    public void GateSharePaths_ExeDir带尾随分隔符()
    {
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), GateSharePaths.ExeDir);
        GateSharePaths.SetExeDirForTest(_dir);
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), GateSharePaths.ExeDir);
        Assert.Equal(Path.Combine(_dir, "RunGate.exe"), GateSharePaths.ParamStr0);
    }

    [Fact]
    public void GateSharePaths_重复设置尾随分隔符不累积()
    {
        GateSharePaths.SetExeDirForTest(_dir + Path.DirectorySeparatorChar);
        Assert.Equal(_dir + Path.DirectorySeparatorChar, GateSharePaths.ExeDir);
    }

    [Fact]
    public void GateSharePaths_Reset回到真实exe目录()
    {
        GateSharePaths.SetExeDirForTest(_dir);
        GateSharePaths.ResetForTest();
        Assert.NotEqual(_dir + Path.DirectorySeparatorChar, GateSharePaths.ExeDir);
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), GateSharePaths.ExeDir);
    }

    // ================= DefStrs（脚本抽取 + 回读比对） =================

    [Fact]
    public void DefStrs_Has65EntriesMatchingSourceExtraction()
    {
        // 抽取脚本（GBK 归一化后按物理 LF 行号）：
        //   $l=[IO.File]::ReadAllLines($norm); $b=$l[1482..1489] -join "`n"
        //   [regex]::Matches($b,"'([^']*)'") → 65 个
        Assert.Equal(65, GateShareLists.DefStrsForTest.Count);

        // 关键下标回读（含原文两处易错点：array[0..64] 恰好填满；'Z' 是全角）
        Assert.Equal("М", GateShareLists.DefStrsForTest[0]);      // 西里尔 М（U+041C），不是拉丁 M
        Assert.Equal("ω", GateShareLists.DefStrsForTest[1]);      // 希腊 ω
        Assert.Equal("С", GateShareLists.DefStrsForTest[2]);      // 西里尔 С（U+0421）
        Assert.Equal("Ａ", GateShareLists.DefStrsForTest[3]);
        Assert.Equal("Ｚ", GateShareLists.DefStrsForTest[62]);    // 全角 Z
        Assert.Equal("c", GateShareLists.DefStrsForTest[63]);     // 半角 c
        Assert.Equal("w", GateShareLists.DefStrsForTest[64]);     // 半角 w
    }

    [Fact]
    public void DefStrs_含7组重复项_去重后58个()
    {
        // 'Ａ'(3,9) 'щ'(7,8) 'Ｂ'(4,38) 'Ｃ'(5,39) 'Ｄ'(10,40) 'Ｅ'(11,41) 'Ｆ'(12,42)
        Assert.Equal(58, GateShareLists.DefStrsForTest.Distinct().Count());
    }

    [Fact]
    public void AddToDefFilterSayMsgList_去重后加入33条()
    {
        // ★ 精算（不要把 58 当成最终值）：
        //   * 65 条字面量里有 7 组**完全相同**的重复（'Ａ'×2、'щ'×2、'Ｂ'/'Ｃ'/'Ｄ'/'Ｅ'/'Ｆ' 各 ×2）
        //     → 精确去重后 58；
        //   * 但 `g_WordFilterList` 是 `TSafeHashStringList`（原文 = `THashedStringList`，
        //     `CaseSensitive` 默认 False）→ `IndexOf` **大小写不敏感** → 25 组**全角大写/小写**
        //     成对折叠（'Ｂ'(U+FF22) ≡ 'ｂ'(U+FF42) … 'Ｚ' ≡ 'ｚ'）→ 58 − 25 = 33。
        //   * 半角 'c'/'w' **不**与全角 'ｃ'/'ｗ' 折叠（ToUpperInvariant 分别给 U+0043 / U+FF23）
        //     → 那两条各自独立，所以是 33 而不是 31。
        //   注：Delphi 侧走 `AnsiCompareText`（GBK 码页 936 的 DBCS 大小写表，同样成对折叠全角字母），
        //   环境内无法实跑 Delphi 复核，标 UNVERIFIED；此处以托管侧可执行语义为准。
        GateShareLists.AddToDefFilterSayMsgList();
        Assert.Equal(33, FormGlobals.g_WordFilterList.Count);

        // 幂等：再调一次不增加
        GateShareLists.AddToDefFilterSayMsgList();
        Assert.Equal(33, FormGlobals.g_WordFilterList.Count);
    }

    [Fact]
    public void AddToDefFilterSayMsgList_保留既有词条()
    {
        FormGlobals.g_WordFilterList.Add("已有词");
        GateShareLists.AddToDefFilterSayMsgList();
        Assert.Equal(34, FormGlobals.g_WordFilterList.Count);          // 33 + 1
        Assert.Equal(0, FormGlobals.g_WordFilterList.IndexOf("已有词"));
    }

    // ================= LoadFilterSayMsgFile（原 :1538-1553） =================

    [Fact]
    public void LoadFilterSayMsgFile_文件不存在时不改列表()
    {
        FormGlobals.g_sWordFilterFileName = P("no-such.txt");
        FormGlobals.g_WordFilterList.Add("keep");
        GateShareLists.LoadFilterSayMsgFile();
        Assert.Equal(1, FormGlobals.g_WordFilterList.Count);
        Assert.Equal(0, FormGlobals.g_WordFilterList.IndexOf("keep"));
    }

    [Fact]
    public void LoadFilterSayMsgFile_读取文件内容()
    {
        string f = P("WordFilter.txt");
        WriteGb(f, "脏话1\r\n脏话2\r\n");
        FormGlobals.g_sWordFilterFileName = f;
        GateShareLists.LoadFilterSayMsgFile();
        Assert.Equal(2, FormGlobals.g_WordFilterList.Count);
        Assert.Equal(0, FormGlobals.g_WordFilterList.IndexOf("脏话1"));
        Assert.Equal(1, FormGlobals.g_WordFilterList.IndexOf("脏话2"));
    }

    [Fact]
    public void LoadFilterSayMsgFile_DoesNotInjectDefaultWords_VersionTypeIs2()
    {
        // ★ 条件编译复核（GateShareLists.cs 文件头）：VERSION_TYPE = 2（Grobal2_Ex.pas:15）
        //   → 原 :1549-1551 的 `AddToDefFilterSayMsgList` 调用**不编译**。
        string f = P("WordFilter.txt");
        WriteGb(f, "only-one\r\n");
        FormGlobals.g_sWordFilterFileName = f;
        GateShareLists.LoadFilterSayMsgFile();

        Assert.Equal(1, FormGlobals.g_WordFilterList.Count);
        Assert.Equal(-1, FormGlobals.g_WordFilterList.IndexOf("Ａ"));   // 默认词表一条都没进来
        Assert.Equal(-1, FormGlobals.g_WordFilterList.IndexOf("М"));
    }

    [Fact]
    public void LoadFilterSayMsgFile_日志等级4被默认门限3滤掉_原文缺陷E()
    {
        // ★ 缺陷 E：原 :1552 用等级 4，而 g_btShowLogLevel 默认 3 → `4 <= 3` 为假
        FormGlobals.g_WordFilterList.Clear();
        GateShareLists.LoadFilterSayMsgFile();
        Assert.Equal(0, GateShareGlobals.g_MainLogStrings.Count);

        GateShareGlobals.g_btShowLogLevel = 4;                          // 抬到 4 就能进日志
        GateShareLists.LoadFilterSayMsgFile();
        Assert.Equal(1, GateShareGlobals.g_MainLogStrings.Count);
        Assert.Contains("加载文字过滤信息完成", GateShareGlobals.g_MainLogStrings[0]);
    }

    // ================= ReadFYDenyIPListFile / Pass（原 :1555-1607） =================

    [Fact]
    public void ReadFYDenyIPListFile_文件不存在返回False且保留旧名单()
    {
        FormGlobals.g_sFYReadDenyIPFile = P("no-ip.txt");
        GateShareGlobals.g_FYDenyIPList.Add("1.2.3.4");
        Assert.False(GateShareLists.ReadFYDenyIPListFile());
        Assert.Equal(1, GateShareGlobals.g_FYDenyIPList.Count);          // 原 :1564 Exit 在 Clear 之前
    }

    [Fact]
    public void ReadFYDenyIPListFile_过滤非IP并去空行()
    {
        string f = P("KickList.txt");
        WriteGb(f, "1.2.3.4\r\n\r\nnot-an-ip\r\n  5.6.7.8  \r\n1.2.3.4\r\n");
        FormGlobals.g_sFYReadDenyIPFile = f;
        GateShareGlobals.g_FYDenyIPList.Add("9.9.9.9");                 // 旧数据应先被 Clear

        Assert.True(GateShareLists.ReadFYDenyIPListFile());
        Assert.Equal(2, GateShareGlobals.g_FYDenyIPList.Count);          // 重复的 1.2.3.4 由 TAddressList 去重
        Assert.NotNull(GateShareGlobals.g_FYDenyIPList.Find("1.2.3.4"));
        Assert.NotNull(GateShareGlobals.g_FYDenyIPList.Find("5.6.7.8"));
        Assert.Null(GateShareGlobals.g_FYDenyIPList.Find("9.9.9.9"));
    }

    [Fact]
    public void ReadFYDenyIPListFile_空文件返回True且名单为空()
    {
        string f = P("empty-ip.txt");
        WriteGb(f, "");
        FormGlobals.g_sFYReadDenyIPFile = f;
        GateShareGlobals.g_FYDenyIPList.Add("1.1.1.1");
        Assert.True(GateShareLists.ReadFYDenyIPListFile());
        Assert.Equal(0, GateShareGlobals.g_FYDenyIPList.Count);
    }

    [Fact]
    public void ReadFYPassIPListFile_文件不存在返回False且保留旧名单()
    {
        FormGlobals.g_sFYReadPassIPFile = P("no-pass.txt");
        GateShareGlobals.g_FYPassIPList.Add("1.2.3.4");
        Assert.False(GateShareLists.ReadFYPassIPListFile());
        Assert.Equal(1, GateShareGlobals.g_FYPassIPList.Count);
    }

    [Fact]
    public void ReadFYPassIPListFile_读取并过滤()
    {
        string f = P("绿色通道.txt");
        WriteGb(f, "10.0.0.1\r\nbad\r\n10.0.0.2\r\n");
        FormGlobals.g_sFYReadPassIPFile = f;
        Assert.True(GateShareLists.ReadFYPassIPListFile());
        Assert.Equal(2, GateShareGlobals.g_FYPassIPList.Count);
    }

    // ================= ReadFYDenyMACListFile（原 :1609-1634） =================

    [Fact]
    public void ReadFYDenyMACListFile_文件不存在也清空_原文缺陷B()
    {
        // ★ 缺陷 B：原 :1617 的 Clear 在 :1620 的 FileExists 判据**之前**
        FormGlobals.g_sFYReadDenyMACFile = P("no-mac.txt");
        GateShareGlobals.g_FYDenyMACList.Add("AA-BB-CC");
        Assert.False(GateShareLists.ReadFYDenyMACListFile());
        Assert.Equal(0, GateShareGlobals.g_FYDenyMACList.Count);         // ← 与 IP 版行为不同
    }

    [Fact]
    public void ReadFYDenyMACListFile_不做IP格式过滤且空行也入表()
    {
        string f = P("DenyMachineIDList.txt");
        WriteGb(f, "AA-BB-CC-DD\r\n\r\nnot-a-mac\r\n");
        FormGlobals.g_sFYReadDenyMACFile = f;
        Assert.True(GateShareLists.ReadFYDenyMACListFile());
        // 3 行全部入表（Trim 后 Add，**无 IsIpaddr 过滤、无空行跳过**）
        Assert.Equal(3, GateShareGlobals.g_FYDenyMACList.Count);
        Assert.Equal(2, GateShareGlobals.g_FYDenyMACList.IndexOf("not-a-mac"));
        Assert.Equal(1, GateShareGlobals.g_FYDenyMACList.IndexOf(""));
    }

    [Fact]
    public void ReadFYDenyMACListFile_重复项由IndexOf语义保留两条()
    {
        // ★ 这里是 TStringList 的 Add（**不去重**），与 AddTempBlockMac 的显式去重不同
        string f = P("dup-mac.txt");
        WriteGb(f, "MAC1\r\nMAC1\r\n");
        FormGlobals.g_sFYReadDenyMACFile = f;
        GateShareLists.ReadFYDenyMACListFile();
        Assert.Equal(2, GateShareGlobals.g_FYDenyMACList.Count);
    }

    // ================= BlockIPList.txt（原 :1636-1671） =================

    [Fact]
    public void LoadBlockIPFile_文件不存在只记日志()
    {
        GateShareLists.LoadBlockIPFile();
        Assert.Equal(0, GateShareGlobals.g_BlockIPList.Count);
        Assert.Equal(0, GateShareGlobals.g_MainLogStrings.Count);        // 等级 4 被滤
    }

    [Fact]
    public void LoadBlockIPFile_读取且不去重不清空()
    {
        WriteGb(P("BlockIPList.txt"), "1.2.3.4\r\n1.2.3.4\r\nbad\r\n");
        GateShareGlobals.g_BlockIPList.Add("9.9.9.9");
        GateShareLists.LoadBlockIPFile();

        // 原文**不先 Clear**；1.2.3.4 由 TAddressList 去重；'bad' 因死守卫（缺陷 1）以 nIPaddr=-1 入库
        Assert.Equal(3, GateShareGlobals.g_BlockIPList.Count);
        Assert.NotNull(GateShareGlobals.g_BlockIPList.Find("9.9.9.9"));
        Assert.NotNull(GateShareGlobals.g_BlockIPList.Find("1.2.3.4"));
        Assert.Equal(-1, GateShareGlobals.g_BlockIPList.Find("bad").nIPaddr);
    }

    [Fact]
    public void SaveBlockIPList_用inet_ntoa还原字符串()
    {
        GateShareGlobals.g_BlockIPList.Add("202.103.100.1");
        GateShareGlobals.g_BlockIPList.Add("10.0.0.5");
        GateShareLists.SaveBlockIPList();

        string text = File.ReadAllText(P("BlockIPList.txt"), GBK);
        Assert.Equal("202.103.100.1\r\n10.0.0.5\r\n", text);
    }

    [Fact]
    public void SaveBlockIPList_非法项的负nIPaddr写成255_255_255_255()
    {
        // ★ 缺陷 1 的落盘后果：nIPaddr = -1 → inet_ntoa($FFFFFFFF) = "255.255.255.255"
        GateShareGlobals.g_BlockIPList.Add("garbage");
        GateShareLists.SaveBlockIPList();
        Assert.Equal("255.255.255.255\r\n", File.ReadAllText(P("BlockIPList.txt"), GBK));
    }

    [Fact]
    public void SaveBlockIPList_空名单写空文件()
    {
        GateShareLists.SaveBlockIPList();
        Assert.True(File.Exists(P("BlockIPList.txt")));
        Assert.Equal("", File.ReadAllText(P("BlockIPList.txt"), GBK));
    }

    [Fact]
    public void BlockIPList_LoadSave往返()
    {
        GateShareGlobals.g_BlockIPList.Add("1.1.1.1");
        GateShareGlobals.g_BlockIPList.Add("255.255.255.255");
        GateShareLists.SaveBlockIPList();

        GateShareGlobals.g_BlockIPList.Clear();
        GateShareLists.LoadBlockIPFile();
        Assert.Equal(2, GateShareGlobals.g_BlockIPList.Count);
        Assert.NotNull(GateShareGlobals.g_BlockIPList.Find("1.1.1.1"));
        Assert.NotNull(GateShareGlobals.g_BlockIPList.Find("255.255.255.255"));
    }

    // ================= BlockMacList.txt（原 :1673-1693） =================

    [Fact]
    public void LoadBlockMacFile_文件不存在只记日志()
    {
        GateShareLists.LoadBlockMacFile();
        Assert.Equal(0, GateShareGlobals.g_BlockMacList.Count);
    }

    [Fact]
    public void LoadBlockMacFile_LoadFromFile本身就会清空()
    {
        // ★ 更正：原文 `g_BlockMacList.LoadFromFile` 走 `TStrings.SetTextStr`，其第一件事就是 `Clear`
        //   → 既有内容**会**被替换（不是"追加"）。原车道文档把它写成"不清空"，此处按原文改正。
        WriteGb(P("BlockMacList.txt"), "MAC-A\r\nMAC-B\r\n");
        GateShareGlobals.g_BlockMacList.Add("OLD");
        GateShareLists.LoadBlockMacFile();
        Assert.Equal(2, GateShareGlobals.g_BlockMacList.Count);
        Assert.Equal(-1, GateShareGlobals.g_BlockMacList.IndexOf("OLD"));
        Assert.Equal(0, GateShareGlobals.g_BlockMacList.IndexOf("MAC-A"));
    }

    [Fact]
    public void SaveBlockMacList_落盘并往返()
    {
        GateShareGlobals.g_BlockMacList.Add("MAC-X");
        GateShareLists.SaveBlockMacList();
        Assert.Equal("MAC-X\r\n", File.ReadAllText(P("BlockMacList.txt"), GBK));

        GateShareGlobals.g_BlockMacList.Clear();
        GateShareLists.LoadBlockMacFile();
        Assert.Equal(1, GateShareGlobals.g_BlockMacList.Count);
    }

    // ================= IPSectionList.txt（原 :1695-1775） =================

    [Fact]
    public void LoadIPSectionList_文件不存在也先清空()
    {
        GateShareGlobals.AddIPSection(new TIPSection { nBeginAddr = 1, nEndAddr = 2 });
        GateShareLists.LoadIPSectionList();
        Assert.Equal(0, GateShareGlobals.IPSectionListCount);             // 原 :1704-1711 先清
    }

    [Fact]
    public void LoadIPSectionList_按TAB分隔并校验区间方向()
    {
        string tab = "\u0008";
        WriteGb(P("IPSectionList.txt"),
            "10.0.0.1" + tab + "10.0.0.255\r\n" +          // 合法
            "192.168.1.255" + tab + "192.168.1.1\r\n" +    // begin > end → 丢
            "\r\n" +                                        // 空行 → 丢
            tab + "10.0.0.5\r\n" +                          // TAB 在第 1 列（Pos <= 1）→ 丢
            "no-tab-line\r\n" +                             // 无 TAB → 丢
            "bad" + tab + "10.0.0.9\r\n" +                   // IP2Long 失败 = INADDR_NONE → 丢
            "172.16.0.1" + tab + "172.16.0.254\r\n");       // 合法

        GateShareLists.LoadIPSectionList();

        Assert.Equal(2, GateShareGlobals.IPSectionListCount);
        Assert.Equal(GateShareInet.IP2Long("10.0.0.1"), GateShareGlobals.GetIPSection(0).nBeginAddr);
        Assert.Equal(GateShareInet.IP2Long("10.0.0.255"), GateShareGlobals.GetIPSection(0).nEndAddr);
        Assert.Equal(GateShareInet.IP2Long("172.16.0.1"), GateShareGlobals.GetIPSection(1).nBeginAddr);
    }

    [Fact]
    public void SaveIPSectionList_用Long2IP与TAB拼接()
    {
        GateShareGlobals.AddIPSection(new TIPSection
        {
            nBeginAddr = GateShareInet.IP2Long("10.0.0.1"),
            nEndAddr = GateShareInet.IP2Long("10.0.0.255"),
        });
        GateShareLists.SaveIPSectionList();
        Assert.Equal("10.0.0.1\u000810.0.0.255\r\n", File.ReadAllText(P("IPSectionList.txt"), GBK));
    }

    [Fact]
    public void IPSectionList_LoadSave往返()
    {
        GateShareGlobals.AddIPSection(new TIPSection
        {
            nBeginAddr = GateShareInet.IP2Long("1.2.3.0"),
            nEndAddr = GateShareInet.IP2Long("1.2.3.255"),
        });
        GateShareLists.SaveIPSectionList();
        GateShareGlobals.ClearIPSectionList();
        GateShareLists.LoadIPSectionList();
        Assert.Equal(1, GateShareGlobals.IPSectionListCount);
        Assert.Equal(GateShareInet.IP2Long("1.2.3.0"), GateShareGlobals.GetIPSection(0).nBeginAddr);
    }

    [Fact]
    public void SaveIPSectionList_空列表写空文件()
    {
        GateShareLists.SaveIPSectionList();
        Assert.Equal("", File.ReadAllText(P("IPSectionList.txt"), GBK));
    }

    // ================= !addrtable.txt（原 :1777-1803） =================

    [Fact]
    public void LoadDBAddressTable_去重与跳注释空行()
    {
        WriteGb(P("!addrtable.txt"), "; comment\r\n\r\n127.0.0.1\r\n127.0.0.1\r\n10.0.0.1\r\n");
        GateShareGlobals.g_DBAddressList.Add("old");
        GateShareLists.LoadDBAddressTable();

        Assert.Equal(2, GateShareGlobals.g_DBAddressList.Count);
        Assert.Equal("127.0.0.1", GateShareGlobals.g_DBAddressList[0]);
        Assert.Equal("10.0.0.1", GateShareGlobals.g_DBAddressList[1]);
    }

    [Fact]
    public void LoadDBAddressTable_文件不存在时仍清空()
    {
        GateShareGlobals.g_DBAddressList.Add("old");
        GateShareLists.LoadDBAddressTable();
        Assert.Equal(0, GateShareGlobals.g_DBAddressList.Count);
    }

    [Fact]
    public void LoadDBAddressTable_只含注释的行被跳过()
    {
        WriteGb(P("!addrtable.txt"), ";a\r\n;b\r\n");
        GateShareLists.LoadDBAddressTable();
        Assert.Equal(0, GateShareGlobals.g_DBAddressList.Count);
    }

    // ================= LoadProcessBlacklist（原 :1866-1907） =================

    [Fact]
    public void LoadProcessBlacklist_清的是DB地址表而不是进程黑名单_原文缺陷A()
    {
        // ★ 缺陷 A：原 :1873 第一行是 `g_DBAddressList.Clear;`
        GateShareGlobals.g_DBAddressList.Add("db-entry");
        WriteGb(P("ProcessBlacklist.txt"), "cheat.exe|" + new string('A', 32) + "\r\n");
        GateShareLists.LoadProcessBlacklist();

        Assert.Equal(0, GateShareGlobals.g_DBAddressList.Count);          // DB 表被清
        Assert.Equal(1, FormGlobals.g_ProcessBlackList.Count);
    }

    [Fact]
    public void LoadProcessBlacklist_只收32位十六进制MD5()
    {
        WriteGb(P("ProcessBlacklist.txt"),
            "good.exe|" + new string('A', 32) + "\r\n" +       // 合法
            "short.exe|" + new string('B', 31) + "\r\n" +      // 长度不对
            "nonhex.exe|" + new string('Z', 32) + "\r\n" +     // 非 hex
            "nosep.exe\r\n" +                                   // 无 '|'
            "|" + new string('C', 32) + "\r\n" +               // 空进程名（Index=1>0 → 收）
            ";comment|" + new string('D', 32) + "\r\n" +       // 注释行
            "\r\n");
        GateShareLists.LoadProcessBlacklist();

        Assert.Equal(2, FormGlobals.g_ProcessBlackList.Count);
        Assert.NotNull(FormGlobals.g_ProcessBlackList.Find(new string('A', 32)));
        Assert.NotNull(FormGlobals.g_ProcessBlackList.Find(new string('C', 32)));
        Assert.Null(FormGlobals.g_ProcessBlackList.Find(new string('D', 32)));
    }

    [Fact]
    public void LoadProcessBlacklist_不清空旧项_原文缺陷F()
    {
        FormGlobals.g_ProcessBlackList.Add("old.exe", new string('E', 32));
        WriteGb(P("ProcessBlacklist.txt"), "new.exe|" + new string('A', 32) + "\r\n");
        GateShareLists.LoadProcessBlacklist();
        Assert.Equal(2, FormGlobals.g_ProcessBlackList.Count);            // 旧项仍在
    }

    [Fact]
    public void LoadProcessBlacklist_重复加载不产生重复项()
    {
        WriteGb(P("ProcessBlacklist.txt"), "x.exe|" + new string('a', 32) + "\r\n");
        GateShareLists.LoadProcessBlacklist();
        GateShareLists.LoadProcessBlacklist();
        Assert.Equal(1, FormGlobals.g_ProcessBlackList.Count);            // Add 的 SameText 去重
    }

    [Fact]
    public void LoadProcessBlacklist_文件不存在时不清DB表()
    {
        // 原 :1873 的 Clear 在 FileExists 判据之前 → 文件不存在也会清
        GateShareGlobals.g_DBAddressList.Add("db-entry");
        GateShareLists.LoadProcessBlacklist();
        Assert.Equal(0, GateShareGlobals.g_DBAddressList.Count);
        Assert.Equal(0, FormGlobals.g_ProcessBlackList.Count);
    }

    // ================= SaveProcessBlacklist（原 :1956-1979） =================

    [Fact]
    public void SaveProcessBlacklist_格式与缺陷A()
    {
        GateShareGlobals.g_DBAddressList.Add("db-entry");
        FormGlobals.g_ProcessBlackList.Add("cheat.exe", new string('a', 32));
        GateShareLists.SaveProcessBlacklist();

        Assert.Equal(0, GateShareGlobals.g_DBAddressList.Count);          // ★ 缺陷 A
        Assert.Equal("cheat.exe|" + new string('A', 32) + "\r\n",
                     File.ReadAllText(P("ProcessBlacklist.txt"), GBK));    // MD5 已大写
    }

    [Fact]
    public void SaveProcessBlacklist_空名单写空文件()
    {
        GateShareLists.SaveProcessBlacklist();
        Assert.True(File.Exists(P("ProcessBlacklist.txt")));
        Assert.Equal("", File.ReadAllText(P("ProcessBlacklist.txt"), GBK));
    }

    [Fact]
    public void ProcessBlacklist_LoadSave往返()
    {
        FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
        FormGlobals.g_ProcessBlackList.Add("b.exe", new string('2', 32));
        GateShareLists.SaveProcessBlacklist();

        FormGlobals.g_ProcessBlackList.Clear();
        GateShareLists.LoadProcessBlacklist();
        Assert.Equal(2, FormGlobals.g_ProcessBlackList.Count);
        Assert.Equal("a.exe", FormGlobals.g_ProcessBlackList.Find(new string('1', 32)).ProcessName);
    }

    // ================= RebuildProcessBlacklist（原 :1909-1954） =================

    [Fact]
    public void RebuildProcessBlacklist_空名单产出空串与全零MD5()
    {
        GateShareLists.RebuildProcessBlacklist();
        Assert.Equal("", FormGlobals.g_ProcessBlacklistStr);
        Assert.Equal(new byte[16], FormGlobals.g_ProcessBlacklistMD5);
        Assert.Empty(GateShareLists.ProcessBlacklistBytes);
    }

    [Fact]
    public void RebuildProcessBlacklist_非法MD5不计数也不调加密接缝()
    {
        int before = GateShareLists.DefaultCipherCallCount;
        FormGlobals.g_ProcessBlackList.Add("bad", "ZZZZ");
        // ★ 注意 Add 会把 MD5 转大写，但 'ZZZZ' 长度不是 32 → StrToMD5Digest 失败
        GateShareLists.RebuildProcessBlacklist();
        Assert.Equal(before, GateShareLists.DefaultCipherCallCount);        // Count=0 → 原 :1937 的分支不进
        Assert.Equal("", FormGlobals.g_ProcessBlacklistStr);
    }

    [Fact]
    public void RebuildProcessBlacklist_按16字节每个MD5传给加密接缝()
    {
        int before = GateShareLists.DefaultCipherCallCount;
        FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
        FormGlobals.g_ProcessBlackList.Add("b.exe", new string('2', 32));
        FormGlobals.g_ProcessBlackList.Add("c.exe", new string('3', 32));
        GateShareLists.RebuildProcessBlacklist();

        Assert.Equal(before + 1, GateShareLists.DefaultCipherCallCount);   // 原文只调一次 EncryptBuffer
        Assert.Equal(48, GateShareLists.DefaultCipherLastLength);          // 3 × SizeOf(MD5) = 48
    }

    [Fact]
    public void RebuildProcessBlacklist_产出MD5等于压缩字节的MD5()
    {
        FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
        GateShareLists.RebuildProcessBlacklist();

        Assert.NotEmpty(GateShareLists.ProcessBlacklistBytes);
        Assert.Equal(GateShareMd5.MD5Bytes(GateShareLists.ProcessBlacklistBytes),
                     FormGlobals.g_ProcessBlacklistMD5);
    }

    [Fact]
    public void RebuildProcessBlacklist_二进制串与字节逐位对应()
    {
        FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
        GateShareLists.RebuildProcessBlacklist();

        string s = FormGlobals.g_ProcessBlacklistStr;
        Assert.Equal(GateShareLists.ProcessBlacklistBytes.Length, s.Length);
        for (int i = 0; i < s.Length; i++)
            Assert.Equal(GateShareLists.ProcessBlacklistBytes[i], (byte)s[i]);
    }

    [Fact]
    public void RebuildProcessBlacklist_上限80条()
    {
        for (int i = 0; i < 90; i++)
            FormGlobals.g_ProcessBlackList.Add("p" + i, i.ToString("X8").PadLeft(32, '0'));
        // ★ MaxCount=80 会让 Add 在第 81 项返回 nil，所以只能靠 Add 之外校验上限：
        Assert.Equal(80, FormGlobals.g_ProcessBlackList.Count);

        GateShareLists.RebuildProcessBlacklist();
        Assert.Equal(80 * 16, GateShareLists.DefaultCipherLastLength);     // 原 :1933 `if Count >= 80 then Break`
    }

    [Fact]
    public void RebuildProcessBlacklist_可注入加密接缝改结果()
    {
        var original = GateShareLists.ProcessBlacklistCipher;
        try
        {
            var originalZlib = GateShareLists.ZlibCompressBuffer;
            FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
            GateShareLists.ProcessBlacklistCipher = new FakeCipher();
            GateShareLists.ZlibCompressBuffer = (src, len) => src.Take(len).ToArray();   // 关掉 zLib 便于断言
            GateShareLists.RebuildProcessBlacklist();
            Assert.Equal(new byte[] { 0xAA, 0xBB }, GateShareLists.ProcessBlacklistBytes);   // 直接是加密接缝的输出
            GateShareLists.ZlibCompressBuffer = originalZlib;
        }
        finally
        {
            GateShareLists.ProcessBlacklistCipher = original;
        }
    }

    private sealed class FakeCipher : GateShareLists.IProcessBlacklistCipher
    {
        public byte[] EncryptBuffer(byte[] buffer, int length) => new byte[] { 0xAA, 0xBB };
    }

    [Fact]
    public void RebuildProcessBlacklist_可注入zLib接缝()
    {
        var original = GateShareLists.ZlibCompressBuffer;
        try
        {
            FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
            GateShareLists.ZlibCompressBuffer = (src, len) => new byte[] { (byte)len, 0x7F };
            GateShareLists.RebuildProcessBlacklist();
            Assert.Equal(new byte[] { 16, 0x7F }, GateShareLists.ProcessBlacklistBytes);
        }
        finally
        {
            GateShareLists.ZlibCompressBuffer = original;
        }
    }

    [Fact]
    public void RebuildProcessBlacklist_清除时也清字节缓冲()
    {
        FormGlobals.g_ProcessBlackList.Add("a.exe", new string('1', 32));
        GateShareLists.RebuildProcessBlacklist();
        Assert.NotEmpty(GateShareLists.ProcessBlacklistBytes);

        FormGlobals.g_ProcessBlackList.Clear();
        GateShareLists.RebuildProcessBlacklist();
        Assert.Empty(GateShareLists.ProcessBlacklistBytes);
        Assert.Equal(new byte[16], FormGlobals.g_ProcessBlacklistMD5);
    }

    // ================= LoadNoVerifyChrList（原 :1981-2014） =================

    [Fact]
    public void LoadNoVerifyChrList_空路径返回False且清空()
    {
        GateShareGlobals.g_LoadNoVerifyChrList.Add("old");
        GateShareGlobals.g_sLoadNoVerifyChrListFile = "";
        Assert.False(GateShareLists.LoadNoVerifyChrList());
        Assert.Equal(0, GateShareGlobals.g_LoadNoVerifyChrList.Count);
    }

    [Fact]
    public void LoadNoVerifyChrList_文件不存在返回False且清空()
    {
        GateShareGlobals.g_LoadNoVerifyChrList.Add("old");
        GateShareGlobals.g_sLoadNoVerifyChrListFile = P("no-chr.txt");
        Assert.False(GateShareLists.LoadNoVerifyChrList());
        Assert.Equal(0, GateShareGlobals.g_LoadNoVerifyChrList.Count);
    }

    [Fact]
    public void LoadNoVerifyChrList_读取并大小写不敏感去重()
    {
        WriteGb(P("白名单用户.txt"), "Alice\r\nalice\r\nBob\r\n");
        GateShareGlobals.g_sLoadNoVerifyChrListFile = P("白名单用户.txt");
        Assert.True(GateShareLists.LoadNoVerifyChrList());
        Assert.Equal(2, GateShareGlobals.g_LoadNoVerifyChrList.Count);
    }

    [Fact]
    public void LoadNoVerifyChrList_保留空行但空行自身被去重()
    {
        // 原文对行**不做 Trim 也不跳空行**，只靠 IndexOf 去重
        WriteGb(P("chr.txt"), "A\r\n\r\n\r\nB\r\n");
        GateShareGlobals.g_sLoadNoVerifyChrListFile = P("chr.txt");
        GateShareLists.LoadNoVerifyChrList();
        // 文件 4 行（A / '' / '' / B）→ 第二个 '' 被 IndexOf 去重 → 3 项
        Assert.Equal(3, GateShareGlobals.g_LoadNoVerifyChrList.Count);
        Assert.Equal(1, GateShareGlobals.g_LoadNoVerifyChrList.IndexOf(""));
    }

    // ================= InitActionIntervalsFileNames（原 :3376-3387） =================

    [Fact]
    public void InitActionIntervalsFileNames_空槽位也拼上exe目录_原文缺陷C()
    {
        // ★ 缺陷 C：判据是 `Length(数组) > 0`（恒真），不是元素非空
        FormGlobals.g_sActionIntervalsFileNames[0] = "HitIntervals.ini";
        FormGlobals.g_sActionIntervalsFileNames[4] = "";                    // amTurn 原本是空串
        GateShareLists.InitActionIntervalsFileNames();

        Assert.Equal(_dir + Path.DirectorySeparatorChar + "HitIntervals.ini",
                     FormGlobals.g_sActionIntervalsFileNames[0]);
        Assert.Equal(_dir + Path.DirectorySeparatorChar,
                     FormGlobals.g_sActionIntervalsFileNames[4]);            // ← 变成目录路径
    }

    [Fact]
    public void InitActionIntervalsFileNames_二次调用会重复拼接()
    {
        FormGlobals.g_sActionIntervalsFileNames[0] = "A.ini";
        GateShareLists.InitActionIntervalsFileNames();
        GateShareLists.InitActionIntervalsFileNames();
        Assert.Equal(_dir + Path.DirectorySeparatorChar + _dir + Path.DirectorySeparatorChar + "A.ini",
                     FormGlobals.g_sActionIntervalsFileNames[0]);
    }

    [Fact]
    public void InitActionIntervalsFileNames_覆盖全部27项()
    {
        GateShareLists.InitActionIntervalsFileNames();
        Assert.All(FormGlobals.g_sActionIntervalsFileNames,
                   s => Assert.StartsWith(_dir, s));
    }

    // ================= 日志（原 :1434-1477） =================

    [Fact]
    public void AddMainLogMsg_等级门限()
    {
        GateShareGlobals.g_btShowLogLevel = 3;
        GateShareLists.AddMainLogMsg("lvl3", 3);
        GateShareLists.AddMainLogMsg("lvl4", 4);
        GateShareLists.AddMainLogMsg("lvl0", 0);
        Assert.Equal(2, GateShareGlobals.g_MainLogStrings.Count);
    }

    [Fact]
    public void AddMainLogMsg_时间前缀()
    {
        var original = GateShareLists.NowProvider;
        try
        {
            GateShareLists.NowProvider = () => new DateTime(2024, 5, 6, 7, 8, 9);
            GateShareLists.AddMainLogMsg("hello", 3);
            GateShareLists.AddMainLogMsg("raw", 3, false);
            Assert.Equal("[07:08:09] hello", GateShareGlobals.g_MainLogStrings[0]);
            Assert.Equal("raw", GateShareGlobals.g_MainLogStrings[1]);
        }
        finally
        {
            GateShareLists.NowProvider = original;
        }
    }

    [Fact]
    public void AddMainLogMsg_等级高于门限时完全不写()
    {
        GateShareGlobals.g_btShowLogLevel = 0;
        GateShareLists.AddMainLogMsg("x", 1);
        Assert.Equal(0, GateShareGlobals.g_MainLogStrings.Count);
    }

    [Fact]
    public void AddMainLogMsg_负数等级视为通过()
    {
        GateShareGlobals.g_btShowLogLevel = 3;
        GateShareLists.AddMainLogMsg("neg", -1);
        Assert.Equal(1, GateShareGlobals.g_MainLogStrings.Count);
    }

    [Fact]
    public void AddIOCPLogMsg_无门限且总带时间()
    {
        var original = GateShareLists.NowProvider;
        try
        {
            GateShareLists.NowProvider = () => new DateTime(2024, 1, 2, 3, 4, 5);
            GateShareGlobals.g_btShowLogLevel = 0;                          // 与主日志不同：不受门限影响
            GateShareLists.AddIOCPLogMsg("m");
            Assert.Equal("[03:04:05] m", GateShareGlobals.g_IOCPLogStrings[0]);
        }
        finally
        {
            GateShareLists.NowProvider = original;
        }
    }

    [Fact]
    public void AddIOCPLogMsg_多次追加()
    {
        GateShareLists.AddIOCPLogMsg("a");
        GateShareLists.AddIOCPLogMsg("b");
        Assert.Equal(2, GateShareGlobals.g_IOCPLogStrings.Count);
    }

    // ================= Add*Block*（原 :1392-1432） =================

    [Fact]
    public void AddTempBlockIP_重复项由容器去重()
    {
        GateShareLists.AddTempBlockIP("1.2.3.4");
        GateShareLists.AddTempBlockIP("1.2.3.4");
        Assert.Equal(1, GateShareGlobals.g_TempIPList.Count);

        GateShareLists.AddBlockIP("1.2.3.4");
        GateShareLists.AddBlockIP("1.2.3.4");
        Assert.Equal(1, GateShareGlobals.g_BlockIPList.Count);
    }

    [Fact]
    public void AddTempBlockIP_空串被容器拒绝()
    {
        GateShareLists.AddTempBlockIP("");
        GateShareLists.AddBlockIP("");
        Assert.Equal(0, GateShareGlobals.g_TempIPList.Count);
        Assert.Equal(0, GateShareGlobals.g_BlockIPList.Count);
    }

    [Fact]
    public void AddTempBlockMac_显式IndexOf去重()
    {
        GateShareLists.AddTempBlockMac("MAC");
        GateShareLists.AddTempBlockMac("MAC");
        GateShareLists.AddTempBlockMac("MAC2");
        Assert.Equal(2, GateShareGlobals.g_TempMacList.Count);
    }

    [Fact]
    public void AddBlockMac_显式IndexOf去重_大小写不敏感()
    {
        GateShareLists.AddBlockMac("mac");
        GateShareLists.AddBlockMac("MAC");                                 // TStringList.IndexOf 不敏感
        Assert.Equal(1, GateShareGlobals.g_BlockMacList.Count);
        Assert.Equal(0, GateShareGlobals.g_BlockMacList.IndexOf("MaC"));
    }

    [Fact]
    public void AddTempBlockMac_空串只加一次()
    {
        // 空串是合法元素（IndexOf('') 首次为 -1）
        GateShareLists.AddTempBlockMac("");
        GateShareLists.AddTempBlockMac("");
        Assert.Equal(1, GateShareGlobals.g_TempMacList.Count);
    }
}
