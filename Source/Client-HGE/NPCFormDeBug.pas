unit NPCFormDeBug;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Variants,
  Classes,
  Graphics,
  Controls,
  Forms,
  DxImageButton,
  Dialogs,
  StdCtrls,
  Fstate,
  MShare,
  GameImages,
  Spin,
  DxControls,
  DxCanvas;

type
  TFrmNPCDeBug = class(TForm)
    Memo1:TMemo;
    GroupBox1:TGroupBox;
    Label1:TLabel;
    ComboBox1:TComboBox;
    Label2:TLabel;
    SpinEdit1:TSpinEdit;
    Memo2:TMemo;
    Button1:TButton;
    Button2:TButton;
    procedure Button1Click(Sender:TObject);
    procedure Button2Click(Sender:TObject);
  private
    { Private declarations }
  public
    { Public declarations }
    procedure Open();
  end;

var
  FrmNPCDeBug:TFrmNPCDeBug;

implementation

{$R *.dfm}

procedure TFrmNPCDeBug.Open();
var
  I:Integer;
begin
  Show;
  ComboBox1.Clear;
  for I := 0 to g_EffectImageList.Count - 1 do begin
    ComboBox1.AddItem(g_EffectImageList[I], nil);
  end;
  if (ComboBox1.Items.Count > 0) then
    ComboBox1.ItemIndex := 0;
end;

procedure TFrmNPCDeBug.Button1Click(Sender:TObject);
var
  Str:string;
  Images:TGameImages;
begin
  Str := Memo1.Text;
  Str := StringReplace(Str, #13, '', [rfReplaceAll]);
  Str := StringReplace(Str, #10, '', [rfReplaceAll]);
  FrmDlg.ShowMDlg(0, '???', Str, not g_boOpenMerchantBigDlg, True);
  Images := TGameImages(g_EffectImageList.Objects[ComboBox1.ItemIndex]);
  FrmDlg.DMerchantDlg.ImageIndex.BeginUpdate;
  try
    FrmDlg.DMerchantDlg.ImageIndex.Image := Images;
    FrmDlg.DMerchantDlg.ImageIndex.Up := SpinEdit1.Value;
    FrmDlg.DMerchantDlg.ImageIndex.Hot := SpinEdit1.Value;
    FrmDlg.DMerchantDlg.ImageIndex.Down := SpinEdit1.Value;
    FrmDlg.DMerchantDlg.ImageIndex.Disabled := SpinEdit1.Value;
    FrmDlg.DMerchantDlg.ImageIndex.Checked := SpinEdit1.Value;
  finally
    FrmDlg.DMerchantDlg.ImageIndex.EndUpdate;
  end;
end;

procedure TFrmNPCDeBug.Button2Click(Sender:TObject);

  function CalcEuclidDistance(r1, r2, g1, g2, b1, b2:Integer):Integer; //HZQ 20230527
  var
    valR, valG, valB:Integer;
  begin
    valR := r2 - r1;
    valG := g2 - g1;
    valB := b2 - b1;
    Result := Trunc(sqrt((valR * ValR) + (valG * ValG) + (valB * valB) + 0.5));
  end;

  function ColorTo256(c:TColor):Byte;
  var
    I, R, G, B, nRlt:integer;
    //pRGB: TRGBQuad;
    nMinEd, nCurED:Integer;
  begin
    R := c and $FF;
    G := (c and $FF00) shr 8;
    B := (c and $FF0000) shr 16;

    nRlt := -1; //HZQ 20230527 默认返回-1，当都不匹配时，使用欧氏距离来确定最佳匹配索引
    for I := 0 to 256 - 1 do begin
      if (g_DefColorTable[I].rgbBlue = B) and (g_DefColorTable[I].rgbRed = R) and (g_DefColorTable[I].rgbGreen = G) then begin
        nRlt := I;
        Break;
      end
    end;

    (* HZQ 20230527以下通过计算两个颜色的欧氏距离来确定最佳匹配索引
    d = sqrt((R2 - R1)^2 + (G2 - G1)^2 + (B2 - B1)^2)
    其中，R1、G1、B1是指定颜色的RGB值，R2、G2、B2是每个索引色的RGB值。
    计算完每个索引色与指定颜色的欧氏距离后，找到距离最小的索引色即可。
    *)
    if nRlt < 0 then begin
      Result := 0;
      nMinEd := CalcEuclidDistance(R, g_DefColorTable[0].rgbRed, G, g_DefColorTable[0].rgbGreen, B, g_DefColorTable[0].rgbBlue);
      for i := 1 to 256 - 1 do begin
        nCurED := CalcEuclidDistance(R, g_DefColorTable[I].rgbRed, G, g_DefColorTable[I].rgbGreen, B, g_DefColorTable[I].rgbBlue);
        if nCurED < nMinEd then begin
          Result := I;
        end;
      end;
    end else begin
      Result := nRlt;
    end;
  end;

  function GetWilId(Img:TGameImages):Integer;
  begin
    Result := g_EffectImageList.IndexOfObject(TObject(Img));
  end;

  function GetFontStr(D:TDxImageButton):string;
  var
    I:Integer;
  begin
    Result := '';

    if D.Caption = '' then Exit;

    if (D is TNpcLabel) and (TNpcLabel(D).m_AutoColors.Count > 0) then begin
      Result := Result + 'AUTOCOLOR=';
      for I := 0 to TNpcLabel(D).m_AutoColors.Count - 1 do begin
        Result := Result + Format('%d,', [ColorTo256(Integer(TNpcLabel(D).m_AutoColors.Items[I]))]);
      end;
      Result := Result + ':';
    end else if D.CaptionColor.Down.Color <> clWhite then begin
      Result := Result + Format('FCOLOR=%d:', [ColorTo256(D.CaptionColor.Up.Color)]);
    end;

    if fsBold in D.CaptionColor.Up.Style then
      Result := Result + 'FBOLD:';

    if D.CaptionColor.Up.Size <> 9 then
      Result := Result + Format('FSIZE=%d:', [D.CaptionColor.Up.Size]);

    if D.CaptionColor.Up.Name <> '宋体' then
      Result := Result + Format('FNAME=%s:', [D.CaptionColor.Up.Name]);

  end;

  function DxControlToString(D:TDxControl):string;
  var
    {I,} nDID:Integer;
    NpcButton:TNpcButton;
    NpcEdit:TNpcInputEdit;
    //NpcItemButton: TNpcItemButton;
    NpcUserItemButton:TNpcUserItemButton;
    NpcItemBoxButton:TNpcItemBoxButton;
    NpcProgressBoxButton:TNpcProgressBoxButton;
    NpcLabel:TNpcLabel;
    CountDownLabel:TCountDownLabel;
    ImgCountDown:TImgCountDownButton;
    FontStr:string;
    OffsetX, OffsetY:Integer;
  const
    C_IMG = 'IMG';
    C_PLAYIMG = 'PLAYIMG';
    C_IMGEX = 'IMGEX';
    C_IMGNUM = 'IMGNUM';
    C_IMGPAY = 'IMGPAY';
    C_PLAYIMGEX = 'PLAYIMGEX';
    C_LOOKS = 'LOOKS';
    C_DNITEMS = 'DNITEMS';
    C_STATEITEM = 'STATEITEM';
    C_NEWOPUI = 'NEWOPUI';
  begin
    Result := '';
    FontStr := '';
    OffsetX := 0;

    OffsetY := 0;
    //    OffsetX := Ord((nDID = 0) and (not (D is TNpcScrollBox))) * g_ConfigClient.nNPCMsgDlgTextOffsetX + 20;
    //    OffsetY := Ord((nDID = 0) and (not (D is TNpcScrollBox))) * g_ConfigClient.nNPCMsgDlgTextOffsetY + 16;

    if D.ControlID = 0 then
      nDID := 99999
    else
      nDID := D.ControlID;

    if D is TNpcLabel then begin
      NpcLabel := TNpcLabel(D);
      Result := Format('<%d&TEXT', [nDID]);
      Result := Result + Format(':%s', [D.Caption]);
      if D.Hint <> '' then
        Result := Result + '|' + D.Hint;
      Result := Result + Format(':%d', [D.Left + OffsetX]);
      Result := Result + Format(':%d', [D.Top + OffsetY]);
      FontStr := GetFontStr(NpcLabel);
      if FontStr <> '' then
        Result := Result + '{' + FontStr + '}';

      if TNpcLabel(D).m_sCmd <> '' then
        Result := Result + '/' + TNpcLabel(D).m_sCmd;
      Result := Result + '>';
    end
    else if D is TNpcScrollBox then begin
      Result := Format('<%d&SCROLLBOX', [nDID]);
      if TNpcScrollBox(D).m_List.Count > 0 then
        Result := Result + Format(':%s', [TNpcScrollBox(D).m_List.DelimitedText]);
      Result := Result + Format(':%d', [GetWilId(D.ImageIndex.Image)]);
      Result := Result + Format(':%d', [D.ImageIndex.Up]);
      Result := Result + Format(':%d', [D.Left + OffsetX]);
      Result := Result + Format(':%d', [D.Top + OffsetY]);
      Result := Result + Format(':%d', [D.Width]);
      Result := Result + Format(':%d', [D.Height]);
      Result := Result + Format(':%d', [Integer(TNpcScrollBox(D).MouseHorizontal)]);
      Result := Result + '>';
    end
    else if D is TNpcItemButton then begin
      Result := Format('<%d&ITEMSHOW', [nDID]);
      Result := Result + Format(':%d', [TNpcItemButton(D).m_nFaceIndex]);
      Result := Result + Format(':%d', [TNpcItemButton(D).m_nCount]);
      Result := Result + Format(':%d', [D.Left + OffsetX]);
      Result := Result + Format(':%d', [D.Top + OffsetY]);
      Result := Result + Format(':%d', [Integer(TNpcItemButton(D).m_boShowBorder)]);
      Result := Result + Format(':%d', [TNpcItemButton(D).m_Light]);
      if TNpcItemButton(D).m_sCmd <> '' then
        Result := Result + '/' + TNpcItemButton(D).m_sCmd;
      Result := Result + '>';
    end
    else if D is TNpcInputEdit then begin
      NpcEdit := TNpcInputEdit(D);
      if NpcEdit.m_IsNumber then
        Result := Format('<%d&INPUTNUM', [nDID])
      else
        Result := Format('<%d&INPUTTEXT', [nDID]);

      Result := Result + Format(':%d', [NpcEdit.m_ID]);
      Result := Result + Format(':%d', [NpcEdit.Left + OffsetX]);
      Result := Result + Format(':%d', [NpcEdit.Top + OffsetY]);
      Result := Result + Format(':%d', [NpcEdit.Width]);
      Result := Result + Format(':%d', [NpcEdit.Height]);
      if NpcEdit.Transparent then
        Result := Result + ':-1'
      else
        Result := Result + Format(':%d', [ColorTo256(NpcEdit.BackgroundColor)]);
      if not NpcEdit.DrawBorder then
        Result := Result + ':-1'
      else
        Result := Result + Format(':%d', [ColorTo256(NpcEdit.BorderColor.Up.Color)]);
      Result := Result + Format(':%d', [ColorTo256(NpcEdit.Font.Color)]);

      Result := Result + Format(':%d', [NpcEdit.m_MinValue]);
      Result := Result + Format(':%d', [NpcEdit.m_MaxValue]);
      Result := Result + Format(':%s', [NpcEdit.m_ValidityTips]);
      Result := Result + Format(':%s', [NpcEdit.HintText]);
      Result := Result + Format(':%d', [ColorTo256(NpcEdit.HintTextFont.Color)]);
      Result := Result + '>';
    end
    else if D is TNpcItemBoxButton then begin
      NpcItemBoxButton := TNpcItemBoxButton(D);
      Result := Format('<%d&ITEMBOX', [nDID]);
      Result := Result + Format(':%d', [NpcItemBoxButton.m_nIndex]);
      Result := Result + Format(':%d', [GetWilId(NpcItemBoxButton.ImageIndex.Image)]);
      Result := Result + Format(':%d', [NpcItemBoxButton.ImageIndex.Up]);
      Result := Result + Format(':%d', [NpcItemBoxButton.Left + OffsetX]);
      Result := Result + Format(':%d', [NpcItemBoxButton.Top + OffsetY]);
      Result := Result + Format(':%d', [NpcItemBoxButton.Width]);
      Result := Result + Format(':%d', [NpcItemBoxButton.Height]);
      Result := Result + Format(':%s', [NpcItemBoxButton.m_StdModes]);
      Result := Result + '>';
    end
    else if D is TNpcProgressBoxButton then begin
      NpcProgressBoxButton := TNpcProgressBoxButton(D);
      Result := Format('<%d&PROGRESSBAR', [nDID]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.Left + OffsetX]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.Top + OffsetY]);
      Result := Result + Format(':%d', [GetWilId(NpcProgressBoxButton.ImageIndex.Image)]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nBgIndex]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressStartIndex]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressCount]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressRefresTime]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressOffsetX]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressOffsetY]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressMinValue]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressMaxValue]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressValue]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressDist]);
      Result := Result + Format(':%d', [ColorTo256(NpcProgressBoxButton.m_nProgressTextColor)]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressTextOffsetX]);
      Result := Result + Format(':%d', [NpcProgressBoxButton.m_nProgressTextOffsetY]);
      Result := Result + Format(':%s', [NpcProgressBoxButton.m_sText]);

      FontStr := GetFontStr(NpcProgressBoxButton);
      if FontStr <> '' then
        Result := Result + '{' + FontStr + '}';

      if NpcProgressBoxButton.m_sCmd <> '' then
        Result := Result + '/' + NpcProgressBoxButton.m_sCmd;
      Result := Result + '>';
    end
    else if D is TNpcUserItemButton then begin
      NpcUserItemButton := TNpcUserItemButton(D);
      Result := Format('<%d&PROGRESSBAR', [nDID]);
      Result := Result + Format(':%d', [NpcUserItemButton.m_nIndex]);
      Result := Result + Format(':%d', [NpcUserItemButton.Left + OffsetX]);
      Result := Result + Format(':%d', [NpcUserItemButton.Top + OffsetY]);
      Result := Result + Format(':%d', [Integer(NpcUserItemButton.m_boShowBorder)]);
      Result := Result + Format(':%d', [NpcUserItemButton.m_Light]);
      if NpcUserItemButton.m_sCmd <> '' then
        Result := Result + '/' + NpcUserItemButton.m_sCmd;
      Result := Result + '>';

    end
    else if D is TCountDownLabel then begin
      CountDownLabel := TCountDownLabel(D);
      Result := Format('<%d&COUNTDOWN', [nDID]);
      Result := Result + Format(':%d', [CountDownLabel.m_OldCountDownValue]);
      Result := Result + Format(':%d', [CountDownLabel.m_LoopCount]);
      Result := Result + Format(':%d', [ColorTo256(CountDownLabel.CaptionColor.Up.Color)]);
      Result := Result + Format(':%d', [CountDownLabel.Left + OffsetX]);
      Result := Result + Format(':%d', [CountDownLabel.Top + OffsetY]);

      FontStr := GetFontStr(CountDownLabel);
      if FontStr <> '' then
        Result := Result + '{' + FontStr + '}';

      if CountDownLabel.m_sCmd <> '' then
        Result := Result + '/' + CountDownLabel.m_sCmd;
      Result := Result + '>';
    end
    else if D is TImgCountDownButton then begin
      ImgCountDown := TImgCountDownButton(D);
      Result := Format('<%d&IMGCOUNTDOWN', [nDID]);
      Result := Result + Format(':%d', [ImgCountDown.m_OldCountDownValue]);
      Result := Result + Format(':%d', [ImgCountDown.m_LoopCount]);
      Result := Result + Format(':%d', [ImgCountDown.m_ImgStartIndex]);
      Result := Result + Format(':%d', [ImgCountDown.m_ImgSpace]);
      Result := Result + Format(':%d', [ImgCountDown.Left + OffsetX]);
      Result := Result + Format(':%d', [ImgCountDown.Top + OffsetY]);
      FontStr := GetFontStr(ImgCountDown);
      if FontStr <> '' then
        Result := Result + '{' + FontStr + '}';

      if ImgCountDown.m_sCmd <> '' then
        Result := Result + '/' + ImgCountDown.m_sCmd;
      Result := Result + '>';
    end
    else if D is TNpcButton then begin
      NpcButton := TNpcButton(D);
      Result := Format('<%d&%s', [nDID, NpcButton.m_ACaption]);
      if NpcButton.m_ACaption = C_IMG then begin
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [GetWilId(NpcButton.ImageIndex.Image)]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end
      else if NpcButton.m_ACaption = C_PLAYIMG then begin
        Result := Result + Format(':%d', [GetWilId(NpcButton.ImageIndex.Image)]);
        Result := Result + Format(':%d', [NpcButton.m_nStartImageIndex]);
        Result := Result + Format(':%d', [NpcButton.m_nStopImageCount - NpcButton.m_nStartImageIndex + 1]);
        Result := Result + Format(':%d', [NpcButton.m_dwPlayImageTime]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);

        Result := Result + Format(':%d', [Integer(NpcButton.BlendMode <> 2)]);

        if NpcButton.Hint <> '' then
          Result := Result + Format(':%s', [NpcButton.Hint]);

        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);

      end
      else if NpcButton.m_ACaption = C_IMGEX then begin
        Result := Result + Format(':%d', [GetWilId(NpcButton.ImageIndex.Image)]);
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Hot]);
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Down]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end
      else if NpcButton.m_ACaption = C_IMGNUM then begin
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Hot]);
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Down]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
      end
      else if NpcButton.m_ACaption = C_IMGPAY then begin
        Result := Result + ':请重新输入:请重新输入:请重新输入';
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        Result := Result + ':请重新输入';
      end
      else if NpcButton.m_ACaption = C_PLAYIMGEX then begin
        Result := Result + Format(':%d', [GetWilId(NpcButton.ImageIndex.Image)]);
        Result := Result + Format(':%d', [NpcButton.m_nStartImageIndex]);
        Result := Result + Format(':%d', [NpcButton.m_nStopImageCount - NpcButton.m_nStartImageIndex + 1]);
        Result := Result + Format(':%d', [NpcButton.m_dwPlayImageTime]);
        Result := Result + Format(':%d', [NpcButton.AddData1]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);

        Result := Result + Format(':%d', [Integer(NpcButton.BlendMode <> 2)]);

        if NpcButton.Hint <> '' then
          Result := Result + Format(':%s', [NpcButton.Hint]);

        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end
      else if NpcButton.m_ACaption = C_LOOKS then begin
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        Result := Result + Format(':%d', [Integer(NpcButton.m_nShowBG)]);
        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end
      else if NpcButton.m_ACaption = C_DNITEMS then begin
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        Result := Result + Format(':%d', [Integer(NpcButton.m_nShowBG)]);
        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end
      else if NpcButton.m_ACaption = C_STATEITEM then begin
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        Result := Result + Format(':%d', [Integer(NpcButton.m_nShowBG)]);
        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end
      else if NpcButton.m_ACaption = C_NEWOPUI then begin
        Result := Result + Format(':%d', [NpcButton.ImageIndex.Up]);
        Result := Result + Format(':%d', [NpcButton.Left + OffsetX]);
        Result := Result + Format(':%d', [NpcButton.Top + OffsetY]);
        if NpcButton.m_sPostText <> '' then
          Result := Result + Format(':%s', [NpcButton.m_sPostText]);
      end;
      FontStr := GetFontStr(NpcButton);
      if FontStr <> '' then
        Result := Result + '{' + FontStr + '}';
      if NpcButton.m_sCmd <> '' then
        Result := Result + '/' + NpcButton.m_sCmd;
      Result := Result + '>';

    end;
  end;

  function Start(AOwner:TDxControl):string;
  var
    I {, J}:Integer;
  begin
    if AOwner.ComponentCount = 0 then
      Exit;

    for I := AOwner.ComponentCount - 1 downto 0 do begin
      Memo2.Lines.Add(DxControlToString(AOwner.Control[I]));
      Start(AOwner.Control[I]);
    end;
  end;

  //var
    //I: Integer;
begin
  Memo2.Lines.Clear;
  Start(FrmDlg.DMerchantDlg);
end;

end.
