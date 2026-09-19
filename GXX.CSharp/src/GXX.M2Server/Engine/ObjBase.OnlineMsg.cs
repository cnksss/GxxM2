namespace GXX.M2Server.Engine;

/// <summary>
/// TCreature 批次J3/J4b 扩展：离线/假人标志 + SysMsg + 性别/职业/主从（ViewOnlineHuman/HumanInfo 窗体族）。
/// SysMsg 当前落到进程内消息列表（客户端管道批次接入后改为按 SendMsg 发送 SM_SYSMESSAGE）。
/// </summary>
public abstract partial class TCreature
{
    /// <summary>Delphi m_boOffLine：离线挂机。</summary>
    public bool m_boOffLine;

    /// <summary>Delphi m_boDummyObject：假人对象。</summary>
    public bool m_boDummyObject;

    /// <summary>Delphi m_btGender（0=男 1=女）。</summary>
    public byte m_btGender;

    /// <summary>Delphi m_boGamePet：宠物对象（RecalcLevelAbilitys 直通跳过）。</summary>
    public bool m_boGamePet;

    /// <summary>进程内系统消息观察列表（SysMsg 落点）。</summary>
    public readonly List<string> SysMsgs = new();

    /// <summary>Delphi TCreature.SysMsg 等效入口（颜色/类型按原签名保留）。</summary>
    public void SysMsg(string sMsg, TMsgColor color, TMsgType msgType)
    {
        SysMsgs.Add(sMsg);
    }
}

/// <summary>TPlayObject 批次J4b/J6 扩展：ViewOnlineHuman/HumanInfo 展示与操作字段子集。</summary>
public partial class TPlayObject
{
    public string m_sIPLocal = "";
    public int m_btPermission;
    public int m_nGameGold;
    public int m_nGamePoint;
    public int m_nPayMentPoint;
    public int m_nGameDiamond;
    public int m_nGameGird;
    public string m_sAutoSendMsg = "";

    // ---- 批次J6：HumanInfo 编辑字段 ----
    public uint m_nGold;              // 金币
    public int m_nPKPOINT;            // PK 值
    public int m_nGameGlory;          // 荣誉值
    public int m_nBonusPoint;         // 已分配属性点
    public bool m_boGameMaster;       // GM
    public bool m_boObServer;         // 观察者
    public bool m_boSuperman;         // 无敌
    public bool m_boPlayOffLine;      // 挂机下线标记
    public bool m_boEmergencyClose;   // 紧急关闭（踢下线）

    /// <summary>Delphi MakeGhost：标记释放（Ghost 对象由引擎统一清理）。</summary>
    public void MakeGhost()
    {
        m_boGhost = true;
        m_dwGhostTick = GXX.Core.Rtl.DelphiRTL.GetTickCount();
    }
}
