(*
 *
 * 单元 : chongchong 
 * 网站 :
 *
 * 说明 : 阿里云公共单元
 *
 * CHANGES:
 * v1.0 <2017-04-21>
 *   + first release
 *)

unit acsCommon;

interface

uses
  Windows, Messages, SysUtils, Classes, Forms, EncdDecd, uMD5, Dialogs,
  flcHash;

type
  TFormatType = (ftXml, ftJson);
  TRequestMethod = (rmGet, rmPost);
  TMsgType = (mtOK, mtAsk, mtWarn, mtErr);

const
  TRequestMethodName: array[TRequestMethod] of string = ('GET', 'POST');
  
const
  MSG_INFO          = '提示';
  MSG_ASK           = '询问';
  MSG_WARN          = '警告';
  MSG_ERR           = '错误';

  function GetSignatureNonce: string;

  function URLEncode(Value: string): string;
  function MD5Encode(Value: string): string;
  function MD5String(Value: string): string;
  function Base64Encode(Value: string): string;
  function Base64Decode(Value: string): string;

  function MidStr(AText, AStart, AEnd: WideString;
    StartPosition: DWORD = 0; IgnoreCase: Boolean = True;
    ResultIfNotFoundEnd: Boolean = True): WideString; overload;

  function ShowMsg(const Msg: string; const Caption: string = ''; MsgType: TMsgType = mtOK): Integer;

implementation


(*
function HmacSha1(const Key, Value: AnsiString): AnsiString;
Label ErrorExit;
const
  KEY_LEN_MAX = 16;
var
  hProv: HCRYPTPROV;
  hKeyHash: HCRYPTHASH;
  hKey: HCRYPTKEY;

  hHmacHash: HCRYPTHASH;
  HmacInfo: HMAC_INFO;

  dwDataLen: LongWord;

  pbHash, pbTemp: PByte;

  I: Integer;

  pb1, pb2: Byte;
begin
  hProv := 0;
  hKeyHash := 0;
  hKey := 0;

  hHmacHash := 0;
  dwDataLen := 0;

  pbHash := nil;

  //--------------------------------------------------------------------
  // Acquire a handle to the default RSA cryptographic service provider.

  if (not CryptAcquireContext(
    hProv,                        // handle of the CSP
    nil,                          // key container name
    nil,                          // CSP name
    PROV_RSA_FULL,                // provider type
    CRYPT_VERIFYCONTEXT)) then    // no key access is requested
  begin
    goto ErrorExit;
  end;

  //--------------------------------------------------------------------
  // Derive a symmetric key from a hash object by performing the
  // following steps:
  //    1. Call CryptCreateHash to retrieve a handle to a hash object.
  //    2. Call CryptHashData to add a text string (password) to the
  //       hash object.
  //    3. Call CryptDeriveKey to create the symmetric key from the
  //       hashed password derived in step 2.
  // You will use the key later to create an HMAC hash object.

  if not CryptCreateHash(
    hProv,                        // handle of the CSP
    CALG_SHA1,                    // hash algorithm to us
    0,                            // hash key
    0,                            // reserved
    hKeyHash) then                // address of hash object handle
  begin
    goto ErrorExit;
  end;

  if not CryptHashData(
    hKeyHash,                     // handle of the HMAC hash object
    PByte(Key),                   // message to hash
    Length(Key),                  // number of bytes of data to add
    0) then                       // flags
  begin
    goto ErrorExit;
  end;

  if (not CryptDeriveKey(
    hProv,                        // handle of the CSP
    CALG_RC4,                     // algorithm ID
    hKeyHash,                     // handle to the hash object
    0,                            // flags
    hKey)) then                   // address of the key handle
  begin
    goto ErrorExit;
  end;

  pb1 := $36;
  pb2 := $5C;

  ZeroMemory(@HmacInfo, SizeOf(HmacInfo));
  HmacInfo.HashAlgid := CALG_SHA1;
  HmacInfo.pbInnerString := PByte($36);
  HmacInfo.cbInnerString := 1;
  HmacInfo.pbOuterString := PByte($5C);
  HmacInfo.cbOuterString := 1;
  if (not CryptCreateHash(
    hProv,                        // handle of the CSP.
    CALG_HMAC,                    // HMAC hash algorithm ID
    hKey,                         // key for the hash (see above)
    0,                            // reserved
    hHmacHash)) then              // address of the hash handle
  begin
   goto ErrorExit;
  end;

  if (not CryptSetHashParam(
    hHmacHash,                    // handle of the HMAC hash object
    HP_HMAC_INFO,                 // setting an HMAC_INFO object
    PByte(@HmacInfo),             // the HMAC_INFO object
    0)) then                      // reserved
  begin
   goto ErrorExit;
  end;

  if (not CryptHashData(
    hHmacHash,                    // handle of the HMAC hash object
    PByte(Value),                 // message to hash
    Length(Value),                // number of bytes of data to add
    0)) then                      // flags
  begin
    goto ErrorExit;
  end;

  //--------------------------------------------------------------------
  // Call CryptGetHashParam twice. Call it the first time to retrieve
  // the size, in bytes, of the hash. Allocate memory. Then call
  // CryptGetHashParam again to retrieve the hash value.

  if (not CryptGetHashParam(
    hHmacHash,                    // handle of the HMAC hash object
    HP_HASHVAL,                   // query on the hash value
    nil,                          // filled on second call
    dwDataLen,                    // length, in bytes, of the hash
    0)) then
  begin
   goto ErrorExit;
  end;

  GetMem(pbHash, dwDataLen);
  if pbHash = nil then
  begin
    goto ErrorExit;
  end;

  if (CryptGetHashParam(
    hHmacHash,                    // handle of the HMAC hash object
    HP_HASHVAL,                   // query on the hash value
    pbHash,                       // pointer to the HMAC hash value
    dwDataLen,                    // length, in bytes, of the hash
    0)) then
  begin
    {
    SetLength(Result, dwDataLen);
    Move(pbHash^, Result[1], dwDataLen);
    }
    pbTemp := pbHash;
    for i := 0 to dwDataLen - 1 do
    begin
      Result := Result + IntToHex(pbTemp^, 2);
      Inc(pbTemp);
    end;
  end;

// Free resources.
ErrorExit:
  if hHmacHash <> 0 then
  begin
    CryptDestroyHash(hHmacHash);
  end;

  if hKey <> 0 then
  begin
    CryptDestroyKey(hKey);
  end;

  if hKeyHash <> 0 then
  begin
    CryptDestroyHash(hKeyHash);
  end;

  if hProv <> 0 then
  begin
    CryptReleaseContext(hProv, 0);
  end;

  if pbHash <> nil then
  begin
    FreeMem(pbHash);
  end;
end;

function Hashhmacsha1(const Key, Value: AnsiString): AnsiString;
const
  KEY_LEN_MAX = 16;
var
  hCryptProvider: HCRYPTPROV;
  hHash: HCRYPTHASH;
  hKey: HCRYPTKEY;
  dwHashLen: dWord;
  i: Integer;

  hPubKey : HCRYPTKey;
  hHmacHash: HCRYPTHASH;
  bHmacHash: array[0..$7F] of Byte;
  dwHmacHashLen: dWord;
  hmac_info : JwaWinCrypt.HMAC_INFO;

  keyBlob: record
    keyHeader: BLOBHEADER;
    keySize: DWORD;
    keyData: array[0..KEY_LEN_MAX-1] of Byte;
  end;
  keyLen : INTEGER;
begin
  dwHmacHashLen := 32;
  {get context for crypt default provider}
  if CryptAcquireContext(hCryptProvider, nil, nil, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT) then
  begin
    {create hash-object MD5}
    if CryptCreateHash(hCryptProvider, CALG_SHA1, 0, 0, hHash) then
    begin

      {get hash from password}
      if CryptHashData(hHash, PByte(Key), Length(Key), 0) then
      begin

        // hHash is now a hash of the provided key, (SHA1)
        // Now we derive a key for it
        hPubKey := 0;

        FillChar(keyBlob, SizeOf(keyBlob), 0);
        keyBlob.keyHeader.bType := PLAINTEXTKEYBLOB;
        keyBlob.keyHeader.bVersion := CUR_BLOB_VERSION;
        keyBlob.keyHeader.aiKeyAlg := CALG_RC4;
        KeyBlob.keySize := KEY_LEN_MAX;

        if(Length(key) < (KEY_LEN_MAX))then
          KeyLen := Length(key)
        else
          KeyLen := KEY_LEN_MAX;
        Move(Key[1], KeyBlob.keyData[0], KeyLen );

        if CryptImportKey(hCryptProvider, @keyBlob, SizeOf(KeyBlob), hPubKey, 0, hKey) then
        begin

          //hkey now holds our key. So we have do the whole thing over again
          ZeroMemory( @hmac_info, SizeOf(hmac_info) );
          hmac_info.HashAlgid := CALG_SHA1;
          if CryptCreateHash(hCryptProvider, CALG_HMAC, hKey, 0, hHmacHash) then
          begin
              if CryptSetHashParam( hHmacHash, HP_HMAC_INFO, @hmac_info, 0) then
              begin

                if CryptHashData(hHmacHash, @Value[1], Length(Value), 0) then
                begin
                  if CryptGetHashParam(hHmacHash, HP_HASHVAL, @bHmacHash[0], dwHmacHashLen, 0) then
                  begin
                    SetLength(Result, dwHmacHashLen);
                    Move(bHmacHash[0], Result[1], dwHmacHashLen);

                    {
                    for i := 0 to dwHmacHashLen-1 do
                      Result := Result + IntToHex(bHmacHash[i], 2);
                    }
                  end
                  else
                   WriteLn( 'CryptGetHashParam ERROR --> ' + SysErrorMessage(GetLastError)) ;
                end
                else
                  WriteLn( 'CryptHashData ERROR --> ' + SysErrorMessage(GetLastError)) ;
                {destroy hash-object}
                CryptDestroyHash(hHmacHash);
                CryptDestroyKey(hKey);
              end
              else
                WriteLn( 'CryptSetHashParam ERROR --> ' + SysErrorMessage(GetLastError)) ;

          end
          else
            WriteLn( 'CryptCreateHash ERROR --> ' + SysErrorMessage(GetLastError)) ;
        end
        else
          WriteLn( 'CryptDeriveKey ERROR --> ' + SysErrorMessage(GetLastError)) ;

      end;
      {destroy hash-object}
      CryptDestroyHash(hHash);
    end;
    {release the context for crypt default provider}
    CryptReleaseContext(hCryptProvider, 0);
  end;
  //Result := AnsiLowerCase(Result);
end;

*)

// 唯一随机数，用于防止网络重放攻击。用户在不同请求间要使用不同的随机数值
function GetSignatureNonce: string;
var
  I: Integer;
  Guid: TGUID;
  S: string;
begin
  if not CreateGUID(Guid) = S_OK then
  begin
    Randomize;

    Guid.D1 := Random(High(Integer));
    Guid.D2 := Random(High(Word));
    Guid.D3 := Random(High(Word));

    for I := 0 to Length(Guid.D4) - 1 do
    begin
      Guid.D4[I] := Random(High(Byte));
    end;
  end;

  S := GUIDToString(Guid);
  SetLength(Result, Length(S) - 2);
  Move(S[2], Result[1], Length(Result));
end;

{**
  过程名:     EncodeURL
  作用:       对字符串进行URL编码
  作者:       虫虫
  日期:       2010.01.06
  参数:       Value: string
  返回值:     string
**}
function URLEncode(Value: string): string;
const
  Digits: array[0..15] of char = ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f');
var
  I, J: Integer;
  WS: WideString;
  S: string;
begin
  Result := '';
  WS := Value;
  for I := 1 to Length(WS) do
  begin
    S := WS[I];
    if Length(S) > 1 then begin
      S := UTF8Encode(S);
      for J := 1 to Length(S) do
        Result := Result + Format('%%%.2x', [Ord(S[J])]);  // '%' + Digits[(Ord(S[J]) shr 4) and $0f] + Digits[Ord(S[J]) and $0f]   //
    end else begin
      if AnsiChar(S[1]) in ['0'..'9', 'a'..'z', 'A'..'Z', '.', '_', '-', '$'] then
        Result := Result + S
      else if s[1] = ' ' then
        Result := Result + '+'
      else
        Result := Result + Format('%%%.2x', [Ord(S[1])]);      //'%' + Digits[(Ord(S[1]) shr 4) and $0f] + Digits[Ord(S[1]) and $0f]   //
    end;
  end;
end;

{**
  过程名:     MD5Encode
  作用:       将字符串进行MD5加密，返回32字节
  作者:       虫虫
  日期:       2010.01.06
  参数:       Value: string
  返回值:     string
**}
function MD5Encode(Value: string): string;
begin
  Result := MD5Print(uMD5.MD5String(Value));
end;

{**
  过程名:     MD5Encode
  作用:       将字符串进行MD5加密，返回32字节
  作者:       虫虫
  日期:       2010.01.06
  参数:       Value: string
  返回值:     string
**}
function MD5String(Value: string): string;
var
  MD5: MD5Digest;
begin
  MD5 := uMD5.MD5String(Value);
  SetLength(Result, High(MD5Digest) + 1);
  CopyMemory(@(Result[1]), @(MD5[0]), High(MD5) + 1);
end;

{**
  过程名:     EncodeBase64
  作用:       将字符串进行Base64加密
  作者:       虫虫
  日期:       2010.01.06
  参数:       Value: string
  返回值:     string
**}
function Base64Encode(Value: string): string;
begin
  Result := EncodeString(Value);
end;

{**
  过程名:     DecodeBase64
  作用:       将字符串进行Base64解密
  作者:       虫虫
  日期:       2010.01.06
  参数:       Value: string
  返回值:     string
**}
function Base64Decode(Value: string): string;
begin
  Result := DecodeString(Value);
end;

{**
  过程名:     MidStr
  作用:       根据指定的首尾字符串取中间字符串
  作者:       虫虫
  日期:       2010.01.29
  参数:       AText : WideString;           待搜索的字符串
              AStart: WideString;           开始字符串
              AEnd  : WideString;           结尾字符串
              StartPosition: DWORD;         开始搜索位置; 优先 AStart, AEnd 参数
              IgnoreCase: Boolean;          忽略大小写
              ResultIfNotFoundEnd: Boolean; 为True时,如果没有搜到 AEnd ,返回 AStart 一直到结束
                                            为False时,如果没有搜到 AEnd ,返回空值
  返回值:     WideString
**}
function MidStr(AText, AStart, AEnd: WideString;
  StartPosition: DWORD; IgnoreCase: Boolean; ResultIfNotFoundEnd: Boolean): WideString; overload;
var
  StrText: WideString;
  Index: Integer;
  P1, P2: PWideChar;
begin
  Result := '';
  if Length(AText) = 0 then Exit;

  if IgnoreCase then
  begin
    StrText := LowerCase(AText);
    AStart  := LowerCase(AStart);
    AEnd    := LowerCase(AEnd);
  end;
  

  P1 := @(StrText[1]);
  P2 := @(AText[1]);
  if StartPosition > 1 then
  begin
    P1 := P1 + StartPosition - 1;
    P2 := P2 + StartPosition - 1;
  end;

  if Length(AStart) > 0 then
  begin
    Index := Pos(AStart, P1);
    if Index = 0 then Exit;
    P1 := P1 + Index + Length(AStart) - 1;
    P2 := P2 + Index + Length(AStart) - 1;
  end;

  Index := MAXINT;
  if Length(AEnd) > 0 then
  begin
    Index := Pos(AEnd, P1);
    if Index = 0 then
    begin
      if not ResultIfNotFoundEnd then
        Exit
      else
        Index := MAXINT;
    end;
  end;
  Result := Copy(P2, 1, Index - 1);
end;

{**
  过程名:     ShowMsg
  作用:       弹出对话框
  作者:       虫虫
  日期:       2010.01.06
  参数:       Msg, Caption: string; MsgType: TMsgType
  返回值:     Integer
**}
function ShowMsg(const Msg: string; const Caption: string = '';
    MsgType: TMsgType = mtOK): Integer;
const
  BtnType: array[TMsgType] of Cardinal = (MB_OK or MB_ICONINFORMATION,
                                          MB_YESNO or MB_ICONQUESTION,
                                          MB_OK or MB_ICONWARNING,
                                          MB_OK or MB_ICONERROR);
  MSG_CAPTION: array[TMsgType] of string = (MSG_INFO, MSG_ASK, MSG_WARN, MSG_ERR);
var
  strCaption: string;
begin
  if Length(Caption) = 0 then
    strCaption := MSG_CAPTION[MsgType]
  else
    strCaption := Caption;
  Result := MessageBox(Application.Handle, PChar(Msg), PChar(strCaption),
    BtnType[MsgType]);
end;


end.
