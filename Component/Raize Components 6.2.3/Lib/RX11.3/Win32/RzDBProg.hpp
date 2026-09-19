// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBProg.pas' rev: 35.00 (Windows)

#ifndef RzdbprogHPP
#define RzdbprogHPP

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
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzPrgres.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbprog
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS EInvalidBaseValue;
class DELPHICLASS TRzDBProgressBar;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION EInvalidBaseValue : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall EInvalidBaseValue(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EInvalidBaseValue(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidBaseValue(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidBaseValue(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidBaseValue(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidBaseValue(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EInvalidBaseValue(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EInvalidBaseValue(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidBaseValue(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidBaseValue(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidBaseValue(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidBaseValue(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EInvalidBaseValue() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzDBProgressBar : public Rzprgres::TRzCustomProgressBar
{
	typedef Rzprgres::TRzCustomProgressBar inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	double FBaseValue;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	Vcl::Dbctrls::TFieldDataLink* FBaseDataLink;
	void __fastcall DataChange(System::TObject* Sender);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall SetBaseValue(double Value);
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual System::UnicodeString __fastcall GetBaseField();
	virtual void __fastcall SetBaseField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual int __fastcall GetPercent();
	
public:
	__fastcall virtual TRzDBProgressBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBProgressBar();
	__property int Percent = {read=GetPercent, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString BaseField = {read=GetBaseField, write=SetBaseField};
	__property double BaseValue = {read=FBaseValue, write=SetBaseValue};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BackColor = {default=16777215};
	__property BackColorStop = {default=-16777201};
	__property BarColor = {default=-16777203};
	__property BarColorStop = {default=-16777190};
	__property BarStyle = {default=0};
	__property BevelWidth = {default=1};
	__property BorderColor = {default=-16777201};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=4};
	__property BorderWidth;
	__property Constraints;
	__property DragKind = {default=0};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property GradientDirection = {default=3};
	__property Height = {default=24};
	__property InteriorOffset;
	__property NumSegments = {default=20};
	__property Orientation = {default=0};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property ShowPercent = {default=1};
	__property ThemeAware = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property Width = {default=200};
	__property OnChange;
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnEndDock;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbprog */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBPROG)
using namespace Rzdbprog;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbprogHPP
