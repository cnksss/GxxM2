using GXX.Core.Util;
using GXX.M2Server.Misc;
using Xunit;

using TEncoding = System.Text.Encoding;

namespace GXX.M2Server.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：**请求 #4 落地** —— <c>StringListHelper.pas</c>（53 行）1:1 移植的测试。
///
/// <para>
/// 该单元是 <c>EncodingHelper.TEncodingHelper.GetBufferEncoding</c> 的**真实生产消费者**
/// （原文 <c>:45</c>）：负责"读无 BOM 的 UTF-8 文件不乱码"。本文件用**同一段文本的 4 种落盘编码**
/// 做差异断言（GBK / UTF-8 无 BOM / UTF-8 带 BOM / UTF-16LE 带 BOM）。
/// </para>
/// </summary>
public sealed class StringListHelperTests : IDisposable
{
    private readonly string _dir;

    public StringListHelperTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p8slh_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { /* 临时目录清理失败不影响门禁 */ }
    }

    private string WriteBytes(string name, byte[] bytes)
    {
        string path = Path.Combine(_dir, name);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    private const string Sample = "第一行\r\n第二行\r\n";

    // ==================================================================
    // LoadFromFile（原文 :22-32）
    // ==================================================================

    [Fact]
    public void LoadFromFile_GbkFile_DecodesViaDefaultEncoding()
    {
        string path = WriteBytes("gbk.txt", GXX.Core.EncodingInit.GBK.GetBytes(Sample));
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, path);

        Assert.Equal(2, list.Count);
        Assert.Equal("第一行", list[0]);
        Assert.Equal("第二行", list[1]);
        Assert.Equal(936, list.GetEncoding().CodePage);       // 回落 DefaultEncoding（GBK）
    }

    [Fact]
    public void LoadFromFile_Utf8NoBomFile_IsNotMojibake()
    {
        // ★ 本单元存在的**唯一理由**（原文 :1-3 的注释："修复StringList读取无bom表的Utf8文件时乱码"）
        string path = WriteBytes("u8.txt", TEncoding.UTF8.GetBytes(Sample));
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, path);

        Assert.Equal("第一行", list[0]);
        Assert.Equal("第二行", list[1]);
        Assert.Equal(65001, list.GetEncoding().CodePage);     // NoBomUTF8
        Assert.Empty(list.GetEncoding().GetPreamble());       // 不写 BOM
    }

    [Fact]
    public void LoadFromFile_Utf8WithBom_StripsBom()
    {
        byte[] bytes = TEncoding.UTF8.GetPreamble()
            .Concat(TEncoding.UTF8.GetBytes(Sample)).ToArray();
        string path = WriteBytes("u8bom.txt", bytes);
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, path);

        Assert.Equal(2, list.Count);
        Assert.Equal("第一行", list[0]);                       // BOM 未被带进首行
        Assert.Equal(3, list.GetEncoding().GetPreamble().Length);
    }

    [Fact]
    public void LoadFromFile_Utf16LeWithBom_Decodes()
    {
        byte[] bytes = TEncoding.Unicode.GetPreamble()
            .Concat(TEncoding.Unicode.GetBytes(Sample)).ToArray();
        string path = WriteBytes("u16.txt", bytes);
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, path);

        Assert.Equal("第一行", list[0]);
        Assert.Equal("第二行", list[1]);
        Assert.Equal(1200, list.GetEncoding().CodePage);
    }

    [Fact]
    public void LoadFromFile_FourEncodingsSameText_AllYieldSameLines()
    {
        // ★ 差异断言：编码对象各不相同，但**还原出的行必须一致**
        var cases = new (string Name, byte[] Bytes)[]
        {
            ("c_gbk.txt", GXX.Core.EncodingInit.GBK.GetBytes(Sample)),
            ("c_u8.txt", TEncoding.UTF8.GetBytes(Sample)),
            ("c_u8bom.txt", TEncoding.UTF8.GetPreamble().Concat(TEncoding.UTF8.GetBytes(Sample)).ToArray()),
            ("c_u16.txt", TEncoding.Unicode.GetPreamble().Concat(TEncoding.Unicode.GetBytes(Sample)).ToArray()),
        };

        var signatures = new List<string>();
        foreach (var (name, bytes) in cases)
        {
            var list = new TStringList();
            TStringListHelper.LoadFromFile(list, WriteBytes(name, bytes));
            Assert.Equal(2, list.Count);
            Assert.Equal(new[] { "第一行", "第二行" }, new[] { list[0], list[1] });
            // 区分 UTF-8「带 BOM」与「无 BOM」：CodePage 相同（65001），但 preamble 长度不同
            signatures.Add($"{list.GetEncoding().CodePage}/{list.GetEncoding().GetPreamble().Length}");
        }

        Assert.Equal(4, signatures.Distinct().Count());        // 四种编码确实被识别为四种不同形态
    }

    [Fact]
    public void LoadFromFile_GbkFileWhoseBytesLookLikeUtf8_IsMisdetected_OriginalFlaw()
    {
        // ★★ 原文缺陷（EncodingHelper.pas:167-179 + 本单元的接线共同造成）：
        //    「值」的 GBK 字节 D6 B5 恰好构成**合法的 UTF-8 两字节序列**（$C0..$DF + $80..$BF），
        //    且其后全是 ASCII ⇒ IsBufferUTF8 返回 True ⇒ 整份文件被当**无 BOM UTF-8** 解码 ⇒ 乱码。
        //    这正是 StringListHelper 想修的"UTF-8 乱码"问题在**反方向**上的副作用。
        byte[] gbk = GXX.Core.EncodingInit.GBK.GetBytes("值");
        Assert.True(GXX.Core.EncodingHelper.TEncodingHelper.IsBufferUTF8(gbk));   // 缺陷的成因
        Assert.Equal(new byte[] { 0xD6, 0xB5 }, gbk);

        string path = WriteBytes("landmine.txt", GXX.Core.EncodingInit.GBK.GetBytes("Key=值\r\n"));
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, path);

        Assert.Equal(1, list.Count);
        Assert.NotEqual("Key=值", list[0]);                    // 乱码（忠实复刻原文行为，未擅自"修好"）
        Assert.Equal(65001, list.GetEncoding().CodePage);      // 被误判为 UTF-8
    }

    [Fact]
    public void LoadFromFile_GbkFileWithUnsafeLeadByte_DecodesAsGbk()
    {
        // 对照组：GBK「测试」= B2 E2 CA D4，B2 不是合法 UTF-8 首字节 ⇒ IsBufferUTF8 为假 ⇒ 正确按 GBK 读
        byte[] gbk = GXX.Core.EncodingInit.GBK.GetBytes("测试");
        Assert.False(GXX.Core.EncodingHelper.TEncodingHelper.IsBufferUTF8(gbk));

        string path = WriteBytes("safe.txt", GXX.Core.EncodingInit.GBK.GetBytes("Key=测试\r\n"));
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, path);
        Assert.Equal("Key=测试", list[0]);
        Assert.Equal(936, list.GetEncoding().CodePage);
    }

    [Fact]
    public void LoadFromFile_MissingFile_ThrowsLikeDelphiFileStream()
    {
        // 原文 TFileStream.Create(FileName, fmOpenRead...) → 文件不存在时抛 EFOpenError
        var list = new TStringList();
        Assert.Throws<FileNotFoundException>(
            () => TStringListHelper.LoadFromFile(list, Path.Combine(_dir, "absent.txt")));
    }

    [Fact]
    public void LoadFromFile_EmptyFile_YieldsNoLines()
    {
        var list = new TStringList();
        TStringListHelper.LoadFromFile(list, WriteBytes("empty.txt", Array.Empty<byte>()));
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void LoadFromFile_AgreesWithTStringListLoadFromFile()
    {
        // 托管 TStringList.LoadFromFile（RTL 语义）与 helper 版（DefaultEncoding 版）在本工程下
        // 默认编码相同 → 结果必须一致
        string path = WriteBytes("agree.txt", TEncoding.UTF8.GetBytes(Sample));
        var a = new TStringList();
        TStringListHelper.LoadFromFile(a, path);
        var b = new TStringList();
        b.LoadFromFile(path);
        Assert.Equal(a.Count, b.Count);
        Assert.Equal(a.Text, b.Text);
        Assert.Equal(a.GetEncoding().CodePage, b.GetEncoding().CodePage);
    }

    // ==================================================================
    // LoadFromStream（原文 :34-51）
    // ==================================================================

    [Fact]
    public void LoadFromStream_NullEncoding_TriggersSniffing()
    {
        var stream = new MemoryStream(TEncoding.UTF8.GetBytes(Sample));
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, null!);
        Assert.Equal(2, list.Count);
        Assert.Equal(65001, list.GetEncoding().CodePage);
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_ExplicitMatchingEncoding_SkipsItsBom()
    {
        byte[] bytes = TEncoding.UTF8.GetPreamble().Concat(TEncoding.UTF8.GetBytes("甲")).ToArray();
        var stream = new MemoryStream(bytes);
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, TEncoding.UTF8);
        Assert.Equal(1, list.Count);
        Assert.Equal("甲", list[0]);
        Assert.Same(TEncoding.UTF8, list.GetEncoding());      // SetEncoding(Encoding) 原样保留
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_ExplicitNonMatchingEncoding_DoesNotSkip()
    {
        var stream = new MemoryStream(GXX.Core.EncodingInit.GBK.GetBytes("甲"));
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, GXX.Core.EncodingInit.GBK);
        Assert.Equal("甲", list[0]);                          // GBK preamble 为空 → 不跳字节
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_UsesListDefaultEncodingAsFallback()
    {
        var stream = new MemoryStream(TEncoding.ASCII.GetBytes("ABC"));
        var list = new TStringList();
        list.DefaultEncoding = TEncoding.ASCII;               // 覆写 TStrings.DefaultEncoding
        TStringListHelper.LoadFromStream(list, stream, null!);
        Assert.Same(TEncoding.ASCII, list.GetEncoding());
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_HonorsStreamPosition()
    {
        // 原文 Size := Stream.Size - Stream.Position（只读**剩余**部分）
        byte[] prefix = GXX.Core.EncodingInit.GBK.GetBytes("ignored");
        byte[] payload = TEncoding.UTF8.GetBytes("甲\r\n");
        var stream = new MemoryStream(prefix.Concat(payload).ToArray())
        {
            Position = prefix.Length,
        };
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, null!);
        Assert.Equal(1, list.Count);
        Assert.Equal("甲", list[0]);
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_EmptyStream_YieldsNoLines()
    {
        var stream = new MemoryStream(Array.Empty<byte>());
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, null!);
        Assert.Equal(0, list.Count);
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_NoBomUtf8_TrailingCrlfDoesNotAddEmptyLine()
    {
        var stream = new MemoryStream(TEncoding.UTF8.GetBytes("a\r\nb\r\n"));
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, null!);
        Assert.Equal(2, list.Count);
        Assert.Equal("b", list[1]);
        stream.Dispose();
    }

    [Fact]
    public void LoadFromStream_SetsEncodingForLaterSave()
    {
        // 原文 :46 注释 "Keep Encoding in case the stream is saved" → 后续 SaveToFile 用同一编码
        byte[] bytes = TEncoding.UTF8.GetPreamble().Concat(TEncoding.UTF8.GetBytes("甲\r\n")).ToArray();
        var stream = new MemoryStream(bytes);
        var list = new TStringList();
        TStringListHelper.LoadFromStream(list, stream, null!);
        stream.Dispose();

        string outPath = Path.Combine(_dir, "saved.txt");
        list.SaveToFile(outPath);
        Assert.Equal(bytes, File.ReadAllBytes(outPath));       // BOM + 内容都写回
    }

    [Fact]
    public void LoadThenSaveThenLoad_ThroughHelper_IsStable()
    {
        string src = WriteBytes("round1.txt", TEncoding.UTF8.GetBytes("一\r\n\r\n三\r\n"));
        var a = new TStringList();
        TStringListHelper.LoadFromFile(a, src);
        string mid = Path.Combine(_dir, "round2.txt");
        a.SaveToFile(mid);
        var b = new TStringList();
        TStringListHelper.LoadFromFile(b, mid);
        Assert.Equal(a.Text, b.Text);
        Assert.Equal(3, b.Count);
    }
}
