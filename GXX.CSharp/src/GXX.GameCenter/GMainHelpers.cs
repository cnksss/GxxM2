using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter;

/// <summary>
/// GMain.pas 顶层（非 TfrmMain 成员）辅助过程 1:1 移植：
/// <list type="bullet">
/// <item>ListBoxAdd / ListBoxDel（581-606，piaoyun 2013-08-28）</item>
/// <item>ClearModValue / ClearTxt（608-622）</item>
/// <item>Clear_SaveConfig / Clear_LoadConfig（626-688）</item>
/// <item>ClearSetupIni（759-839，MAXCHANGELEVEL=1000）</item>
/// <item>ClearGlobal（842-864）</item>
/// <item>GetClearAccountDBSql / GetClearRoleDataDBSql / GetClearUserShopSql / GetClearStorageExSql /
/// GetClearAuctionDataSql（5730-5846，纯字符串常量"清空数据库"SQL）</item>
/// </list>
/// 依赖的 ListBox 由 <see cref="ListBoxTarget"/> 接缝注入（原文直接操作 VCL TListBox）。
/// </summary>
public static class GMainHelpers
{
    /// <summary>GMain.pas:581 <c>procedure ListBoxAdd(ListBox: TListBox; AddStr: string);</c></summary>
    public static void ListBoxAdd(ListBoxTarget listBox, string AddStr)
    {
        for (int i = 0; i <= listBox.ItemsCount - 1; i++)
        {
            if (listBox.ItemAt(i) == AddStr)
            {
                GameCenterDialogs.MessageBox("此文件路径已在列表中，请重新选择！！", "提示信息",
                    GameCenterDialogs.MB_ICONASTERISK);
                return;
            }
        }
        listBox.Add(AddStr);
    }

    /// <summary>GMain.pas:598 <c>procedure ListBoxDel(ListBox: TListBox);</c></summary>
    public static void ListBoxDel(ListBoxTarget listBox)
    {
        listBox.DeleteSelected();
    }

    /// <summary>GMain.pas:608 <c>procedure ClearModValue();</c></summary>
    public static void ClearModValue()
    {
        ClearModValueHandler?.Invoke();
    }

    /// <summary>GMain.pas:610 <c>frmMain.btnClearSave.Enabled := True;</c> 接缝。</summary>
    public static Action? ClearModValueHandler;

    /// <summary>GMain.pas:615 <c>procedure ClearTxt(TxtName: string);</c>
    /// （assignfile/rewrite/closefile → 建空文件；原文如此：若目录不存在会抛 I/O 错误）。</summary>
    public static void ClearTxt(string TxtName)
    {
        File.WriteAllText(TxtName, "");
    }

    /// <summary>GMain.pas:626 <c>function Clear_SaveConfig(): Boolean;</c></summary>
    public static bool Clear_SaveConfig()
    {
        //Result := False;
        g_IniConf.WriteInteger("ClearServer", "MyGetTxtNum", ListCount(ListSource.MyGetTxt));
        if (ListCount(ListSource.MyGetTxt) != 0)
        {
            for (int I = 0; I <= ListCount(ListSource.MyGetTxt) - 1; I++)
            {
                g_IniConf.WriteString("ClearServer", "MyGetTxt" + DelphiSystem.IntToStr(I), ListItemAt(ListSource.MyGetTxt, I));
            }
        }

        g_IniConf.WriteInteger("ClearServer", "MyGetFileNum", ListCount(ListSource.MyGetFile));
        if (ListCount(ListSource.MyGetFile) != 0)
        {
            for (int I = 0; I <= ListCount(ListSource.MyGetFile) - 1; I++)
            {
                g_IniConf.WriteString("ClearServer", "MyGetFile" + DelphiSystem.IntToStr(I), ListItemAt(ListSource.MyGetFile, I));
            }
        }

        g_IniConf.WriteInteger("ClearServer", "MyGetDirNum", ListCount(ListSource.MyGetDir));
        if (ListCount(ListSource.MyGetDir) != 0)
        {
            for (int I = 0; I <= ListCount(ListSource.MyGetDir) - 1; I++)
            {
                g_IniConf.WriteString("ClearServer", "MyGetDir" + DelphiSystem.IntToStr(I), ListItemAt(ListSource.MyGetDir, I));
            }
        }
        return true;
    }

    /// <summary>GMain.pas:656 <c>procedure Clear_LoadConfig();</c></summary>
    public static void Clear_LoadConfig()
    {
        int nMyGetTxtNum = g_IniConf.ReadInteger("ClearServer", "MyGetTxtNum", 0);
        int nMyGetFileNum = g_IniConf.ReadInteger("ClearServer", "MyGetFileNum", 0);
        int nMyGetDirNum = g_IniConf.ReadInteger("ClearServer", "MyGetDirNum", 0);
        if (nMyGetTxtNum != 0)
        {
            ClearList(ListSource.MyGetTxt);
            for (int I = 0; I <= nMyGetTxtNum - 1; I++)
            {
                AddToList(ListSource.MyGetTxt, g_IniConf.ReadString("ClearServer", "MyGetTxt" + DelphiSystem.IntToStr(I), "读取配置文件错误"));
            }
        }

        if (nMyGetFileNum != 0)
        {
            ClearList(ListSource.MyGetFile);
            for (int I = 0; I <= nMyGetFileNum - 1; I++)
            {
                AddToList(ListSource.MyGetFile, g_IniConf.ReadString("ClearServer", "MyGetFile" + DelphiSystem.IntToStr(I), "读取配置文件错误"));
            }
        }
        if (nMyGetDirNum != 0)
        {
            ClearList(ListSource.MyGetDir);
            for (int I = 0; I <= nMyGetDirNum - 1; I++)
            {
                AddToList(ListSource.MyGetDir, g_IniConf.ReadString("ClearServer", "MyGetDir" + DelphiSystem.IntToStr(I), "读取配置文件错误"));
            }
        }
    }

    // ---- Clear_SaveConfig / Clear_LoadConfig / GenBackupConfig 的 ListBox 接缝 ----
    //      （由主窗体接线到 lstMyGetTXT / lstMyGetFile / lstMyGetDir）

    /// <summary>列表项数（<c>lst*.Items.Count</c>）。</summary>
    public static Func<ListSource, int>? ListCountProvider;

    /// <summary>列表第 i 项（<c>lst*.Items.Strings[i]</c>）。</summary>
    public static Func<ListSource, int, string>? ListItemProvider;

    /// <summary>列表清空（<c>lst*.Items.Clear</c>）。</summary>
    public static Action<ListSource>? ListClearHandler;

    /// <summary>列表追加（<c>lst*.Items.Add</c>）。</summary>
    public static Action<ListSource, string>? ListAddHandler;

    /// <summary>GMain.pas:631 等处 <c>lstMyGetTXT</c> 系列列表标识。</summary>
    public enum ListSource
    {
        /// <summary>lstMyGetTXT。</summary>
        MyGetTxt = 0,
        /// <summary>lstMyGetFile。</summary>
        MyGetFile = 1,
        /// <summary>lstMyGetDir。</summary>
        MyGetDir = 2,
    }

    /// <summary>列表项数（未接线时按 0 计，等价于空列表）。</summary>
    public static int ListCount(ListSource src) => ListCountProvider?.Invoke(src) ?? 0;

    /// <summary>列表第 i 项（未接线时空串）。</summary>
    public static string ListItemAt(ListSource src, int i) => ListItemProvider?.Invoke(src, i) ?? "";

    /// <summary>列表清空。</summary>
    public static void ClearList(ListSource src) => ListClearHandler?.Invoke(src);

    /// <summary>列表追加。</summary>
    public static void AddToList(ListSource src, string s) => ListAddHandler?.Invoke(src, s);

    // ===================== ClearSetupIni（GMain.pas:759-839） =====================

    /// <summary>GMain.pas:761 <c>MAXCHANGELEVEL = 1000;</c></summary>
    public const int MAXCHANGELEVEL = 1000;

    /// <summary>GMain.pas:759 <c>procedure ClearSetupIni;</c>
    /// 删除 !setup.txt 中的等级/经验扩展键，空节则整体擦除。</summary>
    public static void ClearSetupIni()
    {
        string FileName = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConfigFile;
        if (!File.Exists(FileName)) return;

        var SL = new List<string>();
        using var SetupIni = new GameCenterIniFile(FileName);
        SetupIni.DeleteKey("Setup", "HighLevel");
        SetupIni.DeleteKey("Setup", "HighLevelGetExp");
        SetupIni.DeleteKey("Setup", "MaxUpLevelCount");
        SetupIni.DeleteKey("Setup", "LimitChangeExp");
        SetupIni.DeleteKey("Exp", "KillMonExpMultiple");
        SetupIni.DeleteKey("Exp", "HighLevelKillMonFixExp");
        SetupIni.DeleteKey("Exp", "UseFixExp");
        SetupIni.DeleteKey("Exp", "BaseExp");
        SetupIni.DeleteKey("Exp", "AddExp");
        SetupIni.DeleteKey("Exp", "HighLevelGroupFixExp");

        for (int I = 1; I <= MAXCHANGELEVEL; I++)
            SetupIni.DeleteKey("Exp", "Level" + DelphiSystem.IntToStr(I));

        for (int I = 1; I <= MAXCHANGELEVEL; I++)
            SetupIni.DeleteKey("Exp", "LevelExpRate" + DelphiSystem.IntToStr(I));

        for (int I = 1; I <= MAXCHANGELEVEL; I++)
            SetupIni.DeleteKey("HeroExp", "Level" + DelphiSystem.IntToStr(I));

        for (int I = 1; I <= MAXCHANGELEVEL; I++)
            SetupIni.DeleteKey("MedicineExp", "Level" + DelphiSystem.IntToStr(I));

        for (int I = 1; I <= MAXCHANGELEVEL; I++)
            SetupIni.DeleteKey("WineExp", "Level" + DelphiSystem.IntToStr(I));

        SetupIni.DeleteKey("Setup", "IncAlcoholTime");
        SetupIni.DeleteKey("Setup", "DecDrinkTime");
        SetupIni.DeleteKey("Setup", "MaxAlcoholValue");
        SetupIni.DeleteKey("Setup", "IncAlcoholValue");
        SetupIni.DeleteKey("Setup", "DecMedicineValue");
        SetupIni.DeleteKey("Setup", "DecMedicineTime");

        SL.Clear();
        SetupIni.ReadSection("Exp", SL);
        if (SL.Count == 0)
            SetupIni.EraseSection("Exp");

        SL.Clear();
        SetupIni.ReadSection("HeroExp", SL);
        if (SL.Count == 0)
            SetupIni.EraseSection("HeroExp");

        SL.Clear();
        SetupIni.ReadSection("MedicineExp", SL);
        if (SL.Count == 0)
            SetupIni.EraseSection("MedicineExp");

        SL.Clear();
        SetupIni.ReadSection("WineExp", SL);
        if (SL.Count == 0)
            SetupIni.EraseSection("WineExp");

        for (int I = 0; I <= 499; I++)
        {
            SetupIni.DeleteKey("Setup", "GlobalVal" + DelphiSystem.IntToStr(I));
            SetupIni.DeleteKey("Setup", "GlobalStrVal" + DelphiSystem.IntToStr(I));
        }

        SetupIni.UpdateFile();
    }

    // ===================== ClearGlobal（GMain.pas:842-864） =====================

    /// <summary>GMain.pas:842 <c>function ClearGlobal(FileName: string): Boolean;</c></summary>
    public static bool ClearGlobal(string FileName)
    {
        //Result := False;
        using var Config = new GameCenterIniFile(FileName);
        for (int I = 0; I <= 999; I++)
        {
            Config.WriteInteger("Setup", "GlobalVal" + DelphiSystem.IntToStr(I), 0);
        }

        for (int I = 0; I <= 999; I++)
        {
            Config.WriteString("Setup", "GlobalStrVal" + DelphiSystem.IntToStr(I), "");
        }

        Config.UpdateFile();
        //sleep(2000);
        return true;
    }

    // ===================== 清库 SQL（GMain.pas:5730-5846，见 GMainClearSql.cs） =====================
}

/// <summary>
/// Delphi <c>TListBox</c> 在 ListBoxAdd/ListBoxDel 中用到的四个成员（ItemsCount/ItemAt/Add/DeleteSelected）。
/// </summary>
public interface ListBoxTarget
{
    /// <summary><c>ListBox.Items.Count</c>。</summary>
    int ItemsCount { get; }

    /// <summary><c>ListBox.Items.Strings[i]</c>。</summary>
    string ItemAt(int index);

    /// <summary><c>ListBox.Items.Add(AddStr)</c>。</summary>
    void Add(string value);

    /// <summary><c>ListBox.DeleteSelected</c>。</summary>
    void DeleteSelected();
}

/// <summary>
/// Delphi <c>Dialogs.ShowMessage</c> / <c>Application.MessageBox</c> 的等效层。
/// 测试注入 <see cref="MessageBoxHandler"/> 阻断真实弹窗（与 GXX.M2Server.Forms.M2Forms 同约定）。
/// </summary>
public static class GameCenterDialogs
{
    public const int MB_OK = 0x00;
    public const int MB_YESNO = 0x04;
    public const int MB_ICONASTERISK = 0x40;
    public const int MB_ICONERROR = 0x10;
    public const int MB_ICONQUESTION = 0x20;
    public const int MB_ICONINFORMATION = 0x40;

    public const int IDOK = 1;
    public const int IDYES = 6;
    public const int IDNO = 7;

    /// <summary>测试注入：(text, caption, flags) → 返回值。</summary>
    public static Func<string, string, int, int>? MessageBoxHandler;

    /// <summary>测试捕获：最近一次弹窗文本/标题。</summary>
    public static string? LastMessage;
    public static string? LastCaption;

    /// <summary>测试注入：下一次 MessageBox 返回值（弹一次后清除）。</summary>
    public static int? NextAnswer;

    public static int MessageBox(string text, string caption, int flags)
    {
        LastMessage = text;
        LastCaption = caption;
        if (NextAnswer.HasValue)
        {
            int answer = NextAnswer.Value;
            NextAnswer = null;
            return answer;
        }
        if (MessageBoxHandler != null)
            return MessageBoxHandler(text, caption, flags);

        var buttons = (flags & MB_YESNO) != 0
            ? System.Windows.Forms.MessageBoxButtons.YesNo
            : System.Windows.Forms.MessageBoxButtons.OK;
        var icon = (flags & MB_ICONQUESTION) != 0 ? System.Windows.Forms.MessageBoxIcon.Question
                 : (flags & MB_ICONERROR) != 0 ? System.Windows.Forms.MessageBoxIcon.Error
                 : System.Windows.Forms.MessageBoxIcon.None;
        return (int)System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon);
    }

    /// <summary>Delphi <c>Dialogs.ShowMessage(msg)</c>（单参数重载）。</summary>
    public static void ShowMessage(string text)
        => MessageBox(text, System.Windows.Forms.Application.ProductName, MB_OK | MB_ICONINFORMATION);

    /// <summary>测试复位（含 LastMessage/LastCaption/NextAnswer）。</summary>
    public static void ResetForTests()
    {
        MessageBoxHandler = null;
        LastMessage = null;
        LastCaption = null;
        NextAnswer = null;
    }
}
