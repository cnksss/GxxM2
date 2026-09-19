library RunGatePlug;

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
  Windows,
  SysUtils,
  Classes,
  MMSystem,
  RungateCommon in 'RungateCommon.pas';

type
  PMyAddData = ^TMyAddData;
  TMyAddData = record
    AddData1: Integer;
    AddData2: array[0..11] of Byte;
  end;

{$R *.res}

// 服务器插件初始化
procedure Init(InitRecord: PInitRecord; IsReload: BOOL); stdcall;
begin
  g_InitRecord := InitRecord^;
  g_InitRecord.AddShowLog('~~~~~加载网关插件完成~~~~~', 0);
end;

// 服务器插件反初始化
procedure Uninit; stdcall;
begin
  g_InitRecord.AddShowLog('~~~~~卸载网关插件完成~~~~~', 0);
end;

// 客户端开始
procedure ClientStart(ClientID: Integer); stdcall;
var
  ClientInfo: TRunGatePlugClientInfo;
begin
  g_InitRecord.GetClientInfo(ClientID, @ClientInfo);
  PMyAddData(ClientInfo.DataAdd).AddData1 := 1000;    // 附加自己的数据
  g_InitRecord.AddShowLog(PChar('~~~~~客户端开始~~~~~，ID:' + IntToStr(ClientID) + '， 用户:' + ClientInfo.ChrName), 0);
end;

// 客户端断开
procedure ClientEnd(ClientID: Integer); stdcall;
var
  ClientInfo: TRunGatePlugClientInfo;
begin
  g_InitRecord.GetClientInfo(ClientID, @ClientInfo);
  g_InitRecord.AddShowLog(PChar('~~~~~客户端断开~~~~~:' + IntToStr(ClientID)), 0);
  g_InitRecord.AddShowLog(PChar('~~~~~自己的附加数据是' + IntToStr( PMyAddData(ClientInfo.DataAdd).AddData1) + '~~~~~'), 0);
end;

// 收到客户端发来的数据包
procedure ClientRecvPacket(ClientID: Integer; DefMsg: PTDefaultMessage; Buf: PAnsiChar; BufLen: Integer; var IsSendToM2: BOOL); stdcall;
begin
  if DefMsg.Ident = 10000 then
  begin
    g_InitRecord.SetClientPlugLoad(ClientID);
    g_InitRecord.SendDataToClient(ClientID, DefMsg, Buf, BufLen);
  end;
end;

// 高级设置 菜单 点击
procedure ShowConfigForm; stdcall;
begin

end;

exports
  Init,
  Uninit,
  ClientStart,
  ClientEnd,
  ClientRecvPacket;
  

begin
end.
