// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellUtils.pas' rev: 35.00 (Windows)

#ifndef RzshellutilsHPP
#define RzshellutilsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.ActiveX.hpp>
#include <Winapi.Windows.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.ShellAPI.hpp>
#include <Vcl.Dialogs.hpp>
#include <Winapi.Messages.hpp>
#include <Vcl.Forms.hpp>
#include <System.Classes.hpp>
#include <Winapi.ShlObj.hpp>
#include <System.Win.ComObj.hpp>
#include <RzShellIntf.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzshellutils
{
//-- forward type declarations -----------------------------------------------
struct TRzModuleVersion;
class DELPHICLASS TRzIdListArray;
class DELPHICLASS TRzPidlList;
struct TLinkData;
class DELPHICLASS TRzDeviceChangeHandler;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TCSIDL : unsigned char { csidlDesktop, csidlInternet, csidlPrograms, csidlControls, csidlPrinters, csidlPersonal, csidlFavorites, csidlStartup, csidlRecent, csidlSendTo, csidlBitBucket, csidlStartMenu, csidl_None1, csidlMyMusic, csidlMyVideo, csidl_None2, csidlDesktopDirectory, csidlDrives, csidlNetwork, csidlNethood, csidlFonts, csidlTemplates, csidlCommonStartMenu, csidlCommonPrograms, csidlCommonStartup, csidlCommonDesktopDirectory, csidlAppData, csidlPrintHood, csidlLocalAppData, csidlAltStartup, csidlCommonAltStartup, csidlCommonFavorites, csidlInternetCache, csidlCookies, csidlHistory, csidlCommonAppData, csidlWindows, csidlSystem, csidlProgramFiles, csidlMyPictures, csidlProfile, csidlSystemX86, csidlProgramFilesX86, 
	csidlProgramFilesCommon, csidlProgramFilesCommonX86, csidlCommonTemplates, csidlCommonDocuments, csidlCommonAdminTools, csidlAdminTools, csidlConnections, csidl_None3, csidl_None4, csidl_None5, csidlCommonMusic, csidlCommonPictures, csidlCommonVideo, csidlResources, csidlResourcesLocalized, csidlCommonOEMLinks, csidlCDBurnArean, csidl_None6, csidlComputersNearMe, csidlNone };

#pragma pack(push,1)
struct DECLSPEC_DRECORD TRzModuleVersion
{
	
public:
	union
	{
		struct 
		{
			__int64 asInt64;
		};
		struct 
		{
			int _3;
			int version;
		};
		struct 
		{
			System::Word _1;
			System::Word _2;
			System::Word minor;
			System::Word major;
		};
		struct 
		{
			__int64 asComp;
		};
		struct 
		{
			int dw1;
			int dw2;
		};
		struct 
		{
			System::Word w1;
			System::Word w2;
			System::Word w3;
			System::Word w4;
		};
		
	};
};
#pragma pack(pop)


typedef TRzModuleVersion *PRzModuleVersion;

enum DECLSPEC_DENUM TRzFriendlyNameFlags : unsigned char { fnNormal, fnInFolder, fnForParsing, fnForEditing };

enum DECLSPEC_DENUM TRzShellIconSize : unsigned char { iszSmall, iszLarge };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzIdListArray : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	Winapi::Shlobj::PItemIDList operator[](int Index) { return this->Item[Index]; }
	
protected:
	_ITEMIDLIST *FPidl;
	_ITEMIDLIST *FLastItem;
	int __fastcall GetCount();
	Winapi::Shlobj::PItemIDList __fastcall GetItem(int Index);
	
public:
	__fastcall TRzIdListArray(Winapi::Shlobj::PItemIDList p);
	__fastcall virtual ~TRzIdListArray();
	Winapi::Shlobj::PItemIDList __fastcall GoUp(int items);
	__property int ItemCount = {read=GetCount, nodefault};
	__property Winapi::Shlobj::PItemIDList Item[int Index] = {read=GetItem/*, default*/};
	__property Winapi::Shlobj::PItemIDList Pidl = {read=FPidl};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPidlList : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	Winapi::Shlobj::PItemIDList operator[](int index) { return this->PIDLs[index]; }
	
private:
	System::Classes::TList* FList;
	bool FSorted;
	Rzshellintf::IShellFolder_NRC* FShellFolder;
	Rzshellintf::IMalloc_NRC* FMalloc;
	System::Types::TDuplicates FDuplicates;
	Winapi::Shlobj::PItemIDList __fastcall GetPIDL(int Index);
	void __fastcall SetPIDL(int Index, Winapi::Shlobj::PItemIDList PIDL);
	void * __fastcall GetObject(int Index);
	void __fastcall SetObject(int Index, void * aObject);
	void __fastcall SetSorted(bool isSorted);
	int __fastcall GetCount();
	int __fastcall GetCapacity();
	void __fastcall SetCapacity(int cap);
	
protected:
	bool __fastcall BinarySearch(const Winapi::Shlobj::PItemIDList PIDL, int &Index);
	bool __fastcall LinearSearch(const Winapi::Shlobj::PItemIDList PIDL, int &Index);
	
public:
	__fastcall TRzPidlList();
	__fastcall virtual ~TRzPidlList();
	virtual int __fastcall Add(const Winapi::Shlobj::PItemIDList PIDL);
	virtual int __fastcall AddObject(const Winapi::Shlobj::PItemIDList PIDL, System::TObject* aObject);
	void __fastcall Delete(int index);
	void __fastcall Clear();
	int __fastcall IndexOf(const Winapi::Shlobj::PItemIDList PIDL);
	void __fastcall Insert(int Index, Winapi::Shlobj::PItemIDList Pidl);
	void __fastcall InsertObject(int Index, Winapi::Shlobj::PItemIDList Pidl, void * aObject);
	void __fastcall Sort();
	__property Winapi::Shlobj::PItemIDList PIDLs[int index] = {read=GetPIDL, write=SetPIDL/*, default*/};
	__property void * Objects[int index] = {read=GetObject, write=SetObject};
	__property bool Sorted = {read=FSorted, write=SetSorted, nodefault};
	__property Rzshellintf::IMalloc_NRC* Malloc = {read=FMalloc, write=FMalloc};
	__property int Count = {read=GetCount, nodefault};
	__property Rzshellintf::IShellFolder_NRC* ShellFolder = {read=FShellFolder, write=FShellFolder};
	__property System::Types::TDuplicates Duplicates = {read=FDuplicates, write=FDuplicates, nodefault};
	__property int Capacity = {read=GetCapacity, write=SetCapacity, nodefault};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TLinkDataOption : unsigned char { ldoUseDesc, ldoUseArgs, ldoUseIcon, ldoUseWorkDir, ldoUseHotKey, ldoUseShowCmd };

typedef System::Set<TLinkDataOption, TLinkDataOption::ldoUseDesc, TLinkDataOption::ldoUseShowCmd> TLinkDataOptions;

struct DECLSPEC_DRECORD TLinkData
{
public:
	System::UnicodeString pathName;
	TLinkDataOptions options;
	System::UnicodeString desc;
	System::UnicodeString args;
	System::UnicodeString iconPath;
	int iconIndex;
	System::UnicodeString workingDir;
	int showCmd;
	System::Word hotkey;
	bool noUI;
	_ITEMIDLIST *idList;
	_WIN32_FIND_DATAW w32fd;
};


typedef void __fastcall (__closure *TRzDeviceChangeEvent)(System::TObject* ASender, Winapi::Messages::TMessage &AMessage);

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDeviceChangeHandler : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	bool FActive;
	HWND FWindowReceiver;
	System::Classes::TList* FNotifyList;
	
protected:
	void __fastcall DeleteItem(int AItemIndex);
	void __fastcall WndProc(Winapi::Messages::TMessage &AMessage);
	void __fastcall BroadcastToList(Winapi::Messages::TMessage &AMessage);
	
public:
	__fastcall TRzDeviceChangeHandler();
	__fastcall virtual ~TRzDeviceChangeHandler();
	void __fastcall Add(TRzDeviceChangeEvent AToNotify);
	void __fastcall Remove(TRzDeviceChangeEvent AToNotify);
	__property bool Active = {read=FActive, write=FActive, nodefault};
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
static const int COMCTL32_VER600 = int(0x60000);
static const int COMCTL32_VER581 = int(0x50051);
static const int COMCTL32_VER580 = int(0x50050);
static const int COMCTL32_VER472 = int(0x40048);
static const int COMCTL32_VER471 = int(0x40047);
static const int COMCTL32_VER470 = int(0x40046);
static const int COMCTL32_VER400 = int(0x40000);
static const int SHELL32_VER50 = int(0x50000);
static const int SHELL32_VER60 = int(0x60000);
extern DELPHI_PACKAGE TRzModuleVersion COMCTL32_VER;
extern DELPHI_PACKAGE TRzModuleVersion SHELL32_VER;
extern DELPHI_PACKAGE int gFlushDriveInfoSem;
#define RZSH_CMDS_DELETE L"delete"
#define RZSH_CMDS_PASTE L"paste"
#define RZSH_CMDS_CUT L"cut"
#define RZSH_CMDS_COPY L"copy"
#define RZSH_CMDS_PROPERTIES L"properties"
#define RZSH_CMDS_EXPLORE L"explore"
#define RZSH_CMDS_OPEN L"open"
#define RZSH_CMDS_FIND L"find"
#define RZSH_CMDS_LINK L"link"
#define RZSH_CMDS_DUN_CREATE L"create"
#define RZSH_CMDS_DUN_CONNECT L"connect"
#define RZSH_CMDID_FORMAT (System::WideChar *)(0x23)
extern DELPHI_PACKAGE bool __fastcall IsWin95(void);
extern DELPHI_PACKAGE bool __fastcall IsOSR2OrGreater(void);
extern DELPHI_PACKAGE bool __fastcall IsWinNT(void);
extern DELPHI_PACKAGE bool __fastcall IsWin2000(void);
extern DELPHI_PACKAGE bool __fastcall HasWin95Shell(void);
extern DELPHI_PACKAGE bool __fastcall GetModuleVersion(const System::UnicodeString aModuleName, TRzModuleVersion &aVersion);
extern DELPHI_PACKAGE void * __fastcall ShellMemAlloc(unsigned size);
extern DELPHI_PACKAGE void * __fastcall ShellMemRealloc(void * p, unsigned size);
extern DELPHI_PACKAGE void __fastcall ShellMemFree(void * p);
extern DELPHI_PACKAGE Rzshellintf::IMalloc_NRC* __fastcall ShellIMalloc(void);
extern DELPHI_PACKAGE HRESULT __fastcall ShellGetFolderFromIdList(Winapi::Shlobj::PItemIDList p, Rzshellintf::IShellFolder_NRC* &ish);
extern DELPHI_PACKAGE HRESULT __fastcall ShellGetIdListFromPath(const System::UnicodeString path, Winapi::Shlobj::PItemIDList &p);
extern DELPHI_PACKAGE System::UnicodeString __fastcall ShellGetPathFromIdList(Winapi::Shlobj::PItemIDList p);
extern DELPHI_PACKAGE System::UnicodeString __fastcall ShellGetDisplayPathName(System::UnicodeString aPathName);
extern DELPHI_PACKAGE System::UnicodeString __fastcall ShellGetSpecialFolderPath(HWND ahwnd, TCSIDL csidl);
extern DELPHI_PACKAGE HRESULT __fastcall ShellGetSpecialFolderIdList(HWND ahwnd, TCSIDL csidl, Winapi::Shlobj::PItemIDList &idlist);
extern DELPHI_PACKAGE int __fastcall ShellGetIconIndex(Winapi::Shlobj::PItemIDList absIdList, unsigned uFlags);
extern DELPHI_PACKAGE int __fastcall ShellGetIconIndexFromPath(const System::UnicodeString path, unsigned uFlags);
extern DELPHI_PACKAGE int __fastcall ShellGetIconIndexFromExt(const System::UnicodeString ext, unsigned uFlags);
extern DELPHI_PACKAGE int __fastcall ShellGetSpecialFolderIconIndex(TCSIDL csidl, unsigned uFlags);
extern DELPHI_PACKAGE TCSIDL __fastcall ShellFindCSIDLFromIdList(Winapi::Shlobj::PItemIDList aIdList);
extern DELPHI_PACKAGE bool __fastcall ShellCSIDLEqualsIdList(Winapi::Shlobj::PItemIDList aIdList, TCSIDL csidl);
extern DELPHI_PACKAGE System::UnicodeString __fastcall ShellGetFriendlyNameFromIdList(Rzshellintf::IShellFolder_NRC* ishf, Winapi::Shlobj::PItemIDList pidl, TRzFriendlyNameFlags flags);
extern DELPHI_PACKAGE System::UnicodeString __fastcall ShellGetFriendlyNameForLastIdListElement(Winapi::Shlobj::PItemIDList AAbsoluteIdList);
extern DELPHI_PACKAGE NativeUInt __fastcall ShellGetSystemImageList(TRzShellIconSize Size);
extern DELPHI_PACKAGE HRESULT __fastcall ShellGetDesktopFolder(Rzshellintf::IShellFolder_NRC* &Folder);
extern DELPHI_PACKAGE System::UnicodeString __fastcall StrretToString(Winapi::Shlobj::PItemIDList pidl, const Rzshellintf::TStrRet &r);
extern DELPHI_PACKAGE void __fastcall StrretFree(const Rzshellintf::TStrRet &r);
extern DELPHI_PACKAGE System::UnicodeString __fastcall EnsureTrailingCharDB(const System::UnicodeString Source, System::WideChar TrailingChar);
extern DELPHI_PACKAGE int __fastcall IdListLen(Winapi::Shlobj::PItemIDList pidl);
extern DELPHI_PACKAGE int __fastcall CompareAbsIdLists(Winapi::Shlobj::PItemIDList pidl1, Winapi::Shlobj::PItemIDList pidl2);
extern DELPHI_PACKAGE Winapi::Shlobj::PItemIDList __fastcall ConcatIdLists(Rzshellintf::IMalloc_NRC* ishm, Winapi::Shlobj::PItemIDList First, Winapi::Shlobj::PItemIDList Second);
extern DELPHI_PACKAGE Winapi::Shlobj::PItemIDList __fastcall CopyIdList(Rzshellintf::IMalloc_NRC* ishm, Winapi::Shlobj::PItemIDList pidl);
extern DELPHI_PACKAGE HRESULT __fastcall CreateShortcut(const System::UnicodeString linkPathName, const TLinkData &linkData);
extern DELPHI_PACKAGE HRESULT __fastcall CreateQuickShortcut(const System::UnicodeString linkPathName, const System::UnicodeString targetPathName);
extern DELPHI_PACKAGE HRESULT __fastcall ResolveShortcut(const System::UnicodeString linkPathName, TLinkData &linkData, bool afWantIdList);
extern DELPHI_PACKAGE bool __fastcall RzClsidFromFileType(System::UnicodeString aExtension, GUID &aCLSID);
extern DELPHI_PACKAGE void __fastcall FlushDriveInfoCache(void);
extern DELPHI_PACKAGE void __fastcall LockFlushDriveInfoCache(void);
extern DELPHI_PACKAGE void __fastcall UnlockFlushDriveInfoCache(void);
extern DELPHI_PACKAGE System::UnicodeString __fastcall FormatStrPos(System::UnicodeString aFmtStr, System::UnicodeString *data, const int data_High);
extern DELPHI_PACKAGE void __fastcall ParametizeCmdLineDB(const System::UnicodeString ins, System::Classes::TStrings* outs);
extern DELPHI_PACKAGE System::UnicodeString __fastcall TrimRightDB(System::UnicodeString Str);
extern DELPHI_PACKAGE void __fastcall CopyCharDB(int &APos, const System::UnicodeString ASource, System::UnicodeString &ADest);
extern DELPHI_PACKAGE TRzDeviceChangeHandler* __fastcall RzDeviceChangeHandler(void);
extern DELPHI_PACKAGE bool __fastcall IsNetworkDriveConnected(System::WideChar ADriveLetter);
}	/* namespace Rzshellutils */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLUTILS)
using namespace Rzshellutils;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshellutilsHPP
