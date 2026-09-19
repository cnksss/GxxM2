
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J65：uCustomNpcUtils.pas 1:1（345 行）测试。
/// 名称表、TCustomNpcConfig ctor 全量默认、INI 往返与绘制模式范围校验、
/// SaveCustomNpcClientConfigs 的 .dat 打包（GUID 头/条数/记录尺寸/CRC/启用优先重排）。
/// </summary>
public sealed class CustomNpcUtilsTests : IDisposable
{
    private readonly string _dir;

    public CustomNpcUtilsTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "j65_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.sEnvirDir = _dir + Path.DirectorySeparatorChar;
        M2Config.sSmartNpcDir = _dir + Path.DirectorySeparatorChar + "CustomNPC" + Path.DirectorySeparatorChar;
        CustomNpcUtils.ResetForTests();
    }

    public void Dispose()
    {
        CustomNpcUtils.ResetForTests();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { }
    }

    // ================= 名称表 =================

    [Fact]
    public void NameTables_MatchDelphi()
    {
        Assert.Equal(new[] { "站立", "动作" }, CustomNpcUtils.NpcActionNames);
        Assert.Equal(new[] { "方向1", "方向2", "方向3", "方向4", "方向5", "方向6", "方向7", "方向8" },
            CustomNpcUtils.NpcDirNames);
        Assert.Equal(new[] { "Dir1", "Dir2", "Dir3", "Dir4", "Dir5", "Dir6", "Dir7", "Dir8" },
            CustomNpcUtils.NpcDirSections);
        Assert.Equal(8, CustomNpcUtils.NpcDirNames.Length);
        Assert.Equal(8, CustomNpcUtils.NpcDirSections.Length);
        Assert.Equal(0, (int)TNpcActionType.atStand);
        Assert.Equal(1, (int)TNpcActionType.atAction);

        Assert.Equal(new Guid("3630817C-5AFB-4A06-873A-C7A2F8AF1F6C"), CustomNpcUtils.ClientCustomNpcConfigFlag);
    }

    // ================= ctor 默认值 =================

    [Fact]
    public void Ctor_BaseConfigDefaults()
    {
        var cfg = new TCustomNpcConfig(1234);

        Assert.Equal(1234, cfg.NpcAppr);
        Assert.False(cfg.IsChanged);

        var b = cfg.ClientBaseConfig;
        Assert.Equal(0, b.HPBgOffsetX);
        Assert.Equal(0, b.HPBgOffsetY);
        Assert.Equal(0, b.HPOffsetX);
        Assert.Equal(0, b.HPOffsetY);
        Assert.Equal(-1, b.HPFile);
        Assert.Equal(-1, b.HPStartIndex);
        Assert.Equal(0, b.HPTextOffsetX);
        Assert.Equal(0, b.HPTextOffsetY);

        Assert.Equal(TCustomDrawMode.mdmNormal, b.StandDrawMode);
        Assert.Equal(TCustomDrawMode.mdmNormal, b.ActionDrawMode);
        Assert.Equal(TCustomDrawMode.mdmBlend, b.StandEffectDrawMode);
        Assert.Equal(TCustomDrawMode.mdmBlend, b.ActionEffectDrawMode);

        Assert.Equal(0, b.KeepPlayFile);
        Assert.Equal(-1, b.KeepPlayIndex);
        Assert.Equal(0, b.KeepPlayCount);
        Assert.Equal(100, b.KeepPlayTime);
        Assert.Equal(1, b.KeepPlayBlendDraw);     // LongBool True
        Assert.Equal(0, b.KeepPlayOffsetX);
        Assert.Equal(0, b.KeepPlayOffsetY);

        Assert.Equal(TCustomNpcDrawOrder.ndoKeep_Chr_Eff, b.DrawOrder);
    }

    [Fact]
    public void Ctor_AllEightDirectionsDefaults()
    {
        var cfg = new TCustomNpcConfig(7);
        for (int i = 0; i < TNpcDirActionList.Count; i++)
        {
            var d = cfg.DirActions[i];
            Assert.Equal(1, d.Enabled);
            Assert.Equal(0, d.Std_File);
            Assert.Equal(-1, d.Std_Index);
            Assert.Equal(0, d.Std_Count);
            Assert.Equal(200, d.Std_Time);
            Assert.Equal(0, d.Std_EffFile);
            Assert.Equal(-1, d.Std_EffIndex);

            Assert.Equal(0, d.Act_File);
            Assert.Equal(-1, d.Act_Index);
            Assert.Equal(0, d.Act_Count);
            Assert.Equal(200, d.Act_Time);
            Assert.Equal(0, d.Act_EffFile);
            Assert.Equal(-1, d.Act_EffIndex);
        }
    }

    [Fact]
    public void IniPath_UsesSmartNpcDirAndAppr()
    {
        var cfg = new TCustomNpcConfig(88);
        Assert.Equal(M2Config.sSmartNpcDir + "88.ini", cfg.IniPath);
    }

    // ================= 落盘 / 回读 =================

    [Fact]
    public void SaveToIniFile_WritesAllKeysAndCreatesDir()
    {
        Assert.False(Directory.Exists(M2Config.sSmartNpcDir));
        var cfg = new TCustomNpcConfig(5);
        cfg.SetChanged();
        Assert.True(cfg.IsChanged);

        cfg.SaveToIniFile();
        Assert.False(cfg.IsChanged);                       // 先复位
        Assert.True(Directory.Exists(M2Config.sSmartNpcDir));

        string text = File.ReadAllText(cfg.IniPath, GXX.Core.EncodingInit.GBK);
        Assert.StartsWith("[BaseConfig]", text);
        Assert.Contains("HPBgOffsetX=0", text);
        Assert.Contains("HPFile=-1", text);
        Assert.Contains("HPStartIndex=-1", text);
        Assert.Contains("StandDrawMode=1", text);          // mdmNormal
        Assert.Contains("StandEffectDrawMode=0", text);    // mdmBlend
        Assert.Contains("DrawOrder=0", text);
        Assert.Contains("KeepPlayTime=100", text);
        Assert.Contains("KeepPlayBlendDraw=1", text);
        Assert.Contains("[Dir1]", text);
        Assert.Contains("[Dir8]", text);
        Assert.Contains("StdIndex=-1", text);
        Assert.Contains("ActEffIndex=-1", text);

        // BaseConfig 出现在 Dir1 之前
        Assert.True(text.IndexOf("[BaseConfig]", StringComparison.Ordinal)
                    < text.IndexOf("[Dir1]", StringComparison.Ordinal));
        Assert.True(text.IndexOf("[Dir1]", StringComparison.Ordinal)
                    < text.IndexOf("[Dir2]", StringComparison.Ordinal));
    }

    [Fact]
    public void SaveLoad_RoundTripAllFields()
    {
        var cfg = new TCustomNpcConfig(11);
        var b = cfg.ClientBaseConfig;
        b.HPBgOffsetX = 1;
        b.HPBgOffsetY = 2;
        b.HPOffsetX = 3;
        b.HPOffsetY = 4;
        b.HPFile = 400;
        b.HPStartIndex = 10;
        b.HPTextOffsetX = 5;
        b.HPTextOffsetY = 6;
        b.StandDrawMode = TCustomDrawMode.mdmBlend;
        b.StandEffectDrawMode = TCustomDrawMode.mdmNormal;
        b.ActionDrawMode = TCustomDrawMode.mdmBlend;
        b.ActionEffectDrawMode = TCustomDrawMode.mdmNormal;
        b.KeepPlayFile = 12;
        b.KeepPlayIndex = 34;
        b.KeepPlayCount = 5;
        b.KeepPlayTime = 60;
        b.KeepPlayBlendDraw = 0;
        b.KeepPlayOffsetX = -7;
        b.KeepPlayOffsetY = -8;
        b.DrawOrder = TCustomNpcDrawOrder.ndoEff_Chr_Keep;
        cfg.ClientBaseConfig = b;

        var d3 = cfg.DirActions[2];
        d3.Enabled = 0;
        d3.Std_File = 7;
        d3.Std_Index = 8;
        d3.Std_Count = 9;
        d3.Std_Time = 120;
        d3.Std_EffFile = 10;
        d3.Std_EffIndex = 11;
        d3.Act_File = 12;
        d3.Act_Index = 13;
        d3.Act_Count = 14;
        d3.Act_Time = 150;
        d3.Act_EffFile = 15;
        d3.Act_EffIndex = 16;
        cfg.DirActions[2] = d3;

        cfg.SaveToIniFile();

        var loaded = new TCustomNpcConfig(11);
        var lb = loaded.ClientBaseConfig;
        Assert.Equal(1, lb.HPBgOffsetX);
        Assert.Equal(2, lb.HPBgOffsetY);
        Assert.Equal(3, lb.HPOffsetX);
        Assert.Equal(4, lb.HPOffsetY);
        Assert.Equal(400, lb.HPFile);
        Assert.Equal(10, lb.HPStartIndex);
        Assert.Equal(5, lb.HPTextOffsetX);
        Assert.Equal(6, lb.HPTextOffsetY);
        Assert.Equal(TCustomDrawMode.mdmBlend, lb.StandDrawMode);
        Assert.Equal(TCustomDrawMode.mdmNormal, lb.StandEffectDrawMode);
        Assert.Equal(TCustomDrawMode.mdmBlend, lb.ActionDrawMode);
        Assert.Equal(TCustomDrawMode.mdmNormal, lb.ActionEffectDrawMode);
        Assert.Equal(12, lb.KeepPlayFile);
        Assert.Equal(34, lb.KeepPlayIndex);
        Assert.Equal(5, lb.KeepPlayCount);
        Assert.Equal(60, lb.KeepPlayTime);
        Assert.Equal(0, lb.KeepPlayBlendDraw);
        Assert.Equal(-7, lb.KeepPlayOffsetX);
        Assert.Equal(-8, lb.KeepPlayOffsetY);
        Assert.Equal(TCustomNpcDrawOrder.ndoEff_Chr_Keep, lb.DrawOrder);

        var ld3 = loaded.DirActions[2];
        Assert.Equal(0, ld3.Enabled);
        Assert.Equal(7, ld3.Std_File);
        Assert.Equal(8, ld3.Std_Index);
        Assert.Equal(9, ld3.Std_Count);
        Assert.Equal(120, ld3.Std_Time);
        Assert.Equal(10, ld3.Std_EffFile);
        Assert.Equal(11, ld3.Std_EffIndex);
        Assert.Equal(12, ld3.Act_File);
        Assert.Equal(13, ld3.Act_Index);
        Assert.Equal(14, ld3.Act_Count);
        Assert.Equal(150, ld3.Act_Time);
        Assert.Equal(15, ld3.Act_EffFile);
        Assert.Equal(16, ld3.Act_EffIndex);

        // 未改动方向保持默认
        Assert.Equal(1, loaded.DirActions[0].Enabled);
        Assert.Equal(-1, loaded.DirActions[0].Std_Index);
    }

    [Fact]
    public void LoadFromIniFile_MissingFileKeepsDefaults()
    {
        var cfg = new TCustomNpcConfig(999);
        Assert.Equal(-1, cfg.ClientBaseConfig.HPFile);
        Assert.Equal(TCustomDrawMode.mdmNormal, cfg.ClientBaseConfig.StandDrawMode);
        Assert.Equal(1, cfg.DirActions[0].Enabled);
    }

    [Fact]
    public void LoadFromIniFile_RejectsOutOfRangeDrawModes()
    {
        Directory.CreateDirectory(M2Config.sSmartNpcDir);
        File.WriteAllText(M2Config.sSmartNpcDir + "3.ini",
            "[BaseConfig]" + Environment.NewLine
            + "StandDrawMode=99" + Environment.NewLine          // 越界 → 保持默认 mdmNormal
            + "StandEffectDrawMode=-5" + Environment.NewLine    // 越界 → 保持默认 mdmBlend
            + "ActionDrawMode=0" + Environment.NewLine          // 合法 → mdmBlend
            + "ActionEffectDrawMode=1" + Environment.NewLine    // 合法 → mdmNormal
            + "DrawOrder=9" + Environment.NewLine               // 越界 → 保持 ndoKeep_Chr_Eff
            + "HPFile=500" + Environment.NewLine, GXX.Core.EncodingInit.GBK);

        var cfg = new TCustomNpcConfig(3);
        Assert.Equal(TCustomDrawMode.mdmNormal, cfg.ClientBaseConfig.StandDrawMode);
        Assert.Equal(TCustomDrawMode.mdmBlend, cfg.ClientBaseConfig.StandEffectDrawMode);
        Assert.Equal(TCustomDrawMode.mdmBlend, cfg.ClientBaseConfig.ActionDrawMode);
        Assert.Equal(TCustomDrawMode.mdmNormal, cfg.ClientBaseConfig.ActionEffectDrawMode);
        Assert.Equal(TCustomNpcDrawOrder.ndoKeep_Chr_Eff, cfg.ClientBaseConfig.DrawOrder);
        Assert.Equal(500, cfg.ClientBaseConfig.HPFile);
    }

    [Fact]
    public void LoadFromIniFile_ReadsDirSectionsAndBoolForms()
    {
        Directory.CreateDirectory(M2Config.sSmartNpcDir);
        File.WriteAllText(M2Config.sSmartNpcDir + "4.ini",
            "[BaseConfig]" + Environment.NewLine
            + "KeepPlayBlendDraw=-1" + Environment.NewLine       // Delphi True
            + "[Dir2]" + Environment.NewLine
            + "Enabled=false" + Environment.NewLine              // 显式 false
            + "StdFile=21" + Environment.NewLine
            + "[Dir5]" + Environment.NewLine
            + "Enabled=0" + Environment.NewLine
            + "ActIndex=77" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        var cfg = new TCustomNpcConfig(4);
        Assert.Equal(1, cfg.ClientBaseConfig.KeepPlayBlendDraw);
        Assert.Equal(0, cfg.DirActions[1].Enabled);
        Assert.Equal(21, cfg.DirActions[1].Std_File);
        Assert.Equal(0, cfg.DirActions[4].Enabled);
        Assert.Equal(77, cfg.DirActions[4].Act_Index);
        Assert.Equal(1, cfg.DirActions[0].Enabled);       // 未出现 → 默认 True
    }

    // ================= .dat 打包 =================

    [Fact]
    public void SaveCustomNpcClientConfigs_HeaderAndRecordSize()
    {
        var cfgs = new List<TCustomNpcConfig> { new(100), new(200) };
        string datPath = Path.Combine(_dir, "npc.dat");
        CustomNpcUtils.SaveCustomNpcClientConfigs(cfgs, datPath);

        byte[] data = File.ReadAllBytes(datPath);
        int recordSize = CustomNpcUtils.ClientRecordSize;

        // 头 = GUID(16) + Count(4) + RecordSize(4) + CRC(4) = 28，记录段自 28 起
        Assert.Equal(28 + 2 * recordSize, data.Length);
        Assert.Equal(28, HeaderSize);
        Assert.Equal(CustomNpcUtils.ClientCustomNpcConfigFlag.ToByteArray(), data[..16]);
        Assert.Equal(2, BitConverter.ToInt32(data, 16));
        Assert.Equal(recordSize, BitConverter.ToInt32(data, 20));

        // 记录起始于 24：wNpcAppr(偏移 0) / wDirCount(偏移 2)；第二条紧随其后
        Assert.Equal(100, BitConverter.ToUInt16(data, 28));
        Assert.Equal(8, BitConverter.ToUInt16(data, 30));                    // 8 方向默认全启用
        Assert.Equal(200, BitConverter.ToUInt16(data, 28 + recordSize));
        // size = wNpcAppr(2) + wDirCount(2) + BaseConfig(62) + Actions(8×28)
        Assert.Equal(CustomNpcUtils.NpcBaseConfigSize + 4 + CustomNpcUtils.NpcDirActionSize * 8, recordSize);
        Assert.Equal(62, CustomNpcUtils.NpcBaseConfigSize);
        Assert.Equal(28, CustomNpcUtils.NpcDirActionSize);
        Assert.Equal(290, recordSize);
    }

    [Fact]
    public void SaveCustomNpcClientConfigs_CrcCoversBodyOnly()
    {
        var cfgs = new List<TCustomNpcConfig> { new(50) };
        string datPath = Path.Combine(_dir, "crc.dat");
        CustomNpcUtils.SaveCustomNpcClientConfigs(cfgs, datPath);
        byte[] data = File.ReadAllBytes(datPath);

        // CRC 位于偏移 24（GUID 16 + Count 4 + RecordSize 4）
        uint stored = BitConverter.ToUInt32(data, 24);
        var payload = new byte[data.Length - HeaderSize];
        Array.Copy(data, HeaderSize, payload, 0, payload.Length);
        Assert.Equal(GXX.Core.Crypto.CheckCrc.BufferCRC(payload, payload.Length), stored);
    }

    [Fact]
    public void SaveCustomNpcClientConfigs_EnabledDirsComeFirst()
    {
        var cfg = new TCustomNpcConfig(77);
        // 关闭方向 0 与 3，其余启用 → 期望顺序 [1,2,4,5,6,7,0,3]，wDirCount = 6
        cfg.DirActions[0] = WithDisabled(cfg.DirActions[0]);
        cfg.DirActions[3] = WithDisabled(cfg.DirActions[3]);
        cfg.DirActions[1] = WithStdFile(cfg.DirActions[1], 101);
        cfg.DirActions[2] = WithStdFile(cfg.DirActions[2], 102);
        cfg.DirActions[4] = WithStdFile(cfg.DirActions[4], 104);
        cfg.DirActions[5] = WithStdFile(cfg.DirActions[5], 105);
        cfg.DirActions[6] = WithStdFile(cfg.DirActions[6], 106);
        cfg.DirActions[7] = WithStdFile(cfg.DirActions[7], 107);
        cfg.DirActions[0] = WithStdFile(cfg.DirActions[0], 100);
        cfg.DirActions[3] = WithStdFile(cfg.DirActions[3], 103);

        string datPath = Path.Combine(_dir, "order.dat");
        CustomNpcUtils.SaveCustomNpcClientConfigs(new List<TCustomNpcConfig> { cfg }, datPath);
        byte[] data = File.ReadAllBytes(datPath);

        int recordOffset = HeaderSize;
        Assert.Equal(6, BitConverter.ToUInt16(data, recordOffset + 2));   // wDirCount
        Assert.Equal(290, CustomNpcUtils.ClientRecordSize);
        Assert.Equal(6, (int)cfg.DirActions[1].Enabled + (int)cfg.DirActions[2].Enabled
            + (int)cfg.DirActions[4].Enabled + (int)cfg.DirActions[5].Enabled
            + (int)cfg.DirActions[6].Enabled + (int)cfg.DirActions[7].Enabled);

        int baseConfigSize = CustomNpcUtils.NpcBaseConfigSize;
        int actionsOffset = recordOffset + 4 + baseConfigSize;
        int actionSize = CustomNpcUtils.NpcDirActionSize;

        // 前 6 项为启用方向（按下标序 1,2,4,5,6,7）
        int[] expectedFirst = { 101, 102, 104, 105, 106, 107 };
        for (int i = 0; i < expectedFirst.Length; i++)
        {
            int stdFile = BitConverter.ToUInt16(data, actionsOffset + i * actionSize + 4);
            Assert.Equal(expectedFirst[i], stdFile);
        }
        // 后 2 项为未启用方向（按下标序 0,3）
        Assert.Equal(100, BitConverter.ToUInt16(data, actionsOffset + 6 * actionSize + 4));
        Assert.Equal(103, BitConverter.ToUInt16(data, actionsOffset + 7 * actionSize + 4));

        // 未启用项的 Enabled 仍为 0
        Assert.Equal(0, BitConverter.ToInt32(data, actionsOffset + 6 * actionSize));
    }

    /// <summary>.dat 头长度：GUID(16) + Count(4) + RecordSize(4) + CRC(4)。</summary>
    private const int HeaderSize = 28;

    private static TNpcDirAction WithDisabled(TNpcDirAction a)
    {
        a.Enabled = 0;
        return a;
    }

    private static TNpcDirAction WithStdFile(TNpcDirAction a, ushort file)
    {
        a.Std_File = file;
        return a;
    }
}
