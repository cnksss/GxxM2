unit IntroScn;

interface

uses
  Windows,
  Messages,
  SysUtils,
  Classes,
  Graphics,
  StdCtrls,
  Controls,
  Forms,
  Dialogs,
  ExtCtrls,
  HGE,
  FState,
  Grobal2,
  SDK,
  ClFunc,
  SoundUtil,
  HUtil32,
  DxCanvas,
  HGECanvas;

type
  TLoginState = (lsLogin, lsNewid, lsNewidRetry, lsChgpw, lsCloseAll, lsRealName, lsBindPhone);

  TSceneType = (stNone, stWelcome, stLogin, stSelectCountry, stSelectChr, stNewChr, stLoading, stLoginNotice, stPlayGame); // stIntro,

  PSelChar = ^TSelChar;

  TSelChar = record
    Valid:Boolean; // 是否是有效人物
    UserChr:TUserCharacterInfo;
    Selected:Boolean; // 是否选择
    FreezeState:Boolean; // 是否是禁用状态
    Unfreezing:Boolean; // 是否正在启用人物
    Freezing:Boolean; // 是否正在禁用人物
    AniIndex:Integer; // 踌绰(绢绰) 局聪皋捞记
    DarkLevel:Integer;
    EffIndex:Integer; // 瓤苞 局聪皋捞记
    StartTime:longword;
    moretime:longword;
    startefftime:longword;
  end;

  TScene = class
  private
  public
    BackgroundColor:TColor;
    scenetype:TSceneType;
    constructor Create(ascenetype:TSceneType);
    procedure Initialize; dynamic;
    procedure Finalize; dynamic;
    procedure OpenScene; dynamic;
    procedure CloseScene; dynamic;
    procedure OpeningScene; dynamic;
    procedure KeyPress(var Key:Char); dynamic;
    procedure KeyDown(var Key:Word; Shift:TShiftState); dynamic;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); dynamic;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); dynamic;
    procedure RenderScene(Sender:TObject); dynamic;
    // procedure PlayScene(MSurface: TDirectDrawSurface); dynamic;//原场景渲染

    // property BackgroundColor: TColor read FBackgroundColor write FBackgroundColor;
  end;

  TWelcomeScene = class(TScene)
  private
    FInitialized:Boolean;
    FTexture:TTexture;
    FOpenScene:Boolean;
    FAddValue:Integer;
    FAlpha:Integer;
    FAlphaTick:LongWord;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Initialize; override;
    procedure Finalize; override;
    procedure OpenScene; override;
    procedure CloseScene; override;
    procedure RenderScene(Sender:TObject); override;
  end;

  TLoginScene = class(TScene)
  private
    m_nCurFrame:Integer;
    m_nMaxFrame:Integer;
    m_dwStartTime:longword;
    m_boNowOpening:Boolean;
    m_boOpenFirst:Boolean;
    m_NewIdRetryUE:TUserEntry;
    m_NewIdRetryAdd:TUserEntryAdd;
  public
    m_sLoginId:string;
    m_sLoginPasswd:string;
    m_boUpdateAccountMode:Boolean;
    constructor Create;
    destructor Destroy; override;
    procedure OpenScene; override;
    procedure CloseScene; override;
    procedure RenderScene(Sender:TObject); override;
    procedure ChangeLoginState(State:TLoginState);
    procedure NewClick;
    procedure NewIdRetry(boupdate:Boolean);
    procedure UpdateAccountInfos(ue:TUserEntry; ua:TUserEntryAdd);
    procedure RealName;
    procedure BindPhone;
    procedure ChgPwClick;
    procedure NewAccountOk;
    procedure NewAccountClose;
    procedure ChgpwOk;
    procedure ChgpwCancel;
    procedure HideLoginBox;
    procedure OpenLoginDoor;
    procedure PassWdFail(FailCode:Integer);
  end;

  TSelectChrScene = class(TScene)
    m_nCurFrame:Integer;
    m_nMaxFrame:Integer;
    m_dwStartTime:longword;
    m_nChrPage:Integer;
    m_nHighIndex:Integer;
  private
    //SoundTimer: TTimer;
    CreateChrMode:Boolean;
    m_nSelected:Integer;
    procedure MakeNewChar(Index:Integer);
  protected
    procedure SoundOnTimer(Sender:TObject);
  public
    NewIndex:Integer;
    ChrArr:array[0..19] of TSelChar;
    constructor Create;
    destructor Destroy; override;
    procedure OpenScene; override;
    procedure CloseScene; override;
    procedure SelChrSelect1Click;
    procedure SelChrSelect2Click;
    procedure SelChrSelect3Click;
    procedure SelChrStartClick;
    procedure SelChrNewChrClick;
    procedure SelChrEraseChrClick;
    procedure SelChrCreditsClick;
    procedure SelChrExitClick;
    procedure SelChrNewClose;
    procedure SelChrNewJob(job:Integer);
    procedure SelChrNewSex(sex:Integer);
    procedure SelChrNewOk;
    procedure SelChrUp;
    procedure SelChrDown;
    procedure SelSetText;
    procedure ClearChrs;
    procedure AddChr(uname:string; job, hair, Level, sex:Integer);
    procedure SelectChr(Index:Integer);
    procedure RenderScene(Sender:TObject); override;
  end;

  TLoginNotice = class(TScene)
  private
  public
    constructor Create;
    destructor Destroy; override;
  end;

implementation

uses
  ClMain,
  MShare,
  DIB,
  DxComponents,
  GameImages,
  MirNewUI205Dlg;

constructor TScene.Create(ascenetype:TSceneType);
begin
  scenetype := ascenetype;
  BackgroundColor := clBlack;
end;

procedure TScene.Initialize;
begin
end;

procedure TScene.Finalize;
begin

end;

procedure TScene.OpenScene;
begin
end;

procedure TScene.CloseScene;
begin
end;

procedure TScene.OpeningScene;
begin
end;

procedure TScene.KeyPress(var Key:Char);
begin
end;

procedure TScene.KeyDown(var Key:Word; Shift:TShiftState);
begin

end;

procedure TScene.MouseMove(Shift:TShiftState; X, Y:Integer);
begin

end;

procedure TScene.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin

end;

procedure TScene.RenderScene(Sender:TObject);
begin

end;

{--------------------- Welcome ----------------------}

constructor TWelcomeScene.Create;
begin
  inherited Create(stWelcome);
  FInitialized := False;
  FTexture := nil;
  FOpenScene := True;
  BackgroundColor := clWhite;
  FAlpha := 0;
  FAlphaTick := MyGetTickCount;
end;

destructor TWelcomeScene.Destroy;
begin
  if FTexture <> nil then
    FreeAndNil(FTexture);
  inherited;
end;

procedure TWelcomeScene.Initialize;
var
  FileData:Pointer;
  FileSize:Integer;
begin
  if not FInitialized then begin
    FInitialized := True;

    if (g_BackImage <> nil) and (g_BackImage.Width * g_BackImage.Height > 4) then begin
      g_BackImage.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
      g_BackImage.BitCount := 32;
      BackgroundColor := g_BackImage.Pixels[0, 0]; // 第一个像素为背景色

      if (g_BackImage.Width >= 400) and (g_BackImage.Height >= 400) then begin
        FileData32(g_BackImage, FileData, FileSize);
        if FileData <> nil then begin
          FTexture := NewTexture(FileData, FileSize, g_BackImage.Width, g_BackImage.Height, $FF000000, True);
          FreeMem(FileData);
        end;
      end
      else begin
        FTexture := NewTexture(g_BackImage);
      end;
    end;
  end;
end;

procedure TWelcomeScene.Finalize;
begin
  FInitialized := False;
  if FTexture <> nil then
    FreeAndNil(FTexture);
end;

procedure TWelcomeScene.OpenScene;
begin
  FAlpha := 0;
  FAlphaTick := MyGetTickCount;
  FAddValue := 0;
  FOpenScene := True;
  // DebugOutStr('TWelcomeScene.OpenScene');
end;

procedure TWelcomeScene.CloseScene;
begin
  FOpenScene := False;
  BackgroundColor := clBlack;
  if FTexture <> nil then
    FreeAndNil(FTexture);
  // DebugOutStr('TWelcomeScene.CloseScene');
end;

procedure TWelcomeScene.RenderScene(Sender:TObject);
begin
  if FOpenScene and (FTexture <> nil) and (FTexture.Width * FTexture.Height > 4) then begin // 画背景图片
    if (FAlpha < 255) {and (MyGetTickCount - FAlphaTick > 50)} then begin
      FAlphaTick := MyGetTickCount;
      if FAlpha mod 10 = 0 then
        Inc(FAddValue);

      Inc(FAlpha, FAddValue);
    end;
    if FAlpha >= 255 then
      GameCanvas.Draw((SCREENWIDTH - FTexture.Width) div 2, (SCREENHEIGHT - FTexture.Height) div 2, FTexture)
    else
      GameCanvas.DrawAlpha((SCREENWIDTH - FTexture.Width) div 2, (SCREENHEIGHT - FTexture.Height) div 2, FTexture, FAlpha);
  end
  else
    BackgroundColor := clBlack;
end;

{--------------------- Login ----------------------}

constructor TLoginScene.Create;
begin
  inherited Create(stLogin);

end;

destructor TLoginScene.Destroy;
begin
  inherited Destroy;
end;

procedure TLoginScene.OpenScene;
begin
  m_nCurFrame := 0;
  m_nMaxFrame := 10;
  m_sLoginId := '';
  m_sLoginPasswd := '';

  m_boOpenFirst := True;

  FrmDlg.OpenDLoginDlg;
  FrmDlg.CloseDNewAccountDlg;

  FrmDlg.CloseDRealNameDlg;
  FrmDlg.CloseDGetPwdBackDlg;
  FrmDlg.CloseDRegAccount;
  FrmDlg.CloseDBindPhone;
  m_boNowOpening := False;

  if (g_ClientVersion = cv176) or g_ConfigClient.boPlayOldVerSound then
    PlayBGM(bmg_gameover176)
  else
    PlayBGM(bmg_intro);
end;

procedure TLoginScene.CloseScene;
begin
  // FrmDlg.DEdId.Visible := FALSE;
 // FrmDlg.DEdPasswd.Visible := FALSE;
  m_boNowOpening := False;
  g_boDoFadeOut := False;
  FrmDlg.CloseDLoginDlg;
  SilenceSound;
end;

procedure TLoginScene.PassWdFail(FailCode:Integer);
begin
  FrmDlg.DEdId.Visible := True;
  FrmDlg.DEdPasswd.Visible := True;
  if FailCode = -1 then begin // 密码错误
    FrmDlg.DEdPasswd.Text := '';
    FrmDlg.DEdPasswd.SetFocus;
  end else if FailCode = -3 then begin // 此帐号已经登录或被异常锁定，请稍候再登录
    // FrmDlg.DEdPasswd.Text := '' ;
    FrmDlg.DEdPasswd.SetFocus;
  end else if FailCode = -100 then begin
    FrmDlg.DEdId.SetFocus;
  end else begin
    FrmDlg.DEdId.Text := '';
    FrmDlg.DEdPasswd.Text := '';
    FrmDlg.DEdId.SetFocus;
  end;
end;

procedure TLoginScene.HideLoginBox;
begin
  // EdId.Visible := FALSE;
  // EdPasswd.Visible := FALSE;
  // FrmDlg.DLogin.Visible := FALSE;
  ChangeLoginState(lsCloseAll);
end;

// 选择服务器成功，执行开门动作

procedure TLoginScene.OpenLoginDoor;
begin
  if (g_ConfigClient.boShowOpenDoor) then begin
    FrmDlg.OpenDDoorDlg;
    FrmDlg.CloseDSelServerDlg;
  end
  else begin
    m_boNowOpening := True;
    m_dwStartTime := MyGetTickCount;
    HideLoginBox;
    FrmDlg.CloseDSelServerDlg;
    PlaySound(s_rock_door_open);

    DScreen.ChangeScene(stSelectChr);

    g_nFadeIndex := 1;
    g_boDoFadeIn := True;
    g_boDoFadeOut := False;
  end;
end;

procedure TLoginScene.RenderScene(Sender:TObject);
begin
  // GameCanvas.FillRect(Bounds(0, 0, SCREENWIDTH, SCREENHEIGHT), BackgroundColor);
end;

procedure TLoginScene.ChangeLoginState(State:TLoginState);
begin
  case State of
    lsLogin:begin
        FrmDlg.CloseDNewAccountDlg;
        FrmDlg.CloseDChgPwDlg;
        FrmDlg.OpenDLoginDlg;
        FrmDlg.CloseDRealNameDlg;
        FrmDlg.CloseDGetPwdBackDlg;
        FrmDlg.CloseDRegAccount;
        FrmDlg.CloseDBindPhone;

        if FrmDlg.DEdId.Visible then
          FrmDlg.DEdId.SetFocus;
      end;
    lsNewidRetry, lsNewid:begin
        if m_boUpdateAccountMode then
          FrmDlg.DEdNewId.Enabled := False
        else
          FrmDlg.DEdNewId.Enabled := True;

        FrmDlg.OpenDNewAccountDlg;
        FrmDlg.CloseDChgPwDlg;
        FrmDlg.CloseDLoginDlg;
        FrmDlg.CloseDRealNameDlg;
        FrmDlg.CloseDGetPwdBackDlg;
        FrmDlg.CloseDRegAccount;
        FrmDlg.CloseDBindPhone;
        if FrmDlg.DEdNewId.Visible and FrmDlg.DEdNewId.Enabled then begin
          FrmDlg.DEdNewId.SetFocus;
        end
        else begin
          if FrmDlg.DEdConfirm.Visible and FrmDlg.DEdConfirm.Enabled then
            FrmDlg.DEdConfirm.SetFocus;
        end;
      end;
    lsChgpw:begin
        FrmDlg.CloseDNewAccountDlg;
        FrmDlg.OpenDChgPwDlg;
        FrmDlg.CloseDLoginDlg;
        FrmDlg.CloseDRealNameDlg;
        FrmDlg.CloseDGetPwdBackDlg;
        FrmDlg.CloseDRegAccount;
        FrmDlg.CloseDBindPhone;
        {
        if FrmDlg.DEdChgId.Visible then FrmDlg.DEdChgId.SetFocus;
        }
      end;
    lsRealName:begin
        FrmDlg.CloseDNewAccountDlg;
        FrmDlg.CloseDChgPwDlg;
        FrmDlg.CloseDLoginDlg;
        FrmDlg.CloseDGetPwdBackDlg;
        FrmDlg.CloseDRegAccount;
        FrmDlg.CloseDBindPhone;
        FrmDlg.OpenDRealNameDlg;
      end;
    lsBindPhone:begin
        FrmDlg.CloseDNewAccountDlg;
        FrmDlg.CloseDChgPwDlg;
        FrmDlg.CloseDLoginDlg;
        FrmDlg.CloseDGetPwdBackDlg;
        FrmDlg.CloseDRegAccount;
        FrmDlg.CloseDRealNameDlg;
        FrmDlg.OpenDBindPhone;
      end;
    lsCloseAll:begin
        FrmDlg.CloseDNewAccountDlg;
        FrmDlg.CloseDChgPwDlg;
        FrmDlg.CloseDLoginDlg;
        FrmDlg.CloseDRealNameDlg;
        FrmDlg.CloseDGetPwdBackDlg;
        FrmDlg.CloseDRegAccount;
        FrmDlg.CloseDBindPhone;
      end;
  end;
end;

procedure TLoginScene.NewClick;
begin
  m_boUpdateAccountMode := FALSE;
  FrmDlg.NewAccountTitle := '';
  ChangeLoginState(lsNewid);
end;

procedure TLoginScene.NewIdRetry(boupdate:Boolean);
begin
  m_boUpdateAccountMode := boupdate;
  ChangeLoginState(lsNewidRetry);
  FrmDlg.NewIdRetry(m_NewIdRetryUE, m_NewIdRetryAdd);
end;

procedure TLoginScene.RealName;
begin
  ChangeLoginState(lsRealName);
end;

procedure TLoginScene.BindPhone;
begin
  ChangeLoginState(lsBindPhone);
end;

procedure TLoginScene.UpdateAccountInfos(ue:TUserEntry; ua:TUserEntryAdd);
begin
  m_NewIdRetryUE := ue;
  m_NewIdRetryAdd := ua;
  m_boUpdateAccountMode := True;
  NewIdRetry(True);
  FrmDlg.NewAccountTitle := '(请填写帐号相关信息。)';
end;

procedure TLoginScene.ChgPwClick;
begin
  ChangeLoginState(lsChgpw);
end;

procedure TLoginScene.NewAccountOk;
var
  ue:TUserEntry;
  ua:TUserEntryAdd;
begin
  if FrmDlg.CheckUserEntrys then begin
    FillChar(ue, SizeOf(TUserEntry), #0);
    FillChar(ua, SizeOf(TUserEntryAdd), #0);

    FrmDlg.NewAccountOk(@ue, @ua);

    { ue.sAccount := LowerCase(FrmDlg.DEdNewId.Text);
     ue.sPassword := FrmDlg.DEdNewPasswd.Text;
     ue.sUserName := FrmDlg.DEdYourName.Text;
       //
     if not EnglishVersion then
       ue.sSSNo := FrmDlg.DEdSSNo.Text
     else
       ue.sSSNo := '650101-1455111';

     ue.sQuiz := FrmDlg.DEdQuiz1.Text;
     ue.sAnswer := Trim(FrmDlg.DEdAnswer1.Text);
     ue.sPhone := FrmDlg.DEdPhone.Text;
     ue.sEMail := Trim(FrmDlg.DEdEMail.Text);

     ua.sQuiz2 := FrmDlg.DEdQuiz2.Text;
     ua.sAnswer2 := Trim(FrmDlg.DEdAnswer2.Text);
     ua.sBirthDay := FrmDlg.DEdBirthDay.Text;
     ua.sMobilePhone := FrmDlg.DEdMobPhone.Text; }
    if EnglishVersion then
      ue.sSSNo := '650101-1455111';

    m_NewIdRetryUE := ue;
    m_NewIdRetryUE.sAccount := '';
    m_NewIdRetryUE.sPassword := '';
    m_NewIdRetryAdd := ua;

    if not m_boUpdateAccountMode then
      frmMain.SendNewAccount(ue, ua)
    else
      frmMain.SendUpdateAccount(ue, ua);
    m_boUpdateAccountMode := FALSE;
    NewAccountClose;
  end;
end;

procedure TLoginScene.NewAccountClose;
begin
  if not m_boUpdateAccountMode then begin
    ChangeLoginState(lsLogin);
    {FrmDlg.DEdId.Visible:=TRUE;
    FrmDlg.DEdPasswd.Visible:=TRUE;
    FrmDlg.DEdId.SetFocus;
    }
  end;
end;

procedure TLoginScene.ChgpwOk;
begin

end;

procedure TLoginScene.ChgpwCancel;
begin
  ChangeLoginState(lsLogin);
end;

{-------------------- TSelectChrScene ------------------------}

constructor TSelectChrScene.Create;
var
  I:Integer;
begin
  CreateChrMode := False;
  SafeFillChar(ChrArr, SizeOf(TSelChar) * (High(ChrArr) + 1), #0);
  for I := 0 to High(ChrArr) do begin
    ChrArr[I].FreezeState := True;
  end;
  //  ChrArr[0].FreezeState := True;
  //  ChrArr[1].FreezeState := True;
  //  ChrArr[2].FreezeState := True;
  NewIndex := 0;
  m_nChrPage := 0;
  m_nHighIndex := 0;
  {
  SoundTimer := TTimer.Create(frmMain.Owner);
  with SoundTimer do
  begin
    OnTimer := SoundOnTimer;
    Interval := 1;
    Enabled := False;
  end;
  }
  inherited Create(stSelectChr);
end;

destructor TSelectChrScene.Destroy;
begin
  inherited Destroy;
end;

procedure TSelectChrScene.RenderScene(Sender:TObject);
begin
  // GameCanvas.FillRect(Bounds(0, 0, SCREENWIDTH, SCREENHEIGHT), BackgroundColor);
end;

procedure TSelectChrScene.OpenScene;
begin
  m_nCurFrame := 0;
  m_nMaxFrame := 10;
  FrmDlg.OpenDSelectChrDlg;
  //SoundTimer.Enabled := True;
  //SoundTimer.Interval := 1;
  g_nFadeIndex := 1;
  g_boDoFadeIn := True;
  g_boDoFadeOut := False;
end;

procedure TSelectChrScene.CloseScene;
begin
  // 这里只关背景音（开始点击的声音不要关 RRRRRR） chongchong 2015-05-11
  //SilenceSound;
  g_PlaySound.Clear(2);
  FrmDlg.CloseDSelectChrDlg;
  //SoundTimer.Enabled := False;
end;

procedure TSelectChrScene.SoundOnTimer(Sender:TObject);
begin
  //PlayBGM(bmg_select);
  //SoundTimer.Enabled := False;
end;

procedure TSelectChrScene.SelChrSelect1Click;
var
  I:Integer;
begin
  if (not ChrArr[m_nChrPage * 2].Selected) and (ChrArr[m_nChrPage * 2].Valid) then begin
    frmMain.SelectChr(ChrArr[m_nChrPage * 2].UserChr.Name);

    for I := 0 to High(ChrArr) do begin
      ChrArr[I].Selected := False;
      ChrArr[I].FreezeState := True;
      if ChrArr[I].Selected then begin
        ChrArr[I].Selected := False;
        ChrArr[I].FreezeState := True;
      end;
    end;

    ChrArr[m_nChrPage * 2].Selected := True; // 是否选择
    ChrArr[m_nChrPage * 2].FreezeState := False; // 是否是禁用状态
    ChrArr[m_nChrPage * 2].Unfreezing := True; // 是否正在启用人物
    ChrArr[m_nChrPage * 2].Freezing := False; // 是否正在禁用人物

    //    ChrArr[m_nChrPage * 2 + 1].FreezeState := True;                                                                  // 是否是禁用状态
    //    ChrArr[m_nChrPage * 2 + 1].Unfreezing := False;                                                                  // 是否正在启用人物
    //    ChrArr[m_nChrPage * 2 + 1].Freezing := ChrArr[m_nChrPage * 2 + 1].Selected;                                                       // 是否正在禁用人物
    //    ChrArr[m_nChrPage * 2 + 1].Selected := False;                                                                    // 是否选择
    //    ChrArr[2].FreezeState := True;                                                                  // 是否是禁用状态
    //    ChrArr[2].Unfreezing := False;                                                                  // 是否正在启用人物
    //    ChrArr[2].Freezing := ChrArr[2].Selected;                                                       // 是否正在禁用人物
    //    ChrArr[2].Selected := False;                                                                    // 是否选择

    ChrArr[m_nChrPage * 2].AniIndex := 0;
    ChrArr[m_nChrPage * 2].DarkLevel := 0;
    ChrArr[m_nChrPage * 2].EffIndex := 0;
    ChrArr[m_nChrPage * 2].StartTime := MyGetTickCount;
    ChrArr[m_nChrPage * 2].moretime := MyGetTickCount;
    ChrArr[m_nChrPage * 2].startefftime := MyGetTickCount;
    PlaySound(s_meltstone);
  end;
end;

procedure TSelectChrScene.SelChrSelect2Click;
var
  I:Integer;
begin
  if (not ChrArr[m_nChrPage * 2 + 1].Selected) and (ChrArr[m_nChrPage * 2 + 1].Valid) then begin
    frmMain.SelectChr(ChrArr[m_nChrPage * 2 + 1].UserChr.Name);

    for I := 0 to High(ChrArr) do begin
      ChrArr[I].Selected := False;
      ChrArr[I].FreezeState := True;
      if ChrArr[I].Selected then begin
        ChrArr[I].Selected := False;
        ChrArr[I].FreezeState := True;
      end;
    end;
    //    ChrArr[m_nChrPage * 2].FreezeState := True;                                                                  // 是否是禁用状态
    //    ChrArr[m_nChrPage * 2].Unfreezing := False;                                                                  // 是否正在启用人物
    //    ChrArr[m_nChrPage * 2].Freezing := ChrArr[m_nChrPage * 2].Selected;                                                       // 是否正在禁用人物
    //    ChrArr[m_nChrPage * 2].Selected := False;                                                                    // 是否选择

    ChrArr[m_nChrPage * 2 + 1].Selected := True; // 是否选择
    ChrArr[m_nChrPage * 2 + 1].FreezeState := False; // 是否是禁用状态
    ChrArr[m_nChrPage * 2 + 1].Unfreezing := True; // 是否正在启用人物
    ChrArr[m_nChrPage * 2 + 1].Freezing := False; // 是否正在禁用人物
    //    ChrArr[2].FreezeState := True;                                                                  // 是否是禁用状态
    //    ChrArr[2].Unfreezing := False;                                                                  // 是否正在启用人物
    //    ChrArr[2].Freezing := ChrArr[2].Selected;                                                       // 是否正在禁用人物
    //    ChrArr[2].Selected := False;                                                                    // 是否选择

    ChrArr[m_nChrPage * 2 + 1].AniIndex := 0;
    ChrArr[m_nChrPage * 2 + 1].DarkLevel := 0;
    ChrArr[m_nChrPage * 2 + 1].EffIndex := 0;
    ChrArr[m_nChrPage * 2 + 1].StartTime := MyGetTickCount;
    ChrArr[m_nChrPage * 2 + 1].moretime := MyGetTickCount;
    ChrArr[m_nChrPage * 2 + 1].startefftime := MyGetTickCount;
    PlaySound(s_meltstone);
  end;
end;

procedure TSelectChrScene.SelChrSelect3Click;
begin
  if (not ChrArr[2].Selected) and (ChrArr[2].Valid) then begin
    frmMain.SelectChr(ChrArr[2].UserChr.Name);
    ChrArr[0].FreezeState := True; // 是否是禁用状态
    ChrArr[0].Unfreezing := False; // 是否正在启用人物
    ChrArr[0].Freezing := ChrArr[0].Selected; // 是否正在禁用人物
    ChrArr[0].Selected := False; // 是否选择

    ChrArr[1].FreezeState := True; // 是否是禁用状态
    ChrArr[1].Unfreezing := False; // 是否正在启用人物
    ChrArr[1].Freezing := ChrArr[1].Selected; // 是否正在禁用人物
    ChrArr[1].Selected := False; // 是否选择

    ChrArr[2].Selected := True; // 是否选择
    ChrArr[2].FreezeState := False; // 是否是禁用状态
    ChrArr[2].Unfreezing := True; // 是否正在启用人物
    ChrArr[2].Freezing := False; // 是否正在禁用人物

    ChrArr[2].Selected := True;
    ChrArr[2].Unfreezing := True;
    ChrArr[2].AniIndex := 0;
    ChrArr[2].DarkLevel := 0;
    ChrArr[2].EffIndex := 0;
    ChrArr[2].StartTime := MyGetTickCount;
    ChrArr[2].moretime := MyGetTickCount;
    ChrArr[2].startefftime := MyGetTickCount;
    PlaySound(s_meltstone);
  end;
end;

procedure TSelectChrScene.SelChrStartClick;
var
  I:Integer;
  chrname:string;
begin
  chrname := '';
  I := 0;
  //  if ChrArr[0].Valid and ChrArr[0].Selected then chrname := ChrArr[0].UserChr.Name;
  //  if ChrArr[1].Valid and ChrArr[1].Selected then chrname := ChrArr[1].UserChr.Name;
  //  if ChrArr[2].Valid and ChrArr[2].Selected then chrname := ChrArr[2].UserChr.Name;

  while True do begin
    if ChrArr[I].Valid and ChrArr[I].Selected then begin
      chrname := ChrArr[I].UserChr.Name;
      Break;
    end;
    Inc(I);
    if I > High(ChrArr) then
      Break;
  end;

  if chrname <> '' then begin
    if not g_boDoFadeOut and not g_boDoFadeIn then begin
      // g_boDoFastFadeOut := True;
      // g_nFadeIndex := 29;
    end;
    // g_sSelChrName := chrname;
    frmMain.SendSelChr(chrname);
  end
  else
    FrmDlg.DMessageDlg('还没创建游戏角色！' + #13#10 + '点击创建角色按钮创建一个游戏角色。', [mbOk]);
end;

procedure TSelectChrScene.SelChrNewChrClick;
begin
  m_nSelected := 0;

  // 修正当有一个角色时，不停的点“创建角色”然后直接关闭返回，第一个角色变成石化 chongchong 2015-12-11
  if ChrArr[0].Valid and ChrArr[0].Selected then
    m_nSelected := 0;
  if ChrArr[1].Valid and (ChrArr[1].UserChr.Name <> '') and ChrArr[1].Selected then
    m_nSelected := 1;
  if ChrArr[2].Valid and (ChrArr[2].UserChr.Name <> '') and ChrArr[2].Selected then
    m_nSelected := 2;

  {
  if (g_ClientVersion >= cvMirs) and (g_ClientVersion <> cvMirNewUI205) then
  begin
    if (not ChrArr[0].Valid) or (not ChrArr[1].Valid) or (not ChrArr[2].Valid) then
    begin
      if not ChrArr[0].Valid then
        MakeNewChar(0)
      else if not ChrArr[1].Valid then
        MakeNewChar(1)
      else
        MakeNewChar(2);
    end
    else
      FrmDlg.DMessageDlg('你可以为每个单独的账号创建三个角色。', [mbOk]);
  end
  else
  }
//  begin
//    if ((not ChrArr[0].Valid) or (ChrArr[0].UserChr.Name = '')) or ((not ChrArr[1].Valid) or (ChrArr[1].UserChr.Name = '')) then
//    begin
//      if (not ChrArr[0].Valid) or (ChrArr[0].UserChr.Name = '') then
//        MakeNewChar(0)
//      else
//        MakeNewChar(1);
//    end
//    else
//      FrmDlg.DMessageDlg('你可以为每个单独的账号创建两个角色。', [mbOk]);
//  end;
  if m_nHighIndex > High(ChrArr) then
    FrmDlg.DMessageDlg('可创建角色已到上限。', [mbOk])
  else
    MakeNewChar(m_nHighIndex + 1);
end;

procedure TSelectChrScene.SelChrEraseChrClick;
var
  n:Integer;
  S:string;
begin
  n := 0;
  while True do begin
    if ChrArr[n].Valid and ChrArr[n].Selected then
      Break;
    Inc(n);
    if n > High(ChrArr) then begin
      n := -1;
      Break;
    end;
  end;
  if n < 0 then
    Exit;
  //  if ChrArr[0].Valid and ChrArr[0].Selected then n := 0;
  //  if ChrArr[1].Valid and ChrArr[1].Selected then n := 1;
  //  if ChrArr[2].Valid and ChrArr[2].Selected then n := 2;

  if (ChrArr[n].Valid) and (not ChrArr[n].FreezeState) and (ChrArr[n].UserChr.Name <> '') then begin
    if g_ClientVersion <> cvMirNewUI205 then begin
      S := '"' + ChrArr[n].UserChr.Name + '" 删除的角色是不能被恢复的。' + sLineBreak + '一段时间内，你将不能使用相同的角色名。' + sLineBreak + '你真的想要删除角色吗？';

      if mrOk = FrmDlg.DMessageDlgDeleteUser(S) then
        frmMain.SendDelChr(ChrArr[n].UserChr.Name);
    end
    else begin
      S := '"' + ChrArr[n].UserChr.Name + '" 删除的角色是不能被恢复的。' + sLineBreak + sLineBreak + '一段时间内，你将不能使用相同的角色名。' + sLineBreak + sLineBreak + '你真的想要删除角色吗？';

      if mrOk = FrmDlg.DMessageDlgDeleteUser(S) then
        frmMain.SendDelChr(ChrArr[n].UserChr.Name);
    end;
  end;
end;

procedure TSelectChrScene.SelChrCreditsClick; // 找回人物
begin
  if not FrmDlg.DListViewDeleteHuman.Owner.Visible then
    frmMain.SendQueryDeleteChr;
end;

procedure TSelectChrScene.SelChrExitClick;
begin
  frmMain.Close;
end;

procedure TSelectChrScene.ClearChrs;
var
  I:Integer;
begin
  SafeFillChar(ChrArr, SizeOf(TSelChar) * (High(ChrArr) + 1), #0);

  for I := 0 to High(ChrArr) do begin
    ChrArr[I].FreezeState := False;
    ChrArr[I].Selected := False;
    ChrArr[I].UserChr.Name := '';
  end;

  ChrArr[m_nChrPage * 2].FreezeState := False;
  //  ChrArr[1].FreezeState := True;
  //  ChrArr[2].FreezeState := True;
  //
  ChrArr[m_nChrPage * 2].Selected := True;
  //  ChrArr[1].Selected := False;
  //  ChrArr[2].Selected := False;
  //
  //  ChrArr[0].UserChr.Name := '';
  //  ChrArr[1].UserChr.Name := '';
  //  ChrArr[2].UserChr.Name := '';
end;

procedure TSelectChrScene.AddChr(uname:string; job, hair, Level, sex:Integer);
var
  n:Integer;
begin
  n := 0;
  while True do begin
    if not ChrArr[n].Valid then
      Break;
    Inc(n);
    if n > High(ChrArr) then begin
      n := -1;
      Break;
    end;
  end;

  if n < 0 then
    Exit;
  //  if not ChrArr[0].Valid then
  //    n := 0
  //  else if not ChrArr[1].Valid then
  //    n := 1
  //  else if not ChrArr[2].Valid then
  //    n := 2
  //  else
  //    Exit;
  ChrArr[n].UserChr.Name := uname;
  ChrArr[n].UserChr.job := job;
  ChrArr[n].UserChr.hair := hair;
  ChrArr[n].UserChr.Level := Level;
  ChrArr[n].UserChr.sex := sex;
  ChrArr[n].Valid := True;
end;

procedure TSelectChrScene.MakeNewChar(Index:Integer);
begin
  CreateChrMode := True;
  NewIndex := Index;
  SafeFillChar(ChrArr[NewIndex].UserChr, SizeOf(TUserCharacterInfo), #0);
  FrmDlg.MakeNewChar(Index, @ChrArr[NewIndex].UserChr);
  ChrArr[NewIndex].Valid := True;
  ChrArr[NewIndex].FreezeState := False;

  SelectChr(NewIndex);

end;

procedure TSelectChrScene.SelectChr(Index:Integer);
begin
  ChrArr[Index].Selected := True;
  ChrArr[Index].DarkLevel := 30;
  ChrArr[Index].StartTime := MyGetTickCount;
  ChrArr[Index].moretime := MyGetTickCount;
  if Index = 0 then begin
    ChrArr[1].Selected := False;
    ChrArr[2].Selected := False;
  end
  else if Index = 1 then begin
    ChrArr[0].Selected := False;
    ChrArr[2].Selected := False;
  end
  else begin
    ChrArr[0].Selected := False;
    ChrArr[1].Selected := False;
  end;
end;

procedure TSelectChrScene.SelChrNewClose;
begin
  ChrArr[NewIndex].Valid := False;
  CreateChrMode := False;
  FrmDlg.CloseDCreateChrDlg;

  ChrArr[m_nSelected].Selected := True;
  ChrArr[m_nSelected].FreezeState := False;

  {if NewIndex = 1 then begin
    ChrArr[0].Selected := True;
    ChrArr[0].FreezeState := False;
  end
  else  if NewIndex = 2 then begin
    ChrArr[0].Selected := True;
    ChrArr[0].FreezeState := False;
  end;}
end;

procedure TSelectChrScene.SelChrUp;
begin
  Dec(m_nChrPage);
  if m_nChrPage < 0 then
    m_nChrPage := 0;
  SelSetText;
end;

procedure TSelectChrScene.SelChrDown;
begin
  if not ChrArr[(m_nChrPage + 1) * 2].Valid then
    Exit;
  Inc(m_nChrPage);
  if m_nChrPage > High(ChrArr) then
    m_nChrPage := High(ChrArr);
  SelSetText;
end;

procedure TSelectChrScene.SelSetText;
begin
  if ChrArr[m_nChrPage * 2].Valid then begin
    if (FrmDlg.LabelSelectChrDlgCharName1 <> nil) then begin
      FrmDlg.LabelSelectChrDlgCharName1.Caption := ChrArr[m_nChrPage * 2].UserChr.Name;
    end;
    if (FrmDlg.LabelSelectChrDlgLevel1 <> nil) then begin
      FrmDlg.LabelSelectChrDlgLevel1.Caption := IntToStr(ChrArr[m_nChrPage * 2].UserChr.Level);
    end;
    if (FrmDlg.LabelSelectChrDlgJob1 <> nil) then begin
      FrmDlg.LabelSelectChrDlgJob1.Caption := GetJobName(ChrArr[m_nChrPage * 2].UserChr.Job);
    end;
  end
  else begin
    if (FrmDlg.LabelSelectChrDlgCharName1 <> nil) then
      FrmDlg.LabelSelectChrDlgCharName1.Caption := '';

    if (FrmDlg.LabelSelectChrDlgLevel1 <> nil) then
      FrmDlg.LabelSelectChrDlgLevel1.Caption := '';

    if (FrmDlg.LabelSelectChrDlgJob1 <> nil) then
      FrmDlg.LabelSelectChrDlgJob1.Caption := '';
  end;

  if ChrArr[m_nChrPage * 2 + 1].Valid then begin
    if (FrmDlg.LabelSelectChrDlgCharName2 <> nil) then begin
      FrmDlg.LabelSelectChrDlgCharName2.Caption := ChrArr[m_nChrPage * 2 + 1].UserChr.Name;
    end;
    if (FrmDlg.LabelSelectChrDlgLevel2 <> nil) then begin
      FrmDlg.LabelSelectChrDlgLevel2.Caption := IntToStr(ChrArr[m_nChrPage * 2 + 1].UserChr.Level);
    end;
    if (FrmDlg.LabelSelectChrDlgJob2 <> nil) then begin
      FrmDlg.LabelSelectChrDlgJob2.Caption := GetJobName(ChrArr[m_nChrPage * 2 + 1].UserChr.Job);
    end;
  end
  else begin
    if (FrmDlg.LabelSelectChrDlgCharName2 <> nil) then
      FrmDlg.LabelSelectChrDlgCharName2.Caption := '';

    if (FrmDlg.LabelSelectChrDlgLevel2 <> nil) then
      FrmDlg.LabelSelectChrDlgLevel2.Caption := '';

    if (FrmDlg.LabelSelectChrDlgJob2 <> nil) then
      FrmDlg.LabelSelectChrDlgJob2.Caption := '';
  end;

  if ChrArr[2].Valid then begin
    if (FrmDlg.LabelSelectChrDlgCharName3 <> nil) then begin
      FrmDlg.LabelSelectChrDlgCharName3.Caption := ChrArr[2].UserChr.Name;
    end;
    if (FrmDlg.LabelSelectChrDlgLevel3 <> nil) then begin
      FrmDlg.LabelSelectChrDlgLevel3.Caption := IntToStr(ChrArr[2].UserChr.Level);
    end;
    if (FrmDlg.LabelSelectChrDlgJob3 <> nil) then begin
      FrmDlg.LabelSelectChrDlgJob3.Caption := GetJobName(ChrArr[2].UserChr.Job);
    end;
  end
  else begin
    if (FrmDlg.LabelSelectChrDlgCharName3 <> nil) then
      FrmDlg.LabelSelectChrDlgCharName3.Caption := '';

    if (FrmDlg.LabelSelectChrDlgLevel3 <> nil) then
      FrmDlg.LabelSelectChrDlgLevel3.Caption := '';

    if (FrmDlg.LabelSelectChrDlgJob3 <> nil) then
      FrmDlg.LabelSelectChrDlgJob3.Caption := '';
  end;
end;

procedure TSelectChrScene.SelChrNewOk;
var
  chrname, shair, sjob, ssex:string;
begin
  chrname := Trim(FrmDlg.DEdChrName.Text);

  if Length(chrname) < 4 then begin
    FrmDlg.DMessageDlg('角色名称长度最低4个字符或2汉字！', [mbOk]);
    Exit;
  end;

  begin
    ChrArr[NewIndex].Valid := False;
    CreateChrMode := False;
    FrmDlg.CloseDCreateChrDlg;

    ChrArr[m_nSelected].Selected := True;
    ChrArr[m_nSelected].FreezeState := False;

    {if NewIndex = 1 then begin
      ChrArr[0].Selected := True;
      ChrArr[0].FreezeState := False;
    end;   }
    // 发型处理
    case ChrArr[NewIndex].UserChr.sex of
      0:begin
          shair := '2';
        end;
      1:begin
          Randomize;
          shair := '3';
          {  case Random(2) of
              1: shair := '1';
              2: shair := '3';
            else shair := '1';
            end;}
        end;
    end;
    // 发型修改为老客户端一样的处理
    // shair := IntToStr(Random(5) + 1); //////****IntToStr(ChrArr[NewIndex].UserChr.Hair);
    sjob := IntToStr(ChrArr[NewIndex].UserChr.job);
    ssex := IntToStr(ChrArr[NewIndex].UserChr.sex);
    frmMain.SendNewChr(frmMain.LoginID, chrname, shair, sjob, ssex);
  end;
end;

procedure TSelectChrScene.SelChrNewJob(job:Integer);
begin
  if (job in [0..2]) and (ChrArr[NewIndex].UserChr.job <> job) then begin
    ChrArr[NewIndex].UserChr.job := job;
    SelectChr(NewIndex);
  end;
end;

procedure TSelectChrScene.SelChrNewSex(sex:Integer);
begin
  if sex <> ChrArr[NewIndex].UserChr.sex then begin
    ChrArr[NewIndex].UserChr.sex := sex;
    SelectChr(NewIndex);
  end;
end;

{--------------------------- TLoginNotice ----------------------------}

constructor TLoginNotice.Create;
begin
  inherited Create(stLoginNotice);
end;

destructor TLoginNotice.Destroy;
begin
  inherited Destroy;
end;

end.
