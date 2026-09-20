// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：TNormNpc 的**标签管理 / 脚本记录排序与二分查找 / 脚本错误上报** 1:1 移植。
//   · AllowSelect                      5934-5951
//   · AddSelectLable                   5953-5965
//   · DeleteSelectLable                5967-5979
//   · ClearScript                      4383-4429
//   · QuickSortRecordList              9955-9994
//   · DoSort                           9996-10017
//   · GetSayingRecordFromRecordList    10019-10046
//   · ScriptActionError                9745-9765
//   · ScriptConditionError             9767-9787
//
// 未覆盖（见报告 §2 判定表）：Create 5879-5915 / Destroy 5917-5932 / Initialize 9864-9875
//   —— 依赖 ObjBase/ObjGame 基类尚未移植的字段（m_nLight / m_btNameColor / m_nWalkSpeed /
//   m_nInitWalkSpeed / m_wAppr / m_Castle），本车道不越区补基类字段。
//   Click 4431-4442 / UserSelect 9807-9835 / SendMsgToUser 9789-9798 / MessageBox 9800-9805 /
//   SendCustemMsg 9837-9862 / GetShowName 9609-9626 / LoadAddData 9900-9924 / SaveAddData 9926-9952
//   —— 依赖 TPlayObject 的脚本标签字段与 TIniFile/TFormat 宿主面，见报告。
//
// Delphi `CompareText`（SysUtils）是**大小写不敏感的 ASCII 比较**，本文件用
// ObjNpcText.CompareText 落地（Delphi 语义：逐字节 UpCase 比较，等长时比长度）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

/// <summary>
/// ObjNpc.pas 用到的文本比较辅助（原文直接调 SysUtils.CompareText）。
/// 独立小工具类，避免与既有同名类型冲突（GXX.M2Server 顶层与 Engine 下均无 CompareText）。
/// </summary>
internal static class ObjNpcText
{
    /// <summary>
    /// Delphi `SysUtils.CompareText(S1, S2): Integer` 的 1:1 语义：
    /// 逐字符 `UpCase` 比较，首个不同处返回 ±1；全部相同则返回长度差。
    /// </summary>
    internal static int CompareText(string s1, string s2)
    {
        s1 ??= "";
        s2 ??= "";
        int n = Math.Min(s1.Length, s2.Length);
        for (int i = 0; i < n; i++)
        {
            char a = char.ToUpperInvariant(s1[i]);
            char b = char.ToUpperInvariant(s2[i]);
            if (a != b)
                return a < b ? -1 : 1;
        }
        return s1.Length.CompareTo(s2.Length);
    }
}

public partial class TNormNpc
{
    /// <summary>
    /// 原文 `function AllowSelect(sLabel: string): Boolean;`（ObjNpc.pas:5934-5951）。
    /// <para><b>照抄的原文细节</b>：`m_NoUserSelectList` 是**前缀黑名单**（`CompareLStr` 大小写不敏感），
    /// 且比较长度取**黑名单条目的长度**（不是 sLabel 的长度）→ 长条目会因 `sLabel` 太短而**不匹配**；
    /// 只有"当前 NPC 是 g_FunctionNPC 或 g_MissionNPC"时才打印诊断（原文 5943-5946）。</para>
    /// </summary>
    public bool AllowSelect(string sLabel)
    {
        bool Result = true;
        for (int I = 0; I <= m_NoUserSelectList.Count - 1; I++)
        {
            if (MonGenParseCore.CompareLStr(sLabel, m_NoUserSelectList[I], m_NoUserSelectList[I].Length))
            {
                if (NpcSeams.IsFunctionOrMissionNpc(this))
                {
                    NpcSeams.MainOutMessage($"不可点字段:{m_NoUserSelectList[I]}");
                }
                Result = false;
                return Result;
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `procedure AddSelectLable(sLabel: string);`（ObjNpc.pas:5953-5965）。
    /// 已存在（按前缀比较）则直接 Exit，否则追加到末尾（**不排序、不去重以外的处理**）。
    /// </summary>
    public void AddSelectLable(string sLabel)
    {
        for (int I = 0; I <= m_NoUserSelectList.Count - 1; I++)
        {
            if (MonGenParseCore.CompareLStr(sLabel, m_NoUserSelectList[I], m_NoUserSelectList[I].Length))
            {
                return;
            }
        }
        m_NoUserSelectList.Add(sLabel);
    }

    /// <summary>
    /// 原文 `procedure DeleteSelectLable(sLabel: string);`（ObjNpc.pas:5967-5979）。
    /// 命中**第一条**即删除并 Break（只删一个）。
    /// </summary>
    public void DeleteSelectLable(string sLabel)
    {
        for (int I = 0; I <= m_NoUserSelectList.Count - 1; I++)
        {
            if (MonGenParseCore.CompareLStr(sLabel, m_NoUserSelectList[I], m_NoUserSelectList[I].Length))
            {
                m_NoUserSelectList.Delete(I);
                break;
            }
        }
    }

    /// <summary>
    /// 原文 `procedure ClearScript; virtual;`（ObjNpc.pas:4383-4429）。
    /// <para>四层嵌套释放（Script → RecordList → ProcedureList → ConditionList/ActionList/ElseActionList），
    /// Delphi 的 `Dispose` / `Free` 在托管侧无对应动作（GC），故**只保留结构性遍历**，
    /// 并在最后 `m_ScriptList.Clear`（原文 4428）。遍历顺序与层级 1:1 保留以便后续接入真实释放钩子。</para>
    /// </summary>
    public virtual void ClearScript()
    {
        for (int I = 0; I <= m_ScriptList.Count - 1; I++)
        {
            TScript Script = (TScript)m_ScriptList[I];
            for (int II = 0; II <= Script.RecordList.Count - 1; II++)
            {
                TSayingRecord SayingRecord = (TSayingRecord)Script.RecordList[II];
                for (int III = 0; III <= SayingRecord.ProcedureList.Count - 1; III++)
                {
                    TSayingProcedure SayingProcedure = (TSayingProcedure)SayingRecord.ProcedureList[III];
                    for (int IIII = 0; IIII <= SayingProcedure.ConditionList.Items.Count - 1; IIII++)
                    {
                        TQuestConditionInfo QuestConditionInfo =
                            (TQuestConditionInfo)SayingProcedure.ConditionList.Items[IIII];
                        // 原文 4405：Dispose(QuestConditionInfo) —— 托管侧由 GC 负责
                        _ = QuestConditionInfo;
                    }
                    for (int IIII = 0; IIII <= SayingProcedure.ActionList.Count - 1; IIII++)
                    {
                        TQuestActionInfo QuestActionInfo = (TQuestActionInfo)SayingProcedure.ActionList[IIII];
                        // 原文 4410：Dispose(QuestActionInfo)
                        _ = QuestActionInfo;
                    }
                    for (int IIII = 0; IIII <= SayingProcedure.ElseActionList.Count - 1; IIII++)
                    {
                        TQuestActionInfo QuestActionInfo = (TQuestActionInfo)SayingProcedure.ElseActionList[IIII];
                        // 原文 4415：Dispose(QuestActionInfo)
                        _ = QuestActionInfo;
                    }
                    // 原文 4417-4420：ConditionList.Free / ActionList.Free / ElseActionList.Free / Dispose(SayingProcedure)
                }
                // 原文 4422-4423：SayingRecord.ProcedureList.Free / Dispose(SayingRecord)
            }
            // 原文 4425-4426：Script.RecordList.Free / Dispose(Script)
        }
        m_ScriptList.Clear();
    }

    /// <summary>
    /// 原文 `procedure QuickSortRecordList(List: TList; L, R: Integer);`（ObjNpc.pas:9955-9994）。
    /// <para>Hoare 划分 + 尾递归消除（`repeat ... until I &gt;= R`）的**逐行照抄**；
    /// `List.Exchange(I, J)` → 交换 `List&lt;object&gt;` 的两个元素；
    /// 枢纽元素在 `P = I` / `P = J` 时同步跟随交换（原文 9975-9984）。</para>
    /// <para>比较用 <see cref="ObjNpcText.CompareText"/>（原文 `CompareText`，大小写不敏感）。</para>
    /// </summary>
    public void QuickSortRecordList(List<object> List, int L, int R)
    {
        int I, J, P;
        TSayingRecord SayingRecord;
        do
        {
            I = L;
            J = R;
            P = (L + R) >> 1;
            SayingRecord = (TSayingRecord)List[P];
            do
            {
                while (ObjNpcText.CompareText(((TSayingRecord)List[I]).sLabel, SayingRecord.sLabel) < 0)
                    I++;

                while (ObjNpcText.CompareText(((TSayingRecord)List[J]).sLabel, SayingRecord.sLabel) > 0)
                    J--;

                if (I <= J)
                {
                    (List[I], List[J]) = (List[J], List[I]);
                    if (P == I)
                    {
                        P = J;
                        SayingRecord = (TSayingRecord)List[P];
                    }
                    else if (P == J)
                    {
                        P = I;
                        SayingRecord = (TSayingRecord)List[P];
                    }
                    I++;
                    J--;
                }
            } while (I <= J);

            if (L < J)
                QuickSortRecordList(List, L, J);
            L = I;
        } while (I < R);
    }

    /// <summary>
    /// 原文 `procedure DoSort;`（ObjNpc.pas:9996-10017）。
    /// 跳过 `RecordList = nil` 或 `Count = 0` 的条目（原文 10005）；原文 10008-10014 的调试输出整段注释保留。
    /// </summary>
    public void DoSort()
    {
        for (int I = 0; I <= m_ScriptList.Count - 1; I++)
        {
            TScript Script = (TScript)m_ScriptList[I];
            if ((Script.RecordList != null) && (Script.RecordList.Count > 0))
            {
                QuickSortRecordList(Script.RecordList, 0, Script.RecordList.Count - 1);
                // 原文 10008-10014：调试用的逐条 OutputDebugString，整段被 `{ }` 注释 —— 原文如此，保留。
            }
        }
    }

    /// <summary>
    /// 原文 `function GetSayingRecordFromRecordList(List: TList; sLabel: string): pTSayingRecord;`
    /// （ObjNpc.pas:10019-10046）。
    /// <para><b>照抄的原文细节</b>：`C = CompareText(...)`；`C &lt; 0` 时只推 `L`；
    /// `C &gt; 0` 时只收 `H`（**不返回**，继续循环）；只有 `C = 0` 才记 `Result` 并 `Break`。
    /// 早退条件：`List = nil` 或 `Length(sLabel) = 0`（返回 nil）。</para>
    /// </summary>
    public TSayingRecord GetSayingRecordFromRecordList(List<object> List, string sLabel)
    {
        TSayingRecord Result = null;
        if ((List != null) && (sLabel.Length > 0))
        {
            int L = 0;
            int H = List.Count - 1;
            while (L <= H)
            {
                int I = (L + H) >> 1;
                int C = ObjNpcText.CompareText(((TSayingRecord)List[I]).sLabel, sLabel);
                if (C < 0)
                    L = I + 1;
                else
                {
                    H = I - 1;
                    if (C == 0)
                    {
                        Result = (TSayingRecord)List[I];
                        break;
                    }
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// 原文 `procedure ScriptActionError(BaseObject: TBaseObject; sErrMsg: string;
    /// QuestActionInfo: pTQuestActionInfo);`（ObjNpc.pas:9745-9765）。
    /// <para>`resourcestring sOutMessage`（9749）在托管侧落为常量 <see cref="ScriptActionErrorFormat"/>；
    /// `%d` 对应 `m_nCurrX` / `m_nCurrY`。原文 9752-9763 的旧实现整段注释保留。</para>
    /// </summary>
    public void ScriptActionError(TCreature BaseObject, string sErrMsg, TQuestActionInfo QuestActionInfo)
    {
        string sMsg = DelphiRTL.Format(ScriptActionErrorFormat,
            sErrMsg, QuestActionInfo.sCmd, m_sCharName, m_sMapName, m_nCurrX, m_nCurrY,
            QuestActionInfo.sParam1, QuestActionInfo.sParam2, QuestActionInfo.sParam3, QuestActionInfo.sParam4,
            QuestActionInfo.sParam5, QuestActionInfo.sParam6, QuestActionInfo.sParam7, QuestActionInfo.sParam8,
            QuestActionInfo.sParam9, QuestActionInfo.sParam10);
        // 原文 9752-9763：拼接式旧实现，整段被 `{ }` 注释 —— 原文如此，保留。
        NpcSeams.MainOutMessage(sMsg);
    }

    /// <summary>原文 9749 的 `resourcestring sOutMessage`（1:1，含全部 16 个占位符）。</summary>
    public const string ScriptActionErrorFormat =
        "[脚本错误] %s 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s 参数7:%s 参数8:%s 参数9:%s 参数10:%s";

    /// <summary>原文 9771 的 `resourcestring sOutMessage`（比 Action 版少开头的 `%s 脚本命令` 之外的 `sErrMsg`）。</summary>
    public const string ScriptConditionErrorFormat =
        "[脚本错误] 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s 参数7:%s 参数8:%s 参数9:%s 参数10:%s";

    /// <summary>
    /// 原文 `procedure ScriptConditionError(BaseObject: TBaseObject; QuestConditionInfo: pTQuestConditionInfo);`
    /// （ObjNpc.pas:9767-9787）。
    /// <para><b>照抄的原文缺陷</b>：9773 **只把 `sMsg` 拼了出来，却从不输出** ——
    /// 整段旧实现（9774-9786，含唯一的 `MainOutMessage` 调用）被 `{ }` 注释掉，
    /// 于是本过程在原文里**实际上什么都不做**（除了一次 Format 计算）。
    /// 托管侧照抄这一行为：只计算、不输出。</para>
    /// </summary>
    public void ScriptConditionError(TCreature BaseObject, TQuestConditionInfo QuestConditionInfo)
    {
        string sMsg = DelphiRTL.Format(ScriptConditionErrorFormat,
            QuestConditionInfo.sCmd, m_sCharName, m_sMapName, m_nCurrX, m_nCurrY,
            QuestConditionInfo.sParam1, QuestConditionInfo.sParam2, QuestConditionInfo.sParam3,
            QuestConditionInfo.sParam4, QuestConditionInfo.sParam5, QuestConditionInfo.sParam6,
            QuestConditionInfo.sParam7, QuestConditionInfo.sParam8, QuestConditionInfo.sParam9,
            QuestConditionInfo.sParam10);
        // 原文 9774-9786：整段（含唯一的 MainOutMessage）被 `{ }` 注释 —— 原文如此，保留，
        // 因此 sMsg 是**死值**；这里用 _ = 显式表达"计算了但不使用"。
        _ = sMsg;
    }
}
