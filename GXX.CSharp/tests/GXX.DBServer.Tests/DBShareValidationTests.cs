using System;
using System.IO;
using System.Text;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// `Source\DBServer\DBShare.pas` 的**人物名校验族**（本车道移植的部分）测试。
///
/// ★ 参数表示（与 `DBShare.cs` 的类注释一致）：
///   · 收 <c>string</c>(AnsiString) 的 5 个函数 → 传 **latin-1 字节串**（`Ansi(...)` / `Raw(...)` 助手）；
///   · 收 <c>WideString</c> 的 2 个函数（`CheckSpecialChar` / `CheckCanCaseChar`）→ 传 **GBK 文本**。
/// </summary>
public class DBShareValidationTests : TempDirTest
{
    public DBShareValidationTests()
    {
        // ★ 必须显式复位本车道新增的**共享静态接缝**：`SelectClientTestBase` 会把名校验族换成"全放行"桩，
        //   而 `TestReset.All()`（TestHelpers.cs，不在本车道分区）不认识这些新接缝 ⇒ 不在这里复位就会
        //   受**同类测试的执行顺序**影响（实测：本类的"转调真实现"用例在整跑时红、单跑时绿）。
        SelectClientDbShareSeam.Reset();
        IDSocCliSeam.Reset();
        SelectClientRoleDbSeam.Reset();
        SelectClientGlobals.Reset();
        TSelectClient.ResetSeams();
        SelectClientGateWiring.DetachAll();
        SelectClientModuleSeam.Reset();

        DBShareSeam.g_DenyChrNameList = new GXX.Core.Util.TStringList();
        DBShareSeam.g_FilterNewHumanNameTextList = new GXX.Core.Util.TStringList();
    }

    /// <summary>GBK 文本 → latin-1 字节串（Delphi AnsiString 的托管表示）。</summary>
    private static string Ansi(string text)
        => Encoding.Latin1.GetString(Encoding.GetEncoding(936).GetBytes(text));

    /// <summary>原始字节 → latin-1 字节串（用于构造非法/边界 GBK 序列）。</summary>
    private static string Raw(params byte[] bytes) => Encoding.Latin1.GetString(bytes);

    // =====================================================================================
    // CheckDenyChrName（:1043-1056）
    // =====================================================================================

    [Fact]
    public void CheckDenyChrName_名单为空时为真()
    {
        Assert.True(DBShare.CheckDenyChrName(Ansi("Aaaa")));
    }

    [Fact]
    public void CheckDenyChrName_命中禁用名单为假()
    {
        DBShareSeam.g_DenyChrNameList.Add(Ansi("GM"));
        Assert.False(DBShare.CheckDenyChrName(Ansi("GM")));
        Assert.True(DBShare.CheckDenyChrName(Ansi("Player")));
    }

    [Fact]
    public void CheckDenyChrName_大小写不敏感()
    {
        DBShareSeam.g_DenyChrNameList.Add(Ansi("Gm"));
        Assert.False(DBShare.CheckDenyChrName(Ansi("gM")));                        // 原文 CompareText(...) = 0
    }

    [Fact]
    public void CheckDenyChrName_汉字名字可命中()
    {
        DBShareSeam.g_DenyChrNameList.Add(Ansi("管理员"));
        Assert.False(DBShare.CheckDenyChrName(Ansi("管理员")));
        Assert.True(DBShare.CheckDenyChrName(Ansi("战士")));
    }

    // =====================================================================================
    // CheckFilterNewHumanChrName（:1058-1077）
    // =====================================================================================

    [Fact]
    public void CheckFilterNewHumanChrName_含0x01为真()
    {
        Assert.True(DBShare.CheckFilterNewHumanChrName(Raw(0x41, 0x01, 0x42)));
    }

    [Fact]
    public void CheckFilterNewHumanChrName_含0xFF为真()
    {
        Assert.True(DBShare.CheckFilterNewHumanChrName(Raw(0x41, 0xFF, 0x42)));
    }

    [Fact]
    public void CheckFilterNewHumanChrName_命中过滤表为真()
    {
        DBShareSeam.g_FilterNewHumanNameTextList.Add(Ansi("fuck"));
        Assert.True(DBShare.CheckFilterNewHumanChrName(Ansi("xfuckx")));           // 子串匹配（Pos > 0）
        Assert.False(DBShare.CheckFilterNewHumanChrName(Ansi("hello")));
    }

    [Fact]
    public void CheckFilterNewHumanChrName_两边都大写_故大小写不敏感()
    {
        DBShareSeam.g_FilterNewHumanNameTextList.Add(Ansi("FuCk"));
        Assert.True(DBShare.CheckFilterNewHumanChrName(Ansi("xxfuckxx")));         // :1063 与 :1070 各折一次
    }

    [Fact]
    public void CheckFilterNewHumanChrName_都不命中为假()
    {
        DBShareSeam.g_FilterNewHumanNameTextList.Add(Ansi("zzz"));
        Assert.False(DBShare.CheckFilterNewHumanChrName(Ansi("Aaaa")));
    }

    [Fact]
    public void CheckFilterNewHumanChrName_UpperCase只折ASCII_不把0xE0折成0xC0()
    {
        // ★★ 差异锁定：`DBShare.UpperCaseAnsi` 只折 ASCII 字母（CP936 DBCS 语义）。
        //    若误用 `DelphiRTL.UpperCase`（ToUpperInvariant），0xE9 与 0xC9 会折到一起 ⇒ **假命中**。
        DBShareSeam.g_FilterNewHumanNameTextList.Add(Raw(0xC9));                   // 'É'
        Assert.False(DBShare.CheckFilterNewHumanChrName(Raw(0xE9)));               // 'é' —— 不应命中
        Assert.True(DBShare.CheckFilterNewHumanChrName(Raw(0xC9)));                // 同一个字节才命中
    }

    // =====================================================================================
    // CheckNumberName（:1103-1122）
    // =====================================================================================

    [Theory]
    [InlineData("Aaaa", false)]
    [InlineData("Aaa1", true)]
    [InlineData("1234", true)]
    [InlineData("0", true)]
    [InlineData("A9z", true)]
    public void CheckNumberName_含ASCII数字为真(string name, bool expected)
    {
        Assert.Equal(expected, DBShare.CheckNumberName(Ansi(name)));
    }

    [Fact]
    public void CheckNumberName_汉字名字不算数字()
    {
        // 汉字在 GBK 下首字节 ≥0x81，`S[1]` 落不到 0x30..0x39
        Assert.False(DBShare.CheckNumberName(Ansi("战士")));
    }

    [Fact]
    public void CheckNumberName_空串为假()
    {
        Assert.False(DBShare.CheckNumberName(""));
    }

    // =====================================================================================
    // CheckLetterName（:1124-1143）
    // =====================================================================================

    [Theory]
    [InlineData("AbcD", true)]
    [InlineData("a", true)]
    [InlineData("Abc1", false)]
    [InlineData("Abc ", false)]
    [InlineData("Abc-", false)]
    public void CheckLetterName_全是英文字母才为真(string name, bool expected)
    {
        Assert.Equal(expected, DBShare.CheckLetterName(Ansi(name)));
    }

    [Fact]
    public void CheckLetterName_汉字为假()
    {
        Assert.False(DBShare.CheckLetterName(Ansi("战士")));
    }

    [Fact]
    public void CheckLetterName_空串为真_原文如此()
    {
        // :1131 `Result := True` 且循环不执行 ⇒ 空串返回 **True**
        Assert.True(DBShare.CheckLetterName(""));
    }

    // =====================================================================================
    // CheckChrName（:1204-1249）—— 含 :1247 的隐式 GBK 解码
    // =====================================================================================

    [Theory]
    [InlineData("Aaaa")]
    [InlineData("a")]
    [InlineData("A1b2")]
    [InlineData("")]
    public void CheckChrName_允许ASCII字母数字(string name)
    {
        Assert.True(DBShare.CheckChrName(Ansi(name)));
    }

    [Theory]
    [InlineData("Aaa@")]                                                          // 0x40 不在单字节白名单
    [InlineData("Aaa ")]                                                          // 0x20
    [InlineData("Aa-a")]
    public void CheckChrName_拒绝非字母数字的ASCII(string name)
    {
        Assert.False(DBShare.CheckChrName(Ansi(name)));
    }

    [Fact]
    public void CheckChrName_接受合法GBK双字节()
    {
        Assert.True(DBShare.CheckChrName(Ansi("战士")));                            // 0xD5 0xBD 0xCA 0xBF
    }

    [Fact]
    public void CheckChrName_单个0x80被拒()
    {
        // 0x80 既不在 0x81..0xFE（双字节前导），也不是 A-Za-z0-9
        Assert.False(DBShare.CheckChrName(Raw(0x80)));
    }

    [Fact]
    public void CheckChrName_双字节尾字节越界被拒()
    {
        Assert.False(DBShare.CheckChrName(Raw(0xB0, 0x3F)));                       // 尾字节 < 0x40
    }

    [Fact]
    public void CheckChrName_前导大于0xF7时尾字节上界收到0xA0()
    {
        // :1222 `FirstChr <= #$F7` 分支允许尾字节 0x40..0xFE；
        // :1223 `FirstChr > #$F7` 分支只允许尾字节 0x40..0xA0
        Assert.True(DBShare.CheckChrName(Raw(0xF7, 0xB0)));                        // 前导 0xF7 ⇒ 上界 0xFE
        Assert.True(DBShare.CheckChrName(Raw(0xF8, 0xA0)));                        // 前导 0xF8 ⇒ 上界 0xA0
        Assert.False(DBShare.CheckChrName(Raw(0xF8, 0xA1)));                       // 超出 0xA0 ⇒ 被拒
    }

    [Fact]
    public void CheckChrName_全角字母被CheckCanCaseChar拒掉()
    {
        // ★ :1247 `Result := CheckCanCaseChar(sChrName)` —— AnsiString→WideString 是**隐式 GBK 解码**。
        //   全角 Ａ 的 GBK 字节 0xA3 0xC1 能通过字节检查，但解码成 'Ａ' 后命中 FilterChars ⇒ False。
        Assert.False(DBShare.CheckChrName(Ansi("Ａ")));
    }

    // =====================================================================================
    // CheckCanCaseChar（:1177-1202）—— FilterChars 逐字照抄
    // =====================================================================================

    [Fact]
    public void CheckCanCaseChar_普通ASCII为真()
    {
        Assert.True(DBShare.CheckCanCaseChar("Aaaa"));
        Assert.True(DBShare.CheckCanCaseChar(""));
    }

    [Theory]
    [InlineData("Α")]          // 希腊大写 Alpha
    [InlineData("α")]          // 希腊小写 alpha
    [InlineData("А")]          // 西里尔大写 A
    [InlineData("а")]          // 西里尔小写 a
    [InlineData("Ⅰ")]          // 罗马数字
    [InlineData("Ａ")]          // 全角大写 A
    [InlineData("ａ")]          // 全角小写 a
    [InlineData("ё")]          // 西里尔 ё
    public void CheckCanCaseChar_命中FilterChars为假(string ch)
    {
        Assert.False(DBShare.CheckCanCaseChar(ch));
    }

    [Fact]
    public void CheckCanCaseChar_汉字不在FilterChars里()
    {
        Assert.True(DBShare.CheckCanCaseChar("战士"));
    }

    // =====================================================================================
    // CheckSpecialChar（:1251-1274）
    // =====================================================================================

    [Fact]
    public void CheckSpecialChar_普通名字为真()
    {
        Assert.True(DBShare.CheckSpecialChar("Aaaa"));
        Assert.True(DBShare.CheckSpecialChar(""));
        Assert.True(DBShare.CheckSpecialChar("战士"));
    }

    [Theory]
    [InlineData("A a")]        // 空格
    [InlineData("A@a")]
    [InlineData("A/a")]
    [InlineData("A.a")]
    [InlineData("A\\a")]
    [InlineData("A'a")]
    [InlineData("A\"a")]
    [InlineData("A_a")]
    [InlineData("A{a}")]
    [InlineData("A|a")]
    public void CheckSpecialChar_命中FilterChars为假(string name)
    {
        Assert.False(DBShare.CheckSpecialChar(name));
    }

    [Fact]
    public void CheckSpecialChar_FilterChars逐字符核对()
    {
        // 原文 :1253 `' /@?''"\.,:;`~!#$%^&*()-_+|[]{}'` —— 逐字照抄，含被转义的单引号。
        const string expect = " /@?'\"\\.,:;`~!#$%^&*()-_+|[]{}";
        foreach (char c in expect) Assert.False(DBShare.CheckSpecialChar(c.ToString()), "应命中：" + c);
    }

    [Fact]
    public void CheckSpecialChar在NewChr里是死分支()
    {
        // ★★ 原文缺陷/死分支（SelectClient.pas:942-948）：
        //   走到那一步的前提是 `nCode = -1`，而它要求 `CheckChrName`（:934）**已返回真**；
        //   `CheckChrName` 只放行 `0-9a-zA-Z` 或**合法的 GBK 双字节**，
        //   而 `CheckSpecialChar` 的 `FilterChars` 全是**非字母数字的 ASCII**（≤0x7D）——
        //   两者交集为空 ⇒ **`CheckSpecialChar` 永远不可能在那里返回 False**。
        //   本用例以"属性"形式锁定：凡被 `CheckSpecialChar` 拒的名字，必先被 `CheckChrName` 拒。
        const string filterChars = " /@?'\"\\.,:;`~!#$%^&*()-_+|[]{}";
        foreach (char c in filterChars)
        {
            string wide = "Aa" + c + "a";
            Assert.False(DBShare.CheckSpecialChar(wide), "CheckSpecialChar 应当拒绝：" + c);
            Assert.False(DBShare.CheckChrName(Ansi(wide)), "但 CheckChrName 会先拒绝：" + c);
        }
    }

    // =====================================================================================
    // LoadChrNameList（:403-425）
    // =====================================================================================

