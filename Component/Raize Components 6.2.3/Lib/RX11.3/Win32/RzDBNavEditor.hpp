// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBNavEditor.pas' rev: 35.00 (Windows)

#ifndef RzdbnaveditorHPP
#define RzdbnaveditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <DesignIntf.hpp>
#include <DesignMenus.hpp>
#include <DesignEditors.hpp>
#include <VCLEditors.hpp>
#include <RzSelectImageEditor.hpp>
#include <Vcl.ImgList.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbnaveditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBNavigatorImageIndexProperty;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBNavigatorImageIndexProperty : public Rzselectimageeditor::TRzCustomImageIndexProperty
{
	typedef Rzselectimageeditor::TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzDBNavigatorImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Rzselectimageeditor::TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzDBNavigatorImageIndexProperty() { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbnaveditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBNAVEDITOR)
using namespace Rzdbnaveditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbnaveditorHPP
