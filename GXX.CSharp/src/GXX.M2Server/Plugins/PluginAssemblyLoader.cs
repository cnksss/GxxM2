using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using GXX.Core;

namespace GXX.M2Server.Plugins;

// =====================================================================================
// 插件装载层：1:1 复刻 PluginManager.pas 的装载路径，并给出托管侧（AssemblyLoadContext）方案。
//
// 原版证据（PluginManager.pas）：
//   * `LoadPluginList`（:2277）先 `LoadSysInternalPlugin`（:2289 → :1954），再读
//     `g_Config.sPlugDir + 'PlugList.txt'`（:2290），逐行跳过空行与 `;` 注释（:2302），
//     对每个文件 `LoadLibrary`（:2310）→ `GetProcAddress(Moudle, 'Init')`（:2313）→
//     填 `TAppFuncDef`（`{$I PluginFuncLoad_Dll.inc}` :2325、`{$I PluginFuncAssign.inc}` :2327）
//     → `DoInit(@MF, BufferCrc(@MF, SizeOf(MF)), 0, @PlugDesc, PlugDescLen)`（:2352）。
//   * `LoadPlugin`（:2379）同样 `LoadLibrary` + `GetProcAddress('Init')`（:2398-2401）。
//   * 内存装载路径：`MemoryLoadLibrary`（:1853）/ `MemoryGetProcAddress`（:1856 / :2017…），
//     卸载走 `UnInit`（:461-463 / :473-475）→ `FreeLibrary`（:465）或 `MemoryFreeLibrary`（:478）。
//   * 文件头注释（PluginManager.pas:15）明确：Common/MemoryModule.pas 的国产改动版"时灵时不灵，
//     还会崩溃"，已换成 GitHub 原版 `MemoryModuleEx`。
//
// 因此接口形态是 **原生 DLL 导出表（C 风格 stdcall 函数指针）**，不是 COM：
//   `Init` / `UnInit` / `Hook*` 都是 `GetProcAddress` 按名字取的导出函数
//   （导出函数清单见 PluginInterface.pas:3190-3301 的注释块，共 40 个）。
//
// 托管侧方案（转换开发文档 §2.2 / §4.7 第 7 条）：
//   原生语义不可等价（LoadLibrary 的 PE 装载、GetProcAddress 的符号解析、内存模块自装载、
//   以及 `TMethod` 方法指针、内联汇编 hook），故托管侧改为：
//   1) **AssemblyLoadContext** 按目录隔离装载托管插件程序集（可卸载，且可解析插件私有依赖）；
//   2) 插件入口以**托管接口**表达（见 <see cref="IM2ServerPlugin"/>），宿主把
//      <see cref="IPluginHostEnv"/> 交给插件，由插件按需在 TAppFuncDef 上填回调；
//   3) 原生 DLL 插件仍可按原版装载路径工作（LoadLibrary/GetProcAddress），
//      用于兼容期内的二进制插件；但**只支持 32 位宿主**（原版 `TPlugInit` 用 stdcall）。
// =====================================================================================

/// <summary>
/// 原文 `TPlugInit`（PluginManager.pas:26-27）：
/// `function(AppFunc: PAppFuncDef; AppFuncCrc: DWORD; ExtParam: Integer; Desc: PAnsiChar; var DescLen: DWORD): BOOL; stdcall;`
/// 托管侧保持同一 ABI（stdcall，第五参为 ref uint），以便直接 GetProcAddress 后调用。
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate int TPlugInit(IntPtr AppFunc, uint AppFuncCrc, int ExtParam, byte[] Desc, ref uint DescLen);

/// <summary>原文 `TPlugUnInit`（PluginManager.pas:29）：`procedure(); stdcall;`</summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate void TPlugUnInit();

/// <summary>
/// 托管插件契约（对应原文"宿主把 TAppFuncDef 交给插件、插件回填 Hook* 回调"的协议）。
/// 插件的托管实现只需实现本接口，由 <see cref="PluginAssemblyLoader"/> 装载。
/// </summary>
public interface IM2ServerPlugin
{
    /// <summary>原文导出函数 `Init`（PluginInterface.pas:3192-3193）。</summary>
    bool Init(IPluginHostEnv env, ref TAppFuncDef appFunc, uint appFuncCrc, int extParam, byte[] desc, ref uint descLen);

    /// <summary>原文导出函数 `UnInit`（PluginInterface.pas:3195）。</summary>
    void UnInit();
}

