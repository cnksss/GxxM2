// p3-m2-sweep 车道测试工具：内存文件系统 + 内存 INI + TPlayObject 假实现。
//
// 用途：Nations.pas 的 SaveConfig/LoadConfig/RenameNationName 全部依赖文件系统与 TIniFile，
// 按任务书「数据库访问走接缝，单测不连真库」的同一原则，这里也**不碰磁盘**：
//   * SweepSeam.CreateIniFile / FileExists / DirectoryExists / ForceDirectories / RenameFile 全部替换为内存实现；
//   * MainOutMessage 收进列表便于断言异常路径；
//   * MyGetTickCount 由测试直接控制（M2Locker 自旋锁超时路径）。
// 每个测试用 `using var env = new SweepTestEnv();` 安装，Dispose 时恢复默认并清空日志。

using System;
using System.Collections.Generic;
using GXX.M2Server.Sweep;

namespace GXX.M2Server.Tests;

/// <summary>内存文件系统 + INI 存储。</summary>
internal sealed class SweepTestFs
{
    /// <summary>已存在的目录集合（大小写不敏感，模拟 Windows）。</summary>
    public readonly HashSet<string> Dirs = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>文件 → 节 → 键 → 值（节/键大小写不敏感，模拟 Win32 profile API）。</summary>
    public readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> Files =
        new(StringComparer.OrdinalIgnoreCase);

    public bool FileExists(string path) => Files.ContainsKey(path);

    public bool DirectoryExists(string path) => Dirs.Contains(path);

    public void ForceDirectories(string path)
    {
        string p = path;
        while (!string.IsNullOrEmpty(p))
        {
            Dirs.Add(p);
            int i = p.LastIndexOf('\\');
            if (i <= 0) break;
            p = p.Substring(0, i);
        }
    }

    public bool RenameFile(string oldName, string newName)
    {
        if (Files.ContainsKey(newName) || !Files.ContainsKey(oldName)) return false;
        Files[newName] = Files[oldName];
        Files.Remove(oldName);
        return true;
    }

    /// <summary>预置一个文件（模拟磁盘上已存在的 ini）。</summary>
    public void Seed(string path, params (string section, string key, string value)[] entries)
    {
        var sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var (section, key, value) in entries)
        {
            if (!sections.TryGetValue(section, out var keys))
            {
                keys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                sections[section] = keys;
            }
            keys[key] = value;
        }
        Files[path] = sections;
    }

    public string Get(string path, string section, string key)
    {
        if (Files.TryGetValue(path, out var sections) && sections.TryGetValue(section, out var keys) &&
            keys.TryGetValue(key, out var v))
            return v;
        return null;
    }

    public bool Has(string path, string section, string key) => Get(path, section, key) != null;

    public int SectionCount(string path) => Files.TryGetValue(path, out var sections) ? sections.Count : 0;
}

/// <summary>内存 INI（<see cref="ISweepIniFile"/>），落盘时机与 <see cref="SweepFastIniFile"/> 对齐：只在写过时才登记文件。</summary>
internal sealed class MemIniFile : ISweepIniFile
{
    private readonly SweepTestFs _fs;
    private readonly string _path;
    private readonly Dictionary<string, Dictionary<string, string>> _sections =
        new(StringComparer.OrdinalIgnoreCase);
    private bool _dirty;

    public MemIniFile(SweepTestFs fs, string path)
    {
        _fs = fs;
        _path = path;
        if (fs.Files.TryGetValue(path, out var existing))
        {
            foreach (var kv in existing)
                _sections[kv.Key] = new Dictionary<string, string>(kv.Value, StringComparer.OrdinalIgnoreCase);
        }
    }

    public string ReadString(string section, string key, string defaultValue)
    {
        if (_sections.TryGetValue(section, out var keys) && keys.TryGetValue(key, out var v)) return v;
        return defaultValue;
    }

    public int ReadInteger(string section, string key, int defaultValue)
    {
        string s = ReadString(section, key, "");
        if (s.Length == 0) return defaultValue;
        if (s.Length > 2 && s[0] == '0' && (s[1] == 'x' || s[1] == 'X')) s = "$" + s.Substring(2);
        if (s[0] == '$')
            return int.TryParse(s.Substring(1), System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out int hex) ? hex : defaultValue;
        return int.TryParse(s, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out int v) ? v : defaultValue;
    }

    public void WriteString(string section, string key, string value)
    {
        if (!_sections.TryGetValue(section, out var keys))
        {
            keys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _sections[section] = keys;
        }
        keys[key] = value;
        _dirty = true;
    }

    public void WriteInteger(string section, string key, int value)
        => WriteString(section, key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    public void Dispose()
    {
        if (_dirty) _fs.Files[_path] = _sections;
    }
}

/// <summary>原文 <see cref="INationsPlayObject"/> 的假实现（记录收到的消息）。</summary>
internal sealed class FakeNationPlayer : INationsPlayObject
{
    /// <summary>原文 <c>TBaseObject.m_btNation: Word</c>（ObjBase.pas:408）。</summary>
    public ushort m_btNation { get; set; }
    public string m_sNationaName { get; set; } = "";

    /// <summary>SendMsg 收到的 (wIdent, wParam, nParam1, nParam2, nParam3, sMsg) 列表。</summary>
    public readonly List<(ushort wIdent, long wParam, long nParam1, long nParam2, long nParam3, string sMsg)> Sent = new();

    /// <summary>非 null 时 SendMsg 抛出该异常（用于覆盖 Nations.pas:206-211 的 except 路径）。</summary>
    public Exception ThrowOnSend;

    /// <summary>非 null 时读取 m_boBanNationChat 抛出该异常（用于覆盖 nCheckCode=4 的异常路径）。</summary>
    public Exception ThrowOnBanGet;

    private bool _banValue;

    public void SendMsg(ushort wIdent, long wParam, long nParam1, long nParam2, long nParam3, string sMsg)
    {
        if (ThrowOnSend != null) throw ThrowOnSend;
        Sent.Add((wIdent, wParam, nParam1, nParam2, nParam3, sMsg));
    }

    // 需要「属性 getter 可抛」 → 用显式接口实现
    bool INationsPlayObject.m_boBanNationChat
    {
        get
        {
            if (ThrowOnBanGet != null) throw ThrowOnBanGet;
            return _banValue;
        }
        set => _banValue = value;
    }

    /// <summary>不触发异常的常规读写（测试代码用）。</summary>
    public bool BanNationChat
    {
        get => _banValue;
        set => _banValue = value;
    }
}

/// <summary>
/// 安装/恢复 p3-m2-sweep 接缝的测试环境。
/// <para>同时保存并恢复被测试改动的 g_Config 字段（<see cref="GXX.M2Server.Engine.M2Config"/>）。</para>
/// </summary>
internal sealed class SweepTestEnv : IDisposable
{
    /// <summary>
    /// 本车道测试**固定**使用的 Envir 目录（= <c>M2Config.sEnvirDir</c> 的出厂默认值，见 M2Config.General.cs:40）。
    /// <para>为什么必须显式钉死：<c>M2Config</c> 是进程级静态全局（对应 Delphi 的 <c>g_Config</c>），
    /// 本测试工程里已有若干测试类会在用例体内改它且**不还原**
    /// （实测：<c>FormGeneralConfigTests.cs:488</c> 把 <c>sEnvirDir</c> 设成 <c>"D:\Mir\Envir\"</c>）。
    /// 由于 xUnit 的并行已被 <c>TestConfig.cs</c> 关闭、测试按类顺序执行，
    /// 一旦那类测试先跑，后续用例看到的 <c>sEnvirDir</c> 就是被污染的残留值；
    /// 此时若测试自己用**硬编码**的 <c>.\Envir\</c> 去 Seed 内存文件系统，就会与实现算出的路径错位
    /// → LoadConfig 找不到文件 → 一连串 NRE/断言失败（本轮集成分支上真实复现过 4 例）。
    /// 因此构造时**主动设定**、析构时**还原**，使本车道的用例与外部残留状态完全解耦。</para>
    /// </summary>
    public const string CanonicalEnvirDir = ".\\Envir\\";

    public readonly SweepTestFs Fs = new();

    private readonly bool _savedShowPreFixMsg = GXX.M2Server.Engine.M2Config.boShowPreFixMsg;
    private readonly string _savedNationMsgPreFix = GXX.M2Server.Engine.M2Config.sNationMsgPreFix;
    private readonly byte _savedFColor = GXX.M2Server.Engine.M2Config.btNationMsgFColor;
    private readonly byte _savedBColor = GXX.M2Server.Engine.M2Config.btNationMsgBColor;
    private readonly string _savedEnvirDir = GXX.M2Server.Engine.M2Config.sEnvirDir;

    /// <summary>
    /// 可控的 MyGetTickCount 时钟源。<b>默认每次调用自增 1</b>（而不是恒定值）——
    /// 这一点很关键：M2Locker 的四个自旋函数（SpinLock/SpinUnLock/BeginRead/BeginWrite）都是
    /// 「CAS 失败 → Sleep(0) → 查超时」的**无界**循环，若 tick 恒定不变，
    /// 一旦 CAS 无法成功（例如 Target 的 Bit0 已被占）就会**永久自旋**（本轮实测踩到过：
    /// 测试进程直接挂死，需 --blame-hang 才能定位）。
    /// 自增时钟让超时分支在 1000/3000 次迭代后确定性触发，同时不影响「CAS 一次成功」的常规路径。
    /// </summary>
    public uint Tick;

    public SweepTestEnv()
    {
        Tick = 0;
        // 钉死 Envir 目录：与外部残留解耦（理由见 CanonicalEnvirDir 的注释）
        GXX.M2Server.Engine.M2Config.sEnvirDir = CanonicalEnvirDir;
        SweepSeam.ResetDefaults();
        SweepSeam.CreateIniFile = path => new MemIniFile(Fs, path);
        SweepSeam.FileExists = Fs.FileExists;
        SweepSeam.DirectoryExists = Fs.DirectoryExists;
        SweepSeam.ForceDirectories = Fs.ForceDirectories;
        SweepSeam.RenameFile = Fs.RenameFile;
        SweepSeam.MyGetTickCount = () => Tick++;
        SweepSeam.LoggedMessages.Clear();
        SweepSeam.MainOutMessage = msg => SweepSeam.LoggedMessages.Add(msg ?? "");
    }

    public void Dispose()
    {
        GXX.M2Server.Engine.M2Config.boShowPreFixMsg = _savedShowPreFixMsg;
        GXX.M2Server.Engine.M2Config.sNationMsgPreFix = _savedNationMsgPreFix;
        GXX.M2Server.Engine.M2Config.btNationMsgFColor = _savedFColor;
        GXX.M2Server.Engine.M2Config.btNationMsgBColor = _savedBColor;
        GXX.M2Server.Engine.M2Config.sEnvirDir = _savedEnvirDir;
        SweepSeam.ResetDefaults();
    }
}
