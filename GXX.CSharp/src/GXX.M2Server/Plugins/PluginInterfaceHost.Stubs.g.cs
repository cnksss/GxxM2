using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Plugins;

// =====================================================================================
// PluginImplement.pas 1:1 转换的**未完成壳**（插件宿主回调表）。
//
// 本文件里的方法名与原文例程名一一对应（`_TFoo_Bar` → `TFoo_Bar`），签名与
// PluginInterface.pas 的 procedural type 逐字一致；方法体是显式 NotImplementedException，
// 每个壳都标了原文行号，待对应引擎单元（ObjBase/ObjPlayer/Envir/Guild/UsrEngn 等）
// 移植后逐条换成真实实现。
//
// 已完成的那部分（内存/列表/字符串列表/内存流/菜单/INI/引擎薄封装等）在
// PluginInterfaceHost.cs 里，逐条对照原行号。
//
// 未完成清单与进度见 docs/并行报告-p2b-m2-plugins.md（§6）。
// =====================================================================================
public sealed partial class PluginInterfaceHost
{
    /// <summary>原文 `TStrLit_LoadFromFile = procedure(Strings: _TStringList; FileName: PAnsiChar); stdcall;`</summary>
    public void TStrLit_LoadFromFile(IStringListHandle Strings, byte[] FileName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TStrLit_LoadFromFile 待 PluginImplement.pas:255 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TStrLit_SaveToFile = procedure(Strings: _TStringList; FileName: PAnsiChar); stdcall;`</summary>
    public void TStrLit_SaveToFile(IStringListHandle Strings, byte[] FileName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TStrLit_SaveToFile 待 PluginImplement.pas:258 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TMagicACList_Count = function(List: _TMagicACList): Integer; stdcall;`</summary>
    public int TMagicACList_Count(IMagicACListHandle pList)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TMagicACList_Count 待 PluginImplement.pas:434 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TMagicACList_GetItem = function(List: _TMagicACList; Index: Integer): PMagicACInfo; stdcall;`</summary>
    public IntPtr TMagicACList_GetItem(IMagicACListHandle pList, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TMagicACList_GetItem 待 PluginImplement.pas:437 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TMagicACList_FindByMagIdx = function(List: _TMagicACList; MagIdx: Integer): PMagicACInfo; stdcall;`</summary>
    public IntPtr TMagicACList_FindByMagIdx(IMagicACListHandle pList, int MagIdx)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TMagicACList_FindByMagIdx 待 PluginImplement.pas:440 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetChrName = function(BaseObject: _TBaseObject; NewName: PAnsiChar): BOOL; stdcall;`</summary>
    public int TBaseObject_SetChrName(IBaseObjectHandle BaseObject, byte[] NewName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetChrName 待 PluginImplement.pas:636 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RefShowName = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_RefShowName(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RefShowName 待 PluginImplement.pas:639 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RefNameColor = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_RefNameColor(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RefNameColor 待 PluginImplement.pas:642 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetGender = function(BaseObject: _TBaseObject; Gender: Byte): BOOL; stdcall;`</summary>
    public int TBaseObject_SetGender(IBaseObjectHandle BaseObject, byte Gender)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetGender 待 PluginImplement.pas:648 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetJob = function(BaseObject: _TBaseObject; Job: Byte): BOOL; stdcall;`</summary>
    public int TBaseObject_SetJob(IBaseObjectHandle BaseObject, byte Job)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetJob 待 PluginImplement.pas:654 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetHair = procedure(BaseObject: _TBaseObject; Hair: Byte); stdcall;`</summary>
    public void TBaseObject_SetHair(IBaseObjectHandle BaseObject, byte Hair)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetHair 待 PluginImplement.pas:660 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetEnvir = function(BaseObject: _TBaseObject): _TEnvirnoment; stdcall;`</summary>
    public IEnvirnoment TBaseObject_GetEnvir(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetEnvir 待 PluginImplement.pas:663 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetMapName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TBaseObject_GetMapName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetMapName 待 PluginImplement.pas:666 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetCurrX = function(BaseObject: _TBaseObject): Integer; stdcall;`</summary>
    public int TBaseObject_GetCurrX(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetCurrX 待 PluginImplement.pas:669 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetCurrY = function(BaseObject: _TBaseObject): Integer; stdcall;`</summary>
    public int TBaseObject_GetCurrY(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetCurrY 待 PluginImplement.pas:672 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetDirection = function(BaseObject: _TBaseObject): Byte; stdcall;`</summary>
    public byte TBaseObject_GetDirection(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetDirection 待 PluginImplement.pas:675 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_MakeGhost = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_MakeGhost(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_MakeGhost 待 PluginImplement.pas:705 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_ReAlive = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_ReAlive(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_ReAlive 待 PluginImplement.pas:708 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_StatusChanged = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_StatusChanged(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_StatusChanged 待 PluginImplement.pas:726 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetSlaveList = function(BaseObject: _TBaseObject): _TList; stdcall;`</summary>
    public IListHandle TBaseObject_GetSlaveList(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetSlaveList 待 PluginImplement.pas:756 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetHitPoint = function(BaseObject: _TBaseObject): Word; stdcall;`</summary>
    public ushort TBaseObject_GetHitPoint(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetHitPoint 待 PluginImplement.pas:819 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetHitPoint = procedure(BaseObject: _TBaseObject; Value: Word); stdcall;`</summary>
    public void TBaseObject_SetHitPoint(IBaseObjectHandle BaseObject, ushort pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetHitPoint 待 PluginImplement.pas:822 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetSpeedPoint = function(BaseObject: _TBaseObject): Word; stdcall;`</summary>
    public ushort TBaseObject_GetSpeedPoint(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetSpeedPoint 待 PluginImplement.pas:825 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetSpeedPoint = procedure(BaseObject: _TBaseObject; Value: Word); stdcall;`</summary>
    public void TBaseObject_SetSpeedPoint(IBaseObjectHandle BaseObject, ushort pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetSpeedPoint 待 PluginImplement.pas:828 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetHitSpeed = function(BaseObject: _TBaseObject): ShortInt; stdcall;`</summary>
    public sbyte TBaseObject_GetHitSpeed(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetHitSpeed 待 PluginImplement.pas:831 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetHitSpeed = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`</summary>
    public void TBaseObject_SetHitSpeed(IBaseObjectHandle BaseObject, sbyte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetHitSpeed 待 PluginImplement.pas:834 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetWalkSpeed = function(BaseObject: _TBaseObject): Integer; stdcall;`</summary>
    public int TBaseObject_GetWalkSpeed(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetWalkSpeed 待 PluginImplement.pas:837 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetWalkSpeed = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`</summary>
    public void TBaseObject_SetWalkSpeed(IBaseObjectHandle BaseObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetWalkSpeed 待 PluginImplement.pas:840 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetHPRecover = function(BaseObject: _TBaseObject): ShortInt; stdcall;`</summary>
    public sbyte TBaseObject_GetHPRecover(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetHPRecover 待 PluginImplement.pas:843 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetHPRecover = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`</summary>
    public void TBaseObject_SetHPRecover(IBaseObjectHandle BaseObject, sbyte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetHPRecover 待 PluginImplement.pas:846 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetMPRecover = function(BaseObject: _TBaseObject): ShortInt; stdcall;`</summary>
    public sbyte TBaseObject_GetMPRecover(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetMPRecover 待 PluginImplement.pas:849 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetMPRecover = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`</summary>
    public void TBaseObject_SetMPRecover(IBaseObjectHandle BaseObject, sbyte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetMPRecover 待 PluginImplement.pas:852 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetPoisonRecover = function(BaseObject: _TBaseObject): ShortInt; stdcall;`</summary>
    public sbyte TBaseObject_GetPoisonRecover(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetPoisonRecover 待 PluginImplement.pas:855 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetPoisonRecover = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`</summary>
    public void TBaseObject_SetPoisonRecover(IBaseObjectHandle BaseObject, sbyte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetPoisonRecover 待 PluginImplement.pas:858 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetAntiPoison = function(BaseObject: _TBaseObject): Byte; stdcall;`</summary>
    public byte TBaseObject_GetAntiPoison(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetAntiPoison 待 PluginImplement.pas:861 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetAntiPoison = procedure(BaseObject: _TBaseObject; Value: Byte); stdcall;`</summary>
    public void TBaseObject_SetAntiPoison(IBaseObjectHandle BaseObject, byte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetAntiPoison 待 PluginImplement.pas:864 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetAntiMagic = function(BaseObject: _TBaseObject): ShortInt; stdcall;`</summary>
    public sbyte TBaseObject_GetAntiMagic(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetAntiMagic 待 PluginImplement.pas:867 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetAntiMagic = procedure(BaseObject: _TBaseObject; Value: ShortInt); stdcall;`</summary>
    public void TBaseObject_SetAntiMagic(IBaseObjectHandle BaseObject, sbyte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetAntiMagic 待 PluginImplement.pas:870 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetLuck = function(BaseObject: _TBaseObject): Integer; stdcall;`</summary>
    public int TBaseObject_GetLuck(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetLuck 待 PluginImplement.pas:873 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetLuck = procedure(BaseObject: _TBaseObject; Value: Integer); stdcall;`</summary>
    public void TBaseObject_SetLuck(IBaseObjectHandle BaseObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetLuck 待 PluginImplement.pas:876 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetAttatckMode = function(BaseObject: _TBaseObject): Byte; stdcall;`</summary>
    public byte TBaseObject_GetAttatckMode(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetAttatckMode 待 PluginImplement.pas:879 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetAttatckMode = procedure(BaseObject: _TBaseObject; Value: Byte); stdcall;`</summary>
    public void TBaseObject_SetAttatckMode(IBaseObjectHandle BaseObject, byte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetAttatckMode 待 PluginImplement.pas:882 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetNation = function(BaseObject: _TBaseObject): Byte; stdcall;`</summary>
    public byte TBaseObject_GetNation(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetNation 待 PluginImplement.pas:885 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetNation = function(BaseObject: _TBaseObject; Nation: Byte): BOOL; stdcall;`</summary>
    public int TBaseObject_SetNation(IBaseObjectHandle BaseObject, byte Nation)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetNation 待 PluginImplement.pas:888 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetNationaName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TBaseObject_GetNationaName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetNationaName 待 PluginImplement.pas:891 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetGuild = function(BaseObject: _TBaseObject): _TGuild; stdcall;`</summary>
    public IGuildHandle TBaseObject_GetGuild(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetGuild 待 PluginImplement.pas:894 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseobject_GetGuildRankNo = function(BaseObject: _TBaseObject): Integer; stdcall;`</summary>
    public int TBaseobject_GetGuildRankNo(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseobject_GetGuildRankNo 待 PluginImplement.pas:897 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseobject_GetGuildRankName = function(BaseObject: _TBaseObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TBaseobject_GetGuildRankName(IBaseObjectHandle BaseObject, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseobject_GetGuildRankName 待 PluginImplement.pas:900 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_IsGuildMaster = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_IsGuildMaster(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_IsGuildMaster 待 PluginImplement.pas:903 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnParalysis = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnParalysis(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnParalysis 待 PluginImplement.pas:956 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnMagicShield = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnMagicShield(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnMagicShield 待 PluginImplement.pas:964 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnRevival = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnRevival(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnRevival 待 PluginImplement.pas:972 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnPosion = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnPosion(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnPosion 待 PluginImplement.pas:980 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnTamming = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnTamming(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnTamming 待 PluginImplement.pas:988 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnFireCross = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnFireCross(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnFireCross 待 PluginImplement.pas:996 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnFrozen = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnFrozen(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnFrozen 待 PluginImplement.pas:1004 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetIsUnCobwebWinding = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_GetIsUnCobwebWinding(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetIsUnCobwebWinding 待 PluginImplement.pas:1012 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SendMsg = procedure(BaseObject, Target: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar); stdcall;`</summary>
    public void TBaseObject_SendMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SendMsg 待 PluginImplement.pas:1045 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SendDelayMsg = procedure(BaseObject, Target: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar; dwDelay: DWORD); stdcall;`</summary>
    public void TBaseObject_SendDelayMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg, uint dwDelay)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SendDelayMsg 待 PluginImplement.pas:1049 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SendRefMsg = procedure(BaseObject: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar; dwDelay: DWORD); stdcall;`</summary>
    public void TBaseObject_SendRefMsg(IBaseObjectHandle BaseObject, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg, uint dwDelay)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SendRefMsg 待 PluginImplement.pas:1053 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SendUpdateMsg = procedure(BaseObject, Target: _TBaseObject; wIdent, wParam: Integer; nParam1, nParam2, nParam3: NativeInt; sMsg: PAnsiChar); stdcall;`</summary>
    public void TBaseObject_SendUpdateMsg(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int wIdent, int wParam, IntPtr nParam1, IntPtr nParam2, IntPtr nParam3, byte[] sMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SendUpdateMsg 待 PluginImplement.pas:1057 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SysMsg = function(BaseObject: _TBaseObject; sMsg: PAnsiChar; FColor, BColor: Byte; MsgType: Integer) : BOOL; stdcall;`</summary>
    public int TBaseObject_SysMsg(IBaseObjectHandle BaseObject, byte[] sMsg, byte FColor, byte BColor, int MsgType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SysMsg 待 PluginImplement.pas:1061 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetBagItemList = function(BaseObject: _TBaseObject): _TList; stdcall;`</summary>
    public IListHandle TBaseObject_GetBagItemList(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetBagItemList 待 PluginImplement.pas:1065 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_IsEnoughBag = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_IsEnoughBag(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_IsEnoughBag 待 PluginImplement.pas:1068 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_IsEnoughBagEx = function(BaseObject: _TBaseObject; AddCount: Integer): BOOL; stdcall;`</summary>
    public int TBaseObject_IsEnoughBagEx(IBaseObjectHandle BaseObject, int AddCount)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_IsEnoughBagEx 待 PluginImplement.pas:1071 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_AddItemToBag = function(BaseObject: _TBaseObject; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TBaseObject_AddItemToBag(IBaseObjectHandle BaseObject, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_AddItemToBag 待 PluginImplement.pas:1074 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_DelBagItemByIndex = function(BaseObject: _TBaseObject; Index: Integer): BOOL; stdcall;`</summary>
    public int TBaseObject_DelBagItemByIndex(IBaseObjectHandle BaseObject, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_DelBagItemByIndex 待 PluginImplement.pas:1077 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_DelBagItemByMakeIdx = function(BaseObject: _TBaseObject; MakeIndex: Integer; ItemName: PAnsiChar): BOOL; stdcall;`</summary>
    public int TBaseObject_DelBagItemByMakeIdx(IBaseObjectHandle BaseObject, int MakeIndex, byte[] ItemName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_DelBagItemByMakeIdx 待 PluginImplement.pas:1080 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_DelBagItemByUserItem = function(BaseObject: _TBaseObject; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TBaseObject_DelBagItemByUserItem(IBaseObjectHandle BaseObject, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_DelBagItemByUserItem 待 PluginImplement.pas:1083 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_IsPtInSafeZone = function(BaseObject: _TBaseObject; Envir: _TEnvirnoment; nX, nY: Integer): BOOL; stdcall;`</summary>
    public int TBaseObject_IsPtInSafeZone(IBaseObjectHandle BaseObject, IEnvirnoment Envir, int nX, int nY)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_IsPtInSafeZone 待 PluginImplement.pas:1089 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RecalcLevelAbil = procedure(BaseObject: _TBaseObject; IsSysDef: BOOL); stdcall;`</summary>
    public void TBaseObject_RecalcLevelAbil(IBaseObjectHandle BaseObject, int IsSysDef)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RecalcLevelAbil 待 PluginImplement.pas:1092 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RecalcAbil = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_RecalcAbil(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RecalcAbil 待 PluginImplement.pas:1095 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RecalcBagWeight = function(BaseObject: _TBaseObject): Integer; stdcall;`</summary>
    public int TBaseObject_RecalcBagWeight(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RecalcBagWeight 待 PluginImplement.pas:1098 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_HasLevelUp = procedure(BaseObject: _TBaseObject; nLevel: Integer); stdcall;`</summary>
    public void TBaseObject_HasLevelUp(IBaseObjectHandle BaseObject, int nLevel)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_HasLevelUp 待 PluginImplement.pas:1104 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_CheckMagicLevelup = function(BaseObject: _TBaseObject; UserMagic: pTUserMagic): BOOL; stdcall;`</summary>
    public int TBaseObject_CheckMagicLevelup(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_CheckMagicLevelup 待 PluginImplement.pas:1119 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_MagicTranPointChanged = procedure(BaseObject: _TBaseObject; UserMagic: pTUserMagic); stdcall;`</summary>
    public void TBaseObject_MagicTranPointChanged(IBaseObjectHandle BaseObject, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_MagicTranPointChanged 待 PluginImplement.pas:1122 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_DamageHealth = procedure(BaseObject: _TBaseObject; nDamage: Integer; StruckFrom: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_DamageHealth(IBaseObjectHandle BaseObject, int nDamage, IBaseObjectHandle StruckFrom)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_DamageHealth 待 PluginImplement.pas:1125 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_DamageSpell = procedure(BaseObject: _TBaseObject; nSpellPoint: Integer); stdcall;`</summary>
    public void TBaseObject_DamageSpell(IBaseObjectHandle BaseObject, int nSpellPoint)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_DamageSpell 待 PluginImplement.pas:1128 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_IncHealthSpell = procedure(BaseObject: _TBaseObject; nHP, nMP: Integer; SendChangedToClient: BOOL); stdcall;`</summary>
    public void TBaseObject_IncHealthSpell(IBaseObjectHandle BaseObject, int nHP, int nMP, int SendChangedToClient)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_IncHealthSpell 待 PluginImplement.pas:1131 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_HealthSpellChanged = procedure(BaseObject: _TBaseObject; dwDelay: DWORD); stdcall;`</summary>
    public void TBaseObject_HealthSpellChanged(IBaseObjectHandle BaseObject, uint dwDelay)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_HealthSpellChanged 待 PluginImplement.pas:1134 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_FeatureChanged = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_FeatureChanged(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_FeatureChanged 待 PluginImplement.pas:1137 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_WeightChanged = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_WeightChanged(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_WeightChanged 待 PluginImplement.pas:1140 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetHitStruckDamage = function(BaseObject: _TBaseObject; Target: _TBaseObject; nDamage: Integer; MagicACInfo: PMagicACInfo; nType: Integer): Integer; stdcall;`</summary>
    public int TBaseObject_GetHitStruckDamage(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nDamage, IntPtr MagicACInfo, int nType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetHitStruckDamage 待 PluginImplement.pas:1144 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetMagStruckDamage = function(BaseObject: _TBaseObject; Target: _TBaseObject; nDamage: Integer): Integer; stdcall;`</summary>
    public int TBaseObject_GetMagStruckDamage(IBaseObjectHandle BaseObject, IBaseObjectHandle Target, int nDamage)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetMagStruckDamage 待 PluginImplement.pas:1148 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_GetActorIcon = function(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;`</summary>
    public int TBaseObject_GetActorIcon(IBaseObjectHandle BaseObject, int pIndex, ref TActorIcon ActorIcon)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_GetActorIcon 待 PluginImplement.pas:1151 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SetActorIcon = function(BaseObject: _TBaseObject; Index: Integer; ActorIcon: pTActorIcon): BOOL; stdcall;`</summary>
    public int TBaseObject_SetActorIcon(IBaseObjectHandle BaseObject, int pIndex, ref TActorIcon ActorIcon)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SetActorIcon 待 PluginImplement.pas:1154 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RefUseIcons = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_RefUseIcons(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RefUseIcons 待 PluginImplement.pas:1157 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RefUseEffects = procedure(BaseObject: _TBaseObject); stdcall;`</summary>
    public void TBaseObject_RefUseEffects(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RefUseEffects 待 PluginImplement.pas:1160 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_SpaceMove = procedure(BaseObject: _TBaseObject; sMapName: PAnsiChar; nX, nY: Integer; nInt: Integer); stdcall;`</summary>
    public void TBaseObject_SpaceMove(IBaseObjectHandle BaseObject, byte[] sMapName, int nX, int nY, int nInt)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_SpaceMove 待 PluginImplement.pas:1163 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_MapRandomMove = procedure(BaseObject: _TBaseObject; sMapName: PAnsiChar; nInt: Integer); stdcall;`</summary>
    public void TBaseObject_MapRandomMove(IBaseObjectHandle BaseObject, byte[] sMapName, int nInt)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_MapRandomMove 待 PluginImplement.pas:1166 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_CanMove = function(BaseObject: _TBaseObject): BOOL; stdcall;`</summary>
    public int TBaseObject_CanMove(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_CanMove 待 PluginImplement.pas:1169 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_CanRun = function(BaseObject: _TBaseObject; nCurrX, nCurrY, nX, nY: Integer): BOOL; stdcall;`</summary>
    public int TBaseObject_CanRun(IBaseObjectHandle BaseObject, int nCurrX, int nCurrY, int nX, int nY)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_CanRun 待 PluginImplement.pas:1172 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_TurnTo = procedure(BaseObject: _TBaseObject; btDir: Byte); stdcall;`</summary>
    public void TBaseObject_TurnTo(IBaseObjectHandle BaseObject, byte btDir)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_TurnTo 待 PluginImplement.pas:1175 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_WalkTo = function(BaseObject: _TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;`</summary>
    public int TBaseObject_WalkTo(IBaseObjectHandle BaseObject, byte btDir, int boFlag)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_WalkTo 待 PluginImplement.pas:1178 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_RunTo = function(BaseObject: _TBaseObject; btDir: Byte; boFlag: BOOL): BOOL; stdcall;`</summary>
    public int TBaseObject_RunTo(IBaseObjectHandle BaseObject, byte btDir, int boFlag)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_RunTo 待 PluginImplement.pas:1181 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TBaseObject_PluginList = function(BaseObject: _TBaseObject): _TList; stdcall;`</summary>
    public IListHandle TBaseObject_PluginList(IBaseObjectHandle BaseObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TBaseObject_PluginList 待 PluginImplement.pas:1184 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetMagicList = function(SmartObject: _TSmartObject): _TList; stdcall;`</summary>
    public IListHandle TSmartObject_GetMagicList(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetMagicList 待 PluginImplement.pas:1192 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetUseItem = function(SmartObject: _TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TSmartObject_GetUseItem(ISmartObjectHandle SmartObject, int pIndex, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetUseItem 待 PluginImplement.pas:1195 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetJewelryBoxStatus = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetJewelryBoxStatus(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetJewelryBoxStatus 待 PluginImplement.pas:1198 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetJewelryBoxStatus = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_SetJewelryBoxStatus(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetJewelryBoxStatus 待 PluginImplement.pas:1201 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetJewelryItem = function(SmartObject: _TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TSmartObject_GetJewelryItem(ISmartObjectHandle SmartObject, int pIndex, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetJewelryItem 待 PluginImplement.pas:1204 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsShowGodBless = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsShowGodBless(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsShowGodBless 待 PluginImplement.pas:1207 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsShowGodBless = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsShowGodBless(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsShowGodBless 待 PluginImplement.pas:1210 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetGodBlessItemsState = function(SmartObject: _TSmartObject; Index: Integer): BOOL; stdcall;`</summary>
    public int TSmartObject_GetGodBlessItemsState(ISmartObjectHandle SmartObject, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetGodBlessItemsState 待 PluginImplement.pas:1213 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetGodBlessItemsState = procedure(SmartObject: _TSmartObject; Index: Integer; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetGodBlessItemsState(ISmartObjectHandle SmartObject, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetGodBlessItemsState 待 PluginImplement.pas:1216 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetGodBlessItem = function(SmartObject: _TSmartObject; Index: Integer; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TSmartObject_GetGodBlessItem(ISmartObjectHandle SmartObject, int pIndex, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetGodBlessItem 待 PluginImplement.pas:1219 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetFengHaoItems = function(SmartObject: _TSmartObject): _TList; stdcall;`</summary>
    public IListHandle TSmartObject_GetFengHaoItems(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetFengHaoItems 待 PluginImplement.pas:1222 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetActiveFengHao = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetActiveFengHao(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetActiveFengHao 待 PluginImplement.pas:1225 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetActiveFengHao = procedure(SmartObject: _TSmartObject; FengHaoIndex: Integer); stdcall;`</summary>
    public void TSmartObject_SetActiveFengHao(ISmartObjectHandle SmartObject, int FengHaoIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetActiveFengHao 待 PluginImplement.pas:1228 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_ActiveFengHaoChanged = procedure(SmartObject: _TSmartObject); stdcall;`</summary>
    public void TSmartObject_ActiveFengHaoChanged(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_ActiveFengHaoChanged 待 PluginImplement.pas:1231 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_DeleteFengHao = procedure(SmartObject: _TSmartObject; Index: Integer); stdcall;`</summary>
    public void TSmartObject_DeleteFengHao(ISmartObjectHandle SmartObject, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_DeleteFengHao 待 PluginImplement.pas:1234 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_ClearFengHao = procedure(SmartObject: _TSmartObject); stdcall;`</summary>
    public void TSmartObject_ClearFengHao(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_ClearFengHao 待 PluginImplement.pas:1237 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetMoveSpeed = function(SmartObject: _TSmartObject): SmallInt; stdcall;`</summary>
    public short TSmartObject_GetMoveSpeed(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetMoveSpeed 待 PluginImplement.pas:1239 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetMoveSpeed = procedure(SmartObject: _TSmartObject; Value: SmallInt); stdcall;`</summary>
    public void TSmartObject_SetMoveSpeed(ISmartObjectHandle SmartObject, short pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetMoveSpeed 待 PluginImplement.pas:1241 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetAttackSpeed = function(SmartObject: _TSmartObject): SmallInt; stdcall;`</summary>
    public short TSmartObject_GetAttackSpeed(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetAttackSpeed 待 PluginImplement.pas:1243 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetAttackSpeed = procedure(SmartObject: _TSmartObject; Value: SmallInt); stdcall;`</summary>
    public void TSmartObject_SetAttackSpeed(ISmartObjectHandle SmartObject, short pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetAttackSpeed 待 PluginImplement.pas:1245 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetSpellSpeed = function(SmartObject: _TSmartObject): SmallInt; stdcall;`</summary>
    public short TSmartObject_GetSpellSpeed(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetSpellSpeed 待 PluginImplement.pas:1247 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetSpellSpeed = procedure(SmartObject: _TSmartObject; Value: SmallInt); stdcall;`</summary>
    public void TSmartObject_SetSpellSpeed(ISmartObjectHandle SmartObject, short pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetSpellSpeed 待 PluginImplement.pas:1249 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_RefGameSpeed = procedure(SmartObject: _TSmartObject); stdcall;`</summary>
    public void TSmartObject_RefGameSpeed(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_RefGameSpeed 待 PluginImplement.pas:1252 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsButch = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsButch(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsButch 待 PluginImplement.pas:1255 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsButch = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsButch(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsButch 待 PluginImplement.pas:1258 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsTrainingNG = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsTrainingNG(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsTrainingNG 待 PluginImplement.pas:1261 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsTrainingNG = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsTrainingNG(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsTrainingNG 待 PluginImplement.pas:1263 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsTrainingXF = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsTrainingXF(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsTrainingXF 待 PluginImplement.pas:1266 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsTrainingXF = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsTrainingXF(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsTrainingXF 待 PluginImplement.pas:1268 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsOpenLastContinuous = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsOpenLastContinuous(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsOpenLastContinuous 待 PluginImplement.pas:1271 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsOpenLastContinuous = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsOpenLastContinuous(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsOpenLastContinuous 待 PluginImplement.pas:1274 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetContinuousMagicOrder = function(SmartObject: _TSmartObject; Index: Integer): Byte; stdcall;`</summary>
    public byte TSmartObject_GetContinuousMagicOrder(ISmartObjectHandle SmartObject, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetContinuousMagicOrder 待 PluginImplement.pas:1277 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetContinuousMagicOrder = procedure(SmartObject: _TSmartObject; Index: Integer; Value: Byte); stdcall;`</summary>
    public void TSmartObject_SetContinuousMagicOrder(ISmartObjectHandle SmartObject, int pIndex, byte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetContinuousMagicOrder 待 PluginImplement.pas:1280 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetPKDieLostExp = function(SmartObject: _TSmartObject): DWORD; stdcall;`</summary>
    public uint TSmartObject_GetPKDieLostExp(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetPKDieLostExp 待 PluginImplement.pas:1283 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetPKDieLostExp = procedure(SmartObject: _TSmartObject; Value: DWORD); stdcall;`</summary>
    public void TSmartObject_SetPKDieLostExp(ISmartObjectHandle SmartObject, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetPKDieLostExp 待 PluginImplement.pas:1285 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetPKDieLostLevel = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetPKDieLostLevel(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetPKDieLostLevel 待 PluginImplement.pas:1288 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetPKDieLostLevel = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_SetPKDieLostLevel(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetPKDieLostLevel 待 PluginImplement.pas:1290 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetPKPoint = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetPKPoint(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetPKPoint 待 PluginImplement.pas:1293 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetPKPoint = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_SetPKPoint(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetPKPoint 待 PluginImplement.pas:1295 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_IncPKPoint = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_IncPKPoint(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_IncPKPoint 待 PluginImplement.pas:1298 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_DecPKPoint = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_DecPKPoint(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_DecPKPoint 待 PluginImplement.pas:1301 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetPKLevel = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetPKLevel(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetPKLevel 待 PluginImplement.pas:1304 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetPKLevel = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_SetPKLevel(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetPKLevel 待 PluginImplement.pas:1306 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsTeleport = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsTeleport(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsTeleport 待 PluginImplement.pas:1309 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsTeleport = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsTeleport(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsTeleport 待 PluginImplement.pas:1311 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsRevival = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsRevival(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsRevival 待 PluginImplement.pas:1314 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsRevival = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsRevival(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsRevival 待 PluginImplement.pas:1316 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetRevivalTime = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetRevivalTime(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetRevivalTime 待 PluginImplement.pas:1319 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetRevivalTime = procedure(SmartObject: _TSmartObject; Value: Integer); stdcall;`</summary>
    public void TSmartObject_SetRevivalTime(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetRevivalTime 待 PluginImplement.pas:1321 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsFlameRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsFlameRing(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsFlameRing 待 PluginImplement.pas:1324 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsFlameRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsFlameRing(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsFlameRing 待 PluginImplement.pas:1326 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsRecoveryRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsRecoveryRing(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsRecoveryRing 待 PluginImplement.pas:1329 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsRecoveryRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsRecoveryRing(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsRecoveryRing 待 PluginImplement.pas:1331 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsMagicShield = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsMagicShield(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsMagicShield 待 PluginImplement.pas:1334 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsMagicShield = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsMagicShield(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsMagicShield 待 PluginImplement.pas:1336 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsMuscleRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsMuscleRing(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsMuscleRing 待 PluginImplement.pas:1339 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsMuscleRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsMuscleRing(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsMuscleRing 待 PluginImplement.pas:1341 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsFastTrain = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsFastTrain(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsFastTrain 待 PluginImplement.pas:1344 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsFastTrain = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsFastTrain(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsFastTrain 待 PluginImplement.pas:1346 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsProbeNecklace = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsProbeNecklace(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsProbeNecklace 待 PluginImplement.pas:1349 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsProbeNecklace = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsProbeNecklace(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsProbeNecklace 待 PluginImplement.pas:1351 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsRecallSuite = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsRecallSuite(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsRecallSuite 待 PluginImplement.pas:1354 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsRecallSuite = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsRecallSuite(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsRecallSuite 待 PluginImplement.pas:1356 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsPirit = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsPirit(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsPirit 待 PluginImplement.pas:1359 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsPirit = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsPirit(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsPirit 待 PluginImplement.pas:1361 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsSupermanItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsSupermanItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsSupermanItem 待 PluginImplement.pas:1364 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsSupermanItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsSupermanItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsSupermanItem 待 PluginImplement.pas:1366 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsExpItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsExpItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsExpItem 待 PluginImplement.pas:1369 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsExpItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsExpItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsExpItem 待 PluginImplement.pas:1371 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetExpItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`</summary>
    public double TSmartObject_GetExpItemValue(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetExpItemValue 待 PluginImplement.pas:1374 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetExpItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`</summary>
    public void TSmartObject_SetExpItemValue(ISmartObjectHandle SmartObject, double pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetExpItemValue 待 PluginImplement.pas:1376 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetExpItemRate = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetExpItemRate(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetExpItemRate 待 PluginImplement.pas:1379 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsPowerItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsPowerItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsPowerItem 待 PluginImplement.pas:1382 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsPowerItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsPowerItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsPowerItem 待 PluginImplement.pas:1384 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetPowerItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`</summary>
    public double TSmartObject_GetPowerItemValue(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetPowerItemValue 待 PluginImplement.pas:1387 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetPowerItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`</summary>
    public void TSmartObject_SetPowerItemValue(ISmartObjectHandle SmartObject, double pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetPowerItemValue 待 PluginImplement.pas:1390 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetPowerItemRate = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_GetPowerItemRate(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetPowerItemRate 待 PluginImplement.pas:1393 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsGuildMove = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsGuildMove(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsGuildMove 待 PluginImplement.pas:1396 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsGuildMove = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsGuildMove(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsGuildMove 待 PluginImplement.pas:1398 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsAngryRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsAngryRing(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsAngryRing 待 PluginImplement.pas:1401 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsAngryRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsAngryRing(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsAngryRing 待 PluginImplement.pas:1403 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsStarRing = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsStarRing(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsStarRing 待 PluginImplement.pas:1406 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsStarRing = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsStarRing(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsStarRing 待 PluginImplement.pas:1408 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsACItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsACItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsACItem 待 PluginImplement.pas:1411 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsACItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsACItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsACItem 待 PluginImplement.pas:1413 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetACItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`</summary>
    public double TSmartObject_GetACItemValue(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetACItemValue 待 PluginImplement.pas:1416 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetACItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`</summary>
    public void TSmartObject_SetACItemValue(ISmartObjectHandle SmartObject, double pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetACItemValue 待 PluginImplement.pas:1418 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsMACItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsMACItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsMACItem 待 PluginImplement.pas:1421 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsMACItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsMACItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsMACItem 待 PluginImplement.pas:1423 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetMACItemValue = function(SmartObject: _TSmartObject): Real; stdcall;`</summary>
    public double TSmartObject_GetMACItemValue(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetMACItemValue 待 PluginImplement.pas:1426 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetMACItemValue = procedure(SmartObject: _TSmartObject; Value: Real); stdcall;`</summary>
    public void TSmartObject_SetMACItemValue(ISmartObjectHandle SmartObject, double pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetMACItemValue 待 PluginImplement.pas:1428 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsNoDropItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsNoDropItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsNoDropItem 待 PluginImplement.pas:1431 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsNoDropItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsNoDropItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsNoDropItem 待 PluginImplement.pas:1433 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetIsNoDropUseItem = function(SmartObject: _TSmartObject): BOOL; stdcall;`</summary>
    public int TSmartObject_GetIsNoDropUseItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetIsNoDropUseItem 待 PluginImplement.pas:1436 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetIsNoDropUseItem = procedure(SmartObject: _TSmartObject; Value: BOOL); stdcall;`</summary>
    public void TSmartObject_SetIsNoDropUseItem(ISmartObjectHandle SmartObject, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetIsNoDropUseItem 待 PluginImplement.pas:1438 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetNGAbility = function(SmartObject: _TSmartObject; AbilityNG: pTAbilityNG): BOOL; stdcall;`</summary>
    public int TSmartObject_GetNGAbility(ISmartObjectHandle SmartObject, ref TAbilityNG AbilityNG)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetNGAbility 待 PluginImplement.pas:1441 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetNGAbility = procedure(SmartObject: _TSmartObject; Value: pTAbilityNG); stdcall;`</summary>
    public void TSmartObject_SetNGAbility(ISmartObjectHandle SmartObject, ref TAbilityNG pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetNGAbility 待 PluginImplement.pas:1443 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_GetAlcohol = function(SmartObject: _TSmartObject; AbilityAlcohol: pTAbilityAlcohol): BOOL; stdcall;`</summary>
    public int TSmartObject_GetAlcohol(ISmartObjectHandle SmartObject, ref TAbilityAlcohol AbilityAlcohol)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_GetAlcohol 待 PluginImplement.pas:1446 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SetAlcohol = procedure(SmartObject: _TSmartObject; Value: pTAbilityAlcohol); stdcall;`</summary>
    public void TSmartObject_SetAlcohol(ISmartObjectHandle SmartObject, ref TAbilityAlcohol pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SetAlcohol 待 PluginImplement.pas:1448 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_RepairAllItem = procedure(SmartObject: _TSmartObject); stdcall;`</summary>
    public void TSmartObject_RepairAllItem(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_RepairAllItem 待 PluginImplement.pas:1451 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_IsAllowUseMagic = function(SmartObject: _TSmartObject; MagicID: Word): BOOL; stdcall;`</summary>
    public int TSmartObject_IsAllowUseMagic(ISmartObjectHandle SmartObject, ushort MagicID)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_IsAllowUseMagic 待 PluginImplement.pas:1454 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_SelectMagic = function(SmartObject: _TSmartObject): Integer; stdcall;`</summary>
    public int TSmartObject_SelectMagic(ISmartObjectHandle SmartObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_SelectMagic 待 PluginImplement.pas:1457 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TSmartObject_AttackTarget = function(SmartObject: _TSmartObject; MagicID: Word; AttackTime: DWORD): BOOL; stdcall;`</summary>
    public int TSmartObject_AttackTarget(ISmartObjectHandle SmartObject, ushort MagicID, uint AttackTime)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TSmartObject_AttackTarget 待 PluginImplement.pas:1460 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetUserID = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetUserID(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetUserID 待 PluginImplement.pas:1468 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIPAddr = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIPAddr(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIPAddr 待 PluginImplement.pas:1471 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIPLocal = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIPLocal(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIPLocal 待 PluginImplement.pas:1474 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMachineID = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetMachineID(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMachineID 待 PluginImplement.pas:1477 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsReadyRun = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsReadyRun(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsReadyRun 待 PluginImplement.pas:1480 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetLogonTime = function(Player: _TPlayObject; LogonTime: PSystemTime): BOOL; stdcall;`</summary>
    public int TPlayObject_GetLogonTime(IPlayObjectHandle pPlayer, IntPtr LogonTime)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetLogonTime 待 PluginImplement.pas:1483 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetSoftVerDate = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetSoftVerDate(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetSoftVerDate 待 PluginImplement.pas:1486 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetClientType = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetClientType(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetClientType 待 PluginImplement.pas:1489 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IsOldClient = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_IsOldClient(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IsOldClient 待 PluginImplement.pas:1492 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetScreenWidth = function(Player: _TPlayObject): Word; stdcall;`</summary>
    public ushort TPlayObject_GetScreenWidth(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetScreenWidth 待 PluginImplement.pas:1495 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetScreenHeight = function(Player: _TPlayObject): Word; stdcall;`</summary>
    public ushort TPlayObject_GetScreenHeight(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetScreenHeight 待 PluginImplement.pas:1498 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetClientViewRange = function(Player: _TPlayObject): Word; stdcall;`</summary>
    public ushort TPlayObject_GetClientViewRange(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetClientViewRange 待 PluginImplement.pas:1501 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetRelevel = function(Player: _TPlayObject): Byte; stdcall;`</summary>
    public byte TPlayObject_GetRelevel(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetRelevel 待 PluginImplement.pas:1504 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetRelevel = procedure(Player: _TPlayObject; Value: Byte); stdcall;`</summary>
    public void TPlayObject_SetRelevel(IPlayObjectHandle pPlayer, byte pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetRelevel 待 PluginImplement.pas:1506 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetBonusPoint = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetBonusPoint(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetBonusPoint 待 PluginImplement.pas:1509 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetBonusPoint = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetBonusPoint(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetBonusPoint 待 PluginImplement.pas:1511 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendAdjustBonus = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendAdjustBonus(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendAdjustBonus 待 PluginImplement.pas:1514 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetHeroName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetHeroName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetHeroName 待 PluginImplement.pas:1517 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetDeputyHeroName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetDeputyHeroName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetDeputyHeroName 待 PluginImplement.pas:1520 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetDeputyHeroJob = function(Player: _TPlayObject): Byte; stdcall;`</summary>
    public byte TPlayObject_GetDeputyHeroJob(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetDeputyHeroJob 待 PluginImplement.pas:1523 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMyHero = function(Player: _TPlayObject): _THeroObject; stdcall;`</summary>
    public IHeroObjectHandle TPlayObject_GetMyHero(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMyHero 待 PluginImplement.pas:1526 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetFixedHero = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetFixedHero(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetFixedHero 待 PluginImplement.pas:1529 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_ClientHeroLogOn = procedure(Player: _TPlayObject; IsDeputyHero: BOOL); stdcall;`</summary>
    public void TPlayObject_ClientHeroLogOn(IPlayObjectHandle pPlayer, int IsDeputyHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_ClientHeroLogOn 待 PluginImplement.pas:1532 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetStorageHero = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetStorageHero(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetStorageHero 待 PluginImplement.pas:1535 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetStorageDeputyHero = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetStorageDeputyHero(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetStorageDeputyHero 待 PluginImplement.pas:1538 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsStorageOpen = function(Player: _TPlayObject; Index: Integer): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsStorageOpen(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsStorageOpen 待 PluginImplement.pas:1541 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsStorageOpen = procedure(Player: _TPlayObject; Index: Integer; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsStorageOpen(IPlayObjectHandle pPlayer, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsStorageOpen 待 PluginImplement.pas:1543 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGold = function(Player: _TPlayObject): DWORD; stdcall;`</summary>
    public uint TPlayObject_GetGold(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGold 待 PluginImplement.pas:1546 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_SetGold(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGold 待 PluginImplement.pas:1548 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGoldMax = function(Player: _TPlayObject): DWORD; stdcall;`</summary>
    public uint TPlayObject_GetGoldMax(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGoldMax 待 PluginImplement.pas:1551 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncGold = function(Player: _TPlayObject; Value: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_IncGold(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncGold 待 PluginImplement.pas:1554 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_DecGold = function(Player: _TPlayObject; Value: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_DecGold(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_DecGold 待 PluginImplement.pas:1557 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GoldChanged = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_GoldChanged(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GoldChanged 待 PluginImplement.pas:1560 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGameGold = function(Player: _TPlayObject): DWORD; stdcall;`</summary>
    public uint TPlayObject_GetGameGold(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGameGold 待 PluginImplement.pas:1563 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGameGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_SetGameGold(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGameGold 待 PluginImplement.pas:1565 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncGameGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_IncGameGold(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncGameGold 待 PluginImplement.pas:1568 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_DecGameGold = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_DecGameGold(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_DecGameGold 待 PluginImplement.pas:1571 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GameGoldChanged = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_GameGoldChanged(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GameGoldChanged 待 PluginImplement.pas:1574 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGamePoint = function(Player: _TPlayObject): DWORD; stdcall;`</summary>
    public uint TPlayObject_GetGamePoint(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGamePoint 待 PluginImplement.pas:1577 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGamePoint = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_SetGamePoint(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGamePoint 待 PluginImplement.pas:1580 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncGamePoint = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_IncGamePoint(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncGamePoint 待 PluginImplement.pas:1583 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_DecGamePoint = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_DecGamePoint(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_DecGamePoint 待 PluginImplement.pas:1586 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGameDiamond = function(Player: _TPlayObject): DWORD; stdcall;`</summary>
    public uint TPlayObject_GetGameDiamond(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGameDiamond 待 PluginImplement.pas:1589 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGameDiamond = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_SetGameDiamond(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGameDiamond 待 PluginImplement.pas:1591 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncGameDiamond = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_IncGameDiamond(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncGameDiamond 待 PluginImplement.pas:1594 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_DecGameDiamond = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_DecGameDiamond(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_DecGameDiamond 待 PluginImplement.pas:1597 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_NewGamePointChanged = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_NewGamePointChanged(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_NewGamePointChanged 待 PluginImplement.pas:1600 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGameGird = function(Player: _TPlayObject): DWORD; stdcall;`</summary>
    public uint TPlayObject_GetGameGird(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGameGird 待 PluginImplement.pas:1603 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGameGird = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_SetGameGird(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGameGird 待 PluginImplement.pas:1605 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncGameGird = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_IncGameGird(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncGameGird 待 PluginImplement.pas:1608 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_DecGameGird = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_DecGameGird(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_DecGameGird 待 PluginImplement.pas:1611 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGameGoldEx = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetGameGoldEx(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGameGoldEx 待 PluginImplement.pas:1614 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGameGoldEx = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetGameGoldEx(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGameGoldEx 待 PluginImplement.pas:1616 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGameGlory = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetGameGlory(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGameGlory 待 PluginImplement.pas:1619 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetGameGlory = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetGameGlory(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetGameGlory 待 PluginImplement.pas:1621 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncGameGlory = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_IncGameGlory(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncGameGlory 待 PluginImplement.pas:1624 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_DecGameGlory = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_DecGameGlory(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_DecGameGlory 待 PluginImplement.pas:1627 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GameGloryChanged = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_GameGloryChanged(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GameGloryChanged 待 PluginImplement.pas:1630 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetPayMentPoint = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetPayMentPoint(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetPayMentPoint 待 PluginImplement.pas:1633 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetPayMentPoint = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetPayMentPoint(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetPayMentPoint 待 PluginImplement.pas:1635 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMemberType = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetMemberType(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMemberType 待 PluginImplement.pas:1638 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetMemberType = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetMemberType(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetMemberType 待 PluginImplement.pas:1640 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMemberLevel = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetMemberLevel(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMemberLevel 待 PluginImplement.pas:1643 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetMemberLevel = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetMemberLevel(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetMemberLevel 待 PluginImplement.pas:1645 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetContribution = function(Player: _TPlayObject): Word; stdcall;`</summary>
    public ushort TPlayObject_GetContribution(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetContribution 待 PluginImplement.pas:1648 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetContribution = procedure(Player: _TPlayObject; Value: Word); stdcall;`</summary>
    public void TPlayObject_SetContribution(IPlayObjectHandle pPlayer, ushort pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetContribution 待 PluginImplement.pas:1650 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObejct_IncExp = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObejct_IncExp(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObejct_IncExp 待 PluginImplement.pas:1653 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendExpChanged = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendExpChanged(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendExpChanged 待 PluginImplement.pas:1656 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncExpNG = procedure(Player: _TPlayObject; Value: DWORD); stdcall;`</summary>
    public void TPlayObject_IncExpNG(IPlayObjectHandle pPlayer, uint pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncExpNG 待 PluginImplement.pas:1659 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendExpNGChanged = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendExpNGChanged(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendExpNGChanged 待 PluginImplement.pas:1662 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IncBeadExp = procedure(Player: _TPlayObject; Value: DWORD; IsFromNPC: BOOL); stdcall;`</summary>
    public void TPlayObject_IncBeadExp(IPlayObjectHandle pPlayer, uint pValue, int IsFromNPC)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IncBeadExp 待 PluginImplement.pas:1665 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarP = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`</summary>
    public int TPlayObject_GetVarP(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarP 待 PluginImplement.pas:1668 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarP = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetVarP(IPlayObjectHandle pPlayer, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarP 待 PluginImplement.pas:1670 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarM = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`</summary>
    public int TPlayObject_GetVarM(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarM 待 PluginImplement.pas:1673 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarM = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetVarM(IPlayObjectHandle pPlayer, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarM 待 PluginImplement.pas:1675 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarD = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`</summary>
    public int TPlayObject_GetVarD(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarD 待 PluginImplement.pas:1678 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarD = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetVarD(IPlayObjectHandle pPlayer, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarD 待 PluginImplement.pas:1680 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarU = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`</summary>
    public int TPlayObject_GetVarU(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarU 待 PluginImplement.pas:1683 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarU = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetVarU(IPlayObjectHandle pPlayer, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarU 待 PluginImplement.pas:1685 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarT = function(Player: _TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetVarT(IPlayObjectHandle pPlayer, int pIndex, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarT 待 PluginImplement.pas:1688 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarT = procedure(Player: _TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;`</summary>
    public void TPlayObject_SetVarT(IPlayObjectHandle pPlayer, int pIndex, byte[] pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarT 待 PluginImplement.pas:1690 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarN = function(Player: _TPlayObject; Index: Integer): Integer; stdcall;`</summary>
    public int TPlayObject_GetVarN(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarN 待 PluginImplement.pas:1693 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarN = procedure(Player: _TPlayObject; Index: Integer; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetVarN(IPlayObjectHandle pPlayer, int pIndex, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarN 待 PluginImplement.pas:1695 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetVarS = function(Player: _TPlayObject; Index: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetVarS(IPlayObjectHandle pPlayer, int pIndex, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetVarS 待 PluginImplement.pas:1698 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetVarS = procedure(Player: _TPlayObject; Index: Integer; Value: PAnsiChar); stdcall;`</summary>
    public void TPlayObject_SetVarS(IPlayObjectHandle pPlayer, int pIndex, byte[] pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetVarS 待 PluginImplement.pas:1700 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetDynamicVarList = function(Player: _TPlayObject): _TList; stdcall;`</summary>
    public IListHandle TPlayObject_GetDynamicVarList(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetDynamicVarList 待 PluginImplement.pas:1703 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetQuestFlagStatus = function(Player: _TPlayObject; nFlag: Integer): Integer; stdcall;`</summary>
    public int TPlayObject_GetQuestFlagStatus(IPlayObjectHandle pPlayer, int nFlag)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetQuestFlagStatus 待 PluginImplement.pas:1707 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetQuestFlagStatus = procedure(Player: _TPlayObject; nFlag: Integer; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetQuestFlagStatus(IPlayObjectHandle pPlayer, int nFlag, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetQuestFlagStatus 待 PluginImplement.pas:1709 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IsOffLine = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_IsOffLine(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IsOffLine 待 PluginImplement.pas:1712 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IsMaster = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_IsMaster(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IsMaster 待 PluginImplement.pas:1715 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMasterName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetMasterName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMasterName 待 PluginImplement.pas:1718 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMasterHuman = function(Player: _TPlayObject): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TPlayObject_GetMasterHuman(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMasterHuman 待 PluginImplement.pas:1721 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetApprenticeNO = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetApprenticeNO(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetApprenticeNO 待 PluginImplement.pas:1724 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetOnlineApprenticeList = function(Player: _TPlayObject): _TList; stdcall;`</summary>
    public IListHandle TPlayObject_GetOnlineApprenticeList(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetOnlineApprenticeList 待 PluginImplement.pas:1727 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetAllApprenticeList = function(Player: _TPlayObject): _TList; stdcall;`</summary>
    public IListHandle TPlayObject_GetAllApprenticeList(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetAllApprenticeList 待 PluginImplement.pas:1730 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetDearName = function(Player: _TPlayObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TPlayObject_GetDearName(IPlayObjectHandle pPlayer, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetDearName 待 PluginImplement.pas:1733 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetDearHuman = function(Player: _TPlayObject): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TPlayObject_GetDearHuman(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetDearHuman 待 PluginImplement.pas:1736 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetMarryCount = function(Player: _TPlayObject): Byte; stdcall;`</summary>
    public byte TPlayObject_GetMarryCount(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetMarryCount 待 PluginImplement.pas:1739 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGroupOwner = function(Player: _TPlayObject): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TPlayObject_GetGroupOwner(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGroupOwner 待 PluginImplement.pas:1742 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetGroupMembers = function(Player: _TPlayObject): _TStringList; stdcall;`</summary>
    public IStringListHandle TPlayObject_GetGroupMembers(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetGroupMembers 待 PluginImplement.pas:1745 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsLockLogin = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsLockLogin(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsLockLogin 待 PluginImplement.pas:1748 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsLockLogin = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsLockLogin(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsLockLogin 待 PluginImplement.pas:1750 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsAllowGroup = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsAllowGroup(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsAllowGroup 待 PluginImplement.pas:1753 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsAllowGroup = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsAllowGroup(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsAllowGroup 待 PluginImplement.pas:1755 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsAllowGroupReCall = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsAllowGroupReCall(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsAllowGroupReCall 待 PluginImplement.pas:1758 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsAllowGroupReCall = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsAllowGroupReCall(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsAllowGroupReCall 待 PluginImplement.pas:1760 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsAllowGuildReCall = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsAllowGuildReCall(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsAllowGuildReCall 待 PluginImplement.pas:1763 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsAllowGuildReCall = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsAllowGuildReCall(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsAllowGuildReCall 待 PluginImplement.pas:1765 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsAllowTrading = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsAllowTrading(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsAllowTrading 待 PluginImplement.pas:1768 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsAllowTrading = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsAllowTrading(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsAllowTrading 待 PluginImplement.pas:1770 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsDisableInviteHorseRiding = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsDisableInviteHorseRiding(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsDisableInviteHorseRiding 待 PluginImplement.pas:1773 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsDisableInviteHorseRiding = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsDisableInviteHorseRiding(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsDisableInviteHorseRiding 待 PluginImplement.pas:1775 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsGameGoldTrading = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsGameGoldTrading(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsGameGoldTrading 待 PluginImplement.pas:1778 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsGameGoldTrading = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsGameGoldTrading(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsGameGoldTrading 待 PluginImplement.pas:1780 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsNewServer = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsNewServer(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsNewServer 待 PluginImplement.pas:1783 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsFilterGlobalDropItemMsg = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsFilterGlobalDropItemMsg(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsFilterGlobalDropItemMsg 待 PluginImplement.pas:1786 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsFilterGlobalDropItemMsg = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsFilterGlobalDropItemMsg(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsFilterGlobalDropItemMsg 待 PluginImplement.pas:1788 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsFilterGlobalCenterMsg = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsFilterGlobalCenterMsg(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsFilterGlobalCenterMsg 待 PluginImplement.pas:1791 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsFilterGlobalCenterMsg = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsFilterGlobalCenterMsg(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsFilterGlobalCenterMsg 待 PluginImplement.pas:1793 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsFilterGolbalSendMsg = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsFilterGolbalSendMsg(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsFilterGolbalSendMsg 待 PluginImplement.pas:1796 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsFilterGolbalSendMsg = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsFilterGolbalSendMsg(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsFilterGolbalSendMsg 待 PluginImplement.pas:1798 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsPleaseDrink = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsPleaseDrink(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsPleaseDrink 待 PluginImplement.pas:1801 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsDrinkWineQuality = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetIsDrinkWineQuality(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsDrinkWineQuality 待 PluginImplement.pas:1804 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsDrinkWineQuality = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetIsDrinkWineQuality(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsDrinkWineQuality 待 PluginImplement.pas:1806 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsDrinkWineAlcohol = function(Player: _TPlayObject): Integer; stdcall;`</summary>
    public int TPlayObject_GetIsDrinkWineAlcohol(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsDrinkWineAlcohol 待 PluginImplement.pas:1809 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsDrinkWineAlcohol = procedure(Player: _TPlayObject; Value: Integer); stdcall;`</summary>
    public void TPlayObject_SetIsDrinkWineAlcohol(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsDrinkWineAlcohol 待 PluginImplement.pas:1811 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_GetIsDrinkWineDrunk = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_GetIsDrinkWineDrunk(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_GetIsDrinkWineDrunk 待 PluginImplement.pas:1814 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SetIsDrinkWineDrunk = procedure(Player: _TPlayObject; Value: BOOL); stdcall;`</summary>
    public void TPlayObject_SetIsDrinkWineDrunk(IPlayObjectHandle pPlayer, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SetIsDrinkWineDrunk 待 PluginImplement.pas:1816 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_MoveToHome = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_MoveToHome(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_MoveToHome 待 PluginImplement.pas:1819 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_MoveRandomToHome = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_MoveRandomToHome(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_MoveRandomToHome 待 PluginImplement.pas:1822 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendSocket = procedure(Player: _TPlayObject; DefMsg: pTDefaultMessage; sMsg: PAnsiChar); stdcall;`</summary>
    public void TPlayObject_SendSocket(IPlayObjectHandle pPlayer, ref TDefaultMessage DefMsg, byte[] sMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendSocket 待 PluginImplement.pas:1825 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendDefMessage = procedure(Player: _TPlayObject; wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word; sMsg: PAnsiChar); stdcall;`</summary>
    public void TPlayObject_SendDefMessage(IPlayObjectHandle pPlayer, ushort wIdent, long nRecog, ushort nParam, ushort nTag, ushort nSeries, byte[] sMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendDefMessage 待 PluginImplement.pas:1828 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendMoveMsg = procedure(Player: _TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nY: Word; nMoveCount: Integer; nFontSize: Integer; nMarqueeTime: Integer); stdcall;`</summary>
    public void TPlayObject_SendMoveMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, ushort nY, int nMoveCount, int nFontSize, int nMarqueeTime)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendMoveMsg 待 PluginImplement.pas:1831 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendCenterMsg = procedure(Player: _TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;`</summary>
    public void TPlayObject_SendCenterMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, int nTime)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendCenterMsg 待 PluginImplement.pas:1834 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendTopBroadCastMsg = function(Player: _TPlayObject; sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer; MsgType: Integer): BOOL; stdcall;`</summary>
    public int TPlayObject_SendTopBroadCastMsg(IPlayObjectHandle pPlayer, byte[] sMsg, byte btFColor, byte btBColor, int nTime, int MsgType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendTopBroadCastMsg 待 PluginImplement.pas:1837 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_CheckTakeOnItems = function(Player: _TPlayObject; Where: Integer; StdItem: pTStdItem): BOOL; stdcall;`</summary>
    public int TPlayObject_CheckTakeOnItems(IPlayObjectHandle pPlayer, int Where, ref TStdItem StdItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_CheckTakeOnItems 待 PluginImplement.pas:1841 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_ProcessUseItemSkill = procedure(Player: _TPlayObject; Where: Integer; StdItem: pTStdItem; IsTakeOn: BOOL); stdcall;`</summary>
    public void TPlayObject_ProcessUseItemSkill(IPlayObjectHandle pPlayer, int Where, ref TStdItem StdItem, int IsTakeOn)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_ProcessUseItemSkill 待 PluginImplement.pas:1844 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendUseItems = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendUseItems(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendUseItems 待 PluginImplement.pas:1848 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendAddItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`</summary>
    public void TPlayObject_SendAddItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendAddItem 待 PluginImplement.pas:1851 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendDelItemList = procedure(Player: _TPlayObject; Items: PAnsiChar; ItemsCount: Integer); stdcall;`</summary>
    public void TPlayObject_SendDelItemList(IPlayObjectHandle pPlayer, byte[] Items, int ItemsCount)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendDelItemList 待 PluginImplement.pas:1854 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendDelItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`</summary>
    public void TPlayObject_SendDelItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendDelItem 待 PluginImplement.pas:1857 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendUpdateItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`</summary>
    public void TPlayObject_SendUpdateItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendUpdateItem 待 PluginImplement.pas:1860 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendItemDuraChange = procedure(Player: _TPlayObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;`</summary>
    public void TPlayObject_SendItemDuraChange(IPlayObjectHandle pPlayer, int ItemWhere, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendItemDuraChange 待 PluginImplement.pas:1863 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendBagItems = procedure(Plyaer: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendBagItems(IPlayObjectHandle Plyaer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendBagItems 待 PluginImplement.pas:1866 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendJewelryBoxItems = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendJewelryBoxItems(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendJewelryBoxItems 待 PluginImplement.pas:1869 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendGodBlessItems = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendGodBlessItems(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendGodBlessItems 待 PluginImplement.pas:1872 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendOpenGodBlessItem = procedure(Player: _TPlayObject; Index: Integer); stdcall;`</summary>
    public void TPlayObject_SendOpenGodBlessItem(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendOpenGodBlessItem 待 PluginImplement.pas:1875 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendCloseGodBlessItem = procedure(Player: _TPlayObject; Index: Integer); stdcall;`</summary>
    public void TPlayObject_SendCloseGodBlessItem(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendCloseGodBlessItem 待 PluginImplement.pas:1878 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendUseMagics = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendUseMagics(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendUseMagics 待 PluginImplement.pas:1881 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendAddMagic = procedure(Player: _TPlayObject; UserMagic: pTUserMagic); stdcall;`</summary>
    public void TPlayObject_SendAddMagic(IPlayObjectHandle pPlayer, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendAddMagic 待 PluginImplement.pas:1884 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendDelMagic = procedure(Player: _TPlayObject; UserMagic: pTUserMagic); stdcall;`</summary>
    public void TPlayObject_SendDelMagic(IPlayObjectHandle pPlayer, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendDelMagic 待 PluginImplement.pas:1887 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendFengHaoItems = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendFengHaoItems(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendFengHaoItems 待 PluginImplement.pas:1890 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendAddFengHaoItem = procedure(Player: _TPlayObject; UserItem: pTUserItem); stdcall;`</summary>
    public void TPlayObject_SendAddFengHaoItem(IPlayObjectHandle pPlayer, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendAddFengHaoItem 待 PluginImplement.pas:1893 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendDelFengHaoItem = procedure(Player: _TPlayObject; Index: Integer); stdcall;`</summary>
    public void TPlayObject_SendDelFengHaoItem(IPlayObjectHandle pPlayer, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendDelFengHaoItem 待 PluginImplement.pas:1896 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_SendSocketStatusFail = procedure(Player: _TPlayObject); stdcall;`</summary>
    public void TPlayObject_SendSocketStatusFail(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_SendSocketStatusFail 待 PluginImplement.pas:1899 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_PlayEffect = procedure(Player: _TPlayObject; nFileIndex, nImageOffset, nImageCount, nLoopCount, nSpeedTime: Integer; btDrawOrder: Byte; nOffsetX: Integer; nOffsetY: Integer); stdcall;`</summary>
    public void TPlayObject_PlayEffect(IPlayObjectHandle pPlayer, int nFileIndex, int nImageOffset, int nImageCount, int nLoopCount, int nSpeedTime, byte btDrawOrder, int nOffsetX, int nOffsetY)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_PlayEffect 待 PluginImplement.pas:1901 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_IsAutoPlayGame = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_IsAutoPlayGame(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_IsAutoPlayGame 待 PluginImplement.pas:1905 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_StartAutoPlayGame = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_StartAutoPlayGame(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_StartAutoPlayGame 待 PluginImplement.pas:1908 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TPlayObject_StopAutoPlayGame = function(Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TPlayObject_StopAutoPlayGame(IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TPlayObject_StopAutoPlayGame 待 PluginImplement.pas:1911 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TDummyObject_IsStart = function(Dummyer: _TDummyObject): BOOL; stdcall;`</summary>
    public int TDummyObject_IsStart(IDummyObjectHandle Dummyer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TDummyObject_IsStart 待 PluginImplement.pas:1919 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TDummyObject_Start = procedure(Dummyer: _TDummyObject); stdcall;`</summary>
    public void TDummyObject_Start(IDummyObjectHandle Dummyer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TDummyObject_Start 待 PluginImplement.pas:1922 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TDummyObject_Stop = procedure(Dummyer: _TDummyObject); stdcall;`</summary>
    public void TDummyObject_Stop(IDummyObjectHandle Dummyer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TDummyObject_Stop 待 PluginImplement.pas:1925 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetAttackMode = function(Hero: _THeroObject): Byte; stdcall;`</summary>
    public byte THeroObject_GetAttackMode(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetAttackMode 待 PluginImplement.pas:1933 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SetAttackMode = function(Hero: _THeroObject; Value: Byte; ShowSysMsg: BOOL): BOOL; stdcall;`</summary>
    public int THeroObject_SetAttackMode(IHeroObjectHandle pHero, byte pValue, int ShowSysMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SetAttackMode 待 PluginImplement.pas:1935 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SetNextAttackMode = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SetNextAttackMode(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SetNextAttackMode 待 PluginImplement.pas:1938 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetBagCount = function(Hero: _THeroObject): Integer; stdcall;`</summary>
    public int THeroObject_GetBagCount(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetBagCount 待 PluginImplement.pas:1941 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetAngryValue = function(Hero: _THeroObject): Integer; stdcall;`</summary>
    public int THeroObject_GetAngryValue(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetAngryValue 待 PluginImplement.pas:1944 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetLoyalPoint = function(Hero: _THeroObject): Real; stdcall;`</summary>
    public double THeroObject_GetLoyalPoint(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetLoyalPoint 待 PluginImplement.pas:1947 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SetLoyalPoint = procedure(Hero: _THeroObject; Value: Real); stdcall;`</summary>
    public void THeroObject_SetLoyalPoint(IHeroObjectHandle pHero, double pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SetLoyalPoint 待 PluginImplement.pas:1949 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendLoyalPointChanged = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendLoyalPointChanged(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendLoyalPointChanged 待 PluginImplement.pas:1951 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_IsDeputy = function(Hero: _THeroObject): BOOL; stdcall;`</summary>
    public int THeroObject_IsDeputy(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_IsDeputy 待 PluginImplement.pas:1954 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetMasterName = function(Hero: _THeroObject; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int THeroObject_GetMasterName(IHeroObjectHandle pHero, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetMasterName 待 PluginImplement.pas:1957 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetQuestFlagStatus = function(Hero: _THeroObject; nFlag: Integer): Integer; stdcall;`</summary>
    public int THeroObject_GetQuestFlagStatus(IHeroObjectHandle pHero, int nFlag)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetQuestFlagStatus 待 PluginImplement.pas:1959 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SetQuestFlagStatus = procedure(Hero: _THeroObject; nFlag: Integer; Value: Integer); stdcall;`</summary>
    public void THeroObject_SetQuestFlagStatus(IHeroObjectHandle pHero, int nFlag, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SetQuestFlagStatus 待 PluginImplement.pas:1961 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendUseItems = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendUseItems(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendUseItems 待 PluginImplement.pas:1964 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendBagItems = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendBagItems(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendBagItems 待 PluginImplement.pas:1967 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendJewelryBoxItems = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendJewelryBoxItems(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendJewelryBoxItems 待 PluginImplement.pas:1970 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendGodBlessItems = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendGodBlessItems(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendGodBlessItems 待 PluginImplement.pas:1973 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendOpenGodBlessItem = procedure(Hero: _THeroObject; Index: Integer); stdcall;`</summary>
    public void THeroObject_SendOpenGodBlessItem(IHeroObjectHandle pHero, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendOpenGodBlessItem 待 PluginImplement.pas:1976 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendCloseGodBlessItem = procedure(Hero: _THeroObject; Index: Integer); stdcall;`</summary>
    public void THeroObject_SendCloseGodBlessItem(IHeroObjectHandle pHero, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendCloseGodBlessItem 待 PluginImplement.pas:1979 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendAddItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`</summary>
    public void THeroObject_SendAddItem(IHeroObjectHandle pHero, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendAddItem 待 PluginImplement.pas:1982 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendDelItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`</summary>
    public void THeroObject_SendDelItem(IHeroObjectHandle pHero, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendDelItem 待 PluginImplement.pas:1985 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendUpdateItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`</summary>
    public void THeroObject_SendUpdateItem(IHeroObjectHandle pHero, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendUpdateItem 待 PluginImplement.pas:1988 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendItemDuraChange = procedure(Hero: _THeroObject; ItemWhere: Integer; UserItem: pTUserItem); stdcall;`</summary>
    public void THeroObject_SendItemDuraChange(IHeroObjectHandle pHero, int ItemWhere, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendItemDuraChange 待 PluginImplement.pas:1991 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendUseMagics = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendUseMagics(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendUseMagics 待 PluginImplement.pas:1994 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendAddMagic = procedure(Hero: _THeroObject; UserMagic: pTUserMagic); stdcall;`</summary>
    public void THeroObject_SendAddMagic(IHeroObjectHandle pHero, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendAddMagic 待 PluginImplement.pas:1997 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendDelMagic = procedure(Hero: _THeroObject; UserMagic: pTUserMagic); stdcall;`</summary>
    public void THeroObject_SendDelMagic(IHeroObjectHandle pHero, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendDelMagic 待 PluginImplement.pas:2000 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_FindGroupMagic = function(Hero: _THeroObject; UserMagic: pTUserMagic): BOOL; stdcall;`</summary>
    public int THeroObject_FindGroupMagic(IHeroObjectHandle pHero, ref TUserMagic UserMagic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_FindGroupMagic 待 PluginImplement.pas:2003 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_GetGroupMagicId = function(Hero: _THeroObject): Integer; stdcall;`</summary>
    public int THeroObject_GetGroupMagicId(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_GetGroupMagicId 待 PluginImplement.pas:2006 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendFengHaoItems = procedure(Hero: _THeroObject); stdcall;`</summary>
    public void THeroObject_SendFengHaoItems(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendFengHaoItems 待 PluginImplement.pas:2009 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendAddFengHaoItem = procedure(Hero: _THeroObject; UserItem: pTUserItem); stdcall;`</summary>
    public void THeroObject_SendAddFengHaoItem(IHeroObjectHandle pHero, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendAddFengHaoItem 待 PluginImplement.pas:2012 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_SendDelFengHaoItem = procedure(Hero: _THeroObject; Index: Integer); stdcall;`</summary>
    public void THeroObject_SendDelFengHaoItem(IHeroObjectHandle pHero, int pIndex)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_SendDelFengHaoItem 待 PluginImplement.pas:2015 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_IncExp = procedure(Hero: _THeroObject; dwExp: DWORD); stdcall;`</summary>
    public void THeroObject_IncExp(IHeroObjectHandle pHero, uint dwExp)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_IncExp 待 PluginImplement.pas:2017 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_IncExpNG = procedure(Hero: _THeroObject; dwExp: DWORD); stdcall;`</summary>
    public void THeroObject_IncExpNG(IHeroObjectHandle pHero, uint dwExp)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_IncExpNG 待 PluginImplement.pas:2019 所属引擎单元移植后接入");
    }

    /// <summary>原文 `THeroObject_IsOldClient = function(Hero: _THeroObject): BOOL; stdcall;`</summary>
    public int THeroObject_IsOldClient(IHeroObjectHandle pHero)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.THeroObject_IsOldClient 待 PluginImplement.pas:2021 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_Create = function(CharName, sMapName, sScript: PAnsiChar; X, Y: Integer; wAppr: Word; boIsHide: BOOL) : _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TNormNpc_Create(byte[] CharName, byte[] sMapName, byte[] sScript, int X, int Y, ushort wAppr, int boIsHide)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_Create 待 PluginImplement.pas:2029 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_LoadNpcScript = procedure(NormNpc: _TNormNpc); stdcall;`</summary>
    public void TNormNpc_LoadNpcScript(INormNpcHandle NormNpc)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_LoadNpcScript 待 PluginImplement.pas:2033 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_ClearScript = procedure(NormNpc: _TNormNpc); stdcall;`</summary>
    public void TNormNpc_ClearScript(INormNpcHandle NormNpc)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_ClearScript 待 PluginImplement.pas:2036 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetFilePath = function(NormNpc: _TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TNormNpc_GetFilePath(INormNpcHandle NormNpc, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetFilePath 待 PluginImplement.pas:2038 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_SetFilePath = procedure(NormNpc: _TNormNpc; Value: PAnsiChar); stdcall;`</summary>
    public void TNormNpc_SetFilePath(INormNpcHandle NormNpc, byte[] pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_SetFilePath 待 PluginImplement.pas:2040 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetPath = function(NormNpc: _TNormNpc; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TNormNpc_GetPath(INormNpcHandle NormNpc, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetPath 待 PluginImplement.pas:2042 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_SetPath = procedure(NormNpc: _TNormNpc; Value: PAnsiChar); stdcall;`</summary>
    public void TNormNpc_SetPath(INormNpcHandle NormNpc, byte[] pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_SetPath 待 PluginImplement.pas:2044 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetIsHide = function(NormNpc: _TNormNpc): BOOL; stdcall;`</summary>
    public int TNormNpc_GetIsHide(INormNpcHandle NormNpc)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetIsHide 待 PluginImplement.pas:2046 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_SetIsHide = procedure(NormNpc: _TNormNpc; Value: BOOL); stdcall;`</summary>
    public void TNormNpc_SetIsHide(INormNpcHandle NormNpc, int pValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_SetIsHide 待 PluginImplement.pas:2048 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetIsQuest = function(NormNpc: _TNormNpc): BOOL; stdcall;`</summary>
    public int TNormNpc_GetIsQuest(INormNpcHandle NormNpc)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetIsQuest 待 PluginImplement.pas:2050 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetLineVariableText = function(NormNpc: _TNormNpc; Player: _TPlayObject; sMsg: PAnsiChar; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TNormNpc_GetLineVariableText(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetLineVariableText 待 PluginImplement.pas:2052 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GotoLable = procedure(NormNpc: _TNormNpc; Player: _TPlayObject; sLabel: PAnsiChar; boExtJmp: BOOL); stdcall;`</summary>
    public void TNormNpc_GotoLable(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sLabel, int boExtJmp)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GotoLable 待 PluginImplement.pas:2055 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_SendMsgToUser = procedure(NormNpc: _TNormNpc; Player: _TPlayObject; sMsg: PAnsiChar); stdcall;`</summary>
    public void TNormNpc_SendMsgToUser(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_SendMsgToUser 待 PluginImplement.pas:2057 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_MessageBox = procedure(NormNpc: _TNormNpc; Player: _TPlayObject; sMsg: PAnsiChar); stdcall;`</summary>
    public void TNormNpc_MessageBox(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sMsg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_MessageBox 待 PluginImplement.pas:2059 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;`</summary>
    public int TNormNpc_GetVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, ref uint sValueSize, ref int nValue)
    {
        sValueSize = default!;
        nValue = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetVarValue 待 PluginImplement.pas:2061 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_SetVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; nValue: Integer): BOOL; stdcall;`</summary>
    public int TNormNpc_SetVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, int nValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_SetVarValue 待 PluginImplement.pas:2064 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_GetDynamicVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; var sValueSize: DWORD; var nValue: Integer): BOOL; stdcall;`</summary>
    public int TNormNpc_GetDynamicVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, ref uint sValueSize, ref int nValue)
    {
        sValueSize = default!;
        nValue = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_GetDynamicVarValue 待 PluginImplement.pas:2067 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TNormNpc_SetDynamicVarValue = function(NormNpc: _TNormNpc; Player: _TPlayObject; sVarName: PAnsiChar; sValue: PAnsiChar; nValue: Integer): BOOL; stdcall;`</summary>
    public int TNormNpc_SetDynamicVarValue(INormNpcHandle NormNpc, IPlayObjectHandle pPlayer, byte[] sVarName, byte[] sValue, int nValue)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TNormNpc_SetDynamicVarValue 待 PluginImplement.pas:2070 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetPlayerList = function(): _TStringList; stdcall;`</summary>
    public IStringListHandle TUserEngine_GetPlayerList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetPlayerList 待 PluginImplement.pas:2079 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetPlayerByName = function(ChrName: PAnsiChar): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TUserEngine_GetPlayerByName(byte[] ChrName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetPlayerByName 待 PluginImplement.pas:2082 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetPlayerByUserID = function(UserID: PAnsiChar): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TUserEngine_GetPlayerByUserID(byte[] UserID)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetPlayerByUserID 待 PluginImplement.pas:2085 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetPlayerByObject = function(AObject: _TObject): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TUserEngine_GetPlayerByObject(object AObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetPlayerByObject 待 PluginImplement.pas:2088 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetOfflinePlayer = function(UserID: PAnsiChar): _TPlayObject; stdcall;`</summary>
    public IPlayObjectHandle TUserEngine_GetOfflinePlayer(byte[] UserID)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetOfflinePlayer 待 PluginImplement.pas:2091 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_KickPlayer = procedure(ChrName: PAnsiChar); stdcall;`</summary>
    public void TUserEngine_KickPlayer(byte[] ChrName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_KickPlayer 待 PluginImplement.pas:2094 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetHeroList = function(): _TStringList; stdcall;`</summary>
    public IStringListHandle TUserEngine_GetHeroList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetHeroList 待 PluginImplement.pas:2097 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetHeroByName = function(ChrName: PAnsiChar): _THeroObject; stdcall;`</summary>
    public IHeroObjectHandle TUserEngine_GetHeroByName(byte[] ChrName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetHeroByName 待 PluginImplement.pas:2100 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_KickHero = function(ChrName: PAnsiChar): BOOL; stdcall;`</summary>
    public int TUserEngine_KickHero(byte[] ChrName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_KickHero 待 PluginImplement.pas:2103 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetMerchantList = function(): _TList; stdcall;`</summary>
    public IListHandle TUserEngine_GetMerchantList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetMerchantList 待 PluginImplement.pas:2106 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetCustomNpcConfigList = function(): _TList; stdcall;`</summary>
    public IListHandle TUserEngine_GetCustomNpcConfigList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetCustomNpcConfigList 待 PluginImplement.pas:2109 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetQuestNPCList = function(): _TStringList; stdcall;`</summary>
    public IStringListHandle TUserEngine_GetQuestNPCList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetQuestNPCList 待 PluginImplement.pas:2112 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetManageNPC = function(): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_GetManageNPC()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetManageNPC 待 PluginImplement.pas:2114 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetFunctionNPC = function(): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_GetFunctionNPC()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetFunctionNPC 待 PluginImplement.pas:2116 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetRobotNPC = function(): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_GetRobotNPC()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetRobotNPC 待 PluginImplement.pas:2118 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_MissionNPC = function(): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_MissionNPC()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_MissionNPC 待 PluginImplement.pas:2120 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindMerchant = function(AObject: _TObject): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_FindMerchant(object AObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindMerchant 待 PluginImplement.pas:2123 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindMerchantByPos = function(MapName: PAnsiChar; nX, nY: Integer): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_FindMerchantByPos(byte[] MapName, int nX, int nY)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindMerchantByPos 待 PluginImplement.pas:2126 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindQuestNPC = function(AObject: _TObject): _TNormNpc; stdcall;`</summary>
    public INormNpcHandle TUserEngine_FindQuestNPC(object AObject)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindQuestNPC 待 PluginImplement.pas:2129 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetMagicList = function(): _TList; stdcall;`</summary>
    public IListHandle TUserEngine_GetMagicList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetMagicList 待 PluginImplement.pas:2132 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetCustomMagicConfigList = function(): _TList; stdcall;`</summary>
    public IListHandle TUserEngine_GetCustomMagicConfigList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetCustomMagicConfigList 待 PluginImplement.pas:2135 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetMagicACList = function(): _TMagicACList; stdcall;`</summary>
    public IMagicACListHandle TUserEngine_GetMagicACList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetMagicACList 待 PluginImplement.pas:2138 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindMagicByName = function(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindMagicByName(byte[] MagName, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindMagicByName 待 PluginImplement.pas:2141 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindMagicByIndex = function(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindMagicByIndex(int MagIdx, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindMagicByIndex 待 PluginImplement.pas:2144 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindMagicByNameEx = function(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindMagicByNameEx(byte[] MagName, int MagAttr, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindMagicByNameEx 待 PluginImplement.pas:2147 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindMagicByIndexEx = function(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindMagicByIndexEx(int MagIdx, int MagAttr, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindMagicByIndexEx 待 PluginImplement.pas:2150 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindHeroMagicByName = function(MagName: PAnsiChar; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindHeroMagicByName(byte[] MagName, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindHeroMagicByName 待 PluginImplement.pas:2153 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindHeroMagicByIndex = function(MagIdx: Integer; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindHeroMagicByIndex(int MagIdx, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindHeroMagicByIndex 待 PluginImplement.pas:2156 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindHeroMagicByNameEx = function(MagName: PAnsiChar; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindHeroMagicByNameEx(byte[] MagName, int MagAttr, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindHeroMagicByNameEx 待 PluginImplement.pas:2159 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_FindHeroMagicByIndexEx = function(MagIdx: Integer; MagAttr: Integer; Magic: pTMagic): BOOL; stdcall;`</summary>
    public int TUserEngine_FindHeroMagicByIndexEx(int MagIdx, int MagAttr, ref TMagic Magic)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_FindHeroMagicByIndexEx 待 PluginImplement.pas:2162 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetStdItemList = function(): _TList; stdcall;`</summary>
    public IListHandle TUserEngine_GetStdItemList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetStdItemList 待 PluginImplement.pas:2165 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetStdItemByName = function(ItemName: PAnsiChar; StdItem: pTStdItem): BOOL; stdcall;`</summary>
    public int TUserEngine_GetStdItemByName(byte[] ItemName, ref TStdItem StdItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetStdItemByName 待 PluginImplement.pas:2168 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetStdItemByIndex = function(ItemIdx: Integer; StdItem: pTStdItem): BOOL; stdcall;`</summary>
    public int TUserEngine_GetStdItemByIndex(int ItemIdx, ref TStdItem StdItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetStdItemByIndex 待 PluginImplement.pas:2171 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetStdItemName = function(ItemIdx: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TUserEngine_GetStdItemName(int ItemIdx, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetStdItemName 待 PluginImplement.pas:2174 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetStdItemIndex = function(ItemName: PAnsiChar): Integer; stdcall;`</summary>
    public int TUserEngine_GetStdItemIndex(byte[] ItemName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetStdItemIndex 待 PluginImplement.pas:2177 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_MonsterList = function(): _TList; stdcall;`</summary>
    public IListHandle TUserEngine_MonsterList()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_MonsterList 待 PluginImplement.pas:2180 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendBroadCastMsg = function(sMsg: PAnsiChar; FColor, BColor: Integer; MsgType: Integer): BOOL; stdcall;`</summary>
    public int TUserEngine_SendBroadCastMsg(byte[] sMsg, int FColor, int BColor, int MsgType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendBroadCastMsg 待 PluginImplement.pas:2182 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendBroadCastMsgExt = function(sMsg: PAnsiChar; MsgType: Integer): BOOL; stdcall;`</summary>
    public int TUserEngine_SendBroadCastMsgExt(byte[] sMsg, int MsgType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendBroadCastMsgExt 待 PluginImplement.pas:2184 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendTopBroadCastMsg = function(sMsg: PAnsiChar; FColor, BColor: Integer; nTime: Integer; MsgType: Integer) : BOOL; stdcall;`</summary>
    public int TUserEngine_SendTopBroadCastMsg(byte[] sMsg, int FColor, int BColor, int nTime, int MsgType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendTopBroadCastMsg 待 PluginImplement.pas:2186 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendMoveMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor: Byte; nY, nMoveCount: Integer; nFontSize: Integer; nMarqueeTime: Integer); stdcall;`</summary>
    public void TUserEngine_SendMoveMsg(byte[] sMsg, byte btFColor, byte btBColor, int nY, int nMoveCount, int nFontSize, int nMarqueeTime)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendMoveMsg 待 PluginImplement.pas:2189 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendCenterMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor: Byte; nTime: Integer); stdcall;`</summary>
    public void TUserEngine_SendCenterMsg(byte[] sMsg, byte btFColor, byte btBColor, int nTime)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendCenterMsg 待 PluginImplement.pas:2192 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendNewLineMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte; nY, nShowMsgTime, nDrawType: Integer); stdcall;`</summary>
    public void TUserEngine_SendNewLineMsg(byte[] sMsg, byte btFColor, byte btBColor, byte btFontSize, int nY, int nShowMsgTime, int nDrawType)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendNewLineMsg 待 PluginImplement.pas:2195 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendSuperMoveMsg = procedure(sMsg: PAnsiChar; btFColor, btBColor, btFontSize: Byte; nY, nMoveCount: Integer); stdcall;`</summary>
    public void TUserEngine_SendSuperMoveMsg(byte[] sMsg, byte btFColor, byte btBColor, byte btFontSize, int nY, int nMoveCount)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendSuperMoveMsg 待 PluginImplement.pas:2199 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_SendSceneShake = procedure(Count: Integer); stdcall;`</summary>
    public void TUserEngine_SendSceneShake(int pCount)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_SendSceneShake 待 PluginImplement.pas:2203 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_CopyToUserItemFromName = function(ItemName: PAnsiChar; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TUserEngine_CopyToUserItemFromName(byte[] ItemName, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_CopyToUserItemFromName 待 PluginImplement.pas:2205 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_CopyToUserItemFromItem = function(StdItem: pTStdItem; ItemIndex: Integer; UserItem: pTUserItem): BOOL; stdcall;`</summary>
    public int TUserEngine_CopyToUserItemFromItem(ref TStdItem StdItem, int ItemIndex, ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_CopyToUserItemFromItem 待 PluginImplement.pas:2207 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_RandomUpgradeItem = procedure(UserItem: pTUserItem); stdcall;`</summary>
    public void TUserEngine_RandomUpgradeItem(ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_RandomUpgradeItem 待 PluginImplement.pas:2209 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_RandomItemNewAbil = procedure(UserItem: pTUserItem); stdcall;`</summary>
    public void TUserEngine_RandomItemNewAbil(ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_RandomItemNewAbil 待 PluginImplement.pas:2212 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetUnknowItemValue = procedure(UserItem: pTUserItem); stdcall;`</summary>
    public void TUserEngine_GetUnknowItemValue(ref TUserItem UserItem)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetUnknowItemValue 待 PluginImplement.pas:2214 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetAllDummyCount = function(): Integer; stdcall;`</summary>
    public int TUserEngine_GetAllDummyCount()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetAllDummyCount 待 PluginImplement.pas:2217 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetMapDummyCount = function(Envir: _TEnvirnoment): Integer; stdcall;`</summary>
    public int TUserEngine_GetMapDummyCount(IEnvirnoment Envir)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetMapDummyCount 待 PluginImplement.pas:2220 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetOfflineCount = function(): Integer; stdcall;`</summary>
    public int TUserEngine_GetOfflineCount()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetOfflineCount 待 PluginImplement.pas:2223 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TUserEngine_GetRealPlayerCount = function(): Integer; stdcall;`</summary>
    public int TUserEngine_GetRealPlayerCount()
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TUserEngine_GetRealPlayerCount 待 PluginImplement.pas:2226 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetGuildName = function(Guild: _TGuild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TGuild_GetGuildName(IGuildHandle pGuild, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetGuildName 待 PluginImplement.pas:2234 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetJoinJob = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetJoinJob(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetJoinJob 待 PluginImplement.pas:2237 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetJoinLevel = function(Guild: _TGuild): DWORD; stdcall;`</summary>
    public uint TGuild_GetJoinLevel(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetJoinLevel 待 PluginImplement.pas:2240 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetJoinMsg = function(Guild: _TGuild; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TGuild_GetJoinMsg(IGuildHandle pGuild, byte[] Dest, ref uint DestLen)
    {
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetJoinMsg 待 PluginImplement.pas:2243 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetBuildPoint = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetBuildPoint(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetBuildPoint 待 PluginImplement.pas:2246 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetAurae = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetAurae(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetAurae 待 PluginImplement.pas:2249 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetStability = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetStability(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetStability 待 PluginImplement.pas:2252 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetFlourishing = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetFlourishing(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetFlourishing 待 PluginImplement.pas:2255 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetChiefItemCount = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetChiefItemCount(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetChiefItemCount 待 PluginImplement.pas:2258 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetMemberCount = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetMemberCount(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetMemberCount 待 PluginImplement.pas:2261 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetOnlineMemeberCount = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetOnlineMemeberCount(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetOnlineMemeberCount 待 PluginImplement.pas:2264 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetMasterCount = function(Guild: _TGuild): Integer; stdcall;`</summary>
    public int TGuild_GetMasterCount(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetMasterCount 待 PluginImplement.pas:2267 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetMaster = procedure(Guild: _TGuild; var Master1, Master2: _TPlayObject); stdcall;`</summary>
    public void TGuild_GetMaster(IGuildHandle pGuild, ref IPlayObjectHandle Master1, ref IPlayObjectHandle Master2)
    {
        Master1 = default!;
        Master2 = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetMaster 待 PluginImplement.pas:2270 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetMasterName = function(Guild: _TGuild; Master1: PAnsiChar; var Master1Size: DWORD; Master2: PAnsiChar; var Master2Size: DWORD): BOOL; stdcall;`</summary>
    public int TGuild_GetMasterName(IGuildHandle pGuild, byte[] Master1, ref uint Master1Size, byte[] Master2, ref uint Master2Size)
    {
        Master1Size = default!;
        Master2Size = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetMasterName 待 PluginImplement.pas:2273 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_CheckMemberIsFull = function(Guild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_CheckMemberIsFull(IGuildHandle pGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_CheckMemberIsFull 待 PluginImplement.pas:2277 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_IsMemeber = function(Guild: _TGuild; CharName: PAnsiChar): BOOL; stdcall;`</summary>
    public int TGuild_IsMemeber(IGuildHandle pGuild, byte[] CharName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_IsMemeber 待 PluginImplement.pas:2280 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_AddMember = function(Guild: _TGuild; Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TGuild_AddMember(IGuildHandle pGuild, IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_AddMember 待 PluginImplement.pas:2283 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_AddMemberEx = function(Guild: _TGuild; CharName: PAnsiChar): BOOL; stdcall;`</summary>
    public int TGuild_AddMemberEx(IGuildHandle pGuild, byte[] CharName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_AddMemberEx 待 PluginImplement.pas:2285 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_DelMemeber = function(Guild: _TGuild; Player: _TPlayObject): BOOL; stdcall;`</summary>
    public int TGuild_DelMemeber(IGuildHandle pGuild, IPlayObjectHandle pPlayer)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_DelMemeber 待 PluginImplement.pas:2288 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_DelMemeberEx = function(Guild: _TGuild; CharName: PAnsiChar): BOOL; stdcall;`</summary>
    public int TGuild_DelMemeberEx(IGuildHandle pGuild, byte[] CharName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_DelMemeberEx 待 PluginImplement.pas:2290 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_IsAllianceGuild = function(Guild: _TGuild; CheckGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_IsAllianceGuild(IGuildHandle pGuild, IGuildHandle CheckGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_IsAllianceGuild 待 PluginImplement.pas:2293 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_IsWarGuild = function(Guild: _TGuild; CheckGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_IsWarGuild(IGuildHandle pGuild, IGuildHandle CheckGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_IsWarGuild 待 PluginImplement.pas:2296 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_IsAttentionGuild = function(Guild: _TGuild; CheckGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_IsAttentionGuild(IGuildHandle pGuild, IGuildHandle CheckGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_IsAttentionGuild 待 PluginImplement.pas:2299 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_AddAlliance = function(Guild: _TGuild; AddGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_AddAlliance(IGuildHandle pGuild, IGuildHandle AddGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_AddAlliance 待 PluginImplement.pas:2302 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_AddWarGuild = function(Guild: _TGuild; AddGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_AddWarGuild(IGuildHandle pGuild, IGuildHandle AddGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_AddWarGuild 待 PluginImplement.pas:2305 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_AddAttentionGuild = function(Guild: _TGuild; AddGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_AddAttentionGuild(IGuildHandle pGuild, IGuildHandle AddGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_AddAttentionGuild 待 PluginImplement.pas:2308 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_DelAllianceGuild = function(Guild: _TGuild; DelGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_DelAllianceGuild(IGuildHandle pGuild, IGuildHandle DelGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_DelAllianceGuild 待 PluginImplement.pas:2311 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_DelAttentionGuild = function(Guild: _TGuild; DelGuild: _TGuild): BOOL; stdcall;`</summary>
    public int TGuild_DelAttentionGuild(IGuildHandle pGuild, IGuildHandle DelGuild)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_DelAttentionGuild 待 PluginImplement.pas:2314 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetRandNameByName = function(Guild: _TGuild; CharName: PAnsiChar; var nRankNo: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TGuild_GetRandNameByName(IGuildHandle pGuild, byte[] CharName, ref int nRankNo, byte[] Dest, ref uint DestLen)
    {
        nRankNo = default!;
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetRandNameByName 待 PluginImplement.pas:2316 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_GetRandNameByPlayer = function(Guild: _TGuild; Player: _TPlayObject; var nRankNo: Integer; Dest: PAnsiChar; var DestLen: DWORD): BOOL; stdcall;`</summary>
    public int TGuild_GetRandNameByPlayer(IGuildHandle pGuild, IPlayObjectHandle pPlayer, ref int nRankNo, byte[] Dest, ref uint DestLen)
    {
        nRankNo = default!;
        DestLen = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_GetRandNameByPlayer 待 PluginImplement.pas:2319 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuild_SendGuildMsg = procedure(Guild: _TGuild; Msg: PAnsiChar); stdcall;`</summary>
    public void TGuild_SendGuildMsg(IGuildHandle pGuild, byte[] Msg)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuild_SendGuildMsg 待 PluginImplement.pas:2323 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuildManager_FindGuild = function(GuildName: PAnsiChar): _TGuild; stdcall;`</summary>
    public IGuildHandle TGuildManager_FindGuild(byte[] GuildName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuildManager_FindGuild 待 PluginImplement.pas:2331 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuildManager_GetPlayerGuild = function(CharName: PAnsiChar): _TGuild; stdcall;`</summary>
    public IGuildHandle TGuildManager_GetPlayerGuild(byte[] CharName)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuildManager_GetPlayerGuild 待 PluginImplement.pas:2334 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuildManager_AddGuild = function(GuildName, GuildMaster: PAnsiChar): BOOL; stdcall;`</summary>
    public int TGuildManager_AddGuild(byte[] GuildName, byte[] GuildMaster)
    {
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuildManager_AddGuild 待 PluginImplement.pas:2337 所属引擎单元移植后接入");
    }

    /// <summary>原文 `TGuildManager_DelGuild = function(GuildName: PAnsiChar; var IsFoundGuild: BOOL): BOOL; stdcall;`</summary>
    public int TGuildManager_DelGuild(byte[] GuildName, ref int IsFoundGuild)
    {
        IsFoundGuild = default!;
        throw new NotImplementedException("接缝：PluginInterfaceHost.TGuildManager_DelGuild 待 PluginImplement.pas:2340 所属引擎单元移植后接入");
    }

}
