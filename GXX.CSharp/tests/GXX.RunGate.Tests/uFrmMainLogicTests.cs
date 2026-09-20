// 测试：Source\RunGate\uFrmMain.pas（实测 LF 4216）的**纯逻辑切片**
//   → src/GXX.RunGate/uFrmMainLogic.cs
//
// 行号口径：物理 LF 行号（`[IO.File]::ReadAllText(p) -split "`n"`），与 read 工具一致；
//   `Get-Content`/`Select-String` 对该文件会漂移到 +17，不要混用。
using System;
using System.Collections.Generic;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

public sealed class uFrmMainLogicTests
{
    // ================= GetSizeString（原 :426-439） =================

    [Fact]
    public void GetSizeString_五档阈值()
    {
        Assert.Equal("1.00TB", RunGateMainLogic.GetSizeString(1099511627776L));   // TB 下界
        Assert.Equal("1.00GB", RunGateMainLogic.GetSizeString(1073741824L));
        Assert.Equal("1.00MB", RunGateMainLogic.GetSizeString(1048576L));
        Assert.Equal("1.00KB", RunGateMainLogic.GetSizeString(1024L));
        Assert.Equal("1023B", RunGateMainLogic.GetSizeString(1023L));
    }

    [Fact]
    public void GetSizeString_边界值与0负值()
    {
        Assert.Equal("0B", RunGateMainLogic.GetSizeString(0));
        Assert.Equal("-1B", RunGateMainLogic.GetSizeString(-1));
        Assert.Equal("1024.00KB", RunGateMainLogic.GetSizeString(1048576L - 1));     // 差 1 就降档
        Assert.Equal("1.50KB", RunGateMainLogic.GetSizeString(512L + 1024L));
    }

    [Fact]
    public void GetSizeString_两位小数与进位()
    {
        Assert.Equal("1.50KB", RunGateMainLogic.GetSizeString(1536L));
        Assert.Equal("2.00KB", RunGateMainLogic.GetSizeString(2047L));               // 四舍五入
        Assert.Equal("1.00MB", RunGateMainLogic.GetSizeString(1048576L));
        Assert.Equal("1.00TB", RunGateMainLogic.GetSizeString(1099511627776L));
    }

    [Fact]
    public void GetSizeString_极大值()
    {
        Assert.Equal("8388608.00TB", RunGateMainLogic.GetSizeString(long.MaxValue));
    }

    // ================= FormatRunTime（原 :2188-2207） =================

    [Fact]
    public void FormatRunTime_0到59秒只显示秒()
    {
        Assert.Equal("0秒", RunGateMainLogic.FormatRunTime(0));
        Assert.Equal("1秒", RunGateMainLogic.FormatRunTime(1000));
        Assert.Equal("59秒", RunGateMainLogic.FormatRunTime(59999));
    }

    [Fact]
    public void FormatRunTime_分钟档()
    {
        Assert.Equal("1分0秒", RunGateMainLogic.FormatRunTime(60000));
        Assert.Equal("1分1秒", RunGateMainLogic.FormatRunTime(61000));
        Assert.Equal("59分59秒", RunGateMainLogic.FormatRunTime(3599999));
    }

    [Fact]
    public void FormatRunTime_小时档()
    {
        Assert.Equal("1时0分0秒", RunGateMainLogic.FormatRunTime(3600000));
        Assert.Equal("23时59分59秒", RunGateMainLogic.FormatRunTime(86399999));
    }

    [Fact]
    public void FormatRunTime_天档()
    {
        Assert.Equal("1天0时0分0秒", RunGateMainLogic.FormatRunTime(86400000));
        Assert.Equal("2天3时4分5秒", RunGateMainLogic.FormatRunTime(2 * 86400000u + 3 * 3600000u + 4 * 60000u + 5000u));
    }

    [Fact]
    public void FormatRunTime_亚秒与回绕极大值()
    {
        Assert.Equal("0秒", RunGateMainLogic.FormatRunTime(999));                    // 不足 1 秒 → 0
        Assert.Equal("49天17时2分47秒", RunGateMainLogic.FormatRunTime(uint.MaxValue));
    }

    // ================= ExtractSelfTimeDateStamp（原 :2942-2965） =================

    /// <summary>造一个最小的 PE 头：e_lfanew=0x80，NT+8 处放 TimeDateStamp。</summary>
    private static byte[] BuildPeImage(uint timeDateStamp)
    {
        var img = new byte[0x100];
        BitConverter.GetBytes(0x80).CopyTo(img, 0x3C);          // e_lfanew
        BitConverter.GetBytes(0x00004550u).CopyTo(img, 0x80);   // 'PE\0\0' Signature
        BitConverter.GetBytes(timeDateStamp).CopyTo(img, 0x80 + 8);
        return img;
    }

    [Fact]
    public void UnixDateToDateTime_纪元与UTC加8()
    {
        // USec = 0 → 1970-01-01 00:00 UTC → +8h = 1970-01-01 08:00
        Assert.Equal(new DateTime(1970, 1, 1, 8, 0, 0), RunGateMainLogic.UnixDateToDateTime(0));
        // 一天后仍是 08:00
        Assert.Equal(new DateTime(1970, 1, 2, 8, 0, 0), RunGateMainLogic.UnixDateToDateTime(86400));
    }

    [Fact]
    public void UnixDateToDateTime_原文硬编码UTC加8_原文缺陷D8()
    {
        // ★ 缺陷 D8：`IncHour(Result, 8)` 无时区接缝；与 UTC 相差固定 8 小时
        var utc = new DateTime(1970, 1, 1, 0, 0, 0);
        Assert.Equal(utc.AddHours(8), RunGateMainLogic.UnixDateToDateTime(0));
        Assert.NotEqual(utc, RunGateMainLogic.UnixDateToDateTime(0));
    }

    [Fact]
    public void UnixDateToDateTime_负值()
    {
        // USec = -86400 → 1969-12-31 UTC → -8h = 1969-12-30 16:00
        // -86400 / 86400 = -1 → 25569 - 1 = 25568 天（1899-12-30 起）→ 1969-12-31，再 +8h
        Assert.Equal(new DateTime(1969, 12, 31, 8, 0, 0), RunGateMainLogic.UnixDateToDateTime(-86400));
    }

    [Fact]
    public void ExtractSelfTimeDateStamp_读NT头TimeDateStamp()
    {
        var img = BuildPeImage(0);                              // 1970-01-01
        Assert.Equal(new DateTime(1970, 1, 1, 8, 0, 0), RunGateMainLogic.ExtractSelfTimeDateStamp(img));
    }

    [Fact]
    public void ExtractSelfTimeDateStamp_e_lfanew参与偏移()
    {
        var img = new byte[0x200];
        BitConverter.GetBytes(0x100).CopyTo(img, 0x3C);
        BitConverter.GetBytes(86400u).CopyTo(img, 0x100 + 8);   // 1970-01-02
        Assert.Equal(new DateTime(1970, 1, 2, 8, 0, 0), RunGateMainLogic.ExtractSelfTimeDateStamp(img));
    }

    [Fact]
    public void ExtractSelfTimeDateStamp_影像过短或头越界返回MinValue()
    {
        Assert.Equal(DateTime.MinValue, RunGateMainLogic.ExtractSelfTimeDateStamp(null));
        Assert.Equal(DateTime.MinValue, RunGateMainLogic.ExtractSelfTimeDateStamp(Array.Empty<byte>()));
        Assert.Equal(DateTime.MinValue, RunGateMainLogic.ExtractSelfTimeDateStamp(new byte[0x20]));

        var bad = new byte[0x80];
        BitConverter.GetBytes(0x1000).CopyTo(bad, 0x3C);        // e_lfanew 指向影像之外
        Assert.Equal(DateTime.MinValue, RunGateMainLogic.ExtractSelfTimeDateStamp(bad));
    }

