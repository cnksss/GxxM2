// ============================================================================
//  源单元：Source/M2Engine/Forms/uFrmMainGamePets.pas（1,353 行，GBK）
//  同源 DFM：Source/M2Engine/Forms/uFrmMainGamePets.dfm（53,800 字节）
//  本文件：窗体族所依赖的**单元级全局与记录**（原文均属 M2Share.pas）
//    · M2Share.pas:541      TLevelNeedExp = array [1 .. MAXCHANGELEVEL] of LongWord
//    · M2Share.pas:683-725  pTGamePetConfig / TGamePetConfig
//    · M2Share.pas:3985     g_GamePetConfigList: TGList
//    · M2Share.pas:32665-32850 ClearGamePetsConfig / LoadGamePetsConfig /
//                           SaveGamePetsConfig / GetGamePetConfig
//    · M2Share.pas:205      MAXCHANGELEVEL = 1000
//
//  ⚠ 接缝说明：
//    M2Share.pas 属**顺序会话常驻区**（`src/GXX.M2Server/**` 其余部分只读），
//    本车道不修改同类既有文件；这些类型在托管侧尚无定义（已 git grep 确认 0 命中），
//    故在此以**本单元依赖子集**形式落地，待 M2Share.pas 全量批次移植后由集成方合并。
// ============================================================================

using GXX.Core.Util;

namespace GXX.M2Server.Forms.GamePets;

/// <summary>
/// `M2Share.pas:685-725` `TGamePetConfig` 1:1（41 个字段，顺序与原文一致）。
/// 原文为 `record` + `New/Dispose` 堆分配 + `g_GamePetConfigList` 存指针；
/// 托管侧为引用类型（`class`），语义等价：列表元素身份比较（`btnDelPetClick :629`）
/// 在两侧都是**引用相等**。
/// </summary>
public sealed class TGamePetConfig
{
    public string Name = "";
    public int CaptureRate;
    public bool EnabledLevelDifference;
    public int LevelDifference;
    public int HPScale;
    public int ShowFile1;
    public int ShowStart1;
    public int ShowCount1;
    public int ShowTime1;
    public int ShowOffsetX1;
    public int ShowOffsetY1;
    public int ShowFile2;
    public int ShowStart2;
    public int ShowCount2;
    public int ShowTime2;
    public int ShowOffsetX2;
    public int ShowOffsetY2;
    public int AddHP;
    public bool IsAddHPRate;
    public int AddDC1;
    public bool IsAddDC1Rate;
    public int AddDC2;
    public bool IsAddDC2Rate;
    public int AddMC1;
    public bool IsAddMC1Rate;
    public int AddMC2;
    public bool IsAddMC2Rate;
    public int AddSC1;
    public bool IsAddSC1Rate;
    public int AddSC2;
    public bool IsAddSC2Rate;
    public int AddAC1;
    public bool IsAddAC1Rate;
    public int AddAC2;
    public bool IsAddAC2Rate;
    public int AddMAC1;
    public bool IsAddMAC1Rate;
    public int AddMAC2;
    public bool IsAddMAC2Rate;
}

/// <summary>
/// `M2Share.pas:3985` `g_GamePetConfigList: TGList` 的宠物子集（原文 TGList 是
/// `TList`-like 容器；本单元只用到 `Count/Items[i]/Add/Delete/Clear`）。
/// </summary>
/// <remarks>原文 `:33580 g_GamePetConfigList := TGList.Create` —— 该初始化随 M2Share 全量批次接入；
/// 此处提供静态实例供窗体与测试共用。</remarks>
public static class GamePetsState
{
    /// <summary>`g_GamePetConfigList`（Delphi 单元级全局）。</summary>
    public static readonly List<TGamePetConfig> g_GamePetConfigList = new();

    /// <summary>
    /// `M2Share.pas:32665-32676` `ClearGamePetsConfig` 1:1：
    /// 逐个 `Dispose`（托管侧 GC 接管 = 直接清空）。
    /// </summary>
    public static void ClearGamePetsConfig()
    {
        // 原文逐个 Dispose(GamePetConfig) 后 Clear；托管侧引用类型无显式释放。
        g_GamePetConfigList.Clear();
    }

    /// <summary>
    /// `M2Share.pas:32835-32850` `GetGamePetConfig(sPetName): pTGamePetConfig` 1:1：
    /// `SameText` 大小写无关匹配，首个命中即返回，未命中返回 nil。
    /// </summary>
    public static TGamePetConfig? GetGamePetConfig(string sPetName)
    {
        // 原文 Result := nil
        for (int i = 0; i <= g_GamePetConfigList.Count - 1; i++)
        {
            TGamePetConfig GamePetConfig = g_GamePetConfigList[i];
            // 原文 SameText（大小写无关）。GXX.Core.Rtl.DelphiRTL 尚无 SameText，
            // 用 BCL 等价实现（Ordinal 大小写折叠；Delphi 7 ANSI 语义对中文名等价）。
            if (string.Equals(GamePetConfig.Name ?? "", sPetName ?? "", StringComparison.OrdinalIgnoreCase))
            {
                return GamePetConfig;
            }
        }
        return null;
    }

    /// <summary>
    /// `M2Share.pas:32678-32757` `LoadGamePetsConfig: Boolean` 1:1。
    /// 文件名 `g_Config.sEnvirDir + 'GamePetConfigs.txt'`；不存在则返回 False。
    /// </summary>
    public static bool LoadGamePetsConfig()
    {
        // 原文 Result := False; ClearGamePetsConfig;
        ClearGamePetsConfig();
        string sFileName = GXX.M2Server.Engine.M2Config.sEnvirDir + "GamePetConfigs.txt";
        if (!File.Exists(sFileName))
            return false;

        using var IniFile = new TFastIniFile(sFileName);
        int ConfigCount = IniFile.ReadInteger("setup", "count", 0);

        for (int i = 1; i <= ConfigCount; i++)
        {
            string sSection = "pet" + GXX.Core.Rtl.DelphiRTL.IntToStr(i);
            var GamePetConfig = new TGamePetConfig();
            GamePetConfig.Name = IniFile.ReadString(sSection, "name", "");
            if (GamePetConfig.Name.Length > 0)
            {
                GamePetConfig.CaptureRate = IniFile.ReadInteger(sSection, "capturerate", 0);

                GamePetConfig.EnabledLevelDifference = IniFile.ReadBool(sSection, "EnabledLevelDifference", true);
                GamePetConfig.LevelDifference = IniFile.ReadInteger(sSection, "LevelDifference", 2);
                GamePetConfig.HPScale = IniFile.ReadInteger(sSection, "HPScale", 50);

                GamePetConfig.ShowFile1 = IniFile.ReadInteger(sSection, "ShowFile1", 0);
                GamePetConfig.ShowStart1 = IniFile.ReadInteger(sSection, "ShowStart1", 0);
                GamePetConfig.ShowCount1 = IniFile.ReadInteger(sSection, "ShowCount1", 0);
                GamePetConfig.ShowTime1 = IniFile.ReadInteger(sSection, "ShowTime1", 0);
                GamePetConfig.ShowOffsetX1 = IniFile.ReadInteger(sSection, "ShowOffsetX1", 0);
                GamePetConfig.ShowOffsetY1 = IniFile.ReadInteger(sSection, "ShowOffsetY1", 0);

                GamePetConfig.ShowFile2 = IniFile.ReadInteger(sSection, "ShowFile2", 0);
                GamePetConfig.ShowStart2 = IniFile.ReadInteger(sSection, "ShowStart2", 0);
                GamePetConfig.ShowCount2 = IniFile.ReadInteger(sSection, "ShowCount2", 0);
                GamePetConfig.ShowTime2 = IniFile.ReadInteger(sSection, "ShowTime2", 0);
                GamePetConfig.ShowOffsetX2 = IniFile.ReadInteger(sSection, "ShowOffsetX2", 0);
                GamePetConfig.ShowOffsetY2 = IniFile.ReadInteger(sSection, "ShowOffsetY2", 0);

                GamePetConfig.AddHP = IniFile.ReadInteger(sSection, "AddHP", 0);
                GamePetConfig.IsAddHPRate = IniFile.ReadBool(sSection, "IsAddHPRate", false);

                GamePetConfig.AddDC1 = IniFile.ReadInteger(sSection, "AddDC1", 0);
                GamePetConfig.IsAddDC1Rate = IniFile.ReadBool(sSection, "IsAddDC1Rate", false);
                GamePetConfig.AddDC2 = IniFile.ReadInteger(sSection, "AddDC2", 0);
                GamePetConfig.IsAddDC2Rate = IniFile.ReadBool(sSection, "IsAddDC2Rate", false);

                GamePetConfig.AddMC1 = IniFile.ReadInteger(sSection, "AddMC1", 0);
                GamePetConfig.IsAddMC1Rate = IniFile.ReadBool(sSection, "IsAddMC1Rate", false);
                GamePetConfig.AddMC2 = IniFile.ReadInteger(sSection, "AddMC2", 0);
                GamePetConfig.IsAddMC2Rate = IniFile.ReadBool(sSection, "IsAddMC2Rate", false);

                GamePetConfig.AddSC1 = IniFile.ReadInteger(sSection, "AddSC1", 0);
                GamePetConfig.IsAddSC1Rate = IniFile.ReadBool(sSection, "IsAddSC1Rate", false);
                GamePetConfig.AddSC2 = IniFile.ReadInteger(sSection, "AddSC2", 0);
                GamePetConfig.IsAddSC2Rate = IniFile.ReadBool(sSection, "IsAddSC2Rate", false);

                GamePetConfig.AddAC1 = IniFile.ReadInteger(sSection, "AddAC1", 0);
                GamePetConfig.IsAddAC1Rate = IniFile.ReadBool(sSection, "IsAddAC1Rate", false);
                GamePetConfig.AddAC2 = IniFile.ReadInteger(sSection, "AddAC2", 0);
                GamePetConfig.IsAddAC2Rate = IniFile.ReadBool(sSection, "IsAddAC2Rate", false);

                GamePetConfig.AddMAC1 = IniFile.ReadInteger(sSection, "AddMAC1", 0);
                GamePetConfig.IsAddMAC1Rate = IniFile.ReadBool(sSection, "IsAddMAC1Rate", false);
                GamePetConfig.AddMAC2 = IniFile.ReadInteger(sSection, "AddMAC2", 0);
                GamePetConfig.IsAddMAC2Rate = IniFile.ReadBool(sSection, "IsAddMAC2Rate", false);

                // 原文：New(PGamePetConfig); PGamePetConfig^ := GamePetConfig; g_GamePetConfigList.Add
                g_GamePetConfigList.Add(GamePetConfig);
            }
        }

        return true;
    }

    /// <summary>
    /// `M2Share.pas:32759-32833` `SaveGamePetsConfig` 1:1。
    /// ⚠ 原文先 `DeleteFile` 再 `TIniFileEx.Create`（**先删后建**，不是覆盖写）；
    /// 且原文写盘路径**没有** `IniFile.Free`/`UpdateFile` 之外的刷新调用。
    /// 保持逐字：先删文件，再全量重建后写盘。
    /// </summary>
    public static void SaveGamePetsConfig()
    {
        string sFileName = GXX.M2Server.Engine.M2Config.sEnvirDir + "GamePetConfigs.txt";

        if (File.Exists(sFileName))
        {
            File.Delete(sFileName);
        }

        using var IniFile = new TFastIniFile(sFileName);
        IniFile.WriteInteger("setup", "count", g_GamePetConfigList.Count);

        for (int i = 0; i <= g_GamePetConfigList.Count - 1; i++)
        {
            string sSection = "pet" + GXX.Core.Rtl.DelphiRTL.IntToStr(i + 1);

            TGamePetConfig GamePetConfig = g_GamePetConfigList[i];
            IniFile.WriteString(sSection, "name", GamePetConfig.Name);

            IniFile.WriteInteger(sSection, "capturerate", GamePetConfig.CaptureRate);

            IniFile.WriteBool(sSection, "EnabledLevelDifference", GamePetConfig.EnabledLevelDifference);
            IniFile.WriteInteger(sSection, "LevelDifference", GamePetConfig.LevelDifference);
            IniFile.WriteInteger(sSection, "HPScale", GamePetConfig.HPScale);

            IniFile.WriteInteger(sSection, "ShowFile1", GamePetConfig.ShowFile1);
            IniFile.WriteInteger(sSection, "ShowStart1", GamePetConfig.ShowStart1);
            IniFile.WriteInteger(sSection, "ShowCount1", GamePetConfig.ShowCount1);
            IniFile.WriteInteger(sSection, "ShowTime1", GamePetConfig.ShowTime1);
            IniFile.WriteInteger(sSection, "ShowOffsetX1", GamePetConfig.ShowOffsetX1);
            IniFile.WriteInteger(sSection, "ShowOffsetY1", GamePetConfig.ShowOffsetY1);

            IniFile.WriteInteger(sSection, "ShowFile2", GamePetConfig.ShowFile2);
            IniFile.WriteInteger(sSection, "ShowStart2", GamePetConfig.ShowStart2);
            IniFile.WriteInteger(sSection, "ShowCount2", GamePetConfig.ShowCount2);
            IniFile.WriteInteger(sSection, "ShowTime2", GamePetConfig.ShowTime2);
            IniFile.WriteInteger(sSection, "ShowOffsetX2", GamePetConfig.ShowOffsetX2);
            IniFile.WriteInteger(sSection, "ShowOffsetY2", GamePetConfig.ShowOffsetY2);

            IniFile.WriteInteger(sSection, "AddHP", GamePetConfig.AddHP);
            IniFile.WriteBool(sSection, "IsAddHPRate", GamePetConfig.IsAddHPRate);

            IniFile.WriteInteger(sSection, "AddDC1", GamePetConfig.AddDC1);
            IniFile.WriteBool(sSection, "IsAddDC1Rate", GamePetConfig.IsAddDC1Rate);
            IniFile.WriteInteger(sSection, "AddDC2", GamePetConfig.AddDC2);
            IniFile.WriteBool(sSection, "IsAddDC2Rate", GamePetConfig.IsAddDC2Rate);

            IniFile.WriteInteger(sSection, "AddMC1", GamePetConfig.AddMC1);
            IniFile.WriteBool(sSection, "IsAddMC1Rate", GamePetConfig.IsAddMC1Rate);
            IniFile.WriteInteger(sSection, "AddMC2", GamePetConfig.AddMC2);
            IniFile.WriteBool(sSection, "IsAddMC2Rate", GamePetConfig.IsAddMC2Rate);

            IniFile.WriteInteger(sSection, "AddSC1", GamePetConfig.AddSC1);
            IniFile.WriteBool(sSection, "IsAddSC1Rate", GamePetConfig.IsAddSC1Rate);
            IniFile.WriteInteger(sSection, "AddSC2", GamePetConfig.AddSC2);
            IniFile.WriteBool(sSection, "IsAddSC2Rate", GamePetConfig.IsAddSC2Rate);

            IniFile.WriteInteger(sSection, "AddAC1", GamePetConfig.AddAC1);
            IniFile.WriteBool(sSection, "IsAddAC1Rate", GamePetConfig.IsAddAC1Rate);
            IniFile.WriteInteger(sSection, "AddAC2", GamePetConfig.AddAC2);
            IniFile.WriteBool(sSection, "IsAddAC2Rate", GamePetConfig.IsAddAC2Rate);

            IniFile.WriteInteger(sSection, "AddMAC1", GamePetConfig.AddMAC1);
            IniFile.WriteBool(sSection, "IsAddMAC1Rate", GamePetConfig.IsAddMAC1Rate);
            IniFile.WriteInteger(sSection, "AddMAC2", GamePetConfig.AddMAC2);
            IniFile.WriteBool(sSection, "IsAddMAC2Rate", GamePetConfig.IsAddMAC2Rate);
        }

        // 原文 TIniFileEx.Free 会把内存缓存刷盘（对应 TFastIniFile.Save）。
        IniFile.Save();
    }
}
