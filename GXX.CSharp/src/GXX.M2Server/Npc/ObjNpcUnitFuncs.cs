// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：**单元级函数** 1:1 移植（原文 implementation 段中不属于任何类的方法）。
//   · TConditionList.Create                     504-507   （→ ObjNpcTypes.cs 的构造函数）
//   · LoadLevelScriptAction                     509-595
//   · LoadLevelScriptCondition                  596-681
//   · GetLevelBaseObjectCondition               682-856
//   · GetLevelBaseObjectAction                  857-1106
//   · CheckStrIsVar                             10406-10509
//   另：interface 段的声明 486-494。
//
// 原文 `ExtractStrings(['.'], [], PChar(sCmd), TempList)` 走托管侧既有等效
// `GXX.M2Server.Engine.TGroupItems.ExtractStrings(char, string)`（GroupItems.cs:287，
// 注释即 "ExtractStrings(['|'], [], ...) 等效：跳空串"）——不复制第二份实现。
//
// ⚠ 偏差登记（本车道不改他人文件，见报告 §6）：
//   原文 `CompareLStr`（HUtil32.pas:1981-1995）= **大小写不敏感** 且要求 `compn > 0`。
//   `GXX.Core.Util.HUtil32.CompareLStr`（HUtil32.cs:581）用的是 `string.CompareOrdinal`
//   → **大小写敏感**、且**缺 `compn <= 0` 守卫**，与原文不符。
//   本文件因此统一转调 `GXX.M2Server.Engine.MonGenParseCore.CompareLStr`（MonGenParseCore.cs:116），
//   它是原文的 1:1 实现（`compn <= 0` 守卫 + `ToUpperInvariant` 逐字符比较）。**未复制第三份**。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

/// <summary>
/// ObjNpc.pas implementation 段的单元级函数集合（原文无类前缀，故为 static）。
/// </summary>
public static class ObjNpcUnitFuncs
{
    /// <summary>
    /// 原文 `function LoadLevelScriptAction(QuestActionInfo: pTQuestActionInfo; sCmd: string): string;`（ObjNpc.pas:509-594）。
    /// 把 `A.B.C` 形式的命令前缀切成 `SELF` + 各级目标，写入 `ScriptCmd`/`ScriptList`，返回最后一段（大写去空格）。
    /// </summary>
    public static string LoadLevelScriptAction(TQuestActionInfo QuestActionInfo, string sCmd)
    {
        QuestActionInfo.ScriptCmd = Array.Empty<int>();
        QuestActionInfo.ScriptList = Array.Empty<string>();
        string Result = sCmd;
        // 原文 518：`if (Pos('.', sCmd) > 0) and (sCmd[Length(sCmd)] <> '.') then`
        if ((DelphiRTL.Pos(".", sCmd) > 0) && (sCmd[sCmd.Length - 1] != '.'))
        {
            List<string> TempList = TGroupItems.ExtractStrings('.', sCmd);
            Result = DelphiRTL.UpperCase(DelphiRTL.Trim(TempList[TempList.Count - 1]));
            TempList.RemoveAt(TempList.Count - 1);
            TempList[0] = DelphiRTL.UpperCase(DelphiRTL.Trim(TempList[0]));
            if (TempList[0] != "SELF")
                TempList.Insert(0, "SELF");
            QuestActionInfo.ScriptCmd = new int[TempList.Count];
            QuestActionInfo.ScriptList = new string[TempList.Count];
            for (int I = 0; I <= TempList.Count - 1; I++)
            {
                string S = DelphiRTL.UpperCase(DelphiRTL.Trim(TempList[I]));
                if (S == "SELF")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_0;
                }
                else if ((S == "H") || (S == "HERO"))
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_1;
                }
                else if (S == "O")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_2;
                }
                else if (S == "M")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_3;
                }
                else if (S == "P")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_4;
                }
                else if (S == "L")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_6;
                }
                else if (S == "HM")
                {
                    // 原文 558-561：只有 g_nKey_HeroExt = 1 才赋值，否则保持 SetLength 的 0（= CMD_RACE_0）
                    if (NpcSeams.g_nKey_HeroExt == 1)
                    {
                        QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_7;
                    }
                }
                else if (S == "HL")
                {
                    // 原文 565-568：同上，未赋值时保持 0（= CMD_RACE_0）
                    if (NpcSeams.g_nKey_HeroExt == 1)
                    {
                        QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_8;
                    }
                }
                else if (S == "PET")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_9;
                }
                else if (S == "BB")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_10;
                }
                else if (S == "BBR")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_12;
                }
                else if (S == "FS")
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_11;
                }
                else
                {
                    QuestActionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_5;
                }
                QuestActionInfo.ScriptList[I] = S;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function LoadLevelScriptCondition(QuestConditionInfo: pTQuestConditionInfo; sCmd: string): string;`（ObjNpc.pas:596-680）。
    /// <para><b>与 LoadLevelScriptAction 的三处真实差异（差异断言）</b>：</para>
    /// <list type="number">
    /// <item>原文 605 只有 `Pos('.', sCmd) > 0`，**没有** Action 那侧的 `sCmd[Length(sCmd)] &lt;&gt; '.'` 尾点保护
    ///   → 以 `.` 结尾的命令（如 `"A."`）会在原文 610 的 `Delete` 之后于 611 行取 `Strings[0]` 越界抛异常。</item>
    /// <item>原文 662-671 把 `BB` / `FS` 两个分支整段注释掉 → 二者落到 `else` 得到 `CMD_RACE_5`（Action 侧是 10/11）。</item>
    /// <item>本函数**没有** `BBR` 分支 → `'BBR'` 也落到 `CMD_RACE_5`（Action 侧是 12）。</item>
    /// </list>
    /// </summary>
    public static string LoadLevelScriptCondition(TQuestConditionInfo QuestConditionInfo, string sCmd)
    {
        QuestConditionInfo.ScriptCmd = Array.Empty<int>();
        QuestConditionInfo.ScriptList = Array.Empty<string>();
        string Result = sCmd;
        // 原文 605：无尾点保护（与 Action 的 518 不同 —— 见 xml 注释差异 1）
        if (DelphiRTL.Pos(".", sCmd) > 0)
        {
            List<string> TempList = TGroupItems.ExtractStrings('.', sCmd);
            Result = DelphiRTL.UpperCase(DelphiRTL.Trim(TempList[TempList.Count - 1]));
            TempList.RemoveAt(TempList.Count - 1);
            TempList[0] = DelphiRTL.UpperCase(DelphiRTL.Trim(TempList[0]));
            if (TempList[0] != "SELF")
                TempList.Insert(0, "SELF");
            QuestConditionInfo.ScriptCmd = new int[TempList.Count];
            QuestConditionInfo.ScriptList = new string[TempList.Count];
            for (int I = 0; I <= TempList.Count - 1; I++)
            {
                string S = DelphiRTL.UpperCase(DelphiRTL.Trim(TempList[I]));
                if (S == "SELF")
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_0;
                }
                else if ((S == "H") || (S == "HERO"))
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_1;
                }
                else if (S == "O")
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_2;
                }
                else if (S == "M")
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_3;
                }
                else if (S == "P")
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_4;
                }
                else if (S == "L")
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_6;
                }
                else if (S == "HM")
                {
                    if (NpcSeams.g_nKey_HeroExt == 1)
                    {
                        QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_7;
                    }
                }
                else if (S == "HL")
                {
                    if (NpcSeams.g_nKey_HeroExt == 1)
                    {
                        QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_8;
                    }
                }
                else if (S == "PET")
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_9;
                }
                // 原文 661-671：`// 检测指令不支持宝宝` 及其后 `{ ... }` 块注释掉的 BB / FS 分支
                // {
                //   else if S = 'BB' then begin QuestConditionInfo.ScriptCmd[I] := CMD_RACE_10; end
                //   else if S = 'FS' then begin QuestConditionInfo.ScriptCmd[I] := CMD_RACE_11; end
                // }
                else
                {
                    QuestConditionInfo.ScriptCmd[I] = ObjNpcConst.CMD_RACE_5;
                }
                QuestConditionInfo.ScriptList[I] = S;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function GetLevelBaseObjectCondition(Npc: TNormNpc; PlayObject: TPlayObject;
    /// QuestConditionInfo: pTQuestConditionInfo): TBaseObject;`（ObjNpc.pas:682-855）。
    /// 原文返回类型 `TBaseObject` 在托管侧以既有接缝 <see cref="TCreature"/> 承载。
    /// <para><b>与 GetLevelBaseObjectAction 的真实差异</b>：</para>
    /// <list type="bullet">
    /// <item>`CMD_RACE_5` 少了 Action 侧 960-961 的 `if BaseObject = nil then Break;` —— 本函数查不到玩家时
    ///   `BaseObject` 会变为 nil 但**不中断**，后续目标解析全部落在 `BaseObject = nil` 的 else 分支里；
    ///   Action 侧则立即中断（差异断言）。</item>
    /// <item>本函数**没有** `CMD_RACE_10/11/12` 分支 → 落到原文 849-850 的 `else Break;`（差异断言）。</item>
    /// </list>
    /// </summary>
    public static TCreature? GetLevelBaseObjectCondition(TNormNpc Npc, TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
    {
        if (QuestConditionInfo.ScriptCmd.Length <= 0)
        {
            return PlayObject;
        }
        else
        {
            TCreature? BaseObject = PlayObject;
            string sCharName = "";
            string sVar = "";
            string sValue = "";
            int nValue = 0;
            // 原文 `Break` 在 `case` 内表示跳出外层 `for`；托管侧 `break` 只跳出 `switch`，
            // 故用 boBreak 标志在 switch 之后跳出 for（语义等价）。
            bool boBreak = false;
            for (int I = 0; I <= QuestConditionInfo.ScriptCmd.Length - 1; I++)
            {
                switch (QuestConditionInfo.ScriptCmd[I])
                {
                    case ObjNpcConst.CMD_RACE_0:
                        {
                            BaseObject = PlayObject;
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_1:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT))
                            {
                                BaseObject = NpcSeams.GetMyHero((TPlayObject)BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_2:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = BaseObject.m_Master;
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_3:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = NpcSeams.GetCurrTarget(BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_4:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = NpcSeams.GetPoseCreate(BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_5:
                        {
                            if (BaseObject != null)
                            {
                                if (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                {
                                    Npc.GetVarValue((TPlayObject)BaseObject, QuestConditionInfo.ScriptList[I],
                                        ref sVar, ref sValue, ref nValue);
                                    sCharName = sValue;
                                    // 原文 768-771 的块注释（else if RC_HEROOBJECT 分支）—— 原文如此，保留
                                }
                                else
                                {
                                    sCharName = QuestConditionInfo.ScriptList[I];
                                }
                                // 原文 777-778 两行注释掉的调试/旧逻辑 —— 原文如此，保留
                                BaseObject = NpcSeams.GetPlayObject(sCharName);
                                // 原文 780-785：这里**没有** nil 检查（Action 侧 960-961 有）
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_6:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = NpcSeams.GetLastHiter(BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_7:
                        {
                            if (NpcSeams.g_nKey_HeroExt == 1)
                            {
                                if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                    && (NpcSeams.GetMyHero((TPlayObject)BaseObject) != null))
                                {
                                    BaseObject = NpcSeams.GetCurrTarget(NpcSeams.GetMyHero((TPlayObject)BaseObject));
                                    if (BaseObject == null)
                                        boBreak = true;
                                }
                                else
                                {
                                    BaseObject = null;
                                    boBreak = true;
                                }
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_8:
                        {
                            if (NpcSeams.g_nKey_HeroExt == 1)
                            {
                                if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                    && (NpcSeams.GetMyHero((TPlayObject)BaseObject) != null))
                                {
                                    BaseObject = NpcSeams.GetLastHiter(NpcSeams.GetMyHero((TPlayObject)BaseObject));
                                    if (BaseObject == null)
                                        boBreak = true;
                                }
                                else
                                {
                                    BaseObject = null;
                                    boBreak = true;
                                }
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_9:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                && (((TPlayObject)BaseObject).m_MyGamePet != null))
                            {
                                BaseObject = ((TPlayObject)BaseObject).m_MyGamePet;
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    default:
                        boBreak = true;
                        break;
                }
                if (boBreak)
                    break;
            }
            return BaseObject;
        }
    }

    /// <summary>
    /// 原文 `function GetLevelBaseObjectAction(Npc: TNormNpc; PlayObject: TPlayObject;
    /// QuestActionInfo: pTQuestActionInfo): TBaseObject;`（ObjNpc.pas:857-1105）。
    /// <para>与 Condition 版的差异见 <see cref="GetLevelBaseObjectCondition"/> 的 xml 注释；
    /// 另有原文 866-869 的 `Result := nil;` + `QuestActionInfo = nil → Exit` 前置守卫（Condition 版无此守卫）。</para>
    /// </summary>
    public static TCreature? GetLevelBaseObjectAction(TNormNpc Npc, TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
    {
        TCreature? Result = null;
        // 修复 加载报错 chongchong 2013-12-10（原文 867）
        if (QuestActionInfo == null)
            return Result;
        if (QuestActionInfo.ScriptCmd.Length <= 0)
        {
            Result = PlayObject;
        }
        else
        {
            TCreature? BaseObject = PlayObject;
            TCreature? TempObject;
            TPlayObject Player;
            string sCharName = "";
            string sVar = "";
            string sValue = "";
            int nValue = 0;
            int Count;
            bool boBreak = false;
            for (int I = 0; I <= QuestActionInfo.ScriptCmd.Length - 1; I++)
            {
                switch (QuestActionInfo.ScriptCmd[I])
                {
                    case ObjNpcConst.CMD_RACE_0:
                        {
                            BaseObject = PlayObject;
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_1:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT))
                            {
                                BaseObject = NpcSeams.GetMyHero((TPlayObject)BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_2:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = BaseObject.m_Master;
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_3:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = NpcSeams.GetCurrTarget(BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_4:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = NpcSeams.GetPoseCreate(BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_5:
                        {
                            if (BaseObject != null)
                            {
                                if (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                {
                                    Npc.GetVarValue((TPlayObject)BaseObject, QuestActionInfo.ScriptList[I],
                                        ref sVar, ref sValue, ref nValue);
                                    sCharName = sValue;
                                    // 原文 948-951 的块注释 —— 原文如此，保留
                                }
                                else
                                {
                                    sCharName = QuestActionInfo.ScriptList[I];
                                }
                                // 原文 957-958 两行注释 —— 原文如此，保留
                                BaseObject = NpcSeams.GetPlayObject(sCharName);
                                if (BaseObject == null)
                                    boBreak = true;   // 原文 960-961（Condition 侧无此判断）
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_6:
                        {
                            if (BaseObject != null)
                            {
                                BaseObject = NpcSeams.GetLastHiter(BaseObject);
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_7:
                        {
                            if (NpcSeams.g_nKey_HeroExt == 1)
                            {
                                if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                    && (NpcSeams.GetMyHero((TPlayObject)BaseObject) != null))
                                {
                                    BaseObject = NpcSeams.GetCurrTarget(NpcSeams.GetMyHero((TPlayObject)BaseObject));
                                    if (BaseObject == null)
                                        boBreak = true;
                                }
                                else
                                {
                                    BaseObject = null;
                                    boBreak = true;
                                }
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_8:
                        {
                            if (NpcSeams.g_nKey_HeroExt == 1)
                            {
                                if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                    && (NpcSeams.GetMyHero((TPlayObject)BaseObject) != null))
                                {
                                    BaseObject = NpcSeams.GetLastHiter(NpcSeams.GetMyHero((TPlayObject)BaseObject));
                                    if (BaseObject == null)
                                        boBreak = true;
                                }
                                else
                                {
                                    BaseObject = null;
                                    boBreak = true;
                                }
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_9:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                && (((TPlayObject)BaseObject).m_MyGamePet != null))
                            {
                                BaseObject = ((TPlayObject)BaseObject).m_MyGamePet;
                                if (BaseObject == null)
                                    boBreak = true;
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_10:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                && (((TPlayObject)BaseObject).m_SlaveList != null))
                            {
                                Player = (TPlayObject)BaseObject;
                                BaseObject = null;
                                for (int II = 0; II <= Player.m_SlaveList.Count - 1; II++)
                                {
                                    TempObject = Player.m_SlaveList[II];
                                    if ((!TempObject.m_boDeath) && (!TempObject.m_boGhost))
                                    {
                                        BaseObject = TempObject;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_11:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                && (((TPlayObject)BaseObject).m_SlaveList != null))
                            {
                                Player = (TPlayObject)BaseObject;
                                BaseObject = null;
                                for (int II = 0; II <= Player.m_SlaveList.Count - 1; II++)
                                {
                                    TempObject = Player.m_SlaveList[II];
                                    if ((!TempObject.m_boDeath) && (!TempObject.m_boGhost)
                                        && NpcSeams.IsCopyMon(TempObject))
                                    {
                                        BaseObject = TempObject;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    case ObjNpcConst.CMD_RACE_12:
                        {
                            if ((BaseObject != null) && (BaseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
                                && (((TPlayObject)BaseObject).m_SlaveList != null))
                            {
                                Player = (TPlayObject)BaseObject;
                                BaseObject = null;
                                Count = 0;
                                // 原文 1082-1091：最多 20 次随机取一条**存活**宝宝；全失败则 BaseObject 保持 nil
                                // 且**不** boBreak（外层 for 继续下一个目标级别）。
                                // 注意原文 `Random(Player.m_SlaveList.Count)` 在 Count = 0 时会取 Items[0] 越界。
                                while ((BaseObject == null) && (Count < 20))
                                {
                                    TempObject = Player.m_SlaveList[NpcSeams.Random(Player.m_SlaveList.Count)];
                                    if ((!TempObject.m_boDeath) && (!TempObject.m_boGhost))
                                    {
                                        BaseObject = TempObject;
                                        break;
                                    }
                                    Count++;
                                }
                            }
                            else
                            {
                                BaseObject = null;
                                boBreak = true;
                            }
                        }
                        break;
                    default:
                        boBreak = true;
                        break;
                }
                if (boBreak)
                    break;
            }
            Result = BaseObject;
        }
        return Result;
    }

    /// <summary>
    /// 原文 `function CheckStrIsVar(sText: string): Boolean;`（ObjNpc.pas:10406-10506）。
    /// <para><b>原文易错点（照抄并保留）</b>：</para>
    /// <list type="bullet">
    /// <item>10416-10419 的 `$HUMAN(` 分支**没有** `Exit`，随后 10420 起会继续判 `$GUILD(` / `$GLOBAL(` / `$STR(`；
    ///   但 `Result := True` 已被置位，故行为上等价于提前返回 —— 只是多跑几次 CompareLStr（照抄，不"优化"）。</item>
    /// <item>10430-10504 的 `$STR(` 分支：`ArrestStringEx(sText, '(', ')', s14)` 的**返回值被丢弃**，
    ///   且 `s14` 是 `var` 出参（原文本意是取括号内内容）。</item>
    /// <item>10500-10503 的 `'N$'` 判定用的是 `s14[2] = '$'`（**区分大小写**），而首字符用 `UpCase`，
    ///   与 10492/10496 同构 —— 但 10496 的 `'S$'` 与 10500 的 `'N$'` 永远不会同时命中，因为
    ///   10490 的 `GetValNameNo(s14) >= 0` 已经对 `S...`/`N...`/`L...` 形式返回了负数才会走到这里。</item>
    /// </list>
    /// </summary>
    public static bool CheckStrIsVar(string sText)
    {
        bool Result = false;
        if ((sText.Length > 3) && (DelphiRTL.Pos("<", sText) > 0) && (DelphiRTL.Pos("$", sText) > 0)
            && (DelphiRTL.Pos(">", sText) > 0))
        {
            sText = DelphiRTL.UpperCase(sText);
            string s14 = "";
            HUtil32.ArrestVariable(sText, '<', '$', '>', 0, ref s14);
            sText = s14;   // 原文 10415 把 var 出参 sText 写回（入参与出参同名）
            if (MonGenParseCore.CompareLStr(sText, "$HUMAN(", "$HUMAN(".Length))
            {
                Result = true;
            }
            if (MonGenParseCore.CompareLStr(sText, "$GUILD(", "$GUILD(".Length))
            {
                Result = true;
                return Result;
            }
            if (MonGenParseCore.CompareLStr(sText, "$GLOBAL(", "$GLOBAL(".Length))
            {
                Result = true;
                return Result;
            }
            if (MonGenParseCore.CompareLStr(sText, "$STR(", "$STR(".Length))
            {
                string s14b = "";
                HUtil32.ArrestStringEx(sText, '(', ')', ref s14b);

                int n18 = NpcSeams.GetValNameNo(s14b);
                if (n18 >= 0)
                {
                    // 原文 10437-10489：每个分支体都只是 `Result := True;`，区间表照抄
                    if (n18 is >= 0 and <= 999)
                    {
                        Result = true;      // P
                    }
                    else if (n18 is >= 1000 and <= 1999)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 2000 and <= 2999)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 3000 and <= 3999)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 4000 and <= 4999)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 5000 and <= 5999)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 6000 and <= 6999)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 7000 and <= 7999)
                    {
                        Result = true;
                    }
                    // 私有变量 U-数字型 chongchong 2014-10-18
                    else if (n18 is >= 8000 and <= 8499)
                    {
                        Result = true;
                    }
                    // 私有变量 T-字符串型 chongchong 2014-10-18
                    else if (n18 is >= 8500 and <= 8999)
                    {
                        Result = true;
                    }
                    // 私有变量 J-数字型(1天1清) chongchong 2014-10-18
                    else if (n18 is >= 9000 and <= 9499)
                    {
                        Result = true;
                    }
                    else if (n18 is >= 9500 and <= 9999)
                    {
                        Result = true;
                    }
                }
                // L变量
                else if ((s14b.Length > 2) && (char.ToUpperInvariant(s14b[0]) == 'L') && (s14b[1] == '$'))
                {
                    Result = true;
                }
                else if ((s14b.Length > 2) && (char.ToUpperInvariant(s14b[0]) == 'S') && (s14b[1] == '$'))
                {
                    Result = true;
                }
                else if ((s14b.Length > 2) && (char.ToUpperInvariant(s14b[0]) == 'N') && (s14b[1] == '$'))
                {
                    Result = true;
                }
            }
        }
        return Result;
    }
}
