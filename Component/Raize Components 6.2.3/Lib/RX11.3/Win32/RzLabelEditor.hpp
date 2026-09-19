// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzLabelEditor.pas' rev: 35.00 (Windows)

#ifndef RzlabeleditorHPP
#define RzlabeleditorHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.SysUtils.hpp>
#include <Winapi.Windows.hpp>
#include <Winapi.Messages.hpp>
#include <System.Classes.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzLabel.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <RzTrkBar.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <RzDesignEditors.hpp>
#include <RzCmboBx.hpp>
#include <Vcl.Tabs.hpp>
#include <RzPanel.hpp>
#include <RzRadGrp.hpp>
#include <RzTabs.hpp>
#include <RzRadChk.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzButton.hpp>
#include <RzCommon.hpp>
#include <System.Types.hpp>
#include <System.UITypes.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzlabeleditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzLabelEditor;
class DELPHICLASS TRzLabelEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzLabelEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzlabel::TRzLabel* __fastcall LabelControl();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall TextStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzLabelEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzLabelEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzLabelEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzpanel::TRzGroupBox* grpPreview;
	Rzlabel::TRzLabel* lblPreview;
	Rzbutton::TRzButton* btnOK;
	Rzbutton::TRzButton* btnCancel;
	Rzlabel::TRzLabel* Label1;
	Rzedit::TRzMemo* edtCaption;
	Rztabs::TRzPageControl* pgcFormat;
	Rztabs::TRzTabSheet* tabTextStyle;
	Rztabs::TRzTabSheet* tabOptions;
	Rzlabel::TRzLabel* Label2;
	Rzlabel::TRzLabel* Label3;
	Rzlabel::TRzLabel* Label6;
	Rztrkbar::TRzTrackBar* trkPointSize;
	Rzpanel::TRzGroupBox* grpFontStyle;
	Rzradchk::TRzCheckBox* chkBold;
	Rzradchk::TRzCheckBox* chkItalic;
	Rzradchk::TRzCheckBox* chkStrikeout;
	Rzradchk::TRzCheckBox* chkUnderline;
	Rzcmbobx::TRzFontComboBox* cbxFonts;
	Rzradgrp::TRzRadioGroup* grpTextStyle;
	Rzpanel::TRzGroupBox* grpShadow;
	Rzlabel::TRzLabel* Label4;
	Rzlabel::TRzLabel* Label5;
	Rzlabel::TRzLabel* Label7;
	Rzlabel::TRzLabel* LblShadowDepth;
	Rztrkbar::TRzTrackBar* trkShadow;
	Rzpanel::TRzGroupBox* grpRotation;
	Rzlabel::TRzLabel* lblAngle;
	Vcl::Buttons::TSpeedButton* btnNone;
	Vcl::Buttons::TSpeedButton* btnFlat;
	Vcl::Buttons::TSpeedButton* btnCurve;
	Rztrkbar::TRzTrackBar* trkAngle;
	Rzradchk::TRzCheckBox* chk15Degrees;
	Rzradchk::TRzRadioButton* optUpperLeft;
	Rzradchk::TRzRadioButton* optUpperCenter;
	Rzradchk::TRzRadioButton* optUpperRight;
	Rzradchk::TRzRadioButton* optLeftCenter;
	Rzradchk::TRzRadioButton* optCenter;
	Rzradchk::TRzRadioButton* optRightCenter;
	Rzradchk::TRzRadioButton* optLowerLeft;
	Rzradchk::TRzRadioButton* optLowerCenter;
	Rzradchk::TRzRadioButton* optLowerRight;
	Rzcommon::TRzRegIniFile* RzRegIniFile1;
	Rzedit::TRzColorEdit* edtFontColor;
	Rzedit::TRzColorEdit* edtHighlightColor;
	Rzedit::TRzColorEdit* edtShadowColor;
	Rzcommon::TRzCustomColors* RzCustomColors1;
	Rzradchk::TRzCheckBox* chkLightStyle;
	void __fastcall edtCaptionChange(System::TObject* Sender);
	void __fastcall grpTextStyleClick(System::TObject* Sender);
	void __fastcall trkPointSizeDrawTick(Rztrkbar::TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	void __fastcall trkPointSizeChange(System::TObject* Sender);
	void __fastcall trkShadowChange(System::TObject* Sender);
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall cbxFontsChange(System::TObject* Sender);
	void __fastcall chkBoldClick(System::TObject* Sender);
	void __fastcall chkItalicClick(System::TObject* Sender);
	void __fastcall chkStrikeoutClick(System::TObject* Sender);
	void __fastcall chkUnderlineClick(System::TObject* Sender);
	void __fastcall trkAngleDrawTick(Rztrkbar::TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	void __fastcall chk15DegreesClick(System::TObject* Sender);
	void __fastcall trkAngleChange(System::TObject* Sender);
	void __fastcall btnRotationClick(System::TObject* Sender);
	void __fastcall optCenterPointClick(System::TObject* Sender);
	void __fastcall chkLightStyleClick(System::TObject* Sender);
	void __fastcall pgcFormatChanging(System::TObject* Sender, int NewIndex, bool &AllowChange);
	void __fastcall edtFontColorChange(System::TObject* Sender);
	void __fastcall edtHighlightColorChange(System::TObject* Sender);
	void __fastcall edtShadowColorChange(System::TObject* Sender);
	void __fastcall FormClose(System::TObject* Sender, System::Uitypes::TCloseAction &Action);
	
private:
	bool FUpdatingControls;
	bool FNoAngleUpdate;
	
public:
	void __fastcall UpdateControls();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzLabelEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzLabelEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzLabelEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzLabelEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzlabeleditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZLABELEDITOR)
using namespace Rzlabeleditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzlabeleditorHPP
