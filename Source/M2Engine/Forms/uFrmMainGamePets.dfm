object FrmGamePets: TFrmGamePets
  Left = 697
  Top = 397
  BorderStyle = bsDialog
  Caption = #23456#29289#35774#32622
  ClientHeight = 506
  ClientWidth = 668
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  TextHeight = 13
  object pgcMain: TPageControl
    Left = 6
    Top = 7
    Width = 659
    Height = 493
    ActivePage = TabSheet2
    TabOrder = 0
    object ts1: TTabSheet
      Caption = #22522#26412#35774#32622
      object lbl2: TLabel
        Left = 298
        Top = 8
        Width = 60
        Height = 13
        Caption = #24618#29289#21517#31216#65306
      end
      object lbl1: TLabel
        Left = 298
        Top = 32
        Width = 60
        Height = 13
        Caption = #25429#25417#20960#29575#65306
      end
      object Label23: TLabel
        Left = 475
        Top = 32
        Width = 162
        Height = 13
        Caption = #20960#29575#20026'0'#26102#34920#31034#24618#29289#19981#25903#25345#25429#25417
      end
      object grp1: TGroupBox
        Left = 3
        Top = 0
        Width = 141
        Height = 462
        Caption = #25152#26377#24618#29289#21015#34920
        TabOrder = 0
        object lstMonsterList: TListBox
          Left = 6
          Top = 16
          Width = 129
          Height = 440
          ItemHeight = 13
          TabOrder = 0
          OnDblClick = lstMonsterListDblClick
          OnKeyDown = lstMonsterListKeyDown
        end
      end
      object GroupBox1: TGroupBox
        Left = 149
        Top = 0
        Width = 141
        Height = 462
        Caption = #24050#35774#32622#30340#24618#29289
        TabOrder = 1
        object lstGamePets: TListBox
          Left = 6
          Top = 16
          Width = 129
          Height = 440
          ItemHeight = 13
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = lstGamePetsClick
        end
      end
      object grp3: TGroupBox
        Left = 296
        Top = 103
        Width = 349
        Height = 162
        Caption = #23456#29289#21160#30011#23637#29616
        TabOrder = 5
        object Label20: TLabel
          Left = 9
          Top = 20
          Width = 60
          Height = 13
          Caption = #36164#28304#25991#20214#65306
        end
        object Label21: TLabel
          Left = 9
          Top = 43
          Width = 60
          Height = 13
          Caption = #24320#22987#22270#29255#65306
        end
        object Label22: TLabel
          Left = 9
          Top = 67
          Width = 60
          Height = 13
          Caption = #22270#29255#25968#37327#65306
        end
        object Label1: TLabel
          Left = 185
          Top = 20
          Width = 60
          Height = 13
          Caption = #36164#28304#25991#20214#65306
        end
        object Label2: TLabel
          Left = 185
          Top = 43
          Width = 60
          Height = 13
          Caption = #24320#22987#22270#29255#65306
        end
        object Label3: TLabel
          Left = 185
          Top = 67
          Width = 60
          Height = 13
          Caption = #22270#29255#25968#37327#65306
        end
        object Label4: TLabel
          Left = 9
          Top = 91
          Width = 60
          Height = 13
          Caption = #25773#25918#38388#38548#65306
        end
        object Label5: TLabel
          Left = 185
          Top = 91
          Width = 60
          Height = 13
          Caption = #25773#25918#38388#38548#65306
        end
        object bvl1: TBevel
          Left = 174
          Top = 8
          Width = 2
          Height = 150
          Shape = bsLeftLine
        end
        object Label16: TLabel
          Left = 27
          Top = 115
          Width = 42
          Height = 13
          Caption = #20559#31227'X'#65306
        end
        object Label17: TLabel
          Left = 27
          Top = 139
          Width = 42
          Height = 13
          Caption = #20559#31227'Y'#65306
        end
        object Label18: TLabel
          Left = 203
          Top = 115
          Width = 42
          Height = 13
          Caption = #20559#31227'X'#65306
        end
        object Label19: TLabel
          Left = 203
          Top = 139
          Width = 42
          Height = 13
          Caption = #20559#31227'Y'#65306
        end
        object cbbPetShowFile1: TComboBox
          Left = 66
          Top = 16
          Width = 100
          Height = 21
          Style = csDropDownList
          TabOrder = 0
        end
        object sePetShowCount1: TSpinEditEx
          Left = 66
          Top = 63
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
        end
        object sePetShowStart1: TSpinEditEx
          Left = 66
          Top = 39
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
        end
        object cbbPetShowFile2: TComboBox
          Left = 242
          Top = 16
          Width = 100
          Height = 21
          Style = csDropDownList
          TabOrder = 1
        end
        object sePetShowCount2: TSpinEditEx
          Left = 242
          Top = 63
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
        end
        object sePetShowStart2: TSpinEditEx
          Left = 242
          Top = 39
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 0
        end
        object sePetShowTime1: TSpinEditEx
          Left = 66
          Top = 87
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 0
        end
        object sePetShowTime2: TSpinEditEx
          Left = 241
          Top = 87
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 0
        end
        object sePetShowOffsetX1: TSpinEditEx
          Left = 66
          Top = 111
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 8
          Value = 0
        end
        object sePetShowOffsetY1: TSpinEditEx
          Left = 66
          Top = 135
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 11
          Value = 0
        end
        object sePetShowOffsetX2: TSpinEditEx
          Left = 241
          Top = 111
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 9
          Value = 0
        end
        object sePetShowOffsetY2: TSpinEditEx
          Left = 241
          Top = 134
          Width = 100
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 10
          Value = 0
        end
      end
      object btnAddPet: TButton
        Left = 296
        Top = 438
        Width = 79
        Height = 25
        Caption = #22686#21152
        TabOrder = 7
        OnClick = btnAddPetClick
      end
      object btnDelPet: TButton
        Left = 386
        Top = 438
        Width = 79
        Height = 25
        Caption = #21024#38500
        TabOrder = 8
        OnClick = btnDelPetClick
      end
      object btnEditPet: TButton
        Left = 475
        Top = 438
        Width = 79
        Height = 25
        Caption = #20462#25913
        TabOrder = 9
        OnClick = btnEditPetClick
      end
      object btnSavePet: TButton
        Left = 565
        Top = 438
        Width = 79
        Height = 25
        Caption = #20445#23384
        TabOrder = 10
        OnClick = btnSavePetClick
      end
      object edtPetName: TEdit
        Left = 355
        Top = 4
        Width = 115
        Height = 21
        TabOrder = 2
      end
      object sePetCaptureRate: TSpinEditEx
        Left = 355
        Top = 28
        Width = 116
        Height = 22
        Hint = #25968#23383#36234#22823#65292#20960#29575#36234#23567
        MaxValue = 0
        MinValue = 0
        ParentShowHint = False
        ShowHint = True
        TabOrder = 3
        Value = 0
      end
      object GroupBox2: TGroupBox
        Left = 296
        Top = 269
        Width = 349
        Height = 164
        Caption = #21319#32423#22686#21152#23646#24615
        TabOrder = 6
        object Label6: TLabel
          Left = 9
          Top = 20
          Width = 73
          Height = 13
          Caption = #22686#21152#23456#29289'HP'#65306
        end
        object Label7: TLabel
          Left = 9
          Top = 44
          Width = 84
          Height = 13
          Caption = #25915#20987#8212'>  '#19979#38480#65306
        end
        object Label8: TLabel
          Left = 9
          Top = 92
          Width = 84
          Height = 13
          Caption = #36947#26415#8212'>  '#19979#38480#65306
        end
        object Label9: TLabel
          Left = 9
          Top = 117
          Width = 84
          Height = 13
          Caption = #38450#24481#8212'>  '#19979#38480#65306
        end
        object Label10: TLabel
          Left = 9
          Top = 141
          Width = 84
          Height = 13
          Caption = #39764#38450#8212'>  '#19979#38480#65306
        end
        object lbl3: TLabel
          Left = 200
          Top = 44
          Width = 36
          Height = 13
          Caption = #19978#38480#65306
        end
        object Label11: TLabel
          Left = 200
          Top = 92
          Width = 36
          Height = 13
          Caption = #19978#38480#65306
        end
        object Label12: TLabel
          Left = 200
          Top = 116
          Width = 36
          Height = 13
          Caption = #19978#38480#65306
        end
        object Label13: TLabel
          Left = 200
          Top = 140
          Width = 36
          Height = 13
          Caption = #19978#38480#65306
        end
        object Label14: TLabel
          Left = 9
          Top = 68
          Width = 84
          Height = 13
          Caption = #39764#27861#8212'>  '#19979#38480#65306
        end
        object Label15: TLabel
          Left = 200
          Top = 68
          Width = 36
          Height = 13
          Caption = #19978#38480#65306
        end
        object lbl4: TLabel
          Left = 203
          Top = 19
          Width = 135
          Height = 13
          Caption = '%:'#25968#25454#24211#37197#32622#20540#30340#30334#20998#27604
        end
        object sePetAddHP: TSpinEditEx
          Left = 88
          Top = 16
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
        end
        object cbbPetAddHPType: TComboBox
          Left = 155
          Top = 16
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 1
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddDC1: TSpinEditEx
          Left = 89
          Top = 40
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
        end
        object cbbPetAddDCType1: TComboBox
          Left = 155
          Top = 40
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddSC1: TSpinEditEx
          Left = 89
          Top = 88
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 10
          Value = 0
        end
        object cbbPetAddSCType1: TComboBox
          Left = 155
          Top = 88
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 11
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddSC2: TSpinEditEx
          Left = 234
          Top = 88
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 12
          Value = 0
        end
        object cbbPetAddSCType2: TComboBox
          Left = 300
          Top = 88
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 13
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddDC2: TSpinEditEx
          Left = 234
          Top = 40
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
        end
        object cbbPetAddDCType2: TComboBox
          Left = 300
          Top = 40
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 5
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddAC1: TSpinEditEx
          Left = 89
          Top = 113
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 14
          Value = 0
        end
        object cbbPetAddACType1: TComboBox
          Left = 155
          Top = 113
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 15
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddAC2: TSpinEditEx
          Left = 234
          Top = 113
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 16
          Value = 0
        end
        object cbbPetAddACType2: TComboBox
          Left = 300
          Top = 113
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 17
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddMAC1: TSpinEditEx
          Left = 89
          Top = 137
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 18
          Value = 0
        end
        object cbbPetAddMACType1: TComboBox
          Left = 155
          Top = 137
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 19
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddMAC2: TSpinEditEx
          Left = 234
          Top = 137
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 20
          Value = 0
        end
        object cbbPetAddMACType2: TComboBox
          Left = 300
          Top = 137
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 21
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddMC1: TSpinEditEx
          Left = 89
          Top = 64
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 0
        end
        object cbbPetAddMCType1: TComboBox
          Left = 155
          Top = 64
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 7
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
        object sePetAddMC2: TSpinEditEx
          Left = 234
          Top = 64
          Width = 63
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 8
          Value = 0
        end
        object cbbPetAddMCType2: TComboBox
          Left = 300
          Top = 64
          Width = 42
          Height = 21
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 9
          Text = #28857
          Items.Strings = (
            #28857
            '%')
        end
      end
      object grp6: TGroupBox
        Left = 296
        Top = 52
        Width = 349
        Height = 47
        Caption = #25429#25417#26465#20214
        TabOrder = 4
        object lbl8: TLabel
          Left = 212
          Top = 23
          Width = 40
          Height = 13
          Caption = #34880#37327'<='
        end
        object lbl9: TLabel
          Left = 328
          Top = 23
          Width = 11
          Height = 13
          Caption = '%'
        end
        object chkLevelDifference: TCheckBox
          Left = 8
          Top = 21
          Width = 83
          Height = 17
          Caption = #21028#26029#31561#32423#24046
          TabOrder = 2
        end
        object seLevelDifference: TSpinEditEx
          Left = 90
          Top = 18
          Width = 71
          Height = 22
          Hint = #25429#25417#32773#31561#32423'-'#24618#29289#31561#32423'>='#31561#32423#24046#65292#21487#20197#20026#36127#25968
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 2
        end
        object seHPScale: TSpinEditEx
          Left = 257
          Top = 18
          Width = 71
          Height = 22
          Hint = #24403#24618#29289#34880#37327#39640#20110#24635#34880#37327#30340#30334#20998#27604#26102#65292#19981#20801#35768#25429#25417
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 50
        end
      end
    end
    object TabSheet1: TTabSheet
      Caption = #21319#32423#32463#39564
      ImageIndex = 1
      object GroupBox198: TGroupBox
        Left = 4
        Top = 3
        Width = 285
        Height = 459
        Caption = #21319#32423#32463#39564
        TabOrder = 0
        object Label647: TLabel
          Left = 9
          Top = 437
          Width = 28
          Height = 13
          Caption = #35745#21010':'
        end
        object GridLevelExp: TStringGrid
          Left = 9
          Top = 16
          Width = 266
          Height = 411
          ColCount = 2
          DefaultColWidth = 68
          DefaultRowHeight = 18
          RowCount = 1001
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
          TabOrder = 0
          OnSetEditText = GridLevelExpSetEditText
          ColWidths = (
            68
            68)
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
            18
            18
            18
            18
            18)
        end
        object cbbLevelExp: TComboBox
          Left = 41
          Top = 433
          Width = 235
          Height = 21
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          OnClick = cbbLevelExpClick
        end
      end
      object GroupBox199: TGroupBox
        Left = 296
        Top = 3
        Width = 172
        Height = 75
        Caption = #26368#39640#31561#32423#38480#21046
        TabOrder = 1
        object Label648: TLabel
          Left = 8
          Top = 24
          Width = 52
          Height = 13
          Caption = #26368#39640#31561#32423':'
        end
        object Label649: TLabel
          Left = 8
          Top = 48
          Width = 52
          Height = 13
          Caption = #26432#24618#32463#39564':'
        end
        object sePetHighLevel: TSpinEditEx
          Left = 64
          Top = 20
          Width = 100
          Height = 22
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 0
          Value = 100000000
          OnChange = sePetHighLevelChange
        end
        object sePetHighLevelGetExp: TSpinEditEx
          Left = 64
          Top = 46
          Width = 100
          Height = 22
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 1
          Value = 1000000
          OnChange = sePetHighLevelGetExpChange
        end
      end
      object btnSaveExp: TButton
        Left = 573
        Top = 438
        Width = 75
        Height = 25
        Caption = #20445#23384
        TabOrder = 3
        OnClick = btnSaveExpClick
      end
      object GroupBox74: TGroupBox
        Left = 296
        Top = 84
        Width = 172
        Height = 94
        Caption = '1000'#32423#20197#21518#32463#39564#37197#21046
        TabOrder = 2
        object Label24: TLabel
          Left = 8
          Top = 43
          Width = 52
          Height = 13
          Caption = #22522#26412#32463#39564':'
        end
        object Label145: TLabel
          Left = 8
          Top = 67
          Width = 52
          Height = 13
          Caption = #22686#21152#32463#39564':'
        end
        object chkPetFixExp: TCheckBox
          Left = 8
          Top = 19
          Width = 145
          Height = 17
          Caption = #20351#29992#24341#25806#20869#37096#22266#23450#32463#39564
          TabOrder = 0
          OnClick = chkPetFixExpClick
        end
        object sePetBaseExp: TSpinEditEx
          Left = 64
          Top = 39
          Width = 101
          Height = 22
          Hint = #20154#29289#22522#26412#32463#39564
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 1
          Value = 100000000
          OnChange = sePetBaseExpChange
        end
        object sePetAddExp: TSpinEditEx
          Left = 64
          Top = 65
          Width = 101
          Height = 22
          Hint = #27599#27425#21319#32423#22686#21152#30340#32463#39564
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 2
          Value = 1000000
          OnChange = sePetAddExpChange
        end
      end
    end
    object TabSheet2: TTabSheet
      Caption = #21442#25968#35774#32622
      ImageIndex = 2
      object grp2: TGroupBox
        Left = 151
        Top = 2
        Width = 166
        Height = 108
        Caption = #21472#21152#23646#24615#32473#20027#20154
        TabOrder = 1
        object lbl5: TLabel
          Left = 8
          Top = 20
          Width = 60
          Height = 13
          Caption = #21472#21152#20493#29575#65306
        end
        object lbl6: TLabel
          Left = 150
          Top = 19
          Width = 11
          Height = 13
          Caption = '%'
        end
        object chkPetHPToMaster: TCheckBox
          Left = 8
          Top = 40
          Width = 65
          Height = 17
          Caption = #21472#21152'HP'
          TabOrder = 1
          OnClick = chkPetHPToMasterClick
        end
        object chkPetDCToMaster: TCheckBox
          Left = 84
          Top = 40
          Width = 65
          Height = 17
          Caption = #21472#21152#25915#20987
          TabOrder = 2
          OnClick = chkPetDCToMasterClick
        end
        object chkPetMCToMaster: TCheckBox
          Left = 8
          Top = 60
          Width = 65
          Height = 17
          Caption = #21472#21152#39764#27861
          TabOrder = 3
          OnClick = chkPetMCToMasterClick
        end
        object chkPetSCToMaster: TCheckBox
          Left = 84
          Top = 60
          Width = 65
          Height = 17
          Caption = #21472#21152#36947#26415
          TabOrder = 4
          OnClick = chkPetSCToMasterClick
        end
        object chkPetACToMaster: TCheckBox
          Left = 8
          Top = 80
          Width = 65
          Height = 17
          Caption = #21472#21152#38450#24481
          TabOrder = 5
          OnClick = chkPetACToMasterClick
        end
        object chkPetMACToMaster: TCheckBox
          Left = 84
          Top = 80
          Width = 65
          Height = 17
          Caption = #21472#21152#39764#38450
          TabOrder = 6
          OnClick = chkPetMACToMasterClick
        end
        object sePetAbilToMasterRate: TSpinEditEx
          Left = 64
          Top = 15
          Width = 86
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = sePetAbilToMasterRateChange
        end
      end
      object grp4: TGroupBox
        Left = 3
        Top = 2
        Width = 142
        Height = 77
        Caption = #23456#29289#36873#39033
        TabOrder = 0
        object chkOpenGamePet: TCheckBox
          Left = 7
          Top = 18
          Width = 97
          Height = 17
          Caption = #24320#21551#23456#29289#31995#32479
          TabOrder = 0
          OnClick = chkOpenGamePetClick
        end
        object chkPetNoEntity: TCheckBox
          Left = 7
          Top = 35
          Width = 122
          Height = 17
          Hint = #21246#36873#27492#36873#39033#21518#65292#23456#29289#19981#21344#22320#22270#20301#32622#65292#21487#38543#24847#31359#36807
          Caption = #23456#29289#26080#23454#20307#27169#24335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkPetNoEntityClick
        end
        object chkPetSleepControlBySlave: TCheckBox
          Left = 7
          Top = 54
          Width = 127
          Height = 17
          Caption = #23456#29289#21463#23453#23453#20241#24687#25511#21046
          TabOrder = 2
          OnClick = chkPetSleepControlBySlaveClick
        end
      end
      object btnSavePetParams: TButton
        Left = 570
        Top = 438
        Width = 79
        Height = 25
        Caption = #20445#23384
        TabOrder = 8
        OnClick = btnSavePetParamsClick
      end
      object grp5: TGroupBox
        Left = 3
        Top = 220
        Width = 142
        Height = 46
        Caption = #23456#29289#21507#33647#38388#38548
        TabOrder = 3
        object lbl7: TLabel
          Left = 8
          Top = 21
          Width = 36
          Height = 13
          Caption = #38388#38548#65306
        end
        object sePetUseItemIntervalTime: TSpinEditEx
          Left = 42
          Top = 18
          Width = 94
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = sePetUseItemIntervalTimeChange
        end
      end
      object GroupBox3: TGroupBox
        Left = 3
        Top = 270
        Width = 142
        Height = 88
        Caption = #26174#31034#21517#31216#35774#32622
        TabOrder = 4
        object Label164: TLabel
          Left = 8
          Top = 40
          Width = 36
          Height = 13
          Caption = #39068#33394#65306
        end
        object Label166: TLabel
          Left = 8
          Top = 65
          Width = 36
          Height = 13
          Caption = #21517#31216#65306
        end
        object chkPetShowMasterName: TCheckBox
          Left = 8
          Top = 16
          Width = 89
          Height = 17
          Caption = #26174#31034#20027#20154#21517#31216
          TabOrder = 0
          OnClick = chkPetShowMasterNameClick
        end
        object sePetNameColor: TColorIndexEdit
          Left = 42
          Top = 36
          Width = 93
          Height = 22
          Hint = #24403#20154#29289#25915#20987#20854#20182#20154#29289#26102#21517#23383#39068#33394#65292#40664#35748#20026'47'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = sePetNameColorChange
          ShowNoneColor = False
        end
        object edtPetSuffixName: TEdit
          Left = 42
          Top = 61
          Width = 93
          Height = 21
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentShowHint = False
          ShowHint = False
          TabOrder = 2
          Text = #30340#23456#29289
          OnChange = edtPetSuffixNameChange
        end
      end
      object grp7: TGroupBox
        Left = 152
        Top = 114
        Width = 166
        Height = 63
        Caption = #25429#25417#25216#33021#38656#35201#29289#21697
        TabOrder = 2
        object chkCapturePetNeedItem: TCheckBox
          Left = 8
          Top = 16
          Width = 153
          Height = 17
          Caption = #38656#35201'StdMode=94'#29289#21697
          TabOrder = 0
          OnClick = chkCapturePetNeedItemClick
        end
        object chkCaptureOKDecDura: TCheckBox
          Left = 8
          Top = 37
          Width = 145
          Height = 17
          Hint = #21246#36873#21017#25429#25417#25104#21151#25165#20943#25345#20037#65292#21542#21017#20351#29992#25216#33021#23601#20943#25345#20037
          Caption = #20165#22312#25429#25417#25104#21151#26102#20943#25345#20037
          TabOrder = 1
          OnClick = chkCaptureOKDecDuraClick
        end
      end
      object grp8: TGroupBox
        Left = 3
        Top = 361
        Width = 142
        Height = 64
        Caption = #25968#37327#38480#21046
        TabOrder = 6
        object lbl10: TLabel
          Left = 7
          Top = 19
          Width = 96
          Height = 13
          Caption = #23456#29289#24635#25968#37327#38480#21046#65306
        end
        object Label25: TLabel
          Left = 7
          Top = 41
          Width = 96
          Height = 13
          Caption = #21516#21517#23456#29289#25968#38480#21046#65306
        end
        object seGamePetMaxCount: TSpinEditEx
          Left = 92
          Top = 14
          Width = 45
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seGamePetMaxCountChange
        end
        object seGamePetNameCount: TSpinEditEx
          Left = 92
          Top = 36
          Width = 45
          Height = 22
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seGamePetNameCountChange
        end
      end
      object grp9: TGroupBox
        Left = 152
        Top = 179
        Width = 161
        Height = 43
        Caption = #23456#29289#27515#20129#21484#21796#38388#38548
        TabOrder = 7
        object lbl11: TLabel
          Left = 8
          Top = 20
          Width = 60
          Height = 13
          Caption = #21484#21796#38388#38548#65306
        end
        object lbl12: TLabel
          Left = 138
          Top = 20
          Width = 12
          Height = 13
          Caption = #31186
        end
        object seGamePetRecallTime: TSpinEditEx
          Left = 64
          Top = 15
          Width = 70
          Height = 22
          Hint = #21516#19968#20010#23456#29289#27515#20129#21518#65292#20877#27425#21484#21796#30340#38388#38548
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 0
          OnChange = seGamePetRecallTimeChange
        end
      end
      object grp10: TGroupBox
        Left = 152
        Top = 225
        Width = 166
        Height = 83
        Caption = #23456#29289#25915#20987#21450#30456#20851
        TabOrder = 5
        object chkEnabledPetAttack: TCheckBox
          Left = 7
          Top = 18
          Width = 97
          Height = 16
          Caption = #20801#35768#23456#29289#25915#20987
          TabOrder = 0
          OnClick = chkEnabledPetAttackClick
        end
        object chkDisableMonAttackPet: TCheckBox
          Left = 7
          Top = 38
          Width = 113
          Height = 16
          Caption = #24618#29289#19981#25915#20987#23456#29289
          TabOrder = 1
          OnClick = chkDisableMonAttackPetClick
        end
        object chkDisableAllAttackPet: TCheckBox
          Left = 7
          Top = 59
          Width = 113
          Height = 16
          Caption = #23456#29289#19981#21463#20219#20309#25915#20987
          TabOrder = 2
          OnClick = chkDisableAllAttackPetClick
        end
      end
      object chkPetNoShowHPProgress: TCheckBox
        Left = 152
        Top = 315
        Width = 113
        Height = 17
        Caption = #19981#26174#31034#23456#29289#34880#26465
        TabOrder = 9
        OnClick = chkPetNoShowHPProgressClick
      end
      object grp11: TGroupBox
        Left = 3
        Top = 80
        Width = 142
        Height = 138
        Caption = #23456#29289#25441#29289#36873#39033
        TabOrder = 10
        object chkEnabledPetPickup: TCheckBox
          Left = 7
          Top = 17
          Width = 122
          Height = 17
          Caption = #20801#35768#23456#29289#25441#21462#29289#21697
          TabOrder = 0
          OnClick = chkEnabledPetPickupClick
        end
        object chkPetPickupFullToMaster: TCheckBox
          Left = 7
          Top = 73
          Width = 130
          Height = 16
          Caption = #21253#35065#28385#26102#25918#20027#20154#21253#35065' '
          TabOrder = 3
          OnClick = chkPetPickupFullToMasterClick
        end
        object chkPetPickupToMaster: TCheckBox
          Left = 7
          Top = 54
          Width = 122
          Height = 17
          Hint = #19981#21246#36873#26102#65292#29289#21697#25918#21040#23456#29289#32972#21253#12290#21246#36873#21518#65292#21017#25918#21040#20027#20154#32972#21253
          Caption = #30452#25509#25441#21040#20027#20154#32972#21253
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = chkPetPickupToMasterClick
        end
        object chkPetQuickPickup: TCheckBox
          Left = 7
          Top = 92
          Width = 97
          Height = 17
          Caption = #20801#35768#24555#36895#25441#29289
          TabOrder = 4
          OnClick = chkPetQuickPickupClick
        end
        object chkPetRangePickup: TCheckBox
          Left = 7
          Top = 112
          Width = 69
          Height = 17
          Hint = #20351#29992#33539#22260#20869#25441#29289#21697#35831#24910#29992#25441#21462#35302#21457#65292#21487#33021#20250#24341#36215#28216#25103#29190#21345
          Caption = #33539#22260#25441#29289
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          OnClick = chkPetRangePickupClick
        end
        object sePetPickupRange: TSpinEditEx
          Left = 76
          Top = 109
          Width = 57
          Height = 22
          Hint = #20351#29992#33539#22260#20869#25441#29289#21697#35831#24910#29992#25441#21462#35302#21457#65292#21487#33021#20250#24341#36215#28216#25103#29190#21345
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 0
          OnChange = sePetPickupRangeChange
        end
        object chkPetOnlyPickMonsterItem: TCheckBox
          Left = 7
          Top = 35
          Width = 132
          Height = 17
          Hint = #21246#36873#27492#36873#39033#21518#65292#23456#29289#21482#20250#25441#24618#29289#29190#20986#30340#29289#21697#65307#24403#21246#36873#8220#20801#35768#23456#29289#25441#21462#29289#21697#8221#26102#26377#25928
          Caption = #23456#29289#21482#25441#24618#29289#29190#20986
          TabOrder = 1
          OnClick = chkPetOnlyPickMonsterItemClick
        end
      end
      object chkEnablePetUseClientPickItems: TCheckBox
        Left = 152
        Top = 336
        Width = 183
        Height = 17
        Hint = #23458#25143#31471#20869#25346#29289#21697#20013#8220#29305#27530#8221#20026#20248#20808#25441#36215#65292#8220#33258#21160#25342#21462#8221#20026#20801#35768#25441#36215
        Caption = #21516#27493#23458#25143#31471#20869#25346#25441#29289#37197#32622
        TabOrder = 11
        OnClick = chkEnablePetUseClientPickItemsClick
      end
      object chkGamePetKillMonTrigger: TCheckBox
        Left = 151
        Top = 357
        Width = 162
        Height = 17
        Caption = #26432#24618#35302#21457'[@GamePetKillMon]'
        TabOrder = 12
        OnClick = chkGamePetKillMonTriggerClick
      end
    end
  end
end
