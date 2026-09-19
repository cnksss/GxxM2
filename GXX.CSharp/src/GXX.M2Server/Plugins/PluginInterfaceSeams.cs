using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Plugins;

// =====================================================================================
// PluginInterface.pas / PluginImplement.pas 的**接缝层**（seam layer）。
//
// 原文把宿主侧真实类型的**指针**直接交给插件（`_TList = TList` 等别名，见
// PluginInterface.pas:45-73）。这些类型属于 M2Server 的其它单元（Envir/ObjBase/ObjPlayer/
// ObjHero/ObjNpc/ObjDummy/Guild/uMagicACUtils/M2Share…），本车道**不顺手移植**，
// 因此在此定义最小接缝接口：
//   - 成员集合 = PluginImplement.pas 里宿主实现体**实际访问**的那部分 API（逐个反推）；
//   - 成员名沿用原文（含原文拼写：`m_boISNGMonster`、`m_PoisonHiter`、`GetRankName2`、
//     `GeTItemObjects`、`GotoLable`、`Memu`、`TGUild` 等一律照抄）；
//   - 标记 `接缝：待 <单元名> 移植后接入`，届时由真实类实现这些接口即可，宿主实现体不用改。
//
// 原文未在 PluginInterface.pas 声明、但被 T*Func 记录引用（且 PluginFuncAssign.inc 会赋值）
// 的两个函数指针 `TM2Engine_GetOtherFileDir` / `TBaseObject_TrainSkill` 已在
// PluginInterfaceTypes.g.cs 补齐（签名取自 PluginImplement.pas:1852 / :3197）。
// =====================================================================================

/// <summary>接缝：原文 `_TList = TList`（PluginInterface.pas:45）。接缝：待 Classes 的 C# 对应物移植后接入。</summary>
public interface IListHandle
{
    int Count { get; }
    void Clear();
    void Add(IntPtr item);
    void Insert(int index, IntPtr item);
    void Remove(IntPtr item);
    void Delete(int index);
    IntPtr GetItem(int index);
    void SetItem(int index, IntPtr item);
    int IndexOf(IntPtr item);
    void Exchange(int index1, int index2);
    void CopyTo(IListHandle dest);
}

/// <summary>接缝：原文 `_TStringList = TStringList`（PluginInterface.pas:47）。待 GXX.Core.Util.TStringList 接入。</summary>
public interface IStringListHandle
{
    bool CaseSensitive { get; set; }
    bool Sorted { get; set; }
    bool Duplicates { get; set; }
    int Count { get; }
    string Text { get; set; }
    void Add(string s);
    void AddObject(string s, object? aObject);
    void Insert(int index, string s);
    void InsertObject(int index, string s, object? aObject);
    void Remove(string s);
    void Delete(int index);
    string GetItem(int index);
    void SetItem(int index, string s);
    object? GetObject(int index);
    void SetObject(int index, object? aObject);
    int IndexOf(string s);
    int IndexOfObject(object? aObject);
    bool Find(string s, ref int index);
    void Exchange(int index1, int index2);
    void LoadFromFile(string fileName);
    void SaveToFile(string fileName);
    void CopyTo(IStringListHandle dest);
}

/// <summary>接缝：原文 `_TMemoryStream = TMemoryStream`（PluginInterface.pas:51）。</summary>
public interface IMemoryStreamHandle
{
    long Size { get; }
    void SetSize(int newSize);
    void Clear();
    int Read(byte[] buffer, int count);
    int Write(byte[] buffer, int count);
    int Seek(int offset, ushort origin);
    IntPtr Memory { get; }
    long Position { get; set; }
    void LoadFromFile(string fileName);
    void SaveToFile(string fileName);
}

/// <summary>接缝：原文 `_TMenuItem = TMenuItem`（PluginInterface.pas:53）。接缝：待菜单层（WinForms ToolStripMenuItem）移植后接入。</summary>
public interface IMenuItem
{
    int Count { get; }
    IMenuItem GetItems(int index);
    string Caption { get; set; }
    bool Enabled { get; set; }
    bool Visable { get; set; }
    bool Checked { get; set; }
    bool RadioItem { get; set; }
    int GroupIndex { get; set; }
    int Tag { get; set; }
}

/// <summary>接缝：原文 `_TIniFile = TIniFile`（PluginInterface.pas:55）。接缝：待 FastIniFile/TIniFile 语义层接入。</summary>
public interface IIniFileHandle
{
    bool SectionExists(string section);
    bool ValueExists(string section, string ident);
    /// <summary>原文 `TIniFile.ReadString` 的返回值（宿主实现体 :1532 直接取它，故接缝按"取字符串"表达）。</summary>
    string ReadString(string section, string ident, string @default);
    void WriteString(string section, string ident, string value);
    int ReadInteger(string section, string ident, int @default);
    void WriteInteger(string section, string ident, int value);
    int ReadBool(string section, string ident, int @default);
    void WriteBool(string section, string ident, int value);
}

/// <summary>接缝：原文 `_TMagicACList = TMagicACList`（PluginInterface.pas:57），uMagicACUtils 单元。</summary>
public interface IMagicACListHandle
{
    int Count { get; }
    IntPtr GetItem(int index);
    IntPtr FindByMagIdx(int magIdx);
}

/// <summary>
/// 接缝：原文 `_TEnvirnoment = TEnvirnoment`（PluginInterface.pas:59），Envir 单元。
/// 成员名 = 宿主实现体访问的字段/方法原名。
/// </summary>
public interface IEnvirnoment
{
    /// <summary>原文 `sMapName`（宿主实现体 :1595）。</summary>
    string sMapName { get; }
    /// <summary>原文 `sMapDesc`（:1611）。</summary>
    string sMapDesc { get; }
    /// <summary>原文 `m_nWidth`（:1624）。</summary>
    int m_nWidth { get; }
    /// <summary>原文 `m_nHeight`（:1630）。</summary>
    int m_nHeight { get; }
    /// <summary>原文 `nMinMap`（:1636）。</summary>
    int nMinMap { get; }
    /// <summary>原文 `m_boMainMap`（:1642）。</summary>
    bool m_boMainMap { get; }
    /// <summary>原文 `sMainMapName`（:1651）。</summary>
    string sMainMapName { get; }
    /// <summary>原文 `m_boMirror`（:1664）。</summary>
    bool m_boMirror { get; }
    /// <summary>原文 `m_dwMirrorCreateTick`（:1670）。</summary>
    uint m_dwMirrorCreateTick { get; }
    /// <summary>原文 `m_dwMirrorSurvivalTime`（:1676）。</summary>
    uint m_dwMirrorSurvivalTime { get; }
    /// <summary>原文 `m_sMirrorExitToMap`（:1685）。</summary>
    string m_sMirrorExitToMap { get; }
    /// <summary>原文 `m_nMirrorMinMap`（:1698）。</summary>
    int m_nMirrorMinMap { get; }
    /// <summary>原文 `m_boAlwaysShowTime`（:1704）。</summary>
    bool m_boAlwaysShowTime { get; }
    /// <summary>原文 `m_boFB`（:1710）。</summary>
    bool m_boFB { get; }
    /// <summary>原文 `m_sFBName`（:1719）。</summary>
    string m_sFBName { get; }
    /// <summary>原文 `m_FBEnterLimit`（:1732，枚举取值 0..3）。</summary>
    int m_FBEnterLimit { get; }
    /// <summary>原文 `m_boFBCreate`（:1738）。</summary>
    bool m_boFBCreate { get; }
    /// <summary>原文 `m_dwFBCreateTime`（:1744）。</summary>
    uint m_dwFBCreateTime { get; }
    /// <summary>原文 `Envir.CanWalk(nX, nY, boFlag)`（:1765）。</summary>
    bool CanWalk(int nX, int nY, bool boFlag);
    /// <summary>原文 `Envir.IsValidObject(nX, nY, nRange, AObject)`（:1771）。</summary>
    bool IsValidObject(int nX, int nY, int nRange, object aObject);
    /// <summary>原文 `Envir.GeTItemObjects(nX, nY, ObjectList)`（:1777，原文大小写如此）。</summary>
    int GeTItemObjects(int nX, int nY, IListHandle objectList);
    /// <summary>原文 `Envir.GeTBaseObjects(nX, nY, IncDeathObject, ObjectList)`（:1783）。</summary>
    int GeTBaseObjects(int nX, int nY, bool incDeathObject, IListHandle objectList);
    /// <summary>原文 `Envir.GetPlayObjects(nX, nY, IncDeathObject, ObjectList)`（:1789）。</summary>
    int GetPlayObjects(int nX, int nY, bool incDeathObject, IListHandle objectList);
}

/// <summary>
/// 接缝：原文 `_TBaseObject = TBaseObject`（PluginInterface.pas:61），ObjBase 单元（TCreature）。
/// 成员名 = 宿主实现体访问的 `m_*` 字段原名；`M_WAbilNewValue/SetM_WAbilNewValue` 是
/// 原文 `m_WAbil.NewValue[i]`（packed 变体数组字段）的托管访问器。
/// </summary>
public interface IBaseObjectHandle
{
    string m_sCharName { get; set; }
    byte m_btGender { get; }
    byte m_btJob { get; }
    byte m_btHair { get; }
    string m_sHomeMap { get; }
    int m_nHomeX { get; }
    int m_nHomeY { get; }
    byte m_btPermission { get; set; }
    bool m_boDeath { get; }
    uint m_dwDeathTick { get; }
    bool m_boGhost { get; }
    uint m_dwGhostTick { get; }
    byte m_btRaceServer { get; }
    ushort m_wAppr { get; }
    byte m_btRaceImg { get; }
    int m_nCharStatus { get; set; }
    int m_nHungerStatus { get; set; }
    bool m_boISNGMonster { get; }
    bool m_boDummyObject { get; }
    int m_nViewRange { get; set; }
    /// <summary>原文 `m_Abil: TAbility`（宿主实现体 :2418 整体赋值）。</summary>
    TAbility M_Abil { get; set; }
    /// <summary>原文 `m_WAbil: TAbility`（宿主实现体 :2424 / :2429）。</summary>
    TAbility M_WAbil { get; set; }
    uint M_WAbilNewValue(int index);
    void SetM_WAbilNewValue(int index, uint value);
    IBaseObjectHandle? m_Master { get; }
    IBaseObjectHandle? GetMasterEx();
    bool m_boSuperMan { get; set; }
    bool m_boAdminMode { get; set; }
    bool m_boTransparent { get; set; }
    bool m_boObMode { get; set; }
    bool m_boStoneMode { get; set; }
    bool m_boStickMode { get; set; }
    bool m_boAnimal { get; set; }
    bool m_boNoItem { get; set; }
    bool m_boCoolEye { get; set; }
    bool m_boHideMode { get; set; }
    bool m_boParalysis { get; set; }
    uint m_dwParalysisRate { get; set; }
    bool m_boMDParalysis { get; set; }
    uint m_dwMDParalysisRate { get; set; }
    bool m_boFrozen { get; set; }
    uint m_dwFrozenRate { get; set; }
    bool m_boCobwebWinding { get; set; }
    uint m_dwCobwebWindingRate { get; set; }
    IBaseObjectHandle? m_TargetCret { get; set; }
    IBaseObjectHandle? m_LastHiter { get; }
    IBaseObjectHandle? m_ExpHitter { get; }
    IBaseObjectHandle? m_PoisonHiter { get; }
    IBaseObjectHandle? m_PoseCreate { get; }
    bool IsProperTarget(IBaseObjectHandle? target);
    bool IsProperFriend(IBaseObjectHandle? target);
    bool TargetInRange(IBaseObjectHandle? target, int nX, int nY, int nRange);
    bool InSafeZone { get; }
    uint GetLevelExp(int nLevel);
}

/// <summary>接缝：原文 `_TSmartObject = TSmartObject`（PluginInterface.pas:63）。</summary>
public interface ISmartObjectHandle : IBaseObjectHandle
{
}

/// <summary>接缝：原文 `_TPlayObject = TPlayObject`（PluginInterface.pas:65），ObjPlayer 单元。</summary>
public interface IPlayObjectHandle : ISmartObjectHandle
{
}

/// <summary>接缝：原文 `_TDummyObject = TDummyObject`（PluginInterface.pas:67），ObjDummy 单元。</summary>
public interface IDummyObjectHandle : IPlayObjectHandle
{
}

/// <summary>接缝：原文 `_THeroObject = THeroObject`（PluginInterface.pas:69），ObjHero 单元。</summary>
public interface IHeroObjectHandle : ISmartObjectHandle
{
}

/// <summary>接缝：原文 `_TNormNpc = TNormNpc`（PluginInterface.pas:71），ObjNpc 单元。</summary>
public interface INormNpcHandle : IBaseObjectHandle
{
}

/// <summary>接缝：原文 `_TGuild = TGuild`（PluginInterface.pas:73；PluginImplement 里写作 `TGUild`，原文如此）。</summary>
public interface IGuildHandle
{
}

// -------------------------------------------------------------------------------------
// 以下四个记录在 PluginInterface.pas 里以指针出现，但**不在本单元声明**；
// 本车道不移植其所属单元，故按 Grobal2.pas 的真实布局定义最小接缝（字段名/顺序照抄），
// 标记 `接缝：待 Grobal2.pas 对应记录移植后替换`。
// -------------------------------------------------------------------------------------

/// <summary>接缝：原文 `pTUserMagic`（PluginInterface.pas:1115），Grobal2.pas 的 TUserMagic。</summary>
public struct TUserMagic
{
    /// <summary>原文 `wMagIdx: Word`。</summary>
    public ushort wMagIdx;
    /// <summary>原文 `btLevel: Byte`。</summary>
    public byte btLevel;
    /// <summary>原文 `btKey: Byte`。</summary>
    public byte btKey;
    /// <summary>原文 `nTranPoint: Integer`。</summary>
    public int nTranPoint;
    /// <summary>原文 `boRegistered: BOOL`。</summary>
    public int boRegistered;
}

/// <summary>接缝：原文 `pTMagic`（PluginInterface.pas:2141），Magic/M2Definition 的 TMagic。</summary>
public struct TMagic
{
    /// <summary>原文 `wMagicId: Word`。</summary>
    public ushort wMagicId;
    /// <summary>原文 `szName: string[20]`（本接缝以 IntPtr 占位，待移植后换 fixed byte[21]）。</summary>
    public IntPtr pszNameSeam;
}

/// <summary>接缝：原文注释中的 `pTDynamicVar`（PluginInterface.pas:1701），Grobal2.pas 的动态变量记录。</summary>
public struct TDynamicVar
{
    /// <summary>原文 `nType: Integer`。</summary>
    public int nType;
    /// <summary>原文 `sValue: string` / `nValue: Integer` 的联合体（本接缝以 IntPtr 占位）。</summary>
    public IntPtr pValueSeam;
}

/// <summary>接缝：原文注释中的 `pTMasterRankInfo`（PluginInterface.pas:1728），Grobal2.pas 的师徒排名记录。</summary>
public struct TMasterRankInfo
{
    /// <summary>原文 `sMasterName: string[20]`（本接缝以 IntPtr 占位）。</summary>
    public IntPtr pMasterNameSeam;
    /// <summary>原文 `nPoint: Integer`。</summary>
    public int nPoint;
}
