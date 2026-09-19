unit uCustomNpcUtils;

interface

uses
  Windows, SysUtils, Classes, IniFilesEx, Grobal2, CheckUnit;

type
  TNpcActionType = (atStand, atAction);

const
  NpcActionNames: array [TNpcActionType] of string = ('站立', '动作');

  {
    NpcDirNames: array[DR_UP..DR_UPLEFT] of string = (
    '上　↑', '右上↗', '右　→', '右下↘', '下　↓', '左下↙', '左　←', '左上↖');

    NpcDirSections: array[DR_UP..DR_UPLEFT] of string = (
    'Top', 'TopRight', 'Right', 'DownRight', 'Down', 'LeftDown', 'Left', 'TopLeft');
  }

  NpcDirNames: array [DR_UP .. DR_UPLEFT] of string = ('方向1', '方向2', '方向3', '方向4', '方向5', '方向6', '方向7', '方向8');

  NpcDirSections: array [DR_UP .. DR_UPLEFT] of string = ('Dir1', 'Dir2', 'Dir3', 'Dir4', 'Dir5', 'Dir6', 'Dir7', 'Dir8');

type
  TCustomNpcConfig = class(TObject)
  private
    FNpcAppr: Word;
    FIsChanged: Boolean;
  public
    ClientBaseConfig: TNpcBaseConfig;
    DirActions: TNpcDirActions;
  public
    constructor Create(ANpcAppr: Word);
    destructor Destroy; override;

    procedure SaveToIniFile;
    procedure LoadFromIniFile;
    procedure SetChanged(Value: Boolean = True);

    property NpcAppr: Word read FNpcAppr;
    property IsChanged: Boolean read FIsChanged;
  end;

procedure SaveCustomNpcClientConfigs(NpcConfigs: TList; FileName: string);

implementation

uses M2Share;

procedure SaveCustomNpcClientConfigs(NpcConfigs: TList; FileName: string);
var
  I, J, Len, Index: Integer;
  CRC: Cardinal;
  P: PAnsiChar;
  MS: TMemoryStream;
  NpcConfig: TCustomNpcConfig;
  ClientConfig: TClientCustomNpcConfig;
begin
  MS := TMemoryStream.Create;
  try
    MS.Write(ClientCustomNPCConfigFlag, SizeOf(ClientCustomNPCConfigFlag));

    Len := NpcConfigs.Count;
    MS.Write(Len, SizeOf(Len));

    Len := SizeOf(TClientCustomNpcConfig);
    MS.Write(Len, SizeOf(Len));

    CRC := 0;
    MS.Write(CRC, SizeOf(CRC));

    for I := 0 to NpcConfigs.Count - 1 do
    begin
      NpcConfig := NpcConfigs.Items[I];

      ClientConfig.wNpcAppr := NpcConfig.FNpcAppr;
      ClientConfig.BaseConfig := NpcConfig.ClientBaseConfig;

      // 将选中的放在前面 chongchong 2016-03-21
      Index := 0;
      for J := Low(NpcConfig.DirActions) to High(NpcConfig.DirActions) do
      begin
        if NpcConfig.DirActions[J].Enabled then
        begin
          ClientConfig.Actions[Index] := NpcConfig.DirActions[J];
          Inc(Index);
        end;
      end;

      ClientConfig.wDirCount := Index;

      for J := Low(NpcConfig.DirActions) to High(NpcConfig.DirActions) do
      begin
        if not NpcConfig.DirActions[J].Enabled then
        begin
          ClientConfig.Actions[Index] := NpcConfig.DirActions[J];
          Inc(Index);
        end;
      end;

      MS.Write(ClientConfig, SizeOf(ClientConfig));
    end;

    Len := SizeOf(ClientCustomNPCConfigFlag) + SizeOf(Len) * 2 + SizeOf(CRC);
    P := MS.Memory;
    Inc(P, Len);
    CRC := BufferCRC(P, MS.Size - Len);

    Len := SizeOf(ClientCustomNPCConfigFlag) + SizeOf(Len) * 2;
    MS.Seek(Len, soFromBeginning);
    MS.Write(CRC, SizeOf(CRC));

    MS.SaveToFile(FileName);
  finally
    MS.Free;
  end;
end;

{ TCustomNpcConfig }

constructor TCustomNpcConfig.Create(ANpcAppr: Word);
var
  I: Integer;
begin
  FIsChanged := False;
  FNpcAppr := ANpcAppr;

  ClientBaseConfig.HPBgOffsetX := 0;
  ClientBaseConfig.HPBgOffsetY := 0;
  ClientBaseConfig.HPOffsetX := 0;
  ClientBaseConfig.HPOffsetY := 0;
  ClientBaseConfig.HPFile := -1;
  ClientBaseConfig.HPStartIndex := -1;

  ClientBaseConfig.HPTextOffsetX := 0;
  ClientBaseConfig.HPTextOffsetY := 0;

  ClientBaseConfig.StandDrawMode := mdmNormal;
  ClientBaseConfig.ActionDrawMode := mdmNormal;
  ClientBaseConfig.StandEffectDrawMode := mdmBlend;
  ClientBaseConfig.ActionEffectDrawMode := mdmBlend;

  ClientBaseConfig.KeepPlayFile := 0;
  ClientBaseConfig.KeepPlayIndex := -1;
  ClientBaseConfig.KeepPlayCount := 0;
  ClientBaseConfig.KeepPlayTime := 100;
  ClientBaseConfig.KeepPlayBlendDraw := True;
  ClientBaseConfig.KeepPlayOffsetX := 0;
  ClientBaseConfig.KeepPlayOffsetY := 0;

  ClientBaseConfig.DrawOrder := ndoKeep_Chr_Eff;

  for I := DR_UP to DR_UPLEFT do
  begin
    DirActions[I].Enabled := True;
    DirActions[I].Std_File := 0;
    DirActions[I].Std_Index := -1;
    DirActions[I].Std_Count := 0;
    DirActions[I].Std_Time := 200;
    DirActions[I].Std_EffFile := 0;
    DirActions[I].Std_EffIndex := -1;
    // DirActions[I].Std_EffCount := 0;
    // DirActions[I].Std_EffTime := 200;

    DirActions[I].Act_File := 0;
    DirActions[I].Act_Index := -1;
    DirActions[I].Act_Count := 0;
    DirActions[I].Act_Time := 200;
    DirActions[I].Act_EffFile := 0;
    DirActions[I].Act_EffIndex := -1;
    // DirActions[I].Act_EffCount := 0;
    // DirActions[I].Act_EffTime := 200;
  end;

  LoadFromIniFile;
end;

destructor TCustomNpcConfig.Destroy;
begin
  inherited;
end;

procedure TCustomNpcConfig.SetChanged(Value: Boolean);
begin
  FIsChanged := Value;
end;

procedure TCustomNpcConfig.LoadFromIniFile;
var
  FileName, SectionName: string;
  IniFile: TIniFileEx;
  I, IntRead: Integer;
  DirAction: PNpcDirAction;
begin
  FIsChanged := False;
  FileName := g_Config.sSmartNpcDir + IntToStr(FNpcAppr) + '.ini';
  if not FileExists(FileName) then
    Exit;

  IniFile := TIniFileEx.Create(FileName);
  try
    SectionName := 'BaseConfig';
    ClientBaseConfig.HPBgOffsetX := IniFile.ReadInteger(SectionName, 'HPBgOffsetX', ClientBaseConfig.HPBgOffsetX);
    ClientBaseConfig.HPBgOffsetY := IniFile.ReadInteger(SectionName, 'HPBgOffsetY', ClientBaseConfig.HPBgOffsetY);

    ClientBaseConfig.HPOffsetX := IniFile.ReadInteger(SectionName, 'HPOffsetX', ClientBaseConfig.HPOffsetX);
    ClientBaseConfig.HPOffsetY := IniFile.ReadInteger(SectionName, 'HPOffsetY', ClientBaseConfig.HPOffsetY);

    ClientBaseConfig.HPFile := IniFile.ReadInteger(SectionName, 'HPFile', ClientBaseConfig.HPFile);
    ClientBaseConfig.HPStartIndex := IniFile.ReadInteger(SectionName, 'HPStartIndex', ClientBaseConfig.HPStartIndex);

    ClientBaseConfig.HPTextOffsetX := IniFile.ReadInteger(SectionName, 'HPTextOffsetX', ClientBaseConfig.HPTextOffsetX);
    ClientBaseConfig.HPTextOffsetY := IniFile.ReadInteger(SectionName, 'HPTextOffsetY', ClientBaseConfig.HPTextOffsetY);

    IntRead := IniFile.ReadInteger(SectionName, 'StandDrawMode', Integer(ClientBaseConfig.StandDrawMode));
    if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
      ClientBaseConfig.StandDrawMode := TCustomDrawMode(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'StandEffectDrawMode', Integer(ClientBaseConfig.StandEffectDrawMode));
    if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
      ClientBaseConfig.StandEffectDrawMode := TCustomDrawMode(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'ActionDrawMode', Integer(ClientBaseConfig.ActionDrawMode));
    if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
      ClientBaseConfig.ActionDrawMode := TCustomDrawMode(IntRead);

    IntRead := IniFile.ReadInteger(SectionName, 'ActionEffectDrawMode', Integer(ClientBaseConfig.ActionEffectDrawMode));
    if (IntRead >= Integer(Low(TCustomDrawMode))) and (IntRead <= Integer(High(TCustomDrawMode))) then
      ClientBaseConfig.ActionEffectDrawMode := TCustomDrawMode(IntRead);

    ClientBaseConfig.KeepPlayFile := IniFile.ReadInteger(SectionName, 'KeepPlayFile', ClientBaseConfig.KeepPlayFile);
    ClientBaseConfig.KeepPlayIndex := IniFile.ReadInteger(SectionName, 'KeepPlayIndex', ClientBaseConfig.KeepPlayIndex);
    ClientBaseConfig.KeepPlayCount := IniFile.ReadInteger(SectionName, 'KeepPlayCount', ClientBaseConfig.KeepPlayCount);
    ClientBaseConfig.KeepPlayTime := IniFile.ReadInteger(SectionName, 'KeepPlayTime', ClientBaseConfig.KeepPlayTime);
    ClientBaseConfig.KeepPlayOffsetX := IniFile.ReadInteger(SectionName, 'KeepPlayOffsetX', ClientBaseConfig.KeepPlayOffsetX);
    ClientBaseConfig.KeepPlayOffsetY := IniFile.ReadInteger(SectionName, 'KeepPlayOffsetY', ClientBaseConfig.KeepPlayOffsetY);
    ClientBaseConfig.KeepPlayBlendDraw := IniFile.ReadBool(SectionName, 'KeepPlayBlendDraw', ClientBaseConfig.KeepPlayBlendDraw);

    IntRead := IniFile.ReadInteger(SectionName, 'DrawOrder', Integer(ClientBaseConfig.DrawOrder));
    if (IntRead >= Integer(Low(TCustomNpcDrawOrder))) and (IntRead <= Integer(High(TCustomNpcDrawOrder))) then
      ClientBaseConfig.DrawOrder := TCustomNpcDrawOrder(IntRead);

    for I := DR_UP to DR_UPLEFT do
    begin
      DirAction := @DirActions[I];
      SectionName := NpcDirSections[I];

      DirAction.Enabled := IniFile.ReadBool(SectionName, 'Enabled', DirAction.Enabled);

      DirAction.Std_File := IniFile.ReadInteger(SectionName, 'StdFile', DirAction.Std_File);
      DirAction.Std_Index := IniFile.ReadInteger(SectionName, 'StdIndex', DirAction.Std_Index);
      DirAction.Std_Count := IniFile.ReadInteger(SectionName, 'StdCount', DirAction.Std_Count);
      DirAction.Std_Time := IniFile.ReadInteger(SectionName, 'StdTime', DirAction.Std_Time);
      DirAction.Std_EffFile := IniFile.ReadInteger(SectionName, 'StdEffFile', DirAction.Std_EffFile);
      DirAction.Std_EffIndex := IniFile.ReadInteger(SectionName, 'StdEffIndex', DirAction.Std_EffIndex);
      // DirAction.Std_EffCount := IniFile.ReadInteger(SectionName, 'StdEffCount', DirAction.Std_EffCount);
      // DirAction.Std_EffTime := IniFile.ReadInteger(SectionName, 'StdEffTime', DirAction.Std_EffTime);

      DirAction.Act_File := IniFile.ReadInteger(SectionName, 'ActFile', DirAction.Act_File);
      DirAction.Act_Index := IniFile.ReadInteger(SectionName, 'ActIndex', DirAction.Act_Index);
      DirAction.Act_Count := IniFile.ReadInteger(SectionName, 'ActCount', DirAction.Act_Count);
      DirAction.Act_Time := IniFile.ReadInteger(SectionName, 'ActTime', DirAction.Act_Time);
      DirAction.Act_EffFile := IniFile.ReadInteger(SectionName, 'ActEffFile', DirAction.Act_EffFile);
      DirAction.Act_EffIndex := IniFile.ReadInteger(SectionName, 'ActEffIndex', DirAction.Act_EffIndex);
      // DirAction.Act_EffCount := IniFile.ReadInteger(SectionName, 'ActEffCount', DirAction.Act_EffCount);
      // DirAction.Act_EffTime := IniFile.ReadInteger(SectionName, 'ActEffTime', DirAction.Act_EffTime);
    end;
  finally
    IniFile.Free;
  end;
end;

procedure TCustomNpcConfig.SaveToIniFile;
var
  FileName, SectionName: string;
  IniFile: TIniFileEx;
  I: Integer;
  DirAction: PNpcDirAction;
begin
  FIsChanged := False;

  if not DirectoryExists(g_Config.sSmartNpcDir) then
    ForceDirectories(g_Config.sSmartNpcDir);

  FileName := g_Config.sSmartNpcDir + IntToStr(FNpcAppr) + '.ini';
  IniFile := TIniFileEx.Create(FileName);
  try
    SectionName := 'BaseConfig';
    IniFile.WriteInteger(SectionName, 'HPBgOffsetX', ClientBaseConfig.HPBgOffsetX);
    IniFile.WriteInteger(SectionName, 'HPBgOffsetY', ClientBaseConfig.HPBgOffsetY);
    IniFile.WriteInteger(SectionName, 'HPOffsetX', ClientBaseConfig.HPOffsetX);
    IniFile.WriteInteger(SectionName, 'HPOffsetY', ClientBaseConfig.HPOffsetY);
    IniFile.WriteInteger(SectionName, 'HPFile', ClientBaseConfig.HPFile);
    IniFile.WriteInteger(SectionName, 'HPStartIndex', ClientBaseConfig.HPStartIndex);

    IniFile.WriteInteger(SectionName, 'HPTextOffsetX', ClientBaseConfig.HPTextOffsetX);
    IniFile.WriteInteger(SectionName, 'HPTextOffsetY', ClientBaseConfig.HPTextOffsetY);

    IniFile.WriteInteger(SectionName, 'StandDrawMode', Integer(ClientBaseConfig.StandDrawMode));
    IniFile.WriteInteger(SectionName, 'StandEffectDrawMode', Integer(ClientBaseConfig.StandEffectDrawMode));
    IniFile.WriteInteger(SectionName, 'ActionDrawMode', Integer(ClientBaseConfig.ActionDrawMode));
    IniFile.WriteInteger(SectionName, 'ActionEffectDrawMode', Integer(ClientBaseConfig.ActionEffectDrawMode));
    IniFile.WriteInteger(SectionName, 'DrawOrder', Integer(ClientBaseConfig.DrawOrder));

    IniFile.WriteInteger(SectionName, 'KeepPlayFile', ClientBaseConfig.KeepPlayFile);
    IniFile.WriteInteger(SectionName, 'KeepPlayIndex', ClientBaseConfig.KeepPlayIndex);
    IniFile.WriteInteger(SectionName, 'KeepPlayCount', ClientBaseConfig.KeepPlayCount);
    IniFile.WriteInteger(SectionName, 'KeepPlayTime', ClientBaseConfig.KeepPlayTime);
    IniFile.WriteInteger(SectionName, 'KeepPlayOffsetX', ClientBaseConfig.KeepPlayOffsetX);
    IniFile.WriteInteger(SectionName, 'KeepPlayOffsetY', ClientBaseConfig.KeepPlayOffsetY);
    IniFile.WriteBool(SectionName, 'KeepPlayBlendDraw', ClientBaseConfig.KeepPlayBlendDraw);

    for I := DR_UP to DR_UPLEFT do
    begin
      DirAction := @DirActions[I];
      SectionName := NpcDirSections[I];

      IniFile.WriteBool(SectionName, 'Enabled', DirAction.Enabled);

      IniFile.WriteInteger(SectionName, 'StdFile', DirAction.Std_File);
      IniFile.WriteInteger(SectionName, 'StdIndex', DirAction.Std_Index);
      IniFile.WriteInteger(SectionName, 'StdCount', DirAction.Std_Count);
      IniFile.WriteInteger(SectionName, 'StdTime', DirAction.Std_Time);
      IniFile.WriteInteger(SectionName, 'StdEffFile', DirAction.Std_EffFile);
      IniFile.WriteInteger(SectionName, 'StdEffIndex', DirAction.Std_EffIndex);
      // IniFile.WriteInteger(SectionName, 'StdEffCount', DirAction.Std_EffCount);
      // IniFile.WriteInteger(SectionName, 'StdEffTime', DirAction.Std_EffTime);

      IniFile.WriteInteger(SectionName, 'ActFile', DirAction.Act_File);
      IniFile.WriteInteger(SectionName, 'ActIndex', DirAction.Act_Index);
      IniFile.WriteInteger(SectionName, 'ActCount', DirAction.Act_Count);
      IniFile.WriteInteger(SectionName, 'ActTime', DirAction.Act_Time);
      IniFile.WriteInteger(SectionName, 'ActEffFile', DirAction.Act_EffFile);
      IniFile.WriteInteger(SectionName, 'ActEffIndex', DirAction.Act_EffIndex);
      // IniFile.WriteInteger(SectionName, 'ActEffCount', DirAction.Act_EffCount);
      // IniFile.WriteInteger(SectionName, 'ActEffTime', DirAction.Act_EffTime);
    end;
  finally
    IniFile.Free;
  end;
end;

end.
