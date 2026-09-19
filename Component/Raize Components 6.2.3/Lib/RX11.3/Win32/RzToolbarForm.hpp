// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzToolbarForm.pas' rev: 35.00 (Windows)

#ifndef RztoolbarformHPP
#define RztoolbarformHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzPanel.hpp>
#include <RzLabel.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <RzSpnEdt.hpp>
#include <RzBorder.hpp>
#include <RzRadGrp.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzButton.hpp>
#include <RzLstBox.hpp>
#include <RzRadChk.hpp>
#include <Vcl.ExtDlgs.hpp>
#include <RzCmboBx.hpp>
#include <RzChkLst.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztoolbarform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzFrmCustomizeToolbar;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzFrmCustomizeToolbar : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzbutton::TRzButton* BtnClose;
	Rzchklst::TRzCheckList* LstControls;
	Vcl::Extctrls::TTimer* Timer1;
	Vcl::Buttons::TSpeedButton* BtnSpacer;
	Vcl::Buttons::TSpeedButton* BtnGrooveSpacer;
	Rzlabel::TRzLabel* LblHint;
	Rzlabel::TRzLabel* LblTextOptions;
	Rzcmbobx::TRzComboBox* CbxTextOptions;
	Rzbutton::TRzButton* BtnMoveUp;
	Rzbutton::TRzButton* BtnMoveDown;
	void __fastcall BtnMoveDownClick(System::TObject* Sender);
	void __fastcall BtnMoveUpClick(System::TObject* Sender);
	void __fastcall LstControlsClick(System::TObject* Sender);
	void __fastcall LstControlsDragDrop(System::TObject* Sender, System::TObject* Source, int X, int Y);
	void __fastcall LstControlsDragOver(System::TObject* Sender, System::TObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	void __fastcall ListBoxDrawItem(Vcl::Controls::TWinControl* Control, int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	void __fastcall LstControlsEndDrag(System::TObject* Sender, System::TObject* Target, int X, int Y);
	void __fastcall LstControlsMouseDown(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	void __fastcall Timer1Timer(System::TObject* Sender);
	void __fastcall CbxTextOptionsClick(System::TObject* Sender);
	void __fastcall LstControlsChange(System::TObject* Sender, int Index, Vcl::Stdctrls::TCheckBoxState NewState);
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	int OldIdx;
	int NewIdx;
	bool GoingUp;
	void __fastcall EnableMoveButtons(int Idx);
	
public:
	Rzbutton::TRzToolbarButton* SelectedBtn;
	System::Classes::TComponent* CompOwner;
	Rzpanel::TRzToolbar* Toolbar;
	void __fastcall UpdateControls(Rzpanel::TRzCustomizeCaptions* CustomizeCaptions, bool ShowTextOptions);
	void __fastcall Reposition();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzFrmCustomizeToolbar(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzFrmCustomizeToolbar(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzFrmCustomizeToolbar() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzFrmCustomizeToolbar(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rztoolbarform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTOOLBARFORM)
using namespace Rztoolbarform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztoolbarformHPP
