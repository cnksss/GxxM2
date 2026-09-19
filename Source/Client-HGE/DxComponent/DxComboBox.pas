unit DxComboBox;

interface
uses
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  HGE,
  DxComponents,
  DxControls,
  DxPopupMenu;
type
  TDxComboBox = class(TDxControl)
  private
    FItems:TStrings;
    FButtonColor:TColor;
    FTextColor:TDxCaptionColor;
    FOnSelect:TNotifyEvent;
    FItemIndex:Integer;
    FShowButton:Boolean;
    procedure ItemChange(Sender:TObject);
    procedure PopupMenuClick(Sender:TObject; X, Y:Integer); stdcall;
    function GetText():string;
    procedure SetText(Value:string);
    procedure SetItemIndex(Value:Integer);
  protected
    procedure SetItems(Value:TStrings);
    procedure DoResize(var NewRect:TRect); override;
  public
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
    property OnSelect:TNotifyEvent read FOnSelect write FOnSelect;
  published
    property ShowButton:Boolean read FShowButton write FShowButton;
    property ItemIndex:Integer read FItemIndex write SetItemIndex;
    property TextColor:TDxCaptionColor read FTextColor write FTextColor;
    property ButtonColor:TColor read FButtonColor write FButtonColor;
    property PopupMenu;
    property BackgroundColor;
    property DrawBorder;

    property Text:string read GetText write SetText;
    property Items:TStrings read FItems write SetItems;
  end;

implementation
uses Math,
  HGECanvas,
  HGEFontEx;
{------------------------------------------------------------------------------}

constructor TDxComboBox.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  FItems := TStringList.Create;
  TStringList(FItems).OnChange := ItemChange;
  PopupMenu := TDxPopupMenu.Create(RootCtrl);
  PopupMenu.Visible := False;
  PopupMenu.Designing := False;
  PopupMenu.OnClick := PopupMenuClick;
  PopupMenu.PopupMenu := Self;

  PopupMenu.Name := 'DxComboBoxPopupMenu';
  Transparent := True;
  DrawBorder := True;
  Width := 200;
  Height := 20;
  FOnSelect := nil;
  FItemIndex := -1;
  FTextColor := TDxCaptionColor.Create;
  FButtonColor := BorderColor.Up.Color;

  FShowButton := True;
end;

destructor TDxComboBox.Destroy;
begin
  FItems.Free;
  FTextColor.Free;
  if PopupMenu <> nil then begin
    PopupMenu.PopupMenu := nil;
    PopupMenu.Free;
  end;
  inherited Destroy;
end;

function TDxComboBox.GetText():string;
begin
  Result := Caption;
end;

procedure TDxComboBox.SetText(Value:string);
begin
  Caption := Value;
end;

procedure TDxComboBox.SetItemIndex(Value:Integer); // 设置选择行
begin
  if FItemIndex <> Value then begin
    FItemIndex := Value;
    (*
    if (FItemIndex >= 0) and (FItemIndex < TDxPopupMenu(PopupMenu).Items.Count) then begin
        if (PopupMenu <> nil) then begin
            Caption := TDxPopupMenu(PopupMenu).Items[FItemIndex]
        end;
    end else begin
        Caption := '';
    end;
    *)
    //HZQ 这样修改逻辑对了，但是他其他的地方未这个PopupMenu未加载，是有问题的
    if (FItemIndex >= 0) and (PopupMenu <> nil) and (FItemIndex < TDxPopupMenu(PopupMenu).Items.Count) then begin
      Caption := TDxPopupMenu(PopupMenu).Items[FItemIndex]
    end else begin
      Caption := '';
    end;
  end;
end;

procedure TDxComboBox.PopupMenuClick(Sender:TObject; X, Y:Integer); // 单击
begin
  if (PopupMenu <> nil) then
    FItemIndex := TDxPopupMenu(PopupMenu).ItemIndex;

  if (PopupMenu <> nil) and (TDxPopupMenu(PopupMenu).ItemIndex >= 0) and (TDxPopupMenu(PopupMenu).ItemIndex < TDxPopupMenu(PopupMenu).Items.Count) then
    Caption := TDxPopupMenu(PopupMenu).Items[TDxPopupMenu(PopupMenu).ItemIndex];

  if Assigned(FOnSelect) then
    FOnSelect(Self);
end;

procedure TDxComboBox.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); // 鼠标按下弹出
var
  vRect:TRect;
begin
  { if (PopupMenu <> nil) then begin
     if PopupMenu.Visible and PointInRect(Point(X, Y), PopupMenu.VisibleRect) then begin
       Exit;
     end else begin
       if PopupMenu.Visible then begin
         PopupMenu.Visible := False;
         Exit;
       end;
     end;
   end;
    }

  if FShowButton and (PopupMenu <> nil) then begin
    if PopupMenu.RootCtrl <> nil then begin
      vRect := VirtualRect;
      if vRect.Bottom + PopupMenu.Height > PopupMenu.RootCtrl.Height then
        PopupMenu.Top := vRect.Top - PopupMenu.Height
      else
        PopupMenu.Top := vRect.Bottom;
    end;
    TDxPopupMenu(PopupMenu).ItemIndex := -1;
    PopupMenu.Left := vRect.Left;
    PopupMenu.Width := Width;
    PopupMenu.Visible := True;
  end;
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxComboBox.DoResize(var NewRect:TRect); // 尺寸改变 调整PopupMenu尺寸
var
  vRect:TRect;
begin
  inherited;
  if NewRect.Right - NewRect.Left < 16 then
    NewRect.Right := NewRect.Left + 16;
  if NewRect.Bottom - NewRect.Top < 16 then
    NewRect.Bottom := NewRect.Top + 16;

  if (PopupMenu <> nil) and PopupMenu.Visible then begin
    if PopupMenu.RootCtrl <> nil then begin
      vRect := VirtualRect;
      if vRect.Bottom + PopupMenu.Height > PopupMenu.RootCtrl.Height then
        PopupMenu.Top := vRect.Top - PopupMenu.Height
      else
        PopupMenu.Top := vRect.Bottom;
    end;
    PopupMenu.Left := vRect.Left;
    PopupMenu.Width := Width;
    PopupMenu.Visible := True;
  end;
end;

procedure TDxComboBox.ItemChange(Sender:TObject);
var
  vRect:TRect;
begin
  if (PopupMenu <> nil) then begin
    TDxPopupMenu(PopupMenu).Items.Assign(FItems);
    if PopupMenu.Visible then begin
      if PopupMenu.RootCtrl <> nil then begin
        vRect := VirtualRect;
        if vRect.Bottom + PopupMenu.Height > PopupMenu.RootCtrl.Height then
          PopupMenu.Top := vRect.Top - PopupMenu.Height
        else
          PopupMenu.Top := vRect.Bottom;
      end;
      PopupMenu.Left := vRect.Left;
      PopupMenu.Width := Width;
      PopupMenu.Visible := False;
    end;
  end;
end;

procedure TDxComboBox.SetItems(Value:TStrings);
begin
  FItems.Assign(Value);
end;

procedure TDxComboBox.Paint;
var
  I, nWidth, nHeight:Integer;
  Font:TDxFont;

  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;

  TextRect:TRect;
  Pt1, Pt2, Pt3:TPoint;

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
    GameCanvas.FillRect(vbRect, BackgroundColor);

  if Enabled then begin
    if MouseDowned then
      Font := BorderColor.Down
    else if MouseMoveed then
      Font := BorderColor.Hot
    else
      Font := BorderColor.Up;
  end
  else
    Font := BorderColor.Disabled;

  if DrawBorder then begin
    PaintRect := vtRect;
    FrameRect(PaintRect, vtRect, vbRect, Font.Color);
    if Font.Bold then begin
      ShrinkRect(PaintRect, 1, 1);
      FrameRect(PaintRect, vtRect, vbRect, Font.Color);
    end;
  end;

  if Enabled then begin
    if MouseDowned then
      Font := FTextColor.Down
    else if MouseMoveed then
      Font := FTextColor.Hot
    else
      Font := FTextColor.Up;
  end
  else
    Font := FTextColor.Disabled;

  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);

  if HGEFont <> nil then begin
    TextImages := HGEFont.GetImageInfos(Text);

    nWidth := 0;
    nHeight := 0;
    for I := 0 to Length(TextImages) - 1 do begin
      if nWidth < TextImages[I].Width then
        nWidth := TextImages[I].Width;

      if TextImages[I].Height <= 0 then
        Inc(nHeight, HGEFont.TextHeight('0'))
      else
        Inc(nHeight, TextImages[I].Height);
    end;

    TextRect := vtRect;
    TextRect.Left := TextRect.Left + 4;
    if FShowButton then begin
      TextRect.Right := TextRect.Right - 14;
    end
    else begin
      TextRect.Right := TextRect.Right - 4;
    end;

    DrawCaption(HGEFont,
      Font, TextImages,
      Bounds(TextRect.Left, vtRect.Top, nWidth, nHeight), ShortRect(vbRect, TextRect), TextRect);

    // DrawCaption(HGEFont,
    // Font,
    // //  Text,
    // Bounds(TextRect.Left, TextRect.Top, Min(HGEFont.TextWidth(Text), TextRect.Right - TextRect.Left), HGEFont.TextHeight('0')), vbRect, vtRect, 0, 0);
  end;
  if FShowButton then begin
    PaintRect := Bounds(vtRect.Left + Width - 14, vtRect.Top + (Height - 6) div 2, 14, Height);
    // 画三角行
    if MouseDowned then begin
      Pt1 := Point(PaintRect.Left + 2, PaintRect.Top + 1);
      Pt2 := Point(PaintRect.Left + 2 + 8, PaintRect.Top + 1);
      Pt3 := Point(PaintRect.Left + 2 + 4, PaintRect.Top + 6 + 1);
    end
    else begin
      Pt1 := Point(PaintRect.Left + 2, PaintRect.Top);
      Pt2 := Point(PaintRect.Left + 2 + 8, PaintRect.Top);
      Pt3 := Point(PaintRect.Left + 2 + 4, PaintRect.Top + 6);
    end;
    GameCanvas.FillTri(Pt1, Pt2, Pt3, FButtonColor, FButtonColor, FButtonColor);
  end;
end;

end.
