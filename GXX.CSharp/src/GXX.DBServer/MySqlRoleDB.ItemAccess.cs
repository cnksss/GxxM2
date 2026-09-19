using GXX.Core.Protocol;

namespace GXX.DBServer;

/// <summary>
/// fixed-buffer 定长数组访问 helper。
///
/// 两处需要它：
///   ① <c>TUserItem</c>（GXX.Core，Grobal2.Types6.cs）只公开了 btValue / btNewValue / Flutes 的访问器，
///      缺 btAddDataByte / nAddDataInt（DbServer 侧才用到）；
///   ② <c>THumData</c>/<c>THeroData</c> 里嵌套的 fixed 数组（经脉穴位、宠物技能、仓库开关、任务标记）
///      在 InlineArray 元素上无法直接在安全上下文里索引 —— 原文是 `HumData.Meridians[I].Acupoints[J]`。
///
/// 本文件**不改动 GXX.Core**（属他人分区），只在本车道内提供同语义的访问器。
/// </summary>
public static unsafe class MySqlItemAccess
{
    // ---------------- TUserItem ----------------

    /// <summary>btAddDataByte[Index] = Value（原文 `UserItem.btAddDataByte[Index2] := Value`）。</summary>
    public static void SetAddDataByte(ref TUserItem Item, int Index, byte Value)
    {
        fixed (TUserItem* p = &Item) { p->btAddDataByte[Index] = Value; }
    }

    /// <summary>btAddDataByte[Index]。</summary>
    public static byte GetAddDataByte(ref TUserItem Item, int Index)
    {
        fixed (TUserItem* p = &Item) { return p->btAddDataByte[Index]; }
    }

    /// <summary>nAddDataInt[Index] = Value。</summary>
    public static void SetAddDataInt(ref TUserItem Item, int Index, int Value)
    {
        fixed (TUserItem* p = &Item) { p->nAddDataInt[Index] = Value; }
    }

    /// <summary>nAddDataInt[Index]。</summary>
    public static int GetAddDataInt(ref TUserItem Item, int Index)
    {
        fixed (TUserItem* p = &Item) { return p->nAddDataInt[Index]; }
    }

    // ---------------- THumData / THeroData 内嵌 fixed 数组 ----------------

    /// <summary>HumData.Meridians[I].Level / BlastHitRate（非 fixed，安全访问）。</summary>
    public static void SetMeridianLevel(ref THumData H, int I, byte Level, byte BlastHitRate)
    {
        H.Meridians[I].Level = Level;
        H.Meridians[I].BlastHitRate = BlastHitRate;
    }

    /// <summary>Meridians[I].Acupoints[J] = Value（fixed byte[5]）。</summary>
    public static void SetMeridianAcupoint(ref THumData H, int I, int J, byte Value)
    {
        fixed (THumData* p = &H) { p->Meridians[I].Acupoints[J] = Value; }
    }

    /// <summary>Meridians[I].Acupoints[J]。</summary>
    public static byte GetMeridianAcupoint(ref THumData H, int I, int J)
    {
        fixed (THumData* p = &H) { return p->Meridians[I].Acupoints[J]; }
    }

    /// <summary>GamePetData[I].wMagics[J] = Value（fixed ushort[8]）。</summary>
    public static void SetGamePetMagic(ref THumData H, int I, int J, ushort Value)
    {
        fixed (THumData* p = &H) { p->GamePetData[I].wMagics[J] = Value; }
    }

    /// <summary>GamePetData[I].wMagics[J]。</summary>
    public static ushort GetGamePetMagic(ref THumData H, int I, int J)
    {
        fixed (THumData* p = &H) { return p->GamePetData[I].wMagics[J]; }
    }

    /// <summary>boStorageOpen[I] = Value（THumData 的 InlineArray&lt;byte,4&gt;）。</summary>
    public static void SetStorageOpen(ref THumData H, int I, byte Value)
    {
        fixed (THumData* p = &H) { p->boStorageOpen[I] = Value; }
    }

    /// <summary>boStorageOpen[I]。</summary>
    public static byte GetStorageOpen(ref THumData H, int I)
    {
        fixed (THumData* p = &H) { return p->boStorageOpen[I]; }
    }

    /// <summary>QuestFlag[I] = Value（THumData 的 InlineArray&lt;byte,128&gt;）。</summary>
    public static void SetQuestFlag(ref THumData H, int I, byte Value)
    {
        fixed (THumData* p = &H) { p->QuestFlag[I] = Value; }
    }

    /// <summary>QuestFlag[I]。</summary>
    public static byte GetQuestFlag(ref THumData H, int I)
    {
        fixed (THumData* p = &H) { return p->QuestFlag[I]; }
    }

    /// <summary>ContinuousMagicOrder[I]。</summary>
    public static byte GetContinuousMagicOrder(ref THumData H, int I)
    {
        fixed (THumData* p = &H) { return p->ContinuousMagicOrder[I]; }
    }

    // ---------------- THeroData 对应物 ----------------

    /// <summary>HeroData.Meridians[I].Level / BlastHitRate。</summary>
    public static void SetHeroMeridianLevel(ref THeroData H, int I, byte Level, byte BlastHitRate)
    {
        H.Meridians[I].Level = Level;
        H.Meridians[I].BlastHitRate = BlastHitRate;
    }

    /// <summary>HeroData.Meridians[I].Acupoints[J] = Value。</summary>
    public static void SetHeroMeridianAcupoint(ref THeroData H, int I, int J, byte Value)
    {
        fixed (THeroData* p = &H) { p->Meridians[I].Acupoints[J] = Value; }
    }

    /// <summary>HeroData.Meridians[I].Acupoints[J]。</summary>
    public static byte GetHeroMeridianAcupoint(ref THeroData H, int I, int J)
    {
        fixed (THeroData* p = &H) { return p->Meridians[I].Acupoints[J]; }
    }

    /// <summary>HeroData.QuestFlag[I] = Value。</summary>
    public static void SetHeroQuestFlag(ref THeroData H, int I, byte Value)
    {
        fixed (THeroData* p = &H) { p->QuestFlag[I] = Value; }
    }

    /// <summary>HeroData.QuestFlag[I]。</summary>
    public static byte GetHeroQuestFlag(ref THeroData H, int I)
    {
        fixed (THeroData* p = &H) { return p->QuestFlag[I]; }
    }

    /// <summary>HeroData.ContinuousMagicOrder[I]。</summary>
    public static byte GetHeroContinuousMagicOrder(ref THeroData H, int I)
    {
        fixed (THeroData* p = &H) { return p->ContinuousMagicOrder[I]; }
    }
}
