using System.Runtime.InteropServices;

namespace GXX.Core.Protocol;

// ============================================================================
// Grobal2.pas 类型部分 1:1 转换（枚举 + 基础记录）
// 字段名/顺序/类型与原 .pas 严格一致；string[N] → fixed byte[N+1]（ShortString 布局）
// ============================================================================

public enum TRandCodeType : byte { rctLogin, rctRegister, rctPwdGetback, rctPwdChange }

public enum TBlastHitType : byte { bhtNone = 0, bhtBlastHit = 1, bhtFatalBlow1 = 2, bhtFatalBlow2 = 3, bhtFatalBlow3 = 4, bhtFatalBlow4 = 5 }

public enum TJewelryBoxStatus : byte { jbsNoActive, jbsActive, jbsOpen }

public enum TItemFormType : byte { ifUnknow, ifGM, ifScript, ifShopBuy, ifMonDrop, ifSysGive, ifMine, ifBoxGive, ifButchItem, ifCaptureMon }

public enum TSockData : byte { st_LoadHumData, st_SaveHumData, st_LoadHeroData, st_SaveHeroData, st_LoadRankingData, st_SaveData, st_GamePoint, st_LoadGuildHumData, st_ChangeHumName, st_ChangeHeroName, st_ChangeHumanGold }

public enum THeroDataType : byte { dt_Create, dt_Delete, dt_Load, dt_CreateDeputyHero, dt_DeleteDeputyHero, dt_LoadDeputyHero, dt_Save, dt_QueryStorageHeroInfo, dt_QueryAssessHeroInfo, dt_AssessHero }

public enum TUnBindItemType : byte { t_UnKnow, t_HP, t_MP, t_Special, t_Book, t_Poison, t_Bujuk }

public enum TItemUpgradeRate : byte { u_None, u_Mon, u_Make, u_Script }

public enum TItemUpgradeType : byte { t_Weapon, t_Dress, t_NeckLace, t_ArmRing, t_Ring, t_HelMet, t_Shoes, t_Belt }

public enum TMagicAttr : byte { mtHum, mtHero, mtContinuous, mtDefense, mtAttack }

public enum TDBChangeGoldType : byte { cgtGold, cgtGameGold, cgtGamePoint, cgtGameDiamond, cgtGameGird, cgtCustomMoney }

public enum TDBQueryHumanInfoType : byte { qhiGuildMemberInfo, qhiGuildJoinUserList }

public enum TDBResultType : byte { drtLoadHuman, drtLoadDummy, drtLoadHero, drtChrRename, drtGetRankData, drtQueryHumanInfo, drtBuyPlayer, drtSellPlayerDelegator }

public enum TGameGoldDealState : byte { s_None, s_Normal, s_Cancel, s_Expired, s_Succeed }

public enum TVerticalAlignment : byte { taAlignTop, taAlignBottom, taVerticalCenter }

// ---- 基础消息结构（wire 核心）----

/// <summary>TDefaultMessage（非 packed，Delphi 32 位下自然对齐 = 16 字节：Int64+4*Word）</summary>
[StructLayout(LayoutKind.Sequential)]
public struct TDefaultMessage
{
    public long Recog;     // Int64，64位支持 2021-01-04
    public ushort Ident;
    public ushort Param;
    public ushort Tag;
    public ushort Series;

    public static readonly int SizeOf = 16;

    public static TDefaultMessage Make(ushort wIdent, long nRecog, ushort wParam, ushort wTag, ushort wSeries)
    {
        TDefaultMessage m = default;
        m.Recog = nRecog;
        m.Ident = wIdent;
        m.Param = wParam;
        m.Tag = wTag;
        m.Series = wSeries;
        return m;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct TM2MsgHeader
{
    public uint dwCode;
    public int nSocket;
    public ushort wGSocketIdx;
    public ushort wIdent;
    public uint wUserListIndex;
    public int nLength;
}

[StructLayout(LayoutKind.Sequential)]
public struct TRungateMsgHeader
{
    public uint Code;
    public uint DataLen;
    public TDefaultMessage Msg;
}

[StructLayout(LayoutKind.Sequential)]
public struct TSocketHeader
{
    public uint dwCode1;
    public ushort wIdent;
    public ushort wReserved;
    public uint dwCode2;
    public uint dwCrc;
    public int nLength;
}

[StructLayout(LayoutKind.Sequential)]
public struct TDBMsgHeader
{
    public uint dwCode;
    public uint dwCrc;
    public TDefaultMessage DefMsg;
    public int nLength;
}

[StructLayout(LayoutKind.Sequential)]
public struct TCheckDBMsgHeader
{
    public ushort wIndent;
    public ushort wData;
}

// ---- 消息体 ----

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TEventData
{
    public ushort Data1;
    public ushort Data2;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TFeature_New
{
    public long Value1;
    public long Value2;
    public ushort wDressEffType;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMessageBodyW
{
    public ushort Param1;
    public ushort Param2;
    public ushort Tag1;
    public ushort Tag2;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMessageBodyL
{
    public int lParam1;
    public int lParam2;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMessageBodyWL
{
    public int lParam1;
    public int lParam2;
    public int lTag1;
    public long lTag2;   // 64位修改 2021-01-04
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TNewMessageBodyWL
{
    public int lParam1;
    public int lParam2;
    public int lTag1;
    public int lTag2;
    public int lTag3;
    public int lTag4;
    public long lTag5;
    public TBlastHitType BlastHitType;
    public int ResID;         // <0 代表不启用
    public int ResStartIdx;   // <0 代表不启用
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMessageHealthSpellChangedInfo
{
    public uint ChangeHP;
    public byte IsAttackFromHum; // Boolean(1)
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TShortMessage
{
    public ushort Ident;
    public ushort wMsg;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TByteRect
{
    public byte Left;
    public byte Top;
    public byte Right;
    public byte Bottom;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TPushedObject
{
    public long nRecogId;   // 64位修改 2021-01-04
    public ushort nCurrX;
    public ushort nCurrY;
    public byte btDir;
    public byte btStep;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TCharDesc
{
    public byte Feature;   // 原注释 Integer，但声明为 Byte
    public long Status;
    public int MagicLevel;
}

// ---- 形象/外观 ----

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TActorIcon
{
    public short nFileIndex;   // WIL资源编号
    public ushort nIconIndex;
    public byte nIconCount;
    public short nX;
    public short nY;
    public byte boBlend;          // Boolean
    public byte btDrawOrder;      // 绘制顺序
    public short nPlayTime;       // 播放速度
    public byte boOnlySelfVisible; // Boolean
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TActorEffect
{
    public short nEffectFileIndex;
    public int nEffectImageOffSet;
    public ushort wEffectImageCount;
    public ushort wEffectFrameTime;
    public int nLoopCount;
    public byte btDrawOrder;
    public int nOffsetX;
    public int nOffsetY;
    public byte boBlendMode;
}

/// <summary>THumFeature：人物外观（packed，wire 使用）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct THumFeature
{
    public ushort wRace;             // 脸型
    public ushort wWeapon;           // 武器
    public ushort wWeaponSound;      // 武器声音 2020-11-24
    public ushort wDress;            // 衣服
    public ushort wShield;           // 盾牌
    public byte btGender;
    public byte btJob;
    public byte btHair;
    public byte btHorseType;
    public byte btCaseltGuild;       // 1=沙行会成员 2=沙行会掌门
    public byte boShopStall;         // Boolean 摆摊
    public byte btBodyColor;
    public byte btShopStallDir;
    public ushort wDressEffType;
    public ushort wDressEffType_30;
    public byte boDressEffNormalDraw;
    public byte boDressEffNoSex;
    public byte boDressEff_30NormalDraw;
    public byte boDressEff_30NoSex;
    public int nChangeAppr;
    public short nWeaponEffectIndex;
    public ushort wWeaponEffectOffSet;
    public ushort wDBWeaponEffectOffSet;
    public short nDressEffectIndex;
    public ushort wDressEffectOffSet;
    public short nShieldEffectIndex;
    public ushort wShieldEffectOffSet;
    public byte boDressEffectNoBlend;
    public byte boDressEffectNoSex;
    public byte boWeaponEffectNoBlend;
    public byte boWeaponEffectNoSex;
    public byte boShieldEffectNoBlend;
    public byte boShieldEffectNoSex;
    public short nShieldAddEffectIndex;
    public ushort wShieldAddEffectOffSet;
    public short nDressAddEffectIndex;
    public byte bDressAddEffectOrder;
    public ushort wDressAddEffectOffSet;
    public ushort wDressAddEffectCount;
    public ushort wDressAddEffectTime;
    public byte boDressAddEffectNoBlend;
    public byte boDressAddEffectDrawCenter;
    public short nMedalEffectIndex;
    public ushort wMedalEffectOffSet;
    public byte boMedalEffectNoBlend;
    public byte boMedalEffectNoSex;
    public byte btDoubleHumHorseType;
    public byte boShowHorseWingsEffect;
    public byte btHorseEffectType;
    public ushort btHorseHum;        // 原声明 Word
    public byte btHorseHumExpand;
    public byte btHorseHair;
    public byte boShowFashion;
    public byte boMagicShield;
    public int btReLevel;            // 原声明 Integer
    public fixed byte sActiveFengHaoName[Grobal2Const.ITEM_NAME_LEN + 1]; // string[60]
    public int nActiveFengHaoID;
    public ushort dwActiveFengHaoLooks;
    public byte btActiveFengHaoReserved;
    public byte btActiveFengHaoColor;
    public byte boShowHair;
    public byte btCboDressUseDiyImage;
    public byte btCboWeaponUseDiyImage;
    public byte btOldHair;
    public byte boPlayMoster;

    public string ActiveFengHaoName
    {
        get { fixed (byte* p = sActiveFengHaoName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); }
        set { fixed (byte* p = sActiveFengHaoName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); }
    }
}

public enum THumBBType : byte { bbNo, bbSlave, bbGamePet }

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMonFeature
{
    public ushort wRaceImg;
    public ushort wWeapon;
    public ushort wAppr;
    public byte btBodyColor;
    public byte btRace;
    public uint MonLevel;
    public THumBBType HumBBType;
    public byte IsExploreItem;          // Boolean
    public byte IsDisableSimpleActor;   // Boolean
    public int nChangeAppr;
}

/// <summary>TFeature：形象结构（Feature:int + 256 字节缓冲）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TFeature
{
    public int Feature;
    public fixed byte Buffer[256];

    public byte[] GetBuffer()
    {
        byte[] b = new byte[256];
        fixed (byte* p = Buffer)
        {
            for (int i = 0; i < 256; i++) b[i] = p[i];
        }
        return b;
    }
    public void SetBuffer(byte[] src)
    {
        fixed (byte* p = Buffer)
        {
            int n = src?.Length ?? 0; if (n > 256) n = 256;
            for (int i = 0; i < n; i++) p[i] = src[i];
            for (int i = n; i < 256; i++) p[i] = 0;
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUnBindItem
{
    public fixed byte sItemName[Grobal2Const.ITEM_NAME_LEN + 1];
    public int nStdMode;
    public int nShape;
    public TUnBindItemType UnBindItemType;
    public int nCount;

    public string ItemName { get { fixed (byte* p = sItemName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = sItemName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSetImageInfo
{
    public TFeature_New EffigyState;
    public fixed byte ChrName[51]; // string[50]

    public string ChrNameStr { get { fixed (byte* p = ChrName) return ShortStr.Get(p, 50); } set { fixed (byte* p = ChrName) ShortStr.Set(p, 50, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TProcessMessage
{
    public ushort wIdent;
    public byte boLateDelivery;
    public long wParam;      // 64位修改 2023-07-18
    public long nParam1;
    public long nParam2;
    public long nParam3;
    public long BaseObject;  // 64位修改 2023-07-18
    public uint dwTimeTick;
    public uint dwDeliveryTime;
    // sMsg: AnsiString 为引用类型，托管侧拆分为独立字段（见 TProcessMessageRef）
    public int nRev;
}

/// <summary>带字符串的进程消息（原 TProcessMessage.sMsg）。</summary>
public class TProcessMessageRef
{
    public ushort wIdent;
    public bool boLateDelivery;
    public long wParam;
    public long nParam1;
    public long nParam2;
    public long nParam3;
    public long BaseObject;
    public uint dwTimeTick;
    public uint dwDeliveryTime;
    public string sMsg = "";
    public int nRev;
}
