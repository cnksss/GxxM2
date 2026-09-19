unit MemoryModule;

interface

uses
  Windows, Classes, SysUtils, MemoryModuleDef
  {$IF CompilerVersion >= 22}, System.AnsiStrings{$IFEND}
  {$IFDEF CPUX64},VMProtectSDK{$ENDIF};

  function MemoryLoadLibary(MemData: Pointer; const MemSize: SIZE_T;
    var RunCode: Integer;
    IsProcessAttach: Boolean = True): PMemoryModule; stdcall;

  procedure MemoryFreeLibrary(Module: PMemoryModule;
    IsProcessDetach: Boolean = True); stdcall;

  function MemoryGetProcAddress(Module: PMemoryModule;
    const Name: PAnsiChar): Pointer; stdcall; overload;

  function MemoryGetProcAddress(Module: PMemoryModule;
    const Index: Integer): Pointer; stdcall; overload;

implementation

{$IFDEF FPC}
  {$mode delphi}
  {$IFDEF CPU64}
    {$DEFINE WIN64}
  {$ENDIF}
{$ENDIF}

function AlignValueUp(Value, Alignment: SIZE_T): SIZE_T; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := (Value + Alignment - 1) div Alignment * Alignment;
end;

function AlignValueDown(Value, Alignment: SIZE_T): SIZE_T; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := Value div Alignment * Alignment;
end;

function CheckSize(ASize, ExpectedSize: SIZE_T): Boolean; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := ASize >= ExpectedSize;
end;

function GetFieldOffset(const Struc; const Field): Cardinal; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := IntPtr(@Field) - IntPtr(@Struc);
end;

function OffsetPointer(Data: Pointer; Offset: SIZE_T): Pointer; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := Pointer(IntPtr(Data) + Offset);
end;

function GetImageFirstSection(NtHeaders: PImageNtHeaders): PImageSectionHeader; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := PImageSectionHeader(
    IntPtr(NtHeaders) +
    GetFieldOffset(NtHeaders^, NtHeaders^.OptionalHeader) +
    NtHeaders^.FileHeader.SizeOfOptionalHeader
  );
end;

function GetHeaderDictionary(Module: PMemoryModule; Index: Integer): PImageDataDirectory; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := PImageDataDirectory(@(Module.headers.OptionalHeader.DataDirectory[Index]));
end;

function GetImageSnapByOrdinal(Ordinal: SIZE_T): boolean; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := (Ordinal and IMAGE_ORDINAL_FLAG32) <> 0;
end;

function GetImageOrdinal(Ordinal: SIZE_T): Word; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := Ordinal and $FFFF;
end;

function GetRealSectionSize(Module: PMemoryModule; Section: PImageSectionHeader): SIZE_T; {$IF CompilerVersion >= 22} inline; {$IFEND}
begin
  Result := Section.SizeOfRawData;
  if Result = 0 then
  begin
    if (Section.Characteristics and IMAGE_SCN_CNT_INITIALIZED_DATA <> 0) then
      Result := Module.Headers.OptionalHeader.SizeOfInitializedData
    else if Section.Characteristics and IMAGE_SCN_CNT_UNINITIALIZED_DATA <> 0 then
      Result := Module.Headers.OptionalHeader.SizeOfUninitializedData
  end;
end;

function GetAllSectionSize(NtHeaders: PImageNtHeaders): SIZE_T;
var
  I: Integer;
  Section: PImageSectionHeader;
  EndSection: SIZE_T;
begin
  Result := 0;
  
  Section := GetImageFirstSection(NtHeaders);
  for I := 0 to NtHeaders.FileHeader.NumberOfSections - 1 do
  begin
    if (Section.SizeOfRawData = 0) then
      EndSection := Section.VirtualAddress + NtHeaders.OptionalHeader.SectionAlignment
    else
      EndSection := Section.VirtualAddress + Section.SizeOfRawData;

    // 2021-01-13
    if EndSection > Result then
    begin
      Result := EndSection;
    end;

    Inc(Section);
  end;
end;

procedure CopySections(MemData: Pointer; NtHeaders: PImageNtHeaders; Module: PMemoryModule);
var
  Section: PImageSectionHeader;
  I: Integer;
  SectionSize: DWORD;
  PtrDest: Pointer;
begin
  Section := GetImageFirstSection(Module.Headers);
  for I := 0 to Module.Headers.FileHeader.NumberOfSections - 1 do
  begin
    if (Section.SizeOfRawData = 0) then
      SectionSize := NtHeaders.OptionalHeader.SectionAlignment
    else
      SectionSize := Section.SizeOfRawData;

    if SectionSize > 0 then
    begin
      // 内存已经分配过，这里不用再分配了
      {
      PtrDest := VirtualAlloc(
        OffsetPointer(Module.CodeBase, Section.VirtualAddress),
        SectionSize,
        MEM_COMMIT,
        PAGE_EXECUTE_READWRITE);
      }

      PtrDest := OffsetPointer(Module.CodeBase, Section.VirtualAddress);
      Section^.Misc.PhysicalAddress := DWORD(PtrDest);

      if Section.SizeOfRawData = 0 then
        ZeroMemory(PtrDest, SectionSize)
      else
        CopyMemory(PtrDest, OffsetPointer(MemData, Section.PointerToRawData), Section.SizeOfRawData);
    end;

    Inc(Section);
  end;
end;

procedure PerformBaseRelocation(Module: PMemoryModule; RelocationOffset: Cardinal);
var
  I, Count: Integer;
  Directory: PImageDataDirectory;
  Relocation: PImageBaseRelocation;
  Dest: Pointer;
  PatchAddrHL: PDWORD;
{$IFDEF WIN64}
  PatchAddrHL64: PUInt64;
{$ENDIF}
  RelInfo: PWORD;
  nType, nOffset: Integer;
begin
  Directory := GetHeaderDictionary(Module, IMAGE_DIRECTORY_ENTRY_BASERELOC);

  if Directory.Size > 0 then
  begin
    Relocation := OffsetPointer(Module.CodeBase, Directory.VirtualAddress);
    while Relocation.VirtualAddress > 0 do
    begin
      Dest := OffsetPointer(Module.CodeBase, Relocation.VirtualAddress);
      RelInfo := OffsetPointer(Relocation, SizeOf(TImageBaseRelocation));

      Count := (Relocation.SizeOfBlock - SizeOf(TImageBaseRelocation)) shr 1;     // (Relocation^.SizeOfBlock - SizeOf(TImageBaseRelocation)) div 2
      for I := 0 to Count - 1 do
      begin
        try
          // the upper 4 bits define the type of relocation
          nType := (RelInfo^ shr 12);
          // the lower 12 bits define the offset
          nOffset := RelInfo^ and $FFF;

          // 重定向在偏移为负值时，会有问题

          case nType of
            IMAGE_REL_BASED_HIGHLOW:
              begin
                PatchAddrHL := OffsetPointer(Dest, nOffset);
                PatchAddrHL^ := PatchAddrHL^ + RelocationOffset;
              end;
          {$IFDEF WIN64}
            IMAGE_REL_BASED_DIR64:
              begin
                PatchAddrHL64 := OffsetPointer(Dest, nOffset);
                PatchAddrHL64^ := PatchAddrHL64^ + RelocationOffset;
              end;
          {$ENDIF}
          end;
        except
          OutputDebugString(PChar(IntToStr(I)));
        end;

        Inc(RelInfo);
      end;

      Relocation := OffsetPointer(Relocation, Relocation.SizeOfBlock);
    end;
  end;
end;

(*
function PAnsiCharToPChar(Addr: Pointer): PChar;
begin
{$IFDEF UNICODE}
  Result := StringToOleStr(PAnsiChar(Addr));
{$ELSE}
  Result := PAnsiChar(Addr);
{$ENDIF}
end;
*)

function BuildImportTable(Module: PMemoryModule): Boolean; stdcall;
var
  Directory: PImageDataDirectory;
  ImportDesc: PImageImportDescriptor;
  Handle: HMODULE;
{$IFDEF WIN64}
  ThunkRef: PUINT_PTR;
{$ELSE}
  ThunkRef: PDWORD;
{$ENDIF}
  FuncRef: ^FARPROC;
  ThunkData: PImageImportByName;
begin
  Result := True;
  Directory := GetHeaderDictionary(Module, IMAGE_DIRECTORY_ENTRY_IMPORT);
  if (Directory = nil) or (Directory.Size = 0) then Exit;

  ImportDesc := OffsetPointer(Module.CodeBase, Directory.VirtualAddress);
  while (not IsBadReadPtr(ImportDesc, SizeOf(TImageImportDescriptor))) and (ImportDesc.Name <> 0) do
  begin
    Handle := LoadLibraryA(OffsetPointer(Module.CodeBase, ImportDesc.Name));
    if (Handle = INVALID_HANDLE_VALUE) then
    begin
      Result := False;
      SetLastError(ERROR_MOD_NOT_FOUND);
      Exit;
    end;

    if ImportDesc.OriginalFirstThunk <> 0 then
    begin
      // 注：导入表双桥结构，OriginalFirstThunk指向INT表，FirstThunk指向IAT表，最终两个表中的表项指向同一个函数地址
      ThunkRef := OffsetPointer(Module.CodeBase, ImportDesc.OriginalFirstThunk);
      FuncRef := OffsetPointer(Module.CodeBase, ImportDesc.FirstThunk);
    end
    else
    begin
      // 无INT,有的程序只保留一个桥，如Borland公司的Tlink只保留桥2
      ThunkRef := OffsetPointer(Module.CodeBase, ImportDesc.FirstThunk);
      FuncRef := OffsetPointer(Module.CodeBase, ImportDesc.FirstThunk);
    end;

    while ThunkRef^ <> 0 do
    begin
      if GetImageSnapByOrdinal(ThunkRef^) then
        FuncRef^ := GetProcAddress(Handle, PAnsiChar(GetImageOrdinal(ThunkRef^)))
      else
      begin
        ThunkData := OffsetPointer(Module.CodeBase, ThunkRef^);
        FuncRef^ := GetProcAddress(Handle, PAnsiChar(@(ThunkData.Name)));
      end;

      if FuncRef^ = nil then
      begin
        Result := False;
        Break;
      end;

      Inc(FuncRef);
      Inc(ThunkRef);
    end;

    if not Result then
    begin
      FreeLibrary(Handle);
      SetLastError(ERROR_PROC_NOT_FOUND);
      Break;
    end
    else
    begin
      Module.ModuleList.Add(Pointer(Handle));
    end;

    Inc(ImportDesc);
  end;
end;

function FinalizeSection(Module: PMemoryModule; SectionData: PSectionFinalizeData): Boolean;
const
  PROTECTION_FLAGS: array [Boolean {执行}, Boolean {读}, Boolean {写}] of DWORD = (
    (
        // not executable
        (PAGE_NOACCESS, PAGE_WRITECOPY),
        (PAGE_READONLY, PAGE_READWRITE)
    ), (
        // executable
        (PAGE_EXECUTE, PAGE_EXECUTE_WRITECOPY),
        (PAGE_EXECUTE_READ, PAGE_EXECUTE_READWRITE)
    )
  );
var
  NewProtect, OldProtect: DWORD;
  IsExecutable: BOOL;
  IsReadable, IsWriteable: BOOL;
begin
  Result := True;
  if SectionData.Size = 0 then Exit;

  // 不要这一段，内存已经开辟，没必要释放，保留没问题
  
  if (SectionData.Characteristics and IMAGE_SCN_MEM_DISCARDABLE <> 0) then
  begin
    // 不再需要部分，可以安全地释放
    if (SectionData.Address = SectionData.AlignedAddress) and
      (
        SectionData.IsLast or
        (Module.Headers.OptionalHeader.SectionAlignment = Module.PageSize) or
        (SectionData.Size mod Module.PageSize = 0)
      ) then
    begin
      // Only allowed to decommit whole pages
      VirtualFree(SectionData.Address, SectionData.Size, MEM_DECOMMIT);
    end;

    Exit;
  end;

  // 根据特征确定保护标志
  IsExecutable := (SectionData.Characteristics and IMAGE_SCN_MEM_EXECUTE) <> 0;
  IsReadable := (SectionData.Characteristics and IMAGE_SCN_MEM_READ) <> 0;
  IsWriteable := (SectionData.Characteristics and IMAGE_SCN_MEM_WRITE) <> 0;

  NewProtect := PROTECTION_FLAGS[IsExecutable, IsReadable, IsWriteable];
  if (SectionData.Characteristics and IMAGE_SCN_MEM_NOT_CACHED <> 0) then
  begin
    NewProtect := NewProtect or PAGE_NOCACHE;
  end;

  // 更改内存访问标志
  Result := VirtualProtect(SectionData.Address, SectionData.Size, NewProtect, OldProtect);
end;

function FinalizeSections(Module: PMemoryModule): Boolean;
var
  ImageOffset: SIZE_T;
  I: Integer;
  Section: PImageSectionHeader;

  SectionData: TSectionFinalizeData;
begin
{$IFDEF WIN64}
  // "PhysicalAddress" might have been truncated to 32bit above, expand to // 64bits again.
  ImageOffset := (Module.Headers.OptionalHeader.ImageBase and $ffffffff00000000);
{$ELSE}
  ImageOffset := 0;
{$ENDIF}

  // 取第一个区段
  Section := GetImageFirstSection(Module.Headers);
  for I := 1 to Module.Headers.FileHeader.NumberOfSections - 1 do
  begin
    SectionData.Address := Pointer(Section.Misc.PhysicalAddress or ImageOffset);
    SectionData.AlignedAddress := Pointer(AlignValueDown(SiZE_T(SectionData.Address), Module.PageSize));
    SectionData.Size := GetRealSectionSize(module, Section);
    SectionData.Characteristics := Section.Characteristics;
    SectionData.IsLast := False;

    FinalizeSection(Module, @SectionData);

    Inc(Section);
  end;

  Result := True;
end;

procedure ExecuteTLS(Module: PMemoryModule);
var
  TlsDir: {$IFDEF WIN64} PImageTlsDirectory64 {$ELSE}PImageTlsDirectory32{$ENDIF};
  Callback: PPointer;
  Directory: PImageDataDirectory;
  CodeBase: Pointer;

  // TLS callback pointers are VA's (ImageBase included) so if the module resides at
  // the other ImageBage they become invalid. This routine relocates them to the
  // actual ImageBase.
  // The case seem to happen with DLLs only and they rarely use TLS callbacks.
  // Moreover, they probably don't work at all when using DLL dynamically which is
  // the case in our code.
  function FixPtr(OldPtr: Pointer): Pointer;
  begin
    Result := Pointer(NativeInt(OldPtr) - Module.Headers.OptionalHeader.ImageBase + NativeInt(CodeBase));
  end;
begin
  CodeBase := Module.CodeBase;

  Directory := GetHeaderDictionary(Module, IMAGE_DIRECTORY_ENTRY_TLS);
  if (Directory.VirtualAddress = 0) then Exit;

  TlsDir := {$IFDEF WIN64} PImageTlsDirectory64 {$ELSE}PImageTlsDirectory32{$ENDIF}(OffsetPointer(CodeBase, Directory.VirtualAddress));

  Callback := Pointer(TlsDir.AddressOfCallBacks);
  if Callback <> nil then
  begin
    Callback := FixPtr(Callback);
    while Callback^ <> nil do
    begin
      PIMAGE_TLS_CALLBACK(FixPtr(Callback^))(CodeBase, DLL_PROCESS_ATTACH, nil);
      Inc(Callback);
    end;
  end;
end;

function MemoryLoadLibary(MemData: Pointer; const MemSize: SIZE_T;
  var RunCode: Integer;
  IsProcessAttach: Boolean): PMemoryModule; stdcall;
var
  Module: PMemoryModule;
  DosHeader: PImageDosHeader;
  NtHeaders: PImageNtHeaders;

  SysInfo: TSystemInfo;
  ImageAlignedSize: SIZE_T;

  Code: Pointer;

  LocationOffset: IntPtr;

  DllEntry: TDllEntryProc;
  IsOK: Boolean;
begin
{$IFDEF PRIVATE_CLIENT}
  {$I AddVmpFeatureCode.inc}
{$ENDIF}

  Module := nil;
  Result := nil;
  RunCode := 0;

  if not CheckSize(MemSize, SizeOf(TImageDosHeader)) then
  begin
    SetLastError(ERROR_INVALID_DATA);
    Exit;
  end;

  RunCode := 1;
  DosHeader := PImageDosHeader(MemData);
  if (DosHeader.e_magic <> IMAGE_DOS_SIGNATURE) then
  begin
    SetLastError(ERROR_BAD_EXE_FORMAT);
    Exit;
  end;

  RunCode := 2;
  if not CheckSize(MemSize, DosHeader._lfanew + SizeOf(IMAGE_NT_HEADERS)) then
  begin
    SetLastError(ERROR_INVALID_DATA);
    Exit;
  end;

  RunCode := 3;
  NtHeaders := PImageNtHeaders(IntPtr(MemData) + DosHeader._lfanew);
  if NtHeaders.Signature <> IMAGE_NT_SIGNATURE then
  begin
    SetLastError(ERROR_BAD_EXE_FORMAT);
    Exit;
  end;

  RunCode := 4;
  if (NtHeaders.FileHeader.Machine <> HOST_MACHINE) then
  begin
    SetLastError(ERROR_BAD_EXE_FORMAT);
    Exit;
  end;

  RunCode := 5;
  // SectionAlignment必须是2的N次方(512 - 64K，默认为512)
  if NtHeaders.OptionalHeader.SectionAlignment and 1 <> 0 then
  begin
    SetLastError(ERROR_BAD_EXE_FORMAT);
    Exit;
  end;

  RunCode := 6;
  try
{$IFNDEF PRIVATE_CLIENT}
  {$IFDEF CPUX64}
    VMProtectBegin('VMProtect_MemoryLoadLibary');
  {$ELSE}
    {$I VMProtectBegin.inc}
  {$ENDIF}
{$ENDIF}

    IsOK := True;
    GetSystemInfo(SysInfo);
    ImageAlignedSize := AlignValueUp(NtHeaders.OptionalHeader.SizeOfImage, SysInfo.dwPageSize);
    if ImageAlignedSize <> AlignValueUp(GetAllSectionSize(NtHeaders), SysInfo.dwPageSize) then
    begin
      SetLastError(ERROR_BAD_EXE_FORMAT);
      IsOK := False;;
    end;

    if IsOK then
    begin
      RunCode := 7;
      Module := PMemoryModule(HeapAlloc(GetProcessHeap(), 0, SizeOf(TMemoryModule)));
      if Module = nil then
      begin
        SetLastError(ERROR_OUTOFMEMORY);
        IsOK := False;
      end;

      if IsOK then
      begin
        RunCode := 8;
        Module.CodeBase := nil;
        Module.ModuleList := TList.Create;
        Module.ModuleList.Capacity := 100;
        Module.PageSize := SysInfo.dwPageSize;
        Module.IsInitialized := False;

        RunCode := 9;
        // 一次提交完整的内存大小
        Code := VirtualAlloc(
          Pointer(NtHeaders.OptionalHeader.ImageBase),
          ImageAlignedSize,             // NtHeaders.OptionalHeader.SizeOfImage
          MEM_COMMIT or MEM_RESERVE,    // MEM_RESERVE 2019-01-14 chongchong
          PAGE_EXECUTE_READWRITE);

        if Code = nil then
        begin
          RunCode := 10;
          Code := VirtualAlloc(
            nil,
            ImageAlignedSize,             // NtHeaders.OptionalHeader.SizeOfImage
            MEM_COMMIT or MEM_RESERVE,    // MEM_RESERVE 2019-01-14 chongchong
            PAGE_EXECUTE_READWRITE);

          if Code = nil then
          begin
            RunCode := 9;
            SetLastError(ERROR_OUTOFMEMORY);
            MemoryFreeLibrary(Module, False);
            IsOK := False;
          end;
        end;

        if IsOK then
        begin
          RunCode := 10;
          {
          headers = (unsigned char *)allocMemory(Code,
              old_header->OptionalHeader.SizeOfHeaders,
              MEM_COMMIT,
              PAGE_READWRITE);
          }
          // 复制PE头到新开辟的内存区域
          CopyMemory(Code, MemData, IntPtr(DosHeader._lfanew) + NtHeaders.OptionalHeader.SizeOfHeaders);
          Module.CodeBase := Code;
          Module.Headers := PImageNtHeaders(IntPtr(Code) + DosHeader._lfanew);
          Module.Headers.OptionalHeader.ImageBase := IntPtr(Code);

          RunCode := 11;
          // 拷贝所有区段数据到开辟的内存中
          CopySections(MemData, NtHeaders, Module);

          RunCode := 12;
          // 调整导入数据的基地址
          LocationOffset := IntPtr(Code) - NtHeaders.OptionalHeader.ImageBase;
          if LocationOffset <> 0 then
          begin
            RunCode := 13;
            PerformBaseRelocation(Module, LocationOffset);
          end;

          RunCode := 14;
          // 加载依赖dll，并构建"PEHeader.OptionalHeader.DataDirectory.Image_directory_entry_import"导入表
          if (not BuildImportTable(Module)) then
          begin
            MemoryFreeLibrary(Module, False);
            IsOK := False;
          end;

          if IsOK then
          begin
            RunCode := 15;

            // 根据标记为 discardable 的部分标题和释放部分标记内存页
            if not FinalizeSections(Module) then
            begin
              MemoryFreeLibrary(Module, False);
              Exit;
            end;

            RunCode := 16;
            // 在主加载之前执行TLS回调
            ExecuteTLS(Module);

            RunCode := 17;
            // get entry point of loaded library
            if IsProcessAttach and (Module.Headers.OptionalHeader.AddressOfEntryPoint <> 0) then
            begin
              RunCode := 18;
              @DllEntry := OffsetPointer(Code, Module.Headers.OptionalHeader.AddressOfEntryPoint);

              RunCode := 19;
              // notify library about attaching to process
              if (@DllEntry <> nil) and (not DllEntry(HINST(Code), DLL_PROCESS_ATTACH, nil)) then
              begin
                SetLastError(ERROR_DLL_INIT_FAILED);
                IsOK := False;
              end;
            end;

            if IsOK then
            begin
              Module.IsInitialized := True;
              Result := Module;
            end;
          end;
        end;
      end;
    end;

{$IFNDEF PRIVATE_CLIENT}
  {$IFDEF CPUX64}
    VMProtectEnd();
  {$ELSE}
    {$I VMProtectEnd.inc}
  {$ENDIF}
{$ENDIF}

  except
    MemoryFreeLibrary(Module, False);
  end;
end;

procedure MemoryFreeLibrary(Module: PMemoryModule; IsProcessDetach: Boolean); stdcall;
var
  I: Integer;
  Handle: HMODULE;
  DllEntry: TDllEntryProc;
begin
{$IFDEF PRIVATE_CLIENT}
  {$I AddVmpFeatureCode.inc}
{$ENDIF}

  if Module = nil then Exit;

{$IFNDEF PRIVATE_CLIENT}
  {$IFDEF CPUX64}
    VMProtectBegin('VMProtect_MemoryFreeLibrary');
  {$ELSE}
    {$I VMProtectBegin.inc}
  {$ENDIF}
{$ENDIF}

  if Module.IsInitialized and IsProcessDetach then
  begin
    // notify library about detaching from process
    @DllEntry := OffsetPointer(Module.CodeBase, Module.Headers.OptionalHeader.AddressOfEntryPoint);
    DllEntry(HINST(Module.CodeBase), DLL_PROCESS_DETACH, nil);
  end;

  for I := 0 to Module.ModuleList.Count - 1 do
  begin
    Handle := HMODULE(Module.ModuleList.Items[I]);
    FreeLibrary(Handle);
  end;

  if Module.CodeBase <> nil then
    VirtualFree(Module.CodeBase, 0, MEM_RELEASE);

  Module.ModuleList.Free;
  HeapFree(GetProcessHeap(), 0, Module);

{$IFNDEF PRIVATE_CLIENT}
  {$IFDEF CPUX64}
    VMProtectEnd();
  {$ELSE}
    {$I VMProtectEnd.inc}
  {$ENDIF}
{$ENDIF}
end;

function MemoryGetProcAddress(Module: PMemoryModule; const Name: PAnsiChar): Pointer; stdcall;
var
  I, Index: Integer;
  NameRef: PDWORD;
  Ordinal: PWord;
  Directory: PImageDataDirectory;
  ExportDir: PImageExportDirectory;
  Offset: DWORD;
begin
{$IFDEF PRIVATE_CLIENT}
  {$I AddVmpFeatureCode.inc}
{$ENDIF}

  Result := nil;
  if Module = nil then Exit;

  Directory := GetHeaderDictionary(Module, IMAGE_DIRECTORY_ENTRY_EXPORT);

  // no export table found
  if Directory.Size = 0 then
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  ExportDir := PImageExportDirectory(OffsetPointer(Module.CodeBase, Directory.VirtualAddress));

  // DLL doesn't export anything
  if (ExportDir.NumberOfNames = 0) or (ExportDir.NumberOfFunctions = 0) then
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

{$IFNDEF PRIVATE_CLIENT}
  {$IFDEF CPUX64}
    VMProtectBegin('VMProtect_MemoryGetProcAddress');
  {$ELSE}
    {$I VMProtectBegin.inc}
  {$ENDIF}
{$ENDIF}

  // search function name in list of exported names
  NameRef := OffsetPointer(Module.CodeBase, ExportDir.AddressOfNames);
  Ordinal := OffsetPointer(Module.CodeBase, ExportDir.AddressOfNameOrdinals);
  Index := -1;
  for I := 0 to ExportDir.NumberOfNames - 1 do
  begin
    if {$IF CompilerVersion >= 22}System.AnsiStrings.{$IFEND}StrComp(Name, PAnsiChar(OffsetPointer(Module.CodeBase, NameRef^))) = 0 then
    begin
      Index := Ordinal^;
      Break;
    end;
    Inc(NameRef);
    Inc(Ordinal);
  end;

{$IFNDEF PRIVATE_CLIENT}
  {$IFDEF CPUX64}
    VMProtectEnd();
  {$ELSE}
    {$I VMProtectEnd.inc}
  {$ENDIF}
{$ENDIF}

  // exported symbol not found
  if (Index = -1) then
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  // name <-> Ordinal number don't match
  if (Cardinal(Index) > ExportDir.NumberOfFunctions) then //HZQ
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  Offset := PDWORD(OffsetPointer(Module.CodeBase, ExportDir.AddressOfFunctions + Cardinal(Index) * 4))^;    // 这个必须是DWORD
  Result := OffsetPointer(Module.CodeBase, Offset);
end;

function MemoryGetProcAddress(Module: PMemoryModule; const Index: Integer): Pointer; stdcall;
var
  Directory: PImageDataDirectory;
  ExportDir: PImageExportDirectory;
  Offset: DWORD;
  ExpIndex: Integer;
begin
  Result := nil;
  if Module = nil then Exit;

  Directory := GetHeaderDictionary(Module, IMAGE_DIRECTORY_ENTRY_EXPORT);

  // no export table found
  if Directory.Size = 0 then
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  ExportDir := PImageExportDirectory(OffsetPointer(Module.CodeBase, Directory.VirtualAddress));

  // DLL doesn't export anything
  if (ExportDir.NumberOfNames = 0) or (ExportDir.NumberOfFunctions = 0) then
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  ExpIndex := Cardinal(Index) -  ExportDir.Base;  //HZQ

  // exported symbol not found
  if (ExpIndex < 0) then
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  // name <-> Ordinal number don't match
  if (Cardinal(ExpIndex) > ExportDir.NumberOfFunctions) then //HZQ
  begin
    SetLastError(ERROR_PROC_NOT_FOUND);
    Exit;
  end;

  Offset := PDWORD(OffsetPointer(Module.CodeBase, ExportDir.AddressOfFunctions + Cardinal(ExpIndex) * 4))^;
  Result := OffsetPointer(Module.CodeBase, Offset);
end;

end.
