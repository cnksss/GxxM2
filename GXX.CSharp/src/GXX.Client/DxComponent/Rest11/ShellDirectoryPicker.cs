using System;
using System.Runtime.InteropServices;
using System.Text;

// =====================================================================================
// LoginDlg.pas:37-95 的两个嵌套过程 1:1 移植：
//   * `function SelectDirCB(Wnd: Hwnd; uMsg: UINT; LPARAM, lpData: LPARAM): Integer stdcall;`
//   * `function SelectDirectory(const Caption: string; const Root: WideString;
//      var Directory: string; Owner: THandle): Boolean;`
//
// 【为什么复用而不是新造第三份】
//   本工程既有代码里**没有任何** `SHBrowseForFolder` / `BFFM_SETSELECTION` / `DisableTaskWindows`
//   的封装（全仓 grep `BrowseForFolder|BFFM_|DisableTaskWindows` 只命中本条与一处无关注释），
//   故这不是"造第三份"，而是唯一一份。待 shell32 目录选择在 GXX.Core/GXX.Client 归位后，
//   本文件整体移交（登记 D-P11-03）。
//
// 【原文用的是 ShlObj 的 TBrowseInfo / PItemIDList / IMalloc】而不是 ShellApi 的
//   BROWSEINFOW + CoTaskMemFree（VCL 自己的 SelectDirectory 走的是 ShlObj 路径）。
//   两处 IMalloc 释放（`ShellMalloc.Free(ItemIDList)` :88 与 `ShellMalloc.Free(Buffer)` :92）
//   在托管侧按 ShlObj 语义映射到 `SHFree`（= shell 分配器的释放入口）。
// =====================================================================================

namespace GXX.Client.DxComponent.Rest11;

/// <summary>
/// LoginDlg.pas:37-95 的 <c>SelectDirCB</c> + <c>SelectDirectory</c> 1:1。
/// </summary>
public static class ShellDirectoryPicker
{
    // ---- ShlObj.pas / ShellApi.pas 常量（值即为 Win32 SDK 原值）----
    private const uint BFFM_INITIALIZED = 1;
    private const uint BFFM_SETSELECTION = 0x0400 + 102;   // WM_USER + 102（原文用的是宽字符版 BFFM_SETSELECTIONW）
    private const uint BIF_RETURNONLYFSDIRS = 0x0001;
    private const int MAX_PATH = 260;

    /// <summary>
    /// LoginDlg.pas:37-42 <c>SelectDirCB</c>：
    /// <c>if (uMsg = BFFM_INITIALIZED) and (lpData &lt;&gt; 0) then SendMessage(Wnd, BFFM_SETSELECTION, Integer(True), lpData);</c>
    /// —— 第 3 参 <c>Integer(True)</c> = 1（宽字符模式），第 4 参是调用方传进来的 <c>PChar(Directory)</c> 缓冲区指针。
    /// </summary>
    private static int SelectDirCB(IntPtr wnd, uint uMsg, IntPtr lParam, IntPtr lpData)
    {
        if (uMsg == BFFM_INITIALIZED && lpData != IntPtr.Zero)
            SendMessage(wnd, BFFM_SETSELECTION, new IntPtr(1), lpData);
        return 0;
    }

    /// <summary>
    /// LoginDlg.pas:44-95 <c>SelectDirectory</c>。逐句对应（行号见注释）。
    /// </summary>
    /// <param name="caption">原文 <c>Caption</c>（<c>lpszTitle := PChar(Caption)</c> :72）。</param>
    /// <param name="root">原文 <c>Root: WideString</c>（<c>Root &lt;&gt; ''</c> 时经 <c>ParseDisplayName</c> 解析 :63-67）。</param>
    /// <param name="directory">原文 <c>var Directory: string</c>（原地回写）。</param>
    /// <param name="owner">原文 <c>Owner: THandle</c>（<c>hwndOwner := Owner</c> :69）。</param>
    public static DirectoryPickResult SelectDirectory(string caption, string root, string directory, IntPtr owner)
    {
        bool result = false;                                        // :55 Result := False;

        if (!LoginDlgHost.DirectoryExists(directory)) directory = string.Empty;   // :56-57

        IntPtr rootItemIdList = IntPtr.Zero;                        // :62
        IntPtr buffer = Marshal.AllocHGlobal(MAX_PATH * 2);         // :60 ShellMalloc.Alloc(MAX_PATH)
        IntPtr browseInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf<BROWSEINFOW>());
        try
        {
            if (!string.IsNullOrEmpty(root))                        // :63 if Root <> '' then
            {
                // :64-66 SHGetDesktopFolder + IDesktopFolder.ParseDisplayName(Application.Handle, nil,
                //       POleStr(Root), Eaten, RootItemIDList, Flags)
                _ = SHGetDesktopFolder(out IShellFolder desktopFolder);
                if (desktopFolder != null)
                {
                    _ = desktopFolder.ParseDisplayName(LoginDlgHost.OwnerWindow, IntPtr.Zero, root,
                        out uint eaten, out rootItemIdList, out uint flags);
                    Marshal.ReleaseComObject(desktopFolder);
                }
            }

            // :68-78 `with BrowseInfo do begin ... end`
            var browseInfo = new BROWSEINFOW
            {
                hwndOwner = owner,                                  // :69
                pidlRoot = rootItemIdList,                          // :70
                pszDisplayName = buffer,                            // :71
                lpszTitle = caption,                                // :72（Unicode 版取宽串，对应原文 PChar 在 Unicode 编译下的 PChar=WideChar）
                ulFlags = BIF_RETURNONLYFSDIRS,                     // :73
                lpfn = IntPtr.Zero,
                lParam = IntPtr.Zero,
                iImage = 0,
            };
            if (!string.IsNullOrEmpty(directory))                   // :74 if Directory <> '' then
            {
                browseInfo.lpfn = Marshal.GetFunctionPointerForDelegate<BrowseCallbackProc>(SelectDirCB);   // :75
                // :76 LPARAM := Integer(PChar(Directory));
                //   原文把**选中目录串**写进 pszDisplayName 所用的同一个 Buffer，回调里再把该指针交给 BFFM_SETSELECTION。
                WriteUnicode(buffer, directory);
                browseInfo.lParam = buffer;
            }
            Marshal.StructureToPtr(browseInfo, browseInfoPtr, false);

            // :79-84 DisableTaskWindows(0) / try ShBrowseForFolder finally EnableTaskWindows(WindowList)
            IntPtr[] windowList = TaskWindows.DisableTaskWindows(0);
            IntPtr itemIdList;
            try
            {
                itemIdList = SHBrowseForFolder(browseInfoPtr);      // :81
            }
            finally
            {
                TaskWindows.EnableTaskWindows(windowList);          // :83
            }

            result = itemIdList != IntPtr.Zero;                     // :85 Result := ItemIDList <> nil;
            if (result)                                             // :86 if Result then
            {
                SHGetPathFromIDList(itemIdList, buffer);            // :87
                SHFree(itemIdList);                                 // :88 ShellMalloc.Free(ItemIDList)
                directory = Marshal.PtrToStringUni(buffer) ?? string.Empty;   // :89 Directory := Buffer;
            }
        }
        finally
        {
            // :91-93 finally ShellMalloc.Free(Buffer)
            Marshal.FreeHGlobal(browseInfoPtr);
            Marshal.FreeHGlobal(buffer);
        }

        return new DirectoryPickResult(result, directory);
    }

    private static void WriteUnicode(IntPtr buffer, string s)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(s + "\0");
        int max = MAX_PATH * 2;
        if (bytes.Length > max) Array.Resize(ref bytes, max);
        Marshal.Copy(bytes, 0, buffer, bytes.Length);
        if (bytes.Length < max) Marshal.WriteInt16(buffer, bytes.Length, 0);
    }

    // ---- Win32 绑定 ----

    /// <summary>ShlObj.TBrowseInfo 的宽字符版（<c>BROWSEINFOW</c>）。字段名与顺序即 SDK 原样。</summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct BROWSEINFOW
    {
        public IntPtr hwndOwner;
        public IntPtr pidlRoot;
        public IntPtr pszDisplayName;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszTitle;
        public uint ulFlags;
        public IntPtr lpfn;
        public IntPtr lParam;
        public int iImage;
    }

    private delegate int BrowseCallbackProc(IntPtr hwnd, uint uMsg, IntPtr lParam, IntPtr lpData);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr SHBrowseForFolder(IntPtr lpbi);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    [DllImport("shell32.dll", ExactSpelling = true)]
    private static extern int SHGetDesktopFolder([MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf);

    /// <summary><c>ShellMalloc.Free(p)</c>（ShlObj 路径）：shell 分配器的释放入口。</summary>
    [DllImport("shell32.dll", ExactSpelling = true)]
    private static extern void SHFree(IntPtr p);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// <c>ShlObj.IShellFolder.ParseDisplayName</c>（原文 :65 用到）。只声明本处需要的成员，
    /// 其余槽位按 vtable 顺序补 <c>IntPtr</c>（<c>IUnknown</c> 3 槽 + <c>ParseDisplayName</c> 在
    /// <c>IShellFolder</c> 的第 3 位）。
    /// </summary>
    [ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellFolder
    {
        // IUnknown
        void _VtblGap1();
        void _VtblGap2();
        // IShellFolder
        int ParseDisplayName(IntPtr hwnd, IntPtr pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
            out uint pchEaten, out IntPtr ppidl, out uint pdwAttributes);
        void _VtblGap3();
        void _VtblGap4();
        void _VtblGap5();
        void _VtblGap6();
        void _VtblGap7();
        void _VtblGap8();
        void _VtblGap9();
        void _VtblGap10();
        void _VtblGap11();
    }
}

/// <summary>
/// VCL <c>Forms.DisableTaskWindows</c> / <c>EnableTaskWindows</c>（原文 :79-83）：
/// 把**本线程**的顶层窗口全部禁用并返回原状态表（选择目录时禁止切换任务）。
///
/// <para>原文返回的是 <c>Pointer</c>（内部 TList），托管侧用 <see cref="IntPtr"/> 数组表达同一份状态表。</para>
/// </summary>
internal static class TaskWindows
{
    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadWndProc lpfn, IntPtr lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bEnable);

    private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>原文 <c>DisableTaskWindows(0)</c>：禁用本线程全部顶层窗口，返回"原本就禁用"的窗口句柄表。</summary>
    public static IntPtr[] DisableTaskWindows(IntPtr except)
    {
        var alreadyDisabled = new System.Collections.Generic.List<IntPtr>();
        uint tid = GetCurrentThreadId();
        EnumThreadWindows(tid, (hWnd, _) =>
        {
            if (hWnd == except) return true;
            if (!IsWindowEnabled(hWnd)) alreadyDisabled.Add(hWnd);
            EnableWindow(hWnd, false);
            return true;
        }, IntPtr.Zero);
        return alreadyDisabled.ToArray();
    }

    /// <summary>原文 <c>EnableTaskWindows(WindowList)</c>：仅把状态表里的窗口重新启用（原文如此：只恢复"被它禁用过"的）。</summary>
    public static void EnableTaskWindows(IntPtr[] windowList)
    {
        foreach (IntPtr h in windowList)
        {
            if (h != IntPtr.Zero) EnableWindow(h, true);
        }
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern uint GetCurrentThreadId();

    [DllImport("user32.dll", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowEnabled(IntPtr hWnd);
}
