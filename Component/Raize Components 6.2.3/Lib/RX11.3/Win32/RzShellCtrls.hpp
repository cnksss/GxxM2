// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzShellCtrls.pas' rev: 35.00 (Windows)

#ifndef RzshellctrlsHPP
#define RzshellctrlsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Winapi.ShellAPI.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Winapi.CommCtrl.hpp>
#include <Vcl.ComCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <Winapi.ShlObj.hpp>
#include <Winapi.ActiveX.hpp>
#include <Vcl.ImgList.hpp>
#include <RzShellIntf.hpp>
#include <RzTreeVw.hpp>
#include <RzListVw.hpp>
#include <RzShellUtils.hpp>
#include <RzCmboBx.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzshellctrls
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzShellLocator;
class DELPHICLASS TRzCustomShellTree;
class DELPHICLASS TRzShellTree;
class DELPHICLASS TRzCustomShellList;
class DELPHICLASS TRzShellList;
class DELPHICLASS TRzCustomShellCombo;
class DELPHICLASS TRzShellCombo;
class DELPHICLASS TRzShellTreeData;
class DELPHICLASS TRzShellListData;
class DELPHICLASS TRzShellComboData;
class DELPHICLASS TRzChangeHandlerThread;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TRzShellTreeOption : unsigned char { stoAutoFill, stoVirtualFolders, stoDesignInteractive, stoDefaultKeyHandling, stoContextMenus, stoDynamicRefresh, stoIncludeNonFolders, stoOleDrag, stoOleDrop, stoShowHidden, stoFilesCanBeFolders };

typedef System::Set<TRzShellTreeOption, TRzShellTreeOption::stoAutoFill, TRzShellTreeOption::stoFilesCanBeFolders> TRzShellTreeOptions;

enum DECLSPEC_DENUM TRzShellListOption : unsigned char { sloAutoFill, sloNonFilesystemAncestors, sloDesignInteractive, sloDefaultKeyHandling, sloContextMenus, sloDontChangeFolder, sloDontGoBelowBase, sloDynamicRefresh, sloHideFoldersWhenLinkedToTree, sloOleDrag, sloOleDrop, sloFolderContextMenu, sloShowHidden, sloFilesCanBeFolders };

typedef System::Set<TRzShellListOption, TRzShellListOption::sloAutoFill, TRzShellListOption::sloFilesCanBeFolders> TRzShellListOptions;

enum DECLSPEC_DENUM TRzShellComboOption : unsigned char { scoAutofill, scoNonFilesystemAncestors };

typedef System::Set<TRzShellComboOption, TRzShellComboOption::scoAutofill, TRzShellComboOption::scoNonFilesystemAncestors> TRzShellComboOptions;

typedef void __fastcall (__closure *TRzShAddItemEvent)(System::TObject* Sender, Rzshellintf::IShellFolder_NRC* ParentIShf, Winapi::Shlobj::PItemIDList ParentAbsIdList, Winapi::Shlobj::PItemIDList ItemRelIdList, int Attribs, System::LongBool &AllowAdd);

typedef void __fastcall (__closure *TRzShTreeInsertItemEvent)(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node);

typedef void __fastcall (__closure *TRzShTreeDeleteItemEvent)(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node, TRzShellTreeData* TreeData);

typedef void __fastcall (__closure *TRzShListDeleteItemEvent)(System::TObject* Sender, Vcl::Comctrls::TListItem* Node, TRzShellListData* ListData);

typedef void __fastcall (__closure *TRzShComboDeleteItemEvent)(System::TObject* Sender, int Item, TRzShellComboData* ComboData);

typedef void __fastcall (__closure *TRzShDblClickOpenEvent)(System::TObject* Sender, bool &Handled);

typedef void __fastcall (__closure *TRzShPopupHintEvent)(System::TObject* Sender, const System::UnicodeString Hint);

enum DECLSPEC_DENUM TRzShellLocator_Which : unsigned char { usePidl, useCSIDL };

class PASCALIMPLEMENTATION TRzShellLocator : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
protected:
	TRzShellLocator_Which FWhich;
	_ITEMIDLIST *FIdList;
	Rzshellutils::TCSIDL FCSIDL;
	System::Classes::TNotifyEvent FOnChange;
	DYNAMIC void __fastcall Changed();
	System::UnicodeString __fastcall GetPathName();
	Winapi::Shlobj::PItemIDList __fastcall GetIdList();
	Rzshellutils::TCSIDL __fastcall GetCSIDL();
	void __fastcall SetIdList(Winapi::Shlobj::PItemIDList Value);
	void __fastcall SetPathName(const System::UnicodeString Value);
	void __fastcall SetCSIDL(Rzshellutils::TCSIDL Value);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	
public:
	__fastcall virtual ~TRzShellLocator();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	bool __fastcall IsEqual(TRzShellLocator* Value);
	void __fastcall Clear();
	void __fastcall ReadData(System::Classes::TStream* Stream);
	void __fastcall WriteData(System::Classes::TStream* Stream);
	__property Winapi::Shlobj::PItemIDList IdList = {read=GetIdList, write=SetIdList};
	__property System::UnicodeString PathName = {read=GetPathName, write=SetPathName};
	__property Rzshellutils::TCSIDL CSIDL = {read=GetCSIDL, write=SetCSIDL, nodefault};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
public:
	/* TObject.Create */ inline __fastcall TRzShellLocator() : System::Classes::TPersistent() { }
	
};


class PASCALIMPLEMENTATION TRzCustomShellTree : public Rztreevw::TRzCustomTreeView
{
	typedef Rztreevw::TRzCustomTreeView inherited;
	
private:
	TRzShellLocator* FBaseFolder;
	TRzShellLocator* FSelectedFolder;
	Vcl::Extctrls::TTimer* FTimer;
	TRzCustomShellList* FShellList;
	TRzShellTreeOptions FOptions;
	TRzShAddItemEvent FOnAddItem;
	TRzShTreeDeleteItemEvent FOnDeleteItem;
	TRzShTreeInsertItemEvent FOnInsertItem;
	TRzShPopupHintEvent FOnPopupHint;
	System::Classes::TNotifyEvent FOnFillComplete;
	System::Classes::TNotifyEvent FOnFillStart;
	TRzCustomShellCombo* FShellCombo;
	Rzshellintf::IContextMenu_NRC* FActiveContextMenu;
	bool FQuickSelect;
	int FIgnoreChanges;
	int FIgnoreErrors;
	bool FEatExpand;
	bool FIgnoreNextChangeNotify;
	void *FChangeHandlerThread;
	bool FDeferRefresh;
	bool FRefreshDeferred;
	Vcl::Comctrls::TTreeNode* FLastNode;
	bool FLoaded;
	bool FDeletingNodes;
	bool FSettingParent;
	
protected:
	Vcl::Comctrls::TTreeNode* FDragNode;
	Rzshellintf::IDataObject_NRC* FIDataObject;
	Rzshellintf::IDropTarget_NRC* FIDropTarget;
	Rzshellintf::IDataObject_NRC* FILastDropDataObject;
	int FInitialDropKeyState;
	int FLastAutoScrollTick;
	System::Types::TPoint FLastAutoOpenPos;
	int FLastAutoOpenTick;
	void __fastcall OleBeginDrag(System::Uitypes::TMouseButton Button);
	virtual Rzshellintf::IDropSource_NRC* __fastcall CreateIDropSource(System::Uitypes::TMouseButton Button, Rzshellintf::IDataObject_NRC* DataObject);
	virtual HRESULT __fastcall OnDropTarget_DragEnter(Rzshellintf::IDataObject_NRC* const dataObj, int grfKeyState, const System::Types::TPoint &pt, int &dwEffect);
	virtual HRESULT __fastcall OnDropTarget_DragOver(int grfKeyState, const System::Types::TPoint &pt, int &dwEffect);
	virtual HRESULT __fastcall OnDropTarget_DragLeave();
	virtual HRESULT __fastcall OnDropTarget_Drop(Rzshellintf::IDataObject_NRC* const dataObj, int grfKeyState, const System::Types::TPoint &pt, int &dwEffect);
	virtual void __fastcall DoDropTargetAutoscroll(const System::Types::TPoint &pt);
	
private:
	void __fastcall InitImageList();
	void __fastcall BaseFolderChanged(System::TObject* Sender);
	void __fastcall SelectedFolderChanged(System::TObject* Sender);
	void __fastcall TimerElapsed(System::TObject* Sender);
	TRzShellTreeData* __fastcall GetSelectedItem();
	System::UnicodeString __fastcall GetSelectedPathName();
	TRzShellTreeData* __fastcall GetShellTreeData(int idx);
	HWND __fastcall GetParentHWND();
	void __fastcall SetBaseFolder(TRzShellLocator* Value);
	void __fastcall SetShellList(TRzCustomShellList* Value);
	void __fastcall SetOptions(TRzShellTreeOptions Value);
	void __fastcall SetSelectedFolder(TRzShellLocator* Value);
	void __fastcall DoSetSelectedIdList(Winapi::Shlobj::PItemIDList Value);
	void __fastcall SetShellCombo(TRzCustomShellCombo* Value);
	MESSAGE void __fastcall AMChangeNotify(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall AMDeferredFill(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall CMWantSpecialKey(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CNNotify(Winapi::Messages::TWMNotify &Msg);
	MESSAGE void __fastcall TVMDeleteItem(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMMenuChar(Winapi::Messages::TWMMenuChar &Msg);
	HIDESBASE MESSAGE void __fastcall WMDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCDestroy(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	HIDESBASE MESSAGE void __fastcall WMMeasureItem(Winapi::Messages::TWMMeasureItem &Msg);
	MESSAGE void __fastcall WMMenuSelect(Winapi::Messages::TWMMenuSelect &Msg);
	MESSAGE void __fastcall WMInitMenuPopup(Winapi::Messages::TWMInitMenuPopup &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	
protected:
	void __fastcall SetSelectedPathName(const System::UnicodeString Value);
	virtual HRESULT __fastcall AddNewShellNode(Rzshellintf::IShellFolder_NRC* Parent, Winapi::Shlobj::PItemIDList ParentAbsIdList, Vcl::Comctrls::TTreeNode* ParentNode, Winapi::Shlobj::PItemIDList RelIdList);
	virtual System::LongBool __fastcall CanAdd(Rzshellintf::IShellFolder_NRC* ParentIShf, Winapi::Shlobj::PItemIDList ParentAbsPidl, Winapi::Shlobj::PItemIDList ItemRelPidl, unsigned Attribs);
	DYNAMIC bool __fastcall CanExpand(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC bool __fastcall CanEdit(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Change(Vcl::Comctrls::TTreeNode* Node);
	virtual void __fastcall DeviceChangeDetected(System::TObject* Sender, Winapi::Messages::TMessage &Msg);
	DYNAMIC void __fastcall DoOnInsertItem(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall NodeContextMenu(Vcl::Comctrls::TTreeNode* Node, System::Types::TPoint &P, Vcl::Menus::TPopupMenu* &Menu);
	bool __fastcall NodeHasData(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Delete(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall ExpandNode(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Edit(const tagTVITEMW &Item);
	virtual void __fastcall FillTree(Rzshellintf::IShellFolder_NRC* ishf, Vcl::Comctrls::TTreeNode* ABaseNode);
	DYNAMIC void __fastcall FillComplete();
	DYNAMIC void __fastcall FillStart();
	Vcl::Comctrls::TTreeNode* __fastcall GetFirstRootLevelShellNode();
	virtual void __fastcall GetImageIndex(Vcl::Comctrls::TTreeNode* Node);
	virtual void __fastcall GetSelectedIndex(Vcl::Comctrls::TTreeNode* Node);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* Component, System::Classes::TOperation Operation);
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DestroyWnd();
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* AParent);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState ShiftState);
	DYNAMIC void __fastcall ProcessSendTo(Vcl::Comctrls::TTreeNode* Node, int Index);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC void __fastcall InstallChangeHandler(const System::UnicodeString PathName);
	void __fastcall InstallChangeHandlersForAllLocalDrives();
	__property TRzShellTreeData* SelectedItem = {read=GetSelectedItem};
	__property TRzShellLocator* SelectedFolder = {read=FSelectedFolder, write=SetSelectedFolder};
	__property System::UnicodeString SelectedPathName = {read=GetSelectedPathName, write=SetSelectedPathName};
	__property TRzShellTreeData* ShellTreeData[int Index] = {read=GetShellTreeData};
	__property Items = {stored=false};
	__property TRzShellLocator* BaseFolder = {read=FBaseFolder, write=SetBaseFolder};
	__property TRzCustomShellList* ShellList = {read=FShellList, write=SetShellList};
	__property TRzShellTreeOptions Options = {read=FOptions, write=SetOptions, default=955};
	__property TRzShAddItemEvent OnAddItem = {read=FOnAddItem, write=FOnAddItem};
	__property TRzShTreeDeleteItemEvent OnDeleteItem = {read=FOnDeleteItem, write=FOnDeleteItem};
	__property TRzShTreeInsertItemEvent OnInsertItem = {read=FOnInsertItem, write=FOnInsertItem};
	__property TRzShPopupHintEvent OnPopupHint = {read=FOnPopupHint, write=FOnPopupHint};
	__property System::Classes::TNotifyEvent OnFillComplete = {read=FOnFillComplete, write=FOnFillComplete};
	__property System::Classes::TNotifyEvent OnFillStart = {read=FOnFillStart, write=FOnFillStart};
	
public:
	__fastcall virtual TRzCustomShellTree(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomShellTree();
	void __fastcall FillItems();
	DYNAMIC void __fastcall RefreshNodes()/* overload */;
	DYNAMIC void __fastcall RefreshNodes(Vcl::Comctrls::TTreeNode* Node)/* overload */;
	TRzShellTreeData* __fastcall GetDataFromNode(Vcl::Comctrls::TTreeNode* Node);
	void __fastcall GoUp(int Levels);
	bool __fastcall CreateNewFolder(bool EditNow);
	HRESULT __fastcall DoCommandForNode(Vcl::Comctrls::TTreeNode* Node, char * Cmd);
	Vcl::Comctrls::TTreeNode* __fastcall FindNodeWithIdList(Vcl::Comctrls::TTreeNode* BaseNode, Winapi::Shlobj::PItemIDList pidl);
	DYNAMIC void __fastcall ProcessMenu(Vcl::Comctrls::TTreeNode* Node, const System::Types::TPoint &P);
	DYNAMIC void __fastcall SortNode(Vcl::Comctrls::TTreeNode* Node);
	void __fastcall Synchronize(bool afApplyToGroup);
	unsigned __fastcall GetDragDropAttributesForNode(Vcl::Comctrls::TTreeNode* Node);
	__property Height = {default=150};
	__property Width = {default=150};
	__property ItemHeightMargin = {default=3};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomShellTree(HWND ParentWindow) : Rztreevw::TRzCustomTreeView(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzShellTree : public TRzCustomShellTree
{
	typedef TRzCustomShellTree inherited;
	
public:
	__property SelectedItem;
	__property SelectedFolder;
	__property SelectedPathName = {default=0};
	__property ShellTreeData;
	__property Items;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoExpand = {default=0};
	__property BaseFolder;
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property BorderWidth = {default=0};
	__property ChangeDelay = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property FrameHotColor = {default=-16777200};
	__property FrameHotTrack = {default=0};
	__property FrameHotStyle = {default=10};
	__property FrameSides = {default=15};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property HideSelection = {default=1};
	__property HotTrack = {default=0};
	__property Indent;
	__property ItemHeight = {default=0};
	__property ItemHeightMargin = {default=3};
	__property MultiSelect = {default=0};
	__property MultiSelectStyle = {default=1};
	__property Options = {default=955};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property RightClickSelect = {default=0};
	__property RowSelect = {default=0};
	__property SelectionPen;
	__property ShellList;
	__property ShowButtons = {default=1};
	__property ShowHint;
	__property ShowLines = {default=1};
	__property ShowRoot = {default=0};
	__property StateImages;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property ToolTips = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnAddItem;
	__property OnAdvancedCustomDraw;
	__property OnAdvancedCustomDrawItem;
	__property OnCancelEdit;
	__property OnChange;
	__property OnChanging;
	__property OnClick;
	__property OnCollapsed;
	__property OnCollapsing;
	__property OnContextPopup;
	__property OnCustomDraw;
	__property OnCustomDrawItem;
	__property OnDblClick;
	__property OnDeleteItem;
	__property OnDeletion;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEdited;
	__property OnEditing;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnExpanded;
	__property OnExpanding;
	__property OnFillComplete;
	__property OnFillStart;
	__property OnGesture;
	__property OnGetImageIndex;
	__property OnGetSelectedIndex;
	__property OnInsertItem;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnNodeContextMenu;
	__property OnPopupHint;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomShellTree.Create */ inline __fastcall virtual TRzShellTree(System::Classes::TComponent* AOwner) : TRzCustomShellTree(AOwner) { }
	/* TRzCustomShellTree.Destroy */ inline __fastcall virtual ~TRzShellTree() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzShellTree(HWND ParentWindow) : TRzCustomShellTree(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzCustomShellList_LCS : unsigned char { lcsNone, lcsShellDetails, lcsDefault };

class PASCALIMPLEMENTATION TRzCustomShellList : public Rzlistvw::TRzCustomListView
{
	typedef Rzlistvw::TRzCustomListView inherited;
	
private:
	TRzCustomShellList_LCS FLastColState;
	bool FLastFolderWasDir;
	TRzShPopupHintEvent FOnPopupHint;
	Rzshellintf::IShellFolder_NRC* FIShf;
	_ITEMIDLIST *FIShfPidl;
	bool FIShellDetailsValid;
	Rzshellintf::IShellDetails_NRC* FIShellDetails;
	TRzShellLocator* FFolder;
	TRzShellListOptions FOptions;
	TRzShAddItemEvent FOnAddItem;
	TRzShListDeleteItemEvent FOnDeleteItem;
	TRzShDblClickOpenEvent FOnDblClickOpen;
	System::Classes::TNotifyEvent FOnFillComplete;
	System::Classes::TNotifyEvent FOnFillStart;
	System::Classes::TNotifyEvent FOnFolderChanged;
	System::UnicodeString FFileFilter;
	void *FFilterLookupTable;
	System::Classes::TStringList* FFilterExtensions;
	Vcl::Extctrls::TTimer* FTimer;
	TRzCustomShellTree* FShellTree;
	TRzCustomShellCombo* FShellCombo;
	Rzshellintf::IContextMenu_NRC* FActiveContextMenu;
	int FSortColumn;
	int FIgnoreChanges;
	bool FIgnoreNextChangeNotify;
	void *FChangeHandlerThread;
	System::UnicodeString FDeferredEditName;
	bool FSkipRButtonUp;
	bool FDeferRefresh;
	bool FRefreshDeferred;
	TRzShellListData* FCurrentItemData;
	unsigned FCurrentItemFlags;
	Rzshellintf::IShellDetails_NRC* FCurrentItemIShd;
	int FLastAutoScrollTick;
	bool FLoaded;
	bool FInCreateWnd;
	bool FSortColumnAssigned;
	bool FDeletingItems;
	
protected:
	Rzshellintf::IDataObject_NRC* FIDataObject;
	void __fastcall OleBeginDrag(System::Uitypes::TMouseButton Button);
	Rzshellintf::IDropTarget_NRC* FIDropTarget;
	Rzshellintf::IDataObject_NRC* FILastDropDataObject;
	int FInitialDropKeyState;
	virtual Rzshellintf::IDropSource_NRC* __fastcall CreateIDropSource(System::Uitypes::TMouseButton Button, Rzshellintf::IDataObject_NRC* DataObject);
	virtual HRESULT __fastcall OnDropTarget_DragEnter(Rzshellintf::IDataObject_NRC* const dataObj, int grfKeyState, const System::Types::TPoint &pt, int &dwEffect);
	virtual HRESULT __fastcall OnDropTarget_DragOver(int grfKeyState, const System::Types::TPoint &pt, int &dwEffect);
	virtual HRESULT __fastcall OnDropTarget_DragLeave();
	virtual HRESULT __fastcall OnDropTarget_Drop(Rzshellintf::IDataObject_NRC* const dataObj, int grfKeyState, const System::Types::TPoint &pt, int &dwEffect);
	
private:
	void __fastcall InitColumns(Rzshellintf::IShellDetails_NRC* ishd);
	void __fastcall InitImageLists();
	bool __fastcall IsFolderStored();
	Rzshellintf::IShellDetails_NRC* __fastcall GetCurrentFolderIShellDetails();
	void __fastcall HandleOnFolderChanged(System::TObject* Sender);
	TRzShellListData* __fastcall GetSelectedItem();
	TRzShellListData* __fastcall GetShellListData(int Index);
	void __fastcall SetFileFilter(const System::UnicodeString Value);
	void __fastcall SetFolder(TRzShellLocator* Value);
	void __fastcall SetShellTree(TRzCustomShellTree* Value);
	void __fastcall SetShellCombo(TRzCustomShellCombo* Value);
	void __fastcall SetSortColumn(int Value);
	void __fastcall SetOptions(TRzShellListOptions Value);
	void __fastcall TimerElapsed(System::TObject* Sender);
	MESSAGE void __fastcall AMChangeNotify(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall AMDeferredEdit(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall AMDeferredFill(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall CMWantSpecialKey(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CNNotify(Winapi::Messages::TWMNotify &Msg);
	MESSAGE void __fastcall WMMenuChar(Winapi::Messages::TWMMenuChar &Msg);
	HIDESBASE MESSAGE void __fastcall WMDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall WMDestroy(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCDestroy(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	HIDESBASE MESSAGE void __fastcall WMMeasureItem(Winapi::Messages::TWMMeasureItem &Msg);
	MESSAGE void __fastcall WMMenuSelect(Winapi::Messages::TWMMenuSelect &Msg);
	HIDESBASE MESSAGE void __fastcall WMRButtonUp(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall WMInitMenuPopup(Winapi::Messages::TWMInitMenuPopup &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	MESSAGE void __fastcall WMGetIShellBrowser(Winapi::Messages::TMessage &Msg);
	
protected:
	bool __fastcall ShouldInclude(Winapi::Shlobj::PItemIDList baseidlist, Winapi::Shlobj::PItemIDList relidlist, unsigned &attrib);
	HRESULT __fastcall AddNewShellItem(Winapi::Shlobj::PItemIDList AbsIdList, Winapi::Shlobj::PItemIDList RelIdList);
	virtual System::LongBool __fastcall CanAdd(Rzshellintf::IShellFolder_NRC* parentIShf, Winapi::Shlobj::PItemIDList parentAbsPidl, Winapi::Shlobj::PItemIDList itemRelPidl, unsigned itemAttributes);
	DYNAMIC bool __fastcall CanEdit(Vcl::Comctrls::TListItem* Item);
	DYNAMIC bool __fastcall DblClickOpen();
	virtual void __fastcall DeviceChangeDetected(System::TObject* Sender, Winapi::Messages::TMessage &Msg);
	DYNAMIC void __fastcall Edit(const tagLVITEMW &Item);
	virtual void __fastcall FillList(Rzshellintf::IShellFolder_NRC* ishf, Winapi::Shlobj::PItemIDList basepidl);
	DYNAMIC void __fastcall FillComplete();
	DYNAMIC void __fastcall FillStart();
	DYNAMIC void __fastcall FolderChanged();
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* Component, System::Classes::TOperation Operation);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DestroyWnd();
	void __fastcall TreeChanged(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Delete(Vcl::Comctrls::TListItem* Item);
	DYNAMIC void __fastcall ColClick(Vcl::Comctrls::TListColumn* Column);
	DYNAMIC HRESULT __fastcall GetUIObjectForAllSelected(const GUID &riid, void * &interfaceOut);
	DYNAMIC HRESULT __fastcall GetUIObjectForItem(Vcl::Comctrls::TListItem* Item, const GUID &riid, void * &interfaceOut);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState ShiftState);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int x, int y);
	DYNAMIC void __fastcall ItemContextMenu(Vcl::Comctrls::TListItem* Item, System::Types::TPoint &P, Vcl::Menus::TPopupMenu* &Menu);
	bool __fastcall ItemHasData(Vcl::Comctrls::TListItem* Item);
	DYNAMIC void __fastcall InsertItem(Vcl::Comctrls::TListItem* Item);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC void __fastcall InstallChangeHandler();
	virtual void __fastcall FilterPreApply();
	virtual bool __fastcall FilterApply(const System::UnicodeString FileName, unsigned Attrib);
	virtual void __fastcall FilterPostApply();
	bool __fastcall IsFolderNetworkShare();
	__property Columns = {stored=false};
	__property Items = {stored=false};
	__property AllocBy = {stored=false, default=0};
	__property TRzShellListData* SelectedItem = {read=GetSelectedItem};
	__property TRzShellListData* ShellListData[int Index] = {read=GetShellListData};
	__property int SortColumn = {read=FSortColumn, write=SetSortColumn, nodefault};
	__property Rzshellintf::IShellFolder_NRC* _IShellFolder = {read=FIShf};
	__property TRzShellListOptions Options = {read=FOptions, write=SetOptions, default=16027};
	__property TRzShellLocator* Folder = {read=FFolder, write=SetFolder, stored=IsFolderStored};
	__property System::UnicodeString FileFilter = {read=FFileFilter, write=SetFileFilter};
	__property TRzShAddItemEvent OnAddItem = {read=FOnAddItem, write=FOnAddItem};
	__property TRzShListDeleteItemEvent OnDeleteItem = {read=FOnDeleteItem, write=FOnDeleteItem};
	__property TRzShPopupHintEvent OnPopupHint = {read=FOnPopupHint, write=FOnPopupHint};
	__property TRzShDblClickOpenEvent OnDblClickOpen = {read=FOnDblClickOpen, write=FOnDblClickOpen};
	__property System::Classes::TNotifyEvent OnFillComplete = {read=FOnFillComplete, write=FOnFillComplete};
	__property System::Classes::TNotifyEvent OnFillStart = {read=FOnFillStart, write=FOnFillStart};
	__property System::Classes::TNotifyEvent OnFolderChanged = {read=FOnFolderChanged, write=FOnFolderChanged};
	
public:
	__fastcall virtual TRzCustomShellList(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomShellList();
	bool __fastcall CreateNewFolder(bool EditNow);
	void __fastcall DoCommandForItem(Vcl::Comctrls::TListItem* Item, char * Cmd);
	void __fastcall DoCommandForAllSelected(char * Cmd);
	void __fastcall DoCommandForFolder(char * Cmd);
	void __fastcall FillItems();
	void __fastcall GoUp(int Levels);
	TRzShellListData* __fastcall GetDataFromItem(Vcl::Comctrls::TListItem* Item);
	void __fastcall OpenItem(Vcl::Comctrls::TListItem* Item);
	void __fastcall OpenSelectedItems();
	void __fastcall ProcessMenu(Vcl::Comctrls::TListItem* Item, const System::Types::TPoint &Pt);
	void __fastcall ProcessMenuForAllSelected(const System::Types::TPoint &Pt);
	void __fastcall ProcessSendTo(int Index);
	void __fastcall RefreshItems();
	unsigned __fastcall GetDragDropAttributesForAllSelected();
	unsigned __fastcall GetDragDropAttributesForItem(Vcl::Comctrls::TListItem* Item);
	virtual void __fastcall SelectAll();
	int __fastcall ShellSelCount();
	void __fastcall SortList();
	void __fastcall Synchronize(bool ApplyToGroup);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomShellList(HWND ParentWindow) : Rzlistvw::TRzCustomListView(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzShellList : public TRzCustomShellList
{
	typedef TRzCustomShellList inherited;
	
public:
	__property SelectedItem;
	__property ShellListData;
	__property SortColumn;
	__property Columns;
	__property Items;
	__property _IShellFolder;
	__property HeaderCanvas;
	__property HeaderHandle;
	__property HeaderSortColumn = {default=-1};
	__property HeaderSortDirection = {default=0};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property BorderWidth = {default=0};
	__property Checkboxes = {default=0};
	__property Color = {default=-16777211};
	__property ColumnClick = {default=1};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FileFilter = {default=0};
	__property FlatScrollBars = {default=0};
	__property Folder;
	__property Font;
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property FrameHotColor = {default=-16777200};
	__property FrameHotTrack = {default=0};
	__property FrameHotStyle = {default=10};
	__property FrameSides = {default=15};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property FullDrag = {default=0};
	__property GridLines = {default=0};
	__property HeaderSortDisplayMode = {default=2};
	__property HideSelection = {default=1};
	__property HotTrack = {default=0};
	__property HotTrackStyles = {default=0};
	__property IconOptions;
	__property MultiSelect = {default=0};
	__property Options = {default=16027};
	__property OwnerData = {default=0};
	__property OwnerDraw = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property RowSelect = {default=0};
	__property ShowColumnHeaders = {default=1};
	__property ShowHint;
	__property StateImages;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property ViewStyle = {default=0};
	__property Visible = {default=1};
	__property OnAddItem;
	__property OnAdvancedCustomDraw;
	__property OnAdvancedCustomDrawItem;
	__property OnAdvancedCustomDrawSubItem;
	__property OnChange;
	__property OnChanging;
	__property OnClick;
	__property OnColumnClick;
	__property OnCompare;
	__property OnContextPopup;
	__property OnCustomDraw;
	__property OnCustomDrawItem;
	__property OnData;
	__property OnDataFind;
	__property OnDataHint;
	__property OnDataStateChange;
	__property OnDblClick;
	__property OnDblClickOpen;
	__property OnDeleteItem;
	__property OnDeletion;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawItem;
	__property OnEdited;
	__property OnEditing;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnFillComplete;
	__property OnFillStart;
	__property OnFolderChanged;
	__property OnGesture;
	__property OnGetImageIndex;
	__property OnInfoTip;
	__property OnInsert;
	__property OnItemContextMenu;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnPopupHint;
	__property OnResize;
	__property OnSelectItem;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomShellList.Create */ inline __fastcall virtual TRzShellList(System::Classes::TComponent* AOwner) : TRzCustomShellList(AOwner) { }
	/* TRzCustomShellList.Destroy */ inline __fastcall virtual ~TRzShellList() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzShellList(HWND ParentWindow) : TRzCustomShellList(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzCustomShellCombo : public Rzcmbobx::TRzCustomImageComboBox
{
	typedef Rzcmbobx::TRzCustomImageComboBox inherited;
	
private:
	int FIgnoreChanges;
	TRzCustomShellTree* FShellTree;
	TRzCustomShellList* FShellList;
	TRzShellLocator* FSelectedFolder;
	TRzShellComboOptions FOptions;
	bool FAutoDropDownCount;
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	TRzShellLocator* __fastcall GetSelectedFolder();
	TRzShellComboData* __fastcall GetShellComboData(int Index);
	void __fastcall SetSelectedFolder(TRzShellLocator* Value);
	void __fastcall SetShellList(TRzCustomShellList* Value);
	void __fastcall SetShellTree(TRzCustomShellTree* Value);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	
protected:
	virtual bool __fastcall CanAdd(Rzshellintf::IShellFolder_NRC* ParentIShf, Winapi::Shlobj::PItemIDList ParentAbsPidl, Winapi::Shlobj::PItemIDList ItemRelPidl, unsigned attribs, int Level);
	DYNAMIC void __fastcall SelEndOk();
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DestroyWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall DeleteItem(void * Item);
	DYNAMIC void __fastcall DropDown();
	virtual void __fastcall GetItemData(Rzcmbobx::TRzImageComboBoxItem* Item);
	virtual void __fastcall SelectedFolderChanged(System::TObject* Sender);
	virtual void __fastcall TreeChanged(Vcl::Comctrls::TTreeNode* Node);
	virtual void __fastcall DeviceChangeDetected(System::TObject* Sender, Winapi::Messages::TMessage &Msg);
	virtual void __fastcall FillCombo(Rzshellintf::IShellFolder_NRC* aIShf, Winapi::Shlobj::PItemIDList aBasePidl, int aIndent, Rzshellutils::TRzIdListArray* aSelectedItem);
	__property bool AutoDropDownCount = {read=FAutoDropDownCount, write=FAutoDropDownCount, default=1};
	__property TRzCustomShellTree* ShellTree = {read=FShellTree, write=SetShellTree};
	__property TRzCustomShellList* ShellList = {read=FShellList, write=SetShellList};
	__property ItemIndent = {default=12};
	__property TRzShellComboOptions Options = {read=FOptions, write=FOptions, default=3};
	__property TRzShellLocator* SelectedFolder = {read=GetSelectedFolder, write=SetSelectedFolder};
	__property TRzShellComboData* ShellComboData[int Index] = {read=GetShellComboData};
	__property Items = {stored=false};
	
public:
	__fastcall virtual TRzCustomShellCombo(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomShellCombo();
	void __fastcall FillItems();
	void __fastcall GoUp(int Levels);
	void __fastcall Synchronize(bool ApplyToGroup);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomShellCombo(HWND ParentWindow) : Rzcmbobx::TRzCustomImageComboBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzShellCombo : public TRzCustomShellCombo
{
	typedef TRzCustomShellCombo inherited;
	
public:
	__property SelectedFolder;
	__property ShellComboData;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoDropDownCount = {default=1};
	__property BiDiMode;
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property DropDownCount = {default=8};
	__property DropDownWidth = {default=0};
	__property Enabled = {default=1};
	__property FlatButtons = {default=0};
	__property Font;
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property FrameHotColor = {default=-16777200};
	__property FrameHotTrack = {default=0};
	__property FrameHotStyle = {default=10};
	__property FrameSides = {default=15};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property ItemIndent = {default=12};
	__property Options = {default=3};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShellList;
	__property ShellTree;
	__property ShowHint;
	__property Sorted = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnChange;
	__property OnClick;
	__property OnCloseUp;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDeleteItem;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawItem;
	__property OnDropDown;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMeasureItem;
	__property OnSelEndCancel;
	__property OnSelEndOk;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomShellCombo.Create */ inline __fastcall virtual TRzShellCombo(System::Classes::TComponent* AOwner) : TRzCustomShellCombo(AOwner) { }
	/* TRzCustomShellCombo.Destroy */ inline __fastcall virtual ~TRzShellCombo() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzShellCombo(HWND ParentWindow) : TRzCustomShellCombo(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellTreeData : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	TRzCustomShellTree* FOwner;
	TRzShellTreeData* FParent;
	_ITEMIDLIST *FRelPidl;
	_ITEMIDLIST *FAbsPidl;
	Rzshellintf::IShellFolder_NRC* FThisIshf;
	Rzshellintf::IShellFolder_NRC* FParentIShf;
	void *FData;
	Winapi::Shlobj::PItemIDList __fastcall GetAbsPidl();
	unsigned __fastcall GetAttributes();
	bool __fastcall GetEditable();
	System::UnicodeString __fastcall GetPathName();
	Rzshellintf::IShellFolder_NRC* __fastcall GetParentIShf();
	Rzshellintf::IShellFolder_NRC* __fastcall GetThisIShf();
	void __fastcall SetData(TRzShellTreeData* AParent, Winapi::Shlobj::PItemIDList thisRelativePidl);
	void __fastcall SetRelPidl(Winapi::Shlobj::PItemIDList newRelPidl);
	
public:
	__fastcall TRzShellTreeData(TRzCustomShellTree* AOwner);
	__fastcall virtual ~TRzShellTreeData();
	HRESULT __fastcall GetIDropTarget(HWND h, Rzshellintf::IDropTarget_NRC* &idt);
	bool __fastcall IsRootDir();
	void __fastcall Flush();
	__property unsigned Attributes = {read=GetAttributes, nodefault};
	__property TRzShellTreeData* Parent = {read=FParent};
	__property Rzshellintf::IShellFolder_NRC* ParentIShf = {read=GetParentIShf};
	__property Winapi::Shlobj::PItemIDList AbsoluteIdList = {read=GetAbsPidl};
	__property Winapi::Shlobj::PItemIDList RelativeIdList = {read=FRelPidl};
	__property Rzshellintf::IShellFolder_NRC* ThisIShf = {read=GetThisIShf};
	__property System::UnicodeString PathName = {read=GetPathName};
	__property bool Editable = {read=GetEditable, nodefault};
	__property void * Data = {read=FData, write=FData};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellListData : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	TRzCustomShellList* FOwner;
	_ITEMIDLIST *FAbsPidl;
	_ITEMIDLIST *FRelPidl;
	void *FData;
	System::UnicodeString FDisplayName;
	bool FDataValid;
	System::UnicodeString FSize;
	System::UnicodeString FType;
	System::UnicodeString FModified;
	Winapi::Shlobj::PItemIDList __fastcall GetAbsoluteIdList();
	unsigned __fastcall GetAttributes();
	bool __fastcall GetEditable();
	System::UnicodeString __fastcall GetDisplayName();
	System::UnicodeString __fastcall GetFilename();
	System::UnicodeString __fastcall GetPathName();
	System::UnicodeString __fastcall GetColText(int col);
	void __fastcall GetExtraData();
	System::UnicodeString __fastcall GetSize();
	System::UnicodeString __fastcall GetFileType();
	System::UnicodeString __fastcall GetModified();
	void __fastcall SetData(Winapi::Shlobj::PItemIDList aRelPidl);
	
public:
	__fastcall TRzShellListData(TRzCustomShellList* aOwner);
	__fastcall virtual ~TRzShellListData();
	bool __fastcall IsFolder();
	bool __fastcall IsLnkShortcut();
	bool __fastcall IsValid();
	bool __fastcall IsFileSystem();
	void __fastcall Flush();
	__property TRzCustomShellList* Owner = {read=FOwner};
	__property Winapi::Shlobj::PItemIDList AbsoluteIdList = {read=GetAbsoluteIdList};
	__property bool Editable = {read=GetEditable, nodefault};
	__property Winapi::Shlobj::PItemIDList RelativeIdList = {read=FRelPidl};
	__property unsigned Attributes = {read=GetAttributes, nodefault};
	__property System::UnicodeString DisplayName = {read=GetDisplayName};
	__property System::UnicodeString FileName = {read=GetFilename};
	__property System::UnicodeString PathName = {read=GetPathName};
	__property System::UnicodeString ColText[int Col] = {read=GetColText};
	__property System::UnicodeString Size = {read=GetSize};
	__property System::UnicodeString FileType = {read=GetFileType};
	__property System::UnicodeString Modified = {read=GetModified};
	__property void * Data = {read=FData, write=FData};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzShellComboData : public System::TObject
{
	typedef System::TObject inherited;
	
protected:
	TRzCustomShellCombo* FOwner;
	Rzshellintf::IShellFolder_NRC* FIShfParent;
	_ITEMIDLIST *FParentPidl;
	_ITEMIDLIST *FRelPidl;
	_ITEMIDLIST *FAbsPidl;
	Rzshellintf::IShellFolder_NRC* FThisIshf;
	void *FData;
	Rzshellintf::IShellFolder_NRC* __fastcall GetThisIShf();
	void __fastcall SetData(Rzshellintf::IShellFolder_NRC* aParentIShf, Winapi::Shlobj::PItemIDList parentPidl, Winapi::Shlobj::PItemIDList curRelativePidl);
	
public:
	__fastcall TRzShellComboData(TRzCustomShellCombo* aOwner);
	__fastcall virtual ~TRzShellComboData();
	__property Winapi::Shlobj::PItemIDList ParentIdList = {read=FParentPidl};
	__property Rzshellintf::IShellFolder_NRC* ParentIShf = {read=FIShfParent};
	__property Winapi::Shlobj::PItemIDList RelativeIdList = {read=FRelPidl};
	__property Winapi::Shlobj::PItemIDList AbsoluteIdList = {read=FAbsPidl};
	__property Rzshellintf::IShellFolder_NRC* ThisIShf = {read=GetThisIShf};
	__property void * Data = {read=FData, write=FData};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzChangeHandlerThread : public System::Classes::TThread
{
	typedef System::Classes::TThread inherited;
	
	
private:
	enum DECLSPEC_DENUM _TRzChangeHandlerThread__1 : unsigned char { taAddNewHandler, taRemoveHandler, taRemoveAllMonitors, taTerminate };
	
	
protected:
	bool FTerminateEx;
	NativeUInt FDoActionEvent;
	NativeUInt FActionProcessedEvent;
	NativeUInt FPendingChangeHandler;
	int FPendingRemoveIndex;
	HWND FOwnerWnd;
	unsigned FChangeMsgId;
	System::Classes::TList* FChangeHandlerList;
	int FNewHandlerIndex;
	_TRzChangeHandlerThread__1 FThreadAction;
	
public:
	__fastcall TRzChangeHandlerThread(HWND OwnerWnd, unsigned ChangeMsgId);
	__fastcall virtual ~TRzChangeHandlerThread();
	virtual void __fastcall Execute();
	bool __fastcall AddMonitorDir(System::UnicodeString Pathname, unsigned Flags, bool Recursive, void * &Token);
	void __fastcall RemoveAllMonitors();
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word RZSH_AM_CHANGE_NOTIFY = System::Word(0x465);
static const System::Word RZSH_AM_DEFERRED_EDIT = System::Word(0x466);
static const System::Word RZSH_AM_DEFERRED_FILL = System::Word(0x467);
static const System::Word RZSH_AM_LAST = System::Word(0x496);
extern DELPHI_PACKAGE int RZSH_CHANGE_NOTIFY_DELAY;
extern DELPHI_PACKAGE int RZSH_CHANGE_NOTIFY_FASTDELAY;
extern DELPHI_PACKAGE int RZSH_TREE_KEY_UPDATE_DELAY;
extern DELPHI_PACKAGE int RZSH_MAX_FOLDER_ATTEMPTS;
extern DELPHI_PACKAGE int RZSH_AUTOSCROLL_THRESHOLD_X;
extern DELPHI_PACKAGE int RZSH_AUTOSCROLL_THRESHOLD_Y;
extern DELPHI_PACKAGE int RZSH_AUTOSCROLL_MINDELAY_MS;
extern DELPHI_PACKAGE int RZSH_AUTOOPEN_DELAY_MS;
extern DELPHI_PACKAGE int RZSH_AUTOOPEN_THRESHOLD_X;
extern DELPHI_PACKAGE int RZSH_AUTOOPEN_THRESHOLD_Y;
extern DELPHI_PACKAGE System::Word CF_IDLIST;
extern DELPHI_PACKAGE bool __fastcall RzShCreateNewFolder(System::UnicodeString aPathname, System::UnicodeString &aNewName);
extern DELPHI_PACKAGE bool __fastcall RzShIsFolder(unsigned Attributes, bool CanTreatFilesAsFolders);
}	/* namespace Rzshellctrls */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSHELLCTRLS)
using namespace Rzshellctrls;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzshellctrlsHPP
