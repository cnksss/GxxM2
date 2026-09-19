// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBCmbo.pas' rev: 35.00 (Windows)

#ifndef RzdbcmboHPP
#define RzdbcmboHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCmboBx.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Data.DB.hpp>
#include <Vcl.VDBConsts.hpp>
#include <System.Variants.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Menus.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbcmbo
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzPaintComboBox;
class DELPHICLASS TRzDBComboBox;
class DELPHICLASS TRzDBLookupComboBox;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzPaintComboBox : public Vcl::Extctrls::TCustomPanel
{
	typedef Vcl::Extctrls::TCustomPanel inherited;
	
private:
	TRzDBComboBox* FComboBox;
	
protected:
	virtual void __fastcall Paint();
	
public:
	__fastcall virtual TRzPaintComboBox(System::Classes::TComponent* AOwner);
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzPaintComboBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPaintComboBox(HWND ParentWindow) : Vcl::Extctrls::TCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBComboBox : public Rzcmbobx::TRzCustomComboBox
{
	typedef Rzcmbobx::TRzCustomComboBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	TRzPaintComboBox* FPaintControl;
	System::UnicodeString FValue;
	System::Classes::TStrings* FValues;
	bool FInSetValue;
	bool FInDataChange;
	bool FForceSetValue;
	void __fastcall DataChange(System::TObject* Sender);
	void __fastcall EditingChange(System::TObject* Sender);
	void __fastcall UpdateData(System::TObject* Sender);
	void __fastcall ValuesChangedHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall WMChar(Winapi::Messages::TWMKey &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall ComboWndProc(Winapi::Messages::TMessage &Msg, HWND ComboWnd, void * ComboProc);
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	virtual System::UnicodeString __fastcall GetComboText();
	virtual void __fastcall SetComboText(const System::UnicodeString Value);
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual void __fastcall SetEditReadOnly();
	virtual Data::Db::TField* __fastcall GetField();
	virtual void __fastcall SetItems(System::Classes::TStrings* const Value);
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetStyle(Vcl::Stdctrls::TComboBoxStyle Value);
	virtual System::UnicodeString __fastcall GetItemValue(int Index);
	virtual System::UnicodeString __fastcall GetValue();
	virtual void __fastcall SetValue(const System::UnicodeString Value);
	virtual void __fastcall SetValues(System::Classes::TStrings* Value);
	
public:
	__fastcall virtual TRzDBComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBComboBox();
	void __fastcall ClearItemsValues();
	void __fastcall AddItemValue(const System::UnicodeString Item, const System::UnicodeString Value);
	void __fastcall InsertItemValue(int Index, const System::UnicodeString Item, const System::UnicodeString Value);
	DYNAMIC bool __fastcall ExecuteAction(System::Classes::TBasicAction* Action);
	virtual bool __fastcall UpdateAction(System::Classes::TBasicAction* Action);
	DYNAMIC bool __fastcall UseRightToLeftAlignment();
	__property Data::Db::TField* Field = {read=GetField};
	__property System::UnicodeString Value = {read=GetValue, write=SetValue};
	__property Text = {default=0};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property Style = {default=0};
	__property Align = {default=0};
	__property AllowEdit = {default=1};
	__property Anchors = {default=3};
	__property AutoComplete = {default=1};
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
	__property KeepSearchCase = {default=0};
	__property MaxLength = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property Sorted = {default=0};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
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
	__property Items = {write=SetItems};
	__property System::Classes::TStrings* Values = {read=FValues, write=SetValues};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBComboBox(HWND ParentWindow) : Rzcmbobx::TRzCustomComboBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBLookupComboBox : public Vcl::Dbctrls::TDBLookupComboBox
{
	typedef Vcl::Dbctrls::TDBLookupComboBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAllowNull;
	bool FAutoDropDown;
	System::Classes::TAlignment FTextAlignment;
	int FButtonWidth;
	Vcl::Graphics::TCanvas* FCanvas;
	System::Uitypes::TColor FFlatButtonColor;
	bool FFlatButtons;
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
	bool FTabOnEnter;
	bool FSelectOnTab;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TMessage &Msg);
	
protected:
	bool FInControl;
	bool FOverControl;
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Paint();
	virtual void __fastcall KeyValueChanged();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::Types::TRect __fastcall GetClientRect();
	virtual int __fastcall GetBorderSize();
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	virtual void __fastcall SetFlatButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatButtons(bool Value);
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
	HIDESBASE bool __fastcall GetReadOnly();
	HIDESBASE void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzDBLookupComboBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBLookupComboBox();
	virtual bool __fastcall UseThemes();
	System::UnicodeString __fastcall GetListValue();
	System::UnicodeString __fastcall GetKeyValue();
	void __fastcall InitKeyValue();
	void __fastcall ClearKeyValue();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowNull = {read=FAllowNull, write=FAllowNull, default=0};
	__property bool AutoDropDown = {read=FAutoDropDown, write=FAutoDropDown, default=0};
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
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, stored=StoreReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=FReadOnlyColorOnFocus, stored=StoreReadOnlyColorOnFocus, default=0};
	__property bool SelectOnTab = {read=FSelectOnTab, write=FSelectOnTab, default=0};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property Align = {default=0};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBLookupComboBox(HWND ParentWindow) : Vcl::Dbctrls::TDBLookupComboBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbcmbo */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBCMBO)
using namespace Rzdbcmbo;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbcmboHPP
