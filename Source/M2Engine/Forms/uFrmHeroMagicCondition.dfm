object FrmHeroMagicCondition: TFrmHeroMagicCondition
  Left = 475
  Top = 529
  BorderStyle = bsDialog
  Caption = #25216#33021#26465#20214#35774#32622
  ClientHeight = 306
  ClientWidth = 618
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 14
  object grp1: TGroupBox
    Left = 11
    Top = 9
    Width = 597
    Height = 104
    Caption = #25191#34892#26465#20214
    TabOrder = 0
    object chkHeroLevel: TCheckBox
      Left = 9
      Top = 22
      Width = 78
      Height = 18
      Caption = #33521#38596#31561#32423
      TabOrder = 0
    end
    object cbbHeroLevelCompareSymbol: TComboBox
      Left = 82
      Top = 19
      Width = 44
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 1
    end
    object edtHeroLevelCompareValue: TSpinEditLongWord
      Left = 214
      Top = 19
      Width = 64
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 3
      Value = 0
    end
    object chkHeroHP: TCheckBox
      Left = 9
      Top = 47
      Width = 78
      Height = 19
      Caption = #33521#38596'HP'
      TabOrder = 4
    end
    object edtHeroHPCompareValue: TSpinEditLongWord
      Left = 214
      Top = 45
      Width = 65
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 7
      Value = 0
    end
    object cbbHeroHPCompareType: TComboBox
      Left = 130
      Top = 45
      Width = 80
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 6
    end
    object cbbHeroHPCompareSymbol: TComboBox
      Left = 82
      Top = 45
      Width = 44
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 5
    end
    object cbbHeroLevelCompareType: TComboBox
      Left = 131
      Top = 19
      Width = 79
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 2
      OnChange = cbbHeroLevelCompareTypeChange
    end
    object chkHeroMP: TCheckBox
      Left = 9
      Top = 73
      Width = 78
      Height = 19
      Caption = #33521#38596'MP'
      TabOrder = 12
    end
    object edtHeroMPCompareValue: TSpinEditLongWord
      Left = 214
      Top = 71
      Width = 65
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 15
      Value = 0
    end
    object cbbHeroMPCompareType: TComboBox
      Left = 130
      Top = 71
      Width = 80
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 14
    end
    object cbbHeroMPCompareSymbol: TComboBox
      Left = 82
      Top = 71
      Width = 44
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 13
    end
    object chkTargetHP: TCheckBox
      Left = 319
      Top = 47
      Width = 78
      Height = 19
      Caption = #30446#26631'HP'
      TabOrder = 8
    end
    object edtTargetHPCompareValue: TSpinEditLongWord
      Left = 524
      Top = 45
      Width = 65
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 11
      Value = 0
    end
    object cbbTargetHPCompareType: TComboBox
      Left = 440
      Top = 45
      Width = 80
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 10
    end
    object cbbTargetHPCompareSymbol: TComboBox
      Left = 392
      Top = 45
      Width = 44
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 9
    end
    object chkTargetMP: TCheckBox
      Left = 319
      Top = 73
      Width = 78
      Height = 19
      Caption = #30446#26631'MP'
      TabOrder = 16
    end
    object edtTargetMPCompareValue: TSpinEditLongWord
      Left = 524
      Top = 71
      Width = 65
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 19
      Value = 0
    end
    object cbbTargetMPCompareType: TComboBox
      Left = 440
      Top = 71
      Width = 80
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 18
    end
    object cbbTargetMPCompareSymbol: TComboBox
      Left = 392
      Top = 71
      Width = 44
      Height = 22
      Style = csDropDownList
      ItemHeight = 14
      TabOrder = 17
    end
  end
  object grp2: TGroupBox
    Left = 11
    Top = 121
    Width = 599
    Height = 78
    Caption = #30446#26631#29366#24577#21028#26029
    TabOrder = 1
    object chkNoPoisonDamageArmor: TCheckBox
      Left = 10
      Top = 51
      Width = 62
      Height = 18
      Caption = #26080#32418#27602
      TabOrder = 7
    end
    object chkNoPoisonDecHealth: TCheckBox
      Left = 89
      Top = 51
      Width = 63
      Height = 18
      Caption = #26080#32511#27602
      TabOrder = 8
    end
    object chkNoPoisoning: TCheckBox
      Left = 170
      Top = 51
      Width = 99
      Height = 18
      Hint = #21482#20013#20102#19968#31181#27602
      Caption = #26080#32418#27602#25110#32511#27602
      TabOrder = 9
    end
    object chkNoPoisonStone: TCheckBox
      Left = 288
      Top = 51
      Width = 62
      Height = 18
      Caption = #26080#40635#30201
      TabOrder = 10
    end
    object chkNoFrozen: TCheckBox
      Left = 367
      Top = 51
      Width = 63
      Height = 18
      Caption = #26080#20912#20923
      TabOrder = 11
    end
    object chkNoForeverFrozen: TCheckBox
      Left = 447
      Top = 51
      Width = 62
      Height = 18
      Caption = #26080#20912#23553
      TabOrder = 12
    end
    object chkNoCobwebWinding: TCheckBox
      Left = 528
      Top = 51
      Width = 62
      Height = 18
      Caption = #26080#34523#32593
      TabOrder = 13
    end
    object chkPoisonDamageArmor: TCheckBox
      Left = 10
      Top = 25
      Width = 62
      Height = 18
      Caption = #20013#32418#27602
      TabOrder = 0
    end
    object chkPoisonDecHealth: TCheckBox
      Left = 89
      Top = 25
      Width = 63
      Height = 18
      Caption = #20013#32511#27602
      TabOrder = 1
    end
    object chkPoisoning: TCheckBox
      Left = 170
      Top = 25
      Width = 99
      Height = 18
      Hint = #21482#20013#20102#19968#31181#27602
      Caption = #20013#32418#27602#25110#32511#27602
      TabOrder = 2
    end
    object chkPoisonStone: TCheckBox
      Left = 288
      Top = 25
      Width = 62
      Height = 18
      Caption = #34987#40635#30201
      TabOrder = 3
    end
    object chkFrozen: TCheckBox
      Left = 367
      Top = 25
      Width = 63
      Height = 18
      Caption = #34987#20912#20923
      TabOrder = 4
    end
    object chkForeverFrozen: TCheckBox
      Left = 447
      Top = 25
      Width = 62
      Height = 18
      Caption = #34987#20912#23553
      TabOrder = 5
    end
    object chkCobwebWinding: TCheckBox
      Left = 528
      Top = 25
      Width = 62
      Height = 18
      Caption = #20013#34523#32593
      TabOrder = 6
    end
  end
  object grp3: TGroupBox
    Left = 11
    Top = 207
    Width = 599
    Height = 53
    Caption = #30446#26631#36523#36793#35282#33394#25968#37327
    TabOrder = 2
    object lbl1: TLabel
      Left = 123
      Top = 26
      Width = 92
      Height = 13
      Caption = #26684#33539#22260#26379#21451#25968#37327'>'
    end
    object Label1: TLabel
      Left = 429
      Top = 26
      Width = 92
      Height = 13
      Caption = #26684#33539#22260#25932#20154#25968#37327'>'
    end
    object chkFriendCount: TCheckBox
      Left = 9
      Top = 24
      Width = 72
      Height = 18
      Caption = #30446#26631#36523#36793
      TabOrder = 0
    end
    object edtFriendCheckValue: TSpinEditLongWord
      Left = 223
      Top = 22
      Width = 61
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 2
      Value = 0
    end
    object edtFriendCheckRange: TSpinEditLongWord
      Left = 82
      Top = 22
      Width = 40
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 1
      Value = 0
    end
    object chkEnemyCount: TCheckBox
      Left = 313
      Top = 24
      Width = 71
      Height = 18
      Caption = #30446#26631#36523#36793
      TabOrder = 3
    end
    object edtEnemyCheckValue: TSpinEditLongWord
      Left = 529
      Top = 22
      Width = 61
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 5
      Value = 0
    end
    object edtEnemyCheckRange: TSpinEditLongWord
      Left = 388
      Top = 22
      Width = 40
      Height = 22
      MaxValue = 0
      MinValue = 0
      TabOrder = 4
      Value = 0
    end
  end
  object btnOK: TButton
    Left = 529
    Top = 267
    Width = 81
    Height = 27
    Caption = #30830#23450
    TabOrder = 3
    OnClick = btnOKClick
  end
  object chkStraightLineCheck: TCheckBox
    Left = 11
    Top = 271
    Width = 205
    Height = 19
    Caption = #33521#38596#19982#30446#26631#33021#26500#25104#30452#32447#26102#20351#29992
    TabOrder = 4
  end
end
