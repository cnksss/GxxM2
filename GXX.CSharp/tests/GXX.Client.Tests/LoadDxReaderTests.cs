using System;
using System.IO;
using System.Linq;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// IGuiReader 两实现（对应 LoadDxControl.pas:61-83 / LoadDxControlEx.pas:74-81）
// 与 GuiReaderExtensions 的读取原语。重点在**短读/负长度**这些原文不判错的边界。
// =====================================================================================
public sealed class LoadDxReaderTests
{
    // ---- TDxMemoryReader（LoadDxControl.pas:61-83）---------------------------------

    [Fact]
    public void MemoryReader_ExactRead_AdvancesPosition()
    {
        var reader = new TDxMemoryReader(new byte[] { 1, 2, 3, 4, 5 }, 5);
        var buf = new byte[3];

        Assert.Equal(3, reader.ReadMemory(buf, 0, 3));
        Assert.Equal(new byte[] { 1, 2, 3 }, buf);
        Assert.Equal(3, reader.Position);
        Assert.Equal(5, reader.Size);
    }

    [Fact]
    public void MemoryReader_ShortRead_Returns_nRem_And_Advances_By_nRem()
    {
        // 原文 LoadDxControl.pas:72-76 的 else 分支。
        var reader = new TDxMemoryReader(new byte[] { 7, 8, 9 }, 3);
        reader.ReadMemory(new byte[2], 0, 2);

        var buf = new byte[4];
        Assert.Equal(1, reader.ReadMemory(buf, 0, 4));
        Assert.Equal(9, buf[0]);
        Assert.Equal(3, reader.Position);
    }

    [Fact]
    public void MemoryReader_AtEnd_ReturnsZero()
    {
        var reader = new TDxMemoryReader(new byte[] { 1 }, 1);
        reader.ReadMemory(new byte[1], 0, 1);
        Assert.Equal(0, reader.ReadMemory(new byte[1], 0, 1));
        Assert.Equal(1, reader.Position);
    }

    [Fact]
    public void MemoryReader_NegativeCount_ReturnsZero_And_Does_Not_Move()
    {
        var reader = new TDxMemoryReader(new byte[] { 1, 2, 3 }, 3);
        Assert.Equal(0, reader.ReadMemory(new byte[4], 0, -1));
        Assert.Equal(0, reader.Position);
    }

    [Fact]
    public void MemoryReader_NegativePosition_ReturnsZero()
    {
        var reader = new TDxMemoryReader(new byte[] { 1, 2, 3 }, 3) { MemoryPosition = -1 };
        Assert.Equal(0, reader.ReadMemory(new byte[4], 0, 2));
    }

    [Fact]
    public void MemoryReader_Declared_Size_Larger_Than_Buffer_Is_Clamped()
    {
        // 差异（// 原文如此（LoadDxControl.pas:70,74））：原文按 MemorySize 走裸指针，
        // 声明长度大于真实缓冲时会 AccessViolation；托管侧以 min 为硬边界，读不到就返回 0。
        var reader = new TDxMemoryReader(new byte[] { 1, 2 }, 100);
        var buf = new byte[8];
        Assert.Equal(2, reader.ReadMemory(buf, 0, 8));
        Assert.Equal(0, reader.ReadMemory(buf, 0, 8));
    }

    [Fact]
    public void MemoryReader_Offset_Into_Target_Buffer_Is_Honoured()
    {
        var reader = new TDxMemoryReader(new byte[] { 5, 6 }, 2);
        var buf = new byte[4];
        Assert.Equal(2, reader.ReadMemory(buf, 2, 2));
        Assert.Equal(new byte[] { 0, 0, 5, 6 }, buf);
    }

    [Fact]
    public void MemoryReader_Reset_Rewinds()
    {
        var reader = new TDxMemoryReader(new byte[] { 1 }, 1);
        reader.ReadMemory(new byte[1], 0, 1);
        reader.Reset(new byte[] { 9, 9 }, 2);
        var buf = new byte[1];
        Assert.Equal(1, reader.ReadMemory(buf, 0, 1));
        Assert.Equal(9, buf[0]);
        Assert.Equal(0, reader.Position - 1);
    }

    // ---- TDxStreamReader（LoadDxControlEx.pas:74-81）--------------------------------

    [Fact]
    public void StreamReader_Reads_And_Tracks_Position_And_Size()
    {
        var reader = new TDxStreamReader(new MemoryStream(new byte[] { 1, 2, 3, 4 }));
        var buf = new byte[2];
        Assert.Equal(2, reader.ReadMemory(buf, 0, 2));
        Assert.Equal(new byte[] { 1, 2 }, buf);
        Assert.Equal(2, reader.Position);
        Assert.Equal(4, reader.Size);
    }

    [Fact]
    public void StreamReader_ShortRead_Returns_Actual_Count()
    {
        var reader = new TDxStreamReader(new MemoryStream(new byte[] { 1, 2, 3 }));
        reader.ReadMemory(new byte[2], 0, 2);
        Assert.Equal(1, reader.ReadMemory(new byte[4], 0, 4));
    }

