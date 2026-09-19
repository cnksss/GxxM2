object frmQRcode: TfrmQRcode
  Left = 758
  Top = 299
  BorderStyle = bsDialog
  Caption = 'QRcode'
  ClientHeight = 277
  ClientWidth = 452
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clBlack
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poDesktopCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 12
  object lbl1: TLabel
    Left = 29
    Top = 16
    Width = 36
    Height = 12
    Caption = 'margin'
  end
  object lbl2: TLabel
    Left = 41
    Top = 48
    Width = 24
    Height = 12
    Caption = 'size'
  end
  object lbl3: TLabel
    Left = 167
    Top = 16
    Width = 30
    Height = 12
    Caption = 'level'
  end
  object lbl4: TLabel
    Left = 148
    Top = 80
    Width = 48
    Height = 12
    Caption = 'casesens'
  end
  object lbl5: TLabel
    Left = 5
    Top = 80
    Width = 60
    Height = 12
    Caption = 'structured'
  end
  object lbl6: TLabel
    Left = 149
    Top = 48
    Width = 48
    Height = 12
    Caption = 'eightbit'
  end
  object lbl7: TLabel
    Left = 285
    Top = 48
    Width = 60
    Height = 12
    Caption = 'background'
  end
  object lbl8: TLabel
    Left = 285
    Top = 16
    Width = 60
    Height = 12
    Caption = 'foreground'
  end
  object lbl9: TLabel
    Left = 30
    Top = 112
    Width = 36
    Height = 12
    Caption = 'output'
  end
  object lbl10: TLabel
    Left = 321
    Top = 80
    Width = 24
    Height = 12
    Caption = 'code'
  end
  object btnGen: TButton
    Left = 392
    Top = 106
    Width = 48
    Height = 25
    Caption = #29983#25104
    TabOrder = 8
    OnClick = btnGenClick
  end
  object mmoContent: TMemo
    Left = 8
    Top = 136
    Width = 433
    Height = 129
    ScrollBars = ssVertical
    TabOrder = 10
  end
  object edtMargin: TEdit
    Left = 69
    Top = 12
    Width = 73
    Height = 20
    BiDiMode = bdLeftToRight
    ParentBiDiMode = False
    TabOrder = 1
    Text = '2'
  end
  object edtSize: TEdit
    Left = 69
    Top = 44
    Width = 73
    Height = 20
    TabOrder = 4
    Text = '3'
  end
  object cbbLevel: TComboBox
    Left = 203
    Top = 12
    Width = 73
    Height = 20
    Style = csDropDownList
    ItemHeight = 12
    TabOrder = 2
    Items.Strings = (
      'L(Lowest)'
      'M'
      'Q'
      'H(Highest)')
  end
  object cbbCasesens: TComboBox
    Left = 203
    Top = 76
    Width = 73
    Height = 20
    Style = csDropDownList
    ItemHeight = 12
    TabOrder = 7
    Items.Strings = (
      '0'
      '1')
  end
  object cbbStructured: TComboBox
    Left = 69
    Top = 76
    Width = 73
    Height = 20
    Style = csDropDownList
    ItemHeight = 12
    TabOrder = 6
    Items.Strings = (
      '0'
      '1')
  end
  object cbbEightBit: TComboBox
    Left = 203
    Top = 44
    Width = 73
    Height = 20
    Style = csDropDownList
    ItemHeight = 12
    TabOrder = 5
    Items.Strings = (
      '0'
      '1')
  end
  object clrbxFore: TColorBox
    Left = 352
    Top = 11
    Width = 89
    Height = 22
    ItemHeight = 16
    TabOrder = 0
  end
  object clrbxBack: TColorBox
    Left = 352
    Top = 43
    Width = 89
    Height = 22
    ItemHeight = 16
    TabOrder = 3
  end
  object edtOutput: TEdit
    Left = 69
    Top = 108
    Width = 308
    Height = 20
    BiDiMode = bdLeftToRight
    ParentBiDiMode = False
    TabOrder = 9
  end
  object cbbCode: TComboBox
    Left = 352
    Top = 76
    Width = 89
    Height = 20
    Style = csDropDownList
    ItemHeight = 12
    TabOrder = 11
    Items.Strings = (
      'UTF8'
      'GB2312')
  end
end
