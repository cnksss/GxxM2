namespace GXX.M2Server.Engine;

/// <summary>Delphi TGameCmd（游戏命令：命令串 + 最小/最大权限）。</summary>
public class TGameCmd
{
    public string sCmd = "";
    public int nPermissionMin;
    public int nPermissionMax = 10;
}

/// <summary>命令列表行（Delphi vstUserCmd NodeData 等效：组标题行或命令行）。</summary>
public class GameCommandRow
{
    public int Index;
    public bool IsGroup;
    public bool IsPermission;
    public string GroupText = "";
    public string Param = "";
    public string Desc = "";
    public TGameCmd? Cmd;
}

/// <summary>
/// 批次J32：GameCommand.pas g_GameCommand 命令注册表（M2Share typed-constant 初值 1:1，
/// nPermissionMax 统一 10）。本批先迁用户命令表（AddUserCommandToList 45 条），
/// 管理员/调试命令表随余页接入。
/// </summary>
public static partial class GameCommandState
{
    /// <summary>Delphi g_GameCommand 命令记录（用户命令全集）。</summary>
    public static class g_GameCommand
    {
        public static readonly TGameCmd Data = new() { sCmd = "Date" };
        public static readonly TGameCmd PRVMSG = new() { sCmd = "PrvMsg" };
        public static readonly TGameCmd ALLOWMSG = new() { sCmd = "AllowMsg" };
        public static readonly TGameCmd LETSHOUT = new() { sCmd = "LetShout" };
        public static readonly TGameCmd LETTRADE = new() { sCmd = "LetTrade" };
        public static readonly TGameCmd LETGUILD = new() { sCmd = "LetGuild" };
        public static readonly TGameCmd LETCHALLENGE = new() { sCmd = "LetChallenge" };
        public static readonly TGameCmd ENDGUILD = new() { sCmd = "EndGuild" };
        public static readonly TGameCmd BANGUILDCHAT = new() { sCmd = "BanGuildChat" };
        public static readonly TGameCmd BANNATIONCHAT = new() { sCmd = "允许国家聊天" };
        public static readonly TGameCmd AUTHALLY = new() { sCmd = "AuthAlly" };
        public static readonly TGameCmd AUTH = new() { sCmd = "联盟" };
        public static readonly TGameCmd AUTHCANCEL = new() { sCmd = "取消联盟" };
        public static readonly TGameCmd DIARY = new() { sCmd = "Diary" };
        public static readonly TGameCmd USERMOVE = new() { sCmd = "Move" };
        public static readonly TGameCmd SEARCHING = new() { sCmd = "Searching" };
        public static readonly TGameCmd ALLOWGROUPCALL = new() { sCmd = "AllowGroupRecall" };
        public static readonly TGameCmd GROUPRECALLL = new() { sCmd = "GroupRecall" };
        public static readonly TGameCmd AllowGuildReCall = new() { sCmd = "AllowGuildRecall" };
        public static readonly TGameCmd GUILDRECALLL = new() { sCmd = "GuildRecall" };
        public static readonly TGameCmd UNLOCKSTORAGE = new() { sCmd = "UnLockStorage" };
        public static readonly TGameCmd UNLOCK = new() { sCmd = "UnLock" };
        public static readonly TGameCmd LOCKLOGON = new() { sCmd = "LockLogin" };
        public static readonly TGameCmd Lock = new() { sCmd = "Lock" };
        public static readonly TGameCmd SETPASSWORD = new() { sCmd = "SetPassword" };
        public static readonly TGameCmd CHGPASSWORD = new() { sCmd = "ChgPassword" };
        public static readonly TGameCmd UNPASSWORD = new() { sCmd = "UnPassword" };
        public static readonly TGameCmd MEMBERFUNCTION = new() { sCmd = "MemberFunc" };
        public static readonly TGameCmd MEMBERFUNCTIONEX = new() { sCmd = "MemberFuncEx" };
        public static readonly TGameCmd DEAR = new() { sCmd = "Dear" };
        public static readonly TGameCmd ALLOWDEARRCALL = new() { sCmd = "AllowDearRecall" };
        public static readonly TGameCmd DEARRECALL = new() { sCmd = "DearRecall" };
        public static readonly TGameCmd MASTER = new() { sCmd = "Master" };
        public static readonly TGameCmd ALLOWMASTERRECALL = new() { sCmd = "AllowMasterRecall" };
        public static readonly TGameCmd MASTERECALL = new() { sCmd = "MasterRecall" };
        public static readonly TGameCmd ATTACKMODE = new() { sCmd = "AttackMode" };
        public static readonly TGameCmd REST = new() { sCmd = "Rest" };
        public static readonly TGameCmd RESTHERO = new() { sCmd = "RestHero" };
        public static readonly TGameCmd TAKEONHORSE = new() { sCmd = "骑马" };
        public static readonly TGameCmd TAKEOFHORSE = new() { sCmd = "下马" };
        public static readonly TGameCmd DISABLEHORSEINVITE = new() { sCmd = "禁止邀请上马" };
        public static readonly TGameCmd PUBLICCHAT = new() { sCmd = "PublicChat" };
        public static readonly TGameCmd CRYCHAT = new() { sCmd = "CryChat" };
        public static readonly TGameCmd OpenSellPlayer = new() { sCmd = "角色交易" };
        public static readonly TGameCmd SendTopChatBoardMsg = new() { sCmd = "SendTopChatBoardMsg" };
        public static readonly TGameCmd LETNATION = new() { sCmd = "加入国家" };

        public static readonly TGameCmd[] All =
        {
            Data, PRVMSG, ALLOWMSG, LETSHOUT, LETTRADE, LETGUILD, LETCHALLENGE, ENDGUILD, BANGUILDCHAT,
            BANNATIONCHAT, AUTHALLY, AUTH, AUTHCANCEL, DIARY, USERMOVE, SEARCHING, ALLOWGROUPCALL,
            GROUPRECALLL, AllowGuildReCall, GUILDRECALLL, UNLOCKSTORAGE, UNLOCK, LOCKLOGON, Lock,
            SETPASSWORD, CHGPASSWORD, UNPASSWORD, MEMBERFUNCTION, MEMBERFUNCTIONEX, DEAR, ALLOWDEARRCALL,
            DEARRECALL, MASTER, ALLOWMASTERRECALL, MASTERECALL, ATTACKMODE, REST, RESTHERO, TAKEONHORSE,
            TAKEOFHORSE, DISABLEHORSEINVITE, PUBLICCHAT, CRYCHAT, OpenSellPlayer, SendTopChatBoardMsg, LETNATION,
        };

        public static readonly string[] DefaultCmds =
        {
            "Date", "PrvMsg", "AllowMsg", "LetShout", "LetTrade", "LetGuild", "LetChallenge", "EndGuild",
            "BanGuildChat", "允许国家聊天", "AuthAlly", "联盟", "取消联盟", "Diary", "Move", "Searching",
            "AllowGroupRecall", "GroupRecall", "AllowGuildRecall", "GuildRecall", "UnLockStorage", "UnLock",
            "LockLogin", "Lock", "SetPassword", "ChgPassword", "UnPassword", "MemberFunc", "MemberFuncEx",
            "Dear", "AllowDearRecall", "DearRecall", "Master", "AllowMasterRecall", "MasterRecall",
            "AttackMode", "Rest", "RestHero", "骑马", "下马", "禁止邀请上马", "PublicChat", "CryChat",
            "角色交易", "SendTopChatBoardMsg", "加入国家",
        };
    }

    /// <summary>测试隔离：按初值表复位全部命令（sCmd 与 nPermissionMin）。</summary>
    public static void ResetForTests()
    {
        for (int i = 0; i < g_GameCommand.All.Length; i++)
        {
            g_GameCommand.All[i].sCmd = g_GameCommand.DefaultCmds[i];
            g_GameCommand.All[i].nPermissionMin = 0;
        }
    }

    /// <summary>AddUserCommandToList 1:1：组标题 + 45 条命令（顺序/参数/说明原文一致）。</summary>
    public static List<GameCommandRow> BuildUserCommandList()
    {
        var rows = new List<GameCommandRow>();
        var idx = 0;
        void Group(string text) => rows.Add(new GameCommandRow { IsGroup = true, GroupText = text });
        void Cmd(TGameCmd cmd, string param, string desc)
        {
            idx++;
            rows.Add(new GameCommandRow { Index = idx, Cmd = cmd, Param = param, Desc = desc });
        }
        Group("聊天及信息");
        Cmd(g_GameCommand.PRVMSG, "人物名称", "禁止指定人物发的私聊信息");
        Cmd(g_GameCommand.ALLOWMSG, "", "禁止别人向自己发私聊信息");
        Cmd(g_GameCommand.LETSHOUT, "", "禁止接收组队聊天信息");
        Cmd(g_GameCommand.BANGUILDCHAT, "", "禁止接收行会聊天信息");
        Cmd(g_GameCommand.BANNATIONCHAT, "", "禁止接收国家聊天信息");
        Cmd(g_GameCommand.PUBLICCHAT, "", "禁止公聊");
        Cmd(g_GameCommand.CRYCHAT, "", "禁止喊话");
        Group("其他开关");
        Cmd(g_GameCommand.LETTRADE, "", "禁止交易物品");
        Cmd(g_GameCommand.LETCHALLENGE, "", "禁止挑战");
        Cmd(g_GameCommand.LETNATION, "", "允许加入国家");
        Group("行会相关");
        Cmd(g_GameCommand.LETGUILD, "", "允许加入行会");
        Cmd(g_GameCommand.AUTHALLY, "", "允许行会进入联盟");
        Cmd(g_GameCommand.AUTH, "", "开始进行行会联盟");
        Cmd(g_GameCommand.AUTHCANCEL, "", "取消行会联盟关系");
        Cmd(g_GameCommand.ENDGUILD, "", "退出当前所加入的行会");
        Group("传送相关");
        Cmd(g_GameCommand.ALLOWGROUPCALL, "", "允许记忆传送");
        Cmd(g_GameCommand.ALLOWMASTERRECALL, "", "允许师徒传送");
        Cmd(g_GameCommand.ALLOWDEARRCALL, "", "允许夫妻传送");
        Cmd(g_GameCommand.AllowGuildReCall, "", "允许行会传送");
        Cmd(g_GameCommand.USERMOVE, "X Y", "传送到某坐标(需佩戴传送戒指)");
        Cmd(g_GameCommand.DEARRECALL, "", "夫妻对方传送到身边");
        Cmd(g_GameCommand.MASTERECALL, "", "师父将徒弟召唤到身边");
        Cmd(g_GameCommand.GROUPRECALLL, "", "将组队人员传送到身边（需佩戴记忆全套装备）");
        Cmd(g_GameCommand.GUILDRECALLL, "", "将行会在线成员传送到身边（需要佩戴行会传送装备）");
        Group("查询服务");
        Cmd(g_GameCommand.Data, "", "查看当前服务器日期时间");
        Cmd(g_GameCommand.DEAR, "", "查询夫妻位置");
        Cmd(g_GameCommand.MASTER, "", "查询师徒位置");
        Cmd(g_GameCommand.SEARCHING, "人物名称", "探测人物所在位置（需要穿戴探测装备）");
        Group("人物操作");
        Cmd(g_GameCommand.UNLOCK, "", "开启登陆锁");
        Cmd(g_GameCommand.LOCKLOGON, "", "开启/关闭登陆锁");
        Cmd(g_GameCommand.SETPASSWORD, "", "设置仓库密码");
        Cmd(g_GameCommand.CHGPASSWORD, "", "修改仓库密码");
        Cmd(g_GameCommand.UNPASSWORD, "", "清除仓库密码（先开锁再清除密码）");
        Cmd(g_GameCommand.Lock, "", "将仓库锁上");
        Cmd(g_GameCommand.UNLOCKSTORAGE, "", "仓库解锁");
        Cmd(g_GameCommand.ATTACKMODE, "", "改变人物攻击模式");
        Cmd(g_GameCommand.RESTHERO, "", "改变英雄状态（跟随/休息/攻击)");
        Cmd(g_GameCommand.REST, "", "改变下属状态（休息/攻击)");
        Cmd(g_GameCommand.DISABLEHORSEINVITE, "", "禁止邀请上马");
        Cmd(g_GameCommand.TAKEONHORSE, "", "戴马牌后骑上马");
        Cmd(g_GameCommand.TAKEOFHORSE, "", "从马上下来");
        Cmd(g_GameCommand.OpenSellPlayer, "", "允许/禁止别人角色交易时向自己发出委托申请");
        Cmd(g_GameCommand.SendTopChatBoardMsg, "", "千里传音，传音筒命令");
        Cmd(g_GameCommand.MEMBERFUNCTION, "", "后台管理");
        Cmd(g_GameCommand.MEMBERFUNCTIONEX, "", "军团");
        return rows;
    }
}
