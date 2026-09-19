// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzChkLst.pas' rev: 35.00 (Windows)

#ifndef RzchklstHPP
#define RzchklstHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Types.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Forms.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Menus.hpp>
#include <RzCommon.hpp>
#include <Vcl.ImgList.hpp>
#include <RzLstBox.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzchklst
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCheckList;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzCheckList : public Rzlstbox::TRzCustomTabbedListBox
{
	typedef Rzlstbox::TRzCustomTabbedListBox inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FChangingItems;
	System::Classes::TStrings* FCheckItems;
	bool FAllowGrayed;
	int FGlyphWidth;
	int FGlyphHeight;
	int FNumGlyphs;
	Vcl::Graphics::TBitmap* FCustomGlyphs;
	Vcl::Imglist::TCustomImageList* FCustomGlyphImages;
	Vcl::Imglist::TChangeLink* FCustomGlyphImagesChangeLink;
	bool FUseCustomGlyphs;
	System::Uitypes::TColor FTransparentColor;
	System::Uitypes::TColor FWinMaskColor;
	System::Uitypes::TColor FHighlightColor;
	System::Uitypes::TColor FItemFillColor;
	System::Uitypes::TColor FItemFocusColor;
	System::Uitypes::TColor FItemFrameColor;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	int FSelectedItem;
	System::Classes::TStringList* FSaveCheckItems;
	int FSaveTopIndex;
	int FSaveItemIndex;
	bool FToggleOnItemClick;
	bool FChangingState;
	Rzcommon::TStateChangingEvent FOnChanging;
	Rzcommon::TStateChangeEvent FOnChange;
	void __fastcall CustomGlyphsChanged(System::TObject* Sender);
	void __fastcall CustomGlyphImagesChange(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMChar(Winapi::Messages::TWMKey &Msg);
	HIDESBASE MESSAGE void __fastcall CMFontChanged(Winapi::Messages::TMessage &Msg);
	HIDESBASE MESSAGE void __fastcall CMEnabledChanged(Winapi::Messages::TMessage &Msg);
	
protected:
	virtual void __fastcall CreateWnd();
	virtual void __fastcall DestroyWnd();
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateItemHeight();
	virtual int __fastcall InitialTabStopOffset();
	virtual void __fastcall ToggleCheckState();
	void __fastcall ExtractGlyph(int Index, Vcl::Graphics::TBitmap* Bitmap, Vcl::Graphics::TBitmap* Source, int W, int H);
	virtual void __fastcall SelectGlyph(int Index, Vcl::Graphics::TBitmap* Glyph);
	virtual int __fastcall OwnerDrawItemIndent();
	virtual System::UnicodeString __fastcall HorzExtentPrefix();
	virtual void __fastcall DrawListItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	void __fastcall UpdateGlyphDimensions();
	void __fastcall InvalidateItemImage(int Index);
	DYNAMIC bool __fastcall CanChange(int Index, Vcl::Stdctrls::TCheckBoxState NewState);
	DYNAMIC void __fastcall Change(int Index, Vcl::Stdctrls::TCheckBoxState NewState);
	DYNAMIC void __fastcall MouseDown(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall MouseUp(System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	DYNAMIC void __fastcall KeyDown(System::Word &Key, System::Classes::TShiftState Shift);
	DYNAMIC void __fastcall KeyUp(System::Word &Key, System::Classes::TShiftState Shift);
	virtual System::Classes::TStrings* __fastcall GetItems();
	virtual bool __fastcall GetItemChecked(int Index);
	virtual void __fastcall SetItemChecked(int Index, bool Value);
	virtual bool __fastcall GetItemEnabled(int Index);
	virtual void __fastcall SetItemEnabled(int Index, bool Value);
	virtual Vcl::Stdctrls::TCheckBoxState __fastcall GetItemState(int Index);
	virtual void __fastcall SetItemState(int Index, Vcl::Stdctrls::TCheckBoxState Value);
	virtual int __fastcall GetItemImageIndex(int Index);
	virtual void __fastcall SetItemImageIndex(int Index, int Value);
	virtual int __fastcall GetItemDisabledIndex(int Index);
	virtual void __fastcall SetItemDisabledIndex(int Index, int Value);
	HIDESBASE virtual void __fastcall SetItems(System::Classes::TStrings* Value);
	virtual void __fastcall SetAllowGrayed(bool Value);
	virtual void __fastcall SetCustomGlyphs(Vcl::Graphics::TBitmap* Value);
	virtual void __fastcall SetCustomGlyphImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetImages(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetUseCustomGlyphs(bool Value);
	virtual void __fastcall SetHighlightColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemFillColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetItemFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetTransparentColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetWinMaskColor(System::Uitypes::TColor Value);
	__property int GlyphWidth = {read=FGlyphWidth, nodefault};
	__property int GlyphHeight = {read=FGlyphHeight, nodefault};
	
public:
	__fastcall virtual TRzCheckList(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCheckList();
	int __fastcall AddEx(const System::UnicodeString S, bool Checked, bool Enabled = true, int ImageIndex = 0xffffffff, int DisabledIndex = 0xffffffff);
	virtual void __fastcall AddItem(System::UnicodeString Item, System::TObject* AObject);
	void __fastcall CheckAll();
	void __fastcall UncheckAll();
	int __fastcall ItemsChecked();
	void __fastcall LoadFromFile(const System::UnicodeString FileName)/* overload */;
	void __fastcall LoadFromFile(const System::UnicodeString FileName, System::Sysutils::TEncoding* Encoding)/* overload */;
	void __fastcall LoadFromStream(System::Classes::TStream* Stream)/* overload */;
	void __fastcall LoadFromStream(System::Classes::TStream* Stream, System::Sysutils::TEncoding* Encoding)/* overload */;
	void __fastcall SaveToFile(const System::UnicodeString FileName)/* overload */;
	void __fastcall SaveToFile(const System::UnicodeString FileName, System::Sysutils::TEncoding* Encoding)/* overload */;
	void __fastcall SaveToStream(System::Classes::TStream* Stream)/* overload */;
	void __fastcall SaveToStream(System::Classes::TStream* Stream, System::Sysutils::TEncoding* Encoding)/* overload */;
	virtual void __fastcall DefaultDrawItem(int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	void __fastcall SetGroupState(int GroupIndex, Vcl::Stdctrls::TCheckBoxState State);
	void __fastcall CheckGroup(int GroupIndex);
	void __fastcall UncheckGroup(int GroupIndex);
	void __fastcall EnableGroup(int GroupIndex);
	void __fastcall DisableGroup(int GroupIndex);
	__property bool ItemChecked[int Index] = {read=GetItemChecked, write=SetItemChecked};
	__property bool ItemEnabled[int Index] = {read=GetItemEnabled, write=SetItemEnabled};
	__property Vcl::Stdctrls::TCheckBoxState ItemState[int Index] = {read=GetItemState, write=SetItemState};
	__property int ItemImageIndex[int Index] = {read=GetItemImageIndex, write=SetItemImageIndex};
	__property int ItemDisabledIndex[int Index] = {read=GetItemDisabledIndex, write=SetItemDisabledIndex};
	
__published:
	__property System::Classes::TStrings* Items = {read=FCheckItems, write=SetItems};
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowGrayed = {read=FAllowGrayed, write=SetAllowGrayed, default=0};
	__property Vcl::Graphics::TBitmap* CustomGlyphs = {read=FCustomGlyphs, write=SetCustomGlyphs};
	__property Vcl::Imglist::TCustomImageList* CustomGlyphImages = {read=FCustomGlyphImages, write=SetCustomGlyphImages};
	__property System::Uitypes::TColor HighlightColor = {read=FHighlightColor, write=SetHighlightColor, default=-16777203};
	__property System::Uitypes::TColor ItemFillColor = {read=FItemFillColor, write=SetItemFillColor, default=-16777211};
	__property System::Uitypes::TColor ItemFocusColor = {read=FItemFocusColor, write=SetItemFocusColor, default=-16777211};
	__property System::Uitypes::TColor ItemFrameColor = {read=FItemFrameColor, write=SetItemFrameColor, default=-16777200};
	__property Vcl::Imglist::TCustomImageList* Images = {read=FImages, write=SetImages};
	__property bool UseCustomGlyphs = {read=FUseCustomGlyphs, write=SetUseCustomGlyphs, default=0};
	__property System::Uitypes::TColor TransparentColor = {read=FTransparentColor, write=SetTransparentColor, default=32896};
	__property System::Uitypes::TColor WinMaskColor = {read=FWinMaskColor, write=SetWinMaskColor, default=65280};
	__property Rzcommon::TStateChangeEvent OnChange = {read=FOnChange, write=FOnChange};
	__property Rzcommon::TStateChangingEvent OnChanging = {read=FOnChanging, write=FOnChanging};
	__property bool ToggleOnItemClick = {read=FToggleOnItemClick, write=FToggleOnItemClick, default=0};
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
	__property DoubleBuffered;
	__property DragCursor = {default=-12};
	__property DragKind = {default=0};
	__property DragMode = {default=0};
	__property Enabled = {default=1};
	__property ShowItemHints = {default=0};
	__property ExtendedSelect = {default=1};
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
	__property GroupColorFromTheme = {default=1};
	__property GroupFont;
	__property HorzScrollBar = {default=0};
	__property ImeMode = {default=3};
	__property ImeName = {default=0};
	__property IncrementalSearch = {default=1};
	__property IntegralHeight = {default=0};
	__property ItemHeight = {default=16};
	__property MultiSelect = {default=0};
	__property ParentBiDiMode = {default=1};
	__property ParentColor = {default=0};
	__property ParentCtl3D = {default=1};
	__property ParentDoubleBuffered = {default=1};
	__property ParentFont = {default=1};
	__property ParentShowHint = {default=1};
	__property PopupMenu;
	__property ShowHint;
	__property Sorted = {default=0};
	__property TabOnEnter = {default=0};
	__property TabOrder = {default=-1};
	__property TabStop = {default=1};
	__property Touch;
	__property UseGradients = {default=1};
	__property Visible = {default=1};
	__property OnClick;
	__property OnContextPopup;
	__property OnData;
	__property OnDataFind;
	__property OnDataObject;
	__property OnDblClick;
	__property OnDeleteItems;
	__property OnDragDrop;
	__property OnDragOver;
	__property OnDrawItem;
	__property OnEndDock;
	__property OnEndDrag;
	__property OnEnter;
	__property OnExit;
	__property OnGesture;
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
	/* TWinControl.CreateParented */ inline __fastcall TRzCheckList(HWND ParentWindow) : Rzlstbox::TRzCustomTabbedListBox(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzchklst */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCHKLST)
using namespace Rzchklst;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzchklstHPP
