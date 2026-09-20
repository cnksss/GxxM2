// 测试：Source\RunGate\MagicIntervalUtils.pas（实测 LF 198）→ src/GXX.RunGate/GateShareMagicIntervalUtils.cs
//
// 覆盖策略：
//   * 每个公开成员 ≥3 用例（空 / 0 / 负 / 越界 / 异常路径）；
//   * 原文缺陷写成**差异断言**（文件头 §1/§2/§5）：二分查找的有序前提、LoadFromFile 缺文件不清空、
//     Word 收窄截断、SaveToFile 无 INI 节头；
//   * 文件系统用例一律用 `Path.GetTempPath()` 下的独立临时目录，Dispose 时删除（不碰真实工作目录）。
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

public sealed class GateShareMagicIntervalTests : IDisposable
{
    private readonly string _dir;

    public GateShareMagicIntervalTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p2rg-magicinterval-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { /* 清理失败不影响断言 */ }
    }

    private string Path_(string name) => Path.Combine(_dir, name);

    private static readonly Encoding GBK = Encoding.GetEncoding(936);

    // ================= Create / Count / Duplicates（原 :46-51, :95-98, :26） =================

    [Fact]
    public void Create_初始为空且Duplicates为False()
    {
        var list = new TMagicIntervalList();
        Assert.Equal(0, list.Count);
        Assert.False(list.Duplicates);          // 原 :49 `FDuplicates := False`
    }

    [Fact]
    public void Duplicates_可写且不改变已有元素()
    {
        var list = new TMagicIntervalList();
        list.Add(5);
        list.Duplicates = true;                 // 原 :26 是可读写属性
        Assert.True(list.Duplicates);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Count_随Add与Clear变化()
    {
        var list = new TMagicIntervalList();
        Assert.Equal(0, list.Count);
        list.Add(1);
        list.Add(2);
        Assert.Equal(2, list.Count);
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    // ================= 索引器 Items（原 :100-106） =================

    [Fact]
    public void Items_越界与负数返回null()
    {
        var list = new TMagicIntervalList();
        Assert.Null(list[-1]);                   // 原 :102 `Index >= 0` 失败
        Assert.Null(list[0]);                    // 空表：`Index <= FList.Count - 1` = `0 <= -1` 失败
        list.Add(7);
        Assert.Null(list[1]);                    // 原 :102 用的是 `<= Count - 1`（不是 `< Count`）
        Assert.NotNull(list[0]);
        Assert.Equal(7, list[0].MagicId);
    }

    [Fact]
    public void Items_未命中返回null而命中返回同一实例()
    {
        var list = new TMagicIntervalList();
        var added = list.Add(3);
        Assert.Same(added, list[0]);
    }

    [Fact]
    public void Items_极大下标不抛异常()
    {
        var list = new TMagicIntervalList();
        list.Add(1);
        Assert.Null(list[int.MaxValue]);
        Assert.Null(list[int.MinValue]);
    }

    // ================= Clear（原 :74-83） =================

    [Fact]
    public void Clear_空表安全()
    {
        var list = new TMagicIntervalList();
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void Clear_清空后可重新Add同一MagicID()
    {
        var list = new TMagicIntervalList();
        list.Add(9).Interval = 111;
        list.Clear();
        var again = list.Add(9);
        Assert.NotNull(again);
        Assert.Equal(0u, again.Interval);        // 新记录 Interval 未赋值 → 0
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Clear_连续两次安全()
    {
        var list = new TMagicIntervalList();
        list.Add(1);
        list.Clear();
        list.Clear();
        Assert.Equal(0, list.Count);
    }

    // ================= Add（原 :61-72） =================

    [Fact]
    public void Add_新元素返回实例且按MagicId升序插入()
    {
        var list = new TMagicIntervalList();
        list.Add(30);
        list.Add(10);
        list.Add(20);
        // 二分 + Insert：无论调用顺序，物理顺序始终按 MagicId 升序
        Assert.Equal(10, list[0].MagicId);
        Assert.Equal(20, list[1].MagicId);
        Assert.Equal(30, list[2].MagicId);
    }

    [Fact]
    public void Add_重复MagicID返回null且保留首次记录()
    {
        var list = new TMagicIntervalList();
        list.Add(4).Interval = 100;
        var dup = list.Add(4);
        Assert.Null(dup);                        // 原 :66 `not Search(...) or FDuplicates` = False
        Assert.Equal(1, list.Count);
        Assert.Equal(100u, list.Find(4).Interval);
    }

    [Fact]
    public void Add_边界值0与65535均可插入且可命中()
    {
        var list = new TMagicIntervalList();
        Assert.NotNull(list.Add(0));
        Assert.NotNull(list.Add(ushort.MaxValue));
        Assert.Equal(0, list[0].MagicId);
        Assert.Equal(ushort.MaxValue, list[1].MagicId);
        Assert.NotNull(list.Find(0));
        Assert.NotNull(list.Find(ushort.MaxValue));
    }

    [Fact]
    public void Add_Duplicates为True时重复项被插入而不是替换()
    {
        var list = new TMagicIntervalList { Duplicates = true };   // 原 :66 的第二个条件
        list.Add(8).Interval = 1;
        var second = list.Add(8);
        Assert.NotNull(second);
        list[0].Interval = 2;

        // ★ 原文顺序语义：`Duplicates=True` 时 Search 收敛到重复段**首个**（:123 `H := I - 1`），
        //   于是新元素被 Insert 到**最前面** —— 先加的在后面。
        Assert.Equal(2, list.Count);
        Assert.Same(second, list[0]);            // 第二个加入的在前
        Assert.Equal(2u, list[0].Interval);
        Assert.Equal(1u, list[1].Interval);      // 第一个加入的在后（Interval 仍是 1）
    }

    // 原文缺陷 1（文件头 §1）：`Search` 是二分查找，**隐含要求 FList 按 MagicId 有序**。
    // 复核结论：原文 `Add` 用 `FList.Insert(Search 返回的插入点)`，因此只要 `Search` 未被覆写，
    // 有序不变量**自动成立**（下面的用例钉死这一点）；真正的风险来自 `Search` 是 `virtual`。
    [Fact]
    public void Add_任意插入顺序下始终保持升序_原文缺陷1的前半()
    {
        var list = new TMagicIntervalList();
        foreach (ushort id in new ushort[] { 100, 1, 999, 50, 2, 3, 500, 7 })
            list.Add(id);

        var ids = new List<int>();
        for (int i = 0; i < list.Count; i++) ids.Add(list[i].MagicId);
        var sorted = new List<int>(ids);
        sorted.Sort();
        Assert.Equal(sorted, ids);                       // 物理顺序恒等于升序
        Assert.NotNull(list.Find(999));
        Assert.NotNull(list.Find(1));
        Assert.Null(list.Find(4));
    }

    private sealed class ReverseSearchList : TMagicIntervalList
    {
        // 覆写 `virtual Search`（原 :108 `virtual`），始终报告"未命中且插入到末尾"→ 破坏有序性
        public override bool Search(ushort MagicID, out int Index)
        {
            Index = Count;
            return false;
        }
    }

    [Fact]
    public void Search_OverrideCanBreakSortedInvariant_虚方法语义()
    {
        var list = new ReverseSearchList();
        list.Add(100);
        list.Add(50);                            // 追加到末尾 → [100, 50] 失序
        Assert.Equal(100, list[0].MagicId);
        Assert.Equal(50, list[1].MagicId);
        // 二分在失序表上漏命中 —— 这正是原文把 Search 设为 virtual 所带来的可能性
        Assert.Null(list.Find(50));
    }

    // ================= Find（原 :85-93） =================

    [Fact]
    public void Find_命中返回实例未命中返回null()
    {
        var list = new TMagicIntervalList();
        list.Add(1).Interval = 10;
        list.Add(2).Interval = 20;
        Assert.Equal(10u, list.Find(1).Interval);
        Assert.Equal(20u, list.Find(2).Interval);
        Assert.Null(list.Find(3));               // 原 :90 `Result := nil`
    }

    [Fact]
    public void Find_空表返回null()
    {
        var list = new TMagicIntervalList();
        Assert.Null(list.Find(0));
        Assert.Null(list.Find(ushort.MaxValue));
    }

    [Fact]
    public void Find_单词表命中()
    {
        var list = new TMagicIntervalList();
        list.Add(42).Interval = 7;
        Assert.Equal(7u, list.Find(42).Interval);
        Assert.Null(list.Find(41));
        Assert.Null(list.Find(43));
    }

    // ================= Search（原 :108-132） =================

    [Fact]
    public void Search_未命中时Index为插入点()
    {
        var list = new TMagicIntervalList();
        list.Add(10);
        list.Add(20);
        list.Add(30);

        Assert.False(list.Search(5, out int i0));
        Assert.Equal(0, i0);                     // 原 :131 `Index := L`
        Assert.False(list.Search(25, out int i1));
        Assert.Equal(2, i1);
        Assert.False(list.Search(99, out int i2));
        Assert.Equal(3, i2);
    }

    [Fact]
    public void Search_空表未命中且Index为0()
    {
        var list = new TMagicIntervalList();
        Assert.False(list.Search(1, out int idx));
        Assert.Equal(0, idx);
    }

    [Fact]
    public void Search_命中且非重复模式时Index为命中下标()
    {
        var list = new TMagicIntervalList();
        list.Add(10);
        list.Add(20);
        list.Add(30);
        Assert.True(list.Search(20, out int idx));
        Assert.Equal(1, idx);                    // 原 :127 `if not FDuplicates then L := I;`
    }

    [Fact]
    public void Search_重复模式下收敛到重复段首个()
    {
        var list = new TMagicIntervalList { Duplicates = true };
        list.Add(7);
        list.Add(7);
        list.Add(7);
        Assert.Equal(3, list.Count);
        Assert.True(list.Search(7, out int idx));
        Assert.Equal(0, idx);                    // 原 :123 `H := I - 1` 继续向左
    }

    [Fact]
    public void Search_边界值0与65535()
    {
        var list = new TMagicIntervalList();
        list.Add(0);
        list.Add(40000);
        Assert.True(list.Search(0, out int i0));
        Assert.Equal(0, i0);
        Assert.True(list.Search(40000, out int i1));
        Assert.Equal(1, i1);
        Assert.False(list.Search(65535, out int i2));   // 插入点 = Count
        Assert.Equal(2, i2);
    }

    // ================= SaveToFile（原 :168-186） =================

    [Fact]
    public void SaveToFile_空表写出空文件()
    {
        string f = Path_("empty.txt");
        new TMagicIntervalList().SaveToFile(f);
        Assert.True(File.Exists(f));
        Assert.Equal("", File.ReadAllText(f, GBK));
    }

    [Fact]
    public void SaveToFile_明文行无INI节头_原文缺陷2()
    {
        string f = Path_("plain.txt");
        var list = new TMagicIntervalList();
        list.Add(11).Interval = 1234;
        list.Add(2).Interval = 0;
        list.SaveToFile(f);

        string text = File.ReadAllText(f, GBK);
        // ★ 与旧接缝（TIniFileEx + [Interval] 节）的差异断言
        Assert.DoesNotContain("[", text, StringComparison.Ordinal);
        Assert.Equal("2=0\r\n11=1234\r\n", text);   // 物理顺序按 MagicId 升序；CRLF 结尾
    }

    [Fact]
    public void SaveToFile_覆盖同名旧文件()
    {
        string f = Path_("overwrite.txt");
        File.WriteAllText(f, "OLD CONTENT\r\n", GBK);
        var list = new TMagicIntervalList();
        list.Add(1).Interval = 2;
        list.SaveToFile(f);
        Assert.Equal("1=2\r\n", File.ReadAllText(f, GBK));
    }

    [Fact]
    public void SaveToFile_负值Interval以LongWord无符号写回()
    {
        // 原文 `Interval: LongWord` —— 托管侧 uint。$-1$ 在 LongWord 域里回绕成 4294967295。
        string f = Path_("unsigned.txt");
        var list = new TMagicIntervalList();
        var m = list.Add(1);
        m.Interval = unchecked((uint)-1);
        list.SaveToFile(f);
        Assert.Equal("1=4294967295\r\n", File.ReadAllText(f, GBK));
    }

    // ================= LoadFromFile（原 :134-166） =================

    [Fact]
    public void LoadFromFile_文件不存在时保留旧列表_原文缺陷3()
    {
        var list = new TMagicIntervalList();
        list.Add(5).Interval = 55;
        list.LoadFromFile(Path_("no-such-file.txt"));   // 原 :142 `Exit` 在 :143 `Clear` 之前
        Assert.Equal(1, list.Count);
        Assert.Equal(55u, list.Find(5).Interval);
    }

    [Fact]
    public void LoadFromFile_读取合法键值并回填Interval()
    {
        string f = Path_("valid.txt");
        File.WriteAllText(f, "1=100\r\n2=200\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        Assert.Equal(2, list.Count);
        Assert.Equal(100u, list.Find(1).Interval);
        Assert.Equal(200u, list.Find(2).Interval);
    }

    [Fact]
    public void LoadFromFile_含空格与制表符的键值被Trim()
    {
        string f = Path_("trim.txt");
        File.WriteAllText(f, "  3 =   300  \r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        Assert.Equal(1, list.Count);
        Assert.Equal(300u, list.Find(3).Interval);
    }

    [Fact]
    public void LoadFromFile_无分隔符或非数字行被跳过()
    {
        string f = Path_("bad.txt");
        File.WriteAllText(f,
            "noseparator\r\n" +
            "=123\r\n" +
            "abc=1\r\n" +
            "1=abc\r\n" +
            "\r\n" +
            "4=4\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        // `noseparator` → Names='noseparator' Value='' → -1 跳过
        // `=123`       → Names='' Value='123'         → -1 跳过
        // `abc=1`      → -1 跳过
        // `1=abc`      → -1 跳过
        // ``（空行）    → -1 跳过
        // `4=4`        → 保留
        Assert.Equal(1, list.Count);
        Assert.Equal(4u, list.Find(4).Interval);
    }

    [Fact]
    public void LoadFromFile_负值行被跳过()
    {
        string f = Path_("neg.txt");
        File.WriteAllText(f, "-1=5\r\n5=-2\r\n6=6\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        // 原文 :154 判据是 `(nMagicID >= 0) and (nMagicValue >= 0)` → 前两行都被跳过
        Assert.Equal(1, list.Count);
        Assert.Equal(6u, list.Find(6).Interval);
    }

    [Fact]
    public void LoadFromFile_重复MagicID时后续行被忽略()
    {
        string f = Path_("dup.txt");
        File.WriteAllText(f, "7=1\r\n7=2\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        Assert.Equal(1, list.Count);
        Assert.Equal(1u, list.Find(7).Interval);   // Add 返回 nil → 第二次赋值不生效
    }

    [Fact]
    public void LoadFromFile_先清空旧数据再加载()
    {
        string f = Path_("clear-first.txt");
        File.WriteAllText(f, "9=9\r\n", GBK);
        var list = new TMagicIntervalList();
        list.Add(1).Interval = 1;
        list.LoadFromFile(f);
        Assert.Equal(1, list.Count);
        Assert.Null(list.Find(1));                 // 原 :143 `Clear` 生效
        Assert.Equal(9u, list.Find(9).Interval);
    }

    // 原文缺陷 5（文件头 §5）：`nMagicID: Integer` 隐式收窄为 `Word`，$R- 下按低 16 位截断。
    [Fact]
    public void LoadFromFile_MagicIdAboveWordRange_TruncatesToLow16Bits()
    {
        string f = Path_("trunc.txt");
        File.WriteAllText(f, "65536=1\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        Assert.Equal(1, list.Count);
        Assert.Equal(0, list[0].MagicId);          // 65536 & 0xFFFF == 0
    }

    [Fact]
    public void LoadFromFile_超出Integer范围的Interval被当作无效()
    {
        string f = Path_("overflow.txt");
        File.WriteAllText(f, "1=3000000000\r\n2=2147483647\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        // StrToIntDef 溢出 → 返回 def(-1) → 第一行跳过；第二行是 Integer 上界，保留
        Assert.Equal(1, list.Count);
        Assert.Null(list.Find(1));
        Assert.Equal(2147483647u, list.Find(2).Interval);
    }

    [Fact]
    public void LoadFromFile_仅一个换行符的空文件得到空列表()
    {
        string f = Path_("onlylines.txt");
        File.WriteAllText(f, "\r\n\r\n", GBK);
        var list = new TMagicIntervalList();
        list.LoadFromFile(f);
        Assert.Equal(0, list.Count);
    }

    // ================= Save → Load 往返（原 :168-186 + :134-166） =================

    [Fact]
    public void SaveThenLoad_无损往返()
    {
        string f = Path_("roundtrip.txt");
        var src = new TMagicIntervalList();
        src.Add(0).Interval = 0;
        src.Add(1).Interval = 1;
        src.Add(65535).Interval = uint.MaxValue;
        src.SaveToFile(f);

        var dst = new TMagicIntervalList();
        dst.LoadFromFile(f);

        // ★ 已知差异（原文缺陷 5 的后果）：Interval 的 uint.MaxValue 写出去是
        //   4294967295，读回来时 `StrToIntDef` 只能给出 Integer → 越界 → 该行被整行跳过。
        //   所以往返对 Interval <= 2147483647 无损，对更大的值会**丢行**。
        Assert.Equal(2, dst.Count);
        Assert.Equal(0u, dst.Find(0).Interval);
        Assert.Equal(1u, dst.Find(1).Interval);
        Assert.Null(dst.Find(65535));
    }

    [Fact]
    public void SaveThenLoad_GBK中文路径可读写()
    {
        string f = Path_("中文目录-魔法CD.txt");
        var src = new TMagicIntervalList();
        src.Add(3).Interval = 33;
        src.SaveToFile(f);
        var dst = new TMagicIntervalList();
        dst.LoadFromFile(f);
        Assert.Equal(33u, dst.Find(3).Interval);
    }

    // ================= Lock / UnLock（原 :188-196） =================

    [Fact]
    public void Lock_UnLock可配对调用()
    {
        var list = new TMagicIntervalList();
        list.Lock();
        list.Add(1);
        list.UnLock();
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Lock_同一线程可重入_与CriticalSection一致()
    {
        var list = new TMagicIntervalList();
        list.Lock();
        list.Lock();                              // EnterCriticalSection 可重入
        list.Add(1);
        list.UnLock();
        list.UnLock();
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void UnLock_无匹配Lock时抛SynchronizationLockException()
    {
        // ★ 差异登记：原文 `LeaveCriticalSection` 在未持有时是未定义行为（不抛异常）；
        //   托管侧 Monitor.Exit 抛 SynchronizationLockException。属"无法 1:1"的运行时差异。
        var list = new TMagicIntervalList();
        Assert.Throws<SynchronizationLockException>(() => list.UnLock());
    }

    [Fact]
    public void Lock_跨线程互斥不破坏列表()
    {
        var list = new TMagicIntervalList();
        var errors = new List<Exception>();
        var threads = new List<System.Threading.Thread>();
        for (int t = 0; t < 4; t++)
        {
            int seed = t * 100;
            var th = new System.Threading.Thread(() =>
            {
                try
                {
                    for (int i = 0; i < 50; i++)
                    {
                        list.Lock();
                        try { list.Add((ushort)(seed + i)).Interval = (uint)i; }
                        finally { list.UnLock(); }
                    }
                }
                catch (Exception ex) { lock (errors) errors.Add(ex); }
            });
            threads.Add(th);
        }
        foreach (var th in threads) th.Start();
        foreach (var th in threads) th.Join();

        Assert.Empty(errors);
        Assert.Equal(200, list.Count);
    }
}
