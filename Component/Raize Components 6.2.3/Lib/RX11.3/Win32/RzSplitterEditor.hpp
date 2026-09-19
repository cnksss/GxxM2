// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSplitterEditor.pas' rev: 35.00 (Windows)

#ifndef RzsplittereditorHPP
#define RzsplittereditorHPP

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
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.Menus.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <TreeIntf.hpp>
#include <VCLSprigs.hpp>
#include <RzTrkBar.hpp>
#include <RzDesignEditors.hpp>
#include <RzButton.hpp>
#include <RzRadChk.hpp>
#include <RzPanel.hpp>
#include <RzSplit.hpp>
#include <RzCmboBx.hpp>
#include <RzLabel.hpp>
#include <RzRadGrp.hpp>
#include <RzTabs.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzsplittereditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzSplitterEditor;
class DELPHICLASS TRzSplitterPaneSprig;
class DELPHICLASS TRzSplitterSprig;
class DELPHICLASS TRzSplitterEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzSplitterEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzsplit::TRzSplitter* __fastcall Splitter();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzSplitterEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzSplitterEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzSplitterPaneSprig : public Vclsprigs::TWinControlSprig
{
	typedef Vclsprigs::TWinControlSprig inherited;
	
private:
	bool FUpperLeft;
	
public:
	__fastcall virtual TRzSplitterPaneSprig(System::Classes::TPersistent* AItem)/* overload */;
	virtual System::UnicodeString __fastcall Caption();
	virtual bool __fastcall Hidden();
	virtual bool __fastcall DragOver(Treeintf::TSprig* AItem);
	virtual bool __fastcall DragDrop(Treeintf::TSprig* AItem);
	virtual bool __fastcall PaletteOver(Treeintf::TSprigClass ASprigClass, System::TClass AClass);
public:
	/* TComponentSprig.Create */ inline __fastcall TRzSplitterPaneSprig(System::Classes::TPersistent* AItem, Treeintf::TSprig* AOwner)/* overload */ : Vclsprigs::TWinControlSprig(AItem, AOwner) { }
	
public:
	/* TSprig.Destroy */ inline __fastcall virtual ~TRzSplitterPaneSprig() { }
	
};

#pragma pack(pop)

typedef System::TMetaClass* TRzSplitterPaneSprigClass;

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzSplitterSprig : public Vclsprigs::TWinControlSprig
{
	typedef Vclsprigs::TWinControlSprig inherited;
	
public:
	__fastcall virtual TRzSplitterSprig(System::Classes::TPersistent* AItem)/* overload */;
	virtual void __fastcall FigureChildren();
public:
	/* TComponentSprig.Create */ inline __fastcall TRzSplitterSprig(System::Classes::TPersistent* AItem, Treeintf::TSprig* AOwner)/* overload */ : Vclsprigs::TWinControlSprig(AItem, AOwner) { }
	
public:
	/* TSprig.Destroy */ inline __fastcall virtual ~TRzSplitterSprig() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzSplitterEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzbutton::TRzButton* BtnOK;
	Rzbutton::TRzButton* BtnCancel;
	Rzpanel::TRzGroupBox* GrpPreview;
	Rzsplit::TRzSplitter* SplPreview;
	Rzradgrp::TRzRadioGroup* GrpOrientation;
	Rzradchk::TRzCheckBox* ChkRealTime;
	Vcl::Extctrls::TPaintBox* PbxUpperLeft;
	Rzradchk::TRzCheckBox* ChkUsePercent;
	Rzradgrp::TRzRadioGroup* GrpFixedPane;
	Rzradchk::TRzCheckBox* ChkShowHotSpot;
	Rztabs::TRzTabControl* TbcRegions;
	Rzradchk::TRzCheckBox* ChkVisible;
	Rzpanel::TRzGroupBox* GrpBorder;
	Rzlabel::TRzLabel* LblOuter;
	Rzlabel::TRzLabel* LblInner;
	Rzcmbobx::TRzComboBox* CbxOuter;
	Rzcmbobx::TRzComboBox* CbxInner;
	Rzcmbobx::TRzComboBox* CbxBarStyle;
	Rzpanel::TRzGroupBox* GrpWidth;
	Rztrkbar::TRzTrackBar* TrkWidth;
	Vcl::Extctrls::TPaintBox* PbxLowerRight;
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall FrameStyleChange(System::TObject* Sender);
	void __fastcall GrpOrientationClick(System::TObject* Sender);
	void __fastcall TrkWidthChange(System::TObject* Sender);
	void __fastcall ChkRealTimeClick(System::TObject* Sender);
	void __fastcall PbxUpperLeftPaint(System::TObject* Sender);
	void __fastcall PbxLowerRightPaint(System::TObject* Sender);
	void __fastcall PbxUpperLeftClick(System::TObject* Sender);
	void __fastcall PbxLowerRightClick(System::TObject* Sender);
	void __fastcall ChkVisibleClick(System::TObject* Sender);
	void __fastcall TrkWidthDrawTick(Rztrkbar::TRzTrackBar* TrackBar, Vcl::Graphics::TCanvas* Canvas, const System::Types::TPoint &Location, int Index);
	void __fastcall CbxBarStyleChange(System::TObject* Sender);
	void __fastcall ChkUsePercentClick(System::TObject* Sender);
	void __fastcall GrpFixedPaneClick(System::TObject* Sender);
	void __fastcall ChkShowHotSpotClick(System::TObject* Sender);
	void __fastcall TbcRegionsChanging(System::TObject* Sender, int NewIndex, bool &AllowChange);
	
private:
	int FRegion;
	void __fastcall SetRegionSettings();
	
public:
	void __fastcall UpdateControls();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzSplitterEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzSplitterEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzSplitterEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSplitterEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzsplittereditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSPLITTEREDITOR)
using namespace Rzsplittereditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzsplittereditorHPP
