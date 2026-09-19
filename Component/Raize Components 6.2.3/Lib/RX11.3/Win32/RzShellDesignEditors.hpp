// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellDesignEditors.pas' rev: 35.00 (Windows)

#ifndef RzshelldesigneditorsHPP
#define RzshelldesigneditorsHPP

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
#include <Vcl.Menus.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ComCtrls.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <VCLEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <RzShellDialogs.hpp>
#include <RzLabel.hpp>
#include <RzShellCtrls.hpp>
#include <RzShellUtils.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzCmboBx.hpp>
#include <RzRadChk.hpp>
#include <RzButton.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzshelldesigneditors
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzShellLocatorProperty;
class DELPHICLASS TCSIDLProperty;
class DELPHICLASS TRzShellDialogEditor;
class DELPHICLASS TRzShellListEditor;
class DELPHICLASS TRzShellTreeEditor;
class DELPHICLASS TRzShellComboEditor;
class DELPHICLASS TRzShellLocatorEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellLocatorProperty : public Designeditors::TClassProperty
{
	typedef Designeditors::TClassProperty inherited;
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual void __fastcall Edit();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzShellLocatorProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TClassProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzShellLocatorProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TCSIDLProperty : public Designeditors::TEnumProperty
{
	typedef Designeditors::TEnumProperty inherited;
	
public:
	virtual void __fastcall GetValues(System::Classes::TGetStrProc Proc);
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TCSIDLProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TEnumProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TCSIDLProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellDialogEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzShellDialogEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzShellDialogEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellListEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzshellctrls::TRzShellList* __fastcall ShellList();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ViewStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzShellListEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzShellListEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellTreeEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzshellctrls::TRzShellTree* __fastcall ShellTree();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ShellListMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzShellTreeEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzShellTreeEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellComboEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzshellctrls::TRzShellCombo* __fastcall ShellCombo();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ShellListMenuHandler(System::TObject* Sender);
	void __fastcall ShellTreeMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzShellComboEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzShellComboEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzShellLocatorEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzbutton::TRzButton* OkBtn;
	Rzbutton::TRzButton* CancelBtn;
	Rzradchk::TRzRadioButton* UsePidlRdo;
	Rzradchk::TRzRadioButton* DontUseAnythingRdo;
	Rzradchk::TRzRadioButton* UsePathnameRdo;
	Rzradchk::TRzRadioButton* UseCSIDLRdo;
	Rzshelldialogs::TRzSelectFolderDialog* RzSelectFolderDialog1;
	Rzcmbobx::TRzComboBox* ComboBox1;
	Rzedit::TRzEdit* PathNameEdt;
	Rzlabel::TRzLabel* ItemTxt;
	Rzbutton::TRzButton* BrowseBtn;
	void __fastcall UsePidlRdoClick(System::TObject* Sender);
	void __fastcall FormClose(System::TObject* Sender, System::Uitypes::TCloseAction &Action);
	void __fastcall BrowseBtnClick(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	Rzshellctrls::TRzShellLocator* FLocator;
	void __fastcall SetLocator(Rzshellctrls::TRzShellLocator* Value);
	
public:
	__property Rzshellctrls::TRzShellLocator* Locator = {read=FLocator, write=SetLocator};
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzShellLocatorEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzShellLocatorEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzShellLocatorEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzShellLocatorEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::UnicodeString __fastcall GetCSIDLStr(Rzshellutils::TCSIDL id);
}	/* namespace Rzshelldesigneditors */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLDESIGNEDITORS)
using namespace Rzshelldesigneditors;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshelldesigneditorsHPP
