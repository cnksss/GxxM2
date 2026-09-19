unit LoadDxControl;

interface
{.$DEFINE OUTPUT_GUI_READORDER}

uses
  Windows,
  Messages,
  SysUtils,
  StrUtils,
  Classes,
  Graphics,
  Controls,
  StdCtrls,
  Grids,
  DxImageForm,
  DxImageButton,
  DxPageControl,
  DxEdit,
  DxLabel,
  DxMemo,
  DxImageGrid,
  DxPopupMenu,
  DxComboBox,
  DxLine,
  DxControls,
  DxComponents,
  DxImageEdit,
  DxTrackBar,
  DxMainBottomForm,
  DxMagicBall,
  DxSexPanel,
  DxGroupAttackProgress,
  DxImageProgress,
  DxSwitchButton,
  UnitDes;

procedure DxFontAssign(DxFont:TDxFont; GuiFont:TGuiFont);
procedure GuiFontAssign(var GuiFont:TGuiFont; DxFont:TDxFont);

// function LoadControlFromStream(Stream: TStream; Background: TDxCustomControl; AComponentList: TList): Integer; overload;
function LoadControlFromStream(MemoryStream:TMemoryStream; Background:TDxControl; AControlAddress:Pointer; sUiName:string):Integer; // overload;
function LoadControlFromMemory(Address, Memory:Pointer; Size:Integer; Background:TDxControl; sUiName:string):Integer;

implementation

{$IFDEF  OUTPUT_GUI_READORDER}
uses
  LogHelper;
{$ENDIF}

var
  PControlAddress:Pointer = nil;

  MemoryData:Pointer = nil;
  MemorySize:Integer = 0;
  MemoryPosition:Integer = 0;
  // ComponentIndex: Integer = 0;
  // ComponentList: TList = nil;

function ReadMemory(var Buffer; Count:Longint):Longint;
var
  nRem:Integer;
begin
  if (MemoryPosition >= 0) and (Count >= 0) then begin
    nRem := MemorySize - MemoryPosition;
    if nRem > 0 then begin
      if nRem >= Count then begin
        Result := Count;
        Move(Pointer(Longint(MemoryData) + MemoryPosition)^, Buffer, Count);
        Inc(MemoryPosition, Count);
      end else begin
        Result := nRem;
        Move(Pointer(Longint(MemoryData) + MemoryPosition)^, Buffer, nRem);
        Inc(MemoryPosition, nRem);
      end;
    end else begin
      Result := 0;
    end;
  end else begin
    Result := 0;
  end;
end;

procedure DxFontAssign(DxFont:TDxFont; GuiFont:TGuiFont);
begin
  DxFont.Color := GuiFont.Color;
  DxFont.BColor := GuiFont.BColor;
  DxFont.Size := GuiFont.Size;
  DxFont.Bold := GuiFont.Bold;
  DxFont.Style := GuiFont.Style;
end;

procedure GuiFontAssign(var GuiFont:TGuiFont; DxFont:TDxFont);
begin
  GuiFont.Color := DxFont.Color;
  GuiFont.BColor := DxFont.BColor;
  GuiFont.Size := DxFont.Size;
  GuiFont.Bold := DxFont.Bold;
  GuiFont.Style := DxFont.Style;
  GuiFont.NameLen := Length(DxFont.Name);
end;

function ReadGuiFontName(AFont:TGuiFont):string;
var
  Text:string;
begin
  Result := '';
  if AFont.NameLen > 0 then begin
    SetLength(Text, AFont.NameLen);
    ReadMemory(Text[1], AFont.NameLen);
    Result := Text;
  end;
end;

procedure LoadComponent(Gui:TGuiType; DxControl:TDxControl; GuiVersion:Integer);
var
  sText:string;
  I:Integer;
  GuiImageForm:TGuiImageForm;
  GuiImageFormShape:TGuiImageFormShape;

  GuiImageForm_New:TGuiImageForm_New;
  GuiImageForm_New2:TGuiImageForm_New2;
  GuiImageForm_New3:TGuiImageForm_New3;

  GuiImageButton:TGuiImageButton;
  GuiImageButton_New2:TGuiImageButton_New2;
  GuiImageButton_New3:TGuiImageButton_New3;
  GuiEdit:TGuiEdit;
  GuiImageEdit:TGuiImageEdit;
  GuiImageEdit_New:TGuiImageEdit_New;
  GuiLabel:TGuiLabel;
  GuiLabel2:TGuiLabel_New;
  GuiMemo:TGuiMemo;
  GuiMemo_New:TGuiMemo_New;
  GuiImageGrid:TGuiImageGrid;
  GuiPopupMenu:TGuiPopupMenu;
  GuiComboBox:TGuiComboBox;
  GuiPageControl:TGuiPageControl;
  GuiPageControl_New:TGuiPageControl_New;
  GuiTabSheet:TGuiTabSheet;
  GuiLine:TGuiLine;
  GuiViewField:TGuiViewField;
  ColRect:TRect;
  GuiTrackBar:TGuiTrackBar;
  GuiMainBottomForm:TGuiMainBottomForm;
  GuiMainBottomForm_New:TGuiMainBottomForm_New;
  GuiMainBottomForm_New2:TGuiMainBottomForm_New2;
  GuiMagicBall:TGuiMagicBall;
  GuiMagicBall2:TGuiMagicBall2;
  GuiSexPanel:TGuiSexPanel;
  GuiGroupAttackProgress:TGuiGroupAttackProgress;
  GuiImageProgress:TGuiImageProgress;
  GuiSwitchButton:TGuiSwitchButton;

  DxImageForm:TDxImageForm;
  DxImageFormShape:TDxImageFormShape;
  DxImageButton:TDxImageButton;
  DxPageControl:TDxPageControl;
  DxTabSheet:TDxTabSheet;

  DxEdit:TDxEdit;
  DxImageEdit:TDxImageEdit;
  DxLabel:TDxLabel;
  DxScrollControl:TDxScrollControl;
  DxImageGrid:TDxImageGrid;
  DxPopupMenu:TDxPopupMenu;
  DxComboBox:TDxComboBox;
  DxLine:TDxLine;
  DxTrackBar:TDxTrackBar;
  DxMainBottomForm:TDxMainBottomForm;
  DxMagicBall:TDxMagicBall;
  DxSexPanel:TDxSexPanel;
  DxGroupAttackProgress:TDxGroupAttackProgress;
  DxImageProgress:TDxImageProgress;
  DxSwitchButton:TDxSwitchButton;

  DxListView:TDxListView;
begin
  // Gui := TGuiType(DxControl.Tag);
  case Gui of
    t_Form:begin
        if GuiVersion < 20160409 then begin
          ReadMemory(GuiImageForm, SizeOf(GuiImageForm));
          DxImageForm := TDxImageForm(DxControl);
          DxImageForm.AutoSize := GuiImageForm.AutoSize;
          DxImageForm.ImageIndex.ImageType := GuiImageForm.ImageIndex.Image;
          DxImageForm.ImageIndex.Up := GuiImageForm.ImageIndex.Up;
          DxImageForm.ImageIndex.Hot := GuiImageForm.ImageIndex.Hot;
          DxImageForm.ImageIndex.Down := GuiImageForm.ImageIndex.Down;
          DxImageForm.ImageIndex.Disabled := GuiImageForm.ImageIndex.Disabled;
          DxImageForm.Center := GuiImageForm.Center;
        end else if GuiVersion < 20160514 then begin
          ReadMemory(GuiImageForm_New, SizeOf(GuiImageForm_New));
          DxImageForm := TDxImageForm(DxControl);
          DxImageForm.AutoSize := GuiImageForm_New.AutoSize;
          DxImageForm.ImageIndex.ImageType := GuiImageForm_New.ImageIndex.Image;
          DxImageForm.ImageIndex.Up := GuiImageForm_New.ImageIndex.Up;
          DxImageForm.ImageIndex.Hot := GuiImageForm_New.ImageIndex.Hot;
          DxImageForm.ImageIndex.Down := GuiImageForm_New.ImageIndex.Down;
          DxImageForm.ImageIndex.Disabled := GuiImageForm_New.ImageIndex.Disabled;
          DxImageForm.Center := GuiImageForm_New.Center;
          DxImageForm.BackgroundColor := GuiImageForm_New.BackgroundColor;
          DxImageForm.BackgroundAlpha := GuiImageForm_New.BackgroundAlpha;
        end else if GuiVersion < 20171106 then begin
          ReadMemory(GuiImageForm_New2, SizeOf(GuiImageForm_New2));
          DxImageForm := TDxImageForm(DxControl);
          DxImageForm.AutoSize := GuiImageForm_New2.AutoSize;
          DxImageForm.ImageIndex.ImageType := GuiImageForm_New2.ImageIndex.Image;
          DxImageForm.ImageIndex.Up := GuiImageForm_New2.ImageIndex.Up;
          DxImageForm.ImageIndex.Hot := GuiImageForm_New2.ImageIndex.Hot;
          DxImageForm.ImageIndex.Down := GuiImageForm_New2.ImageIndex.Down;
          DxImageForm.ImageIndex.Disabled := GuiImageForm_New2.ImageIndex.Disabled;
          DxImageForm.Center := GuiImageForm_New2.Center;
          DxImageForm.BackgroundColor := GuiImageForm_New2.BackgroundColor;
          DxImageForm.BackgroundAlpha := GuiImageForm_New2.BackgroundAlpha;

          DxImageForm.Animation1.ImageType := GuiImageForm_New2.Animation1.ImageType;
          DxImageForm.Animation1.StartIndex := GuiImageForm_New2.Animation1.StartIndex;
          DxImageForm.Animation1.EndIndex := GuiImageForm_New2.Animation1.EndIndex;
          DxImageForm.Animation1.FrameTime := GuiImageForm_New2.Animation1.FrameTime;
          DxImageForm.Animation1.PlayCount := GuiImageForm_New2.Animation1.PlayCount;
          DxImageForm.Animation1.OffsetX := GuiImageForm_New2.Animation1.OffsetX;
          DxImageForm.Animation1.OffsetY := GuiImageForm_New2.Animation1.OffsetY;
          DxImageForm.Animation1.UseImageOffset := GuiImageForm_New2.Animation1.UseImageOffset;
          DxImageForm.Animation1.OutsideAreaDraw := GuiImageForm_New2.Animation1.OutsideAreaDraw;
          DxImageForm.Animation1.Draw := GuiImageForm_New2.Animation1.Draw;
          DxImageForm.Animation1.BlendDraw := GuiImageForm_New2.Animation1.BlendDraw;
          DxImageForm.Animation1.DrawBeforeDef := GuiImageForm_New2.Animation1.DrawBeforeDef;

          DxImageForm.Animation2.ImageType := GuiImageForm_New2.Animation2.ImageType;
          DxImageForm.Animation2.StartIndex := GuiImageForm_New2.Animation2.StartIndex;
          DxImageForm.Animation2.EndIndex := GuiImageForm_New2.Animation2.EndIndex;
          DxImageForm.Animation2.FrameTime := GuiImageForm_New2.Animation2.FrameTime;
          DxImageForm.Animation2.PlayCount := GuiImageForm_New2.Animation2.PlayCount;
          DxImageForm.Animation2.OffsetX := GuiImageForm_New2.Animation2.OffsetX;
          DxImageForm.Animation2.OffsetY := GuiImageForm_New2.Animation2.OffsetY;
          DxImageForm.Animation2.UseImageOffset := GuiImageForm_New2.Animation2.UseImageOffset;
          DxImageForm.Animation2.OutsideAreaDraw := GuiImageForm_New2.Animation2.OutsideAreaDraw;
          DxImageForm.Animation2.Draw := GuiImageForm_New2.Animation2.Draw;
          DxImageForm.Animation2.BlendDraw := GuiImageForm_New2.Animation2.BlendDraw;
          DxImageForm.Animation2.DrawBeforeDef := GuiImageForm_New2.Animation2.DrawBeforeDef;

          DxImageForm.Animation3.ImageType := GuiImageForm_New2.Animation3.ImageType;
          DxImageForm.Animation3.StartIndex := GuiImageForm_New2.Animation3.StartIndex;
          DxImageForm.Animation3.EndIndex := GuiImageForm_New2.Animation3.EndIndex;
          DxImageForm.Animation3.FrameTime := GuiImageForm_New2.Animation3.FrameTime;
          DxImageForm.Animation3.PlayCount := GuiImageForm_New2.Animation3.PlayCount;
          DxImageForm.Animation3.OffsetX := GuiImageForm_New2.Animation3.OffsetX;
          DxImageForm.Animation3.OffsetY := GuiImageForm_New2.Animation3.OffsetY;
          DxImageForm.Animation3.UseImageOffset := GuiImageForm_New2.Animation3.UseImageOffset;
          DxImageForm.Animation3.OutsideAreaDraw := GuiImageForm_New2.Animation3.OutsideAreaDraw;
          DxImageForm.Animation3.Draw := GuiImageForm_New2.Animation3.Draw;
          DxImageForm.Animation3.BlendDraw := GuiImageForm_New2.Animation3.BlendDraw;
          DxImageForm.Animation3.DrawBeforeDef := GuiImageForm_New2.Animation3.DrawBeforeDef;
        end else begin
          ReadMemory(GuiImageForm_New3, SizeOf(GuiImageForm_New3));
          DxImageForm := TDxImageForm(DxControl);
          DxImageForm.AutoSize := GuiImageForm_New3.AutoSize;
          DxImageForm.ImageIndex.ImageType := GuiImageForm_New3.ImageIndex.Image;
          DxImageForm.ImageIndex.Up := GuiImageForm_New3.ImageIndex.Up;
          DxImageForm.ImageIndex.OffsetX := GuiImageForm_New3.ImageIndex.OffsetX;
          DxImageForm.ImageIndex.OffsetY := GuiImageForm_New3.ImageIndex.OffsetY;
          DxImageForm.Center := GuiImageForm_New3.Center;
          DxImageForm.BackgroundColor := GuiImageForm_New3.BackgroundColor;
          DxImageForm.BackgroundAlpha := GuiImageForm_New3.BackgroundAlpha;

          DxImageForm.Animation1.ImageType := GuiImageForm_New3.Animation1.ImageType;
          DxImageForm.Animation1.StartIndex := GuiImageForm_New3.Animation1.StartIndex;
          DxImageForm.Animation1.EndIndex := GuiImageForm_New3.Animation1.EndIndex;
          DxImageForm.Animation1.FrameTime := GuiImageForm_New3.Animation1.FrameTime;
          DxImageForm.Animation1.PlayCount := GuiImageForm_New3.Animation1.PlayCount;
          DxImageForm.Animation1.OffsetX := GuiImageForm_New3.Animation1.OffsetX;
          DxImageForm.Animation1.OffsetY := GuiImageForm_New3.Animation1.OffsetY;
          DxImageForm.Animation1.UseImageOffset := GuiImageForm_New3.Animation1.UseImageOffset;
          DxImageForm.Animation1.OutsideAreaDraw := GuiImageForm_New3.Animation1.OutsideAreaDraw;
          DxImageForm.Animation1.Draw := GuiImageForm_New3.Animation1.Draw;
          DxImageForm.Animation1.BlendDraw := GuiImageForm_New3.Animation1.BlendDraw;
          DxImageForm.Animation1.DrawBeforeDef := GuiImageForm_New3.Animation1.DrawBeforeDef;

          DxImageForm.Animation2.ImageType := GuiImageForm_New3.Animation2.ImageType;
          DxImageForm.Animation2.StartIndex := GuiImageForm_New3.Animation2.StartIndex;
          DxImageForm.Animation2.EndIndex := GuiImageForm_New3.Animation2.EndIndex;
          DxImageForm.Animation2.FrameTime := GuiImageForm_New3.Animation2.FrameTime;
          DxImageForm.Animation2.PlayCount := GuiImageForm_New3.Animation2.PlayCount;
          DxImageForm.Animation2.OffsetX := GuiImageForm_New3.Animation2.OffsetX;
          DxImageForm.Animation2.OffsetY := GuiImageForm_New3.Animation2.OffsetY;
          DxImageForm.Animation2.UseImageOffset := GuiImageForm_New3.Animation2.UseImageOffset;
          DxImageForm.Animation2.OutsideAreaDraw := GuiImageForm_New3.Animation2.OutsideAreaDraw;
          DxImageForm.Animation2.Draw := GuiImageForm_New3.Animation2.Draw;
          DxImageForm.Animation2.BlendDraw := GuiImageForm_New3.Animation2.BlendDraw;
          DxImageForm.Animation2.DrawBeforeDef := GuiImageForm_New3.Animation2.DrawBeforeDef;

          DxImageForm.Animation3.ImageType := GuiImageForm_New3.Animation3.ImageType;
          DxImageForm.Animation3.StartIndex := GuiImageForm_New3.Animation3.StartIndex;
          DxImageForm.Animation3.EndIndex := GuiImageForm_New3.Animation3.EndIndex;
          DxImageForm.Animation3.FrameTime := GuiImageForm_New3.Animation3.FrameTime;
          DxImageForm.Animation3.PlayCount := GuiImageForm_New3.Animation3.PlayCount;
          DxImageForm.Animation3.OffsetX := GuiImageForm_New3.Animation3.OffsetX;
          DxImageForm.Animation3.OffsetY := GuiImageForm_New3.Animation3.OffsetY;
          DxImageForm.Animation3.UseImageOffset := GuiImageForm_New3.Animation3.UseImageOffset;
          DxImageForm.Animation3.OutsideAreaDraw := GuiImageForm_New3.Animation3.OutsideAreaDraw;
          DxImageForm.Animation3.Draw := GuiImageForm_New3.Animation3.Draw;
          DxImageForm.Animation3.BlendDraw := GuiImageForm_New3.Animation3.BlendDraw;
          DxImageForm.Animation3.DrawBeforeDef := GuiImageForm_New3.Animation3.DrawBeforeDef;
        end;
      end;
    t_FormShape:begin
        ReadMemory(GuiImageFormShape, SizeOf(TGuiImageFormShape));
        DxImageFormShape := TDxImageFormShape(DxControl);
        DxImageFormShape.AutoSize := GuiImageFormShape.AutoSize;
        DxImageFormShape.ImageIndex.ImageType := GuiImageFormShape.ImageIndex.Image;
        DxImageFormShape.ImageIndex.Up := GuiImageFormShape.ImageIndex.Up;
        DxImageFormShape.ImageIndex.Hot := GuiImageFormShape.ImageIndex.Hot;
        DxImageFormShape.ImageIndex.Down := GuiImageFormShape.ImageIndex.Down;
        DxImageFormShape.ImageIndex.Disabled := GuiImageFormShape.ImageIndex.Disabled;
        DxImageFormShape.Center := GuiImageFormShape.Center;
        for I := 0 to DxImageFormShape.ImageCount - 1 do begin
          DxImageFormShape.Items[I].ImageType := GuiImageFormShape.ImageIndexs[I].ImageType;
          DxImageFormShape.Items[I].ImageIndex := GuiImageFormShape.ImageIndexs[I].ImageIndex;
          DxImageFormShape.Items[I].Align := GuiImageFormShape.ImageIndexs[I].Align;
          DxImageFormShape.Items[I].Draw := GuiImageFormShape.ImageIndexs[I].Draw;
          DxImageFormShape.Items[I].Stretch := GuiImageFormShape.ImageIndexs[I].Stretch;
          DxImageFormShape.Items[I].Center := GuiImageFormShape.ImageIndexs[I].Center;
          DxImageFormShape.Items[I].BlendMode := GuiImageFormShape.ImageIndexs[I].BlendMode;
          DxImageFormShape.Items[I].SourceRect := GuiImageFormShape.ImageIndexs[I].SrcRect;
          DxImageFormShape.Items[I].DestRect := GuiImageFormShape.ImageIndexs[I].DestRect;
        end;
      end;
    t_Button:begin
        if GuiVersion < 20171106 then begin
          ReadMemory(GuiImageButton, SizeOf(TGuiImageButton));
          DxImageButton := TDxImageButton(DxControl);
          // with DxImageButton do begin
          DxImageButton.AutoSize := GuiImageButton.AutoSize;
          DxImageButton.Alignment := GuiImageButton.Alignment;
          DxImageButton.CaptionDownOffsetX := GuiImageButton.CaptionDownOffsetX;
          DxImageButton.CaptionDownOffsetY := GuiImageButton.CaptionDownOffsetY;
          DxImageButton.ImageIndex.ImageType := GuiImageButton.ImageIndex.Image;
          DxImageButton.ImageIndex.Up := GuiImageButton.ImageIndex.Up;
          DxImageButton.ImageIndex.Hot := GuiImageButton.ImageIndex.Hot;
          DxImageButton.ImageIndex.Down := GuiImageButton.ImageIndex.Down;
          DxImageButton.ImageIndex.Disabled := GuiImageButton.ImageIndex.Disabled;

          DxFontAssign(DxImageButton.CaptionColor.Up, GuiImageButton.CaptionColor.Up);
          DxFontAssign(DxImageButton.CaptionColor.Hot, GuiImageButton.CaptionColor.Hot);
          DxFontAssign(DxImageButton.CaptionColor.Down, GuiImageButton.CaptionColor.Down);
          DxFontAssign(DxImageButton.CaptionColor.Disabled, GuiImageButton.CaptionColor.Disabled);

          DxImageButton.Checked := GuiImageButton.Checked;
          DxImageButton.ClickCount := GuiImageButton.ClickCount;
          DxImageButton.Style := GuiImageButton.Style;
          DxImageButton.Caption := '';
          // end;
          DxImageButton.CaptionColor.Up.Name := ReadGuiFontName(GuiImageButton.CaptionColor.Up);
          DxImageButton.CaptionColor.Hot.Name := ReadGuiFontName(GuiImageButton.CaptionColor.Hot);
          DxImageButton.CaptionColor.Down.Name := ReadGuiFontName(GuiImageButton.CaptionColor.Down);
          DxImageButton.CaptionColor.Disabled.Name := ReadGuiFontName(GuiImageButton.CaptionColor.Disabled);

          if GuiImageButton.CaptionLen > 0 then begin
            SetLength(sText, GuiImageButton.CaptionLen);
            ReadMemory(sText[1], GuiImageButton.CaptionLen);
            DxImageButton.Caption := sText;
          end;
        end
        else if GuiVersion < 20180619 then begin
          ReadMemory(GuiImageButton_New2, SizeOf(GuiImageButton_New2));
          DxImageButton := TDxImageButton(DxControl);
          // with DxImageButton do begin
          DxImageButton.AutoSize := GuiImageButton_New2.AutoSize;
          DxImageButton.Alignment := GuiImageButton_New2.Alignment;
          DxImageButton.CaptionDownOffsetX := GuiImageButton_New2.CaptionDownOffsetX;
          DxImageButton.CaptionDownOffsetY := GuiImageButton_New2.CaptionDownOffsetY;
          DxImageButton.CaptionOffsetX := GuiImageButton_New2.CaptionOffsetX;
          DxImageButton.CaptionOffsetY := GuiImageButton_New2.CaptionOffsetY;
          DxImageButton.ImageIndex.ImageType := GuiImageButton_New2.ImageIndex.Image;
          DxImageButton.ImageIndex.Up := GuiImageButton_New2.ImageIndex.Up;
          DxImageButton.ImageIndex.Hot := GuiImageButton_New2.ImageIndex.Hot;
          DxImageButton.ImageIndex.Down := GuiImageButton_New2.ImageIndex.Down;
          DxImageButton.ImageIndex.Disabled := GuiImageButton_New2.ImageIndex.Disabled;
          DxImageButton.ImageIndex.Checked := GuiImageButton_New2.ImageIndex.Checked;

          DxFontAssign(DxImageButton.CaptionColor.Up, GuiImageButton_New2.CaptionColor.Up);
          DxFontAssign(DxImageButton.CaptionColor.Hot, GuiImageButton_New2.CaptionColor.Hot);
          DxFontAssign(DxImageButton.CaptionColor.Down, GuiImageButton_New2.CaptionColor.Down);
          DxFontAssign(DxImageButton.CaptionColor.Disabled, GuiImageButton_New2.CaptionColor.Disabled);

          DxImageButton.Checked := GuiImageButton_New2.Checked;
          DxImageButton.ClickCount := GuiImageButton_New2.ClickCount;
          DxImageButton.Style := GuiImageButton_New2.Style;
          DxImageButton.Caption := '';
          // end;
          DxImageButton.CaptionColor.Up.Name := ReadGuiFontName(GuiImageButton_New2.CaptionColor.Up);
          DxImageButton.CaptionColor.Hot.Name := ReadGuiFontName(GuiImageButton_New2.CaptionColor.Hot);
          DxImageButton.CaptionColor.Down.Name := ReadGuiFontName(GuiImageButton_New2.CaptionColor.Down);
          DxImageButton.CaptionColor.Disabled.Name := ReadGuiFontName(GuiImageButton_New2.CaptionColor.Disabled);

          if GuiImageButton_New2.CaptionLen > 0 then begin
            SetLength(sText, GuiImageButton_New2.CaptionLen);
            ReadMemory(sText[1], GuiImageButton_New2.CaptionLen);
            DxImageButton.Caption := sText;
          end;
        end
        else begin
          ReadMemory(GuiImageButton_New3, SizeOf(GuiImageButton_New3));
          DxImageButton := TDxImageButton(DxControl);
          // with DxImageButton do begin
          DxImageButton.AutoSize := GuiImageButton_New3.AutoSize;
          DxImageButton.Alignment := GuiImageButton_New3.Alignment;

          DxImageButton.CaptionDownOffsetX := GuiImageButton_New3.CaptionDownOffsetX;
          DxImageButton.CaptionDownOffsetY := GuiImageButton_New3.CaptionDownOffsetY;
          DxImageButton.CaptionOffsetX := GuiImageButton_New3.CaptionOffsetX;
          DxImageButton.CaptionOffsetY := GuiImageButton_New3.CaptionOffsetY;

          DxImageButton.ImageIndex.ImageType := GuiImageButton_New3.ImageIndex.Image;
          DxImageButton.ImageIndex.Up := GuiImageButton_New3.ImageIndex.Up;
          DxImageButton.ImageIndex.Hot := GuiImageButton_New3.ImageIndex.Hot;
          DxImageButton.ImageIndex.Down := GuiImageButton_New3.ImageIndex.Down;
          DxImageButton.ImageIndex.Disabled := GuiImageButton_New3.ImageIndex.Disabled;
          DxImageButton.ImageIndex.Checked := GuiImageButton_New3.ImageIndex.Checked;

          DxFontAssign(DxImageButton.CaptionColor.Up, GuiImageButton_New3.CaptionColor.Up);
          DxFontAssign(DxImageButton.CaptionColor.Hot, GuiImageButton_New3.CaptionColor.Hot);
          DxFontAssign(DxImageButton.CaptionColor.Down, GuiImageButton_New3.CaptionColor.Down);
          DxFontAssign(DxImageButton.CaptionColor.Disabled, GuiImageButton_New3.CaptionColor.Disabled);

          DxImageButton.Checked := GuiImageButton_New3.Checked;
          DxImageButton.ClickCount := GuiImageButton_New3.ClickCount;
          DxImageButton.Style := GuiImageButton_New3.Style;
          DxImageButton.Caption := '';

          DxImageButton.Animation.ImageType := GuiImageButton_New3.Animation.ImageType;
          DxImageButton.Animation.ShowType := GuiImageButton_New3.Animation.ShowType;
          DxImageButton.Animation.StartIndex := GuiImageButton_New3.Animation.StartIndex;
          DxImageButton.Animation.EndIndex := GuiImageButton_New3.Animation.EndIndex;
          DxImageButton.Animation.FrameTime := GuiImageButton_New3.Animation.FrameTime;
          DxImageButton.Animation.PlayCount := GuiImageButton_New3.Animation.PlayCount;
          DxImageButton.Animation.OffsetX := GuiImageButton_New3.Animation.OffsetX;
          DxImageButton.Animation.OffsetY := GuiImageButton_New3.Animation.OffsetY;
          DxImageButton.Animation.UseImageOffset := GuiImageButton_New3.Animation.UseImageOffset;
          DxImageButton.Animation.OutsideAreaDraw := GuiImageButton_New3.Animation.OutsideAreaDraw;
          DxImageButton.Animation.Draw := GuiImageButton_New3.Animation.Draw;
          DxImageButton.Animation.BlendDraw := GuiImageButton_New3.Animation.BlendDraw;
          DxImageButton.Animation.DrawBeforeDef := GuiImageButton_New3.Animation.DrawBeforeDef;

          // end;
          DxImageButton.CaptionColor.Up.Name := ReadGuiFontName(GuiImageButton_New3.CaptionColor.Up);
          DxImageButton.CaptionColor.Hot.Name := ReadGuiFontName(GuiImageButton_New3.CaptionColor.Hot);
          DxImageButton.CaptionColor.Down.Name := ReadGuiFontName(GuiImageButton_New3.CaptionColor.Down);
          DxImageButton.CaptionColor.Disabled.Name := ReadGuiFontName(GuiImageButton_New3.CaptionColor.Disabled);

          if GuiImageButton_New3.CaptionLen > 0 then begin
            SetLength(sText, GuiImageButton_New3.CaptionLen);
            ReadMemory(sText[1], GuiImageButton_New3.CaptionLen);
            DxImageButton.Caption := sText;
          end;
        end;
      end;
    t_Edit:begin
        ReadMemory(GuiEdit, SizeOf(TGuiEdit));
        DxEdit := TDxEdit(DxControl);

        // with DxEdit do begin
        DxEdit.Text := '';
        DxEdit.BackgroundColor := GuiEdit.BackgroundColor;
        DxEdit.DrawBorder := GuiEdit.DrawBorder;

        DxFontAssign(DxEdit.Font, GuiEdit.FontColor);
        DxFontAssign(DxEdit.BorderColor.Up, GuiEdit.BorderColor.Up);
        DxFontAssign(DxEdit.BorderColor.Hot, GuiEdit.BorderColor.Hot);
        DxFontAssign(DxEdit.BorderColor.Down, GuiEdit.BorderColor.Down);
        DxFontAssign(DxEdit.BorderColor.Disabled, GuiEdit.BorderColor.Disabled);

        DxEdit.ReadOnly := GuiEdit.ReadOnly;
        DxEdit.MaxLength := GuiEdit.MaxLength;
        DxEdit.SelectedColor := GuiEdit.SelectedColor;
        DxEdit.SelBackColor := GuiEdit.SelBackColor;
        DxEdit.SelFontColor := GuiEdit.SelFontColor;
        DxEdit.InValue := GuiEdit.InValue;
        DxEdit.PasswordChar := GuiEdit.PasswordChar;
        DxEdit.AllowSelect := GuiEdit.AllowSelect;
        DxEdit.AllowPaste := GuiEdit.AllowPaste;
        DxEdit.TabOrder := GuiEdit.TabOrder;
        // end;
        DxEdit.Font.Name := ReadGuiFontName(GuiEdit.FontColor);

        if GuiEdit.TextLen > 0 then begin
          SetLength(sText, GuiEdit.TextLen);
          ReadMemory(sText[1], GuiEdit.TextLen);
          DxEdit.Text := sText;
        end;
      end;
    t_ImageEdit:begin
        if GuiVersion < 20160430 then begin
          ReadMemory(GuiEdit, SizeOf(TGuiEdit));
          DxImageEdit := TDxImageEdit(DxControl);

          // with DxImageEdit do begin
          DxImageEdit.Text := '';
          DxImageEdit.BackgroundColor := GuiEdit.BackgroundColor;
          DxImageEdit.DrawBorder := GuiEdit.DrawBorder;

          DxFontAssign(DxImageEdit.Font, GuiEdit.FontColor);
          DxFontAssign(DxImageEdit.BorderColor.Up, GuiEdit.BorderColor.Up);
          DxFontAssign(DxImageEdit.BorderColor.Hot, GuiEdit.BorderColor.Hot);
          DxFontAssign(DxImageEdit.BorderColor.Down, GuiEdit.BorderColor.Down);
          DxFontAssign(DxImageEdit.BorderColor.Disabled, GuiEdit.BorderColor.Disabled);

          DxImageEdit.ReadOnly := GuiEdit.ReadOnly;
          DxImageEdit.MaxLength := GuiEdit.MaxLength;
          DxImageEdit.SelectedColor := GuiEdit.SelectedColor;
          DxImageEdit.SelBackColor := GuiEdit.SelBackColor;
          DxImageEdit.SelFontColor := GuiEdit.SelFontColor;
          DxImageEdit.InValue := GuiEdit.InValue;
          DxImageEdit.PasswordChar := GuiEdit.PasswordChar;
          DxImageEdit.AllowSelect := GuiEdit.AllowSelect;
          DxImageEdit.AllowPaste := GuiEdit.AllowPaste;
          DxImageEdit.TabOrder := GuiEdit.TabOrder;
          // end;
          DxImageEdit.Font.Name := ReadGuiFontName(GuiEdit.FontColor);

          if GuiEdit.TextLen > 0 then begin
            SetLength(sText, GuiEdit.TextLen);
            ReadMemory(sText[1], GuiEdit.TextLen);
            DxImageEdit.Text := sText;
          end;
        end
        else if GuiVersion < 20190724 then begin
          ReadMemory(GuiImageEdit, SizeOf(TGuiImageEdit));
          DxImageEdit := TDxImageEdit(DxControl);

          // with DxImageEdit do begin
          DxImageEdit.Text := '';
          DxImageEdit.BackgroundColor := GuiImageEdit.BackgroundColor;

          DxImageEdit.DrawBorder := GuiImageEdit.DrawBorder;

          DxFontAssign(DxImageEdit.Font, GuiImageEdit.FontColor);

          DxImageEdit.DisableBackgroundColor := GuiImageEdit.DisableBackgroundColor;
          DxFontAssign(DxImageEdit.HintTextFont, GuiImageEdit.HintTextFont);
          DxImageEdit.HintTextAlignment := GuiImageEdit.HintTextAlignment;

          DxFontAssign(DxImageEdit.BorderColor.Up, GuiImageEdit.BorderColor.Up);
          DxFontAssign(DxImageEdit.BorderColor.Hot, GuiImageEdit.BorderColor.Hot);
          DxFontAssign(DxImageEdit.BorderColor.Down, GuiImageEdit.BorderColor.Down);
          DxFontAssign(DxImageEdit.BorderColor.Disabled, GuiImageEdit.BorderColor.Disabled);

          DxImageEdit.ReadOnly := GuiImageEdit.ReadOnly;
          DxImageEdit.MaxLength := GuiImageEdit.MaxLength;
          DxImageEdit.SelectedColor := GuiImageEdit.SelectedColor;
          DxImageEdit.SelBackColor := GuiImageEdit.SelBackColor;
          DxImageEdit.SelFontColor := GuiImageEdit.SelFontColor;
          DxImageEdit.InValue := GuiImageEdit.InValue;
          DxImageEdit.PasswordChar := GuiImageEdit.PasswordChar;
          DxImageEdit.AllowSelect := GuiImageEdit.AllowSelect;
          DxImageEdit.AllowPaste := GuiImageEdit.AllowPaste;
          DxImageEdit.TabOrder := GuiImageEdit.TabOrder;

          DxImageEdit.Font.Name := ReadGuiFontName(GuiImageEdit.FontColor);
          DxImageEdit.HintTextFont.Name := ReadGuiFontName(GuiImageEdit.HintTextFont);

          if GuiImageEdit.TextLen > 0 then begin
            SetLength(sText, GuiImageEdit.TextLen);
            ReadMemory(sText[1], GuiImageEdit.TextLen);
            DxImageEdit.Text := sText;
          end;

          if GuiImageEdit.HintTextLen > 0 then begin
            SetLength(sText, GuiImageEdit.HintTextLen);
            ReadMemory(sText[1], GuiImageEdit.HintTextLen);
            DxImageEdit.HintText := sText;
          end;
        end
        else begin
          ReadMemory(GuiImageEdit_New, SizeOf(TGuiImageEdit_New));
          DxImageEdit := TDxImageEdit(DxControl);

          // with DxImageEdit do begin
          DxImageEdit.Text := '';

          DxImageEdit.DrawBorder := GuiImageEdit_New.DrawBorder;

          DxFontAssign(DxImageEdit.Font, GuiImageEdit_New.FontColor);

          DxImageEdit.BackgroundColor := GuiImageEdit_New.BackgroundColor;
          DxImageEdit.BackgroundColorAlpha := GuiImageEdit_New.BackgroundColorAlpha;
          DxImageEdit.BackgroundImage.ImageType := GuiImageEdit_New.BackgroundImage.ImageType;
          DxImageEdit.BackgroundImage.BlendDraw := GuiImageEdit_New.BackgroundImage.BlendMode;
          DxImageEdit.BackgroundImage.OutsideAreaDraw := GuiImageEdit_New.BackgroundImage.OutsideAreaDraw;
          DxImageEdit.BackgroundImage.ImageIndex := GuiImageEdit_New.BackgroundImage.ImageIndex;
          DxImageEdit.BackgroundImage.OffsetX := GuiImageEdit_New.BackgroundImage.OffsetX;
          DxImageEdit.BackgroundImage.OffsetY := GuiImageEdit_New.BackgroundImage.OffsetY;

          DxImageEdit.DisableHideCtrl := GuiImageEdit_New.DisableHideCtrl;
          DxImageEdit.DisableBackgroundTransparent := GuiImageEdit_New.DisableBackgroundTransparent;
          DxImageEdit.DisableBackgroundColor := GuiImageEdit_New.DisableBackgroundColor;
          DxImageEdit.DisableBackgroundAlpha := GuiImageEdit_New.DisableBackgroundAlpha;
          DxImageEdit.DisableBackgroundImage.ImageType := GuiImageEdit_New.DisableBackgroundImage.ImageType;
          DxImageEdit.DisableBackgroundImage.BlendDraw := GuiImageEdit_New.DisableBackgroundImage.BlendMode;
          DxImageEdit.DisableBackgroundImage.OutsideAreaDraw := GuiImageEdit_New.DisableBackgroundImage.OutsideAreaDraw;
          DxImageEdit.DisableBackgroundImage.ImageIndex := GuiImageEdit_New.DisableBackgroundImage.ImageIndex;
          DxImageEdit.DisableBackgroundImage.OffsetX := GuiImageEdit_New.DisableBackgroundImage.OffsetX;
          DxImageEdit.DisableBackgroundImage.OffsetY := GuiImageEdit_New.DisableBackgroundImage.OffsetY;

          DxFontAssign(DxImageEdit.HintTextFont, GuiImageEdit_New.HintTextFont);
          DxImageEdit.HintTextAlignment := GuiImageEdit_New.HintTextAlignment;

          DxFontAssign(DxImageEdit.BorderColor.Up, GuiImageEdit_New.BorderColor.Up);
          DxFontAssign(DxImageEdit.BorderColor.Hot, GuiImageEdit_New.BorderColor.Hot);
          DxFontAssign(DxImageEdit.BorderColor.Down, GuiImageEdit_New.BorderColor.Down);
          DxFontAssign(DxImageEdit.BorderColor.Disabled, GuiImageEdit_New.BorderColor.Disabled);

          DxImageEdit.ReadOnly := GuiImageEdit_New.ReadOnly;
          DxImageEdit.MaxLength := GuiImageEdit_New.MaxLength;
          DxImageEdit.SelectedColor := GuiImageEdit_New.SelectedColor;
          DxImageEdit.SelBackColor := GuiImageEdit_New.SelBackColor;
          DxImageEdit.SelFontColor := GuiImageEdit_New.SelFontColor;
          DxImageEdit.InValue := GuiImageEdit_New.InValue;
          DxImageEdit.PasswordChar := GuiImageEdit_New.PasswordChar;
          DxImageEdit.AllowSelect := GuiImageEdit_New.AllowSelect;
          DxImageEdit.AllowPaste := GuiImageEdit_New.AllowPaste;
          DxImageEdit.TabOrder := GuiImageEdit_New.TabOrder;

          DxImageEdit.Font.Name := ReadGuiFontName(GuiImageEdit_New.FontColor);
          DxImageEdit.HintTextFont.Name := ReadGuiFontName(GuiImageEdit_New.HintTextFont);

          if GuiImageEdit_New.TextLen > 0 then begin
            SetLength(sText, GuiImageEdit_New.TextLen);
            ReadMemory(sText[1], GuiImageEdit_New.TextLen);
            DxImageEdit.Text := sText;
          end;

          if GuiImageEdit_New.HintTextLen > 0 then begin
            SetLength(sText, GuiImageEdit_New.HintTextLen);
            ReadMemory(sText[1], GuiImageEdit_New.HintTextLen);
            DxImageEdit.HintText := sText;
          end;
        end;
      end;
    t_Label:begin
        if GuiVersion < 20160409 then begin
          ReadMemory(GuiLabel, SizeOf(TGuiLabel));
          DxLabel := TDxLabel(DxControl);
          DxLabel.AutoSize := GuiLabel.AutoSize;
          DxLabel.BackgroundColor := GuiLabel.BackgroundColor;
          DxLabel.DrawBorder := GuiLabel.DrawBorder;
          DxLabel.CaptionDownOffsetX := GuiLabel.CaptionDownOffsetX;
          DxLabel.CaptionDownOffsetY := GuiLabel.CaptionDownOffsetY;
          DxFontAssign(DxLabel.CaptionColor.Up, GuiLabel.CaptionColor.Up);
          DxFontAssign(DxLabel.CaptionColor.Hot, GuiLabel.CaptionColor.Hot);
          DxFontAssign(DxLabel.CaptionColor.Down, GuiLabel.CaptionColor.Down);
          DxFontAssign(DxLabel.CaptionColor.Disabled, GuiLabel.CaptionColor.Disabled);

          DxFontAssign(DxLabel.BorderColor.Up, GuiLabel.BorderColor.Up);
          DxFontAssign(DxLabel.BorderColor.Hot, GuiLabel.BorderColor.Hot);
          DxFontAssign(DxLabel.BorderColor.Down, GuiLabel.BorderColor.Down);
          DxFontAssign(DxLabel.BorderColor.Disabled, GuiLabel.BorderColor.Disabled);

          DxLabel.ClickCount := GuiLabel.ClickCount;
          DxLabel.Style := GuiLabel.Style;
          DxLabel.Caption := '';

          DxLabel.CaptionColor.Up.Name := ReadGuiFontName(GuiLabel.CaptionColor.Up);
          DxLabel.CaptionColor.Hot.Name := ReadGuiFontName(GuiLabel.CaptionColor.Hot);
          DxLabel.CaptionColor.Down.Name := ReadGuiFontName(GuiLabel.CaptionColor.Down);
          DxLabel.CaptionColor.Disabled.Name := ReadGuiFontName(GuiLabel.CaptionColor.Disabled);

          if GuiLabel.CaptionLen > 0 then begin
            SetLength(sText, GuiLabel.CaptionLen);
            ReadMemory(sText[1], GuiLabel.CaptionLen);
            DxLabel.Caption := sText;
          end;
        end
        else begin
          ReadMemory(GuiLabel2, SizeOf(TGuiLabel_New));
          DxLabel := TDxLabel(DxControl);
          DxLabel.AutoSize := GuiLabel2.AutoSize;
          DxLabel.Alignment := GuiLabel2.Alignment;
          DxLabel.BackgroundColor := GuiLabel2.BackgroundColor;
          DxLabel.DrawBorder := GuiLabel2.DrawBorder;
          DxLabel.CaptionDownOffsetX := GuiLabel2.CaptionDownOffsetX;
          DxLabel.CaptionDownOffsetY := GuiLabel2.CaptionDownOffsetY;
          DxFontAssign(DxLabel.CaptionColor.Up, GuiLabel2.CaptionColor.Up);
          DxFontAssign(DxLabel.CaptionColor.Hot, GuiLabel2.CaptionColor.Hot);
          DxFontAssign(DxLabel.CaptionColor.Down, GuiLabel2.CaptionColor.Down);
          DxFontAssign(DxLabel.CaptionColor.Disabled, GuiLabel2.CaptionColor.Disabled);

          DxFontAssign(DxLabel.BorderColor.Up, GuiLabel2.BorderColor.Up);
          DxFontAssign(DxLabel.BorderColor.Hot, GuiLabel2.BorderColor.Hot);
          DxFontAssign(DxLabel.BorderColor.Down, GuiLabel2.BorderColor.Down);
          DxFontAssign(DxLabel.BorderColor.Disabled, GuiLabel2.BorderColor.Disabled);

          DxLabel.ClickCount := GuiLabel2.ClickCount;
          DxLabel.Style := GuiLabel2.Style;
          DxLabel.Caption := '';

          DxLabel.CaptionColor.Up.Name := ReadGuiFontName(GuiLabel2.CaptionColor.Up);
          DxLabel.CaptionColor.Hot.Name := ReadGuiFontName(GuiLabel2.CaptionColor.Hot);
          DxLabel.CaptionColor.Down.Name := ReadGuiFontName(GuiLabel2.CaptionColor.Down);
          DxLabel.CaptionColor.Disabled.Name := ReadGuiFontName(GuiLabel2.CaptionColor.Disabled);

          if GuiLabel2.CaptionLen > 0 then begin
            SetLength(sText, GuiLabel2.CaptionLen);
            ReadMemory(sText[1], GuiLabel2.CaptionLen);
            DxLabel.Caption := sText;
          end;
        end;
      end;

    t_Grid:begin
        ReadMemory(GuiImageGrid, SizeOf(TGuiImageGrid));
        DxImageGrid := TDxImageGrid(DxControl);
        // with DxImageGrid do begin
        DxImageGrid.ColCount := GuiImageGrid.ColCount;
        DxImageGrid.RowCount := GuiImageGrid.RowCount;
        DxImageGrid.ColWidth := GuiImageGrid.ColWidth;
        DxImageGrid.RowHeight := GuiImageGrid.RowHeight;
        DxImageGrid.ViewTopLine := GuiImageGrid.ViewTopLine;
        // end;
      end;
    t_ScrollBox, t_ChatMemo, t_ListView, t_TreeView:begin
        if GuiVersion < 20160508 then begin
          ReadMemory(GuiMemo, SizeOf(TGuiMemo));
          DxScrollControl := TDxScrollControl(DxControl);
          // with DxScrollControl do begin
          DxScrollControl.ShowScroll := GuiMemo.ShowScroll;
          DxScrollControl.ItemHeight := GuiMemo.ItemHeight;
          DxScrollControl.ItemIndex := GuiMemo.ItemIndex;
          DxScrollControl.ScrollBars := GuiMemo.ScrollBars;
          DxScrollControl.ScrollSize := GuiMemo.ScrollSize;

          DxScrollControl.ImageIndex.ImageType := GuiMemo.ImageIndex.Image;
          DxScrollControl.ImageIndex.Up := GuiMemo.ImageIndex.Up;
          DxScrollControl.ImageIndex.Hot := GuiMemo.ImageIndex.Hot;
          DxScrollControl.ImageIndex.Down := GuiMemo.ImageIndex.Down;
          DxScrollControl.ImageIndex.Disabled := GuiMemo.ImageIndex.Disabled;

          DxScrollControl.ScrollImageIndex.ImageType := GuiMemo.ScrollImageIndex.Image;
          DxScrollControl.ScrollImageIndex.Up := GuiMemo.ScrollImageIndex.Up;
          DxScrollControl.ScrollImageIndex.Hot := GuiMemo.ScrollImageIndex.Hot;
          DxScrollControl.ScrollImageIndex.Down := GuiMemo.ScrollImageIndex.Down;
          DxScrollControl.ScrollImageIndex.Disabled := GuiMemo.ScrollImageIndex.Disabled;

          DxScrollControl.PrevImageIndex.ImageType := GuiMemo.PrevImageIndex.Image;
          DxScrollControl.PrevImageIndex.Up := GuiMemo.PrevImageIndex.Up;
          DxScrollControl.PrevImageIndex.Hot := GuiMemo.PrevImageIndex.Hot;
          DxScrollControl.PrevImageIndex.Down := GuiMemo.PrevImageIndex.Down;
          DxScrollControl.PrevImageIndex.Disabled := GuiMemo.PrevImageIndex.Disabled;

          DxScrollControl.NextImageIndex.ImageType := GuiMemo.NextImageIndex.Image;
          DxScrollControl.NextImageIndex.Up := GuiMemo.NextImageIndex.Up;
          DxScrollControl.NextImageIndex.Hot := GuiMemo.NextImageIndex.Hot;
          DxScrollControl.NextImageIndex.Down := GuiMemo.NextImageIndex.Down;
          DxScrollControl.NextImageIndex.Disabled := GuiMemo.NextImageIndex.Disabled;

          DxScrollControl.BarImageIndex.ImageType := GuiMemo.BarImageIndex.Image;
          DxScrollControl.BarImageIndex.Up := GuiMemo.BarImageIndex.Up;
          DxScrollControl.BarImageIndex.Hot := GuiMemo.BarImageIndex.Hot;
          DxScrollControl.BarImageIndex.Down := GuiMemo.BarImageIndex.Down;
          DxScrollControl.BarImageIndex.Disabled := GuiMemo.BarImageIndex.Disabled;

          DxScrollControl.ExpandSize := GuiMemo.ExpandSize;
          DxScrollControl.Position := GuiMemo.Position;
          DxScrollControl.VisibleItemCount := GuiMemo.VisibleItemCount;
          DxScrollControl.OffSetX := GuiMemo.OffSetX;
          DxScrollControl.OffSetY := GuiMemo.OffSetY;

          DxScrollControl.ShowItemCount := GuiMemo.ShowItemCount;
          if DxScrollControl is TDxTreeView then
            TDxTreeView(DxScrollControl).ShowButton := GuiMemo.ShowButton;

          if DxScrollControl is TDxListView then begin
            DxListView := TDxListView(DxScrollControl);
            DxListView.ColCount := GuiMemo.ColCount;
            DxListView.ShowGridLine := GuiMemo.ShowGridLine;
            DxListView.GridLineColor := GuiMemo.GridLineColor;
            DxListView.CheckItemControlSize := GuiMemo.CheckItemControlSize;

            for I := 0 to DxListView.ColCount - 1 do begin
              ReadMemory(ColRect, SizeOf(TRect));
              DxListView.ColRects[I] := ColRect;
            end;

            for I := 0 to DxListView.ColCount - 1 do begin
              ReadMemory(GuiViewField, SizeOf(TGuiViewField));
              DxListView.Fields[I].Alignment := GuiViewField.Alignment;
              DxFontAssign(DxListView.Fields[I].Color.Up, GuiViewField.Color.Up);
              DxFontAssign(DxListView.Fields[I].Color.Hot, GuiViewField.Color.Hot);
              DxFontAssign(DxListView.Fields[I].Color.Down, GuiViewField.Color.Down);
              DxFontAssign(DxListView.Fields[I].Color.Disabled, GuiViewField.Color.Disabled);

              DxListView.Fields[I].Color.Up.Name := ReadGuiFontName(GuiViewField.Color.Up);
              DxListView.Fields[I].Color.Hot.Name := ReadGuiFontName(GuiViewField.Color.Hot);
              DxListView.Fields[I].Color.Down.Name := ReadGuiFontName(GuiViewField.Color.Down);
              DxListView.Fields[I].Color.Disabled.Name := ReadGuiFontName(GuiViewField.Color.Disabled);

              if GuiViewField.CaptionLen > 0 then begin
                SetLength(sText, GuiViewField.CaptionLen);
                ReadMemory(sText[1], GuiViewField.CaptionLen);
                DxListView.Fields[I].Caption := sText;
              end;
            end;
          end;
        end
        else begin
          ReadMemory(GuiMemo_new, SizeOf(TGuiMemo_new));

          DxScrollControl := TDxScrollControl(DxControl);
          DxScrollControl.ShowScroll := GuiMemo_new.ShowScroll;
          DxScrollControl.ItemHeight := GuiMemo_new.ItemHeight;
          DxScrollControl.ItemIndex := GuiMemo_new.ItemIndex;
          DxScrollControl.ScrollBars := GuiMemo_new.ScrollBars;
          DxScrollControl.ScrollSize := GuiMemo_new.ScrollSize;

          DxScrollControl.ImageIndex.ImageType := GuiMemo_new.ImageIndex.Image;
          DxScrollControl.ImageIndex.Up := GuiMemo_new.ImageIndex.Up;
          DxScrollControl.ImageIndex.Hot := GuiMemo_new.ImageIndex.Hot;
          DxScrollControl.ImageIndex.Down := GuiMemo_new.ImageIndex.Down;
          DxScrollControl.ImageIndex.Disabled := GuiMemo_new.ImageIndex.Disabled;

          DxScrollControl.ScrollImageIndex.ImageType := GuiMemo_new.ScrollImageIndex.Image;
          DxScrollControl.ScrollImageIndex.Up := GuiMemo_new.ScrollImageIndex.Up;
          DxScrollControl.ScrollImageIndex.Hot := GuiMemo_new.ScrollImageIndex.Hot;
          DxScrollControl.ScrollImageIndex.Down := GuiMemo_new.ScrollImageIndex.Down;
          DxScrollControl.ScrollImageIndex.Disabled := GuiMemo_new.ScrollImageIndex.Disabled;

          DxScrollControl.PrevImageIndex.ImageType := GuiMemo_new.PrevImageIndex.Image;
          DxScrollControl.PrevImageIndex.Up := GuiMemo_new.PrevImageIndex.Up;
          DxScrollControl.PrevImageIndex.Hot := GuiMemo_new.PrevImageIndex.Hot;
          DxScrollControl.PrevImageIndex.Down := GuiMemo_new.PrevImageIndex.Down;
          DxScrollControl.PrevImageIndex.Disabled := GuiMemo_new.PrevImageIndex.Disabled;

          DxScrollControl.NextImageIndex.ImageType := GuiMemo_new.NextImageIndex.Image;
          DxScrollControl.NextImageIndex.Up := GuiMemo_new.NextImageIndex.Up;
          DxScrollControl.NextImageIndex.Hot := GuiMemo_new.NextImageIndex.Hot;
          DxScrollControl.NextImageIndex.Down := GuiMemo_new.NextImageIndex.Down;
          DxScrollControl.NextImageIndex.Disabled := GuiMemo_new.NextImageIndex.Disabled;

          DxScrollControl.BarImageIndex.ImageType := GuiMemo_new.BarImageIndex.Image;
          DxScrollControl.BarImageIndex.Up := GuiMemo_new.BarImageIndex.Up;
          DxScrollControl.BarImageIndex.Hot := GuiMemo_new.BarImageIndex.Hot;
          DxScrollControl.BarImageIndex.Down := GuiMemo_new.BarImageIndex.Down;
          DxScrollControl.BarImageIndex.Disabled := GuiMemo_new.BarImageIndex.Disabled;

          DxScrollControl.ExpandSize := GuiMemo_new.ExpandSize;
          DxScrollControl.Position := GuiMemo_new.Position;
          DxScrollControl.VisibleItemCount := GuiMemo_new.VisibleItemCount;
          DxScrollControl.OffSetX := GuiMemo_new.OffSetX;
          DxScrollControl.OffSetY := GuiMemo_new.OffSetY;

          DxScrollControl.ShowItemCount := GuiMemo_new.ShowItemCount;

          DxScrollControl.BackgroundColor := GuiMemo_new.BackGroupColor;

          if DxScrollControl is TDxTreeView then
            TDxTreeView(DxScrollControl).ShowButton := GuiMemo_new.ShowButton;

          if GuiMemo_new.FontLen > 0 then begin
            SetLength(sText, GuiMemo_new.FontLen);
            ReadMemory(sText[1], GuiMemo_new.FontLen);
          end
          else
            sText := '';

          if DxScrollControl is TDxChatMemo then begin
            TDxChatMemo(DxScrollControl).FontName := sText;
            TDxChatMemo(DxScrollControl).FontSize := GuiMemo_new.FontSize;
            TDxChatMemo(DxScrollControl).FontStroke := GuiMemo_new.FontStroke;
            TDxChatMemo(DxScrollControl).FontBackTransparent := GuiMemo_new.FontBackTransparent;
          end;

          if DxScrollControl is TDxListView then begin
            DxListView := TDxListView(DxScrollControl);
            DxListView.ColCount := GuiMemo_new.ColCount;
            DxListView.ShowGridLine := GuiMemo_new.ShowGridLine;
            DxListView.GridLineColor := GuiMemo_new.GridLineColor;
            DxListView.CheckItemControlSize := GuiMemo_new.CheckItemControlSize;
            for I := 0 to DxListView.ColCount - 1 do begin
              ReadMemory(ColRect, SizeOf(TRect));
              DxListView.ColRects[I] := ColRect;
            end;

            for I := 0 to DxListView.ColCount - 1 do begin
              ReadMemory(GuiViewField, SizeOf(TGuiViewField));
              DxListView.Fields[I].Alignment := GuiViewField.Alignment;
              DxFontAssign(DxListView.Fields[I].Color.Up, GuiViewField.Color.Up);
              DxFontAssign(DxListView.Fields[I].Color.Hot, GuiViewField.Color.Hot);
              DxFontAssign(DxListView.Fields[I].Color.Down, GuiViewField.Color.Down);
              DxFontAssign(DxListView.Fields[I].Color.Disabled, GuiViewField.Color.Disabled);

              DxListView.Fields[I].Color.Up.Name := ReadGuiFontName(GuiViewField.Color.Up);
              DxListView.Fields[I].Color.Hot.Name := ReadGuiFontName(GuiViewField.Color.Hot);
              DxListView.Fields[I].Color.Down.Name := ReadGuiFontName(GuiViewField.Color.Down);
              DxListView.Fields[I].Color.Disabled.Name := ReadGuiFontName(GuiViewField.Color.Disabled);

              if GuiViewField.CaptionLen > 0 then begin
                SetLength(sText, GuiViewField.CaptionLen);
                ReadMemory(sText[1], GuiViewField.CaptionLen);
                DxListView.Fields[I].Caption := sText;
              end;
            end;
          end;
        end;
      end;

    t_PopupMenu:begin
        ReadMemory(GuiPopupMenu, SizeOf(TGuiPopupMenu));
        DxPopupMenu := TDxPopupMenu(DxControl);
        // with DxPopupMenu do begin
        DxPopupMenu.BackgroundColor := GuiPopupMenu.BackgroundColor;
        DxPopupMenu.DrawBorder := GuiPopupMenu.DrawBorder;

        DxFontAssign(DxPopupMenu.ItemColor.Up, GuiPopupMenu.ItemColor.Up);
        DxFontAssign(DxPopupMenu.ItemColor.Hot, GuiPopupMenu.ItemColor.Hot);
        DxFontAssign(DxPopupMenu.ItemColor.Down, GuiPopupMenu.ItemColor.Down);
        DxFontAssign(DxPopupMenu.ItemColor.Disabled, GuiPopupMenu.ItemColor.Disabled);

        DxFontAssign(DxPopupMenu.BorderColor.Up, GuiPopupMenu.BorderColor.Up);
        DxFontAssign(DxPopupMenu.BorderColor.Hot, GuiPopupMenu.BorderColor.Hot);
        DxFontAssign(DxPopupMenu.BorderColor.Down, GuiPopupMenu.BorderColor.Down);
        DxFontAssign(DxPopupMenu.BorderColor.Disabled, GuiPopupMenu.BorderColor.Disabled);

        DxPopupMenu.SelectColor := GuiPopupMenu.SelectColor;
        DxPopupMenu.ItemHeight := GuiPopupMenu.ItemHeight;
        DxPopupMenu.ItemIndex := GuiPopupMenu.ItemIndex;
        // end;

        DxPopupMenu.ItemColor.Up.Name := ReadGuiFontName(GuiPopupMenu.ItemColor.Up);
        DxPopupMenu.ItemColor.Hot.Name := ReadGuiFontName(GuiPopupMenu.ItemColor.Hot);
        DxPopupMenu.ItemColor.Down.Name := ReadGuiFontName(GuiPopupMenu.ItemColor.Down);
        DxPopupMenu.ItemColor.Disabled.Name := ReadGuiFontName(GuiPopupMenu.ItemColor.Disabled);

        if GuiPopupMenu.ItemTextLen > 0 then begin
          SetLength(sText, GuiPopupMenu.ItemTextLen);
          ReadMemory(sText[1], GuiPopupMenu.ItemTextLen);
          DxPopupMenu.Items.Text := sText;
        end;
      end;
    t_PageControl:begin
        if GuiVersion < 20160514 then begin
          ReadMemory(GuiPageControl, SizeOf(TGuiPageControl));
          DxPageControl := TDxPageControl(DxControl);
          DxPageControl.ClientLeft := GuiPageControl.ClientLeft;
          DxPageControl.ClientTop := GuiPageControl.ClientTop;
          DxPageControl.ClientWidth := GuiPageControl.ClientWidth;
          DxPageControl.ClientHeight := GuiPageControl.ClientHeight;
          DxPageControl.TabPosition := GuiPageControl.TabPosition;
          DxPageControl.ButtonWidth := GuiPageControl.ButtonWidth;
          DxPageControl.ButtonHeight := GuiPageControl.ButtonHeight;
          DxPageControl.ShowButton := GuiPageControl.ShowButton;
          DxPageControl.OffSetX := GuiPageControl.OffSetX;
          DxPageControl.OffSetY := GuiPageControl.OffSetY;
        end
        else begin
          ReadMemory(GuiPageControl_New, SizeOf(GuiPageControl_New));
          DxPageControl := TDxPageControl(DxControl);
          DxPageControl.ClientLeft := GuiPageControl_New.ClientLeft;
          DxPageControl.ClientTop := GuiPageControl_New.ClientTop;
          DxPageControl.ClientWidth := GuiPageControl_New.ClientWidth;
          DxPageControl.ClientHeight := GuiPageControl_New.ClientHeight;
          DxPageControl.TabPosition := GuiPageControl_New.TabPosition;
          DxPageControl.ButtonWidth := GuiPageControl_New.ButtonWidth;
          DxPageControl.ButtonHeight := GuiPageControl_New.ButtonHeight;
          DxPageControl.ShowButton := GuiPageControl_New.ShowButton;
          DxPageControl.OffSetX := GuiPageControl_New.OffSetX;
          DxPageControl.OffSetY := GuiPageControl_New.OffSetY;
          DxPageControl.CaptionOffsetX := GuiPageControl_New.CaptionOffsetX;
          DxPageControl.CaptionOffsetY := GuiPageControl_New.CaptionOffsetY;
          DxPageControl.DownCaptionOffsetX := GuiPageControl_New.DownCaptionOffsetX;
          DxPageControl.DownCaptionOffsetY := GuiPageControl_New.DownCaptionOffsetY;
          DxPageControl.ReverseDrawButton := GuiPageControl_New.ReverseDrawButton;
        end;
      end;
    t_ComboBox:begin
        ReadMemory(GuiComboBox, SizeOf(TGuiComboBox));
        DxComboBox := TDxComboBox(DxControl);

        DxPopupMenu := TDxPopupMenu(DxComboBox.PopupMenu);

        // with DxPopupMenu do begin
        sText := '';
        DxPopupMenu.BackgroundColor := GuiComboBox.GuiPopupMenu.BackgroundColor;
        DxPopupMenu.DrawBorder := GuiComboBox.GuiPopupMenu.DrawBorder;

        DxFontAssign(DxPopupMenu.ItemColor.Up, GuiComboBox.GuiPopupMenu.ItemColor.Up);
        DxFontAssign(DxPopupMenu.ItemColor.Hot, GuiComboBox.GuiPopupMenu.ItemColor.Hot);
        DxFontAssign(DxPopupMenu.ItemColor.Down, GuiComboBox.GuiPopupMenu.ItemColor.Down);
        DxFontAssign(DxPopupMenu.ItemColor.Disabled, GuiComboBox.GuiPopupMenu.ItemColor.Disabled);

        DxFontAssign(DxPopupMenu.BorderColor.Up, GuiComboBox.GuiPopupMenu.BorderColor.Up);
        DxFontAssign(DxPopupMenu.BorderColor.Hot, GuiComboBox.GuiPopupMenu.BorderColor.Hot);
        DxFontAssign(DxPopupMenu.BorderColor.Down, GuiComboBox.GuiPopupMenu.BorderColor.Down);
        DxFontAssign(DxPopupMenu.BorderColor.Disabled, GuiComboBox.GuiPopupMenu.BorderColor.Disabled);

        DxPopupMenu.SelectColor := GuiComboBox.GuiPopupMenu.SelectColor;
        DxPopupMenu.ItemHeight := GuiComboBox.GuiPopupMenu.ItemHeight;
        DxPopupMenu.ItemIndex := GuiComboBox.GuiPopupMenu.ItemIndex;
        // end;

        DxPopupMenu.ItemColor.Up.Name := ReadGuiFontName(GuiComboBox.GuiPopupMenu.ItemColor.Up);
        DxPopupMenu.ItemColor.Hot.Name := ReadGuiFontName(GuiComboBox.GuiPopupMenu.ItemColor.Hot);
        DxPopupMenu.ItemColor.Down.Name := ReadGuiFontName(GuiComboBox.GuiPopupMenu.ItemColor.Down);
        DxPopupMenu.ItemColor.Disabled.Name := ReadGuiFontName(GuiComboBox.GuiPopupMenu.ItemColor.Disabled);

        // with DxComboBox do begin
        DxComboBox.BackgroundColor := GuiComboBox.BackgroundColor;
        DxComboBox.DrawBorder := GuiComboBox.DrawBorder;
        DxComboBox.ButtonColor := GuiComboBox.ButtonColor;

        DxFontAssign(DxComboBox.TextColor.Up, GuiComboBox.TextColor.Up);
        DxFontAssign(DxComboBox.TextColor.Hot, GuiComboBox.TextColor.Hot);
        DxFontAssign(DxComboBox.TextColor.Down, GuiComboBox.TextColor.Down);
        DxFontAssign(DxComboBox.TextColor.Disabled, GuiComboBox.TextColor.Disabled);

        DxFontAssign(DxComboBox.BorderColor.Up, GuiComboBox.BorderColor.Up);
        DxFontAssign(DxComboBox.BorderColor.Hot, GuiComboBox.BorderColor.Hot);
        DxFontAssign(DxComboBox.BorderColor.Down, GuiComboBox.BorderColor.Down);
        DxFontAssign(DxComboBox.BorderColor.Disabled, GuiComboBox.BorderColor.Disabled);
        // end;

        DxComboBox.TextColor.Up.Name := ReadGuiFontName(GuiComboBox.TextColor.Up);
        DxComboBox.TextColor.Hot.Name := ReadGuiFontName(GuiComboBox.TextColor.Hot);
        DxComboBox.TextColor.Down.Name := ReadGuiFontName(GuiComboBox.TextColor.Down);
        DxComboBox.TextColor.Disabled.Name := ReadGuiFontName(GuiComboBox.TextColor.Disabled);

        if GuiComboBox.TextLen > 0 then begin
          SetLength(sText, GuiComboBox.TextLen);
          ReadMemory(sText[1], GuiComboBox.TextLen);
          DxComboBox.Text := sText;
        end;
        if GuiComboBox.ItemLen > 0 then begin
          SetLength(sText, GuiComboBox.ItemLen);
          ReadMemory(sText[1], GuiComboBox.ItemLen);
          DxComboBox.Items.Text := sText;
        end;
      end;
    t_TabSheet:begin
        ReadMemory(GuiTabSheet, SizeOf(TGuiTabSheet));
        DxTabSheet := TDxTabSheet(DxControl);
        DxTabSheet.OffSetX := GuiTabSheet.OffSetX;
        DxTabSheet.OffSetY := GuiTabSheet.OffSetY;
        DxTabSheet.TabVisible := not GuiTabSheet.HideTable;

        // with DxTabSheet do begin
        DxTabSheet.Caption := '';
        DxTabSheet.ImageIndex.ImageType := GuiTabSheet.ImageIndex.Image;
        DxTabSheet.ImageIndex.Up := GuiTabSheet.ImageIndex.Up;
        DxTabSheet.ImageIndex.Hot := GuiTabSheet.ImageIndex.Hot;
        DxTabSheet.ImageIndex.Down := GuiTabSheet.ImageIndex.Down;
        DxTabSheet.ImageIndex.Disabled := GuiTabSheet.ImageIndex.Disabled;

        // BackgroundColor := GuiTabSheet.BackgroundColor;
        DxFontAssign(DxTabSheet.CaptionColor.Up, GuiTabSheet.CaptionColor.Up);
        DxFontAssign(DxTabSheet.CaptionColor.Hot, GuiTabSheet.CaptionColor.Hot);
        DxFontAssign(DxTabSheet.CaptionColor.Down, GuiTabSheet.CaptionColor.Down);
        DxFontAssign(DxTabSheet.CaptionColor.Disabled, GuiTabSheet.CaptionColor.Disabled);
        // end;

        DxTabSheet.CaptionColor.Up.Name := ReadGuiFontName(GuiTabSheet.CaptionColor.Up);
        DxTabSheet.CaptionColor.Hot.Name := ReadGuiFontName(GuiTabSheet.CaptionColor.Hot);
        DxTabSheet.CaptionColor.Down.Name := ReadGuiFontName(GuiTabSheet.CaptionColor.Down);
        DxTabSheet.CaptionColor.Disabled.Name := ReadGuiFontName(GuiTabSheet.CaptionColor.Disabled);

        if GuiTabSheet.CaptionLen > 0 then begin
          SetLength(sText, GuiTabSheet.CaptionLen);
          ReadMemory(sText[1], GuiTabSheet.CaptionLen);
          DxTabSheet.Caption := sText;
        end;
        TDxPageControl(DxTabSheet.Owner).ActivePageIndex := 0;
      end;
    t_Line:begin
        ReadMemory(GuiLine, SizeOf(TGuiLine));
        DxLine := TDxLine(DxControl);
        DxLine.Style := GuiLine.LineStyle;
        DxFontAssign(DxLine.LineColor.Up, GuiLine.LineColor.Up);
        DxFontAssign(DxLine.LineColor.Hot, GuiLine.LineColor.Hot);
        DxFontAssign(DxLine.LineColor.Down, GuiLine.LineColor.Down);
        DxFontAssign(DxLine.LineColor.Disabled, GuiLine.LineColor.Disabled);
      end;
    t_TrackBar:begin
        ReadMemory(GuiTrackBar, SizeOf(GuiTrackBar));
        DxTrackBar := TDXTrackBar(DxControl);
        // with DxImageButton do begin
        DxTrackBar.AutoSize := GuiTrackBar.AutoSize;

        DxTrackBar.ImageIndex.ImageType := GuiTrackBar.ImageIndex.Image;
        DxTrackBar.ImageIndex.Up := GuiTrackBar.ImageIndex.Up;
        DxTrackBar.ImageIndex.Hot := GuiTrackBar.ImageIndex.Hot;
        DxTrackBar.ImageIndex.Down := GuiTrackBar.ImageIndex.Down;
        DxTrackBar.ImageIndex.Disabled := GuiTrackBar.ImageIndex.Disabled;

        DxTrackBar.SliderIndex.ImageType := GuiTrackBar.SliderIndex.Image;
        DxTrackBar.SliderIndex.Up := GuiTrackBar.SliderIndex.Up;
        DxTrackBar.SliderIndex.Hot := GuiTrackBar.SliderIndex.Hot;
        DxTrackBar.SliderIndex.Down := GuiTrackBar.SliderIndex.Down;
        DxTrackBar.SliderIndex.Disabled := GuiTrackBar.SliderIndex.Disabled;

        DxTrackBar.Min := GuiTrackBar.Min;
        DxTrackBar.Max := GuiTrackBar.Max;
        DxTrackBar.Position := GuiTrackBar.Position;
      end;
    t_MainBottomForm:begin
        if GuiVersion < 20190729 then begin
          ReadMemory(GuiMainBottomForm, SizeOf(GuiMainBottomForm));
          DxMainBottomForm := TDxMainBottomForm(DxControl);
          DxMainBottomForm.LeftImage.ImageType := GuiMainBottomForm.LeftImageType;
          DxMainBottomForm.LeftImage.Index := GuiMainBottomForm.LeftImageIndex;

          DxMainBottomForm.RightImage.ImageType := GuiMainBottomForm.RightImageType;
          DxMainBottomForm.RightImage.Index := GuiMainBottomForm.RightImageIndex;

          DxMainBottomForm.CenterSetting.AutoStretchSize := GuiMainBottomForm.Center.AutoStretchSize;
          DxMainBottomForm.CenterSetting.OffsetLeft := GuiMainBottomForm.Center.OffsetLeft;
          DxMainBottomForm.CenterSetting.OffsetRight := GuiMainBottomForm.Center.OffsetRight;

          DxMainBottomForm.CenterSetting.StretchImage.MinHeight := GuiMainBottomForm.Center.MinHeight;
          DxMainBottomForm.CenterSetting.StretchImage.MaxHeight := GuiMainBottomForm.Center.MaxHeight;
          DxMainBottomForm.CenterSetting.StretchImage.Height := GuiMainBottomForm.Center.Height;

          DxMainBottomForm.CenterSetting.StretchImage.DragHeightOffsetY := GuiMainBottomForm.Center.DragHeightOffsetY;
          DxMainBottomForm.CenterSetting.StretchImage.DragHeightSize := GuiMainBottomForm.Center.DragHeightSize;

          DxMainBottomForm.CenterSetting.StretchImage.FillCenterAlpha := GuiMainBottomForm.Center.StretchImageFillCenterAlpha;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterColor := GuiMainBottomForm.Center.StretchImageFillCenterColor;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterExpandHorz := GuiMainBottomForm.Center.StretchImageFillCenterExpandHorz;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterExpandVert := GuiMainBottomForm.Center.StretchImageFillCenterExpandVert;

          DxMainBottomForm.CenterSetting.StretchImage.ImageType := GuiMainBottomForm.Center.StretchImageType;
          DxMainBottomForm.CenterSetting.StretchImage.UpLeft := GuiMainBottomForm.Center.StretchImageUpLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Up := GuiMainBottomForm.Center.StretchImageUp;
          DxMainBottomForm.CenterSetting.StretchImage.UpRight := GuiMainBottomForm.Center.StretchImageUpRight;
          DxMainBottomForm.CenterSetting.StretchImage.Left := GuiMainBottomForm.Center.StretchImageLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Right := GuiMainBottomForm.Center.StretchImageRight;
          DxMainBottomForm.CenterSetting.StretchImage.DownLeft := GuiMainBottomForm.Center.StretchImageDownLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Down := GuiMainBottomForm.Center.StretchImageDown;
          DxMainBottomForm.CenterSetting.StretchImage.DownRight := GuiMainBottomForm.Center.StretchImageDownRight;

          DxMainBottomForm.CenterSetting.FillImage.ImageType := GuiMainBottomForm.Center.FillImageType;
          DxMainBottomForm.CenterSetting.FillImage.Index := GuiMainBottomForm.Center.FillImageIndex;
        end
        else if GuiVersion < 20211120 then begin
          ReadMemory(GuiMainBottomForm_New, SizeOf(GuiMainBottomForm_New));
          DxMainBottomForm := TDxMainBottomForm(DxControl);
          DxMainBottomForm.LeftImage.ImageType := GuiMainBottomForm_New.LeftImageType;
          DxMainBottomForm.LeftImage.Index := GuiMainBottomForm_New.LeftImageIndex;

          DxMainBottomForm.RightImage.ImageType := GuiMainBottomForm_New.RightImageType;
          DxMainBottomForm.RightImage.Index := GuiMainBottomForm_New.RightImageIndex;

          DxMainBottomForm.CenterSetting.AutoStretchSize := GuiMainBottomForm_New.Center.AutoStretchSize;
          DxMainBottomForm.CenterSetting.OffsetLeft := GuiMainBottomForm_New.Center.OffsetLeft;
          DxMainBottomForm.CenterSetting.OffsetRight := GuiMainBottomForm_New.Center.OffsetRight;

          DxMainBottomForm.CenterSetting.StretchImage.MinHeight := GuiMainBottomForm_New.Center.MinHeight;
          DxMainBottomForm.CenterSetting.StretchImage.MaxHeight := GuiMainBottomForm_New.Center.MaxHeight;
          DxMainBottomForm.CenterSetting.StretchImage.Height := GuiMainBottomForm_New.Center.Height;

          DxMainBottomForm.CenterSetting.StretchImage.DragHeightOffsetY := GuiMainBottomForm_New.Center.DragHeightOffsetY;
          DxMainBottomForm.CenterSetting.StretchImage.DragHeightSize := GuiMainBottomForm_New.Center.DragHeightSize;

          DxMainBottomForm.CenterSetting.StretchImage.FillCenterAlpha := GuiMainBottomForm_New.Center.StretchImageFillCenterAlpha;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterColor := GuiMainBottomForm_New.Center.StretchImageFillCenterColor;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterExpandHorz := GuiMainBottomForm_New.Center.StretchImageFillCenterExpandHorz;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterExpandVert := GuiMainBottomForm_New.Center.StretchImageFillCenterExpandVert;

          DxMainBottomForm.CenterSetting.StretchImage.ImageType := GuiMainBottomForm_New.Center.StretchImageType;
          DxMainBottomForm.CenterSetting.StretchImage.UpLeft := GuiMainBottomForm_New.Center.StretchImageUpLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Up := GuiMainBottomForm_New.Center.StretchImageUp;
          DxMainBottomForm.CenterSetting.StretchImage.UpRight := GuiMainBottomForm_New.Center.StretchImageUpRight;
          DxMainBottomForm.CenterSetting.StretchImage.Left := GuiMainBottomForm_New.Center.StretchImageLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Right := GuiMainBottomForm_New.Center.StretchImageRight;
          DxMainBottomForm.CenterSetting.StretchImage.DownLeft := GuiMainBottomForm_New.Center.StretchImageDownLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Down := GuiMainBottomForm_New.Center.StretchImageDown;
          DxMainBottomForm.CenterSetting.StretchImage.DownRight := GuiMainBottomForm_New.Center.StretchImageDownRight;

          DxMainBottomForm.CenterSetting.FillImage.ImageType := GuiMainBottomForm_New.Center.FillImageType;
          DxMainBottomForm.CenterSetting.FillImage.Index := GuiMainBottomForm_New.Center.FillImageIndex;

          DxMainBottomForm.Animation1.ImageType := GuiMainBottomForm_New.Animation1.ImageType;
          DxMainBottomForm.Animation1.StartIndex := GuiMainBottomForm_New.Animation1.StartIndex;
          DxMainBottomForm.Animation1.EndIndex := GuiMainBottomForm_New.Animation1.EndIndex;
          DxMainBottomForm.Animation1.FrameTime := GuiMainBottomForm_New.Animation1.FrameTime;
          DxMainBottomForm.Animation1.PlayCount := GuiMainBottomForm_New.Animation1.PlayCount;
          DxMainBottomForm.Animation1.OffsetX := GuiMainBottomForm_New.Animation1.OffsetX;
          DxMainBottomForm.Animation1.OffsetY := GuiMainBottomForm_New.Animation1.OffsetY;
          DxMainBottomForm.Animation1.UseImageOffset := GuiMainBottomForm_New.Animation1.UseImageOffset;
          DxMainBottomForm.Animation1.OutsideAreaDraw := GuiMainBottomForm_New.Animation1.OutsideAreaDraw;
          DxMainBottomForm.Animation1.Draw := GuiMainBottomForm_New.Animation1.Draw;
          DxMainBottomForm.Animation1.BlendDraw := GuiMainBottomForm_New.Animation1.BlendDraw;
          DxMainBottomForm.Animation1.DrawBeforeDef := GuiMainBottomForm_New.Animation1.DrawBeforeDef;
          DxMainBottomForm.Animation1.HorzAlignment := GuiMainBottomForm_New.Animation1.HorzAlignment;
          DxMainBottomForm.Animation1.VertAlignment := GuiMainBottomForm_New.Animation1.VertAlignment;
          DxMainBottomForm.Animation1.AdjustYByHeight := GuiMainBottomForm_New.Animation1.AdjustYByHeight;

          DxMainBottomForm.Animation2.ImageType := GuiMainBottomForm_New.Animation2.ImageType;
          DxMainBottomForm.Animation2.StartIndex := GuiMainBottomForm_New.Animation2.StartIndex;
          DxMainBottomForm.Animation2.EndIndex := GuiMainBottomForm_New.Animation2.EndIndex;
          DxMainBottomForm.Animation2.FrameTime := GuiMainBottomForm_New.Animation2.FrameTime;
          DxMainBottomForm.Animation2.PlayCount := GuiMainBottomForm_New.Animation2.PlayCount;
          DxMainBottomForm.Animation2.OffsetX := GuiMainBottomForm_New.Animation2.OffsetX;
          DxMainBottomForm.Animation2.OffsetY := GuiMainBottomForm_New.Animation2.OffsetY;
          DxMainBottomForm.Animation2.UseImageOffset := GuiMainBottomForm_New.Animation2.UseImageOffset;
          DxMainBottomForm.Animation2.OutsideAreaDraw := GuiMainBottomForm_New.Animation2.OutsideAreaDraw;
          DxMainBottomForm.Animation2.Draw := GuiMainBottomForm_New.Animation2.Draw;
          DxMainBottomForm.Animation2.BlendDraw := GuiMainBottomForm_New.Animation2.BlendDraw;
          DxMainBottomForm.Animation2.DrawBeforeDef := GuiMainBottomForm_New.Animation2.DrawBeforeDef;
          DxMainBottomForm.Animation2.HorzAlignment := GuiMainBottomForm_New.Animation2.HorzAlignment;
          DxMainBottomForm.Animation2.VertAlignment := GuiMainBottomForm_New.Animation2.VertAlignment;
          DxMainBottomForm.Animation2.AdjustYByHeight := GuiMainBottomForm_New.Animation2.AdjustYByHeight;

          DxMainBottomForm.Animation3.ImageType := GuiMainBottomForm_New.Animation3.ImageType;
          DxMainBottomForm.Animation3.StartIndex := GuiMainBottomForm_New.Animation3.StartIndex;
          DxMainBottomForm.Animation3.EndIndex := GuiMainBottomForm_New.Animation3.EndIndex;
          DxMainBottomForm.Animation3.FrameTime := GuiMainBottomForm_New.Animation3.FrameTime;
          DxMainBottomForm.Animation3.PlayCount := GuiMainBottomForm_New.Animation3.PlayCount;
          DxMainBottomForm.Animation3.OffsetX := GuiMainBottomForm_New.Animation3.OffsetX;
          DxMainBottomForm.Animation3.OffsetY := GuiMainBottomForm_New.Animation3.OffsetY;
          DxMainBottomForm.Animation3.UseImageOffset := GuiMainBottomForm_New.Animation3.UseImageOffset;
          DxMainBottomForm.Animation3.OutsideAreaDraw := GuiMainBottomForm_New.Animation3.OutsideAreaDraw;
          DxMainBottomForm.Animation3.Draw := GuiMainBottomForm_New.Animation3.Draw;
          DxMainBottomForm.Animation3.BlendDraw := GuiMainBottomForm_New.Animation3.BlendDraw;
          DxMainBottomForm.Animation3.DrawBeforeDef := GuiMainBottomForm_New.Animation3.DrawBeforeDef;
          DxMainBottomForm.Animation3.HorzAlignment := GuiMainBottomForm_New.Animation3.HorzAlignment;
          DxMainBottomForm.Animation3.VertAlignment := GuiMainBottomForm_New.Animation3.VertAlignment;
          DxMainBottomForm.Animation3.AdjustYByHeight := GuiMainBottomForm_New.Animation3.AdjustYByHeight;

          DxMainBottomForm.Animation4.ImageType := GuiMainBottomForm_New.Animation4.ImageType;
          DxMainBottomForm.Animation4.StartIndex := GuiMainBottomForm_New.Animation4.StartIndex;
          DxMainBottomForm.Animation4.EndIndex := GuiMainBottomForm_New.Animation4.EndIndex;
          DxMainBottomForm.Animation4.FrameTime := GuiMainBottomForm_New.Animation4.FrameTime;
          DxMainBottomForm.Animation4.PlayCount := GuiMainBottomForm_New.Animation4.PlayCount;
          DxMainBottomForm.Animation4.OffsetX := GuiMainBottomForm_New.Animation4.OffsetX;
          DxMainBottomForm.Animation4.OffsetY := GuiMainBottomForm_New.Animation4.OffsetY;
          DxMainBottomForm.Animation4.UseImageOffset := GuiMainBottomForm_New.Animation4.UseImageOffset;
          DxMainBottomForm.Animation4.OutsideAreaDraw := GuiMainBottomForm_New.Animation4.OutsideAreaDraw;
          DxMainBottomForm.Animation4.Draw := GuiMainBottomForm_New.Animation4.Draw;
          DxMainBottomForm.Animation4.BlendDraw := GuiMainBottomForm_New.Animation4.BlendDraw;
          DxMainBottomForm.Animation4.DrawBeforeDef := GuiMainBottomForm_New.Animation4.DrawBeforeDef;
          DxMainBottomForm.Animation4.HorzAlignment := GuiMainBottomForm_New.Animation4.HorzAlignment;
          DxMainBottomForm.Animation4.VertAlignment := GuiMainBottomForm_New.Animation4.VertAlignment;
          DxMainBottomForm.Animation4.AdjustYByHeight := GuiMainBottomForm_New.Animation4.AdjustYByHeight;
        end
        else begin
          ReadMemory(GuiMainBottomForm_New2, SizeOf(GuiMainBottomForm_New2));
          DxMainBottomForm := TDxMainBottomForm(DxControl);
          DxMainBottomForm.LeftImage.ImageType := GuiMainBottomForm_New2.LeftImageType;
          DxMainBottomForm.LeftImage.Index := GuiMainBottomForm_New2.LeftImageIndex;

          DxMainBottomForm.RightImage.ImageType := GuiMainBottomForm_New2.RightImageType;
          DxMainBottomForm.RightImage.Index := GuiMainBottomForm_New2.RightImageIndex;

          DxMainBottomForm.BottomImage.ImageType := GuiMainBottomForm_New2.BottomImageType;
          DxMainBottomForm.BottomImage.Index := GuiMainBottomForm_New2.BottomImageIndex;

          DxMainBottomForm.CenterSetting.AutoStretchSize := GuiMainBottomForm_New2.Center.AutoStretchSize;
          DxMainBottomForm.CenterSetting.OffsetLeft := GuiMainBottomForm_New2.Center.OffsetLeft;
          DxMainBottomForm.CenterSetting.OffsetRight := GuiMainBottomForm_New2.Center.OffsetRight;

          DxMainBottomForm.CenterSetting.StretchImage.MinHeight := GuiMainBottomForm_New2.Center.MinHeight;
          DxMainBottomForm.CenterSetting.StretchImage.MaxHeight := GuiMainBottomForm_New2.Center.MaxHeight;
          DxMainBottomForm.CenterSetting.StretchImage.Height := GuiMainBottomForm_New2.Center.Height;

          DxMainBottomForm.CenterSetting.StretchImage.DragHeightOffsetY := GuiMainBottomForm_New2.Center.DragHeightOffsetY;
          DxMainBottomForm.CenterSetting.StretchImage.DragHeightSize := GuiMainBottomForm_New2.Center.DragHeightSize;

          DxMainBottomForm.CenterSetting.StretchImage.FillCenterAlpha := GuiMainBottomForm_New2.Center.StretchImageFillCenterAlpha;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterColor := GuiMainBottomForm_New2.Center.StretchImageFillCenterColor;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterExpandHorz := GuiMainBottomForm_New2.Center.StretchImageFillCenterExpandHorz;
          DxMainBottomForm.CenterSetting.StretchImage.FillCenterExpandVert := GuiMainBottomForm_New2.Center.StretchImageFillCenterExpandVert;

          DxMainBottomForm.CenterSetting.StretchImage.ImageType := GuiMainBottomForm_New2.Center.StretchImageType;
          DxMainBottomForm.CenterSetting.StretchImage.UpLeft := GuiMainBottomForm_New2.Center.StretchImageUpLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Up := GuiMainBottomForm_New2.Center.StretchImageUp;
          DxMainBottomForm.CenterSetting.StretchImage.UpRight := GuiMainBottomForm_New2.Center.StretchImageUpRight;
          DxMainBottomForm.CenterSetting.StretchImage.Left := GuiMainBottomForm_New2.Center.StretchImageLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Right := GuiMainBottomForm_New2.Center.StretchImageRight;
          DxMainBottomForm.CenterSetting.StretchImage.DownLeft := GuiMainBottomForm_New2.Center.StretchImageDownLeft;
          DxMainBottomForm.CenterSetting.StretchImage.Down := GuiMainBottomForm_New2.Center.StretchImageDown;
          DxMainBottomForm.CenterSetting.StretchImage.DownRight := GuiMainBottomForm_New2.Center.StretchImageDownRight;

          DxMainBottomForm.CenterSetting.FillImage.ImageType := GuiMainBottomForm_New2.Center.FillImageType;
          DxMainBottomForm.CenterSetting.FillImage.Index := GuiMainBottomForm_New2.Center.FillImageIndex;

          DxMainBottomForm.Animation1.ImageType := GuiMainBottomForm_New2.Animation1.ImageType;
          DxMainBottomForm.Animation1.StartIndex := GuiMainBottomForm_New2.Animation1.StartIndex;
          DxMainBottomForm.Animation1.EndIndex := GuiMainBottomForm_New2.Animation1.EndIndex;
          DxMainBottomForm.Animation1.FrameTime := GuiMainBottomForm_New2.Animation1.FrameTime;
          DxMainBottomForm.Animation1.PlayCount := GuiMainBottomForm_New2.Animation1.PlayCount;
          DxMainBottomForm.Animation1.OffsetX := GuiMainBottomForm_New2.Animation1.OffsetX;
          DxMainBottomForm.Animation1.OffsetY := GuiMainBottomForm_New2.Animation1.OffsetY;
          DxMainBottomForm.Animation1.UseImageOffset := GuiMainBottomForm_New2.Animation1.UseImageOffset;
          DxMainBottomForm.Animation1.OutsideAreaDraw := GuiMainBottomForm_New2.Animation1.OutsideAreaDraw;
          DxMainBottomForm.Animation1.Draw := GuiMainBottomForm_New2.Animation1.Draw;
          DxMainBottomForm.Animation1.BlendDraw := GuiMainBottomForm_New2.Animation1.BlendDraw;
          DxMainBottomForm.Animation1.DrawBeforeDef := GuiMainBottomForm_New2.Animation1.DrawBeforeDef;
          DxMainBottomForm.Animation1.HorzAlignment := GuiMainBottomForm_New2.Animation1.HorzAlignment;
          DxMainBottomForm.Animation1.VertAlignment := GuiMainBottomForm_New2.Animation1.VertAlignment;
          DxMainBottomForm.Animation1.AdjustYByHeight := GuiMainBottomForm_New2.Animation1.AdjustYByHeight;

          DxMainBottomForm.Animation2.ImageType := GuiMainBottomForm_New2.Animation2.ImageType;
          DxMainBottomForm.Animation2.StartIndex := GuiMainBottomForm_New2.Animation2.StartIndex;
          DxMainBottomForm.Animation2.EndIndex := GuiMainBottomForm_New2.Animation2.EndIndex;
          DxMainBottomForm.Animation2.FrameTime := GuiMainBottomForm_New2.Animation2.FrameTime;
          DxMainBottomForm.Animation2.PlayCount := GuiMainBottomForm_New2.Animation2.PlayCount;
          DxMainBottomForm.Animation2.OffsetX := GuiMainBottomForm_New2.Animation2.OffsetX;
          DxMainBottomForm.Animation2.OffsetY := GuiMainBottomForm_New2.Animation2.OffsetY;
          DxMainBottomForm.Animation2.UseImageOffset := GuiMainBottomForm_New2.Animation2.UseImageOffset;
          DxMainBottomForm.Animation2.OutsideAreaDraw := GuiMainBottomForm_New2.Animation2.OutsideAreaDraw;
          DxMainBottomForm.Animation2.Draw := GuiMainBottomForm_New2.Animation2.Draw;
          DxMainBottomForm.Animation2.BlendDraw := GuiMainBottomForm_New2.Animation2.BlendDraw;
          DxMainBottomForm.Animation2.DrawBeforeDef := GuiMainBottomForm_New2.Animation2.DrawBeforeDef;
          DxMainBottomForm.Animation2.HorzAlignment := GuiMainBottomForm_New2.Animation2.HorzAlignment;
          DxMainBottomForm.Animation2.VertAlignment := GuiMainBottomForm_New2.Animation2.VertAlignment;
          DxMainBottomForm.Animation2.AdjustYByHeight := GuiMainBottomForm_New2.Animation2.AdjustYByHeight;

          DxMainBottomForm.Animation3.ImageType := GuiMainBottomForm_New2.Animation3.ImageType;
          DxMainBottomForm.Animation3.StartIndex := GuiMainBottomForm_New2.Animation3.StartIndex;
          DxMainBottomForm.Animation3.EndIndex := GuiMainBottomForm_New2.Animation3.EndIndex;
          DxMainBottomForm.Animation3.FrameTime := GuiMainBottomForm_New2.Animation3.FrameTime;
          DxMainBottomForm.Animation3.PlayCount := GuiMainBottomForm_New2.Animation3.PlayCount;
          DxMainBottomForm.Animation3.OffsetX := GuiMainBottomForm_New2.Animation3.OffsetX;
          DxMainBottomForm.Animation3.OffsetY := GuiMainBottomForm_New2.Animation3.OffsetY;
          DxMainBottomForm.Animation3.UseImageOffset := GuiMainBottomForm_New2.Animation3.UseImageOffset;
          DxMainBottomForm.Animation3.OutsideAreaDraw := GuiMainBottomForm_New2.Animation3.OutsideAreaDraw;
          DxMainBottomForm.Animation3.Draw := GuiMainBottomForm_New2.Animation3.Draw;
          DxMainBottomForm.Animation3.BlendDraw := GuiMainBottomForm_New2.Animation3.BlendDraw;
          DxMainBottomForm.Animation3.DrawBeforeDef := GuiMainBottomForm_New2.Animation3.DrawBeforeDef;
          DxMainBottomForm.Animation3.HorzAlignment := GuiMainBottomForm_New2.Animation3.HorzAlignment;
          DxMainBottomForm.Animation3.VertAlignment := GuiMainBottomForm_New2.Animation3.VertAlignment;
          DxMainBottomForm.Animation3.AdjustYByHeight := GuiMainBottomForm_New2.Animation3.AdjustYByHeight;

          DxMainBottomForm.Animation4.ImageType := GuiMainBottomForm_New2.Animation4.ImageType;
          DxMainBottomForm.Animation4.StartIndex := GuiMainBottomForm_New2.Animation4.StartIndex;
          DxMainBottomForm.Animation4.EndIndex := GuiMainBottomForm_New2.Animation4.EndIndex;
          DxMainBottomForm.Animation4.FrameTime := GuiMainBottomForm_New2.Animation4.FrameTime;
          DxMainBottomForm.Animation4.PlayCount := GuiMainBottomForm_New2.Animation4.PlayCount;
          DxMainBottomForm.Animation4.OffsetX := GuiMainBottomForm_New2.Animation4.OffsetX;
          DxMainBottomForm.Animation4.OffsetY := GuiMainBottomForm_New2.Animation4.OffsetY;
          DxMainBottomForm.Animation4.UseImageOffset := GuiMainBottomForm_New2.Animation4.UseImageOffset;
          DxMainBottomForm.Animation4.OutsideAreaDraw := GuiMainBottomForm_New2.Animation4.OutsideAreaDraw;
          DxMainBottomForm.Animation4.Draw := GuiMainBottomForm_New2.Animation4.Draw;
          DxMainBottomForm.Animation4.BlendDraw := GuiMainBottomForm_New2.Animation4.BlendDraw;
          DxMainBottomForm.Animation4.DrawBeforeDef := GuiMainBottomForm_New2.Animation4.DrawBeforeDef;
          DxMainBottomForm.Animation4.HorzAlignment := GuiMainBottomForm_New2.Animation4.HorzAlignment;
          DxMainBottomForm.Animation4.VertAlignment := GuiMainBottomForm_New2.Animation4.VertAlignment;
          DxMainBottomForm.Animation4.AdjustYByHeight := GuiMainBottomForm_New2.Animation4.AdjustYByHeight;
        end;
      end;
    t_MagicBall:begin
        if GuiVersion < 20160818 then begin
          ReadMemory(GuiMagicBall, SizeOf(GuiMagicBall));
          DxMagicBall := TDxMagicBall(DxControl);

          DxMagicBall.BallType := GuiMagicBall.BallType;
          DxMagicBall.ValueAlignment := GuiMagicBall.ValueAlignment;

          DxMagicBall.OverallSetting.ImageType := GuiMagicBall.Overall_ImageType;

          DxMagicBall.OverallSetting.EmptyHPMP := GuiMagicBall.Overall_EmptyHPMP;
          DxMagicBall.OverallSetting.FullHPMP := GuiMagicBall.Overall_FullHPMP;

          DxMagicBall.OverallSetting.EmptyHP := GuiMagicBall.Overall_EmptyHP;
          DxMagicBall.OverallSetting.FullHP := GuiMagicBall.Overall_FullHP;

          DxMagicBall.OverallSetting.Splite := GuiMagicBall.Overall_Splite;
          DxMagicBall.OverallSetting.MiddleZoneWidth := GuiMagicBall.Overall_MiddleZoneWidth;

          DxMagicBall.AloneSetting.ImageType := GuiMagicBall.Alone_ImageType;
          DxMagicBall.AloneSetting.Empty := GuiMagicBall.Alone_Empty;
          DxMagicBall.AloneSetting.Full := GuiMagicBall.Alone_Full;
        end
        else begin
          ReadMemory(GuiMagicBall2, SizeOf(GuiMagicBall2));
          DxMagicBall := TDxMagicBall(DxControl);
          DxMagicBall.BallType := GuiMagicBall2.BallType;
          DxMagicBall.ValueAlignment := GuiMagicBall2.ValueAlignment;

          DxMagicBall.OverallSetting.ImageType := GuiMagicBall2.Overall_ImageType;

          DxMagicBall.OverallSetting.EmptyHPMP := GuiMagicBall2.Overall_EmptyHPMP;
          DxMagicBall.OverallSetting.FullHPMP := GuiMagicBall2.Overall_FullHPMP;

          DxMagicBall.OverallSetting.EmptyHP := GuiMagicBall2.Overall_EmptyHP;
          DxMagicBall.OverallSetting.FullHP := GuiMagicBall2.Overall_FullHP;

          DxMagicBall.OverallSetting.Splite := GuiMagicBall2.Overall_Splite;
          DxMagicBall.OverallSetting.MiddleZoneWidth := GuiMagicBall2.Overall_MiddleZoneWidth;

          DxMagicBall.OverallSetting.EffectDrawBlend := GuiMagicBall2.Overall_EffectDrawBlend;
          DxMagicBall.OverallSetting.EffectImageType := GuiMagicBall2.Overall_EffectImageType;
          DxMagicBall.OverallSetting.EffectHPMPStart := GuiMagicBall2.Overall_EffectHPMPStart;
          DxMagicBall.OverallSetting.EffectHPStart := GuiMagicBall2.Overall_EffectHPStart;
          DxMagicBall.OverallSetting.EffectImageCount := GuiMagicBall2.Overall_EffectImageCount;
          DxMagicBall.OverallSetting.EffectPlayInterval := GuiMagicBall2.Overall_EffectPlayInterval;

          DxMagicBall.AloneSetting.ImageType := GuiMagicBall2.Alone_ImageType;
          DxMagicBall.AloneSetting.Empty := GuiMagicBall2.Alone_Empty;
          DxMagicBall.AloneSetting.Full := GuiMagicBall2.Alone_Full;

          DxMagicBall.AloneSetting.EffectDrawBlend := GuiMagicBall2.Alone_EffectDrawBlend;
          DxMagicBall.AloneSetting.EffectImageType := GuiMagicBall2.Alone_EffectImageType;
          DxMagicBall.AloneSetting.EffectStart := GuiMagicBall2.Alone_EffectStart;
          DxMagicBall.AloneSetting.EffectImageCount := GuiMagicBall2.Alone_EffectImageCount;
          DxMagicBall.AloneSetting.EffectPlayInterval := GuiMagicBall2.Alone_EffectPlayInterval;
        end;
      end;

    t_SexPanel:begin
        ReadMemory(GuiSexPanel, SizeOf(GuiSexPanel));
        DxSexPanel := TDxSexPanel(DxControl);

        DxSexPanel.IsMale := GuiSexPanel.IsMale;
        DxSexPanel.UseSetting2 := GuiSexPanel.UseSettign2;

        DxSexPanel.SexImageSetting.ImageType := GuiSexPanel.ImageType;
        DxSexPanel.SexImageSetting.Male := GuiSexPanel.Male;
        DxSexPanel.SexImageSetting.Female := GuiSexPanel.Female;

        DxSexPanel.SexImageSetting2.ImageType := GuiSexPanel.ImageType2;
        DxSexPanel.SexImageSetting2.Male := GuiSexPanel.Male2;
        DxSexPanel.SexImageSetting2.Female := GuiSexPanel.Female2;
      end;
    t_GroupAttackProgress:begin
        ReadMemory(GuiGroupAttackProgress, SizeOf(GuiGroupAttackProgress));
        DxGroupAttackProgress := TDxGroupAttackProgress(DxControl);

        DxGroupAttackProgress.ProgressAlignment := GuiGroupAttackProgress.ProgressAlignment;

        DxGroupAttackProgress.ContinueSetting.ImageType := GuiGroupAttackProgress.Settings[0].ImageType;
        DxGroupAttackProgress.ContinueSetting.Background := GuiGroupAttackProgress.Settings[0].Background;
        DxGroupAttackProgress.ContinueSetting.FlashStart := GuiGroupAttackProgress.Settings[0].FlashStart;
        DxGroupAttackProgress.ContinueSetting.FlashEnd := GuiGroupAttackProgress.Settings[0].FlashEnd;
        DxGroupAttackProgress.ContinueSetting.FlashInterval := GuiGroupAttackProgress.Settings[0].FlashInterval;
        DxGroupAttackProgress.ContinueSetting.BgOffsetX := GuiGroupAttackProgress.Settings[0].OffsetX1;
        DxGroupAttackProgress.ContinueSetting.BgOffsetY := GuiGroupAttackProgress.Settings[0].OffsetY1;
        DxGroupAttackProgress.ContinueSetting.FlashOffsetX := GuiGroupAttackProgress.Settings[0].OffsetX2;
        DxGroupAttackProgress.ContinueSetting.FlashOffsetY := GuiGroupAttackProgress.Settings[0].OffsetY2;

        DxGroupAttackProgress.GroupSetting.ImageType := GuiGroupAttackProgress.Settings[1].ImageType;
        DxGroupAttackProgress.GroupSetting.Background := GuiGroupAttackProgress.Settings[1].Background;
        DxGroupAttackProgress.GroupSetting.Progress := GuiGroupAttackProgress.Settings[1].Progress;
        DxGroupAttackProgress.GroupSetting.FlashStart := GuiGroupAttackProgress.Settings[1].FlashStart;
        DxGroupAttackProgress.GroupSetting.FlashEnd := GuiGroupAttackProgress.Settings[1].FlashEnd;
        DxGroupAttackProgress.GroupSetting.FlashInterval := GuiGroupAttackProgress.Settings[1].FlashInterval;
        DxGroupAttackProgress.GroupSetting.BgOffsetX := GuiGroupAttackProgress.Settings[1].OffsetX1;
        DxGroupAttackProgress.GroupSetting.BgOffsetY := GuiGroupAttackProgress.Settings[1].OffsetY1;
        DxGroupAttackProgress.GroupSetting.PgOffsetX := GuiGroupAttackProgress.Settings[1].OffsetX2;
        DxGroupAttackProgress.GroupSetting.PgOffsetY := GuiGroupAttackProgress.Settings[1].OffsetY2;
        DxGroupAttackProgress.GroupSetting.ContinueOffsetX := GuiGroupAttackProgress.Settings[1].OffsetX3;
        DxGroupAttackProgress.GroupSetting.ContinueOffsetY := GuiGroupAttackProgress.Settings[1].OffsetY3;

        DxGroupAttackProgress.ContinueAndGroupSetting.ImageType := GuiGroupAttackProgress.Settings[2].ImageType;
        DxGroupAttackProgress.ContinueAndGroupSetting.Background := GuiGroupAttackProgress.Settings[2].Background;
        DxGroupAttackProgress.ContinueAndGroupSetting.Progress := GuiGroupAttackProgress.Settings[2].Progress;
        DxGroupAttackProgress.ContinueAndGroupSetting.FlashStart := GuiGroupAttackProgress.Settings[2].FlashStart;
        DxGroupAttackProgress.ContinueAndGroupSetting.FlashEnd := GuiGroupAttackProgress.Settings[2].FlashEnd;
        DxGroupAttackProgress.ContinueAndGroupSetting.FlashInterval := GuiGroupAttackProgress.Settings[2].FlashInterval;
        DxGroupAttackProgress.ContinueAndGroupSetting.BgOffsetX := GuiGroupAttackProgress.Settings[2].OffsetX1;
        DxGroupAttackProgress.ContinueAndGroupSetting.BgOffsetY := GuiGroupAttackProgress.Settings[2].OffsetY1;
        DxGroupAttackProgress.ContinueAndGroupSetting.PgOffsetX := GuiGroupAttackProgress.Settings[2].OffsetX2;
        DxGroupAttackProgress.ContinueAndGroupSetting.PgOffsetY := GuiGroupAttackProgress.Settings[2].OffsetY2;
        DxGroupAttackProgress.ContinueAndGroupSetting.ContinueOffsetX := GuiGroupAttackProgress.Settings[2].OffsetX3;
        DxGroupAttackProgress.ContinueAndGroupSetting.ContinueOffsetY := GuiGroupAttackProgress.Settings[2].OffsetY3;
      end;
    t_ImageProgress:begin
        ReadMemory(GuiImageProgress, SizeOf(GuiImageProgress));

        DxImageProgress := TDxImageProgress(DxControl);
        DxImageProgress.AutoSize := GuiImageProgress.AutoSize;

        DxImageProgress.ProgressSetting.ImageType := GuiImageProgress.ImageType;

        DxImageProgress.ProgressSetting.ImageBG := GuiImageProgress.ImageBG;
        DxImageProgress.ProgressSetting.ImageProgress := GuiImageProgress.ImageProgress;
        DxImageProgress.ProgressSetting.ImageProgressX := GuiImageProgress.ImageProgressX;
        DxImageProgress.ProgressSetting.ImageProgressY := GuiImageProgress.ImageProgressY;
        DxImageProgress.ProgressSetting.ValueType := GuiImageProgress.ValueType;
        DxImageProgress.ProgressSetting.ValueSplite := GuiImageProgress.ValueSplite;
        DxImageProgress.ProgressSetting.ValueAlignment := GuiImageProgress.ValueAlignment;
        DxImageProgress.ProgressSetting.ValuePrefix := GuiImageProgress.ValuePrefix;
        DxImageProgress.ProgressSetting.ValueSuffix := GuiImageProgress.ValueSuffix;
        DxImageProgress.ProgressSetting.Max := GuiImageProgress.Max;
        DxImageProgress.ProgressSetting.Min := GuiImageProgress.Min;
        DxImageProgress.ProgressSetting.Value := GuiImageProgress.Value;

        DxFontAssign(DxImageProgress.ProgressSetting.Font, GuiImageProgress.Font);
        DxImageProgress.ProgressSetting.Font.Name := ReadGuiFontName(GuiImageProgress.Font);
      end;

    t_SwitchButton:begin
        ReadMemory(GuiSwitchButton, SizeOf(GuiSwitchButton));
        DxSwitchButton := TDxSwitchButton(DxControl);

        DxSwitchButton.AutoSize := GuiSwitchButton.AutoSize;

        DxSwitchButton.CloseSetting.ImageIndex.ImageType := GuiSwitchButton.CloseSetting.ImageIndex.Image;
        DxSwitchButton.CloseSetting.ImageIndex.Up := GuiSwitchButton.CloseSetting.ImageIndex.Up;
        DxSwitchButton.CloseSetting.ImageIndex.Hot := GuiSwitchButton.CloseSetting.ImageIndex.Hot;
        DxSwitchButton.CloseSetting.ImageIndex.Down := GuiSwitchButton.CloseSetting.ImageIndex.Down;
        DxSwitchButton.CloseSetting.ImageIndex.Disabled := GuiSwitchButton.CloseSetting.ImageIndex.Disabled;

        DxFontAssign(DxSwitchButton.CloseSetting.CaptionColor.Up, GuiSwitchButton.CloseSetting.CaptionColor.Up);
        DxFontAssign(DxSwitchButton.CloseSetting.CaptionColor.Hot, GuiSwitchButton.CloseSetting.CaptionColor.Hot);
        DxFontAssign(DxSwitchButton.CloseSetting.CaptionColor.Down, GuiSwitchButton.CloseSetting.CaptionColor.Down);
        DxFontAssign(DxSwitchButton.CloseSetting.CaptionColor.Disabled, GuiSwitchButton.CloseSetting.CaptionColor.Disabled);

        DxSwitchButton.CloseSetting.ClickSound := GuiSwitchButton.CloseSetting.ClickSound;
        DxSwitchButton.CloseSetting.Alignment := GuiSwitchButton.CloseSetting.Alignment;

        DxSwitchButton.CloseSetting.CaptionOffsetX := GuiSwitchButton.CloseSetting.CaptionOffsetX;
        DxSwitchButton.CloseSetting.CaptionOffsetY := GuiSwitchButton.CloseSetting.CaptionOffsetY;

        DxSwitchButton.CloseSetting.CaptionDownOffsetX := GuiSwitchButton.CloseSetting.CaptionDownOffsetX;
        DxSwitchButton.CloseSetting.CaptionDownOffsetY := GuiSwitchButton.CloseSetting.CaptionDownOffsetY;
        DxSwitchButton.CloseSetting.ButtonDownOffsetX := GuiSwitchButton.CloseSetting.ButtonDownOffsetX;
        DxSwitchButton.CloseSetting.ButtonDownOffsetY := GuiSwitchButton.CloseSetting.ButtonDownOffsetY;
        DxSwitchButton.CloseSetting.DrawAligment := GuiSwitchButton.CloseSetting.DrawAligment;

        DxSwitchButton.OpenSetting.ImageIndex.ImageType := GuiSwitchButton.OpenSetting.ImageIndex.Image;
        DxSwitchButton.OpenSetting.ImageIndex.Up := GuiSwitchButton.OpenSetting.ImageIndex.Up;
        DxSwitchButton.OpenSetting.ImageIndex.Hot := GuiSwitchButton.OpenSetting.ImageIndex.Hot;
        DxSwitchButton.OpenSetting.ImageIndex.Down := GuiSwitchButton.OpenSetting.ImageIndex.Down;
        DxSwitchButton.OpenSetting.ImageIndex.Disabled := GuiSwitchButton.OpenSetting.ImageIndex.Disabled;

        DxFontAssign(DxSwitchButton.OpenSetting.CaptionColor.Up, GuiSwitchButton.OpenSetting.CaptionColor.Up);
        DxFontAssign(DxSwitchButton.OpenSetting.CaptionColor.Hot, GuiSwitchButton.OpenSetting.CaptionColor.Hot);
        DxFontAssign(DxSwitchButton.OpenSetting.CaptionColor.Down, GuiSwitchButton.OpenSetting.CaptionColor.Down);
        DxFontAssign(DxSwitchButton.OpenSetting.CaptionColor.Disabled, GuiSwitchButton.OpenSetting.CaptionColor.Disabled);

        DxSwitchButton.OpenSetting.ClickSound := GuiSwitchButton.OpenSetting.ClickSound;
        DxSwitchButton.OpenSetting.Alignment := GuiSwitchButton.OpenSetting.Alignment;

        DxSwitchButton.OpenSetting.CaptionOffsetX := GuiSwitchButton.OpenSetting.CaptionOffsetX;
        DxSwitchButton.OpenSetting.CaptionOffsetY := GuiSwitchButton.OpenSetting.CaptionOffsetY;

        DxSwitchButton.OpenSetting.CaptionDownOffsetX := GuiSwitchButton.OpenSetting.CaptionDownOffsetX;
        DxSwitchButton.OpenSetting.CaptionDownOffsetY := GuiSwitchButton.OpenSetting.CaptionDownOffsetY;
        DxSwitchButton.OpenSetting.ButtonDownOffsetX := GuiSwitchButton.OpenSetting.ButtonDownOffsetX;
        DxSwitchButton.OpenSetting.ButtonDownOffsetY := GuiSwitchButton.OpenSetting.ButtonDownOffsetY;
        DxSwitchButton.OpenSetting.DrawAligment := GuiSwitchButton.OpenSetting.DrawAligment;

        if GuiSwitchButton.CloseSetting.CaptionLen > 0 then begin
          SetLength(sText, GuiSwitchButton.CloseSetting.CaptionLen);
          ReadMemory(sText[1], GuiSwitchButton.CloseSetting.CaptionLen);
          DxSwitchButton.CloseSetting.Caption := sText;
        end;

        if GuiSwitchButton.OpenSetting.CaptionLen > 0 then begin
          SetLength(sText, GuiSwitchButton.OpenSetting.CaptionLen);
          ReadMemory(sText[1], GuiSwitchButton.OpenSetting.CaptionLen);
          DxSwitchButton.OpenSetting.Caption := sText;
        end;
      end;
  end;
end;

function NewDxControl(GuiHeader:TGuiHeader; AOwner:TDxControl):TDxControl;
var
  DxControl:TDxControl;

begin
  DxControl := nil;
  case GuiHeader.Gui of
    t_Form:DxControl := TDxImageForm.Create(AOwner);
    t_FormShape:DxControl := TDxImageFormShape.Create(AOwner);
    t_Button:DxControl := TDxImageButton.Create(AOwner);
    t_Edit:DxControl := TDxEdit.Create(AOwner);
    t_ImageEdit:DxControl := TDxImageEdit.Create(AOwner);
    t_Label:DxControl := TDxLabel.Create(AOwner);
    t_Grid:DxControl := TDxImageGrid.Create(AOwner);
    t_ScrollBox:DxControl := TDxScrollBox.Create(AOwner);
    t_ChatMemo:DxControl := TDxChatMemo.Create(AOwner);
    t_ListView:DxControl := TDxListView.Create(AOwner);
    t_TreeView:DxControl := TDxTreeView.Create(AOwner);
    t_PopupMenu:DxControl := TDxPopupMenu.Create(AOwner);
    t_TabSheet:DxControl := TDxTabSheet.Create(AOwner);
    t_PageControl:DxControl := TDxPageControl.Create(AOwner);
    t_ComboBox:DxControl := TDxComboBox.Create(AOwner);
    t_Line:DxControl := TDxLine.Create(AOwner);
    t_TrackBar:DxControl := TDxTrackBar.Create(AOwner);
    t_MainBottomForm:DxControl := TDxMainBottomForm.Create(AOwner);
    t_MagicBall:DxControl := TDxMagicBall.Create(AOwner);
    t_SexPanel:DxControl := TDxSexPanel.Create(AOwner);
    t_GroupAttackProgress:DxControl := TDxGroupAttackProgress.Create(AOwner);
    t_ImageProgress:DxControl := TDxImageProgress.Create(AOwner);
    t_SwitchButton:DxControl := TDxSwitchButton.Create(AOwner);
  end;

  if DxControl <> nil then begin
    DxControl.GuiType := GuiHeader.Gui;
    TDxControl(PControlAddress^) := DxControl;
    //MessageBox(0,PChar(DxControl.Name),'',MB_OK);
    Inc(PInteger(PControlAddress)); // 地址指向下一个控件

    // DxControl.Name := ''; //GuiHeader.;// '';
   // with DxControl do begin
    DxControl.Left := GuiHeader.Left;
    DxControl.Top := GuiHeader.Top;
    DxControl.Width := GuiHeader.Width;
    DxControl.Height := GuiHeader.Height;
    DxControl.Enabled := GuiHeader.Enabled;
    DxControl.Visible := GuiHeader.Visible;
    DxControl.Transparent := GuiHeader.Transparent;
    DxControl.EnableFocus := GuiHeader.EnableFocus;
    DxControl.Floating := GuiHeader.Floating;
    DxControl.OwnerMove := GuiHeader.OwnerMove;
    DxControl.Designing := False;
    DxControl.OnGetImage := AOwner.OnGetImage;
    DxControl.MouseEvents := GuiHeader.MouseEvents;
    // Tag := Integer(GuiHeader.Gui);
  // DxControl.ImageType := GuiHeader.Image;
  // end;
  end;

  Result := DxControl;
end;

function LoadSubComponent(AOwner:TDxControl; GuiVersion:Integer; sUiName:string):Integer;
var
  I:Integer;
  DxControl:TDxControl;
  GuiHeader:TGuiHeader;
  GuiHeaderAdd:TGuiHeaderAdd;
  sText:string;
begin
  Result := 0;
  if ReadMemory(GuiHeader, SizeOf(TGuiHeader)) = SizeOf(TGuiHeader) then begin
    if GuiVersion >= 20170226 then begin
      DecryptDes(GuiHeader, GuiHeader, SizeOf(TGuiHeader), #2#1#6#14#20#3#4#1#6#5#10#9);
    end;

    DxControl := NewDxControl(GuiHeader, AOwner);
    if DxControl <> nil then begin
      if GuiHeader.NameLen > 0 then begin
        SetLength(sText, GuiHeader.NameLen);
        ReadMemory(sText[1], GuiHeader.NameLen);
        DxControl.Name := sText;
        {$IFDEF  OUTPUT_GUI_READORDER}
        LogHelper.WriteToLogFile(Format('[%s] %s', [sUiName, sText]), LOG_LEVEL_3);
        {$ENDIF}
      end;

      if GuiVersion >= 20160409 then begin
        if ReadMemory(GuiHeaderAdd, SizeOf(TGuiHeaderAdd)) = SizeOf(TGuiHeaderAdd) then begin
          DxControl.ReferenceX := GuiHeaderAdd.ReferenceX;
          DxControl.AdjustYByHeight := GuiHeaderAdd.AdjustYByHeight;
          DxControl.TopAlignment := GuiHeaderAdd.TopAlignment;
          if GuiHeaderAdd.ShowNameLen > 0 then begin
            SetLength(sText, GuiHeaderAdd.ShowNameLen);
            ReadMemory(sText[1], GuiHeaderAdd.ShowNameLen);
            DxControl.ShowName := sText;
          end;

          if GuiHeaderAdd.HintTextLen > 0 then begin
            SetLength(sText, GuiHeaderAdd.HintTextLen);
            ReadMemory(sText[1], GuiHeaderAdd.HintTextLen);
            DxControl.Hint := sText;
          end;
        end;
      end;

      if GuiVersion <= 20171106 then begin
        DxControl.TopAlignment := False;
      end;

      LoadComponent(GuiHeader.Gui, DxControl, GuiVersion);
      Result := GuiHeader.Count;
      for I := 0 to GuiHeader.Count - 1 do begin
        Result := Result + LoadSubComponent(DxControl, GuiVersion, sUiName);
      end;
    end;
  end;
end;

function LoadControlFromStream(MemoryStream:TMemoryStream; Background:TDxControl; AControlAddress:Pointer; sUiName:string):Integer;
begin
  Result := LoadControlFromMemory(AControlAddress, MemoryStream.Memory, MemoryStream.Size, Background, sUiName);
end;

function LoadControlFromMemory(Address, Memory:Pointer; Size:Integer; Background:TDxControl; sUiName:string):Integer;
var
  I, Len:Integer;
  DxControl:TDxControl;
  GuiHeader:TGuiHeader;
  GuiFileHeader:TGuiFileHeader;
  GuiHeaderAdd:TGuiHeaderAdd;
  sText:string;
begin
  MemoryData := Memory;
  MemorySize := Size;
  MemoryPosition := 0;

  PControlAddress := nil;
  PControlAddress := Address;
  ReadMemory(GuiFileHeader, SizeOf(TGuiFileHeader));
  Result := GuiFileHeader.nCount;

  if (GuiFileHeader.nGuiVersion >= 20160409) and (GuiFileHeader.GroupCount > 0) then begin
    for I := 0 to GuiFileHeader.GroupCount - 1 do begin
      ReadMemory(Len, SizeOf(Len));
      if Len > 0 then begin
        SetLength(sText, Len);
        ReadMemory(sText[1], Len);
      end else begin
        sText := '';
      end;
      //OutputDebugString(PChar(Format('i = %d, Len = %d, Position = %d', [i, Len, MemoryPosition])));
    end;
  end;

  while True do begin
    if MemoryPosition >= MemorySize then break;

    if ReadMemory(GuiHeader, SizeOf(TGuiHeader)) = SizeOf(TGuiHeader) then begin
      if GuiFileHeader.nGuiVersion >= 20170226 then begin
        DecryptDes(GuiHeader, GuiHeader, SizeOf(TGuiHeader), #2#1#6#14#20#3#4#1#6#5#10#9);
      end;

      DxControl := NewDxControl(GuiHeader, Background);

      if DxControl <> nil then begin
        if GuiHeader.NameLen > 0 then begin
          SetLength(sText, GuiHeader.NameLen);
          ReadMemory(sText[1], GuiHeader.NameLen);
          DxControl.Name := sText;
          {$IFDEF  OUTPUT_GUI_READORDER}
          LogHelper.WriteToLogFile(Format('[%s] %s', [sUiName, sText]), LOG_LEVEL_3);
          {$ENDIF}
        end;

        if GuiFileHeader.nGuiVersion >= 20160409 then begin
          if ReadMemory(GuiHeaderAdd, SizeOf(TGuiHeaderAdd)) = SizeOf(TGuiHeaderAdd) then begin
            DxControl.ReferenceX := GuiHeaderAdd.ReferenceX;
            DxControl.AdjustYByHeight := GuiHeaderAdd.AdjustYByHeight;
            DxControl.TopAlignment := GuiHeaderAdd.TopAlignment;
            if GuiHeaderAdd.ShowNameLen > 0 then begin
              SetLength(sText, GuiHeaderAdd.ShowNameLen);
              ReadMemory(sText[1], GuiHeaderAdd.ShowNameLen);
              DxControl.ShowName := sText;
            end;

            if GuiHeaderAdd.HintTextLen > 0 then begin
              SetLength(sText, GuiHeaderAdd.HintTextLen);
              ReadMemory(sText[1], GuiHeaderAdd.HintTextLen);
              DxControl.Hint := sText;
            end;
          end;
        end;

        if GuiFileHeader.nGuiVersion <= 20171106 then begin
          DxControl.TopAlignment := False;
        end;

        DxControl.Visible := False;

        LoadComponent(GuiHeader.Gui, DxControl, GuiFileHeader.nGuiVersion);

        for I := 0 to GuiHeader.Count - 1 do begin
          LoadSubComponent(DxControl, GuiFileHeader.nGuiVersion, sUiName);
        end;
      end;
    end else begin
      break;
    end;
  end; // while True do begin  Result := Result +
end;

end.
