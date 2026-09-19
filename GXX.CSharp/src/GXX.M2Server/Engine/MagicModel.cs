using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>TMagicDef：技能定义（Magic.DB 行，对应 pTMagicDef 所需子集）。</summary>
public class TMagicDef
{
    public ushort wMagicId;
    public string sMagicName = "";
    public byte btEffectType;
    public byte btEffect;
    public ushort wSpell;          // MP 消耗
    public ushort wPower;
    public ushort wMaxPower;
    public ushort wDefPower;
    public ushort wDefMaxPower;
    public byte btTrainLv;
    public int TrainMax;
    public uint dwDelay;
    public ushort wDefSpell;
}

/// <summary>TUserMagic：人物已学魔法（pTUserMagic 对应）。</summary>
public class TUserMagicRef
{
    public TMagicDef? MagicInfo;
    public ushort wMagIdx;
    public byte btLevel;
    public byte btNewLevel;    // 九重强化
    public byte btKey;
    public int CurTrain;
}

/// <summary>引擎内功强化生物基座：为魔法层补齐 Delphi TBaseObject 的关键字段与方法。</summary>
public class TSpellCaster : TCreature
{
    public TSpellCaster() { m_btRaceServer = Grobal2Const.RC_PLAYOBJECT; }
}

public class TSpellTarget : TCreature
{
    public TSpellTarget() { m_btRaceServer = Grobal2Const.RC_MONSTER; }
}

public partial class TCreature
{
    private static readonly Random Rnd = new();

    public byte m_btRaceServer;      // RC_*（服务端种族值）
    public int m_nLuck;
    public int m_nAntiMagic;         // 魔法躲避 0..10
    public byte m_btLifeAttrib;      // LA_UNDEAD=1 不死系
    public bool m_boStickMode;       // 无法被推/移动
    public uint[] m_SkillUseTick = new uint[512];
    public readonly List<TCreature> m_VisibleActors = new();

    /// <summary>延时消息（对应 SendDelayMsg 队列；RM_MAGHEALING/RM_DELAYMAGIC 在 ProcessDelayedMessages 中结算）。</summary>
    public struct TDelayedMsg
    {
        public TCreature? Sender;
        public ushort Ident;
        public int P1;
        public int P2;
        public int P3;
        public int Tag;
        public string Body;
    }

    private readonly List<TDelayedMsg> _delayedMsgs = new();

    public int DelayedMsgCount
    {
        get { lock (_delayedMsgs) return _delayedMsgs.Count; }
    }

    public void AddVisibleActor(TCreature c)
    {
        if (!ReferenceEquals(c, this) && !m_VisibleActors.Contains(c))
            m_VisibleActors.Add(c);
    }

    public void SendDelayMsg(TCreature? sender, ushort ident, int p1, int p2, int p3, int tag, string body, int delayMs)
    {
        lock (_delayedMsgs)
        {
            _delayedMsgs.Add(new TDelayedMsg
            {
                Sender = sender,
                Ident = ident,
                P1 = p1,
                P2 = p2,
                P3 = p3,
                Tag = tag,
                Body = body ?? ""
            });
        }
    }

    /// <summary>结算延时消息：RM_MAGHEALING 回血；RM_DELAYMAGIC 扣血（对应 ObjBase Operate 中的处理）。</summary>
    public void ProcessDelayedMessages()
    {
        List<TDelayedMsg> batch;
        lock (_delayedMsgs)
        {
            if (_delayedMsgs.Count == 0) return;
            batch = new List<TDelayedMsg>(_delayedMsgs);
            _delayedMsgs.Clear();
        }
        foreach (var m in batch)
        {
            switch (m.Ident)
            {
                case Grobal2Const.RM_MAGHEALING:
                {
                    uint heal = (uint)Math.Max(0, m.P1);
                    m_wAbil.HP = Math.Min(m_wAbil.MaxHP, m_wAbil.HP + heal);
                    break;
                }
                case Grobal2Const.RM_DELAYMAGIC:
                case Grobal2Const.RM_MAGSTRUCK:
                    StruckDamage(m.P1);
                    break;
            }
        }
    }

    /// <summary>GetAttackPower：基础 + Random(range+1)（幸运上限语义由威力函数承担）。</summary>
    public int GetAttackPower(int nBasePower, int nRange)
    {
        if (nRange <= 0) return nBasePower;
        return nBasePower + Rnd.Next(nRange + 1);
    }

    /// <summary>GetMagicCD：技能附加 CD（默认 0，DB 配置可扩展）。</summary>
    public uint GetMagicCD(ushort magicId) => 0;

    /// <summary>是否合法攻击目标（简化阵营判定：敌对阵营 + 存活）。</summary>
    public bool IsProperTarget(TCreature? target)
    {
        if (target == null || ReferenceEquals(target, this)) return false;
        if (target.m_boDeath || target.m_boGhost) return false;
        bool selfMon = m_btRaceServer >= Grobal2Const.RC_ANIMAL;
        bool tgtMon = target.m_btRaceServer >= Grobal2Const.RC_ANIMAL;
        if (selfMon != tgtMon) return true;               // 异阵营
        if (selfMon && m_btRaceServer != target.m_btRaceServer) return true; // 怪间互攻（不同种族）
        return false;
    }

    /// <summary>是否合法友方目标。</summary>
    public bool IsProperFriend(TCreature? target)
    {
        if (target == null) return false;
        if (target.m_boDeath) return false;
        bool selfMon = m_btRaceServer >= Grobal2Const.RC_ANIMAL;
        bool tgtMon = target.m_btRaceServer >= Grobal2Const.RC_ANIMAL;
        return selfMon == tgtMon;
    }

    /// <summary>CharPushed：沿方向推动 push 格（对应 ObjBase.CharPushed）。</summary>
    public void CharPushed(byte dir, int push)
    {
        if (m_PEnvir == null || m_boStickMode) return;
        for (int i = 0; i < push; i++)
        {
            int nx = m_nCurrX + TMonster.DirDeltaX(dir);
            int ny = m_nCurrY + TMonster.DirDeltaY(dir);
            if (!m_PEnvir.CanWalk(nx, ny)) break;
            m_PEnvir.DeleteFromMap(m_nCurrX, m_nCurrY, this);
            m_nCurrX = nx;
            m_nCurrY = ny;
            m_PEnvir.AddToMap(nx, ny, this);
        }
    }

    /// <summary>GetNextDirection：由坐标差计算 8 方向（Grobal2 DR_ 布局）。</summary>
    public static byte GetNextDirection(int sx, int sy, int tx, int ty)
    {
        int dx = tx - sx, dy = ty - sy;
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

    /// <summary>GetNextPosition：从 (x,y) 沿 dir 走 n 步的目标坐标。</summary>
    public static bool GetNextPosition(int x, int y, byte dir, int n, out int nx, out int ny)
    {
        nx = x + TMonster.DirDeltaX(dir) * n;
        ny = y + TMonster.DirDeltaY(dir) * n;
        return true;
    }

    /// <summary>MagCanHitTarget：视线判定（距离 + 直线可走）。</summary>
    public bool MagCanHitTarget(int sx, int sy, TCreature? target)
    {
        if (target == null || m_PEnvir == null) return false;
        int dx = target.m_nCurrX - sx, dy = target.m_nCurrY - sy;
        if (Math.Abs(dx) > 24 || Math.Abs(dy) > 24) return false;
        // 步进视线检查（忽略端点）
        int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
        if (steps == 0) return true;
        double stepX = (double)dx / steps, stepY = (double)dy / steps;
        double cx = sx, cy = sy;
        for (int i = 1; i < steps; i++)
        {
            cx += stepX;
            cy += stepY;
            if (!m_PEnvir.CanWalk((int)Math.Round(cx), (int)Math.Round(cy))) return false;
        }
        return true;
    }

    /// <summary>MagPassThroughMagic：贯穿线伤害（地狱火/疾光电影），返回命中数。</summary>
    public int MagPassThroughMagic(int startX, int startY, int endX, int endY, byte dir, int power, ushort magicId, bool boFast)
    {
        if (m_PEnvir == null) return 0;
        int hit = 0;
        int x = startX, y = startY;
        while (true)
        {
            if (m_PEnvir.CanWalk(x, y))
            {
                foreach (var obj in m_PEnvir.GetObjects(x, y))
                {
                    if (obj is TCreature t && IsProperTarget(t))
                    {
                        t.StruckDamage(power);
                        hit++;
                    }
                }
            }
            if (x == endX && y == endY) break;
            x += TMonster.DirDeltaX(dir);
            y += TMonster.DirDeltaY(dir);
            if (x < 0 || x >= m_PEnvir.nWidth || y < 0 || y >= m_PEnvir.nHeight) break;
        }
        return hit;
    }
}
