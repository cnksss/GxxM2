// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzColorNamesEditor.pas' rev: 35.00 (Windows)

#ifndef RzcolornameseditorHPP
#define RzcolornameseditorHPP

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
#include <RzCmboBx.hpp>
#include <RzDesignEditors.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Grids.hpp>
#include <RzButton.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzcolornameseditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzColorNamesProperty;
class DELPHICLASS TRzColorNamesEditor;
class DELPHICLASS TRzColorNamesEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzColorNamesProperty : public Designeditors::TClassProperty
{
	typedef Designeditors::TClassProperty inherited;
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall Edit();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzColorNamesProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TClassProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzColorNamesProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzColorNamesEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzColorNamesEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzColorNamesEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzColorNamesEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzbutton::TRzButton* BtnOK;
	Rzbutton::TRzButton* BtnCancel;
	Rzbutton::TRzButton* BtnLoad;
	Rzbutton::TRzButton* BtnSave;
	Vcl::Dialogs::TOpenDialog* DlgOpen;
	Vcl::Dialogs::TSaveDialog* DlgSave;
	Rzpanel::TRzPanel* RzPanel1;
	Vcl::Grids::TStringGrid* GrdColors;
	void __fastcall GrdColorsDrawCell(System::TObject* Sender, int Col, int Row, const System::Types::TRect &Rect, Vcl::Grids::TGridDrawState State);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall FormDestroy(System::TObject* Sender);
	void __fastcall GrdColorsSelectCell(System::TObject* Sender, int Col, int Row, bool &CanSelect);
	void __fastcall BtnLoadClick(System::TObject* Sender);
	void __fastcall BtnSaveClick(System::TObject* Sender);
	void __fastcall GrdColorsSetEditText(System::TObject* Sender, int ACol, int ARow, const System::UnicodeString Value);
	
private:
	Rzcmbobx::TRzColorNames* FColorNames;
	void __fastcall SetColorNames(Rzcmbobx::TRzColorNames* Value);
	
public:
	__property Rzcmbobx::TRzColorNames* ColorNames = {read=FColorNames, write=SetColorNames};
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzColorNamesEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzColorNamesEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzColorNamesEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzColorNamesEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzcolornameseditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCOLORNAMESEDITOR)
using namespace Rzcolornameseditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzcolornameseditorHPP
