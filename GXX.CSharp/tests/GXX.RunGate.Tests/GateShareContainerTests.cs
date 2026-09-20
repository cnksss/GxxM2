// 测试：Source\RunGate\GateShare.pas（实测 LF 3595）的**地址/容器/全局量**族
//   → src/GXX.RunGate/GateShareAddressUtils.cs / GateShareContainers.cs / GateShareGlobals.cs
//
// 覆盖策略：每个公开成员 ≥3 用例（空 / 0 / 负 / 越界 / 异常路径）；
// 原文缺陷一律写成**差异断言**并标注 `原文缺陷 n`（编号与对应 .cs 文件头一致）。
using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

[Collection("RunGateFormLane")]      // 与窗体测试串行：本文件的用例会读/写 GateShareGlobals 与 FormGlobals 静态全局量
public sealed class GateShareAddressTests
{
    // ================= GateShareInet.InetAddr / InetNtoa（原 :2639 / :2989） =================

    [Fact]
    public void InetAddr_原文真值_202_103_100_1()
    {
        // 原 :2981-2982 注释给的真值：inet_addr(202.103.100.1) = 23357386 ($016467CA)
        Assert.Equal(23357386u, GateShareInet.InetAddr("202.103.100.1"));
        Assert.Equal(0x016467CAu, GateShareInet.InetAddr("202.103.100.1"));
    }

    [Fact]
    public void InetAddr_边界值_0_0_0_0与255_255_255_255()
    {
        Assert.Equal(0x00000000u, GateShareInet.InetAddr("0.0.0.0"));
        Assert.Equal(0xFFFFFFFFu, GateShareInet.InetAddr("255.255.255.255"));
        // ★ 交接纪律提到的撞车点：0xFFFFFFFF 既是 inet_addr 的失败值，也是合法广播地址
        Assert.Equal(GateShareInet.INADDR_NONE, GateShareInet.InetAddr("255.255.255.255"));
        Assert.Equal(GateShareInet.INADDR_NONE, GateShareInet.InetAddr("not-an-ip"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4.5")]
    [InlineData("1.2.3.")]
    [InlineData(".2.3.4")]
    [InlineData("1.2.3.256")]
    [InlineData("1.2.3.-1")]
    [InlineData("1.2.3.a")]
    public void InetAddr_非法输入返回INADDR_NONE(string ip)
    {
        Assert.Equal(GateShareInet.INADDR_NONE, GateShareInet.InetAddr(ip));
    }

    [Fact]
    public void InetAddr_首尾空白被忽略()
    {
        Assert.Equal(GateShareInet.InetAddr("1.2.3.4"), GateShareInet.InetAddr("  1.2.3.4  "));
    }

    [Fact]
    public void InetAddr_前导零按十进制解析()
    {
        // Winsock 把 "001.002.003.004" 当十进制；本实现一致
        Assert.Equal(GateShareInet.InetAddr("1.2.3.4"), GateShareInet.InetAddr("001.002.003.004"));
    }

    [Fact]
    public void InetAddr_ShorthandForms_DifferFromWinsock()
    {
        // ★ 已知差异（见 GateShareAddressUtils.cs 文件头）：Windows inet_addr 还接受
        //   1/2/3 段简写与 0x/0 前缀；本实现一律视为非法。
        Assert.Equal(GateShareInet.INADDR_NONE, GateShareInet.InetAddr("0x01020304"));
        Assert.Equal(GateShareInet.INADDR_NONE, GateShareInet.InetAddr("16909060"));
        Assert.Equal(GateShareInet.INADDR_NONE, GateShareInet.InetAddr("1.2.1000"));
    }

    [Fact]
    public void InetNtoa_还原四段()
    {
        Assert.Equal("202.103.100.1", GateShareInet.InetNtoa(23357386u));
        Assert.Equal("0.0.0.0", GateShareInet.InetNtoa(0u));
        Assert.Equal("255.255.255.255", GateShareInet.InetNtoa(0xFFFFFFFFu));
        Assert.Equal("1.2.3.4", GateShareInet.InetNtoa(GateShareInet.InetAddr("1.2.3.4")));
    }

    // ================= ReverseBytes / IP2Long / Long2IP（原 :2971 / :2979 / :2987） =================

    [Fact]
    public void ReverseBytes_按字节反转()
    {
        Assert.Equal(0x04030201u, GateShareInet.ReverseBytes(0x01020304u));
        Assert.Equal(0x01020304u, GateShareInet.ReverseBytes(0x04030201u));
        Assert.Equal(0u, GateShareInet.ReverseBytes(0u));
        Assert.Equal(0xFFFFFFFFu, GateShareInet.ReverseBytes(0xFFFFFFFFu));
    }

    [Fact]
    public void ReverseBytes_两次还原()
    {
        foreach (uint v in new uint[] { 0u, 1u, 0x80000000u, 0xDEADBEEFu, uint.MaxValue })
            Assert.Equal(v, GateShareInet.ReverseBytes(GateShareInet.ReverseBytes(v)));
    }

    [Fact]
    public void IP2Long_得到大端数值()
    {
        // 原 :2983 的注释解释了为什么要 ReverseBytes：让数值顺序 == IP 段顺序
        Assert.Equal(0x01020304u, GateShareInet.IP2Long("1.2.3.4"));
        Assert.Equal(0xCA676401u, GateShareInet.IP2Long("202.103.100.1"));   // ReverseBytes(0x016467CA) = 3395773441
    }

    [Fact]
    public void IP2Long_顺序与IP段一致()
    {
        Assert.True(GateShareInet.IP2Long("10.0.0.1") < GateShareInet.IP2Long("10.0.0.2"));
        Assert.True(GateShareInet.IP2Long("9.255.255.255") < GateShareInet.IP2Long("10.0.0.0"));
        Assert.True(GateShareInet.IP2Long("192.168.0.1") < GateShareInet.IP2Long("192.168.1.1"));
    }

    [Fact]
    public void IP2Long_非法输入得0xFFFFFFFF()
    {
        // inet_addr 失败 → 0xFFFFFFFF，ReverseBytes 后仍是 0xFFFFFFFF
        Assert.Equal(0xFFFFFFFFu, GateShareInet.IP2Long("garbage"));
        Assert.Equal(0xFFFFFFFFu, GateShareInet.IP2Long(""));
    }

    [Fact]
    public void Long2IP_与IP2Long互逆()
    {
        foreach (string ip in new[] { "0.0.0.0", "1.2.3.4", "127.0.0.1", "202.103.100.1", "255.255.255.255" })
            Assert.Equal(ip, GateShareInet.Long2IP(GateShareInet.IP2Long(ip)));
    }

    [Fact]
    public void Long2IP_0与极大值()
    {
        Assert.Equal("0.0.0.0", GateShareInet.Long2IP(0u));
        Assert.Equal("255.255.255.255", GateShareInet.Long2IP(0xFFFFFFFFu));
    }

    // ================= IsHexString（原 :1805-1821） =================

    [Fact]
    public void IsHexString_正常十六进制()
    {
        Assert.True(GateShareInet.IsHexString("0123456789abcdef"));
        Assert.True(GateShareInet.IsHexString("0123456789ABCDEF"));
        Assert.True(GateShareInet.IsHexString(new string('F', 32)));
    }

    [Fact]
    public void IsHexString_空串与非法字符()
    {
        Assert.False(GateShareInet.IsHexString(""));            // 原 :1810 `Length(S) = 0 → False`
        Assert.False(GateShareInet.IsHexString(null));
        Assert.False(GateShareInet.IsHexString("0G"));          // 'G' 不在 ['0'..'9','A'..'F','a'..'f']
        Assert.False(GateShareInet.IsHexString("12 34"));       // 空格非法
        Assert.False(GateShareInet.IsHexString("12-34"));
    }

    [Fact]
    public void IsHexString_单字符边界()
    {
        Assert.True(GateShareInet.IsHexString("0"));
        Assert.True(GateShareInet.IsHexString("f"));
        Assert.False(GateShareInet.IsHexString("g"));
        Assert.False(GateShareInet.IsHexString("@"));           // '0'-1
        Assert.False(GateShareInet.IsHexString("G"));           // 'F'+1
    }

    // ================= StrToHexEx（原 :1823-1837） =================

    [Fact]
    public void StrToHexEx_逐字节大写十六进制()
    {
        Assert.Equal("", GateShareInet.StrToHexEx(""));
        Assert.Equal("00", GateShareInet.StrToHexEx("\0"));
        Assert.Equal("0A", GateShareInet.StrToHexEx("\n"));
        Assert.Equal("FF", GateShareInet.StrToHexEx("\u00FF"));
        Assert.Equal("414243", GateShareInet.StrToHexEx("ABC"));
    }

    [Fact]
    public void StrToHexEx_LengthAbove255_DoesNotHang()
    {
        // ★ 原文缺陷：`I: Byte` + `for I := 1 to Length(S)`（原 :1825/:1832）在长度 > 255 时
        //   计数器回绕 → Delphi 侧**死循环**。托管侧用 int 计数器，不复制这个 hang。
        string s = new string('A', 256);
        string hex = GateShareInet.StrToHexEx(s);
        Assert.Equal(512, hex.Length);
        Assert.Equal(string.Concat(System.Linq.Enumerable.Repeat("41", 256)), hex);
    }

    [Fact]
    public void StrToHexEx_大于0xFF的字符按低8位截断()
    {
        // AnsiString 下标取的是字节；托管 string 是 UTF-16 → 按低 8 位截断（已登记差异）
        Assert.Equal("2D", GateShareInet.StrToHexEx("\u042D"));    // 0x042D → 0x2D
    }

    // ================= HexToStrEx（原 :1839-1864） =================

    [Fact]
    public void HexToStrEx_正常往返()
    {
        string outp = "sentinel";
        Assert.True(GateShareInet.HexToStrEx("414243", ref outp));
        Assert.Equal("ABC", outp);

        string back = "";
        Assert.True(GateShareInet.HexToStrEx("00FF", ref back));
        Assert.Equal("\0\u00FF", back);
    }

    [Fact]
    public void HexToStrEx_奇数长度返回False且OutStr不变()
    {
        string outp = "sentinel";
        Assert.False(GateShareInet.HexToStrEx("ABC", ref outp));
        Assert.Equal("sentinel", outp);          // 原 :1862 只在成功路径赋值
        Assert.False(GateShareInet.HexToStrEx("A", ref outp));
        Assert.Equal("sentinel", outp);
    }

    [Fact]
    public void HexToStrEx_非法字符返回False且OutStr不变()
    {
        string outp = "sentinel";
        Assert.False(GateShareInet.HexToStrEx("41G3", ref outp));
        Assert.Equal("sentinel", outp);
        Assert.False(GateShareInet.HexToStrEx("4 43", ref outp));
        Assert.Equal("sentinel", outp);
    }

    [Fact]
    public void HexToStrEx_空串成功且OutStr置空()
    {
        string outp = "sentinel";
        Assert.True(GateShareInet.HexToStrEx("", ref outp));
        Assert.Equal("", outp);
        // ★ 差异登记：Delphi `AnsiString` 无法为 nil（传 nil 会 AV）；托管侧把 null 当作非法输入返回 False。
        Assert.False(GateShareInet.HexToStrEx(null, ref outp));
    }

    [Fact]
    public void HexToStrEx_大小写混合可解析()
    {
        string outp = "";
        Assert.True(GateShareInet.HexToStrEx("aAbBcC", ref outp));
        Assert.Equal(3, outp.Length);
        Assert.Equal((char)0xAA, outp[0]);
        Assert.Equal((char)0xBB, outp[1]);
        Assert.Equal((char)0xCC, outp[2]);
    }
}

[Collection("RunGateFormLane")]
public sealed class GateShareContainerTests
{
    // ================= TSockaddr / TAddressInfo / TIPSection（原 :29/:78/:211） =================

    [Fact]
    public void TSockaddr_字段默认值为0()
    {
        var s = new TSockaddr();
        Assert.Equal(0, s.nIPaddr);
        Assert.Equal(0u, s.dwStartAttackTick);
        Assert.Equal(0, s.nAttackCount);
        Assert.Equal(0, s.nSocketHandle);
    }

    [Fact]
    public void TAddressInfo_字段默认值()
    {
        var a = new TAddressInfo();
        Assert.Equal("", a.sIPaddr);
        Assert.Equal(0, a.nCount);
        Assert.Equal(0u, a.dwIPCountTick1);
        Assert.Equal(0, a.nIPCount2);
        Assert.Equal(0u, a.dwDenyTick);
    }

    [Fact]
    public void TIPSection_存放大端数值()
    {
        var s = new TIPSection { nBeginAddr = GateShareInet.IP2Long("10.0.0.1"), nEndAddr = GateShareInet.IP2Long("10.0.0.255") };
        Assert.True(s.nBeginAddr < s.nEndAddr);
    }

    // ================= TAddressList（原 :2538-2704） =================

    [Fact]
    public void TAddressList_Add合法IP返回新项()
    {
        var l = new TAddressList();
        var a = l.Add("192.168.1.1");
        Assert.NotNull(a);
        Assert.Equal(1, l.Count);
        Assert.Equal(unchecked((int)GateShareInet.InetAddr("192.168.1.1")), a.nIPaddr);
        Assert.Equal(0, a.nAttackCount);            // 原 :2628
    }

    [Fact]
    public void TAddressList_空串与null返回null()
    {
        var l = new TAddressList();
        Assert.Null(l.Add(""));                     // 原 :2618
        Assert.Null(l.Add(null));
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TAddressList_重复IP返回null不重复添加()
    {
        var l = new TAddressList();
        Assert.NotNull(l.Add("1.2.3.4"));
        Assert.Null(l.Add("1.2.3.4"));              // 原 :2622-2623
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void TAddressList_非法IP字符串仍然入库_原文缺陷1()
    {
        // ★ 原文 :2620 的 `if nIP = INADDR_NONE then Exit;` 是死代码（Integer vs Cardinal 经 Int64 提升）。
        //   后果：非法 IP 被加入，且 nIPaddr = -1（与 255.255.255.255 撞车）。
        var l = new TAddressList();
        var a = l.Add("not-an-ip");
        Assert.NotNull(a);
        Assert.Equal(-1, a.nIPaddr);
        Assert.Equal(1, l.Count);

        // 且"255.255.255.255"（合法广播地址，inet_addr 同样是 0xFFFFFFFF）会被判为**重复**
        Assert.Null(l.Add("255.255.255.255"));
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void TAddressList_Find与FindIndex()
    {
        var l = new TAddressList();
        var a = l.Add("10.0.0.1");
        l.Add("10.0.0.2");

        Assert.Same(a, l.Find("10.0.0.1"));
        Assert.Null(l.Find("10.0.0.3"));
        Assert.Equal(0, l.FindIndex("10.0.0.1"));
        Assert.Equal(1, l.FindIndex("10.0.0.2"));
        Assert.Equal(-1, l.FindIndex("10.0.0.3"));
    }

    [Fact]
    public void TAddressList_Find按数值而不是字符串()
    {
        var l = new TAddressList();
        var a = l.Add("001.002.003.004");
        // inet_addr 的十进制解析 → 与 "1.2.3.4" 同一个 nIPaddr
        Assert.Same(a, l.Find("1.2.3.4"));
    }

    [Fact]
    public void TAddressList_索引器越界返回null()
    {
        var l = new TAddressList();
        Assert.Null(l[-1]);
        Assert.Null(l[0]);                          // 空表：`0 <= -1` 为假
        l.Add("1.1.1.1");
        Assert.Null(l[1]);                          // 原 :2582 `<= Count - 1`
        Assert.NotNull(l[0]);
        Assert.Null(l[int.MaxValue]);
        Assert.Null(l[int.MinValue]);
    }

    [Fact]
    public void TAddressList_Delete按引用与按IP()
    {
        var l = new TAddressList();
        var a = l.Add("1.1.1.1");
        var b = l.Add("2.2.2.2");

        l.Delete(new TSockaddr { nIPaddr = a.nIPaddr });   // 值相同但**不是同一引用** → 不删
        Assert.Equal(2, l.Count);

        l.Delete(a);                                        // 原 :2676 引用相等
        Assert.Equal(1, l.Count);
        Assert.Same(b, l[0]);

        l.Delete("2.2.2.2");                                // 原 :2685-2692 按 IP
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TAddressList_DeleteIndex越界与负下标()
    {
        var l = new TAddressList();
        l.Add("1.1.1.1");
        l.DeleteIndex(-1);
        l.DeleteIndex(1);
        l.DeleteIndex(int.MaxValue);
        Assert.Equal(1, l.Count);
        l.DeleteIndex(0);
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TAddressList_Clear可重复调用()
    {
        var l = new TAddressList();
        l.Add("1.1.1.1");
        l.Add("2.2.2.2");
        l.Clear();
        Assert.Equal(0, l.Count);
        l.Clear();
        Assert.Equal(0, l.Count);
        Assert.Null(l.Find("1.1.1.1"));
    }

    [Fact]
    public void TAddressList_Lock可重入()
    {
        var l = new TAddressList();
        l.Lock();
        l.Lock();                                   // CRITICAL_SECTION 可重入
        l.Add("1.1.1.1");
        l.UnLock();
        l.UnLock();
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void TAddressList_UnLock未持有时抛异常()
    {
        var l = new TAddressList();
        // ★ 差异登记：原文 LeaveCriticalSection 未持有时是未定义行为（不抛）；Monitor.Exit 抛。
        Assert.Throws<System.Threading.SynchronizationLockException>(() => l.UnLock());
    }

    [Fact]
    public void TAddressList_大量元素不重复()
    {
        var l = new TAddressList();
        for (int i = 0; i < 200; i++) Assert.NotNull(l.Add($"10.1.{i / 256}.{i % 256}"));
        Assert.Equal(200, l.Count);
        for (int i = 0; i < 200; i++) Assert.Null(l.Add($"10.1.{i / 256}.{i % 256}"));
        Assert.Equal(200, l.Count);
    }

    // ================= TAddressListEx（原 :2708-2834） =================

    [Fact]
    public void TAddressListEx_Add保存sIPaddr字符串()
    {
        var l = new TAddressListEx();
        var a = l.Add("172.16.0.9");
        Assert.NotNull(a);
        Assert.Equal("172.16.0.9", a.sIPaddr);      // 原 :2798（TSockaddr 没有该字段）
        Assert.Equal(unchecked((int)GateShareInet.InetAddr("172.16.0.9")), a.nIPaddr);
        Assert.Equal(0, a.nCount);
    }

    [Fact]
    public void TAddressListEx_空串重复与非法()
    {
        var l = new TAddressListEx();
        Assert.Null(l.Add(""));
        Assert.Null(l.Add(null));
        Assert.NotNull(l.Add("1.2.3.4"));
        Assert.Null(l.Add("1.2.3.4"));
        Assert.NotNull(l.Add("garbage"));           // ★ 原文缺陷 1 同样适用于 Ex
        Assert.Equal(2, l.Count);
    }

    [Fact]
    public void TAddressListEx_Find与索引器()
    {
        var l = new TAddressListEx();
        Assert.Null(l[0]);
        var a = l.Add("1.1.1.1");
        Assert.Same(a, l.Find("1.1.1.1"));
        Assert.Null(l.Find("1.1.1.2"));
        Assert.Null(l[1]);
        Assert.Null(l[-1]);
        Assert.Same(a, l[0]);
    }

    [Fact]
    public void TAddressListEx_Delete按引用Delete可重复Clear()
    {
        var l = new TAddressListEx();
        var a = l.Add("1.1.1.1");
        l.Add("2.2.2.2");
        l.Delete(a);
        l.Delete(a);                                // 再删无副作用
        Assert.Equal(1, l.Count);
        l.Clear();
        l.Clear();
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TAddressListEx_计数域可写()
    {
        var l = new TAddressListEx();
        var a = l.Add("1.1.1.1");
        a.nCount = 7;
        a.nIPCount1 = 3;
        a.nIPCount2 = 4;
        a.dwIPCountTick1 = 111;
        a.dwIPCountTick2 = 222;
        a.dwDenyTick = 333;
        a.nIPDenyCount = 5;
        Assert.Equal(7, l.Find("1.1.1.1").nCount);
        Assert.Equal(3, l[0].nIPCount1);
        Assert.Equal(4, l[0].nIPCount2);
        Assert.Equal(111u, l[0].dwIPCountTick1);
        Assert.Equal(222u, l[0].dwIPCountTick2);
        Assert.Equal(333u, l[0].dwDenyTick);
        Assert.Equal(5, l[0].nIPDenyCount);
    }

    // ================= TSafeStringList（原 :2883-2924） =================

    [Fact]
    public void TSafeStringList_AddCount与索引()
    {
        var l = new TSafeStringList();
        Assert.Equal(0, l.Count);
        l.Add("a");
        l.Add("b");
        Assert.Equal(2, l.Count);
        Assert.Equal("a", l[0]);
        Assert.Equal("b", l[1]);
        l[1] = "B";
        Assert.Equal("B", l[1]);
    }

    [Fact]
    public void TSafeStringList_IndexOf大小写不敏感()
    {
        // Delphi TStringList 默认 CaseSensitive=False
        var l = new TSafeStringList();
        l.Add("Abc");
        Assert.Equal(0, l.IndexOf("abc"));
        Assert.Equal(0, l.IndexOf("ABC"));
        Assert.Equal(-1, l.IndexOf("abd"));
        Assert.Equal(-1, l.IndexOf(""));
    }

    [Fact]
    public void TSafeStringList_Delete越界抛异常()
    {
        var l = new TSafeStringList();
        l.Add("a");
        Assert.Throws<ArgumentOutOfRangeException>(() => l.Delete(5));
        l.Delete(0);
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TSafeStringList_Text读写()
    {
        var l = new TSafeStringList();
        l.Text = "a\r\nb\r\nc";                     // 末尾无换行 → 3 行
        Assert.Equal(3, l.Count);
        Assert.Equal("a\r\nb\r\nc", l.Text);

        l.Text = "x\r\ny\r\n";                      // 末尾换行不产生额外空行
        Assert.Equal(2, l.Count);

        l.Text = "";
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TSafeStringList_Text处理裸LF与裸CR()
    {
        var l = new TSafeStringList();
        l.Text = "a\nb\rc";
        Assert.Equal(3, l.Count);
        Assert.Equal("a", l[0]);
        Assert.Equal("b", l[1]);
        Assert.Equal("c", l[2]);
    }

    [Fact]
    public void TSafeStringList_落盘往返_GBK()
    {
        string path = Path.Combine(Path.GetTempPath(), "p2rg-tsafe-" + Guid.NewGuid().ToString("N") + ".txt");
        try
        {
            var l = new TSafeStringList();
            l.Add("第一行");
            l.Add("second");
            l.SaveToFile(path);

            var r = new TSafeStringList();
            r.LoadFromFile(path);
            Assert.Equal(2, r.Count);
            Assert.Equal("第一行", r[0]);
            Assert.Equal("second", r[1]);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public void TSafeStringList_Lines快照与Lock()
    {
        var l = new TSafeStringList();
        l.Lock();
        l.Lock();
        l.Add("a");
        l.UnLock();
        l.UnLock();
        Assert.Equal(new[] { "a" }, l.Lines);
        l.Clear();
        Assert.Empty(l.Lines);
    }

    // ================= TSafeMemoryStream（原 :2928-2969；原文无使用点） =================

    [Fact]
    public void TSafeMemoryStream_可读写且Lock可重入()
    {
        using var ms = new TSafeMemoryStream();
        ms.Lock();
        ms.Lock();
        ms.Write(new byte[] { 1, 2, 3 }, 0, 3);
        ms.UnLock();
        ms.UnLock();
        Assert.Equal(3, ms.Length);
    }

    [Fact]
    public void TSafeMemoryStream_UnLock未持有抛异常()
    {
        using var ms = new TSafeMemoryStream();
        Assert.Throws<System.Threading.SynchronizationLockException>(() => ms.UnLock());
    }

    [Fact]
    public void TSafeMemoryStream_Destroy是空操作()
    {
        var ms = new TSafeMemoryStream();
        ms.Destroy();
        ms.WriteByte(1);
        Assert.Equal(1, ms.Length);
        ms.Dispose();
    }

    // ================= TProcessBlacklist（原 :3261-3374） =================

    [Fact]
    public void TProcessBlacklist_默认MaxCount为80()
    {
        var l = new TProcessBlacklist();
        Assert.Equal(80, l.MaxCount);               // 原 :3271
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void TProcessBlacklist_Add把MD5转大写()
    {
        // ★ 与旧接缝的差异断言：原文 :3301 `Result.ProcessMD5 := UpperCase(ProcessMD5)`
        var l = new TProcessBlacklist();
        var a = l.Add("cheat.exe", "0123456789abcdef0123456789abcdef");
        Assert.NotNull(a);
        Assert.Equal("0123456789ABCDEF0123456789ABCDEF", a.ProcessMD5);
        Assert.Equal("cheat.exe", a.ProcessName);   // 进程名**不**转换
    }

    [Fact]
    public void TProcessBlacklist_重复MD5返回null_大小写不敏感()
    {
        var l = new TProcessBlacklist();
        Assert.NotNull(l.Add("p1", new string('A', 32)));
        Assert.Null(l.Add("p2", new string('a', 32)));      // SameText → 命中
        Assert.Null(l.Add("p2", new string('A', 32)));
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void TProcessBlacklist_满80后返回null()
    {
        var l = new TProcessBlacklist();
        for (int i = 0; i < 80; i++) Assert.NotNull(l.Add("p" + i, i.ToString("X8").PadLeft(32, '0')));
        Assert.Equal(80, l.Count);
        Assert.Null(l.Add("p80", new string('F', 32)));     // 原 :3296
        Assert.Equal(80, l.Count);
    }

    [Fact]
    public void TProcessBlacklist_MaxCountForTest可控()
    {
        var l = new TProcessBlacklist { MaxCountForTest = 1 };
        Assert.Equal(1, l.MaxCount);
        Assert.NotNull(l.Add("p", new string('A', 32)));
        Assert.Null(l.Add("q", new string('B', 32)));
    }

    [Fact]
    public void TProcessBlacklist_Find与索引器()
    {
        var l = new TProcessBlacklist();
        Assert.Null(l[0]);
        Assert.Null(l[-1]);
        var a = l.Add("p1", new string('A', 32));
        Assert.Equal(80, l.MaxCount);
        Assert.Same(a, l.Find(new string('a', 32)));        // SameText
        Assert.Null(l.Find(new string('B', 32)));
        Assert.Same(a, l[0]);
        Assert.Same(a, l.Items[0]);
        Assert.Null(l[1]);                                  // 原 :3350 `Index < Count`
    }

    [Fact]
    public void TProcessBlacklist_Delete按引用而非按值()
    {
        var l = new TProcessBlacklist();
        var a = l.Add("p1", new string('A', 32));
        var b = l.Add("p2", new string('B', 32));

        l.Delete(new TProcessInfo { ProcessName = "p1", ProcessMD5 = new string('A', 32) });  // 值相同不同引用
        Assert.Equal(2, l.Count);

        l.Delete(a);
        Assert.Equal(1, l.Count);
        Assert.Same(b, l[0]);

        l.Delete(a);                                        // 再删无副作用
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void TProcessBlacklist_Clear与Lock()
    {
        var l = new TProcessBlacklist();
        l.Add("p", new string('A', 32));
        l.Lock();
        l.Lock();
        l.Clear();
        l.UnLock();
        l.UnLock();
        Assert.Equal(0, l.Count);
        Assert.Null(l.Find(new string('A', 32)));
    }
}

[Collection("RunGateFormLane")]
public sealed class GateShareGlobalsTests
{
    public GateShareGlobalsTests()
    {
        // 本类同时读 FormGlobals 与 GateShareGlobals；两者都是单元级静态量，必须显式复位以免串场。
        FormGlobals.ResetForTest();
        GateShareGlobals.ResetForTest();
    }

    [Fact]
    public void OriginalInlineDefaults_MatchSourceLiterals()
    {
        // 原文 `var` 段的内联初值（GateShare.pas 行号见 GateShareGlobals 的登记表）
        Assert.Equal(3, (int)GateShareGlobals.g_btShowLogLevel);                       // :1172
        Assert.False(GateShareGlobals.g_boOneMACLimitePlayer);                          // :1180
        Assert.Equal(3, GateShareGlobals.g_nOneMACLimitePlayerCount);                   // :1181
        Assert.False(GateShareGlobals.g_boOpenVerifyCode);                              // :1148
        Assert.Equal(3, GateShareGlobals.g_nVerifyCodeErrCount);                        // :1149
        Assert.Equal(4, GateShareGlobals.g_nVerifyCodeRefreshCount);                    // :1150
        Assert.Equal(60, GateShareGlobals.g_nVerifyCodeWaitTime);                       // :1151
        Assert.Equal(30u, GateShareGlobals.g_dwVerifyCodeInterval1);                    // :1152
        Assert.Equal(50u, GateShareGlobals.g_dwVerifyCodeInterval2);                    // :1153
        Assert.Equal(0u, GateShareGlobals.g_dwVerifySuccessAddInterval);                // :1154
        Assert.False(GateShareGlobals.g_boVerifyFailTriggerScript);                     // :1155
        Assert.False(GateShareGlobals.g_boVerifyFailLoginVerify);                       // :1156
        Assert.True(GateShareGlobals.g_boVerifyCodeExcludeMap);                         // :1157
        Assert.Equal("", GateShareGlobals.g_sVerifyCodeExcludeMapFileName);             // :1158
        Assert.False(GateShareGlobals.g_boAutoLoadNoVerifyChrList);                     // :1161
        Assert.Equal(@"D:\MirServer\Mir200\Envir\QuestDiary\白名单用户.txt", GateShareGlobals.g_sLoadNoVerifyChrListFile);   // :1162
        Assert.Equal(300, GateShareGlobals.g_nAutoLoadNoVerifyChrListInterval);         // :1163
        Assert.Equal('*', GateShareGlobals.g_sReplaceWord);                             // :1216
        Assert.Equal("游戏网关", GateShareGlobals.g_sTitleName);                          // :1142
        Assert.Equal("127.0.0.1", GateShareGlobals.g_sServerAddr);                      // :1143
        Assert.Equal(5000, (int)GateShareGlobals.g_wdServerPort);                       // :1144
        Assert.Equal("0.0.0.0", GateShareGlobals.g_sGateAddr);                          // :1145
        Assert.Equal(7200, (int)GateShareGlobals.g_wdGatePort);                         // :1146
        Assert.Equal(27201, (int)GateShareGlobals.g_wdDBPort);                          // :1170
        Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllBlockSize);                 // :597
        Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllSendInterval);              // :598
        Assert.Equal(0u, GateShareGlobals.g_ClientAntiPlugVersion);                     // :601
        Assert.Equal(0u, GateShareGlobals.g_ClientAntiPlugDllStringCRC);                // :603
        Assert.Equal(0, GateShareGlobals.g_ClientAntiPlugDllBlockCount);                // :604
        Assert.Equal("RunGatePlug.dll", GateShareGlobals.g_sRunGatePlusDllName);        // :614
        Assert.Equal(IntPtr.Zero, GateShareGlobals.g_dwGameCenterHandle);               // :1230
        Assert.Equal(IntPtr.Zero, GateShareGlobals.g_RunGatePlugDllHandle);             // :615
    }

    [Fact]
    public void 与FormGlobals的口径差异被显式登记()
    {
        // ★ 差异登记：原文内联初值 vs FormGlobals（窗体测试的新建状态）
        Assert.Equal(50, GateShareGlobals.Original_nMaxConnOfIPaddr);        // 原文 :1218
        Assert.Equal(1u, GateShareGlobals.Original_dwDefenseLevel);          // 原文 :1264
        Assert.Equal(5, (int)GateShareGlobals.Original_wAntiPlugUpdateCheckInterval);   // 原文 :609

        Assert.Equal(0, FormGlobals.g_nMaxConnOfIPaddr);                     // FormGlobals 置 0
        Assert.Equal(0u, FormGlobals.g_dwDefenseLevel);                      // FormGlobals 置 0
        Assert.Equal(1, FormGlobals.g_wAntiPlugUpdateCheckInterval);         // FormGlobals 置 1
    }

    [Fact]
    public void OriginalInlineLiterals表覆盖了全部登记项()
    {
        Assert.Equal(32, GateShareGlobals.OriginalInlineLiterals.Length);
        foreach (var (name, literal) in GateShareGlobals.OriginalInlineLiterals)
        {
            Assert.False(string.IsNullOrEmpty(name));
            Assert.NotNull(literal);
        }
    }

    [Fact]
    public void IPSectionList_门与访问器()
    {
        GateShareGlobals.ResetForTest();
        Assert.Equal(0, GateShareGlobals.IPSectionListCount);

        GateShareGlobals.LockIPSectionList();
        try
        {
            GateShareGlobals.AddIPSection(new TIPSection
            {
                nBeginAddr = GateShareInet.IP2Long("10.0.0.1"),
                nEndAddr = GateShareInet.IP2Long("10.0.0.255"),
            });
        }
        finally
        {
            GateShareGlobals.UnLockIPSectionList();
        }

        Assert.Equal(1, GateShareGlobals.IPSectionListCount);
        Assert.NotNull(GateShareGlobals.GetIPSection(0));
        Assert.Null(GateShareGlobals.GetIPSection(1));
        Assert.Null(GateShareGlobals.GetIPSection(-1));

        GateShareGlobals.ClearIPSectionList();
        Assert.Equal(0, GateShareGlobals.IPSectionListCount);
    }

    [Fact]
    public void ResetForTest把新增全局量复位()
    {
        GateShareGlobals.g_btShowLogLevel = 9;
        GateShareGlobals.g_MainLogStrings.Add("x");
        GateShareGlobals.g_TempIPList.Add("1.2.3.4");
        GateShareGlobals.g_BlockIPList.Add("1.2.3.4");
        GateShareGlobals.g_AttackIPaddrList.Add("1.2.3.4");
        GateShareGlobals.g_CurrIPList.Add("1.2.3.4");
        GateShareGlobals.g_TempMacList.Add("m");
        GateShareGlobals.g_BlockMacList.Add("m");
        GateShareGlobals.g_FYDenyIPList.Add("1.2.3.4");
        GateShareGlobals.g_FYDenyMACList.Add("m");
        GateShareGlobals.g_DBAddressList.Add("db");
        GateShareGlobals.g_VerifyFailUserList.Add("u");
        GateShareGlobals.g_LockUserList.Add("u");
        GateShareGlobals.g_LoginMACPlayerList.Add("u");
        GateShareGlobals.g_VerifyCodeMapList.Add("m");
        GateShareGlobals.g_LoadNoVerifyChrList.Add("c");
        GateShareGlobals.AddIPSection(new TIPSection());
        GateShareGlobals.g_sTitleName = "X";
        GateShareGlobals.g_ClientAntiPlugVersion = 7;
        GateShareGlobals.g_dwFYReadDenyIPTick = 99;

        GateShareGlobals.ResetForTest();

        Assert.Equal(3, (int)GateShareGlobals.g_btShowLogLevel);
        Assert.Equal(0, GateShareGlobals.g_MainLogStrings.Count);
        Assert.Equal(0, GateShareGlobals.g_IOCPLogStrings.Count);
        Assert.Equal(0, GateShareGlobals.g_TempIPList.Count);
        Assert.Equal(0, GateShareGlobals.g_BlockIPList.Count);
        Assert.Equal(0, GateShareGlobals.g_AttackIPaddrList.Count);
        Assert.Equal(0, GateShareGlobals.g_CurrIPList.Count);
        Assert.Equal(0, GateShareGlobals.g_TempMacList.Count);
        Assert.Equal(0, GateShareGlobals.g_BlockMacList.Count);
        Assert.Equal(0, GateShareGlobals.g_FYDenyIPList.Count);
        Assert.Equal(0, GateShareGlobals.g_FYDenyMACList.Count);
        Assert.Equal(0, GateShareGlobals.g_DBAddressList.Count);
        Assert.Equal(0, GateShareGlobals.g_VerifyFailUserList.Count);
        Assert.Equal(0, GateShareGlobals.g_LockUserList.Count);
        Assert.Equal(0, GateShareGlobals.g_LoginMACPlayerList.Count);
        Assert.Equal(0, GateShareGlobals.g_VerifyCodeMapList.Count);
        Assert.Equal(0, GateShareGlobals.g_LoadNoVerifyChrList.Count);
        Assert.Equal(0, GateShareGlobals.IPSectionListCount);
        Assert.Equal("游戏网关", GateShareGlobals.g_sTitleName);
        Assert.Equal(0u, GateShareGlobals.g_ClientAntiPlugVersion);
        Assert.Equal(0u, GateShareGlobals.g_dwFYReadDenyIPTick);
        Assert.Null(GateShareGlobals.g_ClientAntiPlugStream);
    }
}
