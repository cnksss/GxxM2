// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzAboutEditor.pas' rev: 35.00 (Windows)

#ifndef RzabouteditorHPP
#define RzabouteditorHPP

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
#include <Vcl.Forms.hpp>
#include <Vcl.Controls.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <RzCommon.hpp>
#include <RzLabel.hpp>
#include <RzPanel.hpp>
#include <RzBorder.hpp>
#include <RzStatus.hpp>
#include <RzBckgnd.hpp>
#include <RzBmpBtn.hpp>
#include <RzButton.hpp>
#include <Vcl.Imaging.jpeg.hpp>
#include <Vcl.Imaging.pngimage.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzabouteditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzAboutBoxProperty;
class DELPHICLASS TRzAboutEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzAboutBoxProperty : public Designeditors::TPropertyEditor
{
	typedef Designeditors::TPropertyEditor inherited;
	
public:
	virtual void __fastcall Edit();
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual System::UnicodeString __fastcall GetValue();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzAboutBoxProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TPropertyEditor(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzAboutBoxProperty() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzAboutEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzlabel::TRzLabel* lblTrial1;
	Rzlabel::TRzLabel* lblComponentName;
	Vcl::Stdctrls::TLabel* lblCopyright;
	Rzlabel::TRzLabel* lblDescription;
	Rzlabel::TRzURLLabel* urlRaizeWebsite;
	Rzlabel::TRzLabel* lblVersion;
	Vcl::Extctrls::TImage* imgCompBmp;
	Rzlabel::TRzLabel* lblInfo;
	Rzlabel::TRzURLLabel* urlEmail;
	Rzlabel::TRzLabel* lblNewsgroups;
	Rzlabel::TRzURLLabel* urlNewsgroup;
	Vcl::Extctrls::TImage* imgBackground;
	Rzlabel::TRzLabel* lblEmailSupport;
	Rzlabel::TRzLabel* lblTechSupport;
	Vcl::Extctrls::TImage* imgClose;
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall imgCloseClick(System::TObject* Sender);
	void __fastcall imgBackgroundMouseDown(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	
private:
	System::UnicodeString __fastcall FindDescription(System::UnicodeString CompName);
	
public:
	void __fastcall SetComponentName(System::UnicodeString Value);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzAboutEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzAboutEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzAboutEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzAboutEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
extern DELPHI_PACKAGE System::ResourceString _sCopyrightDate;
#define Rzabouteditor_sCopyrightDate System::LoadResourceString(&Rzabouteditor::_sCopyrightDate)
extern DELPHI_PACKAGE System::ResourceString _sCompanyName;
#define Rzabouteditor_sCompanyName System::LoadResourceString(&Rzabouteditor::_sCompanyName)
extern DELPHI_PACKAGE System::ResourceString _sProductVersion;
#define Rzabouteditor_sProductVersion System::LoadResourceString(&Rzabouteditor::_sProductVersion)
extern DELPHI_PACKAGE System::ResourceString _sProductName;
#define Rzabouteditor_sProductName System::LoadResourceString(&Rzabouteditor::_sProductName)
}	/* namespace Rzabouteditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZABOUTEDITOR)
using namespace Rzabouteditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzabouteditorHPP
