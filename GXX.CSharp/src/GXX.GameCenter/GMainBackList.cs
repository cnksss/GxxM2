using System;
using System.IO;
using GXX.Core.Util;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter;

/// <summary>
/// GMain.pas 备份清单持久化段 1:1 移植（行号范围 1446..1540）：
/// <list type="bullet">
/// <item>SaveBackList（1446-1470）</item>
/// <item>LoadBackList（1472-1517）</item>
/// <item>RefBackListToView（1519-1540）</item>
/// </list>
/// 落盘的 <c>BackList.txt</c> 节名为 <c>IntToStr(I)</c>（"0"/"1"/…），键序 Source/Save/Hour/Min/BackMode/GetBack/IsCompress 逐字保留。
/// </summary>
public static class GMainBackList
{
    /// <summary>GMain.pas:1446 <c>procedure TfrmMain.SaveBackList();</c></summary>
    public static void SaveBackList()
    {
        GMainConfig.DeleteBackListFile();
        using var Conini = new GameCenterIniFile(GMainConfig.BackListFileName());
        if (Conini != null)
        {
            for (int I = 0; I <= g_BackUpManager.m_BackUpList.Count - 1; I++)
            {
                IBackUpTask BackUpTask = g_BackUpManager.m_BackUpList[I];
                Conini.WriteString(DelphiSystem.IntToStr(I), "Source", BackUpTask.SourceDirectory);
                Conini.WriteString(DelphiSystem.IntToStr(I), "Save", BackUpTask.DestDirectory);
                Conini.WriteInteger(DelphiSystem.IntToStr(I), "Hour", BackUpTask.Hour);
                Conini.WriteInteger(DelphiSystem.IntToStr(I), "Min", BackUpTask.Min);
                Conini.WriteInteger(DelphiSystem.IntToStr(I), "BackMode", BackUpTask.Mode);
                Conini.WriteBool(DelphiSystem.IntToStr(I), "GetBack", BackUpTask.Start);
                // 是否压缩 piaoyun 2013-08-30
                Conini.WriteBool(DelphiSystem.IntToStr(I), "IsCompress", BackUpTask.IsCompress);
            }
            Conini.UpdateFile();
        }
    }

    /// <summary>
    /// GMain.pas:1472 <c>procedure TfrmMain.LoadBackList();</c>
    /// 接缝：<c>TBackUpTask.Create</c> 属 DataBackUp.pas，由 <see cref="BackUpTaskFactory"/> 注入。
    /// </summary>
    public static void LoadBackList()
    {
        SetButtonEnabled?.Invoke(BackListButton.ButtonBackDel, false);
        SetButtonEnabled?.Invoke(BackListButton.ButtonBackChg, false);
        var List = new List<string>();
        using var Conini = new GameCenterIniFile(GMainConfig.BackListFileName());
        Conini.ReadSections(List);
        if (Conini != null)
        {
            for (int I = 0; I <= List.Count - 1; I++)
            {
                string sSource = Conini.ReadString(List[I], "Source", "");
                string sDest = Conini.ReadString(List[I], "Save", "");
                ushort wHour = (ushort)Conini.ReadInteger(List[I], "Hour", 0);
                ushort wMin = (ushort)Conini.ReadInteger(List[I], "Min", 0);
                byte btBackMode = (byte)Conini.ReadInteger(List[I], "BackMode", 0);
                bool boGetBack = Conini.ReadBool(List[I], "GetBack", true);
                bool IsCompress = Conini.ReadBool(List[I], "IsCompress", true);
                if ((sSource != "") && (sDest != ""))
                {
                    IBackUpTask BackUpTask = BackUpTaskFactory!.Invoke();
                    BackUpTask.SourceDirectory = sSource;
                    BackUpTask.DestDirectory = sDest;
                    BackUpTask.Mode = btBackMode;
                    BackUpTask.Hour = wHour;
                    BackUpTask.Min = wMin;
                    BackUpTask.Start = boGetBack;
                    // 是否压缩 piaoyun 2013-08-30
                    BackUpTask.IsCompress = IsCompress;
                    g_BackUpManager.Add(BackUpTask);
                }
            }
        }
    }

    /// <summary>DataBackUp.pas <c>TBackUpTask.Create</c> 接缝（托管侧无无参构造约束，用工厂注入）。</summary>
    public static Func<IBackUpTask>? BackUpTaskFactory;

    /// <summary>LoadBackList 里 <c>ButtonBackDel/ButtonBackChg.Enabled := False</c> 接缝。</summary>
    public static Action<BackListButton, bool>? SetButtonEnabled;

    /// <summary>主窗体备份页按钮标识。</summary>
    public enum BackListButton
    {
        /// <summary>ButtonBackDel。</summary>
        ButtonBackDel = 0,
        /// <summary>ButtonBackChg。</summary>
        ButtonBackChg = 1,
    }

    /// <summary>
    /// GMain.pas:1519 <c>procedure TfrmMain.RefBackListToView();</c>
    /// 接缝：<c>ListViewDataBackup</c> 由 <see cref="AddListViewRow"/> 注入；
    /// 原文列序为 Caption=SourceDirectory，SubItems = [DestDirectory(带 Object), BackUpCount, FailCount, 启动/停止]。
    /// </summary>
    public static void RefBackListToView()
    {
        ClearListView?.Invoke();
        for (int I = 0; I <= g_BackUpManager.m_BackUpList.Count - 1; I++)
        {
            IBackUpTask BackUpTask = g_BackUpManager.m_BackUpList[I];

            AddListViewRow?.Invoke(
                BackUpTask.SourceDirectory,
                BackUpTask.DestDirectory,
                BackUpTask,
                DelphiSystem.IntToStr(BackUpTask.BackUpCount),
                DelphiSystem.IntToStr(BackUpTask.FailCount),
                BackUpTask.Start ? "启动" : "停止");
        }
    }

    /// <summary><c>ListViewDataBackup.Items.Clear</c> 接缝。</summary>
    public static Action? ClearListView;

    /// <summary>ListViewDataBackup 追加一行（Caption / SubItems 四列 / 行 Object）接缝。</summary>
    public static Action<string, string, IBackUpTask, string, string, string>? AddListViewRow;
}
