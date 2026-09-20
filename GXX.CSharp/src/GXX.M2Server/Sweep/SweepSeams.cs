// 本文件 = p3-m2-sweep 车道（M2Engine 零散单元清扫）各单元共用的**最小接缝层**。
//
// 服务单元（源单元名一律写全路径，供 tools/audit-coverage.ps1 的 E2 证据规则识别）：
//   Source/M2Engine/Nations.pas        —— INI 读写 / 文件系统 / 日志
//   Source/M2Engine/M2Locker.pas       —— MyGetTickCount（原文 M2Share.pas:3589 = timeGetTime）
//   Source/M2Engine/GameGoldDealDB.pas —— 见同目录 GameGoldDealDB.cs
//   Source/M2Engine/HTTPService.pas    —— 见同目录 HTTPService.cs
//
// 【为什么不直接移植依赖】
//   任务书第 2 条：不顺手移植依赖，只定义最小接缝，并注释「接缝：待 <单元名> 移植后接入」。
//   这些能力分别属于未移植的单元（M2Share.pas 的日志/计时、IniFiles.pas 的 TIniFile、
//   SysUtils 的文件系统函数），本车道**不**复制它们，只把「用到的那几个成员」抽象出来，
//   默认实现映射到既有 GXX.Core 产物（TFastIniFile / DelphiRTL），单测可替换为内存实现。
//
// 【复用而非另造】
//   * INI 读写 → GXX.Core.Util.TFastIniFile（FastIniFile.pas 1:1，已在 GXX.Core.Tests 覆盖）；
//   * 计时     → GXX.Core.Rtl.DelphiRTL.GetTickCount（GetTickCount 语义，uint 回绕）；
//   * 配置字段 → GXX.M2Server.Engine.M2Config（g_Config 各字段，已由顺序会话移植）。
//   本文件不复制任何一份上述实现，只做「接口 + 默认转发」。

using System;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Sweep;

/// <summary>
/// Delphi <c>IniFiles.TIniFile</c> 在本车道用到的**最小面**。
/// 原文各单元只用 Create / ReadString / ReadInteger / WriteString / WriteInteger / Free。
/// 接缝：待 IniFiles.pas 的 TIniFile 在 GXX.Core（或 M2Server）归位后接入（默认实现见
/// <see cref="SweepFastIniFile"/>，它转发到既有的 GXX.Core TFastIniFile）。
/// </summary>
public interface ISweepIniFile : IDisposable
{
    /// <summary>原文 <c>TIniFile.ReadString(Section, Ident, Default)</c>。</summary>
    string ReadString(string section, string key, string defaultValue);

    /// <summary>原文 <c>TIniFile.ReadInteger(Section, Ident, Default)</c>。</summary>
    int ReadInteger(string section, string key, int defaultValue);

    /// <summary>原文 <c>TIniFile.WriteString(Section, Ident, Value)</c>。</summary>
    void WriteString(string section, string key, string value);

    /// <summary>原文 <c>TIniFile.WriteInteger(Section, Ident, Value)</c>。</summary>
    void WriteInteger(string section, string key, int value);
}

/// <summary>
/// <see cref="ISweepIniFile"/> 的默认实现：包装既有的 <see cref="TFastIniFile"/>。
///
/// 【落盘时机对齐】原文 <c>TIniFile</c> 是**写穿（write-through）**的（IniFiles.pas 走 Win32
/// profile API），而 <c>TFastIniFile</c> 是内存缓存 + 显式 Save。原文各单元的写法是
/// <c>Config := TIniFile.Create(...); ...写入...; Config.Free;</c>，因此这里把「析构即落盘」
/// 对齐到 <see cref="Dispose"/>，并且**仅在发生过写入时**才落盘 —— 只读用法（如 Nations.LoadConfig
/// 只 ReadString）在原文里不会创建文件，托管侧也必须不创建（脏标记见 <c>_dirty</c>）。
/// </summary>
public sealed class SweepFastIniFile : ISweepIniFile
{
    private readonly TFastIniFile _ini;
    private bool _dirty;

    public SweepFastIniFile(string fileName)
    {
        _ini = new TFastIniFile(fileName);
    }

    public string ReadString(string section, string key, string defaultValue)
        => _ini.ReadString(section, key, defaultValue);

    public int ReadInteger(string section, string key, int defaultValue)
        => _ini.ReadInteger(section, key, defaultValue);

    public void WriteString(string section, string key, string value)
    {
        _ini.WriteString(section, key, value);
        _dirty = true;
    }

    public void WriteInteger(string section, string key, int value)
    {
        _ini.WriteInteger(section, key, value);
        _dirty = true;
    }

    public void Dispose()
    {
        // 原文 Config.Free（写穿语义）→ 托管侧在此落盘；未写入过则不动磁盘。
        if (_dirty) _ini.Save();
        _ini.Dispose();
    }
}

/// <summary>
/// 本车道各单元共用的可替换接缝（默认全部映射到既有的 GXX.Core / BCL 实现）。
/// 单测通过替换这些委托来注入内存文件系统/假 INI/可控计时，**不触碰磁盘**。
/// </summary>
public static class SweepSeam
{
    /// <summary>原文 <c>IniFiles.TIniFile.Create(FileName)</c>。接缝：待 TIniFile 移植后接入。</summary>
    public static Func<string, ISweepIniFile> CreateIniFile { get; set; } = fileName => new SweepFastIniFile(fileName);

    /// <summary>原文 <c>SysUtils.FileExists(FileName): Boolean</c>。接缝：待 DelphiRTL 收录后接入。</summary>
    public static Func<string, bool> FileExists { get; set; } = File.Exists;

    /// <summary>原文 <c>SysUtils.DirectoryExists(Dir): Boolean</c>。接缝：待 DelphiRTL 收录后接入。</summary>
    public static Func<string, bool> DirectoryExists { get; set; } = Directory.Exists;

    /// <summary>原文 <c>SysUtils.ForceDirectories(Dir): Boolean</c>（返回值原文未使用）。</summary>
    public static Action<string> ForceDirectories { get; set; } = dir => Directory.CreateDirectory(dir);

    /// <summary>
    /// 原文 <c>SysUtils.RenameFile(OldName, NewName): Boolean</c>。
    /// Delphi 7 实现是 Win32 <c>MoveFile</c> —— **目标已存在时失败**（不覆盖），故这里显式判存。
    /// 返回值原文在 Nations.pas:382 被忽略（原文如此）。
    /// </summary>
    public static Func<string, string, bool> RenameFile { get; set; } = (oldName, newName) =>
    {
        if (File.Exists(newName)) return false;
        try
        {
            File.Move(oldName, newName);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    };

    /// <summary>
    /// 原文 <c>M2Share.MainOutMessage(sMsg: string)</c>（M2Server 主窗体日志输出）。
    /// 接缝：待 M2Share.pas 的日志输出移植后接入（当前落点见 <see cref="LoggedMessages"/>）。
    /// </summary>
    public static Action<string> MainOutMessage { get; set; } = msg => LoggedMessages.Add(msg ?? "");

    /// <summary>进程内日志观察列表（MainOutMessage 的默认可观测落点，便于断言异常路径）。</summary>
    public static readonly System.Collections.Generic.List<string> LoggedMessages = new();

    /// <summary>
    /// 原文 <c>M2Share.MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime'</c>
    /// （M2Share.pas:3589，winmm 的毫秒计数，uint 回绕）。接缝：待 M2Share.pas 移植后接入。
    /// </summary>
    public static Func<uint> MyGetTickCount { get; set; } = DelphiRTL.GetTickCount;

    /// <summary>恢复全部接缝默认值（测试隔离用）。</summary>
    public static void ResetDefaults()
    {
        CreateIniFile = fileName => new SweepFastIniFile(fileName);
        FileExists = File.Exists;
        DirectoryExists = Directory.Exists;
        ForceDirectories = dir => Directory.CreateDirectory(dir);
        RenameFile = (oldName, newName) =>
        {
            if (File.Exists(newName)) return false;
            try
            {
                File.Move(oldName, newName);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        };
        MainOutMessage = msg => LoggedMessages.Add(msg ?? "");
        MyGetTickCount = DelphiRTL.GetTickCount;
        LoggedMessages.Clear();
    }
}
