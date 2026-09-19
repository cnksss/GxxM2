// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzComboBoxEditor.pas' rev: 35.00 (Windows)

#ifndef RzcomboboxeditorHPP
#define RzcomboboxeditorHPP

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
#include <VCLEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <RzPanel.hpp>
#include <Vcl.Grids.hpp>
#include <RzCmboBx.hpp>
#include <RzButton.hpp>
#include <RzGrids.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzcomboboxeditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzComboBoxEditor;
class DELPHICLASS TRzComboBoxTextProperty;
class DELPHICLASS TRzComboBoxEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzComboBoxEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzcmbobx::TRzComboBox* __fastcall ComboBox();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzComboBoxEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzComboBoxEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzComboBoxTextProperty : public Vcleditors::TCaptionProperty
{
	typedef Vcleditors::TCaptionProperty inherited;
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual void __fastcall GetValues(System::Classes::TGetStrProc Proc);
	virtual void __fastcall SetValue(const System::UnicodeString Value)/* overload */;
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzComboBoxTextProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Vcleditors::TCaptionProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzComboBoxTextProperty() { }
	
	/* Hoisted overloads: */
	
public:
	inline void __fastcall  SetValue(const System::WideString Value){ Designeditors::TPropertyEditor::SetValue(Value); }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzComboBoxEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzPanel* PnlButtons;
	Rzbutton::TRzButton* btnLoad;
	Rzbutton::TRzButton* btnClear;
	Vcl::Dialogs::TOpenDialog* dlgOpen;
	Rzpanel::TRzPanel* RzPanel1;
	Rzbutton::TRzButton* btnOk;
	Rzbutton::TRzButton* btnCancel;
	Rzpanel::TRzPanel* pnlClientArea;
	Rzgrids::TRzStringGrid* grdItemsValues;
	void __fastcall btnLoadClick(System::TObject* Sender);
	void __fastcall btnClearClick(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall grdItemsValuesResize(System::TObject* Sender);
	
private:
	void __fastcall ResetGrid();
	
public:
	void __fastcall UpdateUI(Rzcmbobx::TRzComboBox* ComboBox);
	void __fastcall UpdateCombo(Rzcmbobx::TRzComboBox* ComboBox);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzComboBoxEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzComboBoxEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzComboBoxEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzComboBoxEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzcomboboxeditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCOMBOBOXEDITOR)
using namespace Rzcomboboxeditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzcomboboxeditorHPP
