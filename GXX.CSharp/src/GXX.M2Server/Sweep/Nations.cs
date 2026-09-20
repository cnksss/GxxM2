// 源单元：Source/M2Engine/Nations.pas（357 行统计 / 实际 400 行，MAXNATIONCOUNT=1000）
// 本文件 = TNationManage / TNationInfo 的 1:1 托管移植（namespace GXX.M2Server.Sweep，见文末路径隔离说明）。
//
// 【原文对照】
//   Nations.pas:8-33    声明段（TNationManage 的字段/方法面）
//   Nations.pas:40-65   constructor Create
//   Nations.pas:67-76   destructor Destroy
//   Nations.pas:78-94   GetNationInfo
//   Nations.pas:96-112  GetNationIndex
//   Nations.pas:114-123 GetNationName
//   Nations.pas:125-140 AddMember
//   Nations.pas:142-157 DeleteMember
//   Nations.pas:159-175 IsMember
//   Nations.pas:177-213 SendNationMsg
//   Nations.pas:215-221 Get（property Items 的读方法）
//   Nations.pas:223-262 SaveConfig(btNation)
//   Nations.pas:264-303 SaveConfig()
//   Nations.pas:305-353 LoadConfig
//   Nations.pas:355-397 RenameNationName
//
// 【依赖处理（任务书第 2 条：不顺手移植依赖）】
//   * TPlayObject        —— 见下方 INationsPlayObject 接缝（原文 ObjPlayer.pas 的 TPlayObject 尚未移植国战成员）。
//   * g_Config           —— 直接复用 GXX.M2Server.Engine.M2Config（顺序会话已移植的 g_Config 字段）。
//   * TIniFile / 文件系统 / MainOutMessage / RenameFile —— 见 SweepSeams.cs。
//   * MAXNATIONCOUNT     —— 复用 GXX.Core.Protocol.Grobal2Const.MAXNATIONCOUNT（Grobal2.pas:6477）。
//
// 【路径隔离说明】放在 Sweep/ 子目录只是为了让本车道与顺序会话的 src/GXX.M2Server/** 常驻区物理隔离；
//   实现一律复用 GXX.Core 与 GXX.M2Server 既有代码，不复制第二份。
//
// 【与原文的两处结构性差异（都不改行为，仅托管侧必要）】
//   1. TNationInfo 在原文是 record，但 Nations.pas:15/31 的 Get 返回的是 pTNationInfo（**指针**），
//      且 NpcActionCmd.pas:21627/21649/21736-21786 等外部单元**通过该指针回写**（NationInfo.nGold := …）。
//      托管侧若用 struct 会退化成值拷贝、外部回写丢失 → 因此映射为 **class（引用语义）**，
//      这才是 pTNationInfo 的 1:1 对应物。
//   2. TPlayObject 参数在原文是具体类；这里退化为最小接缝接口 INationsPlayObject（见下）。
//
// 【原文缺陷登记（按原文保留，不修正）】
//   * Nations.pas:198 `if PlayObject.m_boBanNationChat then` —— 只有在**禁止**国家聊天时才发消息，
//     语义可疑（多半应为 `not`）。按任务书第 1 条逐字保留并加注。
//   * Nations.pas:328 `Inc(FCount)` 只在文件存在时递增，且 LoadConfig 可被重复调用 → FCount 会累加
//     （不重置）。逐字保留。
//   * Nations.pas:365 `if GetNationIndex(NewName) > 0 then Exit;` —— 改名成同名也直接 Exit。
//   * Nations.pas:238/280 `RedHomeY` 节的键名与写入名不对称（写入 'nRedHomeX'/'RedHomeY'，
//     读取 'nRedHomeX'/'RedHomeY'），逐字保留。

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Sweep;

/// <summary>
/// 接缝：原文 <c>TPlayObject</c>（ObjPlayer.pas）中本单元用到的**全部成员**。
/// 原文用法只有四类：读 <c>m_btNation</c>、写 <c>m_sNationaName</c>、读 <c>m_boBanNationChat</c>、
/// 调 <c>SendMsg</c>。
///
/// 说明：<c>GXX.M2Server.Engine.TPlayObject</c>（ObjBase.OnlineMsg.cs）目前**没有**
/// <c>m_btNation/m_sNationaName/m_boBanNationChat</c> 三个成员（ObjPlayer.pas 的国战字段尚未移植），
/// 而 Engine/** 是顺序会话常驻区、本车道只读，故这里不扩展那个 partial class
/// （避免 §9.3/§12.8 那类「同名接缝重复」碰撞）。
/// 接缝：待 ObjPlayer.pas 的 TPlayObject 补齐这三个字段后，让它实现本接口（或写 5 行适配器）即可接入。
/// </summary>
public interface INationsPlayObject
{
    /// <summary>原文 <c>TBaseObject.m_btNation: Word</c>（ObjBase.pas:408，注释「国家编号 0没有加入国家」）。</summary>
    ushort m_btNation { get; set; }

    /// <summary>原文 <c>TPlayObject.m_sNationaName: string</c>（ObjPlayer.pas；注意原文拼写少一个 'l'）。</summary>
    string m_sNationaName { get; set; }

    /// <summary>原文 <c>TPlayObject.m_boBanNationChat: Boolean</c>（ObjPlayer.pas）。</summary>
    bool m_boBanNationChat { get; set; }

    /// <summary>
    /// 原文 <c>TCreature.SendMsg(BaseObject, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)</c>。
    /// 托管侧既有 <c>GXX.M2Server.Engine.TCreature.SendMsg(ushort, long, long, long, long, string)</c>
    /// 已是实例方法（原文首参 BaseObject 即接收者，原文此处传的就是 PlayObject 自己 → 等价 this），
    /// 故接缝签名去掉首参（原文 <c>PlayObject.SendMsg(PlayObject, …)</c> 对应 <c>po.SendMsg(…)</c>）。
    /// </summary>
    void SendMsg(ushort wIdent, long wParam, long nParam1, long nParam2, long nParam3, string sMsg);
}

/// <summary>
/// 原文 <c>M2Definition.pas:456-473 TNationInfo</c>（record；此处按 pTNationInfo 的指针语义映射为 class）。
/// 字段名、类型、顺序与原文一致。
/// </summary>
public sealed class TNationInfo
{
    public string sName = "";
    public int nPeoples;
    public string sRedHomeMap = "";
    public int nRedHomeX;
    public int nRedHomeY;
    public string sHomeMap = "";
    public int nHomeX;
    public int nHomeY;
    public string sKingName = "";
    public int nGold;          // 金币
    public ushort wBuilding;   // 建筑能力
    public ushort wArm;        // 军事能力
    public ushort wEconomy;    // 经济能力
    public ushort wPolitics;   // 政治能力
    public ushort wContribution; // 国家贡献
    public byte btMaps;        // 地图数
}

/// <summary>原文 <c>Nations.pas:9 TNationManage</c>：国家/阵营管理（1..MAXNATIONCOUNT = 1..1000）。</summary>
public class TNationManage : IDisposable
{
    private int FCount;

    /// <summary>原文 <c>NationList: array[1..MAXNATIONCOUNT] of TList</c>（成员对象列表）。</summary>
    private readonly List<INationsPlayObject?>[] NationList = new List<INationsPlayObject?>[Grobal2Const.MAXNATIONCOUNT + 1];

    /// <summary>原文 <c>NationConfigList: array[1..MAXNATIONCOUNT] of TNationInfo</c>（下标 1..MAX，[0] 弃用）。</summary>
    private readonly TNationInfo[] NationConfigList = new TNationInfo[Grobal2Const.MAXNATIONCOUNT + 1];

    /// <summary>Delphi <c>CompareText</c> 语义：**只折 ASCII a-z/A-Z**，其余按码位（System.pas 的 UpCase 表）。</summary>
    private static int CompareTextAnsi(string s1, string s2)
    {
        int n = Math.Min(s1.Length, s2.Length);
        for (int i = 0; i < n; i++)
        {
            char c1 = FoldAscii(s1[i]);
            char c2 = FoldAscii(s2[i]);
            if (c1 != c2) return c1 < c2 ? -1 : 1;
        }
        // 前缀相等 → 短者小（与 Delphi CompareText 的逐字节比较一致）
        if (s1.Length == s2.Length) return 0;
        return s1.Length < s2.Length ? -1 : 1;
    }

    private static char FoldAscii(char c)
        => (c >= 'a' && c <= 'z') ? (char)(c - 32) : c;

    /// <summary>原文 <c>Nations.pas:40-65 constructor TNationManage.Create()</c>。</summary>
    public TNationManage()
    {
        FCount = 0;
        for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
        {
            NationList[I] = new List<INationsPlayObject?>();
            NationConfigList[I] = new TNationInfo
            {
                sName = "",
                nPeoples = 0,
                sRedHomeMap = M2Config.sRedHomeMap,
                nRedHomeX = M2Config.nRedHomeX,
                nRedHomeY = M2Config.nRedHomeY,
                sHomeMap = M2Config.sHomeMap,
                nHomeX = M2Config.nHomeX,
                nHomeY = M2Config.nHomeY,
                sKingName = "",
                nGold = 0,
                wBuilding = 0,
                wArm = 0,
                wEconomy = 0,
                wPolitics = 0,
                wContribution = 0,
                btMaps = 0
            };
        }
    }

    /// <summary>原文 <c>Nations.pas:67-76 destructor TNationManage.Destroy</c>（<c>NationList[I].Free</c>）。</summary>
    public void Dispose()
    {
        for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
        {
            // 原文 NationList[I].Free：托管侧 List 由 GC 回收，这里清空以保持「析构后成员列表为空」的可观测行为。
            NationList[I].Clear();
        }
    }

    /// <summary>原文 <c>property Count: Integer read FCount</c>。</summary>
    public int Count => FCount;

    /// <summary>原文 <c>property Items[Index: Integer]: pTNationInfo read Get</c>。</summary>
    public TNationInfo? this[int Index] => Get(Index);

    /// <summary>原文 <c>Nations.pas:78-94 GetNationInfo(const NationName: string): pTNationInfo</c>。</summary>
    public TNationInfo? GetNationInfo(string NationName)
    {
        TNationInfo? Result = null;
        if (NationName != "")
        {
            for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
            {
                if (CompareTextAnsi(NationConfigList[I].sName, NationName) == 0)
                {
                    Result = NationConfigList[I];
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 <c>Nations.pas:96-112 GetNationIndex(const NationName: string): Integer</c>。未找到返回 0。</summary>
    public int GetNationIndex(string NationName)
    {
        int Result = 0;
        if (NationName != "")
        {
            for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
            {
                if (CompareTextAnsi(NationConfigList[I].sName, NationName) == 0)
                {
                    Result = I;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 <c>Nations.pas:114-123 GetNationName(PlayObject: TPlayObject): string</c>（含 AddMember 副作用）。</summary>
    public string GetNationName(INationsPlayObject PlayObject)
    {
        string Result = "";
        if ((PlayObject.m_btNation >= 1) && (PlayObject.m_btNation <= Grobal2Const.MAXNATIONCOUNT) &&
            (NationConfigList[PlayObject.m_btNation].sName != ""))
        {
            AddMember(PlayObject);
            Result = NationConfigList[PlayObject.m_btNation].sName;
        }
        return Result;
    }

    /// <summary>原文 <c>Nations.pas:125-140 AddMember(PlayObject: TPlayObject)</c>（重复加入时 Exit）。</summary>
    public void AddMember(INationsPlayObject PlayObject)
    {
        // 原文 m_btNation 是 **Word**（ObjBase.pas:408），MAXNATIONCOUNT = 1000 → 该上界比较**可假**，
        // 不是恒真（上一轮误按 Byte 建模，导致 1000 号国家成员取不到；已按原文更正）。
        if ((PlayObject.m_btNation >= 1) && (PlayObject.m_btNation <= Grobal2Const.MAXNATIONCOUNT))
        {
            for (int I = 0; I <= NationList[PlayObject.m_btNation].Count - 1; I++)
            {
                if (ReferenceEquals(NationList[PlayObject.m_btNation][I], PlayObject))
                {
                    return; // 原文 Exit
                }
            }
            NationList[PlayObject.m_btNation].Add(PlayObject);
        }
    }

    /// <summary>原文 <c>Nations.pas:142-157 DeleteMember(PlayObject: TPlayObject)</c>（只删第一个匹配）。</summary>
    public void DeleteMember(INationsPlayObject PlayObject)
    {
        if ((PlayObject.m_btNation >= 1) && (PlayObject.m_btNation <= Grobal2Const.MAXNATIONCOUNT))
        {
            for (int I = 0; I <= NationList[PlayObject.m_btNation].Count - 1; I++)
            {
                if (ReferenceEquals(NationList[PlayObject.m_btNation][I], PlayObject))
                {
                    NationList[PlayObject.m_btNation].RemoveAt(I);
                    break;
                }
            }
        }
    }

    /// <summary>原文 <c>Nations.pas:159-175 IsMember(PlayObject: TPlayObject): Boolean</c>。</summary>
    public bool IsMember(INationsPlayObject PlayObject)
    {
        bool Result = false;
        if ((PlayObject.m_btNation >= 1) && (PlayObject.m_btNation <= Grobal2Const.MAXNATIONCOUNT))
        {
            for (int I = 0; I <= NationList[PlayObject.m_btNation].Count - 1; I++)
            {
                if (ReferenceEquals(NationList[PlayObject.m_btNation][I], PlayObject))
                {
                    Result = true;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 <c>Nations.pas:177-213 SendNationMsg(btNation: Word; sMsg: string)</c>。
    /// <para>注意 Nations.pas:198 —— 原文是 <c>if PlayObject.m_boBanNationChat then</c>，
    /// 即**只有被禁言者才收到国家消息**。语义可疑，但按任务书第 1 条逐字保留（见文件头缺陷登记）。</para>
    /// </summary>
    public void SendNationMsg(ushort btNation, string sMsg)
    {
        int nCheckCode = 0;
        try
        {
            if ((btNation >= 1) && (btNation <= Grobal2Const.MAXNATIONCOUNT))
            {
                if (M2Config.boShowPreFixMsg)
                    sMsg = M2Config.sNationMsgPreFix + sMsg;

                for (int I = 0; I <= NationList[btNation].Count - 1; I++)
                {
                    nCheckCode = 3;
                    INationsPlayObject? PlayObject = NationList[btNation][I];
                    if (PlayObject == null)
                        continue; // 原文 Continue

                    nCheckCode = 4;
                    if (PlayObject.m_boBanNationChat) // 原文如此（Nations.pas:198）
                    {
                        nCheckCode = 5;
                        PlayObject.SendMsg(Grobal2Const.RM_NATIONMESSAGE, 0, M2Config.btNationMsgFColor,
                            M2Config.btNationMsgBColor, 0, sMsg);
                        nCheckCode = 6;
                    }
                }
            }
        }
        catch (Exception e)
        {
            SweepSeam.MainOutMessage("[Exceptiion] TNationManage.SendNationMsg CheckCode: " +
                                     DelphiRTL.IntToStr(nCheckCode) + " Msg = " + sMsg); // 原文 'Exceptiion' 拼写如此
            SweepSeam.MainOutMessage(e.Message);
        }
    }

    /// <summary>原文 <c>Nations.pas:215-221 Get(Index: Integer): pTNationInfo</c>（越界或名为空 → nil）。</summary>
    public TNationInfo? Get(int Index)
    {
        if ((Index >= 1) && (Index <= Grobal2Const.MAXNATIONCOUNT) && (NationConfigList[Index].sName != ""))
            return NationConfigList[Index];
        else
            return null;
    }

    /// <summary>原文 <c>Nations.pas:223-262 SaveConfig(btNation: Word)</c>。</summary>
    public void SaveConfig(ushort btNation)
    {
        if ((btNation >= 1) && (btNation <= Grobal2Const.MAXNATIONCOUNT))
        {
            TNationInfo NationInfo = NationConfigList[btNation];
            if (NationInfo.sName != "")
            {
                string sFileName = M2Config.sEnvirDir + "\\Nations\\";
                if (!SweepSeam.DirectoryExists(sFileName))
                    SweepSeam.ForceDirectories(sFileName);

                sFileName = M2Config.sEnvirDir + DelphiRTL.Format("\\Nations\\%s.ini", NationInfo.sName);

                using ISweepIniFile Config = SweepSeam.CreateIniFile(sFileName);

                Config.WriteInteger("Info", "Peoples", NationInfo.nPeoples);
                Config.WriteString("Info", "RedHomeMap", NationInfo.sRedHomeMap);
                Config.WriteInteger("Info", "nRedHomeX", NationInfo.nRedHomeX);
                Config.WriteInteger("Info", "RedHomeY", NationInfo.nRedHomeY);
                Config.WriteString("Info", "HomeMap", NationInfo.sHomeMap);
                Config.WriteInteger("Info", "HomeX", NationInfo.nHomeX);
                Config.WriteInteger("Info", "HomeY", NationInfo.nHomeY);
                Config.WriteString("Info", "King", NationInfo.sKingName);

                Config.WriteInteger("Info", "Gold", NationInfo.nGold);                 // 金币
                Config.WriteInteger("Info", "Building", NationInfo.wBuilding);         // 建筑能力
                Config.WriteInteger("Info", "Arm", NationInfo.wArm);                   // 军事能力
                Config.WriteInteger("Info", "Economy", NationInfo.wEconomy);           // 经济能力
                Config.WriteInteger("Info", "Politics", NationInfo.wPolitics);         // 政治能力
                Config.WriteInteger("Info", "Contribution", NationInfo.wContribution); // 国家贡献
                Config.WriteInteger("Info", "Maps", NationInfo.btMaps);                // 地图数
            }
        }
    }

    /// <summary>原文 <c>Nations.pas:264-303 SaveConfig()</c>（overload，遍历全部国家）。</summary>
    public void SaveConfig()
    {
        for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
        {
            TNationInfo NationInfo = NationConfigList[I];
            if (NationInfo.sName != "")
            {
                string sFileName = M2Config.sEnvirDir + "\\Nations\\";
                if (!SweepSeam.DirectoryExists(sFileName))
                    SweepSeam.ForceDirectories(sFileName);

                sFileName = M2Config.sEnvirDir + DelphiRTL.Format("\\Nations\\%s.ini", NationInfo.sName);

                using ISweepIniFile Config = SweepSeam.CreateIniFile(sFileName);
                Config.WriteInteger("Info", "Peoples", NationInfo.nPeoples);
                Config.WriteString("Info", "RedHomeMap", NationInfo.sRedHomeMap);
                Config.WriteInteger("Info", "nRedHomeX", NationInfo.nRedHomeX);
                Config.WriteInteger("Info", "RedHomeY", NationInfo.nRedHomeY);
                Config.WriteString("Info", "HomeMap", NationInfo.sHomeMap);
                Config.WriteInteger("Info", "HomeX", NationInfo.nHomeX);
                Config.WriteInteger("Info", "HomeY", NationInfo.nHomeY);
                Config.WriteString("Info", "King", NationInfo.sKingName);

                Config.WriteInteger("Info", "Gold", NationInfo.nGold);                 // 金币
                Config.WriteInteger("Info", "Building", NationInfo.wBuilding);         // 建筑能力
                Config.WriteInteger("Info", "Arm", NationInfo.wArm);                   // 军事能力
                Config.WriteInteger("Info", "Economy", NationInfo.wEconomy);           // 经济能力
                Config.WriteInteger("Info", "Politics", NationInfo.wPolitics);         // 政治能力
                Config.WriteInteger("Info", "Contribution", NationInfo.wContribution); // 国家贡献
                Config.WriteInteger("Info", "Maps", NationInfo.btMaps);                // 地图数
            }
        }
    }

    /// <summary>
    /// 原文 <c>Nations.pas:305-353 LoadConfig()</c>。
    /// <para>原文 328 <c>Inc(FCount)</c> 在「国家 ini 文件存在」时递增，且**不重置** FCount ——
    /// 重复调用会累加（逐字保留）。</para>
    /// </summary>
    public void LoadConfig()
    {
        string sFileName = M2Config.sEnvirDir + "\\Nations\\Nations.ini";
        if (SweepSeam.FileExists(sFileName))
        {
            using (ISweepIniFile Config = SweepSeam.CreateIniFile(sFileName))
            {
                for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
                    NationConfigList[I].sName =
                        DelphiRTL.Trim(Config.ReadString("Names", "NationalNames" + DelphiRTL.IntToStr(I), ""));
            }

            for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
            {
                if (NationConfigList[I].sName != "")
                {
                    sFileName = M2Config.sEnvirDir + DelphiRTL.Format("\\Nations\\%s.ini", NationConfigList[I].sName);
                    if (SweepSeam.FileExists(sFileName))
                    {
                        FCount = FCount + 1; // 原文 Inc(FCount)
                        TNationInfo NationInfo = NationConfigList[I];
                        using ISweepIniFile Config = SweepSeam.CreateIniFile(sFileName);
                        NationInfo.nPeoples = Config.ReadInteger("Info", "Peoples", 0);
                        NationInfo.sRedHomeMap = Config.ReadString("Info", "RedHomeMap", M2Config.sRedHomeMap);
                        NationInfo.nRedHomeX = Config.ReadInteger("Info", "nRedHomeX", M2Config.nRedHomeX);
                        NationInfo.nRedHomeY = Config.ReadInteger("Info", "RedHomeY", M2Config.nRedHomeY);
                        NationInfo.sHomeMap = Config.ReadString("Info", "HomeMap", M2Config.sHomeMap);
                        NationInfo.nHomeX = Config.ReadInteger("Info", "HomeX", M2Config.nHomeX);
                        NationInfo.nHomeY = Config.ReadInteger("Info", "HomeY", M2Config.nHomeY);
                        NationInfo.sKingName = Config.ReadString("Info", "King", NationInfo.sKingName);

                        NationInfo.nGold = Config.ReadInteger("Info", "Gold", 0);                 // 金币
                        NationInfo.wBuilding = (ushort)Config.ReadInteger("Info", "Building", 0); // 建筑能力
                        NationInfo.wArm = (ushort)Config.ReadInteger("Info", "Arm", 0);           // 军事能力
                        NationInfo.wEconomy = (ushort)Config.ReadInteger("Info", "Economy", 0);   // 经济能力
                        NationInfo.wPolitics = (ushort)Config.ReadInteger("Info", "Politics", 0); // 政治能力
                        NationInfo.wContribution = (ushort)Config.ReadInteger("Info", "Contribution", 0); // 国家贡献
                        NationInfo.btMaps = (byte)Config.ReadInteger("Info", "Maps", NationInfo.btMaps);  // 地图数
                    }
                }
            }
        }
    }

    /// <summary>
    /// 原文 <c>Nations.pas:355-397 RenameNationName(btNation: Word; NewName: string)</c>。
    /// <para>365「国家名字已经存在」判定：<c>GetNationIndex(NewName) &gt; 0</c> → Exit（改名为同名也会 Exit）。</para>
    /// <para>382 <c>RenameFile</c> 的返回值原文被忽略（逐字保留）。</para>
    /// <para>384 <c>g_NationManage.SaveConfig</c> 原文走全局实例；本类实例即该全局的对应物，故调用 <c>this.SaveConfig()</c>。</para>
    /// </summary>
    public void RenameNationName(ushort btNation, string NewName)
    {
        // 国家名字已经存在
        if (GetNationIndex(NewName) > 0)
            return; // 原文 Exit

        if ((btNation >= 1) && (btNation <= Grobal2Const.MAXNATIONCOUNT))
        {
            TNationInfo NationInfo = NationConfigList[btNation];
            string OldNationName = NationInfo.sName;
            NationInfo.sName = NewName;

            for (int I = 0; I <= NationList[btNation].Count - 1; I++)
            {
                INationsPlayObject Player = NationList[btNation][I]!;
                Player.m_sNationaName = NewName;
            }

            string sOldFile = M2Config.sEnvirDir + DelphiRTL.Format("\\Nations\\%s.ini", OldNationName);
            string sNewFile = M2Config.sEnvirDir + DelphiRTL.Format("\\Nations\\%s.ini", NewName);
            SweepSeam.RenameFile(sOldFile, sNewFile); // 原文忽略返回值

            SaveConfig(); // 原文 g_NationManage.SaveConfig

            string sFileName = M2Config.sEnvirDir + "\\Nations\\Nations.ini";
            if (SweepSeam.FileExists(sFileName))
            {
                using ISweepIniFile Config = SweepSeam.CreateIniFile(sFileName);
                for (int I = 1; I <= Grobal2Const.MAXNATIONCOUNT; I++)
                {
                    Config.WriteString("Names", "NationalNames" + DelphiRTL.IntToStr(I), NationConfigList[I].sName);
                }
            }
        }
    }
}
