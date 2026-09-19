unit LocalDB;

interface

{$DEFINE USE_FDTABLE}

uses
  Windows, Messages, SysUtils, StrUtils, Variants, Classes, Graphics, Controls, Forms, ActiveX, Dialogs, M2Share, DB,
{$IFDEF USE_FDTABLE}
  FireDac.Comp.DataSet, FireDac.Comp.Client,
{$ELSE}
  kbmMemTable,
{$ENDIF}
  HUtil32, Grobal2, ObjBase,
{$IF DBTYPE = BDE}
{$IFNDEF CPUX64}
  DBTables,
{$ENDIF}
{$ELSE}
  ADODB,
{$IFEND}
  ObjNpc, UsrEngn, svMain, ItemEffects, uCustomMonsterUtils, uCustomMagicUtils, EDCode, ObjGuard, WideStrUtils, uCustomNpcUtils
  {, kbmMemBinaryStreamFormat} , M2Threads, SafeAreaManager, SQLite3DataBase,

{$IF CompilerVersion >= 32.0}
  FireDac.Phys.SQLiteCli, FireDac.Phys.SQLiteWrapper.Stat,
{$ELSE}
  SQLiteCli,
{$ENDIF}
  NpcCommon, M2Definition, StringListHelper;

type
  TDefineInfo = record
    sName: string;
    sText: string;
  end;

  pTDefineInfo = ^TDefineInfo;

  TQDDinfo = record
    n00: Integer;
    s04: string;
    sList: TStringList;
  end;

  pTQDDinfo = ^TQDDinfo;

  TGoodFileHeader = record
    nItemCount: Integer;
    Resv: array [0 .. 251] of Integer;
  end;

  TFrmDB = class(TObject)
  private
{$IFDEF USE_FDTABLE}
    FItemsDataSet: TFDMemTable;
    FMonsterDataSet: TFDMemTable;
{$ELSE}
    FItemsDataSet: TkbmMemTable;
    FMonsterDataSet: TkbmMemTable;
{$ENDIF}
    procedure QMangeNPC;
    procedure QFunctionNPC;
    procedure RobotNPC;
    procedure QMissionNPC;
    procedure QBatterNPC;
    procedure DeCodeStringList(StringList: TStringList);
    procedure ResetCustomNpcList;
    { Private declarations }
  public
    SQLiteDB: TSQLite3Database;
{$IF DBTYPE = BDE}
{$IFNDEF CPUX64}
    Query: TQuery;
{$ENDIF}
{$ELSE}
    Query: TADOQuery;
{$IFEND}
    constructor Create();
    destructor Destroy; override;
    function DeCodePlugBuffer(MemoryStream: TMemoryStream): AnsiString;
    function LoadMonitems(MonName: string; var ItemList: TList): Integer;
    function LoadMonitems_Ex(FileName, CallLabel: string; var ItemList: TList): Integer;
    function LoadItemsDB(AutoConnect: Boolean = True): Integer;
    function LoadMinMap(): Integer;
    function LoadMapInfo(): Integer;
    function LoadMonsterDB(): Integer;
    function LoadMagicDB(): Integer;
    function LoadMonGen(): Integer;
    function LoadUnbindList(): Integer;
    function LoadMapQuest(): Integer;
    function LoadQuestDiary(): Integer;
    function LoadAdminList(): Boolean;
    function LoadMerchant(): Integer;
    function LoadGuardList(): Integer;
    function LoadNpcs(): Integer;
    function LoadMakeItem(): Integer;
    function LoadStartPoint(): Integer;
    function LoadNpcScript(NPC: TNormNpc; sPatch, sScritpName: string): Integer;
    function LoadScriptFile(NPC: TNormNpc; sPatch, sScritpName: string; boFlag: Boolean): Integer;
    function GetScriptFileCRC(FileName: string): LongWord;
    function CheckScriptFileChanged(NPC: TNormNpc; IsMerchant: Boolean): Boolean;
    /// 2020-05-18 00:04:32
    function LoadGoodRecord(NPC: TMerchant; sFile: string): Integer;
    function LoadGoodPriceRecord(NPC: TMerchant; sFile: string): Integer;
    function SaveGoodRecord(NPC: TMerchant; sFile: string): Integer;
    function SaveGoodPriceRecord(NPC: TMerchant; sFile: string): Integer;
    function LoadUpgradeWeaponRecord(sNPCName: string; DataList: TList): Integer;
    function SaveUpgradeWeaponRecord(sNPCName: string; DataList: TList): Integer;
    procedure ReloadMerchants();
    function LoadMapEvent(): Integer;
    function LoadIconFile(NPC: TNormNpc; ActorIcons: pTActorIconArray; sPatch, sFileName: string): Integer;
    procedure LoadMonFireDragonGuard(); // 创建火龙守护兽并写入列表 piaoyun 2013-08-19
    function GetStdItemDBField(ItemIdx: Integer; FieldName: string): string;
    function GetMonsterDBField(MonsterName, FieldName: string): string;
    { Public declarations }
  end;

var
  FrmDB: TFrmDB;

procedure ClearMonItemList(List: TList);

implementation

uses
  Math, Envir, EncryptUnit_LF, Common, CheckUnit, ObjFireDragon, ObjMon2, HandleNpcCmds, ObjPlayer, ZipUnit;

procedure ClearMonItemList(List: TList);
var
  I: Integer;
  MonItem: pTMonItemInfo;
begin
  for I := 0 to List.Count - 1 do
  begin
    MonItem := pTMonItemInfo(List.Items[I]);
    if MonItem.List <> nil then
    begin
      ClearMonItemList(MonItem.List);
      MonItem.List.Free;
    end;
    DisPose(MonItem);
  end;
end;

function TFrmDB.LoadAdminList(): Boolean;
var
  sFileName: string;
  sLineText: string;
  sIPaddr: string;
  sCharName: string;
  sData: string;
  LoadList: TStringList;
  AdminInfo: pTAdminInfo;
  I: Integer;
  nLv: Integer;
begin
  Result := False;
  sFileName := g_Config.sEnvirDir + 'AdminList.txt';
  if not FileExists(sFileName) then
    Exit;

  UserEngine.m_AdminList.LockW(1);
  try
    UserEngine.m_AdminList.Clear;
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName, TEncoding.ANSI);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[I];
      nLv := -1;
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        if sLineText[1] = '*' then
          nLv := 10
        else if sLineText[1] = '1' then
          nLv := 9
        else if sLineText[1] = '2' then
          nLv := 8
        else if sLineText[1] = '3' then
          nLv := 7
        else if sLineText[1] = '4' then
          nLv := 6
        else if sLineText[1] = '5' then
          nLv := 5
        else if sLineText[1] = '6' then
          nLv := 4
        else if sLineText[1] = '7' then
          nLv := 3
        else if sLineText[1] = '8' then
          nLv := 2
        else if sLineText[1] = '9' then
          nLv := 1;
        if nLv > 0 then
        begin
          sLineText := GetValidStrCap(sLineText, sData, ['/', '\', ' ', #9]);
          sLineText := GetValidStrCap(sLineText, sCharName, ['/', '\', ' ', #9]);
          sLineText := GetValidStrCap(sLineText, sIPaddr, ['/', '\', ' ', #9]);

          New(AdminInfo);
          AdminInfo.nLv := nLv;
          AdminInfo.sChrName := sCharName;
          AdminInfo.sIPaddr := sIPaddr;
          UserEngine.m_AdminList.Add(AdminInfo);
        end;
      end;
    end;
    LoadList.Free;
  finally
    UserEngine.m_AdminList.UnLockW;
  end;
  Result := True;
end;

function TFrmDB.LoadGuardList(): Integer;
var
  sFileName, s14, s1C, s20, s24, s2C: string;
  tGuardList, PointList: TStringList;
  I, nCount, k: Integer;
  tGuard: TBaseObject;
  sX, sY: string;
begin
  Result := -1;
  sFileName := g_Config.sEnvirDir + 'GuardList.txt';
  if FileExists(sFileName) then
  begin
    tGuardList := TStringList.Create;
    PointList := TStringList.Create;
    tGuardList.LoadFromFile(sFileName);
    for I := 0 to tGuardList.Count - 1 do
    begin
      s14 := tGuardList.Strings[I];
      if (s14 <> '') and (s14[1] <> ';') then
      begin
        s14 := GetValidStrCap(s14, s1C, [' ']);
        if (s1C <> '') and (s1C[1] = '"') then
          ArrestStringEx(s1C, '"', '"', s1C);
        s14 := GetValidStr3(s14, s20, [' ', #9]);
        s14 := GetValidStr3(s14, s24, [' ', #9]);
        if (s24 <> '') and (s24[1] = '[') and (s1C <> '') and (s20 <> '') then
        begin
          ArrestStringEx(s24, '[', ']', s24);
          PointList.Clear;
          nCount := ExtractStrings(['|'], [], PChar(s24), PointList);
          if nCount > 0 then
          begin
            sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
            s14 := GetValidStr3(s14, s2C, [' ', #9, ':']);
            tGuard := UserEngine.RegenMonsterByName(s20, StrToIntDef(sX, 0), StrToIntDef(sY, 0), s1C);
            if tGuard <> nil then
            begin
              tGuard.m_btDirection := StrToIntDef(s2C, 0);
              tGuard.m_btNameColor := g_Config.btGuardNameColor;
              if (nCount > 1) then
              begin
                if (tGuard is TMoveSuperGuard) then
                begin
                  with tGuard as TMoveSuperGuard do
                  begin
                    SetLength(MovePoint, nCount);
                    MovePoint[0].X := StrToIntDef(sX, 0);
                    MovePoint[0].Y := StrToIntDef(sY, 0);
                    for k := 1 to nCount - 1 do
                    begin
                      sY := GetValidStr3(PointList[k], sX, [' ', ',', #9]);
                      MovePoint[k].X := StrToIntDef(sX, 0);
                      MovePoint[k].Y := StrToIntDef(sY, 0);
                    end;
                  end;
                end
                else if (tGuard is TMoveArcherGuard) then
                begin
                  with tGuard as TMoveArcherGuard do
                  begin
                    SetLength(MovePoint, nCount);
                    MovePoint[0].X := StrToIntDef(sX, 0);
                    MovePoint[0].Y := StrToIntDef(sY, 0);
                    for k := 1 to nCount - 1 do
                    begin
                      sY := GetValidStr3(PointList[k], sX, [' ', ',', #9]);
                      MovePoint[k].X := StrToIntDef(sX, 0);
                      MovePoint[k].Y := StrToIntDef(sY, 0);
                    end;
                  end;
                end;
              end;
            end;
          end;
        end;
      end;
    end;
    PointList.Free;
    tGuardList.Free;
    Result := 1;
  end;
end;

procedure GetFieldInfo(FieldInfo: string; var FieldType: TFieldType; var FieldLen, FieldDec: Integer);
var
  p1, p2, pn: Integer;
  vt: string;
begin
  FieldType := ftString; // just a default;
  FieldLen := 255;
  FieldDec := 0;
  p1 := pos('(', FieldInfo);
  if p1 <> 0 then
  begin
    p2 := pos(')', FieldInfo);
    if p2 <> 0 then
    begin
      vt := LowerCase(Copy(FieldInfo, 1, p1 - 1));
      if SameText(vt, 'varchar') or SameText(vt, 'char') or SameText(vt, 'varchar2') or SameText(vt, 'text') then
      begin
        FieldType := ftString;
        FieldLen := StrToInt(Copy(FieldInfo, p1 + 1, p2 - p1 - 1));
        if SameText(vt, 'text') then
        begin
          FieldLen := FieldLen * 2;
        end;
      end
      else if SameText(vt, 'nvarchar') or SameText(vt, 'nchar') or SameText(vt, 'nvarchar2') then
      begin
        FieldType := ftWideString;
        FieldLen := StrToInt(Copy(FieldInfo, p1 + 1, p2 - p1 - 1));
        FieldLen := FieldLen * 2;
      end
      else if SameText(vt, 'numeric') then
      begin
        vt := Copy(FieldInfo, p1 + 1, p2 - p1 - 1);
        pn := pos('.', vt);
        if pn = 0 then
          pn := pos(',', vt);
        FieldType := ftFloat;
        if pn = 0 then
        begin
          FieldLen := StrToInt(vt);
          FieldDec := 0;
        end
        else
        begin
          FieldLen := StrToInt(Copy(vt, 1, pn - 1));
          FieldDec := StrToInt(Copy(vt, pn + 1, 2));
        end;
      end;
    end
    else
      FieldLen := 256;
  end
  else
  begin
    vt := LowerCase(FieldInfo);
    if SameText(vt, 'date') then
    begin
      FieldType := ftDate;
      FieldLen := 10;
    end
    else if SameText(vt, 'datetime') then
    begin
      FieldType := ftDateTime; // fpierce original ftDate
      FieldLen := 24; // aducom
    end
    else if SameText(vt, 'time') then
    begin
      FieldType := ftTime;
      FieldLen := 12;
    end{$IFDEF ASQLITE_D6PLUS}
    else if SameText(vt, 'timestamp') then
    begin
      FieldType := ftTimeStamp;
      FieldLen := 12;
    end{$ENDIF}
    else if SameText(vt, 'integer') or SameText(vt, 'int') then
    begin
      FieldType := ftInteger;
      FieldLen := 12;
    end
    else if SameText(vt, 'float') or SameText(vt, 'real') then
    begin
      FieldType := ftFloat;
      FieldLen := 12;
    end
    else if SameText(vt, 'boolean') or SameText(vt, 'logical') then
    begin
      FieldType := ftBoolean;
      FieldLen := 2;
    end
    else if SameText(vt, 'char') or SameText(vt, 'byte') then
    begin
      FieldType := ftString;
      FieldLen := SizeOf(Char);
    end
    else if SameText(vt, 'text') or SameText(vt, 'shorttext') or SameText(vt, 'string') then
    begin
      FieldType := ftString;
      FieldLen := 255;
    end
    else if SameText(vt, 'widetext') or SameText(vt, 'widestring') then
    begin
      FieldType := ftWideString;
      FieldLen := 512;
    end
    else if SameText(vt, 'currency') or SameText(vt, 'financial') or SameText(vt, 'money') then
    begin
      FieldType := ftCurrency;
      FieldLen := 10;
    end
    else if SameText(vt, 'blob') then
    begin
      FieldType := ftBlob;
      FieldLen := SizeOf(Pointer);
    end
    else if SameText(vt, 'graphic') then
    begin
      FieldType := ftGraphic;
      FieldLen := SizeOf(Pointer);
    end
    else if SameText(vt, 'clob') or SameText(vt, 'memo') or SameText(vt, 'text') or SameText(vt, 'longtext') then
    begin
      FieldType := ftMemo;
      FieldLen := SizeOf(Pointer);
    end
    else if SameText(vt, 'nclob') or SameText(vt, 'nmemo') or SameText(vt, 'ntext') or SameText(vt, 'nlongtext') then
    begin
      FieldType := ftWideString;
      FieldLen := SizeOf(Pointer);
    end;
  end;
end;

function TFrmDB.LoadItemsDB(AutoConnect: Boolean): Integer;
var
  I, Idx, nIndex: Integer;
  StdItem: pTStdItem;
  ItemEffect: pTItemEffect;
  nParam: PInteger;
  InBuf: PAnsiChar; // array[0..1024000] of Char;
  InBytes: Integer;
  Buffer: PAnsiChar;
  PartialFieldsCount: Integer;
  AllFiledsCount: Integer;
  PartialStdItem: TPartialStdItem;
  CompleteStdItem: TCompleteStdItem;
  StdItemsHeader: PTStdItemsHeader;
  // BinaryStreamFormat: TkbmCustomBinaryStreamFormat;
  TempQuery: TDataSet;
{$IFDEF USE_FDTABLE}
  ClientDataSet: TFDMemTable;
{$ELSE}
  ClientDataSet: TkbmMemTable;
{$ENDIF}
  nRet, ColumnCount: Integer;
  ColumnName, ColumnType: string;
  FieldType: TFieldType;
  FieldLen: Integer;
  FieldDec: Integer;
  sm: TSQLStatement;
resourcestring
  sSQLString = 'select * from StdItems';
begin
  if g_boUseSqliteDB and AutoConnect then
    FrmDB.SQLiteDB.Connected := True;

  try
    ClientDataSet := {$IFDEF USE_FDTABLE} TFDMemTable.Create(nil); {$ELSE}
      TkbmMemTable.Create(nil); {$ENDIF}
    try
      UserEngine.ClearSortStdItemList;
      TempQuery := nil;
{$IF MULTI_THREAD = 1}
      if g_MultiThreadRun then
        UserEngine.StdItemList.LockW(1);
      try
{$IFEND}
        try
          for I := 0 to UserEngine.StdItemList.Count - 1 do
          begin
            DisPose(pTStdItem(UserEngine.StdItemList.Items[I]));
          end;
          // Memo.Lines.Add(DecryString_LF2(GetNumToString));
          UserEngine.StdItemList.Clear;
{$IF CHECKCRACK = 1}
          MemoryStream := TMemoryStream.Create;
          MemoryStream.LoadFromFile(Application.ExeName);
          nLen := SizeOf(TM2ServerConfig);
          SetLength(sText, nLen);
          MemoryStream.Seek(-nLen, soEnd);
          MemoryStream.Read(sText[1], nLen);
          DecryBufferA_LF(sText, @ServerConfig, SizeOf(TM2ServerConfig));
          New(nParam);
          nParam^ := BufferCrc(MemoryStream.Memory, MemoryStream.Size - SizeOf(TM2ServerConfig)) - ServerConfig.nParam;
          // Memo.Lines.Add(Format('(nParam^:%d CheckCrc:%d Crc:%d)', [nParam^,
          // BufferCrc(MemoryStream.Memory, MemoryStream.Size - SizeOf(TM2ServerConfig)), ServerConfig.nParam]));
          MemoryStream.Free;
          FillChar(ServerConfig, SizeOf(TM2ServerConfig), 0);
          if nParam^ <> 0 then
            ShowMessageEx
              (DecryString_LF
              ('LSUFU@]>VpqrTPM?YbMvXNybWc@kTP]mJNilOAakVb]tWQDrX@]sPQe?IPqDQRmNNBaIZ`upV`ugMRYAZbMpGsMGIp]cY`XmWOYfNNhgP?=OWcUOIAecXoasUB]ALR]=NaXlKL')
              );
{$ELSE}
          New(nParam);
          nParam^ := 0;
{$IFEND}
          Result := -1;
          if g_boUseSqliteDB then
          begin
            TempQuery := ClientDataSet;
            sm := SQLiteDB.Statements.AddSQLStatement('Select_StdItems');
            sm.Sql := 'select * from StdItems';
            sm.Prepare;
            try
              nRet := sm.Step;
              if nRet = SQLITE_ROW then
              begin
                ColumnCount := sm.GetColumnCount;
                for I := 0 to ColumnCount - 1 do
                begin
                  ColumnName := sm.GetColumnName(I);
                  ColumnType := sm.GetColumnDeclType(I);
                  // OutputDebugString(PChar(Format('%s: %s', [ColumnName, ColumnType])));
                  if ColumnType = '' then
                    GetFieldInfo('string', FieldType, FieldLen, FieldDec) // OL
                  else
                    GetFieldInfo(ColumnType, FieldType, FieldLen, FieldDec);
                  if (FieldType <> ftString) and (FieldType <> ftWideString) then
                  begin
                    with ClientDataSet.FieldDefs.AddFieldDef do
                    begin
                      Name := ColumnName;
                      DataType := FieldType;
                      if FieldType = ftFloat then
                        Precision := FieldDec;
                    end
                  end
                  else
                  begin
                    with ClientDataSet.FieldDefs.AddFieldDef do
                    begin
                      Name := ColumnName;
                      DataType := FieldType;
                      Size := FieldLen;
                    end;
                  end;
                end;
{$IFDEF USE_FDTABLE}
                ClientDataSet.CreateDataSet;
{$ELSE}
                ClientDataSet.CreateTable;
{$ENDIF}
                ClientDataSet.Active := True;
                while (nRet = SQLITE_ROW) do
                begin
                  ClientDataSet.Append;
                  for I := 0 to ColumnCount - 1 do
                  begin
                    case ClientDataSet.FieldDefs[I].DataType of
                      ftWideString:
                        begin
                          ClientDataSet.Fields[I].AsString := sm.GetColumnValueText(I);
                        end;
                      ftString:
                        begin // DI
                          ClientDataSet.Fields[I].AsString := sm.GetColumnValueText(I);
                        end;
                      ftInteger:
                        begin
                          ClientDataSet.Fields[I].AsInteger := sm.GetColumnValueInt(I);
                        end;
                    else
                      begin
                        raise Exception.Create('StdItems 无法识别的数据类型, 字段：' + ClientDataSet.FieldDefs[I].Name);
                      end;
                    end;
                  end;
                  nRet := sm.Step;
                end;
              end;
            finally
              sm.Finalize;
            end;
          end
          else
          begin
{$IFNDEF CPUX64}
            TempQuery := Query;
            Query.Sql.Clear;
            Query.Sql.Add(sSQLString);
            try
              Query.Open;
            finally
              Result := -2;
            end;
{$ELSE}
            Result := -1000;
            MainOutMessage('[错误]***64位引擎不支持BDE数据库***', False);
            Exit;
{$ENDIF}
          end;
          // 复制到 ----> FItemsDataSet
          TempQuery.First;
{$IFDEF USE_FDTABLE}
          FItemsDataSet.CopyDataSet(TempQuery, [coStructure, coRestart, coAppend]);
{$ELSE}
          FItemsDataSet.LoadFromDataSet(TempQuery, [mtcpoStructure, mtcpoProperties]);
{$ENDIF}
          // MemTable.LoadFromDataSet(TempQuery, [mtcpoStructure,mtcpoProperties]);
          TempQuery.First;
          for I := 0 to TempQuery.RecordCount - 1 do
          begin
            New(StdItem);
            FillChar(StdItem^, SizeOf(TStdItem), 0);
            Idx := TempQuery.FieldByName('Idx').AsInteger;
            StdItem.Name := TempQuery.FieldByName('Name').AsString;
            StdItem.DBName := StdItem.Name; // 数据库中的物品名称 这个装备改名 不会改变，防止客户端因为改名后一些功能失效
            StdItem.StdMode := TempQuery.FieldByName('StdMode').AsInteger;

            // 斗笠面巾=-1时，不显示发型
            if (StdItem.StdMode = 16) and (TempQuery.FieldByName('Shape').AsInteger = -1) then
              StdItem.Shape := 1000
            else
              StdItem.Shape := TempQuery.FieldByName('Shape').AsInteger;

            StdItem.Weight := TempQuery.FieldByName('Weight').AsInteger;
            StdItem.AniCount := TempQuery.FieldByName('AniCount').AsInteger;
            StdItem.Source := TempQuery.FieldByName('Source').AsInteger;
            StdItem.Reserved := TempQuery.FieldByName('Reserved').AsInteger;
            StdItem.Reserved1 := TempQuery.FieldByName('Reserved').AsInteger;
            StdItem.Looks := TempQuery.FieldByName('Looks').AsInteger;
            StdItem.DuraMax := Word(TempQuery.FieldByName('DuraMax').AsInteger);
            StdItem.HP := TempQuery.FieldByName('HP').AsInteger;
            StdItem.MP := TempQuery.FieldByName('MP').AsInteger;
            // 21亿支持 chongchong 2015-02-09
            StdItem.AC1 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Ac').AsInteger * (g_Config.nItemsACPowerRate / 10));
            StdItem.AC2 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Ac2').AsInteger * (g_Config.nItemsACPowerRate / 10));
            StdItem.MAC1 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Mac').AsInteger * (g_Config.nItemsACPowerRate / 10));
            StdItem.MAC2 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('MAc2').AsInteger * (g_Config.nItemsACPowerRate / 10));
            StdItem.DC1 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Dc').AsInteger * (g_Config.nItemsPowerRate / 10));
            StdItem.DC2 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Dc2').AsInteger * (g_Config.nItemsPowerRate / 10));
            StdItem.MC1 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Mc').AsInteger * (g_Config.nItemsPowerRate / 10));
            StdItem.MC2 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Mc2').AsInteger * (g_Config.nItemsPowerRate / 10));
            StdItem.SC1 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Sc').AsInteger * (g_Config.nItemsPowerRate / 10));
            StdItem.SC2 := Random(8888) * nParam^ +
              Round(TempQuery.FieldByName('Sc2').AsInteger * (g_Config.nItemsPowerRate / 10));
            StdItem.Need := TempQuery.FieldByName('Need').AsInteger;
            StdItem.NeedLevel := TempQuery.FieldByName('NeedLevel').AsInteger;
            StdItem.Price := TempQuery.FieldByName('Price').AsInteger;
            StdItem.NeedIdentify := Integer(GetGameLogItemNameList(StdItem.Name));
            StdItem.Stock := TempQuery.FieldByName('Stock').AsInteger;
            StdItem.Color := TempQuery.FieldByName('Color').AsInteger; // 地面物品显示颜色
            StdItem.OverLap := TempQuery.FieldByName('OverLap').AsInteger; // 是否是重叠物品
            StdItem.Light := TempQuery.FieldByName('Light').AsInteger; // 首饰发光效果 piaoyun 2013-08-01
            StdItem.Horse := TempQuery.FieldByName('Horse').AsInteger;
            StdItem.Expand1 := TempQuery.FieldByName('Expand1').AsInteger;
            StdItem.Expand2 := TempQuery.FieldByName('Expand2').AsInteger;
            StdItem.Expand3 := TempQuery.FieldByName('Expand3').AsInteger;
            StdItem.Expand4 := TempQuery.FieldByName('Expand4').AsInteger;
            StdItem.Expand5 := TempQuery.FieldByName('Expand5').AsInteger;
            StdItem.Elements[0] := TempQuery.FieldByName('Element').AsInteger;
            StdItem.Elements[1] := TempQuery.FieldByName('Element1').AsInteger;
            StdItem.Elements[2] := TempQuery.FieldByName('Element2').AsInteger;
            StdItem.Elements[3] := TempQuery.FieldByName('Element3').AsInteger;
            StdItem.Elements[4] := TempQuery.FieldByName('Element4').AsInteger;
            StdItem.Elements[5] := TempQuery.FieldByName('Element5').AsInteger;
            StdItem.Elements[6] := TempQuery.FieldByName('Element6').AsInteger;
            StdItem.Elements[7] := TempQuery.FieldByName('Element7').AsInteger;
            StdItem.Elements[8] := TempQuery.FieldByName('Element8').AsInteger;
            StdItem.Elements[9] := TempQuery.FieldByName('Element9').AsInteger;
            StdItem.Elements[10] := TempQuery.FieldByName('Element10').AsInteger;
            StdItem.Elements[11] := TempQuery.FieldByName('Element11').AsInteger;
            StdItem.Elements[12] := TempQuery.FieldByName('Element12').AsInteger;
            StdItem.Elements[13] := TempQuery.FieldByName('Element13').AsInteger;
            StdItem.Elements[14] := TempQuery.FieldByName('Element14').AsInteger;
            StdItem.Elements[15] := TempQuery.FieldByName('Element15').AsInteger;
            StdItem.Elements[16] := TempQuery.FieldByName('Element16').AsInteger;
            StdItem.Elements[17] := TempQuery.FieldByName('Element17').AsInteger;
            StdItem.Elements[18] := TempQuery.FieldByName('Element18').AsInteger;
            StdItem.Elements[19] := TempQuery.FieldByName('Element19').AsInteger;
            StdItem.Elements[20] := TempQuery.FieldByName('Element20').AsInteger;
            StdItem.Elements[21] := TempQuery.FieldByName('Element21').AsInteger;
            StdItem.Elements[22] := TempQuery.FieldByName('Element22').AsInteger;
            StdItem.Elements[23] := TempQuery.FieldByName('Element23').AsInteger;

            if TempQuery.Fields.FindField('Element24') <> nil then
              StdItem.Elements[24] := TempQuery.FieldByName('Element24').AsInteger;

            StdItem.InsuranceGold := TempQuery.FieldByName('InsuranceGold').AsInteger;
            StdItem.InsuranceCurrency := TempQuery.FieldByName('InsuranceCurrency').AsInteger;
            StdItem.BodyEffect.FileIndex := -1; // 内观物品发光效果 文件编号 0
            StdItem.BodyEffect.ImageStart := 0; // 内观物品发光效果 读取位置
            StdItem.BodyEffect.ImageCount := 0; // 内观物品发光效果 读取张数
            StdItem.BodyEffect.OffsetX := 0; // 内观物品发光效果 微调X
            StdItem.BodyEffect.OffsetY := 0; // 内观物品发光效果 微调Y
            StdItem.BagEffect.FileIndex := -1; // 物品发光效果 文件编号 0
            StdItem.BagEffect.ImageStart := 0; // 物品发光效果 读取位置
            StdItem.BagEffect.ImageCount := 0; // 物品发光效果 读取张数
            StdItem.BagEffect.OffsetX := 0; // 包裹中的物品发光效果 微调X
            StdItem.BagEffect.OffsetY := 0; // 包裹中的物品发光效果 微调Y
            StdItem.Effect := Int64(nil);

            if UserEngine.StdItemList.Count = Idx then
            begin
              if StdItem.Name = sSTRING_GOLDNAME then
                g_btGoldItemColor := StdItem.Color;

              UserEngine.StdItemList.Add(StdItem);
              nIndex := GetEffectIndexItemList(StdItem.Name);
              if nIndex > 0 then
              begin
                ItemEffect := g_ItemEffects.Get(nIndex);
                if ItemEffect <> nil then
                begin
                  StdItem.BodyEffect.FileIndex := ItemEffect.FileIndex1; // 内观物品发光效果 文件编号 0
                  StdItem.BodyEffect.ImageStart := ItemEffect.StartIndex1; // 内观物品发光效果 读取位置
                  StdItem.BodyEffect.ImageCount := ItemEffect.ImageCount1; // 内观物品发光效果 读取张数
                  StdItem.BodyEffect.OffsetX := ItemEffect.OffSetX1; // 内观物品发光效果 微调X
                  StdItem.BodyEffect.OffsetY := ItemEffect.OffSetY1; // 内观物品发光效果 微调Y
                  StdItem.BodyEffect.Time := ItemEffect.Time1;
                  StdItem.BodyEffect.IsDrawCenter := ItemEffect.DrawCenter1;
                  StdItem.BodyEffect.IsDrawNoBlend := ItemEffect.NoBlendMode1;
                  StdItem.BodyEffect.IsDrawBelow := ItemEffect.boEfectBelowItem1;
                  StdItem.BagEffect.FileIndex := ItemEffect.FileIndex3; // 包裹中的物品发光效果 文件编号 0
                  StdItem.BagEffect.ImageStart := ItemEffect.StartIndex3; // 包裹中的物品发光效果 读取位置
                  StdItem.BagEffect.ImageCount := ItemEffect.ImageCount3; // 包裹中的物品发光效果 读取张数
                  StdItem.BagEffect.OffsetX := ItemEffect.OffSetX3; // 包裹中的物品发光效果 微调X
                  StdItem.BagEffect.OffsetY := ItemEffect.OffSetY3; // 包裹中的物品发光效果 微调Y
                  StdItem.BagEffect.Time := ItemEffect.Time3;
                  StdItem.BagEffect.IsDrawCenter := ItemEffect.DrawCenter3;
                  StdItem.BagEffect.IsDrawNoBlend := ItemEffect.NoBlendMode3;
                  StdItem.Effect := Int64(ItemEffect);
                  // MainOutMessage(Format('BodyEffectOffsetX:%d BodyEffectOffsetY:%d EffectOffsetX:%d EffectOffsetY:%d Name:%s',

                  // [Idx,StdItem.BodyEffectOffsetX,StdItem.BodyEffectOffsetY,StdItem.EffectOffsetX,StdItem.EffectOffsetY, StdItem.Name]));
                end;
              end;
              g_SndaShopList.UpdateStdItem(StdItem);
              Result := 1;
            end
            else
            begin
              DisPose(nParam);
              MainOutMessage(Format('加载物品(Idx:%d Name:%s)数据失败！', [Idx, StdItem.Name]), False);
              Result := -100;
              Exit;
            end;
            TempQuery.Next;
          end;
          DisPose(nParam);

          g_boGameLogGold := GetGameLogItemNameList(sSTRING_GOLDNAME);
          g_boGameLogHumanDie := GetGameLogItemNameList(g_sHumanDieEvent);
          g_boGameLogGameGold := GetGameLogItemNameList(g_Config.sGameGoldName);
          g_boGameLogGamePoint := GetGameLogItemNameList(g_Config.sGamePointName);
          g_boGameLogCreditPoint := GetGameLogItemNameList(g_Config.sCreditPointName);
        finally
          if TempQuery <> nil then
            TempQuery.Close;
        end;

        UserEngine.InitSortStdItemList;
        InBytes := UserEngine.StdItemList.Count * SizeOf(TStdItem);
        GetMem(InBuf, InBytes);
        try
          Buffer := InBuf;
          // 前面4字节用于记录完整字段数量、非完整字段数量
          Inc(Buffer, SizeOf(TStdItemsHeader));
          PartialFieldsCount := 0;
          AllFiledsCount := 0;

          // 所有数据只发了Looks，暂时用于客户端 'ITEMSHOW:'
          for I := 0 to UserEngine.StdItemList.Count - 1 do
          begin
            StdItem := UserEngine.StdItemList.Items[I];
            FillChar(PartialStdItem.Name[1], SizeOf(PartialStdItem.Name), 0);
            PartialStdItem.Name := StdItem^.Name;
            PartialStdItem.Looks := StdItem^.Looks;
            // 这里主要用于及时雨内挂区分药品类型
            PartialStdItem.StdMode := StdItem^.StdMode;
            PartialStdItem.Shape := StdItem^.Shape;
            // 搞成这样主要是为了减流量 chongchong 2015-02-07
            PartialStdItem._Horse := Min(StdItem^.Horse, High(Byte));
            Move(PartialStdItem, Buffer^, SizeOf(TPartialStdItem));
            Buffer := Buffer + SizeOf(TPartialStdItem);
            Inc(PartialFieldsCount);
          end;

          // 镶嵌宝石的一次发过去
          for I := 0 to UserEngine.StdItemList.Count - 1 do
          begin
            StdItem := UserEngine.StdItemList.Items[I];
            if ((StdItem.StdMode = 46) and (StdItem.Shape = 3)) or (StdItem.StdMode = 93) then
            begin
              CompleteStdItem.Index := I;
              CompleteStdItem.StdItem := StdItem^;
              Move(CompleteStdItem, Buffer^, SizeOf(TCompleteStdItem));
              Buffer := Buffer + SizeOf(TCompleteStdItem);
              Inc(AllFiledsCount);
            end;
          end;

          StdItemsHeader := PTStdItemsHeader(InBuf);
          StdItemsHeader.PartialFieldsCount := PartialFieldsCount;
          StdItemsHeader.AllFiledsCount := AllFiledsCount;
          g_StdItemListTextLen := SizeOf(TStdItemsHeader) + PartialFieldsCount * SizeOf(TPartialStdItem) + AllFiledsCount *
            SizeOf(TCompleteStdItem);
          g_StdItemListText := zLibCompressBuffer(InBuf, g_StdItemListTextLen);
          g_StdItemListTextCRC := BufferCrc(PAnsiChar(g_StdItemListText), Length(g_StdItemListText));
        finally
          FreeMem(InBuf, InBytes);
        end;
{$IF MULTI_THREAD = 1}
      finally
        if g_MultiThreadRun then
          UserEngine.StdItemList.UnLockW;
      end;
{$IFEND}
    finally
      ClientDataSet.Free;
    end;
  finally
    if g_boUseSqliteDB and AutoConnect then
      FrmDB.SQLiteDB.Connected := False;
  end;
end;

function TFrmDB.LoadMagicDB(): Integer;
var
  I, II: Integer;
  Magic: pTMagic;
  J: Integer;
  IsFoundCustomMagic: Boolean;
  CustomMagicConfig: TCustomMagicConfig;
  InBuf: PAnsiChar;
  InBytes: Integer;
  ClientConfig: PClientCustomMagicConfig;
  TempQuery: TDataSet;
{$IFDEF USE_FDTABLE}
  ClientDataSet: TFDMemTable;
{$ELSE}
  ClientDataSet: TkbmMemTable;
{$ENDIF}
  CustomMagicList: TList;
  nRet, ColumnCount: Integer;
  ColumnName, ColumnType: string;
  FieldType: TFieldType;
  FieldLen: Integer;
  FieldDec: Integer;
  sm: TSQLStatement;
resourcestring
  sSQLString = 'select * from Magic';
begin
  UserEngine.m_MagicACList.Clear;
  Result := 0;
  UserEngine.SwitchMagicList();
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_MagicList.LockW(1);
  try
{$IFEND}
    for I := 0 to UserEngine.m_MagicList.Count - 1 do
    begin
      DisPose(pTMagic(UserEngine.m_MagicList.Items[I]));
    end;
    UserEngine.m_MagicList.Clear;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_MagicList.UnLockW;
  end;
{$IFEND}
  ClientDataSet := {$IFDEF USE_FDTABLE} TFDMemTable.Create(nil); {$ELSE}
    TkbmMemTable.Create(nil); {$ENDIF}
  CustomMagicList := TList.Create;
  try
    if g_boUseSqliteDB then
    begin
      TempQuery := ClientDataSet;
      sm := SQLiteDB.Statements.AddSQLStatement('Select_Magic');
      sm.Sql := 'select * from Magic';
      sm.Prepare;
      try
        nRet := sm.Step;
        if nRet = SQLITE_ROW then
        begin
          ColumnCount := sm.GetColumnCount;
          for I := 0 to ColumnCount - 1 do
          begin
            ColumnName := sm.GetColumnName(I);
            ColumnType := sm.GetColumnDeclType(I);
            if ColumnType = '' then
              GetFieldInfo('string', FieldType, FieldLen, FieldDec) // OL
            else
              GetFieldInfo(ColumnType, FieldType, FieldLen, FieldDec);
            if (FieldType <> ftString) and (FieldType <> ftWideString) then
            begin
              with ClientDataSet.FieldDefs.AddFieldDef do
              begin
                Name := ColumnName;
                DataType := FieldType;
                if FieldType = ftFloat then
                  Precision := FieldDec;
              end
            end
            else
            begin
              with ClientDataSet.FieldDefs.AddFieldDef do
              begin
                Name := ColumnName;
                DataType := FieldType;
                Size := FieldLen;
              end;
            end;
          end;
{$IFDEF USE_FDTABLE}
          ClientDataSet.CreateDataSet;
{$ELSE}
          ClientDataSet.CreateTable;
{$ENDIF}
          ClientDataSet.Active := True;
          while (nRet = SQLITE_ROW) do
          begin
            ClientDataSet.Append;
            for I := 0 to ColumnCount - 1 do
            begin
              case ClientDataSet.FieldDefs[I].DataType of
                ftWideString:
                  begin
                    ClientDataSet.Fields[I].AsString := sm.GetColumnValueText(I);
                  end;
                ftString:
                  begin // DI
                    ClientDataSet.Fields[I].AsString := sm.GetColumnValueText(I);
                  end;
                ftInteger:
                  begin
                    ClientDataSet.Fields[I].AsInteger := sm.GetColumnValueInt(I);
                  end;
              else
                begin
                  raise Exception.Create('Magic 无法识别的数据类型, 字段：' + ClientDataSet.FieldDefs[I].Name);
                end;
              end;
            end;
            nRet := sm.Step;
          end;
        end;
      finally
        sm.Finalize;
      end;
    end
    else
    begin
{$IFNDEF CPUX64}
      TempQuery := Query;
      Query.Sql.Clear;
      Query.Sql.Add(sSQLString);
      try
        Query.Open;
      finally
        Result := -2;
      end;
{$ELSE}
      Result := -1000;
      MainOutMessage('[错误]***64位引擎不支持BDE数据库***', False);
      Exit;
{$ENDIF}
    end;
    TempQuery.First;
    for I := 0 to TempQuery.RecordCount - 1 do
    begin
      New(Magic);
      FillChar(Magic^, SizeOf(TMagic), #0);
      Magic.wMagicId := TempQuery.FieldByName('MagId').AsInteger;
      Magic.sMagicName := TempQuery.FieldByName('MagName').AsString;
      Magic.btEffectType := TempQuery.FieldByName('EffectType').AsInteger;
      Magic.btEffect := TempQuery.FieldByName('Effect').AsInteger;
      Magic.wSpell := TempQuery.FieldByName('Spell').AsInteger;
      Magic.wPower := TempQuery.FieldByName('Power').AsInteger;
      Magic.wMaxPower := TempQuery.FieldByName('MaxPower').AsInteger;
      Magic.btJob := TempQuery.FieldByName('Job').AsInteger;
      Magic.btTrainLv := TempQuery.FieldByName('MaxTrainLv').AsInteger;
      for II := 0 to High(Magic.TrainLevel) - 1 do
      begin
        Magic.TrainLevel[II] := TempQuery.FieldByName(Format('NeedL%d', [II + 1])).AsInteger;
        Magic.MaxTrain[II] := TempQuery.FieldByName(Format('L%dTrain', [II + 1])).AsInteger;
      end;
      Magic.TrainLevel[High(Magic.TrainLevel)] := Magic.TrainLevel[High(Magic.TrainLevel) - 1];
      Magic.MaxTrain[High(Magic.TrainLevel)] := Magic.MaxTrain[High(Magic.TrainLevel) - 1];
      { Magic.TrainLevel[0] := TempQuery.FieldByName('NeedL1').AsInteger;
        Magic.TrainLevel[1] := TempQuery.FieldByName('NeedL2').AsInteger;
        Magic.TrainLevel[2] := TempQuery.FieldByName('NeedL3').AsInteger;
        Magic.TrainLevel[3] := TempQuery.FieldByName('NeedL3').AsInteger;
        Magic.MaxTrain[0] := TempQuery.FieldByName('L1Train').AsInteger;
        Magic.MaxTrain[1] := TempQuery.FieldByName('L2Train').AsInteger;
        Magic.MaxTrain[2] := TempQuery.FieldByName('L3Train').AsInteger;
        Magic.MaxTrain[3] := Magic.MaxTrain[2];
        Magic.btTrainLv := 3;
      }
      Magic.dwMagicDelayTime := TempQuery.FieldByName('Delay').AsInteger;
      Magic.wDefSpell := TempQuery.FieldByName('DefSpell').AsInteger;
      Magic.wDefPower := TempQuery.FieldByName('DefPower').AsInteger;
      Magic.wDefMaxPower := TempQuery.FieldByName('DefMaxPower').AsInteger;
      Magic.sDescr := TempQuery.FieldByName('Descr').AsString;
      Magic.CanUpgrade := TempQuery.FieldByName('CanUpgrade').AsInteger;
      Magic.MaxUpgradeLevel := TempQuery.FieldByName('MaxUpgradeLv').AsInteger;
      if Magic.wMagicId > 0 then
      begin
        if Magic.sDescr = '英雄' then
          Magic.MagicAttr := mtHero
        else if (Magic.sDescr = '静之') then
          Magic.MagicAttr := mtDefense
        else if (Magic.sDescr = '怒之') then
          Magic.MagicAttr := mtAttack
        else if Magic.wMagicId in [100 .. 111] then
          Magic.MagicAttr := mtContinuous
        else
        begin
          Magic.MagicAttr := mtHum;
          if Magic.wMagicId in [12 { 刺杀 } , 56 { 逐日剑法 } , 66 { 开天 } , 113 { 断空斩 } { , 115血魄一击 } , 100 { 追心刺 } , 101 { 三绝杀 } , 102
          { 断岳斩 } , 103 { 横扫千军 } ] then
          begin
            UserEngine.m_MagicACList.Add(Magic.wMagicId, Magic.sMagicName);
          end;
        end;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.m_MagicList.LockW(2);
        try
{$IFEND}
          UserEngine.m_MagicList.Add(Magic);
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            UserEngine.m_MagicList.UnLockW;
        end;
{$IFEND}
        if Magic.wMagicId = 202 then
        begin
          if UserEngine.m_SKILL_202Magic = nil then
            New(UserEngine.m_SKILL_202Magic);
          UserEngine.m_SKILL_202Magic.MagicInfo := Magic;
          UserEngine.m_SKILL_202Magic.MagicAttr := Magic.MagicAttr;
          UserEngine.m_SKILL_202Magic.wMagIdx := Magic.wMagicId;
          UserEngine.m_SKILL_202Magic.btLevel := 0;
          UserEngine.m_SKILL_202Magic.btNewLevel := 0;
          UserEngine.m_SKILL_202Magic.nTranPoint := Magic.MaxTrain[UserEngine.m_SKILL_202Magic.btLevel];
          UserEngine.m_SKILL_202Magic.boUsesItemAdd := False;
        end
        else if CheckIsCustomMagic(Magic.wMagicId) then
        begin
          CustomMagicList.Add(Magic);
        end;
      end
      else
      begin
        DisPose(Magic);
      end;
      Result := 1;
      TempQuery.Next;
    end;
    TempQuery.Close;
    // 如果一个自定义技能，定义了人物和英雄的，优先人物
    for I := 0 to CustomMagicList.Count - 1 do
    begin
      Magic := CustomMagicList.Items[I];
      if (Magic.MagicAttr <> mtHero) then
      begin
        IsFoundCustomMagic := False;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.m_CustomMagicList.LockW(1);
        try
{$IFEND}
          for J := 0 to UserEngine.m_CustomMagicList.Count - 1 do
          begin
            CustomMagicConfig := UserEngine.m_CustomMagicList[J];
            if CustomMagicConfig.MagicID = Magic.wMagicId then
            begin
              CustomMagicConfig.MagicName := Magic.sMagicName;
              CustomMagicConfig.IsMagicWarr := Magic.btEffectType = 0;
              // CustomMagicConfig.MagicAttr := Magic.MagicAttr;
              IsFoundCustomMagic := True;
              Break;
            end;
          end;
          if not IsFoundCustomMagic then
          begin
            CustomMagicConfig := TCustomMagicConfig.Create(Magic.sMagicName, Magic.wMagicId, Magic.btEffectType = 0);
            CustomMagicConfig.IsMagicWarr := Magic.btEffectType = 0;
            // CustomMagicConfig.MagicAttr := Magic.MagicAttr;
            UserEngine.m_CustomMagicList.Add(CustomMagicConfig);
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            UserEngine.m_CustomMagicList.UnLockW;
        end;
{$IFEND}
      end;
    end;
    // 如果一个自定义技能，定义了人物和英雄的，优先人物，再次英雄
    for I := 0 to CustomMagicList.Count - 1 do
    begin
      Magic := CustomMagicList.Items[I];
      if (Magic.MagicAttr = mtHero) then
      begin
        IsFoundCustomMagic := False;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.m_CustomMagicList.LockW(1);
        try
{$IFEND}
          for J := 0 to UserEngine.m_CustomMagicList.Count - 1 do
          begin
            CustomMagicConfig := UserEngine.m_CustomMagicList[J];
            if CustomMagicConfig.MagicID = Magic.wMagicId then
            begin
              // CustomMagicConfig.MagicName := Magic.sMagicName;
              // CustomMagicConfig.IsMagicWarr := Magic.btEffectType = 0;
              // CustomMagicConfig.MagicAttr := Magic.MagicAttr;
              IsFoundCustomMagic := True;
              Break;
            end;
          end;
          if not IsFoundCustomMagic then
          begin
            CustomMagicConfig := TCustomMagicConfig.Create(Magic.sMagicName, Magic.wMagicId, Magic.btEffectType = 0);
            CustomMagicConfig.IsMagicWarr := Magic.btEffectType = 0;
            // CustomMagicConfig.MagicAttr := Magic.MagicAttr;
            UserEngine.m_CustomMagicList.Add(CustomMagicConfig);
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            UserEngine.m_CustomMagicList.UnLockW;
        end;
{$IFEND}
      end;
    end;
  finally
    ClientDataSet.Free;
    CustomMagicList.Free;
  end;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomMagicList.LockR(2);
  try
{$IFEND}
    InBytes := UserEngine.m_CustomMagicList.Count * SizeOf(TClientCustomMagicConfig);
    GetMem(InBuf, InBytes + 1);
    try
      ClientConfig := PClientCustomMagicConfig(InBuf);
      for I := 0 to UserEngine.m_CustomMagicList.Count - 1 do
      begin
        CustomMagicConfig := UserEngine.m_CustomMagicList.Items[I];
        ClientConfig^.wMagicId := CustomMagicConfig.MagicID;
        ClientConfig^.MagicBaseConfig := CustomMagicConfig.ClientBaseConfig;
        ClientConfig^.MagicConfigs := CustomMagicConfig.ClientConfigs;
        ClientConfig^.boIsMagicWarr := CustomMagicConfig.IsMagicWarr;
        if CustomMagicConfig.IsMagicWarr then
          ClientConfig^.btNearAttackRange := CustomMagicConfig.ServerConfig.AttackNearRange
        else
          ClientConfig^.btNearAttackRange := 1;
        ClientConfig^.IsAttackUseNG := CustomMagicConfig.ServerConfig.IsAttackUseNG;
        ClientConfig^.NoChangeDir := CustomMagicConfig.ServerConfig.NoChangeDir;
        // ClientConfig^.FailMsg := CustomMagicConfig.ServerConfig.FailMsg;
        if not CustomMagicConfig.ServerConfig.IsCheckVarValue then
        begin
          ClientConfig.NeedItem := CustomMagicConfig.ServerConfig.NeedItem;
          ClientConfig.NeedItemCount := CustomMagicConfig.ServerConfig.NeedItemCount;
          ClientConfig.NeedItemCustomItemName := CustomMagicConfig.ServerConfig.NeedItemCustomItemName;
          ClientConfig.NeedItemUseBagItem := CustomMagicConfig.ServerConfig.NeedItemUseBagItem;
        end
        else
        begin
          ClientConfig.NeedItem := meiNone;
          ClientConfig.NeedItemCount := 0;
          ClientConfig.NeedItemCustomItemName := '';
          ClientConfig.NeedItemUseBagItem := False;
        end;
        Inc(ClientConfig);
      end;
      g_CustomMagicListTextLen := InBytes;
      g_CustomMagicListText := zLibCompressBuffer(InBuf, InBytes);
      g_CustomMagicListTextCRC := BufferCrc(PAnsiChar(g_CustomMagicListText), Length(g_CustomMagicListText));
    finally
      FreeMem(InBuf, InBytes + 1);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMagicList.UnLockR;
  end;
{$IFEND}
  UserEngine.m_boStartLoadMagic := False;
  UserEngine.m_MagicACList.Sort;
  LoadMagicACList;
end;

function TFrmDB.LoadMakeItem(): Integer;
var
  I, n14: Integer;
  s18, s20, s24: string;
  LoadList: TStringList;
  sFileName: string;
  List28: TStringList;
begin
  Result := -1;
  sFileName := g_Config.sEnvirDir + 'MakeItem.txt';
  if not FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.Add(';制造物品列表');
      LoadList.SaveToFile(sFileName);
    finally
      LoadList.Free;
    end;
  end;
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    List28 := nil;
    s24 := '';
    for I := 0 to LoadList.Count - 1 do
    begin
      s18 := Trim(LoadList.Strings[I]);
      if (s18 <> '') and (s18[1] <> ';') then
      begin
        if s18[1] = '[' then
        begin
          if List28 <> nil then
            g_MakeItemList.AddObject(s24, List28);
          List28 := TStringList.Create;
          ArrestStringEx(s18, '[', ']', s24);
        end
        else
        begin
          if List28 <> nil then
          begin
            s18 := GetValidStr3(s18, s20, [' ', #9]);
            n14 := StrToIntDef(Trim(s18), 1);
            List28.AddObject(s20, TObject(n14));
          end;
        end;
      end;
    end;

    if List28 <> nil then
      g_MakeItemList.AddObject(s24, List28);
    LoadList.Free;
    Result := 1;
  end;
end;

function TFrmDB.LoadMapInfo: Integer;

  function LoadMapQuest(sName: string): TMerchant;
  var
    QuestNPC: TMerchant;
  begin
    QuestNPC := TMerchant.Create;
    QuestNPC.m_sMapName := '0';
    QuestNPC.m_nCurrX := 0;
    QuestNPC.m_nCurrY := 0;
    QuestNPC.m_sCharName := sName;
    QuestNPC.m_nFlag := 0;
    QuestNPC.m_wAppr := 0;
    QuestNPC.m_sFilePath := 'MapQuest_def\';
    QuestNPC.m_boIsHide := True;
    QuestNPC.m_boIsQuest := False;
    UserEngine.AddNpc(sName, QuestNPC);
    Result := QuestNPC;
  end;

  procedure LoadSubMapInfo(LoadList: TStringList; sFileName: string);
  resourcestring
    HookError = '[Exception] HookLoadScriptFile MapInfo.txt';
  var
    I: Integer;
    sFilePatchName, sFileDir, s30: string;
    LoadMapList: TStringList;
    MemoryStream: TMemoryStream;
  begin
    sFileDir := g_Config.sEnvirDir + 'MapInfo\';
    if not DirectoryExists(sFileDir) then
      CreateDir(sFileDir);

    sFilePatchName := sFileDir + sFileName;
    if FileExists(sFilePatchName) then
    begin
      LoadMapList := TStringList.Create;
      LoadMapList.LoadFromFile(sFilePatchName);
      DeCodeStringList(LoadMapList);
      for I := 0 to LoadMapList.Count - 1 do
        LoadList.Add(LoadMapList.Strings[I]);

      LoadMapList.Free;
    end
    else
    begin
      if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
      begin
        try
          MemoryStream := TMemoryStream.Create;
          if not g_PluginManager.HookLoadScriptFile(PAnsiChar(AnsiString('MapInfo\' + sFileName)), MemoryStream) then
          begin
            MemoryStream.Free;
            Exit;
          end;

          if (MemoryStream.Size > 0) then
          begin
            s30 := DeCodePlugBuffer(MemoryStream);
            LoadMapList := TStringList.Create;
            LoadMapList.Text := s30;
            DeCodeStringList(LoadMapList);
            for I := 0 to LoadMapList.Count - 1 do
              LoadList.Add(LoadMapList.Strings[I]);

            LoadMapList.Free;
          end;
          MemoryStream.Free;
        except
          on E: Exception do
          begin
            MainOutMessage(HookError);
            MainOutMessage(E.Message);
            Exit;
          end;
        end;
      end;
    end;
  end;

  function CopyX(S: string; cStart, cStop: Char): string;
  var
    I: Integer;
    nStartPos, nStartCount, nStopCount: Integer;
  begin
    Result := '';
    nStartCount := 0;
    nStopCount := 0;
    nStartPos := 0;
    for I := 1 to Length(S) do
    begin
      if nStartPos <= 0 then
      begin
        if S[I] = cStart then
        begin
          nStartPos := I + 1;
          Inc(nStartCount);
        end;
      end
      else
      begin
        if S[I] = cStart then
          Inc(nStartCount)
        else if S[I] = cStop then
          Inc(nStopCount);

        if nStartCount = nStopCount then
        begin
          Result := Copy(S, nStartPos, I - nStartPos);
          Break;
        end;
      end;
    end;
  end;

resourcestring
  HookError = '[Exception] HookLoadScriptFile MapInfo.txt';
var
  sFileName: string;
  LoadList: TStringList;
  I: Integer;
  s28, s30, s34, s38, sMapName, sMainMapName, s44, sMapDesc, s4C, sReConnectMap: string;
  sSecretFlag, sSecretShowName, sSecretDressShap, sSecretWeaponShap: string;
  sTimeMapCode, sTimeMapMin, sTimeMapShowLeft, sTimeMapLabel: string;
  n14, n18, n1C, n20: Integer;
  nServerIndex: Integer;
  MapFlag: TMapFlag;
  QuestNPC: TMerchant;
  sMapInfoFile: string;
  sCaption: string;
  MemoryStream: TMemoryStream;
  k: Integer;
  boFB: Boolean;
  FBList: TList;
  nFBCount: Integer;
  EnterLimit: TFBEnterLimit;
  EnterDelayMin: Integer;
  NoHumClearFBMin: Integer;
  sFBName: string;
  Envir: TEnvirnoment;
  IntTemp: Integer;
begin
  Result := -1;
  sCaption := FrmMain.Caption;
  sFileName := g_Config.sEnvirDir + 'MapInfo.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    if LoadList.Count < 0 then
    begin
      LoadList.Free;
      Exit;
    end;
  end
  else
  begin
    if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
    begin
      try
        MemoryStream := TMemoryStream.Create;
        if not g_PluginManager.HookLoadScriptFile(PAnsiChar('MapInfo.txt'), MemoryStream) then
        begin
          MemoryStream.Free;
          Exit;
        end;

        LoadList := TStringList.Create;
        if (MemoryStream.Size > 0) then
        begin
          // SetLength(s30, MemoryStream.Size);
          // Move(MemoryStream.Memory^, s30[1], MemoryStream.Size);
          s30 := DeCodePlugBuffer(MemoryStream);
          LoadList.Text := s30;
        end;
        MemoryStream.Free;
      except
        on E: Exception do
        begin
          MainOutMessage(HookError);
          MainOutMessage(E.Message);
          Exit;
        end;
      end;
    end
    else
      Exit;
  end;

  DeCodeStringList(LoadList);
  I := 0;
  while (True) do
  begin
    if I >= LoadList.Count then
      Break;

    if CompareLStr('loadmapinfo', LoadList.Strings[I], Length('loadmapinfo')) then
    begin
      sMapInfoFile := GetValidStr3(LoadList.Strings[I], s30, [' ', #9]);
      LoadList.Delete(I);
      if sMapInfoFile <> '' then
        LoadSubMapInfo(LoadList, sMapInfoFile);
    end;
    Inc(I);
  end;

  Result := 1;
  // 加载地图设置
  for I := 0 to LoadList.Count - 1 do
  begin
    s30 := LoadList.Strings[I];
    if (s30 <> '') and (s30[1] = '[') then
    begin
      ZeroMemory(@MapFlag, SizeOf(MapFlag)); // Cursor 2023-05-25 09:29:23

      sMapName := '';
      MapFlag.boSAFE := False;
      s30 := ArrestStringEx(s30, '[', ']', sMapName);
      sMapDesc := GetValidStrCap(sMapName, sMapName, [' ', ',', #9]);
      sMainMapName := Trim(GetValidStr3(sMapName, sMapName, ['|', '/', '\', #9])); // 获取重复利用地图
      if (sMapDesc <> '') and (sMapDesc[1] = '"') then
        ArrestStringEx(sMapDesc, '"', '"', sMapDesc);

      s4C := Trim(GetValidStr3(sMapDesc, sMapDesc, [' ', ',', #9]));
      nServerIndex := StrToIntDef(s4C, 0);
      if sMapName = '' then
        Continue;

      { -ochongchong -c内存泄露 : 去内存泄露【2013-07-18】 }
      // FillChar(MapFlag, SizeOf(TMapFlag), #0);
      MapFlag.nTHUNDER := 0;
      MapFlag.nLAVA := 0;
      MapFlag.boMISSION := False;
      MapFlag.boNODROPITEM := False;
      // 当前地图人物死亡多长时间后自动掉线 piaoyun 2013-09-05
      MapFlag.boDieTime := False;
      MapFlag.dwDieTime := 0;
      // 禁止丢物品 piaoyun 2013-09-05
      MapFlag.boNOTHROWITEM := False;
      // 当前地图魔血石、气血石、幻魔石无效 piaoyun 2013-09-05
      MapFlag.boNotStone := False;
      // 当前地图宝宝不攻击人物 chongchong 2013-12-11
      MapFlag.boSlaveNotAttackHuman := False;
      // 当前地图宝宝不攻击英雄 chongchong 2013-12-11
      MapFlag.boSlaveNotAttackHero := False;
      MapFlag.boSAFE := False;
      MapFlag.boDARK := False;
      MapFlag.boFIGHT := False;
      MapFlag.boFIGHT2 := False;
      MapFlag.boFIGHT3 := False;
      MapFlag.boFIGHT4 := False;
      MapFlag.boDAY := False;
      MapFlag.boQUIZ := False;
      MapFlag.boNORECONNECT := False;
      MapFlag.boMUSIC := False;
      MapFlag.boEXPRATE := False;
      MapFlag.boPKWINLEVEL := False;
      MapFlag.boPKWINEXP := False;
      MapFlag.boPKLOSTLEVEL := False;
      MapFlag.boPKLOSTEXP := False;
      MapFlag.boDECHP := False;
      MapFlag.boINCHP := False;
      MapFlag.boDECGAMEGOLD := False;
      MapFlag.boDECGAMEPOINT := False;
      MapFlag.boINCGAMEGOLD := False;
      MapFlag.boINCGAMEPOINT := False;
      MapFlag.boRUNHUMAN := False;
      MapFlag.boRUNMON := False;
      MapFlag.boNoRunHuman := False;
      MapFlag.boNoRunMon := False;
      MapFlag.boNEEDHOLE := False;
      MapFlag.boNORECALL := False;
      MapFlag.boNOGUILDRECALL := False;
      MapFlag.boNODEARRECALL := False;
      MapFlag.boNOMASTERRECALL := False;
      MapFlag.boNORANDOMMOVE := False;
      MapFlag.boNODRUG := False;
      MapFlag.boMINE := False;
      MapFlag.boNOPOSITIONMOVE := False;
      MapFlag.boNoManNoMon := False;
      MapFlag.boNight := False;
      MapFlag.boNOHORSE := False;
      MapFlag.boNoAutoOnline := False;
      MapFlag.boNoSwitchAttackMode := False;
      MapFlag.boNoHeroProtect := False;
      MapFlag.nL := 0;
      MapFlag.nNEEDSETONFlag := 0;
      MapFlag.nNeedONOFF := 0;
      MapFlag.sMusicFileName := '';
      MapFlag.nPKWINLEVEL := 0;
      MapFlag.nEXPRATE := 0;
      MapFlag.nPKWINEXP := 0;
      MapFlag.nPKLOSTLEVEL := 0;
      MapFlag.nPKLOSTEXP := 0;
      MapFlag.nDECHPPOINT := 0;
      MapFlag.nDECHPTIME := 0;
      MapFlag.nINCHPPOINT := 0;
      MapFlag.nINCHPTIME := 0;
      MapFlag.nDECGAMEGOLD := 0;
      MapFlag.nDECGAMEGOLDTIME := 0;
      MapFlag.nDECGAMEPOINT := 0;
      MapFlag.nDECGAMEPOINTTIME := 0;
      MapFlag.nINCGAMEGOLD := 0;
      MapFlag.nINCGAMEGOLDTIME := 0;
      MapFlag.nINCGAMEPOINT := 0;
      MapFlag.nINCGAMEPOINTTIME := 0;
      MapFlag.sReConnectMap := '';
      MapFlag.sMUSICName := '';
      MapFlag.boFIGHTPK := False; // PK可以爆装备不红名
      MapFlag.boNOFIREMAGIC := False;
      MapFlag.boUnAllowStdItems := False;
      MapFlag.sUnAllowStdItemsText := '';
      MapFlag.boNOTALLOWUSEMAGIC := False;
      MapFlag.sUnAllowMagicText := '';
      MapFlag.boNORECALLHERO := False;
      MapFlag.boNoCallPet := False;
      MapFlag.boAllowUseMyshop := False;
      MapFlag.boWeatherEffect1 := False;
      MapFlag.boWeatherEffect2 := False;
      MapFlag.boWeatherEffect3 := False;
      MapFlag.boOnKillMob := False;
      MapFlag.nSAYLEVEL := 0;
      MapFlag.boDELDROPITEM := False;
      MapFlag.nRevivalMaxCount := 0;
      MapFlag.nRevivalCheckTime := 0;
      MapFlag.boNODROPUSEITEMS := False;
      MapFlag.boNOSAFEPOSITIONMOVE := False;
      MapFlag.boHITMON := False;
      MapFlag.sHITMON := '';
      MapFlag.boNoChallenge := False;
      MapFlag.boNoDeal := False; // 禁止交易
      MapFlag.boNoShop := False; // 禁止商铺
      MapFlag.sDropItemAddUserBag := '';
      MapFlag.boNoAuction := False;
      MapFlag.boNoAutoDropItemToBag := False; // 禁止物品自动入包
      MapFlag.boNoAutoRangePickItem := False; // 禁止范围拾取

      MapFlag.nL := 1;
      QuestNPC := nil;
      MapFlag.nNEEDSETONFlag := -1;
      MapFlag.nNeedONOFF := -1;
      MapFlag.sUnAllowStdItemsText := '';
      MapFlag.sUnAllowMagicText := '';
      MapFlag.boAllowUseMyshop := False;
      MapFlag.boDECHP := False;
      MapFlag.boINCHP := False;
      MapFlag.boDECGAMEGOLD := False;
      MapFlag.boDECGAMEPOINT := False;
      MapFlag.boINCGAMEGOLD := False;
      MapFlag.boINCGAMEPOINT := False;
      MapFlag.boDAY := False;
      MapFlag.boDARK := False;
      MapFlag.boOnKillMob := False;
      MapFlag.boNODROPUSEITEMS := False;
      MapFlag.nSAYLEVEL := -1;
      MapFlag.boDELDROPITEM := False;
      MapFlag.nRevivalMaxCount := -1;
      MapFlag.nRevivalCheckTime := 30000;
      MapFlag.boNOSAFEPOSITIONMOVE := False;
      MapFlag.sMusicFileName := '';
      MapFlag.SecretFlag := 0;
      MapFlag.SecretShowName := '';
      MapFlag.SecretDressShap := 0;
      MapFlag.SecretWeaponShap := 0;
      MapFlag.TimeMapCode := '';
      MapFlag.TimeMapMin := 0;
      MapFlag.TimeMapShowLeft := False;
      MapFlag.TimeMapLabel := '';
      MapFlag.boNeedLevelTime := False;
      MapFlag.dwNeedLevelPoint := 0;
      boFB := False;
      nFBCount := -1;
      EnterLimit := fbel_OnlyCreater;
      EnterDelayMin := 0;
      NoHumClearFBMin := 10;
      while (True) do
      begin
        if s30 = '' then
          Break;

        s28 := s30;
        // s30 := GetValidStr3(s30, s34, [' ', ',', #9]);  // 副本地图 FB(4,祖玛副本,1)中有','，则不过滤
        s30 := GetValidStr3(s30, s34, [' ', #9]);
        if s34 = '' then
          Break;

        // MapFlag.sMusicFileName := '';
        // 增加两个地图命令
        if CompareLStr(s34, 'THUNDER', Length('THUNDER')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nTHUNDER := StrToIntDef(s38, 0);
          Continue;
        end;

        if CompareLStr(s34, 'LAVA', Length('LAVA')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nLAVA := StrToIntDef(s38, 0);
          Continue;
        end;

        // 地图参数 piaoyun 2013-07-25
        if CompareText(s34, 'MISSION') = 0 then
        begin
          MapFlag.boMISSION := True;
          Continue;
        end;

        if CompareText(s34, 'NODROPITEM') = 0 then
        begin
          MapFlag.boNODROPITEM := True;
          Continue;
        end;

        // 地图参数死亡时间控制 piaoyun 2013-09-05
        if CompareLStr(s34, 'DIETIME', Length('DIETIME')) then
        begin
          MapFlag.boDieTime := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.dwDieTime := StrToIntDef(s38, 0);
          Continue;
        end;

        // 禁止丢物品 piaoyun 2013-09-05
        if CompareText(s34, 'NOTHROWITEM') = 0 then
        begin
          MapFlag.boNOTHROWITEM := True;
          Continue;
        end;

        // 当前地图魔血石、气血石、幻魔石无效 piaoyun 2013-09-05
        if CompareText(s34, 'NOTSTONE') = 0 then
        begin
          MapFlag.boNotStone := True;
          Continue;
        end;

        // 当前地图宝宝不攻击人物 piaoyun 2013-09-05
        if CompareText(s34, 'SLAVENOTATTACKHUMAN') = 0 then
        begin
          MapFlag.boSlaveNotAttackHuman := True;
          Continue;
        end;

        // 当前地图宝宝不攻击英雄 piaoyun 2013-09-05
        if CompareText(s34, 'SLAVENOTATTACKHERO') = 0 then
        begin
          MapFlag.boSlaveNotAttackHero := True;
          Continue;
        end;

        /// ////////////////////////////////////////////////
        { TODO -ochongchong -c新增 : 副本地图 ---- 判断 【2013-09-06】 }
        // FB(40,祖玛副本,0,1) 创建40个祖玛副本地图
        // 第3个参数：0:限制队友必须有三职业; 1:不限制职业，队友都可进; 2:只允许自己进入; 3:允许行会进入
        // 第4个参数：副本创建1分钟后(未收回时)，允许延时进入副本时间(分)
        if CompareLStr(s34, 'FB(', Length('FB(')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          s38 := GetValidStr3_Ex(s38, s44, ',');
          nFBCount := StrToIntDef(s44, 0);
          s38 := GetValidStr3_Ex(s38, sFBName, ',');
          s38 := GetValidStr3_Ex(s38, s44, ',');
          s38 := GetValidStr3_Ex(s38, s4C, ',');
          IntTemp := StrToIntDef(s44, -1);
          if (IntTemp >= Integer(Low(TFBEnterLimit))) and (IntTemp <= Integer(High(TFBEnterLimit))) then
            EnterLimit := TFBEnterLimit(IntTemp)
          else
            EnterLimit := fbel_OnlyCreater;
          EnterDelayMin := StrToIntDef(s4C, 0);
          // 进入副本后多长时间没人清除副本 chongchong 2015-10-15
          NoHumClearFBMin := StrToIntDef(s38, 10);
          if NoHumClearFBMin <= 0 then
            NoHumClearFBMin := 10;
          if sFBName = '' then
          begin
            Result := -12;
            MainOutMessage(sMapName + ' 副本名称不能为空.');
            Exit;
          end;
          if not(nFBCount in [2 .. 99]) then
          begin
            Result := -13;
            MainOutMessage(sMapName + ' 副本数量为2~99.');
            Exit;
          end;
          if g_FBMapManager.IndexOf(sFBName) <> -1 then
          begin
            Result := -14;
            MainOutMessage(sMapName + ' 副本名称[' + sFBName + ']已经存在.');
            Exit;
          end;
          boFB := True;
        end;

        if CompareText(s34, 'SAFE') = 0 then
        begin
          MapFlag.boSAFE := True;
          Continue;
        end;

        if CompareText(s34, 'DARK') = 0 then
        begin
          MapFlag.boDARK := True;
          Continue;
        end;

        if CompareText(s34, 'FIGHT') = 0 then
        begin
          MapFlag.boFIGHT := True;
          Continue;
        end;

        if CompareText(s34, 'FIGHT2') = 0 then
        begin
          MapFlag.boFIGHT2 := True;
          Continue;
        end;

        if CompareLStr(s34, 'FIGHT3', Length('FIGHT3')) then // 行会战
        begin
          MapFlag.boFIGHT3 := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.btFIGHT3Flag := StrToIntDef(s38, 0);
          Continue;
        end;

        if CompareLStr(s34, 'INCGOLD', Length('INCGOLD')) then // 自动加金币
        begin
          MapFlag.boINCGOLD := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nINCGOLDPOINT := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nINCGOLDTIME := StrToIntDef(s38, -1);
          Continue;
        end;

        if CompareLStr(s34, 'DECGOLD', Length('DECGOLD')) then // 自动减金币
        begin
          MapFlag.boDECGOLD := True;

          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nDECGOLDPOINT := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nDECGOLDTIME := StrToIntDef(s38, -1);
          Continue;
        end;

        if CompareText(s34, 'FIGHT4') = 0 then
        begin
          MapFlag.boFIGHT4 := True;
          Continue;
        end;
        if CompareText(s34, 'DAY') = 0 then
        begin
          MapFlag.boDAY := True;
          Continue;
        end;
        if CompareText(s34, 'QUIZ') = 0 then
        begin
          MapFlag.boQUIZ := True;
          Continue;
        end;
        if CompareText(s34, 'WEATHER1') = 0 then
        begin
          MapFlag.boWeatherEffect1 := True;
          Continue;
        end;
        if CompareText(s34, 'WEATHER2') = 0 then
        begin
          MapFlag.boWeatherEffect2 := True;
          Continue;
        end;
        if CompareText(s34, 'WEATHER3') = 0 then
        begin
          MapFlag.boWeatherEffect3 := True;
          Continue;
        end;
        if CompareText(s34, 'ONKILLMON') = 0 then
        begin
          MapFlag.boOnKillMob := True;
          Continue;
        end;
        if CompareLStr(s34, 'NORECONNECT', Length('NORECONNECT')) then
        begin
          MapFlag.boNORECONNECT := True;
          ArrestStringEx(s34, '(', ')', sReConnectMap);
          MapFlag.sReConnectMap := sReConnectMap;
          if MapFlag.sReConnectMap = '' then
            Result := -11;
          Continue;
        end;
        if CompareLStr(s34, 'CHECKQUEST', Length('CHECKQUEST')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          if Length(s38) > 0 then
            QuestNPC := LoadMapQuest(s38);
          Continue;
        end;
        if CompareLStr(s34, 'NEEDSET_ON', Length('NEEDSET_ON')) then
        begin
          MapFlag.nNeedONOFF := 1;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nNEEDSETONFlag := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'NEEDSET_OFF', Length('NEEDSET_OFF')) then
        begin
          MapFlag.nNeedONOFF := 0;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nNEEDSETONFlag := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'MUSIC', Length('MUSIC')) then
        begin
          MapFlag.boMUSIC := True;
          s38 := Trim(CopyX(s28, '(', ')'));
          MapFlag.sMusicFileName := s38;
          Continue;
        end;
        if CompareLStr(s34, 'EXPRATE', Length('EXPRATE')) then
        begin
          MapFlag.boEXPRATE := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nEXPRATE := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'PKWINLEVEL', Length('PKWINLEVEL')) then
        begin
          MapFlag.boPKWINLEVEL := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nPKWINLEVEL := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'PKWINEXP', Length('PKWINEXP')) then
        begin
          MapFlag.boPKWINEXP := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nPKWINEXP := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'PKLOSTLEVEL', Length('PKLOSTLEVEL')) then
        begin
          MapFlag.boPKLOSTLEVEL := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nPKLOSTLEVEL := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'PKLOSTEXP', Length('PKLOSTEXP')) then
        begin
          MapFlag.boPKLOSTEXP := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nPKLOSTEXP := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'DECHP', Length('DECHP')) then
        begin
          MapFlag.boDECHP := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nDECHPPOINT := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nDECHPTIME := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'INCHP', Length('INCHP')) then
        begin
          MapFlag.boINCHP := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nINCHPPOINT := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nINCHPTIME := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'DECGAMEGOLD', Length('DECGAMEGOLD')) then
        begin
          MapFlag.boDECGAMEGOLD := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nDECGAMEGOLD := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nDECGAMEGOLDTIME := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'DECGAMEPOINT', Length('DECGAMEPOINT')) then
        begin
          MapFlag.boDECGAMEPOINT := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nDECGAMEPOINT := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nDECGAMEPOINTTIME := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'INCGAMEGOLD', Length('INCGAMEGOLD')) then
        begin
          MapFlag.boINCGAMEGOLD := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nINCGAMEGOLD := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nINCGAMEGOLDTIME := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'INCGAMEPOINT', Length('INCGAMEPOINT')) then
        begin
          MapFlag.boINCGAMEPOINT := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nINCGAMEPOINT := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), -1);
          MapFlag.nINCGAMEPOINTTIME := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'REVIVAL', Length('REVIVAL')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          // 修复时间参数有可能导致无限复活的严重BUG piaoyun 2013-09-05
          MapFlag.nRevivalCheckTime := StrToIntDef(GetValidStr3_Ex(s38, s38, '/'), 30) * 1000;
          // Min(StrToIntDef(GetValidStr3_Ex(s38, s38, ['/']), 30) * 1000, 30000);
          // 设置为255 则不允许复活 piaoyun 2013-09-05
          MapFlag.nRevivalMaxCount := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'RUNHUMAN', Length('RUNHUMAN')) then
        begin
          MapFlag.boRUNHUMAN := True;
          Continue;
        end;
        if CompareLStr(s34, 'RUNMON', Length('RUNMON')) then
        begin
          MapFlag.boRUNMON := True;
          Continue;
        end;
        if CompareLStr(s34, 'NORUNHUMAN', Length('NORUNHUMAN')) then
        begin
          MapFlag.boNoRunHuman := True;
          Continue;
        end;
        if CompareLStr(s34, 'NORUNMON', Length('NORUNMON')) then
        begin
          MapFlag.boNoRunMon := True;
          Continue;
        end;
        if CompareLStr(s34, 'NEEDHOLE', Length('NEEDHOLE')) then
        begin
          MapFlag.boNEEDHOLE := True;
          Continue;
        end;
        if CompareLStr(s34, 'NORECALL', Length('NORECALL')) then
        begin
          MapFlag.boNORECALL := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOGUILDRECALL', Length('NOGUILDRECALL')) then
        begin
          MapFlag.boNOGUILDRECALL := True;
          Continue;
        end;
        if CompareLStr(s34, 'NODEARRECALL', Length('NODEARRECALL')) then
        begin
          MapFlag.boNODEARRECALL := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOMASTERRECALL', Length('NOMASTERRECALL')) then
        begin
          MapFlag.boNOMASTERRECALL := True;
          Continue;
        end;
        if CompareLStr(s34, 'NORANDOMMOVE', Length('NORANDOMMOVE')) then
        begin
          MapFlag.boNORANDOMMOVE := True;
          Continue;
        end;
        if CompareLStr(s34, 'NODRUG', Length('NODRUG')) then
        begin
          MapFlag.boNODRUG := True;
          Continue;
        end;
        if CompareLStr(s34, 'MINE', Length('MINE')) then
        begin
          MapFlag.boMINE := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOPOSITIONMOVE', Length('NOPOSITIONMOVE')) then
        begin
          MapFlag.boNOPOSITIONMOVE := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOTALLOWUSEMAGIC', Length('NOTALLOWUSEMAGIC')) then
        begin // 增加不允许使用魔法
          MapFlag.boNOTALLOWUSEMAGIC := True;
          s38 := Trim(CopyX(s28, '(', ')'));
          MapFlag.sUnAllowMagicText := s38;
          Continue;
        end;
        if CompareLStr(s34, 'NOTALLOWUSEITEMS', Length('NOTALLOWUSEITEMS')) then
        begin // 增加不允许使用物品
          MapFlag.boUnAllowStdItems := True;
          s38 := Trim(CopyX(s28, '(', ')'));
          MapFlag.sUnAllowStdItemsText := s38;
          Continue;
        end;
        if CompareLStr(s34, 'NOALLOWUSEITEMS', Length('NOALLOWUSEITEMS')) then
        begin // 增加不允许使用物品
          MapFlag.boUnAllowStdItems := True;
          s38 := Trim(CopyX(s28, '(', ')'));
          MapFlag.sUnAllowStdItemsText := s38;
          Continue;
        end;
        if CompareLStr(s34, 'NORECALLHERO', Length('NORECALLHERO')) then
        begin
          MapFlag.boNORECALLHERO := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOCALLHERO', Length('NOCALLHERO')) then
        begin
          MapFlag.boNORECALLHERO := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOCALLPET', Length('NOCALLPET')) then
        begin
          MapFlag.boNoCallPet := True;
          Continue;
        end;
        if CompareLStr(s34, 'ALLOWUSEMYSHOP', Length('ALLOWUSEMYSHOP')) then
        begin
          MapFlag.boAllowUseMyshop := True;
          Continue;
        end;
        if CompareLStr(s34, 'DELDROPITEM', Length('DELDROPITEM')) then
        begin
          MapFlag.boDELDROPITEM := True;
          Continue;
        end;
        if CompareLStr(s34, 'SAYLEVEL', Length('SAYLEVEL')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.nSAYLEVEL := StrToIntDef(s38, -1);
          Continue;
        end;
        if CompareLStr(s34, 'NODROPUSEITEMS', Length('NODROPUSEITEMS')) then
        begin
          MapFlag.boNODROPUSEITEMS := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOSAFEPOSITIONMOVE', Length('NOSAFEPOSITIONMOVE')) then
        begin
          MapFlag.boNOSAFEPOSITIONMOVE := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOCHALLENGE', Length('NOCHALLENGE')) then
        begin
          MapFlag.boNoChallenge := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOMANNOMON', Length('NOMANNOMON')) then
        begin
          MapFlag.boNoManNoMon := True;
          Continue;
        end;
        if CompareLStr(s34, 'NIGHT', Length('NIGHT')) then
        begin
          MapFlag.boNight := True;
          Continue;
        end;
        if CompareLStr(s34, 'HITMON', Length('HITMON')) then
        begin
          MapFlag.boHITMON := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.sHITMON := Trim(s38);
          Continue;
        end;
        if CompareLStr(s34, 'NOHORSE', Length('NOHORSE')) then
        begin
          MapFlag.boNOHORSE := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOAUTOONLINE', Length('NOAUTOONLINE')) then
        begin
          MapFlag.boNoAutoOnline := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOSWITCHATTACKMODE', Length('NOSWITCHATTACKMODE')) then
        begin
          MapFlag.boNoSwitchAttackMode := True;
          Continue;
        end;
        if CompareLStr(s34, 'NODEAL', Length('NODEAL')) then
        begin
          MapFlag.boNoDeal := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOSHOP', Length('NOSHOP')) then
        begin
          MapFlag.boNoShop := True;
          Continue;
        end;
        if CompareLStr(s34, 'NOHEROPROTECT', Length('NOHEROPROTECT')) then
        begin
          MapFlag.boNoHeroProtect := True;
          Continue;
        end;
        if CompareLStr(s34, 'DROPITEMADDUSERBAG', Length('DROPITEMADDUSERBAG')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.sDropItemAddUserBag := Trim(s38);
          Continue;
        end;

        if CompareLStr(s34, 'SECRET', Length('SECRET')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          s38 := GetValidStr3_Ex(s38, sSecretFlag, '|');
          s38 := GetValidStr3_Ex(s38, sSecretShowName, '|');
          s38 := GetValidStr3_Ex(s38, sSecretDressShap, '|');
          s38 := GetValidStr3_Ex(s38, sSecretWeaponShap, '|');

          MapFlag.SecretFlag := StrToIntDef(sSecretFlag, 0);
          MapFlag.SecretShowName := sSecretShowName;
          MapFlag.SecretDressShap := StrToIntDef(sSecretDressShap, 0);
          MapFlag.SecretWeaponShap := StrToIntDef(sSecretWeaponShap, 0);
          Continue;
        end;

        if (s34[1] = 'L') then
          MapFlag.nL := StrToIntDef(Copy(s34, 2, Length(s34) - 1), 1);

        if CompareLStr(s34, 'NEEDLEVELTIME', Length('NEEDLEVELTIME')) then
        begin
          MapFlag.boNeedLevelTime := True;
          ArrestStringEx(s34, '(', ')', s38);
          MapFlag.dwNeedLevelPoint := Min(StrToInt64Def(s38, 0), High(LongWord)); // 进地图最低等级
          Continue;
        end;

        if CompareLStr(s34, 'NOAUCTION', Length('NOAUCTION')) then
        begin
          MapFlag.boNoAuction := True;
          Continue;
        end;

        // 禁止物品自动入包
        if CompareLStr(s34, 'NoAutoDropItemToBag', Length('NoAutoDropItemToBag')) then
        begin
          MapFlag.boNoAutoDropItemToBag := True;
          Continue;
        end;

        // 禁止范围拾取
        if CompareLStr(s34, 'NoAutoRangePickItem', Length('NoAutoRangePickItem')) then
        begin
          MapFlag.boNoAutoRangePickItem := True;
          Continue;
        end;

        if CompareLStr(s34, 'TimeMap', Length('TimeMap')) then
        begin
          ArrestStringEx(s34, '(', ')', s38);
          s38 := GetValidStr3_Ex(s38, sTimeMapCode, '|');
          s38 := GetValidStr3_Ex(s38, sTimeMapMin, '|');
          s38 := GetValidStr3_Ex(s38, sTimeMapShowLeft, '|');
          s38 := GetValidStr3_Ex(s38, sTimeMapLabel, '|');
          MapFlag.TimeMapCode := sTimeMapCode;
          MapFlag.TimeMapMin := StrToIntDef(sTimeMapMin, 0);
          MapFlag.TimeMapShowLeft := StrToIntDef(sTimeMapShowLeft, 0) <> 0;
          MapFlag.TimeMapLabel := sTimeMapLabel;
          Continue;
        end;
      end;

      { -ochongchong -c新增 : 副本地图 ---- 载入地图信息 【2013-09-06】 }
      if boFB then
      begin
        if sMainMapName = '' then
          sMainMapName := sMapName;

        FBList := TList.Create;
        g_FBMapManager.AddObject(sFBName, TObject(FBList));
        for k := 1 to nFBCount do
        begin
          sMapName := '$FB_' + sMainMapName + '_' + IntToStr(k);
          Envir := g_MapManager.AddMapInfo(sMapName, sMainMapName, sMapDesc, nServerIndex, @MapFlag, QuestNPC);
          if Envir <> nil then
          begin
            Envir.m_boFB := True;
            Envir.m_sFBName := sFBName;
            Envir.m_FBEnterLimit := EnterLimit;
            Envir.m_dwFBEnterDelayMin := EnterDelayMin * 60000;
            Envir.m_dwFBNoHumClearMin := NoHumClearFBMin * 1000;
            FBList.Add(Envir);
          end
          else
            MainOutMessage('副本地图创建失败，地图名已经存在：' + sMapName);
        end;
      end
      else
        g_MapManager.AddMapInfo(sMapName, sMainMapName, sMapDesc, nServerIndex, @MapFlag, QuestNPC);
    end;

    FrmMain.Caption := sCaption + ' [正在初始化地图信息(' + IntToStr(LoadList.Count) + '/' + IntToStr(I + 1) + ')]';
    Application.ProcessMessages;
  end;
  FrmMain.Caption := sCaption;

  // 加载地图连接点
  for I := 0 to LoadList.Count - 1 do
  begin
    s30 := Trim(LoadList.Strings[I]);
    if (s30 <> '') and (s30[1] <> '[') and (s30[1] <> ';') then
    begin
      s30 := GetValidStr3(s30, s34, [' ', ',', #9]);
      sMapName := s34;
      s30 := GetValidStr3(s30, s34, [' ', ',', #9]);
      n14 := StrToIntDef(s34, 0);
      s30 := GetValidStr3(s30, s34, [' ', ',', #9]);
      n18 := StrToIntDef(s34, 0);
      s30 := GetValidStr3(s30, s34, [' ', ',', '-', '>', #9]);
      s44 := s34;
      s30 := GetValidStr3(s30, s34, [' ', ',', #9]);
      n1C := StrToIntDef(s34, 0);
      s30 := GetValidStr3(s30, s34, [' ', ',', ';', #9]);
      n20 := StrToIntDef(s34, 0);
      g_MapManager.AddMapRoute(sMapName, n14, n18, s44, n1C, n20);
    end;
  end;
  LoadList.Free;
end;

procedure TFrmDB.QFunctionNPC;
var
  sScriptFile: string;
  sScritpDir: string;
  sShowFile: string;
  I: Integer;
  Envir: TEnvirnoment;
begin
  try
    sScriptFile := g_Config.sEnvirDir + sMarket_Def + 'QFunction-0.txt';
    sShowFile := ReplaceChar(sScriptFile, '\', '/');
    sScritpDir := g_Config.sEnvirDir + sMarket_Def;
    if not DirectoryExists(sScritpDir) then
      MkDir(PChar(sScritpDir));

    if g_FunctionNPC = nil then
    begin
      g_FunctionNPC := TMerchant.Create;
      UserEngine.AddMerchant(g_FunctionNPC);
    end;

    g_FunctionNPC.m_NoUserSelectList.Clear;
    g_FunctionNPC.m_sMapName := '0';
    g_FunctionNPC.m_PEnvir := g_MapManager.FindMap(g_FunctionNPC.m_sMapName);
    g_FunctionNPC.m_nCurrX := 0;
    g_FunctionNPC.m_nCurrY := 0;
    g_FunctionNPC.m_sCharName := 'QFunction';
    g_FunctionNPC.m_nFlag := 0;
    g_FunctionNPC.m_wAppr := 0;
    g_FunctionNPC.m_sFilePath := sMarket_Def;
    g_FunctionNPC.m_sScript := 'QFunction';
    g_FunctionNPC.m_boIsHide := True;
    g_FunctionNPC.m_boIsQuest := False;
    g_FunctionNPC.m_NoUserSelectList.Add('@ActionLogClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@ActiveTitle_');
    g_FunctionNPC.m_NoUserSelectList.Add('@Attack');
    g_FunctionNPC.m_NoUserSelectList.Add('@BlastAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@FatalAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@BagUseStoneItemOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@ButchCloneItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@ButchItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@ButtonClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@ArrButtonClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@ClientBuffClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@ArrBuffClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@BuyShopItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@Challenge');
    g_FunctionNPC.m_NoUserSelectList.Add('@Challenge_Win');
    g_FunctionNPC.m_NoUserSelectList.Add('@ChangeHeroNameFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@ChangeHeroNameOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@ChangeHumNameFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@ChangeHumNameOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@ChangeingHeroName');
    g_FunctionNPC.m_NoUserSelectList.Add('@ChangeingHumName');
    g_FunctionNPC.m_NoUserSelectList.Add('@CloseClientBuff');
    g_FunctionNPC.m_NoUserSelectList.Add('@CreateGuild');
    g_FunctionNPC.m_NoUserSelectList.Add('@CreateHeroFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@CreateHeroFailEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@CreateHeroOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@DeleteHeroFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@DeleteHeroOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@DownHorse');
    g_FunctionNPC.m_NoUserSelectList.Add('@DropInsuranceItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@DropItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@DropItems');
    g_FunctionNPC.m_NoUserSelectList.Add('@DummyStart');
    g_FunctionNPC.m_NoUserSelectList.Add('@DummyStop');
    g_FunctionNPC.m_NoUserSelectList.Add('@ExitGuildBefore');
    g_FunctionNPC.m_NoUserSelectList.Add('@ExitGuild');
    g_FunctionNPC.m_NoUserSelectList.Add('@FindPathBegin');
    g_FunctionNPC.m_NoUserSelectList.Add('@FindPathEnd');
    g_FunctionNPC.m_NoUserSelectList.Add('@FindPathStop');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetLevelUp');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetRecall');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetRetake');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetAddMagic');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetBoxItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetBoxsItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetCastle');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetCastleEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetExp');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetHeroBak');
    g_FunctionNPC.m_NoUserSelectList.Add('@GetHeroOk');
    g_FunctionNPC.m_NoUserSelectList.Add('@GodBlessItemClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@GodBlessUpgradeClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupAddMember');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupClose');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupCreate');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupDelMember');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupItemOff');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupItemOn');
    g_FunctionNPC.m_NoUserSelectList.Add('@GroupKillMon');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.PickUpItems');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.PickUpItemEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.Revival');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.NpcRevival');
    g_FunctionNPC.m_NoUserSelectList.Add('@Help');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroActiveTitle_');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroDie');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroDropInsuranceItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroGetExp');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroGetNGExp');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroGodBlessItemClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroGodBlessUpgradeClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroGroupItemOff');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroGroupItemOn');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroKillMon');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroKillMonGetExp');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroKillPlay');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroLevelUp');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroMagicStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroNameExists');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroNameFilter');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroNGLevelUp');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroNotShowFashion');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroOpenSndacasket');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroOverChrCount');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroReNewLevel');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroShowFashion');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroSkillLevelEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroTakeOff');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroTakeOn');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroUnactiveTitle_');
    g_FunctionNPC.m_NoUserSelectList.Add('@HumNameExists');
    g_FunctionNPC.m_NoUserSelectList.Add('@HumNameFilter');
    g_FunctionNPC.m_NoUserSelectList.Add('@ItemDamage');
    g_FunctionNPC.m_NoUserSelectList.Add('@ItemIntoBox');
    g_FunctionNPC.m_NoUserSelectList.Add('@ItemUpgrade');
    g_FunctionNPC.m_NoUserSelectList.Add('@KillMissionMob');
    g_FunctionNPC.m_NoUserSelectList.Add('@KillMon');
    g_FunctionNPC.m_NoUserSelectList.Add('@KillMonGetExp');
    g_FunctionNPC.m_NoUserSelectList.Add('@KillMonster');
    g_FunctionNPC.m_NoUserSelectList.Add('@KillPlay');
    g_FunctionNPC.m_NoUserSelectList.Add('@KillSlave');
    g_FunctionNPC.m_NoUserSelectList.Add('@LeaveGroup');
    g_FunctionNPC.m_NoUserSelectList.Add('@LostCastle');
    g_FunctionNPC.m_NoUserSelectList.Add('@LostCastleEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@Magic');
    g_FunctionNPC.m_NoUserSelectList.Add('@MagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@BlastMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@FatalMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@MagicStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@MagMonFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@MagSelfFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@MagTagFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@MagTagFuncEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@MasterOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@Member');
    g_FunctionNPC.m_NoUserSelectList.Add('@NameLengthFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@NoButchItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@NotHaveHero');
    g_FunctionNPC.m_NoUserSelectList.Add('@NotShowFashion');
    g_FunctionNPC.m_NoUserSelectList.Add('@NumberButtonClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@OnKillMob');
    g_FunctionNPC.m_NoUserSelectList.Add('@OpenSndacasket');
    g_FunctionNPC.m_NoUserSelectList.Add('@PickUpItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@PickUpItemEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@PickUpItems');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.PickUpItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.PickUpItemEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@H.PickUpItems');
    // g_FunctionNPC.m_NoUserSelectList.Add('@GamePetPickUpItem');
    // g_FunctionNPC.m_NoUserSelectList.Add('@GamePetPickUpItemEx');
    // g_FunctionNPC.m_NoUserSelectList.Add('@GamePetPickUpItems');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayDie');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayLevelUp');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayNGLevelUp');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayOffLine');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayReconnection');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayReNewLevel');
    g_FunctionNPC.m_NoUserSelectList.Add('@QueryHeroInfoFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@QueryMyShopFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@RemoveStoneItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@Revival');
    g_FunctionNPC.m_NoUserSelectList.Add('@NpcRevival');
    g_FunctionNPC.m_NoUserSelectList.Add('@Run');
    g_FunctionNPC.m_NoUserSelectList.Add('@ShopStall');
    g_FunctionNPC.m_NoUserSelectList.Add('@ShowFashion');
    g_FunctionNPC.m_NoUserSelectList.Add('@SkillLevelEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@StartAutoOnline');
    g_FunctionNPC.m_NoUserSelectList.Add('@StartGroup');
    g_FunctionNPC.m_NoUserSelectList.Add('@StartMyShop');
    g_FunctionNPC.m_NoUserSelectList.Add('@StdModeFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@StdModeFuncEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroStdModeFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@StopAutoOnline');
    g_FunctionNPC.m_NoUserSelectList.Add('@StopMyShop');
    g_FunctionNPC.m_NoUserSelectList.Add('@Struck');
    g_FunctionNPC.m_NoUserSelectList.Add('@TakeOff');
    g_FunctionNPC.m_NoUserSelectList.Add('@TakeOn');
    g_FunctionNPC.m_NoUserSelectList.Add('@UnactiveTitle_');
    g_FunctionNPC.m_NoUserSelectList.Add('@UnMasterEnd');
    g_FunctionNPC.m_NoUserSelectList.Add('@UnMasterEnd1');
    g_FunctionNPC.m_NoUserSelectList.Add('@ForceUnMasterEnd');
    g_FunctionNPC.m_NoUserSelectList.Add('@ForceUnMasterEnd1');
    g_FunctionNPC.m_NoUserSelectList.Add('@UpgradeClear');
    g_FunctionNPC.m_NoUserSelectList.Add('@UpgradeFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@UpgradeOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@UpHorse');
    g_FunctionNPC.m_NoUserSelectList.Add('@UsePlugin');
    g_FunctionNPC.m_NoUserSelectList.Add('@UserCmd');
    g_FunctionNPC.m_NoUserSelectList.Add('@VerifyFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@Walk');
    g_FunctionNPC.m_NoUserSelectList.Add('@OnTimer');
    g_FunctionNPC.m_NoUserSelectList.Add('@JoinGuild');
    // modify chongchong 2013-07-24
    g_FunctionNPC.m_NoUserSelectList.Add('@UseItemName_Fail');
    g_FunctionNPC.m_NoUserSelectList.Add('@UseItemName_OK');
    g_FunctionNPC.m_NoUserSelectList.Add('@穴位已通');
    g_FunctionNPC.m_NoUserSelectList.Add('@英雄穴位已通');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileBind');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeSendFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeInterval');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeResendMaxCount');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeOK');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeSingleFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeFail');
    g_FunctionNPC.m_NoUserSelectList.Add('@MobileVerifyCodeTimeOut');
    g_FunctionNPC.m_NoUserSelectList.Add('@OpenStoratge2');
    g_FunctionNPC.m_NoUserSelectList.Add('@OpenStoratge3');
    g_FunctionNPC.m_NoUserSelectList.Add('@OpenStoratge4');
    g_FunctionNPC.m_NoUserSelectList.Add('@SlaveMagicStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@SlaveMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@SlaveStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@SlaveAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@SlaveAttackDamage');
    g_FunctionNPC.m_NoUserSelectList.Add('@SlaveStruckDamage');
    g_FunctionNPC.m_NoUserSelectList.Add('@CloneMagicStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@CloneMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@CloneStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@CloneAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@LearnMagic');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroLearnMagic');
    g_FunctionNPC.m_NoUserSelectList.Add('@CustomButtonClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@DlgButtonClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@ClosedBagItemClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@MinMapCustomButtonClick');
    g_FunctionNPC.m_NoUserSelectList.Add('@OnSlaveDie');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroSlaveMagicStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroSlaveStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroSlaveMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroSlaveAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@OnHeroSlaveDie');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroMagSelfFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroMagTagFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroMagTagFuncEx');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroMagMonFunc');
    g_FunctionNPC.m_NoUserSelectList.Add('@EnterMap');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroEnterMap');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeforePlayerSelling');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayerSelling');
    g_FunctionNPC.m_NoUserSelectList.Add('@PlayerSold');
    g_FunctionNPC.m_NoUserSelectList.Add('@RungateMsgFilter');
    g_FunctionNPC.m_NoUserSelectList.Add('@CheckSpeed');
    g_FunctionNPC.m_NoUserSelectList.Add('@CheckPlugin');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetMagicAttack');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetMagicStruck');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetAttackDamage');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetStruckDamage');
    g_FunctionNPC.m_NoUserSelectList.Add('@GamePetKillMon');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeginTakeOff');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroBeginTakeOff');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeginTakeOn');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroBeginTakeOn');
    g_FunctionNPC.m_NoUserSelectList.Add('@AttatckModeChange');
    g_FunctionNPC.m_NoUserSelectList.Add('@AddBag');
    g_FunctionNPC.m_NoUserSelectList.Add('@HumDropItem');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeginUseBead');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeginStorageSave');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeginStorageTake');
    g_FunctionNPC.m_NoUserSelectList.Add('@BeginAuctionBuy');
    g_FunctionNPC.m_NoUserSelectList.Add('@AuctionItemTakeBack');
    g_FunctionNPC.m_NoUserSelectList.Add('@HeroBeginUseBead');

    // 把触发字段加入禁止用户点击列表
    if (g_MapManager <> nil) then
    begin
      for I := 0 to g_MapManager.Count - 1 do
      begin
        Envir := TEnvirnoment(g_MapManager.Items[I]);
        if Envir.m_boHITMON and (Envir.m_sHITMON <> '') then
          g_FunctionNPC.AddSelectLable(Envir.m_sHITMON);
      end;
    end;
  except
  end;
end;

procedure TFrmDB.QMissionNPC; // 任务NPC
var
  sScriptFile: string;
  sScritpDir: string;
  sShowFile: string;
begin
  try
    sScriptFile := g_Config.sEnvirDir + sMarket_Def + 'QMission-0.txt';
    sShowFile := ReplaceChar(sScriptFile, '\', '/');
    sScritpDir := g_Config.sEnvirDir + sMarket_Def;
    if not DirectoryExists(sScritpDir) then
      MkDir(PChar(sScritpDir));
    { if not FileExists(sScriptFile) then begin
      SaveList := TStringList.Create;
      SaveList.Add(';此脚为任务脚本，人物每次登录时都会执行[@Login]');
      SaveList.Add('[@Login]');
      SaveList.Add('#if');
      SaveList.Add('#say');
      SaveList.Add('任务脚本运行成功，欢迎进入本游戏！\ \');
      SaveList.Add('<关闭/@exit> \ \');
      SaveList.Add('');
      try
      SaveList.SaveToFile(sScriptFile);
      except
      end;
      SaveList.Free;
      end; }
    // if FileExists(sScriptFile) then begin
    if g_MissionNPC = nil then
    begin
      g_MissionNPC := TMerchant.Create;
      UserEngine.AddMerchant(g_MissionNPC);
    end;
    g_MissionNPC.m_NoUserSelectList.Clear;
    g_MissionNPC.m_sMapName := '0';
    g_MissionNPC.m_PEnvir := g_MapManager.FindMap(g_MissionNPC.m_sMapName);
    g_MissionNPC.m_nCurrX := 0;
    g_MissionNPC.m_nCurrY := 0;
    g_MissionNPC.m_sCharName := 'QMission';
    g_MissionNPC.m_nFlag := 0;
    g_MissionNPC.m_wAppr := 0;
    g_MissionNPC.m_sFilePath := sMarket_Def;
    g_MissionNPC.m_sScript := 'QMission';
    g_MissionNPC.m_boIsHide := True;
    g_MissionNPC.m_boIsQuest := False;
    g_MissionNPC.m_NoUserSelectList.Add('@Login');
  except
  end;
end;

{ TODO -opiaoyun -c新增 : 读取连击经络脚本 【2013-08-16】 }
procedure TFrmDB.QBatterNPC;
var
  sScriptFile: string;
  sScritpDir: string;
  SaveList: TStringList;
  sShowFile: string;
begin
  try
    sScriptFile := g_Config.sEnvirDir + sMarket_Def + 'QBatter-0.txt';
    sShowFile := ReplaceChar(sScriptFile, '\', '/');
    sScritpDir := g_Config.sEnvirDir + sMarket_Def;
    if not DirectoryExists(sScritpDir) then
      MkDir(PChar(sScritpDir));
    if not FileExists(sScriptFile) then
    begin
      SaveList := TStringList.Create;
      SaveList.Add(';此脚本为连击功能脚本');
      SaveList.SaveToFile(sScriptFile);
      SaveList.Free;
    end;
    if FileExists(sScriptFile) then
    begin
      g_BatterNPC := TMerchant.Create;
      g_BatterNPC.m_sMapName := '0';
      g_BatterNPC.m_nCurrX := 0;
      g_BatterNPC.m_nCurrY := 0;
      g_BatterNPC.m_sCharName := 'QBatter';
      g_BatterNPC.m_nFlag := 0;
      g_BatterNPC.m_wAppr := 0;
      g_BatterNPC.m_sFilePath := sMarket_Def;
      g_BatterNPC.m_sScript := 'QBatter';
      g_BatterNPC.m_boIsHide := True;
      g_BatterNPC.m_boIsQuest := False;
      UserEngine.AddMerchant(g_BatterNPC);
    end
    else
      g_BatterNPC := nil;
  except
    g_BatterNPC := nil;
  end;
end;

procedure TFrmDB.QMangeNPC();
var
  sScriptFile: string;
  sScritpDir: string;
  sShowFile: string;
begin
  try
    sScriptFile := g_Config.sEnvirDir + 'MapQuest_def\' + 'QManage.txt';
    sShowFile := ReplaceChar(sScriptFile, '\', '/');
    sScritpDir := g_Config.sEnvirDir + 'MapQuest_def\';
    if not DirectoryExists(sScritpDir) then
      MkDir(PChar(sScritpDir));
    if g_ManageNPC = nil then
    begin
      g_ManageNPC := TMerchant.Create;
      UserEngine.AddNpc('QManage', g_ManageNPC);
    end;
    g_ManageNPC.m_NoUserSelectList.Clear;
    g_ManageNPC.m_sMapName := '0';
    g_ManageNPC.m_PEnvir := g_MapManager.FindMap(g_ManageNPC.m_sMapName);
    g_ManageNPC.m_nCurrX := 0;
    g_ManageNPC.m_nCurrY := 0;
    g_ManageNPC.m_sCharName := 'QManage';
    g_ManageNPC.m_sScript := 'QManage';
    g_ManageNPC.m_nFlag := 0;
    g_ManageNPC.m_wAppr := 0;
    g_ManageNPC.m_sFilePath := 'MapQuest_def\';
    g_ManageNPC.m_boIsHide := True;
    g_ManageNPC.m_boIsQuest := False;
    g_ManageNPC.m_NoUserSelectList.Add('@Login');
    g_ManageNPC.m_NoUserSelectList.Add('@HeroLogin');
    g_ManageNPC.m_NoUserSelectList.Add('@HeroLogout');
    g_ManageNPC.m_NoUserSelectList.Add('@OnTimer');
  except
  end;
end;

procedure TFrmDB.RobotNPC();
var
  sScriptFile: string;
  sScritpDir: string;
begin
  try
    sScriptFile := g_Config.sEnvirDir + 'Robot_def\' + 'RobotManage.txt';
    sScritpDir := g_Config.sEnvirDir + 'Robot_def\';
    if not DirectoryExists(sScritpDir) then
      MkDir(PChar(sScritpDir));
    { if not FileExists(sScriptFile) then begin
      tSaveList := TStringList.Create;
      tSaveList.Add(';此脚为机器人专用脚本，用于机器人处理功能用的脚本。');
      try
      tSaveList.SaveToFile(sScriptFile);
      except
      end;
      tSaveList.Free;
      end;
      if FileExists(sScriptFile) then begin }
    if g_RobotNPC = nil then
    begin
      g_RobotNPC := TMerchant.Create;
      UserEngine.AddNpc('RobotManage', g_RobotNPC);
    end;
    g_RobotNPC.m_sMapName := '0';
    g_RobotNPC.m_PEnvir := g_MapManager.FindMap(g_RobotNPC.m_sMapName);
    g_RobotNPC.m_nCurrX := 0;
    g_RobotNPC.m_nCurrY := 0;
    g_RobotNPC.m_sCharName := 'RobotManage';
    g_RobotNPC.m_sScript := 'RobotManage';
    g_RobotNPC.m_nFlag := 0;
    g_RobotNPC.m_wAppr := 0;
    g_RobotNPC.m_sFilePath := 'Robot_def\';
    g_RobotNPC.m_boIsHide := True;
    g_RobotNPC.m_boIsQuest := False;
  except
  end;
end;

{ TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-17】 }
function TFrmDB.LoadMapEvent(): Integer;
var
  sFileName, tStr: string;
  tMapEventList: TStringList;
  I: Integer;
  sMapName, sX, sY, s24, s28, s2C, s30, s34, s36, s38, s40, s42, s44, s46, sRange: string;
  MapEvent: pTMapEvent;
  Map: TEnvirnoment;
  nEventType: Integer;
  k, Index: Integer;
  FBList: TList;
  // sFBMapName: string;
begin
  Result := 1;
  sFileName := g_Config.sEnvirDir + 'MapEvent.txt';
  for I := 0 to g_MapEventListOfDropItem.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfDropItem.Items[I]));
  end;
  g_MapEventListOfDropItem.Clear;
  for I := 0 to g_MapEventListOfPickUpItem.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfPickUpItem.Items[I]));
  end;
  g_MapEventListOfPickUpItem.Clear;
  for I := 0 to g_MapEventListOfMine.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfMine.Items[I]));
  end;
  g_MapEventListOfMine.Clear;
  for I := 0 to g_MapEventListOfDoMine.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfDoMine.Items[I]));
  end;
  g_MapEventListOfDoMine.Clear;
  for I := 0 to g_MapEventListOfWalk.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfWalk.Items[I]));
  end;
  g_MapEventListOfWalk.Clear;
  for I := 0 to g_MapEventListOfRun.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfRun.Items[I]));
  end;
  g_MapEventListOfRun.Clear;
  for I := 0 to g_MapEventListOfScatterItem.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfScatterItem.Items[I]));
  end;
  g_MapEventListOfScatterItem.Clear;
  for I := 0 to g_MapEventListOfHorseWalk.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfHorseWalk.Items[I]));
  end;
  g_MapEventListOfHorseWalk.Clear;
  for I := 0 to g_MapEventListOfHorseRun.Count - 1 do
  begin
    DisPose(pTMapEvent(g_MapEventListOfHorseRun.Items[I]));
  end;
  g_MapEventListOfHorseRun.Clear;
  if FileExists(sFileName) then
  begin
    tMapEventList := TStringList.Create;
    try
      tMapEventList.LoadFromFile(sFileName);
      for I := 0 to tMapEventList.Count - 1 do
      begin
        tStr := tMapEventList.Strings[I];
        if (tStr <> '') and (tStr[1] <> ';') then
        begin
          tStr := GetValidStr3(tStr, sMapName, [' ', #9]);
          tStr := GetValidStr3(tStr, sX, [' ', #9]);
          tStr := GetValidStr3(tStr, sY, [' ', #9]);
          tStr := GetValidStr3(tStr, sRange, [' ', #9]);
          tStr := GetValidStr3(tStr, s24, [' ', #9]);
          tStr := GetValidStr3(tStr, s28, [' ', #9]);
          tStr := GetValidStr3(tStr, s2C, [' ', #9]);
          tStr := GetValidStr3(tStr, s30, [' ', #9]);
          if (sMapName <> '') and (sX <> '') and (sY <> '') and (s30 <> '') then
          begin
            { TODO -ochongchong -c新增 : 副本地图 ---- 载入副本地图事件信息 【2013-09-06】 }
            if sMapName[1] = '$' then
            begin
              sMapName := Copy(sMapName, 2, MaxInt);
              Index := g_FBMapManager.IndexOf(sMapName);
              if Index <> -1 then
              begin
                FBList := TList(g_FBMapManager.Objects[Index]);
                for k := 0 to FBList.Count - 1 do
                begin
                  // sFBMapName := TEnvirnoment(FBList[K]).sMapName;
                  New(MapEvent);
                  FillChar(MapEvent.MapFlag, SizeOf(TQuestUnitStatus), 0);
                  FillChar(MapEvent.Condition, SizeOf(TMapCondition), #0);
                  FillChar(MapEvent.StartScript, SizeOf(TStartScript), #0);
                  MapEvent.sMapName := sMapName; // sFBMapName
                  MapEvent.nCurrX := StrToIntDef(sX, 0);
                  MapEvent.nCurrY := StrToIntDef(sY, 0);
                  MapEvent.nRange := StrToIntDef(sRange, 0);
                  s24 := GetValidStr3(s24, s34, [':', #9]);
                  s24 := GetValidStr3(s24, s36, [':', #9]);
                  MapEvent.MapFlag.nQuestUnit := StrToIntDef(s34, -1);
                  MapEvent.MapFlag.boValue := StrToIntDef(s36, 0) <> 0;
                  s28 := GetValidStr3(s28, s38, [':', #9]);
                  s28 := GetValidStr3(s28, s40, [':', #9]);
                  s28 := GetValidStr3(s28, s42, [':', #9]);
                  nEventType := StrToIntDef(s38, 0);
                  MapEvent.Condition.sItemName := Trim(s40);
                  MapEvent.Condition.boNeedGroup := StrToIntDef(s42, 0) <> 0;
                  if MapEvent.Condition.sItemName = '' then
                    MapEvent.Condition.sItemName := '*';
                  MapEvent.nRandomValue := _MAX(StrToIntDef(s2C, 0), 0);
                  s30 := GetValidStr3(s30, s44, [':', #9]);
                  s30 := GetValidStr3(s30, s46, [':', #9]);
                  MapEvent.StartScript.nLable := StrToIntDef(s44, 0);
                  MapEvent.StartScript.sLable := Trim(s46);
                  if nEventType in [1 .. 9] then
                  begin
                    MapEvent.Event := TMapNotifyEvent(nEventType);
                    case nEventType of
                      1:
                        g_MapEventListOfDropItem.Add(MapEvent);
                      2:
                        g_MapEventListOfPickUpItem.Add(MapEvent);
                      3:
                        g_MapEventListOfMine.Add(MapEvent);
                      4:
                        begin
                          MapEvent.Condition.sItemName := '*';
                          g_MapEventListOfWalk.Add(MapEvent);
                        end;
                      5:
                        begin
                          MapEvent.Condition.sItemName := '*';
                          g_MapEventListOfRun.Add(MapEvent);
                        end;
                      6:
                        g_MapEventListOfScatterItem.Add(MapEvent);
                      7:
                        g_MapEventListOfHorseWalk.Add(MapEvent);
                      8:
                        g_MapEventListOfHorseRun.Add(MapEvent);
                      9:
                        g_MapEventListOfDoMine.Add(MapEvent);
                    end;
                  end
                  else
                    DisPose(MapEvent);
                end;
              end;
            end
            else
            begin
              Map := g_MapManager.FindMap(sMapName);
              if (Map <> nil) or (sMapName = '*') then
              begin
                New(MapEvent);
                FillChar(MapEvent.MapFlag, SizeOf(TQuestUnitStatus), 0);
                FillChar(MapEvent.Condition, SizeOf(TMapCondition), #0);
                FillChar(MapEvent.StartScript, SizeOf(TStartScript), #0);
                MapEvent.sMapName := Trim(sMapName);
                MapEvent.nCurrX := StrToIntDef(sX, 0);
                MapEvent.nCurrY := StrToIntDef(sY, 0);
                MapEvent.nRange := StrToIntDef(sRange, 0);
                s24 := GetValidStr3(s24, s34, [':', #9]);
                s24 := GetValidStr3(s24, s36, [':', #9]);
                MapEvent.MapFlag.nQuestUnit := StrToIntDef(s34, -1);
                MapEvent.MapFlag.boValue := StrToIntDef(s36, 0) <> 0;
                s28 := GetValidStr3(s28, s38, [':', #9]);
                s28 := GetValidStr3(s28, s40, [':', #9]);
                s28 := GetValidStr3(s28, s42, [':', #9]);
                nEventType := StrToIntDef(s38, 0);
                MapEvent.Condition.sItemName := Trim(s40);
                MapEvent.Condition.boNeedGroup := StrToIntDef(s42, 0) <> 0;
                if MapEvent.Condition.sItemName = '' then
                  MapEvent.Condition.sItemName := '*';
                MapEvent.nRandomValue := _MAX(StrToIntDef(s2C, 0), 0);
                s30 := GetValidStr3(s30, s44, [':', #9]);
                s30 := GetValidStr3(s30, s46, [':', #9]);
                MapEvent.StartScript.nLable := StrToIntDef(s44, 0);
                MapEvent.StartScript.sLable := Trim(s46);
                if nEventType in [1 .. 9] then
                begin
                  MapEvent.Event := TMapNotifyEvent(nEventType);
                  case nEventType of
                    1:
                      g_MapEventListOfDropItem.Add(MapEvent);
                    2:
                      g_MapEventListOfPickUpItem.Add(MapEvent);
                    3:
                      g_MapEventListOfMine.Add(MapEvent);
                    4:
                      begin
                        MapEvent.Condition.sItemName := '*';
                        g_MapEventListOfWalk.Add(MapEvent);
                      end;
                    5:
                      begin
                        MapEvent.Condition.sItemName := '*';
                        g_MapEventListOfRun.Add(MapEvent);
                      end;
                    6:
                      g_MapEventListOfScatterItem.Add(MapEvent);
                    7:
                      g_MapEventListOfHorseWalk.Add(MapEvent);
                    8:
                      g_MapEventListOfHorseRun.Add(MapEvent);
                    9:
                      g_MapEventListOfDoMine.Add(MapEvent);
                  end;
                end
                else
                  DisPose(MapEvent);
              end;
            end;
          end
          else
            Result := -I;
        end;
      end;
    finally
      tMapEventList.Free;
    end;
  end;

  // 把触发字段加入禁止用户点击列表
  if g_FunctionNPC <> nil then
  begin
    for I := 0 to g_MapEventListOfDropItem.Count - 1 do
    begin
      MapEvent := g_MapEventListOfDropItem.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfPickUpItem.Count - 1 do
    begin
      MapEvent := g_MapEventListOfPickUpItem.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfMine.Count - 1 do
    begin
      MapEvent := g_MapEventListOfMine.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfDoMine.Count - 1 do
    begin
      MapEvent := g_MapEventListOfDoMine.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfWalk.Count - 1 do
    begin
      MapEvent := g_MapEventListOfWalk.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;
    for I := 0 to g_MapEventListOfRun.Count - 1 do
    begin
      MapEvent := g_MapEventListOfRun.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfScatterItem.Count - 1 do
    begin
      MapEvent := g_MapEventListOfScatterItem.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfHorseWalk.Count - 1 do
    begin
      MapEvent := g_MapEventListOfHorseWalk.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;

    for I := 0 to g_MapEventListOfHorseRun.Count - 1 do
    begin
      MapEvent := g_MapEventListOfHorseRun.Items[I];
      if MapEvent.StartScript.sLable <> '' then
        g_FunctionNPC.AddSelectLable(MapEvent.StartScript.sLable);
    end;
  end;
end;

function TFrmDB.LoadMapQuest(): Integer;
var
  FBList: TList;
  Map: TEnvirnoment;
  boGrouped: Boolean;
  I, k, Index: Integer;
  sFileName, tStr: string;
  tMapQuestList: TStringList;
  tmpValue, tmpBool: Integer;
  tmpMapName, tmpsVarName, tmpsBool, tmpMonName, tmpCondition, tmpScriptName, tmpGrouped, tmpsValue: string;
begin
  Result := 1;
  sFileName := g_Config.sEnvirDir + 'MapQuest.txt';

  if FileExists(sFileName) then
  begin
    tMapQuestList := TStringList.Create;
    tMapQuestList.LoadFromFile(sFileName);
    DeCodeStringList(tMapQuestList);

    for I := 0 to tMapQuestList.Count - 1 do
    begin
      tStr := Trim(tMapQuestList.Strings[I]);

      if (tStr <> '') and (tStr[1] <> ';') then
      begin
        tStr := GetValidStr3(tStr, tmpMapName, [' ', #9]);
        tStr := GetValidStr3(tStr, tmpsVarName, [' ', #9]);
        tStr := GetValidStr3(tStr, tmpsBool, [' ', #9]);
        tStr := GetValidStr3(tStr, tmpMonName, [' ', #9]);

        if (tmpMonName <> '') and (tmpMonName[1] = '"') then
          ArrestStringEx(tmpMonName, '"', '"', tmpMonName);

        tStr := GetValidStr3(tStr, tmpCondition, [' ', #9]);
        if (tmpCondition <> '') and (tmpCondition[1] = '"') then
          ArrestStringEx(tmpCondition, '"', '"', tmpCondition);

        tStr := GetValidStr3(tStr, tmpScriptName, [' ', #9]);
        tStr := GetValidStr3(tStr, tmpGrouped, [' ', #9]);

        ArrestStringEx(tmpsVarName, '[', ']', tmpsValue);
        tmpValue := StrToIntDef(tmpsValue, 0);
        tmpBool := StrToIntDef(tmpsBool, 0);
        boGrouped := SameText(tmpGrouped, 'GROUP');

        // 杀怪任务触发增加副本地图支持 chongchong 2016-03-05
        if (tmpMapName <> '') and (tmpMonName <> '') and (tmpScriptName <> '') then
        begin
          if tmpMapName = '*' then
          begin
            for k := 0 to g_MapManager.Count - 1 do
            begin
              Map := g_MapManager.Items[k];
              if (Map = nil) or not Map.CreateQuest(tmpValue, tmpBool, tmpMonName, tmpCondition, tmpScriptName, boGrouped) then
                Result := -I;
            end;
          end
          else
          begin
            if tmpMapName[1] = '$' then
            begin
              tmpMapName := Copy(tmpMapName, 2, MaxInt);
              Index := g_FBMapManager.IndexOf(tmpMapName);
              if Index <> -1 then
              begin
                FBList := TList(g_FBMapManager.Objects[Index]);
                for k := 0 to FBList.Count - 1 do
                begin
                  if not TEnvirnoment(FBList[k]).CreateQuest( //
                    tmpValue, tmpBool, tmpMonName, tmpCondition, tmpScriptName, boGrouped) then
                    Result := -I;
                end;
              end
            end
            else
            begin
              Map := g_MapManager.FindMap(tmpMapName);
              if (Map = nil) or not Map.CreateQuest(tmpValue, tmpBool, tmpMonName, tmpCondition, tmpScriptName, boGrouped) then
                Result := -I;
            end;
          end;
        end
        else
          Result := -I;
      end;
    end;
    tMapQuestList.Free;
  end;

  QMangeNPC();
  QFunctionNPC();
  QMissionNPC;
  RobotNPC();
  QBatterNPC(); // 读取连击经络脚本 piaoyun 2013-08-16
end;

function TFrmDB.LoadMerchant(): Integer;
var
  sFileName, sLineText, sScript, sMapName, sX, sY, sName, sFlag, sAppr, sIsCalste, sCanMove, sMoveTime, sAutoChangeColor,
    sAutoChangeColorTime, sDataFile, sData: string;
  tMerchantList: TStringList;
  tMerchantNPC: TMerchant;
  I, n100: Integer;
  k, Index: Integer;
  FBList: TList;
  sFBMapName: string;
  sMovePoints: string;
  PointList: TStringList;
  nCount, M: Integer;
begin
  sFileName := g_Config.sEnvirDir + 'Merchant.txt';
  if FileExists(sFileName) then
  begin
    PointList := TStringList.Create;
    tMerchantList := TStringList.Create;
    tMerchantList.LoadFromFile(sFileName);
    DeCodeStringList(tMerchantList);
    for I := 0 to tMerchantList.Count - 1 do
    begin
      sLineText := Trim(tMerchantList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sScript, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sMapName, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sX, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sY, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sName, [' ', #9]);
        if (sName <> '') and (sName[1] = '"') then
          ArrestStringEx(sName, '"', '"', sName);

        sLineText := GetValidStr3(sLineText, sFlag, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAppr, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sIsCalste, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sCanMove, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sMoveTime, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAutoChangeColor, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAutoChangeColorTime, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sDataFile, [' ', #9]);
        sMovePoints := '';
        Index := pos('[', sAppr);
        if Index > 0 then
        begin
          ArrestStringEx(sAppr, '[', ']', sMovePoints);
          sAppr := Copy(sAppr, 1, Index - 1);
        end;

        if (sScript <> '') and (sMapName <> '') and (sAppr <> '') then
        begin
          { TODO -ochongchong -c新增 : 副本地图 ---- 载入副本地图NPC信息 【2013-09-06】 }
          if sMapName[1] = '$' then
          begin
            sMapName := Copy(sMapName, 2, MaxInt);
            Index := g_FBMapManager.IndexOf(sMapName);
            if Index <> -1 then
            begin
              FBList := TList(g_FBMapManager.Objects[Index]);
              for k := 0 to FBList.Count - 1 do
              begin
                sFBMapName := TEnvirnoment(FBList[k]).sMapName;
                tMerchantNPC := TMerchant.Create;
                tMerchantNPC.m_boFB := True;
                tMerchantNPC.m_sFBName := sMapName;
                { TODO -ochongchong -c修改 : NPC对应脚本中的\改为/ 【2013-08-28】 }
                // tMerchantNPC.m_sScript := sScript;
                tMerchantNPC.m_sScript := WideStringReplace(sScript, '/', '\', [rfReplaceAll]);
                tMerchantNPC.m_sMapName := sFBMapName;
                tMerchantNPC.m_nCurrX := StrToIntDef(sX, 0);
                tMerchantNPC.m_nCurrY := StrToIntDef(sY, 0);
                tMerchantNPC.m_sCharName := sName;
                tMerchantNPC.m_nFlag := StrToIntDef(sFlag, 0);
                tMerchantNPC.m_wAppr := StrToIntDef(sAppr, 0);
                tMerchantNPC.m_dwMoveTime := StrToIntDef(sMoveTime, 0);
                tMerchantNPC.m_dwNpcAutoChangeColorTime := StrToIntDef(sAutoChangeColorTime, 0) * 1000;
                tMerchantNPC.m_sDataFileName := sDataFile;
                if CompareLStr(sName, '<$STR(', Length('<$STR(')) and (sName[Length(sName)] = '>') then
                begin
                  sData := sName;
                  sData := ArrestStringEx(sData, '(', ')', sName);
                end;

                n100 := GetValNameNo(sName);
                case n100 of
                  6000 .. 6999:
                    begin
                      tMerchantNPC.m_nGlobalAValIndex := n100 - 6000;
                      tMerchantNPC.m_sCharName := g_Config.GlobalAVal[tMerchantNPC.m_nGlobalAValIndex];
                      // ''; //g_Config.GlobalAVal[tMerchantNPC.m_nGlobalAValIndex];
                    end;
                end;

                if StrToIntDef(sIsCalste, 0) <> 0 then
                  tMerchantNPC.m_boCastle := True;
                if (StrToIntDef(sCanMove, 0) <> 0) and (tMerchantNPC.m_dwMoveTime > 0) then
                  tMerchantNPC.m_boCanMove := True;
                if StrToIntDef(sAutoChangeColor, 0) <> 0 then
                  tMerchantNPC.m_boNpcAutoChangeColor := True;

                if (tMerchantNPC.m_wAppr >= 10000) and (Length(sMovePoints) > 0) then
                begin
                  PointList.Clear;
                  nCount := ExtractStrings(['|'], [], PChar(sMovePoints), PointList);
                  if nCount > 0 then
                  begin
                    sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
                    SetLength(tMerchantNPC.MovePoint, nCount + 1);
                    tMerchantNPC.MovePoint[0].X := tMerchantNPC.m_nCurrX;
                    tMerchantNPC.MovePoint[0].Y := tMerchantNPC.m_nCurrY;
                    tMerchantNPC.MovePoint[1].X := StrToIntDef(sX, 0);
                    tMerchantNPC.MovePoint[1].Y := StrToIntDef(sY, 0);
                    for M := 1 to nCount - 1 do
                    begin
                      sY := GetValidStr3(PointList[M], sX, [' ', ',', #9]);
                      tMerchantNPC.MovePoint[M + 1].X := StrToIntDef(sX, 0);
                      tMerchantNPC.MovePoint[M + 1].Y := StrToIntDef(sY, 0);
                    end;
                  end;
                end;
                UserEngine.AddMerchant(tMerchantNPC);
              end;
            end;
          end
          else
          begin
            tMerchantNPC := TMerchant.Create;
            { TODO -ochongchong -c修改 : NPC对应脚本中的\改为/ 【2013-08-28】 }
            // tMerchantNPC.m_sScript := sScript;
            tMerchantNPC.m_sScript := WideStringReplace(sScript, '/', '\', [rfReplaceAll]);
            tMerchantNPC.m_sMapName := sMapName;
            tMerchantNPC.m_nCurrX := StrToIntDef(sX, 0);
            tMerchantNPC.m_nCurrY := StrToIntDef(sY, 0);
            tMerchantNPC.m_sCharName := sName;
            tMerchantNPC.m_nFlag := StrToIntDef(sFlag, 0);
            tMerchantNPC.m_wAppr := StrToIntDef(sAppr, 0);
            tMerchantNPC.m_dwMoveTime := StrToIntDef(sMoveTime, 0);
            tMerchantNPC.m_dwNpcAutoChangeColorTime := StrToIntDef(sAutoChangeColorTime, 0) * 1000;
            tMerchantNPC.m_sDataFileName := sDataFile;
            if CompareLStr(sName, '<$STR(', Length('<$STR(')) and (sName[Length(sName)] = '>') then
            begin
              sData := sName;
              sData := ArrestStringEx(sData, '(', ')', sName);
            end;

            n100 := GetValNameNo(sName);
            case n100 of
              6000 .. 6999:
                begin
                  tMerchantNPC.m_nGlobalAValIndex := n100 - 6000;
                  tMerchantNPC.m_sCharName := g_Config.GlobalAVal[tMerchantNPC.m_nGlobalAValIndex];
                  // ''; //g_Config.GlobalAVal[tMerchantNPC.m_nGlobalAValIndex];
                end;
            end;
            if StrToIntDef(sIsCalste, 0) <> 0 then
              tMerchantNPC.m_boCastle := True;
            if (StrToIntDef(sCanMove, 0) <> 0) and (tMerchantNPC.m_dwMoveTime > 0) then
              tMerchantNPC.m_boCanMove := True;
            if StrToIntDef(sAutoChangeColor, 0) <> 0 then
              tMerchantNPC.m_boNpcAutoChangeColor := True;
            if (tMerchantNPC.m_wAppr >= 10000) and (Length(sMovePoints) > 0) then
            begin
              PointList.Clear;
              nCount := ExtractStrings(['|'], [], PChar(sMovePoints), PointList);
              if nCount > 0 then
              begin
                sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
                SetLength(tMerchantNPC.MovePoint, nCount + 1);
                tMerchantNPC.MovePoint[0].X := tMerchantNPC.m_nCurrX;
                tMerchantNPC.MovePoint[0].Y := tMerchantNPC.m_nCurrY;
                tMerchantNPC.MovePoint[1].X := StrToIntDef(sX, 0);
                tMerchantNPC.MovePoint[1].Y := StrToIntDef(sY, 0);
                for M := 1 to nCount - 1 do
                begin
                  sY := GetValidStr3(PointList[M], sX, [' ', ',', #9]);
                  tMerchantNPC.MovePoint[M + 1].X := StrToIntDef(sX, 0);
                  tMerchantNPC.MovePoint[M + 1].Y := StrToIntDef(sY, 0);
                end;
              end;
            end;
            UserEngine.AddMerchant(tMerchantNPC);
          end;
        end;
      end;
    end;
    PointList.Free;
    tMerchantList.Free;
  end;
  Result := 1;
  ResetCustomNpcList;
end;

function TFrmDB.LoadMinMap: Integer;
var
  sFileName, tStr, sMapNO, sMapIdx: string;
  tMapList: TStringList;
  I, nIdx: Integer;
begin
  Result := 0;
  sFileName := g_Config.sEnvirDir + 'MiniMap.txt';
  if FileExists(sFileName) then
  begin
    MiniMapList.Clear;
    tMapList := TStringList.Create;
    tMapList.LoadFromFile(sFileName);
    DeCodeStringList(tMapList);
    for I := 0 to tMapList.Count - 1 do
    begin
      tStr := tMapList.Strings[I];
      if (tStr <> '') and (tStr[1] <> ';') then
      begin
        tStr := GetValidStr3(tStr, sMapNO, [' ', #9]);
        tStr := GetValidStr3(tStr, sMapIdx, [' ', #9]);
        nIdx := StrToIntDef(sMapIdx, 0);
        if nIdx > 0 then
          MiniMapList.AddObject(sMapNO, TObject(nIdx));
      end;
    end;
    tMapList.Free;
  end;
end;

function TFrmDB.LoadMonGen(): Integer;

  procedure LoadMapGen(MonGenList: TStringList; sFileName: AnsiString);
  resourcestring
    HookError = '[Exception] HookLoadScriptFile';
  var
    I: Integer;
    sFilePatchName: string;
    sFileDir: string;
    sData: string;
    LoadList: TStringList;
    MemoryStream: TMemoryStream;
  begin
    sFileDir := g_Config.sEnvirDir + 'MonGen\';
    if not DirectoryExists(sFileDir) then
    begin
      CreateDir(sFileDir);
    end;
    sFilePatchName := sFileDir + sFileName;
    if FileExists(sFilePatchName) then
    begin
      LoadList := TStringList.Create;
      LoadList.LoadFromFile(sFilePatchName);
      DeCodeStringList(LoadList);
      for I := 0 to LoadList.Count - 1 do
      begin
        MonGenList.Add(LoadList.Strings[I]);
      end;
      LoadList.Free;
    end
    else
    begin
      if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
      begin
        try
          MemoryStream := TMemoryStream.Create;
          if not g_PluginManager.HookLoadScriptFile(PAnsiChar('MonGen\' + sFileName), MemoryStream) then
          begin
            MemoryStream.Free;
            Exit;
          end;
          if (MemoryStream.Size > 0) then
          begin
            // SetLength(sData, MemoryStream.Size);
            // Move(MemoryStream.Memory^, sData[1], MemoryStream.Size);
            sData := DeCodePlugBuffer(MemoryStream);
            LoadList := TStringList.Create;
            LoadList.Text := sData;
            DeCodeStringList(LoadList);
            for I := 0 to LoadList.Count - 1 do
            begin
              MonGenList.Add(LoadList.Strings[I]);
            end;
            LoadList.Free;
          end;
          MemoryStream.Free;
        except
          on E: Exception do
          begin
            MainOutMessage(HookError);
            MainOutMessage(E.Message);
            Exit;
          end;
        end;
      end;
    end;
  end;
// 加这个是为了让系统其他地方配置的怪物有地方可以挂靠(火龙守护，沙巴克城墙等)

  procedure AddEmptyMonGenInfo();
  var
    Info: pTMonGenInfo;
  begin
    New(Info);
    FillChar(Info^, SizeOf(Info), #0);
    Info.sMapName := '';
    Info.sMonName := '';
    Info.CertList := TList.Create;
    Info.Envir := nil;
{$IF MULTI_THREAD = 1}
    if g_MultiThreadRun then
      UserEngine.m_MonGenList.LockW(1);
    try
{$IFEND}
      UserEngine.m_MonGenList.Add(Info);
{$IF MULTI_THREAD = 1}
    finally
      if g_MultiThreadRun then
        UserEngine.m_MonGenList.UnLockW;
    end;
{$IFEND}
  end;

var
  sFileName, sLineText, sData: string;
  MonGenInfo, MonGenInfo2: pTMonGenInfo;
  LoadList: TStringList;
  sMapGenFile: string;
  Envir: TEnvirnoment;
  I: Integer;
  MemoryStream: TMemoryStream;
  k, Index: Integer;
  FBList: TList;
  sMapName: string;
  CompareType: TCompareType;
begin
  Result := 0;
  LoadList := nil;
  sFileName := g_Config.sEnvirDir + 'MonGen.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    DeCodeStringList(LoadList);
  end
  else if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
  begin
    try
      MemoryStream := TMemoryStream.Create;
      if g_PluginManager.HookLoadScriptFile(PAnsiChar('MonGen.txt'), MemoryStream) then
      begin
        LoadList := TStringList.Create;
        if (MemoryStream.Size > 0) then
        begin
          // SetLength(sData, MemoryStream.Size);
          // Move(MemoryStream.Memory^, sData[1], MemoryStream.Size);
          sData := DeCodePlugBuffer(MemoryStream);
          LoadList.Text := sData;
          DeCodeStringList(LoadList);
        end;
      end
      else
      begin
        AddEmptyMonGenInfo;
      end;
      MemoryStream.Free;
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] HookLoadScriptFile');
        MainOutMessage(E.Message);
        AddEmptyMonGenInfo;
        Exit;
      end;
    end;
  end
  else
  begin
    AddEmptyMonGenInfo;
    Exit;
  end;
  if LoadList = nil then
    Exit;
  I := 0;
  while (True) do
  begin
    if I >= LoadList.Count then
      Break;
    if CompareLStr('loadgen', LoadList.Strings[I], Length('loadgen')) then
    begin
      sMapGenFile := GetValidStr3(LoadList.Strings[I], sLineText, [' ', #9]);
      LoadList.Delete(I);
      if sMapGenFile <> '' then
      begin
        LoadMapGen(LoadList, sMapGenFile);
      end;
    end;
    Inc(I);
  end;
  for I := 0 to LoadList.Count - 1 do
  begin
    sLineText := LoadList.Strings[I];
    if (sLineText <> '') and (sLineText[1] <> ';') then
    begin
      New(MonGenInfo);
      FillChar(MonGenInfo^, SizeOf(MonGenInfo), #0);
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.sMapName := sData;
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.nX := StrToIntDef(sData, 0);
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.nY := StrToIntDef(sData, 0);
      sLineText := GetValidStrCap(sLineText, sData, [' ', #9]);
      if (sData <> '') and (sData[1] = '"') then
        ArrestStringEx(sData, '"', '"', sData);
      MonGenInfo.sMonName := sData;
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.nRange := StrToIntDef(sData, 0);
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.nCount := StrToIntDef(sData, 0);
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.dwZenTime := StrToIntDef(sData, -1) * 60 * 1000;
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      if not IsStringNumber(sData) then
        sData := '0';
      MonGenInfo.nMissionGenRate := StrToIntDef(sData, 0); // 集中座标刷新机率 1 -100
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.btNameColor := StrToIntDef(sData, 255); // 名称颜色
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.boIsNGMon := (Length(sData) <> 0) and (sData <> '0'); // 是否是内功怪
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.sNationaID := sData; // 国家名称
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.boCanAttackSameNationPlayer := (Length(sData) <> 0) and (sData <> '0'); // 是否攻击同国家的玩家
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.boNoSameNationMonPK := (Length(sData) <> 0) and (sData <> '0'); // 不同国家名称的怪物PK
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      MonGenInfo.boAllowSameNationPlayerAttack := (Length(sData) <> 0) and (sData <> '0'); // 能否被同国家的人物攻击
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]);
      if (Length(sData) > 0) and (sData[1] = '@') then
        MonGenInfo.TriggerScript := sData
      else
        MonGenInfo.TriggerScript := '';
      if (MonGenInfo.sMapName = '') or (MonGenInfo.sMonName = '') or (MonGenInfo.dwZenTime = 0) then
      begin
        DisPose(MonGenInfo);
        Continue;
      end;
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]); // G变量的序号 By 一支笔 at:2021-06-09 13:48:03
      if (sData <> '') and (sData[1] = 'G') then
        sData := Copy(sData, 2, MaxInt);
      MonGenInfo.GVarIndex := StrToIntDef(sData, -1);
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]); // G变量判断模式
      if sData = '<' then
        CompareType := ctLess
      else if sData = '=' then
        CompareType := ctEqual
      else if sData = '>' then
        CompareType := ctGreater
      else if sData = '<=' then
        CompareType := ctLessEqual
      else if sData = '>=' then
        CompareType := ctGreaterEqual
      else if sData = '<>' then
        CompareType := ctNotEqual
      else
        CompareType := ctFail;
      MonGenInfo.GVarCompareType := CompareType;
      sLineText := GetValidStr3(sLineText, sData, [' ', #9]); // G变量的值
      MonGenInfo.GVarValue := StrToIntDef(sData, 0);
      { TODO -ochongchong -c新增 : 副本地图 ---- 载入副本地图怪物信息 【2013-09-06】 }
      if MonGenInfo.sMapName[1] = '$' then
      begin
        sMapName := Copy(MonGenInfo.sMapName, 2, MaxInt);
        Index := g_FBMapManager.IndexOf(sMapName);
        if Index <> -1 then
        begin
          MonGenInfo.boNoManNoMon := False;
          MonGenInfo.boFB := True;
          MonGenInfo.Envir := nil;
          MonGenInfo.nRace := UserEngine.GetMonRace(MonGenInfo.sMonName);
          if MonGenInfo.nRace <> -1 then
          begin
            MonGenInfo.CertList := TList.Create;
            MonGenInfo.dwStartTick := 0;
{$IF MULTI_THREAD = 1}
            if g_MultiThreadRun then
              UserEngine.m_MonGenList.LockW(4);
            try
{$IFEND}
              UserEngine.m_MonGenList.Add(MonGenInfo);
{$IF MULTI_THREAD = 1}
            finally
              if g_MultiThreadRun then
                UserEngine.m_MonGenList.UnLockW;
            end;
{$IFEND}
            FBList := TList(g_FBMapManager.Objects[Index]);
            for k := 0 to FBList.Count - 1 do
            begin
              Envir := FBList[k];
              New(MonGenInfo2); // MonGenInfo2.CertList 共MonGenInfo.CertList,只引用指针，不管理对象
              MonGenInfo2^ := MonGenInfo^;
              MonGenInfo2.boFB := True;
              MonGenInfo2.sMapName := Envir.sMapName;
              MonGenInfo2.Envir := Envir;
              Envir.m_FBMonGenList.Add(MonGenInfo2);
            end;
          end
          else
            DisPose(MonGenInfo);
        end;
      end
      else
      begin
        Envir := g_MapManager.GetMapInfo(nServerIndex, MonGenInfo.sMapName);
        if (Envir = nil) then
        begin
          DisPose(MonGenInfo);
          Continue;
        end;
        MonGenInfo.boNoManNoMon := True;
        MonGenInfo.Envir := g_MapManager.FindMap(MonGenInfo.sMapName);
        MonGenInfo.boFB := False;
        MonGenInfo.dwStartTick := 0;
        if MonGenInfo.Envir <> nil then
        begin
          MonGenInfo.CertList := TList.Create;
{$IF MULTI_THREAD = 1}
          if g_MultiThreadRun then
            UserEngine.m_MonGenList.LockW(5);
          try
{$IFEND}
            UserEngine.m_MonGenList.Add(MonGenInfo);
{$IF MULTI_THREAD = 1}
          finally
            if g_MultiThreadRun then
              UserEngine.m_MonGenList.UnLockW;
          end;
{$IFEND}
          UserEngine.AddMapMonGenCount(MonGenInfo.sMapName, MonGenInfo.nCount);
        end
        else
          DisPose(MonGenInfo);
      end;
    end;
  end;
  New(MonGenInfo);
  FillChar(MonGenInfo^, SizeOf(MonGenInfo), #0);
  MonGenInfo.sMapName := '';
  MonGenInfo.sMonName := '';
  MonGenInfo.CertList := TList.Create;
  MonGenInfo.Envir := nil;
  MonGenInfo.boNoManNoMon := False;
  MonGenInfo.dwStartTick := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_MonGenList.LockW(6);
  try
{$IFEND}
    UserEngine.m_MonGenList.Add(MonGenInfo);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_MonGenList.UnLockW;
  end;
{$IFEND}
  LoadList.Free;
  Result := 1;
  UserEngine.DoInitSortMapMonGenList;
end;

function TFrmDB.LoadMonsterDB(): Integer;
var
  I, J: Integer;
  Monster: pTMonInfo;
  nTemp: Integer;
  CustomMonsterConfig: TCustomMonsterConfig;
  IsFoundCustomMonster: Boolean;
  InBuf: PAnsiChar;
  InBytes: Integer;
  ClientConfig: PClientCustomMonsterConfig;
  TempQuery: TDataSet;
  IsFound: Boolean;
{$IFDEF USE_FDTABLE}
  ClientDataSet: TFDMemTable;
{$ELSE}
  ClientDataSet: TkbmMemTable;
{$ENDIF}
  nRet, ColumnCount: Integer;
  ColumnName, ColumnType: string;
  FieldType: TFieldType;
  FieldLen: Integer;
  FieldDec: Integer;
  sm: TSQLStatement;
resourcestring
  sSQLString = 'select * from Monster';
begin
  Result := 0;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.MonsterList.LockW(1);
  try
{$IFEND}
    for I := 0 to UserEngine.MonsterList.Count - 1 do
    begin
      Monster := pTMonInfo(UserEngine.MonsterList.Items[I]);
      if Assigned(Monster.ItemList) then
      begin
        ClearMonItemList(Monster.ItemList);
        Monster.ItemList.Free;
      end;
      DisPose(Monster);
    end;
    UserEngine.MonsterList.Clear;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.MonsterList.UnLockW;
  end;
{$IFEND}
  ClientDataSet := {$IFDEF USE_FDTABLE} TFDMemTable.Create(nil); {$ELSE}
    TkbmMemTable.Create(nil); {$ENDIF}
  try
    if g_boUseSqliteDB then
    begin
      TempQuery := ClientDataSet;
      sm := SQLiteDB.Statements.AddSQLStatement('Select_Monster');
      sm.Sql := 'select * from Monster';
      sm.Prepare;
      try
        nRet := sm.Step;
        if nRet = SQLITE_ROW then
        begin
          ColumnCount := sm.GetColumnCount;
          for I := 0 to ColumnCount - 1 do
          begin
            ColumnName := sm.GetColumnName(I);
            ColumnType := sm.GetColumnDeclType(I);
            if ColumnType = '' then
              GetFieldInfo('string', FieldType, FieldLen, FieldDec) // OL
            else
              GetFieldInfo(ColumnType, FieldType, FieldLen, FieldDec);
            if (FieldType <> ftString) and (FieldType <> ftWideString) then
            begin
              with ClientDataSet.FieldDefs.AddFieldDef do
              begin
                Name := ColumnName;
                DataType := FieldType;
                if FieldType = ftFloat then
                  Precision := FieldDec;
              end
            end
            else
            begin
              with ClientDataSet.FieldDefs.AddFieldDef do
              begin
                Name := ColumnName;
                DataType := FieldType;
                Size := FieldLen;
              end;
            end;
          end;
{$IFDEF USE_FDTABLE}
          ClientDataSet.CreateDataSet;
{$ELSE}
          ClientDataSet.CreateTable;
{$ENDIF}
          ClientDataSet.Active := True;
          while (nRet = SQLITE_ROW) do
          begin
            ClientDataSet.Append;
            for I := 0 to ColumnCount - 1 do
            begin
              case ClientDataSet.FieldDefs[I].DataType of
                ftWideString:
                  begin
                    ClientDataSet.Fields[I].AsString := sm.GetColumnValueText(I);
                  end;
                ftString:
                  begin // DI
                    ClientDataSet.Fields[I].AsString := sm.GetColumnValueText(I);
                  end;
                ftInteger:
                  begin
                    ClientDataSet.Fields[I].AsInteger := sm.GetColumnValueInt(I);
                  end;
              else
                begin
                  raise Exception.Create('Monster 无法识别的数据类型, 字段：' + ClientDataSet.FieldDefs[I].Name);
                end;
              end;
            end;
            nRet := sm.Step;
          end;
        end;
      finally
        sm.Finalize;
      end;
    end
    else
    begin
{$IFNDEF CPUX64}
      TempQuery := Query;
      Query.Sql.Clear;
      Query.Sql.Add(sSQLString);
      try
        Query.Open;
      finally
        Result := -1;
      end;
{$ELSE}
      Result := -1000;
      MainOutMessage('[错误]***64位引擎不支持BDE数据库***');
      Exit;
{$ENDIF}
    end;
    // 复制到 ----> FItemsDataSet
    TempQuery.First;
{$IFDEF USE_FDTABLE}
    FMonsterDataSet.CopyDataSet(TempQuery, [coStructure, coRestart, coAppend]);
{$ELSE}
    FMonsterDataSet.LoadFromDataSet(TempQuery, [mtcpoStructure, mtcpoProperties]);
{$ENDIF}
    TempQuery.First;
    for I := 0 to TempQuery.RecordCount - 1 do
    begin
      New(Monster);
      FillChar(Monster^, SizeOf(TMonInfo), 0);
      { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-18】 }
      // Monster.ItemList := TList.Create;
      Monster.btRace := TempQuery.FieldByName('Race').AsInteger;
      Monster.sName := Trim(TempQuery.FieldByName('Name').AsString);
      Monster.btRaceImg := TempQuery.FieldByName('RaceImg').AsInteger;
      Monster.wAppr := TempQuery.FieldByName('Appr').AsInteger;
      Monster.nLevel := TempQuery.FieldByName('Lvl').AsInteger;
      Monster.btLifeAttrib := TempQuery.FieldByName('Undead').AsInteger;
      Monster.wCoolEye := TempQuery.FieldByName('CoolEye').AsInteger;
      Monster.dwExp := TempQuery.FieldByName('Exp').AsInteger;
      // 城门或城墙的状态跟HP值有关，如果HP异常，将导致城墙显示不了
      if Monster.btRace in [110, 111] then
      begin // 如果为城墙或城门由HP不加倍
        Monster.nHP := TempQuery.FieldByName('HP').AsInteger;
      end
      else
      begin
        Monster.nHP := Round(TempQuery.FieldByName('HP').AsInteger * (g_Config.nMonsterPowerRate / 10));
      end;
      Monster.nMP := Round(TempQuery.FieldByName('MP').AsInteger * (g_Config.nMonsterPowerRate / 10));
      Monster.nAC := Min(High(LongWord), Round(TempQuery.FieldByName('AC').AsInteger * (g_Config.nMonsterPowerRate / 10)));
      Monster.nMAC := Min(High(LongWord), Round(TempQuery.FieldByName('MAC').AsInteger * (g_Config.nMonsterPowerRate / 10)));
      Monster.nDC := Min(High(LongWord), Round(TempQuery.FieldByName('DC').AsInteger * (g_Config.nMonsterPowerRate / 10)));
      Monster.nMaxDC := Min(High(LongWord), Round(TempQuery.FieldByName('DCMAX').AsInteger * (g_Config.nMonsterPowerRate / 10)));
      if Monster.nDC > Monster.nMaxDC then
      begin
        nTemp := Monster.nDC;
        Monster.nDC := Monster.nMaxDC;
        Monster.nMaxDC := nTemp;
      end;
      Monster.nMC := Min(High(LongWord), Round(TempQuery.FieldByName('MC').AsInteger * (g_Config.nMonsterPowerRate / 10)));
      Monster.nSC := Min(High(LongWord), Round(TempQuery.FieldByName('SC').AsInteger * (g_Config.nMonsterPowerRate / 10)));
      Monster.wSpeed := TempQuery.FieldByName('SPEED').AsInteger;
      Monster.wHitPoint := TempQuery.FieldByName('HIT').AsInteger;
      Monster.wWalkSpeed := _MAX(200, TempQuery.FieldByName('WALK_SPD').AsInteger);
      Monster.wWalkStep := _MAX(1, TempQuery.FieldByName('WalkStep').AsInteger);
      Monster.wWalkWait := TempQuery.FieldByName('WalkWait').AsInteger;
      Monster.wAttackSpeed := TempQuery.FieldByName('ATTACK_SPD').AsInteger;
      Monster.ExploreItem := TempQuery.FieldByName('ExploreItem').AsInteger;
      Monster.boDisableSimpleActor := TempQuery.FieldByName('DisableSimpleActor').AsInteger = 1;
      // Monster.nAttackState := TempQuery.FieldByName('AttackState').AsInteger;
      // Monster.wAttackSource := TempQuery.FieldByName('AttackSource').AsInteger;
      IsFound := UserEngine.FindMonster(Monster.sName, J);
      if not IsFound then
      begin
        Monster.ItemList := nil;
        LoadMonitems(Monster.sName, Monster.ItemList);
        LoadIconFile(nil, @Monster.Icons, sMonIcons, Monster.sName);
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.MonsterList.LockW(1000);
{$IFEND}
        UserEngine.MonsterList.Insert(J, Monster);
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.MonsterList.UnLockW; // UnlockFile;
{$IFEND}
      end
      else
      begin
        MainOutMessage('重复的怪物名称: ' + Monster.sName, True);
        DisPose(Monster);
      end;
      Result := 1;
      // 自定义怪物列表 chongchong 2014-07-19
      // 此处只添加对象，不删除对象，不然自定义对象绑定的TCustomMonsterConfig会非法
      if (not IsFound) and (Monster.btRace in [154 { 魔王岭怪物 } , 155 { 魔王岭宝宝 } , 156 { 普通怪物 } , 157 { 不主动攻击，可挖尸体怪物 } , 159 { 采集怪 } ]
        ) and (Monster.btRaceImg = 156) then
      begin
        IsFoundCustomMonster := False;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.m_CustomMonsterList.LockW(1);
        try
{$IFEND}
          for J := 0 to UserEngine.m_CustomMonsterList.Count - 1 do
          begin
            CustomMonsterConfig := UserEngine.m_CustomMonsterList[J];
            if CustomMonsterConfig.MonsterAppr = Monster.wAppr then
            begin
              CustomMonsterConfig.MonsterName := Monster.sName;
              IsFoundCustomMonster := True;
              Break;
            end;
          end;
          if not IsFoundCustomMonster then
          begin
            CustomMonsterConfig := TCustomMonsterConfig.Create(Monster.sName, Monster.btRace, Monster.wAppr);
            UserEngine.m_CustomMonsterList.Add(CustomMonsterConfig);
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            UserEngine.m_CustomMonsterList.UnLockW;
        end;
{$IFEND}
      end;
      TempQuery.Next;
    end;
    TempQuery.Close;
  finally
    ClientDataSet.Free;
  end;
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomMonsterList.LockR(2);
  try
{$IFEND}
    InBytes := UserEngine.m_CustomMonsterList.Count * SizeOf(TClientCustomMonsterConfig);
    GetMem(InBuf, InBytes + 1);
    try
      ClientConfig := PClientCustomMonsterConfig(InBuf);
      for I := 0 to UserEngine.m_CustomMonsterList.Count - 1 do
      begin
        CustomMonsterConfig := UserEngine.m_CustomMonsterList.Items[I];
        ClientConfig^.wMonsterAppr := CustomMonsterConfig.MonsterAppr;
        ClientConfig^.BaseConfig := CustomMonsterConfig.ClientBaseConfig;
        ClientConfig^.Actions := CustomMonsterConfig.ClientActions;
        ClientConfig^.AttackConfigs := CustomMonsterConfig.ClientAttackConfigs;
        Inc(ClientConfig);
      end;
      g_CustomMonsterListTextLen := InBytes;
      g_CustomMonsterListText := zLibCompressBuffer(InBuf, InBytes);
      g_CustomMonsterListTextCRC := BufferCrc(PAnsiChar(g_CustomMonsterListText), Length(g_CustomMonsterListText));
    finally
      FreeMem(InBuf, InBytes + 1);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomMonsterList.UnLockR;
  end;
{$IFEND}
end;

function TFrmDB.LoadMonitems(MonName: string; var ItemList: TList): Integer;

  function LoadCallScript(sFileName, sLabel: string; var List: TStringList; Index: Integer): Boolean;
  var
    I, InserIndex: Integer;
    LoadStrList: TStringList;
    bo1D: Boolean;
    s18: string;
  begin
    Result := False;
    if FileExists(sFileName) then
    begin
      LoadStrList := TStringList.Create;
      LoadStrList.LoadFromFile(sFileName);
      DeCodeStringList(LoadStrList);
      sLabel := '[' + sLabel + ']';
      bo1D := False;
      InserIndex := Index + 1;
      for I := 0 to LoadStrList.Count - 1 do
      begin
        s18 := Trim(LoadStrList.Strings[I]);
        if s18 <> '' then
        begin
          if (s18[1] = '{') and (Length(s18) = 1) then
          begin
            Continue;
          end
          else
          begin
            if not bo1D then
            begin
              if (s18[1] = '[') and (CompareText(s18, sLabel) = 0) then
              begin
                bo1D := True;
                // [@区段] 不用插件
                // List.Insert(InserIndex, s18);
                // Inc(InserIndex);
              end
              else
              begin
              end;
            end
            else
            begin
              if (s18[1] = '}') then
              begin
                Result := True;
                Break;
              end
              else
              begin
                List.Insert(InserIndex, s18);
                Inc(InserIndex);
              end;
            end;
          end;
        end;
      end;
      LoadStrList.Free;
    end;
  end;

  function LoadCallScript2(LoadStrList: TStringList; sLabel: string; var List: TStringList; Index: Integer): Boolean;
  var
    I, InserIndex: Integer;
    bo1D: Boolean;
    s18: string;
  begin
    Result := False;
    DeCodeStringList(LoadStrList);
    sLabel := '[' + sLabel + ']';
    bo1D := False;
    InserIndex := Index + 1;
    for I := 0 to LoadStrList.Count - 1 do
    begin
      s18 := Trim(LoadStrList.Strings[I]);
      if s18 <> '' then
      begin
        if (s18[1] = '{') and (Length(s18) = 1) then
        begin
          Continue;
        end
        else
        begin
          if not bo1D then
          begin
            if (s18[1] = '[') and (CompareText(s18, sLabel) = 0) then
            begin
              bo1D := True;
              // [@区段] 不用插件
              // List.Insert(InserIndex, s18);
              // Inc(InserIndex);
            end
            else
            begin
            end;
          end
          else
          begin
            if (s18[1] = '}') then
            begin
              Result := True;
              Break;
            end
            else
            begin
              List.Insert(InserIndex, s18);
              Inc(InserIndex);
            end;
          end;
        end;
      end;
    end;
  end;

var
  nLine: Integer;
  s24: string;
  sFileName: AnsiString;
  LoadList: TStringList;
  MonItem: pTMonItemInfo;
  sLine, s18, s20, s28, s2C, s30, s1C, s34: string;
  n20: Integer;
  sSelPoint, sMaxPoint: string;
  nSelPoint, nMaxPoint: Integer;
  boSelPointIsVar, boMaxPointIsVar: Boolean;
  nLevel: Integer;
  boBegin: Boolean;
  boRandom: Boolean;
  AddList: TList;
  ArrayList: TList;
  MemoryStream: TMemoryStream;
  TempStrings: TStringList;
  I, II: Integer;
  boCheckVar, IsOK: Boolean;
  nInheritedVarType: Byte;
  boCheckVarUseOR: Boolean;
  TempN: Integer;
  CheckVarArr: TMonItemCheckVarArr;
  CheckVarArrIndex: Integer;
  TriggerScrpit: string;
  Var1Name, Var2Name: Char;
  Var1Index, Var2Index: Integer;
  CompareType: TCompareType;
  sTemp1, sTemp2: string;
begin
  Result := 0;
  if ItemList <> nil then
  begin
    ClearMonItemList(ItemList);
    ItemList.Clear;
  end
  else
    ItemList := TList.Create;
  s24 := g_Config.sEnvirDir + 'MonItems\' + MonName + '.txt';
  if not FileExists(s24) then
  begin
    // 怪物爆率支持远程脚本 chongchong 2016-11-25
    if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
    begin
      try
        MemoryStream := TMemoryStream.Create;
        sFileName := 'MonItems\' + MonName + '.txt';
        if not g_PluginManager.HookLoadScriptFile(PAnsiChar(sFileName), MemoryStream) then
        begin
          MemoryStream.Free;
          Exit;
        end;
        LoadList := TStringList.Create;
        if (MemoryStream.Size > 0) then
        begin
          // SetLength(s30, MemoryStream.Size);
          // Move(MemoryStream.Memory^, s30[1], MemoryStream.Size);
          s30 := DeCodePlugBuffer(MemoryStream);
          LoadList.Text := s30;
        end;
        MemoryStream.Free;
      except
        on E: Exception do
        begin
          MainOutMessage('[Exception] HookLoadScriptFile MapInfo.txt');
          MainOutMessage(E.Message);
          Exit;
        end;
      end;
    end
    else
    begin
      Exit;
    end;
    DeCodeStringList(LoadList);
  end
  else
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(s24);
  end;
  nLevel := 0;
  boBegin := False;
  boRandom := False;
  ArrayList := TList.Create;
  AddList := ItemList;
  boCheckVar := False;
  boCheckVarUseOR := False;
  TriggerScrpit := '';
  nInheritedVarType := 0;
  Var1Index := 0;
  Var2Index := 0;
  for I := Low(CheckVarArr) to High(CheckVarArr) do
  begin
    CheckVarArr[I].IsUse := False;
    CheckVarArr[I].Var1Name := '-';
    CheckVarArr[I].Var1Index := 0;
    CheckVarArr[I].CompareType := ctFail;
    CheckVarArr[I].Var2Name := '-';
    CheckVarArr[I].Var2Index := 0;
  end;
  sSelPoint := '';
  sMaxPoint := '';
  nSelPoint := -1;
  nMaxPoint := -1;
  boSelPointIsVar := False;
  boMaxPointIsVar := False;
  try
    nLine := 0;
    while True do
    begin
      if nLine >= LoadList.Count then
        Break;
      s28 := LoadList.Strings[nLine];
      sLine := s28;
      if (s28 = '') or (s28[1] = ';') then
      begin
        Inc(nLine);
        if nLine >= LoadList.Count then
          Break
        else
          Continue;
      end;
      s28 := Trim(s28);
      if (s28 <> '') and (s28[1] = '#') and (CompareLStr(s28, '#CALL', Length('#CALL'))) then
      begin
        s28 := ArrestStringEx(s28, '[', ']', s1C);
        s20 := Trim(s1C);
        s18 := Trim(s28);
        if (Length(s20) > 0) and (s20[1] = '\') then
          s20 := Copy(s20, 2, Length(s20) - 1);
        if (Length(s20) > 0) and (s20[1] = '\') then
          s20 := Copy(s20, 2, Length(s20) - 1);
        s34 := g_Config.sEnvirDir + 'QuestDiary\' + s20;
        // 支持远程脚本 2019-05-14 17:58:43
        if not FileExists(s34) then
        begin
          if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
          begin
            try
              s34 := ReplaceChar('QuestDiary\' + s20, '/', '\');
              MemoryStream := TMemoryStream.Create;
              if not g_PluginManager.HookLoadScriptFile(PAnsiChar(AnsiString(s34)), MemoryStream) then
              begin
                MemoryStream.Free;
                Exit;
              end;
              TempStrings := TStringList.Create;
              try
                if (MemoryStream.Size > 0) then
                begin
                  TempStrings.Text := DeCodePlugBuffer(MemoryStream);
                  if LoadCallScript2(TempStrings, s18, LoadList, nLine) then
                  begin
                    LoadList.Strings[nLine] := ';' + sLine;
                  end;
                end;
              finally
                TempStrings.Free;
              end;
              MemoryStream.Free;
            except
              on E: Exception do
              begin
                MainOutMessage('[Exception] HookLoadScriptFile');
                MainOutMessage(E.Message);
                Exit;
              end;
            end;
          end
          else
          begin
            Result := 1;
            MainOutMessage('脚本文件未找到: ' + s34);
            Exit;
          end;
        end
        else if LoadCallScript(s34, s18, LoadList, nLine) then
        begin
          LoadList.Strings[nLine] := ';' + sLine;
        end
        else
          MainOutMessage('脚本读取失败：' + s20 + s18);
      end
      else if (s28 = ')') or (s28 = '）') then // 支持中文括号 2020-08-15 20:02:54
      begin
        if nLevel > 0 then
        begin
          Dec(nLevel);
          AddList := TList(ArrayList[nLevel]);
          ArrayList.Delete(nLevel);
        end;
      end
      else if boBegin then
      begin
        if (s28 = '(') or (s28 = '（') then // 支持中文括号 2020-08-15 20:02:54
        begin
          if (sSelPoint <> '') and (sMaxPoint <> '') then
          begin
            New(MonItem);
            MonItem.SelPoint.nValue := nSelPoint; // - 1;
            MonItem.SelPoint.IsUseVar := boSelPointIsVar;
            MonItem.SelPoint.VarName := sSelPoint;
            MonItem.MaxPoint.nValue := nMaxPoint;
            MonItem.MaxPoint.IsUseVar := boMaxPointIsVar;
            MonItem.MaxPoint.VarName := sMaxPoint;
            MonItem.boGold := False;
            MonItem.List := TList.Create;
            MonItem.boRandom := boRandom;
            MonItem.Count := 0;
            MonItem.boCheckVar := boCheckVar;
            MonItem.nInheritedVarType := nInheritedVarType;
            MonItem.boCheckVarUseOR := boCheckVarUseOR;
            if (TriggerScrpit = '') or (TriggerScrpit[1] <> '@') then
              MonItem.TriggerScrpit := ''
            else
              MonItem.TriggerScrpit := TriggerScrpit;
            Move(CheckVarArr[0], MonItem.CheckVarArr[0], SizeOf(CheckVarArr));
            AddList.Add(MonItem);
            ArrayList.Add(AddList);
            AddList := MonItem.List;
            Inc(nLevel);
          end;
          boBegin := False;
        end;
      end
      else if CompareLStr(s28, '#CHILD', Length('#CHILD')) then
      begin
        sLine := s28;
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        sSelPoint := s30;
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        sMaxPoint := s30;
        s28 := GetValidStr3(s28, s30, [' ', #9]);
        boRandom := CompareText(s30, 'RANDOM') = 0;
        nSelPoint := StrToIntDef(sSelPoint, -1);
        nMaxPoint := StrToIntDef(sMaxPoint, -1);
        boSelPointIsVar := CheckStrIsVar(sSelPoint);
        boMaxPointIsVar := CheckStrIsVar(sMaxPoint);
        if (not boSelPointIsVar) and (nSelPoint < 0) then
        begin
          MainOutMessage('MonItems file error, ' + s24 + '; ' + sLine);
          Break;
        end
        else
        begin
          if (not boMaxPointIsVar) and (nMaxPoint < 0) then
          begin
            MainOutMessage('MonItems file error, ' + s24 + '; ' + sLine);
            Break;
          end;
        end;
        if SameText(s30, 'RANDOM') then
        begin
          boRandom := True;
          s30 := s28; // 后面一个是条件判断
        end;
        boCheckVar := False;
        nInheritedVarType := 0;
        boCheckVarUseOR := False;
        TriggerScrpit := '';
        Var1Name := '-';
        Var1Index := 0;
        Var2Name := '-';
        Var2Index := 0;
        for I := Low(CheckVarArr) to High(CheckVarArr) do
        begin
          CheckVarArr[I].IsUse := False;
          CheckVarArr[I].Var1Name := '-';
          CheckVarArr[I].Var1Index := 0;
          CheckVarArr[I].CompareType := ctFail;
          CheckVarArr[I].Var2Name := '-';
          CheckVarArr[I].Var2Index := 0;
        end;
        if (Length(s30) > 0) and (s30[1] = '[') and (s30[Length(s30)] = ']') then
        begin
          s28 := Copy(s30, 2, Length(s30) - 2);
          TempN := pos(',', s28);
          if TempN > 0 then
          begin
            sTemp2 := Copy(s28, TempN + 1, MaxInt);
            sTemp2 := GetValidStr3(sTemp2, sTemp1, [';', ',']);
            nInheritedVarType := StrToIntDef(sTemp1, 0);
            TriggerScrpit := sTemp2;
            s28 := Copy(s28, 1, TempN - 1);
          end;
          TempN := pos('|', s28);
          if TempN > 0 then
          begin
            sTemp1 := Copy(s28, TempN + 1, MaxInt);
            boCheckVarUseOR := SameText(sTemp1, 'OR');
            s28 := Copy(s28, 1, TempN - 1);
          end;
          CheckVarArrIndex := 0;
          TempStrings := TStringList.Create;
          try
            TempStrings.Delimiter := ';';
            TempStrings.DelimitedText := s28;
            for I := 0 to TempStrings.Count - 1 do
            begin
              s28 := TempStrings[I];
              TempN := 0;
              for II := 1 to Length(s28) do
              begin
{$IF CompilerVersion >= 22}
                if CharInSet(s28[II], ['<', '=', '>']) then
{$ELSE}
                if s28[II] in ['<', '=', '>'] then
{$IFEND}
                begin
                  TempN := II;
                  Break;
                end;
              end;
              if (TempN = 0) or (TempN >= Length(s28)) then
              begin
                MainOutMessage('MonItems file error, ' + s24 + '; ' + s30);
                boCheckVar := False;
                Break;
              end
              else
              begin
                sTemp1 := s28[TempN];
{$IF CompilerVersion >= 22}
                if CharInSet(s28[TempN + 1], ['=']) then
                  sTemp1 := sTemp1 + '='
                else if CharInSet(s28[TempN + 1], ['>']) then
                  sTemp1 := sTemp1 + '>';
{$ELSE}
                if (s28[TempN + 1] in ['=']) then
                  sTemp1 := sTemp1 + '='
                else if (s28[TempN + 1] in ['>']) then
                  sTemp1 := sTemp1 + '>';
{$IFEND}
                if sTemp1 = '<' then
                  CompareType := ctLess
                else if sTemp1 = '=' then
                  CompareType := ctEqual
                else if sTemp1 = '>' then
                  CompareType := ctGreater
                else if sTemp1 = '<=' then
                  CompareType := ctLessEqual
                else if sTemp1 = '>=' then
                  CompareType := ctGreaterEqual
                else if sTemp1 = '<>' then
                  CompareType := ctNotEqual
                else
                  CompareType := ctFail;
                sTemp2 := Copy(s28, TempN + Length(sTemp1), MaxInt); // 取得比较值
                sTemp1 := Copy(s28, 1, TempN - 1); // 取得变量名
                sTemp1 := Trim(sTemp1);
                sTemp2 := Trim(sTemp2);
                IsOK := True;
                if (CompareType = ctFail) or (Length(sTemp1) = 0) or (Length(sTemp2) = 0) then
                  IsOK := False;
                if IsOK then
                begin
{$IF CompilerVersion >= 22}
                  if CharInSet(sTemp1[1], ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$ELSE}
                  if (sTemp1[1] in ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$IFEND}
                  begin
                    Var1Name := UpCase(sTemp1[1]);
                    sTemp1 := Copy(sTemp1, 2, MaxInt);
                    if not TryStrToInt(sTemp1, Var1Index) then
                      IsOK := False
                    else
                    begin
                      if (Var1Name = 'D') then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                      else if Var1Name = 'M' then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                      else if Var1Name = 'N' then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                      else if Var1Name = 'U' then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 499)
                      else if Var1Name = 'J' then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 499)
                      else if (Var1Name = 'I') then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                      else if Var1Name = 'G' then
                        IsOK := (Var1Index >= 0) and (Var1Index <= 999);
                    end;
                  end
                  else
                  begin
                    Var1Name := '-';
                    IsOK := TryStrToInt(sTemp1, Var1Index);
                  end;
                end;
                if IsOK then
                begin
{$IF CompilerVersion >= 22}
                  if CharInSet(sTemp2[1], ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$ELSE}
                  if (sTemp2[1] in ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$IFEND}
                  begin
                    Var2Name := UpCase(sTemp2[1]);
                    sTemp2 := Copy(sTemp2, 2, MaxInt);
                    if not TryStrToInt(sTemp2, Var2Index) then
                      IsOK := False
                    else
                    begin
                      if (Var2Name = 'D') then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                      else if Var2Name = 'M' then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                      else if Var2Name = 'N' then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                      else if Var2Name = 'U' then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 499)
                      else if Var2Name = 'J' then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 499)
                      else if (Var2Name = 'I') then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                      else if Var2Name = 'G' then
                        IsOK := (Var2Index >= 0) and (Var2Index <= 999);
                    end;
                  end
                  else
                  begin
                    Var2Name := '-';
                    IsOK := TryStrToInt(sTemp2, Var2Index);
                  end;
                end;
                boCheckVar := IsOK;
                if not IsOK then
                begin
                  MainOutMessage('MonItems file error, ' + s24 + '; ' + s30);
                  Break;
                end
                else
                begin
                  CheckVarArr[CheckVarArrIndex].IsUse := True;
                  CheckVarArr[CheckVarArrIndex].Var1Name := Var1Name;
                  CheckVarArr[CheckVarArrIndex].Var1Index := Var1Index;
                  CheckVarArr[CheckVarArrIndex].CompareType := CompareType;
                  CheckVarArr[CheckVarArrIndex].Var2Name := Var2Name;
                  CheckVarArr[CheckVarArrIndex].Var2Index := Var2Index;
                  Inc(CheckVarArrIndex);
                  if CheckVarArrIndex >= Length(CheckVarArr) then
                    Break;
                end;
              end;
            end;
          finally
            TempStrings.Free;
          end;
        end;
        boBegin := True;
      end
      else
      begin
        sLine := s28;
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        sSelPoint := s30;
        s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
        sMaxPoint := s30;
        s28 := GetValidStr3(s28, s30, [' ', #9]);
        if s30 <> '' then
        begin
          if s30[1] = '"' then
            ArrestStringEx(s30, '"', '"', s30);
        end;
        s2C := s30;
        s28 := GetValidStr3(s28, s30, [' ', #9]);
        n20 := StrToIntDef(s30, 1);
        nSelPoint := StrToIntDef(sSelPoint, -1);
        nMaxPoint := StrToIntDef(sMaxPoint, -1);
        boSelPointIsVar := CheckStrIsVar(sSelPoint);
        boMaxPointIsVar := CheckStrIsVar(sMaxPoint);
        if (sSelPoint <> '') and (sSelPoint <> '') and (s2C <> '') then
        begin
          if (not boSelPointIsVar) and (nSelPoint < 0) then
          begin
            MainOutMessage('MonItems file error, ' + s24 + '; ' + sLine);
            Break;
          end
          else
          begin
            if (not boMaxPointIsVar) and (nMaxPoint < 0) then
            begin
              MainOutMessage('MonItems file error, ' + s24 + '; ' + sLine);
              Break;
            end;
          end;
          New(MonItem);
          FillChar(MonItem^, SizeOf(TMonItemInfo), 0);
          MonItem.SelPoint.nValue := nSelPoint; // - 1;
          MonItem.SelPoint.IsUseVar := boSelPointIsVar;
          MonItem.SelPoint.VarName := sSelPoint;
          MonItem.MaxPoint.nValue := nMaxPoint;
          MonItem.MaxPoint.IsUseVar := boMaxPointIsVar;
          MonItem.MaxPoint.VarName := sMaxPoint;
          MonItem.ItemName := s2C;
          if n20 <= 0 then
          begin
            MonItem.Count := 1 + Random(Abs(n20));
          end
          else
          begin
            MonItem.Count := n20;
          end;
          MonItem.List := nil;
          MonItem.boRandom := False;
          MonItem.boGold := SameText(MonItem.ItemName, sSTRING_GOLDNAME);
          AddList.Add(MonItem);
          Inc(Result);
        end;
      end;
      Inc(nLine);
      if nLine >= LoadList.Count then
        Break;
    end;
  finally
    LoadList.Free;
    ArrayList.Free;
  end;
end;

function TFrmDB.LoadMonitems_Ex(FileName, CallLabel: string; var ItemList: TList): Integer;

  function LoadCallScript(sFileName, sLabel: string; var List: TStringList; Index: Integer): Boolean;
  var
    I, InserIndex: Integer;
    LoadStrList: TStringList;
    bo1D: Boolean;
    s18: string;
  begin
    Result := False;
    if FileExists(sFileName) then
    begin
      LoadStrList := TStringList.Create;
      LoadStrList.LoadFromFile(sFileName);
      DeCodeStringList(LoadStrList);
      sLabel := '[' + sLabel + ']';
      bo1D := False;
      InserIndex := Index + 1;
      for I := 0 to LoadStrList.Count - 1 do
      begin
        s18 := Trim(LoadStrList.Strings[I]);
        if s18 <> '' then
        begin
          if (s18[1] = '{') and (Length(s18) = 1) then
          begin
            Continue;
          end
          else
          begin
            if not bo1D then
            begin
              if (s18[1] = '[') and (CompareText(s18, sLabel) = 0) then
              begin
                bo1D := True;
                // [@区段] 不用插件
                // List.Insert(InserIndex, s18);
                // Inc(InserIndex);
              end
              else
              begin
              end;
            end
            else
            begin
              if (s18[1] = '}') then
              begin
                Result := True;
                Break;
              end
              else
              begin
                List.Insert(InserIndex, s18);
                Inc(InserIndex);
              end;
            end;
          end;
        end;
      end;
      LoadStrList.Free;
    end;
  end;

  function LoadCallScript2(LoadStrList: TStringList; sLabel: string; var List: TStringList; Index: Integer): Boolean;
  var
    I, InserIndex: Integer;
    bo1D: Boolean;
    s18: string;
  begin
    Result := False;
    DeCodeStringList(LoadStrList);
    sLabel := '[' + sLabel + ']';
    bo1D := False;
    InserIndex := Index + 1;
    for I := 0 to LoadStrList.Count - 1 do
    begin
      s18 := Trim(LoadStrList.Strings[I]);
      if s18 <> '' then
      begin
        if (s18[1] = '{') and (Length(s18) = 1) then
        begin
          Continue;
        end
        else
        begin
          if not bo1D then
          begin
            if (s18[1] = '[') and (CompareText(s18, sLabel) = 0) then
            begin
              bo1D := True;
              // [@区段] 不用插件
              // List.Insert(InserIndex, s18);
              // Inc(InserIndex);
            end
            else
            begin
            end;
          end
          else
          begin
            if (s18[1] = '}') then
            begin
              Result := True;
              Break;
            end
            else
            begin
              List.Insert(InserIndex, s18);
              Inc(InserIndex);
            end;
          end;
        end;
      end;
    end;
  end;

var
  nLine: Integer;
  LoadList: TStringList;
  MonItem: pTMonItemInfo;
  sLine, s18, s20, s28, s2C, s30, s1C, s34: string;
  n20: Integer;
  sSelPoint, sMaxPoint: string;
  nSelPoint, nMaxPoint: Integer;
  boSelPointIsVar, boMaxPointIsVar: Boolean;
  nLevel: Integer;
  boBegin: Boolean;
  boRandom: Boolean;
  AddList: TList;
  ArrayList: TList;
  MemoryStream: TMemoryStream;
  TempStrings: TStringList;
  I, II: Integer;
  boCheckVar, IsOK: Boolean;
  nInheritedVarType: Byte;
  boCheckVarUseOR: Boolean;
  Var1Name, Var2Name: Char;
  Var1Index, Var2Index, TempN: Integer;
  CompareType: TCompareType;
  sTemp1, sTemp2: string;
  TriggerScrpit: string;
  CheckVarArr: TMonItemCheckVarArr;
  CheckVarArrIndex: Integer;
begin
  Result := 0;
  if ItemList <> nil then
  begin
    ClearMonItemList(ItemList);
    ItemList.Clear;
  end
  else
    ItemList := TList.Create;
  if Length(CallLabel) = 0 then
  begin
    if not FileExists(FileName) then
    begin
      // 怪物爆率支持远程脚本 chongchong 2016-11-25
      if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
      begin
        try
          MemoryStream := TMemoryStream.Create;
          if not g_PluginManager.HookLoadScriptFile(PAnsiChar(AnsiString(FileName)), MemoryStream) then
          begin
            MemoryStream.Free;
            Exit;
          end;
          LoadList := TStringList.Create;
          if (MemoryStream.Size > 0) then
          begin
            // SetLength(s30, MemoryStream.Size);
            // Move(MemoryStream.Memory^, s30[1], MemoryStream.Size);
            s30 := DeCodePlugBuffer(MemoryStream);
            LoadList.Text := s30;
          end;
          MemoryStream.Free;
        except
          on E: Exception do
          begin
            MainOutMessage('[Exception] HookLoadScriptFile MapInfo.txt');
            MainOutMessage(E.Message);
            Exit;
          end;
        end;
      end
      else
      begin
        Exit;
      end;
      DeCodeStringList(LoadList);
    end
    else
    begin
      LoadList := TStringList.Create;
      LoadList.LoadFromFile(FileName);
    end;
  end
  else
  begin
    LoadList := TStringList.Create;
  end;
  nLevel := 0;
  boBegin := False;
  boRandom := False;
  ArrayList := TList.Create;
  AddList := ItemList;
  sSelPoint := '';
  sMaxPoint := '';
  nSelPoint := -1;
  nMaxPoint := -1;
  boSelPointIsVar := False;
  boMaxPointIsVar := False;
  boCheckVar := False;
  boCheckVarUseOR := False;
  TriggerScrpit := '';
  nInheritedVarType := 0;
  Var1Index := 0;
  Var2Index := 0;
  for I := Low(CheckVarArr) to High(CheckVarArr) do
  begin
    CheckVarArr[I].IsUse := False;
    CheckVarArr[I].Var1Name := '-';
    CheckVarArr[I].Var1Index := 0;
    CheckVarArr[I].CompareType := ctFail;
    CheckVarArr[I].Var2Name := '-';
    CheckVarArr[I].Var2Index := 0;
  end;
  if Length(CallLabel) > 0 then
  begin
    if LoadCallScript(FileName, CallLabel, LoadList, -1) then

  end;
  nLine := 0;
  while True do
  begin
    if nLine >= LoadList.Count then
      Break;
    s28 := LoadList.Strings[nLine];
    sLine := s28;
    if (s28 = '') or (s28[1] = ';') then
    begin
      Inc(nLine);
      if nLine >= LoadList.Count then
        Break
      else
        Continue;
    end;
    s28 := Trim(s28);
    if (s28 <> '') and (s28[1] = '#') and (CompareLStr(s28, '#CALL', Length('#CALL'))) then
    begin
      s28 := ArrestStringEx(s28, '[', ']', s1C);
      s20 := Trim(s1C);
      s18 := Trim(s28);
      if (Length(s20) > 0) and (s20[1] = '\') then
        s20 := Copy(s20, 2, Length(s20) - 1);
      if (Length(s20) > 0) and (s20[1] = '\') then
        s20 := Copy(s20, 2, Length(s20) - 1);
      s34 := g_Config.sEnvirDir + 'QuestDiary\' + s20;
      // 支持远程脚本 2019-05-14 17:58:43
      if not FileExists(s34) then
      begin
        if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
        begin
          try
            s34 := ReplaceChar('QuestDiary\' + s20, '/', '\');
            MemoryStream := TMemoryStream.Create;
            if not g_PluginManager.HookLoadScriptFile(PAnsiChar(AnsiString(s34)), MemoryStream) then
            begin
              MemoryStream.Free;
              Exit;
            end;
            TempStrings := TStringList.Create;
            try
              if (MemoryStream.Size > 0) then
              begin
                TempStrings.Text := DeCodePlugBuffer(MemoryStream);
                if LoadCallScript2(TempStrings, s18, LoadList, nLine) then
                begin
                  LoadList.Strings[nLine] := ';' + sLine;
                end;
              end;
            finally
              TempStrings.Free;
            end;
            MemoryStream.Free;
          except
            on E: Exception do
            begin
              MainOutMessage('[Exception] HookLoadScriptFile');
              MainOutMessage(E.Message);
              Exit;
            end;
          end;
        end
        else
        begin
          Result := 1;
          MainOutMessage('脚本文件未找到: ' + s34);
          Exit;
        end;
      end
      else if LoadCallScript(s34, s18, LoadList, nLine) then
      begin
        LoadList.Strings[nLine] := ';' + sLine;
      end
      else
      begin
        MainOutMessage('脚本读取失败：' + s20 + s18);
      end;
    end
    else if (s28 = ')') or (s28 = '）') then // 支持中文括号 2020-08-15 20:02:54
    begin
      if nLevel > 0 then
      begin
        Dec(nLevel);
        AddList := TList(ArrayList[nLevel]);
        ArrayList.Delete(nLevel);
      end;
    end
    else if boBegin then
    begin
      if (s28 = '(') or (s28 = '（') then // 支持中文括号 2020-08-15 20:02:54
      begin
        if (sSelPoint <> '') and (sMaxPoint <> '') then
        begin
          New(MonItem);
          MonItem.SelPoint.nValue := nSelPoint; // - 1;
          MonItem.SelPoint.IsUseVar := boSelPointIsVar;
          MonItem.SelPoint.VarName := sSelPoint;
          MonItem.MaxPoint.nValue := nMaxPoint;
          MonItem.MaxPoint.IsUseVar := boMaxPointIsVar;
          MonItem.MaxPoint.VarName := sMaxPoint;
          MonItem.boGold := False;
          MonItem.List := TList.Create;
          MonItem.boRandom := boRandom;
          MonItem.Count := 0;
          MonItem.boCheckVar := boCheckVar;
          MonItem.nInheritedVarType := nInheritedVarType;
          MonItem.boCheckVarUseOR := boCheckVarUseOR;
          if (TriggerScrpit = '') or (TriggerScrpit[1] <> '@') then
            MonItem.TriggerScrpit := ''
          else
            MonItem.TriggerScrpit := TriggerScrpit;
          Move(CheckVarArr[0], MonItem.CheckVarArr[0], SizeOf(CheckVarArr));
          AddList.Add(MonItem);
          ArrayList.Add(AddList);
          AddList := MonItem.List;
          Inc(nLevel);
        end;
        boBegin := False;
      end;
    end
    else if CompareLStr(s28, '#CHILD', Length('#CHILD')) then
    begin
      sLine := s28;
      s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
      s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
      sSelPoint := s30;
      s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
      sMaxPoint := s30;
      s28 := GetValidStr3(s28, s30, [' ', #9]);
      boRandom := CompareText(s30, 'RANDOM') = 0;
      if SameText(s30, 'RANDOM') then
      begin
        boRandom := True;
        s30 := s28; // 后面一个是条件判断
      end;
      nSelPoint := StrToIntDef(sSelPoint, -1);
      nMaxPoint := StrToIntDef(sMaxPoint, -1);
      boSelPointIsVar := CheckStrIsVar(sSelPoint);
      boMaxPointIsVar := CheckStrIsVar(sMaxPoint);
      if (not boSelPointIsVar) and (nSelPoint < 0) then
      begin
        MainOutMessage('MonItems file error, ' + s34 + '; ' + sLine);
        Break;
      end
      else
      begin
        if (not boMaxPointIsVar) and (nMaxPoint < 0) then
        begin
          MainOutMessage('MonItems file error, ' + s34 + '; ' + sLine);
          Break;
        end;
      end;
      boCheckVar := False;
      nInheritedVarType := 0;
      boCheckVarUseOR := False;
      TriggerScrpit := '';
      Var1Name := '-';
      Var1Index := 0;
      Var2Name := '-';
      Var2Index := 0;
      for I := Low(CheckVarArr) to High(CheckVarArr) do
      begin
        CheckVarArr[I].IsUse := False;
        CheckVarArr[I].Var1Name := '-';
        CheckVarArr[I].Var1Index := 0;
        CheckVarArr[I].CompareType := ctFail;
        CheckVarArr[I].Var2Name := '-';
        CheckVarArr[I].Var2Index := 0;
      end;
      if (Length(s30) > 0) and (s30[1] = '[') and (s30[Length(s30)] = ']') then
      begin
        s28 := Copy(s30, 2, Length(s30) - 2);
        TempN := pos(',', s28);
        if TempN > 0 then
        begin
          sTemp2 := Copy(s28, TempN + 1, MaxInt);
          sTemp2 := GetValidStr3(sTemp2, sTemp1, [';', ',']);
          nInheritedVarType := StrToIntDef(sTemp1, 0);
          TriggerScrpit := sTemp2;
          s28 := Copy(s28, 1, TempN - 1);
        end;
        TempN := pos('|', s28);
        if TempN > 0 then
        begin
          sTemp1 := Copy(s28, TempN + 1, MaxInt);
          boCheckVarUseOR := SameText(sTemp1, 'OR');
          s28 := Copy(s28, 1, TempN - 1);
        end;
        CheckVarArrIndex := 0;
        TempStrings := TStringList.Create;
        try
          TempStrings.Delimiter := ';';
          TempStrings.DelimitedText := s28;
          for I := 0 to TempStrings.Count - 1 do
          begin
            s28 := TempStrings[I];
            TempN := 0;
            for II := 1 to Length(s28) do
            begin
{$IF CompilerVersion >= 22}
              if CharInSet(s28[II], ['<', '=', '>']) then
{$ELSE}
              if s28[II] in ['<', '=', '>'] then
{$IFEND}
              begin
                TempN := II;
                Break;
              end;
            end;
            if (TempN = 0) or (TempN >= Length(s28)) then
            begin
              MainOutMessage('MonItems file error, ' + s34 + ';' + s30);
              boCheckVar := False;
              Break;
            end
            else
            begin
              sTemp1 := s28[TempN];
{$IF CompilerVersion >= 22}
              if CharInSet(s28[TempN + 1], ['=']) then
                sTemp1 := sTemp1 + '='
              else if CharInSet(s28[TempN + 1], ['>']) then
                sTemp1 := sTemp1 + '>';
{$ELSE}
              if (s28[TempN + 1] in ['=']) then
                sTemp1 := sTemp1 + '='
              else if (s28[TempN + 1] in ['>']) then
                sTemp1 := sTemp1 + '>';
{$IFEND}
              if sTemp1 = '<' then
                CompareType := ctLess
              else if sTemp1 = '=' then
                CompareType := ctEqual
              else if sTemp1 = '>' then
                CompareType := ctGreater
              else if sTemp1 = '<=' then
                CompareType := ctLessEqual
              else if sTemp1 = '>=' then
                CompareType := ctGreaterEqual
              else if sTemp1 = '<>' then
                CompareType := ctNotEqual
              else
                CompareType := ctFail;
              sTemp2 := Copy(s28, TempN + Length(sTemp1), MaxInt); // 取得比较值
              sTemp1 := Copy(s28, 1, TempN - 1); // 取得变量名
              sTemp1 := Trim(sTemp1);
              sTemp2 := Trim(sTemp2);
              IsOK := True;
              if (CompareType = ctFail) or (Length(sTemp1) = 0) or (Length(sTemp2) = 0) then
                IsOK := False;
              if IsOK then
              begin
{$IF CompilerVersion >= 22}
                if CharInSet(sTemp1[1], ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$ELSE}
                if (sTemp1[1] in ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$IFEND}
                begin
                  Var1Name := UpCase(sTemp1[1]);
                  sTemp1 := Copy(sTemp1, 2, MaxInt);
                  if not TryStrToInt(sTemp1, Var1Index) then
                    IsOK := False
                  else
                  begin
                    if (Var1Name = 'D') then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                    else if Var1Name = 'M' then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                    else if Var1Name = 'N' then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                    else if Var1Name = 'U' then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 499)
                    else if Var1Name = 'J' then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 499)
                    else if (Var1Name = 'I') then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 999)
                    else if Var1Name = 'G' then
                      IsOK := (Var1Index >= 0) and (Var1Index <= 999);
                  end;
                end
                else
                begin
                  Var1Name := '-';
                  IsOK := TryStrToInt(sTemp1, Var1Index);
                end;
              end;
              if IsOK then
              begin
{$IF CompilerVersion >= 22}
                if CharInSet(sTemp2[1], ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$ELSE}
                if (sTemp2[1] in ['D', 'M', 'N', 'U', 'J', 'I', 'G', 'd', 'm', 'n', 'u', 'j', 'i', 'g']) then
{$IFEND}
                begin
                  Var2Name := UpCase(sTemp2[1]);
                  sTemp2 := Copy(sTemp2, 2, MaxInt);
                  if not TryStrToInt(sTemp2, Var2Index) then
                    IsOK := False
                  else
                  begin
                    if (Var2Name = 'D') then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                    else if Var2Name = 'M' then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                    else if Var2Name = 'N' then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                    else if Var2Name = 'U' then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 499)
                    else if Var2Name = 'J' then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 499)
                    else if (Var2Name = 'I') then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 999)
                    else if Var2Name = 'G' then
                      IsOK := (Var2Index >= 0) and (Var2Index <= 999);
                  end;
                end
                else
                begin
                  Var2Name := '-';
                  IsOK := TryStrToInt(sTemp2, Var2Index);
                end;
              end;
              boCheckVar := IsOK;
              if not IsOK then
              begin
                MainOutMessage('MonItems file error, ' + s34 + ';' + s30);
                Break;
              end
              else
              begin
                CheckVarArr[CheckVarArrIndex].IsUse := True;
                CheckVarArr[CheckVarArrIndex].Var1Name := Var1Name;
                CheckVarArr[CheckVarArrIndex].Var1Index := Var1Index;
                CheckVarArr[CheckVarArrIndex].CompareType := CompareType;
                CheckVarArr[CheckVarArrIndex].Var2Name := Var2Name;
                CheckVarArr[CheckVarArrIndex].Var2Index := Var2Index;
                Inc(CheckVarArrIndex);
                if CheckVarArrIndex >= Length(CheckVarArr) then
                  Break;
              end;
            end;
          end;
        finally
          TempStrings.Free;
        end;
      end;
      boBegin := True;
    end
    else
    begin
      sLine := s28;
      s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
      sSelPoint := s30;
      s28 := GetValidStr3(s28, s30, [' ', '/', #9]);
      sMaxPoint := s30;
      s28 := GetValidStr3(s28, s30, [' ', #9]);
      if s30 <> '' then
      begin
        if s30[1] = '"' then
          ArrestStringEx(s30, '"', '"', s30);
      end;
      s2C := s30;
      s28 := GetValidStr3(s28, s30, [' ', #9]);
      n20 := StrToIntDef(s30, 1);
      nSelPoint := StrToIntDef(sSelPoint, -1);
      nMaxPoint := StrToIntDef(sMaxPoint, -1);
      boSelPointIsVar := CheckStrIsVar(sSelPoint);
      boMaxPointIsVar := CheckStrIsVar(sMaxPoint);
      if (sSelPoint <> '') and (sMaxPoint <> '') and (s2C <> '') then
      begin
        if (not boSelPointIsVar) and (nSelPoint < 0) then
        begin
          MainOutMessage('MonItems file error, ' + s34 + '; ' + sLine);
          Break;
        end
        else
        begin
          if (not boMaxPointIsVar) and (nMaxPoint < 0) then
          begin
            MainOutMessage('MonItems file error, ' + s34 + '; ' + sLine);
            Break;
          end;
        end;
        New(MonItem);
        FillChar(MonItem^, SizeOf(TMonItemInfo), 0);
        MonItem.SelPoint.nValue := nSelPoint; // - 1;
        MonItem.SelPoint.IsUseVar := boSelPointIsVar;
        MonItem.SelPoint.VarName := sSelPoint;
        MonItem.MaxPoint.nValue := nMaxPoint;
        MonItem.MaxPoint.IsUseVar := boMaxPointIsVar;
        MonItem.MaxPoint.VarName := sMaxPoint;
        MonItem.ItemName := s2C;
        if n20 <= 0 then
        begin
          MonItem.Count := 1 + Random(Abs(n20));
        end
        else
        begin
          MonItem.Count := n20;
        end;
        MonItem.List := nil;
        MonItem.boRandom := False;
        MonItem.boGold := SameText(MonItem.ItemName, sSTRING_GOLDNAME);
        AddList.Add(MonItem);
        Inc(Result);
      end;
    end;
    Inc(nLine);
    if nLine >= LoadList.Count then
      Break;
  end;
  LoadList.Free;
  ArrayList.Free;
end;

function TFrmDB.LoadNpcs(): Integer;
var
  sFileName, s18, s20, s24, s28, s2C, s30, s34, s38, s40, s42: string;
  LoadList: TStringList;
  NPC: TNormNpc;
  I: Integer;
begin
  sFileName := g_Config.sEnvirDir + 'Npcs.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      s18 := Trim(LoadList.Strings[I]);
      if (s18 <> '') and (s18[1] <> ';') then
      begin
        s18 := GetValidStrCap(s18, s20, [' ', #9]);
        if (s20 <> '') and (s20[1] = '"') then
          ArrestStringEx(s20, '"', '"', s20);
        s18 := GetValidStr3(s18, s24, [' ', #9]);
        s18 := GetValidStr3(s18, s28, [' ', #9]);
        s18 := GetValidStr3(s18, s2C, [' ', #9]);
        s18 := GetValidStr3(s18, s30, [' ', #9]);
        s18 := GetValidStr3(s18, s34, [' ', #9]);
        s18 := GetValidStr3(s18, s38, [' ', #9]);
        s18 := GetValidStr3(s18, s40, [' ', #9]);
        s18 := GetValidStr3(s18, s42, [' ', #9]);
        if (s20 <> '') and (s28 <> '') and (s38 <> '') then
        begin
          NPC := nil;
          case StrToIntDef(s24, 0) of
            0:
              NPC := TMerchant.Create;
            1:
              NPC := TGuildOfficial.Create;
            2:
              NPC := TCastleOfficial.Create;
          end;

          if NPC <> nil then
          begin
            NPC.m_sMapName := s28;
            NPC.m_nCurrX := StrToIntDef(s2C, 0);
            NPC.m_nCurrY := StrToIntDef(s30, 0);
            NPC.m_sCharName := s20;
            NPC.m_nFlag := StrToIntDef(s34, 0);
            NPC.m_wAppr := StrToIntDef(s38, 0);
            if StrToIntDef(s40, 0) <> 0 then
              NPC.m_boNpcAutoChangeColor := True;

            NPC.m_dwNpcAutoChangeColorTime := StrToIntDef(s42, 0) * 1000;
            UserEngine.AddNpc(s20, NPC);
          end;
        end;
      end;
    end;
    LoadList.Free;
  end;
  Result := 1;
end;

function TFrmDB.LoadQuestDiary(): Integer;

  function sub_48978C(nIndex: Integer): string;
  begin
    if nIndex >= 1000 then
    begin
      Result := IntToStr(nIndex);
      Exit;
    end;
    if nIndex >= 100 then
    begin
      Result := IntToStr(nIndex) + '0';
      Exit;
    end;
    Result := IntToStr(nIndex) + '00';
  end;

var
  I, II: Integer;
  QDDinfoList: TList;
  QDDinfo: pTQDDinfo;
  s14, s18, s1C, s20: string;
  bo2D: Boolean;
  nC: Integer;
  LoadList: TStringList;
begin
  Result := 1;
  for I := 0 to QuestDiaryList.Count - 1 do
  begin
    QDDinfoList := QuestDiaryList.Items[I];
    for II := 0 to QDDinfoList.Count - 1 do
    begin
      QDDinfo := QDDinfoList.Items[II];
      QDDinfo.sList.Free;
      DisPose(QDDinfo);
    end;
    QDDinfoList.Free;
  end;
  QuestDiaryList.Clear;
  bo2D := False;
  nC := 1;
  while (True) do
  begin
    QDDinfoList := nil;
    s14 := 'QuestDiary\' + sub_48978C(nC) + '.txt';
    if FileExists(s14) then
    begin
      s18 := '';
      QDDinfo := nil;
      LoadList := TStringList.Create;
      LoadList.LoadFromFile(s14);
      DeCodeStringList(LoadList);
      for I := 0 to LoadList.Count - 1 do
      begin
        s1C := LoadList.Strings[I];
        if (s1C <> '') and (s1C[1] <> ';') then
        begin
          if (s1C[1] = '[') and (Length(s1C) > 2) then
          begin
            if s18 = '' then
            begin
              ArrestStringEx(s1C, '[', ']', s18);
              QDDinfoList := TList.Create;
              New(QDDinfo);
              QDDinfo.n00 := nC;
              QDDinfo.s04 := s18;
              QDDinfo.sList := TStringList.Create;
              QDDinfoList.Add(QDDinfo);
              bo2D := True;
            end
            else
            begin
              if s1C[1] <> '@' then
              begin
                s1C := GetValidStr3(s1C, s20, [' ', #9]);
                ArrestStringEx(s20, '[', ']', s20);
                New(QDDinfo);
                QDDinfo.n00 := StrToIntDef(s20, 0);
                QDDinfo.s04 := s1C;
                QDDinfo.sList := TStringList.Create;
                QDDinfoList.Add(QDDinfo);
                bo2D := True;
              end
              else
                bo2D := False;
            end;
          end
          else
          begin
            if bo2D then
              QDDinfo.sList.Add(s1C);
          end;
        end;
      end;
      LoadList.Free;
    end
    else
    begin
    end;
    if QDDinfoList <> nil then
      QuestDiaryList.Add(QDDinfoList)
    else
      QuestDiaryList.Add(nil);
    Inc(nC);
    if nC >= 105 then
      Break;
  end;
end;

function TFrmDB.LoadStartPoint(): Integer;
var
  sFileName, tStr, S1, S2, S3, S4, S5, S6, S7, S8: string;
  LoadList: TStringList;
  I, nRange, nIndex, nX1, nX2, nY1, nY2, nTemp, II, nChange: Integer;
  RangeArea: TRangeSafeArea;
  AllotypeArea: TAllotypeSafeArea;
begin
  Result := 0;
  sFileName := g_Config.sEnvirDir + 'StartPoint.txt';
  if FileExists(sFileName) then
  begin
    g_SafeAreaManager.Clear;
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      tStr := Trim(LoadList.Strings[I]);
      if (tStr <> '') and (tStr[1] <> ';') then
      begin
        tStr := GetValidStr3(tStr, S1, [' ', #9]);
        tStr := GetValidStr3(tStr, S2, [' ', #9]);
        tStr := GetValidStr3(tStr, S3, [' ', #9]);
        tStr := GetValidStr3(tStr, S4, [' ', #9]);
        tStr := GetValidStr3(tStr, S5, [' ', #9]);
        tStr := GetValidStr3(tStr, S6, [' ', #9]);
        tStr := GetValidStr3(tStr, S7, [' ', #9]);
        tStr := GetValidStr3(tStr, S8, [' ', #9]);
        if (S1 <> '') and (S2 <> '') and (S3 <> '') then
        begin
          nRange := StrToIntDef(S5, 0);
          if (nRange > 0) then
          begin
            RangeArea := TRangeSafeArea.Create;
            RangeArea.MapName := S1;
            RangeArea.CenterX := StrToIntDef(S2, 0);
            RangeArea.CenterY := StrToIntDef(S3, 0);
            RangeArea.IsDisableSay := Boolean(StrToIntDef(S4, 0));
            RangeArea.Range := nRange;
            RangeArea.ShowType := StrToIntDef(S6, 0);
            RangeArea.IsPKZone := StrToIntDef(S7, 0) <> 0;
            RangeArea.IsPKFire := StrToIntDef(S8, 0) <> 0;
            g_SafeAreaManager.Add(RangeArea);
          end
          else if nRange < 0 then
          begin
            nIndex := pos(':', S2);
            if nIndex > 0 then
            begin
              nX1 := StrToIntDef(Copy(S2, 1, nIndex - 1), 0);
              nY1 := StrToIntDef(Copy(S2, nIndex + 1, MaxInt), 0);
              nIndex := pos(':', S3);
              if nIndex > 0 then
              begin
                nX2 := StrToIntDef(Copy(S3, 1, nIndex - 1), 0);
                nY2 := StrToIntDef(Copy(S3, nIndex + 1, MaxInt), 0);
                if not((Abs(nX1 - nX2) = 0) or (Abs(nY1 - nY2) = 0) or (Abs(nX1 - nX2) = Abs(nY1 - nY2))) then
                begin
                  MainOutMessage('安全区配置错误，无法构造线：' + Trim(LoadList.Strings[I]));
                  Continue;
                end
                else
                begin
                  AllotypeArea := g_SafeAreaManager.GetAllotypeSafeArea(S1, nRange);
                  if AllotypeArea = nil then
                  begin
                    AllotypeArea := TAllotypeSafeArea.Create;
                    AllotypeArea.MapName := S1;
                    AllotypeArea.ID := nRange;
                    g_SafeAreaManager.Add(AllotypeArea);
                  end;
                  if nX1 <> nX2 then
                  begin
                    if nX2 < nX1 then
                    begin
                      nTemp := nX1;
                      nX1 := nX2;
                      nX2 := nTemp;
                      nTemp := nY1;
                      nY1 := nY2;
                      nY2 := nTemp;
                    end;
                    if nY1 < nY2 then
                      nChange := 1
                    else if nY1 = nY2 then
                      nChange := 0
                    else
                      nChange := -1;
                    for II := nX1 to nX2 do
                    begin
                      AllotypeArea.Add(II, nY1, StrToIntDef(S6, 0), 0);
                      nY1 := nY1 + nChange;
                    end;
                  end
                  else
                  begin
                    // 修正自定义安全区绘制 ,总有一边无法绘制 +而且只有特效上才算安全区的问题 2021-01-14
                    if nY2 < nY1 then
                    begin
                      nTemp := nY1;
                      nY1 := nY2;
                      nY2 := nTemp;
                      nTemp := nX1;
                      nX1 := nX2;
                      nX2 := nTemp;
                    end;
                    if nX1 < nX2 then
                      nChange := 1
                    else if nX1 = nX2 then
                      nChange := 0
                    else
                      nChange := -1;
                    for II := nY1 to nY2 do
                    begin
                      AllotypeArea.Add(nX1, II, StrToIntDef(S6, 0), 0);
                      nX1 := nX1 + nChange;
                    end;
                  end;
                end;
              end
              else
              begin
                MainOutMessage('安全区配置错误，坐标配置错误：' + Trim(LoadList.Strings[I]));
                Continue;
              end;
            end
            else
            begin
              AllotypeArea := g_SafeAreaManager.GetAllotypeSafeArea(S1, nRange);
              if AllotypeArea = nil then
              begin
                AllotypeArea := TAllotypeSafeArea.Create;
                AllotypeArea.MapName := S1;
                AllotypeArea.ID := nRange;
                g_SafeAreaManager.Add(AllotypeArea);
              end;
              AllotypeArea.Add(StrToIntDef(S2, 0), StrToIntDef(S3, 0), StrToIntDef(S6, 0), 0);
            end;
          end;
          Result := 1;
        end;
      end;
    end;
    g_SafeAreaManager.Recall;
    LoadList.Free;
  end;
end;

function TFrmDB.LoadUnbindList(): Integer;
var
  sFileName, tStr, sData, s20: string;
  LoadList: TStringList;
  I: Integer;
  n10: Integer;
  UnbindItemInfo: PTUnbindItemInfo;
begin
  Result := 0;
  sFileName := g_Config.sEnvirDir + 'UnbindList.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      tStr := LoadList.Strings[I];
      if (tStr <> '') and (tStr[1] <> ';') then
      begin
        // New(tUnbind);
        tStr := GetValidStr3(tStr, sData, [' ', #9]);
        tStr := GetValidStrCap(tStr, s20, [' ', #9]);
        if (s20 <> '') and (s20[1] = '"') then
          ArrestStringEx(s20, '"', '"', s20);
        GetValidStrCap(tStr, tStr, [' ', #9]);
        n10 := StrToIntDef(sData, 0);
        if n10 > 0 then
        begin
          New(UnbindItemInfo);
          UnbindItemInfo.sItemName := s20;
          UnbindItemInfo.nShape := n10;
          UnbindItemInfo.nCount := StrToIntDef(tStr, 0);
          g_UnbindList.Add(UnbindItemInfo);
        end
        else
        begin
          Result := -I; // 需要取负数
          Break;
        end;
      end;
    end;
    LoadList.Free;
  end;
end;

function TFrmDB.LoadIconFile(NPC: TNormNpc; ActorIcons: pTActorIconArray; sPatch, sFileName: string): Integer;
var
  I, nIndex, nFileIndex, nIconIndex, nIconCount, nX, nY, nDrawOrder, nPlayTime: Integer;
  sLineText: string;
  sIconFileName: string;
  sFileIndex, sIconIndex, sIconCount, sX, sY, sBlend, sDrawOrder, sPlayTime: string;
  LoadList: TStringList;
  CRC: LongWord;
begin
  Result := -1;
  sIconFileName := g_Config.sEnvirDir + sPatch + sFileName + '.txt';
  if FileExists(sIconFileName) then
  begin
    // 修正当文件改变时才重新加载Npc 2020-05-17 23:47:02
    if NPC <> nil then
    begin
      CRC := GetScriptFileCRC(sIconFileName);
      if NPC.m_dwIconFileCRC = CRC then
        Exit;
      NPC.m_dwIconFileCRC := CRC;
    end;
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sIconFileName);
    except
      LoadList.Clear;
    end;
    FillChar(ActorIcons^, SizeOf(TActorIconArray), 0);
    for I := Low(TActorIconArray) to High(TActorIconArray) do
    begin
      ActorIcons[I].nFileIndex := -1;
      ActorIcons[I].nIconCount := 0;
    end;
    nIndex := 0;
    for I := 0 to LoadList.Count - 1 do
    begin
      if nIndex >= 9 then
        Break;
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText = '') or (sLineText[1] = ';') then
        Continue;
      sLineText := GetValidStrCap(sLineText, sFileIndex, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sIconIndex, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sIconCount, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sX, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sY, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sBlend, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sDrawOrder, [' ', #9]);
      sLineText := GetValidStrCap(sLineText, sPlayTime, [' ', #9]);
      nFileIndex := StrToIntDef(sFileIndex, -1);
      nIconIndex := StrToIntDef(sIconIndex, -1);
      nIconCount := StrToIntDef(sIconCount, 1);
      nX := StrToIntDef(sX, 0);
      nY := StrToIntDef(sY, 0);
      nDrawOrder := StrToIntDef(sDrawOrder, 0);
      nPlayTime := StrToIntDef(sPlayTime, 300);
      if (nFileIndex >= 0) and (nIconIndex >= 0) and (nIconCount >= 1) then
      begin
        ActorIcons[nIndex].nFileIndex := nFileIndex;
        ActorIcons[nIndex].nIconIndex := nIconIndex;
        ActorIcons[nIndex].nIconCount := nIconCount;
        ActorIcons[nIndex].nX := nX;
        ActorIcons[nIndex].nY := nY;
        ActorIcons[nIndex].boBlend := sBlend = '1';
        if nDrawOrder = 0 then
          ActorIcons[nIndex].btDrawOrder := 0
        else
          ActorIcons[nIndex].btDrawOrder := 1;
        ActorIcons[nIndex].nPlayTime := nPlayTime;
        Inc(nIndex);
        Inc(Result);
      end;
    end;
    LoadList.Free;
  end
  else
  begin
    FillChar(ActorIcons^, SizeOf(TActorIconArray), 0);
    for I := Low(TActorIconArray) to High(TActorIconArray) do
    begin
      ActorIcons[I].nFileIndex := -1;
      ActorIcons[I].nIconCount := 0;
    end;
  end;
end;

procedure TFrmDB.LoadMonFireDragonGuard(); // 创建火龙守护兽并写入列表 piaoyun 2013-08-19
var
  sFileName, s18, s20, s24, s28, s2C, s30, s34, s38: string;
  LoadList: TStringList;
  Monster: TBaseObject;
  I: Integer;
  nRace: Integer;
begin
  sFileName := g_Config.sEnvirDir + 'FireDragonGuard.txt';
  if not FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.Add(';名称       地图     x  y  dir(0-1)  攻击坐标x  攻击坐标Y(可以为多个以|分隔) ');
      LoadList.Add('火龙守护兽  D2083    51,67  1  77,50|74,50|83,50|80,53|86,53|74,41|71,41|71,44|71,47');
      LoadList.Add('火龙守护兽  D2083    48,70  1  81,44|81,47|78,44|75,44|84,47|78,41|75,41|81,50|84,50');
      LoadList.Add('火龙守护兽  D2083    45,73  1  81,44|84,47|78,41|85,50|75,41|76,51|79,54|73,48');
      LoadList.Add('火龙守护兽  D2083    61,78  0  79,48|79,51|76,48|78,53|75,50|76,45|82,51|81,44|84,47|78,41');
      LoadList.Add('火龙守护兽  D2083    58,81  0  79,48|82,51|85,54|76,45|71,40|82,48|79,45|76,42|85,51|73,42');
      LoadList.Add('火龙守护兽  D2083    55,84  0  80,48|77,48|80,45|77,51|74,51|74,54|71,54|71,57');
      LoadList.SaveToFile(sFileName);
    finally
      LoadList.Free;
    end;
  end;
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    try
      LoadList.LoadFromFile(sFileName);
      for I := 0 to LoadList.Count - 1 do
      begin
        s18 := Trim(LoadList.Strings[I]);
        if (s18 <> '') and (s18[1] <> ';') then
        begin
          s18 := GetValidStrCap(s18, s20, [' ', #9]); // 名字
          s18 := GetValidStr3(s18, s28, [' ', #9]); // 地图
          s18 := GetValidStr3(s18, s24, [' ', #9]); // 坐标
          s30 := GetValidStr3(s24, s2C, [',', #9]); // X,Y
          s18 := GetValidStr3(s18, s34, [' ', #9]); // 方向
          s18 := GetValidStr3(s18, s38, [' ', #9]); // 攻击坐标
          if (s20 <> '') and (s28 <> '') then
          begin
            // 修复数据库配置火龙守护兽错误导致引擎报错 chongchong 2013-11-24
            nRace := UserEngine.GetMonRace(s20);
            if nRace = 145 then
            begin
              Monster := UserEngine.RegenMonsterByName(s28, Str_ToInt(s2C, 0), Str_ToInt(s30, 0), s20);
              if Monster <> nil then
              begin
                Monster.m_btDirection := Str_ToInt(s34, 0); // 设置方向
                TFireDragonGuard(Monster).s_AttickXY := s38;
{$IF MULTI_THREAD = 1}
                if g_MultiThreadRun then
                  UserEngine.m_MonObjectList.LockW(1);
                try
{$IFEND}
                  UserEngine.m_MonObjectList.Add(Monster);
{$IF MULTI_THREAD = 1}
                finally
                  if g_MultiThreadRun then
                    UserEngine.m_MonObjectList.UnLockW;
                end;
{$IFEND}
              end;
            end;
          end;
        end;
      end;
    finally
      LoadList.Free;
    end;
  end;
end;

function TFrmDB.GetScriptFileCRC(FileName: string): LongWord;
type
  TFileTimeEx = record
    CreationTime: TFileTime;
    LastWriteTime: TFileTime;
  end;
var
  // Buf: PByte;
  LastAccessTime: TFileTime;
  FileTimeEx: TFileTimeEx;
  FileStream: TFileStream;
begin
  Result := 0;
  if not FileExists(FileName) then
    Exit;
  FileStream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyNone);
  try
    if GetFileTime(FileStream.Handle, @FileTimeEx.CreationTime, @LastAccessTime, @FileTimeEx.LastWriteTime) then
      Result := BufferCrc(@FileTimeEx, SizeOf(TFileTimeEx));
  finally
    FileStream.Free;
  end;
end;

function TFrmDB.LoadNpcScript(NPC: TNormNpc; sPatch, sScritpName: string): Integer;
begin
  if sPatch = '' then
    sPatch := sNpc_def;
  Result := LoadScriptFile(NPC, sPatch, sScritpName, False);
end;

function TFrmDB.LoadScriptFile(NPC: TNormNpc; sPatch, sScritpName: string; boFlag: Boolean): Integer;
var
  nQuestIdx, I, n1C, n20, n24, nItemType, nPriceRate, nIndex: Integer;
  n6C, n70: Integer;
  sScritpFileName, sTempFileName, s30, s34, s38, s3C, s40, s44, s48, s4C, s50, tmpStr: string;
  LoadList: TStringList;
  DefineList: TList;
  s54, s58, s5C, s74: string;
  DefineInfo: pTDefineInfo;
  bo8D: Boolean;
  Script: pTScript;
  SayingRecord: pTSayingRecord;
  SayingProcedure: pTSayingProcedure;
  QuestConditionInfo: pTQuestConditionInfo;
  QuestActionInfo: pTQuestActionInfo;
  Goods: pTGoods;
  MemoryStream: TMemoryStream;
  boIF, boOR: Boolean;

  function LoadCallScript(sFileName, sLabel: string; var List: TStringList): Boolean;
  var
    I: Integer;
    LoadStrList: TStringList;
    bo1D: Boolean;
    s18: string;
    dwCRC: LongWord;
  begin
    Result := False;
    if FileExists(sFileName) then
    begin
      // 修正当文件改变时才重新加载Npc 2020-05-17 23:47:02
      bo1D := False;
      for I := 0 to NPC.m_CallFileListCRC.Count - 1 do
      begin
        if SameText(NPC.m_CallFileListCRC.Strings[I], sFileName) then
        begin
          bo1D := True;
          Break;
        end;
      end;
      if not bo1D then
      begin
        dwCRC := GetScriptFileCRC(sFileName);
        NPC.m_CallFileListCRC.AddObject(sFileName, TObject(dwCRC));
      end;
      LoadStrList := TStringListEx.Create;
      LoadStrList.LoadFromFile(sFileName);
      DeCodeStringList(LoadStrList);
      sLabel := '[' + sLabel + ']';
      bo1D := False;
      for I := 0 to LoadStrList.Count - 1 do
      begin
        s18 := Trim(LoadStrList.Strings[I]);
        if s18 <> '' then
        begin
          if (s18[1] = '{') and (Length(s18) = 1) then
          begin
            Continue;
          end
          else
          begin
            if not bo1D then
            begin
              if (s18[1] = '[') and (CompareText(s18, sLabel) = 0) then
              begin
                bo1D := True;
                List.Add(s18);
              end
              else
              begin
              end;
            end
            else
            begin
              if (s18[1] = '}') then
              begin
                Result := True;
                Break;
              end
              else
              begin
                List.Add(s18);
              end;
            end;
          end;
        end;
      end;
      LoadStrList.Free;
    end;
  end;

  function LoadCallScript2(LoadStrList: TStringList; sLabel: string; var List: TStringList): Boolean;
  var
    I: Integer;
    bo1D: Boolean;
    s18: string;
  begin
    Result := False;
    DeCodeStringList(LoadStrList);
    sLabel := '[' + sLabel + ']';
    bo1D := False;
    for I := 0 to LoadStrList.Count - 1 do
    begin
      s18 := Trim(LoadStrList.Strings[I]);
      if s18 <> '' then
      begin
        if (s18[1] = '{') and (Length(s18) = 1) then
        begin
          Continue;
        end
        else
        begin
          if not bo1D then
          begin
            if (s18[1] = '[') and (CompareText(s18, sLabel) = 0) then
            begin
              bo1D := True;
              List.Add(s18);
            end
            else
            begin
            end;
          end
          else
          begin
            if (s18[1] = '}') then
            begin
              Result := True;
              Break;
            end
            else
            begin
              List.Add(s18);
            end;
          end;
        end;
      end;
    end;
  end;

  procedure LoadScriptCall(sFileName: string; var LoadList: TStringList);
  var
    I, J, II: Integer;
    IsFoundAct: Boolean;
    TempStrings: TStringList;
    s14, s18, s1C, s20, s34, sTemp, sGoto: string;
  begin
    for I := 0 to LoadList.Count - 1 do
    begin
      s14 := Trim(LoadList.Strings[I]);

      if (s14 <> '') and (s14[1] = '#') and CompareLStr(s14, '#CALL', Length('#CALL')) then
      begin
        IsFoundAct := False;
        for J := I - 1 downto 1 do
        begin
          sTemp := Trim(LoadList.Strings[J]);
          if SameText(Copy(sTemp, 1, 2), '[@') then
          begin
            Break
          end
          else if AnsiStartsText('#IF', sTemp) then
          begin
            Break;
          end
          else
          begin
            if SameText(sTemp, '#act') then
            begin
              IsFoundAct := True;
              Break;
            end
            else if SameText(sTemp, '#elseact') then
            begin
              IsFoundAct := True;
              Break;
            end;
          end;
        end;

        s14 := ArrestStringEx(s14, '[', ']', s1C);
        s20 := Trim(s1C);
        s18 := Trim(s14);
        II := pos('(', s18);
        if II > 0 then
          sGoto := Trim(Copy(s18, 1, II - 1))
        else
          sGoto := s18;

        if (Length(s20) > 0) and (s20[1] = '\') then
          s20 := Copy(s20, 2, Length(s20) - 1);
        if (Length(s20) > 0) and (s20[1] = '\') then
          s20 := Copy(s20, 2, Length(s20) - 1);

        // 支持远程脚本 2019-05-14 18:01:02
        s34 := g_Config.sEnvirDir + 'QuestDiary\' + s20;
        if not FileExists(s34) then
        begin
          if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
          begin
            try
              s34 := ReplaceChar('QuestDiary\' + s20, '/', '\');
              MemoryStream := TMemoryStream.Create;
              if not g_PluginManager.HookLoadScriptFile(PAnsiChar(AnsiString(s34)), MemoryStream) then
              begin
                MemoryStream.Free;
                Exit;
              end;

              TempStrings := TStringList.Create;
              try
                if (MemoryStream.Size > 0) then
                begin
                  s54 := DeCodePlugBuffer(MemoryStream);
                  TempStrings.Text := s54;
                  if LoadCallScript2(TempStrings, sGoto, LoadList) then
                  begin
                    if IsFoundAct then
                    begin
                      LoadList.Strings[I] := 'goto ' + s18;
                    end
                    else
                    begin
                      LoadList.Strings[I] := '#ACT';
                      LoadList.Insert(I + 1, 'goto ' + s18);
                    end;
                  end;
                end;
              finally
                TempStrings.Free;
              end;
              MemoryStream.Free;
            except
              on E: Exception do
              begin
                MainOutMessage('[Exception] HookLoadScriptFile');
                MainOutMessage(E.Message);
                Exit;
              end;
            end;
          end
          else
          begin
            Result := 1;
            MainOutMessage('脚本文件未找到: ' + s34);
            Exit;
          end;
        end
        else if LoadCallScript(s34, sGoto, LoadList) then
        begin
          {
            // 检测重复的标签 2020-03-17 12:20:54
            if LoadList.IndexOf('[' + s18 + ']') >= 0 then
            begin
            MainOutMessage(Format('%s中已经存在[%s]，与%s 中重复', [sFileName, s18, LoadList.Strings[I]]));
            end;
          }
          // 修正在 #elseact中 的 #call 不能正确执行 chongchong 2017-07-26
          if IsFoundAct then
          begin
            LoadList.Strings[I] := 'goto ' + s18;
          end
          else
          begin
            LoadList.Strings[I] := '#ACT';
            LoadList.Insert(I + 1, 'goto ' + s18);
          end;
        end
        else
          MainOutMessage('脚本读取失败：' + s20 + s18);
      end;
    end;
  end;

  function LoadDefineInfo(var LoadList: TStringList; var List: TList): string;
  var
    I, II: Integer;
    s14, s28, s1C, s20, s24: string;
    DefineInfo: pTDefineInfo;
    LoadStrList: TStringList;
    boFound: Boolean;
    dwCRC: LongWord;
  begin
    for I := 0 to LoadList.Count - 1 do
    begin
      s14 := Trim(LoadList.Strings[I]);

      if (s14 <> '') and (s14[1] = '#') then
      begin
        if CompareLStr(s14, '#SETHOME', Length('#SETHOME')) then
        begin
          Result := Trim(GetValidStr3(s14, s1C, [' ', #9]));
          LoadList.Strings[I] := '';
        end;

        if CompareLStr(s14, '#DEFINE', Length('#DEFINE')) then
        begin
          s14 := (GetValidStr3(s14, s1C, [' ', #9]));
          s14 := (GetValidStr3(s14, s20, [' ', #9]));
          s14 := (GetValidStr3(s14, s24, [' ', #9]));
          New(DefineInfo);
          DefineInfo.sName := UpperCase(s20);
          DefineInfo.sText := s24;
          List.Add(DefineInfo);
          LoadList.Strings[I] := '';
        end;

        if CompareLStr(s14, '#ADEFINE', Length('#ADEFINE')) then
        begin
          s14 := (GetValidStr3(s14, s1C, [' ', #9]));
          s14 := (GetValidStr3(s14, s20, [' ', #9]));
          New(DefineInfo);
          DefineInfo.sName := UpperCase(s20);
          DefineInfo.sText := s14;
          List.Add(DefineInfo);
          LoadList.Strings[I] := '';
        end;

        if CompareLStr(s14, '#INCLUDE', Length('#INCLUDE')) then
        begin
          s28 := Trim(GetValidStr3(s14, s1C, [' ', #9]));
          s28 := g_Config.sEnvirDir + 'Defines\' + s28;
          if FileExists(s28) then
          begin
            // 修正当文件改变时才重新加载Npc 2020-05-17 23:47:02
            boFound := False;
            for II := 0 to NPC.m_CallFileListCRC.Count - 1 do
            begin
              if SameText(NPC.m_CallFileListCRC.Strings[II], s28) then
              begin
                boFound := True;
                Break;
              end;
            end;

            if not boFound then
            begin
              dwCRC := GetScriptFileCRC(s28);
              NPC.m_CallFileListCRC.AddObject(s28, TObject(dwCRC));
            end;

            LoadStrList := TStringListEx.Create;
            LoadStrList.LoadFromFile(s28);
            Result := LoadDefineInfo(LoadStrList, List);
            LoadStrList.Free;
          end
          else
            MainOutMessage('脚本读取失败：' + s28);

          LoadList.Strings[I] := '';
        end;
      end;
    end;
  end;

  function MakeNewScript(): pTScript;
  var
    ScriptInfo: pTScript;
  begin
    New(ScriptInfo);
    ScriptInfo.boQuest := False;
    FillChar(ScriptInfo.QuestInfo, SizeOf(TQuestInfo) * 10, #0);
    nQuestIdx := 0;
    ScriptInfo.RecordList := TList.Create;
    NPC.m_ScriptList.Add(ScriptInfo);
    Result := ScriptInfo;
  end;

  function QuestCondition(sText: string; var QuestConditionInfo: pTQuestConditionInfo): Boolean; // 00489DDC
  var
    sCmdLine, sCmd, sParam, sParam1, sParam2, sParam3, sParam4, sParam5, sParam6, sParam7, sParam8, sParam9, sParam10: string;
    nCMDCode: Integer;
    sSubParam1: string;
    sSubParam2: string;
    sSubParam3: string;
    sSubParam4: string;
    sSubParam5: string;
    sSubParam6: string;
    sSubParam7: string;
    sSubParam8: string;
    sSubParam9: string;
    sSubParam10: string;
  label
    L001;
  begin
    Result := False;
    QuestConditionInfo.boNot := False;
    sCmdLine := sText;
    sText := GetValidStrCap(sText, sCmd, [' ', #9]);
    if UpperCase(sCmd) = 'NOT' then
    begin // 取反
      QuestConditionInfo.boNot := True;
      sText := GetValidStrCap(sText, sCmd, [' ', #9]);
    end;
    sParam := sText;
    sText := GetValidStrCap(sText, sParam1, [' ', #9]);
    sSubParam1 := sText;
    sText := GetValidStrCap(sText, sParam2, [' ', #9]);
    sSubParam2 := sText;
    sText := GetValidStrCap(sText, sParam3, [' ', #9]);
    sSubParam3 := sText;
    sText := GetValidStrCap(sText, sParam4, [' ', #9]);
    sSubParam4 := sText;
    sText := GetValidStrCap(sText, sParam5, [' ', #9]);
    sSubParam4 := sText;
    sText := GetValidStrCap(sText, sParam6, [' ', #9]);
    sSubParam6 := sText;
    sText := GetValidStrCap(sText, sParam7, [' ', #9]);
    sSubParam7 := sText;
    sText := GetValidStrCap(sText, sParam8, [' ', #9]);
    sSubParam8 := sText;
    sText := GetValidStrCap(sText, sParam9, [' ', #9]);
    sSubParam9 := sText;
    sText := GetValidStrCap(sText, sParam10, [' ', #9]);
    sSubParam10 := sText;
    sCmd := UpperCase(sCmd);
    sCmd := LoadLevelScriptCondition(QuestConditionInfo, sCmd);
    nCMDCode := 0;
    nIndex := g_CmdConditionList.IndexOf(sCmd);
    if nIndex >= 0 then
    begin
      nCMDCode := Integer(g_CmdConditionList.Objects[nIndex]);
      if nCMDCode = nNC_CHECK then
      begin
        ArrestStringEx(sParam1, '[', ']', sParam1);
      end
      else if (nCMDCode = nNC_CheckMobileNumber) or (nCMDCode = nNC_CheckMobileBind) then
      begin
        if g_nKey_MobileSMS = 0 then
        begin
          nCMDCode := 0;
        end;
      end;
      if nCMDCode >= MAXNPCCMDCODE then
      begin
        nCMDCode := 0;
        MainOutMessage(sCmd + '; 函数超出范围');
      end;
      goto L001;
    end;
    if (g_PluginManager <> nil) and (nCMDCode <= 0) then
    begin
      nCMDCode := g_PluginManager.HookNpcLoadConditionCmd(PAnsiChar(AnsiString(sCmd)));
    end;
  L001:
    if nCMDCode > 0 then
    begin
      QuestConditionInfo.nCMDCode := nCMDCode;
      QuestConditionInfo.sCmd := sCmd;
      QuestConditionInfo.sCmdLine := sCmdLine;
      if (sParam1 <> '') and (sParam1[1] = '"') then
      begin
        ArrestStringEx(sParam1, '"', '"', sParam1);
      end;
      if (sParam2 <> '') and (sParam2[1] = '"') then
      begin
        ArrestStringEx(sParam2, '"', '"', sParam2);
      end;
      if (sParam3 <> '') and (sParam3[1] = '"') then
      begin
        ArrestStringEx(sParam3, '"', '"', sParam3);
      end;
      if (sParam4 <> '') and (sParam4[1] = '"') then
      begin
        ArrestStringEx(sParam4, '"', '"', sParam4);
      end;
      if (sParam5 <> '') and (sParam5[1] = '"') then
      begin
        ArrestStringEx(sParam5, '"', '"', sParam5);
      end;
      if (sParam6 <> '') and (sParam6[1] = '"') then
      begin
        ArrestStringEx(sParam6, '"', '"', sParam6);
      end;
      if (sParam7 <> '') and (sParam7[1] = '"') then
      begin
        ArrestStringEx(sParam7, '"', '"', sParam7);
      end;
      if (sParam8 <> '') and (sParam8[1] = '"') then
      begin
        ArrestStringEx(sParam8, '"', '"', sParam8);
      end;
      if (sParam9 <> '') and (sParam9[1] = '"') then
      begin
        ArrestStringEx(sParam9, '"', '"', sParam9);
      end;
      if (sParam10 <> '') and (sParam10[1] = '"') then
      begin
        ArrestStringEx(sParam10, '"', '"', sParam10);
      end;
      QuestConditionInfo.sParam := sParam;
      QuestConditionInfo.sParam1 := sParam1;
      QuestConditionInfo.sParam2 := sParam2;
      QuestConditionInfo.sParam3 := sParam3;
      QuestConditionInfo.sParam4 := sParam4;
      QuestConditionInfo.sParam5 := sParam5;
      QuestConditionInfo.sParam6 := sParam6;
      QuestConditionInfo.sParam7 := sParam7;
      QuestConditionInfo.sParam8 := sParam8;
      QuestConditionInfo.sParam9 := sParam9;
      QuestConditionInfo.sParam10 := sParam10;
      QuestConditionInfo.sRawParam1 := sParam1;
      QuestConditionInfo.sRawParam2 := sParam2;
      QuestConditionInfo.sRawParam3 := sParam3;
      QuestConditionInfo.sRawParam4 := sParam4;
      QuestConditionInfo.sRawParam5 := sParam5;
      QuestConditionInfo.sRawParam6 := sParam6;
      QuestConditionInfo.sRawParam7 := sParam7;
      QuestConditionInfo.sRawParam8 := sParam8;
      QuestConditionInfo.sRawParam9 := sParam9;
      QuestConditionInfo.sRawParam10 := sParam10;
      QuestConditionInfo.sSubParam1 := sSubParam1;
      QuestConditionInfo.sSubParam2 := sSubParam2;
      QuestConditionInfo.sSubParam3 := sSubParam3;
      QuestConditionInfo.sSubParam4 := sSubParam4;
      QuestConditionInfo.sSubParam5 := sSubParam5;
      QuestConditionInfo.sSubParam6 := sSubParam6;
      QuestConditionInfo.sSubParam7 := sSubParam7;
      QuestConditionInfo.sSubParam8 := sSubParam8;
      QuestConditionInfo.sSubParam9 := sSubParam9;
      QuestConditionInfo.sSubParam10 := sSubParam10;
      QuestConditionInfo.VarInfo1 := GetValNameInfo(sParam1);
      QuestConditionInfo.VarInfo2 := GetValNameInfo(sParam2);
      QuestConditionInfo.VarInfo3 := GetValNameInfo(sParam3);
      QuestConditionInfo.VarInfo4 := GetValNameInfo(sParam4);
      QuestConditionInfo.VarInfo5 := GetValNameInfo(sParam5);
      QuestConditionInfo.VarInfo6 := GetValNameInfo(sParam6);
      QuestConditionInfo.VarInfo7 := GetValNameInfo(sParam7);
      QuestConditionInfo.VarInfo8 := GetValNameInfo(sParam8);
      QuestConditionInfo.VarInfo9 := GetValNameInfo(sParam9);
      QuestConditionInfo.VarInfo10 := GetValNameInfo(sParam10);
      QuestConditionInfo.boCompleteFormat1 := GetValCompleteFormat(sParam1);
      QuestConditionInfo.boCompleteFormat2 := GetValCompleteFormat(sParam2);
      QuestConditionInfo.boCompleteFormat3 := GetValCompleteFormat(sParam3);
      QuestConditionInfo.boCompleteFormat4 := GetValCompleteFormat(sParam4);
      QuestConditionInfo.boCompleteFormat5 := GetValCompleteFormat(sParam5);
      QuestConditionInfo.boCompleteFormat6 := GetValCompleteFormat(sParam6);
      QuestConditionInfo.boCompleteFormat7 := GetValCompleteFormat(sParam7);
      QuestConditionInfo.boCompleteFormat8 := GetValCompleteFormat(sParam8);
      QuestConditionInfo.boCompleteFormat9 := GetValCompleteFormat(sParam9);
      QuestConditionInfo.boCompleteFormat10 := GetValCompleteFormat(sParam10);
      if (QuestConditionInfo.VarInfo1.VarAttr = aNone) { and IsStringNumber(sParam1) } then
        QuestConditionInfo.nParam1 := StrToInt64Def(sParam1, 0);
      if (QuestConditionInfo.VarInfo2.VarAttr = aNone) then
        QuestConditionInfo.nParam2 := StrToInt64Def(sParam2, 0);
      if (QuestConditionInfo.VarInfo3.VarAttr = aNone) then
        QuestConditionInfo.nParam3 := StrToInt64Def(sParam3, 0);
      if (QuestConditionInfo.VarInfo4.VarAttr = aNone) then
        QuestConditionInfo.nParam4 := StrToInt64Def(sParam4, 0);
      if (QuestConditionInfo.VarInfo5.VarAttr = aNone) then
        QuestConditionInfo.nParam5 := StrToInt64Def(sParam5, 0);
      if (QuestConditionInfo.VarInfo6.VarAttr = aNone) then
        QuestConditionInfo.nParam6 := StrToInt64Def(sParam6, 0);
      if (QuestConditionInfo.VarInfo7.VarAttr = aNone) then
        QuestConditionInfo.nParam7 := StrToInt64Def(sParam7, 0);
      if (QuestConditionInfo.VarInfo8.VarAttr = aNone) then
        QuestConditionInfo.nParam8 := StrToInt64Def(sParam8, 0);
      if (QuestConditionInfo.VarInfo9.VarAttr = aNone) then
        QuestConditionInfo.nParam9 := StrToInt64Def(sParam9, 0);
      if (QuestConditionInfo.VarInfo10.VarAttr = aNone) then
        QuestConditionInfo.nParam10 := StrToIntDef(sParam10, 0);
      Result := True;
    end;
  end;

  function QuestAction(sText: string; var QuestActionInfo: pTQuestActionInfo): Boolean;
  var
    sCmdLine, sCmd, sParam, sParam1, sParam2, sParam3, sParam4, sParam5, sParam6, sParam7, sParam8, sParam9, sParam10: string;
    sSubParam1: string;
    sSubParam2: string;
    sSubParam3: string;
    sSubParam4: string;
    sSubParam5: string;
    sSubParam6: string;
    sSubParam7: string;
    sSubParam8: string;
    sSubParam9: string;
    sSubParam10: string;
    nCMDCode: Integer;
    nIndex: Integer;
  label
    L001;
  begin
    Result := False;
    sCmdLine := sText;
    sText := GetValidStrCap(sText, sCmd, [' ', #9]);
    sParam := sText;
    sText := GetValidStrCap(sText, sParam1, [' ', #9]);
    sSubParam1 := sText;
    sText := GetValidStrCap(sText, sParam2, [' ', #9]);
    sSubParam2 := sText;
    sText := GetValidStrCap(sText, sParam3, [' ', #9]);
    sSubParam3 := sText;
    sText := GetValidStrCap(sText, sParam4, [' ', #9]);
    sSubParam4 := sText;
    sText := GetValidStrCap(sText, sParam5, [' ', #9]);
    sSubParam4 := sText;
    sText := GetValidStrCap(sText, sParam6, [' ', #9]);
    sSubParam6 := sText;
    sText := GetValidStrCap(sText, sParam7, [' ', #9]);
    sSubParam7 := sText;
    sText := GetValidStrCap(sText, sParam8, [' ', #9]);
    sSubParam8 := sText;
    sText := GetValidStrCap(sText, sParam9, [' ', #9]);
    sSubParam9 := sText;
    sText := GetValidStrCap(sText, sParam10, [' ', #9]);
    sSubParam10 := sText;
    sCmd := UpperCase(sCmd);
    sCmd := LoadLevelScriptAction(QuestActionInfo, sCmd);
    nCMDCode := 0;
    nIndex := g_CmdActionList.IndexOf(sCmd);
    if nIndex >= 0 then
    begin
      nCMDCode := Integer(g_CmdActionList.Objects[nIndex]);
      if (nCMDCode = nNA_SET) or (nCMDCode = nNA_RESET) then
      begin
        ArrestStringEx(sParam1, '[', ']', sParam1);
      end
      else if (nCMDCode = nNA_SETMAPQUEST) then
      begin
        ArrestStringEx(sParam2, '[', ']', sParam2);
        if not IsStringNumber(sParam2) then
          nCMDCode := 0;
        if not IsStringNumber(sParam3) then
          nCMDCode := 0;
      end
{$IF NEED_KEY <> 2}  // 20230404 HZQ 这段代码可以不要，只是限制功能
      else if (nCMDCode = nNA_CancelHeroForcePeaceMode) or (nCMDCode = nNA_DecAddAngryValueTime) then
      begin
        // VMProtectBegin('VMProtect_QuestAction01');
        if g_nKey_HeroExt = 0 then
        begin
          nCMDCode := 0;
        end;
        // VMProtectEnd();
      end
      else if (nCMDCode = nNA_SetMobileNumber) or (nCMDCode = nNA_SetMobileBind) or (nCMDCode = nNA_SendMobileVerifyCode) then
      begin
        // VMProtectBegin('VMProtect_QuestAction02');
        if g_nKey_MobileSMS = 0 then
        begin
          nCMDCode := 0;
        end;
        // VMProtectEnd();
      end
      else if (nCMDCode = nNA_OPENAUCTIONVIEW) then
      begin
        // VMProtectBegin('VMProtect_QuestAction03');
        if g_nKey_Auction = 0 then
        begin
          nCMDCode := 0;
        end;
        // VMProtectEnd();
      end
{$IF NEED_KEY = 1}
      else if (nCMDCode = nNA_EnableUseClientPickItems) then
      begin
        // VMProtectBegin('VMProtect_QuestAction04');
        if g_nKey_UseClientPickItems = 0 then
        begin
          nCMDCode := 0;
        end;
        // VMProtectEnd();
      end
      else if (nCMDCode = nNA_PreviewMonDropItem) then
      begin
        // VMProtectBegin('VMProtect_QuestAction05');
        if g_nKey_PreviewMonItem = 0 then
          nCMDCode := 0;
        // VMProtectEnd();
      end
      else if (nCMDCode = nNA_PreviewMonDropItemRefresh) then
      begin
        // VMProtectBegin('VMProtect_QuestAction06');
        if g_nKey_PreviewMonItem = 0 then
          nCMDCode := 0;
        // VMProtectEnd();
      end
      else if (nCMDCode = nNA_OpenAddSellPlayerDlg) or (nCMDCode = nNA_BreakAddSellPlayer) or (nCMDCode = nNA_GetSellPlayerCount)
        or (nCMDCode = nNA_DelSellPlayer) or (nCMDCode = nNA_OpenDelSellPlayerDlg) or (nCMDCode = nNA_OpenSellPlayerShopDlg) or
        (nCMDCode = nNA_ChangeAccountInfo) then
      begin
        // VMProtectBegin('VMProtect_QuestAction07');
        if g_nKey_SellPlayer = 0 then
        begin
          nCMDCode := 0;
        end;
        // VMProtectEnd();
      end
      else if (nCMDCode = nNA_OpenAutoDropItemToBag) or (nCMDCode = nNA_CloseAutoDropItemToBag) or
        (nCMDCode = nNA_OpenAutoPickItem) or (nCMDCode = nNA_CloseAutoPickItem) or (nCMDCode = nNA_ForcePickItem) then
      begin
        // VMProtectBegin('VMProtect_QuestAction08');
        if g_nKey_UseClientPickItems = 0 then
        begin
          nCMDCode := 0;
        end;
        // VMProtectEnd();
      end;
{$IFEND}
{$IFEND}
      {
        if nCMDCode >= MAXNPCCMDCODE then begin
        nCMDCode := 0;
        MainOutMessage(sCmd + '; 函数超出范围');
        end;
        goto L001;
      }
    end;
    if (g_PluginManager <> nil) and (nCMDCode <= 0) then
    begin
      nCMDCode := g_PluginManager.HookNpcLoadActionCmd(PAnsiChar(AnsiString(sCmd)));
    end;
  L001:
    if nCMDCode > 0 then
    begin
      QuestActionInfo.nCMDCode := nCMDCode;
      QuestActionInfo.sCmd := sCmd;
      QuestActionInfo.sCmdLine := sCmdLine;
      if (sParam1 <> '') and (sParam1[1] = '"') then
      begin
        ArrestStringEx(sParam1, '"', '"', sParam1);
      end;
      if (sParam2 <> '') and (sParam2[1] = '"') then
      begin
        ArrestStringEx(sParam2, '"', '"', sParam2);
      end;
      if (sParam3 <> '') and (sParam3[1] = '"') then
      begin
        ArrestStringEx(sParam3, '"', '"', sParam3);
      end;
      if (sParam4 <> '') and (sParam4[1] = '"') then
      begin
        ArrestStringEx(sParam4, '"', '"', sParam4);
      end;
      if (sParam5 <> '') and (sParam5[1] = '"') then
      begin
        ArrestStringEx(sParam5, '"', '"', sParam5);
      end;
      if (sParam6 <> '') and (sParam6[1] = '"') then
      begin
        ArrestStringEx(sParam6, '"', '"', sParam6);
      end;
      if (sParam7 <> '') and (sParam7[1] = '"') then
      begin
        ArrestStringEx(sParam7, '"', '"', sParam7);
      end;
      if (sParam8 <> '') and (sParam8[1] = '"') then
      begin
        ArrestStringEx(sParam8, '"', '"', sParam8);
      end;
      if (sParam9 <> '') and (sParam9[1] = '"') then
      begin
        ArrestStringEx(sParam9, '"', '"', sParam9);
      end;
      if (sParam10 <> '') and (sParam10[1] = '"') then
      begin
        ArrestStringEx(sParam10, '"', '"', sParam10);
      end;
      // QuestActionInfo.sParams := sParam;
      QuestActionInfo.sParam1 := sParam1;
      QuestActionInfo.sParam2 := sParam2;
      QuestActionInfo.sParam3 := sParam3;
      QuestActionInfo.sParam4 := sParam4;
      QuestActionInfo.sParam5 := sParam5;
      QuestActionInfo.sParam6 := sParam6;
      QuestActionInfo.sParam7 := sParam7;
      QuestActionInfo.sParam8 := sParam8;
      QuestActionInfo.sParam9 := sParam9;
      QuestActionInfo.sParam10 := sParam10;
      QuestActionInfo.sRawParam1 := sParam1;
      QuestActionInfo.sRawParam2 := sParam2;
      QuestActionInfo.sRawParam3 := sParam3;
      QuestActionInfo.sRawParam4 := sParam4;
      QuestActionInfo.sRawParam5 := sParam5;
      QuestActionInfo.sRawParam6 := sParam6;
      QuestActionInfo.sRawParam7 := sParam7;
      QuestActionInfo.sRawParam8 := sParam8;
      QuestActionInfo.sRawParam9 := sParam9;
      QuestActionInfo.sRawParam10 := sParam10;
      QuestActionInfo.sSubParam1 := sSubParam1;
      QuestActionInfo.sSubParam2 := sSubParam2;
      QuestActionInfo.sSubParam3 := sSubParam3;
      QuestActionInfo.sSubParam4 := sSubParam4;
      QuestActionInfo.sSubParam5 := sSubParam5;
      QuestActionInfo.sSubParam6 := sSubParam6;
      QuestActionInfo.sSubParam7 := sSubParam7;
      QuestActionInfo.sSubParam8 := sSubParam8;
      QuestActionInfo.sSubParam9 := sSubParam9;
      QuestActionInfo.sSubParam10 := sSubParam10;
      QuestActionInfo.VarInfo1 := GetValNameInfo(sParam1);
      QuestActionInfo.VarInfo2 := GetValNameInfo(sParam2);
      QuestActionInfo.VarInfo3 := GetValNameInfo(sParam3);
      QuestActionInfo.VarInfo4 := GetValNameInfo(sParam4);
      QuestActionInfo.VarInfo5 := GetValNameInfo(sParam5);
      QuestActionInfo.VarInfo6 := GetValNameInfo(sParam6);
      QuestActionInfo.VarInfo7 := GetValNameInfo(sParam7);
      QuestActionInfo.VarInfo8 := GetValNameInfo(sParam8);
      QuestActionInfo.VarInfo9 := GetValNameInfo(sParam9);
      QuestActionInfo.VarInfo10 := GetValNameInfo(sParam10);
      QuestActionInfo.boCompleteFormat1 := GetValCompleteFormat(sParam1);
      QuestActionInfo.boCompleteFormat2 := GetValCompleteFormat(sParam2);
      QuestActionInfo.boCompleteFormat3 := GetValCompleteFormat(sParam3);
      QuestActionInfo.boCompleteFormat4 := GetValCompleteFormat(sParam4);
      QuestActionInfo.boCompleteFormat5 := GetValCompleteFormat(sParam5);
      QuestActionInfo.boCompleteFormat6 := GetValCompleteFormat(sParam6);
      QuestActionInfo.boCompleteFormat7 := GetValCompleteFormat(sParam7);
      QuestActionInfo.boCompleteFormat8 := GetValCompleteFormat(sParam8);
      QuestActionInfo.boCompleteFormat9 := GetValCompleteFormat(sParam9);
      QuestActionInfo.boCompleteFormat10 := GetValCompleteFormat(sParam10);
      if (QuestActionInfo.VarInfo1.VarAttr = aNone) { and IsStringNumber(sParam1) } then
        QuestActionInfo.nParam1 := StrToInt64Def(sParam1, 0);
      if (QuestActionInfo.VarInfo2.VarAttr = aNone) { and IsStringNumber(sParam2) } then
        QuestActionInfo.nParam2 := StrToInt64Def(sParam2, 0);
      if (QuestActionInfo.VarInfo3.VarAttr = aNone) { and IsStringNumber(sParam3) } then
        QuestActionInfo.nParam3 := StrToInt64Def(sParam3, 0);
      if (QuestActionInfo.VarInfo4.VarAttr = aNone) then
        QuestActionInfo.nParam4 := StrToInt64Def(sParam4, 0);
      if (QuestActionInfo.VarInfo5.VarAttr = aNone) then
        QuestActionInfo.nParam5 := StrToInt64Def(sParam5, 0);
      if (QuestActionInfo.VarInfo6.VarAttr = aNone) then
        QuestActionInfo.nParam6 := StrToInt64Def(sParam6, 0);
      if (QuestActionInfo.VarInfo7.VarAttr = aNone) then
        QuestActionInfo.nParam7 := StrToInt64Def(sParam7, 0);
      if (QuestActionInfo.VarInfo8.VarAttr = aNone) then
        QuestActionInfo.nParam8 := StrToInt64Def(sParam8, 0);
      if (QuestActionInfo.VarInfo9.VarAttr = aNone) then
        QuestActionInfo.nParam9 := StrToInt64Def(sParam9, 0);
      if (QuestActionInfo.VarInfo10.VarAttr = aNone) then
        QuestActionInfo.nParam10 := StrToInt64Def(sParam10, 0);
      Result := True;
    end;
  end;

/// <summary>
/// 处理多关键字在同一行的情况
/// </summary>
  procedure ProcessCommandText(ACommands: TStrings);
  var
    I, J: Integer;
    tmpSL1, tmpSL2: TStrings;
  begin
    if ACommands.Count <= 0 then
      Exit;

    tmpSL1 := TStringList.Create;
    tmpSL2 := TStringList.Create;
    for I := 0 to ACommands.Count - 1 do
    begin
      tmpSL1.Clear;
      if ExtractStrings(['#'], [], PChar(ACommands[I]), tmpSL1) > 1 then
      begin
        tmpSL2.Clear;
        for J := 0 to tmpSL1.Count - 1 do
        begin
          if AnsiStartsText('if', tmpSL1[J]) //
            or AnsiStartsText('act', tmpSL1[J]) //
            or AnsiStartsText('say', tmpSL1[J]) //
            or AnsiStartsText('elseact', tmpSL1[J]) //
            or AnsiStartsText('elsesay', tmpSL1[J]) //
            or AnsiStartsText('call', tmpSL1[J]) then
            tmpSL2.Add(Format('#%s', [tmpSL1[J]]));
        end;

        if tmpSL2.Count > 1 then
        begin
          ACommands.Delete(I);
          for J := tmpSL2.Count - 1 downto 0 do
            ACommands.Insert(I, tmpSL2[J]);
        end;
      end;
    end;
    tmpSL2.Free;
    tmpSL1.Free;
  end;

begin
  if g_boStopRun then
    Exit;

  Result := -1;
  n6C := 0;
  n70 := 0;
  sTempFileName := sPatch + sScritpName + '.txt';
  sScritpFileName := g_Config.sEnvirDir + sTempFileName;
  NPC.m_CallFileListCRC.Clear;

  if not FileExists(sScritpFileName) then
  begin
    if (g_PluginManager <> nil) and g_PluginManager.IsCheckLoadScriptFileHook then
    begin
      try
        sScritpFileName := ReplaceChar(sPatch + sScritpName + '.txt', '/', '\');
        MemoryStream := TMemoryStream.Create;
        if not g_PluginManager.HookLoadScriptFile(PAnsiChar(AnsiString(sScritpFileName)), MemoryStream) then
        begin
          MemoryStream.Free;
          Exit;
        end;

        LoadList := TStringList.Create;
        if MemoryStream.Size > 0 then
        begin
          s54 := DeCodePlugBuffer(MemoryStream);
          LoadList.Text := s54;
        end;
        MemoryStream.Free;
      except
        on E: Exception do
        begin
          MainOutMessage('[Exception] HookLoadScriptFile');
          MainOutMessage(E.Message);
          Exit;
        end;
      end;
    end
    else
    begin
      Result := 1;
      MainOutMessage('脚本文件未找到: ' + sScritpFileName);
      Exit;
    end;
  end
  else
  begin
    LoadList := TStringListEx.Create;
    try
      // 修正当文件改变时才重新加载Npc 2020-05-17 23:47:02
      NPC.m_dwScriptCRC := GetScriptFileCRC(sScritpFileName);
      LoadList.LoadFromFile(sScritpFileName);
      DeCodeStringList(LoadList);
    except
      LoadList.Free;
      MainOutMessage('脚本文件未找到: ' + sScritpFileName);
      Exit;
    end;
  end;

  I := 0;
  while True do
  begin
    LoadScriptCall(sTempFileName, LoadList);

    Inc(I);
    if I >= 10 then // 最大Call 10 次
      Break;
  end;

  DefineList := TList.Create;
  bo8D := False;
  s54 := LoadDefineInfo(LoadList, DefineList);

  New(DefineInfo);
  DefineInfo.sName := '@HOME';
  if s54 = '' then
    s54 := '@main';

  DefineInfo.sText := s54;
  DefineList.Add(DefineInfo);
  // 常量处理
  for I := 0 to LoadList.Count - 1 do
  begin
    if (g_boExitServer or Application.Terminated) then
      Break;

    s34 := Trim(LoadList.Strings[I]);
    if (s34 <> '') then
    begin
      if (s34[1] = '[') then
      begin
        bo8D := False;
      end
      else
      begin
        if (s34[1] = '#') //
          and (CompareLStr(s34, '#IF', Length('#IF')) // -
          or CompareLStr(s34, '#ACT', Length('#ACT')) // -
          or CompareLStr(s34, '#ELSEACT', Length('#ELSEACT')) // -
          or CompareLStr(s34, '#OR', Length('#OR'))) then
        begin
          bo8D := True;
        end
        else
        begin
          if bo8D then
          begin
            // 将Define 好的常量换成指定值
            for n20 := 0 to DefineList.Count - 1 do
            begin
              DefineInfo := DefineList.Items[n20];
              n1C := 0;
              while (True) do
              begin
                n24 := pos(DefineInfo.sName, UpperCase(s34));
                if n24 <= 0 then
                  Break;

                s58 := Copy(s34, 1, n24 - 1);
                s5C := Copy(s34, Length(DefineInfo.sName) + n24, 256);
                s34 := s58 + DefineInfo.sText + s5C;
                LoadList.Strings[I] := s34;
                Inc(n1C);

                if n1C >= 10 then
                  Break;
              end;
            end; // 将Define 好的常量换成指定值
          end;
        end;
      end;
    end;
  end;

  // 常量处理
  // 释放常量定义内容
  for I := 0 to DefineList.Count - 1 do
    DisPose(pTDefineInfo(DefineList.Items[I]));
  DefineList.Free;

  // 释放常量定义内容
  Script := nil;
  SayingRecord := nil;
  nQuestIdx := 0;
  SayingProcedure := nil;

  ProcessCommandText(LoadList);

  for I := 0 to LoadList.Count - 1 do
  begin
    s34 := Trim(LoadList.Strings[I]);
    if (s34 = '') or (s34[1] = ';') or (s34[1] = '/') then
      Continue;

    if (n6C = 0) and (boFlag) then
    begin
      // 物品价格倍率
      if s34[1] = '%' then
      begin // 0048BA57
        s34 := Copy(s34, 2, Length(s34) - 1);
        nPriceRate := StrToIntDef(s34, -1);
        if nPriceRate >= 55 then
          TMerchant(NPC).m_nPriceRate := nPriceRate;

        Continue;
      end;

      // 物品交易类型
      if s34[1] = '+' then
      begin
        s34 := Copy(s34, 2, Length(s34) - 1);
        nItemType := StrToIntDef(s34, -1);
        if nItemType >= 0 then
          TMerchant(NPC).m_ItemTypeList.Add(Pointer(nItemType));

        Continue;
      end;

      // 增加处理NPC可执行命令设置
      if s34[1] = '(' then
      begin
        ArrestStringEx(s34, '(', ')', s34);
        if s34 <> '' then
        begin
          while (s34 <> '') do
          begin
            s34 := GetValidStr3(s34, s30, [' ', ',', #9]);
            nIndex := g_NpcProcessCommand.IndexOf(s30);
            if nIndex >= 0 then
            begin
              nIndex := Integer(g_NpcProcessCommand.Objects[nIndex]);
              case nIndex of
                nNF_Buy:
                  begin
                    TMerchant(NPC).m_boBuy := True;
                    Continue;
                  end;
                nNF_Sell:
                  begin
                    TMerchant(NPC).m_boSell := True;
                    Continue;
                  end;
                nNF_Trading:
                  begin
                    if g_nKey_Trading <> 0 then
                    begin
                      TMerchant(NPC).m_boBuy := True;
                      TMerchant(NPC).m_boSell := True;
                    end;
                    Continue;
                  end;
                nNF_MakedUrg:
                  begin
                    TMerchant(NPC).m_boMakeDrug := True;
                    Continue;
                  end;
                nNF_Prices:
                  begin
                    TMerchant(NPC).m_boPrices := True;
                    Continue;
                  end;
                nNF_Storage:
                  begin
                    TMerchant(NPC).m_boStorage := True;
                    Continue;
                  end;
                nNF_Storage2:
                  begin
                    TMerchant(NPC).m_boStorage := True;
                    Continue;
                  end;
                nNF_Storage3:
                  begin
                    TMerchant(NPC).m_boStorage := True;
                    Continue;
                  end;
                nNF_Storage4:
                  begin
                    TMerchant(NPC).m_boStorage := True;
                    Continue;
                  end;
                nNF_Getback:
                  begin
                    TMerchant(NPC).m_boGetback := True;
                    Continue;
                  end;
                nNF_Getback2:
                  begin
                    TMerchant(NPC).m_boGetback := True;
                    Continue;
                  end;
                nNF_Getback3:
                  begin
                    TMerchant(NPC).m_boGetback := True;
                    Continue;
                  end;
                nNF_Getback4:
                  begin
                    TMerchant(NPC).m_boGetback := True;
                    Continue;
                  end;
                nNF_UpgradeNow:
                  begin
                    TMerchant(NPC).m_boUpgradenow := True;
                    Continue;
                  end;
                nNF_GetBackupgNow:
                  begin
                    TMerchant(NPC).m_boGetBackupgnow := True;
                    Continue;
                  end;
                nNF_Repair:
                  begin
                    TMerchant(NPC).m_boRepair := True;
                    Continue;
                  end;
                nNF_ArmRemoveStone:
                  begin
                    TMerchant(NPC).m_boArmRemoveStone := True;
                    Continue;
                  end;
                nNF_SuperRepair:
                  begin
                    TMerchant(NPC).m_boS_repair := True;
                    Continue;
                  end;
                nNF_SendMsg:
                  begin
                    TMerchant(NPC).m_boSendmsg := True;
                    Continue;
                  end;
                nNF_UseItemName:
                  begin
                    TMerchant(NPC).m_boUseItemName := True;
                    Continue;
                  end;
                nNF_OfflineMsg:
                  begin
                    TMerchant(NPC).m_boofflinemsg := True;
                    Continue;
                  end;
                nNF_DealGold:
                  begin
                    TMerchant(NPC).m_boDealGold := True;
                    Continue;
                  end;
                nNF_BigStorage:
                  begin
                    TMerchant(NPC).m_boBigStorage := True;
                    Continue;
                  end;
                nNF_BigGetback:
                  begin
                    TMerchant(NPC).m_boBigGetBack := True;
                    Continue;
                  end;
                nNF_GetPreviousPage:
                  begin
                    TMerchant(NPC).m_boGetPreviousPage := True;
                    Continue;
                  end;
                nNF_GetNextPage:
                  begin
                    TMerchant(NPC).m_boGetNextPage := True;
                    Continue;
                  end;
                nNF_CreateHero:
                  begin
                    TMerchant(NPC).m_boCreateHeroName := True;
                    Continue;
                  end;
                nNF_UpgradeNew:
                  begin
                    TMerchant(NPC).m_boUpgradeNew := True;
                    Continue;
                  end;
                nNF_PlayDrink:
                  begin
                    TMerchant(NPC).m_boPleaseDrink := True;
                    Continue;
                  end;
                nNF_PlayMakeWine:
                  begin
                    TMerchant(NPC).m_boMakeWine := True;
                    Continue;
                  end;
                nNF_CreateDeputy:
                  begin
                    TMerchant(NPC).m_boBuHero := True;
                    Continue;
                  end;
                nNF_ReclaimItem:
                  begin
                    TMerchant(NPC).m_boReclaimItem := True;
                    Continue;
                  end;
              end;
            end;
          end;
        end;
        Continue;
      end
    end;

    if s34[1] = '{' then
    begin
      if CompareLStr(s34, '{Quest', Length('{Quest')) then
      begin
        s38 := GetValidStr3(s34, s3C, [' ', '}', #9]);
        GetValidStr3(s38, s3C, [' ', '}', #9]);
        n70 := StrToIntDef(s3C, 0);
        Script := MakeNewScript();
        Script.nQuest := n70;
        Inc(n70);
      end;

      if CompareLStr(s34, '{~Quest', Length('{~Quest')) then
        Continue;
    end;

    if (n6C = 1) and (Script <> nil) and (s34[1] = '#') then
    begin
      s38 := GetValidStr3(s34, s3C, ['=', ' ', #9]);
      Script.boQuest := True;
      if CompareLStr(s34, '#IF', Length('#IF')) then
      begin
        ArrestStringEx(s34, '[', ']', s40);
        Script.QuestInfo[nQuestIdx].wFlag := StrToIntDef(s40, 0);
        GetValidStr3(s38, s44, ['=', ' ', #9]);
        n24 := StrToIntDef(s44, 0);
        if n24 <> 0 then
          n24 := 1;

        Script.QuestInfo[nQuestIdx].btValue := n24;
      end;

      if CompareLStr(s34, '#RAND', Length('#RAND')) then
        Script.QuestInfo[nQuestIdx].nRandRage := StrToIntDef(s44, 0);

      Continue;
    end;

    if s34[1] = '[' then
    begin
      n6C := 10;
      if Script = nil then
      begin
        Script := MakeNewScript();
        Script.nQuest := n70;
      end;

      if CompareText(s34, '[goods]') = 0 then
      begin
        n6C := 20;
        Continue;
      end;

      s34 := ArrestStringEx(s34, '[', ']', s74);
      New(SayingRecord);
      SayingRecord.ProcedureList := TList.Create;
      SayingRecord.sLabel := s74;
      s34 := GetValidStrCap(s34, s74, [' ', #9]);

      SayingRecord.boExtJmp := CompareText(s74, 'TRUE') = 0;

      New(SayingProcedure);
      SayingRecord.ProcedureList.Add(SayingProcedure);
      SayingProcedure.ConditionList := TConditionList.Create;
      SayingProcedure.ActionList := TList.Create;
      SayingProcedure.sSayMsg := '';
      SayingProcedure.ElseActionList := TList.Create;
      SayingProcedure.sElseSayMsg := '';
      Script.RecordList.Add(SayingRecord);

      Continue;
    end;

    if (Script <> nil) and (SayingRecord <> nil) then
    begin
      if (n6C >= 10) and (n6C < 20) and (s34[1] = '#') then
      begin
        boIF := AnsiStartsText('#IF', s34);
        boOR := SameText('#OR', s34);

        if boIF then // 适配 #IF (X) 格式 Cursor 2023-07-12 15:05:48
        begin
          if pos('(', s34) > 0 then
          begin
            ArrestStringEx(s34, '(', ')', tmpStr);
            SayingProcedure.ConditionList.TrueCount := StrToIntDef(Trim(tmpStr), 0);
          end;
        end;

        if boIF or boOR then
        begin
          if (SayingProcedure <> nil) and (SayingProcedure.ConditionList.Count > 0) or (SayingProcedure.sSayMsg <> '') then
          begin
            New(SayingProcedure);
            SayingRecord.ProcedureList.Add(SayingProcedure);
            SayingProcedure.ConditionList := TConditionList.Create;
            if boOR then
              SayingProcedure.ConditionList.ConditionType := ct_or
            else
              SayingProcedure.ConditionList.ConditionType := ct_and;

            SayingProcedure.ActionList := TList.Create;
            SayingProcedure.sSayMsg := '';
            SayingProcedure.ElseActionList := TList.Create;
            SayingProcedure.sElseSayMsg := '';
          end
          else if (SayingProcedure.ConditionList.Count <= 0) and (SayingProcedure.ActionList.Count > 0) then
          begin // 修改#IF #ACT 之间没有检测命令，然后下面的脚步存在检测命令会有问题
            New(SayingProcedure);
            SayingRecord.ProcedureList.Add(SayingProcedure);
            SayingProcedure.ConditionList := TConditionList.Create;
            if boOR then
              SayingProcedure.ConditionList.ConditionType := ct_or
            else
              SayingProcedure.ConditionList.ConditionType := ct_and;

            SayingProcedure.ActionList := TList.Create;
            SayingProcedure.sSayMsg := '';
            SayingProcedure.ElseActionList := TList.Create;
            SayingProcedure.sElseSayMsg := '';
          end
          else
          begin
            if boOR then
              SayingProcedure.ConditionList.ConditionType := ct_or
            else
              SayingProcedure.ConditionList.ConditionType := ct_and;
          end;
          n6C := 11;
        end;

        if CompareText(s34, '#ACT') = 0 then
          n6C := 12;

        if CompareText(s34, '#SAY') = 0 then
          n6C := 10;

        if CompareText(s34, '#ELSEACT') = 0 then
          n6C := 13;

        if CompareText(s34, '#ELSESAY') = 0 then
          n6C := 14;

        Continue;
      end;

      if (n6C = 10) and (SayingProcedure <> nil) then
        SayingProcedure.sSayMsg := SayingProcedure.sSayMsg + s34;

      if (n6C = 11) then
      begin
        New(QuestConditionInfo);
        FillChar(QuestConditionInfo^, SizeOf(TQuestConditionInfo), #0);
        if QuestCondition(Trim(s34), QuestConditionInfo) then
        begin
          SayingProcedure.ConditionList.Add(QuestConditionInfo);
        end
        else
        begin
          DisPose(QuestConditionInfo);
          MainOutMessage(Format('脚本错误：“%s” 文件：%s[%d]', [s34, sScritpFileName, I + 1]));
        end;
      end;

      if (n6C = 12) then
      begin
        New(QuestActionInfo);
        FillChar(QuestActionInfo^, SizeOf(TQuestActionInfo), #0);
        if QuestAction(Trim(s34), QuestActionInfo) then
        begin
          SayingProcedure.ActionList.Add(QuestActionInfo);
        end
        else
        begin
          DisPose(QuestActionInfo);
          MainOutMessage(Format('脚本错误：“%s” 文件：%s[%d]', [s34, sScritpFileName, I + 1]));
        end;
      end;

      if (n6C = 13) then
      begin
        New(QuestActionInfo);
        FillChar(QuestActionInfo^, SizeOf(TQuestActionInfo), #0);
        if QuestAction(Trim(s34), QuestActionInfo) then
        begin
          SayingProcedure.ElseActionList.Add(QuestActionInfo);
        end
        else
        begin
          DisPose(QuestActionInfo);
          MainOutMessage(Format('脚本错误：“%s” 文件：%s[%d]', [s34, sScritpFileName, I + 1]));
        end;
      end;

      if (n6C = 14) then
        SayingProcedure.sElseSayMsg := SayingProcedure.sElseSayMsg + s34;
    end;

    if (n6C = 20) and boFlag then
    begin
      s34 := GetValidStrCap(s34, s48, [' ', #9]);
      s34 := GetValidStrCap(s34, s4C, [' ', #9]);
      s34 := GetValidStrCap(s34, s50, [' ', #9]);
      if (s48 <> '') and (s50 <> '') then
      begin
        New(Goods);
        if (s48 <> '') and (s48[1] = '"') then
          ArrestStringEx(s48, '"', '"', s48);

        Goods.sItemName := s48;
        // NPC销售物品列表 [goods] 限制刷新数量不超过1000
        Goods.nCount := Min(StrToIntDef(s4C, 0), 1000);
        Goods.dwRefillTime := StrToIntDef(s50, 0);
        Goods.dwRefillTick := 0;
        TMerchant(NPC).m_RefillGoodsList.Add(Goods);
      end;
    end;
  end;

  LoadList.Free;
  NPC.DoSort;
  Result := 1;
end;

function TFrmDB.CheckScriptFileChanged(NPC: TNormNpc; IsMerchant: Boolean): Boolean;
var
  sPatch, sScritpName: string;
  sTempFileName, sScritpFileName: string;
  I: Integer;
  TempCRC: LongWord;
begin
  Result := False;
  if g_boStopRun then
    Exit;

  if IsMerchant and (NPC is TMerchant) then
  begin
    sPatch := sMarket_Def;
    if TMerchant(NPC).m_boFB then
      sScritpName := TMerchant(NPC).m_sScript + '-' + TMerchant(NPC).m_sFBName
    else
      sScritpName := TMerchant(NPC).m_sScript + '-' + TMerchant(NPC).m_sMapName;

    sTempFileName := sPatch + sScritpName + '.txt';
    sScritpFileName := g_Config.sEnvirDir + sTempFileName;
  end
  else
  begin
    if NPC.m_boIsQuest then
    begin
      sPatch := NPC.m_sFilePath;
      sScritpName := NPC.m_sCharName + '-' + NPC.m_sMapName;
    end
    else
    begin
      sPatch := NPC.m_sFilePath;
      sScritpName := NPC.m_sCharName;
    end;

    if sPatch = '' then
      sPatch := sNpc_def;

    sTempFileName := sPatch + sScritpName + '.txt';
    sScritpFileName := g_Config.sEnvirDir + sTempFileName;
  end;

  if FileExists(sScritpFileName) then
  begin
    TempCRC := GetScriptFileCRC(sScritpFileName);
    if NPC.m_dwScriptCRC <> TempCRC then
    begin
      Result := True;
      Exit;
    end;

    for I := 0 to NPC.m_CallFileListCRC.Count - 1 do
    begin
      sScritpFileName := NPC.m_CallFileListCRC.Strings[I];
      TempCRC := GetScriptFileCRC(sScritpFileName);
      if LongWord(NPC.m_CallFileListCRC.Objects[I]) <> TempCRC then
      begin
        Result := True;
        Exit;
      end;
    end;
  end
  else
    Result := True;
end;

function TFrmDB.SaveGoodRecord(NPC: TMerchant; sFile: string): Integer;
var
  I, II: Integer;
  sFileName: string;
  FileHandle: Integer;
  UserItem: pTUserItem;
  List: TList;
  Header420: TGoodFileHeader;
begin
  Result := -1;
  sFileName := '.\Envir\Market_Saved\' + sFile + '.sav';
  if FileExists(sFileName) then
  begin
    FileHandle := FileOpen(sFileName, fmOpenWrite or fmShareDenyNone);
  end
  else
  begin
    if not DirectoryExists(ExtractFilePath(sFileName)) then
      ForceDirectories(ExtractFilePath(sFileName)); // 创建多级目录 -- piaoyun 2013-5-26

    FileHandle := FileCreate(sFileName);
  end;

  if FileHandle > 0 then
  begin
    FillChar(Header420, SizeOf(TGoodFileHeader), #0);
    for I := 0 to NPC.m_GoodsList.Count - 1 do
    begin
      List := TList(NPC.m_GoodsList.Items[I]);
      Inc(Header420.nItemCount, List.Count);
    end;

    FileWrite(FileHandle, Header420, SizeOf(TGoodFileHeader));
    for I := 0 to NPC.m_GoodsList.Count - 1 do
    begin
      List := TList(NPC.m_GoodsList.Items[I]);
      for II := 0 to List.Count - 1 do
      begin
        UserItem := List.Items[II];
        FileWrite(FileHandle, UserItem^, SizeOf(TUserItem));
      end;
    end;

    FileClose(FileHandle);
    Result := 1;
  end;
end;

function TFrmDB.SaveGoodPriceRecord(NPC: TMerchant; sFile: string): Integer; // 0048CA64
var
  I: Integer;
  sFileName: string;
  FileHandle: Integer;
  ItemPrice: pTItemPrice;
  Header420: TGoodFileHeader;
begin
  Result := -1;
  sFileName := '.\Envir\Market_Prices\' + sFile + '.prc';
  if FileExists(sFileName) then
  begin
    FileHandle := FileOpen(sFileName, fmOpenWrite or fmShareDenyNone);
  end
  else
  begin
    if not DirectoryExists(ExtractFilePath(sFileName)) then
      ForceDirectories(ExtractFilePath(sFileName)); // 创建多级目录 -- piaoyun 2013-5-26

    FileHandle := FileCreate(sFileName);
  end;

  if FileHandle > 0 then
  begin
    FillChar(Header420, SizeOf(TGoodFileHeader), #0);
    Header420.nItemCount := NPC.m_ItemPriceList.Count;
    FileWrite(FileHandle, Header420, SizeOf(TGoodFileHeader));
    for I := 0 to NPC.m_ItemPriceList.Count - 1 do
    begin
      ItemPrice := NPC.m_ItemPriceList.Items[I];
      FileWrite(FileHandle, ItemPrice^, SizeOf(TItemPrice));
    end;
    FileClose(FileHandle);
    Result := 1;
  end;
end;

procedure TFrmDB.ReloadMerchants;
var
  I, II, nX, nY, n100: Integer;
  sLineText, sFileName, sScript, sMapName, sX, sY, sCharName, sFlag, sAppr, sCastle, sCanMove, sMoveTime, sAutoChangeColor,
    sAutoChangeColorTime, sDataFile: string;
  Merchant: TMerchant;
  LoadList: TStringList;
  boNewNpc: Boolean;
  FBList: TList;
  Index, J: Integer;
  Envir: TEnvirnoment;
  sMovePoints, sData: string;
  PointList: TStringList;
  nCount, M: Integer;
  PlayObject: TPlayobject;
begin
  sFileName := g_Config.sEnvirDir + 'Merchant.txt';
  if not FileExists(sFileName) then
    Exit;

  PointList := TStringList.Create;
  UserEngine.m_MerchantList.LockW(1);
  try
    for I := 0 to UserEngine.m_MerchantList.Count - 1 do
    begin
      Merchant := TMerchant(UserEngine.m_MerchantList.Items[I]);
      if (not Merchant.m_boPlug) //
        and (Merchant <> g_ManageNPC) //
        and (Merchant <> g_RobotNPC) //
        and (Merchant <> g_FunctionNPC) //
        and (Merchant <> g_MissionNPC) //
        and (Merchant <> g_BatterNPC) then
        Merchant.m_nFlag := -1; // 初始化状态，用于在重载时，删除 Merchant.txt 中去除的NPC
    end;

    LoadList := TStringListEx.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := Trim(LoadList.Strings[I]);
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sScript, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sMapName, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sX, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sY, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sCharName, [' ', #9]);
        if (sCharName <> '') and (sCharName[1] = '"') then
          ArrestStringEx(sCharName, '"', '"', sCharName);

        sLineText := GetValidStr3(sLineText, sFlag, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAppr, [' ', #9]);
        sMovePoints := '';
        Index := pos('[', sAppr);
        if Index > 0 then
        begin
          ArrestStringEx(sAppr, '[', ']', sMovePoints);
          sAppr := Copy(sAppr, 1, Index - 1);
        end;

        sLineText := GetValidStr3(sLineText, sCastle, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sCanMove, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sMoveTime, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAutoChangeColor, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sAutoChangeColorTime, [' ', #9]);
        sLineText := GetValidStr3(sLineText, sDataFile, [' ', #9]);
        nX := StrToIntDef(sX, 0);
        nY := StrToIntDef(sY, 0);
        boNewNpc := True;

        { 副本地图 -- 重新载入副本地图 chongchong }
        if (Length(sMapName) > 0) and (sMapName[1] = '$') then
        begin
          sMapName := Copy(sMapName, 2, MaxInt);
          Index := g_FBMapManager.IndexOf(sMapName);
          if Index <> -1 then
          begin
            FBList := TList(g_FBMapManager.Objects[Index]);
            for J := 0 to FBList.Count - 1 do
            begin
              Envir := FBList.Items[J];
              for II := 0 to UserEngine.m_MerchantList.Count - 1 do
              begin
                Merchant := TMerchant(UserEngine.m_MerchantList.Items[II]);
                if (Merchant.m_nFlag < 0) then
                begin
                  if (Merchant.m_sMapName = Envir.sMapName) and
                    (((Merchant.m_nCurrX = nX) and (Merchant.m_nCurrY = nY)) or
                    ((Merchant.m_wAppr >= 10000) and (Length(Merchant.MovePoint) > 0) and (Merchant.MovePoint[0].X = nX) and
                    (Merchant.MovePoint[0].Y = nY))) then
                  begin
                    boNewNpc := False;
                    { TODO -ochongchong -c修改 : NPC对应脚本中的\改为/ 【2013-08-28】 }
                    // Merchant.m_sScript := sScript;
                    Merchant.m_sScript := StringReplace(sScript, '/', '\', [rfReplaceAll]);
                    Merchant.m_sCharName := sCharName;
                    Merchant.m_nFlag := StrToIntDef(sFlag, 0);
                    Merchant.m_wAppr := StrToIntDef(sAppr, 0);
                    Merchant.m_dwMoveTime := StrToIntDef(sMoveTime, 0);
                    Merchant.m_dwNpcAutoChangeColorTime := StrToIntDef(sAutoChangeColorTime, 0) * 1000;
                    Merchant.m_sDataFileName := sDataFile;
                    if CompareLStr(sCharName, '<$STR(', Length('<$STR(')) and (sCharName[Length(sCharName)] = '>') then
                    begin
                      sData := sCharName;
                      sData := ArrestStringEx(sData, '(', ')', sCharName);
                    end;

                    n100 := GetValNameNo(sCharName);
                    case n100 of
                      6000 .. 6999:
                        begin
                          Merchant.m_nGlobalAValIndex := n100 - 6000;
                          Merchant.m_sCharName := g_Config.GlobalAVal[Merchant.m_nGlobalAValIndex];
                        end;
                    end;

                    Merchant.m_boCastle := StrToIntDef(sCastle, 0) <> 0;

                    if (StrToIntDef(sCanMove, 0) <> 0) and (Merchant.m_dwMoveTime > 0) then
                      Merchant.m_boCanMove := True;

                    Merchant.m_boNpcAutoChangeColor := StrToIntDef(sAutoChangeColor, 0) <> 0;
                    if (Merchant.m_wAppr >= 10000) and (Length(sMovePoints) > 0) then
                    begin
                      PointList.Clear;
                      nCount := ExtractStrings(['|'], [], PChar(sMovePoints), PointList);
                      if nCount > 0 then
                      begin
                        sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
                        SetLength(Merchant.MovePoint, nCount + 1);
                        Merchant.MovePoint[0].X := Merchant.m_nCurrX;
                        Merchant.MovePoint[0].Y := Merchant.m_nCurrY;
                        Merchant.MovePoint[1].X := StrToIntDef(sX, 0);
                        Merchant.MovePoint[1].Y := StrToIntDef(sY, 0);
                        for M := 1 to nCount - 1 do
                        begin
                          sY := GetValidStr3(PointList[M], sX, [' ', ',', #9]);
                          Merchant.MovePoint[M + 1].X := StrToIntDef(sX, 0);
                          Merchant.MovePoint[M + 1].Y := StrToIntDef(sY, 0);
                        end;
                      end;
                    end;
                    Break;
                  end;
                end;
              end;

              if boNewNpc then
              begin
                Merchant := TMerchant.Create;
                Merchant.m_sMapName := Envir.sMapName;
                Merchant.m_PEnvir := Envir;
                if Merchant.m_PEnvir <> nil then
                begin
                  Merchant.m_sScript := sScript;
                  Merchant.m_nCurrX := nX;
                  Merchant.m_nCurrY := nY;
                  Merchant.m_sCharName := sCharName;
                  Merchant.m_nFlag := StrToIntDef(sFlag, 0);
                  Merchant.m_wAppr := StrToIntDef(sAppr, 0);
                  Merchant.m_dwMoveTime := StrToIntDef(sMoveTime, 0);
                  Merchant.m_dwNpcAutoChangeColorTime := StrToIntDef(sAutoChangeColorTime, 0) * 1000;
                  Merchant.m_sDataFileName := sDataFile;
                  Merchant.m_boCastle := StrToIntDef(sCastle, 0) <> 0;

                  if (StrToIntDef(sCanMove, 0) <> 0) and (Merchant.m_dwMoveTime > 0) then
                    Merchant.m_boCanMove := True;

                  Merchant.m_boNpcAutoChangeColor := StrToIntDef(sAutoChangeColor, 0) <> 0;
                  if CompareLStr(sCharName, '<$STR(', Length('<$STR(')) and (sCharName[Length(sCharName)] = '>') then
                  begin
                    sData := sCharName;
                    sData := ArrestStringEx(sData, '(', ')', sCharName);
                  end;

                  n100 := GetValNameNo(sCharName);
                  case n100 of
                    6000 .. 6999:
                      begin
                        Merchant.m_nGlobalAValIndex := n100 - 6000;
                        Merchant.m_sCharName := g_Config.GlobalAVal[Merchant.m_nGlobalAValIndex];
                      end;
                  end;

                  if (Merchant.m_wAppr >= 10000) and (Length(sMovePoints) > 0) then
                  begin
                    PointList.Clear;
                    nCount := ExtractStrings(['|'], [], PChar(sMovePoints), PointList);
                    if nCount > 0 then
                    begin
                      sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
                      SetLength(Merchant.MovePoint, nCount + 1);
                      Merchant.MovePoint[0].X := Merchant.m_nCurrX;
                      Merchant.MovePoint[0].Y := Merchant.m_nCurrY;
                      Merchant.MovePoint[1].X := StrToIntDef(sX, 0);
                      Merchant.MovePoint[1].Y := StrToIntDef(sY, 0);

                      for M := 1 to nCount - 1 do
                      begin
                        sY := GetValidStr3(PointList[M], sX, [' ', ',', #9]);
                        Merchant.MovePoint[M + 1].X := StrToIntDef(sX, 0);
                        Merchant.MovePoint[M + 1].Y := StrToIntDef(sY, 0);
                      end;
                    end;
                  end;
                  UserEngine.m_MerchantList.Add(Merchant);
                  Merchant.Initialize;
                end
                else
                  Merchant.Free;
              end;
            end;
          end;
        end
        else // 非副本地图
        begin
          for II := 0 to UserEngine.m_MerchantList.Count - 1 do
          begin
            Merchant := TMerchant(UserEngine.m_MerchantList.Items[II]);
            if (Merchant.m_nFlag < 0) then
            begin
              if (Merchant.m_sMapName = sMapName) and (((Merchant.m_nCurrX = nX) and (Merchant.m_nCurrY = nY)) or
                ((Merchant.m_wAppr >= 10000) and (Length(Merchant.MovePoint) > 0) and (Merchant.MovePoint[0].X = nX) and
                (Merchant.MovePoint[0].Y = nY))) then
              begin
                boNewNpc := False;
                { TODO -ochongchong -c修改 : NPC对应脚本中的\改为/ 【2013-08-28】 }
                // Merchant.m_sScript := sScript;
                Merchant.m_sScript := StringReplace(sScript, '/', '\', [rfReplaceAll]);
                Merchant.m_sCharName := sCharName;
                Merchant.m_nFlag := StrToIntDef(sFlag, 0);
                Merchant.m_wAppr := StrToIntDef(sAppr, 0);
                Merchant.m_dwMoveTime := StrToIntDef(sMoveTime, 0);
                Merchant.m_dwNpcAutoChangeColorTime := StrToIntDef(sAutoChangeColorTime, 0) * 1000;
                Merchant.m_boCastle := StrToIntDef(sCastle, 0) <> 0;

                if (StrToIntDef(sCanMove, 0) <> 0) and (Merchant.m_dwMoveTime > 0) then
                  Merchant.m_boCanMove := True;

                Merchant.m_boNpcAutoChangeColor := StrToIntDef(sAutoChangeColor, 0) <> 0;
                if CompareLStr(sCharName, '<$STR(', Length('<$STR(')) and (sCharName[Length(sCharName)] = '>') then
                begin
                  sData := sCharName;
                  sData := ArrestStringEx(sData, '(', ')', sCharName);
                end;

                n100 := GetValNameNo(sCharName);
                case n100 of
                  6000 .. 6999:
                    begin
                      Merchant.m_nGlobalAValIndex := n100 - 6000;
                      Merchant.m_sCharName := g_Config.GlobalAVal[Merchant.m_nGlobalAValIndex];
                    end;
                end;

                if (Merchant.m_wAppr >= 10000) and (Length(sMovePoints) > 0) then
                begin
                  PointList.Clear;
                  nCount := ExtractStrings(['|'], [], PChar(sMovePoints), PointList);
                  if nCount > 0 then
                  begin
                    sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
                    SetLength(Merchant.MovePoint, nCount + 1);
                    Merchant.MovePoint[0].X := Merchant.m_nCurrX;
                    Merchant.MovePoint[0].Y := Merchant.m_nCurrY;
                    Merchant.MovePoint[1].X := StrToIntDef(sX, 0);
                    Merchant.MovePoint[1].Y := StrToIntDef(sY, 0);

                    for M := 1 to nCount - 1 do
                    begin
                      sY := GetValidStr3(PointList[M], sX, [' ', ',', #9]);
                      Merchant.MovePoint[M + 1].X := StrToIntDef(sX, 0);
                      Merchant.MovePoint[M + 1].Y := StrToIntDef(sY, 0);
                    end;
                  end;
                end;
                Break;
              end;
            end;
          end;

          if boNewNpc then
          begin
            Merchant := TMerchant.Create;
            Merchant.m_sMapName := sMapName;
            Merchant.m_PEnvir := g_MapManager.FindMap(Merchant.m_sMapName);
            if Merchant.m_PEnvir <> nil then
            begin
              Merchant.m_sScript := sScript;
              Merchant.m_nCurrX := nX;
              Merchant.m_nCurrY := nY;
              Merchant.m_sCharName := sCharName;
              Merchant.m_nFlag := StrToIntDef(sFlag, 0);
              Merchant.m_wAppr := StrToIntDef(sAppr, 0);
              Merchant.m_dwMoveTime := StrToIntDef(sMoveTime, 0);
              Merchant.m_dwNpcAutoChangeColorTime := StrToIntDef(sAutoChangeColorTime, 0) * 1000;

              Merchant.m_boCastle := StrToIntDef(sCastle, 0) <> 0;

              if (StrToIntDef(sCanMove, 0) <> 0) and (Merchant.m_dwMoveTime > 0) then
                Merchant.m_boCanMove := True;

              Merchant.m_boNpcAutoChangeColor := StrToIntDef(sAutoChangeColor, 0) <> 0;
              if CompareLStr(sCharName, '<$STR(', Length('<$STR(')) and (sCharName[Length(sCharName)] = '>') then
              begin
                sData := sCharName;
                sData := ArrestStringEx(sData, '(', ')', sCharName);
              end;

              n100 := GetValNameNo(sCharName);
              case n100 of
                6000 .. 6999:
                  begin
                    Merchant.m_nGlobalAValIndex := n100 - 6000;
                    Merchant.m_sCharName := g_Config.GlobalAVal[Merchant.m_nGlobalAValIndex];
                  end;
              end;

              if (Merchant.m_wAppr >= 10000) and (Length(sMovePoints) > 0) then
              begin
                PointList.Clear;
                nCount := ExtractStrings(['|'], [], PChar(sMovePoints), PointList);
                if nCount > 0 then
                begin
                  sY := GetValidStr3(PointList[0], sX, [' ', ',', #9]);
                  SetLength(Merchant.MovePoint, nCount + 1);
                  Merchant.MovePoint[0].X := Merchant.m_nCurrX;
                  Merchant.MovePoint[0].Y := Merchant.m_nCurrY;
                  Merchant.MovePoint[1].X := StrToIntDef(sX, 0);
                  Merchant.MovePoint[1].Y := StrToIntDef(sY, 0);
                  for M := 1 to nCount - 1 do
                  begin
                    sY := GetValidStr3(PointList[M], sX, [' ', ',', #9]);
                    Merchant.MovePoint[M + 1].X := StrToIntDef(sX, 0);
                    Merchant.MovePoint[M + 1].Y := StrToIntDef(sY, 0);
                  end;
                end;
              end;
              UserEngine.m_MerchantList.Add(Merchant);
              Merchant.Initialize;
            end
            else
              Merchant.Free;
          end;
        end;
      end;
    end;
    LoadList.Free;

    for I := UserEngine.m_MerchantList.Count - 1 downto 0 do
    begin
      Merchant := TMerchant(UserEngine.m_MerchantList.Items[I]);
      if Merchant.m_nFlag = -1 then // 此处原本有一逻辑判断，不删除镜像地图NPC，去掉了。Cursor 2023-08-08 09:50:06
      begin
        Merchant.MakeGhost;
        if not Merchant.m_boIsHide then
          Merchant.SendRefMsg(RM_DISAPPEAR, 0, 0, 0, 0, '');

        // Merchant.m_boGhost := True;
        // Merchant.m_dwGhostTick := MyGetTickCount();
        // UserEngine.m_MerchantList.Delete(I);
      end;
    end;
  finally
    UserEngine.m_MerchantList.UnLockW;
  end;
  PointList.Free;

  for I := 0 to UserEngine.m_PlayObjectList.Count - 1 do
  begin
    PlayObject := TPlayobject(UserEngine.m_PlayObjectList.Objects[I]);
    if PlayObject <> nil then
    begin
      PlayObject.m_NPC := nil;
      PlayObject.m_ItemBoxNpc := nil;
    end;
  end;

  ResetCustomNpcList;
end;

function TFrmDB.LoadUpgradeWeaponRecord(sNPCName: string; DataList: TList): Integer; // 0048CBD0
var
  I: Integer;
  FileHandle: Integer;
  sFileName: string;
  UpgradeInfo: pTUpgradeInfo;
  UpgradeRecord: TUpgradeInfo;
  nRecordCount: Integer;
begin
  Result := -1;
  sFileName := '.\Envir\Market_Upg\' + sNPCName + '.upg';
  if FileExists(sFileName) then
  begin
    FileHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
    if FileHandle > 0 then
    begin
      FileRead(FileHandle, nRecordCount, SizeOf(Integer));
      for I := 0 to nRecordCount - 1 do
      begin
        if FileRead(FileHandle, UpgradeRecord, SizeOf(TUpgradeInfo)) = SizeOf(TUpgradeInfo) then
        begin
          New(UpgradeInfo);
          UpgradeInfo^ := UpgradeRecord;
          UpgradeInfo.dwGetBackTick := 0;
          DataList.Add(UpgradeInfo);
        end;
      end;
      FileClose(FileHandle);
      Result := 1;
    end;
  end;
end;

function TFrmDB.SaveUpgradeWeaponRecord(sNPCName: string; DataList: TList): Integer;
var
  I: Integer;
  FileHandle: Integer;
  sFileName: string;
  UpgradeInfo: pTUpgradeInfo;
begin
  Result := -1;
  sFileName := '.\Envir\Market_Upg\' + sNPCName + '.upg';
  if FileExists(sFileName) then
  begin
    FileHandle := FileOpen(sFileName, fmOpenWrite or fmShareDenyNone);
  end
  else
  begin
    if not DirectoryExists(ExtractFilePath(sFileName)) then
    begin
      ForceDirectories(ExtractFilePath(sFileName)); // 创建多级目录 -- piaoyun 2013-5-26
    end;
    FileHandle := FileCreate(sFileName);
  end;

  if FileHandle > 0 then
  begin
    FileWrite(FileHandle, DataList.Count, SizeOf(Integer));
    for I := 0 to DataList.Count - 1 do
    begin
      UpgradeInfo := DataList.Items[I];
      FileWrite(FileHandle, UpgradeInfo^, SizeOf(TUpgradeInfo));
    end;
    FileClose(FileHandle);
    Result := 1;
  end;
end;

function TFrmDB.LoadGoodRecord(NPC: TMerchant; sFile: string): Integer; // 0048C574
var
  I: Integer;
  sFileName: string;
  FileHandle: Integer;
  UserItem: pTUserItem;
  List: TList;
  Header420: TGoodFileHeader;
begin
  Result := -1;
  sFileName := '.\Envir\Market_Saved\' + sFile + '.sav';
  if FileExists(sFileName) then
  begin
    FileHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
    List := nil;
    if FileHandle > 0 then
    begin
      if FileRead(FileHandle, Header420, SizeOf(TGoodFileHeader)) = SizeOf(TGoodFileHeader) then
      begin
        for I := 0 to Header420.nItemCount - 1 do
        begin
          New(UserItem);
          if FileRead(FileHandle, UserItem^, SizeOf(TUserItem)) = SizeOf(TUserItem) then
          begin
            if List = nil then
            begin
              List := TList.Create;
              List.Add(UserItem)
            end
            else
            begin
              if pTUserItem(List.Items[0]).wIndex = UserItem.wIndex then
              begin
                List.Add(UserItem);
              end
              else
              begin
                NPC.m_GoodsList.Add(List);
                List := TList.Create;
                List.Add(UserItem);
              end;
            end;
          end;
        end;

        if List <> nil then
          NPC.m_GoodsList.Add(List);

        FileClose(FileHandle);
        Result := 1;
      end;
    end;
  end;
end;

function TFrmDB.LoadGoodPriceRecord(NPC: TMerchant; sFile: string): Integer; // 0048C918
var
  I: Integer;
  sFileName: string;
  FileHandle: Integer;
  ItemPrice: pTItemPrice;
  Header420: TGoodFileHeader;
begin
  Result := -1;
  sFileName := '.\Envir\Market_Prices\' + sFile + '.prc';
  if FileExists(sFileName) then
  begin
    FileHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
    if FileHandle > 0 then
    begin
      if FileRead(FileHandle, Header420, SizeOf(TGoodFileHeader)) = SizeOf(TGoodFileHeader) then
      begin
        for I := 0 to Header420.nItemCount - 1 do
        begin
          New(ItemPrice);
          if FileRead(FileHandle, ItemPrice^, SizeOf(TItemPrice)) = SizeOf(TItemPrice) then
          begin
            NPC.m_ItemPriceList.Add(ItemPrice);
          end
          else
          begin
            DisPose(ItemPrice);
            Break;
          end;
        end;
      end;
      FileClose(FileHandle);
      Result := 1;
    end;
  end;
end;

function TFrmDB.DeCodePlugBuffer(MemoryStream: TMemoryStream): AnsiString;
var
  InStream, OutStream: Classes.TMemoryStream;
begin
  Result := '';
  InStream := Classes.TMemoryStream.Create;
  OutStream := Classes.TMemoryStream.Create;
  InStream.SetSize(MemoryStream.Size);
  DecryBufferA_LF(MemoryStream.Memory, MemoryStream.Size, InStream.Memory);
  InStream.Position := 0;
  try
    ZipDecompressStream(InStream, OutStream);
  except
    MainOutMessage('[Exception] HookLoadScriptFile ZipDecompress');
  end;

  if OutStream.Size > 0 then
  begin
    SetLength(Result, OutStream.Size);
    Move(OutStream.Memory^, Result[1], OutStream.Size);
  end;

  InStream.Free;
  OutStream.Free;
  // Result := DecryBufferA(MemoryStream.Memory, MemoryStream.Size);
end;

// 解密脚本 piaoyun 2013-12-18
function DeCodeStringEx(sSrc: AnsiString): AnsiString;
var
  pDest: PAnsiChar;
  nMemSize, nDest: DWORD;
begin
  if Trim(sSrc) = '' then
  begin
    Result := sSrc;
    Exit;
  end;
  if (g_PluginManager <> nil) and g_PluginManager.IsCheckDecryptScriptLineHook then
  begin
    nMemSize := Length(sSrc) * 2;
    GetMem(pDest, nMemSize);
    nDest := nMemSize;
    if g_PluginManager.HookDecryptScriptLine(PAnsiChar(sSrc), Length(sSrc), pDest, nDest) then
    begin
      SetLength(sSrc, nDest);
      Move(pDest^, sSrc[1], nDest);
    end;
    FreeMem(pDest, nMemSize);
  end;
  Result := sSrc;
end;

procedure TFrmDB.DeCodeStringList(StringList: TStringList);
var
  I: Integer;
  sLine: string;
  S: AnsiString;
  DecrpytType: Integer;
  pDest: PAnsiChar;
  nMemSize, nDestSize: DWORD;
begin
  DecrpytType := 0;
  if StringList.Count > 0 then
  begin
    sLine := StringList.Strings[0];
    if SameText(sLine, sENCYPTSCRIPTFLAG) then
      DecrpytType := 1
    else if SameText(sLine, sENCYPTSCRIPTFILEFLAG) then
      DecrpytType := 2;
  end;

  if DecrpytType > 0 then
  begin
    try
      // 删除标记
      if StringList.Count > 0 then
        StringList.Delete(0);

      if DecrpytType = 1 then
      begin
        for I := 0 to StringList.Count - 1 do
        begin
          sLine := StringList.Strings[I];
          sLine := DeCodeStringEx(sLine);
          StringList.Strings[I] := sLine;
        end;
      end
      else if DecrpytType = 2 then
      begin
        if (g_PluginManager <> nil) and g_PluginManager.IsCheckDecryptScriptFileHook then
        begin
          S := StringList.Text;
          nMemSize := Length(S) * 3;
          GetMem(pDest, nMemSize);
          nDestSize := nMemSize;
          if g_PluginManager.HookDecryptScriptFile(PAnsiChar(S), Length(S), pDest, nDestSize) then
          begin
            SetLength(S, nDestSize);
            Move(pDest^, S[1], nDestSize);
            StringList.Text := S;
          end;
          FreeMem(pDest, nMemSize);
        end;
      end;
    except
      MainOutMessage('DeCodeStringList 异常！');
    end;
  end;
  // 测试用---保存到解密后文件
  // StringList.SaveToFile('.\StringList.txt');
end;

constructor TFrmDB.Create();
begin
  CoInitialize(nil);
  SQLiteDB := TSQLite3Database.Create;
  SQLiteDB.MustExist := True;
{$IF DBTYPE = BDE}
{$IFNDEF CPUX64}
  Query := TQuery.Create(nil);
{$ENDIF}
{$ELSE}
  Query := TADOQuery.Create(nil);
{$IFEND}
{$IFDEF USE_FDTABLE}
  FItemsDataSet := TFDMemTable.Create(nil);
  FMonsterDataSet := TFDMemTable.Create(nil);
{$ELSE}
  FItemsDataSet := TkbmMemTable.Create(nil);
  FMonsterDataSet := TkbmMemTable.Create(nil);
{$ENDIF}
end;

destructor TFrmDB.Destroy;
begin
  SQLiteDB.Free;
{$IFNDEF CPUX64}
  Query.Free;
{$ENDIF}
  CoUnInitialize;
  FItemsDataSet.Free;
  FMonsterDataSet.Free;
  inherited;
end;

procedure TFrmDB.ResetCustomNpcList;
var
  Merchant: TMerchant;
  I, J, InBytes: Integer;
  InBuf: PAnsiChar;
  Index: Integer;
  IsFoundCustomNpc: Boolean;
  CustomNpcConfig: TCustomNpcConfig;
  ClientConfig: PClientCustomNpcConfig;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomNpcList.LockW(1);
  try
{$IFEND}
    for I := UserEngine.m_CustomNpcList.Count - 1 downto 0 do
    begin
      TCustomNpcConfig(UserEngine.m_CustomNpcList.Items[I]).Free;
      UserEngine.m_CustomNpcList.Delete(I)
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomNpcList.UnLockW;
  end;

{$IFEND}
  UserEngine.m_MerchantList.LockR(2);
  try
    for I := 0 to UserEngine.m_MerchantList.Count - 1 do
    begin
      Merchant := UserEngine.m_MerchantList.Items[I];

      if Merchant.m_wAppr >= 10000 then
      begin
        IsFoundCustomNpc := False;
{$IF MULTI_THREAD = 1}
        if g_MultiThreadRun then
          UserEngine.m_CustomNpcList.LockW(2);
        try
{$IFEND}
          for J := 0 to UserEngine.m_CustomNpcList.Count - 1 do
          begin
            CustomNpcConfig := UserEngine.m_CustomNpcList[J];
            if CustomNpcConfig.NpcAppr = Merchant.m_wAppr then
            begin
              IsFoundCustomNpc := True;
              Break;
            end;
          end;

          if not IsFoundCustomNpc then
          begin
            CustomNpcConfig := TCustomNpcConfig.Create(Merchant.m_wAppr);
            UserEngine.m_CustomNpcList.Add(CustomNpcConfig);
          end;
{$IF MULTI_THREAD = 1}
        finally
          if g_MultiThreadRun then
            UserEngine.m_CustomNpcList.UnLockW;
        end;
{$IFEND}
      end;
    end;
  finally
    UserEngine.m_MerchantList.UnLockR;
  end;

{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then
    UserEngine.m_CustomNpcList.LockR(3);
  try
{$IFEND}
    InBytes := UserEngine.m_CustomNpcList.Count * SizeOf(TClientCustomNpcConfig);
    InBuf := AllocMem(InBytes + 1);
    try
      ClientConfig := PClientCustomNpcConfig(InBuf);

      for I := 0 to UserEngine.m_CustomNpcList.Count - 1 do
      begin
        CustomNpcConfig := UserEngine.m_CustomNpcList.Items[I];

        ClientConfig^.wNpcAppr := CustomNpcConfig.NpcAppr;
        ClientConfig^.BaseConfig := CustomNpcConfig.ClientBaseConfig;
        // ClientConfig^.Actions := CustomNpcConfig.DirActions;
        // 将选中的放在前面 chongchong 2016-03-21
        Index := 0;
        for J := Low(CustomNpcConfig.DirActions) to High(CustomNpcConfig.DirActions) do
        begin
          if CustomNpcConfig.DirActions[J].Enabled then
          begin
            ClientConfig^.Actions[Index] := CustomNpcConfig.DirActions[J];
            Inc(Index);
          end;
        end;

        ClientConfig^.wDirCount := Index;

        for J := Low(CustomNpcConfig.DirActions) to High(CustomNpcConfig.DirActions) do
        begin
          if not CustomNpcConfig.DirActions[J].Enabled then
          begin
            ClientConfig^.Actions[Index] := CustomNpcConfig.DirActions[J];
            Inc(Index);
          end;
        end;

        Inc(ClientConfig);
      end;

      g_CustomNpcListTextLen := InBytes;
      g_CustomNpcListText := zLibCompressBuffer(InBuf, InBytes);
      g_CustomNpcListTextCRC := BufferCrc(PAnsiChar(g_CustomNpcListText), Length(g_CustomNpcListText));
    finally
      FreeMemory(InBuf);
    end;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then
      UserEngine.m_CustomNpcList.UnLockR;
  end;
{$IFEND}
end;

function TFrmDB.GetStdItemDBField(ItemIdx: Integer; FieldName: string): string;
begin
  Result := '';
  if FItemsDataSet.Locate('idx', ItemIdx, []) then
    Result := FItemsDataSet.FieldByName(FieldName).AsString;
end;

function TFrmDB.GetMonsterDBField(MonsterName, FieldName: string): string;
begin
  Result := '';
  if FMonsterDataSet.Locate('name', MonsterName, []) then
    Result := FMonsterDataSet.FieldByName(FieldName).AsString;
end;

end.
