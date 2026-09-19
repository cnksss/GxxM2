using System;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// <c>TCustomIniFile.WriteBool/ReadBool</c> 的原文语义锁定（<c>Source/Common/FastIniFile/FastIniFile.pas</c>）。
///
/// 原文事实（回读确认，非直觉）：
///   * <c>FastIniFile.pas:370</c> <c>TFastIniFile = class(TCustomIniFile)</c>；
///   * <c>:428</c>「And the rest of the Readers/Writers are inherited from TCustomIniFile」
///     —— 该单元**没有**自己的 ReadBool/WriteBool/ReadInteger/WriteInteger；
///   * 本仓库无 Delphi RTL 源码，同源镜像见 <c>Source/Common/MemoryIniFiles.pas</c>：
///     <c>ReadInteger 657-666</c>（'0x'→'$'）、<c>WriteInteger 668-671</c>、
///     <c>ReadBool 673-676</c>（<c>ReadInteger(...) &lt;&gt; 0</c>）、
///     <c>WriteBool 678-683</c>（<c>Values: array[Boolean] of string = ('0','1')</c>）；
///   * 故 <b>WriteBool 写 '1'/'0'（不是 SysUtils 的 '-1'/'0'）</b>，ReadBool 认"任何非 0 整数"。
/// </summary>
public class BoolStrFastIniFileTests : IDisposable
{
    private readonly string _dir;

    public BoolStrFastIniFileTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx-core-boolstr-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private string Path2(string name) => Path.Combine(_dir, name);

    // ============================ WriteBool 写 '1'/'0'（原文如此） ============================

    [Fact]
    public void WriteBool_原文写1和0_不是负一()
    {
        string f = Path2("w.ini");
        using (var ini = new TFastIniFile(f))
        {
            ini.WriteBool("S", "T", true);
            ini.WriteBool("S", "F", false);
            ini.Save();
        }

        string text = File.ReadAllText(f);
        Assert.Contains("T=1", text);
        Assert.Contains("F=0", text);
        Assert.DoesNotContain("-1", text);
    }

    [Fact]
    public void WriteBool_与SysUtils_BoolToStr确实不同_差异断言()
    {
        // 两套语义落盘文本不同：TCustomIniFile 的整数约定（1/0）vs SysUtils.BoolToStr（-1/0）
        string f = Path2("d.ini");
        using (var ini = new TFastIniFile(f))
        {
            ini.WriteBool("S", "IniT", true);
            ini.WriteString("S", "SysUtilsT", DelphiRTL.BoolToStr(true));   // '-1'
            ini.Save();
        }

        using var reread = new TFastIniFile(f);
        Assert.Equal("1", reread.ReadString("S", "IniT", ""));
        Assert.Equal("-1", reread.ReadString("S", "SysUtilsT", ""));
        Assert.NotEqual(reread.ReadString("S", "IniT", ""), reread.ReadString("S", "SysUtilsT", ""));
        // 但两者都能被原文 ReadBool 读回 True（非 0 即真）
        Assert.True(reread.ReadBool("S", "IniT", false));
        Assert.True(reread.ReadBool("S", "SysUtilsT", false));
    }

    [Fact]
    public void WriteInteger_写十进制带负号()
    {
        string f = Path2("i.ini");
        using (var ini = new TFastIniFile(f))
        {
            ini.WriteInteger("S", "N", -7);
            ini.WriteInteger("S", "P", 42);
            ini.Save();
        }

        using var reread = new TFastIniFile(f);
        Assert.Equal("-7", reread.ReadString("S", "N", ""));
        Assert.Equal("-7", reread.ReadInteger("S", "N", 0).ToString());
        Assert.Equal(42, reread.ReadInteger("S", "P", 0));
    }

    // ============================ ReadBool：非 0 整数即真 ============================

    [Fact]
    public void ReadBool_非零整数皆为真_含负一()
    {
        string f = Path2("r.ini");
        File.WriteAllText(f, "[S]\r\nA=1\r\nB=-1\r\nC=2\r\nD=0\r\nE=abc\r\nF=True\r\n");

        using var ini = new TFastIniFile(f);
        Assert.True(ini.ReadBool("S", "A", false));
        Assert.True(ini.ReadBool("S", "B", false));   // 关键：'-1' 必须为真（旧实现读成假）
        Assert.True(ini.ReadBool("S", "C", false));   // '2' 也非 0
        Assert.False(ini.ReadBool("S", "D", true));

        // 非数字串：原文 StrToIntDef 回退 Default → 结果就是 Default 本身
        Assert.False(ini.ReadBool("S", "E", false));
        Assert.True(ini.ReadBool("S", "E", true));
        // 'True' 文本**不被识别**（这是与旧实现的第二处差异：旧实现无条件把 'True' 当真）
        Assert.False(ini.ReadBool("S", "F", false));
        Assert.True(ini.ReadBool("S", "F", true));

        // 缺键 → Default
        Assert.True(ini.ReadBool("S", "Missing", true));
        Assert.False(ini.ReadBool("S", "Missing", false));
    }

    [Fact]
    public void ReadBool_能读回SysUtils_BoolToStr落的负一_与旧实现差异断言()
    {
        // LoginSrv 的 TIniFile.WriteBool 走的是 SysUtils.BoolToStr → 落盘 '-1'（BasicSet.cs:741）
        string f = Path2("login.ini");
        using (var ini = new TFastIniFile(f))
        {
            ini.WriteString("Setup", "EnableMakingID", DelphiRTL.BoolToStr(true));
            ini.Save();
        }

        using var reread = new TFastIniFile(f);
        Assert.Equal("-1", reread.ReadString("Setup", "EnableMakingID", ""));
        Assert.True(reread.ReadBool("Setup", "EnableMakingID", false));   // 旧实现此处为 False
    }

    [Fact]
    public void ReadBool_写读往返一致()
    {
        string f = Path2("rt.ini");
        using (var ini = new TFastIniFile(f))
        {
            ini.WriteBool("S", "T", true);
            ini.WriteBool("S", "F", false);
            ini.Save();
        }

        using var reread = new TFastIniFile(f);
        Assert.True(reread.ReadBool("S", "T", false));
        Assert.False(reread.ReadBool("S", "F", true));
    }

    // ============================ ReadInteger：'0x'/'$' 十六进制（原文先把 0x 改写成 $） ============================

    [Fact]
    public void ReadInteger_支持美元号与0x十六进制_其它回退默认值()
    {
        string f = Path2("h.ini");
        File.WriteAllText(f, "[S]\r\nH=$1F\r\nH2=0X1f\r\nH3=0x1f\r\nBad=abc\r\nEmpty=\r\n");

        using var ini = new TFastIniFile(f);
        Assert.Equal(31, ini.ReadInteger("S", "H", 42));
        Assert.Equal(31, ini.ReadInteger("S", "H2", 42));
        Assert.Equal(31, ini.ReadInteger("S", "H3", 42));
        Assert.Equal(42, ini.ReadInteger("S", "Bad", 42));
        Assert.Equal(42, ini.ReadInteger("S", "Empty", 42));
        Assert.Equal(42, ini.ReadInteger("S", "None", 42));
    }
}
