// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzPanel.pas' rev: 35.00 (Windows)

#ifndef RzpanelHPP
#define RzpanelHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.ImgList.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <RzGrafx.hpp>
#include <RzCommon.hpp>
#include <RzButton.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzpanel
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzPanelDockManager;
class DELPHICLASS TRzCustomPanel;
class DELPHICLASS TRzPanel;
class DELPHICLASS TRzCustomGroupBox;
class DELPHICLASS TRzGroupBox;
class DELPHICLASS TRzSpacer;
class DELPHICLASS TRzToolbarControl;
class DELPHICLASS TRzToolbarControlList;
class DELPHICLASS TRzCustomizeCaptions;
class DELPHICLASS TRzToolbarPopupButton;
class DELPHICLASS TRzToolbar;
class DELPHICLASS TRzStatusBar;
class DELPHICLASS TRzFlowPanel;
class DELPHICLASS TRzGridPanel;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzPanelDockManager : public Vcl::Controls::TDockTree
{
	typedef Vcl::Controls::TDockTree inherited;
	
private:
	TRzCustomPanel* FPanel;
	System::Classes::TWndMethod FOldWndProc;
	void __fastcall WindowProcHook(Winapi::Messages::TMessage &Msg);
	
protected:
	Vcl::Graphics::TFont* FFont;
	Vcl::Graphics::TFont* FCloseFont;
	int FGrabberSize;
	virtual void __fastcall AdjustDockRect(Vcl::Controls::TControl* Control, System::Types::TRect &ARect);
	void __fastcall DrawVertTitle(Vcl::Graphics::TCanvas* Canvas, const System::UnicodeString Caption, const System::Types::TRect &Bounds);
	virtual void __fastcall PaintDockFrame(Vcl::Graphics::TCanvas* Canvas, Vcl::Controls::TControl* Control, const System::Types::TRect &ARect);
	
public:
	__fastcall virtual TRzPanelDockManager(Vcl::Controls::TWinControl* DockSite);
	__fastcall virtual ~TRzPanelDockManager();
	virtual void __fastcall PaintSite(HDC DC);
};


enum DECLSPEC_DENUM TRzGridStyle : unsigned char { gsDots, gsDottedLines, gsSolidLines };

class PASCALIMPLEMENTATION TRzCustomPanel : public Vcl::Extctrls::TCustomPanel
{
	typedef Vcl::Extctrls::TCustomPanel inherited;
	
private:
	bool FInAlignControls;
	Rzcommon::TAlignmentVertical FAlignmentVertical;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Rzcommon::TSides FBorderSides;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FBorderHighlight;
	System::Uitypes::TColor FBorderShadow;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	bool FPaintClientArea;
	System::Uitypes::TColor FPanelColor;
	Rzcommon::TRzVisualStyle FVisualStyle;
	Rzcommon::TRzGradientColorStyle FGradientColorStyle;
	System::Uitypes::TColor FGradientColorStart;
	System::Uitypes::TColor FGradientColorStop;
	Rzcommon::TGradientDirection FGradientDirection;
	System::Uitypes::TColor FGridColor;
	TRzGridStyle FGridStyle;
	System::Word FGridXSize;
	System::Word FGridYSize;
	bool FShowGrid;
	bool FTransparent;
	int FTextMargin;
	int FTextMarginVertical;
	bool FWordWrap;
	bool FShowDockClientCaptions;
	System::Classes::TStringList* FEnabledList;
	System::Classes::TNotifyEvent FOnPaint;
	void __fastcall ReadOldUseGradientsProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	MESSAGE void __fastcall WMThemeChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	void __fastcall UpdatePanelColor();
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Loaded();
	virtual System::Types::TPoint __fastcall CursorPosition();
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	virtual void __fastcall FixClientRect(System::Types::TRect &Rect, bool ShrinkByBorder);
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	virtual System::Types::TRect __fastcall GetClientRect();
	virtual System::Types::TRect __fastcall GetControlRect();
	virtual System::Types::TPoint __fastcall InvalidateMarginSize();
	virtual void __fastcall EnableChildControls(bool Enabled);
	virtual void __fastcall GetGradientColors(System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
	virtual void __fastcall GetGradientFrameColor(System::Uitypes::TColor &FrameColor, int &FrameColorAdjustment);
	virtual void __fastcall DrawCaption(const System::Types::TRect &Rect);
	virtual void __fastcall DrawGrid(const System::Types::TRect &Rect);
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	DYNAMIC Vcl::Controls::_di_IDockManager __fastcall CreateDockManager();
	virtual void __fastcall SetAlignmentVertical(Rzcommon::TAlignmentVertical Value);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual void __fastcall SetGradientColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetGridColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetGridStyle(TRzGridStyle Value);
	virtual void __fastcall SetGridXSize(System::Word Value);
	virtual void __fastcall SetGridYSize(System::Word Value);
	virtual void __fastcall SetPaintClientArea(bool Value);
	virtual void __fastcall SetShowGrid(bool Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetTextMargin(int Value);
	virtual void __fastcall SetTextMarginVertical(int Value);
	virtual void __fastcall SetWordWrap(bool Value);
	virtual void __fastcall SetVisualStyle(Rzcommon::TRzVisualStyle Value);
	__property Rzcommon::TAlignmentVertical AlignmentVertical = {read=FAlignmentVertical, write=SetAlignmentVertical, default=1};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=6};
	__property Rzcommon::TSides BorderSides = {read=FBorderSides, write=SetBorderSides, default=15};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property System::Uitypes::TColor BorderHighlight = {read=FBorderHighlight, write=SetBorderHighlight, default=-16777196};
	__property System::Uitypes::TColor BorderShadow = {read=FBorderShadow, write=SetBorderShadow, default=-16777200};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TRzGradientColorStyle GradientColorStyle = {read=FGradientColorStyle, write=SetGradientColorStyle, default=0};
	__property System::Uitypes::TColor GradientColorStart = {read=FGradientColorStart, write=SetGradientColorStart, default=16777215};
	__property System::Uitypes::TColor GradientColorStop = {read=FGradientColorStop, write=SetGradientColorStop, default=-16777201};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=2};
	__property System::Uitypes::TColor GridColor = {read=FGridColor, write=SetGridColor, default=-16777200};
	__property TRzGridStyle GridStyle = {read=FGridStyle, write=SetGridStyle, default=0};
	__property System::Word GridXSize = {read=FGridXSize, write=SetGridXSize, default=8};
	__property System::Word GridYSize = {read=FGridYSize, write=SetGridYSize, default=8};
	__property bool PaintClientArea = {read=FPaintClientArea, write=SetPaintClientArea, default=1};
	__property int TextMargin = {read=FTextMargin, write=SetTextMargin, default=0};
	__property int TextMarginVertical = {read=FTextMarginVertical, write=SetTextMarginVertical, default=0};
	__property bool ShowGrid = {read=FShowGrid, write=SetShowGrid, default=0};
	__property bool ShowDockClientCaptions = {read=FShowDockClientCaptions, write=FShowDockClientCaptions, default=1};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property Rzcommon::TRzVisualStyle VisualStyle = {read=FVisualStyle, write=SetVisualStyle, default=1};
	__property bool WordWrap = {read=FWordWrap, write=SetWordWrap, default=1};
	__property System::Classes::TNotifyEvent OnPaint = {read=FOnPaint, write=FOnPaint};
	
public:
	__fastcall virtual TRzCustomPanel(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomPanel();
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	void __fastcall SetGridSize(int XSize, int YSize);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomPanel(HWND ParentWindow) : Vcl::Extctrls::TCustomPanel(ParentWindow) { }
	
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


class PASCALIMPLEMENTATION TRzPanel : public TRzCustomPanel
{
	typedef TRzCustomPanel inherited;
	
public:
	__property Canvas;
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Alignment = {default=2};
	__property AlignmentVertical = {default=1};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderInner = {default=0};
	__property BorderOuter = {default=6};
	__property BorderSides = {default=15};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=0};
	__property Caption = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DockSite = {default=0};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property FullRepaint = {default=1};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property GridColor = {default=-16777200};
	__property GridStyle = {default=0};
	__property GridXSize = {default=8};
	__property GridYSize = {default=8};
	__property Locked = {default=0};
	__property PaintClientArea = {default=1};
	__property TextMargin = {default=0};
	__property TextMarginVertical = {default=0};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowDockClientCaptions = {default=1};
	__property ShowGrid = {default=0};
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Transparent = {default=0};
	__property UseDockManager = {default=1};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property WordWrap = {default=1};
	__property OnCanResize;
	__property OnClick;
	__property OnConstrainedResize;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDockDrop;
	__property OnDockOver;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnGetSiteInfo;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnPaint;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnUnDock;
public:
	/* TRzCustomPanel.Create */ inline __fastcall virtual TRzPanel(System::Classes::TComponent* AOwner) : TRzCustomPanel(AOwner) { }
	/* TRzCustomPanel.Destroy */ inline __fastcall virtual ~TRzPanel() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPanel(HWND ParentWindow) : TRzCustomPanel(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzGroupBoxStyle : unsigned char { gsStandard, gsCustom, gsTopLine, gsFlat, gsBanner, gsUnderline };

class PASCALIMPLEMENTATION TRzCustomGroupBox : public TRzCustomPanel
{
	typedef TRzCustomPanel inherited;
	
private:
	TRzGroupBoxStyle FGroupStyle;
	int FBannerHeight;
	Vcl::Graphics::TFont* FCaptionFont;
	bool FCaptionFontChanged;
	System::Types::TRect FCaptionRect;
	bool FEnableControlsOnCheck;
	bool FShowCheckBox;
	int FCheckBoxSize;
	bool FDragging;
	bool FMouseOverCheckBox;
	bool FKeyToggle;
	bool FShowDownVersion;
	System::Classes::TNotifyEvent FOnCheckBoxClick;
	bool FChecked;
	void __fastcall CaptionFontChangeHandler(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall Paint();
	void __fastcall DrawThemedCheckBox();
	void __fastcall DrawNonThemedCheckBox();
	System::Types::TRect __fastcall AdjustCaptionRectForCheckBox();
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	bool __fastcall ShowAccel();
	bool __fastcall ShowFocus();
	virtual void __fastcall ChangeState();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall CheckBoxClick();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	virtual void __fastcall SetBannerHeight(int Value);
	virtual void __fastcall SetGroupBoxStyle(TRzGroupBoxStyle Value);
	bool __fastcall IsCaptionFontStored();
	virtual void __fastcall SetCaptionFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetChecked(bool Value);
	virtual void __fastcall SetEnableControlsOnCheck(bool Value);
	virtual void __fastcall SetShowCheckBox(bool Value);
	__property bool Checked = {read=FChecked, write=SetChecked, default=1};
	__property bool EnableControlsOnCheck = {read=FEnableControlsOnCheck, write=SetEnableControlsOnCheck, default=1};
	__property bool ShowCheckBox = {read=FShowCheckBox, write=SetShowCheckBox, default=0};
	__property System::Classes::TNotifyEvent OnCheckBoxClick = {read=FOnCheckBoxClick, write=FOnCheckBoxClick};
	__property Alignment = {default=0};
	__property AlignmentVertical = {default=0};
	__property BorderOuter = {default=0};
	__property BorderInner = {default=0};
	__property Height = {default=105};
	
public:
	__fastcall virtual TRzCustomGroupBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomGroupBox();
	__property int BannerHeight = {read=FBannerHeight, write=SetBannerHeight, default=0};
	__property TRzGroupBoxStyle GroupStyle = {read=FGroupStyle, write=SetGroupBoxStyle, default=3};
	__property Vcl::Graphics::TFont* CaptionFont = {read=FCaptionFont, write=SetCaptionFont, stored=IsCaptionFontStored};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomGroupBox(HWND ParentWindow) : TRzCustomPanel(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzGroupBox : public TRzCustomGroupBox
{
	typedef TRzCustomGroupBox inherited;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Alignment = {default=0};
	__property Anchors = {default=3};
	__property BannerHeight = {default=0};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderColor = {default=-16777201};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=0};
	__property BorderSides = {default=15};
	__property BorderWidth = {default=0};
	__property Caption = {default=0};
	__property CaptionFont;
	__property Checked = {default=1};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DockSite = {default=0};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property EnableControlsOnCheck = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property GroupStyle = {default=3};
	__property Height = {default=105};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowCheckBox = {default=0};
	__property ShowDockClientCaptions = {default=1};
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Transparent = {default=0};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property OnCheckBoxClick;
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDockDrop;
	__property OnDockOver;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnGetSiteInfo;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnPaint;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnUnDock;
public:
	/* TRzCustomGroupBox.Create */ inline __fastcall virtual TRzGroupBox(System::Classes::TComponent* AOwner) : TRzCustomGroupBox(AOwner) { }
	/* TRzCustomGroupBox.Destroy */ inline __fastcall virtual ~TRzGroupBox() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzGroupBox(HWND ParentWindow) : TRzCustomGroupBox(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzSpacer : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FGrooved;
	Rzcommon::TOrientation FOrientation;
	
protected:
	virtual void __fastcall Paint();
	virtual void __fastcall SetGrooved(bool Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	
public:
	__fastcall virtual TRzSpacer(System::Classes::TComponent* AOwner);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool Grooved = {read=FGrooved, write=SetGrooved, default=0};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property Constraints;
	__property Height = {default=25};
	__property Width = {default=8};
	__property Visible = {default=1};
public:
	/* TGraphicControl.Destroy */ inline __fastcall virtual ~TRzSpacer() { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzToolbarControl : public System::TObject
{
	typedef System::TObject inherited;
	
protected:
	Vcl::Controls::TControl* FControl;
	System::UnicodeString FControlName;
	virtual void __fastcall AssignElements(TRzToolbarControl* Control);
	
public:
	__property Vcl::Controls::TControl* Control = {read=FControl};
	__property System::UnicodeString ControlName = {read=FControlName};
public:
	/* TObject.Create */ inline __fastcall TRzToolbarControl() : System::TObject() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TRzToolbarControl() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzToolbarControlList : public System::Classes::TList
{
	typedef System::Classes::TList inherited;
	
public:
	TRzToolbarControl* operator[](int Index) { return this->Items[Index]; }
	
protected:
	TRzToolbar* FToolbar;
	int FIndexOfLastControlRead;
	TRzToolbarControlList* FTempControlList;
	HIDESBASE TRzToolbarControl* __fastcall Get(int Index);
	HIDESBASE void __fastcall Put(int Index, TRzToolbarControl* Value);
	void __fastcall ControlsAreLoaded(System::Classes::TComponent* OwnerComp);
	virtual void __fastcall ReadControl(System::Classes::TReader* Reader);
	virtual void __fastcall WriteControl(int Index, System::Classes::TWriter* Writer);
	virtual TRzToolbarControl* __fastcall CreateToolBarControl();
	
public:
	__fastcall TRzToolbarControlList(TRzToolbar* AToolbar);
	__fastcall virtual ~TRzToolbarControlList();
	void __fastcall ReadControls(System::Classes::TReader* Reader);
	void __fastcall WriteControls(System::Classes::TWriter* Writer);
	HIDESBASE int __fastcall IndexOf(Vcl::Controls::TControl* Control);
	int __fastcall IndexOfName(const System::UnicodeString ControlName);
	int __fastcall AddControl(Vcl::Controls::TControl* Control);
	int __fastcall AddControlName(const System::UnicodeString ControlName);
	int __fastcall RemoveControl(Vcl::Controls::TControl* Control);
	int __fastcall RemoveControlName(const System::UnicodeString ControlName);
	__property TRzToolbarControl* Items[int Index] = {read=Get, write=Put/*, default*/};
	__property TRzToolbar* Toolbar = {read=FToolbar};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCustomizeCaptions : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	bool FStoreCaptions;
	System::UnicodeString FTitle;
	System::UnicodeString FHint;
	System::UnicodeString FClose;
	System::UnicodeString FMoveUp;
	System::UnicodeString FMoveDown;
	System::UnicodeString FTextOptions;
	System::UnicodeString FNoTextLabels;
	System::UnicodeString FShowTextLabels;
	System::UnicodeString FSelectiveTextOnRight;
	
protected:
	System::UnicodeString __fastcall GetCaption(int Index);
	void __fastcall SetCaption(int Index, const System::UnicodeString Value);
	
public:
	__fastcall TRzCustomizeCaptions();
	
__published:
	__property System::UnicodeString Title = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=1};
	__property System::UnicodeString Hint = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=2};
	__property System::UnicodeString Close = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=3};
	__property System::UnicodeString MoveUp = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=4};
	__property System::UnicodeString MoveDown = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=5};
	__property System::UnicodeString TextOptions = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=6};
	__property System::UnicodeString NoTextLabels = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=7};
	__property System::UnicodeString ShowTextLabels = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=8};
	__property System::UnicodeString SelectiveTextOnRight = {read=GetCaption, write=SetCaption, stored=FStoreCaptions, index=9};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzCustomizeCaptions() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzToolbarPopupButton : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	System::Uitypes::TColor FFrameColor;
	TRzToolbar* FToolbar;
	Vcl::Menus::TPopupMenu* FMenu;
	Rzbutton::TRzToolButton* FSelectedButton;
	bool FPressed;
	bool FDown;
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	virtual void __fastcall DrawBackground(const System::Types::TRect &Bounds, bool Down);
	virtual void __fastcall DrawChevron(const System::Types::TRect &Bounds);
	virtual void __fastcall Paint();
	void __fastcall DisplayButtons();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzToolbarPopupButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzToolbarPopupButton();
	
__published:
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzToolbarPopupButton(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzToolbarTextOptions : unsigned char { ttoNoTextLabels, ttoShowTextLabels, ttoSelectiveTextOnRight, ttoCustom };

class PASCALIMPLEMENTATION TRzToolbar : public TRzCustomPanel
{
	typedef TRzCustomPanel inherited;
	
private:
	bool FAutoResize;
	bool FAutoStyle;
	Vcl::Graphics::TBitmap* FBackground;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	Vcl::Imglist::TCustomImageList* FDisabledImages;
	Vcl::Imglist::TChangeLink* FDisabledImagesChangeLink;
	Rzcommon::TOrientation FOrientation;
	int FRowHeight;
	bool FShowDivider;
	bool FShowButtonCaptions;
	Vcl::Buttons::TButtonLayout FButtonLayout;
	int FButtonWidth;
	int FButtonHeight;
	TRzToolbarTextOptions FTextOptions;
	bool FUpdatingTextOptions;
	bool FWrapControls;
	TRzToolbarPopupButton* FToolbarPopupButton;
	bool FShowToolbarPopupButton;
	TRzCustomizeCaptions* FCustomizeCaptions;
	Rzcommon::TRzRegIniFile* FRegIniFile;
	System::Classes::TNotifyEvent FOnVisibleChanged;
	void __fastcall BackgroundChangedHandler(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	void __fastcall DisabledImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMVisibleChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	TRzToolbarControlList* FToolbarControls;
	int FMargin;
	int FTopMargin;
	int FCalculatedRowHeight;
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall DrawCaption(const System::Types::TRect &Rect);
	virtual void __fastcall Paint();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	void __fastcall CreateToolbarPopupButton();
	void __fastcall AlignToolbarPopupButton();
	void __fastcall SetShowToolbarPopupButton(bool Value);
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	virtual int __fastcall CalculateRowHeight(int Row);
	virtual System::Types::TPoint __fastcall InvalidateMarginSize();
	virtual void __fastcall PositionControl(int Index, int Row, int &Offset);
	virtual TRzToolbarControlList* __fastcall CreateToolbarControlList();
	virtual void __fastcall AdjustStyle(Vcl::Controls::TAlign Value);
	virtual bool __fastcall CanAutoSize(int &NewWidth, int &NewHeight);
	DYNAMIC void __fastcall VisibleChanged();
	virtual Vcl::Controls::TAlign __fastcall GetAlign();
	HIDESBASE virtual void __fastcall SetAlign(Vcl::Controls::TAlign Value);
	virtual void __fastcall SetAutoResize(bool Value);
	virtual void __fastcall SetBackground(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetDisabledImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetMargin(int Value);
	virtual void __fastcall SetTopMargin(int Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetRowHeight(int Value);
	virtual void __fastcall SetShowButtonCaptions(bool Value);
	virtual void __fastcall SetButtonLayout(Vcl::Buttons::TButtonLayout Value);
	virtual void __fastcall SetButtonWidth(int Value);
	virtual void __fastcall SetButtonHeight(int Value);
	virtual void __fastcall SetShowDivider(bool Value);
	virtual void __fastcall SetTextOptions(TRzToolbarTextOptions Value);
	virtual void __fastcall SetWrapControls(bool Value);
	virtual void __fastcall SetCustomizeCaptions(TRzCustomizeCaptions* Value);
	virtual void __fastcall SetRegIniFile(Rzcommon::TRzRegIniFile* Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall CheckAutoResize(bool &Value);
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* AParent);
	virtual void __fastcall SetBiDiMode(System::Classes::TBiDiMode Value);
	virtual void __fastcall SetGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual void __fastcall SetVisualStyle(Rzcommon::TRzVisualStyle Value);
	
public:
	__fastcall virtual TRzToolbar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzToolbar();
	virtual void __fastcall PositionControls();
	virtual void __fastcall UpdateButtonSize(int NewWidth, int NewHeight, bool ShowCaptions);
	void __fastcall Customize(bool ShowTextOptions = true);
	void __fastcall RestoreLayout();
	void __fastcall SaveLayout();
	__property TRzToolbarControlList* ToolbarControls = {read=FToolbarControls};
	__property Canvas;
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Controls::TAlign Align = {read=GetAlign, write=SetAlign, default=1};
	__property bool AutoResize = {read=FAutoResize, write=SetAutoResize, default=1};
	__property bool AutoStyle = {read=FAutoStyle, write=FAutoStyle, default=1};
	__property Vcl::Graphics::TBitmap* Background = {read=FBackground, write=SetBackground};
	__property TRzCustomizeCaptions* CustomizeCaptions = {read=FCustomizeCaptions, write=SetCustomizeCaptions};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property Vcl::Imglist::TCustomImageList* DisabledImages = {read=FDisabledImages, write=SetDisabledImages};
	__property int Margin = {read=FMargin, write=SetMargin, default=4};
	__property int TopMargin = {read=FTopMargin, write=SetTopMargin, default=2};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property int RowHeight = {read=FRowHeight, write=SetRowHeight, default=25};
	__property Vcl::Buttons::TButtonLayout ButtonLayout = {read=FButtonLayout, write=SetButtonLayout, default=0};
	__property int ButtonWidth = {read=FButtonWidth, write=SetButtonWidth, default=25};
	__property int ButtonHeight = {read=FButtonHeight, write=SetButtonHeight, default=25};
	__property Rzcommon::TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
	__property bool ShowButtonCaptions = {read=FShowButtonCaptions, write=SetShowButtonCaptions, default=0};
	__property bool ShowDivider = {read=FShowDivider, write=SetShowDivider, default=1};
	__property TRzToolbarTextOptions TextOptions = {read=FTextOptions, write=SetTextOptions, default=0};
	__property bool WrapControls = {read=FWrapControls, write=SetWrapControls, default=1};
	__property System::Classes::TNotifyEvent OnVisibleChanged = {read=FOnVisibleChanged, write=FOnVisibleChanged};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderColor = {default=-16777201};
	__property BorderInner;
	__property BorderOuter;
	__property BorderSides;
	__property BorderWidth;
	__property Caption = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DockSite = {default=0};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FullRepaint = {default=0};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property Height = {default=32};
	__property Locked = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowDockClientCaptions = {default=0};
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Transparent = {default=0};
	__property UseDockManager = {default=1};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property Width = {default=32};
	__property OnCanResize;
	__property OnClick;
	__property OnConstrainedResize;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDockDrop;
	__property OnDockOver;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnGetSiteInfo;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnPaint;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnUnDock;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzToolbar(HWND ParentWindow) : TRzCustomPanel(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzStatusBar : public TRzCustomPanel
{
	typedef TRzCustomPanel inherited;
	
private:
	bool FAutoStyle;
	Vcl::Graphics::TCanvas* FSizeGripCanvas;
	bool FSimpleStatus;
	Vcl::Controls::TCaption FSimpleCaption;
	Rzcommon::TFrameStyleEx FSimpleFrameStyle;
	bool FSizeGripValid;
	bool FShowSizeGrip;
	bool FAutoScalePanes;
	bool FFirst;
	int FDelta;
	int FLastWidth;
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	DYNAMIC void __fastcall Resize();
	virtual System::Types::TRect __fastcall GetClientRect();
	void __fastcall PaintSizeGrip(const System::Types::TRect &R);
	virtual void __fastcall DrawSimpleStatusBorder(const System::Types::TRect &R);
	virtual void __fastcall GetGradientColors(System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
	virtual void __fastcall Paint();
	void __fastcall ValidateSizeGrip();
	System::Types::TRect __fastcall SizeGripRect();
	virtual void __fastcall AdjustStyle();
	virtual void __fastcall SetShowSizeGrip(bool Value);
	virtual void __fastcall SetSimpleCaption(Vcl::Controls::TCaption Value);
	virtual void __fastcall SetSimpleStatus(bool Value);
	virtual void __fastcall SetSimpleFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	
public:
	__fastcall virtual TRzStatusBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzStatusBar();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AutoScalePanes = {read=FAutoScalePanes, write=FAutoScalePanes, default=0};
	__property bool AutoStyle = {read=FAutoStyle, write=FAutoStyle, default=1};
	__property bool ShowSizeGrip = {read=FShowSizeGrip, write=SetShowSizeGrip, default=1};
	__property Vcl::Controls::TCaption SimpleCaption = {read=FSimpleCaption, write=SetSimpleCaption};
	__property Rzcommon::TFrameStyle SimpleFrameStyle = {read=FSimpleFrameStyle, write=SetSimpleFrameStyle, default=1};
	__property bool SimpleStatus = {read=FSimpleStatus, write=SetSimpleStatus, default=0};
	__property Align = {default=2};
	__property Anchors = {default=3};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderColor = {default=-16777201};
	__property BorderInner;
	__property BorderOuter;
	__property BorderSides;
	__property BorderWidth;
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FullRepaint = {default=1};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property Height;
	__property Locked = {default=0};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
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
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnPaint;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzStatusBar(HWND ParentWindow) : TRzCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzFlowPanel : public Vcl::Extctrls::TCustomFlowPanel
{
	typedef Vcl::Extctrls::TCustomFlowPanel inherited;
	
private:
	bool FInAlignControls;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Rzcommon::TSides FBorderSides;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FBorderHighlight;
	System::Uitypes::TColor FBorderShadow;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	Rzcommon::TRzVisualStyle FVisualStyle;
	Rzcommon::TRzGradientColorStyle FGradientColorStyle;
	System::Uitypes::TColor FGradientColorStart;
	System::Uitypes::TColor FGradientColorStop;
	Rzcommon::TGradientDirection FGradientDirection;
	bool FTransparent;
	System::Classes::TStringList* FEnabledList;
	System::Classes::TNotifyEvent FOnPaint;
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	MESSAGE void __fastcall WMThemeChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FThemeAware;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::Types::TPoint __fastcall CursorPosition();
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	virtual void __fastcall FixClientRect(System::Types::TRect &Rect, bool ShrinkByBorder);
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	virtual System::Types::TRect __fastcall GetControlRect();
	virtual void __fastcall GetGradientColors(System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual void __fastcall SetGradientColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetVisualStyle(Rzcommon::TRzVisualStyle Value);
	
public:
	__fastcall virtual TRzFlowPanel(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFlowPanel();
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	__property Canvas;
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=6};
	__property Rzcommon::TSides BorderSides = {read=FBorderSides, write=SetBorderSides, default=15};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property System::Uitypes::TColor BorderHighlight = {read=FBorderHighlight, write=SetBorderHighlight, default=-16777196};
	__property System::Uitypes::TColor BorderShadow = {read=FBorderShadow, write=SetBorderShadow, default=-16777200};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TRzGradientColorStyle GradientColorStyle = {read=FGradientColorStyle, write=SetGradientColorStyle, default=0};
	__property System::Uitypes::TColor GradientColorStart = {read=FGradientColorStart, write=SetGradientColorStart, default=16777215};
	__property System::Uitypes::TColor GradientColorStop = {read=FGradientColorStop, write=SetGradientColorStop, default=-16777201};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=2};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property Rzcommon::TRzVisualStyle VisualStyle = {read=FVisualStyle, write=SetVisualStyle, default=1};
	__property System::Classes::TNotifyEvent OnPaint = {read=FOnPaint, write=FOnPaint};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property AutoWrap = {default=1};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderWidth = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DockSite = {default=0};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlowStyle = {default=0};
	__property Font;
	__property FullRepaint = {default=1};
	__property Locked = {default=0};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Visible = {default=1};
	__property OnCanResize;
	__property OnClick;
	__property OnConstrainedResize;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzFlowPanel(HWND ParentWindow) : Vcl::Extctrls::TCustomFlowPanel(ParentWindow) { }
	
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


class PASCALIMPLEMENTATION TRzGridPanel : public Vcl::Extctrls::TCustomGridPanel
{
	typedef Vcl::Extctrls::TCustomGridPanel inherited;
	
private:
	bool FInAlignControls;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Rzcommon::TSides FBorderSides;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FBorderHighlight;
	System::Uitypes::TColor FBorderShadow;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	Rzcommon::TRzVisualStyle FVisualStyle;
	Rzcommon::TRzGradientColorStyle FGradientColorStyle;
	System::Uitypes::TColor FGradientColorStart;
	System::Uitypes::TColor FGradientColorStop;
	Rzcommon::TGradientDirection FGradientDirection;
	bool FTransparent;
	System::Classes::TStringList* FEnabledList;
	System::Classes::TNotifyEvent FOnPaint;
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	MESSAGE void __fastcall WMThemeChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::Types::TPoint __fastcall CursorPosition();
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	virtual void __fastcall FixClientRect(System::Types::TRect &Rect, bool ShrinkByBorder);
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	virtual System::Types::TRect __fastcall GetControlRect();
	virtual void __fastcall GetGradientColors(System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
	void __fastcall DrawGridLines();
	virtual void __fastcall Paint();
	virtual void __fastcall CustomFramingChanged();
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual void __fastcall SetGradientColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientDirection(Rzcommon::TGradientDirection Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetVisualStyle(Rzcommon::TRzVisualStyle Value);
	
public:
	__fastcall virtual TRzGridPanel(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzGridPanel();
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	__property Canvas;
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=6};
	__property Rzcommon::TSides BorderSides = {read=FBorderSides, write=SetBorderSides, default=15};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property System::Uitypes::TColor BorderHighlight = {read=FBorderHighlight, write=SetBorderHighlight, default=-16777196};
	__property System::Uitypes::TColor BorderShadow = {read=FBorderShadow, write=SetBorderShadow, default=-16777200};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property Rzcommon::TRzGradientColorStyle GradientColorStyle = {read=FGradientColorStyle, write=SetGradientColorStyle, default=0};
	__property System::Uitypes::TColor GradientColorStart = {read=FGradientColorStart, write=SetGradientColorStart, default=16777215};
	__property System::Uitypes::TColor GradientColorStop = {read=FGradientColorStop, write=SetGradientColorStop, default=-16777201};
	__property Rzcommon::TGradientDirection GradientDirection = {read=FGradientDirection, write=SetGradientDirection, default=2};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property Rzcommon::TRzVisualStyle VisualStyle = {read=FVisualStyle, write=SetVisualStyle, default=1};
	__property System::Classes::TNotifyEvent OnPaint = {read=FOnPaint, write=FOnPaint};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderWidth = {default=0};
	__property Color = {default=-16777201};
	__property ColumnCollection;
	__property Constraints;
	__property ControlCollection;
	__property Ctl3D;
	__property DockSite = {default=0};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property ExpandStyle = {default=0};
	__property Font;
	__property FullRepaint = {default=1};
	__property Locked = {default=0};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property RowCollection;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Visible = {default=1};
	__property OnCanResize;
	__property OnClick;
	__property OnConstrainedResize;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzGridPanel(HWND ParentWindow) : Vcl::Extctrls::TCustomGridPanel(ParentWindow) { }
	
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


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzpanel */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZPANEL)
using namespace Rzpanel;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzpanelHPP
