// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBChk.pas' rev: 35.00 (Windows)

#ifndef RzdbchkHPP
#define RzdbchkHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.SysUtils.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Data.DB.hpp>
#include <Vcl.Menus.hpp>
#include <RzCommon.hpp>
#include <RzButton.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzRadChk.hpp>
#include <System.UITypes.hpp>
#include <Vcl.ImgList.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbchk
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBCheckBox;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBCheckBox : public Rzradchk::TRzCustomCheckBox
{
	typedef Rzradchk::TRzCustomCheckBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	System::UnicodeString FValueCheck;
	System::UnicodeString FValueUncheck;
	bool FNullAsUnchecked;
	Rzradchk::TRzCheckBox* FPaintControl;
	void __fastcall DataChange(System::TObject* Sender);
	void __fastcall UpdateData(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall ChangeState();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	bool __fastcall ValueMatch(const System::UnicodeString ValueList, const System::UnicodeString Value);
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual Data::Db::TField* __fastcall GetField();
	virtual Vcl::Stdctrls::TCheckBoxState __fastcall GetFieldState();
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	virtual void __fastcall SetValueCheck(const System::UnicodeString Value);
	virtual void __fastcall SetValueUncheck(const System::UnicodeString Value);
	virtual void __fastcall SetNullAsUnchecked(bool Value);
	
public:
	__fastcall virtual TRzDBCheckBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBCheckBox();
	__property Checked = {default=0};
	__property Data::Db::TField* Field = {read=GetField};
	__property State;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool NullAsUnchecked = {read=FNullAsUnchecked, write=SetNullAsUnchecked, default=1};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::UnicodeString ValueChecked = {read=FValueCheck, write=SetValueCheck};
	__property System::UnicodeString ValueUnchecked = {read=FValueUncheck, write=SetValueUncheck};
	__property Action;
	__property Align = {default=0};
	__property Alignment = {default=1};
	__property AlignmentVertical = {default=0};
	__property AllowGrayed = {default=0};
	__property Anchors = {default=3};
	__property AutoSize = {default=1};
	__property BiDiMode;
	__property Caption = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property CustomGlyphs;
	__property CustomGlyphImages;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FillColor = {default=-16777211};
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property Font;
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property Height;
	__property HelpContext = {default=0};
	__property HighlightColor = {default=-16777203};
	__property Hint = {default=0};
	__property HotTrack = {default=0};
	__property HotTrackColor = {default=1350640};
	__property HotTrackColorType = {default=1};
	__property HotTrackStyle = {default=0};
	__property LightTextStyle = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=1};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property TextHighlightColor = {default=-16777196};
	__property TextShadowColor = {default=-16777200};
	__property TextShadowDepth = {default=2};
	__property ShowHint;
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property TextStyle = {default=0};
	__property ThemeAware = {default=1};
	__property Transparent = {default=0};
	__property TransparentColor = {default=32896};
	__property UseCustomGlyphs = {default=0};
	__property Visible = {default=1};
	__property WinMaskColor = {default=65280};
	__property WordWrap = {default=0};
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
	/* TWinControl.CreateParented */ inline __fastcall TRzDBCheckBox(HWND ParentWindow) : Rzradchk::TRzCustomCheckBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbchk */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBCHK)
using namespace Rzdbchk;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbchkHPP
