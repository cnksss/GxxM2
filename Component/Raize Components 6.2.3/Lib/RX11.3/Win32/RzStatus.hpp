// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzStatus.pas' rev: 35.00 (Windows)

#ifndef RzstatusHPP
#define RzstatusHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <RzCommon.hpp>
#include <Vcl.Buttons.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <System.Classes.hpp>
#include <RzPrgres.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzstatus
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomStatusPane;
class DELPHICLASS TRzStatusPane;
class DELPHICLASS TRzCustomGlyphStatus;
class DELPHICLASS TRzGlyphStatus;
class DELPHICLASS TRzPollingStatus;
class DELPHICLASS TRzClockStatus;
class DELPHICLASS TRzKeyStrings;
class DELPHICLASS TRzKeyStatus;
class DELPHICLASS TRzResourceStrings;
class DELPHICLASS TRzResourceStatus;
class DELPHICLASS TRzMarqueeStatus;
class DELPHICLASS TRzCustomFieldStatus;
class DELPHICLASS TRzFieldStatus;
class DELPHICLASS TRzVersionInfo;
class DELPHICLASS TRzVersionInfoStatus;
class DELPHICLASS TRzProgressStatus;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzCustomStatusPane : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	System::Classes::TAlignment FAlignment;
	bool FAutoSize;
	bool FAutoHint;
	int FDefaultWidth;
	System::Uitypes::TColor FBlinkColor;
	Rzcommon::TBlinkState FBlinkState;
	bool FBlinking;
	System::Uitypes::TColor FFontColor;
	Vcl::Controls::TBorderWidth FBorderWidth;
	Vcl::Controls::TCaption FCaption;
	int FCaptionOffset;
	bool FParentFillColor;
	System::Uitypes::TColor FFillColor;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	Rzcommon::TFrameStyleEx FFrameStyle;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	bool FTransparent;
	MESSAGE void __fastcall CMGetBlinking(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMBlink(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMHitTest(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::Uitypes::TColor FCurrentColor;
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	DYNAMIC void __fastcall AdjustBounds();
	virtual int __fastcall GetMinWidth();
	virtual System::Types::TRect __fastcall GetCaptionRect();
	System::Uitypes::TColor __fastcall GetFontColor();
	System::Uitypes::TColor __fastcall GetDisabledFontColor();
	virtual void __fastcall DrawCaption(const System::UnicodeString CaptionStr);
	virtual void __fastcall DrawStatusBorder();
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall Blink(Rzcommon::TBlinkState State);
	virtual void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetAutoSize(bool Value);
	virtual void __fastcall SetDefaultWidth(int Value);
	virtual void __fastcall SetBlinking(bool Value);
	virtual void __fastcall SetBlinkColor(System::Uitypes::TColor Value);
	virtual System::Word __fastcall GetBlinkIntervalOff();
	virtual void __fastcall SetBlinkIntervalOff(System::Word Value);
	virtual System::Word __fastcall GetBlinkIntervalOn();
	virtual void __fastcall SetBlinkIntervalOn(System::Word Value);
	virtual void __fastcall SetBorderWidth(Vcl::Controls::TBorderWidth Value);
	virtual void __fastcall SetCaption(Vcl::Controls::TCaption Value);
	virtual void __fastcall SetCaptionOffset(int Value);
	bool __fastcall IsFillColorStored();
	virtual void __fastcall SetFillColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetParentFillColor(bool Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual bool __fastcall UnderSizeGrip(const System::Types::TRect &R);
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=SetAlignment, default=0};
	__property bool AutoHint = {read=FAutoHint, write=FAutoHint, default=0};
	__property bool AutoSize = {read=FAutoSize, write=SetAutoSize, default=0};
	__property bool Blinking = {read=FBlinking, write=SetBlinking, default=0};
	__property System::Uitypes::TColor BlinkColor = {read=FBlinkColor, write=SetBlinkColor, default=-16777203};
	__property Rzcommon::TBlinkState BlinkState = {read=FBlinkState, nodefault};
	__property System::Word BlinkIntervalOff = {read=GetBlinkIntervalOff, write=SetBlinkIntervalOff, default=500};
	__property System::Word BlinkIntervalOn = {read=GetBlinkIntervalOn, write=SetBlinkIntervalOn, default=500};
	__property int CaptionOffset = {read=FCaptionOffset, write=SetCaptionOffset, default=2};
	__property Vcl::Controls::TCaption Caption = {read=FCaption, write=SetCaption};
	__property int DefaultWidth = {read=FDefaultWidth, write=SetDefaultWidth, default=40};
	
public:
	__fastcall virtual TRzCustomStatusPane(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomStatusPane();
	DYNAMIC bool __fastcall ExecuteAction(System::Classes::TBasicAction* Action);
	bool __fastcall UseThemes();
	__property Canvas;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Controls::TBorderWidth BorderWidth = {read=FBorderWidth, write=SetBorderWidth, default=1};
	__property System::Uitypes::TColor FillColor = {read=FFillColor, write=SetFillColor, stored=IsFillColorStored, default=-16777201};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, default=1};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property bool ParentFillColor = {read=FParentFillColor, write=SetParentFillColor, default=1};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=1};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Color = {default=-16777211};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property Height = {default=20};
	__property ParentColor = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Touch;
	__property Visible = {default=1};
	__property Width = {default=100};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
private:
	void *__IRzCustomFramingNotification;	// Rzcommon::IRzCustomFramingNotification 
	
public:
	#if defined(MANAGED_INTERFACE_OPERATORS)
	// {64B1C9EA-C954-428A-95C4-4EA2EB0F1E16}
	operator Rzcommon::_di_IRzCustomFramingNotification()
	{
		Rzcommon::_di_IRzCustomFramingNotification intf;
		this->GetInterface(intf);
		return intf;
	}
	#else
	operator Rzcommon::IRzCustomFramingNotification*(void) { return (Rzcommon::IRzCustomFramingNotification*)&__IRzCustomFramingNotification; }
	#endif
	
};


class PASCALIMPLEMENTATION TRzStatusPane : public TRzCustomStatusPane
{
	typedef TRzCustomStatusPane inherited;
	
__published:
	__property Alignment = {default=0};
	__property AutoHint = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property Caption;
	__property CaptionOffset = {default=2};
	__property DefaultWidth = {default=40};
public:
	/* TRzCustomStatusPane.Create */ inline __fastcall virtual TRzStatusPane(System::Classes::TComponent* AOwner) : TRzCustomStatusPane(AOwner) { }
	/* TRzCustomStatusPane.Destroy */ inline __fastcall virtual ~TRzStatusPane() { }
	
};


enum DECLSPEC_DENUM TGlyphAlignment : unsigned char { gaLeft, gaRight };

class PASCALIMPLEMENTATION TRzCustomGlyphStatus : public TRzCustomStatusPane
{
	typedef TRzCustomStatusPane inherited;
	
private:
	Vcl::Graphics::TBitmap* FGlyph;
	TGlyphAlignment FGlyphAlignment;
	int FGlyphOffset;
	Vcl::Buttons::TNumGlyphs FNumGlyphs;
	bool FShowGlyph;
	System::Uitypes::TImageIndex FImageIndex;
	System::Uitypes::TImageIndex FDisabledIndex;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	void __fastcall GlyphChangedHandler(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::Types::TRect __fastcall GetCaptionRect();
	virtual int __fastcall GetMinWidth();
	System::Types::TPoint __fastcall GetImageSize();
	virtual void __fastcall Paint();
	virtual void __fastcall DrawGlyph(const System::Types::TRect &R);
	virtual void __fastcall DrawImage(const System::Types::TRect &R);
	virtual void __fastcall SetNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual void __fastcall SetGlyph(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetGlyphAlignment(TGlyphAlignment Value);
	virtual void __fastcall SetGlyphOffset(int Value);
	virtual void __fastcall SetShowGlyph(bool Value);
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	__property Vcl::Graphics::TBitmap* Glyph = {read=FGlyph, write=SetGlyph};
	__property TGlyphAlignment GlyphAlignment = {read=FGlyphAlignment, write=SetGlyphAlignment, default=0};
	__property int GlyphOffset = {read=FGlyphOffset, write=SetGlyphOffset, default=2};
	__property Vcl::Buttons::TNumGlyphs NumGlyphs = {read=FNumGlyphs, write=SetNumGlyphs, default=1};
	__property bool ShowGlyph = {read=FShowGlyph, write=SetShowGlyph, default=1};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, default=-1};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	
public:
	__fastcall virtual TRzCustomGlyphStatus(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomGlyphStatus();
};


class PASCALIMPLEMENTATION TRzGlyphStatus : public TRzCustomGlyphStatus
{
	typedef TRzCustomGlyphStatus inherited;
	
__published:
	__property Alignment = {default=0};
	__property AutoHint = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property Caption;
	__property CaptionOffset = {default=2};
	__property DefaultWidth = {default=40};
	__property Glyph;
	__property GlyphAlignment = {default=0};
	__property GlyphOffset = {default=2};
	__property ImageIndex = {default=-1};
	__property DisabledIndex = {default=-1};
	__property Images;
	__property NumGlyphs = {default=1};
	__property ShowGlyph = {default=1};
public:
	/* TRzCustomGlyphStatus.Create */ inline __fastcall virtual TRzGlyphStatus(System::Classes::TComponent* AOwner) : TRzCustomGlyphStatus(AOwner) { }
	/* TRzCustomGlyphStatus.Destroy */ inline __fastcall virtual ~TRzGlyphStatus() { }
	
};


class PASCALIMPLEMENTATION TRzPollingStatus : public TRzCustomStatusPane
{
	typedef TRzCustomStatusPane inherited;
	
private:
	System::Classes::TNotifyEvent FOnTimerExpired;
	
protected:
	DYNAMIC void __fastcall TimerExpired();
	virtual bool __fastcall GetActive();
	virtual void __fastcall SetActive(bool Value);
	virtual System::Word __fastcall GetInterval();
	virtual void __fastcall SetInterval(System::Word Value);
	
public:
	__fastcall virtual TRzPollingStatus(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzPollingStatus();
	
__published:
	__property bool Active = {read=GetActive, write=SetActive, default=1};
	__property System::Word Interval = {read=GetInterval, write=SetInterval, default=500};
	__property System::Classes::TNotifyEvent OnTimerExpired = {read=FOnTimerExpired, write=FOnTimerExpired};
};


typedef void __fastcall (__closure *TRzGetDateTimeEvent)(System::TObject* Sender, System::TDateTime &DateTime);

class PASCALIMPLEMENTATION TRzClockStatus : public TRzPollingStatus
{
	typedef TRzPollingStatus inherited;
	
private:
	System::UnicodeString FFormat;
	System::UnicodeString FDateTimeStr;
	TRzGetDateTimeEvent FOnGetDateTime;
	
protected:
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall TimerExpired();
	virtual System::Types::TRect __fastcall GetCaptionRect();
	virtual void __fastcall Paint();
	virtual void __fastcall SetCurrentDateTimeStr();
	void __fastcall SetFormat(System::UnicodeString Value);
	
public:
	__fastcall virtual TRzClockStatus(System::Classes::TComponent* AOwner);
	
__published:
	__property System::UnicodeString Format = {read=FFormat, write=SetFormat};
	__property TRzGetDateTimeEvent OnGetDateTime = {read=FOnGetDateTime, write=FOnGetDateTime};
	__property Alignment = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property CaptionOffset = {default=2};
	__property DefaultWidth = {default=40};
	__property Width = {default=150};
public:
	/* TRzPollingStatus.Destroy */ inline __fastcall virtual ~TRzClockStatus() { }
	
};


enum DECLSPEC_DENUM TRzToggleKey : unsigned char { tkCapsLock, tkNumLock, tkScrollLock, tkInsert };

enum DECLSPEC_DENUM TRzToggleState : unsigned char { tsOn, tsOff };

enum DECLSPEC_DENUM TRzInsertState : unsigned char { isInsert, isOverwrite };

enum DECLSPEC_DENUM TRzKeyOffAppearance : unsigned char { koaBlank, koaDimmed };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzKeyStrings : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzKeyStatus* FKeyStatus;
	System::UnicodeString FCapsLock;
	System::UnicodeString FNumLock;
	System::UnicodeString FScrollLock;
	System::UnicodeString FInsert;
	System::UnicodeString FOverwrite;
	
protected:
	void __fastcall SetKeyString(int Index, const System::UnicodeString Value);
	
public:
	__fastcall TRzKeyStrings();
	
__published:
	__property System::UnicodeString CapsLock = {read=FCapsLock, write=SetKeyString, index=0};
	__property System::UnicodeString NumLock = {read=FNumLock, write=SetKeyString, index=1};
	__property System::UnicodeString ScrollLock = {read=FScrollLock, write=SetKeyString, index=2};
	__property System::UnicodeString Insert = {read=FInsert, write=SetKeyString, index=3};
	__property System::UnicodeString Overwrite = {read=FOverwrite, write=SetKeyString, index=4};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzKeyStrings() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzKeyStatus : public TRzPollingStatus
{
	typedef TRzPollingStatus inherited;
	
private:
	TRzToggleKey FKey;
	TRzKeyStrings* FKeyStrings;
	bool FStoreKeyStrings;
	TRzToggleState FState;
	TRzInsertState FInitInsertState;
	TRzInsertState FCurrentInsertState;
	bool FInitInsertKeyState;
	TRzKeyOffAppearance FKeyOffAppearance;
	
protected:
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall TimerExpired();
	virtual void __fastcall Paint();
	System::UnicodeString __fastcall GetKeyString(TRzToggleKey Key);
	void __fastcall KeyStringChanged();
	virtual void __fastcall SetKey(TRzToggleKey Value);
	virtual void __fastcall SetKeyOffAppearance(TRzKeyOffAppearance Value);
	virtual void __fastcall SetKeyStrings(TRzKeyStrings* Value);
	virtual void __fastcall SetState(TRzToggleState Value);
	virtual void __fastcall SetInitInsertState(TRzInsertState Value);
	virtual void __fastcall SetCurrentInsertState(TRzInsertState Value);
	TRzInsertState __fastcall GetInsertState();
	void __fastcall SetInsertState(TRzInsertState Value);
	
public:
	__fastcall virtual TRzKeyStatus(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzKeyStatus();
	__property TRzToggleState State = {read=FState, nodefault};
	
__published:
	__property TRzToggleKey Key = {read=FKey, write=SetKey, default=0};
	__property TRzKeyOffAppearance KeyOffAppearance = {read=FKeyOffAppearance, write=SetKeyOffAppearance, default=0};
	__property TRzKeyStrings* KeyStrings = {read=FKeyStrings, write=SetKeyStrings, stored=FStoreKeyStrings};
	__property TRzInsertState InsertState = {read=GetInsertState, write=SetInsertState, default=0};
	__property Alignment = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property DefaultWidth = {default=40};
	__property Width = {default=45};
};


enum DECLSPEC_DENUM TResourceType : unsigned char { rtSystem, rtMemory };

enum DECLSPEC_DENUM TDisplayStyle : unsigned char { dsBar, dsText };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzResourceStrings : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzResourceStatus* FResourceStatus;
	System::UnicodeString FSystem;
	System::UnicodeString FMemory;
	
protected:
	void __fastcall SetResourceString(int Index, const System::UnicodeString Value);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	
public:
	__fastcall TRzResourceStrings();
	
__published:
	__property System::UnicodeString System = {read=FSystem, write=SetResourceString, index=0};
	__property System::UnicodeString Memory = {read=FMemory, write=SetResourceString, index=1};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzResourceStrings() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzResourceStatus : public TRzPollingStatus
{
	typedef TRzPollingStatus inherited;
	
private:
	TRzResourceStrings* FResourceStrings;
	bool FStoreResourceStrings;
	System::Uitypes::TColor FBackColor;
	System::Uitypes::TColor FBackColorStop;
	System::Uitypes::TColor FBarColor;
	System::Uitypes::TColor FBarColorStop;
	Rzcommon::TBarStyle FBarStyle;
	int FFreeMemory;
	int FFreePercent;
	Rzcommon::TGradientDirection FGradientDirection;
	Rzcommon::TOrientation FOrientation;
	Rzprgres::TSegmentRange FNumSegments;
	TResourceType FResourceType;
	bool FShowPercent;
	TDisplayStyle FDisplayStyle;
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	DYNAMIC void __fastcall TimerExpired();
	virtual void __fastcall Paint();
	System::UnicodeString __fastcall GetResourceString(TResourceType ResType);
	void __fastcall ResourceStringChanged();
	virtual void __fastcall SetBackColor(System::Uitypes::TColor Value);
	void __fastcall SetBackColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetBarColor(System::Uitypes::TColor Value);
	void __fastcall SetBarColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetBarStyle(Rzcommon::TBarStyle Value);
	virtual void __fastcall SetDisplayStyle(TDisplayStyle Value);
	void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetNumSegments(Rzprgres::TSegmentRange Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetResourceStrings(TRzResourceStrings* Value);
	virtual void __fastcall SetShowPercent(bool Value);
	
public:
	__fastcall virtual TRzResourceStatus(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzResourceStatus();
	__property int FreeMemory = {read=FFreeMemory, nodefault};
	__property int FreePercent = {read=FFreePercent, nodefault};
	
__published:
	__property System::Uitypes::TColor BackColor = {read=FBackColor, write=SetBackColor, default=-16777201};
	__property System::Uitypes::TColor BackColorStop = {read=FBackColorStop, write=SetBackColorStop, default=-16777201};
	__property System::Uitypes::TColor BarColor = {read=FBarColor, write=SetBarColor, default=-16777203};
	__property System::Uitypes::TColor BarColorStop = {read=FBarColorStop, write=SetBarColorStop, default=-16777190};
	__property Rzcommon::TBarStyle BarStyle = {read=FBarStyle, write=SetBarStyle, default=0};
	__property TDisplayStyle DisplayStyle = {read=FDisplayStyle, write=SetDisplayStyle, default=0};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=3};
	__property Rzprgres::TSegmentRange NumSegments = {read=FNumSegments, write=SetNumSegments, default=15};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property TRzResourceStrings* ResourceStrings = {read=FResourceStrings, write=SetResourceStrings, stored=FStoreResourceStrings};
	__property bool ShowPercent = {read=FShowPercent, write=SetShowPercent, default=0};
	__property Alignment = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property CaptionOffset = {default=2};
	__property Hint = {stored=false, default=0};
	__property ShowHint = {default=1};
};


class PASCALIMPLEMENTATION TRzMarqueeStatus : public TRzStatusPane
{
	typedef TRzStatusPane inherited;
	
private:
	Rzcommon::TRzScrollType FScrollType;
	bool FScrolling;
	int FSteps;
	System::Word FScrollDelay;
	System::Word FScrollSize;
	Vcl::Graphics::TBitmap* FBitmap;
	Vcl::Extctrls::TTimer* FTimer;
	int FCurrentStep;
	Rzcommon::TRzScrollDisplayEvent FOnScrollDisplay;
	System::Classes::TNotifyEvent FOnScrollComplete;
	void __fastcall TimerEventHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall DrawCaption(const System::UnicodeString CaptionStr);
	void __fastcall UpdateSteps();
	DYNAMIC void __fastcall ScrollDisplay(int CurrentStep, int TotalSteps);
	DYNAMIC void __fastcall ScrollComplete();
	void __fastcall SetScrollType(Rzcommon::TRzScrollType Value);
	virtual void __fastcall SetScrolling(bool Value);
	virtual void __fastcall SetScrollSize(System::Word Value);
	void __fastcall SetScrollDelay(System::Word Value);
	
public:
	__fastcall virtual TRzMarqueeStatus(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzMarqueeStatus();
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	
__published:
	__property Rzcommon::TRzScrollType ScrollType = {read=FScrollType, write=SetScrollType, default=1};
	__property bool Scrolling = {read=FScrolling, write=SetScrolling, default=1};
	__property System::Word ScrollSize = {read=FScrollSize, write=SetScrollSize, default=1};
	__property System::Word ScrollDelay = {read=FScrollDelay, write=SetScrollDelay, default=100};
	__property Rzcommon::TRzScrollDisplayEvent OnScrollDisplay = {read=FOnScrollDisplay, write=FOnScrollDisplay};
	__property System::Classes::TNotifyEvent OnScrollComplete = {read=FOnScrollComplete, write=FOnScrollComplete};
};


class PASCALIMPLEMENTATION TRzCustomFieldStatus : public TRzCustomStatusPane
{
	typedef TRzCustomStatusPane inherited;
	
private:
	System::UnicodeString FFieldLabel;
	System::Uitypes::TColor FFieldLabelColor;
	
protected:
	virtual int __fastcall GetMinWidth();
	virtual System::Types::TRect __fastcall GetCaptionRect();
	virtual void __fastcall DrawCaption(const System::UnicodeString CaptionStr);
	virtual void __fastcall SetFieldLabel(const System::UnicodeString Value);
	virtual void __fastcall SetFieldLabelColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzCustomFieldStatus(System::Classes::TComponent* AOwner);
	
__published:
	__property System::UnicodeString FieldLabel = {read=FFieldLabel, write=SetFieldLabel};
	__property System::Uitypes::TColor FieldLabelColor = {read=FFieldLabelColor, write=SetFieldLabelColor, default=-16777203};
	__property Alignment = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property CaptionOffset = {default=2};
public:
	/* TRzCustomStatusPane.Destroy */ inline __fastcall virtual ~TRzCustomFieldStatus() { }
	
};


class PASCALIMPLEMENTATION TRzFieldStatus : public TRzCustomFieldStatus
{
	typedef TRzCustomFieldStatus inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property AutoHint = {default=0};
	__property Caption;
public:
	/* TRzCustomFieldStatus.Create */ inline __fastcall virtual TRzFieldStatus(System::Classes::TComponent* AOwner) : TRzCustomFieldStatus(AOwner) { }
	
public:
	/* TRzCustomStatusPane.Destroy */ inline __fastcall virtual ~TRzFieldStatus() { }
	
};


enum DECLSPEC_DENUM TRzVersionInfoField : unsigned char { vifCompanyName, vifFileDescription, vifFileVersion, vifInternalName, vifCopyright, vifTrademarks, vifOriginalFilename, vifProductName, vifProductVersion, vifComments };

class PASCALIMPLEMENTATION TRzVersionInfo : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
public:
	System::UnicodeString operator[](TRzVersionInfoField Index) { return this->Fields[Index]; }
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::UnicodeString FFilePath;
	System::Classes::TStringList* FVersionInfo;
	bool FVersionInfoAvailable;
	System::Classes::TList* FStatusList;
	System::StaticArray<System::Word, 4> FVersionNumbers;
	System::StaticArray<bool, 4> FModuleAttributes;
	
protected:
	virtual void __fastcall Loaded();
	void __fastcall GetVersionInfo();
	System::UnicodeString __fastcall GetField(int Index);
	System::UnicodeString __fastcall GetVerField(TRzVersionInfoField Index);
	System::Word __fastcall GetVersionNumber(int Index);
	bool __fastcall GetModuleAttribute(int Index);
	System::TDateTime __fastcall GetFileDateTime();
	void __fastcall UpdateStatusControls();
	virtual void __fastcall SetFilePath(const System::UnicodeString Value);
	__property System::UnicodeString Fields[TRzVersionInfoField Index] = {read=GetVerField/*, default*/};
	
public:
	__fastcall virtual TRzVersionInfo(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzVersionInfo();
	void __fastcall AddStatus(TRzVersionInfoStatus* S);
	void __fastcall RemoveStatus(TRzVersionInfoStatus* S);
	System::UnicodeString __fastcall GetCustomKeyValue(const System::UnicodeString Key);
	__property bool VersionInfoAvailable = {read=FVersionInfoAvailable, nodefault};
	__property System::UnicodeString CompanyName = {read=GetField, index=0};
	__property System::UnicodeString FileDescription = {read=GetField, index=1};
	__property System::UnicodeString FileVersion = {read=GetField, index=2};
	__property System::UnicodeString InternalName = {read=GetField, index=3};
	__property System::UnicodeString Copyright = {read=GetField, index=4};
	__property System::UnicodeString Trademarks = {read=GetField, index=5};
	__property System::UnicodeString OriginalFilename = {read=GetField, index=6};
	__property System::UnicodeString ProductName = {read=GetField, index=7};
	__property System::UnicodeString ProductVersion = {read=GetField, index=8};
	__property System::UnicodeString Comments = {read=GetField, index=9};
	__property System::Word MajorVersion = {read=GetVersionNumber, index=1, nodefault};
	__property System::Word MinorVersion = {read=GetVersionNumber, index=2, nodefault};
	__property System::Word Release = {read=GetVersionNumber, index=3, nodefault};
	__property System::Word Build = {read=GetVersionNumber, index=4, nodefault};
	__property bool DebugBuild = {read=GetModuleAttribute, index=1, nodefault};
	__property bool PreRelease = {read=GetModuleAttribute, index=2, nodefault};
	__property bool PrivateBuild = {read=GetModuleAttribute, index=3, nodefault};
	__property bool SpecialBuild = {read=GetModuleAttribute, index=4, nodefault};
	__property System::TDateTime FileDateTime = {read=GetFileDateTime};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString FilePath = {read=FFilePath, write=SetFilePath};
};


class PASCALIMPLEMENTATION TRzVersionInfoStatus : public TRzCustomFieldStatus
{
	typedef TRzCustomFieldStatus inherited;
	
private:
	TRzVersionInfo* FVersionInfo;
	TRzVersionInfoField FField;
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	void __fastcall UpdateCaption();
	virtual void __fastcall SetVersionInfo(TRzVersionInfo* Value);
	virtual void __fastcall SetField(TRzVersionInfoField Value);
	
public:
	__fastcall virtual TRzVersionInfoStatus(System::Classes::TComponent* AOwner);
	
__published:
	__property TRzVersionInfoField Field = {read=FField, write=SetField, nodefault};
	__property TRzVersionInfo* VersionInfo = {read=FVersionInfo, write=SetVersionInfo};
	__property Alignment = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property CaptionOffset = {default=2};
public:
	/* TRzCustomStatusPane.Destroy */ inline __fastcall virtual ~TRzVersionInfoStatus() { }
	
};


class PASCALIMPLEMENTATION TRzProgressStatus : public TRzCustomStatusPane
{
	typedef TRzCustomStatusPane inherited;
	
private:
	System::Uitypes::TColor FBackColor;
	System::Uitypes::TColor FBackColorStop;
	System::Uitypes::TColor FBarColor;
	System::Uitypes::TColor FBarColorStop;
	Rzcommon::TBarStyle FBarStyle;
	Rzcommon::TGradientDirection FGradientDirection;
	Rzprgres::TSegmentRange FNumSegments;
	Rzprgres::TUnsignedLongint FPartsComplete;
	System::Word FPercent;
	bool FShowPercent;
	bool FShowParts;
	Rzprgres::TUnsignedLongint FTotalParts;
	
protected:
	virtual void __fastcall Paint();
	virtual void __fastcall SetBackColor(System::Uitypes::TColor Value);
	void __fastcall SetBackColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetBarColor(System::Uitypes::TColor Value);
	void __fastcall SetBarColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetBarStyle(Rzcommon::TBarStyle Value);
	void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetNumSegments(Rzprgres::TSegmentRange Value);
	virtual void __fastcall SetPartsComplete(Rzprgres::TUnsignedLongint Value);
	virtual void __fastcall SetPercent(System::Word Value);
	virtual void __fastcall SetShowPercent(bool Value);
	virtual void __fastcall SetShowParts(bool Value);
	virtual void __fastcall SetTotalParts(Rzprgres::TUnsignedLongint Value);
	
public:
	__fastcall virtual TRzProgressStatus(System::Classes::TComponent* AOwner);
	void __fastcall IncPartsByOne();
	void __fastcall IncParts(Rzprgres::TUnsignedLongint N);
	
__published:
	__property System::Uitypes::TColor BackColor = {read=FBackColor, write=SetBackColor, default=-16777201};
	__property System::Uitypes::TColor BackColorStop = {read=FBackColorStop, write=SetBackColorStop, default=-16777201};
	__property System::Uitypes::TColor BarColor = {read=FBarColor, write=SetBarColor, default=-16777203};
	__property System::Uitypes::TColor BarColorStop = {read=FBarColorStop, write=SetBarColorStop, default=-16777190};
	__property Rzcommon::TBarStyle BarStyle = {read=FBarStyle, write=SetBarStyle, default=0};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=3};
	__property Rzprgres::TSegmentRange NumSegments = {read=FNumSegments, write=SetNumSegments, default=15};
	__property Rzprgres::TUnsignedLongint PartsComplete = {read=FPartsComplete, write=SetPartsComplete, nodefault};
	__property System::Word Percent = {read=FPercent, write=SetPercent, nodefault};
	__property bool ShowPercent = {read=FShowPercent, write=SetShowPercent, default=0};
	__property bool ShowParts = {read=FShowParts, write=SetShowParts, default=0};
	__property Rzprgres::TUnsignedLongint TotalParts = {read=FTotalParts, write=SetTotalParts, nodefault};
	__property Alignment = {default=0};
	__property Hint = {stored=false, default=0};
	__property ShowHint = {default=1};
public:
	/* TRzCustomStatusPane.Destroy */ inline __fastcall virtual ~TRzProgressStatus() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzstatus */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSTATUS)
using namespace Rzstatus;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzstatusHPP
