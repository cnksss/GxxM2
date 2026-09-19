unit MapUnit;

interface

uses
  Windows,
  Classes,
  SysUtils,
  Graphics,
  HUtil32,
  Grobal2;

type
  // -------------------------------------------------------------------------------
  // Map
  // -------------------------------------------------------------------------------

  TMapPrjInfo = record
    ident:string[16];
    ColCount:Integer;
    RowCount:Integer;
  end;

  TMapHeader = packed record
    wWidth:Word;
    wHeight:Word;
    sTitle:string[15];
    UpdateDate:TDateTime;
    btVersion:Byte;
    Reserved:array[0..22] of Byte;
  end;

  // º«°æ´«Ææ
  TENMapHeader = packed record
    Title:string[16];
    Reserved:LongWord;
    Width:Word;
    Not1:Word;
    Height:Word;
    Not2:Word;
    Reserved2:array[0..24] of char;
  end;

  // ´«Ææ3µØÍ¼ÎÄ¼þÍ·
  TEIMapHeader = packed record
    //Desc: array[0..19] of Char;
    Desc:array[0..4] of Integer;
    wAttr:Word;
    Width:Word;
    Height:Word;
    EventFileIdx:Char;
    FogColor:Char;
  end;

  TMapInfo = packed record
    wBkImg:Word;
    wMidImg:Word;
    wFrImg:Word;
    btDoorIndex:byte; // $80 (¹®Â¦), ¹®ÀÇ ½Äº° ÀÎµ¦½º
    btDoorOffset:byte; // ´ÝÈù ¹®ÀÇ ±×¸²ÀÇ »ó´ë À§Ä¡, $80 (¿­¸²/´ÝÈû(±âº»))
    btAniFrame:byte; // $80(Draw Alpha) +  ÇÁ·¡ÀÓ ¼ö
    btAniTick:byte;
    btArea:byte; // Áö¿ª Á¤º¸
    btLight:byte; // 0..1..4 ±¤¿ø È¿°ú   0..MaxListSize
  end;
  pTMapInfo = ^TMapInfo;

  TNewMapInfo = packed record
    wBkImg:Word;
    wMidImg:Word;
    wFrImg:Word;
    btDoorIndex:Byte; // $80 (¹®Â¦), ¹®ÀÇ ½Äº° ÀÎµ¦½º
    btDoorOffset:Byte; // ´ÝÈù ¹®ÀÇ ±×¸²ÀÇ »ó´ë À§Ä¡, $80 (¿­¸²/´ÝÈû(±âº»))
    btAniFrame:Byte; // $80(Draw Alpha) +  ÇÁ·¡ÀÓ ¼ö
    btAniTick:Byte;
    btArea:Byte; // Áö¿ª Á¤º¸
    btLight:Byte; // 0..1..4 ±¤¿ø È¿°ú   0..MaxListSize
    btUnitBkImg:Byte;
    btUnitMidImg:Byte;
  end;
  pTNewMapInfo = ^TNewMapInfo;

  TReturnMapInfo = packed record
    wBkImg:Word;
    wMidImg:Word;
    wFrImg:Word;
    btDoorIndex:Byte; // $80 (¹®Â¦), ¹®ÀÇ ½Äº° ÀÎµ¦½º
    btDoorOffset:Byte; // ´ÝÈù ¹®ÀÇ ±×¸²ÀÇ »ó´ë À§Ä¡, $80 (¿­¸²/´ÝÈû(±âº»))
    btAniFrame:Byte; // $80(Draw Alpha) +  ÇÁ·¡ÀÓ ¼ö
    btAniTick:Byte;
    btArea:Byte; // Áö¿ª Á¤º¸
    btLight:Byte; // 0..1..4 ±¤¿ø È¿°ú   0..MaxListSize
    btUnitBkImg:Byte;
    btUnitMidImg:Byte;
    wAniTiles:Word;
    Bytes1:array[0..5 - 1] of Byte;
    btAniTilesFrame:Byte;
    btAniTilesTick:Byte;
    btAniType:Byte;
    Bytes2:array[0..12 - 1] of Byte;
  end;
  pTReturnMapInfo = ^TReturnMapInfo;

  // º«°æ´«Ææ
  TENMapInfo = packed record
    BkImg:Word;
    BkImgNot:word;
    MidImg:word;
    FrImg:word;
    DoorIndex:byte;
    DoorOffset:byte;
    AniFrame:byte;
    AniTick:byte;
    Area:byte;
    light:byte;
    btNot:byte;
  end;
  PTENMapInfo = ^TENMapInfo;

  // ´«Ææ3µØÍ¼ - µØ×©
  TEIMapTileInfo = packed record
    btFileIdx:Byte;
    wTileIdx:WORD;
  end;

  // ´«Ææ3µØÍ¼ - µ¥Ôª¸ñ
  TEIMapInfo = packed Record
    btFlag:Byte;
    btObj1Ani:Byte;
    btObj2Ani:Byte; //¶¯»­µÄÕÅÍ¼
    btFileIdx1:Byte; //ÉÏ²ãOBÎÄ¼þºÅ
    btFileIdx2:Byte; //ÏÂ²ãOBÎÄ¼þºÅ
    wObj1:Word; //ÏÂ²ãOBÍ¼Æ¬ºÅ
    wObj2:Word; //ÉÏ²ãOBÍ¼Æ¬ºÅ
    btDoorIdx:Byte;
    btDoorOffset:Byte;
    wLigntNEvent:Word;
    btLigntNEvent1:Byte;
  end;

  TMapCellInfo = packed record
    wBkImg:Integer;
    wMidImg:Word;
    wFrImg:Word;
    btDoorIndex:Byte; // $80 (¹®Â¦), ¹®ÀÇ ½Äº° ÀÎµ¦½º
    btDoorOffset:Byte; // ´ÝÈù ¹®ÀÇ ±×¸²ÀÇ »ó´ë À§Ä¡, $80 (¿­¸²/´ÝÈû(±âº»))
    btAniFrame:Byte; // $80(Draw Alpha) +  ÇÁ·¡ÀÓ ¼ö
    btAniTick:Byte;
    btArea:Byte; // Áö¿ª Á¤º¸
    btLight:Byte; // 0..1..4 ±¤¿ø È¿°ú   0..MaxListSize
    btUnitBkImg:Byte;
    btUnitMidImg:Byte;
    wAniTiles:Word;
    btAniTilesFrame:Byte;
    btAniTilesTick:Byte;
    btAniType:Byte;
  end;
  PTMapCellInfo = ^TMapCellInfo;

  TMapInfoArr = array[0..1000 * 1000 - 1] of TMapInfo; // [0..1000 * 1000 - 1]
  pTMapInfoArr = ^TMapInfoArr;

  TNewMapInfoArr = array[0..1000 * 1000 - 1] of TNewMapInfo; // [0..1000 * 1000 - 1]
  pTNewMapInfoArr = ^TNewMapInfoArr;

  TReturnMapInfoArr = array[0..1000 * 1000 - 1] of TReturnMapInfo; // [0..1000 * 1000 - 1]
  pTReturnMapInfoArr = ^TReturnMapInfoArr;

  TENMapInfoArr = array[0..1000 * 1000 - 1] of TENMapInfo; // [0..1000 * 1000 - 1]
  pTENMapInfoArr = ^TENMapInfoArr;

  TMapInfoArray = array[0..MaxListSize div 4] of Byte;
  PMapInfoArray = ^TMapInfoArray;

  TEIMapTileInfoArr = array[0..MaxListSize] Of TEIMapTileInfo;
  PTEIMapTileInfoArr = ^TEIMapTileInfoArr;

  TEIMapInfoArr = array[0..MaxListSize] of TEIMapInfo;
  pTEIMapInfoArr = ^TEIMapInfoArr;

  TMap = class
  protected //HZQ 20230525 Private UpdateMapSeg never used
    //function LoadMapInfo(sMapFile: string): Boolean;
    procedure UpdateMapSeg(cx, cy:Integer); // , maxsegx, maxsegy: integer);
    procedure LoadMapArr(nCurrX, nCurrY:Integer);
    //procedure SaveMapArr(nCurrX, nCurrY: Integer);
  public
    m_sMapBase:string;
    m_sResourcesMapBase:string;
    m_MArr:array[0..312, 0..312] of TMapCellInfo; // TNewMapInfo;

    m_MArrEIMapTiteInfo:Array[0..40 * 3, 0..40 * 3] Of TEIMapTileInfo;
    m_MArrEIMapInfo:Array[0..40 * 3, 0..40 * 3] Of TEIMapInfo;

    MapCellArray:PMapInfoArray;

    m_boChange:Boolean;
    m_ClientRect:TRect;
    m_OldClientRect:TRect;
    m_nBlockLeft:Integer;
    m_nBlockTop:Integer;

    m_nOldBlockLeft:Integer;
    m_nOldBlockTop:Integer;

    m_nOldLeft:Integer;
    m_nOldTop:Integer;
    m_sOldMap:string;
    m_nCurUnitX:Integer;
    m_nCurUnitY:Integer;
    m_sCurrentMap:string;
    m_sOldCurrentMap:string;
    m_nSegXCount:Integer;
    m_nSegYCount:Integer;

    m_nWidth:Integer;
    m_nHeight:Integer;
    m_btBitCount:Byte;

    m_boStartLoad:Boolean;
    m_boLoadOk:Boolean;

    m_boStartLoadAll:Boolean;
    m_boLoadAllOk:Boolean;
    m_boNewMap:Boolean;
    m_boReturnMap:Boolean;
    m_boLoadMapOk:Boolean;
    m_boENMap:Boolean; // º«¹úµØÍ¼ chongchong 2015-09-12

    m_boEIMap:Boolean;

    m_MapLock:TRTLCriticalSection;
    constructor Create;
    destructor Destroy; override; // Jacky
    procedure Lock;
    procedure UnLock;
    procedure UpdateMapSquare(cx, cy:Integer);
    procedure UpdateMapPos(mx, my:Integer);
    procedure ReadyReload;
    procedure LoadMap(sMapName:string; nMx, nMy:Integer);
    procedure MarkCanWalk(mx, my:Integer; bowalk:Boolean);
    function CanMove(mx, my:Integer):Boolean;
    function NewCanMove(mx, my:Integer):Boolean;
    function CanFly(mx, my:Integer):Boolean;
    function GetDoor(mx, my:Integer):Integer;
    function IsDoorOpen(mx, my:Integer):Boolean;
    function OpenDoor(mx, my:Integer):Boolean;
    function CloseDoor(mx, my:Integer):Boolean;
    procedure LoadAllMap;
    function GetMapCellInfo(nX, nY:Integer):Integer;

    procedure StreamSaveToFile(Sender:TObject; Stream:TMemoryStream; Index:Integer; const FileName:string);
  end;

implementation

uses
  MShare,
  UpdateEngine;

constructor TMap.Create;
begin
  inherited Create;
  InitializeCriticalSection(m_MapLock);
  // GetMem (MInfoArr, sizeof(TMapInfo) * LOGICALMAPUNIT * 3 * LOGICALMAPUNIT * 3);
  m_ClientRect := Rect(0, 0, 0, 0);
  m_boChange := FALSE;
  m_sMapBase := 'Map\';
  m_sResourcesMapBase := g_ResourcesDir + '\Map\'; //'Resources\Map\';
  m_sCurrentMap := '';
  m_sOldCurrentMap := '';
  m_nSegXCount := 0;
  m_nSegYCount := 0;
  m_nCurUnitX := -1;
  m_nCurUnitY := -1;
  m_nBlockLeft := -1;
  m_nBlockTop := -1;
  m_nOldBlockLeft := -1;
  m_nOldBlockTop := -1;
  m_sOldMap := '';

  m_btBitCount := 8;

  m_boStartLoad := False;
  m_boStartLoadAll := False;
  m_boLoadOk := False;
  m_boLoadAllOk := False;

  m_boLoadMapOk := False;
  MapCellArray := nil;
  m_boNewMap := False;
  m_boReturnMap := False; // ¹éÀ´¹ú¼Ê
  m_boENMap := False;
end;

destructor TMap.Destroy;
begin
  if MapCellArray <> nil then begin
    FreeMem(MapCellArray);
    MapCellArray := nil;
  end;
  DeleteCriticalSection(m_MapLock);
  inherited Destroy;
end;

procedure TMap.Lock;
begin
  EnterCriticalSection(m_MapLock);
end;

procedure TMap.UnLock;
begin
  LeaveCriticalSection(m_MapLock);
end;

procedure TMap.UpdateMapSeg(cx, cy:Integer); // , maxsegx, maxsegy: integer);
begin

end;

// ¼ÓÔØµØÍ¼¶ÎÊý¾Ý
// ÒÔµ±Ç°×ù±êÎª×¼

procedure TMap.LoadMapArr(nCurrX, nCurrY:Integer);
const
  XORWORD = $AA38;
  NEWMAPTITLE = 'Map 2010 Ver 1.0';
var
  I:Integer;
  nAline, nC:Integer;
  nLx:Integer;
  nRx:Integer;
  nTy:Integer;
  nBy:Integer;
  sFileName:string;
  nHandle:Integer;
  Header:TMapHeader;
  nMapInfo:Integer;
  MapInfo:PTMapCellInfo;
  RetMapInfo:TReturnMapInfo;
  ENMapHeader:TENMapHeader;
  EnMapInfo:TENMapInfo;
  EIMapHeader:TEIMapHeader;

  Len:Integer;
