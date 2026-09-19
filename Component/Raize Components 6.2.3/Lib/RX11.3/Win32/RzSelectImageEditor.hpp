// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSelectImageEditor.pas' rev: 35.00 (Windows)

#ifndef RzselectimageeditorHPP
#define RzselectimageeditorHPP

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
#include <DesignIntf.hpp>
#include <DesignMenus.hpp>
#include <DesignEditors.hpp>
#include <VCLEditors.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzPanel.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.ImgList.hpp>
#include <Vcl.Menus.hpp>
#include <RzButton.hpp>
#include <RzDesignEditors.hpp>
#include <RzRadChk.hpp>
#include <RzLstBox.hpp>
#include <RzLabel.hpp>
#include <RzStatus.hpp>
#include <RzSpnEdt.hpp>
#include <Vcl.ExtDlgs.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzselectimageeditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzCustomImageIndexProperty;
class DELPHICLASS TRzImageIndexProperty;
class DELPHICLASS TRzToolButtonImageIndexProperty;
class DELPHICLASS TRzTabSheetImageIndexProperty;
class DELPHICLASS TRzGroupCaptionImageIndexProperty;
class DELPHICLASS TRzGroupItemImageIndexProperty;
class DELPHICLASS TRzPathItemImageIndexProperty;
class DELPHICLASS TRzGroupTemplateCaptionImageIndexProperty;
class DELPHICLASS TRzGroupTemplateItemImageIndexProperty;
class DELPHICLASS TRzAnimatorImageIndexProperty;
class DELPHICLASS TRzTrayIconIndexProperty;
class DELPHICLASS TRzToolButtonEditor;
class DELPHICLASS TRzBitBtnEditor;
class DELPHICLASS TRzMenuButtonEditor;
class DELPHICLASS TRzRapidFireButtonEditor;
class DELPHICLASS TRzSelectImageEditor;
class DELPHICLASS TRzSelectImageEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzCustomImageIndexProperty : public Designeditors::TIntegerProperty
{
	typedef Designeditors::TIntegerProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList() = 0 ;
	virtual int __fastcall GetImageHeight(Vcl::Imglist::TCustomImageList* Images, Vcl::Graphics::TCanvas* ACanvas);
	virtual int __fastcall GetImageWidth(Vcl::Imglist::TCustomImageList* Images, Vcl::Graphics::TCanvas* ACanvas);
	
public:
	virtual Designintf::TPropertyAttributes __fastcall GetAttributes();
	virtual void __fastcall GetValues(System::Classes::TGetStrProc Proc);
	virtual void __fastcall Edit();
	void __fastcall ListMeasureHeight(const System::UnicodeString Value, Vcl::Graphics::TCanvas* ACanvas, int &AHeight);
	void __fastcall ListMeasureWidth(const System::UnicodeString Value, Vcl::Graphics::TCanvas* ACanvas, int &AWidth);
	void __fastcall ListDrawValue(const System::UnicodeString Value, Vcl::Graphics::TCanvas* ACanvas, const System::Types::TRect &ARect, bool ASelected);
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzCustomImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Designeditors::TIntegerProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzCustomImageIndexProperty() { }
	
private:
	void *__ICustomPropertyListDrawing;	// Vcleditors::ICustomPropertyListDrawing 
	
public:
	#if defined(MANAGED_INTERFACE_OPERATORS)
	// {BE2B8CF7-DDCA-4D4B-BE26-2396B969F8E0}
	operator Vcleditors::_di_ICustomPropertyListDrawing()
	{
		Vcleditors::_di_ICustomPropertyListDrawing intf;
		this->GetInterface(intf);
		return intf;
	}
	#else
	operator Vcleditors::ICustomPropertyListDrawing*(void) { return (Vcleditors::ICustomPropertyListDrawing*)&__ICustomPropertyListDrawing; }
	#endif
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzToolButtonImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzToolButtonImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzToolButtonImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTabSheetImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzTabSheetImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzTabSheetImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupCaptionImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzGroupCaptionImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzGroupCaptionImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupItemImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzGroupItemImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzGroupItemImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPathItemImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzPathItemImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzPathItemImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupTemplateCaptionImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzGroupTemplateCaptionImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzGroupTemplateCaptionImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzGroupTemplateItemImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzGroupTemplateItemImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzGroupTemplateItemImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzAnimatorImageIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzAnimatorImageIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzAnimatorImageIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzTrayIconIndexProperty : public TRzCustomImageIndexProperty
{
	typedef TRzCustomImageIndexProperty inherited;
	
protected:
	virtual Vcl::Imglist::TCustomImageList* __fastcall GetImageList();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzTrayIconIndexProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : TRzCustomImageIndexProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzTrayIconIndexProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzToolButtonEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzbutton::TRzToolButton* __fastcall ToolButton();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall LayoutMenuHandler(System::TObject* Sender);
	void __fastcall DropDownMenuMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzToolButtonEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzToolButtonEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzBitBtnEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzbutton::TRzBitBtn* __fastcall BitBtn();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzBitBtnEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzBitBtnEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzMenuButtonEditor : public TRzBitBtnEditor
{
	typedef TRzBitBtnEditor inherited;
	
protected:
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	void __fastcall DropDownMenuMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzMenuButtonEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzBitBtnEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzMenuButtonEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzRapidFireButtonEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	Rzspnedt::TRzRapidFireButton* __fastcall Button();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ScrollStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzRapidFireButtonEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzRapidFireButtonEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzSelectImageEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzSelectImageEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzSelectImageEditor() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzTextChangeType : unsigned char { tctNone, tctCaption, tctHint };

class PASCALIMPLEMENTATION TRzSelectImageEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Rzbutton::TRzButton* BtnDone;
	Rzpanel::TRzGroupBox* GrpCustomImages;
	Rzpanel::TRzGroupBox* GrpStockImages;
	Rzbutton::TRzToolbarButton* BtnNew;
	Rzbutton::TRzToolbarButton* BtnOpen;
	Rzbutton::TRzToolbarButton* BtnSave;
	Rzbutton::TRzToolbarButton* BtnFolderClosed;
	Rzbutton::TRzToolbarButton* BtnFolderSelect;
	Rzbutton::TRzToolbarButton* BtnCut;
	Rzbutton::TRzToolbarButton* BtnCopy;
	Rzbutton::TRzToolbarButton* BtnPaste;
	Rzbutton::TRzToolbarButton* BtnUndo;
	Rzbutton::TRzToolbarButton* BtnRedo;
	Rzbutton::TRzToolbarButton* BtnSearchFind;
	Rzbutton::TRzToolbarButton* BtnSearchReplace;
	Rzbutton::TRzToolbarButton* BtnSearchFindNext;
	Rzbutton::TRzToolbarButton* BtnFormatBold;
	Rzbutton::TRzToolbarButton* BtnFormatItalics;
	Rzbutton::TRzToolbarButton* BtnFormatUnderline;
	Rzbutton::TRzToolbarButton* BtnFormatFont;
	Rzbutton::TRzToolbarButton* BtnFormatLeft;
	Rzbutton::TRzToolbarButton* BtnFormatCenter;
	Rzbutton::TRzToolbarButton* BtnFormatRight;
	Rzbutton::TRzToolbarButton* BtnFormatJustify;
	Rzbutton::TRzToolbarButton* BtnFormatBullets;
	Rzbutton::TRzToolbarButton* BtnFolderUp;
	Rzbutton::TRzToolbarButton* BtnFolderNew;
	Rzbutton::TRzToolbarButton* BtnHelp;
	Rzbutton::TRzToolbarButton* BtnFolderOpen;
	Rzbutton::TRzToolbarButton* BtnSearchJumpToLine;
	Rzbutton::TRzToolbarButton* BtnDBPrevious;
	Rzbutton::TRzToolbarButton* BtnDBNext;
	Rzbutton::TRzToolbarButton* BtnDBLast;
	Rzbutton::TRzToolbarButton* BtnDBEdit;
	Rzbutton::TRzToolbarButton* BtnDBPost;
	Rzbutton::TRzToolbarButton* BtnDBCancel;
	Rzbutton::TRzToolbarButton* BtnDBRefresh;
	Rzbutton::TRzToolbarButton* BtnOK;
	Rzbutton::TRzToolbarButton* BtnCancel;
	Rzbutton::TRzToolbarButton* BtnSignalError;
	Rzbutton::TRzToolbarButton* BtnHelpBook;
	Rzbutton::TRzToolbarButton* BtnHelpContext;
	Rzbutton::TRzToolbarButton* BtnEmail;
	Rzbutton::TRzToolbarButton* BtnAttach;
	Rzbutton::TRzToolbarButton* BtnWindowCascade;
	Rzbutton::TRzToolbarButton* BtnWindowHorzTile;
	Rzbutton::TRzToolbarButton* BtnWindowVertTile;
	Rzbutton::TRzToolbarButton* BtnWindowTile;
	Rzbutton::TRzToolbarButton* BtnViewZoom;
	Rzbutton::TRzToolbarButton* BtnViewZoomOut;
	Rzbutton::TRzToolbarButton* BtnViewZoomIn;
	Rzbutton::TRzToolbarButton* BtnToolsCursor;
	Rzbutton::TRzToolbarButton* BtnRecycle;
	Rzbutton::TRzToolbarButton* BtnRecycleXP;
	Rzbutton::TRzToolbarButton* BtnClear;
	Rzbutton::TRzToolbarButton* BtnToolsImage;
	Rzbutton::TRzToolbarButton* BtnPrint;
	Rzbutton::TRzToolbarButton* BtnPrintPreview;
	Rzbutton::TRzToolbarButton* BtnCalendarDate;
	Rzbutton::TRzToolbarButton* BtnProperties;
	Rzbutton::TRzToolbarButton* BtnExecute;
	Rzbutton::TRzToolbarButton* BtnToolsRuler;
	Rzbutton::TRzToolbarButton* BtnToolsHammer;
	Rzbutton::TRzToolbarButton* BtnExit;
	Rzbutton::TRzToolbarButton* BtnSignalFinish;
	Vcl::Menus::TPopupMenu* MnuStock;
	Vcl::Menus::TMenuItem* MnuEditStockHint;
	Rzbutton::TRzToolbarButton* BtnArrowLeft;
	Rzbutton::TRzToolbarButton* BtnArrowUp;
	Rzbutton::TRzToolbarButton* BtnArrowRight;
	Rzbutton::TRzToolbarButton* BtnArrowDown;
	Rzbutton::TRzToolbarButton* BtnMoveLeft;
	Rzbutton::TRzToolbarButton* BtnMoveAllLeft;
	Rzbutton::TRzToolbarButton* BtnMoveRight;
	Rzbutton::TRzToolbarButton* BtnMoveAllRight;
	Rzbutton::TRzToolbarButton* BtnCalendarMonth;
	Rzbutton::TRzToolbarButton* BtnMove;
	Rzbutton::TRzToolbarButton* BtnCopyAll;
	Rzbutton::TRzToolbarButton* BtnSaveAll;
	Rzbutton::TRzToolbarButton* BtnNotebook;
	Rzbutton::TRzToolbarButton* BtnNote;
	Rzbutton::TRzToolbarButton* BtnNotePage;
	Rzbutton::TRzToolbarButton* BtnImport;
	Rzbutton::TRzToolbarButton* BtnExport;
	Rzbutton::TRzToolbarButton* BtnToolsPencil;
	Rzbutton::TRzToolbarButton* BtnToolsPen;
	Rzbutton::TRzToolbarButton* BtnToolsKey;
	Rzbutton::TRzToolbarButton* BtnClearImage;
	Vcl::Extctrls::TImage* ImgGroove;
	Vcl::Extctrls::TImage* ImgSpacer;
	Rzbutton::TRzToolbarButton* BtnDBFirst;
	Rzradchk::TRzCheckBox* ChkSetHint;
	Rzbutton::TRzToolbarButton* BtnPreviewNext;
	Rzbutton::TRzToolbarButton* BtnPreviewPrev;
	Rzbutton::TRzToolbarButton* BtnAlign;
	Rzbutton::TRzToolbarButton* BtnAlignLeft;
	Rzbutton::TRzToolbarButton* BtnAlignTop;
	Rzbutton::TRzToolbarButton* BtnAlignRight;
	Rzbutton::TRzToolbarButton* BtnAlignBottom;
	Rzbutton::TRzToolbarButton* BtnAlignClient;
	Rzbutton::TRzToolbarButton* BtnFileDelete;
	Rzbutton::TRzToolbarButton* BtnAlignNone;
	Rzbutton::TRzToolbarButton* BtnNetWeb;
	Rzbutton::TRzToolbarButton* BtnNetNews;
	Rzbutton::TRzToolbarButton* BtnOrderBackOne;
	Rzbutton::TRzToolbarButton* BtnOrderFrontOne;
	Rzbutton::TRzToolbarButton* BtnOrderToBack;
	Rzbutton::TRzToolbarButton* BtnOrderToFront;
	Rzbutton::TRzToolbarButton* BtnMediaSkipBackward;
	Rzbutton::TRzToolbarButton* BtnMediaRewind;
	Rzbutton::TRzToolbarButton* BtnMediaStop;
	Rzbutton::TRzToolbarButton* BtnMediaPause;
	Rzbutton::TRzToolbarButton* BtnMediaPlay;
	Rzbutton::TRzToolbarButton* BtnMediaFastForward;
	Rzbutton::TRzToolbarButton* BtnMediaSkipForward;
	Rzbutton::TRzToolbarButton* BtnMediaRecord;
	Rzbutton::TRzToolbarButton* BtnSelectAll;
	Rzbutton::TRzToolbarButton* BtnToolsPlug;
	Rzbutton::TRzToolbarButton* BtnOptions;
	Rzbutton::TRzToolbarButton* BtnToolsSpeaker;
	Rzbutton::TRzToolbarButton* BtnToolsPin;
	Rzbutton::TRzToolbarButton* BtnViewIcons;
	Rzbutton::TRzToolbarButton* BtnCalendarWeek;
	Rzbutton::TRzToolbarButton* BtnSignalRed;
	Rzbutton::TRzToolbarButton* BtnSignalOrange;
	Rzbutton::TRzToolbarButton* BtnSignalGreen;
	Rzbutton::TRzToolbarButton* BtnSignalLtBlue;
	Rzbutton::TRzToolbarButton* BtnSignalYellow;
	Rzbutton::TRzToolbarButton* BtnSignalReminder;
	Rzbutton::TRzToolbarButton* BtnSignalWarning;
	Rzbutton::TRzToolbarButton* BtnSignalBlue;
	Rzbutton::TRzToolbarButton* BtnFormatWordWrap;
	Rzbutton::TRzToolbarButton* BtnSignalGray;
	Rzbutton::TRzToolbarButton* BtnFormatTabs;
	Rzbutton::TRzToolbarButton* BtnSignalInfo;
	Rzbutton::TRzToolbarButton* BtnSignalViolet;
	Rzradchk::TRzCheckBox* ChkAddDisabled;
	Rzpanel::TRzGroupBox* GrpImageList;
	Rzlstbox::TRzListBox* LstImageList;
	Vcl::Forms::TScrollBox* SbxCustom;
	Vcl::Menus::TPopupMenu* MnuCustomImages;
	Vcl::Menus::TMenuItem* AddCustomImage1;
	Vcl::Extdlgs::TOpenPictureDialog* DlgOpenPicture;
	Rzlabel::TRzLabel* LblNoImageListTitle;
	Rzlabel::TRzLabel* LblNoImageListMsg;
	Rzbutton::TRzToolbarButton* BtnViewList;
	Rzbutton::TRzToolbarButton* BtnViewDetails;
	Rzbutton::TRzToolbarButton* BtnDBInsert;
	Rzbutton::TRzToolbarButton* BtnDBDelete;
	Vcl::Menus::TPopupMenu* MnuCustom;
	Vcl::Menus::TMenuItem* MnuAdd;
	Vcl::Menus::TMenuItem* MnuSep1;
	Vcl::Menus::TMenuItem* MnuEditCustomHint;
	Vcl::Menus::TMenuItem* MnuDelete;
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall ToolbarButtonClick(System::TObject* Sender);
	void __fastcall MnuAddClick(System::TObject* Sender);
	void __fastcall MnuDeleteClick(System::TObject* Sender);
	void __fastcall FormDestroy(System::TObject* Sender);
	void __fastcall MnuEditStockHintClick(System::TObject* Sender);
	void __fastcall MnuEditCustomHintClick(System::TObject* Sender);
	void __fastcall SbxCustomResize(System::TObject* Sender);
	void __fastcall LstImageListDrawItem(Vcl::Controls::TWinControl* Control, int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	void __fastcall LstImageListClick(System::TObject* Sender);
	void __fastcall FormResize(System::TObject* Sender);
	
private:
	System::Classes::TPersistent* FObject;
	Vcl::Imglist::TCustomImageList* FImageList;
	bool FHasImageListProp;
	bool FHasGlyphProp;
	bool FIsToolButton;
	System::Classes::TStringList* FCustomImages;
	void __fastcall RearrangeCustomButtons();
	Rzbutton::TRzToolbarButton* __fastcall CreateCustomButton(const System::UnicodeString S);
	System::UnicodeString __fastcall GetFileNameFromString(const System::UnicodeString S);
	System::UnicodeString __fastcall GetHintFromString(const System::UnicodeString S);
	void __fastcall AddImageToImgList(Vcl::Imglist::TCustomImageList* ImageList, Vcl::Graphics::TBitmap* Glyph);
	void __fastcall HandleTextChange(System::TObject* Sender);
	int __fastcall GetObjectImageIndex();
	void __fastcall SetObjectImageIndex(int Value);
	void __fastcall SetObjectDisabledIndex(int Value);
	void __fastcall SetObjectCaption(const System::UnicodeString Value);
	void __fastcall SetObjectHint(const System::UnicodeString Value);
	void __fastcall SetObjectGlyph(Vcl::Graphics::TBitmap* Glyph);
	void __fastcall SetObjectNumGlyphs(int Value);
	
public:
	TRzCustomImageIndexProperty* FPropertyEditor;
	bool FEditingProperty;
	Rzbutton::TRzToolbarButton* SelectedBtn;
	System::Classes::TComponent* CompOwner;
	TRzTextChangeType TextChangeType;
	void __fastcall Reposition();
	void __fastcall UpdateControls();
	void __fastcall SetPropertyEditor(TRzCustomImageIndexProperty* PropEditor);
	void __fastcall SetObject(System::Classes::TPersistent* Value, bool EditingProperty);
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzSelectImageEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzSelectImageEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzSelectImageEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzSelectImageEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzselectimageeditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSELECTIMAGEEDITOR)
using namespace Rzselectimageeditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzselectimageeditorHPP
