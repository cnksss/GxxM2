// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellDialogs.pas' rev: 35.00 (Windows)

#ifndef RzshelldialogsHPP
#define RzshelldialogsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.ShlObj.hpp>
#include <RzShellCtrls.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzshelldialogs
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzShellDialog;
class DELPHICLASS TRzSelectFolderButtonCaptions;
class DELPHICLASS TRzCustomSelectFolderDialog;
class DELPHICLASS TRzSelectFolderDialog;
class DELPHICLASS TRzFileDialog;
class DELPHICLASS TRzCustomOpenDialog;
class DELPHICLASS TRzOpenDialog;
class DELPHICLASS TRzCustomSaveDialog;
class DELPHICLASS TRzSaveDialog;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzSelectFolderDialogOption : unsigned char { sfdoCreateDeleteButtons, sfdoContextMenus, sfdoReadOnly, sfdoIncludeNonFolders, sfdoOleDrag, sfdoOleDrop, sfdoCreateFolderIcon, sfdoDeleteFolderIcon, sfdoVirtualFolders, sfdoShowHidden, sfdoFilesCanBeFolders };

typedef System::Set<TRzSelectFolderDialogOption, TRzSelectFolderDialogOption::sfdoCreateDeleteButtons, TRzSelectFolderDialogOption::sfdoFilesCanBeFolders> TRzSelectFolderDialogOptions;

class PASCALIMPLEMENTATION TRzShellDialog : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Vcl::Forms::TPosition FFormPosition;
	int FFormLeft;
	int FFormTop;
	int FFormWidth;
	int FFormHeight;
	System::Uitypes::TWindowState FWindowState;
	System::UnicodeString FTitle;
	System::Classes::TNotifyEvent FOnInit;
	System::Classes::TNotifyEvent FOnFormShow;
	System::Classes::TNotifyEvent FOnFormClose;
	bool FExecuting;
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	Rzcommon::TFramingPreference FFramingPreference;
	System::Uitypes::TColor FButtonColor;
	bool FShowButtonGlyphs;
	System::Uitypes::TColor FHighlightColor;
	bool FHotTrack;
	System::Uitypes::TColor FHotTrackColor;
	Rzcommon::TRzHotTrackColorType FHotTrackColorType;
	Rzcommon::TRzGradientColorStyle FToolBtnGradientColorStyle;
	System::Uitypes::TColor FToolBtnSelectionColorStart;
	System::Uitypes::TColor FToolBtnSelectionColorStop;
	System::Uitypes::TColor FToolBtnSelectionFrameColor;
	Rzcommon::TRzVisualStyle FToolBtnVisualStyle;
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	DYNAMIC void __fastcall DoInitialized();
	DYNAMIC bool __fastcall DoExecute() = 0 ;
	__property bool Executing = {read=FExecuting, nodefault};
	__property System::UnicodeString Title = {read=FTitle, write=FTitle};
	__property Vcl::Forms::TPosition FormPosition = {read=FFormPosition, write=FFormPosition, default=4};
	__property int FormLeft = {read=FFormLeft, write=FFormLeft, default=-1};
	__property int FormTop = {read=FFormTop, write=FFormTop, default=-1};
	__property int FormWidth = {read=FFormWidth, write=FFormWidth, default=-1};
	__property int FormHeight = {read=FFormHeight, write=FFormHeight, default=-1};
	__property System::Uitypes::TWindowState FormWindowState = {read=FWindowState, write=FWindowState, default=0};
	__property System::Uitypes::TColor ButtonColor = {read=FButtonColor, write=FButtonColor, default=-16777201};
	__property bool ShowButtonGlyphs = {read=FShowButtonGlyphs, write=FShowButtonGlyphs, default=0};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=FFrameColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=FFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=FFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=FFramingPreference, default=1};
	__property bool HotTrack = {read=FHotTrack, write=FHotTrack, default=0};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=FHighlightColor, default=-16777203};
	__property System::Uitypes::TColor HotTrackColor = {read=FHotTrackColor, write=FHotTrackColor, default=1350640};
	__property Rzcommon::TRzHotTrackColorType HotTrackColorType = {read=FHotTrackColorType, write=FHotTrackColorType, default=1};
	__property Rzcommon::TRzGradientColorStyle ToolBtnGradientColorStyle = {read=FToolBtnGradientColorStyle, write=FToolBtnGradientColorStyle, default=0};
	__property System::Uitypes::TColor ToolBtnSelectionColorStart = {read=FToolBtnSelectionColorStart, write=FToolBtnSelectionColorStart, default=-16777201};
	__property System::Uitypes::TColor ToolBtnSelectionColorStop = {read=FToolBtnSelectionColorStop, write=FToolBtnSelectionColorStop, default=-16777200};
	__property System::Uitypes::TColor ToolBtnSelectionFrameColor = {read=FToolBtnSelectionFrameColor, write=FToolBtnSelectionFrameColor, default=-16777195};
	__property Rzcommon::TRzVisualStyle ToolBtnVisualStyle = {read=FToolBtnVisualStyle, write=FToolBtnVisualStyle, default=1};
	__property System::Classes::TNotifyEvent OnInitialized = {read=FOnInit, write=FOnInit};
	__property System::Classes::TNotifyEvent OnFormClose = {read=FOnFormClose, write=FOnFormClose};
	__property System::Classes::TNotifyEvent OnFormShow = {read=FOnFormShow, write=FOnFormShow};
	
public:
	__fastcall virtual TRzShellDialog(System::Classes::TComponent* AOwner);
	bool __fastcall Execute();
	void __fastcall ReadStateFromRegistry(HKEY BaseKey, System::UnicodeString SubKeyName, System::UnicodeString ValueName);
	void __fastcall WriteStateToRegistry(HKEY BaseKey, System::UnicodeString SubKeyName, System::UnicodeString ValueName);
	DYNAMIC void __fastcall ReadStateFromStream(System::Classes::TStream* Stream);
	DYNAMIC void __fastcall WriteStateToStream(System::Classes::TStream* Stream);
public:
	/* TComponent.Destroy */ inline __fastcall virtual ~TRzShellDialog() { }
	
};


typedef void __fastcall (__closure *TRzFolderBrowseSelChangeEvent)(System::TObject* Sender, Winapi::Shlobj::PItemIDList NewSel);

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzSelectFolderButtonCaptions : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	System::UnicodeString FOK;
	System::UnicodeString FCancel;
	System::UnicodeString FCreateFolder;
	System::UnicodeString FDeleteFolder;
	
public:
	__fastcall TRzSelectFolderButtonCaptions();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	
__published:
	__property System::UnicodeString OK = {read=FOK, write=FOK};
	__property System::UnicodeString Cancel = {read=FCancel, write=FCancel};
	__property System::UnicodeString CreateFolder = {read=FCreateFolder, write=FCreateFolder};
	__property System::UnicodeString DeleteFolder = {read=FDeleteFolder, write=FDeleteFolder};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzSelectFolderButtonCaptions() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCustomSelectFolderDialog : public TRzShellDialog
{
	typedef TRzShellDialog inherited;
	
private:
	Vcl::Forms::TCustomForm* FForm;
	Rzshellctrls::TRzShellLocator* FBaseFolder;
	TRzSelectFolderDialogOptions FOptions;
	Rzshellctrls::TRzShAddItemEvent FOnAddItem;
	TRzFolderBrowseSelChangeEvent FOnSelChange;
	System::UnicodeString FStatus;
	Rzshellctrls::TRzShellLocator* FSelectedFolder;
	TRzSelectFolderButtonCaptions* FButtonCaptions;
	bool __fastcall GetOkEnabled();
	System::UnicodeString __fastcall GetSelectedPathName();
	void __fastcall SetBaseFolder(Rzshellctrls::TRzShellLocator* Value);
	void __fastcall SetOkEnabled(bool Value);
	void __fastcall SetStatus(const System::UnicodeString Value);
	void __fastcall SetSelectedPathName(const System::UnicodeString Value);
	void __fastcall SetSelectedFolder(Rzshellctrls::TRzShellLocator* Value);
	void __fastcall SetButtonCaptions(TRzSelectFolderButtonCaptions* Value);
	
protected:
	void __fastcall AssertFormActive();
	void __fastcall AssertFormNotActive();
	DYNAMIC bool __fastcall DoExecute();
	virtual Vcl::Forms::TCustomForm* __fastcall CreateForm();
	virtual void __fastcall InitForm(Vcl::Forms::TCustomForm* Form);
	virtual void __fastcall SaveFormSettings(Vcl::Forms::TCustomForm* Form);
	__property Vcl::Forms::TCustomForm* Form = {read=FForm};
	__property TRzSelectFolderButtonCaptions* ButtonCaptions = {read=FButtonCaptions, write=SetButtonCaptions};
	__property System::UnicodeString SelectedPathName = {read=GetSelectedPathName, write=SetSelectedPathName};
	__property Rzshellctrls::TRzShellLocator* SelectedFolder = {read=FSelectedFolder, write=SetSelectedFolder};
	__property System::UnicodeString Status = {read=FStatus, write=SetStatus};
	__property bool OkEnabled = {read=GetOkEnabled, write=SetOkEnabled, nodefault};
	__property Rzshellctrls::TRzShellLocator* BaseFolder = {read=FBaseFolder, write=SetBaseFolder};
	__property TRzSelectFolderDialogOptions Options = {read=FOptions, write=FOptions, default=706};
	__property Rzshellctrls::TRzShAddItemEvent OnAddItem = {read=FOnAddItem, write=FOnAddItem};
	__property TRzFolderBrowseSelChangeEvent OnSelChange = {read=FOnSelChange, write=FOnSelChange};
	
public:
	__fastcall virtual TRzCustomSelectFolderDialog(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomSelectFolderDialog();
	__property Executing;
};


class PASCALIMPLEMENTATION TRzSelectFolderDialog : public TRzCustomSelectFolderDialog
{
	typedef TRzCustomSelectFolderDialog inherited;
	
public:
	__property Form;
	__property SelectedFolder;
	__property SelectedPathName = {default=0};
	__property Status = {default=0};
	__property OkEnabled;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property ButtonCaptions;
	__property Title = {default=0};
	__property FormPosition = {default=4};
	__property FormLeft = {default=-1};
	__property FormTop = {default=-1};
	__property FormWidth = {default=-1};
	__property FormHeight = {default=-1};
	__property FormWindowState = {default=0};
	__property BaseFolder;
	__property Options = {default=706};
	__property FrameColor = {default=-16777200};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property ButtonColor = {default=-16777201};
	__property ShowButtonGlyphs = {default=0};
	__property HotTrack = {default=0};
	__property HighlightColor = {default=-16777203};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorType = {default=1};
	__property ToolBtnGradientColorStyle = {default=0};
	__property ToolBtnSelectionColorStart = {default=-16777201};
	__property ToolBtnSelectionColorStop = {default=-16777200};
	__property ToolBtnSelectionFrameColor = {default=-16777195};
	__property ToolBtnVisualStyle = {default=1};
	__property OnAddItem;
	__property OnFormClose;
	__property OnFormShow;
	__property OnSelChange;
	__property OnInitialized;
public:
	/* TRzCustomSelectFolderDialog.Create */ inline __fastcall virtual TRzSelectFolderDialog(System::Classes::TComponent* AOwner) : TRzCustomSelectFolderDialog(AOwner) { }
	/* TRzCustomSelectFolderDialog.Destroy */ inline __fastcall virtual ~TRzSelectFolderDialog() { }
	
};


enum DECLSPEC_DENUM TRzOpenSaveOption : unsigned char { osoAllowMultiselect, osoCreatePrompt, osoExtensionDifferent, osoFileMustExist, osoHideReadOnly, osoNoChangeDir, osoNoDereferenceLinks, osoNoReadOnlyReturn, osoNoTestFileCreate, osoNoValidate, osoOverwritePrompt, osoReadOnly, osoPathMustExist, osoShareAware, osoShowHelp, osoAllowTree, osoShowTree, osoShowHints, osoHideFoldersInListWhenTreeVisible, osoOleDrag, osoOleDrop, osoShowHidden, osoFilesCanBeFolders };

typedef System::Set<TRzOpenSaveOption, TRzOpenSaveOption::osoAllowMultiselect, TRzOpenSaveOption::osoFilesCanBeFolders> TRzOpenSaveOptions;

class PASCALIMPLEMENTATION TRzFileDialog : public TRzShellDialog
{
	typedef TRzShellDialog inherited;
	
private:
	System::Classes::TStrings* FHistoryList;
	TRzOpenSaveOptions FOptions;
	System::UnicodeString FDefaultExt;
	System::Classes::TStrings* FFiles;
	System::UnicodeString FFilter;
	int FFilterIndex;
	System::Classes::THelpContext FHelpContext;
	System::UnicodeString FInitialDir;
	int FFormSplitterPos;
	Rzshellctrls::TRzShAddItemEvent FOnAddListItem;
	Rzshellctrls::TRzShAddItemEvent FOnAddTreeItem;
	Rzshellctrls::TRzShAddItemEvent FOnAddComboItem;
	System::Classes::TNotifyEvent FOnFolderChanged;
	System::Classes::TNotifyEvent FOnSelectionChanged;
	System::Classes::TNotifyEvent FOnTypeChanged;
	Vcl::Forms::THelpEvent FOnHelp;
	System::UnicodeString __fastcall GetFilename();
	int __fastcall GetFilterIndex();
	void __fastcall SetFilename(const System::UnicodeString Value);
	void __fastcall SetOptions(TRzOpenSaveOptions Value);
	void __fastcall SetFilter(const System::UnicodeString Value);
	void __fastcall SetFilterIndex(int Value);
	void __fastcall SetFormSplitterPos(int Value);
	void __fastcall SetOnAddListItem(Rzshellctrls::TRzShAddItemEvent Value);
	void __fastcall SetOnAddTreeItem(Rzshellctrls::TRzShAddItemEvent Value);
	void __fastcall SetOnAddComboItem(Rzshellctrls::TRzShAddItemEvent Value);
	void __fastcall SetHistoryList(System::Classes::TStrings* Value);
	
protected:
	Vcl::Forms::TCustomForm* FForm;
	DYNAMIC bool __fastcall DoExecute();
	virtual Vcl::Forms::TCustomForm* __fastcall CreateForm();
	virtual void __fastcall InitForm(Vcl::Forms::TCustomForm* Form);
	virtual void __fastcall SaveFormSettings(Vcl::Forms::TCustomForm* Form);
	__property Vcl::Forms::TCustomForm* Form = {read=FForm};
	__property System::UnicodeString DefaultExt = {read=FDefaultExt, write=FDefaultExt};
	__property TRzOpenSaveOptions Options = {read=FOptions, write=SetOptions, nodefault};
	__property System::UnicodeString FileName = {read=GetFilename, write=SetFilename};
	__property System::Classes::TStrings* Files = {read=FFiles};
	__property System::UnicodeString Filter = {read=FFilter, write=SetFilter};
	__property int FilterIndex = {read=GetFilterIndex, write=SetFilterIndex, default=1};
	__property int FormSplitterPos = {read=FFormSplitterPos, write=SetFormSplitterPos, default=-1};
	__property System::Classes::THelpContext HelpContext = {read=FHelpContext, write=FHelpContext, default=0};
	__property System::Classes::TStrings* HistoryList = {read=FHistoryList, write=SetHistoryList, stored=false};
	__property System::UnicodeString InitialDir = {read=FInitialDir, write=FInitialDir};
	__property Rzshellctrls::TRzShAddItemEvent OnAddListItem = {read=FOnAddListItem, write=SetOnAddListItem};
	__property Rzshellctrls::TRzShAddItemEvent OnAddTreeItem = {read=FOnAddTreeItem, write=SetOnAddTreeItem};
	__property Rzshellctrls::TRzShAddItemEvent OnAddComboItem = {read=FOnAddComboItem, write=SetOnAddComboItem};
	__property Vcl::Forms::THelpEvent OnHelp = {read=FOnHelp, write=FOnHelp};
	__property System::Classes::TNotifyEvent OnFolderChanged = {read=FOnFolderChanged, write=FOnFolderChanged};
	__property System::Classes::TNotifyEvent OnSelectionChanged = {read=FOnSelectionChanged, write=FOnSelectionChanged};
	__property System::Classes::TNotifyEvent OnTypeChanged = {read=FOnTypeChanged, write=FOnTypeChanged};
	
public:
	__fastcall virtual TRzFileDialog(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFileDialog();
	DYNAMIC void __fastcall ReadStateFromStream(System::Classes::TStream* Stream);
	DYNAMIC void __fastcall WriteStateToStream(System::Classes::TStream* Stream);
	
__published:
	__property FrameColor = {default=-16777200};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property ButtonColor = {default=-16777201};
	__property ShowButtonGlyphs = {default=0};
	__property HotTrack = {default=0};
	__property HighlightColor = {default=-16777203};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorType = {default=1};
	__property ToolBtnGradientColorStyle = {default=0};
	__property ToolBtnSelectionColorStart = {default=-16777201};
	__property ToolBtnSelectionColorStop = {default=-16777200};
	__property ToolBtnSelectionFrameColor = {default=-16777195};
	__property ToolBtnVisualStyle = {default=1};
};


class PASCALIMPLEMENTATION TRzCustomOpenDialog : public TRzFileDialog
{
	typedef TRzFileDialog inherited;
	
protected:
	virtual void __fastcall InitForm(Vcl::Forms::TCustomForm* Form);
	
public:
	__fastcall virtual TRzCustomOpenDialog(System::Classes::TComponent* AOwner);
public:
	/* TRzFileDialog.Destroy */ inline __fastcall virtual ~TRzCustomOpenDialog() { }
	
};


class PASCALIMPLEMENTATION TRzOpenDialog : public TRzCustomOpenDialog
{
	typedef TRzCustomOpenDialog inherited;
	
public:
	__property Executing;
	__property FileName = {default=0};
	__property Files;
	__property Form;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Title = {default=0};
	__property Options = {default=3833872};
	__property Filter = {default=0};
	__property FilterIndex = {default=1};
	__property FormPosition = {default=4};
	__property FormLeft = {default=-1};
	__property FormTop = {default=-1};
	__property FormWidth = {default=-1};
	__property FormHeight = {default=-1};
	__property FormWindowState = {default=0};
	__property FormSplitterPos = {default=-1};
	__property HelpContext = {default=0};
	__property HistoryList;
	__property InitialDir = {default=0};
	__property DefaultExt = {default=0};
	__property OnAddListItem;
	__property OnAddTreeItem;
	__property OnAddComboItem;
	__property OnInitialized;
	__property OnHelp;
	__property OnFormClose;
	__property OnFormShow;
	__property OnFolderChanged;
	__property OnSelectionChanged;
	__property OnTypeChanged;
public:
	/* TRzCustomOpenDialog.Create */ inline __fastcall virtual TRzOpenDialog(System::Classes::TComponent* AOwner) : TRzCustomOpenDialog(AOwner) { }
	
public:
	/* TRzFileDialog.Destroy */ inline __fastcall virtual ~TRzOpenDialog() { }
	
};


class PASCALIMPLEMENTATION TRzCustomSaveDialog : public TRzFileDialog
{
	typedef TRzFileDialog inherited;
	
protected:
	virtual void __fastcall InitForm(Vcl::Forms::TCustomForm* Form);
	
public:
	__fastcall virtual TRzCustomSaveDialog(System::Classes::TComponent* aOwner);
public:
	/* TRzFileDialog.Destroy */ inline __fastcall virtual ~TRzCustomSaveDialog() { }
	
};


class PASCALIMPLEMENTATION TRzSaveDialog : public TRzCustomSaveDialog
{
	typedef TRzCustomSaveDialog inherited;
	
public:
	__property Executing;
	__property FileName = {default=0};
	__property Files;
	__property Form;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property DefaultExt = {default=0};
	__property Title = {default=0};
	__property Options = {default=3833872};
	__property Filter = {default=0};
	__property FilterIndex = {default=1};
	__property FormPosition = {default=4};
	__property FormLeft = {default=-1};
	__property FormTop = {default=-1};
	__property FormWidth = {default=-1};
	__property FormHeight = {default=-1};
	__property FormWindowState = {default=0};
	__property FormSplitterPos = {default=-1};
	__property HelpContext = {default=0};
	__property HistoryList;
	__property InitialDir = {default=0};
	__property OnAddListItem;
	__property OnAddTreeItem;
	__property OnAddComboItem;
	__property OnInitialized;
	__property OnHelp;
	__property OnFormClose;
	__property OnFormShow;
	__property OnFolderChanged;
	__property OnSelectionChanged;
	__property OnTypeChanged;
public:
	/* TRzCustomSaveDialog.Create */ inline __fastcall virtual TRzSaveDialog(System::Classes::TComponent* aOwner) : TRzCustomSaveDialog(aOwner) { }
	
public:
	/* TRzFileDialog.Destroy */ inline __fastcall virtual ~TRzSaveDialog() { }
	
};


//-- var, const, procedure ---------------------------------------------------
#define DEF_SFDO_OPTIONS (System::Set<TRzSelectFolderDialogOption, TRzSelectFolderDialogOption::sfdoCreateDeleteButtons, TRzSelectFolderDialogOption::sfdoFilesCanBeFolders>() << TRzSelectFolderDialogOption::sfdoContextMenus << TRzSelectFolderDialogOption::sfdoCreateFolderIcon << TRzSelectFolderDialogOption::sfdoDeleteFolderIcon << TRzSelectFolderDialogOption::sfdoShowHidden )
#define DEF_OPEN_OPTIONS (System::Set<TRzOpenSaveOption, TRzOpenSaveOption::osoAllowMultiselect, TRzOpenSaveOption::osoFilesCanBeFolders>() << TRzOpenSaveOption::osoHideReadOnly << TRzOpenSaveOption::osoAllowTree << TRzOpenSaveOption::osoShowHints << TRzOpenSaveOption::osoOleDrag << TRzOpenSaveOption::osoOleDrop << TRzOpenSaveOption::osoShowHidden )
#define DEF_SAVE_OPTIONS (System::Set<TRzOpenSaveOption, TRzOpenSaveOption::osoAllowMultiselect, TRzOpenSaveOption::osoFilesCanBeFolders>() << TRzOpenSaveOption::osoHideReadOnly << TRzOpenSaveOption::osoAllowTree << TRzOpenSaveOption::osoShowHints << TRzOpenSaveOption::osoOleDrag << TRzOpenSaveOption::osoOleDrop << TRzOpenSaveOption::osoShowHidden )
}	/* namespace Rzshelldialogs */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLDIALOGS)
using namespace Rzshelldialogs;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshelldialogsHPP
