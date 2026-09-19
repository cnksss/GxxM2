using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// LoadDxControl.pas 1:1 移植的门禁：LoadComponent 的 23 个分支 + 顶层遍历 + 递归子控件。
// 全部输入都是合成字节（GuiTest 构造），断言读出的字段与**消耗的字节数**。
//
// 构造约定（与原文一致）：
//   * nGuiVersion >= 20160409 时，每个控件头之后一定有 TGuiHeaderAdd（48 字节）；
//   * nGuiVersion >= 20170226 时，TGuiHeader 是 DES 加密的（GuiTest.Header(encrypt: true)）。
// =====================================================================================
public sealed class LoadDxControlLoaderTests
{
    private static (int Result, TDxControl[] Slots, TDxGuiLoaderControl Loader) Load(
        byte[] data, int slotCount = 8, TDxControl background = null, string uiName = "TEST_UI")
    {
        var loader = new TDxGuiLoaderControl();
        var slots = new TDxControl[slotCount];
        var address = new TDxControlAddressList(slots);
        int result = loader.LoadControlFromMemory(address, data, data.Length, background, uiName);
        return (result, slots, loader);
    }

    /// <summary>一个完整的单控件流：FileHeader + GuiHeader + [GuiHeaderAdd] + payload。</summary>
    private static byte[] One(TGuiType gui, byte[] payload, int count = 0, string name = "ctrl",
        int version = GuiTest.Ver20100101, string showName = "", string hint = "")
    {
        var b = GuiTest.New()
            .Raw(GuiTest.FileHeader(version, count))
            .Raw(GuiTest.Header(gui, name, count, left: 11, top: 22, width: 33, height: 44,
                encrypt: version >= GuiTest.Ver20170226).ToArray());
        if (version >= GuiTest.Ver20160409) b.Raw(GuiTest.HeaderAdd(showName, hint).ToArray());
        b.Raw(payload);
        return b.ToArray();
    }

    /// <summary>单控件流的总消费字节数（用于"字节数必须精确"断言）。</summary>
    private static int OneLength(string name, int version, int payloadLength)
    {
        int nameLen = GXX.Core.EncodingInit.GBK.GetByteCount(name);
        return TGuiFileHeader.SizeOf + TGuiHeader.SizeOf + nameLen
             + (version >= GuiTest.Ver20160409 ? TGuiHeaderAdd.SizeOf : 0)
             + payloadLength;
    }

    // ---- 顶层入口 ---------------------------------------------------------------

    [Fact]
    public void LoadControlFromMemory_Returns_nCount_From_FileHeader()
    {
        var (result, _, _) = Load(GuiTest.FileHeader(GuiTest.Ver20100101, 7).ToArray());
        Assert.Equal(7, result);
    }

    [Fact]
    public void LoadControlFromMemory_Empty_Stream_Yields_Zero_Controls()
    {
        var (result, slots, loader) = Load(GuiTest.FileHeader(GuiTest.Ver20100101, 0).ToArray());
        Assert.Equal(0, result);
        Assert.All(slots, s => Assert.Null(s));
        Assert.Equal(TGuiFileHeader.SizeOf, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void LoadControlFromMemory_Null_Argument_Throws_On_Stream_Overload()
    {
        var loader = new TDxGuiLoaderControl();
        Assert.Throws<ArgumentNullException>(() => loader.LoadControlFromStream(null, null, null, "X"));
    }

    [Fact]
    public void LoadControlFromStream_Forwards_Memory_And_Size()
    {
        var data = One(TGuiType.t_Label, new TGuiLabel { CaptionLen = 0 }.ToBytes());
        var loader = new TDxGuiLoaderControl();
        var slots = new TDxControl[4];
        int result = loader.LoadControlFromStream(new MemoryStream(data), null, new TDxControlAddressList(slots), "UI");
        Assert.Equal(0, result);                            // FileHeader.nCount 仍是 0
        Assert.IsType<TDxLabel>(slots[0]);
        Assert.Equal(data.Length, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void LoadControlFromMemory_Reads_Only_Declared_Size_Not_Buffer_Length()
    {
        // Size 参数（不是 buffer.Length）才是硬边界。
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 4 }.ToBytes());
        var padded = data.Concat(new byte[64]).ToArray();
        var loader = new TDxGuiLoaderControl();
        var slots = new TDxControl[4];
        loader.LoadControlFromMemory(new TDxControlAddressList(slots), padded, data.Length, null, "UI");
        Assert.Equal(4, Assert.IsType<TDxImageGrid>(slots[0]).ColCount);
        Assert.Equal(data.Length, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void LoadControlFromMemory_Skips_Group_Names_When_GroupCount_Positive()
    {
        // 原文 LoadDxControl.pas:1701-1712：只有 nGuiVersion >= 20160409 且 GroupCount > 0 才有组名块。
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160409, 1, groupCount: 2));
        b.I32(3).Gbk("abc");                                 // 组名 1
        b.I32(0);                                            // 组名 2 空串（原文走 sText := '' 分支）
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(GuiTest.HeaderAdd().ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 9 }.ToBytes());

        var (_, slots, loader) = Load(b.ToArray());
        Assert.Equal(9, Assert.IsType<TDxImageGrid>(slots[0]).ColCount);
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void LoadControlFromMemory_Group_Names_Ignored_On_Old_Versions()
    {
        // 差异断言：nGuiVersion < 20160409 时没有组名块，GroupCount 虽为 2 也不读。
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1, groupCount: 2));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 5 }.ToBytes());

        var (_, slots, _) = Load(b.ToArray());
        Assert.Equal(5, Assert.IsType<TDxImageGrid>(slots[0]).ColCount);
    }

    [Fact]
    public void LoadControlFromMemory_Enumerates_Multiple_Top_Level_Controls()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 2));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "a").ToArray()).Raw(new TGuiImageGrid { ColCount = 1 }.ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "b").ToArray()).Raw(new TGuiImageGrid { ColCount = 2 }.ToBytes());

        var (_, slots, loader) = Load(b.ToArray());
        Assert.Equal(1, Assert.IsType<TDxImageGrid>(slots[0]).ColCount);
        Assert.Equal(2, Assert.IsType<TDxImageGrid>(slots[1]).ColCount);
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void LoadControlFromMemory_Truncated_Header_Breaks_Loop()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(new byte[TGuiHeader.SizeOf - 4]);

        var (result, slots, _) = Load(b.ToArray(), slotCount: 4);
        Assert.Equal(1, result);
        Assert.All(slots, s => Assert.Null(s));
    }

    [Fact]
    public void LoadControlFromMemory_Header_Is_Decrypted_From_20170226()
    {
        var payload = new TGuiImageGrid { ColCount = 42 }.ToBytes();
        var data = One(TGuiType.t_Grid, payload, name: "g", version: GuiTest.Ver20170226);

        var (_, slots, loader) = Load(data);
        Assert.Equal(42, Assert.IsType<TDxImageGrid>(slots[0]).ColCount);
        Assert.Equal(data.Length, loader.Reader.MemoryPosition);
    }

    // ---- LoadComponent 各分支 ----------------------------------------------------

    [Fact]
    public void Component_Edit_Writes_All_Fields_And_Reads_FontName_Plus_Text()
    {
        var rec = new TGuiEdit
        {
            DrawBorder = true,
            SelectedColor = 0x101010,
            SelBackColor = 0x202020,
            SelFontColor = 0x303030,
            BackgroundColor = 0x404040,
            ReadOnly = true,
            MaxLength = 12,
            InValue = TInValue.vString,
            PasswordChar = (byte)'*',
            AllowSelect = true,
            AllowPaste = true,
            TabOrder = 3,
            TextLen = 2,
        };
        rec.FontColor.NameLen = 4;
        rec.BorderColor.Up.Color = 0x555555;

        var payload = GuiTest.New().Raw(rec.ToBytes()).Gbk("宋体").Gbk("ok").ToArray();
        var (_, slots, loader) = Load(One(TGuiType.t_Edit, payload, name: "ed"));

        var edit = Assert.IsType<TDxEdit>(slots[0]);
        Assert.True(edit.DrawBorder);
        Assert.Equal(0x101010, edit.SelectedColor);
        Assert.Equal(0x202020, edit.SelBackColor);
        Assert.Equal(0x303030, edit.SelFontColor);
        Assert.Equal(0x404040, edit.BackgroundColor);
        Assert.True(edit.ReadOnly);
        Assert.Equal(12, edit.MaxLength);
        Assert.Equal(TInValue.vString, edit.InValue);
        Assert.Equal((byte)'*', edit.PasswordChar);
        Assert.True(edit.AllowSelect);
        Assert.True(edit.AllowPaste);
        Assert.Equal(3, edit.TabOrder);
        Assert.Equal("宋体", edit.Font.Name);
        Assert.Equal("ok", edit.Text);
        Assert.Equal(0x555555, edit.BorderColor.Up.Color);
        Assert.Equal(OneLength("ed", GuiTest.Ver20100101, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Label_Old_Layout_Has_No_Alignment_Field()
    {
        var rec = new TGuiLabel
        {
            AutoSize = true,
            CaptionLen = 0,
            Style = TButtonStyle.bsRadio,
            ClickCount = (GXX.Client.DxComponent.TDxImageButton.TClickSound)3,
        };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();

        var (_, slots, _) = Load(One(TGuiType.t_Label, payload));
        var label = Assert.IsType<TDxLabel>(slots[0]);
        Assert.True(label.AutoSize);
        Assert.Equal(TButtonStyle.bsRadio, label.Style);
        Assert.Equal(3, (int)label.ClickSound);
        Assert.Equal(TDxAlignment.taCenter, label.Alignment);   // TDxImageButton 构造默认，原文此分支未赋值
    }

    [Fact]
    public void Component_Label_New_Layout_Sets_Alignment()
    {
        var rec = new TGuiLabel_New { Alignment = TDxAlignment.taRightJustify, CaptionLen = 0 };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_Label, payload, version: GuiTest.Ver20160409));
        var label = Assert.IsType<TDxLabel>(slots[0]);
        Assert.Equal(TDxAlignment.taRightJustify, label.Alignment);
        Assert.Equal(OneLength("ctrl", GuiTest.Ver20160409, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Button_Old_Layout_And_New2_Share_The_Class_But_Not_The_Record()
    {
        var recOld = new TGuiImageButton { CaptionLen = 0 };
        var payloadOld = GuiTest.New().Raw(recOld.ToBytes()).ToArray();
        var (_, slotsOld, loaderOld) = Load(One(TGuiType.t_Button, payloadOld, version: GuiTest.Ver20171106 - 1));
        Assert.IsType<TDxImageButton>(slotsOld[0]);
        Assert.Equal(OneLength("ctrl", GuiTest.Ver20171106 - 1, TGuiImageButton.SizeOf), loaderOld.Reader.MemoryPosition);

        var recNew = new TGuiImageButton_New2 { CaptionLen = 0 };
        var payloadNew = GuiTest.New().Raw(recNew.ToBytes()).ToArray();
        var (_, slotsNew, loaderNew) = Load(One(TGuiType.t_Button, payloadNew, version: GuiTest.Ver20171106));
        Assert.IsType<TDxImageButton>(slotsNew[0]);
        Assert.Equal(OneLength("ctrl", GuiTest.Ver20171106, TGuiImageButton_New2.SizeOf), loaderNew.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Button_New3_Keeps_Animation_In_Extra()
    {
        var rec = new TGuiImageButton_New3 { CaptionLen = 0 };
        rec.Animation.StartIndex = 5;
        rec.Animation.EndIndex = 9;
        rec.Animation.ShowType = TButtonAnimationShowType.astCheckShow;
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_Button, payload, version: GuiTest.Ver20180619));
        var button = Assert.IsType<TDxImageButton>(slots[0]);
        var extra = TDxControlExtras.Of(button);
        Assert.NotNull(extra.Animation);
        Assert.Equal(5, extra.Animation.StartIndex);
        Assert.Equal(9, extra.Animation.EndIndex);
        Assert.Equal(TButtonAnimationShowType.astCheckShow, extra.Animation.ShowType);
        Assert.Equal(OneLength("ctrl", GuiTest.Ver20180619, TGuiImageButton_New3.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Button_Reads_Caption_And_Four_Font_Names()
    {
        var rec = new TGuiImageButton { CaptionLen = 2 };
        rec.CaptionColor.Up.NameLen = 2;
        rec.CaptionColor.Hot.NameLen = 2;
        rec.CaptionColor.Down.NameLen = 2;
        rec.CaptionColor.Disabled.NameLen = 2;
        var payload = GuiTest.New().Raw(rec.ToBytes()).Gbk("u1").Gbk("h1").Gbk("d1").Gbk("x1").Gbk("OK").ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_Button, payload, name: "bt"));
        var button = Assert.IsType<TDxImageButton>(slots[0]);
        Assert.Equal("u1", button.CaptionColor.Up.Name);
        Assert.Equal("x1", button.CaptionColor.Disabled.Name);
        Assert.Equal("OK", button.Caption);
        Assert.Equal(OneLength("bt", GuiTest.Ver20100101, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_FormShape_Copies_Eight_Items()
    {
        var rec = new TGuiImageFormShape { AutoSize = true };
        rec.ImageIndexs[2].ImageIndex = 77;
        rec.ImageIndexs[7].DestRect = TDxRect.Rect(1, 2, 3, 4);
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_FormShape, payload, name: "fs"));
        var shape = Assert.IsType<TDxImageFormShape>(slots[0]);
        Assert.True(shape.AutoSize);
        Assert.Equal(77, shape.Items[2].ImageIndex);
        Assert.Equal(TDxRect.Rect(1, 2, 3, 4), shape.Items[7].DestRect);
        Assert.Equal(OneLength("fs", GuiTest.Ver20100101, TGuiImageFormShape.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_ListView_Reads_PerColumn_Rects_Then_Fields()
    {
        var rec = new TGuiMemo
        {
            ShowScroll = true,
            ItemHeight = 15,
            ItemIndex = 2,
            ScrollSize = 4,
            ColCount = 2,
            ShowGridLine = true,
            GridLineColor = 0x999999,
            CheckItemControlSize = true,
            ShowItemCount = 6,
        };
        var b = GuiTest.New().Raw(rec.ToBytes());
        b.Raw(GuiTest.New().Rect(TDxRect.Rect(1, 2, 3, 4)).ToArray());
        b.Raw(GuiTest.New().Rect(TDxRect.Rect(5, 6, 7, 8)).ToArray());        for (int i = 0; i < 2; i++)
        {
            var field = new TGuiViewField { Alignment = TDxAlignment.taCenter, CaptionLen = 1 };
            b.Raw(field.ToBytes());
            b.Gbk(i == 0 ? "A" : "B");
        }

        var (_, slots, loader) = Load(One(TGuiType.t_ListView, b.ToArray(), name: "lv"));
        var list = Assert.IsType<TDxListView>(slots[0]);
        Assert.True(list.ShowScroll);
        Assert.Equal(15, list.ItemHeight);
        Assert.Equal(2, list.ColCount);
        Assert.Equal(6, list.ShowItemCount);
        Assert.Equal(TDxRect.Rect(1, 2, 3, 4), list.ColRects[0]);
        Assert.Equal(TDxRect.Rect(5, 6, 7, 8), list.ColRects[1]);
        Assert.Equal("A", list.Fields[0].Caption);
        Assert.Equal("B", list.Fields[1].Caption);
        Assert.Equal(TDxAlignment.taCenter, list.Fields[0].Alignment);
        Assert.Equal(OneLength("lv", GuiTest.Ver20100101, b.Count), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_ListView_With_Zero_Columns_Reads_No_Extra_Blocks()
    {
        var rec = new TGuiMemo { ColCount = 0 };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();
        var (_, slots, loader) = Load(One(TGuiType.t_ListView, payload, name: "lv"));
        Assert.Equal(0, Assert.IsType<TDxListView>(slots[0]).ColCount);
        Assert.Equal(OneLength("lv", GuiTest.Ver20100101, TGuiMemo.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_ChatMemo_New_Layout_Reads_Font_Block()
    {
        var rec = new TGuiMemo_New
        {
            BackGroupColor = 0x111111,
            FontLen = 3,
            FontSize = 12,
            FontStroke = true,
            FontBackTransparent = true,
            ColCount = 0,
        };
        var payload = GuiTest.New().Raw(rec.ToBytes()).Gbk("abc").ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_ChatMemo, payload, version: GuiTest.Ver20160508, name: "cm"));
        var memo = Assert.IsType<TDxChatMemo>(slots[0]);
        Assert.Equal("abc", memo.FontName);
        Assert.Equal(12, memo.FontSize);
        Assert.True(memo.FontStroke);
        Assert.True(memo.FontBackTransparent);
        Assert.Equal(0x111111, memo.BackgroundColor);
        Assert.Equal(OneLength("cm", GuiTest.Ver20160508, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_ChatMemo_New_Layout_Zero_FontLen_Leaves_Empty_Name()
    {
        var rec = new TGuiMemo_New { FontLen = 0 };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();
        var (_, slots, _) = Load(One(TGuiType.t_ChatMemo, payload, version: GuiTest.Ver20160508));
        Assert.Equal(string.Empty, Assert.IsType<TDxChatMemo>(slots[0]).FontName);
    }

    [Fact]
    public void Component_TreeView_Sets_ShowButton()
    {
        var rec = new TGuiMemo_New { ShowButton = true };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();
        var (_, slots, _) = Load(One(TGuiType.t_TreeView, payload, version: GuiTest.Ver20160508));
        Assert.True(Assert.IsType<TDxTreeView>(slots[0]).ShowButton);
    }

    [Fact]
    public void Component_TreeView_Old_Layout_Also_Carries_ShowButton()
    {
        // 差异断言：旧布局 TGuiMemo 也有 ShowButton（偏移 132），新布局在 132 同样位置。
        Assert.Equal(132, TGuiMemo.Layout.OffsetOf("ShowButton"));
        Assert.Equal(132, TGuiMemo_New.Layout.OffsetOf("ShowButton"));

        var rec = new TGuiMemo { ShowButton = true };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();
        var (_, slots, _) = Load(One(TGuiType.t_TreeView, payload));
        Assert.True(Assert.IsType<TDxTreeView>(slots[0]).ShowButton);
    }

    [Fact]
    public void Component_ComboBox_Reads_Eight_Font_Names_Then_Text_And_Items()
    {
        var rec = new TGuiComboBox { TextLen = 2, ItemLen = 3 };
        rec.GuiPopupMenu.ItemColor.Up.NameLen = 2;
        rec.GuiPopupMenu.ItemColor.Hot.NameLen = 2;
        rec.GuiPopupMenu.ItemColor.Down.NameLen = 2;
        rec.GuiPopupMenu.ItemColor.Disabled.NameLen = 2;
        rec.TextColor.Up.NameLen = 2;
        rec.TextColor.Hot.NameLen = 2;
        rec.TextColor.Down.NameLen = 2;
        rec.TextColor.Disabled.NameLen = 2;

        var payload = GuiTest.New().Raw(rec.ToBytes())
            .Gbk("a1").Gbk("a2").Gbk("a3").Gbk("a4")
            .Gbk("b1").Gbk("b2").Gbk("b3").Gbk("b4")
            .Gbk("Tx").Gbk("abc").ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_ComboBox, payload, name: "cb"));
        var combo = Assert.IsType<TDxComboBox>(slots[0]);
        Assert.Equal("b1", combo.TextColor.Up.Name);
        Assert.Equal("b4", combo.TextColor.Disabled.Name);
        Assert.Equal("a1", combo.PopupMenu.ItemColor.Up.Name);
        Assert.Equal("a4", combo.PopupMenu.ItemColor.Disabled.Name);
        Assert.Equal("Tx", combo.Text);
        Assert.Equal("abc", combo.Items.Text);
        Assert.Equal(OneLength("cb", GuiTest.Ver20100101, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_PopupMenu_Reads_Four_Font_Names_Then_Items()
    {
        var rec = new TGuiPopupMenu { SelectColor = 0x1234, ItemHeight = 20, ItemIndex = 1, ItemTextLen = 2 };
        rec.ItemColor.Up.NameLen = 2;
        rec.ItemColor.Hot.NameLen = 2;
        rec.ItemColor.Down.NameLen = 2;
        rec.ItemColor.Disabled.NameLen = 2;
        var payload = GuiTest.New().Raw(rec.ToBytes()).Gbk("f1").Gbk("f2").Gbk("f3").Gbk("f4").Gbk("ok").ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_PopupMenu, payload, name: "pm"));
        var menu = Assert.IsType<TDxPopupMenu>(slots[0]);
        Assert.Equal("f1", menu.ItemColor.Up.Name);
        Assert.Equal("f4", menu.ItemColor.Disabled.Name);
        Assert.Equal("ok", menu.Items.Text);
        Assert.Equal(0x1234, menu.SelectColor);
        Assert.Equal(20, menu.ItemHeight);
        Assert.Equal(OneLength("pm", GuiTest.Ver20100101, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_PageControl_New_Layout_Reads_Five_Extra_Fields()
    {
        var recNew = new TGuiPageControl_New
        {
            ClientLeft = 1,
            ClientTop = 2,
            ClientWidth = 3,
            ClientHeight = 4,
            ButtonWidth = 5,
            ButtonHeight = 6,
            ShowButton = true,
            OffSetX = 7,
            OffSetY = 8,
            CaptionOffsetX = 9,
            CaptionOffsetY = 10,
            DownCaptionOffsetX = 11,
            DownCaptionOffsetY = 12,
            ReverseDrawButton = true,
        };
        var (_, slotsNew, loaderNew) = Load(One(TGuiType.t_PageControl, GuiTest.New().Raw(recNew.ToBytes()).ToArray(),
            version: GuiTest.Ver20160514, name: "pc"));
        var pcNew = Assert.IsType<TDxPageControl>(slotsNew[0]);
        Assert.Equal(9, pcNew.CaptionOffsetX);
        Assert.Equal(11, pcNew.DownCaptionOffsetX);
        Assert.True(pcNew.ReverseDrawButton);
        Assert.Equal(OneLength("pc", GuiTest.Ver20160514, TGuiPageControl_New.SizeOf), loaderNew.Reader.MemoryPosition);

        var recOld = new TGuiPageControl { ButtonWidth = 5, ButtonHeight = 6, OffSetX = 7, OffSetY = 8 };
        var (_, slotsOld, loaderOld) = Load(One(TGuiType.t_PageControl, GuiTest.New().Raw(recOld.ToBytes()).ToArray(),
            version: GuiTest.Ver20160514 - 1, name: "pc"));
        var pcOld = Assert.IsType<TDxPageControl>(slotsOld[0]);
        Assert.Equal(5, pcOld.ButtonWidth);
        Assert.False(pcOld.ReverseDrawButton);      // 差异断言：旧布局不读这 5 个字段
        Assert.Equal(OneLength("pc", GuiTest.Ver20160514 - 1, TGuiPageControl.SizeOf), loaderOld.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_TabSheet_Resets_Host_PageControl_ActivePageIndex()
    {
        var pcRec = new TGuiPageControl_New { ShowButton = true };
        var tsRec = new TGuiTabSheet { OffSetX = 3, OffSetY = 4, HideTable = true, CaptionLen = 0 };

        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160514, 1));
        b.Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 1).ToArray());
        b.Raw(GuiTest.HeaderAdd().ToArray());
        b.Raw(pcRec.ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_TabSheet, "ts").ToArray());
        b.Raw(GuiTest.HeaderAdd().ToArray());
        b.Raw(tsRec.ToBytes());

        var (_, slots, loader) = Load(b.ToArray(), slotCount: 4);
        var pc = Assert.IsType<TDxPageControl>(slots[0]);
        var ts = Assert.IsType<TDxTabSheet>(slots[1]);
        Assert.Equal(pc, ts.DxOwner);
        Assert.False(ts.TabVisible);                      // 原文 TabVisible := not HideTable，HideTable = True
        Assert.Equal(0, pc.ActivePageIndex);              // 原文无条件写 0
        Assert.Equal(3, ts.OffSetX);
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_TabSheet_Without_PageControl_Owner_Does_Not_Throw()
    {
        // 差异（// 原文如此（LoadDxControl.pas:1072））：原文对非 PageControl 宿主硬类型转换会 EInvalidCast；
        // 托管侧 as-转换为 null 后跳过，故不抛异常。
        var tsRec = new TGuiTabSheet { CaptionLen = 0 };
        var payload = GuiTest.New().Raw(tsRec.ToBytes()).ToArray();
        var (_, slots, loader) = Load(One(TGuiType.t_TabSheet, payload, name: "ts"));
        Assert.IsType<TDxTabSheet>(slots[0]);
        Assert.Equal(OneLength("ts", GuiTest.Ver20100101, TGuiTabSheet.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_ImageProgress_Reads_Uint_Fields_And_Font()
    {
        var rec = new TGuiImageProgress
        {
            AutoSize = true,
            ImageBG = 1,
            ImageProgress = 2,
            ImageProgressX = -3,
            ImageProgressY = -4,
            ValueType = TProgressValueType.vtValueAndMax,
            ValueSplite = "-",
            ValueAlignment = TDxAlignment.taCenter,
            ValuePrefix = "<",
            ValueSuffix = ">",
            Max = unchecked((int)4000000000u),
            Min = 1,
            Value = 2000000000,
        };
        rec.Font.NameLen = 2;
        var payload = GuiTest.New().Raw(rec.ToBytes()).Gbk("ab").ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_ImageProgress, payload, name: "ip"));
        var bar = Assert.IsType<TDxImageProgress>(slots[0]);
        var setting = bar.ProgressSetting;
        Assert.True(bar.AutoSize);
        Assert.Equal(1, setting.ImageBG);
        Assert.Equal(-3, setting.ImageProgressX);
        Assert.Equal(TProgressValueType.vtValueAndMax, setting.ValueType);
        Assert.Equal("-", setting.ValueSplite);
        Assert.Equal("<", setting.ValuePrefix);
        Assert.Equal(">", setting.ValueSuffix);
        Assert.Equal(4000000000u, (uint)setting.Max);
        Assert.Equal(2000000000u, (uint)setting.Value);
        Assert.Equal("ab", setting.Font.Name);
        Assert.Equal(OneLength("ip", GuiTest.Ver20100101, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_TrackBar_Min_Max_Position_Order_Is_Preserved()
    {
        var rec = new TGuiTrackBar { AutoSize = true, Min = 5, Max = 100, Position = 50 };
        var payload = GuiTest.New().Raw(rec.ToBytes()).ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_TrackBar, payload, name: "tb"));
        var bar = Assert.IsType<TDXTrackBar>(slots[0]);
        Assert.Equal(5, bar.Min);
        Assert.Equal(100, bar.Max);
        Assert.Equal(50, bar.Position);
        Assert.Equal(OneLength("tb", GuiTest.Ver20100101, TGuiTrackBar.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Line_Style_And_Four_Colors()
    {
        var rec = new TGuiLine { LineStyle = TLineStyle.lsTriangle };
        rec.LineColor.Up.Color = 0x01;
        rec.LineColor.Hot.Color = 0x02;
        rec.LineColor.Down.Color = 0x03;
        rec.LineColor.Disabled.Color = 0x04;
        var (_, slots, loader) = Load(One(TGuiType.t_Line, GuiTest.New().Raw(rec.ToBytes()).ToArray(), name: "ln"));

        var line = Assert.IsType<TDxLine>(slots[0]);
        Assert.Equal(TLineStyle.lsTriangle, line.Style);
        Assert.Equal(0x01, line.LineColor.Up.Color);
        Assert.Equal(0x02, line.LineColor.Hot.Color);
        Assert.Equal(0x03, line.LineColor.Down.Color);
        Assert.Equal(0x04, line.LineColor.Disabled.Color);
        Assert.Equal(OneLength("ln", GuiTest.Ver20100101, TGuiLine.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_MagicBall_Old_And_New_Layouts_Differ()
    {
        var oldRec = new TGuiMagicBall { BallType = TMagicBallType.mbtMP, Overall_EmptyHP = 3 };
        var (_, slotsOld, loaderOld) = Load(One(TGuiType.t_MagicBall, GuiTest.New().Raw(oldRec.ToBytes()).ToArray(), name: "mb"));
        var ballOld = Assert.IsType<TDxMagicBall>(slotsOld[0]);
        Assert.Equal(TMagicBallType.mbtMP, ballOld.BallType);
        Assert.Equal(3, ballOld.OverallSetting.EmptyHP);
        Assert.Equal(OneLength("mb", GuiTest.Ver20100101, TGuiMagicBall.SizeOf), loaderOld.Reader.MemoryPosition);

        var newRec = new TGuiMagicBall2 { Overall_EffectDrawBlend = true, Overall_EffectHPMPStart = 9, Alone_EffectStart = 4 };
        var (_, slotsNew, loaderNew) = Load(One(TGuiType.t_MagicBall, GuiTest.New().Raw(newRec.ToBytes()).ToArray(),
            version: GuiTest.Ver20160818, name: "mb"));
        var ballNew = Assert.IsType<TDxMagicBall>(slotsNew[0]);
        Assert.True(ballNew.OverallSetting.EffectDrawBlend);
        Assert.Equal(9, ballNew.OverallSetting.EffectHPMPStart);
        Assert.Equal(4, ballNew.AloneSetting.EffectStart);
        Assert.Equal(OneLength("mb", GuiTest.Ver20160818, TGuiMagicBall2.SizeOf), loaderNew.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_SexPanel_Typo_Field_UseSettign2()
    {
        // 差异点：记录字段是 UseSettign2（原文笔误），控件属性是 UseSetting2。
        var rec = new TGuiSexPanel { IsMale = true, UseSettign2 = true, Male = 1, Female2 = 2 };
        var (_, slots, loader) = Load(One(TGuiType.t_SexPanel, GuiTest.New().Raw(rec.ToBytes()).ToArray(), name: "sp"));

        var sex = Assert.IsType<TDxSexPanel>(slots[0]);
        Assert.True(sex.IsMale);
        Assert.True(sex.UseSetting2);
        Assert.Equal(1, sex.SexImageSetting.Male);
        Assert.Equal(2, sex.SexImageSetting2.Female);
        Assert.Equal(OneLength("sp", GuiTest.Ver20100101, TGuiSexPanel.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_GroupAttackProgress_Maps_Three_Settings()
    {
        var rec = new TGuiGroupAttackProgress { ProgressAlignment = TMagicBallValueAlignment.mbaTop };
        rec.Settings[0].OffsetX1 = 10;
        rec.Settings[0].OffsetY2 = 20;
        rec.Settings[1].OffsetX3 = 30;
        rec.Settings[2].FlashInterval = 40;
        var (_, slots, loader) = Load(One(TGuiType.t_GroupAttackProgress, GuiTest.New().Raw(rec.ToBytes()).ToArray(), name: "ga"));

        var gap = Assert.IsType<TDxGroupAttackProgress>(slots[0]);
        Assert.Equal(TMagicBallValueAlignment.mbaTop, gap.ProgressAlignment);
        Assert.Equal(10, gap.ContinueSetting.BgOffsetX);
        Assert.Equal(20, gap.ContinueSetting.FlashOffsetY);
        Assert.Equal(30, gap.GroupSetting.ContinueOffsetX);
        Assert.Equal(40, gap.ContinueAndGroupSetting.FlashInterval);
        Assert.Equal(OneLength("ga", GuiTest.Ver20100101, TGuiGroupAttackProgress.SizeOf), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_SwitchButton_Reads_Both_Captions_After_Both_Settings()
    {
        var rec = new TGuiSwitchButton { AutoSize = true };
        rec.CloseSetting.CaptionLen = 2;
        rec.OpenSetting.CaptionLen = 3;
        rec.CloseSetting.ButtonDownOffsetY = 7;
        rec.OpenSetting.DrawAligment = TDrawAligment.daBottom;
        var payload = GuiTest.New().Raw(rec.ToBytes()).Gbk("CL").Gbk("OPN").ToArray();

        var (_, slots, loader) = Load(One(TGuiType.t_SwitchButton, payload, name: "sb"));
        var sw = Assert.IsType<TDxSwitchButton>(slots[0]);
        Assert.True(sw.AutoSize);
        Assert.Equal("CL", sw.CloseSetting.Caption);
        Assert.Equal("OPN", sw.OpenSetting.Caption);
        Assert.Equal(7, sw.CloseSetting.ButtonDownOffsetY);
        Assert.Equal(TDrawAligment.daBottom, sw.OpenSetting.DrawAligment);
        Assert.Equal(OneLength("sb", GuiTest.Ver20100101, payload.Length), loader.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_MainBottomForm_Old_New_And_New2_Layouts()
    {
        var oldRec = new TGuiMainBottomForm { LeftImageType = TImageType.UI1_wil, LeftImageIndex = 1 };
        oldRec.Center.OffsetLeft = -2;
        var (_, slotsOld, loaderOld) = Load(One(TGuiType.t_MainBottomForm, GuiTest.New().Raw(oldRec.ToBytes()).ToArray(), name: "bf"));
        var oldForm = Assert.IsType<TDxMainBottomForm>(slotsOld[0]);
        Assert.Equal(TImageType.UI1_wil, oldForm.LeftImage.ImageType);
        Assert.Equal(-2, oldForm.CenterSetting.OffsetLeft);
        Assert.Equal(OneLength("bf", GuiTest.Ver20100101, TGuiMainBottomForm.SizeOf), loaderOld.Reader.MemoryPosition);

        var newRec = new TGuiMainBottomForm_New();
        newRec.Animation2.HorzAlignment = TDxAlignment.taRightJustify;
        newRec.Animation2.AdjustYByHeight = true;
        var (_, slotsNew, loaderNew) = Load(One(TGuiType.t_MainBottomForm, GuiTest.New().Raw(newRec.ToBytes()).ToArray(),
            version: GuiTest.Ver20190729, name: "bf"));
        var newForm = Assert.IsType<TDxMainBottomForm>(slotsNew[0]);
        Assert.Equal(TDxAlignment.taRightJustify, newForm.Animation2.HorzAlignment);
        Assert.True(newForm.Animation2.AdjustYByHeight);
        Assert.Equal(OneLength("bf", GuiTest.Ver20190729, TGuiMainBottomForm_New.SizeOf), loaderNew.Reader.MemoryPosition);

        var new2Rec = new TGuiMainBottomForm_New2 { BottomImageType = TImageType.UI2_wil, BottomImageIndex = 4 };
        var (_, slotsNew2, loaderNew2) = Load(One(TGuiType.t_MainBottomForm, GuiTest.New().Raw(new2Rec.ToBytes()).ToArray(),
            version: GuiTest.Ver20211120, name: "bf"));
        var new2Form = Assert.IsType<TDxMainBottomForm>(slotsNew2[0]);
        Assert.Equal(4, new2Form.BottomImage.Index);
        Assert.Equal(OneLength("bf", GuiTest.Ver20211120, TGuiMainBottomForm_New2.SizeOf), loaderNew2.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_ImageEdit_Three_Layouts_By_Version()
    {
        // < 20160430：复用 TGuiEdit 布局（原文如此，LoadDxControl.pas:497）
        var asEdit = new TGuiEdit { MaxLength = 9, TextLen = 0 };
        var payload1 = GuiTest.New().Raw(asEdit.ToBytes()).ToArray();
        var (_, slots1, loader1) = Load(One(TGuiType.t_ImageEdit, payload1, version: GuiTest.Ver20160430 - 1, name: "ie"));
        Assert.Equal(9, Assert.IsType<TDxImageEdit>(slots1[0]).MaxLength);
        Assert.Equal(OneLength("ie", GuiTest.Ver20160430 - 1, TGuiEdit.SizeOf), loader1.Reader.MemoryPosition);

        // [20160430, 20190724)：TGuiImageEdit，且 HintText 也读
        var mid = new TGuiImageEdit { MaxLength = 8, TextLen = 0, HintTextLen = 1, DisableBackgroundColor = 0x777777 };
        mid.HintTextFont.NameLen = 0;
        var payload2 = GuiTest.New().Raw(mid.ToBytes()).Gbk("H").ToArray();
        var (_, slots2, loader2) = Load(One(TGuiType.t_ImageEdit, payload2, version: GuiTest.Ver20160430, name: "ie"));
        var midEdit = Assert.IsType<TDxImageEdit>(slots2[0]);
        Assert.Equal(8, midEdit.MaxLength);
        Assert.Equal(0x777777, midEdit.DisableBackgroundColor);
        Assert.Equal("H", midEdit.HintText);
        Assert.Equal(OneLength("ie", GuiTest.Ver20160430, payload2.Length), loader2.Reader.MemoryPosition);

        // >= 20190724：TGuiImageEdit_New
        var neu = new TGuiImageEdit_New { MaxLength = 7, TextLen = 0, HintTextLen = 0, DisableBackgroundAlpha = 5, BackgroundColorAlpha = 6 };
        var payload3 = GuiTest.New().Raw(neu.ToBytes()).ToArray();
        var (_, slots3, loader3) = Load(One(TGuiType.t_ImageEdit, payload3, version: GuiTest.Ver20190724, name: "ie"));
        var newEdit = Assert.IsType<TDxImageEdit>(slots3[0]);
        Assert.Equal(7, newEdit.MaxLength);
        Assert.Equal(5, newEdit.DisableBackgroundAlpha);
        Assert.Equal(6, newEdit.BackgroundColorAlpha);
        Assert.Equal(OneLength("ie", GuiTest.Ver20190724, TGuiImageEdit_New.SizeOf), loader3.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Form_Three_Layouts_By_Version()
    {
        var oldRec = new TGuiImageForm();
        oldRec.ImageIndex.Up = 1;
        var (_, slotsOld, loaderOld) = Load(One(TGuiType.t_Form, GuiTest.New().Raw(oldRec.ToBytes()).ToArray(), name: "f"));
        Assert.Equal(1, Assert.IsType<TDxImageForm>(slotsOld[0]).ImageIndex.Up);
        Assert.Equal(OneLength("f", GuiTest.Ver20100101, TGuiImageForm.SizeOf), loaderOld.Reader.MemoryPosition);

        var midRec = new TGuiImageForm_New { BackgroundAlpha = 200, BackgroundColor = 0xAB };
        var (_, slotsMid, _) = Load(One(TGuiType.t_Form, GuiTest.New().Raw(midRec.ToBytes()).ToArray(),
            version: GuiTest.Ver20160409, name: "f"));
        var mid = Assert.IsType<TDxImageForm>(slotsMid[0]);
        Assert.Equal(200, mid.BackgroundAlpha);
        Assert.Equal(0xAB, mid.BackgroundColor);

        var newRec = new TGuiImageForm_New3();
        newRec.ImageIndex.OffsetX = -5;                  // New3 专有：OffsetX/OffsetY 取代 Hot/Down/Disabled
        newRec.Animation3.PlayCount = 3;
        var (_, slotsNew, loaderNew) = Load(One(TGuiType.t_Form, GuiTest.New().Raw(newRec.ToBytes()).ToArray(),
            version: GuiTest.Ver20171106, name: "f"));
        var neu = Assert.IsType<TDxImageForm>(slotsNew[0]);
        Assert.Equal(-5, neu.ImageIndex.OffsetX);
        Assert.Equal(3, neu.Animation3.PlayCount);
        Assert.Equal(OneLength("f", GuiTest.Ver20171106, TGuiImageForm_New3.SizeOf), loaderNew.Reader.MemoryPosition);
    }

    [Fact]
    public void Component_Form_New3_Does_Not_Touch_Hot_Down_Disabled()
    {
        // 差异断言：New3 的 TGuiImageIndex_Form 没有 Hot/Down/Disabled，故这些保持控件默认值
        // （TDxImageIndex 构造默认 -1，不是 0；所以必须与"新控件默认"比对而不是与 0 比对）。
        var untouched = new TDxImageIndex();
        var payload = GuiTest.New().Raw(new TGuiImageForm_New3().ToBytes()).ToArray();
        var (_, slots, _) = Load(One(TGuiType.t_Form, payload, version: GuiTest.Ver20171106, name: "f"));
        var form = Assert.IsType<TDxImageForm>(slots[0]);
        Assert.Equal(untouched.Hot, form.ImageIndex.Hot);
        Assert.Equal(untouched.Down, form.ImageIndex.Down);
        Assert.Equal(untouched.Disabled, form.ImageIndex.Disabled);
        Assert.Equal(untouched.OffsetY, form.ImageIndex.OffsetY);
    }

    [Fact]
    public void Component_Unknown_Tag_And_t_None_Consume_No_Bytes()
    {
        // 原文如此（LoadDxControl.pas:182）：case 无 else —— t_None 与越界枚举值都不消费 payload。
        foreach (var gui in new[] { TGuiType.t_None, (TGuiType)200 })
        {
            var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
            b.Raw(GuiTest.Header(gui, "x").ToArray());
            b.I32(unchecked((int)0xDEADBEEF));                   // 一段本不该被消费的字节

            var (_, slots, loader) = Load(b.ToArray(), slotCount: 2);
            Assert.Null(slots[0]);                              // NewDxControl 返回 nil
            // 控件头被消费（56 + 40 = 96）；t_None 不建控件也不消费 payload，
            // 剩下 5 字节由顶层 while 的 "读 40 字节失败即 break" 分支顺带吃掉（原文如此）。
            Assert.Equal(b.Count, loader.Reader.MemoryPosition);
        }
    }

    [Fact]
    public void Component_Grid_Reads_Five_Fields()
    {
        var rec = new TGuiImageGrid { ColCount = 3, RowCount = 4, ColWidth = 5, RowHeight = 6, ViewTopLine = 7 };
        var (_, slots, loader) = Load(One(TGuiType.t_Grid, GuiTest.New().Raw(rec.ToBytes()).ToArray(), name: "gd"));

        var grid = Assert.IsType<TDxImageGrid>(slots[0]);
        Assert.Equal(3, grid.ColCount);
        Assert.Equal(4, grid.RowCount);
        Assert.Equal(5, grid.ColWidth);
        Assert.Equal(6, grid.RowHeight);
        Assert.Equal(7, grid.ViewTopLine);
        Assert.Equal(OneLength("gd", GuiTest.Ver20100101, TGuiImageGrid.SizeOf), loader.Reader.MemoryPosition);
    }

    // ---- NewDxControl / 地址表 / 子控件递归 ---------------------------------------

    [Fact]
    public void NewDxControl_Copies_Twelve_Header_Fields()
    {
        var header = TGuiHeader.FromBytes(GuiTest.Header(TGuiType.t_Grid, "n", left: 1, top: 2, width: 3, height: 4,
            enabled: false, visible: false, transparent: true, enableFocus: true, floating: true, ownerMove: true,
            mouseEvents: TMouseEvents.mbLeft).ToArray());

        var control = new TDxGuiLoaderControl().NewDxControl(header, null);
        Assert.Equal(1, control.Left);
        Assert.Equal(2, control.Top);
        Assert.Equal(3, control.Width);
        Assert.Equal(4, control.Height);
        Assert.False(control.Enabled);
        Assert.False(control.Visible);
        Assert.True(control.Transparent);
        Assert.True(control.EnableFocus);
        Assert.True(control.Floating);
        Assert.True(control.OwnerMove);
        Assert.False(control.Designing);
        Assert.Equal(TMouseEvents.mbLeft, control.MouseEvents);
        Assert.Equal(TGuiType.t_Grid, TDxControlExtras.GetGuiType(control));
    }

    [Fact]
    public void NewDxControl_For_t_None_Returns_Null()
    {
        var header = TGuiHeader.FromBytes(GuiTest.Header(TGuiType.t_None, "n").ToArray());
        Assert.Null(new TDxGuiLoaderControl().NewDxControl(header, null));
    }

    [Fact]
    public void NewDxControl_Registers_Linearly_Through_PControlAddress()
    {
        // 原文 LoadDxControl.pas:1594-1596：每建一个控件就写一格并前移。
        var loader = new TDxGuiLoaderControl();
        var slots = new TDxControl[3];
        loader.PControlAddress = new TDxControlAddressList(slots);

        loader.NewDxControl(TGuiHeader.FromBytes(GuiTest.Header(TGuiType.t_Grid, "a").ToArray()), null);
        loader.NewDxControl(TGuiHeader.FromBytes(GuiTest.Header(TGuiType.t_Line, "b").ToArray()), null);
        loader.NewDxControl(TGuiHeader.FromBytes(GuiTest.Header(TGuiType.t_None, "c").ToArray()), null);

        Assert.IsType<TDxImageGrid>(slots[0]);
        Assert.IsType<TDxLine>(slots[1]);
        Assert.Null(slots[2]);                 // t_None 不占格
        Assert.Equal(2, loader.PControlAddress.Index);
    }

    [Fact]
    public void ControlAddressList_Stores_Linearly_And_Overflow_Is_Reported()
    {
        // 原文不判越界；托管侧越界返回 false 并继续推进游标（差异见报告）。
        var slots = new TDxControl[2];
        var list = new TDxControlAddressList(slots);
        Assert.True(list.Store(new TDxImageGrid()));
        Assert.True(list.Store(new TDxImageGrid()));
        Assert.False(list.Store(new TDxImageGrid()));
        Assert.Equal(3, list.Index);
        Assert.NotNull(slots[0]);
        Assert.NotNull(slots[1]);
    }

    [Fact]
    public void ControlAddressList_With_No_Slots_Is_A_NoOp()
    {
        var list = new TDxControlAddressList(null);
        Assert.False(list.Store(new TDxImageGrid()));
        Assert.Equal(1, list.Index);
    }

    [Fact]
    public void LoadSubComponent_With_NameLen_Zero_Still_Reads_Add_And_Component()
    {
        // 差异断言（本车道的核心差异之一）：
        //   LoadDxControl.pas:1636    if DxControl <> nil then                        → NameLen = 0 也继续
        //   LoadDxControlEx.pas:1629  if (DxControl <> nil) and (NameLen > 0) then     → 直接跳过
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160514, 1));
        b.Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 1).ToArray());
        b.Raw(GuiTest.HeaderAdd().ToArray());
        b.Raw(new TGuiPageControl_New().ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "", count: 0).ToArray());   // 子控件名字为空
        b.Raw(GuiTest.HeaderAdd(showName: "SN", hint: "HI").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 6 }.ToBytes());

        var (_, slots, loader) = Load(b.ToArray(), slotCount: 4);
        var grid = Assert.IsType<TDxImageGrid>(slots[1]);
        Assert.Equal(6, grid.ColCount);
        Assert.Equal("SN", grid.ShowName);
        Assert.Equal("HI", grid.HintText);
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);   // 全部消费
    }

    [Fact]
    public void LoadSubComponent_Returns_Parent_Count_Plus_Children_Counts()
    {
        var b = GuiTest.New().Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 2).ToArray());
        b.Raw(new TGuiPageControl().ToBytes());
        for (int i = 0; i < 2; i++)
        {
            b.Raw(GuiTest.Header(TGuiType.t_Grid, "g" + i).ToArray());
            b.Raw(new TGuiImageGrid { ColCount = i + 1 }.ToBytes());
        }

        var slots = new TDxControl[8];
        var loader = new TDxGuiLoaderControl { PControlAddress = new TDxControlAddressList(slots) };
        loader.Reader.Reset(b.ToArray(), b.Count);

        int result = loader.LoadSubComponent(null, GuiTest.Ver20100101, "UI");
        Assert.Equal(2, result);       // 原文 Result := GuiHeader.Count，再加两个返回 0 的子控件
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);
        Assert.IsType<TDxPageControl>(slots[0]);
        Assert.IsType<TDxImageGrid>(slots[1]);
        Assert.Equal(1, Assert.IsType<TDxImageGrid>(slots[1]).ColCount);
        Assert.Equal(2, Assert.IsType<TDxImageGrid>(slots[2]).ColCount);
    }

    [Fact]
    public void LoadSubComponent_Returns_Zero_When_Header_Is_Short()
    {
        var loader = new TDxGuiLoaderControl();
        loader.Reader.Reset(new byte[10], 10);
        Assert.Equal(0, loader.LoadSubComponent(null, GuiTest.Ver20100101, "UI"));
    }

    [Fact]
    public void HintText_Is_Read_From_20160409_Onwards_In_This_Unit()
    {
        // 差异断言：LoadDxControl.pas:1745-1749 没有 20180619 守卫（Ex 有，见 Ex 测试）。
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160409, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(GuiTest.HeaderAdd(hint: "HI").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());

        var (_, slots, loader) = Load(b.ToArray());
        Assert.Equal("HI", slots[0].HintText);
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void HintText_Is_Not_Read_Before_20160409()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());

        var (_, slots, loader) = Load(b.ToArray());
        Assert.Equal(string.Empty, slots[0].HintText);
        Assert.Equal(b.Count, loader.Reader.MemoryPosition);
    }

    [Fact]
    public void TopAlignment_Is_Forced_False_Up_To_20171106()
    {
        foreach (var (version, expected) in new[] { (GuiTest.Ver20171106, false), (GuiTest.Ver20180619, true) })
        {
            var b = GuiTest.New().Raw(GuiTest.FileHeader(version, 1));
            b.Raw(GuiTest.Header(TGuiType.t_Grid, "g", encrypt: version >= GuiTest.Ver20170226).ToArray());
            b.Raw(GuiTest.HeaderAdd(topAlignment: true).ToArray());
            b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());

            var (_, slots, _) = Load(b.ToArray());
            Assert.Equal(expected, slots[0].TopAlignment);
        }
    }

    [Fact]
    public void ReferenceX_And_AdjustYByHeight_Are_Copied_From_HeaderAdd()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160409, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(GuiTest.HeaderAdd(referenceX: TReferenceX.rxRight, adjustYByHeight: true).ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());

        var (_, slots, _) = Load(b.ToArray());
        Assert.Equal(TReferenceX.rxRight, slots[0].ReferenceX);
        Assert.True(slots[0].AdjustYByHeight);
    }

    [Fact]
    public void TopLevel_Controls_Are_Marked_Invisible()
    {
        // 原文 LoadDxControl.pas:1757：顶层控件读完 LoadComponent 前 Visible := False。
        var (_, slots, _) = Load(One(TGuiType.t_Grid, new TGuiImageGrid().ToBytes(), name: "g"));
        Assert.False(slots[0].Visible);
    }

    [Fact]
    public void SubComponents_Keep_Hdr_Visible_Value()
    {
        // 差异断言：Visible := False 只出现在顶层路径（LoadDxControl.pas:1757），子控件走 LoadSubComponent 没有这句。
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 1).ToArray());
        b.Raw(new TGuiPageControl().ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g", visible: true).ToArray());
        b.Raw(new TGuiImageGrid().ToBytes());

        var (_, slots, _) = Load(b.ToArray(), slotCount: 4);
        Assert.False(slots[0].Visible);      // 顶层被强制隐藏
        Assert.True(slots[1].Visible);       // 子控件保持头里的 True
    }

    [Fact]
    public void Control_Names_Are_Decoded_From_Gbk()
    {
        var (_, slots, _) = Load(One(TGuiType.t_Grid, new TGuiImageGrid().ToBytes(), name: "按钮"));
        Assert.Equal("按钮", slots[0].Name);
    }

    [Fact]
    public void OnGetImage_Is_Inherited_From_Background()
    {
        var background = new TDxImageGrid();
        Action<TDxImageIndex, TImageType> hook = (_, __) => { };
        TDxControlExtras.Of(background).OnGetImage = hook;
        background.SetOnGetImage(hook);

        var (_, slots, _) = Load(One(TGuiType.t_Grid, new TGuiImageGrid().ToBytes()), background: background);
        Assert.Same(hook, TDxControlExtras.Of(slots[0]).OnGetImage);
    }

    [Fact]
    public void Truncated_Payload_Is_ZeroFilled_Not_Thrown()
    {
        // 原文如此：LoadComponent 内的记录读取不判短读。这里给不足的 payload，不应抛异常。
        var full = new TGuiImageProgress { Max = 10, Min = 1, Value = 5 }.ToBytes();
        var truncated = full.Take(20).ToArray();
        var (_, slots, _) = Load(One(TGuiType.t_ImageProgress, truncated, name: "ip"));

        var bar = Assert.IsType<TDxImageProgress>(slots[0]);
        Assert.Equal(0u, bar.ProgressSetting.Max);   // 未读到的部分为 0
    }
}
