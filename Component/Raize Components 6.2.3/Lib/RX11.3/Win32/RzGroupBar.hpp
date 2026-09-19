// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzGroupBar.pas' rev: 35.00 (Windows)

#ifndef RzgroupbarHPP
#define RzgroupbarHPP

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
#include <RzCommon.hpp>
#include <RzSpnEdt.hpp>
#include <System.Actions.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzgroupbar
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS ERzGroupBarError;
class DELPHICLASS TRzGroupItemActionLink;
struct TCMGroupItemMsg;
class DELPHICLASS TRzGroupItem;
class DELPHICLASS TRzGroupItems;
class DELPHICLASS TRzGroup;
class DELPHICLASS TRzGroupTemplateItemActionLink;
class DELPHICLASS TRzGroupTemplateItem;
class DELPHICLASS TRzGroupTemplateItems;
class DELPHICLASS TRzGroupTemplatePreviewOptions;
class DELPHICLASS TRzGroupTemplate;
class DELPHICLASS TRzGroupBar;
class DELPHICLASS TRzGroupController;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION ERzGroupBarError : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall ERzGroupBarError(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall ERzGroupBarError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall ERzGroupBarError(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall ERzGroupBarError(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzGroupBarError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall ERzGroupBarError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall ERzGroupBarError(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall ERzGroupBarError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzGroupBarError(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ERzGroupBarError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzGroupBarError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ERzGroupBarError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~ERzGroupBarError() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzCaptionState : unsigned char { csNormal, csHot, csDown };

typedef void __fastcall (__closure *TRzCanOpenEvent)(System::TObject* Sender, bool &CanOpen);

typedef void __fastcall (__closure *TRzCanCloseEvent)(System::TObject* Sender, bool &CanClose);

enum DECLSPEC_DENUM TRzGroupBarStyle : unsigned char { gbsCategoryView, gbsTaskList, gbsOutlook };

enum DECLSPEC_DENUM TRzItemStyle : unsigned char { isSmall, isLarge };

enum DECLSPEC_DENUM TRzCaptionStyle : unsigned char { csSmall, csLarge };

enum DECLSPEC_DENUM TRzItemSelectionStyle : unsigned char { issImage, issCaption, issImageAndCaption };

typedef System::TMetaClass* TRzGroupClass;

class PASCALIMPLEMENTATION TRzGroupItemActionLink : public Vcl::Actnlist::TActionLink
{
	typedef Vcl::Actnlist::TActionLink inherited;
	
protected:
	TRzGroupItem* FClient;
	virtual void __fastcall AssignClient(System::TObject* AClient);
	virtual bool __fastcall DoShowHint(System::UnicodeString &HintStr);
	virtual bool __fastcall IsCaptionLinked();
	virtual bool __fastcall IsCheckedLinked();
	virtual bool __fastcall IsEnabledLinked();
	virtual bool __fastcall IsHintLinked();
	virtual bool __fastcall IsImageIndexLinked();
	virtual bool __fastcall IsVisibleLinked();
	virtual bool __fastcall IsOnExecuteLinked();
	virtual void __fastcall SetCaption(const System::UnicodeString Value);
	virtual void __fastcall SetChecked(bool Value);
	virtual void __fastcall SetEnabled(bool Value);
	virtual void __fastcall SetHint(const System::UnicodeString Value);
	virtual void __fastcall SetImageIndex(int Value);
	virtual void __fastcall SetVisible(bool Value);
	virtual void __fastcall SetOnExecute(System::Classes::TNotifyEvent Value);
public:
	/* TBasicActionLink.Create */ inline __fastcall virtual TRzGroupItemActionLink(System::TObject* AClient) : Vcl::Actnlist::TActionLink(AClient) { }
	/* TBasicActionLink.Destroy */ inline __fastcall virtual ~TRzGroupItemActionLink() { }
	
};


typedef System::TMetaClass* TRzGroupItemActionLinkClass;

#pragma pack(push,1)
struct DECLSPEC_DRECORD TCMGroupItemMsg
{
public:
	unsigned Msg;
	int Unused;
	TRzGroupItem* Sender;
	NativeInt Result;
};
#pragma pack(pop)


class PASCALIMPLEMENTATION TRzGroupItem : public System::Classes::TCollectionItem
{
	typedef System::Classes::TCollectionItem inherited;
	
private:
	Vcl::Controls::TCaption FCaption;
	bool FCaptionChanged;
	System::Uitypes::TImageIndex FDisabledIndex;
	bool FEnabled;
	System::Uitypes::TColor FFontColor;
	System::Uitypes::TFontStyles FFontStyle;
	System::UnicodeString FHint;
	System::Uitypes::TImageIndex FImageIndex;
	System::Byte FIndentLevel;
	bool FVisible;
	bool FSelected;
	int FTag;
	void *FData;
	System::Classes::TNotifyEvent FOnClick;
	TRzGroupItemActionLink* FActionLink;
	bool FClickingCaption;
	TRzCaptionState FCaptionState;
	System::Types::TRect FHotCaptionRect;
	System::Types::TRect FHotImageRect;
	void __fastcall ActionChangeHandler(System::TObject* Sender);
	MESSAGE void __fastcall CMGroupItemSelected(TCMGroupItemMsg &Msg);
	
protected:
	virtual System::UnicodeString __fastcall GetDisplayName();
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	bool __fastcall IsCaptionStored();
	bool __fastcall IsEnabledStored();
	bool __fastcall IsHintStored();
	bool __fastcall IsHelpContextStored();
	bool __fastcall IsImageIndexStored();
	bool __fastcall IsVisibleStored();
	bool __fastcall IsOnClickStored();
	virtual System::Classes::TBasicAction* __fastcall GetAction();
	virtual void __fastcall SetAction(System::Classes::TBasicAction* Value);
	DYNAMIC TRzGroupItemActionLinkClass __fastcall GetActionLinkClass();
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetEnabled(bool Value);
	virtual void __fastcall SetFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFontStyle(System::Uitypes::TFontStyles Value);
	virtual TRzGroup* __fastcall GetGroup();
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetIndentLevel(System::Byte Value);
	virtual void __fastcall SetVisible(bool Value);
	virtual void __fastcall SetSelected(bool Value);
	void __fastcall UpdateSelection();
	__property TRzGroupItemActionLink* ActionLink = {read=FActionLink, write=FActionLink};
	
public:
	__fastcall virtual TRzGroupItem(System::Classes::TCollection* Collection);
	__fastcall virtual ~TRzGroupItem();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	virtual void __fastcall AssignTo(System::Classes::TPersistent* Dest);
	virtual void __fastcall InitiateAction();
	DYNAMIC void __fastcall Click();
	__property System::Types::TRect HotCaptionRect = {read=FHotCaptionRect, write=FHotCaptionRect};
	__property System::Types::TRect HotImageRect = {read=FHotImageRect, write=FHotImageRect};
	__property TRzCaptionState CaptionState = {read=FCaptionState, write=FCaptionState, nodefault};
	__property bool ClickingCaption = {read=FClickingCaption, write=FClickingCaption, nodefault};
	__property void * Data = {read=FData, write=FData};
	__property TRzGroup* Group = {read=GetGroup};
	
__published:
	__property System::Classes::TBasicAction* Action = {read=GetAction, write=SetAction};
	__property Vcl::Controls::TCaption Caption = {read=FCaption, write=SetCaption, stored=IsCaptionStored};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property bool Enabled = {read=FEnabled, write=SetEnabled, stored=IsEnabledStored, default=1};
	__property System::Uitypes::TColor FontColor = {read=FFontColor, write=SetFontColor, default=536870911};
	__property System::Uitypes::TFontStyles FontStyle = {read=FFontStyle, write=SetFontStyle, default=0};
	__property System::UnicodeString Hint = {read=FHint, write=FHint, stored=IsHintStored};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, stored=IsImageIndexStored, default=-1};
	__property System::Byte IndentLevel = {read=FIndentLevel, write=SetIndentLevel, default=0};
	__property bool Selected = {read=FSelected, write=SetSelected, default=0};
	__property int Tag = {read=FTag, write=FTag, default=0};
	__property bool Visible = {read=FVisible, write=SetVisible, default=1};
	__property System::Classes::TNotifyEvent OnClick = {read=FOnClick, write=FOnClick, stored=IsOnClickStored};
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupItems : public System::Classes::TCollection
{
	typedef System::Classes::TCollection inherited;
	
public:
	TRzGroupItem* operator[](int Index) { return this->Items[Index]; }
	
private:
	TRzGroup* FGroup;
	HIDESBASE TRzGroupItem* __fastcall GetItem(int Index);
	HIDESBASE void __fastcall SetItem(int Index, TRzGroupItem* Value);
	
protected:
	DYNAMIC System::Classes::TPersistent* __fastcall GetOwner();
	virtual void __fastcall Update(System::Classes::TCollectionItem* Item);
	
public:
	__fastcall TRzGroupItems(TRzGroup* Group);
	HIDESBASE TRzGroupItem* __fastcall Add();
	__property TRzGroupItem* Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
	__property TRzGroup* Group = {read=FGroup};
public:
	/* TCollection.Destroy */ inline __fastcall virtual ~TRzGroupItems() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzGroupItemHotZone : unsigned char { ihzImageCaption, ihzItemWidth };

typedef void __fastcall (__closure *TRzMouseOverItemEvent)(System::TObject* Sender, TRzGroupItem* Item);

typedef void __fastcall (__closure *TRzGroupItemPopupMenuEvent)(System::TObject* Sender, TRzGroupItem* Item);

class PASCALIMPLEMENTATION TRzGroup : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	TRzGroupBar* FGroupBar;
	TRzGroupController* FGroupController;
	int FOpenedHeight;
	bool FOpened;
	bool FCanClose;
	TRzGroupBarStyle FStyle;
	bool FColorDefault;
	System::Uitypes::TColor FCaptionColor;
	System::Uitypes::TColor FCaptionColorStart;
	System::Uitypes::TColor FCaptionColorStop;
	bool FCaptionColorDefault;
	System::Uitypes::TColor FCaptionHotColor;
	System::Uitypes::TImageIndex FCaptionImageIndex;
	int FCaptionHeight;
	Vcl::Graphics::TFont* FCaptionFont;
	bool FCaptionFontChanged;
	System::Uitypes::TColor FItemHotColor;
	TRzGroupItemHotZone FItemHotZone;
	bool FFontChanged;
	Vcl::Graphics::TFont* FItemStaticFont;
	bool FItemStaticFontChanged;
	bool FDividerVisible;
	System::Uitypes::TColor FDividerColor;
	System::Uitypes::TColor FBorderColor;
	bool FSpecial;
	System::Uitypes::TColor FSelectionColor;
	System::Uitypes::TColor FSelectionFontColor;
	System::Uitypes::TColor FSelectionShadowColor;
	System::Uitypes::TColor FSelectionFrameColor;
	bool FHideAccelerators;
	bool FShowItemSelection;
	TRzItemSelectionStyle FItemSelectionStyle;
	bool FIgnoreSelectedItemClick;
	TRzCaptionState FCaptionState;
	bool FClickingCaption;
	HICON FCaptionCursor;
	TRzGroupItems* FItems;
	TRzItemStyle FItemStyle;
	int FItemHeight;
	System::Byte FItemIndent;
	int FTopItem;
	int FActionClientCount;
	TRzCaptionStyle FCaptionStyle;
	int FItemIndex;
	TRzGroupItem* FOverItem;
	void *FData;
	Vcl::Imglist::TCustomImageList* FSmallImages;
	Vcl::Imglist::TChangeLink* FSmallImagesChangeLink;
	Vcl::Imglist::TCustomImageList* FLargeImages;
	Vcl::Imglist::TChangeLink* FLargeImagesChangeLink;
	Rzspnedt::TRzRapidFireButton* FScrollUpBtn;
	Rzspnedt::TRzRapidFireButton* FScrollDownBtn;
	TRzCanCloseEvent FOnCanClose;
	TRzCanOpenEvent FOnCanOpen;
	System::Classes::TNotifyEvent FOnClose;
	System::Classes::TNotifyEvent FOnOpen;
	TRzMouseOverItemEvent FOnMouseOverItem;
	Vcl::Menus::TPopupMenu* FItemPopupMenu;
	TRzGroupItem* FPopupItem;
	TRzGroupItemPopupMenuEvent FOnItemPopupMenu;
	int __fastcall DrawCaptionImage();
	int __fastcall CaptionImageHeight();
	int __fastcall CaptionYOffset();
	int __fastcall HeaderHeight();
	void __fastcall ImagesChange(System::TObject* Sender);
	void __fastcall CaptionFontChangeHandler(System::TObject* Sender);
	void __fastcall ItemStaticFontChangeHandler(System::TObject* Sender);
	void __fastcall ScrollUpHandler(System::TObject* Sender);
	void __fastcall ScrollDownHandler(System::TObject* Sender);
	MESSAGE void __fastcall CMGroupItemSelected(TCMGroupItemMsg &Msg);
	HIDESBASE MESSAGE void __fastcall CMDesignHitTest(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMTextChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMHintShow(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSetCursor(Winapi::Messages::TWMSetCursor &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMWindowPosChanged(Winapi::Messages::TWMWindowPosMsg &Msg);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	void __fastcall CreateScrollButtons();
	void __fastcall UpdateScrollBtnPositions();
	void __fastcall UpdateScrollBtnVisibility();
	DYNAMIC void __fastcall Resize();
	void __fastcall UpdateGroupBarLayout();
	virtual int __fastcall CalcBaseXOffset();
	bool __fastcall SkipSeparator(int Index);
	virtual void __fastcall DrawItems();
	virtual void __fastcall DrawOpenCloseButton(bool Opened, int Left, int Top, System::Uitypes::TColor FillColor, System::Uitypes::TColor LineColor, System::Uitypes::TColor ChevronColor);
	virtual void __fastcall DrawCategoryGroup();
	virtual void __fastcall DrawThemedCategoryGroup();
	virtual void __fastcall DrawOutlookGroup();
	virtual void __fastcall DrawTaskListGroup();
	virtual void __fastcall Paint();
	virtual void __fastcall Loaded();
	virtual void __fastcall ReadState(System::Classes::TReader* Reader);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* AParent);
	virtual void __fastcall AlignControls(Vcl::Controls::TControl* AControl, System::Types::TRect &Rect);
	int __fastcall ClosedHeight();
	DYNAMIC void __fastcall ChangeScale(int M, int D)/* overload */;
	System::Types::TRect __fastcall CaptionRect();
	System::Types::TPoint __fastcall CursorPosition();
	virtual void __fastcall ToggleState();
	virtual void __fastcall WndProc(Winapi::Messages::TMessage &Msg);
	DYNAMIC void __fastcall DragOver(System::TObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	DYNAMIC void __fastcall DoEndDrag(System::TObject* Target, int X, int Y);
	virtual void __fastcall ItemDestroyed(int Index);
	void __fastcall DisplayItemPopupMenu(int Index, int X, int Y);
	void __fastcall DrawOutOfBounds();
	void __fastcall InvalidateGroupBar();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseMove(System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall DoContextPopup(const System::Types::TPoint &MousePos, bool &Handled);
	DYNAMIC void __fastcall MouseOverItem(TRzGroupItem* Item);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCanClose(bool Value);
	System::Uitypes::TColor __fastcall DefaultCaptionColor(TRzGroupBarStyle Style);
	virtual void __fastcall SetCaptionColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionColorStop(System::Uitypes::TColor Value);
	bool __fastcall IsCaptionColorStored();
	virtual void __fastcall SetCaptionColorDefault(bool Value);
	bool __fastcall IsCaptionFontStored();
	virtual void __fastcall SetCaptionFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetCaptionHeight(int Value);
	virtual void __fastcall SetCaptionHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetCaptionStyle(const TRzCaptionStyle Value);
	virtual void __fastcall SetItemHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotZone(TRzGroupItemHotZone Value);
	bool __fastcall IsItemStaticFontStored();
	virtual void __fastcall SetItemStaticFont(Vcl::Graphics::TFont* Value);
	bool __fastcall StoreColor();
	virtual System::Uitypes::TColor __fastcall GetColor();
	HIDESBASE virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetColorDefault(bool Value);
	System::Uitypes::TColor __fastcall DefaultColor(TRzGroupBarStyle Style);
	virtual void __fastcall SetSelectionColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHideAccelerators(bool Value);
	virtual void __fastcall SetShowItemSelection(bool Value);
	virtual void __fastcall SetItemSelectionStyle(TRzItemSelectionStyle Value);
	virtual TRzGroupItem* __fastcall GetSelectedItem();
	virtual void __fastcall SetItems(TRzGroupItems* Value);
	virtual void __fastcall SetItemStyle(TRzItemStyle Value);
	virtual void __fastcall SetItemHeight(int Value);
	virtual void __fastcall SetItemIndent(System::Byte Value);
	virtual void __fastcall SetItemIndex(int Value);
	virtual void __fastcall SetOpened(bool Value);
	virtual void __fastcall SetOpenedHeight(int Value);
	virtual void __fastcall SetLargeImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetGroupBar(TRzGroupBar* Value);
	virtual int __fastcall GetGroupIndex();
	virtual void __fastcall SetGroupIndex(int Value);
	virtual void __fastcall SetSmallImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetStyle(TRzGroupBarStyle Value);
	virtual void __fastcall SetDividerVisible(bool Value);
	virtual void __fastcall SetDividerColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemPopupMenu(Vcl::Menus::TPopupMenu* Value);
	virtual void __fastcall SetSpecial(bool Value);
	virtual void __fastcall SetTopItem(int Value);
	bool __fastcall NotUsingGroupController();
	virtual void __fastcall SetGroupController(TRzGroupController* Value);
	__property TRzGroupBarStyle Style = {read=FStyle, write=SetStyle, default=0};
	
public:
	__fastcall virtual TRzGroup(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzGroup();
	void __fastcall Open();
	void __fastcall Close();
	void __fastcall AddToGroup(TRzGroupTemplate* Template);
	Vcl::Imglist::TCustomImageList* __fastcall SmallImageList();
	Vcl::Imglist::TCustomImageList* __fastcall LargeImageList();
	Vcl::Imglist::TCustomImageList* __fastcall CaptionImageList();
	virtual void __fastcall InitiateAction();
	void __fastcall ActionClientConnect();
	void __fastcall ActionClientDisconnect();
	void __fastcall AssignActionList(Vcl::Actnlist::TCustomActionList* ActionList, const System::UnicodeString Category = System::UnicodeString());
	void __fastcall Reposition();
	int __fastcall CalculateHeight(int W);
	int __fastcall ItemAtPos(const System::Types::TPoint &P);
	int __fastcall ItemsVisible();
	__property void * Data = {read=FData, write=FData};
	__property TRzGroupBar* GroupBar = {read=FGroupBar, write=SetGroupBar};
	__property int ItemIndex = {read=FItemIndex, write=SetItemIndex, nodefault};
	__property TRzGroupItem* SelectedItem = {read=GetSelectedItem};
	__property TRzGroupItem* PopupItem = {read=FPopupItem};
	__property int TopItem = {read=FTopItem, write=SetTopItem, nodefault};
	
__published:
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, stored=NotUsingGroupController, default=-16777196};
	__property bool CanClose = {read=FCanClose, write=SetCanClose, default=1};
	__property System::Uitypes::TColor CaptionColor = {read=FCaptionColor, write=SetCaptionColor, stored=IsCaptionColorStored, nodefault};
	__property System::Uitypes::TColor CaptionColorStart = {read=FCaptionColorStart, write=SetCaptionColorStart, default=-16777211};
	__property System::Uitypes::TColor CaptionColorStop = {read=FCaptionColorStop, write=SetCaptionColorStop, default=-16777201};
	__property bool CaptionColorDefault = {read=FCaptionColorDefault, write=SetCaptionColorDefault, stored=NotUsingGroupController, default=1};
	__property Vcl::Graphics::TFont* CaptionFont = {read=FCaptionFont, write=SetCaptionFont, stored=IsCaptionFontStored};
	__property int CaptionHeight = {read=FCaptionHeight, write=SetCaptionHeight, stored=NotUsingGroupController, default=20};
	__property System::Uitypes::TColor CaptionHotColor = {read=FCaptionHotColor, write=SetCaptionHotColor, stored=NotUsingGroupController, default=-16777190};
	__property System::Uitypes::TImageIndex CaptionImageIndex = {read=FCaptionImageIndex, write=SetCaptionImageIndex, default=-1};
	__property TRzCaptionStyle CaptionStyle = {read=FCaptionStyle, write=SetCaptionStyle, default=0};
	__property System::Uitypes::TColor Color = {read=GetColor, write=SetColor, stored=StoreColor, nodefault};
	__property bool ColorDefault = {read=FColorDefault, write=SetColorDefault, stored=NotUsingGroupController, default=1};
	__property TRzGroupController* GroupController = {read=FGroupController, write=SetGroupController};
	__property int GroupIndex = {read=GetGroupIndex, write=SetGroupIndex, stored=false, nodefault};
	__property TRzGroupItems* Items = {read=FItems, write=SetItems};
	__property int ItemHeight = {read=FItemHeight, write=SetItemHeight, stored=NotUsingGroupController, default=20};
	__property System::Uitypes::TColor ItemHotColor = {read=FItemHotColor, write=SetItemHotColor, stored=NotUsingGroupController, default=-16777190};
	__property TRzGroupItemHotZone ItemHotZone = {read=FItemHotZone, write=SetItemHotZone, stored=NotUsingGroupController, default=0};
	__property Vcl::Graphics::TFont* ItemStaticFont = {read=FItemStaticFont, write=SetItemStaticFont, stored=IsItemStaticFontStored};
	__property System::Byte ItemIndent = {read=FItemIndent, write=SetItemIndent, stored=NotUsingGroupController, default=20};
	__property TRzItemStyle ItemStyle = {read=FItemStyle, write=SetItemStyle, stored=NotUsingGroupController, default=0};
	__property Vcl::Menus::TPopupMenu* ItemPopupMenu = {read=FItemPopupMenu, write=SetItemPopupMenu};
	__property Vcl::Imglist::TCustomImageList* LargeImages = {read=FLargeImages, write=SetLargeImages};
	__property bool Opened = {read=FOpened, write=SetOpened, nodefault};
	__property int OpenedHeight = {read=FOpenedHeight, write=SetOpenedHeight, default=50};
	__property System::Uitypes::TColor DividerColor = {read=FDividerColor, write=SetDividerColor, stored=NotUsingGroupController, default=-16777203};
	__property bool DividerVisible = {read=FDividerVisible, write=SetDividerVisible, stored=NotUsingGroupController, nodefault};
	__property Vcl::Imglist::TCustomImageList* SmallImages = {read=FSmallImages, write=SetSmallImages};
	__property System::Uitypes::TColor SelectionColor = {read=FSelectionColor, write=SetSelectionColor, stored=NotUsingGroupController, default=-16777201};
	__property System::Uitypes::TColor SelectionFontColor = {read=FSelectionFontColor, write=SetSelectionFontColor, default=-16777208};
	__property System::Uitypes::TColor SelectionShadowColor = {read=FSelectionShadowColor, write=SetSelectionShadowColor, stored=NotUsingGroupController, default=-16777200};
	__property System::Uitypes::TColor SelectionFrameColor = {read=FSelectionFrameColor, write=SetSelectionFrameColor, stored=NotUsingGroupController, default=-16777195};
	__property bool HideAccelerators = {read=FHideAccelerators, write=SetHideAccelerators, stored=NotUsingGroupController, default=1};
	__property bool ShowItemSelection = {read=FShowItemSelection, write=SetShowItemSelection, stored=NotUsingGroupController, default=0};
	__property TRzItemSelectionStyle ItemSelectionStyle = {read=FItemSelectionStyle, write=SetItemSelectionStyle, stored=NotUsingGroupController, default=2};
	__property bool IgnoreSelectedItemClick = {read=FIgnoreSelectedItemClick, write=FIgnoreSelectedItemClick, default=1};
	__property bool Special = {read=FSpecial, write=SetSpecial, stored=NotUsingGroupController, default=0};
	__property TRzCanCloseEvent OnCanClose = {read=FOnCanClose, write=FOnCanClose};
	__property TRzCanOpenEvent OnCanOpen = {read=FOnCanOpen, write=FOnCanOpen};
	__property System::Classes::TNotifyEvent OnClose = {read=FOnClose, write=FOnClose};
	__property System::Classes::TNotifyEvent OnOpen = {read=FOnOpen, write=FOnOpen};
	__property TRzMouseOverItemEvent OnMouseOverItem = {read=FOnMouseOverItem, write=FOnMouseOverItem};
	__property TRzGroupItemPopupMenuEvent OnItemPopupMenu = {read=FOnItemPopupMenu, write=FOnItemPopupMenu};
	__property BiDiMode;
	__property Caption = {default=0};
	__property DragCursor = {default=-12};
	__property DragMode = {default=0};
	__property Font;
	__property Height = {stored=false};
	__property Left = {stored=false};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Top = {stored=false};
	__property Touch;
	__property Visible = {default=1};
	__property Width = {stored=false};
	__property OnDblClick;
	__property OnDragDrop;
	__property OnDragOver;
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
	__property OnResize;
	__property OnStartDrag;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzGroup(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
	/* Hoisted overloads: */
	
protected:
	DYNAMIC inline void __fastcall  ChangeScale(int M, int D, bool isDpiChange){ Vcl::Controls::TWinControl::ChangeScale(M, D, isDpiChange); }
	
};


class PASCALIMPLEMENTATION TRzGroupTemplateItemActionLink : public Vcl::Actnlist::TActionLink
{
	typedef Vcl::Actnlist::TActionLink inherited;
	
protected:
	TRzGroupTemplateItem* FClient;
	virtual void __fastcall AssignClient(System::TObject* AClient);
	virtual bool __fastcall IsCaptionLinked();
	virtual bool __fastcall IsEnabledLinked();
	virtual bool __fastcall IsHintLinked();
	virtual bool __fastcall IsImageIndexLinked();
	virtual bool __fastcall IsVisibleLinked();
	virtual bool __fastcall IsOnExecuteLinked();
	virtual void __fastcall SetCaption(const System::UnicodeString Value);
	virtual void __fastcall SetEnabled(bool Value);
	virtual void __fastcall SetHint(const System::UnicodeString Value);
	virtual void __fastcall SetImageIndex(int Value);
	virtual void __fastcall SetVisible(bool Value);
	virtual void __fastcall SetOnExecute(System::Classes::TNotifyEvent Value);
public:
	/* TBasicActionLink.Create */ inline __fastcall virtual TRzGroupTemplateItemActionLink(System::TObject* AClient) : Vcl::Actnlist::TActionLink(AClient) { }
	/* TBasicActionLink.Destroy */ inline __fastcall virtual ~TRzGroupTemplateItemActionLink() { }
	
};


typedef System::TMetaClass* TRzGroupTemplateItemActionLinkClass;

class PASCALIMPLEMENTATION TRzGroupTemplateItem : public System::Classes::TCollectionItem
{
	typedef System::Classes::TCollectionItem inherited;
	
private:
	Vcl::Controls::TCaption FCaption;
	bool FCaptionChanged;
	System::Uitypes::TImageIndex FDisabledIndex;
	bool FEnabled;
	System::Uitypes::TColor FFontColor;
	System::Uitypes::TFontStyles FFontStyle;
	System::UnicodeString FHint;
	System::Uitypes::TImageIndex FImageIndex;
	System::Byte FIndentLevel;
	bool FVisible;
	int FTag;
	void *FData;
	System::Classes::TNotifyEvent FOnClick;
	TRzGroupTemplateItemActionLink* FActionLink;
	void __fastcall ActionChangeHandler(System::TObject* Sender);
	
protected:
	virtual System::UnicodeString __fastcall GetDisplayName();
	DYNAMIC void __fastcall ActionChange(System::TObject* Sender, bool CheckDefaults);
	bool __fastcall IsCaptionStored();
	bool __fastcall IsEnabledStored();
	bool __fastcall IsHintStored();
	bool __fastcall IsHelpContextStored();
	bool __fastcall IsImageIndexStored();
	bool __fastcall IsVisibleStored();
	bool __fastcall IsOnClickStored();
	virtual System::Classes::TBasicAction* __fastcall GetAction();
	virtual void __fastcall SetAction(System::Classes::TBasicAction* Value);
	DYNAMIC TRzGroupTemplateItemActionLinkClass __fastcall GetActionLinkClass();
	virtual void __fastcall SetCaption(const Vcl::Controls::TCaption Value);
	virtual void __fastcall SetDisabledIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetEnabled(bool Value);
	virtual void __fastcall SetFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFontStyle(System::Uitypes::TFontStyles Value);
	virtual TRzGroupTemplate* __fastcall GetTemplate();
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetIndentLevel(System::Byte Value);
	virtual void __fastcall SetVisible(bool Value);
	__property TRzGroupTemplateItemActionLink* ActionLink = {read=FActionLink, write=FActionLink};
	
public:
	__fastcall virtual TRzGroupTemplateItem(System::Classes::TCollection* Collection);
	__fastcall virtual ~TRzGroupTemplateItem();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	virtual void __fastcall AssignTo(System::Classes::TPersistent* Dest);
	virtual void __fastcall InitiateAction();
	__property void * Data = {read=FData, write=FData};
	__property TRzGroupTemplate* Template = {read=GetTemplate};
	
__published:
	__property System::Classes::TBasicAction* Action = {read=GetAction, write=SetAction};
	__property Vcl::Controls::TCaption Caption = {read=FCaption, write=SetCaption, stored=IsCaptionStored};
	__property System::Uitypes::TImageIndex DisabledIndex = {read=FDisabledIndex, write=SetDisabledIndex, default=-1};
	__property bool Enabled = {read=FEnabled, write=SetEnabled, stored=IsEnabledStored, default=1};
	__property System::Uitypes::TColor FontColor = {read=FFontColor, write=SetFontColor, default=536870911};
	__property System::Uitypes::TFontStyles FontStyle = {read=FFontStyle, write=SetFontStyle, default=0};
	__property System::UnicodeString Hint = {read=FHint, write=FHint, stored=IsHintStored};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, stored=IsImageIndexStored, default=-1};
	__property System::Byte IndentLevel = {read=FIndentLevel, write=SetIndentLevel, default=0};
	__property int Tag = {read=FTag, write=FTag, default=0};
	__property bool Visible = {read=FVisible, write=SetVisible, default=1};
	__property System::Classes::TNotifyEvent OnClick = {read=FOnClick, write=FOnClick, stored=IsOnClickStored};
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupTemplateItems : public System::Classes::TCollection
{
	typedef System::Classes::TCollection inherited;
	
public:
	TRzGroupTemplateItem* operator[](int Index) { return this->Items[Index]; }
	
private:
	TRzGroupTemplate* FTemplate;
	HIDESBASE TRzGroupTemplateItem* __fastcall GetItem(int Index);
	HIDESBASE void __fastcall SetItem(int Index, TRzGroupTemplateItem* Value);
	
protected:
	DYNAMIC System::Classes::TPersistent* __fastcall GetOwner();
	
public:
	__fastcall TRzGroupTemplateItems(TRzGroupTemplate* Template);
	HIDESBASE TRzGroupTemplateItem* __fastcall Add();
	__property TRzGroupTemplateItem* Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
	__property TRzGroupTemplate* Template = {read=FTemplate};
public:
	/* TCollection.Destroy */ inline __fastcall virtual ~TRzGroupTemplateItems() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupTemplatePreviewOptions : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
private:
	TRzGroupTemplate* FTemplate;
	TRzItemStyle FItemStyle;
	Vcl::Imglist::TCustomImageList* FSmallImages;
	Vcl::Imglist::TChangeLink* FSmallImagesChangeLink;
	Vcl::Imglist::TCustomImageList* FLargeImages;
	Vcl::Imglist::TChangeLink* FLargeImagesChangeLink;
	
protected:
	void __fastcall SetLargeImages(Vcl::Imglist::TCustomImageList* Value);
	void __fastcall SetSmallImages(Vcl::Imglist::TCustomImageList* Value);
	
public:
	__fastcall TRzGroupTemplatePreviewOptions(TRzGroupTemplate* ATemplate);
	__fastcall virtual ~TRzGroupTemplatePreviewOptions();
	
__published:
	__property TRzItemStyle ItemStyle = {read=FItemStyle, write=FItemStyle, default=0};
	__property Vcl::Imglist::TCustomImageList* LargeImages = {read=FLargeImages, write=SetLargeImages};
	__property Vcl::Imglist::TCustomImageList* SmallImages = {read=FSmallImages, write=SetSmallImages};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzGroupTemplate : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FCanClose;
	System::UnicodeString FCaption;
	System::Uitypes::TImageIndex FCaptionImageIndex;
	TRzCaptionStyle FCaptionStyle;
	bool FOpened;
	bool FSpecial;
	TRzGroupTemplateItems* FItems;
	Vcl::Menus::TPopupMenu* FItemPopupMenu;
	Vcl::Menus::TPopupMenu* FPopupMenu;
	TRzGroupItemPopupMenuEvent FOnItemPopupMenu;
	TRzGroupTemplatePreviewOptions* FPreviewOptions;
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall SetItems(TRzGroupTemplateItems* Value);
	virtual void __fastcall SetItemPopupMenu(Vcl::Menus::TPopupMenu* Value);
	virtual void __fastcall SetPopupMenu(Vcl::Menus::TPopupMenu* Value);
	void __fastcall SetPreviewOptions(TRzGroupTemplatePreviewOptions* Value);
	
public:
	__fastcall virtual TRzGroupTemplate(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzGroupTemplate();
	void __fastcall AssignActionList(Vcl::Actnlist::TCustomActionList* ActionList, const System::UnicodeString Category = System::UnicodeString());
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool CanClose = {read=FCanClose, write=FCanClose, default=1};
	__property System::UnicodeString Caption = {read=FCaption, write=FCaption};
	__property System::Uitypes::TImageIndex CaptionImageIndex = {read=FCaptionImageIndex, write=FCaptionImageIndex, default=-1};
	__property TRzCaptionStyle CaptionStyle = {read=FCaptionStyle, write=FCaptionStyle, default=0};
	__property TRzGroupTemplateItems* Items = {read=FItems, write=SetItems};
	__property Vcl::Menus::TPopupMenu* ItemPopupMenu = {read=FItemPopupMenu, write=SetItemPopupMenu};
	__property Vcl::Menus::TPopupMenu* PopupMenu = {read=FPopupMenu, write=SetPopupMenu};
	__property bool Opened = {read=FOpened, write=FOpened, default=1};
	__property TRzGroupTemplatePreviewOptions* PreviewOptions = {read=FPreviewOptions, write=SetPreviewOptions};
	__property bool Special = {read=FSpecial, write=FSpecial, default=0};
	__property TRzGroupItemPopupMenuEvent OnItemPopupMenu = {read=FOnItemPopupMenu, write=FOnItemPopupMenu};
};


class PASCALIMPLEMENTATION TRzGroupBar : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	// __classmethod void __fastcall Create@();
	// __classmethod void __fastcall Destroy@();
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Graphics::TCanvas* FCanvas;
	System::Classes::TList* FGroups;
	bool FUpdatingLayout;
	int FUpdateLayoutCount;
	TRzGroupBarStyle FStyle;
	Vcl::Controls::TBevelWidth FBevelWidth;
	Rzcommon::TFrameStyleEx FBorderInner;
	Rzcommon::TFrameStyleEx FBorderOuter;
	Rzcommon::TSides FBorderSides;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FBorderHighlight;
	System::Uitypes::TColor FBorderShadow;
	System::Uitypes::TColor FFlatColor;
	int FFlatColorAdjustment;
	Rzcommon::TRzVisualStyle FVisualStyle;
	Rzcommon::TRzGradientColorStyle FGradientColorStyle;
	System::Uitypes::TColor FGradientColorStart;
	System::Uitypes::TColor FGradientColorStop;
	Rzcommon::TRzGroupBarGradientPath FGradientPath;
	int FGroupBorderSize;
	bool FScrolling;
	int FScrollPosition;
	bool FExclusiveMode;
	bool FUniqueItemSelection;
	Vcl::Imglist::TCustomImageList* FSmallImages;
	Vcl::Imglist::TChangeLink* FSmallImagesChangeLink;
	Vcl::Imglist::TCustomImageList* FLargeImages;
	Vcl::Imglist::TChangeLink* FLargeImagesChangeLink;
	void __fastcall ImagesChange(System::TObject* Sender);
	MESSAGE void __fastcall CMGroupItemSelected(TCMGroupItemMsg &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMFocusChanged(Vcl::Controls::TCMFocusChanged &Msg);
	HIDESBASE MESSAGE void __fastcall WMVScroll(Winapi::Messages::TWMScroll &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCHitTest(Winapi::Messages::TWMNCHitTest &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCCalcSize(Winapi::Messages::TWMNCCalcSize &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	MESSAGE void __fastcall WMThemeChanged(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall UMResetScrollPosition(Winapi::Messages::TMessage &Msg);
	MESSAGE void __fastcall CMStyleChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall Loaded();
	DYNAMIC void __fastcall SetChildOrder(System::Classes::TComponent* Child, int Order);
	virtual void __fastcall SetParent(Vcl::Controls::TWinControl* AParent);
	void __fastcall RepaintBorder();
	void __fastcall RepaintGroups();
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall Resize();
	virtual System::Types::TRect __fastcall GetControlExtents();
	void __fastcall PositionGroups();
	int __fastcall GetTotalHeight();
	void __fastcall ScrollControls(int Delta);
	virtual void __fastcall UpdateScrollBar();
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	HIDESBASE virtual void __fastcall SetBevelWidth(Vcl::Controls::TBevelWidth Value);
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderInner(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderOuter(Rzcommon::TFrameStyleEx Value);
	virtual void __fastcall SetBorderSides(Rzcommon::TSides Value);
	virtual void __fastcall SetBorderHighlight(System::Uitypes::TColor Value);
	virtual void __fastcall SetBorderShadow(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatColorAdjustment(int Value);
	virtual void __fastcall SetGradientColorStyle(Rzcommon::TRzGradientColorStyle Value);
	virtual void __fastcall SetGradientColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetGradientPath(Rzcommon::TRzGroupBarGradientPath Value);
	virtual void __fastcall SetGroupBorderSize(int Value);
	virtual TRzGroup* __fastcall GetGroup(int Index);
	virtual int __fastcall GetGroupCount();
	virtual void __fastcall SetExclusiveMode(bool Value);
	virtual void __fastcall SetLargeImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetSmallImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetScrollPosition(int Value);
	virtual void __fastcall SetStyle(TRzGroupBarStyle Value);
	virtual void __fastcall SetUniqueItemSelection(bool Value);
	virtual void __fastcall SetVisualStyle(Rzcommon::TRzVisualStyle Value);
	__property int TotalHeight = {read=GetTotalHeight, nodefault};
	
public:
	__fastcall virtual TRzGroupBar(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzGroupBar();
	DYNAMIC void __fastcall GetChildren(System::Classes::TGetChildProc Proc, System::Classes::TComponent* Root);
	void __fastcall ShowEntireGroup(TRzGroup* Group);
	void __fastcall ScrollInView(Vcl::Controls::TControl* AControl);
	bool __fastcall IsScrollBarVisible();
	void __fastcall UpdateLayout(TRzGroup* Group = (TRzGroup*)(0x0));
	void __fastcall BeginUpdateLayout();
	void __fastcall EndUpdateLayout();
	virtual TRzGroupClass __fastcall GroupClass();
	TRzGroup* __fastcall CreateGroupFromTemplate(TRzGroupTemplate* Template);
	void __fastcall AddGroup(TRzGroup* Group)/* overload */;
	TRzGroup* __fastcall AddGroup(TRzGroupTemplate* Template)/* overload */;
	void __fastcall InsertGroup(int Index, TRzGroup* Group)/* overload */;
	TRzGroup* __fastcall InsertGroup(int Index, TRzGroupTemplate* Template)/* overload */;
	void __fastcall RemoveGroup(TRzGroup* Group);
	void __fastcall CloseAllGroups();
	void __fastcall OpenAllGroups();
	__property int GroupCount = {read=GetGroupCount, nodefault};
	__property TRzGroup* Groups[int Index] = {read=GetGroup};
	__property int ScrollPosition = {read=FScrollPosition, write=SetScrollPosition, nodefault};
	__property DockManager;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool ExclusiveMode = {read=FExclusiveMode, write=SetExclusiveMode, default=0};
	__property Vcl::Controls::TBevelWidth BevelWidth = {read=FBevelWidth, write=SetBevelWidth, default=1};
	__property Rzcommon::TFrameStyleEx BorderInner = {read=FBorderInner, write=SetBorderInner, default=0};
	__property Rzcommon::TFrameStyleEx BorderOuter = {read=FBorderOuter, write=SetBorderOuter, default=0};
	__property Rzcommon::TSides BorderSides = {read=FBorderSides, write=SetBorderSides, default=15};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777201};
	__property System::Uitypes::TColor BorderHighlight = {read=FBorderHighlight, write=SetBorderHighlight, default=-16777196};
	__property System::Uitypes::TColor BorderShadow = {read=FBorderShadow, write=SetBorderShadow, default=-16777200};
	__property Rzcommon::TRzGradientColorStyle GradientColorStyle = {read=FGradientColorStyle, write=SetGradientColorStyle, default=0};
	__property System::Uitypes::TColor GradientColorStart = {read=FGradientColorStart, write=SetGradientColorStart, default=16777215};
	__property System::Uitypes::TColor GradientColorStop = {read=FGradientColorStop, write=SetGradientColorStop, default=-16777201};
	__property Rzcommon::TRzGroupBarGradientPath GradientPath = {read=FGradientPath, write=SetGradientPath, default=0};
	__property System::Uitypes::TColor FlatColor = {read=FFlatColor, write=SetFlatColor, default=-16777200};
	__property int FlatColorAdjustment = {read=FFlatColorAdjustment, write=SetFlatColorAdjustment, default=30};
	__property Vcl::Imglist::TCustomImageList* LargeImages = {read=FLargeImages, write=SetLargeImages};
	__property Vcl::Imglist::TCustomImageList* SmallImages = {read=FSmallImages, write=SetSmallImages};
	__property TRzGroupBarStyle Style = {read=FStyle, write=SetStyle, default=0};
	__property int GroupBorderSize = {read=FGroupBorderSize, write=SetGroupBorderSize, nodefault};
	__property bool UniqueItemSelection = {read=FUniqueItemSelection, write=SetUniqueItemSelection, default=0};
	__property Rzcommon::TRzVisualStyle VisualStyle = {read=FVisualStyle, write=SetVisualStyle, default=2};
	__property Align = {default=3};
	__property Anchors = {default=3};
	__property AutoSize = {default=0};
	__property BiDiMode;
	__property BorderWidth = {default=0};
	__property Caption = {default=0};
	__property Color;
	__property Constraints;
	__property Ctl3D;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property Font;
	__property Height;
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=1};
	__property ParentCtl3D = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property TabOrder = {default=-1};
	__property TabStop = {default=0};
	__property Touch;
	__property Visible = {default=1};
	__property Width = {default=160};
	__property OnCanResize;
	__property OnClick;
	__property OnConstrainedResize;
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
	/* TWinControl.CreateParented */ inline __fastcall TRzGroupBar(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzGroupProperty : unsigned char { gpAll, gpBorderColor, gpCaptionColor, gpCaptionColorStart, gpCaptionColorStop, gpCaptionFont, gpCaptionHeight, gpCaptionHotColor, gpColor, gpParentColor, gpDividerColor, gpDividerVisible, gpFont, gpParentFont, gpItemHeight, gpItemHotColor, gpItemHotZone, gpItemIndent, gpItemStaticFont, gpSelectionColor, gpSelectionFontColor, gpSelectionShadowColor, gpSelectionFrameColor, gpShowItemSelection, gpItemSelectionStyle, gpIgnoreSelectedItemClick, gpHideAccelerators };

class PASCALIMPLEMENTATION TRzGroupController : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::Classes::TList* FGroupList;
	int FUpdateCount;
	Rzcommon::TRzRegIniFile* FRegIniFile;
	System::Uitypes::TColor FBorderColor;
	System::Uitypes::TColor FCaptionColor;
	System::Uitypes::TColor FCaptionColorStart;
	System::Uitypes::TColor FCaptionColorStop;
	Vcl::Graphics::TFont* FCaptionFont;
	bool FCaptionFontChanged;
	int FCaptionHeight;
	System::Uitypes::TColor FCaptionHotColor;
	System::Uitypes::TColor FColor;
	System::Uitypes::TColor FDividerColor;
	bool FDividerVisible;
	System::UnicodeString FDefaultFontName;
	int FDefaultFontSize;
	Vcl::Graphics::TFont* FFont;
	bool FFontChanged;
	int FItemHeight;
	System::Uitypes::TColor FItemHotColor;
	TRzGroupItemHotZone FItemHotZone;
	System::Byte FItemIndent;
	Vcl::Graphics::TFont* FItemStaticFont;
	bool FItemStaticFontChanged;
	bool FParentColor;
	bool FParentFont;
	System::Uitypes::TColor FSelectionColor;
	System::Uitypes::TColor FSelectionFontColor;
	System::Uitypes::TColor FSelectionShadowColor;
	System::Uitypes::TColor FSelectionFrameColor;
	bool FShowItemSelection;
	TRzItemSelectionStyle FItemSelectionStyle;
	bool FIgnoreSelectedItemClick;
	bool FHideAccelerators;
	void __fastcall FontChangeHandler(System::TObject* Sender);
	void __fastcall CaptionFontChangeHandler(System::TObject* Sender);
	void __fastcall ItemStaticFontChangeHandler(System::TObject* Sender);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateGroupProperty(TRzGroup* G, TRzGroupProperty GroupProperty);
	virtual void __fastcall UpdateAllGroups(TRzGroupProperty GroupProperty);
	void __fastcall UpdateFonts();
	virtual void __fastcall SetBorderColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionColorStart(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionColorStop(System::Uitypes::TColor Value);
	virtual void __fastcall SetCaptionFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetCaptionHeight(int Value);
	virtual void __fastcall SetCaptionHotColor(System::Uitypes::TColor Value);
	bool __fastcall IsFontStored();
	virtual void __fastcall SetFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetItemHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemHotZone(TRzGroupItemHotZone Value);
	virtual void __fastcall SetItemStaticFont(Vcl::Graphics::TFont* Value);
	bool __fastcall StoreColor();
	virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionFontColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionShadowColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetSelectionFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetHideAccelerators(bool Value);
	virtual void __fastcall SetShowItemSelection(bool Value);
	virtual void __fastcall SetItemSelectionStyle(TRzItemSelectionStyle Value);
	virtual void __fastcall SetIgnoreSelectedItemClick(bool Value);
	virtual void __fastcall SetItemHeight(int Value);
	virtual void __fastcall SetItemIndent(System::Byte Value);
	virtual void __fastcall SetDividerVisible(bool Value);
	virtual void __fastcall SetDividerColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetParentColor(bool Value);
	virtual void __fastcall SetParentFont(bool Value);
	virtual void __fastcall SetRegIniFile(Rzcommon::TRzRegIniFile* Value);
	
public:
	__fastcall virtual TRzGroupController(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzGroupController();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	void __fastcall BeginUpdate();
	void __fastcall EndUpdate();
	void __fastcall UpdateGroups();
	void __fastcall SetDefaults(TRzGroupBarStyle Style);
	void __fastcall AddGroup(TRzGroup* G);
	void __fastcall RemoveGroup(TRzGroup* G);
	void __fastcall Load(const System::UnicodeString Section);
	void __fastcall Save(const System::UnicodeString Section);
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor BorderColor = {read=FBorderColor, write=SetBorderColor, default=-16777196};
	__property System::Uitypes::TColor CaptionColor = {read=FCaptionColor, write=SetCaptionColor, default=-16777202};
	__property System::Uitypes::TColor CaptionColorStart = {read=FCaptionColorStart, write=SetCaptionColorStart, default=-16777211};
	__property System::Uitypes::TColor CaptionColorStop = {read=FCaptionColorStop, write=SetCaptionColorStop, default=-16777201};
	__property Vcl::Graphics::TFont* CaptionFont = {read=FCaptionFont, write=SetCaptionFont, stored=FCaptionFontChanged};
	__property int CaptionHeight = {read=FCaptionHeight, write=SetCaptionHeight, default=20};
	__property System::Uitypes::TColor CaptionHotColor = {read=FCaptionHotColor, write=SetCaptionHotColor, default=-16777190};
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, stored=StoreColor, default=-16777201};
	__property System::Uitypes::TColor DividerColor = {read=FDividerColor, write=SetDividerColor, default=-16777203};
	__property bool DividerVisible = {read=FDividerVisible, write=SetDividerVisible, default=0};
	__property Vcl::Graphics::TFont* Font = {read=FFont, write=SetFont, stored=IsFontStored};
	__property int ItemHeight = {read=FItemHeight, write=SetItemHeight, default=20};
	__property System::Uitypes::TColor ItemHotColor = {read=FItemHotColor, write=SetItemHotColor, default=-16777190};
	__property TRzGroupItemHotZone ItemHotZone = {read=FItemHotZone, write=SetItemHotZone, default=0};
	__property System::Byte ItemIndent = {read=FItemIndent, write=SetItemIndent, default=20};
	__property Vcl::Graphics::TFont* ItemStaticFont = {read=FItemStaticFont, write=SetItemStaticFont, stored=FItemStaticFontChanged};
	__property bool ParentColor = {read=FParentColor, write=SetParentColor, default=0};
	__property bool ParentFont = {read=FParentFont, write=SetParentFont, default=1};
	__property System::Uitypes::TColor SelectionColor = {read=FSelectionColor, write=SetSelectionColor, default=-16777201};
	__property System::Uitypes::TColor SelectionFontColor = {read=FSelectionFontColor, write=SetSelectionFontColor, default=-16777208};
	__property System::Uitypes::TColor SelectionShadowColor = {read=FSelectionShadowColor, write=SetSelectionShadowColor, default=-16777200};
	__property System::Uitypes::TColor SelectionFrameColor = {read=FSelectionFrameColor, write=SetSelectionFrameColor, default=-16777195};
	__property bool HideAccelerators = {read=FHideAccelerators, write=SetHideAccelerators, default=1};
	__property bool ShowItemSelection = {read=FShowItemSelection, write=SetShowItemSelection, default=0};
	__property TRzItemSelectionStyle ItemSelectionStyle = {read=FItemSelectionStyle, write=SetItemSelectionStyle, default=2};
	__property bool IgnoreSelectedItemClick = {read=FIgnoreSelectedItemClick, write=SetIgnoreSelectedItemClick, default=1};
	__property Rzcommon::TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
};


//-- var, const, procedure ---------------------------------------------------
static const System::Word um_ResetScrollPosition = System::Word(0x2424);
extern DELPHI_PACKAGE System::ResourceString _sRzCannotLoadGroupController;
#define Rzgroupbar_sRzCannotLoadGroupController System::LoadResourceString(&Rzgroupbar::_sRzCannotLoadGroupController)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotSaveGroupController;
#define Rzgroupbar_sRzCannotSaveGroupController System::LoadResourceString(&Rzgroupbar::_sRzCannotSaveGroupController)
}	/* namespace Rzgroupbar */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZGROUPBAR)
using namespace Rzgroupbar;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzgroupbarHPP
