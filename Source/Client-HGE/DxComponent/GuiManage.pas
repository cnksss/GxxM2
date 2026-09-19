unit GuiManage;

interface
uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls,

  DxBackground,
  DxImageForm,
  DxImageButton,
  DxPageControl,
  DxEdit,
  DxLabel,
  DxMemo,
  DxImageGrid,
  DxPopupMenu,
  DxComboBox,
  DxControls,
  DxComponents;
  //WilImage;
type
  TGuiManage = class(TObject)
  private
    function NewDxControl(GuiHeader: TGuiHeader; AOwner: TDxControl): TDxControl;
    function ReadGuiFontName(FileStream: TStream; AFont: TGuiFont): string;
    procedure DxFontAssign(DxFont: TDxFont; GuiFont: TGuiFont);
    procedure LoadComponent(FileStream: TStream; DxControl: TDxControl);
    procedure LoadSubComponent(FileStream: TStream; AOwner: TDxControl);
  public
    procedure LoadFromStream(FileStream: TStream; Background: TDxBackground);
  end;
//var


implementation
//uses;

procedure TGuiManage.LoadSubComponent(FileStream: TStream; AOwner: TDxControl);
var
  I: Integer;
  DxControl: TDxControl;
  GuiHeader: TGuiHeader;
  Text: string;
begin
  if FileStream.Read(GuiHeader, SizeOf(GuiHeader)) = SizeOf(GuiHeader) then begin
    DxControl := NewDxControl(GuiHeader, AOwner);
    if DxControl <> nil then begin
      if GuiHeader.NameLen > 0 then begin
        SetLength(Text, GuiHeader.NameLen);
        FileStream.Read(Text[1], GuiHeader.NameLen);
        DxControl.Name := Text;
      end;

      LoadComponent(FileStream, DxControl);

      for I := 0 to GuiHeader.Count - 1 do begin
        LoadSubComponent(FileStream, DxControl);
      end;
    end;
  end;
end;

procedure TGuiManage.DxFontAssign(DxFont: TDxFont; GuiFont: TGuiFont);
begin
  DxFont.Color := GuiFont.Color;
  DxFont.BColor := GuiFont.BColor;
  DxFont.Size := GuiFont.Size;
  DxFont.Bold := GuiFont.Bold;
  DxFont.Style := GuiFont.Style;
end;

function TGuiManage.ReadGuiFontName(FileStream: TStream; AFont: TGuiFont): string;
var
  Text: string;
begin
  Result := '';
  if AFont.NameLen > 0 then begin
    SetLength(Text, AFont.NameLen);
    FileStream.Read(Text[1], AFont.NameLen);
    Result := Text;
  end;
end;

procedure TGuiManage.LoadComponent(FileStream: TStream; DxControl: TDxControl);
var
  Gui: TGuiType;
  Text: string;

  GuiImageForm: TGuiImageForm;
  GuiImageButton: TGuiImageButton;
  GuiEdit: TGuiEdit;
  GuiLabel: TGuiLabel;
  GuiMemo: TGuiMemo;
  GuiImageGrid: TGuiImageGrid;
  GuiPopupMenu: TGuiPopupMenu;
  GuiComboBox: TGuiComboBox;
  GuiPageControl: TGuiPageControl;
  GuiTabSheet: TGuiTabSheet;

  DxImageForm: TDxImageForm;
  DxImageButton: TDxImageButton;
  DxPageControl: TDxPageControl;
  DxTabSheet: TDxTabSheet;

  DxEdit: TDxEdit;
  DxLabel: TDxLabel;
  DxMemo: TDxMemo;
  DxImageGrid: TDxImageGrid;
  DxPopupMenu: TDxPopupMenu;
  DxComboBox: TDxComboBox;
begin
  Gui := TGuiType(DxControl.Tag);
  case Gui of
    t_Form: begin
        FileStream.Read(GuiImageForm, SizeOf(TGuiImageForm));
        DxImageForm := TDxImageForm(DxControl);
        DxImageForm.ImageIndex.Up := GuiImageForm.ImageIndex;
      end;
    t_Button: begin
        FileStream.Read(GuiImageButton, SizeOf(TGuiImageButton));
        DxImageButton := TDxImageButton(DxControl);
        with DxImageButton do begin
          AutoSize := GuiImageButton.AutoSize;
          Alignment := GuiImageButton.Alignment;
          ImageIndex.Up := GuiImageButton.ImageIndex.Up;
          ImageIndex.Hot := GuiImageButton.ImageIndex.Hot;
          ImageIndex.Down := GuiImageButton.ImageIndex.Down;
          ImageIndex.Disabled := GuiImageButton.ImageIndex.Disabled;

          DxFontAssign(CaptionColor.Up, GuiImageButton.CaptionColor.Up);
          DxFontAssign(CaptionColor.Hot, GuiImageButton.CaptionColor.Hot);
          DxFontAssign(CaptionColor.Down, GuiImageButton.CaptionColor.Down);
          DxFontAssign(CaptionColor.Disabled, GuiImageButton.CaptionColor.Disabled);

          Checked := GuiImageButton.Checked;
          ClickCount := GuiImageButton.ClickCount;
          Style := GuiImageButton.Style;
        end;
        DxImageButton.CaptionColor.Up.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Up);
        DxImageButton.CaptionColor.Hot.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Hot);
        DxImageButton.CaptionColor.Down.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Down);
        DxImageButton.CaptionColor.Disabled.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Disabled);

        if GuiImageButton.CaptionLen > 0 then begin
          SetLength(Text, GuiImageButton.CaptionLen);
          FileStream.Read(Text[1], GuiImageButton.CaptionLen);
          DxImageButton.Caption := Text;
        end;
      end;
    t_Edit: begin
        FileStream.Read(GuiEdit, SizeOf(TGuiEdit));
        DxEdit := TDxEdit(DxControl);

        with DxEdit do begin
          BackgroundColor := GuiEdit.BackgroundColor;
          DrawBorder := GuiEdit.DrawBorder;

          DxFontAssign(Font, GuiEdit.FontColor);
          DxFontAssign(BorderColor.Up, GuiEdit.BorderColor.Up);
          DxFontAssign(BorderColor.Hot, GuiEdit.BorderColor.Hot);
          DxFontAssign(BorderColor.Down, GuiEdit.BorderColor.Down);
          DxFontAssign(BorderColor.Disabled, GuiEdit.BorderColor.Disabled);

          ReadOnly := GuiEdit.ReadOnly;
          MaxLength := GuiEdit.MaxLength;
          SelectedColor := GuiEdit.SelectedColor;
          SelBackColor := GuiEdit.SelBackColor;
          SelFontColor := GuiEdit.SelFontColor;
          InValue := GuiEdit.InValue;
          PasswordChar := GuiEdit.PasswordChar;
          AllowSelectText := GuiEdit.AllowSelectText;
          AllowPaste := GuiEdit.AllowPaste;
        end;
        DxEdit.Font.Name := ReadGuiFontName(FileStream, GuiEdit.FontColor);

        if GuiEdit.TextLen > 0 then begin
          SetLength(Text, GuiEdit.TextLen);
          FileStream.Read(Text[1], GuiEdit.TextLen);
          DxEdit.Text := Text;
        end;
      end;
    t_Label: begin
        FileStream.Read(GuiLabel, SizeOf(TGuiLabel));
        DxLabel := TDxLabel(DxControl);
        with DxLabel do begin
          AutoSize := GuiLabel.AutoSize;
          BackgroundColor := GuiLabel.BackgroundColor;
          DrawBorder := GuiLabel.DrawBorder;

          DxFontAssign(CaptionColor.Up, GuiLabel.CaptionColor.Up);
          DxFontAssign(CaptionColor.Hot, GuiLabel.CaptionColor.Hot);
          DxFontAssign(CaptionColor.Down, GuiLabel.CaptionColor.Down);
          DxFontAssign(CaptionColor.Disabled, GuiLabel.CaptionColor.Disabled);

          DxFontAssign(BorderColor.Up, GuiLabel.BorderColor.Up);
          DxFontAssign(BorderColor.Hot, GuiLabel.BorderColor.Hot);
          DxFontAssign(BorderColor.Down, GuiLabel.BorderColor.Down);
          DxFontAssign(BorderColor.Disabled, GuiLabel.BorderColor.Disabled);

          ClickCount := GuiLabel.ClickCount;
          Style := GuiLabel.Style;
        end;

        DxLabel.CaptionColor.Up.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Up);
        DxLabel.CaptionColor.Hot.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Hot);
        DxLabel.CaptionColor.Down.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Down);
        DxLabel.CaptionColor.Disabled.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Disabled);

        if GuiLabel.CaptionLen > 0 then begin
          SetLength(Text, GuiLabel.CaptionLen);
          FileStream.Read(Text[1], GuiLabel.CaptionLen);
          DxLabel.Caption := Text;
        end;
      end;

    t_Grid: begin
        FileStream.Read(GuiImageGrid, SizeOf(TGuiImageGrid));
        DxImageGrid := TDxImageGrid(DxControl);
        with DxImageGrid do begin
          ColCount := GuiImageGrid.ColCount;
          RowCount := GuiImageGrid.RowCount;
          ColWidth := GuiImageGrid.ColWidth;
          RowHeight := GuiImageGrid.RowHeight;
          ViewTopLine := GuiImageGrid.ViewTopLine;
        end;
      end;
    t_Memo, t_ChatMemo, t_ListView, t_TreeView: begin
        FileStream.Read(GuiMemo, SizeOf(TGuiMemo));
        DxMemo := TDxMemo(DxControl);
        with DxMemo do begin
          ShowScroll := GuiMemo.ShowScroll;
          ItemHeight := GuiMemo.ItemHeight;
          ItemIndex := GuiMemo.ItemIndex;
          ScrollBars := GuiMemo.ScrollBars;
          ScrollSize := GuiMemo.ScrollSize;

          ImageIndex.Up := GuiMemo.ImageIndex.Up;
          ImageIndex.Hot := GuiMemo.ImageIndex.Hot;
          ImageIndex.Down := GuiMemo.ImageIndex.Down;
          ImageIndex.Disabled := GuiMemo.ImageIndex.Disabled;

          PrevImageIndex.Up := GuiMemo.PrevImageIndex.Up;
          PrevImageIndex.Hot := GuiMemo.PrevImageIndex.Hot;
          PrevImageIndex.Down := GuiMemo.PrevImageIndex.Down;
          PrevImageIndex.Disabled := GuiMemo.PrevImageIndex.Disabled;

          NextImageIndex.Up := GuiMemo.NextImageIndex.Up;
          NextImageIndex.Hot := GuiMemo.NextImageIndex.Hot;
          NextImageIndex.Down := GuiMemo.NextImageIndex.Down;
          NextImageIndex.Disabled := GuiMemo.NextImageIndex.Disabled;

          BarImageIndex.Up := GuiMemo.BarImageIndex.Up;
          BarImageIndex.Hot := GuiMemo.BarImageIndex.Hot;
          BarImageIndex.Down := GuiMemo.BarImageIndex.Down;
          BarImageIndex.Disabled := GuiMemo.BarImageIndex.Disabled;

          MaxValue := GuiMemo.MaxValue;
          Position := GuiMemo.Position;
          RemoveSize := GuiMemo.RemoveSize;
        end;
      end;

    t_PopupMenu: begin
        FileStream.Read(GuiPopupMenu, SizeOf(TGuiPopupMenu));
        DxPopupMenu := TDxPopupMenu(DxControl);
        with DxPopupMenu do begin
          BackgroundColor := GuiPopupMenu.BackgroundColor;
          DrawBorder := GuiPopupMenu.DrawBorder;

          DxFontAssign(ItemColor.Up, GuiPopupMenu.ItemColor.Up);
          DxFontAssign(ItemColor.Hot, GuiPopupMenu.ItemColor.Hot);
          DxFontAssign(ItemColor.Down, GuiPopupMenu.ItemColor.Down);
          DxFontAssign(ItemColor.Disabled, GuiPopupMenu.ItemColor.Disabled);

          DxFontAssign(BorderColor.Up, GuiPopupMenu.BorderColor.Up);
          DxFontAssign(BorderColor.Hot, GuiPopupMenu.BorderColor.Hot);
          DxFontAssign(BorderColor.Down, GuiPopupMenu.BorderColor.Down);
          DxFontAssign(BorderColor.Disabled, GuiPopupMenu.BorderColor.Disabled);

          SelectColor := GuiPopupMenu.SelectColor;
          ItemHeight := GuiPopupMenu.ItemHeight;
          ItemIndex := GuiPopupMenu.ItemIndex;
        end;

        DxPopupMenu.ItemColor.Up.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Up);
        DxPopupMenu.ItemColor.Hot.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Hot);
        DxPopupMenu.ItemColor.Down.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Down);
        DxPopupMenu.ItemColor.Disabled.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Disabled);

        if GuiPopupMenu.ItemTextLen > 0 then begin
          SetLength(Text, GuiPopupMenu.ItemTextLen);
          FileStream.Read(Text[1], GuiPopupMenu.ItemTextLen);
          DxPopupMenu.Items.Text := Text;
        end;
      end;
    t_PageControl: begin
        FileStream.Read(GuiPageControl, SizeOf(TGuiPageControl));
        DxPageControl := TDxPageControl(DxControl);
        with DxPageControl do begin
          ShowButton := GuiPageControl.ShowButton;
          ClientLeft := GuiPageControl.ClientLeft;
          ClientTop := GuiPageControl.ClientTop;
          ClientWidth := GuiPageControl.ClientWidth;
          ClientHeight := GuiPageControl.ClientHeight;
          TabPosition := GuiPageControl.TabPosition;
          PageCount := 0;
          //ActivePageIndex := 0;
          ButtonWidth := GuiPageControl.ButtonWidth;
          ButtonHeight := GuiPageControl.ButtonHeight;
        end;
      end;
    t_ComboBox: begin
        FileStream.Read(GuiComboBox, SizeOf(TGuiComboBox));
        DxComboBox := TDxComboBox(DxControl);

        DxPopupMenu := TDxPopupMenu(DxComboBox.PopupMenu);

        with DxPopupMenu do begin
          BackgroundColor := GuiComboBox.GuiPopupMenu.BackgroundColor;
          DrawBorder := GuiComboBox.GuiPopupMenu.DrawBorder;

          DxFontAssign(ItemColor.Up, GuiComboBox.GuiPopupMenu.ItemColor.Up);
          DxFontAssign(ItemColor.Hot, GuiComboBox.GuiPopupMenu.ItemColor.Hot);
          DxFontAssign(ItemColor.Down, GuiComboBox.GuiPopupMenu.ItemColor.Down);
          DxFontAssign(ItemColor.Disabled, GuiComboBox.GuiPopupMenu.ItemColor.Disabled);

          DxFontAssign(BorderColor.Up, GuiComboBox.GuiPopupMenu.BorderColor.Up);
          DxFontAssign(BorderColor.Hot, GuiComboBox.GuiPopupMenu.BorderColor.Hot);
          DxFontAssign(BorderColor.Down, GuiComboBox.GuiPopupMenu.BorderColor.Down);
          DxFontAssign(BorderColor.Disabled, GuiComboBox.GuiPopupMenu.BorderColor.Disabled);

          SelectColor := GuiComboBox.GuiPopupMenu.SelectColor;
          ItemHeight := GuiComboBox.GuiPopupMenu.ItemHeight;
          ItemIndex := GuiComboBox.GuiPopupMenu.ItemIndex;
        end;

        DxPopupMenu.ItemColor.Up.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Up);
        DxPopupMenu.ItemColor.Hot.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Hot);
        DxPopupMenu.ItemColor.Down.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Down);
        DxPopupMenu.ItemColor.Disabled.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Disabled);

        with DxComboBox do begin
          BackgroundColor := GuiComboBox.BackgroundColor;
          DrawBorder := GuiComboBox.DrawBorder;
          ButtonColor := GuiComboBox.ButtonColor;

          DxFontAssign(TextColor.Up, GuiComboBox.TextColor.Up);
          DxFontAssign(TextColor.Hot, GuiComboBox.TextColor.Hot);
          DxFontAssign(TextColor.Down, GuiComboBox.TextColor.Down);
          DxFontAssign(TextColor.Disabled, GuiComboBox.TextColor.Disabled);

          DxFontAssign(BorderColor.Up, GuiComboBox.BorderColor.Up);
          DxFontAssign(BorderColor.Hot, GuiComboBox.BorderColor.Hot);
          DxFontAssign(BorderColor.Down, GuiComboBox.BorderColor.Down);
          DxFontAssign(BorderColor.Disabled, GuiComboBox.BorderColor.Disabled);
        end;

        DxComboBox.TextColor.Up.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Up);
        DxComboBox.TextColor.Hot.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Hot);
        DxComboBox.TextColor.Down.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Down);
        DxComboBox.TextColor.Disabled.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Disabled);

        if GuiComboBox.TextLen > 0 then begin
          SetLength(Text, GuiComboBox.TextLen);
          FileStream.Read(Text[1], GuiComboBox.TextLen);
          DxComboBox.Text := Text;
        end;
        if GuiComboBox.ItemLen > 0 then begin
          SetLength(Text, GuiComboBox.ItemLen);
          FileStream.Read(Text[1], GuiComboBox.ItemLen);
          DxComboBox.Items.Text := Text;
        end;
      end;
    t_TabSheet: begin
        FileStream.Read(GuiTabSheet, SizeOf(TGuiTabSheet));
        DxTabSheet := TDxTabSheet(DxControl);
        with DxTabSheet do begin
          ImageIndex.Up := GuiTabSheet.ImageIndex.Up;
          ImageIndex.Hot := GuiTabSheet.ImageIndex.Hot;
          ImageIndex.Down := GuiTabSheet.ImageIndex.Down;
          ImageIndex.Disabled := GuiTabSheet.ImageIndex.Disabled;

          DxFontAssign(CaptionColor.Up, GuiTabSheet.CaptionColor.Up);
          DxFontAssign(CaptionColor.Hot, GuiTabSheet.CaptionColor.Hot);
          DxFontAssign(CaptionColor.Down, GuiTabSheet.CaptionColor.Down);
          DxFontAssign(CaptionColor.Disabled, GuiTabSheet.CaptionColor.Disabled);
        end;

        DxTabSheet.CaptionColor.Up.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Up);
        DxTabSheet.CaptionColor.Hot.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Hot);
        DxTabSheet.CaptionColor.Down.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Down);
        DxTabSheet.CaptionColor.Disabled.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Disabled);

        if GuiTabSheet.CaptionLen > 0 then begin
          SetLength(Text, GuiTabSheet.CaptionLen);
          FileStream.Read(Text[1], GuiTabSheet.CaptionLen);
          DxTabSheet.Caption := Text;
        end;
        TDxPageControl(DxTabSheet.Owner).ActivePageIndex := 0;
      end;
    //t_CharMemo: ;
  end;
end;

function TGuiManage.NewDxControl(GuiHeader: TGuiHeader; AOwner: TDxControl): TDxControl;
var
  DxControl: TDxControl;
begin
  DxControl := nil;
  case GuiHeader.Gui of
    t_Form: DxControl := TDxImageForm.Create(AOwner);
    t_Button: DxControl := TDxImageButton.Create(AOwner);
    t_Edit: DxControl := TDxEdit.Create(AOwner);
    t_Label: DxControl := TDxLabel.Create(AOwner);
    t_Grid: DxControl := TDxImageGrid.Create(AOwner);
    t_Memo: DxControl := TDxMemo.Create(AOwner);
    t_ChatMemo: DxControl := TDxChatMemo.Create(AOwner);
    t_ListView: DxControl := TDxListView.Create(AOwner);
    t_TreeView: DxControl := TDxTreeView.Create(AOwner);
    t_PopupMenu: DxControl := TDxPopupMenu.Create(AOwner);
    t_TabSheet: DxControl := TDxTabSheet.Create(AOwner);
    t_PageControl: DxControl := TDxPageControl.Create(AOwner);
    t_ComboBox: DxControl := TDxComboBox.Create(AOwner);
  end;
  if DxControl <> nil then begin
    with DxControl do begin
      Left := GuiHeader.Left;
      Top := GuiHeader.Top;
      Width := GuiHeader.Width;
      Height := GuiHeader.Height;
      Enabled := GuiHeader.Enabled;
      Visible := GuiHeader.Visible;
      Transparent := GuiHeader.Transparent;
      EnableFocus := GuiHeader.EnableFocus;
      Floating := GuiHeader.Floating;
      OnGetImage := AOwner.OnGetImage;
      Tag := Integer(GuiHeader.Gui);
      //ImageType := GuiHeader.Image;
    end;
  end;
  Result := DxControl;
end;

procedure TGuiManage.LoadFromStream(FileStream: TStream; Background: TDxBackground);
var
  I: Integer;
  DxControl: TDxControl;
  GuiHeader: TGuiHeader;
  Len: Integer;
  Text: string;
begin
  while True do begin
    if FileStream.Position >= FileStream.Size then break;
    if FileStream.Read(GuiHeader, SizeOf(GuiHeader)) = SizeOf(GuiHeader) then begin
      DxControl := NewDxControl(GuiHeader, Background);

      if DxControl <> nil then begin
        if GuiHeader.NameLen > 0 then begin
          SetLength(Text, GuiHeader.NameLen);
          FileStream.Read(Text[1], GuiHeader.NameLen);
          DxControl.Name := Text;
        end;
        LoadComponent(FileStream, DxControl);

        //if DxControl.Name = 'DLogin' then DLogin := TDxImageForm(DxControl);

        for I := 0 to GuiHeader.Count - 1 do begin
          LoadSubComponent(FileStream, DxControl);
        end;
      end;
    end else break;
  end; //while True do begin
end;

end.
