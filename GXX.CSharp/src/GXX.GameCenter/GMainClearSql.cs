using System;

namespace GXX.GameCenter;

/// <summary>
/// GMain.pas:5730-5845 清库 SQL 常量函数 1:1 移植（原文为多段 <c>+ sLineBreak +</c> 拼接）。
/// <c>sLineBreak</c> 在 Windows 下为 CRLF（#13#10）。
/// </summary>
public static class GMainClearSql
{
    /// <summary>System.pas <c>sLineBreak</c>（Windows：CRLF）。</summary>
    public const string sLineBreak = "\r\n";

    /// <summary>GMain.pas:5730 <c>function GetClearAccountDBSql: string;</c></summary>
    public static string GetClearAccountDBSql()
    {
        return "delete from Account;";
    }

    /// <summary>GMain.pas:5735 <c>function GetClearRoleDataDBSql: string;</c></summary>
    public static string GetClearRoleDataDBSql()
    {
        return
          "delete from HeroItemElementAdd;" + sLineBreak +
          "delete from HeroItemAddDataByte;" + sLineBreak +
          "delete from HeroItemAddDataInt;" + sLineBreak +
          "delete from HeroItemAddDataText;" + sLineBreak +
          "delete from HeroItemFlute;" + sLineBreak +
          "delete from HeroItemProgress;" + sLineBreak +
          "delete from HeroItemProperty;" + sLineBreak +
          "delete from HeroItemValueAdd;" + sLineBreak +
          "delete from HeroItems;" + sLineBreak +
          //'-------------------------------' + sLineBreak +
          "delete from HeroAbil;" + sLineBreak +
          "delete from HeroAbilNG;" + sLineBreak +
          "delete from HeroAbilNpcAdd;" + sLineBreak +
          "delete from HeroAbilWine;" + sLineBreak +

          "delete from HeroGodBlessState;" + sLineBreak +
          "delete from HeroMagic;" + sLineBreak +
          "delete from HeroQuestFlag;" + sLineBreak +
          "delete from HeroStatusTime;" + sLineBreak +

          "delete from HeroSkillPower;" + sLineBreak +

          "delete from Hero;" + sLineBreak +

          //'-------------------------------' + sLineBreak +
          "delete from HumanItemElementAdd;" + sLineBreak +
          "delete from HumanItemAddDataByte;" + sLineBreak +
          "delete from HumanItemAddDataInt;" + sLineBreak +
          "delete from HumanItemAddDataText;" + sLineBreak +
          "delete from HumanItemFlute;" + sLineBreak +
          "delete from HumanItemProgress;" + sLineBreak +
          "delete from HumanItemProperty;" + sLineBreak +
          "delete from HumanItemValueAdd;" + sLineBreak +
          "delete from HumanItems;" + sLineBreak +
          //'-------------------------------' + sLineBreak +
          "delete from HumanAbil;" + sLineBreak +
          "delete from HumanAbilNG;" + sLineBreak +
          "delete from HumanAbilNpcAdd;" + sLineBreak +
          "delete from HumanAbilWine;" + sLineBreak +
          "delete from HumanGamePetData;" + sLineBreak +
          "delete from HumanGodBlessState;" + sLineBreak +
          "delete from HumanMagic;" + sLineBreak +
          "delete from HumanMagicUseTick;" + sLineBreak +
          "delete from HumanQuestFlag;" + sLineBreak +
          "delete from HumanStatusTime;" + sLineBreak +
          "delete from HumanVariableT;" + sLineBreak +
          "delete from HumanVariableU;" + sLineBreak +

          "delete from HumanSkillPower;" + sLineBreak +

          "delete from Human;" + sLineBreak;
    }

    /// <summary>GMain.pas:5791 <c>function GetClearUserShopSql: string;</c></summary>
    public static string GetClearUserShopSql()
    {
        return
          "delete from ItemElementAdd where ItemType = 1;" + sLineBreak +
          "delete from ItemAddDataByte where ItemType = 1;" + sLineBreak +
          "delete from ItemAddDataInt where ItemType = 1;" + sLineBreak +
          "delete from ItemAddDataText where ItemType = 1;" + sLineBreak +
          "delete from ItemFlute where ItemType = 1;" + sLineBreak +
          "delete from ItemProgress where ItemType = 1;" + sLineBreak +
          "delete from ItemProperty where ItemType = 1;" + sLineBreak +
          "delete from ItemValueAdd where ItemType = 1;" + sLineBreak +
          "delete from Items where ItemType = 1;" + sLineBreak +
          //'-------------------------------' + sLineBreak +

          "delete from UserShopItem;" + sLineBreak +
          "delete from UserShop;" + sLineBreak;
    }

    /// <summary>GMain.pas:5809 <c>function GetClearStorageExSql: string;</c></summary>
    public static string GetClearStorageExSql()
    {
        return
          "delete from ItemElementAdd where ItemType = 0;" + sLineBreak +
          "delete from ItemAddDataByte where ItemType = 0;" + sLineBreak +
          "delete from ItemAddDataInt where ItemType = 0;" + sLineBreak +
          "delete from ItemAddDataText where ItemType = 0;" + sLineBreak +
          "delete from ItemFlute where ItemType = 0;" + sLineBreak +
          "delete from ItemProgress where ItemType = 0;" + sLineBreak +
          "delete from ItemProperty where ItemType = 0;" + sLineBreak +
          "delete from ItemValueAdd where ItemType = 0;" + sLineBreak +
          "delete from Items where ItemType = 0;" + sLineBreak +
          //'-------------------------------' + sLineBreak +

          "delete from StorageEx;" + sLineBreak;
    }

    /// <summary>GMain.pas:5826 <c>function GetClearAuctionDataSql: string;</c></summary>
    public static string GetClearAuctionDataSql()
    {
        return
          "delete from ItemElementAdd where ItemType = 5;" + sLineBreak +
          "delete from ItemAddDataByte where ItemType = 5;" + sLineBreak +
          "delete from ItemAddDataInt where ItemType = 5;" + sLineBreak +
          "delete from ItemAddDataText where ItemType = 5;" + sLineBreak +
          "delete from ItemFlute where ItemType = 5;" + sLineBreak +
          "delete from ItemProgress where ItemType = 5;" + sLineBreak +
          "delete from ItemProperty where ItemType = 5;" + sLineBreak +
          "delete from ItemValueAdd where ItemType = 5;" + sLineBreak +
          "delete from Items where ItemType = 5;" + sLineBreak +
          //'-------------------------------' + sLineBreak +

          "delete from AuctionAttention;" + sLineBreak +
          "delete from AuctionData;" + sLineBreak;

        //'-------------------------------' + sLineBreak +
        //'update sqlite_sequence set seq = 0;' + sLineBreak;
    }
}
