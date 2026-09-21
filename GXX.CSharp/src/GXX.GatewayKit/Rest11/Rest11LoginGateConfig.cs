using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Rtl;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// `Source/LoginGate/ConfigManager.pas` → `Rest11LoginGateConfig.cs`
/// 1:1 逐字移植 `TConfigMgr`（:10-52 类型与字段、:56-88 构造、:90-94 析构、
/// :96-141 四个 Read*、:143-210 LoadConfig、:212-267 SaveConfig）。
///
/// <para>
/// ⚠ 与既有设施的关系（**并存**，不合并）：
/// `GXX.GatewayKit.GateService.LoadConfig`（GateService.cs:67-74）只读 5 个键
/// （`Gateway/GateAddr`、`Gateway/GatePort`、`Server/ServerAddr`、`Server/ServerPort`、
/// `PacketRule/MaxConnOfIPaddr`），段名与 LoginGate 原文 **`[LoginGate]/[Integer]/[Switch]/[Method]` 完全不同**。
/// 本类**不修改** `GateService`，而是把 19 个字段 + 原文段名完整落地，供 LoginGate
/// **显式 opt-in**（例如在确认 `Config.ini` 已按原文段名迁移后再切换）。
/// `GXX.SelGate.CConfigMgr`（SelGateConfig.cs）是同一份 `.pas` 的 **SelGate 副本**投影，
/// 段名为 `Strings/Integer/Switch/Method`、默认端口 5100/7100+i ⇒ 两者**不得互相赋值**。
/// </para>
///
/// <para>
/// 原文缺陷（照抄，未顺手修）：
/// <list type="number">
///   <item>★ `SaveConfig`（:212-267）在 `case 0` 分支使用了变量名 `I`（:231 `if I = 1 then`），
///         但该过程**只声明了 `i: Integer`（小写 i）**，Delphi 标识符大小写不敏感 ⇒ 这**不是**缺陷；
///         真正的问题是 `:231` 的 `I` 与循环变量 `i` 同物，逻辑上恒等价于 `if i = 1 then`。
///         **本实现按 `i = 1` 照抄**（原文如此：Delphi 大小写不敏感）。</item>
///   <item>`ReadFloat`（:132-141）声明了 `szLoadDW` 却从未使用，且**两次读取同一键**
///         （:137 判定、:140 取值）——照抄"读两次"。</item>
///   <item>`LoadConfig`（:179）用 `ReadInteger('LoginGate','Count',0)` 的 **0** 作默认值，
///         而 `ReadInteger` 对 `&lt; 0` 才视为缺失（:114）⇒ 0 会被**当作有效值读回**，
///         于是 `m_nGateCount &lt;= 0` 分支（:180-193）在"键存在且为 0"时也会进入。照抄。</item>
///   <item>`m_tBlockIPMethod`（:39）在构造函数中**未赋值** ⇒ Delphi 序型默认 `mDisconnect`（0）。
///         `LoadConfig`（:177）读 `Method/BlockIPMethod`，缺失时 `ReadInteger` 以
///         `Integer(m_tBlockIPMethod)`（=0）作默认并**回写**。照抄。注意此处默认值是
///         `mDisconnect`(0)，而 `ReadInteger` 把 `&lt; 0` 当缺失 ⇒ 0 仍被接受。</item>
/// </list>
/// </para>
/// </summary>
public class Rest11LoginGateConfig
{
    /// <summary>`ConfigManager.pas:24` `array[1..MAX_SERVER_COUNT]`；`MAX_SERVER_COUNT = 32`（AcceptExWorkedThread.pas:25）。</summary>
    public const int MAX_SERVER_COUNT = 32;

    /// <summary>`ConfigManager.pas:10-14` `TGameGateList = record sServerAdress: string[15]; nServerPort; nGatePort`。</summary>
    public class TGameGateList
    {
        public string sServerAdress = ""; // :11 string[15] → 普通 string（写入 INI 时原样落盘，不截断）
        public int nServerPort;           // :12
        public int nGatePort;             // :13
    }

    // ---- ConfigManager.pas:18-24 ----
    public readonly Rest11LoginGateIniFile m_xIni;                                        // :18 TIniFile
    public string m_szTitle = "";                                                          // :19
    public volatile int m_nShowLogLevel;                                                   // :20
    public volatile int m_nGateCount;                                                      // :21
    public volatile bool m_boCheckVersion;                                                 // :22
    public string m_sClientSoftVer = "";                                                   // :23
    /// <summary>:24 `m_xGameGateList: array[1..MAX_SERVER_COUNT]`；C# 下标 1..32（0 号槽弃用）。</summary>
    public readonly TGameGateList[] m_xGameGateList = new TGameGateList[MAX_SERVER_COUNT + 1];

    // ---- ConfigManager.pas:26-31 LongBool ----
    public volatile bool m_fCheckNewIDOfIP;      // :26
    public volatile bool m_fCheckNullSession;    // :27
    public volatile bool m_fOverSpeedSendBack;   // :28
    public volatile bool m_fDefenceCCPacket;     // :29
    public volatile bool m_fKickOverSpeed;       // :30
    public volatile bool m_fKickOverPacketSize;  // :31

    // ---- ConfigManager.pas:33-37 Integer ----
    public volatile int m_nCheckNewIDOfIP;       // :33
    public volatile int m_nMaxConnectOfIP;       // :34
    public volatile int m_nClientTimeOutTime;    // :35
    public volatile int m_nNomClientPacketSize;  // :36
    public volatile int m_nMaxClientPacketCount; // :37

    /// <summary>:39 `m_tBlockIPMethod: TBlockIPMethod`（构造未赋值 ⇒ 默认首项 mDisconnect = 0，原文如此）。</summary>
    public volatile int m_tBlockIPMethod;

    /// <summary>`ConfigManager.pas:56-88` `constructor TConfigMgr.Create(szFileName)`。</summary>
    public Rest11LoginGateConfig(string szFileName)
    {
        m_xIni = new Rest11LoginGateIniFile(szFileName); // :61
        m_szTitle = "";                                  // :62 原文如此：//'登录网关'; //原来是名称是： 角色网关，明显错误
        m_nShowLogLevel = 3;                             // :63

        m_boCheckVersion = false;                        // :65
        m_sClientSoftVer = "";                           // :66

        m_nGateCount = 1;                                // :68
        for (int i = 1; i <= MAX_SERVER_COUNT; i++)      // :69 Low..High(m_xGameGateList)
        {
            var g = new TGameGateList();
            g.sServerAdress = "127.0.0.1";               // :71
            g.nServerPort = 5500;                        // :72 ← LoginGate 副本默认端口（SelGate 副本是 5100）
            g.nGatePort = 7000 + i - 1;                  // :73 ← LoginGate 副本默认端口（SelGate 副本是 7100+i-1）
            m_xGameGateList[i] = g;
        }

        m_fCheckNewIDOfIP = true;        // :76
        m_fCheckNullSession = true;      // :77
        m_fOverSpeedSendBack = false;    // :78
        m_fDefenceCCPacket = false;      // :79
        m_fKickOverSpeed = false;        // :80
        m_fKickOverPacketSize = true;    // :81

        m_nNomClientPacketSize = 700;    // :83
        m_nMaxConnectOfIP = 20;          // :84
        m_nCheckNewIDOfIP = 5;           // :85
        m_nClientTimeOutTime = 180 * 1000; // :86
        m_nMaxClientPacketCount = 20;    // :87

        // :39 m_tBlockIPMethod 未赋值 ⇒ Delphi 序型默认首项 mDisconnect = 0（原文如此）
        m_tBlockIPMethod = (int)Rest11TBlockIPMethod.mDisconnect;
    }

    /// <summary>`ConfigManager.pas:90-94` `destructor Destroy`（`m_xIni.Free`；托管 INI 每次写入即落盘）。</summary>
    public void Destroy() => m_xIni.Dispose();

    // =====================================================================================
    // ConfigManager.pas:96-141 —— 四个 Read*：把"缺失/非法"的键**立即回写默认值**
    // =====================================================================================

