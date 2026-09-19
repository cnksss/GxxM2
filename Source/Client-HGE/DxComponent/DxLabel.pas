unit DxLabel;

interface
uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  HGE,
  HGECanvas,
  DxControls,
  DxImageButton,
  DxComponents;
type
  TDxLabel = class(TDxImageButton)
  private
    FRowSpacing:Integer;
    FExpandLineHeight:Integer; // 行高扩展
  protected
    procedure DoCaptionChange(); override;
    procedure SetRowSpacing(Value:Integer);
  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure Assign(Source:TDxControl); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    procedure Paint; override;
  published
    property Align;
    property DrawBorder;
    property BackgroundColor;
    property RowSpacing:Integer read FRowSpacing write SetRowSpacing;
    property ExpandLineHeight:Integer read FExpandLineHeight write FExpandLineHeight;
  end;
implementation
uses Math,
  HGEFontEx;

constructor TDxLabel.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  Transparent := True;
  AutoSize := True;
  Caption := Name;
  Width := 32;
  Height := 16;
  CaptionDownOffsetX := 0;
  CaptionDownOffsetY := 0;
  FRowSpacing := 0;
  FExpandLineHeight := 0;
end;

procedure TDxLabel.SetRowSpacing(Value:Integer);
begin
  if FRowSpacing <> Value then begin
    FRowSpacing := Value;
    DoCaptionChange;
  end;
end;

procedure TDxLabel.Assign(Source:TDxControl);
begin
  if Source is TDxLabel then begin
    Left := Source.Left;
    Top := Source.Top;
    Width := Source.Width;
    Height := Source.Height;
    Enabled := Source.Enabled;
    Visible := Source.Visible;

    Transparent := Source.Transparent; // 是否透明
    EnableFocus := Source.EnableFocus; // 是否允许设置焦点
    Floating := Source.Floating; // 是否可以拖动
    OwnerMove := Source.OwnerMove;

    MouseEvents := Source.MouseEvents;

    //Designing := Source.Designing;                                                                    // 是否在设计期
    ReferenceX := Source.ReferenceX;
    AdjustYByHeight := Source.AdjustYByHeight;
    TopAlignment := Source.TopAlignment;

    Center := Source.Center;
    Align := Source.Align;
    AutoSize := Source.AutoSize;
    BackgroundColor := Source.BackgroundColor;
    BlendMode := TDxLabel(Source).BlendMode;

    ImageIndex.Assign(Source.ImageIndex);
    BorderColor.Assign(Source.BorderColor);

    ButtonDownOffsetX := TDxLabel(Source).ButtonDownOffsetX;
    ButtonDownOffsetY := TDxLabel(Source).ButtonDownOffsetY;

    Caption := TDxLabel(Source).Caption;
    Alignment := TDxLabel(Source).Alignment;

    CaptionColor.Assign(TDxLabel(Source).CaptionColor);
    CaptionDownOffsetX := TDxLabel(Source).CaptionDownOffsetX;
    CaptionDownOffsetY := TDxLabel(Source).CaptionDownOffsetY;

    Checked := TDxLabel(Source).Checked;
    ClickCount := TDxLabel(Source).ClickCount;
    DrawBorder := TDxLabel(Source).DrawBorder;
    Hint := TDxLabel(Source).Hint;
    ModalControl := TDxLabel(Source).ModalControl;
    MouseDownBlendMode := TDxLabel(Source).MouseDownBlendMode;
    MouseMoveBlendMode := TDxLabel(Source).MouseMoveBlendMode;
    Style := TDxLabel(Source).Style;

    RowSpacing := TDxLabel(Source).RowSpacing;
    ExpandLineHeight := TDxLabel(Source).ExpandLineHeight;
  end;
end;

procedure TDxLabel.DoCaptionChange();
var
  I, nWidth, nHeight:Integer;
  HGEFont:THGEFont;
  TextImages:TImageInfos;
  LineHeight:Integer;
begin
  HGEFont := TextureFonts.FindFont(CaptionColor.Up.Name, CaptionColor.Up.Size, CaptionColor.Up.Style);
  if HGEFont <> nil then begin
    TextImages := HGEFont.GetImageInfos(Caption);

    if AutoSize then begin
      nWidth := 0;
      nHeight := 0;
      LineHeight := HGEFont.TextHeight('0') + FRowSpacing;
      for I := 0 to Length(TextImages) - 1 do begin
        if nWidth < TextImages[I].Width then
          nWidth := TextImages[I].Width;

        // if CaptionColor.Up.TextImages[I].Height <= 0 then
        Inc(nHeight, LineHeight);

        // else
        // Inc(nHeight, CaptionColor.Up.TextImages[I].Height);
      end;

      if CaptionColor.Up.Bold then begin
        Inc(nWidth, 2);
        Inc(nHeight, 2);
      end;
      Width := nWidth;
      Height := nHeight;
    end;
  end;
end;

procedure TDxLabel.Paint;
var
  I, nWidth, nHeight:Integer;
  Font:TDxFont;
  vtRect:TRect;
  vbRect:TRect;
  X, Y:Integer;
  HGEFont:THGEFont;
  TextImages:TImageInfos;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  DoPaint();

  if Assigned(OnPaint) then
    OnPaint(Self);

  if not Transparent then
    FillRect(vtRect, vtRect, vbRect, BackgroundColor);

  X := 0;
  Y := 0;
  if Enabled then begin
    if MouseDowned or Checked then begin
      Font := CaptionColor.Down;
      X := CaptionDownOffsetX;
      Y := CaptionDownOffsetY;
    end
    else if MouseMoveed then
      Font := CaptionColor.Hot
    else
      Font := CaptionColor.Up;
  end
  else
    Font := CaptionColor.Disabled;

  if Style <> bsButton then begin
    X := 0;
    Y := 0;
  end;

  if Caption <> '' then begin
    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);

    if HGEFont <> nil then begin
      TextImages := HGEFont.GetImageInfos(Caption);

      nWidth := 0;
      nHeight := 0;
      for I := 0 to Length(TextImages) - 1 do begin
        if nWidth < TextImages[I].Width then
          nWidth := TextImages[I].Width;

        if TextImages[I].Height <= 0 then
          Inc(nHeight, HGEFont.TextHeight('0') + FExpandLineHeight)
        else
          Inc(nHeight, TextImages[I].Height + FExpandLineHeight);
      end;

      if Font.Bold then begin
        Inc(nWidth, 2);
        Inc(nHeight, 2);
      end;

      DrawCaption(HGEFont,
        Font, TextImages,
        Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, X, Y, FExpandLineHeight);
    end;
  end;
  // DrawCaption(Font, Caption, ClientRect, vbRect, vtRect);

  if DrawBorder then begin
    if Enabled then begin
      if MouseDowned or Checked then
        Font := BorderColor.Down
      else if MouseMoveed then
        Font := BorderColor.Hot
      else
        Font := BorderColor.Up;
    end
    else
      Font := BorderColor.Disabled;
    FrameRect(vtRect, vtRect, vbRect, Font.Color);
    if Font.Bold and (MouseDowned or Checked) then begin
      vtRect := ShrinkRect(vtRect, 1, 1);
      FrameRect(vtRect, vtRect, vbRect, Font.Color);
    end;
  end;
end;

function TDxLabel.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
  if PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if Assigned(OnInRealArea) then begin
      vRect := VirtualRect;
      OnInRealArea(Self, X - vRect.Left, Y - vRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

end.
