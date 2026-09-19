// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellFolderForm.pas' rev: 35.00 (Windows)

#ifndef RzshellfolderformHPP
#define RzshellfolderformHPP

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
#include <Vcl.ComCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <Winapi.ShlObj.hpp>
#include <RzCommon.hpp>
#include <RzTreeVw.hpp>
#include <RzShellIntf.hpp>
#include <RzShellCtrls.hpp>
#include <RzShellDialogs.hpp>
#include <RzPanel.hpp>
#include <Vcl.ImgList.hpp>
#include <RzButton.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzshellfolderform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzSelectFolderForm;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzSelectFolderForm : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzshellctrls::TRzShellTree* ShellTree;
	Vcl::Extctrls::TPanel* ButtonPanel;
	Rzbutton::TRzBitBtn* btnOk;
	Rzbutton::TRzBitBtn* btnCancel;
	Rzpanel::TRzStatusBar* RzStatusBar1;
	Rzbutton::TRzToolButton* btnCreateFolder;
	Rzbutton::TRzToolButton* btnDeleteFolder;
	Vcl::Controls::TImageList* ImageList1;
	void __fastcall FormResize(System::TObject* Sender);
	void __fastcall ShellTreeChange(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall CreateBtnClick(System::TObject* Sender);
	void __fastcall DeleteBtnClick(System::TObject* Sender);
	
private:
	System::UnicodeString __fastcall GetSelectedPathname();
	System::UnicodeString __fastcall GetStatus();
	bool __fastcall GetOkEnabled();
	void __fastcall SetSelectedPathname(const System::UnicodeString Value);
	void __fastcall SetStatus(const System::UnicodeString Value);
	void __fastcall SetOkEnabled(bool Value);
	HIDESBASE MESSAGE void __fastcall WMGetMinMaxInfo(Winapi::Messages::TWMGetMinMaxInfo &Msg);
	
protected:
	bool FExecuting;
	Rzshelldialogs::TRzSelectFolderButtonCaptions* FButtonCaptions;
	Rzshelldialogs::TRzSelectFolderDialogOptions FOptions;
	System::Classes::TNotifyEvent FOnFormShow;
	System::Classes::TNotifyEvent FOnFormClose;
	Rzshelldialogs::TRzFolderBrowseSelChangeEvent FOnSelChange;
	Rzshellctrls::TRzShAddItemEvent FOnAddItem;
	virtual void __fastcall CreateWnd();
	DYNAMIC void __fastcall DoShow();
	DYNAMIC void __fastcall DoHide();
	DYNAMIC void __fastcall DoTranslation();
	virtual void __fastcall SetOptions(Rzshelldialogs::TRzSelectFolderDialogOptions Value);
	void __fastcall SetButtonCaptions(Rzshelldialogs::TRzSelectFolderButtonCaptions* Value);
	
public:
	__fastcall virtual TRzSelectFolderForm(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzSelectFolderForm();
	void __fastcall InitFraming(System::Uitypes::TColor FrameColor, Rzcommon::TFrameStyle FrameStyle, bool FrameVisible, Rzcommon::TFramingPreference FramingPreference);
	void __fastcall InitHotTracking(System::Uitypes::TColor ButtonColor, bool HotTrack, System::Uitypes::TColor HighlightColor, System::Uitypes::TColor HotTrackColor, Rzcommon::TRzHotTrackColorType HotTrackColorType);
	void __fastcall InitShowButtonGlyphs(bool ShowButtonGlyphs);
	void __fastcall InitToolButtons(Rzcommon::TRzGradientColorStyle ToolBtnGradientColorStyle, System::Uitypes::TColor ToolBtnSelectionColorStart, System::Uitypes::TColor ToolBtnSelectionColorStop, System::Uitypes::TColor ToolBtnSelectionFrameColor, Rzcommon::TRzVisualStyle ToolBtnVisualStyle);
	__property bool Executing = {read=FExecuting, nodefault};
	__property System::UnicodeString SelectedPathname = {read=GetSelectedPathname, write=SetSelectedPathname};
	__property System::UnicodeString Status = {read=GetStatus, write=SetStatus};
	__property bool OkEnabled = {read=GetOkEnabled, write=SetOkEnabled, nodefault};
	__property Rzshelldialogs::TRzSelectFolderButtonCaptions* ButtonCaptions = {read=FButtonCaptions, write=SetButtonCaptions};
	__property Rzshelldialogs::TRzSelectFolderDialogOptions Options = {read=FOptions, write=SetOptions, nodefault};
	__property Rzshellctrls::TRzShAddItemEvent OnAddItem = {read=FOnAddItem, write=FOnAddItem};
	__property System::Classes::TNotifyEvent OnFormClose = {read=FOnFormClose, write=FOnFormClose};
	__property System::Classes::TNotifyEvent OnFormShow = {read=FOnFormShow, write=FOnFormShow};
	__property Rzshelldialogs::TRzFolderBrowseSelChangeEvent OnSelChange = {read=FOnSelChange, write=FOnSelChange};
public:
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzSelectFolderForm(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSelectFolderForm(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzshellfolderform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLFOLDERFORM)
using namespace Rzshellfolderform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshellfolderformHPP
