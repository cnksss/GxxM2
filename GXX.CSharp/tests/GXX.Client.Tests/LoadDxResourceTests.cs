using System;
using System.IO;
using System.Linq;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using GXX.Core.Crypto;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// LoadCompressedUIData（LoadDxControlEx.pas:57-71）与 GuiHeader 的 DES 接缝。
// 这是"客户端全部窗口布局的真源"入口，异常路径必须锁死。
// =====================================================================================
public sealed class LoadDxResourceTests : IDisposable
{
    public void Dispose() => DxGuiResource.Provider = null;

    private static byte[] SampleGui()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 7 }.ToBytes());
        return b.ToArray();
    }

    [Fact]
    public void LoadCompressedUIData_Decompresses_Zlib_Resource_And_Rewinds()
    {
        var raw = SampleGui();
        GuiTest.WithResource("TEST_UI", "ZDAT", GuiTest.Zlib(raw));

        var stream = DxGuiResource.LoadCompressedUIData("TEST_UI", "ZDAT");

        Assert.NotNull(stream);
        Assert.Equal(0, stream.Position);                       // 原文 Result.Position := 0
        Assert.Equal(raw.Length, stream.Length);
        Assert.Equal(raw, stream.ToArray());
    }

    [Fact]
    public void LoadCompressedUIData_Missing_Resource_Returns_Null()
    {
        GuiTest.WithResource("OTHER", "ZDAT", GuiTest.Zlib(SampleGui()));
        Assert.Null(DxGuiResource.LoadCompressedUIData("TEST_UI", "ZDAT"));
    }

    [Fact]
    public void LoadCompressedUIData_No_Provider_Returns_Null()
    {
        DxGuiResource.Provider = null;
        Assert.Null(DxGuiResource.LoadCompressedUIData("TEST_UI", "ZDAT"));
    }

    [Fact]
    public void LoadCompressedUIData_Corrupt_Zlib_Returns_Null()
    {
        GuiTest.WithResource("TEST_UI", "ZDAT", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        Assert.Null(DxGuiResource.LoadCompressedUIData("TEST_UI", "ZDAT"));
    }

    [Fact]
    public void LoadCompressedUIData_Empty_Resource_Is_Still_A_Valid_Zlib_Stream()
    {
        // 空 zlib 流解出来是 0 字节（不是 null）—— 对应原文"资源存在但内容为空"。
        var empty = GXX.Core.Compress.ZlibEx.CompressBuf(Array.Empty<byte>(), 0);
        GuiTest.WithResource("EMPTY", "ZDAT", empty);

        var stream = DxGuiResource.LoadCompressedUIData("EMPTY", "ZDAT");
        Assert.NotNull(stream);
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void LoadCompressedUIData_Wrong_Resource_Type_Returns_Null()
    {
        GuiTest.WithResource("TEST_UI", "ZDAT", GuiTest.Zlib(SampleGui()));
        Assert.Null(DxGuiResource.LoadCompressedUIData("TEST_UI", "RCDATA"));
    }

    [Fact]
    public void Memory_Resource_Provider_Is_Exact_Match_And_Case_Sensitive()
    {
        var provider = new TDxGuiMemoryResourceProvider().Add("A", "ZDAT", new byte[] { 1 });
        Assert.Equal(new byte[] { 1 }, provider.GetResource("A", "ZDAT"));
        Assert.Null(provider.GetResource("a", "ZDAT"));
        Assert.Null(provider.GetResource("A", "zdat"));
    }

    // ---- 端到端：资源 → 解压 → 控件树 ------------------------------------------------

    [Fact]
    public void EndToEnd_From_Zdat_Resource_To_Control_Tree()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 2));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "gd", count: 1).ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "nested").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 4 }.ToBytes());
        GuiTest.WithResource("MIR_CONFIG_DLG_UI", "ZDAT", GuiTest.Zlib(b.ToArray()));

        var stream = DxGuiResource.LoadCompressedUIData("MIR_CONFIG_DLG_UI", "ZDAT");
        var table = new THashedStringList();
        var slot = table.Register("nested");
        int count = new TDxGuiLoaderControlEx(stream).LoadControlFromStream(null, table, "MIR_CONFIG_DLG_UI");

        Assert.Equal(2, count);
        Assert.Equal(4, Assert.IsType<TDxImageGrid>(slot.Value).ColCount);
    }

    // ---- DES 接缝 -------------------------------------------------------------------

    [Fact]
    public void GuiHeaderKey_Is_The_Twelve_Char_Literal_From_The_Original()
    {
        // 原文 #2#1#6#14#20#3#4#1#6#5#10#9
        Assert.Equal(12, DxGuiCrypt.GuiHeaderKey.Length);
        Assert.Equal(new[] { 2, 1, 6, 14, 20, 3, 4, 1, 6, 5, 10, 9 },
            DxGuiCrypt.GuiHeaderKey.Select(c => (int)c).ToArray());
    }

    [Fact]
    public void Encrypt_Then_Decrypt_GuiHeader_Round_Trips()
    {
        var original = GuiTest.Header(TGuiType.t_Label, "abc").ToArray();
        var buffer = (byte[])original.Clone();

        DxGuiCrypt.EncryptGuiHeader(buffer, buffer.Length);
        Assert.NotEqual(original, buffer);

        DxGuiCrypt.DecryptGuiHeader(buffer, buffer.Length);
        Assert.Equal(original, buffer);
        Assert.Equal("abc", TGuiHeader.FromBytes(buffer).NameLen == 3 ? "abc" : "x");
    }

    [Fact]
    public void Decrypt_With_The_Original_Key_Matches_UnitDes_Directly()
    {
        var original = GuiTest.Header(TGuiType.t_Grid, "n").ToArray();
        var viaSeam = (byte[])original.Clone();
        var viaUnitDes = (byte[])original.Clone();

        DxGuiCrypt.DecryptGuiHeader(viaSeam, viaSeam.Length);
        UnitDes.DecryptDes(viaUnitDes, viaUnitDes, viaUnitDes.Length, DxGuiCrypt.GuiHeaderKey);

        Assert.Equal(viaUnitDes, viaSeam);
    }

    [Fact]
    public void Wrong_Key_Produces_Different_Bytes()
    {
        var original = GuiTest.Header(TGuiType.t_Grid, "n").ToArray();
        var wrong = (byte[])original.Clone();
        UnitDes.DecryptDes(wrong, wrong, wrong.Length, "not-the-key");
        var right = (byte[])original.Clone();
        DxGuiCrypt.DecryptGuiHeader(right, right.Length);
        Assert.NotEqual(right, wrong);
    }

    [Fact]
    public void GuiHeader_Size_Is_Two_Des_Blocks()
    {
        // DesCBC 的块大小 BS = 20；解密不会失败的前提正是 40 = 2 × 20。
        Assert.Equal(0, TGuiHeader.SizeOf % UnitDes.BS);
    }
}
