// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzTabStopEditor.pas' rev: 35.00 (Windows)

#ifndef RztabstopeditorHPP
#define RztabstopeditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.Menus.hpp>
#include <RzDesignEditors.hpp>
#include <RzTrkBar.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzLstBox.hpp>
#include <RzRadChk.hpp>
#include <RzLabel.hpp>
#include <RzButton.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>
#include <RzRadGrp.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztabstopeditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzTabStopProperty;
class DELPHICLASS TRzTabbedListBoxEditor;
class DELPHICLASS TRzTabStopEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabStopProperty : public Designeditors::TPropertyEditor
{
	typedef Designeditors::TPropertyEditor inherited;
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall Edit();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzTabStopProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TPropertyEditor(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzTabStopProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabbedListBoxEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzlstbox::TRzTabbedListBox* __fastcall ListBox();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzTabbedListBoxEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzTabbedListBoxEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzTabStopEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzbutton::TRzButton* btnOK;
	Rzbutton::TRzButton* btnCancel;
	Rzpanel::TRzGroupBox* grpPreview;
	Rzpanel::TRzGroupBox* grpTabStops;
	Rzlstbox::TRzListBox* lstTabs;
	Rzlabel::TRzLabel* lblMin;
	Rzlabel::TRzLabel* lblMax;
	Rzlabel::TRzLabel* Label3;
	Rzlabel::TRzLabel* lblTabNum;
	Rzlstbox::TRzTabbedListBox* lstPreview;
	Rzbutton::TRzButton* btnAdd;
	Rzbutton::TRzButton* btnDelete;
	Rzradchk::TRzCheckBox* chkRightAligned;
	Rztrkbar::TRzTrackBar* trkTabPos;
	Rzradgrp::TRzRadioGroup* grpTabStopsMode;
	Rzpanel::TRzPanel* RzPanel1;
	void __fastcall btnAddClick(System::TObject* Sender);
	void __fastcall btnDeleteClick(System::TObject* Sender);
	void __fastcall lstTabsClick(System::TObject* Sender);
	void __fastcall FormShow(System::TObject* Sender);
	void __fastcall trkTabPosChange(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall chkRightAlignedClick(System::TObject* Sender);
	void __fastcall grpTabStopsModeClick(System::TObject* Sender);
	
private:
	bool FUpdating;
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzTabStopEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzTabStopEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzTabStopEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabStopEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rztabstopeditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTABSTOPEDITOR)
using namespace Rztabstopeditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztabstopeditorHPP
