// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzCmboBx.pas' rev: 35.00 (Windows)

#ifndef RzcmbobxHPP
#define RzcmbobxHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzcmbobx
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomComboBox;
class DELPHICLASS TRzComboBox;
class DELPHICLASS TRzColorNames;
class DELPHICLASS TRzColorComboBox;
class DELPHICLASS TRzPreviewFontPanel;
class DELPHICLASS TRzFontComboBox;
class DELPHICLASS TRzMRUComboBox;
class DELPHICLASS TRzImageComboBoxItem;
class DELPHICLASS TRzCustomImageComboBox;
class DELPHICLASS TRzImageComboBox;
struct TRzColorRec;
//-- type declarations -------------------------------------------------------
typedef void __fastcall (__closure *TRzDeleteComboItemEvent)(System::TObject* Sender, void * Item);

class PASCALIMPLEMENTATION TRzCustomComboBox : public Vcl::Stdctrls::TCustomComboBox
{
	typedef Vcl::Stdctrls::TCustomComboBox inherited;
	
private:
	bool FAutoComplete;
	bool FAllowEdit;
	bool FBeepOnInvalidKey;
	bool FFlatButtons;
	System::Uitypes::TColor FFlatButtonColor;
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
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
	bool FKeepSearchCase;
	System::UnicodeString FSearchString;
	int FDropDownWidth;
	int FKeyCount;
	Vcl::Extctrls::TTimer* FTimer;
	bool FTabOnEnter;
	bool FTyping;
	bool FEnterPressed;
	bool FReadOnly;
	bool FSysKeyDown;
	TRzDeleteComboItemEvent FOnDeleteItem;
	System::Classes::TNotifyEvent FOnMatch;
	System::Classes::TNotifyEvent FOnNotInList;
	System::Classes::TNotifyEvent FOnSelEndCancel;
	System::Classes::TNotifyEvent FOnSelEndOk;
	Vcl::Controls::TControl* FMouseControl;
	bool FMouseInClient;
	bool FOverEditArea;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	void __fastcall SetItemHeight2(int Value);
	HIDESBASE MESSAGE void __fastcall WMKeyDown(Winapi::Messages::TWMKey &Msg);
	MESSAGE void __fastcall WMCut(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMPaste(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	HIDESBASE MESSAGE void __fastcall CNCommand(Winapi::Messages::TWMCommand &Msg);
	HIDESBASE MESSAGE void __fastcall CNDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMDeleteItem(Winapi::Messages::TWMDeleteItem &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TWMMouse &Msg);
	MESSAGE void __fastcall WMLButtonDblClick(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Graphics::TCanvas* FCanvas;
	bool FInControl;
	bool FOverControl;
	bool FShowFocus;
	bool FIsFocused;
	int FActualDropDownWidth;
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall InvalidKeyPressed();
	void __fastcall SearchTimerExpired(System::TObject* Sender);
	virtual void __fastcall UpdateIndex(const System::UnicodeString FindStr, const Winapi::Messages::TWMKey &Msg);
	virtual bool __fastcall FindListItem(const System::UnicodeString FindStr, const Winapi::Messages::TMessage &Msg);
	virtual int __fastcall FindClosest(const System::UnicodeString S);
	virtual void __fastcall ComboWndProc(Winapi::Messages::TMessage &Msg, HWND ComboWnd, void * ComboProc);
	virtual void __fastcall EditWndProc(Winapi::Messages::TMessage &Msg);
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	void __fastcall UpdateSearchStr();
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall CloseUp();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall Match();
	DYNAMIC void __fastcall NotInList();
	virtual void __fastcall DeleteItem(void * Item);
	DYNAMIC void __fastcall SelEndCancel();
	DYNAMIC void __fastcall SelEndOk();
	virtual void __fastcall SetFlatButtons(bool Value);
	virtual void __fastcall SetFlatButtonColor(System::Uitypes::TColor Value);
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreReadOnlyColor();
	bool __fastcall StoreReadOnlyColorOnFocus();
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
	virtual void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	__property bool AllowEdit = {read=FAllowEdit, write=FAllowEdit, default=1};
	__property bool AutoComplete = {read=FAutoComplete, write=FAutoComplete, default=1};
	__property Color = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=SetFlatButtonColor, stored=StoreFlatButtonColor, default=-16777201};
	__property bool FlatButtons = {read=FFlatButtons, write=SetFlatButtons, stored=StoreFlatButtons, default=0};
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
	__property bool KeepSearchCase = {read=FKeepSearchCase, write=FKeepSearchCase, default=0};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property int DropDownWidth = {read=FDropDownWidth, write=FDropDownWidth, default=0};
	__property bool ReadOnly = {read=FReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, stored=StoreReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=FReadOnlyColorOnFocus, stored=StoreReadOnlyColorOnFocus, default=0};
	__property TRzDeleteComboItemEvent OnDeleteItem = {read=FOnDeleteItem, write=FOnDeleteItem};
	__property System::Classes::TNotifyEvent OnMatch = {read=FOnMatch, write=FOnMatch};
	__property System::Classes::TNotifyEvent OnNotInList = {read=FOnNotInList, write=FOnNotInList};
	__property System::Classes::TNotifyEvent OnSelEndCancel = {read=FOnSelEndCancel, write=FOnSelEndCancel};
	__property System::Classes::TNotifyEvent OnSelEndOk = {read=FOnSelEndOk, write=FOnSelEndOk};
	__property ItemHeight = {write=SetItemHeight2};
	
public:
	__fastcall virtual TRzCustomComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomComboBox();
	virtual bool __fastcall UseThemes();
	DYNAMIC bool __fastcall Focused();
	virtual void __fastcall ForceText(const System::UnicodeString Value);
	int __fastcall Add(const System::UnicodeString S);
	int __fastcall AddObject(const System::UnicodeString S, System::TObject* AObject);
	void __fastcall ClearSearchString();
	void __fastcall Delete(int Index);
	void __fastcall ClearItems();
	virtual int __fastcall IndexOf(const System::UnicodeString S);
	HIDESBASE void __fastcall Insert(int Index, const System::UnicodeString S);
	void __fastcall InsertObject(int Index, const System::UnicodeString S, System::TObject* AObject);
	int __fastcall Count();
	bool __fastcall FindItem(const System::UnicodeString S);
	bool __fastcall FindClosestItem(const System::UnicodeString S);
	__property bool BeepOnInvalidKey = {read=FBeepOnInvalidKey, write=FBeepOnInvalidKey, default=1};
	__property System::UnicodeString SearchString = {read=FSearchString};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomComboBox(HWND ParentWindow) : Vcl::Stdctrls::TCustomComboBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzComboBox : public TRzCustomComboBox
{
	typedef TRzCustomComboBox inherited;
	
private:
	System::UnicodeString FValue;
	System::Classes::TStrings* FValues;
	bool FInSetValue;
	bool FForceSetValue;
	void __fastcall ValuesChangedHandler(System::TObject* Sender);
	
protected:
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall Click();
	virtual System::UnicodeString __fastcall GetItemValue(int Index);
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall SetValue(const System::UnicodeString Value);
	virtual void __fastcall SetValues(System::Classes::TStrings* Value);
	
public:
	__fastcall virtual TRzComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzComboBox();
	void __fastcall ClearItemsValues();
	void __fastcall AddItemValue(const System::UnicodeString Item, const System::UnicodeString Value);
	void __fastcall InsertItemValue(int Index, const System::UnicodeString Item, const System::UnicodeString Value);
	void __fastcall DeleteItemValue(int Index);
	__property System::UnicodeString Value = {read=GetValue, write=SetValue};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property AllowEdit = {default=1};
	__property Anchors = {default=3};
	__property AutoComplete = {default=1};
	__property AutoCloseUp = {default=0};
	__property AutoDropDown = {default=0};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property Style = {default=0};
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property DropDownCount = {default=8};
	__property DropDownWidth = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property FlatButtonColor = {default=-16777201};
	__property FlatButtons = {default=0};
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
	__property KeepSearchCase = {default=0};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property ItemHeight;
	__property MaxLength = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property Sorted = {default=0};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Text = {default=0};
	__property TextHint = {default=0};
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
	__property OnMatch;
	__property OnMeasureItem;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
	__property OnNotInList;
	__property OnSelect;
	__property OnSelEndCancel;
	__property OnSelEndOk;
	__property OnStartDock;
	__property OnStartDrag;
	__property Items;
	__property ItemIndex = {default=-1};
	__property System::Classes::TStrings* Values = {read=FValues, write=SetValues};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzComboBox(HWND ParentWindow) : TRzCustomComboBox(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzColorNames : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
	
private:
	typedef System::StaticArray<System::UnicodeString, 16> _TRzColorNames__1;
	
	typedef System::StaticArray<System::UnicodeString, 25> _TRzColorNames__2;
	
	
private:
	TRzColorComboBox* FComboBox;
	System::UnicodeString FDefaultColor;
	System::UnicodeString FCustomColor;
	_TRzColorNames__1 FStdColors;
	_TRzColorNames__2 FSysColors;
	
protected:
	void __fastcall SetDefaultColor(const System::UnicodeString Value);
	System::UnicodeString __fastcall GetStdColor(int Index);
	void __fastcall SetStdColor(int Index, const System::UnicodeString Value);
	System::UnicodeString __fastcall GetSysColor(int Index);
	void __fastcall SetSysColor(int Index, const System::UnicodeString Value);
	void __fastcall SetCustomColor(const System::UnicodeString Value);
	
public:
	bool ShowSysColors;
	bool ShowDefaultColor;
	bool ShowCustomColor;
	__fastcall TRzColorNames();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property System::UnicodeString StdColors[int Index] = {read=GetStdColor, write=SetStdColor};
	__property System::UnicodeString SysColors[int Index] = {read=GetSysColor, write=SetSysColor};
	
__published:
	__property System::UnicodeString Default = {read=FDefaultColor, write=SetDefaultColor};
	__property System::UnicodeString Black = {read=GetStdColor, write=SetStdColor, index=0};
	__property System::UnicodeString Maroon = {read=GetStdColor, write=SetStdColor, index=1};
	__property System::UnicodeString Green = {read=GetStdColor, write=SetStdColor, index=2};
	__property System::UnicodeString Olive = {read=GetStdColor, write=SetStdColor, index=3};
	__property System::UnicodeString Navy = {read=GetStdColor, write=SetStdColor, index=4};
	__property System::UnicodeString Purple = {read=GetStdColor, write=SetStdColor, index=5};
	__property System::UnicodeString Teal = {read=GetStdColor, write=SetStdColor, index=6};
	__property System::UnicodeString Gray = {read=GetStdColor, write=SetStdColor, index=7};
	__property System::UnicodeString Silver = {read=GetStdColor, write=SetStdColor, index=8};
	__property System::UnicodeString Red = {read=GetStdColor, write=SetStdColor, index=9};
	__property System::UnicodeString Lime = {read=GetStdColor, write=SetStdColor, index=10};
	__property System::UnicodeString Yellow = {read=GetStdColor, write=SetStdColor, index=11};
	__property System::UnicodeString Blue = {read=GetStdColor, write=SetStdColor, index=12};
	__property System::UnicodeString Fuchsia = {read=GetStdColor, write=SetStdColor, index=13};
	__property System::UnicodeString Aqua = {read=GetStdColor, write=SetStdColor, index=14};
	__property System::UnicodeString White = {read=GetStdColor, write=SetStdColor, index=15};
	__property System::UnicodeString ScrollBar = {read=GetSysColor, write=SetSysColor, index=0};
	__property System::UnicodeString Background = {read=GetSysColor, write=SetSysColor, index=1};
	__property System::UnicodeString ActiveCaption = {read=GetSysColor, write=SetSysColor, index=2};
	__property System::UnicodeString InactiveCaption = {read=GetSysColor, write=SetSysColor, index=3};
	__property System::UnicodeString Menu = {read=GetSysColor, write=SetSysColor, index=4};
	__property System::UnicodeString Window = {read=GetSysColor, write=SetSysColor, index=5};
	__property System::UnicodeString WindowFrame = {read=GetSysColor, write=SetSysColor, index=6};
	__property System::UnicodeString MenuText = {read=GetSysColor, write=SetSysColor, index=7};
	__property System::UnicodeString WindowText = {read=GetSysColor, write=SetSysColor, index=8};
	__property System::UnicodeString CaptionText = {read=GetSysColor, write=SetSysColor, index=9};
	__property System::UnicodeString ActiveBorder = {read=GetSysColor, write=SetSysColor, index=10};
	__property System::UnicodeString InactiveBorder = {read=GetSysColor, write=SetSysColor, index=11};
	__property System::UnicodeString AppWorkSpace = {read=GetSysColor, write=SetSysColor, index=12};
	__property System::UnicodeString Highlight = {read=GetSysColor, write=SetSysColor, index=13};
	__property System::UnicodeString HighlightText = {read=GetSysColor, write=SetSysColor, index=14};
	__property System::UnicodeString BtnFace = {read=GetSysColor, write=SetSysColor, index=15};
	__property System::UnicodeString BtnShadow = {read=GetSysColor, write=SetSysColor, index=16};
	__property System::UnicodeString GrayText = {read=GetSysColor, write=SetSysColor, index=17};
	__property System::UnicodeString BtnText = {read=GetSysColor, write=SetSysColor, index=18};
	__property System::UnicodeString InactiveCaptionText = {read=GetSysColor, write=SetSysColor, index=19};
	__property System::UnicodeString BtnHighlight = {read=GetSysColor, write=SetSysColor, index=20};
	__property System::UnicodeString DkShadow3D = {read=GetSysColor, write=SetSysColor, index=21};
	__property System::UnicodeString Light3D = {read=GetSysColor, write=SetSysColor, index=22};
	__property System::UnicodeString InfoText = {read=GetSysColor, write=SetSysColor, index=23};
	__property System::UnicodeString InfoBk = {read=GetSysColor, write=SetSysColor, index=24};
	__property System::UnicodeString Custom = {read=FCustomColor, write=SetCustomColor};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzColorNames() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzColorComboBox : public TRzCustomComboBox
{
	typedef TRzCustomComboBox inherited;
	
private:
	bool FCancelPick;
	System::Uitypes::TColor FDefaultColor;
	System::Uitypes::TColor FCustomColor;
	TRzColorNames* FColorNames;
	TRzColorNames* FSaveColorNames;
	bool FShowSysColors;
	bool FShowColorNames;
	bool FShowDefaultColor;
	bool FShowCustomColor;
	Vcl::Dialogs::TColorDialogOptions FColorDlgOptions;
	System::Classes::TStrings* FCustomColors;
	bool FStoreColorNames;
	int FSaveItemIndex;
	Rzcommon::TRzRegIniFile* FRegIniFile;
	HIDESBASE MESSAGE void __fastcall CNDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CNCommand(Winapi::Messages::TWMCommand &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DestroyWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	System::UnicodeString __fastcall GetCustomColorName(int Index);
	virtual void __fastcall FixupCustomColors();
	virtual void __fastcall InitColorNames();
	virtual System::Uitypes::TColor __fastcall GetColorFromItem(int Index);
	virtual void __fastcall DrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	DYNAMIC void __fastcall CloseUp();
	virtual void __fastcall SetDefaultColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetColorNames(TRzColorNames* Value);
	virtual void __fastcall SetCustomColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCustomColors(System::Classes::TStrings* Value);
	virtual void __fastcall SetShowCustomColor(bool Value);
	virtual void __fastcall SetShowDefaultColor(bool Value);
	virtual void __fastcall SetShowSysColors(bool Value);
	virtual void __fastcall SetShowColorNames(bool Value);
	virtual System::Uitypes::TColor __fastcall GetSelectedColor();
	virtual void __fastcall SetSelectedColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetRegIniFile(Rzcommon::TRzRegIniFile* Value);
	
public:
	__fastcall virtual TRzColorComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzColorComboBox();
	void __fastcall LoadCustomColors(const System::UnicodeString Section);
	void __fastcall SaveCustomColors(const System::UnicodeString Section);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzColorNames* ColorNames = {read=FColorNames, write=SetColorNames, stored=FStoreColorNames};
	__property System::Uitypes::TColor CustomColor = {read=FCustomColor, write=SetCustomColor, default=0};
	__property System::Classes::TStrings* CustomColors = {read=FCustomColors, write=SetCustomColors};
	__property Vcl::Dialogs::TColorDialogOptions ColorDlgOptions = {read=FColorDlgOptions, write=FColorDlgOptions, default=1};
	__property System::Uitypes::TColor DefaultColor = {read=FDefaultColor, write=SetDefaultColor, default=0};
	__property Rzcommon::TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
	__property bool ShowColorNames = {read=FShowColorNames, write=SetShowColorNames, default=1};
	__property bool ShowCustomColor = {read=FShowCustomColor, write=SetShowCustomColor, default=1};
	__property bool ShowDefaultColor = {read=FShowDefaultColor, write=SetShowDefaultColor, default=1};
	__property bool ShowSysColors = {read=FShowSysColors, write=SetShowSysColors, default=1};
	__property System::Uitypes::TColor SelectedColor = {read=GetSelectedColor, write=SetSelectedColor, default=0};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoCloseUp = {default=0};
	__property AutoDropDown = {default=0};
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
	__property DropDownCount = {default=8};
	__property DropDownWidth = {default=0};
	__property Enabled = {default=1};
	__property FlatButtonColor = {default=-16777201};
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
	__property ItemHeight;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnChange;
	__property OnClick;
	__property OnCloseUp;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDropDown;
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
	__property OnSelect;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzColorComboBox(HWND ParentWindow) : TRzCustomComboBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzPreviewFontPanel : public Vcl::Extctrls::TCustomPanel
{
	typedef Vcl::Extctrls::TCustomPanel inherited;
	
private:
	Vcl::Controls::TWinControl* FControl;
	MESSAGE void __fastcall CMCancelMode(Vcl::Controls::TCMCancelMode &Msg);
	HIDESBASE MESSAGE void __fastcall CMShowingChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall Paint();
	
public:
	__fastcall virtual TRzPreviewFontPanel(System::Classes::TComponent* AOwner);
	__property Vcl::Controls::TWinControl* Control = {write=FControl};
	__property Alignment = {default=2};
	__property Canvas;
	__property Caption = {default=0};
	__property Font;
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzPreviewFontPanel() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPreviewFontPanel(HWND ParentWindow) : Vcl::Extctrls::TCustomPanel(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzFontDevice : unsigned char { fdScreen, fdPrinter };

enum DECLSPEC_DENUM TRzFontType : unsigned char { ftAll, ftTrueType, ftFixedPitch, ftPrinter };

enum DECLSPEC_DENUM TRzShowStyle : unsigned char { ssFontName, ssFontSample, ssFontNameAndSample, ssFontPreview };

class PASCALIMPLEMENTATION TRzFontComboBox : public TRzCustomComboBox
{
	typedef TRzCustomComboBox inherited;
	
private:
	System::UnicodeString FSaveFontName;
	Vcl::Graphics::TFont* FFont;
	TRzFontDevice FFontDevice;
	TRzFontType FFontType;
	int FFontSize;
	System::Uitypes::TFontStyles FFontStyle;
	bool FShowSymbolFonts;
	TRzShowStyle FShowStyle;
	Vcl::Graphics::TBitmap* FTrueTypeBmp;
	Vcl::Graphics::TBitmap* FFixedPitchBmp;
	Vcl::Graphics::TBitmap* FTrueTypeFixedBmp;
	Vcl::Graphics::TBitmap* FPrinterBmp;
	Vcl::Graphics::TBitmap* FDeviceBmp;
	bool FPreviewVisible;
	TRzPreviewFontPanel* FPreviewPanel;
	int FPreviewFontSize;
	int FPreviewWidth;
	int FPreviewHeight;
	Vcl::Stdctrls::TCustomEdit* FPreviewEdit;
	System::UnicodeString FPreviewText;
	int FMRUCount;
	bool FMaintainMRUFonts;
	HIDESBASE MESSAGE void __fastcall CNDrawItem(Winapi::Messages::TWMDrawItem &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMCancelMode(Vcl::Controls::TCMCancelMode &Msg);
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
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall CloseUp();
	virtual void __fastcall DrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	virtual void __fastcall SetFontDevice(TRzFontDevice Value);
	virtual void __fastcall SetFontType(TRzFontType Value);
	virtual Vcl::Graphics::TFont* __fastcall GetSelectedFont();
	virtual void __fastcall SetSelectedFont(Vcl::Graphics::TFont* Value);
	virtual System::UnicodeString __fastcall GetFontName();
	virtual void __fastcall SetFontName(const System::UnicodeString Value);
	virtual void __fastcall SetPreviewEdit(Vcl::Stdctrls::TCustomEdit* Value);
	virtual void __fastcall SetShowSymbolFonts(bool Value);
	virtual void __fastcall SetShowStyle(TRzShowStyle Value);
	
public:
	__fastcall virtual TRzFontComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFontComboBox();
	__property Vcl::Graphics::TFont* SelectedFont = {read=GetSelectedFont, write=SetSelectedFont};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzFontDevice FontDevice = {read=FFontDevice, write=SetFontDevice, default=0};
	__property System::UnicodeString FontName = {read=GetFontName, write=SetFontName};
	__property int FontSize = {read=FFontSize, write=FFontSize, default=8};
	__property System::Uitypes::TFontStyles FontStyle = {read=FFontStyle, write=FFontStyle, default=0};
	__property TRzFontType FontType = {read=FFontType, write=SetFontType, default=0};
	__property bool MaintainMRUFonts = {read=FMaintainMRUFonts, write=FMaintainMRUFonts, default=1};
	__property Vcl::Stdctrls::TCustomEdit* PreviewEdit = {read=FPreviewEdit, write=FPreviewEdit};
	__property int PreviewFontSize = {read=FPreviewFontSize, write=FPreviewFontSize, default=36};
	__property int PreviewHeight = {read=FPreviewHeight, write=FPreviewHeight, default=65};
	__property System::UnicodeString PreviewText = {read=FPreviewText, write=FPreviewText};
	__property int PreviewWidth = {read=FPreviewWidth, write=FPreviewWidth, default=260};
	__property bool ShowSymbolFonts = {read=FShowSymbolFonts, write=SetShowSymbolFonts, default=1};
	__property TRzShowStyle ShowStyle = {read=FShowStyle, write=SetShowStyle, default=0};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoCloseUp = {default=0};
	__property AutoDropDown = {default=0};
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
	__property DropDownCount = {default=14};
	__property DropDownWidth = {default=0};
	__property Enabled = {default=1};
	__property FlatButtonColor = {default=-16777201};
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
	__property ItemHeight;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property Sorted = {default=1};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnChange;
	__property OnClick;
	__property OnCloseUp;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDropDown;
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
	__property OnSelect;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzFontComboBox(HWND ParentWindow) : TRzCustomComboBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzMRUComboBox : public TRzCustomComboBox
{
	typedef TRzCustomComboBox inherited;
	
private:
	System::UnicodeString FRemoveItemCaption;
	Vcl::Menus::TPopupMenu* FEmbeddedMenu;
	bool FSelectFirstItemOnLoad;
	Vcl::Menus::TMenuItem* FMnuUndo;
	Vcl::Menus::TMenuItem* FMnuSeparator1;
	Vcl::Menus::TMenuItem* FMnuCut;
	Vcl::Menus::TMenuItem* FMnuCopy;
	Vcl::Menus::TMenuItem* FMnuPaste;
	Vcl::Menus::TMenuItem* FMnuDelete;
	Vcl::Menus::TMenuItem* FMnuSeparator2;
	Vcl::Menus::TMenuItem* FMnuSelectAll;
	Vcl::Menus::TMenuItem* FMnuSeparator3;
	Vcl::Menus::TMenuItem* FMnuRemove;
	Rzcommon::TRzRegIniFile* FMruRegIniFile;
	System::UnicodeString FMruPath;
	System::UnicodeString FMruSection;
	System::UnicodeString FMruID;
	int FMaxHistory;
	System::Classes::TNotifyEvent FOnEscapeKeyPressed;
	System::Classes::TNotifyEvent FOnEnterKeyPressed;
	void __fastcall EmbeddedMenuPopupHandler(System::TObject* Sender);
	void __fastcall MnuUndoClickHandler(System::TObject* Sender);
	void __fastcall MnuCutClickHandler(System::TObject* Sender);
	void __fastcall MnuCopyClickHandler(System::TObject* Sender);
	void __fastcall MnuPasteClickHandler(System::TObject* Sender);
	void __fastcall MnuDeleteClickHandler(System::TObject* Sender);
	void __fastcall MnuSelectAllClickHandler(System::TObject* Sender);
	void __fastcall MnuRemoveItemClickHandler(System::TObject* Sender);
	
protected:
	int FPopupMenuTag;
	bool FDataIsLoaded;
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Loaded();
	virtual void __fastcall CreateWnd();
	DYNAMIC void __fastcall SetupMenuItem(Vcl::Menus::TMenuItem* AMenuItem, System::UnicodeString ACaption, bool AChecked, bool ARadioItem, int AGroupIndex, int AShortCut, System::Classes::TNotifyEvent AHandler);
	virtual void __fastcall CreatePopupMenuItems();
	virtual void __fastcall InitializePopupMenuItems();
	virtual void __fastcall AddMenuItemsToPopupMenu();
	DYNAMIC void __fastcall EnterKeyPressed();
	DYNAMIC void __fastcall EscapeKeyPressed();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall CloseUp();
	DYNAMIC void __fastcall DoExit();
	virtual void __fastcall SetMruRegIniFile(Rzcommon::TRzRegIniFile* Value);
	void __fastcall SetRemoveItemCaption(const System::UnicodeString Value);
	
public:
	__fastcall virtual TRzMRUComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzMRUComboBox();
	DYNAMIC void __fastcall LoadMRUData(bool FromStream);
	DYNAMIC void __fastcall SaveMRUData();
	DYNAMIC void __fastcall UpdateMRUList();
	void __fastcall UpdateMRUListFromCloseUp();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property int MaxHistory = {read=FMaxHistory, write=FMaxHistory, default=25};
	__property System::UnicodeString MruPath = {read=FMruPath, write=FMruPath};
	__property Rzcommon::TRzRegIniFile* MruRegIniFile = {read=FMruRegIniFile, write=SetMruRegIniFile};
	__property System::UnicodeString MruSection = {read=FMruSection, write=FMruSection};
	__property System::UnicodeString MruID = {read=FMruID, write=FMruID};
	__property System::UnicodeString RemoveItemCaption = {read=FRemoveItemCaption, write=SetRemoveItemCaption};
	__property bool SelectFirstItemOnLoad = {read=FSelectFirstItemOnLoad, write=FSelectFirstItemOnLoad, default=0};
	__property System::Classes::TNotifyEvent OnEnterKeyPressed = {read=FOnEnterKeyPressed, write=FOnEnterKeyPressed};
	__property System::Classes::TNotifyEvent OnEscapeKeyPressed = {read=FOnEscapeKeyPressed, write=FOnEscapeKeyPressed};
	__property Style = {default=0};
	__property Align = {default=0};
	__property AllowEdit = {default=1};
	__property Anchors = {default=3};
	__property AutoComplete = {default=1};
	__property AutoCloseUp = {default=0};
	__property AutoDropDown = {default=0};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property CharCase = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property DropDownCount = {default=8};
	__property DropDownWidth = {default=0};
	__property Enabled = {default=1};
	__property FlatButtonColor = {default=-16777201};
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
	__property ItemHeight;
	__property Items;
	__property KeepSearchCase = {default=0};
	__property MaxLength = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Text = {default=0};
	__property TextHint = {default=0};
	__property Touch;
	__property Visible = {default=1};
	__property OnChange;
	__property OnClick;
	__property OnCloseUp;
	__property OnContextPopup;
	__property OnDblClick;
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
	__property OnMatch;
	__property OnMeasureItem;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnNotInList;
	__property OnSelect;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzMRUComboBox(HWND ParentWindow) : TRzCustomComboBox(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzImageComboBoxItem : public System::TObject
{
	typedef System::TObject inherited;
	
protected:
	TRzCustomImageComboBox* FOwner;
	int FItemIndex;
	int FIndex;
	int FIndentLevel;
	int FImageIndex;
	int FOverlayIndex;
	System::UnicodeString FCaption;
	int FTag;
	void *FData;
	void __fastcall SetIndentLevel(int Value);
	void __fastcall SetImageIndex(int Value);
	void __fastcall SetCaption(const System::UnicodeString Value);
	void __fastcall SetOverlayIndex(int Value);
	
public:
	__fastcall TRzImageComboBoxItem(TRzCustomImageComboBox* AOwner);
	__fastcall virtual ~TRzImageComboBoxItem();
	__property int Index = {read=FIndex, nodefault};
	__property int IndentLevel = {read=FIndentLevel, write=SetIndentLevel, nodefault};
	__property int ImageIndex = {read=FImageIndex, write=SetImageIndex, nodefault};
	__property int OverlayIndex = {read=FOverlayIndex, write=SetOverlayIndex, nodefault};
	__property System::UnicodeString Caption = {read=FCaption, write=SetCaption};
	__property void * Data = {read=FData, write=FData};
	__property int Tag = {read=FTag, write=FTag, nodefault};
};

#pragma pack(pop)

typedef void __fastcall (__closure *TRzDeleteImageComboBoxItemEvent)(System::TObject* Sender, TRzImageComboBoxItem* Item);

typedef void __fastcall (__closure *TRzImageComboBoxGetItemDataEvent)(System::TObject* Sender, TRzImageComboBoxItem* Item);

class PASCALIMPLEMENTATION TRzCustomImageComboBox : public TRzCustomComboBox
{
	typedef TRzCustomComboBox inherited;
	
private:
	bool FAutoSizeHeight;
	Vcl::Imglist::TCustomImageList* FImages;
	int FItemIndent;
	TRzDeleteImageComboBoxItemEvent FOnDeleteImageComboBoxItem;
	TRzImageComboBoxGetItemDataEvent FOnGetItemData;
	bool FInWMSetFont;
	bool FFreeObjOnDelete;
	TRzImageComboBoxItem* __fastcall GetImageComboBoxItem(int index);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	MESSAGE void __fastcall WMSetFont(Winapi::Messages::TWMSetFont &Msg);
	
protected:
	void __fastcall DoAutoSize(HFONT hf);
	HIDESBASEDYNAMIC void __fastcall AutoSize(HFONT hf);
	void __fastcall SetItemIndent(int Value);
	void __fastcall SetImages(Vcl::Imglist::TCustomImageList* const Value);
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DeleteItem(void * Item);
	virtual void __fastcall GetItemData(TRzImageComboBoxItem* Item);
	__property Text = {stored=false, default=0};
	__property bool AutoSizeHeight = {read=FAutoSizeHeight, write=FAutoSizeHeight, default=1};
	__property int ItemIndent = {read=FItemIndent, write=SetItemIndent, default=12};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property TRzDeleteImageComboBoxItemEvent OnDeleteItem = {read=FOnDeleteImageComboBoxItem, write=FOnDeleteImageComboBoxItem};
	__property TRzImageComboBoxGetItemDataEvent OnGetItemData = {read=FOnGetItemData, write=FOnGetItemData};
	
public:
	__fastcall virtual TRzCustomImageComboBox(System::Classes::TComponent* AOwner);
	HIDESBASE virtual TRzImageComboBoxItem* __fastcall AddItem(System::UnicodeString Caption, int ImageIndex, int IndentLevel);
	void __fastcall ItemsBeginUpdate();
	void __fastcall ItemsEndUpdate();
	HIDESBASE void __fastcall Delete(int Index);
	HIDESBASE void __fastcall ClearItems();
	virtual int __fastcall IndexOf(const System::UnicodeString S);
	int __fastcall IndexOfItem(TRzImageComboBoxItem* Item);
	int __fastcall IndexOfData(void * Data);
	__property TRzImageComboBoxItem* ImageComboItem[int Index] = {read=GetImageComboBoxItem};
public:
	/* TRzCustomComboBox.Destroy */ inline __fastcall virtual ~TRzCustomImageComboBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomImageComboBox(HWND ParentWindow) : TRzCustomComboBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzImageComboBox : public TRzCustomImageComboBox
{
	typedef TRzCustomImageComboBox inherited;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoCloseUp = {default=0};
	__property AutoDropDown = {default=0};
	__property AutoSizeHeight = {default=1};
	__property BiDiMode;
	__property CharCase = {default=0};
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
	__property Font;
	__property FlatButtonColor = {default=-16777201};
	__property FlatButtons = {default=0};
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
	__property Images;
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property ItemHeight;
	__property ItemIndent = {default=12};
	__property MaxLength = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property Sorted = {default=0};
	__property TabOnEnter = {default=0};
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
	__property OnGetItemData;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMeasureItem;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
	__property OnSelect;
	__property OnSelEndCancel;
	__property OnSelEndOk;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TRzCustomImageComboBox.Create */ inline __fastcall virtual TRzImageComboBox(System::Classes::TComponent* AOwner) : TRzCustomImageComboBox(AOwner) { }
	
public:
	/* TRzCustomComboBox.Destroy */ inline __fastcall virtual ~TRzImageComboBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzImageComboBox(HWND ParentWindow) : TRzCustomImageComboBox(ParentWindow) { }
	
};


struct DECLSPEC_DRECORD TRzColorRec
{
public:
	System::UnicodeString Name;
	System::Uitypes::TColor Color;
};


typedef System::StaticArray<TRzColorRec, 16> Rzcmbobx__11;

typedef System::StaticArray<TRzColorRec, 25> Rzcmbobx__21;

//-- var, const, procedure ---------------------------------------------------
static const System::Int8 MaxStdColors = System::Int8(0x10);
static const System::Int8 MaxSysColors = System::Int8(0x19);
extern DELPHI_PACKAGE TRzColorRec DefaultColorItem;
extern DELPHI_PACKAGE TRzColorRec CustomColorItem;
extern DELPHI_PACKAGE Rzcmbobx__11 StdColorItems;
extern DELPHI_PACKAGE Rzcmbobx__21 SysColorItems;
#define ptDefault L"AaBbYyZz"
#define ptDefault1 L"AaBbYyZ"
#define ptDefault2 L"AaBbYy"
}	/* namespace Rzcmbobx */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCMBOBX)
using namespace Rzcmbobx;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzcmbobxHPP
