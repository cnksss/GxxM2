unit ConfigMonGen;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, StdCtrls, M2Threads, Vcl.Clipbrd,
  Vcl.ExtCtrls;

type
  TfrmConfigMonGen = class(TForm)
    ListBoxMonGen: TListBox;
    pnl: TPanel;
    procedure ListBoxMonGenDblClick(Sender: TObject);
  private
    { Private declarations }
  public
    procedure Open();
    { Public declarations }
  end;

var
  frmConfigMonGen: TfrmConfigMonGen;

implementation

uses
  UsrEngn, M2Share;

{$R *.dfm}
{ TfrmConfigMonGen }

procedure TfrmConfigMonGen.ListBoxMonGenDblClick(Sender: TObject);
begin
  if ListBoxMonGen.ItemIndex >= 0 then
    Clipboard.AsText := ListBoxMonGen.Items[ListBoxMonGen.ItemIndex];
end;

procedure TfrmConfigMonGen.Open;
var
  I: Integer;
  MonGen: pTMonGenInfo;
begin
  if g_MultiThreadRun then
    UserEngine.m_MonGenList.LockR(14);
  try
    for I := 0 to UserEngine.m_MonGenList.Count - 1 do
    begin
      MonGen := UserEngine.m_MonGenList.Items[I];
      ListBoxMonGen.Items.AddObject(MonGen.sMapName + '(' + IntToStr(MonGen.nX) + ':' + IntToStr(MonGen.nY) + ')' + ' - ' + MonGen.sMonName,
        TObject(MonGen));
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_MonGenList.UnLockR;
  end;
  Self.ShowModal;
end;

end.

