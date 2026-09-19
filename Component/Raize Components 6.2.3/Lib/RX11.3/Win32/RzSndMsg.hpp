// CodeGear C++Builder
// Copyright (c) 1995, 2022 by Embarcadero Technologies, Inc.
// All rights reserved

// (DO NOT EDIT: machine generated header) 'RzSndMsg.pas' rev: 35.00 (Windows)

#ifndef RzsndmsgHPP
#define RzsndmsgHPP

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
#include <Winapi.Mapi.hpp>
#include <RzCommon.hpp>

//-- user supplied -----------------------------------------------------------

namespace Rzsndmsg
{
//-- forward type declarations -----------------------------------------------
class DELPHICLASS EMapiUserAbort;
class DELPHICLASS EMapiError;
class DELPHICLASS TRzSendMessage;
//-- type declarations -------------------------------------------------------
#pragma pack(push,4)
class PASCALIMPLEMENTATION EMapiUserAbort : public System::Sysutils::EAbort
{
	typedef System::Sysutils::EAbort inherited;
	
public:
	/* Exception.Create */ inline __fastcall EMapiUserAbort(const System::UnicodeString Msg) : System::Sysutils::EAbort(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMapiUserAbort(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::EAbort(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMapiUserAbort(NativeUInt Ident)/* overload */ : System::Sysutils::EAbort(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMapiUserAbort(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::EAbort(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMapiUserAbort(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::EAbort(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMapiUserAbort(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::EAbort(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMapiUserAbort(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::EAbort(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMapiUserAbort(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::EAbort(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMapiUserAbort(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::EAbort(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMapiUserAbort(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::EAbort(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMapiUserAbort(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::EAbort(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMapiUserAbort(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::EAbort(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMapiUserAbort() { }
	
};

#pragma pack(pop)

#pragma pack(push,4)
class PASCALIMPLEMENTATION EMapiError : public System::Sysutils::Exception
{
	typedef System::Sysutils::Exception inherited;
	
public:
	unsigned ErrorCode;
public:
	/* Exception.Create */ inline __fastcall EMapiError(const System::UnicodeString Msg) : System::Sysutils::Exception(Msg) { }
	/* Exception.CreateFmt */ inline __fastcall EMapiError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High) : System::Sysutils::Exception(Msg, Args, Args_High) { }
	/* Exception.CreateRes */ inline __fastcall EMapiError(NativeUInt Ident)/* overload */ : System::Sysutils::Exception(Ident) { }
	/* Exception.CreateRes */ inline __fastcall EMapiError(System::PResStringRec ResStringRec)/* overload */ : System::Sysutils::Exception(ResStringRec) { }
	/* Exception.CreateResFmt */ inline __fastcall EMapiError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High) { }
	/* Exception.CreateResFmt */ inline __fastcall EMapiError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High) { }
	/* Exception.CreateHelp */ inline __fastcall EMapiError(const System::UnicodeString Msg, int AHelpContext) : System::Sysutils::Exception(Msg, AHelpContext) { }
	/* Exception.CreateFmtHelp */ inline __fastcall EMapiError(const System::UnicodeString Msg, const System::TVarRec *Args, const int Args_High, int AHelpContext) : System::Sysutils::Exception(Msg, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMapiError(NativeUInt Ident, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, AHelpContext) { }
	/* Exception.CreateResHelp */ inline __fastcall EMapiError(System::PResStringRec ResStringRec, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMapiError(System::PResStringRec ResStringRec, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(ResStringRec, Args, Args_High, AHelpContext) { }
	/* Exception.CreateResFmtHelp */ inline __fastcall EMapiError(NativeUInt Ident, const System::TVarRec *Args, const int Args_High, int AHelpContext)/* overload */ : System::Sysutils::Exception(Ident, Args, Args_High, AHelpContext) { }
	/* Exception.Destroy */ inline __fastcall virtual ~EMapiError() { }
	
};

#pragma pack(pop)

class PASCALIMPLEMENTATION TRzSendMessage : public System::Classes::TComponent
{
	typedef System::Classes::TComponent inherited;
	
private:
	Rzcommon::TRzAboutInfo FAboutInfo;
	System::Classes::TStrings* FAttachments;
	System::Classes::TStrings* FToRecipients;
	System::Classes::TStrings* FCcRecipients;
	System::Classes::TStrings* FBccRecipients;
	Vcl::Stdctrls::TCustomMemo* FMessageMemo;
	System::Classes::TStrings* FMessageText;
	System::UnicodeString FPassword;
	System::UnicodeString FProfileName;
	bool FReview;
	NativeUInt FSession;
	System::UnicodeString FSubject;
	Vcl::Stdctrls::TCustomEdit* FSubjectEdit;
	bool FResolveNames;
	bool FRequestReceipt;
	
protected:
	virtual void __fastcall Notification(System::Classes::TComponent* AComponent, System::Classes::TOperation Operation);
	virtual void __fastcall SetAttachments(System::Classes::TStrings* Value);
	virtual void __fastcall SetToRecipients(System::Classes::TStrings* Value);
	virtual void __fastcall SetCcRecipients(System::Classes::TStrings* Value);
	virtual void __fastcall SetBccRecipients(System::Classes::TStrings* Value);
	virtual void __fastcall SetMailMessage(System::Classes::TStrings* Value);
	virtual void __fastcall SetMessageMemo(Vcl::Stdctrls::TCustomMemo* Value);
	virtual void __fastcall SetSubjectEdit(Vcl::Stdctrls::TCustomEdit* Value);
	
public:
	__fastcall virtual TRzSendMessage(System::Classes::TComponent* AOwner);
	__fastcall virtual ~TRzSendMessage();
	void __fastcall Logon();
	void __fastcall Logoff();
	void __fastcall Send();
	void __fastcall ClearLists();
	bool __fastcall MapiInstalled();
	
__published:
	__property Rzcommon::TRzAboutInfo About = {read=FAboutInfo, write=FAboutInfo, stored=false, nodefault};
	__property System::Classes::TStrings* Attachments = {read=FAttachments, write=SetAttachments};
	__property System::Classes::TStrings* CcRecipients = {read=FCcRecipients, write=SetCcRecipients};
	__property System::Classes::TStrings* BccRecipients = {read=FBccRecipients, write=SetBccRecipients};
	__property Vcl::Stdctrls::TCustomMemo* MessageMemo = {read=FMessageMemo, write=SetMessageMemo};
	__property System::Classes::TStrings* MessageText = {read=FMessageText, write=SetMailMessage};
	__property System::UnicodeString ProfileName = {read=FProfileName, write=FProfileName};
	__property System::UnicodeString Password = {read=FPassword, write=FPassword};
	__property bool Review = {read=FReview, write=FReview, default=1};
	__property bool ResolveNames = {read=FResolveNames, write=FResolveNames, default=1};
	__property bool RequestReceipt = {read=FRequestReceipt, write=FRequestReceipt, default=0};
	__property System::Classes::TStrings* ToRecipients = {read=FToRecipients, write=SetToRecipients};
	__property System::UnicodeString Subject = {read=FSubject, write=FSubject};
	__property Vcl::Stdctrls::TCustomEdit* SubjectEdit = {read=FSubjectEdit, write=SetSubjectEdit};
};


//-- var, const, procedure ---------------------------------------------------
}	/* namespace Rzsndmsg */
#if !defined(DELPHIHEADER_NO_IMPLICIT_NAMESPACE_USE) && !defined(NO_USING_NAMESPACE_RZSNDMSG)
using namespace Rzsndmsg;
#endif
#pragma pack(pop)
#pragma option pop

#pragma delphiheader end.
//-- end unit ----------------------------------------------------------------
#endif	// RzsndmsgHPP
