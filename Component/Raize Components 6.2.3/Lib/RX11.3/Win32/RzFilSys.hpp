// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzFilSys.pas' rev: 35.00 (Windows)

#ifndef RzfilsysHPP
#define RzfilsysHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.FileCtrl.hpp>
#include <Winapi.ShellAPI.hpp>
#include <Vcl.Graphics.hpp>
#include <RzTreeVw.hpp>
#include <Vcl.ComCtrls.hpp>
#include <Winapi.CommCtrl.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ImgList.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzfilsys
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzFileInfo;
class DELPHICLASS TRzFileListBox;
struct TFolderInfo;
class DELPHICLASS TRzDirectoryTree;
class DELPHICLASS TRzDirectoryListBox;
class DELPHICLASS TRzDriveComboBox;
//-- type declarations -------------------------------------------------------
typedef System::Set<Vcl::Filectrl::TDriveType, Vcl::Filectrl::TDriveType::dtUnknown, Vcl::Filectrl::TDriveType::dtRAM> TDriveTypes;

typedef System::Set<System::Int8, 0, 25> TDriveBits;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzFileInfo : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	System::UnicodeString Name;
	int Attr;
	int Time;
	int Size;
	bool IsDirectory;
	NativeUInt IconHandle;
	Vcl::Graphics::TBitmap* IconGlyph;
public:
	/* TObject.Create */ inline __fastcall TRzFileInfo() : System::TObject() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TRzFileInfo() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzFileListBox _DEPRECATED_ATTRIBUTE0  : public Vcl::Filectrl::TFileListBox
{
	typedef Vcl::Filectrl::TFileListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo _DEPRECATED_ATTRIBUTE0 ;
	TRzDirectoryTree* FDirTree _DEPRECATED_ATTRIBUTE0 ;
	System::Classes::TStringList* FFileInfoList _DEPRECATED_ATTRIBUTE0 ;
	bool FShowLongNames _DEPRECATED_ATTRIBUTE0 ;
	bool FAllowOpen _DEPRECATED_ATTRIBUTE0 ;
	bool FUpdatingColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FDisabledColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFocusColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FNormalColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameColor _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TRzFrameController* FFrameController _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameHotColor _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameHotTrack _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameHotStyle _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TSides FFrameSides _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameStyle _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameVisible _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFramingPreference FFramingPreference _DEPRECATED_ATTRIBUTE0 ;
	bool FInReadFileNames _DEPRECATED_ATTRIBUTE0 ;
	HIDESBASE void __fastcall ResetItemHeight _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall ReadOldFrameFlatProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CNDrawItem _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanging _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas _DEPRECATED_ATTRIBUTE0 ;
	bool FOverControl _DEPRECATED_ATTRIBUTE0 ;
	virtual void __fastcall DefineProperties _DEPRECATED_ATTRIBUTE0 (System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Notification _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall UpdateFrame _DEPRECATED_ATTRIBUTE0 (bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall ClearFileInfoList _DEPRECATED_ATTRIBUTE0 ();
	DYNAMIC void __fastcall DblClick _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall ReadBitmaps _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall ReadFileNames _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall LocalSetDirectory _DEPRECATED_ATTRIBUTE0 (const System::UnicodeString NewDirectory);
	System::UnicodeString __fastcall LocalGetFileName _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall LocalSetFileName _DEPRECATED_ATTRIBUTE0 (const System::UnicodeString NewFile);
	virtual int __fastcall Compare _DEPRECATED_ATTRIBUTE0 (TRzFileInfo* A, TRzFileInfo* B);
	virtual void __fastcall QuickSort _DEPRECATED_ATTRIBUTE0 (int L, int R);
	virtual void __fastcall DrawItem _DEPRECATED_ATTRIBUTE0 (int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	DYNAMIC bool __fastcall DoMouseWheelDown _DEPRECATED_ATTRIBUTE0 (System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp _DEPRECATED_ATTRIBUTE0 (System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	bool __fastcall StoreColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall StoreFocusColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall NotUsingController _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetDisabledColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController _DEPRECATED_ATTRIBUTE0 (Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFrameHotStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides _DEPRECATED_ATTRIBUTE0 (Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFramingPreference _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFramingPreference Value);
	virtual bool __fastcall GetShowGlyphs _DEPRECATED_ATTRIBUTE0 ();
	HIDESBASE virtual void __fastcall SetShowGlyphs _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetShowLongNames _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual System::UnicodeString __fastcall GetLongFileName _DEPRECATED_ATTRIBUTE0 ();
	virtual System::UnicodeString __fastcall GetShortFileName _DEPRECATED_ATTRIBUTE0 ();
	
public:
	__fastcall virtual TRzFileListBox _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFileListBox _DEPRECATED_ATTRIBUTE0 ();
	virtual bool __fastcall UseThemes _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall UpOneLevel _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall ApplyFilePath _DEPRECATED_ATTRIBUTE0 (const System::UnicodeString Value);
	__property System::UnicodeString LongFileName = {read=GetLongFileName};
	__property System::UnicodeString ShortFileName = {read=GetShortFileName};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowOpen = {read=FAllowOpen, write=FAllowOpen, default=0};
	__property Columns _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Color _DEPRECATED_ATTRIBUTE0  = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=NotUsingController, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=NotUsingController, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=NotUsingController, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=NotUsingController, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=NotUsingController, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=NotUsingController, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, default=1};
	__property bool ShowLongNames = {read=FShowLongNames, write=SetShowLongNames, default=1};
	__property bool ShowGlyphs = {read=GetShowGlyphs, write=SetShowGlyphs, default=1};
	__property OnMouseWheel _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheelUp _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheelDown _DEPRECATED_ATTRIBUTE0 ;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzFileListBox(HWND ParentWindow) : Vcl::Filectrl::TFileListBox(ParentWindow) { }
	
};


typedef TFolderInfo *PFolderInfo;

struct DECLSPEC_DRECORD TFolderInfo
{
public:
	System::UnicodeString FullPath;
	bool ProcessedChildren;
};


enum DECLSPEC_DENUM TNetworkVolumeFormat : unsigned char { nvfExplorer, nvfUNC, nvfVolumeOnly };

class PASCALIMPLEMENTATION TRzDirectoryTree _DEPRECATED_ATTRIBUTE0  : public Rztreevw::TRzCustomTreeView
{
	typedef Rztreevw::TRzCustomTreeView inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo _DEPRECATED_ATTRIBUTE0 ;
	TRzFileListBox* FFileList _DEPRECATED_ATTRIBUTE0 ;
	Vcl::Stdctrls::TLabel* FDirLabel _DEPRECATED_ATTRIBUTE0 ;
	bool FShowHiddenDirs _DEPRECATED_ATTRIBUTE0 ;
	bool FOpenCurrentDir _DEPRECATED_ATTRIBUTE0 ;
	TNetworkVolumeFormat FNetworkVolumeFormat _DEPRECATED_ATTRIBUTE0 ;
	void *FObjInst _DEPRECATED_ATTRIBUTE0 ;
	void *FOldWndProc _DEPRECATED_ATTRIBUTE0 ;
	HWND FFormHandle _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FSaveDirectory _DEPRECATED_ATTRIBUTE0 ;
	bool FUpdating _DEPRECATED_ATTRIBUTE0 ;
	Vcl::Controls::TImageList* FImages _DEPRECATED_ATTRIBUTE0 ;
	int FFolderOpenIconIndex _DEPRECATED_ATTRIBUTE0 ;
	int FFolderClosedIconIndex _DEPRECATED_ATTRIBUTE0 ;
	TDriveBits FActiveDrives _DEPRECATED_ATTRIBUTE0 ;
	TDriveTypes FDriveTypes _DEPRECATED_ATTRIBUTE0 ;
	System::StaticArray<unsigned, 26> FDriveSerialNums _DEPRECATED_ATTRIBUTE0 ;
	System::WideChar FOldDrive _DEPRECATED_ATTRIBUTE0 ;
	System::Classes::TNotifyEvent FOnDriveChange _DEPRECATED_ATTRIBUTE0 ;
	Vcl::Comctrls::TTVExpandedEvent FOnDeletion _DEPRECATED_ATTRIBUTE0 ;
	void __fastcall AddFolderInfoToNode _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node, const System::UnicodeString NodePath, int IconIndex);
	void __fastcall FormWndProc _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateWindowHandle _DEPRECATED_ATTRIBUTE0 (const Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall DestroyWindowHandle _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall DestroyWnd _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall InitImageList _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall InitView _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Loaded _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Notification _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateActiveDrives _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall ClearTree _DEPRECATED_ATTRIBUTE0 ();
	DYNAMIC bool __fastcall CanExpand _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	virtual void __fastcall ResetNode _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	void __fastcall ProcessChildren _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* &Node);
	bool __fastcall HaveProcessedChildren _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	void __fastcall AddTempNodeIfHasChildren _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* &Node);
	DYNAMIC void __fastcall Delete _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall DriveChange _DEPRECATED_ATTRIBUTE0 ();
	DYNAMIC bool __fastcall CanChange _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Change _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Click _DEPRECATED_ATTRIBUTE0 ();
	DYNAMIC void __fastcall KeyDown _DEPRECATED_ATTRIBUTE0 (System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall EditingHandler _DEPRECATED_ATTRIBUTE0 (System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node, bool &AllowEdit);
	void __fastcall EditedHandler _DEPRECATED_ATTRIBUTE0 (System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node, System::UnicodeString &S);
	virtual System::UnicodeString __fastcall GetDirectory _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetDirectory _DEPRECATED_ATTRIBUTE0 (const System::UnicodeString Value);
	virtual System::WideChar __fastcall GetDrive _DEPRECATED_ATTRIBUTE0 ();
	virtual TDriveBits __fastcall GetDrives _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetDriveTypes _DEPRECATED_ATTRIBUTE0 (TDriveTypes Value);
	virtual void __fastcall SetFileList _DEPRECATED_ATTRIBUTE0 (TRzFileListBox* Value);
	virtual void __fastcall SetDirLabel _DEPRECATED_ATTRIBUTE0 (Vcl::Stdctrls::TLabel* Value);
	virtual void __fastcall SetDirLabelCaption _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetNetworkVolumeFormat _DEPRECATED_ATTRIBUTE0 (TNetworkVolumeFormat Value);
	virtual void __fastcall SetShowHiddenDirs _DEPRECATED_ATTRIBUTE0 (bool Value);
	__property Items _DEPRECATED_ATTRIBUTE0  = {stored=false};
	
public:
	__fastcall virtual TRzDirectoryTree _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDirectoryTree _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall RefreshTree _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall RefreshDriveTree _DEPRECATED_ATTRIBUTE0 (System::WideChar DriveChar);
	bool __fastcall NodeHasData _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	Vcl::Comctrls::TTreeNode* __fastcall GetNodeFromPath _DEPRECATED_ATTRIBUTE0 (System::UnicodeString Path);
	System::UnicodeString __fastcall GetPathFromNode _DEPRECATED_ATTRIBUTE0 (Vcl::Comctrls::TTreeNode* Node);
	void __fastcall UpOneLevel _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall CreateNewDir _DEPRECATED_ATTRIBUTE0 (System::UnicodeString NewDirName, bool PlaceInEditMode);
	__property System::UnicodeString Directory = {read=GetDirectory, write=SetDirectory};
	__property System::WideChar Drive = {read=GetDrive, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Stdctrls::TLabel* DirLabel = {read=FDirLabel, write=SetDirLabel};
	__property TDriveTypes DriveTypes = {read=FDriveTypes, write=SetDriveTypes, default=125};
	__property TRzFileListBox* FileList = {read=FFileList, write=SetFileList};
	__property bool OpenCurrentDir = {read=FOpenCurrentDir, write=FOpenCurrentDir, default=0};
	__property TNetworkVolumeFormat NetworkVolumeFormat = {read=FNetworkVolumeFormat, write=SetNetworkVolumeFormat, default=0};
	__property bool ShowHiddenDirs = {read=FShowHiddenDirs, write=SetShowHiddenDirs, default=0};
	__property System::Classes::TNotifyEvent OnDriveChange = {read=FOnDriveChange, write=FOnDriveChange};
	__property Vcl::Comctrls::TTVExpandedEvent OnDeletion = {read=FOnDeletion, write=FOnDeletion};
	__property Align _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Anchors _DEPRECATED_ATTRIBUTE0  = {default=3};
	__property AutoExpand _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property AutoSelect _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property BiDiMode _DEPRECATED_ATTRIBUTE0 ;
	__property BorderStyle _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property BorderWidth _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property ChangeDelay _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Color _DEPRECATED_ATTRIBUTE0  = {default=-16777211};
	__property Constraints _DEPRECATED_ATTRIBUTE0 ;
	__property Ctl3D _DEPRECATED_ATTRIBUTE0 ;
	__property DisabledColor _DEPRECATED_ATTRIBUTE0  = {default=-16777201};
	__property DragCursor _DEPRECATED_ATTRIBUTE0  = {default=-12};
	__property DragKind _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property DragMode _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Enabled _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property Font _DEPRECATED_ATTRIBUTE0 ;
	__property FocusColor _DEPRECATED_ATTRIBUTE0  = {default=-16777211};
	__property FrameColor _DEPRECATED_ATTRIBUTE0  = {default=-16777200};
	__property FrameController _DEPRECATED_ATTRIBUTE0 ;
	__property FrameHotColor _DEPRECATED_ATTRIBUTE0  = {default=-16777200};
	__property FrameHotTrack _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property FrameHotStyle _DEPRECATED_ATTRIBUTE0  = {default=10};
	__property FrameSides _DEPRECATED_ATTRIBUTE0  = {default=15};
	__property FrameStyle _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property FrameVisible _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property FramingPreference _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property Height _DEPRECATED_ATTRIBUTE0  = {default=150};
	__property HideSelection _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property HotTrack _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Indent _DEPRECATED_ATTRIBUTE0 ;
	__property ParentBiDiMode _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property ParentColor _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property ParentCtl3D _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property ParentFont _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property ParentShowHint _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property PopupMenu _DEPRECATED_ATTRIBUTE0 ;
	__property ReadOnly _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property RightClickSelect _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property RowSelect _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property SelectionPen _DEPRECATED_ATTRIBUTE0 ;
	__property ShowButtons _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property ShowHint _DEPRECATED_ATTRIBUTE0 ;
	__property ShowLines _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property StateImages _DEPRECATED_ATTRIBUTE0 ;
	__property TabOrder _DEPRECATED_ATTRIBUTE0  = {default=-1};
	__property TabStop _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property ToolTips _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property Visible _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property Width _DEPRECATED_ATTRIBUTE0  = {default=250};
	__property OnChange _DEPRECATED_ATTRIBUTE0 ;
	__property OnChanging _DEPRECATED_ATTRIBUTE0 ;
	__property OnClick _DEPRECATED_ATTRIBUTE0 ;
	__property OnCollapsed _DEPRECATED_ATTRIBUTE0 ;
	__property OnCollapsing _DEPRECATED_ATTRIBUTE0 ;
	__property OnCompare _DEPRECATED_ATTRIBUTE0 ;
	__property OnContextPopup _DEPRECATED_ATTRIBUTE0 ;
	__property OnDblClick _DEPRECATED_ATTRIBUTE0 ;
	__property OnDragDrop _DEPRECATED_ATTRIBUTE0 ;
	__property OnDragOver _DEPRECATED_ATTRIBUTE0 ;
	__property OnEndDock _DEPRECATED_ATTRIBUTE0 ;
	__property OnEndDrag _DEPRECATED_ATTRIBUTE0 ;
	__property OnEnter _DEPRECATED_ATTRIBUTE0 ;
	__property OnExit _DEPRECATED_ATTRIBUTE0 ;
	__property OnExpanded _DEPRECATED_ATTRIBUTE0 ;
	__property OnExpanding _DEPRECATED_ATTRIBUTE0 ;
	__property OnKeyDown _DEPRECATED_ATTRIBUTE0 ;
	__property OnKeyPress _DEPRECATED_ATTRIBUTE0 ;
	__property OnKeyUp _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseActivate _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseDown _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseEnter _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseLeave _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseMove _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseUp _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheel _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheelDown _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheelUp _DEPRECATED_ATTRIBUTE0 ;
	__property OnStartDock _DEPRECATED_ATTRIBUTE0 ;
	__property OnStartDrag _DEPRECATED_ATTRIBUTE0 ;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDirectoryTree(HWND ParentWindow) : Rztreevw::TRzCustomTreeView(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDirectoryListBox _DEPRECATED_ATTRIBUTE0  : public Vcl::Filectrl::TDirectoryListBox
{
	typedef Vcl::Filectrl::TDirectoryListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo _DEPRECATED_ATTRIBUTE0 ;
	bool FUpdatingColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FDisabledColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFocusColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FNormalColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameColor _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TRzFrameController* FFrameController _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameHotColor _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameHotTrack _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameHotStyle _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TSides FFrameSides _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameStyle _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameVisible _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFramingPreference FFramingPreference _DEPRECATED_ATTRIBUTE0 ;
	bool FShowLongNames _DEPRECATED_ATTRIBUTE0 ;
	void __fastcall ReadOldFrameFlatProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas _DEPRECATED_ATTRIBUTE0 ;
	bool FOverControl _DEPRECATED_ATTRIBUTE0 ;
	virtual void __fastcall DefineProperties _DEPRECATED_ATTRIBUTE0 (System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Notification _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall UpdateFrame _DEPRECATED_ATTRIBUTE0 (bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall BuildList _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Change _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall ReadBitmaps _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall DrawItem _DEPRECATED_ATTRIBUTE0 (int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	int __fastcall DirLevel _DEPRECATED_ATTRIBUTE0 (const System::UnicodeString PathName);
	System::UnicodeString __fastcall GetLongDirName _DEPRECATED_ATTRIBUTE0 ();
	void __fastcall UpdateDirLabel _DEPRECATED_ATTRIBUTE0 ();
	DYNAMIC bool __fastcall DoMouseWheelDown _DEPRECATED_ATTRIBUTE0 (System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp _DEPRECATED_ATTRIBUTE0 (System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	bool __fastcall StoreColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall StoreFocusColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall NotUsingController _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetDisabledColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController _DEPRECATED_ATTRIBUTE0 (Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFrameHotStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides _DEPRECATED_ATTRIBUTE0 (Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFramingPreference _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFramingPreference Value);
	void __fastcall SetShowLongNames _DEPRECATED_ATTRIBUTE0 (bool Value);
	
public:
	__fastcall virtual TRzDirectoryListBox _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDirectoryListBox _DEPRECATED_ATTRIBUTE0 ();
	virtual bool __fastcall UseThemes _DEPRECATED_ATTRIBUTE0 ();
	__property System::UnicodeString LongDirName = {read=GetLongDirName};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Color _DEPRECATED_ATTRIBUTE0  = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=NotUsingController, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=NotUsingController, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=NotUsingController, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=NotUsingController, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=NotUsingController, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=NotUsingController, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, default=1};
	__property bool ShowLongNames = {read=FShowLongNames, write=SetShowLongNames, default=1};
	__property OnMouseWheel _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheelUp _DEPRECATED_ATTRIBUTE0 ;
	__property OnMouseWheelDown _DEPRECATED_ATTRIBUTE0 ;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDirectoryListBox(HWND ParentWindow) : Vcl::Filectrl::TDirectoryListBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDriveComboBox _DEPRECATED_ATTRIBUTE0  : public Vcl::Filectrl::TDriveComboBox
{
	typedef Vcl::Filectrl::TDriveComboBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo _DEPRECATED_ATTRIBUTE0 ;
	TDriveTypes FDriveTypes _DEPRECATED_ATTRIBUTE0 ;
	bool FFlatButtons _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFlatButtonColor _DEPRECATED_ATTRIBUTE0 ;
	bool FUpdatingColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FDisabledColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFocusColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FNormalColor _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameColor _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TRzFrameController* FFrameController _DEPRECATED_ATTRIBUTE0 ;
	System::Uitypes::TColor FFrameHotColor _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameHotTrack _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameHotStyle _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TSides FFrameSides _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFrameStyleEx FFrameStyle _DEPRECATED_ATTRIBUTE0 ;
	bool FFrameVisible _DEPRECATED_ATTRIBUTE0 ;
	Rzcommon::TFramingPreference FFramingPreference _DEPRECATED_ATTRIBUTE0 ;
	void __fastcall ReadOldFrameFlatProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp _DEPRECATED_ATTRIBUTE0 (System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged _DEPRECATED_ATTRIBUTE0 (Winapi::Messages::TMessage &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas _DEPRECATED_ATTRIBUTE0 ;
	bool FInControl _DEPRECATED_ATTRIBUTE0 ;
	bool FOverControl _DEPRECATED_ATTRIBUTE0 ;
	bool FIsFocused _DEPRECATED_ATTRIBUTE0 ;
	virtual void __fastcall DefineProperties _DEPRECATED_ATTRIBUTE0 (System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall Notification _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall UpdateFrame _DEPRECATED_ATTRIBUTE0 (bool ViaMouse, bool InFocus);
	void __fastcall ReadNewBitmaps _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall BuildList _DEPRECATED_ATTRIBUTE0 ();
	HIDESBASE void __fastcall ResetItemHeight _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetFlatButtons _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFlatButtonColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	bool __fastcall StoreColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall StoreFocusColor _DEPRECATED_ATTRIBUTE0 ();
	bool __fastcall NotUsingController _DEPRECATED_ATTRIBUTE0 ();
	virtual void __fastcall SetDisabledColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController _DEPRECATED_ATTRIBUTE0 (Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor _DEPRECATED_ATTRIBUTE0 (System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFrameHotStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides _DEPRECATED_ATTRIBUTE0 (Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible _DEPRECATED_ATTRIBUTE0 (bool Value);
	virtual void __fastcall SetFramingPreference _DEPRECATED_ATTRIBUTE0 (Rzcommon::TFramingPreference Value);
	void __fastcall SetDriveTypes _DEPRECATED_ATTRIBUTE0 (TDriveTypes Value);
	
public:
	__fastcall virtual TRzDriveComboBox _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDriveComboBox _DEPRECATED_ATTRIBUTE0 ();
	virtual bool __fastcall UseThemes _DEPRECATED_ATTRIBUTE0 ();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TDriveTypes DriveTypes = {read=FDriveTypes, write=SetDriveTypes, default=124};
	__property Color _DEPRECATED_ATTRIBUTE0  = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=SetFlatButtonColor, stored=NotUsingController, default=-16777201};
	__property bool FlatButtons = {read=FFlatButtons, write=SetFlatButtons, stored=NotUsingController, default=0};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=NotUsingController, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=NotUsingController, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=NotUsingController, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=NotUsingController, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=NotUsingController, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=NotUsingController, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=NotUsingController, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, default=1};
	__property Align _DEPRECATED_ATTRIBUTE0  = {default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDriveComboBox(HWND ParentWindow) : Vcl::Filectrl::TDriveComboBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::UnicodeString __fastcall VolumeID(System::WideChar Drive);
extern DELPHI_PACKAGE System::UnicodeString __fastcall NetworkVolume(System::WideChar Drive);
extern DELPHI_PACKAGE void __fastcall GetDriveInfo(System::WideChar Drive, System::UnicodeString &Volume, unsigned &SerialNum);
extern DELPHI_PACKAGE void __fastcall GetVolumeInfo(System::WideChar Drive, TNetworkVolumeFormat VolumeFormat, System::UnicodeString &Volume, unsigned &SerialNum);
extern DELPHI_PACKAGE unsigned __fastcall GetDriveSerialNum(System::WideChar Drive);
extern DELPHI_PACKAGE System::UnicodeString __fastcall UNCPathToDriveMapping(System::UnicodeString UNCPath);
extern DELPHI_PACKAGE System::UnicodeString __fastcall GetCurrentRootDir(void);
}	/* namespace Rzfilsys */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZFILSYS)
using namespace Rzfilsys;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzfilsysHPP
