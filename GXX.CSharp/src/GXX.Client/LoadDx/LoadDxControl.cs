using System;
using System.IO;
using GXX.Client.DxComponent;

namespace GXX.Client.LoadDx;

// =====================================================================================
// Source/Client-HGE/LoadDxControl.pas（实测 1,771 行；任务书写的 1,550 是旧统计）1:1 移植。
//
// 单元职责：把**内存里的** .GUI 字节流还原成 DxComponent 控件树。
// 与 LoadDxControlEx 的关键差别（两单元"看起来一样实则不同"的全部要点）：
//   1. 读字节：本单元用单元级全局 MemoryData/MemorySize/MemoryPosition + 裸指针 Move；
//      Ex 用 TStream。
//   2. 控件登记：本单元用 <c>PControlAddress</c> 裸指针线性推进（LoadDxControl.pas:1594-1596），
//      Ex 用 <c>THashedStringList</c> 按控件名查表。
//   3. LoadSubComponent：本单元 <c>if DxControl &lt;&gt; nil then</c>，**NameLen = 0 时照样
//      读 GuiHeaderAdd 并 LoadComponent**；Ex 是 <c>if (DxControl &lt;&gt; nil) and (NameLen &gt; 0)</c>，
//      NameLen = 0 时直接跳过（连字节都不消费）。
//   4. HintText：本单元只要 GuiVersion >= 20160409 就读；Ex 要求 >= 20180619。
//
// 原文是单元级全局状态（MemoryData/MemoryPosition/PControlAddress 都是 var）。
// 托管侧收进实例字段，避免重入/多线程互相污染；同时保留同名可读字段以便逐行对照。
// =====================================================================================

/// <summary>
/// LoadDxControl.pas 的 <c>LoadControlFromStream</c> / <c>LoadControlFromMemory</c> /
/// <c>LoadSubComponent</c> / <c>NewDxControl</c>。
/// </summary>
public sealed class TDxGuiLoaderControl
{
    /// <summary>原文 LoadDxControl.pas:52-59 的 MemoryData/MemorySize/MemoryPosition 三件套。</summary>
    public readonly TDxMemoryReader Reader = new(Array.Empty<byte>(), 0);

    /// <summary>原文 LoadDxControl.pas:53 <c>PControlAddress:Pointer</c>（控件指针数组的游标）。</summary>
    public TDxControlAddressList PControlAddress;

    /// <summary>
    /// 原文 LoadDxControl.pas:1678-1681。
    /// <c>Result := LoadControlFromMemory(AControlAddress, MemoryStream.Memory, MemoryStream.Size, Background, sUiName);</c>
    /// </summary>
    public int LoadControlFromStream(MemoryStream memoryStream, TDxControl background,
        TDxControlAddressList aControlAddress, string sUiName)
    {
        if (memoryStream == null) throw new ArgumentNullException(nameof(memoryStream));
        // 原文如此（LoadDxControl.pas:1680）：TMemoryStream.Memory 是可直接取用的裸缓冲，永不抛异常。
        // .NET 的 MemoryStream.GetBuffer() 对"由 byte[] 构造且未公开缓冲"的实例会抛
        // UnauthorizedAccessException，故这里退化为 ToArray()（只是多一次拷贝，语义相同）。
        return LoadControlFromMemory(aControlAddress, GetMemoryOf(memoryStream), (int)memoryStream.Length, background, sUiName);
    }

    private static byte[] GetMemoryOf(MemoryStream stream)
    {
        if (stream.TryGetBuffer(out ArraySegment<byte> segment)) return segment.Array;
        return stream.ToArray();
    }

    /// <summary>原文 LoadDxControl.pas:1683-1769（顶层遍历）。</summary>
    public int LoadControlFromMemory(TDxControlAddressList address, byte[] memory, int size,
        TDxControl background, string sUiName)
    {
        int i;

        Reader.Reset(memory, size);                      // 原文 1692-1694

        PControlAddress = address;                       // 原文 1696-1697（原文先置 nil 再赋值）

        // 原文 1698-1699
        Reader.ReadRecord(TGuiFileHeader.SizeOf, TGuiFileHeader.ReadAt, out var guiFileHeader);
        int result = guiFileHeader.nCount;

        // 原文 1701-1712：组名块（仅当 nGuiVersion >= 20160409 且 GroupCount > 0）
        if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20160409 && guiFileHeader.GroupCount > 0)
        {
            for (i = 0; i <= guiFileHeader.GroupCount - 1; i++)
            {
                Reader.ReadInt32(out int len);
                if (len > 0)
                {
                    // 原文如此（LoadDxControl.pas:1705-1706）：组名读出来就丢（只在
                    // {$IFDEF OUTPUT_GUI_READORDER} 下有日志），但因为要推动流位置，必须真读。
                    Reader.ReadFixedString(len);
                }
                // else sText := '';  —— 原文无副作用，此处不写
            }
        }

        // 原文 1714-1768
        while (true)
        {
            if (Reader.MemoryPosition >= Reader.MemorySize) break;      // 原文 1715

            var headerBuffer = new byte[TGuiHeader.SizeOf];
            if (Reader.ReadMemory(headerBuffer, 0, TGuiHeader.SizeOf) == TGuiHeader.SizeOf)
            {
                if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20170226)   // 原文 1718-1720
                    DxGuiCrypt.DecryptGuiHeader(headerBuffer, TGuiHeader.SizeOf);

                var guiHeader = TGuiHeader.ReadAt(headerBuffer, 0);
                var dxControl = NewDxControl(guiHeader, background);

                if (dxControl != null)
                {
                    if (guiHeader.NameLen > 0)                                    // 原文 1725-1732
                        dxControl.Name = Reader.ReadFixedString(guiHeader.NameLen);

                    if (guiFileHeader.nGuiVersion >= GuiComponentLoader.Ver20160409)   // 原文 1734-1751
                    {
                        int readAdd = Reader.ReadRecord(TGuiHeaderAdd.SizeOf, TGuiHeaderAdd.ReadAt, out var guiHeaderAdd);
                        if (readAdd == TGuiHeaderAdd.SizeOf)
                        {
                            dxControl.ReferenceX = guiHeaderAdd.ReferenceX;
                            dxControl.AdjustYByHeight = guiHeaderAdd.AdjustYByHeight;
                            dxControl.TopAlignment = guiHeaderAdd.TopAlignment;
                            if (guiHeaderAdd.ShowNameLen > 0)
                                dxControl.ShowName = Reader.ReadFixedString(guiHeaderAdd.ShowNameLen);
                            // 原文如此（LoadDxControl.pas:1745-1749）：本单元这里**没有** 20180619 版本守卫，
                            // 只要 >= 20160409 就读 HintText —— 与 Ex 的行为不同（见报告"差异断言"）。
                            if (guiHeaderAdd.HintTextLen > 0)
                                dxControl.HintText = Reader.ReadFixedString(guiHeaderAdd.HintTextLen);
                        }
                    }

                    if (guiFileHeader.nGuiVersion <= GuiComponentLoader.Ver20171106)   // 原文 1753-1755
                        dxControl.TopAlignment = false;

                    dxControl.Visible = false;                                      // 原文 1757

                    GuiComponentLoader.LoadComponent(Reader, guiHeader.Gui, dxControl, guiFileHeader.nGuiVersion);

                    for (i = 0; i <= guiHeader.Count - 1; i++)                       // 原文 1761-1763
                        LoadSubComponent(dxControl, guiFileHeader.nGuiVersion, sUiName);
                }
            }
            else
            {
                break;                                                          // 原文 1765-1767
            }
        }

        return result;
    }

    /// <summary>原文 LoadDxControl.pas:1621-1676（递归子控件）。</summary>
    public int LoadSubComponent(TDxControl aOwner, int guiVersion, string sUiName)
    {
        int i;
        int result = 0;

        var headerBuffer = new byte[TGuiHeader.SizeOf];
        if (Reader.ReadMemory(headerBuffer, 0, TGuiHeader.SizeOf) == TGuiHeader.SizeOf)
        {
            if (guiVersion >= GuiComponentLoader.Ver20170226)
                DxGuiCrypt.DecryptGuiHeader(headerBuffer, TGuiHeader.SizeOf);

            var guiHeader = TGuiHeader.ReadAt(headerBuffer, 0);
            var dxControl = NewDxControl(guiHeader, aOwner);

            if (dxControl != null)
            {
                if (guiHeader.NameLen > 0)
                {
                    dxControl.Name = Reader.ReadFixedString(guiHeader.NameLen);
                    // {$IFDEF OUTPUT_GUI_READORDER} LogHelper.WriteToLogFile(Format('[%s] %s',[sUiName, sText]), LOG_LEVEL_3); {$ENDIF}
                    // 原文如此（LoadDxControl.pas:1641-1643）：该日志是编译期开关，默认关闭（`{.$DEFINE}`）。
                }

                if (guiVersion >= GuiComponentLoader.Ver20160409)
                {
                    int readAdd = Reader.ReadRecord(TGuiHeaderAdd.SizeOf, TGuiHeaderAdd.ReadAt, out var guiHeaderAdd);
                    if (readAdd == TGuiHeaderAdd.SizeOf)
                    {
                        dxControl.ReferenceX = guiHeaderAdd.ReferenceX;
                        dxControl.AdjustYByHeight = guiHeaderAdd.AdjustYByHeight;
                        dxControl.TopAlignment = guiHeaderAdd.TopAlignment;
                        if (guiHeaderAdd.ShowNameLen > 0)
                            dxControl.ShowName = Reader.ReadFixedString(guiHeaderAdd.ShowNameLen);
                        if (guiHeaderAdd.HintTextLen > 0)
                            dxControl.HintText = Reader.ReadFixedString(guiHeaderAdd.HintTextLen);
                    }
                }

                if (guiVersion <= GuiComponentLoader.Ver20171106)
                    dxControl.TopAlignment = false;

                GuiComponentLoader.LoadComponent(Reader, guiHeader.Gui, dxControl, guiVersion);

                result = guiHeader.Count;
                for (i = 0; i <= guiHeader.Count - 1; i++)
                    result += LoadSubComponent(dxControl, guiVersion, sUiName);
            }
        }

        return result;
    }

    /// <summary>
    /// 原文 LoadDxControl.pas:1560-1619 的 NewDxControl。
    /// <para>
    /// 与 Ex 版本的差异（LoadDxControl.pas:1593-1596）：建好控件后**立即**写进
    /// <c>PControlAddress^</c> 并把指针前移一个 TDxControl —— 本单元不按名字登记，
    /// 而是按读取顺序线性落进调用方给的数组。
    /// </para>
    /// </summary>
    public TDxControl NewDxControl(TGuiHeader guiHeader, TDxControl aOwner)
    {
        var dxControl = DxControlFactory.NewDxControl(guiHeader, aOwner);
        if (dxControl != null)
        {
            // 原文如此（LoadDxControl.pas:1594-1596）：原文不判越界，写出数组即内存越界；
            // 托管侧 TDxControlAddressList.Store 越界时返回 false 并继续推进游标。
            PControlAddress?.Store(dxControl);
        }
        return dxControl;
    }
}
