unit ObjGame;

interface

uses
  Windows, Classes, SysUtils, Forms, Math, M2Definition;

type
  TGameObject = class(TObject)
    m_ObjGame: TObjGame;
    m_dwAddTime: LongWord;
    m_nMapX: Integer;
    m_nMapY: Integer;
  public
    constructor Create(); virtual;
    destructor Destroy; override;
  end;

  TGateObject = class(TGameObject)
    m_sName: string;
    m_nSMapX: Integer;
    m_nSMapY: Integer;

    m_boFlag: Boolean;
   // m_SEnvir: TObject;
    m_DEnvir: TObject;
    m_sSMapNO: string;
    m_sDMapNO: string;
    m_dwRunTick: LongWord;
    m_dwRunTime: LongWord;
    m_boCenter: Boolean;

    m_BindNPC: TObject;
  public
    constructor Create(); override;
    destructor Destroy; override;
  end;

  TDoorObject = class(TGameObject)
    m_n08: Integer;
    m_Status: pTDoorStatus;
  public
    constructor Create(); override;
  end;

implementation

uses
  M2Share, ObjNpc;

constructor TGameObject.Create();
begin
  m_ObjGame := Obj_None;
  m_dwAddTime := MyGetTickCount;
  m_nMapX := 0;
  m_nMapY := 0;
end;

destructor TGameObject.Destroy;
begin
  inherited;
end;

constructor TGateObject.Create();
begin
  inherited;
  m_ObjGame := Obj_Gate;
  m_boFlag := False;
  m_nSMapX := -1;
  m_nSMapY := -1;
  m_sSMapNO := '';
  m_sDMapNO := '';
  m_sName := IntToStr(NativeInt(Self));
  m_dwRunTick := MyGetTickCount;
  m_dwRunTime := 0;
  m_boCenter := True;
  m_DEnvir := nil;
  m_BindNPC := nil;
end;

destructor TGateObject.Destroy;
begin
  if m_BindNPC <> nil then
  begin
    TMerchant(m_BindNPC).MakeGhost;
    {
    UserEngine.m_MerchantList.Lock;
    try
      for I := 0 to UserEngine.m_MerchantList.Count - 1 do
      begin
        if UserEngine.m_MerchantList.Items[I] = m_BindNPC then
        begin
          UserEngine.m_MerchantList.Delete(I);
          TMerchant(m_BindNPC).Free;
          Break;
        end;
      end;
    finally
      UserEngine.m_MerchantList.UnLock;
    end;
    }
  end;

  inherited;
end;

constructor TDoorObject.Create();
begin
  inherited;
  m_ObjGame := Obj_Door;
  m_n08 := 0;
  m_Status := nil;
end;

end.

