object frmViewLevel: TfrmViewLevel
  Left = 298
  Top = 220
  BorderIcons = [biSystemMenu, biMinimize]
  Caption = #20154#29289#31561#32423#23646#24615
  ClientHeight = 654
  ClientWidth = 768
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 12
  object btnSave: TButton
    Left = 693
    Top = 622
    Width = 65
    Height = 25
    Caption = #20445#23384'(&S)'
    TabOrder = 0
    OnClick = btnSaveClick
  end
  object grp1: TGroupBox
    Left = 13
    Top = 9
    Width = 368
    Height = 608
    TabOrder = 1
    object Label1: TLabel
      Left = 272
      Top = 24
      Width = 36
      Height = 12
      Caption = #32844#19994#65306
    end
    object Label4: TLabel
      Left = 145
      Top = 24
      Width = 60
      Height = 12
      Caption = #35282#33394#31867#22411#65306
    end
    object GridHumanInfo: TStringGrid
      Left = 145
      Top = 48
      Width = 213
      Height = 551
      ColCount = 2
      DefaultRowHeight = 18
      RowCount = 13
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goEditing]
      TabOrder = 0
      ColWidths = (
        50
        157)
      RowHeights = (
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18
        18)
    end
    object cbbDefJob: TComboBox
      Left = 303
      Top = 20
      Width = 55
      Height = 20
      Style = csDropDownList
      ItemIndex = 0
      TabOrder = 1
      Text = #27494#22763
      OnChange = cbbDefJobChange
      Items.Strings = (
        #27494#22763
        #39764#27861#24072
        #36947#22763)
    end
    object GroupBox5: TGroupBox
      Left = 10
      Top = 18
      Width = 125
      Height = 581
      Caption = #31561#32423
      TabOrder = 2
      object lstDefLevels: TListBox
        Left = 8
        Top = 45
        Width = 109
        Height = 527
        ItemHeight = 12
        TabOrder = 0
        OnClick = lstDefLevelsClick
      end
      object seDefLevel: TSpinEditLongWord
        Left = 8
        Top = 19
        Width = 111
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 1
        Value = 0
        OnChange = seDefLevelChange
      end
    end
    object cbbDefUserType: TComboBox
      Left = 200
      Top = 20
      Width = 55
      Height = 20
      Style = csDropDownList
      ItemIndex = 0
      TabOrder = 3
      Text = #20154#29289
      OnChange = cbbDefUserTypeChange
      Items.Strings = (
        #20154#29289
        #33521#38596)
    end
  end
  object rbDef: TRadioButton
    Left = 23
    Top = 7
    Width = 154
    Height = 17
    Caption = #20351#29992#24341#25806#40664#35748#31561#32423#23646#24615
    Font.Charset = GB2312_CHARSET
    Font.Color = clWindowText
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 2
    OnClick = rbDefClick
  end
  object GroupBox1: TGroupBox
    Left = 390
    Top = 9
    Width = 368
    Height = 608
    TabOrder = 3
    object Label2: TLabel
      Left = 272
      Top = 24
      Width = 36
      Height = 12
      Caption = #32844#19994#65306
    end
    object Label3: TLabel
      Left = 145
      Top = 24
      Width = 60
      Height = 12
      Caption = #35282#33394#31867#22411#65306
    end
    object GroupBox3: TGroupBox
      Left = 145
      Top = 48
      Width = 213
      Height = 253
      Caption = '1000'#32423#20197#20869#23646#24615
      TabOrder = 0
      object lbl25: TLabel
        Left = 6
        Top = 22
        Width = 36
        Height = 12
        Caption = #38450#24481#65306
      end
      object lbl27: TLabel
        Left = 6
        Top = 45
        Width = 36
        Height = 12
        Caption = #39764#24481#65306
      end
      object lbl9: TLabel
        Left = 6
        Top = 68
        Width = 36
        Height = 12
        Caption = #25915#20987#65306
      end
      object lbl11: TLabel
        Left = 6
        Top = 91
        Width = 36
        Height = 12
        Caption = #39764#27861#65306
      end
      object lbl14: TLabel
        Left = 6
        Top = 114
        Width = 36
        Height = 12
        Caption = #36947#26415#65306
      end
      object Label5: TLabel
        Left = 6
        Top = 138
        Width = 30
        Height = 12
        Caption = 'MaxHP'
      end
      object Label6: TLabel
        Left = 6
        Top = 161
        Width = 30
        Height = 12
        Caption = 'MaxMP'
      end
      object Label14: TLabel
        Left = 6
        Top = 184
        Width = 36
        Height = 12
        Caption = #32972#21253#65306
      end
      object Label15: TLabel
        Left = 6
        Top = 207
        Width = 36
        Height = 12
        Caption = #36127#37325#65306
      end
      object Label16: TLabel
        Left = 6
        Top = 230
        Width = 36
        Height = 12
        Caption = #33109#21147#65306
      end
      object seAC1: TSpinEditLongWord
        Tag = 2
        Left = 41
        Top = 18
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 0
        Value = 0
        OnChange = seAC1Change
      end
      object seAC2: TSpinEditLongWord
        Tag = 14
        Left = 126
        Top = 18
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 1
        Value = 0
        OnChange = seAC1Change
      end
      object seMAC1: TSpinEditLongWord
        Tag = 3
        Left = 41
        Top = 41
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 2
        Value = 0
        OnChange = seAC1Change
      end
      object seMAC2: TSpinEditLongWord
        Tag = 15
        Left = 126
        Top = 41
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 3
        Value = 0
        OnChange = seAC1Change
      end
      object seDC1: TSpinEditLongWord
        Tag = 4
        Left = 41
        Top = 64
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 4
        Value = 0
        OnChange = seAC1Change
      end
      object seDC2: TSpinEditLongWord
        Tag = 16
        Left = 126
        Top = 64
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 5
        Value = 0
        OnChange = seAC1Change
      end
      object seMC1: TSpinEditLongWord
        Tag = 5
        Left = 41
        Top = 87
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 6
        Value = 0
        OnChange = seAC1Change
      end
      object seMC2: TSpinEditLongWord
        Tag = 17
        Left = 126
        Top = 87
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 7
        Value = 0
        OnChange = seAC1Change
      end
      object seSC1: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 110
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 8
        Value = 0
        OnChange = seAC1Change
      end
      object seSC2: TSpinEditLongWord
        Tag = 18
        Left = 126
        Top = 110
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 9
        Value = 0
        OnChange = seAC1Change
      end
      object seMaxHP: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 133
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 10
        Value = 0
        OnChange = seAC1Change
      end
      object seMaxMP: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 156
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 11
        Value = 0
        OnChange = seAC1Change
      end
      object seMaxWeight: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 179
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 12
        Value = 0
        OnChange = seAC1Change
      end
      object seMaxWearWeight: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 202
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 13
        Value = 0
        OnChange = seAC1Change
      end
      object seMaxHandWeight: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 225
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 14
        Value = 0
        OnChange = seAC1Change
      end
    end
    object GroupBox4: TGroupBox
      Left = 145
      Top = 344
      Width = 213
      Height = 255
      Caption = '1000'#32423#20197#21518#27599#32423#22686#21152#23646#24615
      TabOrder = 1
      object Label7: TLabel
        Left = 6
        Top = 22
        Width = 36
        Height = 12
        Caption = #38450#24481#65306
      end
      object Label8: TLabel
        Left = 6
        Top = 45
        Width = 36
        Height = 12
        Caption = #39764#24481#65306
      end
      object Label9: TLabel
        Left = 6
        Top = 68
        Width = 36
        Height = 12
        Caption = #25915#20987#65306
      end
      object Label10: TLabel
        Left = 6
        Top = 91
        Width = 36
        Height = 12
        Caption = #39764#27861#65306
      end
      object Label11: TLabel
        Left = 6
        Top = 114
        Width = 36
        Height = 12
        Caption = #36947#26415#65306
      end
      object Label12: TLabel
        Left = 6
        Top = 138
        Width = 30
        Height = 12
        Caption = 'MaxHP'
      end
      object Label13: TLabel
        Left = 6
        Top = 161
        Width = 30
        Height = 12
        Caption = 'MaxMP'
      end
      object Label17: TLabel
        Left = 6
        Top = 184
        Width = 36
        Height = 12
        Caption = #32972#21253#65306
      end
      object Label18: TLabel
        Left = 6
        Top = 207
        Width = 36
        Height = 12
        Caption = #36127#37325#65306
      end
      object Label19: TLabel
        Left = 6
        Top = 230
        Width = 36
        Height = 12
        Caption = #33109#21147#65306
      end
      object seAddAC1: TSpinEditLongWord
        Tag = 2
        Left = 41
        Top = 18
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 0
        Value = 0
        OnChange = seAC1Change
      end
      object seAddAC2: TSpinEditLongWord
        Tag = 14
        Left = 126
        Top = 18
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 1
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMAC1: TSpinEditLongWord
        Tag = 3
        Left = 41
        Top = 41
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 2
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMAC2: TSpinEditLongWord
        Tag = 15
        Left = 126
        Top = 41
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 3
        Value = 0
        OnChange = seAC1Change
      end
      object seAddDC1: TSpinEditLongWord
        Tag = 4
        Left = 41
        Top = 64
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 4
        Value = 0
        OnChange = seAC1Change
      end
      object seAddDC2: TSpinEditLongWord
        Tag = 16
        Left = 126
        Top = 64
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 5
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMC1: TSpinEditLongWord
        Tag = 5
        Left = 41
        Top = 87
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 6
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMC2: TSpinEditLongWord
        Tag = 17
        Left = 126
        Top = 87
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 7
        Value = 0
        OnChange = seAC1Change
      end
      object seAddSC1: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 110
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 8
        Value = 0
        OnChange = seAC1Change
      end
      object seAddSC2: TSpinEditLongWord
        Tag = 18
        Left = 126
        Top = 110
        Width = 80
        Height = 21
        MaxValue = 2147483647
        MinValue = 0
        TabOrder = 9
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMaxHP: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 133
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 10
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMaxMP: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 156
        Width = 80
        Height = 21
        MaxValue = 0
        MinValue = 0
        TabOrder = 11
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMaxWeight: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 179
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 12
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMaxWearWeight: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 202
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 13
        Value = 0
        OnChange = seAC1Change
      end
      object seAddMaxHandWeight: TSpinEditLongWord
        Tag = 6
        Left = 41
        Top = 225
        Width = 80
        Height = 21
        MaxValue = 65535
        MinValue = 0
        TabOrder = 14
        Value = 0
        OnChange = seAC1Change
      end
    end
    object rbCustomAutoCalc: TRadioButton
      Left = 143
      Top = 306
      Width = 201
      Height = 17
      Caption = '1000'#32423#20197#21518#24341#25806#33258#21160#35745#31639#23646#24615
      TabOrder = 2
      OnClick = seAC1Change
    end
    object rbCustomSetValue: TRadioButton
      Left = 143
      Top = 324
      Width = 201
      Height = 17
      Caption = '1000'#32423#20197#21518#27599#32423#33258#23450#20041#22686#21152#23646#24615
      TabOrder = 3
      OnClick = seAC1Change
    end
    object GroupBox6: TGroupBox
      Left = 10
      Top = 18
      Width = 125
      Height = 582
      Caption = #31561#32423
      TabOrder = 4
      object seCustomLevel: TSpinEditLongWord
        Left = 8
        Top = 19
        Width = 110
        Height = 21
        MaxValue = 1000
        MinValue = 1
        TabOrder = 0
        Value = 1
        OnChange = seCustomLevelChange
      end
      object lstCustomLevels: TListBox
        Left = 8
        Top = 45
        Width = 109
        Height = 529
        ItemHeight = 12
        TabOrder = 1
        OnClick = lstCustomLevelsClick
      end
    end
    object cbbCustomJob: TComboBox
      Left = 303
      Top = 20
      Width = 55
      Height = 20
      Style = csDropDownList
      ItemIndex = 0
      TabOrder = 5
      Text = #25112#22763
      OnChange = seCustomLevelChange
      Items.Strings = (
        #25112#22763
        #27861#24072
        #36947#22763)
    end
    object cbbCustomUserType: TComboBox
      Left = 200
      Top = 20
      Width = 55
      Height = 20
      Style = csDropDownList
      ItemIndex = 0
      TabOrder = 6
      Text = #20154#29289
      OnChange = seCustomLevelChange
      Items.Strings = (
        #20154#29289
        #33521#38596)
    end
  end
  object rbCustom: TRadioButton
    Left = 399
    Top = 7
    Width = 146
    Height = 17
    Caption = #20351#29992#33258#23450#20041#31561#32423#23646#24615
    Font.Charset = GB2312_CHARSET
    Font.Color = clWindowText
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 4
    OnClick = rbDefClick
  end
  object btnInitCustom: TButton
    Left = 389
    Top = 622
    Width = 163
    Height = 25
    Caption = #21021#22987#21270#33258#23450#20041#31561#32423#23646#24615
    TabOrder = 5
    OnClick = btnInitCustomClick
  end
end
