// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzCaptionEditor.pas' rev: 35.00 (Windows)

#ifndef RzcaptioneditorHPP
#define RzcaptioneditorHPP

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
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzDlgBtn.hpp>
#include <RzEdit.hpp>
#include <RzPanel.hpp>
#include <RzButton.hpp>
#include <RzRadChk.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzcaptioneditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCaptionProperty;
class DELPHICLASS TRzHintProperty;
class DELPHICLASS TRzGroupItemCaptionProperty;
class DELPHICLASS TRzGroupTemplateItemCaptionProperty;
class DELPHICLASS TRzCaptionEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCaptionProperty : public Designeditors::TStringProperty
{
	typedef Designeditors::TStringProperty inherited;
	
public:
	virtual void __fastcall Edit();
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzCaptionProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TStringProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzCaptionProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzHintProperty : public Designeditors::TStringProperty
{
	typedef Designeditors::TStringProperty inherited;
	
public:
	virtual void __fastcall Edit();
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzHintProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TStringProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzHintProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupItemCaptionProperty : public Designeditors::TStringProperty
{
	typedef Designeditors::TStringProperty inherited;
	
public:
	virtual void __fastcall Edit();
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzGroupItemCaptionProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TStringProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzGroupItemCaptionProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupTemplateItemCaptionProperty : public Designeditors::TStringProperty
{
	typedef Designeditors::TStringProperty inherited;
	
public:
	virtual void __fastcall Edit();
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzGroupTemplateItemCaptionProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TStringProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzGroupTemplateItemCaptionProperty() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCaptionEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzedit::TRzMemo* EdtCaption;
	Rzpanel::TRzPanel* PnlEditor;
	Rzdlgbtn::TRzDialogButtons* RzDialogButtons1;
	void __fastcall FormClose(System::TObject* Sender, System::Uitypes::TCloseAction &Action);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall EdtCaptionKeyPress(System::TObject* Sender, System::WideChar &Key);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzCaptionEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzCaptionEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzCaptionEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCaptionEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE void __fastcall CaptionToStringList(const System::UnicodeString Caption, System::Classes::TStrings* Lines);
extern DELPHI_PACKAGE void __fastcall StringListToCaption(System::Classes::TStrings* Lines, System::UnicodeString &Caption);
}	/* namespace Rzcaptioneditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCAPTIONEDITOR)
using namespace Rzcaptioneditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzcaptioneditorHPP
