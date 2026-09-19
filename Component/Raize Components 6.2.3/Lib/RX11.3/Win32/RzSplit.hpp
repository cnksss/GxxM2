// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSplit.pas' rev: 35.00 (Windows)

#ifndef RzsplitHPP
#define RzsplitHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <Vcl.Menus.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzsplit
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzSplitterPane;
class DELPHICLASS TRzPaneData;
class DELPHICLASS TRzSplitter;
class DELPHICLASS TRzSizePanelDockManager;
class DELPHICLASS TRzCustomSizePanel;
class DELPHICLASS TRzSizePanel;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzSplitterPane : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	TRzSplitter* FSplitter;
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	
protected:
	virtual void __fastcall Paint();
	virtual void __fastcall ReadState(System::Classes::TReader* Reader);
	DYNAMIC void __fastcall DoEnter();
	DYNAMIC void __fastcall DoExit();
	bool __fastcall GetVisible();
	HIDESBASE void __fastcall SetVisible(bool Value);
	
public:
	__fastcall virtual TRzSplitterPane(System::Classes::TComponent* AOwner);
	
__published:
	__property bool Visible = {read=GetVisible, write=SetVisible, default=1};
	__property Color = {default=-16777201};
	__property BorderColor = {default=-16777201};
	__property BorderOuter = {default=0};
	__property BorderInner = {default=0};
	__property BorderWidth = {default=0};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property TabStop = {default=0};
	__property ShowDockClientCaptions = {default=1};
	__property Padding;
	__property OnContextPopup;
public:
	/* TRzCustomPanel.Destroy */ inline __fastcall virtual ~TRzSplitterPane() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSplitterPane(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPaneData : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzSplitterPane* FPane;
	
protected:
	System::Uitypes::TColor __fastcall GetBorderColor();
	void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	Rzcommon::TFrameStyleEx __fastcall GetBorderInner();
	void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	Rzcommon::TFrameStyleEx __fastcall GetBorderOuter();
	void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	Vcl::Controls::TBorderWidth __fastcall GetBorderWidth();
	void __fastcall SetBorderWidth(Vcl::Controls::TBorderWidth Value);
	System::Uitypes::TColor __fastcall GetColor();
	void __fastcall SetColor(System::Uitypes::TColor Value);
	bool __fastcall GetDockSite();
	void __fastcall SetDockSite(bool Value);
	System::Uitypes::TColor __fastcall GetFlatColor();
	void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	int __fastcall GetFlatColorAdjustment();
	void __fastcall SetFlatColorAdjustment(int Value);
	bool __fastcall GetShowDockClientCaptions();
	void __fastcall SetShowDockClientCaptions(bool Value);
	bool __fastcall GetVisible();
	void __fastcall SetVisible(bool Value);
	Vcl::Controls::TPadding* __fastcall GetPadding();
	void __fastcall SetPadding(Vcl::Controls::TPadding* Value);
	
public:
	__property TRzSplitterPane* Pane = {read=FPane};
	
__published:
	__property System::Uitypes::TColor BorderColor = {read=GetBorderColor, write=SetBorderColor, default=-16777201};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=GetBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=GetBorderOuter, write=SetBorderOuter, default=0};
	__property Vcl::Controls::TBorderWidth BorderWidth = {read=GetBorderWidth, write=SetBorderWidth, default=0};
	__property System::Uitypes::TColor Color = {read=GetColor, write=SetColor, default=-16777201};
	__property System::Uitypes::TColor FlatColor = {read=GetFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=GetFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property bool DockSite = {read=GetDockSite, write=SetDockSite, default=0};
	__property bool ShowDockClientCaptions = {read=GetShowDockClientCaptions, write=SetShowDockClientCaptions, default=1};
	__property bool Visible = {read=GetVisible, write=SetVisible, default=1};
	__property Vcl::Controls::TPadding* Padding = {read=GetPadding, write=SetPadding};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzPaneData() { }
	
public:
	/* TObject.Create */ inline __fastcall TRzPaneData() : System::Classes::TPersistent() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TSelectedPane : unsigned char { spUpperLeft, spLowerRight };

enum DECLSPEC_DENUM TSplitterStyle : unsigned char { ssStandard, ssGroove, ssBump, ssGroupBar };

enum DECLSPEC_DENUM TFixedPane : unsigned char { fpUpperLeft, fpLowerRight };

enum DECLSPEC_DENUM THotSpotDirection : unsigned char { hsdMin, hsdMax, hsdBoth };

enum DECLSPEC_DENUM THotSpotPosition : unsigned char { hspOpen, hspClosed };

class PASCALIMPLEMENTATION TRzSplitter : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	Rzcommon::TOrientation FOrientation;
	System::StaticArray<TRzPaneData*, 2> FPaneData;
	System::StaticArray<TRzSplitterPane*, 2> FPanes;
	System::StaticArray<System::Classes::TStringList*, 2> FPanesControlList;
	TSelectedPane FSelectedPane;
	int FPosition;
	int FMarginMin;
	int FMarginMax;
	bool FUsePercent;
	int FPercent;
	int FPercentMin;
	int FPercentMax;
	TFixedPane FFixedPane;
	int FFixedSize;
	bool FResizing;
	System::Word FSplitterWidth;
	TSplitterStyle FSplitterStyle;
	bool FRealTimeDrag;
	bool FLockBar;
	System::Types::TRect FBarRect;
	int FOrigPos;
	HICON FHorzCursor;
	HICON FHorzCursorHotSpot;
	HICON FVertCursor;
	HICON FVertCursorHotSpot;
	bool FSliding;
	int FCenterOffset;
	int FManualPosition;
	int FLastPos;
	Vcl::Graphics::TBrush* FBrush;
	HBRUSH FPrevBrush;
	HDC FMaskDC;
	bool FOutsideHotSpot;
	bool FHotSpotVisible;
	System::Uitypes::TColor FHotSpotHighlight;
	System::Uitypes::TColor FHotSpotColor;
	System::Uitypes::TColor FHotSpotDotColor;
	System::Uitypes::TColor FHotSpotFrameColor;
	System::Types::TRect FHotSpotRect;
	THotSpotDirection FHotSpotDirection;
	bool FHotSpotClosed;
	bool FHotSpotClosedToMin;
	bool FHotSpotClosedToMax;
	bool FHotSpotting;
	int FHotSpotPosition;
	bool FHotSpotIgnoreMargins;
	int FHotSpotSizePercent;
	bool FRefreshHotSpot;
	int FGradientColorAdjustment;
	Rzcommon::TRzGroupBarGradientPath FGradientPath;
	System::Classes::TNotifyEvent FOnChange;
	Rzcommon::TPositionChangingEvent FOnChanging;
	System::Classes::TNotifyEvent FOnHotSpotClick;
	Vcl::Controls::TGetSiteInfoEvent FOnULGetSiteInfo;
	Vcl::Controls::TDockOverEvent FOnULDockOver;
	Vcl::Controls::TDockDropEvent FOnULDockDrop;
	Vcl::Controls::TUnDockEvent FOnULUnDock;
	Vcl::Controls::TGetSiteInfoEvent FOnLRGetSiteInfo;
	Vcl::Controls::TDockOverEvent FOnLRDockOver;
	Vcl::Controls::TDockDropEvent FOnLRDockDrop;
	Vcl::Controls::TUnDockEvent FOnLRUnDock;
	void __fastcall ReadBarSize(System::Classes::TReader* Reader);
	void __fastcall WriteBarSize(System::Classes::TWriter* Writer);
	void __fastcall ReadHotSpotClosed(System::Classes::TReader* Reader);
	void __fastcall WriteHotSpotClosed(System::Classes::TWriter* Writer);
	void __fastcall ReadHotSpotClosedToMin(System::Classes::TReader* Reader);
	void __fastcall WriteHotSpotClosedToMin(System::Classes::TWriter* Writer);
	void __fastcall ReadHotSpotClosedToMax(System::Classes::TReader* Reader);
	void __fastcall WriteHotSpotClosedToMax(System::Classes::TWriter* Writer);
	void __fastcall ReadHotSpotPosition(System::Classes::TReader* Reader);
	void __fastcall WriteHotSpotPosition(System::Classes::TWriter* Writer);
	void __fastcall ReadULControls(System::Classes::TReader* Reader);
	void __fastcall WriteULControls(System::Classes::TWriter* Writer);
	void __fastcall ReadLRControls(System::Classes::TReader* Reader);
	void __fastcall WriteLRControls(System::Classes::TWriter* Writer);
	void __fastcall AllocateMaskDC();
	void __fastcall ReleaseMaskDC();
	void __fastcall ULGetSiteInfoHandler(System::TObject* Sender, Vcl::Controls::TControl* DockClient, System::Types::TRect &InfluenceRect, const System::Types::TPoint &MousePos, bool &CanDock);
	void __fastcall ULDockOverHandler(System::TObject* Sender, Vcl::Controls::TDragDockObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	void __fastcall ULDockDropHandler(System::TObject* Sender, Vcl::Controls::TDragDockObject* Source, int X, int Y);
	void __fastcall ULUnDockHandler(System::TObject* Sender, Vcl::Controls::TControl* Client, Vcl::Controls::TWinControl* NewTarget, bool &Allow);
	void __fastcall LRGetSiteInfoHandler(System::TObject* Sender, Vcl::Controls::TControl* DockClient, System::Types::TRect &InfluenceRect, const System::Types::TPoint &MousePos, bool &CanDock);
	void __fastcall LRDockOverHandler(System::TObject* Sender, Vcl::Controls::TDragDockObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	void __fastcall LRDockDropHandler(System::TObject* Sender, Vcl::Controls::TDragDockObject* Source, int X, int Y);
	void __fastcall LRUnDockHandler(System::TObject* Sender, Vcl::Controls::TControl* Client, Vcl::Controls::TWinControl* NewTarget, bool &Allow);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall WMShowWindow(Winapi::Messages::TWMShowWindow &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual System::Types::TRect __fastcall GetClientRect();
	virtual void __fastcall ShowControl(Vcl::Controls::TControl* AControl);
	DYNAMIC void __fastcall Resize();
	virtual void __fastcall UpdateFixedSize();
	virtual int __fastcall GetBorderOffset();
	virtual void __fastcall DrawMask(int NewPos);
	virtual void __fastcall UpdateHotSpotRect();
	virtual void __fastcall UpdateHotSpotHighlight();
	virtual void __fastcall DrawHotSpot(bool Highlight, const System::Types::TPoint &P);
	virtual void __fastcall DrawSplitterBar();
	virtual void __fastcall Paint();
	virtual void __fastcall CheckPosition(int &Value);
	virtual void __fastcall UpdateManualPosition(int P);
	DYNAMIC void __fastcall Change();
	DYNAMIC bool __fastcall CanChange(int NewPos);
	virtual void __fastcall MouseDownSetFocus();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall HotSpotClick();
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetSelectedPane(TSelectedPane Value);
	virtual TRzSplitterPane* __fastcall GetSelectedPaneControl();
	virtual void __fastcall SetFixedPane(TFixedPane Value);
	virtual TRzPaneData* __fastcall GetPaneData(int Index);
	virtual void __fastcall SetPaneData(int Index, TRzPaneData* Value);
	virtual void __fastcall SetPosition(int Value);
	virtual void __fastcall SetLockedPosition(int Value);
	virtual void __fastcall SetMarginMax(int Value);
	virtual void __fastcall SetMarginMin(int Value);
	virtual bool __fastcall GetParentColor();
	HIDESBASE virtual void __fastcall SetParentColor(bool Value);
	virtual void __fastcall SetPercent(int Value);
	virtual void __fastcall SetUsePercent(bool Value);
	virtual void __fastcall SetPercentMax(int Value);
	virtual void __fastcall SetPercentMin(int Value);
	virtual void __fastcall SetSplitterStyle(TSplitterStyle Value);
	virtual void __fastcall SetSplitterWidth(System::Word Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetGradientColorAdjustment(int Value);
	virtual void __fastcall SetGradientPath(Rzcommon::TRzGroupBarGradientPath Value);
	Vcl::Controls::TBorderWidth __fastcall GetBorderWidth();
	HIDESBASE void __fastcall SetBorderWidth(Vcl::Controls::TBorderWidth Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetHotSpotVisible(bool Value);
	void __fastcall SetHotSpotColor(System::Uitypes::TColor Value);
	void __fastcall SetHotSpotDotColor(System::Uitypes::TColor Value);
	void __fastcall SetHotSpotFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotSpotHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotSpotDirection(THotSpotDirection Value);
	virtual void __fastcall SetHotSpotSizePercent(int Value);
	
public:
	__fastcall virtual TRzSplitter(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzSplitter();
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	virtual void __fastcall ResizePanes();
	virtual void __fastcall ResetHotSpot();
	void __fastcall RestoreHotSpot();
	void __fastcall CloseHotSpot()/* overload */;
	void __fastcall CloseHotSpot(bool ToMin)/* overload */;
	__property bool HotSpotClosed = {read=FHotSpotClosed, nodefault};
	__property bool HotSpotClosedToMin = {read=FHotSpotClosedToMin, nodefault};
	__property bool HotSpotClosedToMax = {read=FHotSpotClosedToMax, nodefault};
	__property int HotSpotPosition = {read=FHotSpotPosition, nodefault};
	__property System::Types::TRect HotSpotRect = {read=FHotSpotRect};
	__property int LockedPosition = {read=FPosition, write=SetLockedPosition, nodefault};
	__property TRzSplitterPane* SelectedPaneControl = {read=GetSelectedPaneControl};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Controls::TBorderWidth BorderWidth = {read=GetBorderWidth, write=SetBorderWidth, default=0};
	__property int GradientColorAdjustment = {read=FGradientColorAdjustment, write=SetGradientColorAdjustment, default=30};
	__property TFixedPane FixedPane = {read=FFixedPane, write=SetFixedPane, default=0};
	__property Rzcommon::TRzGroupBarGradientPath GradientPath = {read=FGradientPath, write=SetGradientPath, default=0};
	__property bool LockBar = {read=FLockBar, write=FLockBar, default=0};
	__property int MarginMax = {read=FMarginMax, write=SetMarginMax, default=0};
	__property int MarginMin = {read=FMarginMin, write=SetMarginMin, default=0};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, default=0};
	__property bool ParentColor = {read=GetParentColor, write=SetParentColor, default=0};
	__property int Position = {read=FPosition, write=SetPosition, nodefault};
	__property int Percent = {read=FPercent, write=SetPercent, default=50};
	__property int PercentMax = {read=FPercentMax, write=SetPercentMax, default=100};
	__property int PercentMin = {read=FPercentMin, write=SetPercentMin, default=0};
	__property bool UsePercent = {read=FUsePercent, write=SetUsePercent, default=0};
	__property bool RealTimeDrag = {read=FRealTimeDrag, write=FRealTimeDrag, default=0};
	__property TRzPaneData* UpperLeft = {read=GetPaneData, write=SetPaneData, stored=true, index=1};
	__property TRzPaneData* LowerRight = {read=GetPaneData, write=SetPaneData, stored=true, index=2};
	__property bool HotSpotVisible = {read=FHotSpotVisible, write=SetHotSpotVisible, default=0};
	__property System::Uitypes::TColor HotSpotColor = {read=FHotSpotColor, write=SetHotSpotColor, default=-16777201};
	__property System::Uitypes::TColor HotSpotDotColor = {read=FHotSpotDotColor, write=SetHotSpotDotColor, default=-16777203};
	__property System::Uitypes::TColor HotSpotFrameColor = {read=FHotSpotFrameColor, write=SetHotSpotFrameColor, default=-16777200};
	__property System::Uitypes::TColor HotSpotHighlight = {read=FHotSpotHighlight, write=SetHotSpotHighlight, default=-16777211};
	__property bool HotSpotIgnoreMargins = {read=FHotSpotIgnoreMargins, write=FHotSpotIgnoreMargins, default=1};
	__property THotSpotDirection HotSpotDirection = {read=FHotSpotDirection, write=SetHotSpotDirection, default=0};
	__property int HotSpotSizePercent = {read=FHotSpotSizePercent, write=SetHotSpotSizePercent, default=40};
	__property TSelectedPane SelectedPane = {read=FSelectedPane, write=SetSelectedPane, stored=false, nodefault};
	__property TSplitterStyle SplitterStyle = {read=FSplitterStyle, write=SetSplitterStyle, default=0};
	__property System::Word SplitterWidth = {read=FSplitterWidth, write=SetSplitterWidth, stored=true, default=4};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property Rzcommon::TPositionChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property System::Classes::TNotifyEvent OnHotSpotClick = {read=FOnHotSpotClick, write=FOnHotSpotClick};
	__property Vcl::Controls::TGetSiteInfoEvent OnULGetSiteInfo = {read=FOnULGetSiteInfo, write=FOnULGetSiteInfo};
	__property Vcl::Controls::TDockOverEvent OnULDockOver = {read=FOnULDockOver, write=FOnULDockOver};
	__property Vcl::Controls::TDockDropEvent OnULDockDrop = {read=FOnULDockDrop, write=FOnULDockDrop};
	__property Vcl::Controls::TUnDockEvent OnULUnDock = {read=FOnULUnDock, write=FOnULUnDock};
	__property Vcl::Controls::TGetSiteInfoEvent OnLRGetSiteInfo = {read=FOnLRGetSiteInfo, write=FOnLRGetSiteInfo};
	__property Vcl::Controls::TDockOverEvent OnLRDockOver = {read=FOnLRDockOver, write=FOnLRDockOver};
	__property Vcl::Controls::TDockDropEvent OnLRDockDrop = {read=FOnLRDockDrop, write=FOnLRDockDrop};
	__property Vcl::Controls::TUnDockEvent OnLRUnDock = {read=FOnLRUnDock, write=FOnLRUnDock};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BevelWidth = {default=1};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=0};
	__property BorderShadow = {default=-16777200};
	__property BorderSides = {default=15};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property FullRepaint = {default=1};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property Height = {default=100};
	__property Locked = {default=0};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Transparent = {default=0};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property Width = {default=200};
	__property OnCanResize;
	__property OnConstrainedResize;
	__property OnContextPopup;
	__property OnEnter;
	__property OnExit;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSplitter(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzSizePanelDockManager : public Rzpanel::TRzPanelDockManager
{
	typedef Rzpanel::TRzPanelDockManager inherited;
	
private:
	TRzCustomSizePanel* FSizePanel;
	
protected:
	virtual void __fastcall PositionDockRect(Vcl::Controls::TControl* Client, Vcl::Controls::TControl* DropCtl, Vcl::Controls::TAlign DropAlign, System::Types::TRect &DockRect);
	virtual void __fastcall PaintDockFrame(Vcl::Graphics::TCanvas* Canvas, Vcl::Controls::TControl* Control, const System::Types::TRect &ARect);
	
public:
	__fastcall virtual TRzSizePanelDockManager(Vcl::Controls::TWinControl* DockSite);
	virtual void __fastcall PaintSite(HDC DC);
public:
	/* TRzPanelDockManager.Destroy */ inline __fastcall virtual ~TRzSizePanelDockManager() { }
	
};


typedef int TSizeBarWidth;

class PASCALIMPLEMENTATION TRzCustomSizePanel : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	int FMarginMin;
	int FMarginMax;
	int FMarginOffset;
	HICON FHorzCursor;
	HICON FHorzCursorHotSpot;
	HICON FVertCursor;
	HICON FVertCursorHotSpot;
	bool FLockBar;
	TSizeBarWidth FSizeBarWidth;
	TSplitterStyle FSizeBarStyle;
	bool FResizing;
	bool FOutsideHotSpot;
	bool FHotSpotVisible;
	System::Uitypes::TColor FHotSpotHighlight;
	System::Uitypes::TColor FHotSpotColor;
	System::Uitypes::TColor FHotSpotDotColor;
	System::Uitypes::TColor FHotSpotFrameColor;
	System::Types::TRect FHotSpotRect;
	bool FHotSpotClosed;
	bool FHotSpotting;
	int FHotSpotPosition;
	Rzcommon::TSide FSide;
	System::Classes::TNotifyEvent FOnHotSpotClick;
	bool FHotSpotIgnoreMargins;
	int FHotSpotSizePercent;
	bool FRefreshHotSpot;
	bool FRealTimeDrag;
	int FOrigPos;
	int FOrigWidth;
	int FOrigHeight;
	int FCenterOffset;
	int FLastPos;
	Vcl::Graphics::TBrush* FBrush;
	HBRUSH FPrevBrush;
	HDC FMaskDC;
	int FGradientColorAdjustment;
	Rzcommon::TRzGroupBarGradientPath FGradientPath;
	void __fastcall ReadHotSpotClosed(System::Classes::TReader* Reader);
	void __fastcall WriteHotSpotClosed(System::Classes::TWriter* Writer);
	void __fastcall ReadHotSpotPosition(System::Classes::TReader* Reader);
	void __fastcall WriteHotSpotPosition(System::Classes::TWriter* Writer);
	void __fastcall AllocateMaskDC();
	void __fastcall ReleaseMaskDC();
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall FixClientRect(System::Types::TRect &Rect, bool ShrinkByBorder);
	virtual System::Types::TRect __fastcall GetClientRect();
	virtual System::Types::TRect __fastcall GetControlRect();
	virtual void __fastcall DrawMask(int NewPos);
	virtual void __fastcall UpdateHotSpotRect(const System::Types::TRect &BarRect);
	virtual void __fastcall UpdateHotSpotHighlight();
	virtual void __fastcall DrawHotSpot(bool Highlight);
	virtual void __fastcall DrawSizeBar();
	virtual void __fastcall Paint();
	virtual System::Types::TRect __fastcall GetParentWorkingRect();
	virtual void __fastcall CheckPosition(int &Value);
	virtual bool __fastcall HotSpotTopBottom();
	DYNAMIC Vcl::Controls::_di_IDockManager __fastcall CreateDockManager();
	virtual void __fastcall MouseDownSetFocus();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall HotSpotClick();
	virtual Vcl::Controls::TAlign __fastcall GetAlign();
	HIDESBASE virtual void __fastcall SetAlign(Vcl::Controls::TAlign Value);
	virtual void __fastcall SetSizeBarWidth(TSizeBarWidth Value);
	virtual void __fastcall SetSizeBarStyle(TSplitterStyle Value);
	virtual System::Types::TRect __fastcall GetSizeBarRect();
	virtual int __fastcall GetMarginExtent();
	virtual void __fastcall SetMarginMax(int Value);
	virtual void __fastcall SetMarginMin(int Value);
	virtual void __fastcall SetHotSpotVisible(bool Value);
	void __fastcall SetHotSpotColor(System::Uitypes::TColor Value);
	void __fastcall SetHotSpotDotColor(System::Uitypes::TColor Value);
	void __fastcall SetHotSpotFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotSpotHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotSpotSizePercent(int Value);
	virtual void __fastcall SetSide(Rzcommon::TSide Value);
	virtual void __fastcall SetGradientColorAdjustment(int Value);
	virtual void __fastcall SetGradientPath(Rzcommon::TRzGroupBarGradientPath Value);
	__property Vcl::Controls::TAlign Align = {read=GetAlign, write=SetAlign, default=3};
	__property BorderOuter = {default=0};
	__property int GradientColorAdjustment = {read=FGradientColorAdjustment, write=SetGradientColorAdjustment, default=30};
	__property Rzcommon::TRzGroupBarGradientPath GradientPath = {read=FGradientPath, write=SetGradientPath, default=0};
	__property TSizeBarWidth SizeBarWidth = {read=FSizeBarWidth, write=SetSizeBarWidth, default=4};
	__property TSplitterStyle SizeBarStyle = {read=FSizeBarStyle, write=SetSizeBarStyle, default=0};
	__property int MarginMin = {read=FMarginMin, write=SetMarginMin, default=0};
	__property int MarginMax = {read=FMarginMax, write=SetMarginMax, default=0};
	__property bool LockBar = {read=FLockBar, write=FLockBar, default=0};
	__property bool HotSpotClosed = {read=FHotSpotClosed, nodefault};
	__property int HotSpotPosition = {read=FHotSpotPosition, nodefault};
	__property System::Types::TRect HotSpotRect = {read=FHotSpotRect};
	__property bool HotSpotVisible = {read=FHotSpotVisible, write=SetHotSpotVisible, default=0};
	__property System::Uitypes::TColor HotSpotColor = {read=FHotSpotColor, write=SetHotSpotColor, default=-16777201};
	__property System::Uitypes::TColor HotSpotDotColor = {read=FHotSpotDotColor, write=SetHotSpotDotColor, default=-16777203};
	__property System::Uitypes::TColor HotSpotFrameColor = {read=FHotSpotFrameColor, write=SetHotSpotFrameColor, default=-16777200};
	__property System::Uitypes::TColor HotSpotHighlight = {read=FHotSpotHighlight, write=SetHotSpotHighlight, default=-16777211};
	__property bool HotSpotIgnoreMargins = {read=FHotSpotIgnoreMargins, write=FHotSpotIgnoreMargins, default=0};
	__property int HotSpotSizePercent = {read=FHotSpotSizePercent, write=SetHotSpotSizePercent, default=40};
	__property Rzcommon::TSide Side = {read=FSide, write=SetSide, default=0};
	__property bool RealTimeDrag = {read=FRealTimeDrag, write=FRealTimeDrag, default=0};
	__property System::Classes::TNotifyEvent OnHotSpotClick = {read=FOnHotSpotClick, write=FOnHotSpotClick};
	
public:
	__fastcall virtual TRzCustomSizePanel(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomSizePanel();
	virtual void __fastcall ResetHotSpot();
	void __fastcall RestoreHotSpot();
	void __fastcall CloseHotSpot();
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomSizePanel(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzSizePanel : public TRzCustomSizePanel
{
	typedef TRzCustomSizePanel inherited;
	
public:
	__property HotSpotClosed;
	__property HotSpotPosition;
	__property HotSpotRect;
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=3};
	__property Alignment = {default=2};
	__property AlignmentVertical = {default=1};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=0};
	__property BorderShadow = {default=-16777200};
	__property BorderSides = {default=15};
	__property BorderWidth = {default=0};
	__property Caption = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DockSite = {default=0};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FullRepaint = {default=1};
	__property GradientColorAdjustment = {default=30};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property GradientPath = {default=0};
	__property HotSpotColor = {default=-16777201};
	__property HotSpotDotColor = {default=-16777203};
	__property HotSpotFrameColor = {default=-16777200};
	__property HotSpotHighlight = {default=-16777211};
	__property HotSpotIgnoreMargins = {default=0};
	__property HotSpotSizePercent = {default=40};
	__property HotSpotVisible = {default=0};
	__property Side = {default=0};
	__property LockBar = {default=0};
	__property Locked = {default=0};
	__property MarginMax = {default=0};
	__property MarginMin = {default=0};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property RealTimeDrag = {default=0};
	__property ShowDockClientCaptions = {default=1};
	__property ShowHint;
	__property SizeBarStyle = {default=0};
	__property SizeBarWidth = {default=4};
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property UseDockManager = {default=1};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
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
	__property OnGetSiteInfo;
	__property OnHotSpotClick;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnUnDock;
public:
	/* TRzCustomSizePanel.Create */ inline __fastcall virtual TRzSizePanel(System::Classes::TComponent* AOwner) : TRzCustomSizePanel(AOwner) { }
	/* TRzCustomSizePanel.Destroy */ inline __fastcall virtual ~TRzSizePanel() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSizePanel(HWND ParentWindow) : TRzCustomSizePanel(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzsplit */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSPLIT)
using namespace Rzsplit;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzsplitHPP
