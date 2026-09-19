using System.Collections.Generic;
using System.IO;
using GXX.Core.Util;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>接缝 TIniFile（DBShare.pas / Setting.pas / Ranking.pas 用）的语义与键序测试。</summary>
public class IniFileTests : TempDirTest
{
    [Fact]
    public void 写入键序即落盘顺序_已存在键就地改值不移动位置()
    {
        string f = Path2("a.ini");
        var ini = new TIniFile(f);
        ini.WriteInteger("Setup", "A", 1);
        ini.WriteInteger("Setup", "B", 2);
        ini.WriteInteger("Setup", "A", 9);
        ini.WriteBool("Other", "X", true);
        ini.Dispose();

        Assert.Equal("[Setup]\r\nA=9\r\nB=2\r\n[Other]\r\nX=1\r\n", File.ReadAllText(f));
    }

    [Fact]
    public void 节创建顺序即落盘顺序_EraseSection_不创建空节()
    {
        string f = Path2("b.ini");
        var ini = new TIniFile(f);
        ini.EraseSection("S0");          // 空节：不应出现在文件里
        ini.WriteString("S1", "k", "v");
        ini.WriteString("S0", "k", "v");
        ini.Dispose();

        string text = File.ReadAllText(f);
        Assert.Equal("[S1]\r\nk=v\r\n[S0]\r\nk=v\r\n", text);
        Assert.DoesNotContain("EraseSection", text);
    }

    [Fact]
    public void EraseSection_整节删除_保留其它节()
    {
        string f = Path2("c.ini");
        var ini = new TIniFile(f);
        ini.WriteInteger("A", "1", 1);
        ini.WriteInteger("B", "1", 2);
        ini.EraseSection("A");
        ini.Dispose();

        Assert.Equal("[B]\r\n1=2\r\n", File.ReadAllText(f));
        var reread = new TIniFile(f);
        Assert.False(reread.SectionExists("A"));
        Assert.True(reread.SectionExists("B"));
    }

    [Fact]
    public void ReadBool_按整数读写_写入为1或0()
    {
        string f = Path2("d.ini");
        var ini = new TIniFile(f);
        ini.WriteBool("S", "T", true);
        ini.WriteBool("S", "F", false);
        Assert.Equal("[S]\r\nT=1\r\nF=0\r\n", File.ReadAllText(f));
        Assert.True(ini.ReadBool("S", "T", false));
        Assert.False(ini.ReadBool("S", "F", true));
        Assert.True(ini.ReadBool("S", "Missing", true));
        Assert.False(ini.ReadBool("S", "Missing", false));
    }

    [Fact]
    public void ReadInteger_空值或非数字回退默认值_十六进制带美元号()
    {
        string f = Path2("e.ini");
        var ini = new TIniFile(f);
        ini.WriteString("S", "Empty", "");
        ini.WriteString("S", "Bad", "abc");
        ini.WriteString("S", "Hex", "$1F");
        ini.WriteString("S", "Neg", "-7");
        Assert.Equal(42, ini.ReadInteger("S", "Empty", 42));
        Assert.Equal(42, ini.ReadInteger("S", "Bad", 42));
        Assert.Equal(31, ini.ReadInteger("S", "Hex", 42));
        Assert.Equal(-7, ini.ReadInteger("S", "Neg", 42));
        Assert.Equal(42, ini.ReadInteger("S", "Nope", 42));
    }

    [Fact]
    public void ReadSection_先清空_ReadSectionValues_不清空_原文如此()
    {
        string f = Path2("f.ini");
        var ini = new TIniFile(f);
        ini.WriteString("A", "k1", "v1");
        ini.WriteString("B", "k2", "v2");

        var sl = new TStringList();
        ini.ReadSection("A", sl);
        Assert.Equal(1, sl.Count);
        ini.ReadSection("B", sl);
        Assert.Equal(1, sl.Count);               // ReadSection 会 Clear
        Assert.Equal("k2", sl[0]);

        var vals = new TStringList();
        ini.ReadSectionValues("A", vals);
        Assert.Equal("k1=v1", vals[0]);
        // 原文 TIniFile.ReadSectionValues 不清空 Strings → 跨节累积（LoadServerInfo 的实际行为）
        ini.ReadSectionValues("B", vals);
        Assert.Equal(2, vals.Count);
        Assert.Equal("k1=v1", vals[0]);
        Assert.Equal("k2=v2", vals[1]);
    }

    [Fact]
    public void 读回时可容忍段间空行与注释_FastIniFile_写的格式也能读()
    {
        string f = Path2("g.ini");
        File.WriteAllText(f, "; comment\r\n[S]\r\n\r\nk=v\r\n\r\n");
        var ini = new TIniFile(f);
        Assert.Equal("v", ini.ReadString("S", "k", ""));
        Assert.True(ini.ValueExists("S", "k"));
    }

    [Fact]
    public void DeleteKey_删除单个键_文件里不再出现()
    {
        string f = Path2("h.ini");
        var ini = new TIniFile(f);
        ini.WriteString("S", "a", "1");
        ini.WriteString("S", "b", "2");
        ini.DeleteKey("S", "a");
        Assert.Equal("[S]\r\nb=2\r\n", File.ReadAllText(f));
    }
}
