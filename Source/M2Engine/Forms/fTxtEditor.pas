unit fTxtEditor;

interface

{$WARN SYMBOL_DEPRECATED OFF}
{$WARN UNIT_PLATFORM OFF}

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ExtCtrls, ComCtrls, StdCtrls, Menus,
  StrUtils, FileCtrl, RzShellCtrls, ActnList, RzFilSys, RzChkLst, SynEdit, SynEditTypes, SynEditRegexSearch, SynEditSearch,
  uSynHighlighterSample, M2Share, ObjNpc, RzLabel, System.Actions, System.RegularExpressions, SynEditMiscClasses,
  plgSearchHighlighter, System.ImageList, Vcl.ImgList, RzLstBox, Vcl.ToolWin;

type
  THistory = class
    FileName: string;
    FilePath: string;
  end;

  TfrmTXTEditor = class(TForm)
    pnlLeft: TPanel;
    pnlCommon: TPanel;
    pnlResource: TPanel;
    stat: TStatusBar;
    dlgOpen: TOpenDialog;
    pmEditor: TPopupMenu;
    N2: TMenuItem;
    N3: TMenuItem;
    Np_ReadOnly: TMenuItem;
    N4: TMenuItem;
    N7: TMenuItem;
    N8: TMenuItem;
    N9: TMenuItem;
    N22: TMenuItem;
    imglMain: TImageList;
    tlbMain: TToolBar;
    btnSave: TToolButton;
    btn1: TToolButton;
    btnSearch: TToolButton;
    btnSearchNext: TToolButton;
    btnSearchLast: TToolButton;
    btnSearchReplace: TToolButton;
    btn3: TToolButton;
    btnUndo: TToolButton;
    btnRedo: TToolButton;
    lstCurrDirFiles: TRzFileListBox;
    lstDir: TRzDirectoryListBox;
    actlstMain: TActionList;
    actSave: TAction;
    actSearch: TAction;
    actSearchNext: TAction;
    actSearchPrev: TAction;
    actSearchReplace: TAction;
    actUndo: TAction;
    actRedo: TAction;
    actCopy: TAction;
    actCut: TAction;
    actPaste: TAction;
    actSelectAll: TAction;
    actReadOlny: TAction;
    actSaveAs: TAction;
    pnl: TPanel;
    btnReload: TButton;
    lstNPC: TRzCheckList;
    spl: TSplitter;
    actReload: TAction;
    pnlHistory: TPanel;
    lstHistory: TListBox;
    lblClear: TRzURLLabel;
    pgcTool: TPageControl;
    tsNPCs: TTabSheet;
    tsNavigation: TTabSheet;
    tvLocate: TTreeView;
    btnFormat: TToolButton;
    btn2: TToolButton;
    actFormat: TAction;
    N1: TMenuItem;
    actNote: TAction;
    actUnnote: TAction;
    N5: TMenuItem;
    N6: TMenuItem;
    pnlEditor: TPanel;
    syndtScriptEditor: TSynEdit;
    SynEditRegexSearch: TSynEditRegexSearch;
    SynEditSearch: TSynEditSearch;
    pmNpcList: TPopupMenu;
    actGetMoveCmd: TAction;
    NPC1: TMenuItem;
    actExternalOpen: TAction;
    actExternalOpen1: TMenuItem;
    pnlSearch: TPanel;
    edtSearch: TEdit;
    lbl1: TLabel;
    btnSearchTool: TButton;
    btnSearchLoop: TButton;
    actSearchTool: TAction;
    actSearchLoop: TAction;
    actGoto: TAction;
    N10: TMenuItem;
    stat1: TStatusBar;
    pnlEditor1: TPanel;
    btnHelp: TToolButton;
    actHelp: TAction;
    procedure lstCurrDirFilesDblClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure actSaveExecute(Sender: TObject);
    procedure actSearchNextUpdate(Sender: TObject);
    procedure actSearchPrevUpdate(Sender: TObject);
    procedure actSearchReplaceUpdate(Sender: TObject);
    procedure actFormatterUpdate(Sender: TObject);
    procedure actUndoUpdate(Sender: TObject);
    procedure actRedoUpdate(Sender: TObject);
    procedure actRedoExecute(Sender: TObject);
    procedure actUndoExecute(Sender: TObject);
    procedure actCopyExecute(Sender: TObject);
    procedure actCutExecute(Sender: TObject);
    procedure actPasteExecute(Sender: TObject);
    procedure actSelectAllExecute(Sender: TObject);
    procedure actCutUpdate(Sender: TObject);
    procedure actPasteUpdate(Sender: TObject);
    procedure actReadOlnyExecute(Sender: TObject);
    procedure actSaveUpdate(Sender: TObject);
    procedure actReloadExecute(Sender: TObject);
    procedure pnlResize(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure lstNPCDblClick(Sender: TObject);
    procedure lblClearClick(Sender: TObject);
    procedure lstHistoryDblClick(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure DoSectionTargeting(Sender: TObject);
    procedure actFormatExecute(Sender: TObject);
    procedure actCopyUpdate(Sender: TObject);
    procedure actNoteExecute(Sender: TObject);
    procedure actSearchExecute(Sender: TObject);
    procedure actSearchNextExecute(Sender: TObject);
    procedure actSearchPrevExecute(Sender: TObject);
    procedure actSearchReplaceExecute(Sender: TObject);
    procedure actSearchUpdate(Sender: TObject);
    procedure actFormatUpdate(Sender: TObject);
    procedure syndtScriptEditorReplaceText(Sender: TObject; const ASearch, AReplace: string; Line, Column: Integer;
      var Action: TSynReplaceAction);
    procedure syndtScriptEditorGutterClick(Sender: TObject; Button: TMouseButton; X, Y, Line: Integer; Mark: TSynEditMark);
    procedure actUnnoteExecute(Sender: TObject);
    procedure actGetMoveCmdExecute(Sender: TObject);
    procedure actExternalOpenExecute(Sender: TObject);
    procedure pnlSearchResize(Sender: TObject);
    procedure pgcToolChange(Sender: TObject);
    procedure actSearchToolExecute(Sender: TObject);
    procedure actSearchLoopExecute(Sender: TObject);
    procedure actGotoExecute(Sender: TObject);
    procedure lstNPCChanging(Sender: TObject; Index: Integer; NewState: TCheckBoxState; var AllowChange: Boolean);
    procedure actHelpExecute(Sender: TObject);
  private
    FCurrFile: string;
    FSearchFromCaret: Boolean;
    FSyndtsrch: TSynEditSearch;
    FSyndtrgxsrch: TSynEditRegexSearch;
    FSHP: TSearchTextHightlighterSynEditPlugin;
    FCurrNPCIndex: Integer; // 用于记录祖先脚本，防止CALL出去后，修改CALL脚本不能找到祖先脚本的问题。
    FIsCallMobified: Boolean;

    procedure DoSearchReplaceText(AReplace: Boolean; ABackwards: Boolean);
    procedure ShowSearchReplaceDialog(AReplace: Boolean);
    function LoadScriptTxt(AFilePath: string; AIsCall: Boolean = false): Boolean;
    function GetMerchantScriptFile(AMerchant: TMerchant): string;
    procedure LoadHistory;
    procedure AddHistory(AFilePath: string);
    procedure SectionTargeting(const ASearchText: string);
    procedure SearchInit;
    procedure RefreshScript;
    procedure SearchToolHandler(const ASearchText: string; ASearchIndex: Cardinal = 0);
  public
    { Public declarations }
  end;

var
  gbSearchBackwards: Boolean;
  gbSearchCaseSensitive: Boolean;
  gbSearchFromCaret: Boolean;
  gbSearchSelectionOnly: Boolean;
  gbSearchTextAtCaret: Boolean = true; // 打开这个开关可以直接将选中内容作为搜索内容。
  gbSearchWholeWords: Boolean;
  gbSearchRegex: Boolean;
  gsSearchText: string;
  gsSearchTextHistory: string;
  gsReplaceText: string;
  gsReplaceTextHistory: string;

resourcestring
  SInsert = 'Insert';
  SOverwrite = 'Overwrite';
  SReadOnly = 'Read Only';
  SNonameFileTitle = 'Untitled';
  SEditorCaption = 'Editor';
  STextNotFound = '没有发现目标字符串';

{$J+}

const
  SEARCH_INDEX: Integer = -1;
{$J-}

implementation

{$R *.dfm}

uses
  Vcl.Clipbrd, ShellApi, svMain, dlgSearchText, dlgReplaceText, dlgConfirmReplace;

procedure TfrmTXTEditor.FormClose(Sender: TObject; var Action: TCloseAction);
var
  i: Integer;
  tmpStrs: TStrings;
begin
  tmpStrs := TStringList.Create;
  for i := 0 to lstHistory.Count - 1 do
    tmpStrs.Add(THistory(lstHistory.Items.Objects[i]).FilePath);
  tmpStrs.SaveToFile(ExtractFilePath(ParamStr(0)) + 'EditorHisory.txt');
  tmpStrs.Free;
end;

procedure TfrmTXTEditor.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  if syndtScriptEditor.Modified and FileExists(FCurrFile) then
  begin
    case Application.MessageBox('当前编辑中的文本已被修改，是否保存？', '提示', MB_YESNOCANCEL + MB_ICONQUESTION) of
      IDYES:
        begin
          actSaveExecute(nil);
          syndtScriptEditor.Modified := false;
        end;
      IDNO:
        syndtScriptEditor.Modified := false;
      IDCANCEL:
        CanClose := false;
    end;
  end;
end;

procedure TfrmTXTEditor.FormCreate(Sender: TObject);
var
  i: Integer;
  tmpDir: string;
  tmpMerchant: TMerchant;
begin
  FCurrNPCIndex := -1;

  FSHP := TSearchTextHightlighterSynEditPlugin.Create(syndtScriptEditor);
  FSHP.Attribute.Background := $00B3CFFF;

  tmpDir := ExtractFilePath(ParamStr(0)) + 'Envir';
  if DirectoryExists(tmpDir) then
    lstDir.Directory := tmpDir;

  syndtScriptEditor.Gutter.ShowLineNumbers := true;
  syndtScriptEditor.Highlighter := TSynSampleSyn.Create(syndtScriptEditor);
  FSyndtsrch := TSynEditSearch.Create(nil);
  FSyndtrgxsrch := TSynEditRegexSearch.Create(nil);

  lstNPC.Items.BeginUpdate;
  lstNPC.Items.AddObject('功能脚本(QF)', g_FunctionNPC);
  lstNPC.Items.AddObject('登录脚本(QM)', g_ManageNPC);
  lstNPC.Items.AddObject('机器人脚本(RM)', g_RobotNPC);
  for i := 0 to UserEngine.m_MerchantList.Count - 1 do
  begin
    tmpMerchant := TMerchant(UserEngine.m_MerchantList.Items[i]);
    if (tmpMerchant.m_sMapName = '0') and (tmpMerchant.m_nCurrX = 0) and (tmpMerchant.m_nCurrY = 0) then
      Continue;

    lstNPC.Items.AddObject(Format('%s-%s(%d:%d)', [tmpMerchant.m_sScript, tmpMerchant.m_sMapName, tmpMerchant.m_nCurrX,
      tmpMerchant.m_nCurrY]), tmpMerchant);
  end;
  lstNPC.Items.EndUpdate;

  LoadHistory;
end;

procedure TfrmTXTEditor.lstCurrDirFilesDblClick(Sender: TObject);
begin
  LoadScriptTxt(lstCurrDirFiles.LongFileName);
end;

procedure TfrmTXTEditor.lstHistoryDblClick(Sender: TObject);
var
  i: Integer;
  tmpPath, tmpFile: string;
  tmpMerchant: TMerchant;
begin
  if lstHistory.ItemIndex >= 0 then
  begin
    for i := 0 to lstNPC.Count - 1 do
    begin
      tmpMerchant := TMerchant(lstNPC.Items.Objects[i]);
      tmpPath := GetMerchantScriptFile(tmpMerchant);
      if (tmpMerchant = g_ManageNPC) or (tmpMerchant = g_RobotNPC) then
        tmpFile := Format('%s%s.txt', [tmpPath, tmpMerchant.m_sScript])
      else
        tmpFile := Format('%s%s-%s.txt', [tmpPath, tmpMerchant.m_sScript, tmpMerchant.m_sMapName]);

      if SameText(tmpFile, THistory(lstHistory.Items.Objects[lstHistory.ItemIndex]).FilePath) then
      begin
        lstNPC.Selected[i] := true;
        FCurrNPCIndex := i;
        Break;
      end;
    end;

    LoadScriptTxt(THistory(lstHistory.Items.Objects[lstHistory.ItemIndex]).FilePath);
  end;
end;

procedure TfrmTXTEditor.lstNPCChanging(Sender: TObject; Index: Integer; NewState: TCheckBoxState; var AllowChange: Boolean);
begin
  if lstNPC.ItemIndex < 0 then
    exit;

  AllowChange := not lstNPC.ItemChecked[lstNPC.ItemIndex] //
    or (Application.MessageBox('是否取消对本脚本的重载？', '提示', MB_YESNO + MB_ICONQUESTION) = IDYES);
end;

procedure TfrmTXTEditor.lstNPCDblClick(Sender: TObject);
var
  tmpPath, tmpFile: string;
  tmpMerchant: TMerchant;
begin
  if lstNPC.ItemIndex < 0 then
    exit;

  tmpMerchant := TMerchant(lstNPC.Items.Objects[lstNPC.ItemIndex]);
  tmpPath := GetMerchantScriptFile(tmpMerchant);
  if (tmpMerchant = g_ManageNPC) or (tmpMerchant = g_RobotNPC) then
    tmpFile := Format('%s%s.txt', [tmpPath, tmpMerchant.m_sScript])
  else
    tmpFile := Format('%s%s-%s.txt', [tmpPath, tmpMerchant.m_sScript, tmpMerchant.m_sMapName]);
  LoadScriptTxt(tmpFile);

  FCurrNPCIndex := lstNPC.ItemIndex;
end;

function TfrmTXTEditor.GetMerchantScriptFile(AMerchant: TMerchant): string;
begin
  if SameText(AMerchant.m_sCharName, 'QManage') then
    Result := g_Config.sEnvirDir + 'MapQuest_def\'
  else if SameText(AMerchant.m_sCharName, 'RobotManage') then
    Result := g_Config.sEnvirDir + 'Robot_def\'
  else
    Result := g_Config.sEnvirDir + 'Market_Def\';
end;

procedure TfrmTXTEditor.lblClearClick(Sender: TObject);
var
  i: Integer;
begin
  for i := lstHistory.Count - 1 downto 0 do
  begin
    lstHistory.Items.Objects[i].Free;
    lstHistory.Items.Delete(i);
  end;
end;

procedure TfrmTXTEditor.LoadHistory;
var
  i: Integer;
  tmpFile: string;
  tmpStrs: TStrings;
begin
  tmpFile := ExtractFilePath(ParamStr(0)) + 'EditorHisory.txt';
  if FileExists(tmpFile) then
  begin
    tmpStrs := TStringList.Create;
    tmpStrs.LoadFromFile(tmpFile);

    for i := tmpStrs.Count - 1 downto 0 do
      AddHistory(tmpStrs[i]);
    tmpStrs.Free;
  end;
end;

function TfrmTXTEditor.LoadScriptTxt(AFilePath: string; AIsCall: Boolean): Boolean;
begin
  Result := false;

  syndtScriptEditor.Clear;
  if not FileExists(AFilePath) then
  begin
    stat1.Panels[0].Text := ' 脚本路径：' + AFilePath + '[文件不存在]';
    exit;
  end;

  if syndtScriptEditor.Modified and FileExists(FCurrFile) then
  begin
    case Application.MessageBox('当前编辑中的文本已被修改，是否保存？', '提示', MB_YESNOCANCEL + MB_ICONQUESTION) of
      IDYES:
        begin
          actSaveExecute(nil);
          syndtScriptEditor.Modified := false;
        end;
      IDNO:
        syndtScriptEditor.Modified := false;
      IDCANCEL:
        exit;
    end;
  end;

  try
    syndtScriptEditor.Lines.LoadFromFile(AFilePath);
    FCurrFile := AFilePath;
    syndtScriptEditor.Modified := false;
    AddHistory(AFilePath);
    stat1.Panels[0].Text := ' 脚本路径：' + AFilePath;
    RefreshScript;

    gsSearchText := '';
    gsSearchTextHistory := '';
    gsReplaceText := '';
    gsReplaceTextHistory := '';

    Result := true;
  except
    ShowMessage('打开文件失败，文件可能被占用！');
  end;
end;

procedure TfrmTXTEditor.AddHistory(AFilePath: string);
var
  i: Integer;
  tmpFind: Boolean;
  tmpObj: THistory;
begin
  tmpObj := nil;
  if FileExists(AFilePath) then
  begin
    tmpFind := false;
    for i := 0 to lstHistory.Count - 1 do
    begin
      if SameText(FCurrFile, THistory(lstHistory.Items.Objects[i]).FilePath) then
      begin
        tmpObj := THistory(lstHistory.Items.Objects[i]);
        lstHistory.Items.Delete(i);
        tmpFind := true;
        Break;
      end;
    end;

    if not tmpFind then
    begin
      tmpObj := THistory.Create;
      tmpObj.FileName := ExtractFileName(AFilePath);
      tmpObj.FilePath := AFilePath;
    end;

    lstHistory.Items.Insert(0, tmpObj.FileName);
    lstHistory.Items.Objects[0] := tmpObj;
  end;
end;

procedure TfrmTXTEditor.pnlResize(Sender: TObject);
begin
  btnReload.Left := pnl.Width div 2 - btnReload.Width div 2;
end;

procedure TfrmTXTEditor.pnlSearchResize(Sender: TObject);
begin
  edtSearch.Width := 103 + pnlSearch.Width - 198;
end;

procedure TfrmTXTEditor.SearchInit;
begin
  gbSearchBackwards := false;
  gbSearchCaseSensitive := false;
  gbSearchFromCaret := false;
  gbSearchSelectionOnly := false;
  gbSearchTextAtCaret := true; // 打开这个开关可以直接将选中内容作为搜索内容。
  gbSearchWholeWords := false;
  gbSearchRegex := false;
  gsSearchText := '';
  gsSearchTextHistory := '';
  gsReplaceText := '';
  gsReplaceTextHistory := '';
end;

procedure TfrmTXTEditor.pgcToolChange(Sender: TObject);
begin
  SEARCH_INDEX := -1;
  lstNPC.ItemIndex := -1;
  edtSearch.Text := EmptyStr;
end;

procedure TfrmTXTEditor.actSearchToolExecute(Sender: TObject);
begin
  SEARCH_INDEX := -1;
  SearchToolHandler(edtSearch.Text);
end;

procedure TfrmTXTEditor.actSearchLoopExecute(Sender: TObject);
begin
  if SEARCH_INDEX >= 0 then
    SearchToolHandler(edtSearch.Text, SEARCH_INDEX);
end;

procedure TfrmTXTEditor.SearchToolHandler(const ASearchText: string; ASearchIndex: Cardinal);
var
  i: Integer;
  tmpFind: Boolean;
  tmpCount: Cardinal;
begin
  if Trim(ASearchText).IsEmpty then
    exit;

  tmpFind := false;
  if pgcTool.ActivePage = tsNPCs then
  begin
    tmpCount := lstNPC.Items.Count;
    for i := ASearchIndex to tmpCount - 1 do
    begin
      if AnsiContainsText(lstNPC.Items[i], ASearchText) then
      begin
        tmpFind := true;
        lstNPC.Selected[i] := true;
        Break;
      end;
    end;
  end
  else
  begin
    tmpCount := tvLocate.Items.Count;
    for i := ASearchIndex to tmpCount - 1 do
    begin
      if AnsiContainsText(tvLocate.Items[i].Text, ASearchText) then
      begin
        tmpFind := true;
        tvLocate.Items[i].Selected := true;
        tvLocate.SetFocus;
        Break;
      end;
    end;
  end;

  if i = tmpCount then
  begin
    if SEARCH_INDEX >= 0 then
      actSearchToolExecute(nil);
    exit;
  end;

  if tmpFind or (SEARCH_INDEX >= 0) then
  begin
    SEARCH_INDEX := i + 1;
    if SEARCH_INDEX >= tmpCount then
      SEARCH_INDEX := 0;
  end;
end;

procedure TfrmTXTEditor.SectionTargeting(const ASearchText: string);
begin
  SearchInit;
  gsSearchText := ASearchText;
  DoSearchReplaceText(false, false);
end;

procedure TfrmTXTEditor.DoSearchReplaceText(AReplace, ABackwards: Boolean);
var
  Options: TSynSearchOptions;
begin
  if AReplace then
    Options := [ssoPrompt, ssoReplace, ssoReplaceAll]
  else
    Options := [];
  if ABackwards then
    Include(Options, ssoBackwards);
  if gbSearchCaseSensitive then
    Include(Options, ssoMatchCase);
  if not FSearchFromCaret then
    Include(Options, ssoEntireScope);
  if gbSearchSelectionOnly then
    Include(Options, ssoSelectedOnly);
  if gbSearchWholeWords then
    Include(Options, ssoWholeWord);
  if gbSearchRegex then
    syndtScriptEditor.SearchEngine := SynEditRegexSearch
  else
    syndtScriptEditor.SearchEngine := SynEditSearch;

  if syndtScriptEditor.SearchReplace(gsSearchText, gsReplaceText, Options) = 0 then
  begin
    MessageBeep(MB_ICONASTERISK);

    if ssoBackwards in Options then
      syndtScriptEditor.BlockEnd := syndtScriptEditor.BlockBegin
    else
      syndtScriptEditor.BlockBegin := syndtScriptEditor.BlockEnd;

    syndtScriptEditor.CaretXY := syndtScriptEditor.BlockBegin;
  end;

  if ConfirmReplaceDialog <> nil then
    ConfirmReplaceDialog.Free;
end;

procedure TfrmTXTEditor.ShowSearchReplaceDialog(AReplace: Boolean);
var
  dlg: TTextSearchDialog;
begin
  if AReplace then
    dlg := TTextReplaceDialog.Create(Self)
  else
    dlg := TTextSearchDialog.Create(Self);

  with dlg do
    try
      // assign search options
      SearchBackwards := gbSearchBackwards;
      SearchCaseSensitive := gbSearchCaseSensitive;
      SearchFromCursor := gbSearchFromCaret;
      SearchInSelectionOnly := gbSearchSelectionOnly;
      // start with last search text
      SearchText := gsSearchText;
      if gbSearchTextAtCaret then
      begin
        // if something is selected search for that text
        if syndtScriptEditor.SelAvail and (syndtScriptEditor.BlockBegin.Line = syndtScriptEditor.BlockEnd.Line) then
          SearchText := syndtScriptEditor.SelText
        else
          SearchText := syndtScriptEditor.GetWordAtRowCol(syndtScriptEditor.CaretXY);
      end;

      SearchTextHistory := gsSearchTextHistory;
      if AReplace then
        with dlg as TTextReplaceDialog do
        begin
          ReplaceText := gsReplaceText;
          ReplaceTextHistory := gsReplaceTextHistory;
        end;

      SearchWholeWords := gbSearchWholeWords;
      if ShowModal = mrOK then
      begin
        gbSearchBackwards := SearchBackwards;
        gbSearchCaseSensitive := SearchCaseSensitive;
        gbSearchFromCaret := SearchFromCursor;
        gbSearchSelectionOnly := SearchInSelectionOnly;
        gbSearchWholeWords := SearchWholeWords;
        gbSearchRegex := SearchRegularExpression;
        gsSearchText := SearchText;
        gsSearchTextHistory := SearchTextHistory;

        if AReplace then
          with dlg as TTextReplaceDialog do
          begin
            gsReplaceText := ReplaceText;
            gsReplaceTextHistory := ReplaceTextHistory;
          end;

        FSearchFromCaret := gbSearchFromCaret;
        if gsSearchText <> '' then
        begin
          DoSearchReplaceText(AReplace, gbSearchBackwards);
          FSearchFromCaret := true;
        end;
      end;
    finally
      dlg.Free;
    end;
end;

procedure TfrmTXTEditor.syndtScriptEditorGutterClick(Sender: TObject; Button: TMouseButton; X, Y, Line: Integer;
  Mark: TSynEditMark);
begin
  syndtScriptEditor.SelLength := length(syndtScriptEditor.Lines[syndtScriptEditor.CaretY - 1]);
end;

procedure TfrmTXTEditor.syndtScriptEditorReplaceText(Sender: TObject; const ASearch, AReplace: string; Line, Column: Integer;
  var Action: TSynReplaceAction);
var
  APos: TPoint;
  EditRect: TRect;
begin
  if ASearch = AReplace then
    Action := raSkip
  else
  begin
    APos := syndtScriptEditor.ClientToScreen
      (syndtScriptEditor.RowColumnToPixels(syndtScriptEditor.BufferToDisplayPos(BufferCoord(Column, Line))));
    EditRect := ClientRect;
    EditRect.TopLeft := ClientToScreen(EditRect.TopLeft);
    EditRect.BottomRight := ClientToScreen(EditRect.BottomRight);

    if ConfirmReplaceDialog = nil then
      ConfirmReplaceDialog := TConfirmReplaceDialog.Create(Application);
    ConfirmReplaceDialog.PrepareShow(EditRect, APos.X, APos.Y, APos.Y + syndtScriptEditor.LineHeight, ASearch);
    case ConfirmReplaceDialog.ShowModal of
      mrYes:
        Action := raReplace;
      mrYesToAll:
        Action := raReplaceAll;
      mrNo:
        Action := raSkip;
    else
      Action := raCancel;
    end;
  end;
end;

procedure TfrmTXTEditor.DoSectionTargeting(Sender: TObject);
begin
  if tvLocate.SelectionCount > 0 then
    SectionTargeting(tvLocate.Selected.Text);
end;

procedure TfrmTXTEditor.actSaveUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := syndtScriptEditor.Modified;
end;

procedure TfrmTXTEditor.actSearchExecute(Sender: TObject);
begin
  ShowSearchReplaceDialog(false);
end;

procedure TfrmTXTEditor.actSearchNextExecute(Sender: TObject);
begin
  DoSearchReplaceText(false, false);
end;

procedure TfrmTXTEditor.actSearchNextUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := (gsSearchText <> '')
end;

procedure TfrmTXTEditor.actSearchPrevExecute(Sender: TObject);
begin
  DoSearchReplaceText(false, true);
end;

procedure TfrmTXTEditor.actSearchPrevUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := (gsSearchText <> '')
end;

procedure TfrmTXTEditor.actSearchReplaceExecute(Sender: TObject);
begin
  ShowSearchReplaceDialog(true);
end;

procedure TfrmTXTEditor.actSearchReplaceUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := (tvLocate.Items.Count > 0) and not syndtScriptEditor.ReadOnly;
end;

procedure TfrmTXTEditor.actSearchUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := tvLocate.Items.Count > 0;
end;

procedure TfrmTXTEditor.actSelectAllExecute(Sender: TObject);
begin
  syndtScriptEditor.SelectAll();
end;

procedure TfrmTXTEditor.actUndoUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := syndtScriptEditor.CanUndo;
end;

procedure TfrmTXTEditor.actUndoExecute(Sender: TObject);
begin
  syndtScriptEditor.Undo;
end;

procedure TfrmTXTEditor.actReadOlnyExecute(Sender: TObject);
begin
  actReadOlny.Checked := not syndtScriptEditor.ReadOnly;
  syndtScriptEditor.ReadOnly := not syndtScriptEditor.ReadOnly;
end;

procedure TfrmTXTEditor.actRedoExecute(Sender: TObject);
begin
  syndtScriptEditor.Redo;
end;

procedure TfrmTXTEditor.actRedoUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := syndtScriptEditor.CanRedo;
end;

procedure TfrmTXTEditor.actReloadExecute(Sender: TObject);
var
  i: Integer;
  tmpMerchant: TMerchant;
  tmpNormNpc: TNormNpc;
begin
  if not FIsCallMobified then
  begin
    for i := 0 to lstNPC.Count - 1 do
    begin
      if lstNPC.ItemChecked[i] then
      begin
        lstNPC.ItemChecked[i] := false;

        if (lstNPC.Items.Objects[i] = g_ManageNPC) or (lstNPC.Items.Objects[i] = g_RobotNPC) then // 类继承设计混乱不得已如此区分
        begin
          tmpNormNpc := TNormNpc(lstNPC.Items.Objects[i]);
          tmpNormNpc.ClearScript;
          tmpNormNpc.LoadNpcScript;
          MainOutMessage(Format('重新加载脚本 “%s”...', [tmpNormNpc.m_sScript]));
        end
        else
        begin
          tmpMerchant := TMerchant(lstNPC.Items.Objects[i]);
          tmpMerchant.ClearScript;
          tmpMerchant.LoadNpcScript;
          MainOutMessage(Format('重新加载脚本 “%s”...', [tmpMerchant.m_sScript]));
        end;
      end;
    end;
  end
  else
  begin
    for i := 0 to lstNPC.Count - 1 do
      lstNPC.ItemChecked[i] := false;

    frmMain.MENU_CONTROL_RELOAD_NPCClick(nil);
    FIsCallMobified := false;
  end;
  actReload.Enabled := false;
end;

procedure TfrmTXTEditor.actCopyExecute(Sender: TObject);
begin
  syndtScriptEditor.CopyToClipboard();
end;

procedure TfrmTXTEditor.actCopyUpdate(Sender: TObject);
begin
  TAction(Sender).Enabled := syndtScriptEditor.SelLength > 0;
end;

procedure TfrmTXTEditor.actCutExecute(Sender: TObject);
begin
  syndtScriptEditor.CutToClipboard();
end;

procedure TfrmTXTEditor.actCutUpdate(Sender: TObject);
begin
  TAction(Sender).Enabled := not syndtScriptEditor.ReadOnly and (syndtScriptEditor.SelLength > 0);
end;

procedure TfrmTXTEditor.actExternalOpenExecute(Sender: TObject);
var
  tmpPath, tmpFile: string;
  tmpMerchant: TMerchant;
begin
  if lstNPC.ItemIndex < 0 then
    exit;

  tmpMerchant := TMerchant(lstNPC.Items.Objects[lstNPC.ItemIndex]);
  tmpPath := GetMerchantScriptFile(tmpMerchant);
  if (tmpMerchant = g_ManageNPC) or (tmpMerchant = g_RobotNPC) then
    tmpFile := Format('%s%s.txt', [tmpPath, tmpMerchant.m_sScript])
  else
    tmpFile := Format('%s%s-%s.txt', [tmpPath, tmpMerchant.m_sScript, tmpMerchant.m_sMapName]);

  ShellExecute(0, nil, PChar('notepad.exe'), PChar(tmpFile), nil, SW_SHOWNORMAL);
end;

procedure TfrmTXTEditor.actFormatExecute(Sender: TObject);
var
  i, CharCount, EmptyCount: Integer;
  IsIf, IsIndentation: Boolean;
  tmpstr, Boundary: string;
begin
  IsIf := false;
  EmptyCount := 0;
  Boundary := ';';

  if syndtScriptEditor.Text = EmptyStr then
    exit;

  if (syndtScriptEditor.Lines.Count > 5000) and
    (Application.MessageBox('当前脚本过大，整理格式可能造成M2短暂卡顿，是否继续整理？', '整理提示', MB_YESNO + MB_ICONQUESTION) = IDNO) then
    exit;

  IsIndentation := Application.MessageBox('整理时是否使用代码缩进？', '整理提示', MB_YESNO + MB_ICONQUESTION) = IDYES;

  syndtScriptEditor.Lines.BeginUpdate;
  for i := syndtScriptEditor.Lines.Count - 1 downto 0 do
  begin
    { 大小写转换并去掉首尾空格 }
    syndtScriptEditor.Lines[i] := Trim(UpperCase(syndtScriptEditor.Lines[i]));

    { 删除多余的空行 }
    if syndtScriptEditor.Lines[i] = EmptyStr then
      syndtScriptEditor.Lines.Delete(i);

    { 清除注释行所有空格 }
    if syndtScriptEditor.Lines[i].StartsWith(';', true) then
    begin
      tmpstr := Copy(syndtScriptEditor.Lines[i], 2, length(syndtScriptEditor.Lines[i]) - 1);
      syndtScriptEditor.Lines[i] := ';' + Trim(tmpstr);
    end;

    { 段分界线 }
    if syndtScriptEditor.Lines[i].StartsWith('[@', true) then
    begin
      if i > 0 then
      begin
        tmpstr := syndtScriptEditor.Lines[i - 1]; // 临时字串用于检查段符号所在行的上一行是否有注释
        tmpstr := Trim(tmpstr).Replace(#9 { 去掉制表符 } , '').Replace('　' { 去掉全角空格 } , '');

        { 根据段标志决定是否插入分界线 }
        if (tmpstr = EmptyStr) or (tmpstr[1] <> ';') then
          syndtScriptEditor.Lines.Insert(i, Boundary.PadRight(syndtScriptEditor.RightEdge, '-'));
      end;
    end;
  end;

  for i := 0 to syndtScriptEditor.Lines.Count - 1 do
  begin
    CharCount := length(syndtScriptEditor.Lines[i]);

    if syndtScriptEditor.Lines[i].StartsWith('[@', true) then
    begin
      EmptyCount := 1;
      IsIf := false;
      Continue;
    end;

    if syndtScriptEditor.Lines[i].StartsWith('#if', true) //
      or syndtScriptEditor.Lines[i].StartsWith('#or', true) //
      or syndtScriptEditor.Lines[i].StartsWith('#say', true) //
      or syndtScriptEditor.Lines[i].StartsWith('#act', true) //
      or syndtScriptEditor.Lines[i].StartsWith('#elsesay', true) //
      or syndtScriptEditor.Lines[i].StartsWith('#elseact', true) then
    begin
      if IsIf = true then
        Dec(EmptyCount);
      IsIf := true;

      if IsIndentation then
      begin
        syndtScriptEditor.Lines[i] := syndtScriptEditor.Lines[i].PadLeft(CharCount + EmptyCount, ' ');
        Inc(EmptyCount);
      end;
      Continue;
    end;

    if syndtScriptEditor.Lines[i].StartsWith('break', true) then
    begin
      IsIf := false;

      if IsIndentation then
      begin
        syndtScriptEditor.Lines[i] := syndtScriptEditor.Lines[i].PadLeft(CharCount + EmptyCount, ' ');
        Dec(EmptyCount);
      end;
      Continue;
    end;

    if IsIndentation //
      and (syndtScriptEditor.Lines[i] <> EmptyStr) //
      and (syndtScriptEditor.Lines[i][1] <> ';') then
      syndtScriptEditor.Lines[i] := syndtScriptEditor.Lines[i].PadLeft(CharCount + EmptyCount, ' ');
  end;
  syndtScriptEditor.Lines.EndUpdate;
  syndtScriptEditor.Modified := true;
end;

procedure TfrmTXTEditor.actFormatterUpdate(Sender: TObject);
begin
  TAction(Sender).Enabled := (syndtScriptEditor.Text <> '') and not syndtScriptEditor.ReadOnly;
end;

procedure TfrmTXTEditor.actFormatUpdate(Sender: TObject);
begin
  (Sender as TAction).Enabled := tvLocate.Items.Count > 0;
end;

procedure TfrmTXTEditor.actGetMoveCmdExecute(Sender: TObject);
var
  tmpMerchant: TMerchant;
begin
  if lstNPC.ItemIndex < 0 then
    exit;

  tmpMerchant := TMerchant(lstNPC.Items.Objects[lstNPC.ItemIndex]);

  if not SameText(tmpMerchant.m_PEnvir.sMapName, '0') //
    and (tmpMerchant.m_nCurrX > 0) //
    and (tmpMerchant.m_nCurrY > 0) then
    Clipboard.AsText := Format('@%s %s %d %d', [g_GameCommand.POSITIONMOVE.sCmd, tmpMerchant.m_PEnvir.sMapName,
      tmpMerchant.m_nCurrX, tmpMerchant.m_nCurrY])
  else
    ShowMessage('无法获取功能NPC移动命令！');
end;

procedure TfrmTXTEditor.actGotoExecute(Sender: TObject);
var
  tmpSL, tmpSubSL: TStrings;
  tmpLine, tmpLabel, tmpPath, tmpFilePath: string;
begin
  tmpLine := Trim(syndtScriptEditor.Lines[syndtScriptEditor.CaretXY.Line - 1]);
  if AnsiStartsText(';', tmpLine) then
    exit;

  { 处理#CALL后不追加空格的特殊情况 }
  if AnsiStartsText('#CALL[', tmpLine) then
  begin
    tmpLine := tmpLine.Replace('#CALL[', '#CALL [');
    syndtScriptEditor.Lines[syndtScriptEditor.CaretXY.Line - 1] := //
      syndtScriptEditor.Lines[syndtScriptEditor.CaretXY.Line - 1].Replace('#CALL[', '#CALL [');

    if not syndtScriptEditor.Modified then // 跳出文件时，如果之前未作修改则默认保存
      actSaveExecute(nil);
  end;

  tmpSL := TStringList.Create;
  ExtractStrings([' ', #9], [], PChar(tmpLine), tmpSL);

  if SameText('#CALL', tmpSL[0]) then
  begin
    if tmpSL.Count >= 3 then
    begin
      tmpPath := Trim(tmpSL[1]).Replace('[', '').Replace(']', '').Replace('\\', '\');
      if (tmpPath <> '') and (tmpPath[1] <> '\') then
        tmpPath := '\' + tmpPath;

      tmpLabel := Format('[%s]', [Trim(tmpSL[2])]);
      tmpFilePath := ExtractFilePath(ParamStr(0)) + 'Envir\QuestDiary' + tmpPath;
      if LoadScriptTxt(tmpFilePath, true) then
        SectionTargeting(tmpLabel);

      FIsCallMobified := true;
    end;
  end
  else if SameText('GOTO', tmpSL[0]) then
  begin
    if tmpSL.Count >= 2 then
    begin
      tmpLabel := Format('[%s]', [Trim(tmpSL[1])]);
      SectionTargeting(tmpLabel);
    end;
  end
  else if SameText('#CHILD', tmpSL[0]) then
  begin
    if tmpSL.Count >= 4 then
    begin
      tmpLabel := Trim(tmpSL[3]).Replace('[', '').Replace(']', '');
      tmpSubSL := TStringList.Create;
      tmpSubSL.StrictDelimiter := true;
      tmpSubSL.CommaText := tmpLabel;
      if tmpSubSL.Count >= 3 then
        SectionTargeting(Format('[%s]', [Trim(tmpSubSL[2])]));
      tmpSubSL.Free;
    end;
  end
  else if SameText('GOTOLABEL', tmpSL[0]) or SameText('DELAYGOTO', tmpSL[0]) or SameText('DELAYCALL', tmpSL[0]) then
  begin
    if tmpSL.Count >= 3 then
    begin
      tmpLabel := Format('[%s]', [Trim(tmpSL[2])]);
      SectionTargeting(tmpLabel);
    end;
  end;
  tmpSL.Free;
end;

procedure TfrmTXTEditor.actHelpExecute(Sender: TObject);
begin
  ShellExecute(Application.Handle, nil, 'https://gxxm2.com/#/technical', nil, nil, SW_SHOWNORMAL);
end;

procedure TfrmTXTEditor.actNoteExecute(Sender: TObject);
var
  tmpstr: string;
  i, tmpStratLine, tmpEndLine: Integer;
begin
  with syndtScriptEditor do
  begin
    tmpStratLine := CharIndexToRowCol(SelStart).Line - 1;
    tmpEndLine := CharIndexToRowCol(SelEnd).Line - 1;

    for i := tmpStratLine to tmpEndLine do
    begin
      tmpstr := Trim(Lines[i]);
      if not AnsiStartsText(';', tmpstr) then
      begin
        Lines[i] := '; ' + tmpstr;
        syndtScriptEditor.Modified := true;
      end;
    end;
  end;
end;

procedure TfrmTXTEditor.actUnnoteExecute(Sender: TObject);
var
  tmpstr: string;
  i, j, tmpStratLine, tmpEndLine: Integer;
begin
  with syndtScriptEditor do
  begin
    tmpStratLine := CharIndexToRowCol(SelStart).Line - 1;
    tmpEndLine := CharIndexToRowCol(SelEnd).Line - 1;

    for i := tmpStratLine to tmpEndLine do
    begin
      tmpstr := Trim(Lines[i]);
      if AnsiStartsText(';', tmpstr) then
      begin
        for j := 1 to length(tmpstr) do
        begin
          if (tmpstr[j] <> ';') and (tmpstr[j] <> #9) and (tmpstr[j] <> ' ') and (tmpstr[j] <> '　') then
          begin
            syndtScriptEditor.Modified := true;
            Lines[i] := Copy(tmpstr, j, length(tmpstr));
            Break;
          end;
        end;
      end;
    end;
  end;
end;

procedure TfrmTXTEditor.actPasteExecute(Sender: TObject);
begin
  syndtScriptEditor.PasteFromClipboard();
end;

procedure TfrmTXTEditor.actPasteUpdate(Sender: TObject);
begin
  TAction(Sender).Enabled := not syndtScriptEditor.CanPaste;
end;

procedure TfrmTXTEditor.actSaveExecute(Sender: TObject);
var
  i: Integer;
  tmpMerchant: TMerchant;
  tmpPath, tmpFile: string;
begin
  if syndtScriptEditor.ReadOnly then
  begin
    ShowMessage('当前脚本文件设置为只读状态，无法对修改进行保存！');
    exit;
  end;

  try
    syndtScriptEditor.Lines.SaveToFile(FCurrFile);

    if FCurrNPCIndex >= 0 then
      lstNPC.ItemChecked[FCurrNPCIndex] := true;

    for i := 0 to lstNPC.Count - 1 do
    begin
      tmpMerchant := TMerchant(lstNPC.Items.Objects[i]);
      tmpPath := GetMerchantScriptFile(tmpMerchant);
      if (tmpMerchant = g_ManageNPC) or (tmpMerchant = g_RobotNPC) then
        tmpFile := Format('%s%s.txt', [tmpPath, tmpMerchant.m_sScript])
      else
        tmpFile := Format('%s%s-%s.txt', [tmpPath, tmpMerchant.m_sScript, tmpMerchant.m_sMapName]);

      if SameText(FCurrFile, tmpFile) then
      begin
        RefreshScript;
        Break;
      end;
    end;
    syndtScriptEditor.Modified := false;
    actReload.Enabled := true;
  except
    ShowMessage('保存文件失败，文件可能被占用！');
  end;
end;

procedure TfrmTXTEditor.RefreshScript;
const
  TEST = '\[@.+?\]';
var
  i: Integer;
  tmpRegex: TRegEx;
  tmpMatch: TMatch;
  tmpData, tmpSections: TStrings;
begin
  tmpData := TStringList.Create;
  tmpSections := TStringList.Create;
  try
    for i := 0 to syndtScriptEditor.Lines.Count - 1 do
    begin
      if Trim(syndtScriptEditor.Lines[i]).IsEmpty or (Trim(syndtScriptEditor.Lines[i])[1] = ';') then
        Continue;

      tmpData.Add(Trim(syndtScriptEditor.Lines[i]));
    end;

    tmpRegex := TRegEx.Create(TEST, [roIgnoreCase, roMultiLine]);
    tmpMatch := tmpRegex.Match(tmpData.Text);

    while tmpMatch.Success do
    begin
      if tmpSections.IndexOf(tmpMatch.Value) < 0 then
      begin
        tmpSections.Add(tmpMatch.Value);
        tmpMatch := tmpMatch.NextMatch;
      end
      else
      begin
        Application.MessageBox(PChar('脚本段名称 “' + tmpMatch.Value + '” 重复，请修正后重新加载！'), PChar('错误'));
        SectionTargeting(tmpMatch.Value);
        exit;
      end;
    end;

    tvLocate.Items.Clear;
    for i := 0 to tmpSections.Count - 1 do
      tvLocate.Items.addChild(nil, Trim(tmpSections[i]));
  finally
    tmpSections.Free;
    tmpData.Free;
  end;
end;

end.
