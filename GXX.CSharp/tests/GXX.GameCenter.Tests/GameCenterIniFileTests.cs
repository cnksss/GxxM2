using System;
using System.Collections.Generic;
using System.IO;
using GXX.GameCenter;
using Xunit;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GMain.pas 依赖的 Delphi <c>IniFiles.TIniFile</c> 语义测试（<see cref="GameCenterIniFile"/>）。
/// 重点：构造即建空文件、写操作内存缓冲、UpdateFile 全量落盘、同节同键追加不去重、
/// GBK + CRLF 落盘、EraseSection/DeleteKey/ReadSection。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class GameCenterIniFileTests : GameCenterTestBase
{
    private string IniPath => Under("t.ini");

    [Fact]
    public void Create_WhenFileMissing_CreatesEmptyFile()
    {
        Assert.False(File.Exists(IniPath));
        using var ini = new GameCenterIniFile(IniPath);
        Assert.True(File.Exists(IniPath));
        Assert.Equal("", ReadGbk(IniPath));
    }

    [Fact]
    public void Create_CreatesMissingDirectories()
    {
        string nested = Under("a", "b", "c.ini");
        using var ini = new GameCenterIniFile(nested);
        Assert.True(File.Exists(nested));
    }

    [Fact]
    public void Write_DoesNotTouchDiskUntilUpdateFile()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("S", "K", "V");
        Assert.Equal("", ReadGbk(IniPath));

        ini.UpdateFile();
        AssertLines(IniPath, "[S]", "K=V", "");
    }

    [Fact]
    public void Write_KeepsSectionAndKeyInsertionOrder()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("B", "k1", "v1");
        ini.WriteString("A", "k2", "v2");
        ini.WriteString("B", "k3", "v3");
        ini.UpdateFile();

        // 节序按首次出现；B 节内 k1 在 k3 之前。
        AssertLines(IniPath, "[B]", "k1=v1", "k3=v3", "", "[A]", "k2=v2", "");
    }

    [Fact]
    public void Write_SameKeyTwiceInSameSection_AppendsDuplicate()
    {
        // 原文 TIniFile 的写入是"追加行"语义，不做去重（ClearGlobal 依赖此行为）。
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteInteger("Setup", "GlobalVal0", 0);
        ini.WriteInteger("Setup", "GlobalVal0", 7);
        ini.UpdateFile();

        AssertLines(IniPath, "[Setup]", "GlobalVal0=0", "GlobalVal0=7", "");
    }

    [Fact]
    public void ReadString_OnDuplicateKey_ReturnsLastValue()
    {
        // 与上面落盘文本一致：读回时后写覆盖（原文 TMemIniFile 逐行覆盖）。
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteInteger("Setup", "GlobalVal0", 0);
        ini.WriteInteger("Setup", "GlobalVal0", 7);
        ini.UpdateFile();

        using var reread = new GameCenterIniFile(IniPath);
        Assert.Equal(7, reread.ReadInteger("Setup", "GlobalVal0", -1));
    }

    [Fact]
    public void ReadString_ReturnsDefaultWhenMissing()
    {
        using var ini = new GameCenterIniFile(IniPath);
        Assert.Equal("读读取", ini.ReadString("ClearServer", "MyGetTxt0", "读读取"));
        Assert.Equal(0, ini.ReadInteger("ClearServer", "MyGetTxtNum", 0));
    }

    [Fact]
    public void ReadInteger_NonNumericFallsBackToDefault()
    {
        File.WriteAllText(IniPath, "[S]\r\nK=abc\r\n", GXX.Core.EncodingInit.GBK);
        using var ini = new GameCenterIniFile(IniPath);
        Assert.Equal(42, ini.ReadInteger("S", "K", 42));
    }

    [Fact]
    public void ReadBool_AcceptsOneAndTrueOnly()
    {
        File.WriteAllText(IniPath, "[S]\r\nA=1\r\nB=True\r\nC=-1\r\nD=0\r\n", GXX.Core.EncodingInit.GBK);
        using var ini = new GameCenterIniFile(IniPath);
        Assert.True(ini.ReadBool("S", "A", false));
        Assert.True(ini.ReadBool("S", "B", false));
        // 差异断言：'-1' 虽是 Delphi BoolToStr(True) 的输出，但 TIniFile.ReadBool 只认 '1'/'True'，
        // 故 GMain 用 WriteString(BoolToStr(...)) 写入的布尔键**读回为 False**（原文缺陷，逐字保留）。
        Assert.False(ini.ReadBool("S", "C", true));
        Assert.False(ini.ReadBool("S", "D", true));
    }

    [Fact]
    public void WriteBool_WritesOneOrZero()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteBool("Setup", "EmbeddedWindow", true);
        ini.WriteBool("Setup", "UseSqliteDB", false);
        ini.UpdateFile();
        AssertLines(IniPath, "[Setup]", "EmbeddedWindow=1", "UseSqliteDB=0", "");
    }

    [Fact]
    public void DeleteKey_RemovesKeyAndKeepsSection()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("Exp", "Level1", "1");
        ini.WriteString("Exp", "BaseExp", "2");
        ini.DeleteKey("Exp", "BaseExp");
        ini.UpdateFile();

        using var reread = new GameCenterIniFile(IniPath);
        var keys = new List<string>();
        reread.ReadSection("Exp", keys);
        Assert.Equal(new[] { "Level1" }, keys);
    }

    [Fact]
    public void DeleteKey_OnMissingSectionOrKey_IsNoOp()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("Exp", "Level1", "1");
        ini.DeleteKey("NoSuchSection", "X");
        ini.DeleteKey("Exp", "NoSuchKey");
        ini.UpdateFile();

        using var reread = new GameCenterIniFile(IniPath);
        Assert.Equal("1", reread.ReadString("Exp", "Level1", ""));
    }

    [Fact]
    public void EraseSection_RemovesSectionEntirely()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("GateDBPort0", "1", "27201");
        ini.WriteString("Keep", "K", "V");
        ini.EraseSection("GateDBPort0");
        ini.UpdateFile();

        using var reread = new GameCenterIniFile(IniPath);
        var sections = new List<string>();
        reread.ReadSections(sections);
        Assert.Equal(new[] { "Keep" }, sections);
        Assert.Equal("", reread.ReadString("GateDBPort0", "1", ""));
    }

    [Fact]
    public void Load_ParsesSectionsKeysAndIgnoresComments()
    {
        File.WriteAllText(IniPath,
            "; comment\r\n# hash\r\n[Setup]\r\nServerName=测试服\r\n\r\n[Server]\r\nIDSAddr=127.0.0.1\r\n",
            GXX.Core.EncodingInit.GBK);
        using var ini = new GameCenterIniFile(IniPath);
        Assert.Equal("测试服", ini.ReadString("Setup", "ServerName", ""));
        Assert.Equal("127.0.0.1", ini.ReadString("Server", "IDSAddr", ""));
        var sections = new List<string>();
        ini.ReadSections(sections);
        Assert.Equal(new[] { "Setup", "Server" }, sections);
    }

    [Fact]
    public void Load_TrimsKeyAndValueWhitespace()
    {
        File.WriteAllText(IniPath, "[Setup]\r\n   ServerName   =   BmM2   \r\n", GXX.Core.EncodingInit.GBK);
        using var ini = new GameCenterIniFile(IniPath);
        Assert.Equal("BmM2", ini.ReadString("Setup", "ServerName", ""));
    }

    [Fact]
    public void Load_PreservesKeysWithTrailingSpaceInName()
    {
        // GMain.pas:2180 的 'DataSaveDBPassword ' 尾部空格键名按原样写盘（TIniFile 不修剪 Ident），
        // 读回时 Trim 后查询即可命中。
        using (var w = new GameCenterIniFile(IniPath))
        {
            w.WriteString("DataSaveDB", "DataSaveDBPassword ", "pw");
            w.WriteString("DataSaveDB", "DataSaveDBPassword", "pw2");
            w.UpdateFile();
        }

        AssertLines(IniPath, "[DataSaveDB]", "DataSaveDBPassword =pw", "DataSaveDBPassword=pw2", "");
        Assert.Contains("DataSaveDBPassword =pw", ReadGbk(IniPath));

        // 差异说明：TIniFile 读取时对 Ident 做 Trim → 两个磁盘键读回为同一个键名，后者覆盖前者。
        // GMain.pas:2180 的尾部空格只影响**写盘文本**，不影响读回值（原文即如此）。
        using var ini = new GameCenterIniFile(IniPath);
        // 差异说明：TIniFile 读取时 Ident 会被 Trim，因此写盘时的 'DataSaveDBPassword '（尾空格）
        // 读回时按 'DataSaveDBPassword' 命中；带尾空格的查询键不再等于磁盘键名 → 未命中返回默认值。
        Assert.Equal("<MISS>", ini.ReadString("DataSaveDB", "DataSaveDBPassword ", "<MISS>"));
        Assert.Equal("pw2", ini.ReadString("DataSaveDB", "DataSaveDBPassword", ""));
        var keys = new List<string>();
        ini.ReadSection("DataSaveDB", keys);
        Assert.Equal(new[] { "DataSaveDBPassword", "DataSaveDBPassword" }, keys);
    }

    [Fact]
    public void RoundTrip_WritesGbkAndCrlf()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("Setup", "ServerName", "引擎控制台");
        ini.UpdateFile();

        string text = ReadGbk(IniPath);
        Assert.Contains("ServerName=引擎控制台", text);
        Assert.Contains("\r\n", text);
        byte[] raw = File.ReadAllBytes(IniPath);
        // '引' 的 GBK 编码为 D2 FD，确认未写成 UTF-8。
        Assert.Contains((byte)0xD2, raw);
        Assert.Contains((byte)0xFD, raw);
    }

    [Fact]
    public void ReadSection_ReturnsRepeatedKeysInFileOrder()
    {
        File.WriteAllText(IniPath, "[S]\r\nA=1\r\nB=2\r\nA=3\r\n", GXX.Core.EncodingInit.GBK);
        using var ini = new GameCenterIniFile(IniPath);
        var keys = new List<string>();
        ini.ReadSection("S", keys);
        Assert.Equal(new[] { "A", "B", "A" }, keys);
    }

    [Fact]
    public void ReadSection_MissingSection_LeavesListUnchanged()
    {
        // 原文 ReadSection 只追加；ClearSetupIni 在调用前显式 SL.Clear。
        using var ini = new GameCenterIniFile(IniPath);
        var keys = new List<string> { "pre" };
        ini.ReadSection("Nope", keys);
        Assert.Equal(new[] { "pre" }, keys);
    }

    [Fact]
    public void ReadSections_ReturnsSectionOrder()
    {
        using var ini = new GameCenterIniFile(IniPath);
        ini.WriteString("Z", "k", "v");
        ini.WriteString("A", "k", "v");
        ini.UpdateFile();
        using var reread = new GameCenterIniFile(IniPath);
        var sections = new List<string>();
        reread.ReadSections(sections);
        Assert.Equal(new[] { "Z", "A" }, sections);
    }

    [Fact]
    public void FileName_IsExposedVerbatim()
    {
        using var ini = new GameCenterIniFile(IniPath);
        Assert.Equal(IniPath, ini.FileName);
    }
}
