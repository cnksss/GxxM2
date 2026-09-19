object FrmUserShopView: TFrmUserShopView
  Left = 469
  Top = 336
  BorderStyle = bsDialog
  Caption = #20010#20154#21830#24215#31649#29702
  ClientHeight = 435
  ClientWidth = 528
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 13
  object lbl1: TLabel
    Left = 12
    Top = 405
    Width = 48
    Height = 13
    Caption = #26597#25214#29609#23478
  end
  object lbl2: TLabel
    Left = 261
    Top = 405
    Width = 48
    Height = 13
    Caption = #24215#38138#21517#31216
  end
  object lvItems: TListView
    Left = 12
    Top = 9
    Width = 509
    Height = 380
    Columns = <
      item
        Caption = #24215#38138#32534#21495
        Width = 86
      end
      item
        Caption = #29609#23478#21517#31216
        Width = 129
      end
      item
        Caption = #24215#38138#21517#31216
        Width = 167
      end
      item
        Caption = #21019#24314#26102#38388
        Width = 97
      end>
    GridLines = True
    ReadOnly = True
    RowSelect = True
    TabOrder = 0
    ViewStyle = vsReport
    OnSelectItem = lvItemsSelectItem
  end
  object edtUser: TEdit
    Left = 67
    Top = 401
    Width = 95
    Height = 21
    TabOrder = 1
  end
  object btnSearch: TButton
    Left = 167
    Top = 398
    Width = 70
    Height = 27
    Caption = #26597#25214
    TabOrder = 2
    OnClick = btnSearchClick
  end
  object edtShopName: TEdit
    Left = 322
    Top = 401
    Width = 130
    Height = 21
    Enabled = False
    TabOrder = 3
  end
  object btnRename: TButton
    Left = 460
    Top = 398
    Width = 63
    Height = 27
    Caption = #20462#25913
    Enabled = False
    TabOrder = 4
    OnClick = btnRenameClick
  end
end
