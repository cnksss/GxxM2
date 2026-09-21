// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF；implementation 起于 :1411）
// 本文件：**玩家会话配置下发与状态清理片（切片 Core4）**。
// 覆盖原文行号范围（实现段，行号取自 .p13scratch/_p13_joined.txt 的 implLine 列）：
//   8094  ClearStatusTime                          8101  ClearTimeLabel
//   8114  ClearAllDelayLabel                       8128  SendMapDescription
//   8144  GetMapCanRun                             8194  SendMapCanRun
//   8203  SendNotice                               8308  UserLogon
//   9133  GetBoxItems                              9214  SendGoldInfo
//   9229  SendNewGamePointInfo                     9241  SendGameGlory
//   9246  SendSpecialCmdList                       9292  SendEffectImageList
//   9338  SendMissionNPC                           9350  SendClientModules
//   9400  SendFilterItemList                       9432  SendItemDescList
//   9464  SendItemDescTopList                      9496  SendTzItemDescList
//   9527  SendCustomMonsterConfig                  9577  SendCustomMagicConfig
//   9627  SendCustomNpcConfig                      9677  SendDropItemEffectList
//   9727  SendEnabledAuctionItemList               9757  SendUnbindList
//   9769  SendInputBoxFilterList                   9784  SendStdItemList
//   9836  SendPlugClientList                       9879  SendCustomMoney
//   9914  SendClientBlackModules                   9939  SendCustomItemPropertyConfig
//   9957  SendCustomItemPropertyTextVarList        9975  SendArrButtonConfig
//   9992  SendLogon                                10014 SendServerConfig
//   10035 SendEnableClientUploadPickItems          10044 SendUseItems
//   10085 SendUseIcons                            10133 SendUseEffects
//   10181 SendUseMagic                            10240 ClientTakeOnItemsEx
// 末条方法 `ClientTakeOnItemsEx` 的 `end;` 在 10474，下一条 `ClientTakeOffItemsEx`
// 的 header 在 10476（即本切片上限 10476 是**下一条方法的起始行**，与
// `_p13_joined.txt` 的 endLine 列同口径）。
//
// 方法总数：**42**（与 `_p13_joined.txt` 过滤 implLine ∈ [8094, 10476] 的结果逐条一致）。
//
// 三数对账（本切片）：
//   真实体 8    ClearStatusTime / ClearTimeLabel / ClearAllDelayLabel / SendGoldInfo /
//               SendNewGamePointInfo / SendGameGlory / SendClientBlackModules /
//               SendArrButtonConfig
//   NotPorted 34 见各方法 XML doc 的「缺失成员」清单（逐条带原文行号）
//   原文如此 4    ClearAllDelayLabel（正向 Dispose 不摘链）/ SendNewGamePointInfo（游戏点
//               未下发）/ SendClientBlackModules（ClientCRC 形参完全未使用）/
//               SendArrButtonConfig（两分支报文面不对称且不用 *_CACHE）
//
// ⚠ 本切片**不重复声明**任何既有成员（逐条 grep 取证）：
//   · m_boOffLine / m_boDummyObject / m_btPermission / m_nGamePoint / m_nGameDiamond /
//     m_nGameGird / m_nGameGlory / m_nGameGold → Engine/ObjBase.OnlineMsg.cs
//   · m_sCharName / m_sUserID / m_sIPaddr / m_PEnvir → Engine/ObjBase.cs
//   · m_UseItems → Engine/RecalcChain.cs
//   · m_ActorIcons → Engine/PlayerSurface/TCreature.PlayerSurface.Base.cs
//   · M2Config.sGameGoldName/sGamePointName/sGameDiamondName/sGameGirdName/sCreditPointName
//     → Engine/M2Config.OnlineMsg.cs:47-51
//   · M2Config.g_ArrButtonConfig / g_ArrButtonConfigCRC → Engine/M2Config.ClientConf.cs:46/268
//   · ClientModuleState.g_BlackModuleList / TModuleInfo.sMD5 → Engine/ClientModuleList.cs:7/22
//   · TTimeLabel（nType / boDelete）→ Npc/ObjNpcTypes.cs:463-478
//   · TStringList（Add / Text）→ GXX.Core/Util/TStringList.cs:150/84
//   · EDcode.zEncodeString / zLibCompressString / zLibCompressBuffer → GXX.Core/Protocol/EDcode.cs
//   · ObjNpcConst.sSTRING_GOLDNAME → Npc/ObjNpcSeams.cs:79
// ============================================================================

using System;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Npc;

namespace GXX.M2Server.Engine;

/// <summary>
/// 切片 Core4 独占的常量与打包辅助（**不声明任何原文业务字段**）。
/// </summary>
public static class PlayerSurfaceCore4Const
{
    /// <summary>
    /// 原文 `MAX_STATUS_ATTR = 18;`（Grobal2.pas:36）——
    /// `TStatusTime = array [0 .. MAX_STATUS_ATTR - 1] of Word;`（Grobal2.pas:4118）。
    /// </summary>
    public const int MAX_STATUS_ATTR = 18;

    /// <summary>
    /// 原文 `sSTRING_GOLDNAME = '金币'`（M2Share.pas:210）—— 本切片**复用**既有
    /// <see cref="ObjNpcConst.sSTRING_GOLDNAME"/>（`Npc/ObjNpcSeams.cs:79`），**不另立一份**。
    /// </summary>
    public static string sSTRING_GOLDNAME => ObjNpcConst.sSTRING_GOLDNAME;

    /// <summary>
    /// 原文 `zEncodeString` 的返回值承载在 **AnsiString** 里（`sSendText := zEncodeString(sSendText);`
    /// ⇒ `sSendText` 变成"按字节承载的串"），随后原样交给 `SendSocket(@m_DefMsg, sSendText)`。
    /// 托管侧 <see cref="EDcode.zEncodeString(string)"/> 返回 `byte[]`（`EDcode.cs:402`），
    /// 而 <see cref="TPlayObject.SendSocketRef"/> 收 `string`（= 原文 AnsiString 的口径）——
    /// 故此处做 **1:1 的 AnsiString↔字节映射（Latin-1）**：
    /// 每字节 → 一个同值字符，与仓库既有策略一致
    /// （`GXX.Core/Launcher/LauncherParam.cs:248-249`、`Paradox/ParadoxDataSet.Seams.cs`）。
    /// **这不是"语义替身"**：原文 AnsiString 本来就是"字节序列 + 长度"，Latin-1 是它到
    /// 托管 `string` 的唯一无损映射。
    /// </summary>
    public static string AnsiStringFromBytes(byte[] bytes) => System.Text.Encoding.Latin1.GetString(bytes);

