// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellOpenForm.pas' rev: 35.00 (Windows)

#ifndef RzshellopenformHPP
#define RzshellopenformHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.Classes.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Menus.hpp>
#include <System.Contnrs.hpp>
#include <Vcl.ComCtrls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <RzCommon.hpp>
#include <RzListVw.hpp>
#include <RzTreeVw.hpp>
#include <RzCmboBx.hpp>
#include <RzPanel.hpp>
#include <RzSplit.hpp>
#include <Vcl.ImgList.hpp>
#include <RzButton.hpp>
#include <RzRadChk.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzShellCtrls.hpp>
#include <RzShellUtils.hpp>
#include <RzShellDialogs.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzshellopenform
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzShellOpenSaveForm;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzShellOpenSaveForm_LIS : unsigned char { lisEdit, lisList };

class PASCALIMPLEMENTATION TRzShellOpenSaveForm : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzshellctrls::TRzShellCombo* ShellCombo;
	Rzbutton::TRzToolButton* btnUpOneLevel;
	Rzbutton::TRzToolButton* btnList;
	Rzbutton::TRzToolButton* btnDetails;
	Rzbutton::TRzToolButton* btnCreateNewFolder;
	Rzpanel::TRzPanel* PnlEdits;
	Vcl::Stdctrls::TLabel* lblFileName;
	Vcl::Stdctrls::TLabel* lblFilesOfType;
	Rzcmbobx::TRzComboBox* cbxFileTypes;
	Rzedit::TRzEdit* edtFileName;
	Rzbutton::TRzBitBtn* btnOpen;
	Rzbutton::TRzBitBtn* btnCancel;
	Rzcmbobx::TRzComboBox* cbxFileName;
	Rzradchk::TRzCheckBox* chkReadOnly;
	Rzbutton::TRzToolButton* btnShowTree;
	Rzbutton::TRzBitBtn* btnHelp;
	Vcl::Menus::TPopupMenu* LvPopup;
	Vcl::Menus::TMenuItem* View1Mitm;
	Vcl::Menus::TMenuItem* N1;
	Vcl::Menus::TMenuItem* New1Mitm;
	Vcl::Menus::TMenuItem* N2;
	Vcl::Menus::TMenuItem* Properties1Mitm;
	Vcl::Menus::TMenuItem* Folder1Mitm;
	Vcl::Menus::TMenuItem* LargeIcons1Mitm;
	Vcl::Menus::TMenuItem* Smallicons1MItm;
	Vcl::Menus::TMenuItem* List1Mitm;
	Vcl::Menus::TMenuItem* Details1Mitm;
	Vcl::Menus::TMenuItem* Paste1Mitm;
	Vcl::Menus::TMenuItem* N3;
	Rzbutton::TRzToolButton* btnShowDesktop;
	Rzsplit::TRzSplitter* RzSplitter1;
	Rzshellctrls::TRzShellTree* ShellTree;
	Rzshellctrls::TRzShellList* ShellList;
	Vcl::Controls::TImageList* imlPlaces;
	Vcl::Extctrls::TPanel* PnlJumps;
	Rzbutton::TRzToolButton* btnPlace0;
	Rzbutton::TRzToolButton* btnPlace1;
	Rzbutton::TRzToolButton* btnPlace2;
	Rzbutton::TRzToolButton* btnPlace3;
	Rzbutton::TRzToolButton* btnPlace4;
	Vcl::Stdctrls::TLabel* lblLookIn;
	Vcl::Extctrls::TPanel* PnlWork;
	Vcl::Controls::TImageList* imlToolbar;
	void __fastcall ViewBtnClick(System::TObject* Sender);
	void __fastcall ShellListChange(System::TObject* Sender, Vcl::Comctrls::TListItem* Item, Vcl::Comctrls::TItemChange Change);
	void __fastcall btnUpOneLevelClick(System::TObject* Sender);
	void __fastcall btnShowTreeClick(System::TObject* Sender);
	void __fastcall FormDestroy(System::TObject* Sender);
	void __fastcall FormKeyDown(System::TObject* Sender, System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall FormCloseQuery(System::TObject* Sender, bool &CanClose);
	void __fastcall ShellTreeChange(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node);
	void __fastcall cbxFileTypesSelEndOk(System::TObject* Sender);
	void __fastcall btnCreateNewFolderClick(System::TObject* Sender);
	void __fastcall edtFileNameChange(System::TObject* Sender);
	void __fastcall Paste1MitmClick(System::TObject* Sender);
	void __fastcall Properties1MitmClick(System::TObject* Sender);
	void __fastcall btnHelpClick(System::TObject* Sender);
	void __fastcall chkReadOnlyClick(System::TObject* Sender);
	void __fastcall ShellListFolderChanged(System::TObject* Sender);
	void __fastcall FormResize(System::TObject* Sender);
	void __fastcall btnShowDesktopClick(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall btnPlaceClick(System::TObject* Sender);
	
private:
	System::Contnrs::TObjectList* FPlacesList;
	bool __fastcall FormHelp(System::Word Command, NativeInt Data, bool &CallHelp);
	void __fastcall ListDblClickOpen(System::TObject* Sender, bool &Handled);
	HIDESBASE MESSAGE void __fastcall WMGetMinMaxInfo(Winapi::Messages::TWMGetMinMaxInfo &Msg);
	void __fastcall InitCustomPlaces();
	void __fastcall JumpToPlace(int Num);
	void __fastcall UpdateCurrentPlace();
	
protected:
	System::UnicodeString FDefaultExt;
	Rzshelldialogs::TRzOpenSaveOptions FOptions;
	System::Classes::TStrings* FFiles;
	System::UnicodeString FFilter;
	System::UnicodeString FInitialDir;
	System::Classes::TNotifyEvent FOnTypeChanged;
	System::Classes::TNotifyEvent FOnFolderChanged;
	System::Classes::TNotifyEvent FOnSelectionChanged;
	System::Classes::TNotifyEvent FOnFormShow;
	System::Classes::TNotifyEvent FOnFormClose;
	Vcl::Forms::THelpEvent FOnFormHelp;
	DYNAMIC void __fastcall DoOnFormClose();
	DYNAMIC void __fastcall DoOnFolderChanged();
	DYNAMIC void __fastcall DoOnSelectionChanged();
	DYNAMIC void __fastcall DoOnFormShow();
	DYNAMIC void __fastcall DoOnTypeChanged();
	System::UnicodeString __fastcall GetFilename();
	System::Classes::TStrings* __fastcall GetFiles();
	int __fastcall GetFilterIndex();
	int __fastcall GetFormSplitterPos();
	Rzshellctrls::TRzShAddItemEvent __fastcall GetOnAddListItem();
	Rzshellctrls::TRzShAddItemEvent __fastcall GetOnAddTreeItem();
	Rzshellctrls::TRzShAddItemEvent __fastcall GetOnAddComboItem();
	void __fastcall SetFilename(const System::UnicodeString Value);
	void __fastcall SetFilter(const System::UnicodeString Value);
	void __fastcall SetFilterIndex(int Value);
	void __fastcall SetFormSplitterPos(int Value);
	void __fastcall SetInitialDir(const System::UnicodeString Value);
	void __fastcall SetOptions(Rzshelldialogs::TRzOpenSaveOptions Value);
	void __fastcall SetOnAddListItem(Rzshellctrls::TRzShAddItemEvent Value);
	void __fastcall SetOnAddTreeItem(Rzshellctrls::TRzShAddItemEvent Value);
	void __fastcall SetOnAddComboItem(Rzshellctrls::TRzShAddItemEvent Value);
	System::UnicodeString FUserFilter;
	bool FExecuting;
	System::Classes::TStrings* FSelections;
	TRzShellOpenSaveForm_LIS FLastInputState;
	HWND FHGripWindow;
	virtual void __fastcall CreateWnd();
	DYNAMIC void __fastcall DoTranslation();
	void __fastcall ApplyUserFilter(System::UnicodeString Filter);
	void __fastcall GetSelectedFiles(System::Classes::TStrings* s);
	void __fastcall ShowTree(bool Show);
	DYNAMIC void __fastcall DoHide();
	DYNAMIC void __fastcall DoShow();
	
public:
	__fastcall virtual TRzShellOpenSaveForm(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzShellOpenSaveForm();
	bool __fastcall ParseInputstring(const System::UnicodeString ins);
	void __fastcall InitFraming(System::Uitypes::TColor FrameColor, Rzcommon::TFrameStyle FrameStyle, bool FrameVisible, Rzcommon::TFramingPreference FramingPreference);
	void __fastcall InitHotTracking(System::Uitypes::TColor ButtonColor, bool HotTrack, System::Uitypes::TColor HighlightColor, System::Uitypes::TColor HotTrackColor, Rzcommon::TRzHotTrackColorType HotTrackColorType);
	void __fastcall InitShowButtonGlyphs(bool ShowButtonGlyphs);
	void __fastcall InitToolButtons(Rzcommon::TRzGradientColorStyle ToolBtnGradientColorStyle, System::Uitypes::TColor ToolBtnSelectionColorStart, System::Uitypes::TColor ToolBtnSelectionColorStop, System::Uitypes::TColor ToolBtnSelectionFrameColor, Rzcommon::TRzVisualStyle ToolBtnVisualStyle);
	__property System::UnicodeString DefaultExt = {read=FDefaultExt, write=FDefaultExt};
	__property bool Executing = {read=FExecuting, nodefault};
	__property Rzshelldialogs::TRzOpenSaveOptions Options = {read=FOptions, write=SetOptions, nodefault};
	__property System::UnicodeString FileName = {read=GetFilename, write=SetFilename};
	__property System::Classes::TStrings* Files = {read=GetFiles};
	__property System::UnicodeString Filter = {read=FFilter, write=SetFilter};
	__property int FilterIndex = {read=GetFilterIndex, write=SetFilterIndex, default=1};
	__property int FormSplitterPos = {read=GetFormSplitterPos, write=SetFormSplitterPos, default=-1};
	__property HelpContext = {default=0};
	__property System::UnicodeString InitialDir = {read=FInitialDir, write=SetInitialDir};
	__property Rzshellctrls::TRzShAddItemEvent OnAddListItem = {read=GetOnAddListItem, write=SetOnAddListItem};
	__property Rzshellctrls::TRzShAddItemEvent OnAddTreeItem = {read=GetOnAddTreeItem, write=SetOnAddTreeItem};
	__property Rzshellctrls::TRzShAddItemEvent OnAddComboItem = {read=GetOnAddComboItem, write=SetOnAddComboItem};
	__property OnHelp;
	__property Vcl::Forms::THelpEvent OnFormHelp = {read=FOnFormHelp, write=FOnFormHelp};
	__property System::Classes::TNotifyEvent OnFormClose = {read=FOnFormClose, write=FOnFormClose};
	__property System::Classes::TNotifyEvent OnFormShow = {read=FOnFormShow, write=FOnFormShow};
	__property System::Classes::TNotifyEvent OnFolderChanged = {read=FOnFolderChanged, write=FOnFolderChanged};
	__property System::Classes::TNotifyEvent OnSelectionChanged = {read=FOnSelectionChanged, write=FOnSelectionChanged};
	__property System::Classes::TNotifyEvent OnTypeChanged = {read=FOnTypeChanged, write=FOnTypeChanged};
public:
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzShellOpenSaveForm(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzShellOpenSaveForm(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzshellopenform */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLOPENFORM)
using namespace Rzshellopenform;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshellopenformHPP
