using System;
using System.Collections.Generic;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uFrmAddProcessBlack.pas（107 行）的 6 条校验分支测试 + GateShare.IsHexString + 窗体 DFM 对齐。
/// 重点差异断言：
///   * 分支 2（进程名判空）用**未 Trim 的原始文本**，分支 3（MD5 判空）用 **Trim 后**；
///   * 分支 1 的上限是**硬编码 80**，不是 `g_ProcessBlackList.MaxCount`；
///   * 第 6 条来自 `TProcessBlacklist.Add` 的 nil 返回（MD5 重复）。
/// </summary>
[Collection("RunGateFormLane")]
public class RunGateUtilsFormAddProcessBlackTests
{
    public RunGateUtilsFormAddProcessBlackTests() => FormGlobals.ResetForTest();

    private static readonly Func<string, string, bool> NoDup = (n, m) => false;

    // ---------------- 分支 1：Count >= 80（原 :53-58）----------------

    [Fact]
    public void Validate_达到硬编码上限80被拒绝()
    {
        var r = AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 80, NoDup);
        Assert.Equal(AddProcessBlackResult.MaxCountReached, r);
        Assert.Equal("已经达到最多数量，无法添加", AddProcessBlackLogic.MsgMaxCount);
    }

    [Fact]
    public void Validate_上限用字面量80而不是MaxCount_边界79通过80拒绝()
    {
        var originalMax = FormGlobals.g_ProcessBlackList.MaxCount;
        try
        {
            Assert.Equal(AddProcessBlackResult.OK,
                AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 79, NoDup));
            Assert.Equal(AddProcessBlackResult.MaxCountReached,
                AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 80, NoDup));

            // ★ 差异断言：MaxCount 被改成 10 也不影响该分支（原文写死 80）
            FormGlobals.g_ProcessBlackList.MaxCountForTest = 10;
            Assert.Equal(AddProcessBlackResult.OK,
                AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 11, NoDup));
            Assert.Equal(AddProcessBlackResult.MaxCountReached,
                AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 80, NoDup));
        }
        finally
        {
            // 还原 FMaxCount，避免影响同类其它用例（ResetForTest 也会复位，这里再加一道保险）
            FormGlobals.g_ProcessBlackList.MaxCountForTest = originalMax;
        }
    }

    [Fact]
    public void Validate_上限优先于其它分支_即使进程名为空()
    {
        var r = AddProcessBlackLogic.Validate("", "", 80, NoDup);
        Assert.Equal(AddProcessBlackResult.MaxCountReached, r);
    }

    // ---------------- 分支 2：进程名判空（原 :60-65）----------------

    [Fact]
    public void Validate_进程名空串被拒绝()
    {
        var r = AddProcessBlackLogic.Validate("", new string('A', 32), 0, NoDup);
        Assert.Equal(AddProcessBlackResult.EmptyProcessName, r);
        Assert.Equal("进程名不能为空", AddProcessBlackLogic.MsgEmptyProcessName);
    }

    [Fact]
    public void Validate_进程名null被拒绝()
    {
        var r = AddProcessBlackLogic.Validate(null, new string('A', 32), 0, NoDup);
        Assert.Equal(AddProcessBlackResult.EmptyProcessName, r);
    }

    [Fact]
    public void Validate_进程名单个空格通过_不Trim()
    {
        // ★ 差异断言：原 :60 是 `Length(edtProcessName.Text) = 0`（**不 Trim**）
        //   → 单空格长度为 1，**通过**校验（与 MD5 的 Trim 后判空不同）。
        var r = AddProcessBlackLogic.Validate(" ", new string('A', 32), 0, NoDup);
        Assert.Equal(AddProcessBlackResult.OK, r);
    }

    // ---------------- 分支 3：MD5 判空（原 :67-73）----------------

    [Fact]
    public void Validate_MD5空串被拒绝()
    {
        var r = AddProcessBlackLogic.Validate("a.exe", "", 0, NoDup);
        Assert.Equal(AddProcessBlackResult.EmptyMD5, r);
        Assert.Equal("进程MD5不能为空", AddProcessBlackLogic.MsgEmptyMD5);
    }

    [Fact]
    public void Validate_MD5单个空格被拒绝_Trim后判空()
    {
        // ★ 与分支 2 的差异断言：同一个 " "，进程名通过、MD5 被拒。
        Assert.Equal(AddProcessBlackResult.OK,
            AddProcessBlackLogic.Validate(" ", new string('A', 32), 0, NoDup));
        Assert.Equal(AddProcessBlackResult.EmptyMD5,
            AddProcessBlackLogic.Validate("a.exe", " ", 0, NoDup));
    }

    [Fact]
    public void Validate_MD5前后空格被Trim后接受()
    {
        var r = AddProcessBlackLogic.Validate("a.exe", "  " + new string('A', 32) + "  ", 0, NoDup);
        Assert.Equal(AddProcessBlackResult.OK, r);
    }

    // ---------------- 分支 4：长度必须 32（原 :75-80）----------------

    [Fact]
    public void Validate_长度31与33都被拒绝_32通过()
    {
        Assert.Equal(AddProcessBlackResult.BadMD5Length,
            AddProcessBlackLogic.Validate("a.exe", new string('A', 31), 0, NoDup));
        Assert.Equal(AddProcessBlackResult.BadMD5Length,
            AddProcessBlackLogic.Validate("a.exe", new string('A', 33), 0, NoDup));
        Assert.Equal(AddProcessBlackResult.OK,
            AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 0, NoDup));
        Assert.Equal("进程MD5长度不对", AddProcessBlackLogic.MsgBadMD5Length);
    }

    [Fact]
    public void Validate_长度判定在字符集判定之前_31位非法字符报长度不对()
    {
        // 原 :75 在 :82 之前 → 先报长度
        var r = AddProcessBlackLogic.Validate("a.exe", new string('Z', 31), 0, NoDup);
        Assert.Equal(AddProcessBlackResult.BadMD5Length, r);
        // 32 位非法字符才报字符集
        Assert.Equal(AddProcessBlackResult.NonHexMD5,
            AddProcessBlackLogic.Validate("a.exe", new string('Z', 32), 0, NoDup));
    }

    // ---------------- 分支 5：IsHexString（原 :82-87）----------------

    [Fact]
    public void Validate_32位十六进制通过_大小写都接受()
    {
        Assert.Equal(AddProcessBlackResult.OK,
            AddProcessBlackLogic.Validate("a.exe", "0123456789ABCDEF0123456789abcdef", 0, NoDup));
    }

    [Fact]
    public void Validate_含非十六进制字符被拒绝()
    {
        // 'G' 不在 0-9A-Fa-f 内
        var r = AddProcessBlackLogic.Validate("a.exe", "0123456789ABCDEF0123456789abcdeG", 0, NoDup);
        Assert.Equal(AddProcessBlackResult.NonHexMD5, r);
        Assert.Equal("进程MD5包含非法的字符串", AddProcessBlackLogic.MsgNonHexMD5);
    }

    [Theory]
    [InlineData("0")]        // 数字
    [InlineData("F")]        // 大写
    [InlineData("f")]        // 小写
    [InlineData("aA0F")]
    [InlineData("")]         // 空串：原文 Result := True
    public void IsHexString_合法输入返回true(string s)
        => Assert.True(GateShareHelper.IsHexString(s));

    [Theory]
    [InlineData("g")]
    [InlineData("中")]
    [InlineData("0x12")]
    [InlineData("AB CD")]
    [InlineData("-1")]
    public void IsHexString_非法输入返回false(string s)
        => Assert.False(GateShareHelper.IsHexString(s));

    [Fact]
    public void IsHexString_null等价空串返回true()
        => Assert.True(GateShareHelper.IsHexString(null));

    // ---------------- 分支 6：重复 MD5（原 :96-101）----------------

    [Fact]
    public void Validate_重复MD5被拒绝()
    {
        var md5 = new string('A', 32);
        var r = AddProcessBlackLogic.Validate("a.exe", md5, 0, (n, m) => m == md5);
        Assert.Equal(AddProcessBlackResult.DuplicateMD5, r);
        Assert.Equal("进程MD5已经存在于黑名单中", AddProcessBlackLogic.MsgDuplicateMD5);
    }

    [Fact]
    public void Validate_重复判定在最后_前面任一分支失败都不会走到它()
    {
        bool called = false;
        Func<string, string, bool> probe = (n, m) => { called = true; return true; };

        Assert.Equal(AddProcessBlackResult.EmptyProcessName,
            AddProcessBlackLogic.Validate("", new string('A', 32), 0, probe));
        Assert.False(called);

        Assert.Equal(AddProcessBlackResult.BadMD5Length,
            AddProcessBlackLogic.Validate("a", "AAA", 0, probe));
        Assert.False(called);
    }

    [Fact]
    public void Validate_isDuplicate为null时视为不重复()
    {
        Assert.Equal(AddProcessBlackResult.OK,
            AddProcessBlackLogic.Validate("a.exe", new string('A', 32), 0, null));
    }

    // ---------------- TProcessBlacklist（GateShare.pas:3271-3300）----------------

    [Fact]
    public void ProcessBlackList_重复MD5返回null_不同MD5返回新项()
    {
        var list = new TProcessBlacklist();
        var a = list.Add("p1", new string('A', 32));
        Assert.NotNull(a);
        Assert.Equal(1, list.Count);

        var dup = list.Add("p2", new string('A', 32));       // MD5 相同 → nil
        Assert.Null(dup);
        Assert.Equal(1, list.Count);

        var b = list.Add("p2", new string('B', 32));
        Assert.NotNull(b);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void ProcessBlackList_MaxCount为80时第81项返回null()
    {
        var list = new TProcessBlacklist();
        for (int i = 0; i < 80; i++)
        {
            string md5 = i.ToString("X2").PadLeft(32, '0');
            Assert.NotNull(list.Add("p" + i, md5));
        }
        Assert.Equal(80, list.Count);
        Assert.Null(list.Add("p80", new string('F', 32)));   // 已满
    }

    [Fact]
    public void ProcessBlackList_Delete与Clear()
    {
        var list = new TProcessBlacklist();
        var a = list.Add("p1", new string('A', 32));
        list.Delete(a);
        Assert.Equal(0, list.Count);

        list.Add("p1", new string('A', 32));
        list.Add("p2", new string('B', 32));
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    // ---------------- 窗体（DFM 对齐 + 端到端分支）----------------

    [Fact]
    public void 窗体_DFM属性与控件名对齐()
    {
        using var f = new FrmAddProcessBlack();

        Assert.Equal("添加进程黑名单", f.Text);                   // DFM: Caption
        Assert.Equal(365, f.ClientSize.Width);                    // DFM: ClientWidth=365
        Assert.Equal(122, f.ClientSize.Height);                   // DFM: ClientHeight=122
        Assert.NotNull(f.grp1);
        Assert.NotNull(f.lbl1);
        Assert.NotNull(f.Label1);
        Assert.NotNull(f.edtProcessName);
        Assert.NotNull(f.edtProcessMD5);
        Assert.NotNull(f.btnOK);
        Assert.NotNull(f.btnCancel);
        Assert.Equal("进程信息", f.grp1.Text);
        Assert.Equal("进程名：", f.lbl1.Text);
        Assert.Equal("MD5值：", f.Label1.Text);
        Assert.Equal("确定", f.btnOK.Text);
        Assert.Equal("取消", f.btnCancel.Text);
        Assert.Equal(32, f.edtProcessMD5.MaxLength);              // DFM: MaxLength=32
        Assert.Equal(System.Windows.Forms.DialogResult.Cancel, f.btnCancel.DialogResult);   // DFM: ModalResult=2
        Assert.Null(f.FProcessInfo);
    }

    [Theory]
    [InlineData("", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 0, "进程名不能为空")]
    [InlineData("a.exe", "", 0, "进程MD5不能为空")]
    [InlineData("a.exe", "AAA", 0, "进程MD5长度不对")]
    [InlineData("a.exe", "0123456789ABCDEF0123456789abcdeG", 0, "进程MD5包含非法的字符串")]
    public void 窗体_btnOK_Click各分支弹窗且不置DialogResult(string name, string md5, int preCount, string expectedText)
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();
            for (int i = 0; i < preCount; i++) list.Add("x" + i, i.ToString("X2").PadLeft(32, '0'));

            using var f = new FrmAddProcessBlack();
            f.edtProcessName.Text = name;
            f.edtProcessMD5.Text = md5;
            f.btnOK_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal(expectedText, shown[0]);
            Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);   // 无分支置 mrOK
            Assert.Null(f.FProcessInfo);
            Assert.Equal(preCount, list.Count);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_上限分支弹窗_已经在黑名单80项时()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();
            // ★ 必须复位 FMaxCount：原 GateShare.pas:3271 `FMaxCount := 80`，
            //   但本类别的 `Validate_上限用字面量80而不是MaxCount_...` 用例会把它改成 10；
            //   若不复位，这里 `list.Add` 会在第 10 项后静默返回 nil（原 :3296），
            //   于是 Count < 80 → 走不到"上限"分支而落到"MD5 已存在"分支。
            list.MaxCountForTest = AddProcessBlackLogic.HardCodedMaxCount;
            for (int i = 0; i < 80; i++) list.Add("p" + i, i.ToString("X2").PadLeft(32, '0'));
            Assert.Equal(80, list.Count);   // 前提校验：确实加满了

            using var f = new FrmAddProcessBlack();
            f.edtProcessName.Text = "ok.exe";
            f.edtProcessMD5.Text = new string('F', 32);
            f.btnOK_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal(AddProcessBlackLogic.MsgMaxCount, shown[0]);
            Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_重复MD5分支_Add返回null时弹窗()
    {
        var shown = new List<string>();
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => { shown.Add(t); return System.Windows.Forms.DialogResult.OK; };

            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();
            list.Add("first.exe", new string('A', 32));

            using var f = new FrmAddProcessBlack();
            f.edtProcessName.Text = "second.exe";
            f.edtProcessMD5.Text = new string('A', 32);          // 与已存在项同 MD5
            f.btnOK_Click(f, EventArgs.Empty);

            Assert.Single(shown);
            Assert.Equal(AddProcessBlackLogic.MsgDuplicateMD5, shown[0]);
            Assert.Equal(System.Windows.Forms.DialogResult.None, f.DialogResult);
            Assert.Null(f.FProcessInfo);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_全部通过时加入列表并置DialogResult与FProcessInfo()
    {
        var original = MessageBoxSeam.Show;
        try
        {
            int popups = 0;
            MessageBoxSeam.Show = (t, c, b, i) => { popups++; return System.Windows.Forms.DialogResult.OK; };

            var list = FormGlobals.g_ProcessBlackList;
            list.Clear();

            using var f = new FrmAddProcessBlack();
            f.edtProcessName.Text = "cheat.exe";
            f.edtProcessMD5.Text = " 0123456789abcdef0123456789ABCDEF ";   // 前后空格会被 Trim
            f.btnOK_Click(f, EventArgs.Empty);

            Assert.Equal(0, popups);
            Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);
            Assert.NotNull(f.FProcessInfo);
            Assert.Equal("cheat.exe", f.FProcessInfo.ProcessName);                  // 进程名不 Trim
            // ★ 更正（本轮 TProcessBlacklist 名字/语义对齐）：原文 `GateShare.pas:3301` 在 Add 内部执行
//   `Result.ProcessMD5 := UpperCase(ProcessMD5)`，而 `uFrmAddProcessBlack.pas:91` 把 Add 的返回值
//   直接赋给 `FProcessInfo` —— 故入库后的 MD5 一定是**大写**。旧接缝漏了 UpperCase，本条期望随之错误。
Assert.Equal("0123456789ABCDEF0123456789ABCDEF", f.FProcessInfo.ProcessMD5);   // MD5 Trim 后 + UpperCase
            Assert.Equal(1, list.Count);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }

    [Fact]
    public void 窗体_进程名带空格时原样入库_MD5则被Trim()
    {
        var original = MessageBoxSeam.Show;
        try
        {
            MessageBoxSeam.Show = (t, c, b, i) => System.Windows.Forms.DialogResult.OK;
            FormGlobals.g_ProcessBlackList.Clear();

            using var f = new FrmAddProcessBlack();
            f.edtProcessName.Text = "  spaced.exe  ";      // 不 Trim
            f.edtProcessMD5.Text = new string('A', 32);
            f.btnOK_Click(f, EventArgs.Empty);

            Assert.Equal("  spaced.exe  ", f.FProcessInfo.ProcessName);
        }
        finally
        {
            MessageBoxSeam.Show = original;
        }
    }
}
