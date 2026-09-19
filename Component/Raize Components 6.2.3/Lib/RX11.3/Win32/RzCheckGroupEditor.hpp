// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzCheckGroupEditor.pas' rev: 35.00 (Windows)

#ifndef RzcheckgroupeditorHPP
#define RzcheckgroupeditorHPP

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
#include <RzRadGrp.hpp>
#include <RzPanel.hpp>
#include <RzTrkBar.hpp>
#include <RzEdit.hpp>
#include <Vcl.Mask.hpp>
#include <RzLabel.hpp>
#include <RzButton.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzcheckgroupeditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCheckGroupEditor;
class DELPHICLASS TRzCheckGroupEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCheckGroupEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	virtual Rzradgrp::TRzCheckGroup* __fastcall CheckGroup();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ItemsMenuHandler(System::TObject* Sender);
	void __fastcall GroupStyleMenuHandler(System::TObject* Sender);
	void __fastcall VisualStyleMenuHandler(System::TObject* Sender);
	void __fastcall GradientColorStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzCheckGroupEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzCheckGroupEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCheckGroupEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzPanel* PnlButtons;
	Rzpanel::TRzPanel* PnlOptions;
	Rzedit::TRzEdit* EdtCaption;
	Rzlabel::TRzLabel* Label1;
	Rzedit::TRzMemo* EdtItems;
	Rzlabel::TRzLabel* Label2;
	Rzbutton::TRzButton* BtnLoad;
	Rzlabel::TRzLabel* Label3;
	Rzbutton::TRzButton* BtnClear;
	Vcl::Dialogs::TOpenDialog* DlgOpen;
	Rztrkbar::TRzTrackBar* TrkColumns;
	Rzpanel::TRzPanel* PnlPreview;
	Rzradgrp::TRzCheckGroup* GrpPreview;
	Rzpanel::TRzPanel* RzPanel1;
	Rzbutton::TRzButton* BtnOk;
	Rzbutton::TRzButton* BtnCancel;
	void __fastcall EdtCaptionChange(System::TObject* Sender);
	void __fastcall TrkColumnsChange(System::TObject* Sender);
	void __fastcall EdtItemsChange(System::TObject* Sender);
	void __fastcall BtnLoadClick(System::TObject* Sender);
	void __fastcall BtnClearClick(System::TObject* Sender);
	void __fastcall TrkColumnsDrawTick(Rztrkbar::TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	bool FUpdating;
	
public:
	void __fastcall UpdateControls();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzCheckGroupEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzCheckGroupEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzCheckGroupEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckGroupEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzcheckgroupeditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCHECKGROUPEDITOR)
using namespace Rzcheckgroupeditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzcheckgroupeditorHPP
