using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>`M2Definition.pas` 的 `TVisibleMapItem`(377-389) 1:1（字段顺序与类型照抄）。</summary>
public sealed class TVisibleMapItem
{
    public byte nVisibleFlag;
    public object? BaseObject;
    public int nX;
    public int nY;
    public string sName = "";
    public ushort wLooks;
    public byte btColor2;

    /// <summary>是否有极品属性 (0/1)。</summary>
    public bool boValue;

    /// <summary>`MakeLong(Dura + 1, wEffect)`——低 16 位叠加数量、高 16 位特效号。</summary>
    public int OverlapCountAndEffect;

    public bool boSendHideMsg;
}

/// <summary>`M2Definition.pas` 的 `TVisibleBaseObject`(402-405) 1:1。</summary>
public sealed class TVisibleBaseObject
{
    public byte nVisibleFlag;
    public object? BaseObject;
}

/// <summary>`M2Definition.pas` 的 `TVisibleMapEvent`(393-398) 1:1。</summary>
public sealed class TVisibleMapEvent
{
    public byte nVisibleFlag;
    public object? BaseObject;
    public int nX;
    public int nY;
}

/// <summary>
/// 可见物品/对象/事件 的**三态生命周期** 1:1 移植（批次J102），来源三处：
/// `TBaseObject.UpdateVisibleItem`(ObjBase.pas 31476-31530)、
/// `TPlayObject.HideItem`(ObjPlayer.pas 11330-11342)、
/// `TPlayObject.UpdateVisibleEvent`(ObjPlayer.pas 11344-11364)。
/// 另附其依赖的 `CheckOverLapItem`(M2Share.pas 11028-11032) 与
/// `GetDropItemValue`(M2Share.pas 33204-33265)。
///
/// **`nVisibleFlag` 的三态语义（本批次核心，散落于 6 处赋值点，极易只读懂一半）**：
/// | 值 | 含义 | 赋值点 |
/// |---|---|---|
/// | **2** | **新建**、且本次**尚未**发送可见消息 | 31488、11356 |
/// | **1** | **已可见**（已发/正发可见消息） | 31464、31490、31527、11363 |
/// | **0** | **本帧未被确认**，标记待清除 | 31705、31721、15667 |
///
/// 配合 31947/31975 的清理逻辑：每帧先把所有条目置 **0**（31705/31721），
/// 遍历中凡是再次确认可见的会被重新置 **1**（31464/31527/11363），
/// 帧末凡仍为 **0** 者即 `Delete` 并 `Dispose`。
/// **因此"未被本次遍历触达"与"被显式标记隐藏"最终都归结为同一结果——从表中移除**，
/// 但它们的语义来源完全不同：前者是"看不见了"，后者是"看见了但要求隐藏"。
///
/// **`boSendItemShow` 决定新建条目的初值是 1 还是 2（31487-31490）**：
/// - 调用方**已在本次**发送过 `RM_ITEMSHOW`（`boSendItemShow = True`）→ 置 **1**，
///   表示"已经可见了"，帧末不会被误清；
/// - 未发送（`False`）→ 置 **2**，表示"新建但还没发"，留给后续逻辑决定是否发。
/// 若把这两个初值写反，新掉落的物品会在下一帧被立刻清掉（永远显示不出来）或
/// 永远停在"未发送"状态（重复发送可见消息）。
///
/// **`HideItem` 与 `UpdateVisibleItem` 的配合**（11337-11338）：
/// `HideItem` **不删条目、也不改 `nVisibleFlag`**，只置 `boSendHideMsg := True`；
/// 真正的移除仍由帧末 `nVisibleFlag = 0` 的清理负责。
/// 即"隐藏"是一个**延迟到帧末**的动作，而非立即删除——这样同一帧内
/// 先发的 `RM_ITEMSHOW` 与后发的隐藏标记可以共存而不会撕裂。
///
/// **`UpdateVisibleEvent` 有两处刻意的不对称**（11344-11364）：
/// ① 新建时 `nVisibleFlag` **恒为 2**，**没有** `boSendItemShow` 那样的 1/2 分支；
/// ② 其键是 `IntToStr(NativeInt(MapEvent))`——**指针转字符串**，而非对象身份。
/// </summary>
public static class VisibleItemLifecycleCore
{
    /// <summary>`nVisibleFlag` 的三态常量（原文以字面量 0/1/2 直接书写，此处具名以便阅读）。</summary>
    public const byte FlagStale = 0;
    public const byte FlagVisible = 1;
    public const byte FlagNewUnsent = 2;

    // ===================== 依赖的两个全局函数（M2Share.pas） =====================

    /// <summary>`CheckOverLapItem`(M2Share.pas 11028-11032)：可叠加物品判定。
    /// 三个条件**同时**成立：`OverLap > 0`、`StdMode` 属于 {0, 2, 3, 31, 40, 41, 42, 46, 47}、`DuraMax > 1`。</summary>
    public static bool CheckOverLapItem(int overlap, int stdMode, int duraMax)
    {
        return overlap > 0
               && IsOverLapStdMode(stdMode)
               && duraMax > 1;
    }

    /// <summary>11031：可叠加以外的 StdMode 集合（原文 `in [0, 2, 3, 31, 40, 41, 42, 46, 47]`）。</summary>
    public static bool IsOverLapStdMode(int stdMode)
        => stdMode == 0 || stdMode == 2 || stdMode == 3 || stdMode == 31
           || stdMode == 40 || stdMode == 41 || stdMode == 42 || stdMode == 46 || stdMode == 47;

    /// <summary>
    /// `MakeLong(a, b)`——Delphi RTL：`(a and $FFFF) or ((b and $FFFF) shl 16)`。
    /// 低 16 位为 `a`、高 16 位为 `b`。
    /// </summary>
    public static int MakeLong(int a, int b)
        => (a & 0xFFFF) | ((b & 0xFFFF) << 16);

    /// <summary>与 <see cref="MakeLong"/> 互逆：取低 16 位。</summary>
    public static int LoWord(int value) => value & 0xFFFF;

    /// <summary>与 <see cref="MakeLong"/> 互逆：取高 16 位。</summary>
    public static int HiWord(int value) => (value >> 16) & 0xFFFF;

    /// <summary>
    /// `GetDropItemValue`(M2Share.pas 33204-33265)：物品是否含极品属性。
    /// **三道前置闸门**：`StdItem = nil` → 0；`UserItem = nil` → 0；
    /// **`g_Config.ClientConfigs[83]` 为假 → 0**（33216-33217）。
    ///
    /// 随后按 `StdMode` 分四组判定 `btValue[0..N]` 是否有任一 `> 0`：
    /// - 组 A（`5,6,68,69`）：看 `btValue[0..7]`（**8 项**）
    /// - 组 B（`10,11,12,28,65,66,67`、`19,70,75`、`20,24,52,71,76,79,86`、`21,54,72,77,84`、
    ///   `23,74,82`、`53`、`15,16,22,26,51,62,63,64,29,30,73,78`）：看 `btValue[0..4]`（**5 项**）
    /// - 其他 `StdMode`：`boRet` 保持 `False` → 返回 0
    ///
    /// **注意组 A 比其他组多看 3 个槽位**（0..7 vs 0..4）——这是原文如此，不是笔误；
    /// 已用 `GroupAWatchesEightSlots` 与 `OtherGroupsWatchFiveSlots` 分别固化。
    /// 最后 `Result := Byte(boRet)`，即返回 **0 或 1**，而非"属性数量"。
    /// </summary>
    public static byte GetDropItemValue(
        bool stdItemIsNull, bool userItemIsNull, bool clientConfig83,
        int stdMode, byte[] btValue)
    {
        if (stdItemIsNull) return 0;
        if (userItemIsNull) return 0;
        if (!clientConfig83) return 0;

        bool boRet;

        if (IsGroupA(stdMode))
        {
            boRet = AnyPositive(btValue, 8);
        }
        else if (IsGroupB(stdMode))
        {
            boRet = AnyPositive(btValue, 5);
        }
        else
        {
            boRet = false;
        }

        return (byte)(boRet ? 1 : 0);
    }

    /// <summary>33222：组 A——`5, 6, 68, 69`。</summary>
    public static bool IsGroupA(int stdMode) => stdMode == 5 || stdMode == 6 || stdMode == 68 || stdMode == 69;

    /// <summary>33227-33257：组 B——所有其余列出的 StdMode。</summary>
    public static bool IsGroupB(int stdMode) => Array.IndexOf(GroupBStdModes, stdMode) >= 0;

    /// <summary>组 B 的完整 StdMode 表（由原文 33227-33257 逐段抽取，共 37 项）。</summary>
    public static readonly int[] GroupBStdModes =
    {
        10, 11, 12, 28, 65, 66, 67,          // 33227
        19, 70, 75,                          // 33232
        20, 24, 52, 71, 76, 79, 86,          // 33237
        21, 54, 72, 77, 84,                  // 33242
        23, 74, 82,                          // 33247
        53,                                  // 33252
        15, 16, 22, 26, 51, 62, 63, 64, 29, 30, 73, 78,   // 33257
    };

    private static bool AnyPositive(byte[] btValue, int count)
    {
        for (int i = 0; i < count && i < btValue.Length; i++)
            if (btValue[i] > 0) return true;

        return false;
    }

    // ===================== UpdateVisibleItem（31476-31530） =====================

    /// <summary>`UpdateVisibleItem` 的参数集合。</summary>
    public sealed class UpdateItemArgs
    {
        public int WX;
        public int WY;

        /// <summary>`TItemObject` 身份（用于 `m_VisibleItems` 的键与 `BaseObject` 比较）。</summary>
        public object? MapItem;

        public bool BoSendItemShow;

        public byte MapItemColor;
        public ushort UserItemEffect;
        public ushort UserItemIndex;
        public int UserItemDura;
        public string MapItemDBName = "";
        public string MapItemName = "";
        public ushort MapItemLooks;

        /// <summary>`g_DropEffectItemList.IndexOf(m_sDBName)` 的结果，-1 表示未命中。</summary>
        public int DropEffectIndex = -1;

        /// <summary>命中时 `g_DropEffectItemList.Objects[Index]` 的值。</summary>
        public int DropEffectValue;

        /// <summary>`UserEngine.GetStdItem(wIndex)` 是否返回非 nil。</summary>
        public bool StdItemFound;

        /// <summary>`CheckOverLapItem(StdItem)` 的结果。</summary>
        public bool IsOverLap;

        /// <summary>`g_Config.ClientConfigs[83]`。</summary>
        public bool ClientConfig83;
    }

    /// <summary>
    /// `UpdateVisibleItem`(31476-31530) 1:1。
    /// 条目已存在且 `BaseObject` **引用相等**时仅置 `nVisibleFlag := 1`（31525-31528）；
    /// 否则**新建**并填齐全部字段。
    /// `boSendItemShow` 为 `True` → 初值 **1**，否则 **2**（31487-31490）。
    /// </summary>
    public static TVisibleMapItem UpdateVisibleItem(
        Dictionary<nint, TVisibleMapItem> visibleItems, nint key, UpdateItemArgs args)
    {
        visibleItems.TryGetValue(key, out var existing);

        if (existing is null)
        {
            var item = new TVisibleMapItem();

            // 31487-31490：已发送可见消息则为 1，否则为 2
            item.nVisibleFlag = args.BoSendItemShow ? FlagVisible : FlagNewUnsent;

            item.nX = args.WX;
            item.nY = args.WY;
            item.btColor2 = args.MapItemColor;
            item.BaseObject = args.MapItem;
            item.boSendHideMsg = false;

            // 31497-31505：物品自带特效优先；为 0 时再查掉落特效表
            int wEffect = args.UserItemEffect;
            if (wEffect == 0 && args.DropEffectIndex >= 0)
                wEffect = args.DropEffectValue;

            // 31506-31514：可叠加物品把 Dura+1 作为叠加数写入低 16 位
            if (args.StdItemFound && args.IsOverLap)
                item.OverlapCountAndEffect = MakeLong(args.UserItemDura + 1, wEffect);
            else
                item.OverlapCountAndEffect = MakeLong(0, wEffect);

            // 31515
            byte[] btValue = Array.Empty<byte>();
            item.boValue = GetDropItemValue(
                !args.StdItemFound, false, args.ClientConfig83, 0, btValue) != 0;

            // 31516-31519：DBName 非空且与显示名不同时，用 "显示名/DB名"
            if (args.MapItemDBName != ""
                && !string.Equals(args.MapItemName, args.MapItemDBName, StringComparison.OrdinalIgnoreCase))
            {
                item.sName = args.MapItemName + "/" + args.MapItemDBName;
            }
            else
            {
                item.sName = args.MapItemName;
            }

            item.wLooks = args.MapItemLooks;
            visibleItems[key] = item;

            return item;
        }

        // 31525-31528：**仅当引用相等**才置 1；不相等则什么都不做
        if (ReferenceEquals(existing.BaseObject, args.MapItem))
            existing.nVisibleFlag = FlagVisible;

        return existing;
    }

    // ===================== HideItem（ObjPlayer.pas 11330-11342） =====================

    /// <summary>
    /// `HideItem`(11330-11342) 1:1：**只置 `boSendHideMsg := True`**，
    /// 不删除条目、不改 `nVisibleFlag`。
    /// 三个前置条件**全部**满足才置位：条目非空、其 `BaseObject` 非空、且**引用相等**（11337）。
    /// </summary>
    public static bool HideItem(
        Dictionary<nint, TVisibleMapItem> visibleItems, nint key, object? mapItem)
    {
        visibleItems.TryGetValue(key, out var item);

        if (item is not null && item.BaseObject is not null && ReferenceEquals(item.BaseObject, mapItem))
        {
            item.boSendHideMsg = true;
            return true;
        }

        return false;
    }

    // ===================== UpdateVisibleEvent（11344-11364） =====================

    /// <summary>
    /// `UpdateVisibleEvent`(11344-11364) 1:1。
    /// **键是指针转字符串**（`IntToStr(NativeInt(MapEvent))`，11349），
    /// 新建时 `nVisibleFlag` **恒为 2**（11356，无 `boSendItemShow` 分支）。
    /// 原文 11351-11352 有一行被注释掉的"修复假人卡"逻辑，本移植保留其注释痕迹。
    /// </summary>
    public static TVisibleMapEvent UpdateVisibleEvent(
        Dictionary<string, TVisibleMapEvent> visibleEvents, string key,
        int wX, int wY, object? mapEvent)
    {
        visibleEvents.TryGetValue(key, out var existing);

        if (existing is null)
        {
            var ev = new TVisibleMapEvent
            {
                nVisibleFlag = FlagNewUnsent,     // 11356：**恒为 2**
                nX = wX,
                nY = wY,
                BaseObject = mapEvent,
            };
            visibleEvents[key] = ev;
            return ev;
        }

        // 11362-11363：引用相等才置 1
        if (ReferenceEquals(existing.BaseObject, mapEvent))
            existing.nVisibleFlag = FlagVisible;

        return existing;
    }

    /// <summary>11349：事件表的键由对象指针转换而来。</summary>
    public static string EventKey(nint mapEvent) => mapEvent.ToString();

    // ===================== 帧末清理（31705/31721/31947/31975） =====================

    /// <summary>31705 / 31721 / 15667：遍历中把每个条目置为 `FlagStale`(0)。</summary>
    public static void MarkAllStale<T>(IEnumerable<T> entries, Func<T, byte> getFlag, Action<T, byte> setFlag)
    {
        foreach (var e in entries)
        {
            if (e is not null)
                setFlag(e, FlagStale);
        }
    }

    /// <summary>
    /// 31944-31955 / 31972-31980：帧末清理——**倒序**遍历，凡 `nVisibleFlag = 0` 者删除。
    /// 原文两处均为 `downto 0` 倒序（31944、31972），故此处按索引倒序收集待删键。
    /// </summary>
    public static List<nint> SweepStaleKeys(IReadOnlyList<KeyValuePair<nint, TVisibleBaseObject>> ordered)
    {
        var toRemove = new List<nint>();

        for (int i = ordered.Count - 1; i >= 0; i--)
        {
            var v = ordered[i].Value;
            if (v is not null && v.nVisibleFlag == FlagStale)
                toRemove.Add(ordered[i].Key);
        }

        return toRemove;
    }

    /// <summary>31975：物品表清理同规则（`nVisibleFlag = 0` 即删）。</summary>
    public static List<nint> SweepStaleItemKeys(IReadOnlyList<KeyValuePair<nint, TVisibleMapItem>> ordered)
    {
        var toRemove = new List<nint>();

        for (int i = ordered.Count - 1; i >= 0; i--)
        {
            var v = ordered[i].Value;
            if (v is not null && v.nVisibleFlag == FlagStale)
                toRemove.Add(ordered[i].Key);
        }

        return toRemove;
    }
}
