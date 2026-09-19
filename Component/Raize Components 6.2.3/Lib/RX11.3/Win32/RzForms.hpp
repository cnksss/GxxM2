// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzForms.pas' rev: 35.00 (Windows)

#ifndef RzformsHPP
#define RzformsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <System.Win.Registry.hpp>
#include <Vcl.Graphics.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzforms
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzFormState;
class DELPHICLASS TRzFormShape;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzFormState : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FEnabled;
	Rzcommon::TRzRegIniFile* FRegIniFile;
	System::UnicodeString FSection;
	System::Classes::TNotifyEvent FOrigCreateHandler;
	System::Classes::TNotifyEvent FOrigShowHandler;
	Vcl::Forms::TCloseEvent FOrigCloseHandler;
	System::Classes::TNotifyEvent FOnSaveState;
	System::Classes::TNotifyEvent FOnRestoreState;
	void __fastcall FormCreateHandler(System::TObject* Sender);
	void __fastcall FormShowHandler(System::TObject* Sender);
	void __fastcall FormCloseHandler(System::TObject* Sender, System::Uitypes::TCloseAction &Action);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	System::UnicodeString __fastcall GetSection();
	bool __fastcall ValidMonitorAtPoint(int X, int Y);
	virtual void __fastcall SetRegIniFile(Rzcommon::TRzRegIniFile* Value);
	
public:
	__fastcall virtual TRzFormState(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFormState();
	virtual void __fastcall ChainEvents();
	virtual void __fastcall UnchainEvents();
	virtual void __fastcall SaveState();
	virtual void __fastcall RestoreState();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool Enabled = {read=FEnabled, write=FEnabled, default=1};
	__property System::UnicodeString Section = {read=FSection, write=FSection};
	__property Rzcommon::TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
	__property System::Classes::TNotifyEvent OnSaveState = {read=FOnSaveState, write=FOnSaveState};
	__property System::Classes::TNotifyEvent OnRestoreState = {read=FOnRestoreState, write=FOnRestoreState};
};


class PASCALIMPLEMENTATION TRzFormShape : public Vcl::Extctrls::TImage
{
	typedef Vcl::Extctrls::TImage inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAllowFormDrag;
	bool FInitializedRegion;
	System::Classes::TNotifyEvent FOnFormShow;
	void __fastcall FormShowHandler(System::TObject* Sender);
	
protected:
	void __fastcall InitRegion();
	virtual HRGN __fastcall GetRegionFromBitmap(Vcl::Graphics::TBitmap* B);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	
public:
	__fastcall virtual TRzFormShape(System::Classes::TComponent* AOwner);
	void __fastcall RecreateRegion();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowFormDrag = {read=FAllowFormDrag, write=FAllowFormDrag, default=1};
	__property System::Classes::TNotifyEvent OnFormShow = {read=FOnFormShow, write=FOnFormShow};
	__property Align = {default=5};
	__property Transparent = {default=1};
public:
	/* TImage.Destroy */ inline __fastcall virtual ~TRzFormShape() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzforms */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZFORMS)
using namespace Rzforms;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzformsHPP
