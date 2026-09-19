using System;
using System.IO.MemoryMappedFiles;
using GXX.Core.Rtl;

namespace GXX.GameCenter;

/// <summary>
/// CheckPrevious.pas 全文 1:1 移植（93 行）：单实例互斥 + 已有实例前置。
/// <para>
/// 进程管理规程（转换开发文档 §3.4/§4.6）：原文用 <c>CreateFileMapping</c>/<c>MapViewOfFile</c>
/// 在共享内存里维护 <c>TInstanceInfo{PreviousHandle, RunCounter}</c>；托管侧保留同一语义，
/// 以 <see cref="IInstanceInfoStore"/> 抽象"命名共享内存（是否已存在 + TInstanceInfo 读写）"，
/// 默认实现 <see cref="MemoryMappedInstanceInfoStore"/> 用 .NET <see cref="MemoryMappedFile"/> 承载。
/// 单测注入内存实现即可完整覆盖 <c>MaxInstances</c>/<c>RunCounter</c> 全部分支。
/// </para>
/// 映射名规则与原文一致：<c>ParamStr(0)</c>（可执行文件全路径）去掉全部 '\' 字符
/// （<c>StringReplace(ParamStr(0), '\', '', [rfReplaceAll, rfIgnoreCase])</c>，CheckPrevious.pas:37）。
/// </summary>
public static class CheckPrevious
{
    /// <summary>CheckPrevious.pas:12-15 <c>TInstanceInfo</c>（packed record）当前视图。</summary>
    public static TInstanceInfo InstanceInfo { get; private set; }

    /// <summary>CheckPrevious.pas:18 <c>MappingName: string;</c></summary>
    public static string MappingName { get; private set; } = "";

    /// <summary>CheckPrevious.pas:22 <c>RemoveMe: boolean = True;</c></summary>
    public static bool RemoveMe { get; private set; } = true;

    /// <summary>CheckPrevious.pas:17 <c>MappingHandle: THandle;</c>（0 表示未持有映射）。</summary>
    public static IntPtr MappingHandle { get; private set; } = IntPtr.Zero;

    /// <summary>共享内存访存接缝（默认 <see cref="MemoryMappedInstanceInfoStore"/>）。</summary>
    public static IInstanceInfoStore Store = new MemoryMappedInstanceInfoStore();

    /// <summary>窗口句柄操作接缝（IsIconic/ShowWindow/SetForegroundWindow）。</summary>
    public static IForegroundWindowService? WindowService;

    /// <summary>取当前进程可执行文件全路径（对应 <c>ParamStr(0)</c>；测试注入）。</summary>
    public static Func<string> AppPathProvider = () => Environment.ProcessPath ?? "";

    /// <summary>CheckPrevious.pas:24 <c>procedure CloseFileMap;</c>（原文函数体整体被注释掉，故为空实现）。</summary>
    public static void CloseFileMap()
    {
        /*RemoveMe := False;
        if Assigned(InstanceInfo) then UnmapViewOfFile(InstanceInfo);
        if MappingHandle <> 0 then CloseHandle(MappingHandle);
        InstanceInfo := nil;
        MappingHandle := 0;  */
    }

    /// <summary>
    /// CheckPrevious.pas:33 <c>function RestoreIfRunning(const AppHandle: THandle; MaxInstances: integer = 1): boolean;</c>
    /// 返回 <c>true</c> 表示"已有 >= MaxInstances 个实例正在运行，本实例应退出"。
    /// </summary>
    public static bool RestoreIfRunning(uint AppHandle, int MaxInstances = 1)
    {
        bool Result = true;

        MappingName = DelphiStringReplace.RemoveBackslash(AppPathProvider());

        MappingHandle = Store.OpenOrCreate(MappingName, out bool alreadyExists);

        if (MappingHandle == IntPtr.Zero)
            throw new System.ComponentModel.Win32Exception("CreateFileMapping failed");   // RaiseLastOSError

        if (!alreadyExists)                                                             // GetLastError <> ERROR_ALREADY_EXISTS
        {
            TInstanceInfo info = Store.Read(MappingHandle);

            info.PreviousHandle = AppHandle;
            info.RunCounter = 1;
            Store.Write(MappingHandle, info);
            InstanceInfo = info;

            Result = false;
        }
        else //already runing
        {
            // 原文此处重新 OpenFileMapping；托管侧 Store 已持有同一命名对象，等价。
            TInstanceInfo info = Store.Read(MappingHandle);

            if (info.RunCounter >= MaxInstances)
            {
                RemoveMe = false;

                if (WindowService != null && WindowService.IsIconic(info.PreviousHandle))
                    WindowService.ShowWindow(info.PreviousHandle, IForegroundWindowService.SW_RESTORE);
                WindowService?.SetForegroundWindow(info.PreviousHandle);
            }
            else
            {
                info.PreviousHandle = AppHandle;
                info.RunCounter = 1 + info.RunCounter;
                Store.Write(MappingHandle, info);

                Result = false;
            }
            InstanceInfo = info;
        }
        return Result;
    }

    /// <summary>
    /// CheckPrevious.pas:95-113 <c>finalization</c> 段：<c>RemoveMe</c> 为真时递减一次 RunCounter。
    /// （托管侧由宿主在进程退出/Dispose 时显式调用。）
    /// </summary>
    public static void FinalizeUnit()
    {
        //remove one instance
        if (RemoveMe)
        {
            MappingHandle = Store.OpenExisting(MappingName);
            if (MappingHandle != IntPtr.Zero)
            {
                TInstanceInfo info = Store.Read(MappingHandle);

                info.RunCounter = -1 + info.RunCounter;
                Store.Write(MappingHandle, info);
                InstanceInfo = info;
            }
            else throw new System.ComponentModel.Win32Exception("OpenFileMapping failed"); // RaiseLastOSError
        }
        CloseFileMap();
    }

    /// <summary>测试隔离：复位全部静态状态（不触碰默认 Store 的命名对象）。</summary>
    public static void ResetForTests(IInstanceInfoStore? store = null)
    {
        Store = store ?? new MemoryMappedInstanceInfoStore();
        WindowService = null;
        AppPathProvider = () => Environment.ProcessPath ?? "";
        MappingHandle = IntPtr.Zero;
        MappingName = "";
        RemoveMe = true;
        InstanceInfo = default;
    }

    /// <summary>测试隔离：直接置 <c>RemoveMe</c>（原文为 unit 级 var，finalization 依赖其值）。</summary>
    public static void SetRemoveMeForTests(bool value) => RemoveMe = value;

    /// <summary>测试隔离：直接置映射名（finalization 段依赖 <c>MappingName</c>）。</summary>
    public static void SetMappingNameForTests(string value) => MappingName = value ?? "";
}

/// <summary>
/// CheckPrevious.pas 使用的"命名共享内存 + TInstanceInfo 结构"抽象。
/// 默认实现见 <see cref="MemoryMappedInstanceInfoStore"/>；单测注入内存实现。
/// </summary>
public interface IInstanceInfoStore : IDisposable
{
    /// <summary>
    /// 对应 <c>CreateFileMapping($FFFFFFFF, nil, PAGE_READWRITE, 0, SizeOf(TInstanceInfo), PChar(MappingName))</c>。
    /// </summary>
    /// <param name="name">映射名（已去掉 '\' 的可执行文件路径）。</param>
    /// <param name="alreadyExists">等价于 <c>GetLastError = ERROR_ALREADY_EXISTS</c>。</param>
    /// <returns>映射句柄；0 表示失败（原文 <c>MappingHandle = 0</c> → RaiseLastOSError）。</returns>
    IntPtr OpenOrCreate(string name, out bool alreadyExists);

    /// <summary>对应 <c>OpenFileMapping(FILE_MAP_ALL_ACCESS, False, PChar(MappingName))</c>；0 表示失败。</summary>
    IntPtr OpenExisting(string name);

    /// <summary>对应 <c>MapViewOfFile(...)</c> 后的 <c>InstanceInfo^</c> 读出。</summary>
    TInstanceInfo Read(IntPtr handle);

    /// <summary>对应 <c>InstanceInfo^.xxx := ...</c> 写回共享视图。</summary>
    void Write(IntPtr handle, TInstanceInfo info);
}

/// <summary>
/// <see cref="IInstanceInfoStore"/> 默认实现：.NET <see cref="MemoryMappedFile"/>（8 字节 TInstanceInfo）。
/// </summary>
public sealed class MemoryMappedInstanceInfoStore : IInstanceInfoStore
{
    private const int Size = sizeof(uint) + sizeof(int); // TInstanceInfo: THandle(4) + Integer(4)

    private MemoryMappedFile? _file;
    private MemoryMappedViewAccessor? _view;

    public IntPtr OpenOrCreate(string name, out bool alreadyExists)
    {
        // PAGE_READWRITE + SizeOf(TInstanceInfo) 容量，命名映射：已存在则复用。
        _file = MemoryMappedFile.CreateOrOpen(name, Size, MemoryMappedFileAccess.ReadWrite);
        alreadyExists = System.Diagnostics.Process.GetProcessesByName(
            System.IO.Path.GetFileNameWithoutExtension(name)).Length > 1;
        _view = _file.CreateViewAccessor(0, Size, MemoryMappedFileAccess.ReadWrite);
        return _file.SafeMemoryMappedFileHandle.DangerousGetHandle();
    }

    public IntPtr OpenExisting(string name)
    {
        try
        {
            _file = MemoryMappedFile.OpenExisting(name);
            _view = _file.CreateViewAccessor(0, Size, MemoryMappedFileAccess.ReadWrite);
            return _file.SafeMemoryMappedFileHandle.DangerousGetHandle();
        }
        catch (System.IO.FileNotFoundException)
        {
            return IntPtr.Zero;
        }
    }

    public TInstanceInfo Read(IntPtr handle)
    {
        if (_view == null) return default;
        var info = new TInstanceInfo
        {
            PreviousHandle = _view.ReadUInt32(0),
            RunCounter = _view.ReadInt32(sizeof(uint)),
        };
        return info;
    }

    public void Write(IntPtr handle, TInstanceInfo info)
    {
        if (_view == null) return;
        _view.Write(0, info.PreviousHandle);
        _view.Write(sizeof(uint), info.RunCounter);
    }

    public void Dispose()
    {
        _view?.Dispose();
        _file?.Dispose();
        _view = null;
        _file = null;
    }
}

/// <summary>
/// CheckPrevious.pas 依赖的 Win32 窗口函数（IsIconic/ShowWindow/SetForegroundWindow）接缝。
/// </summary>
public interface IForegroundWindowService
{
    /// <summary>Windows.pas <c>SW_RESTORE = 9</c>。</summary>
    public const int SW_RESTORE = 9;

    /// <summary><c>IsIconic(hWnd)</c>。</summary>
    bool IsIconic(uint hWnd);

    /// <summary><c>ShowWindow(hWnd, nCmdShow)</c>。</summary>
    bool ShowWindow(uint hWnd, int nCmdShow);

    /// <summary><c>SetForegroundWindow(hWnd)</c>。</summary>
    bool SetForegroundWindow(uint hWnd);
}
