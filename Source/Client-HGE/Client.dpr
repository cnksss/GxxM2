program Client;


{$R 'UiRes\BasicUI.res' 'UiRes\BasicUI.rc'}

uses
  //FastMM4,
  Forms,
  Windows,
  Dialogs,
  Controls,
  Classes,
  SysUtils,
  Registry,
  DrawScrn in 'DrawScrn.pas',
  IntroScn in 'IntroScn.pas',
  PlayScn in 'PlayScn.pas',
  MapUnit in 'MapUnit.pas',
  SDK in 'SDK.pas',
  ClMain in 'ClMain.pas' {frmMain},
  ClFunc in 'ClFunc.pas',
  magiceff in 'magiceff.pas',
  SoundUtil in 'SoundUtil.pas',
  Actor in 'Actor.pas',
  HerbActor in 'HerbActor.pas',
  AxeMon in 'AxeMon.pas',
  clEvent in 'clEvent.pas',
  MShare in 'MShare.pas',
  HUtil32 in '..\Common\HUtil32.pas',
  EDcode in '..\Common\EDcode.pas',
  Grobal2 in '..\Common\Grobal2.pas',
  PathFind in 'PathFind.pas',
  DxComboBox in 'DxComponent\DxComboBox.pas',
  DxComponents in 'DxComponent\DxComponents.pas',
  DxControls in 'DxComponent\DxControls.pas',
  DxEdit in 'DxComponent\DxEdit.pas',
  DxImageButton in 'DxComponent\DxImageButton.pas',
  DxImageForm in 'DxComponent\DxImageForm.pas',
  DxImageGrid in 'DxComponent\DxImageGrid.pas',
  DxLabel in 'DxComponent\DxLabel.pas',
  DxMemo in 'DxComponent\DxMemo.pas',
  DxPageControl in 'DxComponent\DxPageControl.pas',
  DxPopupMenu in 'DxComponent\DxPopupMenu.pas',
  DxLine in 'DxComponent\DxLine.pas',
  EncryptUnit in '..\Common\EncryptUnit.pas',
  GameImages in 'ReadResources\GameImages.pas',
  Wil in 'ReadResources\Wil.pas',
  Wis in 'ReadResources\Wis.pas',
  Uib in 'ReadResources\Uib.pas',
  Pak in 'ReadResources\Pak.pas',
  Wzl in 'ReadResources\Wzl.pas',
  FState in 'GUI\Share\FState.pas',
  SerialWindowsDlg in 'GUI\Mir\SerialWindowsDlg.pas',
  MirSequelDlg in 'GUI\Mir\MirSequelDlg.pas',
  HeroWindowsDlg in 'GUI\Mir\HeroWindowsDlg.pas',
  Mir176WindowsDlg in 'GUI\Mir\Mir176WindowsDlg.pas',
  Mir185WindowsDlg in 'GUI\Mir\Mir185WindowsDlg.pas',
  StateWindows in 'GUI\NewStateWin\StateWindows.pas',
  DxCanvas in 'DxComponent\DxCanvas.pas',
  GameConfigDlgs in 'GameConfig\GameConfigDlgs.pas',
  GameConfigDlg in 'GameConfig\Common\GameConfigDlg.pas',
  MagicImageOffset in 'MagicImageOffset.pas',
  HGE in '..\..\Component\HGE FOR DELPHI7\Source\HGE.pas',
  HGECanvas in '..\..\Component\HGE FOR DELPHI7\Source\HGECanvas.pas',
  JSYConfigDlg in 'GameConfig\MirJSY\JSYConfigDlg.pas' {FrmJSYDlg},
  MirConfigDlg in 'GameConfig\Mir\MirConfigDlg.pas',
  BassSound in 'Bass\BassSound.pas',
  HGEFontEx in '..\..\Component\HGE FOR DELPHI7\Source\HGEFontEx.pas',
  HardInfo in '..\Common\HardInfo.pas',
  MD5Util in '..\Common\MD5Util.pas',
  PlugEngine in 'PlugIn\PlugEngine.pas',
  CheckProcessModules in 'CheckProcessModules.pas',
  CheckUnit in '..\..\Common\CheckUnit.pas',
  EncryptUnit_LF in '..\Common\EncryptUnit_LF.pas',
  HashList in '..\Common\HashList.pas',
  MemoryStreamEx in '..\..\Common\MemoryStreamEx.pas',
  IECache in 'IECache.pas',
  ClientBuff in 'ClientBuff.pas',
  Login in 'Login.pas' {FrmLogin},
  uWeatherEffectDef in 'uWeatherEffectDef.pas',
  ConfigShare in 'GameConfig\Common\ConfigShare.pas',
  FilterItems in 'GameConfig\Common\FilterItems.pas',
  MirNewUI205Dlg in 'GUI\Mir\MirNewUI205Dlg.pas',
  CustomActor in 'CustomActor.pas',
  Base64,
  ZlibEx,
  DxImageEdit in 'DxComponent\DxImageEdit.pas',
  WideCharList in '..\..\Component\HGE FOR DELPHI7\Source\WideCharList.pas',
  uAntiPlug in 'uAntiPlug.pas',
  DXTrackBar in 'DxComponent\DXTrackBar.pas',
  UnitDes in '..\Common\UnitDes.pas',
  DxMagicBall in 'DxComponent\DxMagicBall.pas',
  DxMainBottomForm in 'DxComponent\DxMainBottomForm.pas',
  DxSexPanel in 'DxComponent\DxSexPanel.pas',
  DxGroupAttackProgress in 'DxComponent\DxGroupAttackProgress.pas',
  DxImageProgress in 'DxComponent\DxImageProgress.pas',
  DxSwitchButton in 'DxComponent\DxSwitchButton.pas',
  uDropItemEffectList in 'uDropItemEffectList.pas',
  DesUtils in 'ReadResources\DesUtils.pas',
  BeaEngine in 'BeaEngineSource\BeaEngine.pas',
  UpdateEngine in 'UpdateEngine.pas',
  GlobalString in 'GlobalString.pas',
  AsyncCalls in 'AsyncCalls.pas',
  uExceptionStruct in 'uExceptionStruct.pas',
  DxImageButtonEx in 'DxImageButtonEx.pas',
  MsCTF in 'MsCTF.pas',
  imm in 'imm.pas',
  DropItemsMgr in 'DropItemsMgr.pas',
  uFrmNGItemEdit in 'uFrmNGItemEdit.pas' {FrmNGItemEdit},
  NPCFormDeBug in 'NPCFormDeBug.pas' {FrmNPCDeBug},
  DelphiZXIngQRCode in 'QRCode\DelphiZXIngQRCode.pas',
  LoadDxControlEx in 'LoadDxControlEx.pas',
  NearActorHintEffect in 'NearActorHintEffect.pas';

{$R *.res}

//const
  //SEMAPHORE_ALL_ACCESS = $1F0003;
  //IMAGE_FILE_LARGE_ADDRESS_AWARE = $0020;

// 突破2G内存限制 chongchong 2017-03-31
{$SetPEFlags IMAGE_FILE_LARGE_ADDRESS_AWARE}

procedure SetIeCompatibilityMode();
var
    sAppFile:string;
    reg: TRegistry;
const
    IE_8 = $1F40;    //8000, 8.0.0
    IV_8888 = $22B8; //8888, 8.8.88
    IE_9 = $2328;    //9000, 9.0.0
    IE_9999 = $270F; //9999, 9.9.99
    IE_10 = $2710;  //10000, 10.0.0
    IE_11 = $2AF8;  //11000, 11.0.0
    IeCompatibilityKey = 'SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION';
    //SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION// 64位系统实际写入位置
begin
    sAppFile := ExtractFileName(ParamStr(0));
    reg := TRegistry.Create;
    try
       reg.RootKey := HKEY_LOCAL_MACHINE;
       if reg.OpenKey(IeCompatibilityKey, False) then begin
           reg.WriteInteger(sAppFile, IE_11);
           reg.CloseKey();
       end;
    finally
       reg.Free;
    end;
end;   

function DecryString_LF2(Str: string): string;
begin
  if Str <> '' then
  begin
    Result := Base64DecodeStr(DecodeString(Str));
    DecryptDes_New(Result[1], Result[1], Length(Result), IntToStr(NewEncryKey^));
  end
  else
    Result := '';
end;

function EncryString_LF2(Str: string): string;
begin
  if Str <> '' then begin
    EncryptDes_New(Str[1], Str[1], Length(Str), IntToStr(NewEncryKey^));
    Result := EncodeString(Base64EncodeStr(Str));
  end else begin
    Result := '';
  end;
end;


{$IF TESTMODE = 2}

function DecompressString(S: PChar; Len: Integer): string;
var
  OutBuf: Pointer;
  OutBytes: Integer;
begin
  if Len = 0 then
  begin
    Result := '';
    Exit;
  end;

  OutBuf := nil;
  OutBytes := 0;
  try
    DecompressBuf(S, Len, 0, OutBuf, OutBytes);
    SetLength(Result, OutBytes);
    Move(OutBuf^, Result[1], OutBytes);
  finally
    FreeMem(OutBuf);
  end;
end;

function Hex2Str(S: string): string;
var
  I: Integer;
  Len: Integer;
  STemp: string;
begin
  Result := '';
  if Length(S) mod 2 <> 0 then Exit;

  Len := Length(S) div 2;
  SetLength(Result, Len);

  for I := 1 to Len do
  begin
    STemp := Copy(S, I * 2 - 1, 2);
    Result[I] := Chr(StrToIntDef('$' + STemp, 0));
  end;
end;


var
  RsaModulus, RsaPublicKey{, RsaPrivateKey}: string;

  ZlibBuf: Pointer;
  ZlibLen, Len: Integer;
  RSA: PRSA;
  //PrivateKey: pBIO;

  BlockSize, BlockCount: Integer;
  InSize: Integer;

  RSABuf: PChar;
  PIn, POut: PByte;
  I, OutBufSize: Integer;

  bnModulus, bnPublicKey{, bnPrivateKey}: PBIGNUM;

  CallCRC: Cardinal;
{$ELSE}
var
  m_MyVEH: Pointer = nil;
  //m_Semaphore: THandle;
{$IFEND}


// VEH 异常
var
  ErrorEIP: LongWord = 0;
  function ZwTerminateProcess(thread: thandle; dwCode:Dword):Boolean; stdcall; external 'ntdll.dll';

function VectoredHandler(ExceptionInfo: PEXCEPTION_POINTERS): Integer; stdcall;
//var
//  l_protect, l_oldProtect: Cardinal;
begin
  if (ExceptionInfo.ExceptionRecord.ExceptionCode = $C0000096 ) then
  begin
    Inc(ExceptionInfo.ContextRecord.Eip);
    Result := EXCEPTION_CONTINUE_EXECUTION;
    Exit;
  end
  {
  else if (ExceptionInfo.ExceptionRecord.ExceptionCode = $C0000005) then
  begin
    if (ErrorEIP <> ExceptionInfo.ContextRecord.Eip) and (not IsBadReadPtr(Pointer(ExceptionInfo.ContextRecord.Eip), 4)) then
    begin
      ErrorEIP := ExceptionInfo.ContextRecord.Eip;

      l_protect := PAGE_EXECUTE_READWRITE;
      VirtualProtect(Pointer(ExceptionInfo.ContextRecord.Eip), 4, l_protect, @l_oldProtect);
      Dec(ExceptionInfo.ContextRecord.Eip);
      Result := EXCEPTION_CONTINUE_EXECUTION;
      Exit;
    end
    else
    begin
      ErrorEIP := 0;

      Inc(ExceptionInfo.ContextRecord.Eip);
      Result := EXCEPTION_CONTINUE_EXECUTION;
      Exit;
    end;
  end
  }
  else
  begin
    Result := EXCEPTION_CONTINUE_SEARCH;
  end;
end;


function InstallVectoredException: Boolean;
type
  TAddVectored = function(FirstHandler: Integer; VectoredHandler: Pointer): Pointer; stdcall;
var
  Kernel_AddVectored: TAddVectored;
begin
  Result := False;
  Kernel_AddVectored := GetProcAddress(LoadLibrary('Kernel32.dll'), 'AddVectoredExceptionHandler');
  if not Assigned(Kernel_AddVectored) then Exit;
  // 添加VEH  参数1=1表示插入Veh链的头部，=0表示插入到VEH链的尾部
  m_MyVEH := Kernel_AddVectored(1, @VectoredHandler);
  Result := m_MyVEH <> nil;
end;

function UnInstallVectoredException(Vectored: Pointer): Boolean;
type
  TRemoveVectored = function(Vectored: Pointer): LongWord; stdcall;
var
  Kernel_RemoveVectored: TRemoveVectored;
begin
  Result := False;
  Kernel_RemoveVectored := GetProcAddress(LoadLibrary('Kernel32.dll'), 'RemoveVectoredExceptionHandler');
  if not Assigned(Kernel_RemoveVectored) then Exit;
  Result := Kernel_RemoveVectored(Vectored) <> 0;
end;


(*
procedure DisableDEP;
const
  PROCESS_DEP_FALSE: DWORD = $00000000;
  PROCESS_DEP_ENABLE: DWORD = $00000001;
  PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION: DWORD= 2;
var
  SetProcessDEPPolicy: function(dwFlags: DWORD): BOOL; stdcall;
begin
  SetProcessDEPPolicy := GetProcAddress(GetModuleHandle(kernel32), 'SetProcessDEPPolicy');
  if Assigned(SetProcessDEPPolicy) then
  begin

    if not SetProcessDEPPolicy(PROCESS_DEP_FALSE) then
    begin
      DebugOutStr('DEP关闭失败，错误代码:' + IntToStr(GetLastError));
    end
    else
    begin
      DebugOutStr('DEP关闭成功');
    end;
  end;
end;

function CloseDEP: Boolean;
type
  TNtSetInformationProcess = function(ProcessHandle: THANDLE; ProcessInformationClass: ULONG; ProcessInformation: Pointer; ProcessInformationLength: ULONG): DWORD; stdcall;
var
  NtSetInformationProcess: TNtSetInformationProcess;
  hNtdll: HMODULE;
  ExecuteFlags: DWORD;
  nRet: DWORD;
begin
  Result := False;
  hNtdll := LoadLibraryA('ntdll.dll');
  if hNtdll < 32 then Exit;
  NtSetInformationProcess := GetProcAddress(hNtdll, 'NtSetInformationProcess');
  if @NtSetInformationProcess <> nil then
  begin
    ExecuteFlags := 2;

    nRet := NtSetInformationProcess(GetCurrentProcess, $22, @ExecuteFlags, sizeof(ExecuteFlags));
    DebugOutStr('DEP关闭2返回代码:' + IntToStr(nRet));
  end;
end;
*)

function GetRunCount(sMapName: string): Integer;
type
  PClientMapping = ^TClientMapping;
  TClientMapping = packed record
    Handle: THandle;
    RunCount: Integer;
    ClientHandle: array[0..29] of THandle;
  end;
var
  I: Integer;
  Wnd, FileMappingHandle: THandle;
  ClientMapping: PClientMapping;
begin
{$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectBeginUltra.inc}
  asm
  db $EB,$10,'VMProtect begin',3
  end;
{$ELSE}
  {.$I AddVmpFeatureCode.inc}
{$IFEND}


  Result := 0;
  FileMappingHandle := OpenFileMapping(FILE_MAP_ALL_ACCESS, False, PChar(sMapName));
  if FileMappingHandle <> 0 then
  begin
    ClientMapping := MapViewOfFile(FileMappingHandle,
      FILE_MAP_ALL_ACCESS,
      0,
      0,
      SizeOf(TClientMapping));
    if ClientMapping <> nil then begin
      for I := 0 to Length(ClientMapping.ClientHandle) - 1 do begin
        if ClientMapping.ClientHandle[I] <> 0 then begin
          Wnd := GetWindow(ClientMapping.ClientHandle[I], gw_HWndFirst);
          if Wnd = 0 then begin
            Dec(ClientMapping.RunCount);
            if ClientMapping.RunCount < 0 then ClientMapping.RunCount := 0;
            ClientMapping.ClientHandle[I] := 0;
          end;
        end;
      end;

      Result := ClientMapping.RunCount;
    end else begin
      CloseHandle(FileMappingHandle);
      //ClientMapping := nil;
      //FileMappingHandle := 0;
    end;
  end else begin
    FileMappingHandle := CreateFileMapping($FFFFFFFF,
      nil,
      PAGE_READWRITE,
      0,
      SizeOf(TClientMapping),
      PChar(sMapName));

    if FileMappingHandle <> 0 then begin
        ClientMapping := MapViewOfFile(FileMappingHandle,
        FILE_MAP_ALL_ACCESS,
        0,
        0,
        SizeOf(TClientMapping));
        FillChar(ClientMapping^, SizeOf(TClientMapping), #0);
    end;
  end;

{$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectEnd.inc}
  asm
  db $EB,$0E,'VMProtect end',0
  end;
{$IFEND}
end;

procedure UpdateRunCount(sMapName: string; IsInc: Boolean);
type
  PClientMapping = ^TClientMapping;
  TClientMapping = packed record
    Handle: THandle;
    RunCount: Integer;
    ClientHandle: array[0..29] of THandle;
  end;
var
  I: Integer;
  FileMappingHandle: THandle;
  ClientMapping: PClientMapping;
begin
{$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectBeginUltra.inc}
  asm
  db $EB,$10,'VMProtect begin',3
  end;
{$ELSE}
  {.$I AddVmpFeatureCode.inc}
{$IFEND}
  FileMappingHandle := OpenFileMapping(FILE_MAP_ALL_ACCESS, False, PChar(sMapName));
  if FileMappingHandle <> 0 then begin
    ClientMapping := MapViewOfFile(FileMappingHandle,
      FILE_MAP_ALL_ACCESS,
      0,
      0,
      SizeOf(TClientMapping));
    if ClientMapping <> nil then begin
      if IsInc then begin
        ClientMapping.RunCount := ClientMapping.RunCount + 1;

        for I := 0 to Length(ClientMapping.ClientHandle) - 1 do begin
          if ClientMapping.ClientHandle[I] = 0 then begin
            ClientMapping.ClientHandle[I] := Application.Handle;
            Break;
          end;
        end;
      end else begin
        ClientMapping.RunCount := ClientMapping.RunCount - 1;
        if ClientMapping.RunCount < 0 then ClientMapping.RunCount := 0;

        for I := 0 to Length(ClientMapping.ClientHandle) - 1 do begin
          if ClientMapping.ClientHandle[I] = Application.Handle then begin
            ClientMapping.ClientHandle[I] := 0;
            Break;
          end;
        end;
      end;
    end else begin
      CloseHandle(FileMappingHandle);
      //ClientMapping := nil;
      //FileMappingHandle := 0;
    end;
  end else begin
    FileMappingHandle := CreateFileMapping($FFFFFFFF,
      nil,
      PAGE_READWRITE,
      0,
      SizeOf(TClientMapping),
      PChar(sMapName));

    if FileMappingHandle <> 0 then begin
      ClientMapping := MapViewOfFile(FileMappingHandle,
        FILE_MAP_ALL_ACCESS,
        0,
        0,
        SizeOf(TClientMapping));
      FillChar(ClientMapping^, SizeOf(TClientMapping), #0);

      if IsInc then begin
        ClientMapping.RunCount := ClientMapping.RunCount + 1;

        for I := 0 to Length(ClientMapping.ClientHandle) - 1 do begin
          if ClientMapping.ClientHandle[I] = 0 then begin
            ClientMapping.ClientHandle[I] := Application.Handle;
            Break;
          end;
        end;
      end else begin
        ClientMapping.RunCount := ClientMapping.RunCount - 1;
        if ClientMapping.RunCount < 0 then ClientMapping.RunCount := 0;

        for I := 0 to Length(ClientMapping.ClientHandle) - 1 do begin
          if ClientMapping.ClientHandle[I] = Application.Handle then begin
            ClientMapping.ClientHandle[I] := 0;
            Break;
          end;
        end;
      end;
    end;
  end;
{$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectEnd.inc}
 asm
  db $EB,$0E,'VMProtect end',0
 end; 
{$IFEND}
end;



begin
  {$IF TESTMODE = 0}
  {$IF PRIVATE_CLIENT = 0}
    {.$I VMProtectBeginUltra.inc}
  asm
  db $EB,$10,'VMProtect begin',3
  end;
  {$ELSE}
    {.$I AddVmpFeatureCode.inc}
  {$IFEND}
  {$IFEND}
  IsMultiThread := True;
  {$IFDEF DEBUG}
  ReportMemoryLeaksOnShutdown := True;
  //System.RegisterExpectedMemoryLeak;
  {$ENDIF}
  //SetIeCompatibilityMode(); //HZQ 20230829拿掉

  g_sMainParam1 := ParamStr(0);

  g_sSelfFileName := ParamStr(0);
  g_sSelfFilePath := ExtractFilePath(ParamStr(0));
  g_sSelfResourcePath := IncludeTrailingPathDelimiter(g_sSelfFilePath + g_ResourcesDir); //HZQ，后面也有一个，感觉重复了

  FillChar(FCurrentScreenMode, SizeOf(TDeviceMode), 0);
  FillChar(FNewScreenMode, SizeOf(TDeviceMode), 0);
  New(g_PKey);

  DebugOutStr(DecodeResStr(SAppInit01Log));
  InstallVectoredException;

{$IF TESTMODE = 0}
  try
    DebugOutStr(DecodeResStr(SAppInit02Log));

    // 用于三方登录器，因为不想提供EncryptDes_New的代码 chongchong 2018-06-23 14:09:44
    if ParamStr(1) = '0' then begin
      g_PKey^ := StrToIntDef(DecryString_LF(ParamStr(2)), 0);                                           //登录器生成的随机码
      g_sMainParam2 := ParamStr(3);                                                                     //读取设置参数
    end  else begin
      g_PKey^ := StrToIntDef(DecryString_LF2(ParamStr(1)), 0);                                           //登录器生成的随机码
      g_sMainParam2 := ParamStr(2);                                                                     //读取设置参数
    end;

    FillChar(g_ClientParam, SizeOf(TClientParam), #0);
    zDecryBufferK(g_sMainParam2, @g_ClientParam, SizeOf(TClientParam), EncryptUnit.GetKeyValue(g_PKey^));
    //DebugOutStr('Client 3 ');

    {
    ///////// 限制应用启动数量 chongchong 2018-02-03
    m_Semaphore := OpenSemaphore(SEMAPHORE_ALL_ACCESS, True, PChar(string(g_ClientParam.sSemaphoreName)));
    if m_Semaphore = 0 then
    begin
      (*
        第一个参数表示安全控制，一般直接传入NULL。
        第二个参数表示初始资源数量。
        第三个参数表示最大并发数量。
        第四个参数表示信号量的名称，传入NULL表示匿名信号量。
      *)
      m_Semaphore := CreateSemaphore(nil, g_ClientParam.btMaxClientCount, g_ClientParam.btMaxClientCount, PChar(string(g_ClientParam.sSemaphoreName)));     // 初始3个，限制3个
    end;

    if m_Semaphore = 0 then
    begin
      Exit;
    end;
    
    // 等待通知时间0秒
    if WaitForSingleObject(m_Semaphore, 0) <> WAIT_OBJECT_0 then
    begin
      CloseHandle(m_Semaphore);
      Exit;
    end;
    ///////// 限制应用启动数量 chongchong 2018-02-03
    }

    if g_ClientParam.btMaxClientCount > 30 then g_ClientParam.btMaxClientCount := 30;
    
    if GetRunCount(g_ClientParam.sSemaphoreName) >= g_ClientParam.btMaxClientCount then begin
      Exit;
    end;

    UpdateRunCount(g_ClientParam.sSemaphoreName, True);

    g_dwGameLoginHandle := g_ClientParam.Handle;
    g_sServerAddr := g_ClientParam.sServeraddr;
    g_nServerPort := g_ClientParam.nServerPort;

    g_ClientVersion := g_ClientParam.ClientVersion;

    g_boWindowMode := g_ClientParam.boWindowMode;
    g_nScreenWidth := g_ClientParam.wScreenWidth;                                                     // 分辨率
    g_nScreenHeight := g_ClientParam.wScreenHeight;                                                   // 分辨率
    g_nBitCount := g_ClientParam.btBitCount;
    g_boVSync := g_ClientParam.boVSync;
    g_boHardware := g_ClientParam.boHardware;

    g_sUpdateAddr := g_ClientParam.sUpdateAddr;                                                       // 微端更新地址
    g_nUpdatePort := g_ClientParam.nUpdatePort;                                                       // 微端更新端口

    // 先屏蔽微端自动更新，待网关返回数据再开启 piaoyun 2013-11-21
    g_boAutoUpdate := False;
    //g_boAutoUpdate := (g_sUpdateAddr <> '') and (g_nUpdatePort >= 1024) and (g_nUpdatePort <= 65535); // 微端自动更新

    g_sUpdatePassWord := DecryString_LF(g_ClientParam.sUpdatePassWord);

    g_sUpdateGateAddr := g_ClientParam.sUpdateAddr;
    g_nUpdateGatePort := g_ClientParam.nUpdatePort;

    g_ClientDataFile := g_ClientParam.sClientDataFile;

    g_sPromotionFlag := g_ClientParam.sPromotionFlag;

    g_GameLoginConfigUrlMD5 := MD5Print(g_ClientParam.ConfigUrlMD5);

    g_boD3DFormat := False;

    SetMachineID(g_ClientParam.sMachineID);

    DebugOutStr(DecodeResStr(SAppInit03Log));
  except
    DebugOutStr(DecodeResStr(SAppInitErr01));
  end;
  {$IF PRIVATE_CLIENT = 0}
    {.$I VMProtectEnd.inc}
  asm
  db $EB,$0E,'VMProtect end',0
  end;  
  {$IFEND}

{$ELSEIF TESTMODE = 2}
  {.$I VMProtectBeginUltra.inc}
  g_sMainParam1 := ParamStr(1);
  g_sMainParam2 := ParamStr(2);

  if (Length(g_sMainParam1) = 0) or (Length(g_sMainParam2) = 0) then Exit;

  CallCRC := BufferCRC(PChar(g_sMainParam2), Length(g_sMainParam2));
  if not SameText(IntToStr(CallCRC), g_sMainParam1) then begin
    Exit;
  end;

  g_sMainParam2 := Base64DecodeStr(g_sMainParam2);

  {
  RsaPrivateKey := '-----BEGIN PRIVATE KEY-----' + #10 +
    'MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAMuhfS9M4bJT+21V' + #10 +
    'idfG7I2GGfzYe0RZKDdtaoAuMTzVm40AfHZErGq8skKi2GsLzcFD/IL7UfWQTVcT' + #10 +
    'WHhla0ZR5lpy9JP55XbQdpsuJ6dqvreCHgdB1Utd8TK9mnyo9pgN3KSdWKUValjx' + #10 +
    'pMAxgjx9GFGj85nhhaHZH8WtOeNdAgMBAAECgYBFVQS0mC64cxPGVDuMtnRQc3ph' + #10 +
    'tquxx9GDncOHRTMKjYha5/F4q0UxSnI/cgbR28EArs9JIZz0SV+r6DBVPeLaAQhD' + #10 +
    '6lkrxGjzNfxBOj8xYaDW0Mew1E/sZfefgoUc3QQqTDiuVif3gO6r6U7Oxl7sVP1F' + #10 +
    'ovpxItkfswzJOrSugQJBAOam+jgQjMdGDuJubKO3R8U5xHyMY0SBz5QOg5sWR4ew' + #10 +
    'HawHTre1tZAV/MorIEld6G/Hl3D3OwFTQAQz5pvB8G0CQQDiAk7I3QdLWXNG561s' + #10 +
    'E9zZaz7cMV3qLl95tu79YkdS9n+zdhVdi3dwJilaiv5EXL4bHCcSSbe6VY0JHcBI' + #10 +
    'FEixAkBr9GdU6loZwu6giHKMxfHvm7QdX0/u9psDfy+V3P2pwoKAzALJ9WB/iesB' + #10 +
    'bOH1tOBfwRlepDiXzDFARlJ/QGyZAkADU+9fC8ogtOd6osyt67jzxp19VojAewBS' + #10 +
    '5XutZXYRZanJtbJo1zXiA93WBHfr/8WY1phIr6cx6jKScMq1BP9xAkBJHnxS+Bo6' + #10 +
    'ja0s0uHaW41ng7mszPM8isV4J1rbDOeGZxOIGJURocPxp0vzvYRw8FLsT3KOMGuX' + #10 +
    'sqgHbT8sj14M' + #10 +
    '-----END PRIVATE KEY-----';
  PrivateKey := BIO_new_mem_buf(@RsaPrivateKey[1], Length(RsaPrivateKey));
  RSA := ReadKeyFromBIO(PrivateKey, False);
  }

  RsaModulus :=
    '801B680F9BD42A93222FB49C791C6B1BBBA72DBEFAAC235F2A49A03541FF41A871' +
    '8BC2D99A62FD6C6EDF7726C7921FAA59DE1DBC65AD05914F835824B9AB0024FFB9' +
    'D6FA2B1924B3B8FB06D8444D80335038703235C6179425FBD46D2BB779DDBF62AE' +
    '1F06259DE9AC352F2898C91E07A938FEA70AA7341ECB9E41797023D1D1';

  {
  RsaPrivateKey :=
    '3989040EF5106AFB2B822EC894B66A3E003F6266123588CE5015061CBAA15389D5' +
    '0C2840468CF0AE1726EDC8E87463D619FBABE83389606DF16539D95C7B5E4579FE' +
    'A348AF9F669F74DA8056A6A983E2F3292C33E5C693FB900CAAE5954A63A5CF1B91' +
    '54513DEE888EB8DAA11A96B36BF3295AF0C92084DCB66903EFD28FBB2F';
  }

  bnModulus := BN_new();
  bnPublicKey := BN_new();
  //bnPrivateKey := BN_new(); 

  BN_hex2bn(@bnModulus, @RsaModulus[1]);          // 将模数转为大数 注意函数名指定16进制
  BN_set_word(bnPublicKey, $65537);
  //BN_hex2bn(@bnPrivateKey, @RsaPrivateKey[1]);    // 将公匙转为大数 同上

  RSA := RSA_new;
  RSA.n := bnMODULUS;                             // 指定RSA的模数
  RSA.e := bnPublicKey;                           // 指定公匙
  //RSA.d := bnPrivateKey;                          // 指定私匙

  BlockSize := RSA_SIZE(RSA);
  BlockCount := Length(g_sMainParam2) div BlockSize;
  GetMem(RSABuf, BlockSize * BlockCount);
  ZeroMemory(RSABuf, BlockSize * BlockCount);

  PIn := @g_sMainParam2[1];
  POut := PByte(RSABuf);
  OutBufSize := 0;
  for I := 0 to BlockCount - 1 do begin
    Len := RSA_public_decrypt(BlockSize, PIn, POut, RSA, RSA_PKCS1_PADDING);
    Inc(PIn, BlockSize);
    Inc(POut, Len);
    Inc(OutBufSize, Len);
  end;

  g_sMainParam2 := DecompressString(RSABuf, OutBufSize);

  if Length(g_sMainParam2) = SizeOf(TClientParam) then begin
    FillChar(g_ClientParam, SizeOf(TClientParam), #0);
    Move(g_sMainParam2[1], g_ClientParam, SizeOf(TClientParam));

    g_dwGameLoginHandle := g_ClientParam.Handle;
    g_sServerAddr := g_ClientParam.sServeraddr;
    g_nServerPort := g_ClientParam.nServerPort;

    //g_ClientVersion := g_ClientParam.ClientVersion;

    g_boWindowMode := g_ClientParam.boWindowMode;
    g_nScreenWidth := g_ClientParam.wScreenWidth;                                                     // 分辨率
    g_nScreenHeight := g_ClientParam.wScreenHeight;                                                   // 分辨率
    g_nBitCount := g_ClientParam.btBitCount;
    g_boVSync := g_ClientParam.boVSync;
    g_boHardware := g_ClientParam.boHardware;

    g_sUpdateAddr := g_ClientParam.sUpdateAddr;                                                       // 微端更新地址
    g_nUpdatePort := g_ClientParam.nUpdatePort;                                                       // 微端更新端口

    // 先屏蔽微端自动更新，待网关返回数据再开启 piaoyun 2013-11-21
    g_boAutoUpdate := False;                                                                          //--- 调试OK后再开启这里，piaoyun
    //g_boAutoUpdate := (g_sUpdateAddr <> '') and (g_nUpdatePort >= 1024) and (g_nUpdatePort <= 65535); // 微端自动更新

    g_sUpdatePassWord := DecryString_LF(Hex2Str(g_ClientParam.sUpdatePassWord));

    g_sUpdateGateAddr := g_ClientParam.sUpdateAddr;
    g_nUpdateGatePort := g_ClientParam.nUpdatePort;
    g_boD3DFormat := False;

    g_ClientVersion := cvSerial;
    g_ResourcesDir := g_ClientParam.sResourcesDir;
    g_PakDefaultPassword := Hex2Str(g_ClientParam.sPakPassword);
    g_PKey^ := EncryptUnit.GetKeyValue;
  end else begin
    Exit;
  end;
  {.$I VMProtectEnd.inc}
{$ELSE}
  g_PKey^ := EncryptUnit.GetKeyValue;
{$IFEND}

  DebugOutStr(DecodeResStr(SAppInit04Log));
  // 此代码移植于TfrmMain.FormCreate(Sender: TObject) 修复导致登陆器启动异常的严重BUG。！！！ piaoyun 2013-09-04
  try
    DebugOutStr(DecodeResStr(SAppInit05Log));
    {$IF TestMode <> 1}
    LoadConfig(); //读取Client.dat
    {$IFEND}
    DebugOutStr(DecodeResStr(SAppInit06Log));
  except
    DebugOutStr(DecodeResStr(SAppLoadConfigErr));
  end;

  g_sSelfResourcePath := IncludeTrailingPathDelimiter(g_sSelfFilePath + g_ResourcesDir);

  {
  DisableDEP;
  CloseDEP;
  }
  
  DebugOutStr(DecodeResStr(SAppInit07Log));
  if ShowWelcomeDlg = mrOk then begin
    {$IF TestMode = 1}
    LoadConfig(); //读取Client.dat //HZQ 调试模式，读取配置延后，因为用到LoginFrm中的数据
    {$IFEND}

    try
      DebugOutStr(DecodeResStr(SAppInit08Log));
      Application.Initialize;
      DebugOutStr(DecodeResStr(SAppInit09Log));
    except
      DebugOutStr(DecodeResStr(SAppInitErr02));
    end;

    DebugOutStr(DecodeResStr(SAppInit10Log));
    try
      Application.HintPause := 10;
      Application.CreateForm(TfrmMain, frmMain);
  Application.CreateForm(TFrmNPCDeBug, FrmNPCDeBug);
  except
      DebugOutStr(DecodeResStr(SAppCreateMainFormErr));
    end;

    DebugOutStr(DecodeResStr(SAppInit11Log));
    Application.Run;
  end else begin { TODO -ochongchong -c内存泄露 : 处理选择服务器直接退出时内存泄露+++ 【2013-07-20】 }
    Dispose(g_PKey);
{$IF TESTMODE = 1}
    g_PlugFileNameList.Free;
    g_BackImage.Free;
    g_ImageFileList.Free;
    g_DataFileList.Free;
    g_MapFileList.Free;
    g_WavFileList.Free;
    //g_HMapDataFileList.Free;
{$IFEND}
  end;

  {
  ///////// 限制应用启动数量 chongchong 2018-02-03
  (*
  第一个参数是信号量的句柄。
  第二个参数表示增加个数，必须大于0且不超过最大资源数量。
  第三个参数可以用来传出先前的资源计数，设为NULL表示不需要传出。
  *)
  ReleaseSemaphore(m_Semaphore, 1, nil);
  CloseHandle(m_Semaphore);

  ///////// 限制应用启动数量 chongchong 2018-02-03
  }
  UpdateRunCount(g_ClientParam.sSemaphoreName, False);
  
  UnInstallVectoredException(m_MyVEH);
{$IF TESTMODE = 0}
  // 找不到原因，反外挂插件换一次后。大退异常 2019-08-19 23:59:24
  ZwTerminateProcess(GetCurrentProcess(), 0);
{$IFEND}
end.

