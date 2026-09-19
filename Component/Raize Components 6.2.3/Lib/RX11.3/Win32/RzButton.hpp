// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzButton.pas' rev: 35.00 (Windows)

#ifndef RzbuttonHPP
#define RzbuttonHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Windows.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Messages.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.ActnList.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <Vcl.Consts.hpp>
#include <System.Classes.hpp>
#include <RzCommon.hpp>
#include <System.Actions.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzbutton
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomButton;
class DELPHICLASS TRzButton;
class DELPHICLASS TRzBitBtn;
class DELPHICLASS TRzMenuButton;
class DELPHICLASS TRzToolbarButtonActionLink;
class DELPHICLASS TRzToolbarButton;
class DELPHICLASS TRzMenuToolbarButton;
class DELPHICLASS TRzToolButtonActionLink;
class DELPHICLASS TRzToolButton;
class DELPHICLASS TRzControlButton;
class DELPHICLASS TRzShapeButton;
//-- type declarations -------------------------------------------------------
typedef System::StaticArray<System::UnicodeString, 11> Rzbutton__1;

class PASCALIMPLEMENTATION TRzCustomButton : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	Rzcommon::TAlignmentVertical FAlignmentVertical;
	System::Uitypes::TColor FHotTrackColor;
	Rzcommon::TRzHotTrackColorType FHotTrackColorType;
	System::Uitypes::TColor FHighlightColor;
	bool FLightTextStyle;
	Rzcommon::TTextStyle FTextStyle;
	System::Uitypes::TColor FTextHighlightColor;
	System::Uitypes::TColor FTextShadowColor;
	int FTextShadowDepth;
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FDragging;
	bool FHotTrack;
	bool FMouseOverButton;
	bool FShowDownVersion;
	bool FTransparent;
	bool FThemeAware;
	virtual void __fastcall CreateWnd();
	virtual void __fastcall UpdateDisplay();
	virtual void __fastcall RepaintDisplay();
	virtual System::Types::TRect __fastcall GetHotTrackRect();
	HIDESBASE void __fastcall RemoveFocus(bool Removing);
	bool __fastcall ShowAccel();
	bool __fastcall ShowFocus();
	virtual bool __fastcall UseThemes();
	virtual System::Uitypes::TColor __fastcall GetStyleFontColor(bool Enabled);
	virtual void __fastcall Draw3DText(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &R, unsigned Flags);
	DYNAMIC void __fastcall Click();
	virtual void __fastcall ChangeState() = 0 ;
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetAlignmentVertical(Rzcommon::TAlignmentVertical Value);
	virtual void __fastcall SetHotTrack(bool Value);
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetLightTextStyle(bool Value);
	virtual void __fastcall SetTextStyle(Rzcommon::TTextStyle Value);
	virtual void __fastcall SetTextHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTextShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTextShadowDepth(int Value);
	virtual void __fastcall SetThemeAware(bool Value);
	virtual void __fastcall SetTransparent(bool Value);
	__property Rzcommon::TAlignmentVertical AlignmentVertical = {read=FAlignmentVertical, write=SetAlignmentVertical, default=1};
	__property bool HotTrack = {read=FHotTrack, write=SetHotTrack, default=0};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777203};
	__property System::Uitypes::TColor HotTrackColor = {read=FHotTrackColor, write=FHotTrackColor, default=1350640};
	__property Rzcommon::TRzHotTrackColorType HotTrackColorType = {read=FHotTrackColorType, write=FHotTrackColorType, default=1};
	__property System::Uitypes::TColor TextHighlightColor = {read=FTextHighlightColor, write=SetTextHighlightColor, default=-16777196};
	__property bool LightTextStyle = {read=FLightTextStyle, write=SetLightTextStyle, default=0};
	__property System::Uitypes::TColor TextShadowColor = {read=FTextShadowColor, write=SetTextShadowColor, default=-16777200};
	__property int TextShadowDepth = {read=FTextShadowDepth, write=SetTextShadowDepth, default=2};
	__property Rzcommon::TTextStyle TextStyle = {read=FTextStyle, write=SetTextStyle, default=0};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property bool ThemeAware = {read=FThemeAware, write=SetThemeAware, default=1};
	
public:
	__fastcall virtual TRzCustomButton(System::Classes::TComponent* AOwner);
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzCustomButton() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomButton(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzButton : public TRzCustomButton
{
	typedef TRzCustomButton inherited;
	
private:
	System::Classes::TAlignment FAlignment;
	bool FClicking;
	bool FDefault;
	bool FCancel;
	bool FActive;
	System::Uitypes::TModalResult FModalResult;
	bool FKeyToggle;
	bool FAllowAllUp;
	bool FDown;
	int FGroupIndex;
	bool FShowDownPattern;
	bool FShowFocusRect;
	bool FDropDownOnEnter;
	System::Uitypes::TColor FFrameColor;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	HIDESBASE MESSAGE void __fastcall CMDialogKey(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMFocusChanged(Vcl::Controls::TCMFocusChanged &Msg);
	MESSAGE void __fastcall CMRzButtonPressed(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMEraseBkgnd(Winapi::Messages::TWMEraseBkgnd &Msg);
	
protected:
	Vcl::Buttons::TButtonState FState;
	bool FKeyWasPressed;
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall UpdateDisplay();
	virtual void __fastcall ChangeState();
	virtual System::Types::TRect __fastcall GetCaptionRect();
	virtual void __fastcall DrawButtonContent();
	virtual void __fastcall CreateBrushPattern(Vcl::Graphics::TBitmap* PatternBmp);
	virtual void __fastcall Paint();
	void __fastcall UpdateExclusive();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	virtual void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetDefault(bool Value);
	virtual void __fastcall SetAllowAllUp(bool Value);
	virtual void __fastcall SetDown(bool Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetGroupIndex(int Value);
	virtual void __fastcall SetHotTrack(bool Value);
	virtual void __fastcall SetShowDownPattern(bool Value);
	virtual void __fastcall SetShowFocusRect(bool Value);
	
public:
	__fastcall virtual TRzButton(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall Click();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=SetAlignment, default=2};
	__property bool AllowAllUp = {read=FAllowAllUp, write=SetAllowAllUp, default=0};
	__property bool Cancel = {read=FCancel, write=FCancel, default=0};
	__property bool Default = {read=FDefault, write=SetDefault, default=0};
	__property bool DropDownOnEnter = {read=FDropDownOnEnter, write=FDropDownOnEnter, default=1};
	__property int GroupIndex = {read=FGroupIndex, write=SetGroupIndex, default=0};
	__property bool Down = {read=FDown, write=SetDown, default=0};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777195};
	__property System::Uitypes::TModalResult ModalResult = {read=FModalResult, write=FModalResult, default=0};
	__property bool ShowDownPattern = {read=FShowDownPattern, write=SetShowDownPattern, default=1};
	__property bool ShowFocusRect = {read=FShowFocusRect, write=SetShowFocusRect, default=1};
	__property Action;
	__property Align = {default=0};
	__property AlignmentVertical = {default=1};
	__property Anchors = {default=3};
	__property BiDiMode;
	__property Caption = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property DoubleBuffered;
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property DragCursor = {default=-12};
	__property Enabled = {default=1};
	__property Font;
	__property HelpContext = {default=0};
	__property Height = {default=25};
	__property HighlightColor = {default=-16777203};
	__property Hint = {default=0};
	__property HotTrack = {default=0};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorType = {default=1};
	__property LightTextStyle = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property TextHighlightColor = {default=-16777196};
	__property TextShadowColor = {default=-16777200};
	__property TextShadowDepth = {default=2};
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property TextStyle = {default=0};
	__property ThemeAware = {default=1};
	__property Visible = {default=1};
	__property Width = {default=75};
	__property OnClick;
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TCustomControl.Destroy */ inline __fastcall virtual ~TRzButton() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzButton(HWND ParentWindow) : TRzCustomButton(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzBitBtn : public TRzButton
{
	typedef TRzButton inherited;
	
private:
	Vcl::Graphics::TBitmap* FGlyph;
	Vcl::Buttons::TBitBtnKind FKind;
	Vcl::Buttons::TButtonLayout FLayout;
	int FMargin;
	Vcl::Buttons::TNumGlyphs FNumGlyphs;
	int FSpacing;
	bool FModifiedGlyph;
	System::Uitypes::TImageIndex FImageIndex;
	System::Uitypes::TImageIndex FDisabledIndex;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	void __fastcall GlyphChangedHandler(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	DYNAMIC HPALETTE __fastcall GetPalette();
	virtual System::Types::TPoint __fastcall GetImageSize();
	virtual System::Types::TRect __fastcall GetCaptionRect();
	virtual System::Types::TRect __fastcall GetGlyphRect();
	virtual void __fastcall DrawImage(const System::Types::TRect &R);
	virtual void __fastcall DrawGlyph(const System::Types::TRect &R);
	virtual void __fastcall DrawButtonContent();
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	bool __fastcall IsCustom();
	bool __fastcall IsCustomCaption();
	virtual void __fastcall SetGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Buttons::TBitBtnKind __fastcall GetKind();
	virtual void __fastcall SetKind(Vcl::Buttons::TBitBtnKind Value);
	virtual void __fastcall SetLayout(Vcl::Buttons::TButtonLayout Value);
	virtual void __fastcall SetMargin(int Value);
	virtual void __fastcall SetNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual void __fastcall SetSpacing(int Value);
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	
public:
	__fastcall virtual TRzBitBtn(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzBitBtn();
	
__published:
	__property Cancel = {stored=IsCustom, default=0};
	__property Caption = {stored=IsCustomCaption, default=0};
	__property Default = {stored=IsCustom, default=0};
	__property Vcl::Graphics::TBitmap* Glyph = {read=FGlyph, write=SetGlyph, stored=IsCustom};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, default=-1};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property Vcl::Buttons::TBitBtnKind Kind = {read=GetKind, write=SetKind, default=0};
	__property Vcl::Buttons::TButtonLayout Layout = {read=FLayout, write=SetLayout, default=0};
	__property int Margin = {read=FMargin, write=SetMargin, default=2};
	__property ModalResult = {stored=IsCustom, default=0};
	__property Vcl::Buttons::TNumGlyphs NumGlyphs = {read=FNumGlyphs, write=SetNumGlyphs, stored=IsCustom, default=1};
	__property int Spacing = {read=FSpacing, write=SetSpacing, default=4};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzBitBtn(HWND ParentWindow) : TRzButton(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzMenuButton : public TRzBitBtn
{
	typedef TRzBitBtn inherited;
	
private:
	bool FDropped;
	Vcl::Menus::TPopupMenu* FDropDownMenu;
	unsigned FDropTime;
	bool FSkipNextClick;
	bool FShowArrow;
	System::Classes::TNotifyEvent FOnDropDown;
	HIDESBASE MESSAGE void __fastcall WMKeyDown(Winapi::Messages::TWMKey &Msg);
	
protected:
	virtual System::Types::TRect __fastcall GetCaptionRect();
	virtual System::Types::TRect __fastcall GetGlyphRect();
	virtual void __fastcall Paint();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DoDropDown();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DropDown();
	void __fastcall SetDropDownMenu(Vcl::Menus::TPopupMenu* Value);
	virtual void __fastcall SetShowArrow(bool Value);
	
public:
	__fastcall virtual TRzMenuButton(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall Click();
	
__published:
	__property Vcl::Menus::TPopupMenu* DropDownMenu = {read=FDropDownMenu, write=SetDropDownMenu};
	__property bool ShowArrow = {read=FShowArrow, write=SetShowArrow, default=1};
	__property System::Classes::TNotifyEvent OnDropDown = {read=FOnDropDown, write=FOnDropDown};
	__property Width = {default=110};
public:
	/* TRzBitBtn.Destroy */ inline __fastcall virtual ~TRzMenuButton() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzMenuButton(HWND ParentWindow) : TRzBitBtn(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzToolbarButtonActionLink : public Vcl::Buttons::TSpeedButtonActionLink
{
	typedef Vcl::Buttons::TSpeedButtonActionLink inherited;
	
protected:
	virtual bool __fastcall IsCaptionLinked();
public:
	/* TSpeedButtonActionLink.Create */ inline __fastcall virtual TRzToolbarButtonActionLink(System::TObject* AClient) : Vcl::Buttons::TSpeedButtonActionLink(AClient) { }
	
public:
	/* TBasicActionLink.Destroy */ inline __fastcall virtual ~TRzToolbarButtonActionLink() { }
	
};


typedef System::TMetaClass* TRzToolbarButtonActionLinkClass;

class PASCALIMPLEMENTATION TRzToolbarButton : public Vcl::Buttons::TSpeedButton
{
	typedef Vcl::Buttons::TSpeedButton inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FChangingGlyph;
	Vcl::Graphics::TBitmap* FHotGlyph;
	Vcl::Buttons::TNumGlyphs FHotNumGlyphs;
	bool FUseHotGlyph;
	Vcl::Graphics::TBitmap* FStdGlyph;
	Vcl::Buttons::TNumGlyphs FStdNumGlyphs;
	bool FIgnoreActionCaption;
	Vcl::Controls::TCaption FSaveCaption;
	bool FShowCaption;
	void __fastcall ReadSaveCaption(System::Classes::TReader* Reader);
	void __fastcall WriteSaveCaption(System::Classes::TWriter* Writer);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	void __fastcall HotGlyphChangedHandler(System::TObject* Sender);
	
protected:
	bool FMouseOverControl;
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	DYNAMIC Vcl::Controls::TControlActionLinkClass __fastcall GetActionLinkClass();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetHotNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual void __fastcall SetHotGlyph(Vcl::Graphics::TBitmap* Value);
	virtual Vcl::Controls::TCaption __fastcall GetCaption();
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetShowCaption(bool Value);
	
public:
	__fastcall virtual TRzToolbarButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzToolbarButton();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Vcl::Graphics::TBitmap* HotGlyph = {read=FHotGlyph, write=SetHotGlyph};
	__property Vcl::Buttons::TNumGlyphs HotNumGlyphs = {read=FHotNumGlyphs, write=SetHotNumGlyphs, default=1};
	__property bool IgnoreActionCaption = {read=FIgnoreActionCaption, write=FIgnoreActionCaption, default=0};
	__property Vcl::Controls::TCaption Caption = {read=GetCaption, write=SetCaption};
	__property bool ShowCaption = {read=FShowCaption, write=SetShowCaption, default=1};
	__property bool UseHotGlyph = {read=FUseHotGlyph, write=FUseHotGlyph, default=0};
	__property Flat = {default=1};
};


class PASCALIMPLEMENTATION TRzMenuToolbarButton : public TRzToolbarButton
{
	typedef TRzToolbarButton inherited;
	
private:
	bool FDropped;
	Vcl::Menus::TPopupMenu* FDropDownMenu;
	unsigned FDropTime;
	bool FSkipNextClick;
	bool FShowArrow;
	System::Classes::TNotifyEvent FOnDropDown;
	
protected:
	virtual void __fastcall Paint();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DropDown();
	void __fastcall SetDropDownMenu(Vcl::Menus::TPopupMenu* Value);
	void __fastcall SetShowArrow(bool Value);
	
public:
	__fastcall virtual TRzMenuToolbarButton(System::Classes::TComponent* AOwner);
	DYNAMIC void __fastcall Click();
	virtual void __fastcall DoDropDown();
	
__published:
	__property Vcl::Menus::TPopupMenu* DropDownMenu = {read=FDropDownMenu, write=SetDropDownMenu};
	__property bool ShowArrow = {read=FShowArrow, write=SetShowArrow, default=1};
	__property System::Classes::TNotifyEvent OnDropDown = {read=FOnDropDown, write=FOnDropDown};
	__property DragMode = {default=0};
	__property Margin = {default=2};
	__property Width = {default=40};
public:
	/* TRzToolbarButton.Destroy */ inline __fastcall virtual ~TRzMenuToolbarButton() { }
	
};


class PASCALIMPLEMENTATION TRzToolButtonActionLink : public Vcl::Controls::TControlActionLink
{
	typedef Vcl::Controls::TControlActionLink inherited;
	
protected:
	TRzToolButton* FClient;
	virtual void __fastcall AssignClient(System::TObject* AClient);
	virtual bool __fastcall IsCheckedLinked();
	virtual bool __fastcall IsImageIndexLinked();
	virtual void __fastcall SetChecked(bool Value);
	virtual void __fastcall SetImageIndex(int Value);
public:
	/* TBasicActionLink.Create */ inline __fastcall virtual TRzToolButtonActionLink(System::TObject* AClient) : Vcl::Controls::TControlActionLink(AClient) { }
	/* TBasicActionLink.Destroy */ inline __fastcall virtual ~TRzToolButtonActionLink() { }
	
};


typedef System::TMetaClass* TRzToolButtonActionLinkClass;

enum DECLSPEC_DENUM TRzToolButtonState : unsigned char { tbsUp, tbsDisabled, tbsDown, tbsExclusive, tbsDropDown };

enum DECLSPEC_DENUM TRzToolStyle : unsigned char { tsButton, tsDropDown };

class PASCALIMPLEMENTATION TRzToolButton : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAllowAllUp;
	bool FDown;
	bool FDragging;
	bool FFlat;
	int FGroupIndex;
	System::Uitypes::TImageIndex FImageIndex;
	System::Uitypes::TImageIndex FDownIndex;
	System::Uitypes::TImageIndex FDisabledIndex;
	System::Uitypes::TImageIndex FHotIndex;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	Vcl::Imglist::TCustomImageList* FDisabledImages;
	Vcl::Imglist::TChangeLink* FDisabledImagesChangeLink;
	Vcl::Buttons::TButtonLayout FLayout;
	bool FMouseOverButton;
	Vcl::Menus::TPopupMenu* FDropDownMenu;
	unsigned FDropTime;
	bool FTreatAsNormal;
	TRzToolStyle FToolStyle;
	bool FShowCaption;
	int FSpacing;
	bool FTransparent;
	bool FInDoDropDown;
	System::Classes::TAlignment FAlignment;
	Rzcommon::TRzVisualStyle FVisualStyle;
	Rzcommon::TRzGradientColorStyle FGradientColorStyle;
	System::Uitypes::TColor FSelectionColorStart;
	System::Uitypes::TColor FSelectionColorStop;
	System::Uitypes::TColor FSelectionFrameColor;
	bool FUseToolbarButtonLayout;
	bool FUseToolbarButtonSize;
	bool FUseToolbarShowCaption;
	bool FUseToolbarVisualStyle;
	System::Classes::TNotifyEvent FOnDropDown;
	bool __fastcall IsCheckedStored();
	bool __fastcall IsImageIndexStored();
	void __fastcall UpdateExclusive();
	void __fastcall UpdateTracking();
	void __fastcall ReadOldGradientColorStartProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldGradientColorStopProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameColorProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldUseGradientsProp(System::Classes::TReader* Reader);
	void __fastcall ImagesChange(System::TObject* Sender);
	void __fastcall DisabledImagesChange(System::TObject* Sender);
	MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMButtonPressed(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMToolbarShowCaptionChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMToolbarButtonLayoutChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMToolbarButtonSizeChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMToolbarVisualStyleChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	TRzToolButtonState FState;
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall PickupToolbarStyles();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DrawBtnBorder(System::Types::TRect &R);
	virtual void __fastcall DrawImage(const System::Types::TRect &R);
	virtual void __fastcall DrawArrow();
	virtual void __fastcall DrawCaption(const System::Types::TRect &R);
	virtual void __fastcall Paint();
	virtual void __fastcall DoDropDown();
	virtual System::Types::TPoint __fastcall GetImageSize();
	virtual void __fastcall GetImageAndCaptionRects(System::Types::TRect &ImageRect, System::Types::TRect &CaptionRect);
	void __fastcall CheckMinSize();
	System::Types::TPoint __fastcall CursorPosition();
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	DYNAMIC Vcl::Controls::TControlActionLinkClass __fastcall GetActionLinkClass();
	virtual void __fastcall AssignTo(System::Classes::TPersistent* Dest);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DropDown();
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetAllowAllUp(bool Value);
	virtual void __fastcall SetAlignment(System::Classes::TAlignment Value);
	virtual void __fastcall SetGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual void __fastcall SetSelectionColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetDown(bool Value);
	virtual void __fastcall SetDropDownMenu(Vcl::Menus::TPopupMenu* Value);
	virtual void __fastcall SetFlat(bool Value);
	virtual void __fastcall SetGroupIndex(int Value);
	virtual void __fastcall SetHotIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetDownIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetDisabledImages(Vcl::Imglist::TCustomImageList* Value);
	bool __fastcall IsLayoutStored();
	virtual void __fastcall SetUseToolbarButtonLayout(bool Value);
	virtual void __fastcall SetLayout(Vcl::Buttons::TButtonLayout Value);
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* Value);
	bool __fastcall IsSizeStored();
	virtual void __fastcall SetUseToolbarButtonSize(bool Value);
	virtual int __fastcall GetWidth();
	HIDESBASE virtual void __fastcall SetWidth(int Value);
	virtual int __fastcall GetHeight();
	HIDESBASE virtual void __fastcall SetHeight(int Value);
	bool __fastcall IsShowCaptionStored();
	virtual void __fastcall SetUseToolbarShowCaption(bool Value);
	virtual void __fastcall SetShowCaption(bool Value);
	virtual void __fastcall SetSpacing(int Value);
	virtual void __fastcall SetToolStyle(TRzToolStyle Value);
	virtual void __fastcall SetTransparent(bool Value);
	bool __fastcall IsVisualStyleStored();
	virtual void __fastcall SetUseToolbarVisualStyle(bool Value);
	virtual void __fastcall SetVisualStyle(Rzcommon::TRzVisualStyle Value);
	__property bool MouseOverButton = {read=FMouseOverButton, nodefault};
	
public:
	__fastcall virtual TRzToolButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzToolButton();
	Vcl::Imglist::TCustomImageList* __fastcall ImageList();
	Vcl::Imglist::TCustomImageList* __fastcall DisabledImageList();
	DYNAMIC void __fastcall Click();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowAllUp = {read=FAllowAllUp, write=SetAllowAllUp, default=0};
	__property System::Classes::TAlignment Alignment = {read=FAlignment, write=SetAlignment, default=2};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property Rzcommon::TRzGradientColorStyle GradientColorStyle = {read=FGradientColorStyle, write=SetGradientColorStyle, stored=IsVisualStyleStored, nodefault};
	__property System::Uitypes::TColor SelectionColorStart = {read=FSelectionColorStart, write=SetSelectionColorStart, default=-16777201};
	__property System::Uitypes::TColor SelectionColorStop = {read=FSelectionColorStop, write=SetSelectionColorStop, default=-16777200};
	__property System::Uitypes::TColor SelectionFrameColor = {read=FSelectionFrameColor, write=SetSelectionFrameColor, default=-16777195};
	__property int GroupIndex = {read=FGroupIndex, write=SetGroupIndex, default=0};
	__property bool Down = {read=FDown, write=SetDown, stored=IsCheckedStored, default=0};
	__property System::Uitypes::TImageIndex DownIndex = {read=FDownIndex, write=SetDownIndex, default=-1};
	__property Vcl::Menus::TPopupMenu* DropDownMenu = {read=FDropDownMenu, write=SetDropDownMenu};
	__property bool Flat = {read=FFlat, write=SetFlat, default=1};
	__property int Height = {read=GetHeight, write=SetHeight, stored=IsSizeStored, default=25};
	__property System::Uitypes::TImageIndex HotIndex = {read=FHotIndex, write=SetHotIndex, default=-1};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, stored=IsImageIndexStored, default=-1};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property Vcl::Imglist::TCustomImageList* DisabledImages = {read=FDisabledImages, write=SetDisabledImages};
	__property Vcl::Buttons::TButtonLayout Layout = {read=FLayout, write=SetLayout, default=0};
	__property bool ShowCaption = {read=FShowCaption, write=SetShowCaption, stored=IsShowCaptionStored, nodefault};
	__property int Spacing = {read=FSpacing, write=SetSpacing, default=4};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=1};
	__property bool UseToolbarButtonLayout = {read=FUseToolbarButtonLayout, write=SetUseToolbarButtonLayout, default=1};
	__property bool UseToolbarButtonSize = {read=FUseToolbarButtonSize, write=SetUseToolbarButtonSize, default=1};
	__property bool UseToolbarShowCaption = {read=FUseToolbarShowCaption, write=SetUseToolbarShowCaption, default=1};
	__property bool UseToolbarVisualStyle = {read=FUseToolbarVisualStyle, write=SetUseToolbarVisualStyle, default=1};
	__property TRzToolStyle ToolStyle = {read=FToolStyle, write=SetToolStyle, default=0};
	__property Rzcommon::TRzVisualStyle VisualStyle = {read=FVisualStyle, write=SetVisualStyle, stored=IsVisualStyleStored, nodefault};
	__property int Width = {read=GetWidth, write=SetWidth, stored=IsSizeStored, default=25};
	__property System::Classes::TNotifyEvent OnDropDown = {read=FOnDropDown, write=FOnDropDown};
	__property Action;
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BiDiMode;
	__property Caption = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property Hint = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
};


enum DECLSPEC_DENUM TRzControlButtonStyle : unsigned char { cbsNone, cbsLeft, cbsUp, cbsRight, cbsDown, cbsDropDown };

class PASCALIMPLEMENTATION TRzControlButton : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	bool FDown;
	bool FDragging;
	bool FFlat;
	bool FMouseOverButton;
	Vcl::Graphics::TBitmap* FGlyph;
	Vcl::Buttons::TNumGlyphs FNumGlyphs;
	bool FRepeatClicks;
	Vcl::Extctrls::TTimer* FRepeatTimer;
	System::Word FInitialDelay;
	System::Word FDelay;
	TRzControlButtonStyle FStyle;
	void __fastcall UpdateTracking();
	void __fastcall GlyphChangedHandler(System::TObject* Sender);
	void __fastcall TimerExpired(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall DrawBtnFace(System::Types::TRect &R);
	virtual void __fastcall DrawSpinButton(System::Types::TRect &R);
	virtual void __fastcall DrawDropDownButton(System::Types::TRect &R);
	virtual void __fastcall DrawGlyph(const System::Types::TRect &R);
	virtual void __fastcall Paint();
	virtual System::Types::TPoint __fastcall GetImageSize();
	DYNAMIC HPALETTE __fastcall GetPalette();
	System::Types::TPoint __fastcall CursorPosition();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall SetFlat(bool Value);
	virtual void __fastcall SetGlyph(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetNumGlyphs(Vcl::Buttons::TNumGlyphs Value);
	virtual void __fastcall SetStyle(TRzControlButtonStyle Value);
	
public:
	__fastcall virtual TRzControlButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzControlButton();
	DYNAMIC void __fastcall Click();
	
__published:
	__property bool Flat = {read=FFlat, write=SetFlat, default=0};
	__property Vcl::Graphics::TBitmap* Glyph = {read=FGlyph, write=SetGlyph};
	__property Vcl::Buttons::TNumGlyphs NumGlyphs = {read=FNumGlyphs, write=SetNumGlyphs, default=1};
	__property System::Word Delay = {read=FDelay, write=FDelay, default=100};
	__property System::Word InitialDelay = {read=FInitialDelay, write=FInitialDelay, default=400};
	__property bool RepeatClicks = {read=FRepeatClicks, write=FRepeatClicks, default=0};
	__property TRzControlButtonStyle Style = {read=FStyle, write=SetStyle, default=0};
	__property Action;
	__property Caption = {default=0};
	__property Color = {default=-16777201};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Hint = {default=0};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDock;
	__property OnStartDrag;
};


typedef System::Int8 TRzBevelWidth;

enum DECLSPEC_DENUM TRzCaptionPosition : unsigned char { cpCentered, cpXY };

class PASCALIMPLEMENTATION TRzShapeButton : public Vcl::Controls::TGraphicControl
{
	typedef Vcl::Controls::TGraphicControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAutoSize;
	TRzBevelWidth FBevelWidth;
	System::Uitypes::TColor FBevelHighlightColor;
	System::Uitypes::TColor FBevelShadowColor;
	Vcl::Graphics::TBitmap* FBitmap;
	Vcl::Graphics::TBitmap* FBitmapUp;
	Vcl::Graphics::TBitmap* FBitmapDown;
	Vcl::Forms::TFormBorderStyle FBorderStyle;
	System::Uitypes::TColor FBorderColor;
	bool FDragging;
	Vcl::Graphics::TBitmap* FHitTestMask;
	bool FPrevCursorSaved;
	System::Uitypes::TCursor FPrevCursor;
	bool FPrevShowHintSaved;
	bool FPrevShowHint;
	bool FPrevParentShowHint;
	bool FPreciseClick;
	bool FPreciseShowHint;
	TRzCaptionPosition FCaptionPosition;
	int FCaptionX;
	int FCaptionY;
	void __fastcall AdjustBounds();
	void __fastcall AdjustButtonSize(int &W, int &H);
	System::Uitypes::TColor __fastcall BevelColor(const Vcl::Buttons::TButtonState AState, const bool TopLeft);
	void __fastcall BitmapChanged(System::TObject* Sender);
	void __fastcall Create3DBitmap(Vcl::Graphics::TBitmap* Source, const Vcl::Buttons::TButtonState AState, Vcl::Graphics::TBitmap* Target);
	void __fastcall InitPalette(HDC DC);
	MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMSysColorChange(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	
protected:
	Vcl::Buttons::TButtonState FState;
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall DrawButtonText(Vcl::Graphics::TCanvas* Canvas, const System::UnicodeString Caption, const System::Types::TRect &TextBounds, Vcl::Buttons::TButtonState State);
	DYNAMIC HPALETTE __fastcall GetPalette();
	virtual System::Types::TRect __fastcall GetCaptionRect(Vcl::Graphics::TCanvas* Canvas, const System::UnicodeString Caption);
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	virtual void __fastcall Paint();
	virtual void __fastcall ReadBitmapUpData(System::Classes::TStream* Stream);
	virtual void __fastcall WriteBitmapUpData(System::Classes::TStream* Stream);
	virtual void __fastcall ReadBitmapDownData(System::Classes::TStream* Stream);
	virtual void __fastcall WriteBitmapDownData(System::Classes::TStream* Stream);
	virtual void __fastcall SetAutoSize(bool Value);
	virtual void __fastcall SetBevelHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBevelShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBevelWidth(TRzBevelWidth Value);
	virtual void __fastcall SetBitmap(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetBitmapDown(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetBitmapUp(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderStyle(Vcl::Forms::TBorderStyle Value);
	virtual void __fastcall SetCaptionPosition(TRzCaptionPosition Value);
	virtual void __fastcall SetCaptionX(int Value);
	virtual void __fastcall SetCaptionY(int Value);
	
public:
	__fastcall virtual TRzShapeButton(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzShapeButton();
	DYNAMIC void __fastcall Click();
	virtual void __fastcall Invalidate();
	virtual bool __fastcall PtInMask(const int X, const int Y);
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	virtual void __fastcall SetCaptionXY(const int X, const int Y);
	__property Vcl::Graphics::TBitmap* BitmapUp = {read=FBitmapUp};
	__property Vcl::Graphics::TBitmap* BitmapDown = {read=FBitmapDown};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AutoSize = {read=FAutoSize, write=SetAutoSize, default=1};
	__property System::Uitypes::TColor BevelHighlightColor = {read=FBevelHighlightColor, write=SetBevelHighlightColor, default=-16777196};
	__property System::Uitypes::TColor BevelShadowColor = {read=FBevelShadowColor, write=SetBevelShadowColor, default=-16777200};
	__property TRzBevelWidth BevelWidth = {read=FBevelWidth, write=SetBevelWidth, default=2};
	__property Vcl::Graphics::TBitmap* Bitmap = {read=FBitmap, write=SetBitmap};
	__property Vcl::Forms::TBorderStyle BorderStyle = {read=FBorderStyle, write=SetBorderStyle, default=1};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777195};
	__property bool PreciseClick = {read=FPreciseClick, write=FPreciseClick, default=1};
	__property bool PreciseShowHint = {read=FPreciseShowHint, write=FPreciseShowHint, default=1};
	__property TRzCaptionPosition CaptionPosition = {read=FCaptionPosition, write=SetCaptionPosition, default=0};
	__property int CaptionX = {read=FCaptionX, write=SetCaptionX, default=0};
	__property int CaptionY = {read=FCaptionY, write=SetCaptionY, default=0};
	__property Anchors = {default=3};
	__property Caption = {default=0};
	__property Constraints;
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property ShowHint;
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnStartDrag;
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE Rzbutton__1 BitBtnCaptions;
static const System::Word cm_RzButtonPressed = System::Word(0x2421);
extern DELPHI_PACKAGE Vcl::Graphics::TBitmap* __fastcall GetBitBtnGlyph(Vcl::Buttons::TBitBtnKind Kind);
}	/* namespace Rzbutton */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZBUTTON)
using namespace Rzbutton;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzbuttonHPP
