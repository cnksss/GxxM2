using System;
using System.Collections.Generic;
using System.IO;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.Client.GUI.GameConfig;

/// <summary>
/// FilterItems.pas 1:1 移植（全文 1-492 行）：
/// 客户端"过滤物品提示/捡取/显示名"数据库——加载系统表 + 用户表、增删查、导入导出、落盘。
///
/// 指针语义：原文 <c>m_FileItemList:TList</c> / <c>m_ShowItemList:TList</c> 里放的是
/// <c>pTShowItem</c>（New/Dispose 出来的记录指针）。托管侧没有裸指针，
/// 这里用 <c>List&lt;TShowItem&gt;</c> 且元素是 **class 引用**，
/// 因此 <c>m_ShowItemList.Items[I]</c> 与 <c>Find()</c> 返回的是**同一个对象**
/// （对应原文字段级原地修改可见的行为），Dispose 对应"从链表里移除、交由 GC"。
/// </summary>
public class TFileItemDB
{
    /// <summary>原文 <c>m_FileItemList:TList;</c>（FilterItems.pas:12）</summary>
    public readonly List<TShowItem> m_FileItemList = new List<TShowItem>();

    /// <summary>原文 <c>m_ShowItemList:TList; // THashedStringlist;//TStringList;</c>（FilterItems.pas:13）</summary>
    public readonly List<TShowItem> m_ShowItemList = new List<TShowItem>();

    /// <summary>原文 <c>constructor TFileItemDB.Create();</c>（FilterItems.pas:109-113）</summary>
    public TFileItemDB()
    {
        // 原文：m_FileItemList := TList.Create; m_ShowItemList := TList.Create;
    }

    /// <summary>
    /// 原文 <c>destructor TFileItemDB.Destroy;</c>（FilterItems.pas:115-135）。
    /// 原文先 Dispose m_ShowItemList 的每一项，再 Dispose m_FileItemList 的每一项
    /// （中间那段"去内存泄露"的 TODO 注释块**原文如此**，此处照抄为注释）。
    /// </summary>
    ~TFileItemDB()
    {
        // 原文如此（FilterItems.pas:124-128）：下面这段曾被注释掉的 TODO 与真正执行的循环内容完全相同
        // { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-7-17】
        //     for I := 0 to m_FileItemList.Count - 1 do begin
        //       Dispose(m_FileItemList.Items[I]);
        //     end;
        // }
        m_ShowItemList.Clear();   // 原文 Dispose(pTShowItem(m_ShowItemList.Items[I]))
        m_FileItemList.Clear();   // 原文 Dispose(pTShowItem(m_FileItemList.Items[I]))
        // 原文 inherited;（调用 TObject.Destroy）
    }

    // ================================================================================
    // FilterItems.pas:55-107  单元级函数
    // ================================================================================

    /// <summary>原文 FilterItems.pas:55-61。</summary>
    public static string GetSelectString(bool bo)
    {
        string Result;
        if (bo)
            Result = "√";
        else
            Result = "";
        return Result;
    }

    /// <summary>原文 FilterItems.pas:63-75。</summary>
    public static string GetItemTypeName(TItemType ItemType)
    {
        string Result = "";
        switch (ItemType)
        {
            case TItemType.i_Other: Result = "其它类"; break;
            case TItemType.i_HPMPDurg: Result = "药品类"; break;
            case TItemType.i_Dress: Result = "服装类"; break;
            case TItemType.i_Weapon: Result = "武器类"; break;
            case TItemType.i_Jewelry: Result = "首饰类"; break;
            case TItemType.i_Decoration: Result = "饰品类"; break;
            case TItemType.i_Decorate: Result = "装饰类"; break;
            case TItemType.i_diy: Result = "自定类"; break;
            // 注意：**原文没有 i_All 分支**，i_All 落空 → Result 保持 ''
        }
        return Result;
    }

    /// <summary>
    /// 原文 FilterItems.pas:77-88。
    /// 语义要点：先置 <c>i_diy</c> 再逐条独立 if 覆盖（顺序求值，非 case），
    /// 因此任何不匹配的分类名都返回 <c>i_diy</c>。
    /// </summary>
    public static TItemType GetItemType(string ItemType)
    {
        TItemType Result = TItemType.i_diy;
        if (ItemType == "其它类") Result = TItemType.i_Other;
        if (ItemType == "药品类") Result = TItemType.i_HPMPDurg;
        if (ItemType == "服装类") Result = TItemType.i_Dress;
        if (ItemType == "武器类") Result = TItemType.i_Weapon;
        if (ItemType == "首饰类") Result = TItemType.i_Jewelry;
        if (ItemType == "饰品类") Result = TItemType.i_Decoration;
        if (ItemType == "装饰类") Result = TItemType.i_Decorate;
        if (ItemType == "自定类") Result = TItemType.i_diy;
        return Result;
    }

    /// <summary>
    /// 原文 FilterItems.pas:90-107。
    /// 语义要点：<c>g_MySelf = nil</c> 时**在调用 GetNextDirection 之前**就 Exit 返回空串。
    /// </summary>
    public static string GetActorDir(int nX, int nY)
    {
        string Result = "";
        if (ConfigShareSeam.g_MySelf == null) return Result;
        int ndir = NextDirectionSeam.GetNextDirection(ConfigShareSeam.g_MySelf.m_nCurrX, ConfigShareSeam.g_MySelf.m_nCurrY, nX, nY);
        switch (ndir)
        {
            case 0: Result = "↑"; break;
            case 1: Result = "↗"; break;
            case 2: Result = "→"; break;
            case 3: Result = "↘"; break;
            case 4: Result = "↓"; break;
            case 5: Result = "↙"; break;
            case 6: Result = "←"; break;
            case 7: Result = "↖"; break;
        }
        return Result;
    }

    // ================================================================================
    // FilterItems.pas:139-171  LoadFormFile
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:139-171。
    /// 语义要点：文件名 = <c>g_sSelfFilePath + 'Config\' + g_sPlugServerName + '.' + g_sPlugUserName + '.ItemFilter.dat'</c>；
    /// <c>g_MySelf &lt;&gt; nil</c> 时先刷新 <c>g_sPlugUserName</c>；
    /// 文件不存在则**静默不加载**；LoadFromFile 抛异常被 <c>except</c> 吞掉（外层 try/finally 仍释放 LoadList）。
    /// </summary>
    public void LoadFormFile()
    {
        string sDirectory = SelfFilePathSeam.g_sSelfFilePath + "Config\\";
        if (!Directory.Exists(sDirectory)) Directory.CreateDirectory(sDirectory);

        if (ConfigShareSeam.g_MySelf != null)
        {
            ConfigShareGlobal.g_sPlugUserName = ConfigSeams.ProcessFileNameSpecialChar(ConfigShareSeam.g_MySelf.m_sUserName);
        }

        // 原文如此（FilterItems.pas:153）：这里有一个**单独一行的空语句 `;`**
        string sFileName = sDirectory + ConfigShareGlobal.g_sPlugServerName + "." + ConfigShareGlobal.g_sPlugUserName + ".ItemFilter.dat";

        // sFileName := 'Config\%s.%s.ItemFilter.dat', [g_sServerName, sUserName]);

        // g_ClientFunction.AddChatBoardString(PChar('LoadFormFile:' + sFileName), clyellow, clBlue);

        if (File.Exists(sFileName))
        {
            var LoadList = new TStringList();
            try
            {
                try
                {
                    LoadList.LoadFromFile(sFileName);
                    LoadFormList(LoadList, false);
                }
                catch
                {
                    // 原文 except（空）
                }
            }
            finally
            {
                // 原文 LoadList.Free;
            }
        }
    }

    // ================================================================================
    // FilterItems.pas:173-244  LoadFormList
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:173-244。
    /// 语义要点（全部照抄）：
    /// 1) 每行 <c>Trim</c>；空行与首字符 ';' 的行跳过；
    /// 2) 连续 7 次 <c>GetValidStr3(..., [',', #9])</c> 依次取出
    ///    ItemType / ItemName / Hint / PickUp / ShowName / ShowSpecial / AutoMove；
    /// 3) <c>nItemType := StrToIntDef(sItemType, -1)</c>，必须同时满足
    ///    <c>sItemName &lt;&gt; ''</c> 且 <c>Low(TItemType) &lt;= nItemType &lt;= High(TItemType)</c>；
    /// 4) <c>Find</c> 命中 → 原地改 ItemType/sItemType/sItemName，
    ///    **仅当 <c>FromSystem=False</c></c> 才覆盖 5 个勾选位**，最后无条件写 <c>boFromSystem</c>；
    /// 5) 未命中 → <c>New</c> 一条，5 个勾选位**无条件**按 <c>= '1'</c> 判定，
    ///    <c>Add</c> 进 m_ShowItemList，再拷贝一份进 m_FileItemList；
    /// 6) **原文开头的"清空两个链表"整段是被 (* *) 注释掉的**（FilterItems.pas:180-193），
    ///    因此重复调用会把 m_FileItemList 越堆越长（原文如此 → 见 SaveToFile 会写出重复行）。
    /// </summary>
    public void LoadFormList(TStringList StringList, bool FromSystem)
    {
        /*
        for I := 0 to m_ShowItemList.Count - 1 do
        begin
          Dispose(pTShowItem(m_ShowItemList.Items[I]));
        end;
        m_ShowItemList.Clear;

        { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-7-17】}
        for I := 0 to m_FileItemList.Count - 1 do
        begin
          Dispose(pTShowItem(m_FileItemList.Items[I]));
        end;
        m_FileItemList.Clear;
        */

        for (int nIndex = 0; nIndex <= StringList.Count - 1; nIndex++)
        {
            string sLineText = DelphiRTL.Trim(StringList[nIndex]);
            if (sLineText == "") continue;
            if ((sLineText != "") && (sLineText[0] == ';')) continue;   // 原文 sLineText[1]（Delphi 1-based）
            string sItemType = "", sItemName = "", sHint = "", sPickUp = "", sShowName = "", sShowSpecial = "", sAutoMove = "";
            char[] div = { ',', '\t' };
            sLineText = HUtil32.GetValidStr3(sLineText, ref sItemType, div);
            sLineText = HUtil32.GetValidStr3(sLineText, ref sItemName, div);
            sLineText = HUtil32.GetValidStr3(sLineText, ref sHint, div);
            sLineText = HUtil32.GetValidStr3(sLineText, ref sPickUp, div);
            sLineText = HUtil32.GetValidStr3(sLineText, ref sShowName, div);
            sLineText = HUtil32.GetValidStr3(sLineText, ref sShowSpecial, div);
            sLineText = HUtil32.GetValidStr3(sLineText, ref sAutoMove, div);
            int nItemType = DelphiRTL.StrToIntDef(sItemType, -1);
            if ((sItemName != "") &&
                (nItemType >= (int)TItemType.i_All /*Low(TItemType)*/) &&
                (nItemType <= (int)TItemType.i_diy /*High(TItemType)*/))
            {
                TShowItem ShowItem = Find(sItemName);
                if (ShowItem != null)
                {
                    ShowItem.ItemType = (TItemType)nItemType;
                    ShowItem.sItemType = GetItemTypeName(ShowItem.ItemType);
                    ShowItem.sItemName = sItemName;
                    if (!FromSystem)
                    {
                        ShowItem.boHintMsg = (byte)(sHint == "1" ? 1 : 0);
                        ShowItem.boPickup = (byte)(sPickUp == "1" ? 1 : 0);
                        ShowItem.boShowName = (byte)(sShowName == "1" ? 1 : 0);
                        ShowItem.boShowSpecial = (byte)(sShowSpecial == "1" ? 1 : 0);
                        ShowItem.boAutoMove = (byte)(sAutoMove == "1" ? 1 : 0);
                    }
                    ShowItem.boFromSystem = (byte)(FromSystem ? 1 : 0);
                }
                else
                {
                    // 没有找到物品则是自定义物品 piaoyun 2013-09-09
                    ShowItem = new TShowItem();
                    ShowItem.ItemType = (TItemType)nItemType;
                    ShowItem.sItemType = GetItemTypeName(ShowItem.ItemType);
                    ShowItem.sItemName = sItemName;
                    ShowItem.boHintMsg = (byte)(sHint == "1" ? 1 : 0);
                    ShowItem.boPickup = (byte)(sPickUp == "1" ? 1 : 0);
                    ShowItem.boShowName = (byte)(sShowName == "1" ? 1 : 0);
                    ShowItem.boShowSpecial = (byte)(sShowSpecial == "1" ? 1 : 0);
                    ShowItem.boAutoMove = (byte)(sAutoMove == "1" ? 1 : 0);
                    ShowItem.boFromSystem = (byte)(FromSystem ? 1 : 0);
                    //m_ShowItemList.Add(ShowItem);
                    Add(ShowItem);
                    TShowItem FileItem = ShowItem.Clone();   // 原文 New(FileItem); FileItem^ := ShowItem^;
                    m_FileItemList.Add(FileItem);
                }
            }
        }
    }

    // ================================================================================
    // FilterItems.pas:246-252  BackUp
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:246-252。
    /// **原文缺陷（照抄）**：循环边界取 <c>m_FileItemList.Count</c>，却写 <c>m_ShowItemList.Items[I]</c>；
    /// 两个链表长度不一致时会越界（Delphi 侧表现为访问违例）。
    /// 托管侧为了不掩盖该缺陷，用 <c>m_ShowItemList[I]</c> 直接索引（越界同样抛 IndexOutOfRange）。
    /// </summary>
    public void BackUp()
    {
        for (int I = 0; I <= m_FileItemList.Count - 1; I++)
            m_ShowItemList[I].AssignFrom(m_FileItemList[I]);
    }

    // ================================================================================
    // FilterItems.pas:256-293  SaveToFile
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:256-293。
    /// 落盘格式（**逐字，测试断言完整文本**）：
    /// <c>Format('%d,%s,%d,%d,%d,%d,%d', [Integer(ItemType), sItemName, BoolToInt(boHintMsg),
    /// BoolToInt(boPickup), BoolToInt(boShowName), BoolToInt(boShowSpecial), BoolToInt(boAutoMove)])</c>，
    /// 每行一条，写入 <c>g_sSelfFilePath + 'Config\' + Format('%s.%s.ItemFilter.dat', [g_sPlugServerName, g_sPlugUserName])</c>；
    /// <c>g_MySelf = nil</c> → 直接 Exit，**不落盘**。
    /// 注意：循环遍历的是 m_FileItemList，但取的是 <c>Find(FileItem.sItemName)</c> 的结果，
    /// 所以写出的永远是 m_ShowItemList 里的**当前**状态。
    /// </summary>
    public void SaveToFile()
    {
        if (ConfigShareSeam.g_MySelf == null) return;
        string sDirectory = SelfFilePathSeam.g_sSelfFilePath + "Config\\";
        if (!Directory.Exists(sDirectory)) Directory.CreateDirectory(sDirectory);
        if (ConfigShareSeam.g_MySelf != null)
        {
            ConfigShareGlobal.g_sPlugUserName = ConfigSeams.ProcessFileNameSpecialChar(ConfigShareSeam.g_MySelf.m_sUserName);
        }

        string sFileName = sDirectory + DelphiRTL.Format("%s.%s.ItemFilter.dat",
            ConfigShareGlobal.g_sPlugServerName, ConfigShareGlobal.g_sPlugUserName);

        var SaveList = new TStringList();
        for (int I = 0; I <= m_FileItemList.Count - 1; I++)
        {
            TShowItem FileItem = m_FileItemList[I];
            TShowItem ShowItem = Find(FileItem.sItemName);
            // chongchong 2018-02-12 去劫持
            // {$I VMProtectBegin.inc}   ← 托管侧无 VMProtect，按不移植项去除
            if (ShowItem != null)
            {
                SaveList.Add(FormatItemLine(ShowItem));
            }
            // {$I VMProtectEnd.inc}
        }
        try
        {
            SaveList.SaveToFile(sFileName);
        }
        catch
        {
            // 原文 except（空）
        }
    }

    /// <summary>原文多处重复出现的单行格式（SaveToFile / ExportToStrings 各一次，逐字一致）。</summary>
    private static string FormatItemLine(TShowItem ShowItem)
        => DelphiRTL.Format("%d,%s,%d,%d,%d,%d,%d",
            (int)ShowItem.ItemType, ShowItem.sItemName,
            HUtil32.BoolToInt(ShowItem.boHintMsg != 0), HUtil32.BoolToInt(ShowItem.boPickup != 0),
            HUtil32.BoolToInt(ShowItem.boShowName != 0),
            HUtil32.BoolToInt(ShowItem.boShowSpecial != 0), HUtil32.BoolToInt(ShowItem.boAutoMove != 0));

    // ================================================================================
    // FilterItems.pas:295-322  ImportFormFile / ExportToFile
    // ================================================================================

    /// <summary>原文 FilterItems.pas:295-308（文件不存在则静默返回）。</summary>
    public void ImportFormFile(string FileName)
    {
        if (File.Exists(FileName))
        {
            var LoadList = new TStringList();
            try
            {
                LoadList.LoadFromFile(FileName);
                ImportFormStrings(LoadList);
            }
            finally
            {
                // 原文 LoadList.Free;
            }
        }
    }

    /// <summary>原文 FilterItems.pas:310-322（SaveToFile 的异常同样被吞）。</summary>
    public void ExportToFile(string FileName)
    {
        var SaveList = new TStringList();
        ExportToStrings(SaveList);
        try
        {
            SaveList.SaveToFile(FileName);
        }
        catch
        {
            // 原文 except（空）
        }
    }

    // ================================================================================
    // FilterItems.pas:324-362  ImportFormStrings / ExportToStrings
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:324-342。
    /// 先清空两个链表（Dispose 后 Clear），再 <c>LoadFormList(StringList, False)</c>；
    /// 整段包在 <c>try..except</c> 里，异常被吞。
    /// </summary>
    public void ImportFormStrings(TStringList StringList)
    {
        try
        {
            m_ShowItemList.Clear();
            m_FileItemList.Clear();
            LoadFormList(StringList, false);
        }
        catch
        {
            // 原文 except（空）
        }
    }

    /// <summary>原文 FilterItems.pas:344-362（遍历 m_FileItemList，取 Find 的当前状态，格式与 SaveToFile 相同）。</summary>
    public void ExportToStrings(TStringList StringList)
    {
        for (int I = 0; I <= m_FileItemList.Count - 1; I++)
        {
            TShowItem FileItem = m_FileItemList[I];
            TShowItem ShowItem = Find(FileItem.sItemName);
            if (ShowItem != null)
            {
                // chongchong 2018-02-12 去劫持
                // {$I VMProtectBegin.inc}
                StringList.Add(FormatItemLine(ShowItem));
                // {$I VMProtectEnd.inc}
            }
        }
    }

    // ================================================================================
    // FilterItems.pas:364-433  Get / Add / Del / Find
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:364-376。
    /// <c>sItemType = '(全部分类)'</c> 时全量返回，否则 <c>ShowItem.sItemType = sItemType</c>（**大小写敏感**）。
    /// </summary>
    public void Get(string sItemType, List<TShowItem> ItemList)
    {
        if (ItemList == null) return;
        for (int I = 0; I <= m_ShowItemList.Count - 1; I++)
        {
            TShowItem ShowItem = m_ShowItemList[I];
            if ((sItemType == "(全部分类)") || (ShowItem.sItemType == sItemType))
            {
                ItemList.Add(ShowItem);
            }
        }
    }

    /// <summary>原文 FilterItems.pas:378-390（<c>ItemType = i_All</c> 时全量返回）。</summary>
    public void Get(TItemType ItemType, List<TShowItem> ItemList)
    {
        if (ItemList == null) return;
        for (int I = 0; I <= m_ShowItemList.Count - 1; I++)
        {
            TShowItem ShowItem = m_ShowItemList[I];
            if ((ItemType == TItemType.i_All) || (ShowItem.ItemType == ItemType))
            {
                ItemList.Add(ShowItem);
            }
        }
    }

    /// <summary>原文 FilterItems.pas:392-398（重名（CompareText）则不加）。</summary>
    public bool Add(TShowItem ShowItem)
    {
        bool Result = false;
        if (Find(ShowItem.sItemName) != null) return Result;
        m_ShowItemList.Add(ShowItem);
        Result = true;
        return Result;
    }

    /// <summary>
    /// 原文 FilterItems.pas:400-418。
    /// 语义要点：只删 <c>boFromSystem = False</c> 的条目；名字用 <c>CompareText</c>（大小写不敏感）；
    /// 命中后**按同一个下标 I 同时删 m_ShowItemList 与 m_FileItemList** 再 Break。
    /// </summary>
    public bool Del(string sItemName)
    {
        bool Result = false;
        for (int I = 0; I <= m_ShowItemList.Count - 1; I++)
        {
            TShowItem ShowItem = m_ShowItemList[I];
            if ((ShowItem.boFromSystem == 0) && (ConfigShare.SameText(ShowItem.sItemName, sItemName)))
            {
                m_ShowItemList.RemoveAt(I);
                // 可能m_FileItemList链表需要再遍历一次 piaoyun 2013-09-10
                m_FileItemList.RemoveAt(I);
                Result = true;
                break;
            }
        }
        return Result;
    }

    /// <summary>原文 FilterItems.pas:420-433（CompareText 大小写不敏感，返回第一个命中项）。</summary>
    public TShowItem Find(string sItemName)
    {
        TShowItem Result = null;
        for (int I = 0; I <= m_ShowItemList.Count - 1; I++)
        {
            TShowItem ShowItem = m_ShowItemList[I];
            if (ConfigShare.SameText(ShowItem.sItemName, sItemName))
            {
                Result = ShowItem;
                break;
            }
        }
        return Result;
    }

    // ================================================================================
    // FilterItems.pas:435-484  Hint
    // ================================================================================

    /// <summary>
    /// 原文 FilterItems.pas:435-484。
    /// 条件：<c>g_MySelf &lt;&gt; nil</c> 且 <c>ShowItem &lt;&gt; nil</c> 且 <c>ShowItem.boHintMsg</c> 为真。
    /// 文本：<c>'发现[' + sItemName + ']，方位:' + sPosition + sDir + '，坐标:(' + Format('%d,%d', [nX, nY]) + ').'</c>；
    /// 颜色 <c>$00FFFF</c>、背景 <c>clBlue</c>（原样传整数）。
    /// **原文缺陷（照抄）**：<c>case GetNextDirection(...)</c> 对 0..7 之外的返回值
    /// 不会给 sPosition/sDir 赋值，此时用的是**上一轮的局部变量值**——
    /// Delphi 局部 string 未初始化即 ''，托管侧同样以 '' 初始化。
    /// </summary>
    public void Hint(string sItemName, int nX, int nY)
    {
        TShowItem ShowItem = Find(sItemName);
        if ((ConfigShareSeam.g_MySelf != null) && (ShowItem != null) && ShowItem.boHintMsg != 0)
        {
            int nCurrX = ConfigShareSeam.g_MySelf.m_nCurrX;
            int nCurrY = ConfigShareSeam.g_MySelf.m_nCurrY;
            string sHint = "", sPosition = "", sDir = "";

            switch (NextDirectionSeam.GetNextDirection(nCurrX, nCurrY, nX, nY))
            {
                case 0:
                    sPosition = "上";
                    sDir = "↑";
                    break;
                case 1:
                    sPosition = "右上";
                    sDir = "↗";
                    break;

                case 2:
                    sPosition = "右";
                    sDir = "→";
                    break;
                case 3:
                    sPosition = "右下";
                    sDir = "↘";
                    break;
                case 4:
                    sPosition = "下";
                    sDir = "↓";
                    break;
                case 5:
                    sPosition = "左下";
                    sDir = "↙";
                    break;
                case 6:
                    sPosition = "左";
                    sDir = "←";
                    break;
                case 7:
                    sPosition = "左上";
                    sDir = "↖";
                    break;
            }
            sHint = "发现[" + sItemName + "]，方位:" + sPosition + sDir + "，坐标:(" + DelphiRTL.Format("%d,%d", nX, nY) + ").";
            // 原文 DScreen.AddChatBoardString(sHint, $00FFFF, clBlue)
            //   $00FFFF 是 Delphi TColor（BGR 布局）的"黄"，clBlue = $FF0000。
            ChatBoardSeam.AddChatBoardString(sHint, 0x0000FFFF, ClBlue);
        }
    }

    /// <summary>原文 Graphics.clBlue = $FF0000（Delphi TColor 为 BGR 布局）。</summary>
    public const int ClBlue = 0x00FF0000;
}

/// <summary>
/// FilterItems.pas:38-42 的单元级全局变量（→ public static class XxxGlobal，见 §3.3）。
/// </summary>
public static class FilterItemsGlobal
{
    /// <summary>原文 <c>g_FileItemDB:TFileItemDB;</c>，在 unit initialization 段 Create（FilterItems.pas:486-487）。</summary>
    public static readonly TFileItemDB g_FileItemDB = new TFileItemDB();

    /// <summary>原文 <c>g_IsClientPickItemsChanged:Boolean = False;</c> → byte，与 0 比较。</summary>
    public static byte g_IsClientPickItemsChanged;

    /// <summary>原文 <c>g_UploadPickItemsTick:LongWord = 0;</c></summary>
    public static uint g_UploadPickItemsTick;

    /// <summary>原文 <c>g_UploadPickItemsHash:LongWord = 0;</c></summary>
    public static uint g_UploadPickItemsHash;
}

/// <summary>
/// 接缝：MShare.pas:1273 <c>g_sSelfFilePath:string</c>（客户端自身目录，用于拼 Config\ 路径）。
/// 未移植，由宿主/测试注入。
/// </summary>
public static class SelfFilePathSeam
{
    /// <summary>接缝：待 MShare.pas 移植后接入。</summary>
    public static string g_sSelfFilePath = "";
}