begin
  SafeFillChar(m_MArr, SizeOf(m_MArr), 0);
  SafeFillchar(m_MArrEIMapInfo, SizeOf(m_MArrEIMapInfo), #0);
  SafeFillchar(m_MArrEIMapTiteInfo, SizeOf(m_MArrEIMapTiteInfo), #0);

  sFileName := g_sSelfFilePath + m_sResourcesMapBase + m_sCurrentMap + '.map';
  if not FileExists(sFileName) then
    sFileName := g_sSelfFilePath + m_sMapBase + m_sCurrentMap + '.map';

  // ÕâÀïÓÐÓÅ»¯µÄ¿Õ¼ä£¬ÔÚ»»µØÍ¼Ê±£¬´ò¿ªFileHandle£¬ÕâÀïÖ±½ÓÓÃ£¬¶ø²»ÓÃÔÙ´Î´ò¿ª¡£
  // ÅÜÒ»¶Î¾àÀëºó£¬»áµ÷ÓÃÕâÀïÒ»´Î£¬¶øÒ»´ÎÖ´ÐÐµÄÊ±ºò´Ó20-50ºÁÃë²»µÈ

  if FileExists(sFileName) then begin
    nHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
    if nHandle > 0 then begin
      m_boEIMap := False;

      FileRead(nHandle, EIMapHeader, Sizeof(TEIMapHeader));
      if (EIMapHeader.Desc[0] = 0) and (EIMapHeader.Desc[1] = 0) and
        (EIMapHeader.Desc[2] = 0) and (EIMapHeader.Desc[3] = 0) and
        (EIMapHeader.Desc[4] = 0) then begin
        Len := FileSeek(nHandle, 0, soFromEnd);

        if Len = SizeOf(TEIMapHeader) + (EIMapHeader.Width * EIMapHeader.Height * SizeOf(TEIMapTileInfo) div 4) +
          (EIMapHeader.Width * EIMapHeader.Height * SizeOf(TEIMapInfo)) then begin
          m_boEIMap := True;
        end;
      end;

      FileSeek(nHandle, 0, 0);

      if m_boEIMap then begin
        FileRead(nHandle, EIMapHeader, Sizeof(TEIMapHeader));
        Header.wWidth := EIMapHeader.Width;
        header.wHeight := EIMapHeader.Height;
      end
      else begin
        FileRead(nHandle, ENMapHeader, Sizeof(TENMapHeader));
        m_boENMap := (ENMapHeader.Title = NEWMAPTITLE);

        if m_boENMap then begin
          Header.wWidth := ENMapHeader.Width xor XORWORD;
          header.wHeight := ENMapHeader.Height xor XORWORD;
        end
        else begin
          Move(ENMapHeader, Header, SizeOf(Header));

          m_boNewMap := (Header.sTitle[14] = #13) and (Header.sTitle[15] = #10); // (nSize = Header.wWidth * Header.wHeight * SizeOf(TNewMapInfo) + SizeOf(TMapHeader));
          m_boReturnMap := Header.btVersion = 6;

          FileSeek(nHandle, SizeOf(Header), 0);
        end;
      end;

      nLx := (nCurrX - 1) * LOGICALMAPUNIT;
      nRx := (nCurrX + 2) * LOGICALMAPUNIT; // rx
      nTy := (nCurrY - 1) * LOGICALMAPUNIT;
      nBy := (nCurrY + 2) * LOGICALMAPUNIT;
      //nSize := FileSeek(nHandle, 0, 2);
      m_nWidth := Header.wWidth;
      m_nHeight := Header.wHeight;

      if nLx < 0 then nLx := 0;
      if nTy < 0 then nTy := 0;
      if nBy >= Header.wHeight then nBy := Header.wHeight;

      if m_boEIMap then begin
        nMapInfo := SizeOf(TEIMapInfo);
        //nAline := SizeOf(TEIMapInfo) * Header.wHeight;
        nAline := sizeof(TEIMapTileInfo) * (Header.wHeight div 2);
      end
      else if m_boENMap then begin
        nMapInfo := SizeOf(TENMapInfo);
        nAline := SizeOf(TENMapInfo) * Header.wHeight;
      end
      else if m_boReturnMap then begin
        nMapInfo := SizeOf(TReturnMapInfo);
        nAline := SizeOf(TReturnMapInfo) * Header.wHeight;
      end
      else if m_boNewMap then begin
        nMapInfo := SizeOf(TNewMapInfo);
        nAline := SizeOf(TNewMapInfo) * Header.wHeight;
      end
      else begin
        nMapInfo := SizeOf(TMapInfo);
        nAline := SizeOf(TMapInfo) * Header.wHeight;
      end;

      if m_boEIMap then begin
        nAline := SizeOf(TEIMapTileInfo) * (Header.wHeight div 2);
        for I := nLx div 2 to nLx div 2 + 90 do begin
          //OutputDebugString(PChar(IntToStr(I)));
          if (I >= 0) and (I < Header.wWidth div 2) then begin
            FileSeek(nHandle, SizeOf(TEIMapHeader) + (nAline * I) + (SizeOf(TEIMapTileInfo) * nTy div 2), 0);
            FileRead(nHandle, m_MArrEIMapTiteInfo[I - nLx Div 2, 0], nAline);
          end;
        end;

        nAline := SizeOf(TEIMapInfo) * (Header.wHeight);

        for I := nLx To nLx + 90 do begin
          if (I >= 0) and (I < Header.wWidth) then begin
            FileSeek(nHandle, SizeOf(TEIMapHeader) + (Header.wWidth * Header.wHeight * SizeOf(TEIMapTileInfo) div 4) + (nAline * I) + (SizeOf(TEIMapInfo) * nTy), 0);
            FileRead(nHandle, m_MArrEIMapInfo[I - nLx, 0], SizeOf(TEIMapInfo) * (nBy - nTy));
          end;
        end;
      end
      else if m_boENMap then begin
        for I := nLx to nRx - 1 do begin
          if (I >= 0) and (I < Header.wWidth) then begin
            FileSeek(nHandle, SizeOf(TENMapHeader) + (nAline * I) + (nMapInfo * nTy), 0);
            nC := (nBy - nTy);
            MapInfo := @m_MArr[I - nLx, 0];
            while nC > 0 do begin
              FileRead(nHandle, EnMapInfo, SizeOf(EnMapInfo));

              if EnMapInfo.BkImgNot xor $AA38 = $2000 then
                MapInfo.wBkImg := EnMapInfo.BkImg xor $AA38
              else if EnMapInfo.BkImgNot xor $AA38 <> 0 then // 2001, 1
                MapInfo.wBkImg := EnMapInfo.BkImg xor $1AA38
              else
                MapInfo.wBkImg := EnMapInfo.BkImg xor XORWORD;
              {
              if (EnMapInfo.BkImgNot xor $AA38) = $2000 then
               MapInfo.wBkImg := MapInfo.wBkImg or $8000;
              }
              MapInfo.wMidImg := EnMapInfo.MidImg xor XORWORD;
              MapInfo.wFrImg := EnMapInfo.FrImg xor XORWORD;
              MapInfo.btDoorIndex := EnMapInfo.DoorIndex;
              MapInfo.btDoorOffset := EnMapInfo.DoorOffset;
              MapInfo.btAniFrame := EnMapInfo.AniFrame;
              MapInfo.btAniTick := EnMapInfo.AniTick;
              MapInfo.btArea := EnMapInfo.Area;
              MapInfo.btlight := EnMapInfo.light;
              MapInfo.btUnitBkImg := 0;
              MapInfo.btUnitMidImg := 0;
              MapInfo.wAniTiles := 0;
              MapInfo.btAniTilesFrame := 0;
              MapInfo.btAniTilesTick := 0;
              MapInfo.btAniType := 0;

              MapInfo := Pointer(Integer(MapInfo) + SizeOf(TMapCellInfo));
              Dec(nC);
            end;
          end;
        end;
      end
      else begin
        for I := nLx to nRx - 1 do begin
          if (I >= 0) and (I < Header.wWidth) then begin
            FileSeek(nHandle, SizeOf(TMapHeader) + (nAline * I) + (nMapInfo * nTy), 0);
            nC := (nBy - nTy);
            MapInfo := @m_MArr[I - nLx, 0];
            while nC > 0 do begin
              FillChar(RetMapInfo, SizeOf(RetMapInfo), 0);
              FileRead(nHandle, RetMapInfo, nMapInfo);

              MapInfo.wBkImg := RetMapInfo.wBkImg;
              MapInfo.wMidImg := RetMapInfo.wMidImg;
              MapInfo.wFrImg := RetMapInfo.wFrImg;
              MapInfo.btDoorIndex := RetMapInfo.btDoorIndex;
              MapInfo.btDoorOffset := RetMapInfo.btDoorOffset;
              MapInfo.btAniFrame := RetMapInfo.btAniFrame;
              MapInfo.btAniTick := RetMapInfo.btAniTick;
              MapInfo.btArea := RetMapInfo.btArea;
              MapInfo.btlight := RetMapInfo.btlight;
              MapInfo.btUnitBkImg := RetMapInfo.btUnitBkImg;
              MapInfo.btUnitMidImg := RetMapInfo.btUnitMidImg;

              MapInfo.wAniTiles := RetMapInfo.wAniTiles;
              MapInfo.btAniTilesFrame := RetMapInfo.btAniTilesFrame;
              MapInfo.btAniTilesTick := RetMapInfo.btAniTilesTick;
              MapInfo.btAniType := RetMapInfo.btAniType;

              MapInfo := Pointer(Integer(MapInfo) + SizeOf(TMapCellInfo));
              Dec(nC);
            end;
          end;
        end;
      end;

      FileClose(nHandle);
    end;
  end;
end;

(*
procedure TMap.SaveMapArr(nCurrX, nCurrY: Integer);
{var
  I: Integer;
  k: Integer;
  nAline: Integer;
  nLx: Integer;
  nRx: Integer;
  nTy: Integer;
  nBy: Integer;
  sFileName: string;
  nHandle: Integer;
  Header: TMapHeader; }
begin
 { FillChar(m_MArr, SizeOf(m_MArr), #0);
  sFileName := m_sMapBase + m_sCurrentMap + '.map';
  if FileExists(sFileName) then begin
    nHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
    if nHandle > 0 then begin
      FileRead(nHandle, Header, SizeOf(TMapHeader));
      nLx := (nCurrX - 1) * LOGICALMAPUNIT;
      nRx := (nCurrX + 2) * LOGICALMAPUNIT; // rx
      nTy := (nCurrY - 1) * LOGICALMAPUNIT;
      nBy := (nCurrY + 2) * LOGICALMAPUNIT;

      if nLx < 0 then nLx := 0;
      if nTy < 0 then nTy := 0;
      if nBy >= Header.wHeight then nBy := Header.wHeight;
      nAline := SizeOf(TMapInfo) * Header.wHeight;
      for I := nLx to nRx - 1 do begin
        if (I >= 0) and (I < Header.wWidth) then begin
          FileSeek(nHandle, SizeOf(TMapHeader) + (nAline * I) + (SizeOf(TMapInfo) * nTy), 0);
          FileRead(nHandle, m_MArr[I - nLx, 0], SizeOf(TMapInfo) * (nBy - nTy));
        end;
      end;
      FileClose(nHandle);
    end;
  end;}
end;
*)

procedure TMap.ReadyReload;
begin
  m_nCurUnitX := -1;
  m_nCurUnitY := -1;
end;

procedure TMap.UpdateMapSquare(cx, cy:Integer);
begin
  if m_boLoadMapOk then begin
    if (cx <> m_nCurUnitX) or (cy <> m_nCurUnitY) then begin
      LoadMapArr(cx, cy);
      m_nCurUnitX := cx;
      m_nCurUnitY := cy;
    end;
  end;
end;

procedure TMap.UpdateMapPos(mx, my:Integer);
var
  cx, cy:Integer;

  procedure Unmark(xx, yy:Integer);
  var
    ax, ay:Integer;
  begin
    if (cx = xx div LOGICALMAPUNIT) and (cy = yy div LOGICALMAPUNIT) then begin
      ax := xx - m_nBlockLeft;
      ay := yy - m_nBlockTop;
      m_MArr[ax, ay].wFrImg := m_MArr[ax, ay].wFrImg and $7FFF;
      m_MArr[ax, ay].wBkImg := m_MArr[ax, ay].wBkImg and $7FFF;
    end;
  end;
begin
  // EnterCriticalSection(CriticalSection);
  // try
  cx := mx div LOGICALMAPUNIT;
  cy := my div LOGICALMAPUNIT;
  m_nBlockLeft := _MAX(0, (cx - 1) * LOGICALMAPUNIT);
  m_nBlockTop := _MAX(0, (cy - 1) * LOGICALMAPUNIT);
  UpdateMapSquare(cx, cy);

  m_nOldLeft := m_nBlockLeft;
  m_nOldTop := m_nBlockTop;
  // finally
   // LeaveCriticalSection(CriticalSection);
  // end;
end;

procedure TMap.StreamSaveToFile(Sender:TObject; Stream:TMemoryStream; Index:Integer; const FileName:string);
var
  FilePath:string;
begin
  EnterCriticalSection(m_MapLock);
  try
    if Stream <> nil then begin
      FilePath := ExtractFilePath(FileName);

      if not DirectoryExists(FilePath) then begin
        ForceDirectories(FilePath);
      end;

      try
        Stream.SaveToFile(FileName);
      except
      end;
    end;
  finally
    LeaveCriticalSection(m_MapLock);
  end;

  if FileExists(FileName) and (CompareText(ExtractFileName(FileName), m_sCurrentMap + '.map') = 0) then begin
    // ¸Õ¿ªÊ¼½øÈëÓÎÏ·Ê±£¬¿ÉÄÜg_MySelf = nil£¬µ¼ÖÂµØÍ¼Ã»ÓÐÔØÈë£¬²»¸üÐÂ 2020-03-13 22:50:48
    if (g_MySelf <> nil) then
      LoadMap(m_sCurrentMap, g_MySelf.m_nCurrX, g_MySelf.m_nCurrY)
    else
      LoadMap(m_sCurrentMap, LoWord(Index), HiWord(Index));
  end;
end;

procedure TMap.LoadMap(sMapName:string; nMx, nMy:Integer);
var
  sFileName:string;
  boUpdate:Boolean;
  nPos:Integer;
begin
  boUpdate := False;
  EnterCriticalSection(m_MapLock);
  try
    m_boLoadMapOk := False;
    m_nCurUnitX := -1;
    m_nCurUnitY := -1;
    m_sCurrentMap := sMapName;

    sFileName := GetMapFiles(g_sSelfFilePath + m_sMapBase + m_sCurrentMap + '.map'); // m_sResourcesMapBase + m_sCurrentMap + '.map';

    // if not FileExists(sFileName) then
     // sFileName := m_sMapBase + m_sCurrentMap + '.map';

    if FileExists(sFileName) then begin
      m_boLoadMapOk := True;

      UpdateMapPos(nMx, nMy);

      m_boStartLoadAll := False;
      m_boStartLoad := False;
      m_boLoadAllOk := False;
      m_boLoadOk := True;
      if CompareText(m_sOldCurrentMap, m_sCurrentMap) <> 0 then begin
        m_sOldCurrentMap := m_sCurrentMap;
        LoadAllMap;
      end;
      g_boCanDrawTileMap := True;
    end
    else begin
      FillChar(m_MArr, SizeOf(m_MArr), 0);
      if g_boAutoUpdate then
        boUpdate := True;
    end;
  finally
    LeaveCriticalSection(m_MapLock);
  end;

  if boUpdate and (g_UpdateEngine <> nil) then begin
    // Index ¼ÇÂ¼×ø±ê²ÎÊý
    nPos := MakeLong(nMx, nMy);
    g_UpdateEngine.Add(sFileName, udtFileMap, nPos, nil, StreamSaveToFile);
  end;
end;

procedure TMap.LoadAllMap;
const
  XORWORD = $AA38;
  NEWMAPTITLE = 'Map 2010 Ver 1.0';
var
  nHandle:Integer;
  Header:TMapHeader;
  nMapSize:Integer;
  n24, nW, nH:Integer;
  MapBuffer:pTMapInfoArr;
  NewMapBuffer:pTNewMapInfoArr;
  ReturnMapBuffer:pTReturnMapInfoArr;
  ENMapBuffer:pTENMapInfoArr;
  EIMapBuffer:pTEIMapInfoArr;
  //Map3Buffer: pTEIMapInfoArr;
  sFileName:string;

  ENMapHeader:TENMapHeader;
  EIMapHeader:TEIMapHeader;

  Len:Integer;
begin
  sFileName := g_sSelfFilePath + m_sResourcesMapBase + m_sCurrentMap + '.map';
  if not FileExists(sFileName) then
    sFileName := g_sSelfFilePath + m_sMapBase + m_sCurrentMap + '.map';

  sFileName := g_sSelfFilePath + m_sResourcesMapBase + m_sCurrentMap + '.map';
  if not FileExists(sFileName) then
    sFileName := g_sSelfFilePath + m_sMapBase + m_sCurrentMap + '.map';

  if not FileExists(sFileName) then Exit;

  nHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
  if nHandle > 0 then begin
    m_boEIMap := False;

    FileRead(nHandle, EIMapHeader, Sizeof(TEIMapHeader));
    if (EIMapHeader.Desc[0] = 0) and (EIMapHeader.Desc[1] = 0) and
      (EIMapHeader.Desc[2] = 0) and (EIMapHeader.Desc[3] = 0) and
      (EIMapHeader.Desc[4] = 0) then begin
      Len := FileSeek(nHandle, 0, soFromEnd);

      if Len = SizeOf(TEIMapHeader) + (EIMapHeader.Width * EIMapHeader.Height * SizeOf(TEIMapTileInfo) div 4) +
        (EIMapHeader.Width * EIMapHeader.Height * SizeOf(TEIMapInfo)) then begin
        m_boEIMap := True;
      end;
    end;

    FileSeek(nHandle, 0, 0);

    if m_boEIMap then begin
      FileRead(nHandle, EIMapHeader, Sizeof(TEIMapHeader));

      Header.wWidth := EIMapHeader.Width;
      header.wHeight := EIMapHeader.Height;
    end
    else begin
      FileRead(nHandle, ENMapHeader, Sizeof(TENMapHeader));
      m_boENMap := (ENMapHeader.Title = NEWMAPTITLE);

      if m_boENMap then begin
        Header.wWidth := ENMapHeader.Width xor XORWORD;
        header.wHeight := ENMapHeader.Height xor XORWORD;
      end
      else begin
        Move(ENMapHeader, Header, SizeOf(Header));

        m_boNewMap := (Header.sTitle[14] = #13) and (Header.sTitle[15] = #10); // (nSize = Header.wWidth * Header.wHeight * SizeOf(TNewMapInfo) + SizeOf(TMapHeader));
        m_boReturnMap := Header.btVersion = 6;

        FileSeek(nHandle, SizeOf(Header), 0);
      end;
    end;

    if MapCellArray <> nil then begin
      FreeMem(MapCellArray);
      MapCellArray := nil;
    end;

    MapCellArray := AllocMem(Header.wWidth * Header.wHeight);

    if m_boEIMap then begin
      nMapSize := Header.wWidth * m_nHeight * SizeOf(TEIMapInfo);
      EIMapBuffer := AllocMem(nMapSize);

      FileSeek(nHandle, Sizeof(TEIMapHeader) + Header.wWidth * m_nHeight div 4 * Sizeof(TEIMapTileInfo), 0);
      FileRead(nHandle, EIMapBuffer^, nMapSize);

      for nW := 0 to m_nWidth - 1 do begin
        n24 := nW * m_nHeight;
        if m_boStartLoad then break;
        for nH := 0 to m_nHeight - 1 do begin
          if (EIMapBuffer[n24 + nH].btflag) and $1 = 0 then begin
            MapCellArray[n24 + nH] := 1;
          end
          else begin
            MapCellArray[n24 + nH] := 0;
          end;
        end;
      end;
      FreeMem(EIMapBuffer);
    end
    else
      if m_boENMap then begin
        nMapSize := Header.wWidth * Header.wHeight * SizeOf(TENMapInfo);
        ENMapBuffer := AllocMem(nMapSize);

        FileRead(nHandle, ENMapBuffer^, nMapSize);

        for nW := 0 to Header.wWidth - 1 do begin
          n24 := nW * Header.wHeight;
          if m_boStartLoad then break;
          for nH := 0 to Header.wHeight - 1 do begin
            if m_boStartLoad then break;

            ENMapBuffer[n24 + nH].BkImg := ENMapBuffer[n24 + nH].BkImg xor XORWORD;
            if (ENMapBuffer[n24 + nH].BkImgNot xor $AA38) = $2000 then
              ENMapBuffer[n24 + nH].BkImg := ENMapBuffer[n24 + nH].BkImg or $8000;
            ENMapBuffer[n24 + nH].FrImg := ENMapBuffer[n24 + nH].FrImg xor XORWORD;

            if (ENMapBuffer[n24 + nH].BkImg) and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 1;
            end;
            if ENMapBuffer[n24 + nH].FrImg and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 2;
            end;
          end;
        end;
        FreeMem(ENMapBuffer);
      end
      else if m_boReturnMap then begin
        nMapSize := Header.wWidth * Header.wHeight * SizeOf(TReturnMapInfo);
        ReturnMapBuffer := AllocMem(nMapSize);

        FileRead(nHandle, ReturnMapBuffer^, nMapSize);

        for nW := 0 to Header.wWidth - 1 do begin
          n24 := nW * Header.wHeight;
          if m_boStartLoad then break;
          for nH := 0 to Header.wHeight - 1 do begin
            if m_boStartLoad then break;
            if (ReturnMapBuffer[n24 + nH].wBkImg) and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 1;
            end;
            if ReturnMapBuffer[n24 + nH].wFrImg and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 2;
            end;
          end;
        end;
        FreeMem(ReturnMapBuffer);
      end
      else if m_boNewMap then begin
        nMapSize := Header.wWidth * Header.wHeight * SizeOf(TNewMapInfo);
        NewMapBuffer := AllocMem(nMapSize);

        FileRead(nHandle, NewMapBuffer^, nMapSize);

        for nW := 0 to Header.wWidth - 1 do begin
          n24 := nW * Header.wHeight;
          if m_boStartLoad then break;
          for nH := 0 to Header.wHeight - 1 do begin
            if m_boStartLoad then break;
            if (NewMapBuffer[n24 + nH].wBkImg) and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 1;
            end;
            if NewMapBuffer[n24 + nH].wFrImg and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 2;
            end;
          end;
        end;
        FreeMem(NewMapBuffer);
      end
      else begin
        nMapSize := Header.wWidth * Header.wHeight * SizeOf(TMapInfo);
        MapBuffer := AllocMem(nMapSize);

        FileRead(nHandle, MapBuffer^, nMapSize);

        for nW := 0 to Header.wWidth - 1 do begin
          n24 := nW * Header.wHeight;
          if m_boStartLoad then break;
          for nH := 0 to Header.wHeight - 1 do begin
            if m_boStartLoad then break;
            if (MapBuffer[n24 + nH].wBkImg) and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 1;
            end;
            if MapBuffer[n24 + nH].wFrImg and $8000 <> 0 then begin
              MapCellArray[n24 + nH] := 2;
            end;
          end;
        end;
        FreeMem(MapBuffer);
      end;

    FileClose(nHandle);
    m_boLoadAllOk := not m_boStartLoad;

    if m_boLoadAllOk then begin
      LegendMap.Stop;
      LegendMap.Width := Header.wWidth;
      LegendMap.Height := Header.wHeight;
      LegendMap.PathMapArray := nil;
    end;
  end;
end;

function TMap.GetMapCellInfo(nX, nY:Integer):Integer;
begin
  if (MapCellArray <> nil) and (nX >= 0) and (nX < m_nWidth) and (nY >= 0) and (nY < m_nHeight) then begin
    Result := MapCellArray[nX * m_nHeight + nY];
  end
  else begin
    Result := -1;
  end;
end;

procedure TMap.MarkCanWalk(mx, my:Integer; bowalk:Boolean);
var
  cx, cy:Integer;
begin
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  if m_boEIMap then begin
    if bowalk Then
      m_MArrEIMapInfo[cx, cy].btFlag := 1
    else
      m_MArrEIMapInfo[cx, cy].btFlag := 0;
  end
  else begin
    if bowalk then
      m_MArr[cx, cy].wFrImg := m_MArr[cx, cy].wFrImg and $7FFF
    else
      m_MArr[cx, cy].wFrImg := m_MArr[cx, cy].wFrImg or $8000;
  end;
end;  

function TMap.CanMove(mx, my:Integer):Boolean;
var
  cx, cy:Integer;
begin
  Result := FALSE; // jacky
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  if m_boEIMap then begin
    Result := Boolean((m_MArrEIMapInfo[cx, cy].btFlag) and $01);
    if Result Then begin
      if m_MArrEIMapInfo[cx, cy].btDoorIdx and $80 <> 0 Then begin
        if (m_MArrEIMapInfo[cx, cy].btDoorOffset and $80) = 0 Then
          Result := False;
      end;
    end;
  end else if m_boENMap then begin
      Result := MapCellArray[mx * m_nHeight + my] = 0;
      if Result then begin
        if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin
          if (m_MArr[cx, cy].btDoorOffset and $80) = 0 then
            Result := FALSE;
        end;
      end;
  end else begin
      Result := ((m_MArr[cx, cy].wBkImg and $8000) + (m_MArr[cx, cy].wFrImg and $8000)) = 0;
      if Result then begin
        if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin
          if (m_MArr[cx, cy].btDoorOffset and $80) = 0 then
            Result := FALSE;
        end;
      end;
  end;
end;

function TMap.NewCanMove(mx, my:Integer):Boolean;
begin
    Result := GetMapCellInfo(mx, my) = 0;
end;

function TMap.CanFly(mx, my:Integer):Boolean;
var
  cx, cy:Integer;
begin
  Result := FALSE; // jacky
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  Result := (m_MArr[cx, cy].wFrImg and $8000) = 0;
  if Result then begin
    if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin // ¹®Â¦ÀÌ ÀÖÀ½
      if (m_MArr[cx, cy].btDoorOffset and $80) = 0 then
        Result := FALSE;
    end;
  end;
end;

function TMap.GetDoor(mx, my:Integer):Integer;
var
  cx, cy:Integer;
begin
  Result := 0;
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  if m_boEIMap then begin
    If m_MArrEIMapInfo[cx, cy].btDoorIdx and $80 > 0 then Begin
      Result := m_MArrEIMapInfo[cx, cy].btDoorIdx and $7F;
    end;
  end
  else begin
    if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin
      Result := m_MArr[cx, cy].btDoorIndex and $7F;
    end;
  end;
end;

function TMap.IsDoorOpen(mx, my:Integer):Boolean;
var
  cx, cy:Integer;
begin
  Result := FALSE;
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  if m_boEIMap then begin
    if m_MArrEIMapInfo[cx, cy].btDoorIdx and $80 > 0 then begin
      Result := (m_MArrEIMapInfo[cx, cy].btDoorOffset and $80 <> 0);
    end;
  end else begin
    if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin
      Result := (m_MArr[cx, cy].btDoorOffset and $80 <> 0);
    end;
  end;
end;

function TMap.OpenDoor(mx, my:Integer):Boolean;
var
  I, j, cx, cy, idx:Integer;
begin
  Result := FALSE;
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  if m_boEIMap then begin
    if m_MArrEIMapInfo[cx, cy].btDoorIdx and $80 > 0 then begin
      idx := m_MArrEIMapInfo[cx, cy].btDoorIdx and $7F;
      for I := cx - 8 To cx + 10 do begin
        for j := cy - 8 To cy + 10 do Begin
          if (m_MArrEIMapInfo[I, j].btDoorIdx and $7F) = idx Then
            m_MArrEIMapInfo[I, j].btDoorOffset := m_MArrEIMapInfo[I, j].btDoorOffset and $80;
        end;
      end;
    end;
  end
  else begin
    if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin
      idx := m_MArr[cx, cy].btDoorIndex and $7F;
      for I := cx - 10 to cx + 10 do
        for j := cy - 10 to cy + 10 do begin
          if (I > 0) and (j > 0) then
            if (m_MArr[I, j].btDoorIndex and $7F) = idx then
              m_MArr[I, j].btDoorOffset := m_MArr[I, j].btDoorOffset or $80;
        end;
    end;
  end;
end;

function TMap.CloseDoor(mx, my:Integer):Boolean;
var
  I, j, cx, cy, idx:Integer;
begin
  Result := FALSE;
  cx := mx - m_nBlockLeft;
  cy := my - m_nBlockTop;
  if (cx < 0) or (cy < 0) or (cx > 312) or (cy > 312) then Exit;

  if m_boEIMap then begin
    if m_MArrEIMapInfo[cx, cy].btDoorIdx and $80 > 0 then begin
      idx := m_MArrEIMapInfo[cx, cy].btDoorIdx and $7F;
      for I := cx - 8 To cx + 10 do begin
        for j := cy - 8 To cy + 10 do Begin
          if (m_MArrEIMapInfo[I, j].btDoorIdx and $7F) = idx Then
            m_MArrEIMapInfo[I, j].btDoorOffset := m_MArrEIMapInfo[I, j].btDoorOffset and $7F;
        end;
      end;
    end;
  end
  else begin
    if m_MArr[cx, cy].btDoorIndex and $80 > 0 then begin
      idx := m_MArr[cx, cy].btDoorIndex and $7F;
      for I := cx - 8 to cx + 10 do
        for j := cy - 8 to cy + 10 do begin
          if (m_MArr[I, j].btDoorIndex and $7F) = idx then
            m_MArr[I, j].btDoorOffset := m_MArr[I, j].btDoorOffset and $7F;
        end;
    end;
  end;
end;

end.
