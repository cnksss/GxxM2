// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzTreeVw.pas' rev: 35.00 (Windows)

#ifndef RztreevwHPP
#define RztreevwHPP

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
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ComCtrls.hpp>
#include <Winapi.CommCtrl.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ImgList.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztreevw
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomTreeView;
class DELPHICLASS TRzTreeView;
class DELPHICLASS TRzCheckTree;
//-- type declarations -------------------------------------------------------
typedef void __fastcall (__closure *TRzTvOnNodeContextMenuEvent)(System::TObject* aSender, Vcl::Comctrls::TTreeNode* aNode, System::Types::TPoint &aPos, Vcl::Menus::TPopupMenu* &aMenu);

class PASCALIMPLEMENTATION TRzCustomTreeView : public Vcl::Comctrls::TCustomTreeView
{
	typedef Vcl::Comctrls::TCustomTreeView inherited;
	
private:
	bool FUpdatingColor;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FNormalColor;
	System::Uitypes::TColor FFrameColor;
	Rzcommon::TRzFrameController* FFrameController;
	Rzcommon::TRzFrameControllerNotifications FFrameControllerNotifications;
	System::Uitypes::TColor FFrameHotColor;
	bool FFrameHotTrack;
	Rzcommon::TFrameStyleEx FFrameHotStyle;
	Rzcommon::TSides FFrameSides;
	Rzcommon::TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	Rzcommon::TFramingPreference FFramingPreference;
	bool FAutoSelect;
	Vcl::Graphics::TPen* FSelectionPen;
	int FItemHeight;
	int FItemHeightMargin;
	Vcl::Comctrls::TTreeNode* FRClickNode;
	TRzTvOnNodeContextMenuEvent FOnNodeContextMenu;
	bool FMenuAlreadyHandled;
	System::WideChar FPathDelimiter;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	void __fastcall PenChanged(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall WMNCPaint(Winapi::Messages::TWMNCPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnter(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMExit(Winapi::Messages::TWMNoParams &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseEnter(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMMouseLeave(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	HIDESBASE MESSAGE void __fastcall CMSysColorChange(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CNNotify(Winapi::Messages::TWMNotify &Msg);
	HIDESBASE MESSAGE void __fastcall WMRButtonUp(Winapi::Messages::TWMMouse &Msg);
	HIDESBASE MESSAGE void __fastcall WMContextMenu(Winapi::Messages::TMessage &Msg);
	
protected:
	Rzcommon::TRzAboutInfo FAboutInfo;
	Vcl::Controls::TControlCanvas* FCanvas;
	bool FOverControl;
	bool FRecreating;
	virtual void __fastcall CreateParams(Vcl::Controls::TCreateParams &Params);
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateItemHeight();
	virtual void __fastcall UpdateColors();
	virtual void __fastcall UpdateFrame(bool ViaMouse, bool InFocus);
	virtual void __fastcall RepaintFrame();
	DYNAMIC void __fastcall Collapse(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall Expand(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC bool __fastcall DoMouseWheelDown(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC bool __fastcall DoMouseWheelUp(System::Classes::TShiftState Shift, const System::Types::TPoint &MousePos);
	DYNAMIC void __fastcall DoPreNodeContextMenu();
	DYNAMIC void __fastcall DoNodeContextMenu(Vcl::Comctrls::TTreeNode* Node, const System::Types::TPoint &P);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState ShiftState);
	DYNAMIC void __fastcall NodeContextMenu(Vcl::Comctrls::TTreeNode* Node, System::Types::TPoint &Pos, Vcl::Menus::TPopupMenu* &Menu);
	HIDESBASE Vcl::Comctrls::TTreeNode* __fastcall GetSelected();
	HIDESBASE void __fastcall SetSelected(Vcl::Comctrls::TTreeNode* Value);
	virtual void __fastcall LoadTreeFromList(System::Classes::TStrings* List);
	virtual void __fastcall SaveTreeToList(System::Classes::TStrings* List);
	bool __fastcall StoreColor();
	bool __fastcall StoreFocusColor();
	bool __fastcall StoreDisabledColor();
	bool __fastcall StoreParentColor();
	bool __fastcall StoreFlatButtonColor();
	bool __fastcall StoreFlatButtons();
	bool __fastcall StoreFrameColor();
	bool __fastcall StoreFrameHotColor();
	bool __fastcall StoreFrameHotTrack();
	bool __fastcall StoreFrameHotStyle();
	bool __fastcall StoreFrameSides();
	bool __fastcall StoreFrameStyle();
	bool __fastcall StoreFrameVisible();
	bool __fastcall StoreFramingPreference();
	virtual bool __fastcall GetAutoExpand();
	HIDESBASE virtual void __fastcall SetAutoExpand(bool Value);
	virtual void __fastcall SetAutoSelect(bool Value);
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameController(Rzcommon::TRzFrameController* Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameSides(Rzcommon::TSides Value);
	virtual void __fastcall SetFrameStyle(Rzcommon::TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(Rzcommon::TFramingPreference Value);
	virtual void __fastcall SetItemHeight(int Value);
	virtual void __fastcall SetItemHeightMargin(int Value);
	virtual void __fastcall SetSelectionPen(Vcl::Graphics::TPen* Value);
	__property bool AutoExpand = {read=GetAutoExpand, write=SetAutoExpand, default=0};
	__property bool AutoSelect = {read=FAutoSelect, write=SetAutoSelect, default=0};
	__property Color = {stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, stored=StoreDisabledColor, default=-16777201};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, stored=StoreFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, stored=StoreFrameColor, default=-16777200};
	__property Rzcommon::TRzFrameControllerNotifications FrameControllerNotifications = {read=FFrameControllerNotifications, write=FFrameControllerNotifications, default=65535};
	__property Rzcommon::TRzFrameController* FrameController = {read=FFrameController, write=SetFrameController};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, stored=StoreFrameHotColor, default=-16777200};
	__property Rzcommon::TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, stored=StoreFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, stored=StoreFrameHotTrack, default=0};
	__property Rzcommon::TSides FrameSides = {read=FFrameSides, write=SetFrameSides, stored=StoreFrameSides, default=15};
	__property Rzcommon::TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, stored=StoreFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, stored=StoreFrameVisible, default=0};
	__property Rzcommon::TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, stored=StoreFramingPreference, default=1};
	__property int ItemHeight = {read=FItemHeight, write=SetItemHeight, default=0};
	__property int ItemHeightMargin = {read=FItemHeightMargin, write=SetItemHeightMargin, default=0};
	__property System::WideChar PathDelimiter = {read=FPathDelimiter, write=FPathDelimiter, default=92};
	__property Vcl::Graphics::TPen* SelectionPen = {read=FSelectionPen, write=SetSelectionPen};
	__property ParentColor = {default=0};
	__property TabStop = {default=1};
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
	__property TRzTvOnNodeContextMenuEvent OnNodeContextMenu = {read=FOnNodeContextMenu, write=FOnNodeContextMenu};
	
public:
	__fastcall virtual TRzCustomTreeView(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomTreeView();
	virtual bool __fastcall UseThemes();
	Vcl::Comctrls::TTreeNode* __fastcall NodeFromPath(System::UnicodeString Path);
	System::UnicodeString __fastcall PathFromNode(Vcl::Comctrls::TTreeNode* Node);
	void __fastcall SelectByPath(const System::UnicodeString Path);
	void __fastcall UpdateStateIndexDisplay(Vcl::Comctrls::TTreeNode* Node);
	HIDESBASE void __fastcall FullCollapse();
	HIDESBASE void __fastcall FullExpand();
	HIDESBASE void __fastcall LoadFromFile(const System::UnicodeString FileName)/* overload */;
	HIDESBASE void __fastcall LoadFromFile(const System::UnicodeString FileName, System::Sysutils::TEncoding* Encoding)/* overload */;
	HIDESBASE void __fastcall LoadFromStream(System::Classes::TStream* Stream)/* overload */;
	HIDESBASE void __fastcall LoadFromStream(System::Classes::TStream* Stream, System::Sysutils::TEncoding* Encoding)/* overload */;
	HIDESBASE void __fastcall SaveToFile(const System::UnicodeString FileName)/* overload */;
	HIDESBASE void __fastcall SaveToFile(const System::UnicodeString FileName, System::Sysutils::TEncoding* Encoding)/* overload */;
	HIDESBASE void __fastcall SaveToStream(System::Classes::TStream* Stream)/* overload */;
	HIDESBASE void __fastcall SaveToStream(System::Classes::TStream* Stream, System::Sysutils::TEncoding* Encoding)/* overload */;
	void __fastcall InvalidateNode(Vcl::Comctrls::TTreeNode* Node, bool TextOnly, bool EraseBkgnd);
	__property Vcl::Comctrls::TTreeNode* Selected = {read=GetSelected, write=SetSelected};
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCustomTreeView(HWND ParentWindow) : Vcl::Comctrls::TCustomTreeView(ParentWindow) { }
	
};


class PASCALIMPLEMENTATION TRzTreeView : public TRzCustomTreeView
{
	typedef TRzCustomTreeView inherited;
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property SelectionPen;
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoExpand = {default=0};
	__property AutoSelect = {default=0};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property BorderWidth = {default=0};
	__property ChangeDelay = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
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
	__property HideSelection = {default=1};
	__property HotTrack = {default=0};
	__property Images;
	__property Indent;
	__property ItemHeight = {default=0};
	__property ItemHeightMargin = {default=0};
	__property MultiSelect = {default=0};
	__property MultiSelectStyle = {default=1};
	__property PathDelimiter = {default=92};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=0};
	__property RightClickSelect = {default=0};
	__property RowSelect = {default=0};
	__property ShowButtons = {default=1};
	__property ShowHint;
	__property ShowLines = {default=1};
	__property ShowRoot = {default=1};
	__property SortType = {default=0};
	__property StateImages;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property ToolTips = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnAddition;
	__property OnAdvancedCustomDraw;
	__property OnAdvancedCustomDrawItem;
	__property OnCancelEdit;
	__property OnChange;
	__property OnChanging;
	__property OnClick;
	__property OnCollapsed;
	__property OnCollapsing;
	__property OnCompare;
	__property OnContextPopup;
	__property OnCreateNodeClass;
	__property OnCustomDraw;
	__property OnCustomDrawItem;
	__property OnDblClick;
	__property OnDeletion;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEdited;
	__property OnEditing;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnExpanded;
	__property OnExpanding;
	__property OnGesture;
	__property OnGetImageIndex;
	__property OnGetSelectedIndex;
	__property OnHint;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnNodeContextMenu;
	__property OnStartDock;
	__property OnStartDrag;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseWheel;
	__property OnMouseWheelUp;
	__property OnMouseWheelDown;
	__property Items;
public:
	/* TRzCustomTreeView.Create */ inline __fastcall virtual TRzTreeView(System::Classes::TComponent* AOwner) : TRzCustomTreeView(AOwner) { }
	/* TRzCustomTreeView.Destroy */ inline __fastcall virtual ~TRzTreeView() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzTreeView(HWND ParentWindow) : TRzCustomTreeView(ParentWindow) { }
	
};


enum DECLSPEC_DENUM TRzCheckState : unsigned char { csUnknown, csUnchecked, csChecked, csPartiallyChecked };

typedef void __fastcall (__closure *TRzCheckTreeChangingEvent)(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState, bool &AllowChange);

typedef void __fastcall (__closure *TRzCheckTreeChangeEvent)(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);

typedef void __fastcall (__closure *TRzCheckTreeCascadeCompleteEvent)(System::TObject* Sender, Vcl::Comctrls::TTreeNode* Node);

class PASCALIMPLEMENTATION TRzCheckTree : public TRzCustomTreeView
{
	typedef TRzCustomTreeView inherited;
	
private:
	int FSelectedItem;
	int FBmpWidth;
	int FImageWidth;
	bool FChangingState;
	bool FSuspendCascades;
	Vcl::Controls::TImageList* FCheckImages;
	bool FCascadeChecks;
	bool FSilentStateChanges;
	System::Uitypes::TColor FHighlightColor;
	TRzCheckTreeChangingEvent FOnStateChanging;
	TRzCheckTreeChangeEvent FOnStateChange;
	System::Classes::TNotifyEvent FOnUpdateChildren;
	TRzCheckTreeCascadeCompleteEvent FOnCascadeComplete;
	TRzCheckState __fastcall GetItemState(int AbsoluteIndex);
	void __fastcall SetItemState(int AbsoluteIndex, TRzCheckState Value);
	void __fastcall SetNodeCheckState(Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);
	void __fastcall RecurseChildren(Vcl::Comctrls::TTreeNode* Node, bool NodeChecked);
	void __fastcall SetAllChildren(Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);
	HIDESBASE MESSAGE void __fastcall WMPaint(Winapi::Messages::TWMPaint &Msg);
	HIDESBASE MESSAGE void __fastcall CMColorChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall Loaded();
	virtual void __fastcall UpdateItemHeight();
	virtual void __fastcall UpdateImageWidth();
	virtual void __fastcall InitStateImages();
	virtual void __fastcall UpdateParents(Vcl::Comctrls::TTreeNode* Node, bool NodeChecked);
	virtual void __fastcall UpdateChildren(Vcl::Comctrls::TTreeNode* Node, bool NodeChecked);
	DYNAMIC void __fastcall CascadeComplete(Vcl::Comctrls::TTreeNode* Node);
	virtual void __fastcall LoadTreeFromList(System::Classes::TStrings* List);
	virtual void __fastcall SaveTreeToList(System::Classes::TStrings* List);
	DYNAMIC bool __fastcall CanChangeState(Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);
	DYNAMIC void __fastcall StateChange(Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);
	DYNAMIC void __fastcall Delete(Vcl::Comctrls::TTreeNode* Node);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	HIDESBASE MESSAGE void __fastcall WMChar(Winapi::Messages::TWMKey &Msg);
	Vcl::Imglist::TCustomImageList* __fastcall GetImages();
	HIDESBASE void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	
public:
	__fastcall virtual TRzCheckTree(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCheckTree();
	void __fastcall ToggleCheckState(Vcl::Comctrls::TTreeNode* Node);
	void __fastcall ChangeNodeCheckState(Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);
	void __fastcall ForceCheckState(Vcl::Comctrls::TTreeNode* Node, TRzCheckState NewState);
	void __fastcall SetAllNodes(TRzCheckState NewState);
	void __fastcall UpdateCascadingStates(Vcl::Comctrls::TTreeNode* Node);
	void __fastcall UpdateChildrenCascadingStates(Vcl::Comctrls::TTreeNode* ParentNode);
	void __fastcall UpdateStateFromChildren(Vcl::Comctrls::TTreeNode* ParentNode, Vcl::Comctrls::TTreeNode* DeletedNode);
	__property TRzCheckState ItemState[int Index] = {read=GetItemState, write=SetItemState};
	__property bool SilentCheckChanges = {read=FSilentStateChanges, write=FSilentStateChanges, nodefault};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool CascadeChecks = {read=FCascadeChecks, write=FCascadeChecks, default=1};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777203};
	__property Vcl::Imglist::TCustomImageList* Images = {read=GetImages, write=SetImages};
	__property TRzCheckTreeCascadeCompleteEvent OnCascadeComplete = {read=FOnCascadeComplete, write=FOnCascadeComplete};
	__property TRzCheckTreeChangingEvent OnStateChanging = {read=FOnStateChanging, write=FOnStateChanging};
	__property TRzCheckTreeChangeEvent OnStateChange = {read=FOnStateChange, write=FOnStateChange};
	__property System::Classes::TNotifyEvent OnUpdateChildren = {read=FOnUpdateChildren, write=FOnUpdateChildren};
	__property Align = {default=0};
	__property Anchors = {default=3};
	__property AutoExpand = {default=0};
	__property AutoSelect = {default=0};
	__property BiDiMode;
	__property BorderStyle = {default=1};
	__property BorderWidth = {default=0};
	__property ChangeDelay = {default=0};
	__property Color = {default=-16777211};
	__property Constraints;
	__property Ctl3D;
	__property DisabledColor = {default=-16777201};
	__property DoubleBuffered;
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
	__property HideSelection = {default=1};
	__property HotTrack = {default=0};
	__property Indent;
	__property ItemHeight = {default=0};
	__property ItemHeightMargin = {default=0};
	__property MultiSelect = {default=0};
	__property MultiSelectStyle = {default=1};
	__property PathDelimiter = {default=92};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ReadOnly = {default=1};
	__property RightClickSelect = {default=0};
	__property RowSelect = {default=0};
	__property SelectionPen;
	__property ShowButtons = {default=1};
	__property ShowHint;
	__property ShowLines = {default=1};
	__property ShowRoot = {default=1};
	__property SortType = {default=0};
	__property StateImages;
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property Visible = {default=1};
	__property OnAddition;
	__property OnAdvancedCustomDraw;
	__property OnAdvancedCustomDrawItem;
	__property OnCancelEdit;
	__property OnChange;
	__property OnChanging;
	__property OnClick;
	__property OnCollapsed;
	__property OnCollapsing;
	__property OnCompare;
	__property OnContextPopup;
	__property OnCreateNodeClass;
	__property OnCustomDraw;
	__property OnCustomDrawItem;
	__property OnDblClick;
	__property OnDeletion;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnEdited;
	__property OnEditing;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnExpanded;
	__property OnExpanding;
	__property OnGesture;
	__property OnGetImageIndex;
	__property OnGetSelectedIndex;
	__property OnHint;
	__property OnKeyDown;
	__property OnKeyPress;
	__property OnKeyUp;
	__property OnMouseActivate;
	__property OnMouseDown;
	__property OnMouseEnter;
	__property OnMouseLeave;
	__property OnMouseMove;
	__property OnMouseUp;
	__property OnMouseWheel;
	__property OnMouseWheelDown;
	__property OnMouseWheelUp;
	__property OnNodeContextMenu;
	__property OnStartDock;
	__property OnStartDrag;
	__property Items;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckTree(HWND ParentWindow) : TRzCustomTreeView(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
#define RESOURCE_CHECKS L"RZTREEVW_CHECKS"
static const System::Int8 STATE_UNCHECKED = System::Int8(0x1);
static const System::Int8 STATE_CHECKED = System::Int8(0x2);
static const System::Int8 STATE_PARTCHECKED = System::Int8(0x3);
}	/* namespace Rztreevw */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTREEVW)
using namespace Rztreevw;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztreevwHPP
