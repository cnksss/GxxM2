// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLstBox.pas' rev: 35.00 (Windows)

#ifndef RzlstboxHPP
#define RzlstboxHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Controls.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzIntLst.hpp>
#include <Vcl.ComCtrls.hpp>
#include <RzCommon.hpp>
#include <RzCmboBx.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlstbox
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomListBox;
class DELPHICLASS TRzListBox;
class DELPHICLASS EHeaderError;
class DELPHICLASS TRzTabStopList;
class DELPHICLASS TRzCustomTabbedListBox;
class DELPHICLASS TRzTabbedListBox;
class DELPHICLASS TRzPopupEdit;
class DELPHICLASS TRzEditListBox;
class DELPHICLASS TRzRankListBox;
class DELPHICLASS TRzFontListBox;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzCustomListBox : public Vcl::Stdctrls::TCustomListBox
{
	typedef Vcl::Stdctrls::TCustomListBox inherited;
	
private:
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
	bool FIncrementalSearch;
	bool FBeepOnInvalidKey;
	System::Word FHorzExtent;
	bool FHorzScrollBar;
	System::UnicodeString FSearchString;
	Vcl::Extctrls::TTimer* FTimer;
	int FKeyCount;
	bool FTabOnEnter;
	int FOwnerDrawIndent;
	bool FShowItemHints;
	Vcl::Controls::THintWindow* FHintWnd;
	int FClickedItem;
	System::TDateTime FClickedItemTime;
	bool FUseGradients;
	System::Uitypes::TColor FGroupColor;
	bool FGroupColorFromTheme;
	Vcl::Graphics::TFont* FGroupFont;
	bool FGroupFontChanged;
	System::UnicodeString FGroupPrefix;
	bool FShowGroups;
	Vcl::Stdctrls::TDrawItemEvent FOnDrawItem;
	System::Classes::TNotifyEvent FOnMatch;
	System::Classes::TNotifyEvent FOnDeleteItems;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	void __fastcall SearchTimerExpired(System::TObject* Sender);
	void __fastcall GroupFontChangeHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMKeyDown(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall WMChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMMouseMove(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CNDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	
protected:
	Vcl::Graphics::TCanvas* FCanvas;
	bool FOverControl;
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	DYNAMIC void __fastcall Resize();
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	System::Types::TRect __fastcall CalcHintRect(int MaxWidth, const System::UnicodeString HintStr, Vcl::Controls::THintWindow* HintWnd);
	void __fastcall DoHint(int X, int Y);
	void __fastcall ReleaseHintWindow();
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	bool __fastcall ShowFocus();
	virtual int __fastcall OwnerDrawItemIndent();
	virtual void __fastcall UpdateItemHeight();
	virtual System::UnicodeString __fastcall HorzExtentPrefix();
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	virtual void __fastcall GroupFontChanged();
	void __fastcall InvalidKeyPressed();
	DYNAMIC void __fastcall Click();
	virtual int __fastcall FindClosest(const System::UnicodeString S);
	virtual void __fastcall DrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	virtual void __fastcall DrawListItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	virtual void __fastcall DrawGroup(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	DYNAMIC void __fastcall Match();
	DYNAMIC void __fastcall DoDeleteItems();
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
	virtual void __fastcall SetHorzExtent(System::Word Value);
	virtual void __fastcall SetHorzScrollBar(bool Value);
	virtual void __fastcall SetOwnerDrawIndent(int Value);
	virtual System::Classes::TStrings* __fastcall GetItems();
	bool __fastcall StoreGroupPrefix();
	virtual void __fastcall SetGroupPrefix(const System::UnicodeString Value);
	virtual void __fastcall SetGroupColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetGroupColorFromTheme(bool Value);
	virtual void __fastcall SetGroupFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetUseGradients(bool Value);
	virtual bool __fastcall GetItemIsGroup(int Index);
	virtual void __fastcall SetShowGroups(bool Value);
	__property Color = {stored=StoreColor, default=-16777211};
	__property bool ShowItemHints = {read=FShowItemHints, write=FShowItemHints, default=1};
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
	__property System::Uitypes::TColor GroupColor = {read=FGroupColor, write=SetGroupColor, default=-16777197};
	__property bool GroupColorFromTheme = {read=FGroupColorFromTheme, write=SetGroupColorFromTheme, default=1};
	__property Vcl::Graphics::TFont* GroupFont = {read=FGroupFont, write=SetGroupFont, stored=FGroupFontChanged};
	__property System::UnicodeString GroupPrefix = {read=FGroupPrefix, write=SetGroupPrefix, stored=StoreGroupPrefix};
	__property bool HorzScrollBar = {read=FHorzScrollBar, write=SetHorzScrollBar, default=0};
	__property bool IncrementalSearch = {read=FIncrementalSearch, write=FIncrementalSearch, default=1};
	__property int OwnerDrawIndent = {read=FOwnerDrawIndent, write=SetOwnerDrawIndent, default=0};
	__property bool ShowGroups = {read=FShowGroups, write=SetShowGroups, default=0};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property bool UseGradients = {read=FUseGradients, write=SetUseGradients, default=1};
	__property Vcl::Stdctrls::TDrawItemEvent OnDrawItem = {read=FOnDrawItem, write=FOnDrawItem};
	__property System::Classes::TNotifyEvent OnMatch = {read=FOnMatch, write=FOnMatch};
	__property System::Classes::TNotifyEvent OnDeleteItems = {read=FOnDeleteItems, write=FOnDeleteItems};
	
public:
	__fastcall virtual TRzCustomListBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomListBox();
	virtual bool __fastcall UseThemes();
	virtual void __fastcall AdjustHorzExtent();
	virtual void __fastcall DefaultDrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	int __fastcall Add(const System::UnicodeString S);
	int __fastcall AddObject(const System::UnicodeString S, System::TObject* AObject);
	void __fastcall ClearSearchString();
	void __fastcall Delete(int Index);
	int __fastcall IndexOf(const System::UnicodeString S);
	HIDESBASE void __fastcall Insert(int Index, const System::UnicodeString S);
	void __fastcall InsertObject(int Index, const System::UnicodeString S, System::TObject* AObject);
	System::UnicodeString __fastcall SelectedItem();
	bool __fastcall FindItem(const System::UnicodeString S);
	virtual void __fastcall SelectAll();
	void __fastcall UnselectAll();
	void __fastcall DeleteSelectedItems();
	int __fastcall AddGroup(const System::UnicodeString S);
	System::UnicodeString __fastcall ItemCaption(int Index);
	virtual bool __fastcall ItemInsideGroup(int Index);
	virtual int __fastcall ItemGroupIndex(int Index);
	virtual int __fastcall GroupIndexOfItem(int Index);
	int __fastcall ItemsInGroup(int GroupIndex);
	int __fastcall ItemIndexOfGroup(int GroupIndex);
	int __fastcall AddItemToGroup(int GroupIndex, const System::UnicodeString S);
	int __fastcall InsertItemIntoGroup(int GroupIndex, int Index, const System::UnicodeString S);
	void __fastcall ItemToGroup(int Index);
	void __fastcall GroupToItem(int Index);
	__property bool ItemIsGroup[int Index] = {read=GetItemIsGroup};
	__property bool BeepOnInvalidKey = {read=FBeepOnInvalidKey, write=FBeepOnInvalidKey, default=1};
	__property System::UnicodeString SearchString = {read=FSearchString};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomListBox(HWND ParentWindow) : Vcl::Stdctrls::TCustomListBox(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Stdctrls::TCustomListBox::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzListBox : public TRzCustomListBox
{
	typedef TRzCustomListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property Color = {default=-16777211};
	__property Columns = {default=0};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property ExtendedSelect = {default=1};
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
	__property GroupColor = {default=-16777197};
	__property GroupColorFromTheme = {default=1};
	__property GroupFont;
	__property GroupPrefix = {default=0};
	__property HorzScrollBar = {default=0};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property IncrementalSearch = {default=1};
	__property IntegralHeight = {default=0};
	__property ItemHeight = {default=16};
	__property Items;
	__property MultiSelect = {default=0};
	__property OwnerDrawIndent = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowGroups = {default=0};
	__property ShowHint;
	__property ShowItemHints = {default=1};
	__property Sorted = {default=0};
	__property Style = {default=0};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property TabWidth = {default=0};
	__property Touch;
	__property UseGradients = {default=1};
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnData;
	__property OnDataFind;
	__property OnDataObject;
	__property OnDblClick;
	__property OnDeleteItems;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawItem;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMatch;
	__property OnMeasureItem;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomListBox.Create */ inline __fastcall virtual TRzListBox(System::Classes::TComponent* AOwner) : TRzCustomListBox(AOwner) { }
	/* TRzCustomListBox.Destroy */ inline __fastcall virtual ~TRzListBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzListBox(HWND ParentWindow) : TRzCustomListBox(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION EHeaderError : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall EHeaderError(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EHeaderError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EHeaderError(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EHeaderError(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EHeaderError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EHeaderError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EHeaderError(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EHeaderError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EHeaderError(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EHeaderError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EHeaderError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EHeaderError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EHeaderError() { }
	
};

#pragma pack(pop)

typedef System::StaticArray<int, 1000> TRzTabArray;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabStopList : public Rzintlst::TRzIntegerList
{
	typedef Rzintlst::TRzIntegerList inherited;
	
private:
	TRzCustomTabbedListBox* FListBox;
	
protected:
	virtual void __fastcall SetItem(int Index, int Value);
	
public:
	__fastcall TRzTabStopList();
	virtual void __fastcall Delete(int Index);
	virtual void __fastcall Insert(int Index, int Value);
	virtual int __fastcall Add(int Value);
public:
	/* TRzIntegerList.Destroy */ inline __fastcall virtual ~TRzTabStopList() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzTabStopsMode : unsigned char { tsmManual, tsmAutomatic };

class PASCALIMPLEMENTATION TRzCustomTabbedListBox : public TRzCustomListBox
{
	typedef TRzCustomListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	TRzTabStopList* FTabStops;
	TRzTabStopsMode FTabStopsMode;
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	int FDialogUnits;
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall DrawGroup(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	virtual int __fastcall InitialTabStopOffset();
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	virtual void __fastcall GroupFontChanged();
	bool __fastcall StoreTabStops();
	virtual void __fastcall SetTabStops(TRzTabStopList* Value);
	virtual void __fastcall SetTabStopsMode(TRzTabStopsMode Value);
	virtual void __fastcall GetTabArray(int &TabCount, TRzTabArray &TabArray);
	virtual System::UnicodeString __fastcall GetCellText(int ACol, int ARow);
	virtual void __fastcall SetCellText(int ACol, int ARow, const System::UnicodeString Value);
	
public:
	__fastcall virtual TRzCustomTabbedListBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomTabbedListBox();
	void __fastcall UpdateTabStops();
	virtual void __fastcall AdjustTabStops();
	virtual void __fastcall AdjustHorzExtent();
	virtual void __fastcall DefaultDrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	virtual void __fastcall UpdateFromHeader(Vcl::Controls::TControl* Header);
	__property System::UnicodeString Cells[int ACol][int ARow] = {read=GetCellText, write=SetCellText};
	__property int DialogUnits = {read=FDialogUnits, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzTabStopList* TabStops = {read=FTabStops, write=SetTabStops, stored=StoreTabStops};
	__property TRzTabStopsMode TabStopsMode = {read=FTabStopsMode, write=SetTabStopsMode, default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomTabbedListBox(HWND ParentWindow) : TRzCustomListBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzTabbedListBox : public TRzCustomTabbedListBox
{
	typedef TRzCustomTabbedListBox inherited;
	
__published:
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property Color = {default=-16777211};
	__property Columns = {default=0};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property ShowItemHints = {default=0};
	__property ExtendedSelect = {default=1};
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
	__property GroupColor = {default=-16777197};
	__property GroupColorFromTheme = {default=1};
	__property GroupFont;
	__property GroupPrefix = {default=0};
	__property HorzScrollBar = {default=0};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property IncrementalSearch = {default=1};
	__property IntegralHeight = {default=0};
	__property ItemHeight = {default=16};
	__property Items;
	__property MultiSelect = {default=0};
	__property OwnerDrawIndent = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowGroups = {default=0};
	__property ShowHint;
	__property Sorted = {default=0};
	__property Style = {default=0};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property UseGradients = {default=1};
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnData;
	__property OnDataFind;
	__property OnDataObject;
	__property OnDblClick;
	__property OnDeleteItems;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawItem;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMatch;
	__property OnMeasureItem;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomTabbedListBox.Create */ inline __fastcall virtual TRzTabbedListBox(System::Classes::TComponent* AOwner) : TRzCustomTabbedListBox(AOwner) { }
	/* TRzCustomTabbedListBox.Destroy */ inline __fastcall virtual ~TRzTabbedListBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabbedListBox(HWND ParentWindow) : TRzCustomTabbedListBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzPopupEdit : public Vcl::Stdctrls::TCustomEdit
{
	typedef Vcl::Stdctrls::TCustomEdit inherited;
	
private:
	TRzEditListBox* FList;
	MESSAGE void __fastcall CMCancelMode(Vcl::Controls::TCMCancelMode &Msg);
	HIDESBASE MESSAGE void __fastcall CMShowingChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CNKeyDown(Winapi::Messages::TWMKey &Msg);
	
protected:
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	
public:
	__fastcall virtual TRzPopupEdit(System::Classes::TComponent* AOwner);
	__property Font;
	__property Color = {default=-16777211};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPopupEdit(HWND ParentWindow) : Vcl::Stdctrls::TCustomEdit(ParentWindow) { }
	/* TWinControl.Destroy */ inline __fastcall virtual ~TRzPopupEdit() { }
	
};


typedef void __fastcall (__closure *TRzShowingEditorEvent)(System::TObject* Sender, int Index, bool &AllowShow);

typedef void __fastcall (__closure *TRzSizeEditRectEvent)(System::TObject* Sender, int Index, System::Types::TRect &EditRect);

typedef void __fastcall (__closure *TRzItemChangedEvent)(System::TObject* Sender, int Index);

class PASCALIMPLEMENTATION TRzEditListBox : public TRzListBox
{
	typedef TRzListBox inherited;
	
private:
	bool FJustGotFocus;
	bool FAllowEdit;
	TRzPopupEdit* FPopupEdit;
	bool FPopupVisible;
	System::Uitypes::TColor FEditColor;
	System::Uitypes::TColor FEditFontColor;
	int FCurrIdx;
	int FEditorIdx;
	bool FDoubleClicked;
	Vcl::Extctrls::TTimer* FTimer;
	bool FShowEditorOnNextClick;
	bool FAllowDeleteByKbd;
	TRzSizeEditRectEvent FOnSizeEditRect;
	TRzShowingEditorEvent FOnShowingEditor;
	System::Classes::TNotifyEvent FOnHidingEditor;
	TRzItemChangedEvent FOnItemChanged;
	virtual void __fastcall TimerExpired(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall WMLButtonDblClick(Winapi::Messages::TWMMouse &Msg);
	
protected:
	DYNAMIC void __fastcall SizeEditRect(int Index, System::Types::TRect &EditRect);
	DYNAMIC bool __fastcall DoShowingEditor(int Index);
	DYNAMIC void __fastcall DoHidingEditor();
	DYNAMIC void __fastcall ItemChanged(int Index);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	
public:
	__fastcall virtual TRzEditListBox(System::Classes::TComponent* AOwner);
	virtual void __fastcall HideEditor(bool SaveChanges);
	virtual void __fastcall ShowEditor();
	__property TRzPopupEdit* PopupEdit = {read=FPopupEdit};
	
__published:
	__property bool AllowEdit = {read=FAllowEdit, write=FAllowEdit, default=1};
	__property bool AllowDeleteByKbd = {read=FAllowDeleteByKbd, write=FAllowDeleteByKbd, default=0};
	__property System::Uitypes::TColor EditColor = {read=FEditColor, write=FEditColor, default=-16777211};
	__property System::Uitypes::TColor EditFontColor = {read=FEditFontColor, write=FEditFontColor, default=-16777208};
	__property bool PopupVisible = {read=FPopupVisible, nodefault};
	__property TRzSizeEditRectEvent OnSizeEditRect = {read=FOnSizeEditRect, write=FOnSizeEditRect};
	__property TRzShowingEditorEvent OnShowingEditor = {read=FOnShowingEditor, write=FOnShowingEditor};
	__property System::Classes::TNotifyEvent OnHidingEditor = {read=FOnHidingEditor, write=FOnHidingEditor};
	__property TRzItemChangedEvent OnItemChanged = {read=FOnItemChanged, write=FOnItemChanged};
public:
	/* TRzCustomListBox.Destroy */ inline __fastcall virtual ~TRzEditListBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzEditListBox(HWND ParentWindow) : TRzListBox(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzModifierKey : unsigned char { mkShift, mkNone };

typedef void __fastcall (__closure *TRzMoveItemEvent)(System::TObject* Sender, int OldIndex, int NewIndex);

class PASCALIMPLEMENTATION TRzRankListBox : public TRzListBox
{
	typedef TRzListBox inherited;
	
private:
	TRzModifierKey FModifierKey;
	bool FMoveOnDrag;
	bool FMoving;
	int FOldIndex;
	HICON FOldCursor;
	int FColumns;
	System::Uitypes::TCursor FDragCursor;
	System::Uitypes::TDragMode FDragMode;
	bool FExtendedSelect;
	bool FMultiSelect;
	bool FSorted;
	TRzMoveItemEvent FOnMoveItem;
	void __fastcall SetMoveOnDrag(bool Value);
	
protected:
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall MoveItem(int OldIndex, int NewIndex);
	
public:
	__fastcall virtual TRzRankListBox(System::Classes::TComponent* AOwner);
	
__published:
	__property int Columns = {read=FColumns, default=0};
	__property System::Uitypes::TCursor DragCursor = {read=FDragCursor, default=-12};
	__property System::Uitypes::TDragMode DragMode = {read=FDragMode, default=0};
	__property bool ExtendedSelect = {read=FExtendedSelect, default=0};
	__property bool MultiSelect = {read=FMultiSelect, default=0};
	__property bool Sorted = {read=FSorted, default=0};
	__property TRzModifierKey ModifierKey = {read=FModifierKey, write=FModifierKey, default=0};
	__property bool MoveOnDrag = {read=FMoveOnDrag, write=SetMoveOnDrag, default=1};
	__property TRzMoveItemEvent OnMoveItem = {read=FOnMoveItem, write=FOnMoveItem};
public:
	/* TRzCustomListBox.Destroy */ inline __fastcall virtual ~TRzRankListBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzRankListBox(HWND ParentWindow) : TRzListBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzFontListBox : public TRzCustomListBox
{
	typedef TRzCustomListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::UnicodeString FSaveFontName;
	Vcl::Graphics::TFont* FFont;
	Rzcmbobx::TRzFontDevice FFontDevice;
	Rzcmbobx::TRzFontType FFontType;
	int FFontSize;
	System::Uitypes::TFontStyles FFontStyle;
	bool FShowSymbolFonts;
	Rzcmbobx::TRzShowStyle FShowStyle;
	Vcl::Graphics::TBitmap* FTrueTypeBmp;
	Vcl::Graphics::TBitmap* FFixedPitchBmp;
	Vcl::Graphics::TBitmap* FTrueTypeFixedBmp;
	Vcl::Graphics::TBitmap* FPrinterBmp;
	Vcl::Graphics::TBitmap* FDeviceBmp;
	bool FPreviewVisible;
	Rzcmbobx::TRzPreviewFontPanel* FPreviewPanel;
	int FPreviewFontSize;
	int FPreviewWidth;
	int FPreviewHeight;
	Vcl::Stdctrls::TCustomEdit* FPreviewEdit;
	System::UnicodeString FPreviewText;
	int FMRUCount;
	bool FMaintainMRUFonts;
	HIDESBASE MESSAGE void __fastcall CNDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMCancelMode(Vcl::Controls::TCMCancelMode &Msg);
	MESSAGE void __fastcall CMHidePreviewPanel(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DestroyWnd();
	virtual void __fastcall LoadFonts();
	virtual void __fastcall LoadBitmaps();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	void __fastcall UpdatePreviewText();
	virtual void __fastcall HidePreviewPanel();
	virtual void __fastcall ShowPreviewPanel();
	virtual void __fastcall DrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetFontDevice(Rzcmbobx::TRzFontDevice Value);
	virtual void __fastcall SetFontType(Rzcmbobx::TRzFontType Value);
	virtual Vcl::Graphics::TFont* __fastcall GetSelectedFont();
	virtual void __fastcall SetSelectedFont(Vcl::Graphics::TFont* Value);
	virtual System::UnicodeString __fastcall GetFontName();
	virtual void __fastcall SetFontName(const System::UnicodeString Value);
	virtual void __fastcall SetPreviewEdit(Vcl::Stdctrls::TCustomEdit* Value);
	virtual void __fastcall SetShowSymbolFonts(bool Value);
	virtual void __fastcall SetShowStyle(Rzcmbobx::TRzShowStyle Value);
	
public:
	__fastcall virtual TRzFontListBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFontListBox();
	void __fastcall AddFontToMRUList();
	__property Vcl::Graphics::TFont* SelectedFont = {read=GetSelectedFont, write=SetSelectedFont};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Rzcmbobx::TRzFontDevice FontDevice = {read=FFontDevice, write=SetFontDevice, default=0};
	__property System::UnicodeString FontName = {read=GetFontName, write=SetFontName};
	__property int FontSize = {read=FFontSize, write=FFontSize, default=8};
	__property System::Uitypes::TFontStyles FontStyle = {read=FFontStyle, write=FFontStyle, default=0};
	__property Rzcmbobx::TRzFontType FontType = {read=FFontType, write=SetFontType, default=0};
	__property bool MaintainMRUFonts = {read=FMaintainMRUFonts, write=FMaintainMRUFonts, default=0};
	__property Vcl::Stdctrls::TCustomEdit* PreviewEdit = {read=FPreviewEdit, write=FPreviewEdit};
	__property int PreviewFontSize = {read=FPreviewFontSize, write=FPreviewFontSize, default=36};
	__property int PreviewHeight = {read=FPreviewHeight, write=FPreviewHeight, default=65};
	__property System::UnicodeString PreviewText = {read=FPreviewText, write=FPreviewText};
	__property int PreviewWidth = {read=FPreviewWidth, write=FPreviewWidth, default=260};
	__property bool ShowSymbolFonts = {read=FShowSymbolFonts, write=SetShowSymbolFonts, default=1};
	__property Rzcmbobx::TRzShowStyle ShowStyle = {read=FShowStyle, write=SetShowStyle, default=0};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
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
	__property ItemHeight = {default=16};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Sorted = {default=1};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzFontListBox(HWND ParentWindow) : TRzCustomListBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word MaxTabs = System::Word(0x3e8);
#define strDefaultGroupPrefix L"//"
}	/* namespace Rzlstbox */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLSTBOX)
using namespace Rzlstbox;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlstboxHPP
