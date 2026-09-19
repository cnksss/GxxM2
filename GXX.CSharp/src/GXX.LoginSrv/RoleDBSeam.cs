using System;
using System.Collections.Generic;

namespace GXX.LoginSrv;

/// <summary>RoleDB.pas TSerarchRoleData（uFrmDataManager 依赖子集，1:1 字段）。</summary>
public sealed class TSerarchRoleData
{
    public string Account = "";
    public string RoleName = "";
    public int IsDelete;
    public bool IsHero;
    public int Sex;
    public int Job;
    public uint Level;
}

/// <summary>RoleDB.pas TSerarchRoleList（TList 持有 PTSerarchRoleData）。</summary>
public sealed class TSerarchRoleList
{
    private readonly List<TSerarchRoleData> _list = new();

    public int Count => _list.Count;
    public List<TSerarchRoleData> Items => _list;

    public TSerarchRoleData Add(TSerarchRoleData SerarchRoleData)
    {
        _list.Add(SerarchRoleData);
        return SerarchRoleData;
    }

    public void Clear() => _list.Clear();
}

/// <summary>接缝：DBShare.pas THumData（未移植，仅作为不透明句柄在窗体间传递）。</summary>
public sealed class THumData
{
}

/// <summary>接缝：DBShare.pas THeroData（未移植）。</summary>
public sealed class THeroData
{
}

/// <summary>接缝：RoleDB.pas THumanDB 最小面。</summary>
public interface IHumanRoleDB
{
    int SearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    int SearchByName(string HumanName, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    bool Get(string Account, string HumanName, ref THumData? HumData, out int HumanID);
    bool SetEnabled(string Account, string HumanName, int Enabled);
    bool Erase(string Account, string HumanName);
}

/// <summary>接缝：RoleDB.pas THeroDB 最小面。</summary>
public interface IHeroRoleDB
{
    int SearchByAccount(string Account, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    int SearchByName(string HeroName, TSearchMatchType MatchType, TSerarchRoleList RoleList);
    bool Get(string HeroName, ref THeroData? HeroData, out int HeroID);
    bool Erase(string HeroName);
}

/// <summary>接缝：RoleDB.pas TRoleDB（g_RoleDB）。</summary>
public interface IRoleDB
{
    IHumanRoleDB HumanDB { get; }
    IHeroRoleDB HeroDB { get; }
}

/// <summary>
/// 接缝宿主：RoleDB.pas 的全局 g_RoleDB / uFrmRoleDataEdit.pas 的 ShowFrmRoleDataEdit。
/// 待 DBServer 侧 RoleDB/SqliteRoleDB/uFrmRoleDataEdit 移植后接入。
/// </summary>
public static class LoginSrvRoleDb
{
    public static IRoleDB? g_RoleDB;

    /// <summary>uFrmRoleDataEdit.pas ShowFrmRoleDataEdit(HumanID, HumData, HeroData)。</summary>
    public static Action<int, THumData?, THeroData?> ShowFrmRoleDataEdit = (_, _, _) => { };

    public static void ResetForTests()
    {
        g_RoleDB = null;
        ShowFrmRoleDataEdit = (_, _, _) => { };
    }
}
