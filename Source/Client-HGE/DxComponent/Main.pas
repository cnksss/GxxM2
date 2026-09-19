unit Main;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, MMSystem, Share, DxControls, DxComponents, GameImages, Menus,
  Magnetic, Clipbrd, StreamClipbrd, DxCanvas, Wil, Wis, Pak, Wzl, TextureImages,
  StdCtrls, ExtCtrls, RzBmpBtn, RzButton, FastStrings, UnitDes;

type
  TFrmMain = class(TForm)
    PopupMenuMain: TPopupMenu;
    Menu_Create: TMenuItem;
    Menu_Delete: TMenuItem;
    Menu_Copy: TMenuItem;
    Menu_Paste: TMenuItem;
    TimerClose: TTimer;
    OpenDialog: TOpenDialog;
    SaveDialog: TSaveDialog;
    Menu_CreateTDxImageForm: TMenuItem;
    N2: TMenuItem;
    N4: TMenuItem;
    Menu_Save: TMenuItem;
    Menu_SaveToFile: TMenuItem;
    N5: TMenuItem;
    Menu_Exit: TMenuItem;
    Menu_Background: TMenuItem;
    N6: TMenuItem;
    Menu_LoadFromFile: TMenuItem;
    Menu_CreateTDxImageButton: TMenuItem;
    Menu_CreateTDxPageControl: TMenuItem;
    Menu_CreateTDxImageGrid: TMenuItem;
    Menu_CreateTDxLabel: TMenuItem;
    Menu_CreateTDxEdit: TMenuItem;
    Menu_CreateTDxComboBox: TMenuItem;
    Menu_CreateTDxMemo: TMenuItem;
    Menu_CreateTDxChatMemo: TMenuItem;
    Menu_CreateTDxListView: TMenuItem;
    Menu_CreateTDxTreeView: TMenuItem;
    Menu_CreateTDxPopupMenu: TMenuItem;
    TimerStart: TTimer;
    N7: TMenuItem;
    Menu_MainPage: TMenuItem;
    Menu_CreateTDxLine: TMenuItem;
    PopupMenu1: TPopupMenu;
    MenuItem1_MainPage: TMenuItem;
    MenuItem2: TMenuItem;
    MenuItem1_CreateTDxTabSheet: TMenuItem;
    PopupMenu2: TPopupMenu;
    MenuItem2_MainPage: TMenuItem;
    MenuItem3: TMenuItem;
    MenuItem2_DeleteTDxTabSheet: TMenuItem;
    Timer: TTimer;
    Menu_CreateTDxImageFormShape: TMenuItem;
    procedure FormKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure FormKeyPress(Sender: TObject; var Key: Char);
    procedure FormKeyUp(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure Menu_DeleteClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure Menu_CopyClick(Sender: TObject);
    procedure Menu_PasteClick(Sender: TObject);
    procedure FormDblClick(Sender: TObject);
    procedure FormMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure FormMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure FormMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure FormDestroy(Sender: TObject);
    procedure TimerCloseTimer(Sender: TObject);
    procedure PopupMenuMainPopup(Sender: TObject);
    procedure Menu_LoadFromFileClick(Sender: TObject); stdcall;
    procedure Menu_SaveClick(Sender: TObject);
    procedure Menu_SaveToFileClick(Sender: TObject);
    procedure OnCreateDxControlClick(Sender: TObject);
    procedure TimerStartTimer(Sender: TObject);
    procedure Menu_MainPageClick(Sender: TObject);
    procedure MenuItem1_MainPageClick(Sender: TObject);
  private
    function NewDxControl(GuiHeader: TGuiHeader; AOwner: TDxControl): TDxControl;
    procedure LoadComponent(FileStream: TStream; DxControl: TDxControl);
    procedure LoadSubComponent(FileStream: TStream; AOwner: TDxControl; OffSetX: Integer = 0; OffSetY: Integer = 0);

    procedure OnProgramException(Sender: TObject; E: Exception);


    procedure SaveSubComponent(FileStream: TStream; DxControl: TDxControl);
    procedure SaveComponent(FileStream: TStream; DxControl: TDxControl);

    function GetSaveComponentString(DxControl: TDxControl): string;
    procedure SaveSubComponentToPasFile(StringList: TStringList; DxControl: TDxControl);

    procedure SubMenuClick(Sender: TObject);
    procedure RefFileNameList;
    procedure RefBackgroundList;
    procedure ChangeBackgroundClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure LoadFromFileClick(Sender: TObject; X, Y: Integer); stdcall;
  public
    procedure OnDxControlMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer); stdcall;
    procedure OnDxControlClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure OnDxControlDblClick(Sender: TObject; X, Y: Integer); stdcall;
    procedure OnDxControlGetImage(Sender: TObject; ImageType: TImageType; var AImage: TObject);
    procedure OnDeviceCreate(Sender: TObject);
    procedure OnDeviceDestroy(Sender: TObject);

    procedure OnDeviceReset(Sender: TObject);
    procedure OnDeviceLost(Sender: TObject);


    procedure TimerEvent(Sender: TObject);
    procedure ProcessEvent(Sender: TObject);
    procedure RenderEvent(Sender: TObject);
    procedure AddSubMenu(DxControl: TDxControl);
    procedure DelSubMenu(DxControl: TDxControl);
    procedure LoadControl(FileStream: TStream; DxControl: TDxControl; OffSetX: Integer = 0; OffSetY: Integer = 0);
    procedure SaveControl(FileStream: TStream; DxControl: TDxControl);
    procedure SaveToFile(FileName: string);
    procedure ClearComponent;
    procedure TabSheetCreate(Sender: TObject); stdcall;
    procedure TabSheetDestroy(Sender: TObject); stdcall;
    procedure WMEnterSizeMove(var Msg: TMessage); message WM_ENTERSIZEMOVE;
    procedure WMSizing(var Msg: TMessage); message WM_SIZING;
    procedure WMMoving(var Msg: TMessage); message WM_MOVING;
    procedure WMExitSizeMove(var Msg: TMessage); message WM_EXITSIZEMOVE;
    procedure WMSysCommand(var Msg: TMessage); message WM_SYSCOMMAND;
    procedure WMCommand(var Msg: TMessage); message WM_COMMAND;
  end;

var
  FrmMain: TFrmMain;

implementation
uses
  HGE,
  HGECanvas,
  IniFiles,

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
  Structure,
  Objects,
  LoginDlg;
{$R *.dfm}
var
  MagneticWndProc: TSubClass_Proc;
  dummyHandled: Boolean;
  MagneticAddWindow: Boolean;

  DxControlCount: Integer;

procedure TFrmMain.WMEnterSizeMove(var Msg: TMessage);
begin
  inherited;

  if Assigned(MagneticWndProc) then
    MagneticWndProc(Self.Handle, WM_ENTERSIZEMOVE, Msg, dummyHandled);
end;

procedure TFrmMain.WMSizing(var Msg: TMessage);
var
  bHandled: Boolean;
begin
  if not Assigned(MagneticWndProc) then
    inherited
  else
    if MagneticWndProc(Self.Handle, WM_SIZING, Msg, bHandled) then
    if not bHandled then
      inherited;
end;

procedure TFrmMain.WMMoving(var Msg: TMessage);
var
  bHandled: Boolean;
begin
  if not Assigned(MagneticWndProc) then
    inherited
  else
    if MagneticWndProc(Self.Handle, WM_MOVING, Msg, bHandled) then
    if not bHandled then
      inherited;
end;

procedure TFrmMain.WMExitSizeMove(var Msg: TMessage);
begin
  inherited;

  if Assigned(MagneticWndProc) then
    MagneticWndProc(Self.Handle, WM_EXITSIZEMOVE, Msg, dummyHandled);
end;

procedure TFrmMain.WMSysCommand(var Msg: TMessage);
begin
  inherited;

  if Assigned(MagneticWndProc) then
    MagneticWndProc(Self.Handle, WM_SYSCOMMAND, Msg, dummyHandled);
end;

procedure TFrmMain.WMCommand(var Msg: TMessage);
begin
  inherited;

  if Assigned(MagneticWndProc) then
    MagneticWndProc(Self.Handle, WM_COMMAND, Msg, dummyHandled);
end;

//------------------ end of Custom Message Handling procedures -------------------


// procedure to subclass ChildForms window procedure for magnetic effect.

function SubFormWindowProc(Wnd: HWND; Msg, wParam, lParam: Integer): Integer; stdcall;
var
  Handled: boolean;
  Message_: TMessage;
  OrgWndProc: Integer;
begin
  Result := 0;

  if not Assigned(MagneticWndProc) then
  begin
    Result := CallWindowProc(Pointer(OrgWndProc), Wnd, Msg, wParam, lParam);
    exit;
  end;

  OrgWndProc := GetWindowLong(Wnd, GWL_USERDATA);
  if (OrgWndProc = 0) then
    exit;

  Message_.WParam := wParam;
  Message_.LParam := lParam;
  Message_.Result := 0;

  if (Msg = WM_SYSCOMMAND) or (Msg = WM_ENTERSIZEMOVE) or (Msg = WM_EXITSIZEMOVE) or
    (Msg = WM_WINDOWPOSCHANGED) or (Msg = WM_COMMAND) then
  begin
    Result := CallWindowProc(Pointer(OrgWndProc), Wnd, Msg, wParam, lParam);
    MagneticWndProc(Wnd, Msg, Message_, dummyHandled);
  end else if (Msg = WM_MOVING) or (Msg = WM_SIZING) then
  begin
    MagneticWndProc(Wnd, Msg, Message_, Handled);
    if Handled then
    begin
      Result := Message_.Result;
      exit;
    end else
      Result := CallWindowProc(Pointer(OrgWndProc), Wnd, Msg, wParam, lParam);
  end else if (Msg = WM_DESTROY) then
  begin
    if Assigned(MagneticWnd) then
      MagneticWnd.RemoveWindow(Wnd);
    Result := CallWindowProc(Pointer(OrgWndProc), Wnd, Msg, wParam, lParam);
  end else
    Result := CallWindowProc(Pointer(OrgWndProc), Wnd, Msg, wParam, lParam);
end;

procedure TFrmMain.RenderEvent(Sender: TObject);
var
  vtRect: TRect;
  vbRect: TRect;
begin
  if (WindowState = wsMinimized) or g_boClose then begin
    Exit;
  end;
  if g_Background <> nil then begin
    g_Background.Paint;
    if g_SelectComponent <> nil then begin
      vtRect := TDxControl(g_SelectComponent).VirtualRect;
      vbRect := TDxControl(g_SelectComponent).VisibleRect;
      GameCanvas.FrameRect(vtRect, clRed);
      GameCanvas.FrameRect(ShrinkRect(vtRect, 1, 1), clRed);

      {if TDxControl(g_SelectComponent) is TDxMemo then begin
          GameCanvas.FrameRect(TDxMemo(g_SelectComponent).MaxRect, clLime);
          AspTextureFont.TextOut(vtRect.Left,vtRect.Top,IntToStr(TDxMemo(g_SelectComponent).MaxValue));
           vtRect := TDxMemo(g_SelectComponent).MaxBottom.VirtualRect;
           GameCanvas.FrameRect(vtRect, clLime);
           AspTextureFont.TextOut(vtRect.Left,vtRect.Top,TDxMemo(g_SelectComponent).MaxBottom.Name);
      end;}
    end;
  end;
end;

procedure TFrmMain.OnDeviceDestroy(Sender: TObject);
begin
  g_WPrguseImages.Finalize;
  g_WPrguse2Images.Finalize;
  g_WPrguse3Images.Finalize;
  g_WisPrguseImages.Finalize;
  g_WisPrguse2Images.Finalize;
  g_WisPrguse3Images.Finalize;
  g_WPrguseImages_16.Finalize;
  g_WPrguse2Images_16.Finalize;
  g_WPrguse3Images_16.Finalize;
  g_WChrSelImages.Finalize;
  g_WChrSelImages_16.Finalize;
  g_WUIImages.Finalize;
  g_WUI1Images.Finalize;
  g_WUI2Images.Finalize;
  g_WUI3Images.Finalize;
  Timer.Enabled := False;
end;

function GetGameImages(FileName: string): TGameImages;
var
  I, nPos: Integer;
  sFileExt: string;
  sFileName: string;
  sFilePath: string;
  AppFilePath: string;
  APassWord: TPakPassword;
begin
  Result := nil;
  AppFilePath := g_sMirDataDirectory; // ExtractFilePath(Paramstr(0));
  nPos := FastPosNoCase(FileName, '\Graphics\', Length(FileName), Length('\Graphics\'), 1);
  if nPos > 0 then begin
    sFileName := AppFilePath + 'Resources\' + Copy(FileName, Length(AppFilePath) + 1, Length(FileName) - Length(AppFilePath));
    sFileName := ExtractFilePath(sFileName) + ExtractFileNameOnly(sFileName) + '.pak';
    //DebugOutStr('sFileName:'+sFileName);
    if FileExists(sFileName) then begin
      FileName := sFileName;
      sFileExt := '.PAK'; // UpperCase(ExtractFileExt(sFileName));
    end else begin
      sFileExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
    end;
  end else begin
    sFileExt := UpperCase(ExtractFileExt(ExtractFileName(FileName)));
  //if sFileExt <> '.PAK' then begin
    sFilePath := ExtractFilePath(FileName);

    try
      nPos := FastPosNoCase(sFilePath, '\Data\', Length(sFilePath), Length('\Data\'), 1);
    except
      nPos := Length(sFilePath) - 5;
    end;
    if nPos > 0 then
      sFilePath := Copy(sFilePath, 1, nPos);

    if (sFilePath <> '') and (sFilePath[Length(sFilePath)] <> '\') then
      sFilePath := sFilePath + '\';

    sFileName := sFilePath + 'Resources\Data\' + ExtractFileNameOnly(FileName) + '.pak';
    if FileExists(sFileName) then begin
      FileName := sFileName;
      sFileExt := '.PAK'; // UpperCase(ExtractFileExt(sFileName));
    end;
 // end;
 // DebugOutStr('GetGameImages2:'+FileName);
    if (sFileExt = '.WIL') or (sFileExt = '.WIS') then begin
      if not FileExists(FileName) then begin
        sFileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wzl';
        if FileExists(sFileName) then begin
          FileName := sFileName;
          sFileExt := '.WZL';
        end;
      end;
    end;
  end;

  if sFileExt = '.WZL' then begin
    Result := TWzlImages.Create();
    Result.FileName := FileName;
  end else
    if sFileExt = '.WIL' then begin
    Result := TWMImages.Create();
    Result.FileName := FileName;
  end else
    if sFileExt = '.WIS' then begin
    Result := TWisImages.Create();
    Result.FileName := FileName;
  end else
    if sFileExt = '.PAK' then begin
    GetKeyData('gameofmir', @APassWord.Chain, @APassWord.KeyData);
    Result := TPakImages.Create(APassWord);
    Result.FileName := FileName;
  end else begin
    Result := TWMImages.Create();
    Result.FileName := FileName;
  end;
end;

procedure TfrmMain.OnDeviceReset(Sender: TObject);
begin
end;

procedure TfrmMain.OnDeviceLost(Sender: TObject);
begin

end;

procedure TFrmMain.OnDeviceCreate(Sender: TObject);
var
  sFileName: string;
begin
  //CurrentFont := TextureFonts.FindFont('宋体', 9);
  g_WPrguseImages.Initialize;
  g_WPrguse2Images.Initialize;
  g_WPrguse3Images.Initialize;
  g_WisPrguseImages.Initialize;
  g_WisPrguse2Images.Initialize;
  g_WisPrguse3Images.Initialize;
  g_WPrguseImages_16.Initialize;
  g_WPrguse2Images_16.Initialize;
  g_WPrguse3Images_16.Initialize;
  g_WChrSelImages.Initialize;
  g_WChrSelImages_16.Initialize;
  g_WUIImages.Initialize;
  g_WUI1Images.Initialize;
  g_WUI2Images.Initialize;
  g_WUI3Images.Initialize;
  g_WNewopUIImages.Initialize;


  g_FileNameMemo.Designing := False;
  g_FileNameMemo.ShowScroll := True;
  g_FileNameMemo.ExpandSize := 0;
  g_FileNameMemo.ItemHeight := 20;
  g_FileNameMemo.Left := 12;
  g_FileNameMemo.Top := 12;
  g_FileNameMemo.Transparent := False;
  g_FileNameMemo.BackgroundColor := clSilver;
  g_FileNameMemo.Width := g_MainBackground.Width - 24;
  g_FileNameMemo.Height := (g_MainBackground.Height - 36) div 2;
  g_FileNameMemo.OnGetImage := OnDxControlGetImage;

  g_FileNameMemo.PrevImageIndex.ImageType := Prguse2_wil;
  g_FileNameMemo.PrevImageIndex.Up := 292;
  g_FileNameMemo.PrevImageIndex.Down := 293;

  g_FileNameMemo.NextImageIndex.ImageType := Prguse2_wil;
  g_FileNameMemo.NextImageIndex.Up := 294;
  g_FileNameMemo.NextImageIndex.Down := 295;

  g_FileNameMemo.BarImageIndex.ImageType := Prguse2_wil;
  g_FileNameMemo.BarImageIndex.Up := 579;

  g_FileNameMemo.ScrollImageIndex.ImageType := Prguse2_wil;
  g_FileNameMemo.ScrollImageIndex.Up := 291;


  g_BackgroundMemo.Designing := False;
  g_BackgroundMemo.ShowScroll := True;
  g_BackgroundMemo.ExpandSize := 0;
  g_BackgroundMemo.ItemHeight := 20;
  g_BackgroundMemo.Transparent := False;
  g_BackgroundMemo.BackgroundColor := clSilver;
  g_BackgroundMemo.Left := 12;
  g_BackgroundMemo.Top := (g_MainBackground.Height - 36) div 2 + 24;
  g_BackgroundMemo.Width := g_MainBackground.Width - 24;
  g_BackgroundMemo.Height := (g_MainBackground.Height - 48) div 2;
  g_BackgroundMemo.OnGetImage := OnDxControlGetImage;

  g_BackgroundMemo.PrevImageIndex.ImageType := Prguse2_wil;
  g_BackgroundMemo.PrevImageIndex.Up := 292;
  g_BackgroundMemo.PrevImageIndex.Down := 293;

  g_BackgroundMemo.NextImageIndex.ImageType := Prguse2_wil;
  g_BackgroundMemo.NextImageIndex.Up := 294;
  g_BackgroundMemo.NextImageIndex.Down := 295;

  g_BackgroundMemo.BarImageIndex.ImageType := Prguse2_wil;
  g_BackgroundMemo.BarImageIndex.Up := 579;

  g_BackgroundMemo.ScrollImageIndex.ImageType := Prguse2_wil;
  g_BackgroundMemo.ScrollImageIndex.Up := 291;

  Timer.Enabled := True;
end;
//---------------------------------------------------------------------------

procedure TFrmMain.TimerEvent(Sender: TObject);
begin
  if (WindowState = wsMinimized) or g_boClose then begin
    Exit;
  end;
  GameCanvas.Render(RenderEvent, DisplaceRB(clSkyBlue) or $FF000000, True);
end;

//---------------------------------------------------------------------------

procedure TFrmMain.ProcessEvent(Sender: TObject);
begin
  //Inc(GameTicks);
end;

procedure TFrmMain.OnDxControlGetImage(Sender: TObject; ImageType: TImageType; var AImage: TObject);
begin
  case ImageType of
    Prguse_wil: AImage := g_WPrguseImages;
    Prguse2_wil: AImage := g_WPrguse2Images;
    Prguse3_wil: AImage := g_WPrguse3Images;
    Prguse_wis: AImage := g_WisPrguseImages;
    Prguse2_wis: AImage := g_WisPrguse2Images;
    Prguse3_wis: AImage := g_WisPrguse3Images;
    ChrSel_wil: AImage := g_WChrSelImages;
    Prguse_16_wil: AImage := g_WPrguseImages_16;
    Prguse2_16_wil: AImage := g_WPrguse2Images_16;
    Prguse3_16_wil: AImage := g_WPrguse3Images_16;
    ChrSel_16_wil: AImage := g_WChrSelImages_16;
    UI_wil: AImage := g_WUIImages;
    UI1_wil: AImage := g_WUI1Images;
    UI2_wil: AImage := g_WUI2Images;
    UI3_wil: AImage := g_WUI3Images;
    NewopUI_Pak: AImage := g_WNewopUIImages;
  else AImage := nil;
  end;
end;

procedure TFrmMain.OnDxControlDblClick(Sender: TObject; X, Y: Integer);
begin
  ObjectsDlg.AddComponent(TComponent(Sender));
end;

procedure TFrmMain.OnDxControlMouseDown(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
//var
  //DxControl: TDxControl;
begin
  //DxControl := TDxControl(Sender);
  if Sender is TDxControl then
    g_SelectComponent := TDxControl(Sender)
  else
    g_SelectComponent := nil;
 // ObjectsDlg.AddComponent(DxControl);

  if (g_SelectComponent <> nil) and (g_SelectComponent is TDxPageControl) then begin
    Self.PopupMenu := PopupMenu1;
  end else
    if (g_SelectComponent <> nil) and (g_SelectComponent is TDxTabSheet) then begin
    Self.PopupMenu := PopupMenu2;
  end else begin
    Self.PopupMenu := PopupMenuMain;
  end;

end;

procedure TFrmMain.OnDxControlClick(Sender: TObject; X, Y: Integer);
begin
  ObjectsDlg.AddComponent(TComponent(Sender));
end;

procedure TFrmMain.Menu_DeleteClick(Sender: TObject);
var
  TreeNode, ChildTreeNode: TTreeNode;
  DxControl: TDxControl;
begin
  if g_SelectComponent <> nil then begin
    DxControl := TDxControl(g_SelectComponent);
    g_SelectComponent := nil;

    TreeNode := TTreeNode(DxControl.Data);
    StructureDlg.TreeView.Items.Delete(TreeNode);
    ObjectsDlg.JvInspector.Clear;

    FreeAndNil(DxControl);
  end;
end;

procedure TFrmMain.FormKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
var
  TreeNode, ChildTreeNode: TTreeNode;
  MemoryStream: TMemoryStream;
  vRect: TRect;
begin
  if g_Background <> nil then
    g_Background.KeyDown(Key, Shift);

  if (ssCtrl in Shift) then begin
    case Key of
      Byte('C'): begin
          if (g_SelectComponent <> nil) then begin
            MemoryStream := TMemoryStream.Create;
            FrmMain.SaveControl(MemoryStream, TDxControl(g_SelectComponent));
            MemoryStream.Position := 0;
            Clipboard.Clear;

            StreamSaveToClipboard(MemoryStream);
            MemoryStream.Free();

            vRect := TDxControl(g_SelectComponent).VirtualRect;
            g_nComponentX := vRect.Left;
            g_nComponentY := vRect.Top;
          end;
        end;
      Byte('X'): begin
          if (g_SelectComponent <> nil) then begin
            MemoryStream := TMemoryStream.Create;
            FrmMain.SaveControl(MemoryStream, TDxControl(g_SelectComponent));
            MemoryStream.Position := 0;
            Clipboard.Clear;

            StreamSaveToClipboard(MemoryStream);
            MemoryStream.Free();

            vRect := TDxControl(g_SelectComponent).VirtualRect;
            g_nComponentX := vRect.Left;
            g_nComponentY := vRect.Top;

            TreeNode := TTreeNode(TDxControl(g_SelectComponent).Data);
            StructureDlg.TreeView.Items.Delete(TreeNode);
            ObjectsDlg.JvInspector.Clear;
            FreeAndNil(g_SelectComponent);
          end;
        end;
      Byte('Z'): ;
      Byte('V'): begin
          if g_SelectComponent <> nil then begin
            if not ((TDxControl(g_SelectComponent) is TDxImageButton) or
              (TDxControl(g_SelectComponent) is TDxEdit) or
              (TDxControl(g_SelectComponent) is TDxLabel) or
              //(TDxControl(g_SelectComponent) is TDxImageGrid) or
              (TDxControl(g_SelectComponent) is TDxPopupMenu) or
              (TDxControl(g_SelectComponent) is TDxComboBox)) then begin
              MemoryStream := TMemoryStream.Create;
              StreamLoadFromClipboard(MemoryStream);
              MemoryStream.Position := 0;
              LoadControl(MemoryStream, TDxControl(g_SelectComponent), g_nMouseX - g_nComponentX, g_nMouseY - g_nComponentY);
              MemoryStream.Free;
            end;
          end else
            if g_Background <> nil then begin
            MemoryStream := TMemoryStream.Create;
            StreamLoadFromClipboard(MemoryStream);
            MemoryStream.Position := 0;
            LoadControl(MemoryStream, g_Background, g_nMouseX - g_nComponentX, g_nMouseY - g_nComponentY);
            MemoryStream.Free;
          end;
        end;
      Byte('A'): ;
    end;
  end;
end;

procedure TFrmMain.FormKeyPress(Sender: TObject; var Key: Char);
begin
  if g_Background <> nil then
    g_Background.KeyPress(Key);
end;

procedure TFrmMain.FormKeyUp(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if g_Background <> nil then
    g_Background.KeyUp(Key, Shift);
end;

procedure TFrmMain.FormCreate(Sender: TObject);
var
  sFileName: string;
begin
  MagneticWnd := TMagnetic.Create;
  MagneticAddWindow := False;
  if Assigned(MagneticWnd) and (not MagneticAddWindow) then begin
    MagneticAddWindow := True;
    // Set Snap width
    MagneticWnd.SnapWidth := 15;
    // Register main window as a serviced window of TMagnetic Class
    MagneticWnd.AddWindow(Self.Handle, 0, MagneticWndProc);
  end;
  ClientHeight := 768;
  ClientWidth := 1024;

  g_SaveComponentList := TStringList.Create;
  g_Background := nil;


  if (g_sMirDataDirectory <> '') and (g_sMirDataDirectory[Length(g_sMirDataDirectory)] <> '\') then
    g_sMirDataDirectory := g_sMirDataDirectory + '\';


  sFileName := g_sMirDataDirectory + 'Data\Prguse.wil';
  g_WPrguseImages := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse2.wil';
  g_WPrguse2Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse3.wil';
  g_WPrguse3Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse.wis';
  g_WisPrguseImages := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse2.wis';
  g_WisPrguse2Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse3.wis';
  g_WisPrguse3Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse_16.wil';
  g_WPrguseImages_16 := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse2_16.wil';
  g_WPrguse2Images_16 := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\Prguse3_16.wil';
  g_WPrguse3Images_16 := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\ChrSel.wil';
  g_WChrSelImages := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\ChrSel_16.wil';
  g_WChrSelImages_16 := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\UI.wil';
  g_WUIImages := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\UI1.wil';
  g_WUI1Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\UI2.wil';
  g_WUI2Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\UI3.wil';
  g_WUI3Images := GetGameImages(sFileName);

  sFileName := g_sMirDataDirectory + 'Data\NewopUI.pak';
  g_WNewopUIImages := GetGameImages(sFileName);




  StructureDlg := TStructureDlg.Create(Self);
  ObjectsDlg := TObjectsDlg.Create(Self);

  g_MainBackground := TDxControlEngine.Create();

  g_MainBackground.Width := ClientWidth;
  g_MainBackground.Height := ClientHeight;
  g_MainBackground.Name := 'MainBackground';

  g_FileNameMemo := TDxScrollBox.Create(g_MainBackground);
  g_FileNameMemo.Name := 'FileNameMemo';

  g_BackgroundMemo := TDxScrollBox.Create(g_MainBackground);
  g_FileNameMemo.Name := 'BackgroundMemo';

  GameCanvas := THGECanvas.Create;
  GameCanvas.Handle := Handle;
  GameCanvas.BitCount := 16;
  GameCanvas.DepthStencil := True;
  GameCanvas.Width := ClientWidth;
  GameCanvas.Height := ClientHeight;
  GameCanvas.Windowed := True; //g_boWindowMode;
  GameCanvas.Hardware := False; //g_boHardware; //g_boHardware; // g_boHardware;
  GameCanvas.VSync := False;
  GameCanvas.OnInitialize := OnDeviceCreate;
  GameCanvas.OnFinalize := OnDeviceDestroy;
  GameCanvas.OnDeviceLost := OnDeviceLost;
  GameCanvas.OnDeviceReset := OnDeviceReset;
  GameCanvas.LogFilePath := ExtractFilePath(Paramstr(0));
  //GameCanvas.LogFileName := ExtractFilePath(Paramstr(0)) + 'Mir.log';

  if (not GameCanvas.Initialize()) then
  begin
    ShowMessage('Failed to initialize HGE device.');
    Application.Terminate();
    Exit;
  end;

  Timer.OnTimer := TimerEvent;

  TimerStart.Enabled := True;
  Application.OnException := OnProgramException;
  //OnDebugOut := DebugOutStr;
end;

procedure TfrmMain.OnProgramException(Sender: TObject; E: Exception);
begin
  //DebugOutStr(E.Message);
end;

procedure TFrmMain.ClearComponent;
var
  I: Integer;
  TreeNode: TTreeNode;
  DxControlEngine: TDxControlEngine;
begin
  g_Background := nil;
  for I := 0 to StructureDlg.TreeView.Items.Count - 1 do begin
    TreeNode := StructureDlg.TreeView.Items.Item[I];
    if TreeNode.Parent <> nil then Continue;
    DxControlEngine := TDxControlEngine(TreeNode.Data);
    DxControlEngine.Free;
  end;
  StructureDlg.TreeView.Items.Clear;
  g_boCanClose := True;
end;

procedure TFrmMain.LoadSubComponent(FileStream: TStream; AOwner: TDxControl; OffSetX: Integer = 0; OffSetY: Integer = 0);
var
  I: Integer;
  DxControl: TDxControl;
  GuiHeader: TGuiHeader;
  TreeNode: TTreeNode;
  ChildTreeNode: TTreeNode;
  sText: string;
begin
  if FileStream.Read(GuiHeader, SizeOf(TGuiHeader)) = SizeOf(TGuiHeader) then begin
    DxControl := NewDxControl(GuiHeader, AOwner);
    if DxControl <> nil then begin
      DxControl.Left := DxControl.Left + OffSetX;
      DxControl.Top := DxControl.Top + OffSetY;
      if GuiHeader.NameLen > 0 then begin
        SetLength(sText, GuiHeader.NameLen);
        FileStream.Read(sText[1], GuiHeader.NameLen);
        if not FindDxComponent(AOwner, sText) then
          DxControl.Name := sText;
      end;
      TreeNode := TTreeNode(AOwner.Data);
      ChildTreeNode := StructureDlg.TreeView.Items.AddChildObject(TreeNode, DxControl.Name, DxControl);
      DxControl.Data := ChildTreeNode;

      LoadComponent(FileStream, DxControl);

      for I := 0 to GuiHeader.Count - 1 do begin
        LoadSubComponent(FileStream, DxControl);
      end;
    end;
  end;
end;

procedure DxFontAssign(DxFont: TDxFont; GuiFont: TGuiFont);
begin
  DxFont.Color := GuiFont.Color;
  DxFont.BColor := GuiFont.BColor;
  DxFont.Size := GuiFont.Size;
  DxFont.Bold := GuiFont.Bold;
  DxFont.Style := GuiFont.Style;
end;

procedure GuiFontAssign(var GuiFont: TGuiFont; DxFont: TDxFont);
begin
  GuiFont.Color := DxFont.Color;
  GuiFont.BColor := DxFont.BColor;
  GuiFont.Size := DxFont.Size;
  GuiFont.Bold := DxFont.Bold;
  GuiFont.Style := DxFont.Style;
  GuiFont.NameLen := Length(DxFont.Name);
end;

function ReadGuiFontName(FileStream: TStream; AFont: TGuiFont): string;
var
  sText: string;
begin
  Result := '';
  if AFont.NameLen > 0 then begin
    SetLength(sText, AFont.NameLen);
    FileStream.Read(sText[1], AFont.NameLen);
    Result := sText;
  end;
end;

procedure WriteGuiFontName(FileStream: TStream; AFont: TDxFont);
var
  NameLen: Integer;
  sText: string;
begin
  NameLen := Length(AFont.Name);
  if NameLen > 0 then begin
    sText := AFont.Name;
    FileStream.Write(sText[1], NameLen);
  end;
end;

procedure TFrmMain.LoadComponent(FileStream: TStream; DxControl: TDxControl);
var
  Gui: TGuiType;
  sText: string;
  I: Integer;
  GuiImageForm: TGuiImageForm;
  GuiImageFormShape: TGuiImageFormShape;

  GuiImageButton: TGuiImageButton;
  GuiEdit: TGuiEdit;
  GuiLabel: TGuiLabel;
  GuiMemo: TGuiMemo;
  GuiImageGrid: TGuiImageGrid;
  GuiPopupMenu: TGuiPopupMenu;
  GuiComboBox: TGuiComboBox;
  GuiPageControl: TGuiPageControl;
  GuiTabSheet: TGuiTabSheet;
  GuiLine: TGuiLine;

  GuiViewField: TGuiViewField;
  ColRect: TRect;

  DxImageForm: TDxImageForm;
  DxImageFormShape: TDxImageFormShape;
  DxImageButton: TDxImageButton;
  DxPageControl: TDxPageControl;
  DxTabSheet: TDxTabSheet;

  DxEdit: TDxEdit;
  DxLabel: TDxLabel;
  DxScrollControl: TDxScrollControl;
  DxImageGrid: TDxImageGrid;
  DxPopupMenu: TDxPopupMenu;
  DxComboBox: TDxComboBox;

  DxLine: TDxLine;

  DxListView: TDxListView;
begin
  Gui := TGuiType(DxControl.Tag);
  case Gui of
    t_Form: begin
        FileStream.Read(GuiImageForm, SizeOf(TGuiImageForm));
        DxImageForm := TDxImageForm(DxControl);
        DxImageForm.AutoSize := GuiImageForm.AutoSize;
        DxImageForm.ImageIndex.ImageType := GuiImageForm.ImageIndex.Image;
        DxImageForm.ImageIndex.Up := GuiImageForm.ImageIndex.Up;
        DxImageForm.ImageIndex.Hot := GuiImageForm.ImageIndex.Hot;
        DxImageForm.ImageIndex.Down := GuiImageForm.ImageIndex.Down;
        DxImageForm.ImageIndex.Disabled := GuiImageForm.ImageIndex.Disabled;
        DxImageForm.Center := GuiImageForm.Center;

      end;
    t_FormShape: begin
        FileStream.Read(GuiImageFormShape, SizeOf(TGuiImageFormShape));
        DxImageFormShape := TDxImageFormShape(DxControl);
        DxImageFormShape.AutoSize := GuiImageFormShape.AutoSize;
        DxImageFormShape.ImageIndex.ImageType := GuiImageFormShape.ImageIndex.Image;
        DxImageFormShape.ImageIndex.Up := GuiImageFormShape.ImageIndex.Up;
        DxImageFormShape.ImageIndex.Hot := GuiImageFormShape.ImageIndex.Hot;
        DxImageFormShape.ImageIndex.Down := GuiImageFormShape.ImageIndex.Down;
        DxImageFormShape.ImageIndex.Disabled := GuiImageFormShape.ImageIndex.Disabled;
        DxImageFormShape.Center := GuiImageFormShape.Center;
        for I := 0 to DxImageFormShape.ImageCount - 1 do begin
          DxImageFormShape.Items[I].SourceRect := GuiImageFormShape.ImageIndexs[I].SrcRect;
          DxImageFormShape.Items[I].DestRect := GuiImageFormShape.ImageIndexs[I].DestRect;
          DxImageFormShape.Items[I].ImageType := GuiImageFormShape.ImageIndexs[I].ImageType;
          DxImageFormShape.Items[I].ImageIndex := GuiImageFormShape.ImageIndexs[I].ImageIndex;
          DxImageFormShape.Items[I].Align := GuiImageFormShape.ImageIndexs[I].Align;
          DxImageFormShape.Items[I].Draw := GuiImageFormShape.ImageIndexs[I].Draw;
          DxImageFormShape.Items[I].Stretch := GuiImageFormShape.ImageIndexs[I].Stretch;
          DxImageFormShape.Items[I].Center := GuiImageFormShape.ImageIndexs[I].Center;
          DxImageFormShape.Items[I].BlendMode := GuiImageFormShape.ImageIndexs[I].BlendMode;
        end;
      end;

    t_Button: begin
        FileStream.Read(GuiImageButton, SizeOf(TGuiImageButton));
        DxImageButton := TDxImageButton(DxControl);
        //with DxImageButton do begin
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
        //end;
        DxImageButton.CaptionColor.Up.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Up);
        DxImageButton.CaptionColor.Hot.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Hot);
        DxImageButton.CaptionColor.Down.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Down);
        DxImageButton.CaptionColor.Disabled.Name := ReadGuiFontName(FileStream, GuiImageButton.CaptionColor.Disabled);

        if GuiImageButton.CaptionLen > 0 then begin
          SetLength(sText, GuiImageButton.CaptionLen);
          FileStream.Read(sText[1], GuiImageButton.CaptionLen);
          DxImageButton.Caption := sText;
        end;
      end;
    t_Edit: begin
        FileStream.Read(GuiEdit, SizeOf(TGuiEdit));
        DxEdit := TDxEdit(DxControl);

        //with DxEdit do begin
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
        //end;
        DxEdit.Font.Name := ReadGuiFontName(FileStream, GuiEdit.FontColor);

        if GuiEdit.TextLen > 0 then begin
          SetLength(sText, GuiEdit.TextLen);
          FileStream.Read(sText[1], GuiEdit.TextLen);
          DxEdit.Text := sText;
        end;
      end;
    t_Label: begin
        FileStream.Read(GuiLabel, SizeOf(TGuiLabel));
        DxLabel := TDxLabel(DxControl);
        //with DxLabel do begin
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
        //end;

        DxLabel.CaptionColor.Up.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Up);
        DxLabel.CaptionColor.Hot.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Hot);
        DxLabel.CaptionColor.Down.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Down);
        DxLabel.CaptionColor.Disabled.Name := ReadGuiFontName(FileStream, GuiLabel.CaptionColor.Disabled);

        if GuiLabel.CaptionLen > 0 then begin
          SetLength(sText, GuiLabel.CaptionLen);
          FileStream.Read(sText[1], GuiLabel.CaptionLen);
          DxLabel.Caption := sText;
        end;
      end;

    t_Grid: begin
        FileStream.Read(GuiImageGrid, SizeOf(TGuiImageGrid));
        DxImageGrid := TDxImageGrid(DxControl);
        //with DxImageGrid do begin
        DxImageGrid.ColCount := GuiImageGrid.ColCount;
        DxImageGrid.RowCount := GuiImageGrid.RowCount;
        DxImageGrid.ColWidth := GuiImageGrid.ColWidth;
        DxImageGrid.RowHeight := GuiImageGrid.RowHeight;
        DxImageGrid.ViewTopLine := GuiImageGrid.ViewTopLine;
        //end;
      end;
    t_ScrollBox, t_ChatMemo, t_ListView, t_TreeView: begin
        FileStream.Read(GuiMemo, SizeOf(TGuiMemo));
        DxScrollControl := TDxScrollControl(DxControl);
       // with DxMemo do begin
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
            FileStream.Read(ColRect, SizeOf(TRect));
            DxListView.ColRects[I] := ColRect;
          end;

          for I := 0 to DxListView.ColCount - 1 do begin
            FileStream.Read(GuiViewField, SizeOf(TGuiViewField));
            DxListView.Fields[I].Alignment := GuiViewField.Alignment;
            DxFontAssign(DxListView.Fields[I].Color.Up, GuiViewField.Color.Up);
            DxFontAssign(DxListView.Fields[I].Color.Hot, GuiViewField.Color.Hot);
            DxFontAssign(DxListView.Fields[I].Color.Down, GuiViewField.Color.Down);
            DxFontAssign(DxListView.Fields[I].Color.Disabled, GuiViewField.Color.Disabled);

            DxListView.Fields[I].Color.Up.Name := ReadGuiFontName(FileStream, GuiViewField.Color.Up);
            DxListView.Fields[I].Color.Hot.Name := ReadGuiFontName(FileStream, GuiViewField.Color.Hot);
            DxListView.Fields[I].Color.Down.Name := ReadGuiFontName(FileStream, GuiViewField.Color.Down);
            DxListView.Fields[I].Color.Disabled.Name := ReadGuiFontName(FileStream, GuiViewField.Color.Disabled);

            if GuiViewField.CaptionLen > 0 then begin
              SetLength(sText, GuiViewField.CaptionLen);
              FileStream.Read(sText[1], GuiViewField.CaptionLen);
              DxListView.Fields[I].Caption := sText;
            end;
          end;

        end;

      end;

    t_PopupMenu: begin
        FileStream.Read(GuiPopupMenu, SizeOf(TGuiPopupMenu));
        DxPopupMenu := TDxPopupMenu(DxControl);
        //with DxPopupMenu do begin
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
        //end;

        DxPopupMenu.ItemColor.Up.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Up);
        DxPopupMenu.ItemColor.Hot.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Hot);
        DxPopupMenu.ItemColor.Down.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Down);
        DxPopupMenu.ItemColor.Disabled.Name := ReadGuiFontName(FileStream, GuiPopupMenu.ItemColor.Disabled);

        if GuiPopupMenu.ItemTextLen > 0 then begin
          SetLength(sText, GuiPopupMenu.ItemTextLen);
          FileStream.Read(sText[1], GuiPopupMenu.ItemTextLen);
          DxPopupMenu.Items.Text := sText;
        end;
      end;
    t_PageControl: begin
        FileStream.Read(GuiPageControl, SizeOf(TGuiPageControl));
        DxPageControl := TDxPageControl(DxControl);
        //with DxPageControl do begin
        DxPageControl.ClientLeft := GuiPageControl.ClientLeft;
        DxPageControl.ClientTop := GuiPageControl.ClientTop;
        DxPageControl.ClientWidth := GuiPageControl.ClientWidth;
        DxPageControl.ClientHeight := GuiPageControl.ClientHeight;
        DxPageControl.TabPosition := GuiPageControl.TabPosition;
        //DxPageControl.PageCount := 0;
          //ActivePageIndex := 0;
        DxPageControl.ButtonWidth := GuiPageControl.ButtonWidth;
        DxPageControl.ButtonHeight := GuiPageControl.ButtonHeight;
        DxPageControl.ShowButton := GuiPageControl.ShowButton;
        DxPageControl.OffSetX := GuiPageControl.OffSetX;
        DxPageControl.OffSetY := GuiPageControl.OffSetY;
        //end;
      end;
    t_ComboBox: begin
        FileStream.Read(GuiComboBox, SizeOf(TGuiComboBox));
        DxComboBox := TDxComboBox(DxControl);

        DxPopupMenu := TDxPopupMenu(DxComboBox.PopupMenu);

        //with DxPopupMenu do begin
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

        DxPopupMenu.ItemColor.Up.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Up);
        DxPopupMenu.ItemColor.Hot.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Hot);
        DxPopupMenu.ItemColor.Down.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Down);
        DxPopupMenu.ItemColor.Disabled.Name := ReadGuiFontName(FileStream, GuiComboBox.GuiPopupMenu.ItemColor.Disabled);

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
        //end;

        DxComboBox.TextColor.Up.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Up);
        DxComboBox.TextColor.Hot.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Hot);
        DxComboBox.TextColor.Down.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Down);
        DxComboBox.TextColor.Disabled.Name := ReadGuiFontName(FileStream, GuiComboBox.TextColor.Disabled);

        if GuiComboBox.TextLen > 0 then begin
          SetLength(sText, GuiComboBox.TextLen);
          FileStream.Read(sText[1], GuiComboBox.TextLen);
          DxComboBox.Text := sText;
        end;
        if GuiComboBox.ItemLen > 0 then begin
          SetLength(sText, GuiComboBox.ItemLen);
          FileStream.Read(sText[1], GuiComboBox.ItemLen);
          DxComboBox.Items.Text := sText;
        end;
      end;
    t_TabSheet: begin
        FileStream.Read(GuiTabSheet, SizeOf(TGuiTabSheet));
        DxTabSheet := TDxTabSheet(DxControl);
        DxTabSheet.OffSetX := GuiTabSheet.OffSetX;
        DxTabSheet.OffSetY := GuiTabSheet.OffSetY;
        DxTabSheet.CaptionDownOffsetX := GuiTabSheet.CaptionDownOffsetX;
        DxTabSheet.CaptionDownOffsetY := GuiTabSheet.CaptionDownOffsetY;
        //with DxTabSheet do begin
        DxTabSheet.Caption := '';
        DxTabSheet.ImageIndex.ImageType := GuiTabSheet.ImageIndex.Image;
        DxTabSheet.ImageIndex.Up := GuiTabSheet.ImageIndex.Up;
        DxTabSheet.ImageIndex.Hot := GuiTabSheet.ImageIndex.Hot;
        DxTabSheet.ImageIndex.Down := GuiTabSheet.ImageIndex.Down;
        DxTabSheet.ImageIndex.Disabled := GuiTabSheet.ImageIndex.Disabled;

          //BackgroundColor := GuiTabSheet.BackgroundColor;
        DxFontAssign(DxTabSheet.CaptionColor.Up, GuiTabSheet.CaptionColor.Up);
        DxFontAssign(DxTabSheet.CaptionColor.Hot, GuiTabSheet.CaptionColor.Hot);
        DxFontAssign(DxTabSheet.CaptionColor.Down, GuiTabSheet.CaptionColor.Down);
        DxFontAssign(DxTabSheet.CaptionColor.Disabled, GuiTabSheet.CaptionColor.Disabled);
        //end;

        DxTabSheet.CaptionColor.Up.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Up);
        DxTabSheet.CaptionColor.Hot.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Hot);
        DxTabSheet.CaptionColor.Down.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Down);
        DxTabSheet.CaptionColor.Disabled.Name := ReadGuiFontName(FileStream, GuiTabSheet.CaptionColor.Disabled);

        if GuiTabSheet.CaptionLen > 0 then begin
          SetLength(sText, GuiTabSheet.CaptionLen);
          FileStream.Read(sText[1], GuiTabSheet.CaptionLen);
          DxTabSheet.Caption := sText;
        end;
        TDxPageControl(DxTabSheet.Owner).ActivePageIndex := 0;
      end;
    t_Line: begin
        FileStream.Read(GuiLine, SizeOf(TGuiLine));
        DxLine := TDxLine(DxControl);
        DxLine.Style := GuiLine.LineStyle;
        DxFontAssign(DxLine.LineColor.Up, GuiLine.LineColor.Up);
        DxFontAssign(DxLine.LineColor.Hot, GuiLine.LineColor.Hot);
        DxFontAssign(DxLine.LineColor.Down, GuiLine.LineColor.Down);
        DxFontAssign(DxLine.LineColor.Disabled, GuiLine.LineColor.Disabled);
      end;

    //t_CharMemo: ;
  end;
end;

procedure TFrmMain.TabSheetCreate(Sender: TObject);
var
  TreeNode, ChildTreeNode: TTreeNode;
  DxControl: TDxControl;
begin
  DxControl := TDxControl(Sender);
  TreeNode := TTreeNode(TDxControl(DxControl.Owner).Data);
  ChildTreeNode := StructureDlg.TreeView.Items.AddChildObject(TreeNode, DxControl.Name, DxControl);
  DxControl.Data := ChildTreeNode;
end;

procedure TFrmMain.TabSheetDestroy(Sender: TObject);
var
  TreeNode: TTreeNode;
  DxControl: TDxControl;
begin
  DxControl := TDxControl(Sender);
  TreeNode := TTreeNode(DxControl.Data);
  StructureDlg.TreeView.Items.Delete(TreeNode);
  //ObjectsDlg.JvInspector.Clear;
end;

function TFrmMain.NewDxControl(GuiHeader: TGuiHeader; AOwner: TDxControl): TDxControl;
var
  DxControl: TDxControl;
begin
  DxControl := nil;
  case GuiHeader.Gui of
    t_Form: DxControl := TDxImageForm.Create(AOwner);
    t_FormShape: DxControl := TDxImageFormShape.Create(AOwner);
    t_Button: DxControl := TDxImageButton.Create(AOwner);
    t_Edit: DxControl := TDxEdit.Create(AOwner);
    t_Label: DxControl := TDxLabel.Create(AOwner);
    t_Grid: DxControl := TDxImageGrid.Create(AOwner);
    t_ScrollBox: DxControl := TDxScrollBox.Create(AOwner);

    t_ChatMemo: DxControl := TDxChatMemo.Create(AOwner);
    t_ListView: DxControl := TDxListView.Create(AOwner);
    t_TreeView: DxControl := TDxTreeView.Create(AOwner);
    t_PopupMenu: DxControl := TDxPopupMenu.Create(AOwner);
    t_TabSheet: DxControl := TDxTabSheet.Create(AOwner);
    t_PageControl: DxControl := TDxPageControl.Create(AOwner);
    t_ComboBox: DxControl := TDxComboBox.Create(AOwner);
    t_Line: DxControl := TDxLine.Create(AOwner);
  end;
  if DxControl <> nil then begin
    //with DxControl do begin
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
    DxControl.MouseEvents := GuiHeader.MouseEvents;
    DxControl.OnClick := OnDxControlClick;
    DxControl.OnMouseDown := OnDxControlMouseDown;
    DxControl.OnDblClick := OnDxControlDblClick;
    DxControl.OnGetImage := OnDxControlGetImage;
    DxControl.Tag := Integer(GuiHeader.Gui);
    //DxControl.ImageType := GuiHeader.Image;
    if GuiHeader.Gui = t_PageControl then begin
      TDxPageControl(DxControl).OnTabSheetCreate := TabSheetCreate;
      TDxPageControl(DxControl).OnTabSheetDestroy := TabSheetDestroy;
    end;
    //end;
  end;
  Result := DxControl;
end;

procedure TFrmMain.SaveComponent(FileStream: TStream; DxControl: TDxControl);
var
  Gui: TGuiType;
  sText: string;
  I: Integer;
  GuiImageForm: TGuiImageForm;
  GuiImageFormShape: TGuiImageFormShape;
  GuiImageButton: TGuiImageButton;
  GuiEdit: TGuiEdit;
  GuiLabel: TGuiLabel;
  GuiMemo: TGuiMemo;
  GuiImageGrid: TGuiImageGrid;
  GuiPopupMenu: TGuiPopupMenu;
  GuiComboBox: TGuiComboBox;
  GuiPageControl: TGuiPageControl;
  GuiTabSheet: TGuiTabSheet;
  GuiLine: TGuiLine;
  GuiViewField: TGuiViewField;

  ColRect: TRect;

  DxImageForm: TDxImageForm;
  DxImageFormShape: TDxImageFormShape;
  DxImageButton: TDxImageButton;
  DxPageControl: TDxPageControl;
  DxTabSheet: TDxTabSheet;

  DxEdit: TDxEdit;
  DxLabel: TDxLabel;
  DxScrollControl: TDxScrollControl;
  DxImageGrid: TDxImageGrid;
  DxPopupMenu: TDxPopupMenu;
  DxComboBox: TDxComboBox;
  DxLine: TDxLine;



  DxListView: TDxListView;
begin
  Inc(DxControlCount);
  Gui := TGuiType(DxControl.Tag);
  //if DxControl.ImageIndex.ImageType = UI2_wil then DxControl.ImageIndex.ImageType:=Prguse2_wis;
  case Gui of
    t_Form: begin
        FillChar(GuiImageForm, SizeOf(GuiImageForm), 0);
        GuiImageForm.ImageIndex.Image := TDxImageForm(DxControl).ImageIndex.ImageType;
        GuiImageForm.ImageIndex.Up := TDxImageForm(DxControl).ImageIndex.Up;
        GuiImageForm.ImageIndex.Hot := TDxImageForm(DxControl).ImageIndex.Hot;
        GuiImageForm.ImageIndex.Down := TDxImageForm(DxControl).ImageIndex.Down;
        GuiImageForm.ImageIndex.Disabled := TDxImageForm(DxControl).ImageIndex.Disabled;
        GuiImageForm.AutoSize := TDxImageForm(DxControl).AutoSize;
        GuiImageForm.Center := TDxImageForm(DxControl).Center;
        FileStream.Write(GuiImageForm, SizeOf(TGuiImageForm));
      end;
    t_FormShape: begin
        FillChar(GuiImageFormShape, SizeOf(GuiImageFormShape), 0);
        GuiImageFormShape.ImageIndex.Image := TDxImageFormShape(DxControl).ImageIndex.ImageType;
        GuiImageFormShape.ImageIndex.Up := TDxImageFormShape(DxControl).ImageIndex.Up;
        GuiImageFormShape.ImageIndex.Hot := TDxImageFormShape(DxControl).ImageIndex.Hot;
        GuiImageFormShape.ImageIndex.Down := TDxImageFormShape(DxControl).ImageIndex.Down;
        GuiImageFormShape.ImageIndex.Disabled := TDxImageFormShape(DxControl).ImageIndex.Disabled;
        GuiImageFormShape.AutoSize := TDxImageFormShape(DxControl).AutoSize;
        GuiImageFormShape.Center := TDxImageFormShape(DxControl).Center;
        for I := 0 to TDxImageFormShape(DxControl).ImageCount - 1 do begin
          GuiImageFormShape.ImageIndexs[I].SrcRect := TDxImageFormShape(DxControl).Items[I].SourceRect;
          GuiImageFormShape.ImageIndexs[I].DestRect := TDxImageFormShape(DxControl).Items[I].DestRect;
          GuiImageFormShape.ImageIndexs[I].ImageType := TDxImageFormShape(DxControl).Items[I].ImageType;
          GuiImageFormShape.ImageIndexs[I].ImageIndex := TDxImageFormShape(DxControl).Items[I].ImageIndex;
          GuiImageFormShape.ImageIndexs[I].Align := TDxImageFormShape(DxControl).Items[I].Align;
          GuiImageFormShape.ImageIndexs[I].Draw := TDxImageFormShape(DxControl).Items[I].Draw;
          GuiImageFormShape.ImageIndexs[I].Stretch := TDxImageFormShape(DxControl).Items[I].Stretch;
          GuiImageFormShape.ImageIndexs[I].Center := TDxImageFormShape(DxControl).Items[I].Center;
          GuiImageFormShape.ImageIndexs[I].BlendMode := TDxImageFormShape(DxControl).Items[I].BlendMode;
        end;
        FileStream.Write(GuiImageFormShape, SizeOf(TGuiImageFormShape));
      end;
    t_Button: begin
        DxImageButton := TDxImageButton(DxControl);
        FillChar(GuiImageButton, SizeOf(GuiImageButton), 0);
        GuiImageButton.Alignment := DxImageButton.Alignment;
        GuiImageButton.CaptionDownOffsetX := DxImageButton.CaptionDownOffsetX;
        GuiImageButton.CaptionDownOffsetY := DxImageButton.CaptionDownOffsetY;
        GuiImageButton.ImageIndex.Image := DxImageButton.ImageIndex.ImageType;
        GuiImageButton.ImageIndex.Up := DxImageButton.ImageIndex.Up;
        GuiImageButton.ImageIndex.Hot := DxImageButton.ImageIndex.Hot;
        GuiImageButton.ImageIndex.Down := DxImageButton.ImageIndex.Down;
        GuiImageButton.ImageIndex.Disabled := DxImageButton.ImageIndex.Disabled;

        GuiImageButton.AutoSize := DxImageButton.AutoSize;

        GuiFontAssign(GuiImageButton.CaptionColor.Up, DxImageButton.CaptionColor.Up);
        GuiFontAssign(GuiImageButton.CaptionColor.Hot, DxImageButton.CaptionColor.Hot);
        GuiFontAssign(GuiImageButton.CaptionColor.Down, DxImageButton.CaptionColor.Down);
        GuiFontAssign(GuiImageButton.CaptionColor.Disabled, DxImageButton.CaptionColor.Disabled);

        GuiImageButton.Checked := DxImageButton.Checked;
        GuiImageButton.ClickCount := DxImageButton.ClickCount;
        GuiImageButton.Style := DxImageButton.Style;
        GuiImageButton.CaptionLen := Length(DxImageButton.Caption);
        //end;
        FileStream.Write(GuiImageButton, SizeOf(TGuiImageButton));

        WriteGuiFontName(FileStream, DxImageButton.CaptionColor.Up);
        WriteGuiFontName(FileStream, DxImageButton.CaptionColor.Hot);
        WriteGuiFontName(FileStream, DxImageButton.CaptionColor.Down);
        WriteGuiFontName(FileStream, DxImageButton.CaptionColor.Disabled);

        if GuiImageButton.CaptionLen > 0 then begin
          sText := DxImageButton.Caption;
          FileStream.Write(sText[1], GuiImageButton.CaptionLen);
        end;
      end;
    t_Edit: begin
        DxEdit := TDxEdit(DxControl);
        FillChar(GuiEdit, SizeOf(GuiEdit), 0);
        GuiEdit.BackgroundColor := DxEdit.BackgroundColor;
        GuiEdit.DrawBorder := DxEdit.DrawBorder;
        GuiFontAssign(GuiEdit.FontColor, DxEdit.Font);

        GuiFontAssign(GuiEdit.BorderColor.Up, DxEdit.BorderColor.Up);
        GuiFontAssign(GuiEdit.BorderColor.Hot, DxEdit.BorderColor.Hot);
        GuiFontAssign(GuiEdit.BorderColor.Down, DxEdit.BorderColor.Down);
        GuiFontAssign(GuiEdit.BorderColor.Disabled, DxEdit.BorderColor.Disabled);

        GuiEdit.ReadOnly := DxEdit.ReadOnly;
        GuiEdit.MaxLength := DxEdit.MaxLength;
        GuiEdit.SelectedColor := DxEdit.SelectedColor;
        GuiEdit.SelBackColor := DxEdit.SelBackColor;
        GuiEdit.SelFontColor := DxEdit.SelFontColor;
        GuiEdit.InValue := DxEdit.InValue;
        GuiEdit.PasswordChar := DxEdit.PasswordChar;
        GuiEdit.AllowSelect := DxEdit.AllowSelect;
        GuiEdit.AllowPaste := DxEdit.AllowPaste;
        GuiEdit.TabOrder := DxEdit.TabOrder;
        //end;
        sText := DxEdit.Text;
        GuiEdit.TextLen := Length(sText);
        FileStream.Write(GuiEdit, SizeOf(TGuiEdit));

        WriteGuiFontName(FileStream, DxEdit.Font);

        if GuiEdit.TextLen > 0 then begin
          sText := DxEdit.Text;
          FileStream.Write(sText[1], GuiEdit.TextLen);
        end;
      end;
    t_Label: begin
        DxLabel := TDxLabel(DxControl);
        FillChar(GuiLabel, SizeOf(GuiLabel), 0);
        GuiLabel.AutoSize := DxLabel.AutoSize;
        GuiLabel.BackgroundColor := DxLabel.BackgroundColor;
        GuiLabel.DrawBorder := DxLabel.DrawBorder;
        GuiLabel.CaptionDownOffsetX := DxLabel.CaptionDownOffsetX;
        GuiLabel.CaptionDownOffsetY := DxLabel.CaptionDownOffsetY;
        GuiFontAssign(GuiLabel.CaptionColor.Up, DxLabel.CaptionColor.Up);
        GuiFontAssign(GuiLabel.CaptionColor.Hot, DxLabel.CaptionColor.Hot);
        GuiFontAssign(GuiLabel.CaptionColor.Down, DxLabel.CaptionColor.Down);
        GuiFontAssign(GuiLabel.CaptionColor.Disabled, DxLabel.CaptionColor.Disabled);

        GuiFontAssign(GuiLabel.BorderColor.Up, DxLabel.BorderColor.Up);
        GuiFontAssign(GuiLabel.BorderColor.Hot, DxLabel.BorderColor.Hot);
        GuiFontAssign(GuiLabel.BorderColor.Down, DxLabel.BorderColor.Down);
        GuiFontAssign(GuiLabel.BorderColor.Disabled, DxLabel.BorderColor.Disabled);

        GuiLabel.ClickCount := DxLabel.ClickCount;
        GuiLabel.Style := DxLabel.Style;

        GuiLabel.CaptionLen := Length(DxLabel.Caption);
        //end;
        FileStream.Write(GuiLabel, SizeOf(TGuiLabel));

        WriteGuiFontName(FileStream, DxLabel.CaptionColor.Up);
        WriteGuiFontName(FileStream, DxLabel.CaptionColor.Hot);
        WriteGuiFontName(FileStream, DxLabel.CaptionColor.Down);
        WriteGuiFontName(FileStream, DxLabel.CaptionColor.Disabled);

        if GuiLabel.CaptionLen > 0 then begin
          sText := DxLabel.Caption;
          FileStream.Write(sText[1], GuiLabel.CaptionLen);
        end;
      end;

    t_Grid: begin
        DxImageGrid := TDxImageGrid(DxControl);
        FillChar(GuiImageGrid, SizeOf(GuiImageGrid), 0);
        GuiImageGrid.ColCount := DxImageGrid.ColCount;
        GuiImageGrid.RowCount := DxImageGrid.RowCount;
        GuiImageGrid.ColWidth := DxImageGrid.ColWidth;
        GuiImageGrid.RowHeight := DxImageGrid.RowHeight;
        GuiImageGrid.ViewTopLine := DxImageGrid.ViewTopLine;

        FileStream.Write(GuiImageGrid, SizeOf(TGuiImageGrid));
      end;
    t_ScrollBox, t_ChatMemo, t_ListView, t_TreeView: begin
        DxScrollControl := TDxScrollControl(DxControl);
        DxScrollControl.First;
        FillChar(GuiMemo, SizeOf(GuiMemo), 0);
        GuiMemo.ImageIndex.Image := DxScrollControl.ImageIndex.ImageType;
        GuiMemo.ImageIndex.Up := DxScrollControl.ImageIndex.Up;
        GuiMemo.ImageIndex.Hot := DxScrollControl.ImageIndex.Hot;
        GuiMemo.ImageIndex.Down := DxScrollControl.ImageIndex.Down;
        GuiMemo.ImageIndex.Disabled := DxScrollControl.ImageIndex.Disabled;


        GuiMemo.ScrollImageIndex.Image := DxScrollControl.ScrollImageIndex.ImageType;
        GuiMemo.ScrollImageIndex.Up := DxScrollControl.ScrollImageIndex.Up;
        GuiMemo.ScrollImageIndex.Hot := DxScrollControl.ScrollImageIndex.Hot;
        GuiMemo.ScrollImageIndex.Down := DxScrollControl.ScrollImageIndex.Down;
        GuiMemo.ScrollImageIndex.Disabled := DxScrollControl.ScrollImageIndex.Disabled;

        GuiMemo.PrevImageIndex.Image := DxScrollControl.PrevImageIndex.ImageType;
        GuiMemo.PrevImageIndex.Up := DxScrollControl.PrevImageIndex.Up;
        GuiMemo.PrevImageIndex.Hot := DxScrollControl.PrevImageIndex.Hot;
        GuiMemo.PrevImageIndex.Down := DxScrollControl.PrevImageIndex.Down;
        GuiMemo.PrevImageIndex.Disabled := DxScrollControl.PrevImageIndex.Disabled;

        GuiMemo.NextImageIndex.Image := DxScrollControl.NextImageIndex.ImageType;
        GuiMemo.NextImageIndex.Up := DxScrollControl.NextImageIndex.Up;
        GuiMemo.NextImageIndex.Hot := DxScrollControl.NextImageIndex.Hot;
        GuiMemo.NextImageIndex.Down := DxScrollControl.NextImageIndex.Down;
        GuiMemo.NextImageIndex.Disabled := DxScrollControl.NextImageIndex.Disabled;

        GuiMemo.BarImageIndex.Image := DxScrollControl.BarImageIndex.ImageType;
        GuiMemo.BarImageIndex.Up := DxScrollControl.BarImageIndex.Up;
        GuiMemo.BarImageIndex.Hot := DxScrollControl.BarImageIndex.Hot;
        GuiMemo.BarImageIndex.Down := DxScrollControl.BarImageIndex.Down;
        GuiMemo.BarImageIndex.Disabled := DxScrollControl.BarImageIndex.Disabled;

        GuiMemo.ShowScroll := DxScrollControl.ShowScroll;
        GuiMemo.ItemHeight := DxScrollControl.ItemHeight;
        GuiMemo.ItemIndex := DxScrollControl.ItemIndex;
        GuiMemo.ScrollBars := DxScrollControl.ScrollBars;
        GuiMemo.ScrollSize := DxScrollControl.ScrollSize;

        GuiMemo.ExpandSize := DxScrollControl.ExpandSize;
        GuiMemo.Position := DxScrollControl.Position;
        GuiMemo.VisibleItemCount := DxScrollControl.VisibleItemCount;
        GuiMemo.OffSetX := DxScrollControl.OffSetX;
        GuiMemo.OffSetY := DxScrollControl.OffSetY;
        GuiMemo.ShowButton := False;
        GuiMemo.ShowItemCount := DxScrollControl.ShowItemCount;

        if DxScrollControl is TDxTreeView then
          GuiMemo.ShowButton := TDxTreeView(DxScrollControl).ShowButton;

        if DxScrollControl is TDxListView then begin
          GuiMemo.ColCount := TDxListView(DxScrollControl).ColCount;
          GuiMemo.ShowGridLine := TDxListView(DxScrollControl).ShowGridLine;
          GuiMemo.GridLineColor := TDxListView(DxScrollControl).GridLineColor;
          GuiMemo.CheckItemControlSize := TDxListView(DxScrollControl).CheckItemControlSize;

        end;
        FileStream.Write(GuiMemo, SizeOf(TGuiMemo));

        if DxScrollControl is TDxListView then begin
          DxListView := TDxListView(DxScrollControl);
          for I := 0 to DxListView.ColCount - 1 do begin
            ColRect := DxListView.ColRects[I];
            FileStream.Write(ColRect, SizeOf(TRect));
          end;

          for I := 0 to DxListView.ColCount - 1 do begin
            FillChar(GuiViewField, SizeOf(GuiViewField), 0);
            GuiFontAssign(GuiViewField.Color.Up, DxListView.Fields[I].Color.Up);
            GuiFontAssign(GuiViewField.Color.Hot, DxListView.Fields[I].Color.Hot);
            GuiFontAssign(GuiViewField.Color.Down, DxListView.Fields[I].Color.Down);
            GuiFontAssign(GuiViewField.Color.Disabled, DxListView.Fields[I].Color.Disabled);
            GuiViewField.Alignment := DxListView.Fields[I].Alignment;
            GuiViewField.CaptionLen := Length(DxListView.Fields[I].Caption);
            FileStream.Write(GuiViewField, SizeOf(TGuiViewField));

            WriteGuiFontName(FileStream, DxListView.Fields[I].Color.Up);
            WriteGuiFontName(FileStream, DxListView.Fields[I].Color.Hot);
            WriteGuiFontName(FileStream, DxListView.Fields[I].Color.Down);
            WriteGuiFontName(FileStream, DxListView.Fields[I].Color.Disabled);


            if GuiViewField.CaptionLen > 0 then begin
              sText := DxListView.Fields[I].Caption;
              FileStream.Write(sText[1], GuiViewField.CaptionLen);
            end;
          end;
        end;

      end;

    t_PopupMenu: begin
        DxPopupMenu := TDxPopupMenu(DxControl);
        FillChar(GuiPopupMenu, SizeOf(GuiPopupMenu), 0);
        GuiPopupMenu.BackgroundColor := DxPopupMenu.BackgroundColor;
        GuiPopupMenu.DrawBorder := DxPopupMenu.DrawBorder;

        GuiFontAssign(GuiPopupMenu.ItemColor.Up, DxPopupMenu.ItemColor.Up);
        GuiFontAssign(GuiPopupMenu.ItemColor.Hot, DxPopupMenu.ItemColor.Hot);
        GuiFontAssign(GuiPopupMenu.ItemColor.Down, DxPopupMenu.ItemColor.Down);
        GuiFontAssign(GuiPopupMenu.ItemColor.Disabled, DxPopupMenu.ItemColor.Disabled);

        GuiFontAssign(GuiPopupMenu.BorderColor.Up, DxPopupMenu.BorderColor.Up);
        GuiFontAssign(GuiPopupMenu.BorderColor.Hot, DxPopupMenu.BorderColor.Hot);
        GuiFontAssign(GuiPopupMenu.BorderColor.Down, DxPopupMenu.BorderColor.Down);
        GuiFontAssign(GuiPopupMenu.BorderColor.Disabled, DxPopupMenu.BorderColor.Disabled);

        GuiPopupMenu.SelectColor := DxPopupMenu.SelectColor;
        GuiPopupMenu.ItemHeight := DxPopupMenu.ItemHeight;
        GuiPopupMenu.ItemIndex := DxPopupMenu.ItemIndex;
        GuiPopupMenu.ItemTextLen := Length(DxPopupMenu.Items.Text);

        FileStream.Write(GuiPopupMenu, SizeOf(TGuiPopupMenu));

        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Up);
        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Hot);
        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Down);
        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Disabled);

        if GuiPopupMenu.ItemTextLen > 0 then begin
          sText := DxPopupMenu.Items.Text;
          FileStream.Write(sText[1], GuiPopupMenu.ItemTextLen);
        end;

      end;
    t_PageControl: begin
        DxPageControl := TDxPageControl(DxControl);
        FillChar(GuiPageControl, SizeOf(GuiPageControl), 0);
        GuiPageControl.ShowButton := DxPageControl.ShowButton;
        GuiPageControl.ClientLeft := DxPageControl.ClientLeft;
        GuiPageControl.ClientTop := DxPageControl.ClientTop;
        GuiPageControl.ClientWidth := DxPageControl.ClientWidth;
        GuiPageControl.ClientHeight := DxPageControl.ClientHeight;
        GuiPageControl.TabPosition := DxPageControl.TabPosition;
        GuiPageControl.PageCount := DxPageControl.PageCount;
        GuiPageControl.ActivePageIndex := DxPageControl.ActivePageIndex;
        GuiPageControl.ButtonWidth := DxPageControl.ButtonWidth;
        GuiPageControl.ButtonHeight := DxPageControl.ButtonHeight;
        GuiPageControl.OffSetX := DxPageControl.OffSetX;
        GuiPageControl.OffSetY := DxPageControl.OffSetY;
        FileStream.Write(GuiPageControl, SizeOf(TGuiPageControl));
      end;
    t_TabSheet: begin
        DxTabSheet := TDxTabSheet(DxControl);
        FillChar(GuiTabSheet, SizeOf(GuiTabSheet), 0);
        GuiTabSheet.OffSetX := DxTabSheet.OffSetX;
        GuiTabSheet.OffSetY := DxTabSheet.OffSetY;
        GuiTabSheet.CaptionDownOffsetX := DxTabSheet.CaptionDownOffsetX;
        GuiTabSheet.CaptionDownOffsetY := DxTabSheet.CaptionDownOffsetY;
        GuiTabSheet.ImageIndex.Image := DxTabSheet.ImageIndex.ImageType;
        GuiTabSheet.ImageIndex.Up := DxTabSheet.ImageIndex.Up;
        GuiTabSheet.ImageIndex.Hot := DxTabSheet.ImageIndex.Hot;
        GuiTabSheet.ImageIndex.Down := DxTabSheet.ImageIndex.Down;
        GuiTabSheet.ImageIndex.Disabled := DxTabSheet.ImageIndex.Disabled;

        GuiFontAssign(GuiTabSheet.CaptionColor.Up, DxTabSheet.CaptionColor.Up);
        GuiFontAssign(GuiTabSheet.CaptionColor.Hot, DxTabSheet.CaptionColor.Hot);
        GuiFontAssign(GuiTabSheet.CaptionColor.Down, DxTabSheet.CaptionColor.Down);
        GuiFontAssign(GuiTabSheet.CaptionColor.Disabled, DxTabSheet.CaptionColor.Disabled);

        GuiTabSheet.CaptionLen := Length(DxTabSheet.Caption);

        FileStream.Write(GuiTabSheet, SizeOf(TGuiTabSheet));

        WriteGuiFontName(FileStream, DxTabSheet.CaptionColor.Up);
        WriteGuiFontName(FileStream, DxTabSheet.CaptionColor.Hot);
        WriteGuiFontName(FileStream, DxTabSheet.CaptionColor.Down);
        WriteGuiFontName(FileStream, DxTabSheet.CaptionColor.Disabled);

        if GuiTabSheet.CaptionLen > 0 then begin
          sText := DxTabSheet.Caption;
          FileStream.Write(sText[1], GuiTabSheet.CaptionLen);
        end;

      end;
    t_ComboBox: begin
        DxComboBox := TDxComboBox(DxControl);

        DxPopupMenu := TDxPopupMenu(DxComboBox.PopupMenu);
        FillChar(GuiComboBox, SizeOf(GuiComboBox), 0);
        GuiComboBox.GuiPopupMenu.BackgroundColor := DxPopupMenu.BackgroundColor;
        GuiComboBox.GuiPopupMenu.DrawBorder := DxPopupMenu.DrawBorder;

        GuiFontAssign(GuiComboBox.GuiPopupMenu.ItemColor.Up, DxPopupMenu.ItemColor.Up);
        GuiFontAssign(GuiComboBox.GuiPopupMenu.ItemColor.Hot, DxPopupMenu.ItemColor.Hot);
        GuiFontAssign(GuiComboBox.GuiPopupMenu.ItemColor.Down, DxPopupMenu.ItemColor.Down);
        GuiFontAssign(GuiComboBox.GuiPopupMenu.ItemColor.Disabled, DxPopupMenu.ItemColor.Disabled);

        GuiFontAssign(GuiComboBox.GuiPopupMenu.BorderColor.Up, DxPopupMenu.BorderColor.Up);
        GuiFontAssign(GuiComboBox.GuiPopupMenu.BorderColor.Hot, DxPopupMenu.BorderColor.Hot);
        GuiFontAssign(GuiComboBox.GuiPopupMenu.BorderColor.Down, DxPopupMenu.BorderColor.Down);
        GuiFontAssign(GuiComboBox.GuiPopupMenu.BorderColor.Disabled, DxPopupMenu.BorderColor.Disabled);

        GuiComboBox.GuiPopupMenu.SelectColor := DxPopupMenu.SelectColor;
        GuiComboBox.GuiPopupMenu.ItemHeight := DxPopupMenu.ItemHeight;
        GuiComboBox.GuiPopupMenu.ItemIndex := DxPopupMenu.ItemIndex;
        GuiComboBox.GuiPopupMenu.ItemTextLen := 0;

        GuiComboBox.BackgroundColor := DxComboBox.BackgroundColor;
        GuiComboBox.DrawBorder := DxComboBox.DrawBorder;
        GuiComboBox.ButtonColor := DxComboBox.ButtonColor;

        GuiFontAssign(GuiComboBox.TextColor.Up, DxComboBox.TextColor.Up);
        GuiFontAssign(GuiComboBox.TextColor.Hot, DxComboBox.TextColor.Hot);
        GuiFontAssign(GuiComboBox.TextColor.Down, DxComboBox.TextColor.Down);
        GuiFontAssign(GuiComboBox.TextColor.Disabled, DxComboBox.TextColor.Disabled);

        GuiFontAssign(GuiComboBox.BorderColor.Up, DxComboBox.BorderColor.Up);
        GuiFontAssign(GuiComboBox.BorderColor.Hot, DxComboBox.BorderColor.Hot);
        GuiFontAssign(GuiComboBox.BorderColor.Down, DxComboBox.BorderColor.Down);
        GuiFontAssign(GuiComboBox.BorderColor.Disabled, DxComboBox.BorderColor.Disabled);

        GuiComboBox.TextLen := Length(DxComboBox.Text);
        GuiComboBox.ItemLen := Length(DxComboBox.Items.Text);

        FileStream.Write(GuiComboBox, SizeOf(TGuiComboBox));

        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Up);
        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Hot);
        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Down);
        WriteGuiFontName(FileStream, DxPopupMenu.ItemColor.Disabled);

        WriteGuiFontName(FileStream, DxComboBox.TextColor.Up);
        WriteGuiFontName(FileStream, DxComboBox.TextColor.Hot);
        WriteGuiFontName(FileStream, DxComboBox.TextColor.Down);
        WriteGuiFontName(FileStream, DxComboBox.TextColor.Disabled);

        if GuiComboBox.TextLen > 0 then begin
          sText := DxComboBox.Text;
          FileStream.Write(sText[1], GuiComboBox.TextLen);
        end;
        if GuiComboBox.ItemLen > 0 then begin
          sText := DxComboBox.Items.Text;
          FileStream.Write(sText[1], GuiComboBox.ItemLen);
        end;
      end;
    t_Line: begin
        DxLine := TDxLine(DxControl);
        FillChar(GuiLine, SizeOf(GuiLine), 0);
        GuiLine.LineStyle := DxLine.Style;
        GuiFontAssign(GuiLine.LineColor.Up, DxLine.LineColor.Up);
        GuiFontAssign(GuiLine.LineColor.Hot, DxLine.LineColor.Hot);
        GuiFontAssign(GuiLine.LineColor.Down, DxLine.LineColor.Down);
        GuiFontAssign(GuiLine.LineColor.Disabled, DxLine.LineColor.Disabled);
        FileStream.Write(GuiLine, SizeOf(TGuiLine));
      end;
    {t_CharMemo: begin

      end;}
  end;