    [Fact]
    public void StreamReader_NegativeCount_ReturnsZero()
    {
        var reader = new TDxStreamReader(new MemoryStream(new byte[] { 1, 2, 3 }));
        Assert.Equal(0, reader.ReadMemory(new byte[4], 0, -3));
        Assert.Equal(0, reader.Position);
    }

    [Fact]
    public void StreamReader_Null_Stream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TDxStreamReader(null));
    }

    // ---- GuiReaderExtensions --------------------------------------------------------

    [Fact]
    public void ReadRecord_Returns_BytesRead_And_Decodes()
    {
        var bytes = new TGuiImageGrid { ColCount = 3, RowCount = 4, ColWidth = 5, RowHeight = 6, ViewTopLine = 7 }.ToBytes();
        var reader = new TDxMemoryReader(bytes, bytes.Length);

        int read = reader.ReadRecord(TGuiImageGrid.SizeOf, TGuiImageGrid.ReadAt, out var rec);
        Assert.Equal(TGuiImageGrid.SizeOf, read);
        Assert.Equal(3, rec.ColCount);
        Assert.Equal(7, rec.ViewTopLine);
    }

    [Fact]
    public void ReadRecord_ShortRead_ZeroFills_Tail()
    {
        // 原文如此（LoadDxControl.pas:185）：LoadComponent 里记录读取不检查返回值，
        // 短读时 Delphi 局部记录变量尾部是栈残留；托管侧统一 0 填充。
        var bytes = new TGuiImageGrid { ColCount = 3, RowCount = 4, ColWidth = 5, RowHeight = 6, ViewTopLine = 7 }.ToBytes();
        var reader = new TDxMemoryReader(bytes, 8);   // 只给前 8 字节

        int read = reader.ReadRecord(TGuiImageGrid.SizeOf, TGuiImageGrid.ReadAt, out var rec);
        Assert.Equal(8, read);
        Assert.Equal(3, rec.ColCount);     // 前两个字段读到了
        Assert.Equal(0, rec.ColWidth);     // 未读到的部分为 0（不是随机栈值）
        Assert.Equal(0, rec.ViewTopLine);
    }

    [Fact]
    public void ReadExact_Returns_Boolean_Guard_Used_By_Header_Reads()
    {
        var header = GuiTest.Header(TGuiType.t_Label, "abc").ToArray();
        var okReader = new TDxMemoryReader(header, header.Length);
        Assert.True(okReader.ReadExact(TGuiHeader.SizeOf, TGuiHeader.ReadAt, out var good));
        Assert.Equal(TGuiType.t_Label, good.Gui);
        Assert.Equal(3, good.NameLen);

        var shortReader = new TDxMemoryReader(header, TGuiHeader.SizeOf - 4);
        Assert.False(shortReader.ReadExact(TGuiHeader.SizeOf, TGuiHeader.ReadAt, out _));
    }

    [Fact]
    public void ReadInt32_Used_For_Group_Lengths()
    {
        var bytes = BitConverter.GetBytes(0x01020304);
        Assert.True(new TDxMemoryReader(bytes, 4).ReadInt32(out int value));
        Assert.Equal(0x01020304, value);

        Assert.False(new TDxMemoryReader(new byte[] { 1, 2 }, 2).ReadInt32(out _));
    }

    [Fact]
    public void ReadFixedString_Decodes_Gbk_By_Byte_Length()
    {
        var text = "标题";
        var raw = GXX.Core.EncodingInit.GBK.GetBytes(text);
        var reader = new TDxMemoryReader(raw, raw.Length);
        Assert.Equal(text, reader.ReadFixedString(raw.Length));
    }

    [Fact]
    public void ReadFixedString_Zero_Or_Negative_Length_Consumes_Nothing()
    {
        var reader = new TDxMemoryReader(new byte[] { 1, 2, 3 }, 3);
        Assert.Equal(string.Empty, reader.ReadFixedString(0));
        Assert.Equal(string.Empty, reader.ReadFixedString(-5));
        Assert.Equal(0, reader.Position);
    }

    [Fact]
    public void ReadFixedString_Short_Read_Still_Returns_Length_Bytes()
    {
        // 原文 SetLength + ReadMemory 短读时，未填字节保持 #0；这里 0 填充后按 len 解码。
        var reader = new TDxMemoryReader(new byte[] { 0x41, 0x42 }, 2);
        var s = reader.ReadFixedString(4);
        Assert.Equal(4, s.Length);
        Assert.Equal("AB\0\0", s);
    }

    [Fact]
    public void ReadGuiFontName_Zero_NameLen_Consumes_Nothing()
    {
        // 原文 LoadDxControl.pas:108-113：NameLen = 0 时**不消费任何字节**。
        var reader = new TDxMemoryReader(new byte[] { 0x41, 0x42 }, 2);
        Assert.Equal(string.Empty, reader.ReadGuiFontName(new TGuiFont { NameLen = 0 }));
        Assert.Equal(0, reader.Position);
    }

    [Fact]
    public void ReadGuiFontName_Positive_NameLen_Consumes_Exactly_NameLen()
    {
        var raw = GXX.Core.EncodingInit.GBK.GetBytes("宋体");
        var reader = new TDxMemoryReader(raw, raw.Length);
        Assert.Equal("宋体", reader.ReadGuiFontName(new TGuiFont { NameLen = raw.Length }));
        Assert.Equal(raw.Length, reader.Position);
    }

    [Fact]
    public void ReadGuiFontName_Overlong_NameLen_Pads_With_Zeros()
    {
        // 原文 SetLength(Text, NameLen) 会按声明长度分配，短读留下的字节是 #0。
        var reader = new TDxMemoryReader(new byte[] { 0x41 }, 1);
        Assert.Equal("A\0\0\0\0", reader.ReadGuiFontName(new TGuiFont { NameLen = 5 }));
        Assert.Equal(1, reader.Position);
    }
}

// =====================================================================================
// DxGuiFonts：LoadDxControl.pas:85-102 / LoadDxControlEx.pas:83-100
// =====================================================================================
public sealed class LoadDxFontTests
{
    [Fact]
    public void DxFontAssign_Copies_Five_Fields()
    {
        var gui = new TGuiFont { Color = 0x112233, BColor = 0x445566, Size = 14, Bold = true, Style = 0b0001, NameLen = 0 };
        var dx = new TDxFont();

        DxGuiFonts.DxFontAssign(dx, gui);

        Assert.Equal(0x112233, dx.Color);
        Assert.Equal(0x445566, dx.BColor);
        Assert.Equal(14, dx.Size);
        Assert.True(dx.Bold);
        Assert.Equal(new[] { "Bold" }, dx.Style.ToArray());
    }

    [Fact]
    public void DxFontAssign_Style_Bits_Map_To_FontStyle_Names()
    {
        // Delphi TFontStyle = (fsBold, fsItalic, fsUnderline, fsStrikeOut) → 位 0..3。
        var dx = new TDxFont();
        DxGuiFonts.DxFontAssign(dx, new TGuiFont { Style = 0b1111 });
        Assert.Equal(new[] { "Bold", "Italic", "Underline", "StrikeOut" }, dx.Style.ToArray());
    }

    [Fact]
    public void DxFontAssign_Zero_Style_Clears_Previous_Style()
    {
        var dx = new TDxFont();
        dx.StyleSet = new[] { "Bold", "Italic" };
        DxGuiFonts.DxFontAssign(dx, new TGuiFont { Style = 0 });
        Assert.Empty(dx.Style);
    }

    [Fact]
    public void GuiFontAssign_Counts_NameLen_In_Gbk_Bytes()
    {
        // 原文 GuiFont.NameLen := Length(DxFont.Name)；Delphi 的 Length(AnsiString) 是**字节**数。
        var gui = new TGuiFont();
        var dx = new TDxFont { Name = "宋体", Color = 1, BColor = 2, Size = 9, Bold = true };

        DxGuiFonts.GuiFontAssign(gui, dx);

        Assert.Equal(4, gui.NameLen);     // 2 个汉字 × 2 字节
        Assert.Equal(1, gui.Color);
        Assert.Equal(2, gui.BColor);
        Assert.Equal(9, gui.Size);
        Assert.True(gui.Bold);
    }

    [Fact]
    public void GuiFontAssign_Ascii_Name_Length_Is_Char_Count()
    {
        var gui = new TGuiFont();
        DxGuiFonts.GuiFontAssign(gui, new TDxFont { Name = "MS Sans" });
        Assert.Equal(7, gui.NameLen);
    }

    [Fact]
    public void GuiFontAssign_Null_Name_Yields_Zero_NameLen()
    {
        var gui = new TGuiFont();
        DxGuiFonts.GuiFontAssign(gui, new TDxFont { Name = null });
        Assert.Equal(0, gui.NameLen);
    }

    [Fact]
    public void GuiFontAssign_Style_Bits_Are_Rebuilt_From_Names()
    {
        var gui = new TGuiFont();
        var dx = new TDxFont();
        dx.StyleSet = new[] { "Italic", "StrikeOut" };
        DxGuiFonts.GuiFontAssign(gui, dx);
        Assert.Equal(0b1010, gui.Style);
    }

    [Fact]
    public void StyleToNames_And_NamesToStyle_Are_Inverse()
    {
        for (int bits = 0; bits < 16; bits++)
        {
            var names = DxGuiFonts.StyleToNames((byte)bits);
            Assert.Equal((byte)bits, DxGuiFonts.NamesToStyle(names));
        }
    }

    [Fact]
    public void NamesToStyle_Unknown_Names_Are_Ignored_And_Null_Is_Safe()
    {
        Assert.Equal(0, DxGuiFonts.NamesToStyle(new[] { "Nope" }));
        Assert.Equal(0, DxGuiFonts.NamesToStyle(null));
        Assert.Equal(0b0001, DxGuiFonts.NamesToStyle(new[] { "bold", "Nope" }));   // 大小写不敏感
    }
}
