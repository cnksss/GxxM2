// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBRGrp.pas' rev: 35.00 (Windows)

#ifndef RzdbrgrpHPP
#define RzdbrgrpHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzRadGrp.hpp>
#include <Vcl.Menus.hpp>
#include <Data.DB.hpp>
#include <RzPanel.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzCommon.hpp>
#include <RzRadChk.hpp>
#include <System.UITypes.hpp>
#include <Vcl.ImgList.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbrgrp
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBRadioGroup;
class DELPHICLASS TRzDBCheckBoxGroup;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBRadioGroup : public Rzradgrp::TRzCustomRadioGroup
{
	typedef Rzradgrp::TRzCustomRadioGroup inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	System::UnicodeString FValue;
	System::Classes::TStrings* FValues;
	bool FInSetValue;
	System::Classes::TNotifyEvent FOnChange;
	void __fastcall ActiveChange(System::TObject* Sender);
	void __fastcall DataChange(System::TObject* Sender);
	void __fastcall UpdateData(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	DYNAMIC void __fastcall Change();
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual bool __fastcall CanModify();
	virtual Data::Db::TField* __fastcall GetField();
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	virtual System::UnicodeString __fastcall GetButtonValue(int Index);
	virtual void __fastcall SetValue(const System::UnicodeString Value);
	virtual void __fastcall SetItems(System::Classes::TStrings* Value);
	virtual void __fastcall SetValues(System::Classes::TStrings* Value);
	__property Vcl::Dbctrls::TFieldDataLink* DataLink = {read=FDataLink};
	
public:
	__fastcall virtual TRzDBRadioGroup(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBRadioGroup();
	DYNAMIC bool __fastcall ExecuteAction(System::Classes::TBasicAction* Action);
	virtual bool __fastcall UpdateAction(System::Classes::TBasicAction* Action);
	void __fastcall AddItemValue(const System::UnicodeString Item, const System::UnicodeString Value);
	__property Data::Db::TField* Field = {read=GetField};
	__property System::UnicodeString Value = {read=FValue, write=SetValue};
	__property Buttons;
	__property ItemEnabled;
	__property ItemIndex = {default=-1};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property Items = {write=SetItems};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::Classes::TStrings* Values = {read=FValues, write=SetValues};
	__property System::Classes::TNotifyEvent OnChange = {read=FOnChange, write=FOnChange};
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
	__property ItemFrameColor = {default=-16777200};
	__property ItemHighlightColor = {default=-16777203};
	__property ItemHotTrack = {default=0};
	__property ItemHotTrackColor = {default=1350640};
	__property ItemHotTrackColorType = {default=1};
	__property ItemFont;
	__property ItemHeight;
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
	/* TWinControl.CreateParented */ inline __fastcall TRzDBRadioGroup(HWND ParentWindow) : Rzradgrp::TRzCustomRadioGroup(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzDBCheckBoxGroup : public Rzpanel::TRzCustomGroupBox
{
	typedef Rzpanel::TRzCustomGroupBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	System::UnicodeString FValueCheck;
	System::UnicodeString FValueUncheck;
	void __fastcall DataChange(System::TObject* Sender);
	void __fastcall UpdateData(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	MESSAGE void __fastcall CMGetDataLink(Winapi::Messages::TMessage &Msg);
	
protected:
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	bool __fastcall ValueMatch(const System::UnicodeString ValueList, const System::UnicodeString Value);
	virtual void __fastcall ChangeState();
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
	
public:
	__fastcall virtual TRzDBCheckBoxGroup(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBCheckBoxGroup();
	__property Checked = {default=0};
	__property Data::Db::TField* Field = {read=GetField};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property System::UnicodeString ValueChecked = {read=FValueCheck, write=SetValueCheck};
	__property System::UnicodeString ValueUnchecked = {read=FValueUncheck, write=SetValueUncheck};
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
	/* TWinControl.CreateParented */ inline __fastcall TRzDBCheckBoxGroup(HWND ParentWindow) : Rzpanel::TRzCustomGroupBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbrgrp */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBRGRP)
using namespace Rzdbrgrp;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbrgrpHPP
