unit ViewKernelInfo;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, ExtCtrls, ComCtrls, Grids, M2Share,
  M2Threads;

type
  TfrmViewKernelInfo = class(TForm)
    Timer: TTimer;
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    GroupBox1: TGroupBox;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    EditLoadHumanDBCount: TEdit;
    EditLoadHumanDBErrorCoun: TEdit;
    EditSaveHumanDBCount: TEdit;
    EditHumanDBQueryID: TEdit;
    TabSheet2: TTabSheet;
    GroupBox2: TGroupBox;
    Label5: TLabel;
    Label6: TLabel;
    EditWinLotteryCount: TEdit;
    EditNoWinLotteryCount: TEdit;
    GroupBox3: TGroupBox;
    Label9: TLabel;
    Label10: TLabel;
    Label11: TLabel;
    Label12: TLabel;
    EditWinLotteryLevel1: TEdit;
    EditWinLotteryLevel2: TEdit;
    EditWinLotteryLevel3: TEdit;
    EditWinLotteryLevel4: TEdit;
    Label13: TLabel;
    EditWinLotteryLevel5: TEdit;
    Label14: TLabel;
    EditWinLotteryLevel6: TEdit;
    GroupBox4: TGroupBox;
    Label7: TLabel;
    Label8: TLabel;
    EditItemNumber: TEdit;
    EditItemNumberEx: TEdit;
    TabSheet3: TTabSheet;
    GroupBox5: TGroupBox;
    Label15: TLabel;
    Label16: TLabel;
    Label17: TLabel;
    Label18: TLabel;
    Label19: TLabel;
    Label20: TLabel;
    EditGlobalVal1: TEdit;
    EditGlobalVal2: TEdit;
    EditGlobalVal3: TEdit;
    EditGlobalVal4: TEdit;
    EditGlobalVal5: TEdit;
    EditGlobalVal6: TEdit;
    EditGlobalVal7: TEdit;
    Label21: TLabel;
    Label22: TLabel;
    EditGlobalVal8: TEdit;
    EditGlobalVal9: TEdit;
    Label23: TLabel;
    EditGlobalVal10: TEdit;
    Label24: TLabel;
    TabSheet5: TTabSheet;
    GroupBox7: TGroupBox;
    GridThread: TStringGrid;
    TabSheet6: TTabSheet;
    GroupBox8: TGroupBox;
    GridMemory: TStringGrid;
    procedure TimerTimer(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmViewKernelInfo: TfrmViewKernelInfo;

implementation



{$R *.dfm}
{ TfrmViewKernelInfo }

procedure TfrmViewKernelInfo.FormCreate(Sender: TObject);
var
  Config: pTConfig;
  ThreadInfo: pTThreadInfo;
begin
  Config := @g_Config;
  GridThread.Cells[0, 0] := '序号';
  GridThread.Cells[1, 0] := '描述';
  GridThread.Cells[2, 0] := '句柄';
  GridThread.Cells[3, 0] := '线程ID';
  GridThread.Cells[4, 0] := '运行时间';
  GridThread.Cells[5, 0] := 'CPU占用';

  GridMemory.Cells[0, 0] := '名称';
  GridMemory.Cells[1, 0] := '数量';
  GridMemory.Cells[2, 0] := '大小';

  GridMemory.Cells[0, 1] := 'UserItem';
  GridMemory.Cells[0, 2] := 'MapItem';
  GridMemory.Cells[0, 3] := 'BaseObject';
  GridMemory.Cells[0, 4] := 'MapEvent';

  ThreadInfo := @Config.UserEngineThread;
  ThreadInfo.hThreadHandle := 0;
  ThreadInfo.dwRunTick := 0;
  ThreadInfo.nRunTime := 0;
  ThreadInfo.nMaxRunTime := 0;
  ThreadInfo.nRunFlag := 0;
  ThreadInfo := @Config.IDSocketThread;
  ThreadInfo.hThreadHandle := 0;
  ThreadInfo.dwRunTick := 0;
  ThreadInfo.nRunTime := 0;
  ThreadInfo.nMaxRunTime := 0;
  ThreadInfo.nRunFlag := 0;
  ThreadInfo := @Config.DBSOcketThread;
  ThreadInfo.hThreadHandle := 0;
  ThreadInfo.dwRunTick := 0;
  ThreadInfo.nRunTime := 0;
  ThreadInfo.nMaxRunTime := 0;
  ThreadInfo.nRunFlag := 0;
end;

procedure TfrmViewKernelInfo.Open;
begin
  Timer.Enabled := True;
  ShowModal;
  Timer.Enabled := False;
end;

procedure TfrmViewKernelInfo.TimerTimer(Sender: TObject);
var
  I: Integer;
  Thread: TM2RunThread;
  ThreadCPUUsage: Integer;
begin
  EditLoadHumanDBCount.Text := IntToStr(g_Config.nLoadDBCount);
  EditLoadHumanDBErrorCoun.Text := IntToStr(g_Config.nLoadDBErrorCount);
  EditSaveHumanDBCount.Text := IntToStr(g_Config.nSaveDBCount);
  EditHumanDBQueryID.Text := IntToStr(g_Config.nDBQueryID);

  EditItemNumber.Text := IntToStr(g_Config.nItemNumber);
  EditItemNumberEx.Text := IntToStr(g_Config.nItemNumberEx);

  EditWinLotteryCount.Text := IntToStr(g_Config.nWinLotteryCount);
  EditNoWinLotteryCount.Text := IntToStr(g_Config.nNoWinLotteryCount);
  EditWinLotteryLevel1.Text := IntToStr(g_Config.nWinLotteryLevel1);
  EditWinLotteryLevel2.Text := IntToStr(g_Config.nWinLotteryLevel2);
  EditWinLotteryLevel3.Text := IntToStr(g_Config.nWinLotteryLevel3);
  EditWinLotteryLevel4.Text := IntToStr(g_Config.nWinLotteryLevel4);
  EditWinLotteryLevel5.Text := IntToStr(g_Config.nWinLotteryLevel5);
  EditWinLotteryLevel6.Text := IntToStr(g_Config.nWinLotteryLevel6);

  EditGlobalVal1.Text := IntToStr(g_Config.GlobalVal[0]);
  EditGlobalVal2.Text := IntToStr(g_Config.GlobalVal[1]);
  EditGlobalVal3.Text := IntToStr(g_Config.GlobalVal[2]);
  EditGlobalVal4.Text := IntToStr(g_Config.GlobalVal[3]);
  EditGlobalVal5.Text := IntToStr(g_Config.GlobalVal[4]);
  EditGlobalVal6.Text := IntToStr(g_Config.GlobalVal[5]);
  EditGlobalVal7.Text := IntToStr(g_Config.GlobalVal[6]);
  EditGlobalVal8.Text := IntToStr(g_Config.GlobalVal[7]);
  EditGlobalVal9.Text := IntToStr(g_Config.GlobalVal[8]);
  EditGlobalVal10.Text := IntToStr(g_Config.GlobalVal[9]);

  GridThread.RowCount := g_M2RunThreadMgr.Count + 1;
  for I := 0 to g_M2RunThreadMgr.Count - 1 do
  begin
    Thread := g_M2RunThreadMgr.Items[I];
    GridThread.Cells[0, I + 1] := IntToStr(I);
    GridThread.Cells[1, I + 1] := Thread.ThreadDesc;
    GridThread.Cells[2, I + 1] := IntToStr(Thread.Handle);
    GridThread.Cells[3, I + 1] := IntToStr(Thread.ThreadID);
    GridThread.Cells[4, I + 1] := Format('%d/%d/%d', [Thread.RunTick, Thread.MinRunTick, Thread.MaxRunTick]);
    ThreadCPUUsage := Thread.ThreadCPUUsage;
    if ThreadCPUUsage > Thread.MaxThreadCPUUsage then
      Thread.MaxThreadCPUUsage := ThreadCPUUsage;
    GridThread.Cells[5, I + 1] := Format('%d/%d', [ThreadCPUUsage, Thread.MaxThreadCPUUsage]);
  end;

  GridMemory.Cells[1, 2] := '';
  GridMemory.Cells[1, 3] := '';
  GridMemory.Cells[1, 4] := '';
end;

end.

