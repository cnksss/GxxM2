// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBStat.pas' rev: 35.00 (Windows)

#ifndef RzdbstatHPP
#define RzdbstatHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Messages.hpp>
#include <Winapi.Windows.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <RzStatus.hpp>
#include <Data.DB.hpp>
#include <Vcl.DBCtrls.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbstat
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBStatusPane;
class DELPHICLASS TRzDBStateDataLink;
class DELPHICLASS TRzStateCaptions;
class DELPHICLASS TRzDBStateStatus;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzDBStatusPane : public Rzstatus::TRzCustomFieldStatus
{
	typedef Rzstatus::TRzCustomFieldStatus inherited;
	
private:
	void __fastcall DataChange(System::TObject* Sender);
	
protected:
	Vcl::Dbctrls::TFieldDataLink* FDataLink;
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual System::UnicodeString __fastcall GetDataField();
	virtual void __fastcall SetDataField(const System::UnicodeString Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	
public:
	__fastcall virtual TRzDBStatusPane(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBStatusPane();
	
__published:
	__property System::UnicodeString DataField = {read=GetDataField, write=SetDataField};
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property Alignment = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property CaptionOffset = {default=2};
	__property ShowHint = {default=1};
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBStateDataLink : public Data::Db::TDataLink
{
	typedef Data::Db::TDataLink inherited;
	
private:
	TRzDBStateStatus* FStatusControl;
	
protected:
	virtual void __fastcall EditingChanged();
	virtual void __fastcall ActiveChanged();
	
public:
	__fastcall TRzDBStateDataLink(TRzDBStateStatus* AStatusControl);
	__fastcall virtual ~TRzDBStateDataLink();
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzStateCaptions : public System::Classes::TPersistent
{
	typedef System::Classes::TPersistent inherited;
	
	
private:
	typedef System::StaticArray<System::UnicodeString, 7> _TRzStateCaptions__1;
	
	
private:
	_TRzStateCaptions__1 FCaptions;
	TRzDBStateStatus* FStatusControl;
	
protected:
	virtual System::UnicodeString __fastcall GetStateCaption(int Index);
	virtual void __fastcall SetStateCaption(int Index, const System::UnicodeString Value);
	
public:
	__fastcall TRzStateCaptions(TRzDBStateStatus* AStatusControl);
	__fastcall virtual ~TRzStateCaptions();
	__property TRzDBStateStatus* StausControl = {read=FStatusControl};
	__property System::UnicodeString Captions[int Index] = {read=GetStateCaption, write=SetStateCaption};
	
__published:
	__property System::UnicodeString Inactive = {read=GetStateCaption, write=SetStateCaption, index=0};
	__property System::UnicodeString Browse = {read=GetStateCaption, write=SetStateCaption, index=1};
	__property System::UnicodeString Edit = {read=GetStateCaption, write=SetStateCaption, index=2};
	__property System::UnicodeString Insert = {read=GetStateCaption, write=SetStateCaption, index=3};
	__property System::UnicodeString Search = {read=GetStateCaption, write=SetStateCaption, index=4};
	__property System::UnicodeString Calculate = {read=GetStateCaption, write=SetStateCaption, index=5};
	__property System::UnicodeString Filter = {read=GetStateCaption, write=SetStateCaption, index=6};
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzDBStateStatus : public Rzstatus::TRzCustomGlyphStatus
{
	typedef Rzstatus::TRzCustomGlyphStatus inherited;
	
private:
	Vcl::Graphics::TBitmap* FBitmaps;
	TRzStateCaptions* FStateCaptions;
	bool FStoreStateCaptions;
	bool FShowCaption;
	
protected:
	int FGlyphHeight;
	int FGlyphWidth;
	TRzDBStateDataLink* FDataLink;
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall UpdateState();
	virtual void __fastcall ExtractGlyph(int Index, Vcl::Graphics::TBitmap* Bitmap, Vcl::Graphics::TBitmap* Source, int W, int H);
	virtual void __fastcall SelectGlyph(Vcl::Graphics::TBitmap* Glyph);
	virtual void __fastcall SetStateCaptions(TRzStateCaptions* Value);
	virtual Data::Db::TDataSource* __fastcall GetDataSource();
	virtual void __fastcall SetDataSource(Data::Db::TDataSource* Value);
	virtual Data::Db::TDataSetState __fastcall GetDatasetState();
	virtual void __fastcall SetShowCaption(bool Value);
	virtual void __fastcall SetShowGlyph(bool Value);
	
public:
	__fastcall virtual TRzDBStateStatus(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDBStateStatus();
	__property Data::Db::TDataSetState DatasetState = {read=GetDatasetState, nodefault};
	
__published:
	__property Data::Db::TDataSource* DataSource = {read=GetDataSource, write=SetDataSource};
	__property bool ShowCaption = {read=FShowCaption, write=SetShowCaption, default=1};
	__property TRzStateCaptions* StateCaptions = {read=FStateCaptions, write=SetStateCaptions, stored=FStoreStateCaptions};
	__property Alignment = {default=0};
	__property AutoSize = {default=0};
	__property Blinking = {default=0};
	__property BlinkColor = {default=-16777203};
	__property BlinkIntervalOff = {default=500};
	__property BlinkIntervalOn = {default=500};
	__property CaptionOffset = {default=2};
	__property GlyphAlignment = {default=0};
	__property GlyphOffset = {default=5};
	__property ShowGlyph = {default=1};
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbstat */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBSTAT)
using namespace Rzdbstat;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbstatHPP
