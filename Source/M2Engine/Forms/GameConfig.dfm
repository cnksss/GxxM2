object frmGameConfig: TfrmGameConfig
  Left = 275
  Top = 252
  ActiveControl = CheckBoxShowMakeItemMsg
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #28216#25103#21442#25968
  ClientHeight = 372
  ClientWidth = 549
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
    Top = 352
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
  object GameConfigControl: TPageControl
    Left = 8
    Top = 8
    Width = 537
    Height = 337
    ActivePage = GeneralSheet
    MultiLine = True
    TabOrder = 0
    OnChanging = GameConfigControlChanging
    object GeneralSheet: TTabSheet
      Caption = #29615#22659#35774#32622
      ImageIndex = 2
      object GroupBoxInfo: TGroupBox
        Left = 168
        Top = 57
        Width = 145
        Height = 48
        Caption = #23458#25143#31471#29256#26412#21495
        TabOrder = 3
        object Label16: TLabel
          Left = 8
          Top = 24
          Width = 60
          Height = 12
          Caption = #29256#26412#26085#26399#65306
        end
        object EditSoftVersionDate: TEdit
          Left = 68
          Top = 20
          Width = 73
          Height = 20
          Hint = #23458#25143#31471#29256#26412#26085#26399#35774#32622#65292#27492#26085#26399#25968#23383#24517#39035#19982#23458#25143#31471#19978#30340#26085#26399#26631#35782#19968#33268#65292#21542#21017#36827#20837#28216#25103#26102#20250#25552#31034#29256#26412#38169#35823#12290#28857#20445#23384#25353#38062#21518#29983#25928#12290
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = '20020522'
          OnChange = EditSoftVersionDateChange
        end
      end
      object GroupBox5: TGroupBox
        Left = 168
        Top = 5
        Width = 145
        Height = 48
        Caption = #25511#21046#21488#26174#31034#20154#25968#26102#38388'('#31186')'
        TabOrder = 2
        object Label17: TLabel
          Left = 8
          Top = 24
          Width = 60
          Height = 12
          Caption = #26174#31034#38388#38548#65306
        end
        object EditConsoleShowUserCountTime: TSpinEditEx
          Left = 68
          Top = 20
          Width = 61
          Height = 21
          Hint = #31243#24207#25511#21046#21488#26174#31034#22312#32447#20154#25968#26102#38388#38388#38548#12290
          Increment = 10
          MaxValue = 2000
          MinValue = 10
          TabOrder = 0
          Value = 10
          OnChange = EditConsoleShowUserCountTimeChange
        end
      end
      object GroupBox6: TGroupBox
        Left = 8
        Top = 114
        Width = 153
        Height = 100
        Caption = #28216#25103#20844#21578#26174#31034#38388#38548'('#31186')'
        TabOrder = 4
        object Label18: TLabel
          Left = 8
          Top = 27
          Width = 60
          Height = 12
          Caption = #26174#31034#38388#38548#65306
        end
        object Label19: TLabel
          Left = 8
          Top = 51
          Width = 60
          Height = 12
          Caption = #25991#23383#39068#33394#65306
        end
        object Label21: TLabel
          Left = 8
          Top = 75
          Width = 60
          Height = 12
          Caption = #21069#32512#25991#23383#65306
        end
        object EditShowLineNoticeTime: TSpinEditEx
          Left = 68
          Top = 22
          Width = 57
          Height = 21
          Hint = #28216#25103#20013#20844#21578#20449#24687#26174#31034#26102#38388#38388#38548#26102#38388#12290
          Increment = 10
          MaxValue = 2000
          MinValue = 10
          TabOrder = 0
          Value = 10
          OnChange = EditShowLineNoticeTimeChange
        end
        object ComboBoxLineNoticeColor: TComboBox
          Left = 68
          Top = 47
          Width = 57
          Height = 20
          Hint = #20844#21578#25991#23383#26174#31034#40664#35748#39068#33394#12290
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          OnChange = ComboBoxLineNoticeColorChange
        end
        object EditLineNoticePreFix: TEdit
          Left = 68
          Top = 71
          Width = 73
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          MaxLength = 20
          TabOrder = 2
          Text = #12310#20844#21578#12311
          OnChange = EditLineNoticePreFixChange
        end
      end
      object ButtonGeneralSave: TButton
        Left = 400
        Top = 257
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 7
        OnClick = ButtonGeneralSaveClick
      end
      object GroupBox35: TGroupBox
        Left = 320
        Top = 0
        Width = 145
        Height = 214
        Caption = #25511#21046#21488#26174#31034#20449#24687
        TabOrder = 0
        object CheckBoxShowMakeItemMsg: TCheckBox
          Left = 8
          Top = 19
          Width = 97
          Height = 17
          Caption = 'GM'#25805#20316#20449#24687
          TabOrder = 0
          OnClick = CheckBoxShowMakeItemMsgClick
        end
        object CbViewHack: TCheckBox
          Left = 8
          Top = 36
          Width = 97
          Height = 17
          Caption = #36895#24230#24322#24120#20449#24687
          TabOrder = 1
          OnClick = CbViewHackClick
        end
        object CkViewAdmfail: TCheckBox
          Left = 8
          Top = 53
          Width = 97
          Height = 17
          Caption = #38750#27861#30331#24405#20449#24687
          TabOrder = 2
          OnClick = CkViewAdmfailClick
        end
        object CheckBoxShowExceptionMsg: TCheckBox
          Left = 8
          Top = 70
          Width = 97
          Height = 17
          Caption = #24322#24120#38169#35823#20449#24687
          TabOrder = 3
          OnClick = CheckBoxShowExceptionMsgClick
        end
        object chkRecordPrivateMsg: TCheckBox
          Left = 8
          Top = 104
          Width = 97
          Height = 17
          Caption = #31169#23494#32842#22825#20449#24687
          TabOrder = 4
          OnClick = chkRecordPrivateMsgClick
        end
        object chkRecordPublicMsg: TCheckBox
          Left = 8
          Top = 87
          Width = 97
          Height = 17
          Caption = #26222#36890#32842#22825#20449#24687' '
          TabOrder = 5
          OnClick = chkRecordPublicMsgClick
        end
        object chkRecordGuildMsg: TCheckBox
          Left = 8
          Top = 121
          Width = 97
          Height = 17
          Caption = #34892#20250#32842#22825#20449#24687
          TabOrder = 6
          OnClick = chkRecordGuildMsgClick
        end
        object chkRecordCryCryMsg: TCheckBox
          Left = 8
          Top = 138
          Width = 97
          Height = 17
          Caption = #21898#35805#32842#22825#20449#24687
          TabOrder = 7
          OnClick = chkRecordCryCryMsgClick
        end
        object chkRecordGroupMsg: TCheckBox
          Left = 8
          Top = 155
          Width = 97
          Height = 17
          Caption = #32452#38431#32842#22825#20449#24687
          TabOrder = 8
          OnClick = chkRecordGroupMsgClick
        end
        object chkRecordNationMsg: TCheckBox
          Left = 8
          Top = 173
          Width = 97
          Height = 17
          Caption = #22269#23478#32842#22825#20449#24687
          TabOrder = 9
          OnClick = chkRecordNationMsgClick
        end
        object chkPermissionChangeLog: TCheckBox
          Left = 8
          Top = 190
          Width = 93
          Height = 17
          Caption = #35282#33394#26435#38480#25913#21464
          TabOrder = 10
          OnClick = chkPermissionChangeLogClick
        end
      end
      object GroupBox51: TGroupBox
        Left = 8
        Top = 5
        Width = 153
        Height = 100
        Caption = #24191#25773#22312#32447#20154#25968
        TabOrder = 1
        object Label98: TLabel
          Left = 32
          Top = 49
          Width = 36
          Height = 12
          Caption = #20493#29575#65306
        end
        object Label99: TLabel
          Left = 8
          Top = 75
          Width = 60
          Height = 12
          Caption = #38388#38548#26102#38388#65306
        end
        object Label100: TLabel
          Left = 136
          Top = 75
          Width = 12
          Height = 12
          Caption = #31186
        end
        object EditSendOnlineCountRate: TSpinEditEx
          Left = 68
          Top = 44
          Width = 61
          Height = 21
          Hint = #24191#25773#22312#32447#20154#29289#34394#20551#20154#25968#20493#29575#65292#30495#23454#25968#25454#20026#38500#20197'10'#65292#40664#35748#20026'10'#23601#26159#19968#20493#65292'11 '#23601#26159'1.1'#20493#12290
          MaxValue = 2000
          MinValue = 10
          TabOrder = 1
          Value = 10
          OnChange = EditSendOnlineCountRateChange
        end
        object EditSendOnlineTime: TSpinEditEx
          Left = 68
          Top = 71
          Width = 61
          Height = 21
          Hint = #24191#25773#22312#32447#20154#25968#38388#38548#26102#38388#12290
          Increment = 10
          MaxValue = 2000
          MinValue = 5
          TabOrder = 2
          Value = 10
          OnChange = EditSendOnlineTimeChange
        end
        object CheckBoxSendOnlineCount: TCheckBox
          Left = 8
          Top = 24
          Width = 105
          Height = 17
          Hint = #26159#21542#21551#29992#22312#32447#24191#25773#22312#32447#20154#25968#21151#33021#65292#25171#24320#27492#21151#33021#21518#22312#28216#25103#37324#23558#20197#32418#23383#26041#24335#26174#31034#22312#32447#20154#25968#12290
          Caption = #24191#25773#22312#32447#20154#25968
          TabOrder = 0
          OnClick = CheckBoxSendOnlineCountClick
        end
      end
      object GroupBox52: TGroupBox
        Left = 168
        Top = 114
        Width = 145
        Height = 100
        Caption = #29289#21697#24618#29289#25968#25454#24211#20493#29575
        TabOrder = 5
        object Label101: TLabel
          Left = 32
          Top = 26
          Width = 36
          Height = 12
          Caption = #24618#29289#65306
        end
        object Label102: TLabel
          Left = 20
          Top = 50
          Width = 48
          Height = 12
          Caption = #29289#21697#19968#65306
        end
        object Label103: TLabel
          Left = 20
          Top = 74
          Width = 48
          Height = 12
          Caption = #29289#21697#20108#65306
        end
        object EditMonsterPowerRate: TSpinEditEx
          Left = 68
          Top = 22
          Width = 69
          Height = 21
          Hint = #24618#29289#23646#24615#20493#29575'(HP'#12289'MP'#12289'DC'#12289'MC'#12289'SC)'#65292#23454#38469#25968#23383#20026#24403#21069#25968#25454#38500#20197'10'#12290
          MaxValue = 20000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditMonsterPowerRateChange
        end
        object EditEditItemsPowerRate: TSpinEditEx
          Left = 68
          Top = 46
          Width = 69
          Height = 21
          Hint = #29289#21697#23646#24615#20493#29575'(DC'#12289'MC'#12289'SC)'#65292#23454#38469#25968#23383#20026#24403#21069#25968#25454#38500#20197'10'#12290
          MaxValue = 20000000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditEditItemsPowerRateChange
        end
        object EditItemsACPowerRate: TSpinEditEx
          Left = 68
          Top = 70
          Width = 69
          Height = 21
          Hint = #29289#21697#23646#24615#20493#29575'(AC'#12289'MAC'#20108#20010')'#65292#23454#38469#25968#23383#20026#24403#21069#25968#25454#38500#20197'10'#12290
          MaxValue = 2000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditItemsACPowerRateChange
        end
      end
      object GroupBox73: TGroupBox
        Left = 8
        Top = 219
        Width = 153
        Height = 62
        Caption = #23458#25143#31471#29256#26412#25511#21046
        TabOrder = 6
        object CheckBoxCanOldClientLogon: TCheckBox
          Left = 8
          Top = 18
          Width = 129
          Height = 17
          Hint = #26159#21542#20801#35768#26222#36890#26087#23458#25143#31471#30331#24405#28216#25103#65292#38057#19978#20026#25171#24320#65292#22914#26524#20851#38381#27492#21151#33021#65292#21017#32769#23458#25143#31471#23558#26080#27861#30331#24405#21040#28216#25103#12290
          Caption = #20801#35768#26222#36890#23458#25143#31471#30331#24405
          Checked = True
          State = cbChecked
          TabOrder = 0
          OnClick = CheckBoxCanOldClientLogonClick
        end
        object chkOldClient: TCheckBox
          Left = 8
          Top = 40
          Width = 129
          Height = 17
          Hint = #26159#21542#20801#35768#26222#36890#26087#23458#25143#31471#30331#24405#28216#25103#65292#38057#19978#20026#25171#24320#65292#22914#26524#20851#38381#27492#21151#33021#65292#21017#32769#23458#25143#31471#23558#26080#27861#30331#24405#21040#28216#25103#12290
          Caption = #20860#23481'2010'#30331#38470#22120
          Checked = True
          State = cbChecked
          TabOrder = 1
          Visible = False
          OnClick = chkOldClientClick
        end
      end
    end
    object TabSheet4: TTabSheet
      Caption = #28216#25103#36873#39033'(1)'
      ImageIndex = 7
      object Label60: TLabel
        Left = 328
        Top = 12
        Width = 96
        Height = 12
        Caption = #20154#29289#36215#36215#22987#26435#38480#65306
      end
      object GroupBox28: TGroupBox
        Left = 8
        Top = 8
        Width = 145
        Height = 144
        Caption = #28216#25103#27169#24335
        TabOrder = 0
        object CheckBoxTestServer: TCheckBox
          Left = 8
          Top = 16
          Width = 73
          Height = 17
          Hint = #27979#35797#27169#24335#65292#25171#24320#27492#27169#24335#65292#21487#23545#26381#21153#22120#21508#39033#21442#25968#21450#21151#33021#36827#34892#27979#35797#12290
          Caption = #27979#35797#27169#24335
          TabOrder = 0
          OnClick = CheckBoxTestServerClick
        end
        object CheckBoxServiceMode: TCheckBox
          Left = 8
          Top = 33
          Width = 73
          Height = 17
          Hint = #20813#36153#27169#24335#65292#25171#24320#27492#20808#39033#23558#19981#23545#29992#25143#35745#36153#12290
          Caption = #20813#36153#27169#24335
          TabOrder = 1
          OnClick = CheckBoxServiceModeClick
        end
        object CheckBoxVentureMode: TCheckBox
          Left = 8
          Top = 50
          Width = 81
          Height = 17
          Caption = #19981#21047#24618#27169#24335
          TabOrder = 2
          OnClick = CheckBoxVentureModeClick
        end
        object CheckBoxNonPKMode: TCheckBox
          Left = 8
          Top = 68
          Width = 81
          Height = 17
          Caption = #31105#27490'PK'#27169#24335
          TabOrder = 3
          OnClick = CheckBoxNonPKModeClick
        end
        object chkOffLineShop: TCheckBox
          Left = 8
          Top = 85
          Width = 94
          Height = 16
          Hint = #36873#20013#35813#39033#21518#65292#20154#29289#25346#26426#26102#22914#26524#22788#20110#25670#25674#29366#24577#65292#23558#33258#21160#25910#25674#65288#38480#20223'herom2'#25670#25674#65289
          Caption = #31105#27490#25346#26426#25670#25674
          TabOrder = 4
          OnClick = chkOffLineShopClick
        end
        object chkOffLineHero: TCheckBox
          Left = 8
          Top = 101
          Width = 94
          Height = 16
          Hint = #36873#20013#35813#39033#21518#65292#20154#29289#25346#26426#26102#22914#26524#33521#38596#22312#32447#65292#23558#33258#21160#25910#22238#33521#38596#12290
          Caption = #31105#27490#33521#38596#25346#26426
          TabOrder = 5
          OnClick = chkOffLineHeroClick
        end
        object chkOffLineSlave: TCheckBox
          Left = 8
          Top = 117
          Width = 94
          Height = 16
          Hint = #36873#20013#35813#39033#21518#65292#20154#29289#25346#26426#26102#22914#26524#23453#23453#22312#32447#65292#23558#33258#21160#25910#22238#23453#23453#12290
          Caption = #31105#27490#23453#23453#25346#26426
          TabOrder = 6
          OnClick = chkOffLineSlaveClick
        end
      end
      object GroupBox29: TGroupBox
        Left = 8
        Top = 159
        Width = 145
        Height = 98
        Caption = #27979#35797#27169#24335
        TabOrder = 6
        object Label61: TLabel
          Left = 8
          Top = 22
          Width = 60
          Height = 12
          Caption = #24320#22987#31561#32423#65306
        end
        object Label62: TLabel
          Left = 8
          Top = 46
          Width = 60
          Height = 12
          Caption = #24320#22987#37329#24065#65306
        end
        object Label63: TLabel
          Left = 8
          Top = 70
          Width = 60
          Height = 12
          Caption = #20154#25968#38480#21046#65306
        end
        object seTestLevel: TSpinEditEx
          Left = 68
          Top = 18
          Width = 69
          Height = 21
          Hint = #20154#29289#36215#22987#31561#32423#12290
          MaxValue = 20000
          MinValue = 0
          TabOrder = 0
          Value = 10
          OnChange = seTestLevelChange
          OnKeyDown = seTestLevelKeyDown
        end
        object seTestGold: TSpinEditEx
          Left = 68
          Top = 42
          Width = 69
          Height = 21
          Hint = #27979#35797#27169#24335#20154#29289#36215#22987#37329#24065#25968#12290
          Increment = 1000
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = seTestGoldChange
        end
        object seTestUserLimit: TSpinEditEx
          Left = 68
          Top = 66
          Width = 69
          Height = 21
          Hint = #27979#35797#27169#24335#26368#39640#21487#19978#32447#20154#25968#38480#21046#12290
          Increment = 10
          MaxValue = 10000
          MinValue = 0
          TabOrder = 2
          Value = 10
          OnChange = seTestUserLimitChange
        end
      end
      object ButtonOptionSave0: TButton
        Left = 456
        Top = 265
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 8
        OnClick = ButtonOptionSave0Click
      end
      object GroupBox31: TGroupBox
        Left = 328
        Top = 157
        Width = 105
        Height = 48
        Caption = #19978#32447#20154#25968#38480#21046
        TabOrder = 4
        object Label64: TLabel
          Left = 8
          Top = 23
          Width = 36
          Height = 12
          Caption = #20154#25968#65306
        end
        object seUserFull: TSpinEditEx
          Left = 44
          Top = 19
          Width = 53
          Height = 21
          Hint = #26368#26032#21487#19978#32447#20154#25968#38480#21046#65292#36229#36807#27492#20154#25968#21518#19978#32447#23558#25552#31034#32418#23383#12290
          MaxValue = 10000
          MinValue = 0
          TabOrder = 0
          Value = 1000
          OnChange = seUserFullChange
        end
      end
      object GroupBox33: TGroupBox
        Left = 160
        Top = 8
        Width = 161
        Height = 74
        Caption = #20154#29289#36523#19978#37329#24065#25968#38480#21046
        TabOrder = 1
        object Label68: TLabel
          Left = 8
          Top = 23
          Width = 60
          Height = 12
          Caption = #27491#24335#27169#24335#65306
        end
        object Label69: TLabel
          Left = 8
          Top = 47
          Width = 60
          Height = 12
          Caption = #35797#29609#27169#24335#65306
        end
        object seHumanMaxGold: TSpinEditEx
          Left = 67
          Top = 19
          Width = 85
          Height = 21
          Increment = 10000
          MaxValue = 2147483647
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seHumanMaxGoldChange
        end
        object seHumanTryModeMaxGold: TSpinEditEx
          Left = 67
          Top = 43
          Width = 85
          Height = 21
          Increment = 10000
          MaxValue = 2147483647
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = seHumanTryModeMaxGoldChange
        end
      end
      object GroupBox34: TGroupBox
        Left = 160
        Top = 89
        Width = 161
        Height = 63
        Caption = #35797#29609#31561#32423#38480#21046
        TabOrder = 3
        object Label70: TLabel
          Left = 32
          Top = 21
          Width = 36
          Height = 12
          Caption = #31561#32423#65306
        end
        object seTryModeLevel: TSpinEditEx
          Left = 67
          Top = 17
          Width = 85
          Height = 21
          MaxValue = 100
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seTryModeLevelChange
        end
        object CheckBoxTryModeUseStorage: TCheckBox
          Left = 36
          Top = 41
          Width = 121
          Height = 17
          Hint = #35797#29609#27169#24335#20801#35768#20351#29992#20179#24211#12290
          Caption = #35797#29609#27169#24335#20351#29992#20179#24211
          TabOrder = 1
          OnClick = CheckBoxTryModeUseStorageClick
        end
      end
      object GroupBox19: TGroupBox
        Left = 437
        Top = 157
        Width = 89
        Height = 48
        Caption = #32452#38431#25104#21592#25968#37327
        TabOrder = 5
        object Label41: TLabel
          Left = 8
          Top = 23
          Width = 36
          Height = 12
          Caption = #25968#37327#65306
        end
        object seGroupMembersMax: TSpinEditEx
          Left = 44
          Top = 19
          Width = 37
          Height = 21
          Hint = #32452#38431#25104#21592#25968#37327#12290
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seGroupMembersMaxChange
        end
      end
      object RadioGroupMaxLevel: TRadioGroup
        Left = 328
        Top = 33
        Width = 198
        Height = 38
        Hint = #20154#29289#25110#24618#29289#30340#26368#39640#31561#32423#12289'HP'#12289'MP'#38480#21046#65292'IP'#29256#30331#38470#22120#35831#21247#36873#25321'21'#20159#65292#21542#21017#28216#25103#20013#26174#34880#38169#35823
        Caption = #26368#39640#31561#32423#12289'HP'#12289'MP'#38480#21046
        Columns = 3
        Items.Strings = (
          '65535'
          '21'#20159
          '42'#20159)
        TabOrder = 2
        OnClick = RadioGroupMaxLevelClick
      end
      object grp2: TGroupBox
        Left = 160
        Top = 159
        Width = 161
        Height = 98
        Caption = #34892#20250#35774#32622
        TabOrder = 7
        object lbl3: TLabel
          Left = 8
          Top = 23
          Width = 60
          Height = 12
          Caption = #20154#25968#38480#21046#65306
        end
        object Label212: TLabel
          Left = 8
          Top = 47
          Width = 60
          Height = 12
          Caption = #21517#23383#38271#24230#65306
        end
        object Label213: TLabel
          Left = 8
          Top = 71
          Width = 60
          Height = 12
          Caption = #23553#21495#38271#24230#65306
        end
        object seGuildMemberMaxLimit: TSpinEditEx
          Left = 67
          Top = 19
          Width = 85
          Height = 21
          Hint = #34892#20250#20154#25968#38480#21046
          MaxValue = 10000
          MinValue = 0
          TabOrder = 0
          Value = 1000
          OnChange = seGuildMemberMaxLimitChange
        end
        object seGuildNameLen: TSpinEditEx
          Left = 67
          Top = 43
          Width = 85
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seGuildNameLenChange
        end
        object seGuildRankNameLen: TSpinEditEx
          Left = 67
          Top = 67
          Width = 85
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
          OnChange = seGuildRankNameLenChange
        end
      end
      object rgMaxAC: TRadioGroup
        Left = 328
        Top = 74
        Width = 198
        Height = 38
        Hint = #20154#29289#25110#24618#29289#30340#26368#39640#31561#32423#12289'HP'#12289'MP'#38480#21046#65292'IP'#29256#30331#38470#22120#35831#21247#36873#25321'21'#20159#65292#21542#21017#28216#25103#20013#26174#34880#38169#35823
        Caption = 'AC,MAC,DC,MC,SC'#38480#21046
        Columns = 2
        Items.Strings = (
          '65535'
          '21'#20159)
        TabOrder = 9
        OnClick = rgMaxACClick
      end
      object GroupBox3: TGroupBox
        Left = 328
        Top = 208
        Width = 198
        Height = 49
        Caption = #20154#29289#30331#24405#25110#20999#25442#22320#22270#20445#25252
        TabOrder = 10
        object Label13: TLabel
          Left = 8
          Top = 22
          Width = 60
          Height = 12
          Caption = #20445#25252#26102#38388#65306
        end
        object lbl17: TLabel
          Left = 167
          Top = 22
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object seHumChgMapOrLoginProtectTime: TSpinEditEx
          Left = 68
          Top = 18
          Width = 97
          Height = 21
          Hint = #20154#29289#30331#24405#25110#20999#25442#22320#22270#21518#65292#21463#20445#25252#30340#26102#38388
          MaxValue = 10000
          MinValue = 0
          TabOrder = 0
          Value = 10
          OnChange = seHumChgMapOrLoginProtectTimeChange
        end
      end
      object rgMaxHitPoint: TRadioGroup
        Left = 328
        Top = 116
        Width = 198
        Height = 38
        Caption = #20934#30830#12289#25935#25463#38480#21046
        Columns = 2
        Items.Strings = (
          '255'
          '65535')
        TabOrder = 11
        OnClick = rgMaxHitPointClick
      end
      object seStartPermission: TSpinEditEx
        Left = 423
        Top = 8
        Width = 102
        Height = 21
        Hint = #20154#29289#28216#25103#36215#22987#26435#38480#65292#40664#35748#20026'0'#12290
        MaxValue = 10
        MinValue = 0
        TabOrder = 12
        Value = 10
        OnChange = seStartPermissionChange
      end
    end
    object TabSheet1: TTabSheet
      Caption = #24231#26631#33539#22260
      ImageIndex = 4
      object ButtonOptionSave: TButton
        Left = 448
        Top = 245
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 6
        OnClick = ButtonOptionSaveClick
      end
      object GroupBox16: TGroupBox
        Left = 8
        Top = 8
        Width = 105
        Height = 44
        Caption = #23433#20840#21306#33539#22260
        TabOrder = 0
        object Label39: TLabel
          Left = 8
          Top = 21
          Width = 30
          Height = 12
          Caption = #22823#23567':'
        end
        object EditSafeZoneSize: TSpinEditEx
          Left = 44
          Top = 17
          Width = 45
          Height = 21
          Hint = #23433#20840#21306#33539#22260#22823#23567#12290
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditSafeZoneSizeChange
        end
      end
      object GroupBox18: TGroupBox
        Left = 8
        Top = 63
        Width = 105
        Height = 44
        Caption = #26032#20154#20986#29983#28857#33539#22260
        TabOrder = 3
        object Label40: TLabel
          Left = 8
          Top = 22
          Width = 30
          Height = 12
          Caption = #33539#22260':'
        end
        object EditStartPointSize: TSpinEditEx
          Left = 44
          Top = 18
          Width = 45
          Height = 21
          Hint = #26032#20154#20986#29983#28857#25511#21046#65292#40664#35748#20026#21069#19977#20010#23433#20840#21306#35774#32622#12290
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditStartPointSizeChange
        end
      end
      object GroupBox20: TGroupBox
        Left = 120
        Top = 8
        Width = 145
        Height = 89
        Caption = #32418#21517#26449
        TabOrder = 1
        object Label42: TLabel
          Left = 8
          Top = 44
          Width = 36
          Height = 12
          Caption = #22352#26631'X:'
        end
        object Label43: TLabel
          Left = 8
          Top = 68
          Width = 36
          Height = 12
          Caption = #22352#26631'Y:'
        end
        object Label44: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #22320#22270':'
        end
        object EditRedHomeX: TSpinEditEx
          Left = 52
          Top = 40
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditRedHomeXChange
        end
        object EditRedHomeY: TSpinEditEx
          Left = 52
          Top = 64
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditRedHomeYChange
        end
        object EditRedHomeMap: TEdit
          Left = 52
          Top = 16
          Width = 73
          Height = 20
          Hint = #32418#21517#20154#29289#38598#20013#28857#22320#22270#21517#31216#12290
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = '3'
          OnChange = EditRedHomeMapChange
        end
      end
      object GroupBox21: TGroupBox
        Left = 120
        Top = 104
        Width = 145
        Height = 89
        Caption = #32418#21517#27515#20129#22238#22478#28857
        TabOrder = 4
        object Label45: TLabel
          Left = 8
          Top = 44
          Width = 36
          Height = 12
          Caption = #22352#26631'X:'
        end
        object Label46: TLabel
          Left = 8
          Top = 68
          Width = 36
          Height = 12
          Caption = #22352#26631'Y:'
        end
        object Label47: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #22320#22270':'
        end
        object EditRedDieHomeX: TSpinEditEx
          Left = 52
          Top = 40
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditRedDieHomeXChange
        end
        object EditRedDieHomeY: TSpinEditEx
          Left = 52
          Top = 64
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditRedDieHomeYChange
        end
        object EditRedDieHomeMap: TEdit
          Left = 52
          Top = 16
          Width = 73
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = '3'
          OnChange = EditRedDieHomeMapChange
        end
      end
      object GroupBox22: TGroupBox
        Left = 272
        Top = 8
        Width = 145
        Height = 89
        Caption = #24212#24613#22238#22478#28857
        TabOrder = 2
        object Label48: TLabel
          Left = 8
          Top = 44
          Width = 36
          Height = 12
          Caption = #22352#26631'X:'
        end
        object Label49: TLabel
          Left = 8
          Top = 68
          Width = 36
          Height = 12
          Caption = #22352#26631'Y:'
        end
        object Label50: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #22320#22270':'
        end
        object EditHomeX: TSpinEditEx
          Left = 52
          Top = 40
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditHomeXChange
        end
        object EditHomeY: TSpinEditEx
          Left = 52
          Top = 64
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = EditHomeYChange
        end
        object EditHomeMap: TEdit
          Left = 52
          Top = 16
          Width = 73
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = '3'
          OnChange = EditHomeMapChange
        end
      end
      object grp4: TGroupBox
        Left = 8
        Top = 112
        Width = 105
        Height = 133
        Caption = #23433#20840#21306#25552#31034
        TabOrder = 5
        object Label215: TLabel
          Left = 8
          Top = 38
          Width = 36
          Height = 12
          Caption = #22352#26631'Y:'
        end
        object Label216: TLabel
          Left = 8
          Top = 62
          Width = 36
          Height = 12
          Caption = #25991' '#23383':'
        end
        object Label217: TLabel
          Left = 8
          Top = 86
          Width = 36
          Height = 12
          Caption = #32972' '#26223':'
        end
        object Label218: TLabel
          Left = 8
          Top = 110
          Width = 36
          Height = 12
          Caption = #22823' '#23567':'
        end
        object chkHintSafeZone: TCheckBox
          Left = 4
          Top = 15
          Width = 94
          Height = 17
          Hint = #21246#36873#21518#36827#20837#25110#31163#24320#23433#20840#21306#26174#31034#25552#31034
          Caption = #36827#20837#31163#24320#25552#31034
          TabOrder = 0
          OnClick = chkHintSafeZoneClick
        end
        object seHintSafeZoneY: TSpinEditEx
          Left = 44
          Top = 34
          Width = 56
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = seHintSafeZoneYChange
        end
        object seHintSafeZoneFColor: TColorIndexEdit
          Left = 44
          Top = 58
          Width = 57
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 255
          OnChange = seHintSafeZoneFColorChange
          ShowNoneColor = False
        end
        object seHintSafeZoneBColor: TColorIndexEdit
          Left = 44
          Top = 82
          Width = 57
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 3
          Value = 255
          OnChange = seHintSafeZoneBColorChange
          ShowNoneColor = False
        end
        object seHintSafeZoneFSize: TSpinEditEx
          Left = 44
          Top = 106
          Width = 56
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 4
          Value = 10
          OnChange = seHintSafeZoneFSizeChange
        end
      end
      object GroupBox82: TGroupBox
        Left = 272
        Top = 104
        Width = 145
        Height = 89
        Caption = #22269#23478#27169#24335#25511#21046
        TabOrder = 7
        object Label225: TLabel
          Left = 8
          Top = 68
          Width = 54
          Height = 12
          Caption = #22269#32842#32423#21035':'
        end
        object chkNationGroupCheck: TCheckBox
          Left = 8
          Top = 17
          Width = 121
          Height = 17
          Caption = #19981#21516#22269#23478#31105#27490#32452#38431
          TabOrder = 0
          OnClick = chkNationGroupCheckClick
        end
        object chkNationGuildCheck: TCheckBox
          Left = 8
          Top = 42
          Width = 121
          Height = 17
          Caption = #19981#21516#22269#23478#31105#27490#20837#20250
          TabOrder = 1
          OnClick = chkNationGuildCheckClick
        end
        object seNationSayLevel: TSpinEditEx
          Left = 68
          Top = 64
          Width = 53
          Height = 21
          Hint = #20154#29289#36798#21040#22810#23569#32423#26102#25165#20801#35768#21457#36865#22269#32842#20449#24687
          MaxValue = 100
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = seNationSayLevelChange
        end
      end
    end
    object TabSheet3: TTabSheet
      Caption = 'PK'#25511#21046
      ImageIndex = 6
      object ButtonOptionSave2: TButton
        Left = 368
        Top = 181
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 4
        OnClick = ButtonOptionSave2Click
      end
      object GroupBox23: TGroupBox
        Left = 8
        Top = 8
        Width = 153
        Height = 73
        Caption = #33258#21160#20943'PK'#28857#25511#21046
        TabOrder = 0
        object Label51: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #38388#38548#26102#38388':'
        end
        object Label52: TLabel
          Left = 8
          Top = 44
          Width = 54
          Height = 12
          Caption = #19968#27425#28857#25968':'
        end
        object Label53: TLabel
          Left = 128
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object EditDecPkPointTime: TSpinEditEx
          Left = 68
          Top = 16
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditDecPkPointTimeChange
        end
        object EditDecPkPointCount: TSpinEditEx
          Left = 68
          Top = 40
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = EditDecPkPointCountChange
        end
      end
      object GroupBox24: TGroupBox
        Left = 8
        Top = 88
        Width = 153
        Height = 43
        Caption = 'PK'#29366#24577#21464#33394'('#31186')'
        TabOrder = 2
        object Label54: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #26102#38388':'
        end
        object EditPKFlagTime: TSpinEditEx
          Left = 40
          Top = 16
          Width = 105
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditPKFlagTimeChange
        end
      end
      object GroupBox25: TGroupBox
        Left = 8
        Top = 136
        Width = 153
        Height = 89
        Caption = #26432#20154#21152'PK'#28857#25968
        TabOrder = 3
        object Label55: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #20154#29289#26432#20154':'
        end
        object lbl5: TLabel
          Left = 8
          Top = 43
          Width = 54
          Height = 12
          Caption = #20551#20154#26432#20154':'
        end
        object Label226: TLabel
          Left = 8
          Top = 67
          Width = 54
          Height = 12
          Caption = #33521#38596#26432#20154':'
        end
        object seHumanAddPKPoint: TSpinEditEx
          Left = 62
          Top = 16
          Width = 83
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seHumanAddPKPointChange
        end
        object seDummyAddPKPoint: TSpinEditEx
          Left = 62
          Top = 39
          Width = 83
          Height = 21
          MaxValue = 2000
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = seDummyAddPKPointChange
        end
        object seKillHeroAddPKPoint: TSpinEditEx
          Left = 62
          Top = 62
          Width = 83
          Height = 21
          MaxValue = 2000
          MinValue = 0
          TabOrder = 2
          Value = 10
          OnChange = seKillHeroAddPKPointChange
        end
      end
      object GroupBox32: TGroupBox
        Left = 168
        Top = 8
        Width = 265
        Height = 169
        Caption = 'PK'#35268#21017
        TabOrder = 1
        object Label58: TLabel
          Left = 112
          Top = 20
          Width = 66
          Height = 12
          Caption = #22686#21152#31561#32423#25968':'
        end
        object Label65: TLabel
          Left = 112
          Top = 44
          Width = 66
          Height = 12
          Caption = #20943#23569#31561#32423#25968':'
        end
        object Label66: TLabel
          Left = 112
          Top = 68
          Width = 66
          Height = 12
          Caption = #22686#21152#32463#39564#25968':'
        end
        object Label56: TLabel
          Left = 112
          Top = 92
          Width = 66
          Height = 12
          Caption = #20943#23569#32463#39564#25968':'
        end
        object Label67: TLabel
          Left = 8
          Top = 92
          Width = 42
          Height = 12
          Caption = 'PK'#31561#32423':'
        end
        object Label114: TLabel
          Left = 112
          Top = 116
          Width = 66
          Height = 12
          Caption = 'PK'#20445#25252#31561#32423':'
        end
        object Label115: TLabel
          Left = 89
          Top = 140
          Width = 90
          Height = 12
          Caption = #32418#21517'PK'#20445#25252#31561#32423':'
        end
        object CheckBoxKillHumanWinLevel: TCheckBox
          Left = 8
          Top = 18
          Width = 97
          Height = 17
          Caption = #26432#20154#22686#21152#31561#32423
          TabOrder = 1
          OnClick = CheckBoxKillHumanWinLevelClick
        end
        object CheckBoxKilledLostLevel: TCheckBox
          Left = 8
          Top = 36
          Width = 97
          Height = 17
          Caption = #34987#26432#20943#31561#32423
          TabOrder = 2
          OnClick = CheckBoxKilledLostLevelClick
        end
        object CheckBoxKilledLostExp: TCheckBox
          Left = 8
          Top = 68
          Width = 97
          Height = 17
          Caption = #34987#26432#20943#32463#39564
          TabOrder = 6
          OnClick = CheckBoxKilledLostExpClick
        end
        object CheckBoxKillHumanWinExp: TCheckBox
          Left = 8
          Top = 52
          Width = 97
          Height = 17
          Caption = #26432#20154#22686#21152#32463#39564
          TabOrder = 4
          OnClick = CheckBoxKillHumanWinExpClick
        end
        object EditKillHumanWinLevel: TSpinEditEx
          Left = 184
          Top = 16
          Width = 73
          Height = 21
          MaxValue = 100
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditKillHumanWinLevelChange
        end
        object EditKilledLostLevel: TSpinEditEx
          Left = 184
          Top = 40
          Width = 73
          Height = 21
          MaxValue = 100
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = EditKilledLostLevelChange
        end
        object EditKillHumanWinExp: TSpinEditEx
          Left = 184
          Top = 64
          Width = 73
          Height = 21
          Increment = 1000
          MaxValue = 200000000
          MinValue = 1
          TabOrder = 5
          Value = 10
          OnChange = EditKillHumanWinExpChange
        end
        object EditKillHumanLostExp: TSpinEditEx
          Left = 184
          Top = 88
          Width = 73
          Height = 21
          Increment = 1000
          MaxValue = 200000000
          MinValue = 1
          TabOrder = 8
          Value = 10
          OnChange = EditKillHumanLostExpChange
        end
        object EditHumanLevelDiffer: TSpinEditEx
          Left = 56
          Top = 88
          Width = 49
          Height = 21
          MaxValue = 100
          MinValue = 1
          TabOrder = 7
          Value = 10
          OnChange = EditHumanLevelDifferChange
        end
        object CheckBoxPKLevelProtect: TCheckBox
          Left = 8
          Top = 116
          Width = 89
          Height = 17
          Hint = 
            #21551#29992'PK'#20445#25252#21151#33021#65292#25171#24320#27492#21151#33021#21518#65292#28216#25103#20013#39640#20110#20445#25252#31561#32423#30340#20154#29289#23558#19981#21487#20197#26432#20302#20110#20445#25252#31561#32423#30340#20154#29289'('#20302#31561#32423#20154#29289#20808#25915#20987#21464#33394#38500#22806')'#65292#20302#20110#20445#25252#31561#32423#30340 +
            #20154#29289#20063#19981#21487#20197#26432#39640#20110#20445#25252#31561#32423#30340#20154#29289'('#39640#31561#32423#20154#29289#20808#25915#20987#21464#33394#38500#22806')'#12290
          Caption = #26222#36890'PK'#20445#25252
          TabOrder = 10
          OnClick = CheckBoxPKLevelProtectClick
        end
        object EditPKProtectLevel: TSpinEditEx
          Left = 184
          Top = 112
          Width = 73
          Height = 21
          Hint = #20445#25252#31561#32423#12290#27492#31561#32423#20197#19979#20154#29289#21463#20445#25252#65292#20294#20808#25915#20987#21464#33394#21017#19981#21463#20445#25252#12290
          MaxValue = 65535
          MinValue = 1
          TabOrder = 9
          Value = 10
          OnChange = EditPKProtectLevelChange
        end
        object EditRedPKProtectLevel: TSpinEditEx
          Left = 184
          Top = 136
          Width = 73
          Height = 21
          Hint = 
            #32418#21517#20154#29289'PK'#20445#25252#65292#39640#20110#20445#25252#31561#32423#30340#32418#21517#20154#29289#19981#21487#20197#26432#20302#20110#20445#25252#31561#32423#26410#32418#21517#20154#29289#12290#20302#20110#20445#25252#31561#32423#26410#32418#21517#30340#20154#29289#20063#19981#21487#20197#26432#39640#20110#20445#25252#31561#32423#30340#32418#21517#20154#29289 +
            #12290
          MaxValue = 65535
          MinValue = 1
          TabOrder = 11
          Value = 10
          OnChange = EditRedPKProtectLevelChange
        end
      end
      object GroupBox27: TGroupBox
        Left = 8
        Top = 229
        Width = 153
        Height = 63
        Caption = #26432#20154#27494#22120#35781#21650
        TabOrder = 5
        object Label211: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #26426#29575':'
        end
        object seKillHumanWeaponUnlockRate: TSpinEditEx
          Left = 40
          Top = 16
          Width = 105
          Height = 21
          Hint = #20540#36234#23567','#27494#22120#36234#23481#26131#34987#35781#21650
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seKillHumanWeaponUnlockRateChange
        end
        object chkHeroKillHumanNotWeaponUnlock: TCheckBox
          Left = 8
          Top = 42
          Width = 129
          Height = 17
          Hint = #21246#36873#21518#33521#38596#26432#27515#20154#29289#27494#22120#19981#34987#35781#21650
          Caption = #33521#38596#26432#20154#19981#35781#21650#27494#22120
          TabOrder = 1
          OnClick = chkHeroKillHumanNotWeaponUnlockClick
        end
      end
    end
    object TabSheet2: TTabSheet
      Caption = #28216#25103#36873#39033'(2)'
      ImageIndex = 5
      object GroupBox17: TGroupBox
        Left = 280
        Top = 8
        Width = 169
        Height = 262
        Caption = #36305#27493#31359#20154#25511#21046
        TabOrder = 2
        object chkDisHumRun: TCheckBox
          Left = 4
          Top = 17
          Width = 79
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#19981#20801#35768#31359#36807#24618#29289#25110#20854#23427#20154#29289
          Caption = #31105#27490#36305#27493#31359#20154
          TabOrder = 0
          OnClick = chkDisHumRunClick
        end
        object chkRunHum: TCheckBox
          Left = 20
          Top = 35
          Width = 98
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#20854#20182#20154#29289
          Caption = #20801#35768#31359#36807#20154#29289
          TabOrder = 1
          OnClick = chkRunHumClick
        end
        object chkRunMon: TCheckBox
          Left = 20
          Top = 53
          Width = 99
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#24618#29289
          Caption = #20801#35768#31359#36807#24618#29289
          TabOrder = 2
          OnClick = chkRunMonClick
        end
        object chkWarDisHumRun: TCheckBox
          Left = 20
          Top = 143
          Width = 117
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#22312#25915#22478#21306#22495#65292#23558#31105#27490#31359#20154#21450#24618#29289
          Caption = #25915#22478#21306#22495#20840#37096#31105#27490
          TabOrder = 7
          OnClick = chkWarDisHumRunClick
        end
        object chkRunNpc: TCheckBox
          Left = 20
          Top = 71
          Width = 99
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807'NPC'
          Caption = #20801#35768#31359#36807'NPC'
          TabOrder = 3
          OnClick = chkRunNpcClick
        end
        object chkGMRunAll: TCheckBox
          Left = 20
          Top = 107
          Width = 110
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#36229#32423#28216#25103#31649#29702#21592#19981#21463#20197#19978#35774#32622#38480#21046#12290
          Caption = #31649#29702#21592#19981#21463#25511#21046
          TabOrder = 5
          OnClick = chkGMRunAllClick
        end
        object chkRunGuard: TCheckBox
          Left = 20
          Top = 89
          Width = 99
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20154#29289#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
          Caption = #20801#35768#31359#36807#23432#21355
          TabOrder = 4
          OnClick = chkRunGuardClick
        end
        object chkSafeArea: TCheckBox
          Left = 20
          Top = 125
          Width = 102
          Height = 17
          Caption = #23433#20840#21306#19981#21463#25511#21046
          TabOrder = 6
          OnClick = chkSafeAreaClick
        end
        object chkWarHreoRun: TCheckBox
          Left = 20
          Top = 161
          Width = 125
          Height = 17
          Hint = #25171#24320#35813#39033#21518#25915#22478#21306#22495'['#25915#22478#26399#38388']'#23558#20801#35768#31359#36807#33521#38596
          Caption = #25915#22478#21306#22495#20801#35768#31359#33521#38596
          Enabled = False
          TabOrder = 8
          OnClick = chkWarHreoRunClick
        end
        object chkSafeAreaDisNpcRun: TCheckBox
          Left = 20
          Top = 197
          Width = 125
          Height = 17
          Caption = #23433#20840#21306#31105#27490#31359'NPC'
          TabOrder = 10
          OnClick = chkSafeAreaDisNpcRunClick
        end
        object chkSafeAreaDisShopStallHumRun: TCheckBox
          Left = 20
          Top = 215
          Width = 142
          Height = 17
          Caption = #23433#20840#21306#31105#27490#31359#25670#25674#20154#29289
          TabOrder = 11
          OnClick = chkSafeAreaDisShopStallHumRunClick
        end
        object chkSafeAreaDisOffLineHumRun: TCheckBox
          Left = 20
          Top = 233
          Width = 142
          Height = 17
          Caption = #23433#20840#21306#31105#27490#31359#31163#32447#20154#29289
          TabOrder = 12
          OnClick = chkSafeAreaDisOffLineHumRunClick
        end
        object chkWarDisTeleport: TCheckBox
          Left = 20
          Top = 179
          Width = 129
          Height = 17
          Caption = #25915#22478#21306#22495#31105#27490#20256#36865
          TabOrder = 9
          OnClick = chkWarDisTeleportClick
        end
      end
      object ButtonOptionSave3: TButton
        Left = 456
        Top = 261
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 5
        OnClick = ButtonOptionSave3Click
      end
      object GroupBox53: TGroupBox
        Left = 8
        Top = 8
        Width = 129
        Height = 113
        Caption = #20132#26131#25511#21046
        TabOrder = 0
        object Label20: TLabel
          Left = 8
          Top = 20
          Width = 60
          Height = 12
          Caption = #20132#26131#38388#38548#65306
        end
        object Label104: TLabel
          Left = 8
          Top = 44
          Width = 60
          Height = 12
          Caption = #30830#35748#20132#26131#65306
        end
        object Label105: TLabel
          Left = 107
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label106: TLabel
          Left = 107
          Top = 43
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seTryDealTime: TSpinEditEx
          Left = 68
          Top = 16
          Width = 37
          Height = 21
          Hint = #20851#38381#20132#26131#21518#65292#20877#37325#26032#20132#26131#24517#39035#38388#38548#25351#23450#26102#38388#12290
          MaxValue = 10
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seTryDealTimeChange
        end
        object seDealOKTime: TSpinEditEx
          Left = 68
          Top = 40
          Width = 37
          Height = 21
          Hint = #25918#19978#20132#26131#29289#21697#21518#65292#24517#39035#31561#25351#23450#26102#38388#20877#25353#30830#35748#25353#38062#12290
          MaxValue = 10
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = seDealOKTimeChange
        end
        object chkCanNotGetBackDeal: TCheckBox
          Left = 8
          Top = 72
          Width = 110
          Height = 13
          Hint = #25171#24320#27492#21151#33021#21518#65292#20132#26131#30340#29289#21697#25918#19978#20102#21518#23558#19981#21487#20197#21462#22238#65292#21482#33021#21462#28040#20132#26131#20877#37325#26032#20132#26131#12290
          Caption = #31105#27490#21462#22238#29289#21697
          TabOrder = 2
          OnClick = chkCanNotGetBackDealClick
        end
        object chkDisableDeal: TCheckBox
          Left = 8
          Top = 88
          Width = 70
          Height = 13
          Hint = #31105#27490#20132#26131#21518#65292#22312#28216#25103#20013#23558#19981#20801#35768#36827#34892#20132#26131#12290
          Caption = #31105#27490#20132#26131
          TabOrder = 3
          OnClick = chkDisableDealClick
        end
      end
      object GroupBox64: TGroupBox
        Left = 144
        Top = 8
        Width = 129
        Height = 113
        Caption = #25172#29289#21697#25511#21046
        TabOrder = 1
        object Label118: TLabel
          Left = 8
          Top = 44
          Width = 60
          Height = 12
          Caption = #29289#21697#20215#26684#65306
        end
        object Label119: TLabel
          Left = 8
          Top = 68
          Width = 36
          Height = 12
          Caption = #37329#24065#65306
        end
        object seCanDropPrice: TSpinEditEx
          Left = 68
          Top = 40
          Width = 53
          Height = 21
          Hint = #23567#20110#27492#20215#26684#30340#29289#21697#65292#25172#20986#21518#31435#21363#28040#22833#65292#19981#20250#20986#29616#22312#22320#19978#12290
          Increment = 100
          MaxValue = 20000000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = seCanDropPriceChange
        end
        object chkControlDropItem: TCheckBox
          Left = 11
          Top = 20
          Width = 110
          Height = 13
          Hint = #25171#24320#27492#21151#33021#21518#65292#23558#23545#20154#29289#25172#19979#26469#30340#29289#21697#21450#37329#24065#36827#34892#26816#26597#65292#23567#20110#25351#23450#35268#21017#37329#24065#25110#20215#26684#30340#29289#21697#23558#19981#20801#35768#25172#19979#26469#65292#25110#25172#19979#26469#31435#21363#28040#22833#12290
          Caption = #21551#29992#25172#29289#21697#25511#21046
          TabOrder = 0
          OnClick = chkControlDropItemClick
        end
        object seCanDropGold: TSpinEditEx
          Left = 68
          Top = 64
          Width = 53
          Height = 21
          Hint = #23567#20110#25351#23450#25968#37327#30340#37329#24065#65292#23558#31105#27490#25172#20986#12290
          Increment = 100
          MaxValue = 20000000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = seCanDropGoldChange
        end
        object chkIsSafeDisableDrop: TCheckBox
          Left = 11
          Top = 92
          Width = 110
          Height = 13
          Hint = #25171#24320#27492#21151#33021#21518#65292#22312#23433#20840#21306#23558#19981#20801#35768#25172#29289#21697#12290
          Caption = #23433#20840#21306#31105#27490#25172
          TabOrder = 3
          OnClick = chkIsSafeDisableDropClick
        end
      end
      object GroupBox79: TGroupBox
        Left = 8
        Top = 129
        Width = 125
        Height = 137
        Caption = #25361#25112#25511#21046
        TabOrder = 3
        object Label163: TLabel
          Left = 8
          Top = 20
          Width = 60
          Height = 12
          Caption = #25361#25112#38388#38548#65306
        end
        object Label164: TLabel
          Left = 8
          Top = 44
          Width = 60
          Height = 12
          Caption = #30830#35748#25361#25112#65306
        end
        object Label165: TLabel
          Left = 107
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label166: TLabel
          Left = 107
          Top = 43
          Width = 12
          Height = 12
          Caption = #31186
        end
        object lbl8: TLabel
          Left = 8
          Top = 68
          Width = 60
          Height = 12
          Caption = #25361#25112#26102#38388#65306
        end
        object lbl9: TLabel
          Left = 105
          Top = 68
          Width = 12
          Height = 12
          Caption = #20998
        end
        object seTryChallengeTime: TSpinEditEx
          Left = 68
          Top = 16
          Width = 37
          Height = 21
          Hint = #20851#38381#25361#25112#21518#65292#20877#37325#26032#25361#25112#24517#39035#38388#38548#25351#23450#26102#38388#12290
          MaxValue = 10
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seTryChallengeTimeChange
        end
        object seChallengeOKTime: TSpinEditEx
          Left = 68
          Top = 40
          Width = 37
          Height = 21
          Hint = #25918#19978#25361#25112#29289#21697#21518#65292#24517#39035#31561#25351#23450#26102#38388#20877#25353#30830#35748#25353#38062#12290
          MaxValue = 10
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = seChallengeOKTimeChange
        end
        object chkCanNotGetBackChallenge: TCheckBox
          Left = 8
          Top = 96
          Width = 110
          Height = 13
          Hint = #25171#24320#27492#21151#33021#21518#65292#25361#25112#30340#29289#21697#25918#19978#20102#21518#23558#19981#21487#20197#21462#22238#65292#21482#33021#21462#28040#25361#25112#20877#37325#26032#20132#26131#12290
          Caption = #31105#27490#21462#22238#29289#21697
          TabOrder = 3
          OnClick = chkCanNotGetBackChallengeClick
        end
        object chkDisableChallenge: TCheckBox
          Left = 8
          Top = 112
          Width = 70
          Height = 13
          Hint = #31105#27490#25361#25112#21518#65292#22312#28216#25103#20013#23558#19981#20801#35768#36827#34892#25361#25112#12290
          Caption = #31105#27490#25361#25112
          TabOrder = 4
          OnClick = chkDisableChallengeClick
        end
        object seChallengeTime: TSpinEditEx
          Left = 68
          Top = 64
          Width = 37
          Height = 21
          Hint = #25361#25112#25152#29992#26102#38388
          MaxValue = 10
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = seChallengeTimeChange
        end
      end
      object rgChallengeGold: TRadioGroup
        Left = 144
        Top = 129
        Width = 115
        Height = 136
        Caption = #25361#25112#38468#21152#24065
        ItemIndex = 0
        Items.Strings = (
          #37329#21018#30707
          #20803#23453
          #28789#31526)
        TabOrder = 4
        OnClick = rgChallengeGoldClick
      end
    end
    object GameSpeedSheet: TTabSheet
      Caption = #28216#25103#36895#24230
      object PageControlGameSpeed: TPageControl
        Left = 0
        Top = 0
        Width = 529
        Height = 291
        ActivePage = TabSheet11
        Align = alClient
        TabOrder = 0
        object TabSheet11: TTabSheet
          Caption = #22522#26412
          object lbl15: TLabel
            Left = 104
            Top = 4
            Width = 300
            Height = 12
            Caption = #20351#29992'RunGate'#23553#21152#36895#24314#35758#32593#20851#36895#24230#38480#21046#38388#38548#35843#21040'100'#20197#20869#65281
            Font.Charset = GB2312_CHARSET
            Font.Color = clBlue
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox1: TGroupBox
            Left = 8
            Top = 22
            Width = 90
            Height = 184
            Caption = #38388#38548#25511#21046'('#27627#31186')'
            TabOrder = 0
            object Label1: TLabel
              Left = 5
              Top = 24
              Width = 30
              Height = 12
              Caption = #25915#20987':'
            end
            object Label2: TLabel
              Left = 5
              Top = 48
              Width = 30
              Height = 12
              Caption = #39764#27861':'
            end
            object Label3: TLabel
              Left = 5
              Top = 72
              Width = 30
              Height = 12
              Caption = #36305#27493':'
            end
            object Label4: TLabel
              Left = 5
              Top = 96
              Width = 30
              Height = 12
              Caption = #36208#36335':'
            end
            object Label5: TLabel
              Left = 5
              Top = 144
              Width = 30
              Height = 12
              Caption = #25366#32905':'
            end
            object Label6: TLabel
              Left = 5
              Top = 120
              Width = 30
              Height = 12
              Caption = #36716#21521':'
            end
            object EditHitIntervalTime: TSpinEditEx
              Left = 36
              Top = 20
              Width = 45
              Height = 21
              MaxValue = 2000
              MinValue = 0
              TabOrder = 0
              Value = 400
              OnChange = EditHitIntervalTimeChange
            end
            object EditMagicHitIntervalTime: TSpinEditEx
              Left = 36
              Top = 44
              Width = 45
              Height = 21
              MaxValue = 2000
              MinValue = 0
              TabOrder = 1
              Value = 600
              OnChange = EditMagicHitIntervalTimeChange
            end
            object EditRunIntervalTime: TSpinEditEx
              Left = 36
              Top = 68
              Width = 45
              Height = 21
              MaxValue = 2000
              MinValue = 0
              TabOrder = 2
              Value = 300
              OnChange = EditRunIntervalTimeChange
            end
            object EditWalkIntervalTime: TSpinEditEx
              Left = 36
              Top = 92
              Width = 45
              Height = 21
              MaxValue = 2000
              MinValue = 0
              TabOrder = 3
              Value = 300
              OnChange = EditWalkIntervalTimeChange
            end
            object EditTurnIntervalTime: TSpinEditEx
              Left = 36
              Top = 116
              Width = 45
              Height = 21
              MaxValue = 2000
              MinValue = 0
              TabOrder = 4
              Value = 300
              OnChange = EditTurnIntervalTimeChange
            end
            object EditDigUpIntervalTime: TSpinEditEx
              Left = 36
              Top = 140
              Width = 45
              Height = 21
              MaxValue = 2000
              MinValue = 0
              TabOrder = 5
              Value = 10
              OnChange = EditDigUpIntervalTimeChange
            end
          end
          object GroupBox2: TGroupBox
            Left = 104
            Top = 22
            Width = 73
            Height = 184
            Caption = #25968#25454#37327#25511#21046
            TabOrder = 1
            object Label7: TLabel
              Left = 5
              Top = 24
              Width = 30
              Height = 12
              Caption = #25915#20987':'
            end
            object Label8: TLabel
              Left = 5
              Top = 48
              Width = 30
              Height = 12
              Caption = #39764#27861':'
            end
            object Label9: TLabel
              Left = 5
              Top = 72
              Width = 30
              Height = 12
              Caption = #36305#27493':'
            end
            object Label10: TLabel
              Left = 5
              Top = 96
              Width = 30
              Height = 12
              Caption = #36208#36335':'
            end
            object Label11: TLabel
              Left = 5
              Top = 144
              Width = 30
              Height = 12
              Caption = #25366#32905':'
            end
            object Label12: TLabel
              Left = 5
              Top = 120
              Width = 30
              Height = 12
              Caption = #36716#21521':'
            end
            object EditMaxHitMsgCount: TSpinEditEx
              Left = 36
              Top = 20
              Width = 29
              Height = 21
              Hint = #20801#35768#21516#26102#25805#20316#25968#37327#65292#27492#21442#25968#40664#35748#20026'1('#21152#22823#27492#25968#23383#65292#23558#20986#29616#21452#20493#21450#22810#20493#25915#20987')'
              MaxValue = 50
              MinValue = 1
              TabOrder = 0
              Value = 2
              OnChange = EditMaxHitMsgCountChange
            end
            object EditMaxSpellMsgCount: TSpinEditEx
              Left = 36
              Top = 44
              Width = 29
              Height = 21
              Hint = #20801#35768#21516#26102#25805#20316#25968#37327#65292#27492#21442#25968#40664#35748#20026'1('#21152#22823#27492#25968#23383#65292#23558#20986#29616#21452#20493#21450#22810#20493#25915#20987')'
              MaxValue = 50
              MinValue = 1
              TabOrder = 1
              Value = 2
              OnChange = EditMaxSpellMsgCountChange
            end
            object EditMaxRunMsgCount: TSpinEditEx
              Left = 36
              Top = 68
              Width = 29
              Height = 21
              Hint = #20801#35768#21516#26102#25805#20316#25968#37327#65292#27492#21442#25968#40664#35748#20026'1('#21152#22823#27492#25968#23383#65292#23558#20986#29616#21452#20493#21450#22810#20493#25915#20987')'
              MaxValue = 50
              MinValue = 1
              TabOrder = 2
              Value = 2
              OnChange = EditMaxRunMsgCountChange
            end
            object EditMaxWalkMsgCount: TSpinEditEx
              Left = 36
              Top = 92
              Width = 29
              Height = 21
              Hint = #20801#35768#21516#26102#25805#20316#25968#37327#65292#27492#21442#25968#40664#35748#20026'1('#21152#22823#27492#25968#23383#65292#23558#20986#29616#21452#20493#21450#22810#20493#25915#20987')'
              MaxValue = 50
              MinValue = 1
              TabOrder = 3
              Value = 2
              OnChange = EditMaxWalkMsgCountChange
            end
            object EditMaxTurnMsgCount: TSpinEditEx
              Left = 36
              Top = 116
              Width = 29
              Height = 21
              Hint = #20801#35768#21516#26102#25805#20316#25968#37327#65292#27492#21442#25968#40664#35748#20026'1('#21152#22823#27492#25968#23383#65292#23558#20986#29616#21452#20493#21450#22810#20493#25915#20987')'
              MaxValue = 50
              MinValue = 1
              TabOrder = 4
              Value = 2
              OnChange = EditMaxTurnMsgCountChange
            end
            object EditMaxDigUpMsgCount: TSpinEditEx
              Left = 36
              Top = 140
              Width = 29
              Height = 21
              Hint = #20801#35768#21516#26102#25805#20316#25968#37327#65292#27492#21442#25968#40664#35748#20026'1('#21152#22823#27492#25968#23383#65292#23558#20986#29616#21452#20493#21450#22810#20493#25915#20987')'
              MaxValue = 50
              MinValue = 1
              TabOrder = 5
              Value = 2
              OnChange = EditMaxDigUpMsgCountChange
            end
          end
          object GroupBox15: TGroupBox
            Left = 183
            Top = 22
            Width = 149
            Height = 90
            Caption = #25805#20316#25968#25454#25511#21046'(1)'
            TabOrder = 2
            object Label38: TLabel
              Left = 5
              Top = 68
              Width = 30
              Height = 12
              Caption = #27425#25968':'
            end
            object Label142: TLabel
              Left = 67
              Top = 68
              Width = 30
              Height = 12
              Caption = #36807#28388':'
            end
            object CheckBoxboKickOverSpeed: TCheckBox
              Left = 5
              Top = 47
              Width = 121
              Height = 17
              Hint = #23558#36229#36895#25805#20316#30340#20154#29289#36386#19979#32447#12290
              Caption = #25481#32447#22788#29702#36229#36895#25805#20316
              TabOrder = 2
              OnClick = CheckBoxboKickOverSpeedClick
            end
            object EditDropOverSpeed: TSpinEditEx
              Left = 96
              Top = 64
              Width = 46
              Height = 21
              Hint = #36807#28388#36229#36895#25805#20316#25968#25454#65292#25968#23383#36234#23567#36234#20005#65292#36807#28388#21518#23458#25143#31471#20250#20986#29616#21345#20992#25110#21453#24377#29616#35937#12290'('#27627#31186')'
              Increment = 10
              MaxValue = 1000
              MinValue = 1
              TabOrder = 3
              Value = 50
              OnChange = EditDropOverSpeedChange
            end
            object CheckBoxSpellSendUpdateMsg: TCheckBox
              Left = 5
              Top = 15
              Width = 129
              Height = 17
              Hint = #25511#21046#20154#29289#21516#26102#30456#21516#39764#27861#25805#20316#25968#25454#65292#21516#26102#21482#33021#26377#19968#20010#39764#27861#25915#20987#25805#20316
              Caption = #39764#27861#25805#20316#25968#25454#37327#25511#21046
              TabOrder = 0
              OnClick = CheckBoxSpellSendUpdateMsgClick
            end
            object CheckBoxActionSendActionMsg: TCheckBox
              Left = 5
              Top = 31
              Width = 129
              Height = 17
              Hint = #25511#21046#20154#29289#21516#26102#30456#21516#25915#20987#25805#20316#25968#25454#65292#21516#26102#21482#33021#26377#19968#20010#39764#27861#25915#20987#25805#20316
              Caption = #25915#20987#25805#20316#25968#25454#37327#25511#21046
              TabOrder = 1
              OnClick = CheckBoxActionSendActionMsgClick
            end
            object EditOverSpeedKickCount: TSpinEditEx
              Left = 36
              Top = 64
              Width = 29
              Height = 21
              Hint = #36229#36895#27425#25968#65292#36229#25351#23450#27425#25968#21017#34987#36386#19979#32447#12290
              MaxValue = 50
              MinValue = 1
              TabOrder = 4
              Value = 4
              OnChange = EditOverSpeedKickCountChange
            end
          end
          object GroupBox7: TGroupBox
            Left = 183
            Top = 115
            Width = 149
            Height = 91
            Caption = #20154#29289#24367#33136#25511#21046
            TabOrder = 4
            object Label22: TLabel
              Left = 11
              Top = 19
              Width = 54
              Height = 12
              Caption = #20572#30041#26102#38388':'
            end
            object EditStruckTime: TSpinEditEx
              Left = 68
              Top = 15
              Width = 53
              Height = 21
              MaxValue = 1000
              MinValue = 10
              TabOrder = 0
              Value = 100
              OnChange = EditStruckTimeChange
            end
            object CheckBoxDisableStruck: TCheckBox
              Left = 11
              Top = 35
              Width = 105
              Height = 17
              Caption = #20154#29289#26080#24367#33136#21160#20316
              TabOrder = 1
              OnClick = CheckBoxDisableStruckClick
            end
            object CheckBoxDisableSelfStruck: TCheckBox
              Left = 11
              Top = 51
              Width = 105
              Height = 17
              Caption = #20154#29289#33258#24049#19981#24367#33136
              TabOrder = 2
              OnClick = CheckBoxDisableSelfStruckClick
            end
            object chkMagicshieldStruck: TCheckBox
              Left = 11
              Top = 69
              Width = 97
              Height = 17
              Hint = #21246#36873#21518#65292#24403#30446#26631#25140#20102#25252#36523#35013#22791#21482#25481'MP'#26102#65292#19968#26679#26377#21518#20208#21160#20316
              Caption = #26080#35270#25252#36523
              TabOrder = 3
              OnClick = chkMagicshieldStruckClick
            end
          end
          object ButtonGameSpeedDefault: TButton
            Left = 373
            Top = 238
            Width = 65
            Height = 25
            Caption = #40664#35748'(&D)'
            TabOrder = 8
            OnClick = ButtonGameSpeedDefaultClick
          end
          object ButtonGameSpeedSave: TButton
            Left = 448
            Top = 238
            Width = 65
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 9
            OnClick = ButtonGameSpeedSaveClick
          end
          object ButtonActionSpeedConfig: TButton
            Left = 408
            Top = 210
            Width = 105
            Height = 25
            Caption = #32452#21512#36895#24230#35774#32622'(&A)'
            TabOrder = 5
            OnClick = ButtonActionSpeedConfigClick
          end
          object GroupBox4: TGroupBox
            Left = 8
            Top = 211
            Width = 89
            Height = 52
            Caption = #36895#24230#25511#21046#27169#24335
            TabOrder = 6
            object RadioButtonDelyMode: TRadioButton
              Left = 4
              Top = 16
              Width = 73
              Height = 17
              Hint = #23558#36229#36807#36895#24230#30340#25805#20316#36827#34892#24310#26102#22788#29702#65292#20197#20445#25345#27491#24120#36895#24230#65292#20351#29992#27492#31181#27169#24335#23458#25143#31471#20351#29992#21152#36895#23558#36896#25104#21345#30340#29616#35937#12290
              Caption = #20572#39039#25805#20316
              TabOrder = 0
              OnClick = RadioButtonDelyModeClick
            end
            object RadioButtonFilterMode: TRadioButton
              Left = 4
              Top = 32
              Width = 73
              Height = 17
              Hint = #23558#36229#36807#36895#24230#30340#25805#20316#30452#25509#36807#28388#22788#29702#65292#20002#24323#36229#36895#24230#30340#25805#20316#65292#20351#29992#27492#31181#27169#24335#23458#25143#31471#20351#29992#21152#36895#23558#36896#25104#21345#20992#65292#21453#24377#30340#29616#35937#12290
              Caption = #21453#24377#21345#20992
              TabOrder = 1
              OnClick = RadioButtonFilterModeClick
            end
          end
          object GroupBox83: TGroupBox
            Left = 338
            Top = 22
            Width = 178
            Height = 183
            Caption = #25805#20316#25968#25454#25511#21046'(2)'
            TabOrder = 3
            object Label186: TLabel
              Left = 8
              Top = 40
              Width = 90
              Height = 12
              Caption = #25915#20987#24310#26102#26368#22823#20540':'
            end
            object Label187: TLabel
              Left = 149
              Top = 40
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label188: TLabel
              Left = 8
              Top = 64
              Width = 90
              Height = 12
              Caption = #39764#27861#24310#26102#26368#22823#20540':'
            end
            object Label189: TLabel
              Left = 149
              Top = 64
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label190: TLabel
              Left = 8
              Top = 88
              Width = 90
              Height = 12
              Caption = #36305#27493#24310#26102#26368#22823#20540':'
            end
            object Label191: TLabel
              Left = 149
              Top = 88
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label192: TLabel
              Left = 8
              Top = 112
              Width = 90
              Height = 12
              Caption = #36208#36335#24310#26102#26368#22823#20540':'
            end
            object Label193: TLabel
              Left = 149
              Top = 112
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label194: TLabel
              Left = 8
              Top = 136
              Width = 90
              Height = 12
              Caption = #36716#21521#24310#26102#26368#22823#20540':'
            end
            object Label195: TLabel
              Left = 149
              Top = 136
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label196: TLabel
              Left = 8
              Top = 160
              Width = 90
              Height = 12
              Caption = #25366#32905#24310#26102#26368#22823#20540':'
            end
            object Label197: TLabel
              Left = 149
              Top = 160
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object CheckBoxSendUpdateMsg: TCheckBox
              Left = 8
              Top = 16
              Width = 105
              Height = 17
              Hint = 
                #19982#25805#20316#25968#25454#25511#21046'(1)'#37324#38754#30340#25968#25454#37327#25511#21046#30340#21306#21035#65306#13'1.'#20154#29289#31227#21160#12289#39764#27861#25805#20316#12289#25915#20987#25805#20316#37117#21463#25511#21046#13'2.'#38450#27490#21152#36895#22806#25346#31561#21407#22240#65292#36896#25104#38388#38548#25511#21046#22833#25928 +
                #30340#38382#39064
              Caption = #25805#20316#25968#25454#37327#25511#21046
              TabOrder = 0
              OnClick = CheckBoxSendUpdateMsgClick
            end
            object EditMaxHitDeliveryTime: TSpinEditEx
              Left = 96
              Top = 36
              Width = 53
              Height = 21
              Hint = 
                #20154#29289#25915#20987#39057#29575#36229#36807#38388#38548#25511#21046#35774#32622#20540#26102#65292#24341#25806#20869#37096#20250#23545#35813#21160#20316#20570#24310#26102#22788#29702#13#35813#20540#23601#26159#38480#21046#27492#24310#26102#30340#26368#22823#20540#65292#38450#27490#24310#26102#36807#22823#32780#36896#25104#19981#27969#30021#30340#24863#35273#12290#25968#20540 +
                #36234#22823#36234#20005#26684#12290#13#35813#20540#22914#26524#22826#23567#65292#38388#38548#25511#21046#20250#22833#25928#12290
              Increment = 10
              MaxValue = 3000
              MinValue = 0
              TabOrder = 1
              Value = 400
              OnChange = EditMaxHitDeliveryTimeChange
            end
            object EditMaxMagicHitDeliveryTime: TSpinEditEx
              Left = 96
              Top = 60
              Width = 53
              Height = 21
              Hint = 
                #20154#29289#39764#27861#25915#20987#39057#29575#36229#36807#38388#38548#25511#21046#35774#32622#20540#26102#65292#24341#25806#20869#37096#20250#23545#35813#21160#20316#20570#24310#26102#22788#29702#13#35813#20540#23601#26159#38480#21046#27492#24310#26102#30340#26368#22823#20540#65292#38450#27490#24310#26102#36807#22823#32780#36896#25104#19981#27969#30021#30340#24863#35273#12290 +
                #25968#20540#36234#22823#36234#20005#26684#12290#13#35813#20540#22914#26524#22826#23567#65292#38388#38548#25511#21046#20250#22833#25928#12290
              Increment = 10
              MaxValue = 3000
              MinValue = 0
              TabOrder = 2
              Value = 400
              OnChange = EditMaxMagicHitDeliveryTimeChange
            end
            object EditMaxRunDeliveryTime: TSpinEditEx
              Left = 96
              Top = 84
              Width = 53
              Height = 21
              Hint = 
                #20154#29289#36305#27493#39057#29575#36229#36807#38388#38548#25511#21046#35774#32622#20540#26102#65292#24341#25806#20869#37096#20250#23545#35813#21160#20316#20570#24310#26102#22788#29702#13#35813#20540#23601#26159#38480#21046#27492#24310#26102#30340#26368#22823#20540#65292#38450#27490#24310#26102#36807#22823#32780#36896#25104#19981#27969#30021#30340#24863#35273#12290#25968#20540 +
                #36234#22823#36234#20005#26684#12290#13#35813#20540#22914#26524#22826#23567#65292#38388#38548#25511#21046#20250#22833#25928#12290
              Increment = 10
              MaxValue = 3000
              MinValue = 0
              TabOrder = 3
              Value = 400
              OnChange = EditMaxRunDeliveryTimeChange
            end
            object EditMaxWalkDeliveryTime: TSpinEditEx
              Left = 96
              Top = 108
              Width = 53
              Height = 21
              Hint = 
                #20154#29289#36208#36335#39057#29575#36229#36807#38388#38548#25511#21046#35774#32622#20540#26102#65292#24341#25806#20869#37096#20250#23545#35813#21160#20316#20570#24310#26102#22788#29702#13#35813#20540#23601#26159#38480#21046#27492#24310#26102#30340#26368#22823#20540#65292#38450#27490#24310#26102#36807#22823#32780#36896#25104#19981#27969#30021#30340#24863#35273#12290#25968#20540 +
                #36234#22823#36234#20005#26684#12290#13#35813#20540#22914#26524#22826#23567#65292#38388#38548#25511#21046#20250#22833#25928#12290
              Increment = 10
              MaxValue = 3000
              MinValue = 0
              TabOrder = 4
              Value = 400
              OnChange = EditMaxWalkDeliveryTimeChange
            end
            object EditMaxTurnDeliveryTime: TSpinEditEx
              Left = 96
              Top = 132
              Width = 53
              Height = 21
              Hint = 
                #20154#29289#36716#21521#25805#20316#39057#29575#36229#36807#38388#38548#25511#21046#35774#32622#20540#26102#65292#24341#25806#20869#37096#20250#23545#35813#21160#20316#20570#24310#26102#22788#29702#13#35813#20540#23601#26159#38480#21046#27492#24310#26102#30340#26368#22823#20540#65292#38450#27490#24310#26102#36807#22823#32780#36896#25104#19981#27969#30021#30340#24863#35273#12290 +
                #25968#20540#36234#22823#36234#20005#26684#12290#13#35813#20540#22914#26524#22826#23567#65292#38388#38548#25511#21046#20250#22833#25928#12290
              Increment = 10
              MaxValue = 3000
              MinValue = 0
              TabOrder = 5
              Value = 400
              OnChange = EditMaxTurnDeliveryTimeChange
            end
            object EditMaxDigUpDeliveryTime: TSpinEditEx
              Left = 96
              Top = 156
              Width = 53
              Height = 21
              Hint = 
                #20154#29289#25366#32905#25805#20316#39057#29575#36229#36807#38388#38548#25511#21046#35774#32622#20540#26102#65292#24341#25806#20869#37096#20250#23545#35813#21160#20316#20570#24310#26102#22788#29702#13#35813#20540#23601#26159#38480#21046#27492#24310#26102#30340#26368#22823#20540#65292#38450#27490#24310#26102#36807#22823#32780#36896#25104#19981#27969#30021#30340#24863#35273#12290 +
                #25968#20540#36234#22823#36234#20005#26684#12290#13#35813#20540#22914#26524#22826#23567#65292#38388#38548#25511#21046#20250#22833#25928#12290
              Increment = 10
              MaxValue = 3000
              MinValue = 0
              TabOrder = 6
              Value = 400
              OnChange = EditMaxDigUpDeliveryTimeChange
            end
          end
          object grp5: TGroupBox
            Left = 104
            Top = 211
            Width = 105
            Height = 52
            Caption = #39569#39532#36895#24230
            TabOrder = 7
            object chkHorseRun3Grid: TCheckBox
              Left = 7
              Top = 24
              Width = 93
              Height = 17
              Hint = #27492#21151#33021#26242#19981#25903#25345'!!!!!!!!!!!!!!'#13#10#13#10#21246#36873#21518#65292#39569#39532#36305#27493#26102#23558#24320#21551#19968#27493#19977#26684#65292#21542#21017#20026#19968#27493#20004#26684
              Caption = #19968#27493#19977#26684
              TabOrder = 0
              OnClick = chkHorseRun3GridClick
            end
          end
          object chkSpeedControl: TCheckBox
            Left = 9
            Top = 1
            Width = 86
            Height = 17
            Caption = #38480#21046#36229#36895
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
            TabOrder = 10
            OnClick = chkSpeedControlClick
          end
        end
        object TabSheet12: TTabSheet
          Caption = #22806#25346
          ImageIndex = 1
          object Label159: TLabel
            Left = 8
            Top = 136
            Width = 282
            Height = 12
            Caption = #26816#27979#21040#36229#36895#26102#35302#21457' QFunction.txt [@UsePlugin]'#23383#27573
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox78: TGroupBox
            Left = 8
            Top = 5
            Width = 457
            Height = 121
            Caption = #21160#20316#21152#36895#26816#27979
            ParentShowHint = False
            ShowHint = False
            TabOrder = 0
            object Label153: TLabel
              Left = 64
              Top = 44
              Width = 84
              Height = 12
              Caption = #27627#31186#20869#20801#35768#25915#20987
            end
            object Label154: TLabel
              Left = 192
              Top = 44
              Width = 78
              Height = 12
              Caption = #27425#12290' '#36830#32493#36229#36895
            end
            object Label155: TLabel
              Left = 64
              Top = 68
              Width = 84
              Height = 12
              Caption = #27627#31186#20869#20801#35768#39764#27861
            end
            object Label157: TLabel
              Left = 64
              Top = 92
              Width = 84
              Height = 12
              Caption = #27627#31186#20869#20801#35768#31227#21160
            end
            object Label160: TLabel
              Left = 320
              Top = 44
              Width = 60
              Height = 12
              Caption = #27425#35302#21457#33050#26412
            end
            object Label156: TLabel
              Left = 192
              Top = 68
              Width = 78
              Height = 12
              Caption = #27425#12290' '#36830#32493#36229#36895
            end
            object Label158: TLabel
              Left = 320
              Top = 68
              Width = 60
              Height = 12
              Caption = #27425#35302#21457#33050#26412
            end
            object Label161: TLabel
              Left = 192
              Top = 92
              Width = 78
              Height = 12
              Caption = #27425#12290' '#36830#32493#36229#36895
            end
            object Label162: TLabel
              Left = 320
              Top = 92
              Width = 60
              Height = 12
              Caption = #27425#35302#21457#33050#26412
            end
            object CheckBoxCheckActionCount: TCheckBox
              Left = 8
              Top = 16
              Width = 49
              Height = 17
              Caption = #21551#29992
              TabOrder = 0
              OnClick = CheckBoxCheckActionCountClick
            end
            object EditHitCountIntervalTime: TSpinEditEx
              Left = 8
              Top = 40
              Width = 49
              Height = 21
              Hint = #35813#25968#20540#36234#22823#36234#20005#26684
              Increment = 10
              MaxValue = 10000
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              Value = 1000
              OnChange = EditHitCountIntervalTimeChange
            end
            object EditCanHitCount: TSpinEditEx
              Left = 152
              Top = 40
              Width = 33
              Height = 21
              Hint = #35813#25968#20540#36234#23567#36234#20005#26684
              MaxValue = 100
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 2
              OnChange = EditCanHitCountChange
            end
            object EditMagicHitCountIntervalTime: TSpinEditEx
              Left = 8
              Top = 64
              Width = 49
              Height = 21
              Hint = #35813#25968#20540#36234#22823#36234#20005#26684
              Increment = 10
              MaxValue = 10000
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              Value = 1000
              OnChange = EditMagicHitCountIntervalTimeChange
            end
            object EditCanMagicHitCount: TSpinEditEx
              Left = 152
              Top = 64
              Width = 33
              Height = 21
              Hint = #35813#25968#20540#36234#23567#36234#20005#26684
              MaxValue = 100
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 5
              Value = 2
              OnChange = EditCanMagicHitCountChange
            end
            object EditMoveCountIntervalTime: TSpinEditEx
              Left = 8
              Top = 88
              Width = 49
              Height = 21
              Hint = #35813#25968#20540#36234#22823#36234#20005#26684
              MaxValue = 10000
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 7
              Value = 1000
              OnChange = EditMoveCountIntervalTimeChange
            end
            object EditCanMoveCount: TSpinEditEx
              Left = 152
              Top = 88
              Width = 33
              Height = 21
              Hint = #35813#25968#20540#36234#23567#36234#20005#26684
              MaxValue = 100
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 8
              Value = 2
              OnChange = EditCanMoveCountChange
            end
            object EditCheckHitCount: TSpinEditEx
              Left = 280
              Top = 40
              Width = 33
              Height = 21
              Hint = #35813#25968#20540#36234#22823#35823#23553#36234#20302#65292#22914#26524#25968#20540#22826#22823#20102#23601#36215#19981#21040#23553#21152#36895#30340#25928#26524#20102
              MaxValue = 100
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 2
              OnChange = EditCheckHitCountChange
            end
            object EditCheckMagicHitCount: TSpinEditEx
              Left = 280
              Top = 64
              Width = 33
              Height = 21
              Hint = #35813#25968#20540#36234#22823#35823#23553#36234#20302#65292#22914#26524#25968#20540#22826#22823#20102#23601#36215#19981#21040#23553#21152#36895#30340#25928#26524#20102
              MaxValue = 100
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 6
              Value = 2
              OnChange = EditCheckMagicHitCountChange
            end
            object EditCheckMoveCount: TSpinEditEx
              Left = 280
              Top = 88
              Width = 33
              Height = 21
              Hint = #35813#25968#20540#36234#22823#35823#23553#36234#20302#65292#22914#26524#25968#20540#22826#22823#20102#23601#36215#19981#21040#23553#21152#36895#30340#25928#26524#20102
              MaxValue = 100
              MinValue = 1
              ParentShowHint = False
              ShowHint = True
              TabOrder = 9
              Value = 2
              OnChange = EditCheckMoveCountChange
            end
          end
          object ButtonCheckActionSave: TButton
            Left = 400
            Top = 172
            Width = 65
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 2
            OnClick = ButtonCheckActionSaveClick
          end
          object ButtonCheckActionDefault: TButton
            Left = 333
            Top = 172
            Width = 65
            Height = 25
            Caption = #40664#35748'(&D)'
            TabOrder = 1
            OnClick = ButtonCheckActionDefaultClick
          end
        end
      end
    end
    object TabSheet10: TTabSheet
      Caption = #29366#24577#25511#21046
      ImageIndex = 13
      object ButtonCharStatusSave: TButton
        Left = 456
        Top = 261
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 2
        OnClick = ButtonCharStatusSaveClick
      end
      object GroupBoxParaly: TGroupBox
        Left = 8
        Top = 8
        Width = 129
        Height = 89
        Caption = #40635#30201#25511#21046
        TabOrder = 0
        object CheckBoxParalyCanRun: TCheckBox
          Left = 8
          Top = 16
          Width = 73
          Height = 17
          Hint = #20154#29289#34987#40635#30201#21518#26159#21542#20801#35768#36305#21160#65292#38057#19978#20026#20801#35768#36305#21160
          Caption = #20801#35768#36305#21160
          TabOrder = 0
          OnClick = CheckBoxParalyCanRunClick
        end
        object CheckBoxParalyCanWalk: TCheckBox
          Left = 8
          Top = 32
          Width = 73
          Height = 17
          Hint = #20154#29289#34987#40635#30201#21518#26159#21542#20801#35768#36305#21160#65292#38057#19978#20026#20801#35768#36208#21160
          Caption = #20801#35768#36208#21160
          TabOrder = 1
          OnClick = CheckBoxParalyCanWalkClick
        end
        object CheckBoxParalyCanHit: TCheckBox
          Left = 8
          Top = 48
          Width = 73
          Height = 17
          Hint = #20154#29289#34987#40635#30201#21518#26159#21542#20801#35768#36305#21160#65292#38057#19978#20026#20801#35768#25915#20987
          Caption = #20801#35768#25915#20987
          TabOrder = 2
          OnClick = CheckBoxParalyCanHitClick
        end
        object CheckBoxParalyCanSpell: TCheckBox
          Left = 8
          Top = 64
          Width = 73
          Height = 17
          Hint = #20154#29289#34987#40635#30201#21518#26159#21542#20801#35768#36305#21160#65292#38057#19978#20026#20801#35768#39764#27861
          Caption = #20801#35768#39764#27861
          TabOrder = 3
          OnClick = CheckBoxParalyCanSpellClick
        end
      end
      object CheckGroupAttatckMode: TRzCheckGroup
        Left = 376
        Top = 8
        Width = 137
        Height = 171
        Caption = #25915#20987#27169#24335#25511#21046
        Color = 15987699
        GroupStyle = gsStandard
        ItemHeight = 16
        Items.Strings = (
          #20801#35768#20840#20307#25915#20987#27169#24335
          #20801#35768#21644#24179#25915#20987#27169#24335
          #20801#35768#22827#22971#25915#20987#27169#24335
          #20801#35768#24072#24466#25915#20987#27169#24335
          #20801#35768#32534#32452#25915#20987#27169#24335
          #20801#35768#34892#20250#25915#20987#27169#24335
          #20801#35768#21892#24694#25915#20987#27169#24335
          #20801#35768#22269#23478#25915#20987#27169#24335)
        TabOrder = 1
        OnChange = CheckGroupAttatckModeChange
        CheckStates = (
          0
          0
          0
          0
          0
          0
          0
          0)
      end
    end
    object ExpSheet: TTabSheet
      Caption = #21319#32423#32463#39564
      ImageIndex = 1
      object GroupBox8: TGroupBox
        Left = 183
        Top = 8
        Width = 171
        Height = 89
        Caption = #26432#24618#32463#39564
        TabOrder = 1
        object Label23: TLabel
          Left = 11
          Top = 24
          Width = 30
          Height = 12
          Caption = #20493#29575':'
        end
        object EditKillMonExpMultiple: TSpinEditEx
          Left = 44
          Top = 20
          Width = 53
          Height = 21
          MaxValue = 2000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditKillMonExpMultipleChange
        end
        object CheckBoxHighLevelKillMonFixExp: TCheckBox
          Left = 11
          Top = 45
          Width = 134
          Height = 17
          Caption = #39640#31561#32423#26432#24618#32463#39564#19981#21464
          TabOrder = 1
          OnClick = CheckBoxHighLevelKillMonFixExpClick
        end
        object CheckBoxHighLevelGroupFixExp: TCheckBox
          Left = 11
          Top = 64
          Width = 134
          Height = 17
          Caption = #39640#31561#32423#32452#38431#32463#39564#19981#21464
          TabOrder = 2
          OnClick = CheckBoxHighLevelGroupFixExpClick
        end
      end
      object ButtonExpSave: TButton
        Left = 288
        Top = 259
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 7
        OnClick = ButtonExpSaveClick
      end
      object GroupBoxLevelExp: TGroupBox
        Left = 8
        Top = 8
        Width = 169
        Height = 189
        Caption = #21319#32423#32463#39564
        TabOrder = 0
        object Label37: TLabel
          Left = 11
          Top = 165
          Width = 30
          Height = 12
          Caption = #35745#21010':'
        end
        object ComboBoxLevelExp: TComboBox
          Left = 48
          Top = 160
          Width = 113
          Height = 20
          Style = csDropDownList
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          OnClick = ComboBoxLevelExpClick
        end
        object GridLevelExp: TStringGrid
          Left = 8
          Top = 16
          Width = 153
          Height = 137
          ColCount = 2
          DefaultRowHeight = 18
          RowCount = 1001
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
          TabOrder = 0
          OnSetEditText = GridLevelExpSetEditText
          ColWidths = (
            64
            67)
          RowHeights = (
            18
            18
            19
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
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
      object GroupBox74: TGroupBox
        Left = 360
        Top = 8
        Width = 161
        Height = 89
        Caption = '1000'#32423#20197#21518#32463#39564#37197#21046
        TabOrder = 2
        object Label15: TLabel
          Left = 8
          Top = 40
          Width = 54
          Height = 12
          Caption = #22522#26412#32463#39564':'
        end
        object Label145: TLabel
          Left = 8
          Top = 64
          Width = 54
          Height = 12
          Caption = #22686#21152#32463#39564':'
        end
        object CheckBoxFixExp: TCheckBox
          Left = 8
          Top = 16
          Width = 145
          Height = 17
          Caption = #20351#29992#24341#25806#20869#37096#22266#23450#32463#39564
          TabOrder = 0
          OnClick = CheckBoxFixExpClick
        end
        object SpinEditBaseExp: TSpinEditEx
          Left = 64
          Top = 36
          Width = 89
          Height = 21
          Hint = #20154#29289#22522#26412#32463#39564
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 1
          Value = 100000000
          OnChange = SpinEditBaseExpChange
        end
        object SpinEditAddExp: TSpinEditEx
          Left = 64
          Top = 62
          Width = 89
          Height = 21
          Hint = #27599#27425#21319#32423#22686#21152#30340#32463#39564
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 2
          Value = 1000000
          OnChange = SpinEditAddExpChange
        end
      end
      object GroupBox77: TGroupBox
        Left = 183
        Top = 103
        Width = 171
        Height = 93
        Caption = #26368#39640#31561#32423#38480#21046
        TabOrder = 3
        object Label149: TLabel
          Left = 8
          Top = 24
          Width = 54
          Height = 12
          Caption = #26368#39640#31561#32423':'
        end
        object Label150: TLabel
          Left = 8
          Top = 48
          Width = 54
          Height = 12
          Caption = #26432#24618#32463#39564':'
        end
        object EditHighLevel: TSpinEditEx
          Left = 64
          Top = 20
          Width = 89
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 0
          Value = 100000000
          OnChange = EditHighLevelChange
        end
        object EditHighLevelGetExp: TSpinEditEx
          Left = 64
          Top = 44
          Width = 89
          Height = 21
          MaxValue = 2100000000
          MinValue = 0
          TabOrder = 1
          Value = 1000000
          OnChange = EditHighLevelGetExpChange
        end
        object CheckBoxLimitChangeExp: TCheckBox
          Left = 8
          Top = 69
          Width = 97
          Height = 17
          Hint = #20154#29289#31561#32423#36798#21040#26368#39640#31561#32423#38480#21046#26102#20351#29992'ChangeExp'#22686#21152#32463#39564#25353#29031#26432#24618#32463#39564#22686#21152
          Caption = #38480#21046'ChangeExp'
          TabOrder = 2
          OnClick = CheckBoxLimitChangeExpClick
        end
      end
      object GroupBox87: TGroupBox
        Left = 360
        Top = 104
        Width = 169
        Height = 185
        Caption = #31561#32423#32463#39564#25511#21046
        TabOrder = 4
        object GridLevelExpRate: TStringGrid
          Left = 8
          Top = 16
          Width = 153
          Height = 161
          ColCount = 2
          DefaultColWidth = 56
          DefaultRowHeight = 18
          RowCount = 1001
          Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
          TabOrder = 0
          OnSetEditText = GridLevelExpSetEditText
          ColWidths = (
            56
            56)
          RowHeights = (
            18
            18
            19
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
            18
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
      object GroupBox88: TGroupBox
        Left = 183
        Top = 201
        Width = 171
        Height = 51
        Caption = #32463#39564#36798#21040#21518#21487#20197#36830#32493#21319#32423#25511#21046
        TabOrder = 6
        object Label209: TLabel
          Left = 8
          Top = 24
          Width = 66
          Height = 12
          Caption = #21487#20197#36830#32493#21319':'
        end
        object Label210: TLabel
          Left = 136
          Top = 24
          Width = 12
          Height = 12
          Caption = #32423
        end
        object EditMaxUpLevelCount: TSpinEditEx
          Left = 80
          Top = 20
          Width = 49
          Height = 21
          MaxValue = 10
          MinValue = 1
          TabOrder = 0
          Value = 1
          OnChange = EditMaxUpLevelCountChange
        end
      end
      object GroupBox26: TGroupBox
        Left = 8
        Top = 200
        Width = 161
        Height = 81
        Caption = #32463#39564#20998#37197
        TabOrder = 5
        object chkShareExpGroupSameScreen: TCheckBox
          Left = 11
          Top = 17
          Width = 142
          Height = 18
          Hint = #24320#21551#21518#65292#32452#38431#20154#29289#19981#22312#21516#19968#23631#24149#20869#65292#20173#28982#21487#20197#20998#21040#32463#39564#12290
          Caption = #32452#38431#19981#38656#35201#21516#19968#23631#24149#20869
          TabOrder = 0
          OnClick = chkShareExpGroupSameScreenClick
        end
        object chkShareExpGroupSameMap: TCheckBox
          Left = 34
          Top = 37
          Width = 119
          Height = 18
          Hint = #24320#21551#38750#21516#23631#24149#20998#32463#39564#21518#65292#26159#21542#38656#35201#22312#21516#19968#22320#22270#65292#24320#21551#21518#65292#20219#20309#22320#28857#32452#38431#37117#21487#20197#20998#32463#39564#12290
          Caption = #19981#38656#35201#22312#21516#19968#22320#22270
          TabOrder = 1
          OnClick = chkShareExpGroupSameMapClick
        end
        object chkShareExpHeroSameMap: TCheckBox
          Left = 10
          Top = 57
          Width = 143
          Height = 18
          Caption = #33521#38596#19981#38656#35201#22312#21516#19968#22320#22270
          TabOrder = 2
          OnClick = chkShareExpHeroSameMapClick
        end
      end
    end
    object CastleSheet: TTabSheet
      Caption = #22478#22561#21442#25968
      ImageIndex = 3
      object GroupBox9: TGroupBox
        Left = 8
        Top = 8
        Width = 161
        Height = 113
        Caption = #36153#29992#25910#20837
        TabOrder = 0
        object Label24: TLabel
          Left = 11
          Top = 16
          Width = 54
          Height = 12
          Caption = #32500#20462#22478#38376':'
        end
        object Label25: TLabel
          Left = 11
          Top = 40
          Width = 54
          Height = 12
          Caption = #32500#20462#22478#22681':'
        end
        object Label26: TLabel
          Left = 11
          Top = 64
          Width = 54
          Height = 12
          Caption = #38599#29992#24339#31661':'
        end
        object Label27: TLabel
          Left = 11
          Top = 88
          Width = 54
          Height = 12
          Caption = #38599#29992#21355#22763':'
        end
        object EditRepairDoorPrice: TSpinEditEx
          Left = 72
          Top = 12
          Width = 81
          Height = 21
          Increment = 10000
          MaxValue = 100000000
          MinValue = 10000
          TabOrder = 0
          Value = 2000000
          OnChange = EditRepairDoorPriceChange
        end
        object EditRepairWallPrice: TSpinEditEx
          Left = 72
          Top = 36
          Width = 81
          Height = 21
          Increment = 10000
          MaxValue = 100000000
          MinValue = 10000
          TabOrder = 1
          Value = 500000
          OnChange = EditRepairWallPriceChange
        end
        object EditHireArcherPrice: TSpinEditEx
          Left = 72
          Top = 60
          Width = 81
          Height = 21
          Increment = 10000
          MaxValue = 100000000
          MinValue = 10000
          TabOrder = 2
          Value = 300000
          OnChange = EditHireArcherPriceChange
        end
        object EditHireGuardPrice: TSpinEditEx
          Left = 72
          Top = 84
          Width = 81
          Height = 21
          Increment = 10000
          MaxValue = 100000000
          MinValue = 10000
          TabOrder = 3
          Value = 300000
          OnChange = EditHireGuardPriceChange
        end
      end
      object GroupBox10: TGroupBox
        Left = 8
        Top = 125
        Width = 161
        Height = 68
        Caption = #37329#24065#19978#38480
        TabOrder = 5
        object Label31: TLabel
          Left = 11
          Top = 16
          Width = 54
          Height = 12
          Caption = #22478#20869#36164#37329':'
        end
        object Label32: TLabel
          Left = 11
          Top = 40
          Width = 54
          Height = 12
          Caption = #19968#22825#25910#20837':'
        end
        object EditCastleGoldMax: TSpinEditEx
          Left = 72
          Top = 12
          Width = 81
          Height = 21
          Increment = 10000
          MaxValue = 100000000
          MinValue = 10000
          TabOrder = 0
          Value = 10000000
          OnChange = EditCastleGoldMaxChange
        end
        object EditCastleOneDayGold: TSpinEditEx
          Left = 72
          Top = 36
          Width = 81
          Height = 21
          Increment = 10000
          MaxValue = 100000000
          MinValue = 10000
          TabOrder = 1
          Value = 2000000
          OnChange = EditCastleOneDayGoldChange
        end
      end
      object GroupBox11: TGroupBox
        Left = 296
        Top = 58
        Width = 121
        Height = 87
        Caption = #22238#22478#28857
        TabOrder = 3
        object Label28: TLabel
          Left = 11
          Top = 16
          Width = 42
          Height = 12
          Caption = #22320#22270#21495':'
        end
        object Label29: TLabel
          Left = 11
          Top = 40
          Width = 42
          Height = 12
          Caption = #24231#26631' X:'
        end
        object Label30: TLabel
          Left = 11
          Top = 64
          Width = 42
          Height = 12
          Caption = #24231#26631' Y:'
        end
        object EditCastleHomeX: TSpinEditEx
          Left = 56
          Top = 36
          Width = 57
          Height = 21
          MaxValue = 1000
          MinValue = 1
          TabOrder = 1
          Value = 644
          OnChange = EditCastleHomeXChange
        end
        object EditCastleHomeY: TSpinEditEx
          Left = 56
          Top = 60
          Width = 57
          Height = 21
          MaxValue = 1000
          MinValue = 1
          TabOrder = 2
          Value = 290
          OnChange = EditCastleHomeYChange
        end
        object EditCastleHomeMap: TEdit
          Left = 56
          Top = 12
          Width = 57
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          MaxLength = 20
          TabOrder = 0
          Text = '3'
          OnChange = EditCastleHomeMapChange
        end
      end
      object GroupBox12: TGroupBox
        Left = 176
        Top = 8
        Width = 113
        Height = 63
        Caption = #25915#22478#21306#22495#33539#22260
        TabOrder = 1
        object Label34: TLabel
          Left = 11
          Top = 16
          Width = 42
          Height = 12
          Caption = #24231#26631' X:'
        end
        object Label35: TLabel
          Left = 11
          Top = 40
          Width = 42
          Height = 12
          Caption = #24231#26631' Y:'
        end
        object EditWarRangeX: TSpinEditEx
          Left = 56
          Top = 12
          Width = 49
          Height = 21
          MaxValue = 1000
          MinValue = 1
          TabOrder = 0
          Value = 100
          OnChange = EditWarRangeXChange
        end
        object EditWarRangeY: TSpinEditEx
          Left = 56
          Top = 36
          Width = 49
          Height = 21
          MaxValue = 1000
          MinValue = 1
          TabOrder = 1
          Value = 100
          OnChange = EditWarRangeYChange
        end
      end
      object ButtonCastleSave: TButton
        Left = 368
        Top = 165
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 7
        OnClick = ButtonCastleSaveClick
      end
      object GroupBox13: TGroupBox
        Left = 176
        Top = 74
        Width = 113
        Height = 63
        Caption = #31246#25910
        TabOrder = 4
        object Label36: TLabel
          Left = 11
          Top = 40
          Width = 42
          Height = 12
          Caption = #31246#25910#29575':'
        end
        object EditTaxRate: TSpinEditEx
          Left = 56
          Top = 36
          Width = 49
          Height = 21
          MaxValue = 1000
          MinValue = 1
          TabOrder = 1
          Value = 5
          OnChange = EditTaxRateChange
        end
        object CheckBoxGetAllNpcTax: TCheckBox
          Left = 11
          Top = 13
          Width = 94
          Height = 17
          Caption = #25152#26377#21830#20154#20132#31246
          TabOrder = 0
          OnClick = CheckBoxGetAllNpcTaxClick
        end
      end
      object GroupBox14: TGroupBox
        Left = 296
        Top = 8
        Width = 121
        Height = 44
        Caption = #22478#22561#21517#31216
        TabOrder = 2
        object Label33: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #21517#31216':'
        end
        object EditCastleName: TEdit
          Left = 40
          Top = 16
          Width = 73
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = #27801#24052#20811
          OnChange = EditCastleNameChange
        end
      end
      object GroupBox54: TGroupBox
        Left = 176
        Top = 146
        Width = 113
        Height = 47
        Caption = #25104#21592#25240#25187
        TabOrder = 6
        object Label107: TLabel
          Left = 11
          Top = 16
          Width = 42
          Height = 12
          Caption = #25240#25187#29575':'
        end
        object EditCastleMemberPriceRate: TSpinEditEx
          Left = 56
          Top = 12
          Width = 49
          Height = 21
          Hint = #22478#22561#34892#20250#25104#21592#36141#20080#29289#21697#20215#26684#25240#25187#12290#25968#23383#20026#30334#20998#20043#20960#12290
          MaxValue = 200
          MinValue = 10
          TabOrder = 0
          Value = 10
          OnChange = EditCastleMemberPriceRateChange
        end
      end
    end
    object TabSheet5: TTabSheet
      Caption = #20449#24687#25511#21046
      ImageIndex = 8
      object GroupBox36: TGroupBox
        Left = 8
        Top = 8
        Width = 129
        Height = 73
        Caption = #21457#36865#20449#24687#38271#24230
        TabOrder = 0
        object Label71: TLabel
          Left = 11
          Top = 24
          Width = 54
          Height = 12
          Caption = #32842#22825#20449#24687':'
        end
        object Label72: TLabel
          Left = 11
          Top = 48
          Width = 54
          Height = 12
          Caption = #24191#25773#20449#24687':'
        end
        object EditSayMsgMaxLen: TSpinEditEx
          Left = 68
          Top = 20
          Width = 53
          Height = 21
          Hint = #21457#36865#25991#23383#20449#24687#26368#22823#38271#24230#12290
          MaxValue = 255
          MinValue = 1
          TabOrder = 0
          Value = 50
          OnChange = EditSayMsgMaxLenChange
        end
        object EditSayRedMsgMaxLen: TSpinEditEx
          Left = 68
          Top = 44
          Width = 53
          Height = 21
          Hint = 'GM'#21457#32418#33394#24191#25773#25991#23383#26368#22823#38271#24230#12290
          MaxValue = 255
          MinValue = 1
          TabOrder = 1
          Value = 50
          OnChange = EditSayRedMsgMaxLenChange
        end
      end
      object GroupBox37: TGroupBox
        Left = 8
        Top = 88
        Width = 129
        Height = 49
        Caption = #20801#35768#21898#35805#31561#32423
        TabOrder = 4
        object Label73: TLabel
          Left = 11
          Top = 24
          Width = 54
          Height = 12
          Caption = #20154#29289#31561#32423':'
        end
        object EditCanShoutMsgLevel: TSpinEditEx
          Left = 68
          Top = 20
          Width = 53
          Height = 21
          Hint = #20801#35768#21898#35805#31561#32423#65292#20154#29289#24517#39035#21040#36798#25351#23450#31561#32423#21518#25165#21487#20197#21898#35805#12290
          MaxValue = 65535
          MinValue = 1
          TabOrder = 0
          Value = 50
          OnChange = EditCanShoutMsgLevelChange
        end
      end
      object GroupBox38: TGroupBox
        Left = 144
        Top = 8
        Width = 137
        Height = 65
        Caption = #21457#36865#24191#25773#20449#24687
        TabOrder = 1
        object Label75: TLabel
          Left = 11
          Top = 40
          Width = 54
          Height = 12
          Caption = #21457#36865#21629#20196':'
        end
        object CheckBoxShutRedMsgShowGMName: TCheckBox
          Left = 8
          Top = 16
          Width = 105
          Height = 17
          Hint = 'GM'#21457#36865#32418#33394#24191#25773#25991#20214#20449#24687#26102#26159#21542#26174#31034#20154#29289#30340#21517#23383#12290
          Caption = #26174#31034#20154#29289#21517#31216
          TabOrder = 0
          OnClick = CheckBoxShutRedMsgShowGMNameClick
        end
        object EditGMRedMsgCmd: TEdit
          Left = 72
          Top = 37
          Width = 41
          Height = 20
          Hint = #21457#36865#32418#33394#24191#25773#25991#20214#20449#24687#21629#20196#31526#12290#40664#35748#20026#8216'!'#8217#12290
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          MaxLength = 20
          TabOrder = 1
          OnChange = EditGMRedMsgCmdChange
        end
      end
      object ButtonMsgSave: TButton
        Left = 368
        Top = 165
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 6
        OnClick = ButtonMsgSaveClick
      end
      object GroupBox68: TGroupBox
        Left = 144
        Top = 80
        Width = 137
        Height = 97
        Caption = #21457#36865#20449#24687#36895#24230#25511#21046
        TabOrder = 3
        object Label135: TLabel
          Left = 11
          Top = 24
          Width = 54
          Height = 12
          Caption = #21457#36865#38388#38548':'
        end
        object Label138: TLabel
          Left = 11
          Top = 48
          Width = 54
          Height = 12
          Caption = #21457#36865#25968#37327':'
        end
        object Label139: TLabel
          Left = 11
          Top = 72
          Width = 54
          Height = 12
          Caption = #31105#35328#26102#38388':'
        end
        object Label140: TLabel
          Left = 115
          Top = 24
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label141: TLabel
          Left = 115
          Top = 72
          Width = 12
          Height = 12
          Caption = #31186
        end
        object EditSayMsgTime: TSpinEditEx
          Left = 68
          Top = 20
          Width = 45
          Height = 21
          MaxValue = 1000000
          MinValue = 1
          TabOrder = 0
          Value = 50
          OnChange = EditSayMsgTimeChange
        end
        object EditSayMsgCount: TSpinEditEx
          Left = 68
          Top = 44
          Width = 45
          Height = 21
          MaxValue = 255
          MinValue = 1
          TabOrder = 1
          Value = 50
          OnChange = EditSayMsgCountChange
        end
        object EditDisableSayMsgTime: TSpinEditEx
          Left = 68
          Top = 68
          Width = 45
          Height = 21
          MaxValue = 100000
          MinValue = 1
          TabOrder = 2
          Value = 50
          OnChange = EditDisableSayMsgTimeChange
        end
      end
      object GroupBox71: TGroupBox
        Left = 8
        Top = 144
        Width = 129
        Height = 49
        Caption = #26174#31034#21069#32512#20449#24687
        TabOrder = 5
        object CheckBoxShowPreFixMsg: TCheckBox
          Left = 8
          Top = 16
          Width = 105
          Height = 17
          Hint = #28216#25103#20013#32842#22825#26694#20013#26174#31034#30340#20449#24687#26159#21542#26174#31034#21069#32512#20449#24687#12290
          Caption = #26174#31034#20449#24687#30340#21069#32512
          TabOrder = 0
          OnClick = CheckBoxShowPreFixMsgClick
        end
      end
      object GroupBox80: TGroupBox
        Left = 288
        Top = 8
        Width = 129
        Height = 65
        Caption = #31169#32842#31561#32423#26174#31034
        TabOrder = 2
        object Label167: TLabel
          Left = 8
          Top = 40
          Width = 30
          Height = 12
          Caption = #21518#32512':'
        end
        object CheckBoxShowWhisperLevelMsg: TCheckBox
          Left = 8
          Top = 16
          Width = 97
          Height = 17
          Caption = #31169#32842#31561#32423#26174#31034
          TabOrder = 0
          OnClick = CheckBoxShowWhisperLevelMsgClick
        end
        object EditShowWhisperLevelMsg: TEdit
          Left = 40
          Top = 36
          Width = 81
          Height = 20
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 1
          Text = '[Lv%d]'
          OnChange = EditShowWhisperLevelMsgChange
        end
      end
      object GroupBox72: TGroupBox
        Left = 8
        Top = 200
        Width = 129
        Height = 57
        Caption = #21315#37324#20256#38899#12289#20256#38899#31570
        TabOrder = 7
        object Label200: TLabel
          Left = 6
          Top = 24
          Width = 54
          Height = 12
          Caption = #21457#36865#38388#38548':'
        end
        object Label201: TLabel
          Left = 110
          Top = 24
          Width = 12
          Height = 12
          Caption = #31186
        end
        object EditUserItemSayMsgTime: TSpinEditEx
          Left = 63
          Top = 20
          Width = 45
          Height = 21
          MaxValue = 1000000
          MinValue = 1
          TabOrder = 0
          Value = 50
          OnChange = EditUserItemSayMsgTimeChange
        end
      end
      object grp6: TGroupBox
        Left = 144
        Top = 184
        Width = 137
        Height = 49
        Caption = #20449#24687#21457#36865#38271#24230#38480#21046
        TabOrder = 8
        object lbl16: TLabel
          Left = 8
          Top = 24
          Width = 54
          Height = 12
          Caption = #38271#24230#38480#21046':'
        end
        object seMaxInputStringLen: TSpinEditEx
          Left = 68
          Top = 20
          Width = 61
          Height = 21
          MaxValue = 100000
          MinValue = 1
          TabOrder = 0
          Value = 50
          OnChange = seMaxInputStringLenChange
        end
      end
    end
    object TabSheet8: TTabSheet
      Caption = #25991#23383#39068#33394
      ImageIndex = 11
      object ButtonMsgColorSave: TButton
        Left = 464
        Top = 266
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 12
        OnClick = ButtonMsgColorSaveClick
      end
      object GroupBox55: TGroupBox
        Left = 8
        Top = 8
        Width = 100
        Height = 63
        Caption = #32842#22825#25991#23383
        TabOrder = 0
        object Label108: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label109: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditHearMsgFColor: TColorIndexEdit
          Left = 34
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditHearMsgFColorChange
          ShowNoneColor = False
        end
        object EdittHearMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EdittHearMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox56: TGroupBox
        Left = 8
        Top = 72
        Width = 100
        Height = 63
        Caption = #25509#25910#31169#32842#25991#23383
        TabOrder = 4
        object Label110: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label111: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditWhisperMsgFColor: TColorIndexEdit
          Left = 34
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditWhisperMsgFColorChange
          ShowNoneColor = False
        end
        object EditWhisperMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditWhisperMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox57: TGroupBox
        Left = 8
        Top = 136
        Width = 100
        Height = 63
        Caption = #25509#25910'GM'#31169#32842#25991#23383
        TabOrder = 8
        object Label112: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label113: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditGMWhisperMsgFColor: TColorIndexEdit
          Left = 34
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditGMWhisperMsgFColorChange
          ShowNoneColor = False
        end
        object EditGMWhisperMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditGMWhisperMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox58: TGroupBox
        Left = 112
        Top = 8
        Width = 100
        Height = 63
        Caption = #32418#33394#25552#31034#25991#23383
        TabOrder = 1
        object Label116: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label117: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditRedMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditRedMsgFColorChange
          ShowNoneColor = False
        end
        object EditRedMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditRedMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox59: TGroupBox
        Left = 112
        Top = 72
        Width = 100
        Height = 63
        Caption = #32511#33394#25552#31034#25991#23383
        TabOrder = 5
        object Label120: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label121: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditGreenMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditGreenMsgFColorChange
          ShowNoneColor = False
        end
        object EditGreenMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditGreenMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox60: TGroupBox
        Left = 112
        Top = 136
        Width = 100
        Height = 63
        Caption = #34013#33394#25552#31034#25991#23383
        TabOrder = 9
        object Label124: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label125: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditBlueMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditBlueMsgFColorChange
          ShowNoneColor = False
        end
        object EditBlueMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditBlueMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox61: TGroupBox
        Left = 216
        Top = 8
        Width = 100
        Height = 63
        Caption = #21898#35805#25991#23383
        TabOrder = 2
        object Label128: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label129: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditCryMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditCryMsgFColorChange
          ShowNoneColor = False
        end
        object EditCryMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditCryMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox62: TGroupBox
        Left = 216
        Top = 72
        Width = 100
        Height = 63
        Caption = #34892#20250#32842#22825#25991#23383
        TabOrder = 6
        object Label132: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label133: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditGuildMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditGuildMsgFColorChange
          ShowNoneColor = False
        end
        object EditGuildMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditGuildMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox63: TGroupBox
        Left = 216
        Top = 136
        Width = 100
        Height = 63
        Caption = #32534#32452#32842#22825#25991#23383
        TabOrder = 10
        object Label136: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label137: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditGroupMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditGroupMsgFColorChange
          ShowNoneColor = False
        end
        object EditGroupMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditGroupMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox65: TGroupBox
        Left = 320
        Top = 8
        Width = 100
        Height = 63
        Caption = #31069#31119#35821#25991#23383
        TabOrder = 3
        object Label122: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label123: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditCustMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditCustMsgFColorChange
          ShowNoneColor = False
        end
        object EditCustMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditCustMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox75: TGroupBox
        Left = 320
        Top = 72
        Width = 100
        Height = 63
        Caption = #21315#37324#20256#38899#25991#23383
        TabOrder = 7
        object Label147: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label148: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditUserSayMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditUserSayMsgFColorChange
          ShowNoneColor = False
        end
        object EditUserSayMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditUserSayMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox76: TGroupBox
        Left = 320
        Top = 136
        Width = 100
        Height = 63
        Caption = #20256#38899#31570#25991#23383
        TabOrder = 11
        object Label151: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label152: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditTopUserSayMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditTopUserSayMsgFColorChange
          ShowNoneColor = False
        end
        object EditTopUserSayMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditTopUserSayMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox84: TGroupBox
        Left = 424
        Top = 8
        Width = 100
        Height = 63
        Caption = #29289#21697#25481#33853#25552#31034
        TabOrder = 13
        object Label198: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label199: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditDropItemFColor: TColorIndexEdit
          Left = 34
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditDropItemFColorChange
          ShowNoneColor = False
        end
        object EditDropItemBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditDropItemBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox85: TGroupBox
        Left = 112
        Top = 200
        Width = 100
        Height = 63
        Caption = #22269#23478#32842#22825#25991#23383
        TabOrder = 14
        object Label202: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label203: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object EditNationMsgFColor: TColorIndexEdit
          Left = 34
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = EditNationMsgFColorChange
          ShowNoneColor = False
        end
        object EditNationMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = EditNationMsgBColorChange
          ShowNoneColor = False
        end
      end
      object grp1: TGroupBox
        Left = 216
        Top = 200
        Width = 204
        Height = 63
        Caption = 'NPC'#23545#35805#26694#39068#33394
        TabOrder = 15
        object lbl1: TLabel
          Left = 109
          Top = 16
          Width = 30
          Height = 12
          Caption = #31227#21160':'
        end
        object lbl2: TLabel
          Left = 109
          Top = 40
          Width = 30
          Height = 12
          Caption = #25353#19979':'
        end
        object Label227: TLabel
          Left = 5
          Top = 19
          Width = 30
          Height = 12
          Caption = #40664#35748':'
        end
        object seNPCLabelMouseMoveColor: TColorIndexEdit
          Left = 138
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seNPCLabelMouseMoveColorChange
          ShowNoneColor = False
        end
        object seNPCLabelMouseDownColor: TColorIndexEdit
          Left = 138
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seNPCLabelMouseDownColorChange
          ShowNoneColor = False
        end
        object seNPCLabelNormalColor: TColorIndexEdit
          Left = 34
          Top = 14
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = seNPCLabelNormalColorChange
          ShowNoneColor = False
        end
        object chkNPCLabelFontStroke: TCheckBox
          Left = 5
          Top = 40
          Width = 86
          Height = 17
          Caption = 'NPC'#25991#23383#25551#36793
          TabOrder = 3
          OnClick = chkNPCLabelFontStrokeClick
        end
      end
      object GroupBox30: TGroupBox
        Left = 8
        Top = 200
        Width = 100
        Height = 63
        Caption = #21457#36865#31169#32842#25991#23383
        TabOrder = 16
        object Label229: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label230: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object seSendWhisperMsgFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seSendWhisperMsgFColorChange
          ShowNoneColor = False
        end
        object seSendWhisperMsgBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seSendWhisperMsgBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox86: TGroupBox
        Left = 424
        Top = 72
        Width = 100
        Height = 63
        Caption = #20803#23453#20449#24687#21047#26032
        TabOrder = 17
        object Label231: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label232: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object seRefreshGameGoldFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seRefreshGameGoldFColorChange
          ShowNoneColor = False
        end
        object seRefreshGameGoldBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seRefreshGameGoldBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox89: TGroupBox
        Left = 424
        Top = 136
        Width = 100
        Height = 63
        Caption = #32842#22825#20449#24687#20801#35768
        TabOrder = 18
        object Label233: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label234: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object seShowWhisperFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seShowWhisperFColorChange
          ShowNoneColor = False
        end
        object seShowWhisperBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seShowWhisperBColorChange
          ShowNoneColor = False
        end
      end
      object GroupBox90: TGroupBox
        Left = 424
        Top = 200
        Width = 100
        Height = 63
        Caption = #32842#22825#20449#24687#25298#32477
        TabOrder = 19
        object Label235: TLabel
          Left = 5
          Top = 18
          Width = 30
          Height = 12
          Caption = #25991#23383':'
        end
        object Label236: TLabel
          Left = 5
          Top = 40
          Width = 30
          Height = 12
          Caption = #32972#26223':'
        end
        object seCloseWhisperFColor: TColorIndexEdit
          Left = 34
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seCloseWhisperFColorChange
          ShowNoneColor = False
        end
        object seCloseWhisperBColor: TColorIndexEdit
          Left = 34
          Top = 36
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seCloseWhisperBColorChange
          ShowNoneColor = False
        end
      end
    end
    object TabSheet6: TTabSheet
      Caption = #26102#38388#25511#21046
      ImageIndex = 9
      object GroupBox39: TGroupBox
        Left = 4
        Top = 8
        Width = 98
        Height = 45
        Caption = #30003#35831#25915#22478#22825#25968
        TabOrder = 0
        object Label74: TLabel
          Left = 7
          Top = 20
          Width = 30
          Height = 12
          Caption = #22825#25968':'
        end
        object Label77: TLabel
          Left = 79
          Top = 20
          Width = 12
          Height = 12
          Caption = #22825
        end
        object EditStartCastleWarDays: TSpinEditEx
          Left = 40
          Top = 16
          Width = 37
          Height = 21
          Hint = #30003#35831#25915#22478#25152#38656#22825#25968#65292#21253#25324#24403#22825#12290
          MaxValue = 10
          MinValue = 2
          TabOrder = 0
          Value = 4
          OnChange = EditStartCastleWarDaysChange
        end
      end
      object GroupBox40: TGroupBox
        Left = 4
        Top = 57
        Width = 98
        Height = 45
        Caption = #25915#22478#24320#22987#26102#38388
        TabOrder = 4
        object Label76: TLabel
          Left = 6
          Top = 20
          Width = 30
          Height = 12
          Caption = #26102#38388':'
        end
        object Label78: TLabel
          Left = 78
          Top = 20
          Width = 12
          Height = 12
          Caption = #28857
        end
        object EditStartCastlewarTime: TSpinEditEx
          Left = 39
          Top = 16
          Width = 37
          Height = 21
          Hint = #24320#22987#25915#22478#26102#38388#65292'20'#20195#34920'20'#65306'00'
          MaxValue = 24
          MinValue = 1
          TabOrder = 0
          Value = 20
          OnChange = EditStartCastlewarTimeChange
        end
      end
      object GroupBox41: TGroupBox
        Left = 4
        Top = 107
        Width = 98
        Height = 45
        Caption = #25915#22478#32467#26463#25552#31034
        TabOrder = 6
        object Label79: TLabel
          Left = 6
          Top = 20
          Width = 30
          Height = 12
          Caption = #26102#38388':'
        end
        object Label80: TLabel
          Left = 78
          Top = 20
          Width = 12
          Height = 12
          Caption = #20998
        end
        object EditShowCastleWarEndMsgTime: TSpinEditEx
          Left = 39
          Top = 16
          Width = 37
          Height = 21
          Hint = #25915#22478#25112#32467#26463#21069#25351#23450#26102#38388#25552#31034#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditShowCastleWarEndMsgTimeChange
        end
      end
      object GroupBox42: TGroupBox
        Left = 106
        Top = 8
        Width = 106
        Height = 45
        Caption = #25915#22478#26102#38271
        TabOrder = 1
        object Label81: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #26102#38271':'
        end
        object Label82: TLabel
          Left = 88
          Top = 20
          Width = 12
          Height = 12
          Caption = #20998
        end
        object EditCastleWarTime: TSpinEditEx
          Left = 41
          Top = 16
          Width = 45
          Height = 21
          Hint = #25915#22478#26102#38388#38271#24230#65292#40664#35748#20026'3'#20010#23567#26102#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 180
          OnChange = EditCastleWarTimeChange
        end
      end
      object GroupBox43: TGroupBox
        Left = 106
        Top = 57
        Width = 106
        Height = 45
        Caption = #31105#27490#21344#39046#26102#38388
        TabOrder = 5
        object Label83: TLabel
          Left = 8
          Top = 20
          Width = 30
          Height = 12
          Caption = #26102#38271':'
        end
        object Label84: TLabel
          Left = 88
          Top = 20
          Width = 12
          Height = 12
          Caption = #20998
        end
        object EditGetCastleTime: TSpinEditEx
          Left = 41
          Top = 16
          Width = 45
          Height = 21
          Hint = #25915#22478#25112#24320#22987#26102#65292#25351#23450#26102#38388#20869#19981#20801#35768#21344#39046#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditGetCastleTimeChange
        end
      end
      object GroupBox44: TGroupBox
        Left = 216
        Top = 8
        Width = 175
        Height = 92
        Caption = #20154#29289#25968#25454#22788#29702#38388#38548
        TabOrder = 2
        object Label85: TLabel
          Left = 6
          Top = 20
          Width = 108
          Height = 12
          Caption = #20154#29289#25968#25454#20445#23384#38388#38548#65306
        end
        object Label86: TLabel
          Left = 158
          Top = 20
          Width = 12
          Height = 12
          Caption = #20998
        end
        object Label87: TLabel
          Left = 6
          Top = 44
          Width = 108
          Height = 12
          Caption = #20154#29289#36864#20986#37322#25918#38388#38548#65306
        end
        object Label88: TLabel
          Left = 158
          Top = 44
          Width = 12
          Height = 12
          Caption = #20998
        end
        object lbl6: TLabel
          Left = 6
          Top = 68
          Width = 108
          Height = 12
          Caption = #25968#25454#35835#21462#20445#23384#36229#26102#65306
        end
        object lbl7: TLabel
          Left = 158
          Top = 68
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seSaveHumanRcdTime: TSpinEditEx
          Left = 111
          Top = 16
          Width = 45
          Height = 21
          Hint = #20154#29289#25968#25454#33258#21160#20445#23384#38388#38548#26102#38388#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seSaveHumanRcdTimeChange
        end
        object seHumanFreeDelayTime: TSpinEditEx
          Left = 111
          Top = 40
          Width = 45
          Height = 21
          Hint = #20154#29289#36864#21518#25351#23450#26102#38388#21518#37322#25918#26102#38388#65292#36825#20010#26102#38388#19981#33021#22826#30701#65292#21542#21017#21487#33021#24341#36215#38169#35823#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 1
          Value = 5
          OnChange = seHumanFreeDelayTimeChange
        end
        object seGetDBSockMsgTime: TSpinEditEx
          Left = 111
          Top = 64
          Width = 45
          Height = 21
          Hint = #22914#26524#24341#25806#32463#24120'"'#20986#29616'[RunDB]'#20445#23384#29992#25143#25968#25454#36229#26102'"'#35831#35843#25972#36825#20010#20540','#20294#19981#35201#35843#22826#39640#21482#35201#19981#20986#36825#20010#25552#31034#23601#21487#20197#20102
          MaxValue = 255
          MinValue = 1
          TabOrder = 2
          Value = 5
          OnChange = seGetDBSockMsgTimeChange
        end
      end
      object GroupBox46: TGroupBox
        Left = 4
        Top = 156
        Width = 208
        Height = 67
        Caption = #28165#29702#26102#38388' ('#21333#20301':'#31186')'
        TabOrder = 10
        object Label89: TLabel
          Left = 108
          Top = 19
          Width = 36
          Height = 12
          Caption = #27515#23608#65306
        end
        object Label91: TLabel
          Left = 108
          Top = 43
          Width = 36
          Height = 12
          Caption = #29289#21697#65306
        end
        object lbl4: TLabel
          Left = 6
          Top = 19
          Width = 36
          Height = 12
          Caption = #20154#24418#65306
        end
        object Label222: TLabel
          Left = 6
          Top = 43
          Width = 36
          Height = 12
          Caption = #20551#20154#65306
        end
        object seMakeMonGhostTime: TSpinEditEx
          Left = 141
          Top = 15
          Width = 60
          Height = 21
          Hint = #28165#38500#22320#19978#27515#23608#26102#38388#12290
          MaxValue = 6000000
          MinValue = 60
          TabOrder = 1
          Value = 180
          OnChange = seMakeMonGhostTimeChange
        end
        object seClearDropOnFloorItemTime: TSpinEditEx
          Left = 141
          Top = 39
          Width = 60
          Height = 21
          Hint = #28165#38500#22320#19978#29289#21697#26102#38388#12290
          MaxValue = 6000000
          MinValue = 60
          TabOrder = 3
          Value = 3600
          OnChange = seClearDropOnFloorItemTimeChange
        end
        object seMakeGhostTime: TSpinEditEx
          Left = 39
          Top = 15
          Width = 60
          Height = 21
          Hint = #28165#38500#22320#19978#20154#12289#20154#24418#24618#12289#21487#25366#31867#23608#20307#22914':'#40481','#40575#31561#38388#38548
          MaxValue = 6000000
          MinValue = 60
          TabOrder = 0
          Value = 180
          OnChange = seMakeGhostTimeChange
        end
        object seMakeDummyGhostTime: TSpinEditEx
          Left = 39
          Top = 39
          Width = 60
          Height = 21
          Hint = #28165#38500#22320#19978#27515#23608#26102#38388#12290
          MaxValue = 6000000
          MinValue = 60
          TabOrder = 2
          Value = 180
          OnChange = seMakeDummyGhostTimeChange
        end
      end
      object GroupBox47: TGroupBox
        Left = 216
        Top = 107
        Width = 117
        Height = 45
        Caption = #29190#29289#21697#21487#25441#26102#38388
        TabOrder = 8
        object Label93: TLabel
          Left = 11
          Top = 20
          Width = 30
          Height = 12
          Caption = #26102#38271':'
        end
        object Label94: TLabel
          Left = 97
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seFloorItemCanPickUpTime: TSpinEditEx
          Left = 44
          Top = 16
          Width = 51
          Height = 21
          Hint = #20182#20154#29190#24618#29289#25110#25481#22320#19978#29289#21697#21487#25441#38388#38548#26102#38388#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seFloorItemCanPickUpTimeChange
        end
      end
      object ButtonTimeSave: TButton
        Left = 456
        Top = 262
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 12
        OnClick = ButtonTimeSaveClick
      end
      object GroupBox70: TGroupBox
        Left = 106
        Top = 107
        Width = 106
        Height = 45
        Caption = #34892#20250#25112#26102#38271
        TabOrder = 7
        object Label143: TLabel
          Left = 8
          Top = 19
          Width = 30
          Height = 12
          Caption = #26102#38271':'
        end
        object Label144: TLabel
          Left = 88
          Top = 19
          Width = 12
          Height = 12
          Caption = #20998
        end
        object EditGuildWarTime: TSpinEditEx
          Left = 41
          Top = 15
          Width = 45
          Height = 21
          Hint = #34892#20250#25112#26102#38388#38271#24230#12290
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditGuildWarTimeChange
        end
      end
      object grp3: TGroupBox
        Left = 395
        Top = 8
        Width = 130
        Height = 92
        Caption = #29305#27530#20256#36865#38388#38548
        TabOrder = 3
        object Label57: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #22827#22971#20256#36865':'
        end
        object Label59: TLabel
          Left = 8
          Top = 44
          Width = 54
          Height = 12
          Caption = #24072#24466#20256#36865':'
        end
        object Label146: TLabel
          Left = 8
          Top = 68
          Width = 54
          Height = 12
          Caption = #35760#24518#20256#36865':'
        end
        object Label168: TLabel
          Left = 110
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label169: TLabel
          Left = 110
          Top = 44
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label207: TLabel
          Left = 110
          Top = 68
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seDearRecallTime: TSpinEditEx
          Left = 63
          Top = 16
          Width = 45
          Height = 21
          MaxValue = 6000000
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 10
          OnChange = seDearRecallTimeChange
        end
        object seMasterRecallTime: TSpinEditEx
          Left = 63
          Top = 40
          Width = 45
          Height = 21
          MaxValue = 6000000
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 10
          OnChange = seMasterRecallTimeChange
        end
        object seGroupRecallTime: TSpinEditEx
          Left = 63
          Top = 64
          Width = 45
          Height = 21
          MaxValue = 255
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 10
          OnChange = seGroupRecallTimeChange
        end
      end
      object GroupBox45: TGroupBox
        Left = 344
        Top = 107
        Width = 153
        Height = 83
        Caption = #39569#39532#26102#38388#25511#21046
        TabOrder = 9
        object Label214: TLabel
          Left = 8
          Top = 20
          Width = 78
          Height = 12
          Caption = #19978#19979#26102#38388#38388#38548':'
        end
        object Label219: TLabel
          Left = 132
          Top = 20
          Width = 12
          Height = 12
          Caption = #31186
        end
        object Label92: TLabel
          Left = 8
          Top = 43
          Width = 78
          Height = 12
          Caption = #19978#39532#20934#22791#26102#38388':'
        end
        object Label221: TLabel
          Left = 132
          Top = 43
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seHorseTakeTime: TSpinEditEx
          Left = 86
          Top = 16
          Width = 44
          Height = 21
          Hint = #19979#39532#38388#38548#19968#23450#30340#26102#38388#21518#25165#33021#37325#26032#39569#39532
          MaxValue = 6000000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seHorseTakeTimeChange
        end
        object seTakeOnHorseUseTime: TSpinEdit
          Left = 86
          Top = 39
          Width = 44
          Height = 21
          Hint = #19978#39532#20934#22791#26102#20505#22823#20110'1'#31186#21518#28216#25103#20013#21017#26174#31034#36827#24230#13#10#13#10#24453#36827#24230#23436#27605#21518#33258#21160#19978#39532#65292#20934#22791#26102#38388#20869#20219#20309#25805#20316#23558#21462#28040#39569#39532
          MaxValue = 10000
          MinValue = 0
          TabOrder = 1
          Value = 10
          OnChange = seTakeOnHorseUseTimeChange
        end
        object chkReadyOnHorseDisableAction: TCheckBox
          Left = 8
          Top = 63
          Width = 137
          Height = 17
          Hint = #21246#36873#21518#20934#22791#26102#38388#20869#35282#33394#22914#26524#36827#34892#20219#20309#25805#20316#21017#21462#28040#19978#39532
          Caption = #20934#22791#26102#38388#20869#31105#27490#21160#20316
          TabOrder = 2
          OnClick = chkReadyOnHorseDisableActionClick
        end
      end
      object GroupBox81: TGroupBox
        Left = 216
        Top = 156
        Width = 117
        Height = 67
        Caption = 'NPC'#38388#38548' ('#27627#31186')'
        TabOrder = 11
        object Label220: TLabel
          Left = 8
          Top = 20
          Width = 54
          Height = 12
          Caption = #25353#38062#28857#20987':'
        end
        object Label90: TLabel
          Left = 8
          Top = 44
          Width = 54
          Height = 12
          Caption = 'NPC '#28857#20987':'
        end
        object seNpcButtonClickTime: TSpinEditEx
          Left = 64
          Top = 16
          Width = 46
          Height = 21
          Hint = #25511#21046#29609#23478#28857#20987'NPC'#23545#35805#26694#20869#25353#38062#30340#26102#38388#38388#38548#65292#23567#20110#38388#38548#21017#28857#20987#26080#25928
          Increment = 100
          MaxValue = 3000
          MinValue = 200
          TabOrder = 0
          Value = 500
          OnChange = seNpcButtonClickTimeChange
        end
        object seNpcActorClickTime: TSpinEditEx
          Left = 64
          Top = 40
          Width = 46
          Height = 21
          Hint = #25511#21046#29609#23478#28857#20987'NPC'#30340#26102#38388#38388#38548#65292#23567#20110#38388#38548#21017#28857#20987#26080#25928
          Increment = 100
          MaxValue = 3600000
          MinValue = 200
          TabOrder = 1
          Value = 1000
          OnChange = seNpcActorClickTimeChange
        end
      end
      object grp8: TGroupBox
        Left = 344
        Top = 194
        Width = 153
        Height = 45
        Caption = #20154#29289'J'#21464#37327#28165'0'#26102#38388
        TabOrder = 13
        object Label228: TLabel
          Left = 8
          Top = 20
          Width = 84
          Height = 12
          Caption = 'J'#21464#37327#28165'0'#26102#38388#65306
        end
        object lbl18: TLabel
          Left = 130
          Top = 20
          Width = 12
          Height = 12
          Caption = #28857
        end
        object sePlayerVarJClearTime: TSpinEditEx
          Left = 86
          Top = 16
          Width = 41
          Height = 21
          Hint = #24314#35758#19981#35201#39057#32321#20462#25913#27492#26102#38388#65292#22240#20026#21487#33021#23548#33268#24403#22825#30340#21464#37327#19981#28165
          Increment = 100
          MaxValue = 23
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = sePlayerVarJClearTimeChange
        end
      end
    end
    object TabSheet7: TTabSheet
      Caption = #20215#26684#36153#29992
      ImageIndex = 10
      object GroupBox48: TGroupBox
        Left = 8
        Top = 8
        Width = 137
        Height = 49
        Caption = #30003#35831#34892#20250#36153#29992
        TabOrder = 0
        object Label95: TLabel
          Left = 11
          Top = 24
          Width = 30
          Height = 12
          Caption = #36153#29992':'
        end
        object EditBuildGuildPrice: TSpinEditEx
          Left = 44
          Top = 20
          Width = 77
          Height = 21
          Hint = #30003#35831#21019#24314#34892#20250#25152#38656#36153#29992#12290
          MaxValue = 100000000
          MinValue = 1000
          TabOrder = 0
          Value = 1000000
          OnChange = EditBuildGuildPriceChange
        end
      end
      object GroupBox49: TGroupBox
        Left = 8
        Top = 64
        Width = 137
        Height = 49
        Caption = #30003#35831#34892#20250#25112#36153#29992
        TabOrder = 2
        object Label96: TLabel
          Left = 11
          Top = 24
          Width = 30
          Height = 12
          Caption = #36153#29992':'
        end
        object EditGuildWarPrice: TSpinEditEx
          Left = 44
          Top = 20
          Width = 77
          Height = 21
          Hint = #30003#35831#34892#20250#25112#20105#25152#38656#36153#29992#12290
          MaxValue = 100000000
          MinValue = 1000
          TabOrder = 0
          Value = 30000
          OnChange = EditGuildWarPriceChange
        end
      end
      object GroupBox50: TGroupBox
        Left = 8
        Top = 120
        Width = 137
        Height = 49
        Caption = #28860#33647#20215#26684
        TabOrder = 3
        object Label97: TLabel
          Left = 11
          Top = 24
          Width = 30
          Height = 12
          Caption = #20215#26684':'
        end
        object EditMakeDurgPrice: TSpinEditEx
          Left = 44
          Top = 20
          Width = 77
          Height = 21
          Hint = #28860#21046#33647#21697#25152#38656#36153#29992#12290
          MaxValue = 100000000
          MinValue = 10
          TabOrder = 0
          Value = 100
          OnChange = EditMakeDurgPriceChange
        end
      end
      object ButtonPriceSave: TButton
        Left = 8
        Top = 173
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 4
        OnClick = ButtonPriceSaveClick
      end
      object GroupBox66: TGroupBox
        Left = 152
        Top = 8
        Width = 137
        Height = 73
        Caption = #20462#29702#29289#21697
        TabOrder = 1
        object Label126: TLabel
          Left = 11
          Top = 24
          Width = 78
          Height = 12
          Caption = #29305#20462#20215#26684#20493#25968':'
        end
        object Label127: TLabel
          Left = 23
          Top = 48
          Width = 66
          Height = 12
          Caption = #26222#20462#25481#25345#20037':'
        end
        object EditSuperRepairPriceRate: TSpinEditEx
          Left = 88
          Top = 20
          Width = 41
          Height = 21
          Hint = #29305#20462#29289#21697#20215#26684#20493#25968#65292#40664#35748#20026#19977#20493#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 0
          Value = 3
          OnChange = EditSuperRepairPriceRateChange
        end
        object EditRepairItemDecDura: TSpinEditEx
          Left = 88
          Top = 44
          Width = 41
          Height = 21
          Hint = #26222#36890#20462#29702#20250#25481#25345#20037#28857#25968#65281#65281#65281
          MaxValue = 100
          MinValue = 1
          TabOrder = 1
          Value = 3
          OnChange = EditRepairItemDecDuraChange
        end
      end
      object grp7: TGroupBox
        Left = 296
        Top = 8
        Width = 217
        Height = 73
        Caption = #20986#21806#29289#21697#32473'NPC'#21830#24215#20215#26684
        TabOrder = 5
        object chkSellItemToNpcShopNoCalcAddProperty: TCheckBox
          Left = 8
          Top = 22
          Width = 169
          Height = 17
          Caption = #19981#31639#26497#21697#21450#38468#21152#23646#24615
          TabOrder = 0
          OnClick = chkSellItemToNpcShopNoCalcAddPropertyClick
        end
        object chkShowNewValueFromBuyNpcItem: TCheckBox
          Left = 8
          Top = 45
          Width = 185
          Height = 17
          Caption = 'NPC'#21830#24215#29289#21697#26174#31034#26497#21697#23646#24615
          TabOrder = 1
          OnClick = chkShowNewValueFromBuyNpcItemClick
        end
      end
    end
    object TabSheet9: TTabSheet
      Caption = #20154#29289#27515#20129
      ImageIndex = 12
      object PageControl1: TPageControl
        Left = 0
        Top = 0
        Width = 529
        Height = 291
        ActivePage = TabSheet14
        Align = alClient
        TabOrder = 0
        object TabSheet13: TTabSheet
          Caption = #26222#36890#36873#39033
          object GroupBox67: TGroupBox
            Left = 8
            Top = 8
            Width = 201
            Height = 217
            Caption = #27515#20129#25481#29289#21697#35268#21017
            TabOrder = 0
            object Label204: TLabel
              Left = 62
              Top = 173
              Width = 132
              Height = 12
              Caption = #20197#19979#29609#23478#27515#20129#19981#25481#33853#29289#21697
            end
            object Label205: TLabel
              Left = 8
              Top = 195
              Width = 96
              Height = 12
              Caption = #20154#29289#27515#20129#26368#22810#25481#33853
            end
            object Label206: TLabel
              Left = 160
              Top = 195
              Width = 24
              Height = 12
              Caption = #29289#21697
            end
            object CheckBoxKillByMonstDropUseItem: TCheckBox
              Left = 8
              Top = 16
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#24618#29289#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#36523#19978#25140#30340#29289#21697#12290
              Caption = #34987#24618#29289#26432#27515#25481#35013#22791
              TabOrder = 0
              OnClick = CheckBoxKillByMonstDropUseItemClick
            end
            object CheckBoxKillByHumanDropUseItem: TCheckBox
              Left = 8
              Top = 32
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#21035#20154#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#36523#19978#25140#30340#29289#21697#12290
              Caption = #34987#20154#29289#26432#27515#25481#35013#22791
              TabOrder = 1
              OnClick = CheckBoxKillByHumanDropUseItemClick
            end
            object CheckBoxDieScatterBag: TCheckBox
              Left = 8
              Top = 114
              Width = 113
              Height = 17
              Hint = #24403#20154#29289#27515#20129#26102#20250#25353#25481#33853#26426#29575#25481#33853#32972#21253#37324#30340#29289#21697#12290
              Caption = #27515#20129#25481#32972#21253#29289#21697
              TabOrder = 2
              OnClick = CheckBoxDieScatterBagClick
            end
            object CheckBoxDieDropGold: TCheckBox
              Left = 8
              Top = 131
              Width = 113
              Height = 17
              Hint = #24403#20154#29289#27515#20129#26102#20250#25481#33853#36523#19978#30340#37329#24065#12290
              Caption = #27515#20129#25481#37329#24065
              TabOrder = 3
              OnClick = CheckBoxDieDropGoldClick
            end
            object CheckBoxDieRedScatterBagAll: TCheckBox
              Left = 8
              Top = 147
              Width = 145
              Height = 17
              Hint = #32418#21517#20154#29289#27515#20129#26102#25481#33853#32972#21253#20013#20840#37096#29289#21697#12290
              Caption = #32418#21517#25481#20840#37096#32972#21253#29289#21697
              TabOrder = 4
              OnClick = CheckBoxDieRedScatterBagAllClick
            end
            object EditScatterBagItemsMinLevel: TSpinEditEx
              Left = 8
              Top = 171
              Width = 49
              Height = 21
              Hint = #25351#23450#31561#32423#20197#19979#20154#29289#27515#20129#19981#25481#33853#29289#21697
              MaxValue = 0
              MinValue = 0
              TabOrder = 5
              Value = 0
              OnChange = EditScatterBagItemsMinLevelChange
            end
            object EditDropUseItemsMaxCount: TSpinEditEx
              Left = 106
              Top = 191
              Width = 47
              Height = 21
              Hint = #25511#21046#38750#32418#21517#20154#29289#27515#20129#26102#26368#22810#25481#33853#20960#20214#36523#19978#29289#21697
              MaxValue = 0
              MinValue = 0
              TabOrder = 6
              Value = 0
              OnChange = EditDropUseItemsMaxCountChange
            end
            object chkKillByHumanDropJewelryBoxItem: TCheckBox
              Left = 8
              Top = 65
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#21035#20154#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#39318#39280#30418#30340#29289#21697#12290
              Caption = #34987#20154#29289#26432#27515#25481#39318#39280#30418#29289#21697
              TabOrder = 7
              OnClick = chkKillByHumanDropJewelryBoxItemClick
            end
            object chkKillByMonstDropJewelryBoxItem: TCheckBox
              Left = 8
              Top = 49
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#24618#29289#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#39318#39280#30418#30340#29289#21697#12290
              Caption = #34987#24618#29289#26432#27515#25481#39318#39280#30418#29289#21697
              TabOrder = 8
              OnClick = chkKillByMonstDropJewelryBoxItemClick
            end
            object chkKillByHumanDropGodBlessItem: TCheckBox
              Left = 8
              Top = 98
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#21035#20154#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#31070#20305#30418#30340#29289#21697#12290
              Caption = #34987#20154#29289#26432#27515#25481#31070#20305#30418#29289#21697
              TabOrder = 9
              OnClick = chkKillByHumanDropGodBlessItemClick
            end
            object chkKillByMonstDropGodBlessItem: TCheckBox
              Left = 8
              Top = 81
              Width = 160
              Height = 17
              Hint = #24403#20154#29289#34987#24618#29289#26432#27515#26102#20250#25353#25481#33853#26426#29575#25481#33853#31070#20305#30418#30340#29289#21697#12290
              Caption = #34987#24618#29289#26432#27515#25481#31070#20305#30418#29289#21697
              TabOrder = 10
              OnClick = chkKillByMonstDropGodBlessItemClick
            end
          end
          object GroupBox69: TGroupBox
            Left = 216
            Top = 8
            Width = 265
            Height = 145
            Caption = #25481#29289#21697#26426#29575
            TabOrder = 1
            object Label130: TLabel
              Left = 8
              Top = 18
              Width = 66
              Height = 12
              Caption = #25481#36523#19978#35013#22791':'
            end
            object Label131: TLabel
              Left = 8
              Top = 42
              Width = 66
              Height = 12
              Caption = #25481#32418#21517#35013#22791':'
            end
            object Label134: TLabel
              Left = 8
              Top = 66
              Width = 66
              Height = 12
              Caption = #25481#32972#21253#29289#21697':'
            end
            object Label223: TLabel
              Left = 8
              Top = 90
              Width = 66
              Height = 12
              Caption = #39318#39280#30418#29289#21697':'
            end
            object Label224: TLabel
              Left = 8
              Top = 114
              Width = 66
              Height = 12
              Caption = #31070#20305#30418#29289#21697':'
            end
            object ScrollBarDieDropUseItemRate: TScrollBar
              Left = 76
              Top = 16
              Width = 134
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 0
              OnChange = ScrollBarDieDropUseItemRateChange
            end
            object EditDieDropUseItemRate: TEdit
              Left = 216
              Top = 16
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 1
            end
            object ScrollBarDieRedDropUseItemRate: TScrollBar
              Left = 76
              Top = 40
              Width = 134
              Height = 17
              Hint = #32418#21517#20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              PageSize = 0
              TabOrder = 2
              OnChange = ScrollBarDieRedDropUseItemRateChange
            end
            object EditDieRedDropUseItemRate: TEdit
              Left = 216
              Top = 40
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 3
            end
            object ScrollBarDieScatterBagRate: TScrollBar
              Left = 76
              Top = 64
              Width = 134
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#32972#21253#20013#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 4
              OnChange = ScrollBarDieScatterBagRateChange
            end
            object EditDieScatterBagRate: TEdit
              Left = 216
              Top = 64
              Width = 41
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 5
            end
            object scrlbrJewelryBoxItem: TScrollBar
              Left = 76
              Top = 88
              Width = 134
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#32972#21253#20013#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 6
              OnChange = scrlbrJewelryBoxItemChange
            end
            object edtJewelryBoxItem: TEdit
              Left = 216
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
            object scrlbrGodBlessItem: TScrollBar
              Left = 76
              Top = 112
              Width = 134
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#32972#21253#20013#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 8
              OnChange = scrlbrGodBlessItemChange
            end
            object edtGodBlessItem: TEdit
              Left = 216
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
          object ButtonHumanDieSave: TButton
            Left = 448
            Top = 237
            Width = 65
            Height = 25
            Caption = #20445#23384'(&S)'
            TabOrder = 2
            OnClick = ButtonHumanDieSaveClick
          end
        end
        object TabSheet14: TTabSheet
          Caption = #29190#29575#35774#32622
          ImageIndex = 1
          object GroupBoxDieDropUseItemRate: TGroupBox
            Left = 4
            Top = 4
            Width = 513
            Height = 261
            TabOrder = 1
            object Label170: TLabel
              Left = 8
              Top = 22
              Width = 30
              Height = 12
              Caption = #34915#26381':'
            end
            object Label171: TLabel
              Left = 8
              Top = 46
              Width = 30
              Height = 12
              Caption = #27494#22120':'
            end
            object Label172: TLabel
              Left = 8
              Top = 70
              Width = 30
              Height = 12
              Caption = #21195#31456':'
            end
            object Label173: TLabel
              Left = 8
              Top = 94
              Width = 30
              Height = 12
              Caption = #39033#38142':'
            end
            object Label174: TLabel
              Left = 8
              Top = 118
              Width = 30
              Height = 12
              Caption = #22836#30420':'
            end
            object Label175: TLabel
              Left = 8
              Top = 142
              Width = 30
              Height = 12
              Caption = #24038#25163':'
            end
            object Label176: TLabel
              Left = 8
              Top = 166
              Width = 30
              Height = 12
              Caption = #21491#25163':'
            end
            object Label177: TLabel
              Left = 208
              Top = 166
              Width = 30
              Height = 12
              Caption = #26007#31520':'
            end
            object Label178: TLabel
              Left = 208
              Top = 142
              Width = 30
              Height = 12
              Caption = #23453#30707':'
            end
            object Label179: TLabel
              Left = 208
              Top = 118
              Width = 30
              Height = 12
              Caption = #38772#23376':'
            end
            object Label180: TLabel
              Left = 208
              Top = 94
              Width = 30
              Height = 12
              Caption = #33136#24102':'
            end
            object Label181: TLabel
              Left = 208
              Top = 70
              Width = 30
              Height = 12
              Caption = #27602#31526':'
            end
            object Label182: TLabel
              Left = 208
              Top = 46
              Width = 30
              Height = 12
              Caption = #21491#25106':'
            end
            object Label183: TLabel
              Left = 208
              Top = 22
              Width = 30
              Height = 12
              Caption = #24038#25106':'
            end
            object Label184: TLabel
              Left = 384
              Top = 24
              Width = 54
              Height = 12
              Caption = #32418#21517#20493#29575':'
            end
            object Label185: TLabel
              Left = 444
              Top = 44
              Width = 18
              Height = 12
              Caption = '/10'
            end
            object lbl10: TLabel
              Left = 8
              Top = 214
              Width = 30
              Height = 12
              Caption = #39532#29260':'
            end
            object lbl11: TLabel
              Left = 8
              Top = 238
              Width = 30
              Height = 12
              Caption = #30462#29260':'
            end
            object lbl12: TLabel
              Left = 184
              Top = 214
              Width = 54
              Height = 12
              Caption = #26102#35013#27494#22120':'
            end
            object lbl13: TLabel
              Left = 8
              Top = 190
              Width = 30
              Height = 12
              Caption = #20891#40723':'
            end
            object lbl14: TLabel
              Left = 184
              Top = 190
              Width = 54
              Height = 12
              Caption = #26102#35013#34915#26381':'
            end
            object ScrollBarDieDropUseItemRate0: TScrollBar
              Left = 40
              Top = 20
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 0
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate0: TEdit
              Left = 136
              Top = 20
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 1
            end
            object ScrollBarDieDropUseItemRate1: TScrollBar
              Tag = 1
              Left = 40
              Top = 44
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 5
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate1: TEdit
              Tag = 1
              Left = 136
              Top = 44
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 6
            end
            object ScrollBarDieDropUseItemRate2: TScrollBar
              Tag = 2
              Left = 40
              Top = 68
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 9
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate2: TEdit
              Tag = 2
              Left = 136
              Top = 68
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 10
            end
            object ScrollBarDieDropUseItemRate3: TScrollBar
              Tag = 3
              Left = 40
              Top = 92
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 13
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate3: TEdit
              Tag = 3
              Left = 136
              Top = 92
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeMode = imHanguel
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 14
            end
            object ScrollBarDieDropUseItemRate4: TScrollBar
              Tag = 4
              Left = 40
              Top = 116
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 17
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate4: TEdit
              Tag = 4
              Left = 136
              Top = 116
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 18
            end
            object ScrollBarDieDropUseItemRate5: TScrollBar
              Tag = 5
              Left = 40
              Top = 140
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 21
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate5: TEdit
              Tag = 5
              Left = 136
              Top = 140
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 22
            end
            object ScrollBarDieDropUseItemRate6: TScrollBar
              Tag = 6
              Left = 40
              Top = 164
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 26
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate6: TEdit
              Tag = 6
              Left = 136
              Top = 164
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 27
            end
            object ScrollBarDieDropUseItemRate13: TScrollBar
              Tag = 13
              Left = 240
              Top = 164
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 28
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate13: TEdit
              Tag = 13
              Left = 336
              Top = 164
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 29
            end
            object ScrollBarDieDropUseItemRate12: TScrollBar
              Tag = 12
              Left = 240
              Top = 140
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 23
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate12: TEdit
              Tag = 12
              Left = 336
              Top = 140
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 24
            end
            object edtDieDropUseItemRate11: TEdit
              Tag = 11
              Left = 336
              Top = 116
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 20
            end
            object edtDieDropUseItemRate10: TEdit
              Tag = 10
              Left = 336
              Top = 92
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 16
            end
            object edtDieDropUseItemRate9: TEdit
              Tag = 9
              Left = 336
              Top = 68
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 12
            end
            object edtDieDropUseItemRate8: TEdit
              Tag = 8
              Left = 336
              Top = 44
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 8
            end
            object edtDieDropUseItemRate7: TEdit
              Tag = 7
              Left = 336
              Top = 20
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 3
            end
            object ScrollBarDieDropUseItemRate7: TScrollBar
              Tag = 7
              Left = 240
              Top = 20
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 2
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object ScrollBarDieDropUseItemRate8: TScrollBar
              Tag = 8
              Left = 240
              Top = 44
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 7
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object ScrollBarDieDropUseItemRate9: TScrollBar
              Tag = 9
              Left = 240
              Top = 68
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 11
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object ScrollBarDieDropUseItemRate10: TScrollBar
              Tag = 10
              Left = 240
              Top = 92
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 15
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object ScrollBarDieDropUseItemRate11: TScrollBar
              Tag = 11
              Left = 240
              Top = 116
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 19
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object EditDieRedDropUseItemOneRate: TSpinEditEx
              Left = 384
              Top = 40
              Width = 57
              Height = 21
              MaxValue = 0
              MinValue = 0
              TabOrder = 4
              Value = 0
              OnChange = EditDieRedDropUseItemOneRateChange
            end
            object ButtonDieDropUseItemSave: TButton
              Left = 400
              Top = 160
              Width = 75
              Height = 25
              Caption = #20445#23384'(&S)'
              TabOrder = 25
              OnClick = ButtonDieDropUseItemSaveClick
            end
            object ScrollBarDieDropUseItemRate15: TScrollBar
              Tag = 15
              Left = 40
              Top = 212
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 34
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate15: TEdit
              Tag = 5
              Left = 136
              Top = 212
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 35
            end
            object ScrollBarDieDropUseItemRate16: TScrollBar
              Tag = 16
              Left = 40
              Top = 236
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 38
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate16: TEdit
              Tag = 6
              Left = 136
              Top = 236
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 39
            end
            object ScrollBarDieDropUseItemRate18: TScrollBar
              Tag = 18
              Left = 240
              Top = 212
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 36
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate18: TEdit
              Tag = 13
              Left = 336
              Top = 212
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 37
            end
            object ScrollBarDieDropUseItemRate14: TScrollBar
              Tag = 14
              Left = 40
              Top = 188
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 30
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate14: TEdit
              Tag = 12
              Left = 136
              Top = 188
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 31
            end
            object ScrollBarDieDropUseItemRate17: TScrollBar
              Tag = 17
              Left = 240
              Top = 188
              Width = 89
              Height = 17
              Hint = #20154#29289#27515#20129#25481#33853#36523#19978#25140#30340#29289#21697#26426#29575#65292#35774#32622#30340#25968#23383#36234#23567#65292#26426#29575#36234#22823#12290
              Max = 500
              PageSize = 0
              TabOrder = 32
              OnChange = ScrollBarDieDropUseItemRate0Change
            end
            object edtDieDropUseItemRate17: TEdit
              Tag = 13
              Left = 336
              Top = 188
              Width = 35
              Height = 18
              Ctl3D = False
              Enabled = False
              ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
              ParentCtl3D = False
              ReadOnly = True
              TabOrder = 33
            end
          end
          object CheckBoxDropUseItem: TCheckBox
            Left = 12
            Top = 2
            Width = 145
            Height = 17
            Caption = #24320#21551#21333#29420#35013#22791#26426#29575#25481#33853
            TabOrder = 0
            OnClick = CheckBoxDropUseItemClick
          end
        end
      end
    end
  end
end
