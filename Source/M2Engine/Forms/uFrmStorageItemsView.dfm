object FrmStorageItemsView: TFrmStorageItemsView
  Left = 305
  Top = 130
  Caption = #29992#25143#26080#38480#20179#24211#31649#29702
  ClientHeight = 525
  ClientWidth = 810
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object lbl1: TLabel
    Left = 9
    Top = 500
    Width = 48
    Height = 13
    Caption = #26597#25214#29609#23478
  end
  object lbl2: TLabel
    Left = 250
    Top = 500
    Width = 300
    Height = 14
    Caption = #27880#24847#65306#21024#38500#21069#35831#20570#22909#22791#20221#24037#20316#65292#26368#22909#22312#29609#23478#31163#32447#29366#24577#25805#20316
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clRed
    Font.Height = -12
    Font.Name = 'Tahoma'
    Font.Style = []
    ParentFont = False
  end
  object grp1: TGroupBox
    Left = 9
    Top = 6
    Width = 190
    Height = 484
    Caption = #29609#23478#20179#24211#21015#34920
    TabOrder = 0
    object lstUsers: TListBox
      Left = 9
      Top = 17
      Width = 173
      Height = 458
      ItemHeight = 13
      TabOrder = 0
      OnClick = lstUsersClick
    end
  end
  object grp2: TGroupBox
    Left = 207
    Top = 6
    Width = 595
    Height = 484
    Caption = #20179#24211#29289#21697#21015#34920
    TabOrder = 1
    object lvItems: TListView
      Left = 9
      Top = 17
      Width = 578
      Height = 458
      Columns = <
        item
          Caption = #24207#21495
          Width = 54
        end
        item
          Caption = #29289#21697#21517#31216
          Width = 151
        end
        item
          Caption = #29289#21697#32534#21495
          Width = 86
        end
        item
          Caption = #29289#21697'Idx'
          Width = 86
        end
        item
          Caption = #24403#21069#25345#20037
          Width = 86
        end
        item
          Caption = #26368#22823#25345#20037
          Width = 86
        end>
      GridLines = True
      RowSelect = True
      TabOrder = 0
      ViewStyle = vsReport
      OnClick = lvItemsClick
      OnEditing = lvItemsEditing
    end
  end
  object edtUser: TEdit
    Left = 64
    Top = 495
    Width = 95
    Height = 21
    TabOrder = 2
  end
  object btnSearch: TButton
    Left = 164
    Top = 493
    Width = 70
    Height = 27
    Caption = #26597#25214
    TabOrder = 3
    OnClick = btnSearchClick
  end
  object btnDel: TButton
    Left = 650
    Top = 493
    Width = 75
    Height = 27
    Caption = #21024#38500
    TabOrder = 4
    OnClick = btnDelClick
  end
  object btnDelAll: TButton
    Left = 728
    Top = 493
    Width = 74
    Height = 27
    Caption = #20840#37096#21024#38500
    TabOrder = 5
    OnClick = btnDelAllClick
  end
end
