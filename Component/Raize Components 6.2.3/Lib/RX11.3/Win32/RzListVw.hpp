// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzListVw.pas' rev: 35.00 (Windows)

#ifndef RzlistvwHPP
#define RzlistvwHPP

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
#include <Vcl.Forms.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ComCtrls.hpp>
#include <Winapi.CommCtrl.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Menus.hpp>
#include <System.SysUtils.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>
#include <Vcl.ImgList.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlistvw
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomListView;
class DELPHICLASS TRzListView;
//-- type declarations -------------------------------------------------------
typedef void __fastcall (__closure *TRzLVCheckStateChanging)(System::TObject* Sender, Vcl::Comctrls::TListItem* Item, bool ToBeChecked, bool &AllowChange);

typedef void __fastcall (__closure *TRzLVOnItemContextMenuEvent)(System::TObject* Sender, Vcl::Comctrls::TListItem* Item, System::Types::TPoint &Pos, Vcl::Menus::TPopupMenu* &Menu);

typedef void __fastcall (__closure *TRzLVDrawHeaderEvent)(System::TObject* Sender, Vcl::Graphics::TCanvas* Canvas, int Index, const System::Types::TRect &Rect);

enum DECLSPEC_DENUM TRzLVSortDirection : unsigned char { sdAscending, sdDescending };

enum DECLSPEC_DENUM TRzLVHeaderSortDisplayMode : unsigned char { hsdmNone, hsdmLeftAlign, hsdmRightOfText, hsdmRightAlign };

class PASCALIMPLEMENTATION TRzCustomListView : public Vcl::Comctrls::TCustomListView
{
	typedef Vcl::Comctrls::TCustomListView inherited;
	
private:
	bool FAlphaSortAll;
	int FLastIndex;
	bool FFillLastColumn;
	bool FEditOnRowClick;
	bool FEditingCaption;
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FNormalColor;
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	System::Uitypes::TColor FFrameHotColor;
	bool FFrameHotTrack;
	Rzcommon::TFrameStyleEx FFrameHotStyle;
	Rzcommon::TSides FFrameSides;
	Rzcommon::TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	Rzcommon::TFramingPreference FFramingPreference;
	TRzLVCheckStateChanging FOnCheckStateChanging;
	Vcl::Comctrls::TLVNotifyEvent FOnCheckStateChange;
	TRzLVOnItemContextMenuEvent FOnItemContextMenu;
	bool FMenuAlreadyHandled;
	bool FDragStarted;
	Vcl::Graphics::TCanvas* FHeaderCanvas;
	int FHeaderSortColumn;
	TRzLVSortDirection FHeaderSortDirection;
	TRzLVHeaderSortDisplayMode FHeaderSortDisplayMode;
	bool FHeaderDefaultDrawing;
	TRzLVDrawHeaderEvent FOnDrawHeader;
	int FSetHeaderODStyleSem;
	HWND FInternalHeaderHandle;
	void *FHeaderInstance;
	void *FDefHeaderProc;
	bool FRightClicked;
	bool FMouseInNonClient;
	Vcl::Controls::THintWindow* FHintWnd;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CNNotify(Winapi::Messages::TWMNotify &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMNotify(Winapi::Messages::TWMNotify &Msg);
	HIDESBASE MESSAGE void __fastcall WMMouseMove(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE void __fastcall HeaderWndProc(Winapi::Messages::TMessage &Msg);
	HWND __fastcall GetHeaderHandle();
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMSysColorChange(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall AMInitStage2(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMContextMenu(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMParentNotify(Winapi::Messages::TWMParentNotify &Msg);
	HIDESBASE MESSAGE void __fastcall WMRButtonUp(Winapi::Messages::TWMMouse &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FLoading;
	Vcl::Controls::TControlCanvas* FCanvas;
	bool FOverControl;
	int FOverHeaderSection;
	bool FHeaderSectionDown;
	bool FSettingFrameController;
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual bool __fastcall IsCustomDrawn(Vcl::Comctrls::TCustomDrawTarget Target, Vcl::Comctrls::TCustomDrawStage Stage);
	virtual bool __fastcall CustomDrawItem(Vcl::Comctrls::TListItem* Item, Vcl::Comctrls::TCustomDrawState State, Vcl::Comctrls::TCustomDrawStage Stage);
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	virtual void __fastcall ResizeLastColumn();
	virtual void __fastcall CreateWnd();
	void __fastcall DrawHeader(int Index, const System::Types::TRect &Rect);
	void __fastcall DrawHeaderSortGlyphs(int Index, const System::Types::TRect &Rect);
	void __fastcall DefaultDrawHeader(int Index, const System::Types::TRect &Rect);
	void __fastcall SetHeaderODStyle();
	int __fastcall GetMappedColumnIndex(int ColIndex);
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* Value);
	System::Types::TRect __fastcall CalcHintRect(int MaxWidth, const System::UnicodeString HintStr, Vcl::Controls::THintWindow* HintWnd);
	void __fastcall DoHint(int X, int Y);
	void __fastcall ReleaseHintWindow();
	DYNAMIC void __fastcall DoDrawHeader(int Index, const System::Types::TRect &Rect);
	DYNAMIC void __fastcall ColClick(Vcl::Comctrls::TListColumn* Column);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall CheckStateChange(Vcl::Comctrls::TListItem* Item);
	DYNAMIC bool __fastcall CanCheckStateChange(Vcl::Comctrls::TListItem* Item, bool ToBeChecked);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC void __fastcall DoPreItemContextMenu(const System::Types::TPoint &pt);
	DYNAMIC void __fastcall DoItemContextMenu(const System::Types::TPoint &p);
	DYNAMIC void __fastcall ItemContextMenu(Vcl::Comctrls::TListItem* Item, System::Types::TPoint &Pos, Vcl::Menus::TPopupMenu* &Menu);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState ShiftState);
	virtual void __fastcall SetFillLastColumn(bool Value);
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreParentColor();
	bool __fastcall StoreFlatButtonColor();
	bool __fastcall StoreFlatButtons();
	bool __fastcall StoreFrameColor();
	bool __fastcall StoreFrameHotColor();
	bool __fastcall StoreFrameHotTrack();
	bool __fastcall StoreFrameHotStyle();
	bool __fastcall StoreFrameSides();
	bool __fastcall StoreFrameStyle();
	bool __fastcall StoreFrameVisible();
	bool __fastcall StoreFramingPreference();
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides(Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(Rzcommon::TFramingPreference Value);
	virtual void __fastcall SetViewStyle(Vcl::Comctrls::TViewStyle AValue);
	void __fastcall SetHeaderSortColumn(int Value);
	void __fastcall SetHeaderSortDirection(TRzLVSortDirection Value);
	void __fastcall SetHeaderSortDisplayMode(TRzLVHeaderSortDisplayMode Value);
	void __fastcall SetHeaderDefaultDrawing(bool Value);
	__property bool AlphaSortAll = {read=FAlphaSortAll, write=FAlphaSortAll, default=0};
	__property Color = {stored=StoreColor, default=-16777211};
	__property bool Enabled = {read=GetEnabled, write=SetEnabled, default=1};
	__property bool EditOnRowClick = {read=FEditOnRowClick, write=FEditOnRowClick, default=0};
	__property bool FillLastColumn = {read=FFillLastColumn, write=SetFillLastColumn, default=1};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=StoreDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=StoreFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=StoreFrameHotColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=StoreFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=StoreFrameHotTrack, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=StoreFrameSides, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=StoreFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=StoreFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, stored=StoreFramingPreference, default=1};
	__property Vcl::Graphics::TCanvas* HeaderCanvas = {read=FHeaderCanvas};
	__property HWND HeaderHandle = {read=GetHeaderHandle, nodefault};
	__property bool HeaderDefaultDrawing = {read=FHeaderDefaultDrawing, write=SetHeaderDefaultDrawing, default=1};
	__property TRzLVHeaderSortDisplayMode HeaderSortDisplayMode = {read=FHeaderSortDisplayMode, write=SetHeaderSortDisplayMode, default=2};
	__property int HeaderSortColumn = {read=FHeaderSortColumn, write=SetHeaderSortColumn, default=-1};
	__property TRzLVSortDirection HeaderSortDirection = {read=FHeaderSortDirection, write=SetHeaderSortDirection, default=0};
	__property TRzLVCheckStateChanging OnCheckStateChanging = {read=FOnCheckStateChanging, write=FOnCheckStateChanging};
	__property Vcl::Comctrls::TLVNotifyEvent OnCheckStateChange = {read=FOnCheckStateChange, write=FOnCheckStateChange};
	__property TRzLVOnItemContextMenuEvent OnItemContextMenu = {read=FOnItemContextMenu, write=FOnItemContextMenu};
	__property TRzLVDrawHeaderEvent OnDrawHeader = {read=FOnDrawHeader, write=FOnDrawHeader};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
	
public:
	__fastcall virtual TRzCustomListView(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomListView();
	void __fastcall ResetHeaderHandle();
	virtual bool __fastcall UseThemes();
	bool __fastcall EditSelectedCaption();
	bool __fastcall EditItemCaption(Vcl::Comctrls::TListItem* Item);
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	void __fastcall SortByColumn(int Index, TRzLVSortDirection Direction = (TRzLVSortDirection)(0x0));
	void __fastcall SetTopIndex(int Index);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomListView(HWND ParentWindow) : Vcl::Comctrls::TCustomListView(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzListView : public TRzCustomListView
{
	typedef TRzCustomListView inherited;
	
public:
	__property HeaderCanvas;
	__property HeaderHandle;
	__property HeaderSortColumn = {default=-1};
	__property HeaderSortDirection = {default=0};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Action;
	__property Align = {default=0};
	__property AllocBy = {default=0};
	__property AlphaSortAll = {default=0};
	__property Anchors = {default=3};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property Checkboxes = {default=0};
	__property Color = {default=-16777211};
	__property Columns;
	__property ColumnClick = {default=1};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property EditOnRowClick = {default=0};
	__property Enabled = {default=1};
	__property FlatScrollBars = {default=0};
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
	__property Groups;
	__property GroupHeaderImages;
	__property GroupView = {default=0};
	__property HeaderDefaultDrawing = {default=1};
	__property HeaderSortDisplayMode = {default=2};
	__property HideSelection = {default=1};
	__property HotTrack = {default=0};
	__property HotTrackStyles = {default=0};
	__property HoverTime = {default=-1};
	__property IconOptions;
	__property Items;
	__property LargeImages;
	__property MultiSelect = {default=0};
	__property OwnerData = {default=0};
	__property OwnerDraw = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property RowSelect = {default=0};
	__property ShowHint;
	__property ShowColumnHeaders = {default=1};
	__property ShowWorkAreas = {default=0};
	__property SmallImages;
	__property SortType = {default=0};
	__property StateImages;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property ViewStyle = {default=0};
	__property FillLastColumn = {default=1};
	__property Visible = {default=1};
	__property OnAdvancedCustomDraw;
	__property OnAdvancedCustomDrawItem;
	__property OnAdvancedCustomDrawSubItem;
	__property OnChange;
	__property OnChanging;
	__property OnCheckStateChanging;
	__property OnCheckStateChange;
	__property OnClick;
	__property OnColumnClick;
	__property OnColumnDragged;
	__property OnColumnRightClick;
	__property OnContextPopup;
	__property OnCreateItemClass;
	__property OnItemChecked;
	__property OnGetSubItemImage;
	__property OnInfoTip;
	__property OnCompare;
	__property OnCustomDraw;
	__property OnCustomDrawItem;
	__property OnCustomDrawSubItem;
	__property OnData;
	__property OnDataFind;
	__property OnDataHint;
	__property OnDataStateChange;
	__property OnDblClick;
	__property OnDeletion;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawHeader;
	__property OnDrawItem;
	__property OnEdited;
	__property OnEditing;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnGetImageIndex;
	__property OnInsert;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
	__property OnItemContextMenu;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnSelectItem;
public:
	/* TRzCustomListView.Create */ inline __fastcall virtual TRzListView(System::Classes::TComponent* AOwner) : TRzCustomListView(AOwner) { }
	/* TRzCustomListView.Destroy */ inline __fastcall virtual ~TRzListView() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzListView(HWND ParentWindow) : TRzCustomListView(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE void __fastcall DrawArrow(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Rect, System::Uitypes::TColor ArrowColor, bool UpArrow);
extern DELPHI_PACKAGE int __stdcall RzCustomSortDescendingProc(NativeInt Item1, NativeInt Item2, NativeInt ColIndex);
extern DELPHI_PACKAGE int __stdcall RzCustomSortProc(NativeInt Item1, NativeInt Item2, NativeInt ColIndex);
}	/* namespace Rzlistvw */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLISTVW)
using namespace Rzlistvw;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlistvwHPP
