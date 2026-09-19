using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.LoginSrv;

/// <summary>AccountDB.pas TAccountUpdateField。</summary>
public enum TAccountUpdateField
{
    ufPartField,
    ufAllField,
}

/// <summary>RoleDB.pas TSearchMatchType（uFrmDataManager 依赖）。</summary>
public enum TSearchMatchType
{
    smtComplete,
    smtFuzzy,
}

/// <summary>
/// AccountDB.pas TAccountList 1:1：TList 存 TAccountInfo 副本，Add 返回新条目。
/// 接缝说明：Delphi 侧为 PTAccountInfo 指针（New/Dispose），C# 侧以值语义 List&lt;TAccountInfo&gt; 等价，
/// 元素的读写在窗体侧均为"取副本→改→写回"（与原文一致）。
/// </summary>
public sealed class TAccountList
{
    private readonly List<TAccountInfo> _list = new();

    public int Count => _list.Count;

    public TAccountInfo this[int Index]
    {
        get => Index >= 0 && Index < _list.Count ? _list[Index] : default;
        set { if (Index >= 0 && Index < _list.Count) _list[Index] = value; }
    }

    /// <summary>AccountDB.pas:101 TAccountList.Add（New(Result); Result^ := AccountInfo）。</summary>
    public TAccountInfo Add(TAccountInfo AccountInfo)
    {
        _list.Add(AccountInfo);
        return AccountInfo;
    }

    /// <summary>AccountDB.pas:108 TAccountList.Clear（Dispose 全部条目）。</summary>
    public void Clear() => _list.Clear();

    public List<TAccountInfo> Items => _list;
}

/// <summary>
/// AccountDB.pas TAccountDB 抽象基类 1:1（Lock 临界区 + try/except → MainOutMessage）。
/// 接缝：SqliteAccountDB.pas 已由既有 AccountDatabase.cs 承载（本车道只读参考，未改动）；
/// MySqlAccountDB.pas 由 MySqlAccountDB.cs 实现本抽象。
/// </summary>
public abstract class TAccountDB
{
    private readonly object _criticalSection = new();

    public string FileName { get; }

    protected TAccountDB(string FileName)
    {
        this.FileName = FileName;
    }

    protected void Lock() { System.Threading.Monitor.Enter(_criticalSection); }
    protected void UnLock() { System.Threading.Monitor.Exit(_criticalSection); }

    protected abstract void DoInit();
    protected abstract void DoFinal();

    protected abstract bool DoGetAccountByQuick(string UID, string ID, ref TAccountInfo AccountInfo);
    protected abstract bool DoGetAccountByPhone(string Phone, ref TAccountInfo AccountInfo);
    protected abstract bool DoGetAccount(string AccountName, ref TAccountInfo AccountInfo);
    protected abstract int DoFindAccount(string AccountName, TAccountList AccountList);
    protected abstract bool DoUpdateAccount(TAccountInfo AccountInfo, TAccountUpdateField UpdateField);

    protected abstract void DoGetAllAccount(GXX.Core.Util.TStringList AccountList);
    protected abstract bool DoEnabledAccounts(GXX.Core.Util.TStringList AccountList, bool Enabled);

    protected abstract bool DoCheckAccountExists(string AccountName);
    protected abstract bool DoAddAccount(TAccountInfo AccountInfo);

    protected abstract bool DoUnLockAccount(string AccountName);

    public void Init() => DoInit();
    public void Fainal() => DoFinal();

    public bool GetAccountByQuick(string UID, string ID, ref TAccountInfo AccountInfo)
    {
        Lock();
        try
        {
            try { return DoGetAccountByQuick(UID, ID, ref AccountInfo); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public bool GetAccountByPhone(string Phone, ref TAccountInfo AccountInfo)
    {
        Lock();
        try
        {
            try { return DoGetAccountByPhone(Phone, ref AccountInfo); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public bool GetAccount(string AccountName, ref TAccountInfo AccountInfo)
    {
        Lock();
        try
        {
            try { return DoGetAccount(AccountName, ref AccountInfo); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public int FindAccount(string AccountName, TAccountList AccountList)
    {
        Lock();
        try
        {
            try { return DoFindAccount(AccountName, AccountList); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return 0; }
        }
        finally { UnLock(); }
    }

    public bool UpdateAccount(TAccountInfo AccountInfo, TAccountUpdateField UpdateField)
    {
        Lock();
        try
        {
            try { return DoUpdateAccount(AccountInfo, UpdateField); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public void GetAllAccount(GXX.Core.Util.TStringList AccountList)
    {
        Lock();
        try
        {
            try { DoGetAllAccount(AccountList); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); }
        }
        finally { UnLock(); }
    }

    public bool EnabledAccounts(GXX.Core.Util.TStringList AccountList, bool Enabled)
    {
        Lock();
        try
        {
            try { return DoEnabledAccounts(AccountList, Enabled); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public bool CheckAccountExists(string AccountName)
    {
        Lock();
        try
        {
            try { return DoCheckAccountExists(AccountName); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public bool AddAccount(TAccountInfo AccountInfo)
    {
        Lock();
        try
        {
            try { return DoAddAccount(AccountInfo); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    public bool UnLockAccount(string AccountName)
    {
        Lock();
        try
        {
            try { return DoUnLockAccount(AccountName); }
            catch (Exception E) { LoginSrvShare.MainOutMessage(E.Message); return false; }
        }
        finally { UnLock(); }
    }

    /// <summary>AccountDB.pas:350 TAccountDB.Run（原文空实现）。</summary>
    public virtual void Run()
    {
    }
}

/// <summary>
/// 接缝：LMain.pas 的 WriteLogMsg(sIdent, AccountInfo)（账号变更日志）。
/// 待 LMain 移植后接入真实实现。
/// </summary>
public static class LoginSrvLog
{
    public static Action<string, TAccountInfo> WriteLogMsg = (_, _) => { };
}
