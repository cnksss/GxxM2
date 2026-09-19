// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBComboBoxEditor.pas' rev: 35.00 (Windows)

#ifndef RzdbcomboboxeditorHPP
#define RzdbcomboboxeditorHPP

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
#include <RzDBDesignEditors.hpp>
#include <RzPanel.hpp>
#include <Vcl.Grids.hpp>
#include <RzDBCmbo.hpp>
#include <RzButton.hpp>
#include <RzGrids.hpp>
#include <RzDesignEditors.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbcomboboxeditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBComboBoxEditor;
class DELPHICLASS TRzDBComboBoxEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBComboBoxEditor : public Rzdbdesigneditors::TRzDBControlEditor
{
	typedef Rzdbdesigneditors::TRzDBControlEditor inherited;
	
protected:
	Rzdbcmbo::TRzDBComboBox* __fastcall ComboBox();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBComboBoxEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdbdesigneditors::TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBComboBoxEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzDBComboBoxEditDlg : public Vcl::Forms::TForm
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
	void __fastcall UpdateUI(Rzdbcmbo::TRzDBComboBox* ComboBox);
	void __fastcall UpdateCombo(Rzdbcmbo::TRzDBComboBox* ComboBox);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzDBComboBoxEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzDBComboBoxEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzDBComboBoxEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBComboBoxEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbcomboboxeditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBCOMBOBOXEDITOR)
using namespace Rzdbcomboboxeditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbcomboboxeditorHPP
