// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLookupForm.pas' rev: 35.00 (Windows)

#ifndef RzlookupformHPP
#define RzlookupformHPP

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
#include <Vcl.Buttons.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzLstBox.hpp>
#include <RzDlgBtn.hpp>
#include <RzPanel.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlookupform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzLookupForm;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzLookupForm : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Vcl::Extctrls::TPanel* PnlSelections;
	Rzlstbox::TRzListBox* LstSelections;
	Vcl::Extctrls::TPanel* PnlPrompt;
	Vcl::Extctrls::TPanel* PnlSearch;
	Rzedit::TRzEdit* EdtSearch;
	Rzdlgbtn::TRzDialogButtons* PnlButtons;
	void __fastcall EdtSearchChange(System::TObject* Sender);
	void __fastcall FormResize(System::TObject* Sender);
	void __fastcall FormShow(System::TObject* Sender);
	void __fastcall PnlButtonsClickHelp(System::TObject* Sender);
	void __fastcall EdtSearchKeyDown(System::TObject* Sender, System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	int __fastcall FindClosest(System::UnicodeString S);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzLookupForm(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzLookupForm(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzLookupForm() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzLookupForm(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzlookupform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLOOKUPFORM)
using namespace Rzlookupform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlookupformHPP
