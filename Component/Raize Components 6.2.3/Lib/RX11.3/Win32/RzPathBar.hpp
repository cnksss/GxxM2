// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzPathBar.pas' rev: 35.00 (Windows)

#ifndef RzpathbarHPP
#define RzpathbarHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.ActnList.hpp>
#include <System.Classes.hpp>
#include <RzPanel.hpp>
#include <RzGroupBar.hpp>
#include <RzCommon.hpp>
#include <System.Actions.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzpathbar
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzPathItemActionLink;
class DELPHICLASS TRzPathItem;
class DELPHICLASS TRzPathItems;
class DELPHICLASS TRzPathBar;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzPathItemActionLink : public Vcl::Actnlist::TActionLink
{
	typedef Vcl::Actnlist::TActionLink inherited;
	
protected:
	TRzPathItem* FClient;
	virtual void __fastcall AssignClient(System::TObject* AClient);
	virtual bool __fastcall DoShowHint(System::UnicodeString &HintStr);
	virtual bool __fastcall IsCaptionLinked();
	virtual bool __fastcall IsHintLinked();
	virtual bool __fastcall IsImageIndexLinked();
	virtual bool __fastcall IsOnExecuteLinked();
	virtual void __fastcall SetCaption(const System::UnicodeString Value);
	virtual void __fastcall SetHint(const System::UnicodeString Value);
	virtual void __fastcall SetImageIndex(int Value);
	virtual void __fastcall SetOnExecute(System::Classes::TNotifyEvent Value);
public:
	/* TBasicActionLink.Create */ inline __fastcall virtual TRzPathItemActionLink(System::TObject* AClient) : Vcl::Actnlist::TActionLink(AClient) { }
	/* TBasicActionLink.Destroy */ inline __fastcall virtual ~TRzPathItemActionLink() { }
	
};


typedef System::TMetaClass* TRzPathItemActionLinkClass;

class PASCALIMPLEMENTATION TRzPathItem : public System::Classes::TCollectionItem
{
	typedef System::Classes::TCollectionItem inherited;
	
private:
	Vcl::Controls::TCaption FCaption;
	bool FCaptionChanged;
	System::Uitypes::TColor FHighlightColor;
	System::UnicodeString FHint;
	System::Uitypes::TImageIndex FImageIndex;
	System::UnicodeString FLinkInfo;
	System::TObject* FLinkObject;
	int FTag;
	System::Classes::TNotifyEvent FOnClick;
	TRzPathItemActionLink* FActionLink;
	bool FClickingCaption;
	Rzgroupbar::TRzCaptionState FCaptionState;
	System::Types::TRect FHotRect;
	void __fastcall ActionChangeHandler(System::TObject* Sender);
	
protected:
	virtual System::UnicodeString __fastcall GetDisplayName();
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	bool __fastcall IsCaptionStored();
	bool __fastcall IsHintStored();
	bool __fastcall IsHelpContextStored();
	bool __fastcall IsImageIndexStored();
	bool __fastcall IsOnClickStored();
	virtual System::Classes::TBasicAction* __fastcall GetAction();
	virtual void __fastcall SetAction(System::Classes::TBasicAction* Value);
	DYNAMIC TRzPathItemActionLinkClass __fastcall GetActionLinkClass();
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual TRzPathBar* __fastcall GetPathBar();
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	__property TRzPathItemActionLink* ActionLink = {read=FActionLink, write=FActionLink};
	
public:
	__fastcall virtual TRzPathItem(System::Classes::TCollection* Collection);
	__fastcall virtual ~TRzPathItem();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	virtual void __fastcall AssignTo(System::Classes::TPersistent* Dest);
	virtual void __fastcall InitiateAction();
	DYNAMIC void __fastcall Click();
	__property System::Types::TRect HotRect = {read=FHotRect, write=FHotRect};
	__property Rzgroupbar::TRzCaptionState CaptionState = {read=FCaptionState, write=FCaptionState, nodefault};
	__property bool ClickingCaption = {read=FClickingCaption, write=FClickingCaption, nodefault};
	__property System::TObject* LinkObject = {read=FLinkObject, write=FLinkObject};
	__property TRzPathBar* PathBar = {read=GetPathBar};
	
__published:
	__property System::Classes::TBasicAction* Action = {read=GetAction, write=SetAction};
	__property Vcl::Controls::TCaption Caption = {read=FCaption, write=SetCaption, stored=IsCaptionStored};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=536870911};
	__property System::UnicodeString Hint = {read=FHint, write=FHint, stored=IsHintStored};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, stored=IsImageIndexStored, default=-1};
	__property System::UnicodeString LinkInfo = {read=FLinkInfo, write=FLinkInfo};
	__property int Tag = {read=FTag, write=FTag, default=0};
	__property System::Classes::TNotifyEvent OnClick = {read=FOnClick, write=FOnClick, stored=IsOnClickStored};
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPathItems : public System::Classes::TCollection
{
	typedef System::Classes::TCollection inherited;
	
public:
	TRzPathItem* operator[](int Index) { return this->Items[Index]; }
	
private:
	TRzPathBar* FPathBar;
	HIDESBASE TRzPathItem* __fastcall GetItem(int Index);
	HIDESBASE void __fastcall SetItem(int Index, TRzPathItem* Value);
	
protected:
	DYNAMIC System::Classes::TPersistent* __fastcall GetOwner();
	virtual void __fastcall Update(System::Classes::TCollectionItem* Item);
	
public:
	__fastcall TRzPathItems(TRzPathBar* PathBar);
	HIDESBASE TRzPathItem* __fastcall Add();
	__property TRzPathItem* Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
	__property TRzPathBar* PathBar = {read=FPathBar};
public:
	/* TCollection.Destroy */ inline __fastcall virtual ~TRzPathItems() { }
	
};

#pragma pack(pop)

typedef void __fastcall (__closure *TRzMouseOverPathItemEvent)(System::TObject* Sender, TRzPathItem* Item);

typedef void __fastcall (__closure *TRzPathItemPopupMenuEvent)(System::TObject* Sender, TRzPathItem* Item);

enum DECLSPEC_DENUM TRzItemHotStyle : unsigned char { ihsBox, ihsUnderline };

class PASCALIMPLEMENTATION TRzPathBar : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	System::Uitypes::TColor FItemHotColor;
	System::Uitypes::TColor FItemDownColor;
	System::Uitypes::TColor FHighlightColor;
	bool FBoldLastItem;
	TRzItemHotStyle FItemHotStyle;
	Rzgroupbar::TRzCaptionState FCaptionState;
	HICON FItemCursor;
	TRzPathItems* FItems;
	int FActionClientCount;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	System::UnicodeString FSeparator;
	Vcl::Graphics::TFont* FSeparatorFont;
	TRzMouseOverPathItemEvent FOnMouseOverItem;
	Vcl::Menus::TPopupMenu* FItemPopupMenu;
	TRzPathItem* FPopupItem;
	TRzPathItemPopupMenuEvent FOnItemPopupMenu;
	void __fastcall ImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMHintShow(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall DrawItems();
	virtual void __fastcall Paint();
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	void __fastcall UpdateDownColors();
	DYNAMIC void __fastcall DragOver(System::TObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	DYNAMIC void __fastcall DoEndDrag(System::TObject* Target, int X, int Y);
	void __fastcall DisplayItemPopupMenu(int Index, int X, int Y);
	DYNAMIC void __fastcall Click();
	DYNAMIC void __fastcall DoContextPopup(const System::Types::TPoint &MousePos, bool &Handled);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseOverItem(TRzPathItem* Item);
	virtual void __fastcall SetBoldLastItem(bool Value);
	virtual void __fastcall SetItemHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotStyle(TRzItemHotStyle Value);
	virtual void __fastcall SetItems(TRzPathItems* Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetSeparator(const System::UnicodeString Value);
	virtual void __fastcall SetSeparatorFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetItemPopupMenu(Vcl::Menus::TPopupMenu* Value);
	
public:
	__fastcall virtual TRzPathBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzPathBar();
	virtual void __fastcall InitiateAction();
	void __fastcall ActionClientConnect();
	void __fastcall ActionClientDisconnect();
	void __fastcall AssignActionList(Vcl::Actnlist::TCustomActionList* ActionList, const System::UnicodeString Category = System::UnicodeString());
	int __fastcall ItemAtPos(const System::Types::TPoint &P);
	__property TRzPathItem* PopupItem = {read=FPopupItem};
	__property Canvas;
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool BoldLastItem = {read=FBoldLastItem, write=SetBoldLastItem, default=1};
	__property TRzPathItems* Items = {read=FItems, write=SetItems};
	__property System::Uitypes::TColor ItemHotColor = {read=FItemHotColor, write=SetItemHotColor, default=-16777190};
	__property Vcl::Menus::TPopupMenu* ItemPopupMenu = {read=FItemPopupMenu, write=SetItemPopupMenu};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777203};
	__property TRzItemHotStyle ItemHotStyle = {read=FItemHotStyle, write=SetItemHotStyle, default=0};
	__property System::UnicodeString Separator = {read=FSeparator, write=SetSeparator};
	__property Vcl::Graphics::TFont* SeparatorFont = {read=FSeparatorFont, write=SetSeparatorFont};
	__property TRzMouseOverPathItemEvent OnMouseOverItem = {read=FOnMouseOverItem, write=FOnMouseOverItem};
	__property TRzPathItemPopupMenuEvent OnItemPopupMenu = {read=FOnItemPopupMenu, write=FOnItemPopupMenu};
	__property Align = {default=1};
	__property Alignment = {default=2};
	__property AlignmentVertical = {default=1};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderInner = {default=0};
	__property BorderOuter = {default=0};
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
	__property Height = {default=24};
	__property Locked = {default=0};
	__property Padding;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
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
	/* TWinControl.CreateParented */ inline __fastcall TRzPathBar(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzpathbar */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZPATHBAR)
using namespace Rzpathbar;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzpathbarHPP
