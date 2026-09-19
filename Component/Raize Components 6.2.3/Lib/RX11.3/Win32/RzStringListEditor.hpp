// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzStringListEditor.pas' rev: 35.00 (Windows)

#ifndef RzstringlisteditorHPP
#define RzstringlisteditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <System.IniFiles.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <System.Win.Registry.hpp>
#include <Vcl.Menus.hpp>
#include <RzPrgres.hpp>
#include <RzSpnEdt.hpp>
#include <RzBorder.hpp>
#include <RzStatus.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzButton.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzstringlisteditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzStringListProperty;
class DELPHICLASS TRzStringListEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzStringListProperty : public Designeditors::TPropertyEditor
{
	typedef Designeditors::TPropertyEditor inherited;
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall Edit();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzStringListProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TPropertyEditor(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzStringListProperty() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzStringListEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Vcl::Menus::TPopupMenu* mnuEdit;
	Vcl::Menus::TMenuItem* mnuUndo;
	Vcl::Menus::TMenuItem* mnuCut;
	Vcl::Menus::TMenuItem* mnuCopy;
	Vcl::Menus::TMenuItem* mnuPaste;
	Vcl::Menus::TMenuItem* mnuSep1;
	Vcl::Menus::TMenuItem* mnuOpen;
	Vcl::Menus::TMenuItem* mnuSave;
	Vcl::Menus::TMenuItem* mnuSep2;
	Vcl::Menus::TMenuItem* mnuIndent;
	Vcl::Dialogs::TFontDialog* dlgFont;
	Vcl::Menus::TMenuItem* mnuUnindent;
	Vcl::Menus::TMenuItem* mnuPrint;
	Vcl::Menus::TMenuItem* mnuSep3;
	Vcl::Dialogs::TPrintDialog* dlgPrint;
	Rzpanel::TRzPanel* pnlButtons;
	Rzpanel::TRzPanel* RzPanel2;
	Rzbutton::TRzButton* btnOk;
	Rzbutton::TRzButton* btnCancel;
	Rzpanel::TRzPanel* pnlWorkSpace;
	Rzedit::TRzMemo* edtStrings;
	Rzbutton::TRzButton* btnCodeEditor;
	Vcl::Menus::TMenuItem* mnuSelectAll;
	Vcl::Controls::TImageList* ImageList1;
	Rzpanel::TRzToolbar* RzToolbar1;
	Rzbutton::TRzToolButton* btnUnindent;
	Rzbutton::TRzToolButton* btnIndent;
	Rzpanel::TRzSpacer* RzSpacer1;
	Rzbutton::TRzToolButton* btnOpen;
	Rzbutton::TRzToolButton* btnSave;
	Rzpanel::TRzSpacer* RzSpacer2;
	Rzbutton::TRzToolButton* btnPrint;
	Rzpanel::TRzSpacer* RzSpacer3;
	Rzbutton::TRzToolButton* btnCut;
	Rzbutton::TRzToolButton* btnCopy;
	Rzbutton::TRzToolButton* btnPaste;
	Rzbutton::TRzToolButton* btnUndo;
	Rzpanel::TRzSpacer* RzSpacer4;
	Rzbutton::TRzToolButton* btnFont;
	Rzpanel::TRzSpacer* RzSpacer5;
	Rzbutton::TRzToolButton* btnTabSize;
	Rzpanel::TRzSpacer* RzSpacer6;
	Rzspnedt::TRzSpinEdit* spnTabSize;
	Rzpanel::TRzSpacer* RzSpacer7;
	Rzbutton::TRzToolButton* btnSetTabSize;
	Rzbutton::TRzToolButton* btnCancelTabSize;
	Vcl::Dialogs::TOpenDialog* dlgOpen;
	Vcl::Dialogs::TSaveDialog* dlgSave;
	Rzbutton::TRzToolButton* btnSelectAll;
	Rzpanel::TRzSpacer* RzSpacer8;
	Rzpanel::TRzStatusBar* RzStatusBar1;
	Rzstatus::TRzFieldStatus* stsLine;
	Rzstatus::TRzFieldStatus* stsColumn;
	Rzstatus::TRzStatusPane* stsLineCount;
	Rzstatus::TRzProgressStatus* pbrPrint;
	void __fastcall mnuSelectAllClick(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall FormDestroy(System::TObject* Sender);
	void __fastcall btnFontClick(System::TObject* Sender);
	void __fastcall btnUndoClick(System::TObject* Sender);
	void __fastcall btnCutClick(System::TObject* Sender);
	void __fastcall btnCopyClick(System::TObject* Sender);
	void __fastcall btnPasteClick(System::TObject* Sender);
	void __fastcall btnOpenClick(System::TObject* Sender);
	void __fastcall btnSaveClick(System::TObject* Sender);
	void __fastcall btnIndentClick(System::TObject* Sender);
	void __fastcall edtStringsChange(System::TObject* Sender);
	void __fastcall edtStringsKeyDown(System::TObject* Sender, System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall edtStringsKeyUp(System::TObject* Sender, System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall edtStringsClick(System::TObject* Sender);
	void __fastcall edtStringsMouseUp(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	void __fastcall btnTabSizeClick(System::TObject* Sender);
	void __fastcall btnUnindentClick(System::TObject* Sender);
	void __fastcall btnSetTabSizeClick(System::TObject* Sender);
	void __fastcall FormShow(System::TObject* Sender);
	void __fastcall btnPrintClick(System::TObject* Sender);
	void __fastcall btnCodeEditorClick(System::TObject* Sender);
	void __fastcall FormClose(System::TObject* Sender, System::Uitypes::TCloseAction &Action);
	void __fastcall btnSelectAllClick(System::TObject* Sender);
	
private:
	System::UnicodeString SingleLine;
	System::UnicodeString MultipleLines;
	System::Win::Registry::TRegIniFile* DelphiIni;
	int FTabSize;
	int FCurLine;
	int FCurCol;
	System::UnicodeString FPropName;
	bool FModified;
	int __fastcall EndOfLine(int LineNum);
	void __fastcall IndentLine(int LineNum);
	bool __fastcall UnindentLine(int LineNum);
	void __fastcall IndentLines(bool Indent);
	void __fastcall SetTabSize();
	void __fastcall EnableButtons(bool Enable);
	HIDESBASE MESSAGE void __fastcall WMGetMinMaxInfo(Winapi::Messages::TWMGetMinMaxInfo &Msg);
	
public:
	__property System::UnicodeString PropName = {read=FPropName, write=FPropName};
	void __fastcall UpdateLineColStatus();
	void __fastcall UpdateClipboardStatus();
	void __fastcall UpdateButtonStatus();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzStringListEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzStringListEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzStringListEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzStringListEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzstringlisteditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSTRINGLISTEDITOR)
using namespace Rzstringlisteditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzstringlisteditorHPP
