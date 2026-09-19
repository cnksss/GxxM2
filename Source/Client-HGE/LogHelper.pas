unit LogHelper;

interface

uses
Windows,
Classes,
SysUtils,
StrUtils;


//AssignFile：把一个外部文件名和一个文件变量相关联
//Reset：打开一个存在的文件, 只读
//Rewrite：创建并打开一个新文件（或覆盖原有文件）
//Append ：以添加方式打开一个文件,如文件不存在则新建（只适用于文本文件）
//CloseFile：关闭一个打开的文件

type

TApcParam = record
    sLogFile:string;
    sLog:string;
    sMutexName:string;
end;
PApcParam = ^TApcParam;


TLogFile = class
public
    type TLogMode = (logAppend, logNew);
private
    m_sLogFile:string;
    m_sMutexName:string;
    m_hThread:THandle;
    m_idThread:Cardinal;
    m_hEvent:THandle;
private
    function GetUniqueMutexName(sName:string):string;

public
    constructor Create(sLogFile:string; LogMode:TLogMode = logAppend);
    destructor Destroy; override;
    procedure WriteLog(sLog:string); overload;
    procedure WriteLog(sLog: string; Args: array of const); overload;
end;

const
GLOBAL_LOG_LEVEL = 0;

LOG_LEVEL_0 = 0;
LOG_LEVEL_1 = 1;
LOG_LEVEL_2 = 2;
LOG_LEVEL_3 = 3;

type
LPFN_WRITE_ERROR_MESSAGE = procedure (wstrMessage:string) of object;

var
g_LogFile:TLogFile;

procedure WriteToLogFile(sLog:string; nLevel:Integer);

implementation

procedure WriteToLogFile(sLog:string; nLevel:Integer);
begin
    if nLevel >= GLOBAL_LOG_LEVEL then begin
        g_LogFile.WriteLog(sLog);
    end;
end;


procedure ApcProc(param:Pointer);stdcall;
var
    pApc:PApcParam;
    fText:TextFile;
    hMutex:THandle;
    dwWait:DWORD;
begin
    pApc := param;
    if pApc <> nil then begin
        hMutex := CreateMutex(nil, False, PChar(pApc.sMutexName)); //第2个参数确定互斥体不线程所有
        try
            dwWait := WaitForSingleObject(hMutex, 5 * 1000);
                if dwWait = WAIT_OBJECT_0 then begin
                if FileExists(pApc.sLogFile) then begin
                    AssignFile(fText, pApc.sLogFile);
                    Append(fText);
                    WriteLn(fText, pApc.sLog);
                    Close(fText);
                end else begin
                    if DirectoryExists(ExtractFileDir(pApc.sLogFile)) then begin
                        ForceDirectories(ExtractFileDir(pApc.sLogFile));
                    end;
                    AssignFile(fText, pApc.sLogFile);
                    ReWrite(fText);
                    WriteLn(fText, pApc.sLog);
                    Close(fText);
                end;
            end;
        finally
            Dispose(pApc);
            CloseHandle(hMutex);
        end;
    end;
end;

function LogFileProc(param:Pointer):Integer;
var
    dwRet:DWORD;
    logFile:TLogFile;
begin
    logFile := param;
    while True do begin
        dwRet := WaitForSingleObjectEx(logFile.m_hEvent, INFINITE, True);
        if dwRet = WAIT_IO_COMPLETION then begin

        end else if dwRet = WAIT_OBJECT_0 then begin
            break;
        end;
    end;
    Result := 0;
    EndThread(Result);
end;

{ TLogFile }

procedure TLogFile.WriteLog(sLog: string);
var
    pApc:PApcParam;
    wstrTime:string;
begin
    if sLog <> EmptyStr then begin
        wstrTime := FormatDateTime('yyyy-mm-dd hh:mm:ss', now());
        pApc := New(PApcParam);
        pApc.sLogFile := m_sLogFile;
        pApc.sLog := Format('[%s] %s', [wstrTime, sLog]);
        pApc.sMutexName := m_sMutexName;
        if not QueueUserAPC(@ApcProc, m_hThread, UINT_PTR(pApc)) then begin
            Dispose(pApc);
        end;
    end;
end;

procedure TLogFile.WriteLog(sLog: string; Args: array of const);
var
    pApc:PApcParam;
    wstrTime:string;
begin
    if sLog <> EmptyStr then begin
        sLog := Format(sLog, Args);
        wstrTime := FormatDateTime('yyyy-mm-dd hh:mm:ss', now());
        pApc := New(PApcParam);
        pApc.sLogFile := m_sLogFile;
        pApc.sLog := Format('[%s] %s', [wstrTime, sLog]);
        pApc.sMutexName := m_sMutexName;
        if not QueueUserAPC(@ApcProc, m_hThread, UINT_PTR(pApc)) then begin
            Dispose(pApc);
        end;
    end;
end;

constructor TLogFile.Create(sLogFile: string; LogMode: TLogMode);
var
    sMutexName:string;
    fText:TextFile;
begin
    inherited Create;
    sMutexName := LowerCase(sLogFile);
    //sMutexName := System.Hash.THashMD5.GetHashString(sMutexName);
    sMutexName := GetUniqueMutexName(sLogFile);
    m_sMutexName := Format('Global\%s', [sMutexName]);

    m_sLogFile := sLogFile;

    if DirectoryExists(ExtractFileDir(sLogFile)) then begin
        ForceDirectories(ExtractFileDir(sLogFile));
    end;

    try
        if LogMode = logNew then begin
            AssignFile(fText, sLogFile);
            ReWrite(fText);
            Close(fText);
        end;
    except
        OutputDebugString('Clean File Failed');
    end;

    //wstrEventName := Format('LogFile%d', [TLogFile.m_nNumber]);
    m_hEvent := CreateEvent(nil, False, False, nil{ PWideChar(wstrEventName)});
    m_hThread := BeginThread(nil, 0, @LogFileProc, Self, 0, m_idThread);
end;

destructor TLogFile.Destroy;
begin
    SetEvent(m_hEvent);
    WaitForSingleObject(m_hThread, INFINITE);
    CloseHandle(m_hThread);
    CloseHandle(m_hEvent);
    inherited;
end;

function TLogFile.GetUniqueMutexName(sName: string): string;
begin
    SetLength(Result, Length(sName) * 2);
    BinToHex(PChar(sName), @Result[1], Length(sName));
end;

initialization
g_LogFile := TLogFile.Create(ExtractFilePath(ParamStr(0)) + 'MirUI.log');

finalization
g_LogFile.Free;


end.
