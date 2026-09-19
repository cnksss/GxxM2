object FrmHumanExport: TFrmHumanExport
  Left = 752
  Top = 432
  BorderStyle = bsDialog
  Caption = #23548#20986#20154#29289#25968#25454
  ClientHeight = 161
  ClientWidth = 299
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object GroupBox1: TGroupBox
    Left = 11
    Top = 8
    Width = 278
    Height = 73
    Caption = #23548#20986#31163#32447#25346#26426#20154#29289
    TabOrder = 0
    object Label1: TLabel
      Left = 12
      Top = 20
      Width = 84
      Height = 13
      Caption = #38480#21046#23548#20986#25968#37327#65306
    end
    object Label2: TLabel
      Left = 12
      Top = 44
      Width = 84
      Height = 13
      Caption = #26368#20302#20154#29289#31561#32423#65306
    end
    object seLimitCount: TSpinEdit
      Left = 94
      Top = 16
      Width = 88
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 0
      Value = 100
    end
    object seMinLevel: TSpinEdit
      Left = 94
      Top = 40
      Width = 88
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 1
      Value = 20
    end
    object btnHumanExport: TButton
      Left = 193
      Top = 14
      Width = 75
      Height = 25
      Caption = #23548#20986
      TabOrder = 2
      OnClick = btnHumanExportClick
    end
  end
  object GroupBox2: TGroupBox
    Left = 11
    Top = 88
    Width = 278
    Height = 65
    Caption = #23548#20986#20154#29289#25163#26426#21495
    TabOrder = 1
    object btnMobileNumberExport: TButton
      Left = 193
      Top = 17
      Width = 75
      Height = 25
      Caption = #23548#20986
      TabOrder = 0
      OnClick = btnMobileNumberExportClick
    end
    object rbAllMobile: TRadioButton
      Left = 8
      Top = 21
      Width = 113
      Height = 17
      Caption = #23548#20986#25152#26377#25163#26426#21495
      TabOrder = 1
    end
    object rbBindMobile: TRadioButton
      Left = 8
      Top = 42
      Width = 137
      Height = 17
      Hint = #32465#23450#30340#25163#26426#21495#26159#32463#36807#39564#35777#30721#39564#35777#30340
      Caption = #23548#20986#24050#32465#23450#30340#25163#26426#21495
      Checked = True
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      TabStop = True
    end
  end
end
