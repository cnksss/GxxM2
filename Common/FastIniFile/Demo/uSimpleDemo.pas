unit uSimpleDemo;

{.$DEFINE TESTMODE}

{$DEFINE INIFILE}
{$DEFINE MEMINIFILE}
{$DEFINE FASTINIFILE_ANSI}
{$DEFINE FASTINIFILE}

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ComCtrls, Spin, Buttons;

type
  TfrmDemo = class(TForm)
    btnExit: TButton;
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    btnWrite: TButton;
    btnRandomRead: TButton;
    btnRead: TButton;
    spnSections: TSpinEdit;
    spnIdents: TSpinEdit;
    btnReadEverything: TButton;
    btnWriteOne: TButton;
    btnDelete: TBitBtn;
    lblResults: TLabel;
    mmoResults: TMemo;
    TabSheet2: TTabSheet;
    rchDemo: TRichEdit;
    btnReadValueAndComment: TButton;
    lblValue: TLabel;
    edtValue: TEdit;
    lblComment: TLabel;
    edtComment: TEdit;
    btnReadQuotedValues: TButton;
    lblQuotedValues: TLabel;
    edtQuotedValues: TEdit;
    btnReadWriteFixedDateTime: TButton;
    lblDateTime: TLabel;
    edtDateTime: TEdit;
    btnWriteStrings: TButton;
    lbxRecent1: TListBox;
    btnReadStrings: TButton;
    lbxRecent2: TListBox;
    TabSheet3: TTabSheet;
    lblOriginal: TLabel;
    rchComp: TRichEdit;
    Label1: TLabel;
    rchComp1: TRichEdit;
    Label2: TLabel;
    rchComp2: TRichEdit;
    Label3: TLabel;
    rchComp3: TRichEdit;
    Label5: TLabel;
    Memo1: TMemo;
    btnCheckEmptyReadDefault: TButton;
    btnReadSection: TButton;
    btnReadSectionValues: TButton;
    btnTestIfAnsiCompareIsNeeded: TButton;
    btnTestReadBackSpaces: TButton;
    btnEraseHalf: TButton;
    chkIdents: TCheckBox;
    chkSections: TCheckBox;
    pgbResults: TProgressBar;
    btnAbort: TSpeedButton;
    btnWriteFont: TButton;
    btnReadFont: TButton;
    btnRestoreFont: TButton;
    btnWriteUsingTStringList: TButton;
    btnCreateWriteFree: TButton;
    btnCreateReadFree: TButton;
    btnWriteUsingSetStrings: TButton;
    btnEraseLastSection: TButton;
    btnCreateEraseRewrite: TButton;
    btnStatistics: TButton;
    procedure FormShow(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure btnExitClick(Sender: TObject);
    procedure spnSectionsChange(Sender: TObject);
    procedure btnWriteClick(Sender: TObject);
    procedure btnRandomReadClick(Sender: TObject);
    procedure btnReadClick(Sender: TObject);
    procedure btnReadEverythingClick(Sender: TObject);
    procedure btnWriteOneClick(Sender: TObject);
    procedure btnDeleteClick(Sender: TObject);
    procedure btnReadValueAndCommentClick(Sender: TObject);
    procedure btnReadQuotedValuesClick(Sender: TObject);
    procedure btnReadWriteFixedDateTimeClick(Sender: TObject);
    procedure btnWriteStringsClick(Sender: TObject);
    procedure btnReadStringsClick(Sender: TObject);
    procedure btnCheckEmptyReadDefaultClick(Sender: TObject);
    procedure btnTestIfAnsiCompareIsNeededClick(Sender: TObject);
    procedure btnTestReadBackSpacesClick(Sender: TObject);
    procedure btnReadSectionClick(Sender: TObject);
    procedure btnReadSectionValuesClick(Sender: TObject);
    procedure btnEraseHalfClick(Sender: TObject);
    procedure btnCreateEraseRewriteClick(Sender: TObject);
    procedure btnEraseLastSectionClick(Sender: TObject);
    procedure chkIdentsClick(Sender: TObject);
    procedure btnAbortClick(Sender: TObject);
    procedure btnWriteFontClick(Sender: TObject);
    procedure btnReadFontClick(Sender: TObject);
    procedure btnRestoreFontClick(Sender: TObject);
    procedure btnWriteUsingTStringListClick(Sender: TObject);
    procedure btnCreateWriteFreeClick(Sender: TObject);
    procedure btnCreateReadFreeClick(Sender: TObject);
    procedure btnWriteUsingSetStringsClick(Sender: TObject);
    procedure btnStatisticsClick(Sender: TObject);
  private
    { Private declarations }
    FAbort: Boolean;
    FFreq: Int64;
    FLast: Integer;
    procedure UpdateProgressBar(I: Integer);
    procedure Reload;
  public
    { Public declarations }
  end;

var
  frmDemo: TfrmDemo;

implementation

{$R *.DFM}

uses
  IniFiles, FastIniFile,
  uFormatRichEditText;

const
  CSECTION = 'Section';
  CIDENT   = 'Ident';

var
  ApplicationPath: string;
  // Nr of repetitions of sections and/or idents for tests
  // (Maximum for N*M is ±4000 for Standard TIniFile).
  N: Integer = 400;
  M: Integer = 10;

function iif(ABool: boolean; const varTrue, varFalse: integer): integer; overload;
begin
  if ABool then Result := varTrue else Result := varFalse;
end;

procedure TfrmDemo.Reload;
begin
  rchDemo.Lines.LoadFromFile(ApplicationPath + 'demo.ini');
  FormatRichEditText(rchDemo);
  // Put the cursor at the end of the INI-file.
  rchDemo.SelStart  := Length(rchDemo.Text);
  rchDemo.SelLength := 0;
  rchDemo.Perform(EM_SCROLLCARET, 0, 0);
end;

procedure TfrmDemo.FormCreate(Sender: TObject);
begin
  {$IFDEF TESTMODE}
    btnRandomRead.Enabled := True;
    btnRead.Enabled := True;
    btnReadEverything.Enabled := True;
    btnWriteOne.Enabled := True;
    btnEraseHalf.Enabled := True;
    btnCreateEraseRewrite.Enabled := True;
    btnCreateReadFree.Enabled := True;
  {$ELSE}
    btnDeleteClick(nil);
  {$ENDIF}
  QueryPerformanceFrequency(FFreq);
  CopyFile(PChar(ApplicationPath + 'demo.org'), PChar(ApplicationPath + 'demo.ini'), False);
end;

procedure TfrmDemo.FormShow(Sender: TObject);
begin
  Reload;

  spnSections.OnChange := nil;
  spnIdents.OnChange   := nil;
  try
    spnSections.Value := N;
    spnIdents.Value   := M;
  finally
    spnSections.OnChange := spnSectionsChange;
    spnIdents.OnChange   := spnSectionsChange;
  end;

  rchComp.Lines.LoadFromFile(ApplicationPath + 'TestComp.ini');
  FormatRichEditText(rchComp);
end;

procedure TfrmDemo.btnExitClick(Sender: TObject);
begin
  Close;
end;

// *****************************************************************************
// * Page: SPEED                                                               *
// *****************************************************************************

{Gets the number of bytes of virtual memory either reserved or committed by this
 process in K}
function GetAddressSpaceUsed: Cardinal;
var
  LMemoryStatus: TMemoryStatus;
begin
  {Set the structure size}
  LMemoryStatus.dwLength := SizeOf(LMemoryStatus);
  {Get the memory status}
  GlobalMemoryStatus(LMemoryStatus);
  {The result is the total address space less the free address space}
  Result := (LMemoryStatus.dwTotalVirtual - LMemoryStatus.dwAvailVirtual) shr 10;
end;

procedure TfrmDemo.chkIdentsClick(Sender: TObject);
begin
  if not chkIdents.Checked then
  begin
    spnSections.Value := 100;
    mmoResults.Lines.Add('');
    mmoResults.Lines.Add('When "Idents/Values" is switched off the sections will' +
                         ' have an increasing number of idents=value''s pairs.');
  end;
  spnIdents.Enabled := chkIdents.Checked;
  spnSectionsChange(Sender);
end;

procedure TfrmDemo.spnSectionsChange(Sender: TObject);
begin
  N := spnSections.Value;
  M := iif(chkIdents.Checked, spnIdents.Value, spnSections.Value);
  if FileExists(ApplicationPath + 'SpeedTest?.ini') then
  begin
    btnDelete.Font.Color := clBlue;
    btnDelete.Font.Style := [fsBold, fsUnderline];
    btnDelete.Enabled := True;
    btnWrite.Enabled := False;
    btnCreateWriteFree.Enabled := False;
  end;
  btnRandomRead.Enabled := False;
  btnRead.Enabled := False;
  btnReadEverything.Enabled := False;
  btnWriteOne.Enabled := False;
  btnEraseHalf.Enabled := False;
  btnCreateEraseRewrite.Enabled := False;
  btnEraseLastSection.Enabled := False;
  btnCreateReadFree.Enabled := False;
end;

procedure TfrmDemo.UpdateProgressBar(I: Integer);
var
  Curr: Integer;
begin
  Curr := 100 * I div N;
  if FLast <> Curr then
  begin
    FLast := Curr;
    pgbResults.Position := FLast;
    Application.ProcessMessages;
  end;
end;

procedure TfrmDemo.btnWriteClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('(Re)write '+IntToStr(N)+' sections each with '+IntToStr(M)+
                       ' ident=value''s');

  OutputDebugString('SAMPLING ON');

  FAbort := False;
  FLast  := -1;
  btnAbort.Visible := (N * M) > 100000;
  pgbResults.Visible := btnAbort.Visible;
{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        for I := 0 to N-1 do
        begin
          UpdateProgressBar(I);
          if FAbort then Exit;
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        end;
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      for I := 0 to N-1 do
      begin
        UpdateProgressBar(I);
        if FAbort then Exit;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      for I := 0 to N-1 do
      begin
        UpdateProgressBar(I);
        if FAbort then Exit;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      for I := 0 to N-1 do
      begin
        UpdateProgressBar(I);
        if FAbort then Exit;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  btnAbort.Visible := False;
  pgbResults.Visible := False;

  btnDelete.Enabled := True;
  btnRandomRead.Enabled := True;
  btnRead.Enabled := True;
  btnReadEverything.Enabled := True;
  btnWriteOne.Enabled := True;
  btnEraseHalf.Enabled := True;
  btnCreateEraseRewrite.Enabled := True;
  btnEraseLastSection.Enabled := True;
  btnCreateReadFree.Enabled := True;
  btnStatistics.Enabled := True;

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnRandomReadClick(Sender: TObject);
var
  I, J, K, L: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Randomly read '+IntToStr(N)+' sections each with '+IntToStr(M)+
                       ' ident=value''s.');

  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    RandSeed := 0;
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        for I := 0 to N-1 do
        begin
          for J := 0 to M-1 do
          begin
            K := Random(N);
            L := Random(iif(chkIdents.Checked, M, K));
            if ReadInteger(CSECTION+IntToStr(K), CIDENT+IntToStr(L), 0) <> (M*K+L) then
              mmoResults.Lines.Add('Random read failed - one item did not return the expected value.');
          end;
        end;
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  RandSeed := 0;
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      for I := 0 to N-1 do
      begin
        for J := 0 to M-1 do
        begin
          K := Random(N);
          L := Random(iif(chkIdents.Checked, M, K));
          if ReadInteger(CSECTION+IntToStr(K), CIDENT+IntToStr(L), 0) <> (M*K+L) then
            mmoResults.Lines.Add('Random read failed - one item did not return the expected value.');
        end;
      end;
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  RandSeed := 0;
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      for I := 0 to N-1 do
      begin
        for J := 0 to M-1 do
        begin
          K := Random(N);
          L := Random(iif(chkIdents.Checked, M, K));
          if ReadInteger(CSECTION+IntToStr(K), CIDENT+IntToStr(L), 0) <> (M*K+L) then
            mmoResults.Lines.Add('Random read failed - one item did not return the expected value.');
        end;
      end;
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  RandSeed := 0;
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      for I := 0 to N-1 do
      begin
        for J := 0 to M-1 do
        begin
          K := Random(N);
          L := Random(iif(chkIdents.Checked, M, K));
          if ReadInteger(CSECTION+IntToStr(K), CIDENT+IntToStr(L), 0) <> (M*K+L) then
            mmoResults.Lines.Add('Random read failed - one item did not return the expected value.');
        end;
      end;
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnReadClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Read '+IntToStr(N)+' sections each with '+IntToStr(M)+
                       ' ident=value''s');

  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        for I := 0 to N-1 do
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
              mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
            mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
            mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
            mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnReadEverythingClick(Sender: TObject);
var
  I, J: integer;
  Cnt1, Cnt2: Int64;
  Sections : TStringList;
  Idents   : TStringList;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Get all Sections, get all their Idents and read their Values');

  OutputDebugString('SAMPLING ON');

  Sections := TStringList.Create;
  Idents   := TStringList.Create;
  try
{$IFDEF INIFILE}
    if N*M <= 4000 then
    begin
      Sections.Clear;
      QueryPerformanceCounter(Cnt1);
      with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
        try
          ReadSections(Sections);
          if Sections.Count <> N then
            mmoResults.Lines.Add('ReadSections failed - it did not return the expected amount of sections');
          for I := 0 to Sections.Count-1 do
          begin
            ReadSection(Sections[I], Idents);
            if Idents.Count <> iif(chkIdents.Checked, M, Succ(I)) then
              mmoResults.Lines.Add('ReadSection failed - it did not return the expected amount of Idents');
            for J := 0 to Idents.Count-1 do
              if ReadInteger(Sections[I], Idents[J], 0) <> (M*I+J) then
                mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
          end;
          Mem := GetAddressSpaceUsed;
        finally
          Free;
        end;
      QueryPerformanceCounter(Cnt2);
      mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
    end;
{$ENDIF}

{$IFDEF MEMINIFILE}
    Sections.Clear;
    QueryPerformanceCounter(Cnt1);
    with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
      try
        ReadSections(Sections);
        if Sections.Count <> N then
          mmoResults.Lines.Add('ReadSections failed - it did not return the expected amount of sections');
        for I := 0 to Sections.Count-1 do
        begin
          ReadSection(Sections[I], Idents);
          if Idents.Count <> iif(chkIdents.Checked, M, Succ(I)) then
            mmoResults.Lines.Add('ReadSection failed - it did not return the expected amount of Idents');
          for J := 0 to Idents.Count-1 do
            if ReadInteger(Sections[I], Idents[J], 0) <> (M*I+J) then
              mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        end;
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
    Sections.Clear;
    QueryPerformanceCounter(Cnt1);
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
      try
        ReadSections(Sections);
        if Sections.Count <> N then
          mmoResults.Lines.Add('ReadSections failed - it did not return the expected amount of sections');
        for I := 0 to Sections.Count-1 do
        begin
          ReadSection(Sections[I], Idents);
          if Idents.Count <> iif(chkIdents.Checked, M, Succ(I)) then
            mmoResults.Lines.Add('ReadSection failed - it did not return the expected amount of Idents');
          for J := 0 to Idents.Count-1 do
            if ReadInteger(Sections[I], Idents[J], 0) <> (M*I+J) then
              mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        end;
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
    Sections.Clear;
    QueryPerformanceCounter(Cnt1);
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
      try
        InlineCommentsEnabled := False;
        UseAnsiCompare := False;
        ReadSections(Sections);
        if Sections.Count <> N then
          mmoResults.Lines.Add('ReadSections failed - it did not return the expected amount of sections');
        for I := 0 to Sections.Count-1 do
        begin
          ReadSection(Sections[I], Idents);
          if Idents.Count <> iif(chkIdents.Checked, M, Succ(I)) then
            mmoResults.Lines.Add('ReadSection failed - it did not return the expected amount of Idents');
          for J := 0 to Idents.Count-1 do
            if ReadInteger(Sections[I], Idents[J], 0) <> (M*I+J) then
              mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        end;
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  finally
    Idents.Free;
    Sections.Free;
  end;
  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnWriteOneClick(Sender: TObject);
var
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Write/Change from a existing file with '+IntToStr(N)+' sections'+
                       ' each with '+IntToStr(M)+' ident=value''s, one value');

  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        WriteString(CSECTION+IntToStr(N div 2), CIDENT+IntToStr(M div 2), 'Hello');
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      WriteString(CSECTION+IntToStr(N div 2), CIDENT+IntToStr(M div 2), 'Hello');
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      WriteString(CSECTION+IntToStr(N div 2), CIDENT+IntToStr(M div 2), 'Hello');
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      WriteString(CSECTION+IntToStr(N div 2), CIDENT+IntToStr(M div 2), 'Hello');
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnEraseHalfClick(Sender: TObject);
var
  I, J: integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  // ReadEverything can't be used after this since that function
  // expects the sections and idents to be in order.
  btnWrite.Enabled := False;
  btnCreateWriteFree.Enabled := False;
  btnReadEverything.Enabled := False;

  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Erase half of the ('+IntToStr(N)+') sections,' +
                       ' of the remaining sections delete 5 of the' +
                       ' ('+IntToStr(M)+') keys and then rewrite' +
                       ' everything again');

  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        for I := 0 to N-1 do
          if (I mod 2) = 0 then
            EraseSection(CSECTION+IntToStr(I))
          else
            for J := 0 to iif(chkIdents.Checked, M-1, I) do
              if (J mod (M div 5)) = 0 then
                DeleteKey(CSECTION+IntToStr(I), CIDENT+IntToStr(J));
        for I := 0 to N-1 do
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      for I := 0 to N-1 do
        if (I mod 2) = 0 then
          EraseSection(CSECTION+IntToStr(I))
        else
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            if (J mod (M div 5)) = 0 then
              DeleteKey(CSECTION+IntToStr(I), CIDENT+IntToStr(J));
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      for I := 0 to N-1 do
        if (I mod 2) = 0 then
          EraseSection(CSECTION+IntToStr(I))
        else
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            if (J mod (M div 5)) = 0 then
              DeleteKey(CSECTION+IntToStr(I), CIDENT+IntToStr(J));
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      for I := 0 to N-1 do
        if (I mod 2) = 0 then
          EraseSection(CSECTION+IntToStr(I))
        else
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            if (J mod (M div 5)) = 0 then
              DeleteKey(CSECTION+IntToStr(I), CIDENT+IntToStr(J));
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnCreateEraseRewriteClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  // ReadEverything can't be used after this since that function
  // expects the sections and idents to be in order.
  btnWrite.Enabled := False;
  btnCreateWriteFree.Enabled := False;
  btnReadEverything.Enabled := False;

  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('(Re)write '+IntToStr(N)+' sections each with '+IntToStr(M)+
                       ' ident=value''s then erase and rewrite all sections');

  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        for I := 0 to N-1 do
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        for I := N-1 downto 0 do
        begin
          EraseSection(CSECTION+IntToStr(I));
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        end;
        for I := 0 to N-1 do
        begin
          EraseSection(CSECTION+IntToStr(I));
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        end;
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      for I := N-1 downto 0 do
      begin
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      for I := 0 to N-1 do
      begin
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      for I := N-1 downto 0 do
      begin
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      for I := 0 to N-1 do
      begin
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      for I := 0 to N-1 do
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      for I := N-1 downto 0 do
      begin
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      for I := 0 to N-1 do
      begin
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      end;
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  btnDelete.Enabled := True;
  btnRandomRead.Enabled := True;
  btnRead.Enabled := True;
  btnReadEverything.Enabled := True;
  btnWriteOne.Enabled := True;
  btnEraseHalf.Enabled := True;
  btnCreateEraseRewrite.Enabled := True;
  btnEraseLastSection.Enabled := True;
  btnCreateReadFree.Enabled := True;
  btnStatistics.Enabled := True;

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnEraseLastSectionClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Erase and rewrite last section');

  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
      try
        I := N-1;
        EraseSection(CSECTION+IntToStr(I));
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
    try
      I := N-1;
      EraseSection(CSECTION+IntToStr(I));
      for J := 0 to iif(chkIdents.Checked, M-1, I) do
        WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
    try
      I := N-1;
      EraseSection(CSECTION+IntToStr(I));
      for J := 0 to iif(chkIdents.Checked, M-1, I) do
        WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      InlineCommentsEnabled := False;
      UseAnsiCompare := False;
      I := N-1;
      EraseSection(CSECTION+IntToStr(I));
      for J := 0 to iif(chkIdents.Checked, M-1, I) do
        WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
      Mem := GetAddressSpaceUsed;
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnCreateWriteFreeClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add(IntToStr(N)+' times: Create object, write a section with' +
                       ' '+IntToStr(M)+ ' ident=value''s and then Free it again');

  Mem := 0;
  OutputDebugString('SAMPLING ON');

  FAbort := False;
  FLast  := -1;
  btnAbort.Visible := (N * M) > 10000;
  pgbResults.Visible := btnAbort.Visible;

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    for I := 0 to N-1 do
      with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
        try
          UpdateProgressBar(I);
          if FAbort then Exit;
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
          Mem := GetAddressSpaceUsed;
          UpdateFile;
        finally
          Free;
        end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  for I := 0 to N-1 do
    with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
      try
        UpdateProgressBar(I);
        if FAbort then Exit;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  for I := 0 to N-1 do
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
      try
        UpdateProgressBar(I);
        if FAbort then Exit;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  for I := 0 to N-1 do
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
      try
        InlineCommentsEnabled := False;
        UseAnsiCompare := False;
        UpdateProgressBar(I);
        if FAbort then Exit;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          WriteInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), M*I+J);
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  btnAbort.Visible := False;
  pgbResults.Visible := False;

  btnDelete.Enabled := True;
  btnRandomRead.Enabled := True;
  btnRead.Enabled := True;
  btnReadEverything.Enabled := True;
  btnWriteOne.Enabled := True;
  btnEraseHalf.Enabled := True;
  btnCreateEraseRewrite.Enabled := True;
  btnEraseLastSection.Enabled := True;
  btnCreateReadFree.Enabled := True;

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnCreateReadFreeClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add(IntToStr(N)+' times: Create object, read a section with' +
                       ' '+IntToStr(M)+ ' ident=value''s and then Free it again');

  Mem := 0;
  OutputDebugString('SAMPLING ON');

{$IFDEF INIFILE}
  if N*M <= 4000 then
  begin
    QueryPerformanceCounter(Cnt1);
    for I := 0 to N-1 do
      with TIniFile.Create(ApplicationPath + 'SpeedTest1.ini') do
        try
          for J := 0 to iif(chkIdents.Checked, M-1, I) do
            if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
              mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
          Mem := GetAddressSpaceUsed;
        finally
          Free;
        end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TIniFile'#9#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  end;
{$ENDIF}

{$IFDEF MEMINIFILE}
  QueryPerformanceCounter(Cnt1);
  for I := 0 to N-1 do
    with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
      try
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
            mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE_ANSI}
  QueryPerformanceCounter(Cnt1);
  for I := 0 to N-1 do
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
      try
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
            mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

{$IFDEF FASTINIFILE}
  QueryPerformanceCounter(Cnt1);
  for I := 0 to N-1 do
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
      try
        InlineCommentsEnabled := False;
        UseAnsiCompare := False;
        for J := 0 to iif(chkIdents.Checked, M-1, I) do
          if ReadInteger(CSECTION+IntToStr(I), CIDENT+IntToStr(J), 0) <> (M*I+J) then
            mmoResults.Lines.Add('Read failed - one item did not return the expected value.');
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
{$ENDIF}

  OutputDebugString('SAMPLING OFF');
end;

procedure TfrmDemo.btnWriteUsingTStringListClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
  SL: TStringList;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('(Re)write '+IntToStr(N)+' sections each with '+IntToStr(M)+
                       ' ident=value''s');

  SL := TStringList.Create;
  try
    for I := 0 to N-1 do
    begin
      SL.Add('['+CSECTION+IntToStr(I)+']');
      for J := 0 to iif(chkIdents.Checked, M-1, I) do
        SL.Add(CIDENT+IntToStr(J) + '=' + IntToStr(M*I+J));
      SL.Add('');
    end;

    QueryPerformanceCounter(Cnt1);
    with TStringList.Create do
      try
        Assign(SL);
        SaveToFile(ApplicationPath + 'SpeedTest1.ini');
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TStringList'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  finally
    SL.Free;
  end;

  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest2.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest3.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest4.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest5.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest6.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest7.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest8.ini'), False);
  CopyFile(PChar(ApplicationPath + 'SpeedTest1.ini'), PChar(ApplicationPath + 'SpeedTest9.ini'), False);

  btnDelete.Enabled := True;
  btnRandomRead.Enabled := True;
  btnRead.Enabled := True;
  btnReadEverything.Enabled := True;
  btnWriteOne.Enabled := True;
  btnEraseHalf.Enabled := True;
  btnCreateEraseRewrite.Enabled := True;
  btnEraseLastSection.Enabled := True;
  btnCreateReadFree.Enabled := True;
  btnStatistics.Enabled := True;
end;

procedure TfrmDemo.btnWriteUsingSetStringsClick(Sender: TObject);
var
  I, J: Integer;
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
  SL: TStringList;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('(Re)write '+IntToStr(N)+' sections each with '+IntToStr(M)+
                       ' ident=value''s');

  SL := TStringList.Create;
  try
    for I := 0 to N-1 do
    begin
      SL.Add('['+CSECTION+IntToStr(I)+']');
      for J := 0 to iif(chkIdents.Checked, M-1, I) do
        SL.Add(CIDENT+IntToStr(J) + '=' + IntToStr(M*I+J));
      SL.Add('');
    end;

  {$IFDEF MEMINIFILE}
    QueryPerformanceCounter(Cnt1);
    with TMemIniFile.Create(ApplicationPath + 'SpeedTest2.ini') do
      try
        SetStrings(SL);
        Mem := GetAddressSpaceUsed;
        UpdateFile;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TMemIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  {$ENDIF}

  {$IFDEF FASTINIFILE_ANSI}
    QueryPerformanceCounter(Cnt1);
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest3.ini') do
      try
        SetStrings(SL);
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  {$ENDIF}

  {$IFDEF FASTINIFILE}
    QueryPerformanceCounter(Cnt1);
    with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
      try
        InlineCommentsEnabled := False;
        UseAnsiCompare := False;
        SetStrings(SL);
        Mem := GetAddressSpaceUsed;
      finally
        Free;
      end;
    QueryPerformanceCounter(Cnt2);
    mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
  {$ENDIF}

  finally
    SL.Free;
  end;

  btnDelete.Enabled := True;
  btnRandomRead.Enabled := True;
  btnRead.Enabled := True;
  btnReadEverything.Enabled := True;
  btnWriteOne.Enabled := True;
  btnEraseHalf.Enabled := True;
  btnCreateEraseRewrite.Enabled := True;
  btnEraseLastSection.Enabled := True;
  btnCreateReadFree.Enabled := True;
  btnStatistics.Enabled := True;
end;

procedure TfrmDemo.btnStatisticsClick(Sender: TObject);
var
  Cnt1, Cnt2: Int64;
  Mem: Cardinal;
begin
  mmoResults.Lines.Add('');
  mmoResults.Lines.Add('Statistics of SpeedTest4.ini');

  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'SpeedTest4.ini') do
    try
      mmoResults.Lines.Add('Statistics.BlankLines = '#9 + IntToStr(Statistics.BlankLines));
      mmoResults.Lines.Add('Statistics.Comments = '#9   + IntToStr(Statistics.Comments));
      mmoResults.Lines.Add('Statistics.Idents = '#9#9   + IntToStr(Statistics.Idents));
      mmoResults.Lines.Add('Statistics.Sections = '#9   + IntToStr(Statistics.Sections));
      Mem := GetAddressSpaceUsed;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  mmoResults.Lines.Add('TFastIniFile'#9 + Format('%.3f s'#9' %d Kb', [(Cnt2-Cnt1) / FFreq, Mem]));
end;

procedure TfrmDemo.btnDeleteClick(Sender: TObject);
begin
  DeleteFile(ApplicationPath + 'SpeedTest0.ini');
  DeleteFile(ApplicationPath + 'SpeedTest1.ini');
  DeleteFile(ApplicationPath + 'SpeedTest2.ini');
  DeleteFile(ApplicationPath + 'SpeedTest3.ini');
  DeleteFile(ApplicationPath + 'SpeedTest4.ini');
  DeleteFile(ApplicationPath + 'SpeedTest5.ini');
  DeleteFile(ApplicationPath + 'SpeedTest6.ini');
  DeleteFile(ApplicationPath + 'SpeedTest7.ini');
  DeleteFile(ApplicationPath + 'SpeedTest8.ini');
  DeleteFile(ApplicationPath + 'SpeedTest9.ini');
  btnDelete.Font.Color := clGreen;
  btnDelete.Font.Style := [];
  btnDelete.Enabled := False;
  btnWrite.Enabled := True;
  btnRandomRead.Enabled := False;
  btnRead.Enabled := False;
  btnReadEverything.Enabled := False;
  btnWriteOne.Enabled := False;
  btnEraseHalf.Enabled := False;
  btnCreateEraseRewrite.Enabled := True;
  btnEraseLastSection.Enabled := False;
  btnCreateWriteFree.Enabled := True;
  btnCreateReadFree.Enabled := False;
  btnStatistics.Enabled := False;
end;


// *****************************************************************************
// * Page: EXTRA                                                               *
// *****************************************************************************

procedure TfrmDemo.btnReadValueAndCommentClick(Sender: TObject);
begin
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      edtValue.Text := ReadString('Section', 'Ident', '');
      edtComment.Text := ReadInlineComment('Section', 'Ident', '');
    finally
      Free;
    end;
end;

procedure TfrmDemo.btnReadQuotedValuesClick(Sender: TObject);
const
  SectionStr = 'Quoted Values';
begin
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      edtQuotedValues.Text := ReadString(SectionStr, 'QV1', '') +
                              ReadString(SectionStr, 'QV2', '') +
                              ReadString(SectionStr, 'QV3', '');
    finally
      Free;
    end;
end;

procedure TfrmDemo.btnReadWriteFixedDateTimeClick(Sender: TObject);
const
  SectionStr = 'Fixed DateTime formatting';
begin
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      // Delete entire section including the leading comment lines.
      EraseSection(SectionStr, True);

      WriteFixedDate(SectionStr, 'Date', Now);
      WriteFixedTime(SectionStr, 'Time', Now);
      WriteFixedDateTime(SectionStr, 'DateTime', Now);

      // This shows how to insert comment afterwards.
      InsertComment(SectionStr, '',
        'I don''t like the system dependecies of the original functions ',
        ipBeforeSection, []);
      InsertComment(SectionStr, '',
        'TFastIniFile has functions which uses a fixed lay-out.',
        ipBeforeSection, []);

      edtDateTime.Text :=
        FormatDateTime('dddddd tt',
          ReadFixedDateTime('Fixed DateTime formatting', 'DateTime', 0));
    finally
      Free;
    end;
  Reload;
end;

procedure TfrmDemo.btnWriteStringsClick(Sender: TObject);
const
  SectionStr = 'Recent files';
begin
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      // This shows how to insert comment before a section even exists.
      InsertComment(SectionStr, '',
        'Usage: WriteStrings(''Recent files'', ''File'', lbxRecent1.Items);',
        ipBeforeSection, [imCanCreateKey, imOverwriteExisting]);

      WriteStrings(SectionStr, 'File', lbxRecent1.Items);
    finally
      Free;
    end;
  btnReadStrings.Enabled := True;
  Reload;
end;

procedure TfrmDemo.btnReadStringsClick(Sender: TObject);
begin
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      ReadStrings('Recent files', 'File', lbxRecent2.Items);
    finally
      Free;
    end;
end;

procedure TfrmDemo.btnWriteFontClick(Sender: TObject);
var
  F: TFont;
begin
  F := TFont.Create;
  try
    F.Name := 'Arial';
    F.Color := clBlue;
    F.Size := 8;
    F.Style := [fsBold, fsItalic];
    with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
      try
        WriteFont(Name, 'Font', F);
      finally
        Free;
      end;
  finally
    F.Free;
  end;
  btnReadFont.Enabled := True;
  Reload;
end;

procedure TfrmDemo.btnReadFontClick(Sender: TObject);
begin
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      Font := ReadFont(Name, 'Font', Font);
    finally
      Free;
    end;
  btnRestoreFont.Enabled := True;
end;

procedure TfrmDemo.btnRestoreFontClick(Sender: TObject);
begin
  Font.Name := 'MS Sans Serif';
  Font.Color := clWindowText;
  Font.Style := [];
  with TFastIniFile.Create(ApplicationPath + 'demo.ini') do
    try
      WriteFont(Name, 'Font', Font);
    finally
      Free;
    end;
  Reload;
end;


// *****************************************************************************
// * Page: COMPATIBILITY                                                       *
// *****************************************************************************

procedure TfrmDemo.btnCheckEmptyReadDefaultClick(Sender: TObject);
var
  Cnt1, Cnt2: Int64;
begin
  Memo1.Clear;
  Memo1.Lines.Add('Check reading back Idents with no value assigned to them [Section0]');

  Memo1.Lines.Add('');
  CopyFile(PChar(ApplicationPath + 'TestComp.ini'), PChar(ApplicationPath + 'TestComp1.ini'), False);
  QueryPerformanceCounter(Cnt1);
  with TIniFile.Create(ApplicationPath + 'TestComp1.ini') do
    try
      Memo1.Lines.Add('ReadString(''Section0'', ''IdentWith'', ''Default'') returns:'#9+
                      ReadString('Section0', 'IdentWith',    'Default (IdentWith)'));
      Memo1.Lines.Add('ReadString(''Section0'', ''IdentWithOut'', ''Default'') returns:'#9+
                      ReadString('Section0', 'IdentWithOut', 'Default'));
      WriteString('Section0', 'IdentWithOut', 'New');
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TIniFile'#9#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  CopyFile(PChar(ApplicationPath + 'TestComp.ini'), PChar(ApplicationPath + 'TestComp2.ini'), False);
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'TestComp2.ini') do
    try
      Memo1.Lines.Add('ReadString(''Section0'', ''IdentWith'', ''Default'') returns:'#9+
                      ReadString('Section0', 'IdentWith',    'Default (IdentWith)'));
      Memo1.Lines.Add('ReadString(''Section0'', ''IdentWithOut'', ''Default'') returns:'#9+
                      ReadString('Section0', 'IdentWithOut', 'Default'));
      WriteString('Section0', 'IdentWithOut', 'New');
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TMemIniFile'#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  CopyFile(PChar(ApplicationPath + 'TestComp.ini'), PChar(ApplicationPath + 'TestComp3.ini'), False);
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'TestComp3.ini') do
    try
      UseAnsiCompare := True;
      Memo1.Lines.Add('ReadString(''Section0'', ''IdentWith'', ''Default'') returns:'#9+
                      ReadString('Section0', 'IdentWith',    'Default (IdentWith)'));
      Memo1.Lines.Add('ReadString(''Section0'', ''IdentWithOut'', ''Default'') returns:'#9+
                      ReadString('Section0', 'IdentWithOut', 'Default'));
      WriteString('Section0', 'IdentWithOut', 'New');
      UpdateFile;
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  btnReadSection.Enabled := True;
  btnReadSectionValues.Enabled := True;
end;

procedure TfrmDemo.btnReadSectionClick(Sender: TObject);
begin
  Memo1.Clear;
  Memo1.Lines.Add('In the RichEdits above you can see the result of ReadSection()');
  Memo1.Lines.Add('after a WriteString(''Section0'', ''IdentWithOut'', ''New'');');

  with TIniFile.Create(ApplicationPath + 'TestComp1.ini') do
    try
      rchComp1.Lines.Clear;
      ReadSection('Section0', rchComp1.Lines);
    finally
      Free;
    end;
  with TMemIniFile.Create(ApplicationPath + 'TestComp2.ini') do
    try
      rchComp2.Lines.Clear;
      ReadSection('Section0', rchComp2.Lines);
    finally
      Free;
    end;
  with TFastIniFile.Create(ApplicationPath + 'TestComp3.ini') do
    try
      rchComp3.Lines.Clear;
      ReadSection('Section0', rchComp3.Lines);
    finally
      Free;
    end;
  Memo1.Lines.Add('');
  Memo1.Lines.Add('NOTE: Note the "weird" empty line which TMemInifile returns.');
end;

procedure TfrmDemo.btnReadSectionValuesClick(Sender: TObject);
begin
  Memo1.Lines.Clear;
  Memo1.Lines.Add('In the RichEdits above you can see the result of ReadSectionValues()');
  Memo1.Lines.Add('after a WriteString(''Section0'', ''IdentWithOut'', ''New'');');

  with TIniFile.Create(ApplicationPath + 'TestComp1.ini') do
    try
      rchComp1.Lines.Clear;
      ReadSectionValues('Section0', rchComp1.Lines);
    finally
      Free;
    end;
  with TMemIniFile.Create(ApplicationPath + 'TestComp2.ini') do
    try
      rchComp2.Lines.Clear;
      ReadSectionValues('Section0', rchComp2.Lines);
    finally
      Free;
    end;
  with TFastIniFile.Create(ApplicationPath + 'TestComp3.ini') do
    try
      rchComp3.Lines.Clear;
      ReadSectionValues('Section0', rchComp3.Lines);
    finally
      Free;
    end;
  Memo1.Lines.Add('');
  Memo1.Lines.Add('NOTE: in D5 you will see an error in what is returned by '+
                  'TIniFile.ReadSectionsValues, it is missing "IdentWith"');
end;

procedure TfrmDemo.btnTestReadBackSpacesClick(Sender: TObject);
var
  Cnt1, Cnt2: Int64;
begin
  Memo1.Clear;
  Memo1.Lines.Add('Test reading back a Section with Idents containing spaces  [Section1].');
  Memo1.Lines.Add('First try to read an Indent without and then with spaces.');

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      Memo1.Lines.Add(ReadString('Section1', 'IdentWithoutSpace', 'TIniFile failed (1)'));
      Memo1.Lines.Add(ReadString('Section1', 'Ident with space',  'TIniFile failed (2)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TIniFile'#9#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      Memo1.Lines.Add(ReadString('Section1', 'IdentWithoutSpace', 'TMemIniFile failed (1)'));
      Memo1.Lines.Add(ReadString('Section1', 'Ident with space',  'TMemIniFile failed (2)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TMemIniFile'#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      UseAnsiCompare := True;
      Memo1.Lines.Add(ReadString('Section1', 'IdentWithoutSpace', 'FastIniFile failed (1)'));
      Memo1.Lines.Add(ReadString('Section1', 'Ident with space',  'FastIniFile failed (2)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  Memo1.Lines.Add('NOTE: D5 TMemIniFile will fail on the "Ident with space", not '+
                  'because of the spaces in the middle but because of the space at the end.')
end;

procedure TfrmDemo.btnTestIfAnsiCompareIsNeededClick(Sender: TObject);
var
  Cnt1, Cnt2: Int64;
begin
  Memo1.Clear;
  Memo1.Lines.Add('Testing reading back an Ident with a Ü vs ü  [Section2]');

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      Memo1.Lines.Add('ReadString(.., ''überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'überhaupt', 'TIniFile failed (1)'));
      Memo1.Lines.Add('ReadString(.., ''Überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'Überhaupt', 'TIniFile failed (1)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TIniFile'#9#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TMemIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      Memo1.Lines.Add('ReadString(.., ''überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'überhaupt', 'TMemIniFile failed (1)'));
      Memo1.Lines.Add('ReadString(.., ''Überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'Überhaupt', 'TMemIniFile failed (2)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TMemIniFile'#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      UseAnsiCompare := True;
      Memo1.Lines.Add('ReadString(.., ''überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'überhaupt', 'TFastIniFile failed (1)'));
      Memo1.Lines.Add('ReadString(.., ''Überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'Überhaupt', 'TFastIniFile failed (2)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TFastIniFile ANSI'#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]));

  Memo1.Lines.Add('');
  QueryPerformanceCounter(Cnt1);
  with TFastIniFile.Create(ApplicationPath + 'TestComp.ini') do
    try
      UseAnsiCompare := False;
      Memo1.Lines.Add('ReadString(.., ''überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'überhaupt', 'TFastIniFile failed (1)'));
      Memo1.Lines.Add('ReadString(.., ''Überhaupt'', ..) returns:'#9 +
                      ReadString('Section2', 'Überhaupt', 'TFastIniFile failed (2)'));
    finally
      Free;
    end;
  QueryPerformanceCounter(Cnt2);
  Memo1.Lines.Add('TFastIniFile '#9 + Format('%.3f s', [(Cnt2-Cnt1) / FFreq]) +
                  ' With this latest test property UseAnsiCompare was False.');
end;

procedure TfrmDemo.btnAbortClick(Sender: TObject);
begin
  FAbort := True;
end;

initialization
  OutputDebugString('SAMPLING OFF');
  ApplicationPath := ExtractFilePath(Application.ExeName);
end.
