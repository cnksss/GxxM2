// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzCheckListEditor.pas' rev: 35.00 (Windows)

#ifndef RzchecklisteditorHPP
#define RzchecklisteditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Controls.hpp>
#include <System.Classes.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <RzChkLst.hpp>
#include <RzDesignEditors.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzLstBox.hpp>
#include <Vcl.Dialogs.hpp>
#include <RzButton.hpp>
#include <RzRadChk.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>
#include <RzShellDialogs.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzLabel.hpp>
#include <Vcl.ImgList.hpp>
#include <RzSpnEdt.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzchecklisteditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCheckListProperty;
class DELPHICLASS TRzCheckListEditor;
class DELPHICLASS TRzCheckListEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCheckListProperty : public Designeditors::TPropertyEditor
{
	typedef Designeditors::TPropertyEditor inherited;
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall Edit();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzCheckListProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TPropertyEditor(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzCheckListProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCheckListEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzchklst::TRzCheckList* __fastcall CheckList();
	virtual int __fastcall AlignMenuIndex();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzCheckListEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzCheckListEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCheckListEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzGroupBox* grpPreview;
	Rzchklst::TRzCheckList* lstPreview;
	Rzbutton::TRzButton* btnAdd;
	Rzbutton::TRzButton* btnEdit;
	Rzbutton::TRzButton* btnDelete;
	Rzbutton::TRzButton* btnMoveUp;
	Rzbutton::TRzButton* btnMoveDown;
	Vcl::Dialogs::TOpenDialog* dlgOpen;
	Vcl::Dialogs::TSaveDialog* dlgSave;
	Rzbutton::TRzButton* btnOK;
	Rzbutton::TRzButton* btnCancel;
	Rzpanel::TRzPanel* RzPanel1;
	Rzlabel::TRzLabel* lblImageIndex;
	Rzlabel::TRzLabel* lblDisabledIndex;
	Rzradchk::TRzCheckBox* chkItemEnabled;
	Rzradchk::TRzRadioButton* optItem;
	Rzradchk::TRzRadioButton* optGroup;
	Rzpanel::TRzToolbar* RzToolbar1;
	Vcl::Controls::TImageList* ImageList1;
	Rzbutton::TRzButton* btnLoad;
	Rzbutton::TRzButton* btnSave;
	Rzbutton::TRzButton* btnClear;
	Rzpanel::TRzSpacer* RzSpacer1;
	Rzpanel::TRzSpacer* RzSpacer2;
	Rzradchk::TRzCheckBox* chkAllowGrayed;
	Rzspnedt::TRzSpinEdit* spnImageIndex;
	Rzspnedt::TRzSpinEdit* spnDisabledIndex;
	Rzpanel::TRzSpacer* RzSpacer3;
	void __fastcall lstPreviewClick(System::TObject* Sender);
	void __fastcall chkItemEnabledClick(System::TObject* Sender);
	void __fastcall chkAllowGrayedClick(System::TObject* Sender);
	void __fastcall btnEditClick(System::TObject* Sender);
	void __fastcall btnAddClick(System::TObject* Sender);
	void __fastcall btnDeleteClick(System::TObject* Sender);
	void __fastcall btnMoveUpClick(System::TObject* Sender);
	void __fastcall btnMoveDownClick(System::TObject* Sender);
	void __fastcall btnLoadClick(System::TObject* Sender);
	void __fastcall btnClearClick(System::TObject* Sender);
	void __fastcall btnSaveClick(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall optItemClick(System::TObject* Sender);
	void __fastcall optGroupClick(System::TObject* Sender);
	void __fastcall spnImageIndexChange(System::TObject* Sender);
	void __fastcall spnDisabledIndexChange(System::TObject* Sender);
	
private:
	void __fastcall EnableButtons(bool Enable);
	void __fastcall EnableMoveButtons(int Idx);
	void __fastcall EnableItemControls();
	
public:
	void __fastcall UpdateControls();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzCheckListEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzCheckListEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzCheckListEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckListEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzchecklisteditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCHECKLISTEDITOR)
using namespace Rzchecklisteditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzchecklisteditorHPP
