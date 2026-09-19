using System;
using System.IO;
using GXX.Client.DxComponent;

namespace GXX.Client.LoadDx;

// =====================================================================================
// Source/Client-HGE/LoadDxControlEx.pas（实测 2,469 行；任务书写的 2,125 是旧统计）1:1 移植。
//
// 本单元才是**运行时真正被调用**的那一支（各窗口单元都 uses LoadDxControlEx 并调
// LoadCompressedUIData + LoadControlFromStream）。相对 LoadDxControl.pas 的四处行为差异：
//   1. 读字节走 TStream（IGuiReader/TDxStreamReader），不再有全局 MemoryPosition。
//   2. 控件按**名字**登记：ControlAddrList:THashedStringList；未登记（pControlAddr = nil）
//      的控件读完立刻 Free（原文注释："等读完所有信息后，释放GUI控件，确保能正确的读取文件数据"）。
//   3. LoadSubComponent / LoadControlFromStream 的入口守卫是
//      <c>(DxControl &lt;&gt; nil) and (GuiHeader.NameLen &gt; 0)</c> —— NameLen = 0 时
//      **连 GuiHeaderAdd 和 LoadComponent 都不执行**，一个字节都不消费。
//   4. HintText 只在 <c>nGuiVersion &gt;= 20180619</c> 时读（原文注释 HZQ 20230619/20230620）。
//
// 另有 PatchLoadControlFromStream / PatchLoadSubComponent 两个"补丁版"：
// 与正式版的差别是**先递归、后按名字登记**，且登记失败（名字不在表里）时 Free；
// 正式版是**先登记、后递归**。两者对同一个输入会产生不同的读取顺序语义（见报告）。
//
// 死代码：LoadDxControlEx.pas:1795-2306 的 ApplyControlData 整个被 (* *) 注释掉 —
// 不参与编译，故不移植，仅在报告"未覆盖行号"里登记。
// =====================================================================================

/// <summary>
/// LoadDxControlEx.pas 的 <c>LoadControlFromStream</c> / <c>PatchLoadControlFromStream</c> /
/// <c>LoadSubComponent</c> / <c>PatchLoadSubComponent</c>。
/// </summary>
public sealed class TDxGuiLoaderControlEx
{
    private readonly TDxStreamReader _reader;

    /// <param name="streamUI">对应原文的 streamUI:TStream（通常是 LoadCompressedUIData 的返回值）。</param>
    public TDxGuiLoaderControlEx(Stream streamUI)
    {
        if (streamUI == null) throw new ArgumentNullException(nameof(streamUI));
        _reader = new TDxStreamReader(streamUI);
    }

    /// <summary>供测试/调用方复用同一个流位置（原文 streamUI 就是外部传入的对象）。</summary>
    public IGuiReader Reader => _reader;

    /// <summary>
    /// 原文 LoadDxControlEx.pas:1688-1793。
    /// <para>返回值 = <c>GuiFileHeader.nCount</c>（读取前即定，递归结果**不累加** —— 与 LoadDxControl.pas 不同）。</para>
    /// </summary>
    public int LoadControlFromStream(TDxControl background, THashedStringList controlAddrList, string sUiName)
    {
        int i;

        _reader.ReadRecord(TGuiFileHeader.SizeOf, TGuiFileHeader.ReadAt, out var guiFileHeader);   // 原文 1698
        int result = guiFileHeader.nCount;                                                        // 原文 1699

        // 原文 1701-1712
        if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20160409 && guiFileHeader.GroupCount > 0)
        {
            for (i = 0; i <= guiFileHeader.GroupCount - 1; i++)
            {
                _reader.ReadInt32(out int len);
                if (len > 0) _reader.ReadFixedString(len);
                // else sText := '';  —— 原文无副作用
            }
        }

