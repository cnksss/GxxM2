using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GXX.Core;
using GXX.Core.Rtl;

namespace GXX.SelGate;

/// <summary>
/// SelGate ConfigManager.pas → SelGateConfig.cs
/// 1:1 逐字移植 TConfigMgr（:17-47）：INI 读取（缺键即回写默认值）、
/// LoadConfig（:136-200）、SaveConfig（:202-247，含 nType=0/1/2 三分支与**键写入顺序**）。
///
/// 与 GatewayKit 的差异（必须保留）：
///   GatewayKit.GateService.LoadConfig（GateService.cs:67-74）只读 Gateway/Server/PacketRule 四个键；
///   SelGate 原版读取的是 Strings/Integer/Switch/Method/SelGates_iocp（+ SelGate 兼容段），
///   键名带 "2" 后缀（ClientTimeOutTime2 / NomClientPacketSize2 / MaxClientPacketCount2），
///   且**读取时会把缺省值立即写回 INI**（ReadString/ReadInteger/ReadBool，:91-125）。
/// </summary>
public class CConfigMgr
{
    /// <summary>ConfigManager.pas:22 m_xGameGateList: array[1..MAX_SERVER_COUNT]；
    /// MAX_SERVER_COUNT = 32（AcceptExWorkedThread.pas:26）。</summary>
    public const int MAX_SERVER_COUNT = 32;

    /// <summary>ConfigManager.pas:10-14 TGameGateList = record。</summary>
    public class TGameGateList
    {
        public string sServerAdress = ""; // :11 string[15] → 短串上限 15 字节（移植为普通 string，写入时由 INI 原样落盘）
        public int nServerPort;           // :12
        public int nGatePort;             // :13
    }

    public readonly SelIniFile m_xIni;                       // :18 m_xIni: TIniFile
    public string m_szTitle;                                  // :19

    /// <summary>ConfigManager.pas:20 m_nShowLogLevel。</summary>
    public volatile int m_nShowLogLevel;

    /// <summary>ConfigManager.pas:21 m_nGateCount。</summary>
    public volatile int m_nGateCount;

    /// <summary>ConfigManager.pas:22 m_xGameGateList: array[1..MAX_SERVER_COUNT]；C# 侧下标 1..MAX_SERVER_COUNT（0 号槽弃用）。</summary>
    public readonly TGameGateList[] m_xGameGateList = new TGameGateList[MAX_SERVER_COUNT + 1];

    // ---- ConfigManager.pas:24-29 LongBool 开关 ----
    public volatile bool m_fCheckNewIDOfIP;
    public volatile bool m_fCheckNullSession;
    public volatile bool m_fOverSpeedSendBack;
    public volatile bool m_fDefenceCCPacket;
    public volatile bool m_fKickOverSpeed;
    public volatile bool m_fKickOverPacketSize;

    // ---- ConfigManager.pas:31-35 Integer 参数 ----
    public volatile int m_nCheckNewIDOfIP;
    public volatile int m_nMaxConnectOfIP;
    public volatile int m_nClientTimeOutTime;
    public volatile int m_nNomClientPacketSize;
    public volatile int m_nMaxClientPacketCount;

    /// <summary>ConfigManager.pas:37 m_tBlockIPMethod: TBlockIPMethod（未在构造器赋值 ⇒ 默认第 1 项 mDisconnect=0）。</summary>
    public volatile int m_tBlockIPMethod;

    /// <summary>ConfigManager.pas:54-83 constructor TConfigMgr.Create。</summary>
    public CConfigMgr(string szFileName)
    {
        m_xIni = new SelIniFile(szFileName); // :59
        m_szTitle = "";                      // :60 原文如此：//'角色网关'; //原来是名称是： 角色网关，明显错误
        m_nShowLogLevel = 3;                 // :61

        m_nGateCount = 1;                    // :63
        for (int i = 1; i <= MAX_SERVER_COUNT; i++) // :64 Low..High(m_xGameGateList)
        {
            var g = new TGameGateList();
            g.sServerAdress = "127.0.0.1";   // :66
            g.nServerPort = 5100;            // :67
            g.nGatePort = 7100 + i - 1;      // :68
            m_xGameGateList[i] = g;
        }

        m_fCheckNewIDOfIP = true;        // :71
        m_fCheckNullSession = true;      // :72
        m_fOverSpeedSendBack = false;    // :73
        m_fDefenceCCPacket = false;      // :74
        m_fKickOverSpeed = false;        // :75
        m_fKickOverPacketSize = true;    // :76

        m_nNomClientPacketSize = 700;    // :78
        m_nMaxConnectOfIP = 20;          // :79
        m_nCheckNewIDOfIP = 5;           // :80
        m_nClientTimeOutTime = 180 * 1000; // :81
        m_nMaxClientPacketCount = 20;    // :82

        // :37 m_tBlockIPMethod 未赋值 ⇒ Delphi 序型默认首项 mDisconnect = 0（原文如此）
        m_tBlockIPMethod = (int)TBlockIPMethod.mDisconnect;
    }

    /// <summary>ConfigManager.pas:85-89 destructor：m_xIni.Free。</summary>
    public void Destroy() => m_xIni.Dispose();

    /// <summary>ConfigManager.pas:91-101 ReadString：空串视为缺失并回写 Default。</summary>
    public string ReadString(string Section, string Ident, string Default)
    {
        string Result = Default;                                        // :95
        string szLoadStr = m_xIni.ReadString(Section, Ident, "");       // :96
        if (szLoadStr == "")                                            // :97
            m_xIni.WriteString(Section, Ident, Default);                // :98
        else
            Result = szLoadStr;                                         // :100
        return Result;
    }

    /// <summary>ConfigManager.pas:103-113 ReadInteger：负值视为缺失并回写 Default。</summary>
    public int ReadInteger(string Section, string Ident, int Default)
    {
        int Result = Default;                                    // :107
        int szLoadInt = m_xIni.ReadInteger(Section, Ident, -1);  // :108
        if (szLoadInt < 0)                                       // :109
            m_xIni.WriteInteger(Section, Ident, Default);        // :110
        else
            Result = szLoadInt;                                  // :112
        return Result;
    }

    /// <summary>ConfigManager.pas:115-125 ReadBool：负值视为缺失并回写 Default；否则 != 0。</summary>
    public bool ReadBool(string Section, string Ident, bool Default)
    {
        bool Result = Default;                                   // :119
        int szLoadInt = m_xIni.ReadInteger(Section, Ident, -1);  // :120
        if (szLoadInt < 0)                                       // :121
            m_xIni.WriteBool(Section, Ident, Default);           // :122
        else
            Result = szLoadInt != 0;                             // :124
        return Result;
    }

    /// <summary>
    /// ConfigManager.pas:127-134 ReadFloat。原文两次读取同一键（:130 判定、:133 取值），
    /// C# 侧沿用同一写法以保留"读两次"的语义（首次读默认 0）。
    /// </summary>
    public double ReadFloat(string Section, string Ident, double Default)
    {
        double Result = Default;                                                          // :129
        if (m_xIni.ReadFloat(Section, Ident, 0) < 0.10)                                   // :130
            m_xIni.WriteFloat(Section, Ident, Default);                                   // :131
        else
            Result = m_xIni.ReadFloat(Section, Ident, Default);                           // :133
        return Result;
    }

    /// <summary>ConfigManager.pas:136-200 LoadConfig。</summary>
    public void LoadConfig()
    {
        // String
        m_szTitle = ReadString("Strings", "Title", m_szTitle);                                    // :141

        // Integer
        m_nShowLogLevel = ReadInteger("Integer", "ShowLogLevel", m_nShowLogLevel);                // :144
        m_nClientTimeOutTime = ReadInteger("Integer", "ClientTimeOutTime2", m_nClientTimeOutTime); // :145
        if (m_nClientTimeOutTime < 10 * 1000)                                                      // :146
        {
            m_nClientTimeOutTime = 10 * 1000;                                                      // :148
            m_xIni.WriteInteger("Integer", "ClientTimeOutTime2", m_nClientTimeOutTime);            // :149
        }

        m_nMaxConnectOfIP = ReadInteger("Integer", "MaxConnectOfIP", m_nMaxConnectOfIP);                    // :152
        m_nCheckNewIDOfIP = ReadInteger("Integer", "CheckNewIDOfIP", m_nCheckNewIDOfIP);                    // :153
        m_nNomClientPacketSize = ReadInteger("Integer", "NomClientPacketSize2", m_nNomClientPacketSize);    // :154
        m_nMaxClientPacketCount = ReadInteger("Integer", "MaxClientPacketCount2", m_nMaxClientPacketCount);// :155

        // Boolean
        m_fCheckNewIDOfIP = ReadBool("Switch", "CheckNewIDOfIP", m_fCheckNewIDOfIP);              // :158
        m_fCheckNullSession = ReadBool("Switch", "CheckNullSession", m_fCheckNullSession);        // :159
        m_fOverSpeedSendBack = ReadBool("Switch", "OverSpeedSendBack", m_fOverSpeedSendBack);    // :160
        m_fDefenceCCPacket = ReadBool("Switch", "DefenceCCPacket", m_fDefenceCCPacket);          // :161
        m_fKickOverSpeed = ReadBool("Switch", "KickOverSpeed", m_fKickOverSpeed);                // :162
        m_fKickOverPacketSize = ReadBool("Switch", "KickOverPacketSize", m_fKickOverPacketSize); // :163

        //
        m_tBlockIPMethod = ReadInteger("Method", "BlockIPMethod", m_tBlockIPMethod);             // :166

        m_nGateCount = ReadInteger("SelGates_iocp", "Count", 0);                                 // :168
        if (m_nGateCount > 0)                                                                    // :169
        {
            for (int i = 1; i <= m_nGateCount; i++)                                              // :171
            {
                m_xGameGateList[i].sServerAdress = ReadString("SelGates_iocp", "ServerAddr" + DelphiRTL.IntToStr(i), m_xGameGateList[i].sServerAdress); // :173
                m_xGameGateList[i].nServerPort = ReadInteger("SelGates_iocp", "ServerPort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nServerPort);   // :174
                m_xGameGateList[i].nGatePort = ReadInteger("SelGates_iocp", "GatePort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nGatePort);         // :175
            }

            for (int I = m_nGateCount + 1; I <= MAX_SERVER_COUNT; I++)                            // :178
            {
                m_xGameGateList[I].sServerAdress = m_xGameGateList[1].sServerAdress;              // :180
                m_xGameGateList[I].nServerPort = m_xGameGateList[1].nServerPort;                  // :181
                m_xGameGateList[I].nGatePort = m_xGameGateList[1].nGatePort + I - 1;              // :182
            }
        }
        else
        {
            m_nGateCount = 1;                                                                     // :187

            m_xGameGateList[1].sServerAdress = ReadString("SelGate", "ServerAddr", m_xGameGateList[1].sServerAdress); // :189
            m_xGameGateList[1].nServerPort = ReadInteger("SelGate", "ServerPort", m_xGameGateList[1].nServerPort);    // :190
            m_xGameGateList[1].nGatePort = ReadInteger("SelGate", "GatePort", m_xGameGateList[1].nGatePort);          // :191

            for (int I = 2; I <= MAX_SERVER_COUNT; I++)                                           // :193
            {
                m_xGameGateList[I].sServerAdress = m_xGameGateList[1].sServerAdress;              // :195
                m_xGameGateList[I].nServerPort = m_xGameGateList[1].nServerPort;                  // :196
                m_xGameGateList[I].nGatePort = m_xGameGateList[1].nGatePort + I - 1;              // :197
            }
        }
    }

    /// <summary>
    /// ConfigManager.pas:202-247 SaveConfig。
    /// nType=0：Strings/Title、Integer/ShowLogLevel、SelGates_iocp 段（Count + 每网关三键，**顺序固定**）。
    /// nType=1：Integer 五键 → Switch 六键 → Method/BlockIPMethod（PacketRuleConfig.btnSaveClick:1）。
    /// nType=2：空分支（原文如此，:241-244 什么都不做）。
    /// </summary>
    public void SaveConfig(int nType)
    {
        switch (nType)
        {
            case 0:
                m_xIni.WriteString("Strings", "Title", m_szTitle);                         // :211
                m_xIni.WriteInteger("Integer", "ShowLogLevel", m_nShowLogLevel);           // :212

                m_xIni.WriteInteger("SelGates_iocp", "Count", m_nGateCount);               // :214
                for (int i = 1; i <= m_nGateCount; i++)                                    // :215
                {
                    m_xIni.WriteString("SelGates_iocp", "ServerAddr" + DelphiRTL.IntToStr(i), m_xGameGateList[i].sServerAdress); // :217
                    m_xIni.WriteInteger("SelGates_iocp", "ServerPort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nServerPort);  // :218
                    m_xIni.WriteInteger("SelGates_iocp", "GatePort" + DelphiRTL.IntToStr(i), m_xGameGateList[i].nGatePort);      // :219
                }
                break;
            case 1:
                // Integer
                m_xIni.WriteInteger("Integer", "MaxConnectOfIP", m_nMaxConnectOfIP);               // :225
                m_xIni.WriteInteger("Integer", "CheckNewIDOfIP", m_nCheckNewIDOfIP);               // :226
                m_xIni.WriteInteger("Integer", "ClientTimeOutTime2", m_nClientTimeOutTime);        // :227
                m_xIni.WriteInteger("Integer", "NomClientPacketSize2", m_nNomClientPacketSize);    // :228
                m_xIni.WriteInteger("Integer", "MaxClientPacketCount2", m_nMaxClientPacketCount);  // :229

                // Boolean
                m_xIni.WriteBool("Switch", "CheckNewIDOfIP", m_fCheckNewIDOfIP);               // :232
                m_xIni.WriteBool("Switch", "CheckNullSession", m_fCheckNullSession);           // :233
                m_xIni.WriteBool("Switch", "OverSpeedSendBack", m_fOverSpeedSendBack);         // :234
                m_xIni.WriteBool("Switch", "DefenceCCPacket", m_fDefenceCCPacket);             // :235
                m_xIni.WriteBool("Switch", "KickOverSpeed", m_fKickOverSpeed);                 // :236
                m_xIni.WriteBool("Switch", "KickOverPacketSize", m_fKickOverPacketSize);       // :237
                //
                m_xIni.WriteInteger("Method", "BlockIPMethod", m_tBlockIPMethod);              // :239
                break;
            case 2:
                // :241-244 原文如此：空分支
                break;
        }
    }
}

/// <summary>
/// Delphi IniFiles.TIniFile 的**保序**最小复刻（SelGate 只用到 String/Integer/Bool/Float 读写 + 落盘）。
///
/// 关键语义差异（相对 GXX.Core.Util.TFastIniFile，见 FastIniFile.cs:14/61）：
///   1) TFastIniFile 的键字典保持插入序的 Dictionary，重写同一键**不改变位置**；
///      Delphi TIniFile 是文件级重写，同样保持首次出现的位置 —— 两者一致，但 TFastIniFile
///      ReadBool 默认值语义（FastIniFile.cs:94-98，默认 "1"/"0"）与 TIniFile 不同；
///   2) 本类额外提供 TIniFile 的 **空值不写**（WriteString('' ) 跳过）与 Boolean 落盘为
///      -1/0（Delphi BoolToStr 语义），以保证与 TConfigMgr.SaveConfig 的落盘文本逐字一致。
/// </summary>
public class SelIniFile : IDisposable
{
    private readonly string _fileName;
    private readonly List<string> _sectionOrder = new();
    private readonly Dictionary<string, List<string>> _keyOrder = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Dictionary<string, string>> _sections = new(StringComparer.OrdinalIgnoreCase);

    public SelIniFile(string fileName)
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

    /// <summary>Delphi TIniFile.ReadInteger：以 StrToIntDef 语义解析，失败取默认值。</summary>
    public int ReadInteger(string section, string ident, int defaultValue)
        => DelphiRTL.StrToIntDef(ReadString(section, ident, ""), defaultValue);

    /// <summary>Delphi TIniFile.ReadBool：值可解析为整数则 !=0，否则取默认值。</summary>
    public bool ReadBool(string section, string ident, bool defaultValue)
        => ReadInteger(section, ident, defaultValue ? -1 : 0) != 0;

    /// <summary>Delphi TIniFile.ValueExists 语义（键是否存在于该节）。</summary>
    public bool ValueExists(string section, string ident)
        => _sections.TryGetValue(section, out var keys) && keys.ContainsKey(ident);

    /// <summary>Delphi TIniFile.ReadFloat：以 StrToFloatDef 语义解析，失败取默认值。</summary>
    public double ReadFloat(string section, string ident, double defaultValue)
        => DelphiRTL.StrToFloatDef(ReadString(section, ident, ""), defaultValue);

    /// <summary>Delphi TIniFile.WriteString（UpdateFile=True）：空值直接跳过，不落盘；写入 **立即落盘**。</summary>
    public void WriteString(string section, string ident, string value)
    {
        if (string.IsNullOrEmpty(value)) return; // 原文如此：TIniFile.WriteString 对空串不写入
        EnsureSection(section);
        if (!_sections[section].ContainsKey(ident)) _keyOrder[section].Add(ident);
        _sections[section][ident] = value;
        Save();                                  // TIniFile 默认 UpdateFile=True：每次写入即更新文件
    }

    public void WriteInteger(string section, string ident, int value)
        => WriteString(section, ident, DelphiRTL.IntToStr(value));

    /// <summary>Delphi TIniFile.WriteBool → BoolToStr：True 落盘为 -1、False 为 0。</summary>
    public void WriteBool(string section, string ident, bool value)
        => WriteString(section, ident, value ? "-1" : "0");

    /// <summary>Delphi TIniFile.WriteFloat（InvariantCulture 语义）。</summary>
    public void WriteFloat(string section, string ident, double value)
        => WriteString(section, ident, FormatFloat(value));

    /// <summary>Delphi FloatToStr 默认 15 位有效数字，此处按 InvariantCulture 保留小数。</summary>
    public static string FormatFloat(double v)
    {
        if (v == Math.Floor(v) && Math.Abs(v) < 1e15)
            return v.ToString("0.0#############", System.Globalization.CultureInfo.InvariantCulture);
        return v.ToString("G15", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>Flush → 落盘（GBK，CRLF，节间空行，与 Delphi TIniFile 输出一致）。</summary>
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

    /// <summary>对应 Delphi m_xIni.Free（ConfigManager.pas:87）；TIniFile 写入已即时落盘，此处无需再写。</summary>
    public void Dispose() { }
}