    /// <summary>
    /// 原文 `SendSocketEx(@m_DefMsg, @g_ArrButtonConfig, SizeOf(g_ArrButtonConfig));`
    /// （ObjPlayer.pas:9982）—— 把整块 `TArrButtonGroupConfig` 数组**按内存原样**送出。
    /// 托管侧 `M2Config.g_ArrButtonConfig`（`M2Config.ClientConf.cs:268`）是
    /// `ArrButtonGroupConfig[7]`（每元素 6 个 `int`，`ClientConf.cs:275-281`），
    /// `MemoryMarshal.AsBytes` 即 `SizeOf(...)` 的 1:1 对应物（7 × 24 = 168 字节）。
    /// </summary>
    public static byte[] BlitArrButtonConfig()
        => MemoryMarshal.AsBytes(new ReadOnlySpan<ArrButtonGroupConfig>(M2Config.g_ArrButtonConfig)).ToArray();
}

/// <summary>
/// `ObjPlayer.pas:8094-10476` 的 42 条 `TPlayObject` 方法。
/// </summary>
public partial class TPlayObject
{
    // ==================================================================
    // 本切片需要的字段（原文均属 `TPlayObject`/`TBaseObject`；grep 确认托管侧尚无同名成员）
    // ==================================================================

    /// <summary>
    /// 原文 `m_wStatusTimeArr: TStatusTime; // 0x60`（ObjBase.pas:143）
    /// = `array[0 .. MAX_STATUS_ATTR - 1] of Word`（Grobal2.pas:4118）。
    /// 本切片 `ClearStatusTime`（:8095）整块清零。
    /// </summary>
    public ushort[] m_wStatusTimeArr = new ushort[PlayerSurfaceCore4Const.MAX_STATUS_ATTR];

    /// <summary>
    /// 原文 `m_nStatusPowerTime: TStatusTime;`（ObjBase.pas:140）—— 与
    /// <see cref="m_wStatusTimeArr"/> 同型（都是 `TStatusTime`，故原文 :8096 用
    /// `SizeOf(TStatusTime)` 清零是**对的**）。
    /// </summary>
    public ushort[] m_nStatusPowerTime = new ushort[PlayerSurfaceCore4Const.MAX_STATUS_ATTR];

    /// <summary>
    /// 原文 `m_nStatusPower: array [0 .. MAX_STATUS_ATTR - 1] of Integer;`（ObjBase.pas:139）。
    /// ⚠ 与上面两个**不同型**（Integer 而非 Word），故原文 :8097 用的是
    /// `SizeOf(m_nStatusPower)`（变量，72 字节）而不是 `SizeOf(TStatusTime)`（36 字节）——
    /// 逐字保留这一处"类型/变量混用"的写法差异。
    /// </summary>
    public int[] m_nStatusPower = new int[PlayerSurfaceCore4Const.MAX_STATUS_ATTR];

    /// <summary>
    /// 原文 `m_TimeLabelList: TList;`（ObjPlayer.pas:261，:1675 处 `TList.Create`）。
    /// 元素类型：原文是 `pTTimeLabel`（`TTimeLabel` 记录指针，ObjNpc.pas:247-258）；
    /// 托管侧 `TTimeLabel` 是**引用类型**（`Npc/ObjNpcTypes.cs:463`），故
    /// `Items[I]` 的"就地改写"语义（原文靠指针别名）在托管侧天然成立。
    /// </summary>
    public readonly System.Collections.Generic.List<TTimeLabel> m_TimeLabelList = new();

    // ==================================================================
    // ClearStatusTime（原文 8094-8101）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.ClearStatusTime();`（`ObjPlayer.pas:8093` 声明头，
    /// 实现 `:8094-8099`）。
    /// <code>
    ///   FillChar(m_wStatusTimeArr,   SizeOf(TStatusTime),    #0);   // 8095
    ///   FillChar(m_nStatusPowerTime, SizeOf(TStatusTime),    #0);   // 8096
    ///   FillChar(m_nStatusPower,     SizeOf(m_nStatusPower), #0);   // 8097
    ///   m_nCharStatus := GetCharStatus;                             // 8098
    /// </code>
    /// ★ 逐字保留的原文细节：第 1、2 行用**类型** `TStatusTime` 作长度（两者确实同型），
    /// 第 3 行却用**变量** `m_nStatusPower` —— 这不是笔误（`m_nStatusPower` 是 Integer 数组，
    /// 长度恰为 2 倍），托管侧按各自数组长度清零，语义等价。
    /// 接缝：`GetCharStatus` → <see cref="PlayerSurfaceBaseSeams.GetCharStatus"/>。
    /// </summary>
    public void ClearStatusTime()
    {
        // 原文 8094-8099
        // 原文 8095：FillChar(m_wStatusTimeArr, SizeOf(TStatusTime), #0);
        Array.Clear(m_wStatusTimeArr, 0, m_wStatusTimeArr.Length);
        // 原文 8096：FillChar(m_nStatusPowerTime, SizeOf(TStatusTime), #0);
        Array.Clear(m_nStatusPowerTime, 0, m_nStatusPowerTime.Length);
        // 原文 8097：FillChar(m_nStatusPower, SizeOf(m_nStatusPower), #0);
        Array.Clear(m_nStatusPower, 0, m_nStatusPower.Length);
        // 原文 8098：m_nCharStatus := GetCharStatus;
        //   原文此处**不写括号**（Delphi 无参函数调用可省括号），逐字保留该写法差异。
        m_nCharStatus = PlayerSurfaceBaseSeams.GetCharStatus(this);
    }

    // ==================================================================
    // ClearTimeLabel（原文 8101-8114）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.ClearTimeLabel(nType: Integer);`
    /// （`ObjPlayer.pas:8101` 声明头，实现 `:8102-8112`）。
    /// <code>
    ///   for I := m_TimeLabelList.Count - 1 downto 0 do      // 8106
    ///   begin
    ///     TimeLabel := m_TimeLabelList.Items[I];            // 8108
    ///     if (TimeLabel.nType = nType) then                 // 8109
    ///       TimeLabel.boDelete := True;                     // 8110
    ///   end;
    /// </code>
    /// ★ 语义要点（逐字保留）：**倒序**遍历，命中只**置 `boDelete` 标记、不从列表摘除**
    /// （真正摘除在心跳 :4155/:4167/:4194/:4244）。**不是**"删除"。
    /// </summary>
    public void ClearTimeLabel(int nType)
    {
        // 原文 8106：for I := m_TimeLabelList.Count - 1 downto 0 do
        for (int i = m_TimeLabelList.Count - 1; i >= 0; i--)
        {
            // 原文 8108：TimeLabel := m_TimeLabelList.Items[I];
            TTimeLabel timeLabel = m_TimeLabelList[i];
            // 原文 8109-8110：if (TimeLabel.nType = nType) then TimeLabel.boDelete := True;
            if (timeLabel.nType == nType)
                timeLabel.boDelete = true;
        }
    }

    // ==================================================================
    // ClearAllDelayLabel（原文 8114-8128）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.ClearAllDelayLabel;`（`ObjPlayer.pas:8114` 声明头，
    /// 实现 `:8115-8125`）。
    /// <code>
    ///   for I := 0 to m_TimeLabelList.Count - 1 do   // 8119
    ///   begin
    ///     TimeLabel := m_TimeLabelList.Items[I];     // 8121
    ///     Dispose(TimeLabel);                        // 8122
    ///   end;
    ///   m_TimeLabelList.Clear;                       // 8124
    /// </code>
    /// ★ 原文如此 / ★ 原文缺陷（原文 8119-8122）：与本族其它删除点的**倒序 + 先摘后放**
    /// （`:4155`、`:4167`、`:4194`、`:4244`、`:6528`）不同，这里**正向遍历、只 Dispose
    /// 不摘链**，直到 :8124 才 `Clear` —— 中间每一步列表里都是**已释放的悬垂指针**。
    /// 托管侧 `TTimeLabel` 是引用类型，`Dispose` 无对应动作（GC 接管），悬垂态**不可观测**，
    /// 故此处以注释留痕、不伪造"托管 Dispose"（锁定用例见
    /// `ObjPlayerCore4Tests.ClearAllDelayLabel_ForwardDisposeWithoutUnlink_OriginalDefect`）。
    /// </summary>
    public void ClearAllDelayLabel()
    {
        // 原文 8119：for I := 0 to m_TimeLabelList.Count - 1 do
        for (int i = 0; i <= m_TimeLabelList.Count - 1; i++)
        {
            // 原文 8121：TimeLabel := m_TimeLabelList.Items[I];
            TTimeLabel timeLabel = m_TimeLabelList[i];
            // 原文 8122：Dispose(TimeLabel);
            //   ★ 原文缺陷 / 原文如此：**只释放、不从列表摘除**（悬垂指针留在表内至 :8124）。
            //   托管侧 TTimeLabel 是引用类型（Npc/ObjNpcTypes.cs:463），无 Dispose 语义，
            //   故这一行**没有可执行的托管对应物**；这里刻意不写任何"近似释放"，
            //   仅保留原文的"遍历—后置 Clear"结构。
            _ = timeLabel;
        }
        // 原文 8124：m_TimeLabelList.Clear;
        m_TimeLabelList.Clear();
    }

    // ==================================================================
    // SendGoldInfo（原文 9214-9228）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendGoldInfo(boSendName: Boolean);`
    /// （`ObjPlayer.pas:9213` 声明头，实现 `:9214-9226`）。
    /// <code>
    ///   // if m_nSoftVersionDateEx = 0 then Exit;      // 9217 —— 原文注释掉
    ///   if boSendName then
    ///     sMsg := g_Config.sGameGoldName + #13 + g_Config.sGamePointName + #13 + sSTRING_GOLDNAME
    ///           + #13 + g_Config.sGameDiamondName + #13 + g_Config.sGameGirdName   // 9220-9221
    ///   else
    ///     sMsg := '';                                                             // 9224
    ///   SendDefMessage(SM_GAMEGOLDNAME, m_nGameGold, LoWord(m_nGamePoint),
    ///                  HiWord(m_nGamePoint), 0, sMsg);                            // 9225
    /// </code>
    /// ★ 分隔符是 **`#13`（单字节 CR）**，**不是** `sLineBreak` —— 逐字保留。
    /// 原文 9217 那行 `if m_nSoftVersionDateEx = 0 then Exit;` 被注释掉，本实现同样**不**加该早退。
    /// </summary>
    public void SendGoldInfo(bool boSendName)
    {
        // 原文 9214-9226
        // 原文 9216：var sMsg: string;
        string sMsg;
        // 原文 9217：// if m_nSoftVersionDateEx = 0 then Exit;    （原文注释，保留为注释）
        // 原文 9218-9222
        if (boSendName)
        {
            // 原文 9220-9221：五个名称用 #13 连接（**不是** sLineBreak）
            sMsg = M2Config.sGameGoldName + '\r' + M2Config.sGamePointName + '\r'
                 + PlayerSurfaceCore4Const.sSTRING_GOLDNAME + '\r'
                 + M2Config.sGameDiamondName + '\r' + M2Config.sGameGirdName;
        }
        else
        {
            // 原文 9224：sMsg := '';
            sMsg = "";
        }

        // 原文 9225：SendDefMessage(SM_GAMEGOLDNAME, m_nGameGold, LoWord(m_nGamePoint), HiWord(m_nGamePoint), 0, sMsg);
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_GAMEGOLDNAME, m_nGameGold,
            PlayerSurfacePack.LoWord(m_nGamePoint), PlayerSurfacePack.HiWord(m_nGamePoint), 0, sMsg);
    }

    // ==================================================================
    // SendNewGamePointInfo（原文 9229-9240）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendNewGamePointInfo(boSendName: Boolean);`
    /// （`ObjPlayer.pas:9228` 声明头，实现 `:9229-9238`）。
    /// <code>
    ///   if boSendName then
    ///     sSendMsg := g_Config.sGameDiamondName + #13 + g_Config.sGameGirdName
    ///               + #13 + g_Config.sCreditPointName        // 9234
    ///   else
    ///     sSendMsg := '';                                    // 9236
    ///   SendDefMessage(SM_GAMEPOINTNAME, m_nGameDiamond, LoWord(m_nGameGird),
    ///                  HiWord(m_nGameGird), 0, sSendMsg);    // 9237
    /// </code>
    /// ★★ **原文缺陷（原文 9237，逐字保留、不修正）**：本条报文标识是
    /// `SM_GAMEPOINTNAME`（**游戏点**名称），但
    /// ① nRecog 传的是 `m_nGameDiamond`（**金刚石**），
    /// ② wParam/wTag 拆的是 `m_nGameGird`（**灵符**），
    /// ③ 名称串只有 3 项（`sGameDiamondName`/`sGameGirdName`/`sCreditPointName`），
    /// —— **`m_nGamePoint`（游戏点）的值在全方法里根本没有出现**。
    /// 换句话说：游戏点的数值**永远不会被下发**。这是原文的真实行为，锁定用例
    /// `ObjPlayerCore4Tests.SendNewGamePointInfo_GamePointValueIsNeverSent_OriginalDefect`。
    /// </summary>
    public void SendNewGamePointInfo(bool boSendName)
    {
        // 原文 9229-9238
        // 原文 9230：var sSendMsg: AnsiString;
        string sSendMsg;
        // 原文 9233-9236
        if (boSendName)
        {
            // 原文 9234：三名（金刚石 / 灵符 / 声望）用 #13 连接
            sSendMsg = M2Config.sGameDiamondName + '\r' + M2Config.sGameGirdName + '\r'
                     + M2Config.sCreditPointName;
        }
        else
        {
            // 原文 9236：sSendMsg := '';
            sSendMsg = "";
        }

        // 原文 9237：SendDefMessage(SM_GAMEPOINTNAME, m_nGameDiamond, LoWord(m_nGameGird), HiWord(m_nGameGird), 0, sSendMsg);
        //   ★ 原文缺陷（原文如此）：nRecog 用 m_nGameDiamond、Param/Tag 拆 m_nGameGird，
        //     **m_nGamePoint 从未被读取** —— 逐字照抄，不"顺手修成" m_nGamePoint。
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_GAMEPOINTNAME, m_nGameDiamond,
            PlayerSurfacePack.LoWord(m_nGameGird), PlayerSurfacePack.HiWord(m_nGameGird), 0, sSendMsg);
    }

    // ==================================================================
    // SendGameGlory（原文 9241-9245）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendGameGlory;`（`ObjPlayer.pas:9240` 声明头，
    /// 实现 `:9241-9243`）：`SendDefMessage(SM_GAMEGLORY, m_nGameGlory, 0, 0, 0, '');`
    /// —— **无早退、无条件、无分支**（注意它**不判** `m_boOffLine`/`m_boDummyObject`，
    /// 与同族的 `Send*` 不同），逐字保留。
    /// </summary>
    public void SendGameGlory()
    {
        // 原文 9241-9243
        // 原文 9242：SendDefMessage(SM_GAMEGLORY, m_nGameGlory, 0, 0, 0, '');
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_GAMEGLORY, m_nGameGlory, 0, 0, 0, "");
    }

    // ==================================================================
    // SendClientBlackModules（原文 9914-9938）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendClientBlackModules(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （`ObjPlayer.pas:9913` 声明头，实现 `:9914-9936`）。
    /// <code>
    ///   if m_boOffLine or m_boDummyObject then Exit;                  // 9920-9921
    ///   sSendText := '';                                              // 9923
    ///   ModuleList := TStringList.Create;                             // 9924
    ///   for I := 0 to g_BlackModuleList.Count - 1 do                  // 9925
    ///   begin
    ///     ModuleInfo := g_BlackModuleList.Items[I];                   // 9927
    ///     ModuleList.Add(ModuleInfo.sMD5);                            // 9928
    ///   end;
    ///   sSendText := ModuleList.Text;                                 // 9931
    ///   ModuleList.Free;                                              // 9932
    ///   sSendText := zEncodeString(sSendText);                        // 9933
    ///   m_DefMsg := MakeDefaultMsg(SM_BLACKMODULEMD5, 0, 0, 0, ShowProgress);  // 9934
    ///   SendSocket(@m_DefMsg, sSendText);                             // 9935
    /// </code>
    /// ★★ **原文缺陷（原文 9913 的形参表 / 全方法体，逐字保留）**：形参
    /// **`ClientCRC` 在整个方法体里一次都没有被读取** ——
    /// 同族的 `SendSpecialCmdList`（:9269）、`SendEffectImageList`（:9315）、`SendModuleMD5`
    /// 都有 `g_XxxCRC &lt;&gt; ClientCRC` 短路，**只有本方法没有** ⇒ 客户端每次请求都**全量重发**
    /// 黑名单。锁定用例
    /// `ObjPlayerCore4Tests.SendClientBlackModules_ClientCrcIsIgnored_OriginalDefect`。
    /// 注意 `ShowProgress` **是**被用到的（走到 `MakeDefaultMsg` 的 wSeries）。
    /// </summary>
    /// <param name="ShowProgress">原文 `ShowProgress: Word = 0`（原样透传给 `MakeDefaultMsg` 的 wSeries）。</param>
    /// <param name="ClientCRC">原文 `ClientCRC: LongWord = 0`（★ 原文缺陷：全方法体未使用）。</param>
    public void SendClientBlackModules(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        // 原文 9914-9936
        // 原文 9920-9921：if m_boOffLine or m_boDummyObject then Exit;
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 9923：sSendText := '';
        string sSendText = "";

        // 原文 9924：ModuleList := TStringList.Create;
        var moduleList = new TStringList();
        // 原文 9925-9929：for I := 0 to g_BlackModuleList.Count - 1 do ... ModuleList.Add(ModuleInfo.sMD5);
        for (int i = 0; i <= ClientModuleState.g_BlackModuleList.Count - 1; i++)
        {
            // 原文 9927：ModuleInfo := g_BlackModuleList.Items[I];
            TModuleInfo moduleInfo = ClientModuleState.g_BlackModuleList[i];
            // 原文 9928：ModuleList.Add(ModuleInfo.sMD5);
            moduleList.Add(moduleInfo.sMD5);
        }

        // 原文 9931：sSendText := ModuleList.Text;
        sSendText = moduleList.Text;
        // 原文 9932：ModuleList.Free;
        //   托管侧 TStringList 由 GC 释放；原文 Free 之后不再触碰 moduleList，故无可见差异。
        // 原文 9933：sSendText := zEncodeString(sSendText);
        sSendText = PlayerSurfaceCore4Const.AnsiStringFromBytes(EDcode.zEncodeString(sSendText));

        // 原文 9934：m_DefMsg := MakeDefaultMsg(SM_BLACKMODULEMD5, 0, 0, 0, ShowProgress);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_BLACKMODULEMD5, 0, 0, 0, ShowProgress);
        // 原文 9935：SendSocket(@m_DefMsg, sSendText);
        SendSocketRef(m_DefMsg, sSendText);
        // ★ 原文缺陷：形参 ClientCRC 到此处仍未（也从未）被读取 —— 见上方 XML doc。
        _ = ClientCRC;
    }

    // ==================================================================
    // SendArrButtonConfig（原文 9975-9991）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendArrButtonConfig(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （`ObjPlayer.pas:9974` 声明头，实现 `:9975-9988`）。
    /// <code>
    ///   if m_boOffLine or m_boDummyObject then Exit;                            // 9976-9977
    ///   if g_ArrButtonConfigCRC &lt;&gt; ClientCRC then                              // 9979
    ///   begin
    ///     m_DefMsg := MakeDefaultMsg(SM_ARR_BUTTON_CONFIG, 0, 0, 0, ShowProgress);  // 9981
    ///     SendSocketEx(@m_DefMsg, @g_ArrButtonConfig, SizeOf(g_ArrButtonConfig));   // 9982
    ///   end
    ///   else
    ///   begin
    ///     m_DefMsg := MakeDefaultMsg(SM_ARR_BUTTON_CONFIG, 0, 0, 0, ShowProgress);  // 9986
    ///     SendSocket(@m_DefMsg, '');                                                // 9987
    ///   end;
    /// </code>
    /// ★ 原文如此 / ★ 原文缺陷（原文 9981-9987，逐字保留）：
    /// ① 两个分支**都用** `SM_ARR_BUTTON_CONFIG`，**没有** `SM_ARR_BUTTON_CONFIG_CACHE`
    ///    这种"缓存命中"标识（同族的 `SendModuleMD5`/`SendStdItemList`/`SendServerConfig`
    ///    都有 `*_CACHE`）—— 客户端无法从标识区分"这是缓存命中、包体为空"；
    /// ② 两个分支的**报文面不对称**：命中分支走 `SendSocket(..., '')`（**不带** 168 字节配置块），
    ///    未命中分支走 `SendSocketEx(..., @g_ArrButtonConfig, SizeOf(...))`。
    /// 锁定用例 `ObjPlayerCore4Tests.SendArrButtonConfig_BranchesAreAsymmetric_OriginalDefect`。
    /// </summary>
    public void SendArrButtonConfig(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        // 原文 9975-9988
        // 原文 9976-9977：if m_boOffLine or m_boDummyObject then Exit;
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 9979：if g_ArrButtonConfigCRC <> ClientCRC then
        if (M2Config.g_ArrButtonConfigCRC != ClientCRC)
        {
            // 原文 9981：m_DefMsg := MakeDefaultMsg(SM_ARR_BUTTON_CONFIG, 0, 0, 0, ShowProgress);
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ARR_BUTTON_CONFIG, 0, 0, 0, ShowProgress);
            // 原文 9982：SendSocketEx(@m_DefMsg, @g_ArrButtonConfig, SizeOf(g_ArrButtonConfig));
            SendSocketExRef(m_DefMsg, PlayerSurfaceCore4Const.BlitArrButtonConfig());
        }
        else
        {
            // 原文 9986：m_DefMsg := MakeDefaultMsg(SM_ARR_BUTTON_CONFIG, 0, 0, 0, ShowProgress);
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ARR_BUTTON_CONFIG, 0, 0, 0, ShowProgress);
            // 原文 9987：SendSocket(@m_DefMsg, '');
            //   ★ 原文缺陷：命中分支**不送配置块**，且用的是与 9982 不同的发送面。
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 以下 34 条：依赖尚未移植的其它单元的成员 → 显式留痕（台账 §48.1，禁止裸桩）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendMapDescription();`（ObjPlayer.pas:8128）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：
    /// <c>TEnvirnoment.m_boMUSIC</c>（:8132）、<c>TEnvirnoment.m_sMusicFileName</c>（:8133）、
    /// <c>TEnvirnoment.m_boNight</c>（:8138）、<c>TEnvirnoment.m_nSecretFlag</c>（:8139）、
    /// <c>TEnvirnoment.m_nSecretFlag2</c>（:8139）、
    /// 以及常量 <c>SecretFlag_HumAndHeroPercentHP</c>。
    /// 托管 <c>Engine/Envir.cs:80</c> 的 <c>TEnvirnoment</c> 只有
    /// <c>sMapName/sMapDesc/nWidth/nHeight/nServerIndex/boMainMap/MapCellArray/MapData/DoorList/QuestNpcList</c>，
    /// 且**不是 partial**，本车道无法在其上补字段。</para>
    /// </summary>
    public void SendMapDescription()
    {
        PortNotPorted(nameof(SendMapDescription), 8128);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.GetMapCanRun(var boRunHuman, boRunMon, boRunNpc, boRunGuard: Boolean);`
    /// （ObjPlayer.pas:8144）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：
    /// <c>TEnvirnoment.m_boNoRunHuman</c>（:8169）、<c>m_boNoRunMon</c>（:8170）、
    /// <c>m_boRUNHUMAN</c>（:8186）、<c>m_boRUNMON</c>（:8188）（托管 <c>Envir.cs</c> 均无）；
    /// <c>TBaseObject.InSafeZone</c>（:8164，ObjBase.pas:35612/35657 两个重载未移植）；
    /// <c>g_CastleManager.InCastleWarArea(Self)</c>（:8156，托管 <c>CastleState.cs:9</c> 只有
    /// <c>InCastleWarArea(envir,nX,nY)</c> 三参重载，**没有**以 <c>TBaseObject</c> 为参的版本）；
    /// <c>g_Config.boSafeAreaDisNpcRun</c>（:8173）。</para>
    /// </summary>
    public void GetMapCanRun(ref bool boRunHuman, ref bool boRunMon, ref bool boRunNpc, ref bool boRunGuard)
    {
        PortNotPorted(nameof(GetMapCanRun), 8144);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendMapCanRun;`（ObjPlayer.pas:8194）。
    /// <para><b>未移植</b>。缺失成员：<c>GetMapCanRun</c>（本片同批留痕，原文 :8198）；
    /// 其依赖面见 <see cref="GetMapCanRun"/>。</para>
    /// </summary>
    public void SendMapCanRun()
    {
        PortNotPorted(nameof(SendMapCanRun), 8194);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendNotice(IsRealSendNotice: Boolean);`（ObjPlayer.pas:8203，105 行）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：
    /// <c>NoticeManager</c>（:8215，托管侧 <c>Sweep9/Forms/NoticeM/TNoticeManager.cs:254</c>
    /// 的静态字段可空且未接线）、<c>WideReplaceText</c>（:8220-8221，全仓无同名实现）、
    /// <c>sLineBreak</c>（:8222）、<c>FRecbNoticeCode</c>（:8293）、<c>FIsSendRunNotice</c>（:8297）。
    /// 其中 <c>zLibCompressString</c>（:8225）已存在（<c>EDcode.cs:511</c>），但不足以补齐本方法。</para>
    /// </summary>
    public void SendNotice(bool IsRealSendNotice)
    {
        PortNotPorted(nameof(SendNotice), 8203);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.UserLogon(); virtual;`（声明 ObjPlayer.pas:1219 一带，
    /// 实现 :8308-9132，**825 行**）。
    /// <para><b>未移植（显式留痕）</b>。原文是登录总入口（Code 1..74 的 try..except 大流程），
    /// 阻塞面（逐段带原文行号）：<c>g_Config.boTestServer</c>（:8336）、
    /// <c>m_Abil.Level</c>（:8338，<c>m_Abil</c> 未切出）、<c>Initialize</c>/<c>SendMissionNPC</c>
    /// （:8353/:8355）、<c>UserEngine.GetHumPermission</c>（:8367）、<c>GetStartPoint</c>（:8369）、
    /// <c>m_MagicList</c> 的 <c>sub_4C713C</c>（:8382）、<c>UserEngine.CopyToUserItemFromName</c>
    /// （:8391 等 4 处）、<c>m_ItemList</c> 限时物品/复制品清扫（:8449-8621）、
    /// <c>g_ItemRules</c>（:8500/:8594/:8629/:8681）、<c>m_StorageItemList</c>/<c>m_BigStorageItemList</c>
    /// （:8624/:8678）、<c>g_M2DataDB.StorageDB</c>（:8688）、<c>AddGameDataLog</c>（:8729）、
    /// <c>g_PluginManager.HookPlayerLogin1..4</c>（:8331/:8764/:8986/:9089）、
    /// <c>g_Config.AttatckModes[]</c>（:8779/:8791）、<c>RecalcLevelAbilitys</c>/<c>GetLevelExp</c>
    /// （:8756/:8758）、<c>g_GuildManager</c>（:8899）、<c>g_NationManage</c>（:8907）、
    /// <c>g_MapManager</c>（:9056）、<c>EnterAnotherMap</c>（:9059）、<c>SearchViewRange</c>（:9081）、
    /// <c>g_PayInfoList</c>/<c>SavePayInfo</c>（:9105/:9121）、<c>FrmIDSoc</c>（:9126）、
    /// <c>g_M2DataDB.UserShopDB</c>（:9127）、<c>CheckDenyLogon</c>/<c>CheckMarry</c>/<c>CheckMaster</c>
    /// （:8976/:9001/:9002）等。按任务书第 4 条，**不**臆造替身。</para>
    /// <para>★ 保留原文的 <c>virtual</c>（原文 :1219 一带声明为 `virtual`）。</para>
    /// </summary>
    public virtual void UserLogon()
    {
        PortNotPorted(nameof(UserLogon), 8308);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.GetBoxItems;`（ObjPlayer.pas:9133，含内嵌 `GetNewName`）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_BoxsList.GetBoxsItem</c>（:9179/:9194）、
    /// <c>m_BoxConfig</c>（<c>TBoxServerConfig</c>，ObjPlayer.pas:365，托管侧无此类型）、
    /// <c>TClientItem</c> 的可写缓冲视图（:9161）、<c>g_Config.sCreditPointName</c>/
    /// <c>sGameDiamondName</c>（:9142/:9144，托管已有）与 <c>ClientItem.S.StdMode</c>（:9138）。</para>
    /// </summary>
    public void GetBoxItems()
    {
        PortNotPorted(nameof(GetBoxItems), 9133);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendSpecialCmdList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9246）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_Config.DBotFuncs</c>（:9252/:9269）、
    /// <c>g_GateArr</c> 与 <c>TGateInfo.sSpecialCmdCRC</c>（:9254-9255，**全仓无 g_GateArr**）、
    /// <c>g_SpecialCmdListText</c>（:9257）、<c>g_SpecialCmdList</c>（:9257）、
    /// <c>g_SpecialCmdListTextCRC</c>（:9255）。</para>
    /// </summary>
    public void SendSpecialCmdList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendSpecialCmdList), 9246);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendEffectImageList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9292）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sEffectImageListCRC</c>
    /// （:9301-9302）、<c>g_EffectImageListText</c>（:9304）、<c>g_EffectImageListTextCRC</c>（:9302）、
    /// <c>m_nOffOnlineTick</c>（:9296，本片未声明，见 <see cref="SendClientBlackModules"/> 同族早退）。</para>
    /// </summary>
    public void SendEffectImageList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendEffectImageList), 9292);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendMissionNPC;`（ObjPlayer.pas:9338）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_MissionNPC</c>（:9345）、
    /// <c>g_sSendPageCaptionText</c>（:9346）、<c>m_nOffOnlineTick</c>（:9342）。</para>
    /// </summary>
    public void SendMissionNPC()
    {
        PortNotPorted(nameof(SendMissionNPC), 9338);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendClientModules(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9350）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sModuleCRC</c>
    /// （:9359-9360）。注：<c>g_ModuleListText</c>/<c>g_ModuleListTextLen</c>/<c>g_ModuleListTextCRC</c>
    /// （:9362/:9370/:9375）**已存在**（<c>Engine/ClientModuleList.cs:24-26</c>），
    /// <c>g_Config.boClientCheckModule</c>/<c>boClientAddModule</c> 也已存在
    /// （<c>Engine/M2Config.ServerValue.cs:48-49</c>）—— 唯一阻塞项是 <c>g_GateArr</c>。</para>
    /// </summary>
    public void SendClientModules(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendClientModules), 9350);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendFilterItemList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9400）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_Config.boSendFilterItemList</c>（:9404）、
    /// <c>SendDelayMsg</c>（:9405，ObjBase.pas）、<c>g_NameFilterListText</c>（:9414）、
    /// <c>g_NameFilterListTextCRC</c>（:9409）、<c>g_GateArr</c>/<c>TGateInfo.sFilterItemListCRC</c>（:9411-9412）。</para>
    /// </summary>
    public void SendFilterItemList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendFilterItemList), 9400);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendItemDescList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9432）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_Config.boSendItemDescList</c>（:9436）、
    /// <c>SendDelayMsg</c>（:9437）、<c>g_ItemDescListText</c>（:9446）、
    /// <c>g_ItemDescListTextCRC</c>（:9441）、<c>g_GateArr</c>/<c>TGateInfo.sItemDescListCRC</c>（:9443-9444）。</para>
    /// </summary>
    public void SendItemDescList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendItemDescList), 9432);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendItemDescTopList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9464）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_Config.boSendItemDescTopList</c>（:9468）、
    /// <c>SendDelayMsg</c>（:9469）、<c>g_ItemDescTopListText</c>（:9478）、
    /// <c>g_ItemDescTopListTextCRC</c>（:9473）、<c>g_GateArr</c>/<c>TGateInfo.sItemDescTopListCRC</c>（:9475-9476）。</para>
    /// </summary>
    public void SendItemDescTopList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendItemDescTopList), 9464);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendTzItemDescList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9496）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_Config.boSendTzItemDescList</c>（:9499）、
    /// <c>SendDelayMsg</c>（:9500）、<c>g_TzItemDescListText</c>（:9509）、
    /// <c>g_TzItemDescListTextCRC</c>（:9504）、<c>g_GateArr</c>/<c>TGateInfo.sTZItemDescListCRC</c>（:9506-9507）。</para>
    /// </summary>
    public void SendTzItemDescList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendTzItemDescList), 9496);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendCustomMonsterConfig(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9527）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sCustomMonsterConfigCRC</c>
    /// （:9536-9537）、<c>g_CustomMonsterListText</c>（:9539）、
    /// <c>g_CustomMonsterListTextLen</c>（:9540）、<c>g_CustomMonsterListTextCRC</c>（:9537）、
    /// <c>g_Config.boSendCustomMonsterConfig</c>（:9552）。</para>
    /// </summary>
    public void SendCustomMonsterConfig(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendCustomMonsterConfig), 9527);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendCustomMagicConfig(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9577）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sCustomMagicConfigCRC</c>
    /// （:9586-9587）、<c>g_Config.boSendCustomMagicConfig</c>（:9602）。注：
    /// <c>g_CustomMagicListText</c>/<c>g_CustomMagicListTextLen</c>/<c>g_CustomMagicListTextCRC</c>
    /// （:9589-9595）**已存在**（<c>Forms/CustomMagic/CustomMagicSeams.cs:474-479</c>），
    /// 唯一阻塞项仍是 <c>g_GateArr</c>。</para>
    /// </summary>
    public void SendCustomMagicConfig(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendCustomMagicConfig), 9577);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendCustomNpcConfig(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9627）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sCustomNpcConfigCRC</c>
    /// （:9636-9637）、<c>g_CustomNpcListText</c>（:9639）、<c>g_CustomNpcListTextLen</c>（:9640）、
    /// <c>g_CustomNpcListTextCRC</c>（:9637）、<c>g_Config.boSendCustomNpcConfig</c>（:9652）。</para>
    /// </summary>
    public void SendCustomNpcConfig(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendCustomNpcConfig), 9627);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendDropItemEffectList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9677）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sDropItemEffectListCRC</c>
    /// （:9686-9687）、<c>g_DropItemEffectListText</c>（:9689）、
    /// <c>g_DropItemEffectListTextLen</c>（:9690）、<c>g_DropItemEffectListTextCRC</c>（:9687）。</para>
    /// </summary>
    public void SendDropItemEffectList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendDropItemEffectList), 9677);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendEnabledAuctionItemList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9727）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_EnabledAuctionItemListText</c>（:9736）、
    /// <c>g_EnabledAuctionItemListTextLen</c>（:9737）、<c>g_EnabledAuctionItemListTextCRC</c>（:9742）。
    /// ★ 注意本方法**不依赖** <c>g_GateArr</c>（两个分支都没有网关缓存判定），
    /// 但三个文本/CRC 全局在托管侧**均不存在**，故仍只能留痕。</para>
    /// </summary>
    public void SendEnabledAuctionItemList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendEnabledAuctionItemList), 9727);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUnbindList();`（ObjPlayer.pas:9757）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_UnBindItemListText</c>（:9764）、
    /// <c>g_BindItemTypeList</c>（:9764）。</para>
    /// </summary>
    public void SendUnbindList()
    {
        PortNotPorted(nameof(SendUnbindList), 9757);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendInputBoxFilterList();`（ObjPlayer.pas:9769）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_InputBoxFilterList</c> 的
    /// **<c>.Text</c> / <c>.Count</c> 读取面**（:9778-9779）。托管侧
    /// <c>Npc/ObjNpcInputSeams.cs:42</c> 只提供 <c>GetInputBoxFilterList(): object?</c>
    /// （**专为判 nil**，无法取 Text/Count），故不能凭该接缝取值。</para>
    /// </summary>
    public void SendInputBoxFilterList()
    {
        PortNotPorted(nameof(SendInputBoxFilterList), 9769);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendStdItemList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9784）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sStdItemListCRC</c>
    /// （:9790-9791）、<c>g_StdItemListText</c>（:9793）、<c>g_StdItemListTextLen</c>（:9794）、
    /// <c>g_StdItemListTextCRC</c>（:9791）、<c>m_dwStdItemListTextCRC</c>（:9796，本片未声明）。</para>
    /// </summary>
    public void SendStdItemList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendStdItemList), 9784);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendPlugClientList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9836）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sPlugFileCRC</c>
    /// （:9842-9843）。注：<c>g_PlugFileMD5ListText</c>/<c>g_PlugFileMD5ListTextLen</c>/
    /// <c>g_PlugFileMD5ListTextCRC</c>（:9845/:9850/:9856）**已存在**
    /// （<c>Sweep9/Forms/uFrmClientPlugManager/TFrmClientPlugManager.cs:82-101</c>），
    /// 唯一阻塞项是 <c>g_GateArr</c>。</para>
    /// </summary>
    public void SendPlugClientList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendPlugClientList), 9836);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendCustomMoney(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9879）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_CustomMoneyList</c>（:9889）、
    /// <c>pTCustomMoney</c>/<c>TCustomMoney</c>（:9891-9892）、
    /// <c>TClientCustomMoney</c> 记录与 <c>zEncodeBuffer(@ClientCustomMoney, SizeOf(...))</c>（:9898）。
    /// 注：<c>EDcode.zEncodeBuffer</c>（<c>EDcode.cs:406</c>）**已存在**，缺的是记录类型本身。</para>
    /// </summary>
    public void SendCustomMoney(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendCustomMoney), 9879);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendCustomItemPropertyConfig(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9939）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_CustomItemPropertyText</c>（:9945）、
    /// <c>g_CustomItemPropertyChecks</c> 的**可用实例**（:9945）。注：<c>g_CustomItemPropertyCRC</c>
    /// （:9943）**已存在**（<c>Forms/ItemProperty/CustomItemPropertySeams.cs:243</c>），
    /// 但同文件的 <c>g_CustomItemPropertyChecks</c> 是**未接线即抛异常**的属性
    /// （<c>CustomItemPropertySeams.cs:215-219</c>），故不能当作可用数据源。</para>
    /// </summary>
    public void SendCustomItemPropertyConfig(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendCustomItemPropertyConfig), 9939);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendCustomItemPropertyTextVarList(ShowProgress: Word = 0; ClientCRC: LongWord = 0);`
    /// （ObjPlayer.pas:9957）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_CustomItemPropertyTextVarListText</c>（:9963）、
    /// <c>g_CustomItemPropertyTextVarListTextLen</c>（:9964）。注：<c>...TextVarListTextCRC</c>（:9961）
    /// **已存在**（<c>CustomItemPropertySeams.cs:246</c>）。</para>
    /// </summary>
    public void SendCustomItemPropertyTextVarList(ushort ShowProgress = 0, uint ClientCRC = 0)
    {
        PortNotPorted(nameof(SendCustomItemPropertyTextVarList), 9957);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendLogon();`（ObjPlayer.pas:9992）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>GetFeatureToLong(@Feature)</c>（:9999，
    /// ObjBase.pas 的全局函数，未移植）、<c>TMessageBodyWL</c> 记录与
    /// <c>Move(MessageBodyWL, S[1], SizeOf(TMessageBodyWL))</c> 的字节布局（:9993/:10008-10009）、
    /// <c>m_nLight</c>（:9998）、<c>m_boAllowGroup</c>（:10001，ObjPlayer.pas:72，本片未声明）、
    /// <c>m_btDeputyHeroJob</c>（:10006，ObjPlayer.pas:381，本片未声明）。
    /// 其中 <c>m_nCurrX/m_nCurrY/m_btDirection/m_nCharStatus</c> 与
    /// <c>MakeWord</c>/<c>MakeLong</c>（<c>PlayerSurfacePack</c>）已就绪。</para>
    /// </summary>
    public void SendLogon()
    {
        PortNotPorted(nameof(SendLogon), 9992);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendServerConfig();`（ObjPlayer.pas:10014）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_GateArr</c>/<c>TGateInfo.sServerConfigCRC</c>
    /// （:10018-10020）、<c>g_ServerConfigText</c>（:10022）、<c>g_ServerConfigTextLen</c>（:10022）、
    /// <c>g_ServerConfigTextCRC</c>（:10020）、<c>TClientConfig</c> 的 <c>SizeOf</c>（:10023）。
    /// ⚠ 托管同命名空间下另有 <c>M2Config.SendServerConfig()</c>（<c>M2Config.GameSpeed.cs:119</c>）
    /// 与 <c>CustomMagicSeams.SendServerConfig()</c>（<c>CustomMagicSeams.cs:452</c>）——
    /// 那是**别的类**上的调用计数器/接缝，与本方法无关，**未改动**。</para>
    /// </summary>
    public void SendServerConfig()
    {
        PortNotPorted(nameof(SendServerConfig), 10014);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendEnableClientUploadPickItems;`（ObjPlayer.pas:10035）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>g_nKey_UseClientPickItems</c>（:10036，
    /// M2Share 全局；托管侧只有 <b>窗体实例字段</b> <c>Forms/ViewList2Form.Rules.cs:181</c> 与
    /// <c>Forms/GamePets/GamePetsForm.cs:266</c>，**不是全局**，不能直读）、
    /// <c>m_btEnableUseClientPickItems</c>（:10037/:10039）。
    /// 注：<c>g_Config.boEnablePlayerUseClientPickItems</c>（:10037）**已存在**
    /// （<c>Engine/M2Config.ViewList2.cs:12</c>）。</para>
    /// </summary>
    public void SendEnableClientUploadPickItems()
    {
        PortNotPorted(nameof(SendEnableClientUploadPickItems), 10035);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUseItems;`（ObjPlayer.pas:10044）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>UserItemToClientItem(@m_UseItems[I], StdItem, ClientItem, True, False)</c>
    /// 的**真实字节编码器**（:10071）。托管接缝
    /// <c>TCreature.PlayerSurface.Items.cs:47</c> 的
    /// <c>UserItemToClientItem: Func&lt;TUserItemView, TStdItem, object?&gt;</c> **返回 object?**，
    /// 不是可写进 <c>InBuf</c> 的 <c>TClientItem</c> 布局，故无法用它装配
    /// <c>SendSocketEx(..., InBuf, InBytes)</c> 所需的连续缓冲（:10080）。
    /// 其余（<c>m_UseItems</c>、<c>m_boOffLine/m_boDummyObject</c>、<c>m_boSendUseItemsOK</c>）均已就绪
    /// 或可声明 —— 单点阻塞在编码器。</para>
    /// </summary>
    public void SendUseItems()
    {
        PortNotPorted(nameof(SendUseItems), 10044);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUseIcons(BaseObject: TBaseObject);`（ObjPlayer.pas:10085）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>m_PEnvir.m_nSecretFlag</c>、
    /// <c>m_PEnvir.m_nSecretFlag2</c>（:10103-10104）与常量 <c>SecretFlag_HideActorIcons</c>
    /// （:10103-10104）。托管 <c>Engine/Envir.cs:80</c> 的 <c>TEnvirnoment</c> 无这两个字段且
    /// **不是 partial**。注：<c>m_ActorIcons</c>（<c>TCreature.PlayerSurface.Base.cs:211</c>）、
    /// <c>m_btRaceServer</c>、<c>m_boOffLine/m_boDummyObject</c> 均已就绪。</para>
    /// </summary>
    public void SendUseIcons(TCreature baseObject)
    {
        PortNotPorted(nameof(SendUseIcons), 10085);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUseEffects(BaseObject: TBaseObject);`（ObjPlayer.pas:10133）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>m_BaseObjectEffects</c> 锁列表与
    /// <c>LockR(3)</c>/<c>UnLockR</c>（:10148/:10165）、其元素
    /// <c>pTBaseObjectEffect.ActorEffect</c>（:10152-10153）。注：<c>TActorEffect</c>
    /// 记录本身**已存在**（<c>GXX.Core/Protocol/Grobal2.Types1.cs:227</c>），缺的是容器。</para>
    /// </summary>
    public void SendUseEffects(TCreature baseObject)
    {
        PortNotPorted(nameof(SendUseEffects), 10133);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUseMagic;`（ObjPlayer.pas:10181）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>UserMagicToClientMagic</c>（:10208）、
    /// <c>GetMagicCD</c>（:10209）、<c>GetCustomMagicConfig</c> 与 <c>TCustomMagicConfig</c>
    /// 的 <c>ServerConfig.FailMsg</c>（:10211-10215）、<c>m_CustomSkillUseTick</c>（:10217）、
    /// <c>TClientMagic</c> 记录布局（:10185/:10207）、<c>CUSTOM_MAGIC_START_ID</c>（:10217）。
    /// 注：<c>m_MagicList</c>（<c>ObjBase.cs:197</c>）与 <c>EDcode.zLibCompressBuffer</c>
    /// （:10226）已就绪。</para>
    /// </summary>
    public void SendUseMagic()
    {
        PortNotPorted(nameof(SendUseMagic), 10181);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.ClientTakeOnItemsEx(btWhere: Byte; nItemIdx: Integer; sItemName: string);`
    /// （ObjPlayer.pas:10240，**237 行**）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：<c>CheckUserItems</c>（:10285）、
    /// <c>ItemUnit.GetItemAddValue</c>（:10288）、<c>CheckTakeOnItems</c>（:10289）、
    /// <c>CheckItemBindUse</c>（:10289）、<c>InDisableTakeOffList</c>（:10318）、
    /// <c>GetUserItemBindValue</c>（:10319）、<c>g_FunctionNPC.GotoLable</c>（:10336 等 5 处）、
    /// <c>ProcessUseItemSkill</c>（:10391/:10403/:10411）、<c>DelBagItem</c>（:10395）、
    /// <c>AddItemToBag</c>/<c>SendAddItem</c>（:10413/:10415）、
    /// <c>DropItemDown</c>（:10419）、<c>FeatureChanged</c>（:10452）、<c>RefShowName</c>（:10454）、
    /// 以及一条**原文缺陷**：<c>:10365</c> 在 `@BeginTakeOn` 之后判的却是
    /// <c>m_boStopTakeOff</c>（而非 <c>m_boStopTakeOn</c>）—— 一旦移植必须逐字保留该行，
    /// 故此处**不**半移植。</para>
    /// </summary>
    public void ClientTakeOnItemsEx(byte btWhere, int nItemIdx, string sItemName)
    {
        PortNotPorted(nameof(ClientTakeOnItemsEx), 10240);
    }
}
