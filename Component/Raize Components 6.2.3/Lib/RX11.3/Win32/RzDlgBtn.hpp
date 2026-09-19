// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDlgBtn.pas' rev: 35.00 (Windows)

#ifndef RzdlgbtnHPP
#define RzdlgbtnHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzButton.hpp>
#include <System.SysUtils.hpp>
#include <RzCommon.hpp>
#include <RzPanel.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdlgbtn
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDialogButtons;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDialogButtons : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FShowDivider;
	bool FShowGlyphs;
	System::StaticArray<int, 3> FWidths;
	System::Classes::TNotifyEvent FOnClickOk;
	System::Classes::TNotifyEvent FOnClickCancel;
	System::Classes::TNotifyEvent FOnClickHelp;
	int FButtonHeight;
	int FButtonWidth;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall BtnOkClickHandler(System::TObject* Sender);
	void __fastcall BtnCancelClickHandler(System::TObject* Sender);
	void __fastcall BtnHelpClickHandler(System::TObject* Sender);
	
protected:
	Rzbutton::TRzBitBtn* FBtnOk;
	Rzbutton::TRzBitBtn* FBtnCancel;
	Rzbutton::TRzBitBtn* FBtnHelp;
	virtual void __fastcall CreateButtons();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	virtual void __fastcall AdjustPanelSize();
	virtual void __fastcall PositionButtons();
	DYNAMIC void __fastcall Resize();
	DYNAMIC void __fastcall BtnOkClick();
	DYNAMIC void __fastcall BtnCancelClick();
	DYNAMIC void __fastcall BtnHelpClick();
	virtual System::Classes::TBasicAction* __fastcall GetBtnAction(int Index);
	virtual void __fastcall SetBtnAction(int Index, System::Classes::TBasicAction* Value);
	virtual Vcl::Controls::TAlign __fastcall GetAlign();
	HIDESBASE virtual void __fastcall SetAlign(Vcl::Controls::TAlign Value);
	virtual System::Uitypes::TColor __fastcall GetButtonColor();
	virtual void __fastcall SetButtonColor(System::Uitypes::TColor Value);
	virtual System::Uitypes::TColor __fastcall GetButtonFrameColor();
	virtual void __fastcall SetButtonFrameColor(System::Uitypes::TColor Value);
	virtual Vcl::Controls::TCaption __fastcall GetOkCaption();
	virtual void __fastcall SetOkCaption(const Vcl::Controls::TCaption Value);
	bool __fastcall StoreOkCaption();
	virtual Vcl::Controls::TCaption __fastcall GetCancelCaption();
	virtual void __fastcall SetCancelCaption(const Vcl::Controls::TCaption Value);
	bool __fastcall StoreCancelCaption();
	virtual Vcl::Controls::TCaption __fastcall GetHelpCaption();
	virtual void __fastcall SetHelpCaption(const Vcl::Controls::TCaption Value);
	bool __fastcall StoreHelpCaption();
	virtual bool __fastcall GetBtnEnable(int Index);
	virtual void __fastcall SetBtnEnable(int Index, bool Value);
	virtual int __fastcall GetBtnWidth(int Index);
	virtual void __fastcall SetBtnWidth(int Index, const int Value);
	virtual void __fastcall SetShowDivider(bool Value);
	virtual void __fastcall SetShowGlyphs(bool Value);
	virtual bool __fastcall GetShowButton(int Index);
	virtual void __fastcall SetShowButton(int Index, bool Value);
	virtual bool __fastcall GetCancelCancel();
	virtual void __fastcall SetCancelCancel(bool Value);
	virtual bool __fastcall GetOKDefault();
	virtual void __fastcall SetOKDefault(bool Value);
	virtual bool __fastcall GetHotTrack();
	virtual void __fastcall SetHotTrack(bool Value);
	virtual System::Uitypes::TColor __fastcall GetHotTrackColor();
	virtual void __fastcall SetHotTrackColor(System::Uitypes::TColor Value);
	virtual Rzcommon::TRzHotTrackColorType __fastcall GetHotTrackColorType();
	virtual void __fastcall SetHotTrackColorType(Rzcommon::TRzHotTrackColorType Value);
	virtual System::Uitypes::TColor __fastcall GetHighlightColor();
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual System::Uitypes::TModalResult __fastcall GetBtnModalResult(int Index);
	virtual void __fastcall SetBtnModalResult(int Index, System::Uitypes::TModalResult Value);
	
public:
	__fastcall virtual TRzDialogButtons(System::Classes::TComponent* AOwner);
	__property Rzbutton::TRzBitBtn* BtnOK = {read=FBtnOk};
	__property Rzbutton::TRzBitBtn* BtnCancel = {read=FBtnCancel};
	__property Rzbutton::TRzBitBtn* BtnHelp = {read=FBtnHelp};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Classes::TBasicAction* ActionOk = {read=GetBtnAction, write=SetBtnAction, index=1};
	__property System::Classes::TBasicAction* ActionCancel = {read=GetBtnAction, write=SetBtnAction, index=2};
	__property System::Classes::TBasicAction* ActionHelp = {read=GetBtnAction, write=SetBtnAction, index=3};
	__property Vcl::Controls::TAlign Align = {read=GetAlign, write=SetAlign, default=2};
	__property System::Uitypes::TColor ButtonColor = {read=GetButtonColor, write=SetButtonColor, default=-16777201};
	__property System::Uitypes::TColor ButtonFrameColor = {read=GetButtonFrameColor, write=SetButtonFrameColor, default=-16777195};
	__property Vcl::Controls::TCaption CaptionOk = {read=GetOkCaption, write=SetOkCaption, stored=StoreOkCaption};
	__property Vcl::Controls::TCaption CaptionCancel = {read=GetCancelCaption, write=SetCancelCaption, stored=StoreCancelCaption};
	__property Vcl::Controls::TCaption CaptionHelp = {read=GetHelpCaption, write=SetHelpCaption, stored=StoreHelpCaption};
	__property bool EnableOk = {read=GetBtnEnable, write=SetBtnEnable, index=1, default=1};
	__property bool EnableCancel = {read=GetBtnEnable, write=SetBtnEnable, index=2, default=1};
	__property bool EnableHelp = {read=GetBtnEnable, write=SetBtnEnable, index=3, default=1};
	__property bool HotTrack = {read=GetHotTrack, write=SetHotTrack, default=0};
	__property System::Uitypes::TColor HighlightColor = {read=GetHighlightColor, write=SetHighlightColor, default=-16777203};
	__property System::Uitypes::TColor HotTrackColor = {read=GetHotTrackColor, write=SetHotTrackColor, default=1350640};
	__property Rzcommon::TRzHotTrackColorType HotTrackColorType = {read=GetHotTrackColorType, write=SetHotTrackColorType, default=1};
	__property bool CancelCancel = {read=GetCancelCancel, write=SetCancelCancel, default=1};
	__property bool OKDefault = {read=GetOKDefault, write=SetOKDefault, default=1};
	__property System::Uitypes::TModalResult ModalResultOk = {read=GetBtnModalResult, write=SetBtnModalResult, index=1, default=1};
	__property System::Uitypes::TModalResult ModalResultCancel = {read=GetBtnModalResult, write=SetBtnModalResult, index=2, default=2};
	__property System::Uitypes::TModalResult ModalResultHelp = {read=GetBtnModalResult, write=SetBtnModalResult, index=3, default=0};
	__property bool ShowDivider = {read=FShowDivider, write=SetShowDivider, default=0};
	__property bool ShowGlyphs = {read=FShowGlyphs, write=SetShowGlyphs, default=0};
	__property bool ShowOKButton = {read=GetShowButton, write=SetShowButton, index=1, default=1};
	__property bool ShowCancelButton = {read=GetShowButton, write=SetShowButton, index=2, default=1};
	__property bool ShowHelpButton = {read=GetShowButton, write=SetShowButton, index=3, default=0};
	__property int WidthOk = {read=GetBtnWidth, write=SetBtnWidth, index=1, default=75};
	__property int WidthCancel = {read=GetBtnWidth, write=SetBtnWidth, index=2, default=75};
	__property int WidthHelp = {read=GetBtnWidth, write=SetBtnWidth, index=3, default=75};
	__property System::Classes::TNotifyEvent OnClickOk = {read=FOnClickOk, write=FOnClickOk};
	__property System::Classes::TNotifyEvent OnClickCancel = {read=FOnClickCancel, write=FOnClickCancel};
	__property System::Classes::TNotifyEvent OnClickHelp = {read=FOnClickHelp, write=FOnClickHelp};
	__property Color = {default=-16777201};
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Font;
	__property FullRepaint = {default=1};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property Height = {default=36};
	__property Locked = {default=0};
	__property ParentColor = {default=0};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnResize;
	__property OnStartDrag;
public:
	/* TRzCustomPanel.Destroy */ inline __fastcall virtual ~TRzDialogButtons() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDialogButtons(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdlgbtn */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDLGBTN)
using namespace Rzdlgbtn;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdlgbtnHPP
