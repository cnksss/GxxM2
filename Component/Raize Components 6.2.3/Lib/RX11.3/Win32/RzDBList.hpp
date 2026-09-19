// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBList.pas' rev: 35.00 (Windows)

#ifndef RzdblistHPP
#define RzdblistHPP

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
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <RzLstBox.hpp>
#include <Vcl.DBCtrls.hpp>
#include <Data.DB.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdblist
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBListBox;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBListBox : public Rzlstbox::TRzCustomListBox
{
	typedef Rzlstbox::TRzCustomListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FJumpingToEdit;
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	void __fastcall DataChange(System::TObject* Sender);
	void __fastcall UpdateData(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMLButtonDown(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	
protected:
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyPress(System::WideChar &Key);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TField* __fastcall GetField();
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual bool __fastcall GetReadOnly();
	virtual void __fastcall SetReadOnly(bool Value);
	HIDESBASE virtual void __fastcall SetItems(System::Classes::TStrings* Value);
	
public:
	__fastcall virtual TRzDBListBox(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBListBox();
	__property Data::Db::TField* Field = {read=GetField};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ReadOnly = {read=GetReadOnly, write=SetReadOnly, default=0};
	__property Items = {write=SetItems};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BeepOnInvalidKey = {default=1};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property Color = {default=-16777211};
	__property Columns = {default=0};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property FocusColor = {default=-16777211};
	__property FrameColor = {default=-16777200};
	__property FrameControllerNotifications = {default=65535};
	__property FrameController;
	__property FrameHotColor = {default=-16777200};
	__property FrameHotTrack = {default=0};
	__property FrameHotStyle = {default=10};
	__property FrameSides = {default=15};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property GroupColor = {default=-16777197};
	__property GroupFont;
	__property GroupPrefix = {default=0};
	__property HorzScrollBar = {default=0};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property IntegralHeight = {default=0};
	__property ItemHeight = {default=16};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowGroups = {default=0};
	__property ShowHint;
	__property ShowItemHints = {default=1};
	__property Sorted = {default=0};
	__property Style = {default=0};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property TabWidth = {default=0};
	__property UseGradients = {default=1};
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawItem;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMatch;
	__property OnMeasureItem;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnStartDock;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBListBox(HWND ParentWindow) : Rzlstbox::TRzCustomListBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdblist */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBLIST)
using namespace Rzdblist;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdblistHPP
