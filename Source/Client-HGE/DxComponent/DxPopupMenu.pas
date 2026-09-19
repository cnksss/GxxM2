unit DxPopupMenu;

interface
uses
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  DxComponents,
  HGE,
  DxControls,
  DxLabel;
type
  TDxItemMenuList = class(TStringList)
  private
    function GetItemEnabled(Index:Integer):Boolean;
    function GetItemVisible(Index:Integer):Boolean;
    function GetItemChecked(Index:Integer):Boolean;
    procedure SetItemEnabled(Index:Integer; Value:Boolean);
    procedure SetItemVisible(Index:Integer; Value:Boolean);
    procedure SetItemChecked(Index:Integer; Value:Boolean);
  protected
    function GetObject(Index:Integer):TObject; override;
  public
    destructor Destroy; override;
    function AddObject(const S:string; AObject:TObject):Integer; override;
    procedure Clear; override;
    procedure Delete(Index:Integer); override;
    property Enabled[Index:Integer]:Boolean read GetItemEnabled write SetItemEnabled;
    property Visible[Index:Integer]:Boolean read GetItemVisible write SetItemVisible;
    property Checked[Index:Integer]:Boolean read GetItemChecked write SetItemChecked;
  end;

  TDxItemMenu = record
    Enabled:Boolean;
    Visible:Boolean;
    Checked:Boolean;
    AObject:TObject;
    //SubMenuItems: TDxItemMenuList;
  end;
  pTDxItemMenu = ^TDxItemMenu;

  TDxPopupMenu = class(TDxControl)
  private
    FSelectColor:TColor;
    FItemHeight:Integer;
    FItemIndex:Integer;
    FItems:TDxItemMenuList;
    FItemColor:TDxCaptionColor;
    FAlpha:Byte;
    procedure SetItemHeight(Value:Integer);
    procedure SetItemIndex(Value:Integer);
    procedure ItemChange(Sender:TObject);
    procedure DrawItemCaption(vtRect, vbRect:TRect);
    function ItemCount:Integer;
  protected
    procedure SetItems(Value:TDxItemMenuList);
    procedure DoShow(); override;
    procedure DoHide(); override;
    procedure DoResize(var NewRect:TRect); override;
    procedure DoClick(X, Y:Integer); override;
  public
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
  published
    property BackgroundColor;
    property DrawBorder;
    property ItemColor:TDxCaptionColor read FItemColor write FItemColor;
    property Items:TDxItemMenuList read FItems write SetItems;
    property SelectColor:TColor read FSelectColor write FSelectColor;
    property ItemHeight:Integer read FItemHeight write SetItemHeight;
    property ItemIndex:Integer read FItemIndex write SetItemIndex;
    property Alpha:Byte read FAlpha write FAlpha;
  end;

implementation
uses Math,
  HGECanvas,
  HGEFontEx;
{------------------------------------------------------------------------------}

destructor TDxItemMenuList.Destroy;
var
  I:Integer;
  MenuItem:pTDxItemMenu;
begin
  for I := 0 to Count - 1 do begin
    MenuItem := pTDxItemMenu(Objects[I]);

    {
    if MenuItem.SubMenuItems <> nil then
    begin
      FreeAndNil(MenuItem.SubMenuItems);
    end;
    }
    Dispose(MenuItem);
  end;
  inherited Destroy;
end;

function TDxItemMenuList.GetObject(Index:Integer):TObject;
begin
  Result := inherited GetObject(Index);
  {if Result <> nil then begin
    Result := pTDxItemMenu(Result).AObject;
  end; }
end;

function TDxItemMenuList.GetItemEnabled(Index:Integer):Boolean;
begin
  Result := pTDxItemMenu(Objects[Index]).Enabled;
end;

function TDxItemMenuList.GetItemVisible(Index:Integer):Boolean;
begin
  Result := pTDxItemMenu(Objects[Index]).Visible;
end;

function TDxItemMenuList.GetItemChecked(Index:Integer):Boolean;
begin
  Result := pTDxItemMenu(Objects[Index]).Checked;
end;

procedure TDxItemMenuList.SetItemEnabled(Index:Integer; Value:Boolean);
begin
  pTDxItemMenu(Objects[Index]).Enabled := Value;
end;

procedure TDxItemMenuList.SetItemVisible(Index:Integer; Value:Boolean);
begin
  pTDxItemMenu(Objects[Index]).Visible := Value;
end;

procedure TDxItemMenuList.SetItemChecked(Index:Integer; Value:Boolean);
begin
  pTDxItemMenu(Objects[Index]).Checked := Value;
end;

function TDxItemMenuList.AddObject(const S:string; AObject:TObject):Integer;
var
  DxItemMenu:pTDxItemMenu;
begin
  New(DxItemMenu);
  DxItemMenu.Enabled := S <> '-';
  DxItemMenu.Visible := True;
  DxItemMenu.Checked := False;
  DxItemMenu.AObject := AObject;
  //DxItemMenu.SubMenuItems := nil;
  Result := inherited AddObject(S, TObject(DxItemMenu));
end;

procedure TDxItemMenuList.Clear;
var
  I:Integer;
  MenuItem:pTDxItemMenu;
begin
  for I := 0 to Count - 1 do begin
    MenuItem := pTDxItemMenu(Objects[I]);
    {
    if MenuItem.SubMenuItems <> nil then
    begin
      FreeAndNil(MenuItem.SubMenuItems);
    end;
    }
    Dispose(MenuItem);
  end;
  inherited Clear;
end;

procedure TDxItemMenuList.Delete(Index:Integer);
var
  MenuItem:pTDxItemMenu;
begin
  MenuItem := pTDxItemMenu(Objects[Index]);
  {
  if MenuItem.SubMenuItems <> nil then
  begin
    FreeAndNil(MenuItem.SubMenuItems);
  end;
  }
  Dispose(MenuItem);
  inherited Delete(Index);
end;

{------------------------------------------------------------------------------}

constructor TDxPopupMenu.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  FItems := TDxItemMenuList.Create;
  TStringList(FItems).OnChange := ItemChange;
  Width := 150;
  Height := 200;
  Transparent := False;
  BackgroundColor := clWhite;

  DrawBorder := True;

  BorderColor.Up.Color := clGray;
  BorderColor.Hot.Color := clGray;
  BorderColor.Down.Color := clGray;
  BorderColor.Disabled.Color := clGray;

  FSelectColor := clBlue;
  FAlpha := 255;
  FItemHeight := 20;
  FItemIndex := -1;
  FItemColor := TDxCaptionColor.Create;
  FItemColor.Up.Color := clBlack;
  FItemColor.Hot.Color := clWhite;
  FItemColor.Down.Color := clWhite;
  FItemColor.Disabled.Color := clGray;
end;

destructor TDxPopupMenu.Destroy;
begin
  try
    if Assigned(PopupMenu) then
      PopupMenu.PopupMenu := nil;
  except
    PopupMenu := nil;
  end;
  PopupMenu := nil;
  FItems.Free;
  FItemColor.Free;

  inherited Destroy;
end;

procedure TDxPopupMenu.DoShow();
var
  NewRect:TRect;
begin
  inherited;

  NewRect := ClientRect;
  DoResize(NewRect);
  ClientRect := NewRect;

  BringToFront;
  SetFocus;
  RootCtrl.ActiveMenu := Self;
end;

procedure TDxPopupMenu.DoHide();
begin
  inherited;
  if (Owner <> nil) and (Owner is TDxPopupMenu) then
    TDxControl(Owner).Visible := False;

  if RootCtrl.ActiveMenu = Self then begin
    RootCtrl.ActiveMenu := nil;

    // RootCtrl.SetFocus;
  end;
end;

procedure TDxPopupMenu.DoClick(X, Y:Integer);
begin
  if (FItemIndex >= 0) and (FItemIndex < ItemCount) then begin
    if FItems.Enabled[FItemIndex] and FItems.Visible[FItemIndex] then
      if not Designing then begin
        if PopupMenu <> nil then
          PopupMenu.SetFocus;
        Close;
      end;
  end;
  if Assigned(OnClick) then
    OnClick(Self, X, Y);
  // inherited;
end;

function TDxPopupMenu.ItemCount:Integer;
var
  I:Integer;
begin
  Result := 0;
  for I := 0 to FItems.Count - 1 do
    if FItems.Visible[I] then Inc(Result);
end;

procedure TDxPopupMenu.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  vRect:TRect;
begin
  inherited MouseMove(Shift, X, Y);
  vRect := VirtualRect;
  FItemIndex := (Y - vRect.Top) div FItemHeight;
  if FItemIndex >= ItemCount then
    FItemIndex := -1;

  {
  if (FItemIndex >= 0) and (FItemIndex <= FItems.Count - 1) then
  begin
    MenuItem := pTDxItemMenu(FItems.Objects[FItemIndex]);
    if (Pos('¸´ÖÆ', FItems.Strings[FItemIndex]) > 0) then
    begin
      if (MenuItem.SubMenuItems = nil) then
      begin
        MenuItem.SubMenuItems := TDxItemMenuList.Create;
        MenuItem.SubMenuItems.Add('aaa');
        MenuItem.SubMenuItems.Add('bbb');
      end;
    end;
  end;
  }
end;

procedure TDxPopupMenu.SetItemHeight(Value:Integer);
begin
  FItemHeight := Max(Value, g_CurrentFontHeight);
end;

procedure TDxPopupMenu.SetItemIndex(Value:Integer);
begin
  FItemIndex := Value;
  if FItemIndex >= ItemCount then FItemIndex := -1;
end;

procedure TDxPopupMenu.DoResize(var NewRect:TRect);
begin
  inherited;
  if Designing and (ItemCount = 0) then Exit;
  NewRect.Bottom := NewRect.Top + ItemCount * FItemHeight + 6;
end;

procedure TDxPopupMenu.ItemChange(Sender:TObject);
begin
  if Designing and (ItemCount = 0) then Exit;
  Height := ItemCount * FItemHeight + 6;
end;

procedure TDxPopupMenu.SetItems(Value:TDxItemMenuList);
begin
  FItems.Assign(Value);
end;

procedure TDxPopupMenu.DrawItemCaption(vtRect, vbRect:TRect);
var
  Index, nIndex, nX, nY:Integer;
  PaintRect:TRect;
  DestRect:TRect;
  Font:TDxFont;
  HGEFont:THGEFont;
  ImageInfo:TImageInfo;
begin
  if ItemCount = 0 then Exit;
  if FAlpha = 255 then
    FillRect(vtRect, vtRect, vbRect, BackgroundColor)
  else
    FillRectAlpha(vtRect, vtRect, vbRect, BackgroundColor, FAlpha);

  nIndex := 0;

  for Index := 0 to FItems.Count - 1 do begin
    if FItemIndex = nIndex then begin
      if FItems.Enabled[Index] then
        Font := ItemColor.Hot
      else
        Font := ItemColor.Disabled;
    end
    else begin
      if FItems.Enabled[Index] then
        Font := ItemColor.Up
      else
        Font := ItemColor.Disabled;
    end;

    if FItems.Visible[Index] then begin
      PaintRect := vtRect;
      PaintRect.Left := PaintRect.Left + 3;
      PaintRect.Top := PaintRect.Top + 3 + nIndex * FItemHeight;
      PaintRect.Right := PaintRect.Left + Width - 3 * 2;
      PaintRect.Bottom := PaintRect.Top + FItemHeight;
      if FItems[Index] = '-' then begin
        PaintRect.Top := PaintRect.Top + (PaintRect.Bottom - PaintRect.Top) div 2;
        PaintRect.Bottom := PaintRect.Top + 1;
        PaintRect := ShortRect(PaintRect, vbRect);
        if FAlpha = 255 then
          GameCanvas.FillRect(PaintRect, BorderColor.Up.Color)
        else
          GameCanvas.FillRectAlpha(PaintRect, BorderColor.Up.Color, FAlpha);
      end
      else begin
        HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
        if HGEFont <> nil then begin
          ImageInfo := HGEFont.GetImageInfo(FItems[Index]);
          if FItemIndex = nIndex then begin
            if FAlpha = 255 then
              FillRect(PaintRect, vtRect, vbRect, FSelectColor)
            else
              FillRectAlpha(PaintRect, vtRect, vbRect, FSelectColor, FAlpha);
          end;

          PaintRect.Top := PaintRect.Top + (FItemHeight - HGEFont.TextHeight('Pp')) div 2;
          PaintRect.Bottom := PaintRect.Top + HGEFont.TextHeight('Pp');
          DestRect := PaintRect;
          PaintRect := Rect(0, 0, ImageInfo.Width, ImageInfo.Height);
          PaintRect := ReallyPaintRect(DestRect, PaintRect, vtRect, vbRect, nX, nY);
          if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
            HGEFont.TextRect(nX, nY, PaintRect, ImageInfo.ImageIndexs, Font.Color);
          end;
        end;

        {
        MenuItem := pTDxItemMenu(FItems.Objects[index]);
        if MenuItem.SubMenuItems <> nil then
        begin
          HGEFont.TextOut(vtRect.Right - 16, nY, '>', Font.Color);
        end;
        }
      end;

      Inc(nIndex);
    end;
  end;

  if DrawBorder then begin
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
    PaintRect := vtRect;
    FrameRect(PaintRect, vtRect, vbRect, Font.Color);

    if Font.Bold then begin
      ShrinkRect(PaintRect, 1, 1);
      FrameRect(PaintRect, vtRect, vbRect, Font.Color);
    end;
  end;
end;

procedure TDxPopupMenu.Paint;
var
  I:Integer;
  vtRect:TRect;
  vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if Designing then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  DoPaint();

  DrawItemCaption(vtRect, vbRect);

  for I := ControlCount - 1 downto 0 do
    if (Control[I].Visible) then
      Control[I].Paint;
end;

end.
