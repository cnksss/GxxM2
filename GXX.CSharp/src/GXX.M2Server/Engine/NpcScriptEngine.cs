using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>TNormNpc 最小模型（脚本宿主）。</summary>
public class TNormNpc
{
    public string m_sCharName;

    public TNormNpc(string name) { m_sCharName = name; }
}

/// <summary>脚本玩家状态（TPlayObject 脚本可见子集）。</summary>
public partial class TScriptPlayer
{
    public string m_sCharName = "";
    public uint m_wAbil_Level;

    // 兼容 TAbility 访问（Level 走 m_wAbil）
    public TAbility m_wAbil;

    public uint Level
    {
        get => m_wAbil.Level;
        set => m_wAbil.Level = value;
    }

    public byte m_btJob;
    public byte m_btGender;
    public bool m_boAdmin;
    public int Gold;
    public int GameGold;
    public int GamePoint;
    public int CreditPoint;
    public int BonusPoint;
    public int PKPoint;
    public long Exp;
    public bool m_boDeath;
    public bool Kicked;
    public readonly List<string> Skills = new();
    public readonly List<string> Messages = new();
    public readonly Dictionary<string, int> Items = new();
    public readonly int[] PVars = new int[100];    // P0..P99
    public readonly int[] GVars = new int[100];    // G0..G99
    public bool TimerRecallActive;
    public string? RecallMapName;
    public readonly HashSet<string> NameLists = new(StringComparer.OrdinalIgnoreCase);

    public int GetItemcount(string name) => Items.TryGetValue(name, out int v) ? v : 0;
    public bool HasItem(string name) => GetItemcount(name) > 0;

    public int GetPVar(int idx) => PVars[Math.Clamp(idx, 0, 99)];
    public void SetPVar(int idx, int v) => PVars[Math.Clamp(idx, 0, 99)] = v;
}

/// <summary>TQuestConditionInfo / TQuestActionInfo（脚本命令行解析结果）。</summary>
public class TScriptCmd
{
    public string Name = "";
    public int CmdCode;
    public string[] Params = Array.Empty<string>();

    public int GetInt(int idx, int def = 0)
        => idx < Params.Length && int.TryParse(Params[idx], out int v) ? v : def;

    public string GetStr(int idx)
        => idx < Params.Length ? Params[idx] : "";
}

/// <summary>脚本段（[@label] 下的 IF/ACT/SAY 块）。</summary>
public class TScriptSection
{
    public string Label = "";
    public List<TScriptCmd> Conditions = new();
    public List<TScriptCmd> Actions = new();
    public List<TScriptCmd> ElseActions = new();
    public List<string> SayLines = new();
    public List<string> ElseSayLines = new();
}

/// <summary>解析后的整个脚本文件。</summary>
public class NpcScript
{
    public string FileName = "";
    public Dictionary<string, TScriptSection> Labels = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>RunLabel 结果。</summary>
public class ScriptRunResult
{
    public bool ConditionsPassed;
    public bool ActionsAllOk = true;
    public bool Break;
    public bool Close;
    public string SayText = "";
}

/// <summary>
/// HandleNpcCmds.pas + NpcConditionCmd.pas + NpcActionCmd.pas 1:1 架构转换：
/// ConditionCmdArray/ActionCmdArray[1000] 代码分发 + 命令名→码表 + 脚本解析（[@label]/#IF/#ACT/#SAY/#ELSEACT/#ELSESAY）
/// + 条件/动作处理器（核心命令集）+ $变量文本替换。
/// </summary>
public class NpcScriptEngine
{
    // ---------------- 代码注册表（对应 initialization 段） ----------------

    public delegate bool TConditionCmd(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd);

    public delegate bool TActionCmd(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run);

    public static readonly TConditionCmd?[] ConditionCmdArray = new TConditionCmd?[1000];
    public static readonly TActionCmd?[] ActionCmdArray = new TActionCmd?[1000];

