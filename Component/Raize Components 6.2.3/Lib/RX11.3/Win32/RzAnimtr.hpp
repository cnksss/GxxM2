// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzAnimtr.pas' rev: 35.00 (Windows)

#ifndef RzanimtrHPP
#define RzanimtrHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Graphics.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.ImgList.hpp>
#include <System.UITypes.hpp>
#include <System.Classes.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzanimtr
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzAnimator;
//-- type declarations -------------------------------------------------------
typedef void __fastcall (__closure *TRzAnimatorFrameChangeEvent)(System::TObject* Sender, int Frame);

class PASCALIMPLEMENTATION TRzAnimator : public Vcl::Controls::TCustomControl
{
	typedef Vcl::Controls::TCustomControl inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	bool FAnimate;
	bool FContinuous;
	System::Word FDelay;
	System::Word FDelayUntilRepeat;
	System::Uitypes::TImageIndex FLastImageIndex;
	System::Uitypes::TImageIndex FImageIndex;
	Vcl::Imglist::TCustomImageList* FImages;
	Vcl::Imglist::TChangeLink* FImagesChangeLink;
	Vcl::Graphics::TBitmap* FBitmap;
	Vcl::Extctrls::TTimer* FTimer;
	bool FTransparent;
	TRzAnimatorFrameChangeEvent FOnFrameChange;
	System::Classes::TNotifyEvent FOnCycleComplete;
	void __fastcall TimerExpired(System::TObject* Sender);
	void __fastcall ImagesChange(System::TObject* Sender);
	HIDESBASE MESSAGE void __fastcall WMSize(Winapi::Messages::TWMSize &Msg);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DrawImage();
	virtual void __fastcall Paint();
	DYNAMIC void __fastcall FrameChange(int Frame);
	DYNAMIC void __fastcall CycleComplete();
	virtual void __fastcall SetAnimate(bool Value);
	virtual void __fastcall SetContinuous(bool Value);
	virtual void __fastcall SetDelay(System::Word Value);
	virtual void __fastcall SetImageIndex(System::Uitypes::TImageIndex Value);
	virtual void __fastcall SetImageList(Vcl::Imglist::TCustomImageList* Value);
	virtual void __fastcall SetTransparent(bool Value);
	
public:
	__fastcall virtual TRzAnimator(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzAnimator();
	void __fastcall StartAnimation();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool Animate = {read=FAnimate, write=SetAnimate, default=1};
	__property bool Continuous = {read=FContinuous, write=SetContinuous, default=1};
	__property System::Word Delay = {read=FDelay, write=SetDelay, default=100};
	__property System::Word DelayUntilRepeat = {read=FDelayUntilRepeat, write=FDelayUntilRepeat, default=100};
	__property System::Uitypes::TImageIndex ImageIndex = {read=FImageIndex, write=SetImageIndex, default=0};
	__property Vcl::Imglist::TCustomImageList* ImageList = {read=FImages, write=SetImageList};
	__property bool Transparent = {read=FTransparent, write=SetTransparent, default=0};
	__property TRzAnimatorFrameChangeEvent OnFrameChange = {read=FOnFrameChange, write=FOnFrameChange};
	__property System::Classes::TNotifyEvent OnCycleComplete = {read=FOnCycleComplete, write=FOnCycleComplete};
	__property Color = {default=-16777211};
	__property Touch;
	__property OnClick;
	__property OnDblClick;
	__property OnContextPopup;
	__property OnGesture;
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzAnimator(HWND ParentWindow) : Vcl::Controls::TCustomControl(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzanimtr */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZANIMTR)
using namespace Rzanimtr;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzanimtrHPP
