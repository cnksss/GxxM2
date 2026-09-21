using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：DBShare.pas 中本车道各单元依赖的**最小**部分（未整体移植 DBShare）
//   · TRouteInfo / pTRouteInfo              DBShare.pas:26-39
//   · g_RouteInfo                            DBShare.pas:150（array[0..19]）
//   · 配置/文件全局（Setting/Ranking 用）     DBShare.pas:112-242
//   · GateRouteIP                            DBShare.pas:705-729
//   · SaveServerInfo / LoadServerInfo         DBShare.pas:427-490 / 492-620
//   · GetRankingList                          DBShare.pas:362-386
//   · TRoleRankData / TRoleRankList          RoleDB.pas:39-50 / 100-114 / 454-503
//   · TRankingEngine.RefRanking 的调用点      uFrmMain.pas:1186-1204（线程体留作接缝）
//   待 DBShare.pas / RoleDB.pas 正式移植后合并（届时本文件的类型定义迁移、Seam 类删除）。
// ============================================================================================

/// <summary>RoleDB.pas:39-50 `TRoleRankData`（带 variant 部分 → C# 两字段并存）。</summary>
public class TRoleRankData
{
    public int RankIndex;
    public string HumanName = "";
    public string HeroName = "";

    /// <summary>case 0: (Level: LongWord)。</summary>
    public uint Level;

    /// <summary>case 1: (MasterCount: LongWord)。</summary>
    public uint MasterCount;
}

/// <summary>RoleDB.pas:100-114 / 454-503 `TRoleRankList`（TList 包装）。</summary>
public class TRoleRankList
{
    private readonly List<TRoleRankData> FList = new List<TRoleRankData>();

    public int Count => FList.Count;

    /// <summary>RoleDB.pas:491-497 `GetItems`：越界返回 nil。</summary>
    public TRoleRankData Items(int Index)
        => (Index >= 0 && Index < FList.Count) ? FList[Index] : null;

    /// <summary>RoleDB.pas:468-473 `Add`。</summary>
    public TRoleRankData Add(TRoleRankData RoleRankData)
    {
        FList.Add(RoleRankData);
        return RoleRankData;
    }

    /// <summary>RoleDB.pas:475-484 `Clear`。</summary>
    public void Clear() => FList.Clear();

    /// <summary>RoleDB.pas:499-503 `SetCapacity`（托管侧无对应，保留签名）。</summary>
    public void SetCapacity(int Value) => FList.Capacity = Value;
}

/// <summary>
/// DBShare.pas:26-39 `TRouteInfo` / `pTRouteInfo`。
/// Delphi 为 record 但全工程只通过指针访问（`RouteInfo := @g_RouteInfo[I]`，RouteManage/RouteEdit 还会就地改写），
/// 故 C# 侧用 class 精确保持"同一实例被多处共享改写"的引用语义。
/// </summary>
public class TRouteInfo
{
    public int nGateCount;
    public TShortString15 sSelGateIP;                        // string[15]
    public TShortString15[] sGameGateIP = NewShortArray();   // array[0..7] of string[15]
    public int[] nGameGatePort = new int[8];                 // array[0..7] of Integer
    public int[] nGameGateDBPort = new int[8];               // array[0..7] of Integer
    public uint[] dwGameGateConnectTick = new uint[8];       // array[0..7] of LongWord

    /// <summary>DBShare.pas:35 主网关有几个不能连接时，分配备用网关 chongchong 2015-07-24。</summary>
    public int GameGateDisconnectCount;

    public byte EnabledRunGate2List;                         // Boolean → byte
    public TRunGateList RunGate2List = new TRunGateList();

    private static TShortString15[] NewShortArray()
    {
        var a = new TShortString15[8];
        for (int i = 0; i < a.Length; i++) a[i] = new TShortString15("");
        return a;
    }

    /// <summary>DBShare.pas:506-516 的字段初值（LoadServerInfo 起始清零；测试可直接调用）。</summary>
    public void ResetToDefault()
    {
        nGateCount = 0;
        sSelGateIP = "";
        for (int i = 0; i < 8; i++)
        {
            sGameGateIP[i] = "";
            nGameGatePort[i] = 0;
            nGameGateDBPort[i] = 0;
            dwGameGateConnectTick[i] = 0;
        }
        GameGateDisconnectCount = 1;
        EnabledRunGate2List = 0;
        RunGate2List.Clear();
    }
}

/// <summary>
/// DBShare.pas 的 unit 级全局（§3.3 规则：unit var → public static class）。
/// 仅收录本车道用到的成员，其余待 DBShare 正式移植。
/// </summary>
public static class DBShareSeam
{
    // ---------------- 路径/文件名（DBShare.pas:112-138） ----------------

    public static string g_sFilePath = "";
    public static string g_sConfFileName = @".\Dbsrc.ini";
    public static string g_sGateConfFileName = @".\!ServerInfo.txt";
    public static string g_sGateListFileName = @".\!GateList.ini";

    // ---------------- 路由（DBShare.pas:150） ----------------

    public static readonly TRouteInfo[] g_RouteInfo = CreateRouteInfoArray(20);

    private static TRouteInfo[] CreateRouteInfoArray(int n)
    {
        var a = new TRouteInfo[n];
        for (int i = 0; i < n; i++) a[i] = new TRouteInfo();
        return a;
    }

    // ---------------- 基本设置（DBShare.pas:217-230） ----------------

    public static byte g_boCanCreateHuman = 1;          // True  允许建立新人物
    public static byte g_boCanDeleteHuman = 1;          // True  允许删除人物
    public static byte g_boCanGetBackDeleteHuman = 1;   // True  允许找回删除的人物
    public static int g_nCanDeleteHumanLowLevel = 45;   // 以上级别不允许被删除
    public static byte g_boForbidNumberName = 0;        // False 禁止建立包含数字的人物名
    public static byte g_boForbidLetterName = 0;        // False 禁止建立全英文人物名
    public static byte g_boDenyChrName = 0;             // False 允许特殊字符创建人物
    public static byte g_boUseActiveRunGage = 0;        // False
    public static byte g_boShowBlockIPLog = 0;          // False
    public static int g_nCreateChrNameCount = 20;

    public static TStringList g_FilterNewHumanNameTextList = new TStringList();
    public static TStringList g_FilterRankingNameTextList = new TStringList();

    /// <summary>
    /// DBShare.pas:145 `g_DenyChrNameList: TStringList;`（**2026 第 2 轮新增**）：
    /// 人物名禁用名单，由 <see cref="DBShare.LoadChrNameList"/>（原文 :403-425）装载、
    /// <see cref="DBShare.CheckDenyChrName"/>（原文 :1043-1056）消费。
    /// 放在本文件是为了与它的兄弟 `g_FilterNewHumanNameTextList`（:144）同处；
    /// 将来 DBShare.pas 整体移植时一并迁走。
    /// </summary>
    public static TStringList g_DenyChrNameList = new TStringList();

    // ---------------- 排行榜（DBShare.pas:176-215） ----------------

    public static int g_nRankingMinLevel = 20;
    public static int g_nRankingMaxLevel = 500;
    public static int g_nRankingCount = 100;

    public static byte g_boAutoRefRanking = 1;          // True
    public static int g_nAutoRefRankingType = 0;
    public static uint g_dwAutoRefRankingTick;

    public static int g_nRefRankingHour1 = 0;
    public static int g_nRefRankingHour2 = 0;
    public static int g_nRefRankingMinute1 = 5;
    public static int g_nRefRankingMinute2 = 5;

    public static double g_TodayDate = 0;               // TDate

    public static byte g_boRefRanking = 0;              // False
    public static byte g_boCanRanking = 1;              // True
    public static uint g_RefRankingTick = 0;

    public static readonly TRoleRankList g_HumanRankList = new TRoleRankList();
    public static readonly TRoleRankList g_WarriorRankList = new TRoleRankList();
    public static readonly TRoleRankList g_WizardRankList = new TRoleRankList();
    public static readonly TRoleRankList g_TaoistRankList = new TRoleRankList();
    public static readonly TRoleRankList g_MasterRankList = new TRoleRankList();
    public static readonly TRoleRankList g_HeroRankList = new TRoleRankList();
    public static readonly TRoleRankList g_HeroWarriorRankList = new TRoleRankList();
    public static readonly TRoleRankList g_HeroWizardRankList = new TRoleRankList();
    public static readonly TRoleRankList g_HeroTaoistRankList = new TRoleRankList();

    /// <summary>
    /// 接缝：uFrmMain.pas:1186-1204 `TRankingEngine.RefRanking` 的引擎体（依赖 g_RoleDB.HumanDB/HeroDB，
    /// 属角色 DB 车道）。Ranking 窗体只负责触发与显示，这里用委托留出调用点。
    /// </summary>
    public static Action RankingEngine_RefRanking = () => { };

    // ---------------- DBShare.pas:362-386 GetRankingList ----------------

    public static TRoleRankList GetRankingList(int nTablePage, int nPageType)
    {
        TRoleRankList Result = null;
        switch (nTablePage)
        {
            case 0:
                switch (nPageType)
                {
                    case 0: Result = g_HumanRankList; break;
                    case 1: Result = g_WarriorRankList; break;
                    case 2: Result = g_WizardRankList; break;
                    case 3: Result = g_TaoistRankList; break;
                }
                break;
            case 1:
                switch (nPageType)
                {
                    case 0: Result = g_HeroRankList; break;
                    case 1: Result = g_HeroWarriorRankList; break;
                    case 2: Result = g_HeroWizardRankList; break;
                    case 3: Result = g_HeroTaoistRankList; break;
                }
                break;
            case 2: Result = g_MasterRankList; break;
        }
        return Result;
    }

    // ---------------- DBShare.pas:705-729 GateRouteIP ----------------

    /// <summary>DBShare.pas:706-713 内嵌 `GetRoute`：随机挑一条主游戏网关。</summary>
    private static string GetRoute(TRouteInfo RouteInfo, out int nGatePort)
    {
        int nGateIndex = DelphiRandom.Random(RouteInfo.nGateCount);
        string Result = RouteInfo.sGameGateIP[nGateIndex];
        nGatePort = RouteInfo.nGameGatePort[nGateIndex];
        return Result;
    }

    /// <summary>
    /// DBShare.pas:705-729 `GateRouteIP(sGateIP; var nPort)`：按角色网关地址查路由并随机取一条游戏网关。
    /// 原文如此：未命中时返回 '' 且 nPort = 0；命中但 nGateCount = 0 时 Random(0) = 0 仍取 [0] 槽位。
    /// </summary>
    public static string GateRouteIP(string sGateIP, out int nPort)
    {
        nPort = 0;
        string Result = "";
        for (int I = 0; I < g_RouteInfo.Length; I++)
        {
            TRouteInfo RouteInfo = g_RouteInfo[I];
            if (RouteInfo.sSelGateIP == sGateIP)
            {
                Result = GetRoute(RouteInfo, out nPort);
                break;
            }
        }
        return Result;
    }

    // ---------------- DBShare.pas:427-490 SaveServerInfo ----------------

    /// <summary>
    /// DBShare.pas:427-490 `SaveServerInfo`：写 !ServerInfo.txt（TStringList）+ !GateList.ini（TIniFile）。
    /// 原文如此：`LoadList.SaveToFile` 在 for 循环**内部**（被写了 20 次，最终内容相同）。
    /// </summary>
    public static void SaveServerInfo()
    {
        int I, J;
        var LoadList = new TStringList();
        try
        {
            for (I = 0; I < g_RouteInfo.Length; I++)
            {
                if (g_RouteInfo[I].nGateCount > 0)
                {
                    string S = g_RouteInfo[I].sSelGateIP + "\t";

                    for (J = 0; J <= g_RouteInfo[I].nGateCount - 1; J++)
                    {
                        S = S + g_RouteInfo[I].sGameGateIP[J] + "\t" + DelphiRTL.IntToStr(g_RouteInfo[I].nGameGatePort[J]) + "\t";
                    }

                    LoadList.Add(S);
                }
                LoadList.SaveToFile(g_sGateConfFileName);
            }
        }
        finally
        {
            // LoadList.Free
        }

        var Conf = new TIniFile(g_sGateListFileName);
        try
        {
            for (I = 0; I < g_RouteInfo.Length; I++)
            {
                Conf.EraseSection("GateDBPort" + DelphiRTL.IntToStr(I));

                if (g_RouteInfo[I].nGateCount == 0) continue;

                for (J = 0; J <= g_RouteInfo[I].nGateCount - 1; J++)
                {
                    Conf.WriteInteger("GateDBPort" + DelphiRTL.IntToStr(I), DelphiRTL.IntToStr(J + 1), g_RouteInfo[I].nGameGateDBPort[J]);
                }

                Conf.WriteBool("setup", "enable" + DelphiRTL.IntToStr(I), TBool.ToBool(g_RouteInfo[I].EnabledRunGate2List));
                Conf.WriteInteger("setup", "count" + DelphiRTL.IntToStr(I), g_RouteInfo[I].GameGateDisconnectCount);

                Conf.EraseSection("list" + DelphiRTL.IntToStr(I));
                TRunGateList RunGateList = g_RouteInfo[I].RunGate2List;
                for (J = 0; J <= RunGateList.Count - 1; J++)
                {
                    Conf.WriteString("list" + DelphiRTL.IntToStr(I), DelphiRTL.IntToStr(J),
                      DelphiRTL.IntToStr((int)RunGateList.Items(J).Enabled) + "\t" +
                      RunGateList.Items(J).IP + "\t" +
                      DelphiRTL.IntToStr(RunGateList.Items(J).Port) + "\t" +
                      DelphiRTL.IntToStr(RunGateList.Items(J).Level) + "\t" +
                      DelphiRTL.IntToStr(RunGateList.Items(J).DBPort)
                      );
                }
            }
        }
        finally
        {
            Conf.Dispose();
        }
    }

    // ---------------- DBShare.pas:492-620 LoadServerInfo ----------------

    /// <summary>
    /// DBShare.pas:492-620 `LoadServerInfo`：清零路由 → 读 !ServerInfo.txt → 读 !GateList.ini。
    /// 原文如此（注意两处刻意的奇怪行为，已在测试中锁定）：
    ///   ① 路由行数超过 20（g_RouteInfo 容量）时不设边界检查 → Delphi 越界写(UB)，C# 抛 IndexOutOfRangeException；
    ///   ② `Conf.ReadSectionValues` 不清空 LoadList，导致 list0 的条目在 list1/list2… 被重复解析并再次 Add。
    /// </summary>
    public static void LoadServerInfo()
    {
        int I, J;
        TStringList LoadList;
        int nRouteIdx, nGateIdx;
        string sLineText, sGameGate;
        string sSelGateIPaddr = "", sGameGateIPaddr = "", sGameGatePort = "";
        TIniFile Conf;

        string S1 = "", S2 = "", S3 = "", S4 = "", S5 = "";
        int nPort;
        TRunGateList RunGateList;

        for (I = 0; I < g_RouteInfo.Length; I++)
        {
            g_RouteInfo[I].ResetToDefault();
        }

        if (!File.Exists(g_sGateConfFileName))
        {
            LoadList = new TStringList();
            LoadList.Add("127.0.0.1 127.0.0.1 7200");
            try
            {
                LoadList.SaveToFile(g_sGateConfFileName);
            }
            catch
            {
            }
            // LoadList.Free
        }

        if (File.Exists(g_sGateConfFileName))
        {
            LoadList = new TStringList();
            try
            {
                LoadList.LoadFromFile(g_sGateConfFileName);
            }
            catch
            {
            }
            nRouteIdx = 0;
            for (I = 0; I <= LoadList.Count - 1; I++)
            {
                sLineText = DelphiRTL.Trim(LoadList[I]);
                if ((sLineText != "") && (sLineText[0] != ';'))
                {
                    sGameGate = HUtil32Seam.GetValidStr3(sLineText, ref sSelGateIPaddr, new[] { ' ', '\t' });
                    if ((sGameGate == "") || (sSelGateIPaddr == "")) continue;
                    g_RouteInfo[nRouteIdx].sSelGateIP = DelphiRTL.Trim(sSelGateIPaddr);
                    g_RouteInfo[nRouteIdx].nGateCount = 0;
                    nGateIdx = 0;
                    while (sGameGate != "")
                    {
                        sGameGate = HUtil32Seam.GetValidStr3(sGameGate, ref sGameGateIPaddr, new[] { ' ', '\t' });
                        sGameGate = HUtil32Seam.GetValidStr3(sGameGate, ref sGameGatePort, new[] { ' ', '\t' });
                        g_RouteInfo[nRouteIdx].sGameGateIP[nGateIdx] = DelphiRTL.Trim(sGameGateIPaddr);
                        g_RouteInfo[nRouteIdx].nGameGatePort[nGateIdx] = DelphiRTL.StrToIntDef(sGameGatePort, 0);
                        nGateIdx++;
                    }
                    g_RouteInfo[nRouteIdx].nGateCount = nGateIdx;
                    nRouteIdx++;
                }
            }
            // LoadList.Free
        }

        if (File.Exists(g_sGateListFileName))
        {
            LoadList = new TStringList();
            try
            {
                Conf = new TIniFile(g_sGateListFileName);
                try
                {
                    for (I = 0; I < g_RouteInfo.Length; I++)
                    {
                        for (J = 0; J <= g_RouteInfo[I].nGateCount - 1; J++)
                        {
                            g_RouteInfo[I].nGameGateDBPort[J] = Conf.ReadInteger("GateDBPort" + DelphiRTL.IntToStr(I), DelphiRTL.IntToStr(J + 1), 0);
                        }

                        RunGateList = g_RouteInfo[I].RunGate2List;
                        g_RouteInfo[I].EnabledRunGate2List = TBool.ToByte(Conf.ReadBool("setup", "enable" + DelphiRTL.IntToStr(I), false));
                        g_RouteInfo[I].GameGateDisconnectCount = Conf.ReadInteger("setup", "count" + DelphiRTL.IntToStr(I), 1);
                        if ((g_RouteInfo[I].GameGateDisconnectCount < 1) || (g_RouteInfo[I].GameGateDisconnectCount > 8))
                            g_RouteInfo[I].GameGateDisconnectCount = 1;

                        Conf.ReadSectionValues("list" + DelphiRTL.IntToStr(I), LoadList);

                        for (J = 0; J <= LoadList.Count - 1; J++)
                        {
                            sLineText = LoadList[J];
                            if ((sLineText.Length == 0) || (sLineText[0] == ';')) continue;

                            sLineText = TStringsHelper.ValueFromIndex(sLineText);

                            sLineText = HUtil32Seam.GetValidStr3(sLineText, ref S1, new[] { ' ', '\t' });
                            sLineText = HUtil32Seam.GetValidStr3(sLineText, ref S2, new[] { ' ', '\t' });
                            sLineText = HUtil32Seam.GetValidStr3(sLineText, ref S3, new[] { ' ', '\t' });
                            sLineText = HUtil32Seam.GetValidStr3(sLineText, ref S4, new[] { ' ', '\t' });
                            sLineText = HUtil32Seam.GetValidStr3(sLineText, ref S5, new[] { ' ', '\t' });

                            nPort = DelphiRTL.StrToIntDef(S3, 0);
                            if (HUtil32.IsIPaddr(S2) && (nPort > 0) && (nPort <= 65535))
                            {
                                RunGateList.Add(TBool.ToByte(DelphiRTL.StrToIntDef(S1, 0) > 0), S2, (ushort)nPort, (ushort)DelphiRTL.StrToIntDef(S5, 0), DelphiRTL.StrToIntDef(S4, 0));
                            }
                        }

                        RunGateList.DoSort();
                    }
                }
                finally
                {
                    Conf.Dispose();
                }
            }
            finally
            {
                // LoadList.Free
            }
        }
    }
}

/// <summary>Delphi `TStrings` 的辅助语义（GXX.Core.TStringList 未覆盖部分）。</summary>
public static class TStringsHelper
{
    /// <summary>
    /// Delphi `TStrings.ValueFromIndex`：取首个 '=' 之后的内容；无 '=' 时返回 ''。
    /// </summary>
    public static string ValueFromIndex(string line)
    {
        int p = line.IndexOf('=');
        return p < 0 ? "" : line.Substring(p + 1);
    }

    /// <summary>
    /// Delphi `TStrings.SetTextStr`：按 #13#10 / #13 / #10 拆行覆盖写入；
    /// 结尾换行不产生额外的空行（`while S &lt;&gt; ''` 语义）。
    /// </summary>
    public static void SetText(TStringList list, string value)
    {
        list.Clear();
        if (string.IsNullOrEmpty(value)) return;
        int start = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '\n' || value[i] == '\r')
            {
                list.Add(value.Substring(start, i - start));
                if (value[i] == '\r' && i + 1 < value.Length && value[i + 1] == '\n') i++;
                start = i + 1;
            }
        }
        if (start < value.Length) list.Add(value.Substring(start));
    }
}
