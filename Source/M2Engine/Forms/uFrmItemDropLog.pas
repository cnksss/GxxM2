unit uFrmItemDropLog;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, VirtualTrees, M2Share, HUtil32;

type
  TFrmItemDropLog = class(TForm)
    vstLogs: TVirtualStringTree;
    procedure vstLogsGetNodeDataSize(Sender: TBaseVirtualTree;
      var NodeDataSize: Integer);
    procedure vstLogsFreeNode(Sender: TBaseVirtualTree; Node: PVirtualNode);
    procedure vstLogsGetText(Sender: TBaseVirtualTree; Node: PVirtualNode;
      Column: TColumnIndex; TextType: TVSTTextType;
      var CellText: string);
    procedure FormKeyPress(Sender: TObject; var Key: Char);
  private
    { Private declarations }
    procedure LoadLog(FileName: string; MapName: string);
  public
    { Public declarations }
  end;

  function ShowFrmItemDropLog(ItemName, MapName: string): Boolean;

implementation

type
  PDropItemLogData = ^TDropItemLogData;
  TDropItemLogData = record
    DropDate: TDateTime;
    ItemOwner: string;
    DropMonName: string;
    MapName: string;
    nX, nY: Integer;
  end;

{$R *.dfm}

function ShowFrmItemDropLog(ItemName, MapName: string): Boolean;
var
  FileName: string;
  FrmItemDropLog: TFrmItemDropLog;
begin
  FileName := g_Config.sItemDropLogDir + ItemName + '.txt';
  FrmItemDropLog := TFrmItemDropLog.Create(nil);
  try
    FrmItemDropLog.Caption := ItemName + ' µôÂäÈÕÖ¾';
    FrmItemDropLog.LoadLog(FileName, MapName);
    FrmItemDropLog.ShowModal;

    Result := True;
  finally
    FrmItemDropLog.Free;
  end;
end;

procedure TFrmItemDropLog.vstLogsGetNodeDataSize(Sender: TBaseVirtualTree;
  var NodeDataSize: Integer);
begin
  NodeDataSize := SizeOf(TDropItemLogData);
end;

procedure TFrmItemDropLog.vstLogsFreeNode(Sender: TBaseVirtualTree;
  Node: PVirtualNode);
var
  LogData: PDropItemLogData;
begin
  LogData := Sender.GetNodeData(Node);
  LogData.ItemOwner := '';
  LogData.DropMonName := '';
  LogData.MapName := '';
end;

procedure TFrmItemDropLog.LoadLog(FileName, MapName: string);
var
  I: Integer;
  SL: TStringList;
  S, S1, S2, S3, S4, S5, S6, S7: string;
  d: Double;
  Node: PVirtualNode;
  LogData: PDropItemLogData;

  DateFmt: TFormatSettings;
begin
  if not FileExists(FileName) then Exit;

  SL := TStringList.Create;
  try
    SL.LoadFromFile(FileName);

    for I := SL.Count - 1 downto 0 do
    begin
      S := SL.Strings[I];

      S := GetValidStr3(S, S1, [' ', #9]);
      S := GetValidStr3(S, S2, [' ', #9]);
      S := GetValidStr3(S, S3, [' ', #9]);
      S := GetValidStr3(S, S4, [' ', #9]);
      S := GetValidStr3(S, S5, [' ', #9]);
      S := GetValidStr3(S, S6, [' ', #9]);
      S := GetValidStr3(S, S7, [' ', #9]);

    {$IF CompilerVersion >= 22}
      DateFmt := TFormatSettings.Create(GetThreadLocale);
    {$ELSE}
      GetLocaleFormatSettings(GetThreadLocale, DateFmt);
    {$IFEND}
      DateFmt.DateSeparator := '-';
      DateFmt.TimeSeparator := ':';

      d := StrToDateTime(S1 + ' ' + S2, DateFmt);
      if (d > 0) and ((MapName = '*') or SameText(MapName, S4)) then
      begin
        Node := vstLogs.AddChild(nil);
        LogData := vstLogs.GetNodeData(Node);
        LogData.DropDate := d;
        LogData.ItemOwner := S3;
        LogData.DropMonName := S4;
        LogData.MapName := S5;
        LogData.nX := StrToIntDef(S6, 0);
        LogData.nY := StrToIntDef(S7, 0);
      end;
    end;
  finally
    SL.Free;
  end;
end;

procedure TFrmItemDropLog.vstLogsGetText(Sender: TBaseVirtualTree;
  Node: PVirtualNode; Column: TColumnIndex; TextType: TVSTTextType;
  var CellText: string);
var
  LogData: PDropItemLogData;
begin
  LogData := Sender.GetNodeData(Node);
  case Column of
    0: CellText := FormatDateTime('yyyy/mm/dd hh:nn:ss', LogData.DropDate);
    1: CellText := LogData.ItemOwner;
    2: CellText := LogData.DropMonName;
    3: CellText := LogData.MapName;
    4: CellText := Format('%d, %d', [LogData.nX, LogData.nY]);
  end;
end;

procedure TFrmItemDropLog.FormKeyPress(Sender: TObject; var Key: Char);
begin
  if Key = Chr(VK_ESCAPE) then
    ModalResult := mrCancel;
end;

end.
