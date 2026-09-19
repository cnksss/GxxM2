// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLaunch.pas' rev: 35.00 (Windows)

#ifndef RzlaunchHPP
#define RzlaunchHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlaunch
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzLaunchThread;
class DELPHICLASS TRzLauncher;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TShowMode : unsigned char { smNormal, smMaximized, smMinimized, smHide };

class PASCALIMPLEMENTATION TRzLaunchThread : public System::Classes::TThread
{
	typedef System::Classes::TThread inherited;
	
private:
	TRzLauncher* FLauncher;
	
protected:
	virtual void __fastcall Execute();
	
public:
	__fastcall TRzLaunchThread(TRzLauncher* ALauncher);
public:
	/* TThread.Destroy */ inline __fastcall virtual ~TRzLaunchThread() { }
	
};


typedef void __fastcall (__closure *TRzLaunchErrorEvent)(System::TObject* Sender, unsigned ErrorCode);

enum DECLSPEC_DENUM TRzWaitType : unsigned char { wtFullStop, wtProcessMessages };

class PASCALIMPLEMENTATION TRzLauncher : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	NativeUInt FHInstance;
	System::UnicodeString FAction;
	System::UnicodeString FFileName;
	System::UnicodeString FParameters;
	TShowMode FShowMode;
	System::UnicodeString FStartDir;
	int FTimeout;
	TRzWaitType FWaitType;
	bool FWaitUntilFinished;
	System::Classes::TNotifyEvent FOnFinished;
	System::Classes::TNotifyEvent FOnTimeout;
	TRzLaunchErrorEvent FOnError;
	unsigned FExitCode;
	unsigned FLastErrorCode;
	NativeUInt FHProcess;
	bool FRunning;
	TRzLaunchThread* FBackgroundThread;
	
protected:
	void __fastcall StartProcess();
	DYNAMIC void __fastcall Finished();
	DYNAMIC void __fastcall DoTimeout();
	DYNAMIC void __fastcall LaunchError();
	virtual void __fastcall WaitForProcessAndProcessMsgs();
	virtual void __fastcall WaitForProcessFullStop();
	virtual void __fastcall WaitForProcessFromThread();
	
public:
	__fastcall virtual TRzLauncher(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzLauncher();
	virtual void __fastcall Launch();
	virtual unsigned __fastcall Execute();
	System::UnicodeString __fastcall GetErrorMsg(unsigned ErrorCode);
	HWND __fastcall GetWindowHandle();
	__property NativeUInt HInstance = {read=FHInstance, nodefault};
	__property unsigned ExitCode = {read=FExitCode, nodefault};
	__property NativeUInt HProcess = {read=FHProcess, nodefault};
	__property bool Running = {read=FRunning, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString Action = {read=FAction, write=FAction};
	__property System::UnicodeString FileName = {read=FFileName, write=FFileName};
	__property System::UnicodeString Parameters = {read=FParameters, write=FParameters};
	__property TShowMode ShowMode = {read=FShowMode, write=FShowMode, default=0};
	__property System::UnicodeString StartDir = {read=FStartDir, write=FStartDir};
	__property int Timeout = {read=FTimeout, write=FTimeout, nodefault};
	__property TRzWaitType WaitType = {read=FWaitType, write=FWaitType, default=0};
	__property bool WaitUntilFinished = {read=FWaitUntilFinished, write=FWaitUntilFinished, default=0};
	__property System::Classes::TNotifyEvent OnFinished = {read=FOnFinished, write=FOnFinished};
	__property TRzLaunchErrorEvent OnError = {read=FOnError, write=FOnError};
	__property System::Classes::TNotifyEvent OnTimeout = {read=FOnTimeout, write=FOnTimeout};
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::StaticArray<int, 4> ShowWindowModes;
}	/* namespace Rzlaunch */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLAUNCH)
using namespace Rzlaunch;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlaunchHPP
