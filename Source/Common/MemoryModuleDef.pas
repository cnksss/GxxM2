unit MemoryModuleDef;

interface

uses
  Windows, Classes;

const
  IMAGE_FILE_MACHINE_AMD64 = $08664;

{$IFDEF WIN64}
  HOST_MACHINE = IMAGE_FILE_MACHINE_AMD64;
{$ELSE}
  HOST_MACHINE = IMAGE_FILE_MACHINE_I386;
{$ENDIF}

const
  IMAGE_REL_BASED_ABSOLUTE          =   0;
  IMAGE_REL_BASED_HIGH              =   1;
  IMAGE_REL_BASED_LOW               =   2;
  IMAGE_REL_BASED_HIGHLOW           =   3;
  IMAGE_REL_BASED_HIGHADJ           =   4;
  IMAGE_REL_BASED_MIPS_JMPADDR      =   5;
  IMAGE_REL_BASED_SECTION           =   6;
  IMAGE_REL_BASED_REL32             =   7;
  IMAGE_REL_BASED_MIPS_JMPADDR16    =   9;
  IMAGE_REL_BASED_IA64_IMM64        =   9;
  IMAGE_REL_BASED_DIR64             =   10;
  IMAGE_REL_BASED_HIGH3ADJ          =   11;


  IMAGE_ORDINAL_FLAG32 = DWORD($80000000);
  IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
  IMAGE_DIRECTORY_ENTRY_BASERELOC = 5;
  IMAGE_SCN_LNK_NRELOC_CVFL = $01000000;
  IMAGE_SCN_MEM_DISCARDABLE = $02000000;
  IMAGE_SCN_MEM_NOT_CACHED = $04000000;
  IMAGE_SCN_MEM_NOT_PAGED = $08000000;
  IMAGE_SCN_MEM_NOT_SHARED = $10000000;
  IMAGE_SCN_MEM_EXECUTE = $20000000;
  IMAGE_SCN_MEM_READ = $40000000;
  IMAGE_SCN_MEM_WRITE = DWORD($80000000);
  IMAGE_SCN_CNT_INITIALIZED_DATA = $00000040;
  IMAGE_SCN_CNT_UNINITIALIZED_DATA = $00000080;
  IMAGE_DIRECTORY_ENTRY_EXPORT = 0;

type
  // D7 下面没有 ULONG_PTR, UINT_PTR
  {$if CompilerVersion <= 18.5}     
    Int32   = Integer;
    IntPtr  = NativeInt;
    UIntPtr = NativeUInt;

    PUInt64 = ^UInt64;

    INT_PTR = IntPtr;
    LONG_PTR = NativeInt;
    UINT_PTR = UIntPtr;

    SIZE_T = NativeInt;
    SSIZE_T = NativeInt;
  {$ifend}

  PMemoryModule = ^TMemoryModule;
  TMemoryModule = packed record
    Headers: PImageNtHeaders;
    CodeBase: Pointer;
    ModuleList: TList;
    PageSize: SIZE_T;
    IsInitialized: BOOL;
  end;

  PImageExportDirectory = ^TImageExportDirectory;
  _IMAGE_EXPORT_DIRECTORY = packed record
    Characteristics: DWORD;
    TimeDateStamp: DWORD;
    MajorVersion: Word;
    MinorVersion: Word;
    Name: DWORD;
    Base: DWORD;
    NumberOfFunctions: DWORD;
    NumberOfNames: DWORD;
    AddressOfFunctions: DWORD;              // RVA from base of image
    AddressOfNames: DWORD;                  // RVA from base of image
    AddressOfNameOrdinals: DWORD;           // RVA from base of image
  end;
  TImageExportDirectory = _IMAGE_EXPORT_DIRECTORY;
  IMAGE_EXPORT_DIRECTORY = _IMAGE_EXPORT_DIRECTORY;

  PImageDosHeader = ^TImageDosHeader;
  _IMAGE_DOS_HEADER = packed record
    e_magic: Word;
    e_cblp: Word;
    e_cp: Word;
    e_crlc: Word;
    e_cparhdr: Word;
    e_minalloc: Word;
    e_maxalloc: Word;
    e_ss: Word;
    e_sp: Word;
    e_csum: Word;
    e_ip: Word;
    e_cs: Word;
    e_lfarlc: Word;
    e_ovno: Word;
    e_res: array [0..3] of Word;
    e_oemid: Word;
    e_oeninfo: Word;
    e_res2: array [0..9] of Word;
    _lfanew: LongInt;
  end;
  TImageDosHeader = _IMAGE_DOS_HEADER;
  IMAGE_DOS_HEADER = _IMAGE_DOS_HEADER;

  PImageSectionHeader = ^TImageSectionHeader;
  _IMAGE_SECTION_HEADER = packed record
    Name: packed array [0 .. IMAGE_SIZEOF_SHORT_NAME - 1] of Byte;
    Misc: TISHMisc;
    VirtualAddress: DWORD;
    SizeOfRawData: DWORD;
    PointerToRawData: DWORD;
    PointerToRelocations: DWORD;
    PointerToLinenumbers: DWORD;
    NumberOfRelocations: Word;
    NuberOfLinenumbers: Word;
    Characteristics: DWORD;
  end;
  TImageSectionHeader = _IMAGE_SECTION_HEADER;
  IMAGE_SECTION_HEADER = _IMAGE_SECTION_HEADER;

  PImageBaseRelocation = ^TImageBaseRelocation;
  _IMAGE_BASE_RELOCATION = packed record
    VirtualAddress: DWORD;
    SizeOfBlock: DWORD;
  end;
  TImageBaseRelocation = _IMAGE_BASE_RELOCATION;
  IMAGE_BASE_RELOCATION = _IMAGE_BASE_RELOCATION;

  PImageImportDescriptor = ^TImageImportDescriptor;
  _IMAGE_IMPORT_DESCRIPTOR = packed record
    OriginalFirstThunk: DWORD;
    TimeDateStamp: DWORD;
    ForwarderChain: DWORD;
    Name: DWORD;
    FirstThunk: DWORD;
  end;
  TImageImportDescriptor = _IMAGE_IMPORT_DESCRIPTOR;
  IMAGE_IMPORT_DESCRIPTOR = _IMAGE_IMPORT_DESCRIPTOR;

  PImageImportByName = ^TImageImportByName;
  _IMAGE_IMPORT_BY_NAME = packed record
    Hint: Word;
    Name: array [0..255] of Byte; // original: "Name: array [0..0] of Byte;"
  end;
  TImageImportByName = _IMAGE_IMPORT_BY_NAME;
  IMAGE_IMPORT_BY_NAME = _IMAGE_IMPORT_BY_NAME;

  PSectionFinalizeData = ^TSectionFinalizeData;
  _SECTION_FINALIZE_DATA = packed record
    Address: Pointer;
    AlignedAddress: Pointer;
    Size: SIZE_T;
    Characteristics: DWORD;
    IsLast: BOOL;
  end;
  TSectionFinalizeData = _SECTION_FINALIZE_DATA;
  SECTION_FINALIZE_DATA = _SECTION_FINALIZE_DATA;

  PImageTlsDirectory32 = ^IMAGE_TLS_DIRECTORY32;
  _IMAGE_TLS_DIRECTORY32 = record
    StartAddressOfRawData: DWORD;
    EndAddressOfRawData: DWORD;
    AddressOfIndex: DWORD;             // PDWORD
    AddressOfCallBacks: DWORD;         // PIMAGE_TLS_CALLBACK *
    SizeOfZeroFill: DWORD;
    Characteristics: DWORD;
  end;
  TImageTlsDirectory32 = _IMAGE_TLS_DIRECTORY32;
  IMAGE_TLS_DIRECTORY32 = _IMAGE_TLS_DIRECTORY32;

  PImageTlsDirectory64 = ^IMAGE_TLS_DIRECTORY64;
  _IMAGE_TLS_DIRECTORY64 = record
    StartAddressOfRawData: ULONGLONG;
    EndAddressOfRawData: ULONGLONG;
    AddressOfIndex: ULONGLONG;         // PDWORD
    AddressOfCallBacks: ULONGLONG;     // PIMAGE_TLS_CALLBACK *;
    SizeOfZeroFill: DWORD;
    Characteristics: DWORD;
  end;
  TImageTlsDirectory64 = _IMAGE_TLS_DIRECTORY64;
  IMAGE_TLS_DIRECTORY64 = _IMAGE_TLS_DIRECTORY64;

type
  PIMAGE_TLS_CALLBACK = procedure (DllHandle: Pointer; Reason: DWORD; Reserved: Pointer); stdcall;
  TImageTlsCallback = PIMAGE_TLS_CALLBACK;

  TDllEntryProc = function(hInstDLL: THandle; dwReason: DWORD; lpReserved: Pointer): BOOL; stdcall;
  PDllEntryProc = ^TDllEntryProc;

implementation

end.
