// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzBorder.pas' rev: 35.00 (Windows)

#ifndef RzborderHPP
#define RzborderHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Vcl.Controls.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Forms.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.Menus.hpp>
#include <RzGrafx.hpp>
#include <RzPanel.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzborder
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzBorder;
class DELPHICLASS TRzLEDDisplay;
class DELPHICLASS TRzProgressDisplay;
class DELPHICLASS TRzMeter;
class DELPHICLASS TRzColorPicker;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzBorder : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Controls::TBevelWidth FBevelWidth;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Vcl::Controls::TBorderWidth FBorderWidth;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FBorderHighlight;
	System::Uitypes::TColor FBorderShadow;
	Rzcommon::TSides FBorderSides;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::Types::TRect __fastcall GetClientRect();
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	virtual System::Uitypes::TColor __fastcall InteriorColor();
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall SetBevelWidth(Vcl::Controls::TBevelWidth Value);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderWidth(Vcl::Controls::TBorderWidth Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	
public:
	__fastcall virtual TRzBorder(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzBorder();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Controls::TBevelWidth BevelWidth = {read=FBevelWidth, write=SetBevelWidth, default=1};
	__property Vcl::Controls::TBorderWidth BorderWidth = {read=FBorderWidth, write=SetBorderWidth, default=0};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property System::Uitypes::TColor BorderHighlight = {read=FBorderHighlight, write=SetBorderHighlight, default=-16777196};
	__property System::Uitypes::TColor BorderShadow = {read=FBorderShadow, write=SetBorderShadow, default=-16777200};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=2};
	__property Rzcommon::TSides BorderSides = {read=FBorderSides, write=SetBorderSides, default=15};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Constraints;
	__property DragKind = {default=0};
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property ParentShowHint = {default=1};
	__property ShowHint;
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
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


class PASCALIMPLEMENTATION TRzLEDDisplay : public TRzBorder
{
	typedef TRzBorder inherited;
	
private:
	System::Classes::TAlignment FAlignment;
	Vcl::Controls::TCaption FCaption;
	int FDotSize;
	int FDotGap;
	System::Uitypes::TColor FBackColor;
	System::Uitypes::TColor FSegOnColor;
	System::Uitypes::TColor FSegOffColor;
	Rzcommon::TRzScrollType FScrollType;
	bool FScrolling;
	int FSteps;
	System::Word FScrollDelay;
	Vcl::Extctrls::TTimer* FTimer;
	int FCurrentStep;
	Rzcommon::TRzScrollDisplayEvent FOnScrollDisplay;
	System::Classes::TNotifyEvent FOnScrollComplete;
	void __fastcall TimerEventHandler(System::TObject* Sender);
	MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	void __fastcall UpdateDotDimensions();
	void __fastcall UpdateSteps();
	virtual System::Uitypes::TColor __fastcall InteriorColor();
	virtual void __fastcall Paint();
	virtual void __fastcall DrawCell(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::WideChar Symbol);
	System::Uitypes::TColor __fastcall DotColor(System::WideChar Symbol, int Col, int Row);
	DYNAMIC void __fastcall ScrollDisplay(int CurrentStep, int TotalSteps);
	DYNAMIC void __fastcall ScrollComplete();
	void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetBevelWidth(Vcl::Controls::TBevelWidth Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderWidth(Vcl::Controls::TBorderWidth Value);
	virtual void __fastcall SetBackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSegOnColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSegOffColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	void __fastcall SetScrollType(Rzcommon::TRzScrollType Value);
	virtual void __fastcall SetScrolling(bool Value);
	void __fastcall SetScrollDelay(System::Word Value);
	
public:
	__fastcall virtual TRzLEDDisplay(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzLEDDisplay();
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	
__published:
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=SetAlignment, default=0};
	__property Vcl::Controls::TCaption Caption = {read=FCaption, write=SetCaption};
	__property System::Uitypes::TColor BackColor = {read=FBackColor, write=SetBackColor, default=0};
	__property System::Uitypes::TColor SegOnColor = {read=FSegOnColor, write=SetSegOnColor, default=65280};
	__property System::Uitypes::TColor SegOffColor = {read=FSegOffColor, write=SetSegOffColor, default=13056};
	__property Rzcommon::TRzScrollType ScrollType = {read=FScrollType, write=SetScrollType, default=1};
	__property bool Scrolling = {read=FScrolling, write=SetScrolling, default=0};
	__property System::Word ScrollDelay = {read=FScrollDelay, write=SetScrollDelay, default=100};
	__property Rzcommon::TRzScrollDisplayEvent OnScrollDisplay = {read=FOnScrollDisplay, write=FOnScrollDisplay};
	__property System::Classes::TNotifyEvent OnScrollComplete = {read=FOnScrollComplete, write=FOnScrollComplete};
	__property Align = {default=0};
	__property BorderOuter = {default=8};
	__property Enabled = {default=1};
	__property Height = {default=30};
	__property ShowHint;
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property Visible = {default=1};
	__property Width = {default=100};
	__property OnClick;
	__property OnDblClick;
};


class PASCALIMPLEMENTATION TRzProgressDisplay : public TRzBorder
{
	typedef TRzBorder inherited;
	
	
private:
	typedef System::DynamicArray<System::UnicodeString> _TRzProgressDisplay__1;
	
	
private:
	_TRzProgressDisplay__1 FLines;
	int FNextLine;
	int FTopLine;
	bool FFirstPage;
	int FFontHeight;
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	System::UnicodeString __fastcall GetLines();
	void __fastcall CopyLines(System::UnicodeString *A, const int A_High);
	virtual System::Uitypes::TColor __fastcall InteriorColor();
	virtual void __fastcall Loaded();
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall Resize();
	void __fastcall UpdateNumberOfLines();
	
public:
	__fastcall virtual TRzProgressDisplay(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzProgressDisplay();
	void __fastcall AddStep(const System::UnicodeString Step);
	void __fastcall Clear();
	
__published:
	__property BorderOuter = {default=8};
	__property Color = {default=-16777211};
	__property Font;
	__property Height = {default=50};
	__property ParentColor = {default=1};
	__property ParentFont = {default=1};
	__property Width = {default=200};
};


enum DECLSPEC_DENUM TRzMeterType : unsigned char { mtNormal, mtBar3D, mtLED };

enum DECLSPEC_DENUM TRzMeterShape : unsigned char { msRectangular, msTrapezoidal };

class PASCALIMPLEMENTATION TRzMeter : public TRzBorder
{
	typedef TRzBorder inherited;
	
private:
	int FMin;
	int FMax;
	int FValue;
	System::Byte FBWidth;
	System::Uitypes::TColor FBarColor;
	System::Uitypes::TColor FBackColor;
	TRzMeterType FMeterType;
	TRzMeterShape FMeterShape;
	Rzgrafx::TTrapDirect FTrapDirect;
	Rzgrafx::TTrapShape FTrapShape;
	int FTrapMin;
	Rzcommon::TDirection FDirection;
	Vcl::Graphics::TBitmap* FBitmap;
	Vcl::Graphics::TBrushStyle FBrushStyle;
	bool FShowPercent;
	System::Byte FSegSpacing;
	System::Uitypes::TColor FSegOffColor;
	System::StaticArray<Rzcommon::TPositiveByte, 3> FSegCount;
	System::StaticArray<System::Uitypes::TColor, 3> FSegColor;
	void __fastcall BitmapChangedHandler(System::TObject* Sender);
	
protected:
	virtual System::Uitypes::TColor __fastcall InteriorColor();
	virtual void __fastcall Paint();
	void __fastcall DrawStandardBar(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds);
	void __fastcall DrawLEDBar(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds);
	virtual void __fastcall SetMin(int Value);
	virtual void __fastcall SetMax(int Value);
	virtual void __fastcall SetValue(int Value);
	virtual void __fastcall SetBarColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetMeterType(TRzMeterType Value);
	virtual void __fastcall SetMeterShape(TRzMeterShape Value);
	virtual void __fastcall SetTrapDirect(Rzgrafx::TTrapDirect Value);
	virtual void __fastcall SetTrapShape(Rzgrafx::TTrapShape Value);
	virtual void __fastcall SetTrapMin(int Value);
	virtual void __fastcall SetDirection(Rzcommon::TDirection Value);
	virtual void __fastcall SetBitmap(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetBrush(Vcl::Graphics::TBrushStyle Value);
	virtual void __fastcall SetShowPercent(bool Value);
	virtual int __fastcall GetPercent();
	virtual void __fastcall SetSegSpacing(System::Byte Value);
	virtual void __fastcall SetSegOffColor(System::Uitypes::TColor Value);
	virtual Rzcommon::TPositiveByte __fastcall GetSegCount(int Index);
	virtual void __fastcall SetSegCount(int Index, Rzcommon::TPositiveByte Value);
	virtual System::Uitypes::TColor __fastcall GetSegColor(int Index);
	virtual void __fastcall SetSegColor(int Index, System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzMeter(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzMeter();
	void __fastcall AddValue(int Number);
	__property int Percent = {read=GetPercent, nodefault};
	
__published:
	__property Rzcommon::TDirection Direction = {read=FDirection, write=SetDirection, default=3};
	__property Vcl::Graphics::TBitmap* Bitmap = {read=FBitmap, write=SetBitmap};
	__property Vcl::Graphics::TBrushStyle BrushStyle = {read=FBrushStyle, write=SetBrush, nodefault};
	__property int Min = {read=FMin, write=SetMin, default=0};
	__property int Max = {read=FMax, write=SetMax, default=100};
	__property int Value = {read=FValue, write=SetValue, default=0};
	__property System::Uitypes::TColor BackColor = {read=FBackColor, write=SetBackColor, default=0};
	__property System::Uitypes::TColor BarColor = {read=FBarColor, write=SetBarColor, default=255};
	__property TRzMeterType MeterType = {read=FMeterType, write=SetMeterType, default=2};
	__property TRzMeterShape MeterShape = {read=FMeterShape, write=SetMeterShape, default=0};
	__property bool ShowPercent = {read=FShowPercent, write=SetShowPercent, default=1};
	__property System::Byte SegSpacing = {read=FSegSpacing, write=SetSegSpacing, default=2};
	__property System::Uitypes::TColor SegOffColor = {read=FSegOffColor, write=SetSegOffColor, default=0};
	__property Rzcommon::TPositiveByte Seg1Count = {read=GetSegCount, write=SetSegCount, index=0, default=5};
	__property System::Uitypes::TColor Seg1Color = {read=GetSegColor, write=SetSegColor, index=0, default=32768};
	__property Rzcommon::TPositiveByte Seg2Count = {read=GetSegCount, write=SetSegCount, index=1, default=3};
	__property System::Uitypes::TColor Seg2Color = {read=GetSegColor, write=SetSegColor, index=1, default=65535};
	__property Rzcommon::TPositiveByte Seg3Count = {read=GetSegCount, write=SetSegCount, index=2, default=2};
	__property System::Uitypes::TColor Seg3Color = {read=GetSegColor, write=SetSegColor, index=2, default=255};
	__property Rzgrafx::TTrapDirect TrapezoidDir = {read=FTrapDirect, write=SetTrapDirect, default=0};
	__property int TrapezoidMin = {read=FTrapMin, write=SetTrapMin, default=6};
	__property Rzgrafx::TTrapShape TrapezoidShape = {read=FTrapShape, write=SetTrapShape, default=1};
	__property Align = {default=0};
	__property BorderOuter = {default=8};
	__property Color = {default=-16777211};
	__property Font;
	__property Height = {default=30};
	__property ParentColor = {default=1};
	__property ParentFont = {default=1};
	__property ShowHint;
	__property ParentShowHint = {default=1};
	__property Width = {default=150};
	__property OnClick;
	__property OnDblClick;
};


class PASCALIMPLEMENTATION TRzColorPicker : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	System::Uitypes::TColor FCustomColor;
	System::Uitypes::TColor FDefaultColor;
	Vcl::Dialogs::TColorDialogOptions FColorDlgOptions;
	Rzcommon::TRzCustomColors* FCustomColors;
	System::UnicodeString FCustomColorCaption;
	System::UnicodeString FNoColorCaption;
	System::UnicodeString FDefaultColorCaption;
	bool FShowCustomColor;
	bool FShowNoColor;
	bool FShowDefaultColor;
	bool FShowSystemColors;
	int FSelColorIndex;
	int FHighlightColorIndex;
	System::Uitypes::TColor FThemeBorderColor;
	System::Classes::TNotifyEvent FOnChange;
	bool FShowColorHints;
	Vcl::Controls::THintWindow* FHintWnd;
	System::Uitypes::TColor FButtonColor;
	System::Uitypes::TColor FButtonFontColor;
	bool FIsPopup;
	int __fastcall Margin();
	HIDESBASE void __fastcall UpdateBounds();
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Paint();
	virtual bool __fastcall CanAutoSize(int &NewWidth, int &NewHeight);
	int __fastcall HitTest(int X, int Y);
	System::Types::TRect __fastcall GetCellRect(int Index);
	System::Types::TRect __fastcall CalcHintRect(int MaxWidth, const System::UnicodeString HintStr, Vcl::Controls::THintWindow* HintWnd);
	void __fastcall DoHint(int X, int Y);
	void __fastcall ReleaseHintWindow();
	virtual void __fastcall PickCustomColor();
	DYNAMIC void __fastcall ColorChanged();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	virtual void __fastcall SetButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCustomColorCaption(const System::UnicodeString Value);
	virtual void __fastcall SetCustomColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCustomColors(Rzcommon::TRzCustomColors* Value);
	virtual void __fastcall SetDefaultColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetNoColorCaption(const System::UnicodeString Value);
	virtual void __fastcall SetDefaultColorCaption(const System::UnicodeString Value);
	virtual void __fastcall SetShowCustomColor(bool Value);
	virtual void __fastcall SetShowNoColor(bool Value);
	virtual void __fastcall SetShowDefaultColor(bool Value);
	virtual void __fastcall SetShowSystemColors(bool Value);
	virtual void __fastcall SetSelColorIndex(int Value);
	virtual void __fastcall SetHighlightColorIndex(int Value);
	virtual System::Uitypes::TColor __fastcall GetSelectedColor();
	virtual void __fastcall SetSelectedColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzColorPicker(System::Classes::TComponent* AOwner);
	__property bool IsPopup = {write=FIsPopup, nodefault};
	__property int SelColorIndex = {read=FSelColorIndex, write=SetSelColorIndex, nodefault};
	
__published:
	__property System::Uitypes::TColor ButtonColor = {read=FButtonColor, write=SetButtonColor, default=-16777201};
	__property System::Uitypes::TColor ButtonFontColor = {read=FButtonFontColor, write=SetButtonFontColor, default=-16777208};
	__property System::UnicodeString CustomColorCaption = {read=FCustomColorCaption, write=SetCustomColorCaption};
	__property System::Uitypes::TColor CustomColor = {read=FCustomColor, write=SetCustomColor, default=16777215};
	__property Rzcommon::TRzCustomColors* CustomColors = {read=FCustomColors, write=SetCustomColors};
	__property Vcl::Dialogs::TColorDialogOptions ColorDlgOptions = {read=FColorDlgOptions, write=FColorDlgOptions, default=1};
	__property System::Uitypes::TColor DefaultColor = {read=FDefaultColor, write=SetDefaultColor, default=-16777203};
	__property System::UnicodeString DefaultColorCaption = {read=FDefaultColorCaption, write=SetDefaultColorCaption};
	__property bool ShowColorHints = {read=FShowColorHints, write=FShowColorHints, default=1};
	__property System::UnicodeString NoColorCaption = {read=FNoColorCaption, write=SetNoColorCaption};
	__property System::Uitypes::TColor SelectedColor = {read=GetSelectedColor, write=SetSelectedColor, default=536870911};
	__property bool ShowNoColor = {read=FShowNoColor, write=SetShowNoColor, default=0};
	__property bool ShowCustomColor = {read=FShowCustomColor, write=SetShowCustomColor, default=0};
	__property bool ShowDefaultColor = {read=FShowDefaultColor, write=SetShowDefaultColor, default=0};
	__property bool ShowSystemColors = {read=FShowSystemColors, write=SetShowSystemColors, default=0};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BevelWidth = {default=1};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=8};
	__property BorderSides = {default=15};
	__property BorderColor = {default=-16777211};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=2};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
	__property Font;
	__property FrameController;
	__property FrameControllerNotifications = {default=65535};
	__property Hint = {default=0};
	__property ParentColor = {default=0};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEndDock;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
public:
	/* TRzCustomPanel.Destroy */ inline __fastcall virtual ~TRzColorPicker() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzColorPicker(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::ResourceString _sRzColorBlack;
#define Rzborder_sRzColorBlack System::LoadResourceString(&Rzborder::_sRzColorBlack)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBrown;
#define Rzborder_sRzColorBrown System::LoadResourceString(&Rzborder::_sRzColorBrown)
extern DELPHI_PACKAGE System::ResourceString _sRzColorOliveGreen;
#define Rzborder_sRzColorOliveGreen System::LoadResourceString(&Rzborder::_sRzColorOliveGreen)
extern DELPHI_PACKAGE System::ResourceString _sRzColorDarkGreen;
#define Rzborder_sRzColorDarkGreen System::LoadResourceString(&Rzborder::_sRzColorDarkGreen)
extern DELPHI_PACKAGE System::ResourceString _sRzColorDarkTeal;
#define Rzborder_sRzColorDarkTeal System::LoadResourceString(&Rzborder::_sRzColorDarkTeal)
extern DELPHI_PACKAGE System::ResourceString _sRzColorDarkBlue;
#define Rzborder_sRzColorDarkBlue System::LoadResourceString(&Rzborder::_sRzColorDarkBlue)
extern DELPHI_PACKAGE System::ResourceString _sRzColorIndigo;
#define Rzborder_sRzColorIndigo System::LoadResourceString(&Rzborder::_sRzColorIndigo)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGray80;
#define Rzborder_sRzColorGray80 System::LoadResourceString(&Rzborder::_sRzColorGray80)
extern DELPHI_PACKAGE System::ResourceString _sRzColorDarkRed;
#define Rzborder_sRzColorDarkRed System::LoadResourceString(&Rzborder::_sRzColorDarkRed)
extern DELPHI_PACKAGE System::ResourceString _sRzColorOrange;
#define Rzborder_sRzColorOrange System::LoadResourceString(&Rzborder::_sRzColorOrange)
extern DELPHI_PACKAGE System::ResourceString _sRzColorDarkYellow;
#define Rzborder_sRzColorDarkYellow System::LoadResourceString(&Rzborder::_sRzColorDarkYellow)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGreen;
#define Rzborder_sRzColorGreen System::LoadResourceString(&Rzborder::_sRzColorGreen)
extern DELPHI_PACKAGE System::ResourceString _sRzColorTeal;
#define Rzborder_sRzColorTeal System::LoadResourceString(&Rzborder::_sRzColorTeal)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBlue;
#define Rzborder_sRzColorBlue System::LoadResourceString(&Rzborder::_sRzColorBlue)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBlueGray;
#define Rzborder_sRzColorBlueGray System::LoadResourceString(&Rzborder::_sRzColorBlueGray)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGray50;
#define Rzborder_sRzColorGray50 System::LoadResourceString(&Rzborder::_sRzColorGray50)
extern DELPHI_PACKAGE System::ResourceString _sRzColorRed;
#define Rzborder_sRzColorRed System::LoadResourceString(&Rzborder::_sRzColorRed)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLightOrange;
#define Rzborder_sRzColorLightOrange System::LoadResourceString(&Rzborder::_sRzColorLightOrange)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLime;
#define Rzborder_sRzColorLime System::LoadResourceString(&Rzborder::_sRzColorLime)
extern DELPHI_PACKAGE System::ResourceString _sRzColorSeaGreen;
#define Rzborder_sRzColorSeaGreen System::LoadResourceString(&Rzborder::_sRzColorSeaGreen)
extern DELPHI_PACKAGE System::ResourceString _sRzColorAqua;
#define Rzborder_sRzColorAqua System::LoadResourceString(&Rzborder::_sRzColorAqua)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLightBlue;
#define Rzborder_sRzColorLightBlue System::LoadResourceString(&Rzborder::_sRzColorLightBlue)
extern DELPHI_PACKAGE System::ResourceString _sRzColorViolet;
#define Rzborder_sRzColorViolet System::LoadResourceString(&Rzborder::_sRzColorViolet)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGray40;
#define Rzborder_sRzColorGray40 System::LoadResourceString(&Rzborder::_sRzColorGray40)
extern DELPHI_PACKAGE System::ResourceString _sRzColorPink;
#define Rzborder_sRzColorPink System::LoadResourceString(&Rzborder::_sRzColorPink)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGold;
#define Rzborder_sRzColorGold System::LoadResourceString(&Rzborder::_sRzColorGold)
extern DELPHI_PACKAGE System::ResourceString _sRzColorYellow;
#define Rzborder_sRzColorYellow System::LoadResourceString(&Rzborder::_sRzColorYellow)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBrightGreen;
#define Rzborder_sRzColorBrightGreen System::LoadResourceString(&Rzborder::_sRzColorBrightGreen)
extern DELPHI_PACKAGE System::ResourceString _sRzColorTurquoise;
#define Rzborder_sRzColorTurquoise System::LoadResourceString(&Rzborder::_sRzColorTurquoise)
extern DELPHI_PACKAGE System::ResourceString _sRzColorSkyBlue;
#define Rzborder_sRzColorSkyBlue System::LoadResourceString(&Rzborder::_sRzColorSkyBlue)
extern DELPHI_PACKAGE System::ResourceString _sRzColorPlum;
#define Rzborder_sRzColorPlum System::LoadResourceString(&Rzborder::_sRzColorPlum)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGray25;
#define Rzborder_sRzColorGray25 System::LoadResourceString(&Rzborder::_sRzColorGray25)
extern DELPHI_PACKAGE System::ResourceString _sRzColorRose;
#define Rzborder_sRzColorRose System::LoadResourceString(&Rzborder::_sRzColorRose)
extern DELPHI_PACKAGE System::ResourceString _sRzColorTan;
#define Rzborder_sRzColorTan System::LoadResourceString(&Rzborder::_sRzColorTan)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLightYellow;
#define Rzborder_sRzColorLightYellow System::LoadResourceString(&Rzborder::_sRzColorLightYellow)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLightGreen;
#define Rzborder_sRzColorLightGreen System::LoadResourceString(&Rzborder::_sRzColorLightGreen)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLightTurquoise;
#define Rzborder_sRzColorLightTurquoise System::LoadResourceString(&Rzborder::_sRzColorLightTurquoise)
extern DELPHI_PACKAGE System::ResourceString _sRzColorPaleBlue;
#define Rzborder_sRzColorPaleBlue System::LoadResourceString(&Rzborder::_sRzColorPaleBlue)
extern DELPHI_PACKAGE System::ResourceString _sRzColorLavender;
#define Rzborder_sRzColorLavender System::LoadResourceString(&Rzborder::_sRzColorLavender)
extern DELPHI_PACKAGE System::ResourceString _sRzColorWhite;
#define Rzborder_sRzColorWhite System::LoadResourceString(&Rzborder::_sRzColorWhite)
extern DELPHI_PACKAGE System::ResourceString _sRzColorScrollBar;
#define Rzborder_sRzColorScrollBar System::LoadResourceString(&Rzborder::_sRzColorScrollBar)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBackground;
#define Rzborder_sRzColorBackground System::LoadResourceString(&Rzborder::_sRzColorBackground)
extern DELPHI_PACKAGE System::ResourceString _sRzColorActiveCaption;
#define Rzborder_sRzColorActiveCaption System::LoadResourceString(&Rzborder::_sRzColorActiveCaption)
extern DELPHI_PACKAGE System::ResourceString _sRzColorInactiveCaption;
#define Rzborder_sRzColorInactiveCaption System::LoadResourceString(&Rzborder::_sRzColorInactiveCaption)
extern DELPHI_PACKAGE System::ResourceString _sRzColorMenu;
#define Rzborder_sRzColorMenu System::LoadResourceString(&Rzborder::_sRzColorMenu)
extern DELPHI_PACKAGE System::ResourceString _sRzColorWindow;
#define Rzborder_sRzColorWindow System::LoadResourceString(&Rzborder::_sRzColorWindow)
extern DELPHI_PACKAGE System::ResourceString _sRzColorWindowFrame;
#define Rzborder_sRzColorWindowFrame System::LoadResourceString(&Rzborder::_sRzColorWindowFrame)
extern DELPHI_PACKAGE System::ResourceString _sRzColorMenuText;
#define Rzborder_sRzColorMenuText System::LoadResourceString(&Rzborder::_sRzColorMenuText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorWindowText;
#define Rzborder_sRzColorWindowText System::LoadResourceString(&Rzborder::_sRzColorWindowText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorCaptionText;
#define Rzborder_sRzColorCaptionText System::LoadResourceString(&Rzborder::_sRzColorCaptionText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorActiveBorder;
#define Rzborder_sRzColorActiveBorder System::LoadResourceString(&Rzborder::_sRzColorActiveBorder)
extern DELPHI_PACKAGE System::ResourceString _sRzColorInactiveBorder;
#define Rzborder_sRzColorInactiveBorder System::LoadResourceString(&Rzborder::_sRzColorInactiveBorder)
extern DELPHI_PACKAGE System::ResourceString _sRzColorAppWorkSpace;
#define Rzborder_sRzColorAppWorkSpace System::LoadResourceString(&Rzborder::_sRzColorAppWorkSpace)
extern DELPHI_PACKAGE System::ResourceString _sRzColorHighlight;
#define Rzborder_sRzColorHighlight System::LoadResourceString(&Rzborder::_sRzColorHighlight)
extern DELPHI_PACKAGE System::ResourceString _sRzColorHighlightText;
#define Rzborder_sRzColorHighlightText System::LoadResourceString(&Rzborder::_sRzColorHighlightText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBtnFace;
#define Rzborder_sRzColorBtnFace System::LoadResourceString(&Rzborder::_sRzColorBtnFace)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBtnShadow;
#define Rzborder_sRzColorBtnShadow System::LoadResourceString(&Rzborder::_sRzColorBtnShadow)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGrayText;
#define Rzborder_sRzColorGrayText System::LoadResourceString(&Rzborder::_sRzColorGrayText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBtnText;
#define Rzborder_sRzColorBtnText System::LoadResourceString(&Rzborder::_sRzColorBtnText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorInactiveCaptionText;
#define Rzborder_sRzColorInactiveCaptionText System::LoadResourceString(&Rzborder::_sRzColorInactiveCaptionText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorBtnHighlight;
#define Rzborder_sRzColorBtnHighlight System::LoadResourceString(&Rzborder::_sRzColorBtnHighlight)
extern DELPHI_PACKAGE System::ResourceString _sRzColor3DDkShadow;
#define Rzborder_sRzColor3DDkShadow System::LoadResourceString(&Rzborder::_sRzColor3DDkShadow)
extern DELPHI_PACKAGE System::ResourceString _sRzColor3DLight;
#define Rzborder_sRzColor3DLight System::LoadResourceString(&Rzborder::_sRzColor3DLight)
extern DELPHI_PACKAGE System::ResourceString _sRzColorInfoText;
#define Rzborder_sRzColorInfoText System::LoadResourceString(&Rzborder::_sRzColorInfoText)
extern DELPHI_PACKAGE System::ResourceString _sRzColorInfoBk;
#define Rzborder_sRzColorInfoBk System::LoadResourceString(&Rzborder::_sRzColorInfoBk)
extern DELPHI_PACKAGE System::ResourceString _sRzColorHotLight;
#define Rzborder_sRzColorHotLight System::LoadResourceString(&Rzborder::_sRzColorHotLight)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGradientActiveCaption;
#define Rzborder_sRzColorGradientActiveCaption System::LoadResourceString(&Rzborder::_sRzColorGradientActiveCaption)
extern DELPHI_PACKAGE System::ResourceString _sRzColorGradientInactiveCaption;
#define Rzborder_sRzColorGradientInactiveCaption System::LoadResourceString(&Rzborder::_sRzColorGradientInactiveCaption)
extern DELPHI_PACKAGE System::ResourceString _sRzColorMenuHighlight;
#define Rzborder_sRzColorMenuHighlight System::LoadResourceString(&Rzborder::_sRzColorMenuHighlight)
extern DELPHI_PACKAGE System::ResourceString _sRzColorMenuBar;
#define Rzborder_sRzColorMenuBar System::LoadResourceString(&Rzborder::_sRzColorMenuBar)
extern DELPHI_PACKAGE System::ResourceString _sRzDefaultColorCaption;
#define Rzborder_sRzDefaultColorCaption System::LoadResourceString(&Rzborder::_sRzDefaultColorCaption)
extern DELPHI_PACKAGE System::ResourceString _sRzNoColorCaption;
#define Rzborder_sRzNoColorCaption System::LoadResourceString(&Rzborder::_sRzNoColorCaption)
extern DELPHI_PACKAGE System::ResourceString _sRzCustomColorCaption;
#define Rzborder_sRzCustomColorCaption System::LoadResourceString(&Rzborder::_sRzCustomColorCaption)
}	/* namespace Rzborder */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBORDER)
using namespace Rzborder;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzborderHPP
