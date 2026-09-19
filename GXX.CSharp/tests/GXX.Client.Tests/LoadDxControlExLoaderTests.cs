using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using Xunit;

namespace GXX.Client.Tests;

// =====================================================================================
// LoadDxControlEx.pas 1:1 移植的门禁。本文件同时是**两单元差异**的守卫：
//   1. NameLen = 0 → Ex 直接跳过（连 GuiHeaderAdd / LoadComponent 都不执行）
//   2. HintText 只在 nGuiVersion >= 20180619 才读
//   3. 名字不在 ControlAddrList 里 → 控件读完立刻 Free
//   4. Patch* 版本"先递归后登记"，且槽位非空不覆盖
// =====================================================================================
public sealed class LoadDxControlExLoaderTests
{
    private static TDxGuiLoaderControlEx Loader(byte[] data)
        => new(new MemoryStream(data));

    /// <summary>一个完整的单控件流（Ex 侧版本，含 >= 20160409 的 GuiHeaderAdd）。</summary>
    private static byte[] One(TGuiType gui, byte[] payload, int count = 0, string name = "ctrl",
        int version = GuiTest.Ver20100101, string showName = "", string hint = "")
    {
        var b = GuiTest.New()
            .Raw(GuiTest.FileHeader(version, count))
            .Raw(GuiTest.Header(gui, name, count, left: 7, top: 8, width: 9, height: 10,
                encrypt: version >= GuiTest.Ver20170226).ToArray());
        if (version >= GuiTest.Ver20160409) b.Raw(GuiTest.HeaderAdd(showName, hint).ToArray());
        b.Raw(payload);
        return b.ToArray();
    }

    private static THashedStringList Table(params string[] names)
    {
        var table = new THashedStringList();
        foreach (var n in names) table.Register(n);
        return table;
    }

    // ---- THashedStringList / TDxControlRef -----------------------------------------

    [Fact]
    public void HashedStringList_IndexOf_Is_Case_Sensitive_And_MinusOne_When_Absent()
    {
        var table = Table("MainForm", "btnOk");
        Assert.Equal(0, table.IndexOf("MainForm"));
        Assert.Equal(1, table.IndexOf("btnOk"));
        Assert.Equal(-1, table.IndexOf("mainform"));
        Assert.Equal(-1, table.IndexOf("nope"));
        Assert.Equal(2, table.Count);
    }

    [Fact]
    public void HashedStringList_Objects_Holds_The_Same_Slots()
    {
        var table = Table("a", "b");
        Assert.Equal(2, table.Objects.Count);
        Assert.IsType<TDxControlRef>(table.Objects[0]);
        Assert.Null(((TDxControlRef)table.Objects[0]).Value);
    }

    [Fact]
    public void ControlRef_Is_A_Mutable_Slot()
    {
        var reference = new TDxControlRef();
        Assert.Null(reference.Value);
        reference.Value = new TDxImageGrid();
        Assert.IsType<TDxImageGrid>(reference.Value);
        Assert.IsType<TDxControlRef>(new TDxControlRef(new TDxLine()));
    }

    // ---- LoadControlFromStream ------------------------------------------------------

    [Fact]
    public void LoadControlFromStream_Returns_nCount_And_Registers_By_Name()
    {
        var table = Table("g");
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 4 }.ToBytes(), count: 3, name: "g");

        int result = Loader(data).LoadControlFromStream(null, table, "UI");

        Assert.Equal(3, result);                                  // FileHeader.nCount
        var grid = Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal(4, grid.ColCount);
        Assert.Equal("g", grid.Name);
    }

    [Fact]
    public void LoadControlFromStream_Unregistered_Name_Is_Disposed_But_Stream_Still_Consumed()
    {
        // 原文 LoadDxControlEx.pas:1785-1787：pControlAddr = nil → DxControl.Free（为继续读文件数据）。
        var table = Table("other");
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 4 }.ToBytes(), name: "g");
        var loader = Loader(data);

        loader.LoadControlFromStream(null, table, "UI");

        Assert.Null(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal(data.Length, loader.Reader.Position);         // 字节仍然全部消费
    }

    [Fact]
    public void LoadControlFromStream_Null_Table_Is_Safe()
    {
        // 差异（// 原文如此（LoadDxControlEx.pas:1740））：原文用 ControlAddrList.IndexOf 直接解引用，
        // 传 nil 会 AccessViolation；托管侧用 ?. 兜住（测试里显式覆盖这条路径）。
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 4 }.ToBytes(), name: "g");
        var loader = Loader(data);
        loader.LoadControlFromStream(null, null, "UI");
        Assert.Equal(data.Length, loader.Reader.Position);
    }

    [Fact]
    public void LoadControlFromStream_Group_Names_Are_Consumed()
    {
        var table = Table("g");
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160409, 1, groupCount: 2));
        b.I32(4).Gbk("abcd").I32(0);
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(GuiTest.HeaderAdd().ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 8 }.ToBytes());

        Loader(b.ToArray()).LoadControlFromStream(null, table, "UI");
        Assert.Equal(8, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[0]).Value).ColCount);
    }

    [Fact]
    public void LoadControlFromStream_Enumerates_Until_Stream_End()
    {
        var table = Table("a", "b");
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 2));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "a").ToArray()).Raw(new TGuiImageGrid { ColCount = 1 }.ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "b").ToArray()).Raw(new TGuiImageGrid { ColCount = 2 }.ToBytes());

        Loader(b.ToArray()).LoadControlFromStream(null, table, "UI");
        Assert.Equal(1, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[0]).Value).ColCount);
        Assert.Equal(2, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[1]).Value).ColCount);
    }

    [Fact]
    public void LoadControlFromStream_Encrypted_Header_Round_Trips()
    {
        var table = Table("g");
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 11 }.ToBytes(), name: "g", version: GuiTest.Ver20170226);

        Loader(data).LoadControlFromStream(null, table, "UI");
        Assert.Equal(11, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[0]).Value).ColCount);
    }

    [Fact]
    public void LoadControlFromStream_Constructor_Rejects_Null_Stream()
    {
        Assert.Throws<ArgumentNullException>(() => new TDxGuiLoaderControlEx(null));
    }

    // ---- 差异 1：NameLen = 0 --------------------------------------------------------

    [Fact]
    public void NameLen_Zero_Skips_GuiHeaderAdd_And_Payload_Entirely()
    {
        // 差异断言（LoadDxControlEx.pas:1629/1732）：
        //   if (DxControl <> nil) and (GuiHeader.NameLen > 0) then ...
        // NameLen = 0 → 连 GuiHeaderAdd 与 LoadComponent 都不执行，**一个字节都不多消费**。
        // 对照 LoadDxControlLoaderTests.LoadSubComponent_With_NameLen_Zero_Still_Reads_Add_And_Component：
        // 基础版对同一输入会一路读到 payload 结束。
        var b = GuiTest.New()
            .Raw(GuiTest.Header(TGuiType.t_Grid, "", count: 0).ToArray())   // NameLen = 0
            .Raw(GuiTest.HeaderAdd(showName: "SN").ToArray())
            .Raw(new TGuiImageGrid { ColCount = 6 }.ToBytes());

        var loader = Loader(b.ToArray());
        int result = loader.LoadSubComponent(null, Table(), GuiTest.Ver20160409, "UI");

        Assert.Equal(0, result);
        Assert.Equal(TGuiHeader.SizeOf, loader.Reader.Position);   // 只吃掉 40 字节的头
    }

    [Fact]
    public void NameLen_Zero_In_SubComponent_Also_Skips()
    {
        // 同一差异在顶层路径上的表现：父控件（有名字）正常加载，子控件（NameLen = 0）被跳过。
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160514, 1));
        b.Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 1).ToArray());
        b.Raw(GuiTest.HeaderAdd().ToArray());
        b.Raw(new TGuiPageControl_New().ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "", count: 0).ToArray());     // 子控件 NameLen = 0
        b.Raw(GuiTest.HeaderAdd(showName: "SN").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 6 }.ToBytes());

        var table = Table("pc");
        var loader = Loader(b.ToArray());
        loader.LoadControlFromStream(null, table, "UI");

        var pc = Assert.IsType<TDxPageControl>(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal("pc", pc.Name);
        // 子控件（NameLen = 0）被整块跳过：它在表里没有名字，也永远不会被登记；
        // 剩余的 HeaderAdd + payload 由顶层 while 循环的短读吃掉（原文同样的行为）。
        Assert.Single(table.Objects);
        Assert.True(loader.Reader.Position > TGuiFileHeader.SizeOf + TGuiHeader.SizeOf);
    }

    // ---- 差异 2：HintText 的版本界 ---------------------------------------------------

    [Fact]
    public void HintText_Is_Not_Read_At_20160409()
    {
        // 差异断言：Ex 在 < 20180619 时**不读** HintText —— 于是 HintText 的字节会被
        // 当成 payload 的开始，读出垃圾（这里明确验证 Hint 没被赋值）。
        var table = Table("g");
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160409, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(GuiTest.HeaderAdd(hint: "HI").ToArray());   // HintTextLen = 2，但 Ex 不会读
        b.Raw(new byte[16]);                              // 用 0 填充，保证错位后仍能读完

        Loader(b.ToArray()).LoadControlFromStream(null, table, "UI");
        var grid = Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal(string.Empty, grid.HintText);
    }

    [Fact]
    public void HintText_Is_Read_From_20180619()
    {
        var table = Table("g");
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20180619, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g", encrypt: true).ToArray());
        b.Raw(GuiTest.HeaderAdd(hint: "HI").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());

        Loader(b.ToArray()).LoadControlFromStream(null, table, "UI");
        var grid = Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal("HI", grid.HintText);
        Assert.Equal(3, grid.ColCount);
    }

    [Fact]
    public void ShowName_Is_Always_Read_From_20160409_Regardless_Of_Hint()
    {
        var table = Table("g");
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20160409, 1));
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(GuiTest.HeaderAdd(showName: "SN").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 3 }.ToBytes());

        Loader(b.ToArray()).LoadControlFromStream(null, table, "UI");
        Assert.Equal("SN", ((TDxControlRef)table.Objects[0]).Value.ShowName);
    }

    // ---- 差异 3/4：Patch* 版本 ------------------------------------------------------

    [Fact]
    public void PatchLoadControlFromStream_Registers_After_Recursion()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 1).ToArray());
        b.Raw(new TGuiPageControl().ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g", count: 0).ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 5 }.ToBytes());

        var table = Table("pc", "g");
        var loader = Loader(b.ToArray());
        loader.PatchLoadControlFromStream(null, table, "UI");

        Assert.IsType<TDxPageControl>(((TDxControlRef)table.Objects[0]).Value);
        Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[1]).Value);
        Assert.Equal(b.Count, loader.Reader.Position);
    }

    [Fact]
    public void PatchLoadControlFromStream_Does_Not_Overwrite_An_Occupied_Slot()
    {
        // 差异断言（LoadDxControlEx.pas:2454-2456）：Patch 版只在槽为空时写入。
        var existing = new TDxImageGrid();
        var table = Table("g");
        ((TDxControlRef)table.Objects[0]).Value = existing;

        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 5 }.ToBytes(), name: "g");
        Loader(data).PatchLoadControlFromStream(null, table, "UI");

        Assert.Same(existing, ((TDxControlRef)table.Objects[0]).Value);
    }

    [Fact]
    public void PatchLoadControlFromStream_Unknown_Name_Is_Disposed()
    {
        var table = Table("other");
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 5 }.ToBytes(), name: "g");
        var loader = Loader(data);

        loader.PatchLoadControlFromStream(null, table, "UI");

        Assert.Null(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal(data.Length, loader.Reader.Position);
    }

    [Fact]
    public void Normal_Version_Overwrites_An_Occupied_Slot_While_Patch_Does_Not()
    {
        // 差异断言：正式版 LoadControlFromStream 无条件 <c>pControlAddr^.Value := DxControl</c>
        // （LoadDxControlEx.pas:1747-1749），Patch 版只在槽为空时写（:2454-2456）。
        var existingNormal = new TDxImageGrid();
        var tableNormal = Table("g");
        ((TDxControlRef)tableNormal.Objects[0]).Value = existingNormal;
        var data = One(TGuiType.t_Grid, new TGuiImageGrid { ColCount = 5 }.ToBytes(), name: "g");
        Loader(data).LoadControlFromStream(null, tableNormal, "UI");
        Assert.NotSame(existingNormal, ((TDxControlRef)tableNormal.Objects[0]).Value);

        var existingPatch = new TDxImageGrid();
        var tablePatch = Table("g");
        ((TDxControlRef)tablePatch.Objects[0]).Value = existingPatch;
        Loader(data).PatchLoadControlFromStream(null, tablePatch, "UI");
        Assert.Same(existingPatch, ((TDxControlRef)tablePatch.Objects[0]).Value);
    }

    [Fact]
    public void Patch_Registers_Both_Parent_And_Children()
    {
        var b = GuiTest.New().Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 1).ToArray());
        b.Raw(new TGuiPageControl().ToBytes());
        b.Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 5 }.ToBytes());

        var table = Table("pc", "g");
        var loader = Loader(b.ToArray());
        loader.PatchLoadSubComponent(null, table, GuiTest.Ver20100101);

        Assert.IsType<TDxPageControl>(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal(5, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[1]).Value).ColCount);
        Assert.Equal(b.Count, loader.Reader.Position);
    }

    [Fact]
    public void PatchLoadSubComponent_Unknown_Name_Is_Disposed()
    {
        var table = Table("other");
        var b = GuiTest.New().Raw(GuiTest.Header(TGuiType.t_Grid, "g").ToArray());
        b.Raw(new TGuiImageGrid { ColCount = 5 }.ToBytes());
        var loader = Loader(b.ToArray());

        loader.PatchLoadSubComponent(null, table, GuiTest.Ver20100101);

        Assert.Null(((TDxControlRef)table.Objects[0]).Value);
        Assert.Equal(b.Count, loader.Reader.Position);
    }

    // ---- 与基础版一致的 LoadComponent 行为（抽样） -----------------------------------

    [Fact]
    public void Ex_LoadComponent_Uses_The_Same_Widget_Mapping_As_Base()
    {
        var table = Table("lbl", "bar", "tp");
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 3));

        var label = new TGuiLabel { CaptionLen = 2, AutoSize = true };
        label.CaptionColor.Up.NameLen = 0;
        label.CaptionColor.Hot.NameLen = 0;
        label.CaptionColor.Down.NameLen = 0;
        label.CaptionColor.Disabled.NameLen = 0;
        b.Raw(GuiTest.Header(TGuiType.t_Label, "lbl").ToArray()).Raw(label.ToBytes()).Gbk("Hi");

        var bar = new TGuiImageProgress { Max = 9, Min = 1, Value = 3, ValueType = TProgressValueType.vtValue };
        b.Raw(GuiTest.Header(TGuiType.t_ImageProgress, "bar").ToArray()).Raw(bar.ToBytes());

        var track = new TGuiTrackBar { Min = 2, Max = 8, Position = 5 };
        b.Raw(GuiTest.Header(TGuiType.t_TrackBar, "tp").ToArray()).Raw(track.ToBytes());

        Loader(b.ToArray()).LoadControlFromStream(null, table, "UI");

        Assert.Equal("Hi", Assert.IsType<TDxLabel>(((TDxControlRef)table.Objects[0]).Value).Caption);
        var progress = Assert.IsType<TDxImageProgress>(((TDxControlRef)table.Objects[1]).Value);
        Assert.Equal(9u, progress.ProgressSetting.Max);
        Assert.Equal(TProgressValueType.vtValue, progress.ProgressSetting.ValueType);
        var trackBar = Assert.IsType<TDXTrackBar>(((TDxControlRef)table.Objects[2]).Value);
        Assert.Equal(2, trackBar.Min);
        Assert.Equal(8, trackBar.Max);
        Assert.Equal(5, trackBar.Position);
    }

    [Fact]
    public void Ex_Unknown_Tag_Consumes_No_Bytes()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(GuiTest.Header(TGuiType.t_None, "x").ToArray());
        b.I32(unchecked((int)0x1234));

        var loader = Loader(b.ToArray());
        var table = Table();
        loader.LoadControlFromStream(null, table, "UI");

        // 控件头被消费（56 + 40 = 96），t_None 不建控件也不消费 payload；
        // 剩下的 5 字节由顶层 while 的短读吃掉（原文如此，见报告"缺陷"节）。
        Assert.Empty(table.Objects);
        Assert.Equal(b.Count, loader.Reader.Position);
    }

    [Fact]
    public void Ex_SubComponent_Returns_Count_Plus_Children()
    {
        var b = GuiTest.New().Raw(GuiTest.Header(TGuiType.t_PageControl, "pc", count: 2).ToArray());
        b.Raw(new TGuiPageControl().ToBytes());
        for (int i = 0; i < 2; i++)
        {
            b.Raw(GuiTest.Header(TGuiType.t_Grid, "g" + i).ToArray());
            b.Raw(new TGuiImageGrid { ColCount = i + 1 }.ToBytes());
        }

        var table = Table("pc", "g0", "g1");
        var loader = Loader(b.ToArray());
        int result = loader.LoadSubComponent(null, table, GuiTest.Ver20100101, "UI");

        Assert.Equal(2, result);
        Assert.Equal(1, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[1]).Value).ColCount);
        Assert.Equal(2, Assert.IsType<TDxImageGrid>(((TDxControlRef)table.Objects[2]).Value).ColCount);
        Assert.Equal(b.Count, loader.Reader.Position);
    }

    [Fact]
    public void Ex_Truncated_Header_Breaks_Loop()
    {
        var b = GuiTest.New().Raw(GuiTest.FileHeader(GuiTest.Ver20100101, 1));
        b.Raw(new byte[TGuiHeader.SizeOf - 1]);

        var loader = Loader(b.ToArray());
        int result = loader.LoadControlFromStream(null, Table(), "UI");
        Assert.Equal(1, result);
        Assert.Equal(b.Count, loader.Reader.Position);
    }
}
