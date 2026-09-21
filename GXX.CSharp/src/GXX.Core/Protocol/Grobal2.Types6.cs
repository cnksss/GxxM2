using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GXX.Core.Protocol;

// ============================================================================
// Grobal2.pas 类型部分 6：TUserItem（用户物品核心记录）及补充类型
// ============================================================================

/// <summary>TUserItem：M2 侧用户物品（M2↔DB 存档 wire 结构，packed）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserItem
{
    public int MakeIndex;
    public ushort wIndex;         // 物品id
    public fixed byte Name[Grobal2Const.ITEM_NAME_LEN + 1];  // string[60]
    public ushort Dura;           // 当前持久值
    public ushort DuraMax;        // 最大持久值
    public fixed int btValue[14];
    public uint dwHeroM2DressEffect;  // MakeLong(btHeroM2DressEffect, boHeroM2DressNoBlend)
    public byte btUpgradeCount;
    public byte boStartTime;
    public int nLimitTime;        // 限时物品（分钟）
    public byte btHeroM2Light;
    public fixed ushort btNewValue[30];
    public byte btColor;
    public byte boIsBind;
    public byte btBindOption;     // 绑定选项 Bit（TUserItemBindValueType）
    public ushort wEffect;        // 特效编号（新）
    public byte btFluteCount;     // 凹槽数量
    public TFluteInfo Flutes_0; public TFluteInfo Flutes_1; public TFluteInfo Flutes_2; public TFluteInfo Flutes_3;
    public TFluteInfo Flutes_4; public TFluteInfo Flutes_5; public TFluteInfo Flutes_6; public TFluteInfo Flutes_7;
    public TUserItemProgress Progress0;
    public TUserItemProgress Progress1;
    public TUserItemProperty CustomProperty;
    public TUserItemFrom ItemFrom;
    public ushort wInsuranceCount;
    public ushort wNewLooks;
    public ushort wNewShape;
    public ushort wNewExpand3;
    public ushort wNewExpand4;
    public fixed byte btAddDataByte[Grobal2Const.USER_ITEM_ADD_DATA_BYTE_COUNT];
    public fixed int nAddDataInt[Grobal2Const.USER_ITEM_ADD_DATA_INT_COUNT];
    public ShortStr21 sAddDataText_0;   // array[0..1] of string[20]
    public ShortStr21 sAddDataText_1;

    public string NameStr { get { fixed (byte* p = Name) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = Name) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }

    public int GetBtValue(int i) { fixed (int* p = btValue) return p[i]; }
    public void SetBtValue(int i, int v) { fixed (int* p = btValue) p[i] = v; }
    public ushort GetNewValue(int i) { fixed (ushort* p = btNewValue) return p[i]; }
    public void SetNewValue(int i, ushort v) { fixed (ushort* p = btNewValue) p[i] = v; }
    public TFluteInfo GetFlute(int i) { fixed (void* bp = &Flutes_0) { var p = (TFluteInfo*)bp; return p[i]; } }
    public void SetFlute(int i, TFluteInfo v) { fixed (void* bp = &Flutes_0) { var p = (TFluteInfo*)bp; p[i] = v; } }

    public string GetAddDataText(int i) => i == 0 ? sAddDataText_0.Value : sAddDataText_1.Value;
    public void SetAddDataText(int i, string v) { if (i == 0) sAddDataText_0.Value = v; else sAddDataText_1.Value = v; }
}

// ---- Delphi TObject 引用型记录（非 wire，托管 class）----

/// <summary>TFoundryItem（含 TList 引用）。</summary>
public class TFoundryItem
{
    public string sItemName = "";
    public int nItemCount;
    public int nItemRate;
    public System.Collections.Generic.List<object> ItemList = new();
}

public struct TFoundryNeedItem
{
    public string sItemName;
    public int nItemCount;
    public byte btDelete;
}

/// <summary>TUnbindItemInfo（record，非 packed，含引用 string）。</summary>
public class TUnbindItemInfo
{
    public string sItemName = "";
    public int nShape;
    public int nCount;
}

/// <summary>TSendUserData。</summary>
public class TSendUserData
{
    public int nSocketIndx;
    public int nSocketHandle;
    public string sMsg = "";
}

/// <summary>TChrMsg（客户端消息队列元素）。</summary>
public class TChrMsg
{
    public int Ident;
    public int x;
    public int y;
    public int dir;
    public long State;
    public TFeature Feature;
    public string saying = "";
    public int sound;
}

/// <summary>TRegInfo。</summary>
public class TRegInfo
{
    public string sKey = "";
    public string sServerName = "";
    public string sRegSrvIP = "";
    public int nRegPort;
}

/// <summary>TUserCharacterInfo（record 带 string[N]）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserCharacterInfo
{
    public fixed byte Name[31];   // string[30]
    public byte Job;
    public byte HAIR;
    public uint Level;
    public byte sex;

    public string NameStr { get { fixed (byte* p = Name) return ShortStr.Get(p, 30); } set { fixed (byte* p = Name) ShortStr.Set(p, 30, value); } }

    /// <summary>
    /// ★ 集成方补（台账 §63.4 裁定 CR-1）：**原文客户端侧该字段名是 `sChrName`**
    /// （`Client-HGE/ClMain.pas:23199/23214/23234` 读写 `g_DeleteHumanInfoArray[I].sChrName`、
    /// `MShare.pas` 的 `g_SelDeleteHumanInfo.sChrName`；声明见 `Common/Grobal2.pas`
    /// 的 `sChrName: string[ACTOR_NAME_LEN]`）。本托管类型同时服务服务端与客户端两侧，
    /// 上面已有 `NameStr` 访问器 —— 这里补一个**同缓冲别名**（读/写都落到同一 30 字节短串），
    /// 好让 GUI 侧把 `FStateMShareSeam.g_SelDeleteHumanInfo_sChrName` 这类"单字段接缝"退役、改指真身。
    /// </summary>
    /// <remarks>与 `NameStr` 是**同一块存储** —— 这也是**不能**新加一个 `fixed byte[31]` 字段的原因：
    /// 那会变成两份状态（台账 §58.2 的 `m_wAbil`/`m_WAbil` 就是同一个坑）。</remarks>
    public string sChrName { get => NameStr; set => NameStr = value; }
}

/// <summary>TDummyLogon。</summary>
public class TDummyLogon
{
    public string sCharName = "";
    public string sMapName = "";
    public int nX;
    public int nY;
}

/// <summary>TDBSaveHuman。</summary>
public class TDBSaveHuman
{
    public int nSessionID;
    public long PlayObject;
    public THumData Data;
}

/// <summary>TDBSaveHero。</summary>
public class TDBSaveHero
{
    public int nSessionID;
    public long PlayObject;
    public THeroData Data;
}

/// <summary>TDBResult（含指针缓冲，托管 class）。</summary>
public class TDBResult
{
    public TDBResultType ResultType;
    public byte[] LoadBuf;
    public int LoadBufLen;
    public byte[] DataBuf;
    public int DataBufLen;
    public int nResult;
    public string sResult = "";
}

public class TDBLoadHumanResult
{
    public TDBLoadHuman LoadUser;
    public THumData Data;
    public int nResult;
}

/// <summary>TAccountInfo2。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAccountInfo2
{
    public long PlayObject;
    public long Npc;
    public fixed byte AccountName[15];   // string[14]
    public fixed byte Password[11];      // string[10]
    public fixed byte UserName[21];      // string[20]
    public fixed byte BirthDay[11];      // string[10]
    public fixed byte Questions1[21];    // string[20]
    public fixed byte Answers1[13];      // string[12]
    public fixed byte Questions2[21];    // string[20]
    public fixed byte Answers2[13];      // string[12]
    public fixed byte MobilePhone[14];   // string[13]
    public fixed byte Mail[41];          // string[40]
    public fixed byte L2Password[21];    // string[20]

    public string AccountNameStr { get { fixed (byte* p = AccountName) return ShortStr.Get(p, 14); } set { fixed (byte* p = AccountName) ShortStr.Set(p, 14, value); } }
    public string PasswordStr { get { fixed (byte* p = Password) return ShortStr.Get(p, 10); } set { fixed (byte* p = Password) ShortStr.Set(p, 10, value); } }
    public string UserNameStr { get { fixed (byte* p = UserName) return ShortStr.Get(p, 20); } set { fixed (byte* p = UserName) ShortStr.Set(p, 20, value); } }
    public string BirthDayStr { get { fixed (byte* p = BirthDay) return ShortStr.Get(p, 10); } set { fixed (byte* p = BirthDay) ShortStr.Set(p, 10, value); } }
    public string Questions1Str { get { fixed (byte* p = Questions1) return ShortStr.Get(p, 20); } set { fixed (byte* p = Questions1) ShortStr.Set(p, 20, value); } }
    public string Answers1Str { get { fixed (byte* p = Answers1) return ShortStr.Get(p, 12); } set { fixed (byte* p = Answers1) ShortStr.Set(p, 12, value); } }
    public string Questions2Str { get { fixed (byte* p = Questions2) return ShortStr.Get(p, 20); } set { fixed (byte* p = Questions2) ShortStr.Set(p, 20, value); } }
    public string Answers2Str { get { fixed (byte* p = Answers2) return ShortStr.Get(p, 12); } set { fixed (byte* p = Answers2) ShortStr.Set(p, 12, value); } }
    public string MobilePhoneStr { get { fixed (byte* p = MobilePhone) return ShortStr.Get(p, 13); } set { fixed (byte* p = MobilePhone) ShortStr.Set(p, 13, value); } }
    public string MailStr { get { fixed (byte* p = Mail) return ShortStr.Get(p, 40); } set { fixed (byte* p = Mail) ShortStr.Set(p, 40, value); } }
    public string L2PasswordStr { get { fixed (byte* p = L2Password) return ShortStr.Get(p, 20); } set { fixed (byte* p = L2Password) ShortStr.Set(p, 20, value); } }
}

/// <summary>TClientCmd（string[25] x2）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientCmd
{
    public fixed byte sCmd[26];
    public fixed byte sCaption[26];

    public string Cmd { get { fixed (byte* p = sCmd) return ShortStr.Get(p, 25); } set { fixed (byte* p = sCmd) ShortStr.Set(p, 25, value); } }
    public string Caption { get { fixed (byte* p = sCaption) return ShortStr.Get(p, 25); } set { fixed (byte* p = sCaption) ShortStr.Set(p, 25, value); } }
}

/// <summary>TMonHPProgress。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMonHPProgress
{
    public int nHPBGIndex;
    public short nHPBGX;
    public short nHPBGY;
    public int nImageIndex;
    public short nImageX;
    public short nImageY;
    public int nHPIndex;
    public short nHPX;
    public short nHPY;
    public short boShowLevel;
    public short nLevelX;
    public short nLevelY;
    public short boShowMonName;
    public short nMonNameX;
    public short nMonNameY;
    public short boShowHPValue;
    public short nHPValueX;
    public short nHPValueY;
    public short boShowHPPercent;
    public short nHPPercentX;
    public short nHPPercentY;
    public short boShowExpHinter;
    public short nExpHinterX;
    public short nExpHinterY;
    public byte btHorizAlign;
    public ushort wHPBlockCount;
    public short nHPBlockOffsetX;
    public short nHPBlockOffsetY;
    public fixed byte m_ExpHinterName[Grobal2Const.ACTOR_NAME_LEN + 1];

    public string ExpHinterName { get { fixed (byte* p = m_ExpHinterName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = m_ExpHinterName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[InlineArray(4)]
public struct IntArray4
{
    private int _e0;
}
