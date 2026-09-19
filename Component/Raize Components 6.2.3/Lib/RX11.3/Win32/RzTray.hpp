// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzTray.pas' rev: 35.00 (Windows)

#ifndef RztrayHPP
#define RztrayHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <System.Classes.hpp>
#include <Winapi.ShellAPI.hpp>
#include <Vcl.Graphics.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.ImgList.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztray
{
//-- forward type declarations -----------------------------------------------
struct TRzTimeoutOrVersion;
struct _RZNOTIFYICONDATAA;
struct _RZNOTIFYICONDATAW;
class DELPHICLASS ERzTrayIconError;
class DELPHICLASS TRzTrayIcon;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzBalloonHintIcon : unsigned char { bhiNone, bhiInfo, bhiWarning, bhiError };

typedef System::Int8 TRzBalloonHintTimeout;

struct DECLSPEC_DRECORD TRzTimeoutOrVersion
{
	
public:
	union
	{
		struct 
		{
			unsigned uVersion;
		};
		struct 
		{
			unsigned uTimeout;
		};
		
	};
};


typedef _RZNOTIFYICONDATAA *PRzNotifyIconDataA;

typedef _RZNOTIFYICONDATAW *PRzNotifyIconDataW;

typedef PRzNotifyIconDataA PNotifyIconData;

struct DECLSPEC_DRECORD _RZNOTIFYICONDATAA
{
public:
	unsigned cbSize;
	HWND Wnd;
	unsigned uID;
	unsigned uFlags;
	unsigned uCallbackMessage;
	HICON hIcon;
	System::StaticArray<char, 128> szTip;
	unsigned dwState;
	unsigned dwStateMask;
	System::StaticArray<char, 256> szInfo;
	TRzTimeoutOrVersion TimeoutOrVersion;
	System::StaticArray<char, 64> szInfoTitle;
	unsigned dwInfoFlags;
};


struct DECLSPEC_DRECORD _RZNOTIFYICONDATAW
{
public:
	unsigned cbSize;
	HWND Wnd;
	unsigned uID;
	unsigned uFlags;
	unsigned uCallbackMessage;
	HICON hIcon;
	System::StaticArray<System::WideChar, 128> szTip;
	unsigned dwState;
	unsigned dwStateMask;
	System::StaticArray<System::WideChar, 256> szInfo;
	TRzTimeoutOrVersion TimeoutOrVersion;
	System::StaticArray<System::WideChar, 64> szInfoTitle;
	unsigned dwInfoFlags;
};


typedef _RZNOTIFYICONDATAA _RZNOTIFYICONDATA;

typedef _RZNOTIFYICONDATAA TRzNotifyIconDataA;

typedef _RZNOTIFYICONDATAW TRzNotifyIconDataW;

typedef _RZNOTIFYICONDATAW TRzNotifyIconData;

typedef _RZNOTIFYICONDATAA RZNOTIFYICONDATAA;

typedef _RZNOTIFYICONDATAW RZNOTIFYICONDATAW;

typedef _RZNOTIFYICONDATAA RZNOTIFYICONDATA;

#pragma pack(push,4)
class PASCALIMPLEMENTATION ERzTrayIconError : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall ERzTrayIconError(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall ERzTrayIconError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall ERzTrayIconError(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall ERzTrayIconError(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzTrayIconError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzTrayIconError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall ERzTrayIconError(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall ERzTrayIconError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzTrayIconError(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzTrayIconError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzTrayIconError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzTrayIconError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~ERzTrayIconError() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzTrayIconClicks : unsigned char { ticNone, ticLeftClick, ticLeftDblClick, ticLeftClickUp, ticRightClick, ticRightDblClick, ticRightClickUp };

typedef void __fastcall (__closure *TRzQueryEndSessionEvent)(System::TObject* Sender, bool &AllowSessionToEnd);

class PASCALIMPLEMENTATION TRzTrayIcon : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	_RZNOTIFYICONDATAW FIconData;
	_NOTIFYICONDATAW FIconDataPreV5;
	Vcl::Graphics::TIcon* FIcon;
	Vcl::Imglist::TCustomImageList* FIconList;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	Vcl::Menus::TPopupMenu* FPopupMenu;
	Vcl::Extctrls::TTimer* FTimer;
	System::UnicodeString FHint;
	int FIconIndex;
	bool FEnabled;
	HWND FMsgWindow;
	bool FHideOnMinimize;
	bool FHideOnStartup;
	bool FTaskBarRecreated;
	bool FMenuVisible;
	bool FManualRestore;
	bool FDoubleClicking;
	TRzTrayIconClicks FRestoreOn;
	TRzTrayIconClicks FPopupMenuOn;
	System::Classes::TNotifyEvent FOnMinimizeApp;
	System::Classes::TNotifyEvent FOnRestoreApp;
	System::Classes::TNotifyEvent FOnLButtonDown;
	System::Classes::TNotifyEvent FOnLButtonUp;
	System::Classes::TNotifyEvent FOnLButtonDblClick;
	System::Classes::TNotifyEvent FOnRButtonDown;
	System::Classes::TNotifyEvent FOnRButtonUp;
	System::Classes::TNotifyEvent FOnRButtonDblClick;
	bool FShell32Ver5;
	System::Classes::TNotifyEvent FOnBalloonHintHide;
	System::Classes::TNotifyEvent FOnBalloonHintClose;
	System::Classes::TNotifyEvent FOnBalloonHintClick;
	TRzQueryEndSessionEvent FOnQueryEndSession;
	System::Classes::TNotifyEvent FOnEndSession;
	void __fastcall MinimizeAppHandler(System::TObject* Sender);
	void __fastcall RestoreAppHandler(System::TObject* Sender);
	void __fastcall TimerExpiredHandler(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* Component, System::Classes::TOperation Operation);
	void __fastcall Update();
	void __fastcall EnabledChanged();
	void __fastcall DeleteIcon();
	void __fastcall HideApplication();
	void __fastcall ShowApplication();
	DYNAMIC void __fastcall QueryEndSession(bool &AllowSessionToEnd);
	DYNAMIC void __fastcall EndSession();
	DYNAMIC void __fastcall LButtonDown();
	DYNAMIC void __fastcall LButtonUp();
	DYNAMIC void __fastcall LButtonDblClick();
	DYNAMIC void __fastcall RButtonDown();
	DYNAMIC void __fastcall RButtonUp();
	DYNAMIC void __fastcall RButtonDblClick();
	DYNAMIC void __fastcall DoMinimizeApp();
	DYNAMIC void __fastcall DoRestoreApp();
	virtual bool __fastcall GetAnimate();
	virtual void __fastcall SetAnimate(bool Value);
	virtual void __fastcall SetEnabled(bool Value);
	virtual void __fastcall SetHint(const System::UnicodeString Value);
	virtual int __fastcall GetInterval();
	virtual void __fastcall SetInterval(int Value);
	virtual void __fastcall SetIconIndex(int Value);
	virtual void __fastcall SetIconList(Vcl::Imglist::TCustomImageList* Value);
	__property _RZNOTIFYICONDATAW IconData = {read=FIconData};
	__property _NOTIFYICONDATAW IconDataPreV5 = {read=FIconDataPreV5};
	
public:
	__fastcall virtual TRzTrayIcon(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzTrayIcon();
	void __fastcall MsgWndProc(Winapi::Messages::TMessage &Msg);
	void __fastcall MinimizeApp();
	void __fastcall RestoreApp();
	void __fastcall ShowMenu();
	void __fastcall SetDefaultIcon();
	bool __fastcall SupportBalloonHints();
	void __fastcall ShowBalloonHint(const System::UnicodeString Title, const System::UnicodeString Msg, TRzBalloonHintIcon IconType = (TRzBalloonHintIcon)(0x1), TRzBalloonHintTimeout TimeoutSecs = (TRzBalloonHintTimeout)(0xa));
	void __fastcall HideBalloonHint();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool Animate = {read=GetAnimate, write=SetAnimate, default=0};
	__property bool Enabled = {read=FEnabled, write=SetEnabled, default=1};
	__property bool HideOnMinimize = {read=FHideOnMinimize, write=FHideOnMinimize, default=1};
	__property bool HideOnStartup = {read=FHideOnStartup, write=FHideOnStartup, default=0};
	__property System::UnicodeString Hint = {read=FHint, write=SetHint};
	__property Vcl::Menus::TPopupMenu* PopupMenu = {read=FPopupMenu, write=FPopupMenu};
	__property Vcl::Imglist::TCustomImageList* Icons = {read=FIconList, write=SetIconList};
	__property int IconIndex = {read=FIconIndex, write=SetIconIndex, default=0};
	__property int Interval = {read=GetInterval, write=SetInterval, default=1000};
	__property TRzTrayIconClicks RestoreOn = {read=FRestoreOn, write=FRestoreOn, default=2};
	__property TRzTrayIconClicks PopupMenuOn = {read=FPopupMenuOn, write=FPopupMenuOn, default=6};
	__property System::Classes::TNotifyEvent OnBalloonHintHide = {read=FOnBalloonHintHide, write=FOnBalloonHintHide};
	__property System::Classes::TNotifyEvent OnBalloonHintClose = {read=FOnBalloonHintClose, write=FOnBalloonHintClose};
	__property System::Classes::TNotifyEvent OnBalloonHintClick = {read=FOnBalloonHintClick, write=FOnBalloonHintClick};
	__property System::Classes::TNotifyEvent OnMinimizeApp = {read=FOnMinimizeApp, write=FOnMinimizeApp};
	__property System::Classes::TNotifyEvent OnRestoreApp = {read=FOnRestoreApp, write=FOnRestoreApp};
	__property System::Classes::TNotifyEvent OnLButtonDown = {read=FOnLButtonDown, write=FOnLButtonDown};
	__property System::Classes::TNotifyEvent OnLButtonUp = {read=FOnLButtonUp, write=FOnLButtonUp};
	__property System::Classes::TNotifyEvent OnLButtonDblClick = {read=FOnLButtonDblClick, write=FOnLButtonDblClick};
	__property System::Classes::TNotifyEvent OnRButtonDown = {read=FOnRButtonDown, write=FOnRButtonDown};
	__property System::Classes::TNotifyEvent OnRButtonUp = {read=FOnRButtonUp, write=FOnRButtonUp};
	__property System::Classes::TNotifyEvent OnRButtonDblClick = {read=FOnRButtonDblClick, write=FOnRButtonDblClick};
	__property System::Classes::TNotifyEvent OnEndSession = {read=FOnEndSession, write=FOnEndSession};
	__property TRzQueryEndSessionEvent OnQueryEndSession = {read=FOnQueryEndSession, write=FOnQueryEndSession};
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word wm_TrayIconMessage = System::Word(0x2422);
}	/* namespace Rztray */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTRAY)
using namespace Rztray;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztrayHPP
