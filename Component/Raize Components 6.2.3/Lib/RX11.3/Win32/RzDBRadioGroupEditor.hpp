// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBRadioGroupEditor.pas' rev: 35.00 (Windows)

#ifndef RzdbradiogroupeditorHPP
#define RzdbradiogroupeditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.ComCtrls.hpp>
#include <System.Classes.hpp>
#include <Vcl.Dialogs.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <RzDBDesignEditors.hpp>
#include <RzRadGrp.hpp>
#include <RzPanel.hpp>
#include <RzTrkBar.hpp>
#include <Vcl.Grids.hpp>
#include <RzDBRGrp.hpp>
#include <RzEdit.hpp>
#include <Vcl.Mask.hpp>
#include <RzLabel.hpp>
#include <RzButton.hpp>
#include <RzGrids.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbradiogroupeditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBRadioGroupEditor;
class DELPHICLASS TRzDBRadioGroupEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBRadioGroupEditor : public Rzdbdesigneditors::TRzDBControlEditor
{
	typedef Rzdbdesigneditors::TRzDBControlEditor inherited;
	
protected:
	Rzdbrgrp::TRzDBRadioGroup* __fastcall RadioGroup();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall GroupStyleMenuHandler(System::TObject* Sender);
	void __fastcall VisualStyleMenuHandler(System::TObject* Sender);
	void __fastcall GradientColorStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBRadioGroupEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdbdesigneditors::TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBRadioGroupEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzDBRadioGroupEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzPanel* pnlButtons;
	Rzpanel::TRzPanel* PnlOptions;
	Rzbutton::TRzButton* btnLoad;
	Rzbutton::TRzButton* btnClear;
	Vcl::Dialogs::TOpenDialog* dlgOpen;
	Rzpanel::TRzPanel* pnlPreview;
	Rzdbrgrp::TRzDBRadioGroup* grpPreview;
	Rzpanel::TRzPanel* RzPanel1;
	Rzbutton::TRzButton* btnOk;
	Rzbutton::TRzButton* btnCancel;
	Rzgrids::TRzStringGrid* grdItemsValues;
	Rzlabel::TRzLabel* Label1;
	Rzedit::TRzEdit* edtCaption;
	Rzlabel::TRzLabel* Label2;
	Rztrkbar::TRzTrackBar* trkColumns;
	void __fastcall edtCaptionChange(System::TObject* Sender);
	void __fastcall trkColumnsChange(System::TObject* Sender);
	void __fastcall btnLoadClick(System::TObject* Sender);
	void __fastcall btnClearClick(System::TObject* Sender);
	void __fastcall trkColumnsDrawTick(Rztrkbar::TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall grdItemsValuesResize(System::TObject* Sender);
	void __fastcall grdItemsValuesClick(System::TObject* Sender);
	void __fastcall btnOkClick(System::TObject* Sender);
	
private:
	bool FUpdating;
	void __fastcall ResetGrid();
	
public:
	void __fastcall UpdateControls();
	void __fastcall UpdateGroup();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzDBRadioGroupEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzDBRadioGroupEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzDBRadioGroupEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBRadioGroupEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbradiogroupeditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBRADIOGROUPEDITOR)
using namespace Rzdbradiogroupeditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbradiogroupeditorHPP
