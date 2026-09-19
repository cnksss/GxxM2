// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzImageListEditor.pas' rev: 35.00 (Windows)

#ifndef RzimagelisteditorHPP
#define RzimagelisteditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Winapi.Windows.hpp>
#include <DesignIntf.hpp>
#include <DesignMenus.hpp>
#include <DesignEditors.hpp>
#include <VCLEditors.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.ImgList.hpp>
#include <Vcl.Graphics.hpp>
#include <RzDesignEditors.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzimagelisteditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzImageListEditor;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzImageListEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Vcl::Imglist::TCustomImageList* __fastcall ImageList();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzImageListEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzImageListEditor() { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzimagelisteditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZIMAGELISTEDITOR)
using namespace Rzimagelisteditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzimagelisteditorHPP
