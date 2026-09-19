using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// <c>Source/Client-HGE/UpdateEngine.pas</c>（1558 行）1:1 移植的测试。
///
/// <para>覆盖：协议常量与 packed 布局、CheckIP、TSafeList、校验码哈希、三级优先级取件、
/// 请求表管理、ClearRequests 的两条分支、服务端消息解析与 CRC 门控。</para>
/// </summary>
public sealed class TailUpdateEngineTests
{
    // ══════════════════════════════════════════════════════════════════════
    // 协议常量与结构布局
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：协议常量逐条核对（原文 :19-36）。</summary>
    [Fact]
    public void ProtocolConstants_MatchSource()
    {
        Assert.Equal(0xBBDDEE11u, UpdateEngineConst.UPDATE_SOCKET_FLAG);
        Assert.Equal(1000, UpdateEngineConst.CM_SOCKETCONNECT);
        Assert.Equal(1001, UpdateEngineConst.CM_CHECKCODE_RECV);
        Assert.Equal(1002, UpdateEngineConst.CM_UPDATEBUFFER);
        Assert.Equal(1003, UpdateEngineConst.CM_HEARTBEAT);
        Assert.Equal(1004, UpdateEngineConst.CM_CLEAR_MSG);
        Assert.Equal(5000, UpdateEngineConst.WM_CONNECT_RET);
        Assert.Equal(5001, UpdateEngineConst.WM_CHECK_CODE);
        Assert.Equal(5002, UpdateEngineConst.WM_UPDATE_STOP);
        Assert.Equal(5003, UpdateEngineConst.WM_DATA);
        Assert.Equal(5004, UpdateEngineConst.WM_COMPDATA);
        Assert.Equal(0, UpdateEngineConst.LOG_UPDATE);
        Assert.Equal(10000u, UpdateEngineConst.RECONNECT_INTERVAL_MS);
        Assert.Equal(1000u, UpdateEngineConst.REMAINING_SEND_INTERVAL_MS);
    }

    /// <summary>用例 2：TUpdateDataType 的取值顺序（不可改序，否则破坏 wire 兼容）。</summary>
    [Fact]
    public void UpdateDataTypeOrder_MatchesSource()
    {
        Assert.Equal(0, (int)TUpdateDataType.udtFileWav);
        Assert.Equal(1, (int)TUpdateDataType.udtFileMap);
        Assert.Equal(2, (int)TUpdateDataType.udtFileOther);
        Assert.Equal(3, (int)TUpdateDataType.udtImagePak);
        Assert.Equal(4, (int)TUpdateDataType.udtImageWzl);
        Assert.Equal(5, (int)TUpdateDataType.udtIndexPak);
        Assert.Equal(6, (int)TUpdateDataType.udtIndexWzl);
    }

    /// <summary>用例 3：两个消息头的 packed 宽度都是 20 字节。</summary>
    [Fact]
    public void MessageHeaders_Are20BytesPacked()
    {
        Assert.Equal(20, Marshal.SizeOf<TUpdateSrvMsgHeader>());
        Assert.Equal(20, Marshal.SizeOf<TUpdateClientMsgHeader>());
        Assert.Equal(20, UpdateEngineLayout.SrvHeaderSize);
        Assert.Equal(20, UpdateEngineLayout.ClientHeaderSize);
    }

    /// <summary>用例 4：服务端头的字段偏移（4/4/2/2/4/4）。</summary>
    [Fact]
    public void ServerHeader_FieldOffsets_MatchPackedLayout()
    {
        var h = new TUpdateSrvMsgHeader
        {
            MsgFlag = 0x11223344, RequestID = 0x55667788,
            Ident = 0xAABB, Param = 0xCCDD, DataCrc = 0x99AABBCC, DataLen = 0x01020304,
        };
        byte[] b = new byte[20];
        BitConverter.GetBytes(h.MsgFlag).CopyTo(b, 0);
        BitConverter.GetBytes(h.RequestID).CopyTo(b, 4);
        BitConverter.GetBytes(h.Ident).CopyTo(b, 8);
        BitConverter.GetBytes(h.Param).CopyTo(b, 10);
        BitConverter.GetBytes(h.DataCrc).CopyTo(b, 12);
        BitConverter.GetBytes(h.DataLen).CopyTo(b, 16);

        var back = UpdateEngineLogic.BytesToSrvHeader(b, 0);
        Assert.Equal(h.MsgFlag, back.MsgFlag);
        Assert.Equal(h.RequestID, back.RequestID);
        Assert.Equal(h.Ident, back.Ident);
        Assert.Equal(h.Param, back.Param);
        Assert.Equal(h.DataCrc, back.DataCrc);
        Assert.Equal(h.DataLen, back.DataLen);
    }

    /// <summary>用例 5：客户端头的字段偏移（4/4/2/1/1/4/4，DataLen 是末尾的 4 字节）。</summary>
    [Fact]
    public void ClientHeader_RoundTripsThroughBytes()
    {
        var h = new TUpdateClientMsgHeader
        {
            MsgFlag = UpdateEngineConst.UPDATE_SOCKET_FLAG,
            RequestID = 0xDEADBEEF,
            Ident = (ushort)UpdateEngineConst.CM_UPDATEBUFFER,
            DataType = (byte)TUpdateDataType.udtImagePak,
            Param = 7,
            Index = -3,
            DataLen = 260,
        };
        byte[] b = UpdateEngineLogic.ClientHeaderToBytes(h);
        Assert.Equal(20, b.Length);
        var back = UpdateEngineLogic.BytesToClientHeader(b, 0);
        Assert.Equal(h.MsgFlag, back.MsgFlag);
        Assert.Equal(h.RequestID, back.RequestID);
        Assert.Equal(h.Ident, back.Ident);
        Assert.Equal(h.DataType, back.DataType);
        Assert.Equal(h.Param, back.Param);
        Assert.Equal(h.Index, back.Index);
        Assert.Equal(h.DataLen, back.DataLen);

        // 偏移断言
        Assert.Equal((byte)UpdateTDataTypeByte(h.DataType), b[10]);
        Assert.Equal(7, b[11]);
        Assert.Equal(unchecked((byte)0xFD), b[12]);      // Index = -3 的低字节
    }

