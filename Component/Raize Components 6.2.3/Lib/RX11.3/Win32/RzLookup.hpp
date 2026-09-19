// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLookup.pas' rev: 35.00 (Windows)

#ifndef RzlookupHPP
#define RzlookupHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <RzCommon.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlookup
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzLookupDialog;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzLookupDialog : public Rzcommon::TRzDialogComponent
{
	typedef Rzcommon::TRzDialogComponent inherited;
	
private:
	bool FAutoSelect;
	bool FButtonGlyphs;
	System::Uitypes::TEditCharCase FCharCase;
	System::UnicodeString FPrompt;
	System::Classes::TStrings* FList;
	int FSelectedIndex;
	System::UnicodeString FSelectedItem;
	Vcl::Stdctrls::TCustomEdit* FSearchEdit;
	System::UnicodeString FSearchString;
	bool FUpdateSearchEdit;
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall SetList(System::Classes::TStrings* Value);
	
public:
	__fastcall virtual TRzLookupDialog(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzLookupDialog();
	bool __fastcall Execute();
	__property System::UnicodeString SearchString = {read=FSearchString, write=FSearchString};
	__property int SelectedIndex = {read=FSelectedIndex, write=FSelectedIndex, nodefault};
	__property System::UnicodeString SelectedItem = {read=FSelectedItem};
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AutoSelect = {read=FAutoSelect, write=FAutoSelect, default=1};
	__property bool ButtonGlyphs = {read=FButtonGlyphs, write=FButtonGlyphs, default=0};
	__property System::Uitypes::TEditCharCase CharCase = {read=FCharCase, write=FCharCase, default=0};
	__property System::UnicodeString Prompt = {read=FPrompt, write=FPrompt};
	__property System::Classes::TStrings* List = {read=FList, write=SetList};
	__property Vcl::Stdctrls::TCustomEdit* SearchEdit = {read=FSearchEdit, write=FSearchEdit};
	__property bool UpdateSearchEdit = {read=FUpdateSearchEdit, write=FUpdateSearchEdit, default=0};
	__property BorderStyle = {default=2};
	__property Caption = {default=0};
	__property CaptionOK = {default=0};
	__property CaptionCancel = {default=0};
	__property CaptionHelp = {default=0};
	__property Font;
	__property FrameColor = {default=-16777200};
	__property FrameStyle = {default=1};
	__property FrameVisible = {default=0};
	__property FramingPreference = {default=1};
	__property Height = {default=275};
	__property HelpContext = {default=0};
	__property Width = {default=275};
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzlookup */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLOOKUP)
using namespace Rzlookup;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlookupHPP
