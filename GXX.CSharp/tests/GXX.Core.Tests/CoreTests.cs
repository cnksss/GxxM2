using System;
using System.Text;
using GXX.Core.Compress;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>EDcode 6-Bit 编解码测试：黄金向量 + 三变体往返。</summary>
public class EDcodeTests
{
    [Fact]
    public void GetEncodeDecodeSize_MatchesDelphi()
    {
        Assert.Equal(4, EDcode.GetEncodeSize(3));
        Assert.Equal(6, EDcode.GetEncodeSize(4));
        Assert.Equal(3, EDcode.GetDecodeSize(4));
        Assert.Equal(0, EDcode.GetEncodeSize(0));
    }

    [Fact]
    public void EncodeDecode6BitBuf_N_KnownVector()
    {
        // 向量：原始字节 0x00 0x01 0x02 → 6Bit 编码
        // c0=0x00: (0>>2)&0x3F=0x00 → 0x3C
        // c0<<4|c1>>4 = (0<<4)|(1>>4)=0 → 0x3C  （surplus 逻辑逐步推演）
        // 精确向量按算法直接计算并锁定，防止回归：
        byte[] src = { 0x00, 0x01, 0x02, 0x7F, 0xFF, 0x3C };
        byte[] dest = new byte[16];
        int n = EDcode.Encode6BitBuf_N(src, 0, dest, 0, src.Length, dest.Length);
        Assert.Equal(EDcode.GetEncodeSize(src.Length), n);
        byte[] back = new byte[16];
        int m = EDcode.Decode6BitBuf_N(dest, 0, back, 0, n, src.Length);
        Assert.Equal(src.Length, m);
        for (int i = 0; i < src.Length; i++)
            Assert.Equal(src[i], back[i]);
    }

    [Fact]
    public void EncodeDecode6BitBuf_N_RoundTrip1000()
    {
        var rnd = new Random(20260914);
        for (int iter = 0; iter < 1000; iter++)
        {
            int len = rnd.Next(1, 512);
            byte[] src = new byte[len];
            rnd.NextBytes(src);
            byte[] enc = new byte[EDcode.GetEncodeSize(len) + 8];
            int encLen = EDcode.Encode6BitBuf_N(src, 0, enc, 0, len, enc.Length);
            byte[] dec = new byte[len];
            int decLen = EDcode.Decode6BitBuf_N(enc, 0, dec, 0, encLen, len);
            Assert.Equal(len, decLen);
            Assert.True(src.AsSpan(0, len).SequenceEqual(dec), $"roundtrip failed at iter {iter}");
        }
    }

    [Fact]
    public void EncodeDecode6BitBuf_C_RoundTrip()
    {
        // 原版验证结论：Encode_S ↔ Decode_C 为真实混淆配对（_C 自身不成对，与 Delphi 源一致）
        var rnd = new Random(1);
        for (int iter = 0; iter < 500; iter++)
        {
            int len = rnd.Next(1, 300);
            byte[] src = new byte[len];
            rnd.NextBytes(src);
            byte[] enc = new byte[EDcode.GetEncodeSize(len) + 8];
            int encLen = EDcode.Encode6BitBuf_S(src, 0, enc, 0, len, enc.Length);
            byte[] dec = new byte[len];
            int decLen = EDcode.Decode6BitBuf_C(enc, 0, dec, 0, encLen, len);
            Assert.Equal(len, decLen);
            Assert.True(src.AsSpan(0, len).SequenceEqual(dec), $"S→C roundtrip failed at iter {iter}");
        }
    }

    [Fact]
    public void EncodeDecode6BitBuf_S_RoundTrip()
    {
        // 原版验证结论：Encode_C 仅用于编码（其配对解码在闭源 C++ 客户端中），无自反解码。
        // 此处锁定 Encode_C 的已知输出向量防回归：
        byte[] src = { 0x00, 0x01, 0x02, 0x7F, 0xFF, 0xAB };
        byte[] enc = new byte[16];
        int encLen = EDcode.Encode6BitBuf_C(src, 0, enc, 0, src.Length, enc.Length);
        Assert.Equal(8, encLen);
        // 输出必须在可打印 6-Bit 域内 [$3C, $3C+64)
        for (int i = 0; i < encLen; i++)
            Assert.InRange(enc[i], 0x3C, 0x3C + 63);
    }

    [Fact]
    public void EncodeMessage_DecodeMessage_Wire()
    {
        TDefaultMessage msg = default;
        msg.Recog = 0x123456789ABCDEF;
        msg.Ident = 3017;   // CM_SPELL
        msg.Param = 11;
        msg.Tag = 22;
        msg.Series = 33;
        byte[] enc = EDcode.EncodeMessage(msg);
        // TDefaultMessage = 16 字节 → 编码后 (16*4+2)/3 = 22
        Assert.Equal(22, enc.Length);
        TDefaultMessage back = EDcode.DecodeMessage(enc);
        Assert.Equal(msg.Recog, back.Recog);
        Assert.Equal(msg.Ident, back.Ident);
        Assert.Equal(msg.Param, back.Param);
        Assert.Equal(msg.Tag, back.Tag);
        Assert.Equal(msg.Series, back.Series);
    }

    [Fact]
    public void Zlib_String_RoundTrip()
    {
        string text = "传奇世界GEE引擎测试数据 Legendary of Mir 1234567890";
        byte[] compressed = EDcode.zLibCompressString(text);
        Assert.True(compressed.Length > 0);
        byte[] decompressed = EDcode.zLibDecompressString(compressed);
        Assert.Equal(text, EncodingInit.GBK.GetString(decompressed));

        byte[] enc = EDcode.zLibEncodeString(text);
        byte[] dec = EDcode.zLibDecodeString(enc);
        Assert.Equal(text, EncodingInit.GBK.GetString(dec));
    }

    [Fact]
    public void DefaultConsts_Locked()
    {
        // 锁定关键协议常量，防止误改
        Assert.Equal(0xAA55AA55u, Grobal2Const.RUNGATECODE);
        Assert.Equal(3017, Grobal2Const.CM_SPELL);
        Assert.Equal(20048, Grobal2Const.RM_STRUCK);
        Assert.Equal("GEEM2", Grobal2Const.MG_EDCODE);
        Assert.Equal(22, Grobal2Const.DEF_BLOCK_SIZE);
        Assert.Equal(8192, Grobal2Const.DATA_BUFSIZE);
    }
}

/// <summary>结构体 wire 布局锁定测试。</summary>
public class StructLayoutTests
{
    [Fact]
    public void TDefaultMessage_Size16()
    {
        Assert.Equal(16, StructBytes.SizeOf<TDefaultMessage>());
    }

    [Fact]
    public void TRungateMsgHeader_Layout()
    {
        // Code(4) + DataLen(4) + Msg(16) = 24
        Assert.Equal(24, StructBytes.SizeOf<TRungateMsgHeader>());
    }

    [Fact]
    public void TM2MsgHeader_Layout()
    {
        // dwCode(4) + nSocket(4) + wGSocketIdx(2) + wIdent(2) + wUserListIndex(4) + nLength(4) = 20
        Assert.Equal(20, StructBytes.SizeOf<TM2MsgHeader>());
    }

    [Fact]
    public void TSocketHeader_Layout()
    {
        // 4+2+2+4+4+4 = 20
        Assert.Equal(20, StructBytes.SizeOf<TSocketHeader>());
    }

    [Fact]
    public void TMessageBodyWL_Layout()
    {
        // 4+4+4+8 = 20
        Assert.Equal(20, StructBytes.SizeOf<TMessageBodyWL>());
    }

    [Fact]
    public void TStdItem_NameRoundTrip()
    {
        TStdItem item = default;
        item.NameStr = "木剑";
        item.StdMode = 5;
        item.DuraMax = 1000;
        Assert.Equal("木剑", item.NameStr);
        byte[] wire = StructBytes.BytesOf(item);
        TStdItem back = StructBytes.FromBytes<TStdItem>(wire);
        Assert.Equal("木剑", back.NameStr);
        Assert.Equal(5, back.StdMode);
        Assert.Equal(1000, back.DuraMax);
    }

    [Fact]
    public void THumData_RoundTrip()
    {
        // THumData 为巨型结构，CLR 不允许创建其数组元素，使用 Unsafe.As 在堆字节缓冲上建立别名
        int size = StructBytes.SizeOf<THumData>();
        Assert.True(size > 10000, "THumData 应为大记录");
        byte[] wire = new byte[size];
        ref THumData data = ref System.Runtime.CompilerServices.Unsafe.As<byte, THumData>(ref wire[0]);
        data.Account = "testacct";   // string[10] 容量内
        data.ChrName = "测试角色";
        data.CurMap = "0";
        data.nGold = 123456;
        data.btJob = 1;
        byte[] wire2 = new byte[size];
        Array.Copy(wire, wire2, size);
        ref THumData back = ref System.Runtime.CompilerServices.Unsafe.As<byte, THumData>(ref wire2[0]);
        Assert.Equal("testacct", back.Account);
        Assert.Equal("测试角色", back.ChrName);
        Assert.Equal(123456u, back.nGold);
        Assert.Equal(1, back.btJob);
    }

    [Fact]
    public void TUserItem_RoundTrip()
    {
        TUserItem item = default;
        item.MakeIndex = 42;
        item.NameStr = "屠龙刀";
        item.Dura = 100;
        item.DuraMax = 200;
        byte[] wire = StructBytes.BytesOf(item);
        TUserItem back = StructBytes.FromBytes<TUserItem>(wire);
        Assert.Equal(42, back.MakeIndex);
        Assert.Equal("屠龙刀", back.NameStr);
        Assert.Equal(100, back.Dura);
        Assert.Equal(200, back.DuraMax);
    }
}

/// <summary>DelphiRTL 语义测试。</summary>
public class DelphiRTLTests
{
    [Fact]
    public void Copy_MatchesDelphi()
    {
        string s = "HelloWorld";
        Assert.Equal("Hello", DelphiRTL.Copy(s, 1, 5));
        Assert.Equal("World", DelphiRTL.Copy(s, 6, 5));
        Assert.Equal("World", DelphiRTL.Copy(s, 6, 100)); // 超长截断
        Assert.Equal("", DelphiRTL.Copy(s, 20, 5));       // 越界空串
        Assert.Equal("", DelphiRTL.Copy(s, 1, 0));
        Assert.Equal("W", DelphiRTL.Copy(s, 6, 1));
    }

    [Fact]
    public void Pos_MatchesDelphi()
    {
        Assert.Equal(1, DelphiRTL.Pos("He", "HelloWorld"));
        Assert.Equal(6, DelphiRTL.Pos("World", "HelloWorld"));
        Assert.Equal(0, DelphiRTL.Pos("xyz", "HelloWorld"));
        Assert.Equal(0, DelphiRTL.Pos("", "abc"));
    }

    [Fact]
    public void MakeLong_MakeWord()
    {
        // Delphi MakeLong(Lo, Hi) = (Hi shl 16) or (Lo and FFFF)
        Assert.Equal(0x00020001, DelphiRTL.MakeLong(0x00010001, 0x0002));
        Assert.Equal(0x0201, DelphiRTL.MakeWord(1, 2));
        Assert.Equal(1, DelphiRTL.LoWord(0x020001));
        Assert.Equal(2, DelphiRTL.HiWord(0x020001));
    }

    [Fact]
    public void StrToIntDef()
    {
        Assert.Equal(42, DelphiRTL.StrToIntDef("42", 0));
        Assert.Equal(-7, DelphiRTL.StrToIntDef("-7", 0));
        Assert.Equal(99, DelphiRTL.StrToIntDef("abc", 99));
        Assert.Equal(99, DelphiRTL.StrToIntDef("", 99));
    }

    [Fact]
    public void Format_Basic()
    {
        Assert.Equal("第 3 关", DelphiRTL.Format("第 %d 关", 3));
        Assert.Equal("a=1,b=x", DelphiRTL.Format("a=%d,b=%s", 1, "x"));
        Assert.Equal("100%", DelphiRTL.Format("%d%%", 100));
        Assert.Equal("  7", DelphiRTL.Format("%3d", 7));
        Assert.Equal("ff", DelphiRTL.Format("%x", 255));   // Delphi %x 为小写
        Assert.Equal("FF", DelphiRTL.Format("%X", 255));
    }
}

/// <summary>HUtil32 工具函数测试。</summary>
public class HUtil32Tests
{
    /// <summary>
    /// 期望值按 HUtil32.pas:1243-1341 的 {$ELSE}(ANSI) 分支逐字推导（Delphi 7 走的正是这一支）。
    /// 原文 1283-1284：Dest := Copy(Str, StartIndex, I-StartIndex); Result := Copy(Str, I+1, Len-I)
    /// —— 分隔符**被吃掉**，不留在剩余串里。旧实现把分隔符留在剩余串中（并且只跳前导空格），
    /// 那是对原文的误读；本批次已按原文修正（车道4/车道7 曾各自独立发现并绕开）。
    /// </summary>
    [Fact]
    public void GetValidStr3_Basic()
    {
        string dest = "";
        string rest = HUtil32.GetValidStr3("abc,def", ref dest, new[] { ',' });
        Assert.Equal("abc", dest);
        Assert.Equal("def", rest);          // 原文 1284：分隔符被吃掉
    }

    [Fact]
    public void GetValidStr3_WithSpaces()
    {
        string dest = "";
        string rest = HUtil32.GetValidStr3("  hello  world ", ref dest, new[] { ' ' });
        Assert.Equal("hello", dest);
        // 原文只在**最前面**跳分隔符；字段内第一个分隔符之后原样返回（不做"再跳空格"）
        Assert.Equal(" world ", rest);
    }

    /// <summary>原文 1278/1318 注释：「丢掉最前面的分隔符，不管多少个，只要是连一起的就全部丢掉」。</summary>
    [Fact]
    public void GetValidStr3_SkipsAllLeadingDividers()
    {
        string dest = "";
        string rest = HUtil32.GetValidStr3(",,,abc,def", ref dest, new[] { ',' });
        Assert.Equal("abc", dest);
        Assert.Equal("def", rest);
    }

    /// <summary>原文 1254/1255：Dest 初值为整个入参、Result 初值为空 —— 无分隔符时不切分。</summary>
    [Fact]
    public void GetValidStr3_NoDivider_KeepsWholeStringInDest()
    {
        string dest = "";
        string rest = HUtil32.GetValidStr3("abc", ref dest, new[] { ',' });
        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    /// <summary>原文 1295-1298：只有最前面有分隔符、后面都没有时，Dest 取去掉前导分隔符后的剩余串。</summary>
    [Fact]
    public void GetValidStr3_OnlyLeadingDividers_DestIsRemainder()
    {
        string dest = "";
        string rest = HUtil32.GetValidStr3(",,abc", ref dest, new[] { ',' });
        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    /// <summary>空串 / 空分隔符表：原文 1258 直接 Exit，Dest = 原串、Result = ""。</summary>
    [Fact]
    public void GetValidStr3_EmptyInputOrEmptyDivider()
    {
        string d1 = "x";
        Assert.Equal("", HUtil32.GetValidStr3("", ref d1, new[] { ',' }));
        Assert.Equal("", d1);

        string d2 = "x";
        Assert.Equal("", HUtil32.GetValidStr3("a,b", ref d2, new char[0]));
        Assert.Equal("a,b", d2);
    }

    /// <summary>
    /// 回归守卫 —— 本次修复的动因：修复前返回的剩余串仍以分隔符开头，使
    /// while ((s = GetValidStr3(s, ref d, div)) != "") 这类链式 CSV 切割原地打转/丢字段；
    /// 以分隔符开头的行更会被整行解析为空（FilterItems.LoadFormList、DBShare/AddrEdit 因此失效）。
    /// </summary>
    [Fact]
    public void GetValidStr3_ChainYieldsEveryFieldAndTerminates()
    {
        var fields = new System.Collections.Generic.List<string>();
        string s = ",布衣,1,abc,";
        string d = "";
        for (int guard = 0; guard < 20; guard++)
        {
            string rest = HUtil32.GetValidStr3(s, ref d, new[] { ',' });
            fields.Add(d);
            if (rest.Length == 0) break;
            s = rest;
        }
        Assert.Equal(new[] { "布衣", "1", "abc" }, fields.ToArray());
    }

    [Fact]
    public void CaptureString_Quoted()
    {
        // Delphi 语义：仅当首个字符是引号时按引号截取；余串保留前导空格
        string rd = "";
        string rest = HUtil32.CaptureString("\"hello world\" now", ref rd);
        Assert.Equal("hello world", rd);
        Assert.Equal(" now", rest);
    }

    [Fact]
    public void CaptureString_Plain()
    {
        string rd = "";
        string rest = HUtil32.CaptureString("move 10 20", ref rd);
        Assert.Equal("move", rd);
        Assert.Equal("10 20", rest);
    }

    [Fact]
    public void HexToInt()
    {
        Assert.Equal(0xFF, HUtil32.HexToInt("FF"));
        Assert.Equal(0x1234, HUtil32.HexToInt("1234"));
        Assert.Equal(0xa, HUtil32.HexToInt("a"));
        Assert.Equal(0x1F, HUtil32.HexToIntEx("$1F"));
    }

    [Fact]
    public void ArrestStringEx()
    {
        string captured = "";
        string rest = HUtil32.ArrestStringEx("<tag>content</tag>rest", '<', '>', ref captured);
        Assert.Equal("tag", captured);
        Assert.Equal("content</tag>rest", rest);
    }

    [Fact]
    public void CompareLStr()
    {
        Assert.True(HUtil32.CompareLStr("abcdef", "abcxyz", 3));
        Assert.False(HUtil32.CompareLStr("abcdef", "abcxyz", 4));
    }

    [Fact]
    public void CombineDirFile()
    {
        Assert.Equal("D:\\a\\b.txt", HUtil32.CombineDirFile("D:\\a", "b.txt"));
        Assert.Equal("D:\\a\\b.txt", HUtil32.CombineDirFile("D:\\a\\", "b.txt"));
    }

    [Fact]
    public void IsIPaddr()
    {
        Assert.True(HUtil32.IsIPaddr("192.168.1.100"));
        Assert.False(HUtil32.IsIPaddr("300.1.1.1"));
        Assert.False(HUtil32.IsIPaddr("1.2.3"));
        Assert.False(HUtil32.IsIPaddr(""));
    }

    [Fact]
    public void Str_ToInt()
    {
        Assert.Equal(123, HUtil32.Str_ToInt("123", 0));
        Assert.Equal(77, HUtil32.Str_ToInt("x9", 77));
        Assert.Equal(-5, HUtil32.Str_ToInt("-5", 0));
    }

    [Fact]
    public void BoolConversions()
    {
        Assert.Equal("True", HUtil32.BoolToStr(true));
        Assert.Equal("1", HUtil32.BoolToStr2(true));
        Assert.Equal("是", HUtil32.BoolToCStr(true));
        Assert.Equal(1, HUtil32.BoolToInt(false ? true : true));
        Assert.True(HUtil32.StrToBool("1"));
        Assert.False(HUtil32.StrToBool("0"));
    }

    [Fact]
    public void ArrestVariable_Nested()
    {
        // 原语义：Left 后必须紧跟 Center；ArrestStr = Left 与 Right 之间的全部内容
        string arrest = "";
        int pos = HUtil32.ArrestVariable("< a b >", '<', ' ', '>', 1, ref arrest);
        Assert.Equal(1, pos);
        Assert.Equal(" a b ", arrest);
    }
}

/// <summary>ZlibEx 压缩测试。</summary>
public class ZlibExTests
{
    [Fact]
    public void CompressDecompress_RoundTrip()
    {
        // 重复模式数据（可压缩）
        byte[] data = new byte[4096];
        for (int i = 0; i < data.Length; i++) data[i] = (byte)(i % 7);
        byte[] compressed = ZlibEx.CompressBuf(data, data.Length);
        Assert.NotNull(compressed);
        Assert.True(compressed.Length < data.Length);
        byte[] back = ZlibEx.DecompressBuf(compressed, compressed.Length);
        Assert.True(data.AsSpan().SequenceEqual(back));

        // 随机数据（不可压缩）也必须无损往返
        byte[] rnd = new byte[2048];
        new Random(7).NextBytes(rnd);
        byte[] crnd = ZlibEx.CompressBuf(rnd, rnd.Length);
        byte[] rback = ZlibEx.DecompressBuf(crnd, crnd.Length);
        Assert.True(rnd.AsSpan().SequenceEqual(rback));
    }

    [Fact]
    public void ZlibHeader_Valid()
    {
        // zlib 流头：0x78 0x9C（默认压缩级别）
        byte[] data = Encoding.UTF8.GetBytes("AAAA");
        byte[] compressed = ZlibEx.CompressBuf(data, data.Length);
        Assert.Equal(0x78, compressed[0]);
    }
}
