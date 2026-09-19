// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzCommon.pas' rev: 35.00 (Windows)

#ifndef RzcommonHPP
#define RzcommonHPP

#pragma delphiheader begin
#pragma option push
#pragma option -w-      // All warnings off
#pragma option -Vx      // Zero-length empty class member 
#pragma pack(push,8)
#include <System.hpp>
#include <SysInit.hpp>
#include <Winapi.Windows.hpp>
#include <System.Types.hpp>
#include <System.UITypes.hpp>
#include <System.Classes.hpp>
#include <Vcl.Forms.hpp>
#include <Vcl.Graphics.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.Menus.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <System.SysUtils.hpp>
#include <Vcl.ImgList.hpp>
#include <Vcl.Themes.hpp>
#include <Winapi.UxTheme.hpp>
#include <System.Win.Registry.hpp>
#include <System.Contnrs.hpp>
#include <System.IniFiles.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzcommon
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS EInvalidFieldType;
class DELPHICLASS TRzBlinkingControlsList;
class DELPHICLASS TRzOldPropReader;
class DELPHICLASS TRzDialogComponent;
__interface DELPHIINTERFACE IRzCustomFramingNotification;
typedef System::DelphiInterface<IRzCustomFramingNotification> _di_IRzCustomFramingNotification;
class DELPHICLASS TRzFrameController;
class DELPHICLASS ENoRegIniFile;
class DELPHICLASS TRzRegIniFile;
class DELPHICLASS TRzPropertyItem;
class DELPHICLASS TRzPropertyCollection;
class DELPHICLASS TRzPropertyStore;
class DELPHICLASS TRzCustomColors;
class DELPHICLASS TRzMenuController;
//-- type declarations -------------------------------------------------------
enum DECLSPEC_DENUM TAlignmentVertical : unsigned char { avTop, avCenter, avBottom };

enum DECLSPEC_DENUM TBlinkState : unsigned char { bsOn, bsOff };

enum DECLSPEC_DENUM TTextStyle : unsigned char { tsNormal, tsRaised, tsRecessed, tsShadow };

enum DECLSPEC_DENUM TOrientation : unsigned char { orHorizontal, orVertical };

enum DECLSPEC_DENUM TBarStyle : unsigned char { bsTraditional, bsLED, bsGradient };

enum DECLSPEC_DENUM TLineStyle : unsigned char { lsNone, lsFlat, lsGroove, lsBump };

enum DECLSPEC_DENUM TFrameStyleEx : unsigned char { fsNone, fsFlat, fsGroove, fsBump, fsLowered, fsButtonDown, fsRaised, fsButtonUp, fsStatus, fsPopup, fsFlatBold, fsFlatRounded };

typedef TFrameStyleEx TFrameStyle;

enum DECLSPEC_DENUM TFramingPreference : unsigned char { fpCustomFraming, fpXPThemes };

enum DECLSPEC_DENUM TRzGroupBarGradientPath : unsigned char { gpTopToBottom, gpBottomToTop };

enum DECLSPEC_DENUM TGradientDirection : unsigned char { gdDiagonalUp, gdDiagonalDown, gdHorizontalEnd, gdHorizontalCenter, gdHorizontalBox, gdVerticalEnd, gdVerticalCenter, gdVerticalBox, gdSquareBox, gdBigSquareBox };

enum DECLSPEC_DENUM TDirection : unsigned char { dirUp, dirDown, dirLeft, dirRight };

enum DECLSPEC_DENUM TRzScrollStyle : unsigned char { scsNone, scsLeft, scsUp, scsRight, scsDown };

enum DECLSPEC_DENUM TSide : unsigned char { sdLeft, sdTop, sdRight, sdBottom };

typedef System::Set<TSide, TSide::sdLeft, TSide::sdBottom> TSides;

typedef void __fastcall (__closure *TPositionChangingEvent)(System::TObject* Sender, int NewPos, bool &AllowChange);

enum DECLSPEC_DENUM TExpandOnType : unsigned char { etMouseButton2Click, etFocus, etNone };

typedef System::Byte TPositiveByte;

typedef System::Word TPositiveSmallint;

typedef int TPositiveInteger;

typedef System::Word TUnsignedSmallint;

typedef NativeInt TRzNativeInt;

#pragma pack(push,4)
class PASCALIMPLEMENTATION EInvalidFieldType : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall EInvalidFieldType(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EInvalidFieldType(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidFieldType(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EInvalidFieldType(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidFieldType(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EInvalidFieldType(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EInvalidFieldType(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EInvalidFieldType(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidFieldType(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EInvalidFieldType(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidFieldType(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EInvalidFieldType(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EInvalidFieldType() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzTwoDigitYearConverter : unsigned char { ycStandard, ycPastNotFuture };

enum DECLSPEC_DENUM TRzScrollType : unsigned char { stNone, stRightToLeft, stLeftToRight };

typedef void __fastcall (__closure *TRzScrollDisplayEvent)(System::TObject* Sender, int CurrentStep, int TotalSteps);

enum DECLSPEC_DENUM TRzHotTrackColorType : unsigned char { htctComplement, htctActual };

typedef void __fastcall (__closure *TStateChangingEvent)(System::TObject* Sender, int Index, Vcl::Stdctrls::TCheckBoxState NewState, bool &AllowChange);

typedef void __fastcall (__closure *TStateChangeEvent)(System::TObject* Sender, int Index, Vcl::Stdctrls::TCheckBoxState NewState);

enum DECLSPEC_DENUM TRzAboutInfo : unsigned char { aiRaizeComponents };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzBlinkingControlsList : public System::TObject
{
	typedef System::TObject inherited;
	
private:
	TBlinkState FBlinkState;
	System::Classes::TList* FControls;
	Vcl::Extctrls::TTimer* FTimer;
	System::Word FIntervalOff;
	System::Word FIntervalOn;
	
protected:
	virtual int __fastcall GetCount();
	
public:
	__fastcall TRzBlinkingControlsList();
	__fastcall virtual ~TRzBlinkingControlsList();
	void __fastcall Add(Vcl::Controls::TControl* Control);
	void __fastcall Remove(Vcl::Controls::TControl* Control);
	void __fastcall TimerFired(System::TObject* Sender);
	__property int Count = {read=GetCount, nodefault};
	__property System::Word IntervalOff = {read=FIntervalOff, write=FIntervalOff, nodefault};
	__property System::Word IntervalOn = {read=FIntervalOn, write=FIntervalOn, nodefault};
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzGradientColorStyle : unsigned char { gcsSystem, gcsMSOffice, gcsCustom };

enum DECLSPEC_DENUM TRzVisualStyle : unsigned char { vsClassic, vsWinXP, vsGradient };

enum DECLSPEC_DENUM TRzXPThemeColor : unsigned char { xptcEditBorder, xptcNormalGroupFont, xptcSpecialGroupFont, xptcGroupBarFill, xptcListGroupFill, xptcListGroupFont, xptcGroupBoxFont, xptcSpinButtonBorder };

enum DECLSPEC_DENUM TRzXPColorScheme : unsigned char { xpcsBlue, xpcsGreen, xpcsSilver };

enum DECLSPEC_DENUM TRzUIStyle : unsigned char { uiWindows95, uiWindowsXP, uiWindowsVista, uiWindows10, uiCustomVclStyle, uiCustomColor };

enum DECLSPEC_DENUM TRzButtonDisplayState : unsigned char { bdsNormal, bdsDown, bdsHot, bdsDisabled };

enum DECLSPEC_DENUM TRzButtonHotTrackStyle : unsigned char { htsInterior, htsFrame };

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzOldPropReader : public System::TObject
{
	typedef System::TObject inherited;
	
public:
	__classmethod void __fastcall ReadOldBooleanProp(System::Classes::TReader* Reader);
	__classmethod void __fastcall ReadOldEnumProp(System::Classes::TReader* Reader);
	__classmethod void __fastcall ReadOldIdentProp(System::Classes::TReader* Reader);
	__classmethod void __fastcall ReadOldIntegerProp(System::Classes::TReader* Reader);
	__classmethod void __fastcall ReadOldSetProp(System::Classes::TReader* Reader);
	__classmethod void __fastcall ReadOldStringProp(System::Classes::TReader* Reader);
	__classmethod void __fastcall WriteOldProp(System::Classes::TWriter* Writer);
public:
	/* TObject.Create */ inline __fastcall TRzOldPropReader() : System::TObject() { }
	/* TObject.Destroy */ inline __fastcall virtual ~TRzOldPropReader() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzDialogComponent : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Vcl::Forms::TFormBorderStyle FBorderStyle;
	System::UnicodeString FCaption;
	System::UnicodeString FCaptionOk;
	System::UnicodeString FCaptionCancel;
	System::UnicodeString FCaptionHelp;
	bool FCenterToParent;
	Vcl::Graphics::TFont* FFont;
	System::Uitypes::TColor FFrameColor;
	TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	TFramingPreference FFramingPreference;
	int FHeight;
	System::Classes::THelpContext FHelpContext;
	int FOriginLeft;
	int FOriginTop;
	int FWidth;
	Vcl::Forms::TPosition FPosition;
	
protected:
	TRzAboutInfo FAboutInfo;
	void __fastcall CenterForm(Vcl::Forms::TForm* Dlg);
	virtual void __fastcall SetFont(Vcl::Graphics::TFont* Value);
	virtual void __fastcall SetCenterToParent(bool Value);
	__property Vcl::Forms::TFormBorderStyle BorderStyle = {read=FBorderStyle, write=FBorderStyle, default=2};
	__property System::UnicodeString Caption = {read=FCaption, write=FCaption};
	__property System::UnicodeString CaptionOK = {read=FCaptionOk, write=FCaptionOk};
	__property System::UnicodeString CaptionCancel = {read=FCaptionCancel, write=FCaptionCancel};
	__property System::UnicodeString CaptionHelp = {read=FCaptionHelp, write=FCaptionHelp};
	__property Vcl::Graphics::TFont* Font = {read=FFont, write=SetFont};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=FFrameColor, default=-16777200};
	__property TFrameStyle FrameStyle = {read=FFrameStyle, write=FFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=FFrameVisible, default=0};
	__property TFramingPreference FramingPreference = {read=FFramingPreference, write=FFramingPreference, default=1};
	__property int Height = {read=FHeight, write=FHeight, default=300};
	__property System::Classes::THelpContext HelpContext = {read=FHelpContext, write=FHelpContext, default=0};
	__property int Width = {read=FWidth, write=FWidth, default=350};
	
public:
	__fastcall virtual TRzDialogComponent(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzDialogComponent();
	
__published:
	__property bool CenterToParent = {read=FCenterToParent, write=SetCenterToParent, default=0};
	__property int OriginLeft = {read=FOriginLeft, write=FOriginLeft, default=100};
	__property Vcl::Forms::TPosition Position = {read=FPosition, write=FPosition, default=4};
	__property int OriginTop = {read=FOriginTop, write=FOriginTop, default=100};
};


enum DECLSPEC_DENUM TRzFrameControllerProperty : unsigned char { fcpColor, fcpFocusColor, fcpDisabledColor, fcpReadOnlyColor, fcpReadOnlyColorOnFocus, fcpParentColor, fcpFlatButtonColor, fcpFlatButtons, fcpFrameColor, fcpFrameHotColor, fcpFrameHotTrack, fcpFrameHotStyle, fcpFrameSides, fcpFrameStyle, fcpFrameVisible, fcpFramingPreference, fcpAll };

typedef TRzFrameControllerProperty TRzFrameControllerPropertyConnection;

typedef System::Set<TRzFrameControllerPropertyConnection, TRzFrameControllerPropertyConnection::fcpColor, TRzFrameControllerPropertyConnection::fcpFramingPreference> TRzFrameControllerNotifications;

__interface  INTERFACE_UUID("{64B1C9EA-C954-428A-95C4-4EA2EB0F1E16}") IRzCustomFramingNotification  : public System::IInterface 
{
	virtual void __fastcall CustomFramingChanged() = 0 ;
};

class PASCALIMPLEMENTATION TRzFrameController : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	TRzAboutInfo FAboutInfo;
	System::Uitypes::TColor FColor;
	bool FParentColor;
	System::Classes::TList* FFrameList;
	System::Uitypes::TColor FFlatButtonColor;
	bool FFlatButtons;
	System::Uitypes::TColor FDisabledColor;
	System::Uitypes::TColor FReadOnlyColor;
	bool FReadOnlyColorOnFocus;
	System::Uitypes::TColor FFocusColor;
	System::Uitypes::TColor FFrameColor;
	System::Uitypes::TColor FFrameHotColor;
	bool FFrameHotTrack;
	TFrameStyleEx FFrameHotStyle;
	TSides FFrameSides;
	TFrameStyleEx FFrameStyle;
	bool FFrameVisible;
	TFramingPreference FFramingPreference;
	int FUpdateCount;
	TRzRegIniFile* FRegIniFile;
	void __fastcall ReadOldFrameFlatProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameFocusStyleProp(System::Classes::TReader* Reader);
	bool __fastcall StoreColor();
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	TRzFrameControllerNotifications __fastcall GetNotifications(System::Classes::TComponent* C);
	virtual void __fastcall UpdateControlFrame(System::Classes::TComponent* C, TRzFrameControllerProperty FrameProperty);
	virtual void __fastcall UpdateFrames(TRzFrameControllerProperty FrameProperty);
	virtual void __fastcall SetColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetParentColor(bool Value);
	virtual void __fastcall SetFlatButtonColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFlatButtons(bool Value);
	virtual void __fastcall SetDisabledColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetReadOnlyColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetReadOnlyColorOnFocus(bool Value);
	virtual void __fastcall SetFocusColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotColor(System::Uitypes::TColor Value);
	virtual void __fastcall SetFrameHotTrack(bool Value);
	virtual void __fastcall SetFrameHotStyle(TFrameStyle Value);
	virtual void __fastcall SetFrameSides(TSides Value);
	virtual void __fastcall SetFrameStyle(TFrameStyle Value);
	virtual void __fastcall SetFrameVisible(bool Value);
	virtual void __fastcall SetFramingPreference(TFramingPreference Value);
	virtual void __fastcall SetRegIniFile(TRzRegIniFile* Value);
	
public:
	__fastcall virtual TRzFrameController(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzFrameController();
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	void __fastcall BeginUpdate();
	void __fastcall EndUpdate();
	void __fastcall UpdateControls();
	void __fastcall AddControl(System::Classes::TComponent* C);
	void __fastcall RemoveControl(System::Classes::TComponent* C);
	void __fastcall Load(const System::UnicodeString Section);
	void __fastcall Save(const System::UnicodeString Section);
	
__published:
	__property TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Uitypes::TColor Color = {read=FColor, write=SetColor, stored=StoreColor, default=-16777211};
	__property System::Uitypes::TColor DisabledColor = {read=FDisabledColor, write=SetDisabledColor, default=-16777201};
	__property System::Uitypes::TColor ReadOnlyColor = {read=FReadOnlyColor, write=SetReadOnlyColor, default=-16777192};
	__property bool ReadOnlyColorOnFocus = {read=FReadOnlyColorOnFocus, write=SetReadOnlyColorOnFocus, default=0};
	__property System::Uitypes::TColor FlatButtonColor = {read=FFlatButtonColor, write=SetFlatButtonColor, default=-16777201};
	__property bool FlatButtons = {read=FFlatButtons, write=SetFlatButtons, default=1};
	__property System::Uitypes::TColor FocusColor = {read=FFocusColor, write=SetFocusColor, default=-16777211};
	__property System::Uitypes::TColor FrameColor = {read=FFrameColor, write=SetFrameColor, default=-16777200};
	__property System::Uitypes::TColor FrameHotColor = {read=FFrameHotColor, write=SetFrameHotColor, default=-16777200};
	__property TFrameStyle FrameHotStyle = {read=FFrameHotStyle, write=SetFrameHotStyle, default=10};
	__property bool FrameHotTrack = {read=FFrameHotTrack, write=SetFrameHotTrack, default=0};
	__property TSides FrameSides = {read=FFrameSides, write=SetFrameSides, default=15};
	__property TFrameStyle FrameStyle = {read=FFrameStyle, write=SetFrameStyle, default=1};
	__property bool FrameVisible = {read=FFrameVisible, write=SetFrameVisible, default=0};
	__property TFramingPreference FramingPreference = {read=FFramingPreference, write=SetFramingPreference, default=1};
	__property bool ParentColor = {read=FParentColor, write=SetParentColor, default=0};
	__property TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION ENoRegIniFile : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	/* Exception.Create */ inline __fastcall ENoRegIniFile(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall ENoRegIniFile(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall ENoRegIniFile(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall ENoRegIniFile(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall ENoRegIniFile(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall ENoRegIniFile(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall ENoRegIniFile(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall ENoRegIniFile(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ENoRegIniFile(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall ENoRegIniFile(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ENoRegIniFile(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall ENoRegIniFile(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~ENoRegIniFile() { }
	
};

#pragma pack(pop)

enum DECLSPEC_DENUM TRzPathType : unsigned char { ptIniFile, ptRegistry };

enum DECLSPEC_DENUM TRzRegKey : unsigned char { hkeyClassesRoot, hkeyCurrentUser, hkeyLocalMachine, hkeyUsers, hkeyPerformanceData, hkeyCurrentConfig, hkeyDynData };

enum DECLSPEC_DENUM TRzRegAccessKey : unsigned char { keyQueryValue, keySetValue, keyCreateSubKey, keyEnumerateSubKeys, keyNotify, keyCreateLink, keyRead, keyWrite, keyExecute, keyAllAccess };

typedef System::Set<TRzRegAccessKey, TRzRegAccessKey::keyQueryValue, TRzRegAccessKey::keyAllAccess> TRzRegAccess;

enum DECLSPEC_DENUM TRzSpecialFolder : unsigned char { sfNone, sfUserAppDataRoaming, sfUserAppDataLocal, sfUserDocuments, sfProgramData };

enum DECLSPEC_DENUM TRzIniFileEncoding : unsigned char { feDefault, feUTF8, feUnicode };

class PASCALIMPLEMENTATION TRzRegIniFile : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	TRzAboutInfo FAboutInfo;
	System::UnicodeString FPath;
	TRzPathType FPathType;
	TRzRegKey FRegKey;
	TRzRegAccess FRegAccess;
	TRzSpecialFolder FSpecialFolder;
	TRzIniFileEncoding FFileEncoding;
	bool FRefreshStorage;
	bool FAutoUpdateIniFile;
	System::Inifiles::TMemIniFile* FIni;
	System::Win::Registry::TRegistryIniFile* FReg;
	
protected:
	void __fastcall CheckAccess();
	virtual void __fastcall SetPath(const System::UnicodeString Value);
	virtual void __fastcall SetPathType(TRzPathType Value);
	virtual void __fastcall SetRegKey(TRzRegKey Value);
	virtual void __fastcall SetRegAccess(TRzRegAccess Value);
	virtual void __fastcall SetSpecialFolder(TRzSpecialFolder Value);
	virtual void __fastcall SetFileEncoding(TRzIniFileEncoding Value);
	
public:
	__fastcall virtual TRzRegIniFile(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzRegIniFile();
	void __fastcall UpdateIniFile();
	bool __fastcall SectionExists(const System::UnicodeString Section);
	bool __fastcall ValueExists(const System::UnicodeString Section, const System::UnicodeString Name);
	bool __fastcall ReadBool(const System::UnicodeString Section, const System::UnicodeString Name, bool Default);
	void __fastcall WriteBool(const System::UnicodeString Section, const System::UnicodeString Name, bool Value);
	System::TDateTime __fastcall ReadDate(const System::UnicodeString Section, const System::UnicodeString Name, System::TDateTime Default);
	void __fastcall WriteDate(const System::UnicodeString Section, const System::UnicodeString Name, System::TDateTime Value);
	System::TDateTime __fastcall ReadDateTime(const System::UnicodeString Section, const System::UnicodeString Name, System::TDateTime Default);
	void __fastcall WriteDateTime(const System::UnicodeString Section, const System::UnicodeString Name, System::TDateTime Value);
	double __fastcall ReadFloat(const System::UnicodeString Section, const System::UnicodeString Name, double Default);
	void __fastcall WriteFloat(const System::UnicodeString Section, const System::UnicodeString Name, double Value);
	int __fastcall ReadInteger(const System::UnicodeString Section, const System::UnicodeString Name, int Default);
	void __fastcall WriteInteger(const System::UnicodeString Section, const System::UnicodeString Name, int Value);
	System::UnicodeString __fastcall ReadString(const System::UnicodeString Section, const System::UnicodeString Name, const System::UnicodeString Default);
	void __fastcall WriteString(const System::UnicodeString Section, const System::UnicodeString Name, const System::UnicodeString Value);
	System::TDateTime __fastcall ReadTime(const System::UnicodeString Section, const System::UnicodeString Name, System::TDateTime Default);
	void __fastcall WriteTime(const System::UnicodeString Section, const System::UnicodeString Name, System::TDateTime Value);
	int __fastcall ReadBinaryStream(const System::UnicodeString Section, const System::UnicodeString Name, System::Classes::TStream* Value);
	void __fastcall WriteBinaryStream(const System::UnicodeString Section, const System::UnicodeString Name, System::Classes::TStream* Value);
	void __fastcall ReadSection(const System::UnicodeString Section, System::Classes::TStrings* Strings);
	void __fastcall ReadSections(System::Classes::TStrings* Strings);
	void __fastcall ReadSectionValues(const System::UnicodeString Section, System::Classes::TStrings* Strings);
	void __fastcall EraseSection(const System::UnicodeString Section);
	void __fastcall DeleteKey(const System::UnicodeString Section, const System::UnicodeString Name);
	
__published:
	__property TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property bool AutoUpdateIniFile = {read=FAutoUpdateIniFile, write=FAutoUpdateIniFile, default=1};
	__property TRzIniFileEncoding FileEncoding = {read=FFileEncoding, write=SetFileEncoding, default=0};
	__property System::UnicodeString Path = {read=FPath, write=SetPath};
	__property TRzPathType PathType = {read=FPathType, write=SetPathType, default=0};
	__property TRzRegKey RegKey = {read=FRegKey, write=SetRegKey, default=1};
	__property TRzRegAccess RegAccess = {read=FRegAccess, write=SetRegAccess, default=512};
	__property TRzSpecialFolder SpecialFolder = {read=FSpecialFolder, write=SetSpecialFolder, default=0};
};


#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPropertyItem : public System::Classes::TCollectionItem
{
	typedef System::Classes::TCollectionItem inherited;
	
private:
	System::Classes::TComponent* FComponent;
	System::UnicodeString FPropertyName;
	
protected:
	virtual System::UnicodeString __fastcall GetDisplayName();
	
public:
	__fastcall virtual TRzPropertyItem(System::Classes::TCollection* Collection);
	virtual void __fastcall Assign(System::Classes::TPersistent* Source);
	
__published:
	__property System::Classes::TComponent* Component = {read=FComponent, write=FComponent};
	__property System::UnicodeString PropertyName = {read=FPropertyName, write=FPropertyName};
public:
	/* TCollectionItem.Destroy */ inline __fastcall virtual ~TRzPropertyItem() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION TRzPropertyCollection : public System::Classes::TCollection
{
	typedef System::Classes::TCollection inherited;
	
public:
	TRzPropertyItem* operator[](int Index) { return this->Items[Index]; }
	
private:
	TRzPropertyStore* FStore;
	HIDESBASE TRzPropertyItem* __fastcall GetItem(int Index);
	HIDESBASE void __fastcall SetItem(int Index, TRzPropertyItem* Value);
	
protected:
	DYNAMIC System::Classes::TPersistent* __fastcall GetOwner();
	
public:
	__fastcall TRzPropertyCollection(TRzPropertyStore* Store);
	HIDESBASE TRzPropertyItem* __fastcall Add();
	TRzPropertyItem* __fastcall AddProperty(System::Classes::TComponent* Component, const System::UnicodeString PropertyName);
	__property TRzPropertyItem* Items[int Index] = {read=GetItem, write=SetItem/*, default*/};
	__property TRzPropertyStore* Store = {read=FStore};
public:
	/* TCollection.Destroy */ inline __fastcall virtual ~TRzPropertyCollection() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzPropertyStore : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	TRzAboutInfo FAboutInfo;
	TRzPropertyCollection* FProperties;
	TRzRegIniFile* FRegIniFile;
	System::UnicodeString FSection;
	void __fastcall SaveProperties(const System::UnicodeString Section, const System::UnicodeString ParentPropName, System::TObject* Obj);
	void __fastcall SaveCollection(const System::UnicodeString Section, const System::UnicodeString FullPropName, System::Classes::TCollection* Collection);
	void __fastcall SaveStringList(const System::UnicodeString Section, const System::UnicodeString FullPropName, System::Classes::TStrings* StrList);
	void __fastcall LoadProperties(const System::UnicodeString Section, const System::UnicodeString ParentPropName, System::TObject* Obj);
	void __fastcall LoadCollection(const System::UnicodeString Section, const System::UnicodeString FullPropName, System::Classes::TCollection* Collection);
	void __fastcall LoadStringList(const System::UnicodeString Section, const System::UnicodeString FullPropName, System::Classes::TStrings* StrList);
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	System::UnicodeString __fastcall GetSection();
	virtual void __fastcall SetProperties(TRzPropertyCollection* Value);
	virtual void __fastcall SetRegIniFile(TRzRegIniFile* Value);
	
public:
	__fastcall virtual TRzPropertyStore(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzPropertyStore();
	virtual void __fastcall Save();
	virtual void __fastcall Load();
	void __fastcall AddProperty(System::Classes::TComponent* Component, const System::UnicodeString PropertyName);
	
__published:
	__property TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzPropertyCollection* Properties = {read=FProperties, write=SetProperties};
	__property TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
	__property System::UnicodeString Section = {read=FSection, write=FSection};
};


class PASCALIMPLEMENTATION TRzCustomColors : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	TRzAboutInfo FAboutInfo;
	System::Classes::TStrings* FColors;
	TRzRegIniFile* FRegIniFile;
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall InitColors();
	System::UnicodeString __fastcall GetColorName(int Index);
	void __fastcall FixupColors();
	virtual void __fastcall SetColors(System::Classes::TStrings* Value);
	virtual void __fastcall SetRegIniFile(TRzRegIniFile* Value);
	
public:
	__fastcall virtual TRzCustomColors(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzCustomColors();
	void __fastcall Load(const System::UnicodeString Section);
	void __fastcall Save(const System::UnicodeString Section);
	
__published:
	__property TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Classes::TStrings* Colors = {read=FColors, write=SetColors};
	__property TRzRegIniFile* RegIniFile = {read=FRegIniFile, write=SetRegIniFile};
};


class PASCALIMPLEMENTATION TRzMenuController : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	TRzAboutInfo FAboutInfo;
	System::Classes::TList* FMenuItemList;
	TRzGradientColorStyle FGradientColorStyle;
	System::Uitypes::TColor FSelectionColorStart;
	System::Uitypes::TColor FSelectionColorStop;
	System::Uitypes::TColor FSelectionFrameColor;
	System::Uitypes::TColor FIconBarColorStart;
	System::Uitypes::TColor FIconBarColorStop;
	System::Uitypes::TColor FMenuColor;
	System::Uitypes::TColor FMenuFontColor;
	Vcl::Graphics::TFont* FMenuFont;
	bool FUseCustomMenuFont;
	bool FUseMenuColorForMainMenu;
	HBRUSH FMainMenuBrushHandle;
	bool __fastcall HideMenuPrefix();
	void __fastcall ReadOldGradientColorStartProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldGradientColorStopProp(System::Classes::TReader* Reader);
	void __fastcall ReadOldFrameColorProp(System::Classes::TReader* Reader);
	void __fastcall SetMenuFontColor(System::Uitypes::TColor Value);
	void __fastcall SetMenuFont(Vcl::Graphics::TFont* Value);
	void __fastcall SetUseMenuColorForMainMenu(bool Value);
	void __fastcall UpdateMainMenuColor();
	void __fastcall AdvancedDrawItemHandler(System::TObject* Sender, Vcl::Graphics::TCanvas* ACanvas, const System::Types::TRect &ARect, Winapi::Windows::TOwnerDrawState State);
	void __fastcall MeasureItemHandler(System::TObject* Sender, Vcl::Graphics::TCanvas* ACanvas, int &Width, int &Height);
	void __fastcall MenuFontChangeHandler(System::TObject* Sender);
	
protected:
	virtual void __fastcall DefineProperties(System::Classes::TFiler* Filer);
	virtual void __fastcall Loaded();
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	void __fastcall SetupMenus();
	void __fastcall SetupMenuItems();
	void __fastcall SetupMenuItem(Vcl::Menus::TMenuItem* Item);
	void __fastcall CleanupMenuItems();
	void __fastcall CleanupMenuItem(Vcl::Menus::TMenuItem* Item);
	bool __fastcall MenuIsRightToLeft(Vcl::Menus::TMenuItem* Item);
	void __fastcall MeasureMenuItem(Vcl::Menus::TMenuItem* Item, Vcl::Graphics::TCanvas* Canvas, int &Width, int &Height);
	void __fastcall PaintMenuItem(Vcl::Menus::TMenuItem* Item, Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Rect, Winapi::Windows::TOwnerDrawState State);
	
public:
	__fastcall virtual TRzMenuController(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzMenuController();
	
__published:
	__property TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property TRzGradientColorStyle GradientColorStyle = {read=FGradientColorStyle, write=FGradientColorStyle, default=0};
	__property System::Uitypes::TColor SelectionColorStart = {read=FSelectionColorStart, write=FSelectionColorStart, default=-16777201};
	__property System::Uitypes::TColor SelectionColorStop = {read=FSelectionColorStop, write=FSelectionColorStop, default=-16777200};
	__property System::Uitypes::TColor SelectionFrameColor = {read=FSelectionFrameColor, write=FSelectionFrameColor, default=-16777195};
	__property System::Uitypes::TColor IconBarColorStart = {read=FIconBarColorStart, write=FIconBarColorStart, default=16777215};
	__property System::Uitypes::TColor IconBarColorStop = {read=FIconBarColorStop, write=FIconBarColorStop, default=-16777201};
	__property System::Uitypes::TColor MenuColor = {read=FMenuColor, write=FMenuColor, default=-16777211};
	__property System::Uitypes::TColor MenuFontColor = {read=FMenuFontColor, write=SetMenuFontColor, default=-16777208};
	__property Vcl::Graphics::TFont* MenuFont = {read=FMenuFont, write=SetMenuFont, stored=FUseCustomMenuFont};
	__property bool UseCustomMenuFont = {read=FUseCustomMenuFont, write=FUseCustomMenuFont, default=0};
	__property bool UseMenuColorForMainMenu = {read=FUseMenuColorForMainMenu, write=SetUseMenuColorForMainMenu, default=0};
};


enum DECLSPEC_DENUM TRzWindowsVersion : unsigned char { win95, winNT, win98, winMe, win2000, winXP, winServer2003, winVista, win7, win8, win10 };

//-- var, const, procedure ---------------------------------------------------
#define RaizeComponents_Version L"6.2.3"
static const System::Word cm_GetBlinking = System::Word(0x8fb);
static const System::Word cm_Blink = System::Word(0x8fc);
static const System::Word cm_ToolbarShowCaptionChanged = System::Word(0x8fd);
static const System::Word cm_ToolbarButtonLayoutChanged = System::Word(0x8fe);
static const System::Word cm_ToolbarButtonSizeChanged = System::Word(0x8ff);
static const System::Word cm_ToolbarVisualStyleChanged = System::Word(0x900);
static const System::Word cm_HidePreviewPanel = System::Word(0x901);
static const System::Word cm_GroupItemSelected = System::Word(0x902);
#define sdAllSides (System::Set<TSide, TSide::sdLeft, TSide::sdBottom>() << TSide::sdLeft << TSide::sdTop << TSide::sdRight << TSide::sdBottom )
#define fsDoubleBorders (System::Set<TFrameStyleEx, TFrameStyleEx::fsNone, TFrameStyleEx::fsFlatRounded>() << TFrameStyleEx::fsGroove << TFrameStyleEx::fsBump << TFrameStyleEx::fsLowered << TFrameStyleEx::fsButtonDown << TFrameStyleEx::fsRaised << TFrameStyleEx::fsButtonUp << TFrameStyleEx::fsFlatBold << TFrameStyleEx::fsFlatRounded )
extern DELPHI_PACKAGE System::StaticArray<System::Uitypes::TColor, 11> ULFrameColor;
extern DELPHI_PACKAGE System::StaticArray<System::Uitypes::TColor, 11> LRFrameColor;
extern DELPHI_PACKAGE System::StaticArray<System::Word, 3> DrawTextAlignments;
extern DELPHI_PACKAGE System::StaticArray<System::Word, 3> SetTextAlignments;
extern DELPHI_PACKAGE System::StaticArray<System::Word, 2> DrawTextWordWrap;
extern DELPHI_PACKAGE TRzBlinkingControlsList* BlinkingControls;
static const int xpHotTrackColor = int(0x149bf0);
static const int xpRadChkMarkColor = int(0x21a121);
static const int xpRadChkFrameColor = int(0x80511c);
static const int xpButtonFrameColor = int(0x743c00);
static const int xpButtonFaceColor = int(0xf0f4f4);
static const int xpEditFrameColor = int(0xb99d7f);
static const int xpTabControlFrameColor = int(0x9c9b91);
static const int xpTabControlColor = int(0xf5f6f7);
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_Selection_FrameColor;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_Selection_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_Selection_ColorStop;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_Panel_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_Panel_ColorStop;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_Panel_FrameColor;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_GroupBar_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeBlue_GroupBar_ColorStop;
static const int xpOfficeBlue_CategoryGroup_CaptionBackColor = int(16777215);
static const int xpOfficeBlue_CategoryGroup_CaptionBackColorStart = int(16777215);
static const int xpOfficeBlue_CategoryGroup_CaptionBackColorStop = int(0xf7d3c6);
static const int xpOfficeBlue_CategoryGroup_CaptionFontColor = int(0xa53c00);
static const int xpOfficeBlue_CategoryGroup_CaptionFontHotColor = int(0xff8e42);
static const int xpOfficeBlue_CategoryGroup_CaptionButtonColor = int(0xfcfcfc);
static const int xpOfficeBlue_CategoryGroup_CaptionButtonBorderColor = int(0xd9bab3);
static const int xpOfficeBlue_CategoryGroup_CaptionDividerColor = int(0xc56a31);
static const int xpOfficeBlue_CategoryGroup_GroupColor = int(0xf7dfd6);
static const int xpOfficeBlue_CategoryGroup_GroupBorderColor = int(16777215);
static const int xpOfficeBlue_CategoryGroup_ItemFontColor = int(0xc65d21);
static const int xpOfficeBlue_CategoryGroup_ItemFontHotColor = int(0xff8e42);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionBackColor = int(0xc56a31);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionBackColorStart = int(0xbe500f);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionBackColorStop = int(0xce5d29);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionFontColor = int(16777215);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionFontHotColor = int(0xf7dfd6);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionButtonColor = int(0xc35e2f);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionButtonBorderColor = int(0xde9a6a);
static const int xpOfficeBlue_CategoryGroupSpecial_CaptionDividerColor = int(0xc56a31);
static const int xpOfficeBlue_CategoryGroupSpecial_GroupColor = int(0xfff3ef);
static const int xpOfficeBlue_CategoryGroupSpecial_GroupBorderColor = int(16777215);
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_Selection_FrameColor;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_Selection_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_Selection_ColorStop;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_Panel_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_Panel_ColorStop;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_Panel_FrameColor;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_GroupBar_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeGreen_GroupBar_ColorStop;
static const int xpOfficeGreen_CategoryGroup_CaptionBackColor = int(0xb8e7e0);
static const int xpOfficeGreen_CategoryGroup_CaptionBackColorStart = int(0xd5f3f1);
static const int xpOfficeGreen_CategoryGroup_CaptionBackColorStop = int(0xb8e7e0);
static const int xpOfficeGreen_CategoryGroup_CaptionFontColor = int(0x1c674b);
static const int xpOfficeGreen_CategoryGroup_CaptionFontHotColor = int(0x1d9272);
static const int xpOfficeGreen_CategoryGroup_CaptionButtonColor = int(0xfcfcfc);
static const int xpOfficeGreen_CategoryGroup_CaptionButtonBorderColor = int(0xbed5ca);
static const int xpOfficeGreen_CategoryGroup_CaptionDividerColor = int(0x70a093);
static const int xpOfficeGreen_CategoryGroup_GroupColor = int(0xecf6f6);
static const int xpOfficeGreen_CategoryGroup_GroupBorderColor = int(16777215);
static const int xpOfficeGreen_CategoryGroup_ItemFontColor = int(0x2d6656);
static const int xpOfficeGreen_CategoryGroup_ItemFontHotColor = int(0x1d9272);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionBackColor = int(0x70a093);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionBackColorStart = int(0x4e9682);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionBackColorStop = int(0x67a896);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionFontColor = int(16777215);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionFontHotColor = int(0xa9d6c8);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionButtonColor = int(0x4fa382);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionButtonBorderColor = int(0x81b7a9);
static const int xpOfficeGreen_CategoryGroupSpecial_CaptionDividerColor = int(0x70a093);
static const int xpOfficeGreen_CategoryGroupSpecial_GroupColor = int(0xecf6f6);
static const int xpOfficeGreen_CategoryGroupSpecial_GroupBorderColor = int(16777215);
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_Selection_FrameColor;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_Selection_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_Selection_ColorStop;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_Panel_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_Panel_ColorStop;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_Panel_FrameColor;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_GroupBar_ColorStart;
extern DELPHI_PACKAGE System::Uitypes::TColor xpOfficeSilver_GroupBar_ColorStop;
static const int xpOfficeSilver_CategoryGroup_CaptionBackColor = int(0xe0d7d6);
static const int xpOfficeSilver_CategoryGroup_CaptionBackColorStart = int(0xfcfbfb);
static const int xpOfficeSilver_CategoryGroup_CaptionBackColorStop = int(0xe0d7d6);
static const int xpOfficeSilver_CategoryGroup_CaptionFontColor = int(0x3d3d3f);
static const int xpOfficeSilver_CategoryGroup_CaptionFontHotColor = int(0x7c7c7e);
static const int xpOfficeSilver_CategoryGroup_CaptionButtonColor = int(0xfcfcfc);
static const int xpOfficeSilver_CategoryGroup_CaptionButtonBorderColor = int(0xd1c5c3);
static const int xpOfficeSilver_CategoryGroup_CaptionDividerColor = int(0xbfb4b2);
static const int xpOfficeSilver_CategoryGroup_GroupColor = int(0xf5f1f0);
static const int xpOfficeSilver_CategoryGroup_GroupBorderColor = int(16777215);
static const int xpOfficeSilver_CategoryGroup_ItemFontColor = int(0x3d3d3f);
static const int xpOfficeSilver_CategoryGroup_ItemFontHotColor = int(0x7c7c7e);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionBackColor = int(0xab9594);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionBackColorStart = int(0xa58d8c);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionBackColorStop = int(0xc7b6b4);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionFontColor = int(16777215);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionFontHotColor = int(0xe0d7d6);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionButtonColor = int(0x96746e);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionButtonBorderColor = int(0xdec7c1);
static const int xpOfficeSilver_CategoryGroupSpecial_CaptionDividerColor = int(0xbfb4b2);
static const int xpOfficeSilver_CategoryGroupSpecial_GroupColor = int(0xf5f1f0);
static const int xpOfficeSilver_CategoryGroupSpecial_GroupBorderColor = int(16777215);
extern DELPHI_PACKAGE TRzXPColorScheme CurrentXPColorScheme;
extern DELPHI_PACKAGE bool FullColorSupported;
extern DELPHI_PACKAGE int IncrementalSearchResetDelay;
#define fccAll (System::Set<TRzFrameControllerProperty, TRzFrameControllerProperty::fcpColor, TRzFrameControllerProperty::fcpAll>() << TRzFrameControllerProperty::fcpColor << TRzFrameControllerProperty::fcpFocusColor << TRzFrameControllerProperty::fcpDisabledColor << TRzFrameControllerProperty::fcpReadOnlyColor << TRzFrameControllerProperty::fcpReadOnlyColorOnFocus << TRzFrameControllerProperty::fcpParentColor << TRzFrameControllerProperty::fcpFlatButtonColor << TRzFrameControllerProperty::fcpFlatButtons << TRzFrameControllerProperty::fcpFrameColor << TRzFrameControllerProperty::fcpFrameHotColor << TRzFrameControllerProperty::fcpFrameHotTrack << TRzFrameControllerProperty::fcpFrameHotStyle << TRzFrameControllerProperty::fcpFrameSides << TRzFrameControllerProperty::fcpFrameStyle \
	<< TRzFrameControllerProperty::fcpFrameVisible << TRzFrameControllerProperty::fcpFramingPreference )
static const System::Int8 fcpColorBit = System::Int8(0x1);
static const System::Int8 fcpFocusColorBit = System::Int8(0x2);
static const System::Int8 fcpDisabledColorBit = System::Int8(0x4);
static const System::Int8 fcpReadOnlyColorBit = System::Int8(0x8);
static const System::Int8 fcpReadOnlyColorOnFocusBit = System::Int8(0x10);
static const System::Int8 fcpParentColorBit = System::Int8(0x20);
static const System::Int8 fcpFlatButtonColorBit = System::Int8(0x40);
static const System::Byte fcpFlatButtonsBit = System::Byte(0x80);
static const System::Word fcpFrameColorBit = System::Word(0x100);
static const System::Word fcpFrameHotColorBit = System::Word(0x200);
static const System::Word fcpFrameHotTrackBit = System::Word(0x400);
static const System::Word fcpFrameHotStyleBit = System::Word(0x800);
static const System::Word fcpFrameSidesBit = System::Word(0x1000);
static const System::Word fcpFrameStyleBit = System::Word(0x2000);
static const System::Word fcpFrameVisibleBit = System::Word(0x4000);
static const System::Word fcpFramingPreferenceBit = System::Word(0x8000);
extern DELPHI_PACKAGE System::ResourceString _sRzNoRegIniFile;
#define Rzcommon_sRzNoRegIniFile System::LoadResourceString(&Rzcommon::_sRzNoRegIniFile)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotLoadCustomFraming;
#define Rzcommon_sRzCannotLoadCustomFraming System::LoadResourceString(&Rzcommon::_sRzCannotLoadCustomFraming)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotSaveCustomFraming;
#define Rzcommon_sRzCannotSaveCustomFraming System::LoadResourceString(&Rzcommon::_sRzCannotSaveCustomFraming)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotLoadCustomColors;
#define Rzcommon_sRzCannotLoadCustomColors System::LoadResourceString(&Rzcommon::_sRzCannotLoadCustomColors)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotSaveCustomColors;
#define Rzcommon_sRzCannotSaveCustomColors System::LoadResourceString(&Rzcommon::_sRzCannotSaveCustomColors)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotLoadProperties;
#define Rzcommon_sRzCannotLoadProperties System::LoadResourceString(&Rzcommon::_sRzCannotLoadProperties)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotSaveProperties;
#define Rzcommon_sRzCannotSaveProperties System::LoadResourceString(&Rzcommon::_sRzCannotSaveProperties)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotRestoreFormState;
#define Rzcommon_sRzCannotRestoreFormState System::LoadResourceString(&Rzcommon::_sRzCannotRestoreFormState)
extern DELPHI_PACKAGE System::ResourceString _sRzCannotSaveFormState;
#define Rzcommon_sRzCannotSaveFormState System::LoadResourceString(&Rzcommon::_sRzCannotSaveFormState)
extern DELPHI_PACKAGE void __fastcall FreeBlinkingControlsListIfEmpty(void);
extern DELPHI_PACKAGE System::Types::TRect __fastcall CenterRect(const System::Types::TRect &R, int Width, int Height);
extern DELPHI_PACKAGE void __fastcall ColorToHSL(System::Uitypes::TColor C, System::Byte &H, System::Byte &S, System::Byte &L);
extern DELPHI_PACKAGE System::Byte __fastcall ColorHue(System::Uitypes::TColor C);
extern DELPHI_PACKAGE System::Byte __fastcall ColorSaturation(System::Uitypes::TColor C);
extern DELPHI_PACKAGE System::Byte __fastcall ColorLuminance(System::Uitypes::TColor C);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall HSLtoColor(System::Byte H, System::Byte S, System::Byte L);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall DarkerColor(System::Uitypes::TColor C, System::Byte Adjustment);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall LighterColor(System::Uitypes::TColor C, System::Byte Adjustment);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall AdjustColor(System::Uitypes::TColor C, int Adjustment);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall BlendColors(System::Uitypes::TColor ForeColor, System::Uitypes::TColor BackColor, System::Byte Alpha);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall XorColors(System::Uitypes::TColor ForeColor, System::Uitypes::TColor BackColor);
extern DELPHI_PACKAGE bool __fastcall ColorsTooClose(System::Uitypes::TColor ForeColor, System::Uitypes::TColor BackColor);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall ComplementaryColor(System::Uitypes::TColor C)/* overload */;
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall ComplementaryColor(System::Uitypes::TColor C, System::Byte Luminance)/* overload */;
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall GetXPThemeColor(TRzXPThemeColor Element);
extern DELPHI_PACKAGE TRzXPColorScheme __fastcall GetXPColorScheme(void);
extern DELPHI_PACKAGE void __fastcall GetGradientSelectionColors(TRzGradientColorStyle ColorStyle, System::Uitypes::TColor &FrameColor, System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
extern DELPHI_PACKAGE void __fastcall GetGradientPanelColors(TRzGradientColorStyle ColorStyle, System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall GetGradientPanelFrameColor(TRzGradientColorStyle ColorStyle);
extern DELPHI_PACKAGE void __fastcall GetGradientStatusBarColors(TRzGradientColorStyle ColorStyle, System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor, System::Uitypes::TColor &DividerColor);
extern DELPHI_PACKAGE void __fastcall GetGradientGroupBarColors(TRzGradientColorStyle ColorStyle, System::Uitypes::TColor &StartColor, System::Uitypes::TColor &StopColor);
extern DELPHI_PACKAGE void __fastcall GetGradientCategoryGroupColors(TRzGradientColorStyle ColorStyle, bool SpecialGroup, System::Uitypes::TColor &CaptionBackColor, System::Uitypes::TColor &CaptionBackColorStart, System::Uitypes::TColor &CaptionBackColorStop, System::Uitypes::TColor &CaptionFontColor, System::Uitypes::TColor &CaptionFontHotColor, System::Uitypes::TColor &CaptionButtonColor, System::Uitypes::TColor &CaptionButtonBorderColor, System::Uitypes::TColor &CaptionDividerColor, System::Uitypes::TColor &GroupColor, System::Uitypes::TColor &GroupBorderColor);
extern DELPHI_PACKAGE void __fastcall GetGradientOutlookGroupColors(TRzGradientColorStyle ColorStyle, System::Uitypes::TColor &CaptionBackStartColor, System::Uitypes::TColor &CaptionBackStopColor, System::Uitypes::TColor &CaptionFontColor, System::Uitypes::TColor &CaptionFontHotColor, System::Uitypes::TColor &GroupStartColor, System::Uitypes::TColor &GroupStopColor);
extern DELPHI_PACKAGE void __fastcall GetGradientGroupItemColors(TRzGradientColorStyle ColorStyle, System::Uitypes::TColor &ItemFontColor, System::Uitypes::TColor &ItemFontHotColor);
extern DELPHI_PACKAGE Vcl::Themes::TCustomStyleServices* __fastcall ActiveStyleServices(void);
extern DELPHI_PACKAGE bool __fastcall ActiveStyleServicesEnabled(void);
extern DELPHI_PACKAGE bool __fastcall ActiveStyleServicesAvailable(void);
extern DELPHI_PACKAGE void __fastcall ActiveStyleServicesDrawText(HDC DC, const Vcl::Themes::TThemedElementDetails &Details, const System::UnicodeString S, const System::Types::TRect &R, unsigned Flags);
extern DELPHI_PACKAGE bool __fastcall UsingSystemStyle(void);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall ActiveStyleSystemColor(System::Uitypes::TColor C);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall ActiveStyleColor(Vcl::Themes::TStyleColor SC);
extern DELPHI_PACKAGE System::Uitypes::TColor __fastcall ActiveStyleFontColor(Vcl::Themes::TStyleFont SF);
extern DELPHI_PACKAGE bool __fastcall IsFullColorSupported(void);
extern DELPHI_PACKAGE void __fastcall DrawDropShadow(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, int Depth, System::Uitypes::TColor ShadowColor = (System::Uitypes::TColor)(0x0));
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawSides(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor ULColor, System::Uitypes::TColor LRColor, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawBevel(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor ULColor, System::Uitypes::TColor LRColor, int Width, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawCtl3DBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, bool Lowered);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawCtl3DBorderSides(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, bool Lowered, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawButtonBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, bool Lowered);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawButtonBorderSides(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, bool Lowered, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawColorButtonBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor FaceColor, bool Lowered);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawColorButtonBorderSides(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor FaceColor, bool Lowered, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TFrameStyle Style);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawBorderSides(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TFrameStyle Style, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawColorBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor FaceColor, TFrameStyle Style);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawColorBorderSides(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor FaceColor, TFrameStyle Style, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawBox(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor LineColor);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawFilledBox(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor LineColor, System::Uitypes::TColor FillColor);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawBoxCorners(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor LineColor, int CornerWidth);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawDashedBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor DashColor);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawFocusBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor Color = (System::Uitypes::TColor)(0x1fffffff));
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawRoundedFlatBorder(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor Color, TSides Sides);
extern DELPHI_PACKAGE System::Types::TRect __fastcall DrawInnerOuterBorders(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TFrameStyleEx BorderOuter, TFrameStyleEx BorderInner, int BorderWidth, TSides BorderSides, int BevelWidth, System::Uitypes::TColor BorderColor, System::Uitypes::TColor BorderHighlight, System::Uitypes::TColor BorderShadow, System::Uitypes::TColor FlatColor, int FlatColorAdjustment, System::Uitypes::TColor Color, System::Uitypes::TColor ParentColor, bool Transparent, bool SoftInnerFlatBorder = false);
extern DELPHI_PACKAGE void __fastcall DrawGroupBarBackground(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TRzVisualStyle VisualStyle, TRzGradientColorStyle ColorStyle, TRzGroupBarGradientPath GradientPath, System::Uitypes::TColor CustomStartColor, System::Uitypes::TColor CustomStopColor);
extern DELPHI_PACKAGE void __fastcall DrawLEDBar(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TOrientation Orientation, System::Uitypes::TColor BarColor, System::Uitypes::TColor BackColor, int NumSegments, int Percent, bool ThemeAware, bool Transparent);
extern DELPHI_PACKAGE void __fastcall DrawPercentBar(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TOrientation Orientation, System::Uitypes::TColor BarColor, System::Uitypes::TColor BackColor, System::Word Percent, bool ShowPercent, bool Transparent, bool ShowParts = false, int PartsComplete = 0x0, int TotalParts = 0x0);
extern DELPHI_PACKAGE void __fastcall DrawGradientPercentBar(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TOrientation Orientation, System::Uitypes::TColor BarColor, System::Uitypes::TColor BarColorStop, System::Uitypes::TColor BackColor, System::Uitypes::TColor BackColorStop, TGradientDirection gradientDirection, System::Word Percent, bool ShowPercent, bool Transparent, bool ShowParts = false, int PartsComplete = 0x0, int TotalParts = 0x0);
extern DELPHI_PACKAGE void __fastcall DrawFrame(Vcl::Graphics::TCanvas* Canvas, int Width, int Height, TFrameStyle FrameStyle, System::Uitypes::TColor EraseColor, System::Uitypes::TColor FrameColor, TSides FrameSides, bool Transparent = false);
extern DELPHI_PACKAGE void __fastcall InvalidateWindowFrame(HWND Handle, const System::Types::TRect &Bounds);
extern DELPHI_PACKAGE void __fastcall InvalidateControls(Vcl::Controls::TWinControl* Container);
extern DELPHI_PACKAGE void __fastcall DrawDropDownArrow(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TRzUIStyle UIStyle, bool Down, bool Enabled = true);
extern DELPHI_PACKAGE void __fastcall DrawSpinArrow(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor Color, System::Uitypes::TColor DisabledColor, TDirection Direction, bool Down, bool Enabled = true)/* overload */;
extern DELPHI_PACKAGE void __fastcall DrawSpinArrow(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TRzUIStyle UIStyle, TDirection Direction, bool Down, bool Enabled = true)/* overload */;
extern DELPHI_PACKAGE void __fastcall DrawCloseX(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, System::Uitypes::TColor Color, bool Down, bool Enabled = true)/* overload */;
extern DELPHI_PACKAGE void __fastcall DrawCloseX(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TRzUIStyle UIStyle, bool Down, bool Enabled = true)/* overload */;
extern DELPHI_PACKAGE void __fastcall DrawHighlightBox(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, TGradientDirection GradientDirection, System::Uitypes::TColor StartColor, System::Uitypes::TColor StopColor);
extern DELPHI_PACKAGE void __fastcall DrawCheckBox(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, Vcl::Stdctrls::TCheckBoxState CheckState, TRzButtonDisplayState DisplayState, bool Focused, TRzButtonHotTrackStyle HotTrackStyle, System::Uitypes::TColor FrameColor, System::Uitypes::TColor MarkColor, System::Uitypes::TColor FillColor, System::Uitypes::TColor FocusColor, System::Uitypes::TColor DisabledColor, System::Uitypes::TColor HotTrackStartColor, System::Uitypes::TColor HotTrackStopColor, bool ReadOnly, bool ReadOnlyColorOnFocus, System::Uitypes::TColor ReadOnlyColor);
extern DELPHI_PACKAGE void __fastcall DrawRadioButton(Vcl::Graphics::TCanvas* Canvas, const System::Types::TRect &Bounds, bool Checked, TRzButtonDisplayState DisplayState, bool Focused, TRzButtonHotTrackStyle HotTrackStyle, System::Uitypes::TColor FrameColor, System::Uitypes::TColor MarkColor, System::Uitypes::TColor FillColor, System::Uitypes::TColor FocusColor, System::Uitypes::TColor DisabledColor, System::Uitypes::TColor HotTrackStartColor, System::Uitypes::TColor HotTrackStopColor, System::Uitypes::TColor BackgroundColor, bool Transparent, System::Uitypes::TColor TransparentColor, bool ReadOnly, bool ReadOnlyColorOnFocus, System::Uitypes::TColor ReadOnlyColor);
extern DELPHI_PACKAGE void __fastcall AddImageToImageList(Vcl::Imglist::TCustomImageList* ImageList, Vcl::Graphics::TBitmap* Glyph, bool AddDisabled, int &ImageIndex, int &DisabledIndex);
extern DELPHI_PACKAGE System::UnicodeString __fastcall GetNewComponentName(System::Classes::TComponent* AOwner, const System::UnicodeString BaseName, bool TryNoIndex = true);
extern DELPHI_PACKAGE System::UnicodeString __fastcall CreateValidIdent(const System::UnicodeString Ident, const System::UnicodeString DefaultName);
extern DELPHI_PACKAGE HFONT __fastcall RotateFont(Vcl::Graphics::TFont* Font, int Angle);
extern DELPHI_PACKAGE bool __fastcall IsTrueTypeFont(Vcl::Graphics::TFont* Font);
extern DELPHI_PACKAGE int __fastcall GetMinFontHeight(Vcl::Graphics::TFont* Font);
extern DELPHI_PACKAGE int __fastcall GetAvgCharWidth(Vcl::Graphics::TFont* Font);
extern DELPHI_PACKAGE System::WideChar __fastcall FirstNonWhitespaceChar(const System::UnicodeString S);
extern DELPHI_PACKAGE System::WideChar __fastcall LastChar(const System::UnicodeString S);
extern DELPHI_PACKAGE int __fastcall CountChar(System::WideChar C, const System::UnicodeString S);
extern DELPHI_PACKAGE System::UnicodeString __fastcall CopyEx(const System::UnicodeString S, int Start, System::WideChar C, int Count);
extern DELPHI_PACKAGE bool __fastcall RemoveChar(System::UnicodeString &S, System::WideChar C);
extern DELPHI_PACKAGE System::UnicodeString __fastcall RemoveAccelerators(const System::UnicodeString S);
extern DELPHI_PACKAGE System::UnicodeString __fastcall ExpandEnvironmentVariables(const System::UnicodeString S);
extern DELPHI_PACKAGE int __fastcall Min(int A, int B);
extern DELPHI_PACKAGE int __fastcall Max(int A, int B);
extern DELPHI_PACKAGE void __fastcall Swap(int &A, int &B)/* overload */;
extern DELPHI_PACKAGE void __fastcall Swap(System::Word &A, System::Word &B)/* overload */;
extern DELPHI_PACKAGE void __fastcall UpdateObjectInspector(Vcl::Controls::TControl* AControl);
extern DELPHI_PACKAGE System::Types::TRect __fastcall GetDesktopClientRect(void);
extern DELPHI_PACKAGE System::Types::TRect __fastcall GetActiveWorkArea(Vcl::Controls::TWinControl* Window);
extern DELPHI_PACKAGE int __fastcall GetActiveWorkAreaWidth(Vcl::Controls::TWinControl* Window);
extern DELPHI_PACKAGE int __fastcall GetActiveWorkAreaHeight(Vcl::Controls::TWinControl* Window);
extern DELPHI_PACKAGE System::Types::TRect __fastcall GetMonitorWorkArea(Vcl::Forms::TMonitor* Monitor);
extern DELPHI_PACKAGE System::Types::TRect __fastcall GetMonitorBoundsRect(Vcl::Forms::TMonitor* Monitor);
extern DELPHI_PACKAGE Vcl::Forms::TMonitor* __fastcall GetMonitorContainingPoint(const System::Types::TPoint &P);
extern DELPHI_PACKAGE void __fastcall CenterToWindow(Vcl::Controls::TWinControl* ChildWin, Vcl::Controls::TWinControl* Window, int &Left, int &Top);
extern DELPHI_PACKAGE void __fastcall CenterToForm(Vcl::Controls::TWinControl* ChildWin, Vcl::Forms::TCustomForm* AForm, int &Left, int &Top);
extern DELPHI_PACKAGE void __fastcall CenterToMDIChild(Vcl::Controls::TWinControl* ChildWin, Vcl::Forms::TForm* AForm, int &Left, int &Top);
extern DELPHI_PACKAGE bool __fastcall RunningUnder(TRzWindowsVersion ver);
extern DELPHI_PACKAGE bool __fastcall RunningAtLeast(TRzWindowsVersion ver);
extern DELPHI_PACKAGE int __fastcall DrawString(Vcl::Graphics::TCanvas* Canvas, const System::UnicodeString S, System::Types::TRect &Bounds, unsigned Flags);
extern DELPHI_PACKAGE void __fastcall DrawStringCentered(Vcl::Graphics::TCanvas* Canvas, const System::UnicodeString S, const System::Types::TRect &Bounds);
}	/* namespace Rzcommon */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZCOMMON)
using namespace Rzcommon;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzcommonHPP
