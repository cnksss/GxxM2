{*******************************************************************************

 * qrencode - QR Code encoder
 *
 * QR Code encoding tool
 * This code is taken from Kentaro Fukuchi's qrenc.c
 * then editted and packed into a .pas file.
 * Copyright (C) 2006-2011 Kentaro Fukuchi <kentaro@fukuchi.org>
 *
 * Copyright (C) 2014 Hao Shi <admin@hicpp.com> 
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
                                                               
    revision history
      2014-04-14  update from qrencode-3.4.3

*******************************************************************************}

unit qrenc;

interface

uses
Windows,
SysUtils,
Classes,
struct,
Graphics,
SyncObjs;

function qr(const sText: AnsiString; IsUtf8String:Boolean; nMargin, nDotSize, nEightBit, nCaseSensitive, nLevel:Integer; nForeColor, nBackColor:UInt ):TBitmap;

implementation

uses
qrencode;

var
qrCriticalSection:TCriticalSection; 


function writeBMP(qrcode: PQRcode; pQRParam:PQRCodeGenerateParam):TBitmap;
var
  nRealWidth, x, xx, y, yy: Integer;
  p: PByte;
  pixDest:TRGBTriple;
  pix, pixNew : PRGBTriple;
begin
  if(qrcode <> nil) and (qrcode.width > 0) and (qrcode.data <> nil) then begin
      nRealWidth := (qrcode.width + pQRParam.nMargin * 2) * pQRParam.nDotSize;
      Result := TBitmap.Create;
      Result.PixelFormat := pf24bit;
      Result.SetSize(nRealWidth, nRealWidth);
      Result.Canvas.Brush.Color := pQRParam.nBackColor;
      Result.Canvas.FillRect(Bounds(0,0, nRealWidth, nRealWidth));

      pixDest.rgbtBlue := (pQRParam.nForeColor shr 16) and $FF;
      pixDest.rgbtGreen := (pQRParam.nForeColor shr 8) and $FF;
      pixDest.rgbtRed := (pQRParam.nForeColor) and $FF;
      //设置需要改变的像素为前景色
      for y := 0 to qrcode.width - 1 do begin
        p := PIndex(qrcode.data, y * qrcode.width);   //当前需要测试的数据
        pix := Result.ScanLine[(y + pQRParam.nMargin) * pQRParam.nDotSize];  //当前需要改变颜色的像素
        Inc(pix, pQRParam.nMargin * pQRParam.nDotSize);                      //跳过每行的margin
        for x := 0 to qrcode.width - 1 do begin
          if (p^ and 1) <> 0 then begin   //需要改变的像素
               for xx := 0 to pQRParam.nDotSize - 1 do begin  //重复size大小的前景色
                   pix^ := pixDest;
                   Inc(pix);
               end;
          end else begin  //跳过不需要改变的像素
              Inc(pix, pQRParam.nDotSize);
          end;
          Inc(p);
        end;
        //总共size行，其它行的数据复制当前行
        pix := Result.ScanLine[(y + pQRParam.nMargin) * pQRParam.nDotSize];
        for yy := 1 to pQRParam.nDotSize - 1 do begin
          pixNew := Result.ScanLine[(y + pQRParam.nMargin) * pQRParam.nDotSize + yy];
          CopyMemory(pixNew, pix, SizeOf(TRGBTriple) * nRealWidth);
        end;
      end;
  end else begin
      Result := nil;
  end;
end;

(*
function writeBMP(qrcode: PQRcode; var bmp: TBitmap): Integer;
var
  //bmp: TBitmap;
  realwidth, x, xx, y, yy: Integer;
  p: PByte;
  pix, pixNew: PRGBTriple;
begin
  Result := -1;
  if qrcode = nil then exit;
  
  realwidth := (qrcode.width + margin * 2) * size;
  bmp := TBitmap.Create;
  try
    bmp.PixelFormat := pf24bit;
//    bmp.HandleType := bmDDB;
    bmp.Width := realwidth;
    bmp.Height := realwidth;
    //设置背景色（整个图片全部设置成背景色，然后设置需要改变的像素为前景色）开始，
    //设置第一行的颜色
    pix := bmp.ScanLine[0];
    for x := 0 to realwidth - 1 do
    begin
      pix^.rgbtRed := bg_color[0];
      pix^.rgbtGreen := bg_color[1];
      pix^.rgbtBlue := bg_color[2];
      Inc(pix);
    end;
    //后面行的数据复制第一行
    pix := bmp.ScanLine[0];
    for y := 1 to realwidth - 1 do
    begin
      pixNew := bmp.ScanLine[y];
      CopyMemory(pixNew, pix, SizeOf(TRGBTriple) * realwidth);
    end;
    //设置背景色结束

    //设置需要改变的像素为前景色
    for y := 0 to qrcode.width - 1 do
    begin
      p := PIndex(qrcode.data, y * qrcode.width);   //当前需要测试的数据
      pix := bmp.ScanLine[(y + margin) * size];     //当前需要改变颜色的像素
      Inc(pix, margin * size);                      //跳过每行的margin
      for x := 0 to qrcode.width - 1 do
      begin
        if (p^ and 1) <> 0 then   //需要改变的像素
        begin
          for xx := 0 to size - 1 do  //重复size大小的前景色
          begin
            pix^.rgbtRed := fg_color[0];
            pix^.rgbtGreen := fg_color[1];
            pix^.rgbtBlue := fg_color[2];
            Inc(pix);              
          end;
        end else  //跳过不需要改变的像素
          Inc(pix, size);
        Inc(p);
      end;
      //总共size行，其它行的数据复制当前行
      pix := bmp.ScanLine[(y + margin) * size];
      for yy := 1 to size - 1 do
      begin
        pixNew := bmp.ScanLine[(y + margin) * size + yy];
        CopyMemory(pixNew, pix, SizeOf(TRGBTriple) * realwidth);
      end;
    end;


    //pix := nil;
    //pixNew := nil;
    Result := 0;
  finally
    //FreeAndNil(bmp);
  end;
end;
*)

function encode(const intext: PByte; length: Integer; pQRParam:PQRCodeGenerateParam): PQRcode;
var
  code: PQRcode;
begin
	if pQRParam.nMicro <> 0 then begin
		if pQRParam.nEightBit <> 0 then begin
			code := QRcode_encodeDataMQR(length, intext, pQRParam.nVersion, pQRParam.nLevel);
		end else begin
			code := QRcode_encodeStringMQR(PAnsiChar(intext), pQRParam.nVersion, pQRParam.nLevel, pQRParam.qrmHint, pQRParam.nCaseSensitive);
		end;
	end else begin
		if pQRParam.nEightBit <> 0 then begin
			code := QRcode_encodeData(length, intext, pQRParam.nVersion, pQRParam.nLevel);
		end else begin
			code := QRcode_encodeString(PAnsiChar(intext), pQRParam.nVersion, pQRParam.nLevel, pQRParam.qrmHint, pQRParam.nCaseSensitive);
		end;
	end;
	Result := code;
end;

function qrcode(const intext: PByte; length: Integer; pQRParam:PQRCodeGenerateParam):TBitmap;
var
  qrcode: PQRcode;
begin
	qrcode := encode(intext, length, pQRParam);
  if qrcode <> nil then begin
     Result := WriteBmp(qrcode, pQRParam);
     QRcode_free(qrcode);
  end else begin
     Result := nil;
  end;
end;

function encodeStructured(const intext: PByte; length: Integer; pQRParam:PQRCodeGenerateParam): PQRcode_List;
var
  list: PQRcode_List;
begin
	if pQRParam.nEightBit <> 0 then begin
		list := QRcode_encodeDataStructured(length, intext, pQRParam.nVersion, pQRParam.nLevel);
	end else begin
		list := QRcode_encodeStringStructured(PAnsiChar(intext), pQRParam.nVersion, pQRParam.nLevel, pQRParam.qrmHint, pQRParam.nCaseSensitive);
	end;
	Result := list;
end;

procedure qrencodeStructured(const intext: PByte; length: Integer; pQRParam:PQRCodeGenerateParam);
var
  qrlist, p: PQRcode_List;
begin
   qrlist := encodeStructured(intext, length, pQRParam);
   if qrlist = nil then begin
       Exit;
	 end;
  p := qrlist;
  try
      while p <> nil do  begin
        if p.code = nil then begin
           Continue;
        end;
        //所谓结构化，就是一次可以生成多个QRCode
        //var bmp:TBitmap;
        //bmp := writeBMP(p.code, pQRParam);
        p := p.next;
      end;
  finally
      QRcode_List_free(qrlist);
  end;
end;

function qrencode(const AStr: PByte; ALen: Integer; pQRParam:PQRCodeGenerateParam):TBitmap;
begin
  //if structured = 1 then begin
  //   qrencodeStructured(AStr, ALen, PAnsiChar(AOut))
  //end else begin
  Result := qrcode(AStr, ALen, pQRParam);
  //end;
end;

function qr(const sText: AnsiString; IsUtf8String:Boolean; nMargin, nDotSize, nEightBit, nCaseSensitive, nLevel:Integer; nForeColor, nBackColor:UInt ):TBitmap;
var
  sutf8: UTF8String;
  pb: PByte;
  iLen: Integer;
  qrParam:TQRCodeGenerateParam;
begin
  if IsUtf8String then begin
    sutf8 := AnsiToUtf8(sText);
    iLen := Length(sutf8);
    pb := PByte(PAnsiChar(sutf8));
  end else begin
    iLen := Length(sText);
    pb := PByte(PAnsiChar(sText));
  end;

  qrParam.nVersion := 1;
  qrParam.nLevel := QRecLevel(nLevel);
  qrParam.nCaseSensitive := nCaseSensitive;
  qrParam.nEightBit := nEightBit;
  qrParam.nDotSize := nDotSize;
  qrParam.nMargin := nMargin;
  qrParam.nMicro := 0;
  qrParam.nDpi := 72;
  qrParam.qrmHint := QR_MODE_8;
  qrParam.nForeColor := nForeColor;
  qrParam.nBackColor := nBackColor;

  qrCriticalSection.Enter; //HZQ 20230606 加临界区保护全局变量
  try
      Result := qrencode(pb, iLen, @qrParam);
      QRcode_clearCache();
  finally
      qrCriticalSection.Leave;
  end;
end; 

initialization
qrCriticalSection := TCriticalSection.Create;

finalization
qrCriticalSection.Free;

end.
