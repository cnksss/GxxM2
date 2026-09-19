using System;
using System.Collections.Generic;

namespace GXX.M2Server.Engine;

/// <summary>TScriptPlayer 状态扩展（批次I：NPC 命令所需的完整人物状态面）。</summary>
public partial class TScriptPlayer
{
    public string m_sDearName = "";          // 配偶
    public string m_sMasterName = "";        // 师傅
    public int m_btRenewLevel;               // 转生等级
    public int m_nMemberType;                // 会员类型
    public int m_nMemberLevel;               // 会员等级
    public int m_nGameDiamond;               // 金刚石
    public int m_nGameGird;                  // 灵符
    public int m_nGameGlory;                 // 荣耀
    public int m_nCreditPoint2;              // 声望（副本字段）
    public int m_nBonusPoint;                // 属性点
    public byte m_btAttackMode;              // 攻击模式 0..7
    public byte m_btHair;                    // 发型（CHANGEGENDER 变更性别时调整）
    public bool m_boOnHorse;                 // 骑马
    public bool m_boAdminMode;               // 管理员模式（隐身）
    public bool m_boSlaveRelax;              // 宝宝休息
    public uint m_dwOnlineLongMin;           // 在线分钟
    public string m_sKillMonName = "";       // 最近杀死怪名
    public string m_sHitMonName = "";        // 最近攻击怪名
    public string m_sIPaddr = "127.0.0.1";
    public string m_sAccount = "";
    public bool m_boIsNewHuman;              // 新人物
    public int m_nPayMent;                   // 计费模式
    public int m_nScriptGotoCount;           // 脚本跳转计数（防死循环）
    public string m_sRandomString = "";      // RANDOMNO 抽签结果

    /// <summary>全局变量（GLOBALVAR：对应 M2 全局 g_GVarList）。</summary>
    public static readonly Dictionary<string, int> GlobalVars = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>人物级字符串变量（HUMANVAR）。</summary>
    public readonly Dictionary<string, string> HumanStrVars = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>名单文件（Envir 目录名单：按文件名索引，CHECKNAMELISTPOSITION 按 0 基行号查询）。</summary>
    public static readonly Dictionary<string, List<string>> NameListFiles = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>名字列表（NameList：脚本文件名单，模拟为引擎集合）。</summary>
    public static readonly HashSet<string> EngineNameList = new(StringComparer.OrdinalIgnoreCase);
    public static readonly HashSet<string> EngineAccountList = new(StringComparer.OrdinalIgnoreCase);
    public static readonly HashSet<string> EngineIpList = new(StringComparer.OrdinalIgnoreCase);
    public static readonly HashSet<string> EngineGuildList = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>是否行会成员（由行会系统回填：GuildManager.MemberOfGuild）。</summary>
    public Func<string, bool>? IsMemberOfGuildFn;

    public bool HasGuild => IsMemberOfGuildFn != null && IsMemberOfGuildFn(m_sCharName);

    public bool IsGuildMasterFn() => false; // 由行会系统回填
}
