using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.DBServer;

namespace GXX.DBServer.Tests;

/// <summary>记录 MessageBox / MessageDlg / ShowMessage 的调用（绝不弹真实窗体）。</summary>
public sealed class UiRecorder : IDisposable
{
    public readonly List<(string Text, string Caption, uint Flags)> MessageBoxes = new();
    public readonly List<(string Msg, TMsgDlgType Type, TMsgDlgButtons Buttons)> MessageDlgs = new();
    public readonly List<string> ShowMessages = new();

    /// <summary>MessageDlg 的返回值（默认 mrYes，即"确认"）。</summary>
    public int MessageDlgResult = TModalResult.mrYes;

    public UiRecorder()
    {
        UiSeam.MessageBox = (text, caption, flags) =>
        {
            MessageBoxes.Add((text, caption, flags));
            return TMsgBox.IDOK;
        };
        UiSeam.MessageDlg = (msg, type, buttons, help) =>
        {
            MessageDlgs.Add((msg, type, buttons));
            return MessageDlgResult;
        };
        UiSeam.ShowMessage = msg => ShowMessages.Add(msg);
    }

    public void Dispose()
    {
        // 复位成永不阻塞的桩（真实 MessageBox 只在没有记录器时才应出现）
        UiSeam.MessageBox = (_, _, _) => TMsgBox.IDOK;
        UiSeam.MessageDlg = (_, _, _, _) => TModalResult.mrNo;
        UiSeam.ShowMessage = _ => { };
    }
}

/// <summary>内存 ListView 实现（IListViewSink 注入用）。</summary>
public sealed class MemoryListViewSink : IListViewSink
{
    public readonly List<(string Caption, object Tag, string[] SubItems)> Rows = new();

    public int Count => Rows.Count;

    public void Clear() => Rows.Clear();

    public void AddRow(string caption, object tag, params string[] subItems)
        => Rows.Add((caption, tag, subItems));

    public object GetTag(int index) => Rows[index].Tag;
}

/// <summary>内存人物库（IHumDataDB 注入用，替代 HumDB/SqliteRoleDB）。</summary>
public sealed class MemoryHumDataDB : IHumDataDB
{
    public readonly Dictionary<int, THumDataInfo> Records = new();
    public readonly List<int> Deleted = new();
    public readonly List<int> Updated = new();
    public readonly Dictionary<int, string> FoundByName = new();

    public bool OpenResult = true;
    public int OpenCount;
    public int CloseCount;
    public int RebuildCount;

    public int Count => Records.Count;

    public bool Open()
    {
        OpenCount++;
        return OpenResult;
    }

    public void Close() => CloseCount++;

    public void Find(string sChrName, TStringList items)
    {
        foreach (var kv in FoundByName)
        {
            items.AddObject(kv.Value, kv.Key);
        }
    }

    public void Delete(int nIndex)
    {
        Deleted.Add(nIndex);
        Records.Remove(nIndex);
    }

    public void Rebuild() => RebuildCount++;

    public int Get(int index, out THumDataInfo record)
    {
        if (!Records.TryGetValue(index, out record)) { record = null; return -1; }
        return index;
    }

    public void Update(int index, THumDataInfo record)
    {
        Updated.Add(index);
        Records[index] = record;
    }
}

/// <summary>带临时工作目录的测试基类：把全部落盘路径指到临时目录。</summary>
public abstract class TempDirTest : IDisposable
{
    protected readonly string Dir;

    protected TempDirTest()
    {
        Dir = Path.Combine(Path.GetTempPath(), "gxx-dbserver-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Dir);
        TestReset.All();
        DBShareSeam.g_sFilePath = Dir + Path.DirectorySeparatorChar;
        DBShareSeam.g_sConfFileName = Path.Combine(Dir, "Dbsrc.ini");
        DBShareSeam.g_sGateConfFileName = Path.Combine(Dir, "!ServerInfo.txt");
        DBShareSeam.g_sGateListFileName = Path.Combine(Dir, "!GateList.ini");
        FdbExploreSeam.ClearItemLogFile = Path.Combine(Dir, "ClearItemLog.txt");
    }

    public void Dispose()
    {
        try { Directory.Delete(Dir, true); } catch { }
    }

    protected string Path2(string name) => Path.Combine(Dir, name);
}

/// <summary>把 Delphi unit 级全局量复位到 DBShare.pas / FDBexpl.pas 的声明初值。</summary>
public static class TestReset
{
    public static void All()
    {
        for (int i = 0; i < DBShareSeam.g_RouteInfo.Length; i++) DBShareSeam.g_RouteInfo[i].ResetToDefault();

        DBShareSeam.g_sFilePath = "";
        DBShareSeam.g_sConfFileName = @".\Dbsrc.ini";
        DBShareSeam.g_sGateConfFileName = @".\!ServerInfo.txt";
        DBShareSeam.g_sGateListFileName = @".\!GateList.ini";

        DBShareSeam.g_boCanCreateHuman = 1;
        DBShareSeam.g_boCanDeleteHuman = 1;
        DBShareSeam.g_boCanGetBackDeleteHuman = 1;
        DBShareSeam.g_nCanDeleteHumanLowLevel = 45;
        DBShareSeam.g_boForbidNumberName = 0;
        DBShareSeam.g_boForbidLetterName = 0;
        DBShareSeam.g_boDenyChrName = 0;
        DBShareSeam.g_boUseActiveRunGage = 0;
        DBShareSeam.g_boShowBlockIPLog = 0;
        DBShareSeam.g_nCreateChrNameCount = 20;
        DBShareSeam.g_FilterNewHumanNameTextList = new TStringList();
        DBShareSeam.g_FilterRankingNameTextList = new TStringList();

        DBShareSeam.g_nRankingMinLevel = 20;
        DBShareSeam.g_nRankingMaxLevel = 500;
        DBShareSeam.g_nRankingCount = 100;
        DBShareSeam.g_boAutoRefRanking = 1;
        DBShareSeam.g_nAutoRefRankingType = 0;
        DBShareSeam.g_dwAutoRefRankingTick = 0;
        DBShareSeam.g_nRefRankingHour1 = 0;
        DBShareSeam.g_nRefRankingHour2 = 0;
        DBShareSeam.g_nRefRankingMinute1 = 5;
        DBShareSeam.g_nRefRankingMinute2 = 5;
        DBShareSeam.g_TodayDate = 0;
        DBShareSeam.g_boRefRanking = 0;
        DBShareSeam.g_boCanRanking = 1;
        DBShareSeam.g_RefRankingTick = 0;

        DBShareSeam.g_HumanRankList.Clear();
        DBShareSeam.g_WarriorRankList.Clear();
        DBShareSeam.g_WizardRankList.Clear();
        DBShareSeam.g_TaoistRankList.Clear();
        DBShareSeam.g_MasterRankList.Clear();
        DBShareSeam.g_HeroRankList.Clear();
        DBShareSeam.g_HeroWarriorRankList.Clear();
        DBShareSeam.g_HeroWizardRankList.Clear();
        DBShareSeam.g_HeroTaoistRankList.Clear();

        DBShareSeam.RankingEngine_RefRanking = () => { };
        RouteEditUnit.ShowFrmRouteEdit = RouteEditUnit.DefaultShowFrmRouteEdit;

        FdbExploreSeam.dwInterval = 3000;
        FdbExploreSeam.g_nClearIndex = 0;
        FdbExploreSeam.g_nClearCount = 0;
        FdbExploreSeam.g_nClearItemIndexCount = 0;
        FdbExploreSeam.g_nClearRecordCount = 0;
        FdbExploreSeam.boAutoClearDB = 0;
        FdbExploreSeam.nMonth1 = FdbExploreSeam.nDay1 = FdbExploreSeam.nLevel1 = 0;
        FdbExploreSeam.nMonth2 = FdbExploreSeam.nDay2 = FdbExploreSeam.nLevel2 = 0;
        FdbExploreSeam.nMonth3 = FdbExploreSeam.nDay3 = FdbExploreSeam.nLevel3 = 0;
        FdbExploreSeam.InClearMakeIndexList = _ => false;
        FdbExploreSeam.g_HumDataDB = null;
        FdbExploreSeam.FrmDBSrv_DelHum = _ => { };
        FdbExploreSeam.FrmDBSrv_CopyHumData = (_, _, _) => false;
        FdbExploreSeam.FrmUserSoc_NewChrData = (_, _, _, _, _) => false;
        FdbExploreSeam.FrmNewChr_sub_49BD60 = _ => { };
        FdbExploreSeam.FrmCopyRcd_sub_49C09C = () => false;
        FdbExploreSeam.FrmCopyRcd_s2F0 = "";
        FdbExploreSeam.FrmCopyRcd_s2F4 = "";
        FdbExploreSeam.FrmCopyRcd_s2F8 = "";
        FdbExploreSeam.ClearItemLogFile = "ClearItemLog.txt";
        FdbExploreSeam.Now = DelphiDate.Now;

        // 确定性时钟 / 随机
        DelphiTick.GetTickCount = () => 0;
        DelphiRandom.Next = _ => 0;

        // 消息框桩（永不阻塞）
        UiSeam.MessageBox = (_, _, _) => TMsgBox.IDOK;
        UiSeam.MessageDlg = (_, _, _, _) => TModalResult.mrNo;
        UiSeam.ShowMessage = _ => { };
    }

    /// <summary>构造一个 TUserItem（只填 FDBexpl 用到的两个字段）。</summary>
    public static TUserItem Item(ushort wIndex, int makeIndex) => new TUserItem { wIndex = wIndex, MakeIndex = makeIndex };

    /// <summary>构造一个人物记录。</summary>
    public static THumDataInfo MakeInfo(string name, double createDate, ushort level)
    {
        var info = new THumDataInfo();
        info.Header.sName = name;
        info.Header.dCreateDate = createDate;
        info.Data.ChrName = name;
        info.Data.Abil.Level = level;
        return info;
    }
}
