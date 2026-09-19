unit NearActorHintEffect;

interface

uses
Windows,
Classes,
SysUtils;


type
TNearActorHintInfo = record
   X:Integer;
   Y:Integer;
   nFriendFlag:Integer;
end;
PNearActorHintInfo = ^TNearActorHintInfo;

TNearActorHintEffectMgr = class
const
    FRAME_TICK_COUNT = 150;
private
    m_nInfoCount:Integer;
    m_dwHintEffectFrameCount:DWORD;
    m_dwLastHintEffectTick:DWORD;
    m_dwHintEffctFrameIndex:DWORD;
    m_arrHintInfo:array of TNearActorHintInfo;
    function GetFrameIndex: Integer;
    function GetLatestInfoIndex: Integer;
public
    constructor Create;
    destructor Destroy; override;

    procedure AddActorHint(X, Y, nFriendFlag:Integer);
    procedure DeleteLatestHint();
    procedure CleaerHint();
    procedure FrameDrive();

    function GetHintInfo(nIndex:Integer):PNearActorHintInfo;

    property FrameIndex:Integer read GetFrameIndex;
    property InfoCount:Integer read m_nInfoCount;
    property LatestInfoIndex:Integer read GetLatestInfoIndex;
    property FrameCount:DWORD read m_dwHintEffectFrameCount write m_dwHintEffectFrameCount;
end;

implementation

uses
MShare;

{ TNearActorHintEffectMgr }

procedure TNearActorHintEffectMgr.AddActorHint(X, Y, nFriendFlag: Integer);
begin
    if Length(m_arrHintInfo) < (m_nInfoCount + 1) then begin
        SetLength(m_arrHintInfo, m_nInfoCount + 50); //每次增加50
    end;
    m_arrHintInfo[m_nInfoCount].X := X;
    m_arrHintInfo[m_nInfoCount].Y := Y;
    m_arrHintInfo[m_nInfoCount].nFriendFlag := nFriendFlag;
    Inc(m_nInfoCount);
end;

procedure TNearActorHintEffectMgr.CleaerHint;
begin
    m_nInfoCount := 0;
    m_dwHintEffectFrameCount := 6;
end;

constructor TNearActorHintEffectMgr.Create;
begin
    SetLength(m_arrHintInfo, 200);
end;

procedure TNearActorHintEffectMgr.DeleteLatestHint;
begin
    if m_nInfoCount > 0 then begin
        Dec(m_nInfoCount);
    end;
end;

destructor TNearActorHintEffectMgr.Destroy;
begin
    SetLength(m_arrHintInfo, 0);
end;

procedure TNearActorHintEffectMgr.FrameDrive;
var
    dwCurTick:DWORD;
begin
    dwCurTick := MyGetTickCount();

    if m_dwLastHintEffectTick = 0 then begin
         m_dwLastHintEffectTick := dwCurTick;
         m_dwHintEffctFrameIndex := 0;
    end else begin
        if dwCurTick - m_dwLastHintEffectTick >= FRAME_TICK_COUNT then begin
            m_dwLastHintEffectTick := dwCurTick;
            Inc(m_dwHintEffctFrameIndex);
            if (m_dwHintEffctFrameIndex >= (m_dwHintEffectFrameCount + 0)) then begin //给一个中间停留的机会
                m_dwHintEffctFrameIndex := 0;
            end;
        end;
    end;
end;

function TNearActorHintEffectMgr.GetFrameIndex: Integer;
begin
    Result := Integer(m_dwHintEffctFrameIndex);
end;

function TNearActorHintEffectMgr.GetHintInfo(nIndex: Integer): PNearActorHintInfo;
begin
    if m_nInfoCount > 0 then begin
        if nIndex >= 0 then begin
            if (nIndex < m_nInfoCount) then begin
                Result := @m_arrHintInfo[nIndex];
            end else begin
                Result := nil;
            end;
        end else begin
            Result := @m_arrHintInfo[m_nInfoCount-1];
        end;
    end else begin
        Result := nil;
    end;
end;

function TNearActorHintEffectMgr.GetLatestInfoIndex: Integer;
begin
    Result := m_nInfoCount - 1;
end;

end.
