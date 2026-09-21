using GXX.Core.Compress;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>M2Share.pas TModuleInfo（客户端模块白/黑名单条目）。</summary>
public class TModuleInfo
{
    public string sFileName = "";
    public string sMD5 = "";
    public bool boMode; // True=远程添加
}

/// <summary>
/// M2Share.pas 客户端模块名单全局与持久化（批次J12：ClientModules.pas 依赖）：
/// g_ModuleList/g_BlackModuleList + Save/LoadClientModules（ModuleList.txt 'MD5|FileName|Mode'，
/// 名单文本 zlib 压缩与 CRC 存 g_ModuleListText/CRC 供下发）。
/// </summary>
public static class ClientModuleState
{
    public static readonly List<TModuleInfo> g_ModuleList = new();
    public static readonly List<TModuleInfo> g_BlackModuleList = new();

    public static int g_ModuleListTextLen;
    public static byte[] g_ModuleListText = Array.Empty<byte>();
    public static uint g_ModuleListTextCRC;

    public static string ModuleListPath => M2Config.sEnvirDir + "ModuleList.txt";

    public static void ResetForTests(string? envirDir)
    {
        g_ModuleList.Clear();
        g_BlackModuleList.Clear();
        g_ModuleListTextLen = 0;
        g_ModuleListText = Array.Empty<byte>();
        g_ModuleListTextCRC = 0;
        if (envirDir != null)
            M2Config.sEnvirDir = envirDir;
    }

    private static string BuildModuleText(List<TModuleInfo> list)
    {
        var text = "";
        foreach (var m in list)
            text += m.sMD5 + "\r\n";
        return text;
    }

    /// <summary>M2Share.pas SaveClientModules 1:1（白名单落盘 + 压缩文本/CRC）。</summary>
    public static void SaveClientModules()
    {
        var text = BuildModuleText(g_ModuleList);
        var saveList = new List<string>();
        foreach (var m in g_ModuleList)
            saveList.Add(m.sMD5 + "|" + m.sFileName + "|" + (m.boMode ? 1 : 0));

        // GBK 一律经 GXX.Core.EncodingInit.GBK 获取：其内部先 Ensure() 注册 CodePagesEncodingProvider，消除加载顺序依赖（CP936 实例等价）。
        var raw = GXX.Core.EncodingInit.GBK.GetBytes(text);
        g_ModuleListTextLen = raw.Length;
        g_ModuleListText = ZlibEx.CompressBuf(raw, raw.Length);
        g_ModuleListTextCRC = CRC32Of(g_ModuleListText);

        try
        {
            Directory.CreateDirectory(M2Config.sEnvirDir);
            File.WriteAllLines(Path.Combine(M2Config.sEnvirDir, "ModuleList.txt"), saveList, GXX.Core.EncodingInit.GBK);
        }
        catch
        {
            // Delphi 空异常吞掉 1:1
        }
    }

    /// <summary>M2Share.pas SaveClientBlackModules（黑名单同格式落盘）。</summary>
    public static void SaveClientBlackModules()
    {
        var path = Path.Combine(M2Config.sEnvirDir, "ModuleBlackList.txt");
        var saveList = new List<string>();
        foreach (var m in g_BlackModuleList)
            saveList.Add(m.sMD5 + "|" + m.sFileName + "|" + (m.boMode ? 1 : 0));
        File.WriteAllLines(path, saveList, GXX.Core.EncodingInit.GBK);
    }

    /// <summary>M2Share.pas LoadClientModules 1:1（清表 → 逐行 MD5|FileName|Mode，MD5 必须满 32 位）。</summary>
    public static void LoadClientModules()
    {
        g_ModuleListTextLen = 0;
        g_ModuleListText = Array.Empty<byte>();
        g_ModuleListTextCRC = 0;

        var fileName = ModuleListPath;
        if (!File.Exists(fileName))
            return;

        g_ModuleList.Clear();

        var lines = File.ReadAllLines(fileName, GXX.Core.EncodingInit.GBK);
        var text = "";
        foreach (var raw in lines)
        {
            var lineText = raw.Trim();
            if (lineText == "" || lineText.StartsWith(';'))
                continue;
            var parts = lineText.Split('|');
            if (parts.Length < 3)
                continue;
            var sMD5 = parts[0];
            var sFileName = parts[1];
            var sMode = parts[2];
            if (sMD5.Length == 32)
            {
                g_ModuleList.Add(new TModuleInfo { sMD5 = sMD5, sFileName = sFileName, boMode = sMode == "1" });
                text += sMD5 + "\r\n";
            }
        }

        var rawBytes = GXX.Core.EncodingInit.GBK.GetBytes(text);
        g_ModuleListTextLen = rawBytes.Length;
        g_ModuleListText = ZlibEx.CompressBuf(rawBytes, rawBytes.Length);
        g_ModuleListTextCRC = CRC32Of(g_ModuleListText);
    }

    /// <summary>M2Share.pas LoadClientBlackModules（黑名单加载）。</summary>
    public static void LoadClientBlackModules()
    {
        var path = Path.Combine(M2Config.sEnvirDir, "ModuleBlackList.txt");
        if (!File.Exists(path))
            return;
        g_BlackModuleList.Clear();
        foreach (var raw in File.ReadAllLines(path, GXX.Core.EncodingInit.GBK))
        {
            var parts = raw.Split('|');
            if (parts.Length >= 3 && parts[0].Length == 32)
                g_BlackModuleList.Add(new TModuleInfo { sMD5 = parts[0], sFileName = parts[1], boMode = parts[2] == "1" });
        }
    }

    private static uint CRC32Of(byte[] data)
    {
        // GXX.Core CheckCrc 等效（BufferCrc）
        uint crc = 0xFFFFFFFF;
        foreach (var b in data)
        {
            crc ^= b;
            for (int k = 0; k < 8; k++)
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
        }
        return ~crc;
    }
}
