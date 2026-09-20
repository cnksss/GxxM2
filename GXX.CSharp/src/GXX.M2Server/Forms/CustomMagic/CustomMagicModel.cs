// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = 该窗体依赖的**数据模型最小面**：
//   * 窗体自身在 implementation 段声明的 node-data 记录
//     （uFrmCustomMagic.pas:858-885）—— 原文如此，属本单元。
//   * uCustomMagicUtils.pas 的记录/类（:83-238）—— 该单元尚未移植（只移植了
//     枚举与名称表，见 Engine\CustomMagicUtils.cs 批次J61），这里按其原文**逐字**
//     落一份最小面。**接缝：待 uCustomMagicUtils.pas 数据层移植后由其正式接管，
//     届时删除本文件的重复声明**（跨车道重名规程：台账 §12.8 / §14）。
//     注意 GXX.M2Server.Engine.MagicBatchH.cs:62 另有一个**子集**同名
//     TCustomMagicConfig（服务端运行时用），与本文件的正式面**不同**；
//     本文件全部类型在 GXX.M2Server.Forms.CustomMagic 命名空间内，不与其 CS0101。
//
// 覆盖行号（Delphi）：uFrmCustomMagic.pas 858-885；
//                     uCustomMagicUtils.pas 83-238、753-756
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms.CustomMagic;

/// <summary>
/// Delphi 短字符串（<c>string[N]</c>）赋值语义：超过 N 个字符即截断。
/// 对应 uCustomMagicUtils.pas:147-149/153/155/186 的 string[40]/string[20]/string[ITEM_NAME_LEN]。
/// </summary>
public static class CustomMagicShortStr
{
    /// <summary>Delphi <c>string[N]</c> 赋值：超过 N 个字符截断（不抛异常）。</summary>
    public static string Trunc(string? value, int maxLen)
    {
        value ??= "";
        return value.Length <= maxLen ? value : value[..maxLen];
    }
}

// ---------------------------------------------------------------------------
// uCustomMagicUtils.pas:83-135 —— 记录
// 原文是 record（值类型）；窗体通过 PMagicChangeAttributesRecord 指针在树上共享一份，
// 故托管侧一律建模为 class（引用语义 == 原文的指针相等），并在报告登记该形式偏差。
// ---------------------------------------------------------------------------

/// <summary>uCustomMagicUtils.pas:83-90 TMagicAdditionalDamage 1:1（附加伤害）。</summary>
public sealed class TMagicAdditionalDamage
{
    public bool Checked;
    public byte Rate;
    public byte Rate2;
    public byte Time;
    public byte TimeUnit;
    public byte Time2;
}

/// <summary>uCustomMagicUtils.pas:96-111 TMagicChangeAttributesRecord 1:1（增减属性）。</summary>
public sealed class TMagicChangeAttributesRecord
{
    public bool IsChecked;              // 启用
    public int Rate;                    // 几率
    public int RateAdd;                 // 几率递增
    public int LowValue;                // 下限值
    public bool LowValueIsPoint;        // 下限单位 False: %; True: 点数
    public int LowValueAdd;             // 下限递增
    public int HighValue;               // 上限值
    public bool HighValueIsPoint;       // 上限单位 False: %; True: 点数
    public int HighValueAdd;            // 上限限递增
    public int Time;                    // 时间
    public int TimeAdd;                 // 时间递增
    public bool TimeAddIsPoint;         // 递增单位 False: %; True: 分钟
    public bool ShowHint;
    public string HintText = "";        // 原文如此（uCustomMagicUtils.pas:110）string，非 short string
}

/// <summary>uCustomMagicUtils.pas:115-127 TMagicAttackChangeElementRecord 1:1（元素）。</summary>
public sealed class TMagicAttackChangeElementRecord
{
    public bool IsChecked;              // 启用
    public int Rate;                    // 几率
    public int RateAdd;                 // 几率递增
    public int Value;                   // 修改值
    public bool ValueIsPoint;           // 修改值单位 False: %; True: 点数
    public int ValueAdd;                // 值递增
    public int Time;                    // 时间
    public int TimeAdd;                 // 时间递增
    public bool TimeAddIsPoint;         // 递增单位 False: %; True: 分钟
    public bool ShowHint;
    public string HintText = "";
}

/// <summary>uCustomMagicUtils.pas:129-135 TBreakDefenseInfo 1:1（破防）。</summary>
public sealed class TBreakDefenseInfo
{
    public bool IsChecked;
    public int Rate;
    public int RateAdd;
    public int Value;
    public int ValueAdd;
}

/// <summary>
/// uCustomMagicUtils.pas:139-213 TMagicServerConfig 1:1（服务端配置）。
/// 原文是 record（值类型）；窗体以 <c>@ServerConfig</c> 指针长期持有（:1939），
/// 故托管侧建模为 class 以保住"引用同一份"的语义。
/// </summary>
public sealed class TMagicServerConfig
{
    public TCustomOperateMode OperateMode;                  // 操作模式 chongchong 2014-09-06
    public bool IsAttackUseNG;                              // 使用内功值释放
    public bool NoChangeDir;
    public bool DisableInSafeZone;                          // 禁止安全区使用
    public int AttackDelayTime;                             // 伤害延时时间
    public int UseInterval;                                 // 使用间隔
    // FailNoShowEff: Boolean;                              // 时间未到或用失败时不显示魔法
    private string _failMsg = "";                           // FailMsg: string[40]
    private string _succeedMsg = "";                        // SucceedMsg: string[40]
    private string _closeMsg = "";                          // CloseMsg: string[40]
    public bool IsCheckVarValue;                            // 检查变量
    public TMagicNeedItem NeedItem;                         // 所需佩戴物品
    public int NeedItemCount;                               // 所需物品数量
    private string _needItemCustomItemName = "";            // NeedItemCustomItemName: string[ITEM_NAME_LEN]
    public bool NeedItemUseBagItem;
    private string _checkVarName = "";                      // CheckVarName: string[20]
    public TCheckVarType CheckVarType;
    public int CheckVarValue;
    public int CheckVarAdd;
    public TCustomAttackMode AttackMode;                    // 攻击方式
    public TCustomAttackTarget AttackTarget;                // 攻击目标
    public TCustomAttackPowerCalc AttackPowerCalc;          // 威力计算
    public readonly int[] AttackPowerRates = new int[CustomMagicUtilsConst.MaxCustomMagicLevel + 2]; // array[0..MaxCustomMagicLevel+1] 威力倍数
    public int AttackPowerLineAdd;                          // 线性攻击威力递增
    public int AttackPowerUndeadAdd;                        // 不死系怪物伤害加成
    public bool AttackTeleportAttack;                       // 瞬移攻击
    public bool IsNoTeleportNoAttack;
    public int AttackTeleportRate;                          // 瞬移几率
    public bool AttackTeleportRunHum;
    public bool AttackTeleportRunMon;
    public bool AttackTeleportRunNpc;
    public bool AttackTeleportRunGuard;
    public bool AttackTeleportRunObstacle;
    public bool AttackTeleportWarDisHumRun;
    public bool AttackTeleportCannotRunItem;
    public bool AttackTeleportRush;                         // 增加突进型瞬移 By 一支笔 at:2021-07-06 14:01:29
    public int AttackTeleportRushCount;
    public bool AttackTeleportAfterDamage;                  // 造成伤害后瞬移 By 一支笔 at:2021-07-06 14:02:16
    public int AttackNearRange;                             // 近攻范围
    public int AttackGroupRange;                            // 群攻范围
    public int AttackLineWidth;                             // 直线攻击宽度
    public bool EnableAntiMagic;
    public bool EnableHitPoint;
    public bool EnabledCallMonster;                         // 允许召唤怪物
    public int CallMonstersRate;                            // 怪物召唤几率
    public int CallMonstersRoyaltySec;                      // 召唤宝宝叛变时间
    private readonly string[] _callMonsters = { "", "" };   // CallMonsters: array[0..1] of string[ITEM_NAME_LEN] 召唤怪物1-4
    public readonly int[] CallMonsterNums = new int[2];     // CallMonsterNums: array[0..1] 怪物1-4数量
    public int CallMonstersLevel;
    public readonly TMagicAdditionalDamage[] Additionals = new TMagicAdditionalDamage[11]; // array[0..10]
    public int AdditionalHP0;                               // 绿毒掉血
    public bool AdditionalHighLevel4;                       // 可推动高等级
    public byte AdditionalPushedType4;                      // 推动类型
    public bool AttackTargetStatus;
    public byte AttackTargetStatusTime;
    public byte AttackTargetStatusTimeUnit;
    public byte AttackTargetStatusTime2;
    public ushort AttackTargetStatusDelay;
    public readonly TMagicChangeAttributesRecord[] AttackSubAttrib =
        new TMagicChangeAttributesRecord[CustomMagicUtilsConst.MagicAttackDecAttributesTypeCount]; // array[TMagicAttackDecAttributesType]
    public readonly TMagicAttackChangeElementRecord[] AttackSubElements =
        new TMagicAttackChangeElementRecord[CustomMagicUtilsConst.ItemElementsTypeCount];          // array[TItemElementsType]
    public readonly TBreakDefenseInfo[] AttackBreakDefense =
        new TBreakDefenseInfo[CustomMagicUtilsConst.BreakDefenseTypeCount];                        // array[TBreakDefenseType]
    public readonly TMagicChangeAttributesRecord[] ProtectAddAttrib =
        new TMagicChangeAttributesRecord[CustomMagicUtilsConst.MagicProtectAddAttributesTypeCount]; // array[TMagicProtectAddAttributesType]
    public readonly TMagicAttackChangeElementRecord[] ProtectAddElements =
        new TMagicAttackChangeElementRecord[CustomMagicUtilsConst.ItemElementsTypeCount];          // array[TItemElementsType]
    // 保护模式
    public bool ProtectAddHPSlow;                           // 多次回血
    public ushort ProtectAddHpSlowCount;                    // 回血次数
    public bool ProtectTargetStatus;
    public byte ProtectTargetStatusTime;
    public byte ProtectTargetStatusTimeUnit;
    public byte ProtectTargetStatusTime2;
    public ushort ProtectTargetStatusDelay;
    public int ProtectTargetRange;                          // 保护目标范围
    // ProtectSelfRate: Integer;                            // 自我保护几率

    /// <summary>FailMsg: string[40]（原文 :147）。</summary>
    public string FailMsg { get => _failMsg; set => _failMsg = CustomMagicShortStr.Trunc(value, 40); }

    /// <summary>SucceedMsg: string[40]（原文 :148）。</summary>
    public string SucceedMsg { get => _succeedMsg; set => _succeedMsg = CustomMagicShortStr.Trunc(value, 40); }

    /// <summary>CloseMsg: string[40]（原文 :149）。</summary>
    public string CloseMsg { get => _closeMsg; set => _closeMsg = CustomMagicShortStr.Trunc(value, 40); }

    /// <summary>NeedItemCustomItemName: string[ITEM_NAME_LEN]（原文 :153）。</summary>
    public string NeedItemCustomItemName
    {
        get => _needItemCustomItemName;
        set => _needItemCustomItemName = CustomMagicShortStr.Trunc(value, Grobal2Const.ITEM_NAME_LEN);
    }

    /// <summary>CheckVarName: string[20]（原文 :155）。</summary>
    public string CheckVarName { get => _checkVarName; set => _checkVarName = CustomMagicShortStr.Trunc(value, 20); }

    /// <summary>CallMonsters[0..1]: string[ITEM_NAME_LEN]（原文 :186）。</summary>
    public string GetCallMonster(int index) => _callMonsters[index];

    /// <summary>CallMonsters[0..1] 写回（Delphi 短字符串截断）。</summary>
    public void SetCallMonster(int index, string value)
        => _callMonsters[index] = CustomMagicShortStr.Trunc(value, Grobal2Const.ITEM_NAME_LEN);

    /// <summary>Low(CallMonsters) = 0，High(CallMonsters) = 1（原文 :187）。</summary>
    public const int CallMonstersLow = 0;
    /// <summary>High(CallMonsters)。</summary>
    public const int CallMonstersHigh = 1;

    public TMagicServerConfig()
    {
        for (int i = 0; i < Additionals.Length; i++)
            Additionals[i] = new TMagicAdditionalDamage();
        for (int i = 0; i < AttackSubAttrib.Length; i++)
            AttackSubAttrib[i] = new TMagicChangeAttributesRecord();
        for (int i = 0; i < AttackSubElements.Length; i++)
            AttackSubElements[i] = new TMagicAttackChangeElementRecord();
        for (int i = 0; i < AttackBreakDefense.Length; i++)
            AttackBreakDefense[i] = new TBreakDefenseInfo();
        for (int i = 0; i < ProtectAddAttrib.Length; i++)
            ProtectAddAttrib[i] = new TMagicChangeAttributesRecord();
        for (int i = 0; i < ProtectAddElements.Length; i++)
            ProtectAddElements[i] = new TMagicAttackChangeElementRecord();
    }
}

/// <summary>
/// uCustomMagicUtils.pas:224 的 <c>TMagicClientConfigs = array[TMagicPlusLevel] of TMagicClientConfig</c>
/// 的元素持有者。Delphi 窗体以 <c>@ClientConfigs[level]</c> 取得**稳定指针**（:2394）并长时间持有
/// （FCurrentClientConfig），故托管侧用 class 包一层还原指针语义。
/// </summary>
public sealed class TMagicClientConfigHolder
{
    /// <summary>被持有的 TMagicClientConfig（packed 布局照抄 GXX.Core.Protocol）。</summary>
    public TMagicClientConfig Value;
}

/// <summary>
/// uCustomMagicUtils.pas:215-238 TCustomMagicConfig 1:1（自定义技能配置对象）。
/// <para>
/// 接缝：原文 :349-751 的构造默认值（约 400 行）与 :758+ 的 LoadFromIniFile/SaveToIniFile
/// 属 uCustomMagicUtils.pas，**不在本单元**，本类只保留窗体用到的面：
/// <see cref="SetChanged"/>(:753-756 1:1)、IsChanged、MagicName/MagicID/IsMagicWarr 与三份配置。
/// 默认值由 <see cref="CustomMagicConfigDefaults.Apply"/> 接缝注入（默认空实现）。
/// </para>
/// </summary>
public sealed class TCustomMagicConfig
{
    private string _magicName = "";
    private bool _isMagicWarr;
    private bool _isChanged;

    /// <summary>ClientBaseConfig: TMagicClientBaseConfig（原文 :223）。</summary>
    public TMagicClientBaseConfig ClientBaseConfig;

    /// <summary>ClientConfigs: TMagicClientConfigs = array[TMagicPlusLevel]（原文 :224）。</summary>
    public readonly TMagicClientConfigHolder[] ClientConfigs =
    {
        new TMagicClientConfigHolder(), new TMagicClientConfigHolder(),
        new TMagicClientConfigHolder(), new TMagicClientConfigHolder(),
    };

    /// <summary>ServerConfig: TMagicServerConfig（原文 :225）。</summary>
    public TMagicServerConfig ServerConfig = new();

    /// <summary>原文 :227 constructor Create(AMagicName, AMagicID, AIsMagicWarr)。</summary>
    public TCustomMagicConfig(string aMagicName, ushort aMagicID, bool aIsMagicWarr)
    {
        _magicName = aMagicName;
        _magicId = aMagicID;
        _isMagicWarr = aIsMagicWarr;
        // 接缝：待 uCustomMagicUtils.pas 数据层（:349-751 默认值）移植后由其正式接管
        CustomMagicConfigDefaults.Apply?.Invoke(this);
    }

    private ushort _magicId;

    /// <summary>property IsMagicWarr（原文 :229，可读可写）。</summary>
    public bool IsMagicWarr { get => _isMagicWarr; set => _isMagicWarr = value; }

    /// <summary>procedure SetChanged(Value: Boolean = True)（原文 :233、:753-756）。</summary>
    public void SetChanged(bool value = true) => _isChanged = value;

    /// <summary>property MagicName（原文 :235）。</summary>
    public string MagicName { get => _magicName; set => _magicName = value; }

    /// <summary>property MagicID: Word read FMagicID（原文 :236，只读）。</summary>
    public ushort MagicID => _magicId;

    /// <summary>property IsChanged: Boolean read FIsChanged（原文 :237，只读）。</summary>
    public bool IsChanged => _isChanged;
}

/// <summary>
/// uFrmCustomMagic.pas:9-13 的 WM_USER+300..303 起始编辑消息号（原文常量 1:1）。
/// </summary>
public static class CustomMagicWm
{
    /// <summary>WM_USER 基址（Delphi Windows 单元）。</summary>
    public const int WM_USER = 0x0400;

    /// <summary>WM_STARTEDITING_DEC_ATTRIB（原文 :10）。</summary>
    public const int WM_STARTEDITING_DEC_ATTRIB = WM_USER + 300;
    /// <summary>WM_STARTEDITING_DEC_ELEMENT（原文 :11）。</summary>
    public const int WM_STARTEDITING_DEC_ELEMENT = WM_USER + 301;
    /// <summary>WM_STARTEDITING_INC_ATTRIB（原文 :12）。</summary>
    public const int WM_STARTEDITING_INC_ATTRIB = WM_USER + 302;
    /// <summary>WM_STARTEDITING_INC_ELEMENT（原文 :13）。</summary>
    public const int WM_STARTEDITING_INC_ELEMENT = WM_USER + 303;
}

// ---------------------------------------------------------------------------
// uFrmCustomMagic.pas:858-885 —— 本单元自己声明的 node-data 记录（原文如此）
// ---------------------------------------------------------------------------

/// <summary>uFrmCustomMagic.pas:859-863 PMagicConfigNodeData / TMagicConfigNodeData 1:1。</summary>
public sealed class TMagicConfigNodeData
{
    /// <summary>Config: TCustomMagicConfig。</summary>
    public TCustomMagicConfig? Config;
}

/// <summary>uFrmCustomMagic.pas:866-871 PAttackDecAttribData / TAttackDecAttribData 1:1。</summary>
public sealed class TAttackDecAttribData
{
    /// <summary>AttribType: TMagicAttackDecAttributesType。</summary>
    public GXX.M2Server.Engine.TMagicAttackDecAttributesType AttribType;
    /// <summary>Data: PMagicChangeAttributesRecord（指向 ServerConfig.AttackSubAttrib[i] 同一对象）。</summary>
    public TMagicChangeAttributesRecord? Data;
}

/// <summary>uFrmCustomMagic.pas:873-878 PProtectAddAttribData / TProtectAddAttribData 1:1。</summary>
public sealed class TProtectAddAttribData
{
    /// <summary>AttribType: TMagicProtectAddAttributesType。</summary>
    public GXX.M2Server.Engine.TMagicProtectAddAttributesType AttribType;
    /// <summary>Data: PMagicChangeAttributesRecord（指向 ServerConfig.ProtectAddAttrib[i] 同一对象）。</summary>
    public TMagicChangeAttributesRecord? Data;
}

/// <summary>uFrmCustomMagic.pas:880-885 PMagicElementData / TMagicElementData 1:1。</summary>
public sealed class TMagicElementData
{
    /// <summary>ElementType: TItemElementsType。</summary>
    public GXX.M2Server.Engine.TItemElementsType ElementType;
    /// <summary>Data: PMagicAttackChangeElementRecord（指向 AttackSubElements / ProtectAddElements[i]）。</summary>
    public TMagicAttackChangeElementRecord? Data;
}

/// <summary>
/// TMagicServerConfig 定长数组的 Delphi 上界常量。
/// 原文用 Low/High(枚举) 与 MaxCustomMagicLevel；这里把枚举元素个数落成常量，
/// 数值与 Engine\CustomMagicUtils.cs 的名称表长度逐一核对（测试锁定）。
/// </summary>
public static class CustomMagicUtilsConst
{
    /// <summary>MaxCustomMagicLevel（uCustomMagicUtils.pas，值 9）。</summary>
    public const int MaxCustomMagicLevel = 9;

    /// <summary>High(TMagicAttackDecAttributesType)+1 = 9。</summary>
    public const int MagicAttackDecAttributesTypeCount = 9;
    /// <summary>High(TMagicProtectAddAttributesType)+1 = 16。</summary>
    public const int MagicProtectAddAttributesTypeCount = 16;
    /// <summary>High(TItemElementsType)+1 = 25。</summary>
    public const int ItemElementsTypeCount = 25;
    /// <summary>High(TBreakDefenseType)+1 = 6。</summary>
    public const int BreakDefenseTypeCount = 6;
    /// <summary>High(TMagicPlusLevel)+1 = 4。</summary>
    public const int MagicPlusLevelCount = 4;
}

/// <summary>
/// 接缝：uCustomMagicUtils.pas TCustomMagicConfig.Create（:349-751）的默认值填充。
/// 默认 null（新对象即全零，与 Delphi 托管 record 字段的初值一致）；
/// 待 uCustomMagicUtils.pas 数据层移植后由它挂上正式实现。
/// </summary>
public static class CustomMagicConfigDefaults
{
    /// <summary>默认值注入点。</summary>
    public static Action<TCustomMagicConfig>? Apply;

    /// <summary>
    /// 接缝：uCustomMagicUtils.pas TCustomMagicConfig.SaveToIniFile（原文 :758+ 邻域）。
    /// 默认 null（不落盘）；btnSaveClick（:3601-3627）只对 <c>IsChanged</c> 的节点调用它。
    /// </summary>
    public static Action<TCustomMagicConfig>? SaveToIniFile;
}