    /// <summary>命令名→码表（M2 脚本方言常用名）。</summary>
    public static readonly Dictionary<string, int> ConditionNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CHECK"] = NpcCmdCodes.nNC_CHECK,
        ["RANDOM"] = NpcCmdCodes.nNC_RANDOM,
        ["RANDOMEX"] = NpcCmdCodes.nNC_RANDOMEX,
        ["GENDER"] = NpcCmdCodes.nNC_GENDER,
        ["DAYTIME"] = NpcCmdCodes.nNC_DAYTIME,
        ["CHECKLEVEL"] = NpcCmdCodes.nNC_CHECKLEVEL,
        ["CHECKJOB"] = NpcCmdCodes.nNC_CHECKJOB,
        ["CHECKITEM"] = NpcCmdCodes.nNC_CHECKITEM,
        ["CHECKITEMW"] = NpcCmdCodes.nNC_CHECKITEMW,
        ["CHECKGOLD"] = NpcCmdCodes.nNC_CHECKGOLD,
        ["CHECKDURA"] = NpcCmdCodes.nNC_CHECKDURA,
        ["DAYOFWEEK"] = NpcCmdCodes.nNC_DAYOFWEEK,
        ["HOUR"] = NpcCmdCodes.nNC_HOUR,
        ["MIN"] = NpcCmdCodes.nNC_MIN,
        ["CHECKPKPOINT"] = NpcCmdCodes.nNC_CHECKPKPOINT,
        ["CHECKMONMAP"] = NpcCmdCodes.nNC_CHECKMONMAP,
        ["CHECKHUM"] = NpcCmdCodes.nNC_CHECKHUM,
        ["CHECKBAGGAGE"] = NpcCmdCodes.nNC_CHECKBAGGAGE,
        ["EQUAL"] = NpcCmdCodes.nNC_EQUAL,
        ["LARGE"] = NpcCmdCodes.nNC_LARGE,
        ["SMALL"] = NpcCmdCodes.nNC_SMALL,
        ["CHECKNAMELIST"] = NpcCmdCodes.nNC_CHECKNAMELIST,
        ["ISGUILDMASTER"] = NpcCmdCodes.nNC_ISGUILDMASTER,
        ["ISATTACKGUILD"] = NpcCmdCodes.nNC_ISATTACKGUILD,
        ["ISDEFENSEGUILD"] = NpcCmdCodes.nNC_ISDEFENSEGUILD,
        ["HASGUILD"] = NpcCmdCodes.nNC_HASGUILD,
        ["ISSYSOP"] = NpcCmdCodes.nNC_ISSYSOP,
        ["ISADMIN"] = NpcCmdCodes.nNC_ISADMIN,
        ["CHECKCREDITPOINT"] = NpcCmdCodes.nNC_CHECKCREDITPOINT,
        ["CHECKLEVELEX"] = NpcCmdCodes.nNC_CHECKLEVELEX,
        ["CHECKGAMEGOLD"] = NpcCmdCodes.nNC_CHECKGAMEGOLD,
        ["CHECKGAMEPOINT"] = NpcCmdCodes.nNC_CHECKGAMEPOINT,
        ["CHECKVAR"] = NpcCmdCodes.nNC_CHECKVAR,
        ["CHECKMAPNAME"] = NpcCmdCodes.nNC_CHECKMAPNAME,
        ["CHECKSKILL"] = NpcCmdCodes.nNC_CHECKSKILL,
        ["CHECKCONTAINSTEXT"] = NpcCmdCodes.nNC_CHECKCONTAINSTEXT,
        ["COMPARETEXT"] = NpcCmdCodes.nNC_COMPARETEXT,
        ["CHECKHP"] = NpcCmdCodes.nNC_CHECKHP,
        ["CHECKMP"] = NpcCmdCodes.nNC_CHECKMP,
        ["CHECKDC"] = NpcCmdCodes.nNC_CHECKDC,
        ["CHECKMC"] = NpcCmdCodes.nNC_CHECKMC,
        ["CHECKSC"] = NpcCmdCodes.nNC_CHECKSC,
        ["CHECKEXP"] = NpcCmdCodes.nNC_CHECKEXP,
        ["CHECKHPPER"] = NpcCmdCodes.nNC_CHECKHPPER,
        ["CHECKMPPER"] = NpcCmdCodes.nNC_CHECKMPPER,
        ["CHECKONHORSE"] = NpcCmdCodes.nNC_CHECKONHORSE,
        ["ISDUMMY"] = NpcCmdCodes.nNC_ISDUMMY,
        ["CHECKOFFLINE"] = NpcCmdCodes.nNC_CHECKOFFLINE
    };

    public static readonly Dictionary<string, int> ActionNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SET"] = NpcCmdCodes.nNA_SET,
        ["TAKE"] = NpcCmdCodes.nNA_TAKE,
        ["GIVE"] = NpcCmdCodes.nNA_GIVE,
        ["TAKEW"] = NpcCmdCodes.nNA_TAKEW,
        ["CLOSE"] = NpcCmdCodes.nNA_CLOSE,
        ["BREAK"] = NpcCmdCodes.nNA_BREAK,
        ["TIMERECALL"] = NpcCmdCodes.nNA_TIMERECALL,
        ["BREAKTIMERECALL"] = NpcCmdCodes.nNA_BREAKTIMERECALL,
        ["MAPMOVE"] = NpcCmdCodes.nNA_MAPMOVE,
        ["MAP"] = NpcCmdCodes.nNA_MAP,
        ["MONGEN"] = NpcCmdCodes.nNA_MONGEN,
        ["MONCLEAR"] = NpcCmdCodes.nNA_MONCLEAR,
        ["MOV"] = NpcCmdCodes.nNA_MOV,
        ["INC"] = NpcCmdCodes.nNA_INC,
        ["DEC"] = NpcCmdCodes.nNA_DEC,
        ["DIV"] = NpcCmdCodes.nNA_DIV,
        ["MUL"] = NpcCmdCodes.nNA_MUL,
        ["PERCENT"] = NpcCmdCodes.nNA_PERCENT,
        ["SENDMSG"] = NpcCmdCodes.nNA_SENDMSG,
        ["PKPOINT"] = NpcCmdCodes.nNA_PKPOINT,
        ["MOVR"] = NpcCmdCodes.nNA_MOVR,
        ["PLAYDICE"] = NpcCmdCodes.nNA_PLAYDICE,
        ["ADDNAMELIST"] = NpcCmdCodes.nNA_ADDNAMELIST,
        ["DELNAMELIST"] = NpcCmdCodes.nNA_DELNAMELIST,
        ["GOTO"] = NpcCmdCodes.nNA_GOTO,
        ["CLEARNAMELIST"] = NpcCmdCodes.nNA_CLEARNAMELIST,
        ["KILLSLAVE"] = NpcCmdCodes.nNA_KILLSLAVE,
        ["CHANGELEVEL"] = NpcCmdCodes.nNA_CHANGELEVEL,
        ["DELSKILL"] = NpcCmdCodes.nNA_DELSKILL,
        ["ADDSKILL"] = NpcCmdCodes.nNA_ADDSKILL,
        ["SKILLLEVEL"] = NpcCmdCodes.nNA_SKILLLEVEL,
        ["CHANGEPKPOINT"] = NpcCmdCodes.nNA_CHANGEPKPOINT,
        ["CHANGEEXP"] = NpcCmdCodes.nNA_CHANGEEXP,
        ["GIVEEXP"] = NpcCmdCodes.nNA_CHANGEEXP,
        ["CHANGEJOB"] = NpcCmdCodes.nNA_CHANGEJOB,
        ["GAMEGOLD"] = NpcCmdCodes.nNA_GAMEGOLD,
        ["GAMEPOINT"] = NpcCmdCodes.nNA_GAMEPOINT,
        ["RENEWLEVEL"] = NpcCmdCodes.nNA_RENEWLEVEL,
        ["KILLMONEXPRATE"] = NpcCmdCodes.nNA_KILLMONEXPRATE,
        ["POWERRATE"] = NpcCmdCodes.nNA_POWERRATE,
        ["CHANGEPERMISSION"] = NpcCmdCodes.nNA_CHANGEPERMISSION,
        ["KILL"] = NpcCmdCodes.nNA_KILL,
        ["KICK"] = NpcCmdCodes.nNA_KICK,
        ["BONUSPOINT"] = NpcCmdCodes.nNA_BONUSPOINT,
        ["CREDITPOINT"] = NpcCmdCodes.nNA_CREDITPOINT,
        ["HUMANHP"] = NpcCmdCodes.nNA_HUMANHP,
        ["HUMANMP"] = NpcCmdCodes.nNA_HUMANMP,
        ["VAR"] = NpcCmdCodes.nNA_VAR,
        ["CALCVAR"] = NpcCmdCodes.nNA_CALCVAR,
        ["OFFLINE"] = NpcCmdCodes.nNA_OFFLINE,
        ["MESSAGEBOX"] = NpcCmdCodes.nNA_MESSAGEBOX,
        ["GAMEDIAMOND"] = NpcCmdCodes.nNA_GAMEDIAMOND
    };

    static NpcScriptEngine()
    {
        RegisterCommands();
        // 批次I：合并 NpcCommon.pas g_Cmd*List 全量名称表（278 条件 + 728 动作）
        foreach (var kv in NpcCmdNames.ConditionNames)
            ConditionNames[kv.Key] = kv.Value;
        foreach (var kv in NpcCmdNames.ActionNames)
            ActionNames[kv.Key] = kv.Value;
        // 注册批次I 余下处理器（状态级实现 + 状态级等效桩）
        NpcScriptCommands.RegisterAll();
    }

    private static void RegC(int code, TConditionCmd handler) => ConditionCmdArray[code] = handler;
    private static void RegA(int code, TActionCmd handler) => ActionCmdArray[code] = handler;

    /// <summary>对应 NpcConditionCmd/NpcActionCmd 的 initialization 段注册。</summary>
    private static void RegisterCommands()
    {
        // ---- 条件 ----
        RegC(NpcCmdCodes.nNC_CHECK, CmdCheck);
        RegC(NpcCmdCodes.nNC_RANDOM, CmdRandom);
        RegC(NpcCmdCodes.nNC_RANDOMEX, CmdRandomEx);
        RegC(NpcCmdCodes.nNC_GENDER, CmdGender);
        RegC(NpcCmdCodes.nNC_CHECKLEVEL, CmdCheckLevel);
        RegC(NpcCmdCodes.nNC_CHECKJOB, CmdCheckJob);
        RegC(NpcCmdCodes.nNC_CHECKITEM, CmdCheckItem);
        RegC(NpcCmdCodes.nNC_CHECKGOLD, CmdCheckGold);
        RegC(NpcCmdCodes.nNC_CHECKPKPOINT, CmdCheckPkPoint);
        RegC(NpcCmdCodes.nNC_EQUAL, CmdEqual);
        RegC(NpcCmdCodes.nNC_LARGE, CmdLarge);
        RegC(NpcCmdCodes.nNC_SMALL, CmdSmall);
        RegC(NpcCmdCodes.nNC_ISSYSOP, CmdIsAdmin);
        RegC(NpcCmdCodes.nNC_ISADMIN, CmdIsAdmin);
        RegC(NpcCmdCodes.nNC_CHECKGAMEGOLD, CmdCheckGameGold);
        RegC(NpcCmdCodes.nNC_CHECKGAMEPOINT, CmdCheckGamePoint);
        RegC(NpcCmdCodes.nNC_CHECKVAR, CmdCheckVar);
        RegC(NpcCmdCodes.nNC_CHECKHP, CmdCheckHp);
        RegC(NpcCmdCodes.nNC_CHECKMP, CmdCheckMp);
        RegC(NpcCmdCodes.nNC_CHECKSKILL, CmdCheckSkill);
        RegC(NpcCmdCodes.nNC_CHECKCONTAINSTEXT, CmdContainsText);
        RegC(NpcCmdCodes.nNC_COMPARETEXT, CmdCompareText);

        // ---- 动作 ----
        RegA(NpcCmdCodes.nNA_SET, CmdSet);
        RegA(NpcCmdCodes.nNA_TAKE, CmdTake);
        RegA(NpcCmdCodes.nNA_GIVE, CmdGive);
        RegA(NpcCmdCodes.nNA_CLOSE, CmdClose);
        RegA(NpcCmdCodes.nNA_BREAK, CmdBreak);
        RegA(NpcCmdCodes.nNA_TIMERECALL, CmdTimeRecall);
        RegA(NpcCmdCodes.nNA_BREAKTIMERECALL, CmdBreakTimeRecall);
        RegA(NpcCmdCodes.nNA_MAPMOVE, CmdMapMove);
        RegA(NpcCmdCodes.nNA_MOV, CmdMov);
        RegA(NpcCmdCodes.nNA_INC, CmdInc);
        RegA(NpcCmdCodes.nNA_DEC, CmdDec);
        RegA(NpcCmdCodes.nNA_MUL, CmdMul);
        RegA(NpcCmdCodes.nNA_DIV, CmdDiv);
        RegA(NpcCmdCodes.nNA_PERCENT, CmdPercent);
        RegA(NpcCmdCodes.nNA_SENDMSG, CmdSendMsg);
        RegA(NpcCmdCodes.nNA_PKPOINT, CmdPkPoint);
        RegA(NpcCmdCodes.nNA_ADDNAMELIST, CmdAddNameList);
        RegA(NpcCmdCodes.nNA_DELNAMELIST, CmdDelNameList);
        RegA(NpcCmdCodes.nNA_CHANGELEVEL, CmdChangeLevel);
        RegA(NpcCmdCodes.nNA_DELSKILL, CmdDelSkill);
        RegA(NpcCmdCodes.nNA_ADDSKILL, CmdAddSkill);
        RegA(NpcCmdCodes.nNA_CHANGEPKPOINT, CmdChangePkPoint);
        RegA(NpcCmdCodes.nNA_CHANGEEXP, CmdChangeExp);
        RegA(NpcCmdCodes.nNA_CHANGEJOB, CmdChangeJob);
        RegA(NpcCmdCodes.nNA_GAMEGOLD, CmdGameGold);
        RegA(NpcCmdCodes.nNA_GAMEPOINT, CmdGamePoint);
        RegA(NpcCmdCodes.nNA_KILL, CmdKill);
        RegA(NpcCmdCodes.nNA_KICK, CmdKick);
        RegA(NpcCmdCodes.nNA_BONUSPOINT, CmdBonusPoint);
        RegA(NpcCmdCodes.nNA_CREDITPOINT, CmdCreditPoint);
        RegA(NpcCmdCodes.nNA_HUMANHP, CmdHumanHp);
        RegA(NpcCmdCodes.nNA_HUMANMP, CmdHumanMp);
    }

    public static int ConditionNameToCode(string name)
        => ConditionNames.TryGetValue(name, out int v) ? v : 0;

    public static int ActionNameToCode(string name)
        => ActionNames.TryGetValue(name, out int v) ? v : 0;

    // ---------------- 脚本解析 ----------------

    public static NpcScript ParseScript(string text, string fileName = "")
    {
        var script = new NpcScript { FileName = fileName };
        TScriptSection? cur = null;
        string block = "";   // "", "IF", "ACT", "SAY", "ELSEACT", "ELSESAY"

        foreach (var rawLine in text.Replace("\r\n", "\n").Split('\n'))
        {
            string line = rawLine.Trim();
            if (line.Length == 0)
            {
                if (cur != null && block == "SAY") cur.SayLines.Add("");
                if (cur != null && block == "ELSESAY") cur.ElseSayLines.Add("");
                continue;
            }
            if (line.StartsWith(';') || line.StartsWith("//")) continue;

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                string label = line.Substring(1, line.Length - 2).Trim();
                if (!script.Labels.TryGetValue(label, out var sec))
                {
                    sec = new TScriptSection { Label = label };
                    script.Labels[label] = sec;
                }
                cur = sec;
                block = "";
                continue;
            }

            if (cur == null) continue;

            if (line.Equals("#IF", StringComparison.OrdinalIgnoreCase) || line.Equals("#OR", StringComparison.OrdinalIgnoreCase)) { block = "IF"; continue; }
            if (line.Equals("#ACT", StringComparison.OrdinalIgnoreCase)) { block = "ACT"; continue; }
            if (line.Equals("#ELSEACT", StringComparison.OrdinalIgnoreCase)) { block = "ELSEACT"; continue; }
            if (line.Equals("#SAY", StringComparison.OrdinalIgnoreCase)) { block = "SAY"; continue; }
            if (line.Equals("#ELSESAY", StringComparison.OrdinalIgnoreCase)) { block = "ELSESAY"; continue; }

            switch (block)
            {
                case "IF":
                {
                    var cmd = ParseCmd(line);
                    if (cmd != null) cur.Conditions.Add(cmd);
                    break;
                }
                case "ACT":
                {
                    var cmd = ParseCmd(line);
                    if (cmd != null) cur.Actions.Add(cmd);
                    break;
                }
                case "ELSEACT":
                {
                    var cmd = ParseCmd(line);
                    if (cmd != null) cur.ElseActions.Add(cmd);
                    break;
                }
                case "SAY":
                    cur.SayLines.Add(line);
                    break;
                case "ELSESAY":
                    cur.ElseSayLines.Add(line);
                    break;
            }
        }
        return script;
    }

    private static TScriptCmd? ParseCmd(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return null;
        var cmd = new TScriptCmd { Name = parts[0] };
        cmd.Params = new string[parts.Length - 1];
        Array.Copy(parts, 1, cmd.Params, 0, parts.Length - 1);
        cmd.CmdCode = ConditionNameToCode(cmd.Name);
        if (cmd.CmdCode == 0)
            cmd.CmdCode = ActionNameToCode(cmd.Name);
        return cmd;
    }

    // ---------------- 条件执行（QuestCheckCondition） ----------------

    public bool CheckConditionText(TNormNpc npc, TScriptPlayer user, string line)
    {
        var cmd = ParseCmd(line);
        if (cmd == null) return false;
        return QuestCheckCondition(npc, user, new List<TScriptCmd> { cmd });
    }

    public bool RunActionText(TNormNpc npc, TScriptPlayer user, string line)
    {
        var cmd = ParseCmd(line);
        if (cmd == null) return false;
        var run = new ScriptRunResult();
        return QuestActionProcess(npc, "", user, new List<TScriptCmd> { cmd }, ref run);
    }

    /// <summary>对应 QuestCheckCondition：全部条件 AND 语义。</summary>
    public bool QuestCheckCondition(TNormNpc npc, TScriptPlayer user, List<TScriptCmd> conditions)
    {
        foreach (var cmd in conditions)
        {
            var handler = cmd.CmdCode > 0 && cmd.CmdCode < ConditionCmdArray.Length ? ConditionCmdArray[cmd.CmdCode] : null;
            if (handler == null || !handler(npc, user, cmd))
                return false;
        }
        return true;
    }

    /// <summary>对应 QuestActionProcess：顺序执行动作链。</summary>
    public bool QuestActionProcess(TNormNpc npc, string sLabel, TScriptPlayer user, List<TScriptCmd> actions, ref ScriptRunResult run)
    {
        bool allOk = true;
        foreach (var cmd in actions)
        {
            if (run.Break) break;
            var handler = cmd.CmdCode > 0 && cmd.CmdCode < ActionCmdArray.Length ? ActionCmdArray[cmd.CmdCode] : null;
            if (handler == null)
            {
                allOk = false;   // 未实现/未知命令
                continue;
            }
            if (!handler(npc, user, cmd, run))
                allOk = false;
        }
        return allOk;
    }

    // ---------------- RunLabel（对应 TNormNpc.GotoLabel 语义） ----------------

    public ScriptRunResult RunLabel(NpcScript script, string label, TNormNpc npc, TScriptPlayer user)
    {
        var run = new ScriptRunResult();
        if (!script.Labels.TryGetValue(label, out var sec))
        {
            run.SayText = "";
            return run;
        }

        run.ConditionsPassed = QuestCheckCondition(npc, user, sec.Conditions);
        if (run.ConditionsPassed)
        {
            run.ActionsAllOk = QuestActionProcess(npc, label, user, sec.Actions, ref run);
            run.SayText = BuildSay(npc, user, sec.SayLines);
        }
        else
        {
            QuestActionProcess(npc, label, user, sec.ElseActions, ref run);
            run.SayText = BuildSay(npc, user, sec.ElseSayLines);
        }
        return run;
    }

    // ---------------- $变量文本替换（对应 GetLineVariableText） ----------------

    public string BuildSay(TNormNpc npc, TScriptPlayer user, List<string> lines)
    {
        var sb = new StringBuilder();
        foreach (var line in lines)
        {
            sb.AppendLine(ReplaceVars(line, user));
        }
        return sb.ToString().TrimEnd();
    }

    public string ReplaceVars(string text, TScriptPlayer user)
    {
        string s = text;
        s = s.Replace("$USERNAME", user.m_sCharName);
        s = s.Replace("$LEVEL", user.Level.ToString());
        s = s.Replace("$GOLD", user.Gold.ToString());
        s = s.Replace("$GAMEGOLD", user.GameGold.ToString());
        s = s.Replace("$GAMEPOINT", user.GamePoint.ToString());
        s = s.Replace("$JOB", JobName(user.m_btJob));
        s = s.Replace("$PKPOINT", user.PKPoint.ToString());
        s = s.Replace("$CREDITPOINT", user.CreditPoint.ToString());
        // <$STR(P0)> / <$STR(G1)>
        int idx;
        while ((idx = s.IndexOf("<$STR(", StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            int end = s.IndexOf(')', idx);
            if (end < 0) break;
            string varName = s.Substring(idx + 6, end - idx - 6);
            s = s.Substring(0, idx) + ReadVar(user, varName).ToString() + s.Substring(end + 1);
        }
        return s;
    }

    public static string JobName(byte job) => job switch
    {
        0 => "Warrior",
        1 => "Wizard",
        2 => "Taoist",
        _ => "Unknown"
    };

    // ---------------- 变量读写（P0..P99 / G0..G99） ----------------

    public static bool TryParseVarName(string name, out bool isG, out int idx)
    {
        isG = false;
        idx = -1;
        if (string.IsNullOrEmpty(name)) return false;
        if (name.Length < 2) return false;
        if (name[0] is 'P' or 'p') { isG = false; }
        else if (name[0] is 'G' or 'g') { isG = true; }
        else return false;
        return int.TryParse(name.Substring(1), out idx);
    }

    public static int ReadVar(TScriptPlayer user, string name)
    {
        if (TryParseVarName(name, out bool isG, out int idx))
            return isG ? user.GVars[Math.Clamp(idx, 0, 99)] : user.PVars[Math.Clamp(idx, 0, 99)];
        return 0;
    }

    public static void WriteVar(TScriptPlayer user, string name, int value)
    {
        if (TryParseVarName(name, out bool isG, out int idx))
        {
            idx = Math.Clamp(idx, 0, 99);
            if (isG) user.GVars[idx] = value;
            else user.PVars[idx] = value;
        }
    }

    // ================= 条件处理器（NpcConditionCmd.pas 对应） =================

    private static bool CmdCheck(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        // ConditionOfCheck：任务标记位（此处以 P 变量模拟 QuestFlag）
        int flag = cmd.GetInt(0, -1);
        int expect = cmd.GetInt(1, -1);
        if (flag < 0 || expect < 0) return false;
        if (expect != 0) expect = 1;
        return (user.GetPVar(flag % 100) != 0 ? 1 : 0) == expect;
    }

    private static bool CmdRandom(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        int n = cmd.GetInt(0, 0);
        if (n <= 0) return false;
        return Rnd2.Next(n) == 0;
    }

    private static bool CmdRandomEx(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        // RANDOMEX 抽签概率：A/B 命中（RANDOMEX 30 100 → 30% 概率）
        int a = cmd.GetInt(0), b = cmd.GetInt(1);
        if (b <= 0) return false;
        return Rnd2.Next(b) < a;
    }

    private static bool CmdGender(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        string g = cmd.GetStr(0).ToUpperInvariant();
        return g switch
        {
            "MAN" or "男" => user.m_btGender == 0,
            "WOMAN" or "女" => user.m_btGender == 1,
            _ => false
        };
    }

    private static bool CmdCheckLevel(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        int min = cmd.GetInt(0);
        int max = cmd.GetInt(1, 0);
        uint level = user.Level;
        if (level < (uint)min) return false;
        if (max > 0 && level > (uint)max) return false;
        return true;
    }

    private static bool CmdCheckJob(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        string j = cmd.GetStr(0).ToUpperInvariant();
        return j switch
        {
            "WARRIOR" or "战士" => user.m_btJob == 0,
            "WIZARD" or "法师" => user.m_btJob == 1,
            "TAOIST" or "道士" => user.m_btJob == 2,
            _ => false
        };
    }

    private static bool CmdCheckItem(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        string item = cmd.GetStr(0);
        int count = cmd.GetInt(1, 1);
        return user.GetItemcount(item) >= count;
    }

    private static bool CmdCheckGold(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => user.Gold >= cmd.GetInt(0);

    private static bool CmdCheckPkPoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        int min = cmd.GetInt(0);
        int max = cmd.GetInt(1, 0);
        if (max > 0) return user.PKPoint >= min && user.PKPoint <= max;
        return user.PKPoint >= min;
    }

    private static bool CmdEqual(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => ReadVar(user, cmd.GetStr(0)) == cmd.GetInt(1);

    private static bool CmdLarge(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => ReadVar(user, cmd.GetStr(0)) > cmd.GetInt(1);

    private static bool CmdSmall(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => ReadVar(user, cmd.GetStr(0)) < cmd.GetInt(1);

    private static bool CmdIsAdmin(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => user.m_boAdmin;

    private static bool CmdCheckGameGold(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        int count = cmd.GetInt(0);
        int op = cmd.GetInt(1, 0);   // 0:>= 1:<= 2:==
        return op switch
        {
            1 => user.GameGold <= count,
            2 => user.GameGold == count,
            _ => user.GameGold >= count
        };
    }

    private static bool CmdCheckGamePoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        int count = cmd.GetInt(0);
        int op = cmd.GetInt(1, 0);
        return op switch
        {
            1 => user.GamePoint <= count,
            2 => user.GamePoint == count,
            _ => user.GamePoint >= count
        };
    }

    private static bool CmdCheckVar(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
    {
        // CHECKVAR 变量 类型(0:INTEGER) 操作符(> < =) 值
        string varName = cmd.GetStr(0);
        string op = cmd.GetStr(2);
        int value = cmd.GetInt(3);
        int v = ReadVar(user, varName);
        return op switch
        {
            ">" => v > value,
            "<" => v < value,
            "=" => v == value,
            ">=" => v >= value,
            "<=" => v <= value,
            _ => false
        };
    }

    private static bool CmdCheckHp(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => user.m_wAbil.HP >= (uint)cmd.GetInt(0);

    private static bool CmdCheckMp(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => user.m_wAbil.MP >= (uint)cmd.GetInt(0);

    private static bool CmdCheckSkill(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => user.Skills.Contains(cmd.GetStr(0), StringComparer.OrdinalIgnoreCase);

    private static bool CmdContainsText(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => cmd.GetStr(0).Contains(cmd.GetStr(1), StringComparison.OrdinalIgnoreCase);

    private static bool CmdCompareText(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd)
        => string.Equals(cmd.GetStr(0), cmd.GetStr(1), StringComparison.OrdinalIgnoreCase);

    // ================= 动作处理器（NpcActionCmd.pas 对应） =================

    private static bool CmdSet(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        // SET 标记 值：任务标记
        int flag = cmd.GetInt(0);
        int value = cmd.GetInt(1, 1);
        user.SetPVar(flag % 100, value);
        return true;
    }

    private static bool CmdGive(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string item = cmd.GetStr(0);
        int count = cmd.GetInt(1, 1);
        if (item == "" || count <= 0) return false;
        user.Items[item] = user.GetItemcount(item) + count;
        return true;
    }

    private static bool CmdTake(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string item = cmd.GetStr(0);
        int count = cmd.GetInt(1, 1);
        if (user.GetItemcount(item) < count) return false;
        user.Items[item] -= count;
        return true;
    }

    private static bool CmdClose(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        run.Close = true;
        return true;
    }

    private static bool CmdBreak(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        run.Break = true;
        return true;
    }

    private static bool CmdTimeRecall(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.TimerRecallActive = true;
        user.RecallMapName = cmd.GetStr(1);
        return true;
    }

    private static bool CmdBreakTimeRecall(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.TimerRecallActive = false;
        return true;
    }

    private static bool CmdMapMove(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        // MAPMOVE 地图 X Y：地图存在性由引擎地图表校验；此处记录目标（世界模型在 UsrEngn 集成）
        string map = cmd.GetStr(0);
        if (map == "") return false;
        user.RecallMapName = map;
        return true;
    }

    private static bool CmdMov(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        WriteVar(user, cmd.GetStr(0), cmd.GetInt(1));
        return true;
    }

    private static bool CmdInc(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string name = cmd.GetStr(0);
        WriteVar(user, name, ReadVar(user, name) + cmd.GetInt(1));
        return true;
    }

    private static bool CmdDec(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string name = cmd.GetStr(0);
        WriteVar(user, name, ReadVar(user, name) - cmd.GetInt(1));
        return true;
    }

    private static bool CmdMul(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string name = cmd.GetStr(0);
        WriteVar(user, name, ReadVar(user, name) * cmd.GetInt(1));
        return true;
    }

    private static bool CmdDiv(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string name = cmd.GetStr(0);
        int d = cmd.GetInt(1);
        if (d == 0) return false;
        WriteVar(user, name, ReadVar(user, name) / d);
        return true;
    }

    private static bool CmdPercent(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string name = cmd.GetStr(0);
        int p = cmd.GetInt(1);
        WriteVar(user, name, ReadVar(user, name) * p / 100);
        return true;
    }

    private static bool CmdSendMsg(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.Messages.Add(ReplaceVarsStatic(cmd.GetStr(0), user));
        return true;
    }

    private static bool CmdPkPoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        user.PKPoint = op == "-" ? Math.Max(0, user.PKPoint - v) : user.PKPoint + v;
        return true;
    }

    private static bool CmdChangePkPoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
        => CmdPkPoint(npc, user, cmd, run);

    private static bool CmdAddNameList(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.NameLists.Add(user.m_sCharName);
        return true;
    }

    private static bool CmdDelNameList(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.NameLists.Remove(user.m_sCharName);
        return true;
    }

    private static bool CmdChangeLevel(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.Level = (uint)Math.Max(1, cmd.GetInt(0));
        return true;
    }

    private static bool CmdDelSkill(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
        => user.Skills.RemoveAll(s => s.Equals(cmd.GetStr(0), StringComparison.OrdinalIgnoreCase)) > 0;

    private static bool CmdAddSkill(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string skill = cmd.GetStr(0);
        if (skill == "" || user.Skills.Contains(skill, StringComparer.OrdinalIgnoreCase)) return false;
        user.Skills.Add(skill);
        return true;
    }

    private static bool CmdChangeExp(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        long v = cmd.GetInt(1);
        user.Exp = op == "-" ? Math.Max(0, user.Exp - v) : user.Exp + v;
        return true;
    }

    private static bool CmdChangeJob(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string j = cmd.GetStr(0).ToUpperInvariant();
        user.m_btJob = j switch
        {
            "WARRIOR" or "战士" => 0,
            "WIZARD" or "法师" => 1,
            "TAOIST" or "道士" => 2,
            _ => user.m_btJob
        };
        return true;
    }

    private static bool CmdGameGold(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        user.GameGold = op == "-" ? Math.Max(0, user.GameGold - v) : user.GameGold + v;
        return true;
    }

    private static bool CmdGamePoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        user.GamePoint = op == "-" ? Math.Max(0, user.GamePoint - v) : user.GamePoint + v;
        return true;
    }

    private static bool CmdKill(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.m_boDeath = true;
        return true;
    }

    private static bool CmdKick(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        user.Kicked = true;
        return true;
    }

    private static bool CmdBonusPoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        user.BonusPoint = op == "-" ? Math.Max(0, user.BonusPoint - v) : user.BonusPoint + v;
        return true;
    }

    private static bool CmdCreditPoint(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        user.CreditPoint = op == "-" ? Math.Max(0, user.CreditPoint - v) : user.CreditPoint + v;
        return true;
    }

    private static bool CmdHumanHp(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        if (op == "=")
            user.m_wAbil.HP = (uint)Math.Clamp(v, 0, (long)user.m_wAbil.MaxHP);
        else if (op == "-")
            user.m_wAbil.HP = (uint)Math.Max(0, user.m_wAbil.HP - v);
        else
            user.m_wAbil.HP = Math.Min(user.m_wAbil.MaxHP, user.m_wAbil.HP + (uint)v);
        return true;
    }

    private static bool CmdHumanMp(TNormNpc npc, TScriptPlayer user, TScriptCmd cmd, ScriptRunResult run)
    {
        string op = cmd.GetStr(0);
        int v = cmd.GetInt(1);
        if (op == "=")
            user.m_wAbil.MP = (uint)Math.Clamp(v, 0, (long)user.m_wAbil.MaxMP);
        else if (op == "-")
            user.m_wAbil.MP = (uint)Math.Max(0, user.m_wAbil.MP - v);
        else
            user.m_wAbil.MP = Math.Min(user.m_wAbil.MaxMP, user.m_wAbil.MP + (uint)v);
        return true;
    }

    private static string ReplaceVarsStatic(string text, TScriptPlayer user)
    {
        return text
            .Replace("$USERNAME", user.m_sCharName)
            .Replace("$LEVEL", user.Level.ToString())
            .Replace("$GOLD", user.Gold.ToString());
    }

    private static readonly Random Rnd2 = new();
}
