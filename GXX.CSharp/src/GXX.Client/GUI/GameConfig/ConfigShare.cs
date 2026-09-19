using System;
using System.Windows.Forms;
using GXX.Client.GUI.GameConfig.Seams;
using GXX.Core.Rtl;

namespace GXX.Client.GUI.GameConfig;

/// <summary>
/// ConfigShare.pas 1:1 移植（全文 1-992 行）：
/// 配置共享层——快捷键记录/全局快捷键表、GetKeyDownStr 按键名生成，
/// 以及人物/英雄包裹内"解包药/绑定药/特殊药/技能书"的查找函数族。
///
/// 说明：原文的 <c>pTShortcutKey = ^TShortcutKey</c>（指向记录的指针）在托管侧不存在，
/// TShortcutKey 用 struct，<c>p^</c> 语义由 struct 引用传递表达。
/// 原文 <c>g_ShortcutKeys:TShortcutKeys = array[0..15] of TShortcutKey</c> 为空初始化
/// （Delphi 全局 var 零初始化），C# 侧用 16 个默认构造的 struct 表达同一零值。
/// </summary>
public struct TShortcutKey
{
    /// <summary>原文 Use:Boolean（Delphi Boolean 1 字节）→ byte，与 0 比较。</summary>
    public byte Use;
    /// <summary>原文 Key:Word → ushort。</summary>
    public ushort Key;
    /// <summary>原文 Shift:TShiftState。</summary>
    public DelphiShiftState Shift;
}

/// <summary>原文 <c>TShortcutKeys = array[0..15] of TShortcutKey</c>。</summary>
public sealed class TShortcutKeys
{
    /// <summary>原文 <c>array[0..15]</c>，共 16 项。</summary>
    public const int Low = 0;
    public const int High = 15;
    private readonly TShortcutKey[] _items = new TShortcutKey[16];
    /// <summary>原文下标访问 <c>g_ShortcutKeys[I]</c>。</summary>
    public TShortcutKey this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
    public int Length => _items.Length;
}

/// <summary>
/// ConfigShare.pas:28-31 的单元级全局变量。
/// 按工程规范（转换开发文档 §3.3）：Delphi unit-level var → <c>public static class XxxGlobal</c>。
/// </summary>
public static class ConfigShareGlobal
{
    /// <summary>原文 <c>g_sPlugServerName:string = '';</c>（ConfigShare.pas:29）</summary>
    public static string g_sPlugServerName = "";
    /// <summary>原文 <c>g_sPlugUserName:string = '';</c>（ConfigShare.pas:30）</summary>
    public static string g_sPlugUserName = "";
    /// <summary>原文 <c>g_ShortcutKeys:TShortcutKeys;</c>（ConfigShare.pas:31）</summary>
    public static readonly TShortcutKeys g_ShortcutKeys = new TShortcutKeys();
}

/// <summary>
/// ConfigShare.pas 的函数族（原文为单元级 function，C# 侧收敛为静态类，方法名逐字保留）。
/// </summary>
public static class ConfigShare
{
    // ================================================================================
    // ConfigShare.pas:83-195  GetKeyDownStr（含嵌套 function GetKey）
    // ================================================================================

    /// <summary>
    /// 原文 ConfigShare.pas:89-149 的嵌套 <c>function GetKey(Key:Word):string</c>。
    /// 保留原文的**顺序 if（非 else if）链**：后面的分支可以覆盖前面的赋值，
    /// 例如 Key=VK_TAB(9) 先被 <c>(Key&gt;=48)and(Key&lt;=57)</c> 排除，命中 <c>Key=VK_TAB</c> → 'Tab'，
    /// 随后又命中 <c>(Key&gt;=8)and(Key&lt;=46)</c> 的 <c>9:'Tab'</c>（结果相同，原文如此）。
    /// </summary>
    public static string GetKey(ushort Key)
    {
        string Result = "";
        if (((Key >= 48) && (Key <= 57)) || ((Key >= 65) && (Key <= 90)) ||
            ((Key >= 96) && (Key <= 105)))
        {
            // 原文：Result := Chr(Key)（Chr 为 AnsiChar，托管侧 char）
            Result = ((char)Key).ToString();
        }
        if (Key == (ushort)Keys.Tab)
        {
            Result = "Tab";
        }
        if (Key == (ushort)Keys.Scroll)
        {
            Result = "Scroll";
        }
        if (Key >= (ushort)Keys.F1 && Key <= (ushort)Keys.F12)
        {
            switch (Key)
            {
                case (ushort)Keys.F1: Result = "F1"; break;
                case (ushort)Keys.F2: Result = "F2"; break;
                case (ushort)Keys.F3: Result = "F3"; break;
                case (ushort)Keys.F4: Result = "F4"; break;
                case (ushort)Keys.F5: Result = "F5"; break;
                case (ushort)Keys.F6: Result = "F6"; break;
                case (ushort)Keys.F7: Result = "F7"; break;
                case (ushort)Keys.F8: Result = "F8"; break;
                case (ushort)Keys.F9: Result = "F9"; break;
                case (ushort)Keys.F10: Result = "F10"; break;
                case (ushort)Keys.F11: Result = "F11"; break;
                case (ushort)Keys.F12: Result = "F12"; break;
            }
        }
        if ((Key >= 186) && (Key <= 222))
        {// 其他键
            switch (Key)
            {
                case 186: Result = ";"; break;
                case 187: Result = "="; break;
                case 188: Result = ","; break;
                case 189: Result = "-"; break;
                case 190: Result = "."; break;
                case 191: Result = "/"; break;
                case 192: Result = "`"; break;
                case 219: Result = "["; break;
                case 220: Result = "\\"; break;
                case 221: Result = "]"; break;
                case 222: Result = ((char)27).ToString(); break; // 原文 Char(27)
            }
        }

        if ((Key >= 8) && (Key <= 46))
        {// 方向键
            switch (Key)
            {
                case 8: Result = "退格"; break;
                case 9: Result = "Tab"; break;
                case 13: Result = "Enter"; break;
                case 32: Result = "空格"; break;
                case 33: Result = "PageUp"; break;
                case 34: Result = "PageDown"; break;
                case 35: Result = "End"; break;
                case 36: Result = "Home"; break;
                case 45: Result = "Insert"; break;
                case 46: Result = "Delete"; break;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 ConfigShare.pas:89-195 <c>GetKeyDownStr(Key:Word; Shift:TShiftState; IncludeFN:Boolean):string</c>。
    /// 语义要点（逐字保留）：
    /// 1) 只有 Key 不在 {VK_MENU, VK_CONTROL, VK_SHIFT, VK_TAB, VK_SCROLL} 且落在
    ///    数字/字母/小键盘/F 键(仅 IncludeFN)/OEM(186..222) 之一时，才加修饰前缀；
    /// 2) 前缀是**互斥 if-else 链**，判定顺序 Shift→Alt→Ctrl→Left→Right→Middle→Double；
    /// 3) **无论如何最后都执行 <c>Result := Result + GetKey(Key)</c>**——
    ///    即使是纯修饰键（VK_SHIFT 等）也会返回 GetKey 的结果（通常为空串）。
    /// </summary>
    public static string GetKeyDownStr(ushort Key, DelphiShiftState Shift, bool IncludeFN = false)
    {
        string Result = "";

        bool IsFN = false;
        if ((Key >= (ushort)Keys.F1) && (Key <= (ushort)Keys.F12))
        {
            if (IncludeFN)
                IsFN = true;
            else
                IsFN = false;
        }

        if ((Key != (ushort)Keys.Menu) && (Key != (ushort)Keys.ControlKey) && (Key != (ushort)Keys.ShiftKey) &&
            (Key != (ushort)Keys.Tab) && (Key != (ushort)Keys.Scroll) &&
            (((Key >= 48) && (Key <= 57)) ||
             ((Key >= 65) && (Key <= 90)) ||
             ((Key >= 96) && (Key <= 105)) ||
             IsFN ||
             ((Key >= 186) && (Key <= 222))
            ))
        {
            if ((Shift & DelphiShiftState.ssShift) != 0)
            {
                Result = "Shift+";
            }
            else if ((Shift & DelphiShiftState.ssAlt) != 0)
            {
                Result = "Alt+";
            }
            else if ((Shift & DelphiShiftState.ssCtrl) != 0)
            {
                Result = "Ctrl+";
            }
            else if ((Shift & DelphiShiftState.ssLeft) != 0)
            {
                Result = "Left+";
            }
            else if ((Shift & DelphiShiftState.ssRight) != 0)
            {
                Result = "Right+";
            }
            else if ((Shift & DelphiShiftState.ssMiddle) != 0)
            {
                Result = "Middle+";
            }
            else if ((Shift & DelphiShiftState.ssDouble) != 0)
            {
                Result = "Double+";
            }
        }

        Result = Result + GetKey(Key);
        return Result;
    }

    // ================================================================================
    // ConfigShare.pas:33-81 声明的查找函数族（实现见 197-990 行）
    // ================================================================================

    /// <summary>
    /// 原文 ConfigShare.pas:197-210。
    /// 扫描 <c>g_ItemArr[Low .. GetMaxBagCount-1]</c>，名字非空且 CompareText 相等则返回下标。
    /// </summary>
    public static int FindBagItemName(string sItemName)
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
                {
                    Result = I;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:212-225。</summary>
    public static int FindHeroBagItemName(string sItemName)
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (ConfigShareSeam.g_HeroItemArr[I].s.Name != "")
            {
                if (SameText(ConfigShareSeam.g_HeroItemArr[I].s.Name, sItemName))
                {
                    Result = I;
                    break;
                }
            }
        }
        return Result;
    }

    // ---------------- HP 药 ----------------

    /// <summary>原文 ConfigShare.pas:227-249。</summary>
    public static int FindHumBindHPItemIndex()
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
            {
                // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                    (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)) && (ConfigShareSeam.g_ItemArr[I].s.Reserved == 0))
                {
                    for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                    {
                        TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                        if ((UnBindItem.UnBindItemType == TUnBindItemType.t_HP) &&
                            (ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                        {
                            Result = I;
                            return Result;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:251-276（外层先遍历绑定表，命中后内层扫包裹，<b>无论是否找到都 Exit</b>）。</summary>
    public static int FindHumBindHPItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
            {
                TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                if (SameText(UnBindItem.sItemName, sItemName))
                {
                    for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
                    {
                        // 修正内挂吃药会吃31类卷轴的问题chongchong 2013-10-27
                        if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                            (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)) && (ConfigShareSeam.g_ItemArr[I].s.Reserved == 0))
                        {
                            if ((UnBindItem.UnBindItemType == TUnBindItemType.t_HP) &&
                                (ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:278-300（英雄侧**不判 Reserved**）。</summary>
    public static int FindHeroBindHPItemIndex()
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero != null)
        {
            if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
            {
                for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                {
                    // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                    if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                        (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                    {
                        for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                        {
                            TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                            if ((UnBindItem.UnBindItemType == TUnBindItemType.t_HP) &&
                                (ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:302-327。</summary>
    public static int FindHeroBindHPItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero != null)
        {
            if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
            {
                for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                {
                    TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                    if (SameText(UnBindItem.sItemName, sItemName))
                    {
                        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                        {
                            // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                            if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                                (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                            {
                                if ((UnBindItem.UnBindItemType == TUnBindItemType.t_HP) &&
                                    (ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                                {
                                    Result = I;
                                    return Result;
                                }
                            }
                        }
                        return Result;
                    }
                }
            }
        }
        return Result;
    }

    // ---------------- MP 药 ----------------

    /// <summary>原文 ConfigShare.pas:329-351。</summary>
    public static int FindHumBindMPItemIndex()
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
            {
                // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                    (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)) && (ConfigShareSeam.g_ItemArr[I].s.Reserved == 0))
                {
                    for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                    {
                        TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                        if ((UnBindItem.UnBindItemType == TUnBindItemType.t_MP) &&
                            (ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                        {
                            Result = I;
                            return Result;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:353-378（注意：此处内层**不判 Reserved**，与 FindHumBindHPItemIndex(sItemName) 不同，原文如此）。</summary>
    public static int FindHumBindMPItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
            {
                TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                if (SameText(sItemName, UnBindItem.sItemName))
                {
                    for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
                    {
                        // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                        if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                            (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)))
                        {
                            if ((UnBindItem.UnBindItemType == TUnBindItemType.t_MP) &&
                                (ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:380-402。</summary>
    public static int FindHeroBindMPItemIndex()
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero != null)
        {
            if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
            {
                for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                {
                    // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                    if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                        (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                    {
                        for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                        {
                            TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                            if ((UnBindItem.UnBindItemType == TUnBindItemType.t_MP) &&
                                (ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:404-429。</summary>
    public static int FindHeroBindMPItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero != null)
        {
            if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
            {
                for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                {
                    TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                    if (SameText(sItemName, UnBindItem.sItemName))
                    {
                        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                        {
                            // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                            if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                                (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                            {
                                if ((UnBindItem.UnBindItemType == TUnBindItemType.t_MP) &&
                                    (ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                                {
                                    Result = I;
                                    return Result;
                                }
                            }
                        }
                        return Result;
                    }
                }
            }
        }
        return Result;
    }

    // ---------------- 特殊药 ----------------

    /// <summary>原文 ConfigShare.pas:431-453。</summary>
    public static int FindHumBindSpecialItemIndex()
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
            {
                // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                    (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)) && (ConfigShareSeam.g_ItemArr[I].s.Reserved == 0))
                {
                    for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                    {
                        TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                        if ((UnBindItem.UnBindItemType == TUnBindItemType.t_Special) &&
                            (ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                        {
                            Result = I;
                            return Result;
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:455-480。</summary>
    public static int FindHumBindSpecialItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
            {
                TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                if (SameText(UnBindItem.sItemName, sItemName))
                {
                    for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
                    {
                        // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                        if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                            (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)))
                        {
                            if ((UnBindItem.UnBindItemType == TUnBindItemType.t_Special) &&
                                (ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:482-504。</summary>
    public static int FindHeroBindSpecialItemIndex()
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero != null)
        {
            if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
            {
                for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                {
                    // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                    if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                        (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                    {
                        for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                        {
                            TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                            if ((UnBindItem.UnBindItemType == TUnBindItemType.t_Special) &&
                                (ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:506-531。</summary>
    public static int FindHeroBindSpecialItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero != null)
        {
            if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
            {
                for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
                {
                    TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                    if (SameText(UnBindItem.sItemName, sItemName))
                    {
                        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                        {
                            // 修正内挂吃药会吃31类卷轴的问题 chongchong 2013-10-27
                            if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                                (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                            {
                                if ((UnBindItem.UnBindItemType == TUnBindItemType.t_Special) &&
                                    (ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                                {
                                    Result = I;
                                    return Result;
                                }
                            }
                        }
                        return Result;
                    }
                }
            }
        }
        return Result;
    }

    // ---------------- 未绑定（解包）药 ----------------

    /// <summary>
    /// 原文 ConfigShare.pas:533-557。
    /// 两段扫描：先 <c>[Low+6 .. GetMaxBagCount-1]</c> 判 <c>Shape in [0,1]</c>，
    /// 再 <c>[Low .. 5]</c> 判 <c>Shape = 0</c>（原文如此，两段判据不同）。
    /// </summary>
    public static int FindHumUnBindHPItemIndex()
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0 || ConfigShareSeam.g_ItemArr[I].s.Shape == 1)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 == 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }

        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 == 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:559-591（OnEqualName=True 时命中即返回，不看装备形态）。</summary>
    public static int FindHumUnBindHPItemIndex(string sItemName, bool OnEqualName = false)
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                if (OnEqualName)
                {
                    Result = I;
                    return Result;
                }
                else if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 == 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }

        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                if (OnEqualName)
                {
                    Result = I;
                    return Result;
                }
                else if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 == 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:593-606。</summary>
    public static int FindHeroUnBindHPItemIndex()
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (ConfigShareSeam.g_HeroItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_HeroItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_HeroItemArr[I].s.MAC1 == 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:608-621。</summary>
    public static int FindHeroUnBindHPItemIndex(string sItemName)
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (SameText(ConfigShareSeam.g_HeroItemArr[I].s.Name, sItemName))
            {
                if ((ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_HeroItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_HeroItemArr[I].s.MAC1 == 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:623-647（特殊药判据：Shape &gt; 0 且 AC1 &gt; 0 且 MAC1 &gt; 0）。</summary>
    public static int FindHumUnBindSpecialItemIndex()
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape > 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }

        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape > 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:649-681。</summary>
    public static int FindHumUnBindSpecialItemIndex(string sItemName, bool OnEqualName = false)
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                if (OnEqualName)
                {
                    Result = I;
                    return Result;
                }
                else if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape > 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }

        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                if (OnEqualName)
                {
                    Result = I;
                    return Result;
                }
                else if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape > 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:683-696。</summary>
    public static int FindHeroUnBindSpecialItemIndex()
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (ConfigShareSeam.g_HeroItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.Shape > 0)
                    && (ConfigShareSeam.g_HeroItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_HeroItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:698-711。</summary>
    public static int FindHeroUnBindSpecialItemIndex(string sItemName)
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (SameText(ConfigShareSeam.g_HeroItemArr[I].s.Name, sItemName))
            {
                if ((ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.Shape > 0)
                    && (ConfigShareSeam.g_HeroItemArr[I].s.AC1 > 0) && (ConfigShareSeam.g_HeroItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:713-735（MP 判据：Shape = 0 且 AC1 = 0 且 MAC1 &gt; 0）。</summary>
    public static int FindHumUnBindMPItemIndex()
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 == 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if (ConfigShareSeam.g_ItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 == 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:737-768。</summary>
    public static int FindHumUnBindMPItemIndex(string sItemName, bool OnEqualName = false)
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                if (OnEqualName)
                {
                    Result = I;
                    return Result;
                }
                else if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 == 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if (SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                if (OnEqualName)
                {
                    Result = I;
                    return Result;
                }
                else if ((ConfigShareSeam.g_ItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_ItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_ItemArr[I].s.AC1 == 0) && (ConfigShareSeam.g_ItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:770-783。</summary>
    public static int FindHeroUnBindMPItemIndex()
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (ConfigShareSeam.g_HeroItemArr[I].s.Name != "")
            {
                if ((ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_HeroItemArr[I].s.AC1 == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:785-798。</summary>
    public static int FindHeroUnBindMPItemIndex(string sItemName)
    {
        int Result = -1;
        for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
        {
            if (SameText(ConfigShareSeam.g_HeroItemArr[I].s.Name, sItemName))
            {
                if ((ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.Shape == 0)
                    && (ConfigShareSeam.g_HeroItemArr[I].s.AC1 == 0) && (ConfigShareSeam.g_HeroItemArr[I].s.MAC1 > 0))
                {
                    Result = I;
                    return Result;
                }
            }
        }
        return Result;
    }

    // ---------------- 技能书 / 通用绑定物品 ----------------

    /// <summary>原文 ConfigShare.pas:800-819。</summary>
    public static int FindHumUnBindBookItemIndex(string sItemName)
    {
        int Result = -1;
        for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
        {
            if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                Result = I;
                return Result;
            }
        }

        for (int I = 0 /*Low(g_ItemArr)*/; I <= 5; I++)
        {
            if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && SameText(ConfigShareSeam.g_ItemArr[I].s.Name, sItemName))
            {
                Result = I;
                return Result;
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:821-846（注意：原文此函数**不判 Reserved**，与 HP/MP/Special 的 Bind 版不同）。</summary>
    public static int FindHumBindBookItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
            {
                TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                if (SameText(UnBindItem.sItemName, sItemName))
                {
                    for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
                    {
                        // 修正吃回城卷会吃掉卷轴 + (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
                        if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                            (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)))
                        {
                            if ((ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:848-873（用途同 FindHumBindBookItemIndex，但**不检查 UnBindItemType**）。</summary>
    public static int FindHumBindItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.HumBagNoUseItemCount() > 6)
        {
            for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
            {
                TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                if (SameText(UnBindItem.sItemName, sItemName))
                {
                    for (int I = 0 /*Low(g_ItemArr)*/ + 6; I <= ConfigShareSeam.GetMaxBagCount() - 1; I++)
                    {
                        // 修正吃回城卷会吃掉卷轴 + (not (g_ItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
                        if ((ConfigShareSeam.g_ItemArr[I].s.Name != "") && (ConfigShareSeam.g_ItemArr[I].s.StdMode == 31) &&
                            (!ShapeIn0_1_15To51(ConfigShareSeam.g_ItemArr[I].s.Shape)))
                        {
                            if ((ConfigShareSeam.g_ItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                    return Result;
                }
            }
        }
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:875-899（开头 <c>if g_MyHero = nil then Exit;</c>，与同族其它函数写法不同，原文如此）。</summary>
    public static int FindHeroBindItemIndex(string sItemName)
    {
        int Result = -1;
        if (ConfigShareSeam.g_MyHero == null) return Result;
        if (ConfigShareSeam.g_MyHero.m_nBagCount - ConfigShareSeam.HeroBagItemCount() >= 6)
        {
            for (int II = 0; II <= ConfigShareSeam.g_UnbindItemList.Count - 1; II++)
            {
                TUnBindItem UnBindItem = ConfigShareSeam.g_UnbindItemList[II];
                if (SameText(UnBindItem.sItemName, sItemName))
                {
                    for (int I = 0 /*Low*/; I <= ConfigShareSeam.g_HeroItemArr.Length - 1 /*High*/; I++)
                    {
                        // 修正吃回城卷会吃掉卷轴 + (not (g_HeroItemArr[I].s.Shape in [0, 1, 15..51])) chongchong 2013-12-11
                        if ((ConfigShareSeam.g_HeroItemArr[I].s.Name != "") && (ConfigShareSeam.g_HeroItemArr[I].s.StdMode == 31) &&
                            (!ShapeIn0_1_15To51(ConfigShareSeam.g_HeroItemArr[I].s.Shape)))
                        {
                            if ((ConfigShareSeam.g_HeroItemArr[I].s.Shape == UnBindItem.nShape))
                            {
                                Result = I;
                                return Result;
                            }
                        }
                    }
                    return Result;
                }
            }
        }
        return Result;
    }

    // ================================================================================
    // ConfigShare.pas:901-990  未绑定优先、绑定兜底的组合入口
    // 语义：全部是"先查 UnBind，<0 再查 Bind"。
    // ================================================================================

    /// <summary>原文 ConfigShare.pas:901-906。</summary>
    public static int FindHumHPItemIndex()
    {
        int Result = FindHumUnBindHPItemIndex();
        if (Result < 0)
            Result = FindHumBindHPItemIndex();
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:908-913。</summary>
    public static int FindHumHPItemIndex(string sItemName, bool OnEqualName = false)
    {
        int Result = FindHumUnBindHPItemIndex(sItemName, OnEqualName);
        if (Result < 0)
            Result = FindHumBindHPItemIndex(sItemName);
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:915-920。</summary>
    public static int FindHumMPItemIndex()
    {
        int Result = FindHumUnBindMPItemIndex();
        if (Result < 0)
            Result = FindHumBindMPItemIndex();
        return Result;
    }

    /// <summary>
    /// 原文 ConfigShare.pas:922-927。
    /// **原文缺陷（照抄）**：这里把 <c>OnEqualName</c> 丢了——调用的是
    /// <c>FindHumUnBindMPItemIndex(sItemName)</c>（单参重载 → OnEqualName 取默认 False），
    /// 而同族的 HP/Special 版本都传了 <c>OnEqualName</c>。
    /// </summary>
    public static int FindHumMPItemIndex(string sItemName, bool OnEqualName = false)
    {
        int Result = FindHumUnBindMPItemIndex(sItemName);
        if (Result < 0)
            Result = FindHumBindMPItemIndex(sItemName);
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:929-934。</summary>
    public static int FindHeroHPItemIndex()
    {
        int Result = FindHeroUnBindHPItemIndex();
        if (Result < 0)
            Result = FindHeroBindHPItemIndex();
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:936-941。</summary>
    public static int FindHeroHPItemIndex(string sItemName)
    {
        int Result = FindHeroUnBindHPItemIndex(sItemName);
        if (Result < 0)
            Result = FindHeroBindHPItemIndex(sItemName);
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:943-948。</summary>
    public static int FindHeroMPItemIndex()
    {
        int Result = FindHeroUnBindMPItemIndex();
        if (Result < 0)
            Result = FindHeroBindMPItemIndex();
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:950-955。</summary>
    public static int FindHeroMPItemIndex(string sItemName)
    {
        int Result = FindHeroUnBindMPItemIndex(sItemName);
        if (Result < 0)
            Result = FindHeroBindMPItemIndex(sItemName);
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:957-962。</summary>
    public static int FindHumSpecialItemIndex()
    {
        int Result = FindHumUnBindSpecialItemIndex();
        if (Result < 0)
            Result = FindHumBindSpecialItemIndex();
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:964-969。</summary>
    public static int FindHumSpecialItemIndex(string sItemName, bool OnEqualName = false)
    {
        int Result = FindHumUnBindSpecialItemIndex(sItemName, OnEqualName);
        if (Result < 0)
            Result = FindHumBindSpecialItemIndex(sItemName);
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:971-976。</summary>
    public static int FindHeroSpecialItemIndex()
    {
        int Result = FindHeroUnBindSpecialItemIndex();
        if (Result < 0)
            Result = FindHeroBindSpecialItemIndex();
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:978-983。</summary>
    public static int FindHeroSpecialItemIndex(string sItemName)
    {
        int Result = FindHeroUnBindSpecialItemIndex(sItemName);
        if (Result < 0)
            Result = FindHeroBindSpecialItemIndex(sItemName);
        return Result;
    }

    /// <summary>原文 ConfigShare.pas:985-990。</summary>
    public static int FindHumBookItemIndex(string sItemName)
    {
        int Result = FindHumUnBindBookItemIndex(sItemName);
        if (Result < 0)
            Result = FindHumBindBookItemIndex(sItemName);
        return Result;
    }

    // ================================================================================
    // 内部辅助
    // ================================================================================

    /// <summary>
    /// Delphi <c>CompareText(a, b) = 0</c>：**大小写不敏感**（非区域敏感）。
    /// 与工程内既有移植一致（MonGenLoadCore.AnsiCompareText 用 OrdinalIgnoreCase）。
    /// </summary>
    internal static bool SameText(string a, string b)
        => string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;

    /// <summary>
    /// Delphi <c>Shape in [0, 1, 15..51]</c> 的等价表达（ConfigShare 中一律以 <c>not (... in [...])</c> 使用）。
    /// </summary>
    internal static bool ShapeIn0_1_15To51(ushort Shape)
        => Shape == 0 || Shape == 1 || (Shape >= 15 && Shape <= 51);
}
