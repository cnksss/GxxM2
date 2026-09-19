using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Core.Protocol;
using GXX.M2Server.Plugins;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 并行批次P2b：PluginImplement.pas 宿主回调实现的行为测试。
///
/// 覆盖重点（均按原文逐条推导，原文行号写在断言注释里）：
///   * `Dest: PAnsiChar; var DestLen: DWORD` 出参缓冲的**边界语义**（原文 :1026 等 5 处同构代码）；
///   * `_TIniFile_ReadString` 比其它 Get* 多一个 `Length(S) &gt; 0` 条件（原文 :1533）；
///   * `_TM2Engine_MainOutMessage` 忽略 IsAddTime 恒传 True（原文 :1904）；
///   * `_TEnvir_GetMapParam` / `_TEnvir_GetMapParamValue` 是**空实现**（原文 :1750-1760，不补全）；
///   * `_TMenu_Add/Insert` 的 `PlugID error` 抛出与插件反查（原文 :1318 / :1377）；
///   * `_TStrList_*` / `_TMemStream_*` / `_TIniFile_*` 的完整读写往返；
///   * `BufferCrc` 的确定性。
/// </summary>
public sealed class PluginsHostRuntimeTests
{
    // ------------------------------------------------------------------ 测试宿主环境

    private sealed class FakeEnv : IPluginHostEnv
    {
        public string Version { get; set; } = "GXX-Test-1.0";
        public DateTime BuildTime { get; set; } = new DateTime(2021, 1, 6);
        public string AppDir { get; set; } = @"C:\M2\";
        public IntPtr MainFormHandle { get; set; } = (IntPtr)0x1234;
        public List<string> Messages { get; } = new();
        public Dictionary<int, string> OtherDirs { get; } = new();
        public Dictionary<int, int> VarI { get; } = new();
        public Dictionary<int, int> VarG { get; } = new();
        public Dictionary<int, string> VarA { get; } = new();
        public string MainFormCaptionExt { get; private set; } = string.Empty;
        public IEnvirnoment Envir { get; set; }
        public IListHandle Maps { get; } = new TListHandle();
        public IIniFileHandle Config { get; set; }
        public IIniFileHandle StringConf { get; set; }
        public IMenuItem MainMenu { get; set; }
        public Dictionary<string, IMenuItem> FrameMenus { get; } = new();
        public HashSet<IntPtr> KnownPlugins { get; } = new();
        public List<(IntPtr PlugId, IMenuItem Item, object Method)> AddedMenus { get; } = new();

        public void MainOutMessage(string msg, bool isAddTime) => Messages.Add((isAddTime ? "T:" : "F:") + msg);
        public string GetOtherFileDir(int m2FileType) => OtherDirs.TryGetValue(m2FileType, out var v) ? v : string.Empty;
        public int GetGlobalVarI(int index) => VarI.TryGetValue(index, out var v) ? v : 0;
        public void SetGlobalVarI(int index, int value) => VarI[index] = value;
        public int GetGlobalVarG(int index) => VarG.TryGetValue(index, out var v) ? v : 0;
        public void SetGlobalVarG(int index, int value) => VarG[index] = value;
        public string GetGlobalVarA(int index) => VarA.TryGetValue(index, out var v) ? v : string.Empty;
        public void SetGlobalVarA(int index, string value) => VarA[index] = value;
        public void SetMainFormCaptionExt(string caption) => MainFormCaptionExt = caption;
        public IEnvirnoment FindMap(string mapName) => Envir;
        public IListHandle MapList => Maps;
        public IMenuItem MainMenuItems => MainMenu;
        public IMenuItem GetFrameMenuItem(string field) => FrameMenus.TryGetValue(field, out var v) ? v : null;
        public bool PluginExists(IntPtr plugId) => KnownPlugins.Contains(plugId);
        public void PluginAddMenu(IntPtr plugId, IMenuItem item, object notifyEventMethod) => AddedMenus.Add((plugId, item, notifyEventMethod));
        public IUserEngineSeam UserEngine => null;
        public int GetConfigInt(string field) => 0;
        public IMagicACListHandle MagicACList => null;
        public int GetTakeOnPosition(int stdMode) => stdMode * 2;
        public bool GetUserItemBindValue(byte bindValue, int bindType) => (bindValue & (1 << bindType)) != 0;
        public void SetUserItemBindValue(ref byte bindValue, int bindType, bool value)
        {
            if (value) bindValue |= (byte)(1 << bindType);
            else bindValue &= (byte)~(1 << bindType);
        }
        public uint GetRGB(byte color) => (uint)(0xFF000000 | (color << 16) | (color << 8) | color);
    }

    private static FakeEnv NewEnv() => new FakeEnv();

    // ------------------------------------------------------------------ 文本出参缓冲

    /// <summary>
    /// 原文 `_TStrList_GetText`（:1020-1033）：判定为 `DestLen &gt; Length(S)`（必须留结尾 0），
    /// 写入成功才 Result=True，但 DestLen 恒被改写为实际字节数。
    /// </summary>
    [Theory]
    [InlineData(0u, 0, 5)]      // Dest=nil：不写，Result=False
    [InlineData(5u, 0, 5)]      // DestLen == Length(S)：不满足 '>'，不写
    [InlineData(6u, 1, 5)]      // 留出 1 字节给结尾 0：写成功
    public void TStrList_GetText_BufferBoundaryFollowsOriginal(uint destLen, int expectedResult, int expectedLen)
    {
        var host = new PluginInterfaceHost(NewEnv());
        var list = host.TStrList_Create();
        host.TStrList_Add(list, Gbk("ABCDE"));       // 5 字节

        var dest = destLen == 0 ? null : new byte[destLen];
        var len = destLen;
        var result = host.TStrList_GetText(list, dest, ref len);

        Assert.Equal(expectedResult, result);
        Assert.Equal((uint)expectedLen, len);
        if (result == 1) Assert.Equal(0, dest[5]);   // 结尾 0
    }

    /// <summary>原文 `_TStrList_GetText`：DestLen 恒被改写（即便没写成功）。</summary>
    [Fact]
    public void TStrList_GetText_AlwaysRewritesDestLen()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var list = host.TStrList_Create();
        host.TStrList_Add(list, Gbk("HELLO"));
        var dest = new byte[2];
        var len = 2u;
        var r = host.TStrList_GetText(list, dest, ref len);
        Assert.Equal(0, r);
        Assert.Equal(5u, len);
    }

    /// <summary>
    /// 原文 `_TIniFile_ReadString`（:1526-1540）比其它 Get* 多一个 `Length(S) &gt; 0`：
    /// 读到的值为空串时**永不写、Result 恒 False**，但 DestLen 仍被改写为 0。
    /// </summary>
    [Fact]
    public void TIniFile_ReadString_EmptyValueNeverWrites()
    {
        var path = Path.Combine(Path.GetTempPath(), "gxx_plugin_ini_" + Guid.NewGuid().ToString("N") + ".ini");
        try
        {
            File.WriteAllText(path, "[Sec]\r\nEmpty=\r\nFilled=ABC\r\n", GXX.Core.EncodingInit.GBK);
            var host = new PluginInterfaceHost(NewEnv());
            var ini = host.TIniFile_Create(Gbk(path));

            var dest = new byte[16];
            var len = 16u;
            Assert.Equal(0, host.TIniFile_ReadString(ini, Gbk("Sec"), Gbk("Empty"), Gbk(""), dest, ref len));
            Assert.Equal(0u, len);

            len = 16u;
            Assert.Equal(1, host.TIniFile_ReadString(ini, Gbk("Sec"), Gbk("Filled"), Gbk(""), dest, ref len));
            Assert.Equal(3u, len);
            Assert.Equal("ABC", GbkStr(dest, 3));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    /// <summary>原文 `_TIniFile_*` 完整往返：SectionExists/ValueExists/Read/Write String|Integer|Bool。</summary>
    [Fact]
    public void TIniFile_ReadWriteRoundTrip()
    {
        var path = Path.Combine(Path.GetTempPath(), "gxx_plugin_ini_" + Guid.NewGuid().ToString("N") + ".ini");
        try
        {
            var host = new PluginInterfaceHost(NewEnv());
            var ini = host.TIniFile_Create(Gbk(path));
            Assert.Equal(0, host.TIniFile_SectionExists(ini, Gbk("A")));

            host.TIniFile_WriteString(ini, Gbk("A"), Gbk("S"), Gbk("中文值"));
            host.TIniFile_WriteInteger(ini, Gbk("A"), Gbk("N"), -12345);
            host.TIniFile_WriteBool(ini, Gbk("A"), Gbk("B"), 1);

            Assert.Equal(1, host.TIniFile_SectionExists(ini, Gbk("A")));
            Assert.Equal(1, host.TIniFile_ValueExists(ini, Gbk("A"), Gbk("S")));
            Assert.Equal(0, host.TIniFile_ValueExists(ini, Gbk("A"), Gbk("Missing")));
            Assert.Equal(-12345, host.TIniFile_ReadInteger(ini, Gbk("A"), Gbk("N"), 0));
            Assert.Equal(7, host.TIniFile_ReadInteger(ini, Gbk("A"), Gbk("Missing"), 7));
            Assert.Equal(1, host.TIniFile_ReadBool(ini, Gbk("A"), Gbk("B"), 0));

            // 重新打开（落盘后读回）
            var ini2 = host.TIniFile_Create(Gbk(path));
            var dest = new byte[64];
            var len = 64u;
            Assert.Equal(1, host.TIniFile_ReadString(ini2, Gbk("A"), Gbk("S"), Gbk(""), dest, ref len));
            Assert.Equal("中文值", GbkStr(dest, (int)len));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // ------------------------------------------------------------------ M2Engine 薄封装

    /// <summary>
    /// 原文 `_TM2Engine_MainOutMessage`（:1902-1905）：`MainOutMessage(Msg, True {IsAddTime})`
    /// —— 传入的 IsAddTime 被忽略，恒传 True。
    /// </summary>
    [Fact]
    public void TM2Engine_MainOutMessage_IgnoresIsAddTime()
    {
        var env = NewEnv();
        var host = new PluginInterfaceHost(env);
        host.TM2Engine_MainOutMessage(Gbk("hello"), 0);
        Assert.Equal(new[] { "T:hello" }, env.Messages);
    }

    /// <summary>原文 `_TM2Engine_GetVersionInt`（:1807）：`Y*10000 + M*100 + D`。</summary>
    [Fact]
    public void TM2Engine_GetVersionInt_EncodesBuildDate()
    {
        var env = NewEnv();
        env.BuildTime = new DateTime(2021, 1, 6);
        var host = new PluginInterfaceHost(env);
        Assert.Equal(20210106, host.TM2Engine_GetVersionInt());
    }

    /// <summary>原文 `_TM2Engine_GetOtherFileDir`（:1852-1900）：16 类目录；14/15 也走 switch。</summary>
    [Theory]
    [InlineData(0, "envir")]
    [InlineData(1, "plug")]
    [InlineData(15, "conlog")]
    [InlineData(99, "")]        // switch 无分支 → S 保持空串（原文如此）
    public void TM2Engine_GetOtherFileDir_SwitchSemantics(int type, string expected)
    {
        var env = NewEnv();
        env.OtherDirs[0] = "envir";
        env.OtherDirs[1] = "plug";
        env.OtherDirs[15] = "conlog";
        var host = new PluginInterfaceHost(env);
        var dest = new byte[64];
        var len = 64u;
        Assert.Equal(1, host.TM2Engine_GetOtherFileDir(type, dest, ref len));
        Assert.Equal(expected, GbkStr(dest, (int)len));
    }

    /// <summary>原文 `_TM2Engine_GetGlobalIniFile`（:1841）：0→Config、1→StringConf、其余 nil。</summary>
    [Fact]
    public void TM2Engine_GetGlobalIniFile_MapsZeroAndOne()
    {
        var env = NewEnv();
        env.Config = new TIniFileHandle(Path.Combine(Path.GetTempPath(), "gxx_none_1.ini"));
        env.StringConf = new TIniFileHandle(Path.Combine(Path.GetTempPath(), "gxx_none_2.ini"));
        var host = new PluginInterfaceHost(env);
        Assert.Same(env.Config, host.TM2Engine_GetGlobalIniFile(0));
        Assert.Same(env.StringConf, host.TM2Engine_GetGlobalIniFile(1));
        Assert.Null(host.TM2Engine_GetGlobalIniFile(2));
    }

    /// <summary>原文 `_TM2Engine_SetGlobalVarI/G/A`（:1914 / :1927 / :1950）：赋值后恒返回 True。</summary>
    [Fact]
    public void TM2Engine_GlobalVars_SetReturnsTrueAndStores()
    {
        var env = NewEnv();
        var host = new PluginInterfaceHost(env);
        Assert.Equal(1, host.TM2Engine_SetGlobalVarI(3, 42));
        Assert.Equal(1, host.TM2Engine_SetGlobalVarG(4, 43));
        Assert.Equal(1, host.TM2Engine_SetGlobalVarA(5, Gbk("A5")));
        Assert.Equal(42, host.TM2Engine_GetGlobalVarI(3));
        Assert.Equal(43, host.TM2Engine_GetGlobalVarG(4));

        var dest = new byte[16];
        var len = 16u;
        Assert.Equal(1, host.TM2Engine_GetGlobalVarA(5, dest, ref len));
        Assert.Equal("A5", GbkStr(dest, (int)len));
    }

    /// <summary>原文 `_TM2Engine_SetMainFormCaption`（:1820）：写 `sCaptionExtText`。</summary>
    [Fact]
    public void TM2Engine_SetMainFormCaption_WritesExtText()
    {
        var env = NewEnv();
        var host = new PluginInterfaceHost(env);
        host.TM2Engine_SetMainFormCaption(Gbk("EXT"));
        Assert.Equal("EXT", env.MainFormCaptionExt);
    }

    /// <summary>原文 `_TM2Engine_CheckBindType` / `_SetBindValue`（:2122 / :2127）：位操作往返。</summary>
    [Fact]
    public void TM2Engine_BindValue_RoundTrip()
    {
        var env = NewEnv();
        var host = new PluginInterfaceHost(env);
        byte v = 0;
        host.TM2Engine_SetBindValue(ref v, 3, 1);
        Assert.Equal(1, host.TM2Engine_CheckBindType(v, 3));
        Assert.Equal(0, host.TM2Engine_CheckBindType(v, 2));
        host.TM2Engine_SetBindValue(ref v, 3, 0);
        Assert.Equal(0, host.TM2Engine_CheckBindType(v, 3));
    }

    /// <summary>原文 `_TM2Engine_GetTakeOnPosition` / `_GetRGB`（:2117 / :2132）：直接转调宿主。</summary>
    [Fact]
    public void TM2Engine_ThinDelegates()
    {
        var host = new PluginInterfaceHost(NewEnv());
        Assert.Equal(10, host.TM2Engine_GetTakeOnPosition(5));
        Assert.Equal(0xFF808080u, host.TM2Engine_GetRGB(0x80));
    }

    // ------------------------------------------------------------------ 空实现照抄

    /// <summary>
    /// 原文 `_TEnvir_GetMapParam`（:1750-1753）与 `_TEnvir_GetMapParamValue`（:1757-1760）
    /// 的函数体**只有 `Result := False;`**——照抄为恒返回 0，不"补全"。
    /// </summary>
    [Fact]
    public void TEnvir_GetMapParam_IsAnOriginalEmptyStub()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var envir = new FakeEnvir();
        Assert.Equal(0, host.TEnvir_GetMapParam(envir, Gbk("FIGHT4")));
        var dest = new byte[8];
        var len = 8u;
        Assert.Equal(0, host.TEnvir_GetMapParamValue(envir, Gbk("INCGAMEGOLD"), dest, ref len));
    }

    // ------------------------------------------------------------------ 菜单

    /// <summary>原文 `_TMenu_Add`（:1296-1352）：PlugID 反查失败时 `raise Exception.Create('PlugID error')`。</summary>
    [Fact]
    public void TMenu_Add_UnknownPlugIdThrows()
    {
        var env = NewEnv();
        env.MainMenu = new FakeMenuItem();
        var host = new PluginInterfaceHost(env);
        var ex = Assert.Throws<InvalidOperationException>(() => host.TMenu_Add((IntPtr)777, null, Gbk("x"), 1, null));
        Assert.Equal("PlugID error", ex.Message);
    }

    /// <summary>原文 `_TMenu_Add`：合法 PlugID 时建项、设 Caption/Tag、并登记到插件。</summary>
    [Fact]
    public void TMenu_Add_KnownPlugIdRegistersMenu()
    {
        var env = NewEnv();
        env.MainMenu = new FakeMenuItem();
        env.KnownPlugins.Add((IntPtr)9);
        var host = new PluginInterfaceHost(env);

        TNotifyEventEx click = sender => env.Messages.Add("clicked:" + ((FakeMenuItem)sender).Caption);
        var item = host.TMenu_Add((IntPtr)9, null, Gbk("插件菜单"), 66, click);

        Assert.NotNull(item);
        Assert.Equal("插件菜单", item!.Caption);
        Assert.Equal(66, item.Tag);
        Assert.Single(env.AddedMenus);
        Assert.Equal((IntPtr)9, env.AddedMenus[0].PlugId);

        // 原文 NotifyEventEx（:1288）通过 TMethod 绑定；托管侧用 PluginMenuNotify 等价触发
        var notify = Assert.IsType<PluginMenuNotify>(env.AddedMenus[0].Method);
        PluginInterfaceHost.NotifyEventEx(notify);
        Assert.Contains("clicked:插件菜单", env.Messages);
    }

    /// <summary>原文 `_TMenu_GetCaption`（:1410-1423）：与 GetText 同构的缓冲判定。</summary>
    [Fact]
    public void TMenu_GetCaption_BufferSemantics()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var item = new FakeMenuItem { Caption = "ABC" };
        var dest = new byte[4];
        var len = 4u;
        Assert.Equal(1, host.TMenu_GetCaption(item, dest, ref len));
        Assert.Equal(3u, len);
        Assert.Equal("ABC", GbkStr(dest, 3));

        len = 3u;
        Assert.Equal(0, host.TMenu_GetCaption(item, dest, ref len));   // 必须留结尾 0
        Assert.Equal(3u, len);
    }

    /// <summary>原文 `_TMenu_*` 取值/设置器一一对应到 VCL 属性（注意 `GetVisable`→`Visible`）。</summary>
    [Fact]
    public void TMenu_Accessors()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var item = new FakeMenuItem();
        host.TMenu_SetCaption(item, Gbk("C")); Assert.Equal("C", item.Caption);
        host.TMenu_SetEnabled(item, 1); Assert.Equal(1, host.TMenu_GetEnabled(item));
        host.TMenu_SetVisable(item, 0); Assert.Equal(0, host.TMenu_GetVisable(item));
        host.TMenu_SetChecked(item, 1); Assert.Equal(1, host.TMenu_GetChecked(item));
        host.TMenu_SetRadioItem(item, 1); Assert.Equal(1, host.TMenu_GetRadioItem(item));
        host.TMenu_SetGroupIndex(item, 5); Assert.Equal(5, host.TMenu_GetGroupIndex(item));
        host.TMenu_SetTag(item, 9); Assert.Equal(9, host.TMenu_GetTag(item));
        Assert.Equal(0, host.TMenu_Count(item));
    }

    // ------------------------------------------------------------------ TList / TMemoryStream

    /// <summary>原文 `_TList_*`（:903-966）：与 TList 语义逐条对应（Exchange 交换、CopyTo 覆盖目标）。</summary>
    [Fact]
    public void TList_Operations()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var list = host.TList_Create();
        Assert.Equal(0, host.TList_Count(list));
        host.TList_Add(list, (IntPtr)1);
        host.TList_Add(list, (IntPtr)2);
        host.TList_Insert(list, 1, (IntPtr)9);
        Assert.Equal(3, host.TList_Count(list));
        Assert.Equal((IntPtr)9, host.TList_GetItem(list, 1));
        Assert.Equal(2, host.TList_IndexOf(list, (IntPtr)2));
        host.TList_Exchange(list, 0, 2);
        Assert.Equal((IntPtr)2, host.TList_GetItem(list, 0));
        host.TList_SetItem(list, 0, (IntPtr)7);
        Assert.Equal((IntPtr)7, host.TList_GetItem(list, 0));
        host.TList_Remove(list, (IntPtr)7);
        Assert.Equal(2, host.TList_Count(list));
        host.TList_Delete(list, 0);
        Assert.Equal(1, host.TList_Count(list));

        var dest = host.TList_Create();
        host.TList_CopyTo(list, dest);
        Assert.Equal(host.TList_Count(list), host.TList_Count(dest));
        host.TList_Clear(list);
        Assert.Equal(0, host.TList_Count(list));
    }

    /// <summary>原文 `_TMemStream_*`（:1159-1222）：Size/SetSize/Clear/Read/Write/Seek/Position 往返。</summary>
    [Fact]
    public void TMemStream_Operations()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var s = host.TMemStream_Create();
        Assert.Equal(0, host.TMemStream_GetSize(s));

        var w = host.TMemStream_Write(s, new byte[] { 1, 2, 3, 4 }, 4);
        Assert.Equal(4, w);
        Assert.Equal(4, host.TMemStream_GetSize(s));
        Assert.Equal(4, host.TMemStream_GetPosition(s));

        Assert.Equal(0, host.TMemStream_Seek(s, 0, 0));    // 0=从头
        Assert.Equal(0, host.TMemStream_GetPosition(s));
        var buf = new byte[2];
        Assert.Equal(2, host.TMemStream_Read(s, buf, 2));
        Assert.Equal(new byte[] { 1, 2 }, buf);
        Assert.Equal(2, host.TMemStream_Seek(s, 0, 1));    // 1=当前位置

        host.TMemStream_SetPosition(s, 3);
        Assert.Equal(3, host.TMemStream_GetPosition(s));
        host.TMemStream_SetSize(s, 8);
        Assert.Equal(8, host.TMemStream_GetSize(s));
        host.TMemStream_Clear(s);
        Assert.Equal(0, host.TMemStream_GetSize(s));

        // 文件往返
        var path = Path.Combine(Path.GetTempPath(), "gxx_plugin_ms_" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            host.TMemStream_Write(s, new byte[] { 9, 8 }, 2);
            host.TMemStream_SaveToFile(s, Gbk(path));
            var s2 = host.TMemStream_Create();
            host.TMemStream_LoadFromFile(s2, Gbk(path));
            Assert.Equal(2, host.TMemStream_GetSize(s2));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // ------------------------------------------------------------------ TStrList 文本列表

    /// <summary>原文 `_TStrList_*`（:972-1153）：行增删、对象绑定、Exchange、Text 往返。</summary>
    [Fact]
    public void TStrList_Operations()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var l = host.TStrList_Create();
        Assert.Equal(0, host.TStrList_Count(l));

        host.TStrList_Add(l, Gbk("one"));
        host.TStrList_AddObject(l, Gbk("two"), "OBJ");
        host.TStrList_Insert(l, 0, Gbk("zero"));
        host.TStrList_InsertObject(l, 1, Gbk("half"), 42);

        Assert.Equal(4, host.TStrList_Count(l));
        Assert.Equal("zero", GetLine(host, l, 0));
        Assert.Equal("half", GetLine(host, l, 1));
        Assert.Equal(42, host.TStrList_GetObject(l, 1));
        // 原 :1095 / :1100：IndexOf / IndexOfObject 走 TStringList 的首个匹配 [one, half, zero, two]
        Assert.Equal(3, host.TStrList_IndexOf(l, Gbk("two")));
        Assert.Equal(3, host.TStrList_IndexOfObject(l, "OBJ"));

        host.TStrList_Exchange(l, 0, 2);
        Assert.Equal("one", GetLine(host, l, 0));

        var idx = -1;
        Assert.Equal(1, host.TStrList_Find(l, Gbk("half"), ref idx));
        idx = -1;
        Assert.Equal(0, host.TStrList_Find(l, Gbk("nope"), ref idx));

        host.TStrList_SetItem(l, 0, Gbk("ONE"));
        Assert.Equal("ONE", GetLine(host, l, 0));
        host.TStrList_SetObject(l, 0, 7);
        Assert.Equal(7, host.TStrList_GetObject(l, 0));

        host.TStrList_Remove(l, Gbk("nope"));
        Assert.Equal(4, host.TStrList_Count(l));
        host.TStrList_Remove(l, Gbk("ONE"));
        Assert.Equal(3, host.TStrList_Count(l));
        host.TStrList_Delete(l, 0);
        Assert.Equal(2, host.TStrList_Count(l));

        // 原文如此（PluginImplement.pas:1150）：TStrList_CopyTo 走 TStringList.Assign，
        // 只复制 Text（行文本），**不复制行绑定的对象** —— 照抄该边界。
        var dest = host.TStrList_Create();
        host.TStrList_CopyTo(l, dest);
        Assert.Equal(host.TStrList_Count(l), host.TStrList_Count(dest));
        Assert.NotNull(((IStringListHandle)dest).Text);
    }
    /// <summary>原文 `_TStrList_GetDuplicates/SetDuplicates`（:1002 / :1007）：dupAccept ↔ True。</summary>
    [Fact]
    public void TStrList_DuplicatesFlag()
    {
        var host = new PluginInterfaceHost(NewEnv());
        var l = host.TStrList_Create();
        Assert.Equal(0, host.TStrList_GetDuplicates(l));
        host.TStrList_SetDuplicates(l, 1);
        Assert.Equal(1, host.TStrList_GetDuplicates(l));
        host.TStrList_SetDuplicates(l, 0);
        Assert.Equal(0, host.TStrList_GetDuplicates(l));
    }

    // ------------------------------------------------------------------ 宿主实现体内部辅助

    /// <summary>原文 `BufferCrc`（宿主 :2352 调用）：同一份数据必须得到同一个值。</summary>
    [Fact]
    public void BufferCrc_IsDeterministic()
    {
        var a = new byte[] { 1, 2, 3, 4, 5 };
        var b = new byte[] { 1, 2, 3, 4, 5 };
        var c = new byte[] { 1, 2, 3, 4, 6 };
        Assert.Equal(BufferCrc.Compute(a), BufferCrc.Compute(b));
        Assert.NotEqual(BufferCrc.Compute(a), BufferCrc.Compute(c));
        Assert.Equal(0xCBF43926u, BufferCrc.Compute(System.Text.Encoding.ASCII.GetBytes("123456789"))); // 标准 CRC-32 校验值
    }

    // ------------------------------------------------------------------ 接缝替身

    /// <summary>接缝 `IEnvirnoment` 的最小替身（只用于 `GetMapParam**` 的空实现断言）。</summary>
    private sealed class FakeEnvir : IEnvirnoment
    {
        public string sMapName => "0";
        public string sMapDesc => "";
        public int m_nWidth => 0;
        public int m_nHeight => 0;
        public int nMinMap => 0;
        public bool m_boMainMap => false;
        public string sMainMapName => "";
        public bool m_boMirror => false;
        public uint m_dwMirrorCreateTick => 0;
        public uint m_dwMirrorSurvivalTime => 0;
        public string m_sMirrorExitToMap => "";
        public int m_nMirrorMinMap => 0;
        public bool m_boAlwaysShowTime => false;
        public bool m_boFB => false;
        public string m_sFBName => "";
        public int m_FBEnterLimit => 0;
        public bool m_boFBCreate => false;
        public uint m_dwFBCreateTime => 0;
        public bool CanWalk(int nX, int nY, bool boFlag) => false;
        public bool IsValidObject(int nX, int nY, int nRange, object aObject) => false;
        public int GeTItemObjects(int nX, int nY, IListHandle objectList) => 0;
        public int GeTBaseObjects(int nX, int nY, bool incDeathObject, IListHandle objectList) => 0;
        public int GetPlayObjects(int nX, int nY, bool incDeathObject, IListHandle objectList) => 0;
    }

    private sealed class FakeMenuItem : IMenuItem
    {
        private readonly List<IMenuItem> _children = new();
        public int Count => _children.Count;
        public IMenuItem GetItems(int index) => index < _children.Count ? _children[index] : AddChild();
        private IMenuItem AddChild() { var c = new FakeMenuItem(); _children.Add(c); return c; }
        public string Caption { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public bool Visable { get; set; }
        public bool Checked { get; set; }
        public bool RadioItem { get; set; }
        public int GroupIndex { get; set; }
        public int Tag { get; set; }
    }

    /// <summary>用原文的 `TStrList_GetItem` 读取一行（走出参缓冲，再按 DestLen 解 GBK）。</summary>
    private static string GetLine(PluginInterfaceHost host, IStringListHandle list, int index)
    {
        var dest = new byte[64];
        var len = 64u;
        Assert.Equal(1, host.TStrList_GetItem(list, index, dest, ref len));
        return GbkStr(dest, (int)len);
    }

    // ------------------------------------------------------------------ GBK 辅助

    private static byte[] Gbk(string s)
    {
        var b = GXX.Core.EncodingInit.GBK.GetBytes(s);
        var buf = new byte[b.Length + 1];
        Array.Copy(b, buf, b.Length);
        return buf;
    }

    private static string GbkStr(byte[] b, int len) => GXX.Core.EncodingInit.GBK.GetString(b, 0, len);
}
