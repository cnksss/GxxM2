// ============================================================================================
// 车道 `p10-client-scrn`：`DropItemsMgr.pas`（570 行）整单元 1:1 移植。
//
//   原文区间            内容
//   14-56     TDropItem（record，pTDropItem = ^TDropItem）
//   58-87     TPointDropItemList（某点掉落物品）
//   89-124    TDropItemsMgr
//   126-568   实现（构造函数/析构/Clear/二分查找/增删/刷新绘制名单）
//
// ★ 与 `Scenes/DropItemFx.cs` 的关系（本车道任务书第 3 条）★★
//   `DropItemFx.cs` 里有两块东西：
//     (a) `DropItemFx` 静态类 + `FxImage`/`DropItemFxState`/`DropItemDrawOp`/`DropItemEffectDef`
//         —— 来自 **PlayScn.pas**（掉落物特效帧/闪烁节拍/名字定位），**不是** DropItemsMgr.pas；
//     (b) `PointDropItemList` / `DropItemsStore` / `DropItem` / `ShowItemInfo`
//         —— DropItemsMgr.pas 的 **headless 等价物**（文件头自述「TDropItemsMgr（246-560）headless 镜像」）。
//   本文件落成 `TDropItemsMgr`/`TPointDropItemList`/`TDropItem`（**原文名**）后，(b) 确实被取代。
//   **但本车道不能删它**：`Scenes/PlaySceneCore.cs:194` 引用 `DropItemsStore`，而该文件**不在本车道分区**
//   （分区只有 `Scenes/DrawScrn/**` + 例外授权的 `Scenes/DropItemFx.cs`）。
//   删除会让 `PlaySceneCore.cs` 编译失败 ⇒ 门禁变红。
//   ⇒ 按任务书「否则如实登记」执行：**D-P10-02** 给出集成方的一步式处置清单。
//   `TDropItemsMgr` / `PointDropItemList` 名称不冲突（原文带 `T` 前缀）⇒ 当前门禁可绿。
//
// 【原文缺陷 D-P10-D04】`TDropItem.Name` / `DBName` 是 **Delphi 短字符串 `string[60]`**：
//   赋值时按 **GBK 字节**截断到 60（一个汉字 2 字节 ⇒ 实际最多 30 个汉字），**静默丢数据**。
//   本移植用 `SetShortString60` 复刻该截断并配差异断言（见 ScrnDrawDropItemsTests）。
// ============================================================================================

using System;
using System.Collections.Generic;
using GXX.Client.GUI.GameConfig;
using GXX.Client.Tail;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using TColor = GXX.Client.GUI.Mir.TColor;

namespace GXX.Client.Scenes;

/// <summary>
/// DropItemsMgr.pas 引用、而工程中尚无正式归属的外部全局（接缝）。
/// </summary>
public static class DropItemsMgrEnv
{
    /// <summary>MShare.pas:2229 `g_DropItemEffectList:TDropItemEffectList = nil`（后在 13392 Create）。</summary>
    public static TDropItemEffectList g_DropItemEffectList = new TDropItemEffectList();

    /// <summary>FilterItems.pas:486-487 `g_FileItemDB:TFileItemDB`（复用车道正式归属）。</summary>
    public static TFileItemDB g_FileItemDB => FilterItemsGlobal.g_FileItemDB;

    /// <summary>MShare.pas `PlugInEnabled`（与 DrawScrn 侧同一开关）。</summary>
    public static bool PlugInEnabled { get => DrawScrnEnv.PlugInEnabled; set => DrawScrnEnv.PlugInEnabled = value; }

    /// <summary>MShare.pas `g_ConfigDlg.GetShowItem(DBName):pTShowItem`。</summary>
    public static object GetShowItem(string dbName) => DrawScrnEnv.GetShowItem(dbName);

    /// <summary>MShare.pas MyGetTickCount。</summary>
    public static uint MyGetTickCount() => DrawScrnEnv.MyGetTickCount;

    /// <summary>
    /// MShare.pas:3510-3524 `function Makecode_T(pstr:Integer):Integer`。
    /// <para>SDK.pas:27 `ENCRYPOINT = 0` ⇒ 走 `{$ELSE} Result := pstr; {$IFEND}` 分支（恒等）。
    /// ENCRYPOINT=1 的加扰分支（`a := Random(8)+1; Result := (pstr xor a)*10 + a`）**未启用**，
    /// 此处保留其注释形态以供对照。</para>
    /// </summary>
    public static int Makecode_T(int pstr)
    {
        // {$IF ENCRYPOINT = 1}
        //   a := Random(8) + 1;
        //   Result := (pstr xor a) * 10 + a;
        // {$ELSE}
        return pstr;
        // {$IFEND}
    }

    /// <summary>
    /// MShare.pas:3526-3543 `function Cutecode_T(pstr:Integer):Integer`。
    /// <para>SDK.pas:27 `ENCRYPOINT = 0` ⇒ 恒等分支。
    /// ENCRYPOINT=1 分支为 `t := pstr mod 10; Result := (pstr div 10) xor t;`。</para>
    /// </summary>
    public static int Cutecode_T(int pstr)
    {
        // {$IF ENCRYPOINT = 1}
        //   t := pstr mod 10;
        //   Result := (pstr div 10) xor t;
        // {$ELSE}
        return pstr;
        // {$IFEND}
    }

    /// <summary>Delphi 短字符串 `string[N]` 的赋值语义：按 GBK 字节截断到 N。</summary>
    public static string SetShortString(string value, int capacity)
    {
        value ??= "";
        byte[] data = EncodingInit.GBK.GetBytes(value);
        if (data.Length <= capacity) return value;
        return EncodingInit.GBK.GetString(data, 0, capacity);
    }

    /// <summary>测试复位。</summary>
    public static void ResetForTests()
    {
        g_DropItemEffectList = new TDropItemEffectList();
        DrawScrnEnv.PlugInEnabled = false;
    }
}

/// <summary>
/// DropItemsMgr.pas:14-56 `TDropItem`（`pTDropItem = ^TDropItem`）。
/// <para>原文是 record + 裸指针（New/Dispose 堆对象）→ 托管 `sealed class`（引用语义一致）。</para>
/// </summary>
public sealed class TDropItem
{
    public int ID;

    public ushort X;
    public ushort Y;

    public bool Visible;
    public bool ShowName;
    public bool ShowFlash;
    public byte FlashStep;

    public ushort Looks;
    public ushort OverlapCount;

    public bool boValueItem; // 是否极品装备
    public TColor ItemColor;

    private string _name = "";
    private string _dbName = "";

    /// <summary>原文 30 `Name:string[60]`（短字符串，GBK 字节上限 60）。</summary>
    public string Name
    {
        get => _name;
        set => _name = DropItemsMgrEnv.SetShortString(value, 60);
    }

    /// <summary>原文 31 `DBName:string[60]`（短字符串，GBK 字节上限 60）。</summary>
    public string DBName
    {
        get => _dbName;
        set => _dbName = DropItemsMgrEnv.SetShortString(value, 60);
    }

    public uint FlashTime;
    public uint FlashStepTime;

    public object ItemTexture;
    public int TextureWidth;
    public int TextureHeight;

    public uint ShowNameTime;
    public object ShowItem;

    /// <summary>原文 43 `NameImageInfo:TImageInfo`（HGEFontEx.pas）。</summary>
    public TImageInfo NameImageInfo = new TImageInfo();

    public uint dwGhostTick;

    // 排除到挂机捡物列表时间 chongchong 2014-12-04
    public uint dwGJExcludeTick;

    public TDropItemEffect ItemEffect;
    public int ItemEffectFrame;
    public uint ItemEffectTick;

    public int ValueItemEffectFrame;
    public uint ValueItemEffectTick;
}

/// <summary>DropItemsMgr.pas:61-87 TPointDropItemList（某点掉落物品）。</summary>
public class TPointDropItemList
{
    public List<TDropItem> FList;
    public List<TDropItem> FDrawList;
    public TDropItemsMgr FOwner;
    public ushort FPointX;
    public ushort FPointY;

    /// <summary>DropItemsMgr.pas:73/135-146 constructor Create(AOwner:TDropItemsMgr; X, Y:Word)。</summary>
    public TPointDropItemList(TDropItemsMgr aOwner, ushort x, ushort y)
    {
        FList = new List<TDropItem>();
        // 原文 FList.Capacity := 4（Delphi TList 预分配；List<T> 无对应语义，不移植）

        FDrawList = new List<TDropItem>();
        // 原文 FDrawList.Capacity := 3

        FOwner = aOwner;
        FPointX = x;
        FPointY = y;
    }

    /// <summary>DropItemsMgr.pas:74/148-155 destructor Destroy; override。</summary>
    public void Free()
    {
        Clear();
        FList = null;

        FDrawList = null;
    }

    /// <summary>DropItemsMgr.pas:79/157-167 procedure Clear。</summary>
    public void Clear()
    {
        // 原文对每项 Dispose(DropItem)（托管侧 GC）
        FList.Clear();
    }

    /// <summary>DropItemsMgr.pas:68/169-172 function GetCount:Integer。</summary>
    public int GetCount() => FList.Count;

    /// <summary>DropItemsMgr.pas:69/174-180 function GetItems(Index:Integer):PTDropItem（越界返回 nil）。</summary>
    public TDropItem GetItems(int index)
    {
        if ((index >= 0) && (index <= FList.Count - 1))
            return FList[index];
        return null;
    }

    /// <summary>DropItemsMgr.pas:70/182-185 function GetDrawCount:Integer。</summary>
    public int GetDrawCount() => FDrawList.Count;

    /// <summary>DropItemsMgr.pas:71/187-190 function GetDrawItems(Index:Integer):pTDropItem（**越界不判**，原文如此）。</summary>
    public TDropItem GetDrawItems(int index) => FDrawList[index];

    /// <summary>DropItemsMgr.pas:83/192-242 procedure RefreshDrawList(ResetShowItem:Boolean = False)。</summary>
    public void RefreshDrawList(bool resetShowItem = false)
    {
        TDropItem dropItem;
        bool boFind;
        FDrawList.Clear();

        for (int i = 0; i < FList.Count; i++)
        {
            dropItem = FList[i];

            if (DropItemsMgrEnv.PlugInEnabled && resetShowItem)
            {
                dropItem.ShowItem = DropItemsMgrEnv.g_FileItemDB.Find(dropItem.DBName);
            }

            if ((dropItem.ShowItem != null) && dropItem.Visible && (((TShowItem)dropItem.ShowItem).boShowName != 0))
            {
                if (FDrawList.Count < 3)
                    FDrawList.Add(dropItem);
                else
                {
                    dropItem.NameImageInfo.ImageIndexs = null;
                    dropItem.NameImageInfo.Width = 0;
                    dropItem.NameImageInfo.Height = 0;
                }
            }
            else
            {
                dropItem.NameImageInfo.ImageIndexs = null;
                dropItem.NameImageInfo.Width = 0;
                dropItem.NameImageInfo.Height = 0;
            }
        }

        if (FDrawList.Count >= 3) return;

        if (FList.Count > 0)
        {
            for (int i = 0; i < FList.Count; i++)
            {
                dropItem = FList[i];
                boFind = false;
                for (int ii = 0; ii <= FDrawList.Count - 1; ii++)
                {
                    if (ReferenceEquals(FDrawList[ii], dropItem))
                    {
                        boFind = true;
                        break;
                    }
                }

                if ((!boFind) && dropItem.Visible)
                    FDrawList.Add(dropItem);

                if (FDrawList.Count >= 3) break;
            }
        }
    }

    /// <summary>DropItemsMgr.pas:76 property X:Word read FPointX。</summary>
    public ushort X => FPointX;

    /// <summary>DropItemsMgr.pas:77 property Y:Word read FPointY。</summary>
    public ushort Y => FPointY;

    /// <summary>DropItemsMgr.pas:80 property Count:Integer read GetCount。</summary>
    public int Count => GetCount();

    /// <summary>DropItemsMgr.pas:81 property Items[Index:Integer]:PTDropItem read GetItems; default。</summary>
    public TDropItem Items(int index) => GetItems(index);

    /// <summary>DropItemsMgr.pas:84 property DrawCount:Integer read GetDrawCount。</summary>
    public int DrawCount => GetDrawCount();

    /// <summary>DropItemsMgr.pas:85 property DrawItems[Index:Integer]:PTDropItem read GetDrawItems。</summary>
    public TDropItem DrawItems(int index) => GetDrawItems(index);
}

/// <summary>DropItemsMgr.pas:89-124 TDropItemsMgr（按 ID 索引 + 按 (X,Y) 有序点表）。</summary>
public class TDropItemsMgr
{
    public TRTLCriticalSection FCS = new TRTLCriticalSection();
    public List<TDropItem> FSortIDItemList;
    public List<TDropItem> FNoUseItemList;
    public List<TPointDropItemList> FPointList;

    /// <summary>DropItemsMgr.pas:246-265 constructor Create。</summary>
    public TDropItemsMgr()
    {
        FSortIDItemList = new List<TDropItem>();
        // 原文 FSortIDItemList.Capacity := 256

        FNoUseItemList = new List<TDropItem>();
        // 原文 FNoUseItemList.Capacity := 512

        for (int i = 0; i <= 512 - 1; i++)
        {
            FNoUseItemList.Add(new TDropItem());
        }

        FPointList = new List<TPointDropItemList>();
        // 原文 FPointList.Capacity := 256
    }

    /// <summary>DropItemsMgr.pas:267-275 destructor Destroy; override。</summary>
    public void Free()
    {
        ClearAndFree();
        FSortIDItemList = null;
        FNoUseItemList = null;
        FPointList = null;
    }

    /// <summary>DropItemsMgr.pas:98/277-295 procedure ClearAndFree。</summary>
    public void ClearAndFree()
    {
        for (int i = 0; i < FPointList.Count; i++)
        {
            FPointList[i].Free();
        }
        FPointList.Clear();
        FSortIDItemList.Clear();

        for (int i = 0; i < FNoUseItemList.Count; i++)
        {
            // 原文 Dispose(DropItem)
        }
        FNoUseItemList.Clear();
    }

    /// <summary>DropItemsMgr.pas:109/297-318 procedure Clear。</summary>
    public void Clear()
    {
        for (int i = 0; i < FSortIDItemList.Count; i++)
        {
            var dropItem = FSortIDItemList[i];
            if (FNoUseItemList.Count >= 512)
            {
                // 原文 Dispose(DropItem)
            }
            else
                FNoUseItemList.Add(dropItem);
        }
        FSortIDItemList.Clear();

        for (int i = 0; i < FPointList.Count; i++)
        {
            var pointItemList = FPointList[i];
            pointItemList.FList.Clear();
            pointItemList.Free();
        }
        FPointList.Clear();
    }

    /// <summary>DropItemsMgr.pas:110/320-323 procedure Lock（原文 EnterCriticalSection(FCS)）。</summary>
    public void Lock() => FCS.Enter();

    /// <summary>DropItemsMgr.pas:111/554-557 procedure UnLock（原文 LeaveCriticalSection(FCS)）。</summary>
    public void UnLock() => FCS.Leave();

    /// <summary>DropItemsMgr.pas:95/320-323 function GetCount:Integer。</summary>
    public int GetCount() => FPointList.Count;

    /// <summary>DropItemsMgr.pas:96/325-331 function GetItems(Index:Integer):TPointDropItemList（越界返回 nil）。</summary>
    public TPointDropItemList GetItems(int index)
    {
        if ((index >= 0) && (index <= FPointList.Count - 1))
            return FPointList[index];
        return null;
    }

    /// <summary>DropItemsMgr.pas:115/333-341 function GetItemByID(ID:Integer):pTDropItem。</summary>
    public TDropItem GetItemByID(int id)
    {
        if (IDSearch(id, out int index))
            return FSortIDItemList[index];
        return null;
    }

    /// <summary>DropItemsMgr.pas:116/343-351 function GetItemListByPoint(X, Y:Word):TPointDropItemList。</summary>
    public TPointDropItemList GetItemListByPoint(ushort x, ushort y)
    {
        if (PointSearch(x, y, out int index))
            return FPointList[index];
        return null;
    }

    /// <summary>DropItemsMgr.pas:117/353-356 function GetItemListIndexByY(Y:Word):Integer。</summary>
    public int GetItemListIndexByY(ushort y)
    {
        PointSearch(65535, y, out int result);
        return result;
    }

    /// <summary>DropItemsMgr.pas:100/358-361 function IDCompare(ID1, ID2:Integer):Integer; virtual。</summary>
    protected virtual int IDCompare(int id1, int id2) => id1 - id2;

    /// <summary>DropItemsMgr.pas:101/363-384 function IDSearch(ID:Integer; var Index:Integer):Boolean; virtual。</summary>
    protected virtual bool IDSearch(int id, out int index)
    {
        bool result = false;
        int l = 0;
        int h = FSortIDItemList.Count - 1;
        int i = 0;
        int c;
        while (l <= h)
        {
            i = l + ((h - l) >> 1);
            c = IDCompare(FSortIDItemList[i].ID, id);
            if (c < 0)
                l = i + 1;
            else
            {
                h = i - 1;
                if (c == 0)
                {
                    result = true;
                    l = i;
                }
            }
        }
        index = l;
        return result;
    }

    /// <summary>DropItemsMgr.pas:103/386-389 function PointCompare(X1, Y1, X2, Y2:Word):Integer; virtual。</summary>
    protected virtual int PointCompare(ushort x1, ushort y1, ushort x2, ushort y2)
    {
        return unchecked(DelphiRTL.MakeLong(x1, y1) - DelphiRTL.MakeLong(x2, y2));
    }

    /// <summary>DropItemsMgr.pas:104/391-414 function PointSearch(X, Y:Word; var Index:Integer):Boolean; virtual。</summary>
    protected virtual bool PointSearch(ushort x, ushort y, out int index)
    {
        bool result = false;
        int l = 0;
        int h = FPointList.Count - 1;
        int i;
        int c;
        while (l <= h)
        {
            i = l + ((h - l) >> 1);
            var pointDropItemList = FPointList[i];
            c = PointCompare(pointDropItemList.FPointX, pointDropItemList.FPointY, x, y);
            if (c < 0)
                l = i + 1;
            else
            {
                h = i - 1;
                if (c == 0)
                {
                    result = true;
                    l = i;
                }
            }
        }
        index = l;
        return result;
    }

    /// <summary>DropItemsMgr.pas:118/416-501 function AddDropItem(ID:Integer; nX, nY:Word; DBName:string; EffectIndex:Integer; var IsNew:Boolean):pTDropItem。</summary>
    public TDropItem AddDropItem(int id, ushort nX, ushort nY, string dbName, int effectIndex, out bool isNew)
    {
        TDropItem result;
        TPointDropItemList pointDropItemList;
        TDropItemEffect? dropItemEffect;

        //Result := nil; //HZQ 20230525 以下每个路径均有对Result的赋值
        if (IDSearch(id, out int index))
        {
            result = FSortIDItemList[index];
            result.DBName = dbName;
            isNew = false;
        }
        else
        {
            isNew = true;
            if (FNoUseItemList.Count > 0)
            {
                result = FNoUseItemList[0];
                FNoUseItemList.RemoveAt(0);
            }
            else
            {
                result = new TDropItem();
            }

            // 原文 FillChar(Result^, SizeOf(TDropItem), 0) —— 托管侧新对象即零值/空串，语义等价

            result.ID = id;
            result.X = (ushort)DropItemsMgrEnv.Makecode_T(nX);
            result.Y = (ushort)DropItemsMgrEnv.Makecode_T(nY);
            result.DBName = dbName;
            result.Visible = true;

            if (DropItemsMgrEnv.PlugInEnabled)
                result.ShowItem = DropItemsMgrEnv.GetShowItem(dbName);

            FSortIDItemList.Insert(index, result);

            if (PointSearch(nX, nY, out int pointIndex))
            {
                pointDropItemList = FPointList[pointIndex];

                dropItemEffect = null;
                if (effectIndex > 0)
                    dropItemEffect = DropItemsMgrEnv.g_DropItemEffectList.Get(effectIndex);

                if (dropItemEffect != null)
                {
                    result.ItemEffect = dropItemEffect.Value;
                    result.ItemEffectFrame = dropItemEffect.Value.StartIndex;
                    result.ItemEffectTick = DropItemsMgrEnv.MyGetTickCount();

                    result.FlashTime = DropItemsMgrEnv.MyGetTickCount();
                    result.ShowFlash = false;
                    result.FlashStepTime = DropItemsMgrEnv.MyGetTickCount();
                    result.FlashStep = 0;
                }
                else
                {
                    result.ItemEffect.FileIndex = -1;
                }

                if ((dropItemEffect != null) && (pointDropItemList.FList.Count > 0))
                    pointDropItemList.FList.Insert(0, result);
                else
                    pointDropItemList.FList.Add(result);
            }
            else
            {
                pointDropItemList = new TPointDropItemList(this, (ushort)DropItemsMgrEnv.Makecode_T(nX), (ushort)DropItemsMgrEnv.Makecode_T(nY));
                FPointList.Insert(pointIndex, pointDropItemList);

                dropItemEffect = null;
                if (effectIndex > 0)
                    dropItemEffect = DropItemsMgrEnv.g_DropItemEffectList.Get(effectIndex);

                if (dropItemEffect != null)
                {
                    result.ItemEffect = dropItemEffect.Value;
                    result.ItemEffectFrame = dropItemEffect.Value.StartIndex;
                    result.ItemEffectTick = DropItemsMgrEnv.MyGetTickCount();

                    result.FlashTime = DropItemsMgrEnv.MyGetTickCount();
                    result.ShowFlash = false;
                    result.FlashStepTime = DropItemsMgrEnv.MyGetTickCount();
                    result.FlashStep = 0;
                }
                else
                {
                    result.ItemEffect.FileIndex = -1;
                }

                if ((dropItemEffect != null) && (pointDropItemList.FList.Count > 0))
                    pointDropItemList.FList.Insert(0, result);
                else
                    pointDropItemList.FList.Add(result);
            }
        }

        return result;
    }

    /// <summary>DropItemsMgr.pas:120/503-547 function DelDropItem(ID:Integer; var X, Y:Word):pTDropItem。</summary>
    public TDropItem DelDropItem(int id, out ushort x, out ushort y)
    {
        TDropItem result = null;
        x = 0;
        y = 0;
        if (IDSearch(id, out int index))
        {
            result = FSortIDItemList[index];
            FSortIDItemList.RemoveAt(index);
            result.Visible = false;

            x = (ushort)DropItemsMgrEnv.Cutecode_T(result.X);
            y = (ushort)DropItemsMgrEnv.Cutecode_T(result.Y);

            if (PointSearch((ushort)DropItemsMgrEnv.Cutecode_T(result.X), (ushort)DropItemsMgrEnv.Cutecode_T(result.Y), out int pointIndex))
            {
                var pointDropItemList = FPointList[pointIndex];

                //IsFound := False;
                for (int i = 0; i <= pointDropItemList.Count - 1; i++)
                {
                    if (pointDropItemList.Items(i).ID == result.ID)
                    {
                        //IsFound := True;
                        pointDropItemList.FList.RemoveAt(i);
                        break;
                    }
                }

                if (pointDropItemList.Count == 0)
                {
                    pointDropItemList.Free();
                    FPointList.RemoveAt(pointIndex);
                }
            }
            else
            {
                // OutputDebugString('aaa');
            }

            FNoUseItemList.Add(result);
        }
        return result;
    }

    /// <summary>DropItemsMgr.pas:113/559-568 procedure RefreshDrawList(ResetShowItem:Boolean = False)。</summary>
    public void RefreshDrawList(bool resetShowItem = false)
    {
        for (int i = 0; i < FPointList.Count; i++)
        {
            FPointList[i].RefreshDrawList(resetShowItem);
        }
    }

    /// <summary>DropItemsMgr.pas:122 property Count:Integer read GetCount。</summary>
    public int Count => GetCount();

    /// <summary>DropItemsMgr.pas:123 property Items[Index:Integer]:TPointDropItemList read GetItems。</summary>
    public TPointDropItemList Items(int index) => GetItems(index);
}
