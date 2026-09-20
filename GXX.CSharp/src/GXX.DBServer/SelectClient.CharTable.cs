using System;
using System.Collections.Generic;
using GXX.Core.Rtl;

namespace GXX.DBServer;

// ============================================================================================
// SelectClient.pas:10-53 —— TUserInfo / pTUserInfo / TUserArray / TSelectChar
//
// 【托管映射说明】（不改变任何分支与边界行为）
//   · 原文 `TUserInfo = record` + `pTUserInfo = ^TUserInfo`，且全单元只经指针访问、就地改写。
//     §3.1「record（非 packed）→ struct」，但本 record **从不按值复制**，改用 `class` 才能保持
//     「同一实例被多处共享改写」的原文语义（与 MySqlRoleDB.Seam.cs 的 TQueryHumanData 同一处置）。
//   · 原文 `TUserArray = array[0..1000 - 1] of TUserInfo` 是**值数组**；
//     Delphi 里 class 的实例数据在堆上被清零，故 Create 之前每个槽的 Socket = nil。
//     托管侧 `TUserInfo[]` 的元素是 null 引用 ⇒ **必须显式 new 满 1000 个**，否则 `Add`/`Initialize`
//     会对 null 引用求值（原文没有这个状态）。见 CreateUserArray。
//   · 原文 `OnLineList/DeleteList: TList` 存 `Pointer(下标)` ⇒ 托管侧 `List<int>`。
//   · 原文 `nSelGateID: ShortInt`（**有符号字节**），而 OpenUser:699 从 `m_nGateID: Integer` 赋值。
//     Delphi 默认 {$R-}，该赋值**静默截断低 8 位**；本单元保留同一截断（见 TSelectClient.OpenUser）。
// ============================================================================================

/// <summary>SelectClient.pas:10-25 `TUserInfo`（record；全单元经 pTUserInfo 访问）。</summary>
public sealed class TUserInfo
{
    /// <summary>:11 `nIndex: Integer;`</summary>
    public int nIndex;

    /// <summary>:12 `sAccount: string;`</summary>
    public string sAccount = "";

    /// <summary>:13 `sUserIPaddr: string;`</summary>
    public string sUserIPaddr = "";

    /// <summary>:14 `sGateIPaddr: string;`</summary>
    public string sGateIPaddr = "";

    /// <summary>:16 `sConnID: string;`</summary>
    public string sConnID = "";

    /// <summary>:17 `nSessionID: Integer;`</summary>
    public int nSessionID;

    /// <summary>:18 `Socket: TCustomWinSocket;`（原文 nil 判定即"槽位空闲"）。</summary>
    public TCustomWinSocket? Socket;

    /// <summary>:19 `sReceiveText: string;`（AnsiString ⇒ 托管侧为 latin-1 字节串）。</summary>
    public string sReceiveText = "";

    /// <summary>:20 `boChrSelected: Boolean;`</summary>
    public bool boChrSelected;

    /// <summary>:21 `boChrQueryed: Boolean;`</summary>
    public bool boChrQueryed;

    /// <summary>:22 `dwTick34: LongWord;`</summary>
    public uint dwTick34;

    /// <summary>:23 `dwChrTick: LongWord;`</summary>
    public uint dwChrTick;

    /// <summary>:24 `nSelGateID: ShortInt;`（有符号字节）。</summary>
    public sbyte nSelGateID;
}

/// <summary>
/// SelectClient.pas:30-53 `TSelectChar`（1000 槽选人会话表）。
///
/// 这是本单元唯一的纯状态逻辑，也是本车道覆盖最扎实的部分。
/// 原文的每个方法都在这里逐字对应；差异只在「record→class」与「TList→List&lt;int&gt;」两种表示。
/// </summary>
public sealed class TSelectChar
{
    /// <summary>SelectClient.pas:28 `TUserArray = array[0..1000 - 1] of TUserInfo;`</summary>
    public const int USER_ARRAY_LENGTH = 1000;

    /// <summary>SelectClient.pas:32 `UserArray: TUserArray;`</summary>
    private readonly TUserInfo[] UserArray;

    /// <summary>SelectClient.pas:33 `OnLineList: TList;`（存 Pointer(下标)）。</summary>
    private readonly List<int> OnLineList = new List<int>();

    /// <summary>SelectClient.pas:34 `DeleteList: TList;`（存放回的下标）。</summary>
    private readonly List<int> DeleteList = new List<int>();

    /// <summary>
    /// SelectClient.pas:102-107 `constructor TSelectChar.Create`：
    /// <c>OnLineList := TList.Create; DeleteList := TList.Create; Initialize;</c>
    /// </summary>
    public TSelectChar()
    {
        UserArray = CreateUserArray();
        Initialize();
    }

    /// <summary>
    /// 对应 Delphi 堆对象的"实例数据已清零"：1000 个槽全部是**已初始化的空记录**（Socket = nil）。
    /// </summary>
    private static TUserInfo[] CreateUserArray()
    {
        var a = new TUserInfo[USER_ARRAY_LENGTH];
        for (int i = 0; i < a.Length; i++) a[i] = new TUserInfo();
        return a;
    }

    /// <summary>
    /// SelectClient.pas:109-114 `destructor TSelectChar.Destroy`：
    /// <c>Finalize; DeleteList.Free; OnLineList.Free;</c>
    /// （托管侧无 Free；保留方法以便 1:1 对照与显式复位。）
    /// </summary>
    public void Destroy()
    {
        Finalize();
    }

    /// <summary>
    /// SelectClient.pas:116-122 `function GetItem(Index: Integer): pTUserInfo`：
    /// 越界返回 nil（**不是抛异常**）。
    /// </summary>
    private TUserInfo? GetItem(int Index)
        => (Index >= 0) && (Index < UserArray.Length) ? UserArray[Index] : null;

    /// <summary>SelectClient.pas:124-127 `function GetCount: Integer`：`Result := Length(UserArray)` = 1000。</summary>
    private int GetCount() => UserArray.Length;

    /// <summary>
    /// SelectClient.pas:129-135 `function GetOnLineItem(Index: Integer): pTUserInfo`：
    /// 先按 OnLineList.Count 判界，再转 GetItem（下标来自 OnLineList，理论上必定合法）。
    /// </summary>
    private TUserInfo? GetOnLineItem(int Index)
        => (Index >= 0) && (Index < OnLineList.Count) ? GetItem(OnLineList[Index]) : null;

    /// <summary>SelectClient.pas:137-140 `function GetOnLineCount: Integer`。</summary>
    private int GetOnLineCount() => OnLineList.Count;

    /// <summary>SelectClient.pas:49 `property Items[Index: Integer]: pTUserInfo read GetItem;`</summary>
    public TUserInfo? Items(int Index) => GetItem(Index);

    /// <summary>SelectClient.pas:50 `property Count: Integer read GetCount;`（恒 1000）。</summary>
    public int Count => GetCount();

    /// <summary>SelectClient.pas:51 `property OnLineItems[Index: Integer]: pTUserInfo read GetOnLineItem;`</summary>
    public TUserInfo? OnLineItems(int Index) => GetOnLineItem(Index);

    /// <summary>SelectClient.pas:52 `property OnLineCount: Integer read GetOnLineCount;`</summary>
    public int OnLineCount => GetOnLineCount();

    /// <summary>
    /// SelectClient.pas:142-168 `function Add: Integer`。
    ///
    /// 原文分支：
    /// <code>
    /// Result := -1;
    /// if DeleteList.Count &gt; 0 then begin nIndex := Integer(DeleteList[0]); OnLineList.Add(Pointer(nIndex));
    ///   Result := nIndex; DeleteList.Delete(0); end
    /// else for I := 0 to Count - 1 do begin tUserInfo := Items[I];
    ///   if tUserInfo.Socket = nil then begin OnLineList.Add(Pointer(I)); Result := I; break; end; end;
    /// </code>
    /// ★ 原文如此：**回收分支不检查该槽是否已在 OnLineList 里**，也**不初始化**槽位
    ///   （初始化由调用方 OpenUser:688 负责）。表满且 DeleteList 为空时返回 **-1**。
    /// </summary>
    public int Add()
    {
        int Result = -1;
        if (DeleteList.Count > 0)
        {
            int nIndex = DeleteList[0];
            OnLineList.Add(nIndex);
            Result = nIndex;
            DeleteList.RemoveAt(0);
        }
        else
        {
            for (int I = 0; I <= Count - 1; I++)
            {
                TUserInfo? tUserInfo = Items(I);
                if (tUserInfo!.Socket == null)
                {
                    OnLineList.Add(I);
                    Result = I;
                    break;
                }
            }
        }
        return Result;
    }

    /// <summary>
    /// SelectClient.pas:170-195 `procedure Initialize; overload;`
    /// （清空两张表 + 把 1000 槽全部复位为"原始空记录"）。
    ///
    /// ★ 设的字段（14 个）：nIndex / sAccount / sUserIPaddr / sGateIPaddr / sConnID / nSessionID /
    ///   Socket / sReceiveText / dwTick34 / dwChrTick / boChrSelected / boChrQueryed / nSelGateID。
    ///   **与 Finalize（无参）不同**：见 Finalize 的差异说明。
    /// </summary>
    public void Initialize()
    {
        OnLineList.Clear();
        DeleteList.Clear();
        for (int I = 0; I <= Count - 1; I++)
        {
            TUserInfo? UserInfo = Items(I);
            UserInfo!.nIndex = -1;
            UserInfo.sAccount = "";
            UserInfo.sUserIPaddr = "";
            UserInfo.sGateIPaddr = "";

            UserInfo.sConnID = "";
            UserInfo.nSessionID = 0;
            UserInfo.Socket = null;
            UserInfo.sReceiveText = "";
            UserInfo.dwTick34 = DelphiRTL.GetTickCount();
            UserInfo.dwChrTick = DelphiRTL.GetTickCount();
            UserInfo.boChrSelected = false;
            UserInfo.boChrQueryed = false;
            UserInfo.nSelGateID = 0;
        }
    }

    /// <summary>
    /// SelectClient.pas:197-214 `procedure Finalize; overload;`
    ///
    /// ★★ **与 Initialize（无参）看起来一样，实则不同**（差异断言见测试）：
    ///   本方法只清 7 个字段 —— nIndex / Socket / sAccount / sUserIPaddr / sGateIPaddr / sConnID / sReceiveText；
    ///   **不**动 nSessionID / dwTick34 / dwChrTick / boChrSelected / boChrQueryed / nSelGateID，
    ///   也**不**清 OnLineList / DeleteList。
    ///   （对比 Initialize：清两张表，且连 nSessionID/boChr*/tick/nSelGateID 一起复位。）
    ///
    /// 【托管侧命名】Delphi `Finalize` 与 C# `object.Finalize` 同名，触发 CS0465/CS0114 警告；
    ///   本文件用 #pragma 抑制并**保留原名**（比改名更能 1:1 对照原文）。
    ///   注意本仓既有先例是改名（MySqlRoleDB.Seam.cs 的 FinalizeStatement）—— 这里选择保留原名，
    ///   登记为偏差 D-p7-3（仅警告抑制方式不同，无行为差异）。
    /// </summary>
#pragma warning disable CS0465, CS0114
    public void Finalize()
#pragma warning restore CS0465, CS0114
    {
        for (int I = 0; I <= Count - 1; I++)
        {
            TUserInfo? UserInfo = Items(I);
            UserInfo!.nIndex = -1;
            UserInfo.Socket = null;
            UserInfo.sAccount = "";
            UserInfo.sUserIPaddr = "";
            UserInfo.sGateIPaddr = "";

            UserInfo.sConnID = "";
            UserInfo.sReceiveText = "";
        }
    }

    /// <summary>
    /// SelectClient.pas:216-238 `procedure Initialize(Index: Integer); overload;`
    /// 与无参 Initialize 设**同一批 14 个字段**，但只针对一个槽，且**不清两张表**。
    /// <c>Items[Index]</c> 越界返回 nil ⇒ 原文整段 if 被跳过（**不抛异常**）；托管侧同。
    /// </summary>
    public void Initialize(int Index)
    {
        TUserInfo? UserInfo = Items(Index);
        if (UserInfo != null)
        {
            UserInfo.nIndex = -1;
            UserInfo.sAccount = "";
            UserInfo.sUserIPaddr = "";
            UserInfo.sGateIPaddr = "";

            UserInfo.sConnID = "";
            UserInfo.nSessionID = 0;
            UserInfo.Socket = null;
            UserInfo.sReceiveText = "";
            UserInfo.dwTick34 = DelphiRTL.GetTickCount();
            UserInfo.dwChrTick = DelphiRTL.GetTickCount();
            UserInfo.boChrSelected = false;
            UserInfo.boChrQueryed = false;
            UserInfo.nSelGateID = 0;
        }
    }

    /// <summary>
    /// SelectClient.pas:240-258 `procedure Finalize(Index: Integer); overload;`
    /// = 无参 Finalize 那 7 个字段 + <c>OnLineList.Remove(Pointer(Index))</c> + <c>DeleteList.Add(Pointer(Index))</c>。
    ///
    /// ★ 原文如此：<c>TList.Remove</c> 只删**第一个**匹配项；若同一槽在 OnLineList 里出现多次
    ///   （Add 的回收分支不查重，理论上可达），只摘掉一个。
    /// ★ 原文如此：**无条件**往 DeleteList 追加，不查重 ⇒ 多次 Finalize(同一 Index) 会让该下标
    ///   在 DeleteList 里出现多次，随后 Add 会把它**重复**放进 OnLineList。
    /// ★ 越界（Items[Index] = nil）时整段跳过，**不**动两张表。
    /// </summary>
#pragma warning disable CS0465, CS0114
    public void Finalize(int Index)
#pragma warning restore CS0465, CS0114
    {
        TUserInfo? UserInfo = Items(Index);
        if (UserInfo != null)
        {
            OnLineList.Remove(Index);
            DeleteList.Add(Index);
            UserInfo.nIndex = -1;
            UserInfo.Socket = null;
            UserInfo.sAccount = "";
            UserInfo.sUserIPaddr = "";
            UserInfo.sGateIPaddr = "";

            UserInfo.sConnID = "";
            UserInfo.sReceiveText = "";
        }
    }
}