    [Fact]
    public void LoadChrNameList_文件不存在时返回假且不动名单()
    {
        DBShareSeam.g_DenyChrNameList.Add("keepme");
        Assert.False(DBShare.LoadChrNameList(Path2("nope.txt")));
        Assert.Equal(1, DBShareSeam.g_DenyChrNameList.Count);                      // :408 整个 if 不执行
        Assert.Equal("keepme", DBShareSeam.g_DenyChrNameList[0]);
    }

    [Fact]
    public void LoadChrNameList_载入并清掉空白行()
    {
        string file = Path2("deny.txt");
        File.WriteAllText(file, "aaa\r\n\r\n   \r\n\t\r\nbbb\r\n", Encoding.GetEncoding(936));

        Assert.True(DBShare.LoadChrNameList(file));

        Assert.Equal(2, DBShareSeam.g_DenyChrNameList.Count);                      // 空行/空白行/制表符行都被删掉
        Assert.Equal("aaa", DBShareSeam.g_DenyChrNameList[0]);
        Assert.Equal("bbb", DBShareSeam.g_DenyChrNameList[1]);
    }

    [Fact]
    public void LoadChrNameList_重载会先清空旧内容()
    {
        string file = Path2("deny2.txt");
        File.WriteAllText(file, "new1\r\n", Encoding.GetEncoding(936));
        DBShareSeam.g_DenyChrNameList.Add("old");

        Assert.True(DBShare.LoadChrNameList(file));

        Assert.Equal(1, DBShareSeam.g_DenyChrNameList.Count);                      // :410 Clear
        Assert.Equal("new1", DBShareSeam.g_DenyChrNameList[0]);
    }

    [Fact]
    public void LoadChrNameList_载入后CheckDenyChrName能命中()
    {
        string file = Path2("deny3.txt");
        File.WriteAllText(file, "GM\r\nAdmin\r\n", Encoding.GetEncoding(936));

        DBShare.LoadChrNameList(file);

        Assert.False(DBShare.CheckDenyChrName(Ansi("gm")));
        Assert.False(DBShare.CheckDenyChrName(Ansi("ADMIN")));
        Assert.True(DBShare.CheckDenyChrName(Ansi("Player")));
    }

    // =====================================================================================
    // 接缝转调（SelectClientDbShareSeam → DBShare）
    // =====================================================================================

    [Fact]
    public void 接缝已转调真实现_不再抛()
    {
        Assert.True(SelectClientDbShareSeam.CheckChrName(Ansi("Aaaa")));
        Assert.False(SelectClientDbShareSeam.CheckChrName(Ansi("Aaa@")));
        Assert.True(SelectClientDbShareSeam.CheckSpecialChar("Aaaa"));
        Assert.False(SelectClientDbShareSeam.CheckSpecialChar("A a"));
        Assert.True(SelectClientDbShareSeam.CheckDenyChrName(Ansi("Aaaa")));
        Assert.False(SelectClientDbShareSeam.CheckNumberName(Ansi("Aaaa")));
        Assert.True(SelectClientDbShareSeam.CheckNumberName(Ansi("Aaa1")));
        Assert.True(SelectClientDbShareSeam.CheckLetterName(Ansi("Abcd")));
        Assert.False(SelectClientDbShareSeam.CheckFilterNewHumanChrName(Ansi("Aaaa")));
    }

    [Fact]
    public void 接缝常量与DBShare单一真源一致()
    {
        Assert.Equal(DBShare.MIN_CHAR_NAME_LEN, SelectClientDbShareSeam.MIN_CHAR_NAME_LEN);
        Assert.Equal(DBShare.MAX_CHAR_NAME_LEN, SelectClientDbShareSeam.MAX_CHAR_NAME_LEN);
        Assert.Equal((byte)DBShare.TextCharsFirst, SelectClientDbShareSeam.TextCharsFirst);
        Assert.Equal((byte)DBShare.TextCharsLast, SelectClientDbShareSeam.TextCharsLast);
    }

    [Fact]
    public void 主动网关路由族已转调真实现_不再抛()
    {
        // 2026 第 4 轮：`GateActiveRouteIP` / `CheckActiveRunGate` 已移植（`DBShare.cs`）⇒ 接缝同样改为转调。
        // ★ 该路径**默认关闭**（`g_boUseActiveRunGage` 默认 False，SelectClient.pas:1138 判它）
        //   —— 真实现的行为覆盖见 `DBShareActiveGateTests`（那里显式打开开关）。
        SelectClientDbShareSeam.Reset();
        Assert.Equal("", SelectClientDbShareSeam.GateActiveRouteIP("127.0.0.1", out int port));
        Assert.Equal(0, port);
        Assert.False(SelectClientDbShareSeam.CheckActiveRunGate("127.0.0.1", 7200));
    }
}
