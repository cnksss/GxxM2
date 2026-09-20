// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK，49,232 LF）／Source\M2Engine\ObjBase.pas
// 本文件：**金额容器片**（切片 2 / 4）。
// 覆盖原文行号范围：
//   ObjPlayer.pas:201   m_nGameGoldEx: Integer;
//   ObjPlayer.pas:250   m_nDealGoldPose: Integer;
//   ObjPlayer.pas:48    m_nBigStoragePage: Integer;
//   ObjPlayer.pas:1168  procedure GoldChanged();
//   ObjPlayer.pas:1169  procedure GameGoldChanged;
//   ObjPlayer.pas:2529-2532  TPlayObject.GoldChanged
//   ObjPlayer.pas:2534-2537  TPlayObject.GameGoldChanged
//   ObjPlayer.pas:1181  function IncGold(tGold: Cardinal): Boolean;
//   ObjPlayer.pas:3231-3249  TPlayObject.IncGold（含 Delphi Cardinal 回绕）
//   ObjPlayer.pas:1212  function DecGold(nGold: Cardinal): Boolean;
//   ObjPlayer.pas:3284-3293  TPlayObject.DecGold
//   ObjPlayer.pas:3251-3260  TPlayObject.IncGameGold
//   ObjPlayer.pas:3295-3303  TPlayObject.DecGameGold
//   ObjBase.pas:105     m_nGold: LongWord;     ← 已在托管侧存在，本车道**不重复声明**
//   ObjBase.pas:106     m_nGoldMax: LongWord;
//   ObjBase.pas:11276   m_nGold := 0;   /  ObjBase.pas:11311  m_nGoldMax := g_Config.nHumanMaxGold;
//   ObjPlayer.pas:202   m_nGameGold: LongWord; ← 已在托管侧存在，本车道**不重复声明**
//
// ⚠ 不重复声明的既有成员（`git grep` 证据见交付报告）：
//   · `m_nGold`      → `Engine/ObjBase.OnlineMsg.cs:44`（`public uint m_nGold`，类型一致）
//   · `m_nGameGold`  → `Engine/ObjBase.OnlineMsg.cs:36`（**类型偏差**：托管 `int`、原文 `LongWord`）
//   · `m_ItemList`   → `Engine/ObjBase.cs:166`（`List<TUserItem>`）
// ============================================================================

using System;
using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>
/// 金额（金币 / 元宝）容器的接缝：原文这些动作最终都要经过 `TBaseObject.SendMsg` 向
/// 客户端下发（`UsrEngn` 与客户端管道批次），托管侧仅 `TCreature.SendMsg` 的**入队版**存在，
/// 签名与语义都不同（无 `BaseObject` 发送者参数、且是入队而非下发）。
/// 故按任务书第 4 条落**最小接缝**，默认实现 = 「无宿主」。
/// </summary>
/// <remarks>
/// 接入方要求（精确签名）见交付报告「接缝清单」：
/// 应把本类三个委托接到 `TBaseObject.SendMsg(BaseObject,wIdent,wParam,nParam1,nParam2,nParam3,sMsg)`。
/// </remarks>
public static class PlayerSurfaceMsgSeams
{
    /// <summary>原文 `SendUpdateMsg(Self, wIdent, 0, 0, 0, 0, '')`
    /// （ObjPlayer.pas:2531 / 2536 / 2541 / 2546）。默认：无宿主，丢弃。</summary>
    public static Action<TCreature, int> SendUpdateMsg { get; set; } = (_, _) => { };

    /// <summary>原文 `SendDefMessage(wIdent, nRecog, nParam, nTag, nSeries, sMsg)`
    /// （ObjPlayer.pas:12655 SM_DELITEM 等）。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, ushort, long, ushort, ushort, ushort, string> SendDefMessage { get; set; }
        = (_, _, _, _, _, _, _) => { };

    /// <summary>原文 `SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem))`
    /// （ObjPlayer.pas:3386 SM_ADDITEM）。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, object> SendSocketEx { get; set; } = (_, _) => { };

    /// <summary>`UserEngine.GetStdItemName(wIndex): string`（ObjBase.pas:41678 CheckItems）。</summary>
    public static Func<int, string> GetStdItemName { get; set; } = _ => "";

    /// <summary>`UserEngine.GetStdItem(wIndex): pTStdItem`（ObjPlayer.pas:3369/12648）。</summary>
    public static Func<int, TStdItem?> GetStdItem { get; set; } = _ => null;

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        SendUpdateMsg = (_, _) => { };
        SendDefMessage = (_, _, _, _, _, _, _) => { };
        SendSocketEx = (_, _) => { };
        GetStdItemName = _ => "";
        GetStdItem = _ => null;
    }
}

/// <summary>
/// ObjPlayer.pas / ObjBase.pas 的**金额容器与操作**。
/// </summary>
public partial class TPlayObject
{
    // ------------------------------------------------------------------
    // 字段（ObjPlayer.pas）
    // ------------------------------------------------------------------

    /// <summary>原文 `m_nGameGoldEx: Integer; // 扩展游戏币`（ObjPlayer.pas:201）。</summary>
    public int m_nGameGoldEx;

    /// <summary>原文 `m_nDealGoldPose: Integer;`（ObjPlayer.pas:250）——交易时摆放的金币数量。</summary>
    public int m_nDealGoldPose;

    /// <summary>原文 `m_nBigStoragePage: Integer; // 无限仓库的当前页数`（ObjPlayer.pas:48）。</summary>
    public int m_nBigStoragePage;

    /// <summary>
    /// 原文 `m_dwRecordBeadExp: LongWord;`（ObjPlayer.pas 内，聚灵珠待结算经验）。
    /// `SendAddItem`(ObjPlayer.pas:3388/3391) 与 `IncBeadExp` 使用；
    /// 原文在 `ClearData` 里清零。
    /// </summary>
    public uint m_dwRecordBeadExp;

    /// <summary>原文 `m_nCurrentItemMakeIndex: Integer;`（ObjPlayer.pas:3376/3379）——`g_FunctionNPC` 的 `@AddBag` 事件期间生效。</summary>
    public int m_nCurrentItemMakeIndex;

    /// <summary>原文 `m_sCurrentItemName: string;`（ObjPlayer.pas:3377/3380）——同上。</summary>
    public string m_sCurrentItemName = "";

    /// <summary>原文 `m_nGateIdx: Integer;`（ObjPlayer.pas:246）——人物所在网关号。
    /// ⚠ 原文 `m_nGSocketIdx`（:245）已由 `Engine/ObjBase.cs:160` 声明，**未重复声明**。</summary>
    public int m_nGateIdx;

    /// <summary>
    /// 原文 `m_nGoldMax: LongWord; // 0x268 人物身上最多可带金币数(Dword)`（ObjBase.pas:106）。
    /// 原文在 :11311 初始化为 `g_Config.nHumanMaxGold`；托管侧该配置已存在
    /// （`Engine/M2Config.GameOption.cs` 的 `nHumanMaxGold`），此处按原文同样取该配置值。
    /// </summary>
    public uint m_nGoldMax = (uint)Math.Max(0, M2Config.nHumanMaxGold);

    // ------------------------------------------------------------------
    // 金币（m_nGold 已在 ObjBase.OnlineMsg.cs:44 声明，此处只用不声明）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `function TPlayObject.IncGold(tGold: Cardinal): Boolean;`（ObjPlayer.pas:3231-3249）。
    /// 逐字语义要点（三条都照抄）：
    /// ① `Result := False` 先置，`tGold = 0` 时**保持 False**（不是「无变化成功」）；
    /// ② `tmpValue := Min(m_nGold + tGold, MAXDWORD - 1)` —— 这里的加法是 **Cardinal 加法**
    ///    （32 位无符号回绕），再加 `Min`；托管侧必须用 `unchecked` 复刻回绕，
    ///    否则 `m_nGold = 4000000000` 加 `tGold = 1000000000` 在 C# 里会得到
    ///    `long` 的 5000000000 而不是回绕后的 705032704；
    /// ③ 超上限时**夹到 `m_nGoldMax`**，`Result := m_nGold > tmpGold`。
    /// </summary>
    public bool IncGold(uint tGold)
    {
        // 原文 3236：Result := False;
        bool result = false;
        // 原文 3237：tmpGold := m_nGold;
        uint tmpGold = m_nGold;

        // 原文 3239：if tGold > 0 then
        if (tGold > 0)
        {
            // 原文 3241：tmpValue := Min(m_nGold + tGold, MAXDWORD - 1);
            // MAXDWORD = $FFFFFFFF，MAXDWORD - 1 = $FFFFFFFE。
            // ⚠ `m_nGold + tGold` 是 **Cardinal 加法**（回绕），故用 unchecked。
            uint tmpValue = Math.Min(unchecked(m_nGold + tGold), uint.MaxValue - 1);
            if (tmpValue <= m_nGoldMax)
                m_nGold = tmpValue;          // 原文 3243
            else
                m_nGold = m_nGoldMax;        // 原文 3245

            // 原文 3247：Result := m_nGold > tmpGold;
            result = m_nGold > tmpGold;
        }

        return result;                       // 原文 3248
    }

    /// <summary>
    /// 原文 `function TPlayObject.DecGold(nGold: Cardinal): Boolean;`（ObjPlayer.pas:3284-3293）。
    /// `m_nGold >= nGold` 才扣，否则**原地不动**并返回 False（不是夹到 0）。
    /// ⚠ 与 `DecGameGold`（:3295-3303）**语义不同**：后者不够时夹到 0。
    /// </summary>
    public bool DecGold(uint nGold)
    {
        // 原文 3286：Result := False;
        bool result = false;

        // 原文 3288：if m_nGold >= nGold then
        if (m_nGold >= nGold)
        {
            m_nGold -= nGold;                // 原文 3290：Dec(m_nGold, nGold);
            result = true;                   // 原文 3291
        }

        return result;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.GoldChanged();`（ObjPlayer.pas:2529-2532 声明于 :1168）。
    /// `SendUpdateMsg(Self, RM_GOLDCHANGED, 0, 0, 0, 0, '')`。
    /// ⚠ **不是** `virtual`（原文无 `virtual`）—— 托管侧同样不标 `virtual`。
    /// </summary>
    public void GoldChanged()
    {
        // 原文 2531
        PlayerSurfaceMsgSeams.SendUpdateMsg(this, Grobal2Const.RM_GOLDCHANGED);
    }

    // ------------------------------------------------------------------
    // 元宝 / 游戏币（m_nGameGold 已在 ObjBase.OnlineMsg.cs:36 声明）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `procedure TPlayObject.IncGameGold(nGameGold: LongWord);`（ObjPlayer.pas:3251-3260）。
    /// 与 `IncGold` **不同**：这里先提升到 `Int64` 判溢出，超 `High(LongWord)` 才夹到上限，
    /// **不做 `MAXDWORD - 1` 也不夹 `m_nGoldMax`**。
    /// </summary>
    public void IncGameGold(uint nGameGold)
    {
        // 原文 3255：Int64Value := Int64(m_nGameGold) + nGameGold;
        long int64Value = (long)(uint)m_nGameGold + nGameGold;
        // 原文 3256：if Int64Value > High(LongWord) then
        if (int64Value > uint.MaxValue)
            m_nGameGold = unchecked((int)uint.MaxValue);   // 原文 3257：m_nGameGold := High(LongWord);
        else
            m_nGameGold = (int)(uint)int64Value;           // 原文 3259：m_nGameGold := Int64Value;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DecGameGold(nGameGold: LongWord);`（ObjPlayer.pas:3295-3303）。
    /// 不够时**夹到 0**（与 `DecGold` 的「原地不动」不同）。
    /// </summary>
    public void DecGameGold(uint nGameGold)
    {
        // 原文 3297：if m_nGameGold >= nGameGold then
        if ((uint)m_nGameGold >= nGameGold)
            m_nGameGold = (int)((uint)m_nGameGold - nGameGold);  // 原文 3299：Dec(m_nGameGold, nGameGold);
        else
            m_nGameGold = 0;                                     // 原文 3302：m_nGameGold := 0;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.GameGoldChanged();`（ObjPlayer.pas:2534-2537 声明于 :1169）。
    /// `SendUpdateMsg(Self, RM_GAMEGOLDCHANGED, 0, 0, 0, 0, '')`。
    /// </summary>
    public void GameGoldChanged()
    {
        // 原文 2536
        PlayerSurfaceMsgSeams.SendUpdateMsg(this, Grobal2Const.RM_GAMEGOLDCHANGED);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.NewGamePointChanged();`（ObjPlayer.pas:2539-2542）。
    /// 与元宝同族的「点数变更」通知 —— `TMerchant.UserSelect` 的元宝分支会用到。
    /// </summary>
    public void NewGamePointChanged()
    {
        // 原文 2541
        PlayerSurfaceMsgSeams.SendUpdateMsg(this, Grobal2Const.RM_GAMEPOINTCHANGED);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.GameGloryChanged();`（ObjPlayer.pas:2544-2547）。
    /// </summary>
    public void GameGloryChanged()
    {
        // 原文 2546
        PlayerSurfaceMsgSeams.SendUpdateMsg(this, Grobal2Const.RM_GAMEGLORY);
    }
}
