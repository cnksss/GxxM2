// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzRadGrp.pas' rev: 35.00 (Windows)

#ifndef RzradgrpHPP
#define RzradgrpHPP

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
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzPanel.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ImgList.hpp>
#include <RzCommon.hpp>
#include <RzIntLst.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzRadChk.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzradgrp
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomRadioGroup;
class DELPHICLASS TRzRadioGroup;
class DELPHICLASS TRzCustomCheckGroup;
class DELPHICLASS TRzCheckGroup;
//-- type declarations -------------------------------------------------------
typedef void __fastcall (__closure *TRzIndexChangingEvent)(System::TObject* Sender, int NewIndex, bool &AllowChange);

class PASCALIMPLEMENTATION TRzCustomRadioGroup : public Rzpanel::TRzCustomGroupBox
{
	typedef Rzpanel::TRzCustomGroupBox inherited;
	
private:
	System::Classes::TList* FButtons;
	Vcl::Graphics::TFont* FItemFont;
	bool FItemFontChanged;
	System::Classes::TStrings* FItems;
	int FItemIndex;
	int FColumns;
	bool FReading;
	bool FUpdating;
	bool FSpaceEvenly;
	int FStartXPos;
	int FStartYPos;
	int FVerticalSpacing;
	int FItemHeight;
	bool FItemHeightChanged;
	int FGlyphWidth;
	int FGlyphHeight;
	int FNumGlyphs;
	Vcl::Graphics::TBitmap* FCustomGlyphs;
	Vcl::Imglist::TCustomImageList* FCustomGlyphImages;
	Vcl::Imglist::TChangeLink* FCustomGlyphImagesChangeLink;
	bool FUseCustomGlyphs;
	System::Uitypes::TColor FTransparentColor;
	System::Uitypes::TColor FWinMaskColor;
	bool FLightTextStyle;
	Rzcommon::TTextStyle FTextStyle;
	System::Uitypes::TColor FTextHighlightColor;
	System::Uitypes::TColor FTextShadowColor;
	int FTextShadowDepth;
	bool FTabOnEnter;
	System::Uitypes::TColor FItemFrameColor;
	bool FItemHotTrack;
	System::Uitypes::TColor FItemHotTrackColor;
	Rzcommon::TRzHotTrackColorType FItemHotTrackColorType;
	System::Uitypes::TColor FItemHighlightColor;
	bool FReadOnly;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
	TRzIndexChangingEvent FOnChanging;
	void __fastcall ReadOldFlatProp(System::Classes::TReader* Reader);
	void __fastcall ButtonClick(System::TObject* Sender);
	void __fastcall ItemsChange(System::TObject* Sender);
	void __fastcall ItemFontChanged(System::TObject* Sender);
	void __fastcall CustomGlyphsChanged(System::TObject* Sender);
	void __fastcall CustomGlyphImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	virtual void __fastcall SetButtonCount(int Value);
	virtual void __fastcall UpdateButtons();
	void __fastcall FocusNextButton();
	void __fastcall FocusPreviousButton();
	virtual void __fastcall ReadState(System::Classes::TReader* Reader);
	virtual bool __fastcall CanModify();
	virtual void __fastcall CustomFramingChanged();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC bool __fastcall CanChange(int NewIndex);
	virtual Rzradchk::TRzRadioButton* __fastcall GetButtons(int Index);
	virtual Vcl::Controls::TCaption __fastcall GetCaption();
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetColumns(int Value);
	virtual void __fastcall SetCustomGlyphs(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetCustomGlyphImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetItemFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotTrack(bool Value);
	virtual void __fastcall SetItemHotTrackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotTrackColorType(Rzcommon::TRzHotTrackColorType Value);
	virtual void __fastcall SetItemHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBannerHeight(int Value);
	virtual void __fastcall SetGroupBoxStyle(Rzpanel::TRzGroupBoxStyle Value);
	bool __fastcall GetItemEnabled(int Index);
	void __fastcall SetItemEnabled(int Index, bool Value);
	virtual void __fastcall SetItemFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetItemHeight(int Value);
	virtual void __fastcall SetItemIndex(int Value);
	virtual void __fastcall SetItems(System::Classes::TStrings* Value);
	virtual void __fastcall SetLightTextStyle(bool Value);
	virtual void __fastcall SetTextHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTextShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTextShadowDepth(int Value);
	virtual void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetReadOnlyColorOnFocus(bool Value);
	virtual void __fastcall SetSpaceEvenly(bool Value);
	virtual void __fastcall SetStartPos(int Index, int Value);
	virtual void __fastcall SetTextStyle(Rzcommon::TTextStyle Value);
	virtual void __fastcall SetVerticalSpacing(int Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetTransparentColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetUseCustomGlyphs(bool Value);
	virtual void __fastcall SetWinMaskColor(System::Uitypes::TColor Value);
	__property Rzradchk::TRzRadioButton* Buttons[int Index] = {read=GetButtons};
	__property Vcl::Controls::TCaption Caption = {read=GetCaption, write=SetCaption};
	__property int Columns = {read=FColumns, write=SetColumns, default=1};
	__property Vcl::Graphics::TBitmap* CustomGlyphs = {read=FCustomGlyphs, write=SetCustomGlyphs};
	__property Vcl::Imglist::TCustomImageList* CustomGlyphImages = {read=FCustomGlyphImages, write=SetCustomGlyphImages};
	__property System::Uitypes::TColor ItemFrameColor = {read=FItemFrameColor, write=SetItemFrameColor, default=-16777200};
	__property bool ItemHotTrack = {read=FItemHotTrack, write=SetItemHotTrack, default=0};
	__property System::Uitypes::TColor ItemHighlightColor = {read=FItemHighlightColor, write=SetItemHighlightColor, default=-16777203};
	__property System::Uitypes::TColor ItemHotTrackColor = {read=FItemHotTrackColor, write=SetItemHotTrackColor, default=1350640};
	__property Rzcommon::TRzHotTrackColorType ItemHotTrackColorType = {read=FItemHotTrackColorType, write=SetItemHotTrackColorType, default=1};
	__property Vcl::Graphics::TFont* ItemFont = {read=FItemFont, write=SetItemFont, stored=FItemFontChanged};
	__property int ItemHeight = {read=FItemHeight, write=SetItemHeight, stored=FItemHeightChanged, nodefault};
	__property int ItemIndex = {read=FItemIndex, write=SetItemIndex, default=-1};
	__property bool ItemEnabled[int Index] = {read=GetItemEnabled, write=SetItemEnabled};
	__property System::Classes::TStrings* Items = {read=FItems, write=SetItems};
	__property bool LightTextStyle = {read=FLightTextStyle, write=SetLightTextStyle, default=0};
	__property System::Uitypes::TColor TextHighlightColor = {read=FTextHighlightColor, write=SetTextHighlightColor, default=-16777196};
	__property System::Uitypes::TColor TextShadowColor = {read=FTextShadowColor, write=SetTextShadowColor, default=-16777200};
	__property int TextShadowDepth = {read=FTextShadowDepth, write=SetTextShadowDepth, default=2};
	__property bool ReadOnly = {read=FReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=SetReadOnlyColorOnFocus, default=0};
	__property bool SpaceEvenly = {read=FSpaceEvenly, write=SetSpaceEvenly, default=0};
	__property int StartXPos = {read=FStartXPos, write=SetStartPos, index=1, default=8};
	__property int StartYPos = {read=FStartYPos, write=SetStartPos, index=2, default=2};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property Rzcommon::TTextStyle TextStyle = {read=FTextStyle, write=SetTextStyle, default=0};
	__property System::Uitypes::TColor TransparentColor = {read=FTransparentColor, write=SetTransparentColor, default=32896};
	__property bool UseCustomGlyphs = {read=FUseCustomGlyphs, write=SetUseCustomGlyphs, default=0};
	__property int VerticalSpacing = {read=FVerticalSpacing, write=SetVerticalSpacing, default=3};
	__property System::Uitypes::TColor WinMaskColor = {read=FWinMaskColor, write=SetWinMaskColor, default=65280};
	__property TRzIndexChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property Alignment = {default=0};
	__property AlignmentVertical = {default=0};
	
public:
	__fastcall virtual TRzCustomRadioGroup(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomRadioGroup();
	virtual void __fastcall ArrangeButtons();
	DYNAMIC void __fastcall FlipChildren(bool AllLevels);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomRadioGroup(HWND ParentWindow) : Rzpanel::TRzCustomGroupBox(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzRadioGroup : public TRzCustomRadioGroup
{
	typedef TRzCustomRadioGroup inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
public:
	__property Buttons;
	__property ItemEnabled;
	
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
	__property Caption;
	__property CaptionFont;
	__property Color = {default=-16777201};
	__property Columns = {default=1};
	__property Constraints;
	__property Ctl3D;
	__property CustomGlyphs;
	__property CustomGlyphImages;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FrameController;
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property GroupStyle = {default=3};
	__property Height = {default=105};
	__property Hint = {default=0};
	__property ItemFrameColor = {default=-16777200};
	__property ItemHotTrack = {default=0};
	__property ItemHighlightColor = {default=-16777203};
	__property ItemHotTrackColor = {default=1350640};
	__property ItemHotTrackColorType = {default=1};
	__property ItemFont;
	__property ItemHeight;
	__property ItemIndex = {default=-1};
	__property Items;
	__property LightTextStyle = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property TextHighlightColor = {default=-16777196};
	__property TextShadowColor = {default=-16777200};
	__property TextShadowDepth = {default=2};
	__property Touch;
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property SpaceEvenly = {default=0};
	__property StartXPos = {index=1, default=8};
	__property StartYPos = {index=2, default=2};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property TextStyle = {default=0};
	__property Transparent = {default=0};
	__property TransparentColor = {default=32896};
	__property UseCustomGlyphs = {default=0};
	__property VerticalSpacing = {default=3};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property WinMaskColor = {default=65280};
	__property OnChanging;
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
	/* TRzCustomRadioGroup.Create */ inline __fastcall virtual TRzRadioGroup(System::Classes::TComponent* AOwner) : TRzCustomRadioGroup(AOwner) { }
	/* TRzCustomRadioGroup.Destroy */ inline __fastcall virtual ~TRzRadioGroup() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzRadioGroup(HWND ParentWindow) : TRzCustomRadioGroup(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzCustomCheckGroup : public Rzpanel::TRzCustomGroupBox
{
	typedef Rzpanel::TRzCustomGroupBox inherited;
	
private:
	System::Classes::TList* FChecks;
	Vcl::Graphics::TFont* FItemFont;
	bool FItemFontChanged;
	System::Classes::TStrings* FItems;
	int FColumns;
	bool FReading;
	bool FSpaceEvenly;
	int FStartXPos;
	int FStartYPos;
	int FVerticalSpacing;
	int FItemHeight;
	bool FItemHeightChanged;
	int FGlyphWidth;
	int FGlyphHeight;
	int FNumGlyphs;
	Vcl::Graphics::TBitmap* FCustomGlyphs;
	Vcl::Imglist::TCustomImageList* FCustomGlyphImages;
	Vcl::Imglist::TChangeLink* FCustomGlyphImagesChangeLink;
	bool FUseCustomGlyphs;
	System::Uitypes::TColor FTransparentColor;
	System::Uitypes::TColor FWinMaskColor;
	bool FLightTextStyle;
	Rzcommon::TTextStyle FTextStyle;
	System::Uitypes::TColor FTextHighlightColor;
	System::Uitypes::TColor FTextShadowColor;
	int FTextShadowDepth;
	bool FTabOnEnter;
	bool FAllowGrayed;
	System::Uitypes::TColor FItemFrameColor;
	bool FItemHotTrack;
	System::Uitypes::TColor FItemHotTrackColor;
	Rzcommon::TRzHotTrackColorType FItemHotTrackColorType;
	System::Uitypes::TColor FItemHighlightColor;
	bool FReadOnly;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
	Rzintlst::TRzIntegerList* FItemStates;
	Rzcommon::TStateChangeEvent FOnChange;
	void __fastcall ReadCheckStates(System::Classes::TReader* Reader);
	void __fastcall WriteCheckStates(System::Classes::TWriter* Writer);
	void __fastcall CheckClick(System::TObject* Sender);
	void __fastcall ItemsChange(System::TObject* Sender);
	void __fastcall ItemFontChanged(System::TObject* Sender);
	void __fastcall CustomGlyphsChanged(System::TObject* Sender);
	void __fastcall CustomGlyphImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMDialogChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	virtual void __fastcall SetCheckCount(int Value);
	virtual void __fastcall UpdateChecks();
	int __fastcall GetIndex(Rzradchk::TRzCheckBox* CheckBox);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Loaded();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall ReadState(System::Classes::TReader* Reader);
	virtual bool __fastcall CanModify();
	virtual void __fastcall CustomFramingChanged();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	DYNAMIC void __fastcall Change(int Index, Vcl::Stdctrls::TCheckBoxState NewState);
	virtual Rzradchk::TRzCheckBox* __fastcall GetChecks(int Index);
	virtual void __fastcall SetAllowGrayed(bool Value);
	virtual Vcl::Controls::TCaption __fastcall GetCaption();
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetColumns(int Value);
	virtual void __fastcall SetCustomGlyphs(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetCustomGlyphImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetGroupBoxStyle(Rzpanel::TRzGroupBoxStyle Value);
	virtual void __fastcall SetItemFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotTrack(bool Value);
	virtual void __fastcall SetItemHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotTrackColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotTrackColorType(Rzcommon::TRzHotTrackColorType Value);
	virtual bool __fastcall GetItemChecked(int Index);
	virtual void __fastcall SetItemChecked(int Index, bool Value);
	virtual bool __fastcall GetItemEnabled(int Index);
	virtual void __fastcall SetItemEnabled(int Index, bool Value);
	virtual void __fastcall SetItemFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetItemHeight(int Value);
	virtual void __fastcall SetItems(System::Classes::TStrings* Value);
	virtual Vcl::Stdctrls::TCheckBoxState __fastcall GetItemState(int Index);
	virtual void __fastcall SetItemState(int Index, Vcl::Stdctrls::TCheckBoxState Value);
	virtual void __fastcall SetLightTextStyle(bool Value);
	virtual void __fastcall SetTextHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTextShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTextShadowDepth(int Value);
	virtual void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetReadOnlyColorOnFocus(bool Value);
	virtual void __fastcall SetSpaceEvenly(bool Value);
	virtual void __fastcall SetStartPos(int Index, int Value);
	virtual void __fastcall SetTextStyle(Rzcommon::TTextStyle Value);
	virtual void __fastcall SetVerticalSpacing(int Value);
	virtual void __fastcall SetTransparent(bool Value);
	virtual void __fastcall SetTransparentColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetUseCustomGlyphs(bool Value);
	virtual void __fastcall SetWinMaskColor(System::Uitypes::TColor Value);
	__property bool AllowGrayed = {read=FAllowGrayed, write=SetAllowGrayed, default=0};
	__property Rzradchk::TRzCheckBox* Checks[int Index] = {read=GetChecks};
	__property Vcl::Controls::TCaption Caption = {read=GetCaption, write=SetCaption};
	__property int Columns = {read=FColumns, write=SetColumns, default=1};
	__property Vcl::Graphics::TBitmap* CustomGlyphs = {read=FCustomGlyphs, write=SetCustomGlyphs};
	__property Vcl::Imglist::TCustomImageList* CustomGlyphImages = {read=FCustomGlyphImages, write=SetCustomGlyphImages};
	__property System::Uitypes::TColor ItemFrameColor = {read=FItemFrameColor, write=SetItemFrameColor, default=-16777200};
	__property bool ItemHotTrack = {read=FItemHotTrack, write=SetItemHotTrack, default=0};
	__property System::Uitypes::TColor ItemHighlightColor = {read=FItemHighlightColor, write=SetItemHighlightColor, default=-16777203};
	__property System::Uitypes::TColor ItemHotTrackColor = {read=FItemHotTrackColor, write=SetItemHotTrackColor, default=1350640};
	__property Rzcommon::TRzHotTrackColorType ItemHotTrackColorType = {read=FItemHotTrackColorType, write=SetItemHotTrackColorType, default=1};
	__property Vcl::Graphics::TFont* ItemFont = {read=FItemFont, write=SetItemFont, stored=FItemFontChanged};
	__property int ItemHeight = {read=FItemHeight, write=SetItemHeight, stored=FItemHeightChanged, nodefault};
	__property bool ItemChecked[int Index] = {read=GetItemChecked, write=SetItemChecked};
	__property bool ItemEnabled[int Index] = {read=GetItemEnabled, write=SetItemEnabled};
	__property Vcl::Stdctrls::TCheckBoxState ItemState[int Index] = {read=GetItemState, write=SetItemState};
	__property System::Classes::TStrings* Items = {read=FItems, write=SetItems};
	__property bool LightTextStyle = {read=FLightTextStyle, write=SetLightTextStyle, default=0};
	__property System::Uitypes::TColor TextHighlightColor = {read=FTextHighlightColor, write=SetTextHighlightColor, default=-16777196};
	__property System::Uitypes::TColor TextShadowColor = {read=FTextShadowColor, write=SetTextShadowColor, default=-16777200};
	__property int TextShadowDepth = {read=FTextShadowDepth, write=SetTextShadowDepth, default=2};
	__property bool ReadOnly = {read=FReadOnly, write=SetReadOnly, default=0};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=SetReadOnlyColorOnFocus, default=0};
	__property bool SpaceEvenly = {read=FSpaceEvenly, write=SetSpaceEvenly, default=0};
	__property int StartXPos = {read=FStartXPos, write=SetStartPos, index=1, default=8};
	__property int StartYPos = {read=FStartYPos, write=SetStartPos, index=2, default=2};
	__property bool TabOnEnter = {read=FTabOnEnter, write=FTabOnEnter, default=0};
	__property Rzcommon::TTextStyle TextStyle = {read=FTextStyle, write=SetTextStyle, default=0};
	__property System::Uitypes::TColor TransparentColor = {read=FTransparentColor, write=SetTransparentColor, default=32896};
	__property bool UseCustomGlyphs = {read=FUseCustomGlyphs, write=SetUseCustomGlyphs, default=0};
	__property int VerticalSpacing = {read=FVerticalSpacing, write=SetVerticalSpacing, default=3};
	__property System::Uitypes::TColor WinMaskColor = {read=FWinMaskColor, write=SetWinMaskColor, default=65280};
	__property Rzcommon::TStateChangeEvent OnChange = {read=FOnChange, write=FOnChange};
	__property Alignment = {default=0};
	__property AlignmentVertical = {default=0};
	
public:
	__fastcall virtual TRzCustomCheckGroup(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomCheckGroup();
	virtual void __fastcall ArrangeChecks();
	DYNAMIC void __fastcall FlipChildren(bool AllLevels);
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomCheckGroup(HWND ParentWindow) : Rzpanel::TRzCustomGroupBox(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzCheckGroup : public TRzCustomCheckGroup
{
	typedef TRzCustomCheckGroup inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	
public:
	__property Checks;
	__property ItemChecked;
	__property ItemEnabled;
	__property ItemState;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Align = {default=0};
	__property Alignment = {default=0};
	__property AllowGrayed = {default=0};
	__property Anchors = {default=3};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderColor = {default=-16777201};
	__property BorderInner = {default=0};
	__property BorderOuter = {default=0};
	__property BorderSides = {default=15};
	__property BorderWidth = {default=0};
	__property Caption;
	__property CaptionFont;
	__property Color = {default=-16777201};
	__property Columns = {default=1};
	__property Constraints;
	__property Ctl3D;
	__property CustomGlyphs;
	__property CustomGlyphImages;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property Font;
	__property FrameController;
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property GroupStyle = {default=3};
	__property Height = {default=105};
	__property ItemFrameColor = {default=-16777200};
	__property ItemHighlightColor = {default=-16777203};
	__property ItemHotTrack = {default=0};
	__property ItemHotTrackColor = {default=1350640};
	__property ItemHotTrackColorType = {default=1};
	__property ItemFont;
	__property ItemHeight;
	__property Items;
	__property LightTextStyle = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property TextHighlightColor = {default=-16777196};
	__property TextShadowColor = {default=-16777200};
	__property TextShadowDepth = {default=2};
	__property ReadOnly = {default=0};
	__property ReadOnlyColor = {default=-16777192};
	__property ReadOnlyColorOnFocus = {default=0};
	__property ShowHint;
	__property SpaceEvenly = {default=0};
	__property StartXPos = {index=1, default=8};
	__property StartYPos = {index=2, default=2};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property TextStyle = {default=0};
	__property Touch;
	__property Transparent = {default=0};
	__property TransparentColor = {default=32896};
	__property UseCustomGlyphs = {default=0};
	__property VerticalSpacing = {default=3};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property WinMaskColor = {default=65280};
	__property OnChange;
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
	/* TRzCustomCheckGroup.Create */ inline __fastcall virtual TRzCheckGroup(System::Classes::TComponent* AOwner) : TRzCustomCheckGroup(AOwner) { }
	/* TRzCustomCheckGroup.Destroy */ inline __fastcall virtual ~TRzCheckGroup() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckGroup(HWND ParentWindow) : TRzCustomCheckGroup(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzradgrp */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZRADGRP)
using namespace Rzradgrp;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzradgrpHPP
