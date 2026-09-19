object frmFunctionConfig: TfrmFunctionConfig
  Left = 299
  Top = 133
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #21151#33021#35774#32622
  ClientHeight = 461
  ClientWidth = 460
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  ShowHint = True
  OnCreate = FormCreate
  TextHeight = 12
  object Label14: TLabel
    Left = 8
    Top = 446
    Width = 432
    Height = 12
    Caption = #35843#25972#30340#21442#25968#31435#21363#29983#25928#65292#22312#32447#26102#35831#30830#35748#27492#21442#25968#30340#20316#29992#20877#35843#25972#65292#20081#35843#25972#23558#23548#33268#28216#25103#28151#20081
    Font.Charset = ANSI_CHARSET
    Font.Color = clRed
    Font.Height = -12
    Font.Name = #23435#20307
    Font.Style = []
    ParentFont = False
  end
  object FunctionConfigControl: TPageControl
    Left = 8
    Top = 8
    Width = 457
    Height = 430
    ActivePage = TabSheet41
    MultiLine = True
    TabOrder = 0
    OnChanging = FunctionConfigControlChanging
    object TabSheetGeneral: TTabSheet
      Caption = #22522#26412#21151#33021
      ImageIndex = 3
      object Label933: TLabel
        Left = 154
        Top = 322
        Width = 126
        Height = 12
        Caption = #27668#34880#30707#31867#21487#29992#34880#37327'/'#25345#20037
      end
      object grp51: TGroupBox
        Left = 152
        Top = 96
        Width = 292
        Height = 92
        Caption = #24187#39764#30707#35774#32622
        TabOrder = 7
        object Label139: TLabel
          Left = 161
          Top = 44
          Width = 66
          Height = 12
          Caption = #20943#25345#20037'('#28857#65289
        end
        object Label141: TLabel
          Left = 132
          Top = 43
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object Label143: TLabel
          Left = 6
          Top = 19
          Width = 144
          Height = 12
          Caption = #24320#21551#26465#20214' '#24403#21069'MP'#20302#20110'MaxMP'
        end
        object Label115: TLabel
          Left = 6
          Top = 44
          Width = 48
          Height = 12
          Caption = #24320#21551#38388#38548
        end
        object Label138: TLabel
          Left = 18
          Top = 68
          Width = 36
          Height = 12
          Caption = #22686#21152'MP'
        end
        object seMPRockDecValue: TSpinEditEx
          Left = 226
          Top = 39
          Width = 57
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seMPRockDecValueChange
        end
        object seMPRockAddValue: TSpinEditEx
          Left = 58
          Top = 63
          Width = 70
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seMPRockAddValueChange
        end
        object seMPRockTime: TSpinEditEx
          Left = 57
          Top = 39
          Width = 70
          Height = 21
          MaxValue = 600000
          MinValue = 100
          TabOrder = 2
          Value = 100
          OnChange = seMPRockTimeChange
        end
        object seMPRockRate: TSpinEditEx
          Left = 152
          Top = 15
          Width = 70
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 0
          OnChange = seMPRockRateChange
        end
        object cbbMPRockType: TComboBox
          Left = 226
          Top = 15
          Width = 57
          Height = 20
          Hint = '%'#65306#22914#21069#38754#22635'90'#65292#21017#34920#31034'MP <= MaxMP * 90%'#13#10#28857#65306#22914#21069#38754#22635'90'#65292#21017#34920#31034'MaxMP - MP >= 90'
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 4
          Text = '%'
          OnChange = cbbMPRockTypeChange
          Items.Strings = (
            '%'
            #28857)
        end
        object cbbMPRockAddType: TComboBox
          Left = 133
          Top = 64
          Width = 151
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 5
          Text = #24187#39764#30707#24403#21069#25345#20037'%'
          OnChange = cbbMPRockAddTypeChange
          Items.Strings = (
            #24187#39764#30707#24403#21069#25345#20037'%'
            #28857
            'MaxHP%'
            #24187#39764#30707#24635#25345#20037'%'
            #21333#27425#24674#22797#24187#39764#30707#24635#25345#20037#19975#20998#27604)
        end
      end
      object GroupBox7: TGroupBox
        Left = 8
        Top = 229
        Width = 137
        Height = 94
        Caption = #33021#37327#25511#21046
        TabOrder = 2
        object CheckBoxHungerSystem: TCheckBox
          Left = 8
          Top = 16
          Width = 121
          Height = 17
          Hint = #21551#29992#27492#21151#33021#21518#65292#20154#29289#24517#39035#23450#26102#21507#39135#29289#20197#20445#25345#33021#37327#65292#22914#26524#38271#26102#38388#26410#21507#39135#29289#65292#20154#29289#23558#34987#39295#27515#12290
          Caption = #21551#29992#33021#37327#25511#21046#31995#32479
          TabOrder = 0
          OnClick = CheckBoxHungerSystemClick
        end
        object GroupBoxHunger: TGroupBox
          Left = 8
          Top = 36
          Width = 113
          Height = 53
          Caption = #33021#37327#19981#22815#26102
          TabOrder = 1
          object CheckBoxHungerDecPower: TCheckBox
            Left = 6
            Top = 32
            Width = 97
            Height = 17
            Hint = #20154#29289#30340#25915#20987#21147#65292#19982#20154#29289#30340#33021#37327#30456#20851#65292#33021#37327#19981#22815#26102#20154#29289#30340#25915#20987#21147#23558#38543#20043#19979#38477#12290
            Caption = #33258#21160#20943#25915#20987#21147
            TabOrder = 1
            OnClick = CheckBoxHungerDecPowerClick
          end
          object CheckBoxHungerDecHP: TCheckBox
            Left = 6
            Top = 16
            Width = 89
            Height = 17
            Hint = #24403#20154#29289#38271#26102#38388#27809#21507#39135#29289#21518#33021#37327#38477#21040'0'#21518#65292#23558#24320#22987#33258#21160#20943#23569'HP'#20540#65292#38477#21040'0'#21518#65292#20154#29289#27515#20129#12290
            Caption = #33258#21160#20943'HP'
            TabOrder = 0
            OnClick = CheckBoxHungerDecHPClick
          end
        end
      end
      object ButtonGeneralSave: TButton
        Left = 376
        Top = 319
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 4
        OnClick = ButtonGeneralSaveClick
      end
      object GroupBox34: TGroupBox
        Left = 8
        Top = 0
        Width = 137
        Height = 225
        Caption = #21517#23383#26174#31034#39068#33394
        TabOrder = 0
        object Label85: TLabel
          Left = 11
          Top = 16
          Width = 54
          Height = 12
          Caption = #25915#20987#29366#24577':'
        end
        object Label87: TLabel
          Left = 11
          Top = 39
          Width = 54
          Height = 12
          Caption = #40644#21517#29366#24577':'
        end
        object Label89: TLabel
          Left = 11
          Top = 62
          Width = 54
          Height = 12
          Caption = #32418#21517#29366#24577':'
        end
        object Label91: TLabel
          Left = 11
          Top = 85
          Width = 54
          Height = 12
          Caption = #32852#30431#25112#20105':'
        end
        object Label93: TLabel
          Left = 11
          Top = 108
          Width = 54
          Height = 12
          Caption = #25932#23545#25112#20105':'
        end
        object Label95: TLabel
          Left = 11
          Top = 132
          Width = 54
          Height = 12
          Caption = #25112#20105#21306#22495':'
        end
        object Label113: TLabel
          Left = 11
          Top = 155
          Width = 48
          Height = 12
          Caption = 'NPC'#21517#23383':'
        end
        object Label738: TLabel
          Left = 11
          Top = 201
          Width = 54
          Height = 12
          Caption = #22823#20992#21517#23383':'
        end
        object Label756: TLabel
          Left = 11
          Top = 178
          Width = 54
          Height = 12
          Caption = #38613#20687#21517#23383':'
        end
        object EditPKFlagNameColor: TColorIndexEdit
          Left = 64
          Top = 12
          Width = 61
          Height = 21
          Hint = #24403#20154#29289#25915#20987#20854#20182#20154#29289#26102#21517#23383#39068#33394#65292#40664#35748#20026'47'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditPKFlagNameColorChange
          ShowNoneColor = False
        end
        object EditPKLevel1NameColor: TColorIndexEdit
          Left = 64
          Top = 35
          Width = 61
          Height = 21
          Hint = #24403#20154#29289'PK'#28857#36229#36807'100'#28857#26102#21517#23383#39068#33394#65292#40664#35748#20026'251'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditPKLevel1NameColorChange
          ShowNoneColor = False
        end
        object EditPKLevel2NameColor: TColorIndexEdit
          Left = 64
          Top = 58
          Width = 61
          Height = 21
          Hint = #24403#20154#29289'PK'#28857#36229#36807'200'#28857#26102#21517#23383#39068#33394#65292#40664#35748#20026'249'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = EditPKLevel2NameColorChange
          ShowNoneColor = False
        end
        object EditAllyAndGuildNameColor: TColorIndexEdit
          Left = 64
          Top = 81
          Width = 61
          Height = 21
          Hint = #24403#20154#29289#22312#34892#20250#25112#20105#26102#65292#26412#34892#20250#21450#32852#30431#34892#20250#20154#29289#21517#23383#39068#33394#65292#40664#35748#20026'180'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = EditAllyAndGuildNameColorChange
          ShowNoneColor = False
        end
        object EditWarGuildNameColor: TColorIndexEdit
          Left = 64
          Top = 104
          Width = 61
          Height = 21
          Hint = #24403#20154#29289#22312#34892#20250#25112#20105#26102#65292#25932#23545#34892#20250#20154#29289#21517#23383#39068#33394#65292#40664#35748#20026'69'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 4
          Value = 100
          OnChange = EditWarGuildNameColorChange
          ShowNoneColor = False
        end
        object EditInFreePKAreaNameColor: TColorIndexEdit
          Left = 64
          Top = 127
          Width = 61
          Height = 21
          Hint = #24403#20154#29289#22312#34892#20250#25112#20105#21306#22495#26102#20154#29289#21517#23383#39068#33394#65292#40664#35748#20026'221'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 5
          Value = 100
          OnChange = EditInFreePKAreaNameColorChange
          ShowNoneColor = False
        end
        object EditMerchantNameColor: TColorIndexEdit
          Left = 64
          Top = 150
          Width = 61
          Height = 21
          Hint = 'NPC'#21517#23383#39068#33394#65292#40664#35748#20026'250'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 6
          Value = 100
          OnChange = EditMerchantNameColorChange
          ShowNoneColor = False
        end
        object seGuardNameColor: TColorIndexEdit
          Left = 64
          Top = 196
          Width = 61
          Height = 21
          Hint = 'NPC'#21517#23383#39068#33394#65292#40664#35748#20026'250'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 7
          Value = 100
          OnChange = seGuardNameColorChange
          ShowNoneColor = False
        end
        object seMerchant273NameColor: TColorIndexEdit
          Left = 64
          Top = 173
          Width = 61
          Height = 21
          Hint = 'NPC'#21517#23383#39068#33394#65292#40664#35748#20026'250'
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 8
          Value = 100
          OnChange = seMerchant273NameColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox49: TGroupBox
        Left = 152
        Top = 193
        Width = 292
        Height = 120
        Caption = #39764#34880#30707#35774#32622
        TabOrder = 1
        object Label145: TLabel
          Left = 161
          Top = 43
          Width = 66
          Height = 12
          Caption = #20943#25345#20037'('#28857#65289
        end
        object Label147: TLabel
          Left = 132
          Top = 43
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object Label149: TLabel
          Left = 8
          Top = 19
          Width = 156
          Height = 12
          Caption = #24320#21551#26465#20214' '#24403#21069'HMP'#20302#20110'MaxHMP'
        end
        object Label142: TLabel
          Left = 6
          Top = 44
          Width = 48
          Height = 12
          Caption = #24320#21551#38388#38548
        end
        object Label144: TLabel
          Left = 12
          Top = 68
          Width = 42
          Height = 12
          Caption = #22686#21152'HMP'
        end
        object lbl165: TLabel
          Left = 172
          Top = 95
          Width = 108
          Height = 12
          Caption = #35814#32454#35828#26126#21442#38405#35828#26126#20070
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object seHMPRockDecValue: TSpinEditEx
          Left = 226
          Top = 39
          Width = 57
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 0
          OnChange = seHMPRockDecValueChange
        end
        object seHMPRockAddValue: TSpinEditEx
          Left = 58
          Top = 64
          Width = 70
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
          OnChange = seHMPRockAddValueChange
        end
        object seHMPRockTime: TSpinEditEx
          Left = 58
          Top = 39
          Width = 70
          Height = 21
          MaxValue = 600000
          MinValue = 100
          TabOrder = 1
          Value = 100
          OnChange = seHMPRockTimeChange
        end
        object seHMPRockRate: TSpinEditEx
          Left = 166
          Top = 15
          Width = 56
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seHMPRockRateChange
        end
        object cbbHMPRockType: TComboBox
          Left = 226
          Top = 15
          Width = 57
          Height = 20
          Hint = '%'#65306#22914#21069#38754#22635'90'#65292#21017#34920#31034'HMP <= MaxHMP * 90%'#13#10#28857#65306#22914#21069#38754#22635'90'#65292#21017#34920#31034'MaxHMP - HMP >= 90'
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 4
          Text = '%'
          OnChange = cbbHMPRockTypeChange
          Items.Strings = (
            '%'
            #28857)
        end
        object cbbHMPRockAddType: TComboBox
          Left = 133
          Top = 64
          Width = 151
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 5
          Text = #39764#34880#30707#24403#21069#25345#20037'%'
          OnChange = cbbHMPRockAddTypeChange
          Items.Strings = (
            #39764#34880#30707#24403#21069#25345#20037'%'
            #28857
            'MaxHP%'
            #39764#34880#30707#24635#25345#20037'%'
            #21333#27425#24674#22797#39764#34880#30707#24635#25345#20037#19975#20998#27604)
        end
        object chkHMPUse2Times: TCheckBox
          Left = 8
          Top = 92
          Width = 121
          Height = 17
          Hint = 
            #21246#36873#65306#22914#26524'HP'#28385#36275#26465#20214#20351#29992'1'#27425#21152'HP'#65292#22914#26524'MP'#28385#36275#26465#20214#20877#20351#29992'1'#27425#21152'MP'#13#10#19981#21246#65306#22914#26524'HP'#25110'MP'#26377'1'#20010#28385#36275#26465#20214#65292#20351#29992#19968#27425#21152'HP'#12289'M' +
            'P'
          Caption = #20998#27425#20351#29992#21152'HP'#65292'MP'
          TabOrder = 6
          OnClick = chkHMPUse2TimesClick
        end
      end
      object CheckBoxDropOverLapItem: TCheckBox
        Left = 8
        Top = 344
        Width = 297
        Height = 16
        Caption = #25172#21472#21152#29289#21697#26102#65292#20840#37096#25172#25481#12290#19981#26174#31034#36755#20837#25968#37327#23545#35805#26694
        TabOrder = 5
        OnClick = CheckBoxDropOverLapItemClick
      end
      object CheckBoxOpenMapEvent: TCheckBox
        Left = 8
        Top = 325
        Width = 121
        Height = 17
        Caption = #21551#29992#22320#22270#20107#20214#35302#21457
        TabOrder = 3
        OnClick = CheckBoxOpenMapEventClick
      end
      object grp50: TGroupBox
        Left = 152
        Top = 0
        Width = 292
        Height = 92
        Caption = #27668#34880#30707#35774#32622
        TabOrder = 6
        object Label114: TLabel
          Left = 6
          Top = 19
          Width = 144
          Height = 12
          Caption = #24320#21551#26465#20214' '#24403#21069'HP'#20302#20110'MaxHP'
        end
        object Label136: TLabel
          Left = 161
          Top = 44
          Width = 66
          Height = 12
          Caption = #20943#25345#20037'('#28857#65289
        end
        object Label137: TLabel
          Left = 6
          Top = 44
          Width = 48
          Height = 12
          Caption = #24320#21551#38388#38548
        end
        object lbl111: TLabel
          Left = 132
          Top = 44
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object lbl138: TLabel
          Left = 18
          Top = 68
          Width = 36
          Height = 12
          Caption = #22686#21152'HP'
        end
        object seHPRockRate: TSpinEditEx
          Left = 152
          Top = 15
          Width = 70
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seHPRockRateChange
        end
        object seHPRockTime: TSpinEditEx
          Left = 60
          Top = 39
          Width = 70
          Height = 21
          MaxValue = 600000
          MinValue = 100
          TabOrder = 1
          Value = 100
          OnChange = seHPRockTimeChange
        end
        object seHPRockAddValue: TSpinEditEx
          Left = 60
          Top = 64
          Width = 70
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
          OnChange = seHPRockAddValueChange
        end
        object seHPRockDecValue: TSpinEditEx
          Left = 226
          Top = 39
          Width = 57
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 0
          OnChange = seHPRockDecValueChange
        end
        object cbbHPRockAddType: TComboBox
          Left = 133
          Top = 64
          Width = 151
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 4
          Text = #27668#34880#30707#24403#21069#25345#20037'%'
          OnChange = cbbHPRockAddTypeChange
          Items.Strings = (
            #27668#34880#30707#24403#21069#25345#20037'%'
            #28857
            'MaxHP%'
            #27668#34880#30707#24635#25345#20037'%'
            #21333#27425#24674#22797#27668#34880#30707#24635#25345#20037#19975#20998#27604)
        end
        object cbbHPRockType: TComboBox
          Left = 226
          Top = 15
          Width = 57
          Height = 20
          Hint = '%'#65306#22914#21069#38754#22635'90'#65292#21017#34920#31034'HP <= MaxHP * 90%'#13#10#28857#65306#22914#21069#38754#22635'90'#65292#21017#34920#31034'MaxHP - HP >= 90'
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 5
          Text = '%'
          OnChange = cbbHPRockTypeChange
          Items.Strings = (
            '%'
            #28857)
        end
      end
      object seHMPDivDura: TSpinEditEx
        Left = 282
        Top = 318
        Width = 63
        Height = 21
        Hint = #37197#32622#20026'10'#65292#27668#34880#30707#25345#20037#20026'1000'#26102#65292#34880#37327#20026#65306'1000*10=10000'#28857#34880
        MaxValue = 10000
        MinValue = 1
        TabOrder = 8
        Value = 1
        OnChange = seHMPDivDuraChange
      end
    end
    object PasswordSheet: TTabSheet
      Caption = #23494#30721#20445#25252
      ImageIndex = 2
      object GroupBox1: TGroupBox
        Left = 8
        Top = 0
        Width = 433
        Height = 249
        TabOrder = 0
        object CheckBoxEnablePasswordLock: TCheckBox
          Left = 8
          Top = -5
          Width = 121
          Height = 25
          Caption = #21551#29992#23494#30721#20445#25252#31995#32479
          TabOrder = 0
          OnClick = CheckBoxEnablePasswordLockClick
        end
        object GroupBox2: TGroupBox
          Left = 8
          Top = 16
          Width = 265
          Height = 185
          Caption = #38145#23450#25511#21046
          TabOrder = 1
          object CheckBoxLockGetBackItem: TCheckBox
            Left = 8
            Top = 16
            Width = 121
            Height = 17
            Caption = #31105#27490#21462#20179#24211#21830#21697
            TabOrder = 0
            OnClick = CheckBoxLockGetBackItemClick
          end
          object GroupBox4: TGroupBox
            Left = 8
            Top = 40
            Width = 249
            Height = 137
            Caption = #30331#24405#38145#23450
            TabOrder = 1
            object CheckBoxLockWalk: TCheckBox
              Left = 8
              Top = 32
              Width = 98
              Height = 17
              Caption = #31105#27490#36208#36335
              TabOrder = 2
              OnClick = CheckBoxLockWalkClick
            end
            object CheckBoxLockRun: TCheckBox
              Left = 8
              Top = 49
              Width = 98
              Height = 17
              Caption = #31105#27490#36305#27493
              TabOrder = 4
              OnClick = CheckBoxLockRunClick
            end
            object CheckBoxLockHit: TCheckBox
              Left = 8
              Top = 66
              Width = 98
              Height = 17
              Caption = #31105#27490#25915#20987
              TabOrder = 6
              OnClick = CheckBoxLockHitClick
            end
            object CheckBoxLockSpell: TCheckBox
              Left = 8
              Top = 82
              Width = 98
              Height = 17
              Caption = #31105#27490#39764#27861
              TabOrder = 8
              OnClick = CheckBoxLockSpellClick
            end
            object CheckBoxLockSendMsg: TCheckBox
              Left = 112
              Top = 32
              Width = 133
              Height = 17
              Caption = #31105#27490#32842#22825
              TabOrder = 3
              OnClick = CheckBoxLockSendMsgClick
            end
            object CheckBoxLockInObMode: TCheckBox
              Left = 112
              Top = 16
              Width = 133
              Height = 17
              Hint = #22914#26524#26377#23494#30721#20445#25252#26102#65292#20154#29289#30331#24405#26102#20026#38544#36523#29366#24577#65292#24618#29289#19981#20250#25915#20987#20154#29289#65292#22312#36755#20837#23494#30721#24320#38145#21518#24674#22797#27491#24120#12290
              Caption = #38145#23450#26102#20026#38544#36523#27169#24335
              TabOrder = 1
              OnClick = CheckBoxLockInObModeClick
            end
            object CheckBoxLockLogin: TCheckBox
              Left = 8
              Top = 16
              Width = 98
              Height = 17
              Caption = #38145#23450#20154#29289#30331#24405
              TabOrder = 0
              OnClick = CheckBoxLockLoginClick
            end
            object CheckBoxLockUseItem: TCheckBox
              Left = 112
              Top = 82
              Width = 133
              Height = 17
              Caption = #31105#27490#20351#29992#21697
              TabOrder = 9
              OnClick = CheckBoxLockUseItemClick
            end
            object CheckBoxLockDropItem: TCheckBox
              Left = 112
              Top = 66
              Width = 133
              Height = 17
              Caption = #31105#27490#25172#29289#21697
              TabOrder = 7
              OnClick = CheckBoxLockDropItemClick
            end
            object CheckBoxLockDealItem: TCheckBox
              Left = 112
              Top = 49
              Width = 133
              Height = 17
              Caption = #31105#27490#20132#26131#29289#21697
              TabOrder = 5
              OnClick = CheckBoxLockDealItemClick
            end
            object chkLockSummonHero: TCheckBox
              Left = 112
              Top = 98
              Width = 133
              Height = 17
              Caption = #31105#27490#21484#21796#33521#38596
              TabOrder = 11
              OnClick = chkLockSummonHeroClick
            end
            object chkLockShop: TCheckBox
              Left = 8
              Top = 115
              Width = 98
              Height = 17
              Caption = #31105#27490#21830#38138
              TabOrder = 12
              OnClick = chkLockShopClick
            end
            object chkLockChallenge: TCheckBox
              Left = 8
              Top = 98
              Width = 98
              Height = 17
              Caption = #31105#27490#25361#25112
              TabOrder = 10
              OnClick = chkLockChallengeClick
            end
            object chkLockStall: TCheckBox
              Left = 112
              Top = 115
              Width = 133
              Height = 17
              Hint = #36873#25321#35813#39033#21518#38145#23450#26102#20154#29289#19981#33021#36827#34892#25670#25674#21644#20010#20154#21830#24215
              Caption = #31105#27490#25670#25674#12289#20010#20154#21830#24215
              TabOrder = 13
              OnClick = chkLockStallClick
            end
          end
        end
        object GroupBox3: TGroupBox
          Left = 280
          Top = 16
          Width = 145
          Height = 65
          Caption = #23494#30721#36755#20837#38169#35823#25511#21046
          TabOrder = 2
          object Label1: TLabel
            Left = 8
            Top = 18
            Width = 54
            Height = 12
            Caption = #38169#35823#27425#25968':'
          end
          object EditErrorPasswordCount: TSpinEditEx
            Left = 68
            Top = 15
            Width = 53
            Height = 21
            Hint = #22312#24320#38145#26102#36755#20837#23494#30721#65292#22914#26524#36755#20837#38169#35823#36229#36807#25351#23450#27425#25968#65292#21017#38145#23450#23494#30721#65292#24517#39035#37325#26032#30331#24405#19968#27425#25165#21487#20197#20877#27425#36755#20837#23494#30721#12290
            MaxValue = 10
            MinValue = 1
            TabOrder = 0
            Value = 10
            OnChange = EditErrorPasswordCountChange
          end
          object CheckBoxErrorCountKick: TCheckBox
            Left = 8
            Top = 40
            Width = 129
            Height = 17
            Caption = #36229#36807#25351#23450#27425#25968#36386#19979#32447
            Enabled = False
            TabOrder = 1
            OnClick = CheckBoxErrorCountKickClick
          end
        end
        object ButtonPasswordLockSave: TButton
          Left = 360
          Top = 213
          Width = 65
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 3
          OnClick = ButtonPasswordLockSaveClick
        end
      end
    end
    object TabSheet32: TTabSheet
      Caption = #32467#23130#31995#32479
      ImageIndex = 4
    end
    object TabSheet33: TTabSheet
      Caption = #24072#24466#31995#32479
      ImageIndex = 5
      object GroupBox21: TGroupBox
        Left = 8
        Top = 8
        Width = 161
        Height = 153
        Caption = #24466#24351#20986#24072
        TabOrder = 0
        object GroupBox22: TGroupBox
          Left = 8
          Top = 16
          Width = 145
          Height = 49
          Caption = #20986#24072#31561#32423
          TabOrder = 0
          object Label29: TLabel
            Left = 8
            Top = 18
            Width = 54
            Height = 12
            Caption = #20986#24072#31561#32423':'
          end
          object EditMasterOKLevel: TSpinEditEx
            Left = 68
            Top = 15
            Width = 53
            Height = 21
            Hint = #20986#24072#31561#32423#35774#32622#65292#20154#29289#22312#25308#24072#21518#65292#21040#25351#23450#31561#32423#21518#23558#33258#21160#20986#24072#12290
            MaxValue = 65535
            MinValue = 1
            TabOrder = 0
            Value = 10
            OnChange = EditMasterOKLevelChange
          end
        end
        object GroupBox23: TGroupBox
          Left = 8
          Top = 72
          Width = 145
          Height = 73
          Caption = #24072#29238#25152#24471
          TabOrder = 1
          object Label30: TLabel
            Left = 8
            Top = 18
            Width = 54
            Height = 12
            Caption = #22768#26395#28857#25968':'
          end
          object Label31: TLabel
            Left = 8
            Top = 42
            Width = 54
            Height = 12
            Caption = #20998#37197#28857#25968':'
          end
          object EditMasterOKCreditPoint: TSpinEditEx
            Left = 68
            Top = 15
            Width = 53
            Height = 21
            Hint = #24466#24351#20986#24072#21518#65292#24072#29238#24471#21040#30340#22768#26395#28857#25968#12290
            MaxValue = 100
            MinValue = 0
            TabOrder = 0
            Value = 10
            OnChange = EditMasterOKCreditPointChange
          end
          object EditMasterOKBonusPoint: TSpinEditEx
            Left = 68
            Top = 39
            Width = 53
            Height = 21
            Hint = #24466#24351#20986#24072#21518#65292#24072#29238#24471#21040#30340#20998#37197#28857#25968#12290
            MaxValue = 1000
            MinValue = 0
            TabOrder = 1
            Value = 10
            OnChange = EditMasterOKBonusPointChange
          end
        end
      end
      object ButtonMasterSave: TButton
        Left = 360
        Top = 157
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonMasterSaveClick
      end
      object grp52: TGroupBox
        Left = 176
        Top = 8
        Width = 113
        Height = 41
        Caption = #24466#24351#25968#37327
        TabOrder = 2
        object Label710: TLabel
          Left = 8
          Top = 18
          Width = 42
          Height = 12
          Caption = #25910#24466#25968':'
        end
        object seMasterCount: TSpinEditEx
          Left = 52
          Top = 15
          Width = 53
          Height = 21
          Hint = #20801#35768#25910#24466#24351#25968#37327
          MaxValue = 65535
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seMasterCountChange
        end
      end
    end
    object TabSheet38: TTabSheet
      Caption = #36716#29983#31995#32479
      ImageIndex = 9
      object GroupBox29: TGroupBox
        Left = 8
        Top = 8
        Width = 113
        Height = 257
        Caption = #33258#21160#21464#33394
        TabOrder = 0
        object Label56: TLabel
          Left = 11
          Top = 16
          Width = 18
          Height = 12
          Caption = #19968':'
        end
        object Label58: TLabel
          Left = 11
          Top = 40
          Width = 18
          Height = 12
          Caption = #20108':'
        end
        object Label60: TLabel
          Left = 11
          Top = 64
          Width = 18
          Height = 12
          Caption = #19977':'
        end
        object Label62: TLabel
          Left = 11
          Top = 88
          Width = 18
          Height = 12
          Caption = #22235':'
        end
        object Label64: TLabel
          Left = 11
          Top = 112
          Width = 18
          Height = 12
          Caption = #20116':'
        end
        object Label66: TLabel
          Left = 11
          Top = 136
          Width = 18
          Height = 12
          Caption = #20845':'
        end
        object Label68: TLabel
          Left = 11
          Top = 160
          Width = 18
          Height = 12
          Caption = #19971':'
        end
        object Label70: TLabel
          Left = 11
          Top = 184
          Width = 18
          Height = 12
          Caption = #20843':'
        end
        object Label72: TLabel
          Left = 11
          Top = 208
          Width = 18
          Height = 12
          Caption = #20061':'
        end
        object Label74: TLabel
          Left = 11
          Top = 232
          Width = 18
          Height = 12
          Caption = #21313':'
        end
        object EditReNewNameColor1: TColorIndexEdit
          Left = 40
          Top = 12
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditReNewNameColor1Change
          ShowNoneColor = False
        end
        object EditReNewNameColor2: TColorIndexEdit
          Left = 40
          Top = 36
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditReNewNameColor2Change
          ShowNoneColor = False
        end
        object EditReNewNameColor3: TColorIndexEdit
          Left = 40
          Top = 60
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = EditReNewNameColor3Change
          ShowNoneColor = False
        end
        object EditReNewNameColor4: TColorIndexEdit
          Left = 40
          Top = 84
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = EditReNewNameColor4Change
          ShowNoneColor = False
        end
        object EditReNewNameColor5: TColorIndexEdit
          Left = 40
          Top = 108
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 4
          Value = 100
          OnChange = EditReNewNameColor5Change
          ShowNoneColor = False
        end
        object EditReNewNameColor6: TColorIndexEdit
          Left = 40
          Top = 132
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 5
          Value = 100
          OnChange = EditReNewNameColor6Change
          ShowNoneColor = False
        end
        object EditReNewNameColor7: TColorIndexEdit
          Left = 40
          Top = 156
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 6
          Value = 100
          OnChange = EditReNewNameColor7Change
          ShowNoneColor = False
        end
        object EditReNewNameColor8: TColorIndexEdit
          Left = 40
          Top = 180
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 7
          Value = 100
          OnChange = EditReNewNameColor8Change
          ShowNoneColor = False
        end
        object EditReNewNameColor9: TColorIndexEdit
          Left = 40
          Top = 204
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 8
          Value = 100
          OnChange = EditReNewNameColor9Change
          ShowNoneColor = False
        end
        object EditReNewNameColor10: TColorIndexEdit
          Left = 40
          Top = 228
          Width = 62
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 9
          Value = 100
          OnChange = EditReNewNameColor10Change
          ShowNoneColor = False
        end
      end
      object ButtonReNewLevelSave: TButton
        Left = 360
        Top = 157
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 3
        OnClick = ButtonReNewLevelSaveClick
      end
      object GroupBox30: TGroupBox
        Left = 128
        Top = 8
        Width = 105
        Height = 65
        Caption = #21517#23383#21464#33394
        TabOrder = 1
        object Label57: TLabel
          Left = 8
          Top = 42
          Width = 30
          Height = 12
          Caption = #38388#38548':'
        end
        object Label59: TLabel
          Left = 83
          Top = 44
          Width = 12
          Height = 12
          Caption = #31186
        end
        object EditReNewNameColorTime: TSpinEditEx
          Left = 44
          Top = 39
          Width = 37
          Height = 21
          MaxValue = 10
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditReNewNameColorTimeChange
        end
        object CheckBoxReNewChangeColor: TCheckBox
          Left = 8
          Top = 16
          Width = 89
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#36716#29983#30340#20154#29289#30340#21517#23383#39068#33394#20250#33258#21160#21464#21270#12290
          Caption = #33258#21160#21464#33394
          TabOrder = 0
          OnClick = CheckBoxReNewChangeColorClick
        end
      end
      object GroupBox33: TGroupBox
        Left = 128
        Top = 80
        Width = 105
        Height = 41
        Caption = #36716#29983#25511#21046
        TabOrder = 2
        object CheckBoxReNewLevelClearExp: TCheckBox
          Left = 8
          Top = 16
          Width = 89
          Height = 17
          Hint = #36716#29983#26102#26159#21542#28165#38500#24050#32463#26377#30340#32463#39564#20540#12290
          Caption = #28165#38500#24050#26377#32463#39564
          TabOrder = 0
          OnClick = CheckBoxReNewLevelClearExpClick
        end
      end
    end
    object TabSheet39: TTabSheet
      Caption = #23453#23453#35774#32622
      ImageIndex = 10
      object ButtonMonUpgradeSave: TButton
        Left = 380
        Top = 336
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 8
        OnClick = ButtonMonUpgradeSaveClick
      end
      object GroupBox32: TGroupBox
        Left = 5
        Top = 0
        Width = 92
        Height = 239
        Caption = #31561#32423#39068#33394
        TabOrder = 0
        object Label65: TLabel
          Left = 8
          Top = 42
          Width = 18
          Height = 12
          Caption = #19968':'
        end
        object Label67: TLabel
          Left = 8
          Top = 64
          Width = 18
          Height = 12
          Caption = #20108':'
        end
        object Label69: TLabel
          Left = 8
          Top = 86
          Width = 18
          Height = 12
          Caption = #19977':'
        end
        object Label71: TLabel
          Left = 8
          Top = 108
          Width = 18
          Height = 12
          Caption = #22235':'
        end
        object Label73: TLabel
          Left = 8
          Top = 130
          Width = 18
          Height = 12
          Caption = #20116':'
        end
        object Label75: TLabel
          Left = 8
          Top = 152
          Width = 18
          Height = 12
          Caption = #20845':'
        end
        object Label76: TLabel
          Left = 8
          Top = 174
          Width = 18
          Height = 12
          Caption = #19971':'
        end
        object Label77: TLabel
          Left = 8
          Top = 196
          Width = 18
          Height = 12
          Caption = #20843':'
        end
        object Label86: TLabel
          Left = 8
          Top = 218
          Width = 18
          Height = 12
          Caption = #20061':'
        end
        object Label140: TLabel
          Left = 8
          Top = 19
          Width = 18
          Height = 12
          Caption = #38646':'
        end
        object EditMonUpgradeColor1: TColorIndexEdit
          Tag = 1
          Left = 27
          Top = 37
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor2: TColorIndexEdit
          Tag = 2
          Left = 27
          Top = 59
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor3: TColorIndexEdit
          Tag = 3
          Left = 27
          Top = 81
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor4: TColorIndexEdit
          Tag = 4
          Left = 27
          Top = 103
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 4
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor5: TColorIndexEdit
          Tag = 5
          Left = 27
          Top = 125
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 5
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor6: TColorIndexEdit
          Tag = 6
          Left = 27
          Top = 147
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 6
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor7: TColorIndexEdit
          Tag = 7
          Left = 27
          Top = 169
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 7
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor8: TColorIndexEdit
          Tag = 8
          Left = 27
          Top = 191
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 8
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor9: TColorIndexEdit
          Tag = 9
          Left = 27
          Top = 213
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 9
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
        object EditMonUpgradeColor0: TColorIndexEdit
          Left = 27
          Top = 15
          Width = 58
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditMonUpgradeColor1Change
          ShowNoneColor = False
        end
      end
      object GroupBox31: TGroupBox
        Left = 101
        Top = 0
        Width = 84
        Height = 238
        Caption = #21319#32423#26432#24618#25968
        TabOrder = 1
        object Label61: TLabel
          Left = 7
          Top = 19
          Width = 18
          Height = 12
          Caption = #19968':'
        end
        object Label63: TLabel
          Left = 7
          Top = 42
          Width = 18
          Height = 12
          Caption = #20108':'
        end
        object Label78: TLabel
          Left = 7
          Top = 64
          Width = 18
          Height = 12
          Caption = #19977':'
        end
        object Label79: TLabel
          Left = 7
          Top = 86
          Width = 18
          Height = 12
          Caption = #22235':'
        end
        object Label80: TLabel
          Left = 7
          Top = 108
          Width = 18
          Height = 12
          Caption = #20116':'
        end
        object Label81: TLabel
          Left = 7
          Top = 130
          Width = 18
          Height = 12
          Caption = #20845':'
        end
        object Label82: TLabel
          Left = 7
          Top = 152
          Width = 18
          Height = 12
          Caption = #19971':'
        end
        object Label83: TLabel
          Left = 7
          Top = 174
          Width = 30
          Height = 12
          Caption = #22522#25968':'
        end
        object Label84: TLabel
          Left = 7
          Top = 196
          Width = 30
          Height = 12
          Caption = #20493#29575':'
        end
        object EditMonUpgradeKillCount1: TSpinEditEx
          Left = 36
          Top = 15
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditMonUpgradeKillCount1Change
        end
        object EditMonUpgradeKillCount2: TSpinEditEx
          Left = 36
          Top = 37
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditMonUpgradeKillCount2Change
        end
        object EditMonUpgradeKillCount3: TSpinEditEx
          Left = 36
          Top = 59
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = EditMonUpgradeKillCount3Change
        end
        object EditMonUpgradeKillCount4: TSpinEditEx
          Left = 36
          Top = 81
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = EditMonUpgradeKillCount4Change
        end
        object EditMonUpgradeKillCount5: TSpinEditEx
          Left = 36
          Top = 103
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 4
          Value = 100
          OnChange = EditMonUpgradeKillCount5Change
        end
        object EditMonUpgradeKillCount6: TSpinEditEx
          Left = 36
          Top = 125
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 5
          Value = 100
          OnChange = EditMonUpgradeKillCount6Change
        end
        object EditMonUpgradeKillCount7: TSpinEditEx
          Left = 36
          Top = 147
          Width = 41
          Height = 21
          Increment = 10
          MaxValue = 9999
          MinValue = 0
          TabOrder = 6
          Value = 100
          OnChange = EditMonUpgradeKillCount7Change
        end
        object EditMonUpLvNeedKillBase: TSpinEditEx
          Left = 36
          Top = 169
          Width = 41
          Height = 21
          Hint = #26432#24618#25968#37327'='#31561#32423' * '#20493#29575' + '#31561#32423' + '#22522#25968' + '#27599#32423#25968#37327
          MaxValue = 9999
          MinValue = 0
          TabOrder = 7
          Value = 100
          OnChange = EditMonUpLvNeedKillBaseChange
        end
        object EditMonUpLvRate: TSpinEditEx
          Left = 36
          Top = 191
          Width = 41
          Height = 21
          Hint = #26432#24618#25968#37327'='#24618#29289#31561#32423' * '#20493#29575' + '#31561#32423' + '#22522#25968' + '#27599#32423#25968#37327
          MaxValue = 9999
          MinValue = 0
          TabOrder = 8
          Value = 100
          OnChange = EditMonUpLvRateChange
        end
      end
      object GroupBox35: TGroupBox
        Left = 190
        Top = 0
        Width = 110
        Height = 113
        Caption = #20027#20154#27515#20129#25511#21046
        TabOrder = 2
        object Label88: TLabel
          Left = 6
          Top = 40
          Width = 54
          Height = 12
          Caption = #21464#24322#26426#29575':'
        end
        object Label90: TLabel
          Left = 6
          Top = 64
          Width = 54
          Height = 12
          Caption = #22686#21152#25915#20987':'
        end
        object Label92: TLabel
          Left = 6
          Top = 88
          Width = 54
          Height = 12
          Caption = #22686#21152#36895#24230':'
        end
        object CheckBoxMasterDieMutiny: TCheckBox
          Left = 8
          Top = 16
          Width = 96
          Height = 17
          Caption = #20027#20154#27515#21518#21464#24322
          TabOrder = 0
          OnClick = CheckBoxMasterDieMutinyClick
        end
        object EditMasterDieMutinyRate: TSpinEditEx
          Left = 62
          Top = 36
          Width = 41
          Height = 21
          Hint = #25968#23383#36234#23567#65292#21464#24322#26426#29575#36234#22823#12290
          MaxValue = 9999
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditMasterDieMutinyRateChange
        end
        object EditMasterDieMutinyPower: TSpinEditEx
          Left = 62
          Top = 60
          Width = 41
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = EditMasterDieMutinyPowerChange
        end
        object EditMasterDieMutinySpeed: TSpinEditEx
          Left = 62
          Top = 84
          Width = 41
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = EditMasterDieMutinySpeedChange
        end
      end
      object GroupBox47: TGroupBox
        Left = 190
        Top = 119
        Width = 110
        Height = 65
        Caption = #19971#24425#23453#23453
        TabOrder = 4
        object Label112: TLabel
          Left = 6
          Top = 40
          Width = 54
          Height = 12
          Caption = #26102#38388#38388#38548':'
        end
        object CheckBoxBBMonAutoChangeColor: TCheckBox
          Left = 8
          Top = 16
          Width = 97
          Height = 17
          Caption = #23453#23453#33258#21160#21464#33394
          TabOrder = 0
          OnClick = CheckBoxBBMonAutoChangeColorClick
        end
        object EditBBMonAutoChangeColorTime: TSpinEditEx
          Left = 62
          Top = 36
          Width = 41
          Height = 21
          Hint = #25968#23383#36234#23567#65292#21464#33394#36895#24230#36234#24555#65292#21333#20301'('#31186')'#12290
          MaxValue = 9999
          MinValue = 1
          TabOrder = 1
          Value = 100
          OnChange = EditBBMonAutoChangeColorTimeChange
        end
      end
      object GroupBox103: TGroupBox
        Left = 190
        Top = 190
        Width = 110
        Height = 48
        Caption = #23453#23453#25915#20987#23041#21147#20493#29575
        TabOrder = 5
        object Label221: TLabel
          Left = 6
          Top = 18
          Width = 54
          Height = 12
          Caption = #23041#21147#20493#25968':'
        end
        object EditSlavePowerRate: TSpinEditEx
          Left = 62
          Top = 14
          Width = 42
          Height = 21
          Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
          MaxValue = 10000
          MinValue = 1
          TabOrder = 0
          Value = 100
          OnChange = EditSlavePowerRateChange
        end
      end
      object GroupBox181: TGroupBox
        Left = 333
        Top = 240
        Width = 115
        Height = 91
        Caption = #23453#23453#20854#20182#36873#39033
        TabOrder = 7
        object CheckBoxMasterRoyaltyDie: TCheckBox
          Left = 6
          Top = 14
          Width = 107
          Height = 17
          Caption = #21467#21464#21518#31435#21363#27515#20129
          TabOrder = 0
          OnClick = CheckBoxMasterRoyaltyDieClick
        end
        object CheckBoxSlaveRelaxCanStruck: TCheckBox
          Left = 6
          Top = 30
          Width = 107
          Height = 17
          Caption = #23453#23453#20241#24687#26102#26080#25932
          ParentShowHint = False
          ShowHint = False
          TabOrder = 1
          OnClick = CheckBoxSlaveRelaxCanStruckClick
        end
        object chkSlaveAlwaysShowName: TCheckBox
          Left = 6
          Top = 47
          Width = 97
          Height = 17
          Hint = #22312#23458#25143#31471#20869#25346#24320#21551#20102#8220#24618#29289#26174#21517#8221#21518#65306#13#10#13#10#21246#36873#21017#19968#30452#26174#31034#23453#23453#21517#23383#65292#19981#21246#36873#21017#21482#26377#40736#26631#31227#19978#21435#25165#26174#21517
          Caption = #23453#23453#22987#32456#26174#21517
          TabOrder = 2
          OnClick = chkSlaveAlwaysShowNameClick
        end
        object chkMasterRoyaltyFullHP: TCheckBox
          Left = 6
          Top = 65
          Width = 97
          Height = 17
          Hint = #21246#36873#21518#65292#23453#23453#21467#21464#21518#19981#20250#20943#34880
          Caption = #21467#21464#21518#19981#20943#34880
          TabOrder = 3
          OnClick = chkMasterRoyaltyFullHPClick
        end
      end
      object GroupBox210: TGroupBox
        Left = 5
        Top = 240
        Width = 115
        Height = 83
        Caption = #23453#23453#25915#20987#36873#39033
        TabOrder = 6
        object chkSlaveNotAttackHuman: TCheckBox
          Left = 6
          Top = 16
          Width = 107
          Height = 17
          Caption = #23453#23453#19981#25915#20987#20154#29289
          ParentShowHint = False
          ShowHint = False
          TabOrder = 0
          OnClick = chkSlaveNotAttackHumanClick
        end
        object chkSlaveNotAttackHero: TCheckBox
          Left = 6
          Top = 32
          Width = 107
          Height = 17
          Caption = #23453#23453#19981#25915#20987#33521#38596
          ParentShowHint = False
          ShowHint = False
          TabOrder = 1
          OnClick = chkSlaveNotAttackHeroClick
        end
        object chkSlaveLockTarget: TCheckBox
          Left = 6
          Top = 48
          Width = 107
          Height = 17
          Hint = #21246#36873#21518#28216#25103#20869#24555#25463#38190#21487#25511#21046#23453#23453#25915#20987#30446#26631
          Caption = 'Ctrl+R'#38145#23450#30446#26631
          TabOrder = 2
          OnClick = chkSlaveLockTargetClick
        end
        object chkSlaveNoLockHuman: TCheckBox
          Left = 5
          Top = 64
          Width = 107
          Height = 17
          Hint = #21246#36873#21518#23453#23453#19981#20801#35768#20351#29992#24555#25463#38190#38145#23450#20154#29289#21644#33521#38596
          Caption = #31105#27490#38145#23450#20154#29289
          TabOrder = 3
          OnClick = chkSlaveNoLockHumanClick
        end
      end
      object grp53: TGroupBox
        Left = 304
        Top = 0
        Width = 145
        Height = 238
        Caption = #23453#23453#23646#24615#21472#21152
        TabOrder = 3
        object GroupBox217: TGroupBox
          Left = 6
          Top = 35
          Width = 133
          Height = 79
          Caption = #21472#21152#21040#23453#23453#25915#20987'DC'
          TabOrder = 1
          object Label711: TLabel
            Left = 8
            Top = 57
            Width = 54
            Height = 12
            Caption = #22522#30784#20493#29575':'
          end
          object seBBAttrPlusAddAttackRate: TSpinEditEx
            Left = 64
            Top = 53
            Width = 62
            Height = 21
            Hint = 
              #23646#24615#21472#21152#21442#25968#65306' '#22522#30784#20493#29575#38500#20197'100'#20026#23454#38469#20493#25968#13#10#26080#24378#21270#25216#33021#26102#65306#21472#21152#23646#24615' = '#22522#30784#20493#29575' * '#20154#29289#23646#24615#13#10#24378#21270#25216#33021#26102#26102#65306#21472#21152#23646#24615' =' +
              ' '#22522#30784#20493#29575' * '#20154#29289#23646#24615' *  '#24378#21270#25216#33021#20493#25968
            MaxValue = 9999
            MinValue = 0
            TabOrder = 2
            Value = 100
            OnChange = seBBAttrPlusAddAttackRateChange
          end
          object cbbBBAttrPlusAddAttackForm: TComboBox
            Left = 8
            Top = 32
            Width = 118
            Height = 20
            Style = csDropDownList
            TabOrder = 1
            OnChange = cbbBBAttrPlusAddAttackFormChange
            Items.Strings = (
              #26681#25454#20154#29289#32844#19994#23450
              #26681#25454#20154#29289'DC'#21472#21152
              #26681#25454#20154#29289'MC'#21472#21152
              #26681#25454#20154#29289'SC'#21472#21152)
          end
          object chkBBAttrPlusAddAttack: TCheckBox
            Left = 8
            Top = 14
            Width = 122
            Height = 17
            Hint = #21472#21152#20027#20154#30340#19979#38480#21644#19978#38480#23646#24615#32473#23453#23453#65292#21442#25968#20462#25913#21518#23567#36864#29983#25928
            Caption = #21152#20154#29289#25915#20987#32473#23453#23453':'
            TabOrder = 0
            OnClick = chkBBAttrPlusAddAttackClick
          end
        end
        object GroupBox218: TGroupBox
          Left = 6
          Top = 118
          Width = 133
          Height = 56
          Caption = #21472#21152#20154#29289#38450#24481#32473#23453#23453
          TabOrder = 2
          object Label712: TLabel
            Left = 8
            Top = 35
            Width = 54
            Height = 12
            Caption = #22522#30784#20493#29575':'
          end
          object chkBBAttrPlusAddDefence: TCheckBox
            Left = 8
            Top = 14
            Width = 58
            Height = 18
            Hint = #21472#21152#20027#20154#30340#19979#38480#21644#19978#38480#23646#24615#32473#23453#23453#65292#21442#25968#20462#25913#21518#23567#36864#29983#25928
            Caption = #21152#38450#24481
            TabOrder = 0
            OnClick = chkBBAttrPlusAddDefenceClick
          end
          object chkBBAttrPlusAddMagicDefence: TCheckBox
            Left = 69
            Top = 14
            Width = 58
            Height = 18
            Hint = #21472#21152#20027#20154#30340#19979#38480#21644#19978#38480#23646#24615#32473#23453#23453#65292#21442#25968#20462#25913#21518#23567#36864#29983#25928
            Caption = #21152#39764#24481
            TabOrder = 1
            OnClick = chkBBAttrPlusAddMagicDefenceClick
          end
          object seBBAttrPlusAddDefenceRate: TSpinEditEx
            Left = 64
            Top = 31
            Width = 62
            Height = 21
            Hint = 
              #23646#24615#21472#21152#21442#25968#65306' '#22522#30784#20493#29575#38500#20197'100'#20026#23454#38469#20493#25968#13#10#26080#24378#21270#25216#33021#26102#65306#21472#21152#23646#24615' = '#22522#30784#20493#29575' * '#20154#29289#23646#24615#13#10#24378#21270#25216#33021#26102#26102#65306#21472#21152#23646#24615' =' +
              ' '#22522#30784#20493#29575' * '#20154#29289#23646#24615' *  '#24378#21270#25216#33021#20493#25968
            MaxValue = 9999
            MinValue = 0
            TabOrder = 2
            Value = 100
            OnChange = seBBAttrPlusAddDefenceRateChange
          end
        end
        object GroupBox219: TGroupBox
          Left = 6
          Top = 176
          Width = 133
          Height = 56
          Caption = #21472#21152#20154#29289'HP'#32473#23453#23453
          TabOrder = 3
          object Label713: TLabel
            Left = 8
            Top = 34
            Width = 54
            Height = 12
            Caption = #22522#30784#20493#29575':'
          end
          object chkBBAttrPlusAddHP: TCheckBox
            Left = 8
            Top = 14
            Width = 97
            Height = 17
            Hint = #21472#21152#20027#20154'MAXHP'#32473#23453#23453#65292#21442#25968#20462#25913#21518#23567#36864#29983#25928
            Caption = #22686#21152#23453#23453'HP'
            TabOrder = 0
            OnClick = chkBBAttrPlusAddHPClick
          end
          object seBBAttrPlusAddHPRate: TSpinEditEx
            Left = 64
            Top = 30
            Width = 62
            Height = 21
            Hint = 
              #23646#24615#21472#21152#21442#25968#65306' '#22522#30784#20493#29575#38500#20197'100'#20026#23454#38469#20493#25968#13#10#26080#24378#21270#25216#33021#26102#65306#21472#21152#23646#24615' = '#22522#30784#20493#29575' * '#20154#29289#23646#24615#13#10#24378#21270#25216#33021#26102#26102#65306#21472#21152#23646#24615' =' +
              ' '#22522#30784#20493#29575' * '#20154#29289#23646#24615' *  '#24378#21270#25216#33021#20493#25968
            MaxValue = 9999
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seBBAttrPlusAddHPRateChange
          end
        end
        object chkBBAttrPlusAddOnlyMagic: TCheckBox
          Left = 9
          Top = 16
          Width = 129
          Height = 17
          Hint = #21246#36873#26102#65292#21482#26377#36947#22763#25216#33021#21484#21796#39607#39621#12289#21484#21796#31070#20861#25165#20250#21472#21152#23646#24615#13#10#19981#21246#36873#65292#19981#31649#26159#33050#26412#21629#20196#65292#36824#26159#20219#20309#25216#33021#21484#21796#20986#26469#30340'BB'#37117#20250#21472#21152#23646#24615
          Caption = #21482#38480#21484#21796#39607#39621#21644#31070#20861
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
          TabOrder = 0
          OnClick = chkBBAttrPlusAddOnlyMagicClick
        end
      end
      object GroupBox231: TGroupBox
        Left = 125
        Top = 240
        Width = 204
        Height = 120
        Caption = #27599#21319#19968#32423#22686#21152#23646#24615
        TabOrder = 9
        object Label784: TLabel
          Left = 8
          Top = 38
          Width = 24
          Height = 12
          Caption = #34880#37327
        end
        object lbl127: TLabel
          Left = 8
          Top = 60
          Width = 24
          Height = 12
          Caption = #38450#24481
        end
        object lbl128: TLabel
          Left = 8
          Top = 82
          Width = 24
          Height = 12
          Caption = #39764#38450
        end
        object lbl129: TLabel
          Left = 94
          Top = 38
          Width = 24
          Height = 12
          Caption = #25915#20987
        end
        object lbl130: TLabel
          Left = 94
          Top = 60
          Width = 48
          Height = 12
          Caption = #31227#21160#36895#24230
        end
        object lbl131: TLabel
          Left = 94
          Top = 82
          Width = 48
          Height = 12
          Caption = #25915#20987#36895#24230
        end
        object seSlave9HP: TSpinEditEx
          Left = 35
          Top = 34
          Width = 53
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seSlave9HPChange
        end
        object seSlave9AC: TSpinEditEx
          Left = 35
          Top = 56
          Width = 53
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seSlave9ACChange
        end
        object seSlave9MAC: TSpinEditEx
          Left = 35
          Top = 78
          Width = 53
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = seSlave9MACChange
        end
        object seSlave9DC: TSpinEditEx
          Left = 145
          Top = 34
          Width = 53
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = seSlave9DCChange
        end
        object seSlave9MoveSpeed: TSpinEditEx
          Left = 145
          Top = 56
          Width = 53
          Height = 21
          Hint = #25968#25454#24211#36895#24230' - ('#24618#29289#31561#32423' * '#27599#32423#25552#21319')'
          MaxValue = 9999
          MinValue = 0
          TabOrder = 4
          Value = 100
          OnChange = seSlave9MoveSpeedChange
        end
        object seSlave9HitSpeed: TSpinEditEx
          Left = 145
          Top = 78
          Width = 53
          Height = 21
          Hint = #25968#25454#24211#36895#24230' - ('#24618#29289#31561#32423' * '#27599#32423#25552#21319')'
          MaxValue = 9999
          MinValue = 0
          TabOrder = 5
          Value = 100
          OnChange = seSlave9HitSpeedChange
        end
        object chkSlaveLevelupUseNewAttr: TCheckBox
          Left = 8
          Top = 15
          Width = 188
          Height = 17
          Hint = #21551#29992#21518#19979#38754#22635#20889#30340#25165#26377#25928#65292#19981#21551#29992#34920#31034#20351#29992#31995#32479#40664#35748#35745#31639#35268#21017#13#10#13#10#21246#36873#21518#65292#33509#31070#20861#12289#26032#39607#39621#31561#24618#29289#36895#24230#19981#23545#65292#21487#35843#25972#25968#25454#24211#36895#24230
          Caption = #21551#29992#26032#30340#23453#23453#21319#32423#23646#24615#35745#31639#35268#21017
          TabOrder = 6
          OnClick = chkSlaveLevelupUseNewAttrClick
        end
        object chkSlaveLevelupAddLowerAttr: TCheckBox
          Left = 8
          Top = 100
          Width = 169
          Height = 17
          Hint = 
            #37325#26032#21484#21796#23453#23453#25110#23453#23453#21319#32423#29983#25928#13#10#13#10#26410#21246#36873#26102#65292#23453#23453#21319#32423#21482#22686#21152#25915#20987'/'#38450#24481'/'#39764#38450#30340#19978#38480#23646#24615#13#10#13#10#21246#36873#21518#65292#23453#23453#21319#32423#21516#26102#22686#21152#25915#20987'/'#38450#24481'/' +
            #39764#38450#30340#19979#38480#21450#19978#38480#23646#24615
          Caption = #21516#26102#22686#21152#19979#38480#23646#24615
          TabOrder = 7
          OnClick = chkSlaveLevelupAddLowerAttrClick
        end
      end
      object chkSlaveKillHumanIncPK: TCheckBox
        Left = 4
        Top = 326
        Width = 117
        Height = 17
        Caption = #23453#23453#26432#20154#20027#20154#21152'PK'
        TabOrder = 10
        OnClick = chkSlaveKillHumanIncPKClick
      end
      object chkSlaveDisableStruck: TCheckBox
        Left = 4
        Top = 342
        Width = 105
        Height = 17
        Hint = #23453#23453#34987#25915#20987#21518#26159#21542#26174#31034#21518#20208#21160#20316
        Caption = #23453#23453#26080#21518#20208#21160#20316
        TabOrder = 11
        OnClick = chkSlaveDisableStruckClick
      end
    end
    object MonSaySheet: TTabSheet
      Caption = #24618#29289#35828#35805
      object GroupBox40: TGroupBox
        Left = 8
        Top = 8
        Width = 137
        Height = 49
        Caption = #24618#29289#35828#35805
        TabOrder = 0
        object CheckBoxMonSayMsg: TCheckBox
          Left = 8
          Top = 16
          Width = 97
          Height = 17
          Caption = #24320#21551#24618#29289#35828#35805
          TabOrder = 0
          OnClick = CheckBoxMonSayMsgClick
        end
      end
      object ButtonMonSayMsgSave: TButton
        Left = 376
        Top = 277
        Width = 65
        Height = 20
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonMonSayMsgSaveClick
      end
    end
    object TabSheet1: TTabSheet
      Caption = #25216#33021#39764#27861
      ImageIndex = 1
      object ButtonSkillSave: TButton
        Left = 380
        Top = 342
        Width = 64
        Height = 21
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonSkillSaveClick
      end
      object MagicPageControl: TPageControl
        Left = 0
        Top = -3
        Width = 449
        Height = 341
        ActivePage = TabSheet61
        MultiLine = True
        TabOrder = 0
        object TabSheet61: TTabSheet
          Caption = #25216#33021#21442#25968
          object GroupBox17: TGroupBox
            Left = 2
            Top = 1
            Width = 145
            Height = 81
            Caption = #39764#27861#25915#20987#33539#22260#38480#21046
            TabOrder = 0
            object Label12: TLabel
              Left = 8
              Top = 18
              Width = 54
              Height = 12
              Caption = #33539#22260#22823#23567':'
            end
            object EditMagicAttackRage: TSpinEditEx
              Left = 68
              Top = 15
              Width = 53
              Height = 21
              Hint = #39764#27861#25915#20987#26377#25928#36317#31163#65292#36229#36807#25351#23450#36317#31163#25915#20987#26080#25928#12290
              MaxValue = 20
              MinValue = 1
              TabOrder = 0
              Value = 10
              OnChange = EditMagicAttackRageChange
            end
            object CheckBoxViewRangeCanMagicAttack: TCheckBox
              Left = 8
              Top = 38
              Width = 129
              Height = 17
              Hint = #22312#23631#24149#20869#21487#20197#30475#21040#30340#24618#29289#65292#39764#27861#37117#21487#20197#25915#20987#21040#12290
              Caption = #26681#25454#35282#33394#30340#35270#35273#33539#22260
              TabOrder = 1
              OnClick = CheckBoxViewRangeCanMagicAttackClick
            end
            object chkShowMsgMagicRangeExceed: TCheckBox
              Left = 8
              Top = 58
              Width = 121
              Height = 17
              Hint = #21246#36873#21518#22914#39764#27861#25915#20987#36317#31163#19981#22815#28216#25103#20013#23558#25552#31034#29609#23478
              Caption = #39764#27861#25915#20987#33539#22260#25552#31034
              TabOrder = 2
              OnClick = chkShowMsgMagicRangeExceedClick
            end
          end
          object GroupBox53: TGroupBox
            Left = 154
            Top = 1
            Width = 121
            Height = 41
            Caption = #26377#25928#26102#38388#20493#25968
            TabOrder = 1
            object Label117: TLabel
              Left = 8
              Top = 19
              Width = 36
              Height = 12
              Caption = #20493#25968#65306
            end
            object Label877: TLabel
              Left = 106
              Top = 19
              Width = 6
              Height = 12
              Caption = '%'
            end
            object SpinEditMagDelayTime: TSpinEditEx
              Left = 42
              Top = 14
              Width = 60
              Height = 21
              Hint = #12304#26410#23436#25104#12305#21482#23545#22914#39764#27861#30462#65292#28779#22681#20043#31867#30340#39764#27861#26377#19968#23450#30340#24310#26102#26102#38388#30340#39764#27861#26377#25928#12290#13#39764#27861#26377#25928#26102#38388#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = SpinEditMagDelayTimeChange
            end
          end
          object GroupBox80: TGroupBox
            Left = 2
            Top = 83
            Width = 145
            Height = 42
            Caption = #27602#31526#25345#20037#27604#20363
            TabOrder = 3
            object lbl1: TLabel
              Left = 8
              Top = 18
              Width = 36
              Height = 12
              Caption = '1'#25345#20037'='
            end
            object EditMagicItemRate: TSpinEditEx
              Left = 44
              Top = 15
              Width = 69
              Height = 21
              Hint = 'StdItems'#20013#30340'DuraMax'#30340#28857#25968
              MaxValue = 10000
              MinValue = 1
              TabOrder = 0
              Value = 10
              OnChange = EditMagicItemRateChange
            end
          end
          object RadioGroupHumNeedMagicItem: TRadioGroup
            Left = 2
            Top = 126
            Width = 145
            Height = 65
            Hint = 
              '1'#65306#36523#19978#25110#21253#35065#20013#37117#19981#38656#35201#31526#25110#27602#65292#23601#21487#20197#30452#25509#20351#29992#39764#27861#13'2'#65306#38656#35201#36523#19978#20329#25140#31526#25110#27602#65292#25165#21487#20197#20351#29992#39764#27861#13'3'#65306#39318#20808#20351#29992#36523#19978#20329#25140#31526#25110#27602#65292#22914#26524#36523#19978 +
              #27809#26377#20329#25140#65292#23601#20351#29992#21253#35065#20013#30340#31526#25110#27602#12290
            Caption = #36947#22763#25216#33021#35774#32622#19968
            Items.Strings = (
              #19981#38656#35201#31526#25110#27602
              #20351#29992#20329#25140#30340#31526#25110#27602
              #20351#29992#21253#35065#20013#30340#31526#25110#27602)
            TabOrder = 4
            OnClick = RadioGroupHumNeedMagicItemClick
          end
          object GroupBox92: TGroupBox
            Left = 2
            Top = 193
            Width = 145
            Height = 93
            Caption = #36947#22763#25216#33021#35774#32622#20108
            TabOrder = 5
            object Label919: TLabel
              Left = 8
              Top = 69
              Width = 84
              Height = 12
              Caption = #21484#21796#23453#23453#24635#25968#65306
            end
            object chkRecallManySlave1: TCheckBox
              Left = 8
              Top = 16
              Width = 132
              Height = 17
              Caption = #21516#26102#21484#21796#39607#39621#21644#31070#20861
              TabOrder = 0
              OnClick = chkRecallManySlave1Click
            end
            object chkRecallManySlave2: TCheckBox
              Left = 8
              Top = 32
              Width = 132
              Height = 17
              Caption = #21516#26102#21484#21796#26376#28789#21644#22307#20861
              TabOrder = 1
              OnClick = chkRecallManySlave2Click
            end
            object chkRecallManySlave3: TCheckBox
              Left = 8
              Top = 48
              Width = 132
              Height = 17
              Caption = #21516#26102#21484#21796#31070#20861#21644#22307#20861
              TabOrder = 2
              OnClick = chkRecallManySlave3Click
            end
            object seRecallMonCount: TSpinEditEx
              Left = 88
              Top = 65
              Width = 46
              Height = 21
              Hint = #38480#23450#21484#21796#25216#33021#26102#23453#23453#24635#25968#37327#65288'0'#20026#19981#38480#21046#65289#65292#36229#36807#25968#37327#26102#20572#27490#21484#21796
              MaxValue = 100
              MinValue = 0
              TabOrder = 3
              Value = 1
              OnChange = seRecallMonCountChange
            end
          end
          object GroupBox104: TGroupBox
            Left = 154
            Top = 87
            Width = 121
            Height = 41
            Caption = #39764#27861#38145#23450
            TabOrder = 2
            object Label222: TLabel
              Left = 8
              Top = 19
              Width = 36
              Height = 12
              Caption = #33539#22260#65306
            end
            object Label878: TLabel
              Left = 104
              Top = 19
              Width = 12
              Height = 12
              Caption = #26684
            end
            object EditMagicLockRange: TSpinEditEx
              Left = 42
              Top = 15
              Width = 60
              Height = 21
              Hint = #24403#21069#25915#20987#30340#22352#26631#33539#22260#20869#26597#25214#38145#23450#30446#26631#65292#25968#23383#36234#22823#65292#36234#23481#26131#38145#23450
              MaxValue = 12
              MinValue = 0
              TabOrder = 0
              Value = 1
              OnChange = EditMagicLockRangeChange
            end
          end
          object chkMagicNotHinder: TCheckBox
            Left = 153
            Top = 228
            Width = 113
            Height = 17
            Hint = #24320#21551#27492#21151#33021#21518#65292#31867#20284#28789#39746#28779#31526#12289#22823#28779#29699#31561#23558#19981#20877#21463#38556#30861#29289#24433#21709#32780#25171#19981#20013
            Caption = #39764#27861#24573#30053#38556#30861
            TabOrder = 6
            OnClick = chkMagicNotHinderClick
          end
          object chkMagicDefinition: TCheckBox
            Left = 153
            Top = 248
            Width = 113
            Height = 17
            Hint = #21246#36873#21518#23558#22823#24133#22686#21152#25915#20987#31227#21160#30446#26631#30340#21629#20013#20960#29575
            Caption = #25552#39640#39764#27861#31934#30830#24230
            TabOrder = 7
            OnClick = chkMagicDefinitionClick
          end
          object GroupBox222: TGroupBox
            Left = 154
            Top = 43
            Width = 121
            Height = 41
            Caption = #36817#36523#25915#20987#23041#21147#20493#25968
            TabOrder = 8
            object Label735: TLabel
              Left = 8
              Top = 19
              Width = 36
              Height = 12
              Caption = #20493#25968#65306
            end
            object lbl143: TLabel
              Left = 106
              Top = 19
              Width = 6
              Height = 12
              Caption = '%'
            end
            object seNearAttackPowerRate: TSpinEditEx
              Left = 42
              Top = 15
              Width = 60
              Height = 21
              Hint = #39764#27861#23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seNearAttackPowerRateChange
            end
          end
          object GroupBox112: TGroupBox
            Left = 280
            Top = 1
            Width = 157
            Height = 171
            Caption = #25216#33021#30456#20851#25552#31034
            TabOrder = 9
            object Label773: TLabel
              Left = 12
              Top = 109
              Width = 36
              Height = 12
              Caption = 'X'#22352#26631':'
            end
            object Label774: TLabel
              Left = 12
              Top = 131
              Width = 36
              Height = 12
              Caption = 'Y'#22352#26631':'
            end
            object Label875: TLabel
              Left = 12
              Top = 19
              Width = 54
              Height = 12
              Caption = #22833#36133#25991#23383':'
            end
            object Label876: TLabel
              Left = 12
              Top = 42
              Width = 54
              Height = 12
              Caption = #22833#36133#32972#26223':'
            end
            object Label879: TLabel
              Left = 12
              Top = 64
              Width = 54
              Height = 12
              Caption = #25104#21151#25991#23383':'
            end
            object Label880: TLabel
              Left = 12
              Top = 87
              Width = 54
              Height = 12
              Caption = #25104#21151#32972#26223':'
            end
            object seMagicMsgX: TSpinEditEx
              Left = 48
              Top = 104
              Width = 46
              Height = 21
              Hint = #25216#33021#20919#21364#26102#38388#26410#21040#26102#65292#25552#31034#20449#24687#30340'x'#22352#26631
              MaxValue = 0
              MinValue = 0
              TabOrder = 0
              Value = 1
              OnChange = seMagicMsgXChange
            end
            object seMagicMsgY: TSpinEditEx
              Left = 48
              Top = 126
              Width = 46
              Height = 21
              Hint = #25216#33021#20919#21364#26102#38388#26410#21040#26102#65292#25552#31034#20449#24687#30340'y'#22352#26631
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 1
              OnChange = seMagicMsgYChange
            end
            object seMagicFailMsgFColor: TColorIndexEdit
              Left = 69
              Top = 15
              Width = 80
              Height = 21
              MaxLength = 3
              MaxValue = 255
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seMagicFailMsgFColorChange
              ShowNoneColor = False
            end
            object seMagicFailMsgBColor: TColorIndexEdit
              Left = 69
              Top = 37
              Width = 80
              Height = 21
              MaxLength = 3
              MaxValue = 255
              MinValue = 0
              TabOrder = 3
              Value = 100
              OnChange = seMagicFailMsgBColorChange
              ShowNoneColor = False
            end
            object chkMagicMsgAddChatBoardMsg: TCheckBox
              Left = 12
              Top = 148
              Width = 100
              Height = 19
              Caption = #22312#32842#22825#26694#26174#31034' '
              TabOrder = 4
              OnClick = chkMagicMsgAddChatBoardMsgClick
            end
            object chkMagicMsgXRightToLeft: TCheckBox
              Left = 95
              Top = 109
              Width = 55
              Height = 18
              Caption = #20174#21491#31639
              TabOrder = 5
              OnClick = chkMagicMsgXRightToLeftClick
            end
            object seMagicOKMsgFColor: TColorIndexEdit
              Left = 69
              Top = 60
              Width = 80
              Height = 21
              MaxLength = 3
              MaxValue = 255
              MinValue = 0
              TabOrder = 6
              Value = 100
              OnChange = seMagicOKMsgFColorChange
              ShowNoneColor = False
            end
            object seMagicOKMsgBColor: TColorIndexEdit
              Left = 69
              Top = 82
              Width = 80
              Height = 21
              MaxLength = 3
              MaxValue = 255
              MinValue = 0
              TabOrder = 7
              Value = 100
              OnChange = seMagicOKMsgBColorChange
              ShowNoneColor = False
            end
            object chkMagicMsgYBottomToTop: TCheckBox
              Left = 95
              Top = 128
              Width = 54
              Height = 18
              Caption = #20174#19979#31639
              TabOrder = 8
              OnClick = chkMagicMsgYBottomToTopClick
            end
          end
          object GroupBox115: TGroupBox
            Left = 154
            Top = 132
            Width = 121
            Height = 93
            Caption = #20154#29289'4'#32423#25216#33021#23041#21147'+'
            TabOrder = 10
            object Label920: TLabel
              Left = 8
              Top = 22
              Width = 60
              Height = 12
              Caption = #28872#28779#21073#27861#65306
            end
            object Label921: TLabel
              Left = 20
              Top = 46
              Width = 48
              Height = 12
              Caption = #28781#22825#28779#65306
            end
            object Label922: TLabel
              Left = 8
              Top = 70
              Width = 60
              Height = 12
              Caption = #28789#39746#28779#31526#65306
            end
            object edtHumSkill7PowerLV4: TSpinEditEx
              Left = 65
              Top = 17
              Width = 48
              Height = 21
              Hint = #22312#21407#26469#25216#33021#30340#22522#30784#19978#22686#21152#30340#26432#20260#21147#30340#30334#20998#27604#13#10#21482#23545#20154#29289#30340'4'#32423#28872#28779#21073#27861#26377#25928
              MaxValue = 100
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 10
              OnChange = edtHumSkill7PowerLV4Change
            end
            object edtHumSkill45PowerLV4: TSpinEditEx
              Left = 65
              Top = 41
              Width = 48
              Height = 21
              Hint = #22312#21407#26469#25216#33021#30340#22522#30784#19978#22686#21152#30340#26432#20260#21147#30340#30334#20998#27604#13#10#21482#23545#20154#29289#30340'4'#32423#28781#22825#28779#26377#25928
              MaxValue = 100
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              Value = 10
              OnChange = edtHumSkill45PowerLV4Change
            end
            object edtHumSkill13PowerLV4: TSpinEditEx
              Left = 65
              Top = 65
              Width = 48
              Height = 21
              Hint = #22312#21407#26469#25216#33021#30340#22522#30784#19978#22686#21152#30340#26432#20260#21147#30340#30334#20998#27604#13#10#21482#23545#20154#29289#30340'4'#32423#28789#39746#28779#31526#26377#25928
              MaxValue = 100
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 10
              OnChange = edtHumSkill13PowerLV4Change
            end
          end
          object grp48: TGroupBox
            Left = 280
            Top = 174
            Width = 157
            Height = 85
            Caption = #23553#25112#22763#25216#33021#36830#25918
            TabOrder = 11
            object lbl161: TLabel
              Left = 12
              Top = 39
              Width = 60
              Height = 12
              Caption = #36830#25918#25216#33021#65306
            end
            object lbl162: TLabel
              Left = 12
              Top = 61
              Width = 60
              Height = 12
              Caption = #26368#23569#38388#38548#65306
            end
            object lbl163: TLabel
              Left = 126
              Top = 61
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object chkDisableWarrContinueHit: TCheckBox
              Left = 12
              Top = 16
              Width = 133
              Height = 17
              Hint = #21246#36873#21518#23545#25216#33021#36827#34892#24310#26102#26045#23637
              Caption = #24320#21551#23553#25216#33021#36830#25918#21151#33021
              TabOrder = 0
              OnClick = chkDisableWarrContinueHitClick
            end
            object edtDisableWarrContinueHitIDs: TEdit
              Left = 72
              Top = 35
              Width = 77
              Height = 20
              Hint = #36755#20837#25216#33021#25968#25454#24211'ID'#65292#22810#20010#25216#33021#20043#38388#20197','#20998#38548
              TabOrder = 1
              Text = '26,56,66'
              OnChange = edtDisableWarrContinueHitIDsChange
            end
            object seWarrContinueHitMinInterval: TSpinEditEx
              Left = 72
              Top = 57
              Width = 50
              Height = 21
              Hint = #40664#35748#20540'1000'#65292#25216#33021#21516#26102#20919#21364#21518#65292#26045#23637#25216#33021#38388#38548#26102#38388#65292#24320#21551#23553#25216#33021#36830#25918#21518#29983#25928
              MaxValue = 5000
              MinValue = 100
              TabOrder = 2
              Value = 100
              OnChange = seWarrContinueHitMinIntervalChange
            end
          end
        end
        object TabSheet62: TTabSheet
          Caption = #25112#22763#25216#33021
          ImageIndex = 1
          object PageControl4: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            ActivePage = ts3
            Align = alClient
            MultiLine = True
            TabOrder = 0
            object ts3: TTabSheet
              Caption = #21313#27493#19968#26432
              ImageIndex = 11
              object grp6: TGroupBox
                Left = 195
                Top = 8
                Width = 142
                Height = 217
                Caption = #25216#33021#25928#26524
                TabOrder = 1
                object chkSkill204MbAttackMon: TCheckBox
                  Left = 6
                  Top = 16
                  Width = 107
                  Height = 13
                  Caption = #20801#35768#40635#30201#24618#29289
                  TabOrder = 0
                  OnClick = chkSkill204MbAttackMonClick
                end
                object chkSkill204MbAttackHuman: TCheckBox
                  Left = 6
                  Top = 35
                  Width = 107
                  Height = 13
                  Caption = #20801#35768#40635#30201#20154#29289
                  TabOrder = 1
                  OnClick = chkSkill204MbAttackHumanClick
                end
                object chkSkill204MbAttackSlave: TCheckBox
                  Left = 6
                  Top = 54
                  Width = 127
                  Height = 13
                  Caption = #20801#35768#40635#30201#23453#23453#12289#33521#38596
                  TabOrder = 2
                  OnClick = chkSkill204MbAttackSlaveClick
                end
                object chkSkill204MbFastParalysis: TCheckBox
                  Left = 6
                  Top = 73
                  Width = 123
                  Height = 13
                  Hint = #24403#20154#29289#25110#24618#29289#34987#35813#25216#33021#40635#30201#21518#65292#26159#21542#34987#25915#20987#39532#19978#21462#28040#40635#30201#29366#24577#12290
                  Caption = #26159#21542#24555#36895#35299#38500#40635#30201
                  TabOrder = 3
                  OnClick = chkSkill204MbFastParalysisClick
                end
                object chkSkill204RunGuard: TCheckBox
                  Left = 6
                  Top = 149
                  Width = 99
                  Height = 13
                  Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
                  Caption = #20801#35768#31359#36807#23432#21355
                  TabOrder = 7
                  OnClick = chkSkill204RunGuardClick
                end
                object chkSkill204RunNpc: TCheckBox
                  Left = 6
                  Top = 130
                  Width = 99
                  Height = 13
                  Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807'NPC'
                  Caption = #20801#35768#31359#36807'NPC'
                  TabOrder = 6
                  OnClick = chkSkill204RunNpcClick
                end
                object chkSkill204RunMon: TCheckBox
                  Left = 6
                  Top = 111
                  Width = 99
                  Height = 13
                  Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#24618#29289
                  Caption = #20801#35768#31359#36807#24618#29289
                  TabOrder = 5
                  OnClick = chkSkill204RunMonClick
                end
                object chkSkill204RunHum: TCheckBox
                  Left = 6
                  Top = 92
                  Width = 98
                  Height = 13
                  Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#20854#20182#20154#29289
                  Caption = #20801#35768#31359#36807#20154#29289
                  TabOrder = 4
                  OnClick = chkSkill204RunHumClick
                end
                object chkSkill204WarDisHumRun: TCheckBox
                  Left = 6
                  Top = 187
                  Width = 127
                  Height = 13
                  Hint = #25171#24320#27492#21151#33021#21518#65292#22312#25915#22478#21306#22495#25915#30340#22478#26102#27573#20840#37096#31105#27490
                  Caption = #25915#22478#26102#27573'('#21306#22495')'#20840#31105
                  TabOrder = 8
                  OnClick = chkSkill204WarDisHumRunClick
                end
                object chkSkill204RunObstacle: TCheckBox
                  Left = 6
                  Top = 168
                  Width = 111
                  Height = 13
                  Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
                  Caption = #20801#35768#31359#36807#38556#30861#29289
                  TabOrder = 9
                  OnClick = chkSkill204RunObstacleClick
                end
              end
              object grp7: TGroupBox
                Left = 10
                Top = 8
                Width = 175
                Height = 241
                Caption = #25216#33021#21442#25968
                TabOrder = 0
                object lbl10: TLabel
                  Left = 10
                  Top = 24
                  Width = 108
                  Height = 12
                  Caption = #22522#30784#25915#20987#23041#21147#20493#25968#65306
                end
                object lbl13: TLabel
                  Left = 10
                  Top = 46
                  Width = 108
                  Height = 12
                  Caption = #27599#32423#22686#21152#23041#21147#20493#25968#65306
                end
                object lbl11: TLabel
                  Left = 10
                  Top = 68
                  Width = 108
                  Height = 12
                  Caption = #40635#30201#22522#30784#26102#38271'('#31186')'#65306
                end
                object lbl12: TLabel
                  Left = 10
                  Top = 112
                  Width = 108
                  Height = 12
                  Caption = #27599#32423#22686#21152#26102#38388'('#31186')'#65306
                end
                object lbl22: TLabel
                  Left = 10
                  Top = 134
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label588: TLabel
                  Left = 10
                  Top = 156
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object Label167: TLabel
                  Left = 58
                  Top = 90
                  Width = 60
                  Height = 12
                  Caption = #40635#30201#20960#29575#65306
                end
                object Label886: TLabel
                  Left = 10
                  Top = 178
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#36317#31163#35774#32622#65306
                end
                object seSkill204BasicPowerRate: TSpinEditEx
                  Left = 115
                  Top = 19
                  Width = 51
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 0
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill204BasicPowerRateChange
                end
                object seSkill204LevelupPowerRate: TSpinEditEx
                  Left = 115
                  Top = 41
                  Width = 51
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 0
                  TabOrder = 1
                  Value = 50
                  OnChange = seSkill204LevelupPowerRateChange
                end
                object seSkill204BasicMbTimer: TSpinEditEx
                  Left = 115
                  Top = 63
                  Width = 51
                  Height = 21
                  Hint = #40635#30201#22522#30784#26102#38271#65292#40664#35748'2'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 2
                  Value = 2
                  OnChange = seSkill204BasicMbTimerChange
                end
                object seSkill204LevelupMbTimer: TSpinEditEx
                  Left = 115
                  Top = 107
                  Width = 51
                  Height = 21
                  Hint = #25216#33021#31561#32423#27599#19978#21319#19968#32423#26102#22686#21152#30340#40635#30201#26102#38388#65292#40664#35748'1'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 3
                  Value = 1
                  OnChange = seSkill204LevelupMbTimerChange
                end
                object seSkill204CD: TSpinEditEx
                  Left = 115
                  Top = 129
                  Width = 51
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'15'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 4
                  Value = 15
                  OnChange = seSkill204CDChange
                end
                object seSkill204Rage: TSpinEditEx
                  Left = 115
                  Top = 151
                  Width = 51
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 5
                  Value = 1
                  OnChange = seSkill204RageChange
                end
                object chkSkill204SameLevel: TCheckBox
                  Left = 10
                  Top = 196
                  Width = 97
                  Height = 17
                  Hint = #21246#36873#21518#21516#31561#32423#29609#23478#21463#21040#27492#25216#33021#25915#20987#21518#21487#34987#40635#30201#65288#38656#21246#36873#20801#35768#40635#30201#20154#29289#26041#21487#29983#25928#65289
                  Caption = #21516#31561#32423#21487#40635#30201
                  TabOrder = 6
                  OnClick = chkSkill204SameLevelClick
                end
                object seSkill204BasicMbRate: TSpinEditEx
                  Left = 115
                  Top = 85
                  Width = 51
                  Height = 21
                  Hint = #20540#36234#22823#65292#20960#29575#36234#23567
                  MaxValue = 1000
                  MinValue = 0
                  TabOrder = 7
                  Value = 2
                  OnChange = seSkill204BasicMbRateChange
                end
                object chkSkill204DisableStopItem: TCheckBox
                  Left = 10
                  Top = 214
                  Width = 97
                  Height = 17
                  Hint = #21246#36873#21518#31105#27490#20351#29992#25216#33021#30452#25509#39134#21040#29289#21697#19978#38754
                  Caption = #31105#27490#39134#35013#22791
                  TabOrder = 8
                  OnClick = chkSkill204DisableStopItemClick
                end
                object seSkill204Distance: TSpinEditEx
                  Left = 115
                  Top = 173
                  Width = 51
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 9
                  Value = 1
                  OnChange = seSkill204DistanceChange
                end
              end
            end
            object ts19: TTabSheet
              Caption = #26059#39118#26025
              ImageIndex = 12
              object GroupBox206: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 161
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label655: TLabel
                  Left = 8
                  Top = 18
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label656: TLabel
                  Left = 8
                  Top = 48
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object Label657: TLabel
                  Left = 8
                  Top = 76
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#23041#21147#20493#25968#35774#32622#65306
                end
                object seSKILL208CD: TSpinEditEx
                  Left = 116
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'5'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 0
                  Value = 5
                  OnChange = seSKILL208CDChange
                end
                object seSkill208Rage: TSpinEditEx
                  Left = 116
                  Top = 43
                  Width = 45
                  Height = 21
                  Hint = #40664#35748#25915#20987#33539#22260'3x3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill208RageChange
                end
                object seSkill208PowerRate: TSpinEditEx
                  Left = 115
                  Top = 73
                  Width = 45
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 2
                  Value = 100
                  OnChange = seSkill208PowerRateChange
                end
                object chkSKILL208HeroDuanJin: TCheckBox
                  Left = 8
                  Top = 104
                  Width = 73
                  Height = 17
                  Caption = #33521#38596#26029#31563
                  TabOrder = 3
                  OnClick = chkSKILL208HeroDuanJinClick
                end
                object chkSKILL208PlayMosterDuanJin: TCheckBox
                  Left = 8
                  Top = 128
                  Width = 113
                  Height = 17
                  Caption = #20154#24418#24618#26029#31563#12289#20998#36523
                  TabOrder = 4
                  OnClick = chkSKILL208PlayMosterDuanJinClick
                end
              end
            end
            object TabSheet2: TTabSheet
              Caption = #24443#22320#38025
              object GroupBox56: TGroupBox
                Left = 8
                Top = 8
                Width = 185
                Height = 137
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label119: TLabel
                  Left = 16
                  Top = 24
                  Width = 84
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388#65306
                end
                object Label120: TLabel
                  Left = 152
                  Top = 46
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object lbl30: TLabel
                  Left = 16
                  Top = 46
                  Width = 84
                  Height = 12
                  Caption = #25915#20987#23041#21147#20493#25968#65306
                end
                object lbl31: TLabel
                  Left = 16
                  Top = 68
                  Width = 84
                  Height = 12
                  Caption = #26368#22823#25915#20987#33539#22260#65306
                end
                object seDedingMagicCD: TSpinEditEx
                  Left = 96
                  Top = 20
                  Width = 51
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seDedingMagicCDChange
                end
                object seDeDingMagicBasicPowerRate: TSpinEditEx
                  Left = 96
                  Top = 43
                  Width = 51
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seDeDingMagicBasicPowerRateChange
                end
                object seDeDingMagicAttackRange: TSpinEditEx
                  Left = 96
                  Top = 65
                  Width = 44
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290#40664#35748'3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 2
                  Value = 3
                  OnChange = seDeDingMagicAttackRangeChange
                end
                object chkDedingAllowPK: TCheckBox
                  Left = 16
                  Top = 90
                  Width = 129
                  Height = 17
                  Caption = #20801#35768#31354#30446#26631#25915#20987
                  TabOrder = 3
                  OnClick = chkDedingAllowPKClick
                end
                object chkDedingDisabledPK: TCheckBox
                  Left = 16
                  Top = 112
                  Width = 105
                  Height = 17
                  Caption = #31105#27490'PK'
                  TabOrder = 4
                  OnClick = chkDedingDisabledPKClick
                end
              end
            end
            object TabSheet7: TTabSheet
              Caption = #21050#26432#21073#27861
              ImageIndex = 2
              object GroupBox9: TGroupBox
                Left = 5
                Top = 5
                Width = 105
                Height = 41
                Caption = #26080#38480#21050#26432
                TabOrder = 0
                object CheckBoxLimitSwordLong: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 92
                  Height = 17
                  Hint = #25171#24320#27492#21151#33021#21518#65292#23558#26816#26597#26816#26597#38548#20301#26159#21542#26377#35282#33394#23384#22312#65292#20197#31105#27490#20992#20992#21050#26432#12290
                  Caption = #31105#27490#26080#38480#21050#26432
                  TabOrder = 0
                  OnClick = CheckBoxLimitSwordLongClick
                end
              end
              object GroupBox10: TGroupBox
                Left = 5
                Top = 49
                Width = 105
                Height = 41
                Caption = #25915#20987#21147#20493#25968
                TabOrder = 1
                object Label4: TLabel
                  Left = 8
                  Top = 19
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label10: TLabel
                  Left = 90
                  Top = 19
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object EditSwordLongPowerRate: TSpinEditEx
                  Left = 39
                  Top = 14
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = EditSwordLongPowerRateChange
                end
              end
              object GroupBox227: TGroupBox
                Left = 117
                Top = 5
                Width = 105
                Height = 41
                Caption = #25915#26432#23041#21147#20493#25968
                TabOrder = 2
                object Label781: TLabel
                  Left = 8
                  Top = 19
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label782: TLabel
                  Left = 90
                  Top = 19
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkillYedoPowerRate: TSpinEditEx
                  Left = 39
                  Top = 14
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkillYedoPowerRateChange
                end
              end
            end
            object TabSheet8: TTabSheet
              Caption = #28872#28779#21073#27861
              ImageIndex = 3
              object GroupBox59: TGroupBox
                Left = 8
                Top = 8
                Width = 145
                Height = 68
                Caption = #20351#29992#38388#38548#26102#38388
                TabOrder = 0
                object Label121: TLabel
                  Left = 16
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #38388#38548#65306
                end
                object Label122: TLabel
                  Left = 116
                  Top = 20
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seFireHitWaitTime: TSpinEditEx
                  Left = 56
                  Top = 16
                  Width = 53
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seFireHitWaitTimeChange
                end
                object chkCloseFireHitSkillFailHint: TCheckBox
                  Left = 16
                  Top = 42
                  Width = 121
                  Height = 17
                  Caption = #20851#38381#22833#36133#25552#31034
                  TabOrder = 1
                  OnClick = chkCloseFireHitSkillFailHintClick
                end
              end
              object GroupBox122: TGroupBox
                Left = 160
                Top = 10
                Width = 145
                Height = 45
                Caption = #25915#20987#21147#20493#25968
                TabOrder = 1
                object Label247: TLabel
                  Left = 16
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #20493#25968#65306
                end
                object Label248: TLabel
                  Left = 112
                  Top = 20
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seFireHitPowerRate: TSpinEditEx
                  Left = 56
                  Top = 16
                  Width = 53
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seFireHitPowerRateChange
                end
              end
              object GroupBox179: TGroupBox
                Left = 8
                Top = 88
                Width = 417
                Height = 64
                Caption = #24320#21551#21452#28872#28779
                TabOrder = 2
                object lbl139: TLabel
                  Left = 278
                  Top = 40
                  Width = 48
                  Height = 12
                  Caption = #22522#30784#20540#65306
                end
                object lbl140: TLabel
                  Left = 120
                  Top = 41
                  Width = 60
                  Height = 12
                  Caption = #24310#26102#26041#24335#65306
                end
                object lbl141: TLabel
                  Left = 386
                  Top = 40
                  Width = 24
                  Height = 12
                  Caption = #27627#31186
                end
                object chkEnableDoubleFireHitSkill: TCheckBox
                  Left = 8
                  Top = 17
                  Width = 83
                  Height = 17
                  Caption = #24320#21551#21452#28872#28779
                  TabOrder = 0
                  OnClick = chkEnableDoubleFireHitSkillClick
                end
                object chkEnableDoubleFireHitDelayClose: TCheckBox
                  Left = 8
                  Top = 38
                  Width = 92
                  Height = 17
                  Caption = #28872#28779#28040#22833#24310#26102
                  TabOrder = 1
                  OnClick = chkEnableDoubleFireHitDelayCloseClick
                end
                object cbbDoubleFireHitDelayCloseType: TComboBox
                  Left = 175
                  Top = 37
                  Width = 85
                  Height = 20
                  Hint = 
                    #22914#65306#25353#25216#33021#31561#32423#24310#26102#65292#22522#30784#20540'500'#13#10#13#10'0'#32423#25216#33021#65306#24310#26102'500'#65307'1'#32423#25216#33021#65306#24310#26102'1500'#65307'2'#32423#25216#33021#65306#24310#26102'2500'#13#10#13#10#25216#33021#31561#32423#27599#22686 +
                    #21152#19968#32423#65292#24310#26102#26102#38388#22810'1'#31186
                  Style = csDropDownList
                  ItemIndex = 0
                  TabOrder = 2
                  Text = #25216#33021#31561#32423
                  OnChange = cbbDoubleFireHitDelayCloseTypeChange
                  Items.Strings = (
                    #25216#33021#31561#32423
                    #22266#23450#26102#38388)
                end
                object seDoubleFireHitDelayCloseValue: TSpinEditEx
                  Left = 322
                  Top = 36
                  Width = 61
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 0
                  TabOrder = 3
                  Value = 1
                  OnChange = seDoubleFireHitDelayCloseValueChange
                end
              end
            end
            object TabSheet9: TTabSheet
              Caption = #29422#23376#21564
              ImageIndex = 4
              object GroupBox190: TGroupBox
                Left = 8
                Top = 8
                Width = 417
                Height = 209
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label583: TLabel
                  Left = 16
                  Top = 127
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object lbl27: TLabel
                  Left = 15
                  Top = 22
                  Width = 150
                  Height = 12
                  Caption = '0'#32423#25216#33021#8212'> '#40635#30201#26102#38271'('#31186')'#65306
                end
                object lbl38: TLabel
                  Left = 247
                  Top = 22
                  Width = 60
                  Height = 12
                  Caption = #40635#30201#33539#22260#65306
                end
                object Label148: TLabel
                  Left = 15
                  Top = 46
                  Width = 150
                  Height = 12
                  Caption = '1'#32423#25216#33021#8212'> '#40635#30201#26102#38271'('#31186')'#65306
                end
                object Label809: TLabel
                  Left = 247
                  Top = 46
                  Width = 60
                  Height = 12
                  Caption = #40635#30201#33539#22260#65306
                end
                object Label810: TLabel
                  Left = 15
                  Top = 70
                  Width = 150
                  Height = 12
                  Caption = '2'#32423#25216#33021#8212'> '#40635#30201#26102#38271'('#31186')'#65306
                end
                object Label811: TLabel
                  Left = 247
                  Top = 70
                  Width = 60
                  Height = 12
                  Caption = #40635#30201#33539#22260#65306
                end
                object Label812: TLabel
                  Left = 15
                  Top = 94
                  Width = 150
                  Height = 12
                  Caption = '3'#32423#25216#33021#8212'> '#40635#30201#26102#38271'('#31186')'#65306
                end
                object Label813: TLabel
                  Left = 247
                  Top = 94
                  Width = 60
                  Height = 12
                  Caption = #40635#30201#33539#22260#65306
                end
                object seSkill41CD: TSpinEditEx
                  Left = 120
                  Top = 123
                  Width = 49
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seSkill41CDChange
                end
                object seSkill41MbTimer0: TSpinEditEx
                  Left = 164
                  Top = 18
                  Width = 49
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 0
                  Value = 3
                  OnChange = seSkill41MbTimer0Change
                end
                object chkSkill41MbAttackSlave: TCheckBox
                  Left = 16
                  Top = 147
                  Width = 129
                  Height = 17
                  Caption = #20801#35768#40635#30201#23453#23453#12289#33521#38596
                  TabOrder = 3
                  OnClick = chkSkill41MbAttackSlaveClick
                end
                object chkSkill41MbAttackPlayObject: TCheckBox
                  Left = 16
                  Top = 170
                  Width = 97
                  Height = 17
                  Hint = #25171#24320#27492#21151#33021#21518#65292#23601#21487#20197#40635#30201#20154#29289
                  Caption = #20801#35768#40635#30201#20154#29289
                  TabOrder = 4
                  OnClick = chkSkill41MbAttackPlayObjectClick
                end
                object seSkill41MbRange0: TSpinEditEx
                  Left = 305
                  Top = 18
                  Width = 49
                  Height = 21
                  Hint = #40635#30201#33539#22260#40664#35748#20026'2'#12289#24314#35758#25968#20540#19981#35201#36229#36807'3'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seSkill41MbRange0Change
                end
                object seSkill41MbTimer1: TSpinEditEx
                  Tag = 1
                  Left = 164
                  Top = 42
                  Width = 49
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 5
                  Value = 3
                  OnChange = seSkill41MbTimer0Change
                end
                object seSkill41MbRange1: TSpinEditEx
                  Tag = 1
                  Left = 305
                  Top = 42
                  Width = 49
                  Height = 21
                  Hint = #40635#30201#33539#22260#40664#35748#20026'2'#12289#24314#35758#25968#20540#19981#35201#36229#36807'3'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 6
                  Value = 10
                  OnChange = seSkill41MbRange0Change
                end
                object seSkill41MbTimer2: TSpinEditEx
                  Tag = 2
                  Left = 164
                  Top = 66
                  Width = 49
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 7
                  Value = 3
                  OnChange = seSkill41MbTimer0Change
                end
                object seSkill41MbRange2: TSpinEditEx
                  Tag = 2
                  Left = 305
                  Top = 66
                  Width = 49
                  Height = 21
                  Hint = #40635#30201#33539#22260#40664#35748#20026'2'#12289#24314#35758#25968#20540#19981#35201#36229#36807'3'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 8
                  Value = 10
                  OnChange = seSkill41MbRange0Change
                end
                object seSkill41MbTimer3: TSpinEditEx
                  Tag = 3
                  Left = 164
                  Top = 90
                  Width = 49
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 9
                  Value = 3
                  OnChange = seSkill41MbTimer0Change
                end
                object seSkill41MbRange3: TSpinEditEx
                  Tag = 3
                  Left = 305
                  Top = 90
                  Width = 49
                  Height = 21
                  Hint = #40635#30201#33539#22260#40664#35748#20026'2'#12289#24314#35758#25968#20540#19981#35201#36229#36807'3'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 10
                  Value = 10
                  OnChange = seSkill41MbRange0Change
                end
                object chkDisableSkill41MbSameLevel: TCheckBox
                  Left = 192
                  Top = 147
                  Width = 137
                  Height = 17
                  Caption = #31105#27490#40635#30201#21516#31561#32423#30446#26631
                  TabOrder = 11
                  OnClick = chkDisableSkill41MbSameLevelClick
                end
              end
            end
            object TabSheet10: TTabSheet
              Caption = #25810#40857#25163
              ImageIndex = 5
              object GroupBox50: TGroupBox
                Left = 8
                Top = 8
                Width = 177
                Height = 161
                Caption = #26159#21542#21487#20197#25235#20154#29289
                TabOrder = 0
                object lbl32: TLabel
                  Left = 16
                  Top = 124
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object chkSkill71PullPlayObject: TCheckBox
                  Left = 16
                  Top = 16
                  Width = 89
                  Height = 17
                  Caption = #20801#35768#25235#20154#29289
                  TabOrder = 0
                  OnClick = chkSkill71PullPlayObjectClick
                end
                object chkSkill71PullCrossInSafeZone: TCheckBox
                  Left = 16
                  Top = 100
                  Width = 121
                  Height = 17
                  Caption = #23433#20840#21306#20840#31105#27490
                  TabOrder = 2
                  OnClick = chkSkill71PullCrossInSafeZoneClick
                end
                object chkSkill71PullSlave: TCheckBox
                  Left = 16
                  Top = 37
                  Width = 113
                  Height = 17
                  Caption = #20801#35768#25235#23453#23453#12289#33521#38596
                  TabOrder = 1
                  OnClick = chkSkill71PullSlaveClick
                end
                object seSkill71CD: TSpinEditEx
                  Left = 120
                  Top = 120
                  Width = 41
                  Height = 21
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 3
                  Value = 10
                  OnChange = seSkill71CDChange
                end
                object chkSkill71DisableAttackSameLevel: TCheckBox
                  Left = 16
                  Top = 58
                  Width = 129
                  Height = 17
                  Caption = #31105#27490#25235#21516#31561#32423#30446#26631
                  TabOrder = 4
                  OnClick = chkSkill71DisableAttackSameLevelClick
                end
                object chkSkill71DisableAttackFriend: TCheckBox
                  Left = 16
                  Top = 79
                  Width = 129
                  Height = 17
                  Hint = #19981#21246#36873#21487#20197#25235#25152#26377#23545#35937#65307#21246#36873#21518#31105#27490#25346#21516#34892#20250#12289#38431#21451#65292#22827#22971#12289#24072#24466#65288#21363#65306#19981#25235#25915#20987#27169#24335#19979#19981#33021#20260#23475#30340#30446#26631#65289
                  Caption = #31105#27490#25235#38431#21451
                  TabOrder = 5
                  OnClick = chkSkill71DisableAttackFriendClick
                end
              end
            end
            object TabSheet3: TTabSheet
              Caption = #36880#26085#21073#27861
              ImageIndex = 7
              object GroupBox60: TGroupBox
                Left = 8
                Top = 8
                Width = 129
                Height = 41
                Caption = #25915#20987#21147#20493#25968
                TabOrder = 0
                object Label123: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label124: TLabel
                  Left = 96
                  Top = 20
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object EditSkill56PowerRate: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = EditSkill56PowerRateChange
                end
              end
              object GroupBox83: TGroupBox
                Left = 8
                Top = 56
                Width = 129
                Height = 49
                Caption = #20351#29992#38388#38548#26102#38388
                TabOrder = 1
                object Label186: TLabel
                  Left = 8
                  Top = 24
                  Width = 30
                  Height = 12
                  Caption = #26102#38388':'
                end
                object Label187: TLabel
                  Left = 96
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditSWordHitWaitTime: TSpinEditEx
                  Left = 44
                  Top = 20
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = EditSWordHitWaitTimeChange
                end
              end
            end
            object TabSheet25: TTabSheet
              Caption = #40857#24433#21073#27861
              ImageIndex = 8
              object GroupBox58: TGroupBox
                Left = 8
                Top = 8
                Width = 153
                Height = 97
                Caption = #25915#20987#21147#20493#25968
                TabOrder = 0
                object Label128: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object Label129: TLabel
                  Left = 120
                  Top = 20
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label188: TLabel
                  Left = 8
                  Top = 46
                  Width = 60
                  Height = 12
                  Caption = #20919#21364#26102#38388#65306
                end
                object Label189: TLabel
                  Left = 120
                  Top = 46
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object lbl95: TLabel
                  Left = 8
                  Top = 72
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#33539#22260#65306
                end
                object seSkill42PowerRate: TSpinEditEx
                  Left = 68
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill42PowerRateChange
                end
                object seSkill42HitWaitTime: TSpinEditEx
                  Left = 68
                  Top = 41
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seSkill42HitWaitTimeChange
                end
                object seSkill42Range: TSpinEditEx
                  Left = 68
                  Top = 68
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seSkill42RangeChange
                end
              end
            end
            object TabSheet26: TTabSheet
              Caption = #21452#40857#26025'/'#21322#26376
              ImageIndex = 9
              object GroupBox68: TGroupBox
                Left = 8
                Top = 8
                Width = 129
                Height = 45
                Caption = #21452#40857#26025#25915#20987#21147#20493#25968
                TabOrder = 0
                object Label130: TLabel
                  Left = 8
                  Top = 23
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label131: TLabel
                  Left = 96
                  Top = 23
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkill40PowerRate: TSpinEditEx
                  Left = 44
                  Top = 18
                  Width = 45
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill40PowerRateChange
                end
              end
              object GroupBox239: TGroupBox
                Left = 8
                Top = 64
                Width = 129
                Height = 45
                Caption = #21322#26376#25915#20987#21147#20493#25968
                TabOrder = 1
                object Label807: TLabel
                  Left = 8
                  Top = 23
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label808: TLabel
                  Left = 96
                  Top = 23
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkill25PowerRate: TSpinEditEx
                  Left = 44
                  Top = 18
                  Width = 45
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill25PowerRateChange
                end
              end
            end
            object TabSheet27: TTabSheet
              Caption = #38647#38662#21073#27861
              ImageIndex = 10
              object GroupBox77: TGroupBox
                Left = 8
                Top = 59
                Width = 129
                Height = 41
                Caption = #25915#20987#21147#20493#25968
                TabOrder = 0
                object Label132: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label133: TLabel
                  Left = 114
                  Top = 20
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object EditSkill43PowerRate: TSpinEditEx
                  Left = 40
                  Top = 15
                  Width = 70
                  Height = 21
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = EditSkill43PowerRateChange
                end
              end
              object grp55: TGroupBox
                Left = 8
                Top = 103
                Width = 129
                Height = 112
                Caption = #38647#30005#40635#30201#25511#21046
                TabOrder = 1
                object Label724: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #20960#29575':'
                end
                object Label725: TLabel
                  Left = 114
                  Top = 20
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label726: TLabel
                  Left = 8
                  Top = 44
                  Width = 30
                  Height = 12
                  Caption = #26102#38388':'
                end
                object Label727: TLabel
                  Left = 111
                  Top = 44
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label728: TLabel
                  Left = 8
                  Top = 68
                  Width = 54
                  Height = 12
                  Caption = #20260#23475#22686#21152':'
                end
                object Label729: TLabel
                  Left = 114
                  Top = 68
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill43LDMBRate: TSpinEditEx
                  Left = 40
                  Top = 15
                  Width = 70
                  Height = 21
                  Hint = #20540#36234#22823#20960#29575#36234#22823
                  MaxValue = 100
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill43LDMBRateChange
                end
                object seSkill43LDMBTime: TSpinEditEx
                  Left = 40
                  Top = 39
                  Width = 70
                  Height = 21
                  Hint = #20540#36234#23567#20960#29575#36234#22823
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seSkill43LDMBTimeChange
                end
                object seSkill43LDMBPowerAdd: TSpinEditEx
                  Left = 64
                  Top = 63
                  Width = 46
                  Height = 21
                  Hint = #20540#36234#23567#20960#29575#36234#22823
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 2
                  Value = 100
                  OnChange = seSkill43LDMBPowerAddChange
                end
                object chkSkill43LockParaly: TCheckBox
                  Left = 8
                  Top = 88
                  Width = 113
                  Height = 17
                  Caption = #20851#32852#40635#30201#29366#24577#25511#21046
                  TabOrder = 3
                  OnClick = chkSkill43LockParalyClick
                end
              end
              object GroupBox220: TGroupBox
                Left = 8
                Top = 8
                Width = 129
                Height = 49
                Caption = #20351#29992#38388#38548#26102#38388
                TabOrder = 2
                object Label730: TLabel
                  Left = 8
                  Top = 24
                  Width = 36
                  Height = 12
                  Caption = #38388#38548#65306
                end
                object Label731: TLabel
                  Left = 111
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seSkill43HitWaitTime: TSpinEditEx
                  Left = 40
                  Top = 20
                  Width = 70
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill43HitWaitTimeChange
                end
              end
            end
            object TabSheet28: TTabSheet
              Caption = #24320#22825#26025'/'#26029#31354#26025
              ImageIndex = 11
              object GroupBox78: TGroupBox
                Left = 8
                Top = 8
                Width = 181
                Height = 94
                Caption = #24320#22825#26025#25915#20987#21147#20493#25968
                TabOrder = 0
                object Label153: TLabel
                  Left = 8
                  Top = 69
                  Width = 78
                  Height = 12
                  Caption = #33521#38596#36731#20987#20493#25968':'
                end
                object Label155: TLabel
                  Left = 147
                  Top = 69
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label206: TLabel
                  Left = 8
                  Top = 22
                  Width = 78
                  Height = 12
                  Caption = #20154#29289#37325#20987#20493#25968':'
                end
                object Label207: TLabel
                  Left = 147
                  Top = 22
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label231: TLabel
                  Left = 8
                  Top = 45
                  Width = 78
                  Height = 12
                  Caption = #33521#38596#37325#20987#20493#25968':'
                end
                object Label232: TLabel
                  Left = 147
                  Top = 45
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seHeroSkill66PowerRate: TSpinEditEx
                  Left = 87
                  Top = 64
                  Width = 57
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seHeroSkill66PowerRateChange
                end
                object seHumSkill66HighPowerRate: TSpinEditEx
                  Left = 88
                  Top = 18
                  Width = 57
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seHumSkill66HighPowerRateChange
                end
                object seHeroSkill66HighPowerRate: TSpinEditEx
                  Left = 88
                  Top = 41
                  Width = 57
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seHeroSkill66HighPowerRateChange
                end
              end
              object GroupBox85: TGroupBox
                Left = 200
                Top = 140
                Width = 153
                Height = 69
                Caption = #24320#22825#26025#20351#29992#38388#38548#26102#38388
                TabOrder = 1
                object Label190: TLabel
                  Left = 8
                  Top = 24
                  Width = 30
                  Height = 12
                  Caption = #38388#38548':'
                end
                object Label191: TLabel
                  Left = 112
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label703: TLabel
                  Left = 8
                  Top = 46
                  Width = 54
                  Height = 12
                  Caption = #33521#38596#38388#38548':'
                  Visible = False
                end
                object Label704: TLabel
                  Left = 112
                  Top = 46
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object EditSkill66HitWaitTime: TSpinEditEx
                  Left = 63
                  Top = 20
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = EditSkill66HitWaitTimeChange
                end
                object seHeroSkill66HitWaitTime: TSpinEditEx
                  Left = 63
                  Top = 42
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill66HitWaitTimeChange
                end
              end
              object GroupBox95: TGroupBox
                Left = 8
                Top = 108
                Width = 181
                Height = 68
                Caption = #33521#38596#37325#20987#35302#21457
                TabOrder = 2
                object Label205: TLabel
                  Left = 8
                  Top = 43
                  Width = 78
                  Height = 12
                  Caption = #33521#38596#37325#20987#26426#29575':'
                end
                object seHeroSkill66HighAttackRate: TSpinEditEx
                  Left = 87
                  Top = 39
                  Width = 57
                  Height = 21
                  Hint = #37325#20987#26426#29575#65292#25968#23383#36234#23567#65292#26426#29575#36234#39640
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seHeroSkill66HighAttackRateChange
                end
                object chkHeroSkill66HighAttackNoUseRate: TCheckBox
                  Left = 8
                  Top = 18
                  Width = 156
                  Height = 17
                  Hint = #21246#36873#26681#25454#31561#32423#21028#26029#65292#33521#38596#31561#32423'>='#30446#26631#31561#32423#65292#20351#29992#37325#20987#65292#21542#21017#36731#20987#13#10#13#10#19981#21246#36873#21017#33521#38596#37325#20987#20351#29992#20960#29575
                  Caption = #33521#38596#24320#22825#26025#20351#29992#31561#32423#21387#21046
                  TabOrder = 1
                  OnClick = chkHeroSkill66HighAttackNoUseRateClick
                end
              end
              object GroupBox240: TGroupBox
                Left = 200
                Top = 8
                Width = 153
                Height = 45
                Caption = #26029#31354#26025#25915#20987#21147#20493#25968
                TabOrder = 3
                object Label830: TLabel
                  Left = 8
                  Top = 22
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label831: TLabel
                  Left = 121
                  Top = 22
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkill113PowerRate: TSpinEditEx
                  Left = 63
                  Top = 17
                  Width = 57
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill113PowerRateChange
                end
              end
              object GroupBox242: TGroupBox
                Left = 200
                Top = 60
                Width = 153
                Height = 69
                Caption = #26029#31354#26025#20351#29992#38388#38548#26102#38388
                TabOrder = 4
                object Label832: TLabel
                  Left = 8
                  Top = 24
                  Width = 30
                  Height = 12
                  Caption = #38388#38548':'
                end
                object Label833: TLabel
                  Left = 112
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label834: TLabel
                  Left = 8
                  Top = 46
                  Width = 54
                  Height = 12
                  Caption = #33521#38596#38388#38548':'
                  Visible = False
                end
                object Label835: TLabel
                  Left = 112
                  Top = 46
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object seSkill113HitWaitTime: TSpinEditEx
                  Left = 63
                  Top = 20
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill113HitWaitTimeChange
                end
                object seHeroSkill113HitWaitTime: TSpinEditEx
                  Left = 63
                  Top = 42
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill113HitWaitTimeChange
                end
              end
            end
            object TabSheet90: TTabSheet
              Caption = #37326#34542#20914#25758
              ImageIndex = 10
              object GroupBox193: TGroupBox
                Left = 8
                Top = 8
                Width = 177
                Height = 89
                Caption = #25552#31034#20449#24687
                TabOrder = 0
                object Label565: TLabel
                  Left = 16
                  Top = 64
                  Width = 60
                  Height = 12
                  Caption = #20919#21364#26102#38388#65306
                end
                object Label566: TLabel
                  Left = 128
                  Top = 64
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object chkShowDoMotaeboMsg: TCheckBox
                  Left = 16
                  Top = 16
                  Width = 105
                  Height = 17
                  Caption = #26174#31034#25552#31034#20449#24687
                  TabOrder = 0
                  OnClick = chkShowDoMotaeboMsgClick
                end
                object chkDoMotaeboPushSameLevel: TCheckBox
                  Left = 16
                  Top = 40
                  Width = 153
                  Height = 17
                  Caption = #21487#20197#25512#21160#31561#32423#30456#21516#30340#35282#33394
                  TabOrder = 1
                  OnClick = chkDoMotaeboPushSameLevelClick
                end
                object seDoMotaeboCD: TSpinEditEx
                  Left = 76
                  Top = 60
                  Width = 45
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seDoMotaeboCDChange
                end
              end
              object chkBarbaricSeptum: TCheckBox
                Left = 8
                Top = 108
                Width = 121
                Height = 17
                Caption = #20801#35768#20840#23616#38548#20301#37326#34542
                TabOrder = 1
                OnClick = chkBarbaricSeptumClick
              end
            end
          end
        end
        object TabSheet63: TTabSheet
          Caption = #27861#24072#25216#33021
          ImageIndex = 2
          object PageControl5: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            ActivePage = ts4
            Align = alClient
            MultiLine = True
            TabOrder = 0
            object ts4: TTabSheet
              Caption = #20912#38684#38634#38632
              ImageIndex = 10
              object grp8: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 145
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object lbl19: TLabel
                  Left = 8
                  Top = 42
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label564: TLabel
                  Left = 8
                  Top = 72
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object lbl51: TLabel
                  Left = 10
                  Top = 100
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#23041#21147#20493#25968#35774#32622#65306
                end
                object chkSkill205ReduceMP: TCheckBox
                  Left = 8
                  Top = 18
                  Width = 81
                  Height = 17
                  Caption = #20987#20013#20943'MP'#20540
                  TabOrder = 0
                  OnClick = chkSkill205ReduceMPClick
                end
                object seSkill205CD: TSpinEditEx
                  Left = 116
                  Top = 39
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'5'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 1
                  Value = 5
                  OnChange = seSkill205CDChange
                end
                object seSkill205Rage: TSpinEditEx
                  Left = 116
                  Top = 67
                  Width = 45
                  Height = 21
                  Hint = #40664#35748#25915#20987#33539#22260'2x2'#65292#24314#35758#19981#35201#35774#32622#36229#36807'3x3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 2
                  Value = 1
                  OnChange = seSkill205RageChange
                end
                object seSkill205PowerRate: TSpinEditEx
                  Left = 115
                  Top = 97
                  Width = 45
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 3
                  Value = 100
                  OnChange = seSkill205PowerRateChange
                end
                object chkSkill205PowerTwoAttack: TCheckBox
                  Left = 8
                  Top = 120
                  Width = 73
                  Height = 17
                  Caption = #20004#27425#20260#23475
                  TabOrder = 4
                  OnClick = chkSkill205PowerTwoAttackClick
                end
              end
            end
            object ts5: TTabSheet
              Caption = #20912#38684#32676#38632
              ImageIndex = 11
              object grp10: TGroupBox
                Left = 2
                Top = 8
                Width = 165
                Height = 161
                Caption = #25216#33021#21442#25968
                TabOrder = 0
                object lbl14: TLabel
                  Left = 10
                  Top = 20
                  Width = 108
                  Height = 12
                  Caption = #22522#30784#25915#20987#23041#21147#20493#25968#65306
                end
                object lbl15: TLabel
                  Left = 10
                  Top = 44
                  Width = 108
                  Height = 12
                  Caption = #27599#32423#22686#21152#23041#21147#20493#25968#65306
                end
                object lbl16: TLabel
                  Left = 10
                  Top = 67
                  Width = 108
                  Height = 12
                  Caption = #40635#30201#22522#30784#26102#38271'('#31186')'#65306
                end
                object lbl17: TLabel
                  Left = 10
                  Top = 90
                  Width = 108
                  Height = 12
                  Caption = #27599#32423#22686#21152#26102#38388'('#31186')'#65306
                end
                object lbl20: TLabel
                  Left = 10
                  Top = 114
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label584: TLabel
                  Left = 10
                  Top = 136
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object seSkill206BasicPowerRate: TSpinEditEx
                  Left = 115
                  Top = 17
                  Width = 45
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill206BasicPowerRateChange
                end
                object seSkill206LevelupPowerRate: TSpinEditEx
                  Left = 115
                  Top = 40
                  Width = 45
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 1
                  Value = 50
                  OnChange = seSkill206LevelupPowerRateChange
                end
                object seSkill206BasicMbTimer: TSpinEditEx
                  Left = 115
                  Top = 64
                  Width = 45
                  Height = 21
                  Hint = #40635#30201#22522#30784#26102#38271#65292#40664#35748'2'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 2
                  Value = 2
                  OnChange = seSkill206BasicMbTimerChange
                end
                object seSkill206LevelupMbTimer: TSpinEditEx
                  Left = 115
                  Top = 88
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#31561#32423#27599#19978#21319#19968#32423#26102#22686#21152#30340#40635#30201#26102#38388#65292#40664#35748'1'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 3
                  Value = 1
                  OnChange = seSkill206LevelupMbTimerChange
                end
                object seSkill206CD: TSpinEditEx
                  Left = 115
                  Top = 111
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'15'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 4
                  Value = 15
                  OnChange = seSkill206CDChange
                end
                object seSkill206Rage: TSpinEditEx
                  Left = 115
                  Top = 131
                  Width = 45
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 5
                  Value = 1
                  OnChange = seSkill206RageChange
                end
              end
              object grp9: TGroupBox
                Left = 184
                Top = 8
                Width = 241
                Height = 161
                Caption = #25216#33021#25928#26524
                TabOrder = 1
                object lbl36: TLabel
                  Left = 128
                  Top = 24
                  Width = 60
                  Height = 12
                  Caption = #20912#20923#20960#29575#65306
                end
                object chkSkill206MbAttackSlave: TCheckBox
                  Left = 10
                  Top = 79
                  Width = 175
                  Height = 17
                  Caption = #20801#35768#40635#30201'/'#20912#20923#23453#23453#12289#33521#38596
                  TabOrder = 2
                  OnClick = chkSkill206MbAttackSlaveClick
                end
                object chkSkill206MbAttackMon: TCheckBox
                  Left = 10
                  Top = 40
                  Width = 107
                  Height = 17
                  Caption = #20801#35768#40635#30201'/'#20912#20923#24618#29289
                  TabOrder = 0
                  OnClick = chkSkill206MbAttackMonClick
                end
                object chkSkill206MbAttackHuman: TCheckBox
                  Left = 10
                  Top = 59
                  Width = 107
                  Height = 17
                  Caption = #20801#35768#40635#30201'/'#20912#20923#20154#29289
                  TabOrder = 1
                  OnClick = chkSkill206MbAttackHumanClick
                end
                object chkSkill206MbFastParalysis: TCheckBox
                  Left = 10
                  Top = 98
                  Width = 107
                  Height = 17
                  Hint = #24403#20154#29289#25110#24618#29289#34987#35813#25216#33021#40635#30201#21518#65292#26159#21542#34987#25915#20987#39532#19978#21462#28040#40635#30201#29366#24577#12290
                  Caption = #26159#21542#24555#36895#40635#30201
                  TabOrder = 3
                  OnClick = chkSkill206MbFastParalysisClick
                end
                object chkSkill206Frozen: TCheckBox
                  Left = 10
                  Top = 22
                  Width = 79
                  Height = 17
                  Hint = #21246#36873#21518#20026#20912#20923#25928#26524#65292#40664#35748#20026#40635#30201#25928#26524
                  Caption = #20801#35768#20912#20923
                  TabOrder = 4
                  OnClick = chkSkill206FrozenClick
                end
                object chkSkill206SameLevel: TCheckBox
                  Left = 10
                  Top = 118
                  Width = 143
                  Height = 17
                  Hint = #21516#31561#32423#20801#35768#40635#30201#12289#20912#20923
                  Caption = #21516#31561#32423#20801#35768#40635#30201#12289#20912#20923
                  TabOrder = 5
                  OnClick = chkSkill206SameLevelClick
                end
                object seSkill206FrozenRate: TSpinEditEx
                  Left = 187
                  Top = 19
                  Width = 45
                  Height = 21
                  Hint = #25968#23383#36234#23567#20960#29575#36234#39640
                  MaxValue = 100
                  MinValue = 1
                  TabOrder = 6
                  Value = 100
                  OnChange = seSkill206FrozenRateChange
                end
              end
            end
            object ts17: TTabSheet
              Caption = #20116#38647#36720
              ImageIndex = 13
              object GroupBox205: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 105
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label652: TLabel
                  Left = 8
                  Top = 18
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label653: TLabel
                  Left = 8
                  Top = 48
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object Label654: TLabel
                  Left = 8
                  Top = 76
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#23041#21147#20493#25968#35774#32622#65306
                end
                object seSKILL209CD: TSpinEditEx
                  Left = 116
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'5'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 0
                  Value = 5
                  OnChange = seSKILL209CDChange
                end
                object seSkill209Rage: TSpinEditEx
                  Left = 116
                  Top = 43
                  Width = 45
                  Height = 21
                  Hint = #40664#35748#25915#20987#33539#22260'3x3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill209RageChange
                end
                object seSkill209PowerRate: TSpinEditEx
                  Left = 115
                  Top = 73
                  Width = 45
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 2
                  Value = 100
                  OnChange = seSkill209PowerRateChange
                end
              end
            end
            object TabSheet14: TTabSheet
              Caption = #35825#24785#20043#20809
              object GroupBox38: TGroupBox
                Left = 8
                Top = 8
                Width = 162
                Height = 41
                Caption = #24618#29289#31561#32423#38480#21046
                TabOrder = 0
                object Label98: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #31561#32423':'
                end
                object EditMagTammingLevel: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 109
                  Height = 21
                  Hint = #25351#23450#31561#32423#20197#19979#30340#24618#29289#25165#20250#34987#35825#24785#65292#25351#23450#31561#32423#20197#19978#30340#24618#29289#35825#24785#26080#25928#12290
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = EditMagTammingLevelChange
                end
              end
              object GroupBox45: TGroupBox
                Left = 176
                Top = 8
                Width = 113
                Height = 41
                Caption = #35825#24785#25968#37327
                TabOrder = 1
                object Label111: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #25968#37327':'
                end
                object EditTammingCount: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 61
                  Height = 21
                  Hint = #21487#35825#24785#24618#29289#25968#37327#12290
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = EditTammingCountChange
                end
              end
              object GroupBox39: TGroupBox
                Left = 8
                Top = 56
                Width = 162
                Height = 73
                Caption = #35825#24785#26426#29575
                TabOrder = 2
                object Label99: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24618#29289#31561#32423':'
                end
                object Label100: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #24618#29289#34880#37327':'
                end
                object EditMagTammingTargetLevel: TSpinEditEx
                  Left = 64
                  Top = 15
                  Width = 90
                  Height = 21
                  Hint = #24618#29289#31561#32423#27604#29575#65292#27492#25968#23383#36234#23567#26426#29575#36234#22823#12290
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = EditMagTammingTargetLevelChange
                end
                object EditMagTammingHPRate: TSpinEditEx
                  Left = 64
                  Top = 39
                  Width = 90
                  Height = 21
                  Hint = #24618#29289#34880#37327#27604#29575#65292#27492#25968#23383#36234#22823#65292#26426#29575#36234#22823#12290
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 1
                  Value = 1
                  OnChange = EditMagTammingHPRateChange
                end
              end
              object GroupBox120: TGroupBox
                Left = 176
                Top = 56
                Width = 113
                Height = 41
                Caption = #24310#38271#21467#21464#26102#38388
                TabOrder = 3
                object Label244: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #22522#25968':'
                end
                object EditMasterRoyaltyTime: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 61
                  Height = 21
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 60
                  OnChange = EditMasterRoyaltyTimeChange
                end
              end
            end
            object TabSheet15: TTabSheet
              Caption = #28779#22681
              ImageIndex = 1
              object GroupBox46: TGroupBox
                Left = 8
                Top = 8
                Width = 113
                Height = 41
                Caption = #23433#20840#21306#31105#27490#28779#22681
                TabOrder = 0
                object CheckBoxFireCrossInSafeZone: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 97
                  Height = 17
                  Hint = #25171#24320#27492#21151#33021#21518#65292#22312#23433#20840#21306#19981#20801#35768#25918#28779#22681#12290
                  Caption = #31105#27490#28779#22681
                  TabOrder = 0
                  OnClick = CheckBoxFireCrossInSafeZoneClick
                end
              end
              object GroupBox117: TGroupBox
                Left = 128
                Top = 8
                Width = 121
                Height = 41
                Caption = #25442#22320#22270#28040#22833
                TabOrder = 1
                object CheckBoxDisableChangeMapFireCross: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 81
                  Height = 17
                  Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#19979#32447#25110#26356#25442#22320#22270#21518#65292#25918#20986#26469#30340#28779#22681#33258#21160#28040#22833#12290
                  Caption = #25442#22320#22270#28040#22833
                  TabOrder = 0
                  OnClick = CheckBoxDisableChangeMapFireCrossClick
                end
              end
              object GroupBox118: TGroupBox
                Left = 8
                Top = 56
                Width = 161
                Height = 81
                BiDiMode = bdRightToLeft
                Caption = #24378#24230#25511#21046
                ParentBiDiMode = False
                TabOrder = 2
                object Label240: TLabel
                  Left = 8
                  Top = 24
                  Width = 54
                  Height = 12
                  Caption = #26368#38271#26102#38388':'
                end
                object Label241: TLabel
                  Left = 120
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #20998
                end
                object Label242: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label243: TLabel
                  Left = 120
                  Top = 44
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object EditFireCrossMaxTime: TSpinEditEx
                  Left = 64
                  Top = 20
                  Width = 49
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 5
                  OnChange = EditFireCrossMaxTimeChange
                end
                object EditFireCrossPowerRate: TSpinEditEx
                  Left = 64
                  Top = 42
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = EditFireCrossPowerRateChange
                end
              end
            end
            object TabSheet16: TTabSheet
              Caption = #22307#35328#26415
              ImageIndex = 2
              object GroupBox37: TGroupBox
                Left = 8
                Top = 8
                Width = 113
                Height = 41
                Caption = #24618#29289#31561#32423#38480#21046
                TabOrder = 0
                object Label97: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #31561#32423':'
                end
                object seMagTurnUndeadLevel: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 61
                  Height = 21
                  Hint = #25351#23450#31561#32423#20197#19979#30340#24618#29289#25165#20250#34987#22307#35328#65292#25351#23450#31561#32423#20197#19978#30340#24618#29289#22307#35328#26080#25928#12290
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seMagTurnUndeadLevelChange
                end
              end
              object chkMagTurnUndeadSameLevel: TCheckBox
                Left = 10
                Top = 56
                Width = 135
                Height = 17
                Hint = #21246#36873#21518#20801#35768#22307#35328#21516#31561#32423#30340#19981#27515#31995#24618#29289
                Caption = #20801#35768#25915#20987#21516#31561#32423#24618#29289
                TabOrder = 1
                OnClick = chkMagTurnUndeadSameLevelClick
              end
            end
            object TabSheet17: TTabSheet
              Caption = #22320#29425#38647#20809
              ImageIndex = 3
              object GroupBox15: TGroupBox
                Left = 8
                Top = 8
                Width = 153
                Height = 73
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label9: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#33539#22260#65306
                end
                object Label590: TLabel
                  Left = 8
                  Top = 46
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object Label591: TLabel
                  Left = 122
                  Top = 46
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seElecBlizzardRange: TSpinEditEx
                  Left = 68
                  Top = 15
                  Width = 49
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seElecBlizzardRangeChange
                end
                object seElecBlizzardPowerRate: TSpinEditEx
                  Left = 68
                  Top = 42
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seElecBlizzardPowerRateChange
                end
              end
            end
            object TabSheet19: TTabSheet
              Caption = #29190#35010#28779#28976
              ImageIndex = 4
              object GroupBox13: TGroupBox
                Left = 8
                Top = 8
                Width = 153
                Height = 73
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label7: TLabel
                  Left = 8
                  Top = 22
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#33539#22260#65306
                end
                object lbl40: TLabel
                  Left = 8
                  Top = 46
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object lbl41: TLabel
                  Left = 122
                  Top = 46
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seFireBoomRage: TSpinEditEx
                  Left = 68
                  Top = 17
                  Width = 49
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seFireBoomRageChange
                end
                object seFireBoomRagePowerRate: TSpinEditEx
                  Left = 68
                  Top = 42
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seFireBoomRagePowerRateChange
                end
              end
            end
            object TabSheet23: TTabSheet
              Caption = #20912#21638#21742
              ImageIndex = 5
              object GroupBox14: TGroupBox
                Left = 8
                Top = 8
                Width = 153
                Height = 73
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label8: TLabel
                  Left = 8
                  Top = 22
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#33539#22260#65306
                end
                object lbl42: TLabel
                  Left = 8
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object lbl43: TLabel
                  Left = 122
                  Top = 45
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSnowWindRange: TSpinEditEx
                  Left = 68
                  Top = 17
                  Width = 49
                  Height = 21
                  Hint = #40664#35748#33539#22260#20026'1'#12289#24314#35758#33539#22260#35774#32622#19981#35201#36229#36807'2'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seSnowWindRangeChange
                end
                object seSnowWindPowerRate: TSpinEditEx
                  Left = 68
                  Top = 41
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seSnowWindPowerRateChange
                end
              end
              object GroupBox215: TGroupBox
                Left = 8
                Top = 88
                Width = 153
                Height = 49
                Caption = #20351#29992#38388#38548#26102#38388
                TabOrder = 1
                object Label134: TLabel
                  Left = 8
                  Top = 24
                  Width = 60
                  Height = 12
                  Caption = #26102#38388#38388#38548#65306
                end
                object Label135: TLabel
                  Left = 122
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seSnowwindWaitTime: TSpinEditEx
                  Left = 68
                  Top = 20
                  Width = 49
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSnowwindWaitTimeChange
                end
              end
            end
            object TabSheet18: TTabSheet
              Caption = #28781#22825#28779
              ImageIndex = 6
              object GroupBox51: TGroupBox
                Left = 8
                Top = 8
                Width = 153
                Height = 92
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object lbl44: TLabel
                  Left = 8
                  Top = 41
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object lbl45: TLabel
                  Left = 122
                  Top = 41
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label708: TLabel
                  Left = 8
                  Top = 68
                  Width = 60
                  Height = 12
                  Caption = #20351#29992#38388#38548#65306
                end
                object Label709: TLabel
                  Left = 122
                  Top = 68
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object chkPlayObjectReduceMP: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 97
                  Height = 17
                  Caption = #20987#20013#20943'MP'#20540
                  TabOrder = 0
                  OnClick = chkPlayObjectReduceMPClick
                end
                object seMakeFireDayPowerRate: TSpinEditEx
                  Left = 68
                  Top = 37
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMakeFireDayPowerRateChange
                end
                object seMakeFireDayTime: TSpinEditEx
                  Left = 68
                  Top = 64
                  Width = 49
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seMakeFireDayTimeChange
                end
              end
            end
            object TabSheet6: TTabSheet
              Caption = #27969#26143#28779#38632
              ImageIndex = 7
              object GroupBox61: TGroupBox
                Left = 8
                Top = 8
                Width = 129
                Height = 65
                Caption = #25915#20987#20260#23475
                TabOrder = 0
                object Label125: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label126: TLabel
                  Left = 96
                  Top = 20
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object EditSkill58PowerRate: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = EditSkill58PowerRateChange
                end
                object chkSkill58PowerTwoAttack: TCheckBox
                  Left = 8
                  Top = 40
                  Width = 73
                  Height = 17
                  Caption = #20004#27425#20260#23475
                  TabOrder = 1
                  OnClick = chkSkill58PowerTwoAttackClick
                end
              end
              object GroupBox86: TGroupBox
                Left = 8
                Top = 80
                Width = 129
                Height = 41
                Caption = #25915#20987#33539#22260
                TabOrder = 1
                object Label192: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #33539#22260':'
                end
                object EditSkill58AttackRange: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 61
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = EditSkill58AttackRangeChange
                end
              end
              object GroupBox191: TGroupBox
                Left = 8
                Top = 128
                Width = 129
                Height = 81
                Caption = #20351#29992#38388#38548#26102#38388
                TabOrder = 2
                object Label585: TLabel
                  Left = 8
                  Top = 24
                  Width = 30
                  Height = 12
                  Caption = #38388#38548':'
                end
                object Label586: TLabel
                  Left = 104
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label882: TLabel
                  Left = 8
                  Top = 48
                  Width = 30
                  Height = 12
                  Caption = #33521#38596':'
                  Visible = False
                end
                object Label883: TLabel
                  Left = 104
                  Top = 48
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object seSkill58WaitTime: TSpinEditEx
                  Left = 44
                  Top = 20
                  Width = 57
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill58WaitTimeChange
                end
                object seHeroSkill58WaitTime: TSpinEditEx
                  Left = 44
                  Top = 44
                  Width = 57
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill58WaitTimeChange
                end
              end
            end
            object TabSheet51: TTabSheet
              Caption = #28779#28976#20912
              ImageIndex = 8
              object GroupBox41: TGroupBox
                Left = 8
                Top = 8
                Width = 159
                Height = 68
                Caption = #35282#33394#31561#32423#26426#29575#35774#32622
                TabOrder = 0
                object Label101: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #30456#24046#26426#29575#65306
                end
                object Label102: TLabel
                  Left = 8
                  Top = 44
                  Width = 60
                  Height = 12
                  Caption = #30456#24046#38480#21046#65306
                end
                object EditMabMabeHitRandRate: TSpinEditEx
                  Left = 68
                  Top = 15
                  Width = 80
                  Height = 21
                  Hint = #25915#20987#34987#25915#20987#21452#26041#30456#24046#31561#32423#21629#20013#26426#29575#65292#25968#23383#36234#22823#26426#29575#36234#23567#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = EditMabMabeHitRandRateChange
                end
                object EditMabMabeHitMinLvLimit: TSpinEditEx
                  Left = 68
                  Top = 39
                  Width = 80
                  Height = 21
                  Hint = #25915#20987#34987#25915#20987#21452#26041#30456#24046#31561#32423#21629#20013#26426#29575#65292#26368#23567#38480#21046#65292#25968#23383#36234#23567#26426#29575#36234#20302#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = EditMabMabeHitMinLvLimitChange
                end
              end
              object GroupBox43: TGroupBox
                Left = 176
                Top = 8
                Width = 166
                Height = 68
                Caption = #40635#30201#26102#38388#25511#21046
                TabOrder = 1
                object Label104: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #21442#25968#20493#29575#65306
                end
                object Label862: TLabel
                  Left = 8
                  Top = 44
                  Width = 60
                  Height = 12
                  Caption = #26368#22823#26102#38271#65306
                end
                object lbl142: TLabel
                  Left = 149
                  Top = 44
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditMabMabeHitMabeTimeRate: TSpinEditEx
                  Left = 65
                  Top = 15
                  Width = 80
                  Height = 21
                  Hint = #40635#30201#26102#38388#38271#24230#20493#29575#65292#22522#25968#19982#35282#33394#30340#39764#27861#26377#20851#12290#13#10#13#10#40635#30201#26102#38388'='#39764#27861#38500#20197#20493#25968#65307#25968#36234#22823#40635#30201#26102#38388#36234#23567#12290
                  MaxValue = 100000000
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EditMabMabeHitMabeTimeRateChange
                end
                object seMaxMabMabeHitMabeTime: TSpinEditEx
                  Left = 65
                  Top = 39
                  Width = 80
                  Height = 21
                  Hint = '0'#34920#31034#19981#38480#23450#26102#38271#65292#22823#20110'0'#38480#23450#26102#38271
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seMaxMabMabeHitMabeTimeChange
                end
              end
              object GroupBox42: TGroupBox
                Left = 8
                Top = 80
                Width = 159
                Height = 44
                Caption = #40635#30201#21629#20013#26426#29575
                TabOrder = 2
                object Label103: TLabel
                  Left = 8
                  Top = 21
                  Width = 60
                  Height = 12
                  Caption = #21629#20013#26426#29575#65306
                end
                object EditMabMabeHitSucessRate: TSpinEditEx
                  Left = 68
                  Top = 15
                  Width = 80
                  Height = 21
                  Hint = #25915#20987#40635#30201#26426#29575#65292#26368#23567#38480#21046#65292#25968#23383#36234#23567#26426#29575#36234#20302#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = EditMabMabeHitSucessRateChange
                end
              end
            end
            object TabSheet94: TTabSheet
              Caption = #25239#25298#28779#29615
              ImageIndex = 9
              object GroupBox187: TGroupBox
                Left = 8
                Top = 8
                Width = 177
                Height = 57
                Caption = #25511#21046
                TabOrder = 0
                object CheckBoxFireWindPushSameLevel: TCheckBox
                  Left = 8
                  Top = 24
                  Width = 153
                  Height = 17
                  Caption = #21487#20197#25512#21160#31561#32423#30456#21516#30340#35282#33394
                  TabOrder = 0
                  OnClick = CheckBoxFireWindPushSameLevelClick
                end
              end
            end
            object ts15: TTabSheet
              Caption = #39764#27861#30462
              ImageIndex = 12
              object lbl164: TLabel
                Left = 4
                Top = 220
                Width = 222
                Height = 12
                Caption = '4'#32423#39764#27861#30462#38450#24481#20493#29575#20849#20139#24378#21270#19968#37325#38450#24481#20493#29575
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object grp38: TGroupBox
                Left = 8
                Top = 8
                Width = 117
                Height = 41
                Caption = #26222#36890#30462#38450#24481#20493#29575
                TabOrder = 0
                object lbl92: TLabel
                  Left = 8
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #20493#29575#65306
                end
                object lbl136: TLabel
                  Left = 102
                  Top = 20
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seOrdinarySkill31Rate: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 53
                  Height = 21
                  Hint = #22914#35774#32622#20026'15%,'#21017'0'#32423#39764#27861#30462#25269#24481'15%,1'#32423'-30%,2'#32423'-45%,3'#32423'-60%'
                  MaxValue = 20
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = seOrdinarySkill31RateChange
                end
              end
              object grp39: TGroupBox
                Left = 136
                Top = 56
                Width = 145
                Height = 65
                Caption = #24378#21270#30462#38450#24481#20493#29575
                TabOrder = 1
                object lbl93: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #38450#24481#20493#29575':'
                end
                object Label917: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seSkill31Rate: TSpinEditEx
                  Left = 68
                  Top = 39
                  Width = 69
                  Height = 21
                  Hint = #22914#35774#32622#20026'40%,'#21017#39764#27861#30462#25269#24481'40%'
                  MaxValue = 100
                  MinValue = 1
                  TabOrder = 0
                  Value = 40
                  OnChange = seSkill31RateChange
                end
                object cbbSkill31Level: TComboBox
                  Left = 68
                  Top = 16
                  Width = 69
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 1
                  OnChange = cbbSkill31LevelChange
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp58: TGroupBox
                Left = 136
                Top = 8
                Width = 257
                Height = 41
                Caption = #23458#25143#31471#29305#25928
                TabOrder = 2
                object chkSkill31UseNewEffect: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 117
                  Height = 17
                  Caption = #20351#29992#26032#39764#27861#30462#29305#25928
                  TabOrder = 0
                  OnClick = chkSkill31UseNewEffectClick
                end
                object chkSkill31UseLEGEffect: TCheckBox
                  Left = 144
                  Top = 16
                  Width = 97
                  Height = 17
                  Hint = #21246#36873#21518#21017'4'#32423#30462#20026'LEG'#26679#24335#24425#30462#65292#19981#21246#36873#21017#25745#30462#30636#38388#24425#33394
                  Caption = 'LEG'#22235#32423#30462#25928#26524
                  TabOrder = 1
                  OnClick = chkSkill31UseLEGEffectClick
                end
              end
              object GroupBox247: TGroupBox
                Left = 8
                Top = 56
                Width = 117
                Height = 41
                Caption = #22124#39746#27836#27901#25269#24481#22686#21152
                TabOrder = 3
                object Label885: TLabel
                  Left = 8
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #20493#29575#65306
                end
                object Label914: TLabel
                  Left = 102
                  Top = 20
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill113RateAddWithSkill63: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 53
                  Height = 21
                  Hint = #22686#21152#23545#22124#39746#27836#27901#25216#33021#30340#20260#23475#25269#24481#30334#20998#27604#13#10#13#10#20248#20808#35745#31639#23545#22124#39746#27836#27901#25269#24481#22686#21152#65292#20877#35745#31639#40664#35748#38450#24481#20493#29575
                  MaxValue = 100
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill113RateAddWithSkill63Change
                end
              end
            end
            object ts22: TTabSheet
              Caption = #25307#39746#26415
              ImageIndex = 14
              object GroupBox229: TGroupBox
                Left = 4
                Top = 51
                Width = 167
                Height = 67
                Caption = #21484#21796#20960#29575
                TabOrder = 0
                object lbl125: TLabel
                  Left = 8
                  Top = 19
                  Width = 60
                  Height = 12
                  Caption = #25216#33021#31561#32423#65306
                end
                object Label582: TLabel
                  Left = 8
                  Top = 43
                  Width = 60
                  Height = 12
                  Caption = #21484#21796#20960#29575#65306
                end
                object cbbSpiritualismMagicLevel: TComboBox
                  Left = 66
                  Top = 15
                  Width = 94
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbSpiritualismMagicLevelChange
                  Items.Strings = (
                    '0'#32423#25216#33021
                    '1'#32423#25216#33021
                    '2'#32423#25216#33021
                    '3'#32423#25216#33021
                    #24378#21270'1'#37325
                    #24378#21270'2'#37325
                    #24378#21270'3'#37325
                    #24378#21270'4'#37325
                    #24378#21270'5'#37325
                    #24378#21270'6'#37325
                    #24378#21270'7'#37325
                    #24378#21270'8'#37325
                    #24378#21270'9'#37325)
                end
                object seSpiritualismRate: TSpinEditEx
                  Left = 66
                  Top = 38
                  Width = 94
                  Height = 21
                  Hint = #25968#23383#36234#23567#65292#20960#29575#36234#39640#65307'0'#34920#31034#26080#20960#29575
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 1
                  Value = 2
                  OnChange = seSpiritualismRateChange
                end
              end
              object GroupBox230: TGroupBox
                Left = 176
                Top = 4
                Width = 137
                Height = 43
                Caption = #24310#38271#21467#21464#26102#38388
                TabOrder = 1
                object Label786: TLabel
                  Left = 8
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #22522#25968#65306
                end
                object lbl126: TLabel
                  Left = 104
                  Top = 20
                  Width = 24
                  Height = 12
                  Caption = #20998#38047
                end
                object seSpiritualismRoyaltyTime: TSpinEditEx
                  Left = 42
                  Top = 15
                  Width = 61
                  Height = 21
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 60
                  OnChange = seSpiritualismRoyaltyTimeChange
                end
              end
              object GroupBox188: TGroupBox
                Left = 4
                Top = 4
                Width = 167
                Height = 43
                Caption = #31561#32423#38480#21046
                TabOrder = 2
                object chkSpiritualismLevelDiff: TCheckBox
                  Left = 8
                  Top = 17
                  Width = 83
                  Height = 17
                  Caption = #21028#26029#31561#32423#24046
                  TabOrder = 0
                  OnClick = chkSpiritualismLevelDiffClick
                end
                object seSpiritualismLevelDiff: TSpinEditEx
                  Left = 90
                  Top = 14
                  Width = 71
                  Height = 21
                  Hint = #25429#25417#32773#31561#32423'-'#24618#29289#31561#32423'>='#31561#32423#24046#65292#21487#20197#20026#36127#25968
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 1
                  Value = 2
                  OnChange = seSpiritualismLevelDiffChange
                end
              end
              object GroupBox228: TGroupBox
                Left = 176
                Top = 51
                Width = 137
                Height = 41
                Caption = #38480#21046#25307#39746#25968#37327
                TabOrder = 3
                object Label783: TLabel
                  Left = 8
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #25968#37327#65306
                end
                object seSpiritualismBBCount: TSpinEditEx
                  Left = 42
                  Top = 15
                  Width = 61
                  Height = 21
                  Hint = #21487#25307#39746#24618#29289#25968#37327#12290
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seSpiritualismBBCountChange
                end
              end
              object chkSpiritualismDisableUndeadMon: TCheckBox
                Left = 176
                Top = 96
                Width = 137
                Height = 17
                Caption = #31105#27490#25307#19981#27515#31995#24618#29289
                TabOrder = 4
                OnClick = chkSpiritualismDisableUndeadMonClick
              end
            end
            object ts23: TTabSheet
              Caption = #38647#30005'/'#32676#38647#26415
              ImageIndex = 15
              object GroupBox241: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 68
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label829: TLabel
                  Left = 8
                  Top = 19
                  Width = 108
                  Height = 12
                  Caption = #22522#30784#25915#20987#33539#22260#35774#32622#65306
                end
                object Label828: TLabel
                  Left = 8
                  Top = 43
                  Width = 96
                  Height = 12
                  Caption = #21319#32423#21152#25915#20987#33539#22260#65306
                end
                object seSkill37Range: TSpinEditEx
                  Left = 116
                  Top = 14
                  Width = 45
                  Height = 21
                  Hint = '0'#32423#25216#33021#25915#20987#33539#22260
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seSkill37RangeChange
                end
                object seSkill37RangeAdd: TSpinEditEx
                  Left = 116
                  Top = 38
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#31561#32423#21319#27599'1'#32423#65292#21152#25915#20987#33539#22260#20540
                  MaxValue = 12
                  MinValue = 0
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill37RangeAddChange
                end
              end
              object GroupBox245: TGroupBox
                Left = 8
                Top = 80
                Width = 169
                Height = 73
                Caption = #25216#33021#23041#21147
                TabOrder = 1
                object Label863: TLabel
                  Left = 8
                  Top = 22
                  Width = 72
                  Height = 12
                  Caption = #38647#30005#26415#23041#21147#65306
                end
                object Label864: TLabel
                  Left = 8
                  Top = 46
                  Width = 72
                  Height = 12
                  Caption = #32676#38647#26415#23041#21147#65306
                end
                object Label865: TLabel
                  Left = 133
                  Top = 46
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label866: TLabel
                  Left = 133
                  Top = 22
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkillGroupLighteningPowerRate: TSpinEditEx
                  Left = 79
                  Top = 42
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkillGroupLighteningPowerRateChange
                end
                object seSkillLighteningPowerRate: TSpinEditEx
                  Left = 79
                  Top = 18
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seSkillLighteningPowerRateChange
                end
              end
            end
          end
        end
        object TabSheet64: TTabSheet
          Caption = #36947#22763#25216#33021
          ImageIndex = 3
          object PageControl3: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            Align = alClient
            TabOrder = 1
          end
          object PageControl6: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            ActivePage = TabSheet22
            Align = alClient
            MultiLine = True
            TabOrder = 0
            object ts1: TTabSheet
              Caption = #28789#39746#28779#31526'/'#35010#31070#31526
              ImageIndex = 11
              object grp3: TGroupBox
                Left = 5
                Top = 2
                Width = 180
                Height = 191
                Caption = #35010#31070#31526#20998#35010#35774#32622
                TabOrder = 0
                object lbl3: TLabel
                  Left = 32
                  Top = 18
                  Width = 84
                  Height = 12
                  Caption = #22522#30784#20998#35010#27425#25968#65306
                end
                object lbl2: TLabel
                  Left = 8
                  Top = 42
                  Width = 108
                  Height = 12
                  Caption = #27599#32423#22686#21152#20998#35010#27425#25968#65306
                end
                object lbl18: TLabel
                  Left = 8
                  Top = 66
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label785: TLabel
                  Left = 32
                  Top = 90
                  Width = 84
                  Height = 12
                  Caption = #20998#35010#20960#29575#35774#32622#65306
                end
                object Label938: TLabel
                  Left = 10
                  Top = 163
                  Width = 84
                  Height = 12
                  Caption = #20998#35010#23041#21147#20445#24213#65306
                end
                object Label939: TLabel
                  Left = 144
                  Top = 163
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label940: TLabel
                  Left = 10
                  Top = 139
                  Width = 84
                  Height = 12
                  Caption = #20998#35010#23041#21147#34928#20943#65306
                end
                object Label941: TLabel
                  Left = 144
                  Top = 139
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkill202BaseCount: TSpinEditEx
                  Left = 116
                  Top = 15
                  Width = 53
                  Height = 21
                  Hint = #25216#33021#31561#32423'0'#32423#26102#30340#20998#35010#27425#25968#65292#40664#35748'2'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 0
                  Value = 2
                  OnChange = seSkill202BaseCountChange
                end
                object seSkill202LevelupCount: TSpinEditEx
                  Left = 116
                  Top = 39
                  Width = 53
                  Height = 21
                  Hint = #25216#33021#31561#32423#27599#19978#21319#19968#32423#26102#22686#21152#30340#20998#35010#27425#25968#65292#40664#35748'1'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill202LevelupCountChange
                end
                object seSkill202CD: TSpinEditEx
                  Left = 116
                  Top = 63
                  Width = 53
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'5'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 2
                  Value = 5
                  OnChange = seSkill202CDChange
                end
                object seSkill202Rate: TSpinEditEx
                  Left = 116
                  Top = 87
                  Width = 53
                  Height = 21
                  Hint = #20998#35010#20960#29575#65306#20540#36234#22823#20960#29575#36234#23567
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 5
                  OnChange = seSkill202RateChange
                end
                object chkSkill202RateOnlyMon: TCheckBox
                  Left = 76
                  Top = 112
                  Width = 97
                  Height = 17
                  Caption = #21482#23545#24618#29289#20998#35010
                  TabOrder = 4
                  OnClick = chkSkill202RateOnlyMonClick
                end
                object seSkill202PowerMin: TSpinEditEx
                  Left = 92
                  Top = 159
                  Width = 49
                  Height = 21
                  Hint = #20445#24213#23041#21147
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 5
                  Value = 100
                  OnChange = seSkill202PowerMinChange
                end
                object seSkill202PowerDec: TSpinEditEx
                  Left = 92
                  Top = 135
                  Width = 49
                  Height = 21
                  Hint = #27599#20998#35010#19968#27425#25216#33021#23041#21147#20943#23569#19968#23450#27604#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 6
                  Value = 100
                  OnChange = seSkill202PowerDecChange
                end
              end
              object GroupBox91: TGroupBox
                Left = 192
                Top = 2
                Width = 197
                Height = 69
                Caption = #25216#33021#23041#21147
                TabOrder = 1
                object Label228: TLabel
                  Left = 8
                  Top = 45
                  Width = 96
                  Height = 12
                  Caption = #35010#31070#31526#23041#21147#20493#25968#65306
                end
                object Label229: TLabel
                  Left = 165
                  Top = 45
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label227: TLabel
                  Left = 8
                  Top = 21
                  Width = 108
                  Height = 12
                  Caption = #28789#39746#28779#31526#23041#21147#20493#25968#65306
                end
                object Label230: TLabel
                  Left = 165
                  Top = 21
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkill202PowerRate: TSpinEditEx
                  Left = 113
                  Top = 41
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill202PowerRateChange
                end
                object seSkillFireCharmPowerRate: TSpinEditEx
                  Left = 113
                  Top = 17
                  Width = 49
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seSkillFireCharmPowerRateChange
                end
              end
            end
            object ts2: TTabSheet
              Caption = #27515#20129#20043#30524
              ImageIndex = 12
              object grp4: TGroupBox
                Left = 13
                Top = 9
                Width = 167
                Height = 224
                Caption = #25216#33021#21442#25968
                TabOrder = 0
                object lbl4: TLabel
                  Left = 10
                  Top = 47
                  Width = 84
                  Height = 12
                  Caption = #40635#30201#22522#30784#26102#38271#65306
                end
                object lbl7: TLabel
                  Left = 10
                  Top = 71
                  Width = 84
                  Height = 12
                  Caption = #27599#32423#22686#21152#26102#38388#65306
                end
                object lbl8: TLabel
                  Left = 146
                  Top = 47
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object lbl9: TLabel
                  Left = 146
                  Top = 71
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label884: TLabel
                  Left = 10
                  Top = 21
                  Width = 84
                  Height = 12
                  Caption = #40635#30201#20960#29575#35774#32622#65306
                end
                object seSkill203BasicMbTimer: TSpinEditEx
                  Left = 91
                  Top = 42
                  Width = 51
                  Height = 21
                  Hint = #40635#30201#22522#30784#26102#38271#65292#40664#35748'2'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 0
                  Value = 2
                  OnChange = seSkill203BasicMbTimerChange
                end
                object chkSkill203MbAttackMon: TCheckBox
                  Left = 10
                  Top = 93
                  Width = 107
                  Height = 17
                  Caption = #20801#35768#40635#30201#24618#29289
                  TabOrder = 2
                  OnClick = chkSkill203MbAttackMonClick
                end
                object chkSkill203MbAttackHuman: TCheckBox
                  Left = 10
                  Top = 114
                  Width = 107
                  Height = 17
                  Caption = #20801#35768#40635#30201#20154#29289
                  TabOrder = 3
                  OnClick = chkSkill203MbAttackHumanClick
                end
                object chkSkill203MbAttackSlave: TCheckBox
                  Left = 10
                  Top = 135
                  Width = 135
                  Height = 17
                  Caption = #20801#35768#40635#30201#23453#23453#12289#33521#38596
                  TabOrder = 4
                  OnClick = chkSkill203MbAttackSlaveClick
                end
                object chkSkill203Damagearmor: TCheckBox
                  Left = 10
                  Top = 155
                  Width = 107
                  Height = 17
                  Caption = #20801#35768#20013#32418#27602
                  TabOrder = 5
                  OnClick = chkSkill203DamagearmorClick
                end
                object chkSkill203DecHealth: TCheckBox
                  Left = 10
                  Top = 176
                  Width = 107
                  Height = 17
                  Caption = #20801#35768#20013#32511#27602
                  TabOrder = 6
                  OnClick = chkSkill203DecHealthClick
                end
                object chkSkill203MbFastParalysis: TCheckBox
                  Left = 10
                  Top = 197
                  Width = 107
                  Height = 17
                  Hint = #24403#20154#29289#25110#24618#29289#34987#35813#25216#33021#40635#30201#21518#65292#26159#21542#34987#25915#20987#39532#19978#21462#28040#40635#30201#29366#24577#12290
                  Caption = #26159#21542#24555#36895#40635#30201
                  TabOrder = 7
                  OnClick = chkSkill203MbFastParalysisClick
                end
                object seSkill203LevelupMbTimer: TSpinEditEx
                  Left = 91
                  Top = 66
                  Width = 51
                  Height = 21
                  Hint = #25216#33021#31561#32423#27599#19978#21319#19968#32423#26102#22686#21152#30340#40635#30201#26102#38388#65292#40664#35748'1'
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill203LevelupMbTimerChange
                end
                object seSkill203PoisonRate: TSpinEditEx
                  Left = 91
                  Top = 18
                  Width = 51
                  Height = 21
                  Hint = #40635#30201#20960#29575#65306#20540#36234#22823#20960#29575#36234#23567
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 8
                  Value = 5
                  OnChange = seSkill203PoisonRateChange
                end
              end
              object grp5: TGroupBox
                Left = 186
                Top = 9
                Width = 191
                Height = 128
                Caption = #25216#33021#21442#25968
                TabOrder = 1
                object lbl5: TLabel
                  Left = 42
                  Top = 24
                  Width = 84
                  Height = 12
                  Caption = #22522#30784#23041#21147#20493#25968#65306
                end
                object lbl6: TLabel
                  Left = 18
                  Top = 51
                  Width = 108
                  Height = 12
                  Caption = #27599#32423#22686#21152#23041#21147#20493#25968#65306
                end
                object lbl21: TLabel
                  Left = 18
                  Top = 77
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object Label589: TLabel
                  Left = 18
                  Top = 104
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object seSkill203BasicPowerRate: TSpinEditEx
                  Left = 124
                  Top = 19
                  Width = 51
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill203BasicPowerRateChange
                end
                object seSkill203LevelupPowerRate: TSpinEditEx
                  Left = 124
                  Top = 46
                  Width = 51
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 1
                  Value = 50
                  OnChange = seSkill203LevelupPowerRateChange
                end
                object seSkill203CD: TSpinEditEx
                  Left = 124
                  Top = 72
                  Width = 51
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'15'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 2
                  Value = 15
                  OnChange = seSkill203CDChange
                end
                object seSkill203Rage: TSpinEditEx
                  Left = 124
                  Top = 99
                  Width = 51
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 3
                  Value = 1
                  OnChange = seSkill203RageChange
                end
              end
            end
            object ts18: TTabSheet
              Caption = #24189#20901#28779#31526
              ImageIndex = 13
              object grp46: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 105
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object lbl101: TLabel
                  Left = 8
                  Top = 18
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388'('#31186')'#65306
                end
                object lbl102: TLabel
                  Left = 8
                  Top = 48
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#25915#20987#33539#22260#35774#32622#65306
                end
                object lbl103: TLabel
                  Left = 8
                  Top = 76
                  Width = 108
                  Height = 12
                  Caption = #25216#33021#23041#21147#20493#25968#35774#32622#65306
                end
                object seSKILL210CD: TSpinEditEx
                  Left = 116
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25216#33021#20919#21364#26102#38388#65292#40664#35748'5'
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 0
                  Value = 5
                  OnChange = seSKILL210CDChange
                end
                object seSkill210Rage: TSpinEditEx
                  Left = 116
                  Top = 43
                  Width = 45
                  Height = 21
                  Hint = #40664#35748#25915#20987#33539#22260'3x3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill210RageChange
                end
                object seSkill210PowerRate: TSpinEditEx
                  Left = 115
                  Top = 73
                  Width = 45
                  Height = 21
                  Hint = #23454#38469#20493#25968#31561#20110#24403#21069#25968#23383#38500#20197'100'
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 2
                  Value = 100
                  OnChange = seSkill210PowerRateChange
                end
              end
            end
            object TabSheet20: TTabSheet
              Caption = #26045#27602#26415'/'#32676#27602#26415
              object GroupBox16: TGroupBox
                Left = 8
                Top = 8
                Width = 137
                Height = 43
                Caption = #27602#33647#38477#28857
                TabOrder = 0
                object Label11: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #28857#25968#25511#21046#65306
                end
                object EditAmyOunsulPoint: TSpinEditEx
                  Left = 68
                  Top = 16
                  Width = 53
                  Height = 21
                  Hint = #20013#27602#21518#25351#23450#26102#38388#20869#38477#28857#25968#65292#23454#38469#28857#25968#36319#25216#33021#31561#32423#21450#26412#36523#36947#26415#39640#20302#26377#20851#65292#27492#21442#25968#21482#26159#35843#20854#20013#31639#27861#21442#25968#65292#27492#25968#23383#36234#23567#65292#28857#25968#36234#22823#12290
                  MaxValue = 100
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EdiAmyOunsulPointChange
                end
              end
              object GroupBox182: TGroupBox
                Left = 8
                Top = 57
                Width = 137
                Height = 67
                Caption = #27602#25345#32493#26102#38388
                TabOrder = 2
                object Label570: TLabel
                  Left = 8
                  Top = 20
                  Width = 36
                  Height = 12
                  Caption = #26102#38271#65306
                end
                object Label571: TLabel
                  Left = 8
                  Top = 44
                  Width = 36
                  Height = 12
                  Caption = #26368#22823#65306
                end
                object Label572: TLabel
                  Left = 104
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object Label573: TLabel
                  Left = 104
                  Top = 43
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditAmyOunsulTimeRate: TSpinEditEx
                  Left = 44
                  Top = 16
                  Width = 53
                  Height = 21
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EditAmyOunsulTimeRateChange
                end
                object EditAmyOunsulMaxTime: TSpinEditEx
                  Left = 44
                  Top = 40
                  Width = 53
                  Height = 21
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = EditAmyOunsulMaxTimeChange
                end
              end
              object GroupBox195: TGroupBox
                Left = 288
                Top = 8
                Width = 128
                Height = 42
                Caption = #20449#24687#25552#31034
                TabOrder = 4
                object CheckBoxShowYouPoisoned: TCheckBox
                  Left = 8
                  Top = 17
                  Width = 97
                  Height = 17
                  Caption = #25552#31034#20013#27602#20449#24687
                  TabOrder = 0
                  OnClick = CheckBoxShowYouPoisonedClick
                end
              end
              object grp12: TGroupBox
                Left = 152
                Top = 127
                Width = 128
                Height = 65
                Caption = #32511#27602#20943'HP'#26102#38388'('#27627#31186')'
                TabOrder = 3
                object lbl33: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #38388#38548#26102#38388#65306
                end
                object sePosionDecHealthTime: TSpinEditEx
                  Left = 66
                  Top = 16
                  Width = 55
                  Height = 21
                  Hint = #20154#29289#20013#32511#27602#21518','#20943#23569#29983#21629#26102#38388#38388#38548
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 0
                  Value = 2500
                  OnChange = sePosionDecHealthTimeChange
                end
                object chkPosionStopIncHealth: TCheckBox
                  Left = 8
                  Top = 41
                  Width = 113
                  Height = 17
                  Caption = #20013#32511#27602#20572#27490#22238#34880
                  TabOrder = 1
                  OnClick = chkPosionStopIncHealthClick
                end
              end
              object grp13: TGroupBox
                Left = 152
                Top = 8
                Width = 128
                Height = 43
                Caption = #32418#27602#20943#38450#24481#21450#25345#20037#29575
                TabOrder = 1
                object lbl34: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #25345#20037#27604#29575#65306
                end
                object sePosionDamagarmor: TSpinEditEx
                  Left = 66
                  Top = 16
                  Width = 55
                  Height = 21
                  Hint = #20154#29289#20013#32418#27602#20943#38450#24481#21450#29289#21697#25345#20037#27604#29575#65292#27492#25968#20540#38500#20197'10'#20026#30495#23454#25968#20540
                  MaxValue = 20000
                  MinValue = 10
                  TabOrder = 0
                  Value = 10
                  OnChange = sePosionDamagarmorChange
                end
              end
              object GroupBox113: TGroupBox
                Left = 152
                Top = 57
                Width = 128
                Height = 67
                Caption = #32418#27602#20943#39764#38450
                TabOrder = 5
                object Label780: TLabel
                  Left = 8
                  Top = 44
                  Width = 60
                  Height = 12
                  Caption = #20943#38450#27604#29575#65306
                end
                object lbl124: TLabel
                  Left = 112
                  Top = 46
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object sePosionDecMACRate: TSpinEditEx
                  Left = 66
                  Top = 40
                  Width = 43
                  Height = 21
                  Hint = #20154#29289#20013#32418#27602#20943#39764#38450#27604#29575#65292#27492#25968#20540#38500#20197'10'#20026#30495#23454#25968#20540
                  MaxValue = 100
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = sePosionDecMACRateChange
                end
                object chkEnabledPosionDecMAC: TCheckBox
                  Left = 8
                  Top = 18
                  Width = 113
                  Height = 17
                  Caption = #24320#21551#32418#27602#20943#39764#38450
                  TabOrder = 1
                  OnClick = chkEnabledPosionDecMACClick
                end
              end
              object GroupBox232: TGroupBox
                Left = 8
                Top = 127
                Width = 137
                Height = 66
                Caption = #20351#29992#38388#38548
                TabOrder = 6
                object Label787: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #26045#27602#38388#38548#65306
                end
                object Label788: TLabel
                  Left = 118
                  Top = 20
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label789: TLabel
                  Left = 8
                  Top = 44
                  Width = 60
                  Height = 12
                  Caption = #32676#27602#38388#38548#65306
                end
                object Label790: TLabel
                  Left = 118
                  Top = 44
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seSkillAmyounsulCDTime: TSpinEditEx
                  Left = 67
                  Top = 16
                  Width = 48
                  Height = 21
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkillAmyounsulCDTimeChange
                end
                object seSkillGroupAmyounsulCDTime: TSpinEditEx
                  Left = 67
                  Top = 40
                  Width = 48
                  Height = 21
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = seSkillGroupAmyounsulCDTimeChange
                end
              end
              object GroupBox233: TGroupBox
                Left = 288
                Top = 57
                Width = 128
                Height = 67
                Caption = #32676#27602#26415#36873#39033
                TabOrder = 7
                object chkSkillGroupAmyounsulRed: TCheckBox
                  Left = 8
                  Top = 18
                  Width = 113
                  Height = 17
                  Caption = #20801#35768#32418#27602
                  TabOrder = 0
                  OnClick = chkSkillGroupAmyounsulRedClick
                end
                object chkSkillGroupAmyounsulGreen: TCheckBox
                  Left = 8
                  Top = 41
                  Width = 113
                  Height = 17
                  Caption = #20801#35768#32511#27602
                  TabOrder = 1
                  OnClick = chkSkillGroupAmyounsulGreenClick
                end
              end
            end
            object TabSheet21: TTabSheet
              Caption = #21484#21796#39607#39621
              ImageIndex = 1
              object GroupBox5: TGroupBox
                Left = 5
                Top = 2
                Width = 132
                Height = 104
                Caption = #22522#26412#35774#32622
                TabOrder = 0
                object Label2: TLabel
                  Left = 8
                  Top = 18
                  Width = 54
                  Height = 12
                  Caption = #24618#29289#21517#31216':'
                end
                object Label3: TLabel
                  Left = 8
                  Top = 58
                  Width = 54
                  Height = 12
                  Caption = #21484#21796#25968#37327':'
                end
                object Label663: TLabel
                  Left = 8
                  Top = 81
                  Width = 54
                  Height = 12
                  Caption = #21484#21796#38388#38548':'
                end
                object Label664: TLabel
                  Left = 114
                  Top = 81
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditBoneFammName: TEdit
                  Left = 8
                  Top = 32
                  Width = 105
                  Height = 20
                  Hint = #35774#32622#40664#35748#21484#21796#30340#24618#29289#21517#31216#12290
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = EditBoneFammNameChange
                end
                object EditBoneFammCount: TSpinEditEx
                  Left = 60
                  Top = 55
                  Width = 53
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26368#22823#25968#37327#12290
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = EditBoneFammCountChange
                end
                object seRecallBoneFammWaitTime: TSpinEditEx
                  Left = 60
                  Top = 78
                  Width = 53
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26102#38388#38388#38548#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seRecallBoneFammWaitTimeChange
                end
              end
              object GroupBox6: TGroupBox
                Left = 144
                Top = 2
                Width = 289
                Height = 123
                Caption = #39640#32423#35774#32622
                TabOrder = 1
                object GridBoneFamm: TStringGrid
                  Left = 8
                  Top = 16
                  Width = 265
                  Height = 100
                  ColCount = 4
                  DefaultRowHeight = 18
                  FixedCols = 0
                  RowCount = 11
                  Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goEditing]
                  TabOrder = 0
                  OnSetEditText = GridBoneFammSetEditText
                  ColWidths = (
                    55
                    76
                    57
                    52)
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
                    18)
                end
              end
              object grp49: TGroupBox
                Left = 5
                Top = 128
                Width = 132
                Height = 104
                Caption = #25351#23450#24378#21270#21484#21796#39607#39621#21517
                TabOrder = 2
                object lbl108: TLabel
                  Left = 8
                  Top = 19
                  Width = 36
                  Height = 12
                  Caption = '1-3'#37325':'
                end
                object Label674: TLabel
                  Left = 8
                  Top = 40
                  Width = 36
                  Height = 12
                  Caption = '4-6'#37325':'
                end
                object lbl109: TLabel
                  Left = 8
                  Top = 61
                  Width = 36
                  Height = 12
                  Caption = '7-9'#37325':'
                end
                object Label698: TLabel
                  Left = 8
                  Top = 82
                  Width = 36
                  Height = 12
                  Caption = '9'#37325#21518':'
                end
                object edtPlusBoneFammName1_3: TEdit
                  Left = 44
                  Top = 15
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = EditBoneFammNameChange
                end
                object edtPlusBoneFammName4_6: TEdit
                  Left = 44
                  Top = 36
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 1
                  OnChange = EditBoneFammNameChange
                end
                object edtPlusBoneFammName7_9: TEdit
                  Left = 44
                  Top = 57
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 2
                  OnChange = EditBoneFammNameChange
                end
                object edtPlusBoneFammName9_N: TEdit
                  Left = 44
                  Top = 78
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 3
                  OnChange = EditBoneFammNameChange
                end
              end
              object GroupBox213: TGroupBox
                Left = 144
                Top = 128
                Width = 289
                Height = 104
                Caption = #25351#23450#24378#21270#21484#21796#39607#39621#31561#32423
                TabOrder = 3
                object Label680: TLabel
                  Left = 8
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '1'#37325':'
                end
                object Label681: TLabel
                  Left = 8
                  Top = 41
                  Width = 24
                  Height = 12
                  Caption = '4'#37325':'
                end
                object Label682: TLabel
                  Left = 8
                  Top = 63
                  Width = 24
                  Height = 12
                  Caption = '7'#37325':'
                end
                object Label683: TLabel
                  Left = 100
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '2'#37325':'
                end
                object Label684: TLabel
                  Left = 192
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '3'#37325':'
                end
                object Label685: TLabel
                  Left = 100
                  Top = 41
                  Width = 24
                  Height = 12
                  Caption = '5'#37325':'
                end
                object Label686: TLabel
                  Left = 192
                  Top = 41
                  Width = 24
                  Height = 12
                  Caption = '6'#37325':'
                end
                object Label687: TLabel
                  Left = 100
                  Top = 63
                  Width = 24
                  Height = 12
                  Caption = '8'#37325':'
                end
                object Label688: TLabel
                  Left = 192
                  Top = 63
                  Width = 24
                  Height = 12
                  Caption = '9'#37325':'
                end
                object lbl110: TLabel
                  Left = 8
                  Top = 84
                  Width = 210
                  Height = 12
                  Caption = '9'#37325#20197#21518#27599#22686#21152'1'#37325#25216#33021#31561#32423','#23453#23453#22686#21152#65306
                end
                object sePlusBoneFammLevel1: TSpinEditEx
                  Left = 36
                  Top = 15
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel2: TSpinEditEx
                  Left = 128
                  Top = 15
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel3: TSpinEditEx
                  Left = 220
                  Top = 15
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 2
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel4: TSpinEditEx
                  Left = 36
                  Top = 37
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 3
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel5: TSpinEditEx
                  Left = 128
                  Top = 37
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 4
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel6: TSpinEditEx
                  Left = 220
                  Top = 37
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 5
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel7: TSpinEditEx
                  Left = 36
                  Top = 58
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 6
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel8: TSpinEditEx
                  Left = 128
                  Top = 58
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 7
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevel9: TSpinEditEx
                  Left = 220
                  Top = 58
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 8
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
                object sePlusBoneFammLevelAfter9: TSpinEditEx
                  Left = 220
                  Top = 80
                  Width = 52
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 9
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
              end
              object chkBonePlugSettingPriority: TCheckBox
                Left = 4
                Top = 108
                Width = 140
                Height = 17
                Hint = #24403#20154#29289#31561#32423#28385#36275#39640#32423#35774#32622#65292#32780#25216#33021#21448#36827#34892#20102#24378#21270#26102#65292#21246#36873#20123#36873#39033#65292#23453#23453#21517#21450#31561#32423#65292#20197#24378#21270#20026#20934
                Caption = #24378#21270#37197#32622#20248#20808#39640#32423#35774#32622
                TabOrder = 4
                OnClick = chkBonePlugSettingPriorityClick
              end
            end
            object TabSheet22: TTabSheet
              Caption = #21484#21796#31070#20861
              ImageIndex = 2
              object GroupBox11: TGroupBox
                Left = 5
                Top = 2
                Width = 132
                Height = 85
                Caption = #22522#26412#35774#32622
                TabOrder = 0
                object Label5: TLabel
                  Left = 8
                  Top = 18
                  Width = 42
                  Height = 12
                  Caption = #24618#29289#21517':'
                end
                object Label6: TLabel
                  Left = 8
                  Top = 39
                  Width = 54
                  Height = 12
                  Caption = #21484#21796#25968#37327':'
                end
                object Label661: TLabel
                  Left = 8
                  Top = 62
                  Width = 54
                  Height = 12
                  Caption = #21484#21796#38388#38548':'
                end
                object Label662: TLabel
                  Left = 114
                  Top = 62
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditDogzName: TEdit
                  Left = 50
                  Top = 14
                  Width = 74
                  Height = 20
                  Hint = #35774#32622#40664#35748#21484#21796#30340#24618#29289#21517#31216#12290
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = EditDogzNameChange
                end
                object EditDogzCount: TSpinEditEx
                  Left = 60
                  Top = 36
                  Width = 53
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26368#22823#25968#37327#12290
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = EditDogzCountChange
                end
                object seRecallDogzWaitTime: TSpinEditEx
                  Left = 60
                  Top = 59
                  Width = 53
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26102#38388#38388#38548#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seRecallDogzWaitTimeChange
                end
              end
              object GroupBox12: TGroupBox
                Left = 152
                Top = 2
                Width = 281
                Height = 121
                Caption = #39640#32423#35774#32622
                TabOrder = 1
                object GridDogz: TStringGrid
                  Left = 8
                  Top = 16
                  Width = 265
                  Height = 99
                  ColCount = 4
                  DefaultRowHeight = 18
                  FixedCols = 0
                  RowCount = 11
                  Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goEditing]
                  TabOrder = 0
                  OnSetEditText = GridBoneFammSetEditText
                  ColWidths = (
                    55
                    76
                    57
                    52)
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
                    18)
                end
              end
              object chkDogzGotoMaster: TCheckBox
                Left = 5
                Top = 90
                Width = 132
                Height = 17
                Caption = #37325#22797#21484#21796#22238#20154#29289#36523#36793
                TabOrder = 2
                OnClick = chkDogzGotoMasterClick
              end
              object GroupBox211: TGroupBox
                Left = 5
                Top = 129
                Width = 132
                Height = 103
                Caption = #25351#23450#24378#21270#21484#21796#31070#20861#21517
                TabOrder = 3
                object Label675: TLabel
                  Left = 8
                  Top = 19
                  Width = 36
                  Height = 12
                  Caption = '1-3'#37325':'
                end
                object Label676: TLabel
                  Left = 8
                  Top = 40
                  Width = 36
                  Height = 12
                  Caption = '4-6'#37325':'
                end
                object Label677: TLabel
                  Left = 8
                  Top = 61
                  Width = 36
                  Height = 12
                  Caption = '7-9'#37325':'
                end
                object Label699: TLabel
                  Left = 8
                  Top = 82
                  Width = 36
                  Height = 12
                  Caption = '9'#37325#21518':'
                end
                object edtPlusDogzName1_3: TEdit
                  Left = 44
                  Top = 15
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = EditDogzNameChange
                end
                object edtPlusDogzName4_6: TEdit
                  Left = 44
                  Top = 36
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 1
                  OnChange = EditDogzNameChange
                end
                object edtPlusDogzName7_9: TEdit
                  Left = 44
                  Top = 57
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 2
                  OnChange = EditDogzNameChange
                end
                object edtPlusDogzName9_N: TEdit
                  Left = 44
                  Top = 78
                  Width = 80
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 3
                  OnChange = EditBoneFammNameChange
                end
              end
              object GroupBox214: TGroupBox
                Left = 152
                Top = 129
                Width = 281
                Height = 103
                Caption = #25351#23450#24378#21270#21484#21796#31070#20861#31561#32423
                TabOrder = 4
                object Label689: TLabel
                  Left = 8
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '1'#37325':'
                end
                object Label690: TLabel
                  Left = 8
                  Top = 40
                  Width = 24
                  Height = 12
                  Caption = '4'#37325':'
                end
                object Label691: TLabel
                  Left = 8
                  Top = 61
                  Width = 24
                  Height = 12
                  Caption = '7'#37325':'
                end
                object Label692: TLabel
                  Left = 100
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '2'#37325':'
                end
                object Label693: TLabel
                  Left = 192
                  Top = 19
                  Width = 24
                  Height = 12
                  Caption = '3'#37325':'
                end
                object Label694: TLabel
                  Left = 100
                  Top = 40
                  Width = 24
                  Height = 12
                  Caption = '5'#37325':'
                end
                object Label695: TLabel
                  Left = 192
                  Top = 40
                  Width = 24
                  Height = 12
                  Caption = '6'#37325':'
                end
                object Label696: TLabel
                  Left = 100
                  Top = 61
                  Width = 24
                  Height = 12
                  Caption = '8'#37325':'
                end
                object Label697: TLabel
                  Left = 192
                  Top = 61
                  Width = 24
                  Height = 12
                  Caption = '9'#37325':'
                end
                object Label700: TLabel
                  Left = 8
                  Top = 84
                  Width = 210
                  Height = 12
                  Caption = '9'#37325#20197#21518#27599#22686#21152'1'#37325#25216#33021#31561#32423','#23453#23453#22686#21152#65306
                end
                object sePlusDogzLevel1: TSpinEditEx
                  Left = 36
                  Top = 15
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel2: TSpinEditEx
                  Left = 128
                  Top = 15
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel3: TSpinEditEx
                  Left = 220
                  Top = 15
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 2
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel4: TSpinEditEx
                  Left = 36
                  Top = 36
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 3
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel5: TSpinEditEx
                  Left = 128
                  Top = 36
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 4
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel6: TSpinEditEx
                  Left = 220
                  Top = 36
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 5
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel7: TSpinEditEx
                  Left = 36
                  Top = 57
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 6
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel8: TSpinEditEx
                  Left = 128
                  Top = 57
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 7
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevel9: TSpinEditEx
                  Left = 220
                  Top = 57
                  Width = 52
                  Height = 21
                  MaxValue = 2147483647
                  MinValue = 1
                  TabOrder = 8
                  Value = 10
                  OnChange = EditDogzNameChange
                end
                object sePlusDogzLevelAfter9: TSpinEditEx
                  Left = 220
                  Top = 80
                  Width = 52
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 9
                  Value = 10
                  OnChange = EditBoneFammNameChange
                end
              end
              object chkDogzPlugSettingPriority: TCheckBox
                Left = 5
                Top = 109
                Width = 140
                Height = 17
                Hint = #24403#20154#29289#31561#32423#28385#36275#39640#32423#35774#32622#65292#32780#25216#33021#21448#36827#34892#20102#24378#21270#26102#65292#21246#36873#20123#36873#39033#65292#23453#23453#21517#21450#31561#32423#65292#20197#24378#21270#20026#20934
                Caption = #24378#21270#37197#32622#20248#20808#39640#32423#35774#32622
                TabOrder = 5
                OnClick = chkDogzPlugSettingPriorityClick
              end
            end
            object TabSheet24: TTabSheet
              Caption = #26032#35781#21650#26415
              ImageIndex = 3
              object grp59: TGroupBox
                Left = 8
                Top = 8
                Width = 175
                Height = 113
                Caption = #25216#33021#35774#32622
                TabOrder = 0
                object lbl112: TLabel
                  Left = 8
                  Top = 23
                  Width = 84
                  Height = 12
                  Caption = #27604#20363#38477#20302#22522#25968#65306
                end
                object Label736: TLabel
                  Left = 8
                  Top = 46
                  Width = 84
                  Height = 12
                  Caption = #35774#32622#26102#38388#20493#25968#65306
                end
                object Label737: TLabel
                  Left = 156
                  Top = 46
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label740: TLabel
                  Left = 8
                  Top = 69
                  Width = 84
                  Height = 12
                  Caption = #25216#33021#20351#29992#38388#38548#65306
                end
                object Label741: TLabel
                  Left = 153
                  Top = 69
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object lbl113: TLabel
                  Left = 8
                  Top = 92
                  Width = 150
                  Height = 12
                  Caption = #20197#19978#35774#32622#20165#23545#25216#33021'ID=46'#26377#25928
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clRed
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                end
                object seSkill46PowerBase: TSpinEditEx
                  Left = 88
                  Top = 18
                  Width = 78
                  Height = 21
                  Hint = #35774#32622#30446#26631#25915#20987#12289#39764#27861#12289#36947#26415#27604#20363#38477#20302#22522#25968#13#10#22914#35774#32622#20026'3'#65292#21017'0'#32423#25216#33021#26102#65292#38477#20302#30446#26631'3%'#65292'1'#32423#25216#33021#38477#20302#30446#26631'6%'#65292'2'#32423#25216#33021#38477#20302#30446#26631'9%'
                  MaxValue = 30
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seSkill46PowerBaseChange
                end
                object seSkill46SecRate: TSpinEditEx
                  Left = 88
                  Top = 41
                  Width = 60
                  Height = 21
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkill46SecRateChange
                end
                object seSkill46Time: TSpinEditEx
                  Left = 88
                  Top = 64
                  Width = 60
                  Height = 21
                  Hint = #26032#35781#21650#26415#20351#29992#38388#38548#65292#25968#20540#36234#22823#38388#38548#36234#38271
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seSkill46TimeChange
                end
              end
            end
            object TabSheet4: TTabSheet
              Caption = #22124#34880#26415
              ImageIndex = 4
              object GroupBox96: TGroupBox
                Left = 8
                Top = 8
                Width = 140
                Height = 46
                Caption = #21560#34880#30334#20998#30334
                TabOrder = 0
                object Label208: TLabel
                  Left = 8
                  Top = 21
                  Width = 54
                  Height = 12
                  Caption = #21560#34880#27604#20363':'
                end
                object Label209: TLabel
                  Left = 124
                  Top = 23
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object EditSkill57AddHPRate: TSpinEditEx
                  Left = 68
                  Top = 18
                  Width = 53
                  Height = 21
                  Hint = #24618#29289#25481#30340#34880#65292#33258#24049#21487#20197#21560#25910#22810#23569#13'100%'#34920#31034#21487#20197#20840#37096
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EditSkill57AddHPRateChange
                end
              end
              object GroupBox185: TGroupBox
                Left = 8
                Top = 58
                Width = 140
                Height = 67
                Caption = #25915#20987#35774#32622
                TabOrder = 1
                object Label579: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #25915#20987#20493#25968':'
                end
                object Label580: TLabel
                  Left = 124
                  Top = 20
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label146: TLabel
                  Left = 8
                  Top = 45
                  Width = 54
                  Height = 12
                  Caption = #20351#29992#38388#38548':'
                end
                object Label707: TLabel
                  Left = 122
                  Top = 43
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditSkill57PowerRate: TSpinEditEx
                  Left = 68
                  Top = 15
                  Width = 53
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = EditSkill57PowerRateChange
                end
                object seSkill57Time: TSpinEditEx
                  Left = 68
                  Top = 40
                  Width = 53
                  Height = 21
                  Hint = #20351#29992#38388#38548#65292#25968#20540#36234#22823#38388#38548#36234#38271
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seSkill57TimeChange
                end
              end
            end
            object TabSheet30: TTabSheet
              Caption = #21484#21796#26376#28789
              ImageIndex = 5
              object GroupBox97: TGroupBox
                Left = 4
                Top = 2
                Width = 140
                Height = 148
                Caption = #22522#26412#35774#32622
                TabOrder = 0
                object Label210: TLabel
                  Left = 8
                  Top = 18
                  Width = 54
                  Height = 12
                  Caption = #24618#29289#21517#31216':'
                end
                object Label211: TLabel
                  Left = 8
                  Top = 58
                  Width = 60
                  Height = 12
                  Caption = #21484#21796#25968#37327#65306
                end
                object lbl29: TLabel
                  Left = 8
                  Top = 82
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#36317#31163#65306
                end
                object Label665: TLabel
                  Left = 8
                  Top = 107
                  Width = 60
                  Height = 12
                  Caption = #21484#21796#38388#38548#65306
                end
                object Label666: TLabel
                  Left = 117
                  Top = 107
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object edtMonthSpiritName: TEdit
                  Left = 9
                  Top = 32
                  Width = 105
                  Height = 20
                  Hint = #35774#32622#40664#35748#21484#21796#30340#24618#29289#21517#31216#12290
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = EditBoneFammNameChange
                end
                object seMonthSpiritCount: TSpinEditEx
                  Left = 70
                  Top = 55
                  Width = 44
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26368#22823#25968#37327#12290
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = seMonthSpiritCountChange
                end
                object seMonthSpiritAttackRange: TSpinEditEx
                  Left = 70
                  Top = 79
                  Width = 44
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290#40664#35748'4'#65292#24314#35758#25915#20987#33539#22260#25511#21046#22312'6'#20197#20869
                  MaxValue = 12
                  MinValue = 4
                  TabOrder = 2
                  Value = 4
                  OnChange = seMonthSpiritAttackRangeChange
                end
                object seRecallMonthSpiritWaitTime: TSpinEditEx
                  Left = 70
                  Top = 103
                  Width = 44
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26102#38388#38388#38548#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 10
                  OnChange = seRecallMonthSpiritWaitTimeChange
                end
              end
              object GroupBox98: TGroupBox
                Left = 152
                Top = 2
                Width = 281
                Height = 148
                Caption = #39640#32423#35774#32622
                TabOrder = 1
                object GridMonthSpirit: TStringGrid
                  Left = 8
                  Top = 16
                  Width = 265
                  Height = 123
                  ColCount = 4
                  DefaultRowHeight = 18
                  FixedCols = 0
                  RowCount = 11
                  Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goEditing]
                  TabOrder = 0
                  OnSetEditText = GridBoneFammSetEditText
                  ColWidths = (
                    55
                    76
                    57
                    52)
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
                    18)
                end
              end
              object GroupBox99: TGroupBox
                Left = 4
                Top = 157
                Width = 141
                Height = 69
                Caption = #37325#20987#35774#32622
                TabOrder = 2
                object Label212: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #37325#20987#26426#29575':'
                end
                object Label213: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label214: TLabel
                  Left = 114
                  Top = 44
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object EditMonthSpiritHighAttackRate: TSpinEditEx
                  Left = 64
                  Top = 16
                  Width = 50
                  Height = 21
                  Hint = #37325#20987#26426#29575#65292#25968#23383#36234#23567#65292#26426#29575#36234#39640
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 0
                  Value = 10
                  OnChange = EditMonthSpiritHighAttackRateChange
                end
                object EditMonthSpiritHighPowerRate: TSpinEditEx
                  Left = 64
                  Top = 39
                  Width = 50
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = EditMonthSpiritHighPowerRateChange
                end
              end
              object CheckBoxMonthSpiritGotoMaster: TCheckBox
                Left = 155
                Top = 207
                Width = 137
                Height = 17
                Caption = #37325#22797#21484#21796#22238#21040#20154#29289#36523#36793
                TabOrder = 3
                OnClick = CheckBoxMonthSpiritGotoMasterClick
              end
              object CheckBoxMonthSpiritUseMasterMP: TCheckBox
                Left = 155
                Top = 166
                Width = 97
                Height = 17
                Caption = #19982#20027#20154#20849#29992#34013
                Enabled = False
                TabOrder = 4
                OnClick = CheckBoxMonthSpiritUseMasterMPClick
              end
              object CheckBoxMonthSpiritAttackSame: TCheckBox
                Left = 155
                Top = 187
                Width = 129
                Height = 17
                Caption = #19982#20027#20154#25915#20987#21516#19968#30446#26631
                TabOrder = 5
                OnClick = CheckBoxMonthSpiritAttackSameClick
              end
            end
            object TabSheet12: TTabSheet
              Caption = #21484#21796#22307#20861
              ImageIndex = 6
              object GroupBox100: TGroupBox
                Left = 5
                Top = 2
                Width = 140
                Height = 135
                Caption = #22522#26412#35774#32622
                TabOrder = 0
                object Label215: TLabel
                  Left = 8
                  Top = 18
                  Width = 54
                  Height = 12
                  Caption = #24618#29289#21517#31216':'
                end
                object Label216: TLabel
                  Left = 8
                  Top = 58
                  Width = 54
                  Height = 12
                  Caption = #21484#21796#25968#37327':'
                end
                object Label217: TLabel
                  Left = 8
                  Top = 90
                  Width = 54
                  Height = 12
                  Caption = #21484#21796#38388#38548':'
                end
                object Label218: TLabel
                  Left = 114
                  Top = 90
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditBigDogzName: TEdit
                  Left = 8
                  Top = 32
                  Width = 105
                  Height = 20
                  Hint = #35774#32622#40664#35748#21484#21796#30340#24618#29289#21517#31216#12290
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = EditDogzNameChange
                end
                object EditBigDogzCount: TSpinEditEx
                  Left = 60
                  Top = 55
                  Width = 53
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26368#22823#25968#37327#12290
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 1
                  Value = 10
                  OnChange = EditBigDogzCountChange
                end
                object EditRecallBigDogWaitTime: TSpinEditEx
                  Left = 60
                  Top = 87
                  Width = 53
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26102#38388#38388#38548#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = EditRecallBigDogWaitTimeChange
                end
              end
              object GroupBox101: TGroupBox
                Left = 152
                Top = 2
                Width = 281
                Height = 135
                Caption = #39640#32423#35774#32622
                TabOrder = 1
                object GridBigDogz: TStringGrid
                  Left = 8
                  Top = 16
                  Width = 265
                  Height = 113
                  ColCount = 4
                  DefaultRowHeight = 18
                  FixedCols = 0
                  RowCount = 11
                  Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goColSizing, goEditing]
                  TabOrder = 0
                  OnSetEditText = GridBoneFammSetEditText
                  ColWidths = (
                    55
                    76
                    57
                    52)
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
                    18)
                end
              end
              object CheckBoxBigDogzGotoMaster: TCheckBox
                Left = 6
                Top = 144
                Width = 137
                Height = 17
                Caption = #37325#22797#21484#21796#22238#21040#20154#29289#36523#36793
                TabOrder = 2
                OnClick = CheckBoxBigDogzGotoMasterClick
              end
            end
            object TabSheet5: TTabSheet
              Caption = #39123#39118#30772
              ImageIndex = 7
              object GroupBox121: TGroupBox
                Left = 8
                Top = 8
                Width = 129
                Height = 41
                Caption = #25915#20987#21147#20493#25968
                TabOrder = 0
                object Label245: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #20493#25968':'
                end
                object Label246: TLabel
                  Left = 96
                  Top = 20
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object EditSkill52PowerRate: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 45
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = EditSkill52PowerRateChange
                end
              end
              object GroupBox123: TGroupBox
                Left = 8
                Top = 56
                Width = 129
                Height = 41
                Caption = #25915#20987#33539#22260
                TabOrder = 1
                object Label249: TLabel
                  Left = 8
                  Top = 20
                  Width = 30
                  Height = 12
                  Caption = #33539#22260':'
                end
                object EditSkill52AttackRange: TSpinEditEx
                  Left = 44
                  Top = 15
                  Width = 61
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = EditSkill52AttackRangeChange
                end
              end
            end
            object TabSheet92: TTabSheet
              Caption = #26080#26497#30495#27668
              ImageIndex = 9
              object GroupBox183: TGroupBox
                Left = 8
                Top = 3
                Width = 177
                Height = 70
                Caption = #26377#25928#26102#38388#25511#21046
                TabOrder = 0
                object Label574: TLabel
                  Left = 8
                  Top = 24
                  Width = 84
                  Height = 12
                  Caption = #26377#25928#26102#38388#22522#25968#65306
                end
                object Label575: TLabel
                  Left = 8
                  Top = 48
                  Width = 84
                  Height = 12
                  Caption = #20351#29992#38388#38548#25511#21046#65306
                end
                object Label576: TLabel
                  Left = 148
                  Top = 48
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object EditElectrodelessWaitTime: TSpinEditEx
                  Left = 92
                  Top = 44
                  Width = 49
                  Height = 21
                  Hint = #26080#26497#30495#27668#20351#29992#38388#38548#65292#25968#20540#36234#22823#38388#38548#36234#38271
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = EditElectrodelessWaitTimeChange
                end
                object EditElectrodelessBase: TSpinEditEx
                  Left = 92
                  Top = 20
                  Width = 49
                  Height = 21
                  Hint = #26080#26497#30495#27668#25345#32493#26102#38388#25511#21046#65292#25968#20540#36234#22823#26102#38388#36234#20037
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = EditElectrodelessBaseChange
                end
              end
              object GroupBox88: TGroupBox
                Left = 193
                Top = 2
                Width = 235
                Height = 232
                TabOrder = 1
                object Label619: TLabel
                  Left = 13
                  Top = 41
                  Width = 24
                  Height = 12
                  Caption = 'Lv.0'
                end
                object Label620: TLabel
                  Left = 13
                  Top = 65
                  Width = 24
                  Height = 12
                  Caption = 'Lv.1'
                end
                object Label621: TLabel
                  Left = 13
                  Top = 90
                  Width = 24
                  Height = 12
                  Caption = 'Lv.2'
                end
                object Label622: TLabel
                  Left = 13
                  Top = 114
                  Width = 24
                  Height = 12
                  Caption = 'Lv.3'
                end
                object Label935: TLabel
                  Left = 13
                  Top = 138
                  Width = 24
                  Height = 12
                  Caption = 'Lv.4'
                end
                object Label936: TLabel
                  Left = 13
                  Top = 162
                  Width = 30
                  Height = 12
                  Caption = #24378#21270':'
                end
                object Panel1: TPanel
                  Left = 9
                  Top = 12
                  Width = 32
                  Height = 19
                  Caption = #31561#32423
                  TabOrder = 0
                end
                object Panel3: TPanel
                  Left = 50
                  Top = 12
                  Width = 84
                  Height = 19
                  Caption = #25345#32493#26102#38388'('#31186')'
                  TabOrder = 1
                end
                object seElectrodelessTimeL0: TSpinEditEx
                  Left = 51
                  Top = 37
                  Width = 85
                  Height = 21
                  Hint = #27492#21442#25968#20026#25216#33021#30340#26368#38271#26377#25928#26102#38388
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seElectrodelessTimeL0Change
                end
                object seElectrodelessTimeL1: TSpinEditEx
                  Tag = 1
                  Left = 51
                  Top = 61
                  Width = 85
                  Height = 21
                  Hint = #27492#21442#25968#20026#25216#33021#30340#26368#38271#26377#25928#26102#38388
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seElectrodelessTimeL0Change
                end
                object seElectrodelessTimeL2: TSpinEditEx
                  Tag = 2
                  Left = 51
                  Top = 86
                  Width = 85
                  Height = 21
                  Hint = #27492#21442#25968#20026#25216#33021#30340#26368#38271#26377#25928#26102#38388
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seElectrodelessTimeL0Change
                end
                object seElectrodelessTimeL3: TSpinEditEx
                  Tag = 3
                  Left = 51
                  Top = 110
                  Width = 85
                  Height = 21
                  Hint = #27492#21442#25968#20026#25216#33021#30340#26368#38271#26377#25928#26102#38388
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 5
                  Value = 0
                  OnChange = seElectrodelessTimeL0Change
                end
                object chkElectrodelessTimeSet: TCheckBox
                  Left = 51
                  Top = 207
                  Width = 73
                  Height = 17
                  Hint = #21246#36873#21518#65292#20197#35774#32622#26102#38388#20026#25216#33021#25345#32493#26102#38388#13#10#19981#21246#36873#65292#20026#25216#33021#25345#32493#26102#38388#19978#38480#25511#21046
                  Caption = #26102#38388#25511#21046
                  TabOrder = 6
                  OnClick = chkElectrodelessTimeSetClick
                end
                object Panel2: TPanel
                  Left = 142
                  Top = 12
                  Width = 83
                  Height = 19
                  Caption = #23041#21147#20493#25968'%'
                  TabOrder = 7
                end
                object seElectrodelessPowerRateL0: TSpinEditEx
                  Left = 142
                  Top = 37
                  Width = 86
                  Height = 21
                  Hint = #36947#26415#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 8
                  Value = 100
                  OnChange = seElectrodelessPowerRateL0Chnge
                end
                object seElectrodelessPowerRateL1: TSpinEditEx
                  Tag = 1
                  Left = 142
                  Top = 61
                  Width = 86
                  Height = 21
                  Hint = #36947#26415#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 9
                  Value = 100
                  OnChange = seElectrodelessPowerRateL0Chnge
                end
                object seElectrodelessPowerRateL2: TSpinEditEx
                  Tag = 2
                  Left = 142
                  Top = 86
                  Width = 86
                  Height = 21
                  Hint = #36947#26415#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 10
                  Value = 100
                  OnChange = seElectrodelessPowerRateL0Chnge
                end
                object seElectrodelessPowerRateL3: TSpinEditEx
                  Tag = 3
                  Left = 142
                  Top = 110
                  Width = 86
                  Height = 21
                  Hint = #36947#26415#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 11
                  Value = 100
                  OnChange = seElectrodelessPowerRateL0Chnge
                end
                object chkElectrodelessUseMaxSC: TCheckBox
                  Left = 131
                  Top = 207
                  Width = 94
                  Height = 17
                  Hint = #21246#36873#25216#33021#23041#21147#25353#26368#39640#36947#26415'*'#23041#21147#20493#25968#65292#21542#21017#22312#26368#20302#36947#26415#33267#26368#39640#36947#26415#38388#21462#19968#20010#20540'*'#23041#21147#20493#25968
                  Caption = #25353#26368#39640#36947#26415#31639
                  TabOrder = 12
                  OnClick = chkElectrodelessUseMaxSCClick
                end
                object seElectrodelessTimeL4: TSpinEditEx
                  Tag = 4
                  Left = 51
                  Top = 134
                  Width = 85
                  Height = 21
                  Hint = #27492#21442#25968#20026#25216#33021#30340#26368#38271#26377#25928#26102#38388
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 13
                  Value = 0
                  OnChange = seElectrodelessTimeL0Change
                end
                object seElectrodelessPowerRateL4: TSpinEditEx
                  Tag = 4
                  Left = 141
                  Top = 134
                  Width = 86
                  Height = 21
                  Hint = #36947#26415#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 14
                  Value = 100
                  OnChange = seElectrodelessPowerRateL0Chnge
                end
                object cbbElectrodelessNewLevel: TComboBox
                  Left = 51
                  Top = 158
                  Width = 85
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 15
                  OnChange = cbbElectrodelessNewLevelChange
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
                object seElectrodelessTimeLNew: TSpinEditEx
                  Tag = 4
                  Left = 51
                  Top = 181
                  Width = 85
                  Height = 21
                  Hint = #27492#21442#25968#20026#25216#33021#30340#26368#38271#26377#25928#26102#38388
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 16
                  Value = 0
                  OnChange = seElectrodelessTimeLNewChange
                end
                object seElectrodelessPowerRateLNew: TSpinEditEx
                  Tag = 4
                  Left = 141
                  Top = 181
                  Width = 86
                  Height = 21
                  Hint = #36947#26415#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 17
                  Value = 100
                  OnChange = seElectrodelessPowerRateLNewChange
                end
              end
            end
            object TabSheet93: TTabSheet
              Caption = #27668#21151#27874
              ImageIndex = 10
              object GroupBox186: TGroupBox
                Left = 8
                Top = 8
                Width = 177
                Height = 57
                Caption = #25511#21046
                TabOrder = 0
                object CheckBoxQigongPushSameLevel: TCheckBox
                  Left = 8
                  Top = 24
                  Width = 153
                  Height = 17
                  Caption = #21487#20197#25512#21160#31561#32423#30456#21516#30340#35282#33394
                  TabOrder = 0
                  OnClick = CheckBoxQigongPushSameLevelClick
                end
              end
            end
            object ts7: TTabSheet
              Caption = #31070#22307#25112#30002#26415
              ImageIndex = 13
              object lbl47: TLabel
                Left = 8
                Top = 112
                Width = 156
                Height = 12
                Caption = #27492#39029#21442#25968#35774#32622#20110#24189#28789#30462#36890#29992
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = [fsBold]
                ParentFont = False
              end
              object grp16: TGroupBox
                Left = 8
                Top = 8
                Width = 153
                Height = 97
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object lbl46: TLabel
                  Left = 8
                  Top = 24
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object lbl48: TLabel
                  Left = 118
                  Top = 24
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object lbl49: TLabel
                  Left = 8
                  Top = 48
                  Width = 60
                  Height = 12
                  Caption = #26102#38388#20493#25968#65306
                end
                object lbl50: TLabel
                  Left = 118
                  Top = 48
                  Width = 24
                  Height = 12
                  Caption = '/100'
                end
                object seSkill15PowerRate: TSpinEditEx
                  Left = 72
                  Top = 20
                  Width = 43
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = seSkill15PowerRateChange
                end
                object seSkill15TimeRate: TSpinEditEx
                  Left = 72
                  Top = 44
                  Width = 43
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seSkill15TimeRateChange
                end
                object chkSkill15OfflineClear: TCheckBox
                  Left = 8
                  Top = 72
                  Width = 137
                  Height = 17
                  Hint = #21246#36873#21518#20154#29289#19979#32447#21518#23558#19981#20445#23384#25216#33021#22686#21152#30340#38450#24481#21644#39764
                  Caption = #19979#32447#19981#20445#23384#22686#21152#23646#24615
                  TabOrder = 2
                  OnClick = chkSkill15OfflineClearClick
                end
              end
            end
            object ts20: TTabSheet
              Caption = #31105#38178#26415
              ImageIndex = 14
              object GroupBox224: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 121
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label748: TLabel
                  Left = 8
                  Top = 24
                  Width = 84
                  Height = 12
                  Caption = #25216#33021#20351#29992#38388#38548#65306
                end
                object Label749: TLabel
                  Left = 134
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label747: TLabel
                  Left = 8
                  Top = 48
                  Width = 84
                  Height = 12
                  Caption = #27599#32423#22686#21152#26102#38388#65306
                end
                object Label750: TLabel
                  Left = 134
                  Top = 48
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label751: TLabel
                  Left = 8
                  Top = 72
                  Width = 84
                  Height = 12
                  Caption = #27599#32423#22686#21152#33539#22260#65306
                end
                object Label752: TLabel
                  Left = 134
                  Top = 72
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object seSkill69CD: TSpinEditEx
                  Left = 88
                  Top = 20
                  Width = 43
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = seSkill69CDChange
                end
                object seSkill69AddTime: TSpinEditEx
                  Left = 88
                  Top = 44
                  Width = 43
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seSkill69AddTimeChange
                end
                object seSkill69AddRange: TSpinEditEx
                  Left = 88
                  Top = 68
                  Width = 43
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seSkill69AddRangeChange
                end
                object chkSkill69SameLevel: TCheckBox
                  Left = 7
                  Top = 93
                  Width = 153
                  Height = 17
                  Caption = #21487#20197#31105#38178#31561#32423#30456#21516#30340#35282#33394
                  TabOrder = 3
                  OnClick = chkSkill69SameLevelClick
                end
              end
            end
            object TabSheet55: TTabSheet
              Caption = #24515#28789#21484#21796
              ImageIndex = 15
              object GroupBox184: TGroupBox
                Left = 8
                Top = 8
                Width = 169
                Height = 49
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label577: TLabel
                  Left = 8
                  Top = 24
                  Width = 84
                  Height = 12
                  Caption = #25216#33021#20351#29992#38388#38548#65306
                end
                object Label578: TLabel
                  Left = 134
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seSkill70CD: TSpinEditEx
                  Left = 88
                  Top = 20
                  Width = 43
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = seSkill70CDChange
                end
              end
            end
          end
        end
        object TabSheet13: TTabSheet
          Caption = #21512#20987#25216#33021
          ImageIndex = 4
          object GroupBox75: TGroupBox
            Left = 4
            Top = 84
            Width = 156
            Height = 131
            Caption = #24594#27668#27133
            TabOrder = 5
            object Label178: TLabel
              Left = 8
              Top = 16
              Width = 84
              Height = 12
              Caption = #24594#27668#27133#26368#22823#20540#65306
            end
            object Label179: TLabel
              Left = 8
              Top = 40
              Width = 84
              Height = 12
              Caption = #24594#27668#27133#22686#21152#20540#65306
            end
            object Label180: TLabel
              Left = 8
              Top = 64
              Width = 84
              Height = 12
              Caption = #28779#40857#20043#24515#20943#23569#65306
            end
            object Label181: TLabel
              Left = 8
              Top = 88
              Width = 84
              Height = 12
              Caption = #24594#27668#22686#21152#38388#38548#65306
            end
            object EditMaxAngryValue: TSpinEditEx
              Left = 88
              Top = 12
              Width = 57
              Height = 21
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 1
              OnChange = EditMaxAngryValueChange
            end
            object EditAddAngryValue: TSpinEditEx
              Left = 88
              Top = 36
              Width = 57
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = EditAddAngryValueChange
            end
            object EditDecFirDragonPoint: TSpinEditEx
              Left = 88
              Top = 60
              Width = 57
              Height = 21
              MaxValue = 10000
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = EditDecFirDragonPointChange
            end
            object EditAddAngryValueTime: TSpinEditEx
              Left = 88
              Top = 84
              Width = 57
              Height = 21
              Increment = 100
              MaxValue = 10000
              MinValue = 500
              TabOrder = 3
              Value = 500
              OnChange = EditAddAngryValueTimeChange
            end
            object chkNoNeedFirDragon: TCheckBox
              Left = 9
              Top = 109
              Width = 144
              Height = 17
              Caption = #21512#20987#24594#27668#26080#38656#28779#40857#20043#24515
              TabOrder = 4
              OnClick = chkNoNeedFirDragonClick
            end
          end
          object GroupBox69: TGroupBox
            Left = 166
            Top = 1
            Width = 130
            Height = 109
            Caption = #30772#39746#26025'['#25112'-'#25112']'
            TabOrder = 1
            object Label165: TLabel
              Left = 10
              Top = 19
              Width = 60
              Height = 12
              Caption = #24618#29289#20260#23475#65306
            end
            object Label629: TLabel
              Left = 10
              Top = 67
              Width = 60
              Height = 12
              Caption = #25915#20987#33539#22260#65306
            end
            object Label794: TLabel
              Left = 10
              Top = 43
              Width = 60
              Height = 12
              Caption = #20154#29289#20260#23475#65306
            end
            object seSkill60PowerRate: TSpinEditEx
              Left = 66
              Top = 15
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seSkill60PowerRateChange
            end
            object seSkill60PowerRange: TSpinEditEx
              Left = 66
              Top = 63
              Width = 55
              Height = 21
              Hint = #39764#27861#25915#20987#33539#22260#21322#24452'('#28857')'#65292#40664#35748#20026'4'#13#10#27492#21442#25968#20063#25511#21046#27492#25216#33021#30340#37322#25918#33539#22260#65292#19982#30446#26631#36229#20986#33539#22260#23558#37322#25918#22833#36133
              MaxValue = 10
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = seSkill60PowerRangeChange
            end
            object seSkill60AttackHumPowerRate: TSpinEditEx
              Left = 66
              Top = 39
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 2
              Value = 100
              OnChange = seSkill60AttackHumPowerRateChange
            end
            object chkSkill60NotMagBubbleDefence: TCheckBox
              Left = 10
              Top = 86
              Width = 110
              Height = 17
              Caption = #26080#35270#39764#27861#30462#38450#24481
              TabOrder = 3
              OnClick = chkSkill60NotMagBubbleDefenceClick
            end
          end
          object GroupBox70: TGroupBox
            Left = 166
            Top = 207
            Width = 130
            Height = 86
            Caption = #21128#26143#26025'['#25112'-'#36947']'
            TabOrder = 6
            object Label168: TLabel
              Left = 10
              Top = 20
              Width = 60
              Height = 12
              Caption = #24618#29289#20260#23475#65306
            end
            object Label796: TLabel
              Left = 10
              Top = 43
              Width = 60
              Height = 12
              Caption = #20154#29289#20260#23475#65306
            end
            object seSkill61PowerRate: TSpinEditEx
              Left = 66
              Top = 15
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seSkill61PowerRateChange
            end
            object chkSkill61NotMagBubbleDefence: TCheckBox
              Left = 10
              Top = 61
              Width = 110
              Height = 17
              Caption = #26080#35270#39764#27861#30462#38450#24481
              TabOrder = 1
              OnClick = chkSkill61NotMagBubbleDefenceClick
            end
            object seSkill61AttackHumPowerRate: TSpinEditEx
              Left = 66
              Top = 39
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 2
              Value = 100
              OnChange = seSkill61AttackHumPowerRateChange
            end
          end
          object GroupBox71: TGroupBox
            Left = 166
            Top = 116
            Width = 130
            Height = 83
            Caption = #38647#38662#19968#20987'['#25112'-'#27861']'
            TabOrder = 3
            object Label170: TLabel
              Left = 10
              Top = 20
              Width = 60
              Height = 12
              Caption = #24618#29289#20260#23475#65306
            end
            object Label798: TLabel
              Left = 10
              Top = 43
              Width = 60
              Height = 12
              Caption = #20154#29289#20260#23475#65306
            end
            object seSkill62PowerRate: TSpinEditEx
              Left = 66
              Top = 15
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seSkill62PowerRateChange
            end
            object chkSkill62NotMagBubbleDefence: TCheckBox
              Left = 10
              Top = 62
              Width = 110
              Height = 17
              Caption = #26080#35270#39764#27861#30462#38450#24481
              TabOrder = 1
              OnClick = chkSkill62NotMagBubbleDefenceClick
            end
            object seSkill62AttackHumPowerRate: TSpinEditEx
              Left = 66
              Top = 39
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 2
              Value = 100
              OnChange = seSkill62AttackHumPowerRateChange
            end
          end
          object GroupBox72: TGroupBox
            Left = 306
            Top = 181
            Width = 130
            Height = 114
            Caption = #22124#39746#27836#27901'['#36947'-'#36947']'
            TabOrder = 4
            object Label172: TLabel
              Left = 10
              Top = 20
              Width = 60
              Height = 12
              Caption = #24618#29289#20260#23475#65306
            end
            object Label630: TLabel
              Left = 10
              Top = 64
              Width = 60
              Height = 12
              Caption = #25915#20987#33539#22260#65306
            end
            object Label804: TLabel
              Left = 10
              Top = 42
              Width = 60
              Height = 12
              Caption = #20154#29289#20260#23475#65306
            end
            object seSkill63PowerRate: TSpinEditEx
              Left = 66
              Top = 15
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seSkill63PowerRateChange
            end
            object seSkill63PowerRange: TSpinEditEx
              Left = 66
              Top = 59
              Width = 55
              Height = 21
              Hint = #39764#27861#25915#20987#33539#22260#21322#24452'('#28857')'#65292#40664#35748#20026'3'
              MaxValue = 10
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = seSkill63PowerRangeChange
            end
            object chkSkill63GreenPoison: TCheckBox
              Left = 10
              Top = 81
              Width = 105
              Height = 17
              Hint = #36873#20013#27492#39033#21518#65292#30446#26631#34987#22124#39746#27836#27901#25915#20987#25104#21151#26102#65292#21516#26102#20013#32511#27602#65292#21542#21017#19981#20013#32511#27602
              Caption = #30446#26631#21516#26102#20013#32511#27602
              TabOrder = 2
              OnClick = chkSkill63GreenPoisonClick
            end
            object seSkill63AttackHumPowerRate: TSpinEditEx
              Left = 66
              Top = 37
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 3
              Value = 100
              OnChange = seSkill63AttackHumPowerRateChange
            end
            object chkSkill63UseSpeedPoint: TCheckBox
              Left = 10
              Top = 95
              Width = 105
              Height = 17
              Hint = #36873#20013#27492#39033#21518#65292#21463#25915#20987#30340#30446#26631#25935#25463#36234#39640#65292#36530#36991#25481#25915#20987#30340#20960#29575#36234#22823
              Caption = #35745#31639#30446#26631#25935#25463
              TabOrder = 4
              OnClick = chkSkill63UseSpeedPointClick
            end
          end
          object GroupBox73: TGroupBox
            Left = 306
            Top = 83
            Width = 130
            Height = 97
            Caption = #26411#26085#23457#21028'['#27861'-'#36947']'
            TabOrder = 2
            object Label174: TLabel
              Left = 10
              Top = 18
              Width = 60
              Height = 12
              Caption = #24618#29289#20260#23475#65306
            end
            object Label628: TLabel
              Left = 10
              Top = 62
              Width = 60
              Height = 12
              Caption = #25915#20987#33539#22260#65306
            end
            object Label802: TLabel
              Left = 10
              Top = 40
              Width = 60
              Height = 12
              Caption = #20154#29289#20260#23475#65306
            end
            object seSkill64PowerRate: TSpinEditEx
              Left = 66
              Top = 13
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seSkill64PowerRateChange
            end
            object seSkill64PowerRange: TSpinEditEx
              Left = 66
              Top = 57
              Width = 55
              Height = 21
              Hint = #39764#27861#25915#20987#33539#22260#21322#24452'('#28857')'#65292#40664#35748#20026'3'
              MaxValue = 10
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = seSkill64PowerRangeChange
            end
            object chkSkill64MakeStone: TCheckBox
              Left = 10
              Top = 78
              Width = 97
              Height = 17
              Caption = #30446#26631#21516#26102#40635#30201
              TabOrder = 2
              OnClick = chkSkill64MakeStoneClick
            end
            object seSkill64AttackHumPowerRate: TSpinEditEx
              Left = 66
              Top = 35
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 3
              Value = 100
              OnChange = seSkill64AttackHumPowerRateChange
            end
          end
          object GroupBox74: TGroupBox
            Left = 306
            Top = -1
            Width = 130
            Height = 83
            Caption = #28779#40857#27668#28976'['#27861'-'#27861']'
            TabOrder = 7
            object Label176: TLabel
              Left = 10
              Top = 20
              Width = 60
              Height = 12
              Caption = #24618#29289#20260#23475#65306
            end
            object Label627: TLabel
              Left = 10
              Top = 64
              Width = 60
              Height = 12
              Caption = #25915#20987#33539#22260#65306
            end
            object Label800: TLabel
              Left = 10
              Top = 42
              Width = 60
              Height = 12
              Caption = #20154#29289#20260#23475#65306
            end
            object seSkill65PowerRate: TSpinEditEx
              Left = 66
              Top = 15
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seSkill65PowerRateChange
            end
            object seSkill65PowerRange: TSpinEditEx
              Left = 66
              Top = 59
              Width = 55
              Height = 21
              Hint = #39764#27861#25915#20987#33539#22260#21322#24452'('#28857')'#65292#40664#35748#20026'3'
              MaxValue = 10
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = seSkill65PowerRangeChange
            end
            object seSkill65AttackHumPowerRate: TSpinEditEx
              Left = 66
              Top = 37
              Width = 55
              Height = 21
              Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 1000
              MinValue = 1
              TabOrder = 2
              Value = 100
              OnChange = seSkill65AttackHumPowerRateChange
            end
          end
          object grp34: TGroupBox
            Left = 4
            Top = 0
            Width = 157
            Height = 80
            Caption = #21512#20987#25511#21046
            TabOrder = 0
            object lbl104: TLabel
              Left = 13
              Top = 36
              Width = 84
              Height = 12
              Caption = #21512#20987#24594#27133#25511#21046#65306
            end
            object Label658: TLabel
              Left = 140
              Top = 37
              Width = 6
              Height = 12
              Caption = '%'
            end
            object chkHeroJointAttack: TCheckBox
              Left = 12
              Top = 15
              Width = 141
              Height = 17
              Hint = #25171#24320#20123#21151#33021#65292#23558#20801#35768#33521#38596#20351#29992#21512#20987#25216#33021
              Caption = #20801#35768#20351#29992#33521#38596#21512#20987
              TabOrder = 0
              OnClick = chkHeroJointAttackClick
            end
            object seAngryAgainValue: TSpinEditEx
              Left = 95
              Top = 32
              Width = 42
              Height = 21
              Hint = #27492#25511#21046#20026#24403#21512#20987#36317#31163#36229#20986#33539#22260#23548#33268#21512#20987#22833#36133#65292#22312#24594#31967#21097#20313#30334#20998#27604#24773#20917#19979#25509#36817#21512#20987#30446#26631#21487#35302#21457#21512#20987#65281
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 1
              OnChange = seAngryAgainValueChange
            end
            object chkHeroJointAttackFly: TCheckBox
              Left = 12
              Top = 57
              Width = 77
              Height = 17
              Hint = #21246#36873#21518#65292#20351#29992#21512#20987#25216#33021#26102#65292#33521#38596#19981#22312#36523#36793#21017#31435#21363#39134#21040#20027#20154#36523#36793#36827#34892#21512#20987
              Caption = #31354#38477#27169#24335
              TabOrder = 2
              OnClick = chkHeroJointAttackFlyClick
            end
          end
          object grp62: TGroupBox
            Left = 4
            Top = 220
            Width = 156
            Height = 42
            Caption = #21512#20987#25216#33021#20260#23475#25511#21046
            TabOrder = 8
            object lbl114: TLabel
              Left = 8
              Top = 19
              Width = 96
              Height = 12
              Caption = #27599#25552#21319'1'#32423#20260#23475'+'#65306
            end
            object Label743: TLabel
              Left = 144
              Top = 19
              Width = 6
              Height = 12
              Caption = '%'
            end
            object seSkillJointAttackLevelRate: TSpinEditEx
              Left = 101
              Top = 14
              Width = 41
              Height = 21
              MaxValue = 300
              MinValue = 1
              TabOrder = 0
              Value = 1
              OnChange = seSkillJointAttackLevelRateChange
            end
          end
        end
        object TabSheet11: TTabSheet
          Caption = #36890#29992#25216#33021
          ImageIndex = 5
          object PageControl2: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            ActivePage = TabSheet29
            Align = alClient
            TabOrder = 0
            object TabSheet29: TTabSheet
              Caption = #20998#36523#26415
              object GroupBox79: TGroupBox
                Left = 8
                Top = 2
                Width = 185
                Height = 132
                Caption = #22522#26412#35774#32622
                TabOrder = 0
                object Label157: TLabel
                  Left = 20
                  Top = 20
                  Width = 78
                  Height = 12
                  Caption = #20801#35768#20998#36523#25968#37327':'
                end
                object Label159: TLabel
                  Left = 20
                  Top = 44
                  Width = 78
                  Height = 12
                  Caption = #20998#36523#23384#27963#26102#38388':'
                end
                object Label160: TLabel
                  Left = 166
                  Top = 44
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label116: TLabel
                  Left = 8
                  Top = 68
                  Width = 90
                  Height = 12
                  Caption = #27599#32423#21152#23384#27963#26102#38388':'
                end
                object Label814: TLabel
                  Left = 166
                  Top = 68
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seCopySelfMaxCount: TSpinEditEx
                  Left = 99
                  Top = 16
                  Width = 64
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = seCopySelfMaxCountChange
                end
                object seCopySelfExistTime: TSpinEditEx
                  Left = 99
                  Top = 40
                  Width = 64
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seCopySelfExistTimeChange
                end
                object chkCopySelfNonUseSpellPoint: TCheckBox
                  Left = 8
                  Top = 89
                  Width = 73
                  Height = 17
                  Hint = #21246#19978#21518#65292#26080#38656#21507#33647#65292#21487#20197#20351#29992#39764#27861
                  Caption = #26080#38480#39764#27861
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 3
                  OnClick = chkCopySelfNonUseSpellPointClick
                end
                object chkAlwaysFollowMasterAttack: TCheckBox
                  Left = 8
                  Top = 109
                  Width = 161
                  Height = 17
                  Caption = #21644#20027#20154#25915#20987#21516#19968#30446#26631
                  TabOrder = 4
                  OnClick = chkAlwaysFollowMasterAttackClick
                end
                object seCopySelfLevelUpAddExistTime: TSpinEditEx
                  Left = 99
                  Top = 64
                  Width = 63
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#20998#36523#23384#27963#26102#38388#22686#21152'x'#31186
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seCopySelfLevelUpAddExistTimeChange
                end
              end
              object GroupBox203: TGroupBox
                Left = 8
                Top = 136
                Width = 185
                Height = 83
                Caption = #21517#31216#35774#32622
                TabOrder = 1
                object lbl94: TLabel
                  Left = 8
                  Top = 42
                  Width = 78
                  Height = 12
                  Caption = #20998#36523#21517#31216#21518#32512':'
                end
                object Label755: TLabel
                  Left = 32
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #21517#23383#39068#33394':'
                end
                object edtCopySelfSuffix: TEdit
                  Left = 87
                  Top = 38
                  Width = 75
                  Height = 20
                  ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = edtCopySelfSuffixChange
                end
                object chkShowCopySelfSuffix: TCheckBox
                  Left = 8
                  Top = 62
                  Width = 93
                  Height = 17
                  Caption = #26174#31034#20998#36523#21518#32512
                  TabOrder = 1
                  OnClick = chkShowCopySelfSuffixClick
                end
                object seCopySelfNameColor: TColorIndexEdit
                  Left = 87
                  Top = 16
                  Width = 72
                  Height = 21
                  Hint = #24403#20154#29289#25915#20987#20854#20182#20154#29289#26102#21517#23383#39068#33394#65292#40664#35748#20026'47'
                  MaxLength = 3
                  MaxValue = 255
                  MinValue = 0
                  TabOrder = 2
                  Value = 100
                  OnChange = seCopySelfNameColorChange
                  ShowNoneColor = False
                end
              end
              object GroupBox208: TGroupBox
                Left = 8
                Top = 221
                Width = 185
                Height = 49
                Caption = #21484#21796#25511#21046
                TabOrder = 2
                object Label659: TLabel
                  Left = 8
                  Top = 23
                  Width = 78
                  Height = 12
                  Caption = #21484#21796#26102#38388#38388#38548':'
                end
                object Label660: TLabel
                  Left = 166
                  Top = 23
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seRecallCopySelfWaitTime: TSpinEditEx
                  Left = 87
                  Top = 19
                  Width = 74
                  Height = 21
                  Hint = #35774#32622#21487#21484#21796#26102#38388#38388#38548#12290
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seRecallCopySelfWaitTimeChange
                end
              end
              object grp63: TGroupBox
                Left = 200
                Top = 2
                Width = 137
                Height = 47
                Caption = #33521#38596#21484#21796#20998#36523#34880#37327#27604#20363
                TabOrder = 3
                object lbl117: TLabel
                  Left = 8
                  Top = 22
                  Width = 72
                  Height = 12
                  Caption = #34880#37327#27604#20363#20540#65306
                end
                object lbl118: TLabel
                  Left = 122
                  Top = 22
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seHeroRecallCopySelfHPRate: TSpinEditEx
                  Left = 78
                  Top = 19
                  Width = 42
                  Height = 21
                  Hint = #33521#38596#20998#36523#25216#33021#20026#34987#21160#25216#33021#65292#24403'HP'#20302#20110#35774#32622#20540#21518#25165#20250#20351#29992#65292#40664#35748'100%'
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 100
                  OnChange = seHeroRecallCopySelfHPRateChange
                end
              end
              object grp77: TGroupBox
                Left = 200
                Top = 52
                Width = 137
                Height = 57
                Caption = #36947#22763#20998#36523
                TabOrder = 4
                object chkCopyMonWarrorAttack: TCheckBox
                  Left = 4
                  Top = 15
                  Width = 128
                  Height = 17
                  Hint = #27809#26377#39764#27861#25110#20351#29992#39764#27861#22833#36133#26102#65292#26159#21542#20351#29992#29289#29702#25915#20987
                  Caption = #36947#22763#26080#39764#27861#29289#29702#25915#20987
                  TabOrder = 0
                  OnClick = chkCopyMonWarrorAttackClick
                end
                object chkCopyMon700HPUseBaseAttack: TCheckBox
                  Left = 4
                  Top = 33
                  Width = 128
                  Height = 16
                  Hint = #36947#22763','#24403#30446#26631'HP'#23569#20110'700'#26102','#21487#20197#20351#29992#29289#29702#25915#20987
                  Caption = '700MaxHP'#19979#29289#29702#25915#20987
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 1
                  OnClick = chkCopyMon700HPUseBaseAttackClick
                end
              end
              object chkCopyMonInheritedMasterSpeed: TCheckBox
                Left = 200
                Top = 112
                Width = 121
                Height = 17
                Hint = #21246#36873#21518#65292#20998#36523#25915#20987#36895#24230'/'#31227#21160#36895#24230'/'#39764#27861#36895#24230#21644#20027#20154#20445#25252#19968#33268#65292#21542#21017#21644#33521#38596#20849#29992#36895#24230#37197#32622
                Caption = #20998#36523#32487#25215#20027#20154#36895#24230
                TabOrder = 5
                OnClick = chkCopyMonInheritedMasterSpeedClick
              end
            end
            object TabSheet36: TTabSheet
              Caption = #20094#22372#22823#25386#31227
              ImageIndex = 1
              object GroupBox87: TGroupBox
                Left = 8
                Top = 8
                Width = 177
                Height = 129
                Caption = #25216#33021#35774#32622
                TabOrder = 0
                object Label193: TLabel
                  Left = 8
                  Top = 24
                  Width = 84
                  Height = 12
                  Caption = #25216#33021#20919#21364#26102#38388#65306
                end
                object Label194: TLabel
                  Left = 154
                  Top = 24
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object lbl96: TLabel
                  Left = 8
                  Top = 48
                  Width = 84
                  Height = 12
                  Caption = #20351#29992#25104#21151#20960#29575#65306
                end
                object Label815: TLabel
                  Left = 8
                  Top = 74
                  Width = 84
                  Height = 12
                  Caption = #27599#32423#21152#25104#21151#29575#65306
                end
                object Label825: TLabel
                  Left = 157
                  Top = 72
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill72HitWaitTime: TSpinEditEx
                  Left = 88
                  Top = 20
                  Width = 62
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill72HitWaitTimeChange
                end
                object seSkill72Rate: TSpinEditEx
                  Left = 88
                  Top = 44
                  Width = 62
                  Height = 21
                  Hint = #25968#20540#36234#23567#25104#21151#36234#39640
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seSkill72RateChange
                end
                object seSkill72LevelUpRateAdd: TSpinEditEx
                  Left = 88
                  Top = 69
                  Width = 62
                  Height = 21
                  Hint = #27599#21319#19968#32423#65292#25104#21151#20960#29575#22686#21152'%'
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seSkill72LevelUpRateAddChange
                end
                object chkSkill72DisableStopItem: TCheckBox
                  Left = 74
                  Top = 96
                  Width = 87
                  Height = 17
                  Hint = #21246#36873#21518#31105#27490#20351#29992#25216#33021#30452#25509#39134#21040#29289#21697#19978#38754
                  Caption = #31105#27490#39134#35013#22791
                  TabOrder = 3
                  OnClick = chkSkill72DisableStopItemClick
                end
              end
            end
            object TabSheet43: TTabSheet
              Caption = #25252#20307#31070#30462
              ImageIndex = 3
              object GroupBox89: TGroupBox
                Left = 8
                Top = 8
                Width = 187
                Height = 89
                Caption = #26102#38388#25511#21046
                TabOrder = 0
                object Label197: TLabel
                  Left = 26
                  Top = 41
                  Width = 78
                  Height = 12
                  Caption = #25216#33021#26377#25928#26102#38388':'
                end
                object Label198: TLabel
                  Left = 166
                  Top = 41
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label203: TLabel
                  Left = 26
                  Top = 17
                  Width = 78
                  Height = 12
                  Caption = #25216#33021#20351#29992#38388#38548':'
                end
                object Label204: TLabel
                  Left = 166
                  Top = 17
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label816: TLabel
                  Left = 14
                  Top = 65
                  Width = 90
                  Height = 12
                  Caption = #27599#32423#21152#26377#25928#26102#38388':'
                end
                object Label817: TLabel
                  Left = 166
                  Top = 65
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object seSuperShiledValidTime: TSpinEditEx
                  Left = 106
                  Top = 37
                  Width = 58
                  Height = 21
                  Hint = #24320#30462#21518#65292#24320#30462#29366#24577#25345#32493#26102#38388#65288#25252#20307#20943#25915#20987#65289
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSuperShiledValidTimeChange
                end
                object seLastSuperShiledTime: TSpinEditEx
                  Left = 106
                  Top = 13
                  Width = 58
                  Height = 21
                  Hint = #20987#30772'('#28040#22833')'#21518#24674#22797#38388#38548'XX'#31186#65292#20174#30462#20987#30772#25110#28040#22833#26102#24320#22987#35745#31639#19979#27425#20351#29992#38388#38548
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seLastSuperShiledTimeChange
                end
                object seSuperShiledLevelUpAddValidTime: TSpinEditEx
                  Left = 106
                  Top = 61
                  Width = 58
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#26377#25928#26102#38388#22686#21152'x'#31186
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seSuperShiledLevelUpAddValidTimeChange
                end
              end
              object GroupBox90: TGroupBox
                Left = 8
                Top = 99
                Width = 187
                Height = 164
                Caption = #20960#29575'/'#27604#20363#35774#32622
                TabOrder = 2
                object Label199: TLabel
                  Left = 14
                  Top = 19
                  Width = 90
                  Height = 12
                  Caption = #25252#20307#20943#25915#20987#27604#20363':'
                end
                object Label200: TLabel
                  Left = 169
                  Top = 19
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label818: TLabel
                  Left = 14
                  Top = 44
                  Width = 90
                  Height = 12
                  Caption = #27599#32423#20943#25915#20987#27604#20363':'
                end
                object Label819: TLabel
                  Left = 169
                  Top = 44
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label201: TLabel
                  Left = 26
                  Top = 67
                  Width = 78
                  Height = 12
                  Caption = #30462#34987#20987#30772#20960#29575':'
                end
                object Label821: TLabel
                  Left = 14
                  Top = 92
                  Width = 90
                  Height = 12
                  Caption = #27599#32423#20943#30462#30772#20960#29575':'
                end
                object Label202: TLabel
                  Left = 26
                  Top = 115
                  Width = 78
                  Height = 12
                  Caption = #25216#33021#29983#25928#20960#29575':'
                end
                object Label822: TLabel
                  Left = 14
                  Top = 139
                  Width = 90
                  Height = 12
                  Caption = #27599#32423#21152#29983#25928#20960#29575':'
                end
                object Label826: TLabel
                  Left = 169
                  Top = 92
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label820: TLabel
                  Left = 169
                  Top = 139
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSuperShiledPowerRate: TSpinEditEx
                  Left = 106
                  Top = 15
                  Width = 58
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSuperShiledPowerRateChange
                end
                object seSuperShiledLevelUpDecPowerRate: TSpinEditEx
                  Left = 106
                  Top = 39
                  Width = 58
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#20943#25915#20987#27604#20363'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seSuperShiledLevelUpDecPowerRateChange
                end
                object seCloseSuperShiledRate: TSpinEditEx
                  Left = 106
                  Top = 63
                  Width = 57
                  Height = 21
                  Hint = #25968#23383#36234#23567#65292#26426#29575#36234#39640
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seCloseSuperShiledRateChange
                end
                object seCloseSuperShiledLevelUpDecRate: TSpinEditEx
                  Left = 106
                  Top = 87
                  Width = 58
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#20943#30462#30772#20960#29575#27604#20363'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 10
                  OnChange = seCloseSuperShiledLevelUpDecRateChange
                end
                object seOpenSuperShiledRate: TSpinEditEx
                  Left = 106
                  Top = 111
                  Width = 57
                  Height = 21
                  Hint = #25968#23383#36234#23567#65292#26426#29575#36234#39640
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 4
                  Value = 10
                  OnChange = seOpenSuperShiledRateChange
                end
                object seOpenSuperShiledLevelUpAddRate: TSpinEditEx
                  Left = 106
                  Top = 135
                  Width = 57
                  Height = 21
                  Hint = #25968#23383#36234#23567#65292#26426#29575#36234#39640
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 5
                  Value = 10
                  OnChange = seOpenSuperShiledLevelUpAddRateChange
                end
              end
              object GroupBoxCloseSuperShiled: TGroupBox
                Left = 201
                Top = 8
                Width = 230
                Height = 133
                Caption = #25915#20987#30772#25252#20307#31070#30462
                TabOrder = 1
                object lbl144: TLabel
                  Left = 138
                  Top = 19
                  Width = 36
                  Height = 12
                  Caption = '% '#36882#22686
                end
                object Label757: TLabel
                  Left = 138
                  Top = 42
                  Width = 36
                  Height = 12
                  Caption = '% '#36882#22686
                end
                object Label758: TLabel
                  Left = 138
                  Top = 65
                  Width = 36
                  Height = 12
                  Caption = '% '#36882#22686
                end
                object Label759: TLabel
                  Left = 138
                  Top = 87
                  Width = 36
                  Height = 12
                  Caption = '% '#36882#22686
                end
                object Label760: TLabel
                  Left = 138
                  Top = 110
                  Width = 36
                  Height = 12
                  Caption = '% '#36882#22686
                end
                object chkUseSkillCloseSuperShiled0: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 84
                  Height = 17
                  Caption = #21050#26432' '#20960#29575#65306
                  TabOrder = 0
                  OnClick = chkUseSkillCloseSuperShiled0Click
                end
                object chkUseSkillCloseSuperShiled1: TCheckBox
                  Tag = 1
                  Left = 8
                  Top = 39
                  Width = 84
                  Height = 17
                  Caption = #28872#28779' '#20960#29575#65306
                  TabOrder = 1
                  OnClick = chkUseSkillCloseSuperShiled0Click
                end
                object chkUseSkillCloseSuperShiled2: TCheckBox
                  Tag = 2
                  Left = 8
                  Top = 62
                  Width = 84
                  Height = 17
                  Caption = #24320#22825' '#20960#29575#65306
                  TabOrder = 2
                  OnClick = chkUseSkillCloseSuperShiled0Click
                end
                object chkUseSkillCloseSuperShiled3: TCheckBox
                  Tag = 3
                  Left = 8
                  Top = 85
                  Width = 84
                  Height = 17
                  Caption = #36880#26085' '#20960#29575#65306
                  TabOrder = 3
                  OnClick = chkUseSkillCloseSuperShiled0Click
                end
                object chkUseSkillCloseSuperShiled4: TCheckBox
                  Tag = 4
                  Left = 8
                  Top = 108
                  Width = 84
                  Height = 17
                  Caption = #21512#20987' '#20960#29575#65306
                  TabOrder = 4
                  OnClick = chkUseSkillCloseSuperShiled0Click
                end
                object seUseSkillCloseSuperShiled0_Rate: TSpinEditEx
                  Left = 88
                  Top = 14
                  Width = 47
                  Height = 21
                  Hint = #21050#26432#21073#26415#30772#25252#20307#30462#20960#29575#65288#30334#20998#27604#65289
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 5
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateChange
                end
                object seUseSkillCloseSuperShiled1_Rate: TSpinEditEx
                  Tag = 1
                  Left = 88
                  Top = 37
                  Width = 47
                  Height = 21
                  Hint = #28872#28779#21073#27861#30772#25252#20307#30462#20960#29575#65288#30334#20998#27604#65289
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 6
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateChange
                end
                object seUseSkillCloseSuperShiled2_Rate: TSpinEditEx
                  Tag = 2
                  Left = 88
                  Top = 60
                  Width = 47
                  Height = 21
                  Hint = #24320#22825#26025#30772#25252#20307#30462#20960#29575#65288#30334#20998#27604#65289
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 7
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateChange
                end
                object seUseSkillCloseSuperShiled3_Rate: TSpinEditEx
                  Tag = 3
                  Left = 88
                  Top = 83
                  Width = 47
                  Height = 21
                  Hint = #36880#26085#21073#27861#30772#25252#20307#30462#20960#29575#65288#30334#20998#27604#65289
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 8
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateChange
                end
                object seUseSkillCloseSuperShiled4_Rate: TSpinEditEx
                  Tag = 4
                  Left = 88
                  Top = 106
                  Width = 47
                  Height = 21
                  Hint = #21512#20987#30772#25252#20307#30462#20960#29575#65288#30334#20998#27604#65289
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 9
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateChange
                end
                object seUseSkillCloseSuperShiled0_RateAdd: TSpinEditEx
                  Left = 176
                  Top = 14
                  Width = 47
                  Height = 21
                  Hint = #25216#33021#27599#21319#19968#32423#22686#21152#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 10
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateAddChange
                end
                object seUseSkillCloseSuperShiled1_RateAdd: TSpinEditEx
                  Tag = 1
                  Left = 176
                  Top = 37
                  Width = 47
                  Height = 21
                  Hint = #25216#33021#27599#21319#19968#32423#22686#21152#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 11
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateAddChange
                end
                object seUseSkillCloseSuperShiled2_RateAdd: TSpinEditEx
                  Tag = 2
                  Left = 176
                  Top = 60
                  Width = 47
                  Height = 21
                  Hint = #25216#33021#27599#21319#19968#32423#22686#21152#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 12
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateAddChange
                end
                object seUseSkillCloseSuperShiled3_RateAdd: TSpinEditEx
                  Tag = 3
                  Left = 176
                  Top = 83
                  Width = 47
                  Height = 21
                  Hint = #25216#33021#27599#21319#19968#32423#22686#21152#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 13
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateAddChange
                end
                object seUseSkillCloseSuperShiled4_RateAdd: TSpinEditEx
                  Tag = 4
                  Left = 176
                  Top = 106
                  Width = 47
                  Height = 21
                  Hint = #25216#33021#27599#21319#19968#32423#22686#21152#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 14
                  Value = 10
                  OnChange = seUseSkillCloseSuperShiled0_RateAddChange
                end
              end
              object GroupBox93: TGroupBox
                Left = 201
                Top = 143
                Width = 230
                Height = 80
                Caption = #20854#20182#35774#32622
                TabOrder = 3
                object CheckBoxAutoOpenSuperShiled: TCheckBox
                  Left = 8
                  Top = 16
                  Width = 89
                  Height = 17
                  Caption = #33258#21160#24320#21551#31070#30462
                  TabOrder = 0
                  OnClick = CheckBoxAutoOpenSuperShiledClick
                end
                object chkShowSuperShiledEffect: TCheckBox
                  Left = 8
                  Top = 37
                  Width = 111
                  Height = 17
                  Caption = #24320#21551#26102#26174#31034#25928#26524
                  TabOrder = 1
                  OnClick = chkShowSuperShiledEffectClick
                end
                object chkCloseSuperShiledHint: TCheckBox
                  Left = 120
                  Top = 16
                  Width = 97
                  Height = 17
                  Caption = #20851#38381#25552#31034#20449#24687
                  TabOrder = 2
                  OnClick = chkCloseSuperShiledHintClick
                end
                object chkShowSuperShiledSound: TCheckBox
                  Left = 120
                  Top = 37
                  Width = 105
                  Height = 17
                  Caption = #24320#21551#26102#25773#25918#22768#38899
                  TabOrder = 3
                  OnClick = chkShowSuperShiledSoundClick
                end
                object chkShowSuperShiledEffect2: TCheckBox
                  Left = 8
                  Top = 57
                  Width = 111
                  Height = 17
                  Hint = #34987#25915#20987#26102#65292#20854#20182#29609#23478#26159#21542#21487#20197#30475#21040#25928#26524#65292#24314#35758#21246#36873
                  Caption = #38450#24481#26102#26174#31034#25928#26524
                  TabOrder = 4
                  OnClick = chkShowSuperShiledEffect2Click
                end
                object chkShowSuperShiledSound2: TCheckBox
                  Left = 120
                  Top = 57
                  Width = 105
                  Height = 17
                  Hint = #34987#25915#20987#26102#65292#20854#20182#29609#23478#26159#21542#21487#20197#21548#21040#22768#38899#65292#24314#35758#21246#36873
                  Caption = #38450#24481#26102#25773#25918#22768#38899
                  TabOrder = 5
                  OnClick = chkShowSuperShiledSound2Click
                end
              end
            end
            object TabSheet89: TTabSheet
              Caption = #20506#22825#36767#22320
              ImageIndex = 3
              object GroupBox175: TGroupBox
                Left = 8
                Top = 8
                Width = 173
                Height = 145
                Caption = #21442#25968#35774#32622
                TabOrder = 0
                object Label561: TLabel
                  Left = 32
                  Top = 24
                  Width = 60
                  Height = 12
                  Caption = #20919#21364#26102#38388#65306
                end
                object lbl24: TLabel
                  Left = 32
                  Top = 93
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#20493#25968#65306
                end
                object lbl25: TLabel
                  Left = 152
                  Top = 92
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object lbl23: TLabel
                  Left = 32
                  Top = 70
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#33539#22260#65306
                end
                object lbl26: TLabel
                  Left = 149
                  Top = 23
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label701: TLabel
                  Left = 8
                  Top = 47
                  Width = 84
                  Height = 12
                  Caption = #33521#38596#20919#21364#26102#38388#65306
                  Visible = False
                end
                object Label702: TLabel
                  Left = 149
                  Top = 45
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object Label823: TLabel
                  Left = 26
                  Top = 116
                  Width = 66
                  Height = 12
                  Caption = #27599#32423#21152#20260#23475':'
                end
                object Label824: TLabel
                  Left = 152
                  Top = 116
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill114HitWaitTime: TSpinEditEx
                  Left = 88
                  Top = 20
                  Width = 57
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill114HitWaitTimeChange
                end
                object seSkill114PowerRate: TSpinEditEx
                  Left = 88
                  Top = 89
                  Width = 57
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 1000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seSkill114PowerRateChange
                end
                object seSkill114AttackRange: TSpinEditEx
                  Left = 88
                  Top = 66
                  Width = 57
                  Height = 21
                  Hint = #39764#27861#25915#20987#33539#22260#21322#24452#12290#40664#35748'8'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 2
                  Value = 8
                  OnChange = seSkill114AttackRangeChange
                end
                object seHeroSkill114HitWaitTime: TSpinEditEx
                  Left = 88
                  Top = 43
                  Width = 57
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 3
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill114HitWaitTimeChange
                end
                object seSkill114LevelUpAddPowerRate: TSpinEditEx
                  Left = 90
                  Top = 112
                  Width = 58
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#25915#20987#20260#23475#22686#21152'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 10
                  OnChange = seSkill114LevelUpAddPowerRateChange
                end
              end
              object GroupBox84: TGroupBox
                Left = 8
                Top = 165
                Width = 173
                Height = 45
                Caption = #37322#25918#25511#21046
                TabOrder = 1
                object chkSkill114AttackUseNG: TCheckBox
                  Left = 16
                  Top = 18
                  Width = 129
                  Height = 17
                  Hint = #21246#36873#21518#35813#25216#33021#20351#29992#20869#21151#20540#37322#25918'('#28385#20869#21151#20540#37322#25918#19968#27425#21518#65292#20869#21151#20540#28165'0)'#65292#19981#21246#36873#21017#21017#29992#39764#27861#20540#37322#25918
                  Caption = #20351#29992#20869#21151#20540#37322#25918
                  TabOrder = 0
                  OnClick = chkSkill114AttackUseNGClick
                end
              end
            end
            object ts24: TTabSheet
              Caption = #34880#39748#19968#20987
              ImageIndex = 4
              object grp75: TGroupBox
                Left = 2
                Top = 5
                Width = 142
                Height = 132
                Caption = #25112#22763#35774#32622
                TabOrder = 1
                object Label836: TLabel
                  Left = 17
                  Top = 18
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label837: TLabel
                  Left = 126
                  Top = 18
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label838: TLabel
                  Left = 17
                  Top = 40
                  Width = 54
                  Height = 12
                  Caption = #20154#29289#38388#38548':'
                end
                object Label839: TLabel
                  Left = 123
                  Top = 40
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label840: TLabel
                  Left = 17
                  Top = 62
                  Width = 54
                  Height = 12
                  Caption = #33521#38596#38388#38548':'
                  Visible = False
                end
                object Label841: TLabel
                  Left = 123
                  Top = 62
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object Label858: TLabel
                  Left = 5
                  Top = 84
                  Width = 66
                  Height = 12
                  Caption = #27599#32423#21152#20260#23475':'
                end
                object Label859: TLabel
                  Left = 126
                  Top = 84
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill115PowerRate: TSpinEditEx
                  Left = 73
                  Top = 13
                  Width = 50
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill115PowerRateChange
                end
                object seSkill115HitWaitTime: TSpinEditEx
                  Left = 73
                  Top = 35
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seSkill115HitWaitTimeChange
                end
                object seHeroSkill115HitWaitTime: TSpinEditEx
                  Left = 73
                  Top = 57
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill115HitWaitTimeChange
                end
                object seSkill115LevelUpAddPowerRate: TSpinEditEx
                  Left = 73
                  Top = 79
                  Width = 50
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#25915#20987#20260#23475#22686#21152'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 10
                  OnChange = seSkill115LevelUpAddPowerRateChange
                end
              end
              object grp76: TGroupBox
                Left = 146
                Top = 5
                Width = 142
                Height = 132
                Caption = #27861#24072#35774#32622
                TabOrder = 2
                object Label842: TLabel
                  Left = 17
                  Top = 18
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label843: TLabel
                  Left = 127
                  Top = 18
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label846: TLabel
                  Left = 17
                  Top = 40
                  Width = 54
                  Height = 12
                  Caption = #20154#29289#38388#38548':'
                end
                object Label847: TLabel
                  Left = 124
                  Top = 39
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label848: TLabel
                  Left = 17
                  Top = 62
                  Width = 54
                  Height = 12
                  Caption = #33521#38596#38388#38548':'
                  Visible = False
                end
                object Label849: TLabel
                  Left = 124
                  Top = 63
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object Label854: TLabel
                  Left = 17
                  Top = 84
                  Width = 54
                  Height = 12
                  Caption = #20260#23475#33539#22260':'
                end
                object Label856: TLabel
                  Left = 5
                  Top = 107
                  Width = 66
                  Height = 12
                  Caption = #27599#32423#21152#20260#23475':'
                end
                object Label857: TLabel
                  Left = 126
                  Top = 107
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill116PowerRate: TSpinEditEx
                  Left = 74
                  Top = 13
                  Width = 50
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 0
                  Value = 100
                  OnChange = seSkill116PowerRateChange
                end
                object seSkill116HitWaitTime: TSpinEditEx
                  Left = 74
                  Top = 35
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  OnChange = seSkill116HitWaitTimeChange
                end
                object seHeroSkill116HitWaitTime: TSpinEditEx
                  Left = 74
                  Top = 57
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill116HitWaitTimeChange
                end
                object seSkill116Range: TSpinEditEx
                  Left = 74
                  Top = 79
                  Width = 50
                  Height = 21
                  Hint = #32676#25915#20260#23475#33539#22260#21322#24452#12290#40664#35748'1'#13#10#13#10#22914#35774#32622#20026'1'#65292#21017#33539#22260#20026'1*2+1'#65292#21363'3*3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 3
                  Value = 8
                  OnChange = seSkill116RangeChange
                end
                object seSkill116LevelUpAddPowerRate: TSpinEditEx
                  Left = 74
                  Top = 103
                  Width = 50
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#25915#20987#20260#23475#22686#21152'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 10
                  OnChange = seSkill116LevelUpAddPowerRateChange
                end
              end
              object GroupBox244: TGroupBox
                Left = 291
                Top = 5
                Width = 142
                Height = 132
                Caption = #36947#22763#35774#32622
                TabOrder = 0
                object Label850: TLabel
                  Left = 17
                  Top = 40
                  Width = 54
                  Height = 12
                  Caption = #20154#29289#38388#38548':'
                end
                object Label851: TLabel
                  Left = 124
                  Top = 40
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label852: TLabel
                  Left = 17
                  Top = 62
                  Width = 54
                  Height = 12
                  Caption = #33521#38596#38388#38548':'
                  Visible = False
                end
                object Label853: TLabel
                  Left = 124
                  Top = 63
                  Width = 12
                  Height = 12
                  Caption = #31186
                  Visible = False
                end
                object Label844: TLabel
                  Left = 17
                  Top = 18
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label845: TLabel
                  Left = 127
                  Top = 18
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label855: TLabel
                  Left = 17
                  Top = 85
                  Width = 54
                  Height = 12
                  Caption = #20260#23475#33539#22260':'
                end
                object Label860: TLabel
                  Left = 5
                  Top = 107
                  Width = 66
                  Height = 12
                  Caption = #27599#32423#21152#20260#23475':'
                end
                object Label861: TLabel
                  Left = 126
                  Top = 107
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkill117HitWaitTime: TSpinEditEx
                  Left = 74
                  Top = 36
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 10
                  OnChange = seSkill117HitWaitTimeChange
                end
                object seHeroSkill117HitWaitTime: TSpinEditEx
                  Left = 74
                  Top = 58
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 10
                  Visible = False
                  OnChange = seHeroSkill117HitWaitTimeChange
                end
                object seSkill117PowerRate: TSpinEditEx
                  Left = 74
                  Top = 14
                  Width = 50
                  Height = 21
                  Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 10000
                  MinValue = 1
                  TabOrder = 2
                  Value = 100
                  OnChange = seSkill117PowerRateChange
                end
                object seSkill117Range: TSpinEditEx
                  Left = 74
                  Top = 80
                  Width = 50
                  Height = 21
                  Hint = #32676#25915#20260#23475#33539#22260#21322#24452#12290#40664#35748'1'#13#10#13#10#22914#35774#32622#20026'1'#65292#21017#33539#22260#20026'1*2+1'#65292#21363'3*3'
                  MaxValue = 12
                  MinValue = 1
                  TabOrder = 3
                  Value = 8
                  OnChange = seSkill117RangeChange
                end
                object seSkill117LevelUpAddPowerRate: TSpinEditEx
                  Left = 73
                  Top = 102
                  Width = 50
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#25915#20987#20260#23475#22686#21152'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 10
                  OnChange = seSkill117LevelUpAddPowerRateChange
                end
              end
              object GroupBox243: TGroupBox
                Left = 2
                Top = 141
                Width = 430
                Height = 42
                Caption = #37322#25918#25511#21046
                TabOrder = 3
                object chkSkill115UseNG: TCheckBox
                  Left = 8
                  Top = 17
                  Width = 113
                  Height = 17
                  Caption = #20351#29992#20869#21151#20540#37322#25918
                  TabOrder = 0
                  OnClick = chkSkill115UseNGClick
                end
                object chkSkill115NGNoEnoughDecHP: TCheckBox
                  Left = 129
                  Top = 17
                  Width = 193
                  Height = 17
                  Caption = #26080#20869#21151#25110#20869#21147#19981#36275#26102#25187#33258#36523#34880#37327
                  TabOrder = 1
                  OnClick = chkSkill115NGNoEnoughDecHPClick
                end
                object seSkill115NGNoEnoughDecHPValue: TSpinEditEx
                  Left = 322
                  Top = 15
                  Width = 50
                  Height = 21
                  Hint = #27599#25552#21319#19968#32423#65292#25915#20987#20260#23475#22686#21152'%'
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 10
                  OnChange = seSkill115NGNoEnoughDecHPValueChange
                end
                object cbbSkill115NGNoEnoughDecHPType: TComboBox
                  Left = 373
                  Top = 15
                  Width = 51
                  Height = 20
                  Hint = '%'#65306#20197#24403#21069#21097#20313#34880#37327#30340#30334#20998#27604#25187#38500
                  Style = csDropDownList
                  ItemIndex = 0
                  TabOrder = 3
                  Text = '%'
                  OnChange = cbbSkill115NGNoEnoughDecHPTypeChange
                  Items.Strings = (
                    '%'
                    #28857)
                end
              end
            end
          end
        end
        object TabSheet68: TTabSheet
          Caption = #20869#21151#25216#33021
          ImageIndex = 7
          object GroupBox132: TGroupBox
            Left = 4
            Top = 5
            Width = 160
            Height = 116
            Caption = #30456#20851#21442#25968
            TabOrder = 0
            object Label282: TLabel
              Left = 20
              Top = 20
              Width = 72
              Height = 12
              Caption = #20869#21147#20540#21442#25968#65306
            end
            object Label283: TLabel
              Left = 8
              Top = 44
              Width = 84
              Height = 12
              Caption = #20027#20307#32463#39564#21442#25968#65306
            end
            object Label285: TLabel
              Left = 8
              Top = 68
              Width = 84
              Height = 12
              Caption = #33521#38596#32463#39564#21442#25968#65306
            end
            object Label907: TLabel
              Left = 8
              Top = 92
              Width = 84
              Height = 12
              Caption = #26368#39640#20869#21151#31561#32423#65306
            end
            object EditNGLevelValue: TSpinEditEx
              Left = 88
              Top = 16
              Width = 65
              Height = 21
              Hint = #20869#32622#20844#24335#35745#31639#20986#30340#20540'+'#35774#32622#20540','#21363#20026#27599#20010#31561#32423#30340#20869#21147#20540#19978#38480#13#10#27880':'#26368#22823#20869#21147#20540#19978#38480#20026'65535.'#40664#35748#20540'10'
              MaxValue = 50
              MinValue = 1
              TabOrder = 0
              Value = 10
              OnChange = EditNGLevelValueChange
            end
            object EditNGLevelExpValue: TSpinEditEx
              Left = 88
              Top = 40
              Width = 65
              Height = 21
              Hint = #20869#32622#20844#24335#35745#31639#20986#30340#20540'+'#35774#32622#20540','#13#10#21363#20026#27599#20010#31561#32423#30340#21319#32423#25152#38656#30340#32463#39564#13#10#40664#35748#20540':55330'
              MaxValue = 10000000
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = EditNGLevelExpValueChange
            end
            object EditNGHeroLevelExpValue: TSpinEditEx
              Left = 88
              Top = 64
              Width = 65
              Height = 21
              Hint = #20869#32622#20844#24335#35745#31639#20986#30340#20540'+'#35774#32622#20540','#13#10#21363#20026#27599#20010#31561#32423#30340#21319#32423#25152#38656#30340#32463#39564#13#10#40664#35748#20540':62400'
              MaxValue = 10000000
              MinValue = 1
              TabOrder = 2
              Value = 10
              OnChange = EditNGHeroLevelExpValueChange
            end
            object seNGMaxLevelLimte: TSpinEditEx
              Left = 87
              Top = 88
              Width = 65
              Height = 21
              MaxValue = 65535
              MinValue = 1
              TabOrder = 3
              Value = 10
              OnChange = seNGMaxLevelLimteChange
            end
          end
          object GroupBox133: TGroupBox
            Left = 4
            Top = 125
            Width = 161
            Height = 48
            Caption = #20869#21147#24674#22797#36895#24230
            TabOrder = 2
            object Label286: TLabel
              Left = 8
              Top = 24
              Width = 84
              Height = 12
              Caption = #20869#21147#24674#22797#36895#24230#65306
            end
            object EditNGIncTime: TSpinEditEx
              Left = 88
              Top = 19
              Width = 65
              Height = 21
              MaxValue = 65535
              MinValue = 1
              TabOrder = 0
              Value = 8
              OnChange = EditNGIncTimeChange
            end
          end
          object GroupBox134: TGroupBox
            Left = 4
            Top = 177
            Width = 162
            Height = 45
            Caption = #20869#21151#25216#33021
            TabOrder = 4
            object Label287: TLabel
              Left = 8
              Top = 19
              Width = 138
              Height = 12
              Caption = #22686#24378#25915'('#38450')'#65306'          %'
            end
            object EditNGSkillPowerRate: TSpinEditEx
              Left = 82
              Top = 15
              Width = 55
              Height = 21
              Hint = 
                #20869#21151#25216#33021#27599#32423#21487#22686#21152#30340#23041#21147#27604#29575','#22914#28872#28779#23041#21147#20026'100,'#13#10#23398#20064'0'#32423#24594#20043#28872#28779','#35774#32622#20540#20026'25,'#21017#26368#21518#23041#21147#20026':100+100*25/100' +
                '=125'#13#10#23398#20064'1'#32423#24594#20043#28872#28779','#35774#32622#20540#20026'25,'#21017#26368#21518#23041#21147#20026':100+100*50/100=150'#13#10#13#10#22914#23398#38745#20043#28872#28779'0'#32423','#30446#26631#23398#26377 +
                #24594#20043#28872#28779'0'#32423','#35774#32622#20540#20026'25 '#13#10#23398#20064'0'#32423#38745#20043#28872#28779','#35774#32622#20540#20026'25,'#21017#26368#21518#23041#21147#20026':125-100*25/100=100'
              MaxValue = 10000
              MinValue = 1
              TabOrder = 0
              Value = 25
              OnChange = EditNGSkillPowerRateChange
            end
          end
          object GroupBox135: TGroupBox
            Left = 180
            Top = 128
            Width = 225
            Height = 67
            Caption = #20869#21151#31561#32423'+'#25915#38450
            TabOrder = 5
            object Label288: TLabel
              Left = 8
              Top = 19
              Width = 12
              Height = 12
              Caption = #27599
            end
            object lbl145: TLabel
              Left = 67
              Top = 19
              Width = 96
              Height = 12
              Caption = #32423#20869#21151#22686#21152#25915#20987#28857
            end
            object Label762: TLabel
              Left = 8
              Top = 43
              Width = 12
              Height = 12
              Caption = #27599
            end
            object Label763: TLabel
              Left = 67
              Top = 43
              Width = 96
              Height = 12
              Caption = #32423#20869#21151#22686#21152#25915#20987#28857
            end
            object edtNGLevelPowerAdd_Level: TSpinEditEx
              Left = 22
              Top = 15
              Width = 44
              Height = 21
              Hint = #20869#21151#31561#32423#22686#21152#25915#20987#30340#35774#32622','#22914#35774#32622#20026'8'#13#21017#27599'8'#32423#30340#20869#21151#31561#32423#21487#22686#21152'n'#28857#25915#20987
              MaxValue = 10
              MinValue = 1
              TabOrder = 0
              Value = 8
              OnChange = edtNGLevelPowerAdd_LevelChange
            end
            object edtNGLevelPowerAdd_Power: TSpinEditEx
              Left = 166
              Top = 15
              Width = 44
              Height = 21
              MaxValue = 10
              MinValue = 1
              TabOrder = 1
              Value = 8
              OnChange = edtNGLevelPowerAdd_PowerChange
            end
            object edtNGLevelPowerDec_Level: TSpinEditEx
              Left = 22
              Top = 39
              Width = 44
              Height = 21
              Hint = #20869#21151#31561#32423#22686#21152#38450#24481#30340#35774#32622','#22914#35774#32622#20026'8'#13#21017#27599'8'#32423#30340#20869#21151#31561#32423#21487#22686#21152'n'#28857#20869#21151#38450#24481
              MaxValue = 10
              MinValue = 1
              TabOrder = 2
              Value = 8
              OnChange = edtNGLevelPowerDec_LevelChange
            end
            object edtNGLevelPowerDec_Power: TSpinEditEx
              Left = 166
              Top = 39
              Width = 44
              Height = 21
              MaxValue = 10
              MinValue = 1
              TabOrder = 3
              Value = 8
              OnChange = edtNGLevelPowerDec_PowerChange
            end
          end
          object GroupBox136: TGroupBox
            Left = 180
            Top = 78
            Width = 193
            Height = 45
            Caption = #26432#24618#20869#21151#32463#39564
            TabOrder = 3
            object Label289: TLabel
              Left = 8
              Top = 22
              Width = 114
              Height = 12
              Caption = #26432#24618#20869#21151#32463#39564#20493#29575':  '
              Font.Charset = GB2312_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
            end
            object Label761: TLabel
              Left = 178
              Top = 22
              Width = 6
              Height = 12
              Caption = '%'
            end
            object EditNGKillMonExpMultiple: TSpinEditEx
              Left = 113
              Top = 18
              Width = 62
              Height = 21
              Hint = #26432#24618#25152#24471#30340#32463#39564'*'#27492#21442#25968'/100='#24471#21040#30340#20869#21151#32463#39564
              MaxValue = 65535
              MinValue = 1
              TabOrder = 0
              Value = 40
              OnChange = EditNGKillMonExpMultipleChange
            end
          end
          object GroupBox137: TGroupBox
            Left = 180
            Top = 5
            Width = 193
            Height = 67
            Caption = #20869#21151#30456#20851
            TabOrder = 1
            object Label290: TLabel
              Left = 8
              Top = 20
              Width = 108
              Height = 12
              Caption = #39278#37202#22686#21152#20869#21151#32463#39564#20540
            end
            object Label291: TLabel
              Left = 8
              Top = 44
              Width = 132
              Height = 12
              Caption = #22686#21152#25915#20987'('#38450#24481')'#20943#20869#21147#20540
            end
            object EditNGDrinkIncExp: TSpinEditEx
              Left = 123
              Top = 15
              Width = 62
              Height = 21
              Hint = #39278#37202#21487#20197#33719#24471#20869#21151#32463#39564#65292#37202#37327#36234#22823#65292#39278#29992#37202#30340#21697#36136#36234#39640#65292#13#10#33719#24471#20869#21151#32463#39564#36234#22810','#26377#37257#37202#24230#26102#27599#27425#22686#21152#30340#20869#21151#32463#39564
              MaxValue = 100000000
              MinValue = 1
              TabOrder = 0
              Value = 10
              OnChange = EditNGDrinkIncExpChange
            end
            object EditNGHitStruckDecNG: TSpinEditEx
              Left = 143
              Top = 39
              Width = 42
              Height = 21
              Hint = #20869#21151#23545#20110#26222#36890#25915#20987#38450#25269#24481','#38656#35201#28040#32791#30340#20869#21147#20540#13#10#20869#21151#23545#20110#22686#21152#25915#20987#21147','#38656#35201#28040#32791#30340#20869#21147#20540
              MaxValue = 65535
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = EditNGHitStruckDecNGChange
            end
          end
        end
        object TabSheet53: TTabSheet
          Caption = #36830#20987#25216#33021
          ImageIndex = 6
          object PageControl7: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            ActivePage = ts25
            Align = alClient
            TabOrder = 0
            object TabSheet66: TTabSheet
              Caption = #22522#26412#35774#32622
              ImageIndex = 1
              object GroupBox131: TGroupBox
                Left = 8
                Top = 8
                Width = 144
                Height = 49
                Caption = #26102#38388#25511#21046
                TabOrder = 0
                object Label281: TLabel
                  Left = 125
                  Top = 25
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label284: TLabel
                  Left = 8
                  Top = 24
                  Width = 60
                  Height = 12
                  Caption = #20351#29992#38388#38548#65306
                end
                object EditUseContinuousMagicTime: TSpinEditEx
                  Left = 65
                  Top = 20
                  Width = 57
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 30
                  OnChange = EditUseContinuousMagicTimeChange
                end
              end
              object grp42: TGroupBox
                Left = 8
                Top = 64
                Width = 144
                Height = 45
                Caption = #36830#20987#37322#25918#25511#21046
                TabOrder = 1
                object chkContinuousAttackUseNG: TCheckBox
                  Left = 16
                  Top = 18
                  Width = 113
                  Height = 17
                  Hint = #21246#36873#21518#36830#20987#25216#33021#20351#29992#20869#21151#20540#37322#25918'('#28385#20869#21151#20540#37322#25918#19968#27425#21518#65292#20869#21151#20540#28165'0)'#65292#19981#21246#36873#21017#21017#29992#39764#27861#20540#37322#25918
                  Caption = #20351#29992#20869#21151#20540#37322#25918
                  TabOrder = 0
                  OnClick = chkContinuousAttackUseNGClick
                end
              end
              object GroupBox209: TGroupBox
                Left = 8
                Top = 115
                Width = 144
                Height = 73
                Caption = #36947#27861#20445#25252#30462
                TabOrder = 2
                object Label668: TLabel
                  Left = 8
                  Top = 24
                  Width = 60
                  Height = 12
                  Caption = #22266#23450#20445#25252#65306
                end
                object Label669: TLabel
                  Left = 8
                  Top = 49
                  Width = 60
                  Height = 12
                  Caption = #38543#26426#20445#25252#65306
                end
                object Label670: TLabel
                  Left = 125
                  Top = 25
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label671: TLabel
                  Left = 125
                  Top = 49
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seContinuousProtect: TSpinEditEx
                  Left = 65
                  Top = 20
                  Width = 57
                  Height = 21
                  Hint = #36947#27861#37322#25918#36830#20987#26102#65292#21487#25269#24481#21463#21040#30340#20260#23475#26368#20302#30334#20998#27604
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 30
                  OnChange = seContinuousProtectChange
                end
                object seContinuousProtectRandom: TSpinEditEx
                  Left = 65
                  Top = 45
                  Width = 57
                  Height = 21
                  Hint = #36947#27861#37322#25918#36830#20987#26102#65292#21487#25269#24481#21463#21040#30340#20260#23475#26368#22823#38543#26426#30334#20998#27604#13#10#22914#26524#38543#26426#20540#23567#20110#22266#23450#20540#65292#21017#20197#22266#23450#20540#20026#20934
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 1
                  Value = 30
                  OnChange = seContinuousProtectRandomChange
                end
              end
              object GroupBox223: TGroupBox
                Left = 159
                Top = 8
                Width = 208
                Height = 49
                Caption = #36830#20987#25216#33021#20260#23475#25511#21046
                TabOrder = 3
                object Label744: TLabel
                  Left = 8
                  Top = 24
                  Width = 144
                  Height = 12
                  Caption = #25216#33021#27599#25552#21319#19968#32423#20260#23475#22686#21152#65306
                end
                object Label745: TLabel
                  Left = 195
                  Top = 24
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seContinuousAttackLevelRate: TSpinEditEx
                  Left = 150
                  Top = 19
                  Width = 42
                  Height = 21
                  MaxValue = 300
                  MinValue = 1
                  TabOrder = 0
                  Value = 1
                  OnChange = seContinuousAttackLevelRateChange
                end
              end
              object grp1: TGroupBox
                Left = 159
                Top = 64
                Width = 122
                Height = 125
                Caption = #36830#20987#39034#24207#26292#20987#20960#29575
                TabOrder = 4
                object lblContinueBlastRateOrder1: TLabel
                  Left = 8
                  Top = 24
                  Width = 48
                  Height = 12
                  Caption = #31532#19968#26684#65306
                end
                object lblContinueBlastRateOrder2: TLabel
                  Left = 8
                  Top = 48
                  Width = 48
                  Height = 12
                  Caption = #31532#20108#26684#65306
                end
                object lblContinueBlastRateOrder3: TLabel
                  Left = 8
                  Top = 71
                  Width = 48
                  Height = 12
                  Caption = #31532#19977#26684#65306
                end
                object lblContinueBlastRateOrder4: TLabel
                  Left = 8
                  Top = 95
                  Width = 48
                  Height = 12
                  Caption = #31532#22235#26684#65306
                end
                object Label635: TLabel
                  Left = 106
                  Top = 24
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label672: TLabel
                  Left = 106
                  Top = 48
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label673: TLabel
                  Left = 106
                  Top = 71
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label904: TLabel
                  Left = 106
                  Top = 95
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seSkillContinueOrderBlastRates1: TSpinEditEx
                  Left = 55
                  Top = 19
                  Width = 45
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 0
                  Value = 1
                  OnChange = seSkillContinueOrderBlastRates1Change
                end
                object seSkillContinueOrderBlastRates2: TSpinEditEx
                  Tag = 1
                  Left = 55
                  Top = 43
                  Width = 45
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 1
                  Value = 1
                  OnChange = seSkillContinueOrderBlastRates1Change
                end
                object seSkillContinueOrderBlastRates3: TSpinEditEx
                  Tag = 2
                  Left = 55
                  Top = 67
                  Width = 45
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 2
                  Value = 1
                  OnChange = seSkillContinueOrderBlastRates1Change
                end
                object seSkillContinueOrderBlastRates4: TSpinEditEx
                  Tag = 3
                  Left = 55
                  Top = 91
                  Width = 45
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 3
                  Value = 1
                  OnChange = seSkillContinueOrderBlastRates1Change
                end
              end
            end
            object TabSheet65: TTabSheet
              Caption = #32463#32476#30456#20851
              object PageControl12: TPageControl
                Left = 0
                Top = 0
                Width = 433
                Height = 267
                ActivePage = TabSheet84
                Align = alClient
                TabOrder = 0
                object TabSheet84: TTabSheet
                  Caption = #20914#33033
                  object GroupBox125: TGroupBox
                    Left = 8
                    Top = 8
                    Width = 113
                    Height = 177
                    Caption = #38656#35201#20869#21151#31561#32423
                    TabOrder = 0
                    object Label251: TLabel
                      Left = 8
                      Top = 22
                      Width = 24
                      Height = 12
                      Caption = #24189#38376
                    end
                    object Label252: TLabel
                      Left = 8
                      Top = 50
                      Width = 24
                      Height = 12
                      Caption = #36890#35895
                    end
                    object Label253: TLabel
                      Left = 8
                      Top = 80
                      Width = 24
                      Height = 12
                      Caption = #21830#26354
                    end
                    object Label254: TLabel
                      Left = 8
                      Top = 111
                      Width = 24
                      Height = 12
                      Caption = #22235#28385
                    end
                    object Label255: TLabel
                      Left = 8
                      Top = 143
                      Width = 24
                      Height = 12
                      Caption = #27178#39592
                    end
                    object EditAcupoints0_0: TSpinEditEx
                      Left = 39
                      Top = 17
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = EditAcupoints0_0Change
                    end
                    object EditAcupoints0_1: TSpinEditEx
                      Tag = 1
                      Left = 39
                      Top = 47
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 255
                      TabOrder = 1
                      Value = 0
                      OnChange = EditAcupoints0_0Change
                    end
                    object EditAcupoints0_2: TSpinEditEx
                      Tag = 2
                      Left = 39
                      Top = 76
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = EditAcupoints0_0Change
                    end
                    object EditAcupoints0_3: TSpinEditEx
                      Tag = 3
                      Left = 39
                      Top = 106
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = EditAcupoints0_0Change
                    end
                    object EditAcupoints0_4: TSpinEditEx
                      Tag = 4
                      Left = 39
                      Top = 139
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 4
                      Value = 0
                      OnChange = EditAcupoints0_0Change
                    end
                  end
                end
                object TabSheet85: TTabSheet
                  Caption = #38452#36343
                  ImageIndex = 1
                  object GroupBox126: TGroupBox
                    Left = 8
                    Top = 8
                    Width = 113
                    Height = 177
                    Caption = #25171#36890#38656#35201#20869#21151#31561#32423
                    TabOrder = 0
                    object Label256: TLabel
                      Left = 8
                      Top = 22
                      Width = 24
                      Height = 12
                      Caption = #26228#26126
                    end
                    object Label257: TLabel
                      Left = 8
                      Top = 50
                      Width = 24
                      Height = 12
                      Caption = #30424#32570
                    end
                    object Label258: TLabel
                      Left = 8
                      Top = 80
                      Width = 24
                      Height = 12
                      Caption = #20132#20449
                    end
                    object Label259: TLabel
                      Left = 8
                      Top = 111
                      Width = 24
                      Height = 12
                      Caption = #29031#28023
                    end
                    object Label260: TLabel
                      Left = 8
                      Top = 143
                      Width = 24
                      Height = 12
                      Caption = #28982#39592
                    end
                    object EditAcupoints1_0: TSpinEditEx
                      Left = 39
                      Top = 17
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = EditAcupoints1_0Change
                    end
                    object EditAcupoints1_1: TSpinEditEx
                      Tag = 1
                      Left = 39
                      Top = 47
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 1
                      Value = 0
                      OnChange = EditAcupoints1_0Change
                    end
                    object EditAcupoints1_2: TSpinEditEx
                      Tag = 2
                      Left = 39
                      Top = 76
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = EditAcupoints1_0Change
                    end
                    object EditAcupoints1_3: TSpinEditEx
                      Tag = 3
                      Left = 39
                      Top = 106
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = EditAcupoints1_0Change
                    end
                    object EditAcupoints1_4: TSpinEditEx
                      Tag = 4
                      Left = 39
                      Top = 139
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 4
                      Value = 0
                      OnChange = EditAcupoints1_0Change
                    end
                  end
                end
                object TabSheet86: TTabSheet
                  Caption = #38452#32500
                  ImageIndex = 2
                  object GroupBox127: TGroupBox
                    Left = 8
                    Top = 8
                    Width = 113
                    Height = 177
                    Caption = #25171#36890#38656#35201#20869#21151#31561#32423
                    TabOrder = 0
                    object Label261: TLabel
                      Left = 9
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = #24265#27849
                    end
                    object Label262: TLabel
                      Left = 8
                      Top = 50
                      Width = 24
                      Height = 12
                      Caption = #26399#38376
                    end
                    object Label263: TLabel
                      Left = 8
                      Top = 80
                      Width = 24
                      Height = 12
                      Caption = #24220#33293
                    end
                    object Label264: TLabel
                      Left = 8
                      Top = 111
                      Width = 24
                      Height = 12
                      Caption = #20914#38376
                    end
                    object Label265: TLabel
                      Left = 8
                      Top = 143
                      Width = 24
                      Height = 12
                      Caption = #31569#23486
                    end
                    object EditAcupoints2_0: TSpinEditEx
                      Left = 39
                      Top = 17
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = EditAcupoints2_0Change
                    end
                    object EditAcupoints2_1: TSpinEditEx
                      Tag = 1
                      Left = 39
                      Top = 47
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 1
                      Value = 0
                      OnChange = EditAcupoints2_0Change
                    end
                    object EditAcupoints2_2: TSpinEditEx
                      Tag = 2
                      Left = 39
                      Top = 76
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = EditAcupoints2_0Change
                    end
                    object EditAcupoints2_3: TSpinEditEx
                      Tag = 3
                      Left = 39
                      Top = 106
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = EditAcupoints2_0Change
                    end
                    object EditAcupoints2_4: TSpinEditEx
                      Tag = 4
                      Left = 39
                      Top = 139
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 4
                      Value = 0
                      OnChange = EditAcupoints2_0Change
                    end
                  end
                end
                object TabSheet87: TTabSheet
                  Caption = #20219#33033
                  ImageIndex = 3
                  object GroupBox128: TGroupBox
                    Left = 8
                    Top = 8
                    Width = 113
                    Height = 177
                    Caption = #25171#36890#38656#35201#20869#21151#31561#32423
                    TabOrder = 0
                    object Label266: TLabel
                      Left = 8
                      Top = 22
                      Width = 24
                      Height = 12
                      Caption = #25215#27974
                    end
                    object Label267: TLabel
                      Left = 8
                      Top = 50
                      Width = 24
                      Height = 12
                      Caption = #22825#31361
                    end
                    object Label268: TLabel
                      Left = 8
                      Top = 80
                      Width = 24
                      Height = 12
                      Caption = #40480#23614
                    end
                    object Label269: TLabel
                      Left = 8
                      Top = 111
                      Width = 24
                      Height = 12
                      Caption = #27668#28023
                    end
                    object Label270: TLabel
                      Left = 8
                      Top = 143
                      Width = 24
                      Height = 12
                      Caption = #26354#39592
                    end
                    object EditAcupoints3_0: TSpinEditEx
                      Left = 39
                      Top = 17
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = EditAcupoints3_0Change
                    end
                    object EditAcupoints3_1: TSpinEditEx
                      Tag = 1
                      Left = 39
                      Top = 47
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 1
                      Value = 0
                      OnChange = EditAcupoints3_0Change
                    end
                    object EditAcupoints3_2: TSpinEditEx
                      Tag = 2
                      Left = 39
                      Top = 76
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = EditAcupoints3_0Change
                    end
                    object EditAcupoints3_3: TSpinEditEx
                      Tag = 3
                      Left = 39
                      Top = 106
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = EditAcupoints3_0Change
                    end
                    object EditAcupoints3_4: TSpinEditEx
                      Tag = 4
                      Left = 39
                      Top = 139
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 4
                      Value = 0
                      OnChange = EditAcupoints3_0Change
                    end
                  end
                end
                object TabSheet88: TTabSheet
                  Caption = #22855#32463
                  ImageIndex = 4
                  object GroupBox174: TGroupBox
                    Left = 8
                    Top = 8
                    Width = 113
                    Height = 177
                    Caption = #25171#36890#38656#35201#20869#21151#31561#32423
                    TabOrder = 0
                    object Label556: TLabel
                      Left = 8
                      Top = 22
                      Width = 24
                      Height = 12
                      Caption = #31070#20914
                    end
                    object Label557: TLabel
                      Left = 8
                      Top = 80
                      Width = 24
                      Height = 12
                      Caption = #22841#33034
                    end
                    object Label558: TLabel
                      Left = 8
                      Top = 51
                      Width = 24
                      Height = 12
                      Caption = #20108#30334
                    end
                    object Label559: TLabel
                      Left = 8
                      Top = 111
                      Width = 24
                      Height = 12
                      Caption = #20843#39118
                    end
                    object Label560: TLabel
                      Left = 8
                      Top = 143
                      Width = 24
                      Height = 12
                      Caption = #28044#27849
                    end
                    object EditAcupoints4_0: TSpinEditEx
                      Left = 39
                      Top = 17
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = EditAcupoints4_0Change
                    end
                    object EditAcupoints4_1: TSpinEditEx
                      Tag = 1
                      Left = 39
                      Top = 47
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 255
                      TabOrder = 1
                      Value = 0
                      OnChange = EditAcupoints4_0Change
                    end
                    object EditAcupoints4_2: TSpinEditEx
                      Tag = 2
                      Left = 39
                      Top = 76
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = EditAcupoints4_0Change
                    end
                    object EditAcupoints4_3: TSpinEditEx
                      Tag = 3
                      Left = 39
                      Top = 106
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = EditAcupoints4_0Change
                    end
                    object EditAcupoints4_4: TSpinEditEx
                      Tag = 4
                      Left = 39
                      Top = 139
                      Width = 50
                      Height = 21
                      MaxValue = 255
                      MinValue = 0
                      TabOrder = 4
                      Value = 0
                      OnChange = EditAcupoints4_0Change
                    end
                  end
                end
              end
            end
            object ts25: TTabSheet
              Caption = #25112#22763#36830#20987
              ImageIndex = 3
              object PageControl9: TPageControl
                Left = 0
                Top = 0
                Width = 433
                Height = 267
                ActivePage = TabSheet72
                Align = alClient
                MultiLine = True
                TabOrder = 0
                object TabSheet72: TTabSheet
                  Caption = #36861#24515#21050
                  object GroupBox138: TGroupBox
                    Left = 8
                    Top = 1
                    Width = 161
                    Height = 93
                    Caption = #25915#20987#35774#32622
                    TabOrder = 0
                    object Label292: TLabel
                      Left = 8
                      Top = 20
                      Width = 66
                      Height = 12
                      Caption = #25915#20987#21147#20493#25968':'
                    end
                    object Label293: TLabel
                      Left = 148
                      Top = 20
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label764: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = #24573#35270#38450#24481#20960#29575
                    end
                    object Label765: TLabel
                      Left = 148
                      Top = 44
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label772: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = #30772#38450#24481#30462#20960#29575
                    end
                    object Label887: TLabel
                      Left = 147
                      Top = 68
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousPowerRate100: TSpinEditEx
                      Left = 86
                      Top = 15
                      Width = 57
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                    object edtSkillContinuousCloseDefenseRates100: TSpinEditEx
                      Left = 86
                      Top = 39
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 1
                      TabOrder = 1
                      Value = 100
                      OnChange = edtSkillContinuousCloseDefenseRates100Change
                    end
                    object seSkill100BreakDefenceUpRate: TSpinEditEx
                      Left = 86
                      Top = 63
                      Width = 57
                      Height = 21
                      Hint = #30772#39764#27861#30462'/'#26032#27494#21147#30462'/'#26032#36947#21147#30462#20960#29575
                      MaxValue = 100
                      MinValue = 1
                      TabOrder = 2
                      Value = 100
                      OnChange = seSkill100BreakDefenceUpRateChange
                    end
                  end
                  object GroupBox172: TGroupBox
                    Left = 8
                    Top = 100
                    Width = 161
                    Height = 141
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label536: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label537: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label538: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label539: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label540: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label541: TLabel
                      Left = 148
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label542: TLabel
                      Left = 148
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label543: TLabel
                      Left = 148
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label544: TLabel
                      Left = 148
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label545: TLabel
                      Left = 148
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate100_0: TSpinEditEx
                      Left = 86
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate100_0Change
                    end
                    object EditSkillContinuousBlastHitRate100_1: TSpinEditEx
                      Tag = 1
                      Left = 86
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate100_0Change
                    end
                    object EditSkillContinuousBlastHitRate100_2: TSpinEditEx
                      Tag = 2
                      Left = 86
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate100_0Change
                    end
                    object EditSkillContinuousBlastHitRate100_3: TSpinEditEx
                      Tag = 3
                      Left = 86
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate100_0Change
                    end
                    object EditSkillContinuousBlastHitRate100_4: TSpinEditEx
                      Tag = 4
                      Left = 86
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate100_0Change
                    end
                  end
                  object GroupBox173: TGroupBox
                    Left = 176
                    Top = 100
                    Width = 217
                    Height = 141
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label546: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label547: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label548: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label549: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label550: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label551: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label552: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label553: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label554: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label555: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates100_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates100_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates100_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates100_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates100_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates100_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates100_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates100_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates100_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates100_0Change
                    end
                  end
                  object grp15: TGroupBox
                    Left = 176
                    Top = 1
                    Width = 241
                    Height = 43
                    Caption = #25512#21160#35774#32622
                    TabOrder = 3
                    object lbl28: TLabel
                      Left = 8
                      Top = 20
                      Width = 72
                      Height = 12
                      Caption = #25512#21160#30446#26631#36317#31163
                    end
                    object chkDoMotaebo100PushSameLevel: TCheckBox
                      Left = 145
                      Top = 17
                      Width = 85
                      Height = 19
                      Caption = #21487#25512#21516#31561#32423
                      TabOrder = 0
                      OnClick = chkDoMotaebo100PushSameLevelClick
                    end
                    object seDoMotaebo100PushDistance: TSpinEditEx
                      Left = 85
                      Top = 16
                      Width = 44
                      Height = 21
                      MaxValue = 4
                      MinValue = 1
                      TabOrder = 1
                      Value = 4
                      OnChange = seDoMotaebo100PushDistanceChange
                    end
                  end
                  object GroupBox107: TGroupBox
                    Left = 176
                    Top = 46
                    Width = 241
                    Height = 43
                    Caption = #25112#25216#23553#38145#35774#32622
                    TabOrder = 4
                    object Label888: TLabel
                      Left = 8
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #20960#29575#65306
                    end
                    object Label889: TLabel
                      Left = 97
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label890: TLabel
                      Left = 128
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #26102#38388#65306
                    end
                    object Label891: TLabel
                      Left = 217
                      Top = 21
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object seWarrContinuousStatusLock100: TSpinEditEx
                      Left = 41
                      Top = 17
                      Width = 53
                      Height = 21
                      Hint = #35774#32622#34987#25112#22763#36830#20987#25216#33021#25171#20013#30340#29366#24577'('#19981#33021#31227#21160#65292#19981#33021#39764#27861')'#30340#20960#29575#65292#40664#35748#20026'100%'
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 30
                      OnChange = seWarrContinuousStatusLockChange
                    end
                    object seWarrContinuousStatusLockTime100: TSpinEditEx
                      Left = 161
                      Top = 17
                      Width = 53
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 30
                      OnChange = seWarrContinuousStatusLockTimeChange
                    end
                  end
                end
                object TabSheet73: TTabSheet
                  Caption = #19977#32477#26432
                  ImageIndex = 1
                  object GroupBox139: TGroupBox
                    Left = 8
                    Top = 1
                    Width = 161
                    Height = 66
                    Caption = #25915#20987#35774#32622
                    TabOrder = 0
                    object Label294: TLabel
                      Left = 8
                      Top = 20
                      Width = 66
                      Height = 12
                      Caption = #25915#20987#21147#20493#25968':'
                    end
                    object Label295: TLabel
                      Left = 148
                      Top = 20
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label766: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = #24573#35270#38450#24481#20960#29575
                    end
                    object Label767: TLabel
                      Left = 148
                      Top = 44
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousPowerRate101: TSpinEditEx
                      Tag = 1
                      Left = 86
                      Top = 15
                      Width = 57
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                    object edtSkillContinuousCloseDefenseRates101: TSpinEditEx
                      Tag = 1
                      Left = 86
                      Top = 39
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 1
                      TabOrder = 1
                      Value = 100
                      OnChange = edtSkillContinuousCloseDefenseRates100Change
                    end
                  end
                  object GroupBox170: TGroupBox
                    Left = 8
                    Top = 74
                    Width = 161
                    Height = 141
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label516: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label517: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label518: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label519: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label520: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label521: TLabel
                      Left = 148
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label522: TLabel
                      Left = 148
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label523: TLabel
                      Left = 148
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label524: TLabel
                      Left = 148
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label525: TLabel
                      Left = 148
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate101_0: TSpinEditEx
                      Left = 86
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate101_0Change
                    end
                    object EditSkillContinuousBlastHitRate101_1: TSpinEditEx
                      Tag = 1
                      Left = 86
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate101_0Change
                    end
                    object EditSkillContinuousBlastHitRate101_2: TSpinEditEx
                      Tag = 2
                      Left = 86
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate101_0Change
                    end
                    object EditSkillContinuousBlastHitRate101_3: TSpinEditEx
                      Tag = 3
                      Left = 86
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate101_0Change
                    end
                    object EditSkillContinuousBlastHitRate101_4: TSpinEditEx
                      Tag = 4
                      Left = 86
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate101_0Change
                    end
                  end
                  object GroupBox171: TGroupBox
                    Left = 176
                    Top = 74
                    Width = 217
                    Height = 141
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label526: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label527: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label528: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label529: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label530: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label531: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label532: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label533: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label534: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label535: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates101_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates101_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates101_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates101_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates101_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates101_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates101_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates101_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates101_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates101_0Change
                    end
                  end
                  object GroupBox108: TGroupBox
                    Left = 176
                    Top = 1
                    Width = 241
                    Height = 43
                    Caption = #25112#25216#23553#38145#35774#32622
                    TabOrder = 3
                    object Label892: TLabel
                      Left = 8
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #20960#29575#65306
                    end
                    object Label893: TLabel
                      Left = 97
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label894: TLabel
                      Left = 128
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #26102#38388#65306
                    end
                    object Label895: TLabel
                      Left = 217
                      Top = 21
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object seWarrContinuousStatusLock101: TSpinEditEx
                      Tag = 1
                      Left = 41
                      Top = 17
                      Width = 53
                      Height = 21
                      Hint = #35774#32622#34987#25112#22763#36830#20987#25216#33021#25171#20013#30340#29366#24577'('#19981#33021#31227#21160#65292#19981#33021#39764#27861')'#30340#20960#29575#65292#40664#35748#20026'100%'
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 30
                      OnChange = seWarrContinuousStatusLockChange
                    end
                    object seWarrContinuousStatusLockTime101: TSpinEditEx
                      Tag = 1
                      Left = 161
                      Top = 17
                      Width = 53
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 30
                      OnChange = seWarrContinuousStatusLockTimeChange
                    end
                  end
                end
                object TabSheet74: TTabSheet
                  Caption = #26029#23731#26025
                  ImageIndex = 2
                  object lbl146: TLabel
                    Left = 176
                    Top = 5
                    Width = 84
                    Height = 12
                    Caption = #30452#32447#20260#23475#33539#22260#65306
                  end
                  object GroupBox140: TGroupBox
                    Left = 8
                    Top = 1
                    Width = 161
                    Height = 66
                    Caption = #25915#20987#35774#32622
                    TabOrder = 0
                    object Label296: TLabel
                      Left = 8
                      Top = 20
                      Width = 66
                      Height = 12
                      Caption = #25915#20987#21147#20493#25968':'
                    end
                    object Label297: TLabel
                      Left = 148
                      Top = 20
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label768: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = #24573#35270#38450#24481#20960#29575
                    end
                    object Label769: TLabel
                      Left = 148
                      Top = 44
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousPowerRate102: TSpinEditEx
                      Tag = 2
                      Left = 86
                      Top = 15
                      Width = 57
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                    object edtSkillContinuousCloseDefenseRates102: TSpinEditEx
                      Tag = 2
                      Left = 86
                      Top = 39
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 1
                      TabOrder = 1
                      Value = 100
                      OnChange = edtSkillContinuousCloseDefenseRates100Change
                    end
                  end
                  object GroupBox168: TGroupBox
                    Left = 8
                    Top = 74
                    Width = 161
                    Height = 141
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label496: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label497: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label498: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label499: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label500: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label501: TLabel
                      Left = 148
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label502: TLabel
                      Left = 148
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label503: TLabel
                      Left = 148
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label504: TLabel
                      Left = 148
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label505: TLabel
                      Left = 148
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate102_0: TSpinEditEx
                      Left = 86
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate102_0Change
                    end
                    object EditSkillContinuousBlastHitRate102_1: TSpinEditEx
                      Tag = 1
                      Left = 86
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate102_0Change
                    end
                    object EditSkillContinuousBlastHitRate102_2: TSpinEditEx
                      Tag = 2
                      Left = 86
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate102_0Change
                    end
                    object EditSkillContinuousBlastHitRate102_3: TSpinEditEx
                      Tag = 3
                      Left = 86
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate102_0Change
                    end
                    object EditSkillContinuousBlastHitRate102_4: TSpinEditEx
                      Tag = 4
                      Left = 86
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate102_0Change
                    end
                  end
                  object GroupBox169: TGroupBox
                    Left = 176
                    Top = 74
                    Width = 217
                    Height = 141
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label506: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label507: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label508: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label509: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label510: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label511: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label512: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label513: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label514: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label515: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates102_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates102_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates102_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates102_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates102_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates102_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates102_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates102_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates102_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates102_0Change
                    end
                  end
                  object GroupBox109: TGroupBox
                    Left = 176
                    Top = 24
                    Width = 241
                    Height = 43
                    Caption = #25112#25216#23553#38145#35774#32622
                    TabOrder = 3
                    object Label896: TLabel
                      Left = 8
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #20960#29575#65306
                    end
                    object Label897: TLabel
                      Left = 97
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label898: TLabel
                      Left = 128
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #26102#38388#65306
                    end
                    object Label899: TLabel
                      Left = 217
                      Top = 21
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object seWarrContinuousStatusLock102: TSpinEditEx
                      Tag = 2
                      Left = 41
                      Top = 17
                      Width = 53
                      Height = 21
                      Hint = #35774#32622#34987#25112#22763#36830#20987#25216#33021#25171#20013#30340#29366#24577'('#19981#33021#31227#21160#65292#19981#33021#39764#27861')'#30340#20960#29575#65292#40664#35748#20026'100%'
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 30
                      OnChange = seWarrContinuousStatusLockChange
                    end
                    object seWarrContinuousStatusLockTime102: TSpinEditEx
                      Tag = 2
                      Left = 161
                      Top = 17
                      Width = 53
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 30
                      OnChange = seWarrContinuousStatusLockTimeChange
                    end
                  end
                  object seSkill102AttackRange: TSpinEditEx
                    Tag = 2
                    Left = 257
                    Top = 1
                    Width = 53
                    Height = 21
                    MaxValue = 8
                    MinValue = 1
                    TabOrder = 4
                    Value = 8
                    OnChange = seSkill102AttackRangeChange
                  end
                end
                object TabSheet75: TTabSheet
                  Caption = #27178#25195#21315#20891
                  ImageIndex = 3
                  object Label905: TLabel
                    Left = 176
                    Top = 5
                    Width = 60
                    Height = 12
                    Caption = #25915#20987#33539#22260#65306
                  end
                  object GroupBox141: TGroupBox
                    Left = 8
                    Top = 1
                    Width = 161
                    Height = 66
                    Caption = #25915#20987#35774#32622
                    TabOrder = 0
                    object Label298: TLabel
                      Left = 8
                      Top = 20
                      Width = 66
                      Height = 12
                      Caption = #25915#20987#21147#20493#25968':'
                    end
                    object Label299: TLabel
                      Left = 148
                      Top = 20
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label770: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = #24573#35270#38450#24481#20960#29575
                    end
                    object Label771: TLabel
                      Left = 148
                      Top = 44
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousPowerRate103: TSpinEditEx
                      Tag = 3
                      Left = 86
                      Top = 15
                      Width = 57
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                    object edtSkillContinuousCloseDefenseRates103: TSpinEditEx
                      Tag = 3
                      Left = 86
                      Top = 39
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 1
                      TabOrder = 1
                      Value = 100
                      OnChange = edtSkillContinuousCloseDefenseRates100Change
                    end
                  end
                  object GroupBox166: TGroupBox
                    Left = 8
                    Top = 74
                    Width = 161
                    Height = 141
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label476: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label477: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label478: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label479: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label480: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label481: TLabel
                      Left = 148
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label482: TLabel
                      Left = 148
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label483: TLabel
                      Left = 148
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label484: TLabel
                      Left = 148
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label485: TLabel
                      Left = 148
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate103_0: TSpinEditEx
                      Left = 86
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate103_0Change
                    end
                    object EditSkillContinuousBlastHitRate103_1: TSpinEditEx
                      Tag = 1
                      Left = 86
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate103_0Change
                    end
                    object EditSkillContinuousBlastHitRate103_2: TSpinEditEx
                      Tag = 2
                      Left = 86
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate103_0Change
                    end
                    object EditSkillContinuousBlastHitRate103_3: TSpinEditEx
                      Tag = 3
                      Left = 86
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate103_0Change
                    end
                    object EditSkillContinuousBlastHitRate103_4: TSpinEditEx
                      Tag = 4
                      Left = 86
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate103_0Change
                    end
                  end
                  object GroupBox167: TGroupBox
                    Left = 176
                    Top = 74
                    Width = 217
                    Height = 141
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label486: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label487: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label488: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label489: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label490: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label491: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label492: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label493: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label494: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label495: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates103_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates103_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates103_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates103_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates103_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates103_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates103_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates103_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates103_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates103_0Change
                    end
                  end
                  object GroupBox110: TGroupBox
                    Left = 176
                    Top = 24
                    Width = 241
                    Height = 43
                    Caption = #25112#25216#23553#38145#35774#32622
                    TabOrder = 3
                    object Label900: TLabel
                      Left = 8
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #20960#29575#65306
                    end
                    object Label901: TLabel
                      Left = 97
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label902: TLabel
                      Left = 128
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #26102#38388#65306
                    end
                    object Label903: TLabel
                      Left = 217
                      Top = 21
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object seWarrContinuousStatusLock103: TSpinEditEx
                      Tag = 3
                      Left = 41
                      Top = 17
                      Width = 53
                      Height = 21
                      Hint = #35774#32622#34987#25112#22763#36830#20987#25216#33021#25171#20013#30340#29366#24577'('#19981#33021#31227#21160#65292#19981#33021#39764#27861')'#30340#20960#29575#65292#40664#35748#20026'100%'
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 30
                      OnChange = seWarrContinuousStatusLockChange
                    end
                    object seWarrContinuousStatusLockTime103: TSpinEditEx
                      Tag = 3
                      Left = 161
                      Top = 17
                      Width = 53
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 30
                      OnChange = seWarrContinuousStatusLockTimeChange
                    end
                  end
                  object seSkill103AttackRange: TSpinEditEx
                    Tag = 2
                    Left = 231
                    Top = 1
                    Width = 53
                    Height = 21
                    MaxValue = 8
                    MinValue = 1
                    TabOrder = 4
                    Value = 8
                    OnChange = seSkill103AttackRangeChange
                  end
                end
              end
            end
            object ts26: TTabSheet
              Caption = #27861#24072#36830#20987
              ImageIndex = 4
              object PageControl10: TPageControl
                Left = 0
                Top = 0
                Width = 433
                Height = 267
                ActivePage = TabSheet78
                Align = alClient
                TabOrder = 0
                object TabSheet76: TTabSheet
                  Caption = #20964#33310#31085
                  object GroupBox145: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label306: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label307: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate104: TSpinEditEx
                      Tag = 4
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox164: TGroupBox
                    Left = 8
                    Top = 44
                    Width = 161
                    Height = 145
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label456: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label457: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label458: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label459: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label460: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label461: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label462: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label463: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label464: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label465: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate104_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate104_0Change
                    end
                    object EditSkillContinuousBlastHitRate104_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate104_0Change
                    end
                    object EditSkillContinuousBlastHitRate104_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate104_0Change
                    end
                    object EditSkillContinuousBlastHitRate104_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate104_0Change
                    end
                    object EditSkillContinuousBlastHitRate104_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate104_0Change
                    end
                  end
                  object GroupBox165: TGroupBox
                    Left = 176
                    Top = 44
                    Width = 217
                    Height = 145
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label466: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label467: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label468: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label469: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label470: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label471: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label472: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label473: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label474: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label475: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates104_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates104_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates104_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates104_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates104_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates104_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates104_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates104_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates104_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates104_0Change
                    end
                  end
                end
                object TabSheet77: TTabSheet
                  Caption = #24778#38647#29190
                  ImageIndex = 1
                  object Label906: TLabel
                    Left = 176
                    Top = 5
                    Width = 60
                    Height = 12
                    Caption = #25915#20987#33539#22260#65306
                  end
                  object GroupBox143: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label302: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label303: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate105: TSpinEditEx
                      Tag = 5
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox162: TGroupBox
                    Left = 8
                    Top = 44
                    Width = 161
                    Height = 145
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label436: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label437: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label438: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label439: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label440: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label441: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label442: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label443: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label444: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label445: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate105_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate105_0Change
                    end
                    object EditSkillContinuousBlastHitRate105_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate105_0Change
                    end
                    object EditSkillContinuousBlastHitRate105_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate105_0Change
                    end
                    object EditSkillContinuousBlastHitRate105_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate105_0Change
                    end
                    object EditSkillContinuousBlastHitRate105_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate105_0Change
                    end
                  end
                  object GroupBox163: TGroupBox
                    Left = 176
                    Top = 44
                    Width = 217
                    Height = 145
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label446: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label447: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label448: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label449: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label450: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label451: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label452: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label453: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label454: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label455: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates105_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates105_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates105_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates105_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates105_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates105_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates105_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates105_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates105_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates105_0Change
                    end
                  end
                  object seSkill105AttackRange: TSpinEditEx
                    Tag = 2
                    Left = 231
                    Top = 1
                    Width = 53
                    Height = 21
                    MaxValue = 8
                    MinValue = 1
                    TabOrder = 3
                    Value = 8
                    OnChange = seSkill105AttackRangeChange
                  end
                end
                object TabSheet78: TTabSheet
                  Caption = #20912#22825#38634#22320
                  ImageIndex = 2
                  object Label913: TLabel
                    Left = 426
                    Top = 217
                    Width = 6
                    Height = 12
                    Caption = '%'
                  end
                  object GroupBox144: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label304: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label305: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate106: TSpinEditEx
                      Tag = 6
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox160: TGroupBox
                    Left = 8
                    Top = 76
                    Width = 161
                    Height = 145
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label416: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label417: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label418: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label419: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label420: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label421: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label422: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label423: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label424: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label425: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate106_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate106_0Change
                    end
                    object EditSkillContinuousBlastHitRate106_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate106_0Change
                    end
                    object EditSkillContinuousBlastHitRate106_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate106_0Change
                    end
                    object EditSkillContinuousBlastHitRate106_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate106_0Change
                    end
                    object EditSkillContinuousBlastHitRate106_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate106_0Change
                    end
                  end
                  object GroupBox161: TGroupBox
                    Left = 176
                    Top = 76
                    Width = 217
                    Height = 145
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label426: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label427: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label428: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label429: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label430: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label431: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label432: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label433: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label434: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label435: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates106_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates106_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates106_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates106_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates106_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates106_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates106_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates106_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates106_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates106_0Change
                    end
                  end
                  object GroupBox111: TGroupBox
                    Left = 176
                    Top = 0
                    Width = 241
                    Height = 68
                    Caption = #20912#20923#35774#32622
                    TabOrder = 3
                    object Label908: TLabel
                      Left = 5
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #20960#29575#65306
                    end
                    object Label910: TLabel
                      Left = 85
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label916: TLabel
                      Left = 115
                      Top = 21
                      Width = 60
                      Height = 12
                      Caption = #20960#29575#36882#22686#65306
                    end
                    object Label918: TLabel
                      Left = 220
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label909: TLabel
                      Left = 5
                      Top = 45
                      Width = 36
                      Height = 12
                      Caption = #26102#38388#65306
                    end
                    object Label915: TLabel
                      Left = 82
                      Top = 45
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object Label912: TLabel
                      Left = 115
                      Top = 45
                      Width = 60
                      Height = 12
                      Caption = #26102#38388#36882#22686#65306
                    end
                    object Label911: TLabel
                      Left = 217
                      Top = 45
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object seSkill106AddFrozenRate: TSpinEditEx
                      Tag = 3
                      Left = 37
                      Top = 16
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = seSkill106AddFrozenRateChange
                    end
                    object seSkill106AddFrozenRate2: TSpinEditEx
                      Tag = 3
                      Left = 171
                      Top = 16
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 0
                      OnChange = seSkill106AddFrozenRate2Change
                    end
                    object seSkill106AddFrozenTime: TSpinEditEx
                      Tag = 3
                      Left = 37
                      Top = 40
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = seSkill106AddFrozenTimeChange
                    end
                    object seSkill106AddFrozenTime2: TSpinEditEx
                      Tag = 3
                      Left = 171
                      Top = 40
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = seSkill106AddFrozenTime2Change
                    end
                  end
                end
                object TabSheet79: TTabSheet
                  Caption = #21452#40857#30772
                  ImageIndex = 3
                  object GroupBox142: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label300: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label301: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate107: TSpinEditEx
                      Tag = 7
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox158: TGroupBox
                    Left = 8
                    Top = 44
                    Width = 161
                    Height = 145
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label396: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label397: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label398: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label399: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label400: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label401: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label402: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label403: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label404: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label405: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate107_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate107_0Change
                    end
                    object EditSkillContinuousBlastHitRate107_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate107_0Change
                    end
                    object EditSkillContinuousBlastHitRate107_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate107_0Change
                    end
                    object EditSkillContinuousBlastHitRate107_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate107_0Change
                    end
                    object EditSkillContinuousBlastHitRate107_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate107_0Change
                    end
                  end
                  object GroupBox159: TGroupBox
                    Left = 176
                    Top = 44
                    Width = 217
                    Height = 145
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label406: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label407: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label408: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label409: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label410: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label411: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label412: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label413: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label414: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label415: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates107_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates107_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates107_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates107_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates107_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates107_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates107_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates107_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates107_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates107_0Change
                    end
                  end
                end
              end
            end
            object ts27: TTabSheet
              Caption = #36947#22763#36830#20987
              ImageIndex = 5
              object PageControl11: TPageControl
                Left = 0
                Top = 0
                Width = 433
                Height = 267
                ActivePage = TabSheet83
                Align = alClient
                TabOrder = 0
                object TabSheet80: TTabSheet
                  Caption = #34382#21880#35776
                  object GroupBox149: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label314: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label315: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate108: TSpinEditEx
                      Tag = 8
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox156: TGroupBox
                    Left = 8
                    Top = 44
                    Width = 161
                    Height = 145
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label376: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label377: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label378: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label379: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label380: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label381: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label382: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label383: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label384: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label385: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate108_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate108_0Change
                    end
                    object EditSkillContinuousBlastHitRate108_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate108_0Change
                    end
                    object EditSkillContinuousBlastHitRate108_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate108_0Change
                    end
                    object EditSkillContinuousBlastHitRate108_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate108_0Change
                    end
                    object EditSkillContinuousBlastHitRate108_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate108_0Change
                    end
                  end
                  object GroupBox157: TGroupBox
                    Left = 176
                    Top = 44
                    Width = 217
                    Height = 145
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label386: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label387: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label388: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label389: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label390: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label391: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label392: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label393: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label394: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label395: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates108_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates108_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates108_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates108_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates108_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates108_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates108_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates108_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates108_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates108_0Change
                    end
                  end
                end
                object TabSheet81: TTabSheet
                  Caption = #20843#21350#25484
                  ImageIndex = 1
                  object GroupBox148: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label312: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label313: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate109: TSpinEditEx
                      Tag = 9
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox154: TGroupBox
                    Left = 8
                    Top = 44
                    Width = 161
                    Height = 145
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label356: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label357: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label358: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label359: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label360: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label361: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label362: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label363: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label364: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label365: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate109_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate109_0Change
                    end
                    object EditSkillContinuousBlastHitRate109_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate109_0Change
                    end
                    object EditSkillContinuousBlastHitRate109_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate109_0Change
                    end
                    object EditSkillContinuousBlastHitRate109_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate109_0Change
                    end
                    object EditSkillContinuousBlastHitRate109_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate109_0Change
                    end
                  end
                  object GroupBox155: TGroupBox
                    Left = 176
                    Top = 44
                    Width = 217
                    Height = 145
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label366: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label367: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label368: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label369: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label370: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label371: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label372: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label373: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label374: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label375: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates109_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates109_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates109_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates109_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates109_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates109_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates109_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates109_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates109_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates109_0Change
                    end
                  end
                end
                object TabSheet82: TTabSheet
                  Caption = #19977#28976#21650
                  ImageIndex = 2
                  object GroupBox147: TGroupBox
                    Left = 8
                    Top = 0
                    Width = 161
                    Height = 41
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label310: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label311: TLabel
                      Left = 128
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate110: TSpinEditEx
                      Tag = 10
                      Left = 44
                      Top = 15
                      Width = 82
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox152: TGroupBox
                    Left = 8
                    Top = 100
                    Width = 161
                    Height = 141
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label336: TLabel
                      Left = 8
                      Top = 19
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label337: TLabel
                      Left = 8
                      Top = 44
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label338: TLabel
                      Left = 8
                      Top = 68
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label339: TLabel
                      Left = 8
                      Top = 93
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label340: TLabel
                      Left = 8
                      Top = 116
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label341: TLabel
                      Left = 142
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label342: TLabel
                      Left = 142
                      Top = 45
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label343: TLabel
                      Left = 142
                      Top = 69
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label344: TLabel
                      Left = 142
                      Top = 93
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label345: TLabel
                      Left = 142
                      Top = 117
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate110_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate110_0Change
                    end
                    object EditSkillContinuousBlastHitRate110_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate110_0Change
                    end
                    object EditSkillContinuousBlastHitRate110_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate110_0Change
                    end
                    object EditSkillContinuousBlastHitRate110_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate110_0Change
                    end
                    object EditSkillContinuousBlastHitRate110_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate110_0Change
                    end
                  end
                  object GroupBox153: TGroupBox
                    Left = 176
                    Top = 100
                    Width = 217
                    Height = 141
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label346: TLabel
                      Left = 8
                      Top = 19
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label347: TLabel
                      Left = 8
                      Top = 44
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label348: TLabel
                      Left = 8
                      Top = 68
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label349: TLabel
                      Left = 8
                      Top = 93
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label350: TLabel
                      Left = 8
                      Top = 116
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label351: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label352: TLabel
                      Left = 158
                      Top = 45
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label353: TLabel
                      Left = 158
                      Top = 69
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label354: TLabel
                      Left = 158
                      Top = 93
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label355: TLabel
                      Left = 158
                      Top = 117
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates110_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates110_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates110_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 40
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates110_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates110_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 64
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates110_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates110_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 88
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates110_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates110_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 112
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates110_0Change
                    end
                  end
                  object GroupBox114: TGroupBox
                    Left = 176
                    Top = 0
                    Width = 241
                    Height = 89
                    Caption = #25512#21160#30446#26631#35774#32622
                    TabOrder = 3
                    object Label925: TLabel
                      Left = 5
                      Top = 21
                      Width = 36
                      Height = 12
                      Caption = #20960#29575#65306
                    end
                    object Label926: TLabel
                      Left = 85
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label927: TLabel
                      Left = 115
                      Top = 21
                      Width = 60
                      Height = 12
                      Caption = #20960#29575#36882#22686#65306
                    end
                    object Label928: TLabel
                      Left = 220
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label929: TLabel
                      Left = 5
                      Top = 45
                      Width = 36
                      Height = 12
                      Caption = #26684#25968#65306
                    end
                    object Label930: TLabel
                      Left = 82
                      Top = 45
                      Width = 12
                      Height = 12
                      Caption = #26684
                    end
                    object Label931: TLabel
                      Left = 115
                      Top = 45
                      Width = 60
                      Height = 12
                      Caption = #26684#25968#36882#22686#65306
                    end
                    object Label932: TLabel
                      Left = 217
                      Top = 45
                      Width = 12
                      Height = 12
                      Caption = #26684
                    end
                    object seSkill110PushedRate: TSpinEditEx
                      Tag = 3
                      Left = 37
                      Top = 16
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 0
                      OnChange = seSkill110PushedRateChange
                    end
                    object seSkill110PushedRate2: TSpinEditEx
                      Tag = 3
                      Left = 171
                      Top = 16
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 0
                      OnChange = seSkill110PushedRate2Change
                    end
                    object seSkill110PushedRange: TSpinEditEx
                      Tag = 3
                      Left = 37
                      Top = 40
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = seSkill110PushedRangeChange
                    end
                    object seSkill110PushedRange2: TSpinEditEx
                      Tag = 3
                      Left = 171
                      Top = 40
                      Width = 44
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 0
                      OnChange = seSkill110PushedRange2Change
                    end
                    object chkSkill110PushedHighLevel: TCheckBox
                      Left = 9
                      Top = 63
                      Width = 72
                      Height = 18
                      Caption = #25512#39640#31561#32423
                      TabOrder = 4
                      OnClick = chkSkill110PushedHighLevelClick
                    end
                  end
                end
                object TabSheet83: TTabSheet
                  Caption = #19975#21073#24402#23447
                  ImageIndex = 3
                  object GroupBox146: TGroupBox
                    Left = 3
                    Top = 0
                    Width = 146
                    Height = 43
                    Caption = #25915#20987#21147#20493#25968
                    TabOrder = 0
                    object Label308: TLabel
                      Left = 8
                      Top = 20
                      Width = 30
                      Height = 12
                      Caption = #20493#25968':'
                    end
                    object Label309: TLabel
                      Left = 117
                      Top = 20
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousPowerRate111: TSpinEditEx
                      Tag = 11
                      Left = 39
                      Top = 15
                      Width = 78
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 100000
                      MinValue = 1
                      TabOrder = 0
                      Value = 100
                      OnChange = EditSkillContinuousPowerRate100Change
                    end
                  end
                  object GroupBox150: TGroupBox
                    Left = 3
                    Top = 63
                    Width = 146
                    Height = 133
                    Caption = #26292#20987#20960#29575
                    TabOrder = 1
                    object Label316: TLabel
                      Left = 8
                      Top = 20
                      Width = 72
                      Height = 12
                      Caption = 'Lev.1 '#26292#20987#29575
                    end
                    object Label317: TLabel
                      Left = 8
                      Top = 43
                      Width = 72
                      Height = 12
                      Caption = 'Lev.2 '#26292#20987#29575
                    end
                    object Label318: TLabel
                      Left = 8
                      Top = 66
                      Width = 72
                      Height = 12
                      Caption = 'Lev.3 '#26292#20987#29575
                    end
                    object Label319: TLabel
                      Left = 8
                      Top = 90
                      Width = 72
                      Height = 12
                      Caption = 'Lev.4 '#26292#20987#29575
                    end
                    object Label320: TLabel
                      Left = 8
                      Top = 113
                      Width = 72
                      Height = 12
                      Caption = 'Lev.5 '#26292#20987#29575
                    end
                    object Label321: TLabel
                      Left = 135
                      Top = 21
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label322: TLabel
                      Left = 135
                      Top = 44
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label323: TLabel
                      Left = 135
                      Top = 67
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label324: TLabel
                      Left = 135
                      Top = 90
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object Label325: TLabel
                      Left = 135
                      Top = 113
                      Width = 6
                      Height = 12
                      Caption = '%'
                    end
                    object EditSkillContinuousBlastHitRate111_0: TSpinEditEx
                      Left = 80
                      Top = 16
                      Width = 52
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 0
                      Value = 5
                      OnChange = EditSkillContinuousBlastHitRate111_0Change
                    end
                    object EditSkillContinuousBlastHitRate111_1: TSpinEditEx
                      Tag = 1
                      Left = 80
                      Top = 39
                      Width = 52
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 1
                      Value = 10
                      OnChange = EditSkillContinuousBlastHitRate111_0Change
                    end
                    object EditSkillContinuousBlastHitRate111_2: TSpinEditEx
                      Tag = 2
                      Left = 80
                      Top = 62
                      Width = 52
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 2
                      Value = 20
                      OnChange = EditSkillContinuousBlastHitRate111_0Change
                    end
                    object EditSkillContinuousBlastHitRate111_3: TSpinEditEx
                      Tag = 3
                      Left = 80
                      Top = 85
                      Width = 52
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 3
                      Value = 30
                      OnChange = EditSkillContinuousBlastHitRate111_0Change
                    end
                    object EditSkillContinuousBlastHitRate111_4: TSpinEditEx
                      Tag = 4
                      Left = 80
                      Top = 108
                      Width = 52
                      Height = 21
                      MaxValue = 100
                      MinValue = 0
                      TabOrder = 4
                      Value = 50
                      OnChange = EditSkillContinuousBlastHitRate111_0Change
                    end
                  end
                  object GroupBox151: TGroupBox
                    Left = 157
                    Top = 63
                    Width = 257
                    Height = 133
                    Caption = #26292#20987#20260#23475#29575
                    TabOrder = 2
                    object Label326: TLabel
                      Left = 8
                      Top = 20
                      Width = 84
                      Height = 12
                      Caption = 'Lev.1'#26292#20987#20260#23475':'
                    end
                    object Label327: TLabel
                      Left = 8
                      Top = 43
                      Width = 84
                      Height = 12
                      Caption = 'Lev.2'#26292#20987#20260#23475':'
                    end
                    object Label328: TLabel
                      Left = 8
                      Top = 66
                      Width = 84
                      Height = 12
                      Caption = 'Lev.3'#26292#20987#20260#23475':'
                    end
                    object Label329: TLabel
                      Left = 8
                      Top = 90
                      Width = 84
                      Height = 12
                      Caption = 'Lev.4'#26292#20987#20260#23475':'
                    end
                    object Label330: TLabel
                      Left = 8
                      Top = 113
                      Width = 84
                      Height = 12
                      Caption = 'Lev.5'#26292#20987#20260#23475';'
                    end
                    object Label331: TLabel
                      Left = 158
                      Top = 21
                      Width = 24
                      Height = 12
                      Caption = '/100'
                      Font.Charset = GB2312_CHARSET
                      Font.Color = clBlack
                      Font.Height = -12
                      Font.Name = #23435#20307
                      Font.Style = []
                      ParentFont = False
                    end
                    object Label332: TLabel
                      Left = 158
                      Top = 44
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label333: TLabel
                      Left = 158
                      Top = 67
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label334: TLabel
                      Left = 158
                      Top = 90
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object Label335: TLabel
                      Left = 158
                      Top = 113
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object EditSkillContinuousBlastHitPowerRates111_0: TSpinEditEx
                      Left = 96
                      Top = 16
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 0
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates111_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates111_1: TSpinEditEx
                      Tag = 1
                      Left = 96
                      Top = 39
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 1
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates111_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates111_2: TSpinEditEx
                      Tag = 2
                      Left = 96
                      Top = 62
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates111_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates111_3: TSpinEditEx
                      Tag = 3
                      Left = 96
                      Top = 85
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 3
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates111_0Change
                    end
                    object EditSkillContinuousBlastHitPowerRates111_4: TSpinEditEx
                      Tag = 4
                      Left = 96
                      Top = 108
                      Width = 57
                      Height = 21
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 4
                      Value = 150
                      OnChange = EditSkillContinuousBlastHitPowerRates111_0Change
                    end
                  end
                  object grp47: TGroupBox
                    Left = 157
                    Top = 0
                    Width = 257
                    Height = 61
                    Caption = #27602#28895#35774#32622
                    TabOrder = 3
                    object lbl105: TLabel
                      Left = 75
                      Top = 19
                      Width = 24
                      Height = 12
                      Caption = #31561#32423
                    end
                    object lbl106: TLabel
                      Left = 155
                      Top = 19
                      Width = 24
                      Height = 12
                      Caption = #26102#38388
                    end
                    object lbl107: TLabel
                      Left = 231
                      Top = 19
                      Width = 12
                      Height = 12
                      Caption = #31186
                    end
                    object Label739: TLabel
                      Left = 8
                      Top = 42
                      Width = 84
                      Height = 12
                      Caption = #25345#32487#25481#34880#27604#20363#65306
                    end
                    object Label742: TLabel
                      Left = 137
                      Top = 42
                      Width = 24
                      Height = 12
                      Caption = '/100'
                    end
                    object chkToxicSmoke: TCheckBox
                      Left = 8
                      Top = 17
                      Width = 61
                      Height = 17
                      Hint = #20013#27602#28895#26399#38388#65292#30446#26631#33258#21160#24674#22797#20307#21147#22833#25928
                      Caption = #20013#27602#28895
                      TabOrder = 0
                      OnClick = chkToxicSmokeClick
                    end
                    object cbbSkillContinuousPowerRate111: TComboBox
                      Left = 102
                      Top = 15
                      Width = 49
                      Height = 20
                      Hint = #25216#33021#31561#32423
                      Style = csDropDownList
                      ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                      ItemIndex = 0
                      TabOrder = 1
                      Text = '0'#32423
                      OnChange = cbbSkillContinuousPowerRate111Change
                      Items.Strings = (
                        '0'#32423
                        '1'#32423
                        '2'#32423
                        '3'#32423
                        '4'#32423
                        '5'#32423)
                    end
                    object seToxicSmokeTime: TSpinEditEx
                      Left = 182
                      Top = 15
                      Width = 46
                      Height = 21
                      Hint = #27602#28895#25345#32493#26102#38388#65292#25968#23383#36234#22823#65292#27602#28895#25345#32493#26102#38388#36234#38271
                      MaxValue = 0
                      MinValue = 0
                      TabOrder = 2
                      Value = 0
                      OnChange = seToxicSmokeTimeChange
                    end
                    object seToxicSmokeDecHPRate: TSpinEditEx
                      Tag = 11
                      Left = 88
                      Top = 37
                      Width = 45
                      Height = 21
                      Hint = #25915#20987#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                      MaxValue = 1000
                      MinValue = 1
                      TabOrder = 3
                      Value = 100
                      OnChange = seToxicSmokeDecHPRateChange
                    end
                  end
                end
              end
            end
          end
        end
        object ts8: TTabSheet
          Caption = #24378#21270#25216#33021
          ImageIndex = 8
          object pgc1: TPageControl
            Left = 0
            Top = 0
            Width = 441
            Height = 295
            ActivePage = ts11
            Align = alClient
            TabOrder = 0
            object ts9: TTabSheet
              Caption = #25112#22763
              object grp17: TGroupBox
                Left = 5
                Top = 8
                Width = 136
                Height = 73
                Caption = #22522#26412#21073#26415
                TabOrder = 0
                object lbl52: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl53: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower0: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower0Change
                end
                object cbbMagicNewLevel0: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel0Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp18: TGroupBox
                Left = 5
                Top = 88
                Width = 136
                Height = 73
                Caption = #25915#26432#21073#26415
                TabOrder = 2
                object lbl54: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl55: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower1: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower1Change
                end
                object cbbMagicNewLevel1: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel1Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp19: TGroupBox
                Left = 5
                Top = 168
                Width = 136
                Height = 73
                Caption = #21050#26432#21073#26415
                TabOrder = 4
                object lbl56: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl57: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower2: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower2Change
                end
                object cbbMagicNewLevel2: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel2Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp20: TGroupBox
                Left = 148
                Top = 8
                Width = 136
                Height = 73
                Caption = #21322#26376#24367#20992
                TabOrder = 1
                object lbl58: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl59: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower3: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower3Change
                end
                object cbbMagicNewLevel3: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel3Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp21: TGroupBox
                Left = 148
                Top = 88
                Width = 136
                Height = 73
                Caption = #28872#28779#21073#27861
                TabOrder = 3
                object lbl60: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl61: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower4: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower4Change
                end
                object cbbMagicNewLevel4: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel4Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp22: TGroupBox
                Left = 148
                Top = 168
                Width = 136
                Height = 73
                Caption = #36880#26085#21073#27861
                TabOrder = 5
                object lbl62: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl63: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower5: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower5Change
                end
                object cbbMagicNewLevel5: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel5Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
            end
            object ts10: TTabSheet
              Caption = #27861#24072
              ImageIndex = 1
              object grp23: TGroupBox
                Left = 5
                Top = 8
                Width = 136
                Height = 73
                Caption = #27969#26143#28779#38632
                TabOrder = 0
                object lbl64: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl65: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower20: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower20Change
                end
                object cbbMagicNewLevel20: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel20Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp24: TGroupBox
                Left = 5
                Top = 88
                Width = 136
                Height = 73
                Caption = #28779#22681
                TabOrder = 2
                object lbl66: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl67: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower21: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower21Change
                end
                object cbbMagicNewLevel21: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel21Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp25: TGroupBox
                Left = 5
                Top = 168
                Width = 136
                Height = 73
                Caption = #39764#27861#30462
                TabOrder = 4
                Visible = False
                object lbl68: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl69: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower22: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower22Change
                end
                object cbbMagicNewLevel22: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel22Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp26: TGroupBox
                Left = 148
                Top = 8
                Width = 136
                Height = 73
                Caption = #38647#30005#26415
                TabOrder = 1
                object lbl70: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl71: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower23: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower23Change
                end
                object cbbMagicNewLevel23: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel23Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp27: TGroupBox
                Left = 148
                Top = 88
                Width = 136
                Height = 73
                Caption = #28781#22825#28779
                TabOrder = 3
                object lbl72: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl73: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower24: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower24Change
                end
                object cbbMagicNewLevel24: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel24Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object GroupBox212: TGroupBox
                Left = 148
                Top = 168
                Width = 136
                Height = 73
                Caption = #20912#21638#21742
                TabOrder = 5
                object Label678: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label679: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower25: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower25Change
                end
                object cbbMagicNewLevel25: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel25Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
            end
            object ts11: TTabSheet
              Caption = #36947#22763
              ImageIndex = 2
              object grp28: TGroupBox
                Left = 5
                Top = 8
                Width = 136
                Height = 73
                Caption = #26045#27602#26415'/'#32676#20307#26045#27602#26415
                TabOrder = 0
                object lbl74: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl75: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower40: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower40Change
                end
                object cbbMagicNewLevel40: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel40Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp29: TGroupBox
                Left = 5
                Top = 88
                Width = 136
                Height = 73
                Caption = #24189#28789#30462
                TabOrder = 2
                object lbl76: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl77: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower41: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower41Change
                end
                object cbbMagicNewLevel41: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel41Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp30: TGroupBox
                Left = 5
                Top = 168
                Width = 136
                Height = 73
                Caption = #31070#22307#25112#30002#26415
                TabOrder = 4
                object lbl78: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl79: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower42: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower42Change
                end
                object cbbMagicNewLevel42: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel42Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp31: TGroupBox
                Left = 148
                Top = 8
                Width = 205
                Height = 73
                Caption = #28789#39746#28779#31526
                TabOrder = 1
                object lbl80: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl81: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object lbl35: TLabel
                  Left = 135
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #35010#31070#31526#20960#29575
                end
                object seMagicNewLevelPower43: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower43Change
                end
                object cbbMagicNewLevel43: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel43Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
                object seNewLevelMagic43LSFRate: TSpinEditEx
                  Left = 135
                  Top = 39
                  Width = 61
                  Height = 21
                  Hint = #35010#31070#31526#20960#29575#65292#25968#23383#36234#23567#65292#20960#29575#36234#22823#65307'0'#34920#31034#26080#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seNewLevelMagic43LSFRateChange
                end
              end
              object grp32: TGroupBox
                Left = 148
                Top = 88
                Width = 136
                Height = 73
                Caption = #22124#34880#26415
                TabOrder = 3
                object lbl82: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl83: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower44: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower44Change
                end
                object cbbMagicNewLevel44: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel44Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object grp33: TGroupBox
                Left = 148
                Top = 168
                Width = 136
                Height = 73
                Caption = #39123#39118#30772
                TabOrder = 5
                object lbl84: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object lbl85: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seMagicNewLevelPower45: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPower45Change
                end
                object cbbMagicNewLevel45: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevel45Change
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
              object GroupBox189: TGroupBox
                Left = 292
                Top = 88
                Width = 136
                Height = 73
                Caption = #21484#21796#25216#33021#23453#23453#23646#24615#21472#21152
                TabOrder = 6
                object Label714: TLabel
                  Left = 8
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23646#24615#20493#29575':'
                end
                object Label715: TLabel
                  Left = 8
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object seBBAttrPlusNewLevelRate: TSpinEditEx
                  Left = 64
                  Top = 40
                  Width = 66
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seBBAttrPlusNewLevelRateChange
                end
                object cbbBBAttrPlusNewLevel: TComboBox
                  Left = 64
                  Top = 16
                  Width = 66
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbBBAttrPlusNewLevelChange
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
              end
            end
            object ts14: TTabSheet
              Caption = #38468#21152
              ImageIndex = 3
              object GroupBox82: TGroupBox
                Left = 5
                Top = 8
                Width = 185
                Height = 97
                Hint = #38750#25351#23450#24378#21270#25216#33021#38656#35201#24378#21270#23041#21147#65292#35831#22312#27492#35774#32622#36890#29992#20110#20854#20182#26410#25351#23450#20219#20309#25216#33021
                Caption = #20854#23427#25216#33021#24378#21270#23041#21147#20493#25968
                TabOrder = 0
                object Label161: TLabel
                  Left = 38
                  Top = 44
                  Width = 54
                  Height = 12
                  Caption = #23041#21147#20493#25968':'
                end
                object Label185: TLabel
                  Left = 38
                  Top = 20
                  Width = 54
                  Height = 12
                  Caption = #24378#21270#31561#32423':'
                end
                object lbl91: TLabel
                  Left = 8
                  Top = 69
                  Width = 84
                  Height = 12
                  Caption = '9'#37325#21518#23041#21147#20493#25968':'
                end
                object seMagicNewLevelPower: TSpinEditEx
                  Left = 96
                  Top = 40
                  Width = 79
                  Height = 21
                  Hint = #23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 1
                  Value = 100
                  OnChange = seMagicNewLevelPowerChange
                end
                object cbbMagicNewLevel: TComboBox
                  Left = 96
                  Top = 16
                  Width = 79
                  Height = 20
                  Style = csDropDownList
                  ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
                  TabOrder = 0
                  OnChange = cbbMagicNewLevelChange
                  Items.Strings = (
                    #24378#21270#19968#37325
                    #24378#21270#20108#37325
                    #24378#21270#19977#37325
                    #24378#21270#22235#37325
                    #24378#21270#20116#37325
                    #24378#21270#20845#37325
                    #24378#21270#19971#37325
                    #24378#21270#20843#37325
                    #24378#21270#20061#37325)
                end
                object seNewLevelMagicPowerRatesAfter9: TSpinEditEx
                  Left = 96
                  Top = 64
                  Width = 79
                  Height = 21
                  Hint = #20061#37325#20197#21518#27599#37325#22686#21152#23041#21147#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
                  MaxValue = 100000
                  MinValue = 1
                  TabOrder = 2
                  Value = 100
                  OnChange = seNewLevelMagicPowerRatesAfter9Change
                end
              end
            end
          end
        end
        object ts21: TTabSheet
          Caption = #25216#33021#30772#38450'/'#39764#27861#30462
          ImageIndex = 9
          object lbl123: TLabel
            Left = 164
            Top = 268
            Width = 240
            Height = 24
            Caption = #39764#27861#25216#33021#35270#30446#26631#39764#24481#20915#23450#20260#23475#65292#13#10#26412#36523#26080#35270#38450#24481#19981#24517#35774#32622#39764#27861#25915#20987#25216#33021#30772#38450#27604#65281
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object Label239: TLabel
            Left = 164
            Top = 248
            Width = 252
            Height = 12
            Caption = #19978#38754#30772#30462#20026#24573#35270#39764#27861#30462#38450#24481#27604#20363#65292#38750#25171#30772#39764#27861#30462
            Font.Charset = GB2312_CHARSET
            Font.Color = clBlue
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object grp67: TGroupBox
            Left = 4
            Top = 4
            Width = 153
            Height = 289
            Caption = #25216#33021#21015#34920
            TabOrder = 0
            object lstMagicAC: TListBox
              Left = 8
              Top = 16
              Width = 137
              Height = 267
              ItemHeight = 12
              TabOrder = 0
              OnClick = lstMagicACClick
            end
          end
          object grp68: TGroupBox
            Left = 162
            Top = 4
            Width = 151
            Height = 181
            TabOrder = 1
            object lbl122: TLabel
              Left = 10
              Top = 35
              Width = 72
              Height = 12
              Caption = #23545#20154#29289#30772#38450#65306
            end
            object Label775: TLabel
              Left = 10
              Top = 59
              Width = 72
              Height = 12
              Caption = #23545#24618#29289#30772#38450#65306
            end
            object Label776: TLabel
              Left = 135
              Top = 35
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label777: TLabel
              Left = 135
              Top = 59
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label778: TLabel
              Left = 10
              Top = 83
              Width = 72
              Height = 12
              Caption = #23545#33521#38596#30772#38450#65306
            end
            object Label779: TLabel
              Left = 135
              Top = 83
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label233: TLabel
              Left = 10
              Top = 107
              Width = 72
              Height = 12
              Caption = #23545#20154#29289#30772#30462#65306
            end
            object Label234: TLabel
              Left = 10
              Top = 131
              Width = 72
              Height = 12
              Caption = #23545#24618#29289#30772#30462#65306
            end
            object Label235: TLabel
              Left = 135
              Top = 107
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label236: TLabel
              Left = 135
              Top = 131
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label237: TLabel
              Left = 10
              Top = 155
              Width = 72
              Height = 12
              Caption = #23545#33521#38596#30772#30462#65306
            end
            object Label238: TLabel
              Left = 135
              Top = 155
              Width = 6
              Height = 12
              Caption = '%'
            end
            object chkMagicACEnabled: TCheckBox
              Left = 10
              Top = 12
              Width = 49
              Height = 17
              Caption = #24320#21551
              TabOrder = 0
              OnClick = chkMagicACEnabledClick
            end
            object seMagicACHum: TSpinEditEx
              Left = 79
              Top = 30
              Width = 52
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seMagicACHumChange
            end
            object seMagicACMon: TSpinEditEx
              Left = 79
              Top = 54
              Width = 52
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seMagicACMonChange
            end
            object seMagicACHero: TSpinEditEx
              Left = 79
              Top = 78
              Width = 52
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seMagicACHeroChange
            end
            object seDefenceHum: TSpinEditEx
              Left = 79
              Top = 102
              Width = 52
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = seDefenceHumChange
            end
            object seDefenceMon: TSpinEditEx
              Left = 79
              Top = 126
              Width = 52
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 5
              Value = 0
              OnChange = seDefenceMonChange
            end
            object seDefenceHero: TSpinEditEx
              Left = 79
              Top = 150
              Width = 52
              Height = 21
              MaxValue = 100
              MinValue = 0
              TabOrder = 6
              Value = 0
              OnChange = seDefenceHeroChange
            end
          end
        end
      end
    end
    object TabSheet34: TTabSheet
      Caption = #21319#32423#27494#22120
      ImageIndex = 6
      object GroupBox8: TGroupBox
        Left = 8
        Top = 8
        Width = 161
        Height = 145
        Caption = #22522#26412#35774#32622
        TabOrder = 0
        object Label13: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #26368#39640#28857#25968':'
        end
        object Label15: TLabel
          Left = 8
          Top = 42
          Width = 54
          Height = 12
          Caption = #25152#38656#36153#29992':'
        end
        object Label16: TLabel
          Left = 8
          Top = 66
          Width = 54
          Height = 12
          Caption = #25152#38656#26102#38388':'
        end
        object Label17: TLabel
          Left = 8
          Top = 90
          Width = 54
          Height = 12
          Caption = #36807#26399#26102#38388':'
        end
        object Label18: TLabel
          Left = 136
          Top = 65
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label19: TLabel
          Left = 136
          Top = 89
          Width = 12
          Height = 12
          Caption = #22825
        end
        object EditUpgradeWeaponMaxPoint: TSpinEditEx
          Left = 68
          Top = 15
          Width = 61
          Height = 21
          MaxValue = 1000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditUpgradeWeaponMaxPointChange
        end
        object EditUpgradeWeaponPrice: TSpinEditEx
          Left = 68
          Top = 39
          Width = 61
          Height = 21
          MaxValue = 1000000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditUpgradeWeaponPriceChange
        end
        object EditUPgradeWeaponGetBackTime: TSpinEditEx
          Left = 68
          Top = 63
          Width = 61
          Height = 21
          MaxValue = 36000000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditUPgradeWeaponGetBackTimeChange
        end
        object EditClearExpireUpgradeWeaponDays: TSpinEditEx
          Left = 68
          Top = 87
          Width = 61
          Height = 21
          MaxValue = 100
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = EditClearExpireUpgradeWeaponDaysChange
        end
        object CheckBoxWeaponUpgradeFailNotDelete: TCheckBox
          Left = 8
          Top = 112
          Width = 137
          Height = 17
          Caption = #27494#22120#21319#32423#22833#36133#19981#30772#30862
          TabOrder = 4
          OnClick = CheckBoxWeaponUpgradeFailNotDeleteClick
        end
      end
      object GroupBox18: TGroupBox
        Left = 176
        Top = 8
        Width = 265
        Height = 89
        Caption = #25915#20987#21147#21319#32423
        TabOrder = 1
        object Label20: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #25104#21151#26426#29575':'
        end
        object Label21: TLabel
          Left = 8
          Top = 42
          Width = 54
          Height = 12
          Caption = #20108#28857#26426#29575':'
        end
        object Label22: TLabel
          Left = 8
          Top = 66
          Width = 54
          Height = 12
          Caption = #19977#28857#26426#29575':'
        end
        object ScrollBarUpgradeWeaponDCRate: TScrollBar
          Left = 64
          Top = 16
          Width = 145
          Height = 17
          Hint = #21319#32423#25915#20987#21147#28857#25968#25104#21151#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarUpgradeWeaponDCRateChange
        end
        object EditUpgradeWeaponDCRate: TEdit
          Left = 216
          Top = 16
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarUpgradeWeaponDCTwoPointRate: TScrollBar
          Left = 64
          Top = 40
          Width = 145
          Height = 17
          Hint = #24471#21040#20108#28857#23646#24615#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarUpgradeWeaponDCTwoPointRateChange
        end
        object EditUpgradeWeaponDCTwoPointRate: TEdit
          Left = 216
          Top = 40
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
        object ScrollBarUpgradeWeaponDCThreePointRate: TScrollBar
          Left = 64
          Top = 64
          Width = 145
          Height = 17
          Hint = #24471#21040#19977#28857#23646#24615#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 4
          OnChange = ScrollBarUpgradeWeaponDCThreePointRateChange
        end
        object EditUpgradeWeaponDCThreePointRate: TEdit
          Left = 216
          Top = 64
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 5
        end
      end
      object GroupBox19: TGroupBox
        Left = 176
        Top = 104
        Width = 265
        Height = 97
        Caption = #36947#26415#21319#32423
        TabOrder = 2
        object Label23: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #25104#21151#26426#29575':'
        end
        object Label24: TLabel
          Left = 8
          Top = 42
          Width = 54
          Height = 12
          Caption = #20108#28857#26426#29575':'
        end
        object Label25: TLabel
          Left = 8
          Top = 66
          Width = 54
          Height = 12
          Caption = #19977#28857#26426#29575':'
        end
        object ScrollBarUpgradeWeaponSCRate: TScrollBar
          Left = 64
          Top = 16
          Width = 145
          Height = 17
          Hint = #21319#32423#36947#26415#28857#25968#25104#21151#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarUpgradeWeaponSCRateChange
        end
        object EditUpgradeWeaponSCRate: TEdit
          Left = 216
          Top = 16
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarUpgradeWeaponSCTwoPointRate: TScrollBar
          Left = 64
          Top = 40
          Width = 145
          Height = 17
          Hint = #24471#21040#20108#28857#23646#24615#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarUpgradeWeaponSCTwoPointRateChange
        end
        object EditUpgradeWeaponSCTwoPointRate: TEdit
          Left = 216
          Top = 40
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
        object ScrollBarUpgradeWeaponSCThreePointRate: TScrollBar
          Left = 64
          Top = 64
          Width = 145
          Height = 17
          Hint = #24471#21040#19977#28857#23646#24615#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 4
          OnChange = ScrollBarUpgradeWeaponSCThreePointRateChange
        end
        object EditUpgradeWeaponSCThreePointRate: TEdit
          Left = 216
          Top = 64
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 5
        end
      end
      object GroupBox20: TGroupBox
        Left = 176
        Top = 208
        Width = 265
        Height = 89
        Caption = #39764#27861#21319#32423
        TabOrder = 3
        object Label26: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #25104#21151#26426#29575':'
        end
        object Label27: TLabel
          Left = 8
          Top = 42
          Width = 54
          Height = 12
          Caption = #20108#28857#26426#29575':'
        end
        object Label28: TLabel
          Left = 8
          Top = 66
          Width = 54
          Height = 12
          Caption = #19977#28857#26426#29575':'
        end
        object ScrollBarUpgradeWeaponMCRate: TScrollBar
          Left = 64
          Top = 16
          Width = 145
          Height = 17
          Hint = #21319#32423#39764#27861#28857#25968#25104#21151#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarUpgradeWeaponMCRateChange
        end
        object EditUpgradeWeaponMCRate: TEdit
          Left = 216
          Top = 16
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarUpgradeWeaponMCTwoPointRate: TScrollBar
          Left = 64
          Top = 40
          Width = 145
          Height = 17
          Hint = #24471#21040#20108#28857#23646#24615#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarUpgradeWeaponMCTwoPointRateChange
        end
        object EditUpgradeWeaponMCTwoPointRate: TEdit
          Left = 216
          Top = 40
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
        object ScrollBarUpgradeWeaponMCThreePointRate: TScrollBar
          Left = 64
          Top = 64
          Width = 145
          Height = 17
          Hint = #24471#21040#19977#28857#23646#24615#26426#29575#65292#26426#29575#20026#24038#22823#21491#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 4
          OnChange = ScrollBarUpgradeWeaponMCThreePointRateChange
        end
        object EditUpgradeWeaponMCThreePointRate: TEdit
          Left = 216
          Top = 64
          Width = 41
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 5
        end
      end
      object ButtonUpgradeWeaponSave: TButton
        Left = 8
        Top = 277
        Width = 65
        Height = 20
        Caption = #20445#23384'(&S)'
        TabOrder = 4
        OnClick = ButtonUpgradeWeaponSaveClick
      end
      object ButtonUpgradeWeaponDefaulf: TButton
        Left = 80
        Top = 277
        Width = 65
        Height = 20
        Caption = #40664#35748'(&D)'
        TabOrder = 5
        OnClick = ButtonUpgradeWeaponDefaulfClick
      end
    end
    object TabSheet35: TTabSheet
      Caption = #25366#30719#25511#21046
      ImageIndex = 7
      object GroupBox24: TGroupBox
        Left = 8
        Top = 8
        Width = 273
        Height = 60
        Caption = #24471#21040#30719#30707#26426#29575
        TabOrder = 0
        object Label32: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #21629#20013#26426#29575':'
        end
        object Label33: TLabel
          Left = 8
          Top = 36
          Width = 54
          Height = 12
          Caption = #25366#30719#26426#29575':'
        end
        object ScrollBarMakeMineHitRate: TScrollBar
          Left = 72
          Top = 16
          Width = 129
          Height = 15
          Hint = #35774#32622#30340#25968#23383#36234#23567#26426#29575#36234#22823#12290
          Max = 500
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarMakeMineHitRateChange
        end
        object EditMakeMineHitRate: TEdit
          Left = 208
          Top = 16
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarMakeMineRate: TScrollBar
          Left = 72
          Top = 36
          Width = 129
          Height = 15
          Hint = #35774#32622#30340#25968#23383#36234#23567#26426#29575#36234#22823#12290
          Max = 500
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarMakeMineRateChange
        end
        object EditMakeMineRate: TEdit
          Left = 208
          Top = 36
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
      end
      object GroupBox25: TGroupBox
        Left = 8
        Top = 72
        Width = 273
        Height = 217
        Caption = #30719#30707#31867#22411#26426#29575
        TabOrder = 2
        object Label34: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #30719#30707#22240#23376':'
        end
        object Label35: TLabel
          Left = 8
          Top = 38
          Width = 42
          Height = 12
          Caption = #37329#30719#29575':'
        end
        object Label36: TLabel
          Left = 8
          Top = 56
          Width = 42
          Height = 12
          Caption = #38134#30719#29575':'
        end
        object Label37: TLabel
          Left = 8
          Top = 76
          Width = 42
          Height = 12
          Caption = #38081#30719#29575':'
        end
        object Label38: TLabel
          Left = 8
          Top = 96
          Width = 54
          Height = 12
          Caption = #40657#38081#30719#29575':'
        end
        object ScrollBarStoneTypeRate: TScrollBar
          Left = 72
          Top = 16
          Width = 129
          Height = 15
          Max = 500
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarStoneTypeRateChange
        end
        object EditStoneTypeRate: TEdit
          Left = 208
          Top = 16
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarGoldStoneMax: TScrollBar
          Left = 72
          Top = 36
          Width = 129
          Height = 15
          Max = 500
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarGoldStoneMaxChange
        end
        object EditGoldStoneMax: TEdit
          Left = 208
          Top = 36
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
        object ScrollBarSilverStoneMax: TScrollBar
          Left = 72
          Top = 56
          Width = 129
          Height = 15
          Max = 500
          PageSize = 0
          TabOrder = 4
          OnChange = ScrollBarSilverStoneMaxChange
        end
        object EditSilverStoneMax: TEdit
          Left = 208
          Top = 56
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 5
        end
        object ScrollBarSteelStoneMax: TScrollBar
          Left = 72
          Top = 76
          Width = 129
          Height = 15
          Max = 500
          PageSize = 0
          TabOrder = 6
          OnChange = ScrollBarSteelStoneMaxChange
        end
        object EditSteelStoneMax: TEdit
          Left = 208
          Top = 76
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 7
        end
        object EditBlackStoneMax: TEdit
          Left = 208
          Top = 96
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 9
        end
        object ScrollBarBlackStoneMax: TScrollBar
          Left = 72
          Top = 96
          Width = 129
          Height = 15
          Max = 500
          PageSize = 0
          TabOrder = 8
          OnChange = ScrollBarBlackStoneMaxChange
        end
      end
      object ButtonMakeMineSave: TButton
        Left = 376
        Top = 277
        Width = 65
        Height = 20
        Caption = #20445#23384'(&S)'
        TabOrder = 4
        OnClick = ButtonMakeMineSaveClick
      end
      object GroupBox26: TGroupBox
        Left = 288
        Top = 8
        Width = 153
        Height = 121
        Caption = #30719#30707#21697#36136
        TabOrder = 1
        object Label39: TLabel
          Left = 8
          Top = 18
          Width = 78
          Height = 12
          Caption = #30719#30707#26368#23567#21697#36136':'
        end
        object Label40: TLabel
          Left = 8
          Top = 42
          Width = 78
          Height = 12
          Caption = #26222#36890#21697#36136#33539#22260':'
        end
        object Label41: TLabel
          Left = 8
          Top = 66
          Width = 66
          Height = 12
          Caption = #39640#21697#36136#26426#29575':'
        end
        object Label42: TLabel
          Left = 8
          Top = 90
          Width = 66
          Height = 12
          Caption = #39640#21697#36136#33539#22260':'
        end
        object EditStoneMinDura: TSpinEditEx
          Left = 92
          Top = 15
          Width = 45
          Height = 21
          Hint = #30719#30707#20986#29616#26368#20302#21697#36136#28857#25968#12290
          MaxValue = 1000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditStoneMinDuraChange
        end
        object EditStoneGeneralDuraRate: TSpinEditEx
          Left = 92
          Top = 39
          Width = 45
          Height = 21
          Hint = #30719#30707#38543#26426#20986#29616#21697#36136#28857#25968#33539#22260#12290
          MaxValue = 1000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditStoneGeneralDuraRateChange
        end
        object EditStoneAddDuraRate: TSpinEditEx
          Left = 92
          Top = 63
          Width = 45
          Height = 21
          Hint = #30719#30707#20986#29616#39640#21697#36136#28857#25968#26426#29575#65292#39640#21697#36136#37327#25351#21487#36798#21040'20'#25110#20197#19978#30340#28857#25968#12290
          MaxValue = 1000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditStoneAddDuraRateChange
        end
        object EditStoneAddDuraMax: TSpinEditEx
          Left = 92
          Top = 87
          Width = 45
          Height = 21
          Hint = #39640#21697#36136#30719#30707#38543#26426#20986#29616#21697#36136#28857#25968#33539#22260#12290
          MaxValue = 1000
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = EditStoneAddDuraMaxChange
        end
      end
      object ButtonMakeMineDefault: TButton
        Left = 296
        Top = 277
        Width = 65
        Height = 20
        Caption = #40664#35748'(&D)'
        TabOrder = 3
        OnClick = ButtonMakeMineDefaultClick
      end
    end
    object TabSheet42: TTabSheet
      Caption = #31069#31119#27833#25511#21046
      ImageIndex = 12
      object GroupBox44: TGroupBox
        Left = 8
        Top = 8
        Width = 273
        Height = 217
        Caption = #26426#29575#35774#32622
        TabOrder = 0
        object Label105: TLabel
          Left = 8
          Top = 18
          Width = 54
          Height = 12
          Caption = #35781#21650#26426#29575':'
        end
        object Label106: TLabel
          Left = 8
          Top = 38
          Width = 54
          Height = 12
          Caption = #19968#32423#28857#25968':'
        end
        object Label107: TLabel
          Left = 8
          Top = 56
          Width = 54
          Height = 12
          Caption = #20108#32423#28857#25968':'
        end
        object Label108: TLabel
          Left = 8
          Top = 76
          Width = 54
          Height = 12
          Caption = #20108#32423#26426#29575':'
        end
        object Label109: TLabel
          Left = 8
          Top = 96
          Width = 54
          Height = 12
          Caption = #19977#32423#28857#25968':'
        end
        object Label110: TLabel
          Left = 8
          Top = 116
          Width = 54
          Height = 12
          Caption = #19977#32423#26426#29575':'
        end
        object ScrollBarWeaponMakeUnLuckRate: TScrollBar
          Left = 72
          Top = 16
          Width = 129
          Height = 15
          Hint = #20351#29992#31069#31119#27833#35781#21650#26426#29575#65292#25968#23383#36234#22823#26426#29575#36234#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarWeaponMakeUnLuckRateChange
        end
        object EditWeaponMakeUnLuckRate: TEdit
          Left = 208
          Top = 16
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarWeaponMakeLuckPoint1: TScrollBar
          Left = 72
          Top = 36
          Width = 129
          Height = 15
          Hint = #24403#27494#22120#30340#24184#36816#28857#23567#20110#27492#28857#25968#26102#20351#29992#31069#31119#27833#21017'100% '#25104#21151#12290
          Max = 500
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarWeaponMakeLuckPoint1Change
        end
        object EditWeaponMakeLuckPoint1: TEdit
          Left = 208
          Top = 36
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
        object ScrollBarWeaponMakeLuckPoint2: TScrollBar
          Left = 72
          Top = 56
          Width = 129
          Height = 15
          Hint = #24403#27494#22120#30340#24184#36816#28857#23567#20110#27492#28857#25968#26102#20351#29992#31069#31119#27833#21017#25353#25351#23450#26426#29575#20915#23450#26159#21542#21152#24184#36816#12290
          Max = 500
          PageSize = 0
          TabOrder = 4
          OnChange = ScrollBarWeaponMakeLuckPoint2Change
        end
        object EditWeaponMakeLuckPoint2: TEdit
          Left = 208
          Top = 56
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 5
        end
        object ScrollBarWeaponMakeLuckPoint2Rate: TScrollBar
          Left = 72
          Top = 76
          Width = 129
          Height = 15
          Hint = #26426#29575#28857#25968#65292#25968#23383#36234#22823#26426#29575#36234#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 6
          OnChange = ScrollBarWeaponMakeLuckPoint2RateChange
        end
        object EditWeaponMakeLuckPoint2Rate: TEdit
          Left = 208
          Top = 76
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 7
        end
        object EditWeaponMakeLuckPoint3: TEdit
          Left = 208
          Top = 96
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 9
        end
        object ScrollBarWeaponMakeLuckPoint3: TScrollBar
          Left = 72
          Top = 96
          Width = 129
          Height = 15
          Hint = #24403#27494#22120#30340#24184#36816#28857#23567#20110#27492#28857#25968#26102#20351#29992#31069#31119#27833#21017#25353#25351#23450#26426#29575#20915#23450#26159#21542#21152#24184#36816#12290
          Max = 500
          PageSize = 0
          TabOrder = 8
          OnChange = ScrollBarWeaponMakeLuckPoint3Change
        end
        object ScrollBarWeaponMakeLuckPoint3Rate: TScrollBar
          Left = 72
          Top = 116
          Width = 129
          Height = 15
          Hint = #26426#29575#28857#25968#65292#25968#23383#36234#22823#26426#29575#36234#23567#12290
          Max = 500
          PageSize = 0
          TabOrder = 10
          OnChange = ScrollBarWeaponMakeLuckPoint3RateChange
        end
        object EditWeaponMakeLuckPoint3Rate: TEdit
          Left = 208
          Top = 116
          Width = 57
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 11
        end
      end
      object ButtonWeaponMakeLuckDefault: TButton
        Left = 296
        Top = 277
        Width = 65
        Height = 20
        Caption = #40664#35748'(&D)'
        TabOrder = 1
        OnClick = ButtonWeaponMakeLuckDefaultClick
      end
      object ButtonWeaponMakeLuckSave: TButton
        Left = 376
        Top = 277
        Width = 65
        Height = 20
        Caption = #20445#23384'(&S)'
        TabOrder = 2
        OnClick = ButtonWeaponMakeLuckSaveClick
      end
    end
    object TabSheet37: TTabSheet
      Caption = #24425#31080#25511#21046
      ImageIndex = 8
      object GroupBox27: TGroupBox
        Left = 8
        Top = 8
        Width = 273
        Height = 169
        Caption = #20013#22870#26426#29575
        TabOrder = 0
        object Label43: TLabel
          Left = 8
          Top = 42
          Width = 42
          Height = 12
          Caption = #19968#31561#22870':'
        end
        object Label44: TLabel
          Left = 8
          Top = 62
          Width = 42
          Height = 12
          Caption = #20108#31561#22870':'
        end
        object Label45: TLabel
          Left = 8
          Top = 80
          Width = 42
          Height = 12
          Caption = #19977#31561#22870':'
        end
        object Label46: TLabel
          Left = 8
          Top = 100
          Width = 42
          Height = 12
          Caption = #22235#31561#22870':'
        end
        object Label47: TLabel
          Left = 8
          Top = 120
          Width = 42
          Height = 12
          Caption = #20116#31561#22870':'
        end
        object Label48: TLabel
          Left = 8
          Top = 140
          Width = 42
          Height = 12
          Caption = #20845#31561#22870':'
        end
        object Label49: TLabel
          Left = 8
          Top = 18
          Width = 30
          Height = 12
          Caption = #22240#23376':'
        end
        object ScrollBarWinLottery1Max: TScrollBar
          Left = 56
          Top = 40
          Width = 129
          Height = 15
          Max = 1000000
          PageSize = 0
          TabOrder = 2
          OnChange = ScrollBarWinLottery1MaxChange
        end
        object EditWinLottery1Max: TEdit
          Left = 192
          Top = 40
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 3
        end
        object ScrollBarWinLottery2Max: TScrollBar
          Left = 56
          Top = 60
          Width = 129
          Height = 15
          Max = 1000000
          PageSize = 0
          TabOrder = 4
          OnChange = ScrollBarWinLottery2MaxChange
        end
        object EditWinLottery2Max: TEdit
          Left = 192
          Top = 60
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 5
        end
        object ScrollBarWinLottery3Max: TScrollBar
          Left = 56
          Top = 80
          Width = 129
          Height = 15
          Max = 1000000
          PageSize = 0
          TabOrder = 6
          OnChange = ScrollBarWinLottery3MaxChange
        end
        object EditWinLottery3Max: TEdit
          Left = 192
          Top = 80
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 7
        end
        object ScrollBarWinLottery4Max: TScrollBar
          Left = 56
          Top = 100
          Width = 129
          Height = 15
          Max = 1000000
          PageSize = 0
          TabOrder = 8
          OnChange = ScrollBarWinLottery4MaxChange
        end
        object EditWinLottery4Max: TEdit
          Left = 192
          Top = 100
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 9
        end
        object EditWinLottery5Max: TEdit
          Left = 192
          Top = 120
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 11
        end
        object ScrollBarWinLottery5Max: TScrollBar
          Left = 56
          Top = 120
          Width = 129
          Height = 15
          Max = 1000000
          PageSize = 0
          TabOrder = 10
          OnChange = ScrollBarWinLottery5MaxChange
        end
        object ScrollBarWinLottery6Max: TScrollBar
          Left = 56
          Top = 140
          Width = 129
          Height = 15
          Max = 1000000
          PageSize = 0
          TabOrder = 12
          OnChange = ScrollBarWinLottery6MaxChange
        end
        object EditWinLottery6Max: TEdit
          Left = 192
          Top = 140
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 13
        end
        object EditWinLotteryRate: TEdit
          Left = 192
          Top = 16
          Width = 73
          Height = 18
          Ctl3D = False
          Enabled = False
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ParentCtl3D = False
          ReadOnly = True
          TabOrder = 1
        end
        object ScrollBarWinLotteryRate: TScrollBar
          Left = 56
          Top = 16
          Width = 129
          Height = 15
          Max = 100000
          PageSize = 0
          TabOrder = 0
          OnChange = ScrollBarWinLotteryRateChange
        end
      end
      object GroupBox28: TGroupBox
        Left = 288
        Top = 8
        Width = 145
        Height = 169
        Caption = #22870#37329
        TabOrder = 1
        object Label50: TLabel
          Left = 8
          Top = 18
          Width = 42
          Height = 12
          Caption = #19968#31561#22870':'
        end
        object Label51: TLabel
          Left = 8
          Top = 42
          Width = 42
          Height = 12
          Caption = #20108#31561#22870':'
        end
        object Label52: TLabel
          Left = 8
          Top = 66
          Width = 42
          Height = 12
          Caption = #19977#31561#22870':'
        end
        object Label53: TLabel
          Left = 8
          Top = 90
          Width = 42
          Height = 12
          Caption = #22235#31561#22870':'
        end
        object Label54: TLabel
          Left = 8
          Top = 114
          Width = 42
          Height = 12
          Caption = #20116#31561#22870':'
        end
        object Label55: TLabel
          Left = 8
          Top = 138
          Width = 42
          Height = 12
          Caption = #20845#31561#22870':'
        end
        object EditWinLottery1Gold: TSpinEditEx
          Left = 56
          Top = 15
          Width = 81
          Height = 21
          Increment = 500
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 0
          Value = 100000000
          OnChange = EditWinLottery1GoldChange
        end
        object EditWinLottery2Gold: TSpinEditEx
          Left = 56
          Top = 39
          Width = 81
          Height = 21
          Increment = 500
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditWinLottery2GoldChange
        end
        object EditWinLottery3Gold: TSpinEditEx
          Left = 56
          Top = 63
          Width = 81
          Height = 21
          Increment = 500
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditWinLottery3GoldChange
        end
        object EditWinLottery4Gold: TSpinEditEx
          Left = 56
          Top = 87
          Width = 81
          Height = 21
          Increment = 500
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = EditWinLottery4GoldChange
        end
        object EditWinLottery5Gold: TSpinEditEx
          Left = 56
          Top = 111
          Width = 81
          Height = 21
          Increment = 500
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 4
          Value = 10
          OnChange = EditWinLottery5GoldChange
        end
        object EditWinLottery6Gold: TSpinEditEx
          Left = 56
          Top = 135
          Width = 81
          Height = 21
          Increment = 500
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 5
          Value = 10
          OnChange = EditWinLottery6GoldChange
        end
      end
      object ButtonWinLotterySave: TButton
        Left = 376
        Top = 277
        Width = 65
        Height = 20
        Caption = #20445#23384'(&S)'
        ModalResult = 1
        TabOrder = 3
        OnClick = ButtonWinLotterySaveClick
      end
      object ButtonWinLotteryDefault: TButton
        Left = 296
        Top = 277
        Width = 65
        Height = 20
        Caption = #40664#35748'(&D)'
        TabOrder = 2
        OnClick = ButtonWinLotteryDefaultClick
      end
    end
    object TabSheet40: TTabSheet
      Caption = #31048#31095#29983#25928
      ImageIndex = 11
      object GroupBox36: TGroupBox
        Left = 8
        Top = 8
        Width = 137
        Height = 89
        Caption = #31048#31095#29983#25928
        TabOrder = 0
        object Label94: TLabel
          Left = 11
          Top = 40
          Width = 54
          Height = 12
          Caption = #29983#25928#26102#38271':'
        end
        object Label96: TLabel
          Left = 11
          Top = 64
          Width = 54
          Height = 12
          Caption = #33021#37327#20493#25968':'
          Enabled = False
        end
        object CheckBoxSpiritMutiny: TCheckBox
          Left = 8
          Top = 16
          Width = 113
          Height = 17
          Caption = #21551#29992#31048#31095#29305#27530#21151#33021
          TabOrder = 0
          OnClick = CheckBoxSpiritMutinyClick
        end
        object EditSpiritMutinyTime: TSpinEditEx
          Left = 72
          Top = 36
          Width = 49
          Height = 21
          MaxValue = 9999
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditSpiritMutinyTimeChange
        end
        object EditSpiritPowerRate: TSpinEditEx
          Left = 72
          Top = 60
          Width = 49
          Height = 21
          Enabled = False
          MaxValue = 9999
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = EditSpiritPowerRateChange
        end
      end
      object ButtonSpiritMutinySave: TButton
        Left = 360
        Top = 261
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonSpiritMutinySaveClick
      end
    end
    object TabSheet50: TTabSheet
      Caption = #33521#38596#35774#32622
      ImageIndex = 14
      object ButtonHeroOptionSave: TButton
        Left = 384
        Top = 336
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonHeroOptionSaveClick
      end
      object PageControlHeroConfig: TPageControl
        Left = 0
        Top = 0
        Width = 449
        Height = 336
        ActivePage = TabSheet44
        Align = alTop
        TabOrder = 0
        object TabSheet44: TTabSheet
          Caption = #22522#26412#35774#32622
          object GroupBox64: TGroupBox
            Left = 205
            Top = 1
            Width = 122
            Height = 84
            Caption = #33521#38596#23432#25252
            TabOrder = 2
            object Label183: TLabel
              Left = 8
              Top = 40
              Width = 48
              Height = 12
              Caption = #38656#35201#31561#32423
            end
            object Label184: TLabel
              Left = 8
              Top = 62
              Width = 48
              Height = 12
              Caption = #23432#25252#33539#22260
            end
            object EditNeedGuardLevel: TSpinEditEx
              Left = 59
              Top = 36
              Width = 56
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = EditNeedGuardLevelChange
            end
            object EditGuardRange: TSpinEditEx
              Left = 59
              Top = 58
              Width = 56
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = EditGuardRangeChange
            end
            object chkHeroDisableSafeZoneProtect: TCheckBox
              Left = 9
              Top = 16
              Width = 106
              Height = 16
              Caption = #31105#27490#23433#20840#21306#23432#25252
              TabOrder = 0
              OnClick = chkHeroDisableSafeZoneProtectClick
            end
          end
          object GroupBox67: TGroupBox
            Left = 104
            Top = 1
            Width = 98
            Height = 85
            Caption = #25915#20987#38388#38548'('#27627#31186')'
            TabOrder = 1
            object Label158: TLabel
              Left = 9
              Top = 18
              Width = 24
              Height = 12
              Caption = #25112#22763
            end
            object Label162: TLabel
              Left = 9
              Top = 40
              Width = 24
              Height = 12
              Caption = #27861#24072
            end
            object Label163: TLabel
              Left = 9
              Top = 62
              Width = 24
              Height = 12
              Caption = #36947#22763
            end
            object EditHeroWarrorAttackTime: TSpinEditEx
              Left = 36
              Top = 14
              Width = 55
              Height = 21
              MaxValue = 10000
              MinValue = 10
              TabOrder = 0
              Value = 10
              OnChange = EditHeroWarrorAttackTimeChange
            end
            object EditHeroTaoistAttackTime: TSpinEditEx
              Left = 36
              Top = 58
              Width = 55
              Height = 21
              MaxValue = 10000
              MinValue = 10
              TabOrder = 2
              Value = 10
              OnChange = EditHeroTaoistAttackTimeChange
            end
            object EditHeroWizardAttackTime: TSpinEditEx
              Left = 36
              Top = 36
              Width = 55
              Height = 21
              MaxValue = 10000
              MinValue = 10
              TabOrder = 1
              Value = 10
              OnChange = EditHeroWizardAttackTimeChange
            end
          end
          object GroupBox65: TGroupBox
            Left = 3
            Top = 224
            Width = 124
            Height = 84
            Caption = #33521#38596#26174#31034#21517#31216#35774#32622
            TabOrder = 8
            object Label164: TLabel
              Left = 8
              Top = 40
              Width = 36
              Height = 12
              Caption = #39068#33394#65306
            end
            object Label166: TLabel
              Left = 8
              Top = 62
              Width = 36
              Height = 12
              Caption = #21518#32512#65306
            end
            object CheckBoxHeroShowMasterName: TCheckBox
              Left = 8
              Top = 16
              Width = 102
              Height = 17
              Caption = #26174#31034#20027#20154#21517#31216
              TabOrder = 0
              OnClick = CheckBoxHeroShowMasterNameClick
            end
            object EditHeroNameColor: TColorIndexEdit
              Left = 39
              Top = 36
              Width = 72
              Height = 21
              Hint = #24403#20154#29289#25915#20987#20854#20182#20154#29289#26102#21517#23383#39068#33394#65292#40664#35748#20026'47'
              MaxLength = 3
              MaxValue = 255
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditHeroNameColorChange
              ShowNoneColor = False
            end
            object EditHeroSuffixName: TEdit
              Left = 39
              Top = 58
              Width = 72
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 1
              Text = #30340#33521#38596
              OnChange = EditHeroSuffixNameChange
            end
          end
          object GroupBox76: TGroupBox
            Left = 3
            Top = 156
            Width = 124
            Height = 61
            Caption = #21253#35065#35774#32622
            TabOrder = 6
            object Label182: TLabel
              Left = 10
              Top = 40
              Width = 60
              Height = 12
              Caption = #38656#35201#31561#32423#65306
            end
            object EditNeedLevel: TSpinEditEx
              Left = 67
              Top = 35
              Width = 48
              Height = 21
              Hint = #25351#23450#21253#35065#25968#38656#35201#30340#31561#32423#12290
              MaxValue = 65535
              MinValue = 0
              TabOrder = 1
              Value = 1
              OnChange = EditNeedLevelChange
            end
            object ComboBoxBagItemCount: TComboBox
              Left = 10
              Top = 14
              Width = 104
              Height = 20
              Style = csDropDownList
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 0
              OnChange = ComboBoxBagItemCountChange
              Items.Strings = (
                '10'#26684
                '20'#26684
                '30'#26684
                '35'#26684
                '40'#26684)
            end
          end
          object GroupBox66: TGroupBox
            Left = 3
            Top = 1
            Width = 98
            Height = 85
            Caption = #34892#36208#38388#38548'('#27627#31186')'
            TabOrder = 0
            object Label152: TLabel
              Left = 8
              Top = 18
              Width = 24
              Height = 12
              Caption = #25112#22763
            end
            object Label154: TLabel
              Left = 8
              Top = 40
              Width = 24
              Height = 12
              Caption = #27861#24072
            end
            object Label156: TLabel
              Left = 8
              Top = 62
              Width = 24
              Height = 12
              Caption = #36947#22763
            end
            object EditHeroWarrorWalkTime: TSpinEditEx
              Left = 35
              Top = 14
              Width = 55
              Height = 21
              MaxValue = 10000
              MinValue = 10
              TabOrder = 0
              Value = 10
              OnChange = EditHeroWarrorWalkTimeChange
            end
            object EditHeroWizardWalkTime: TSpinEditEx
              Left = 35
              Top = 36
              Width = 55
              Height = 21
              MaxValue = 10000
              MinValue = 10
              TabOrder = 1
              Value = 10
              OnChange = EditHeroWizardWalkTimeChange
            end
            object EditHeroTaoistWalkTime: TSpinEditEx
              Left = 35
              Top = 58
              Width = 55
              Height = 21
              MaxValue = 10000
              MinValue = 10
              TabOrder = 2
              Value = 10
              OnChange = EditHeroTaoistWalkTimeChange
            end
          end
          object GroupBox201: TGroupBox
            Left = 266
            Top = 252
            Width = 148
            Height = 55
            Caption = #33521#38596#24367#33136#25511#21046
            TabOrder = 7
            object chkHeroDisableStruck: TCheckBox
              Left = 10
              Top = 16
              Width = 105
              Height = 17
              Hint = #33521#38596#34987#25915#20987#21518#26159#21542#26174#31034#24367#33136#21160#20316
              Caption = #33521#38596#26080#24367#33136#21160#20316
              TabOrder = 0
              OnClick = chkHeroDisableStruckClick
            end
            object chkHeroDisableSelfStruck: TCheckBox
              Left = 10
              Top = 33
              Width = 105
              Height = 17
              Caption = #33521#38596#33258#24049#19981#24367#33136
              TabOrder = 1
              OnClick = chkHeroDisableSelfStruckClick
            end
          end
          object GroupBox202: TGroupBox
            Left = 3
            Top = 89
            Width = 124
            Height = 64
            Caption = #33521#38596#21484#21796#38388#38548
            TabOrder = 5
            object Label151: TLabel
              Left = 8
              Top = 19
              Width = 60
              Height = 12
              Caption = #20027#23558#33521#38596#65306
            end
            object Label271: TLabel
              Left = 8
              Top = 41
              Width = 60
              Height = 12
              Caption = #21103#23558#33521#38596#65306
            end
            object EditHeroRecallTime: TSpinEditEx
              Left = 65
              Top = 15
              Width = 50
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 0
              Value = 0
              OnChange = EditHeroRecallTimeChange
            end
            object EditRecallDeputyHeroTime: TSpinEditEx
              Left = 65
              Top = 37
              Width = 50
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = EditRecallDeputyHeroTimeChange
            end
          end
          object grp35: TGroupBox
            Left = 134
            Top = 156
            Width = 124
            Height = 64
            Caption = #33521#38596#20986#29983#31561#32423
            TabOrder = 4
            object lbl88: TLabel
              Left = 10
              Top = 20
              Width = 60
              Height = 12
              Caption = #20027#23558#33521#38596#65306
            end
            object Label633: TLabel
              Left = 10
              Top = 43
              Width = 60
              Height = 12
              Caption = #21103#23558#33521#38596#65306
            end
            object seHeroMasterStartLevel: TSpinEditEx
              Left = 67
              Top = 16
              Width = 50
              Height = 21
              MaxValue = 10000
              MinValue = 1
              TabOrder = 0
              Value = 11
              OnChange = seHeroMasterStartLevelChange
            end
            object seHeroSlaveStartLevel: TSpinEditEx
              Left = 67
              Top = 38
              Width = 50
              Height = 21
              MaxValue = 10000
              MinValue = 1
              TabOrder = 1
              Value = 11
              OnChange = seHeroSlaveStartLevelChange
            end
          end
          object grp40: TGroupBox
            Left = 266
            Top = 142
            Width = 148
            Height = 107
            Caption = #22522#26412#35774#32622
            TabOrder = 3
            object chkHeroCalcWeaponSpeed: TCheckBox
              Left = 10
              Top = 51
              Width = 132
              Height = 17
              Hint = #21246#36873#21518#33521#38596#31359#25140#25915#20987#21152#36895#35013#22791#23558#25552#21319#25915#20987#36895#24230
              Caption = #33521#38596#35745#31639#27494#22120#36895#24230
              TabOrder = 1
              OnClick = chkHeroCalcWeaponSpeedClick
            end
            object chkCreditPointWithLevel: TCheckBox
              Left = 10
              Top = 68
              Width = 132
              Height = 17
              Hint = #21246#36873#21518#24403#35013#22791#38656#35201#22768#26395#20540#26102#65292#21017#25353#31561#32423#31639#65292#35745#31639#27604#20363#20026'1:1'
              Caption = #22768#26395#35013#22791#25353#31561#32423#35745#31639' '
              TabOrder = 2
              OnClick = chkCreditPointWithLevelClick
            end
            object CheckBoxHeroPickUpItem: TCheckBox
              Left = 10
              Top = 16
              Width = 132
              Height = 17
              Caption = #20801#35768#33521#38596#25441#21462#29289#21697
              TabOrder = 0
              OnClick = CheckBoxHeroPickUpItemClick
            end
            object chkHeroAutoSuperShiled: TCheckBox
              Left = 10
              Top = 86
              Width = 129
              Height = 17
              Caption = #33521#38596#33258#21160#24320#25252#20307#31070#30462
              TabOrder = 3
              OnClick = chkHeroAutoSuperShiledClick
            end
            object chkHeroOnlyPickMonsterItem: TCheckBox
              Left = 10
              Top = 34
              Width = 132
              Height = 17
              Hint = #21246#36873#27492#36873#39033#21518#65292#33521#38596#21482#20250#25441#24618#29289#29190#20986#30340#29289#21697#65307#24403#21246#36873#8220#20801#35768#33521#38596#25441#21462#29289#21697#8221#26102#26377#25928
              Caption = #33521#38596#21482#25441#24618#29289#29190#20986
              TabOrder = 4
              OnClick = chkHeroOnlyPickMonsterItemClick
            end
          end
          object grp45: TGroupBox
            Left = 266
            Top = 89
            Width = 148
            Height = 51
            Caption = #33521#38596#36319#38543#35774#32622
            TabOrder = 9
            object lbl100: TLabel
              Left = 11
              Top = 82
              Width = 60
              Height = 12
              Caption = #25915#20987#33539#22260#65306
              Visible = False
            end
            object seHeroAttackRange: TSpinEditEx
              Left = 68
              Top = 78
              Width = 63
              Height = 21
              Hint = #33539#22260#36234#23567#65292#33521#38596#27963#21160#25915#20987#33539#22260#36234#23567
              MaxValue = 20
              MinValue = 3
              TabOrder = 0
              Value = 3
              Visible = False
              OnChange = seHeroAttackRangeChange
            end
            object chkHeroFollowMasterWithDiffScreen: TCheckBox
              Left = 10
              Top = 31
              Width = 132
              Height = 17
              Hint = 
                #27492#36873#39033#21253#21547#33521#38596#38596#21644#20027#20154#19981#22312#21516#19968#22320#22270#13#10#21246#36873#21518#65292#33521#38596#22788#20110#25915#20987#12289#36319#38543#27169#24335#19979#37117#20250#22312#19968#23450#33539#22260#36319#38543#20027#20154#13#10#19981#21246#36873#21017#21482#26377#22312#33521#38596#26080#25915#20987#30446#26631#26102#25165 +
                #36319#38543#20027#20154
              Caption = #19981#21516#23631#22238#21040#20027#20154#36523#36793
              TabOrder = 1
              OnClick = chkHeroFollowMasterWithDiffScreenClick
            end
            object chkHeroNoMoveOnSleep: TCheckBox
              Left = 10
              Top = 16
              Width = 132
              Height = 16
              Hint = #33521#38596#22312#20241#24687#29366#24577#26102#65292#13#10#21246#36873#26102#65306#20250#38543#20027#20154#19968#36215#25442#22320#22270#13#10#19981#21246#36873#65306#19981#20250#38543#20027#20154#19968#36215#25442#22320#22270
              Caption = #20241#24687#19981#38543#20027#20154#25442#22320#22270
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              OnClick = chkHeroNoMoveOnSleepClick
            end
          end
          object GroupBox234: TGroupBox
            Left = 134
            Top = 224
            Width = 124
            Height = 44
            Caption = #33521#38596#32842#22825#26694#21069#32512
            TabOrder = 10
            object Label175: TLabel
              Left = 8
              Top = 22
              Width = 36
              Height = 12
              Caption = #21069#32512#65306
            end
            object edtHeroSayPrefix: TEdit
              Left = 40
              Top = 18
              Width = 80
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 0
              Text = #30340#33521#38596
              OnChange = edtHeroSayPrefixChange
            end
          end
          object grp74: TGroupBox
            Left = 134
            Top = 88
            Width = 124
            Height = 64
            Caption = #36947#27861#33521#38596#36530#36991
            TabOrder = 11
            object Label827: TLabel
              Left = 9
              Top = 22
              Width = 36
              Height = 12
              Caption = #38388#38548#65306
            end
            object seHeroAvoidTime: TSpinEditEx
              Left = 44
              Top = 17
              Width = 72
              Height = 21
              Hint = #23454#38469#38388#38548#19981#20250#20302#20110#34892#36208#38388#38548#35774#32622
              MaxValue = 100000
              MinValue = 10
              TabOrder = 0
              Value = 10
              OnChange = seHeroAvoidTimeChange
            end
            object chkHeroDFAvoidTargetRight: TCheckBox
              Left = 8
              Top = 40
              Width = 100
              Height = 17
              Caption = #24320#21551#26234#33021#36530#36991
              TabOrder = 1
              OnClick = chkHeroDFAvoidTargetRightClick
            end
          end
          object GroupBox246: TGroupBox
            Left = 331
            Top = 1
            Width = 106
            Height = 58
            Caption = #25112#22763#36895#24230#34917#20607
            TabOrder = 12
            object Label881: TLabel
              Left = 7
              Top = 35
              Width = 36
              Height = 12
              Caption = #34917#20607#20540
            end
            object seWarrCmpInvTime: TSpinEdit
              Left = 46
              Top = 31
              Width = 47
              Height = 21
              Hint = #25968#23383#36234#23567', '#25112#22763#33521#38596#36861#20987#30446#26631#36234#24555#65292#40664#35748'100'
              Increment = 10
              MaxValue = 999
              MinValue = 0
              TabOrder = 0
              Value = 200
              OnChange = seWarrCmpInvTimeChange
            end
            object chkHeroHitCmp: TCheckBox
              Left = 6
              Top = 18
              Width = 93
              Height = 11
              Hint = #29992#20110#34917#20607#31227#21160#21518#30340#19979#19968#27425#25915#20987#38388#38548#65288#21363#31227#21160#25552#39640#19968#27425#25915#20987#36895#24230#65289
              Caption = #24320#21551#36895#24230#34917#20607
              TabOrder = 1
              OnClick = chkHeroHitCmpClick
            end
          end
        end
        object TabSheet45: TTabSheet
          Caption = #33521#38596#27515#20129
          ImageIndex = 1
          object GroupBox106: TGroupBox
            Left = 8
            Top = 5
            Width = 177
            Height = 160
            Caption = #27515#20129#25481#29289#21697#35268#21017
            TabOrder = 0
            object CheckBoxKillByMonstDropHeroUseItem: TCheckBox
              Left = 8
              Top = 16
              Width = 121
              Height = 17
              Hint = #24403#33521#38596#34987#24618#29289#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#36523#19978#25140#30340#29289#21697#12290
              Caption = #34987#24618#29289#26432#27515#25481#35013#22791
              TabOrder = 0
              OnClick = CheckBoxKillByMonstDropHeroUseItemClick
            end
            object CheckBoxKillByHumanDropHeroUseItem: TCheckBox
              Left = 8
              Top = 32
              Width = 121
              Height = 17
              Hint = #24403#33521#38596#34987#21035#20154#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#36523#19978#25140#30340#29289#21697#12290
              Caption = #34987#20154#29289#26432#27515#25481#35013#22791
              TabOrder = 1
              OnClick = CheckBoxKillByHumanDropHeroUseItemClick
            end
            object CheckBoxDieScatterHeroBag: TCheckBox
              Left = 8
              Top = 119
              Width = 113
              Height = 17
              Hint = #24403#33521#38596#27515#20129#26102#20250#25353#25481#33853#26426#29575#25481#33853#32972#21253#37324#30340#29289#21697#12290
              Caption = #27515#20129#25481#32972#21253#29289#21697
              TabOrder = 2
              OnClick = CheckBoxDieScatterHeroBagClick
            end
            object CheckBoxDieRedScatterHeroBagAll: TCheckBox
              Left = 8
              Top = 136
              Width = 129
              Height = 17
              Hint = #32418#21517#33521#38596#27515#20129#26102#25481#33853#32972#21253#20013#20840#37096#29289#21697#12290
              Caption = #32418#21517#25481#20840#37096#32972#21253#29289#21697
              TabOrder = 3
              OnClick = CheckBoxDieRedScatterHeroBagAllClick
            end
            object chkKillByHumanDropHeroJewelryBoxItem: TCheckBox
              Left = 8
              Top = 67
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#21035#20154#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#39318#39280#30418#30340#29289#21697#12290
              Caption = #34987#20154#29289#26432#27515#25481#39318#39280#30418#29289#21697
              TabOrder = 4
              OnClick = chkKillByHumanDropHeroJewelryBoxItemClick
            end
            object chkKillByMonstDropHeroJewelryBoxItem: TCheckBox
              Left = 8
              Top = 49
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#24618#29289#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#39318#39280#30418#30340#29289#21697#12290
              Caption = #34987#24618#29289#26432#27515#25481#39318#39280#30418#29289#21697
              TabOrder = 5
              OnClick = chkKillByMonstDropHeroJewelryBoxItemClick
            end
            object chkKillByHumanDropHeroGodBlessItem: TCheckBox
              Left = 8
              Top = 101
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#21035#20154#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#31070#20305#30418#30340#29289#21697#12290
              Caption = #34987#20154#29289#26432#27515#25481#31070#20305#30418#29289#21697
              TabOrder = 6
              OnClick = chkKillByHumanDropHeroGodBlessItemClick
            end
            object chkKillByMonstDropHeroGodBlessItem: TCheckBox
              Left = 8
              Top = 84
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#24618#29289#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#31070#20305#30418#30340#29289#21697#12290
              Caption = #34987#24618#29289#26432#27515#25481#31070#20305#30418#29289#21697
              TabOrder = 7
              OnClick = chkKillByMonstDropHeroGodBlessItemClick
            end
          end
          object GroupBox105: TGroupBox
            Left = 192
            Top = 5
            Width = 244
            Height = 140
            Caption = #27515#20129#25481#29289#21697#26426#29575
            TabOrder = 1
            object Label223: TLabel
              Left = 8
              Top = 18
              Width = 66
              Height = 12
              Caption = #25481#36523#19978#35013#22791':'
            end
            object Label224: TLabel
              Left = 8
              Top = 42
              Width = 66
              Height = 12
              Caption = #25481#32418#21517#35013#22791':'
            end
            object Label225: TLabel
              Left = 8
              Top = 66
              Width = 66
              Height = 12
              Caption = #25481#32972#21253#29289#21697':'
            end
            object Label716: TLabel
              Left = 8
              Top = 90
              Width = 66
              Height = 12
              Caption = #39318#39280#30418#29289#21697':'
            end
            object Label717: TLabel
              Left = 8
              Top = 114
              Width = 66
              Height = 12
              Caption = #31070#20305#30418#29289#21697':'
            end
            object ScrollBarDieDropHeroUseItemRate: TScrollBar
              Left = 76
              Top = 16
              Width = 112
              Height = 17
              Hint = #33521#38596#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 0
              OnChange = ScrollBarDieDropHeroUseItemRateChange
            end
            object EditDieDropHeroUseItemRate: TEdit
              Left = 191
              Top = 16
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 1
            end
            object ScrollBarDieRedDropHeroUseItemRate: TScrollBar
              Left = 76
              Top = 40
              Width = 112
              Height = 17
              Hint = #32418#21517#33521#38596#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              PageSize = 0
              TabOrder = 2
              OnChange = ScrollBarDieRedDropHeroUseItemRateChange
            end
            object EditDieRedDropHeroUseItemRate: TEdit
              Left = 191
              Top = 40
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 3
            end
            object ScrollBarDieScatterHeroBagRate: TScrollBar
              Left = 76
              Top = 64
              Width = 112
              Height = 17
              Hint = #33521#38596#27515#20129#25481#33853#32972#21253#20013#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 4
              OnChange = ScrollBarDieScatterHeroBagRateChange
            end
            object EditDieScatterHeroBagRate: TEdit
              Left = 191
              Top = 64
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 5
            end
            object scrlbrHeroJewelryBoxItem: TScrollBar
              Left = 76
              Top = 88
              Width = 112
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#32972#21253#20013#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 6
              OnChange = scrlbrHeroJewelryBoxItemChange
            end
            object edtHeroJewelryBoxItem: TEdit
              Left = 191
              Top = 88
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 7
            end
            object scrlbrHeroGodBlessItem: TScrollBar
              Left = 76
              Top = 112
              Width = 112
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#32972#21253#20013#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 8
              OnChange = scrlbrHeroGodBlessItemChange
            end
            object edtHeroGodBlessItem: TEdit
              Left = 191
              Top = 112
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 9
            end
          end
          object GroupBox177: TGroupBox
            Left = 8
            Top = 171
            Width = 177
            Height = 71
            Caption = #27515#20129#30456#20851
            TabOrder = 2
            object Label631: TLabel
              Left = 125
              Top = 22
              Width = 24
              Height = 12
              Caption = '/100'
            end
            object lbl86: TLabel
              Left = 14
              Top = 22
              Width = 72
              Height = 12
              Caption = #27515#20129#25481#32463#39564#65306
            end
            object Label623: TLabel
              Left = 2
              Top = 46
              Width = 84
              Height = 12
              Caption = #23608#20307#28165#29702#38388#38548#65306
            end
            object lbl116: TLabel
              Left = 148
              Top = 46
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object seHeroDieExpRate: TSpinEditEx
              Left = 83
              Top = 18
              Width = 40
              Height = 21
              Hint = #27515#20129#26102#25481#24403#21069#32463#39564#30340#30334#20998#27604#65292#20026'0'#26102#19981#25481#32463#39564
              MaxValue = 100
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 100
              OnChange = seHeroDieExpRateChange
            end
            object seClearHeroGhostTick: TSpinEditEx
              Left = 83
              Top = 42
              Width = 65
              Height = 21
              Hint = #33521#38596#27515#20129#21518#28165#29702#23608#20307#31561#24453#26102#38388#65292#40664#35748'3000'#27627#31186
              Increment = 1000
              MaxValue = 100000
              MinValue = 1000
              TabOrder = 1
              Value = 3000
              OnChange = seClearHeroGhostTickChange
            end
          end
          object grp41: TGroupBox
            Left = 192
            Top = 155
            Width = 241
            Height = 38
            Caption = #20854#23427#35774#32622
            TabOrder = 3
            object Label634: TLabel
              Left = 130
              Top = 15
              Width = 60
              Height = 12
              Caption = #35781#21650#26426#29575#65306
            end
            object chkKillHeroWeaponUnlock: TCheckBox
              Left = 8
              Top = 13
              Width = 121
              Height = 17
              Caption = #26432#27515#33521#38596#27494#22120#35781#21650
              TabOrder = 0
              OnClick = chkKillHeroWeaponUnlockClick
            end
            object seKillHeroWeaponUnlockRate: TSpinEditEx
              Left = 187
              Top = 11
              Width = 45
              Height = 21
              Hint = #20540#36234#23567','#27494#22120#36234#23481#26131#34987#35781#21650
              MaxValue = 2000
              MinValue = 1
              TabOrder = 1
              Value = 10
              OnChange = seKillHeroWeaponUnlockRateChange
            end
          end
        end
        object ts12: TTabSheet
          Caption = #20854#23427#35774#32622
          ImageIndex = 2
          object Label937: TLabel
            Left = 157
            Top = 280
            Width = 108
            Height = 12
            Caption = #38145#23450#30446#26631#36317#31163#38480#21046#65306
          end
          object GroupBox176: TGroupBox
            Left = 5
            Top = 90
            Width = 146
            Height = 143
            Caption = #36947#22763#33521#38596#35774#32622
            TabOrder = 3
            object Label667: TLabel
              Left = 6
              Top = 78
              Width = 84
              Height = 12
              Caption = 'HP'#20197#19979#19981#26045#27602#65306
            end
            object lbl147: TLabel
              Left = 5
              Top = 120
              Width = 24
              Height = 12
              Caption = #25216#33021
            end
            object chkHeroCallBB: TCheckBox
              Left = 7
              Top = 14
              Width = 95
              Height = 17
              Caption = #21484#21796#23453#23453#25968#37327' '
              TabOrder = 0
              OnClick = chkHeroCallBBClick
            end
            object seHeroCallBBCount: TSpinEditEx
              Left = 101
              Top = 12
              Width = 38
              Height = 21
              MaxValue = 1000
              MinValue = 0
              TabOrder = 1
              Value = 10
              OnChange = seHeroCallBBCountChange
            end
            object chkHero700HPUseBaseAttack: TCheckBox
              Left = 7
              Top = 38
              Width = 135
              Height = 15
              Hint = #36947#22763','#24403#30446#26631'HP'#23569#20110#25351#23450#20540#26102','#21487#20197#20351#29992#29289#29702#25915#20987
              Caption = '          MaxHP'#19979#30733
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              OnClick = chkHero700HPUseBaseAttackClick
            end
            object chkHeroTaosAutoChangePoison: TCheckBox
              Left = 6
              Top = 58
              Width = 79
              Height = 17
              Hint = #27492#36873#39033#23545#20351#29992#37197#25140#31526#27602#12289#20351#29992#21253#35065#20013#30340#31526#27602#26377#25928
              Caption = #33258#21160#25442#27602
              TabOrder = 3
              OnClick = chkHeroTaosAutoChangePoisonClick
            end
            object seHeroTaoUsePoisonMinHP: TSpinEditEx
              Left = 85
              Top = 74
              Width = 53
              Height = 21
              Hint = #25915#20987#30446#26631#22312#25351#23450'HP'#20197#19979#19981#26045#27602
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 10
              OnChange = seHeroTaoUsePoisonMinHPChange
            end
            object chkHeroNoTargetRecallBB: TCheckBox
              Left = 5
              Top = 98
              Width = 132
              Height = 17
              Hint = #36947#22763#33521#38596#26080#25915#20987#30446#26631#26102#65292#21487#20197#21484#21796#23453#23453
              Caption = #36947#22763#26080#30446#26631#21484#21796#23453#23453
              TabOrder = 5
              OnClick = chkHeroNoTargetRecallBBClick
            end
            object seHero700HPValue: TSpinEditEx
              Left = 25
              Top = 34
              Width = 55
              Height = 21
              MaxValue = 100000
              MinValue = 0
              TabOrder = 6
              Value = 10
              OnChange = seHero700HPValueChange
            end
            object cbbHeroNeedMagicItem: TComboBox
              Left = 33
              Top = 116
              Width = 104
              Height = 20
              Style = csDropDownList
              TabOrder = 7
              OnChange = cbbHeroNeedMagicItemChange
              Items.Strings = (
                #26080#38656#27602'/'#31526
                #20351#29992#20329#25140#27602'/'#31526
                #20351#29992#32972#21253#27602'/'#31526)
            end
          end
          object GroupBox180: TGroupBox
            Left = 157
            Top = 87
            Width = 139
            Height = 68
            Caption = #33521#38596#22235#32423#25216#33021
            TabOrder = 1
            object Label640: TLabel
              Left = 11
              Top = 22
              Width = 60
              Height = 12
              Caption = #22235#32423#35302#21457#65306
            end
            object Label641: TLabel
              Left = 11
              Top = 44
              Width = 60
              Height = 12
              Caption = #23041#21147#22686#21152#65306
            end
            object seHeroGotoLV4: TSpinEditEx
              Left = 67
              Top = 17
              Width = 65
              Height = 21
              Hint = #24544#35802#24230#36798#21040#25351#23450#25968#20540#26102'(1'#20026'0.01)'#65292#35302#21457#22235#32423#25216#33021#13#10#13#10#35774#32622#20026'20000'#65292#21487#20197#20851#38381#22235#32423#25216#33021#35302#21457
              MaxValue = 20000
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 3000
              OnChange = seHeroGotoLV4Change
            end
            object seHeroPowerLV4: TSpinEditEx
              Left = 67
              Top = 39
              Width = 65
              Height = 21
              Hint = #22312#21407#26469#25216#33021#30340#22522#30784#19978#22686#21152#30340#26432#20260#21147#30340#30334#20998#27604#13#10#21482#23545#33521#38596#30340'4'#32423#28872#28779#21073#27861#12289#28781#22825#28779#21644#28789#39746#28779#31526#26377#25928
              MaxValue = 50
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              Value = 10
              OnChange = seHeroPowerLV4Change
            end
          end
          object GroupBox192: TGroupBox
            Left = 157
            Top = 3
            Width = 279
            Height = 81
            Caption = #24544#35802#24230#35774#32622
            TabOrder = 2
            object Label642: TLabel
              Left = 12
              Top = 17
              Width = 48
              Height = 12
              Caption = #21484#21796#22686#21152
            end
            object Label643: TLabel
              Left = 12
              Top = 39
              Width = 48
              Height = 12
              Caption = #33719#24471#32463#39564
            end
            object Label644: TLabel
              Left = 143
              Top = 39
              Width = 42
              Height = 12
              Caption = #28857'/'#22686#21152
            end
            object Label645: TLabel
              Left = 143
              Top = 17
              Width = 48
              Height = 12
              Caption = #21484#22238#20943#23569
            end
            object Label646: TLabel
              Left = 12
              Top = 61
              Width = 48
              Height = 12
              Caption = #27515#20129#20943#23569
            end
            object seHeroFealtyCallAdd: TSpinEditEx
              Left = 62
              Top = 12
              Width = 78
              Height = 21
              Hint = #33521#38596#27599#27425#25163#21160#21484#21796#20986#26469#22686#21152#24544#35802#24230#65292#24544#35802#24230#21333#20301#20026'% (1'#20026'0.01)'
              MaxValue = 10000
              MinValue = 0
              TabOrder = 0
              Value = 0
              OnChange = seHeroFealtyCallAddChange
            end
            object seHeroFealtyExp: TSpinEditEx
              Left = 62
              Top = 34
              Width = 78
              Height = 21
              Hint = #24403#33521#38596#33719#24471#35813#39033#25351#23450#32463#39564#20540#26102#65292#33258#21160#22686#21152#33521#38596#24544#35802#24230
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 0
              OnChange = seHeroFealtyExpChange
            end
            object seHeroFealtyExpAdd: TSpinEditEx
              Left = 194
              Top = 35
              Width = 78
              Height = 21
              Hint = #24403#33521#38596#33719#24471#35813#39033#25351#23450#32463#39564#20540#26102#65292#33258#21160#22686#21152#33521#38596#24544#35802#24230#65292#24544#35802#24230#21333#20301#20026'% (1'#20026'0.01)'
              MaxValue = 10000
              MinValue = 0
              TabOrder = 3
              Value = 0
              OnChange = seHeroFealtyExpAddChange
            end
            object seHeroFealtyCallBackDel: TSpinEditEx
              Left = 194
              Top = 12
              Width = 78
              Height = 21
              Hint = #24403#33521#38596#34987#25163#21160#21484#22238#26102'('#36864#20986#28216#25103#31561#19981#31639')'#65292#20943#23569#33521#38596#24544#35802#24230#65292#24544#35802#24230#21333#20301#20026'% (1'#20026'0.01)'
              MaxValue = 10000
              MinValue = 0
              TabOrder = 1
              Value = 0
              OnChange = seHeroFealtyCallBackDelChange
            end
            object seHeroFealtyDeathDel: TSpinEditEx
              Left = 62
              Top = 56
              Width = 78
              Height = 21
              Hint = #24403#33521#38596#27515#20129#26102#65292#33258#21160#20943#23569#33521#38596#24544#35802#24230#30340#20540#65292#24544#35802#24230#21333#20301#20026'% (1'#20026'0.01)'
              MaxValue = 10000
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = seHeroFealtyDeathDelChange
            end
          end
          object grp36: TGroupBox
            Left = 5
            Top = 3
            Width = 146
            Height = 85
            Caption = #33521#38596'HP/MP'#25511#21046
            TabOrder = 0
            object Label636: TLabel
              Left = 9
              Top = 22
              Width = 36
              Height = 12
              Caption = #25112#22763#65306
            end
            object Label637: TLabel
              Left = 9
              Top = 42
              Width = 36
              Height = 12
              Caption = #27861#24072#65306
            end
            object Label638: TLabel
              Left = 9
              Top = 62
              Width = 36
              Height = 12
              Caption = #36947#22763#65306
            end
            object lbl87: TLabel
              Left = 111
              Top = 22
              Width = 24
              Height = 12
              Caption = '/100'
            end
            object Label632: TLabel
              Left = 111
              Top = 42
              Width = 24
              Height = 12
              Caption = '/100'
            end
            object lbl89: TLabel
              Left = 111
              Top = 62
              Width = 24
              Height = 12
              Caption = '/100'
            end
            object seHeroWarrHPMPRate: TSpinEditEx
              Left = 44
              Top = 17
              Width = 63
              Height = 21
              Hint = #33521#38596'HP/MP='#20154#29289'HP/MP*'#35774#32622#20540'/100'
              MaxValue = 60000
              MinValue = 10
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 160
              OnChange = seHeroWarrHPMPRateChange
            end
            object seHeroWizardHPMPRate: TSpinEditEx
              Left = 44
              Top = 38
              Width = 63
              Height = 21
              Hint = #33521#38596'HP/MP='#20154#29289'HP/MP*'#35774#32622#20540'/100'#13#10#22914#38656#20154#29289#19982#33521#38596'HP/MP'#19968#33268#65292#21017#35774#32622#20026'100'
              MaxValue = 60000
              MinValue = 10
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              Value = 160
              OnChange = seHeroWizardHPMPRateChange
            end
            object seHeroTaosHPMPRate: TSpinEditEx
              Left = 44
              Top = 57
              Width = 63
              Height = 21
              Hint = #33521#38596'HP/MP='#20154#29289'HP/MP*'#35774#32622#20540'/100'#13#10#22914#38656#20154#29289#19982#33521#38596'HP/MP'#19968#33268#65292#21017#35774#32622#20026'100'
              MaxValue = 60000
              MinValue = 10
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 160
              OnChange = seHeroTaosHPMPRateChange
            end
          end
          object grp44: TGroupBox
            Left = 157
            Top = 158
            Width = 139
            Height = 113
            Caption = #20854#23427#35774#32622
            TabOrder = 4
            object lbl120: TLabel
              Left = 7
              Top = 90
              Width = 72
              Height = 12
              Caption = #33521#38596#22238#25910#26102#38388
            end
            object chkHeroCanUseMootebo: TCheckBox
              Left = 7
              Top = 15
              Width = 118
              Height = 17
              Hint = #27492#36873#39033#21482#36866#29992#20110#25112#22763#33521#38596
              Caption = #20801#35768#20351#29992#37326#34542#20914#25758
              TabOrder = 0
              OnClick = chkHeroCanUseMooteboClick
            end
            object chkWarrorAttack: TCheckBox
              Left = 7
              Top = 33
              Width = 126
              Height = 17
              Hint = #27809#26377#39764#27861#25110#20351#29992#39764#27861#22833#36133#26102#65292#26159#21542#20351#29992#29289#29702#25915#20987
              Caption = #36947#22763#26080#39764#27861#29289#29702#25915#20987
              TabOrder = 1
              OnClick = chkWarrorAttackClick
            end
            object chkHeroNotAvoidLastHinter: TCheckBox
              Left = 7
              Top = 50
              Width = 118
              Height = 17
              Hint = #38145#23450#25915#20987#30446#26631'A'#21518#65292#24403'B'#36817#36523#25915#20987#33521#38596#26102#65292#33521#38596#26159#21542#36530#36991'B'
              Caption = #36947#27861#19981#36530#36991#38750#30446#26631
              TabOrder = 2
              OnClick = chkHeroNotAvoidLastHinterClick
            end
            object seHeroLogonTimeMasterDie: TSpinEditEx
              Left = 84
              Top = 86
              Width = 46
              Height = 21
              Hint = #20027#20154#27515#20129#21518#65292#33521#38596#33258#21160#25910#22238#26102#38388#65281
              MaxValue = 1000000
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 20
              OnChange = seHeroLogonTimeMasterDieChange
            end
            object chkHeroStateDlgNoMove: TCheckBox
              Left = 7
              Top = 68
              Width = 122
              Height = 17
              Hint = #21246#36873#21518'NPC'#29366#24577#22270#26631#19981#20250#22240'NPC'#23545#35805#26694#32780#31227#21160#65292#26174#31034#20026#34987#36974#25377
              Caption = #33521#38596#22270#26631#19981#31227#21160
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              OnClick = chkHeroStateDlgNoMoveClick
            end
          end
          object grp56: TGroupBox
            Left = 4
            Top = 237
            Width = 147
            Height = 63
            Caption = #33521#38596#24615#33021#21442#25968
            TabOrder = 5
            object Label732: TLabel
              Left = 11
              Top = 18
              Width = 60
              Height = 12
              Caption = #20998#37197#26102#38388#65306
            end
            object Label733: TLabel
              Left = 11
              Top = 40
              Width = 60
              Height = 12
              Caption = #25191#34892#26102#38388#65306
            end
            object seHeroLimit: TSpinEditEx
              Left = 67
              Top = 13
              Width = 72
              Height = 21
              Hint = #22788#29702#33521#38596#25968#25454#20998#37197#30340#26102#38388#65288#40664#35748#20540'30'#65289
              MaxValue = 1000000
              MinValue = 5
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 3000
              OnChange = seHeroLimitChange
            end
            object seHeroRunTime: TSpinEditEx
              Left = 67
              Top = 35
              Width = 72
              Height = 21
              Hint = #27599#27425#22788#29702#33521#38596#30340#26102#38388#38388#38548','#25968#23383#36234#23567#33521#38596#36234#28789#27963' ('#24433#21709'CPU'#21344#29992')'#13#10#24314#35758#35774#20540#22823#20110'20 ('#40664#35748#20540'40)'
              MaxValue = 1000000
              MinValue = 20
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              Value = 20
              OnChange = seHeroRunTimeChange
            end
          end
          object grp57: TGroupBox
            Left = 301
            Top = 86
            Width = 135
            Height = 168
            Caption = #25112#22763#33521#38596#36873#39033
            TabOrder = 6
            object Label734: TLabel
              Left = 6
              Top = 77
              Width = 72
              Height = 12
              Caption = #25915#20987#36208#20301#20960#29575
            end
            object Label754: TLabel
              Left = 6
              Top = 100
              Width = 72
              Height = 12
              Caption = #36208#21050#26432#20301#20960#29575
            end
            object Label746: TLabel
              Left = 6
              Top = 123
              Width = 72
              Height = 12
              Caption = #21050#26432#36817#36523#20960#29575
            end
            object Label753: TLabel
              Left = 7
              Top = 18
              Width = 60
              Height = 12
              Caption = #40664#35748#25216#33021#65306
            end
            object Label624: TLabel
              Left = 6
              Top = 146
              Width = 72
              Height = 12
              Caption = #36817#36523#28872#28779#20960#29575
            end
            object seHeroWarrAttackMoveRate: TSpinEditEx
              Left = 80
              Top = 72
              Width = 46
              Height = 21
              Hint = #20540#36234#22823#65292#36208#20301#20960#29575#36234#20302#65307#36229#36807'500'#19981#36208#20301
              MaxValue = 1000
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 5
              OnChange = seHeroWarrAttackMoveRateChange
            end
            object chkHeroTargetAgainNoMove: TCheckBox
              Left = 6
              Top = 37
              Width = 118
              Height = 17
              Hint = #21246#36873#21518#33521#38596#23545#21516#19968#30446#26631#38145#23450#27425#25968#20026#21452#25968#26102#21017#25915#20987#26102#19981#36208#20301#65292#38145#23450#27425#25968#20026#21333#25968#21017#36208#20301
              Caption = #20004#27425#38145#23450#19981#36208#20301
              TabOrder = 1
              OnClick = chkHeroTargetAgainNoMoveClick
            end
            object chkHeroTargetAgainNoMoveDF: TCheckBox
              Left = 6
              Top = 55
              Width = 119
              Height = 17
              Hint = #21246#36873#20108#27425#38145#23450#21518#35813#36873#39033#21017#29983#25928#20026#21487#36873#29366#24577#12289#19981#21246#36873#21017#26080#25928
              Caption = #20108#27425#38145#23450#36947#27861#26377#25928
              TabOrder = 2
              OnClick = chkHeroTargetAgainNoMoveDFClick
            end
            object seHeroWarrAttacSkillErgumRate: TSpinEditEx
              Left = 80
              Top = 95
              Width = 46
              Height = 21
              Hint = #20540#36234#22823#65292#36208#21050#26432#20301#20960#29575#36234#23567
              MaxValue = 1000
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 5
              OnChange = seHeroWarrAttacSkillErgumRateChange
            end
            object seHeroWarrAttakNear: TSpinEditEx
              Left = 80
              Top = 118
              Width = 46
              Height = 21
              Hint = #25112#22763#22312#21050#26432#20301#25915#20987#26102#36817#36523#20960#29575#65292#20540#36234#22823#65292#36817#36523#20960#29575#36234#23567
              MaxValue = 1000
              MinValue = 2
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              Value = 5
              OnChange = seHeroWarrAttakNearChange
            end
            object cbbHeroWarriorDefaultSkill: TComboBox
              Left = 60
              Top = 14
              Width = 66
              Height = 20
              Style = csDropDownList
              ItemIndex = 0
              TabOrder = 5
              Text = #26222#36890#25915#20987
              OnChange = cbbHeroWarriorDefaultSkillChange
              Items.Strings = (
                #26222#36890#25915#20987
                #20992#20992#21050#26432
                #21322#26376#24367#20992)
            end
            object seHeroWarrNearFireSword: TSpinEditEx
              Left = 80
              Top = 141
              Width = 46
              Height = 21
              Hint = #25112#22763#22312'2'#26684#21450#20197#22806#30340#36317#31163#25915#20987#30446#26631#19988#28872#28779#23601#32490#26102#65292#26377#19968#23450#30340#20960#29575#36817#36523#65292#25968#23383#36234#22823#65292#20960#29575#36234#20302
              MaxValue = 1000
              MinValue = 2
              ParentShowHint = False
              ShowHint = True
              TabOrder = 6
              Value = 5
              OnChange = seHeroWarrNearFireSwordChange
            end
          end
          object seHeroTargetRangeLimit: TSpinEditEx
            Left = 260
            Top = 276
            Width = 46
            Height = 21
            MaxValue = 30
            MinValue = 0
            ParentShowHint = False
            ShowHint = False
            TabOrder = 7
            Value = 20
            OnClick = seHeroTargetRangeLimitClick
          end
        end
        object ts13: TTabSheet
          Caption = #32463#39564#35774#32622
          ImageIndex = 3
          object GroupBox198: TGroupBox
            Left = 4
            Top = 3
            Width = 176
            Height = 278
            Caption = #21319#32423#32463#39564
            TabOrder = 0
            object Label647: TLabel
              Left = 7
              Top = 258
              Width = 30
              Height = 12
              Caption = #35745#21010':'
            end
            object GridLevelExp: TStringGrid
              Left = 7
              Top = 16
              Width = 160
              Height = 233
              ColCount = 2
              DefaultColWidth = 68
              DefaultRowHeight = 18
              RowCount = 1
              FixedRows = 0
              Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
              TabOrder = 0
              OnSetEditText = GridLevelExpSetEditText
              ColWidths = (
                68
                68)
              RowHeights = (
                18)
            end
            object ComboBoxLevelExp: TComboBox
              Left = 44
              Top = 253
              Width = 125
              Height = 20
              Style = csDropDownList
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 1
              OnClick = ComboBoxLevelExpClick
            end
          end
          object GroupBox199: TGroupBox
            Left = 192
            Top = 122
            Width = 170
            Height = 78
            Caption = #26368#39640#31561#32423#38480#21046
            TabOrder = 2
            object Label648: TLabel
              Left = 8
              Top = 24
              Width = 54
              Height = 12
              Caption = #26368#39640#31561#32423':'
            end
            object Label649: TLabel
              Left = 8
              Top = 48
              Width = 54
              Height = 12
              Caption = #26432#24618#32463#39564':'
            end
            object seHeroHighLevel: TSpinEditEx
              Left = 64
              Top = 20
              Width = 89
              Height = 21
              MaxValue = 2100000000
              MinValue = 0
              TabOrder = 0
              Value = 100000000
              OnChange = seHeroHighLevelChange
            end
            object seHeroHighLevelGetExp: TSpinEditEx
              Left = 64
              Top = 46
              Width = 89
              Height = 21
              MaxValue = 2100000000
              MinValue = 0
              TabOrder = 1
              Value = 1000000
              OnChange = seHeroHighLevelGetExpChange
            end
          end
          object GroupBox200: TGroupBox
            Left = 192
            Top = 206
            Width = 170
            Height = 49
            Caption = '1000'#32423#20197#21518#21152#22266#23450#21319#32423#32463#39564
            TabOrder = 3
            object Label650: TLabel
              Left = 8
              Top = 25
              Width = 54
              Height = 12
              Caption = #21319#32423#32463#39564':'
            end
            object seHeroLevel1000FixedExp: TSpinEditLongWord
              Left = 64
              Top = 20
              Width = 89
              Height = 21
              Hint = #20197'1000'#32423#32463#39564#20026#22522#30784#65292#27599#21319#19968#32423#21319#32423#32463#39564#22686#21152#19968#20010#22266#23450#20540
              MaxValue = 0
              MinValue = 0
              TabOrder = 0
              Value = 1000000
              OnChange = seHeroLevel1000FixedExpChange
            end
          end
          object grp37: TGroupBox
            Left = 192
            Top = 3
            Width = 170
            Height = 110
            Caption = #32463#39564#20998#37197
            TabOrder = 1
            object Label150: TLabel
              Left = 8
              Top = 64
              Width = 108
              Height = 12
              Caption = #33521#38596#33719#21462#26432#24618#32463#39564#65306
            end
            object Label127: TLabel
              Left = 8
              Top = 86
              Width = 108
              Height = 12
              Caption = #33521#38596#33719#21462#20854#20182#32463#39564#65306
            end
            object lbl90: TLabel
              Left = 154
              Top = 64
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label639: TLabel
              Left = 154
              Top = 86
              Width = 6
              Height = 12
              Caption = '%'
            end
            object chkHeroGetAllExp: TCheckBox
              Left = 8
              Top = 18
              Width = 158
              Height = 17
              Hint = #24403#21246#36873#21518#65292#20154#29289#21644#33521#38596#22343#33719#24471#24618#29289#30340#20840#37096#32463#39564#20540#65292#21542#21017#25353#26432#24618#32463#39564#27604#20363#20998#37197
              Caption = #20154#29289#33521#38596#33719#21462#20840#26432#24618#32463#39564
              TabOrder = 0
              OnClick = chkHeroGetAllExpClick
            end
            object seHeroKillMonExpRate: TSpinEditEx
              Left = 111
              Top = 59
              Width = 41
              Height = 21
              Hint = #22914#20154#29289#33719#21462#20840#37096#32463#39564#19981#21246#36873#65292#21017#26432#24618#32463#39564#20154#29289#21644#33521#38596#25353#27604#20363#20998#37197#65292#21246#36873#21518#21017#20026#33521#38596#33719#24471#24618#29289#30340#32463#39564#27604#20363
              MaxValue = 10000000
              MinValue = 1
              TabOrder = 1
              Value = 1
              OnChange = seHeroKillMonExpRateChange
            end
            object seHeroNotKillMonExpRate: TSpinEditEx
              Left = 111
              Top = 81
              Width = 41
              Height = 21
              Hint = #21253#25324#33050#26412#25110#20854#20182#26041#24335#32473#20104#30340#32463#39564
              MaxValue = 10000000
              MinValue = 1
              TabOrder = 2
              Value = 1
              OnChange = seHeroNotKillMonExpRateChange
            end
            object chkHumanGetAllExp: TCheckBox
              Left = 8
              Top = 38
              Width = 155
              Height = 17
              Hint = #21246#36873#21518#20154#29289#23558#33719#24471#24618#29289#20840#37096#32463#39564#65292#33521#38596#21017#25353#24618#29289#32463#39564#30334#20998#27604#35745#31639
              Caption = #20154#29289#33719#21462#20840#37096#32463#39564
              TabOrder = 3
              OnClick = chkHumanGetAllExpClick
            end
          end
        end
        object tsHeroExt: TTabSheet
          Caption = #25193#23637#35774#32622
          ImageIndex = 4
          object chkHeroForcePeaceMode: TCheckBox
            Left = 8
            Top = 3
            Width = 131
            Height = 17
            Hint = #21246#36873#21518#65292#33521#38596#23558#19968#30452#20445#25345#22312#21644#24179#27169#24335#65292#19981#20250#38543#20027#20154#25915#20987#27169#24335#21464#21270#32780#25913#21464
            Caption = #24378#21046#33521#38596#21644#24179#27169#24335
            ParentShowHint = False
            ShowHint = True
            TabOrder = 0
            OnClick = chkHeroForcePeaceModeClick
          end
          object grp71: TGroupBox
            Left = 8
            Top = 94
            Width = 161
            Height = 68
            Caption = #33521#38596#23041#21147
            TabOrder = 1
            object lbl133: TLabel
              Left = 8
              Top = 20
              Width = 60
              Height = 12
              Caption = #25915#20987#20154#29289#65306
            end
            object lbl134: TLabel
              Left = 144
              Top = 20
              Width = 6
              Height = 12
              Caption = '%'
            end
            object Label651: TLabel
              Left = 8
              Top = 44
              Width = 60
              Height = 12
              Caption = #25915#20987#24618#29289#65306
            end
            object Label791: TLabel
              Left = 144
              Top = 44
              Width = 6
              Height = 12
              Caption = '%'
            end
            object seHeroAttackHumPowerRate: TSpinEditEx
              Left = 64
              Top = 16
              Width = 76
              Height = 21
              Hint = #20154#29289#65306#21253#21547#29609#23478#12289#33521#38596#12289#20551#20154#21450#23545#24212#30340#23453#23453
              MaxValue = 100000
              MinValue = 0
              TabOrder = 0
              Value = 100
              OnChange = seHeroAttackHumPowerRateChange
            end
            object seHeroAttackMonPowerRate: TSpinEditEx
              Left = 64
              Top = 40
              Width = 76
              Height = 21
              MaxValue = 100000
              MinValue = 0
              TabOrder = 1
              Value = 100
              OnChange = seHeroAttackMonPowerRatehange
            end
          end
          object grp72: TGroupBox
            Left = 8
            Top = 24
            Width = 161
            Height = 65
            Caption = #33521#38596#27169#24335#25903#25345
            TabOrder = 2
            object chkHeroStatus0: TCheckBox
              Left = 8
              Top = 18
              Width = 70
              Height = 17
              Caption = #25915#20987#27169#24335
              TabOrder = 0
              OnClick = chkHeroStatus0Click
            end
            object chkHeroStatus1: TCheckBox
              Tag = 1
              Left = 8
              Top = 39
              Width = 70
              Height = 17
              Caption = #36319#38543#27169#24335
              TabOrder = 1
              OnClick = chkHeroStatus1Click
            end
            object chkHeroStatus2: TCheckBox
              Tag = 2
              Left = 88
              Top = 18
              Width = 70
              Height = 17
              Caption = #20241#24687#27169#24335
              TabOrder = 2
              OnClick = chkHeroStatus2Click
            end
            object chkHeroStatus3: TCheckBox
              Tag = 3
              Left = 88
              Top = 41
              Width = 70
              Height = 17
              Hint = #21246#36873#21518#33521#38596#23558#21644#20027#20154#25915#20987#21516#19968#30446#26631
              Caption = #32479#19968#25915#20987
              TabOrder = 3
              OnClick = chkHeroStatus3Click
            end
          end
          object grp73: TGroupBox
            Left = 8
            Top = 168
            Width = 161
            Height = 45
            Caption = #24618#29289#25915#20987#33521#38596#23041#21147
            TabOrder = 3
            object Label792: TLabel
              Left = 8
              Top = 20
              Width = 60
              Height = 12
              Caption = #25915#20987#23041#21147#65306
            end
            object Label793: TLabel
              Left = 144
              Top = 20
              Width = 6
              Height = 12
              Caption = '%'
            end
            object seMonAttackHeroPowerRate: TSpinEditEx
              Left = 64
              Top = 16
              Width = 76
              Height = 21
              MaxValue = 100000
              MinValue = 0
              TabOrder = 0
              Value = 100
              OnChange = seMonAttackHeroPowerRateChange
            end
          end
          object GroupBox204: TGroupBox
            Left = 8
            Top = 218
            Width = 161
            Height = 45
            Caption = #20154#29289#25915#20987#33521#38596#23041#21147
            TabOrder = 4
            object Label169: TLabel
              Left = 8
              Top = 20
              Width = 60
              Height = 12
              Caption = #25915#20987#23041#21147#65306
            end
            object Label171: TLabel
              Left = 144
              Top = 20
              Width = 6
              Height = 12
              Caption = '%'
            end
            object seHumanAttackHeroPowerRate: TSpinEditEx
              Left = 64
              Top = 16
              Width = 76
              Height = 21
              MaxValue = 100000
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 100
              OnChange = seHumanAttackHeroPowerRateChange
            end
          end
          object GroupBox52: TGroupBox
            Left = 175
            Top = 23
            Width = 169
            Height = 242
            Caption = #36305#27493#31359#20154#25511#21046
            TabOrder = 5
            object chkDisHeroRun: TCheckBox
              Left = 4
              Top = 17
              Width = 79
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#33521#38596#23558#19981#20801#35768#31359#36807#24618#29289#25110#20854#23427#20154#29289
              Caption = #31105#27490#36305#27493#31359#20154
              TabOrder = 0
              OnClick = chkDisHeroRunClick
            end
            object chkHeroRunMon: TCheckBox
              Left = 20
              Top = 55
              Width = 99
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#33521#38596#23558#21487#20197#31359#36807#24618#29289
              Caption = #20801#35768#31359#36807#24618#29289
              TabOrder = 1
              OnClick = chkHeroRunMonClick
            end
            object chkHeroRunNpc: TCheckBox
              Left = 20
              Top = 74
              Width = 99
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#33521#38596#23558#21487#20197#31359#36807'NPC'
              Caption = #20801#35768#31359#36807'NPC'
              TabOrder = 2
              OnClick = chkHeroRunNpcClick
            end
            object chkHeroRunGuard: TCheckBox
              Left = 20
              Top = 93
              Width = 99
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#33521#38596#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
              Caption = #20801#35768#31359#36807#23432#21355
              TabOrder = 3
              OnClick = chkHeroRunGuardClick
            end
            object chkHeroSafeArea: TCheckBox
              Left = 20
              Top = 112
              Width = 102
              Height = 17
              Caption = #23433#20840#21306#19981#21463#25511#21046
              TabOrder = 4
              OnClick = chkHeroSafeAreaClick
            end
            object chkHeroSafeAreaDisNpcRun: TCheckBox
              Left = 20
              Top = 169
              Width = 125
              Height = 17
              Caption = #23433#20840#21306#31105#27490#31359'NPC'
              TabOrder = 5
              OnClick = chkHeroSafeAreaDisNpcRunClick
            end
            object chkSafeAreaDisShopStallHeroRun: TCheckBox
              Left = 20
              Top = 188
              Width = 142
              Height = 17
              Caption = #23433#20840#21306#31105#27490#31359#25670#25674#20154#29289
              TabOrder = 6
              OnClick = chkSafeAreaDisShopStallHeroRunClick
            end
            object chkSafeAreaDisOffLineHeroRun: TCheckBox
              Left = 20
              Top = 207
              Width = 142
              Height = 17
              Caption = #23433#20840#21306#31105#27490#31359#31163#32447#20154#29289
              TabOrder = 7
              OnClick = chkSafeAreaDisOffLineHeroRunClick
            end
            object chkHeroWarDisHumRun: TCheckBox
              Left = 20
              Top = 131
              Width = 117
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#22312#25915#22478#21306#22495#65292#23558#31105#27490#31359#20154#21450#24618#29289
              Caption = #25915#22478#21306#22495#20840#37096#31105#27490
              TabOrder = 8
              OnClick = chkHeroWarDisHumRunClick
            end
            object chkHeroWarHreoRun: TCheckBox
              Left = 20
              Top = 150
              Width = 125
              Height = 17
              Hint = #25171#24320#35813#39033#21518#25915#22478#21306#22495'['#25915#22478#26399#38388']'#23558#20801#35768#31359#36807#33521#38596
              Caption = #25915#22478#21306#22495#20801#35768#31359#33521#38596
              Enabled = False
              TabOrder = 9
              OnClick = chkHeroWarHreoRunClick
            end
            object chkHeroRunHum: TCheckBox
              Left = 20
              Top = 36
              Width = 98
              Height = 17
              Hint = #25171#24320#27492#21151#33021#21518#65292#33521#38596#23558#21487#20197#31359#36807#20854#20182#20154#29289
              Caption = #20801#35768#31359#36807#20154#29289
              TabOrder = 10
              OnClick = chkHeroRunHumClick
            end
          end
          object chkDisableMonsterAttackHero: TCheckBox
            Left = 8
            Top = 270
            Width = 121
            Height = 17
            Hint = #20154#29289'/'#33521#38596'/'#23453#23453#38500#22806#30340#24618#29289
            Caption = #31105#27490#24618#29289#25915#20987#33521#38596
            TabOrder = 6
            OnClick = chkDisableMonsterAttackHeroClick
          end
          object chkDisableHeroAttackMonster: TCheckBox
            Left = 8
            Top = 288
            Width = 121
            Height = 17
            Hint = #20154#29289'/'#33521#38596'/'#23453#23453#38500#22806#30340#24618#29289
            Caption = #31105#27490#33521#38596#25915#20987#24618#29289
            TabOrder = 7
            OnClick = chkDisableHeroAttackMonsterClick
          end
          object chkHeroKillMonTrigger: TCheckBox
            Left = 179
            Top = 271
            Width = 97
            Height = 17
            Hint = #21246#36873#21518#33521#38596#26432#27515#24618#29289#35302#21457'QF'#20013' @HeroKillMon'#23383#27573
            Caption = #33521#38596#26432#24618#35302#21457
            TabOrder = 8
            OnClick = chkHeroKillMonTriggerClick
          end
        end
        object TabSheet46: TTabSheet
          Caption = #25193#23637#35774#32622#20108
          ImageIndex = 5
          object GroupBox225: TGroupBox
            Left = 10
            Top = 10
            Width = 207
            Height = 151
            Caption = #33521#38596#33539#22260#35774#32622
            TabOrder = 0
            object Label942: TLabel
              Left = 8
              Top = 20
              Width = 84
              Height = 12
              Caption = #23432#25252#31354#38477#33539#22260#65306
            end
            object Label944: TLabel
              Left = 8
              Top = 44
              Width = 84
              Height = 12
              Caption = #38145#23450#31354#38477#33539#22260#65306
            end
            object Label943: TLabel
              Left = 8
              Top = 68
              Width = 84
              Height = 12
              Caption = #21512#20987#31354#38477#33539#22260#65306
            end
            object seHeroProtectFlyRange: TSpinEditEx
              Left = 94
              Top = 16
              Width = 76
              Height = 21
              Hint = #20154#29289#65306#21253#21547#29609#23478#12289#33521#38596#12289#20551#20154#21450#23545#24212#30340#23453#23453
              MaxValue = 255
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seHeroProtectFlyRangeChange
            end
            object seHeroLockFlyRange: TSpinEditEx
              Left = 94
              Top = 40
              Width = 76
              Height = 21
              MaxValue = 255
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seHeroLockFlyRangeChange
            end
            object seHeroJoinAttackFlyRange: TSpinEditEx
              Left = 94
              Top = 64
              Width = 76
              Height = 21
              MaxValue = 255
              MinValue = 1
              TabOrder = 2
              Value = 100
              OnChange = seHeroJoinAttackFlyRangeChange
            end
          end
        end
      end
    end
    object TabSheet31: TTabSheet
      Caption = #33073#26426#30331#24405
      ImageIndex = 14
      object GroupBox81: TGroupBox
        Left = 8
        Top = 8
        Width = 185
        Height = 89
        Caption = #33073#26426#30331#24405#35774#32622
        TabOrder = 0
        object lbl132: TLabel
          Left = 139
          Top = 43
          Width = 36
          Height = 12
          Caption = #23433#20840#21306
        end
        object CheckBoxOffLineLoginSafeArea: TCheckBox
          Left = 8
          Top = 17
          Width = 121
          Height = 17
          Caption = #21482#19978#32447#23433#20840#21306#20154#29289
          TabOrder = 0
          OnClick = CheckBoxOffLineLoginSafeAreaClick
        end
        object RadioButtonOffLineLoginMapName1: TRadioButton
          Left = 8
          Top = 41
          Width = 90
          Height = 17
          Caption = #33073#26426#25346#22312#22320#22270
          TabOrder = 1
          OnClick = RadioButtonOffLineLoginMapName1Click
        end
        object RadioButtonOffLineLoginMapName2: TRadioButton
          Left = 8
          Top = 65
          Width = 129
          Height = 17
          Caption = #33073#26426#25346#22312#20154#29289#22238#22478#28857
          TabOrder = 2
          OnClick = RadioButtonOffLineLoginMapName2Click
        end
        object edtSetOffLineLoginMapName: TEdit
          Left = 100
          Top = 39
          Width = 37
          Height = 20
          Hint = #21442#25968#20026#22320#22270#25991#20214#21517#23383#65292#27604#22914#22303#22478'3.map'#65292#22635#20889'3'#21363#21487
          TabOrder = 3
          Text = '3'
          OnChange = edtSetOffLineLoginMapNameChange
        end
      end
      object ButtonOffLineSave: TButton
        Left = 376
        Top = 291
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonOffLineSaveClick
      end
      object grp61: TGroupBox
        Left = 8
        Top = 102
        Width = 185
        Height = 41
        Caption = #24618#29289#25915#20987#35774#32622
        TabOrder = 2
        object chkMonNoAttackOffLinePlayer: TCheckBox
          Left = 8
          Top = 17
          Width = 137
          Height = 17
          Caption = #24618#29289#19981#25915#20987#33073#26426#20154#29289
          TabOrder = 0
          OnClick = chkMonNoAttackOffLinePlayerClick
        end
      end
    end
    object TabSheet41: TTabSheet
      Caption = #20010#20154#21830#24215
      ImageIndex = 15
      object GroupBox102: TGroupBox
        Left = 8
        Top = 8
        Width = 249
        Height = 89
        Caption = #40664#35748#24215#38138#35774#32622
        TabOrder = 0
        object Label219: TLabel
          Left = 8
          Top = 20
          Width = 168
          Height = 12
          Caption = #24215#38138#20013#20801#35768#23384#25918#30340#26368#39640#29289#21697#25968#65306
        end
        object Label220: TLabel
          Left = 8
          Top = 44
          Width = 168
          Height = 12
          Caption = #20179#24211#20013#20801#35768#23384#25918#30340#26368#39640#29289#21697#25968#65306
        end
        object EditMaxMyShopSellingItemCount: TSpinEditEx
          Left = 176
          Top = 16
          Width = 55
          Height = 21
          Hint = 
            #27492#21442#25968#20026#40664#35748#24215#38138#20801#35768#21516#26102#20986#21806#30340#29289#21697#25968#37327#13#10#13#10#20026#38450#27490#20010#20154#21830#24215#19978#26550#19968#20123#22403#22334#29289#21697#65292#23548#33268#20010#20154#21830#24215#25972#20307#29289#21697#36807#22810#65292#24433#21709#25928#29575#65292#26412#21442#25968#26368#22823#38480#21046 +
            '50'
          MaxValue = 50
          MinValue = 0
          TabOrder = 0
          Value = 10
          OnChange = EditMaxMyShopSellingItemCountChange
        end
        object EditMaxMyShopStorageItemCount: TSpinEditEx
          Left = 176
          Top = 40
          Width = 55
          Height = 21
          Hint = 
            #27492#21442#25968#20026#40664#35748#24215#38138#20179#24211#20013#20801#35768#23384#25918#29289#21697#25968#37327#13#10#13#10#20026#38450#27490#20010#20154#21830#24215#19978#26550#19968#20123#22403#22334#29289#21697#65292#23548#33268#20010#20154#21830#24215#25972#20307#29289#21697#36807#22810#65292#24433#21709#25928#29575#65292#26412#21442#25968#26368#22823#38480#21046 +
            '50'
          MaxValue = 50
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = EditMaxMyShopStorageItemCountChange
        end
        object CheckBoxOfflineCloseMyShop: TCheckBox
          Left = 7
          Top = 66
          Width = 145
          Height = 17
          Hint = #21830#38138#20851#38381#21518#65292#26080#27861#36141#20080#35813#21830#38138#30340#29289#21697#65292#26080#27861#25628#32034#21040#35813#29289#21697#12290#31163#32447#25346#26426#20154#29289#19981#20250#20851#38381#12290
          Caption = #20154#29289#19979#32447#20851#38381#20010#20154#21830#24215
          TabOrder = 2
          OnClick = CheckBoxOfflineCloseMyShopClick
        end
        object chkProhibitModifyPrices: TCheckBox
          Left = 152
          Top = 66
          Width = 94
          Height = 17
          Hint = #21830#38138#20851#38381#21518#65292#26080#27861#36141#20080#35813#21830#38138#30340#29289#21697#65292#26080#27861#25628#32034#21040#35813#29289#21697#12290#31163#32447#25346#26426#20154#29289#19981#20250#20851#38381#12290
          Caption = #31105#27490#20462#25913#20215#26684
          TabOrder = 3
          OnClick = chkProhibitModifyPricesClick
        end
      end
      object ButtonMyShopSave: TButton
        Left = 376
        Top = 327
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 5
        OnClick = ButtonMyShopSaveClick
      end
      object RadioGroupShopType: TRadioGroup
        Left = 8
        Top = 104
        Width = 112
        Height = 57
        Hint = 
          #40664#35748#24215#38138#65306#22320#22270#21442#25968#24517#39035#22686#21152'ALLOWUSEMYSHOP'#25165#21487#20197#36827#34892#36141#20080#21644#21462#22238#13#10#20223'HeroM2'#25670#25674#65306#22320#22270#21442#25968#22686#21152'ALLOWUSEM' +
          'YSHOP'#29992#20110#25351#23450#22320#22270#25670#25674#35774#32622
        Caption = #24215#38138#31867#22411
        Items.Strings = (
          #40664#35748#24215#38138
          #20223'HeroM2'#25670#25674)
        TabOrder = 2
        OnClick = RadioGroupShopTypeClick
      end
      object grp2: TGroupBox
        Left = 8
        Top = 253
        Width = 249
        Height = 65
        Caption = #20179#24211#35774#32622
        TabOrder = 4
        object lblInfinityStorageCount: TLabel
          Left = 8
          Top = 42
          Width = 96
          Height = 12
          Caption = #40664#35748#21487#23384'44'#20214#29289#21697
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = []
          ParentFont = False
        end
        object edtInfinityStorageCount: TSpinEditEx
          Left = 194
          Top = 18
          Width = 48
          Height = 21
          Hint = #26080#38480#20179#24211#23481#37327#65292#20154#29289#23454#38469#20179#24211#23481#37327'='#26222#36890#20179#24211#23481#37327'+'#26080#38480#20179#24211#23481#37327'    '#13#10#21442#25968#23454#26102#29983#25928#65292#27491#24120#28216#25103#20013#20943#23569#20179#24211#23481#37327#21487#33021#23548#33268#20179#24211#29289#21697#20002#22833
          MaxValue = 65535
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = edtInfinityStorageCountChange
        end
        object chkInfinityStorage: TCheckBox
          Left = 8
          Top = 18
          Width = 177
          Height = 17
          Caption = #20801#35768#20179#24211#25193#23637'---->'#25193#23637#25968#37327#65306
          TabOrder = 0
          OnClick = chkInfinityStorageClick
        end
      end
      object GroupBox57: TGroupBox
        Left = 266
        Top = 10
        Width = 175
        Height = 115
        Caption = #20223'HeroM2'#25670#25674#36873#39033
        TabOrder = 1
        object chkOpenSelfShop: TCheckBox
          Left = 7
          Top = 17
          Width = 104
          Height = 16
          Hint = #26159#21542#24320#21551#26381#21153#22120#20223'HeroM2'#25670#25674#21151#33021#12290
          Caption = #24320#21551#25670#25674#21151#33021
          TabOrder = 0
          OnClick = chkOpenSelfShopClick
        end
        object chkSafeZoneShop: TCheckBox
          Left = 7
          Top = 36
          Width = 144
          Height = 16
          Hint = #36873#20013#35813#39033#65292#21482#26377#22312#23433#20840#21306#25165#20801#35768#25670#25674#12290
          Caption = #21482#20801#35768#22312#23433#20840#21306#20869#25670#25674
          TabOrder = 1
          OnClick = chkSafeZoneShopClick
        end
        object chkMapShop: TCheckBox
          Left = 7
          Top = 56
          Width = 160
          Height = 16
          Hint = #36873#20013#35813#39033#65292#21482#26377#24403#22320#22270#26631#24535' ALLOWUSEMYSHOP '#25171#24320#26102#25165#20801#35768#22312#35813#22320#22270#25670#25674#12290
          Caption = #21482#20801#35768#22312#25351#23450#22320#22270#20869#25670#25674
          TabOrder = 2
          OnClick = chkMapShopClick
        end
        object chkShopStallCanNotAttack: TCheckBox
          Left = 7
          Top = 75
          Width = 123
          Height = 16
          Hint = #38450#27490#25670#25674#26399#38388#20986#29616#34987#24694#24847#26432#23475
          Caption = #25670#25674#26399#38388#26080#25932#27169#24335
          TabOrder = 3
          OnClick = chkShopStallCanNotAttackClick
        end
        object chkShopHeadPic: TCheckBox
          Left = 7
          Top = 94
          Width = 162
          Height = 16
          Caption = #26174#31034'"'#20010#20154#21830#24215'"'#22270#26631
          TabOrder = 4
          OnClick = chkShopHeadPicClick
        end
      end
      object GroupBox48: TGroupBox
        Left = 127
        Top = 104
        Width = 130
        Height = 142
        Caption = #24215#38138#20840#23616#31246#25910#25511#21046
        TabOrder = 3
        object Label195: TLabel
          Left = 15
          Top = 23
          Width = 60
          Height = 12
          Caption = #37329#24065#31246#25910#65306
        end
        object Label196: TLabel
          Left = 117
          Top = 23
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label562: TLabel
          Left = 15
          Top = 47
          Width = 60
          Height = 12
          Caption = #20803#23453#31246#25910#65306
        end
        object Label563: TLabel
          Left = 117
          Top = 47
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label718: TLabel
          Left = 3
          Top = 71
          Width = 72
          Height = 12
          Caption = #37329#21018#30707#31246#25910#65306
        end
        object Label719: TLabel
          Left = 117
          Top = 71
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label720: TLabel
          Left = 15
          Top = 96
          Width = 60
          Height = 12
          Caption = #28789#31526#31246#25910#65306
        end
        object Label721: TLabel
          Left = 117
          Top = 96
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label722: TLabel
          Left = 3
          Top = 120
          Width = 72
          Height = 12
          Caption = #28216#25103#28857#31246#25910#65306
        end
        object Label723: TLabel
          Left = 117
          Top = 120
          Width = 6
          Height = 12
          Caption = '%'
        end
        object seSellOffGoldTaxRate: TSpinEditEx
          Left = 73
          Top = 19
          Width = 41
          Height = 21
          Hint = #24403#29609#23478#36890#36807#40664#35748#24215#38138#25110#25670#25674#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seSellOffGoldTaxRateChange
        end
        object seSellOffGameGoldTaxRate: TSpinEditEx
          Left = 73
          Top = 43
          Width = 41
          Height = 21
          Hint = #24403#29609#23478#36890#36807#40664#35748#24215#38138#25110#25670#25674#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#20803#23453#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seSellOffGameGoldTaxRateChange
        end
        object seSellOffGameDiamondTaxRate: TSpinEditEx
          Left = 73
          Top = 67
          Width = 41
          Height = 21
          Hint = #24403#29609#23478#36890#36807#40664#35748#24215#38138#25110#25670#25674#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#21018#30707#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 2
          Value = 0
          OnChange = seSellOffGameDiamondTaxRateChange
        end
        object seSellOffGameGirdTaxRate: TSpinEditEx
          Left = 73
          Top = 92
          Width = 41
          Height = 21
          Hint = #24403#29609#23478#36890#36807#40664#35748#24215#38138#25110#25670#25674#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#28789#31526#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 3
          Value = 0
          OnChange = seSellOffGameGirdTaxRateChange
        end
        object seSellOffGamePointTaxRate: TSpinEditEx
          Left = 73
          Top = 116
          Width = 41
          Height = 21
          Hint = #24403#29609#23478#36890#36807#40664#35748#24215#38138#25110#25670#25674#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#28216#25103#28857#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seSellOffGamePointTaxRateChange
        end
      end
      object GroupBox216: TGroupBox
        Left = 267
        Top = 130
        Width = 174
        Height = 43
        Caption = #23492#21806#31995#32479#31246#25910#25511#21046
        TabOrder = 6
        object Label705: TLabel
          Left = 9
          Top = 21
          Width = 60
          Height = 12
          Caption = #20803#23453#31246#25910#65306
        end
        object Label706: TLabel
          Left = 113
          Top = 21
          Width = 24
          Height = 12
          Caption = '/100'
        end
        object seJSOfGameGoldTaxRate: TSpinEditEx
          Left = 67
          Top = 16
          Width = 42
          Height = 21
          Hint = #24403#29609#23478#36890#36807#40664#35748#24215#38138#25110#25670#25674#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#20803#23453#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seJSOfGameGoldTaxRateChange
        end
      end
      object grp54: TGroupBox
        Left = 8
        Top = 167
        Width = 112
        Height = 79
        Caption = #20801#35768#36135#24065#31867#22411
        TabOrder = 7
        object chkMyShopGold: TCheckBox
          Left = 6
          Top = 16
          Width = 45
          Height = 17
          Caption = #37329#24065
          TabOrder = 0
          OnClick = chkMyShopGoldClick
        end
        object chkMyShopGameGold: TCheckBox
          Left = 64
          Top = 16
          Width = 45
          Height = 17
          Caption = #20803#23453
          TabOrder = 1
          OnClick = chkMyShopGameGoldClick
        end
        object chkMyShopGameDiamond: TCheckBox
          Left = 6
          Top = 37
          Width = 55
          Height = 17
          Caption = #37329#21018#30707
          TabOrder = 2
          OnClick = chkMyShopGameDiamondClick
        end
        object chkMyShopGameGird: TCheckBox
          Left = 64
          Top = 37
          Width = 45
          Height = 17
          Caption = #28789#31526
          TabOrder = 3
          OnClick = chkMyShopGameGirdClick
        end
        object chkMyShopGamePoint: TCheckBox
          Left = 6
          Top = 57
          Width = 55
          Height = 17
          Caption = #28216#25103#28857
          TabOrder = 4
          OnClick = chkMyShopGamePointClick
        end
      end
      object GroupBox221: TGroupBox
        Left = 267
        Top = 178
        Width = 174
        Height = 43
        Caption = #20010#20154#21830#24215#20986#21806#29289#21697#26102#38388#38480#21046
        TabOrder = 8
        object lbl37: TLabel
          Left = 147
          Top = 21
          Width = 24
          Height = 12
          Caption = #20998#38047
        end
        object chkMySellShopItemTime: TCheckBox
          Left = 8
          Top = 18
          Width = 73
          Height = 17
          Caption = #26102#38388#38480#21046
          TabOrder = 0
          OnClick = chkMySellShopItemTimeClick
        end
        object seMySellShopItemTime: TSpinEditEx
          Left = 80
          Top = 16
          Width = 64
          Height = 21
          Hint = #20010#20154#21830#24215#20986#21806#29289#21697#36229#20986#35774#32622#26102#38388#20250#25918#20837#21040#20010#20154#21830#24215#30340#20179#24211
          MaxValue = 100000
          MinValue = 1
          TabOrder = 1
          Value = 30
          OnChange = seMySellShopItemTimeChange
        end
      end
      object grp64: TGroupBox
        Left = 267
        Top = 227
        Width = 174
        Height = 43
        Caption = #25670#25674#25991#23383#20449#24687#38271#24230
        TabOrder = 9
        object lbl119: TLabel
          Left = 8
          Top = 21
          Width = 72
          Height = 12
          Caption = #25991#23383#38271#24230#25511#21046
        end
        object seMySellShowItemNamLen: TSpinEditEx
          Left = 84
          Top = 16
          Width = 59
          Height = 21
          Hint = #25670#25674#26102#25991#23383#30340#38271#24230#65292#24403#20026'0'#26102#19981#26174#31034
          MaxValue = 30
          MinValue = 0
          TabOrder = 0
          Value = 30
          OnChange = seMySellShowItemNamLenChange
        end
      end
      object GroupBox116: TGroupBox
        Left = 267
        Top = 275
        Width = 174
        Height = 43
        Caption = #20010#20154#21830#24215#25805#20316#38388#38548
        TabOrder = 10
        object Label923: TLabel
          Left = 9
          Top = 21
          Width = 60
          Height = 12
          Caption = #25805#20316#38388#38548#65306
        end
        object Label924: TLabel
          Left = 137
          Top = 21
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object seMyShopOperateInterval: TSpinEditEx
          Left = 63
          Top = 16
          Width = 68
          Height = 21
          MaxValue = 60000
          MinValue = 10
          TabOrder = 0
          Value = 10
          OnChange = seMyShopOperateIntervalChange
        end
      end
    end
    object TabSheet49: TTabSheet
      Caption = #20854#23427#25511#21046
      ImageIndex = 17
      object pgcBonusAbilof: TPageControl
        Left = 0
        Top = 0
        Width = 449
        Height = 366
        ActivePage = ts6
        Align = alClient
        TabOrder = 0
        object ts6: TTabSheet
          Caption = #20854#20182#25511#21046#19968
          object Label581: TLabel
            Left = 64
            Top = 236
            Width = 144
            Height = 12
            Caption = #28857#24184#36816#26102#20154#29289#26368#20339#25915#20987#29366#24577
          end
          object lbl99: TLabel
            Left = 8
            Top = 284
            Width = 84
            Height = 12
            Caption = #33050#26412#24490#29615#27425#25968#65306
          end
          object ButtonOther: TButton
            Left = 358
            Top = 304
            Width = 75
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 11
            OnClick = ButtonOtherClick
          end
          object GroupBox124: TGroupBox
            Left = 8
            Top = 72
            Width = 201
            Height = 41
            TabOrder = 2
            object Label250: TLabel
              Left = 8
              Top = 20
              Width = 36
              Height = 12
              Caption = #26174#31034#65306
            end
            object EditMysteriousManName: TEdit
              Left = 44
              Top = 16
              Width = 147
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 1
              OnChange = EditMysteriousManNameChange
            end
            object chkShowMysteriousMan: TCheckBox
              Left = 9
              Top = -2
              Width = 107
              Height = 17
              Hint = #21246#36873#21518#25140#26007#31520#21518#26174#31034#31070#31192#20154'[GM'#26080#25928']'
              Caption = #26159#21542#26174#31034#31070#31192#20154
              Checked = True
              State = cbChecked
              TabOrder = 0
              OnClick = chkShowMysteriousManClick
            end
          end
          object GroupBox178: TGroupBox
            Left = 8
            Top = 8
            Width = 201
            Height = 61
            Caption = #35013#22791#25481#25345#20037#20493#25968
            TabOrder = 0
            object Label567: TLabel
              Left = 8
              Top = 20
              Width = 36
              Height = 12
              Caption = #20493#25968#65306
            end
            object Label568: TLabel
              Left = 96
              Top = 20
              Width = 24
              Height = 12
              Caption = '/100'
            end
            object EditDamageItemDuraRate: TSpinEditEx
              Left = 44
              Top = 15
              Width = 45
              Height = 21
              Hint = #25481#25345#20037#20493#25968#65292#25968#23383#22823#23567' '#38500#20197' 100'#20026#23454#38469#20493#25968#12290
              MaxValue = 10000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditDamageItemDuraRateChange
            end
            object CheckBoxDeleteItemDuraZero: TCheckBox
              Left = 9
              Top = 37
              Width = 129
              Height = 17
              Caption = #25345#20037#20026'0'#26102#28040#22833
              TabOrder = 1
              OnClick = CheckBoxDeleteItemDuraZeroClick
            end
          end
          object GroupBox194: TGroupBox
            Left = 8
            Top = 185
            Width = 201
            Height = 38
            Caption = #34593#28891
            TabOrder = 6
            object CheckBoxDuraChangeLight: TCheckBox
              Left = 9
              Top = 14
              Width = 169
              Height = 17
              Caption = #38543#30528#25345#20037#30340#25913#21464#32780#25913#21464#20142#24230
              TabOrder = 0
              OnClick = CheckBoxDuraChangeLightClick
            end
          end
          object GroupBox196: TGroupBox
            Left = 224
            Top = 8
            Width = 161
            Height = 61
            Caption = #27602#32032#27494#22120
            TabOrder = 1
            object CheckBoxPoisonWeaponCanMagicAttack: TCheckBox
              Left = 8
              Top = 16
              Width = 97
              Height = 17
              Caption = #39764#27861#25915#20987#26377#25928
              TabOrder = 0
              OnClick = CheckBoxPoisonWeaponCanMagicAttackClick
            end
            object CheckBoxPoisonWeaponCanHitAllTarget: TCheckBox
              Left = 8
              Top = 37
              Width = 145
              Height = 17
              Hint = #21246#36873#21518#25152#26377#25915#20987#21040#30340#30446#26631#37117#26377#25928#65292#21542#21017#21482#26377#27491#23545#38754#30340#37027#20010#30446#26631#26377#25928
              Caption = #29289#29702#25915#20987#25152#26377#30446#26631#26377#25928
              TabOrder = 1
              OnClick = CheckBoxPoisonWeaponCanHitAllTargetClick
            end
          end
          object GroupBox197: TGroupBox
            Left = 224
            Top = 72
            Width = 161
            Height = 41
            Caption = #21253#35065#21047#26032#36895#24230
            TabOrder = 3
            object Label587: TLabel
              Left = 59
              Top = 20
              Width = 12
              Height = 12
              Caption = #31186
            end
            object EditQueryBagItemsTime: TSpinEditEx
              Left = 12
              Top = 15
              Width = 46
              Height = 21
              MaxValue = 10000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditQueryBagItemsTimeChange
            end
            object chkShowRefreshBagMsg: TCheckBox
              Left = 87
              Top = 17
              Width = 68
              Height = 17
              Hint = #26159#21542#24320#21551#28216#25103#20013#21047#26032#21253#35065#32842#22825#26694#25552#31034
              Caption = #21047#26032#25552#31034
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              OnClick = chkShowRefreshBagMsgClick
            end
          end
          object EditMaxLuckMaxPower: TSpinEditEx
            Left = 8
            Top = 232
            Width = 53
            Height = 21
            Hint = #35813#36873#39033#21516#26102#25511#21046#26368#22823#24184#36816#21644#26368#22823#35781#21650#20540
            MaxValue = 100
            MinValue = 1
            TabOrder = 10
            Value = 1
            OnChange = EditMaxLuckMaxPowerChange
          end
          object chkGroupReCallNotInSafeZone: TCheckBox
            Left = 224
            Top = 116
            Width = 153
            Height = 17
            Hint = #21246#36873#21518#35760#24518#22871#12289#34892#20250#20256#36865#12289#22827#22971#20256#36865#12289#24072#24466#20256#36865#23558#19981#33021#20256#36865#23433#20840#21306#20154#29289
            Caption = #31105#27490#35760#24518#20256#36865#23433#20840#21306#20154#29289
            TabOrder = 4
            OnClick = chkGroupReCallNotInSafeZoneClick
          end
          object chkNewHumanAttatckMode_HAM_PEACE: TCheckBox
            Left = 224
            Top = 135
            Width = 121
            Height = 17
            Hint = #21246#36873#21518#26032#20154#19978#32447#40664#35748#20026#21644#24179#27169#24335
            Caption = #26032#20154#21644#24179#25915#20987#27169#24335
            TabOrder = 5
            OnClick = chkNewHumanAttatckMode_HAM_PEACEClick
          end
          object chkAutoGroupMaster: TCheckBox
            Left = 224
            Top = 154
            Width = 121
            Height = 17
            Hint = #21246#36873#21518#24403#38431#38271#27515#20129#25110#32773#19979#32447#21518#33258#21160#26356#25442#38431#20237#20013#31561#32423#26368#39640#32773#20026#38431#38271
            Caption = #33258#21160#26367#25442#38431#38271
            TabOrder = 7
            OnClick = chkAutoGroupMasterClick
          end
          object chkGuardNotAttackPlayMoster: TCheckBox
            Left = 224
            Top = 173
            Width = 161
            Height = 17
            Hint = #21246#36873#21518#22823#20992#12289#23432#21355#23558#19981#20250#25915#20987#20154#24418#24618#12289#20998#36523
            Caption = #22823#20992#19981#25915#20987#20154#24418#24618#12289#20998#36523
            TabOrder = 8
            OnClick = chkGuardNotAttackPlayMosterClick
          end
          object chkWarNoDropUseItem: TCheckBox
            Left = 224
            Top = 192
            Width = 161
            Height = 17
            Hint = #21246#36873#21518#25915#22478#21306#22495#20154#29289#27515#20129#19981#29190#36523#19978#29289#21697
            Caption = #25915#22478#21306#22495#19981#25481#33853#36523#19978#29289#21697
            TabOrder = 9
            OnClick = chkWarNoDropUseItemClick
          end
          object seLimitScriptGotoCount: TSpinEditEx
            Left = 89
            Top = 280
            Width = 57
            Height = 21
            Hint = 
              #40664#35748#20540'20'#65292#24314#35758#19981#35201#36229#36807'200'#65292#24403#35774#32622#27425#25968#36739#22823#26102#65292#33050#26412#22797#26434#24230#36739#39640#25110#25191#34892#39057#29575#36739#39640#65292#21487#33021#23548#33268'M2'#21345#27515#13#10#13#10#24403#25191#34892#27425#25968#36807#22823#23548#33268'M2'#21345 +
              #27515#26102#65292#26410#36798#21040#35774#32622#30340#27425#25968#24341#25806#19981#20250#25253#27515#24490#29615#33050#26412#38169#35823#65292#32780#26080#27861'M2'#23450#20301#21345#27515#30340#21407#22240#65281#65281#65281
            MaxValue = 65535
            MinValue = 1
            TabOrder = 12
            Value = 20
            OnChange = seLimitScriptGotoCountChange
          end
          object grp69: TGroupBox
            Left = 8
            Top = 118
            Width = 201
            Height = 64
            Caption = #21560#34880#27494#22120#35774#32622
            TabOrder = 13
            object lbl39: TLabel
              Left = 8
              Top = 20
              Width = 60
              Height = 12
              Caption = #21560#34880#20493#29575#65306
            end
            object seHongMoSuiteRate: TSpinEditEx
              Left = 68
              Top = 17
              Width = 53
              Height = 21
              Hint = #21560#34880#27494#22120#27599#27425#21560#34880#37327' = '#27494#22120#37325#37327#215' '#25968#23383#22823#23567' '#247' 100 '#65288#20363#65306#25968#23383'200 = 2'#20493#65289
              MaxValue = 500000
              MinValue = 10
              TabOrder = 0
              Value = 100
              OnChange = seHongMoSuiteRateChange
            end
            object chkHongMoSuiteWithPower: TCheckBox
              Left = 8
              Top = 40
              Width = 111
              Height = 17
              Hint = #21246#36873#27492#39033#21017#25353#29031#27599#27425#23545#30446#26631#20260#23475#20540#36827#34892#21560#34880#12289#20260#23475#36234#39640#21560#34880#36234#22810#13#10#13#10#19981#21246#36873#21017#25353#29031#27494#22120#37325#37327#36827#34892#21560#34880#65292#27494#22120#36234#37325#21560#34880#36234#22810
              Caption = #25353#25915#20987#20260#23475#21560#34880
              TabOrder = 1
              OnClick = chkHongMoSuiteWithPowerClick
            end
          end
          object chkLuckUseNewAlgorism: TCheckBox
            Left = 8
            Top = 258
            Width = 161
            Height = 17
            Hint = #21246#36873#21518#65292#24184#36816#35843#25972#25915#20987#19979#38480#65292#35781#21650#35843#25972#25915#20987#19978#38480
            Caption = #24184#36816#21450#35781#21650#20351#29992#26032#31639#27861
            TabOrder = 14
            OnClick = chkLuckUseNewAlgorismClick
          end
          object chkM2CacheRankData: TCheckBox
            Left = 8
            Top = 304
            Width = 177
            Height = 17
            Hint = 
              #29992#20110'NPC'#21629#20196#65306'GetRankNameByNo'#65292'GetRankNoByName'#65292'CheckSelfRankNo'#13#10#13#10#22914#26524#27809#26377#29992 +
              #19978#38754#30340'2'#20010#21629#20196#65292#24314#35758#19981#21246#36873
            Caption = #20174'DBServer'#21047#26032#25490#21517#25968#25454
            Font.Charset = GB2312_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = [fsBold]
            ParentFont = False
            TabOrder = 15
            OnClick = chkM2CacheRankDataClick
          end
          object chkGroupUseOldMode: TCheckBox
            Left = 224
            Top = 212
            Width = 136
            Height = 17
            Hint = #21246#36873#21518#65306#22312#34987#32452#38431#30340#20154#21592#24320#32452#30340#24773#20917#19979#65292#19981#38656#35201#25509#21463#32452#38431#30830#35748#35831#27714#65292#30452#25509#23558#20154#25289#21040#38431#20237#20013
            Caption = #22797#21476#32452#38431#19981#35810#38382#27169#24335
            TabOrder = 16
            OnClick = chkGroupUseOldModeClick
          end
        end
        object TabSheet91: TTabSheet
          Caption = #20854#20182#25511#21046#20108
          ImageIndex = 1
          object Label618: TLabel
            Left = 2
            Top = 263
            Width = 240
            Height = 12
            Caption = #19978#38754#30340#21442#25968#35843#25972#21518#65292#23458#25143#31471#38656#35201#23567#36864#25165#33021#29983#25928
            Font.Charset = ANSI_CHARSET
            Font.Color = clTeal
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox54: TGroupBox
            Left = 4
            Top = 8
            Width = 140
            Height = 241
            Caption = #25112#22763#23646#24615#28040#32791#28857#25511#21046
            TabOrder = 0
            object Label569: TLabel
              Left = 14
              Top = 24
              Width = 60
              Height = 12
              Caption = #29289#29702#25915#20987#65306
            end
            object Label592: TLabel
              Left = 14
              Top = 48
              Width = 60
              Height = 12
              Caption = #39764#27861#25915#20987#65306
            end
            object Label593: TLabel
              Left = 14
              Top = 72
              Width = 60
              Height = 12
              Caption = #36947#26415#25915#20987#65306
            end
            object Label594: TLabel
              Left = 14
              Top = 96
              Width = 60
              Height = 12
              Caption = #29289#29702#38450#24481#65306
            end
            object Label595: TLabel
              Left = 14
              Top = 120
              Width = 60
              Height = 12
              Caption = #39764#27861#38450#24481#65306
            end
            object Label596: TLabel
              Left = 14
              Top = 144
              Width = 60
              Height = 12
              Caption = #29983' '#21629' '#20540#65306
            end
            object Label597: TLabel
              Left = 14
              Top = 168
              Width = 60
              Height = 12
              Caption = #39764' '#27861' '#20540#65306
            end
            object Label598: TLabel
              Left = 14
              Top = 192
              Width = 60
              Height = 12
              Caption = #20934'    '#30830#65306
            end
            object Label599: TLabel
              Left = 14
              Top = 216
              Width = 60
              Height = 12
              Caption = #25935'    '#25463#65306
            end
            object seBonusAbilofWarrDC: TSpinEditEx
              Left = 72
              Top = 20
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29289#29702#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'17'
              MaxValue = 0
              MinValue = 0
              TabOrder = 0
              Value = 17
              OnChange = seBonusAbilofWarrDCChange
            end
            object seBonusAbilofWarrMC: TSpinEditEx
              Left = 72
              Top = 44
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 100
              OnChange = seBonusAbilofWarrMCChange
            end
            object seBonusAbilofWarrSC: TSpinEditEx
              Left = 72
              Top = 68
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#36947#27861#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seBonusAbilofWarrSCChange
            end
            object seBonusAbilofWarrAC: TSpinEditEx
              Left = 72
              Top = 92
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29289#29702#38450#24481#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 100
              OnChange = seBonusAbilofWarrACChange
            end
            object seBonusAbilofWarrMAC: TSpinEditEx
              Left = 72
              Top = 116
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#38450#24481#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 100
              OnChange = seBonusAbilofWarrMACChange
            end
            object seBonusAbilofWarrHP: TSpinEditEx
              Left = 72
              Top = 140
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29983#21629#20540#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'1'
              MaxValue = 0
              MinValue = 0
              TabOrder = 5
              Value = 100
              OnChange = seBonusAbilofWarrHPChange
            end
            object seBonusAbilofWarrMP: TSpinEditEx
              Left = 72
              Top = 164
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#20540#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'3'
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 100
              OnChange = seBonusAbilofWarrMPChange
            end
            object seBonusAbilofWarrHit: TSpinEditEx
              Left = 72
              Top = 188
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#21629#20013#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 7
              Value = 100
              OnChange = seBonusAbilofWarrHitChange
            end
            object seBonusAbilofWarrSpeed: TSpinEditEx
              Left = 72
              Top = 212
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#25935#25463#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'35'
              MaxValue = 0
              MinValue = 0
              TabOrder = 8
              Value = 100
              OnChange = seBonusAbilofWarrSpeedChange
            end
          end
          object GroupBox62: TGroupBox
            Left = 150
            Top = 8
            Width = 140
            Height = 241
            Caption = #27861#24072#23646#24615#28040#32791#28857#25511#21046
            TabOrder = 1
            object Label600: TLabel
              Left = 14
              Top = 24
              Width = 60
              Height = 12
              Caption = #29289#29702#25915#20987#65306
            end
            object Label601: TLabel
              Left = 14
              Top = 48
              Width = 60
              Height = 12
              Caption = #39764#27861#25915#20987#65306
            end
            object Label602: TLabel
              Left = 14
              Top = 72
              Width = 60
              Height = 12
              Caption = #36947#26415#25915#20987#65306
            end
            object Label603: TLabel
              Left = 14
              Top = 96
              Width = 60
              Height = 12
              Caption = #29289#29702#38450#24481#65306
            end
            object Label604: TLabel
              Left = 14
              Top = 120
              Width = 60
              Height = 12
              Caption = #39764#27861#38450#24481#65306
            end
            object Label605: TLabel
              Left = 14
              Top = 144
              Width = 60
              Height = 12
              Caption = #29983' '#21629' '#20540#65306
            end
            object Label606: TLabel
              Left = 14
              Top = 168
              Width = 60
              Height = 12
              Caption = #39764' '#27861' '#20540#65306
            end
            object Label607: TLabel
              Left = 14
              Top = 192
              Width = 60
              Height = 12
              Caption = #20934'    '#30830#65306
            end
            object Label608: TLabel
              Left = 14
              Top = 216
              Width = 60
              Height = 12
              Caption = #25935'    '#25463#65306
            end
            object seBonusAbilofWizardDC: TSpinEditEx
              Left = 72
              Top = 20
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29289#29702#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'17'
              MaxValue = 0
              MinValue = 0
              TabOrder = 0
              Value = 100
              OnChange = seBonusAbilofWizardDCChange
            end
            object seBonusAbilofWizardMC: TSpinEditEx
              Left = 72
              Top = 44
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'25'
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 100
              OnChange = seBonusAbilofWizardMCChange
            end
            object seBonusAbilofWizardSC: TSpinEditEx
              Left = 72
              Top = 68
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#36947#27861#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'30'
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seBonusAbilofWizardSCChange
            end
            object seBonusAbilofWizardAC: TSpinEditEx
              Left = 72
              Top = 92
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29289#29702#38450#24481#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 100
              OnChange = seBonusAbilofWizardACChange
            end
            object seBonusAbilofWizardMAC: TSpinEditEx
              Left = 72
              Top = 116
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#38450#24481#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'15'
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 100
              OnChange = seBonusAbilofWizardMACChange
            end
            object seBonusAbilofWizardHP: TSpinEditEx
              Left = 72
              Top = 140
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29983#21629#20540#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'2'
              MaxValue = 0
              MinValue = 0
              TabOrder = 5
              Value = 100
              OnChange = seBonusAbilofWizardHPChange
            end
            object seBonusAbilofWizardMP: TSpinEditEx
              Left = 72
              Top = 164
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#20540#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'1'
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 100
              OnChange = seBonusAbilofWizardMPChange
            end
            object seBonusAbilofWizardHit: TSpinEditEx
              Left = 72
              Top = 188
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#21629#20013#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'25'
              MaxValue = 0
              MinValue = 0
              TabOrder = 7
              Value = 100
              OnChange = seBonusAbilofWizardHitChange
            end
            object seBonusAbilofWizardSpeed: TSpinEditEx
              Left = 72
              Top = 212
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#25935#25463#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'35'
              MaxValue = 0
              MinValue = 0
              TabOrder = 8
              Value = 100
              OnChange = seBonusAbilofWizardSpeedChange
            end
          end
          object GroupBox63: TGroupBox
            Left = 297
            Top = 8
            Width = 140
            Height = 241
            Caption = #36947#22763#23646#24615#28040#32791#28857#25511#21046
            TabOrder = 2
            object Label609: TLabel
              Left = 14
              Top = 24
              Width = 60
              Height = 12
              Caption = #29289#29702#25915#20987#65306
            end
            object Label610: TLabel
              Left = 14
              Top = 48
              Width = 60
              Height = 12
              Caption = #39764#27861#25915#20987#65306
            end
            object Label611: TLabel
              Left = 14
              Top = 72
              Width = 60
              Height = 12
              Caption = #36947#26415#25915#20987#65306
            end
            object Label612: TLabel
              Left = 14
              Top = 96
              Width = 60
              Height = 12
              Caption = #29289#29702#38450#24481#65306
            end
            object Label613: TLabel
              Left = 14
              Top = 120
              Width = 60
              Height = 12
              Caption = #39764#27861#38450#24481#65306
            end
            object Label614: TLabel
              Left = 14
              Top = 144
              Width = 60
              Height = 12
              Caption = #29983' '#21629' '#20540#65306
            end
            object Label615: TLabel
              Left = 14
              Top = 168
              Width = 60
              Height = 12
              Caption = #39764' '#27861' '#20540#65306
            end
            object Label616: TLabel
              Left = 14
              Top = 192
              Width = 60
              Height = 12
              Caption = #20934'    '#30830#65306
            end
            object Label617: TLabel
              Left = 14
              Top = 216
              Width = 60
              Height = 12
              Caption = #25935'    '#25463#65306
            end
            object seBonusAbilofTaosDC: TSpinEditEx
              Left = 72
              Top = 20
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29289#29702#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 0
              Value = 100
              OnChange = seBonusAbilofTaosDCChange
            end
            object seBonusAbilofTaosMC: TSpinEditEx
              Left = 72
              Top = 44
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'30'
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 100
              OnChange = seBonusAbilofTaosMCChange
            end
            object seBonusAbilofTaosSC: TSpinEditEx
              Left = 72
              Top = 68
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#36947#27861#25915#20987#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'17'
              MaxValue = 0
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seBonusAbilofTaosSCChange
            end
            object seBonusAbilofTaosAC: TSpinEditEx
              Left = 72
              Top = 92
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29289#29702#38450#24481#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'20'
              MaxValue = 0
              MinValue = 0
              TabOrder = 3
              Value = 100
              OnChange = seBonusAbilofTaosACChange
            end
            object seBonusAbilofTaosMAC: TSpinEditEx
              Left = 72
              Top = 116
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#38450#24481#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'15'
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 100
              OnChange = seBonusAbilofTaosMACChange
            end
            object seBonusAbilofTaosHP: TSpinEditEx
              Left = 72
              Top = 140
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#29983#21629#20540#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'2'
              MaxValue = 0
              MinValue = 0
              TabOrder = 5
              Value = 100
              OnChange = seBonusAbilofTaosHPChange
            end
            object seBonusAbilofTaosMP: TSpinEditEx
              Left = 72
              Top = 164
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#39764#27861#20540#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'1'
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 100
              OnChange = seBonusAbilofTaosMPChange
            end
            object seBonusAbilofTaosHit: TSpinEditEx
              Left = 72
              Top = 188
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#21629#20013#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'30'
              MaxValue = 0
              MinValue = 0
              TabOrder = 7
              Value = 100
              OnChange = seBonusAbilofTaosHitChange
            end
            object seBonusAbilofTaosSpeed: TSpinEditEx
              Left = 72
              Top = 212
              Width = 54
              Height = 21
              Hint = #35843#25972'1'#28857#30340#25935#25463#23646#24615#25152#38656#35201#30340#23646#24615#28857', '#40664#35748#20026'30'
              MaxValue = 0
              MinValue = 0
              TabOrder = 8
              Value = 100
              OnChange = seBonusAbilofTaosSpeedChange
            end
          end
          object btnBonusAbilofSave: TButton
            Left = 362
            Top = 253
            Width = 75
            Height = 25
            Caption = #20445#23384
            TabOrder = 3
            OnClick = btnBonusAbilofSaveClick
          end
        end
        object ts16: TTabSheet
          Caption = #20854#20182#25511#21046#19977
          ImageIndex = 2
          object btnOther3: TButton
            Left = 358
            Top = 304
            Width = 75
            Height = 25
            Caption = #20445#23384
            TabOrder = 0
            OnClick = btnOther3Click
          end
          object grp43: TGroupBox
            Left = 6
            Top = 3
            Width = 170
            Height = 77
            Caption = #22797#27963#25106#25351#35774#32622
            TabOrder = 1
            object lbl97: TLabel
              Left = 11
              Top = 21
              Width = 60
              Height = 12
              Caption = #22797#27963#38388#38548#65306
            end
            object lbl98: TLabel
              Left = 125
              Top = 21
              Width = 12
              Height = 12
              Caption = #31186
            end
            object chkRevivalTouch: TCheckBox
              Left = 11
              Top = 56
              Width = 97
              Height = 17
              Hint = #21246#36873#21518#20154#29289#22797#27963#23558#35302#21457'QF'#33050#26412#20013#30340'[@Revival]'
              Caption = #24320#21551#22797#27963#35302#21457
              TabOrder = 0
              OnClick = chkRevivalTouchClick
            end
            object seRevivalTime: TSpinEditEx
              Left = 68
              Top = 17
              Width = 54
              Height = 21
              Hint = #35774#32622#21518#20154#29289#20108#27425#22797#27963#38388#38548#24517#39035#22823#20110#27492#21442#25968#25165#21487#20197#32487#32493#20351#29992#22797#27963#25106#25351
              MaxValue = 0
              MinValue = 0
              TabOrder = 1
              Value = 17
              OnChange = seRevivalTimeChange
            end
            object chkSaveRevivalTime: TCheckBox
              Left = 11
              Top = 39
              Width = 97
              Height = 17
              Hint = #21246#36873#21518#20445#23384#22797#27963#26102#38388#65292#23567#36864#21518#27809#21040#22797#27963#26102#38388#19981#22797#27963#65307#19981#21246#36873#21017#23567#36864#37325#36827#19981#31649#26377#27809#26377#21040#22797#27963#26102#38388#65292#31532#19968#27425#22797#27963#26377#25928
              Caption = #20445#23384#22797#27963#26102#38388
              TabOrder = 2
              OnClick = chkSaveRevivalTimeClick
            end
          end
          object GroupBox119: TGroupBox
            Left = 6
            Top = 84
            Width = 169
            Height = 59
            Caption = #21103#26412#22320#22270#35774#32622
            TabOrder = 2
            object chkFBDisableDelay30s: TCheckBox
              Left = 11
              Top = 38
              Width = 145
              Height = 17
              Hint = #21103#26412#22320#22270#21040#26102#38388#21518#26159#21542#24310#26102'30'#31186#36864#20986#22320#22270
              Caption = #31105#29992#21103#26412#21040#26102#24310#26102'30'#31186
              TabOrder = 0
              OnClick = chkFBDisableDelay30sClick
            end
            object chkFBExitCreaterOffline: TCheckBox
              Left = 11
              Top = 17
              Width = 154
              Height = 17
              Hint = #21103#26412#21019#24314#20154#19981#22312#21103#26412#22320#22270#26102#65292#21103#26412#22320#22270#20013#30340#20154#20840#37096#22238#22478#13#10#20363#22914#65306#34892#20250#25484#38376#19981#22312#21103#26412#22320#22270#26102#65292#34892#20250#25104#21592#23558#22238#22478
              Caption = #21019#24314#20154#19981#22312#21103#26412#36864#20986#21103#26412
              TabOrder = 1
              OnClick = chkFBExitCreaterOfflineClick
            end
          end
          object GroupBox207: TGroupBox
            Left = 182
            Top = 66
            Width = 249
            Height = 61
            Caption = #39318#39280#30418#35774#32622
            TabOrder = 3
            object lbl115: TLabel
              Left = 5
              Top = 39
              Width = 96
              Height = 12
              Caption = #39318#39280#30418#25552#31034#20449#24687#65306
            end
            object chkJewelryCalcBasicAbilitys: TCheckBox
              Left = 5
              Top = 16
              Width = 82
              Height = 17
              Hint = #21246#36873#21518#39318#39280#30418#20869#35013#22791#23558#35745#31639#22522#30784#23646#24615'('#22914#65306#25915#20987#12289#39764#27861#12289#36947#26415')'
              Caption = #31639#35013#22791#23646#24615
              TabOrder = 0
              OnClick = chkJewelryCalcBasicAbilitysClick
            end
            object chkJewelryCalcGroupAbilitys: TCheckBox
              Left = 96
              Top = 17
              Width = 80
              Height = 17
              Hint = #21246#36873#21518#39318#39280#30418#20013#29289#21697#23558#35745#31639#21040#22871#35013#35302#21457
              Caption = #31639#22871#35013#23646#24615
              TabOrder = 1
              OnClick = chkJewelryCalcGroupAbilitysClick
            end
            object edtJewelryBoxHint: TEdit
              Left = 96
              Top = 35
              Width = 144
              Height = 20
              MaxLength = 20
              TabOrder = 2
              OnChange = edtJewelryBoxHintChange
            end
            object chkJewelryDecDura: TCheckBox
              Left = 184
              Top = 16
              Width = 57
              Height = 17
              Caption = #25481#25345#20037
              TabOrder = 3
              OnClick = chkJewelryDecDuraClick
            end
          end
          object GroupBox55: TGroupBox
            Left = 6
            Top = 146
            Width = 169
            Height = 56
            Caption = #35013#22791#21051#21517
            TabOrder = 4
            object Label118: TLabel
              Left = 11
              Top = 34
              Width = 72
              Height = 12
              Caption = #33258#23450#20041#21069#32512#65306
            end
            object CheckBoxItemName: TCheckBox
              Left = 11
              Top = 13
              Width = 153
              Height = 17
              Caption = #20351#29992#29609#23478#30340#21517#31216#20570#21069#32512
              TabOrder = 0
              OnClick = CheckBoxItemNameClick
            end
            object EditItemName: TEdit
              Left = 83
              Top = 30
              Width = 78
              Height = 20
              ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              TabOrder = 1
              Text = #12310#25913#12311
              OnChange = EditItemNameChange
            end
          end
          object grp60: TGroupBox
            Left = 295
            Top = 3
            Width = 136
            Height = 62
            Caption = #34892#20250#31995#32479
            TabOrder = 5
            object chkOpenNewGuild: TCheckBox
              Left = 8
              Top = 17
              Width = 105
              Height = 17
              Hint = #26356#25913#21518#38656#37325#21551#29983#25928
              Caption = #21551#29992#26032#34892#20250#31995#32479
              TabOrder = 0
              OnClick = chkOpenNewGuildClick
            end
            object chkNoShowNewGuildHumanCount: TCheckBox
              Left = 8
              Top = 38
              Width = 119
              Height = 17
              Caption = #19981#26174#31034#26032#34892#20250#20154#25968
              TabOrder = 1
              OnClick = chkNoShowNewGuildHumanCountClick
            end
          end
          object chkCloseNPCNoItemMsg: TCheckBox
            Left = 184
            Top = 182
            Width = 249
            Height = 17
            Caption = #20851#38381'NPC'#21629#20196#25552#31034#65306#20320#36523#19978#27809#26377#25140#25351#23450#29289#21697
            TabOrder = 6
            OnClick = chkCloseNPCNoItemMsgClick
          end
          object grp65: TGroupBox
            Left = 182
            Top = 3
            Width = 110
            Height = 63
            Caption = #26143#26143#31561#32423#26174#31034
            TabOrder = 7
            object lbl121: TLabel
              Left = 5
              Top = 19
              Width = 60
              Height = 12
              Caption = #36882#22686#22522#25968#65306
            end
            object Label226: TLabel
              Left = 5
              Top = 42
              Width = 60
              Height = 12
              Caption = #21333#34892#25968#37327#65306
            end
            object seStarBaseNum: TSpinEditEx
              Left = 60
              Top = 15
              Width = 42
              Height = 21
              MaxValue = 10
              MinValue = 2
              TabOrder = 0
              Value = 10
              OnChange = seStarBaseNumChange
            end
            object seStarLineMaxCount: TSpinEditEx
              Left = 60
              Top = 38
              Width = 42
              Height = 21
              Hint = #19968#34892#26368#22810#26174#31034#30340#26143#26143#25968#37327#65292#36229#36807#25968#37327#25442#34892
              MaxValue = 30
              MinValue = 2
              TabOrder = 1
              Value = 10
              OnChange = seStarLineMaxCountChange
            end
          end
          object grp66: TGroupBox
            Left = 6
            Top = 205
            Width = 169
            Height = 38
            Caption = #32858#28789#29664#30456#20851
            TabOrder = 8
            object chkRecordBeadExp: TCheckBox
              Left = 8
              Top = 16
              Width = 97
              Height = 17
              Hint = 
                #21246#36873#21518#20154#29289#22312#32447#33719#24471#32463#39564#26102#20505#24182#19988#32972#21253#27809#26377#32858#28789#29664#25110#32773#32858#28789#29664#23384#28385#21518#20840#37096#20445#30041#19968#20221#32463#39564#65292#20877#27425#25918#20837#26032#30340#32858#28789#29664#21363#21051#32047#21152#21040#26032#30340#32858#28789#29664#65292#23567#36864#21518#20445#30041 +
                #32463#39564#37322#25918#65281
              Caption = #32463#39564#20445#30041
              TabOrder = 0
              OnClick = chkRecordBeadExpClick
            end
          end
          object grp70: TGroupBox
            Left = 182
            Top = 131
            Width = 249
            Height = 42
            Caption = #27602#31526#20301#32622#25511#21046
            TabOrder = 9
            object chkDisableDuFuTakeArmRingL: TCheckBox
              Left = 8
              Top = 16
              Width = 153
              Height = 17
              Hint = #21246#36873#21518#27602#31526#19981#33021#20329#25140#21040#20154#29289#39318#39280#20301#32622#65292#21482#33021#20329#25140#21040#19979'4'#26684#30340#20301#32622#65292'176'#30028#38754#21247#36873
              Caption = #31105#27490#39318#39280#20301#32622#20329#25140#27602#31526
              TabOrder = 0
              OnClick = chkDisableDuFuTakeArmRingLClick
            end
          end
          object GroupBox94: TGroupBox
            Left = 6
            Top = 246
            Width = 169
            Height = 38
            Caption = #20256#36865#25511#21046
            TabOrder = 10
            object chkDisableMoveParalysisHuman: TCheckBox
              Left = 8
              Top = 16
              Width = 153
              Height = 17
              Caption = #20154#29289#40635#30201#29366#24577#31105#27490#20256#36865
              TabOrder = 0
              OnClick = chkDisableMoveParalysisHumanClick
            end
          end
        end
      end
    end
    object TabSheet52: TTabSheet
      Caption = #37202#39302#31995#32479
      ImageIndex = 18
      object GroupBox129: TGroupBox
        Left = 8
        Top = 8
        Width = 166
        Height = 217
        Caption = #33647#21147#20540#30456#20851
        TabOrder = 0
        object Label272: TLabel
          Left = 26
          Top = 189
          Width = 54
          Height = 12
          Caption = #20943#33647#21147#20540':'
        end
        object Label273: TLabel
          Left = 4
          Top = 168
          Width = 78
          Height = 12
          Caption = #20943#33647#21147#20540#38388#38548':'
        end
        object Label274: TLabel
          Left = 150
          Top = 168
          Width = 12
          Height = 12
          Caption = #31186
        end
        object GridMedicineExp: TStringGrid
          Left = 6
          Top = 19
          Width = 153
          Height = 137
          ColCount = 2
          DefaultRowHeight = 18
          RowCount = 1
          FixedRows = 0
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
          TabOrder = 0
          OnEnter = GridMedicineExpEnter
          ColWidths = (
            64
            67)
          RowHeights = (
            18)
        end
        object EditDecMedicineValue: TSpinEditEx
          Left = 85
          Top = 185
          Width = 65
          Height = 21
          Hint = #22312#32447#25351#23450#26102#38388#20869#27809#26377#39278#29992#33647#37202','#21017#20943#33647#21147#20540
          MaxValue = 100
          MinValue = 1
          TabOrder = 2
          Value = 1
          OnChange = EditDecMedicineValueChange
        end
        object EditDecMedicineTime: TSpinEditEx
          Left = 85
          Top = 164
          Width = 65
          Height = 21
          Hint = #38271#26399#27809#26377#39278#37202','#38388#38548#22810#38271#26102#38388#20943#33647#21147#20540
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 1
          Value = 43200
          OnChange = EditDecMedicineTimeChange
        end
      end
      object GroupBox130: TGroupBox
        Left = 180
        Top = 8
        Width = 213
        Height = 113
        Caption = #39278#37202#30456#20851
        TabOrder = 1
        object Label275: TLabel
          Left = 17
          Top = 20
          Width = 102
          Height = 12
          Caption = #37202#37327#36827#24230#22686#21152#38388#38548':'
        end
        object Label276: TLabel
          Left = 193
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label277: TLabel
          Left = 28
          Top = 44
          Width = 90
          Height = 12
          Caption = #37257#37202#24230#20943#23569#38388#38548':'
        end
        object Label278: TLabel
          Left = 193
          Top = 44
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label279: TLabel
          Left = 28
          Top = 68
          Width = 90
          Height = 12
          Caption = #37202#37327#19978#38480#21021#22987#20540':'
        end
        object Label280: TLabel
          Left = 17
          Top = 92
          Width = 102
          Height = 12
          Caption = #37202#37327#21319#32423#19978#38480#22686#21152':'
        end
        object EditIncAlcoholTime: TSpinEditEx
          Left = 129
          Top = 16
          Width = 57
          Height = 21
          Hint = #37202#37327#36827#24230#22686#21152#26102#38388#38388#38548
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditIncAlcoholTimeChange
        end
        object EditDecDrinkTime: TSpinEditEx
          Left = 129
          Top = 40
          Width = 57
          Height = 21
          Hint = #37257#37202#24230#20943#23569#38388#38548#26102#38388
          MaxValue = 100000000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditDecDrinkTimeChange
        end
        object EditMaxAlcoholValue: TSpinEditEx
          Left = 129
          Top = 64
          Width = 57
          Height = 21
          Hint = #37202#37327#21021#22987#19978#38480#20540
          MaxValue = 65535
          MinValue = 1
          TabOrder = 2
          Value = 2000
          OnChange = EditMaxAlcoholValueChange
        end
        object EditIncAlcoholValue: TSpinEditEx
          Left = 129
          Top = 88
          Width = 57
          Height = 21
          Hint = #24403#21069#37202#37327#20540#36798#21040#19978#38480#21518','#37202#37327#19978#38480#22686#21152#25351#23450#30340#20540
          MaxValue = 65535
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = EditIncAlcoholValueChange
        end
      end
      object ButtonSaveWine: TButton
        Left = 366
        Top = 304
        Width = 75
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 2
        OnClick = ButtonSaveWineClick
      end
    end
    object TabSheet54: TTabSheet
      Caption = #25293#21334#34892
      ImageIndex = 19
      object GroupBox235: TGroupBox
        Left = 8
        Top = 8
        Width = 249
        Height = 42
        Caption = #40664#35748#35774#32622
        TabOrder = 0
        object Label173: TLabel
          Left = 8
          Top = 18
          Width = 156
          Height = 12
          Caption = #20010#20154#21516#26102#21442#19982#31454#25293#29289#21697#25968#37327#65306
        end
        object seAuctioningItemsCount: TSpinEditEx
          Left = 160
          Top = 14
          Width = 81
          Height = 21
          Hint = #27492#21442#25968#20026#40664#35748#24215#38138#20801#35768#21516#26102#20986#21806#30340#29289#21697#25968#37327
          MaxValue = 20
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seAuctioningItemsCountChange
        end
      end
      object GroupBox236: TGroupBox
        Left = 267
        Top = 7
        Width = 176
        Height = 140
        Caption = #20801#35768#36135#24065#21450#31246#25910
        TabOrder = 1
        object Label177: TLabel
          Left = 160
          Top = 19
          Width = 6
          Height = 12
          Caption = '%'
        end
        object lbl135: TLabel
          Left = 70
          Top = 19
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object Label867: TLabel
          Left = 160
          Top = 43
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label868: TLabel
          Left = 70
          Top = 43
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object Label869: TLabel
          Left = 160
          Top = 67
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label870: TLabel
          Left = 70
          Top = 67
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object Label871: TLabel
          Left = 160
          Top = 92
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label872: TLabel
          Left = 70
          Top = 92
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object Label873: TLabel
          Left = 160
          Top = 116
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label874: TLabel
          Left = 70
          Top = 116
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object seAuctionGoldTaxRate: TSpinEditEx
          Left = 102
          Top = 15
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seAuctionGoldTaxRateChange
        end
        object chkAuctionCurrencyType1: TCheckBox
          Left = 8
          Top = 17
          Width = 60
          Height = 17
          Caption = #37329#24065
          TabOrder = 1
          OnClick = chkAuctionCurrencyType1Click
        end
        object chkAuctionCurrencyType2: TCheckBox
          Left = 8
          Top = 41
          Width = 60
          Height = 17
          Caption = #20803#23453
          TabOrder = 2
          OnClick = chkAuctionCurrencyType1Click
        end
        object chkAuctionCurrencyType3: TCheckBox
          Left = 8
          Top = 65
          Width = 60
          Height = 17
          Caption = #37329#21018#30707
          TabOrder = 3
          OnClick = chkAuctionCurrencyType1Click
        end
        object chkAuctionCurrencyType4: TCheckBox
          Left = 8
          Top = 89
          Width = 60
          Height = 17
          Caption = #28789#31526
          TabOrder = 4
          OnClick = chkAuctionCurrencyType1Click
        end
        object chkAuctionCurrencyType5: TCheckBox
          Left = 8
          Top = 113
          Width = 60
          Height = 17
          Caption = #28216#25103#28857
          TabOrder = 5
          OnClick = chkAuctionCurrencyType1Click
        end
        object seAuctionGameGoldTaxRate: TSpinEditEx
          Left = 102
          Top = 39
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 6
          Value = 0
          OnChange = seAuctionGameGoldTaxRateChange
        end
        object seAuctionGameDiamondTaxRate: TSpinEditEx
          Left = 102
          Top = 63
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 7
          Value = 0
          OnChange = seAuctionGameDiamondTaxRateChange
        end
        object seAuctionGameGirdTaxRate: TSpinEditEx
          Left = 102
          Top = 87
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 8
          Value = 0
          OnChange = seAuctionGameGirdTaxRateChange
        end
        object seAuctionGamePointTaxRate: TSpinEditEx
          Left = 102
          Top = 111
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 9
          Value = 0
          OnChange = seAuctionGamePointTaxRateChange
        end
      end
      object btnAuction: TButton
        Left = 376
        Top = 331
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 2
        OnClick = btnAuctionClick
      end
      object GroupBox237: TGroupBox
        Left = 8
        Top = 183
        Width = 249
        Height = 108
        Caption = #20840#26381#24191#25773
        TabOrder = 3
        object Label626: TLabel
          Left = 140
          Top = 19
          Width = 36
          Height = 12
          Caption = #36153#29992#65306
        end
        object Label795: TLabel
          Left = 8
          Top = 19
          Width = 60
          Height = 12
          Caption = #25910#36153#36135#24065#65306
        end
        object lbl137: TLabel
          Left = 8
          Top = 66
          Width = 60
          Height = 12
          Caption = #24191#25773#20869#23481#65306
        end
        object Label806: TLabel
          Left = 8
          Top = 43
          Width = 60
          Height = 12
          Caption = #24191#25773#26102#38388#65306
        end
        object seAuctionBroadcastPrice: TSpinEditEx
          Left = 172
          Top = 15
          Width = 71
          Height = 21
          Hint = #21457#36865#20840#26381#24191#25773#26102#25910#36153
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seAuctionBroadcastPriceChange
        end
        object cbbAuctionBroadcastCurrencyType: TComboBox
          Left = 64
          Top = 15
          Width = 73
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 1
          Text = #20803#23453
          OnChange = cbbAuctionBroadcastCurrencyTypeChange
          Items.Strings = (
            #20803#23453
            #28216#25103#28857
            #37329#24065
            #37329#21018#30707
            #28789#31526)
        end
        object seAuctionBroadcastShowTime: TSpinEditEx
          Left = 66
          Top = 39
          Width = 71
          Height = 21
          Hint = #24191#25773#26174#31034#26102#38388
          MaxValue = 600
          MinValue = 5
          TabOrder = 2
          Value = 5
          OnChange = seAuctionBroadcastShowTimeChange
        end
        object edtAuctionBroadcastText: TEdit
          Left = 8
          Top = 80
          Width = 233
          Height = 20
          TabOrder = 3
          Text = '%user'#22312#25293#21334#24066#22330#21457#21806'%item,'#24213#20215#20165#20026'%price %mname'#65292#27442#36141#20174#36895
          OnChange = edtAuctionBroadcastTextChange
        end
      end
      object GroupBox238: TGroupBox
        Left = 8
        Top = 56
        Width = 249
        Height = 123
        Caption = #29289#21697#21697#36136
        TabOrder = 4
        object Label625: TLabel
          Left = 8
          Top = 43
          Width = 42
          Height = 12
          Caption = #21697#36136'1'#65306
        end
        object Label797: TLabel
          Left = 135
          Top = 43
          Width = 42
          Height = 12
          Caption = #21697#36136'2'#65306
        end
        object Label799: TLabel
          Left = 8
          Top = 67
          Width = 42
          Height = 12
          Caption = #21697#36136'3'#65306
        end
        object Label801: TLabel
          Left = 135
          Top = 67
          Width = 42
          Height = 12
          Caption = #21697#36136'4'#65306
        end
        object Label803: TLabel
          Left = 8
          Top = 91
          Width = 42
          Height = 12
          Caption = #21697#36136'5'#65306
        end
        object Label805: TLabel
          Left = 135
          Top = 91
          Width = 42
          Height = 12
          Caption = #21697#36136'6'#65306
        end
        object seAuctionItemColor1: TColorIndexEdit
          Left = 48
          Top = 38
          Width = 67
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 250
          OnChange = seAuctionItemColor1Change
          ShowNoneColor = False
        end
        object seAuctionItemColor2: TColorIndexEdit
          Tag = 1
          Left = 175
          Top = 38
          Width = 67
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 254
          OnChange = seAuctionItemColor1Change
          ShowNoneColor = False
        end
        object seAuctionItemColor3: TColorIndexEdit
          Tag = 2
          Left = 48
          Top = 62
          Width = 67
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 251
          OnChange = seAuctionItemColor1Change
          ShowNoneColor = False
        end
        object seAuctionItemColor4: TColorIndexEdit
          Tag = 3
          Left = 174
          Top = 62
          Width = 67
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 3
          Value = 253
          OnChange = seAuctionItemColor1Change
          ShowNoneColor = False
        end
        object seAuctionItemColor5: TColorIndexEdit
          Tag = 4
          Left = 48
          Top = 86
          Width = 67
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 4
          Value = 241
          OnChange = seAuctionItemColor1Change
          ShowNoneColor = False
        end
        object seAuctionItemColor6: TColorIndexEdit
          Tag = 5
          Left = 174
          Top = 86
          Width = 67
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 5
          Value = 243
          OnChange = seAuctionItemColor1Change
          ShowNoneColor = False
        end
        object chkOpenAuctionItemColors: TCheckBox
          Left = 8
          Top = 16
          Width = 97
          Height = 17
          Caption = #24320#21551#29289#21697#21697#36136
          TabOrder = 6
          OnClick = chkOpenAuctionItemColorsClick
        end
      end
    end
    object ts28: TTabSheet
      Caption = #35282#33394#20986#21806
      ImageIndex = 19
      object lbl159: TLabel
        Left = 189
        Top = 11
        Width = 132
        Height = 12
        Caption = #35282#33394#22996#25176#20986#21806#26102#38388#38480#21046#65306
      end
      object lbl160: TLabel
        Left = 396
        Top = 11
        Width = 24
        Height = 12
        Caption = #23567#26102
      end
      object grp11: TGroupBox
        Left = 3
        Top = 7
        Width = 176
        Height = 140
        Caption = #20801#35768#36135#24065#21450#31246#25910
        TabOrder = 0
        object lbl148: TLabel
          Left = 160
          Top = 19
          Width = 6
          Height = 12
          Caption = '%'
        end
        object lbl149: TLabel
          Left = 70
          Top = 19
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object lbl150: TLabel
          Left = 160
          Top = 43
          Width = 6
          Height = 12
          Caption = '%'
        end
        object lbl151: TLabel
          Left = 70
          Top = 43
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object lbl152: TLabel
          Left = 160
          Top = 67
          Width = 6
          Height = 12
          Caption = '%'
        end
        object lbl153: TLabel
          Left = 70
          Top = 67
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object lbl154: TLabel
          Left = 160
          Top = 92
          Width = 6
          Height = 12
          Caption = '%'
        end
        object lbl155: TLabel
          Left = 70
          Top = 92
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object lbl156: TLabel
          Left = 160
          Top = 116
          Width = 6
          Height = 12
          Caption = '%'
        end
        object lbl157: TLabel
          Left = 70
          Top = 116
          Width = 36
          Height = 12
          Caption = #31246#25910#65306
        end
        object seSellPlayerGoldTaxRate: TSpinEditEx
          Left = 101
          Top = 16
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seSellPlayerGoldTaxRateChange
        end
        object chkSellPlayerCurrencyType1: TCheckBox
          Left = 8
          Top = 17
          Width = 60
          Height = 17
          Caption = #37329#24065
          TabOrder = 1
          OnClick = chkSellPlayerCurrencyType1Click
        end
        object chkSellPlayerCurrencyType2: TCheckBox
          Left = 8
          Top = 41
          Width = 60
          Height = 17
          Caption = #20803#23453
          TabOrder = 2
          OnClick = chkSellPlayerCurrencyType1Click
        end
        object chkSellPlayerCurrencyType3: TCheckBox
          Left = 8
          Top = 65
          Width = 60
          Height = 17
          Caption = #37329#21018#30707
          TabOrder = 3
          OnClick = chkSellPlayerCurrencyType1Click
        end
        object chkSellPlayerCurrencyType4: TCheckBox
          Left = 8
          Top = 89
          Width = 60
          Height = 17
          Caption = #28789#31526
          TabOrder = 4
          OnClick = chkSellPlayerCurrencyType1Click
        end
        object chkSellPlayerCurrencyType5: TCheckBox
          Left = 8
          Top = 113
          Width = 60
          Height = 17
          Caption = #28216#25103#28857
          TabOrder = 5
          OnClick = chkSellPlayerCurrencyType1Click
        end
        object seSellPlayerGameGoldTaxRate: TSpinEditEx
          Left = 102
          Top = 39
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 6
          Value = 0
          OnChange = seSellPlayerGameGoldTaxRateChange
        end
        object seSellPlayerGameDiamondTaxRate: TSpinEditEx
          Left = 102
          Top = 63
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 7
          Value = 0
          OnChange = seSellPlayerGameDiamondTaxRateChange
        end
        object seSellPlayerGameGirdTaxRate: TSpinEditEx
          Left = 102
          Top = 87
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 8
          Value = 0
          OnChange = seSellPlayerGameGirdTaxRateChange
        end
        object seSellPlayerGamePointTaxRate: TSpinEditEx
          Left = 102
          Top = 111
          Width = 53
          Height = 21
          Hint = #24403#29609#23478#20132#26131#25104#21151#21518#65292#25910#21462#21334#23478#37329#24065#30340#31246#25910#12290
          MaxValue = 100
          MinValue = 0
          TabOrder = 9
          Value = 0
          OnChange = seSellPlayerGamePointTaxRateChange
        end
      end
      object seSellPlayerTime: TSpinEditEx
        Left = 318
        Top = 7
        Width = 75
        Height = 21
        Hint = #22996#25176#26102#38388#20026'0'#26102#65292#34920#31034#19981#38480#21046#26102#38388#12290#21542#21017#36229#36807#19968#23450#30340#26102#38388#26410#23436#25104#20132#26131#30340#35282#33394#23558#20250#33258#21160#21462#28040#20986#21806
        MaxValue = 1000
        MinValue = 0
        TabOrder = 1
        Value = 0
        OnChange = seSellPlayerTimeChange
      end
      object btnSellPlayerOK: TButton
        Left = 376
        Top = 331
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 2
        OnClick = btnSellPlayerOKClick
      end
      object chkSellPlayerViewStorage: TCheckBox
        Left = 189
        Top = 34
        Width = 172
        Height = 17
        Caption = #20801#35768#26597#30475#20986#21806#35282#33394#30340#20179#24211
        TabOrder = 3
        OnClick = chkSellPlayerViewStorageClick
      end
      object chkSellPlayerViewStorageEx: TCheckBox
        Left = 189
        Top = 55
        Width = 188
        Height = 17
        Caption = #20801#35768#26597#30475#20986#21806#35282#33394#30340#26080#38480#20179#24211
        TabOrder = 4
        OnClick = chkSellPlayerViewStorageExClick
      end
      object grp14: TGroupBox
        Left = 3
        Top = 152
        Width = 176
        Height = 73
        Caption = #26597#30475#35282#33394#20854#20182#20449#24687#25991#23383#20559#31227
        TabOrder = 5
        object lbl158: TLabel
          Left = 11
          Top = 24
          Width = 66
          Height = 12
          Caption = 'X'#22352#26631#20559#31227#65306
        end
        object Label934: TLabel
          Left = 11
          Top = 48
          Width = 66
          Height = 12
          Caption = 'Y'#22352#26631#20559#31227#65306
        end
        object seSellPlayerViewOtherInfoTextOffsetY: TSpinEditEx
          Left = 73
          Top = 44
          Width = 88
          Height = 21
          MaxValue = 32767
          MinValue = -32768
          TabOrder = 0
          Value = 0
          OnChange = seSellPlayerViewOtherInfoTextOffsetYChange
        end
        object seSellPlayerViewOtherInfoTextOffsetX: TSpinEditEx
          Left = 73
          Top = 20
          Width = 88
          Height = 21
          MaxValue = 32767
          MinValue = -32768
          TabOrder = 1
          Value = 0
          OnChange = seSellPlayerViewOtherInfoTextOffsetXChange
        end
      end
      object chkSellPlayerAutoRecallHero: TCheckBox
        Left = 189
        Top = 76
        Width = 156
        Height = 17
        Caption = #35282#33394#20986#21806#33258#21160#21484#21796#33521#38596
        TabOrder = 6
        OnClick = chkSellPlayerAutoRecallHeroClick
      end
    end
  end
end