/// <summary>一个已装载插件（原文 `TPlugin`，PluginManager.pas:337-...）。</summary>
public sealed class LoadedPlugin
{
    /// <summary>原文 `TPlugin.FFileName`（PluginManager.pas:101 附近的字段集）。</summary>
    public string FileName { get; internal set; } = string.Empty;

    /// <summary>原文 `TPlugin.FIsMemLoad`（内存装载 or LoadLibrary）。</summary>
    public bool IsMemLoad { get; internal set; }

    /// <summary>原文 `TPlugin.FIsInitOK`（PluginManager.pas:101）。</summary>
    public bool IsInitOK { get; internal set; }

    /// <summary>原文 `TPlugin.FPlugDesc`（插件自报描述，来自 `Init` 的 Desc 出参）。</summary>
    public string PlugDesc { get; internal set; } = string.Empty;

    /// <summary>原文 `MF.PluginID := NativeInt(Plugin)`（PluginManager.pas:2323）对应的句柄。</summary>
    public IntPtr PluginId { get; internal set; }

    /// <summary>托管插件实例（原生 DLL 插件为 null）。</summary>
    public IM2ServerPlugin? ManagedPlugin { get; internal set; }

    /// <summary>原生模块句柄（托管插件为 IntPtr.Zero）。</summary>
    public IntPtr Module { get; internal set; }

    /// <summary>归属的 AssemblyLoadContext（托管插件）。</summary>
    public AssemblyLoadContext? LoadContext { get; internal set; }
}

/// <summary>
/// 插件装载器：1:1 复刻 PluginManager.LoadPluginList 的行为（PlugList.txt 解析 +
/// 内部插件优先 + 每条失败不影响其余），托管侧额外提供 AssemblyLoadContext 路径。
/// </summary>
public sealed class PluginAssemblyLoader
{
    private readonly string _plugDir;
    private readonly List<LoadedPlugin> _plugins = new();
    private IntPtr _nextPluginId = 1;

    public PluginAssemblyLoader(string plugDir)
    {
        _plugDir = plugDir ?? throw new ArgumentNullException(nameof(plugDir));
    }

    /// <summary>已装载插件（原文 `g_PluginManager.Items`）。</summary>
    public IReadOnlyList<LoadedPlugin> Plugins => _plugins;

    /// <summary>原文 `PlugList.txt` 里的行（原文 `PlugList: TStringList`，:2306）。</summary>
    public List<string> PlugList { get; } = new();

    /// <summary>
    /// 解析 `PlugList.txt` 的行：原文如此（PluginManager.pas:2301-2303）——
    /// `Trim` 后跳过空行与首字符为 `;` 的行，其余原样作为文件名。
    /// </summary>
    public static List<string> ParsePlugList(IEnumerable<string> lines)
    {
        var result = new List<string>();
        foreach (var raw in lines)
        {
            var s = (raw ?? string.Empty).Trim();
            if (s.Length == 0 || s[0] == ';') continue;
            result.Add(s);
        }
        return result;
    }

    /// <summary>原文 `LoadPluginList`（PluginManager.pas:2277）：读目录下 PlugList.txt；不存在则直接返回。</summary>
    public void LoadPluginList(IPluginHostEnv env)
    {
        // 原文 :2291-2292：目录不存在则 CreateDir
        if (!Directory.Exists(_plugDir)) Directory.CreateDirectory(_plugDir);

        var fileName = Path.Combine(_plugDir, "PlugList.txt");
        if (!File.Exists(fileName)) return;

        PlugList.Clear();
        PlugList.AddRange(ParsePlugList(File.ReadAllLines(fileName, EncodingInit.GBK)));

        foreach (var line in PlugList)
        {
            var full = Path.Combine(_plugDir, line);
            if (!File.Exists(full)) continue;
            try
            {
                LoadOne(env, line, full, isMemLoad: false);
            }
            catch (Exception ex)
            {
                // 原文 :2364-2367：except → MainOutMessage('插件初始化出错[%s]') + 回滚
                env.MainOutMessage($"插件初始化出错[{line}]：{ex.Message}", false);
            }
        }
    }

    /// <summary>
    /// 装载单个插件：先按托管程序集（AssemblyLoadContext）尝试，再回落原生 DLL
    /// （对应原文 `LoadLibrary` + `GetProcAddress('Init')`）。
    /// </summary>
    public LoadedPlugin? LoadOne(IPluginHostEnv env, string plugFileName, string fullPath, bool isMemLoad)
    {
        if (IsManagedAssembly(fullPath))
        {
            return LoadManaged(env, plugFileName, fullPath);
        }
        return LoadNative(env, plugFileName, fullPath, isMemLoad);
    }

    /// <summary>用 PE 头判断是否托管程序集（`BSJB` 签名），避免把原生 DLL 交给 ALC。</summary>
    public static bool IsManagedAssembly(string path)
    {
        try
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);
            if (fs.Length < 0x40) return false;
            fs.Position = 0x3C;
            var peOffset = br.ReadInt32();
            if (peOffset <= 0 || peOffset + 0x18 > fs.Length) return false;
            fs.Position = peOffset + 0x18;
            var magic = br.ReadUInt16();
            if (magic != 0x10B && magic != 0x20B) return false;
            fs.Position = peOffset + 0x18 + (magic == 0x10B ? 0xE0 : 0xF0);
            var cliRva = br.ReadInt32();
            return cliRva != 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 托管装载：每个插件一个独立 <see cref="AssemblyLoadContext"/>（可卸载）。
    /// 对应原文"每个 DLL 一个 FModule 句柄"的隔离粒度。
    /// </summary>
    private LoadedPlugin LoadManaged(IPluginHostEnv env, string plugFileName, string fullPath)
    {
        var alc = new PluginLoadContext(fullPath);
        var asm = alc.LoadFromAssemblyPath(Path.GetFullPath(fullPath));

        // 原文 :2313 / :2401 都只认导出名 'Init'；托管侧取第一个实现 IM2ServerPlugin 的公开无参构造类型。
        Type? pluginType = null;
        foreach (var t in asm.GetTypes())
        {
            if (t.IsAbstract || !typeof(IM2ServerPlugin).IsAssignableFrom(t)) continue;
            if (t.GetConstructor(Type.EmptyTypes) is null) continue;
            pluginType = t;
            break;
        }
        if (pluginType is null)
        {
            alc.Unload();
            env.MainOutMessage($"插件初始化失败[{plugFileName}]：未找到 IM2ServerPlugin 实现", false);
            return null!;
        }

        var instance = (IM2ServerPlugin)Activator.CreateInstance(pluginType)!;
        var loaded = new LoadedPlugin
        {
            FileName = plugFileName,
            IsMemLoad = false,
            ManagedPlugin = instance,
            LoadContext = alc,
            PluginId = (IntPtr)(_nextPluginId++),
        };
        _plugins.Add(loaded);

        var appFunc = new TAppFuncDef { PluginID = loaded.PluginId };
        var desc = new byte[400];
        var descLen = (uint)desc.Length;
        var handle = GCHandle.Alloc(appFunc, GCHandleType.Pinned);
        uint crc;
        try
        {
            // 原文 :2352：BufferCrc(@MF, SizeOf(MF))
            crc = BufferCrc.Compute(handle.AddrOfPinnedObject(), Marshal.SizeOf<TAppFuncDef>());
        }
        finally
        {
            handle.Free();
        }
        var ok = instance.Init(env, ref appFunc, crc, 0, desc, ref descLen);
        loaded.IsInitOK = ok;
        loaded.PlugDesc = PluginHostText.ReadAnsi(desc);
        if (!ok)
        {
            // 原文 :2358-2363：初始化失败 → 提示 + 移除
            env.MainOutMessage($"插件初始化失败[{plugFileName}]", false);
            _plugins.Remove(loaded);
            alc.Unload();
        }
        return loaded;
    }

    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
    private static extern IntPtr LoadLibraryA(string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    /// <summary>
    /// 原生装载（兼容期）：逐字复刻原文 :2310-2371 —— `LoadLibrary` → `GetProcAddress('Init')`
    /// → 填 TAppFuncDef → `DoInit(...)`；失败则 `FreeLibrary`。
    /// 注：`TPlugInit` 是 stdcall，**只能在 32 位宿主进程里安全调用**（64 位下没有 stdcall 概念，
    /// 但 ABI 恰好等价于默认调用约定）。
    /// </summary>
    private LoadedPlugin? LoadNative(IPluginHostEnv env, string plugFileName, string fullPath, bool isMemLoad)
    {
        var module = LoadLibraryA(fullPath);
        // 原文 :2311：`if Moudle > 32 then`（Win32 HINSTANCE 错误码 ≤ 32）
        if (module == IntPtr.Zero || module.ToInt64() <= 32) return null;

        var initPtr = GetProcAddress(module, "Init");
        if (initPtr == IntPtr.Zero)
        {
            // 原文 :2370-2371：没有 Init 导出就 FreeLibrary
            FreeLibrary(module);
            return null;
        }

        var loaded = new LoadedPlugin
        {
            FileName = plugFileName,
            IsMemLoad = isMemLoad,
            Module = module,
            PluginId = (IntPtr)(_nextPluginId++),
        };
        _plugins.Add(loaded);

        var init = Marshal.GetDelegateForFunctionPointer<TPlugInit>(initPtr);
        var appFunc = new TAppFuncDef { PluginID = loaded.PluginId };
        var desc = new byte[400];
        var descLen = (uint)desc.Length;

        var handle = GCHandle.Alloc(appFunc, GCHandleType.Pinned);
        try
        {
            // 原文 :2352：BufferCrc(@MF, SizeOf(MF))
            var crc = BufferCrc.Compute(handle.AddrOfPinnedObject(), Marshal.SizeOf<TAppFuncDef>());
            var ok = init(handle.AddrOfPinnedObject(), crc, 0, desc, ref descLen) != 0;
            loaded.IsInitOK = ok;
            loaded.PlugDesc = PluginHostText.ReadAnsi(desc);
            if (!ok)
            {
                env.MainOutMessage($"插件初始化失败[{plugFileName}]", false);
                _plugins.Remove(loaded);
                FreeLibrary(module);
                return null;
            }
        }
        finally
        {
            handle.Free();
        }
        return loaded;
    }

    /// <summary>
    /// 卸载：原文 `TPlugin.UnLoad`（PluginManager.pas:412-484）——
    /// `GetProcAddress('UnInit')` → 调用 → `FreeLibrary`（:461-465）。
    /// 托管插件对应 `UnInit()` → `AssemblyLoadContext.Unload()`。
    /// </summary>
    public void Unload(LoadedPlugin plugin)
    {
        if (plugin.IsInitOK)
        {
            if (plugin.ManagedPlugin is not null)
            {
                plugin.ManagedPlugin.UnInit();
            }
            else if (plugin.Module != IntPtr.Zero)
            {
                var ptr = GetProcAddress(plugin.Module, "UnInit");
                if (ptr != IntPtr.Zero) Marshal.GetDelegateForFunctionPointer<TPlugUnInit>(ptr)();
            }
        }

        if (plugin.Module != IntPtr.Zero) FreeLibrary(plugin.Module);
        plugin.LoadContext?.Unload();
        _plugins.Remove(plugin);
    }
}

/// <summary>
/// 插件专用 <see cref="AssemblyLoadContext"/>（可卸载：collectible: true）。
/// 对应原文"每个插件一个 FModule 句柄 + FreeLibrary"的隔离与卸载语义。
/// </summary>
public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath)
        : base(name: $"M2Plugin:{Path.GetFileName(pluginPath)}", isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    /// <summary>先按插件目录解析私有依赖，再回落到默认上下文（宿主 GXX.Core 等）。</summary>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path is null ? null : LoadFromAssemblyPath(path);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return path is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
    }
}

/// <summary>
/// 原文 `PluginManager.pas` 里用到的 `BufferCrc(@MF, SizeOf(MF))`
/// （Init 的第二参 `AppFuncCrc`，:2352 / :1937）。
/// 接缝：**算法待核**——原文 CRC 实现在 `Common/CheckCrc.pas`，本车道不移植该单元，
/// 故这里只保证"同一份 TAppFuncDef 得到同一个确定性值"，**不保证与原版 BufferCrc 数值一致**。
/// </summary>
public static class BufferCrc
{
    /// <summary>对 `TAppFuncDef` 的托管布局做确定性 32 位 CRC（接缝，算法待核）。</summary>
    public static uint Compute(IntPtr appFunc, int sizeOf)
    {
        if (appFunc == IntPtr.Zero || sizeOf <= 0) return 0;
        var buf = new byte[sizeOf];
        Marshal.Copy(appFunc, buf, 0, sizeOf);
        return Compute(buf);
    }

    /// <summary>标准 CRC-32（反射多项式 0xEDB88320，初值/终值 0xFFFFFFFF）。</summary>
    public static uint Compute(byte[] data)
    {
        var crc = 0xFFFFFFFFu;
        foreach (var b in data)
        {
            crc ^= b;
            for (var i = 0; i < 8; i++)
            {
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320u : crc >> 1;
            }
        }
        return crc ^ 0xFFFFFFFFu;
    }
}

/// <summary>原文 `TNotifyEventMethod`（PluginInterface.pas:41-46 / PluginImplement.pas:41-46）的托管对应。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TNotifyEventMethod
{
    /// <summary>原文 `Click: TNotifyEventEx`。</summary>
    public TNotifyEventEx? Click;
    /// <summary>原文 `Sender: TObject`。</summary>
    public IntPtr Sender;
}
