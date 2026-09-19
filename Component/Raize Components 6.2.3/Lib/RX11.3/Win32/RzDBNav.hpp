// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBNav.pas' rev: 35.00 (Windows)

#ifndef RzdbnavHPP
#define RzdbnavHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <Winapi.Messages.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Data.DB.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <System.Classes.hpp>
#include <RzCommon.hpp>
#include <RzPanel.hpp>
#include <RzButton.hpp>
#include <Vcl.Menus.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbnav
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBNavigatorImageIndexes;
class DELPHICLASS TRzDBNavigator;
class DELPHICLASS TRzNavigatorToolButton;
class DELPHICLASS TRzNavigatorDataLink;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBNavigatorImageIndexes : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzDBNavigator* FNavigator;
	System::Uitypes::TImageIndex FFirst;
	System::Uitypes::TImageIndex FFirstDisabled;
	System::Uitypes::TImageIndex FPrevious;
	System::Uitypes::TImageIndex FPreviousDisabled;
	System::Uitypes::TImageIndex FNext;
	System::Uitypes::TImageIndex FNextDisabled;
	System::Uitypes::TImageIndex FLast;
	System::Uitypes::TImageIndex FLastDisabled;
	System::Uitypes::TImageIndex FInsert;
	System::Uitypes::TImageIndex FInsertDisabled;
	System::Uitypes::TImageIndex FDelete;
	System::Uitypes::TImageIndex FDeleteDisabled;
	System::Uitypes::TImageIndex FEdit;
	System::Uitypes::TImageIndex FEditDisabled;
	System::Uitypes::TImageIndex FPost;
	System::Uitypes::TImageIndex FPostDisabled;
	System::Uitypes::TImageIndex FCancel;
	System::Uitypes::TImageIndex FCancelDisabled;
	System::Uitypes::TImageIndex FRefresh;
	System::Uitypes::TImageIndex FRefreshDisabled;
	
protected:
	virtual System::Uitypes::TImageIndex __fastcall GetImageIndex(int Index);
	virtual void __fastcall SetImageIndex(int Index, System::Uitypes::TImageIndex Value);
	
public:
	__fastcall TRzDBNavigatorImageIndexes(TRzDBNavigator* Navigator);
	__property TRzDBNavigator* Navigator = {read=FNavigator};
	
__published:
	__property System::Uitypes::TImageIndex First = {read=GetImageIndex, write=SetImageIndex, index=0, default=-1};
	__property System::Uitypes::TImageIndex FirstDisabled = {read=GetImageIndex, write=SetImageIndex, index=1, default=-1};
	__property System::Uitypes::TImageIndex Previous = {read=GetImageIndex, write=SetImageIndex, index=2, default=-1};
	__property System::Uitypes::TImageIndex PreviousDisabled = {read=GetImageIndex, write=SetImageIndex, index=3, default=-1};
	__property System::Uitypes::TImageIndex Next = {read=GetImageIndex, write=SetImageIndex, index=4, default=-1};
	__property System::Uitypes::TImageIndex NextDisabled = {read=GetImageIndex, write=SetImageIndex, index=5, default=-1};
	__property System::Uitypes::TImageIndex Last = {read=GetImageIndex, write=SetImageIndex, index=6, default=-1};
	__property System::Uitypes::TImageIndex LastDisabled = {read=GetImageIndex, write=SetImageIndex, index=7, default=-1};
	__property System::Uitypes::TImageIndex Insert = {read=GetImageIndex, write=SetImageIndex, index=8, default=-1};
	__property System::Uitypes::TImageIndex InsertDisabled = {read=GetImageIndex, write=SetImageIndex, index=9, default=-1};
	__property System::Uitypes::TImageIndex Delete = {read=GetImageIndex, write=SetImageIndex, index=10, default=-1};
	__property System::Uitypes::TImageIndex DeleteDisabled = {read=GetImageIndex, write=SetImageIndex, index=11, default=-1};
	__property System::Uitypes::TImageIndex Edit = {read=GetImageIndex, write=SetImageIndex, index=12, default=-1};
	__property System::Uitypes::TImageIndex EditDisabled = {read=GetImageIndex, write=SetImageIndex, index=13, default=-1};
	__property System::Uitypes::TImageIndex Post = {read=GetImageIndex, write=SetImageIndex, index=14, default=-1};
	__property System::Uitypes::TImageIndex PostDisabled = {read=GetImageIndex, write=SetImageIndex, index=15, default=-1};
	__property System::Uitypes::TImageIndex Cancel = {read=GetImageIndex, write=SetImageIndex, index=16, default=-1};
	__property System::Uitypes::TImageIndex CancelDisabled = {read=GetImageIndex, write=SetImageIndex, index=17, default=-1};
	__property System::Uitypes::TImageIndex Refresh = {read=GetImageIndex, write=SetImageIndex, index=18, default=-1};
	__property System::Uitypes::TImageIndex RefreshDisabled = {read=GetImageIndex, write=SetImageIndex, index=19, default=-1};
public:
	/* TPersistent.Destroy */ inline __fastcall virtual ~TRzDBNavigatorImageIndexes() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzNavigatorButton : unsigned char { nbFirst, nbPrior, nbNext, nbLast, nbInsert, nbDelete, nbEdit, nbPost, nbCancel, nbRefresh };

typedef System::Set<TRzNavigatorButton, TRzNavigatorButton::nbFirst, TRzNavigatorButton::nbRefresh> TRzNavigatorButtons;

enum DECLSPEC_DENUM Rzdbnav__2 : unsigned char { nbsAllowTimer, nbsFocusRect };

typedef System::Set<Rzdbnav__2, Rzdbnav__2::nbsAllowTimer, Rzdbnav__2::nbsFocusRect> TRzNavigatorButtonStyle;

typedef void __fastcall (__closure *TRzNavigatorButtonClickEvent)(System::TObject* Sender, TRzNavigatorButton Button);

enum DECLSPEC_DENUM TRzNavigatorEnablementStyle : unsigned char { nesTraditional, nesNoBrowseOnEdit };

class PASCALIMPLEMENTATION TRzDBNavigator : public Rzpanel::TRzCustomPanel
{
	typedef Rzpanel::TRzCustomPanel inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	TRzNavigatorDataLink* FDataLink;
	TRzNavigatorButtons FVisibleButtons;
	System::Classes::TStrings* FHints;
	System::Classes::TStrings* FDefHints;
	int ButtonWidth;
	System::Types::TPoint MinBtnSize;
	TRzNavigatorButtonClickEvent FOnNavClick;
	TRzNavigatorButtonClickEvent FBeforeAction;
	TRzNavigatorButton FocusedButton;
	bool FConfirmDelete;
	bool FFlat;
	TRzNavigatorEnablementStyle FEnablementStyle;
	Vcl::Imglist::TCustomImageList* FInternalImages;
	TRzDBNavigatorImageIndexes* FImageIndexes;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	void __fastcall BtnMouseDown(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	void __fastcall ClickHandler(System::TObject* Sender);
	void __fastcall HintsChanged(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetFocus(Winapi::Messages::TWMSetFocus &Msg);
	HIDESBASE MESSAGE void __fastcall WMKillFocus(Winapi::Messages::TWMKillFocus &Msg);
	MESSAGE void __fastcall WMGetDlgCode(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanging(Winapi::Messages::TWMWindowPosMsg &Msg);
	HIDESBASE MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	System::StaticArray<TRzNavigatorToolButton*, 10> Buttons;
	void __fastcall LoadImages();
	void __fastcall InitButtons();
	void __fastcall InitHints();
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DataChanged();
	virtual void __fastcall EditingChanged();
	void __fastcall ActiveChanged();
	void __fastcall UpdateImages(TRzNavigatorButton NavBtn);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	void __fastcall CalcMinSize(int &W, int &H);
	virtual Rzcommon::TRzGradientColorStyle __fastcall GetButtonGradientColorStyle();
	virtual void __fastcall SetButtonGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual System::Uitypes::TColor __fastcall GetButtonSelectionColorStart();
	virtual void __fastcall SetButtonSelectionColorStart(System::Uitypes::TColor Value);
	virtual System::Uitypes::TColor __fastcall GetButtonSelectionColorStop();
	virtual void __fastcall SetButtonSelectionColorStop(System::Uitypes::TColor Value);
	virtual System::Uitypes::TColor __fastcall GetButtonSelectionFrameColor();
	virtual void __fastcall SetButtonSelectionFrameColor(System::Uitypes::TColor Value);
	virtual Rzcommon::TRzVisualStyle __fastcall GetButtonVisualStyle();
	virtual void __fastcall SetButtonVisualStyle(Rzcommon::TRzVisualStyle Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual void __fastcall SetFlat(bool Value);
	virtual System::Classes::TStrings* __fastcall GetHints();
	virtual void __fastcall SetHints(System::Classes::TStrings* Value);
	virtual void __fastcall SetImageIndexes(TRzDBNavigatorImageIndexes* Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetSize(int &W, int &H);
	HIDESBASE virtual void __fastcall SetVisible(TRzNavigatorButtons Value);
	
public:
	__fastcall virtual TRzDBNavigator(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBNavigator();
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	virtual void __fastcall SetBounds(int ALeft, int ATop, int AWidth, int AHeight);
	virtual void __fastcall BtnClick(TRzNavigatorButton NavBtn);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property Rzcommon::TRzGradientColorStyle ButtonGradientColorStyle = {read=GetButtonGradientColorStyle, write=SetButtonGradientColorStyle, default=0};
	__property System::Uitypes::TColor ButtonSelectionColorStart = {read=GetButtonSelectionColorStart, write=SetButtonSelectionColorStart, default=-16777201};
	__property System::Uitypes::TColor ButtonSelectionColorStop = {read=GetButtonSelectionColorStop, write=SetButtonSelectionColorStop, default=-16777200};
	__property System::Uitypes::TColor ButtonSelectionFrameColor = {read=GetButtonSelectionFrameColor, write=SetButtonSelectionFrameColor, default=-16777195};
	__property Rzcommon::TRzVisualStyle ButtonVisualStyle = {read=GetButtonVisualStyle, write=SetButtonVisualStyle, default=1};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ConfirmDelete = {read=FConfirmDelete, write=FConfirmDelete, default=1};
	__property TRzNavigatorEnablementStyle EnablementStyle = {read=FEnablementStyle, write=FEnablementStyle, default=0};
	__property TRzNavigatorButtons VisibleButtons = {read=FVisibleButtons, write=SetVisible, default=1023};
	__property bool Flat = {read=FFlat, write=SetFlat, default=1};
	__property System::Classes::TStrings* Hints = {read=GetHints, write=SetHints};
	__property TRzDBNavigatorImageIndexes* ImageIndexes = {read=FImageIndexes, write=SetImageIndexes};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property TRzNavigatorButtonClickEvent BeforeAction = {read=FBeforeAction, write=FBeforeAction};
	__property TRzNavigatorButtonClickEvent OnClick = {read=FOnNavClick, write=FOnNavClick};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property BevelWidth = {default=1};
	__property BiDiMode;
	__property BorderInner = {default=0};
	__property BorderOuter = {default=6};
	__property BorderSides = {default=15};
	__property BorderColor = {default=-16777201};
	__property BorderHighlight = {default=-16777196};
	__property BorderShadow = {default=-16777200};
	__property BorderWidth = {default=0};
	__property Color = {default=-16777201};
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property FlatColor = {default=-16777200};
	__property FlatColorAdjustment = {default=30};
	__property FullRepaint = {default=1};
	__property GradientColorStyle = {default=0};
	__property GradientColorStart = {default=16777215};
	__property GradientColorStop = {default=-16777201};
	__property GradientDirection = {default=2};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Transparent = {default=0};
	__property Visible = {default=1};
	__property VisualStyle = {default=1};
	__property OnCanResize;
	__property OnConstrainedResize;
	__property OnContextPopup;
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnPaint;
	__property OnResize;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnUnDock;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzDBNavigator(HWND ParentWindow) : Rzpanel::TRzCustomPanel(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzNavigatorToolButton : public Rzbutton::TRzToolButton
{
	typedef Rzbutton::TRzToolButton inherited;
	
private:
	TRzNavigatorButton FNavBtn;
	TRzNavigatorButtonStyle FNavStyle;
	Vcl::Extctrls::TTimer* FRepeatTimer;
	void __fastcall TimerExpired(System::TObject* Sender);
	
protected:
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	
public:
	__fastcall virtual ~TRzNavigatorToolButton();
	__property TRzNavigatorButtonStyle NavStyle = {read=FNavStyle, write=FNavStyle, nodefault};
	__property TRzNavigatorButton NavBtn = {read=FNavBtn, write=FNavBtn, nodefault};
public:
	/* TRzToolButton.Create */ inline __fastcall virtual TRzNavigatorToolButton(System::Classes::TComponent* AOwner) : Rzbutton::TRzToolButton(AOwner) { }
	
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzNavigatorDataLink : public Data::Db::TDataLink
{
	typedef Data::Db::TDataLink inherited;
	
private:
	TRzDBNavigator* FNavigator;
	
protected:
	virtual void __fastcall EditingChanged();
	virtual void __fastcall DataSetChanged();
	virtual void __fastcall ActiveChanged();
	
public:
	__fastcall TRzNavigatorDataLink(TRzDBNavigator* Navigator);
	__fastcall virtual ~TRzNavigatorDataLink();
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::ResourceString _SRzFirstRecord;
#define Rzdbnav_SRzFirstRecord System::LoadResourceString(&Rzdbnav::_SRzFirstRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzPriorRecord;
#define Rzdbnav_SRzPriorRecord System::LoadResourceString(&Rzdbnav::_SRzPriorRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzNextRecord;
#define Rzdbnav_SRzNextRecord System::LoadResourceString(&Rzdbnav::_SRzNextRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzLastRecord;
#define Rzdbnav_SRzLastRecord System::LoadResourceString(&Rzdbnav::_SRzLastRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzInsertRecord;
#define Rzdbnav_SRzInsertRecord System::LoadResourceString(&Rzdbnav::_SRzInsertRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzDeleteRecord;
#define Rzdbnav_SRzDeleteRecord System::LoadResourceString(&Rzdbnav::_SRzDeleteRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzEditRecord;
#define Rzdbnav_SRzEditRecord System::LoadResourceString(&Rzdbnav::_SRzEditRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzPostEdit;
#define Rzdbnav_SRzPostEdit System::LoadResourceString(&Rzdbnav::_SRzPostEdit)
extern DELPHI_PACKAGE System::ResourceString _SRzCancelEdit;
#define Rzdbnav_SRzCancelEdit System::LoadResourceString(&Rzdbnav::_SRzCancelEdit)
extern DELPHI_PACKAGE System::ResourceString _SRzRefreshRecord;
#define Rzdbnav_SRzRefreshRecord System::LoadResourceString(&Rzdbnav::_SRzRefreshRecord)
extern DELPHI_PACKAGE System::ResourceString _SRzDeleteRecordQuestion;
#define Rzdbnav_SRzDeleteRecordQuestion System::LoadResourceString(&Rzdbnav::_SRzDeleteRecordQuestion)
}	/* namespace Rzdbnav */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBNAV)
using namespace Rzdbnav;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbnavHPP
