// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzDBDesignEditors.pas' rev: 35.00 (Windows)

#ifndef RzdbdesigneditorsHPP
#define RzdbdesigneditorsHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <System.Classes.hpp>
#include <Vcl.Menus.hpp>
#include <DesignIntf.hpp>
#include <DesignEditors.hpp>
#include <DBReg.hpp>
#include <RzDesignEditors.hpp>
#include <RzEdit.hpp>
#include <RzDBEdit.hpp>
#include <RzDBList.hpp>
#include <RzDBCmbo.hpp>
#include <RzDBChk.hpp>
#include <RzDBLbl.hpp>
#include <RzDBBnEd.hpp>
#include <RzDBStat.hpp>
#include <RzDBNav.hpp>
#include <RzDBSpin.hpp>
#include <RzDBRGrp.hpp>
#include <RzDBGrid.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzdbdesigneditors
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS TRzDBControlEditor;
class DELPHICLASS TRzDBListBoxEditor;
class DELPHICLASS TRzDBLookupComboBoxEditor;
class DELPHICLASS TRzDBMemoEditor;
class DELPHICLASS TRzDBRichEditEditor;
class DELPHICLASS TRzDBDateTimeEditEditor;
class DELPHICLASS TRzDBStatusPaneEditor;
class DELPHICLASS TRzDBStateStatusEditor;
class DELPHICLASS TRzDBLabelEditor;
class DELPHICLASS TRzDBProgressBarEditor;
class DELPHICLASS TRzDBButtonEditEditor;
class DELPHICLASS TRzDBNumericEditEditor;
class DELPHICLASS TRzDBSpinEditEditor;
class DELPHICLASS TRzDBSpinnerEditor;
class DELPHICLASS TRzDBNavigatorEditor;
class DELPHICLASS TRzDBCheckBoxEditor;
class DELPHICLASS TRzDBCheckBoxGroupEditor;
class DELPHICLASS TRzDBGridEditor;
class DELPHICLASS TRzSearchFieldProperty;
class DELPHICLASS TRzDBDateTimeFormatProperty;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBControlEditor : public Rzdesigneditors::TRzDefaultEditor
{
	typedef Rzdesigneditors::TRzDefaultEditor inherited;
	
protected:
	virtual int __fastcall DataSourceMenuIndex();
	virtual int __fastcall DataFieldMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall DataSourceMenuHandler(System::TObject* Sender);
	void __fastcall DataFieldMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBControlEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : Rzdesigneditors::TRzDefaultEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBControlEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBListBoxEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdblist::TRzDBListBox* __fastcall ListBox();
	virtual int __fastcall AlignMenuIndex();
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBListBoxEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBListBoxEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBLookupComboBoxEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	virtual int __fastcall ListSourceMenuIndex();
	virtual int __fastcall KeyFieldMenuIndex();
	virtual int __fastcall ListFieldMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ListSourceMenuHandler(System::TObject* Sender);
	void __fastcall KeyFieldMenuHandler(System::TObject* Sender);
	void __fastcall ListFieldMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBLookupComboBoxEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBLookupComboBoxEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBMemoEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	virtual bool __fastcall GetWordWrap();
	virtual void __fastcall SetWordWrap(bool Value);
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	__property bool WordWrap = {read=GetWordWrap, write=SetWordWrap, nodefault};
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBMemoEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBMemoEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBRichEditEditor : public TRzDBMemoEditor
{
	typedef TRzDBMemoEditor inherited;
	
protected:
	virtual bool __fastcall GetWordWrap();
	virtual void __fastcall SetWordWrap(bool Value);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBRichEditEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBMemoEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBRichEditEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBDateTimeEditEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbedit::TRzDBDateTimeEdit* __fastcall DateTimeEdit();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ElementsMenuHandler(System::TObject* Sender);
	void __fastcall FirstDayOfWeekMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBDateTimeEditEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBDateTimeEditEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBStatusPaneEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	virtual int __fastcall FlatStyleMenuIndex();
	virtual int __fastcall AutoSizeMenuIndex();
	virtual int __fastcall AlignmentMenuIndex();
	virtual int __fastcall BlinkingMenuIndex();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall AlignmentMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBStatusPaneEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBStatusPaneEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBStateStatusEditor : public TRzDBStatusPaneEditor
{
	typedef TRzDBStatusPaneEditor inherited;
	
protected:
	Rzdbstat::TRzDBStateStatus* __fastcall StateStatus();
	virtual int __fastcall DataFieldMenuIndex();
	virtual int __fastcall FlatStyleMenuIndex();
	virtual int __fastcall AutoSizeMenuIndex();
	virtual int __fastcall AlignmentMenuIndex();
	virtual int __fastcall BlinkingMenuIndex();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall GlyphAlignmentMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBStateStatusEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBStatusPaneEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBStateStatusEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBLabelEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdblbl::TRzDBLabel* __fastcall LabelControl();
	virtual int __fastcall AlignMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall TextStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBLabelEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBLabelEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBProgressBarEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	virtual int __fastcall BaseFieldMenuIndex();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall BaseFieldMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBProgressBarEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBProgressBarEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBButtonEditEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbbned::TRzDBButtonEdit* __fastcall ButtonEdit();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall ButtonKindMenuHandler(System::TObject* Sender);
	void __fastcall AltBtnKindMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBButtonEditEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBButtonEditEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBNumericEditEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbedit::TRzDBNumericEdit* __fastcall NumericEdit();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBNumericEditEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBNumericEditEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBSpinEditEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbspin::TRzDBSpinEdit* __fastcall SpinEdit();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall DirectionMenuHandler(System::TObject* Sender);
	void __fastcall OrientationMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBSpinEditEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBSpinEditEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBSpinnerEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbspin::TRzDBSpinner* __fastcall Spinner();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBSpinnerEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBSpinnerEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBNavigatorEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbnav::TRzDBNavigator* __fastcall Navigator();
	virtual int __fastcall DataFieldMenuIndex();
	virtual bool __fastcall GetCompRefData(int Index, System::Classes::TComponentClass &CompRefClass, System::UnicodeString &CompRefPropName, System::Classes::TNotifyEvent &CompRefMenuHandler);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBNavigatorEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBNavigatorEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBCheckBoxEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbchk::TRzDBCheckBox* __fastcall CheckBox();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBCheckBoxEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBCheckBoxEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBCheckBoxGroupEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbrgrp::TRzDBCheckBoxGroup* __fastcall GroupBox();
	virtual void __fastcall PrepareMenuItem(int Index, Vcl::Menus::TMenuItem* const Item);
	void __fastcall GroupStyleMenuHandler(System::TObject* Sender);
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBCheckBoxGroupEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBCheckBoxGroupEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBGridEditor : public TRzDBControlEditor
{
	typedef TRzDBControlEditor inherited;
	
protected:
	Rzdbgrid::TRzDBGrid* __fastcall Grid();
	virtual int __fastcall DataFieldMenuIndex();
	virtual int __fastcall AlignMenuIndex();
	
public:
	virtual int __fastcall GetVerbCount();
	virtual System::UnicodeString __fastcall GetVerb(int Index);
	virtual void __fastcall ExecuteVerb(int Index);
public:
	/* TComponentEditor.Create */ inline __fastcall virtual TRzDBGridEditor(System::Classes::TComponent* AComponent, Designintf::_di_IDesigner ADesigner) : TRzDBControlEditor(AComponent, ADesigner) { }
	
public:
	/* TObject.Destroy */ inline __fastcall virtual ~TRzDBGridEditor() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzSearchFieldProperty : public Dbreg::TDBStringProperty
{
	typedef Dbreg::TDBStringProperty inherited;
	
public:
	virtual void __fastcall GetValueList(System::Classes::TStrings* List);
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzSearchFieldProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Dbreg::TDBStringProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzSearchFieldProperty() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzDBDateTimeFormatProperty : public Rzdesigneditors::TRzDateTimeFormatProperty
{
	typedef Rzdesigneditors::TRzDateTimeFormatProperty inherited;
	
protected:
	virtual Rzdesigneditors::TRzDateTimeFormatFilter __fastcall FormatFilter();
public:
	/* TPropertyEditor.Create */ inline __fastcall virtual TRzDBDateTimeFormatProperty(const Designintf::_di_IDesigner ADesigner, int APropCount) : Rzdesigneditors::TRzDateTimeFormatProperty(ADesigner, APropCount) { }
	/* TPropertyEditor.Destroy */ inline __fastcall virtual ~TRzDBDateTimeFormatProperty() { }
	
};

#pragma pack(pop)

//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzdbdesigneditors */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZDBDESIGNEDITORS)
using namespace Rzdbdesigneditors;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzdbdesigneditorsHPP
