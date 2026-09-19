object FrmSafeFilter: TFrmSafeFilter
  Left = 361
  Top = 219
  BorderStyle = bsDialog
  Caption = #32593#32476#23433#20840#36807#28388
  ClientHeight = 620
  ClientWidth = 875
  Color = clBtnFace
  Font.Charset = ANSI_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  DesignSize = (
    875
    620)
  PixelsPerInch = 96
  TextHeight = 12
  object Label7: TLabel
    Left = 640
    Top = 595
    Width = 120
    Height = 12
    Anchors = [akLeft, akBottom]
    Caption = #20197#19978#21442#25968#35843#21518#31435#21363#29983#25928
    Font.Charset = ANSI_CHARSET
    Font.Color = clRed
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object grp3: TGroupBox
    Left = 505
    Top = 7
    Width = 359
    Height = 204
    Anchors = [akLeft, akTop, akBottom]
    Caption = 'Mac'#22320#22336#36807#28388
    TabOrder = 2
    DesignSize = (
      359
      204)
    object lbl1: TLabel
      Left = 9
      Top = 15
      Width = 60
      Height = 12
      Caption = #21160#24577#36807#28388#65306
    end
    object Label5: TLabel
      Left = 9
      Top = 108
      Width = 60
      Height = 12
      Anchors = [akLeft, akBottom]
      Caption = #27704#20037#36807#28388#65306
    end
    object lstTempMac: TListBox
      Left = 7
      Top = 29
      Width = 344
      Height = 78
      Hint = #21160#24577'MAC'#36807#28388#21015#34920#65292#22312#27492#21015#34920#20013#30340'MAC'#23558#26080#27861#24314#31435#36830#25509#65292#20294#22312#31243#24207#37325#26032#21551#21160#26102#27492#21015#34920#30340#20449#24687#23558#34987#28165#31354
      Anchors = [akLeft, akTop, akRight, akBottom]
      ItemHeight = 12
      Items.Strings = (
        '888.888.888.888')
      ParentShowHint = False
      PopupMenu = pmTempMac
      ShowHint = True
      Sorted = True
      TabOrder = 0
      OnKeyDown = lstActiveKeyDown
    end
    object lstBlockMac: TListBox
      Left = 7
      Top = 123
      Width = 344
      Height = 73
      Hint = #27704#20037'MAC'#36807#28388#21015#34920#65292#22312#27492#21015#34920#20013#30340'MAC'#23558#26080#27861#24314#31435#36830#25509#65292#31243#24207#37325#26032#21518#20250#37325#26032#21152#36733#20445#23384#30340#21015#34920
      Anchors = [akLeft, akBottom]
      ItemHeight = 12
      Items.Strings = (
        '888.888.888.888')
      ParentShowHint = False
      PopupMenu = pmBlockMac
      ShowHint = True
      Sorted = True
      TabOrder = 1
      OnKeyDown = lstActiveKeyDown
    end
  end
  object GroupBox1: TGroupBox
    Left = 256
    Top = 8
    Width = 240
    Height = 575
    Anchors = [akLeft, akTop, akBottom]
    Caption = 'IP'#22320#22336#36807#28388
    TabOrder = 1
    DesignSize = (
      240
      575)
    object LabelTempList: TLabel
      Left = 7
      Top = 17
      Width = 54
      Height = 12
      Caption = #21160#24577#36807#28388':'
    end
    object Label1: TLabel
      Left = 122
      Top = 17
      Width = 54
      Height = 12
      Caption = #27704#20037#36807#28388':'
    end
    object Label23: TLabel
      Left = 7
      Top = 331
      Width = 60
      Height = 12
      Anchors = [akLeft, akBottom]
      Caption = #36807#28388'IP'#27573': '
    end
    object lstTemp: TListBox
      Left = 7
      Top = 32
      Width = 110
      Height = 295
      Hint = #21160#24577#36807#28388#21015#34920#65292#22312#27492#21015#34920#20013#30340'IP'#23558#26080#27861#24314#31435#36830#25509#65292#20294#22312#31243#24207#37325#26032#21551#21160#26102#27492#21015#34920#30340#20449#24687#23558#34987#28165#31354
      Anchors = [akLeft, akTop, akBottom]
      ItemHeight = 12
      Items.Strings = (
        '888.888.888.888')
      ParentShowHint = False
      PopupMenu = pmTemp
      ShowHint = True
      Sorted = True
      TabOrder = 0
      OnKeyDown = lstActiveKeyDown
    end
    object lstBlock: TListBox
      Left = 122
      Top = 32
      Width = 110
      Height = 295
      Hint = #27704#20037#36807#28388#21015#34920#65292#22312#27492#21015#34920#20013#30340'IP'#23558#26080#27861#24314#31435#36830#25509#65292#27492#21015#34920#23558#20445#23384#20110#37197#32622#25991#20214#20013#65292#22312#31243#24207#37325#26032#21551#21160#26102#20250#37325#26032#21152#36733#27492#21015#34920
      Anchors = [akLeft, akTop, akBottom]
      ItemHeight = 12
      Items.Strings = (
        '888.888.888.888')
      ParentShowHint = False
      PopupMenu = pmBlock
      ShowHint = True
      Sorted = True
      TabOrder = 1
      OnKeyDown = lstActiveKeyDown
    end
    object lstIpSection: TListBox
      Left = 7
      Top = 347
      Width = 225
      Height = 220
      Anchors = [akLeft, akBottom]
      ItemHeight = 12
      Items.Strings = (
        '888.888.888.888')
      ParentShowHint = False
      PopupMenu = pmIpSection
      ShowHint = True
      Sorted = True
      TabOrder = 2
    end
  end
  object grp2: TGroupBox
    Left = 663
    Top = 256
    Width = 201
    Height = 107
    Anchors = [akLeft, akBottom]
    Caption = #36830#25509#20445#25252
    TabOrder = 5
    object Label2: TLabel
      Left = 21
      Top = 18
      Width = 54
      Height = 12
      Caption = #36830#25509#38480#21046':'
    end
    object Label3: TLabel
      Left = 139
      Top = 18
      Width = 42
      Height = 12
      Caption = #36830#25509'/IP'
    end
    object Label9: TLabel
      Left = 21
      Top = 40
      Width = 54
      Height = 12
      Caption = #36830#25509#36229#26102':'
    end
    object Label10: TLabel
      Left = 139
      Top = 40
      Width = 12
      Height = 12
      Caption = #31186
    end
    object Label22: TLabel
      Left = 70
      Top = 62
      Width = 66
      Height = 12
      Caption = #27627#31186'/'#36830#25509#25968
    end
    object Label24: TLabel
      Left = 70
      Top = 85
      Width = 66
      Height = 12
      Caption = #27627#31186'/'#36830#25509#25968
    end
    object seMaxConnect: TSpinEdit
      Left = 76
      Top = 13
      Width = 61
      Height = 21
      Hint = #21333#20010'IP'#22320#22336#65292#26368#22810#21487#20197#24314#31435#36830#25509#25968#65292#36229#36807#25351#23450#36830#25509#25968#23558#25353#19979#38754#30340#25805#20316#22788#29702
      MaxValue = 2000
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      Value = 50
    end
    object seKeepConnectTimeOut: TSpinEdit
      Left = 76
      Top = 35
      Width = 61
      Height = 21
      MaxValue = 60
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      Value = 5
    end
    object seIPCountLimit1: TSpinEditEx
      Left = 138
      Top = 58
      Width = 55
      Height = 21
      MaxValue = 255
      MinValue = 1
      TabOrder = 3
      Value = 1
    end
    object seIPCountLimit2: TSpinEditEx
      Left = 138
      Top = 81
      Width = 55
      Height = 21
      MaxValue = 255
      MinValue = 1
      TabOrder = 5
      Value = 1
    end
    object seIPCountLimitTime1: TSpinEditEx
      Left = 8
      Top = 58
      Width = 60
      Height = 21
      Increment = 100
      MaxValue = 600000
      MinValue = 1
      TabOrder = 2
      Value = 1
    end
    object seIPCountLimitTime2: TSpinEditEx
      Left = 8
      Top = 81
      Width = 60
      Height = 21
      Increment = 100
      MaxValue = 600000
      MinValue = 1
      TabOrder = 4
      Value = 1
    end
  end
  object GroupBox3: TGroupBox
    Left = 505
    Top = 216
    Width = 359
    Height = 37
    Anchors = [akLeft, akBottom]
    Caption = #25915#20987#25805#20316
    TabOrder = 3
    object rbAddBlockList: TRadioButton
      Left = 232
      Top = 14
      Width = 119
      Height = 17
      Hint = #23558#27492#36830#25509#30340'IP'#21152#20837#27704#20037#36807#28388#21015#34920#65292#24182#23558#27492'IP'#30340#25152#26377#36830#25509#24378#34892#20013#26029
      Caption = #21152#20837#27704#20037#36807#28388#21015#34920
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
    end
    object rbAddTempList: TRadioButton
      Left = 98
      Top = 14
      Width = 121
      Height = 17
      Hint = #23558#27492#36830#25509#30340'IP'#21152#20837#21160#24577#36807#28388#21015#34920#65292#24182#23558#27492'IP'#30340#25152#26377#36830#25509#24378#34892#20013#26029
      Caption = #21152#20837#21160#24577#36807#28388#21015#34920
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
    end
    object rbDisConnect: TRadioButton
      Left = 8
      Top = 14
      Width = 73
      Height = 17
      Hint = #23558#36830#25509#31616#21333#30340#26029#24320#22788#29702
      Caption = #26029#24320#36830#25509
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
    end
  end
  object GroupBox4: TGroupBox
    Left = 505
    Top = 256
    Width = 151
    Height = 82
    Anchors = [akLeft, akBottom]
    Caption = #27969#37327#25511#21046
    TabOrder = 4
    object Label6: TLabel
      Left = 13
      Top = 18
      Width = 54
      Height = 12
      Caption = #26368#22823#38480#21046':'
    end
    object Label8: TLabel
      Left = 13
      Top = 40
      Width = 54
      Height = 12
      Caption = #25968#37327#38480#21046':'
    end
    object seMaxClientPacketSize: TSpinEdit
      Left = 69
      Top = 13
      Width = 76
      Height = 21
      Hint = 
        #25910#21040#30340#23458#25143#31471#19968#20010#23436#25972#21253#30340#26368#22823#38480#21046#65292#22914#26524#36229#36807#27492#22823#23567#65292#21017#34987#35270#20026#25915#20987#12290#13#10#13#10#35813#36873#39033#19981#21253#21547#21047#26032#23458#25143#31471#36827#31243#12289#28216#25103#24555#29031#21450#26700#38754#24555#29031#21151#33021#65292#21487#20197 +
        #36866#24403#36873#25321#19968#20010#36739#23567#30340#20540#12290
      Increment = 10
      MaxValue = 512
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      Value = 512
    end
    object chkLostLine: TCheckBox
      Left = 12
      Top = 60
      Width = 117
      Height = 17
      Hint = #25171#24320#27492#21151#33021#21518#65292#22914#26524#23458#25143#31471#30340#21457#36865#30340#25968#25454#36229#36807#25351#23450#38480#21046#23558#20250#30452#25509#23558#20854#25481#32447
      BiDiMode = bdLeftToRight
      Caption = #27969#37327#24322#24120#25481#32447#22788#29702
      ParentBiDiMode = False
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
    end
    object seMaxClientPacketCount: TSpinEdit
      Left = 69
      Top = 35
      Width = 76
      Height = 21
      Hint = #19968#27425#24615#25509#21463#21040#25968#25454#20449#24687#21253#30340#25968#37327#65292#36229#36807#25351#23450#25968#37327#21017#35270#20026#25915#20987#65281
      MaxValue = 200
      MinValue = 3
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      Value = 200
    end
  end
  object btnOK: TButton
    Left = 782
    Top = 589
    Width = 82
    Height = 25
    Anchors = [akLeft, akBottom]
    Caption = #30830#23450'(&O)'
    Default = True
    TabOrder = 11
    OnClick = btnOKClick
  end
  object GroupBox2: TGroupBox
    Left = 505
    Top = 342
    Width = 151
    Height = 63
    Anchors = [akLeft, akBottom]
    Caption = #38450'CC'#22788#29702
    TabOrder = 6
    object Label11: TLabel
      Left = 10
      Top = 18
      Width = 78
      Height = 12
      Caption = #38450'CC'#25915#20987#26102#38388':'
    end
    object Label12: TLabel
      Left = 10
      Top = 41
      Width = 78
      Height = 12
      Caption = 'CC'#25915#20987#20020#30028#25968':'
    end
    object seAttackTick: TSpinEdit
      Left = 91
      Top = 14
      Width = 54
      Height = 21
      Increment = 10
      MaxValue = 6000
      MinValue = 100
      TabOrder = 0
      Value = 200
    end
    object seAttackCount: TSpinEdit
      Left = 91
      Top = 36
      Width = 54
      Height = 21
      MaxValue = 100
      MinValue = 1
      TabOrder = 1
      Value = 10
    end
  end
  object GroupBox6: TGroupBox
    Left = 663
    Top = 409
    Width = 201
    Height = 130
    Anchors = [akLeft, akBottom]
    Caption = #38450#24481#35774#32622
    TabOrder = 9
    object Label25: TLabel
      Left = 8
      Top = 18
      Width = 54
      Height = 12
      Caption = #38450#24481#31561#32423':'
    end
    object trckbrDefenseLevel: TTrackBar
      Left = 64
      Top = 10
      Width = 133
      Height = 26
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      OnChange = trckbrDefenseLevelChange
    end
    object chkDefenseToLevel1: TCheckBox
      Left = 8
      Top = 40
      Width = 129
      Height = 17
      Caption = #21463#25915#20987#38450#24481#35843#20026'1'#32423
      ParentShowHint = False
      ShowHint = True
      TabOrder = 1
      OnClick = trckbrDefenseLevelChange
    end
    object chkAutoClearTemp: TCheckBox
      Left = 8
      Top = 84
      Width = 121
      Height = 17
      Caption = #28165#38500#21160#24577#36807#28388#21015#34920
      ParentShowHint = False
      ShowHint = True
      TabOrder = 5
      OnClick = trckbrDefenseLevelChange
    end
    object seAutoClearTemp: TSpinEditEx
      Left = 136
      Top = 82
      Width = 56
      Height = 21
      Hint = #27599#38548#19968#27573#26102#38388#65292#33258#21160#28165#38500#21160#24577#21015#34920#25968#25454' '#65288#21333#20301#65306#31186#65289
      MaxValue = 10000
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 6
      Value = 1
      OnChange = trckbrDefenseLevelChange
    end
    object chkResotreDefense: TCheckBox
      Left = 8
      Top = 62
      Width = 129
      Height = 17
      Caption = #26080#25915#20987#36824#21407#38450#24481#31561#32423
      ParentShowHint = False
      ShowHint = True
      TabOrder = 3
      OnClick = trckbrDefenseLevelChange
    end
    object seResotreDefense: TSpinEditEx
      Left = 136
      Top = 60
      Width = 56
      Height = 21
      Hint = #24403#27809#26377#21463#21040#25915#20987#26102#65292#31561#24453#25351#23450#30340#26102#38388#21518#23558#38450#24481#31561#32423#35843#25972#20026#35774#23450#30340#31561#32423
      MaxValue = 10000
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 4
      Value = 1
      OnChange = trckbrDefenseLevelChange
    end
    object seDefenseToLevel1: TSpinEditEx
      Left = 136
      Top = 38
      Width = 56
      Height = 21
      Hint = #24403#21463#21040#35774#23450#30340#25915#20987#27425#25968#21518#65292#38450#24481#31561#32423#19981#20026'1'#32423#26102#65292#31243#24207#33258#21160#35843#25972#20026'1'#32423
      MaxValue = 1000
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 2
      Value = 1
      OnChange = trckbrDefenseLevelChange
    end
    object chkAddAllToTemp: TCheckBox
      Left = 8
      Top = 106
      Width = 129
      Height = 17
      Caption = #36830#25509#21152#20837#21040#21160#24577#36807#28388
      ParentShowHint = False
      ShowHint = True
      TabOrder = 7
      OnClick = trckbrDefenseLevelChange
    end
    object seAddAllToTemp: TSpinEditEx
      Left = 136
      Top = 104
      Width = 56
      Height = 21
      MaxValue = 10000
      MinValue = 1
      ParentShowHint = False
      ShowHint = True
      TabOrder = 8
      Value = 1
      OnChange = trckbrDefenseLevelChange
    end
  end
  object GroupBox5: TGroupBox
    Left = 505
    Top = 409
    Width = 151
    Height = 130
    Anchors = [akLeft, akBottom]
    Caption = #21457#35328#35774#32622
    TabOrder = 8
    object Label13: TLabel
      Left = 120
      Top = 107
      Width = 12
      Height = 12
      Caption = #31186
    end
    object Label14: TLabel
      Left = 120
      Top = 85
      Width = 12
      Height = 12
      Caption = #27425
    end
    object Label15: TLabel
      Left = 120
      Top = 63
      Width = 24
      Height = 12
      Caption = #27627#31186
    end
    object Label16: TLabel
      Left = 8
      Top = 63
      Width = 54
      Height = 12
      Caption = #26102#38388#38388#38548':'
    end
    object Label17: TLabel
      Left = 8
      Top = 41
      Width = 54
      Height = 12
      Caption = #25991#23383#38271#24230':'
    end
    object Label18: TLabel
      Left = 8
      Top = 85
      Width = 54
      Height = 12
      Caption = #21457#35328#27425#25968':'
    end
    object Label19: TLabel
      Left = 8
      Top = 107
      Width = 54
      Height = 12
      Caption = #31105#35328#26102#38388':'
    end
    object seSayMaxLen: TSpinEdit
      Left = 64
      Top = 36
      Width = 55
      Height = 21
      MaxLength = 2
      MaxValue = 99
      MinValue = 0
      TabOrder = 1
      Value = 0
    end
    object seSayTime: TSpinEdit
      Left = 64
      Top = 58
      Width = 55
      Height = 21
      Increment = 1000
      MaxValue = 99999
      MinValue = 1
      TabOrder = 2
      Value = 3000
    end
    object seSayMaxCount: TSpinEdit
      Left = 64
      Top = 80
      Width = 55
      Height = 21
      MaxValue = 100
      MinValue = 1
      TabOrder = 3
      Value = 1
    end
    object seSayDisableTime: TSpinEdit
      Left = 64
      Top = 102
      Width = 55
      Height = 21
      MaxValue = 65535
      MinValue = 1
      TabOrder = 4
      Value = 1
    end
    object chkSayMsgControl: TCheckBox
      Left = 8
      Top = 16
      Width = 105
      Height = 19
      BiDiMode = bdLeftToRight
      Caption = #24320#21551#21457#35328#25511#21046
      ParentBiDiMode = False
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
      OnClick = chkSayMsgControlClick
    end
  end
  object grp1: TGroupBox
    Left = 8
    Top = 8
    Width = 240
    Height = 575
    Anchors = [akLeft, akTop, akBottom]
    Caption = #24403#21069#36830#25509
    TabOrder = 0
    DesignSize = (
      240
      575)
    object Label4: TLabel
      Left = 8
      Top = 17
      Width = 54
      Height = 12
      Caption = #36830#25509#21015#34920':'
    end
    object lstActive: TListBox
      Left = 7
      Top = 32
      Width = 225
      Height = 534
      Hint = #24403#21069#36830#25509#65292#40736#26631#21491#38190#36827#34892#25805#20316
      Anchors = [akLeft, akTop, akBottom]
      ItemHeight = 12
      Items.Strings = (
        '888.888.888.888')
      ParentShowHint = False
      PopupMenu = pmActive
      ShowHint = True
      Sorted = True
      TabOrder = 0
      OnKeyDown = lstActiveKeyDown
    end
  end
  object grp5: TGroupBox
    Left = 505
    Top = 545
    Width = 359
    Height = 38
    Anchors = [akLeft, akBottom]
    Caption = #39564#35777#23458#25143#31471#26159#21542#21512#27861
    TabOrder = 10
    object lbl3: TLabel
      Left = 124
      Top = 18
      Width = 96
      Height = 12
      Caption = #23458#25143#31471#39564#35777#22833#36133#65306
    end
    object chkOpenCheckClient: TCheckBox
      Left = 8
      Top = 16
      Width = 113
      Height = 17
      Hint = #24320#21551#23458#25143#31471#21512#27861#24615#39564#35777#21518#65292#21482#26377'GEE'#37197#22871#30340#30331#24405#22120#25165#33021#36830#25509#65292#19975#33021#30331#24405#22120#23558#26080#27861#36830#25509#65281#65281
      Caption = #21551#29992#23458#25143#31471#39564#35777
      ParentShowHint = False
      ShowHint = True
      TabOrder = 0
    end
    object cbbCheckClientFailBlockMode: TComboBox
      Left = 216
      Top = 14
      Width = 136
      Height = 20
      Style = csDropDownList
      ItemHeight = 12
      ItemIndex = 1
      TabOrder = 1
      Text = 'IP'#21152#20837#21160#24577#36807#28388#21015#34920
      Items.Strings = (
        #26029#24320
        'IP'#21152#20837#21160#24577#36807#28388#21015#34920
        'IP'#21152#20837#27704#20037#36807#28388#21015#34920)
    end
  end
  object grp4: TGroupBox
    Left = 663
    Top = 366
    Width = 201
    Height = 39
    Caption = #23458#25143#31471#38750#27861#21253#26816#26597
    TabOrder = 7
    object lbl2: TLabel
      Left = 123
      Top = 18
      Width = 72
      Height = 12
      Caption = #27425#38750#27861#20026#25915#20987
    end
    object chkCheckClientPacketLegal: TCheckBox
      Left = 8
      Top = 16
      Width = 57
      Height = 17
      Caption = #25968#25454#21253
      TabOrder = 0
    end
    object seCheckClientPacketCount: TSpinEditEx
      Left = 65
      Top = 14
      Width = 55
      Height = 21
      MaxValue = 255
      MinValue = 1
      TabOrder = 1
      Value = 1
    end
  end
  object pmActive: TPopupMenu
    OnChange = pmActiveChange
    Left = 56
    Top = 160
    object mniActiveRefesh: TMenuItem
      Caption = #21047#26032'(&R)'
      OnClick = mniActiveRefeshClick
    end
    object mniActiveSort: TMenuItem
      Caption = #25490#24207'(&S)'
      OnClick = mniActiveSortClick
    end
    object N3: TMenuItem
      Caption = '-'
    end
    object mniActiveAddToTemp: TMenuItem
      Caption = #21152#20837#21160#24577#36807#28388#21015#34920'(&B)'
      OnClick = mniActiveAddToTempClick
    end
    object mniActiveAddAllToTemp: TMenuItem
      Caption = #20840#37096#21152#20837#21160#24577#36807#28388#21015#34920'(&G)'
      OnClick = mniActiveAddAllToTempClick
    end
    object N2: TMenuItem
      Caption = '-'
    end
    object mniActiveAddToBlock: TMenuItem
      Caption = #21152#20837#27704#20037#36807#28388#21015#34920'(&E)'
      OnClick = mniActiveAddToBlockClick
    end
    object mniActiveAddAllToBlock: TMenuItem
      Caption = #20840#37096#21152#20837#27704#20037#36807#28388#21015#34920'(F)'
      OnClick = mniActiveAddAllToBlockClick
    end
    object mniN8: TMenuItem
      Caption = '-'
    end
    object mniActiveAddAllNoUserToTemp: TMenuItem
      Caption = #26080#24080#25143#36830#25509#20840#37096#21152#20837#21160#24577#36807#28388
      OnClick = mniActiveAddAllNoUserToTempClick
    end
    object mniActiveAddAllNoUserToBlock: TMenuItem
      Caption = #26080#24080#25143#36830#25509#20840#37096#21152#20837#27704#20037#36807#28388
      OnClick = mniActiveAddAllNoUserToBlockClick
    end
    object N1: TMenuItem
      Caption = '-'
    end
    object mniActiveKick: TMenuItem
      Caption = #36386#38500#19979#32447'(&T)'
      OnClick = mniActiveKickClick
    end
  end
  object pmTemp: TPopupMenu
    OnPopup = pmTempPopup
    Left = 336
    Top = 136
    object mniTempRefresh: TMenuItem
      Caption = #21047#26032'(&R)'
      OnClick = mniTempRefreshClick
    end
    object mniTempSort: TMenuItem
      Caption = #25490#24207'(&S)'
      OnClick = mniTempSortClick
    end
    object mniN4: TMenuItem
      Caption = '-'
    end
    object mniTempAdd: TMenuItem
      Caption = #22686#21152'(&A)'
      OnClick = mniTempAddClick
    end
    object mniTempDelete: TMenuItem
      Caption = #21024#38500'(&D)'
      OnClick = mniTempDeleteClick
    end
    object mniTempClear: TMenuItem
      Caption = #28165#31354'(&C)'
      OnClick = mniTempClearClick
    end
    object mniN5: TMenuItem
      Caption = '-'
    end
    object mniTempAddToBlock: TMenuItem
      Caption = #21152#20837#27704#20037#36807#28388#21015#34920'(&E)'
      OnClick = mniTempAddToBlockClick
    end
    object mniTempAddAllToBlock: TMenuItem
      Caption = #20840#37096#21152#20837#27704#20037#36807#28388#21015#34920'(&F)'
      OnClick = mniTempAddAllToBlockClick
    end
  end
  object pmBlock: TPopupMenu
    OnPopup = pmBlockPopup
    Left = 440
    Top = 136
    object mniBlockRefresh: TMenuItem
      Caption = #21047#26032'(&R)'
      OnClick = mniBlockRefreshClick
    end
    object mniBlockSort: TMenuItem
      Caption = #25490#24207'(&S)'
      OnClick = mniBlockSortClick
    end
    object mniN6: TMenuItem
      Caption = '-'
    end
    object mniBlockAdd: TMenuItem
      Caption = #22686#21152'(&A)'
      OnClick = mniBlockAddClick
    end
    object mniBlockDelete: TMenuItem
      Caption = #21024#38500'(&D)'
      OnClick = mniBlockDeleteClick
    end
    object mniBlockClear: TMenuItem
      Caption = #28165#31354'(&C)'
      OnClick = mniBlockClearClick
    end
    object mniN7: TMenuItem
      Caption = '-'
    end
    object mniBlockAddToTemp: TMenuItem
      Caption = #21152#20837#21160#24577#36807#28388#21015#34920'(&B)'
      OnClick = mniBlockAddToTempClick
    end
    object mniBlockAddAllToTemp: TMenuItem
      Caption = #20840#37096#21152#20837#21160#24577#36807#28388#21015#34920'(&G)'
      OnClick = mniBlockAddAllToTempClick
    end
  end
  object pmIpSection: TPopupMenu
    OnPopup = pmIpSectionPopup
    Left = 343
    Top = 213
    object mniIpSectionSort: TMenuItem
      Caption = #25490#24207'(&S)'
      OnClick = mniIpSectionSortClick
    end
    object mniIpSectionAdd: TMenuItem
      Caption = #22686#21152'IP'#27573'(&A)'
      OnClick = mniIpSectionAddClick
    end
    object mniIpSectionDel: TMenuItem
      Caption = #21024#38500'IP'#27573'(&D)'
      OnClick = mniIpSectionDelClick
    end
  end
  object pmTempMac: TPopupMenu
    OnPopup = pmTempMacPopup
    Left = 96
    Top = 344
    object mniTempMacRefresh: TMenuItem
      Caption = #21047#26032'(&R)'
      OnClick = mniTempMacRefreshClick
    end
    object mniTempMacSort: TMenuItem
      Caption = #25490#24207'(&S)'
      OnClick = mniTempMacSortClick
    end
    object MenuItem3: TMenuItem
      Caption = '-'
    end
    object mniTempMacAdd: TMenuItem
      Caption = #22686#21152'(&A)'
      OnClick = mniTempMacAddClick
    end
    object mniTempMacDelete: TMenuItem
      Caption = #21024#38500'(&D)'
      OnClick = mniTempMacDeleteClick
    end
    object mniTempMacClear: TMenuItem
      Caption = #28165#31354'(&C)'
      OnClick = mniTempMacClearClick
    end
    object MenuItem7: TMenuItem
      Caption = '-'
    end
    object mniTempMacAddToBlock: TMenuItem
      Caption = #21152#20837#27704#20037'MAC'#36807#28388#21015#34920'(&E)'
      OnClick = mniTempMacAddToBlockClick
    end
    object mniTempMacAddAllToBlock: TMenuItem
      Caption = #20840#37096#21152#20837#27704#20037'MAC'#36807#28388#21015#34920'(&F)'
      OnClick = mniTempMacAddAllToBlockClick
    end
  end
  object pmBlockMac: TPopupMenu
    OnPopup = pmBlockMacPopup
    Left = 104
    Top = 432
    object mniBlockMacRefresh: TMenuItem
      Caption = #21047#26032'(&R)'
      OnClick = mniBlockMacRefreshClick
    end
    object mniBlockMacSort: TMenuItem
      Caption = #25490#24207'(&S)'
      OnClick = mniBlockMacSortClick
    end
    object MenuItem4: TMenuItem
      Caption = '-'
    end
    object mniBlockMacAdd: TMenuItem
      Caption = #22686#21152'(&A)'
      OnClick = mniBlockMacAddClick
    end
    object mniBlockMacDelete: TMenuItem
      Caption = #21024#38500'(&D)'
      OnClick = mniBlockMacDeleteClick
    end
    object mniBlockMacClear: TMenuItem
      Caption = #28165#31354'(&C)'
      OnClick = mniBlockMacClearClick
    end
    object MenuItem9: TMenuItem
      Caption = '-'
    end
    object mniBlockMacAddToTempMac: TMenuItem
      Caption = #21152#20837#21160#24577#36807#28388#21015#34920'(&B)'
      OnClick = mniBlockMacAddToTempMacClick
    end
    object mniBlockMacAddAllToTempMac: TMenuItem
      Caption = #20840#37096#21152#20837#21160#24577#36807#28388#21015#34920'(&G)'
      OnClick = mniBlockMacAddAllToTempMacClick
    end
  end
end
