using GXX.Core;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J16：M2Share.pas 任务页面标题名单全局与持久化（ConfigMissionNpcPage.pas 依赖）：
/// g_MissionPageCaptionList + GetMissionPageCaption（CompareText 重复检查）+
/// Save/LoadMissionPageCaptionList（MissionPageCaptionList.txt，';' 注释跳过，g_sSendPageCaptionText 下发文本）。
/// </summary>
public static class MissionPageState
{
    /// <summary>g_MissionPageCaptionList: TStringList。</summary>
    public static readonly TStringList g_MissionPageCaptionList = new();

    /// <summary>g_sSendPageCaptionText（保存时同步，供下发）。</summary>
    public static string g_sSendPageCaptionText = "";

    public static string ListPath(string envirDir) => envirDir + "MissionPageCaptionList.txt";

    public static void ResetForTests(string? envirDir)
    {
        g_MissionPageCaptionList.Clear();
        g_sSendPageCaptionText = "";
        if (envirDir != null)
            M2Config.sEnvirDir = envirDir;
    }

    /// <summary>GetMissionPageCaption 1:1（CompareText 忽略大小写重复检查）。</summary>
    public static bool GetMissionPageCaption(string sCaption)
    {
        for (int i = 0; i < g_MissionPageCaptionList.Count; i++)
        {
            if (string.Equals(sCaption, g_MissionPageCaptionList[i], StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>LoadMissionPageCaptionList 1:1（';' 注释与空行跳过）。</summary>
    public static bool LoadMissionPageCaptionList()
    {
        g_MissionPageCaptionList.Clear();
        var sFileName = ListPath(M2Config.sEnvirDir);
        if (!File.Exists(sFileName))
            return false;

        foreach (var raw in File.ReadAllLines(sFileName, EncodingInit.GBK))
        {
            var lineText = raw.Trim();
            if (lineText == "" || lineText.StartsWith(';'))
                continue;
            g_MissionPageCaptionList.Add(lineText);
        }
        return true;
    }

    /// <summary>SaveMissionPageCaptionList 1:1（同步 g_sSendPageCaptionText + 落盘）。</summary>
    public static bool SaveMissionPageCaptionList()
    {
        g_sSendPageCaptionText = g_MissionPageCaptionList.AsEnumerable().Aggregate("", (a, b) => a + b + "\r\n");
        try
        {
            File.WriteAllLines(ListPath(M2Config.sEnvirDir), g_MissionPageCaptionList.AsEnumerable(), EncodingInit.GBK);
        }
        catch
        {
            // Delphi 空 except 吞异常 1:1
        }
        return true;
    }
}

/// <summary>
/// uFrmCombatPowerAddVar.pas 核心函数 1:1：CheckCombatPowerVarSupport
/// （首字符 D/M/N/U/J 或前缀 N$ 的战力变量支持检查）。
/// </summary>
public static class CombatPowerAddVar
{
    public static bool CheckCombatPowerVarSupport(string varName)
    {
        if (varName.Length == 0)
            return false;
        varName = varName.ToUpperInvariant();
        return "DMNUJ".Contains(varName[0])
            || (varName.Length > 2 && varName.StartsWith("N$"));
    }
}
