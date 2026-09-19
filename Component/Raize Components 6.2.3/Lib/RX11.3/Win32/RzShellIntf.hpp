// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellIntf.pas' rev: 35.00 (Windows)

#ifndef RzshellintfHPP
#define RzshellintfHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.ActiveX.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Graphics.hpp>
#include <Winapi.ShlObj.hpp>
#include <Winapi.ShellAPI.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------
#define NO_WIN32_LEAN_AND_MEAN

namespace Rzshellintf
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS IUnknown_NRC;
class DELPHICLASS IOleWindow_NRC;
class DELPHICLASS IMalloc_NRC;
struct TStrRet;
struct TShColInfo;
class DELPHICLASS IShellDetails_NRC;
struct TCMInvokeCommandInfo;
class DELPHICLASS IContextMenu_NRC;
class DELPHICLASS IContextMenu2_NRC;
class DELPHICLASS IEnumIDList_NRC;
class DELPHICLASS IShellFolder_NRC;
class DELPHICLASS IShellFolder2_NRC;
class DELPHICLASS IShellLink_NRC;
class DELPHICLASS IEnumFormatEtc_NRC;
class DELPHICLASS IDataObject_NRC;
class DELPHICLASS IDropTarget_NRC;
class DELPHICLASS IDropSource_NRC;
//-- type declarations -------------------------------------------------------
typedef HWND TRzHandle;

#pragma pack(push,4)
class PASCALIMPLEMENTATION IUnknown_NRC : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	virtual HRESULT __stdcall QueryInterface(const GUID &iid, void *obj) = 0 ;
	virtual int __stdcall AddRef() = 0 ;
	virtual int __stdcall Release() = 0 ;
public:
	/* TObject.Create */ inline __fastcall IUnknown_NRC() : System::TObject() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IUnknown_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IOleWindow_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall GetWindow(HWND &wnd) = 0 ;
	virtual HRESULT __stdcall ContextSensitiveHelp(System::LongBool fEnterMode) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IOleWindow_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IOleWindow_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IMalloc_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual void * __stdcall Alloc(int cb) = 0 ;
	virtual void * __stdcall Realloc(void * pv, int cb) = 0 ;
	HIDESBASE virtual void __stdcall Free(void * pv) = 0 ;
	virtual int __stdcall GetSize(void * pv) = 0 ;
	virtual int __stdcall DidAlloc(void * pv) = 0 ;
	virtual void __stdcall HeapMinimize() = 0 ;
public:
	/* TObject.Create */ inline __fastcall IMalloc_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IMalloc_NRC() { }
	
};

#pragma pack(pop)

struct DECLSPEC_DRECORD TStrRet
{
public:
	unsigned uType;
	
public:
	union
	{
		struct 
		{
			System::StaticArray<char, 261> cStr;
		};
		struct 
		{
			unsigned uOffset;
		};
		struct 
		{
			System::WideChar *pOleStr;
		};
		
	};
};


typedef TStrRet *PStrRet;

struct DECLSPEC_DRECORD TShColInfo
{
public:
	int justify;
	int width;
	TStrRet text;
};


typedef TShColInfo *PShColInfo;

#pragma pack(push,4)
class PASCALIMPLEMENTATION IShellDetails_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall GetDetailsOf(Winapi::Shlobj::PItemIDList pidl, unsigned col, TShColInfo &info) = 0 ;
	virtual HRESULT __stdcall ColumnClick(unsigned col) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IShellDetails_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IShellDetails_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,1)
struct DECLSPEC_DRECORD TCMInvokeCommandInfo
{
public:
	unsigned cbSize;
	unsigned fMask;
	NativeUInt hwnd;
	char *lpVerb;
	char *lpParameters;
	char *lpDirectory;
	int nShow;
	unsigned dwHotKey;
	NativeUInt hIcon;
};
#pragma pack(pop)


typedef TCMInvokeCommandInfo *PCMInvokeCommandInfo;

#pragma pack(push,4)
class PASCALIMPLEMENTATION IContextMenu_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall QueryContextMenu(NativeUInt hMenu, unsigned indexMenu, unsigned idCmdFirst, unsigned idCmdLast, unsigned uFlags) = 0 ;
	virtual HRESULT __stdcall InvokeCommand(const TCMInvokeCommandInfo &ici) = 0 ;
	virtual HRESULT __stdcall GetCommandString(unsigned idCmd, unsigned uType, PUINT pwReserved, char * pszName, unsigned cchMax) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IContextMenu_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IContextMenu_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IContextMenu2_NRC : public IContextMenu_NRC
{
	typedef IContextMenu_NRC inherited;
	
public:
	virtual HRESULT __stdcall HandleMenuMsg(unsigned uMsg, unsigned wParam, unsigned lParam) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IContextMenu2_NRC() : IContextMenu_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IContextMenu2_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IEnumIDList_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall Next(int celt, Winapi::Shlobj::PItemIDList &pidl, System::PInteger pFetched) = 0 ;
	virtual HRESULT __stdcall Skip(int celt) = 0 ;
	virtual HRESULT __stdcall Reset() = 0 ;
	virtual HRESULT __stdcall Clone(IEnumIDList_NRC* &enumIdList) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IEnumIDList_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IEnumIDList_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IShellFolder_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall ParseDisplayName(HWND hwndOwner, void * reserved, System::WideChar * displayName, unsigned &chEaten, Winapi::Shlobj::PItemIDList &pidl, unsigned &dwAttributes) = 0 ;
	virtual HRESULT __stdcall EnumObjects(HWND hwndOwner, unsigned grfFlags, IEnumIDList_NRC* &penumIdList) = 0 ;
	virtual HRESULT __stdcall BindToObject(Winapi::Shlobj::PItemIDList pidl, void * reserved, const GUID &riid, void * &pvOut) = 0 ;
	virtual HRESULT __stdcall BindToStorage(Winapi::Shlobj::PItemIDList pidl, void * reserved, const GUID &riid, void * &pvObj) = 0 ;
	virtual HRESULT __stdcall CompareIDs(NativeInt lparam, Winapi::Shlobj::PItemIDList pidl1, Winapi::Shlobj::PItemIDList pidl2) = 0 ;
	virtual HRESULT __stdcall CreateViewObject(HWND hwndOwner, const GUID &riid, void * &ppvOut) = 0 ;
	virtual HRESULT __stdcall GetAttributesOf(unsigned cidl, Winapi::Shlobj::PItemIDList &pidl, unsigned &rgfInOut) = 0 ;
	virtual HRESULT __stdcall GetUIObjectOf(HWND hwndOwner, unsigned cild, Winapi::Shlobj::PItemIDList &pidl, const GUID &riid, PUINT prgfInOut, void * &ppvOut) = 0 ;
	virtual HRESULT __stdcall GetDisplayNameOf(Winapi::Shlobj::PItemIDList pidl, unsigned uFlags, TStrRet &name) = 0 ;
	virtual HRESULT __stdcall SetNameOf(HWND hwndOwner, Winapi::Shlobj::PItemIDList pidl, System::WideChar * swzName, unsigned uFlags, Winapi::Shlobj::PItemIDList &pidlOut) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IShellFolder_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IShellFolder_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IShellFolder2_NRC : public IShellFolder_NRC
{
	typedef IShellFolder_NRC inherited;
	
public:
	virtual void __stdcall F1() = 0 ;
	virtual void __stdcall F2() = 0 ;
	virtual void __stdcall F3() = 0 ;
	virtual HRESULT __stdcall GetDefaultColumnState(unsigned iColumn, unsigned &pcsFlags) = 0 ;
	virtual void __stdcall F5() = 0 ;
	virtual HRESULT __stdcall GetDetailsOf(Winapi::Shlobj::PItemIDList pidl, unsigned iColumn, TShColInfo &sd) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IShellFolder2_NRC() : IShellFolder_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IShellFolder2_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IShellLink_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall GetPath(System::WideChar * pszFile, int cchMaxPath, _WIN32_FIND_DATAW &fd, unsigned fFlags) = 0 ;
	virtual HRESULT __stdcall GetIDList(Winapi::Shlobj::PItemIDList &pidl) = 0 ;
	virtual HRESULT __stdcall SetIDList(Winapi::Shlobj::PItemIDList pidl) = 0 ;
	virtual HRESULT __stdcall GetDescription(System::WideChar * pszName, int cchMaxName) = 0 ;
	virtual HRESULT __stdcall SetDescription(System::WideChar * pszName) = 0 ;
	virtual HRESULT __stdcall GetWorkingDirectory(System::WideChar * pszDir, int cchMaxPath) = 0 ;
	virtual HRESULT __stdcall SetWorkingDirectory(System::WideChar * pszDir) = 0 ;
	virtual HRESULT __stdcall GetArguments(System::WideChar * pszArgs, int cchMaxPath) = 0 ;
	virtual HRESULT __stdcall SetArguments(System::WideChar * pszArgs) = 0 ;
	virtual HRESULT __stdcall GetHotKey(System::Word &wHotKey) = 0 ;
	virtual HRESULT __stdcall SetHotKey(System::Word wHotKey) = 0 ;
	virtual HRESULT __stdcall GetShowCmd(int &iShowCmd) = 0 ;
	virtual HRESULT __stdcall SetShowCmd(int iShowCmd) = 0 ;
	virtual HRESULT __stdcall GetIconLocation(System::WideChar * pszIconPath, int cchIconPath, int &iIcon) = 0 ;
	virtual HRESULT __stdcall SetIconLocation(System::WideChar * pszIconPath, int iIcon) = 0 ;
	virtual HRESULT __stdcall SetRelativePath(System::WideChar * pszPathRel, unsigned dwReserved) = 0 ;
	virtual HRESULT __stdcall Resolve(HWND hwnd, unsigned fFlags) = 0 ;
	virtual HRESULT __stdcall SetPath(System::WideChar * pszFile) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IShellLink_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IShellLink_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IEnumFormatEtc_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall Next(int celt, void *elt, System::PLongInt pceltFetched) = 0 ;
	virtual HRESULT __stdcall Skip(int celt) = 0 ;
	virtual HRESULT __stdcall Reset() = 0 ;
	virtual HRESULT __stdcall Clone(IEnumFormatEtc_NRC* &Enum) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IEnumFormatEtc_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IEnumFormatEtc_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IDataObject_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall GetData(tagFORMATETC &formatetcIn, tagSTGMEDIUM &medium) = 0 ;
	virtual HRESULT __stdcall GetDataHere(tagFORMATETC &formatetc, tagSTGMEDIUM &medium) = 0 ;
	virtual HRESULT __stdcall QueryGetData(tagFORMATETC &formatetc) = 0 ;
	virtual HRESULT __stdcall GetCanonicalFormatEtc(tagFORMATETC &formatetc, tagFORMATETC &formatetcOut) = 0 ;
	virtual HRESULT __stdcall SetData(tagFORMATETC &formatetc, tagSTGMEDIUM &medium, System::LongBool fRelease) = 0 ;
	virtual HRESULT __stdcall EnumFormatEtc(int dwDirection, IEnumFormatEtc_NRC* &enumFormatEtc) = 0 ;
	virtual HRESULT __stdcall DAdvise(tagFORMATETC &formatetc, int advf, void * advSink, int &dwConnection) = 0 ;
	virtual HRESULT __stdcall DUnadvise(int dwConnection) = 0 ;
	virtual HRESULT __stdcall EnumDAdvise(void * &enumAdvise) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IDataObject_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IDataObject_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IDropTarget_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall DragEnter(IDataObject_NRC* dataObj, int grfKeyState, System::Types::TPoint pt, int &dwEffect) = 0 ;
	virtual HRESULT __stdcall DragOver(int grfKeyState, System::Types::TPoint pt, int &dwEffect) = 0 ;
	virtual HRESULT __stdcall DragLeave() = 0 ;
	virtual HRESULT __stdcall Drop(IDataObject_NRC* dataObj_NRC, int grfKeyState, System::Types::TPoint pt, int &dwEffect) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IDropTarget_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IDropTarget_NRC() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION IDropSource_NRC : public IUnknown_NRC
{
	typedef IUnknown_NRC inherited;
	
public:
	virtual HRESULT __stdcall QueryContinueDrag(System::LongBool fEscapePressed, int grfKeyState) = 0 ;
	virtual HRESULT __stdcall GiveFeedback(int dwEffect) = 0 ;
public:
	/* TObject.Create */ inline __fastcall IDropSource_NRC() : IUnknown_NRC() { }
	/* TObject.Destroy */ inline __fastcall virtual ~IDropSource_NRC() { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
static const System::Byte PTSHELLCONTROLS_VERSION = System::Byte(0xbe);
static const System::Word PTSHELLCONTROLS_PATCHSYM = System::Word(0x20);
static const System::Int8 PTSHELLCONTROLS_PATCH = System::Int8(-64);
}	/* namespace Rzshellintf */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLINTF)
using namespace Rzshellintf;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshellintfHPP
