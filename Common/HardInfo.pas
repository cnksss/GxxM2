unit HardInfo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, StrUtils, Registry, ActiveX, MD5Util, Math,
  {$IF CompilerVersion >= 22}System.AnsiStrings,{$IFEND}
  Nb30 {Net bios 30};

type

  TRegisters = record
    EAX: DWORD;
    EBX: DWORD;
    ECX: DWORD;
    EDX: DWORD;
  end;
  TCPUID = array [1 .. 4] of Longint;
// OS
function GetWindowsVersion: string;
function GetDisplayDevice: string;
function GetDisplayFrequency: Integer;

  // IDE
function GetIdeSerialNumber: pchar;
function GetIdeDiskSerialNumber(var SerialNumber: string;
  var ModelNumber: string;
  var FirmwareRev: string;
  var TotalAddressableSectors: ULong;
  var SectorCapacity: ULong;
  var SectorsPerTrack: Word): Boolean;                                                              // 得到硬盘物理号
  // CPU          
function GetCpuIDstringEx: string;   
function GetAdapterMac(ANo: Integer): string;
{$IFDEF M2SERVER}
function GetCpuName: String;
function GetCPUCount: String;
function GetMemorySize: string;
{$ENDIF}
implementation


// 获取第一个IDE硬盘的序列号

function GetIdeSerialNumber: pchar;
const
  IDENTIFY_BUFFER_SIZE = 512;
type
  TIDERegs = packed record
    bFeaturesReg: BYTE;                                                                             // Used for specifying SMART "commands".
    bSectorCountReg: BYTE;                                                                          // IDE sector count register
    bSectorNumberReg: BYTE;                                                                         // IDE sector number register
    bCylLowReg: BYTE;                                                                               // IDE low order cylinder value
    bCylHighReg: BYTE;                                                                              // IDE high order cylinder value
    bDriveHeadReg: BYTE;                                                                            // IDE drive/head register
    bCommandReg: BYTE;                                                                              // Actual IDE command.
    bReserved: BYTE;                                                                                // reserved for future use.  Must be zero.
  end;
  TSendCmdInParams = packed record
   // Buffer size in bytes
    cBufferSize: DWORD;
   // Structure with drive register values.
    irDriveRegs: TIDERegs;
   // Physical drive number to send command to (0,1,2,3).
    bDriveNumber: BYTE;
    bReserved: array[0..2] of Byte;
    dwReserved: array[0..3] of DWORD;
    bBuffer: array[0..0] of Byte;                                                                   // Input buffer.
  end;
  TIdSector = packed record
    wGenConfig: Word;
    wNumCyls: Word;
    wReserved: Word;
    wNumHeads: Word;
    wBytesPerTrack: Word;
    wBytesPerSector: Word;
    wSectorsPerTrack: Word;
    wVendorUnique: array[0..2] of Word;
    sSerialNumber: array[0..19] of CHAR;
    wBufferType: Word;
    wBufferSize: Word;
    wECCSize: Word;
    sFirmwareRev: array[0..7] of Char;
    sModelNumber: array[0..39] of Char;
    wMoreVendorUnique: Word;
    wDoubleWordIO: Word;
    wCapabilities: Word;
    wReserved1: Word;
    wPIOTiming: Word;
    wDMATiming: Word;
    wBS: Word;
    wNumCurrentCyls: Word;
    wNumCurrentHeads: Word;
    wNumCurrentSectorsPerTrack: Word;
    ulCurrentSectorCapacity: DWORD;
    wMultSectorStuff: Word;
    ulTotalAddressableSectors: DWORD;
    wSingleWordDMA: Word;
    wMultiWordDMA: Word;
    bReserved: array[0..127] of BYTE;
  end;
  PIdSector = ^TIdSector;
  TDriverStatus = packed record
   // 驱动器返回的错误代码，无错则返回0
    bDriverError: Byte;
   // IDE出错寄存器的内容，只有当bDriverError 为 SMART_IDE_ERROR 时有效
    bIDEStatus: Byte;
    bReserved: array[0..1] of Byte;
    dwReserved: array[0..1] of DWORD;
  end;
  TSendCmdOutParams = packed record
   // bBuffer的大小
    cBufferSize: DWORD;
   // 驱动器状态
    DriverStatus: TDriverStatus;
   // 用于保存从驱动器读出的数据的缓冲区，实际长度由cBufferSize决定
    bBuffer: array[0..0] of BYTE;
  end;
var
  hDevice: THandle;
  cbBytesReturned: DWORD;
// ptr : PChar;
  SCIP: TSendCmdInParams;
  aIdOutCmd: array[0..(SizeOf(TSendCmdOutParams) + IDENTIFY_BUFFER_SIZE - 1) - 1] of Byte;
  IdOutCmd: TSendCmdOutParams absolute aIdOutCmd;

  procedure ChangeByteOrder(var Data; Size: Integer);
  var
    ptr: PChar;
    i: Integer;
    c: Char;
  begin
    ptr := @Data;
    for i := 0 to (Size shr 1) - 1 do
    begin
      c := ptr^;
      ptr^ := (ptr + 1)^;
      (ptr + 1)^ := c;
      Inc(ptr, 2);
    end;
  end;
begin
  Result := '';                                                                                     // 如果出错则返回空串
  if SysUtils.Win32Platform = VER_PLATFORM_WIN32_NT then
  begin                                                                                             // Windows NT, Windows 2000
       // 提示! 改变名称可适用于其它驱动器，如第二个驱动器： '\\.\PhysicalDrive1\'
    hDevice := CreateFile('\\.\PhysicalDrive0', GENERIC_READ or GENERIC_WRITE,
      FILE_SHARE_READ or FILE_SHARE_WRITE, nil, OPEN_EXISTING, 0, 0);
  end
  else                                                                                              // Version Windows 95 OSR2, Windows 98
    hDevice := CreateFile('\\.\SMARTVSD', 0, 0, nil, CREATE_NEW, 0, 0);
  if hDevice = INVALID_HANDLE_VALUE then Exit;
  try
    FillChar(SCIP, SizeOf(TSendCmdInParams) - 1, #0);
    FillChar(aIdOutCmd, SizeOf(aIdOutCmd), #0);
    cbBytesReturned := 0;
     // Set up data structures for IDENTIFY command.
    with SCIP do
    begin
      cBufferSize := IDENTIFY_BUFFER_SIZE;
 // bDriveNumber := 0;
      with irDriveRegs do
      begin
        bSectorCountReg := 1;
        bSectorNumberReg := 1;
 // if Win32Platform=VER_PLATFORM_WIN32_NT then bDriveHeadReg := $A0
 // else bDriveHeadReg := $A0 or ((bDriveNum and 1) shl 4);
        bDriveHeadReg := $A0;
        bCommandReg := $EC;
      end;
    end;
    if not DeviceIoControl(hDevice, $0007C088, @SCIP, SizeOf(TSendCmdInParams) - 1,
      @aIdOutCmd, SizeOf(aIdOutCmd), cbBytesReturned, nil) then Exit;
  finally
    CloseHandle(hDevice);
  end;
  with PIdSector(@IdOutCmd.bBuffer)^ do
  begin
    ChangeByteOrder(sSerialNumber, SizeOf(sSerialNumber));
    (PChar(@sSerialNumber) + SizeOf(sSerialNumber))^ := #0;
    Result := PChar(@sSerialNumber);
  end;
end;

 // 更多关于 S.M.A.R.T. ioctl 的信息可查看:
 // http://www.microsoft.com/hwdev/download/respec/iocltapi.rtf

 // MSDN库中也有一些简单的例子
 // Windows Development -> Win32 Device Driver Kit ->
 // SAMPLE: SmartApp.exe Accesses SMART stats in IDE drives

 // 还可以查看 http://www.mtgroup.ru/~alexk
 // IdeInfo.zip - 一个简单的使用了S.M.A.R.T. Ioctl API的Delphi应用程序

 // 注意:

 // WinNT/Win2000 - 你必须拥有对硬盘的读/写访问权限

 // Win98
 // SMARTVSD.VXD 必须安装到 \windows\system\iosubsys
 // (不要忘记在复制后重新启动系统)

// ===========================================================
// 这个函数返回的显示刷新率是以Hz为单位的

function GetDisplayFrequency: Integer;
var
  DeviceMode: TDeviceMode;
begin
  EnumDisplaySettings(nil, Cardinal(-1), DeviceMode);
  Result := DeviceMode.dmDisplayFrequency;
end;

function GetDisplayDevice: string;
var
  lpDisplayDevice: TDisplayDevice;
  dwFlags: DWORD;
  cc: DWORD;
begin
  lpDisplayDevice.cb := sizeof(lpDisplayDevice);
  dwFlags := 0;
  cc := 0;
  while EnumDisplayDevices(nil, cc, lpDisplayDevice, dwFlags) do
  begin
    Inc(cc);
    if (lpDisplayDevice.DeviceName = '\\.\Display1') or (lpDisplayDevice.DeviceName = '\\.\DISPLAY1') then
      Result := lpDisplayDevice.DeviceString;
   // ListBox1.Items.Add(lpDisplayDevice.DeviceString); {there is also additional information in lpDisplayDevice}
  end;
end;

function CountSetBits(const bitMask: Cardinal): DWORD;
var
  LSHIFT     : DWORD;
  bitSetCount: DWORD;
  bitTest    : Uint64;
  I          : DWORD;
begin
  LSHIFT      := sizeof(Cardinal) * 8 - 1;
  bitSetCount := 0;
  bitTest     := 1 shl LSHIFT;

  for I := 0 to LSHIFT - 1 do
  begin
    bitSetCount := Ifthen((bitMask and bitTest) = 0, 1, 0);
    bitTest     := bitTest div 2;
  end;

  Result := bitSetCount;
end;
{$IFDEF M2SERVER}
procedure GetCPUID(Param: Cardinal; var Registers: TRegisters);
asm
  {$IF Defined(CPUX86)}
  PUSH    EBX                         { save affected registers }
  PUSH    EDI
  MOV     EDI, Registers
  XOR     EBX, EBX                    { clear EBX register }
  XOR     ECX, ECX                    { clear ECX register }
  XOR     EDX, EDX                    { clear EDX register }
  DB $0F, $A2                         { CPUID opcode }
  MOV     TRegisters(EDI).&EAX, EAX   { save EAX register }
  MOV     TRegisters(EDI).&EBX, EBX   { save EBX register }
  MOV     TRegisters(EDI).&ECX, ECX   { save ECX register }
  MOV     TRegisters(EDI).&EDX, EDX   { save EDX register }
  POP     EDI                         { restore registers }
  POP     EBX
  {$ELSEIF Defined(CPUX64)}
  PUSH    RBX                         { save affected registers }
  PUSH    RDI
  MOV     RDI, Registers
  XOR     EBX, EBX                    { clear EBX register }
  XOR     ECX, ECX                    { clear ECX register }
  XOR     EDX, EDX                    { clear EDX register }
  CPUID
  MOV     TRegisters(RDI).&EAX, EAX   { save EAX register }
  MOV     TRegisters(RDI).&EBX, EBX   { save EBX register }
  MOV     TRegisters(RDI).&ECX, ECX   { save ECX register }
  MOV     TRegisters(RDI).&EDX, EDX   { save EDX register }
  POP     RDI                         { restore registers }
  POP     RBX
  {$IFEND}
end;


function GetMemorySize: string;
var
  oMemoinfo: TMemoryStatusEx;
  cSize: Int64;
begin
  oMemoinfo.dwLength := SizeOf(TMemoryStatusEx);
  GlobalMemoryStatusEx(oMemoinfo);
  cSize := oMemoinfo.ullTotalPhys;
  if cSize > 1024*1024*1024 then
    Result := FloatToStr(Round(cSize / (1024*1024*1024))) + 'GB'
  else if cSize > 1024*1024 then
    Result := FloatToStr(Round(cSize / (1024 * 1024))) + 'MB'
  else if cSize > 1024 then
    Result := FloatToStr(Round(cSize / 1024)) + 'KB'
  else
    Result := FloatToStr(cSize) + 'Bytes';
end;

function GetCpuName: String;
var
  regs          : TRegisters;
  processor_name: array [0 .. 48] of AnsiChar;
  III           : Integer;
  TTT           : Cardinal;
begin
  for III := 2 to 4 do
  begin
    TTT := 1 shl 31 + III;
    GetCPUID(TTT, regs);
    //CPUID := GetCPUID();
    Move(regs.EAX, processor_name[(III - 2) * 16 + 00], 4);
    Move(regs.EBX, processor_name[(III - 2) * 16 + 04], 4);
    Move(regs.ECX, processor_name[(III - 2) * 16 + 08], 4);
    Move(regs.EDX, processor_name[(III - 2) * 16 + 12], 4);
  end;
  processor_name[48] := #0;
  Result             := string(AnsiString(processor_name));
end;

{ 获取 CPU 个数 }
function GetCPUCount: String;
var
  Buffer               : array of SYSTEM_LOGICAL_PROCESSOR_INFORMATION;
  ReturnLength         : DWORD;
  III, Count           : Integer;
  processorCoreCount   : Integer;
  numaNodeCount        : Integer;
  logicalProcessorCount: Integer;
  processorPackageCount: Integer;
  JJJ                  : Integer;
begin
  SetLength(Buffer, 1);
  ReturnLength := sizeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION);

  { 第一次调用获取缓冲区大小 }
  if not GetLogicalProcessorInformation(@Buffer[0], ReturnLength) then
  begin
    if GetLastError = ERROR_INSUFFICIENT_BUFFER then
    begin
      SetLength(Buffer, ReturnLength div sizeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION) + 1);
      { 第二次调用，返回结果 }
      if not GetLogicalProcessorInformation(@Buffer[0], ReturnLength) then
      begin
        Exit;
      end;
    end;
  end;

  processorCoreCount    := 0;
  numaNodeCount         := 0;
  logicalProcessorCount := 0;
  processorPackageCount := 0;

  Count   := ReturnLength div sizeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION);
  for III := 0 to Count - 1 do
  begin
    case Buffer[III].Relationship of
      RelationProcessorCore:
        Inc(processorCoreCount);
      RelationNumaNode:
        Inc(numaNodeCount);
      RelationProcessorPackage:
        Inc(processorPackageCount);
      RelationCache:
        begin
          JJJ := CountSetBits(Buffer[III].ProcessorMask);
          if JJJ = 1 then
          begin
            Inc(logicalProcessorCount);
          end;
        end;
    end;
  end;
  Result := Format('NumaNodes=%d PhysicalProcessorPackages=%d ProcessorCores=%d LogicalProcessors=%d', [numaNodeCount, processorPackageCount, processorCoreCount,
    logicalProcessorCount]);
end;

{$ENDIF}


function GetCpuIDstringEx: string;
var
  SysInfo: TSYSTEMINFO;
begin
  GetSystemInfo(SysInfo);
  Result := IntToStr(SysInfo.dwOemId) + IntToStr(SysInfo.dwNumberOfProcessors) +
    IntToStr(SysInfo.dwProcessorType) + IntToStr(SysInfo.wProcessorRevision);
end;

function GetIdeDiskSerialNumber(var SerialNumber: string; var ModelNumber: string;
  var FirmwareRev: string; var TotalAddressableSectors: ULong;
  var SectorCapacity: ULong; var SectorsPerTrack: Word): Boolean;                                   // 得到硬盘物理号
type
  TSrbIoControl = packed record
    HeaderLength: ULong;
    Signature: array[0..7] of Char;
    Timeout: ULong;
    ControlCode: ULong;
    ReturnCode: ULong;
    Length: ULong;
  end;

  SRB_IO_CONTROL = TSrbIoControl;
  PSrbIoControl = ^TSrbIoControl;

  TIDERegs = packed record
    bFeaturesReg: Byte;                                                                             // Used for specifying SMART "commands".
    bSectorCountReg: Byte;                                                                          // IDE sector count register
    bSectorNumberReg: Byte;                                                                         // IDE sector number register
    bCylLowReg: Byte;                                                                               // IDE low order cylinder value
    bCylHighReg: Byte;                                                                              // IDE high order cylinder value
    bDriveHeadReg: Byte;                                                                            // IDE drive/head register
    bCommandReg: Byte;                                                                              // Actual IDE command.
    bReserved: Byte;                                                                                // reserved. Must be zero.
  end;
  IDEREGS = TIDERegs;
  PIDERegs = ^TIDERegs;

  TSendCmdInParams = packed record
    cBufferSize: DWORD;
    irDriveRegs: TIDERegs;
    bDriveNumber: Byte;
    bReserved: array[0..2] of Byte;
    dwReserved: array[0..3] of DWORD;
    bBuffer: array[0..0] of Byte;
  end;
  SENDCMDINPARAMS = TSendCmdInParams;
  PSendCmdInParams = ^TSendCmdInParams;

  TIdSector = packed record
    wGenConfig: Word;
    wNumCyls: Word;
    wReserved: Word;
    wNumHeads: Word;
    wBytesPerTrack: Word;
    wBytesPerSector: Word;
    wSectorsPerTrack: Word;
    wVendorUnique: array[0..2] of Word;
    sSerialNumber: array[0..19] of Char;
    wBufferType: Word;
    wBufferSize: Word;
    wECCSize: Word;
    sFirmwareRev: array[0..7] of Char;
    sModelNumber: array[0..39] of Char;
    wMoreVendorUnique: Word;
    wDoubleWordIO: Word;
    wCapabilities: Word;
    wReserved1: Word;
    wPIOTiming: Word;
    wDMATiming: Word;
    wBS: Word;
    wNumCurrentCyls: Word;
    wNumCurrentHeads: Word;
    wNumCurrentSectorsPerTrack: Word;
    ulCurrentSectorCapacity: ULong;
    wMultSectorStuff: Word;
    ulTotalAddressableSectors: ULong;
    wSingleWordDMA: Word;
    wMultiWordDMA: Word;
    bReserved: array[0..127] of Byte;
  end;
  PIdSector = ^TIdSector;

const
  IDE_ID_FUNCTION = $EC;
  IDENTIFY_BUFFER_SIZE = 512;
  DFP_RECEIVE_DRIVE_DATA = $0007C088;
  IOCTL_SCSI_MINIPORT = $0004D008;
  IOCTL_SCSI_MINIPORT_IDENTIFY = $001B0501;
  DataSize = sizeof(TSendCmdInParams) - 1 + IDENTIFY_BUFFER_SIZE;
  BufferSize = sizeof(SRB_IO_CONTROL) + DataSize;
  W9xBufferSize = IDENTIFY_BUFFER_SIZE + 16;
var
  hDevice: THandle;
  cbBytesReturned: DWORD;
  pInData: PSendCmdInParams;
  pOutData: Pointer;                                                                                // PSendCmdOutParams
  Buffer: array[0..BufferSize - 1] of Byte;
  srbControl: TSrbIoControl absolute Buffer;

  procedure ChangeByteOrder(var Data; Size: Integer);
  var
    ptr: PChar;
    i: Integer;
    c: Char;
  begin
    ptr := @Data;
    for i := 0 to (Size shr 1) - 1 do
    begin
      c := ptr^;
      ptr^ := (ptr + 1)^;
      (ptr + 1)^ := c;
      Inc(ptr, 2);
    end;
  end;

begin
  Result := False;
  FillChar(Buffer, BufferSize, #0);
  if Win32Platform = VER_PLATFORM_WIN32_NT then
  begin                                                                                             // Windows NT, Windows 2000
// Get SCSI port handle
    hDevice := CreateFile('\\.\Scsi0:',
      GENERIC_READ or GENERIC_WRITE,
      FILE_SHARE_READ or FILE_SHARE_WRITE,
      nil, OPEN_EXISTING, 0, 0);
    if hDevice = INVALID_HANDLE_VALUE then Exit;
    try
      srbControl.HeaderLength := sizeof(SRB_IO_CONTROL);
      System.Move('SCSIDISK', srbControl.Signature, 8);
      srbControl.Timeout := 2;
      srbControl.Length := DataSize;
      srbControl.ControlCode := IOCTL_SCSI_MINIPORT_IDENTIFY;
      pInData := PSendCmdInParams(PChar(@Buffer)
        + sizeof(SRB_IO_CONTROL));
      pOutData := pInData;
      with pInData^ do
      begin
        cBufferSize := IDENTIFY_BUFFER_SIZE;
        bDriveNumber := 0;
        with irDriveRegs do
        begin
          bFeaturesReg := 0;
          bSectorCountReg := 1;
          bSectorNumberReg := 1;
          bCylLowReg := 0;
          bCylHighReg := 0;
          bDriveHeadReg := $A0;
          bCommandReg := IDE_ID_FUNCTION;
        end;
      end;
      if not DeviceIoControl(hDevice, IOCTL_SCSI_MINIPORT,
        @Buffer, BufferSize, @Buffer, BufferSize,
        cbBytesReturned, nil) then Exit;
    finally
      CloseHandle(hDevice);
    end;
  end
  else
  begin                                                                                             // Windows 95 OSR2, Windows 98
    hDevice := CreateFile('\\.\SMARTVSD', 0, 0, nil,
      CREATE_NEW, 0, 0);
    if hDevice = INVALID_HANDLE_VALUE then Exit;
    try
      pInData := PSendCmdInParams(@Buffer);
      pOutData := @pInData^.bBuffer;
      with pInData^ do
      begin
        cBufferSize := IDENTIFY_BUFFER_SIZE;
        bDriveNumber := 0;
        with irDriveRegs do
        begin
          bFeaturesReg := 0;
          bSectorCountReg := 1;
          bSectorNumberReg := 1;
          bCylLowReg := 0;
          bCylHighReg := 0;
          bDriveHeadReg := $A0;
          bCommandReg := IDE_ID_FUNCTION;
        end;
      end;
      if not DeviceIoControl(hDevice, DFP_RECEIVE_DRIVE_DATA,
        pInData, sizeof(TSendCmdInParams) - 1, pOutData,
        W9xBufferSize, cbBytesReturned, nil) then Exit;
    finally
      CloseHandle(hDevice);
    end;
  end;
  with PIdSector(PChar(pOutData) + 16)^ do
  begin
    ChangeByteOrder(sSerialNumber, sizeof(sSerialNumber));
    SetString(SerialNumber, sSerialNumber, sizeof(sSerialNumber));                                  // 硬盘生产序号

    ChangeByteOrder(sModelNumber, sizeof(sModelNumber));
    SetString(ModelNumber, sModelNumber, sizeof(sModelNumber));                                     // 硬盘型号

    ChangeByteOrder(sFirmwareRev, sizeof(sFirmwareRev));
    SetString(FirmwareRev, sFirmwareRev, sizeof(sFirmwareRev));                                     // 硬盘硬件版本
    Result := True;
    ChangeByteOrder(ulTotalAddressableSectors, sizeof(ulTotalAddressableSectors));
    TotalAddressableSectors := ulTotalAddressableSectors;                                           // 硬盘ulTotalAddressableSectors参数

    ChangeByteOrder(ulCurrentSectorCapacity, sizeof(ulCurrentSectorCapacity));
    SectorCapacity := ulCurrentSectorCapacity;                                                      // 硬盘wBytesPerSector参数

    ChangeByteOrder(wNumCurrentSectorsPerTrack, sizeof(wNumCurrentSectorsPerTrack));
    SectorsPerTrack := wNumCurrentSectorsPerTrack;                                                  // 硬盘wSectorsPerTrack参数
  end;
end;

function GetWindowsVersion: string;
var
 // windows api structure
  VersionInfo: TOSVersionInfo;
begin
// get size of the structure
  FillChar(VersionInfo, SizeOf(VersionInfo), 0);
  VersionInfo.dwOSVersionInfoSize := SizeOf(VersionInfo);
  // populate the struct using api call
  GetVersionEx(VersionInfo);
  // platformid gets the core platform
  // major and minor versions also included.

  case VersionInfo.dwPlatformid of
    0:
      begin
        result := 'Windows 3.11';
      end;                                                                                        // end 0

    1:
      begin
        case VersionInfo.dwMinorVersion of
          0: result := 'Windows 95';
          10:
            begin
              if (VersionInfo.szCSDVersion[1] = 'A') then
                Result := 'Windows 98 SE'
              else
                Result := 'Windows 98';
            end;                                                                                  // end 10
          90: result := 'Windows Millenium';
        else
          result := 'Unknown Version';
        end;                                                                                      // end case
      end;                                                                                        // end 1

    2:
      begin
        case VersionInfo.dwMajorVersion of
          3, 4: result := 'Windows NT ' +
            IntToStr(VersionInfo.dwMajorVersion) + '.' +
              IntToStr(VersionInfo.dwMinorVersion);
          5:
            begin
              case VersionInfo.dwMinorVersion of
                0: result := 'Windows 2000';
                1: result := 'Windows Whistler';
              end;                                                                                // end case
            end;                                                                                  // end 5
        else
          result := 'Unknown Version';
        end;                                                                                      // end case
       // service packs apply to the NT/2000 platform
        if VersionInfo.szCSDVersion <> '' then
          result := result + ' Service pack: ' + VersionInfo.szCSDVersion;
      end;                                                                                        // end 2
  else
    result := 'Unknown Platform';
  end;                                                                                            // end case
   // add build info.
  result := result + ', Build: ' +
    IntToStr(Loword(VersionInfo.dwBuildNumber));
                                                                                            // end version info
end;                                                                                                // GetWindowsVersion



function GetAdapterMac(ANo: Integer): string;
// 获取网卡的MAC地址
var
  Ncb: TNcb;
  Adapter: TAdapterStatus;
  Lanaenum: TLanaenum;
  IntIdx: Integer;                                                                                  //
  cRc: AnsiChar;
  StrTemp: string;
begin
  Result := '';
  try
    ZeroMemory(@Ncb, SizeOf(Ncb));
    Ncb.ncb_command := Chr(NCbenum);
    NetBios(@NCb);
    Ncb.ncb_buffer := @Lanaenum;                                                                    // 再处理enum命令
    Ncb.ncb_length := SizeOf(Lanaenum);
    cRc := NetBios(@Ncb);
    if Ord(cRc) <> 0 then exit;
    ZeroMemory(@Ncb, SizeOf(Ncb));                                                                  // 适配器清零
    Ncb.ncb_command := Chr(NcbReset);
    Ncb.ncb_lana_num := Lanaenum.lana[aNo];
    cRc := NetBios(@Ncb);
    if Ord(cRc) <> 0 then exit;
    // 得到适配器状态
    ZeroMemory(@Ncb, SizeOf(Ncb));
    Ncb.ncb_command := Chr(NcbAstat);
    Ncb.ncb_lana_num := Lanaenum.lana[aNo];
    {$IF CompilerVersion >= 22}System.AnsiStrings.{$IFEND}StrPcopy(Ncb.ncb_callname, '*');
    Ncb.ncb_buffer := @Adapter;
    Ncb.ncb_length := SizeOf(Adapter);
    NetBios(@Ncb);
    // 将mac地址转换成字符串输出
    StrTemp := '';

    for IntIdx := 0 to 5 do
      StrTemp := StrTemp + IntToHex(Integer(Adapter.adapter_address[intIdx]), 2);
    Result := StrTemp;
  finally

  end;
end;

function GetNetCardName: string;
  function RegEnum(RootKey: HKEY; Name: string; var ResultList: string; const DoKeys: Boolean): Boolean;
  var//枚举
    i: Integer;
    iRes: Integer;
    s: string;
    hTemp: HKEY;
    Buf: Pointer;
    BufSize: Cardinal;
  begin
    Result := False;
    ResultList := '';
    if RegOpenKeyEx(RootKey, PChar(Name), 0, KEY_READ, hTemp)=ERROR_SUCCESS then
    begin
      Result := True;
      BufSize := 1024;
      GetMem(Buf, BufSize);
      i := 0;

      repeat
        BufSize := 1024;
        if DoKeys then
          iRes := RegEnumKeyEx(hTemp, i, Buf, BufSize, nil, nil, nil, nil)
        else
          iRes := RegEnumValue(hTemp, i, Buf, BufSize, nil, nil, nil, nil);
        if iRes = ERROR_SUCCESS then
        begin
          SetLength(s, BufSize);
          Move(Buf^, s[1], BufSize);
          if ResultList = '' then
            ResultList := s
          else
            ResultList := Concat(ResultList, #13#10, s);
          inc(i);
        end;
      until iRes <> ERROR_SUCCESS;

      FreeMem(Buf);
      RegCloseKey(hTemp);
    end;
  end;
const
  RootKey = HKEY_LOCAL_MACHINE;
  NetKey  = 'SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkCards\';
var
  Reg: TRegistry;
  i: Integer;
  netListText: string;
  netList: TStrings;
begin
  RegEnum(RootKey, NetKey, netListText, True);
  netList := TStringList.Create;
  netList.Text := netListText;
  Reg := TRegistry.Create;
  Reg.RootKey := RootKey;
  for i := 0 to netList.Count-1 do
  begin
    if Reg.OpenKey(NetKey+netList.Strings[i], False) then
    begin
      Result := Reg.ReadString('ServiceName');
      Reg.CloseKey;
      Break;
    end;
  end;
  Reg.Free;
  netList.Free;
end;

function GetNetCardMac(NetCardName: string): string;
const
  OID_802_3_PERMANENT_ADDRESS: Integer   = $01010101;
  OID_802_3_CURRENT_ADDRESS: Integer     = $01010102;
  IOCTL_NDIS_QUERY_GLOBAL_STATS: Integer = $00170002;
var
  hDevice: THandle;
  inBuf: Integer;
  outBuf: array[1..256] of Byte;
  BytesReturned: DWORD;
  MacAddr: string;
  i: Integer;
begin
  inBuf := OID_802_3_PERMANENT_ADDRESS;
  Result := '';
  hDevice := INVALID_HANDLE_VALUE;
  try
    hDevice := CreateFile(PChar('\\.\' + NetCardName),
      GENERIC_READ or GENERIC_WRITE, FILE_SHARE_READ
      or FILE_SHARE_WRITE, nil, OPEN_EXISTING, 0, 0);
    if hDevice <> INVALID_HANDLE_VALUE then
    begin
      if DeviceIoControl(hDevice,
        IOCTL_NDIS_QUERY_GLOBAL_STATS,
        @inBuf, 4, @outBuf, 256, BytesReturned, nil) then
      begin
        MacAddr := '';
        for i := 1 to BytesReturned do
        begin
          MacAddr := MacAddr + IntToHex(outBuf[i], 2);
        end;
        Result := MacAddr;
      end;
    end;
  finally
    if hDevice <> INVALID_HANDLE_VALUE then
      CloseHandle(hDevice);
  end;
end;

function FormatMac(AMac: string): string;
var
  I:Integer;
  nMac, v: string;
begin
  Result:='';
  nMac:=AMac;
  for i := 1 to 6 do
  begin
    v := LeftStr(nMac, 2);
    nMac := RightStr(nMac, Length(nMac) - 2);
    if i = 1 then
      Result := v
    else
      Result := Result + '-' + v;
  end; 
end;

{
function GetHwid2: string;
var
  DiskInfo: TDiskDriveInfo;
  BiosInfo: TBiosInfo;
  CpuInfo: TProcessorInfo;
  CSysInfo: TComputerSystemInfo;
  S: string;
begin
  DiskInfo := TDiskDriveInfo.Create(nil);
  BiosInfo := TBiosInfo.Create(nil);
  CpuInfo  := TProcessorInfo.Create(nil);
  CSysInfo := TComputerSystemInfo.Create(nil);
  try
    DiskInfo.Active := True;
    BiosInfo.Active := True;
    CpuInfo.Active := True;
    CSysInfo.Active := True;

    S := // BIOS信息
         BiosInfo.BiosProperties.Manufacturer +                     // 制造商
         BiosInfo.BiosProperties.SoftwareElementID +                // 软件ID
         IntToStr(Trunc(BiosInfo.BiosProperties.ReleaseDate)) +     // 发行日期
         BiosInfo.BiosProperties.SMBIOSBIOSVersion +                // F1（CMOS进入键）
         IntToStr(BiosInfo.BiosProperties.SMBIOSMajorVersion) +     // 主版本
         IntToStr(BiosInfo.BiosProperties.SMBIOSMinorVersion) +     // 次版本

         // 主板信息
         CSysInfo.ComputerSystemProperties.Manufacturer +           // 主板制造商
         CSysInfo.ComputerSystemProperties.Model +                  // 主板型号

         // 硬盘信息
         DiskInfo.DiskDriveProperties.Caption +                     // 硬盘类型
         IntToStr(DiskInfo.DiskDriveProperties.BytesPerSector) +    // 每扇区多少字节
         IntToStr(DiskInfo.DiskDriveProperties.SectorsPerTrack) +   // 每磁道多个扇区
         IntToStr(DiskInfo.DiskDriveProperties.TotalTracks) +       // 共有多少磁道\
         DiskInfo.DiskDriveProperties.SerialNumber +

         // CPU信息
         CpuInfo.ProcessorProperties.Manufacturer +                 // CPU制造商
         CpuInfo.ProcessorProperties.Caption +                      // CPU类型
         CpuInfo.ProcessorProperties.SocketDesignation +            // CPU接口类型
         CpuInfo.ProcessorProperties.ProcessorId +                  // CPU ID
         IntToStr(CpuInfo.ProcessorProperties.Revision) +           // CPU版本号
         IntToStr(CpuInfo.ProcessorProperties.L2CacheSize) +        // 二级缓存大小
         IntToStr(CpuInfo.ProcessorProperties.NumberOfCores);       // 内核数量

         // MAC地址
         // FormatMac(GetNetCardMac(GetNetCardName));
    Result := RivestStr(S);
  finally
    BiosInfo.Free;
    DiskInfo.Free;
    CpuInfo.Free;
    CSysInfo.Free;
  end;
end;

initialization
  CoInitialize(nil);

finalization
  CoUninitialize();

}
end.
