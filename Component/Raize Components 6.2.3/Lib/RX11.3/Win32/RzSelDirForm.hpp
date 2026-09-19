// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSelDirForm.pas' rev: 35.00 (Windows)

#ifndef RzseldirformHPP
#define RzseldirformHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.UITypes.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.FileCtrl.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzFilSys.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <Vcl.ComCtrls.hpp>
#include <RzTreeVw.hpp>
#include <RzPanel.hpp>
#include <RzDlgBtn.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzseldirform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzSelDirForm;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzSelDirForm : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzPanel* PnlFolders;
	Rzpanel::TRzPanel* PnlPrompt;
	Rzpanel::TRzPanel* PnlDrives;
	Vcl::Stdctrls::TLabel* LblDrives;
	Vcl::Stdctrls::TLabel* LblDir;
	Rzedit::TRzEdit* EdtDir;
	Vcl::Stdctrls::TLabel* LblFolders;
	Vcl::Stdctrls::TLabel* LblPrompt;
	Rzdlgbtn::TRzDialogButtons* PnlButtons;
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall EdtDirEnter(System::TObject* Sender);
	void __fastcall EdtDirExit(System::TObject* Sender);
	void __fastcall PnlButtonsClickOk(System::TObject* Sender);
	void __fastcall PnlButtonsClickHelp(System::TObject* Sender);
	
private:
	System::UnicodeString __fastcall GetDirectory();
	void __fastcall SetDirectory(System::UnicodeString Value);
	void __fastcall TvwDirsChange(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node);
	
public:
	Rzfilsys::TRzDirectoryTree* TvwDirs;
	bool AllowCreate;
	__property System::UnicodeString Directory = {read=GetDirectory, write=SetDirectory};
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzSelDirForm(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzSelDirForm(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzSelDirForm() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSelDirForm(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzseldirform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSELDIRFORM)
using namespace Rzseldirform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzseldirformHPP
