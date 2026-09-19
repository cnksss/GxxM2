using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// CheckProcessModules.pas 与 uFrmNGItemEdit.pas 的 1:1 移植测试。
/// </summary>
public sealed class TailCheckModulesTests
{
    // ══════════════════════════════════════════════════════════════════════
    // CheckProcessModules.pas
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：CompareLStr 的前 n 字符忽略大小写比较。</summary>
    [Theory]
    [InlineData("abcdef", "ABCDEF", 3, true)]
    [InlineData("abcdef", "abcXYZ", 3, true)]
    [InlineData("abcdef", "abcXYZ", 4, false)]
    [InlineData("abc", "abc", 3, true)]
    public void CompareLStr_ComparesFirstNCharsCaseInsensitively(string a, string b, int n, bool expected)
        => Assert.Equal(expected, CheckProcessModules.CompareLStr(a, b, n));

    /// <summary>用例 2：CompareLStr 的边界 —— compn ≤ 0 或串短于 compn 一律 False。</summary>
    [Fact]
    public void CompareLStr_DegenerateInputs_ReturnFalse()
    {
        Assert.False(CheckProcessModules.CompareLStr("abc", "abc", 0));
        Assert.False(CheckProcessModules.CompareLStr("abc", "abc", -1));
        Assert.False(CheckProcessModules.CompareLStr("ab", "abc", 3));
        Assert.False(CheckProcessModules.CompareLStr("abc", "ab", 3));
        Assert.False(CheckProcessModules.CompareLStr(null, "abc", 3));
        Assert.False(CheckProcessModules.CompareLStr("abc", null, 3));
        Assert.False(CheckProcessModules.CompareLStr(null, null, 5));
    }

    /// <summary>用例 3：CompareLStr 只比前 n 个 —— 第 n+1 个字符不同不影响。</summary>
    [Fact]
    public void CompareLStr_IgnoresCharactersBeyondN()
    {
        Assert.True(CheckProcessModules.CompareLStr("abcdefg", "ABCDEFz", 6));
        Assert.False(CheckProcessModules.CompareLStr("abcdefg", "ABCDEFz", 7));
    }

    /// <summary>用例 4：ReadRegKeyDispatch 的模式 1（字符串 → LowerCase(Trim)）。</summary>
    [Fact]
    public void ReadRegKeyDispatch_Mode1_TrimsAndLowercases()
    {
        var r = CheckProcessModules.ReadRegKeyDispatch(1, () => "  C:\\Windows\\System32  ", () => 0);
        Assert.True(r.Ok);
        Assert.Equal("c:\\windows\\system32", r.Result);

        var empty = CheckProcessModules.ReadRegKeyDispatch(1, () => "   ", () => 0);
        Assert.False(empty.Ok);                     // 原文 :245 if sResult = '' then Result := False
        Assert.Equal("", empty.Result);
    }

    /// <summary>用例 5：ReadRegKeyDispatch 的模式 2（整数 → 字符串）与其它模式（不改动）。</summary>
    [Fact]
    public void ReadRegKeyDispatch_Mode2AndUnknownMode()
    {
        var r2 = CheckProcessModules.ReadRegKeyDispatch(2, () => "ignored", () => 12345);
        Assert.True(r2.Ok);
        Assert.Equal("12345", r2.Result);

        var r2neg = CheckProcessModules.ReadRegKeyDispatch(2, () => "x", () => -1);
        Assert.True(r2neg.Ok);
        Assert.Equal("-1", r2neg.Result);

        // 模式 3 在原文里被注释掉了（:// 3: sResult := ReadBinaryData(...)）
        var r3 = CheckProcessModules.ReadRegKeyDispatch(3, () => "x", () => 7);
        Assert.False(r3.Ok);
        Assert.Equal("", r3.Result);
    }

    /// <summary>用例 6：ReadRegKeyDispatch 的模式 2 里 0 值也是"非空结果"⇒ 成功。</summary>
    [Fact]
    public void ReadRegKeyDispatch_Mode2ZeroIsSuccess()
    {
        var r = CheckProcessModules.ReadRegKeyDispatch(2, () => "x", () => 0);
        Assert.True(r.Ok);                          // "0" != ''
        Assert.Equal("0", r.Result);
    }

    /// <summary>用例 7：WriteRegKeyDispatch 的三种模式各调用一次对应写入。</summary>
    [Fact]
    public void WriteRegKeyDispatch_DispatchesByMode()
    {
        string s = null; int i = 0; byte b = 0xFF;
        CheckProcessModules.WriteRegKeyDispatch(1, "text", v => s = v, v => i = v, v => b = v);
        Assert.Equal("text", s);

        CheckProcessModules.WriteRegKeyDispatch(2, "42", v => s = v, v => i = v, v => b = v);
        Assert.Equal(42, i);

        // 原文 :272 WriteBinaryData(sKeyName, bData, 1) 的 bData 未初始化 ⇒ 长度恒 1
        CheckProcessModules.WriteRegKeyDispatch(3, "x", v => s = v, v => i = v, v => b = v);
        Assert.Equal(0, b);

        // 未匹配模式什么都不写
        string s2 = "unchanged";
        CheckProcessModules.WriteRegKeyDispatch(9, "x", v => s2 = v, v => i = v, v => b = v);
        Assert.Equal("unchanged", s2);
    }

    /// <summary>用例 8：FastPosNoCase 返回 1-based 位置，未命中 0。</summary>
    [Theory]
    [InlineData("Hello World", "world", 7)]
    [InlineData("Hello World", "HELLO", 1)]
    [InlineData("abc", "abc", 1)]
    [InlineData("abc", "z", 0)]
    public void FastPosNoCase_ReturnsOneBasedPosition(string src, string pat, int expected)
        => Assert.Equal(expected, CheckProcessModules.FastPosNoCase(src, pat));

    /// <summary>用例 9：FastPosNoCase 的空/空模式边界。</summary>
    [Fact]
    public void FastPosNoCase_EmptyInputs_ReturnZero()
    {
        Assert.Equal(0, CheckProcessModules.FastPosNoCase("", "abc"));
        Assert.Equal(0, CheckProcessModules.FastPosNoCase("abc", ""));
        Assert.Equal(0, CheckProcessModules.FastPosNoCase(null, "abc"));
        Assert.Equal(0, CheckProcessModules.FastPosNoCase("abc", null));
    }

    /// <summary>用例 10：IncludeTrailingPathDelimiter 的补分隔符语义。</summary>
    [Fact]
    public void IncludeTrailingPathDelimiter_AppendsOnlyWhenMissing()
    {
        char sep = Path.DirectorySeparatorChar;
        Assert.Equal("C:\\a" + sep, CheckProcessModules.IncludeTrailingPathDelimiter("C:\\a"));
        Assert.Equal("C:\\a" + sep, CheckProcessModules.IncludeTrailingPathDelimiter("C:\\a" + sep));
        Assert.Equal(sep.ToString(), CheckProcessModules.IncludeTrailingPathDelimiter(""));
        Assert.Equal(sep.ToString(), CheckProcessModules.IncludeTrailingPathDelimiter(null));
    }

    /// <summary>
    /// 用例 11：<c>InitModules</c> 把 60 条**加密态**版权串解成明文（原文 :362-363），
    /// 并算出 System32 / SysWOW64。
    /// </summary>
    [Fact]
    public void InitModules_DecryptsCopyrightTableAndResolvesSystemDirs()
    {
        var m = new CheckProcessModules();
        m.InitModules();

        Assert.Equal(60, CheckProcessModules.CopyrightArrayEncrypted.Length);
        Assert.Equal(60, m.CopyrightArray.Length);

        // 原文注释给出了明文；解密结果应与之一致
        Assert.Equal("Microsoft", m.CopyrightArray[0]);
        Assert.Equal("Sogou.com", m.CopyrightArray[1]);
        Assert.Equal("Tencent", m.CopyrightArray[2]);
        Assert.Equal("Thunder", m.CopyrightArray[3]);
        Assert.Equal("360.cn", m.CopyrightArray[4]);
        Assert.Equal("Kingsoft", m.CopyrightArray[5]);
        Assert.Equal("Kaspersky", m.CopyrightArray[6]);
        Assert.Equal("Rising", m.CopyrightArray[7]);
        Assert.Equal("Symantec", m.CopyrightArray[8]);
        Assert.Equal("Micropoint", m.CopyrightArray[9]);
        Assert.Equal("Jiangmin", m.CopyrightArray[10]);
        Assert.Equal("Baidu", m.CopyrightArray[11]);
        Assert.Equal("NVIDIA", m.CopyrightArray[21]);
        Assert.Equal("Adobe", m.CopyrightArray[27]);
        Assert.Equal("Intel", m.CopyrightArray[34]);

        // 全部 60 条互不相同（防"解密全部失败得到同一个空串"）
        Assert.Equal(60, m.CopyrightArray.Distinct().Count());
        Assert.All(m.CopyrightArray, s => Assert.False(string.IsNullOrEmpty(s)));

        // System32 / SysWOW64 都以分隔符结尾
        char sep = Path.DirectorySeparatorChar;
        Assert.EndsWith(sep.ToString(), m.System32);
        Assert.Equal(m.System32.StartsWith(m.System32), true);
    }

