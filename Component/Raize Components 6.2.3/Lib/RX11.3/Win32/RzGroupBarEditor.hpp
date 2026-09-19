// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzGroupBarEditor.pas' rev: 35.00 (Windows)

#ifndef RzgroupbareditorHPP
#define RzgroupbareditorHPP

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
#include <RzGroupBar.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <Vcl.Controls.hpp>
#include <RzLabel.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <RzPanel.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzgroupbareditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzGroupBarEditor;
class DELPHICLASS TRzGroupEditor;
class DELPHICLASS TRzGroupControllerEditor;
class DELPHICLASS TRzGroupTemplateEditor;
class DELPHICLASS TRzGroupTemplatePreviewDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupBarEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzgroupbar::TRzGroupBar* __fastcall GroupBar();
	virtual int __fastcall AlignMenuIndex();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall GroupBarStyleMenuHandler(System::TObject* Sender);
	void __fastcall VisualStyleMenuHandler(System::TObject* Sender);
	void __fastcall GradientColorStyleMenuHandler(System::TObject* Sender);
	void __fastcall SmallImagesMenuHandler(System::TObject* Sender);
	void __fastcall LargeImagesMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
	virtual void __fastcall Edit();
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzGroupBarEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzGroupBarEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzgroupbar::TRzGroup* __fastcall Group();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ActionListMenuHandler(System::TObject* Sender);
	void __fastcall ItemsMenuHandler(System::TObject* Sender);
	void __fastcall ItemStyleMenuHandler(System::TObject* Sender);
	void __fastcall GroupBarStyleMenuHandler(System::TObject* Sender);
	void __fastcall VisualStyleMenuHandler(System::TObject* Sender);
	void __fastcall GradientColorStyleMenuHandler(System::TObject* Sender);
	void __fastcall SmallImagesMenuHandler(System::TObject* Sender);
	void __fastcall LargeImagesMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
	virtual void __fastcall Edit();
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzGroupEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzGroupEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupControllerEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzgroupbar::TRzGroupController* __fastcall GroupController();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
	virtual void __fastcall Edit();
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzGroupControllerEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzGroupControllerEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupTemplateEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzgroupbar::TRzGroupTemplate* __fastcall Template();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ActionListMenuHandler(System::TObject* Sender);
	void __fastcall SmallImagesMenuHandler(System::TObject* Sender);
	void __fastcall LargeImagesMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
	virtual void __fastcall Edit();
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzGroupTemplateEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzGroupTemplateEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzGroupTemplatePreviewDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzgroupbar::TRzGroupBar* GroupBar;
	void __fastcall FormKeyPress(System::TObject* Sender, System::WideChar &Key);
	void __fastcall FormCreate(System::TObject* Sender);
	
private:
	void __fastcall ItemClickHandler(System::TObject* Sender);
	
public:
	void __fastcall SetTemplate(Rzgroupbar::TRzGroupTemplate* Template);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzGroupTemplatePreviewDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzGroupTemplatePreviewDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzGroupTemplatePreviewDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzGroupTemplatePreviewDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzgroupbareditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZGROUPBAREDITOR)
using namespace Rzgroupbareditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzgroupbareditorHPP
