using System;
using System.Runtime.InteropServices;
using GXX.Core;
using GXX.Core.Rtl;

namespace GXX.GameCenter;

/// <summary>
/// GShare.pas 第 13..46 行（interface 的 const/type 段）1:1 移植。
/// 说明：本文件只移植 GMain/GCertServerSet 实际用到的 GShare 声明部分（TProgram/TRunGateInfo/TCheckCode
/// 三个记录 + MAXRUNGATECOUNT 常量）；GShare.pas 的 LoadConfig/RunProgram/StopProgram/SendProgramMsg
/// 属独立单元，本车道未全部移植（见交付报告"未完成清单"）。
/// </summary>
public static class GShareConst
{
    /// <summary>GShare.pas:14 <c>MAXRUNGATECOUNT = 8;</c></summary>
    public const int MAXRUNGATECOUNT = 8;

    /// <summary>GShare.pas:17 <c>TWindowHandle = UInt64; //THandle;</c></summary>
    public const ulong INVALID_WINDOW_HANDLE = 0;
}

/// <summary>
/// GShare.pas:19-32 TProgram 1:1 移植（非 packed record）。
/// 字段顺序/类型逐字对应；<c>ProcessInfo: TProcessInformation</c> 在托管侧由
/// <see cref="ProcessHandle"/>（Delphi 侧 ProcessHandle 对应的 OS 进程句柄）承载——
/// 托管 Process 对象本身即同时持有 hProcess/pid，故 ProcessInfo 折叠进 ProcessHandle。
/// <c>sProgramFile: string[50]</c> / <c>sDirectory: string[100]</c> 为 ShortString，
/// 赋值时按 GBK 字节截断（见 <see cref="ProgramFile"/> / <see cref="Directory"/> 的 setter）。
/// </summary>
public struct TProgram
{
    /// <summary>是否随 GameCenter 一起启动。</summary>
    public bool boGetStart;

    /// <summary>程序异常停止，是否重新启动。</summary>
    public bool boReStart;

    /// <summary>启动后是否最小化。</summary>
    public bool boMinimize;

    /// <summary>0,1,2,3 未启动，正在启动，已启动,正在关闭。</summary>
    public byte btStartStatus;

    private string _sProgramFile;
    private string _sDirectory;

    /// <summary>Delphi 侧对应 <c>ProcessInfo</c>+<c>ProcessHandle</c> 两个字段（CreateProcess 的输出）。</summary>
    public IntPtr ProcessHandle;

    /// <summary>GShare.pas:29 <c>MainFormHandle: TWindowHandle</c>（没有证据证实 64 位程序窗口句柄是 64 位的，保险起见）。</summary>
    public ulong MainFormHandle;

    /// <summary>主窗体 X（作为命令行第 4 个参数传给被启动程序）。</summary>
    public int nMainFormX;

    /// <summary>主窗体 Y（作为命令行第 5 个参数传给被启动程序）。</summary>
    public int nMainFormY;

    /// <summary>GShare.pas:25 <c>sProgramFile: string[50]</c>（ShortString：GBK 字节截断到 50）。</summary>
    public string sProgramFile
    {
        get => _sProgramFile ?? "";
        set => _sProgramFile = ShortStringClamp(value, 50);
    }

    /// <summary>GShare.pas:26 <c>sDirectory: string[100]</c>（ShortString：GBK 字节截断到 100）。</summary>
    public string sDirectory
    {
        get => _sDirectory ?? "";
        set => _sDirectory = ShortStringClamp(value, 100);
    }

    /// <summary>Delphi <c>string[N]</c> 赋值语义：按 GBK 字节截断到 N 字节后回解为字符串。</summary>
    public static string ShortStringClamp(string value, int capacity)
    {
        if (string.IsNullOrEmpty(value)) return "";
        byte[] data = EncodingInit.GBK.GetBytes(value);
        if (data.Length <= capacity) return value;
        return EncodingInit.GBK.GetString(data, 0, capacity);
    }

    /// <summary>便捷构造（等价于 Delphi 中以记录变量初始化后逐字段赋值的写法）。</summary>
    public static TProgram Create(bool getStart, bool minimize, bool reStart, int formX, int formY,
                                  string programFile, string directory)
    {
        var p = new TProgram
        {
            boGetStart = getStart,
            boMinimize = minimize,
            boReStart = reStart,
            btStartStatus = 0,
            MainFormHandle = GShareConst.INVALID_WINDOW_HANDLE,
            nMainFormX = formX,
            nMainFormY = formY,
        };
        p.sProgramFile = programFile;
        p.sDirectory = directory;
        return p;
    }
}

/// <summary>
/// GShare.pas:36-41 TRunGateInfo 1:1 移植（非 packed record）。
/// </summary>
public struct TRunGateInfo
{
    /// <summary>该网关是否参与启动（由 <c>g_nRunGate_Count</c> 推导）。</summary>
    public bool boGetStart;

    private string _sGateAddr;

    /// <summary>GShare.pas:38 <c>sGateAddr: string[15]</c>（ShortString）。</summary>
    public string sGateAddr
    {
        get => _sGateAddr ?? "";
        set => _sGateAddr = TProgram.ShortStringClamp(value, 15);
    }

    /// <summary>对外网关端口（GameGate/GatePort）。</summary>
    public int nGatePort;

    /// <summary>网关连数据库端口（GameGate/DBPort）。</summary>
    public int nDBPort;
}

/// <summary>
/// GShare.pas:43-46 TCheckCode 1:1 移植。
/// </summary>
public struct TCheckCode
{
    public uint dwThread0;
    public string sThread0;
}

/// <summary>
/// SysUtils.BoolToStr 的 Delphi 7 语义（默认 UseBoolStrs=False）：
/// True → '-1'，False → '0'。
/// 注意与 <c>GXX.Core.Util.HUtil32.BoolToStr</c>（返回 "True"/"False"）不同——
/// GMain.pas 多处 <c>IniGameConf.WriteString(..., BoolToStr(...))</c> 落盘文本依赖本语义。
/// </summary>
public static class DelphiSystem
{
    /// <summary>SysUtils.BoolToStr(Boolean)（Delphi 7 默认重载）。</summary>
    public static string BoolToStr(bool value) => value ? "-1" : "0";

    /// <summary>SysUtils.BoolToStr(Boolean, Boolean)（UseBoolStrs=True → 'True'/'False'）。</summary>
    public static string BoolToStr(bool value, bool useBoolStrs)
        => useBoolStrs ? (value ? "True" : "False") : (value ? "-1" : "0");

    /// <summary>SysUtils.TrimLeft/TrimRight/Trim 等价（Delphi 空白集：空格+#0..#32）。</summary>
    public static string Trim(string s)
    {
        if (s == null) return "";
        int b = 0, e = s.Length;
        while (b < e && s[b] <= ' ') b++;
        while (e > b && s[e - 1] <= ' ') e--;
        return s.Substring(b, e - b);
    }

    /// <summary>SysUtils.IntToStr。</summary>
    public static string IntToStr(int v) => v.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

/// <summary>
/// System.pas <c>StringReplace</c> 的 Delphi 语义（本车道用到 rfReplaceAll+rfIgnoreCase 组合）。
/// </summary>
public static class DelphiStringReplace
{
    /// <summary>CheckPrevious.pas:37 <c>StringReplace(ParamStr(0), '\', '', [rfReplaceAll, rfIgnoreCase])</c>。</summary>
    public static string RemoveBackslash(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        return path.Replace("\\", "");
    }
}

/// <summary>
/// CheckPrevious.pas 第 10..15 行的 <c>TInstanceInfo</c> packed record 布局
/// （THandle=4 字节 + Integer=4 字节，共 8 字节）。
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TInstanceInfo
{
    /// <summary>已运行实例的主窗口句柄。</summary>
    public uint PreviousHandle;

    /// <summary>实例计数（初始 1，第二个实例起累加）。</summary>
    public int RunCounter;
}