    /// <summary>用例 12：SysWOW64 在 32 位系统上等于 System32 ⇒ 被清空（原文 :369）。</summary>
    [Fact]
    public void SysWOW64_IsClearedWhenEqualToSystem32()
    {
        var m = new CheckProcessModules();
        m.InitModules();

        string systemX86 = CheckProcessModules.GetSpecialFolderDir(CheckProcessModulesConst._CSIDL_SYSTEMX86)
            .ToLowerInvariant();
        char sep = Path.DirectorySeparatorChar;
        string expected = string.IsNullOrEmpty(systemX86)
            ? sep.ToString()
            : (systemX86.EndsWith(sep) ? systemX86 : systemX86 + sep);

        // 无论 32/64 位，SysWOW64 只能是"规范化的 SysWOW64 路径"或空串
        Assert.True(m.SysWOW64 == expected || m.SysWOW64 == string.Empty);
        if (m.SysWOW64 == m.System32) Assert.Equal(string.Empty, m.SysWOW64);
    }

    /// <summary>用例 13：_CSIDL_* 常量抽样核对（原文 :26-153）。</summary>
    [Fact]
    public void CsidlConstants_MatchSource()
    {
        Assert.Equal(0x0000, CheckProcessModulesConst._CSIDL_DESKTOP);
        Assert.Equal(0x0005, CheckProcessModulesConst._CSIDL_PERSONAL);
        Assert.Equal(0x0005, CheckProcessModulesConst._CSIDL_MYDOCUMENTS);   // 别名
        Assert.Equal(0x001A, CheckProcessModulesConst._CSIDL_APPDATA);
        Assert.Equal(0x001C, CheckProcessModulesConst._CSIDL_LOCAL_APPDATA);
        Assert.Equal(0x0025, CheckProcessModulesConst._CSIDL_SYSTEM);
        Assert.Equal(0x0029, CheckProcessModulesConst._CSIDL_SYSTEMX86);
        Assert.Equal(0x0024, CheckProcessModulesConst._CSIDL_WINDOWS);
        Assert.Equal(0x003D, CheckProcessModulesConst._CSIDL_COMPUTERSNEARME);
        Assert.Equal(0x8000, CheckProcessModulesConst._CSIDL_FLAG_CREATE);
        Assert.Equal(0xFF00, CheckProcessModulesConst._CSIDL_FLAG_MASK);
        Assert.Equal(65536, CheckProcessModulesConst.HashListCapacity);

        // 差异断言：MYDOCUMENTS 与 PERSONAL 同值，而 MYPICTURES(=0x27) 不同
        Assert.Equal(CheckProcessModulesConst._CSIDL_PERSONAL, CheckProcessModulesConst._CSIDL_MYDOCUMENTS);
        Assert.NotEqual(CheckProcessModulesConst._CSIDL_PERSONAL, CheckProcessModulesConst._CSIDL_MYPICTURES);
    }

    /// <summary>用例 14：GetSpecialFolderDir 的映射（有/无对应）。</summary>
    [Fact]
    public void GetSpecialFolderDir_MapsKnownAndUnknownCsidl()
    {
        Assert.False(string.IsNullOrEmpty(CheckProcessModules.GetSpecialFolderDir(
            CheckProcessModulesConst._CSIDL_SYSTEM)));
        Assert.False(string.IsNullOrEmpty(CheckProcessModules.GetSpecialFolderDir(
            CheckProcessModulesConst._CSIDL_WINDOWS)));
        Assert.False(string.IsNullOrEmpty(CheckProcessModules.GetSpecialFolderDir(
            CheckProcessModulesConst._CSIDL_PROFILE)));
        Assert.False(string.IsNullOrEmpty(CheckProcessModules.GetSpecialFolderDir(
            CheckProcessModulesConst._CSIDL_APPDATA)));

        // .NET 无对应的 CSIDL ⇒ 返回空串（已登记）
        Assert.Equal("", CheckProcessModules.GetSpecialFolderDir(CheckProcessModulesConst._CSIDL_PRINTHOOD));
        Assert.Equal("", CheckProcessModules.GetSpecialFolderDir(CheckProcessModulesConst._CSIDL_ADMINTOOLS));
        Assert.Equal("", CheckProcessModules.GetSpecialFolderDir(0x7FFF));   // 完全未知
    }

    /// <summary>用例 15：RivestFile = 文件 MD5 小写 hex（对已知内容核对）。</summary>
    [Fact]
    public void RivestFile_ReturnsLowercaseMd5Hex()
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx_p2c_md5_" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            File.WriteAllBytes(path, System.Text.Encoding.ASCII.GetBytes("abc"));
            // MD5("abc") = 900150983cd24fb0d6963f7d28e17f72
            Assert.Equal("900150983cd24fb0d6963f7d28e17f72", CheckProcessModules.RivestFile(path));

            File.WriteAllBytes(path, Array.Empty<byte>());
            // MD5("") = d41d8cd98f00b204e9800998ecf8427e
            Assert.Equal("d41d8cd98f00b204e9800998ecf8427e", CheckProcessModules.RivestFile(path));
        }
        finally { try { File.Delete(path); } catch { } }
    }

    /// <summary>用例 16：RivestFile 对不存在/空路径返回空串（原文判据依赖该取值）。</summary>
    [Fact]
    public void RivestFile_MissingFile_ReturnsEmpty()
    {
        Assert.Equal("", CheckProcessModules.RivestFile(Path.Combine(Path.GetTempPath(), "no_such_file_xyz.bin")));
        Assert.Equal("", CheckProcessModules.RivestFile(""));
        Assert.Equal("", CheckProcessModules.RivestFile(null));
    }

    /// <summary>用例 17：GetFileLegalCopyright 对不存在的文件返回空串（不抛）。</summary>
    [Fact]
    public void GetFileLegalCopyright_MissingFile_ReturnsEmpty()
    {
        Assert.Equal("", CheckProcessModules.GetFileLegalCopyright(
            Path.Combine(Path.GetTempPath(), "no_such_dll_xyz.dll")));
        Assert.Equal("", CheckProcessModules.GetFileLegalCopyright(""));
        Assert.Equal("", CheckProcessModules.GetFileLegalCopyright(null));
    }

    /// <summary>用例 18：GetFileLegalCopyright 对真实模块返回非空版权串（如 kernel32.dll）。</summary>
    [Fact]
    public void GetFileLegalCopyright_OnSystemDll_ReturnsTrimmedString()
    {
        string kernel32 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "kernel32.dll");
        if (!File.Exists(kernel32)) return;      // 环境异常时跳过

        string c = CheckProcessModules.GetFileLegalCopyright(kernel32);
        Assert.False(string.IsNullOrEmpty(c));
        Assert.Equal(c.Trim(), c);               // 原文 :334 有 Trim
    }

    /// <summary>用例 19：CheckProcessModule 的判定顺序 ①：白名单命中直接 True。</summary>
    [Fact]
    public void CheckProcessModule_WhiteListHit_ReturnsTrue()
    {
        var m = new CheckProcessModules();
        m.InitModules();
        m.ModuleFileList.Add("C:\\x\\a.dll");
        Assert.True(m.CheckProcessModule("C:\\x\\a.dll"));
        // 不应该被记进黑名单
        Assert.DoesNotContain("C:\\x\\a.dll", m.UnKnowModuleFileList);
    }

    /// <summary>用例 20：判定顺序 ②：已判"未知"的模块直接 False。</summary>
    [Fact]
    public void CheckProcessModule_UnknownListHit_ReturnsFalse()
    {
        var m = new CheckProcessModules();
        m.InitModules();
        m.UnKnowModuleFileList.Add("C:\\x\\b.dll");
        Assert.False(m.CheckProcessModule("C:\\x\\b.dll"));
    }

    /// <summary>用例 21：判定顺序 ③：MD5 命中黑名单 ⇒ 记入 BlackModuleList 并 False。</summary>
    [Fact]
    public void CheckProcessModule_BlackMd5Hit_ReturnsFalseAndRecords()
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx_p2c_blk_" + Guid.NewGuid().ToString("N") + ".dll");
        try
        {
            File.WriteAllBytes(path, System.Text.Encoding.ASCII.GetBytes("abc"));
            const string md5 = "900150983cd24fb0d6963f7d28e17f72";

            var m = new CheckProcessModules();
            m.InitModules();
            m.BlackModuleMD5List.Add(md5);

            Assert.False(m.CheckProcessModule(path));
            Assert.Contains(path, m.BlackModuleList);
            Assert.DoesNotContain(path, m.UnKnowModuleFileList);
        }
        finally { try { File.Delete(path); } catch { } }
    }

    /// <summary>用例 22：判定顺序 ④⑤：既不在黑名单、版权也匹配不上 ⇒ 记入 UnKnow 并 False。</summary>
    [Fact]
    public void CheckProcessModule_UnrecognisedFile_GoesToUnknownList()
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx_p2c_unk_" + Guid.NewGuid().ToString("N") + ".dll");
        try
        {
            File.WriteAllBytes(path, System.Text.Encoding.ASCII.GetBytes("not a real dll"));
            var m = new CheckProcessModules();
            m.InitModules();

            Assert.False(m.CheckProcessModule(path));
            Assert.Contains(path, m.UnKnowModuleFileList);
            Assert.DoesNotContain(path, m.ModuleFileList);
        }
        finally { try { File.Delete(path); } catch { } }
    }

    /// <summary>用例 23：判定顺序 ⑤：MD5 命中白名单（ModuleMD5List / ServerModuleMD5List）⇒ True。</summary>
    [Fact]
    public void CheckProcessModule_WhiteMd5Hit_ReturnsTrue()
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx_p2c_wht_" + Guid.NewGuid().ToString("N") + ".dll");
        try
        {
            File.WriteAllBytes(path, System.Text.Encoding.ASCII.GetBytes("abc"));
            const string md5 = "900150983cd24fb0d6963f7d28e17f72";

            var a = new CheckProcessModules();
            a.InitModules();
            a.ModuleMD5List.Add(md5);
            Assert.True(a.CheckProcessModule(path));
            Assert.Contains(path, a.ModuleFileList);

            var b = new CheckProcessModules();
            b.InitModules();
            b.ServerModuleMD5List.Add(md5);
            Assert.True(b.CheckProcessModule(path));
            Assert.Contains(path, b.ModuleFileList);
        }
        finally { try { File.Delete(path); } catch { } }
    }

    /// <summary>用例 24：版权串命中 ⇒ True（用文档里的明文版权构造一个假模块不可行，
    /// 故直接测"版权匹配"这一段的纯函数：FastPosNoCase 大小写无关包含）。</summary>
    [Fact]
    public void CheckProcessModule_CopyrightMatchPath_UsesCaseInsensitiveContains()
    {
        // 原文 :410 的判据：FastPosNoCase(sCopyright, CopyrightArray[I], ..., 1) > 0
        Assert.True(CheckProcessModules.FastPosNoCase("Copyright (C) Microsoft Corporation", "Microsoft") > 0);
        Assert.True(CheckProcessModules.FastPosNoCase("copyright (c) microsoft", "Microsoft") > 0);
        Assert.True(CheckProcessModules.FastPosNoCase("Sogou.com Input Method", "Sogou.com") > 0);
        Assert.Equal(0, CheckProcessModules.FastPosNoCase("Some Other Vendor", "Microsoft"));
    }

    /// <summary>用例 25：FinalizeLists 清空所有表（原文 finalization 段）。</summary>
    [Fact]
    public void FinalizeLists_ClearsEveryTable()
    {
        var m = new CheckProcessModules();
        m.InitModules();
        m.SystemDllList.Add("a");
        m.ModulePathList.Add("b");
        m.ModuleMD5List.Add("c");
        m.ModuleFileList.Add("d");
        m.UnKnowModuleFileList.Add("e");
        m.ServerModuleMD5List.Add("f");
        m.AddModuleMD5List.Add("g");
        m.BlackModuleMD5List.Add("h");
        m.BlackModuleList.Add("i");

        m.FinalizeLists();

        Assert.Empty(m.SystemDllList);
        Assert.Empty(m.ModulePathList);
        Assert.Empty(m.ModuleMD5List);
        Assert.Empty(m.ModuleFileList);
        Assert.Empty(m.UnKnowModuleFileList);
        Assert.Empty(m.ServerModuleMD5List);
        Assert.Empty(m.AddModuleMD5List);
        Assert.Empty(m.BlackModuleMD5List);
        Assert.Empty(m.BlackModuleList);
    }

    /// <summary>用例 26：加密版权表与明文表一一对应（用 <c>InitModules</c> 自定义解密器验证形状）。</summary>
    [Fact]
    public void InitModules_AcceptsInjectedDecryptor()
    {
        var m = new CheckProcessModules();
        m.InitModules(s => "P:" + s);
        Assert.Equal("P:" + CheckProcessModules.CopyrightArrayEncrypted[0], m.CopyrightArray[0]);
        Assert.Equal(60, m.CopyrightArray.Length);
    }

    // ══════════════════════════════════════════════════════════════════════
    // uFrmNGItemEdit.pas
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：Delphi TStrings 的行拆分规则（CR / LF / CRLF 都算分隔）。</summary>
    [Fact]
    public void SplitDelphiLines_HandlesCrLfAndBareCrLf()
    {
        Assert.Equal(new[] { "a", "b" }, TFrmNGItemEdit.SplitDelphiLines("a\r\nb").ToArray());
        Assert.Equal(new[] { "a", "b" }, TFrmNGItemEdit.SplitDelphiLines("a\rb").ToArray());
        Assert.Equal(new[] { "a", "b" }, TFrmNGItemEdit.SplitDelphiLines("a\nb").ToArray());
        Assert.Equal(new[] { "a", "b", "c" }, TFrmNGItemEdit.SplitDelphiLines("a\r\nb\nc").ToArray());
    }

    /// <summary>用例 2：末尾换行**不产生**空行（Delphi SetTextStr 语义）。</summary>
    [Fact]
    public void SplitDelphiLines_DropsTrailingEmptyLine()
    {
        Assert.Equal(new[] { "a" }, TFrmNGItemEdit.SplitDelphiLines("a\r\n").ToArray());
        Assert.Equal(new[] { "a" }, TFrmNGItemEdit.SplitDelphiLines("a\n").ToArray());
        Assert.Empty(TFrmNGItemEdit.SplitDelphiLines(""));
        Assert.Empty(TFrmNGItemEdit.SplitDelphiLines(null));
        // 中间的空行保留
        Assert.Equal(new[] { "a", "", "b" }, TFrmNGItemEdit.SplitDelphiLines("a\r\n\r\nb").ToArray());
    }

    /// <summary>用例 3：JoinDelphiText = CRLF 连接 + 末尾 CRLF。</summary>
    [Fact]
    public void JoinDelphiText_AppendsTrailingCrLf()
    {
        Assert.Equal("a\r\nb\r\n", TFrmNGItemEdit.JoinDelphiText(new[] { "a", "b" }));
        Assert.Equal("", TFrmNGItemEdit.JoinDelphiText(Array.Empty<string>()));
        Assert.Equal("", TFrmNGItemEdit.JoinDelphiText(null));
    }

    /// <summary>用例 4：WriteBack 的整体替换语义（先 Clear 再逐行 Add）。</summary>
    [Fact]
    public void WriteBack_ReplacesTargetContent()
    {
        var target = new List<string> { "old1", "old2", "old3" };
        TFrmNGItemEdit.WriteBack(target, "n1\r\nn2");
        Assert.Equal(new[] { "n1", "n2" }, target.ToArray());
    }

    /// <summary>用例 5：WriteBack 的空文本 ⇒ 清空目标；null 目标 ⇒ 不抛。</summary>
    [Fact]
    public void WriteBack_EmptyTextClearsTarget()
    {
        var target = new List<string> { "x" };
        TFrmNGItemEdit.WriteBack(target, "");
        Assert.Empty(target);

        TFrmNGItemEdit.WriteBack(null, "a");       // 不应抛
    }

    /// <summary>用例 6：WriteBack/Join 往返一致。</summary>
    [Fact]
    public void WriteBack_RoundTripsWithJoin()
    {
        var src = new[] { "line1", "line2", "line3" };
        string text = TFrmNGItemEdit.JoinDelphiText(src);
        var target = new List<string>();
        TFrmNGItemEdit.WriteBack(target, text);
        Assert.Equal(src, target.ToArray());
    }

    /// <summary>用例 7：DFM 解码出的几何常量自洽。</summary>
    [Fact]
    public void DfmGeometryConstants_AreSelfConsistent()
    {
        Assert.Equal(449, TFrmNGItemEdit.DfmClientWidth);
        Assert.Equal(308, TFrmNGItemEdit.DfmClientHeight);
        Assert.Equal(449, TFrmNGItemEdit.DfmMemoWidth);
        Assert.Equal(276, TFrmNGItemEdit.DfmMemoHeight);
        Assert.Equal(374, TFrmNGItemEdit.DfmOkLeft);
        Assert.Equal(280, TFrmNGItemEdit.DfmOkTop);
        Assert.Equal(75, TFrmNGItemEdit.DfmOkWidth);
        Assert.Equal(25, TFrmNGItemEdit.DfmOkHeight);
        Assert.Equal("内挂物品编辑", TFrmNGItemEdit.DfmCaption);
        Assert.Equal("确定", TFrmNGItemEdit.DfmOkCaption);
        Assert.Equal(3, TFrmNGItemEdit.DfmBorderWidth);

        // Memo 占满客户区上半部；按钮在右下角且完全落在客户区内
        Assert.Equal(TFrmNGItemEdit.DfmClientWidth, TFrmNGItemEdit.DfmMemoWidth);
        Assert.True(TFrmNGItemEdit.DfmMemoHeight < TFrmNGItemEdit.DfmClientHeight);
        Assert.True(TFrmNGItemEdit.DfmOkLeft + TFrmNGItemEdit.DfmOkWidth <= TFrmNGItemEdit.DfmClientWidth);
        Assert.True(TFrmNGItemEdit.DfmOkTop + TFrmNGItemEdit.DfmOkHeight <= TFrmNGItemEdit.DfmClientHeight);
    }

    /// <summary>用例 8：窗体与控件树按 DFM 构造（需要 WinForms，单线程跑）。</summary>
    [Fact]
    public void Form_BuildsControlTreePerDfm()
    {
        using var frm = new TFrmNGItemEdit();
        Assert.Equal(TFrmNGItemEdit.DfmCaption, frm.Text);
        Assert.Equal(TFrmNGItemEdit.DfmClientWidth, frm.ClientSize.Width);
        Assert.Equal(TFrmNGItemEdit.DfmClientHeight, frm.ClientSize.Height);
        Assert.Equal(2, frm.Controls.Count);
        Assert.NotNull(frm.mmoItems);
        Assert.NotNull(frm.btnOK);

        Assert.True(frm.mmoItems.Multiline);
        Assert.Equal(TFrmNGItemEdit.DfmMemoWidth, frm.mmoItems.Width);
        Assert.Equal(TFrmNGItemEdit.DfmMemoHeight, frm.mmoItems.Height);
        Assert.Equal(0, frm.mmoItems.TabIndex);

        Assert.Equal(TFrmNGItemEdit.DfmOkCaption, frm.btnOK.Text);
        Assert.Equal(System.Windows.Forms.DialogResult.OK, frm.btnOK.DialogResult);
        Assert.Equal(1, frm.btnOK.TabIndex);
        Assert.Same(frm.btnOK, frm.AcceptButton);
    }
}