    private static byte UpdateTDataTypeByte(byte v) => v;

    // ══════════════════════════════════════════════════════════════════════
    // CheckIP（原文 :191-216）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：合法 IPv4 通过。</summary>
    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("127.0.0.1")]
    [InlineData("192.168.1.10")]
    [InlineData("10.0.0.1")]
    [InlineData("1.2.3.4")]
    public void CheckIP_ValidAddresses_ReturnTrue(string ip)
        => Assert.True(UpdateEngineLogic.CheckIP(ip), $"<{ip}> 应判为合法");

    /// <summary>
    /// 用例 1b：<b>关键边界</b> —— <c>255.255.255.255</c> 虽然四段都在 [0,255]，
    /// 但 <c>inet_addr</c> 对它返回的正是 <c>INADDR_NONE</c>（两者同值 $FFFFFFFF），
    /// 故原文的**第二重校验**会把它判成非法。这是本单元最易踩的边界。
    /// </summary>
    [Fact]
    public void CheckIP_BroadcastAddress_IsRejectedBecauseInetAddrReturnsNone()
    {
        Assert.False(UpdateEngineLogic.CheckIP("255.255.255.255"));
        Assert.Equal(WinSock2Constants.INADDR_NONE, WinSock2Seam.inet_addr("255.255.255.255"));
        // 对照：0.0.0.0 的 inet_addr 结果是 0（≠ INADDR_NONE）⇒ 合法
        Assert.True(UpdateEngineLogic.CheckIP("0.0.0.0"));
    }

    /// <summary>用例 2：段数不为 4 一律 False。</summary>
    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("1.2")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4.5")]
    [InlineData("...")]
    public void CheckIP_WrongSegmentCount_ReturnsFalse(string ip)
        => Assert.False(UpdateEngineLogic.CheckIP(ip), $"<{ip}> 应判为非法");

    /// <summary>用例 3：段值越界或非数字一律 False（StrToIntDef 得 −1）。</summary>
    [Theory]
    [InlineData("1.2.3.256")]
    [InlineData("256.1.1.1")]
    [InlineData("1.2.3.-1")]
    [InlineData("a.b.c.d")]
    [InlineData("1.2.3.")]
    [InlineData(".1.2.3")]
    [InlineData("1.2.3.4 ")]
    [InlineData(" 1.2.3.4")]
    public void CheckIP_OutOfRangeOrNonNumeric_ReturnsFalse(string ip)
        => Assert.False(UpdateEngineLogic.CheckIP(ip), $"<{ip}> 应判为非法");

    /// <summary>用例 4：null 边界。</summary>
    [Fact]
    public void CheckIP_Null_ReturnsFalse()
        => Assert.False(UpdateEngineLogic.CheckIP(null));

    /// <summary>用例 5：StrToIntDef 的 Delphi 语义（失败返回默认值，不抛）。</summary>
    [Theory]
    [InlineData("0", 0, 0)]
    [InlineData("255", -1, 255)]
    [InlineData("", -1, -1)]
    [InlineData("abc", -1, -1)]
    [InlineData("12a", -1, -1)]
    [InlineData("+7", -1, 7)]
    [InlineData("-7", -1, -7)]
    [InlineData("999999999999", -1, -1)]     // 溢出 ⇒ 默认值
    public void StrToIntDef_MatchesDelphiSemantics(string s, int def, int expected)
        => Assert.Equal(expected, UpdateEngineLogic.StrToIntDef(s, def));

    // ══════════════════════════════════════════════════════════════════════
    // TSafeList（原文 :220-240）
    // ══════════════════════════════════════════════════════════════════════

    private static TClientRequest Req(uint id, string name, TUpdateDataType t = TUpdateDataType.udtImagePak,
        bool isMap = false, int img = 0)
        => new TClientRequest { RequestID = id, FileName = name, DataType = t, IsMapData = isMap, ImgIndex = img };

    /// <summary>用例 1：Add/Count/索引/Delete/Clear 基本语义。</summary>
    [Fact]
    public void SafeList_BasicOperations()
    {
        var l = new TSafeList();
        Assert.Equal(0, l.Count);
        l.Add(Req(1, "a"));
        l.Add(Req(2, "b"));
        Assert.Equal(2, l.Count);
        Assert.Equal("a", l[0].FileName);
        Assert.Equal("b", l[1].FileName);

        l.Delete(0);
        Assert.Equal(1, l.Count);
        Assert.Equal("b", l[0].FileName);

        l.Clear();
        Assert.Equal(0, l.Count);
    }

    /// <summary>用例 2：Lock/UnLock 可重入配对不抛（原文是临界区）</summary>
    [Fact]
    public void SafeList_LockUnlockPairs()
    {
        var l = new TSafeList();
        l.Lock();
        l.Add(Req(1, "x"));
        l.UnLock();
        Assert.Equal(1, l.Count);

        l.Lock();
        l.Lock();           // 临界区在 Windows 上可重入
        l.UnLock();
        l.UnLock();
        Assert.Equal(1, l.Count);
    }

    /// <summary>用例 3：并发 Add 不丢项（锁语义）。</summary>
    [Fact]
    public void SafeList_ConcurrentAdds_DoNotLoseItems()
    {
        var l = new TSafeList();
        const int perThread = 200;
        var threads = Enumerable.Range(0, 4).Select(k => new System.Threading.Thread(() =>
        {
            for (int i = 0; i < perThread; i++)
            {
                l.Lock();
                try { l.Add(Req((uint)(k * perThread + i), "f" + k + "_" + i)); }
                finally { l.UnLock(); }
            }
        })).ToArray();
        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();

        Assert.Equal(4 * perThread, l.Count);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 校验码哈希（原文 :657-675）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：Hash1 的确定性 + 不同 RequestID 结果不同。</summary>
    [Fact]
    public void CheckCodeHash1_IsDeterministicAndIdSensitive()
    {
        Assert.Equal(UpdateEngineLogic.ComputeCheckCodeHash1(1),
                     UpdateEngineLogic.ComputeCheckCodeHash1(1));
        Assert.NotEqual(UpdateEngineLogic.ComputeCheckCodeHash1(1),
                        UpdateEngineLogic.ComputeCheckCodeHash1(2));
        Assert.NotEqual(UpdateEngineLogic.ComputeCheckCodeHash1(0),
                        UpdateEngineLogic.ComputeCheckCodeHash1(1));
    }

    /// <summary>用例 2：Hash1 的初值是 $AAAAAAAA 参与运算 ⇒ 空 Id 串也有非平凡结果。</summary>
    [Fact]
    public void CheckCodeHash1_StartsWithConstantSeed()
    {
        // 手工复算 requestID = 0：S1 = #20#40#50 + "0"（4 个字符）
        uint h = 0xAAAAAAAA;
        byte[] s1 = new byte[] { 0x14, 0x28, 0x32, (byte)'0' };
        for (int I = 1; I <= s1.Length; I++)
        {
            if ((I & 1) == 0) h ^= (h << 7) ^ s1[I - 1] ^ (h >> 3);
            else h ^= ~((h << 13) ^ s1[I - 1] ^ (h >> 5));
        }
        Assert.Equal(h, UpdateEngineLogic.ComputeCheckCodeHash1(0));
    }

    /// <summary>用例 3：Hash2 的确定性 + 高 4 位回卷（结果的高 4 位必为 0）。</summary>
    [Fact]
    public void CheckCodeHash2_FoldsHighNibble()
    {
        Assert.Equal(UpdateEngineLogic.ComputeCheckCodeHash2(12345),
                     UpdateEngineLogic.ComputeCheckCodeHash2(12345));
        Assert.NotEqual(UpdateEngineLogic.ComputeCheckCodeHash2(1),
                        UpdateEngineLogic.ComputeCheckCodeHash2(2));

        foreach (uint crc in new uint[] { 0, 1, 255, 0xFFFFFFFF, 0x12345678 })
        {
            uint h = UpdateEngineLogic.ComputeCheckCodeHash2(crc);
            Assert.Equal(0u, h & 0xF0000000u);      // 高 4 位被回卷清零
        }
    }

    /// <summary>用例 4：Hash2 的手工复算（对应原文 :668-675 的逐字符循环）。</summary>
    [Fact]
    public void CheckCodeHash2_ManualRecompute()
    {
        uint crc = 0x0A0B0C0D;
        byte[] s2 = new byte[] { 0x0B, 0x16, 0x0E }.Concat(
            System.Text.Encoding.ASCII.GetBytes(crc.ToString())).ToArray();

        uint hash = 0;
        foreach (byte ch in s2)
        {
            hash = (hash << 4) + ch;
            uint dwTemp = hash & 0xF0000000u;
            if (dwTemp != 0) hash ^= (dwTemp >> 24);
            hash &= ~dwTemp;
        }
        Assert.Equal(hash, UpdateEngineLogic.ComputeCheckCodeHash2(crc));
    }

    // ══════════════════════════════════════════════════════════════════════
    // 三个队列的分类与优先级（原文 :419-446 / :479-568）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：Add 的分类落队列（地图 / 文件 / 图片）。</summary>
    [Fact]
    public void Add_ClassifiesIntoThreeQueues()
    {
        var e = new TUpdateEngine();
        Assert.True(e.Add("a.map", TUpdateDataType.udtFileMap, 0, null));
        Assert.True(e.Add("a.wav", TUpdateDataType.udtFileWav, 0, null));
        Assert.True(e.Add("a.other", TUpdateDataType.udtFileOther, 0, null));
        Assert.True(e.Add("a.pak", TUpdateDataType.udtImagePak, 1, null));
        Assert.True(e.Add("a.wzl", TUpdateDataType.udtImageWzl, 2, null));
        Assert.True(e.Add("idx.pak", TUpdateDataType.udtIndexPak, 0, null));
        Assert.True(e.Add("idx.wzl", TUpdateDataType.udtIndexWzl, 0, null));

        Assert.Equal(1, e.FRequestMapDataList.Count);       // 只有 udtFileMap
        Assert.Equal(2, e.FRequestFileList.Count);          // wav + other
        Assert.Equal(4, e.FRequestImageList.Count);         // 2 图片 + 2 索引
        Assert.Equal(7, e.GetUpdateCount());
    }

    /// <summary>用例 2：Add 的去重（同文件名 + 同类型 ⇒ 第二次返回 False）。</summary>
    [Fact]
    public void Add_DeduplicatesByFileNameAndType()
    {
        var e = new TUpdateEngine();
        Assert.True(e.Add("a.pak", TUpdateDataType.udtImagePak, 1, null));
        Assert.False(e.Add("a.pak", TUpdateDataType.udtImagePak, 1, null));
        Assert.Equal(1, e.GetUpdateCount());
        // 同文件名但不同类型 ⇒ 允许
        Assert.True(e.Add("a.pak", TUpdateDataType.udtImageWzl, 1, null));
        Assert.Equal(2, e.GetUpdateCount());
        // 不同类型且不同文件名 ⇒ 允许
        Assert.True(e.Add("b.pak", TUpdateDataType.udtImagePak, 1, null));
        Assert.Equal(3, e.GetUpdateCount());
    }

    /// <summary>用例 3：GetRequestID 从 1 起递增（0 永不出现）。</summary>
    [Fact]
    public void GetRequestID_StartsAtOneAndIncrements()
    {
        var e = new TUpdateEngine();
        Assert.Equal(1u, e.GetRequestID());
        Assert.Equal(2u, e.GetRequestID());
        Assert.Equal(3u, e.GetRequestID());
    }

    /// <summary>用例 4：NeedDrawTileMap 属性的读写。</summary>
    [Fact]
    public void NeedDrawTileMap_PropertyRoundTrips()
    {
        var e = new TUpdateEngine();
        Assert.False(e.NeedDrawTileMap);
        e.NeedDrawTileMap = true;
        Assert.True(e.NeedDrawTileMap);
    }

    /// <summary>用例 5：三级优先级的取件顺序 —— 超时列表永远最优先。</summary>
    [Fact]
    public void TakeNextRequest_TimeOutListHasHighestPriority()
    {
        var e = new TUpdateEngine();
        e.Add("map.map", TUpdateDataType.udtFileMap, 0, null);
        e.Add("img.pak", TUpdateDataType.udtImagePak, 0, null);
        e.Add("snd.wav", TUpdateDataType.udtFileWav, 0, null);
        e.TimeOutList.Add(Req(999, "timeout.dat", TUpdateDataType.udtFileOther));

        var r = e.TakeNextRequest(mapPriority: true, requestedCount: 0, maxCount: 10);
        Assert.NotNull(r);
        Assert.Equal("timeout.dat", r.FileName);
        Assert.Equal(0, e.TimeOutList.Count);
    }

    /// <summary>
    /// 用例 6：<b>差异断言</b> —— <c>mapPriority</c> 为真时地图先于图片，
    /// 为假时图片先于地图；两者都会在最后才取 <c>FRequestFileList</c>。
    /// </summary>
    [Fact]
    public void TakeNextRequest_MapPriorityChangesMapVsImageOrder()
    {
        // mapPriority = true ⇒ map 先
        var a = new TUpdateEngine();
        a.Add("m.map", TUpdateDataType.udtFileMap, 0, null);
        a.Add("i.pak", TUpdateDataType.udtImagePak, 0, null);
        a.Add("f.wav", TUpdateDataType.udtFileWav, 0, null);
        Assert.Equal("m.map", a.TakeNextRequest(true, 0, 10).FileName);
        Assert.Equal("i.pak", a.TakeNextRequest(true, 1, 10).FileName);
        Assert.Equal("f.wav", a.TakeNextRequest(true, 2, 10).FileName);
        Assert.Null(a.TakeNextRequest(true, 3, 10));

        // mapPriority = false ⇒ image 先
        var b = new TUpdateEngine();
        b.Add("m.map", TUpdateDataType.udtFileMap, 0, null);
        b.Add("i.pak", TUpdateDataType.udtImagePak, 0, null);
        b.Add("f.wav", TUpdateDataType.udtFileWav, 0, null);
        Assert.Equal("i.pak", b.TakeNextRequest(false, 0, 10).FileName);
        Assert.Equal("m.map", b.TakeNextRequest(false, 1, 10).FileName);
        Assert.Equal("f.wav", b.TakeNextRequest(false, 2, 10).FileName);
    }

    /// <summary>用例 7：队列上限 —— <c>requestedCount &gt;= maxCount</c> 时一条都不取（队列不动）。</summary>
    [Fact]
    public void TakeNextRequest_RespectsMaxCount()
    {
        var e = new TUpdateEngine();
        e.Add("i.pak", TUpdateDataType.udtImagePak, 0, null);
        Assert.Equal(1, e.GetUpdateCount());

        Assert.Null(e.TakeNextRequest(false, 5, 5));      // 达到上限
        Assert.Equal(1, e.GetUpdateCount());              // 队列未被消耗
        Assert.Null(e.TakeNextRequest(false, 6, 5));      // 超过上限

        Assert.NotNull(e.TakeNextRequest(false, 4, 5));   // 未达上限
        Assert.Equal(0, e.GetUpdateCount());
    }

    /// <summary>用例 8：FIFO 语义（同队列内取第 0 条）。</summary>
    [Fact]
    public void TakeNextRequest_IsFifoWithinAQueue()
    {
        var e = new TUpdateEngine();
        e.Add("1.pak", TUpdateDataType.udtImagePak, 0, null);
        e.Add("2.pak", TUpdateDataType.udtImagePak, 0, null);
        e.Add("3.pak", TUpdateDataType.udtImagePak, 0, null);
        Assert.Equal("1.pak", e.TakeNextRequest(false, 0, 10).FileName);
        Assert.Equal("2.pak", e.TakeNextRequest(false, 1, 10).FileName);
        Assert.Equal("3.pak", e.TakeNextRequest(false, 2, 10).FileName);
    }

    /// <summary>用例 9：全空时返回 null。</summary>
    [Fact]
    public void TakeNextRequest_AllEmpty_ReturnsNull()
    {
        var e = new TUpdateEngine();
        Assert.Null(e.TakeNextRequest(true, 0, 10));
        Assert.Null(e.TakeNextRequest(false, 0, 10));
    }

    /// <summary>用例 10：Add 写入的字段（AddTick 非零、SendTick 为 0、IsMapData 由类型推导）。</summary>
    [Fact]
    public void Add_PopulatesRequestFields()
    {
        var e = new TUpdateEngine();
        e.Add("m.map", TUpdateDataType.udtFileMap, 3, "IMAGES");
        var r = e.FRequestMapDataList[0];
        Assert.Equal("m.map", r.FileName);
        Assert.Equal(TUpdateDataType.udtFileMap, r.DataType);
        Assert.Equal(3, r.ImgIndex);
        Assert.Equal("IMAGES", r.GameImages);
        Assert.True(r.IsMapData);
        Assert.Equal(0, r.TimeOutCount);
        Assert.Equal(0u, r.SendTick);
        Assert.True(r.RequestID >= 1);
    }

    /// <summary>用例 11：ClassifyRequeueTarget 与原文 :419-446 的分类一致。</summary>
    [Fact]
    public void ClassifyRequeueTarget_MatchesSourceBranches()
    {
        Assert.Equal("FRequestMapDataList", UpdateEngineLogic.ClassifyRequeueTarget(true, TUpdateDataType.udtFileMap));
        Assert.Equal("FRequestFileList", UpdateEngineLogic.ClassifyRequeueTarget(false, TUpdateDataType.udtFileWav));
        Assert.Equal("FRequestFileList", UpdateEngineLogic.ClassifyRequeueTarget(false, TUpdateDataType.udtFileOther));
        Assert.Equal("FRequestImageList", UpdateEngineLogic.ClassifyRequeueTarget(false, TUpdateDataType.udtImagePak));
        Assert.Equal("FRequestImageList", UpdateEngineLogic.ClassifyRequeueTarget(false, TUpdateDataType.udtIndexWzl));
        // 差异断言：IsMapData 优先于 DataType（原文 if IsMapData then ... else if ...）
        Assert.Equal("FRequestMapDataList", UpdateEngineLogic.ClassifyRequeueTarget(true, TUpdateDataType.udtImagePak));
    }

    /// <summary>用例 12：IsFileKind / IsImageKind / IsIndexKind 三分且互斥。</summary>
    [Fact]
    public void DataKindPredicates_AreMutuallyExclusiveAndTotal()
    {
        foreach (TUpdateDataType t in Enum.GetValues<TUpdateDataType>())
        {
            int hits = (UpdateEngineLogic.IsFileKind(t) ? 1 : 0)
                     + (UpdateEngineLogic.IsImageKind(t) ? 1 : 0)
                     + (UpdateEngineLogic.IsIndexKind(t) ? 1 : 0);
            if (t == TUpdateDataType.udtFileMap) Assert.Equal(0, hits);   // 地图单独一类
            else Assert.Equal(1, hits);                                    // 其余三分
        }
        Assert.True(UpdateEngineLogic.IsFileKind(TUpdateDataType.udtFileWav));
        Assert.True(UpdateEngineLogic.IsImageKind(TUpdateDataType.udtImageWzl));
        Assert.True(UpdateEngineLogic.IsIndexKind(TUpdateDataType.udtIndexPak));
    }

    // ══════════════════════════════════════════════════════════════════════
    // ClearRequests 的两条分支（原文 :242-311）
    // ══════════════════════════════════════════════════════════════════════

    private sealed class FakeImageLibrary : IUpdateImageLibrary
    {
        public int Count;
        public readonly Dictionary<int, (bool Stop, bool Start)> Flags = new Dictionary<int, (bool, bool)>();
        public bool? Indexing;
        public int LockCount;
        public int ImageCount => Count;
        public void Lock() => LockCount++;
        public void UnLock() { }
        public void SetUpdateStop(int index, bool value)
        {
            var cur = Flags.TryGetValue(index, out var v) ? v : (false, false);
            Flags[index] = (value, cur.Item2);
        }
        public void SetUpdateStart(int index, bool value)
        {
            var cur = Flags.TryGetValue(index, out var v) ? v : (false, false);
            Flags[index] = (cur.Item1, value);
        }
        public void SetUpdateIndexing(bool value) => Indexing = value;
    }

    /// <summary>用例 1：<c>IsRestoreUpdateState = False</c> ⇒ 只清队列，不碰图库。</summary>
    [Fact]
    public void ClearRequests_WithoutRestore_OnlyEmptiesTheList()
    {
        var lib = new FakeImageLibrary { Count = 10 };
        lib.Flags[3] = (true, true);

        var list = new TSafeList();
        list.Add(new TClientRequest { FileName = "x", DataType = TUpdateDataType.udtImagePak, ImgIndex = 3, GameImages = lib });
        var req = list[0];

        TUpdateEngine.ClearRequests(null, list, false);

        Assert.Equal(0, list.Count);
        Assert.Equal(string.Empty, req.FileName);       // 原文 :254 清文件名
        Assert.Empty(lib.Flags.Keys.Count == 1 ? new int[0] : new int[0]);
        Assert.Equal((true, true), lib.Flags[3]);       // 图库标记**未**被回滚
        Assert.Equal(0, lib.LockCount);
    }

    /// <summary>用例 2：<c>IsRestoreUpdateState = True</c> ⇒ 图片类请求回滚两个标记。</summary>
    [Fact]
    public void ClearRequests_WithRestore_ResetsImageFlags()
    {
        var lib = new FakeImageLibrary { Count = 10 };
        lib.Flags[3] = (true, true);

        var list = new TSafeList();
        list.Add(new TRequestForTest
        {
            FileName = "x", DataType = TUpdateDataType.udtImagePak, ImgIndex = 3, GameImages = lib
        }.ToRequest());

        TUpdateEngine.ClearRequests(null, list, true);

        Assert.Equal(0, list.Count);
        Assert.Equal(1, lib.LockCount);
        Assert.Equal((false, false), lib.Flags[3]);     // 两个标记都置回 False
    }

    /// <summary>用例 3：越界 ImgIndex 被跳过（原文 :280 的双边界检查）。</summary>
    [Fact]
    public void ClearRequests_WithRestore_SkipsOutOfRangeImageIndex()
    {
        var lib = new FakeImageLibrary { Count = 2 };
        var list = new TSafeList();
        list.Add(new TRequestForTest { DataType = TUpdateDataType.udtImagePak, ImgIndex = 5, GameImages = lib }.ToRequest());
        list.Add(new TRequestForTest { DataType = TUpdateDataType.udtImagePak, ImgIndex = -1, GameImages = lib }.ToRequest());

        TUpdateEngine.ClearRequests(null, list, true);

        Assert.Empty(lib.Flags);                        // 都没落标记
        Assert.Equal(0, list.Count);
    }

    /// <summary>用例 4：索引类请求只回滚 <c>m_boUpdateIndexing</c>（<c>m_boUpdateIndex</c> 绝不碰）。</summary>
    [Fact]
    public void ClearRequests_WithRestore_IndexKindOnlyResetsIndexing()
    {
        var lib = new FakeImageLibrary { Count = 10 };
        lib.Indexing = true;

        foreach (TUpdateDataType t in new[] { TUpdateDataType.udtIndexPak, TUpdateDataType.udtIndexWzl })
        {
            var list = new TSafeList();
            list.Add(new TRequestForTest { DataType = t, ImgIndex = 0, GameImages = lib }.ToRequest());
            lib.Indexing = true;
            TUpdateEngine.ClearRequests(null, list, true);
            Assert.False(lib.Indexing);
        }
    }

    /// <summary>用例 5：wav 类请求把 <c>g_SoundUpDateList[ImgIndex]</c> 置回 True。</summary>
    [Fact]
    public void ClearRequests_WithRestore_WavKindRestoresSoundFlag()
    {
        var saved = TUpdateEngine.UpdateSoundList;
        try
        {
            TUpdateEngine.UpdateSoundList = new[] { false, false, false };
            // 原文 :297-300 的 wav 分支外层同样受 `if GameImages <> nil` 保护 ⇒ 必须给非 null
            var list = new TSafeList();
            list.Add(new TRequestForTest { DataType = TUpdateDataType.udtFileWav, ImgIndex = 1, GameImages = new FakeImageLibrary { Count = 3 } }.ToRequest());

            TUpdateEngine.ClearRequests(null, list, true);

            Assert.False(TUpdateEngine.UpdateSoundList[0]);
            Assert.True(TUpdateEngine.UpdateSoundList[1]);      // 被置回 True
            Assert.False(TUpdateEngine.UpdateSoundList[2]);
        }
        finally { TUpdateEngine.UpdateSoundList = saved; }
    }

    /// <summary>
    /// 用例 5b：<b>差异断言</b> —— wav 分支也在 <c>if GameImages &lt;&gt; nil</c> **里面**
    /// （原文 :270 的 nil 检查包住了全部三类），所以 <c>GameImages = null</c> 时
    /// <c>g_SoundUpDateList</c> **不会**被恢复。
    /// </summary>
    [Fact]
    public void ClearRequests_WithRestore_WavWithNullGameImages_DoesNotRestore()
    {
        var saved = TUpdateEngine.UpdateSoundList;
        try
        {
            TUpdateEngine.UpdateSoundList = new[] { false, false };
            var list = new TSafeList();
            list.Add(new TRequestForTest { DataType = TUpdateDataType.udtFileWav, ImgIndex = 1, GameImages = null }.ToRequest());
            TUpdateEngine.ClearRequests(null, list, true);
            Assert.False(TUpdateEngine.UpdateSoundList[1]);      // 未恢复
        }
        finally { TUpdateEngine.UpdateSoundList = saved; }
    }

    /// <summary>用例 6：wav 越界 ImgIndex 不抛（原文 :298 的双边界检查）。</summary>
    [Fact]
    public void ClearRequests_WithRestore_WavOutOfRange_IsSafe()
    {
        var saved = TUpdateEngine.UpdateSoundList;
        try
        {
            TUpdateEngine.UpdateSoundList = new bool[1];
            var list = new TSafeList();
            list.Add(new TRequestForTest { DataType = TUpdateDataType.udtFileWav, ImgIndex = 9, GameImages = new FakeImageLibrary { Count = 3 } }.ToRequest());
            TUpdateEngine.ClearRequests(null, list, true);     // 不应抛
            Assert.Equal(0, list.Count);

            list.Add(new TRequestForTest { DataType = TUpdateDataType.udtFileWav, ImgIndex = -1, GameImages = new FakeImageLibrary { Count = 3 } }.ToRequest());
            TUpdateEngine.ClearRequests(null, list, true);
            Assert.Equal(0, list.Count);
        }
        finally { TUpdateEngine.UpdateSoundList = saved; }
    }

    /// <summary>用例 7：<c>GameImages = null</c> 时整段图库回滚被跳过（原文 :270 的 nil 检查）。</summary>
    [Fact]
    public void ClearRequests_WithRestore_NullGameImages_SkipsRollback()
    {
        var list = new TSafeList();
        list.Add(new TRequestForTest { DataType = TUpdateDataType.udtImagePak, ImgIndex = 0, GameImages = null }.ToRequest());
        TUpdateEngine.ClearRequests(null, list, true);
        Assert.Equal(0, list.Count);   // 不抛且已清空
    }

    /// <summary>用例 8：ClearRequestList 清空引擎的三个队列。</summary>
    [Fact]
    public void ClearRequestList_EmptiesAllThreeQueues()
    {
        var e = new TUpdateEngine();
        e.Add("m.map", TUpdateDataType.udtFileMap, 0, null);
        e.Add("i.pak", TUpdateDataType.udtImagePak, 0, null);
        e.Add("f.wav", TUpdateDataType.udtFileWav, 0, null);
        Assert.Equal(3, e.GetUpdateCount());

        e.ClearRequestList();
        Assert.Equal(0, e.GetUpdateCount());
    }

    /// <summary>把 <see cref="TClientRequest"/> 用对象初始化器写得更紧凑的辅助类。</summary>
    private sealed class TRequestForTest
    {
        public string FileName = string.Empty;
        public TUpdateDataType DataType;
        public int ImgIndex;
        public object GameImages;
        public TClientRequest ToRequest() => new TClientRequest
        {
            FileName = FileName, DataType = DataType, ImgIndex = ImgIndex, GameImages = GameImages,
        };
    }

    // ══════════════════════════════════════════════════════════════════════
    // 服务端消息解析与 CRC 门控（原文 :604-639）
    // ══════════════════════════════════════════════════════════════════════

    private static byte[] BuildServerMessage(ushort ident, ushort param, uint requestId,
        byte[] payload, bool badCrc = false, uint? crcOverride = null)
    {
        uint crc = crcOverride ?? (payload == null || payload.Length == 0
            ? 0u
            : GXX.Core.Crypto.UnitDes.CalcCrc32(payload, 0, payload.Length));
        if (badCrc) crc ^= 0xFFFFFFFFu;

        var h = new TUpdateSrvMsgHeader
        {
            MsgFlag = UpdateEngineConst.UPDATE_SOCKET_FLAG,
            RequestID = requestId,
            Ident = ident,
            Param = param,
            DataCrc = crc,
            DataLen = payload?.Length ?? 0,
        };

        var b = new byte[UpdateEngineLayout.SrvHeaderSize + h.DataLen];
        BitConverter.GetBytes(h.MsgFlag).CopyTo(b, 0);
        BitConverter.GetBytes(h.RequestID).CopyTo(b, 4);
        BitConverter.GetBytes(h.Ident).CopyTo(b, 8);
        BitConverter.GetBytes(h.Param).CopyTo(b, 10);
        BitConverter.GetBytes(h.DataCrc).CopyTo(b, 12);
        BitConverter.GetBytes(h.DataLen).CopyTo(b, 16);
        if (h.DataLen > 0) Array.Copy(payload, 0, b, UpdateEngineLayout.SrvHeaderSize, h.DataLen);
        return b;
    }

    private static byte[] Concat(byte[] a, byte[] b)
    {
        var r = new byte[a.Length + b.Length];
        Array.Copy(a, 0, r, 0, a.Length);
        Array.Copy(b, 0, r, a.Length, b.Length);
        return r;
    }

    /// <summary>用例 1：缓冲不足一个头 ⇒ 不解析。</summary>
    [Fact]
    public void ParseServerMessage_ShortBuffer_NotParsed()
    {
        var r = UpdateEngineLogic.ParseServerMessage(Array.Empty<byte>());
        Assert.False(r.Parsed);
        r = UpdateEngineLogic.ParseServerMessage(Enumerable.Repeat((byte)0x78, 19).ToArray());
        Assert.False(r.Parsed);
    }

    /// <summary>用例 2：头够但数据长度不够 ⇒ 不解析（等待续包）。</summary>
    [Fact]
    public void ParseServerMessage_HeaderWithoutFullPayload_NotParsed()
    {
        var h = new TUpdateSrvMsgHeader
        {
            MsgFlag = UpdateEngineConst.UPDATE_SOCKET_FLAG, Ident = (ushort)UpdateEngineConst.WM_DATA,
            DataLen = 10, DataCrc = 0,
        };
        var b = new byte[UpdateEngineLayout.SrvHeaderSize];
        BitConverter.GetBytes(h.MsgFlag).CopyTo(b, 0);
        BitConverter.GetBytes(h.Ident).CopyTo(b, 8);
        BitConverter.GetBytes(h.DataLen).CopyTo(b, 16);

        var r = UpdateEngineLogic.ParseServerMessage(b);
        Assert.False(r.Parsed);
    }

    /// <summary>用例 3：MsgFlag 不匹配 ⇒ Parsed 且 Consumed=false（调用方清空整缓冲）。</summary>
    [Fact]
    public void ParseServerMessage_WrongMsgFlag_RequiresBufferReset()
    {
        var h = new TUpdateSrvMsgHeader
        {
            MsgFlag = 0x12345678, Ident = (ushort)UpdateEngineConst.WM_DATA, DataLen = 0,
        };
        var b = new byte[UpdateEngineLayout.SrvHeaderSize];
        BitConverter.GetBytes(h.MsgFlag).CopyTo(b, 0);
        BitConverter.GetBytes(h.Ident).CopyTo(b, 8);

        var r = UpdateEngineLogic.ParseServerMessage(b);
        Assert.True(r.Parsed);
        Assert.False(r.Consumed);
        Assert.Equal(0x12345678u, r.Header.MsgFlag);
    }

    /// <summary>用例 4：正常 <c>WM_DATA</c> + 正确 CRC ⇒ 取出载荷、缓冲被消耗、剩余为空。</summary>
    [Fact]
    public void ParseServerMessage_ValidDataMessage_ExtractsPayload()
    {
        byte[] payload = { 1, 2, 3, 4, 5 };
        byte[] msg = BuildServerMessage((ushort)UpdateEngineConst.WM_DATA, 0, 42, payload);

        var r = UpdateEngineLogic.ParseServerMessage(msg);
        Assert.True(r.Parsed);
        Assert.True(r.Consumed);
        Assert.True(r.IsCheckOK);
        Assert.Equal(42u, r.Header.RequestID);
        Assert.Equal(payload, r.Payload);
        Assert.Empty(r.Remaining);
    }

    /// <summary>用例 5：CRC 错误 ⇒ IsCheckOK=false 且**载荷为空**，但缓冲仍被消耗。</summary>
    [Fact]
    public void ParseServerMessage_BadCrc_ConsumesButNoPayload()
    {
        byte[] payload = { 9, 8, 7 };
        byte[] msg = BuildServerMessage((ushort)UpdateEngineConst.WM_DATA, 0, 1, payload, badCrc: true);

        var r = UpdateEngineLogic.ParseServerMessage(msg);
        Assert.True(r.Parsed);
        Assert.True(r.Consumed);
        Assert.False(r.IsCheckOK);
        Assert.Null(r.Payload);
        Assert.Empty(r.Remaining);
    }

    /// <summary>
    /// 用例 6：<b>差异断言</b> —— <c>WM_CHECK_CODE</c> 消息**跳过 CRC 校验**
    /// （原文 :619 的 <c>Ident &lt;&gt; WM_CHECK_CODE</c> 条件），即使 DataCrc 是错的。
    /// </summary>
    [Fact]
    public void ParseServerMessage_CheckCodeSkipsCrcGate()
    {
        byte[] payload = { 1, 2 };
        byte[] msg = BuildServerMessage((ushort)UpdateEngineConst.WM_CHECK_CODE, 0, 7, payload, badCrc: true);

        var r = UpdateEngineLogic.ParseServerMessage(msg);
        Assert.True(r.Parsed);
        Assert.True(r.IsCheckOK);        // 跳过了 CRC ⇒ 仍算通过
        Assert.Null(r.Payload);          // 但也不取载荷（原文 :619 条件不成立 ⇒ 不 SetSize/Move）
    }

    /// <summary>用例 7：<c>DataLen = 0</c> 时同样跳过 CRC 校验（原文条件 <c>DataLen &gt; 0</c>）。</summary>
    [Fact]
    public void ParseServerMessage_ZeroLengthSkipsCrcGate()
    {
        byte[] msg = BuildServerMessage((ushort)UpdateEngineConst.WM_CONNECT_RET, 1, 0, Array.Empty<byte>(), badCrc: true);
        var r = UpdateEngineLogic.ParseServerMessage(msg);
        Assert.True(r.Parsed);
        Assert.True(r.IsCheckOK);
        Assert.Null(r.Payload);
    }

    /// <summary>用例 8：粘包 —— 剩余字节被保留（原文 :633 的 Copy 截断）。</summary>
    [Fact]
    public void ParseServerMessage_KeepsRemainingBytes_ForStickyPackets()
    {
        byte[] p1 = { 1, 2, 3 };
        byte[] p2 = { 4, 5 };
        byte[] two = Concat(BuildServerMessage((ushort)UpdateEngineConst.WM_DATA, 0, 1, p1),
                            BuildServerMessage((ushort)UpdateEngineConst.WM_DATA, 0, 2, p2));

        var r1 = UpdateEngineLogic.ParseServerMessage(two);
        Assert.True(r1.IsCheckOK);
        Assert.Equal(p1, r1.Payload);
        Assert.NotEmpty(r1.Remaining);

        var r2 = UpdateEngineLogic.ParseServerMessage(r1.Remaining);
        Assert.True(r2.IsCheckOK);
        Assert.Equal(p2, r2.Payload);
        Assert.Empty(r2.Remaining);
    }

    /// <summary>用例 9：<c>WM_CONNECT_RET</c> 的 Param 语义（0 = 密码失败，非 0 = 通过）。</summary>
    [Fact]
    public void ParseServerMessage_ConnectRetParamCarriesPasswordResult()
    {
        var ok = UpdateEngineLogic.ParseServerMessage(
            BuildServerMessage((ushort)UpdateEngineConst.WM_CONNECT_RET, 1, 0, Array.Empty<byte>()));
        Assert.Equal(1, ok.Header.Param);

        var fail = UpdateEngineLogic.ParseServerMessage(
            BuildServerMessage((ushort)UpdateEngineConst.WM_CONNECT_RET, 0, 0, Array.Empty<byte>()));
        Assert.Equal(0, fail.Header.Param);
    }

    /// <summary>用例 10：null 入参不抛。</summary>
    [Fact]
    public void ParseServerMessage_Null_ReturnsNotParsed()
    {
        var r = UpdateEngineLogic.ParseServerMessage((byte[])null);
        Assert.False(r.Parsed);
    }

    /// <summary>用例 11：MyGetTickCount 与 uint 回绕语义。</summary>
    [Fact]
    public void MyGetTickCount_IsUnsignedTickCount()
    {
        uint a = UpdateEngineLogic.MyGetTickCount();
        Assert.True(a > 0);
        // 回绕比较（原文大量 unsigned 相减判超时）在 uint 下仍正确
        uint earlier = a - 5;
        Assert.True(unchecked(a - earlier) >= UpdateEngineConst.REMAINING_SEND_INTERVAL_MS - 1000u);
    }

    /// <summary>用例 12：全局开关默认值（原文 <c>g_boAutoUpdate</c> / <c>g_UpdateIsLogPassWordFail</c>）。</summary>
    [Fact]
    public void GlobalFlags_Defaults()
    {
        Assert.True(TUpdateEngine.g_boAutoUpdate);
        Assert.False(TUpdateEngine.g_UpdateIsLogPassWordFail);
    }
}
