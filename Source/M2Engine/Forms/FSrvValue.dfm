object FrmServerValue: TFrmServerValue
  Left = 318
  Top = 133
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #24615#33021#21442#25968#37197#32622
  ClientHeight = 222
  ClientWidth = 467
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  ShowHint = True
  OnCreate = FormCreate
  TextHeight = 12
  object Label18: TLabel
    Left = 8
    Top = 191
    Width = 312
    Height = 24
    Caption = #35843#25972#30340#21442#25968#31435#21363#29983#25928#65292#22312#32447#26102#35831#30830#35748#27492#21442#25968#30340#20316#29992#20877#35843#25972#65292#13#20081#35843#25972#23558#23548#33268#28216#25103#28151#20081
    Font.Charset = ANSI_CHARSET
    Font.Color = clRed
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object BitBtn1: TBitBtn
    Left = 366
    Top = 190
    Width = 91
    Height = 25
    Caption = #30830#23450'(&O)'
    Kind = bkOK
    NumGlyphs = 2
    TabOrder = 0
    OnClick = BitBtn1Click
  end
  object CbViewHack: TCheckBox
    Left = 152
    Top = 154
    Width = 145
    Height = 17
    Caption = #26174#31034#28216#25103#36895#24230#24322#24120#20449#24687
    Checked = True
    State = cbChecked
    TabOrder = 1
    OnClick = CbViewHackClick
  end
  object CkViewAdmfail: TCheckBox
    Left = 152
    Top = 171
    Width = 121
    Height = 17
    Caption = #26174#31034#38750#27861#30331#24405#20449#24687
    TabOrder = 2
    OnClick = CkViewAdmfailClick
  end
  object GroupBox1: TGroupBox
    Left = 152
    Top = 8
    Width = 169
    Height = 145
    Caption = #32593#20851#25968#25454#20256#36755
    TabOrder = 3
    object Label8: TLabel
      Left = 11
      Top = 16
      Width = 66
      Height = 12
      Caption = #25968#25454#22359#22823#23567':'
    end
    object Label7: TLabel
      Left = 9
      Top = 40
      Width = 66
      Height = 12
      Caption = #33258#26816#25968#25454#22359':'
    end
    object Label9: TLabel
      Left = 7
      Top = 61
      Width = 66
      Height = 12
      Caption = #20445#30041#25968#25454#22359':'
      Enabled = False
    end
    object Label10: TLabel
      Left = 7
      Top = 85
      Width = 54
      Height = 12
      Caption = #36127#36733#27979#35797':'
    end
    object Label11: TLabel
      Left = 145
      Top = 16
      Width = 6
      Height = 12
      Caption = 'B'
    end
    object Label12: TLabel
      Left = 145
      Top = 40
      Width = 6
      Height = 12
      Caption = 'B'
    end
    object Label13: TLabel
      Left = 145
      Top = 64
      Width = 6
      Height = 12
      Caption = 'B'
    end
    object Label14: TLabel
      Left = 145
      Top = 88
      Width = 12
      Height = 12
      Caption = 'KB'
    end
    object EGateLoad: TSpinEditEx
      Left = 78
      Top = 84
      Width = 59
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 0
      Value = 0
      OnChange = EGateLoadChange
    end
    object EAvailableBlock: TSpinEditEx
      Left = 78
      Top = 60
      Width = 59
      Height = 21
      Enabled = False
      MaxValue = 0
      MinValue = 0
      TabOrder = 1
      Value = 0
      OnChange = EAvailableBlockChange
    end
    object ECheckBlock: TSpinEditEx
      Left = 78
      Top = 36
      Width = 59
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 2
      Value = 0
      OnChange = ECheckBlockChange
    end
    object ESendBlock: TSpinEditEx
      Left = 78
      Top = 13
      Width = 59
      Height = 21
      MaxValue = 4194304
      MinValue = 0
      TabOrder = 3
      Value = 0
      OnChange = ESendBlockChange
    end
    object CheckBoxSendCompressDataToRunGate: TCheckBox
      Left = 8
      Top = 119
      Width = 145
      Height = 17
      Caption = #25968#25454#21387#32553#21518#21457#36865#21040#32593#20851
      TabOrder = 4
      OnClick = CheckBoxSendCompressDataToRunGateClick
    end
  end
  object GroupBox2: TGroupBox
    Left = 8
    Top = 8
    Width = 137
    Height = 179
    Caption = #22788#29702#26102#38388#20998#37197'('#27627#31186')'
    TabOrder = 4
    object Label1: TLabel
      Left = 16
      Top = 16
      Width = 54
      Height = 12
      Caption = #20154#29289#22788#29702':'
    end
    object Label2: TLabel
      Left = 16
      Top = 40
      Width = 54
      Height = 12
      Caption = #24618#29289#22788#29702':'
    end
    object Label3: TLabel
      Left = 16
      Top = 64
      Width = 54
      Height = 12
      Caption = #24618#29289#21047#26032':'
    end
    object Label4: TLabel
      Left = 16
      Top = 88
      Width = 54
      Height = 12
      Caption = #25968#25454#20256#36755':'
    end
    object Label5: TLabel
      Left = 16
      Top = 136
      Width = 48
      Height = 12
      Caption = 'NPC'#22788#29702':'
    end
    object Label6: TLabel
      Left = 16
      Top = 112
      Width = 54
      Height = 12
      Caption = #20445#30041#22788#29702':'
      Enabled = False
    end
    object EHum: TSpinEditEx
      Left = 76
      Top = 13
      Width = 47
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 0
      Value = 0
      OnChange = EHumChange
    end
    object EMon: TSpinEditEx
      Left = 76
      Top = 37
      Width = 47
      Height = 21
      Hint = #27599#27425#21047#24618#20998#37197#30340#26102#38388#65292#36234#22823#21333#27425#21047#30340#24618#29289#36234#22810
      MaxValue = 0
      MinValue = 0
      TabOrder = 1
      Value = 0
      OnChange = EMonChange
    end
    object EZen: TSpinEditEx
      Left = 76
      Top = 61
      Width = 47
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 2
      Value = 0
      OnChange = EZenChange
    end
    object ESoc: TSpinEditEx
      Left = 76
      Top = 85
      Width = 47
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 3
      Value = 0
      OnChange = ESocChange
    end
    object ENpc: TSpinEditEx
      Left = 76
      Top = 133
      Width = 47
      Height = 21
      MaxValue = 0
      MinValue = 0
      TabOrder = 4
      Value = 0
      OnChange = ENpcChange
    end
    object EDec: TSpinEditEx
      Left = 76
      Top = 109
      Width = 47
      Height = 21
      Enabled = False
      MaxValue = 0
      MinValue = 0
      TabOrder = 5
      Value = 0
    end
  end
  object GroupBox3: TGroupBox
    Left = 328
    Top = 8
    Width = 129
    Height = 145
    Caption = #24618#29289#22788#29702#25511#21046
    TabOrder = 5
    object Label15: TLabel
      Left = 8
      Top = 16
      Width = 54
      Height = 12
      Caption = #21047#24618#20493#25968':'
    end
    object Label16: TLabel
      Left = 8
      Top = 64
      Width = 54
      Height = 12
      Caption = #22788#29702#38388#38548':'
    end
    object Label17: TLabel
      Left = 8
      Top = 40
      Width = 54
      Height = 12
      Caption = #21047#24618#38388#38548':'
    end
    object Label19: TLabel
      Left = 8
      Top = 88
      Width = 54
      Height = 12
      Caption = #24618#29289#36816#34892':'
    end
    object EditZenMonRate: TSpinEditEx
      Left = 68
      Top = 13
      Width = 47
      Height = 21
      MaxValue = 1000
      MinValue = 0
      TabOrder = 0
      Value = 1
      OnChange = EditZenMonRateChange
    end
    object EditProcessTime: TSpinEditEx
      Left = 68
      Top = 61
      Width = 47
      Height = 21
      Hint = #22788#29702#24618#29289#30340#26102#38388#38388#38548#65292#27492#35774#32622#25968#23383#36234#22823#65292#24618#29289#34892#21160#36234#24930#12290#13#25968#23383#36234#23567#65292#24618#29289#34892#21160#36234#28789#27963#65292'CPU'#21344#29992#36234#39640#12290#40664#35748#20540'250'
      Increment = 10
      MaxValue = 1000
      MinValue = 50
      TabOrder = 1
      Value = 50
      OnChange = EditProcessTimeChange
    end
    object EditZenMonTime: TSpinEditEx
      Left = 68
      Top = 37
      Width = 47
      Height = 21
      Increment = 10
      MaxValue = 1000
      MinValue = 0
      TabOrder = 2
      Value = 1
      OnChange = EditZenMonTimeChange
    end
    object EditProcessMonsterInterval: TSpinEditEx
      Left = 68
      Top = 85
      Width = 47
      Height = 21
      Hint = #31354#38386#26102#22788#29702#24618#29289#26816#27979#27425#25968#65292#25968#23383#36234#22823#24618#29289#36816#34892#36234#36831#38045#12290#40664#35748'=3'
      MaxValue = 10
      MinValue = 0
      TabOrder = 3
      Value = 1
      OnChange = EditProcessMonsterIntervalChange
    end
  end
  object ButtonDefault: TButton
    Left = 368
    Top = 158
    Width = 89
    Height = 25
    Caption = #40664#35748#35774#32622
    TabOrder = 6
    OnClick = ButtonDefaultClick
  end
end
