// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBLook.pas' rev: 35.00 (Windows)

#ifndef RzdblookHPP
#define RzdblookHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <Data.DB.hpp>
#include <RzCommon.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Grids.hpp>
#include <Vcl.DBGrids.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzDBNav.hpp>
#include <System.UITypes.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdblook
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS EInvalidSearchField;
class DELPHICLASS EInvalidDataset;
class DELPHICLASS TRzDBLookupDialog;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION EInvalidSearchField : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall EInvalidSearchField(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EInvalidSearchField(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidSearchField(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidSearchField(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidSearchField(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidSearchField(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EInvalidSearchField(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EInvalidSearchField(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidSearchField(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidSearchField(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidSearchField(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidSearchField(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EInvalidSearchField() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EInvalidDataset : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall EInvalidDataset(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EInvalidDataset(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidDataset(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidDataset(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidDataset(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidDataset(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EInvalidDataset(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EInvalidDataset(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidDataset(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidDataset(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidDataset(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidDataset(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EInvalidDataset() { }
	
};

#pragma pack(pop)

typedef void __fastcall (__closure *TRzDrawColumnCellEvent)(Vcl::Dbgrids::TDBGrid* Grid, const System::Types::TRect &Rect, int DataCol, Vcl::Dbgrids::TColumn* Column, Vcl::Grids::TGridDrawState State);

typedef void __fastcall (__closure *TRzDrawDataCellEvent)(Vcl::Dbgrids::TDBGrid* Grid, const System::Types::TRect &Rect, Data::Db::TField* Field, Vcl::Grids::TGridDrawState State);

enum DECLSPEC_DENUM TRzDBLookupSearchMethod : unsigned char { smLocate, smFilter };

class PASCALIMPLEMENTATION TRzDBLookupDialog : public Rzcommon::TRzDialogComponent
{
	typedef Rzcommon::TRzDialogComponent inherited;
	
private:
	bool FAutoSelect;
	bool FButtonGlyphs;
	System::Uitypes::TEditCharCase FCharCase;
	Data::Db::TDataSet* FDataset;
	Vcl::Dbgrids::TDBGridOptions FGridOptions;
	System::UnicodeString FPrompt;
	bool FMoveSrchFldToFront;
	bool FNumbersOnly;
	System::UnicodeString FKeyField;
	System::UnicodeString FSearchField;
	Vcl::Stdctrls::TCustomEdit* FSearchEdit;
	TRzDBLookupSearchMethod FSearchMethod;
	System::UnicodeString FSearchString;
	bool FShowNavigator;
	bool FShowNavigatorHints;
	int FNavigatorWidth;
	Rzdbnav::TRzNavigatorButtons FVisibleNavButtons;
	bool FUpdateSearchEdit;
	TRzDrawColumnCellEvent FOnDrawColumnCell;
	TRzDrawDataCellEvent FOnDrawDataCell;
	void __fastcall DrawColumnCellHandler(System::TObject* Sender, const System::Types::TRect &Rect, int DataCol, Vcl::Dbgrids::TColumn* Column, Vcl::Grids::TGridDrawState State);
	void __fastcall DrawDataCellHandler(System::TObject* Sender, const System::Types::TRect &Rect, Data::Db::TField* Field, Vcl::Grids::TGridDrawState State);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	
public:
	__fastcall virtual TRzDBLookupDialog(System::Classes::TComponent* AOwner);
	bool __fastcall Execute();
	__property System::UnicodeString SearchString = {read=FSearchString, write=FSearchString};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AutoSelect = {read=FAutoSelect, write=FAutoSelect, default=1};
	__property bool ButtonGlyphs = {read=FButtonGlyphs, write=FButtonGlyphs, default=0};
	__property System::Uitypes::TEditCharCase CharCase = {read=FCharCase, write=FCharCase, default=0};
	__property Data::Db::TDataSet* Dataset = {read=FDataset, write=FDataset};
	__property Vcl::Dbgrids::TDBGridOptions GridOptions = {read=FGridOptions, write=FGridOptions, default=3325};
	__property bool MoveSearchField = {read=FMoveSrchFldToFront, write=FMoveSrchFldToFront, default=1};
	__property bool NumbersOnly = {read=FNumbersOnly, write=FNumbersOnly, default=0};
	__property System::UnicodeString Prompt = {read=FPrompt, write=FPrompt};
	__property System::UnicodeString KeyField = {read=FKeyField, write=FKeyField};
	__property int NavigatorWidth = {read=FNavigatorWidth, write=FNavigatorWidth, default=221};
	__property Vcl::Stdctrls::TCustomEdit* SearchEdit = {read=FSearchEdit, write=FSearchEdit};
	__property System::UnicodeString SearchField = {read=FSearchField, write=FSearchField};
	__property TRzDBLookupSearchMethod SearchMethod = {read=FSearchMethod, write=FSearchMethod, nodefault};
	__property bool ShowNavigator = {read=FShowNavigator, write=FShowNavigator, default=1};
	__property bool ShowNavigatorHints = {read=FShowNavigatorHints, write=FShowNavigatorHints, default=0};
	__property bool UpdateSearchEdit = {read=FUpdateSearchEdit, write=FUpdateSearchEdit, default=0};
	__property Rzdbnav::TRzNavigatorButtons VisibleNavButtons = {read=FVisibleNavButtons, write=FVisibleNavButtons, default=1023};
	__property TRzDrawColumnCellEvent OnDrawColumnCell = {read=FOnDrawColumnCell, write=FOnDrawColumnCell};
	__property TRzDrawDataCellEvent OnDrawDataCell = {read=FOnDrawDataCell, write=FOnDrawDataCell};
	__property BorderStyle = {default=2};
	__property Caption = {default=0};
	__property CaptionOK = {default=0};
	__property CaptionCancel = {default=0};
	__property CaptionHelp = {default=0};
	__property Font;
	__property FrameColor = {default=-16777200};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property Height = {default=300};
	__property HelpContext = {default=0};
	__property Width = {default=350};
public:
	/* TRzDialogComponent.Destroy */ inline __fastcall virtual ~TRzDBLookupDialog() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdblook */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBLOOK)
using namespace Rzdblook;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdblookHPP
