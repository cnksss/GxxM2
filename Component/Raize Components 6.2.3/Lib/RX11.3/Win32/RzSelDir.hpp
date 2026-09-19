// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSelDir.pas' rev: 35.00 (Windows)

#ifndef RzseldirHPP
#define RzseldirHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzCommon.hpp>
#include <RzFilSys.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzseldir
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzSelDirDialog;
//-- type declarations -------------------------------------------------------
class PASCALIMPLEMENTATION TRzSelDirDialog _DEPRECATED_ATTRIBUTE0  : public Rzcommon::TRzDialogComponent
{
	typedef Rzcommon::TRzDialogComponent inherited;
	
private:
	bool FAllowCreate _DEPRECATED_ATTRIBUTE0 ;
	bool FButtonGlyphs _DEPRECATED_ATTRIBUTE0 ;
	bool FChangeCurrentDir _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FDirectory _DEPRECATED_ATTRIBUTE0 ;
	Rzfilsys::TNetworkVolumeFormat FNetworkVolumeFormat _DEPRECATED_ATTRIBUTE0 ;
	bool FOpenCurrentDir _DEPRECATED_ATTRIBUTE0 ;
	bool FAutoSelect _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FPrompt _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FPromptFolders _DEPRECATED_ATTRIBUTE0 ;
	System::UnicodeString FPromptDrives _DEPRECATED_ATTRIBUTE0 ;
	Rzfilsys::TDriveTypes FDriveTypes _DEPRECATED_ATTRIBUTE0 ;
	
public:
	__fastcall virtual TRzSelDirDialog _DEPRECATED_ATTRIBUTE0 (System::Classes::TComponent* AOwner);
	DYNAMIC bool __fastcall Execute _DEPRECATED_ATTRIBUTE0 ();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AllowCreate = {read=FAllowCreate, write=FAllowCreate, default=0};
	__property bool ButtonGlyphs = {read=FButtonGlyphs, write=FButtonGlyphs, default=0};
	__property bool ChangeCurrentDir = {read=FChangeCurrentDir, write=FChangeCurrentDir, default=1};
	__property System::UnicodeString Directory = {read=FDirectory, write=FDirectory};
	__property Rzfilsys::TDriveTypes DriveTypes = {read=FDriveTypes, write=FDriveTypes, nodefault};
	__property Rzfilsys::TNetworkVolumeFormat NetworkVolumeFormat = {read=FNetworkVolumeFormat, write=FNetworkVolumeFormat, default=0};
	__property bool OpenCurrentDir = {read=FOpenCurrentDir, write=FOpenCurrentDir, default=0};
	__property bool AutoSelect = {read=FAutoSelect, write=FAutoSelect, default=0};
	__property System::UnicodeString Prompt = {read=FPrompt, write=FPrompt};
	__property System::UnicodeString PromptFolders = {read=FPromptFolders, write=FPromptFolders};
	__property System::UnicodeString PromptDrives = {read=FPromptDrives, write=FPromptDrives};
	__property BorderStyle _DEPRECATED_ATTRIBUTE0  = {default=3};
	__property Caption _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property CaptionOK _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property CaptionCancel _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property CaptionHelp _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Font _DEPRECATED_ATTRIBUTE0 ;
	__property FrameColor _DEPRECATED_ATTRIBUTE0  = {default=-16777200};
	__property FrameStyle _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property FrameVisible _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property FramingPreference _DEPRECATED_ATTRIBUTE0  = {default=1};
	__property Height _DEPRECATED_ATTRIBUTE0  = {default=325};
	__property HelpContext _DEPRECATED_ATTRIBUTE0  = {default=0};
	__property Width _DEPRECATED_ATTRIBUTE0  = {default=275};
public:
	/* TRzDialogComponent.Destroy */ inline __fastcall virtual ~TRzSelDirDialog() { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzseldir */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSELDIR)
using namespace Rzseldir;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzseldirHPP
