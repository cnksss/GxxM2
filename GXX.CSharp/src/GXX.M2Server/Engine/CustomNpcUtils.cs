using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Crypto;

namespace GXX.M2Server.Engine;

// ============================================================================
// uCustomNpcUtils.pas 1:1 移植（批次J65，345 行）
// 自定义 NPC 客户端配置：8 方向站立/动作图库 + 血条/常驻特效/绘制顺序，
// 落盘 &lt;sSmartNpcDir&gt;&lt;NpcAppr&gt;.ini（BaseConfig + Dir1..Dir8），
// 并打包为登录器 .dat（GUID 头 + 条数 + 记录尺寸 + CRC32）。
// ============================================================================

/// <summary>uCustomNpcUtils.pas TNpcActionType 1:1。</summary>
public enum TNpcActionType
{
    atStand,
    atAction,
}

/// <summary>TNpcDirActions 的托管镜像（8 方向，按 DR_UP..DR_UPLEFT 下标 0..7）。</summary>
public sealed class TNpcDirActionList
{
    public const int Count = 8;

    private readonly TNpcDirAction[] _items = new TNpcDirAction[Count];

    public TNpcDirAction this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public TNpcDirAction[] Raw => _items;
}

/// <summary>
/// uCustomNpcUtils.pas TCustomNpcConfig 1:1（批次J65）：
/// ctor 全量默认（HPFile/HPStartIndex/KeepPlayIndex/各方向 Index = -1、Time = 200、
/// KeepPlayTime = 100、KeepPlayBlendDraw = True、DrawOrder = ndoKeep_Chr_Eff）+ LoadFromIniFile。
/// </summary>
public sealed class TCustomNpcConfig
{
    public ushort FNpcAppr;
    public bool FIsChanged;

    public TNpcBaseConfig ClientBaseConfig;
    public readonly TNpcDirActionList DirActions = new();

    public ushort NpcAppr => FNpcAppr;
    public bool IsChanged => FIsChanged;

    public TCustomNpcConfig(ushort aNpcAppr)
    {
        FIsChanged = false;
        FNpcAppr = aNpcAppr;

        ClientBaseConfig.HPBgOffsetX = 0;
        ClientBaseConfig.HPBgOffsetY = 0;
        ClientBaseConfig.HPOffsetX = 0;
        ClientBaseConfig.HPOffsetY = 0;
        ClientBaseConfig.HPFile = -1;
        ClientBaseConfig.HPStartIndex = -1;
        ClientBaseConfig.HPTextOffsetX = 0;
        ClientBaseConfig.HPTextOffsetY = 0;

        ClientBaseConfig.StandDrawMode = TCustomDrawMode.mdmNormal;
        ClientBaseConfig.ActionDrawMode = TCustomDrawMode.mdmNormal;
        ClientBaseConfig.StandEffectDrawMode = TCustomDrawMode.mdmBlend;
        ClientBaseConfig.ActionEffectDrawMode = TCustomDrawMode.mdmBlend;

        ClientBaseConfig.KeepPlayFile = 0;
        ClientBaseConfig.KeepPlayIndex = -1;
        ClientBaseConfig.KeepPlayCount = 0;
        ClientBaseConfig.KeepPlayTime = 100;
        ClientBaseConfig.KeepPlayBlendDraw = 1;
        ClientBaseConfig.KeepPlayOffsetX = 0;
        ClientBaseConfig.KeepPlayOffsetY = 0;

        ClientBaseConfig.DrawOrder = TCustomNpcDrawOrder.ndoKeep_Chr_Eff;

        for (int i = 0; i < TNpcDirActionList.Count; i++)
            DirActions[i] = DefaultDirAction();

        LoadFromIniFile();
    }

    /// <summary>单方向默认值（ctor 循环体 1:1）。</summary>
    public static TNpcDirAction DefaultDirAction() => new()
    {
        Enabled = 1,          // LongBool True
        Std_File = 0,
        Std_Index = -1,
        Std_Count = 0,
        Std_Time = 200,
        Std_EffFile = 0,
        Std_EffIndex = -1,
        Act_File = 0,
        Act_Index = -1,
        Act_Count = 0,
        Act_Time = 200,
        Act_EffFile = 0,
        Act_EffIndex = -1,
    };

    public void SetChanged(bool value = true) => FIsChanged = value;

    /// <summary>配置文件路径：&lt;sSmartNpcDir&gt;&lt;FNpcAppr&gt;.ini。</summary>
    public string IniPath => M2Config.sSmartNpcDir + FNpcAppr.ToString(CultureInfo.InvariantCulture) + ".ini";

    /// <summary>
    /// LoadFromIniFile（190-273）1:1：文件缺失直接返回；
    /// BaseConfig 14 键（四个绘制模式读后做枚举范围校验）+ Dir1..Dir8 各 12 键。
    /// </summary>
    public void LoadFromIniFile()
    {
        FIsChanged = false;
        string fileName = IniPath;
        if (!File.Exists(fileName))
            return;

        var ini = TGroupItems.ReadIniAll(fileName);
        const string section = "BaseConfig";

        ClientBaseConfig.HPBgOffsetX = TGroupItems.ReadIniInt(ini, section, "HPBgOffsetX", ClientBaseConfig.HPBgOffsetX);
        ClientBaseConfig.HPBgOffsetY = TGroupItems.ReadIniInt(ini, section, "HPBgOffsetY", ClientBaseConfig.HPBgOffsetY);

        ClientBaseConfig.HPOffsetX = TGroupItems.ReadIniInt(ini, section, "HPOffsetX", ClientBaseConfig.HPOffsetX);
        ClientBaseConfig.HPOffsetY = TGroupItems.ReadIniInt(ini, section, "HPOffsetY", ClientBaseConfig.HPOffsetY);

        ClientBaseConfig.HPFile = TGroupItems.ReadIniInt(ini, section, "HPFile", ClientBaseConfig.HPFile);
        ClientBaseConfig.HPStartIndex = TGroupItems.ReadIniInt(ini, section, "HPStartIndex", ClientBaseConfig.HPStartIndex);

        ClientBaseConfig.HPTextOffsetX = TGroupItems.ReadIniInt(ini, section, "HPTextOffsetX", ClientBaseConfig.HPTextOffsetX);
        ClientBaseConfig.HPTextOffsetY = TGroupItems.ReadIniInt(ini, section, "HPTextOffsetY", ClientBaseConfig.HPTextOffsetY);

        // 四个绘制模式：读后做枚举范围校验（越界保持原值）
        int drawMode = TGroupItems.ReadIniInt(ini, section, "StandDrawMode", (int)ClientBaseConfig.StandDrawMode);
        if (IsValidDrawMode(drawMode))
            ClientBaseConfig.StandDrawMode = (TCustomDrawMode)drawMode;

        drawMode = TGroupItems.ReadIniInt(ini, section, "StandEffectDrawMode", (int)ClientBaseConfig.StandEffectDrawMode);
        if (IsValidDrawMode(drawMode))
            ClientBaseConfig.StandEffectDrawMode = (TCustomDrawMode)drawMode;

        drawMode = TGroupItems.ReadIniInt(ini, section, "ActionDrawMode", (int)ClientBaseConfig.ActionDrawMode);
        if (IsValidDrawMode(drawMode))
            ClientBaseConfig.ActionDrawMode = (TCustomDrawMode)drawMode;

        drawMode = TGroupItems.ReadIniInt(ini, section, "ActionEffectDrawMode", (int)ClientBaseConfig.ActionEffectDrawMode);
        if (IsValidDrawMode(drawMode))
            ClientBaseConfig.ActionEffectDrawMode = (TCustomDrawMode)drawMode;

        drawMode = TGroupItems.ReadIniInt(ini, section, "DrawOrder", (int)ClientBaseConfig.DrawOrder);
        if (drawMode >= (int)TCustomNpcDrawOrder.ndoKeep_Chr_Eff && drawMode <= (int)TCustomNpcDrawOrder.ndoEff_Chr_Keep)
            ClientBaseConfig.DrawOrder = (TCustomNpcDrawOrder)drawMode;

        ClientBaseConfig.KeepPlayFile = TGroupItems.ReadIniInt(ini, section, "KeepPlayFile", ClientBaseConfig.KeepPlayFile);
        ClientBaseConfig.KeepPlayIndex = TGroupItems.ReadIniInt(ini, section, "KeepPlayIndex", ClientBaseConfig.KeepPlayIndex);
        ClientBaseConfig.KeepPlayCount = TGroupItems.ReadIniInt(ini, section, "KeepPlayCount", ClientBaseConfig.KeepPlayCount);
        ClientBaseConfig.KeepPlayTime = TGroupItems.ReadIniInt(ini, section, "KeepPlayTime", ClientBaseConfig.KeepPlayTime);
        ClientBaseConfig.KeepPlayOffsetX = TGroupItems.ReadIniInt(ini, section, "KeepPlayOffsetX", ClientBaseConfig.KeepPlayOffsetX);
        ClientBaseConfig.KeepPlayOffsetY = TGroupItems.ReadIniInt(ini, section, "KeepPlayOffsetY", ClientBaseConfig.KeepPlayOffsetY);
        ClientBaseConfig.KeepPlayBlendDraw = ReadBool(ini, section, "KeepPlayBlendDraw", ClientBaseConfig.KeepPlayBlendDraw != 0) ? (byte)1 : (byte)0;

        for (int i = 0; i < TNpcDirActionList.Count; i++)
        {
            var dirAction = DirActions[i];
            string sectionName = CustomNpcUtils.NpcDirSections[i];

            dirAction.Enabled = ReadBool(ini, sectionName, "Enabled", dirAction.Enabled != 0) ? 1 : 0;

            dirAction.Std_File = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "StdFile", dirAction.Std_File);
            dirAction.Std_Index = (short)TGroupItems.ReadIniInt(ini, sectionName, "StdIndex", dirAction.Std_Index);
            dirAction.Std_Count = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "StdCount", dirAction.Std_Count);
            dirAction.Std_Time = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "StdTime", dirAction.Std_Time);
            dirAction.Std_EffFile = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "StdEffFile", dirAction.Std_EffFile);
            dirAction.Std_EffIndex = (short)TGroupItems.ReadIniInt(ini, sectionName, "StdEffIndex", dirAction.Std_EffIndex);

            dirAction.Act_File = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "ActFile", dirAction.Act_File);
            dirAction.Act_Index = (short)TGroupItems.ReadIniInt(ini, sectionName, "ActIndex", dirAction.Act_Index);
            dirAction.Act_Count = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "ActCount", dirAction.Act_Count);
            dirAction.Act_Time = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "ActTime", dirAction.Act_Time);
            dirAction.Act_EffFile = (ushort)TGroupItems.ReadIniInt(ini, sectionName, "ActEffFile", dirAction.Act_EffFile);
            dirAction.Act_EffIndex = (short)TGroupItems.ReadIniInt(ini, sectionName, "ActEffIndex", dirAction.Act_EffIndex);

            DirActions[i] = dirAction;
        }
    }

    private static bool IsValidDrawMode(int value)
        => value >= (int)TCustomDrawMode.mdmBlend && value <= (int)TCustomDrawMode.mdmNormal;

    private static bool ReadBool(Dictionary<string, Dictionary<string, string>> ini,
        string section, string key, bool def)
    {
        string raw = TGroupItems.ReadIniString(ini, section, key, def ? "1" : "0").Trim();
        if (raw.Length == 0)
            return def;
        if (raw == "0" || raw.Equals("false", StringComparison.OrdinalIgnoreCase))
            return false;
        if (raw == "-1" || raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;
        return def;
    }

    /// <summary>
    /// SaveToIniFile（275-343）1:1：先复位 IsChanged，目录不存在则创建；
    /// BaseConfig 20 键（顺序与原文一致）+ Dir1..Dir8 各 12 键。
    /// </summary>
    public void SaveToIniFile()
    {
        FIsChanged = false;

        if (!string.IsNullOrEmpty(M2Config.sSmartNpcDir) && !Directory.Exists(M2Config.sSmartNpcDir))
            Directory.CreateDirectory(M2Config.sSmartNpcDir);

        string fileName = IniPath;
        var sb = new StringBuilder();

        const string section = "BaseConfig";
        sb.Append("[BaseConfig]").Append("\r\n");
        sb.Append("HPBgOffsetX=").Append(ClientBaseConfig.HPBgOffsetX).Append("\r\n");
        sb.Append("HPBgOffsetY=").Append(ClientBaseConfig.HPBgOffsetY).Append("\r\n");
        sb.Append("HPOffsetX=").Append(ClientBaseConfig.HPOffsetX).Append("\r\n");
        sb.Append("HPOffsetY=").Append(ClientBaseConfig.HPOffsetY).Append("\r\n");
        sb.Append("HPFile=").Append(ClientBaseConfig.HPFile).Append("\r\n");
        sb.Append("HPStartIndex=").Append(ClientBaseConfig.HPStartIndex).Append("\r\n");
        sb.Append("HPTextOffsetX=").Append(ClientBaseConfig.HPTextOffsetX).Append("\r\n");
        sb.Append("HPTextOffsetY=").Append(ClientBaseConfig.HPTextOffsetY).Append("\r\n");
        sb.Append("StandDrawMode=").Append((int)ClientBaseConfig.StandDrawMode).Append("\r\n");
        sb.Append("StandEffectDrawMode=").Append((int)ClientBaseConfig.StandEffectDrawMode).Append("\r\n");
        sb.Append("ActionDrawMode=").Append((int)ClientBaseConfig.ActionDrawMode).Append("\r\n");
        sb.Append("ActionEffectDrawMode=").Append((int)ClientBaseConfig.ActionEffectDrawMode).Append("\r\n");
        sb.Append("DrawOrder=").Append((int)ClientBaseConfig.DrawOrder).Append("\r\n");
        sb.Append("KeepPlayFile=").Append(ClientBaseConfig.KeepPlayFile).Append("\r\n");
        sb.Append("KeepPlayIndex=").Append(ClientBaseConfig.KeepPlayIndex).Append("\r\n");
        sb.Append("KeepPlayCount=").Append(ClientBaseConfig.KeepPlayCount).Append("\r\n");
        sb.Append("KeepPlayTime=").Append(ClientBaseConfig.KeepPlayTime).Append("\r\n");
        sb.Append("KeepPlayOffsetX=").Append(ClientBaseConfig.KeepPlayOffsetX).Append("\r\n");
        sb.Append("KeepPlayOffsetY=").Append(ClientBaseConfig.KeepPlayOffsetY).Append("\r\n");
        sb.Append("KeepPlayBlendDraw=").Append(ClientBaseConfig.KeepPlayBlendDraw != 0 ? 1 : 0).Append("\r\n");

        for (int i = 0; i < TNpcDirActionList.Count; i++)
        {
            var dirAction = DirActions[i];
            string sectionName = CustomNpcUtils.NpcDirSections[i];

            sb.Append('[').Append(sectionName).Append(']').Append("\r\n");
            sb.Append("Enabled=").Append(dirAction.Enabled != 0 ? 1 : 0).Append("\r\n");
            sb.Append("StdFile=").Append(dirAction.Std_File).Append("\r\n");
            sb.Append("StdIndex=").Append(dirAction.Std_Index).Append("\r\n");
            sb.Append("StdCount=").Append(dirAction.Std_Count).Append("\r\n");
            sb.Append("StdTime=").Append(dirAction.Std_Time).Append("\r\n");
            sb.Append("StdEffFile=").Append(dirAction.Std_EffFile).Append("\r\n");
            sb.Append("StdEffIndex=").Append(dirAction.Std_EffIndex).Append("\r\n");
            sb.Append("ActFile=").Append(dirAction.Act_File).Append("\r\n");
            sb.Append("ActIndex=").Append(dirAction.Act_Index).Append("\r\n");
            sb.Append("ActCount=").Append(dirAction.Act_Count).Append("\r\n");
            sb.Append("ActTime=").Append(dirAction.Act_Time).Append("\r\n");
            sb.Append("ActEffFile=").Append(dirAction.Act_EffFile).Append("\r\n");
            sb.Append("ActEffIndex=").Append(dirAction.Act_EffIndex).Append("\r\n");
        }

        try
        {
            File.WriteAllText(fileName, sb.ToString(), Encoding.GetEncoding(936));
        }
        catch
        {
            // Delphi TIniFile 写失败忽略
        }
    }
}

/// <summary>uCustomNpcUtils.pas 单元级（批次J65）：名称表 + .dat 打包。</summary>
public static class CustomNpcUtils
{
    /// <summary>NpcActionNames[TNpcActionType]（站立/动作）。</summary>
    public static readonly string[] NpcActionNames = { "站立", "动作" };

    /// <summary>NpcDirNames[DR_UP..DR_UPLEFT]（方向1..方向8）。</summary>
    public static readonly string[] NpcDirNames =
        { "方向1", "方向2", "方向3", "方向4", "方向5", "方向6", "方向7", "方向8" };

    /// <summary>NpcDirSections[DR_UP..DR_UPLEFT]（Dir1..Dir8）。</summary>
    public static readonly string[] NpcDirSections =
        { "Dir1", "Dir2", "Dir3", "Dir4", "Dir5", "Dir6", "Dir7", "Dir8" };

    /// <summary>ClientCustomNPCConfigFlag: TGUID（登录器 .dat 头）。</summary>
    public static readonly Guid ClientCustomNpcConfigFlag = new("3630817C-5AFB-4A06-873A-C7A2F8AF1F6C");

    /// <summary>g_CustomNpcList（窗体装载用；Delphi 由 uFrmCustomNpc 持有）。</summary>
    public static readonly List<TCustomNpcConfig> CustomNpcList = new();

    /// <summary>SizeOf(TClientCustomNpcConfig)（Pack=1 运行时布局）。</summary>
    public static int ClientRecordSize => System.Runtime.CompilerServices.Unsafe.SizeOf<TClientCustomNpcConfig>();

    /// <summary>SizeOf(TNpcBaseConfig)（Pack=1 运行时布局）。</summary>
    public static int NpcBaseConfigSize => System.Runtime.CompilerServices.Unsafe.SizeOf<TNpcBaseConfig>();

    /// <summary>SizeOf(TNpcDirAction)（Pack=1 运行时布局）。</summary>
    public static int NpcDirActionSize => System.Runtime.CompilerServices.Unsafe.SizeOf<TNpcDirAction>();

    /// <summary>测试隔离。</summary>
    public static void ResetForTests() => CustomNpcList.Clear();

    /// <summary>
    /// SaveCustomNpcClientConfigs（52-119）1:1：
    /// [GUID(16)] [Count(4)] [RecordSize(4)] [CRC(4)] 后接 Count 条 TClientCustomNpcConfig；
    /// 每条内 DirActions 重排为「启用优先」（Enabled 的按序放前，其余按序放后，wDirCount = 启用数）；
    /// CRC 为跳过 24 字节头后的 BufferCRC，回写至偏移 20。
    /// </summary>
    public static void SaveCustomNpcClientConfigs(IReadOnlyList<TCustomNpcConfig> npcConfigs, string fileName)
    {
        // Pack=1 结构必须用 Unsafe.SizeOf（运行时布局）；Marshal.SizeOf 走 interop 布局会多出对齐填充
        int recordSize = System.Runtime.CompilerServices.Unsafe.SizeOf<TClientCustomNpcConfig>();

        // Delphi: MS 先写 [GUID][Count][RecordSize][CRC 占位]，逐条追加记录，
        // 然后在最后 4 条记录的首字节上算 CRC 并回写偏移 20。
        // 关键：CRC 只覆盖"记录段"，不含头 24 字节中的 CRC 占位本身。
        var body = new List<byte>(npcConfigs.Count * recordSize);

        foreach (var npcConfig in npcConfigs)
        {
            var clientConfig = new TClientCustomNpcConfig
            {
                wNpcAppr = npcConfig.FNpcAppr,
                BaseConfig = npcConfig.ClientBaseConfig,
            };

            // 将选中的放在前面 chongchong 2016-03-21
            int index = 0;
            for (int j = 0; j < TNpcDirActionList.Count; j++)
            {
                if (npcConfig.DirActions[j].Enabled != 0)
                {
                    clientConfig.Actions[index] = npcConfig.DirActions[j];
                    index++;
                }
            }
            clientConfig.wDirCount = (ushort)index;

            for (int j = 0; j < TNpcDirActionList.Count; j++)
            {
                if (npcConfig.DirActions[j].Enabled == 0)
                {
                    clientConfig.Actions[index] = npcConfig.DirActions[j];
                    index++;
                }
            }

            body.AddRange(StructToBytes(clientConfig));
        }

        byte[] bodyBytes = body.ToArray();
        uint crc = CheckCrc.BufferCRC(bodyBytes, bodyBytes.Length);

        var payload = new List<byte>(24 + bodyBytes.Length);
        payload.AddRange(ClientCustomNpcConfigFlag.ToByteArray());
        payload.AddRange(BitConverter.GetBytes(npcConfigs.Count));
        payload.AddRange(BitConverter.GetBytes(recordSize));
        payload.AddRange(BitConverter.GetBytes(crc));
        payload.AddRange(bodyBytes);

        try
        {
            File.WriteAllBytes(fileName, payload.ToArray());
        }
        catch
        {
            // 忽略
        }
    }

    /// <summary>
    /// 结构 → 字节：必须用 Unsafe.SizeOf 的运行时布局逐字节复制。
    /// Marshal.SizeOf/StructureToPtr 走 interop 布局，对 Pack=1 结构会多出对齐填充，
    /// 与头部声明的记录尺寸不一致并污染 CRC。
    /// </summary>
    public static byte[] StructToBytes<T>(T value) where T : unmanaged
    {
        unsafe
        {
            var span = System.Runtime.InteropServices.MemoryMarshal.CreateReadOnlySpan(ref value, 1);
            return System.Runtime.InteropServices.MemoryMarshal.AsBytes(span).ToArray();
        }
    }
}
