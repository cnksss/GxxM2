// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzToolbarEditor.pas' rev: 35.00 (Windows)

#ifndef RztoolbareditorHPP
#define RztoolbareditorHPP

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
#include <DesignEditors.hpp>
#include <DesignMenus.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Dialogs.hpp>
#include <Vcl.Buttons.hpp>
#include <Vcl.StdCtrls.hpp>
#include <RzPanel.hpp>
#include <RzLabel.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Menus.hpp>
#include <RzSpnEdt.hpp>
#include <RzBorder.hpp>
#include <RzRadGrp.hpp>
#include <Vcl.Mask.hpp>
#include <RzEdit.hpp>
#include <RzButton.hpp>
#include <RzDesignEditors.hpp>
#include <RzLstBox.hpp>
#include <RzRadChk.hpp>
#include <Vcl.ExtDlgs.hpp>
#include <System.UITypes.hpp>
#include <System.Types.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rztoolbareditor
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzToolbarEditor;
class DELPHICLASS TRzToolbarEditDlg;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzToolbarEditor : public Rzdesigneditors::TRzComponentEditor
{
	typedef Rzdesigneditors::TRzComponentEditor inherited;
	
protected:
	Rzpanel::TRzToolbar* __fastcall Toolbar();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall LayoutMenuHandler(System::TObject* Sender);
	void __fastcall SettingsMenuHandler(System::TObject* Sender);
	void __fastcall VisualStyleMenuHandler(System::TObject* Sender);
	void __fastcall GradientColorStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzToolbarEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzComponentEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzToolbarEditor() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzToolbarEditDlg : public Vcl::Forms::TForm
{
	typedef Vcl::Forms::TForm inherited;
	
__published:
	Vcl::Menus::TPopupMenu* MnuStock;
	Vcl::Menus::TMenuItem* MnuEditStockHint;
	Rzpanel::TRzGroupBox* GrpControls;
	Rzlstbox::TRzListBox* LstOrder;
	Vcl::Extctrls::TTimer* Timer1;
	Rzpanel::TRzPanel* RzPanel1;
	Rzbutton::TRzToolbarButton* BtnMoveUp;
	Rzbutton::TRzToolbarButton* BtnMoveDown;
	Rzbutton::TRzToolbarButton* BtnDeleteControl;
	Vcl::Buttons::TSpeedButton* BtnChangePrefix;
	Rzpanel::TRzGroupBox* GrpStockImages;
	Vcl::Buttons::TSpeedButton* BtnSpacer;
	Vcl::Buttons::TSpeedButton* BtnGrooveSpacer;
	Rzborder::TRzBorder* RzBorder1;
	Rzpanel::TRzGroupBox* GrpCustomImages;
	Vcl::Forms::TScrollBox* SbxCustom;
	Vcl::Menus::TPopupMenu* MnuCustomImages;
	Vcl::Menus::TMenuItem* AddCustomImage1;
	Vcl::Extdlgs::TOpenPictureDialog* DlgOpenPicture;
	Rzlabel::TRzLabel* LblNoImageListMsg;
	Rzlabel::TRzLabel* LblNoImageListTitle;
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
	Rzbutton::TRzToolbarButton* BtnDBFirst;
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
	Rzbutton::TRzToolbarButton* BtnViewList;
	Rzbutton::TRzToolbarButton* BtnViewDetails;
	Rzbutton::TRzToolbarButton* BtnDBInsert;
	Rzbutton::TRzToolbarButton* BtnDBDelete;
	Rzpanel::TRzPanel* PnlStyle;
	Rzpanel::TRzPanel* PnlClose;
	Rzpanel::TRzGroupBox* GrpStyle;
	Rzbutton::TRzToolbarButton* BtnNoCaptionsStyle;
	Rzbutton::TRzToolbarButton* BtnCaptionsBottomStyle;
	Rzbutton::TRzToolbarButton* BtnCaptionsRightStyle;
	Rzradchk::TRzCheckBox* ChkAddDisabled;
	Rzbutton::TRzButton* BtnDone;
	Vcl::Menus::TPopupMenu* MnuCustom;
	Vcl::Menus::TMenuItem* MnuAdd;
	Vcl::Menus::TMenuItem* MnuSep1;
	Vcl::Menus::TMenuItem* MnuEditCustomHint;
	Vcl::Menus::TMenuItem* MnuDelete;
	Vcl::Menus::TPopupMenu* MnuControls;
	Vcl::Menus::TMenuItem* MnuDeleteControl;
	void __fastcall FormCreate(System::TObject* Sender);
	void __fastcall ToolbarButtonClick(System::TObject* Sender);
	void __fastcall BtnSpacerClick(System::TObject* Sender);
	void __fastcall MnuAddClick(System::TObject* Sender);
	void __fastcall MnuDeleteClick(System::TObject* Sender);
	void __fastcall FormDestroy(System::TObject* Sender);
	void __fastcall MnuEditStockHintClick(System::TObject* Sender);
	void __fastcall BtnMoveDownClick(System::TObject* Sender);
	void __fastcall BtnMoveUpClick(System::TObject* Sender);
	void __fastcall LstOrderClick(System::TObject* Sender);
	void __fastcall LstOrderDragDrop(System::TObject* Sender, System::TObject* Source, int X, int Y);
	void __fastcall LstOrderDragOver(System::TObject* Sender, System::TObject* Source, int X, int Y, System::Uitypes::TDragState State, bool &Accept);
	void __fastcall LstOrderDrawItem(Vcl::Controls::TWinControl* Control, int Index, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	void __fastcall LstOrderEndDrag(System::TObject* Sender, System::TObject* Target, int X, int Y);
	void __fastcall LstOrderMouseDown(System::TObject* Sender, System::Uitypes::TMouseButton Button, System::Classes::TShiftState Shift, int X, int Y);
	void __fastcall Timer1Timer(System::TObject* Sender);
	void __fastcall BtnDeleteControlClick(System::TObject* Sender);
	void __fastcall BtnChangePrefixClick(System::TObject* Sender);
	void __fastcall ButtonStyleClick(System::TObject* Sender);
	void __fastcall MnuEditCustomHintClick(System::TObject* Sender);
	void __fastcall SbxCustomResize(System::TObject* Sender);
	void __fastcall MnuPrefixClick(System::TObject* Sender);
	void __fastcall MnuSuffixClick(System::TObject* Sender);
	void __fastcall FormResize(System::TObject* Sender);
	
private:
	bool FUsePrefix;
	System::UnicodeString FPrefix;
	int OldIdx;
	int NewIdx;
	bool GoingUp;
	System::Classes::TStringList* FCustomImages;
	void __fastcall RearrangeCustomButtons();
	Rzbutton::TRzToolbarButton* __fastcall CreateCustomButton(const System::UnicodeString S);
	System::UnicodeString __fastcall GetFileNameFromString(const System::UnicodeString S);
	System::UnicodeString __fastcall GetHintFromString(const System::UnicodeString S);
	void __fastcall EnableMoveButtons(int Idx);
	Rzbutton::TRzToolButton* __fastcall CreateNewButton(const System::UnicodeString Hint);
	void __fastcall AddImageToImageList(Rzbutton::TRzToolButton* Btn, Vcl::Graphics::TBitmap* Glyph);
	
public:
	Rzbutton::TRzToolbarButton* SelectedBtn;
	System::Classes::TComponent* CompOwner;
	Rzpanel::TRzToolbar* Toolbar;
	void __fastcall UpdateControls();
	void __fastcall Reposition();
public:
	/* TCustomForm.Create */ inline __fastcall virtual TRzToolbarEditDlg(System::Classes::TComponent* AOwner) : Vcl::Forms::TForm(AOwner) { }
	/* TCustomForm.CreateNew */ inline __fastcall virtual TRzToolbarEditDlg(System::Classes::TComponent* AOwner, int Dummy) : Vcl::Forms::TForm(AOwner, Dummy) { }
	/* TCustomForm.Destroy */ inline __fastcall virtual ~TRzToolbarEditDlg() { }
	
public:
	/* TWinControl.CreateParented */ inline __fastcall TRzToolbarEditDlg(HWND ParentWindow) : Vcl::Forms::TForm(ParentWindow) { }
	
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rztoolbareditor */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZTOOLBAREDITOR)
using namespace Rztoolbareditor;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RztoolbareditorHPP
