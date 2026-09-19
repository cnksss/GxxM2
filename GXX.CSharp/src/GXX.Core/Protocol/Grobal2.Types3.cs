using System.Runtime.InteropServices;

namespace GXX.Core.Protocol;

// ============================================================================
// Grobal2.pas 类型部分 3：属性/角色数据（TAbility / THumData / THeroData 等）
// ============================================================================

/// <summary>内功属性。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAbilityNG
{
    public ushort Level;
    public ushort NH;
    public ushort MaxNH;
    public uint Exp;
    public uint MaxExp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientAbilityNG
{
    public ushort Level;
    public ushort NH;
    public ushort MaxNH;
    public uint Exp;
    public uint MaxExp;
    public int NGDamage;
    public int UnNGDamage;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAbilityAlcohol
{
    public ushort Alcohol;
    public ushort MaxAlcohol;
    public ushort WineDrinkValue;
    public int MedicineLevel;
    public ushort MedicineValue;
    public ushort MaxMedicineValue;
}

/// <summary>TNakedAbility：Size 20。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TNakedAbility
{
    public int DC;
    public int MC;
    public int SC;
    public int AC;
    public int MAC;
    public int HP;
    public int MP;
    public int Hit;
    public int Speed;
    public int X2;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAdjustBonus
{
    public TNakedAbility BonusTick;
    public TNakedAbility BonusAbil;
    public TNakedAbility NakedAbil;
}

/// <summary>TNoBoundAbility：Size 40。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TNoBoundAbility
{
    public int AC1; public int AC2;
    public int MAC1; public int MAC2;
    public int DC1; public int DC2;
    public int MC1; public int MC2;
    public int SC1; public int SC2;
    public uint MaxHP;
    public uint MaxMP;
}

/// <summary>TAbility：21亿修改结构体（packed, Size 40 + NewValue[30]）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAbility
{
    public uint Level;
    public int AC1; public int AC2;
    public int MAC1; public int MAC2;
    public int DC1; public int DC2;
    public int MC1; public int MC2;
    public int SC1; public int SC2;
    public uint HP;
    public uint MP;
    public uint MaxHP;
    public uint MaxMP;
    public uint Exp;
    public uint MaxExp;
    public int Weight;
    public int MaxWeight;
    public int WearWeight;
    public int MaxWearWeight;
    public int HandWeight;
    public int MaxHandWeight;
    public int CreditPoint;
    public fixed uint NewValue[30];
    // 0暴击几率 1攻击伤害 2物减 3魔减 4忽视防御 5反弹 6暴率 7体力 8魔力 9怒气
    // 10合击 11怪爆 12防爆 13防麻痹 14防护身 15防复活 16防毒 17防诱惑 18防火墙
    // 19防冰冻 20防蛛网 21致命几率 22致命伤害 23致命防御 24暴击抗性

    public uint GetNewValue(int i) { fixed (uint* p = NewValue) return p[i]; }
    public void SetNewValue(int i, uint v) { fixed (uint* p = NewValue) p[i] = v; }
}

/// <summary>TOAbility：TAbility 的无 NewValue 版本。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TOAbility
{
    public int Level;
    public int AC1; public int AC2;
    public int MAC1; public int MAC2;
    public int DC1; public int DC2;
    public int MC1; public int MC2;
    public int SC1; public int SC2;
    public uint HP;
    public uint MP;
    public uint MaxHP;
    public uint MaxMP;
    public uint Exp;
    public uint MaxExp;
    public int Weight;
    public int MaxWeight;
    public int WearWeight;
    public int MaxWearWeight;
    public int HandWeight;
    public int MaxHandWeight;
    public int CreditPoint;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TAddAbility
{
    public uint wHP;
    public uint wMP;
    public ushort wHitPoint;
    public ushort wSpeedPoint;
    public int nAC1; public int nAC2;
    public int nMAC1; public int nMAC2;
    public int nDC1; public int nDC2;
    public int nMC1; public int nMC2;
    public int nSC1; public int nSC2;
    public byte bt1DF;              // 神圣
    public ushort wAntiPoison;
    public ushort wPoisonRecover;
    public ushort wHealthRecover;
    public ushort wSpellRecover;
    public ushort wAntiMagic;
    public byte btLuck;
    public byte btUnLuck;
    public int nHitSpeed;
    public byte btWeaponStrong;
    public ushort wNPRecoverTime;
    public ushort wNPRecoverPoint;
}

[StructLayout(LayoutKind.Sequential)]
public struct TWAbility
{
    public uint dwExp;
    public int wHP;
    public int wMP;
    public int wMaxHP;
    public int wMaxMP;
}

/// <summary>TMeridian：经脉。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMeridian
{
    public fixed byte Acupoints[5];  // 每脉5穴位是否打通
    public byte Level;
    public byte BlastHitRate;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TSaveNpcSkillPowerAdd
{
    public short HumanAttackPercent;
    public short HumanAttackValue;
    public short MonAttackPercent;
    public short MonAttackValue;
    public short DefensePercent;
    public short DefenseValue;
    public ushort RemainingTime;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TMoney
{
    public fixed byte sName[31];   // string[30]
    public int nCount;

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, 30); } set { fixed (byte* p = sName) ShortStr.Set(p, 30, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TStorageViewItemListHeader
{
    public int Count;
    public int MaxCount;
    public ushort Page;
    public ushort MaxPage;
    public ushort PosX;
    public ushort PosY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TDBGuildMemberInfo
{
    public fixed byte sName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte btSex;
    public byte btJob;
    public uint nLevel;
    public double dtLastLogin;

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGuildMemeberInfo
{
    public fixed byte sName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public byte boOnline;
    public byte boMaster;
    public byte btSex;
    public byte btJob;
    public uint nLevel;
    public fixed byte nRankName[101];  // string[100]
    public double dtLastLogin;

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string RankName { get { fixed (byte* p = nRankName) return ShortStr.Get(p, 100); } set { fixed (byte* p = nRankName) ShortStr.Set(p, 100, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGroupMember
{
    public fixed byte sCharName[Grobal2Const.ACTOR_NAME_LEN + 1];
    public fixed byte sMapName[Grobal2Const.MAP_NAME_LEN + 1];
    public fixed byte sMapDesc[41];
    public byte Job;
    public byte Gender;
    public byte IsCaptain;
    public long nRecogId;
    public uint Level;
    public uint HP;
    public uint MaxHP;
    public uint MP;
    public uint MaxMP;

    public string CharName { get { fixed (byte* p = sCharName) return ShortStr.Get(p, Grobal2Const.ACTOR_NAME_LEN); } set { fixed (byte* p = sCharName) ShortStr.Set(p, Grobal2Const.ACTOR_NAME_LEN, value); } }
    public string MapName { get { fixed (byte* p = sMapName) return ShortStr.Get(p, Grobal2Const.MAP_NAME_LEN); } set { fixed (byte* p = sMapName) ShortStr.Set(p, Grobal2Const.MAP_NAME_LEN, value); } }
    public string MapDesc { get { fixed (byte* p = sMapDesc) return ShortStr.Get(p, 40); } set { fixed (byte* p = sMapDesc) ShortStr.Set(p, 40, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGamePetData
{
    public fixed byte sName[Grobal2Const.ITEM_NAME_LEN + 1];
    public uint Level;
    public uint HP;
    public uint MP;
    public uint Exp;
    public fixed ushort wMagics[Grobal2Const.MAX_GAMEPET_MAGIC_COUNT];
    public uint DieTick;

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = sName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TGamePetAbility
{
    public uint Level;
    public int AC1; public int AC2;
    public int MAC1; public int MAC2;
    public int DC1; public int DC2;
    public int MC1; public int MC2;
    public int SC1; public int SC2;
    public uint HP;
    public uint MP;
    public uint MaxHP;
    public uint MaxMP;
    public uint Exp;
    public uint MaxExp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientGamePetShowConfig
{
    public ushort wAppr;
    public short ShowFile1; public short ShowStart1; public short ShowCount1; public short ShowTime1; public short ShowOffsetX1; public short ShowOffsetY1;
    public short ShowFile2; public short ShowStart2; public short ShowCount2; public short ShowTime2; public short ShowOffsetX2; public short ShowOffsetY2;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TClientGamePetData
{
    public fixed byte sName[Grobal2Const.ITEM_NAME_LEN + 1];
    public TGamePetAbility GamePetAbility;
    public fixed ushort wMagics[Grobal2Const.MAX_GAMEPET_MAGIC_COUNT];
    public TClientGamePetShowConfig ShowConfig;

    public string NameStr { get { fixed (byte* p = sName) return ShortStr.Get(p, Grobal2Const.ITEM_NAME_LEN); } set { fixed (byte* p = sName) ShortStr.Set(p, Grobal2Const.ITEM_NAME_LEN, value); } }
}

// ---- 账号体系 ----

/// <summary>TAccountInfo：新账号结构。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TAccountInfo
{
    public fixed byte AccountName[15];   // string[14]
    public byte IsDisable;
    public fixed byte Password[11];      // string[10]
    public fixed byte UserName[21];      // string[20]
    public fixed byte IDCard[19];        // string[18]
    public fixed byte BirthDay[11];      // string[10]
    public fixed byte Questions1[21];    // string[20]
    public fixed byte Answers1[13];      // string[12]
    public fixed byte Questions2[21];    // string[20]
    public fixed byte Answers2[13];      // string[12]
    public fixed byte Phone[15];         // string[14]
    public fixed byte MobilePhone[14];   // string[13]
    public fixed byte Mail[41];          // string[40]
    public fixed byte L2Password[21];    // string[20]
    public int CreateDate;
    public int LoginDate;
    public fixed byte LoginMac[33];      // string[32]
    public int LoginIP;
    public uint LastActionTick;
    public int ErrorCount;
    public fixed byte Memo[21];          // string[20]
    public fixed byte UID[71];           // string[70]
    public fixed byte CID[31];           // string[30]

    public string Get(int idx)
    {
        fixed (byte* basePtr = AccountName)
        {
            // 各字段容量表
            int[] caps = { 14, 10, 20, 18, 10, 20, 12, 20, 12, 14, 13, 40, 20, 0, 0, 32, 0, 0, 0, 20, 70, 30 };
            // 计算偏移
            int off = 0;
            int[] order = { 14, 10, 20, 18, 10, 20, 12, 20, 12, 14, 13, 40, 20 };
            if (idx < order.Length)
            {
                for (int i = 0; i < idx; i++) off += order[i] + 1;
                return ShortStr.Get(basePtr + off, order[idx]);
            }
            return "";
        }
    }

    public string AccountNameStr { get { fixed (byte* p = AccountName) return ShortStr.Get(p, 14); } set { fixed (byte* p = AccountName) ShortStr.Set(p, 14, value); } }
    public string PasswordStr { get { fixed (byte* p = Password) return ShortStr.Get(p, 10); } set { fixed (byte* p = Password) ShortStr.Set(p, 10, value); } }
    public string UserNameStr { get { fixed (byte* p = UserName) return ShortStr.Get(p, 20); } set { fixed (byte* p = UserName) ShortStr.Set(p, 20, value); } }
    public string IDCardStr { get { fixed (byte* p = IDCard) return ShortStr.Get(p, 18); } set { fixed (byte* p = IDCard) ShortStr.Set(p, 18, value); } }
    public string BirthDayStr { get { fixed (byte* p = BirthDay) return ShortStr.Get(p, 10); } set { fixed (byte* p = BirthDay) ShortStr.Set(p, 10, value); } }
    public string Questions1Str { get { fixed (byte* p = Questions1) return ShortStr.Get(p, 20); } set { fixed (byte* p = Questions1) ShortStr.Set(p, 20, value); } }
    public string Answers1Str { get { fixed (byte* p = Answers1) return ShortStr.Get(p, 12); } set { fixed (byte* p = Answers1) ShortStr.Set(p, 12, value); } }
    public string Questions2Str { get { fixed (byte* p = Questions2) return ShortStr.Get(p, 20); } set { fixed (byte* p = Questions2) ShortStr.Set(p, 20, value); } }
    public string Answers2Str { get { fixed (byte* p = Answers2) return ShortStr.Get(p, 12); } set { fixed (byte* p = Answers2) ShortStr.Set(p, 12, value); } }
    public string PhoneStr { get { fixed (byte* p = Phone) return ShortStr.Get(p, 14); } set { fixed (byte* p = Phone) ShortStr.Set(p, 14, value); } }
    public string MobilePhoneStr { get { fixed (byte* p = MobilePhone) return ShortStr.Get(p, 13); } set { fixed (byte* p = MobilePhone) ShortStr.Set(p, 13, value); } }
    public string MailStr { get { fixed (byte* p = Mail) return ShortStr.Get(p, 40); } set { fixed (byte* p = Mail) ShortStr.Set(p, 40, value); } }
    public string L2PasswordStr { get { fixed (byte* p = L2Password) return ShortStr.Get(p, 20); } set { fixed (byte* p = L2Password) ShortStr.Set(p, 20, value); } }
    public string LoginMacStr { get { fixed (byte* p = LoginMac) return ShortStr.Get(p, 32); } set { fixed (byte* p = LoginMac) ShortStr.Set(p, 32, value); } }
    public string MemoStr { get { fixed (byte* p = Memo) return ShortStr.Get(p, 20); } set { fixed (byte* p = Memo) ShortStr.Set(p, 20, value); } }
    public string UidStr { get { fixed (byte* p = UID) return ShortStr.Get(p, 70); } set { fixed (byte* p = UID) ShortStr.Set(p, 70, value); } }
    public string CidStr { get { fixed (byte* p = CID) return ShortStr.Get(p, 30); } set { fixed (byte* p = CID) ShortStr.Set(p, 30, value); } }
}

/// <summary>TUserEntry：老版账号注册结构（兼容老客户端）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserEntry
{
    public fixed byte sAccount[11];      // string[10]
    public fixed byte sPassword[11];     // string[10]
    public fixed byte sUserName[21];     // string[20]
    public fixed byte sSSNo[15];         // string[14]
    public fixed byte sPhone[15];        // string[14]
    public fixed byte sQuiz[21];         // string[20]
    public fixed byte sAnswer[13];       // string[12]
    public fixed byte sEMail[41];        // string[40]
    public fixed byte sRandCode[11];     // string[10]

    public string Account { get { fixed (byte* p = sAccount) return ShortStr.Get(p, 10); } set { fixed (byte* p = sAccount) ShortStr.Set(p, 10, value); } }
    public string Password { get { fixed (byte* p = sPassword) return ShortStr.Get(p, 10); } set { fixed (byte* p = sPassword) ShortStr.Set(p, 10, value); } }
    public string UserName { get { fixed (byte* p = sUserName) return ShortStr.Get(p, 20); } set { fixed (byte* p = sUserName) ShortStr.Set(p, 20, value); } }
    public string SSNo { get { fixed (byte* p = sSSNo) return ShortStr.Get(p, 14); } set { fixed (byte* p = sSSNo) ShortStr.Set(p, 14, value); } }
    public string Phone { get { fixed (byte* p = sPhone) return ShortStr.Get(p, 14); } set { fixed (byte* p = sPhone) ShortStr.Set(p, 14, value); } }
    public string Quiz { get { fixed (byte* p = sQuiz) return ShortStr.Get(p, 20); } set { fixed (byte* p = sQuiz) ShortStr.Set(p, 20, value); } }
    public string Answer { get { fixed (byte* p = sAnswer) return ShortStr.Get(p, 12); } set { fixed (byte* p = sAnswer) ShortStr.Set(p, 12, value); } }
    public string EMail { get { fixed (byte* p = sEMail) return ShortStr.Get(p, 40); } set { fixed (byte* p = sEMail) ShortStr.Set(p, 40, value); } }
    public string RandCode { get { fixed (byte* p = sRandCode) return ShortStr.Get(p, 10); } set { fixed (byte* p = sRandCode) ShortStr.Set(p, 10, value); } }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TUserEntryAdd
{
    public fixed byte sQuiz2[21];        // string[20]
    public fixed byte sAnswer2[13];      // string[12]
    public fixed byte sBirthDay[11];     // string[10]
    public fixed byte sMobilePhone[14];  // string[13]
    public fixed byte sMemo[21];         // string[20]
    public fixed byte sL2Password[21];   // string[20]

    public string Quiz2 { get { fixed (byte* p = sQuiz2) return ShortStr.Get(p, 20); } set { fixed (byte* p = sQuiz2) ShortStr.Set(p, 20, value); } }
    public string Answer2 { get { fixed (byte* p = sAnswer2) return ShortStr.Get(p, 12); } set { fixed (byte* p = sAnswer2) ShortStr.Set(p, 12, value); } }
    public string BirthDay { get { fixed (byte* p = sBirthDay) return ShortStr.Get(p, 10); } set { fixed (byte* p = sBirthDay) ShortStr.Set(p, 10, value); } }
    public string MobilePhone { get { fixed (byte* p = sMobilePhone) return ShortStr.Get(p, 13); } set { fixed (byte* p = sMobilePhone) ShortStr.Set(p, 13, value); } }
    public string Memo { get { fixed (byte* p = sMemo) return ShortStr.Get(p, 20); } set { fixed (byte* p = sMemo) ShortStr.Set(p, 20, value); } }
    public string L2Password { get { fixed (byte* p = sL2Password) return ShortStr.Get(p, 20); } set { fixed (byte* p = sL2Password) ShortStr.Set(p, 20, value); } }
}
