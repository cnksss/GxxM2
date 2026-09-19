{*******************************************************}
{                                                       }
{       异常处理的一些数据结构 -- 飘云                  }
{                                                       }
{       版权所有 (C) 2013 piaoyun                       }
{                                                       }
{*******************************************************}

unit uExceptionStruct;

interface

uses
  Windows;

type
  PEXCEPTION_POINTERS = ^EXCEPTION_POINTERS;

  //////////////////////////////////////////////////////////////////////////////
  //PJmpInstruction = ^TJmpInstruction;
  TJmpInstruction = packed record
    opCode:Byte;
    distance:Longint;
  end;

  PExcDesc = ^TExcDesc;
  TExcDescEntry = record
    vTable:Pointer;
    handler:Pointer;
  end;

  TExcDesc = packed record
    jmp:TJmpInstruction;
    case Integer of
      0:(instructions:array[0..0] of Byte);
      1:(cnt:Integer; excTab:array[0..0] of TExcDescEntry);
  end;

  PExcFrame = ^TExcFrame;
  TExcFrame = record
    next:PExcFrame;
    desc:PExcDesc;
    hEBP:Pointer;
    case Integer of
      0:();
      1:(ConstructedObject:Pointer);
      2:(SelfOfMethod:Pointer);
  end;
  //////////////////////////////////////////////////////////////////////////////
  (*
  FLOATING_SAVE_AREA = packed record
    ControlWord: LongWord;
    StatusWord: LongWord;
    TagWord: LongWord;
    ErrorOffset: LongWord;
    ErrorSelector: LongWord;
    DataOffset: LongWord;
    DataSelector: LongWord;
    RegisterArea: array[0..80 - 1] of Byte;
    Cr0NpxState: LongWord;
  end;
  TFloatingSaveArea = FLOATING_SAVE_AREA;

  THREAD_CONTEXT = packed record
    ContextFlags: LongWord;
    Dr0: LongWord;
    Dr1: LongWord;
    Dr2: LongWord;
    Dr3: LongWord;
    Dr6: LongWord;
    Dr7: LongWord;
    FloatSave: FLOATING_SAVE_AREA;
    SegGs: LongWord;
    SegFs: LongWord;
    SegEs: LongWord;
    SegDs: LongWord;
    Edi: LongWord;
    Esi: LongWord;
    Ebx: LongWord;
    Edx: LongWord;
    Ecx: LongWord;
    Eax: LongWord;
    Ebp: LongWord;
    Eip: LongWord;
    SegCs: LongWord;
    EFlags: LongWord;
    Esp: LongWord;
    SegSs: LongWord;
  end;
  TThreadContext = THREAD_CONTEXT;
  PThreadContext = ^THREAD_CONTEXT;

  PExceptionRecord = ^TExceptionRecord;
  TExceptionRecord = record
    ExceptionCode: LongWord;
    ExceptionFlags: LongWord;
    OuterException: PExceptionRecord;
    ExceptionAddress: Pointer;
    NumberParameters: Longint;
    case {IsOsException:}  Boolean of
      True: (ExceptionInformation: array[0..14] of Longint);
      False: (ExceptAddr: Pointer; ExceptObject: Pointer);
  end;
  *)

  PExecption_Handler = ^Exception_Handler;
  PException_Registration = ^Exception_Registration;

  _ExceptionHandler = record
    ExceptionRecord:PExceptionRecord;
    SEH:PException_Registration;
    Context:PContext;
    DispatcherContext:Pointer;
  end;
  Exception_Handler = _ExceptionHandler;

  _ExceptionRegistration = record
    Prev:PException_Registration;
    Handler:PExecption_Handler;
  end;
  Exception_Registration = _ExceptionRegistration;

  // 回调函数返回值 -- SEH异常
  EXCEPTION_DISPOSITION = (
    ExceptionContinueExecution, { 恢复寄存器，继续执行 }
    ExceptionContinueSearch, { 调用处理链表中下一个处理函数 }
    ExceptionNestedException, { 函数中出发了新的异常 }
    ExceptionCollidedUnwind); { 发生了嵌套展开操作 }
  TExceptionDisposition = EXCEPTION_DISPOSITION;

  // 函数原型
  //function _except_handler(ExceptionRecord: PExceptionRecord; EstablisherFrame: Pointer; ContextRecord: PContext; DispatcherContex: Pointer): EXCEPTION_DISPOSITION; cdecl;
  //function ExceptionHandler( ExceptionHandler: EXCEPTION_HANDLER ): LongInt; Cdecl;
  TSEHExceptionHandler = function(ExceptionRecord:PExceptionRecord;
    EstablisherFrame:PExcFrame; ContextRecord:PContext;
    DispatcherContext:Pointer):EXCEPTION_DISPOSITION; cdecl;

const
  // 非SEH异常返回值 - SEH异常 见上面定义
  EXCEPTION_EXECUTE_HANDLER = 1; // 表示我已经处理了异常,可以优雅地结束了
  EXCEPTION_CONTINUE_SEARCH = 0; // 表示我不处理,其他人来吧,于是windows调用默认的处理程序显示一个错误框,并结束
  EXCEPTION_CONTINUE_EXECUTION = -1; // 表示忽略此异常，请从异常发生处继续执行

  EH_NONE = $0;
  EH_NONCONTINUABLE = $1;
  EH_UNWINDING = $2;
  EH_EXIT_UNWIND = $4;
  EH_STACK_INVALID = $8;
  EH_NESTED_CALL = $10;

implementation

end.
