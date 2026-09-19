unit CheckProcessModules;

interface
uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Registry,
  tlHelp32,
  Dialogs,
  ShlObj,
  HashList,
  MD5Util,
  CheckCrc,
  FastStrings,
  EncryptUnit;

function GetSpecialFolderDir(mFolder:Integer):string;
function CheckProcessModule(const sMoudle:string):Boolean;
var
  ServerModuleMD5List:THashList;
  BlackModuleMD5List:THashList; // 黑名单 MD5
  BlackModuleList:THashList; // 黑名单
  AddModuleMD5List:THashList;
const
  // {$INCLUDE SystemDllNameFile.txt}
  _CSIDL_DESKTOP = $0000; // <desktop>

  _CSIDL_INTERNET = $0001; // Internet Explorer (icon on desktop)

  _CSIDL_PROGRAMS = $0002; // Start Menu\Programs

  _CSIDL_CONTROLS = $0003; // My Computer\Control Panel

  _CSIDL_PRINTERS = $0004; // My Computer\Printers

  _CSIDL_PERSONAL = $0005; // My Documents

  _CSIDL_FAVORITES = $0006; // <user name>\Favorites

  _CSIDL_STARTUP = $0007; // Start Menu\Programs\Startup

  _CSIDL_RECENT = $0008; // <user name>\Recent

  _CSIDL_SENDTO = $0009; // <user name>\SendTo

  _CSIDL_BITBUCKET = $000A; // <desktop>\Recycle Bin

  _CSIDL_STARTMENU = $000B; // <user name>\Start Menu

  _CSIDL_MYDOCUMENTS = _CSIDL_PERSONAL; // Personal was just a silly name for My Documents

  _CSIDL_MYMUSIC = $000D; // "My Music" folder

  _CSIDL_MYVIDEO = $000E; // "My Videos" folder

  _CSIDL_DESKTOPDIRECTORY = $0010; // <user name>\Desktop

  _CSIDL_DRIVES = $0011; // My Computer

  _CSIDL_NETWORK = $0012; // Network Neighborhood (My Network Places)

  _CSIDL_NETHOOD = $0013; // <user name>\nethood

  _CSIDL_FONTS = $0014; // windows\fonts

  _CSIDL_TEMPLATES = $0015;

  _CSIDL_COMMON_STARTMENU = $0016; // All Users\Start Menu

  _CSIDL_COMMON_PROGRAMS = $0017; // All Users\Start Menu\Programs

  _CSIDL_COMMON_STARTUP = $0018; // All Users\Startup

  _CSIDL_COMMON_DESKTOPDIRECTORY = $0019; // All Users\Desktop

  _CSIDL_APPDATA = $001A; // <user name>\Application Data

  _CSIDL_PRINTHOOD = $001B; // <user name>\PrintHood

  _CSIDL_LOCAL_APPDATA = $001C; // <user name>\Local Settings\Applicaiton Data (non roaming)

  _CSIDL_ALTSTARTUP = $001D; // non localized startup

  _CSIDL_COMMON_ALTSTARTUP = $001E; // non localized common startup

  _CSIDL_COMMON_FAVORITES = $001F;

  _CSIDL_INTERNET_CACHE = $0020;

  _CSIDL_COOKIES = $0021;

  _CSIDL_HISTORY = $0022;

  _CSIDL_COMMON_APPDATA = $0023; // All Users\Application Data

  _CSIDL_WINDOWS = $0024; // GetWindowsDirectory()

  _CSIDL_SYSTEM = $0025; // GetSystemDirectory()

  _CSIDL_PROGRAM_FILES = $0026; // C:\Program Files

  _CSIDL_MYPICTURES = $0027; // C:\Program Files\My Pictures

  _CSIDL_PROFILE = $0028; // USERPROFILE

  _CSIDL_SYSTEMX86 = $0029; // x86 system directory on RISC

  _CSIDL_PROGRAM_FILESX86 = $002A; // x86 C:\Program Files on RISC

  _CSIDL_PROGRAM_FILES_COMMON = $002B; // C:\Program Files\Common

  _CSIDL_PROGRAM_FILES_COMMONX86 = $002C; // x86 Program Files\Common on RISC

  _CSIDL_COMMON_TEMPLATES = $002D; // All Users\Templates

  _CSIDL_COMMON_DOCUMENTS = $002E; // All Users\Documents

  _CSIDL_COMMON_ADMINTOOLS = $002F; // All Users\Start Menu\Programs\Administrative Tools

  _CSIDL_ADMINTOOLS = $0030; // <user name>\Start Menu\Programs\Administrative Tools

  _CSIDL_CONNECTIONS = $0031; // Network and Dial-up Connections

  _CSIDL_COMMON_MUSIC = $0035; // All Users\My Music

  _CSIDL_COMMON_PICTURES = $0036; // All Users\My Pictures

  _CSIDL_COMMON_VIDEO = $0037; // All Users\My Video

  _CSIDL_RESOURCES = $0038; // Resource Direcotry

  _CSIDL_RESOURCES_LOCALIZED = $0039; // Localized Resource Direcotry

  _CSIDL_COMMON_OEM_LINKS = $003A; // Links to All Users OEM specific apps

  _CSIDL_CDBURN_AREA = $003B; // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning

  // unused                               0x003c
  _CSIDL_COMPUTERSNEARME = $003D; // Computers Near Me (computered from Workgroup membership)

  _CSIDL_FLAG_CREATE = $8000; // combine with _CSIDL_ value to force folder creation in SHGetFolderPath()

  _CSIDL_FLAG_DONT_VERIFY = $4000; // combine with _CSIDL_ value to return an unverified folder path

  _CSIDL_FLAG_DONT_UNEXPAND = $2000; // combine with _CSIDL_ value to avoid unexpanding environment variables

  _CSIDL_FLAG_NO_ALIAS = $1000; // combine with _CSIDL_ value to insure non-alias versions of the pidl

  _CSIDL_FLAG_PER_USER_INIT = $0800; // combine with _CSIDL_ value to indicate per-user init (eg. upgrade)

  _CSIDL_FLAG_MASK = $FF00; // mask for all possible flag values

implementation
uses ZLibEx;
var
  SystemDllList:THashList;
  ModulePathList:THashList;
  ModuleMD5List:THashList;
  ModuleFileList:THashList;
  UnKnowModuleFileList:THashList;

  SysWOW64:string = '';

  CopyrightArray:array[0..59] of string = (
    'cQHSXsW?PoUSQfMRTsbJ', // 'Microsoft', //微软
    'ZpVaIgHPDgXBaaXcUekY', // 'Sogou.com', //搜狗
    'OwFri=XQYnNbU]UopyK@', // 'Tencent', //腾讯
    'UnUpmRZbeJJPqaYopytb', // 'Thunder', //迅雷
    'veW@ydP_MRNpHsa', // '360.cn', //360
    'pPOa]sZaUCWraTMp@yVi', // 'Kingsoft', //金山
    'gaUBQrLoAfZ@ESOSTnwN', // 'Kaspersky', //卡巴
    'czHbEmYPmPYCH<L', // 'Rising', //瑞星
    'mIWbqmZ^yLI`IjQ?<yW>', // 'Symantec', //诺顿
    'xCJ?=NRQduWny]QOIdFsXyKLxw', // 'Micropoint', //微点
    'WCFr]aXrUVQPpqMSXyeg', // 'Jiangmin',  //江民
    'XtIa=RZcdpTopLk', // 'Baidu',百度
    'GTU@icUsAcKOpxv', // '百度'
    'kaUsdoNPAqNOpdO', // 'Apple', //苹果
    'kuW_PlTPyIPBabMOLyAc', // 'Broadcom',
    'y[PCa@ZaQCJ?plG', // 'AVAST',
    'ZiYbQ>JSYpYPPHk', // 'McAfee',
    'VINSajUbuMKOpvU', // 'ESET',
    'iRRA]KI@qNHQQEUQMoGrQ=KLZA', // 'Trend Micro', //趋势科技
    '@lMqerIp]lMRpkYopyxm', // 'Lingoes', //灵格斯词霸
    'uPJ?USLReINB]GQsDkX_]fYOQqPBduUopyjZ', // 'Jean-loup Gailly' Jean-loup Gailly & Mark Adler
    'rmTRMqZcAcV@hOz', // NVIDIA
    'PSPS=aTc]dJ@EvN?YdI@yLUoUKTc@kLOpywT', // ATI Technologies
    'TxI@Q_WBEuVa]nRRxyxb', // Advanced
    'NtOs=RNsErIRApNBtpWrQvUoMcKOpgR', // www.52hxw.com
    'nnIbMGYCAlZ_EVPOpysW', // Feitian
    'C>YPygXRiSZBpVZ', // Andrea
    'epWBxnXrMnNOpKh', // Adobe
    'JrGrygLQ]^YpYgPOpyI<', // Brother
    'WNTNypYbaMKOpHj', // DTS.
    'dWWQXoLo=DRcaLJ@miIrXyKL=G', // Fortemedia
    'CBVcaSW_=qWqXUG', // Radius
    'ciY@a]N`dpGq=TLOXuI`ynJLqh', // Heidelberger
    'BAPs<gIRa]OQXrTNxuHpxsX?UmNCHpOp`yHI', // www.EnterSafe.com
    'hOWcUsMA=gMOpLU', // Intel
    'OZIaaIGrenY_=LYopy[I', // Knowles
    'L<XPiKHSQQJ?p=C', // Waves
    'fPIoYKJButTpLuPOpygW', // Infosec
    'CaOAesHS]MKOpUa', // Real
    'cpQCIoZBQ=ZRILUrupIA<oYNhgM?`qYSDtIF', // Dolby Laboratories
    'ErZaacQbUtVBlkYopyRl', // Realtek
    'bnRbAFRBtmRQYoM@@yQa', // Synopsys
    'gnMC]NTaEsKOpnj', // Sony
    'OKMryVTOY^PSTlMo<yjZ', // SRS Labs
    'eGWrMiVoLrTsEjP?HtCw', // Synaptics
    'mHPQ]]IrAuZA]cPOpyjT', // TOSHIBA
    'ujW_=vYbPuRRxix', // VMware
    'UPN_PrXaekYqEBPOpyoa', // GameCap
    'ASTbyoIbYfFoL[k', // Lenovo
    'KeM@MQNoIqNpusTbuoFnxoUoMaTcHQH', // Microelectronic
    'dRNRyaJOAhJ@HoOp@yxO', // nianqing
    'fjQoLoQpEMKOpsC', // 念青
    'WXMAetIbaVPO@ef', // '新浪网',
    'fdYQXqUceSNPtms', // '易语言',
    'YAROYQPoAfJPxrn', // '奇虎网',
    'wHMpaBPRqsKOpKP', // '巨盾'
    'XHURhkR_YsKOpt<', // '极点五笔'
    'QLOsaDXC]>PC]mQPPyXh', // '恒信科技'
    'BQHbdsR@AsWSA?NBXyyp', // 飞天诚信
    'm[TrYdWs]?IPy@Q@pyuS' // 农业银行
    );

function ReadRegKey(RegRootKey:Integer; const iMode:Integer; const sPath,
  sKeyName:string; var sResult:string):Boolean; // HKEY_LOCAL_MACHINE
var
  rRegObject:TRegistry;
begin
  rRegObject := TRegistry.Create;
  Result := False;
  try
    with rRegObject do begin
      RootKey := RegRootKey;
      if OpenKey(sPath, False) then begin
        case iMode of
          1:sResult := LowerCase(Trim(ReadString(sKeyName)));
          2:sResult := IntToStr(ReadInteger(sKeyName));
          // 3: sResult := ReadBinaryData(sKeyName, Buffer, BufSize);
        end;
        if sResult = '' then
          Result := False
        else
          Result := True;
      end;
      CloseKey;
    end;
  finally
    rRegObject.Free;
  end;
end;
// _____________________________________________________________________//

function WriteRegKey(RegRootKey:Integer; const iMode:Integer; const sPath, sKeyName,
  sKeyValue:string):Boolean;
var
  rRegObject:TRegistry;
  bData:Byte;
begin
  rRegObject := TRegistry.Create;
  try
    with rRegObject do begin
      RootKey := RegRootKey;
      if OpenKey(sPath, True) then begin
        case iMode of
          1:WriteString(sKeyName, sKeyValue);
          2:WriteInteger(sKeyName, StrToInt(sKeyValue));
          3:WriteBinaryData(sKeyName, bData, 1);
        end;
        Result := True;
      end
      else
        Result := False;
      CloseKey;
    end;
  finally
    rRegObject.Free;
  end;
end;

function GetSpecialFolderDir(mFolder:Integer):string;
{   返回获取系统文件或系统目录   ShlObj}
{
        _CSIDL_BITBUCKET                   *       回收站
        _CSIDL_CONTROLS                     *       控制面板
        _CSIDL_DESKTOP                       *       桌面
        _CSIDL_DESKTOPDIRECTORY             桌面目录               // 如C:\WINDOWS\Desktop
        _CSIDL_DRIVES                         *       我的电脑
        _CSIDL_FONTS                                   字体                       // 如C:\WINDOWS\FONTS
        _CSIDL_NETHOOD                               网上邻居目录       // 如C:\WINDOWS\NetHood
        _CSIDL_NETWORK                       *       网上邻居
        _CSIDL_PERSONAL                             我的文档               // 如C:\My   Documents
        _CSIDL_PRINTERS                     *       打印机
        _CSIDL_PROGRAMS                             程序组                   // 如C:\WINDOWS\Start   Menu\Programs
        _CSIDL_RECENT                                 最近文档               // 如C:\WINDOWS\Recent
        _CSIDL_SENDTO                                 发送到                   // 如C:\WINDOWS\SentTo
        _CSIDL_STARTMENU                           开始菜单               // 如C:\WINDOWS\Start   Menu
        _CSIDL_STARTUP                               启动                       // 如C:\WINDOWS\启动
        _CSIDL_TEMPLATES                           模版                       // 如C:\WINDOWS\ShellNew
}
var
  vItemIDList:PItemIDList;
  vBuffer:array[0..MAX_PATH] of Char;
begin
  SHGetSpecialFolderLocation(0, mFolder, vItemIDList);
  SHGetPathFromIDList(vItemIDList, vBuffer); // 转换成文件系统的路径
  Result := vBuffer;
end;

function GetFileLegalCopyright(sFileName:string):string;
const
  SFInfo = '\StringFileInfo\';
var
  VersionInfo:PChar;
  InfoSize:LongWord;
  Translation:Pointer;
  InfoPointer:Pointer;
  VersionValue:string;
begin
  Result := '';
  InfoSize := GetFileVersionInfoSize(PChar(sFileName), InfoSize);
  if InfoSize = 0 then Exit;
  VersionInfo := AllocMem(InfoSize);
  try
    if GetFileVersionInfo(PChar(sFileName), 0, InfoSize, VersionInfo) then begin
      if VerQueryValue(VersionInfo, '\VarFileInfo\Translation', Translation, InfoSize) then begin
        VersionValue := SFInfo + IntToHex(LoWord(Longint(Translation^)), 4) +
          IntToHex(HiWord(Longint(Translation^)), 4) + '\';
        if VerQueryValue(VersionInfo, PChar(VersionValue + 'LegalCopyright'), InfoPointer, InfoSize) then
          Result := Trim(string(PChar(InfoPointer)));
      end;
    end;
  finally
    FreeMem(VersionInfo);
  end;
end;

procedure InitModules;
var
  I:Integer;
  // sText: string;
  sSystem32:string;
  // StringList: TStringList;
begin
  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-13】 }
  // sText := '';
  // StringList := TStringList.Create;
  SystemDllList := THashList.Create(65536);
  ModuleMD5List := THashList.Create(65536);
  ModuleFileList := THashList.Create(65536);
  UnKnowModuleFileList := THashList.Create(65536);
  ServerModuleMD5List := THashList.Create(65536);
  ModulePathList := THashList.Create(65536);

  AddModuleMD5List := THashList.Create(65536);
  BlackModuleMD5List := THashList.Create(65536);
  BlackModuleList := THashList.Create(65536);
  for I := 0 to Length(CopyrightArray) - 1 do
    CopyrightArray[I] := DecryString(CopyrightArray[I]);

  sSystem32 := IncludeTrailingPathDelimiter(LowerCase(GetSpecialFolderDir(_CSIDL_SYSTEM)));

  // C:\Windows\SysWOW64
  SysWOW64 := IncludeTrailingPathDelimiter(LowerCase(GetSpecialFolderDir(_CSIDL_SYSTEMX86))); // 64位系统  41 0029 C:\Windows\SysWOW64
  if (SysWOW64 = sSystem32) then SysWOW64 := '';
end;

function CompareLStr(Src, targ:string; compn:Integer):Boolean;
var
  I:Integer;
begin
  Result := False;
  if (compn <= 0) or (Length(Src) < compn) or (Length(targ) < compn) then Exit;
  Result := True;
  for I := 1 to compn do
    if UpCase(Src[I]) <> UpCase(targ[I]) then begin
      Result := False;
      Break;
    end;
end;

function CheckProcessModule(const sMoudle:string):Boolean;
var
  I:Integer;
  sMD5, sCopyright:string;
begin
  { if BlackModuleList.Exists(sMoudle) then begin
     Result := False;
     Exit;
   end; }

  Result := ModuleFileList.Exists(sMoudle);
  if not Result then begin
    if UnKnowModuleFileList.Exists(sMoudle) then Exit;

    sMD5 := RivestFile(sMoudle);
    if BlackModuleMD5List.Exists(sMD5) then begin
      BlackModuleList.Add(sMoudle, nil);
      Result := False;
      Exit;
    end;

    sCopyright := GetFileLegalCopyright(sMoudle);
    if sCopyright <> '' then begin
      for I := 0 to Length(CopyrightArray) - 1 do begin
        if FastPosNoCase(sCopyright, CopyrightArray[I], Length(sCopyright), Length(CopyrightArray[I]), 1) > 0 then begin
          ModuleFileList.Add(sMoudle, nil);
          Result := True;
          Exit;
        end;
      end;
    end;

    if (sMD5 <> '') and ModuleMD5List.Exists(sMD5) then begin
      ModuleFileList.Add(sMoudle, nil);
      Result := True;
    end
    else if (sMD5 <> '') and ServerModuleMD5List.Exists(sMD5) then begin
      ModuleFileList.Add(sMoudle, nil);
      Result := True;
    end
    else begin
      UnKnowModuleFileList.Add(sMoudle, nil);
    end;
  end;
end;

initialization
  InitModules;

finalization
  SystemDllList.Free;
  ModulePathList.Free;
  ModuleMD5List.Free;
  ModuleFileList.Free;
  UnKnowModuleFileList.Free;
  ServerModuleMD5List.Free;
  AddModuleMD5List.Free;
  BlackModuleMD5List.Free;
  BlackModuleList.Free;
end.
