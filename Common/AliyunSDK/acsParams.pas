(*
 *
 * 单元 : chongchong
 * 网站 :
 *
 * 说明 : 阿里云参数列表
 *
 * CHANGES:
 * v1.0 <2017-04-21>
 *   + first release
 *)

unit acsParams;

interface

uses
  Windows, SysUtils, Classes, acsCommon, flcHash;

type
  TAcsParams = class(TObject)
  private
    FParams: TStringList;
  public
    constructor Create;
    destructor Destroy; override;

    procedure AddParam(Key, Value: string; ParamIndex: Integer = -1);
    procedure DelParam(Key: string);
    procedure ClearParams;
    function  SignParams(Secret: string; Method: TRequestMethod): string;
    function  GetParamsText: string;
    function  GetParamValueByKey(Key: string): string;
  end;

var
  g_SignStr: string;
  g_RequestStr: string;

implementation

uses
  acsUtils;

{ TTaobaoParams }

constructor TAcsParams.Create;
begin
  FParams := TStringList.Create;
end;

destructor TAcsParams.Destroy;
begin
  FParams.Free;
  inherited;
end;

procedure TAcsParams.AddParam(Key, Value: string; ParamIndex: Integer = -1);
var
  Index: Integer;
begin
  Key := Trim(Key);
  Value := Trim(Value);

  if Length(Key) = 0 then Exit;

  Index := FParams.IndexOfName(Key); 
  if Index >= 0 then
  begin
    if Length(Value) = 0 then
      FParams.Delete(Index)
    else
      FParams.ValueFromIndex[Index] := Value;
  end
  else if Length(Value) > 0 then
  begin
    if ParamIndex = -1 then
      FParams.Add(Key + '=' + Value)
    else
      FParams.Insert(ParamIndex, Key + '=' + Value);
  end;
end;

procedure TAcsParams.DelParam(Key: string);
var
  Index: Integer;
begin
  Key := Trim(Key);
  if Length(Key) = 0 then Exit;
  Index := FParams.IndexOfName(Key);
  if Index >= 0 then
  begin
    FParams.Delete(Index);
  end
end;

procedure TAcsParams.ClearParams;
begin
  FParams.Clear;
end;


(*
 *   delphi 自带相关的排序搞死人，下面的排序结果不对 SignName 应该 排在SignatureNonce 前面
 *
 *   SL.Add('SignName');
 *   SL.Add('SignatureNonce');
 *   SL.Add('SignatureVersion');
 *   SL.Add('SignatureMethod');
 *
 *)

function CompareAnsiStr(const S1, S2: AnsiString): integer;
var
  P1, P2: PAnsiChar;
  i: integer;
  L1, L2: integer;
begin
  { Length and PChar of S1 }
  L1:= Length(S1);
  P1:= PAnsiChar(S1);

  { Length and PChar of S2 }
  L2:= Length(S2);
  P2:= PAnsiChar(S2);

  { Continue the loop until the end of one string is reached. }
  i:= 0;
  while (i < L1) and (i < L2) do
  begin
    if (P1^ <> P2^) then
    begin
      Result := Ord(P1^) - Ord(P2^);
      Exit;

    end;
    Inc(P1);
    Inc(P2);
    Inc(i);
  end;

  { If chars were not different return the difference in length }
  Result:= L1 - L2;
end;

function ParamSort(List: TStringList; Index1, Index2: Integer): Integer;
begin
  Result := CompareAnsiStr(List.Strings[Index1], List.Strings[Index2]);
end;

function TAcsParams.SignParams(Secret: string; Method: TRequestMethod): string;
var
  I: Integer;
  StrOld, S, ParamName: string;
begin
  Result := '';
  StrOld := FParams.Text;

  FParams.CustomSort(ParamSort);
  for I := 0 to FParams.Count - 1 do
  begin
    ParamName := FParams.Names[I];
    Result := Result + '&' + URLEncode(ParamName) + '=' + URLEncode(FParams.ValueFromIndex[I]);
  end;
  
  FParams.Text := StrOld;

  Result := StringReplace(Result, '+', '%20', [rfReplaceAll]);

  Result := TRequestMethodName[Method] + '&' + URLEncode('/') + '&' + URLEncode(Copy(Result, 2, MaxInt));

  g_SignStr := Result;
  
  S := SHA1DigestToStrA(CalcHMAC_SHA1(g_AcsUtil.KeySecret + '&', Result));
  Result := Base64Encode(S);
end;

function TAcsParams.GetParamsText: string;
var
  I: Integer;
  sTemp: String;
begin
  Result := '';
  for I := 0 to FParams.Count - 1 do
  begin
    sTemp := (FParams.ValueFromIndex[I]);
    sTemp := URLEncode(sTemp);
    Result := Result + FParams.Names[I] + '=' + sTemp + '&';
  end;
  if Length(Result) > 0 then
  begin
    Result := Copy(Result, 1, Length(Result) - 1);
  end;
end;

function TAcsParams.GetParamValueByKey(Key: string): string;
begin
  Result := FParams.Values[Key];
end;

end.
