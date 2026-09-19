unit ClientSession;

interface

uses
  Windows, Messages, SysUtils, Classes, ClientThread, AcceptExWorkedThread, IOCPTypeDef, ThreadPool,
  Protocol, SHSocket, SyncObj, Dialogs, EDcode, UnitDes, WinSock2;

type
  TSessionObj = class(TSyncObj) //会话对象类
  public
    m_pUserOBJ: PUserOBJ;
    m_pOverlapRecv: PIOCPCommObj;
    m_pOverlapSend: PIOCPCommObj;
    m_tIOCPSender: TIOCPWriter;
    m_tLastGameSvr: TClientThread;

    m_fKickFlag: Boolean;
    m_fHandleLogin: Byte;
    m_dwSessionID: LongWord;
    m_nSvrListIdx: Integer;
    m_nSvrObject: Integer;
    m_wRandKey: Word;

    m_dwClientTimeOutTick: LongWord;

    m_dwProtocolPassword: LongWord; // 通讯协议密码
    m_IsCanSetL2Password: Boolean; // 是否可设置二级密码
    m_IsCanCheckL2Password: Boolean; // 是否可检测二级密码

    m_IsDelayClose: Boolean;
    m_dwDelayCloseTick: LongWord;

    constructor Create;
    destructor Destroy; override;
    procedure ProcessCltData(const Addr: Integer; const Len: Integer; var Succeed: BOOL; const fCDPacket: Boolean = False);
    procedure ProcessSvrData(GS: TClientThread; const Addr: Integer; const Len: Integer);
    procedure ReCreate;
    procedure UserEnter;
    procedure UserLeave;
    procedure SendDefMessage(wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word; sMsg: string);   // 2021-01-05
    procedure DelayClose(DelayTick: LongWord);
  end;

var
  g_pFillUserObj: TSessionObj = nil;
  g_UserList: array[0..USER_ARRAY_COUNT - 1] of TSessionObj; //会话对象列表
var
  lastqueryTick: DWORD;
  starttick: DWORD;
  enterCount: Integer;
  mainhwnd: HWND;
  gDeny: Boolean;


implementation

uses
  FuncForComm, SendQueue, LogManager, ConfigManager, Misc, HUtil32, Grobal2,
  IPAddrFilter;

constructor TSessionObj.Create;
begin
  inherited;
  Randomize();
  m_fKickFlag := False;
  m_nSvrObject := 0;
  m_dwClientTimeOutTick := GetTickCount();
  m_fHandleLogin := 0;
  m_nSvrListIdx := 0;

  m_wRandKey := 0;

  m_dwProtocolPassword := 0;
  m_IsCanSetL2Password := False;
  m_IsCanCheckL2Password := False;

  m_IsDelayClose := False;
  m_dwDelayCloseTick := GetTickCount;
end;

destructor TSessionObj.Destroy;
begin
  inherited;
end;

procedure TSessionObj.ReCreate();
begin
  m_fKickFlag := False;

  m_nSvrObject := 0;
  m_fHandleLogin := 0;
  m_dwClientTimeOutTick := GetTickCount;

  m_dwProtocolPassword := 0;
  m_IsCanSetL2Password := False;
  m_IsCanCheckL2Password := False;

  m_IsDelayClose := False;
  m_dwDelayCloseTick := GetTickCount;
end;

function RotateBits(C: Char; Bits: Integer): Char;
var
  SI: Word;
begin
  Bits := Bits mod 8;
  if Bits < 0 then
  begin
    SI := MakeWord(Byte(C), 0);
    SI := SI shl Abs(Bits);
  end
  else
  begin
    SI := MakeWord(0, Byte(C));
    SI := SI shr Abs(Bits);
  end;
  SI := Swap(SI);
  SI := Lo(SI) or Hi(SI);
  Result := chr(SI);
end;

procedure TSessionObj.SendDefMessage(wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word; sMsg: string);
var
  iLen: Integer;
  Cmd: TCmdPack;
  TempBuf, SendBuf: array[0..2048 - 1] of Char;
begin
  if (m_tLastGameSvr = nil) or not m_tLastGameSvr.Active then
    Exit;

  Cmd.Recog := nRecog;
  Cmd.ident := wIdent;
  Cmd.param := nParam;
  Cmd.tag := nTag;
  Cmd.param := nSeries;

  SendBuf[0] := '#';
  Move(Cmd, TempBuf[1], SizeOf(TCmdPack)); //把Cmd的内容，复制到TempBuf中
  if sMsg <> '' then
  begin
    Move(sMsg[1], TempBuf[SizeOf(TCmdPack) + 1], Length(sMsg));
    //加密编码TempBuf中的内容，保存到SendBuf中去
    iLen := EncodeBuffer(@TempBuf[1], SizeOf(TCmdPack) + Length(sMsg), @SendBuf[1], SizeOf(SendBuf));
  end
  else
  begin
    iLen := EncodeBuffer(@TempBuf[1], SizeOf(TCmdPack), @SendBuf[1], SizeOf(SendBuf));
  end;
  SendBuf[iLen + 1] := '!';
  if not m_tIOCPSender.SendData(m_pOverlapSend, @SendBuf[0], iLen + 2) then
  begin
    InterlockedExchange(Integer(m_pOverlapRecv.Socket), Integer(INVALID_SOCKET));
  end;
end;

procedure TSessionObj.ProcessCltData(const Addr, Len: Integer; var Succeed: BOOL; const fCDPacket: Boolean);
var
  i, nBuffer: Integer;
  nABuf, nBBuf: PAnsiChar;
  nNewIDCode, nRand: Integer;
  nDeCodeLen: Integer;
  nEnCodeLen: Integer;

  Cmd: TCmdPack;
  CltCmd: TCmdPack;

  pszBuf: array[0..1024 - 1] of Char;
  sSend, sRecv: string;
  szAccount: string;
  szKey1, szKey2: string;

  PwdChk, Direction: Integer;
  ShiftVal, PasswordDigit: Integer;
  pUserEntry: ^TUserEntry;
  log: string;

  IsPackError: Boolean;

  TempLen: Integer;
  a, b, c: DWORD;
  k: Integer;
  Key: LongWord;
  ArrKey: array[0..7] of Byte;
  sSoftVersion, sMachineID: string;
label
  labFailExit;
begin
  if m_fKickFlag then
  begin //m_fKickFlag为真，表示连接进来的用户已被踢掉，直接反转m_fKickFlag标志，退出
    m_fKickFlag := False;
    Succeed := False;
    Exit;
  end;

  if Len > g_pConfig.m_nNomClientPacketSize then
  begin
    if g_pLogMgr.CheckLevel(4) then
      g_pLogMgr.Add('数据包超长: ' + IntToStr(Len));
    KickUser(m_pUserOBJ.nIPAddr);
    Succeed := False;
    Exit;
  end;

  PByte(Addr + Len)^ := 0;

  if (Len >= 5) and g_pConfig.m_fDefenceCCPacket then
  begin
    if (StrPos(PChar(Addr), 'HTTP/') <> nil) then
    begin //StrPos返回第二个参数在第一个参数中第一次出现的位置指针
      if g_pLogMgr.CheckLevel(6) then
        g_pLogMgr.Add('CC Attack, Kick: ' + m_pUserOBJ.pszIPAddr);
      KickUser(m_pUserOBJ.nIPAddr); //Kick即英文踢的意思
      Succeed := False;
      Exit;
    end;
  end;

  if (Len >= 1) then
  begin
    if (StrPos(PChar(Addr), '$') <> nil) then
    begin
      if g_pLogMgr.CheckLevel(6) then
        g_pLogMgr.Add('$ Attack, Kick: ' + m_pUserOBJ.pszIPAddr);
      KickUser(m_pUserOBJ.nIPAddr);
      Succeed := False;
      Exit;
    end;
  end;

  if gDeny then
  begin
    KickUser(m_pUserOBJ.nIPAddr);
    Succeed := False;
    Exit;
  end;

  //到这里，m_pOverlapRecv.ABuffer和m_pOverlapRecv.BBuffer，应该已经被赋过值了
  nABuf := PAnsiChar(m_pOverlapRecv.ABuffer); //这时的nABuf是整数，作为内存地址来使用
  nBBuf := PAnsiChar(m_pOverlapRecv.BBuffer);

  if Len < DEF_BLOCK_SIZE then   // 2021-01-05
  begin
    KickUser(m_pUserOBJ.nIPAddr);
    Succeed := False;
    Exit;
  end;

  CltCmd := DecodeMessage(PAnsiChar(Addr), DEF_BLOCK_SIZE); // 2021-01-05
  SetLength(sRecv, Len - DEF_BLOCK_SIZE);                   // 2021-01-05
  if Length(sRecv) > 0 then
  begin
    Move(PChar(Addr + DEF_BLOCK_SIZE)^, sRecv[1], Length(sRecv));   // 2021-01-05
  end;

  //nDeCodeLen := DecodeBuffer(PAnsiChar(Addr), Len, nABuf, TEMP_ENCODE_LEN); //把参数Addr的值，进行解密，数据的长度是Len，解密后存到nABuf中
  //nABuf[nEnCodeLen] := #0;

{.$I VMProtectBegin.inc}
  IsPackError := False;
  if Length(sRecv) > 0 then
  begin
    ArrKey[0] := m_dwProtocolPassword and $0000000F;
    ArrKey[1] := m_dwProtocolPassword and $000000F0;
    ArrKey[2] := m_dwProtocolPassword and $00000F00;
    ArrKey[3] := m_dwProtocolPassword and $0000F000;
    ArrKey[4] := m_dwProtocolPassword and $000F0000;
    ArrKey[5] := m_dwProtocolPassword and $00F00000;
    ArrKey[6] := m_dwProtocolPassword and $0F000000;
    ArrKey[7] := m_dwProtocolPassword and $F0000000;

    TempLen := Length(sRecv);
    k := 1;

{$IF VER_TYPE = 1}
    a := g_nProtocolKey1;
    b := g_nProtocolKey2;
{$ELSE}
    a := $801B0D01;
    b := $5DAE409F;
{$IFEND}

    c := m_dwProtocolPassword;

    while TempLen >= 12 do
    begin
      a := a + DWORD(Ord(sRecv[k + 0]) + (Ord(sRecv[k + 1]) shl 8) + (Ord(sRecv[k + 2]) shl 16) + (Ord(sRecv[k + 3]) shl 24));
      b := b + DWORD(Ord(sRecv[k + 4]) + (Ord(sRecv[k + 5]) shl 8) + (Ord(sRecv[k + 6]) shl 16) + (Ord(sRecv[k + 7]) shl 24));
      c := c + DWORD(Ord(sRecv[k + 8]) + (Ord(sRecv[k + 9]) shl 8) + (Ord(sRecv[k + 10]) shl 16) + (Ord(sRecv[k + 11]) shl 24));

      // a -= b; a -= c; a ^= c >> 13;
      a := a - b; a := a - c; a := a xor (c shr ArrKey[5]);

      // b -= c; b -= a; b ^= a << 8;
      b := b - c; b := b - a; b := b xor (a shl ArrKey[7]);

      // c -= a; c -= b; c ^= b >> 13;
      c := c - a; c := c - b; c := c xor (b shr ArrKey[2]);

      // a -= b; a -= c; a ^= c >> 12;
      a := a - b; a := a - c; a := a xor (c shr ArrKey[4]);

      // b -= c; b -= a; b ^= a << 16;
      b := b - c; b := b - a; b := b xor (a shl ArrKey[3]);

      // c -= a; c -= b; c ^= b >> 5;
      c := c - a; c := c - b; c := c xor (b shr ArrKey[0]);

      // a -= b; a -= c; a ^= c >> 3;
      a := a - b; a := a - c; a := a xor (c shr ArrKey[6]);

      // b -= c; b -= a; b ^= a << 10;
      b := b - c; b := b - a; b := b xor (a shl ArrKey[1]);

      // c -= a; c -= b; c ^= b >> 15;
      c := c - a; c := c - b; c := c xor (b shr 3);

      Inc(k, 12);
      Dec(TempLen, 12);
    end;

    c := c + DWORD(Length(sRecv));

    if TempLen >= 11 then
      c := c + DWORD(Ord(sRecv[k + 10]) shl 24);
    if TempLen >= 10 then
      c := c + DWORD(Ord(sRecv[k + 9]) shl 16);
    if TempLen >= 9 then
      c := c + DWORD(Ord(sRecv[k + 8]) shl 8);

    if TempLen >= 8 then
      b := b + DWORD(Ord(sRecv[k + 7]) shl 24);
    if TempLen >= 7 then
      b := b + DWORD(Ord(sRecv[k + 6]) shl 16);
    if TempLen >= 6 then
      b := b + DWORD(Ord(sRecv[k + 5]) shl 8);
    if TempLen >= 5 then
      b := b + DWORD(Ord(sRecv[k + 4]));

    if TempLen >= 4 then
      a := a + DWORD(Ord(sRecv[k + 3]) shl 24);
    if TempLen >= 3 then
      a := a + DWORD(Ord(sRecv[k + 2]) shl 16);
    if TempLen >= 2 then
      a := a + DWORD(Ord(sRecv[k + 1]) shl 8);
    if TempLen >= 1 then
      a := a + DWORD(Ord(sRecv[k + 0]));

    // a -= b; a -= c; a ^= c >> 13;
    a := a - b; a := a - c; a := a xor (c shr ArrKey[5]);

    // b -= c; b -= a; b ^= a << 8;
    b := b - c; b := b - a; b := b xor (a shl ArrKey[7]);

    // c -= a; c -= b; c ^= b >> 13;
    c := c - a; c := c - b; c := c xor (b shr ArrKey[2]);

    // a -= b; a -= c; a ^= c >> 12;
    a := a - b; a := a - c; a := a xor (c shr ArrKey[4]);

    // b -= c; b -= a; b ^= a << 16;
    b := b - c; b := b - a; b := b xor (a shl ArrKey[3]);

    // c -= a; c -= b; c ^= b >> 5;
    c := c - a; c := c - b; c := c xor (b shl ArrKey[0]);

    // a -= b; a -= c; a ^= c >> 3;
    a := a - b; a := a - c; a := a xor (c shr ArrKey[6]);

    // b -= c; b -= a; b ^= a << 10;
    b := b - c; b := b - a; b := b xor (a shl ArrKey[1]);

    // c -= a; c -= b; c ^= b >> 15;
    c := c - a; c := c - b; c := c xor (b shr 3);

    if c <> LongWord(CltCmd.Recog) then
    begin
      IsPackError := True;
      sRecv := '';
    end
    else
    begin
      sRecv := DecodeString(sRecv);

{$IF VER_TYPE = 1}
      DecryptDes(sRecv[1], sRecv[1], Length(sRecv), IntToStr(m_dwProtocolPassword or (m_dwProtocolPassword xor g_nProtocolKey3)));
{$ELSE}
      DecryptDes(sRecv[1], sRecv[1], Length(sRecv), IntToStr(m_dwProtocolPassword));
{$IFEND}
    end;
  end;
{.$I VMProtectEnd.inc}

  if IsPackError then
  begin
{.$I VMProtectBeginUltra.inc}
    // g_pLogMgr.Add(Format('客户端有错误的请求: %s', [m_pUserOBJ.pszIPAddr]));
    KickUser(m_pUserOBJ.nIPAddr);
{.$I VMProtectEnd.inc}
    Exit;
  end;

  (*
  if (m_fHandleLogin = 0) and (CltCmd.Cmd = CM_QUERYDYNCODE) then
  begin
    m_dwClientTimeOutTick := GetTickCount;
{$I '..\Common\Macros\VMPBM.inc'}
    if (nDeCodeLen > SizeOf(TCmdPack)) then
    begin

      if Length(sRecv) > 4 then
      begin
        g_DCP_mars.InitStr('');
        szKey1 := g_DCP_mars.DecryptString(g_pszDecodeKey^);
        g_DCP_mars.InitStr(szKey1);
        szKey2 := g_DCP_mars.DecryptString(sRecv);

        //ShowMessage(szKey1 + '  ' + szKey2 + '   ' + g_pszDecodeKey^ + '   ' + sRecv);

        if (szKey1 = szKey2) and (g_nEndeBufLen > 0) then
        begin
          m_fHandleLogin := 2;

          Randomize();
          nRand := Random(High(Word)); //得到一个随机数
          g_DCP_mars.InitStr(IntToStr(nRand)); //把这个随机数作为加密的Key
          g_DCP_mars.Reset;
          szKey1 := EncodeString(g_DCP_mars.EncryptString(g_pszDecodeKey^));
          m_wRandKey := Random(High(Word)); //再次取得一个随机数

          Cmd.ident := SM_QUERYDYNCODE;
          Cmd.Recog := g_pLTCrc^;
          Cmd.param := nRand; //这是用于加密和解密的Key，但是需要转成String
          Cmd.tag := m_wRandKey;
          Cmd.Series := Length(szKey1);

          EncodeBuf(Integer(@Cmd), SizeOf(TCmdPack), Integer(@pszBuf[0])); //把Cmd加密，保存到pszBuf中
          EncodeBuf(Integer(@g_pszEndeBuffer[0]), g_nEndeBufLen, nBBuf);
          sSend := '#' + StrPas(@pszBuf[0]) + szKey1 + StrPas(PChar(nBBuf)) + '!';

          //ShowMessage('待发送的数据是： ' + sSend);

          if not m_tIOCPSender.SendData(m_pOverlapSend, @sSend[1], Length(sSend)) then
          begin
            InterlockedExchange(Integer(m_pOverlapRecv.Socket), Integer(INVALID_SOCKET));
          end;


          if CltCmd.tag = 1 then // 登录器访问
          begin
            SendQueryData;
          end;

        end;
      end;
    end;
{$I '..\Common\Macros\VMPE.inc'}
    Exit;
  end;
  *)

  m_fHandleLogin := 2;

  if m_fHandleLogin = 2 then
  begin
    case CltCmd.Ident of
      CM_GETVERIFICATIONCODE,
        CM_PHONELOGIN,
        CM_REALNAME,
        CM_GETPASSWORDBACK_PHONE,//找回密码 根据手机
        CM_GETPASSWORDBACK,//找回密码 根据密保
        CM_CHANGEPHONE,//修改密码 根据手机
        CM_CHANGEPASSWORD_NEW,//修改密码 根据密保
        CM_CREATEACCOUNT,
        CM_BINDPHONE,
        CM_QUICKLOGIN,
        CM_MACHINEID,
        CM_IDPASSWORD,
        CM_ADDNEWUSER,
        CM_UPDATEUSER,
        CM_SELECTSERVER,
        CM_CHANGEPASSWORD,
        CM_CHANGERANDOMCODE,
        CM_CHECKRANDOMCODE,
        CM_SETL2PASSWORD,
        CM_GETBACKPASSWORD,
        CM_CHECKL2PASSWORD:
        begin
          if (CltCmd.Ident = CM_SETL2PASSWORD) then
          begin
            if not m_IsCanSetL2Password then
            begin
              if g_pLogMgr.CheckLevel(1) then
                g_pLogMgr.Add('非法设置二级密码: ' + m_pUserOBJ.pszIPAddr);

              KickUser(m_pUserOBJ.nIPAddr);
              Succeed := False;
              Exit;
            end
            else
            begin
              m_IsCanSetL2Password := False;
            end;
          end
          else if (CltCmd.Ident = CM_CHECKL2PASSWORD) then
          begin
            if not m_IsCanCheckL2Password then
            begin
              if g_pLogMgr.CheckLevel(1) then
                g_pLogMgr.Add('非法检测二级密码:  ' + m_pUserOBJ.pszIPAddr);

              KickUser(m_pUserOBJ.nIPAddr);
              Succeed := False;
              Exit;
            end
            else
            begin
              m_IsCanCheckL2Password := False;
            end;
          end
          else if CltCmd.Ident = CM_MACHINEID then
          begin
            sRecv := DecodeString(sRecv);         // 修正版本号判断错误 2020-06-09
            sSoftVersion := GetValidStr3(sRecv, sMachineID, ['/']);
            if (g_pConfig.m_boCheckVersion) then
            begin
              if not SameText(g_pConfig.m_sClientSoftVer, sSoftVersion) then
              begin
                CltCmd := MakeDefaultMsg(SM_CHECKCLIENTVERSION_FAIL, 0, 0, 0, 0);

                sSend := '#' + EncodeMessage(CltCmd) + '!';

                if not m_tIOCPSender.SendData(m_pOverlapSend, PChar(sSend), Length(sSend)) then
                begin
                  InterlockedExchange(Integer(m_pOverlapRecv.Socket), Integer(INVALID_SOCKET));
                end;

                DelayClose(1000);
              end;
            end;

            (*
            if (Length(sMachineID) > 0) and IsBlockMachine(sMachineID) then
            begin
              Inc(dwAttackCount);
              dwAttackTick := GetTickCount;
              dwClearTempTick := GetTickCount;
              dwResotreDefenseTick := GetTickCount;

              { TODO -ochongchong -c增加 : 受攻击防御调为1级 【2013-08-30】 }
              if boDefenseToLevel1 and (dwAttackCount >= dwDefenseToLevel1) then
              begin
                if dwDefenseLevel <> 1 then
                begin
                  dwDefenseLevel := 1;
                  MainOutMessage('自动调整防御调为1级', 1);
                end;
              end;

              MainOutMessage('过滤机器码: ' + sMachineID, 1);

              KickUser(m_pUserOBJ.nIPAddr);
              Succeed := False;
            end;
            *)
            Exit;
          end;

          nEnCodeLen := EncodeMessage(CltCmd, nBBuf, TEMP_ENCODE_LEN);
          if sRecv <> '' then
          begin
            Move(sRecv[1], PChar(nBBuf + nEnCodeLen)^, Length(sRecv));
          end;

          nBBuf[nEnCodeLen + Length(sRecv)] := #0;

          pszBuf[0] := '%';
          StrFmt(@pszBuf[1], 'A%d/#1%s!$', [m_pUserOBJ._SendObj.Socket, PChar(nBBuf)]);
          m_tLastGameSvr.SendBuffer(@pszBuf[0], StrLen(pszBuf));
        end;

      (*
      CM_IDPASSWORD:
        begin
          szKey1 := IntToHex(m_wRandKey, 8);

          Direction := 1;
          PasswordDigit := 1;
          PwdChk := 0;
          for I := 1 to Length(szKey1) do
            Inc(PwdChk, Ord(szKey1[I]));

          for I := 1 to Length(sRecv) do
          begin
            if Length(szKey1) = 0 then
              ShiftVal := I
            else
              ShiftVal := Ord(szKey1[PasswordDigit]);
            if Odd(I) then
              sRecv[I] := RotateBits(sRecv[I], -Direction * (ShiftVal + PwdChk))
            else
              sRecv[I] := RotateBits(sRecv[I], Direction * (ShiftVal + PwdChk));
            Inc(PasswordDigit);
            if PasswordDigit > Length(szKey1) then
              PasswordDigit := 1;
          end;

          Move(sRecv[1], PChar(nABuf + SizeOf(TCmdPack))^, Length(sRecv));
          nEnCodeLen := EncodeBuf(Integer(nABuf), nDeCodeLen, Integer(nBBuf));
          nBBuf[nEnCodeLen] := #0;

          pszBuf[0] := '%';
          StrFmt(@pszBuf[1], 'A%d/#1%s!$', [m_pUserOBJ._SendObj.Socket, PChar(nBBuf)]);
          m_tLastGameSvr.SendBuffer(@pszBuf[0], StrLen(pszBuf));
{$I '..\Common\Macros\VMPE.inc'}
        end;
      CM_PROTOCOL,
        CM_SELECTSERVER,
        CM_CHANGEPASSWORD,
        CM_UPDATEUSER,
        CM_GETBACKPASSWORD:
        begin
          m_dwClientTimeOutTick := GetTickCount;

          nEnCodeLen := EncodeBuf(Integer(nABuf), nDeCodeLen, Integer(nBBuf));
          nBBuf[nEnCodeLen] := #0;

          pszBuf[0] := '%';
          StrFmt(@pszBuf[1], 'A%d/#1%s!$', [m_pUserOBJ._SendObj.Socket, PChar(nBBuf)]);
          m_tLastGameSvr.SendBuffer(@pszBuf[0], StrLen(pszBuf));
        end;

      CM_ADDNEWUSER:
        begin
          m_dwClientTimeOutTick := GetTickCount;
          if nDeCodeLen > SizeOf(TCmdPack) then
          begin
            pUserEntry := Pointer(nABuf + SizeOf(TCmdPack));
            szAccount := Trim(pUserEntry.sAccount);
            nNewIDCode := 1;
            if (szAccount = '') or (Length(szAccount) < 2) then
            begin
              nNewIDCode := -1;
              goto labFailExit;
            end;
            if not CheckAccountName(szAccount) then
            begin
              nNewIDCode := -1;
              goto labFailExit;
            end;

          labFailExit:
            if nNewIDCode <> 1 then
            begin
              SendDefMessage(SM_NEWID_FAIL, nNewIDCode, 0, 0, 0, '');
              Exit;
            end;

            if CheckNewIDOfIP(m_pUserOBJ.nIPAddr) then
            begin
              KickUser(m_pUserOBJ.nIPAddr);
              Succeed := False;
              if g_pLogMgr.CheckLevel(4) then
                g_pLogMgr.Add(Format('注册帐号超速: %s', [m_pUserOBJ.pszIPAddr]));
              Exit;
            end;

            nEnCodeLen := EncodeBuf(Integer(nABuf), nDeCodeLen, Integer(nBBuf));
            nBBuf[nEnCodeLen] := #0;

            pszBuf[0] := '%';
            StrFmt(@pszBuf[1], 'A%d/#1%s!$', [m_pUserOBJ._SendObj.Socket, PChar(nBBuf)]);
            m_tLastGameSvr.SendBuffer(@pszBuf[0], StrLen(pszBuf));
          end
          else
            Exit;
        end;
      *)
    else
      begin
        if g_pLogMgr.CheckLevel(4) then
          g_pLogMgr.Add(Format('错误的数据包索引: %d', [CltCmd.Ident]));
        KickUser(m_pUserOBJ.nIPAddr);
        Succeed := False;
      end;
    end;
  end;
end;

procedure TSessionObj.ProcessSvrData(GS: TClientThread; const Addr, Len: Integer);
begin
  if m_fKickFlag then
  begin
    m_fKickFlag := False;
    InterlockedExchange(Integer(m_pOverlapRecv.Socket), Integer(INVALID_SOCKET));
    SHSocket.FreeSocket(m_pOverlapSend.Socket);
    Exit;
  end;

  if not m_tIOCPSender.SendData(m_pOverlapSend, PChar(Addr), Len) then
  begin
    InterlockedExchange(Integer(m_pOverlapRecv.Socket), Integer(INVALID_SOCKET));
  end;
end;

procedure TSessionObj.UserEnter;
var
  szSendBuf: string;
  dwTemp: array[0..3] of LongWord;
begin
  Inc(enterCount);
  m_fHandleLogin := 0;
  g_ProcMsgThread.AddSession(Self);

  Randomize;
  m_dwProtocolPassword := 1 + Random(High(Integer) - 1);

  m_IsCanSetL2Password := False;
  m_IsCanCheckL2Password := False;

  szSendBuf := '%' + Format('O%d/%s/%s$', [m_pUserOBJ._SendObj.Socket, m_pUserOBJ.pszIPAddr, m_pUserOBJ.pszLocalIPAddr]);
  m_tLastGameSvr.SendBuffer(@szSendBuf[1], Length(szSendBuf));

  // 不确定是否客户端连上后立即发消息到客户端是否有错误 2020-01-16
{.$I VMProtectBegin.inc}
  dwTemp[0] := Random(High(Integer) - 1);
  dwTemp[1] := Random(High(Integer) - 1);
  dwTemp[2] := Random(High(Integer) - 1);
  dwTemp[3] := m_dwProtocolPassword xor dwTemp[0] xor dwTemp[1] xor dwTemp[2];

  szSendBuf := EncodeBuffer(@dwTemp[0], SizeOf(dwTemp));
  if not m_tIOCPSender.SendData(m_pOverlapSend, @szSendBuf[1], Length(szSendBuf)) then
  begin
    InterlockedExchange(Integer(m_pOverlapRecv.Socket), Integer(INVALID_SOCKET));
  end;
{.$I VMProtectEnd.inc}
end;

procedure TSessionObj.UserLeave;
var
  szSenfBuf: string;
  i, nCode: Integer;
  DynPacket: pTDynPacket;
begin
  nCode := 0;
  try
    nCode := 1;
    m_fHandleLogin := 0;
    szSenfBuf := '%' + Format('X%d$', [m_pUserOBJ._SendObj.Socket]);
    m_tLastGameSvr.SendBuffer(@szSenfBuf[1], Length(szSenfBuf));

    nCode := 2;
    DeleteConnectOfIP(Self.m_pUserOBJ.nIPAddr);

    nCode := 3;
    i := 0;
    EnterCriticalSection(PSendQueueNode(m_pOverlapSend).QueueLock);
    try
      while True do
      begin
        i := 1;
        if PSendQueueNode(m_pOverlapSend).DynSendList.Count = 0 then
          Break;
        DynPacket := m_tIOCPSender.SendQueue.GetDynPacket(m_pOverlapSend);
        if DynPacket = nil then
          Break;
        i := 2;
        FreeMem(DynPacket.Buf);
        Dispose(DynPacket);
        PSendQueueNode(m_pOverlapSend).DynSendList.Delete(0);
      end;
    finally
      LeaveCriticalSection(PSendQueueNode(m_pOverlapSend).QueueLock);
    end;

    nCode := 4;
    if g_ProcMsgThread <> nil then
      g_ProcMsgThread.DelSession(Self);
  except
    on M: Exception do
      g_pLogMgr.Add(Format('TSessionObj.UserLeave: %d %s', [nCode, M.Message]));
  end;
end;

procedure TSessionObj.DelayClose(DelayTick: LongWord);
begin
  m_dwDelayCloseTick := GetTickCount + DelayTick;
  m_IsDelayClose := True;
end;

procedure FillUserList();
var
  i: Integer;
begin
  if g_pFillUserObj = nil then
    g_pFillUserObj := TSessionObj.Create;
  g_pFillUserObj.m_tLastGameSvr := nil;
  for i := 0 to USER_ARRAY_COUNT - 1 do
    g_UserList[i] := g_pFillUserObj;
end;

procedure CleanupUserList();
begin
  if g_pFillUserObj <> nil then
    g_pFillUserObj.Free;
end;

initialization
  FillUserList();
  gDeny := False;

  starttick := GetTickCount;
  lastqueryTick := starttick;
  enterCount := 0;

finalization
  CleanupUserList();

end.

