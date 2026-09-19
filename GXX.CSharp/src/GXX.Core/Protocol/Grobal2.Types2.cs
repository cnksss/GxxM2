using System;
using System.Runtime.InteropServices;

namespace GXX.Core.Protocol;

// ============================================================================
// Grobal2.pas 类型部分 2：物品体系（TStdItem / TUserItem / TClientItem 等）
// ============================================================================

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TStdItemEffect
{
    public short FileIndex;      // 物品发光效果 文件编号
    public ushort ImageStart;    // 读取位置
    public byte ImageCount;      // 读取张数
    public byte IsDrawCenter;    // 居中播放
    public byte IsDrawNoBlend;   // 非透明绘制
    public byte IsDrawBelow;     // 底层绘制
    public short OffsetX;        // 微调X
    public short OffsetY;        // 微调Y
    public ushort Time;          // 播放速度
}

/// <summary>TStdItem：物品基础定义（数据库 StdItems.DB 行的二进制布局）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TStdItem
{
    public fixed byte Name[Grobal2Const.ITEM_NAME_LEN + 1];    // string[60]
    public fixed byte DBName[Grobal2Const.ITEM_NAME_LEN + 1];  // string[60]
    public byte StdMode;
    public ushort Shape;
    public byte Weight;
    public ushort AniCount;
    public int Source;
    public byte Reserved;
    public byte NeedIdentify;
    public ushort Looks;
    public ushort DuraMax;
    public ushort Reserved1;
    public int HP;
    public int MP;
    public int AC1;
    public int AC2;
    public int MAC1;
    public int MAC2;
    public int DC1;
    public int DC2;
    public int MC1;
    public int MC2;
    public int SC1;
    public int SC2;
    public int Need;
    public int NeedLevel;
    public int Price;
    public ushort OverLap;      // 是否重叠物品
    public byte Color;          // 物品名称颜色
    public int Stock;
    public int Light;           // 数据库 Light 字段
    public int Horse;
    public int Expand1;
    public int Expand2;
    public int Expand3;
    public int Expand4;
    public int Expand5;
    public fixed ushort Elements[25];   // array[0..24] of Word
    public int InsuranceCurrency;
    public int InsuranceGold;
    public TStdItemEffect BagEffect;    // 包裹中物品发光效果
    public TStdItemEffect BodyEffect;   // 内观中物品发光效果
    public long Effect;                 // 指针改64位 2021-01-05

    public string NameStr { get { fixed (byte* p = Name) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = Name) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
    public string DBNameStr { get { fixed (byte* p = DBName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = DBName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }

    public ushort[] GetElements()
    {
        fixed (ushort* p = Elements) { var a = new ushort[25]; for (int i = 0; i < 25; i++) a[i] = p[i]; return a; }
    }
    public void SetElements(ushort[] src)
    {
        fixed (ushort* p = Elements)
        {
            for (int i = 0; i < 25; i++) p[i] = src != null && i < src.Length ? src[i] : (ushort)0;
        }
    }
}

/// <summary>TPartialStdItem：部分字段 StdItem。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TPartialStdItem
{
    public fixed byte Name[Grobal2Const.ITEM_NAME_LEN + 1];
    public ushort Looks;
    public byte StdMode;
    public ushort Shape;
    public byte _Horse;

    public string NameStr { get { fixed (byte* p = Name) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = Name) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TCompleteStdItem
{
    public int Index;
    public TStdItem StdItem;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TStdItemsHeader
{
    public int PartialFieldsCount; // 部分字段（整个记录全发）
    public int AllFiledsCount;     // 完整字段（只发部分记录）
}

/// <summary>自定义物品进度条。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserItemProgress
{
    public byte boOpen;       // Boolean
    public byte btNameColor;
    public byte btCount;
    public byte btShowType;   // 0:不显示 1:百分比 2:数值
    public ushort wMax;
    public ushort wValue;
    public ushort wLevel;
    public fixed byte sName[32]; // string[31]

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, 31); } set { fixed (byte* p = sName) ShortStr.Set(p, 31, value); } }
}

/// <summary>单个自定义属性。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TCustomProperty
{
    public byte btColor;
    public byte btBindType;
    public byte btShowFlag;
    public byte btPercent;   // 0:点数 1:单件% 2:全身%
    public byte btHintModule;
    public fixed int nValues[Grobal2Const.ITEM_PROP_VALUES_COUNT]; // [0..2]

    public int[] GetValues() { fixed (int* p = nValues) { var a = new int[3]; for (int i = 0; i < 3; i++) a[i] = p[i]; return a; } }
    public void SetValues(int[] v) { fixed (int* p = nValues) { for (int i = 0; i < 3; i++) p[i] = v != null && i < v.Length ? v[i] : 0; } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserItemProperty
{
    public fixed byte sText[129];     // string[128]
    public byte btTextColor;
    public TCustomProperty Properties_0;   // array[0..19] 展开（见辅助方法）
    public TCustomProperty Properties_1;
    public TCustomProperty Properties_2;
    public TCustomProperty Properties_3;
    public TCustomProperty Properties_4;
    public TCustomProperty Properties_5;
    public TCustomProperty Properties_6;
    public TCustomProperty Properties_7;
    public TCustomProperty Properties_8;
    public TCustomProperty Properties_9;
    public TCustomProperty Properties_10;
    public TCustomProperty Properties_11;
    public TCustomProperty Properties_12;
    public TCustomProperty Properties_13;
    public TCustomProperty Properties_14;
    public TCustomProperty Properties_15;
    public TCustomProperty Properties_16;
    public TCustomProperty Properties_17;
    public TCustomProperty Properties_18;
    public TCustomProperty Properties_19;

    public string TextStr { get { fixed (byte* p = sText) return ShortStr.Get(p, 128); } set { fixed (byte* p = sText) ShortStr.Set(p, 128, value); } }

    public TCustomProperty GetProp(int i)
    {
        fixed (void* basePtr = &Properties_0)
        {
            var p = (TCustomProperty*)basePtr;
            return p[i];
        }
    }
    public void SetProp(int i, TCustomProperty v)
    {
        fixed (void* basePtr = &Properties_0)
        {
            var p = (TCustomProperty*)basePtr;
            p[i] = v;
        }
    }
}

/// <summary>TUserItemFrom：物品来源。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserItemFrom
{
    public TItemFormType ItemForm;
    public fixed byte sMapName[Grobal2Const.MAP_NAME_LEN + 1];  // string[30]
    public fixed byte sMonName[41];                             // string[40]
    public fixed byte sMakerName[Grobal2Const.ACTOR_NAME_LEN + 1]; // string[14]
    public double DateTime;                                     // TDateTime (8字节)

    public string MapName { get { fixed (byte* p = sMapName) return ShortStr.Get(p, Grobal2Const.MAP_NAME_LEN); } set { fixed (byte* p = sMapName) ShortStr.Set(p, Grobal2Const.MAP_NAME_LEN, value); } }
    public string MonName { get { fixed (byte* p = sMonName) return ShortStr.Get(p, 40); } set { fixed (byte* p = sMonName) ShortStr.Set(p, 40, value); } }
    public string MakerName { get { fixed (byte* p = sMakerName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sMakerName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TFluteInfo
{
    public ushort GemIndex;
    public ushort GemCount;
}

/// <summary>TClientItem：发送到客户端的物品（large wire 结构）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientItem
{
    public TStdItem s;
    public int MakeIndex;
    public ushort Dura;
    public ushort DuraMax;
    public byte IsBind;           // Boolean
    public byte btFluteCount;
    public byte btUpgradeCount;   // 升级次数
    public byte btHeroM2Light;    // HeroM2 SetItemsLight
    public fixed int btValue[14];        // 附加属性
    public fixed ushort NewValue[30];
    public TFluteInfo Flutes_0; public TFluteInfo Flutes_1; public TFluteInfo Flutes_2; public TFluteInfo Flutes_3;
    public TFluteInfo Flutes_4; public TFluteInfo Flutes_5; public TFluteInfo Flutes_6; public TFluteInfo Flutes_7;
    public TUserItemProgress Progress0;
    public TUserItemProgress Progress1;
    public TUserItemProperty CustomProperty;
    public TUserItemFrom ItemFrom;
    public ushort wInsuranceCount;

    public int GetBtValue(int i) { fixed (int* p = btValue) return p[i]; }
    public void SetBtValue(int i, int v) { fixed (int* p = btValue) p[i] = v; }
    public ushort GetNewValue(int i) { fixed (ushort* p = NewValue) return p[i]; }
    public void SetNewValue(int i, ushort v) { fixed (ushort* p = NewValue) p[i] = v; }
    public TFluteInfo GetFlute(int i)
    {
        fixed (void* bp = &Flutes_0) { var p = (TFluteInfo*)bp; return p[i]; }
    }
    public void SetFlute(int i, TFluteInfo v) { fixed (void* bp = &Flutes_0) { var p = (TFluteInfo*)bp; p[i] = v; } }
}

/// <summary>TMagic_C：客户端魔法定义。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMagic_C
{
    public TMagicAttr MagicAttr;
    public ushort wMagicId;
    public fixed byte sMagicName[Grobal2Const.ITEM_NAME_LEN + 1];
    public byte btEffectType;
    public byte btEffect;
    public ushort wSpell;
    public fixed int MaxTrain[16];
    public byte btTrainLv;
    public uint dwMagicDelayTime;
    public ushort wDefSpell;
    public int CanUpgrade;        // 是否允许升级
    public int MaxUpgradeLevel;

    public string MagicName { get { fixed (byte* p = sMagicName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = sMagicName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

/// <summary>TClientMagic：客户端魔法（84 字节注释）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientMagic
{
    public byte Key;         // AnsiChar
    public byte Level;
    public byte NewLevel;    // 九重
    public int CurTrain;
    public TMagic_C Def;
    public uint dwInterval;
    public uint dwRealInterval;
    public uint dwLastUseTick;
}

/// <summary>THumMagic：人物魔法记录（非 packed，Delphi 自然对齐：Word+3*Byte+pad+Int+Byte+pad）。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct THumMagic
{
    public TMagicAttr MagicAttr;  // enum:byte
    public ushort wMagIdx;
    public byte btLevel;
    public byte btNewLevel;
    public byte btKey;
    public int nTranPoint;        // 当前持久值（对齐后偏移 8）
    public byte boUsesItemAdd;    // 是否装备触发
}
