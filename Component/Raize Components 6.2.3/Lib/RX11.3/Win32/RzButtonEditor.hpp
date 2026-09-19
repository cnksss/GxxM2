// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzButtonEditor.pas' rev: 35.00 (Windows)

#ifndef RzbuttoneditorHPP
#define RzbuttoneditorHPP

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
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Buttons.hpp>
#include <RzCommon.hpp>
#include <RzSpnEdt.hpp>
#include <RzDesignEditors.hpp>
#include <RzRadChk.hpp>
#include <RzLabel.hpp>
#include <RzRadGrp.hpp>
#include <RzButton.hpp>
#include <RzPanel.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzbuttoneditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzButtonEditor;
class DELPHICLASS TRzButtonEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzButtonEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzbutton::TRzButton* __fastcall Button();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzButtonEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzButtonEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzButtonEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzGroupBox* GrpPreview;
	Rzbutton::TRzButton* BtnPreview;
	Rzbutton::TRzButton* BtnOK;
	Rzbutton::TRzButton* BtnCancel;
	Rzradgrp::TRzRadioGroup* GrpModalResult;
	Rzpanel::TRzGroupBox* GrpSize;
	Rzradchk::TRzRadioButton* OptStandardSize;
	Rzradchk::TRzRadioButton* OptLargeSize;
	Rzradchk::TRzRadioButton* OptCustomSize;
	Rzlabel::TRzLabel* LblWidth;
	Rzlabel::TRzLabel* LblHeight;
	Rzpanel::TRzGroupBox* GrpSpecial;
	Rzbutton::TRzButton* BtnOKTemplate;
	Rzbutton::TRzButton* BtnCancelTemplate;
	Rzbutton::TRzButton* BtnHelpTemplate;
	Rzpanel::TRzGroupBox* GrpKeyboard;
	Rzradchk::TRzCheckBox* ChkDefault;
	Rzradchk::TRzCheckBox* ChkCancel;
	Rzpanel::TRzGroupBox* GrpCaption;
	Rzedit::TRzEdit* EdtCaption;
	Rzlabel::TRzLabel* Label1;
	Rzlabel::TRzLabel* Label4;
	Rzlabel::TRzLabel* Label5;
	Rzbutton::TRzButton* BtnYesTemplate;
	Rzbutton::TRzButton* BtnNoTemplate;
	Rzradchk::TRzCheckBox* ChkEnabled;
	Rzspnedt::TRzSpinEdit* SpnWidth;
	Rzspnedt::TRzSpinEdit* SpnHeight;
	Rzspnedt::TRzSpinEdit* SpnModalResult;
	void __fastcall EdtCaptionChange(System::TObject* Sender);
	void __fastcall ChkDefaultClick(System::TObject* Sender);
	void __fastcall ChkCancelClick(System::TObject* Sender);
	void __fastcall ButtonSizeClick(System::TObject* Sender);
	void __fastcall ChkEnabledClick(System::TObject* Sender);
	void __fastcall GrpModalResultClick(System::TObject* Sender);
	void __fastcall BtnOKTemplateClick(System::TObject* Sender);
	void __fastcall BtnCancelTemplateClick(System::TObject* Sender);
	void __fastcall BtnHelpTemplateClick(System::TObject* Sender);
	void __fastcall BtnYesTemplateClick(System::TObject* Sender);
	void __fastcall BtnNoTemplateClick(System::TObject* Sender);
	void __fastcall SpnWidthChange(System::TObject* Sender);
	void __fastcall SpnHeightChange(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	void __fastcall EnableWidthHeight(bool Enable);
	void __fastcall UpdateWidthHeight();
	
public:
	void __fastcall UpdateControls();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzButtonEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzButtonEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzButtonEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzButtonEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzbuttoneditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBUTTONEDITOR)
using namespace Rzbuttoneditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzbuttoneditorHPP
