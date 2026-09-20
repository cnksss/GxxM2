using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// ObjBase.pas TCreature 1:1 核心转换（角色/怪物/NPC 基类）。
/// 保留原关键字段命名（m_nCurrX/m_btDirection/m_wAbil 等）与消息驱动架构。
/// </summary>
public abstract partial class TCreature
{
    // ---- 标识 ----
    public long m_nRecogId;                 // 唯一标识（原 TObject 指针 → 64位）
    public int m_nCurrX;
    public int m_nCurrY;
    public byte m_btDirection;
    public byte m_btRace;                   // RC_PLAYOBJECT / RC_MONSTER ...
    public byte m_btRaceImg;
    public string m_sCharName = "";
    public string m_sMapName = "";
    public TEnvirnoment? m_PEnvir;

    // ---- 状态 ----
    public bool m_boGhost;                  // 已释放
    public bool m_boDeath;
    public bool m_boVisible = true;
    public bool m_boAddToMaped;
    public uint m_dwGhostTick;

    // ---- 属性（TAbility）----
    public TAbility m_wAbil;

    // ---- 消息队列（TProcessMessage → SendMsg/Operate）----
    private readonly Queue<TProcessMessageRef> m_MsgList = new();

    public static long NextRecogId = 1000;

    protected TCreature()
    {
        m_nRecogId = System.Threading.Interlocked.Increment(ref NextRecogId);
    }

    // ---- 消息机制 ----

    public void SendMsg(ushort wIdent, long wParam, long nParam1, long nParam2, long nParam3, string sMsg)
    {
        lock (m_MsgList)
        {
            if (m_MsgList.Count > 1000) return; // 队列上限（对应原 SendMessage 队列溢出丢弃）
            m_MsgList.Enqueue(new TProcessMessageRef
            {
                wIdent = wIdent,
                wParam = wParam,
                nParam1 = nParam1,
                nParam2 = nParam2,
                nParam3 = nParam3,
                sMsg = sMsg,
                dwTimeTick = DelphiRTL.GetTickCount(),
                BaseObject = 0
            });
        }
    }

    /// <summary>处理全部积压消息（对应 Operate(LPDWDelayTime)）。</summary>
    public virtual void Operate()
    {
        while (true)
        {
            TProcessMessageRef? msg;
            lock (m_MsgList)
            {
                if (m_MsgList.Count == 0) return;
                msg = m_MsgList.Dequeue();
            }
            if (msg != null)
                Operate(msg);
        }
    }

    protected virtual void Operate(TProcessMessageRef msg)
    {
        switch (msg.wIdent)
        {
            case Grobal2Const.RM_WALK:
                WalkTo((byte)msg.wParam);
                break;
            case Grobal2Const.RM_TURN:
                m_btDirection = (byte)msg.wParam;
                break;
            case Grobal2Const.RM_STRUCK:
                StruckDamage((int)msg.nParam1);
                break;
        }
    }

    // ---- 移动 ----

    /// <summary>WalkTo：按方向走一格（对应 WalkTo(btDir)）。</summary>
    public virtual bool WalkTo(byte btDir)
    {
        if (m_PEnvir == null || m_boDeath || m_boGhost) return false;
        int newX = m_nCurrX + DirToX(btDir);
        int newY = m_nCurrY + DirToY(btDir);
        if (!m_PEnvir.CanWalk(newX, newY)) return false;
        if (!m_PEnvir.DoorOpened(newX, newY)) return false;

        m_PEnvir.DeleteFromMap(m_nCurrX, m_nCurrY, this);
        m_nCurrX = newX;
        m_nCurrY = newY;
        m_btDirection = btDir;
        m_PEnvir.AddToMap(m_nCurrX, m_nCurrY, this);
        return true;
    }

    public static int DirToX(byte dir) => DirDeltaX(dir);
    public static int DirToY(byte dir) => DirDeltaY(dir);

    /// <summary>DR_UP..DR_UPLEFT 的 X 增量（Grobal2 方向布局）。</summary>
    public static int DirDeltaX(byte dir) => s_DirX[Math.Min(dir, (byte)7)];
    /// <summary>DR_UP..DR_UPLEFT 的 Y 增量。</summary>
    public static int DirDeltaY(byte dir) => s_DirY[Math.Min(dir, (byte)7)];

    private static readonly int[] s_DirX = { 0, 1, 1, 1, 0, -1, -1, -1 };
    private static readonly int[] s_DirY = { -1, -1, 0, 1, 1, 1, 0, -1 };

    // ---- 战斗 ----

    public virtual void StruckDamage(int damage)
    {
        int hp = (int)Math.Max(0, (long)m_wAbil.HP - damage);
        m_wAbil.HP = (uint)hp;
        if (hp <= 0)
        {
            m_boDeath = true;
            Die();
        }
    }

    protected virtual void Die()
    {
        m_boDeath = true;
    }

    /// <summary>运行一次（对应 Run()：心跳/再生/超时清理）。</summary>
    public virtual void Run()
    {
        Operate();
    }
}

/// <summary>ObjPlayer.pas TPlayObject 核心（在线玩家会话）。</summary>
public partial class TPlayObject : TCreature
{
    public string m_sUserID = "";            // 账号
    public string m_sIPaddr = "";
    public int m_nSocket;                    // 网关套接字 ID
    public int m_nGSocketIdx;                // 网关索引
    public uint m_dwLogonTick;
    public bool m_boReadyRun;
    public long m_nSessionId;

    // 物品/魔法容器（对应 THumanUseItems/m_ItemList）
    // m_ItemList 原为 List<TUserItem> 且**全仓零调用方**；车道 p6-m2-playersurface 指出：
    // 它的物品容器用 List<TUserItemView>（与既有 m_UseItems 元素类型一致），两者元素类型不同
    // 因而**无法用 override 桥接**。此处按该建议改为 TUserItemView，使
    // `BagItems => m_ItemList` 成为可能（否则 BagItems 只能另持一份后备字段，形成双容器）。
    public List<TUserItemView> m_ItemList = new();
    public List<THumMagic> m_MagicList = new();

    public TPlayObject()
    {
        m_btRace = Grobal2Const.RC_PLAYOBJECT;
        // 原文 m_TVal/m_ZVal/m_sString 的元素默认值是 ShortString 的 ''（空串），
        // 而托管侧数组元素默认是 null。车道 p6-m2-playersurface 无法自行修正
        // （本文件已有无参构造，它再声明一个会 CS0111），故由集成方在此调用其提供的迁移函数。
        PlayerSurfaceVarDefaults.MigrateStringVarDefaults(this);
    }

    public override void Run()
    {
        base.Run();
        // 心跳：超时踢线由 UserEngine 统一处理
    }
}

/// <summary>ObjMon.pas TAnimal/TCreature 怪物核心。</summary>
public class TMonster : TCreature
{
    public uint m_dwWalkTick;
    public uint m_dwAttackTick;
    public int m_nViewRange = 8;
    public TCreature? m_Target;

    public TMonster()
    {
        m_btRace = Grobal2Const.RC_MONSTER;
    }

    public override void Run()
    {
        base.Run();
        if (m_boDeath || m_boGhost) return;
        uint now = DelphiRTL.GetTickCount();
        // AI：视野内追踪目标（对应 TAnimal.Run 的搜索/追击简化主循环）
        if (m_Target != null && !m_Target.m_boDeath && m_PEnvir != null)
        {
            if (now > m_dwWalkTick)
            {
                m_dwWalkTick = now + 800;
                int dx = Math.Sign(m_Target.m_nCurrX - m_nCurrX);
                int dy = Math.Sign(m_Target.m_nCurrY - m_nCurrY);
                byte dir = DirFromDelta(dx, dy);
                WalkTo(dir);
            }
        }
    }

    public static byte DirFromDelta(int dx, int dy)
    {
        if (dx == 0 && dy < 0) return Grobal2Const.DR_UP;
        if (dx > 0 && dy < 0) return Grobal2Const.DR_UPRIGHT;
        if (dx > 0 && dy == 0) return Grobal2Const.DR_RIGHT;
        if (dx > 0 && dy > 0) return Grobal2Const.DR_DOWNRIGHT;
        if (dx == 0 && dy > 0) return Grobal2Const.DR_DOWN;
        if (dx < 0 && dy > 0) return Grobal2Const.DR_DOWNLEFT;
        if (dx < 0 && dy == 0) return Grobal2Const.DR_LEFT;
        if (dx < 0 && dy < 0) return Grobal2Const.DR_UPLEFT;
        return Grobal2Const.DR_UP;
    }
}