end;

procedure TFrmMain.SaveSubComponent(FileStream: TStream; DxControl: TDxControl);
var
  I: Integer;
  sText: string;
  GuiHeader: TGuiHeader;
begin
  g_SaveComponentList.AddObject(DxControl.Name, DxControl);
  FillChar(GuiHeader, SizeOf(GuiHeader), 0);
  GuiHeader.Gui := TGuiType(DxControl.Tag);
  GuiHeader.Left := DxControl.Left;
  GuiHeader.Top := DxControl.Top;
  GuiHeader.Width := DxControl.Width;
  GuiHeader.Height := DxControl.Height;
  GuiHeader.Enabled := DxControl.Enabled;
  GuiHeader.Visible := DxControl.Visible;
  GuiHeader.Transparent := DxControl.Transparent;
  GuiHeader.EnableFocus := DxControl.EnableFocus;
  GuiHeader.Floating := DxControl.Floating;
  GuiHeader.OwnerMove := DxControl.OwnerMove;
  GuiHeader.MouseEvents := DxControl.MouseEvents;
  GuiHeader.NameLen := Length(DxControl.Name);
  GuiHeader.Background := 0;
  GuiHeader.Count := DxControl.ControlCount;
  FileStream.Write(GuiHeader, SizeOf(TGuiHeader));
  if GuiHeader.NameLen > 0 then begin
    sText := DxControl.Name;
    FileStream.Write(sText[1], GuiHeader.NameLen);
  end;

  SaveComponent(FileStream, DxControl);

  for I := 0 to DxControl.ControlCount - 1 do begin
    SaveSubComponent(FileStream, DxControl.Control[I]);
  end;
end;

procedure TFrmMain.LoadControl(FileStream: TStream; DxControl: TDxControl; OffSetX: Integer = 0; OffSetY: Integer = 0);
begin
  LoadSubComponent(FileStream, DxControl, OffSetX, OffSetY);
end;

procedure TFrmMain.SaveControl(FileStream: TStream; DxControl: TDxControl);
var
  I: Integer;
  sText: string;
  GuiHeader: TGuiHeader;
begin
  FillChar(GuiHeader, SizeOf(GuiHeader), 0);
  GuiHeader.Gui := TGuiType(DxControl.Tag);
  GuiHeader.Left := DxControl.Left;
  GuiHeader.Top := DxControl.Top;
  GuiHeader.Width := DxControl.Width;
  GuiHeader.Height := DxControl.Height;
  GuiHeader.Enabled := DxControl.Enabled;
  GuiHeader.Visible := DxControl.Visible;
  GuiHeader.Transparent := DxControl.Transparent;
  GuiHeader.EnableFocus := DxControl.EnableFocus;
  GuiHeader.Floating := DxControl.Floating;
  GuiHeader.OwnerMove := DxControl.OwnerMove;
  GuiHeader.MouseEvents := DxControl.MouseEvents;
  GuiHeader.NameLen := Length(DxControl.Name);
  //GuiHeader.Background := Integer(Background);
  GuiHeader.Count := DxControl.ComponentCount;
  FileStream.Write(GuiHeader, SizeOf(TGuiHeader));

  if GuiHeader.NameLen > 0 then begin
    sText := DxControl.Name;
    FileStream.Write(sText[1], GuiHeader.NameLen);
  end;

  SaveComponent(FileStream, DxControl);

  for I := 0 to DxControl.ControlCount - 1 do begin
    SaveSubComponent(FileStream, DxControl.Control[I]);
  end;
  FileStream.Size := FileStream.Position;
end;

procedure TFrmMain.SaveToFile(FileName: string);
  function ExtractFileNameOnly(const fname: string): string;
  var
    extpos: Integer;
    ext, fn: string;
  begin
    ext := ExtractFileExt(fname);
    fn := ExtractFileName(fname);
    if ext <> '' then begin
      extpos := Pos(ext, fn);
      Result := Copy(fn, 1, extpos - 1);
    end else
      Result := fn;
  end;
var
  ComponentList: TList;
  I, II, III: Integer;

  FileStream: TFileStream;
  TreeNode: TTreeNode;
  sText: string;
  Background: TDxControl;
  DxControl: TDxControl;
  GuiHeader: TGuiHeader;
  FileHeader: TGuiFileHeader;

  StringList: TStringList;
begin
  if FileName <> '' then begin
    if ExtractFileExt(FileName) <> '.GUI' then
      FileName := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.GUI';

    if FileExists(FileName) then begin
      FileSetAttr(FileName, 0);
      FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone);
    end else begin
      FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone or fmCreate);
    end;
    DxControlCount := 0;
    FileStream.Position := 0;
    FillChar(FileHeader, SizeOf(TGuiFileHeader), #0);
    FileHeader.dCreateDate := Now;
    FileHeader.ClientVersion := g_ClientVersion;
    FileHeader.nGuiVersion := PROVERSION;
    FileStream.Write(FileHeader, SizeOf(TGuiFileHeader));
    g_SaveComponentList.Clear;
    StringList := TStringList.Create;
    for I := 0 to StructureDlg.TreeView.Items.Count - 1 do begin
      TreeNode := StructureDlg.TreeView.Items.Item[I];
      if TreeNode.Parent <> nil then Continue;
      Background := TDxControl(TreeNode.Data);

      ComponentList := TList.Create;
      for II := 0 to Background.ComponentCount - 1 do begin
        DxControl := Background.Control[II];
        if (DxControl is TDxPopupMenu) and
          (DxControl.PopupMenu <> nil) and
          (DxControl.PopupMenu is TDxComboBox) then Continue;
        ComponentList.Add(DxControl);
      end;

      for II := 0 to ComponentList.Count - 1 do begin
        DxControl := ComponentList[II];
        g_SaveComponentList.AddObject(DxControl.Name, DxControl);
        FillChar(GuiHeader, SizeOf(GuiHeader), 0);
        GuiHeader.Gui := TGuiType(DxControl.Tag);
        GuiHeader.Left := DxControl.Left;
        GuiHeader.Top := DxControl.Top;
        GuiHeader.Width := DxControl.Width;
        GuiHeader.Height := DxControl.Height;
        GuiHeader.Enabled := DxControl.Enabled;
        GuiHeader.Visible := DxControl.Visible;
        GuiHeader.Transparent := DxControl.Transparent;
        GuiHeader.EnableFocus := DxControl.EnableFocus;
        GuiHeader.Floating := DxControl.Floating;
        GuiHeader.OwnerMove := DxControl.OwnerMove;
        GuiHeader.MouseEvents := DxControl.MouseEvents;
        GuiHeader.NameLen := Length(DxControl.Name);
        GuiHeader.Background := Integer(Background);
      //GuiHeader.Image := DxControl.ImageType;
        GuiHeader.Count := DxControl.ComponentCount;
        FileStream.Write(GuiHeader, SizeOf(TGuiHeader));

        if GuiHeader.NameLen > 0 then begin
          sText := DxControl.Name;
          FileStream.Write(sText[1], GuiHeader.NameLen);
        end;

        SaveComponent(FileStream, DxControl);

        for III := 0 to DxControl.ControlCount - 1 do begin
          SaveSubComponent(FileStream, DxControl.Control[III]);
        end;

        FileStream.Size := FileStream.Position;
      end;
      ComponentList.Free;
    end;

    for I := 0 to g_SaveComponentList.Count - 1 do begin
      StringList.Add(GetSaveComponentString(TDxControl(g_SaveComponentList.Objects[I])));
    end;

    Clipboard.AsText := StringList.Text;
    try
      StringList.SaveToFile(ExtractFilePath(Paramstr(0)) + 'FState.txt');
    except

    end;

    FileStream.Position := 0;
    FileHeader.nCount := g_SaveComponentList.Count;
    FileStream.Write(FileHeader, SizeOf(TGuiFileHeader));
    FileStream.Free;
    StringList.Free
  end;
end;

function TFrmMain.GetSaveComponentString(DxControl: TDxControl): string;
var
  Gui: TGuiType;
begin
  Gui := TGuiType(DxControl.Tag);
  case Gui of
    t_Form: Result := '    ' + DxControl.Name + ': TDxImageForm;';
    t_FormShape: Result := '    ' + DxControl.Name + ': TDxImageFormShape;';
    t_Button: Result := '    ' + DxControl.Name + ': TDxImageButton;';
    t_Edit: Result := '    ' + DxControl.Name + ': TDxEdit;';
    t_Label: Result := '    ' + DxControl.Name + ': TDxLabel;';
    t_Grid: Result := '    ' + DxControl.Name + ': TDxImageGrid;';
    t_ScrollBox: Result := '    ' + DxControl.Name + ': TDxScrollBox;';
    t_ChatMemo: Result := '    ' + DxControl.Name + ': TDxChatMemo;';
    t_ListView: Result := '    ' + DxControl.Name + ': TDxListView;';
    t_TreeView: Result := '    ' + DxControl.Name + ': TDxTreeView;';
    t_PopupMenu: Result := '    ' + DxControl.Name + ': TDxPopupMenu;';
    t_PageControl: Result := '    ' + DxControl.Name + ': TDxPageControl;';
    t_TabSheet: Result := '    ' + DxControl.Name + ': TDxTabSheet;';
    t_ComboBox: Result := '    ' + DxControl.Name + ': TDxComboBox;';
    t_Line: Result := '    ' + DxControl.Name + ': TDxLine;';
  end;
end;

procedure TFrmMain.SaveSubComponentToPasFile(StringList: TStringList; DxControl: TDxControl);
var
  I: Integer;
  DxComboBox: TDxComboBox;
  DxPopupMenu: TDxPopupMenu;
begin
  StringList.AddObject(DxControl.Name, DxControl);
  for I := 0 to DxControl.ControlCount - 1 do begin
    SaveSubComponentToPasFile(StringList, DxControl.Control[I]);
  end;
end;
{-------------------------------------------------------------------------------}

procedure TFrmMain.Menu_CopyClick(Sender: TObject);
var
  MemoryStream: TMemoryStream;
  vRect: TRect;
begin
  if (g_SelectComponent <> nil) then begin
    MemoryStream := TMemoryStream.Create;
    vRect := TDxControl(g_SelectComponent).VirtualRect;
    g_nComponentX := vRect.Left;
    g_nComponentY := vRect.Top;
    FrmMain.SaveControl(MemoryStream, TDxControl(g_SelectComponent));
    MemoryStream.Position := 0;
    Clipboard.Clear;

    StreamSaveToClipboard(MemoryStream);
    MemoryStream.Free();
  end;
end;

procedure TFrmMain.Menu_PasteClick(Sender: TObject);
var
  MemoryStream: TMemoryStream;
begin
  if not ((TDxControl(g_SelectComponent) is TDxImageButton) or
    (TDxControl(g_SelectComponent) is TDxEdit) or
    (TDxControl(g_SelectComponent) is TDxLabel) or
    //(TDxControl(g_SelectComponent) is TDxImageGrid) or
    (TDxControl(g_SelectComponent) is TDxPopupMenu) or
    (TDxControl(g_SelectComponent) is TDxComboBox)) then begin
    MemoryStream := TMemoryStream.Create;
    StreamLoadFromClipboard(MemoryStream);
    MemoryStream.Position := 0;
    FrmMain.LoadControl(MemoryStream, TDxControl(g_SelectComponent), g_nMouseX - g_nComponentX, g_nMouseY - g_nComponentY);
    MemoryStream.Free;
  end;
end;

procedure TFrmMain.FormDblClick(Sender: TObject);
var
  pt: TPoint;
begin
  GetCursorPos(pt);
  pt := ScreenToClient(pt);
  if g_Background <> nil then
    g_Background.DblClick(pt.X, pt.Y);
end;

procedure TFrmMain.FormMouseDown(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
var
  pt: TPoint;
  DxControl: TDxControl;
  TreeNode: TTreeNode;
begin
  GetCursorPos(pt);

  //showmessage(IntToStr(pt.X)+' '+IntToStr(X)+' '+IntToStr(Left));

  pt := ScreenToClient(pt);

  g_nMouseX := X;
  g_nMouseY := Y;

  if g_Background <> nil then begin

    g_Background.MouseDown(Button, Shift, pt.X, pt.Y);
  end;
  if g_SelectComponent <> nil then begin
    DxControl := TDxControl(g_SelectComponent);
    TreeNode := TTreeNode(DxControl.Data);
    if (TreeNode <> nil) then begin
      //showmessage(TreeNode.Text);
      TreeNode.Expanded := True;
      //TreeNode.Expand(True);
      TreeNode.Selected := True;
      //if Assigned(DxControl.OnClick) then
       // DxControl.OnClick(DxControl, 0, 0);
    end;
  end;
end;

procedure TFrmMain.FormMouseMove(Sender: TObject; Shift: TShiftState;
  X, Y: Integer);
var
  Screenpt: TPoint;
  Originpt: TPoint;
  boSetPt: Boolean;
begin
  GetCursorPos(Screenpt);
  //pt := ScreenToClient(pt);  ClientToScreen(     Application.MainForm.   pt.X,pt.Y
  if g_Background <> nil then begin
    if (g_Background.MouseDownControl <> nil) then begin
      boSetPt := False;
      Originpt := ClientOrigin;
      if Screenpt.X < Originpt.X then begin
        Screenpt.X := Originpt.X;
        X := 0;
        boSetPt := True;
      end;
      if Screenpt.Y < Originpt.Y then begin
        Screenpt.Y := Originpt.Y;
        Y := 0;
        boSetPt := True;
      end;
      if Screenpt.X > Originpt.X + ClientWidth then begin
        Screenpt.X := Originpt.X + ClientWidth;
        X := g_Background.Width;
        boSetPt := True;
      end;
      if Screenpt.Y > Originpt.Y + ClientHeight then begin
        Screenpt.Y := Originpt.Y + ClientHeight;
        Y := g_Background.Height;
        boSetPt := True;
      end;
      if boSetPt then
        SetCursorPos(Screenpt.X, Screenpt.Y);
    end;
    g_Background.MouseMove(Shift, X, Y);
  end;
end;

procedure TFrmMain.FormMouseUp(Sender: TObject; Button: TMouseButton;
  Shift: TShiftState; X, Y: Integer);
var
  pt: TPoint;
begin
  GetCursorPos(pt);
  pt := ScreenToClient(pt);
  if g_Background <> nil then
    g_Background.MouseUp(Button, Shift, X, Y);
end;

procedure TFrmMain.FormCloseQuery(Sender: TObject;
  var CanClose: Boolean);
begin
  if (not g_boClearComponent) {or (StructureDlg.TreeView.Items.Count > 0) } then begin
    CanClose := False;
    g_boClose := True;
    TimerClose.Enabled := True;
  end;
end;

procedure TFrmMain.FormDestroy(Sender: TObject);
begin
  MagneticWnd.Free;
end;

procedure TFrmMain.TimerCloseTimer(Sender: TObject);
begin
  if not g_boClearComponent then begin
    g_boClearComponent := True;
    ClearComponent;
    Timer.Enabled := False;
    GameCanvas.Finalize;

    FreeAndNil(GameCanvas);

    g_WPrguseImages.Free;
    g_WPrguse2Images.Free;
    g_WPrguse3Images.Free;

    g_WisPrguseImages.Free;
    g_WisPrguse2Images.Free;
    g_WisPrguse3Images.Free;


    g_WPrguseImages_16.Free;
    g_WPrguse2Images_16.Free;
    g_WPrguse3Images_16.Free;


    g_WChrSelImages.Free;
    g_WChrSelImages_16.Free;

    g_WUIImages.Free;
    g_WUI1Images.Free;
    g_WUI2Images.Free;
    g_WUI3Images.Free;
    g_WNewopUIImages.Free;
    g_SaveComponentList.Free;

    g_MainBackground.Free;
  end;
  if g_boCanClose then Close;
end;

procedure TFrmMain.OnCreateDxControlClick(Sender: TObject);
var
  TreeNode, ChildTreeNode: TTreeNode;
  DxControl: TDxControl;
  AOwner: TDxControl;
  PageControl: TDxPageControl;
  TabSheet: TDxTabSheet;
  SelectGui: TGuiType;
  vtRect: TRect;
begin
  SelectGui := TGuiType(TMenuItem(Sender).Tag);
  if (SelectGui <> t_None) then begin
    AOwner := nil;
    if (g_SelectComponent <> nil) then begin
      AOwner := TDxControl(g_SelectComponent);
      if (AOwner is TDxImageButton) or
        (AOwner is TDxEdit) or
        (AOwner is TDxLabel) or
        //(AOwner is TDxImageGrid) or
      (AOwner is TDxPopupMenu) or
        (AOwner is TDxComboBox) or
        (AOwner is TDxLine)
        then begin
        Exit;
      end;
    end else
      if g_Background <> nil then begin
      AOwner := g_Background;
    end;
    if AOwner <> nil then begin
      DxControl := nil;
      case SelectGui of
        t_Form: DxControl := TDxImageForm.Create(AOwner);
        t_FormShape: DxControl := TDxImageFormShape.Create(AOwner);
        t_Button: DxControl := TDxImageButton.Create(AOwner);
        t_Edit: DxControl := TDxEdit.Create(AOwner);
        t_Label: DxControl := TDxLabel.Create(AOwner);
        t_Grid: DxControl := TDxImageGrid.Create(AOwner);
        t_ScrollBox: DxControl := TDxScrollBox.Create(AOwner);
        t_ChatMemo: DxControl := TDxChatMemo.Create(AOwner);
        t_ListView: DxControl := TDxListView.Create(AOwner);

        t_TreeView: DxControl := TDxTreeView.Create(AOwner);
        t_PopupMenu: DxControl := TDxPopupMenu.Create(AOwner);
        t_PageControl: DxControl := TDxPageControl.Create(AOwner);
        t_ComboBox: DxControl := TDxComboBox.Create(AOwner);
        t_Line: DxControl := TDxLine.Create(AOwner);

        t_TabSheet: begin
            if AOwner is TDxPageControl then
              DxControl := TDxTabSheet.Create(AOwner);
          end;
      end;

      if DxControl <> nil then begin
        g_SelectComponent := DxControl;
        DxControl.OnMouseDown := OnDxControlMouseDown;
        DxControl.OnClick := OnDxControlClick;
        DxControl.OnDblClick := OnDxControlDblClick;
        DxControl.OnGetImage := OnDxControlGetImage;
        DxControl.Tag := Integer(SelectGui);
        TreeNode := TTreeNode(AOwner.Data);
        ChildTreeNode := StructureDlg.TreeView.Items.AddChildObject(TreeNode, DxControl.Name, DxControl);
        DxControl.Data := ChildTreeNode;
        if SelectGui = t_PageControl then begin
          TDxPageControl(DxControl).OnTabSheetCreate := TabSheetCreate;
          TDxPageControl(DxControl).OnTabSheetDestroy := TabSheetDestroy;
        end else
          if SelectGui = t_TabSheet then begin
          TabSheet := TDxTabSheet(DxControl);
          TabSheet.Designing := False;
          PageControl := TDxPageControl(AOwner);
          if PageControl.PageCount > 0 then begin
            TabSheet.Assign(PageControl.Pages[PageControl.PageCount - 1]);
          end else begin
            TabSheet.Left := PageControl.ClientLeft;
            TabSheet.Top := PageControl.ClientTop;
            TabSheet.Width := PageControl.ClientWidth;
            TabSheet.Height := PageControl.ClientHeight;
          end;
          TabSheet.TabVisible := True;
          PageControl.ActivePage := TabSheet;
        end;
        vtRect := AOwner.VirtualRect;
        DxControl.Left := g_nMouseX - vtRect.Left;
        DxControl.Top := g_nMouseY - vtRect.Top;
{$IF  CLIENTEXE = 0}
        ObjectsDlg.AddComponent(DxControl);
{$IFEND}

      end else begin
        g_SelectComponent := nil;
      end;
    end;
  end;
end;

procedure TFrmMain.PopupMenuMainPopup(Sender: TObject);
var
  DxControl: TDxControl;
begin
  if g_SelectComponent <> nil then begin
    DxControl := TDxControl(g_SelectComponent);
    if ((DxControl is TDxImageButton) or
      (DxControl is TDxEdit) or
      (DxControl is TDxLabel) or
      //(DxControl is TDxImageGrid) or
      (DxControl is TDxPopupMenu) or
      (DxControl is TDxComboBox) or
      (DxControl is TDxLine)) then

      Menu_Create.Enabled := False
    else
      Menu_Create.Enabled := True;
  end else
    if g_Background <> nil then begin
    Menu_Create.Enabled := True;
  end else begin
    Menu_Create.Enabled := False;
  end;
end;

procedure TFrmMain.SubMenuClick(Sender: TObject);
var
  Item: TMenuItem;
  Background: TDxControlEngine;
  TreeNode: TTreeNode;
begin
  Item := TMenuItem(Sender);
  Background := TDxControlEngine(Item.Tag);
  TreeNode := TTreeNode(Background.Data);
  TreeNode.Selected := True;
  StructureDlg.TreeViewClick(Self);
end;

procedure TFrmMain.AddSubMenu(DxControl: TDxControl);
var
  Item: TMenuItem;
begin
  Item := TMenuItem.Create(Menu_Background);
  Item.Tag := Integer(DxControl);
  Item.OnClick := SubMenuClick;
  Item.Caption := DxControl.Name;
  Menu_Background.Add(Item);
  RefBackgroundList;
end;

procedure TFrmMain.DelSubMenu(DxControl: TDxControl);
var
  I: Integer;
  Item: TMenuItem;
begin
  for I := 0 to Menu_Background.Count - 1 do begin
    Item := Menu_Background.Items[I];
    if TDxControl(Item.Tag) = DxControl then begin
      Menu_Background.Delete(I);
      Item.Free;
      RefBackgroundList;
      break;
    end;
  end;
end;

procedure TFrmMain.Menu_LoadFromFileClick(Sender: TObject);
var
  I: Integer;
  DxControl: TDxControl;
  Background: TDxControlEngine;
  GuiHeader: TGuiHeader;
  FileStream: TFileStream;
  TreeNode: TTreeNode;
  ChildTreeNode: TTreeNode;
  FileHeader: TGuiFileHeader;
  Len: Integer;
  sText: string;
  IniFile: TIniFile;
begin
  OpenDialog.Filter := 'GUI文件|*.GUI';
  if OpenDialog.Execute then begin
    g_sGuiFileName := OpenDialog.FileName;

    ClearComponent;
    g_nBackground := 0;
    g_Background := g_MainBackground;
    FileStream := TFileStream.Create(OpenDialog.FileName, fmOpenRead or fmShareDenyNone);
    FileStream.Read(FileHeader, SizeOf(TGuiFileHeader));
    Background := nil;
    g_SelectComponent := nil;
    while True do begin
      if FileStream.Position >= FileStream.Size then break;
      if FileStream.Read(GuiHeader, SizeOf(TGuiHeader)) = SizeOf(TGuiHeader) then begin
        if (Background = nil) or (Background.ReserveIndex <> GuiHeader.Background) then begin
          Inc(g_nBackground);
          Background := TDxControlEngine.Create();
          Background.Name := Background.Name; //+ IntToStr(g_nBackground);
          Background.Width := ClientWidth;
          Background.Height := ClientHeight;
          Background.ReserveIndex := GuiHeader.Background;
          Background.OnMouseDown := OnDxControlMouseDown;
          Background.OnClick := OnDxControlClick;
          Background.OnDblClick := OnDxControlDblClick;
          AddSubMenu(Background);
          TreeNode := StructureDlg.TreeView.Items.AddObject(nil, Background.Name, Background);
          Background.Data := TreeNode;
        end;

        DxControl := NewDxControl(GuiHeader, Background);

        if DxControl <> nil then begin
          if GuiHeader.NameLen > 0 then begin
            SetLength(sText, GuiHeader.NameLen);
            FileStream.Read(sText[1], GuiHeader.NameLen);
            if not FindDxComponent(Background, sText) then
              DxControl.Name := sText;
          end;
          TreeNode := TTreeNode(Background.Data);
          ChildTreeNode := StructureDlg.TreeView.Items.AddChildObject(TreeNode, DxControl.Name, DxControl);
          DxControl.Data := ChildTreeNode;

          LoadComponent(FileStream, DxControl);

          for I := 0 to GuiHeader.Count - 1 do begin
            LoadSubComponent(FileStream, DxControl);
          end;
        end;
      end else break;
    end; //while True do begin
    FileStream.Free;
  end;

  if StructureDlg.TreeView.Items.Count <= 0 then begin
    Inc(g_nBackground);
    Background := TDxControlEngine.Create();
    Background.Name := Background.Name; //+ IntToStr(g_nBackground);
    Background.Width := ClientWidth;
    Background.Height := ClientHeight;
    Background.ReserveIndex := GuiHeader.Background;
    Background.OnMouseDown := OnDxControlMouseDown;
    Background.OnClick := OnDxControlClick;
    Background.OnDblClick := OnDxControlDblClick;
    AddSubMenu(Background);
    TreeNode := StructureDlg.TreeView.Items.AddObject(nil, Background.Name, Background);
    Background.Data := TreeNode;
  end;

  Menu_Save.Enabled := StructureDlg.TreeView.Items.Count > 0;
 { if StructureDlg.TreeView.Items.Count > 0 then begin
    StructureDlg.TreeView.Items[0].Selected := True;
    StructureDlg.TreeViewClick(Sender);
  end;   }
  if g_FileNameList.IndexOf(g_sGuiFileName) < 0 then g_FileNameList.Add(g_sGuiFileName);
  IniFile := TIniFile.Create(ExtractFilePath(Application.ExeName) + 'Config.ini');
  for I := 0 to g_FileNameList.Count - 1 do begin
    IniFile.WriteString('FileNames', IntToStr(I), g_FileNameList.Strings[I]);
  end;
  IniFile.Free;
  RefFileNameList;
  //RefBackgroundList;
end;

procedure TFrmMain.Menu_SaveClick(Sender: TObject);
begin
  //Menu_SaveFile.Enabled := False;
  SaveToFile(g_sGuiFileName);
  //Menu_SaveFile.Enabled := True;
end;

procedure TFrmMain.Menu_SaveToFileClick(Sender: TObject);
begin
  //Menu_SaveToFile.Enabled := False;
  SaveDialog.Filter := 'GUI文件|*.GUI';
  if SaveDialog.Execute then begin
    SaveToFile(SaveDialog.FileName);
  end;
  //Menu_SaveToFile.Enabled := True;
end;

procedure TFrmMain.LoadFromFileClick(Sender: TObject; X, Y: Integer);
  function ExtractFileNameOnly(const fname: string): string;
  var
    extpos: Integer;
    ext, fn: string;
  begin
    ext := ExtractFileExt(fname);
    fn := ExtractFileName(fname);
    if ext <> '' then begin
      extpos := Pos(ext, fn);
      Result := Copy(fn, 1, extpos - 1);
    end else
      Result := fn;
  end;
var
  I: Integer;
  DxControl: TDxControl;
  Background: TDxControlEngine;
  GuiHeader: TGuiHeader;
  FileStream: TFileStream;
  TreeNode: TTreeNode;
  ChildTreeNode: TTreeNode;
  FileHeader: TGuiFileHeader;
  Len: Integer;
  sText: string;
  sFileName: string;
  DxLabel: TDxLabel;
begin
  DxLabel := TDxLabel(Sender);
  g_sGuiFileName := DxLabel.Caption;
  if g_sGuiFileName <> '' then begin
    if ExtractFileExt(g_sGuiFileName) <> '.GUI' then
      g_sGuiFileName := ExtractFilePath(g_sGuiFileName) + ExtractFileNameOnly(g_sGuiFileName) + '.GUI';

    if FileExists(g_sGuiFileName) then begin
      FileSetAttr(g_sGuiFileName, 0);
      FileStream := TFileStream.Create(g_sGuiFileName, fmOpenReadWrite or fmShareDenyNone);

      ClearComponent;
      g_nBackground := 0;
      g_Background := g_MainBackground;
  //FileStream := TFileStream.Create(g_sGuiFileName, fmOpenRead or fmShareDenyNone);
      FileStream.Read(FileHeader, SizeOf(TGuiFileHeader));
      Background := nil;
      g_SelectComponent := nil;
      while True do begin
        if FileStream.Position >= FileStream.Size then break;
        if FileStream.Read(GuiHeader, SizeOf(TGuiHeader)) = SizeOf(TGuiHeader) then begin
          if (Background = nil) or (Background.ReserveIndex <> GuiHeader.Background) then begin
            Inc(g_nBackground);
            Background := TDxControlEngine.Create();
            Background.Name := Background.Name; // + IntToStr(g_nBackground);
            Background.Width := ClientWidth;
            Background.Height := ClientHeight;
            Background.ReserveIndex := GuiHeader.Background;
            Background.OnMouseDown := OnDxControlMouseDown;
            Background.OnClick := OnDxControlClick;
            Background.OnDblClick := OnDxControlDblClick;
            AddSubMenu(Background);
            TreeNode := StructureDlg.TreeView.Items.AddObject(nil, Background.Name, Background);
            Background.Data := TreeNode;
          end;

          DxControl := NewDxControl(GuiHeader, Background);

          if DxControl <> nil then begin
            if GuiHeader.NameLen > 0 then begin
              SetLength(sText, GuiHeader.NameLen);
              FileStream.Read(sText[1], GuiHeader.NameLen);
              if not FindDxComponent(Background, sText) then
                DxControl.Name := sText;
            end;
            TreeNode := TTreeNode(Background.Data);
            ChildTreeNode := StructureDlg.TreeView.Items.AddChildObject(TreeNode, DxControl.Name, DxControl);
            DxControl.Data := ChildTreeNode;

            LoadComponent(FileStream, DxControl);

            for I := 0 to GuiHeader.Count - 1 do begin
              LoadSubComponent(FileStream, DxControl);
            end;
          end;
        end else break;
      end; //while True do begin

      if StructureDlg.TreeView.Items.Count <= 0 then begin
        Inc(g_nBackground);
        Background := TDxControlEngine.Create();
        Background.Name := Background.Name; // + IntToStr(g_nBackground);
        Background.Width := ClientWidth;
        Background.Height := ClientHeight;
        Background.ReserveIndex := GuiHeader.Background;
        Background.OnMouseDown := OnDxControlMouseDown;
        Background.OnClick := OnDxControlClick;
        Background.OnDblClick := OnDxControlDblClick;
        AddSubMenu(Background);
        TreeNode := StructureDlg.TreeView.Items.AddObject(nil, Background.Name, Background);
        Background.Data := TreeNode;
      end;

      FileStream.Free;
      RefBackgroundList;
      Menu_Save.Enabled := StructureDlg.TreeView.Items.Count > 0;
      if StructureDlg.TreeView.Items.Count > 0 then begin
        StructureDlg.TreeView.Items[0].Selected := True;
        StructureDlg.TreeViewClick(Sender);
      end;
    end;
  end;
end;

procedure TFrmMain.RefFileNameList;
var
  I: Integer;
  DxLabel: TDxLabel;
begin
  while g_FileNameMemo.ControlCount > 0 do
    g_FileNameMemo.Control[0].Free;

  g_FileNameMemo.Position := 0;
  for I := 0 to g_FileNameList.Count - 1 do begin
    DxLabel := TDxLabel.Create(g_FileNameMemo);
    DxLabel.Designing := False;
    DxLabel.Left := 1;
    DxLabel.Top := I * g_FileNameMemo.ItemHeight;
    DxLabel.Caption := g_FileNameList.Strings[I];
    DxLabel.CaptionColor.Up.Style := [fsBold, fsUnderline];
    DxLabel.CaptionColor.Up.Color := clBlue;
    DxLabel.CaptionColor.Up.Bold := False;
    DxLabel.CaptionColor.Up.Size := 12;

    DxLabel.CaptionColor.Down.Style := [fsBold, fsUnderline];
    DxLabel.CaptionColor.Down.Color := clRed;
    DxLabel.CaptionColor.Down.Bold := False;
    DxLabel.CaptionColor.Down.Size := 12;

    DxLabel.CaptionColor.Hot.Style := [fsBold, fsUnderline];
    DxLabel.CaptionColor.Hot.Color := clYellow;
    DxLabel.CaptionColor.Hot.Bold := False;
    DxLabel.CaptionColor.Hot.Size := 12;

    DxLabel.Alignment := taLeftJustify;
    DxLabel.AutoSize := False;
    DxLabel.Width := g_FileNameMemo.Width - 30;
    DxLabel.Height := g_FileNameMemo.ItemHeight;
    DxLabel.OnClick := LoadFromFileClick;
  end;

  g_FileNameMemo.First;
end;

procedure TFrmMain.ChangeBackgroundClick(Sender: TObject; X, Y: Integer);
var
  DxLabel: TDxLabel;
begin
  g_SelectComponent := nil;
  DxLabel := TDxLabel(Sender);
  g_Background := TDxControlEngine(DxLabel.Data);
end;

procedure TFrmMain.RefBackgroundList;
var
  I, nC: Integer;
  Item: TMenuItem;
  TreeNode: TTreeNode;
  DxLabel: TDxLabel;
begin
  while g_BackgroundMemo.ControlCount > 0 do
    g_BackgroundMemo.Control[0].Free;
  g_BackgroundMemo.Position := 0;
  nC := 0;
  for I := 0 to StructureDlg.TreeView.Items.Count - 1 do begin
    TreeNode := StructureDlg.TreeView.Items[I];
    if TreeNode.Parent <> nil then Continue;
    DxLabel := TDxLabel.Create(g_BackgroundMemo);
    DxLabel.Designing := False;
    DxLabel.Left := 1;
    DxLabel.Top := nC * g_BackgroundMemo.ItemHeight;
    DxLabel.Caption := TreeNode.Text;
    DxLabel.CaptionColor.Up.Style := [fsBold, fsUnderline];
    DxLabel.CaptionColor.Up.Color := clBlue;
    DxLabel.CaptionColor.Up.Bold := False;
    DxLabel.CaptionColor.Up.Size := 12;

    DxLabel.CaptionColor.Down.Style := [fsBold, fsUnderline];
    DxLabel.CaptionColor.Down.Color := clRed;
    DxLabel.CaptionColor.Down.Bold := False;
    DxLabel.CaptionColor.Down.Size := 12;

    DxLabel.CaptionColor.Hot.Style := [fsBold, fsUnderline];
    DxLabel.CaptionColor.Hot.Color := clYellow;
    DxLabel.CaptionColor.Hot.Bold := False;
    DxLabel.CaptionColor.Hot.Size := 12;

    DxLabel.Alignment := taLeftJustify;
    DxLabel.AutoSize := False;
    DxLabel.Width := g_BackgroundMemo.Width - 30;
    DxLabel.Height := g_BackgroundMemo.ItemHeight;
    DxLabel.Data := Pointer(TreeNode.Data);
    DxLabel.OnClick := ChangeBackgroundClick;
    Inc(nC);
  end;
  g_BackgroundMemo.First;
end;

procedure TFrmMain.TimerStartTimer(Sender: TObject);
begin
  TimerStart.Enabled := False;

  RefFileNameList;
  g_Background := g_MainBackground;

  StructureDlg.Top := Top;
  StructureDlg.Left := Left + Width;
  StructureDlg.Height := Height;

  ObjectsDlg.Top := Top;
  ObjectsDlg.Left := Left - StructureDlg.Width;
  ObjectsDlg.Height := Height;


  StructureDlg.Show;
  ObjectsDlg.Show;
end;

procedure TFrmMain.Menu_MainPageClick(Sender: TObject);
begin
  g_SelectComponent := nil;
  g_Background := g_MainBackground;
end;

procedure TFrmMain.MenuItem1_MainPageClick(Sender: TObject);
var
  pt: TPoint;
begin
  GetCursorPos(pt);
  PopupMenuMain.Popup(pt.X, pt.Y);
end;

end.