    /// <summary>`ConfigManager.pas:96-106`：空串视为缺失并回写 `Default`。</summary>
    public string ReadString(string Section, string Ident, string Default)
    {
        string Result = Default;                                 // :100
        string szLoadStr = m_xIni.ReadString(Section, Ident, ""); // :101
        if (szLoadStr == "")                                     // :102
            m_xIni.WriteString(Section, Ident, Default);          // :103
        else
            Result = szLoadStr;                                   // :105
        return Result;
    }

    /// <summary>`ConfigManager.pas:108-118`：`&lt; 0` 视为缺失并回写 `Default`（原文用 -1 探测）。</summary>
    public int ReadInteger(string Section, string Ident, int Default)
    {
        int Result = Default;                                       // :112
        int szLoadInt = m_xIni.ReadInteger(Section, Ident, -1);     // :113
        if (szLoadInt < 0)                                          // :114
            m_xIni.WriteInteger(Section, Ident, Default);            // :115
        else
            Result = szLoadInt;                                      // :117
        return Result;
    }

    /// <summary>`ConfigManager.pas:120-130`：`&lt; 0` 视为缺失并回写 `Default`；否则 `&lt;&gt; 0`。</summary>
    public bool ReadBool(string Section, string Ident, bool Default)
    {
        // :124-129 原文：szLoadInt := m_xIni.ReadInteger(Section, Ident, -1);
        //              if szLoadInt < 0 then WriteBool(Default) else Result := szLoadInt <> 0
        int szLoadInt = m_xIni.ReadInteger(Section, Ident, -1);
        if (szLoadInt < 0)
        {
            m_xIni.WriteBool(Section, Ident, Default);               // :127
            return Default;
        }
        return szLoadInt != 0;                                       // :129
    }

    /// <summary>
    /// `ConfigManager.pas:132-141`：原文声明 `szLoadDW: Double` 后**从未使用**，
    /// 且 `:137` 判定 + `:140` 取值**读了两次**（首次读默认 0）——照抄。
    /// </summary>
    public double ReadFloat(string Section, string Ident, double Default)
    {
        double Result = Default;                                                       // :136
        if (m_xIni.ReadFloat(Section, Ident, 0) < 0.10)                                // :137
            m_xIni.WriteFloat(Section, Ident, Default);                                 // :138
        else
            Result = m_xIni.ReadFloat(Section, Ident, Default);                         // :140
        return Result;
    }

    // =====================================================================================
    // ConfigManager.pas:143-210 LoadConfig
    // 段名：[LoginGate] / [Integer] / [Switch] / [Method]
    // =====================================================================================
    public void LoadConfig()
    {
        // String
        m_szTitle = ReadString("LoginGate", "Title", m_szTitle);                                  // :148

        // Integer
        m_nShowLogLevel = ReadInteger("LoginGate", "ShowLogLevel", m_nShowLogLevel);              // :151

        m_boCheckVersion = ReadBool("LoginGate", "CheckClientSoft", m_boCheckVersion);            // :153
        m_sClientSoftVer = ReadString("LoginGate", "ClientSoftVer", m_sClientSoftVer);            // :154

        m_nClientTimeOutTime = ReadInteger("Integer", "ClientTimeOutTime3", m_nClientTimeOutTime); // :156
        if (m_nClientTimeOutTime < 10 * 1000)                                                      // :157
        {
            m_nClientTimeOutTime = 10 * 1000;                                                      // :159
            m_xIni.WriteInteger("Integer", "ClientTimeOutTime3", m_nClientTimeOutTime);            // :160
        }

        m_nMaxConnectOfIP = ReadInteger("Integer", "MaxConnectOfIP", m_nMaxConnectOfIP);                    // :163
        m_nCheckNewIDOfIP = ReadInteger("Integer", "CheckNewIDOfIP", m_nCheckNewIDOfIP);                    // :164
        m_nNomClientPacketSize = ReadInteger("Integer", "NomClientPacketSize2", m_nNomClientPacketSize);    // :165
        m_nMaxClientPacketCount = ReadInteger("Integer", "MaxClientPacketCount2", m_nMaxClientPacketCount);// :166

        // Boolean
        m_fCheckNewIDOfIP = ReadBool("Switch", "CheckNewIDOfIP", m_fCheckNewIDOfIP);               // :169
        m_fCheckNullSession = ReadBool("Switch", "CheckNullSession", m_fCheckNullSession);         // :170
        m_fOverSpeedSendBack = ReadBool("Switch", "OverSpeedSendBack", m_fOverSpeedSendBack);      // :171
        m_fDefenceCCPacket = ReadBool("Switch", "DefenceCCPacket", m_fDefenceCCPacket);            // :172
        m_fKickOverSpeed = ReadBool("Switch", "KickOverSpeed", m_fKickOverSpeed);                  // :173
        m_fKickOverPacketSize = ReadBool("Switch", "KickOverPacketSize", m_fKickOverPacketSize);   // :174

        m_tBlockIPMethod = ReadInteger("Method", "BlockIPMethod", m_tBlockIPMethod);               // :177

        m_nGateCount = ReadInteger("LoginGate", "Count", 0);                                        // :179
        if (m_nGateCount <= 0)                                                                      // :180
        {
            m_nGateCount = 1;                                                                       // :182
            m_xGameGateList[1].sServerAdress = ReadString("LoginGate", "ServerAddr", m_xGameGateList[1].sServerAdress);   // :183
            m_xGameGateList[1].nServerPort = ReadInteger("LoginGate", "ServerPort", m_xGameGateList[1].nServerPort);     // :184
            m_xGameGateList[1].nGatePort = ReadInteger("LoginGate", "GatePort", m_xGameGateList[1].nGatePort);           // :185

            for (int I = 2; I <= MAX_SERVER_COUNT; I++)                                             // :187 for I := 2 to MAX_SERVER_COUNT
            {
                m_xGameGateList[I].sServerAdress = m_xGameGateList[1].sServerAdress;                 // :189
                m_xGameGateList[I].nServerPort = m_xGameGateList[1].nServerPort;                     // :190
                m_xGameGateList[I].nGatePort = m_xGameGateList[1].nGatePort + I - 1;                 // :191
            }
        }
        else
        {
            for (int i = 1; i <= m_nGateCount; i++)                                                  // :196 for i := 1 to m_nGateCount
            {
                m_xGameGateList[i].sServerAdress = ReadString("LoginGate", "ServerAddr" + DelphiRTL.IntToStr(i), m_xGameGateList[i].sServerAdress); // :198
                m_xGameGateList[i].nServerPort = ReadInteger("LoginGate", "ServerPort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nServerPort);   // :199
                m_xGameGateList[i].nGatePort = ReadInteger("LoginGate", "GatePort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nGatePort);         // :200
            }

            for (int I = m_nGateCount + 1; I <= MAX_SERVER_COUNT; I++)                               // :203 for I := m_nGateCount + 1 to MAX_SERVER_COUNT
            {
                m_xGameGateList[I].sServerAdress = m_xGameGateList[1].sServerAdress;                 // :205
                m_xGameGateList[I].nServerPort = m_xGameGateList[1].nServerPort;                     // :206
                m_xGameGateList[I].nGatePort = m_xGameGateList[1].nGatePort + I - 1;                 // :207
            }
        }
    }

    // =====================================================================================
    // ConfigManager.pas:212-267 SaveConfig —— nType = 0/1/2 三分支，键写入顺序照抄
    // =====================================================================================
    public void SaveConfig(int nType)
    {
        switch (nType)
        {
            case 0:                                                                                  // :219
                m_xIni.WriteString("LoginGate", "Title", m_szTitle);                                 // :221
                m_xIni.WriteInteger("LoginGate", "ShowLogLevel", m_nShowLogLevel);                   // :222

                m_xIni.WriteInteger("LoginGate", "Count", m_nGateCount);                             // :224
                for (int i = 1; i <= m_nGateCount; i++)                                              // :225 for i := 1 to m_nGateCount
                {
                    m_xIni.WriteString("LoginGate", "ServerAddr" + DelphiRTL.IntToStr(i), m_xGameGateList[i].sServerAdress); // :227
                    m_xIni.WriteInteger("LoginGate", "ServerPort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nServerPort);  // :228
                    m_xIni.WriteInteger("LoginGate", "GatePort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nGatePort);      // :229

                    // :231 `if I = 1 then` —— Delphi 标识符大小写不敏感，此处即循环变量 i（原文如此）
                    if (i == 1)
                    {
                        m_xIni.WriteString("LoginGate", "ServerAddr", m_xGameGateList[i].sServerAdress); // :233
                        m_xIni.WriteInteger("LoginGate", "ServerPort", m_xGameGateList[i].nServerPort);  // :234
                        m_xIni.WriteInteger("LoginGate", "GatePort", m_xGameGateList[i].nGatePort);      // :235
                    }
                }

                m_xIni.WriteBool("LoginGate", "CheckClientSoft", m_boCheckVersion);                  // :239
                m_xIni.WriteString("LoginGate", "ClientSoftVer", m_sClientSoftVer);                  // :240
                break;

            case 1:                                                                                  // :242
                // Integer
                m_xIni.WriteInteger("Integer", "MaxConnectOfIP", m_nMaxConnectOfIP);                 // :245
                m_xIni.WriteInteger("Integer", "CheckNewIDOfIP", m_nCheckNewIDOfIP);                 // :246
                m_xIni.WriteInteger("Integer", "ClientTimeOutTime3", m_nClientTimeOutTime);          // :247
                m_xIni.WriteInteger("Integer", "NomClientPacketSize2", m_nNomClientPacketSize);      // :248
                m_xIni.WriteInteger("Integer", "MaxClientPacketCount2", m_nMaxClientPacketCount);    // :249

                // Boolean
                m_xIni.WriteBool("Switch", "CheckNewIDOfIP", m_fCheckNewIDOfIP);                     // :252
                m_xIni.WriteBool("Switch", "CheckNullSession", m_fCheckNullSession);                 // :253
                m_xIni.WriteBool("Switch", "OverSpeedSendBack", m_fOverSpeedSendBack);               // :254
                m_xIni.WriteBool("Switch", "DefenceCCPacket", m_fDefenceCCPacket);                   // :255
                m_xIni.WriteBool("Switch", "KickOverSpeed", m_fKickOverSpeed);                       // :256
                m_xIni.WriteBool("Switch", "KickOverPacketSize", m_fKickOverPacketSize);             // :257
                //
                m_xIni.WriteInteger("Method", "BlockIPMethod", m_tBlockIPMethod);                    // :259
                break;

            case 2:                                                                                  // :261
                // :262-264 原文如此：空分支
                break;
        }
    }
}

/// <summary>
/// Delphi `IniFiles.TIniFile` 的**保序**最小复刻（保留 `[LoginGate]` 等原文段名时所需的读写落盘）。
///
/// <para>
/// 与 `GXX.SelGate.SelIniFile` 的实现同构（同一份 Delphi `TIniFile` 语义的**两份投影**：
/// LoginGate 不得依赖 SelGate，反之亦然）；与 `GXX.Core.Util.TFastIniFile` 的差异见
/// `SelGateConfig.cs:264-273` 的登记（`ReadBool` 默认值语义、WriteString 空值不写、
/// Boolean 落盘为 -1/0）。
/// </para>
/// </summary>
public class Rest11LoginGateIniFile : IDisposable
{
    private readonly string _fileName;
    private readonly List<string> _sectionOrder = new();
    private readonly Dictionary<string, List<string>> _keyOrder = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Dictionary<string, string>> _sections = new(StringComparer.OrdinalIgnoreCase);

    public Rest11LoginGateIniFile(string fileName)
    {
        _fileName = fileName;
        Load();
    }

    public string FileName => _fileName;

    private void Load()
    {
        _sections.Clear();
        _sectionOrder.Clear();
        _keyOrder.Clear();
        if (!File.Exists(_fileName)) return;

        string current = "";
        foreach (string raw in File.ReadAllLines(_fileName, EncodingInit.GBK))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(';')) continue;
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                current = line.Substring(1, line.Length - 2).Trim();
                EnsureSection(current);
                continue;
            }
            if (current == "") continue;

            // Delphi TIniFile：无 '=' 的行按整行当键名、值为空串处理
            int eq = line.IndexOf('=');
            string key = eq >= 0 ? line.Substring(0, eq).Trim() : line.Trim();
            string val = eq >= 0 ? line.Substring(eq + 1).Trim() : "";
            EnsureSection(current);
            if (!_sections[current].ContainsKey(key)) _keyOrder[current].Add(key);
            _sections[current][key] = val;
        }
    }

    private void EnsureSection(string section)
    {
        if (_sections.ContainsKey(section)) return;
        _sections[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _keyOrder[section] = new List<string>();
        _sectionOrder.Add(section);
    }

    public string ReadString(string section, string ident, string defaultValue)
    {
        if (_sections.TryGetValue(section, out var keys) && keys.TryGetValue(ident, out string? v))
            return v ?? defaultValue;
        return defaultValue;
    }

    /// <summary>Delphi `TIniFile.ReadInteger`：`StrToIntDef` 语义，失败取默认值。</summary>
    public int ReadInteger(string section, string ident, int defaultValue)
        => DelphiRTL.StrToIntDef(ReadString(section, ident, ""), defaultValue);

    /// <summary>Delphi `TIniFile.ReadBool`：可解析为整数则 `&lt;&gt; 0`，否则取默认值。</summary>
    public bool ReadBool(string section, string ident, bool defaultValue)
        => ReadInteger(section, ident, defaultValue ? -1 : 0) != 0;

    /// <summary>Delphi `TIniFile.ValueExists`。</summary>
    public bool ValueExists(string section, string ident)
        => _sections.TryGetValue(section, out var keys) && keys.ContainsKey(ident);

    /// <summary>Delphi `TIniFile.ReadFloat`（`StrToFloatDef` 语义）。</summary>
    public double ReadFloat(string section, string ident, double defaultValue)
        => DelphiRTL.StrToFloatDef(ReadString(section, ident, ""), defaultValue);

    /// <summary>Delphi `TIniFile.WriteString`（UpdateFile=True）：**空值直接跳过**；写入即落盘。</summary>
    public void WriteString(string section, string ident, string value)
    {
        if (string.IsNullOrEmpty(value)) return; // 原文如此：TIniFile.WriteString 对空串不写入
        EnsureSection(section);
        if (!_sections[section].ContainsKey(ident)) _keyOrder[section].Add(ident);
        _sections[section][ident] = value;
        Save();
    }

    public void WriteInteger(string section, string ident, int value)
        => WriteString(section, ident, DelphiRTL.IntToStr(value));

    /// <summary>Delphi `TIniFile.WriteBool` → `BoolToStr`：True 落盘为 `-1`、False 为 `0`。</summary>
    public void WriteBool(string section, string ident, bool value)
        => WriteString(section, ident, value ? "-1" : "0");

    /// <summary>Delphi `TIniFile.WriteFloat`（InvariantCulture）。</summary>
    public void WriteFloat(string section, string ident, double value)
        => WriteString(section, ident, FormatFloat(value));

    /// <summary>Delphi `FloatToStr` 默认 15 位有效数字。</summary>
    public static string FormatFloat(double v)
    {
        if (v == Math.Floor(v) && Math.Abs(v) < 1e15)
            return v.ToString("0.0#############", System.Globalization.CultureInfo.InvariantCulture);
        return v.ToString("G15", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>落盘（GBK，CRLF，节尾空行，与 Delphi `TIniFile` 输出一致）。</summary>
    public void Save()
    {
        var sb = new StringBuilder();
        foreach (string sec in _sectionOrder)
        {
            sb.Append('[').Append(sec).Append(']').Append("\r\n");
            foreach (string key in _keyOrder[sec])
                sb.Append(key).Append('=').Append(_sections[sec][key]).Append("\r\n");
            sb.Append("\r\n");
        }
        string? dir = Path.GetDirectoryName(_fileName);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_fileName, sb.ToString(), EncodingInit.GBK);
    }

    public string ToText()
    {
        var sb = new StringBuilder();
        foreach (string sec in _sectionOrder)
        {
            sb.Append('[').Append(sec).Append(']').Append("\r\n");
            foreach (string key in _keyOrder[sec])
                sb.Append(key).Append('=').Append(_sections[sec][key]).Append("\r\n");
            sb.Append("\r\n");
        }
        return sb.ToString();
    }

    /// <summary>对应 `m_xIni.Free`（ConfigManager.pas:92）；写入已即时落盘，无需再写。</summary>
    public void Dispose() { }
}