        // 原文 1714-1792
        while (true)
        {
            if (_reader.Position >= _reader.Size) break;                    // 原文 1715

            var headerBuffer = new byte[TGuiHeader.SizeOf];
            if (_reader.ReadMemory(headerBuffer, 0, TGuiHeader.SizeOf) == TGuiHeader.SizeOf)
            {
                if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20170226)   // 原文 1726-1728
                    DxGuiCrypt.DecryptGuiHeader(headerBuffer, TGuiHeader.SizeOf);

                var guiHeader = TGuiHeader.ReadAt(headerBuffer, 0);
                var dxControl = DxControlFactory.NewDxControl(guiHeader, background);   // 原文 1730

                // 原文如此（LoadDxControlEx.pas:1732）：**此守卫带 NameLen > 0**。
                if (dxControl != null && guiHeader.NameLen > 0)
                {
                    dxControl.Name = _reader.ReadFixedString(guiHeader.NameLen);        // 原文 1733-1735

                    // 原文 1740-1749：按名字查表，命中则把指针槽指向本控件
                    int nAddrIndex = controlAddrList?.IndexOf(dxControl.Name) ?? -1;
                    TDxControlRef pControlAddr = nAddrIndex >= 0
                        ? controlAddrList.Objects[nAddrIndex] as TDxControlRef
                        : null;
                    if (pControlAddr != null) pControlAddr.Value = dxControl;

                    if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20160409)    // 原文 1752-1771
                    {
                        int readAdd = _reader.ReadRecord(TGuiHeaderAdd.SizeOf, TGuiHeaderAdd.ReadAt, out var guiHeaderAdd);
                        if (readAdd == TGuiHeaderAdd.SizeOf)
                        {
                            dxControl.ReferenceX = guiHeaderAdd.ReferenceX;
                            dxControl.AdjustYByHeight = guiHeaderAdd.AdjustYByHeight;
                            dxControl.TopAlignment = guiHeaderAdd.TopAlignment;
                            if (guiHeaderAdd.ShowNameLen > 0)
                                dxControl.ShowName = _reader.ReadFixedString(guiHeaderAdd.ShowNameLen);

                            // 原文如此（LoadDxControlEx.pas:1763）：只有 20180619 之后的版本才有 HintText。
                            if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20180619)
                            {
                                if (guiHeaderAdd.HintTextLen > 0)
                                    dxControl.HintText = _reader.ReadFixedString(guiHeaderAdd.HintTextLen);
                            }
                        }
                    }

                    if (guiFileHeader.nGuiVersion <= GuiComponentLoader.Ver20171106)    // 原文 1773-1775
                        dxControl.TopAlignment = false;

                    dxControl.Visible = false;                                          // 原文 1777

                    GuiComponentLoader.LoadComponent(_reader, guiHeader.Gui, dxControl, guiFileHeader.nGuiVersion);

                    for (i = 0; i <= guiHeader.Count - 1; i++)                           // 原文 1781-1783
                        LoadSubComponent(dxControl, controlAddrList, guiFileHeader.nGuiVersion, sUiName);

                    if (pControlAddr == null)                                           // 原文 1785-1787
                        dxControl.Dispose();   // 原文 DxControl.Free
                }
            }
            else
            {
                break;                                                                  // 原文 1789-1791
            }
        }

        return result;
    }

    /// <summary>
    /// 原文 LoadDxControlEx.pas:1612-1686。
    /// <para>返回值 = <c>GuiHeader.Count</c> 加上所有子控件返回值的和（Guard 与顶层一致）。</para>
    /// </summary>
    public int LoadSubComponent(TDxControl aOwner, THashedStringList controlAddrList, int guiVersion, string sUiName)
    {
        int i;
        int result = 0;

        var headerBuffer = new byte[TGuiHeader.SizeOf];
        if (_reader.ReadMemory(headerBuffer, 0, TGuiHeader.SizeOf) == TGuiHeader.SizeOf)   // 原文 1622
        {
            if (guiVersion >= GuiComponentLoader.Ver20170226)                               // 原文 1623-1625
                DxGuiCrypt.DecryptGuiHeader(headerBuffer, TGuiHeader.SizeOf);

            var guiHeader = TGuiHeader.ReadAt(headerBuffer, 0);
            var dxControl = DxControlFactory.NewDxControl(guiHeader, aOwner);               // 原文 1627

            // 原文如此（LoadDxControlEx.pas:1629）：与 LoadDxControl.pas:1636 的唯一结构性差异。
            if (dxControl != null && guiHeader.NameLen > 0)
            {
                dxControl.Name = _reader.ReadFixedString(guiHeader.NameLen);                // 原文 1630-1632

                int nAddrIndex = controlAddrList?.IndexOf(dxControl.Name) ?? -1;            // 原文 1637-1643
                TDxControlRef pControlAddr = nAddrIndex >= 0
                    ? controlAddrList.Objects[nAddrIndex] as TDxControlRef
                    : null;
                if (pControlAddr != null) pControlAddr.Value = dxControl;                   // 原文 1644-1646

                if (guiVersion >= GuiComponentLoader.Ver20160409)                           // 原文 1648-1667
                {
                    int readAdd = _reader.ReadRecord(TGuiHeaderAdd.SizeOf, TGuiHeaderAdd.ReadAt, out var guiHeaderAdd);
                    if (readAdd == TGuiHeaderAdd.SizeOf)
                    {
                        dxControl.ReferenceX = guiHeaderAdd.ReferenceX;
                        dxControl.AdjustYByHeight = guiHeaderAdd.AdjustYByHeight;
                        dxControl.TopAlignment = guiHeaderAdd.TopAlignment;
                        if (guiHeaderAdd.ShowNameLen > 0)
                            dxControl.ShowName = _reader.ReadFixedString(guiHeaderAdd.ShowNameLen);

                        // 原文如此（LoadDxControlEx.pas:1659）：HZQ 20230619 增加版本兼容检查。
                        if (guiVersion >= GuiComponentLoader.Ver20180619)
                        {
                            if (guiHeaderAdd.HintTextLen > 0)
                                dxControl.HintText = _reader.ReadFixedString(guiHeaderAdd.HintTextLen);
                        }
                    }
                }

                if (guiVersion <= GuiComponentLoader.Ver20171106)                           // 原文 1669-1671
                    dxControl.TopAlignment = false;

                GuiComponentLoader.LoadComponent(_reader, guiHeader.Gui, dxControl, guiVersion);   // 原文 1673

                result = guiHeader.Count;                                                   // 原文 1675
                for (i = 0; i <= guiHeader.Count - 1; i++)                                  // 原文 1677-1679
                    result += LoadSubComponent(dxControl, controlAddrList, guiVersion, sUiName);

                if (pControlAddr == null)                                                   // 原文 1681-1683
                    dxControl.Dispose();   // 原文 DxControl.Free
            }
        }

        return result;
    }

    /// <summary>
    /// 原文 LoadDxControlEx.pas:2376-2467 PatchLoadControlFromStream（"补丁版"）。
    /// <para>
    /// 与正式版的差别：① 无返回值；② 每个顶层控件**先递归子控件、最后才按名字登记**；
    /// ③ 名字不在表里 → Free；④ 名字已在表里但槽位非空 → **不覆盖**（正式版直接覆盖）。
    /// </para>
    /// </summary>
    public void PatchLoadControlFromStream(TDxControl background, THashedStringList controlAddrList, string sUiName)
    {
        int i;

        _reader.ReadRecord(TGuiFileHeader.SizeOf, TGuiFileHeader.ReadAt, out var guiFileHeader);   // 原文 2386

        if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20160409 && guiFileHeader.GroupCount > 0)
        {
            for (i = 0; i <= guiFileHeader.GroupCount - 1; i++)                                     // 原文 2389-2397
            {
                _reader.ReadInt32(out int len);
                if (len > 0) _reader.ReadFixedString(len);
            }
        }

        while (true)                                                                                // 原文 2400-2464
        {
            if (_reader.Position >= _reader.Size) break;

            var headerBuffer = new byte[TGuiHeader.SizeOf];
            if (_reader.ReadMemory(headerBuffer, 0, TGuiHeader.SizeOf) == TGuiHeader.SizeOf)
            {
                if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20170226)                    // 原文 2404-2406
                    DxGuiCrypt.DecryptGuiHeader(headerBuffer, TGuiHeader.SizeOf);

                var guiHeader = TGuiHeader.ReadAt(headerBuffer, 0);
                var dxControl = DxControlFactory.NewDxControl(guiHeader, background);                // 原文 2408

                if (dxControl != null && guiHeader.NameLen > 0)                                     // 原文 2410-2459
                {
                    dxControl.Name = _reader.ReadFixedString(guiHeader.NameLen);

                    if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20160409)                // 原文 2418-2437
                    {
                        int readAdd = _reader.ReadRecord(TGuiHeaderAdd.SizeOf, TGuiHeaderAdd.ReadAt, out var guiHeaderAdd);
                        if (readAdd == TGuiHeaderAdd.SizeOf)
                        {
                            dxControl.ReferenceX = guiHeaderAdd.ReferenceX;
                            dxControl.AdjustYByHeight = guiHeaderAdd.AdjustYByHeight;
                            dxControl.TopAlignment = guiHeaderAdd.TopAlignment;
                            if (guiHeaderAdd.ShowNameLen > 0)
                                dxControl.ShowName = _reader.ReadFixedString(guiHeaderAdd.ShowNameLen);

                            if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20180619)        // 原文 2429
                            {
                                if (guiHeaderAdd.HintTextLen > 0)
                                    dxControl.HintText = _reader.ReadFixedString(guiHeaderAdd.HintTextLen);
                            }
                        }
                    }

                    if (guiFileHeader.nGuiVersion <= GuiComponentLoader.Ver20171106)                // 原文 2439-2441
                        dxControl.TopAlignment = false;

                    dxControl.Visible = false;                                                      // 原文 2443

                    GuiComponentLoader.LoadComponent(_reader, guiHeader.Gui, dxControl, guiFileHeader.nGuiVersion);

                    for (i = 0; i <= guiHeader.Count - 1; i++)                                       // 原文 2447-2449
                        PatchLoadSubComponent(dxControl, controlAddrList, guiFileHeader.nGuiVersion);

                    // 原文 2451-2459：登记在**递归之后**，且槽位非空时不覆盖。
                    int nAddrIndex = controlAddrList?.IndexOf(dxControl.Name) ?? -1;
                    TDxControlRef pControlAddr = nAddrIndex >= 0
                        ? controlAddrList.Objects[nAddrIndex] as TDxControlRef
                        : null;
                    if (pControlAddr != null)
                    {
                        if (pControlAddr.Value == null) pControlAddr.Value = dxControl;
                    }
                    else
                    {
                        dxControl.Dispose();   // 原文 DxControl.Free
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 原文 LoadDxControlEx.pas:2308-2374 PatchLoadSubComponent。
    /// <para>
    /// 原文如此（LoadDxControlEx.pas:2329）：方法签名里**没有** sUiName 参数，
    /// 但 {$IFDEF OUTPUT_GUI_READORDER} 块里却引用了 sUiName —— 该分支默认关闭所以能编译；
    /// 一旦打开该开关本单元编译不过（原文缺陷，已登记）。
    /// </para>
    /// </summary>
    public void PatchLoadSubComponent(TDxControl aOwner, THashedStringList controlAddrList, int guiVersion)
    {
        int i;

        var headerBuffer = new byte[TGuiHeader.SizeOf];
        if (_reader.ReadMemory(headerBuffer, 0, TGuiHeader.SizeOf) == TGuiHeader.SizeOf)   // 原文 2317
        {
            if (guiVersion >= GuiComponentLoader.Ver20170226)                               // 原文 2318-2320
                DxGuiCrypt.DecryptGuiHeader(headerBuffer, TGuiHeader.SizeOf);

            var guiHeader = TGuiHeader.ReadAt(headerBuffer, 0);
            var dxControl = DxControlFactory.NewDxControl(guiHeader, aOwner);               // 原文 2322

            if (dxControl != null && guiHeader.NameLen > 0)                                 // 原文 2324
            {
                dxControl.Name = _reader.ReadFixedString(guiHeader.NameLen);                // 原文 2325-2327

                if (guiVersion >= GuiComponentLoader.Ver20160409)                           // 原文 2332-2351
                {
                    int readAdd = _reader.ReadRecord(TGuiHeaderAdd.SizeOf, TGuiHeaderAdd.ReadAt, out var guiHeaderAdd);
                    if (readAdd == TGuiHeaderAdd.SizeOf)
                    {
                        dxControl.ReferenceX = guiHeaderAdd.ReferenceX;
                        dxControl.AdjustYByHeight = guiHeaderAdd.AdjustYByHeight;
                        dxControl.TopAlignment = guiHeaderAdd.TopAlignment;
                        if (guiHeaderAdd.ShowNameLen > 0)
                            dxControl.ShowName = _reader.ReadFixedString(guiHeaderAdd.ShowNameLen);

                        if (guiVersion >= GuiComponentLoader.Ver20180619)                   // 原文 2343
                        {
                            if (guiHeaderAdd.HintTextLen > 0)
                                dxControl.HintText = _reader.ReadFixedString(guiHeaderAdd.HintTextLen);
                        }
                    }
                }

                if (guiVersion <= GuiComponentLoader.Ver20171106)                           // 原文 2353-2355
                    dxControl.TopAlignment = false;

                GuiComponentLoader.LoadComponent(_reader, guiHeader.Gui, dxControl, guiVersion);   // 原文 2357

                for (i = 0; i <= guiHeader.Count - 1; i++)                                  // 原文 2359-2361
                    PatchLoadSubComponent(dxControl, controlAddrList, guiVersion);

                // 原文 2363-2371：递归之后再登记；名字不在表里则 Free。
                int nAddrIndex = controlAddrList?.IndexOf(dxControl.Name) ?? -1;
                if (nAddrIndex >= 0)
                {
                    var pControlAddr = controlAddrList.Objects[nAddrIndex] as TDxControlRef;
                    if (pControlAddr != null && pControlAddr.Value == null)
                        pControlAddr.Value = dxControl;
                }
                else
                {
                    dxControl.Dispose();   // 原文 DxControl.Free
                }
            }
        }
    }
}
