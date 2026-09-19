// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBLookupForm.pas' rev: 35.00 (Windows)

#ifndef RzdblookupformHPP
#define RzdblookupformHPP

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
#include <Vcl.Grids.hpp>
#include <Vcl.DBGrids.hpp>
#include <Data.DB.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzPanel.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzDlgBtn.hpp>
#include <RzDBNav.hpp>
#include <RzDBGrid.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdblookupform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBLookupForm;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBLookupForm : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Data::Db::TDataSource* SrcLookup;
	Vcl::Extctrls::TPanel* PnlPrompt;
	Vcl::Stdctrls::TLabel* LblPrompt;
	Vcl::Extctrls::TPanel* PnlNavigator;
	Rzedit::TRzEdit* EdtSearch;
	Rzpanel::TRzPanel* PnlLookup;
	Rzdbgrid::TRzDBGrid* GrdLookup;
	Rzdlgbtn::TRzDialogButtons* PnlButtons;
	Rzdbnav::TRzDBNavigator* NavLookup;
	void __fastcall EdtSearchChange(System::TObject* Sender);
	void __fastcall FormShow(System::TObject* Sender);
	void __fastcall FormResize(System::TObject* Sender);
	void __fastcall EdtSearchKeyPress(System::TObject* Sender, System::WideChar &Key);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall PnlButtonsClickHelp(System::TObject* Sender);
	void __fastcall EdtSearchKeyDown(System::TObject* Sender, System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall GrdLookupDblClick(System::TObject* Sender);
	
public:
	System::UnicodeString SearchField;
	System::UnicodeString KeyField;
	bool NumbersOnly;
	bool UseFilter;
	bool FilterOnStringField;
	void __fastcall FindKeyField(const System::UnicodeString KeyValue);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzDBLookupForm(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzDBLookupForm(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzDBLookupForm() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBLookupForm(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdblookupform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBLOOKUPFORM)
using namespace Rzdblookupform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdblookupformHPP
