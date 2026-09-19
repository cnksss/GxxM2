// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzPanelEditor.pas' rev: 35.00 (Windows)

#ifndef RzpaneleditorHPP
#define RzpaneleditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Controls.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Winapi.Windows.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <RzCmboBx.hpp>
#include <RzPanel.hpp>
#include <RzTrkBar.hpp>
#include <RzRadGrp.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzRadChk.hpp>
#include <RzButton.hpp>
#include <RzLabel.hpp>
#include <RzCommon.hpp>
#include <System.Types.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzpaneleditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzPanelEditor;
class DELPHICLASS TRzPanelEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPanelEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzpanel::TRzPanel* __fastcall Panel();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall VisualStyleMenuHandler(System::TObject* Sender);
	void __fastcall GradientColorStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzPanelEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzPanelEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzPanelEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzGroupBox* GrpColors;
	Rzpanel::TRzGroupBox* GrpWidth;
	Rztrkbar::TRzTrackBar* TrkWidth;
	Rzpanel::TRzGroupBox* GrpPreview;
	Rzlabel::TRzLabel* Label1;
	Rzpanel::TRzGroupBox* GrpBorderStyle;
	Rzbutton::TRzButton* BtnOK;
	Rzbutton::TRzButton* BtnCancel;
	Rzpanel::TRzPanel* PnlPreview;
	Rzlabel::TRzLabel* Label2;
	Rzlabel::TRzLabel* Label3;
	Rzradchk::TRzRadioButton* OptBorderWidth;
	Rzradchk::TRzRadioButton* OptBevelWidth;
	Rzlabel::TRzLabel* LblOuter;
	Rzcmbobx::TRzComboBox* CbxOuter;
	Rzlabel::TRzLabel* LblInner;
	Rzcmbobx::TRzComboBox* CbxInner;
	Rzlabel::TRzLabel* Label4;
	Rzpanel::TRzGroupBox* GrpCaption;
	Rzedit::TRzEdit* EdtCaption;
	Rzradgrp::TRzRadioGroup* GrpAlignment;
	Rzcommon::TRzRegIniFile* RzRegIniFile1;
	Rzedit::TRzColorEdit* EdtPanelColor;
	Rzedit::TRzColorEdit* EdtBorderColor;
	Rzedit::TRzColorEdit* EdtBorderHighlight;
	Rzedit::TRzColorEdit* EdtBorderShadow;
	Rzcommon::TRzCustomColors* RzCustomColors1;
	void __fastcall TrkWidthDrawTick(Rztrkbar::TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	void __fastcall ChkBorderClick(System::TObject* Sender);
	void __fastcall CbxOuterChange(System::TObject* Sender);
	void __fastcall CbxInnerChange(System::TObject* Sender);
	void __fastcall TrkWidthChange(System::TObject* Sender);
	void __fastcall OptBorderWidthClick(System::TObject* Sender);
	void __fastcall GrpAlignmentClick(System::TObject* Sender);
	void __fastcall EdtCaptionChange(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall EdtPanelColorChange(System::TObject* Sender);
	void __fastcall EdtBorderColorChange(System::TObject* Sender);
	void __fastcall EdtBorderHighlightChange(System::TObject* Sender);
	void __fastcall EdtBorderShadowChange(System::TObject* Sender);
	void __fastcall FormClose(System::TObject* Sender, System::Uitypes::TCloseAction &Action);
	
public:
	void __fastcall UpdateControls();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzPanelEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzPanelEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzPanelEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzPanelEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzpaneleditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZPANELEDITOR)
using namespace Rzpaneleditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzpaneleditorHPP
