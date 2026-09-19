object FrmUserShopGetMoneyTotal: TFrmUserShopGetMoneyTotal
  Left = 342
  Top = 262
  BorderStyle = bsDialog
  BorderWidth = 6
  Caption = #20010#20154#21830#24215#24453#21462#22238#36135#24065#32479#35745
  ClientHeight = 479
  ClientWidth = 862
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 14
  object lvMoney: TListView
    Left = 0
    Top = 0
    Width = 862
    Height = 450
    Align = alClient
    Columns = <
      item
        Caption = #21517#31216
        Width = 215
      end
      item
        Width = 129
      end
      item
        Width = 129
      end
      item
        Width = 129
      end
      item
        Width = 129
      end
      item
        Width = 129
      end>
    RowSelect = True
    PopupMenu = pm1
    TabOrder = 0
    ViewStyle = vsReport
  end
  object Panel1: TPanel
    Left = 0
    Top = 450
    Width = 862
    Height = 29
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 1
    object lbl1: TLabel
      Left = 105
      Top = 9
      Width = 36
      Height = 13
      Alignment = taRightJustify
      Caption = #20803#23453#65306
    end
    object lbl211: TLabel
      Left = 9
      Top = 9
      Width = 48
      Height = 13
      Caption = #32479#35745#25968#25454
    end
    object lbl2: TLabel
      Left = 254
      Top = 9
      Width = 48
      Height = 13
      Alignment = taRightJustify
      Caption = #28216#25103#28857#65306
    end
    object lbl3: TLabel
      Left = 421
      Top = 9
      Width = 36
      Height = 13
      Alignment = taRightJustify
      Caption = #37329#24065#65306
    end
    object lbl4: TLabel
      Left = 569
      Top = 9
      Width = 48
      Height = 13
      Alignment = taRightJustify
      Caption = #37329#21018#30707#65306
    end
    object lbl5: TLabel
      Left = 740
      Top = 9
      Width = 36
      Height = 13
      Alignment = taRightJustify
      Caption = #28789#31526#65306
    end
    object edt1: TEdit
      Left = 138
      Top = 4
      Width = 90
      Height = 21
      ReadOnly = True
      TabOrder = 0
      Text = 'edt1'
    end
    object edt2: TEdit
      Left = 296
      Top = 4
      Width = 91
      Height = 21
      ReadOnly = True
      TabOrder = 1
      Text = 'edt1'
    end
    object edt3: TEdit
      Left = 454
      Top = 4
      Width = 91
      Height = 21
      ReadOnly = True
      TabOrder = 2
      Text = 'edt1'
    end
    object edt4: TEdit
      Left = 613
      Top = 4
      Width = 90
      Height = 21
      ReadOnly = True
      TabOrder = 3
      Text = 'edt1'
    end
    object edt5: TEdit
      Left = 771
      Top = 4
      Width = 91
      Height = 21
      ReadOnly = True
      TabOrder = 4
      Text = 'edt1'
    end
  end
  object pm1: TPopupMenu
    Left = 392
    Top = 176
    object mniN1: TMenuItem
      Caption = #21047#26032#25968#25454
      OnClick = mniN1Click
    end
  end
end
