// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzToolbarPrefixForm.pas' rev: 35.00 (Windows)

#ifndef RztoolbarprefixformHPP
#define RztoolbarprefixformHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <RzButton.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzRadChk.hpp>
#include <RzLabel.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztoolbarprefixform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzFrmPrefixSuffix;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzFrmPrefixSuffix : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzlabel::TRzLabel* LblPrefix;
	Rzradchk::TRzRadioButton* OptPrefix;
	Rzradchk::TRzRadioButton* OptSuffix;
	Rzedit::TRzEdit* EdtPrefix;
	Rzbutton::TRzButton* BtnOK;
	Rzbutton::TRzButton* BtnCancel;
	void __fastcall OptPrefixClick(System::TObject* Sender);
	void __fastcall OptSuffixClick(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzFrmPrefixSuffix(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzFrmPrefixSuffix(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzFrmPrefixSuffix() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzFrmPrefixSuffix(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rztoolbarprefixform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTOOLBARPREFIXFORM)
using namespace Rztoolbarprefixform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztoolbarprefixformHPP
