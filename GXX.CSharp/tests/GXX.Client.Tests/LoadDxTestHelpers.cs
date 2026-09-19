using System;
using System.Collections.Generic;
using System.IO;
using GXX.Client.DxComponent;
using GXX.Client.LoadDx;
using GXX.Core.Compress;

namespace GXX.Client.Tests;

// =====================================================================================
// LoadDx 车道的**合成字节**构造器。
//
// 为什么必须合成：仓库里没有随客户端发布的那批 'ZDAT' .GUI 资源（它们编译在 Client.exe 的
// 资源段里），所以本车道全部测试都用按格式手工拼出来的字节，逐字段断言偏移与宽度，
// 不依赖任何仓库外真实资源。
//
// 全部字节都由 GuiRecords 的 WriteAt 产出（与 LoadDxControl*.pas 的读取端互为镜像），
// 因此"格式"这一层是自洽的：写→读往返 + 逐字段偏移断言。
// =====================================================================================

internal sealed class GuiBytes
{
    private readonly List<byte> _bytes = new();

    public int Count => _bytes.Count;

    public GuiBytes Raw(byte[] value)
    {
        if (value != null) _bytes.AddRange(value);
        return this;
    }

    public GuiBytes I32(int value)
    {
        _bytes.AddRange(BitConverter.GetBytes(value));
        return this;
    }

    public GuiBytes U16(ushort value)
    {
        _bytes.AddRange(BitConverter.GetBytes(value));
        return this;
    }

    /// <summary>Windows.TRect 的内存像（Left/Top/Right/Bottom，各 4 字节，共 16）。</summary>
    public GuiBytes Rect(TDxRect rect)
    {
        I32(rect.Left).I32(rect.Top).I32(rect.Right).I32(rect.Bottom);
        return this;
    }

    /// <summary>原始 GBK 字节（Delphi 里 <c>SetLength(sText, Len); ReadMemory(sText[1], Len)</c> 的对象）。</summary>
    public GuiBytes Gbk(string text)
    {
        _bytes.AddRange(GXX.Core.EncodingInit.GBK.GetBytes(text ?? string.Empty));
        return this;
    }

    public GuiBytes Gbk(string text, int fixedLen)
    {
        var raw = GXX.Core.EncodingInit.GBK.GetBytes(text ?? string.Empty);
        _bytes.AddRange(raw);
        if (raw.Length < fixedLen) _bytes.AddRange(new byte[fixedLen - raw.Length]);
        return this;
    }

    public GuiBytes Fill(int count, byte value = 0)
    {
        for (int i = 0; i < count; i++) _bytes.Add(value);
        return this;
    }

    public byte[] ToArray() => _bytes.ToArray();

    public MemoryStream ToStream() => new(_bytes.ToArray());
}

internal static class GuiTest
{
    public const int Ver20100101 = 20100101;   // 编辑器的 PROVERSION（DxComponent/Share.pas:8）
    public const int Ver20160409 = 20160409;
    public const int Ver20160430 = 20160430;
    public const int Ver20160508 = 20160508;
    public const int Ver20160514 = 20160514;
    public const int Ver20160818 = 20160818;
    public const int Ver20170226 = 20170226;
    public const int Ver20171106 = 20171106;
    public const int Ver20180619 = 20180619;
    public const int Ver20190724 = 20190724;
    public const int Ver20190729 = 20190729;
    public const int Ver20211120 = 20211120;

    public static GuiBytes New() => new();

    /// <summary><c>TGuiFileHeader</c>（packed，56 字节）。</summary>
    public static byte[] FileHeader(int guiVersion, int count, ushort groupCount = 0, string desc = "")
        => new TGuiFileHeader
        {
            sDesc = desc,
            ClientVersion = TClientVersion.cv176,
            Reserve = 0,
            GroupCount = groupCount,
            nGuiVersion = guiVersion,
            nCount = count,
            dCreateDate = 0.0,
        }.ToBytes();

    /// <summary><c>TGuiHeader</c>（40 字节）+ 紧随其后的 NameLen 个 GBK 名字字节。</summary>
    public static GuiBytes Header(TGuiType gui, string name = "ctrl", int count = 0,
        int left = 0, int top = 0, int width = 0, int height = 0,
        bool enabled = true, bool visible = true, bool transparent = false,
        bool enableFocus = false, bool floating = false, bool ownerMove = false,
        TMouseEvents mouseEvents = TMouseEvents.Default, bool encrypt = false)
    {
        var nameBytes = GXX.Core.EncodingInit.GBK.GetBytes(name ?? string.Empty);
        var header = new TGuiHeader
        {
            Gui = gui,
            Left = left,
            Top = top,
            Width = width,
            Height = height,
            Enabled = enabled,
            Visible = visible,
            Transparent = transparent,
            EnableFocus = enableFocus,
            Floating = floating,
            OwnerMove = ownerMove,
            MouseEvents = mouseEvents,
            NameLen = nameBytes.Length,
            Background = 0,
            Count = count,
        }.ToBytes();

        if (encrypt) DxGuiCrypt.EncryptGuiHeader(header, header.Length);   // 原文 nGuiVersion >= 20170226 且 header 已加密

        var b = New().Raw(header);
        if (nameBytes.Length > 0) b.Gbk(name);
        return b;
    }

    /// <summary><c>TGuiHeaderAdd</c>（48 字节）+ ShowName + HintText。原文仅在该版本段才存在。</summary>
    public static GuiBytes HeaderAdd(string showName = "", string hint = "",
        TReferenceX referenceX = TReferenceX.rxLeft, bool adjustYByHeight = false, bool topAlignment = false)
    {
        var showBytes = GXX.Core.EncodingInit.GBK.GetBytes(showName ?? string.Empty);
        var hintBytes = GXX.Core.EncodingInit.GBK.GetBytes(hint ?? string.Empty);
        var b = New().Raw(new TGuiHeaderAdd
        {
            ShowNameLen = showBytes.Length,
            ReferenceX = referenceX,
            AdjustYByHeight = adjustYByHeight,
            TopAlignment = topAlignment,
            Reserverd = 0,
            HintTextLen = (ushort)hintBytes.Length,
            Reserverd2 = 0,
        }.ToBytes());
        if (showBytes.Length > 0) b.Gbk(showName);
        if (hintBytes.Length > 0) b.Gbk(hint);
        return b;
    }

    /// <summary>把多个 GuiBytes 顺序拼起来。</summary>
    public static GuiBytes Cat(params GuiBytes[] parts)
    {
        var b = New();
        foreach (var p in parts) if (p != null) b.Raw(p.ToArray());
        return b;
    }

    /// <summary>zlib 压缩（对应编译期把 .GUI 压进 'ZDAT' 资源的步骤）。</summary>
    public static byte[] Zlib(byte[] raw) => ZlibEx.CompressBuf(raw, raw.Length);

    /// <summary>注册一个资源提供者并把 Provider 指过去（测试结束由调用方复位）。</summary>
    public static TDxGuiMemoryResourceProvider WithResource(string name, string type, byte[] data)
    {
        var provider = new TDxGuiMemoryResourceProvider().Add(name, type, data);
        DxGuiResource.Provider = provider;
        return provider;
    }
}
