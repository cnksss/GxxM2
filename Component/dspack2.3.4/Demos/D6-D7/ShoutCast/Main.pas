unit Main;

interface

uses
  Windows, Messages, SysUtils, Graphics, Controls, Forms, Dialogs, DirectShow9,
  ActiveX, StdCtrls, DSUtil, ComCtrls, ShellAPI, Definitions, Buttons, Classes,
  ExtCtrls;

type
  TForm1 = class(TForm, IAsyncExCallBack)
    TmrNillAll: TTimer;
    Label1: TLabel;
    ComboBox1: TComboBox;
    Button6: TButton;
    GroupBox5: TGroupBox;
    Label7: TLabel;
    Label2: TLabel;
    Label8: TLabel;
    Label3: TLabel;
    TrackBar1: TTrackBar;
    TrackBar2: TTrackBar;
    GroupBox4: TGroupBox;
    RadioButton3: TRadioButton;
    RadioButton4: TRadioButton;
    GroupBox2: TGroupBox;
    Label20: TLabel;
    Label21: TLabel;
    Label22: TLabel;
    Label23: TLabel;
    Label24: TLabel;
    Label25: TLabel;
    Label26: TLabel;
    Label27: TLabel;
    GroupBox3: TGroupBox;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    GroupBox9: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormCreate(Sender: TObject);
    procedure TrackBar1Change(Sender: TObject);
    procedure Button6Click(Sender: TObject);
    procedure TrackBar2Change(Sender: TObject);
    procedure TmrNillAllTimer(Sender: TObject);
    procedure Label19Click(Sender: TObject);
    procedure Label26Click(Sender: TObject);
  private
    FCurrentUrlLocation: string;
    FFontColorGroupbox4: TColor;
    // destroy all used Com Objects
    procedure NillAll;
    // open URL
    procedure OpenURL;
    { All callback strings need to be copied before setting them to a Label
       or any other external Object                                         }
    function AsyncExFilterState(Buffering: LongBool; PreBuffering: LongBool;
      Connecting: LongBool; Playing: LongBool;
      BufferState: integer): HRESULT; stdcall;
    function AsyncExICYNotice(IcyItemName: PChar;
      ICYItem: PChar): HRESULT; stdcall;
    function AsyncExMetaData(Title: PChar; URL: PChar): HRESULT; stdcall;
    function AsyncExSockError(ErrString: PChar): HRESULT; stdcall;
  end;

const
  ConnectCaption = 'connect';
  DisConnectCaption = 'disconnect';

var
  Form1: TForm1;
  GraphBuilder: IGraphBuilder = nil;
  MediaControl: IMediaControl = nil;
  AsyncEx: IBaseFilter = nil;
  FileSource: IFilesourcefilter = nil;
  AsyncExControl: IAsyncExControl = nil;
  Mpeg1Splitter: IBaseFilter = nil;
  Pin: IPin = nil;

implementation

{$R *.dfm}

procedure TForm1.NillAll;
begin
  // MediaControl.Stop is required before destroying filters and interfaces
  if Assigned(MediaControl) then
    MediaControl.Stop;
  if Assigned(AsyncExControl) then
    AsyncExControl.FreeCallback;
  Application.HandleMessage;
  if Assigned(Pin) then
    Pin := nil;
  if Assigned(FileSource) then
    FileSource := nil;
  if Assigned(MediaControl) then
    MediaControl := nil;
  if Assigned(GraphBuilder) then
    GraphBuilder := nil;
  if Assigned(AsyncEx) then
    AsyncEx := nil;
  if Assigned(AsyncExControl) then
    AsyncExControl := nil;
  button6.Caption := ConnectCaption;
  GroupBox4.Font.Color := FFontColorGroupbox4;
  GroupBox4.Enabled := true;
  RadioButton3.Enabled := true;
  RadioButton4.Enabled := true;
end;

procedure TForm1.OpenURL();
begin
  NillAll;
  button6.Caption := DisConnectCaption;
  GroupBox4.Font.Color := clDkGray;
  GroupBox4.Enabled := false;
  RadioButton3.Enabled := false;
  RadioButton4.Enabled := false;
  CheckDSError(CoCreateInstance(TGUID(CLSID_FilterGraph), nil, CLSCTX_INPROC,
    TGUID(IID_IGraphBuilder), GraphBuilder));
  CheckDSError(GraphBuilder.QueryInterface(IID_IMediaControl, MediaControl));
  if failed(CoCreateInstance(CLSID_AsyncEx, nil, CLSCTX_INPROC,
    IID_IBaseFilter, AsyncEx)) then
  begin
    showmessage('you need a registered AsyncEx filter to run this' +
      ' example, location: DSPACK\Demos\D6-D7\Filters\AsyncEx');
    exit;
  end;
  CoCreateInstance(CLSID_Mpeg1Split, nil, CLSCTX_INPROC,
    IID_IBaseFilter, Mpeg1Splitter);
  CheckDSError(GraphBuilder.AddFilter(Mpeg1Splitter, 'MPEG1 Splitter'));
  CheckDSError(AsyncEx.QueryInterface(IID_IAsyncExControl, AsyncExControl));
  if assigned(AsyncExControl) then
    if failed(AsyncExControl.SetCallBack(self)) then
      exit;
  if assigned(AsyncExControl) then
  begin
    if RadioButton3.Checked then
      if failed(AsyncExControl.SetConnectToURL(
        PChar(ComboBox1.Text), TrackBar1.Position * 1000, true)) then
        exit;
    if RadioButton4.Checked then
      if failed(AsyncExControl.SetConnectToURL(
        PChar(ComboBox1.Text), TrackBar1.Position * 1000, false)) then
        exit;
  end;
  if assigned(AsyncExControl) then
    if failed(AsyncExControl.SetBuffersize(
      TrackBar2.Position * 1000)) then
      exit;
  if assigned(AsyncEx) then
    if failed(AsyncEx.FindPin(pinID, Pin)) then
      exit;
  if assigned(GraphBuilder) then
    if failed(GraphBuilder.AddFilter(AsyncEx,
      StringToOleStr('DSPlayer AsyncSource'))) then
      exit;
  if assigned(GraphBuilder) then
    if failed(GraphBuilder.Render(pin)) then
      exit;
  if assigned(MediaControl) then
    if failed(MediaControl.Run) then
      exit;
end;

procedure TForm1.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  NillAll;
  CoUninitialize;
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
  GroupBox9.DoubleBuffered := true;
  FCurrentUrlLocation := ComboBox1.Text;
  FFontColorGroupbox4 := GroupBox4.Font.Color;
  CoInitialize(nil);
  Label18.Hint := Label18.Caption;
  Label18.ShowHint := true;
  Label19.Hint := Label19.Caption;
  Label19.ShowHint := true;
  Label19.Font.Color := clBlue;
  Label19.Font.Style := [fsUnderline];
  Label24.Hint := Label24.Caption;
  Label24.ShowHint := true;
  Label25.Hint := Label25.Caption;
  Label25.ShowHint := true;
  Label26.Hint := Label26.Caption;
  Label26.ShowHint := true;
  Label26.Font.Color := clBlue;
  Label26.Font.Style := [fsUnderline];
  Label8.Caption := inttostr(TrackBar1.Position) + ' kb';
  Label3.Caption := inttostr(TrackBar2.Position) + ' kb';
end;

procedure TForm1.TrackBar1Change(Sender: TObject);
begin
  Label8.Caption := inttostr(TrackBar1.Position) + ' kb';
end;

procedure TForm1.Button6Click(Sender: TObject);
begin
  if Button6.Caption = ConnectCaption then
  begin
    if (FCurrentUrlLocation <> ComboBox1.Text) then
      FCurrentUrlLocation := ComboBox1.Text;
    OpenURL
  end
  else
    NillAll;
end;

procedure TForm1.TrackBar2Change(Sender: TObject);
begin
  Label3.Caption := inttostr(TrackBar2.Position) + ' kb';
  if assigned(AsyncExControl) then
    AsyncExControl.SetBuffersize(TrackBar2.Position * 1000);
end;

procedure TForm1.TmrNillAllTimer(Sender: TObject);
begin
  NillAll;
  TmrNillAll.Enabled := false;
end;

procedure TForm1.Label19Click(Sender: TObject);
begin
  if (Label19.Caption <> 'N/A') then
    ShellExecute(0, 'open', PChar(Label19.Hint), nil, nil, SW_SHOWNORMAL);
end;

procedure TForm1.Label26Click(Sender: TObject);
begin
  if Label26.Caption <> 'N/A' then
    ShellExecute(0, 'open', PChar(Label26.Hint), nil, nil, SW_SHOWNORMAL);
end;

// DSPlayer AsyncSource CallBack

function TForm1.AsyncExFilterState(Buffering: LongBool; PreBuffering: LongBool;
  Connecting: LongBool; Playing: LongBool;
  BufferState: integer): HRESULT; stdcall;
begin
  if PreBuffering then
    Label6.Caption := '( ' + inttostr(BufferState) + '% ) prebuffering....';
  if Buffering then
    Label6.Caption := '( ' + inttostr(BufferState) + '% ) buffering....';
  if Connecting then
    Label6.Caption := 'connecting....';
  if Playing then
    Label6.Caption := 'playing....';
  if not Buffering and not PreBuffering and not Connecting and not Playing then
  begin
    Label6.Caption := 'N/A';
    Label18.Caption := 'N/A';
    Label19.Caption := 'N/A';
  end;
  Result := S_OK;
end;

function TForm1.AsyncExICYNotice(IcyItemName: PChar;
  ICYItem: PChar): HRESULT; stdcall;
const // ICY Item Names
  ICYMetaInt = 'icy-metaint:';
  ICYName = 'icy-name:';
  ICYGenre = 'icy-genre:';
  ICYURL = 'icy-url:';
  ICYBitrate = 'icy-br:';
  ICYError = 'icy-error:';
begin
  if IcyItemName = ICYError then
  begin
    showmessage(copy(ICYItem, 1, length(ICYItem)));
    TmrNillAll.Enabled := true;
  end;
  if IcyItemName = ICYName then
  begin
    if length(ICYItem) > 39 then
      Label24.Caption := copy(ICYItem, 1, 39) + '...'
    else
      Label24.Caption := copy(ICYItem, 1, length(ICYItem));
    Label24.Hint := copy(ICYItem, 1, length(ICYItem));
  end;
  if (IcyItemName = ICYGenre) then
  begin
    if (length(ICYItem) > 39) then
      Label25.Caption := copy(ICYItem, 1, 39) + '...'
    else
      Label25.Caption := copy(ICYItem, 1, length(ICYItem));
    Label25.Hint := copy(ICYItem, 1, length(ICYItem));
  end;
  if (IcyItemName = ICYURL) then
  begin
    if (length(ICYItem) > 30) then
      Label26.Caption := copy(ICYItem, 1, 30) + '...'
    else
      Label26.Caption := copy(ICYItem, 1, length(ICYItem));
    Label26.Hint := copy(ICYItem, 1, length(ICYItem));
  end;
  if (IcyItemName = ICYBitrate) then
    Label27.Caption := copy(ICYItem, 1, length(ICYItem));
  Result := S_OK;
end;

function TForm1.AsyncExMetaData(Title: PChar; URL: PChar): HRESULT; stdcall;
begin
  if (length(Title) > 50) then
    Label18.Caption := copy(Title, 1, 45) + '...'
  else
    Label18.Caption := copy(Title, 1, length(Title));
  Label18.Hint := copy(Title, 1, length(Title));
  if (length(URL) > 50) then
    Label19.Caption := copy(URL, 1, 45) + '...'
  else
    Label19.Caption := copy(URL, 1, length(URL));
  Label19.Hint := copy(URL, 1, length(URL));
  Result := S_OK;
end;

function TForm1.AsyncExSockError(ErrString: PChar): HRESULT; stdcall;
begin
  showmessage('can not connect to URL'#13#10#13#10 +
    'Reason:'#13#10 + copy(ErrString, 1, length(ErrString)));
  TmrNillAll.Enabled := true;
  Result := S_OK;
end;

end.

