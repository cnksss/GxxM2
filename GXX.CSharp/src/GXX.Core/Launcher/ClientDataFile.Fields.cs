using System.Text;

namespace GXX.Core.Launcher;

/// <summary>
/// <see cref="ClientDataFile"/> 的具名字段访问（偏移全部取自 <see cref="ClientDataFields"/> 的实测校准结果）。
/// 每个访问器都直接读写记录字节，未涉及的字节保持不变。
/// </summary>
public sealed partial class ClientDataFile
{
    // ---------------- 顶层：名称 / 密码 ----------------

    /// <summary>必备补丁名（如 "NewopUI.Pak"），对应配置器的「补丁文件」。</summary>
    public string GamePlanName
    {
        get => GetShortString(ClientDataFields.OffGamePlanName, ClientDataFields.CapGamePlanName);
        set => SetShortString(ClientDataFields.OffGamePlanName, ClientDataFields.CapGamePlanName, value);
    }

    /// <summary>RunGate 网关密码，对应配置器的「RunGatePassword / 登录密码」。</summary>
    public string RunGatePassWord
    {
        get => GetShortString(ClientDataFields.OffRunGatePassWord, ClientDataFields.CapRunGatePassWord);
        set => SetShortString(ClientDataFields.OffRunGatePassWord, ClientDataFields.CapRunGatePassWord, value);
    }

    /// <summary>资源目录名（如 "KMCQ"），对应配置器的「Resources目录」。</summary>
    public string ResourcesDir
    {
        get => GetShortString(ClientDataFields.OffResourcesDir, ClientDataFields.CapResourcesDir);
        set => SetShortString(ClientDataFields.OffResourcesDir, ClientDataFields.CapResourcesDir, value);
    }

    /// <summary>登录器版本号（日期串，如 "2026-06-06"），对应配置器的「版本号」。</summary>
    public string GameLoginVersion
    {
        get => GetShortString(ClientDataFields.OffGameLoginVersion, ClientDataFields.CapGameLoginVersion);
        set => SetShortString(ClientDataFields.OffGameLoginVersion, ClientDataFields.CapGameLoginVersion, value);
    }

    // ---------------- 单值开关 ----------------

    public bool ChangeScreenBitCount
    {
        get => GetBool(ClientDataFields.OffChangeScreenBitCount);
        set => SetBool(ClientDataFields.OffChangeScreenBitCount, value);
    }

    public bool ShowOpenDoor
    {
        get => GetBool(ClientDataFields.OffShowOpenDoor);
        set => SetBool(ClientDataFields.OffShowOpenDoor, value);
    }

    public bool Show1024
    {
        get => GetBool(ClientDataFields.OffShow1024);
        set => SetBool(ClientDataFields.OffShow1024, value);
    }

    // ---------------- 内挂复选框 ----------------

    /// <summary>内挂配置复选框个数（真机实测 = 101）。</summary>
    public int ClientConfigCount => ClientDataFields.CountClientConfigs;

    /// <summary>第 i 个内挂复选框（对应 <c>TConfigChecked</c> 枚举语义）。</summary>
    public bool GetClientConfig(int i)
    {
        if ((uint)i >= ClientDataFields.CountClientConfigs) throw new ArgumentOutOfRangeException(nameof(i));
        return GetBool(ClientDataFields.OffClientConfigs + i);
    }

    public void SetClientConfig(int i, bool value)
    {
        if ((uint)i >= ClientDataFields.CountClientConfigs) throw new ArgumentOutOfRangeException(nameof(i));
        SetBool(ClientDataFields.OffClientConfigs + i, value);
    }

    public bool ClientConfigEx0
    {
        get => GetBool(ClientDataFields.OffClientConfigsEx);
        set => SetBool(ClientDataFields.OffClientConfigsEx, value);
    }

    /// <summary>手动技能编号 [0..4]。</summary>
    public uint GetManualCustomHit(int i)
    {
        if ((uint)i >= 5) throw new ArgumentOutOfRangeException(nameof(i));
        return GetUInt32(ClientDataFields.OffHumManuallyCustomHits + i * 4);
    }

    public void SetManualCustomHit(int i, uint value)
    {
        if ((uint)i >= 5) throw new ArgumentOutOfRangeException(nameof(i));
        SetUInt32(ClientDataFields.OffHumManuallyCustomHits + i * 4, value);
    }

    // ---------------- 文本数组 ----------------

    /// <summary>攻击模式显示文字 [0..7]（<c>string[40]</c>）。</summary>
    public string GetAttackModeText(int i) =>
        GetShortString(ClientDataFields.OffAttackModeTexts + i * (ClientDataFields.CapAttackModeText + 1),
                       ClientDataFields.CapAttackModeText);

    public void SetAttackModeText(int i, string value) =>
        SetShortString(ClientDataFields.OffAttackModeTexts + i * (ClientDataFields.CapAttackModeText + 1),
                       ClientDataFields.CapAttackModeText, value);

    /// <summary>经验提示文字（<c>string[60]</c>）。</summary>
    public string ExpAddHintText
    {
        get => GetShortString(ClientDataFields.OffExpAddHintText, ClientDataFields.CapHintText60);
        set => SetShortString(ClientDataFields.OffExpAddHintText, ClientDataFields.CapHintText60, value);
    }

    public string NGExpAddHintText
    {
        get => GetShortString(ClientDataFields.OffNGExpAddHintText, ClientDataFields.CapHintText60);
        set => SetShortString(ClientDataFields.OffNGExpAddHintText, ClientDataFields.CapHintText60, value);
    }

    /// <summary>元素新属性文字 [0..26]（<c>string[80]</c>，真机实测 27 项）。</summary>
    public int ElementTextCount => ClientDataFields.CountElementTexts;

    public string GetElementText(int i) =>
        GetShortString(ClientDataFields.OffElementNewPropertyTexts + i * (ClientDataFields.CapElementText + 1),
                       ClientDataFields.CapElementText);

    public void SetElementText(int i, string value) =>
        SetShortString(ClientDataFields.OffElementNewPropertyTexts + i * (ClientDataFields.CapElementText + 1),
                       ClientDataFields.CapElementText, value);

    /// <summary>人物栏属性分组标题 [0..6]（<c>string[40]</c>，<c>\</c> 表示换行）。</summary>
    public int HumGroupCaptionCount => ClientDataFields.CountHumGroupCaptions;

    public string GetHumGroupCaption(int i) =>
        GetShortString(ClientDataFields.OffHumPropertyGroupCaption + i * (ClientDataFields.CapAttackModeText + 1),
                       ClientDataFields.CapAttackModeText);

    public void SetHumGroupCaption(int i, string value) =>
        SetShortString(ClientDataFields.OffHumPropertyGroupCaption + i * (ClientDataFields.CapAttackModeText + 1),
                       ClientDataFields.CapAttackModeText, value);

    public string ItemHintFluteStoneText
    {
        get => GetShortString(ClientDataFields.OffItemHintFluteStoneText, ClientDataFields.CapElementText);
        set => SetShortString(ClientDataFields.OffItemHintFluteStoneText, ClientDataFields.CapElementText, value);
    }

    public string ItemHintNoFluteStoneText
    {
        get => GetShortString(ClientDataFields.OffItemHintNoFluteStoneText, ClientDataFields.CapElementText);
        set => SetShortString(ClientDataFields.OffItemHintNoFluteStoneText, ClientDataFields.CapElementText, value);
    }

    /// <summary>镶嵌宝石文字颜色（$-1 = clNone 时为 0x1FFFFFFF，真机实测 536870911）。</summary>
    public int ItemHintFluteStoneColor
    {
        get => GetInt32(ClientDataFields.OffItemHintFluteStoneColor);
        set => SetInt32(ClientDataFields.OffItemHintFluteStoneColor, value);
    }

    public int ItemHintNoFluteStoneColor
    {
        get => GetInt32(ClientDataFields.OffItemHintNoFluteStoneColor);
        set => SetInt32(ClientDataFields.OffItemHintNoFluteStoneColor, value);
    }

    /// <summary>发型偏移 [0..11]（顺序见 Delphi 声明：User/Other/Hero × X,Y × 两组）。</summary>
    public int GetHairOffset(int i)
    {
        if ((uint)i >= 12) throw new ArgumentOutOfRangeException(nameof(i));
        return GetInt32(ClientDataFields.OffHairOffsets + i * 4);
    }

    public void SetHairOffset(int i, int value)
    {
        if ((uint)i >= 12) throw new ArgumentOutOfRangeException(nameof(i));
        SetInt32(ClientDataFields.OffHairOffsets + i * 4, value);
    }

    // ---------------- payload 段落（只读视图，写入见 PayloadEditor）----------------

    /// <summary>取一个 payload 段落（按记录的 (Offset,Size) 对 + 可选 Crc 校验）。</summary>
    public byte[] GetSegment(int offsetField, int sizeField, int crcField = -1)
    {
        int off = GetInt32(offsetField);
        int size = GetInt32(sizeField);
        if (size <= 0) return Array.Empty<byte>();
        if (off < 0 || (long)off + size > Payload.Length)
            throw new InvalidDataException($"段落越界：off={off} size={size} payload={Payload.Length}");
        var seg = new byte[size];
        Buffer.BlockCopy(Payload, off, seg, 0, size);
        return seg;
    }

    /// <summary>校验 payload 段落是否正常（大小 &gt; 0 且落在 payload 内）。</summary>
    public bool HasSegment(int offsetField, int sizeField)
    {
        int off = GetInt32(offsetField), size = GetInt32(sizeField);
        return size > 0 && off >= 0 && (long)off + size <= Payload.Length;
    }

    /// <summary>把记录内容格式化成可读转储（调试用）。</summary>
    public string DumpKnownFields()
    {
        var sb = new StringBuilder();
        foreach (var f in ClientDataFields.Catalog)
        {
            sb.Append($"{f.Offset,6}  {f.Name,-30} {f.Kind}");
            switch (f.Kind)
            {
                case "str":
                    if (f.Count == 1) sb.Append($"  \"{GetShortString(f.Offset, f.Capacity)}\"");
                    else
                        for (int i = 0; i < f.Count; i++)
                            sb.Append($"\n              [{i}] \"{GetShortString(f.OffsetOf(i), f.Capacity)}\"");
                    break;
                case "bool": sb.Append($"  {GetBool(f.Offset)}"); break;
                case "u32": sb.Append($"  {GetUInt32(f.Offset)}"); break;
                case "i32": sb.Append($"  {GetInt32(f.Offset)}"); break;
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
