using System;
using GXX.Core.Protocol;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// 结构布局 / 就地写入语义的守卫测试。
///
/// 这些断言锁的是"原文用指针就地改写 record"这一前提在托管侧仍然成立 ——
/// 若 THumData/THeroData 的 InlineArray 语义变了（例如将来改成 class 或 get-only indexer），
/// MySqlRoleDB 的整档读写会静默失效，这里会先炸。
/// </summary>
[Collection("MySqlRoleDb")]
public class MySqlRoleDbLayoutTests
{
    [Fact]
    public void HumData_NpcSkillPowerAdd_IsWritableInPlace()
    {
        var hum = new THumData();
        hum.NpcSkillPowerAdd[4].RemainingTime = 1;
        hum.NpcSkillPowerAdd[4].HumanAttackPercent = 3;
        Assert.Equal(1, hum.NpcSkillPowerAdd[4].RemainingTime);
        Assert.Equal(3, hum.NpcSkillPowerAdd[4].HumanAttackPercent);
    }

    [Fact]
    public void HumData_CustomMoney_IsWritableInPlace()
    {
        var hum = new THumData();
        hum.CustomMoney[7].NameStr = "GameGold";
        hum.CustomMoney[7].nCount = 99;
        Assert.Equal("GameGold", hum.CustomMoney[7].NameStr);
        Assert.Equal(99, hum.CustomMoney[7].nCount);
    }

    [Fact]
    public void HumData_InlineByteArrays_AreWritableThroughHelpers()
    {
        var hum = new THumData();
        MySqlItemAccess.SetStorageOpen(ref hum, 1, 1);
        MySqlItemAccess.SetQuestFlag(ref hum, 5, 1);
        MySqlItemAccess.SetMeridianAcupoint(ref hum, 0, 0, 9);
        MySqlItemAccess.SetGamePetMagic(ref hum, 0, 0, 700);
        Assert.Equal(1, MySqlItemAccess.GetStorageOpen(ref hum, 1));
        Assert.Equal(1, MySqlItemAccess.GetQuestFlag(ref hum, 5));
        Assert.Equal(9, MySqlItemAccess.GetMeridianAcupoint(ref hum, 0, 0));
        Assert.Equal(700, MySqlItemAccess.GetGamePetMagic(ref hum, 0, 0));
    }

    [Fact]
    public void UserItem_FixedArrays_AreWritableThroughHelpers()
    {
        var item = new TUserItem();
        item.SetBtValue(3, 11);
        item.SetNewValue(4, 22);
        MySqlItemAccess.SetAddDataByte(ref item, 5, 33);
        MySqlItemAccess.SetAddDataInt(ref item, 6, 44);
        item.SetAddDataText(1, "x");

        Assert.Equal(11, item.GetBtValue(3));
        Assert.Equal(22, item.GetNewValue(4));
        Assert.Equal(33, MySqlItemAccess.GetAddDataByte(ref item, 5));
        Assert.Equal(44, MySqlItemAccess.GetAddDataInt(ref item, 6));
        Assert.Equal("x", item.GetAddDataText(1));
    }

    [Fact]
    public void HeroData_InlineByteArrays_AreWritableThroughHelpers()
    {
        var hero = new THeroData();
        MySqlItemAccess.SetHeroQuestFlag(ref hero, 9, 1);
        MySqlItemAccess.SetHeroMeridianAcupoint(ref hero, 1, 2, 7);
        Assert.Equal(1, MySqlItemAccess.GetHeroQuestFlag(ref hero, 9));
        Assert.Equal(7, MySqlItemAccess.GetHeroMeridianAcupoint(ref hero, 1, 2));
    }

    [Fact]
    public void HumData_IsARecordStruct_SoPassingByRefIsRequired()
    {
        // 原文处处用 PTHumData 指针；托管侧必须 ref 传递，值传递会丢改动
        var hum = new THumData();
        hum.ChrName = "chr";
        Mutate(hum);
        Assert.Equal("chr", hum.ChrName);        // 值传递 → 不变
        MutateRef(ref hum);
        Assert.Equal("changed", hum.ChrName);    // ref → 生效
    }

    private static void Mutate(THumData h) => h.ChrName = "changed";
    private static void MutateRef(ref THumData h) => h.ChrName = "changed";
}
