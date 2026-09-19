unit uFrmUserShopGetMoneyTotal;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ExtCtrls, ComCtrls, M2Share, Menus, M2DataCommon;

type
  TFrmUserShopGetMoneyTotal = class(TForm)
    lvMoney: TListView;
    Panel1: TPanel;
    lbl1: TLabel;
    lbl211: TLabel;
    edt1: TEdit;
    lbl2: TLabel;
    edt2: TEdit;
    lbl3: TLabel;
    edt3: TEdit;
    lbl4: TLabel;
    edt4: TEdit;
    lbl5: TLabel;
    edt5: TEdit;
    pm1: TPopupMenu;
    mniN1: TMenuItem;
    procedure FormCreate(Sender: TObject);
    procedure mniN1Click(Sender: TObject);
  private
    { Private declarations }
    procedure RefreshData;
  public
    { Public declarations }
  end;

  procedure ShowFrmUserShopGetMoneyTotal;

implementation

{$R *.dfm}

procedure ShowFrmUserShopGetMoneyTotal;
var
  FrmUserShopGetMoneyTotal: TFrmUserShopGetMoneyTotal;
begin
  FrmUserShopGetMoneyTotal := TFrmUserShopGetMoneyTotal.Create(nil);
  try
    FrmUserShopGetMoneyTotal.RefreshData;
    FrmUserShopGetMoneyTotal.ShowModal;
  finally
    FrmUserShopGetMoneyTotal.Free;
  end;
end;

{ TFrmUserShopGetMoneyTotal }

procedure TFrmUserShopGetMoneyTotal.RefreshData;
var
  I, J: Integer;
  Item: TListItem;
  MoneyItem: PTSelledAndNoGetMoneyTotal;
  SumMoneys: array[0..4] of Int64;
  Moneys: array[0..4] of Int64;
  ItemList: TList;

  LastHuman: string;
begin
  lvMoney.Clear;

  SumMoneys[0] := 0;
  SumMoneys[1] := 0;
  SumMoneys[2] := 0;
  SumMoneys[3] := 0;
  SumMoneys[4] := 0;

  Moneys[0] := 0;
  Moneys[1] := 0;
  Moneys[2] := 0;
  Moneys[3] := 0;
  Moneys[4] := 0;

  LastHuman := '';

  ItemList := TList.Create;
  try
    if g_M2DataDB.UserShopDB.GetSelledAndNoGetMoneyTotal(ItemList) > 0 then
    begin
      for I := 0 to ItemList.Count - 1 do
      begin
        MoneyItem := ItemList.Items[I];

        if not SameText(MoneyItem.sMasterName, LastHuman) then
        begin
          if Length(LastHuman) > 0 then
          begin
            if (Moneys[0] <> 0) or (Moneys[1] <> 0) or (Moneys[2] <> 0) or (Moneys[3] <> 0) or (Moneys[4] <> 0) then
            begin
              Item := lvMoney.Items.Add;
              Item.Caption := LastHuman;

              for J := 0 to 4 do
              begin
                Item.SubItems.Add(IntToStr(Moneys[J]));
              end;
            end;
          end;

          Moneys[0] := 0;
          Moneys[1] := 0;
          Moneys[2] := 0;
          Moneys[3] := 0;
          Moneys[4] := 0;
          LastHuman := MoneyItem.sMasterName;
        end;

        if MoneyItem.btMoneyType in [0..4] then
        begin
          Moneys[MoneyItem.btMoneyType] := Moneys[MoneyItem.btMoneyType] + MoneyItem.nSumPrice;
          SumMoneys[MoneyItem.btMoneyType] := SumMoneys[MoneyItem.btMoneyType] + MoneyItem.nSumPrice;
        end;

      end;
    end;
  finally
    ItemList.Free;
  end;

  if (Moneys[0] <> 0) or (Moneys[1] <> 0) or (Moneys[2] <> 0) or (Moneys[3] <> 0) or (Moneys[4] <> 0) then
  begin
    Item := lvMoney.Items.Add;
    Item.Caption := LastHuman;

    for J := 0 to 4 do
    begin
      Item.SubItems.Add(IntToStr(Moneys[J]));
    end;
  end;

  edt1.Text := IntToStr(SumMoneys[0]);
  edt2.Text := IntToStr(SumMoneys[1]);
  edt3.Text := IntToStr(SumMoneys[2]);
  edt4.Text := IntToStr(SumMoneys[3]);
  edt5.Text := IntToStr(SumMoneys[4]);
end;

procedure TFrmUserShopGetMoneyTotal.FormCreate(Sender: TObject);
begin
  lvMoney.Column[1].Caption := g_Config.sGameGoldName;
  lvMoney.Column[2].Caption := g_Config.sGamePointName;
  lvMoney.Column[3].Caption := sSTRING_GOLDNAME;
  lvMoney.Column[4].Caption := g_Config.sGameDiamondName;
  lvMoney.Column[5].Caption := g_Config.sGameGirdName;

  lbl1.Caption := lvMoney.Column[1].Caption + ':';
  lbl2.Caption := lvMoney.Column[2].Caption + ':';
  lbl3.Caption := lvMoney.Column[3].Caption + ':';
  lbl4.Caption := lvMoney.Column[4].Caption + ':';
  lbl5.Caption := lvMoney.Column[5].Caption + ':';

  lbl1.Left := edt1.Left - Canvas.TextWidth(lbl1.Caption) - 2;
  lbl2.Left := edt2.Left - Canvas.TextWidth(lbl2.Caption) - 2;
  lbl3.Left := edt3.Left - Canvas.TextWidth(lbl3.Caption) - 2;
  lbl4.Left := edt4.Left - Canvas.TextWidth(lbl4.Caption) - 2;
  lbl5.Left := edt5.Left - Canvas.TextWidth(lbl5.Caption) - 2;

end;

procedure TFrmUserShopGetMoneyTotal.mniN1Click(Sender: TObject);
begin
  RefreshData;
end;

end.
