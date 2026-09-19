// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzPathBarEditor.pas' rev: 35.00 (Windows)

#ifndef RzpathbareditorHPP
#define RzpathbareditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Forms.hpp>
#include <System.Classes.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzPathBar.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <Vcl.Controls.hpp>
#include <RzLabel.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzpathbareditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzPathBarEditor;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPathBarEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzpathbar::TRzPathBar* __fastcall PathBar();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ItemsMenuHandler(System::TObject* Sender);
	void __fastcall ImagesMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
	virtual void __fastcall Edit();
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzPathBarEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzPathBarEditor() { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzpathbareditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZPATHBAREDITOR)
using namespace Rzpathbareditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzpathbareditorHPP
