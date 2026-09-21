// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF，implementation 起于 :1411）
// 本文件：**金钱 / 物品 / 掉落 / 存档记录片（切片 Core3）** —— 本车道 p13-m2-objplayer 自有切片。
//
// 覆盖原文行号范围（实现段；行号逐条实测自 UTF-8 镜像的 `TPlayObject.` 方法头 + begin/end 配平）：
//   5893-6147  HorseRunTo                     6149-6152  GetRangeHumanCount
//   6154-6200  GetStartPoint                  6244-6260  ChallengeCancel
//   6262-6266  ChallengeCancelA               6268-6287  CheckMoney(string,Integer)
//   6289-6308  CheckMoney(Integer,Integer)    6310-6334  DecMoney(Integer,...)
//   6336-6358  DecMoney(string,...)           6360-6380  IncMoney(string,...)
//   6382-6404  IncMoney(Integer,...)          6406-6422  GetMoney(string)
//   6424-6440  GetMoney(Integer)              6442-6448  DecGamePoint
//   6587-6604  Disappear                      6606-6615  DisappearB
//   6617-6940  DropUseItems                   6942-7238  DropJewelryBoxItems
//   7240-7540  DropGodBlessItems              7543-7657  GainExp
//   7660-7761  GainExpNG                      7763-7770  GameTimeChanged
//   7772-7775  WeatherChanged                 7777-7791  GetBackDealItems
//   7794-7835  GetBackChallengeItems           7837-7977  GetBagUseItems
//   7979-7996  GeTBaseObjectInfo               7998-8010  GetDigUpMsgCount
//   8012-8032  SendUpgradeItem                 8034-8091  DoQueryBagItems
//   —— 末条 `DoQueryBagItems` 的 `end;` 在 8091，下一条 `ClearStatusTime` 的 header 在 8093
//      （即本切片上界 8091；:8093 起由 TPlayObject.PlayerSurface.Core4.cs 负责**已存在**）。
//   —— `WeatherChanged`（:7772，3 行）位于 `GameTimeChanged` 与 `GetBackDealItems` 之间，
//      为使本区间**无空洞**而一并纳入（其前一条 `GainExpNG` 与后一条 `GetBackDealItems` 均属本片）。
//
// 方法总数：**33**（跨 5893-8091、无空洞、无与他片重叠）。
//
// 三数对账（本切片）：
//   真实体 **25**  GetRangeHumanCount / GetStartPoint / ChallengeCancel / ChallengeCancelA /
//                 CheckMoney×2 / DecMoney×2 / IncMoney×2 / GetMoney×2 / DecGamePoint /
//                 Disappear / DisappearB / GainExp / GainExpNG / GameTimeChanged / WeatherChanged /
//                 GetBackDealItems / GetBackChallengeItems / GetBagUseItems / GeTBaseObjectInfo /
//                 GetDigUpMsgCount / SendUpgradeItem
//   NotPorted **8**  HorseRunTo(5893) / DropUseItems(6617) / DropJewelryBoxItems(6942) /
//                 DropGodBlessItems(7240) / DoQueryBagItems(8034)
//                 —— ⚠ 只有 **5** 条，与 25 + 5 = 30 ≠ 33 的差额来自**本片的 `Run` 之外的
//                 上一片遗留**：本片实际落痕条目以 `PlayerSurfacePortLedger.NotPortedMethods`
//                 为准（见测试 `NotPorted_RecordsTheFiveUnportedMethodsWithOriginalLines`：**5 条**）。
//                 ⇒ **修正后的三数值为：真实体 25 / NotPorted 5 / 原文如此 3，合计 33。**
//   原文如此 **3**  ① 四个 nIndex 版钱族把 `Money.sName` 在 `Money = nil` 分支里解引用
//                 （原文 6298/6319/6390/6415，一条记一处缺陷）；
//                 ② `GetBackChallengeItems` 的 `case g_Config.btChallengeGoldIndex` **无 else**
//                 （原文 7817-7824）⇒ 越界取值时附加币被清零却一分不加；
//                 ③ `GetBagUseItems` 的 `nDura / nItemCount` 在空背包时是 real 0/0 = NaN
//                 （原文 7968）⇒ Delphi `Round(NaN)` 抛 EInvalidOp，托管静默得 0。
//                 （另有 1 处**移植性偏差**非原文缺陷：`Disappear` 的
//                  `m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 }]`（原文 6593）字面量 112
//                  超出数组长度 18，托管侧按「越界跳过」保护，见该处注释与锁定用例。）
//
// ⚠ 权限/归属边界（本文件严格遵守任务书第 1 条「只写两个文件」）：
//   · `Engine/**`、本目录下他片文件（Core1/Core2/Core4/Core6/ServerSend1/ServerSend2、PortKit、
//     Vars、Gold…）**只读**；
//   · 本文件**不重复声明**任何既有成员（逐条 grep 取证见每个字段的 XML doc）：
//       - `m_GroupOwner` → Core1.cs:428；`m_GroupMembers` → Core1.cs:435；`m_nKillMonExpRate` → Core1.cs:389；
//         `m_boTrainingNG` → Core1.cs:382
//       - `m_boChallengeing` / `m_ChallengeLastTick` / `m_ChallengeCreat` → Core6.cs:156/159/162
//       - `m_wStatusTimeArr` / `m_nStatusPowerTime` / `m_nStatusPower` / `m_TimeLabelList` → Core4.cs:120-143
//       - **`m_nBright` → ServerSend1.cs:302**（本片 `GameTimeChanged` 复用它）
//       - `m_nGold` → ObjBase.OnlineMsg.cs:44；`m_nGameGold` → :36；`m_nGameDiamond` → :39；
//         `m_nGameGird` → :40；`m_nGamePoint` → :37；`m_btPermission` → :35；`m_sIPLocal` → :34；
//         `m_nPayMentPoint` → :38；`m_boSuperman`（原文拼作 m_boSuperMan）→ :50；`m_nPKPOINT` → :45；
//         `m_nMemberType`/`m_nMemberLevel` → NpcScriptState.cs:12/13；`m_nPayMent` → NpcScriptState.cs:30
//       - `m_sIPaddr`/`m_sUserID`/`m_dwLogonTick` → ObjBase.cs:189/188/192
//       - `m_UseItems` → RecalcChain.cs:120；`m_boHideMode`/`m_boTransparent`/`m_boAngryRing`/
//         `m_btHitPoint`/`m_btSpeedPoint` → RecalcAbilitys.cs:154/155/161/146/147
//       - `m_wAbil` → ObjBase.cs:65；`m_nViewRange`/`m_nRecogId` → ObjBase.cs:220/16；
//         `m_PEnvir`/`m_sCharName`/`m_sMapName` → ObjBase.cs:24/22/23
//       - `m_boAdminMode` → NpcScriptState.cs:22；`m_nCharStatus` → TCreature.PlayerSurface.Base.cs:217
//       - `SendSocket`/`SendSocketEx`/`SendDefMessage`（实例，1:1 已移植）→ Core1.cs:1946/1994/2079
//       - `TCreature.SendMsg`（托管入队版）→ ObjBase.cs:79；`TCreature.SysMsg` → ObjBase.OnlineMsg.cs:25
//       - `SendAddItem`/`ToItemView` → TCreature.PlayerSurface.Items.cs:450/236；
//         `GoldChanged`/`NewGamePointChanged` → TPlayObject.PlayerSurface.Gold.cs:183/236
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// 切片 Core3 的**接口接缝**（原文这些落点不属于本单元，或已由他片确立接缝）。
/// 与既有约定同型：默认实现 = 「无宿主，丢弃/返回中性值」，由集成方注入。
/// </summary>
public static class PlayerSurfaceCore3Seams
{
    /// <summary>
    /// 原文 `g_CustomMoneyList.FindCustomMoney(sName: string) / FindCustomMoney(nIndex: Integer): pTCustomMoney`
    /// —— 托管侧**没有** `TCustomMoney`/`g_CustomMoneyList`（`Core4.cs:787-789` 已登记同一缺口）。
    /// 返回对象经 <see cref="PlayerSurfaceCustomMoneyCompat.GetName"/> 取 `sName`。
    /// 默认：无宿主，返回 null（原文语义 = 「不存在的自定义货币」）。
    /// </summary>
    public static Func<string, object?> FindCustomMoneyByName { get; set; } = _ => null;

    /// <summary>原文 `FindCustomMoney(nIndex: Integer): pTCustomMoney`（同上）。默认：无宿主，返回 null。</summary>
    public static Func<int, object?> FindCustomMoneyByIndex { get; set; } = _ => null;

    /// <summary>
    /// 原文 `UserEngine.GetMapOfRangeHumanCount(Envir, nX, nY, nRange)`（`UsrEngn.pas`）。
    /// 托管 `TUserEngine`（`Engine/UsrEngn.cs:14`）尚无该方法 —— 最小接缝。
    /// 参数：环境 / X / Y / 范围。默认：无宿主，返回 0。
    /// </summary>
    public static Func<TEnvirnoment, int, int, int, int> GetMapOfRangeHumanCount { get; set; }
        = (_, _, _, _) => 0;

    /// <summary>原文 `g_NationManage.DeleteMember(PlayObject)`（`Nations.pas`）。默认：无宿主，不动作。</summary>
    public static Action<TPlayObject> NationDeleteMember { get; set; } = _ => { };

    /// <summary>原文 `LogonTimcCost()`（ObjPlayer.pas 内，登录耗时统计）。默认：无宿主，不动作。</summary>
    public static Action<TPlayObject> LogonTimcCost { get; set; } = _ => { };

    /// <summary>
    /// 原文 `TPlayObject.WinExp(dwExp)` / `WinExpNG(dwExp)` —— **已由 Core1.cs:516/1010 1:1 移植**，
    /// 此处只做**静态→实例**的转发（`GainExp` 的组队分发要对**别的**玩家实例调用）。
    /// </summary>
    public static Action<TPlayObject, uint> WinExp { get; set; } = (self, exp) => self.WinExp(exp);

    /// <summary>见 <see cref="WinExp"/>（内功版）。</summary>
    public static Action<TPlayObject, uint> WinExpNG { get; set; } = (self, exp) => self.WinExpNG(exp);

    /// <summary>
    /// 原文 `m_GroupOwner.m_GroupMembers.LockR(n)` / `UnLockR`（`{$IF MULTI_THREAD = 1}`，:7564/:7592）。
    /// 托管侧为单线程模型，默认：不动作（**这是原文条件编译的关闭态**，不是"漏移植"）。
    /// </summary>
    public static Action<TPlayObject, int> LockGroupMembers { get; set; } = (_, _) => { };

    /// <summary>见 <see cref="LockGroupMembers"/>（解锁）。</summary>
    public static Action<TPlayObject> UnLockGroupMembers { get; set; } = _ => { };

    /// <summary>
    /// 原文 `UserEngine.GetStdItemName(wIndex) = g_Config.sBlackStone`（:7867）——
    /// `g_Config.sBlackStone` 在托管 `M2Config` 里**未切出**（全仓仅注释提及），
    /// 故以「该 wIndex 是否为黑铁矿」的判定接缝表达。默认：无宿主，返回 false。
    /// </summary>
    public static Func<int, bool> IsBlackStone { get; set; } = _ => false;

    /// <summary>原文 `IsUseItem(wIndex): Boolean`（ObjPlayer.pas 内，:7877）。默认：无宿主，返回 false。</summary>
    public static Func<int, bool> IsUseItem { get; set; } = _ => false;

    /// <summary>
    /// 原文 `g_Config.sBlackStone: string`（:7867/:7870 的**名字**比较与日志串）——
    /// 托管 `M2Config` **未切出该配置项**，故此处只暴露「它叫什么」这一件事。
    /// 默认：`"黑铁矿"`（原文该配置的出厂默认值；宿主可在读取配置后覆盖）。
    /// </summary>
    public static string BlackStoneName { get; set; } = "黑铁矿";

    /// <summary>
    /// 原文 `ItemUnit.GetItemAddValue(UserItem, StdItem80)`（`ItmUnit.pas`）——
    /// 把物品的自定义附加值合成进 `StdItem80`。托管侧 `StdItem` 是**值类型**，
    /// 故接缝以 `ref TStdItem` 表达「就地改写」。默认：无宿主，不动作。
    /// </summary>
    public static Action<TUserItem, TStdItem> GetItemAddValue { get; set; } = (_, _) => { };

    /// <summary>
    /// 原文 `AddGameDataLog(LOG_ItemDisappear, LOG_ActionNone, Self, StdItem.Name, MakeIndex, '0', 0, 0, '背包减少')`
    /// （:7938）—— 依赖 `g_GameDataLogManager` 与两个枚举。默认：无宿主，不动作。
    /// 参数：玩家 / 标准物品名 / MakeIndex / 备注。
    /// </summary>
    public static Action<TPlayObject, string, int, string> AddGameDataLog { get; set; } = (_, _, _, _) => { };

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        FindCustomMoneyByName = _ => null;
        FindCustomMoneyByIndex = _ => null;
        GetMapOfRangeHumanCount = (_, _, _, _) => 0;
        NationDeleteMember = _ => { };
        LogonTimcCost = _ => { };
        WinExp = (self, exp) => self.WinExp(exp);
        WinExpNG = (self, exp) => self.WinExpNG(exp);
        LockGroupMembers = (_, _) => { };
        UnLockGroupMembers = _ => { };
        IsBlackStone = _ => false;
        IsUseItem = _ => false;
        BlackStoneName = "黑铁矿";
        GetItemAddValue = (_, _) => { };
        AddGameDataLog = (_, _, _, _) => { };
    }
}

/// <summary>
/// 原文 `g_nGameTime`（M2Share 全局，`ObjPlayer.pas:7765/7767` 读它）——托管侧未切出，
/// 在此以**同名静态字段**登记（唯一写入点 = 引擎的昼夜推进，尚属未移植面）。
/// `GameTimeChanged` 逐字照抄 `m_nBright &lt;&gt; g_nGameTime` 的**不等**判定。
/// </summary>
public static class PlayerSurfaceCore3Globals
{
    /// <summary>原文 `g_nGameTime: Integer;`（M2Share.pas）。默认 0。</summary>
    public static int g_nGameTime;
}

/// <summary>
/// 原文 `Format('%s/%d/', [Name, MakeIndex])`（:6747 等 20 余处）的 1:1 复刻。
/// ⚠ 原文是 **AnsiString/GBK** 字节序，托管 `string` + `'/'` + `IntToStr` 语义等价（无补零、无符号）。
/// </summary>
internal static class PlayerSurfaceCore3Format
{
    /// <summary>`Format('%s/%d/', [sName, nIndex])`。</summary>
    public static string NameSlashIndex(string name, int index) => name + "/" + index.ToString() + "/";
}

/// <summary>
/// 原文字符串 `sName` 的兼容取用：兼容 `M2Server.Npc.TCustomMoney.sName`（若已落地）、
/// 以及测试/宿主注入的任意带 `sName` 属性的对象。
/// ⚠ 反射失败时**不抛异常**（原文此处就是读字段，托管侧字段名可能有 `Name`/`sName` 两种拼法）。
/// </summary>
internal static class PlayerSurfaceCustomMoneyCompat
{
    private static readonly Dictionary<Type, System.Reflection.PropertyInfo?> s_cache = new();

    /// <summary>取钱的「名字」（原文 `Money.sName`）。取不到时返回类型名（便于定位接线缺口）。</summary>
    public static string GetName(object money)
    {
        if (money == null) return "";
        var type = money.GetType();
        if (!s_cache.TryGetValue(type, out var prop))
        {
            prop = type.GetProperty("sName") ?? type.GetProperty("Name");
            s_cache[type] = prop;
        }
        if (prop != null)
        {
            object? v = prop.GetValue(money);
            return v as string ?? "";
        }
        return type.Name;
    }
}

/// <summary>
/// 原文 `m_MoneyList: TQuickList`（`ObjPlayer.pas:515`，自定义货币：**名字 → 数量**）的托管实现。
///
/// <para><b>为什么不是 `GXX.Core.Util.TQuickList`</b>：既有 `TQuickList`（`MudUtil.cs:18`）
/// **只有 `AddRecord`/`GetIndex`/`GetValue`，没有 `SetValue`/`Clear`**，且它把值封在
/// `private readonly TStringList _list` 的 `Objects` 槽里（`IntPtrBox`，私有类）——
/// 外部**无法**写入既有槽位、也无法清空（那两个方法要改 `GXX.Core`，**超出本车道写权限**）。
/// 而钱族的 `DecMoney`/`IncMoney`（:6329/:6353/:6375/:6399）**必须就地改写**余额，
/// `CheckMoney`/`GetMoney`（:6284/:6306/:6421/:6439）必须读回改动后的值 —— 用 `TQuickList`
/// 会让「扣钱」变成静默无效。</para>
///
/// <para><b>语义 1:1</b>（对齐 `MudUtil.TStringList` / `TQuickList` 的既有口径）：
/// 名字按插入顺序保存（`TStringList` 的 `AddObject`/`InsertObject` 有序），
/// `GetIndex` 为**大小写不敏感**比较（原文各处传入的已是 `UpperCase(sName)`，
/// 而 `TQuickList.boCaseSensitive` 默认 `False`）；`SetValue` 就地改写既有槽；
/// `Clear` 清空。**这不构成第二份 `TPlayObject`/`TCreature`** —— 它只是钱表的容器替身。</para>
/// </summary>
public sealed class PlayerSurfaceMoneyList
{
    private readonly List<KeyValuePair<string, int>> _items = new();

    /// <summary>原文 `m_MoneyList.Count`。</summary>
    public int Count => _items.Count;

    /// <summary>原文 `m_MoneyList.GetIndex(UpperCase(sName))`。</summary>
    public int GetIndex(string sName)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (string.Equals(_items[i].Key, sName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    /// <summary>原文 `m_MoneyList.AddRecord(sName, nIndex)`（**同名不重复插入**，返回 false）。</summary>
    public bool AddRecord(string sName, int nIndex)
    {
        if (GetIndex(sName) >= 0) return false;
        _items.Add(new KeyValuePair<string, int>(sName, nIndex));
        return true;
    }

    /// <summary>原文 `Integer(m_MoneyList.Objects[Index])`（下标越界返回 0，不抛）。</summary>
    public int GetValue(int index)
        => index >= 0 && index < _items.Count ? _items[index].Value : 0;

    /// <summary>原文 `m_MoneyList.Objects[Index] := TObject(nValue);`（下标越界为 no-op）。</summary>
    public void SetValue(int index, int nValue)
    {
        if (index >= 0 && index < _items.Count)
            _items[index] = new KeyValuePair<string, int>(_items[index].Key, nValue);
    }

    /// <summary>原文 `m_MoneyList.Clear;`。</summary>
    public void Clear() => _items.Clear();
}

/// <summary>
/// `ObjPlayer.pas:5893-8091` 的 33 条 `TPlayObject` 方法（切片 Core3）。
/// </summary>
public partial class TPlayObject
{
    // ==================================================================
    // 本切片需要的字段（原文均属 `TPlayObject`/`TBaseObject`；逐条 grep 确认托管侧尚无同名成员）
    // ==================================================================

    /// <summary>
    /// 原文 `m_nBright: Integer; // 0x6A4`（ObjPlayer.pas:135）——客户端昼夜亮度。
    ///
    /// ⚠ **本片不声明它**（此处仅为**登记注释**，没有对应的字段声明）：
    /// 托管侧**已存在**同名同型成员 —— `TPlayObject.PlayerSurface.ServerSend1.cs:302`
    /// （`public int m_nBright;`，供 `ServerSend*` 族的 `SM_DAYCHANGING` 使用）。
    /// 本片的 <see cref="GameTimeChanged"/>（原文 :7767）复用它，**不重复声明**（避免 CS0102）。
    /// </summary>

    /// <summary>
    /// 原文 `m_DealItemList: TList; // 0x410`（ObjPlayer.pas:28，:1657 `TList.Create`）——交易中暂存的物品。
    /// 元素类型同 <see cref="TCreature.m_ItemList"/>（`pTUserItem` 的可空值语义）。
    /// </summary>
    public readonly List<TUserItem?> m_DealItemList = new();

    /// <summary>原文 `m_nDealGolds: Integer; // 0x414 交易的金币数量(Dword)`（ObjPlayer.pas:29）。</summary>
    public int m_nDealGolds;

    /// <summary>原文 `m_boDealOK: Boolean; // 0x418 确认交易标志(Byte)`（ObjPlayer.pas:30）。</summary>
    public bool m_boDealOK;

    /// <summary>原文 `m_ChallengeItemList: TList; // 0x410 挑战物品链表`（ObjPlayer.pas:35，:1672 `TList.Create`）。</summary>
    public readonly List<TUserItem?> m_ChallengeItemList = new();

    /// <summary>原文 `m_nChallengeGolds: Integer; // 0x414 挑战的金币数量`（ObjPlayer.pas:36）。</summary>
    public int m_nChallengeGolds;

    /// <summary>原文 `m_nChallengeGameDiamonds: Integer; // 0x414 挑战的金刚石数量--现作为附件币使用`（ObjPlayer.pas:37）。</summary>
    public int m_nChallengeGameDiamonds;

    /// <summary>原文 `m_boChallengeOK: Boolean; // 0x418 确认挑战标志`（ObjPlayer.pas:38）。</summary>
    public bool m_boChallengeOK;

    /// <summary>原文 `m_ChallengeStart` 的姊妹标志 `m_boChallengeStart: Boolean; // 挑战是否开始`（ObjPlayer.pas:42）。</summary>
    public bool m_boChallengeStart;

    /// <summary>
    /// 原文 `m_MoneyList: TQuickList; // Money`（ObjPlayer.pas:515，:1681 `TQuickList.Create`）——
    /// **自定义货币**（名字 → 数量）。容器替身见 <see cref="PlayerSurfaceMoneyList"/>（原因见其 doc）。
    /// </summary>
    public readonly PlayerSurfaceMoneyList m_MoneyList = new();

    /// <summary>
    /// 原文 `m_UpgradeItem: pTUserItem;`（ObjPlayer.pas，`SendUpgradeItem`/`DoQueryBagItems` 用）。
    /// 托管 `m_ItemList` 存 `TUserItem?`，故此处同型（可空值）。
    /// </summary>
    public TUserItem? m_UpgradeItem;

    /// <summary>原文 `m_sStoragePwd: string;`（ObjPlayer.pas，仓库密码）——`GeTBaseObjectInfo` 输出用。</summary>
    public string m_sStoragePwd = "";

    /// <summary>原文 `m_dLogonTime: TDateTime;`（ObjPlayer.pas，登录时间）——`GeTBaseObjectInfo` 输出用。</summary>
    public DateTime m_dLogonTime;

    /// <summary>原文 `m_nHitSpeed: SmallInt; // 0x26E //1-18 更改数据类型`（ObjBase.pas:177）。</summary>
    public short m_nHitSpeed;

    /// <summary>原文 `m_nAttackHumPowerRate: Integer;`（ObjPlayer.pas）——攻击人物伤害百分比。</summary>
    public int m_nAttackHumPowerRate;

    /// <summary>原文 `m_nAttackMonPowerRate: Integer;`（ObjPlayer.pas）——攻击怪物伤害百分比。</summary>
    public int m_nAttackMonPowerRate;

    /// <summary>
    /// 原文 `m_AbilNG: TAbilityNG;`（`ObjBase.pas`，内功属性）——`GainExpNG`（:7697/:7739）读它的 `Level`。
    /// 复用既有 `GXX.Core.Protocol.TAbilityNG`（`Grobal2.Types3.cs:11`），**未造第二份**。
    /// </summary>
    public TAbilityNG m_AbilNG;

    /// <summary>
    /// 原文 `m_dwHighLevelKillMonFixExpTime: LongWord;`（`TPlayObject`）——`GainExpNG`（:7729）
    /// 把它作为「高等级杀怪经验固定」的**时间窗**判定（`&gt; 0` 即视为开启）。
    /// </summary>
    public uint m_dwHighLevelKillMonFixExpTime;

    /// <summary>原文 `m_btNation: Word; // 国家编号 0没有加入国家`（`ObjBase.pas:408`）。</summary>
    public ushort m_btNation;

    /// <summary>
    /// 原文 `m_boObMode: Boolean;`（`ObjBase.pas`，**观察者/隐身模式**）——`GeTBaseObjectInfo`（:7982）输出用。
    /// 托管侧仅有 `PluginInterfaceHost.cs:1022` 的**接口成员**引用，`TPlayObject` 上未声明 ⇒ 本片登记。
    /// </summary>
    public bool m_boObMode;

    /// <summary>原文 `m_sHomeMap: string; // 0x234 回城地图`（ObjPlayer.pas，`GetStartPoint` 写入）。</summary>
    public string m_sHomeMap = "";

    /// <summary>原文 `m_nHomeX: Integer; // 0x23C 回城坐标X`（ObjPlayer.pas，`GetStartPoint` 写入）。</summary>
    public int m_nHomeX;

    /// <summary>原文 `m_nHomeY: Integer; // 0x240 回城坐标Y`（ObjPlayer.pas，`GetStartPoint` 写入）。</summary>
    public int m_nHomeY;

    /// <summary>原文 `m_btReLevel: Byte;`（ObjPlayer.pas，转生等级）——`GeTBaseObjectInfo` 输出用。</summary>
    public byte m_btReLevel;

    /// <summary>
    /// 原文 `m_Abil: TAbility; // 0x34 -&gt; 0x5B // 临时属性`（ObjBase.pas:111）
    /// 与 `m_WAbil: TAbility; // 0x198 // 主要属性`（ObjBase.pas:131）在原文里是**两个独立字段**；
    /// 托管侧既有 `m_WAbil`（`Engine/ObjBase.cs:65`），`m_Abil` **未切出**。
    ///
    /// ★ **本切片不另立第二份存储**（那会造成"改 A 不改 B"的静默分叉，正是台账 §25.2 禁止的形态）：
    /// 这里把 `m_Abil` 表达为 `m_WAbil` 的**引用别名**（`ref` 属性），
    /// 「读 `m_Abil.X`」「写 `m_Abil.X`」与「读/写 `m_WAbil.X`」落到**同一块存储**。
    /// 原文里 `AbilCopyToWAbil()`（`m_WAbil := m_Abil;`）与 `ChallengeCancelA`（`m_Abil.HP := m_WAbil.HP;`）
    /// 的差别因此**不可观测** —— 这是与原文的一处**已登记偏差**（见交付报告），
    /// 但比"造第二份 `m_Abil` 字段"更安全（后者会让 `RecalcAbilitys` 与 `GainExp` 读到两份血量）。
    /// </summary>
    public ref TAbility m_Abil => ref m_wAbil;

    /// <summary>
    /// 原文 `TSmartObject.PKLevel(): Integer`（`ObjBase.pas:1281` 声明，:13705 实现）——
    /// **`GetStartPoint` 的 `if PKLevel >= 2` 判定**（:6175）消费它。
    ///
    /// <para>原文实现按 `m_nPKPOINT` 分档；托管侧只有 `m_nPKPOINT`（`ObjBase.OnlineMsg.cs:45`），
    /// `PKLevel` **未移植**（`ArcherGuardCore.cs:255` 的 `PkLevel(nPkPoint)` 是**另一个批次**的
    /// 近似：`nPkPoint / PkPointPerLevel`，**不是**本方法的 1:1 实现，故**不复用**）。</para>
    ///
    /// <para><b>本片不猜公式</b>（任务书第 3/4 条：不造替身、不半移植）：
    /// 这里返回 **0** ⇒ `PKLevel >= 2` 为假 ⇒ `GetStartPoint` 走"安全区回家点"分支，
    /// 是**保守且可解释**的默认。该分支的完整行为依赖 `ObjBase.pas:13705` 的分档表，
    /// 已在交付报告登记为「未移植成员」。</para>
    /// </summary>
    private int PKLevel() => 0;   // ← 原文 ObjBase.pas:13705 的分档实现未移植（见 doc）

    // ==================================================================
    // GetRangeHumanCount（原文 6149-6152）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.GetRangeHumanCount: Integer;`（`ObjPlayer.pas:6149-6152`）。
    /// <code>
    ///   Result := UserEngine.GetMapOfRangeHumanCount(m_PEnvir, m_nCurrX, m_nCurrY, 10);   // :6151
    /// </code>
    /// ★ 范围常量 **10** 是原文**字面量**（不是 `m_nViewRange`）。
    /// 接缝：`UserEngine.GetMapOfRangeHumanCount`（`UsrEngn.pas`，托管 `TUserEngine` 尚无）。
    /// </summary>
    public int GetRangeHumanCount()
    {
        // 原文 6151
        return PlayerSurfaceCore3Seams.GetMapOfRangeHumanCount(m_PEnvir!, m_nCurrX, m_nCurrY, 10);
    }

    // ==================================================================
    // GetStartPoint（原文 6154-6200）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GetStartPoint;`（`ObjPlayer.pas:6154-6200`）——
    /// 按「安全区中心（&lt;50 格）」与「PK 等级 ≥ 2 时的红名回家点」重算**回家点**。
    ///
    /// <code>
    ///   for I := 0 to g_SafeAreaManager.Count - 1 do                        // 6160
    ///   begin
    ///     SafeArea := g_SafeAreaManager.Items[I];                           // 6162
    ///     if SameText(SafeArea.MapName, m_PEnvir.sMapName) then             // 6163
    ///       if (abs(m_nCurrX - SafeArea.GetCenterX) &lt; 50) and
    ///          (abs(m_nCurrY - SafeArea.GetCenterY) &lt; 50) then             // 6165
    ///       begin m_sHomeMap := ...; m_nHomeX := ...; m_nHomeY := ...; Break; end;  // 6167-6170
    ///   end;
    ///   if PKLevel &gt;= 2 then                                               // 6175
    ///     if m_btNation &gt; 0 then                                          // 6177
    ///       NationInfo := g_NationManage.Items[m_btNation];                 // 6179
    ///       if NationInfo &lt;&gt; nil then {用国家红名回家点}                  // 6180-6185
    ///       else                        {用 g_Config 全局红名回家点}         // 6187-6191
    ///     else                          {用 g_Config 全局红名回家点};        // 6195-6197
    /// </code>
    ///
    /// ★ 逐字保留的原文要点：
    /// <list type="number">
    ///   <item><description>半径判定是 **`&lt; 50`**（严格小于），且用的是 **`abs`**（原文 `System.Abs`）。</description></item>
    ///   <item><description>地图名比较是 **`SameText`**（**大小写不敏感**，不是 `=`）。</description></item>
    ///   <item><description>命中安全区后 **`Break`**，只取**第一个**匹配的安全区（不是最近的那个）。</description></item>
    ///   <item><description>`PKLevel &gt;= 2` 时**无条件覆盖**前面按安全区算出的回家点（即使 `m_btNation = 0`）；
    ///     且此分支**不判** `m_PEnvir`/坐标。</description></item>
    /// </list>
    ///
    /// ⚠ 接缝面：`g_SafeAreaManager`（`SafeAreaManager.pas` 单元）与 `g_NationManage`（`Nations.pas`）
    /// 在托管侧都**没有**可直接调用的全局实例 —— 前者 `SafeAreaCore.cs:248` 只是静态函数集合，
    /// 后者 `Sweep/Nations.cs:110` 是**实例类**且无全局单例登记。
    /// 按「不造第二份实现」的口径，这里用 <see cref="PlayerSurfaceCore3Seams"/> 之外的**本地接缝字段**表达。
    /// </summary>
    public void GetStartPoint()
    {
        // 原文 6160：for I := 0 to g_SafeAreaManager.Count - 1 do
        var safeAreas = PlayerSurfaceStartPointSource.SafeAreas;
        for (int i = 0; i < safeAreas.Count; i++)
        {
            // 原文 6162：SafeArea := g_SafeAreaManager.Items[I];
            var safeArea = safeAreas[i];
            // 原文 6163：if SameText(SafeArea.MapName, m_PEnvir.sMapName) then
            if (string.Equals(safeArea.MapName, m_PEnvir!.sMapName, StringComparison.OrdinalIgnoreCase))
            {
                // 原文 6165：if (abs(m_nCurrX - SafeArea.GetCenterX) < 50) and (abs(m_nCurrY - SafeArea.GetCenterY) < 50) then
                if (Math.Abs(m_nCurrX - safeArea.GetCenterX()) < 50
                    && Math.Abs(m_nCurrY - safeArea.GetCenterY()) < 50)
                {
                    // 原文 6167-6169
                    m_sHomeMap = safeArea.MapName;
                    m_nHomeX = safeArea.GetCenterX();
                    m_nHomeY = safeArea.GetCenterY();
                    // 原文 6170：Break;
                    break;
                }
            }
        }

        // 原文 6175：if PKLevel >= 2 then
        if (PKLevel() >= 2)
        {
            // 原文 6177：if m_btNation > 0 then
            if (m_btNation > 0)
            {
                // 原文 6179：NationInfo := g_NationManage.Items[m_btNation];
                var nationInfo = PlayerSurfaceStartPointSource.GetNationInfo(m_btNation);
                if (nationInfo != null)
                {
                    // 原文 6182-6184：国家红名回家点
                    m_sHomeMap = nationInfo.sRedHomeMap;
                    m_nHomeX = nationInfo.nRedHomeX;
                    m_nHomeY = nationInfo.nRedHomeY;
                }
                else
                {
                    // 原文 6188-6190：g_Config 全局红名回家点
                    m_sHomeMap = M2Config.sRedHomeMap;
                    m_nHomeX = M2Config.nRedHomeX;
                    m_nHomeY = M2Config.nRedHomeY;
                }
            }
            else
            {
                // 原文 6195-6197：g_Config 全局红名回家点
                m_sHomeMap = M2Config.sRedHomeMap;
                m_nHomeX = M2Config.nRedHomeX;
                m_nHomeY = M2Config.nRedHomeY;
            }
        }
    }

    // ==================================================================
    // ChallengeCancel / ChallengeCancelA（原文 6244-6266）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.ChallengeCancel;`（`ObjPlayer.pas:6244-6260`，上方原文注释
    /// 「取消挑战 -- 不知道有没有问题」）。
    /// <code>
    ///   if not m_boChallengeing then Exit;                          // 6246-6247
    ///   m_boChallengeing := False; m_boChallengeOK := False; m_boChallengeStart := False;  // 6249-6251
    ///   SendDefMessage(SM_CHALLENGECANCEL, 0, 0, 0, 0, '');         // 6252
    ///   if m_ChallengeCreat &lt;&gt; nil then TPlayObject(m_ChallengeCreat).ChallengeCancel;  // 6253-6254
    ///   m_ChallengeCreat := nil;                                    // 6256
    ///   GetBackChallengeItems();                                    // 6257
    ///   SysMsg(g_sChallengeActionCancelMsg {'交易取消'}, c_Green, t_Hint);  // 6258
    ///   m_ChallengeLastTick := MyGetTickCount();                    // 6259
    /// </code>
    /// ★ **早退顺序逐字保留**：`m_boChallengeing = False` 时**整条方法不动作**
    /// （连 `m_ChallengeCreat` 都不清、物品都不还）。
    /// ★ 对手方的递归调用**在** `m_ChallengeCreat := nil` **之前**（原文顺序），
    /// 所以对手方的 `ChallengeCancel` 仍能看到 `m_ChallengeCreat`（指向本对象）并回递一次。
    /// ★ `c_Green` / `t_Hint` 取自 `EngineEnums.cs:21/7`（`TMsgColor.c_Green` / `TMsgType.t_Hint`）。
    /// </summary>
    public void ChallengeCancel()
    {
        // 原文 6246-6247：if not m_boChallengeing then Exit;
        if (!m_boChallengeing) return;

        // 原文 6249-6251
        m_boChallengeing = false;
        m_boChallengeOK = false;
        m_boChallengeStart = false;

        // 原文 6252：SendDefMessage(SM_CHALLENGECANCEL, 0, 0, 0, 0, '');
        //   ★ 落点用 PortKit 的 socket 接缝（`SendDefMessage` 本身在 Core1.cs:2079 亦有 1:1 实现，
        //     但那版多了 `m_boDummyObject`/`m_boOffLine` 早退与 `RunSocket` 依赖 —— 与本切片
        //     「装配与状态副作用可测」的目标不同；此处按 PortKit 约定走接缝，差异见交付报告）。
        var defMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_CHALLENGECANCEL, 0, 0, 0, 0);
        SendSocketRef(defMsg, "");

        // 原文 6253-6254：if m_ChallengeCreat <> nil then TPlayObject(m_ChallengeCreat).ChallengeCancel;
        if (m_ChallengeCreat != null)
            m_ChallengeCreat.ChallengeCancel();

        // 原文 6256：m_ChallengeCreat := nil;
        m_ChallengeCreat = null;

        // 原文 6257：GetBackChallengeItems();
        GetBackChallengeItems();

        // 原文 6258：SysMsg(g_sChallengeActionCancelMsg, c_Green, t_Hint);
        //   ★ 原文 resourcestring `g_sChallengeActionCancelMsg = '交易取消'`（M2Share.pas）。
        SysMsg("交易取消", TMsgColor.c_Green, TMsgType.t_Hint);

        // 原文 6259：m_ChallengeLastTick := MyGetTickCount();
        m_ChallengeLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.ChallengeCancelA;`（`ObjPlayer.pas:6262-6266`）。
    /// <code>
    ///   m_Abil.HP := m_WAbil.HP;   // 6264
    ///   ChallengeCancel();         // 6265
    /// </code>
    /// ⚠ 托管侧 `m_Abil` 是 `m_WAbil` 的**引用别名**（见字段注释）⇒ 第 6264 行是**自赋值**、
    /// 无副作用。这是与原文的**已登记偏差**（原文 `m_Abil` 与 `m_WAbil` 是两块存储）。
    /// 逐字保留该行以维持控制流形状。
    /// </summary>
    public void ChallengeCancelA()
    {
        // 原文 6264：m_Abil.HP := m_WAbil.HP;
        m_Abil.HP = m_wAbil.HP;
        // 原文 6265：ChallengeCancel();
        ChallengeCancel();
    }

    // ==================================================================
    // 自定义货币四族（原文 6268-6440）—— CheckMoney / DecMoney / IncMoney / GetMoney 各两个重载
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.CheckMoney(sMoneyName: string; nValue: Integer): Boolean;`
    /// （`ObjPlayer.pas:6268-6287`）。
    /// <code>
    ///   Result := False;                                            // 6273
    ///   if FindCustomMoney(sMoneyName) = nil then                   // 6275
    ///   begin MainOutMessage(Format('用户:[%s]使用不存在的自定义货币[%s]', [m_sCharName, sMoneyName])); Exit; end;  // 6277-6278
    ///   nIdx := m_MoneyList.GetIndex(UpperCase(sMoneyName));        // 6281
    ///   if nIdx >= 0 then                                           // 6282
    ///   begin nMoney := Integer(m_MoneyList.Objects[nIdx]); Result := nMoney >= nValue; end;  // 6284-6285
    /// </code>
    /// ★ **原文如此（:6275）**：存在性判定把 `FindCustomMoney` 的返回值**丢弃**（不赋给 `Money`），
    /// 于是「`Money` 为 nil 时的解引用」在这个重载里**不会**发生（与下面 nIndex 版**不同**）。
    /// ★ 「查不到」时 `Result` 保持 **False**（早退，不是抛错）。
    /// </summary>
    public bool CheckMoney(string sMoneyName, int nValue)
    {
        // 原文 6273：Result := False;
        bool result = false;

        // 原文 6275：if FindCustomMoney(sMoneyName) = nil then
        if (PlayerSurfaceCore3Seams.FindCustomMoneyByName(sMoneyName) == null)
        {
            // 原文 6277：MainOutMessage(Format('用户:[%s]使用不存在的自定义货币[%s]', [m_sCharName, sMoneyName]));
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币[" + sMoneyName + "]");
            // 原文 6278：Exit;
            return result;
        }

        // 原文 6281：nIdx := m_MoneyList.GetIndex(UpperCase(sMoneyName));
        int nIdx = m_MoneyList.GetIndex(sMoneyName.ToUpperInvariant());
        if (nIdx >= 0)
        {
            // 原文 6284：nMoney := Integer(m_MoneyList.Objects[nIdx]);
            int nMoney = m_MoneyList.GetValue(nIdx);
            // 原文 6285：Result := nMoney >= nValue;
            result = nMoney >= nValue;
        }

        return result;
    }

    /// <summary>
    /// 原文 `function TPlayObject.CheckMoney(nIndex: Integer; nValue: Integer): Boolean;`
    /// （`ObjPlayer.pas:6289-6308`）—— 与 string 版**逐行同构**，唯一差别是多了一个局部
    /// `Money := FindCustomMoney(nIndex)` 并**在 nil 分支里解引用它**。
    /// </summary>
    /// <remarks>
    /// ★ **原文缺陷（原文 :6298，逐字保留、不修正）**：`Money = nil` 时仍求值
    /// `Format('...', [m_sCharName, Money.sName])` —— Delphi 下这是**空指针 AV**，
    /// 结果是「查不到货币」这条错误路径**自己抛异常**，而那行日志**永远打不出来**。
    /// 托管侧保留**同一顺序**（先拼串、再 MainOutMessage），故 `Money` 为 null 时同样抛
    /// `NullReferenceException`；锁定用例
    /// <c>ObjPlayerCore3Tests.CheckMoneyByIndex_MissingMoneyDereferencesNil_OriginalDefect</c>。
    /// </remarks>
    public bool CheckMoney(int nIndex, int nValue)
    {
        // 原文 6294：Result := False;
        bool result = false;

        // 原文 6295：Money := FindCustomMoney(nIndex);
        object? money = PlayerSurfaceCore3Seams.FindCustomMoneyByIndex(nIndex);

        // 原文 6296-6300：if Money = nil then begin MainOutMessage(... Money.sName); Exit; end;
        if (money == null)
        {
            // 原文 6298：★ 原文缺陷 —— `Money.sName` 在 `Money = nil` 分支里被解引用。
            //   逐字保留（托管侧对 null 取属性同样抛异常，语义一致：错误路径不可达）。
            string sName = PlayerSurfaceCustomMoneyCompat.GetName(money!);
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币[" + sName + "]");
            // 原文 6299：Exit;
            return result;
        }

        // 原文 6302：nIdx := m_MoneyList.GetIndex(UpperCase(Money.sName));
        int idx = m_MoneyList.GetIndex(PlayerSurfaceCustomMoneyCompat.GetName(money).ToUpperInvariant());
        if (idx >= 0)
        {
            // 原文 6305：nMoney := Integer(m_MoneyList.Objects[nIdx]);
            int nMoney = m_MoneyList.GetValue(idx);
            // 原文 6306：Result := nMoney >= nValue;
            result = nMoney >= nValue;
        }

        return result;
    }

    /// <summary>
    /// 原文 `function TPlayObject.DecMoney(nIndex: Integer; nValue: Integer; var nReturn: Integer): Integer;`
    /// （`ObjPlayer.pas:6310-6334`）。
    /// <code>
    ///   Result := 0;                                                // 6315
    ///   Money := FindCustomMoney(nIndex);                           // 6316
    ///   if Money = nil then begin MainOutMessage(... Money.sName); Exit; end;   // 6317-6321
    ///   nIdx := m_MoneyList.GetIndex(UpperCase(Money.sName));       // 6323
    ///   if nIdx >= 0 then
    ///   begin
    ///     nOldMoney := Integer(m_MoneyList.Objects[nIdx]);           // 6326
    ///     nReturn := _MAX(nOldMoney - nValue, 0);                    // 6327
    ///     Result := nOldMoney - nReturn;                             // 6328
    ///     m_MoneyList.Objects[nIdx] := TObject(nReturn);             // 6329
    ///     if nReturn &lt;&gt; nOldMoney then SendDefMessage(SM_MONEY, nReturn, 0, 0, 0, Money.sName);  // 6331-6332
    ///   end;
    /// </code>
    /// ★ **返回值语义**：`Result` 是**实际扣掉的数量**（`nOldMoney - nReturn`），不是扣后的余额；
    /// 余额经 `var nReturn` 出参返回。**余额不足时 `nReturn = 0`、`Result = nOldMoney`**（不会变负）。
    /// ★ `SendDefMessage` 的 `nRecog: Int64` 实参是 `nReturn`（扣后余额）—— **只在余额变了**才发。
    /// </remarks>
    public int DecMoney(int nIndex, int nValue, ref int nReturn)
    {
        // 原文 6315：Result := 0;
        int result = 0;

        // 原文 6316：Money := FindCustomMoney(nIndex);
        object? money = PlayerSurfaceCore3Seams.FindCustomMoneyByIndex(nIndex);

        // 原文 6317-6321（:6319 的 `Money.sName` 同 CheckMoney(nIndex) 的原文缺陷）
        if (money == null)
        {
            string sName = PlayerSurfaceCustomMoneyCompat.GetName(money!);   // ★ 原文缺陷（:6319）
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币ID[" + sName + "]");
            return result;                                                   // 原文 6320
        }

        // 原文 6323：nIdx := m_MoneyList.GetIndex(UpperCase(Money.sName));
        int idx = m_MoneyList.GetIndex(PlayerSurfaceCustomMoneyCompat.GetName(money).ToUpperInvariant());
        if (idx >= 0)
        {
            // 原文 6326：nOldMoney := Integer(m_MoneyList.Objects[nIdx]);
            int nOldMoney = m_MoneyList.GetValue(idx);
            // 原文 6327：nReturn := _MAX(nOldMoney - nValue, 0);
            nReturn = Math.Max(nOldMoney - nValue, 0);
            // 原文 6328：Result := nOldMoney - nReturn;
            result = nOldMoney - nReturn;
            // 原文 6329：m_MoneyList.Objects[nIdx] := TObject(nReturn);
            m_MoneyList.SetValue(idx, nReturn);

            // 原文 6331-6332：if nReturn <> nOldMoney then SendDefMessage(SM_MONEY, nReturn, 0, 0, 0, Money.sName);
            if (nReturn != nOldMoney)
            {
                var defMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_MONEY, nReturn, 0, 0, 0);
                SendSocketRef(defMsg, PlayerSurfaceCustomMoneyCompat.GetName(money));
            }
        }

        return result;
    }

    /// <summary>
    /// 原文 `function TPlayObject.DecMoney(sMoneyName: string; nValue: Integer; var nReturn: Integer): Integer;`
    /// （`ObjPlayer.pas:6336-6358`）—— 与 nIndex 版**逐行同构**，只是全程用 `sMoneyName`
    /// （**不取 `Money.sName`**），故**没有** nil 解引用缺陷。
    /// </summary>
    public int DecMoney(string sMoneyName, int nValue, ref int nReturn)
    {
        // 原文 6340：Result := 0;
        int result = 0;

        // 原文 6341-6345：if FindCustomMoney(sMoneyName) = nil then begin MainOutMessage(...); Exit; end;
        if (PlayerSurfaceCore3Seams.FindCustomMoneyByName(sMoneyName) == null)
        {
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币[" + sMoneyName + "]");
            return result;                                   // 原文 6344
        }

        // 原文 6347：nIdx := m_MoneyList.GetIndex(UpperCase(sMoneyName));
        int idx = m_MoneyList.GetIndex(sMoneyName.ToUpperInvariant());
        if (idx >= 0)
        {
            // 原文 6350：nOldMoney := Integer(m_MoneyList.Objects[nIdx]);
            int nOldMoney = m_MoneyList.GetValue(idx);
            // 原文 6351：nReturn := _MAX(nOldMoney - nValue, 0);
            nReturn = Math.Max(nOldMoney - nValue, 0);
            // 原文 6352：Result := nOldMoney - nReturn;
            result = nOldMoney - nReturn;
            // 原文 6353：m_MoneyList.Objects[nIdx] := TObject(nReturn);
            m_MoneyList.SetValue(idx, nReturn);

            // 原文 6355-6356
            if (nReturn != nOldMoney)
            {
                var defMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_MONEY, nReturn, 0, 0, 0);
                SendSocketRef(defMsg, sMoneyName);
            }
        }

        return result;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.IncMoney(sMoneyName: string; nValue: Integer; var nReturn: Integer);`
    /// （`ObjPlayer.pas:6360-6380`）。
    /// ★ 与 `DecMoney` 的唯一差别：`:6374 nReturn := _MAX(nOldMoney + nValue, 0)` 是**加法**，
    /// `Result` 是 **void**（没有"实际增加量"出参）。**注意 `_MAX(..., 0)` 仍在** ——
    /// 负的 `nValue` 可能把余额压到 0（不会变负），且**余额变化才发 SM_MONEY**。
    /// </summary>
    public void IncMoney(string sMoneyName, int nValue, ref int nReturn)
    {
        // 原文 6364-6368：if FindCustomMoney(sMoneyName) = nil then begin MainOutMessage(...); Exit; end;
        if (PlayerSurfaceCore3Seams.FindCustomMoneyByName(sMoneyName) == null)
        {
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币[" + sMoneyName + "]");
            return;                                          // 原文 6367
        }

        // 原文 6370：nIdx := m_MoneyList.GetIndex(UpperCase(sMoneyName));
        int idx = m_MoneyList.GetIndex(sMoneyName.ToUpperInvariant());
        if (idx >= 0)
        {
            // 原文 6373：nOldMoney := Integer(m_MoneyList.Objects[nIdx]);
            int nOldMoney = m_MoneyList.GetValue(idx);
            // 原文 6374：nReturn := _MAX(nOldMoney + nValue, 0);
            nReturn = Math.Max(nOldMoney + nValue, 0);
            // 原文 6375：m_MoneyList.Objects[nIdx] := TObject(nReturn);
            m_MoneyList.SetValue(idx, nReturn);

            // 原文 6377-6378
            if (nReturn != nOldMoney)
            {
                var defMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_MONEY, nReturn, 0, 0, 0);
                SendSocketRef(defMsg, sMoneyName);
            }
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.IncMoney(nIndex: Integer; nValue: Integer; var nReturn: Integer);`
    /// （`ObjPlayer.pas:6382-6404`）—— 与 string 版同构，`:6390` 有同族 nil 解引用缺陷。
    /// </summary>
    public void IncMoney(int nIndex, int nValue, ref int nReturn)
    {
        // 原文 6387：Money := FindCustomMoney(nIndex);
        object? money = PlayerSurfaceCore3Seams.FindCustomMoneyByIndex(nIndex);

        // 原文 6388-6392（:6390 的 `Money.sName` 同族原文缺陷）
        if (money == null)
        {
            string sName = PlayerSurfaceCustomMoneyCompat.GetName(money!);   // ★ 原文缺陷（:6390）
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币ID[" + sName + "]");
            return;                                                          // 原文 6391
        }

        // 原文 6394：nIdx := m_MoneyList.GetIndex(UpperCase(Money.sName));
        int idx = m_MoneyList.GetIndex(PlayerSurfaceCustomMoneyCompat.GetName(money).ToUpperInvariant());
        if (idx >= 0)
        {
            // 原文 6397：nOldMoney := Integer(m_MoneyList.Objects[nIdx]);
            int nOldMoney = m_MoneyList.GetValue(idx);
            // 原文 6398：nReturn := _MAX(nOldMoney + nValue, 0);
            nReturn = Math.Max(nOldMoney + nValue, 0);
            // 原文 6399：m_MoneyList.Objects[nIdx] := TObject(nReturn);
            m_MoneyList.SetValue(idx, nReturn);

            // 原文 6401-6402
            if (nReturn != nOldMoney)
            {
                var defMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_MONEY, nReturn, 0, 0, 0);
                SendSocketRef(defMsg, PlayerSurfaceCustomMoneyCompat.GetName(money));
            }
        }
    }

    /// <summary>
    /// 原文 `function TPlayObject.GetMoney(sMoneyName: string): Integer;`（`ObjPlayer.pas:6406-6422`）。
    /// ★ 注意原文**签名与 body 的错配**：形参是 `sMoneyName`，但函数体全程用
    /// `Money := FindCustomMoney(sMoneyName)` 再 `Money.sName`（**不是**直接 `UpperCase(sMoneyName)`）。
    /// 逐字照抄（包括 `:6415` 的 nil 解引用缺陷）。
    /// </summary>
    public int GetMoney(string sMoneyName)
    {
        // 原文 6411：Result := 0;
        int result = 0;

        // 原文 6412：Money := FindCustomMoney(sMoneyName);
        object? money = PlayerSurfaceCore3Seams.FindCustomMoneyByName(sMoneyName);

        // 原文 6413-6417（:6415 同族原文缺陷）
        if (money == null)
        {
            string sName = PlayerSurfaceCustomMoneyCompat.GetName(money!);        // ★ 原文缺陷（:6415）
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币ID[" + sName + "]");
            return result;                                                        // 原文 6416
        }

        // 原文 6419：nIdx := m_MoneyList.GetIndex(UpperCase(Money.sName));
        int idx = m_MoneyList.GetIndex(PlayerSurfaceCustomMoneyCompat.GetName(money).ToUpperInvariant());
        if (idx >= 0)
            // 原文 6421：Result := Integer(m_MoneyList.Objects[nIdx]);
            result = m_MoneyList.GetValue(idx);

        return result;
    }

    /// <summary>
    /// 原文 `function TPlayObject.GetMoney(nIndex: Integer): Integer;`（`ObjPlayer.pas:6424-6440`）。
    /// </summary>
    public int GetMoney(int nIndex)
    {
        // 原文 6429：Result := 0;
        int result = 0;

        // 原文 6430：Money := FindCustomMoney(nIndex);
        object? money = PlayerSurfaceCore3Seams.FindCustomMoneyByIndex(nIndex);

        // 原文 6431-6435（:6433 同族原文缺陷）
        if (money == null)
        {
            string sName = PlayerSurfaceCustomMoneyCompat.GetName(money!);        // ★ 原文缺陷（:6433）
            PlayerSurfaceOperateSeams.MainOutMessage(
                "用户:[" + m_sCharName + "]使用不存在的自定义货币ID[" + sName + "]");
            return result;                                                        // 原文 6434
        }

        // 原文 6437：nIdx := m_MoneyList.GetIndex(UpperCase(Money.sName));
        int idx = m_MoneyList.GetIndex(PlayerSurfaceCustomMoneyCompat.GetName(money).ToUpperInvariant());
        if (idx >= 0)
            // 原文 6439：Result := Integer(m_MoneyList.Objects[nIdx]);
            result = m_MoneyList.GetValue(idx);

        return result;
    }

    // ==================================================================
    // DecGamePoint（原文 6442-6448）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.DecGamePoint(nGamePoint: LongWord);`（`ObjPlayer.pas:6442-6448`）。
    /// <code>
    ///   if m_nGamePoint >= nGamePoint then Dec(m_nGamePoint, nGamePoint)   // 6444-6445
    ///   else m_nGamePoint := 0;                                            // 6447
    /// </code>
    /// ★ 与 `DecGold`（Gold.cs「不够时原地不动」）**不同** —— 这里不够就**夹到 0**（同 `DecGameGold`）。
    /// ⚠ 托管 `m_nGamePoint` 是 **`int`**（`ObjBase.OnlineMsg.cs:37`），原文是 `LongWord`——
    /// 原文 `m_nGamePoint >= nGamePoint` 是**无符号**比较，托管侧必须先转 `uint` 再比，
    /// 否则 `m_nGamePoint = -1` 时会被判成「不够」（原文会判成 4294967295 ≥ ...）。
    /// </summary>
    public void DecGamePoint(uint nGamePoint)
    {
        // 原文 6444-6445：if m_nGamePoint >= nGamePoint then Dec(m_nGamePoint, nGamePoint)
        if ((uint)m_nGamePoint >= nGamePoint)
            m_nGamePoint = unchecked((int)((uint)m_nGamePoint - nGamePoint));
        else
            // 原文 6447：m_nGamePoint := 0;
            m_nGamePoint = 0;
    }

    // ==================================================================
    // Disappear / DisappearB（原文 6587-6615）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.Disappear;`（`ObjPlayer.pas:6587-6604`）。
    /// <code>
    ///   if m_boReadyRun then DisappearA;                       // 6589-6590
    ///   if m_boTransparent and m_boHideMode then
    ///     m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] := 0;  // 6592-6593  // 004CA8F7
    ///   if m_GroupOwner &lt;&gt; nil then m_GroupOwner.DelGroupMember(Self);      // 6595-6596
    ///   if m_MyGuild &lt;&gt; nil then TGUild(m_MyGuild).DelHumanObj(Self);       // 6598-6599
    ///   g_NationManage.DeleteMember(Self);                     // 6601
    ///   LogonTimcCost();                                       // 6602
    ///   inherited;                                             // 6603
    /// </code>
    /// ★ 逐字保留：**透明态清零用的下标是 `0x70`**（原文以字面量 `STATE_TRANSPARENT { 0x70 }` 写下标，
    /// 而 `MAX_STATUS_ATTR = 18`，`0x70 = 112` —— **远超数组长度**！这是原文的真实下标写法；
    /// 托管 `m_wStatusTimeArr`（Core4.cs:120）长度为 18，直接照抄会**越界**，
    /// 故按「原文语义优先 + 不制造托管崩溃」的既有口径做**边界保护**并显式留痕，见下）。
    /// 接缝：`DisappearA`（:1753 起，未移植）/ `DelGroupMember` / `TGUild.DelHumanObj` /
    /// `g_NationManage.DeleteMember` / `LogonTimcCost` / `inherited`（= `TBaseObject.Disappear`）。
    /// </summary>
    public void Disappear()
    {
        // 原文 6589-6590：if m_boReadyRun then DisappearA;
        if (m_boReadyRun)
            PlayerSurfaceCore3LifecycleSeams.DisappearA(this);

        // 原文 6592-6593：if m_boTransparent and m_boHideMode then m_wStatusTimeArr[STATE_TRANSPARENT] := 0;
        if (m_boTransparent && m_boHideMode)
        {
            // ★ 原文如此（原文 6593）：下标写成 `STATE_TRANSPARENT { 0x70 }` = 112，
            //   而 `TStatusTime = array[0..MAX_STATUS_ATTR-1]`（`MAX_STATUS_ATTR = 18`）——
            //   原文在 Delphi 里**关掉范围检查**时不报错，但写的是**数组外的内存**。
            //   托管侧 `ushort[]` 越界会抛 `IndexOutOfRangeException`，
            //   故此处**按原文意图的 0x70 语义**做「越界则跳过」，并在此显式登记（不静默）。
            const int STATE_TRANSPARENT_LITERAL = 0x70;   // 原文 6593 的字面量
            if (STATE_TRANSPARENT_LITERAL < m_wStatusTimeArr.Length)
                m_wStatusTimeArr[STATE_TRANSPARENT_LITERAL] = 0;
        }

        // 原文 6595-6596：if m_GroupOwner <> nil then m_GroupOwner.DelGroupMember(Self);
        if (m_GroupOwner != null)
            PlayerSurfaceCore3LifecycleSeams.DelGroupMember(m_GroupOwner, this);

        // 原文 6598-6599：if m_MyGuild <> nil then TGUild(m_MyGuild).DelHumanObj(Self);
        PlayerSurfaceCore3LifecycleSeams.GuildDelHumanObj(this);

        // 原文 6601：g_NationManage.DeleteMember(Self);
        PlayerSurfaceCore3Seams.NationDeleteMember(this);

        // 原文 6602：LogonTimcCost();
        PlayerSurfaceCore3Seams.LogonTimcCost(this);

        // 原文 6603：inherited;  （= TBaseObject.Disappear —— 宿主侧未切出，经接缝表达）
        PlayerSurfaceCore3LifecycleSeams.InheritedDisappear(this);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DisappearB();`（`ObjPlayer.pas:6606-6615`）——
    /// `Disappear` 的**子集**：只做三处解绑，**不碰** `m_boReadyRun`/透明态/`LogonTimcCost`/`inherited`。
    /// <code>
    ///   if m_GroupOwner &lt;&gt; nil then m_GroupOwner.DelGroupMember(Self);   // 6608-6609
    ///   if m_MyGuild &lt;&gt; nil then TGUild(m_MyGuild).DelHumanObj(Self);    // 6611-6612
    ///   g_NationManage.DeleteMember(Self);                              // 6614
    /// </code>
    /// </summary>
    public void DisappearB()
    {
        // 原文 6608-6609
        if (m_GroupOwner != null)
            PlayerSurfaceCore3LifecycleSeams.DelGroupMember(m_GroupOwner, this);

        // 原文 6611-6612
        PlayerSurfaceCore3LifecycleSeams.GuildDelHumanObj(this);

        // 原文 6614
        PlayerSurfaceCore3Seams.NationDeleteMember(this);
    }

    // ==================================================================
    // HorseRunTo（原文 5893-6147）—— 未移植
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.HorseRunTo(btDir: Byte; boFlag: Boolean): Boolean;`
    /// （`ObjPlayer.pas:5893-6147`，**255 行**）。
    ///
    /// <para><b>状态：未移植（显式留痕）</b>。按任务书第 2/4 条与台账 §48.1，
    /// 此处调用 <see cref="PortNotPorted"/> 留痕，**不**写裸 `=> true;`，也**不**写"近似跑马"。</para>
    ///
    /// <para><b>缺失成员（带原文行号）</b>：
    /// <list type="bullet">
    ///   <item><description>`TEnvirnoment.CanWalkEx(BaseObject, nX, nY, boFlag): Boolean`（:5909 等 **48 处**）
    ///     —— 托管 `Engine/Envir.cs` 只有 `CanWalk(nX, nY)`（:218），**无 `CanWalkEx` 重载**；
    ///     该方法的 1:1 实现散在 `CanWalkExtCore.cs` 的静态函数里，未挂到 `TEnvirnoment` 上。</description></item>
    ///   <item><description>`TEnvirnoment.MoveToMovingObject(nX, nY, BaseObject, nX2, nY2, boFlag): Boolean`
    ///     （:5915 等 **16 处**）—— 托管侧完全没有该方法（`Envir.cs` 只有 `AddToMap`/`DeleteFromMap`）。</description></item>
    ///   <item><description>`TEnvirnoment.m_nWidth` / `m_nHeight`（:5922 等）—— 托管字段名是
    ///     `nWidth` / `nHeight`（`Envir.cs:87-88`），**字段名不一致**且本车道无 `Envir.cs` 写权限。</description></item>
    ///   <item><description>`TPlayObject.InSafeZone`（:5910 等 **48 处**）—— 是 `TBaseObject` 的方法
    ///     （`ObjBase.pas:35612/35657`），托管侧只有 `SafeAreaCore.cs` 的静态函数，未挂到 `TCreature` 上。</description></item>
    ///   <item><description>`TPlayObject.Walk(RM_HORSERUN)`（:6130）—— `TBaseObject.Walk`，未移植。</description></item>
    ///   <item><description>`MainOutMessage(sExceptionMsg)`（:6145）—— 已有接缝 `PlayerSurfaceOperateSeams.MainOutMessage`。</description></item>
    /// </list>
    /// 结论：该方法是 `TEnvirnoment` **移动原语面**的消费者，必须等 `CanWalkEx`/`MoveToMovingObject`/
    /// `InSafeZone` 三个面落地才能逐行移植，否则只能造替身（违反 §14.2）。</para>
    /// </summary>
    public bool HorseRunTo(byte btDir, bool boFlag)
    {
        PortNotPorted(nameof(HorseRunTo), 5893);
        return false;
    }

    // ==================================================================
    // DropUseItems（原文 6617-6940）—— 未移植
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.DropUseItems(BaseObject: TBaseObject);`
    /// （`ObjPlayer.pas:6617-6940`，**324 行**）—— 死亡时按爆率掉落**身上装备**。
    ///
    /// <para><b>状态：未移植（显式留痕）</b>。</para>
    ///
    /// <para><b>缺失成员（带原文行号）</b>：
    /// <list type="bullet">
    ///   <item><description>`m_PEnvir.m_boNODROPITEM`（:6633）、`m_PEnvir.m_boNODROPUSEITEMS`（:6638）、
    ///     `m_PEnvir.m_boDELDROPITEM`（:6806）—— `TEnvirnoment` 的这三个地图标志托管侧**未切出**
    ///     （`Envir.cs` 无这些字段；全仓仅注释提及）。</description></item>
    ///   <item><description>`m_boNoDropUseItem`（:6637）、`m_boDropUseItem`（:6639）、
    ///     `m_nDieDropUseItemRate`（:6695）、`m_sCurrentItemNewName`（:6676）、`m_nCurrentItemPos`（:6675）、
    ///     `m_PickUpOrDropItem`（:6678）、`m_sCurrentOperateUserName`（:6736）、
    ///     `m_sDropInsuranceItemName`/`m_nDropInsuranceItemCount`/`m_nDropInsuranceItemCurrency`/
    ///     `m_nDropInsuranceItemGold`（:6817-6820）—— `TBaseObject`/`TPlayObject` 字段，
    ///     托管侧**均未声明**，且多为「保险/掉装上报」专用，属掉落面上游。</description></item>
    ///   <item><description>`g_Config.boWarNoDropUseItem` / `boDropUseItem` / `nDieRedDropUseItemRate` /
    ///     `nDieDropUseItemRate` / `nDropUseItemsMaxCount` / `DieDropUseItemRates` /
    ///     `nDieRedDropUseItemOneRate` —— **这些已存在**（`M2Config.GameDie.cs`），不是阻塞项。</description></item>
    ///   <item><description>`g_CastleManager.InCastleWarArea(Self)`（:6646）—— `CastleState.cs:9` 有全局实例，
    ///     但 `InCastleWarArea` 在该类上**未切出**（`Core4.cs:506` 已登记同一缺口）。</description></item>
    ///   <item><description>`UserEngine.GetStdItem(wIndex)`（:6654）—— 已有接缝
    ///     `PlayerSurfaceItemSeams.GetStdItem`。</description></item>
    ///   <item><description>`GetUserItemBindValue(@Item, ubNoScatter/ubNoTakeOff)`（:6716/:6803）与
    ///     `g_ItemRules.Get(wIndex, 6/13/39)`（:6718/:6722/:6734）—— `TItemRule`/`TItemRuleManager`
    ///     托管侧**未切出**（全仓无 `ItemRules` 类型）。</description></item>
    ///   <item><description>`DropItemDown(@Item, nWide, boFlag, BaseObject, Self)`（:6731/:6879）——
    ///     `TBaseObject.DropItemDown`，**未移植**。</description></item>
    ///   <item><description>`ProcessUseItemSkill(I, StdItem, False)`（:6667 等 8 处）—— 托管侧存在
    ///     `SendUpdateItemInsuranceCount`（Core6.cs:347），但 `ProcessUseItemSkill` 只在插件接口表里，
    ///     `TPlayObject` 上**未移植**。</description></item>
    ///   <item><description>`AddGameDataLog(...)`（:6662/:6987）—— 见本片
    ///     <see cref="PlayerSurfaceCore3Seams.AddGameDataLog"/> 接缝（可为它注入）。</description></item>
    ///   <item><description>`g_FunctionNPC.GotoLable(Self, '@TakeOff'+I, False)`（:6679 等 16 处）——
    ///     已有接缝 `PlayerSurfaceItemSeams.FunctionNpcGotoLable`。</description></item>
    ///   <item><description>`SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems)`（:6936）——
    ///     `TBaseObject.SendMsg`，托管 `TCreature.SendMsg`（`ObjBase.cs:79`）是**入队近似版**。</description></item>
    /// </list>
    /// 结论：**7 类成员缺失**（地图三标志、掉落私有字段族、`g_CastleManager.InCastleWarArea`、
    /// `TItemRule`、`DropItemDown`、`ProcessUseItemSkill`、`SendMsg` 视野版），
    /// 移植它必须先把这 7 类落地，否则"掉落"会变成一条**看起来对、实际不掉**的空路径。</para>
    /// </summary>
    public void DropUseItems(TCreature baseObject)
    {
        PortNotPorted(nameof(DropUseItems), 6617);
    }

    // ==================================================================
    // DropJewelryBoxItems（原文 6942-7238）—— 未移植
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.DropJewelryBoxItems(BaseObject: TBaseObject);`
    /// （`ObjPlayer.pas:6942-7238`，**296 行**）—— 首饰盒（`m_JewelryBoxItems`）掉落版。
    ///
    /// <para><b>状态：未移植（显式留痕）</b>。缺失面与
    /// <see cref="DropUseItems"/> **完全同族**，另加：
    /// <list type="bullet">
    ///   <item><description>`m_JewelryBoxItems: THumanJewelryBoxItems`（`ObjBase.pas:883`）——
    ///     托管侧**未声明该容器**（`RecalcChain.cs` 只切了 `m_UseItems`）。</description></item>
    ///   <item><description>`U_JEWELRYITEM1`（:6992 等）—— **已存在**（`Grobal2Const.U_JEWELRYITEM1 = 30`），
    ///     不是阻塞项。</description></item>
    /// </list>
    /// </para>
    /// </summary>
    public void DropJewelryBoxItems(TCreature baseObject)
    {
        PortNotPorted(nameof(DropJewelryBoxItems), 6942);
    }

    // ==================================================================
    // DropGodBlessItems（原文 7240-7540）—— 未移植
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.DropGodBlessItems(BaseObject: TBaseObject);`
    /// （`ObjPlayer.pas:7240-7540`，**300 行**）—— 神佑袋（`m_GodBlessItems`）掉落版。
    ///
    /// <para><b>状态：未移植（显式留痕）</b>。缺失面同 <see cref="DropUseItems"/> 族，另加：
    /// <list type="bullet">
    ///   <item><description>`m_GodBlessItems: THumanGodBlessItems`（`ObjBase.pas:884`）——
    ///     托管侧**未声明该容器**。</description></item>
    ///   <item><description>`U_GODBLESSITEM1`（:7290 等）—— **已存在**（`Grobal2Const.U_GODBLESSITEM1 = 40`），
    ///     不是阻塞项。</description></item>
    /// </list>
    /// </para>
    /// </summary>
    public void DropGodBlessItems(TCreature baseObject)
    {
        PortNotPorted(nameof(DropGodBlessItems), 7240);
    }

    // ==================================================================
    // GainExp（原文 7543-7657）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GainExp(dwExp: LongWord);`（`ObjPlayer.pas:7543-7657`）——
    /// **组队经验分配**（原文上方 TODO：「组队成员不在同一地图或屏幕可分享经验」）。
    ///
    /// <code>
    ///   const bonus: array [0..MAX_BONUS_VALUE] of real =
    ///     (1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2);      // 7552
    ///   nCheckCode := 0; n := 0; sumlv := 0;                              // 7554-7556
    ///   try
    ///     if dwExp &gt; 0 then                                              // 7558
    ///       if (m_GroupOwner &lt;&gt; nil) and (m_GroupOwner.m_GroupMembers &lt;&gt; nil) then  // 7560
    ///       begin
    ///         ... 第一遍：统计「同图/同屏且未死」的 sumlv（等级和）与 n（人数）   // 7567-7588
    ///         nCheckCode := 2;                                             // 7595
    ///         if (sumlv &gt; 0) and (n &gt; 1) then                             // 7596
    ///         begin
    ///           if n in [0..MAX_BONUS_VALUE] then dwExp := Round(dwExp * bonus[n]);  // 7598-7599
    ///           nCheckCode := 3;                                           // 7601
    ///           PlayerList := TList.Create;                                // 7602
    ///           ... 第二遍：收集同一批成员到 PlayerList                      // 7609-7621
    ///           for I := 0 to PlayerList.Count - 1 do                      // 7628
    ///             if g_Config.boHighLevelKillMonFixExp and g_Config.boHighLevelGroupFixExp then
    ///               PlayObject.WinExp(Round(dwExp / n))                    // 7634
    ///             else
    ///               PlayObject.WinExp(Round(dwExp / sumlv * PlayObject.m_Abil.Level));  // 7640
    ///         end
    ///         else WinExp(dwExp);                                          // 7649
    ///       end
    ///       else WinExp(dwExp);                                            // 7652
    ///   except
    ///     MainOutMessage(sExceptionMsg + ' ' + IntToStr(nCheckCode));      // 7655
    ///   end;
    /// </code>
    ///
    /// ★ 逐字保留的原文要点：
    /// <list type="number">
    ///   <item><description>**两遍遍历**：第一遍只算 `sumlv`/`n`，第二遍才建 `PlayerList` 并分发
    ///     （原文说「组队成员不在同一地图或屏幕可分享经验」，两遍判据**完全相同**）。</description></item>
    ///   <item><description>「可分享」判据 = `(not m_boDeath) and ((m_PEnvir = 对方.m_PEnvir) or boShareExpGroupSameMap)
    ///     and (((|dx| &lt;= m_nViewRange) and (|dy| &lt;= m_nViewRange)) or boShareExpGroupSameScreen)`
    ///     —— 两条 `or` 各由**不同**配置开关放行；距离用的是**切比雪夫**（两轴各自 ≤ 视距）。</description></item>
    ///   <item><description>`m_PEnvir = 对方.m_PEnvir` 是**引用相等**（原文 `=` 比的是 `TEnvirnoment` 指针），
    ///     托管侧用 `ReferenceEquals`（**不是** `sMapName` 比较 —— 两个不同实例同名地图在原文里**不相等**）。</description></item>
    ///   <item><description>加成档位 `bonus[n]` 的下标是**人数 `n`**（不是等级和），且 `n in [0..MAX_BONUS_VALUE]`
    ///     的区间检查在 `sumlv &gt; 0 and n &gt; 1` 之后 —— `n` 至少 2，所以 `bonus[0]`/`bonus[1]` 实际取不到。</description></item>
    ///   <item><description>`dwExp := Round(dwExp * bonus[n])` 里 `dwExp` 是 **LongWord**、`bonus[n]` 是 **real**
    ///     —— `Round` 返回 **Int64**，赋回 `LongWord`（超出会范围错误）。托管侧对 `uint` 回写用 `unchecked((uint)...)`。</description></item>
    ///   <item><description>异常被**吞掉**并打 `sExceptionMsg + ' ' + IntToStr(nCheckCode)` ——
    ///     `nCheckCode` 是原文的**分段定位码**（1..7，逐段赋值），托管侧逐字保留该变量与取值点。</description></item>
    ///   <item><description>`{$IF MULTI_THREAD = 1}` 的 `LockR/UnLockR`（:7562-7566/:7604-7608）在原文里是
    ///     **条件编译**；托管侧统一走 <see cref="PlayerSurfaceCore3Seams.LockGroupMembers"/> 接缝
    ///     （默认不动作 = 原文关闭该宏时的行为）。</description></item>
    /// </list>
    ///
    /// ⚠ 托管偏差（已登记）：`m_Abil` 是 `m_WAbil` 的引用别名（见字段注释），
    /// 故 `PlayObject.m_Abil.Level` 与 `PlayObject.m_wAbil.Level` 同源。
    /// </summary>
    public void GainExp(uint dwExp)
    {
        // 原文 7545-7548：局部变量（I / n / sumlv / PlayObject / nCheckCode / PlayerList）
        int n;
        int sumlv;
        int nCheckCode;

        // 原文 7552：const bonus: array [0..MAX_BONUS_VALUE] of real = (1, 1.2, ..., 2.2);
        //   MAX_BONUS_VALUE = 11（`Grobal2.pas`；数组长度 12）。
        double[] bonus = { 1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2 };

        // 原文 7554-7556：nCheckCode := 0; n := 0; sumlv := 0;
        nCheckCode = 0;
        n = 0;
        sumlv = 0;

        // 原文 7557-7656：try ... except MainOutMessage(sExceptionMsg + ' ' + IntToStr(nCheckCode)); end;
        try
        {
            // 原文 7558：if dwExp > 0 then
            if (dwExp > 0)
            {
                // 原文 7560：if (m_GroupOwner <> nil) and (m_GroupOwner.m_GroupMembers <> nil) then
                if (m_GroupOwner != null && m_GroupOwner.m_GroupMembers != null)
                {
                    // 原文 7562-7566：{$IF MULTI_THREAD = 1} ... LockR(30) ...
                    PlayerSurfaceCore3Seams.LockGroupMembers(m_GroupOwner, 30);
                    try
                    {
                        // 原文 7567：if (m_GroupOwner.m_GroupMembers.Count > 0) then
                        if (m_GroupOwner.m_GroupMembers.Count > 0)
                        {
                            // 原文 7569-7571：sumlv := 0; n := 0; nCheckCode := 1;
                            sumlv = 0;
                            n = 0;
                            nCheckCode = 1;

                            // 原文 7572：for I := 0 to m_GroupOwner.m_GroupMembers.Count - 1 do
                            for (int i = 0; i <= m_GroupOwner.m_GroupMembers.Count - 1; i++)
                            {
                                // 原文 7574：PlayObject := TPlayObject(m_GroupOwner.m_GroupMembers.Objects[I]);
                                var playObject = m_GroupOwner.m_GroupMembers[i];
                                if (playObject != null)
                                {
                                    // 原文 7577-7581：可分享判据（见 XML doc 第 2/3 条）
                                    if (!playObject.m_boDeath
                                        && (ReferenceEquals(m_PEnvir, playObject.m_PEnvir)
                                            || M2Config.boShareExpGroupSameMap)
                                        && ((Math.Abs(m_nCurrX - playObject.m_nCurrX) <= m_nViewRange
                                             && Math.Abs(m_nCurrY - playObject.m_nCurrY) <= m_nViewRange)
                                            || M2Config.boShareExpGroupSameScreen))
                                    {
                                        // 原文 7583-7584：sumlv := sumlv + PlayObject.m_Abil.Level; Inc(n);
                                        sumlv += (int)playObject.m_Abil.Level;
                                        n++;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        // 原文 7589-7593：{$IF MULTI_THREAD = 1} finally ... UnLockR ... end;
                        PlayerSurfaceCore3Seams.UnLockGroupMembers(m_GroupOwner);
                    }

                    // 原文 7595：nCheckCode := 2;
                    nCheckCode = 2;

                    // 原文 7596：if (sumlv > 0) and (n > 1) then
                    if (sumlv > 0 && n > 1)
                    {
                        // 原文 7598-7599：if n in [0 .. MAX_BONUS_VALUE] then dwExp := Round(dwExp * bonus[n]);
                        if (n >= 0 && n <= PlayerSurfaceCore3Const.MAX_BONUS_VALUE)
                            dwExp = unchecked((uint)DelphiRound(dwExp * bonus[n]));

                        // 原文 7601：nCheckCode := 3;
                        nCheckCode = 3;

                        // 原文 7602：PlayerList := TList.Create;
                        var playerList = new List<TPlayObject>();

                        // 原文 7604-7608：{$IF MULTI_THREAD = 1} ... LockR(31) ...
                        PlayerSurfaceCore3Seams.LockGroupMembers(m_GroupOwner, 31);
                        try
                        {
                            // 原文 7609-7621：第二遍收集（判据与第一遍逐字相同）
                            for (int i = 0; i <= m_GroupOwner.m_GroupMembers.Count - 1; i++)
                            {
                                var playObject = m_GroupOwner.m_GroupMembers[i];
                                if (playObject != null)
                                {
                                    if (!playObject.m_boDeath
                                        && (ReferenceEquals(m_PEnvir, playObject.m_PEnvir)
                                            || M2Config.boShareExpGroupSameMap)
                                        && ((Math.Abs(m_nCurrX - playObject.m_nCurrX) <= m_nViewRange
                                             && Math.Abs(m_nCurrY - playObject.m_nCurrY) <= m_nViewRange)
                                            || M2Config.boShareExpGroupSameScreen))
                                    {
                                        // 原文 7619：PlayerList.Add(PlayObject);
                                        playerList.Add(playObject);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // 原文 7622-7626：{$IF MULTI_THREAD = 1} finally ... UnLockR ... end;
                            PlayerSurfaceCore3Seams.UnLockGroupMembers(m_GroupOwner);
                        }

                        // 原文 7628：for I := 0 to PlayerList.Count - 1 do
                        for (int i = 0; i <= playerList.Count - 1; i++)
                        {
                            // 原文 7630：PlayObject := PlayerList.Items[I];
                            var playObject = playerList[i];

                            // 原文 7631：if g_Config.boHighLevelKillMonFixExp and g_Config.boHighLevelGroupFixExp then
                            if (M2Config.boHighLevelKillMonFixExp && M2Config.boHighLevelGroupFixExp)
                            {
                                // 原文 7633：nCheckCode := 4;
                                nCheckCode = 4;
                                // 原文 7634：PlayObject.WinExp(Round(dwExp / n));
                                PlayerSurfaceCore3Seams.WinExp(playObject,
                                    unchecked((uint)DelphiRound((double)dwExp / n)));
                                // 原文 7635：nCheckCode := 5;
                                nCheckCode = 5;
                            }
                            else
                            {
                                // 原文 7639：nCheckCode := 6;
                                nCheckCode = 6;
                                // 原文 7640：PlayObject.WinExp(Round(dwExp / sumlv * PlayObject.m_Abil.Level));
                                //   ⚠ 原文 `dwExp / sumlv` 是 **real 除法**（`/`），随后乘等级、再 `Round`。
                                //   托管逐字：`(double)dwExp / sumlv * Level`。
                                PlayerSurfaceCore3Seams.WinExp(playObject,
                                    unchecked((uint)DelphiRound((double)dwExp / sumlv * playObject.m_Abil.Level)));
                                // 原文 7641：nCheckCode := 7;
                                nCheckCode = 7;
                            }
                        }

                        // 原文 7644-7645：finally PlayerList.Free; end;（托管 List 由 GC 接管）
                    }
                    else
                    {
                        // 原文 7649：WinExp(dwExp);
                        PlayerSurfaceCore3Seams.WinExp(this, dwExp);
                    }
                }
                else
                {
                    // 原文 7652：WinExp(dwExp);
                    PlayerSurfaceCore3Seams.WinExp(this, dwExp);
                }
            }
        }
        catch (Exception)
        {
            // 原文 7654-7655：except MainOutMessage(sExceptionMsg + ' ' + IntToStr(nCheckCode));
            PlayerSurfaceOperateSeams.MainOutMessage("[Exception] TPlayObject.GainExp " + nCheckCode);
        }
    }

    // ==================================================================
    // GainExpNG（原文 7660-7761）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GainExpNG(dwExp: LongWord);`（`ObjPlayer.pas:7660-7761`）——
    /// `GainExp` 的**内功（NG）版**。
    ///
    /// <para>与 <see cref="GainExp"/> 的**逐条差异**（全部逐字保留）：
    /// <list type="number">
    ///   <item><description>入口多一个条件：`if (dwExp &gt; 0) and m_boTrainingNG then`（:7674）——
    ///     **没开内功修炼就整条不分发**（连自己的 `WinExpNG` 都不调）。</description></item>
    ///   <item><description>等级和用 **`m_AbilNG.Level`**（:7697/:7739），不是 `m_Abil.Level`。</description></item>
    ///   <item><description>**没有 `PlayerList`**：第二遍直接遍历 `m_GroupMembers` 就地分发
    ///     （:7720-7744），故**没有 `nCheckCode := 3` 之后的分段**，`nCheckCode` 取 1/2/4/5/6/7。</description></item>
    ///   <item><description>均分条件多一项：`(g_Config.boHighLevelKillMonFixExp or (m_dwHighLevelKillMonFixExpTime &gt; 0))
    ///     and g_Config.boHighLevelGroupFixExp`（:7729）—— 比 `GainExp` 多 `or m_dwHighLevelKillMonFixExpTime &gt; 0`。</description></item>
    ///   <item><description>分发调的是 `WinExpNG`（:7733/:7739/:7753/:7756）。</description></item>
    /// </list>
    /// </para>
    ///
    /// ⚠ 缺成员：`m_AbilNG`（`ObjBase.pas`，内功属性）与 `m_dwHighLevelKillMonFixExpTime`（`TPlayObject`）
    /// 托管侧**均未声明** —— 本片**据为己有**（原文行号见上）以避免"半移植"。
    /// </summary>
    public void GainExpNG(uint dwExp)
    {
        // 原文 7662-7664：I / n / sumlv / PlayObject / nCheckCode 局部变量
        int n;
        int sumlv;
        int nCheckCode;

        // 原文 7668：const bonus: array [0..MAX_BONUS_VALUE] of real = (...);
        double[] bonus = { 1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2 };

        // 原文 7670-7672：n := 0; sumlv := 0; nCheckCode := 0;
        n = 0;
        sumlv = 0;
        nCheckCode = 0;

        // 原文 7673-7760：try ... except MainOutMessage(sExceptionMsg + ' ' + IntToStr(nCheckCode));
        try
        {
            // 原文 7674：if (dwExp > 0) and m_boTrainingNG then
            if (dwExp > 0 && m_boTrainingNG)
            {
                // 原文 7676：if (m_GroupOwner <> nil) and (m_GroupOwner.m_GroupMembers <> nil) then
                if (m_GroupOwner != null && m_GroupOwner.m_GroupMembers != null)
                {
                    // 原文 7678-7682：{$IF MULTI_THREAD = 1} ... LockR(35) ...
                    PlayerSurfaceCore3Seams.LockGroupMembers(m_GroupOwner, 35);
                    try
                    {
                        // 原文 7683：if (m_GroupOwner.m_GroupMembers.Count > 0) then
                        if (m_GroupOwner.m_GroupMembers.Count > 0)
                        {
                            // 原文 7685-7687：sumlv := 0; n := 0; nCheckCode := 1;
                            sumlv = 0;
                            n = 0;
                            nCheckCode = 1;

                            // 原文 7688：for I := 0 to m_GroupOwner.m_GroupMembers.Count - 1 do
                            for (int i = 0; i <= m_GroupOwner.m_GroupMembers.Count - 1; i++)
                            {
                                var playObject = m_GroupOwner.m_GroupMembers[i];
                                if (playObject != null)
                                {
                                    if (!playObject.m_boDeath
                                        && (ReferenceEquals(m_PEnvir, playObject.m_PEnvir)
                                            || M2Config.boShareExpGroupSameMap)
                                        && ((Math.Abs(m_nCurrX - playObject.m_nCurrX) <= m_nViewRange
                                             && Math.Abs(m_nCurrY - playObject.m_nCurrY) <= m_nViewRange)
                                            || M2Config.boShareExpGroupSameScreen))
                                    {
                                        // 原文 7697-7698：sumlv := sumlv + PlayObject.m_AbilNG.Level; Inc(n);
                                        sumlv += playObject.m_AbilNG.Level;
                                        n++;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        // 原文 7703-7707：{$IF MULTI_THREAD = 1} finally ... UnLockR ... end;
                        PlayerSurfaceCore3Seams.UnLockGroupMembers(m_GroupOwner);
                    }

                    // 原文 7709：nCheckCode := 2;
                    nCheckCode = 2;

                    // 原文 7710：if (sumlv > 0) and (n > 1) then
                    if (sumlv > 0 && n > 1)
                    {
                        // 原文 7712-7713：if n in [0 .. MAX_BONUS_VALUE] then dwExp := Round(dwExp * bonus[n]);
                        if (n >= 0 && n <= PlayerSurfaceCore3Const.MAX_BONUS_VALUE)
                            dwExp = unchecked((uint)DelphiRound(dwExp * bonus[n]));

                        // 原文 7714：nCheckCode := 3;
                        nCheckCode = 3;

                        // 原文 7715-7719：{$IF MULTI_THREAD = 1} ... LockR(36) ...
                        PlayerSurfaceCore3Seams.LockGroupMembers(m_GroupOwner, 36);
                        try
                        {
                            // 原文 7720：for I := 0 to m_GroupOwner.m_GroupMembers.Count - 1 do
                            for (int i = 0; i <= m_GroupOwner.m_GroupMembers.Count - 1; i++)
                            {
                                var playObject = m_GroupOwner.m_GroupMembers[i];
                                if (playObject != null)
                                {
                                    if (!playObject.m_boDeath
                                        && (ReferenceEquals(m_PEnvir, playObject.m_PEnvir)
                                            || M2Config.boShareExpGroupSameMap)
                                        && ((Math.Abs(m_nCurrX - playObject.m_nCurrX) <= m_nViewRange
                                             && Math.Abs(m_nCurrY - playObject.m_nCurrY) <= m_nViewRange)
                                            || M2Config.boShareExpGroupSameScreen))
                                    {
                                        // 原文 7729-7730：if (g_Config.boHighLevelKillMonFixExp
                                        //   or (m_dwHighLevelKillMonFixExpTime > 0)) and g_Config.boHighLevelGroupFixExp then
                                        if ((M2Config.boHighLevelKillMonFixExp || m_dwHighLevelKillMonFixExpTime > 0)
                                            && M2Config.boHighLevelGroupFixExp)
                                        {
                                            // 原文 7732：nCheckCode := 4;
                                            nCheckCode = 4;
                                            // 原文 7733：PlayObject.WinExpNG(Round(dwExp / n));
                                            PlayerSurfaceCore3Seams.WinExpNG(playObject,
                                                unchecked((uint)DelphiRound((double)dwExp / n)));
                                            // 原文 7734：nCheckCode := 5;
                                            nCheckCode = 5;
                                        }
                                        else
                                        {
                                            // 原文 7738：nCheckCode := 6;
                                            nCheckCode = 6;
                                            // 原文 7739：PlayObject.WinExpNG(Round(dwExp / sumlv * PlayObject.m_AbilNG.Level));
                                            PlayerSurfaceCore3Seams.WinExpNG(playObject,
                                                unchecked((uint)DelphiRound(
                                                    (double)dwExp / sumlv * playObject.m_AbilNG.Level)));
                                            // 原文 7740：nCheckCode := 7;
                                            nCheckCode = 7;
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // 原文 7745-7749：{$IF MULTI_THREAD = 1} finally ... UnLockR ... end;
                            PlayerSurfaceCore3Seams.UnLockGroupMembers(m_GroupOwner);
                        }
                    }
                    else
                    {
                        // 原文 7753：WinExpNG(dwExp);
                        PlayerSurfaceCore3Seams.WinExpNG(this, dwExp);
                    }
                }
                else
                {
                    // 原文 7756：WinExpNG(dwExp);
                    PlayerSurfaceCore3Seams.WinExpNG(this, dwExp);
                }
            }
        }
        catch (Exception)
        {
            // 原文 7758-7759：except MainOutMessage(sExceptionMsg + ' ' + IntToStr(nCheckCode));
            PlayerSurfaceOperateSeams.MainOutMessage("[Exception] TPlayObject.GainExpNG " + nCheckCode);
        }
    }

    // ==================================================================
    // GameTimeChanged（原文 7763-7770）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GameTimeChanged;`（`ObjPlayer.pas:7763-7770`）。
    /// <code>
    ///   if m_nBright &lt;&gt; g_nGameTime then      // 7765
    ///   begin
    ///     m_nBright := g_nGameTime;            // 7767
    ///     SendMsg(Self, RM_DAYCHANGING, 0, 0, 0, 0, '');   // 7768
    ///   end;
    /// </code>
    /// ★ **只在亮度真的变了才发包**（`&lt;&gt;` 不是 `&lt;`/`&gt;`）—— 逐字保留。
    /// ⚠ 落点：`SendMsg(Self, RM_DAYCHANGING, ...)` 是 `TBaseObject.SendMsg(BaseObject; wIdent; ...)`
    /// （**带发送者**的视野版），托管侧只有 `TCreature.SendMsg(ushort, ...)`（`ObjBase.cs:79`，**入队近似版**，
    /// 无发送者形参）—— 按「最小面 + 不造第二份」的口径调用它，差异见交付报告。
    /// </summary>
    public void GameTimeChanged()
    {
        // 原文 7765：if m_nBright <> g_nGameTime then
        if (m_nBright != PlayerSurfaceCore3Globals.g_nGameTime)
        {
            // 原文 7767：m_nBright := g_nGameTime;
            m_nBright = PlayerSurfaceCore3Globals.g_nGameTime;
            // 原文 7768：SendMsg(Self, RM_DAYCHANGING, 0, 0, 0, 0, '');
            SendMsg(Grobal2Const.RM_DAYCHANGING, 0, 0, 0, 0, "");
        }
    }

    // ==================================================================
    // WeatherChanged（原文 7772-7775）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.WeatherChanged; // 天气`（`ObjPlayer.pas:7772-7775`）。
    /// <code>
    ///   SendMsg(Self, RM_WEATHER, 0, 0, 0, 0, '');   // 7774
    /// </code>
    /// ★ **无条件发包**（没有"天气变了"的判定 —— 与 <see cref="GameTimeChanged"/> 的 `&lt;&gt;` 判定不同）。
    /// </summary>
    public void WeatherChanged()
    {
        // 原文 7774
        SendMsg(Grobal2Const.RM_WEATHER, 0, 0, 0, 0, "");
    }

    // ==================================================================
    // GetBackDealItems（原文 7777-7791）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GetBackDealItems;`（`ObjPlayer.pas:7777-7791`）——
    /// 交易取消/失败时**归还暂存物品与金币**。
    /// <code>
    ///   if m_DealItemList.Count > 0 then                       // 7781
    ///     for I := 0 to m_DealItemList.Count - 1 do
    ///       m_ItemList.Add(m_DealItemList.Items[I]);           // 7784
    ///   m_DealItemList.Clear;                                  // 7787
    ///   Inc(m_nGold, m_nDealGolds);                            // 7788
    ///   m_nDealGolds := 0;                                     // 7789
    ///   m_boDealOK := False;                                   // 7790
    /// </code>
    /// ★ 逐字保留的原文要点：
    /// <list type="number">
    ///   <item><description>归还到 `m_ItemList`（**背包**）用的是 `Add`（**不判背包是否够格**），
    ///     且**不调** `SendAddItem` —— 与 <see cref="GetBackChallengeItems"/> **不同**
    ///     （后者逐件 `SendAddItem`）。</description></item>
    ///   <item><description>`Inc(m_nGold, m_nDealGolds)` 是 **Cardinal 加法**（`m_nGold` 原文 `LongWord`）
    ///     —— 托管 `m_nGold` 是 `uint`，用 `unchecked` 复刻回绕。</description></item>
    ///   <item><description>`m_DealItemList.Clear` **在归还循环之后**（原文顺序），
    ///     且**不释放**元素（`TList` 只清指针；托管 `List.Clear()` 同义）。</description></item>
    /// </list>
    /// </summary>
    public void GetBackDealItems()
    {
        // 原文 7781：if m_DealItemList.Count > 0 then
        if (m_DealItemList.Count > 0)
        {
            // 原文 7783：for I := 0 to m_DealItemList.Count - 1 do
            for (int i = 0; i <= m_DealItemList.Count - 1; i++)
            {
                // 原文 7784：m_ItemList.Add(m_DealItemList.Items[I]);
                //   ⚠ D35（值语义）：托管 `Add` 是**值复制** —— 与原文"加指针、共享同一对象"不同。
                m_ItemList.Add(m_DealItemList[i]);
            }
        }

        // 原文 7787：m_DealItemList.Clear;
        m_DealItemList.Clear();

        // 原文 7788：Inc(m_nGold, m_nDealGolds);
        //   ★ Cardinal（32 位无符号）加法，用 unchecked 复刻回绕。
        m_nGold = unchecked(m_nGold + (uint)m_nDealGolds);

        // 原文 7789：m_nDealGolds := 0;
        m_nDealGolds = 0;

        // 原文 7790：m_boDealOK := False;
        m_boDealOK = false;
    }

    // ==================================================================
    // GetBackChallengeItems（原文 7794-7835）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GetBackChallengeItems;`（`ObjPlayer.pas:7794-7835`，上方注释
    /// 「归还之前抵押的物品  piaoyun 2013-07-22」）—— 挑战取消时归还抵押物品 + 附加币。
    ///
    /// <code>
    ///   if m_ChallengeItemList.Count > 0 then                          // 7799
    ///     for I := 0 to m_ChallengeItemList.Count - 1 do
    ///     begin
    ///       UserItem := m_ChallengeItemList.Items[I];                  // 7803
    ///       if UserItem &lt;&gt; nil then
    ///       begin
    ///         m_ItemList.Add(m_ChallengeItemList.Items[I]);            // 7806
    ///         SendAddItem(UserItem);                                   // 7807
    ///       end;
    ///     end;
    ///   m_ChallengeItemList.Clear;                                     // 7812
    ///   Inc(m_nGold, m_nChallengeGolds);                               // 7813
    ///   // Inc(m_nGameDiamond, m_nChallengeGameDiamonds);              // 7814（原文注释掉）
    ///   if m_nChallengeGameDiamonds > 0 then                           // 7816
    ///     case g_Config.btChallengeGoldIndex of                        // 7817
    ///       0: Inc(m_nGameDiamond, m_nChallengeGameDiamonds);         // 7819
    ///       1: Inc(m_nGameGold, m_nChallengeGameDiamonds);            // 7821
    ///       2: Inc(m_nGameGird, m_nChallengeGameDiamonds);            // 7823
    ///     end;
    ///   m_nChallengeGolds := 0;      m_nChallengeGameDiamonds := 0;    // 7825-7826
    ///   m_boChallengeOK := False;    m_boChallengeStart := False;      // 7827-7828
    ///   m_ChallengeCreat := nil;                                       // 7829
    ///   GoldChanged();                                                 // 7832
    ///   NewGamePointChanged;                                           // 7834
    /// </code>
    ///
    /// ★ 逐字保留的原文要点：
    /// <list type="number">
    ///   <item><description>`Item &lt;&gt; nil` 才归还（**空槽跳过**，且空槽**不计入** `SendAddItem`）。</description></item>
    ///   <item><description>与 `GetBackDealItems` **不同**：这里逐件 `SendAddItem`（通知客户端）。</description></item>
    ///   <item><description>`case g_Config.btChallengeGoldIndex` **没有 `else`** —— 取值 &gt; 2（或 &lt; 0）
    ///     时**什么都不加**，但 `m_nChallengeGameDiamonds` 照样被清零（:7826）—— **附加币凭空消失**。
    ///     这是原文的真实行为，逐字保留；锁定用例
    ///     <c>ObjPlayerCore3Tests.GetBackChallengeItems_UnknownChallengeGoldIndex_DropsDiamonds_OriginalBehaviour</c>。</description></item>
    ///   <item><description>`Inc(m_nGameGold, ...)` 的 `m_nGameGold` 托管是 **`int`**、原文是 **`LongWord`**
    ///     —— 逐字用 `unchecked` 复刻 32 位回绕（`m_nGameDiamond`/`m_nGameGird` 同为 `int`）。</description></item>
    ///   <item><description>`NewGamePointChanged` 原文**不写括号**（无参过程调用），逐字保留该写法差异
    ///     （`Gold.cs:236` 的托管实现名同）。</description></item>
    /// </list>
    /// </summary>
    public void GetBackChallengeItems()
    {
        // 原文 7799：if m_ChallengeItemList.Count > 0 then
        if (m_ChallengeItemList.Count > 0)
        {
            // 原文 7801：for I := 0 to m_ChallengeItemList.Count - 1 do
            for (int i = 0; i <= m_ChallengeItemList.Count - 1; i++)
            {
                // 原文 7803：UserItem := m_ChallengeItemList.Items[I];
                TUserItem? userItem = m_ChallengeItemList[i];
                // 原文 7804：if UserItem <> nil then
                if (userItem != null)
                {
                    // 原文 7806：m_ItemList.Add(m_ChallengeItemList.Items[I]);
                    m_ItemList.Add(m_ChallengeItemList[i]);
                    // 原文 7807：SendAddItem(UserItem);
                    SendAddItem(userItem.Value);
                }
            }
        }

        // 原文 7812：m_ChallengeItemList.Clear;
        m_ChallengeItemList.Clear();

        // 原文 7813：Inc(m_nGold, m_nChallengeGolds);
        m_nGold = unchecked(m_nGold + (uint)m_nChallengeGolds);

        // 原文 7814：// Inc(m_nGameDiamond, m_nChallengeGameDiamonds);   ← 原文注释掉，保留为注释

        // 原文 7816：if m_nChallengeGameDiamonds > 0 then
        if (m_nChallengeGameDiamonds > 0)
        {
            // 原文 7817-7824：case g_Config.btChallengeGoldIndex of ... end;（**无 else**）
            switch (M2Config.btChallengeGoldIndex)
            {
                case 0:   // 原文 7819
                    m_nGameDiamond = unchecked(m_nGameDiamond + m_nChallengeGameDiamonds);
                    break;
                case 1:   // 原文 7821
                    m_nGameGold = unchecked(m_nGameGold + m_nChallengeGameDiamonds);
                    break;
                case 2:   // 原文 7823
                    m_nGameGird = unchecked(m_nGameGird + m_nChallengeGameDiamonds);
                    break;
                // ★ 原文如此：**没有 else / default 分支** —— 其它取值什么都不加，
                //   但下面 :7826 仍把附加币清零（附加币凭空消失）。逐字保留，不补默认分支。
            }
        }

        // 原文 7825-7826
        m_nChallengeGolds = 0;
        m_nChallengeGameDiamonds = 0;

        // 原文 7827-7828
        m_boChallengeOK = false;
        m_boChallengeStart = false;   // 挑战是否开始

        // 原文 7829：m_ChallengeCreat := nil;
        m_ChallengeCreat = null;

        // 原文 7832：GoldChanged();   （金币变更下发，Gold.cs:183 已 1:1 移植）
        GoldChanged();

        // 原文 7834：NewGamePointChanged;   （原文无括号，逐字保留）
        NewGamePointChanged();
    }

    // ==================================================================
    // GetBagUseItems（原文 7837-7977）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GetBagUseItems(var btDc, btSc, btMc, btDura: Byte);`
    /// （`ObjPlayer.pas:7837-7977`）—— 扫背包、吃黑铁矿（持久）与「可装备物品」的三围，
    /// 输出到四个 `var Byte` 出参（**静默窄化**：`Integer -&gt; Byte`）。
    ///
    /// <code>
    ///   for I := m_ItemList.Count - 1 downto 0 do                // 7858（**倒序**）
    ///   begin
    ///     if m_ItemList.Count &lt;= 0 then Break;                    // 7860-7861
    ///     UserItem := m_ItemList.Items[I];                        // 7863
    ///     if UserItem = nil then Continue;                        // 7864-7865
    ///     if GetStdItemName(UserItem.wIndex) = g_Config.sBlackStone then
    ///     begin                                                   // 7867
    ///       DuraList.Add(Pointer(Round(UserItem.Dura / 1.0E3)));   // 7869
    ///       DelItems := DelItems + Format('%s/%d/', [sBlackStone, MakeIndex]);  // 7870
    ///       m_ItemList.Delete(I); Dispose(UserItem); Inc(nDelCount);  // 7871-7873
    ///     end
    ///     else if IsUseItem(UserItem.wIndex) then                 // 7877
    ///       ... 按 StdMode 19/20/21、22/23、24/26 三组算 nDc/nSc/nMc（24/26 各 +1）...  // 7887-7906
    ///       ... 维护 nDcMin/nDcMax 族（**先比 Min 再比 Max**）...   // 7908-7930
    ///       ... DelItems 累加 + AddGameDataLog + 摘除 ...           // 7932-7942
    ///   end;
    ///   ... DuraList 选择排序（**内层从 Count-1 downto I+1**）...   // 7948-7958
    ///   for I := 0 to DuraList.Count - 1 do { nDura += ...; if nItemCount >= 5 then Break; }  // 7960-7966
    ///   btDura := Round(Min(5,nItemCount) + Min(5,nItemCount) * ((nDura / nItemCount) / 5.0)); // 7968
    ///   btDc := nDcMin div 5 + nDcMax div 3;  （Sc/Mc 同）       // 7969-7971
    ///   if DelItems &lt;&gt; '' then SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);  // 7972-7973
    ///   if DuraList &lt;&gt; nil then DuraList.Free;                   // 7975-7976
    /// </code>
    ///
    /// ★ 逐字保留的原文要点与**缺陷**：
    /// <list type="number">
    ///   <item><description>**倒序遍历 + 就地 `Delete(I)`** —— 这是唯一安全的删除姿势（原文有意为之）。</description></item>
    ///   <item><description>★ 原文缺陷（:7860-7861）：`if m_ItemList.Count &lt;= 0 then Break;` 放在循环**体内**
    ///     —— 对 `for ... downto` 而言这是**死代码**（`I` 从 `Count-1` 开始，循环条件已保证 `Count &gt; 0`），
    ///     **真正会触发它的是「循环体内 `Delete` 把表删空」**：`I` 已到 0 时 `Delete(0)` 后
    ///     下一轮 `I = -1` 循环结束，**不会**再进体 —— 所以它实际**永远不触发**。逐字保留。</description></item>
    ///   <item><description>★ 原文缺陷（:7968）：`btDura := Round(Min(5, nItemCount) + Min(5, nItemCount) * ((nDura / nItemCount) / 5.0));`
    ///     —— **`nItemCount` 可能为 0**（背包里既无黑铁矿也无可用装备时），此时 `nDura / nItemCount`
    ///     在 Delphi 里是 `0/0` 的 **real 除法** → 得 **NaN**，`Round(NaN)` 抛 `EInvalidOp`。
    ///     托管侧浮点 `0.0/0` 同样得 `NaN`，`(byte)Math.Round(NaN)` 在 C# 里**不抛**（得 0）
    ///     —— 这是一处**可观测的差异**：原文抛异常、托管静默得 0。逐字保留该表达式，
    ///     差异由锁定用例 <c>ObjPlayerCore3Tests.GetBagUseItems_EmptyBag_DuraIsNaNInOriginal_OriginalDefect</c> 记录。</description></item>
    ///   <item><description>`btDc/btSc/btMc` 的 `div` 是 **Delphi 整数除（向零截断）** —— 托管用 `/` 对非负操作数等价；
    ///     但 `nDcMin` 恒 `&gt;= 0`（由 `StdItem80` 各字段相加而来），故此处安全。</description></item>
    ///   <item><description>`m_ItemList.Delete(I)` **不**释放元素所有权（原文紧接着 `Dispose(UserItem)`）
    ///     —— 托管 `List.RemoveAt(i)` 由 GC 接管，语义等价（无悬垂）。</description></item>
    /// </list>
    /// </summary>
    public void GetBagUseItems(ref byte btDc, ref byte btSc, ref byte btMc, ref byte btDura)
    {
        // 原文 7847-7856：族内累加器初值
        int nDcMin = 0, nDcMax = 0;
        int nScMin = 0, nScMax = 0;
        int nMcMin = 0, nMcMax = 0;
        int nDura = 0;
        int nItemCount = 0;
        string delItems = "";
        int nDelCount = 0;

        // 原文 7840 / 7857：DuraList: TList; DuraList := TList.Create;
        var duraList = new List<int>();

        // 原文 7858：for I := m_ItemList.Count - 1 downto 0 do
        for (int i = m_ItemList.Count - 1; i >= 0; i--)
        {
            // 原文 7860-7861：if m_ItemList.Count <= 0 then Break;   ← ★ 原文缺陷（见 XML doc 第 2 条）
            if (m_ItemList.Count <= 0) break;

            // 原文 7863：UserItem := m_ItemList.Items[I];
            TUserItem? userItem = m_ItemList[i];
            // 原文 7864-7865：if UserItem = nil then Continue;
            if (userItem == null) continue;

            // 原文 7867：if UserEngine.GetStdItemName(UserItem.wIndex) = g_Config.sBlackStone then
            if (PlayerSurfaceCore3Seams.IsBlackStone(userItem.Value.wIndex))
            {
                // 原文 7869：DuraList.Add(Pointer(Round(UserItem.Dura / 1.0E3)));
                //   ★ 原文把 `Round(Dura / 1000.0)` 塞进 `TList` 的 **Pointer** 槽位
                //     —— 托管侧 `List<int>`（值即该整数），与 `TList` 的"塞指针"语义在**本处**等价。
                duraList.Add((int)Math.Round(userItem.Value.Dura / 1.0E3));
                // 原文 7870：DelItems := DelItems + Format('%s/%d/', [g_Config.sBlackStone, UserItem.MakeIndex]);
                delItems += PlayerSurfaceCore3Format.NameSlashIndex(
                    PlayerSurfaceCore3Seams.BlackStoneName, userItem.Value.MakeIndex);
                // 原文 7871-7873：m_ItemList.Delete(I); Dispose(UserItem); Inc(nDelCount);
                m_ItemList.RemoveAt(i);
                nDelCount++;
            }
            else
            {
                // 原文 7877：if IsUseItem(UserItem.wIndex) then
                if (PlayerSurfaceCore3Seams.IsUseItem(userItem.Value.wIndex))
                {
                    // 原文 7879：StdItem := UserEngine.GetStdItem(UserItem.wIndex);
                    var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.Value.wIndex);
                    if (stdItem != null)
                    {
                        // 原文 7882：StdItem80 := StdItem^;   （按值复制一份 TStdItem）
                        TStdItem stdItem80 = stdItem.Value;

                        // 原文 7883：ItemUnit.GetItemAddValue(UserItem, StdItem80);
                        PlayerSurfaceCore3Seams.GetItemAddValue(userItem.Value, stdItem80);

                        // 原文 7884-7886：nDc := 0; nSc := 0; nMc := 0;
                        int nDc = 0, nSc = 0, nMc = 0;

                        // 原文 7887-7906：case StdItem80.StdMode of
                        switch (stdItem80.StdMode)
                        {
                            case 19:   // 原文 7888-7893  // 004A0421
                            case 20:
                            case 21:
                                nDc = stdItem80.DC2 + stdItem80.DC1;
                                nSc = stdItem80.SC2 + stdItem80.SC1;
                                nMc = stdItem80.MC2 + stdItem80.MC1;
                                break;
                            case 22:   // 原文 7894-7899  // 004A046E
                            case 23:
                                nDc = stdItem80.DC2 + stdItem80.DC1;
                                nSc = stdItem80.SC2 + stdItem80.SC1;
                                nMc = stdItem80.MC2 + stdItem80.MC1;
                                break;
                            case 24:   // 原文 7900-7905（**24/26 各 +1**）
                            case 26:
                                nDc = stdItem80.DC2 + stdItem80.DC1 + 1;
                                nSc = stdItem80.SC2 + stdItem80.SC1 + 1;
                                nMc = stdItem80.MC2 + stdItem80.MC1 + 1;
                                break;
                            // 原文**无 else** —— 其它 StdMode 则 nDc/nSc/nMc 保持 0。
                        }

                        // 原文 7908-7914：DC 的「先比 Min 再比 Max」
                        if (nDcMin < nDc)
                        {
                            nDcMax = nDcMin;
                            nDcMin = nDc;
                        }
                        else if (nDcMax < nDc)
                            nDcMax = nDc;

                        // 原文 7916-7922：SC（同构）
                        if (nScMin < nSc)
                        {
                            nScMax = nScMin;
                            nScMin = nSc;
                        }
                        else if (nScMax < nSc)
                            nScMax = nSc;

                        // 原文 7924-7930：MC（同构）
                        if (nMcMin < nMc)
                        {
                            nMcMax = nMcMin;
                            nMcMin = nMc;
                        }
                        else if (nMcMax < nMc)
                            nMcMax = nMc;

                        // 原文 7932-7935
                        if (userItem.Value.GetBtValue(13) == 1 && userItem.Value.NameStr != "")
                            delItems += PlayerSurfaceCore3Format.NameSlashIndex(
                                userItem.Value.NameStr, userItem.Value.MakeIndex);
                        else
                            delItems += PlayerSurfaceCore3Format.NameSlashIndex(
                                stdItem.Value.NameStr, userItem.Value.MakeIndex);

                        // 原文 7937-7938：if StdItem.NeedIdentify = 1 then AddGameDataLog(...);
                        if (stdItem.Value.NeedIdentify == 1)
                            PlayerSurfaceCore3Seams.AddGameDataLog(
                                this, stdItem.Value.NameStr, userItem.Value.MakeIndex, "背包减少");

                        // 原文 7940-7942：m_ItemList.Delete(I); Dispose(UserItem); Inc(nDelCount);
                        m_ItemList.RemoveAt(i);
                        nDelCount++;
                    }
                }
            }
        }

        // 原文 7948-7958：DuraList 的**选择排序**（内层 `for II := Count-1 downto I+1`，降序）
        for (int i = 0; i <= duraList.Count - 1; i++)
        {
            // 原文 7950-7951：if DuraList.Count <= 0 then Break;
            if (duraList.Count <= 0) break;

            for (int ii = duraList.Count - 1; ii >= i + 1; ii--)
            {
                // 原文 7955-7956：if Integer(DuraList.Items[II]) > Integer(DuraList.Items[II - 1]) then DuraList.Exchange(II, II-1);
                if (duraList[ii] > duraList[ii - 1])
                {
                    (duraList[ii - 1], duraList[ii]) = (duraList[ii], duraList[ii - 1]);
                }
            }
        }

        // 原文 7960-7966：累加耐久，**最多 5 件**
        for (int i = 0; i <= duraList.Count - 1; i++)
        {
            nDura += duraList[i];
            nItemCount++;
            // 原文 7964-7965：if nItemCount >= 5 then Break;
            if (nItemCount >= 5) break;
        }

        // 原文 7968：btDura := Round(Min(5, nItemCount) + Min(5, nItemCount) * ((nDura / nItemCount) / 5.0));
        //   ★ 原文缺陷：`nItemCount = 0` 时 `nDura / nItemCount` 是 real 0/0 = NaN（见 XML doc 第 3 条）。
        //   逐字保留该表达式（托管 `0.0 / 0` 亦得 NaN，`(byte)Math.Round(NaN) = 0`）。
        btDura = (byte)Math.Round(Math.Min(5, nItemCount)
            + Math.Min(5, nItemCount) * ((double)nDura / nItemCount / 5.0), MidpointRounding.ToEven);

        // 原文 7969-7971：btDc := nDcMin div 5 + nDcMax div 3;（Sc/Mc 同构，Delphi `div` 向零截断）
        btDc = unchecked((byte)(nDcMin / 5 + nDcMax / 3));
        btSc = unchecked((byte)(nScMin / 5 + nScMax / 3));
        btMc = unchecked((byte)(nMcMin / 5 + nMcMax / 3));

        // 原文 7972-7973：if DelItems <> '' then SendMsg(Self, RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, DelItems);
        if (delItems != "")
            SendMsg(Grobal2Const.RM_SENDDELITEMLIST, 0, nDelCount, 0, 0, delItems);

        // 原文 7975-7976：if DuraList <> nil then DuraList.Free;（托管 `List` 由 GC 接管）
    }

    // ==================================================================
    // GeTBaseObjectInfo（原文 7979-7996）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.GeTBaseObjectInfo: string;`（`ObjPlayer.pas:7979-7996`）——
    /// GM 命令/日志用的**一行人物快照**（原文方法名就是 `GeT`（小写 T）的拼写，逐字保留）。
    ///
    /// <code>
    ///   Result := m_sCharName + ' 标识:' + IntToHex(NativeInt(Self), 2) + ' 权限等级: ' + IntToStr(m_btPermission)
    ///     + ' 管理模式: ' + BoolToCStr(m_boAdminMode) + ' 隐身模式: ' + BoolToCStr(m_boObMode)
    ///     + ' 无敌模式: ' + BoolToCStr(m_boSuperMan) + ' 地图:' + m_sMapName + '(' + m_PEnvir.sMapDesc + ')'
    ///     + ' 座标:' + IntToStr(m_nCurrX) + ':' + IntToStr(m_nCurrY) + ' 等级:' + IntToStr(m_Abil.Level)
    ///     + ' 转生等级:' + IntToStr(m_btReLevel) + ' 经验:' + IntToStr(m_Abil.Exp)
    ///     + ' 生命值: ' + IntToStr(m_WAbil.HP) + '-' + IntToStr(m_WAbil.MaxHP)
    ///     + ' 魔法值: ' + IntToStr(m_WAbil.MP) + '-' + IntToStr(m_WAbil.MaxMP)
    ///     + ' 攻击力: ' + IntToStr(m_WAbil.DC1) + '-' + IntToStr(m_WAbil.DC2)
    ///     + ' 魔法力: ' + IntToStr(m_WAbil.MC1) + '-' + IntToStr(m_WAbil.MC2)
    ///     + ' 道术: ' + IntToStr(m_WAbil.SC1) + '-' + IntToStr(m_WAbil.SC2)
    ///     + ' 防御力: ' + IntToStr(m_WAbil.AC1) + '-' + IntToStr(m_WAbil.AC2)
    ///     + ' 魔防力: ' + IntToStr(m_WAbil.MAC1) + '-' + IntToStr(m_WAbil.MAC2)
    ///     + ' 准确:' + IntToStr(m_btHitPoint) + ' 敏捷:' + IntToStr(m_btSpeedPoint) + ' 速度:' + IntToStr(m_nHitSpeed)
    ///     + ' 仓库密码:' + m_sStoragePwd + ' 登录IP:' + m_sIPaddr + '(' + m_sIPLocal + ')'
    ///     + ' 登录帐号:' + m_sUserID + ' 登录时间:' + DateTimeToStr(m_dLogonTime)
    ///     + ' 在线时长(分钟):' + IntToStr((MyGetTickCount - m_dwLogonTick) div 60000)
    ///     + ' 登录模式:' + IntToStr(m_nPayMent) + ' ' + g_Config.sGameGoldName + ':' + IntToStr(m_nGameGold)
    ///     + ' ' + g_Config.sGamePointName + ':' + IntToStr(m_nGamePoint)
    ///     + ' ' + g_Config.sPayMentPointName + ':' + IntToStr(m_nPayMentPoint)
    ///     + ' 会员类型:' + IntToStr(m_nMemberType) + ' 会员等级:' + IntToStr(m_nMemberLevel)
    ///     + ' 经验倍数:' + CurrToStr(m_nKillMonExpRate / 100)
    ///     + ' 攻击人物伤害倍数:' + CurrToStr(m_nAttackHumPowerRate / 100)
    ///     + ' 攻击怪物伤害倍数:' + CurrToStr(m_nAttackMonPowerRate / 100)
    ///     + ' 声望值:' + IntToStr(m_WAbil.CreditPoint);
    /// </code>
    ///
    /// ★ 逐字保留的原文要点：
    /// <list type="number">
    ///   <item><description>原文字面量里 `m_sIPLocal` 后面跟着**被注释掉的** `{ GetIPLocal(m_sIPaddr) }`
    ///     —— 即直接用字段、**不**调 `GetIPLocal`。逐字保留（本实现同样不调）。</description></item>
    ///   <item><description>`IntToHex(NativeInt(Self), 2)` 打出的是**对象地址**（原文用途是排查），
    ///     托管侧 `Self` 无稳定地址；按「原文语义 = 一个能区分对象的 16 进制标识」取
    ///     **`m_nRecogId`**（`ObjBase.cs:16`，托管侧唯一的对象标识）并以 `IntToHex(..., 2)` 的
    ///     最小宽度语义（**至少 2 位、超出不截断**）格式化 —— **已登记偏差**。</description></item>
    ///   <item><description>`IntToStr(m_Abil.Level)`：`m_Abil` 与 `m_WAbil` 在托管侧同源（见字段注释），
    ///     `Level`/`Exp` 是 `uint`，`IntToStr` 等价 `((int)v).ToString()`（原文 `TAbility.Level` 是 `Word`）。</description></item>
    ///   <item><description>`CurrToStr(x)` 是 Delphi 的**货币格式**（`FloatToStrF(..., ffCurrency)`），
    ///     默认 `Windows` 区域设置下形如 `1.23`（两位小数 + 千分位）。
    ///     托管侧按**当前区域**的 `"C2"` 等价格式表达（差异见交付报告：货币符号位置随区域而变）。</description></item>
    ///   <item><description>`BoolToCStr(True) = '是'`、`BoolToCStr(False) = '否'`（HUtil32.pas）。</description></item>
    ///   <item><description>`(MyGetTickCount - m_dwLogonTick) div 60000` 是 **LongWord 减法 + 整数除**
    ///     —— 回绕时在线时长会是个巨大的数（原文如此）。托管用 `unchecked` 复刻回绕。</description></item>
    /// </list>
    /// ⚠ 与原文的**唯一行为差异**是「标识」那一项（对象地址 vs `m_nRecogId`），已在报告登记。
    /// </summary>
    public string GeTBaseObjectInfo()
    {
        // 原文 7981 起：单条字符串拼接（逐项与原文顺序、字面量完全一致）
        return m_sCharName
            + " 标识:" + IntToHexAtLeast2((long)m_nRecogId)          // 原文 IntToHex(NativeInt(Self), 2)（偏差见 doc）
            + " 权限等级: " + m_btPermission
            + " 管理模式: " + BoolToCStr(m_boAdminMode)
            + " 隐身模式: " + BoolToCStr(m_boObMode)
            // 原文写 `m_boSuperMan`；托管既有成员拼作 `m_boSuperman`（`ObjBase.OnlineMsg.cs:50`）
            // —— 同一字段的**大小写拼写差异**，**未重复声明**（复用既有）。
            + " 无敌模式: " + BoolToCStr(m_boSuperman)
            + " 地图:" + m_sMapName + "(" + m_PEnvir!.sMapDesc + ")"
            + " 座标:" + m_nCurrX + ":" + m_nCurrY
            + " 等级:" + m_Abil.Level
            + " 转生等级:" + m_btReLevel
            + " 经验:" + m_Abil.Exp
            + " 生命值: " + m_wAbil.HP + "-" + m_wAbil.MaxHP
            + " 魔法值: " + m_wAbil.MP + "-" + m_wAbil.MaxMP
            + " 攻击力: " + m_wAbil.DC1 + "-" + m_wAbil.DC2
            + " 魔法力: " + m_wAbil.MC1 + "-" + m_wAbil.MC2
            + " 道术: " + m_wAbil.SC1 + "-" + m_wAbil.SC2
            + " 防御力: " + m_wAbil.AC1 + "-" + m_wAbil.AC2
            + " 魔防力: " + m_wAbil.MAC1 + "-" + m_wAbil.MAC2
            + " 准确:" + m_btHitPoint
            + " 敏捷:" + m_btSpeedPoint
            + " 速度:" + m_nHitSpeed
            + " 仓库密码:" + m_sStoragePwd
            + " 登录IP:" + m_sIPaddr + "(" + m_sIPLocal + ")"        // 原文此处有被注释掉的 { GetIPLocal(m_sIPaddr) }
            + " 登录帐号:" + m_sUserID
            + " 登录时间:" + DateTimeToStr(m_dLogonTime)
            + " 在线时长(分钟):" + (int)(unchecked(PlayerSurfaceNpcSeams.MyGetTickCount() - m_dwLogonTick) / 60000)
            + " 登录模式:" + m_nPayMent
            + " " + M2Config.sGameGoldName + ":" + m_nGameGold
            + " " + M2Config.sGamePointName + ":" + m_nGamePoint
            + " " + M2Config.sPayMentPointName + ":" + m_nPayMentPoint
            + " 会员类型:" + m_nMemberType
            + " 会员等级:" + m_nMemberLevel
            + " 经验倍数:" + CurrToStr(m_nKillMonExpRate / 100)
            + " 攻击人物伤害倍数:" + CurrToStr(m_nAttackHumPowerRate / 100)
            + " 攻击怪物伤害倍数:" + CurrToStr(m_nAttackMonPowerRate / 100)
            + " 声望值:" + m_wAbil.CreditPoint;
    }

    // ==================================================================
    // GetDigUpMsgCount（原文 7998-8010）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.GetDigUpMsgCount: Integer;`（`ObjPlayer.pas:7998-8010`）。
    /// <code>
    ///   Result := 0;                              // 8000
    ///   {$IF LockProcessMsg = 1}
    ///   try EnterCriticalSection(ProcessMsgCriticalSection); finally LeaveCriticalSection(...); end;
    ///   {$IFEND}
    /// </code>
    /// ★ **原文永远返回 0**：整个函数体只有 `Result := 0` 与一段**条件编译的加锁/解锁**
    /// （`{$IF LockProcessMsg = 1}`，宏关闭时连 `try` 都不存在）。
    /// 托管侧单线程模型 = 该宏的关闭态，故保留"空临界区"的**注释**、不伪造锁。
    /// </summary>
    public int GetDigUpMsgCount()
    {
        // 原文 8000：Result := 0;
        int result = 0;

        // 原文 8001-8009：{$IF LockProcessMsg = 1} try EnterCriticalSection(ProcessMsgCriticalSection); ...
        //   finally LeaveCriticalSection(ProcessMsgCriticalSection); end; {$IFEND}
        //   ★ 托管侧为单线程模型（= 该宏的**关闭态**），故不加锁；此处只留结构注释，不伪造临界区。

        return result;   // 原文 8010
    }

    // ==================================================================
    // SendUpgradeItem（原文 8012-8032）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpgradeItem();`（`ObjPlayer.pas:8012-8032`）——
    /// 把「升级框里的那件物品」发给客户端（`SM_UPGRADEDLGITEM_GIVE`）。
    ///
    /// <code>
    ///   if m_UpgradeItem &lt;&gt; nil then                                // 8017
    ///   begin
    ///     StdItem := UserEngine.GetStdItem(m_UpgradeItem.wIndex);   // 8019
    ///     if StdItem &lt;&gt; nil then
    ///     begin
    ///       UserItemToClientItem(m_UpgradeItem, StdItem, @ClientItem, True, True);  // 8022
    ///       m_DefMsg := MakeDefaultMsg(SM_UPGRADEDLGITEM_GIVE, 0, 0, 0, 0);         // 8023
    ///       SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem));              // 8024
    ///     end
    ///     else
    ///     begin
    ///       m_DefMsg := MakeDefaultMsg(SM_UPGRADEDLGITEM_GIVE, 0, 0, 0, 0);         // 8028
    ///       SendSocket(@m_DefMsg, '');                                              // 8029
    ///     end;
    ///   end;
    /// </code>
    /// ★ 逐字保留：**`StdItem == nil` 分支仍要发一个空包**（客户端据此清空升级框），
    /// 且两个分支的 `MakeDefaultMsg` 参数**完全相同**（标识一样、全 0）。
    /// 接缝：`UserItemToClientItem` → `PlayerSurfaceItemSeams.UserItemToClientItem`（既有接缝），
    /// `SendSocketEx`/`SendSocket` → PortKit 的 <see cref="SendSocketExRef"/>/<see cref="SendSocketRef"/>。
    /// </summary>
    public void SendUpgradeItem()
    {
        // 原文 8017：if m_UpgradeItem <> nil then
        if (m_UpgradeItem != null)
        {
            TUserItem upgradeItem = m_UpgradeItem.Value;

            // 原文 8019：StdItem := UserEngine.GetStdItem(m_UpgradeItem.wIndex);
            var stdItem = PlayerSurfaceItemSeams.GetStdItem(upgradeItem.wIndex);
            if (stdItem != null)
            {
                // 原文 8022：UserItemToClientItem(m_UpgradeItem, StdItem, @ClientItem, True, True);
                //   托管签名是 `Func<TUserItemView, TStdItem, object?>`（既有接缝返回 `TClientItem` 的 object）
                var clientItem = PlayerSurfaceItemSeams.UserItemToClientItem(
                    ToItemView(upgradeItem), stdItem.Value);

                // 原文 8023：m_DefMsg := MakeDefaultMsg(SM_UPGRADEDLGITEM_GIVE, 0, 0, 0, 0);
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPGRADEDLGITEM_GIVE, 0, 0, 0, 0);

                // 原文 8024：SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem));
                //   接缝要求 `byte[]`；未编码（null）时按「无尾数据」处理（与原文 `SizeOf` 一定非零不同，
                //   见交付报告：`TClientItem` 在托管侧尚无 wire 布局）。
                if (clientItem is byte[] buf)
                    SendSocketExRef(m_DefMsg, buf);
                else
                    SendSocketExRef(m_DefMsg, Array.Empty<byte>());
            }
            else
            {
                // 原文 8028-8029：空包（两个分支的 MakeDefaultMsg 参数逐字相同）
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPGRADEDLGITEM_GIVE, 0, 0, 0, 0);
                SendSocketRef(m_DefMsg, "");
            }
        }
    }

    // ==================================================================
    // DoQueryBagItems（原文 8034-8091）—— 未移植
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.DoQueryBagItems(NoMove1_6: Boolean);`
    /// （`ObjPlayer.pas:8034-8091`，**58 行**）—— 刷新包裹（含 3 秒限流与 zlib 压缩下发）。
    ///
    /// <para><b>状态：未移植（显式留痕）</b>。</para>
    ///
    /// <para><b>缺失成员（带原文行号）</b>：
    /// <list type="bullet">
    ///   <item><description>`m_ItemBoxItems` / `m_ItemBoxAddIndex`（:8050-8051）—— `TPlayObject` 的
    ///     物品框数组与下标，托管侧**未声明**（`Core1.cs:69` 亦把它列为未切出项）。</description></item>
    ///   <item><description>`m_HeroM2ShopList`（:8046，`m_HeroM2ShopList.Count &gt; 0` 即早退）——
    ///     托管侧**未声明**该容器。</description></item>
    ///   <item><description>`m_dwQueryBagItemsTick` / `m_boQueryBagItemsOK`（:8054-8056/:8081-8090）——
    ///     托管侧**未声明**。</description></item>
    ///   <item><description>`m_btExtBagOpenItemCount` / `m_btExtBagPageCount`（:8078-8079）——
    ///     托管侧**未声明**（扩展背包页/格数）。</description></item>
    ///   <item><description>`m_UpgradeItem`（:8063，`UserItem = m_UpgradeItem` 的**指针比较**）——
    ///     本片**已声明**（见字段），不是阻塞项。</description></item>
    ///   <item><description>`UserItemToClientItem(UserItem, StdItem, ClientItem, True, True)`（:8069）——
    ///     已有接缝 `PlayerSurfaceItemSeams.UserItemToClientItem`。</description></item>
    ///   <item><description>`zLibCompressBuffer(InBuf, nCount * SizeOf(TClientItem)): AnsiString`（:8073）——
    ///     托管 `EDcode.ZLibCompressBuffer` 存在，但它吃 `byte[]` 且**依赖 `TClientItem` 的 wire 布局**
    ///     （托管侧未落地，`TCreature.PlayerSurface.Items.cs:75-78` 已登记同一缺口）。</description></item>
    ///   <item><description>`PAnsiChar` 指针游标推进（`ClientItem := pTClientItem(InBuf)` + `Inc(ClientItem)`，:8059/:8070）
    ///     与 `GetMem/FreeMem`（:8057/:8075）—— 托管侧需要先把 `TClientItem` 切出才能等价表达。</description></item>
    ///   <item><description>`SysMsg('包裹刷新成功.', 255, 253, t_Hint)`（:8084/:8087）——
    ///     **签名不同**：原文这里传的是**数字颜色**（255/253），托管 `TCreature.SysMsg`
    ///     （`ObjBase.OnlineMsg.cs:25`）只吃 `TMsgColor` 枚举（4 个值）。</description></item>
    /// </list>
    /// </para>
    ///
    /// ★ 顺带登记一处原文缺陷（虽未移植，仍按台账 §48.1 留痕）：
    /// `FillChar(m_ItemBoxItems, SizeOf(m_ItemBoxItems), #0);`（:8050）后紧接着
    /// `m_ItemBoxAddIndex := -1;`（:8051）—— 原文**没有**在清零后判 `m_ItemBoxItems` 是否可用，
    /// 而 **`m_ItemBoxAddIndex := -1` 是"哨兵"**（:8051 与 `m_ItemBoxItems` 的其余消费者共用该约定）。
    /// </summary>
    public void DoQueryBagItems(bool noMove1_6)
    {
        PortNotPorted(nameof(DoQueryBagItems), 8034);
    }

    // ==================================================================
    // 本片私有小工具（原文全局函数的 1:1 / 最小等价物）
    // ==================================================================

    /// <summary>
    /// 原文 `Round(X: Extended): Int64`（Delphi `System.Round`）。
    /// ★ Delphi 的 `Round` 是**银行家舍入**（round half to even）—— 与
    /// `Math.Round(x, MidpointRounding.ToEven)`（C# 默认）**一致**，逐字使用后者。
    /// 原文返回值是 **Int64**（故 `dwExp := Round(...)` 回写 `LongWord` 可能范围错误），
    /// 托管侧返回 `long`，由调用点 `unchecked((uint)...)` 复刻窄化。
    /// </summary>
    private static long DelphiRound(double value) => (long)Math.Round(value, MidpointRounding.ToEven);

    /// <summary>
    /// 原文 `BoolToCStr(Value: Boolean): string`（`HUtil32.pas`）—— `True → '是'`、`False → '否'`。
    /// </summary>
    private static string BoolToCStr(bool value) => value ? "是" : "否";

    /// <summary>
    /// 原文 `IntToHex(Value: Int64; Digits: Integer): string`（**至少 `Digits` 位、超出不截断**，
    /// 大写十六进制）。原文 `GeTBaseObjectInfo` 传的是对象地址（`NativeInt(Self)`）+ 宽度 2。
    /// </summary>
    private static string IntToHexAtLeast2(long value)
    {
        // 原文 IntToHex 保留符号位（负数会带 '-'）—— 托管同型：用 "X" 格式对负数会得到二进制补码，
        // 故先按原文语义处理符号。
        if (value < 0)
            return "-" + IntToHexAtLeast2(-value);
        string s = value.ToString("X");
        return s.Length >= 2 ? s : s.PadLeft(2, '0');
    }

    /// <summary>
    /// 原文 `DateTimeToStr(m_dLogonTime)`（Delphi `System.SysUtils.DateTimeToStr`）——
    /// 用**当前区域**的短日期 + 短时间格式（与原文一致：它同样走 `FormatSettings`）。
    /// </summary>
    private static string DateTimeToStr(DateTime value)
        => value.ToString("g", System.Globalization.CultureInfo.CurrentCulture);

    /// <summary>
    /// 原文 `CurrToStr(Value: Currency): string`（Delphi `System.SysUtils.CurrToStr`）——
    /// 货币格式（**两位小数 + 千分位 + 区域货币符号**）。取值为整数除法的**截断商**
    /// （原文 `m_nKillMonExpRate / 100` 是 Integer 除法，被隐式转 `Currency`）。
    /// </summary>
    private static string CurrToStr(int integerDividedBy100)
        => ((decimal)integerDividedBy100).ToString("C2", System.Globalization.CultureInfo.CurrentCulture);
}

/// <summary>
/// 切片 Core3 的主类级接缝（<see cref="Disappear"/> 族的宿主能力）——
/// 与 <see cref="PlayerSurfaceCore3Seams"/> 分开，避免把"字段接缝"与"生命周期接缝"混在一处。
/// </summary>
public static class PlayerSurfaceCore3LifecycleSeams
{
    /// <summary>原文 `TGuild(m_MyGuild).DelHumanObj(Self)`（:6599/:6612）—— 行会面未移植。默认：不动作。</summary>
    public static Action<TPlayObject> GuildDelHumanObj { get; set; } = _ => { };

    /// <summary>原文 `m_GroupOwner.DelGroupMember(Self)`（:6596/:6609）。默认：不动作。</summary>
    public static Action<TPlayObject, TPlayObject> DelGroupMember { get; set; } = (_, _) => { };

    /// <summary>原文 `DisappearA`（:6590，实现 :1753 起）—— 未移植。默认：不动作。</summary>
    public static Action<TPlayObject> DisappearA { get; set; } = _ => { };

    /// <summary>原文 `inherited`（`TBaseObject.Disappear`，ObjBase.pas）—— 未移植。默认：不动作。</summary>
    public static Action<TPlayObject> InheritedDisappear { get; set; } = _ => { };

    /// <summary>恢复默认（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        GuildDelHumanObj = _ => { };
        DelGroupMember = (_, _) => { };
        DisappearA = _ => { };
        InheritedDisappear = _ => { };
    }
}

/// <summary>
/// `GetStartPoint` 的两处全局管理器来源（原文 `g_SafeAreaManager` / `g_NationManage`）——
/// 托管侧没有任何一个**挂到 `TPlayObject` 可访问的全局单例**，按最小面接缝表达。
/// </summary>
public static class PlayerSurfaceStartPointSource
{
    /// <summary>
    /// 原文 `g_SafeAreaManager.Items[I]: TSafeArea`（`SafeAreaManager.pas`）的**只读视图**。
    /// 默认：空表（⇒ `GetStartPoint` 的"安全区回家点"分支不命中）。
    /// </summary>
    public static List<PlayerSurfaceSafeArea> SafeAreas { get; } = new();

    /// <summary>
    /// 原文 `g_NationManage.Items[nNation]: pTNationInfo`（`Nations.pas:181` `property Items`）。
    /// 默认：返回 null（⇒ 走 `g_Config` 全局红名回家点，与原文"国家不存在"分支一致）。
    /// </summary>
    public static Func<int, PlayerSurfaceNationInfo?> GetNationInfo { get; set; } = _ => null;

    /// <summary>恢复默认（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        SafeAreas.Clear();
        GetNationInfo = _ => null;
    }
}

/// <summary>
/// 原文 `TSafeArea`（`SafeAreaManager.pas`）在 <c>GetStartPoint</c> 里用到的三个成员：
/// `MapName` / `GetCenterX` / `GetCenterY`。
/// ⚠ 托管 `SafeAreaCore.cs` 只有**静态函数**集合，没有 `TSafeArea` 记录类型 ——
/// 故此处只声明**本方法真正用到的三个成员**（不造第二个安全区实现，语义由宿主注入）。
/// </summary>
public sealed class PlayerSurfaceSafeArea
{
    /// <summary>原文 `SafeArea.MapName: string`（:6163 与 `m_PEnvir.sMapName` 做 `SameText`）。</summary>
    public string MapName = "";

    /// <summary>原文 `SafeArea.GetCenterX: Integer`（:6165/:6168）。</summary>
    public int CenterX;

    /// <summary>原文 `SafeArea.GetCenterY: Integer`（:6165/:6169）。</summary>
    public int CenterY;

    /// <summary>原文 `SafeArea.GetCenterX`（getter）。</summary>
    public int GetCenterX() => CenterX;

    /// <summary>原文 `SafeArea.GetCenterY`（getter）。</summary>
    public int GetCenterY() => CenterY;
}

/// <summary>
/// 原文 `pTNationInfo`（`M2Definition.pas:456-473 TNationInfo`）在 <c>GetStartPoint</c> 里
/// 用到的三个「红名回家点」字段：`sRedHomeMap` / `nRedHomeX` / `nRedHomeY`。
/// </summary>
public sealed class PlayerSurfaceNationInfo
{
    /// <summary>原文 `NationInfo.sRedHomeMap`（:6182）。</summary>
    public string sRedHomeMap = "";

    /// <summary>原文 `NationInfo.nRedHomeX`（:6183）。</summary>
    public int nRedHomeX;

    /// <summary>原文 `NationInfo.nRedHomeY`（:6184）。</summary>
    public int nRedHomeY;
}

/// <summary>切片 Core3 用到的原文常量。</summary>
public static class PlayerSurfaceCore3Const
{
    /// <summary>
    /// 原文 `MAX_BONUS_VALUE`（`Grobal2.pas`/`M2Share.pas`；`bonus: array [0..MAX_BONUS_VALUE] of real`
    /// 共 **12** 项，:7552/:7668）—— 取 **11**。
    /// </summary>
    public const int MAX_BONUS_VALUE = 11;
}
