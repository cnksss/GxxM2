using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.DBServer;

// FDBexpl.pas (1-369) → FDBexpl.cs
// 人物数据管理窗体（查找/删除/重建/自动清理过期人物与指定 MakeIndex 的物品）。
//
// 接缝说明（原文 uses 的四个单元在提供的源码树中**不存在**，FDBexpl 无法在 Delphi 侧单独编译）：
//   · uses HumDB      → g_HumDataDB / THumDataInfo / InClearMakeIndexList（本文件以 IHumDataDB 接口 + THumDataInfo 占位定义）
//   · uses Main       → dwInterval / g_nClear* / boAutoClearDB / nMonth1..nLevel3 / FrmDBSrv（FdbExploreSeam）
//   · uses NewChr     → FrmNewChr.sub_49BD60（FdbExploreSeam.NewChrHandler）
//   · uses frmcpyrcd  → FrmCopyRcd.sub_49C09C / s2F0 / s2F4 / s2F8（FdbExploreSeam.CopyRcd*）
// 这些占位值的取值来源缺失，一律标注"接缝"，待对应单元移植后以原文为准。

/// <summary>HumDB.pas 的人物数据头部（原文缺失；仅保留 FDBexpl 用到的字段）。</summary>
public class THumInfoHeader
{
    /// <summary>TDateTime（天数）。原文 FDBexpl.pas:225 用 `Header.dCreateDate &lt; dt20` 判过期。</summary>
    public double dCreateDate;

    /// <summary>人物名（原文为 string[14]，此处 string；接缝待 HumDB.pas 移植后校正）。</summary>
    public string sName = "";
}

/// <summary>
/// HumDB.pas 的 `THumDataInfo`（原文缺失）。
/// 托管侧用 class 以保持原文 `var ChrRecord: THumDataInfo` 的"就地改写调用方记录"语义。
/// </summary>
public class THumDataInfo
{
    public THumInfoHeader Header = new THumInfoHeader();

    public THumData Data;

    /// <summary>
    /// 接缝：HumDB.pas 的 Data 含 `HumAddItems`，而 Grobal2.pas 的 THumData 无此字段。
    /// 长度 30 为占位（与 THumData.HumItems 对齐），待 HumDB.pas 移植后按原文常量校正。
    /// </summary>
    public TUserItem[] HumAddItems = new TUserItem[30];
}

/// <summary>
/// HumDB.pas `g_HumDataDB` 的接口接缝（真实实现是 Sqlite/MySql 的角色库）。
/// 单测注入内存实现，绝不连真实数据库。
/// </summary>
public interface IHumDataDB
{
    /// <summary>FDBexpl.pas:69/88/220 `g_HumDataDB.Open`（返回是否成功）。</summary>
    bool Open();

    /// <summary>FDBexpl.pas:76/92/242 `g_HumDataDB.Close`。</summary>
    void Close();

    /// <summary>FDBexpl.pas:70 `Find(sChrName, ListBox1.Items)`：向 TStrings 追加名称，Objects[i] 为记录号。</summary>
    void Find(string sChrName, TStringList items);

    /// <summary>FDBexpl.pas:89/230 `Delete(nIndex)`。</summary>
    void Delete(int nIndex);

    /// <summary>FDBexpl.pas:101 `Rebuild()`。</summary>
    void Rebuild();

    /// <summary>FDBexpl.pas:221 `g_HumDataDB.Count`。</summary>
    int Count { get; }

    /// <summary>FDBexpl.pas:223 `n8 := g_HumDataDB.Get(g_nClearIndex, ChrRecord)`（Get 填充 var 记录并返回记录号，&lt;0 表示失败）。</summary>
    int Get(int index, out THumDataInfo record);

    /// <summary>FDBexpl.pas:234 `g_HumDataDB.Update(g_nClearIndex, ChrRecord)`。</summary>
    void Update(int index, THumDataInfo record);
}

/// <summary>
/// FDBexpl.pas 依赖的 HumDB/Main/NewChr/frmcpyrcd 单元级变量与回调（全部为接缝）。
/// 默认值源码缺失，除 DFM 可见者（Timer1.Interval=3000）外一律留 0 并由调用方注入。
/// </summary>
public static class FdbExploreSeam
{
    /// <summary>接缝：Main.pas `dwInterval`（DFM 中 Timer1.Interval=3000，FormCreate 会覆盖）。</summary>
    public static int dwInterval = 3000;

    /// <summary>接缝：Main.pas `g_nClearIndex`。</summary>
    public static int g_nClearIndex;

    /// <summary>接缝：Main.pas `g_nClearCount`。</summary>
    public static int g_nClearCount;

    /// <summary>接缝：Main.pas `g_nClearItemIndexCount`。</summary>
    public static int g_nClearItemIndexCount;

    /// <summary>接缝：Main.pas `g_nClearRecordCount`。</summary>
    public static int g_nClearRecordCount;

    /// <summary>接缝：Main.pas `boAutoClearDB`（FDBexpl.pas:100 处被注释掉的 `//boAutoClearDB := False;`）。</summary>
    public static byte boAutoClearDB;

    /// <summary>接缝：Main.pas 的等级/时长阈值（DFM 提示为 1级/1星期、7级/1个月、14级/4个月，具体数值源码缺失）。</summary>
    public static ushort nMonth1, nDay1, nLevel1;
    public static ushort nMonth2, nDay2, nLevel2;
    public static ushort nMonth3, nDay3, nLevel3;

    /// <summary>接缝：HumDB.pas `InClearMakeIndexList(MakeIndex)`（命中即清理该物品）。</summary>
    public static Func<int, bool> InClearMakeIndexList = _ => false;

    /// <summary>接缝：HumDB.pas `g_HumDataDB`。</summary>
    public static IHumDataDB g_HumDataDB;

    /// <summary>接缝：uFrmMain.pas `FrmDBSrv.DelHum(sHumName)`。</summary>
    public static Action<string> FrmDBSrv_DelHum = _ => { };

    /// <summary>接缝：uFrmMain.pas `FrmDBSrv.CopyHumData(sSrc, sDest, sUserId)`。</summary>
    public static Func<string, string, string, bool> FrmDBSrv_CopyHumData = (_, _, _) => false;

    /// <summary>接缝：Main.pas `FrmUserSoc.NewChrData(sChrName, Job, Sex, Hair, IsHero)`。</summary>
    public static Func<string, int, int, int, bool, bool> FrmUserSoc_NewChrData = (_, _, _, _, _) => false;

    /// <summary>接缝：NewChr.pas `FrmNewChr.sub_49BD60(var sChrName)`。</summary>
    public static Action<Action<string>> FrmNewChr_sub_49BD60 = _ => { };

    /// <summary>接缝：frmcpyrcd.pas `FrmCopyRcd.sub_49C09C` 与其三个字段 s2F0/s2F4/s2F8。</summary>
    public static Func<bool> FrmCopyRcd_sub_49C09C = () => false;
    public static string FrmCopyRcd_s2F0 = "";
    public static string FrmCopyRcd_s2F4 = "";
    public static string FrmCopyRcd_s2F8 = "";

    /// <summary>接缝：原文硬编码的相对文件名 'ClearItemLog.txt'，暴露以便单测改到临时目录。</summary>
    public static string ClearItemLogFile = "ClearItemLog.txt";

    /// <summary>接缝：时间源（原文 Now()）。</summary>
    public static Func<double> Now = DelphiDate.Now;
}

/// <summary>FDBexpl.pas 的非 UI 逻辑（可单测）。</summary>
public static class FdbExploreLogic
{
    /// <summary>Grobal2.Types4.cs 的 InlineArray 长度常量（InlineArray 不暴露 Length）。</summary>
    public const int HumItemsLength = 30;       // TUserItemArray30  HumItems
    public const int BagItemsLength = 206;      // TUserItemArray206 BagItems (ALL_BAG_ITEM_COUNT)
    public const int StorageItemsLength = 196;  // TUserItemArray196 StorageItems

    /// <summary>
    /// FDBexpl.pas:155-180 嵌套函数 `GetDateTime(wM, wD: Word): TDateTime`：
    ///   从今天往回退 wM 个月（逐月退，月份到 1 再退则年 -= 1、月 := 12），
    ///   再往回退 wD 天（逐天退，日到 1 再退则 Day := 28 并把月/年继续回退）。
    /// 原文如此：日回退时直接跳到 28 号（规避月末天数问题）。
    /// </summary>
    public static double GetDateTime(ushort wM, ushort wD, ushort nowYear, ushort nowMonth, ushort nowDay)
    {
        ushort Year = nowYear, Month = nowMonth, Day = nowDay;
        int i;
        for (i = 0; i <= wM - 1; i++)
        {
            if (Month > 1) Month--;
            else
            {
                Month = 12;
                Year--;
            }
        }
        for (i = 0; i <= wD - 1; i++)
        {
            if (Day > 1) Day--;
            else
            {
                Day = 28;
                if (Month > 1) Month--;
                else
                {
                    Month = 12;
                    Year--;
                }
            }
        }
        return DelphiDate.EncodeDate(Year, Month, Day);
    }

    /// <summary>FDBexpl.pas:155-180 嵌套 `GetDateTime`（用当前时间）。</summary>
    public static double GetDateTime(ushort wM, ushort wD)
    {
        DelphiDate.DecodeDate(FdbExploreSeam.Now(), out ushort y, out ushort m, out ushort d);
        return GetDateTime(wM, wD, y, m, d);
    }

    /// <summary>
    /// FDBexpl.pas:250-335 `ClearHumanItem`：
    ///   按 HumItems → HumAddItems → BagItems → StorageItems 的顺序扫描，命中 InClearMakeIndexList 的槽位
    ///   （wIndex &gt; 0 才考虑）先整条拷贝进 ClearList、再把原槽 wIndex 清 0；
    ///   有清理时累加 g_nClearItemIndexCount，并把每条明细 `sChrName \t wIndex \t MakeIndex`
    ///   **Insert(0, …)** 进 ClearItemLog.txt（新的在最前，保留文件原有内容）。
    /// </summary>
    public static bool ClearHumanItem(THumDataInfo Info, out int clearedCount)
    {
        bool Result = false;
        List<TUserItem> ClearList = null;
        clearedCount = 0;

        // ChrRecord.Data.HumItems
        for (int i = 0; i <= HumItemsLength - 1; i++)
        {
            ref TUserItem UserItem = ref Info.Data.HumItems[i];
            if (UserItem.wIndex <= 0) continue;
            if (FdbExploreSeam.InClearMakeIndexList(UserItem.MakeIndex))
            {
                ClearList ??= new List<TUserItem>();
                ClearList.Add(UserItem);
                UserItem.wIndex = 0;
                Result = true;
            }
        }
        // ChrRecord.Data.HumAddItems
        for (int i = 0; i <= Info.HumAddItems.Length - 1; i++)
        {
            ref TUserItem UserItem = ref Info.HumAddItems[i];
            if (UserItem.wIndex <= 0) continue;
            if (FdbExploreSeam.InClearMakeIndexList(UserItem.MakeIndex))
            {
                ClearList ??= new List<TUserItem>();
                ClearList.Add(UserItem);

                UserItem.wIndex = 0;
                Result = true;
            }
        }
        // ChrRecord.Data.BagItems
        for (int i = 0; i <= BagItemsLength - 1; i++)
        {
            ref TUserItem UserItem = ref Info.Data.BagItems[i];
            if (UserItem.wIndex <= 0) continue;
            if (FdbExploreSeam.InClearMakeIndexList(UserItem.MakeIndex))
            {
                ClearList ??= new List<TUserItem>();
                ClearList.Add(UserItem);
                UserItem.wIndex = 0;
                Result = true;
            }
        }
        // ChrRecord.Data.StorageItems
        for (int i = 0; i <= StorageItemsLength - 1; i++)
        {
            ref TUserItem UserItem = ref Info.Data.StorageItems[i];
            if (UserItem.wIndex <= 0) continue;
            if (FdbExploreSeam.InClearMakeIndexList(UserItem.MakeIndex))
            {
                ClearList ??= new List<TUserItem>();
                ClearList.Add(UserItem);

                UserItem.wIndex = 0;
                Result = true;
            }
        }
        if (Result)
        {
            clearedCount = ClearList.Count;
            FdbExploreSeam.g_nClearItemIndexCount += ClearList.Count;

            var SaveList = new TStringList();
            string sFileName = FdbExploreSeam.ClearItemLogFile;
            if (File.Exists(sFileName))
            {
                SaveList.LoadFromFile(sFileName);
            }
            for (int i = 0; i <= ClearList.Count - 1; i++)
            {
                TUserItem UserItem = ClearList[i];
                string sMsg = Info.Data.ChrName + "\t" + DelphiRTL.IntToStr(UserItem.wIndex) + "\t" + DelphiRTL.IntToStr(UserItem.MakeIndex);
                SaveList.Insert(0, sMsg);
            }
            SaveList.SaveToFile(sFileName);
        }
        return Result;
    }

    /// <summary>
    /// FDBexpl.pas:153-248 `Timer1Timer`（去掉 UI 部分）：
    ///   返回被删除的人物名（''=无），并就地更新 g_nClearIndex / g_nClearCount / g_nClearRecordCount。
    /// 三种"过期"条件是 **or** 关系（阈值来自 CkLv1/CkLv7/CkLv14 是否勾选，未勾选时阈值为 0）。
    /// </summary>
    public static string Timer1Timer(IHumDataDB db, bool ckLv1, bool ckLv7, bool ckLv14)
    {
        if (FdbExploreSeam.boAutoClearDB == 0) return "";

        ushort w32 = 0, w38 = 0, w3E = 0;
        ushort wDayCount1 = 0, wDayCount7 = 0, wDayCount14 = 0;
        ushort wLevel1 = 0, wLevel7 = 0, wLevel14 = 0;
        int n8, n10;

        string sHumName = "";
        THumDataInfo ChrRecord;

        if (ckLv1)
        {
            w32 = FdbExploreSeam.nMonth1;
            wDayCount1 = FdbExploreSeam.nDay1;
            wLevel1 = FdbExploreSeam.nLevel1;
        }
        if (ckLv7)
        {
            w38 = FdbExploreSeam.nMonth2;
            wDayCount7 = FdbExploreSeam.nDay2;
            wLevel7 = FdbExploreSeam.nLevel2;
        }
        if (ckLv14)
        {
            w3E = FdbExploreSeam.nMonth3;
            wDayCount14 = FdbExploreSeam.nDay3;
            wLevel14 = FdbExploreSeam.nLevel3;
        }
        double dt20 = GetDateTime(w32, wDayCount1);
        double dt28 = GetDateTime(w38, wDayCount7);
        double dt30 = GetDateTime(w3E, wDayCount14);
        FdbExploreSeam.g_nClearRecordCount = 0;
        sHumName = "";
        try
        {
            if (db.Open())
            {
                FdbExploreSeam.g_nClearRecordCount = db.Count;
                if (FdbExploreSeam.g_nClearIndex < FdbExploreSeam.g_nClearRecordCount)
                {
                    n8 = db.Get(FdbExploreSeam.g_nClearIndex, out ChrRecord);
                    if (n8 >= 0)
                    {
                        if ((ChrRecord.Header.dCreateDate < dt20) && (ChrRecord.Data.Abil.Level <= wLevel1) ||
                            (ChrRecord.Header.dCreateDate < dt28) && (ChrRecord.Data.Abil.Level <= wLevel7) ||
                            (ChrRecord.Header.dCreateDate < dt30) && (ChrRecord.Data.Abil.Level <= wLevel14))
                        {
                            n10 = n8;
                            sHumName = ChrRecord.Header.sName;
                            db.Delete(n10);
                            FdbExploreSeam.g_nClearCount++;
                        }
                        else
                        {
                            int dummy;
                            if (ClearHumanItem(ChrRecord, out dummy))
                            {
                                db.Update(FdbExploreSeam.g_nClearIndex, ChrRecord);
                            }
                        }
                        FdbExploreSeam.g_nClearIndex++;
                    }
                }
                else FdbExploreSeam.g_nClearIndex = 0;
            }
        }
        finally
        {
            db.Close();
        }
        if (sHumName != "")
        {
            FdbExploreSeam.FrmDBSrv_DelHum(sHumName);
        }
        return sHumName;
    }

    /// <summary>FDBexpl.pas:57-78 `EdFindKeyPress`：非 #13 直接返回；名称为空返回；否则清空两列表并 Find。</summary>
    public static void EdFindKeyPress(IHumDataDB db, char Key, string edFindText, TStringList listBox1, List<int> listBox2)
    {
        int i;
        string sChrName;

        if (Key != '\r') return;
        sChrName = DelphiRTL.Trim(edFindText);
        if (sChrName == "") return;
        listBox1.Clear();
        listBox2.Clear();

        try
        {
            if (db.Open())
            {
                db.Find(sChrName, listBox1);
                for (i = 0; i <= listBox1.Count - 1; i++)
                {
                    listBox2.Add(Convert.ToInt32(listBox1.GetObject(i)));   // Integer(ListBox1.Items.Objects[i])
                }
            }
        }
        finally
        {
            db.Close();
        }
    }

    /// <summary>FDBexpl.pas:80-95 `BtnDelClick`：无选中返回；确认后 Open→Delete→Close。</summary>
    public static void BtnDelClick(IHumDataDB db, int listBox1ItemIndex, int recordIndex)
    {
        if (listBox1ItemIndex <= -1) return;
        if (UiSeam.MessageDlg("是否确认删除人物数据 " + DelphiRTL.IntToStr(recordIndex) + " ？",
                TMsgDlgType.mtConfirmation, TMsgDlgButtons.mbYes | TMsgDlgButtons.mbNo, 0) == TModalResult.mrYes)
        {
            try
            {
                if (db.Open())
                {
                    db.Delete(recordIndex);
                }
            }
            finally
            {
                db.Close();
            }
        }
    }

    /// <summary>FDBexpl.pas:97-104 `BtnRebuildClick`：确认后 Rebuild 并提示完成。</summary>
    public static void BtnRebuildClick(IHumDataDB db)
    {
        if (UiSeam.MessageDlg("在重建数据库过程中，数据库服务器将停止工作，是否确认继续？",
                TMsgDlgType.mtConfirmation, TMsgDlgButtons.mbYes | TMsgDlgButtons.mbNo, 0) == TModalResult.mrYes)
        {
            //boAutoClearDB := False;
            db.Rebuild();
            UiSeam.MessageDlg("数据库重建完成！！！", TMsgDlgType.mtInformation, TMsgDlgButtons.mbOK, 0);
        }
    }

    /// <summary>FDBexpl.pas:146-151 `BtnAutoCleanClick`：翻转 boAutoClearDB 并切换按钮标题。</summary>
    public static string BtnAutoCleanClick()
    {
        FdbExploreSeam.boAutoClearDB = (byte)(FdbExploreSeam.boAutoClearDB == 0 ? 1 : 0);
        if (FdbExploreSeam.boAutoClearDB != 0) return "自动清理";
        return "已停止清理";
    }

    /// <summary>FDBexpl.pas:112-118 `BtnAddClick`：FrmNewChr.sub_49BD60(sChrName)（sChrName 由回调回填）。</summary>
    public static string BtnAddClick()
    {
        string sChrName = "";
        FdbExploreSeam.FrmNewChr_sub_49BD60(s => sChrName = s);
        //FrmUserSoc.NewChrData(sChrName, 0, 0, 0, False);
        return sChrName;
    }

    /// <summary>FDBexpl.pas:336-347 `BtnCopyRcdClick`。</summary>
    public static bool BtnCopyRcdClick(out string sSrcChrName, out string sDestChrName, out string sUserId)
    {
        sSrcChrName = "";
        sDestChrName = "";
        sUserId = "";
        if (!FdbExploreSeam.FrmCopyRcd_sub_49C09C()) return false;
        sSrcChrName = FdbExploreSeam.FrmCopyRcd_s2F0;
        sDestChrName = FdbExploreSeam.FrmCopyRcd_s2F4;
        sUserId = FdbExploreSeam.FrmCopyRcd_s2F8;
        if (FdbExploreSeam.FrmDBSrv_CopyHumData(sSrcChrName, sDestChrName, sUserId))
            UiSeam.ShowMessage(sSrcChrName + " -> " + sDestChrName + " 复制成功！！！");
        return true;
    }

    /// <summary>
    /// FDBexpl.pas:349-361 `BtnCopyNewClick`：
    ///   原文如此：先 `FrmUserSoc.NewChrData(sDestChrName, 0, 0, 0, False)` 建新人物，成功后再 CopyHumData；
    ///   两者都成功才提示。
    /// </summary>
    public static bool BtnCopyNewClick(out string sSrcChrName, out string sDestChrName, out string sUserId)
    {
        sSrcChrName = "";
        sDestChrName = "";
        sUserId = "";
        if (!FdbExploreSeam.FrmCopyRcd_sub_49C09C()) return false;
        sSrcChrName = FdbExploreSeam.FrmCopyRcd_s2F0;
        sDestChrName = FdbExploreSeam.FrmCopyRcd_s2F4;
        sUserId = FdbExploreSeam.FrmCopyRcd_s2F8;
        if (FdbExploreSeam.FrmUserSoc_NewChrData(sDestChrName, 0, 0, 0, false) &&
            FdbExploreSeam.FrmDBSrv_CopyHumData(sSrcChrName, sDestChrName, sUserId))
            UiSeam.ShowMessage(sSrcChrName + " -> " + sDestChrName + " 复制成功！！！");
        return true;
    }
}

/// <summary>FDBexpl.pas:48 `var FrmFDBExplore: TFrmFDBExplore;`。</summary>
public static class FdbExploreGlobal
{
    public static FrmFDBExplore FrmFDBExplore;
}

/// <summary>FDBexpl.pas:9-45 `TFrmFDBExplore`（DFM: FDBexpl.dfm，二进制 DFM 已用 DfmToText 还原）。</summary>
public class FrmFDBExplore : Form
{
    // DFM: FrmFDBExplore Left=577 Top=292 BorderIcons=[biSystemMenu,biMinimize] BorderStyle=bsSingle
    //      Caption='人物数据管理' ClientHeight=238 ClientWidth=577 OnCreate=FormCreate OnDestroy=FormDestroy
    public ListBox ListBox1;
    public TextBox EdFind;
    public Label Label1;
    public Button BtnAdd;
    public Button BtnDel;
    public ListBox ListBox2;
    public Button BtnRebuild;
    public Button BtnBlankCount;
    public GroupBox GroupBox1;
    public Button BtnAutoClean;
    public System.Windows.Forms.Timer Timer1;
    public Button BtnCopyRcd;
    public Button BtnCopyNew;
    public CheckBox CkLv1;
    public CheckBox CkLv7;
    public CheckBox CkLv14;

    /// <summary>FDBexpl.pas:40 `SList_320: TStringList`（FormDestroy 里 Free）。</summary>
    public TStringList SList_320;

    public FrmFDBExplore()
    {
        // DFM: FrmFDBExplore Left=577 Top=292 BorderStyle=bsSingle Caption='人物数据管理' ClientHeight=238 ClientWidth=577
        Text = "人物数据管理";
        StartPosition = FormStartPosition.Manual;
        Location = new System.Drawing.Point(577, 292);
        ClientSize = new System.Drawing.Size(577, 238);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;

        // DFM: Label1 Left=16 Top=152 Width=78 Height=12 Caption='搜索人物数据:'
        Label1 = new Label { Left = 16, Top = 152, Width = 78, Height = 12, Text = "搜索人物数据:" };
        // DFM: ListBox1 Left=16 Top=8 Width=89 Height=137 ItemHeight=12 TabOrder=0
        ListBox1 = new ListBox { Left = 16, Top = 8, Width = 89, Height = 137, ItemHeight = 12, TabIndex = 0, IntegralHeight = false };
        // DFM: EdFind Left=16 Top=168 Width=121 Height=20 Hint='输入人物的名称再按回车（Enter）开始搜索'
        //      ParentShowHint=False ShowHint=True TabOrder=1 OnKeyPress=EdFindKeyPress
        EdFind = new TextBox { Left = 16, Top = 168, Width = 121, Height = 20, TabIndex = 1 };
        // DFM: BtnAdd Left=264 Top=8 Width=89 Height=25 Hint='创建一个新的人物数据' Caption='新建人物数据' TabOrder=2 OnClick=BtnAddClick
        BtnAdd = new Button { Left = 264, Top = 8, Width = 89, Height = 25, Text = "新建人物数据", TabIndex = 2 };
        // DFM: BtnDel Left=264 Top=38 Width=89 Height=25 Hint='删除当前选定的人物数据' Caption='删除人物数据' TabOrder=3 OnClick=BtnDelClick
        BtnDel = new Button { Left = 264, Top = 38, Width = 89, Height = 25, Text = "删除人物数据", TabIndex = 3 };
        // DFM: ListBox2 Left=112 Top=8 Width=137 Height=137 ItemHeight=12 TabOrder=4
        ListBox2 = new ListBox { Left = 112, Top = 8, Width = 137, Height = 137, ItemHeight = 12, TabIndex = 4, IntegralHeight = false };
        // DFM: BtnRebuild Left=264 Top=128 Width=89 Height=25 Caption='重建数据库' TabOrder=5 OnClick=BtnRebuildClick
        BtnRebuild = new Button { Left = 264, Top = 128, Width = 89, Height = 25, Text = "重建数据库", TabIndex = 5 };
        // DFM: BtnBlankCount Left=144 Top=168 Width=105 Height=21 Caption='清空搜索结果(&C)' TabOrder=6 OnClick=BtnBlankCountClick
        BtnBlankCount = new Button { Left = 144, Top = 168, Width = 105, Height = 21, Text = "清空搜索结果(&C)", TabIndex = 6 };
        // DFM: GroupBox1 Left=376 Top=7 Width=177 Height=114 Caption='自动清理数据' TabOrder=7
        GroupBox1 = new GroupBox { Left = 376, Top = 7, Width = 177, Height = 114, Text = "自动清理数据", TabIndex = 7 };
        // DFM: BtnAutoClean Left=8 Top=70 Width=97 Height=27 Caption='自动清理' TabOrder=0 OnClick=BtnAutoCleanClick
        BtnAutoClean = new Button { Left = 8, Top = 70, Width = 97, Height = 27, Text = "自动清理", TabIndex = 0 };
        // DFM: CkLv1 Left=8 Top=12 Width=153 Height=25 Caption='1级以下人物(1星期)' Checked=True State=cbChecked TabOrder=1
        CkLv1 = new CheckBox { Left = 8, Top = 12, Width = 153, Height = 25, Text = "1级以下人物(1星期)", Checked = true, TabIndex = 1 };
        // DFM: CkLv7 Left=8 Top=32 Width=145 Height=17 Caption='7级以下人物(1个月)' Checked=True State=cbChecked TabOrder=2
        CkLv7 = new CheckBox { Left = 8, Top = 32, Width = 145, Height = 17, Text = "7级以下人物(1个月)", Checked = true, TabIndex = 2 };
        // DFM: CkLv14 Left=8 Top=48 Width=145 Height=17 Caption='14级以下人物 (4个月)' Checked=True State=cbChecked TabOrder=3
        CkLv14 = new CheckBox { Left = 8, Top = 48, Width = 145, Height = 17, Text = "14级以下人物 (4个月)", Checked = true, TabIndex = 3 };
        // DFM: BtnCopyRcd Left=264 Top=68 Width=89 Height=25 Hint='将指定的人物数据复制到指定的另一个人物上' Caption='复制人物数据' TabOrder=8 OnClick=BtnCopyRcdClick
        BtnCopyRcd = new Button { Left = 264, Top = 68, Width = 89, Height = 25, Text = "复制人物数据", TabIndex = 8 };
        // DFM: BtnCopyNew Left=264 Top=98 Width=89 Height=25 Hint='新建一个人物数据，并将指定的人物数据复制此人物中' Caption='复制到新人物' TabOrder=9 OnClick=BtnCopyNewClick
        BtnCopyNew = new Button { Left = 264, Top = 98, Width = 89, Height = 25, Text = "复制到新人物", TabIndex = 9 };
        // DFM: Timer1 Enabled=False Interval=3000 OnTimer=Timer1Timer Left=352 Top=200
        Timer1 = new System.Windows.Forms.Timer { Enabled = false, Interval = 3000 };

        GroupBox1.Controls.Add(BtnAutoClean);
        GroupBox1.Controls.Add(CkLv1);
        GroupBox1.Controls.Add(CkLv7);
        GroupBox1.Controls.Add(CkLv14);

        Controls.Add(Label1);
        Controls.Add(ListBox1);
        Controls.Add(EdFind);
        Controls.Add(BtnAdd);
        Controls.Add(BtnDel);
        Controls.Add(ListBox2);
        Controls.Add(BtnRebuild);
        Controls.Add(BtnBlankCount);
        Controls.Add(GroupBox1);
        Controls.Add(BtnCopyRcd);
        Controls.Add(BtnCopyNew);

        // DFM: FrmFDBExplore OnCreate=FormCreate / OnDestroy=FormDestroy
        Load += (s, e) => FormCreate(s, e);
        FormClosed += (s, e) => FormDestroy(s, e);
        EdFind.KeyPress += (s, e) =>
        {
            if (e.KeyChar == '\r') EdFindKeyPress(s, e);
        };
        BtnDel.Click += (s, e) => BtnDelClick(s, e);
        BtnRebuild.Click += (s, e) => BtnRebuildClick(s, e);
        BtnBlankCount.Click += (s, e) => BtnBlankCountClick(s, e);
        BtnAdd.Click += (s, e) => BtnAddClick(s, e);
        BtnAutoClean.Click += (s, e) => BtnAutoCleanClick(s, e);
        Timer1.Tick += (s, e) => Timer1Timer(s, e);
        BtnCopyRcd.Click += (s, e) => BtnCopyRcdClick(s, e);
        BtnCopyNew.Click += (s, e) => BtnCopyNewClick(s, e);
    }

    /// <summary>FDBexpl.pas:136-144 `FormCreate`。</summary>
    public void FormCreate(object Sender, EventArgs e)
    {
        Timer1.Interval = FdbExploreSeam.dwInterval;
        Timer1.Enabled = true;
        SList_320 = new TStringList();
        FdbExploreSeam.g_nClearIndex = 0;
        FdbExploreSeam.g_nClearCount = 0;
        FdbExploreSeam.g_nClearItemIndexCount = 0;
    }

    /// <summary>FDBexpl.pas:364-367 `FormDestroy`。</summary>
    public void FormDestroy(object Sender, EventArgs e)
    {
        SList_320?.Clear();      // SList_320.Free
    }

    /// <summary>FDBexpl.pas:57-78 `EdFindKeyPress`。</summary>
    public void EdFindKeyPress(object Sender, KeyPressEventArgs e)
    {
        var objs = new List<int>();
        var list1 = new TStringList();
        FdbExploreLogic.EdFindKeyPress(FdbExploreSeam.g_HumDataDB, e.KeyChar, EdFind.Text, list1, objs);
        ListBox1.Items.Clear();
        ListBox2.Items.Clear();
        for (int i = 0; i < list1.Count; i++)
        {
            ListBox1.Items.Add(list1[i]);
            ListBox2.Items.Add(DelphiRTL.IntToStr(i < objs.Count ? objs[i] : 0));
        }
    }

    /// <summary>FDBexpl.pas:80-95 `BtnDelClick`。</summary>
    public void BtnDelClick(object Sender, EventArgs e)
    {
        int recordIndex = ListBox1.SelectedIndex >= 0 && ListBox1.SelectedIndex < ListBox1.Items.Count
            ? DelphiRTL.StrToIntDef(Convert.ToString(ListBox2.Items[ListBox1.SelectedIndex]), 0)
            : 0;
        FdbExploreLogic.BtnDelClick(FdbExploreSeam.g_HumDataDB, ListBox1.SelectedIndex, recordIndex);
    }

    /// <summary>FDBexpl.pas:97-104 `BtnRebuildClick`。</summary>
    public void BtnRebuildClick(object Sender, EventArgs e)
    {
        FdbExploreLogic.BtnRebuildClick(FdbExploreSeam.g_HumDataDB);
    }

    /// <summary>FDBexpl.pas:106-110 `BtnBlankCountClick`：清空两个列表。</summary>
    public void BtnBlankCountClick(object Sender, EventArgs e)
    {
        ListBox1.Items.Clear();
        ListBox2.Items.Clear();
    }

    /// <summary>FDBexpl.pas:112-118 `BtnAddClick`。</summary>
    public void BtnAddClick(object Sender, EventArgs e)
    {
        FdbExploreLogic.BtnAddClick();
    }

    /// <summary>FDBexpl.pas:146-151 `BtnAutoCleanClick`。</summary>
    public void BtnAutoCleanClick(object Sender, EventArgs e)
    {
        BtnAutoClean.Text = FdbExploreLogic.BtnAutoCleanClick();
    }

    /// <summary>FDBexpl.pas:153-248 `Timer1Timer`。</summary>
    public void Timer1Timer(object Sender, EventArgs e)
    {
        FdbExploreLogic.Timer1Timer(FdbExploreSeam.g_HumDataDB, CkLv1.Checked, CkLv7.Checked, CkLv14.Checked);
    }

    /// <summary>FDBexpl.pas:336-347 `BtnCopyRcdClick`。</summary>
    public void BtnCopyRcdClick(object Sender, EventArgs e)
    {
        FdbExploreLogic.BtnCopyRcdClick(out _, out _, out _);
    }

    /// <summary>FDBexpl.pas:349-361 `BtnCopyNewClick`。</summary>
    public void BtnCopyNewClick(object Sender, EventArgs e)
    {
        FdbExploreLogic.BtnCopyNewClick(out _, out _, out _);
    }
}
