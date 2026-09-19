namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J33：GameCommand.pas 管理员/调试命令表（AddAdminCommandToList/AddDebugCommandToList 全量条目）。
/// 默认串经 typed-constant 核对的有 AdminCmdOverrides/DebugCmdOverrides；未逐项核对者按名字派生
/// （全大写名 Pascal 化，其余原样），运行时以 CommandConf INI 载入为准。
/// </summary>
public static partial class GameCommandState
{
    /// <summary>已核对的默认命令串/权限覆盖（M2Share typed-constant 1:1；未列出者按名字派生默认）。</summary>
    public static readonly System.Collections.Generic.Dictionary<string, (string Cmd, int Min, int Max)> AdminCmdOverrides = new()
    {
        ["GAMEMASTER"] = ("GameMaster", 10, 10),
        ["OBSERVER"] = ("Observer", 10, 10),
        ["SUEPRMAN"] = ("Superman", 10, 10),
        ["KICK"] = ("Kick", 10, 10),
        ["TING"] = ("Ting", 10, 10),
        ["SUPERTING"] = ("SuperTing", 10, 10),
        ["MAPMOVE"] = ("MapMove", 10, 10),
        ["Level"] = ("Level", 10, 10),
        ["RECALL"] = ("Recall", 10, 10),
        ["REGOTO"] = ("ReGoto", 10, 10),
        ["HUMANLOCAL"] = ("HumanLocal", 3, 10),
        ["Move"] = ("Move", 3, 6),
        ["POSITIONMOVE"] = ("PositionMove", 3, 6),
        ["INFO"] = ("Info", 3, 10),
        ["MOBLEVEL"] = ("MobLevel", 3, 10),
        ["MOBCOUNT"] = ("MobCount", 3, 10),
        ["HUMANCOUNT"] = ("HumanCount", 3, 10),
        ["Map"] = ("Map", 3, 10),
        ["WHO"] = ("Who", 0, 10),
        ["TOTAL"] = ("Total", 0, 10),
        ["MAKE"] = ("Make", 0, 10),
        ["CLRPASSWORD"] = ("ClrPassword", 10, 10),
        ["VIEWWHISPER"] = ("ViewWhisper", 0, 10),
    };

    private static readonly System.Collections.Generic.Dictionary<string, string> DebugCmdOverrides = new()
    {
        ["SHOWFLAG"] = "showflag",
        ["SHOWOPEN"] = "showopen",
        ["SHOWUNIT"] = "showunit",
        ["MOBNPC"] = "MobNpc",
        ["DELNPC"] = "DelNpc",
        ["ReLoadNpc"] = "ReLoadNpc",
    };

    private static string DeriveDefaultCmd(string name)
        => name == name.ToUpperInvariant() && name.Length > 1
            ? char.ToUpperInvariant(name[0]) + name[1..].ToLowerInvariant()
            : name;

    public static readonly System.Collections.Generic.Dictionary<string, TGameCmd> AdminCmdByName = new();
    public static readonly System.Collections.Generic.Dictionary<string, TGameCmd> DebugCmdByName = new();

    static GameCommandState()
    {
        foreach (var (section, cmdName, _, _, _) in GameCommandTables.RawEntries)
        {
            var cmd = new TGameCmd();
            ApplyAdminDebugDefault(cmd, cmdName);
            if (section == "admin")
                AdminCmdByName[cmdName] = cmd;
            else
                DebugCmdByName[cmdName] = cmd;
        }
    }

    private static void ApplyAdminDebugDefault(TGameCmd cmd, string name)
    {
        if (AdminCmdOverrides.TryGetValue(name, out var ov))
        {
            cmd.sCmd = ov.Cmd;
            cmd.nPermissionMin = ov.Min;
            cmd.nPermissionMax = ov.Max;
            return;
        }
        cmd.sCmd = DeriveDefaultCmd(name);
    }

    /// <summary>AddAdminCommandToList 1:1（100 条命令，IsPermission 标记按原文）。</summary>
    public static List<GameCommandRow> BuildAdminCommandList()
    {
        var rows = new List<GameCommandRow>();
        var idx = 0;
        foreach (var (section, cmdName, isPermission, param, desc) in GameCommandTables.RawEntries)
        {
            if (section != "admin")
                continue;
            idx++;
            rows.Add(new GameCommandRow
            {
                Index = idx,
                IsPermission = isPermission == "True",
                Param = param,
                Desc = desc,
                Cmd = AdminCmdByName[cmdName],
            });
        }
        return rows;
    }

    /// <summary>AddDebugCommandToList 1:1（40 条命令）。</summary>
    public static List<GameCommandRow> BuildDebugCommandList()
    {
        var rows = new List<GameCommandRow>();
        var idx = 0;
        foreach (var (section, cmdName, _, param, desc) in GameCommandTables.RawEntries)
        {
            if (section != "debug")
                continue;
            idx++;
            rows.Add(new GameCommandRow
            {
                Index = idx,
                Param = param,
                Desc = desc,
                Cmd = DebugCmdByName[cmdName],
            });
        }
        return rows;
    }

    /// <summary>测试隔离：管理员/调试命令复位。</summary>
    public static void ResetAdminDebugForTests()
    {
        foreach (var (name, cmd) in AdminCmdByName)
            ApplyAdminDebugDefault(cmd, name);
        foreach (var (name, cmd) in DebugCmdByName)
            cmd.sCmd = DebugCmdOverrides.TryGetValue(name, out var d) ? d : DeriveDefaultCmd(name);
    }
}
