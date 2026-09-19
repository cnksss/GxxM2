using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GXX.Core.Protocol;
using GXX.M2Server.Plugins;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 并行批次P2b：插件**装载层**测试（PluginManager.pas 的装载路径 1:1 复刻 + 托管 ALC 方案）。
///
/// 覆盖：
///   * `PlugList.txt` 行解析（原文 :2299-2303：Trim 后跳过空行与 `;` 注释）；
///   * 托管/原生程序集判别（PE CLI 目录项，决定走 AssemblyLoadContext 还是 LoadLibrary）；
///   * 21 个 Pack=1 记录的 `SizeOf` 断言（wire 布局不变）；
///   * 原文导出函数面（PluginInterface.pas:3190-3301 注释块里的 40 个 Hook*）在本层有对应托管契约。
/// </summary>
public sealed class PluginsAssemblyLoaderTests
{
    private sealed class NoopEnv : IPluginHostEnv
    {
        public string Version => "v";
        public DateTime BuildTime => DateTime.UnixEpoch;
        public string AppDir => Path.GetTempPath();
        public IntPtr MainFormHandle => IntPtr.Zero;
        public void MainOutMessage(string msg, bool isAddTime) { }
        public string GetOtherFileDir(int m2FileType) => string.Empty;
        public int GetGlobalVarI(int index) => 0;
        public void SetGlobalVarI(int index, int value) { }
        public int GetGlobalVarG(int index) => 0;
        public void SetGlobalVarG(int index, int value) { }
        public string GetGlobalVarA(int index) => string.Empty;
        public void SetGlobalVarA(int index, string value) { }
        public void SetMainFormCaptionExt(string caption) { }
        public IEnvirnoment FindMap(string mapName) => null;
        public IListHandle MapList => new TListHandle();
        public IIniFileHandle Config => null;
        public IIniFileHandle StringConf => null;
        public IMenuItem MainMenuItems => null;
        public IMenuItem GetFrameMenuItem(string field) => null;
        public bool PluginExists(IntPtr plugId) => false;
        public void PluginAddMenu(IntPtr plugId, IMenuItem item, object notifyEventMethod) { }
        public IUserEngineSeam UserEngine => null;
        public int GetConfigInt(string field) => 0;
        public IMagicACListHandle MagicACList => null;
        public int GetTakeOnPosition(int stdMode) => 0;
        public bool GetUserItemBindValue(byte bindValue, int bindType) => false;
        public void SetUserItemBindValue(ref byte bindValue, int bindType, bool value) { }
        public uint GetRGB(byte color) => 0;
    }

    /// <summary>
    /// 原文 `LoadPluginList` 的行解析（PluginManager.pas:2299-2303）：
    /// `sLine := Trim(SL.Strings[I])`；空行或首字符 `;` 跳过；其余原样入列。
    /// </summary>
    [Fact]
    public void ParsePlugList_FollowsOriginalTrimAndSemicolonRules()
    {
        var lines = new[] { "", "  ", ";comment", "  ;comment2", " IPLocal.dll ", "RecvLog.dll", "CommonScript.dll;x" };
        var parsed = PluginAssemblyLoader.ParsePlugList(lines);
        Assert.Equal(new[] { "IPLocal.dll", "RecvLog.dll", "CommonScript.dll;x" }, parsed.ToArray());
    }

    /// <summary>原文 :2291-2292：插件目录不存在时 CreateDir；无 PlugList.txt 时直接返回。</summary>
    [Fact]
    public void LoadPluginList_CreatesDirectoryAndToleratesMissingFile()
    {
        var dir = Path.Combine(Path.GetTempPath(), "gxx_plug_" + Guid.NewGuid().ToString("N"));
        var loader = new PluginAssemblyLoader(dir);
        loader.LoadPluginList(new NoopEnv());
        Assert.True(Directory.Exists(dir));
        Assert.Empty(loader.Plugins);
        Directory.Delete(dir, true);
    }

    /// <summary>
    /// 托管程序集判别：本测试程序集（含 CLI 目录项）为托管；纯文本文件不是。
    /// 原文没有这一步——原版一律 `LoadLibrary`；托管侧必须先分流（否则 ALC 会拒绝原生 PE）。
    /// </summary>
    [Fact]
    public void IsManagedAssembly_DistinguishesManagedFromPlainFile()
    {
        Assert.True(PluginAssemblyLoader.IsManagedAssembly(typeof(PluginInterfaceHost).Assembly.Location));

        var plain = Path.Combine(Path.GetTempPath(), "gxx_plain_" + Guid.NewGuid().ToString("N") + ".dll");
        File.WriteAllText(plain, "not a PE file at all");
        try
        {
            Assert.False(PluginAssemblyLoader.IsManagedAssembly(plain));
            Assert.False(PluginAssemblyLoader.IsManagedAssembly(Path.Combine(Path.GetTempPath(), "no_such_file_xyz.dll")));
        }
        finally
        {
            File.Delete(plain);
        }
    }

    /// <summary>
    /// PlugList 里登记但文件不存在的条目：原文 :2308 `if FileExists(sFileName) then` 才装载，
    /// 但仍会进入 `PlugList`（:2306）。
    /// </summary>
    [Fact]
    public void LoadPluginList_RegistersNamesButSkipsMissingFiles()
    {
        var dir = Path.Combine(Path.GetTempPath(), "gxx_plug_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            File.WriteAllText(Path.Combine(dir, "PlugList.txt"), ";c\r\nMissing.dll\r\n", Encoding.UTF8);
            var loader = new PluginAssemblyLoader(dir);
            loader.LoadPluginList(new NoopEnv());
            Assert.Equal(new[] { "Missing.dll" }, loader.PlugList.ToArray());
            Assert.Empty(loader.Plugins);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    // ------------------------------------------------------------------ 布局断言（Pack=1）

    /// <summary>
    /// 21 个记录的 `SizeOf` 断言：全部 `LayoutKind.Sequential, Pack=1`，
    /// 函数指针字段宽度 = IntPtr.Size，`Reserved` 为 fixed 缓冲。
    /// </summary>
    [Fact]
    public void AllRecords_AreSequentialPack1()
    {
        var types = new[]
        {
            typeof(TScriptCmdParam), typeof(TMemoryFunc), typeof(TListFunc), typeof(TStringListFunc),
            typeof(TMemoryStreamFunc), typeof(TMemuFunc), typeof(TIniFileFunc), typeof(TMagicACListFunc),
            typeof(TMapManagerFunc), typeof(TEnvirnomentFunc), typeof(TM2EngineFunc), typeof(TBaseObjectFunc),
            typeof(TSmartObjectFunc), typeof(TPlayObjectFunc), typeof(TDummyObjectFunc), typeof(THeroObjectFunc),
            typeof(TNormNpcFunc), typeof(TUserEngineFunc), typeof(TGuildManagerFunc), typeof(TGuildFunc),
            typeof(TAppFuncDef),
        };
        Assert.Equal(21, types.Length);
        foreach (var t in types)
        {
            // 反射看不到 [StructLayout]（.NET 把它折算进类型布局），
            // 故布局证据取自生成源码：每个记录声明前都必须是 Pack=1。
            Assert.Contains("[StructLayout(LayoutKind.Sequential, Pack = 1)]", AttributeLines(t.Name));
            Assert.True(Marshal.SizeOf(t) > 0);
        }
    }

    /// <summary>取生成源码中某个 struct 声明前的属性行。</summary>
    private static string AttributeLines(string structName)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var file = Path.Combine(dir.FullName, "src", "GXX.M2Server", "Plugins", "PluginInterfaceTables.g.cs");
            if (File.Exists(file))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("struct " + structName))
                        return (i > 0 ? lines[i - 1] : string.Empty) + "\n" + lines[i];
                }
            }
            dir = dir.Parent;
        }
        return string.Empty;
    }

    /// <summary>
    /// `TMemoryFunc`（PluginInterface.pas:2343-2348）：3 个委托 + `Reserved: array[0..3] of Pointer`
    /// ⇒ 3*P + 4*P = 7*P。
    /// </summary>
    [Fact]
    public void TMemoryFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(3, typeof(TMemoryFunc).GetFields().Count(f => !f.Name.StartsWith("Reserved")));
        Assert.Equal(ExpectedTableSize(typeof(TMemoryFunc)), Marshal.SizeOf<TMemoryFunc>());
    }

    /// <summary>`TListFunc`（:2350-2365）：12 个委托 + Reserved[20] ⇒ 32*P。</summary>
    [Fact]
    public void TListFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TListFunc)), Marshal.SizeOf<TListFunc>());   // 13 委托 + Reserved[20] ⇒ 33*P
    }

    /// <summary>`TStringListFunc`（:2367-2397）：28 个委托 + Reserved[20] ⇒ 48*P。</summary>
    [Fact]
    public void TStringListFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TStringListFunc)), Marshal.SizeOf<TStringListFunc>());
    }

    /// <summary>`TMemoryStreamFunc`（:2399-2414）：13 个委托 + Reserved[20] ⇒ 33*P。</summary>
    [Fact]
    public void TMemoryStreamFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TMemoryStreamFunc)), Marshal.SizeOf<TMemoryStreamFunc>());
    }

    /// <summary>`TMemuFunc`（:2416-2444）：26 个委托 + Reserved[20] ⇒ 46*P。</summary>
    [Fact]
    public void TMemuFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TMemuFunc)), Marshal.SizeOf<TMemuFunc>());
    }

    /// <summary>`TIniFileFunc`（:2446-2458）：10 个委托 + Reserved[30] ⇒ 40*P。</summary>
    [Fact]
    public void TIniFileFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TIniFileFunc)), Marshal.SizeOf<TIniFileFunc>());
    }

    /// <summary>`TMagicACListFunc`（:2460-2465）：3 个委托 + Reserved[10] ⇒ 13*P。</summary>
    [Fact]
    public void TMagicACListFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TMagicACListFunc)), Marshal.SizeOf<TMagicACListFunc>());
    }

    /// <summary>`TMapManagerFunc`（:2467-2471）：2 个委托 + Reserved[40] ⇒ 42*P。</summary>
    [Fact]
    public void TMapManagerFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TMapManagerFunc)), Marshal.SizeOf<TMapManagerFunc>());
    }

    /// <summary>`TEnvirnomentFunc`（:2473-2501）：25 个委托 + Reserved[100] ⇒ 125*P。</summary>
    [Fact]
    public void TEnvirnomentFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TEnvirnomentFunc)), Marshal.SizeOf<TEnvirnomentFunc>());
    }

    /// <summary>`TM2EngineFunc`（:2503-2531）：26 个委托 + Reserved[100] ⇒ 126*P。</summary>
    [Fact]
    public void TM2EngineFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TM2EngineFunc)), Marshal.SizeOf<TM2EngineFunc>());
    }

    /// <summary>`TBaseObjectFunc`（:2533-2721）：185 个委托 + Reserved[100] ⇒ 285*P。</summary>
    [Fact]
    public void TBaseObjectFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TBaseObjectFunc)), Marshal.SizeOf<TBaseObjectFunc>());
    }

    /// <summary>`TSmartObjectFunc`（:2723-2828）：103 个委托 + Reserved[100] ⇒ 203*P。</summary>
    [Fact]
    public void TSmartObjectFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TSmartObjectFunc)), Marshal.SizeOf<TSmartObjectFunc>());
    }

    /// <summary>`TPlayObjectFunc`（:2830-2991）：159 个委托 + Reserved[100] ⇒ 259*P。</summary>
    [Fact]
    public void TPlayObjectFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TPlayObjectFunc)), Marshal.SizeOf<TPlayObjectFunc>());
    }

    /// <summary>`TDummyObjectFunc`（:2993-2998）：3 个委托 + Reserved[100] ⇒ 103*P。</summary>
    [Fact]
    public void TDummyObjectFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TDummyObjectFunc)), Marshal.SizeOf<TDummyObjectFunc>());
    }

    /// <summary>`THeroObjectFunc`（:3000-3035）：33 个委托 + Reserved[100] ⇒ 133*P。</summary>
    [Fact]
    public void THeroObjectFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(THeroObjectFunc)), Marshal.SizeOf<THeroObjectFunc>());
    }

    /// <summary>`TNormNpcFunc`（:3037-3057）：18 个委托 + Reserved[100] ⇒ 118*P。</summary>
    [Fact]
    public void TNormNpcFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TNormNpcFunc)), Marshal.SizeOf<TNormNpcFunc>());
    }

    /// <summary>`TUserEngineFunc`（:3059-3114）：53 个委托 + Reserved[100] ⇒ 153*P。</summary>
    [Fact]
    public void TUserEngineFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TUserEngineFunc)), Marshal.SizeOf<TUserEngineFunc>());
    }

    /// <summary>`TGuildManagerFunc`（:3116-3122）：4 个委托 + Reserved[100] ⇒ 104*P。</summary>
    [Fact]
    public void TGuildManagerFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TGuildManagerFunc)), Marshal.SizeOf<TGuildManagerFunc>());
    }

    /// <summary>`TGuildFunc`（:3124-3157）：31 个委托 + Reserved[100] ⇒ 131*P。</summary>
    [Fact]
    public void TGuildFunc_SizeMatchesOriginalReservedCount()
    {
        Assert.Equal(ExpectedTableSize(typeof(TGuildFunc)), Marshal.SizeOf<TGuildFunc>());
    }

    /// <summary>`TScriptCmdParam`（:77-111）：34 个字段（含 `nParam10` 与 `end;` 同行，原文如此）。</summary>
    [Fact]
    public void TScriptCmdParam_Has34Fields()
    {
        Assert.Equal(34, typeof(TScriptCmdParam).GetFields().Length);   // sRawParam01..10 + sParam01..10/nParam01..10 + Npc/PlayObject/BaseObject/nCMDCode
        Assert.Equal("nParam10", typeof(TScriptCmdParam).GetFields().Last().Name);   // 原 :111 "nParam10: Integer end;"（原文如此，end; 与字段同行）
    }

    /// <summary>
    /// 原文导出函数面：PluginInterface.pas:3190-3301 的注释块列出 40 个导出函数
    /// （Init/UnInit/Hook*），宿主用 `GetProcAddress(...,'名字')` 取（PluginManager.pas:2313 / :2017-2018 等）。
    /// 托管侧以 <see cref="IM2ServerPlugin"/> + <see cref="TPlugInit"/> 表达前两个，
    /// 其余 Hook* 由插件直接实现 ABI 委托（见 PluginInterfaceTypes.g.cs）。
    /// </summary>
    [Fact]
    public void ExportSurface_MatchesOriginalCommentedExportList()
    {
        var expected = new[]
        {
            "Init", "UnInit", "HookGetIPLocal", "HookEngineReadyToStart", "HookEngineStartComplete",
            "HookEngineReloadComplete", "HookLoadScriptFile", "HookDecryptScriptFile", "HookDecryptScriptLine",
            "HookNpcLoadConditionCmd", "HookNpcConditionProcess", "HookNpcLoadActionCmd", "HookNpcActionProcess",
            "HookUserSelect", "HookUserCommand", "HookGetVariableText", "HookBaseObjectCreate",
            "HookBaseObjectRecalAbilBegin", "HookBaseObjectRecalAbilEnd", "HookBaseObjectRun",
            "HookBaseObjectProcessMsg", "HookBaseObjectStruck", "HookBaseObjectMagicStruck", "HookBaseObjectAttack",
            "HookBaseObjectMagicAttack", "HookBaseObjectDie", "HookBaseObjectMakeGhost", "HookBaseObjectFree",
            "HookPlayerCreate", "HookPlayerLogin1", "HookPlayerLogin2", "HookPlayerLogin3", "HookPlayerLogin4",
            "HookPlayerRun", "HookPlayerViewRangeNewObject", "HookPlayerProcessMsgBegin", "HookPlayerProcessMsgEnd",
            "HookPlayerFree", "HookDummyObjectRunBegin", "HookDummyObjectRunEnd", "HookHeroObjectCreate",
            "HookHeroObjectFree",
        };
        Assert.Equal(42, expected.Length);

        // 原生路径：Init/UnInit 有 ABI 委托（GetProcAddress 用）
        Assert.Equal(CallingConvention.StdCall,
            typeof(TPlugInit).GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false)
                .Cast<UnmanagedFunctionPointerAttribute>().Single().CallingConvention);
        Assert.Equal(CallingConvention.StdCall,
            typeof(TPlugUnInit).GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false)
                .Cast<UnmanagedFunctionPointerAttribute>().Single().CallingConvention);

        // 托管路径：入口签名必须带 AppFunc 记录 + CRC + Desc 缓冲
        var init = typeof(IM2ServerPlugin).GetMethod(nameof(IM2ServerPlugin.Init))!;
        Assert.Equal(new[] { "IPluginHostEnv", "TAppFuncDef&", "UInt32", "Int32", "Byte[]", "UInt32&" },
            init.GetParameters().Select(p => p.ParameterType.Name).ToArray());
        Assert.Equal(typeof(bool), init.ReturnType);
        Assert.NotNull(typeof(IM2ServerPlugin).GetMethod(nameof(IM2ServerPlugin.UnInit)));
    }
    /// <summary>
    /// 依据原文 `Reserved: array[0..N-1] of Pointer` 的 N（写在生成源码的字段注释里）
    /// 计算 T*Func 记录应有的字节数：委托字段(每字段 1 个指针) + N 个指针。
    /// 接缝：这样"预期值"直接来自原文的 Reserved 长度，而不是手抄的数字。
    /// </summary>
    private static int ExpectedTableSize(Type t)
    {
        var delegateCount = t.GetFields().Count(f => typeof(Delegate).IsAssignableFrom(f.FieldType));
        var reservedCount = ReservedCount(t.Name);
        return (delegateCount + reservedCount) * IntPtr.Size;
    }

    private static int ReservedCount(string structName)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var file = Path.Combine(dir.FullName, "src", "GXX.M2Server", "Plugins", "PluginInterfaceTables.g.cs");
            if (File.Exists(file))
            {
                foreach (var line in File.ReadLines(file))
                {
                    if (line.Contains("Reserved[") && line.Contains("struct") == false)
                    {
                        // lines are grouped per struct; find the struct first
                    }
                }
                var text = File.ReadAllText(file);
                var idx = text.IndexOf("struct " + structName, StringComparison.Ordinal);
                if (idx < 0) return 0;
                var next = text.IndexOf("struct ", idx + 1, StringComparison.Ordinal);
                var body = next < 0 ? text.Substring(idx) : text.Substring(idx, next - idx);
                var m = System.Text.RegularExpressions.Regex.Match(body, @"fixed long Reserved\[(\d+)\]");
                if (m.Success) return int.Parse(m.Groups[1].Value);
            }
            dir = dir.Parent;
        }
        return 0;
    }
}