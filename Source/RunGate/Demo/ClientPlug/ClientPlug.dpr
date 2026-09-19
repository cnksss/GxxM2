library ClientPlug;

{ Important note about DLL memory management: ShareMem must be the
  first unit in your library's USES clause AND your project's (select
  Project-View Source) USES clause if your DLL exports any procedures or
  functions that pass strings as parameters or function results. This
  applies to all strings passed to and from your DLL--even those that
  are nested in records and classes. ShareMem is the interface unit to
  the BORLNDMM.DLL shared memory manager, which must be deployed along
  with your DLL. To avoid using BORLNDMM.DLL, pass string information
  using PChar or ShortString parameters. }

uses
  SysUtils,
  Classes,
  Windows,
  ClientPlugCommon in 'ClientPlugCommon.pas';

{$R *.res}

function Init(AppFunc: PAppFuncDef): BOOL; stdcall;
var
  DefMsg: TDefaultMessage;
  S: string;
begin
  g_AppFuncDef := AppFunc^;
  S := '~~~~~~~~~~~~~~~~~~加载反外挂插件成功~~~~~~~~~~~~~~~~~~~';
  g_AppFuncDef.AddChatText(PChar(S), $FFFFFFFF, 0);

  DefMsg.Recog := 0;
  DefMsg.Ident := 10000;
  DefMsg.Param := 0;
  DefMsg.Tag := 0;
  DefMsg.Series := 0;

  g_AppFuncDef.SendSocket(@DefMsg, nil, 0);
end;

procedure UnInit; stdcall;
begin

end;

procedure HookRecv(DefMsg: pTDefaultMessage; lpData: PChar; InDataLen: Integer); stdcall;
begin
  if DefMsg.Ident = 10000 then
  begin
    g_AppFuncDef.AddChatText('这个没什么用，只是测试下网关插件返回数据', $FFFFFFFF, 0);
  end;
end;

exports
  Init,
  UnInit,
  HookRecv;

begin
end.
