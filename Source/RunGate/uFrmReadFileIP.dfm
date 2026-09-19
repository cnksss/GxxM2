object FrmReadFileIP: TFrmReadFileIP
  Left = 563
  Top = 310
  BorderStyle = bsDialog
  Caption = #38450#24481#35774#32622
  ClientHeight = 284
  ClientWidth = 554
  Color = clBtnFace
  Font.Charset = ANSI_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 12
  object grp1: TGroupBox
    Left = 7
    Top = 7
    Width = 538
    Height = 74
    Caption = #36807#28388#21015#34920
    TabOrder = 0
    object lbl1: TLabel
      Left = 8
      Top = 21
      Width = 90
      Height = 12
      Caption = #35835#21462#26412#22320'IP'#21015#34920':'
    end
    object lbl2: TLabel
      Left = 386
      Top = 21
      Width = 54
      Height = 12
      Caption = #35835#21462#38388#38548':'
    end
    object Label1: TLabel
      Left = 516
      Top = 21
      Width = 12
      Height = 12
      Caption = #31186
    end
    object lbl3: TLabel
      Left = 8
      Top = 48
      Width = 90
      Height = 12
      Caption = #19979#36733#36828#31243'IP'#22320#22336':'
    end
    object Label8: TLabel
      Left = 386
      Top = 48
      Width = 54
      Height = 12
      Caption = #19979#36733#38388#38548':'
    end
    object Label9: TLabel
      Left = 516
      Top = 48
      Width = 12
      Height = 12
      Caption = #20998
    end
    object edtFYReadDenyIPFile: TEdit
      Left = 100
      Top = 17
      Width = 280
      Height = 20
      TabOrder = 0
    end
    object seFYReadDenyIPTime: TSpinEditEx
      Left = 442
      Top = 16
      Width = 72
      Height = 21
      Increment = 10
      MaxValue = 600000000
      MinValue = 60
      TabOrder = 1
      Value = 60
    end
    object edtFYDownDenyIPUrl: TEdit
      Left = 100
      Top = 44
      Width = 280
      Height = 20
      Hint = #35831#22635#20889#20934#30830#30340#36828#31243#21015#34920#22320#22336#65292#20445#35777#32593#31449#36890#35759#27491#24120#13#10#22914#26524#32593#22336#20026#31354#65292#21017#20851#38381#36828#31243#19979#36733
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
    end
    object seFYDownDenyIPTime: TSpinEditEx
      Left = 442
      Top = 43
      Width = 72
      Height = 21
      Increment = 10
      MaxValue = 120
      MinValue = 2
      TabOrder = 3
      Value = 60
    end
  end
  object GroupBox1: TGroupBox
    Left = 7
    Top = 92
    Width = 538
    Height = 74
    Caption = #32511#33394#36890#36947
    TabOrder = 1
    object Label2: TLabel
      Left = 8
      Top = 21
      Width = 90
      Height = 12
      Caption = #35835#21462#26412#22320'IP'#21015#34920':'
    end
    object Label3: TLabel
      Left = 386
      Top = 21
      Width = 54
      Height = 12
      Caption = #35835#21462#38388#38548':'
    end
    object Label4: TLabel
      Left = 516
      Top = 21
      Width = 12
      Height = 12
      Caption = #31186
    end
    object Label10: TLabel
      Left = 8
      Top = 48
      Width = 90
      Height = 12
      Caption = #19979#36733#36828#31243'IP'#22320#22336':'
    end
    object Label11: TLabel
      Left = 386
      Top = 48
      Width = 54
      Height = 12
      Caption = #19979#36733#38388#38548':'
    end
    object Label12: TLabel
      Left = 516
      Top = 48
      Width = 12
      Height = 12
      Caption = #20998
    end
    object edtFYReadPassIPFile: TEdit
      Left = 100
      Top = 17
      Width = 280
      Height = 20
      TabOrder = 0
    end
    object seFYReadPassIPTime: TSpinEditEx
      Left = 441
      Top = 16
      Width = 72
      Height = 21
      Increment = 10
      MaxValue = 600000000
      MinValue = 60
      TabOrder = 1
      Value = 60
    end
    object edtFYDownPassIPUrl: TEdit
      Left = 100
      Top = 44
      Width = 280
      Height = 20
      Hint = #35831#22635#20889#20934#30830#30340#36828#31243#21015#34920#22320#22336#65292#20445#35777#32593#31449#36890#35759#27491#24120#13#10#22914#26524#32593#22336#20026#31354#65292#21017#20851#38381#36828#31243#19979#36733
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
    end
    object seFYDownPassIPTime: TSpinEditEx
      Left = 442
      Top = 43
      Width = 72
      Height = 21
      Increment = 10
      MaxValue = 120
      MinValue = 2
      TabOrder = 3
      Value = 60
    end
  end
  object btnOK: TButton
    Left = 469
    Top = 254
    Width = 75
    Height = 25
    Caption = #30830#23450'(&O)'
    TabOrder = 2
    OnClick = btnOKClick
  end
  object GroupBox2: TGroupBox
    Left = 7
    Top = 175
    Width = 538
    Height = 74
    Caption = #26426#22120#30721#36807#28388#21015#34920' '#65288#24341#25806#37197#32622#65289
    TabOrder = 3
    object Label5: TLabel
      Left = 8
      Top = 21
      Width = 90
      Height = 12
      Caption = #26412#22320#26426#22120#30721#21015#34920':'
    end
    object Label6: TLabel
      Left = 386
      Top = 21
      Width = 54
      Height = 12
      Caption = #35835#21462#38388#38548':'
    end
    object Label7: TLabel
      Left = 516
      Top = 21
      Width = 12
      Height = 12
      Caption = #31186
    end
    object Label13: TLabel
      Left = 8
      Top = 48
      Width = 90
      Height = 12
      Caption = #19979#36733#36828#31243#26426#22120#30721':'
    end
    object Label14: TLabel
      Left = 386
      Top = 48
      Width = 54
      Height = 12
      Caption = #19979#36733#38388#38548':'
    end
    object Label15: TLabel
      Left = 516
      Top = 48
      Width = 12
      Height = 12
      Caption = #20998
    end
    object edtFYReadDenyMACFile: TEdit
      Left = 100
      Top = 17
      Width = 280
      Height = 20
      TabOrder = 0
    end
    object seFYReadDenyMACTime: TSpinEditEx
      Left = 442
      Top = 17
      Width = 72
      Height = 21
      Increment = 10
      MaxValue = 600000000
      MinValue = 60
      TabOrder = 1
      Value = 60
    end
    object edtFYDownDenyMACUrl: TEdit
      Left = 100
      Top = 44
      Width = 280
      Height = 20
      Hint = #35831#22635#20889#20934#30830#30340#36828#31243#21015#34920#22320#22336#65292#20445#35777#32593#31449#36890#35759#27491#24120#13#10#22914#26524#32593#22336#20026#31354#65292#21017#20851#38381#36828#31243#19979#36733
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
    end
    object seFYDownDenyMACTime: TSpinEditEx
      Left = 442
      Top = 43
      Width = 72
      Height = 21
      Increment = 10
      MaxValue = 120
      MinValue = 2
      TabOrder = 3
      Value = 60
    end
  end
  object chkOnlyWhiteListLink: TCheckBox
    Left = 8
    Top = 258
    Width = 201
    Height = 17
    Caption = #21482#20801#35768#32511#33394#36890#36947#65288#30333#21517#21333#65289#36830#25509
    TabOrder = 4
  end
end
