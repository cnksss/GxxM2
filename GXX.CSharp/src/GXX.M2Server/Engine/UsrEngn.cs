using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// UsrEngn.pas TUserEngine 核心转换：地图表/在线玩家/怪物管理 + 主循环。
/// </summary>
public class TUserEngine : IDisposable
{
    public readonly Dictionary<string, TEnvirnoment> MapList = new(StringComparer.OrdinalIgnoreCase);
    public readonly List<TPlayObject> PlayObjects = new();
    public readonly List<TMonster> Monsters = new();

    /// <summary>怪物定义（MonsterDB 文本行）。</summary>
    public class TMonDBRecord
    {
        public string sName = "";
        public ushort wRaceImg;
        public uint wHP;
        public byte wMP;
        public byte wAttack;
        public byte wDefense;
    }
    public readonly List<TMonDBRecord> MonDefList = new();

    private readonly object _lock = new();
    private uint _dwProcessMapTick;
    private uint _dwProcessMonTick;

    public int PlayObjectCount { get { lock (_lock) return PlayObjects.Count; } }
    public int MonsterCount { get { lock (_lock) return Monsters.Count; } }
    public int MapCount => MapList.Count;

    // ---- 批次J3：技能注册表 + 全服广播（UsrEngn.pas 1:1 子集） ----

    /// <summary>MagicList 等效：已加载技能定义（按 TMagicAttr 分册扫描）。</summary>
    public readonly List<(TMagicDef Magic, TMagicAttr Attr)> MagicDefs = new();

    /// <summary>m_HeroObjectList 等效（批次J4b：ViewOnlineHuman 英雄列表；THeroObject 为 TPlayObject 派生语义）。</summary>
    public readonly List<TPlayObject> HeroObjects = new();

    /// <summary>AddMagicDef：加载 Magic.DB 行（Delphi LoadMagicList 入册语义）。</summary>
    public void AddMagicDef(TMagicDef magic, TMagicAttr attr) => MagicDefs.Add((magic, attr));

    /// <summary>FindMagic(nMagIdx, MagicAttr)：按 ID + 属性扫描（UsrEngn.pas 8100 行 1:1 顺序）。</summary>
    public TMagicDef? FindMagic(int nMagIdx, TMagicAttr attr)
    {
        foreach (var (magic, a) in MagicDefs)
        {
            if (magic != null && magic.wMagicId == nMagIdx && a == attr)
                return magic;
        }
        return null;
    }

    /// <summary>SendBroadCastMsgExt(sMsg, MsgType)：全服广播（UsrEngn.pas 9491 行 1:1，
    /// 跳过 ghost/death/offline/dummy；MsgFrom 缺省入口）。</summary>
    public void SendBroadCastMsgExt(string sMsg, TMsgType msgType)
    {
        lock (_lock)
        {
            foreach (var playObject in PlayObjects)
            {
                if (playObject == null) continue;
                if (!playObject.m_boGhost && !playObject.m_boDeath && !playObject.m_boOffLine && !playObject.m_boDummyObject)
                    playObject.SysMsg(sMsg, TMsgColor.c_Red, msgType);
            }
        }
    }


    /// <summary>LoadMapList：从 MapInfo.txt / 直接加载地图目录的 .map 文件。</summary>
    public bool LoadMaps(string mapDir)
    {
        if (!Directory.Exists(mapDir)) return false;
        foreach (string file in Directory.GetFiles(mapDir, "*.map"))
        {
            string name = Path.GetFileNameWithoutExtension(file);
            var env = new TEnvirnoment { sMapName = name };
            if (env.LoadMapData(file))
            {
                MapList[name] = env;
            }
        }
        return MapList.Count > 0;
    }

    public TEnvirnoment? FindMap(string name)
        => MapList.TryGetValue(name, out var env) ? env : null;

    /// <summary>AddPlayer：玩家上线（对应 UsrEngn.AddPlayObject）。</summary>
    public bool AddPlayObject(TPlayObject player, string mapName, int x, int y)
    {
        var env = FindMap(mapName);
        if (env == null) return false;
        player.m_PEnvir = env;
        player.m_sMapName = mapName;
        player.m_nCurrX = x;
        player.m_nCurrY = y;
        if (!env.CanAddToMapPosition(x, y))
        {
            // 找最近可站格
            bool placed = false;
            for (int r = 1; r < 10 && !placed; r++)
            {
                for (int dx = -r; dx <= r && !placed; dx++)
                    for (int dy = -r; dy <= r && !placed; dy++)
                        if (env.CanWalk(x + dx, y + dy))
                        {
                            player.m_nCurrX = x + dx;
                            player.m_nCurrY = y + dy;
                            placed = true;
                        }
            }
        }
        env.AddToMap(player.m_nCurrX, player.m_nCurrY, player);
        player.m_boAddToMaped = true;
        player.m_boReadyRun = true;
        lock (_lock) PlayObjects.Add(player);
        return true;
    }

    public void RemovePlayObject(TPlayObject player)
    {
        player.m_PEnvir?.DeleteFromMap(player.m_nCurrX, player.m_nCurrY, player);
        lock (_lock) PlayObjects.Remove(player);
    }

    public TMonster? SpawnMonster(string mapName, int x, int y, string monName)
    {
        var env = FindMap(mapName);
        if (env == null || !env.CanWalk(x, y)) return null;
        var mon = new TMonster
        {
            m_sCharName = monName,
            m_sMapName = mapName,
            m_PEnvir = env,
            m_nCurrX = x,
            m_nCurrY = y
        };
        env.AddToMap(x, y, mon);
        lock (_lock) Monsters.Add(mon);
        return mon;
    }

    /// <summary>主循环（对应 UserEngine.Process / Run）。</summary>
    public void Process()
    {
        uint now = DelphiRTL.GetTickCount();

        TPlayObject[] players;
        lock (_lock) players = PlayObjects.ToArray();
        foreach (var p in players)
            p.Run();

        if (now > _dwProcessMonTick)
        {
            _dwProcessMonTick = now + 60; // 怪物 AI 节拍
            TMonster[] mons;
            lock (_lock) mons = Monsters.ToArray();
            foreach (var m in mons)
                m.Run();
        }
    }

    public void Dispose()
    {
        MapList.Clear();
        lock (_lock)
        {
            PlayObjects.Clear();
            Monsters.Clear();
        }
    }
}
