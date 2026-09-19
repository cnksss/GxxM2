// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzTabs.pas' rev: 35.00 (Windows)

#ifndef RztabsHPP
#define RztabsHPP

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
#include <Vcl.Controls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <System.Classes.hpp>
#include <RzCommon.hpp>
#include <RzGrafx.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztabs
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS ERzTabControlError;
class DELPHICLASS TRzTabScroller;
class DELPHICLASS TRzTabControlCloseButton;
class DELPHICLASS TRzActiveTabCloseButton;
class DELPHICLASS TRzTabMenuButton;
class DELPHICLASS TRzTabRegionCache;
class DELPHICLASS TRzTextExtentCache;
class DELPHICLASS TRzTabColors;
class DELPHICLASS TRzTextColors;
class DELPHICLASS TRzCustomTabControl;
class DELPHICLASS TRzTabData;
class DELPHICLASS TRzTabDataList;
class DELPHICLASS TRzTabSheet;
class DELPHICLASS TRzPageControl;
class DELPHICLASS TRzTabCollectionItem;
class DELPHICLASS TRzTabCollection;
class DELPHICLASS TRzCollectionTabControl;
class DELPHICLASS TRzTabControl;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION ERzTabControlError : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall ERzTabControlError(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall ERzTabControlError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall ERzTabControlError(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall ERzTabControlError(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzTabControlError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzTabControlError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall ERzTabControlError(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall ERzTabControlError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzTabControlError(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzTabControlError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzTabControlError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzTabControlError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~ERzTabControlError() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzTabScrollerBtn : unsigned char { sbDownLeft, sbUpRight };

class PASCALIMPLEMENTATION TRzTabScroller : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TOrientation FOrientation;
	TRzCustomTabControl* FTabControl;
	bool FPressed;
	bool FDown;
	bool FMouseOverButton;
	TRzTabScrollerBtn FOver;
	TRzTabScrollerBtn FCurrent;
	Vcl::Extctrls::TTimer* FRepeatTimer;
	System::Word FInitialDelay;
	System::Word FDelay;
	System::Classes::TNotifyEvent FOnUpRightClick;
	System::Classes::TNotifyEvent FOnDownLeftClick;
	void __fastcall UpdateTracking();
	void __fastcall RepeatTimerExpired(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DownLeftClick();
	DYNAMIC void __fastcall UpRightClick();
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetOrientation(Rzcommon::TOrientation Value);
	
public:
	__fastcall virtual TRzTabScroller(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzTabScroller();
	
__published:
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property Rzcommon::TOrientation Orientation = {read=FOrientation, write=SetOrientation, stored=false, nodefault};
	__property System::Classes::TNotifyEvent OnUpRightClick = {read=FOnUpRightClick, write=FOnUpRightClick};
	__property System::Classes::TNotifyEvent OnDownLeftClick = {read=FOnDownLeftClick, write=FOnDownLeftClick};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabScroller(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzTabControlCloseButton : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	System::Uitypes::TColor FFrameColor;
	TRzCustomTabControl* FTabControl;
	bool FPressed;
	bool FDown;
	bool FMouseOverButton;
	System::Classes::TNotifyEvent FOnClose;
	void __fastcall UpdateTracking();
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall DoClose();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzTabControlCloseButton(System::Classes::TComponent* AOwner);
	
__published:
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property System::Classes::TNotifyEvent OnClose = {read=FOnClose, write=FOnClose};
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzTabControlCloseButton() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabControlCloseButton(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzActiveTabCloseButton : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	System::Uitypes::TColor FFrameColor;
	TRzCustomTabControl* FTabControl;
	bool FPressed;
	bool FDown;
	bool FMouseOverButton;
	System::Classes::TNotifyEvent FOnClose;
	void __fastcall UpdateTracking();
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall DoClose();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzActiveTabCloseButton(System::Classes::TComponent* AOwner);
	
__published:
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property System::Classes::TNotifyEvent OnClose = {read=FOnClose, write=FOnClose};
public:
	/* TGraphicControl.Destroy */ inline __fastcall virtual ~TRzActiveTabCloseButton() { }
	
};


class PASCALIMPLEMENTATION TRzTabMenuButton : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	System::Uitypes::TColor FFrameColor;
	TRzCustomTabControl* FTabControl;
	Vcl::Menus::TPopupMenu* FMenu;
	bool FPressed;
	bool FDown;
	bool FMouseOverButton;
	System::Classes::TNotifyEvent FOnDisplayMenu;
	void __fastcall TabSelectedHandler(System::TObject* Sender);
	void __fastcall UpdateTracking();
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzTabMenuButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzTabMenuButton();
	DYNAMIC void __fastcall DisplayMenu();
	
__published:
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property System::Classes::TNotifyEvent OnDisplayMenu = {read=FOnDisplayMenu, write=FOnDisplayMenu};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabMenuButton(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzTabStyle : unsigned char { tsSingleSlant, tsDoubleSlant, tsCutCorner, tsRoundCorners, tsBackSlant, tsSquareCorners };

enum DECLSPEC_DENUM TRzTabSequence : unsigned char { tsStandard, tsReverse };

enum DECLSPEC_DENUM TRzTabOrientation : unsigned char { toTop, toLeft, toBottom, toRight };

enum DECLSPEC_DENUM TRzImagePosition : unsigned char { ipLeft, ipTop, ipRight, ipBottom, ipBack, ipStretch };

enum DECLSPEC_DENUM TRzHorizontalAlignment : unsigned char { haLeft, haCenter, haRight };

enum DECLSPEC_DENUM TRzVerticalAlignment : unsigned char { vaTop, vaCenter, vaBottom };

enum DECLSPEC_DENUM TRzTextVerticalBaseline : unsigned char { tvbLeft, tvbRight };

enum DECLSPEC_DENUM TRzTextStyle : unsigned char { tsHotTrack, tsSelected, tsUnselected, tsDisabled };

enum DECLSPEC_DENUM TRzTabHotTrackStyle : unsigned char { htsTab, htsText, htsTabBar };

enum DECLSPEC_DENUM TRzTabHotTrackColorSource : unsigned char { htcsTabColor, htcsHotTrackColorProp };

typedef System::StaticArray<System::Types::TPoint, 20> TRzTabRegionPts;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabRegionCache : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::Classes::TList* FCache;
	int FCapacity;
	int __fastcall GetCount();
	void __fastcall SetCapacity(int Value);
	
public:
	__fastcall TRzTabRegionCache();
	__fastcall virtual ~TRzTabRegionCache();
	void __fastcall Add(const System::Types::TRect &ARect, HRGN ARegion);
	void __fastcall Clear();
	HRGN __fastcall Find(const System::Types::TRect &ARect);
	__property int Capacity = {read=FCapacity, write=SetCapacity, default=1};
	__property int Count = {read=GetCount, nodefault};
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTextExtentCache : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	System::Classes::TStringList* FCache;
	
public:
	__fastcall TRzTextExtentCache();
	__fastcall virtual ~TRzTextExtentCache();
	void __fastcall Add(const System::UnicodeString AString, const System::Types::TSize &Extent);
	void __fastcall Clear();
	bool __fastcall Find(const System::UnicodeString AString, System::Types::TSize &Extent);
};

#pragma pack(pop)

typedef void __fastcall (__closure *TRzTabOrderChangeEvent)(System::TObject* Sender, int OldIndex, int NewIndex);

typedef void __fastcall (__closure *TRzTabDraggingEvent)(System::TObject* Sender, int TabIndex, bool &AllowDrag);

typedef void __fastcall (__closure *TRzTabCloseEvent)(System::TObject* Sender, bool &AllowClose);

typedef void __fastcall (__closure *TRzTabChangingEvent)(System::TObject* Sender, int NewIndex, bool &AllowChange);

typedef void __fastcall (__closure *TRzPaintBackgroundEvent)(System::TObject* Sender, Vcl::Graphics::TCanvas* ACanvas, const System::Types::TRect &ARect, bool &Handled);

typedef void __fastcall (__closure *TRzPaintTabBackgroundEvent)(System::TObject* Sender, Vcl::Graphics::TCanvas* ACanvas, int ATabIndex, const System::Types::TRect &ARect, bool &Handled);

typedef void __fastcall (__closure *TRzPaintCardBackgroundEvent)(System::TObject* Sender, Vcl::Graphics::TCanvas* ACanvas, int ARow, const System::Types::TRect &ARect, bool &Handled);

typedef void __fastcall (__closure *TRzGetTextColorEvent)(System::TObject* Sender, int ATabIndex, TRzTextStyle AStyle, System::Uitypes::TColor &AColor, bool &Handled);

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabColors : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzCustomTabControl* FTabControl;
	System::Uitypes::TColor FHighlightBar;
	System::Uitypes::TColor FShadow;
	System::Uitypes::TColor FUnselected;
	
protected:
	virtual void __fastcall SetHighlightBar(System::Uitypes::TColor Value);
	virtual void __fastcall SetShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetUnselected(System::Uitypes::TColor Value);
	
public:
	__fastcall TRzTabColors(TRzCustomTabControl* TabControl);
	
__published:
	__property System::Uitypes::TColor HighlightBar = {read=FHighlightBar, write=SetHighlightBar, default=-16777203};
	__property System::Uitypes::TColor Shadow = {read=FShadow, write=SetShadow, default=-16777201};
	__property System::Uitypes::TColor Unselected = {read=FUnselected, write=SetUnselected, default=-16777211};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzTabColors() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTextColors : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzCustomTabControl* FTabControl;
	System::Uitypes::TColor FDisabled;
	System::Uitypes::TColor FSelected;
	System::Uitypes::TColor FUnselected;
	
protected:
	virtual void __fastcall SetDisabled(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelected(System::Uitypes::TColor Value);
	virtual void __fastcall SetUnselected(System::Uitypes::TColor Value);
	
public:
	__fastcall TRzTextColors(TRzCustomTabControl* TabControl);
	
__published:
	__property System::Uitypes::TColor Disabled = {read=FDisabled, write=SetDisabled, default=-16777199};
	__property System::Uitypes::TColor Selected = {read=FSelected, write=SetSelected, default=-16777198};
	__property System::Uitypes::TColor Unselected = {read=FUnselected, write=SetUnselected, default=-16777198};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzTextColors() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCustomTabControl : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	Vcl::Graphics::TBitmap* FBuffer;
	bool FCalcNeeded;
	System::Classes::TStringList* FCalcTextExtentLines;
	bool FChangingDone;
	System::Classes::TList* FCommands;
	bool FDoneTabIndexDefault;
	System::Classes::TStringList* FDrawTabTextLines;
	int FFirstInView;
	int FFixedDimension;
	System::Uitypes::TColor FFocusRectBrushColor;
	System::Classes::TList* FHFonts;
	System::Types::TPoint FHitTest;
	int FHotTrackIndex;
	System::UnicodeString FOriginalHint;
	System::Types::TRect FPrevDisplayRect;
	int FRowExtent;
	TRzTabScroller* FScroller;
	bool FScrollerNeeded;
	TRzTabDataList* FTabDataList;
	TRzTabRegionCache* FTabRegionCache;
	int FTabRegionCacheSize;
	TRzTabControlCloseButton* FCloseButton;
	bool FShowCloseButton;
	TRzActiveTabCloseButton* FActiveTabCloseButton;
	bool FShowCloseButtonOnActiveTab;
	TRzTabMenuButton* FMenuButton;
	bool FShowMenuButton;
	bool FSortTabMenu;
	Vcl::Graphics::TFont* FTextFont;
	NativeUInt FTimerHandle;
	Vcl::Extctrls::TTimer* FScrollTimer;
	System::Word FInitialDelay;
	System::Word FDelay;
	bool FAlignTabs;
	System::Uitypes::TColor FBackgroundColor;
	bool FBoldCurrentTab;
	System::Uitypes::TColor FButtonColor;
	System::Uitypes::TColor FButtonColorDisabled;
	System::Uitypes::TColor FButtonSymbolColor;
	System::Uitypes::TColor FButtonSymbolColorDisabled;
	bool FShowCard;
	bool FShowCardFrame;
	bool FShowFullFrame;
	bool FUseColoredTabs;
	bool FLightenUnselectedColoredTabs;
	int FCutCornerSize;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	TRzImagePosition FImagePosition;
	TRzHorizontalAlignment FImageAlignment;
	int FImageMargin;
	TRzVerticalAlignment FImageAlignmentVertical;
	System::Uitypes::TColor FHotTrackColor;
	TRzTabHotTrackColorSource FHotTrackColorSource;
	Rzcommon::TRzHotTrackColorType FHotTrackColorType;
	bool FHotTrack;
	TRzTabHotTrackStyle FHotTrackStyle;
	bool FHotTracking;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	int FMargin;
	bool FMultiLine;
	bool FParentBackgroundColor;
	int FRowIndent;
	int FRowOverlap;
	System::Uitypes::TColor FScrollBtnArrowColor;
	System::Uitypes::TColor FScrollBtnFaceColor;
	Rzcommon::TOrientation FTextOrientation;
	TRzTextVerticalBaseline FTextVerticalBaseline;
	int FTabHeight;
	bool FTabHints;
	int FTabIndex;
	int FTabIndexDefault;
	TRzTabOrientation FTabOrientation;
	TRzTabSequence FTabSequence;
	TRzTabStyle FTabStyle;
	int FTabWidth;
	int FTabOverlap;
	bool FSoftCorners;
	int FRawDragTabIndex;
	TRzTabColors* FTabColors;
	TRzTextColors* FTextColors;
	TRzHorizontalAlignment FTextAlignment;
	TRzVerticalAlignment FTextAlignmentVertical;
	bool FShowShadow;
	bool FUseGradients;
	bool FTransparent;
	bool FShowFocusRect;
	bool FAllowTabDragging;
	System::Types::TRect FLastDragOverRect;
	int FLastMoveTabIndex;
	bool FMoveTabIndicatorVisible;
	System::Uitypes::TColor FDragIndicatorColor;
	System::Classes::TNotifyEvent FOnAlignControls;
	System::Classes::TNotifyEvent FOnPageChange;
	System::Classes::TNotifyEvent FOnChange;
	TRzTabChangingEvent FOnChanging;
	TRzTabCloseEvent FOnClose;
	TRzGetTextColorEvent FOnGetTextColor;
	TRzPaintBackgroundEvent FOnPaintBackground;
	TRzPaintCardBackgroundEvent FOnPaintCardBackground;
	TRzPaintTabBackgroundEvent FOnPaintTabBackground;
	System::Classes::TNotifyEvent FOnTabClick;
	TRzTabOrderChangeEvent FOnTabOrderChange;
	TRzTabDraggingEvent FOnTabDragging;
	System::Classes::TNotifyEvent FOnScrolledTabs;
	void __fastcall AddCommand(const int Command);
	void __fastcall AddCommandPt(const int Command, const System::Types::TPoint &APoint);
	void __fastcall AlignScroller();
	void __fastcall AlignCloseButton();
	void __fastcall AlignMenuButton();
	void __fastcall BringTabToFrontRow(TRzTabData* TabData);
	void __fastcall CalcCardDrawCommands(int ARow);
	System::Types::TSize __fastcall CalcDefaultTabFaceExtent(Vcl::Graphics::TFont* AFont);
	System::Types::TSize __fastcall CalcImageExtent(System::Uitypes::TImageIndex AImageIndex);
	System::Types::TRect __fastcall CalcMappedCardRect(int ARow);
	System::Types::TRect __fastcall CalcMappedTabRect(int ATabIndex, TRzTabData* TabData);
	void __fastcall CalcMappedTabRegionPts(int ATabIndex, TRzTabRegionPts &Pts, int &NumPts);
	System::Types::TPoint __fastcall CalcMapPoint(const System::Types::TPoint &RawPt);
	System::Types::TRect __fastcall CalcMapRect(const System::Types::TRect &RawRect);
	void __fastcall CalcMetrics();
	void __fastcall CalcScrollerNeeded();
	System::Types::TSize __fastcall CalcTabExtentFromTabFaceExtent(const System::Types::TSize &TabFaceExtent);
	System::Types::TRect __fastcall CalcTabFaceRect(const System::Types::TRect &ARect);
	HRGN __fastcall CalcTabRegion(int ATabIndex, const System::Types::TRect &ARect);
	void __fastcall CalcTabRegionPts(int ATabIndex, TRzTabRegionPts &Pts, int &NumPts);
	System::Types::TSize __fastcall CalcTextExtent(const System::UnicodeString S, bool Horizontal);
	void __fastcall CalcTabDrawCommands(int ATabIndex);
	System::Types::TRect __fastcall CalcTabRect(int ATabIndex, TRzTabData* TabData);
	System::Types::TRect __fastcall CalcWholeRect();
	void __fastcall CancelHotTrackTimer();
	bool __fastcall CanSelectTab(int ATabIndex);
	void __fastcall CheckCalcNeeded();
	void __fastcall CreateScroller();
	void __fastcall CreateCloseButton();
	void __fastcall CreateActiveTabCloseButton();
	void __fastcall CreateMenuButton();
	void __fastcall DeselectFont();
	void __fastcall DoRealign();
	void __fastcall DoTextOut(const System::Types::TRect &ARect, int X, int Y, const System::UnicodeString AString, Vcl::Graphics::TCanvas* ACanvas, bool Horizontal, System::Uitypes::TColor AColor);
	void __fastcall ProcessCommands();
	void __fastcall DrawTabBackground(int ATabIndex, const System::Types::TRect &ARect);
	void __fastcall DrawTabFace(int ATabIndex, const System::Types::TRect &ARect);
	void __fastcall DrawTabs();
	void __fastcall DrawControlButtons();
	System::Types::TRect __fastcall GetDisplayRect();
	int __fastcall GetExtraTopMargin();
	int __fastcall GetFirstVisible();
	int __fastcall GetIndexHeight();
	System::Types::TRect __fastcall GetIndexRect();
	int __fastcall GetIndexWidth();
	int __fastcall GetInitialTabOffset();
	int __fastcall GetLastVisible();
	int __fastcall GetScrollerWidth();
	int __fastcall GetScrollerHeight();
	int __fastcall GetCloseButtonWidth();
	int __fastcall GetCloseButtonHeight();
	int __fastcall GetMenuButtonWidth();
	int __fastcall GetMenuButtonHeight();
	int __fastcall GetTabOffset(int ATabHeight);
	System::UnicodeString __fastcall GetHint();
	void __fastcall SetHint(const System::UnicodeString Value);
	bool __fastcall IsBackgroundColorStored();
	void __fastcall ParseTextLines(const System::UnicodeString S, System::Classes::TStrings* Lines);
	HFONT __fastcall PopHFont();
	void __fastcall PushHFont(HFONT Value);
	void __fastcall ScrollTabs(bool Next);
	void __fastcall SelectFont();
	void __fastcall StopHotTracking();
	void __fastcall ReadFixedDimension(System::Classes::TReader* Reader);
	void __fastcall WriteFixedDimension(System::Classes::TWriter* Writer);
	void __fastcall ImagesChange(System::TObject* Sender);
	void __fastcall ScrollUpRightClickHandler(System::TObject* Sender);
	void __fastcall ScrollDownLeftClickHandler(System::TObject* Sender);
	void __fastcall CloseButtonClickHandler(System::TObject* Sender);
	void __fastcall ScrollTimerExpired(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMParentColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	MESSAGE void __fastcall WMTimer(Winapi::Messages::TWMTimer &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	virtual void __fastcall Changing(int NewIndex, bool &Allowed);
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	TRzTabDataList* __fastcall GetTabDataList();
	virtual void __fastcall GetTextColor(int ATabIndex, TRzTextStyle AStyle, System::Uitypes::TColor &AColor, bool &Handled);
	virtual void __fastcall AdjustClientRect(System::Types::TRect &Rect);
	virtual void __fastcall Rebuild();
	virtual void __fastcall TabClick();
	bool __fastcall ShowAccel();
	bool __fastcall ShowFocus();
	bool __fastcall TabInView(TRzTabData* TabData);
	HIDESBASE virtual void __fastcall InvalidateControl();
	virtual void __fastcall Paint();
	virtual void __fastcall PaintBackground(Vcl::Graphics::TCanvas* ACanvas, const System::Types::TRect &ARect, bool &Handled);
	virtual void __fastcall PaintCardBackground(Vcl::Graphics::TCanvas* ACanvas, int ARow, const System::Types::TRect &ARect, bool &Handled);
	virtual void __fastcall PaintTabBackground(Vcl::Graphics::TCanvas* ACanvas, int ATabIndex, const System::Types::TRect &ARect, bool &Handled);
	virtual void __fastcall DrawMoveTabIndicator(const System::Types::TRect &R);
	virtual void __fastcall DestroyActiveTab();
	virtual void __fastcall ActiveTabMoved(int Index);
	virtual bool __fastcall UpdatingTabs();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DragOver(System::TObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	DYNAMIC void __fastcall TabOrderChange(int OldIndex, int NewIndex);
	DYNAMIC bool __fastcall CanDragTab(int TabIndex);
	DYNAMIC void __fastcall PageChange();
	DYNAMIC bool __fastcall CanClose();
	virtual void __fastcall SetAlignTabs(bool Value);
	virtual void __fastcall SetBackgroundColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBoldCurrentTab(bool Value);
	virtual void __fastcall SetButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonColorDisabled(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonSymbolColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetButtonSymbolColorDisabled(System::Uitypes::TColor Value);
	virtual void __fastcall SetCalcNeeded(bool Value);
	virtual void __fastcall SetShowCard(bool Value);
	virtual void __fastcall SetShowCardFrame(bool Value);
	virtual void __fastcall SetShowFullFrame(bool Value);
	virtual void __fastcall SetShowCloseButton(bool Value);
	virtual void __fastcall SetShowCloseButtonOnActiveTab(bool Value);
	virtual void __fastcall SetShowMenuButton(bool Value);
	virtual void __fastcall SetUseColoredTabs(bool Value);
	virtual void __fastcall SetLightenUnselectedColoredTabs(bool Value);
	virtual void __fastcall SetCutCornerSize(int Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetImagePosition(TRzImagePosition Value);
	virtual void __fastcall SetImageAlignment(TRzHorizontalAlignment Value);
	virtual void __fastcall SetImageMargin(int Value);
	virtual void __fastcall SetImageAlignmentVertical(TRzVerticalAlignment Value);
	virtual void __fastcall SetHotTrack(bool Value);
	virtual void __fastcall SetHotTrackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHotTrackColorSource(TRzTabHotTrackColorSource Value);
	virtual void __fastcall SetHotTrackColorType(Rzcommon::TRzHotTrackColorType Value);
	virtual void __fastcall SetHotTrackStyle(TRzTabHotTrackStyle Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetMargin(int Value);
	virtual void __fastcall SetMultiLine(bool Value);
	virtual void __fastcall SetParentBackgroundColor(bool Value);
	virtual void __fastcall SetRowExtent(int Value);
	virtual void __fastcall SetRowIndent(int Value);
	virtual void __fastcall SetRowOverlap(int Value);
	virtual void __fastcall SetShowFocusRect(bool Value);
	virtual void __fastcall SetShowShadow(bool Value);
	virtual void __fastcall SetSoftCorners(bool Value);
	virtual void __fastcall SetTabOverlap(int Value);
	virtual void __fastcall SetTextOrientation(Rzcommon::TOrientation Value);
	virtual void __fastcall SetTabColors(TRzTabColors* Value);
	virtual void __fastcall SetTabHeight(int Value);
	virtual void __fastcall SetTabHints(bool Value);
	virtual void __fastcall SetTabIndex(int Value);
	virtual void __fastcall SetTabOrientation(TRzTabOrientation Value);
	virtual void __fastcall SetTabRegionCacheSize(int Value);
	virtual void __fastcall SetTabSequence(TRzTabSequence Value);
	virtual void __fastcall SetTabStyle(TRzTabStyle Value);
	virtual void __fastcall SetTabWidth(int Value);
	virtual void __fastcall SetTextColors(TRzTextColors* Value);
	virtual void __fastcall SetTextAlignment(TRzHorizontalAlignment Value);
	virtual void __fastcall SetTextAlignmentVertical(TRzVerticalAlignment Value);
	virtual void __fastcall SetTextVerticalBaseline(TRzTextVerticalBaseline Value);
	virtual void __fastcall SetUseGradients(bool Value);
	virtual void __fastcall SetTransparent(bool Value);
	__property bool AlignTabs = {read=FAlignTabs, write=SetAlignTabs, default=0};
	__property bool AllowTabDragging = {read=FAllowTabDragging, write=FAllowTabDragging, default=0};
	__property System::Uitypes::TColor BackgroundColor = {read=FBackgroundColor, write=SetBackgroundColor, stored=IsBackgroundColorStored, nodefault};
	__property bool BoldCurrentTab = {read=FBoldCurrentTab, write=SetBoldCurrentTab, default=0};
	__property System::Uitypes::TColor ButtonColor = {read=FButtonColor, write=SetButtonColor, default=-16777201};
	__property System::Uitypes::TColor ButtonColorDisabled = {read=FButtonColorDisabled, write=SetButtonColorDisabled, default=536870911};
	__property System::Uitypes::TColor ButtonSymbolColor = {read=FButtonSymbolColor, write=SetButtonSymbolColor, default=536870911};
	__property System::Uitypes::TColor ButtonSymbolColorDisabled = {read=FButtonSymbolColorDisabled, write=SetButtonSymbolColorDisabled, default=536870911};
	__property int CutCornerSize = {read=FCutCornerSize, write=SetCutCornerSize, default=6};
	__property System::Types::TRect DisplayRect = {read=GetDisplayRect};
	__property System::Uitypes::TColor DragIndicatorColor = {read=FDragIndicatorColor, write=FDragIndicatorColor, default=5263440};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=0};
	__property TRzHorizontalAlignment ImageAlignment = {read=FImageAlignment, write=SetImageAlignment, default=1};
	__property int ImageMargin = {read=FImageMargin, write=SetImageMargin, default=2};
	__property TRzImagePosition ImagePosition = {read=FImagePosition, write=SetImagePosition, default=0};
	__property TRzVerticalAlignment ImageAlignmentVertical = {read=FImageAlignmentVertical, write=SetImageAlignmentVertical, default=1};
	__property System::UnicodeString Hint = {read=GetHint, write=SetHint};
	__property bool HotTrack = {read=FHotTrack, write=SetHotTrack, default=1};
	__property System::Uitypes::TColor HotTrackColor = {read=FHotTrackColor, write=SetHotTrackColor, default=1350640};
	__property TRzTabHotTrackColorSource HotTrackColorSource = {read=FHotTrackColorSource, write=SetHotTrackColorSource, default=0};
	__property Rzcommon::TRzHotTrackColorType HotTrackColorType = {read=FHotTrackColorType, write=SetHotTrackColorType, default=1};
	__property int HotTrackIndex = {read=FHotTrackIndex, nodefault};
	__property TRzTabHotTrackStyle HotTrackStyle = {read=FHotTrackStyle, write=SetHotTrackStyle, default=0};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property int Margin = {read=FMargin, write=SetMargin, default=0};
	__property bool MultiLine = {read=FMultiLine, write=SetMultiLine, default=0};
	__property bool ParentBackgroundColor = {read=FParentBackgroundColor, write=SetParentBackgroundColor, default=1};
	__property int RowIndent = {read=FRowIndent, write=SetRowIndent, default=5};
	__property int RowOverlap = {read=FRowOverlap, write=SetRowOverlap, default=5};
	__property TRzTabScroller* Scroller = {read=FScroller, stored=false};
	__property TRzTabControlCloseButton* CloseButton = {read=FCloseButton, stored=false};
	__property TRzActiveTabCloseButton* ActiveTabCloseButton = {read=FActiveTabCloseButton, stored=false};
	__property TRzTabMenuButton* MenuButton = {read=FMenuButton, stored=false};
	__property bool ShowCard = {read=FShowCard, write=SetShowCard, default=1};
	__property bool ShowCardFrame = {read=FShowCardFrame, write=SetShowCardFrame, default=1};
	__property bool ShowCloseButton = {read=FShowCloseButton, write=SetShowCloseButton, default=0};
	__property bool ShowCloseButtonOnActiveTab = {read=FShowCloseButtonOnActiveTab, write=SetShowCloseButtonOnActiveTab, default=0};
	__property bool ShowMenuButton = {read=FShowMenuButton, write=SetShowMenuButton, default=0};
	__property bool SortTabMenu = {read=FSortTabMenu, write=FSortTabMenu, default=1};
	__property bool ShowFocusRect = {read=FShowFocusRect, write=SetShowFocusRect, default=1};
	__property bool ShowFullFrame = {read=FShowFullFrame, write=SetShowFullFrame, default=1};
	__property bool ShowShadow = {read=FShowShadow, write=SetShowShadow, default=1};
	__property bool SoftCorners = {read=FSoftCorners, write=SetSoftCorners, default=0};
	__property int TabOverlap = {read=FTabOverlap, write=SetTabOverlap, default=-1};
	__property Rzcommon::TOrientation TextOrientation = {read=FTextOrientation, write=SetTextOrientation, default=0};
	__property TRzTabColors* TabColors = {read=FTabColors, write=SetTabColors};
	__property int TabHeight = {read=FTabHeight, write=SetTabHeight, default=0};
	__property bool TabHints = {read=FTabHints, write=SetTabHints, default=0};
	__property int TabIndex = {read=FTabIndex, write=SetTabIndex, default=-1};
	__property int TabIndexDefault = {read=FTabIndexDefault, write=FTabIndexDefault, default=0};
	__property TRzTabOrientation TabOrientation = {read=FTabOrientation, write=SetTabOrientation, default=0};
	__property int TabRegionCacheSize = {read=FTabRegionCacheSize, write=SetTabRegionCacheSize, default=1};
	__property TRzTabSequence TabSequence = {read=FTabSequence, write=SetTabSequence, default=0};
	__property TRzTabStyle TabStyle = {read=FTabStyle, write=SetTabStyle, default=0};
	__property int TabWidth = {read=FTabWidth, write=SetTabWidth, default=0};
	__property TRzTextColors* TextColors = {read=FTextColors, write=SetTextColors};
	__property TRzHorizontalAlignment TextAlignment = {read=FTextAlignment, write=SetTextAlignment, default=1};
	__property TRzVerticalAlignment TextAlignmentVertical = {read=FTextAlignmentVertical, write=SetTextAlignmentVertical, default=1};
	__property TRzTextVerticalBaseline TextVerticalBaseline = {read=FTextVerticalBaseline, write=SetTextVerticalBaseline, default=0};
	__property bool UseColoredTabs = {read=FUseColoredTabs, write=SetUseColoredTabs, default=0};
	__property bool LightenUnselectedColoredTabs = {read=FLightenUnselectedColoredTabs, write=SetLightenUnselectedColoredTabs, default=1};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property bool UseGradients = {read=FUseGradients, write=SetUseGradients, default=1};
	__property System::Classes::TNotifyEvent OnAlignControls = {read=FOnAlignControls, write=FOnAlignControls};
	__property System::Classes::TNotifyEvent OnPageChange = {read=FOnPageChange, write=FOnPageChange};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
	__property TRzTabChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property TRzTabCloseEvent OnClose = {read=FOnClose, write=FOnClose};
	__property TRzGetTextColorEvent OnGetTextColor = {read=FOnGetTextColor, write=FOnGetTextColor};
	__property TRzPaintBackgroundEvent OnPaintBackground = {read=FOnPaintBackground, write=FOnPaintBackground};
	__property TRzPaintCardBackgroundEvent OnPaintCardBackground = {read=FOnPaintCardBackground, write=FOnPaintCardBackground};
	__property TRzPaintTabBackgroundEvent OnPaintTabBackground = {read=FOnPaintTabBackground, write=FOnPaintTabBackground};
	__property System::Classes::TNotifyEvent OnTabClick = {read=FOnTabClick, write=FOnTabClick};
	__property TRzTabDraggingEvent OnTabDragging = {read=FOnTabDragging, write=FOnTabDragging};
	__property TRzTabOrderChangeEvent OnTabOrderChange = {read=FOnTabOrderChange, write=FOnTabOrderChange};
	__property System::Classes::TNotifyEvent OnScrolledTabs = {read=FOnScrolledTabs, write=FOnScrolledTabs};
	
public:
	__fastcall virtual TRzCustomTabControl(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomTabControl();
	virtual void __fastcall Change();
	void __fastcall CloseActiveTab();
	virtual void __fastcall HideAllTabs();
	virtual void __fastcall ShowAllTabs();
	int __fastcall GetExtentOfAllTabs();
	int __fastcall TabAtPos(int X, int Y);
	bool __fastcall TabIndexInView(int Index);
	DYNAMIC void __fastcall DragDrop(System::TObject* Source, int X, int Y);
	__property TabStop = {default=1};
	__property int FixedDimension = {read=FFixedDimension, nodefault};
	__property int RowExtent = {read=FRowExtent, nodefault};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomTabControl(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabData : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	System::UnicodeString FCaption;
	System::Uitypes::TColor FColor;
	System::Uitypes::TImageIndex FDisabledIndex;
	bool FEnabled;
	System::UnicodeString FHint;
	System::Uitypes::TImageIndex FImageIndex;
	System::Types::TRect FRawRect;
	bool FVisible;
	int FRow;
	
public:
	__fastcall TRzTabData();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property System::UnicodeString Caption = {read=FCaption, write=FCaption};
	__property System::Uitypes::TColor Color = {read=FColor, write=FColor, default=-16777201};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=FDisabledIndex, default=-1};
	__property bool Enabled = {read=FEnabled, write=FEnabled, default=1};
	__property System::UnicodeString Hint = {read=FHint, write=FHint};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=FImageIndex, default=-1};
	__property System::Types::TRect RawRect = {read=FRawRect, write=FRawRect};
	__property int Row = {read=FRow, write=FRow, default=0};
	__property bool Visible = {read=FVisible, write=FVisible, default=1};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzTabData() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabDataList : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
public:
	TRzTabData* operator[](int Index) { return this->Items[Index]; }
	
private:
	System::Classes::TList* FTabList;
	
protected:
	virtual int __fastcall GetCount();
	virtual TRzTabData* __fastcall GetItem(int Index);
	virtual void __fastcall SetItem(int Index, TRzTabData* Value);
	
public:
	__fastcall TRzTabDataList();
	__fastcall virtual ~TRzTabDataList();
	int __fastcall Add(TRzTabData* Item);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	void __fastcall Clear();
	void __fastcall Delete(int Index);
	TRzTabData* __fastcall First();
	int __fastcall IndexOf(TRzTabData* Item);
	void __fastcall Insert(int Index, TRzTabData* Item);
	TRzTabData* __fastcall Last();
	void __fastcall Move(int CurIndex, int NewIndex);
	int __fastcall Remove(TRzTabData* Item);
	__property int Count = {read=GetCount, nodefault};
	__property TRzTabData* Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzTabSheet : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	System::Uitypes::TImageIndex FDisabledIndex;
	System::Uitypes::TImageIndex FImageIndex;
	TRzPageControl* FPageControl;
	bool FTabEnabled;
	bool FTabVisible;
	TRzPaintBackgroundEvent FOnPaintBackground;
	System::Classes::TNotifyEvent FOnHide;
	System::Classes::TNotifyEvent FOnShow;
	void *FData;
	void __fastcall AssignToTabData(System::Classes::TPersistent* Dest);
	HIDESBASE void __fastcall Changed();
	void __fastcall InternalSetVisible(bool Value);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	HIDESBASE MESSAGE void __fastcall CMShowingChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall AssignTo(System::Classes::TPersistent* Dest);
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall DestroyHandle();
	virtual void __fastcall Paint();
	void __fastcall PaintBackground(Vcl::Graphics::TCanvas* ACanvas, const System::Types::TRect &ARect, bool &Handled);
	virtual void __fastcall ReadState(System::Classes::TReader* Reader);
	DYNAMIC void __fastcall DoHide();
	DYNAMIC void __fastcall DoShow();
	virtual System::Uitypes::TColor __fastcall GetColor();
	HIDESBASE virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetPageControl(TRzPageControl* APageControl);
	virtual int __fastcall GetPageIndex();
	virtual void __fastcall SetPageIndex(int Value);
	virtual void __fastcall SetTabEnabled(bool Value);
	virtual int __fastcall GetTabIndex();
	virtual void __fastcall SetTabVisible(bool Value);
	virtual bool __fastcall GetVisible();
	HIDESBASE virtual void __fastcall SetVisible(bool Value);
	
public:
	__fastcall virtual TRzTabSheet(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzTabSheet();
	__property void * Data = {read=FData, write=FData};
	__property TRzPageControl* PageControl = {read=FPageControl, write=SetPageControl};
	__property int TabIndex = {read=GetTabIndex, nodefault};
	__property bool Visible = {read=GetVisible, write=SetVisible, nodefault};
	
__published:
	__property System::Uitypes::TColor Color = {read=GetColor, write=SetColor, default=-16777201};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, default=-1};
	__property int PageIndex = {read=GetPageIndex, write=SetPageIndex, stored=false, nodefault};
	__property bool TabEnabled = {read=FTabEnabled, write=SetTabEnabled, default=1};
	__property bool TabVisible = {read=FTabVisible, write=SetTabVisible, default=1};
	__property TRzPaintBackgroundEvent OnPaintBackground = {read=FOnPaintBackground, write=FOnPaintBackground};
	__property System::Classes::TNotifyEvent OnHide = {read=FOnHide, write=FOnHide};
	__property System::Classes::TNotifyEvent OnShow = {read=FOnShow, write=FOnShow};
	__property Caption = {default=0};
	__property Constraints;
	__property Enabled = {default=1};
	__property Font;
	__property Height = {stored=false};
	__property Left = {stored=false};
	__property Padding;
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Top = {stored=false};
	__property Touch;
	__property Width = {stored=false};
	__property OnClick;
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
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabSheet(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzPageControl : public TRzCustomTabControl
{
	typedef TRzCustomTabControl inherited;
	
private:
	TRzTabSheet* FActivePage;
	TRzTabSheet* FActivePageDefault;
	TRzTabSheet* FNewDockSheet;
	TRzTabSheet* FUndockingPage;
	bool FDblClickUndocks;
	System::Classes::TList* FPages;
	bool FSaveResources;
	bool FInitActivePage;
	void __fastcall ChangeActivePage(TRzTabSheet* Page);
	void __fastcall InsertPage(TRzTabSheet* Page);
	void __fastcall RemovePage(TRzTabSheet* Page);
	Vcl::Controls::TControl* __fastcall GetDockClientFromMousePos(const System::Types::TPoint &MousePos);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogKey(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMDockClient(Vcl::Controls::TCMDockClient &Msg);
	MESSAGE void __fastcall CMDockNotification(Vcl::Controls::TCMDockNotification &Msg);
	HIDESBASE MESSAGE void __fastcall CMUnDockClient(Vcl::Controls::TCMUnDockClient &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMLButtonDblClk(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	
protected:
	virtual void __fastcall Changing(int NewIndex, bool &Allowed);
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DestroyActiveTab();
	virtual void __fastcall ActiveTabMoved(int Index);
	virtual void __fastcall PaintCardBackground(Vcl::Graphics::TCanvas* ACanvas, int ARow, const System::Types::TRect &ARect, bool &Handled);
	virtual void __fastcall Rebuild();
	DYNAMIC void __fastcall SetChildOrder(System::Classes::TComponent* Child, int Order);
	virtual void __fastcall ShowControl(Vcl::Controls::TControl* AControl);
	DYNAMIC void __fastcall DoAddDockClient(Vcl::Controls::TControl* Client, const System::Types::TRect &ARect);
	DYNAMIC void __fastcall DockOver(Vcl::Controls::TDragDockObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	DYNAMIC void __fastcall DoRemoveDockClient(Vcl::Controls::TControl* Client);
	TRzTabSheet* __fastcall GetPageFromDockClient(Vcl::Controls::TControl* Client);
	DYNAMIC void __fastcall GetSiteInfo(Vcl::Controls::TControl* Client, System::Types::TRect &InfluenceRect, const System::Types::TPoint &MousePos, bool &CanDock);
	virtual TRzTabSheet* __fastcall GetPage(int Index);
	virtual int __fastcall GetPageCount();
	virtual void __fastcall SetActivePage(TRzTabSheet* Page);
	virtual int __fastcall GetActivePageIndex();
	virtual void __fastcall SetActivePageIndex(int Value);
	virtual void __fastcall SetUseColoredTabs(bool Value);
	virtual void __fastcall SetSaveResources(bool Value);
	
public:
	__fastcall virtual TRzPageControl(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzPageControl();
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	DYNAMIC void __fastcall PageChange();
	virtual void __fastcall Change();
	TRzTabSheet* __fastcall FindNextPage(TRzTabSheet* CurPage, bool GoForward, bool CheckTabVisible);
	TRzTabSheet* __fastcall PageForTab(int ATabIndex);
	void __fastcall SelectNextPage(bool GoForward);
	virtual void __fastcall HideAllTabs();
	virtual void __fastcall ShowAllTabs();
	void __fastcall MakeControlVisible(Vcl::Controls::TControl* AControl);
	__property int PageCount = {read=GetPageCount, nodefault};
	__property TRzTabSheet* Pages[int Index] = {read=GetPage};
	__property int ActivePageIndex = {read=GetActivePageIndex, write=SetActivePageIndex, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzTabSheet* ActivePage = {read=FActivePage, write=SetActivePage};
	__property TRzTabSheet* ActivePageDefault = {read=FActivePageDefault, write=FActivePageDefault};
	__property bool SaveResources = {read=FSaveResources, write=SetSaveResources, default=0};
	__property bool DblClickUndocks = {read=FDblClickUndocks, write=FDblClickUndocks, default=1};
	__property Align = {default=0};
	__property AlignTabs = {default=0};
	__property AllowTabDragging = {default=0};
	__property Anchors = {default=3};
	__property BackgroundColor;
	__property BoldCurrentTab = {default=0};
	__property ButtonColor = {default=-16777201};
	__property ButtonColorDisabled = {default=536870911};
	__property ButtonSymbolColor = {default=536870911};
	__property ButtonSymbolColorDisabled = {default=536870911};
	__property Color = {default=-16777201};
	__property UseColoredTabs = {default=0};
	__property LightenUnselectedColoredTabs = {default=1};
	__property Constraints;
	__property CutCornerSize = {default=6};
	__property DockSite = {default=0};
	__property DragCursor = {default=-12};
	__property DragIndicatorColor = {default=5263440};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
	__property ImageAlignment = {default=1};
	__property ImageMargin = {default=2};
	__property ImagePosition = {default=0};
	__property ImageAlignmentVertical = {default=1};
	__property HelpContext = {default=0};
	__property Hint;
	__property HotTrack = {default=1};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorSource = {default=0};
	__property HotTrackColorType = {default=1};
	__property HotTrackIndex;
	__property HotTrackStyle = {default=0};
	__property Images;
	__property Margin = {default=0};
	__property MultiLine = {default=0};
	__property ParentBackgroundColor = {default=1};
	__property ParentColor = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property RowIndent = {default=5};
	__property RowOverlap = {default=5};
	__property ShowCardFrame = {default=1};
	__property ShowCloseButton = {default=0};
	__property ShowCloseButtonOnActiveTab = {default=0};
	__property ShowMenuButton = {default=0};
	__property SortTabMenu = {default=1};
	__property ShowFocusRect = {default=1};
	__property ShowFullFrame = {default=1};
	__property ShowHint;
	__property ShowShadow = {default=1};
	__property SoftCorners = {default=0};
	__property TabOverlap = {default=-1};
	__property TextOrientation = {default=0};
	__property TabColors;
	__property TabHeight = {default=0};
	__property TabHints = {default=0};
	__property TabIndex = {default=-1};
	__property TabOrder = {default=-1};
	__property TabOrientation = {default=0};
	__property TabSequence = {default=0};
	__property TabStop = {default=1};
	__property TabStyle = {default=0};
	__property TabWidth = {default=0};
	__property TextColors;
	__property TextAlignment = {default=1};
	__property TextAlignmentVertical = {default=1};
	__property TextVerticalBaseline = {default=0};
	__property Touch;
	__property Transparent = {default=0};
	__property UseGradients = {default=1};
	__property Visible = {default=1};
	__property OnChange;
	__property OnChanging;
	__property OnClick;
	__property OnClose;
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
	__property OnGetTextColor;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnAlignControls;
	__property OnPageChange;
	__property OnPaintBackground;
	__property OnPaintCardBackground;
	__property OnPaintTabBackground;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnTabClick;
	__property OnTabDragging;
	__property OnTabOrderChange;
	__property OnScrolledTabs;
	__property OnUnDock;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPageControl(HWND ParentWindow) : TRzCustomTabControl(ParentWindow) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabCollectionItem : public System::Classes::TCollectionItem
{
	typedef System::Classes::TCollectionItem inherited;
	
private:
	System::UnicodeString FCaption;
	System::Uitypes::TColor FColor;
	System::Uitypes::TImageIndex FDisabledIndex;
	bool FEnabled;
	System::UnicodeString FHint;
	System::Uitypes::TImageIndex FImageIndex;
	bool FVisible;
	int FTag;
	void *FData;
	void __fastcall AssignToTabData(System::Classes::TPersistent* Dest);
	
protected:
	virtual System::UnicodeString __fastcall GetDisplayName();
	virtual void __fastcall AssignTo(System::Classes::TPersistent* Dest);
	virtual void __fastcall SetCaption(const System::UnicodeString Value);
	virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetEnabled(bool Value);
	virtual void __fastcall SetHint(const System::UnicodeString Value);
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetVisible(bool Value);
	TRzCollectionTabControl* __fastcall GetTabControl();
	
public:
	__fastcall virtual TRzTabCollectionItem(System::Classes::TCollection* Collection);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	__property void * Data = {read=FData, write=FData};
	__property TRzCollectionTabControl* TabControl = {read=GetTabControl};
	
__published:
	__property System::UnicodeString Caption = {read=FCaption, write=SetCaption};
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, default=-16777201};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property bool Enabled = {read=FEnabled, write=SetEnabled, default=1};
	__property System::UnicodeString Hint = {read=FHint, write=SetHint};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, default=-1};
	__property int Tag = {read=FTag, write=FTag, default=0};
	__property bool Visible = {read=FVisible, write=SetVisible, default=1};
public:
	/* TCollectionItem.Destroy */ inline __fastcall virtual ~TRzTabCollectionItem() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabCollection : public System::Classes::TCollection
{
	typedef System::Classes::TCollection inherited;
	
public:
	TRzTabCollectionItem* operator[](int Index) { return this->Items[Index]; }
	
private:
	TRzCollectionTabControl* FTabControl;
	
protected:
	DYNAMIC System::Classes::TPersistent* __fastcall GetOwner();
	virtual void __fastcall Update(System::Classes::TCollectionItem* Item);
	HIDESBASE virtual TRzTabCollectionItem* __fastcall GetItem(int Index);
	HIDESBASE virtual void __fastcall SetItem(int Index, TRzTabCollectionItem* Value);
	
public:
	__fastcall TRzTabCollection(TRzCollectionTabControl* TabControl);
	HIDESBASE TRzTabCollectionItem* __fastcall Add();
	HIDESBASE TRzTabCollectionItem* __fastcall Insert(int Index);
	HIDESBASE void __fastcall Delete(int Index);
	void __fastcall Move(int CurIndex, int NewIndex);
	HIDESBASE void __fastcall Clear();
	__property TRzTabCollectionItem* Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
	__property TRzCollectionTabControl* TabControl = {read=FTabControl};
public:
	/* TCollection.Destroy */ inline __fastcall virtual ~TRzTabCollection() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzCollectionTabControl : public TRzCustomTabControl
{
	typedef TRzCustomTabControl inherited;
	
private:
	TRzTabCollection* FTabCollection;
	TRzTabCollection* __fastcall GetTabCollection();
	void __fastcall SetTabCollection(TRzTabCollection* Value);
	
protected:
	virtual void __fastcall Rebuild();
	virtual void __fastcall DestroyActiveTab();
	virtual void __fastcall ActiveTabMoved(int Index);
	virtual bool __fastcall UpdatingTabs();
	
public:
	__fastcall virtual TRzCollectionTabControl(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCollectionTabControl();
	virtual void __fastcall HideAllTabs();
	virtual void __fastcall ShowAllTabs();
	__property TabIndex = {default=-1};
	__property TRzTabCollection* Tabs = {read=GetTabCollection, write=SetTabCollection};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCollectionTabControl(HWND ParentWindow) : TRzCustomTabControl(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzTabControl : public TRzCollectionTabControl
{
	typedef TRzCollectionTabControl inherited;
	
public:
	__property DisplayRect;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property AlignTabs = {default=0};
	__property AllowTabDragging = {default=0};
	__property Anchors = {default=3};
	__property BackgroundColor;
	__property BoldCurrentTab = {default=0};
	__property ButtonColor = {default=-16777201};
	__property ButtonColorDisabled = {default=536870911};
	__property ButtonSymbolColor = {default=536870911};
	__property ButtonSymbolColorDisabled = {default=536870911};
	__property Color = {default=-16777201};
	__property UseColoredTabs = {default=0};
	__property LightenUnselectedColoredTabs = {default=1};
	__property Constraints;
	__property CutCornerSize = {default=6};
	__property DragIndicatorColor = {default=5263440};
	__property Enabled = {default=1};
	__property Font;
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=0};
	__property ImageAlignment = {default=1};
	__property ImageMargin = {default=2};
	__property ImagePosition = {default=0};
	__property ImageAlignmentVertical = {default=1};
	__property HelpContext = {default=0};
	__property Hint;
	__property HotTrack = {default=1};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorSource = {default=0};
	__property HotTrackColorType = {default=1};
	__property HotTrackIndex;
	__property HotTrackStyle = {default=0};
	__property Images;
	__property Margin = {default=0};
	__property MultiLine = {default=0};
	__property Padding;
	__property ParentBackgroundColor = {default=1};
	__property ParentColor = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property RowIndent = {default=5};
	__property RowOverlap = {default=5};
	__property ShowCard = {default=1};
	__property ShowCardFrame = {default=1};
	__property ShowCloseButton = {default=0};
	__property ShowCloseButtonOnActiveTab = {default=0};
	__property ShowMenuButton = {default=0};
	__property SortTabMenu = {default=1};
	__property ShowFocusRect = {default=1};
	__property ShowFullFrame = {default=1};
	__property ShowHint;
	__property ShowShadow = {default=1};
	__property SoftCorners = {default=0};
	__property TabOverlap = {default=-1};
	__property TextOrientation = {default=0};
	__property TabColors;
	__property TabHeight = {default=0};
	__property TabHints = {default=0};
	__property TabIndex = {default=-1};
	__property TabIndexDefault = {default=0};
	__property TabOrder = {default=-1};
	__property TabOrientation = {default=0};
	__property Tabs;
	__property TabSequence = {default=0};
	__property TabStop = {default=1};
	__property TabStyle = {default=0};
	__property TabWidth = {default=0};
	__property TextColors;
	__property TextAlignment = {default=1};
	__property TextAlignmentVertical = {default=1};
	__property TextVerticalBaseline = {default=0};
	__property Touch;
	__property Transparent = {default=0};
	__property UseGradients = {default=1};
	__property Visible = {default=1};
	__property OnChange;
	__property OnChanging;
	__property OnClick;
	__property OnClose;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
	__property OnGetTextColor;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnPaintBackground;
	__property OnPaintCardBackground;
	__property OnPaintTabBackground;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnTabClick;
	__property OnTabDragging;
	__property OnTabOrderChange;
	__property OnScrolledTabs;
	__property OnUnDock;
public:
	/* TRzCollectionTabControl.Create */ inline __fastcall virtual TRzTabControl(System::Classes::TComponent* AOwner) : TRzCollectionTabControl(AOwner) { }
	/* TRzCollectionTabControl.Destroy */ inline __fastcall virtual ~TRzTabControl() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTabControl(HWND ParentWindow) : TRzCollectionTabControl(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
static const System::Int8 MaxTabRegionPts = System::Int8(0x14);
static const System::Int8 OptimumTabRegionCacheSize = System::Int8(0x1);
static const int DefaultDragIndicatorColor = int(0x505050);
}	/* namespace Rztabs */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTABS)
using namespace Rztabs;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztabsHPP
