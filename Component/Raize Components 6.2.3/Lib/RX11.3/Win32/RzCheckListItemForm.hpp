// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzCheckListItemForm.pas' rev: 35.00 (Windows)

#ifndef RzchecklistitemformHPP
#define RzchecklistitemformHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Controls.hpp>
#include <System.Classes.hpp>
#include <RzEdit.hpp>
#include <RzButton.hpp>
#include <RzLabel.hpp>
#include <RzRadChk.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzchecklistitemform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCheckItemEditDlg;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzCheckItemEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzlabel::TRzLabel* Label1;
	Rzbutton::TRzButton* btnOK;
	Rzbutton::TRzButton* btnCancel;
	Rzedit::TRzMemo* edtItem;
	Rzradchk::TRzRadioButton* optItem;
	Rzradchk::TRzRadioButton* optGroup;
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	void __fastcall SetItem(const System::UnicodeString Item);
	System::UnicodeString __fastcall GetItem();
	
public:
	__property System::UnicodeString Item = {read=GetItem, write=SetItem};
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzCheckItemEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzCheckItemEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzCheckItemEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckItemEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzchecklistitemform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCHECKLISTITEMFORM)
using namespace Rzchecklistitemform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzchecklistitemformHPP
