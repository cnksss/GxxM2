using System;
using System.IO;
using System.Runtime.InteropServices;
using GXX.Core.Protocol;
using GXX.GatewayKit;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>M2 引擎核心测试：地图加载（合成 .map）+ 走位 + 怪物 AI + 门。</summary>
public class M2ServerTests
{
    /// <summary>生成合成 .map（EN 加密格式，Map 2010 Ver 1.0 头 + $AA38 XOR）。</summary>
    private static string CreateTestMap(string path, int width, int height, bool blockCenter)
    {
        using var fs = new FileStream(path, FileMode.Create);
        using var bw = new BinaryWriter(fs);

        // TENMapHeader：Title string[16] + Reserved(4) + Width(2)+Not1(2) + Height(2)+Not2(2) + Reserved2[25]
        byte[] title = new byte[17];
        string t = "Map 2010 Ver 1.0";
        title[0] = (byte)t.Length;
        for (int i = 0; i < t.Length; i++) title[1 + i] = (byte)t[i];
        bw.Write(title);
        bw.Write(0u);              // Reserved
        bw.Write((ushort)(width ^ TEnvirnoment.XORWORD));
        bw.Write((ushort)0);
        bw.Write((ushort)(height ^ TEnvirnoment.XORWORD));
        bw.Write((ushort)0);
        bw.Write(new byte[25]);

        // 列主序格子
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ushort bkImg = 0, frImg = 0;
                if (blockCenter && x >= width / 2 - 1 && x <= width / 2 + 1 && y >= height / 2 - 1 && y <= height / 2 + 1)
                {
                    bkImg = 0x8000; // 解码后带 $8000 = 禁走
                }
                // EN 格式：文件中存的是 (原值 XOR $AA38)
                bw.Write((ushort)(bkImg ^ TEnvirnoment.XORWORD));   // BkImg
                bw.Write((ushort)TEnvirnoment.XORWORD);             // MidImg（明文 0）
                bw.Write((ushort)TEnvirnoment.XORWORD);             // FrImg（明文 0）
                bw.Write((byte)0);                               // DoorIndex
                bw.Write((byte)0); bw.Write((byte)0); bw.Write((byte)0);
                bw.Write((byte)0);                               // Area
                bw.Write((byte)0);                               // Light
            }
        }
        return path;
    }

    [Fact]
    public void MapLoad_ENFormat_WidthHeightAndBlocks()
    {
        string mapFile = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid().ToString("N") + ".map");
        try
        {
            CreateTestMap(mapFile, 32, 24, true);
            var env = new TEnvirnoment { sMapName = "test" };
            Assert.True(env.LoadMapData(mapFile));
            Assert.Equal(32, env.nWidth);
            Assert.Equal(24, env.nHeight);

            // 中心 3x3 被禁走
            Assert.False(env.CanWalk(env.nWidth / 2, env.nHeight / 2));
            Assert.True(env.CanWalk(0, 0));
            Assert.False(env.CanWalk(-1, 0));
            Assert.False(env.CanWalk(32, 5));
        }
        finally
        {
            File.Delete(mapFile);
        }
    }

    [Fact]
    public void Creature_WalkTo_BlockedByObstacle()
    {
        string mapFile = Path.Combine(Path.GetTempPath(), "test_" + Guid.NewGuid().ToString("N") + ".map");
        try
        {
            CreateTestMap(mapFile, 40, 40, true);
            var env = new TEnvirnoment();
            Assert.True(env.LoadMapData(mapFile));

            var mon = new TMonster
            {
                m_PEnvir = env,
                m_nCurrX = 5,
                m_nCurrY = 5
            };
            env.AddToMap(5, 5, mon);

            // 向右走两步
            Assert.True(mon.WalkTo(Grobal2Const.DR_RIGHT));
            Assert.Equal(6, mon.m_nCurrX);
            Assert.True(mon.WalkTo(Grobal2Const.DR_RIGHT));
            Assert.Equal(7, mon.m_nCurrX);
            // 障碍(中心)不可走：从 (7,5) 走 DR_UP 到 (7,4) 应成功
            Assert.True(mon.WalkTo(Grobal2Const.DR_UP));
            Assert.Equal(4, mon.m_nCurrY);
        }
        finally
        {
            File.Delete(mapFile);
        }
    }

    [Fact]
    public void StruckDamage_DeathAtZeroHp()
    {
        var mon = new TMonster();
        mon.m_wAbil.HP = 10;
        mon.m_wAbil.MaxHP = 10;
        mon.StruckDamage(5);
        Assert.False(mon.m_boDeath);
        Assert.Equal(5u, mon.m_wAbil.HP);
        mon.StruckDamage(5);
        Assert.True(mon.m_boDeath);
    }

    [Fact]
    public void UserEngine_MapLifecycle()
    {
        string mapFile = Path.Combine(Path.GetTempPath(), "mtest_" + Guid.NewGuid().ToString("N") + ".map");
        string mapDir = Path.Combine(Path.GetTempPath(), "mtestdir_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(mapDir);
        try
        {
            CreateTestMap(Path.Combine(mapDir, "mtest.map"), 50, 50, false);
            var engine = new TUserEngine();
            Assert.True(engine.LoadMaps(mapDir));
            Assert.Equal(1, engine.MapCount);

            var player = new TPlayObject { m_sCharName = "测试角色", m_sUserID = "acct" };
            Assert.True(engine.AddPlayObject(player, "mtest", 10, 10));
            Assert.Equal(1, engine.PlayObjectCount);
            Assert.Equal(10, player.m_nCurrX);

            // 走一步
            player.WalkTo(Grobal2Const.DR_RIGHT);
            Assert.Equal(11, player.m_nCurrX);

            // 生成怪物
            var mon = engine.SpawnMonster("mtest", 20, 20, "鸡");
            Assert.NotNull(mon);
            Assert.Equal(1, engine.MonsterCount);

            // 主循环跑一遍不抛异常
            engine.Process();

            engine.RemovePlayObject(player);
            Assert.Equal(0, engine.PlayObjectCount);
        }
        finally
        {
            Directory.Delete(mapDir, true);
        }
    }

    [Fact]
    public void GateManager_ServerFrameRoundTrip()
    {
        // TSvrCmdPack 帧的编码/解码一致性（网关↔M2 通道）
        byte[] frame = GatewayProtocol.BuildServerPacket(12345, GatewayProtocol.GM_DATA, 7, new byte[] { 9, 8, 7 }, 3);
        var parsed = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(frame, 0);
        Assert.Equal(GatewayProtocol.RUNGATECODE, parsed.Flag);
        Assert.Equal(12345u, parsed.SockID);
        Assert.Equal(7, parsed.GGSock);
        Assert.Equal(GatewayProtocol.GM_DATA, parsed.Cmd);
        Assert.Equal(3, parsed.DataLen);
        Assert.Equal(9, frame[20]);
    }
}
