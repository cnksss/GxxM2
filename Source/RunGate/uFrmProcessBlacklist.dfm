object FrmProcessBlacklist: TFrmProcessBlacklist
  Left = 350
  Top = 258
  BorderStyle = bsDialog
  Caption = #36827#31243#40657#21517#21333
  ClientHeight = 441
  ClientWidth = 622
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -13
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object lbl1: TLabel
    Left = 7
    Top = 10
    Width = 65
    Height = 13
    Caption = #25628#32034#23383#27573#65306
  end
  object lbl2: TLabel
    Left = 143
    Top = 10
    Width = 65
    Height = 13
    Caption = #25628#32034#20869#23481#65306
  end
  object lbl3: TLabel
    Left = 5
    Top = 422
    Width = 183
    Height = 13
    Caption = #35828#26126#65306#36827#31243#21015#34920#26368#22810#21482#25903#25345'80'#20010
  end
  object lvProcessBlacklist: TListView
    Left = 6
    Top = 32
    Width = 610
    Height = 385
    Columns = <
      item
        Caption = #24207#21495
      end
      item
        Caption = #36827#31243#21517
        Width = 200
      end
      item
        Caption = 'MD5'
        Width = 330
      end>
    ColumnClick = False
    GridLines = True
    ReadOnly = True
    RowSelect = True
    PopupMenu = pmDelete
    TabOrder = 4
    ViewStyle = vsReport
  end
  object cbbSearchField: TComboBox
    Left = 68
    Top = 6
    Width = 65
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    ItemIndex = 0
    TabOrder = 0
    Text = #36827#31243
    Items.Strings = (
      #36827#31243
      'MD5')
  end
  object edtSearchText: TEdit
    Left = 204
    Top = 6
    Width = 181
    Height = 21
    TabOrder = 1
    OnKeyDown = edtSearchTextKeyDown
  end
  object btnSearch: TButton
    Left = 388
    Top = 5
    Width = 53
    Height = 22
    Caption = #25628#32034
    TabOrder = 2
    OnClick = btnSearchClick
  end
  object btnSearchNext: TButton
    Left = 446
    Top = 5
    Width = 71
    Height = 22
    Caption = #25628#32034#19979#19968#20010
    TabOrder = 3
    OnClick = btnSearchNextClick
  end
  object btnAdd: TButton
    Left = 545
    Top = 5
    Width = 71
    Height = 22
    Caption = #28155#21152
    TabOrder = 5
    OnClick = btnAddClick
  end
  object pmDelete: TPopupMenu
    OnPopup = pmDeletePopup
    Left = 152
    Top = 200
    object mniDelete: TMenuItem
      Caption = #21024#38500#36827#31243
      OnClick = mniDeleteClick
    end
  end
end