    [Fact]
    public void ExtractSelfTimeDateStampFromFile_文件不存在返回MinValue()
    {
        string p = Path.Combine(Path.GetTempPath(), "p2rg-nope-" + Guid.NewGuid().ToString("N") + ".bin");
        Assert.Equal(DateTime.MinValue, RunGateMainLogic.ExtractSelfTimeDateStampFromFile(p));
    }

    [Fact]
    public void ExtractSelfTimeDateStampFromFile_读真实文件()
    {
        string p = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(p, BuildPeImage(86400));
            Assert.Equal(new DateTime(1970, 1, 2, 8, 0, 0), RunGateMainLogic.ExtractSelfTimeDateStampFromFile(p));
        }
        finally { File.Delete(p); }
    }

    // ================= RecallPreAllocatedSize（原 :3837-3877） =================

    [Fact]
    public void Recall_循环上界含64且break判据()
    {
        // blockSize = 0 → 第一轮 Max=0 >= 0 → Break → Count = 0 + 0 + 36
        var r = RunGateMainLogic.RecallPreAllocatedSize(0, 1);
        Assert.Equal(RunGateMainLogic.OVERLAPPED_EX_SIZE, r.Count);
    }

    [Fact]
    public void Recall_blockSize为1KB时累加前9轮()
    {
        // blockSize = 1 → 阈值 = 1024；Max = I*128 → I=8 时 1024 >= 1024 → 在第 9 轮 Break
        var r = RunGateMainLogic.RecallPreAllocatedSize(1, 1);
        int expected = 0;
        for (int I = 0; I <= 8; I++) expected += (I << 7) + RunGateMainLogic.OVERLAPPED_EX_SIZE;
        Assert.Equal(expected, r.Count);
    }

    [Fact]
    public void Recall_含UseIocpClient的60MiB项_原文缺陷D10()
    {
        // ★ 缺陷 D10：UseIocpClient = 1 → 额外 + 1MiB × 60
        var withIocp = RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: true);
        var withoutIocp = RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: false);
        Assert.Equal(RunGateMainLogic.MAX_IOCP_CLIENT_RECV_BUFFER_SIZE * RunGateMainLogic.IOCP_CLIENT_RECV_BUFFER_COUNT,
                     withIocp.MemSize - withoutIocp.MemSize);
        Assert.Equal(1048576 * 60, withIocp.MemSize - withoutIocp.MemSize);
    }

    [Fact]
    public void Recall_乘预分配个数()
    {
        var one = RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: false);
        var three = RunGateMainLogic.RecallPreAllocatedSize(1, 3, useIocpClient: false);
        Assert.Equal(one.Count * 3, three.MemSize);
    }

    [Fact]
    public void Recall_大于等于400MiB判定为过大()
    {
        // 用 preAllocatedCount 放大到阈值附近
        var r = RunGateMainLogic.RecallPreAllocatedSize(0, 20_000_000, useIocpClient: false);
        Assert.True(r.MemSize >= RunGateMainLogic.PreAllocatedTooLargeThreshold);
        Assert.True(r.IsTooLarge);

        var small = RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: false);
        Assert.False(small.IsTooLarge);
    }

    [Fact]
    public void Recall_文本三档单位()
    {
        Assert.EndsWith("MBytes", RunGateMainLogic.RecallPreAllocatedSize(1, 100000).Text);
        Assert.EndsWith("KBytes", RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: false).Text);
        // 构造 < 1024 字节：overlappedExSize 取 0 且 blockSize 0 → Count = 0 → MemSize = 0
        var zero = RunGateMainLogic.RecallPreAllocatedSize(0, 1, overlappedExSize: 0, useIocpClient: false);
        Assert.Equal("0.00Bytes", zero.Text);
    }

    // ================= RecommendPreAllocatedCount（原 :3885-3899） =================

    [Fact]
    public void Recommend_按300MiB除以MemSize()
    {
        var r = RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: false);
        int expected = (300 << 20) / r.Count;
        Assert.Equal(expected, RunGateMainLogic.RecommendPreAllocatedCount(1));
    }

    [Fact]
    public void Recommend_不含UseIocpClient项()
    {
        // 若含 60MiB 项，推荐值会明显变小；原文这里没有那段 {$IF}
        var withLoopOnly = RunGateMainLogic.RecallPreAllocatedSize(1, 1, useIocpClient: false).Count;
        Assert.Equal((300 << 20) / withLoopOnly, RunGateMainLogic.RecommendPreAllocatedCount(1));
    }

    [Fact]
    public void Recommend_blockSize为0时退化为单轮()
    {
        int memSize = RunGateMainLogic.OVERLAPPED_EX_SIZE;              // I=0 → 0 + 36
        Assert.Equal((300 << 20) / memSize, RunGateMainLogic.RecommendPreAllocatedCount(0));
    }

    [Fact]
    public void Recommend_大blockSize会跑到上界64()
    {
        // blockSize 很大 → 永不 Break → I 到 64
        int memSize = 0;
        for (int I = 0; I <= 64; I++) memSize += (I << 7) + RunGateMainLogic.OVERLAPPED_EX_SIZE;
        Assert.Equal((300 << 20) / memSize, RunGateMainLogic.RecommendPreAllocatedCount(100000));
    }

    [Fact]
    public void Recommend_除零不可达因为MemSize至少36()
    {
        // 原文没有除零护栏，但 I=0 时必加 SizeOf(OVERLAPPEDEx)
        Assert.True(RunGateMainLogic.RecommendPreAllocatedCount(0) > 0);
        Assert.True(RunGateMainLogic.RecommendPreAllocatedCount(-1) > 0);
    }

    // ================= PosDelphi（原 :3427 / :3630-3633） =================

    [Fact]
    public void PosDelphi_1based且大小写敏感()
    {
        Assert.Equal(1, RunGateMainLogic.PosDelphi("a", "abc"));
        Assert.Equal(2, RunGateMainLogic.PosDelphi("b", "abc"));
        Assert.Equal(0, RunGateMainLogic.PosDelphi("A", "abc"));    // 大小写敏感
        Assert.Equal(0, RunGateMainLogic.PosDelphi("z", "abc"));
    }

    [Fact]
    public void PosDelphi_空needle命中一切_补偿DelphiRTL差异_原文缺陷D5()
    {
        // ★ 缺陷 D5：Delphi `Pos('', S) = 1`；`GXX.Core.Rtl.DelphiRTL.Pos("")` 返回 0 → 此处补偿
        Assert.Equal(1, RunGateMainLogic.PosDelphi("", "anything"));
        Assert.Equal(1, RunGateMainLogic.PosDelphi("", ""));
        Assert.Equal(1, RunGateMainLogic.PosDelphi(null, "x"));
    }

    [Fact]
    public void PosDelphi_空haystack()
    {
        Assert.Equal(0, RunGateMainLogic.PosDelphi("a", ""));
        Assert.Equal(0, RunGateMainLogic.PosDelphi("a", null));
    }

    // ================= 进程列表搜索（原 :3419-3466） =================

    [Fact]
    public void PrepareProcessSearchKey_Trim与UpperCase()
    {
        Assert.Equal("ABC", RunGateMainLogic.PrepareProcessSearchKey("  abc  "));
        Assert.Equal("A B", RunGateMainLogic.PrepareProcessSearchKey("\ta b\n"));
    }

    [Fact]
    public void PrepareProcessSearchKey_空串返回null表示Exit()
    {
        Assert.Null(RunGateMainLogic.PrepareProcessSearchKey(""));
        Assert.Null(RunGateMainLogic.PrepareProcessSearchKey("   "));
        Assert.Null(RunGateMainLogic.PrepareProcessSearchKey(null));
    }

    [Fact]
    public void ProcessRowMatches_大小写不敏感的子串匹配()
    {
        string key = RunGateMainLogic.PrepareProcessSearchKey("explorer");
        Assert.True(RunGateMainLogic.ProcessRowMatches("C:\\Windows\\EXPLORER.EXE", key));
        Assert.False(RunGateMainLogic.ProcessRowMatches("C:\\Windows\\notepad.exe", key));
    }

    [Fact]
    public void ProcessRowMatches_原文缺陷D2的空Selected读取不影响判定()
    {
        // ★ 缺陷 D2：`ListItem.Selected;` 是空读取（无副作用）→ 判定只取决于 Caption
        string key = RunGateMainLogic.PrepareProcessSearchKey("a");
        Assert.True(RunGateMainLogic.ProcessRowMatches("a", key));
        Assert.False(RunGateMainLogic.ProcessRowMatches("", key));
    }

    [Fact]
    public void ProcessNextSearchStartIndex_原文缺陷D1的off_by_one()
    {
        // 入参语义：原文 `btnNextSearchClick` 直接消费字段 `FSearchIndex`（初值 0，
        // 只会被写成 `I + 1` 或 0），本函数即"修正后的起始下标"。
        // ★ 缺陷 D1：判据 `>= Count - 1`（应为 `>= Count`）
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(0, 5));    // 0 >= 4 为假 → 原样 0
        Assert.Equal(1, RunGateMainLogic.ProcessNextSearchStartIndex(1, 5));
        Assert.Equal(3, RunGateMainLogic.ProcessNextSearchStartIndex(3, 5));
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(4, 5));    // 4 >= 4 → 0（★ 丢失最后一行）
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(9, 5));
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(0, 0));    // 0 >= -1 → 0
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(0, 1));    // 0 >= 0 → 0
        // ★ 差异：负值直接透传（原文会用它做 `Items[-1]` → EListError）。
        //   实践中不可达：FSearchIndex 初值 0，只被赋 `I + 1` 或 0。
        Assert.Equal(-1, RunGateMainLogic.ProcessNextSearchStartIndex(-1, 5));
    }

    [Fact]
    public void ProcessNextSearchStartIndex_最后一行无法被搜到()
    {
        // 3 行：第 0 行命中后 FSearchIndex = 1 → 从 1 继续
        Assert.Equal(1, RunGateMainLogic.ProcessNextSearchStartIndex(1, 3));
        // 第 1 行命中后 FSearchIndex = 2 → 2 >= 2 → 被重置为 0（★ 缺陷 D1）
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(2, 3));
        // 于是"下一个"永远从第 0 行重扫 —— 只要第 0 行也命中，就又回到第 0 行，
        // 索引 2（最后一行）只能在"第一次搜索"或"从 1 开始扫"时被扫到。
        Assert.Equal(0, RunGateMainLogic.ProcessNextSearchStartIndex(3, 3));
    }

    // ================= 在线用户搜索（原 :3612-3766） =================

    private static List<string> SubItems(params string[] s) => new List<string>(s);

    [Fact]
    public void OnlineSubItemIndexForField_四个字段映射()
    {
        Assert.Equal(2, RunGateMainLogic.OnlineSubItemIndexForField(RunGateMainLogic.OnlineFieldAccount));
        Assert.Equal(3, RunGateMainLogic.OnlineSubItemIndexForField(RunGateMainLogic.OnlineFieldName));
        Assert.Equal(0, RunGateMainLogic.OnlineSubItemIndexForField(RunGateMainLogic.OnlineFieldIP));
        Assert.Equal(4, RunGateMainLogic.OnlineSubItemIndexForField(RunGateMainLogic.OnlineFieldMac));
        Assert.Equal(-1, RunGateMainLogic.OnlineSubItemIndexForField(-1));
        Assert.Equal(-1, RunGateMainLogic.OnlineSubItemIndexForField(4));      // 原文 case 无 else
        Assert.Equal(-1, RunGateMainLogic.OnlineSubItemIndexForField(99));
    }

    [Fact]
    public void OnlineRowMatches_SubItems不足5个时不匹配_原文缺陷D4()
    {
        // ★ 缺陷 D4：前置条件是 `Item.SubItems.Count > 4`
        var four = SubItems("1.1.1.1", "7200", "acc", "name");
        Assert.False(RunGateMainLogic.OnlineRowMatches(four, "acc", RunGateMainLogic.OnlineFieldAccount, true));
        Assert.False(RunGateMainLogic.OnlineRowMatches(four, "acc", RunGateMainLogic.OnlineFieldAccount, false));
        var empty = SubItems();
        Assert.False(RunGateMainLogic.OnlineRowMatches(empty, "x", RunGateMainLogic.OnlineFieldIP, true));
        Assert.False(RunGateMainLogic.OnlineRowMatches(null, "x", RunGateMainLogic.OnlineFieldIP, true));
    }

    [Fact]
    public void OnlineRowMatches_模糊匹配大小写敏感_原文缺陷D5()
    {
        var row = SubItems("1.1.1.1", "7200", "Account1", "Hero", "AA-BB");
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "ccount", RunGateMainLogic.OnlineFieldAccount, true));
        Assert.False(RunGateMainLogic.OnlineRowMatches(row, "CCount", RunGateMainLogic.OnlineFieldAccount, true));  // 大小写敏感
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "", RunGateMainLogic.OnlineFieldAccount, true));          // 空 needle 命中一切
    }

    [Fact]
    public void OnlineRowMatches_精确匹配大小写不敏感全等_原文缺陷D6()
    {
        var row = SubItems("1.1.1.1", "7200", "Account1", "Hero", "AA-BB");
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "account1", RunGateMainLogic.OnlineFieldAccount, false));
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "ACCOUNT1", RunGateMainLogic.OnlineFieldAccount, false));
        Assert.False(RunGateMainLogic.OnlineRowMatches(row, "ccount", RunGateMainLogic.OnlineFieldAccount, false));  // 非全等
        Assert.False(RunGateMainLogic.OnlineRowMatches(row, "", RunGateMainLogic.OnlineFieldAccount, false));        // 空 ≠ 全等
    }

    [Fact]
    public void OnlineRowMatches_字段映射到不同SubItems()
    {
        var row = SubItems("10.0.0.1", "7200", "acc", "hero", "MAC-1");
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "10.0.0.1", RunGateMainLogic.OnlineFieldIP, false));
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "hero", RunGateMainLogic.OnlineFieldName, false));
        Assert.True(RunGateMainLogic.OnlineRowMatches(row, "MAC-1", RunGateMainLogic.OnlineFieldMac, false));
        Assert.False(RunGateMainLogic.OnlineRowMatches(row, "10.0.0.1", RunGateMainLogic.OnlineFieldName, false));
    }

    [Fact]
    public void OnlineNextSearchStartIndex_两次重置()
    {
        Assert.Equal(0, RunGateMainLogic.OnlineNextSearchStartIndex(-1, 5));   // -1 → 0+1=0？不：-1+1=0 → 不重置
        Assert.Equal(0, RunGateMainLogic.OnlineNextSearchStartIndex(-2, 5));   // -2+1=-1 < 0 → 0
        Assert.Equal(2, RunGateMainLogic.OnlineNextSearchStartIndex(1, 5));
        Assert.Equal(0, RunGateMainLogic.OnlineNextSearchStartIndex(4, 5));    // 5 >= 5 → 0
        Assert.Equal(0, RunGateMainLogic.OnlineNextSearchStartIndex(9, 5));
        Assert.Equal(0, RunGateMainLogic.OnlineNextSearchStartIndex(0, 0));    // 1 >= 0 → 0
    }

    // ================= pmProcessListPopup（原 :3777-3794） =================

    [Fact]
    public void ProcessPopup_无选中时全隐藏()
    {
        var v = RunGateMainLogic.GetProcessPopupVisibility(false, SubItems("a", "b"), true);
        Assert.False(v.AddBlackProcess);
        Assert.False(v.SendFileToRungate);
        Assert.False(v.SetRoot);
    }

    [Fact]
    public void ProcessPopup_无选中但SubItems有值也不能显示SetRoot()
    {
        // 原 :3785 `(ListItem <> nil) and (ListItem.SubItems[0] <> '')` → ListItem = nil 时必为 False
        var v = RunGateMainLogic.GetProcessPopupVisibility(false, SubItems("root"), false);
        Assert.False(v.SetRoot);
    }

    [Fact]
    public void ProcessPopup_SubItems不足2个走早退分支()
    {
        var v = RunGateMainLogic.GetProcessPopupVisibility(true, SubItems("root"), true);
        Assert.False(v.AddBlackProcess);
        Assert.False(v.SendFileToRungate);
        Assert.True(v.SetRoot);                                   // 原 :3785 SubItems[0] <> ''
    }

    [Fact]
    public void ProcessPopup_SubItems1为空走早退分支()
    {
        var v = RunGateMainLogic.GetProcessPopupVisibility(true, SubItems("root", ""), true);
        Assert.False(v.AddBlackProcess);
        Assert.False(v.SendFileToRungate);
        Assert.True(v.SetRoot);
    }

    [Fact]
    public void ProcessPopup_进程列表模式()
    {
        var v = RunGateMainLogic.GetProcessPopupVisibility(true, SubItems("c:\\a.exe", "md5"), true);
        Assert.True(v.AddBlackProcess);                           // 原 :3791
        Assert.False(v.SendFileToRungate);
        Assert.False(v.SetRoot);                                  // 原 :3793 `not FIsProcessList and ...`
    }

    [Fact]
    public void ProcessPopup_目录列表模式()
    {
        var v = RunGateMainLogic.GetProcessPopupVisibility(true, SubItems("D:\\dir", "attr"), false);
        Assert.False(v.AddBlackProcess);
        Assert.True(v.SendFileToRungate);
        Assert.True(v.SetRoot);
    }

    [Fact]
    public void ProcessPopup_SubItems0为空时SetRoot为假()
    {
        var v = RunGateMainLogic.GetProcessPopupVisibility(true, SubItems("", "attr"), false);
        Assert.True(v.SendFileToRungate);
        Assert.False(v.SetRoot);
    }

    [Fact]
    public void ProcessPopup_SubItems为空时托管侧安全返回_原文缺陷D3()
    {
        // ★ 缺陷 D3：Delphi 的 `ListItem.SubItems[0]` 在 Count = 0 时抛 EListError；
        //   托管侧 `SubItemOrEmpty` 返回 "" → 早退分支的 SetRoot = true and "" <> "" = False（不抛）
        var v = RunGateMainLogic.GetProcessPopupVisibility(true, SubItems(), true);
        Assert.False(v.SetRoot);
        Assert.Null(Record.Exception(() => RunGateMainLogic.GetProcessPopupVisibility(true, SubItems(), true)));
    }

    [Fact]
    public void SubItemOrEmpty_越界与null()
    {
        Assert.Equal("a", RunGateMainLogic.SubItemOrEmpty(SubItems("a"), 0));
        Assert.Equal("", RunGateMainLogic.SubItemOrEmpty(SubItems("a"), 1));
        Assert.Equal("", RunGateMainLogic.SubItemOrEmpty(SubItems("a"), -1));
        Assert.Equal("", RunGateMainLogic.SubItemOrEmpty(null, 0));
        Assert.Equal("", RunGateMainLogic.SubItemOrEmpty(SubItems("a", null), 1));
    }
}
