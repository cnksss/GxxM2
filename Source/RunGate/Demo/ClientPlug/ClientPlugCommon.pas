unit ClientPlugCommon;

interface

uses
  Classes, SysUtils, Windows;

type
  TDefaultMessage = record
    Recog: Int64;
    Ident: Word;
    Param: Word;
    Tag: Word;
    Series: Word;
  end;
  pTDefaultMessage = ^TDefaultMessage;


type
  { 在聊天位置输出文字 }
  TAddChatText = procedure(Src: PAnsiChar; ForeColor: Integer; BackColor: Integer); stdcall;
    
  { 发送消息 }
  TSendSocketFunc = procedure(Msg: PTDefaultMessage; AddData: PAnsiChar; AddDataLen: Integer); stdcall;

  PAppFuncDef = ^TAppFuncDef;
  TAppFuncDef = record
    AddChatText: TAddChatText;                      // 在聊天位置输出文字
    SendSocket: TSendSocketFunc;                    // 发送消息
  end;

var
  g_AppFuncDef: TAppFuncDef;

implementation

end.
