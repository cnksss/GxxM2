object FrmConfigClient: TFrmConfigClient
  Left = 356
  Top = 179
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #23458#25143#31471#25511#21046
  ClientHeight = 537
  ClientWidth = 627
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 12
  object ClientPageControl: TPageControl
    Left = 0
    Top = 0
    Width = 627
    Height = 537
    ActivePage = tsOption2
    Align = alClient
    TabOrder = 0
    OnChanging = ClientPageControlChanging
    ExplicitWidth = 623
    ExplicitHeight = 536
    object TabSheet1: TTabSheet
      Caption = #30028#38754#26174#31034
      object GroupBox75: TGroupBox
        Left = 12
        Top = 10
        Width = 595
        Height = 449
        TabOrder = 0
        object Label5: TLabel
          Left = 9
          Top = 414
          Width = 66
          Height = 12
          Caption = #27983#35272#22120#22320#22336':'
        end
        object CheckBoxControlHelpButton: TCheckBox
          Left = 9
          Top = 13
          Width = 147
          Height = 18
          Caption = #26174#31034#25171#24320#30028#38754#24110#21161#25353#38062
          TabOrder = 0
          OnClick = CheckBoxControlHelpButtonClick
        end
        object CheckBoxRankButton: TCheckBox
          Left = 9
          Top = 30
          Width = 147
          Height = 18
          Caption = #26174#31034#25171#24320#25490#34892#27036#25353#38062
          TabOrder = 3
          OnClick = CheckBoxRankButtonClick
        end
        object CheckBoxWhisperButton: TCheckBox
          Left = 9
          Top = 48
          Width = 156
          Height = 19
          Caption = #26174#31034#25171#24320#31169#32842#20449#24687#25353#38062
          TabOrder = 5
          OnClick = CheckBoxWhisperButtonClick
        end
        object CheckBoxActionLogButton: TCheckBox
          Left = 9
          Top = 138
          Width = 147
          Height = 17
          Caption = #26174#31034#25171#24320#27963#21160#26085#35760#25353#38062
          TabOrder = 12
          OnClick = CheckBoxActionLogButtonClick
        end
        object CheckBoxMissionButton: TCheckBox
          Left = 9
          Top = 101
          Width = 147
          Height = 19
          Caption = #26174#31034#25171#24320#20219#21153#26085#35760#25353#38062
          TabOrder = 10
          OnClick = CheckBoxMissionButtonClick
        end
        object CheckBoxWebButton: TCheckBox
          Left = 9
          Top = 190
          Width = 147
          Height = 18
          Caption = #26174#31034#25171#24320#27983#35272#22120#25353#38062
          TabOrder = 15
          OnClick = CheckBoxWebButtonClick
        end
        object CheckBoxOpenShopButton: TCheckBox
          Left = 9
          Top = 84
          Width = 147
          Height = 18
          Caption = #26174#31034#25171#24320#21830#38138#25353#38062
          TabOrder = 9
          OnClick = CheckBoxOpenShopButtonClick
        end
        object CheckBoxUserShopButton: TCheckBox
          Left = 9
          Top = 120
          Width = 156
          Height = 18
          Caption = #26174#31034#25171#24320#20010#20154#24215#38138#25353#38062
          TabOrder = 11
          OnClick = CheckBoxUserShopButtonClick
        end
        object CheckBoxFriendButton: TCheckBox
          Left = 9
          Top = 66
          Width = 130
          Height = 18
          Caption = #26174#31034#25171#24320#22909#21451#25353#38062
          TabOrder = 7
          OnClick = CheckBoxFriendButtonClick
        end
        object EditHomePage: TEdit
          Left = 86
          Top = 409
          Width = 501
          Height = 20
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 27
          Text = 'http://www.Gxxm2.com'
          OnChange = EditHomePageChange
        end
        object CheckBoxOpenHeroButton: TCheckBox
          Left = 9
          Top = 154
          Width = 130
          Height = 18
          Caption = #26174#31034#33521#38596#30456#20851#25353#38062
          TabOrder = 13
          OnClick = CheckBoxOpenHeroButtonClick
        end
        object CheckBoxShowMerchantDlgHelp: TCheckBox
          Left = 9
          Top = 172
          Width = 156
          Height = 19
          Caption = #26174#31034'NPC'#23545#35805#26694#65311#25353#38062
          TabOrder = 14
          OnClick = CheckBoxShowMerchantDlgHelpClick
        end
        object GroupBox3: TGroupBox
          Left = 345
          Top = 17
          Width = 242
          Height = 355
          Caption = #29305#27530#21629#20196
          TabOrder = 2
          object Label6: TLabel
            Left = 9
            Top = 258
            Width = 54
            Height = 12
            Caption = #26174#31034#21517#31216':'
          end
          object Label7: TLabel
            Left = 9
            Top = 284
            Width = 54
            Height = 12
            Caption = #29992#25143#21629#20196':'
          end
          object ListViewSpecialCmd: TListView
            Left = 9
            Top = 17
            Width = 225
            Height = 234
            Columns = <
              item
                Caption = #26174#31034#21517#31216
                Width = 108
              end
              item
                Caption = #29992#25143#21629#20196
                Width = 108
              end>
            GridLines = True
            ReadOnly = True
            TabOrder = 0
            ViewStyle = vsReport
            OnClick = ListViewSpecialCmdClick
          end
          object EditSpecialCmdCaption: TEdit
            Left = 69
            Top = 258
            Width = 156
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 1
          end
          object EditSpecialCmd: TEdit
            Left = 69
            Top = 284
            Width = 156
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
            TabOrder = 2
          end
          object ButtonSpecialCmdAdd: TButton
            Left = 9
            Top = 319
            Width = 53
            Height = 27
            Caption = #22686#21152
            TabOrder = 3
            OnClick = ButtonSpecialCmdAddClick
          end
          object ButtonSpecialCmdDel: TButton
            Left = 65
            Top = 319
            Width = 53
            Height = 27
            Caption = #21024#38500
            TabOrder = 4
            OnClick = ButtonSpecialCmdDelClick
          end
          object ButtonSpecialCmdChg: TButton
            Left = 121
            Top = 319
            Width = 53
            Height = 27
            Caption = #20462#25913
            TabOrder = 5
            OnClick = ButtonSpecialCmdChgClick
          end
          object ButtonSpecialCmdSave: TButton
            Left = 177
            Top = 319
            Width = 53
            Height = 27
            Caption = #20445#23384
            TabOrder = 6
            OnClick = ButtonSpecialCmdSaveClick
          end
        end
        object CheckBoxDBotFunc1: TCheckBox
          Left = 9
          Top = 296
          Width = 147
          Height = 18
          Caption = #26174#31034#36807#28388#20844#32842#20449#24687#25353#38062
          TabOrder = 21
          OnClick = CheckBoxDBotFunc1Click
        end
        object CheckBoxDBotFunc2: TCheckBox
          Tag = 1
          Left = 9
          Top = 313
          Width = 173
          Height = 19
          Caption = #26174#31034#36807#28388#40644#33394#23383#20307#21898#35805#25353#38062
          TabOrder = 22
          OnClick = CheckBoxDBotFunc1Click
        end
        object CheckBoxDBotFunc3: TCheckBox
          Tag = 2
          Left = 9
          Top = 332
          Width = 147
          Height = 18
          Caption = #26174#31034#36807#28388#31169#32842#20449#24687#25353#38062
          TabOrder = 23
          OnClick = CheckBoxDBotFunc1Click
        end
        object CheckBoxDBotFunc4: TCheckBox
          Tag = 3
          Left = 9
          Top = 349
          Width = 147
          Height = 18
          Caption = #26174#31034#36807#28388#34892#20250#21898#35805#25353#38062
          TabOrder = 24
          OnClick = CheckBoxDBotFunc1Click
        end
        object CheckBoxDBotFunc6: TCheckBox
          Tag = 5
          Left = 9
          Top = 384
          Width = 121
          Height = 19
          Caption = #26174#31034#29305#27530#21629#20196#25353#38062
          TabOrder = 26
          OnClick = CheckBoxDBotFunc1Click
        end
        object CheckBoxDBotFunc5: TCheckBox
          Tag = 4
          Left = 9
          Top = 367
          Width = 139
          Height = 19
          Caption = #26174#31034#33258#21160#21898#35805#21151#33021#38062
          TabOrder = 25
          OnClick = CheckBoxDBotFunc1Click
        end
        object CheckBoxChallengeButton: TCheckBox
          Left = 9
          Top = 208
          Width = 104
          Height = 18
          Caption = #26174#31034#25361#25112#25353#38062
          TabOrder = 16
          OnClick = CheckBoxChallengeButtonClick
        end
        object chkGemUpgrade: TCheckBox
          Left = 9
          Top = 225
          Width = 104
          Height = 18
          Hint = #20851#38381#27492#39033#28216#25103#20013#32972#21253#26639#23558#19981#33021#36827#34892#35013#22791#23453#30707#21319#32423' '
          Caption = #24320#21551#23453#30707#21319#32423
          ParentShowHint = False
          ShowHint = True
          TabOrder = 17
          OnClick = chkGemUpgradeClick
        end
        object chkShowGuildName: TCheckBox
          Left = 9
          Top = 243
          Width = 130
          Height = 19
          Hint = #20154#29289#21517#31216#26174#31034#34892#20250#20449#24687','#22914#19981#35774#32622','#21017#21482#13#26377#27801#24052#20811#34892#20250#22312#20154#29289#21517#31216#26174#31034','#20854#23427#34892#13#20250'('#38500#25915#22478#21306#22495#22806')'#19981#22312#20154#29289#21517#31216#26174#31034#20449#24687
          Caption = #21517#23383#26174#31034#34892#20250#20449#24687
          ParentShowHint = False
          ShowHint = True
          TabOrder = 18
          OnClick = chkShowGuildNameClick
        end
        object chkShopGuiCanMove: TCheckBox
          Left = 9
          Top = 261
          Width = 130
          Height = 18
          Hint = #24320#21551#21518#21830#38138#30028#38754#23558#21487#20197#33258#30001#31227#21160','#30028#38754#40664#35748#23621#20013#26174#31034
          Caption = #21830#38138#30028#38754#21487#31227#21160
          ParentShowHint = False
          ShowHint = True
          TabOrder = 19
          OnClick = chkShopGuiCanMoveClick
        end
        object chkNPCGuiCanMove: TCheckBox
          Left = 9
          Top = 278
          Width = 130
          Height = 18
          Hint = #21246#36873#21518'npc'#23545#35805#26694#20197#21450#33521#38596#22270#26631#21487#33258#30001#31227#21160#13#10#13#10#21246#36873#21518#33258#23450#20041#22823#23545#35805#26694#21629#20196#20013#31105#27490#31227#21160#21442#25968#23558#26080#25928
          Caption = 'NPC'#30028#38754#21487#31227#21160
          ParentShowHint = False
          ShowHint = True
          TabOrder = 20
          OnClick = chkNPCGuiCanMoveClick
        end
        object chkShowGlory: TCheckBox
          Tag = 5
          Left = 181
          Top = 13
          Width = 122
          Height = 18
          Caption = #26174#31034#33635#35465#20449#24687
          TabOrder = 1
          OnClick = chkShowGloryClick
        end
        object chkShowHorseButton: TCheckBox
          Left = 181
          Top = 30
          Width = 130
          Height = 18
          Caption = #26174#31034#19978#39532'/'#19979#39532#25353#38062
          TabOrder = 4
          OnClick = chkShowHorseButtonClick
        end
        object chkShowDeputyHeroButton: TCheckBox
          Left = 181
          Top = 48
          Width = 130
          Height = 19
          Caption = #26174#31034#21103#23558#33521#38596#25353#38062
          TabOrder = 6
          OnClick = chkShowDeputyHeroButtonClick
        end
        object chkShowBagArrange: TCheckBox
          Left = 181
          Top = 68
          Width = 130
          Height = 18
          Caption = #26174#31034#32972#21253#25972#29702#25353#38062
          TabOrder = 8
          OnClick = chkShowBagArrangeClick
        end
      end
      object ButtonPrguseSave: TButton
        Left = 534
        Top = 463
        Width = 70
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonPrguseSaveClick
      end
    end
    object TabSheet2: TTabSheet
      Caption = #20869#25346#25511#21046
      object pgc1: TPageControl
        Left = 0
        Top = 0
        Width = 624
        Height = 463
        ActivePage = ts1
        Align = alCustom
        TabOrder = 0
        object ts1: TTabSheet
          Caption = #20869#25346#25511#21046
          ImageIndex = 1
          object RzCheckGroupClientConfig: TRzCheckGroup
            Left = 10
            Top = 94
            Width = 595
            Height = 339
            Caption = ''
            Color = 15987699
            Columns = 5
            GroupStyle = gsStandard
            Items.Strings = (
              #26174#31034#34880#26465
              #25968#23383#26174#34880
              #32844#19994#31561#32423
              #32463#39564#36807#28388
              #39030#37096#20449#24687
              #26174#31034#20154#21517
              #21482#26174#20154#21517
              #25968#23383#39128#34880
              #33258#21160#25441#21462
              #25915#20987#19981#21345
              #27494#22120#31616#35013
              #31283#22914#27888#23665
              #20813#36127#37325
              #39764#27861#38145#23450
              #20840#37096#25342#21462
              #33258#21160#25918#33647
              #33258#21160#20851#32452
              #25345#20037#35686#21578
              #20813'Shift'#38190
              #38544#34255#23608#20307
              #38544#34255#32709#33152#25928#26524
              #38544#34255#27494#22120#25928#26524
              #26174#31034#22320#22270#26631#35782
              #20154#29289#39640#20142#26174#34880
              #33258#21160#38544#36523
              #33258#21160#26029#31354#26025
              #33258#21160#24320#30462
              #25509#36817#24320#30462
              #20992#20992#21050#26432
              #38548#20301#21050#26432
              #36208#20301#21050#26432
              #26234#33021#21322#26376
              #33258#21160#28872#28779
              #36880#26085#21073#27861
              #21452#40857#26025
              #40857#24433#21073#27861
              #32972#26223#38899#20048
              #37325#22797#38899#20048
              #26174#31034#24618#21517
              #20869#21151#40644#26465
              #38450#27490#30707#21270
              #25163#21160#20912#21638#21742
              #25163#21160#29190#35010#28779#28976
              #30142#20809#30005#24433#38145#23450#30446#26631
              #25163#21160#27969#26143#28779#38632
              #32418#32511#27602#20114#25442
              #20813#21161#36305
              #33258#21160#24320#22825#26025
              #33521#38596#25345#32493#24320#30462
              #21103#33521#38596#25345#32493#24320#30462
              #20027#23558#33521#38596#33647#21697
              #21103#23558#33521#38596#33647#21697
              #23631#24149#38663#21160
              'NPC'#26174#21517
              'NPC'#26174#34880
              'Shift'#24320#20851
              #38544#34255#31216#21495
              #33258#21160#20957#32858#25216#33021
              #31105#27490#25289#21160#32842#22825#26694
              #35013#22791#23545#27604
              #38899#37327
              #25345#32493#25366#21462
              #31105#27490#20132#26131
              #24494#31471#29366#24577#26174#31034
              #24618#29289#31616#35013
              #25163#21160#21313#27493#19968#26432
              #34915#26381#31616#35013
              #38544#34255#29305#25928
              #33258#21160#21512#20987
              #21512#20987#19981#25171#24618
              #32972#21253#23545#27604
              #25163#21160#25511#21046#22320#29425#28779
              #34880#37327#21333#20301
              #24038#20391#26174#31034#32452#38431#20449#24687
              #23453#23453#31616#35013
              #33258#23450#20041#25216#33021'1'
              #33258#23450#20041#25216#33021'2'
              #33258#23450#20041#25216#33021'3'
              #33258#23450#20041#25216#33021'4'
              #33258#23450#20041#25216#33021'5'
              #33258#23450#20041#25216#33021'6'
              #33258#23450#20041#25216#33021'7'
              #33258#23450#20041#25216#33021'8'
              #26497#21697#29305#25928
              #33258#21160#36830#20987
              #38544#34255#39030#25140#33457#32718
              #38544#34255#24618#29289#39030#25140
              #28779#22681#28129#21270
              #33258#21160#32469#34892)
            StartXPos = 9
            StartYPos = 0
            TabOrder = 2
            OnChange = RzCheckGroupClientConfigChange
            CheckStates = (
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0
              0)
          end
          object RzCheckGroupClientTabSheet: TRzCheckGroup
            Left = 10
            Top = 58
            Width = 595
            Height = 35
            Caption = ''
            Color = 15987699
            Columns = 12
            GroupStyle = gsStandard
            Items.Strings = (
              #22522#26412
              #29289#21697
              #20445#25252
              #33647#21697
              #25216#33021
              #25353#38190
              #25112#26007
              #25346#26426
              #24110#21161)
            SpaceEvenly = True
            StartXPos = 9
            StartYPos = -4
            TabOrder = 1
            OnChange = RzCheckGroupClientTabSheetChange
            CheckStates = (
              0
              0
              0
              0
              0
              0
              0
              0
              0)
          end
          object GroupBox2: TGroupBox
            Left = 10
            Top = 1
            Width = 595
            Height = 57
            Caption = #22522#26412#35774#32622
            TabOrder = 0
            object CheckBoxStartGameAuxiliary: TCheckBox
              Left = 9
              Top = 16
              Width = 78
              Height = 18
              Hint = #24320#21551#21518#20869#25346#21151#33021#25165#21487#20197#27491#24120#20351#29992#65292#20851#38381#21518#20869#25346#25152#26377#21151#33021#22833#25928
              Caption = #21551#29992#20869#25346
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              OnClick = CheckBoxStartGameAuxiliaryClick
            end
            object CheckBoxCanOpenGameConfigDlg: TCheckBox
              Left = 9
              Top = 33
              Width = 130
              Height = 19
              Hint = #26159#21542#21487#20197#21628#20986#20869#25346#30340#30028#38754
              Caption = #20801#35768#21628#20986#20869#25346#30028#38754
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              OnClick = CheckBoxCanOpenGameConfigDlgClick
            end
            object CheckBoxNotCanUseClientConfig: TCheckBox
              Left = 172
              Top = 16
              Width = 148
              Height = 18
              Hint = 
                #21246#19978#21518#65292#29992#25143#20869#25346#20013#30340#25152#26377#36873#39033#37117#22833#25928#65292#21482#26681#25454'M2'#30340#20869#25346#35774#32622#21442#25968#12290#27604#22914#26174#31034#34880#26465#65292#22914#26524'M2'#30340#26174#31034#34880#26465#21246#19978#20102#65292#20869#25346#20013#26174#31034#34880#26465#29992#25143#21246#19981#21246#37117 +
                #20250#26174#31034#34880#26465#12290
              Caption = #31105#29992#23458#25143#31471#20869#25346#35774#32622
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              OnClick = CheckBoxNotCanUseClientConfigClick
            end
            object RadioButtonPlugIn1: TRadioButton
              Left = 345
              Top = 16
              Width = 78
              Height = 18
              Caption = #40664#35748#20869#25346
              TabOrder = 2
              OnClick = RadioButtonPlugIn1Click
            end
            object RadioButtonPlugIn2: TRadioButton
              Left = 345
              Top = 33
              Width = 104
              Height = 19
              Caption = #20223#21450#26102#38632#20869#25346
              TabOrder = 5
              OnClick = RadioButtonPlugIn2Click
            end
            object chkGreenHintNewStyle: TCheckBox
              Left = 172
              Top = 33
              Width = 148
              Height = 19
              Hint = #21246#19978#21518#65292#32511#23383#20449#24687#26174#31034#38450#24481#12289#39764#38450#12289#25915#20987#12289#39764#27861#12289#36947#26415#23646#24615
              Caption = #39030#37096#32511#23383#20351#29992#26032#26679#24335
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              OnClick = chkGreenHintNewStyleClick
            end
          end
        end
        object ts2: TTabSheet
          Caption = #36895#24230#25511#21046
          ImageIndex = 1
          object lbl5: TLabel
            Left = 13
            Top = 344
            Width = 306
            Height = 72
            Caption = 
              #22914#21457#29616#31227#21160#26377#23567#24494#23567#21345#39039#65292#21487#20197#23558#31227#21160#38388#38548#35774#32622#20026#36127#20540#13#10#13#10#33509#26080'ChangeSpeed'#35843#25972#20154#29289#30340#31227#21160#36895#24230#65292#21487#35774#32622#20026#22914#19979#20540#65306#13#10#31227#21160#36895 +
              #24230#65306'-360'#65307#31227#21160#38388#38548#65306'400'#13#10#13#10#35843#25972#31227#21160#38388#38548#21518#65292#23454#38469#31227#21160#36895#24230#20197#32593#20851#30340#35843#35797#36895#24230#20026#20934
            Font.Charset = GB2312_CHARSET
            Font.Color = clBlue
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox1: TGroupBox
            Left = 9
            Top = 22
            Width = 595
            Height = 143
            TabOrder = 0
            object Label1: TLabel
              Left = 9
              Top = 13
              Width = 144
              Height = 24
              Caption = #31227#21160#36895#24230#12290#36208#36335#25110#36305#27493#26102#65292#13#21160#20316#24320#22987#21040#32467#26463#30340#36895#24230
            end
            object Label2: TLabel
              Left = 215
              Top = 13
              Width = 120
              Height = 24
              Caption = #25915#20987#36895#24230#12290#25915#20987#26102#65292#13#21160#20316#24320#22987#21040#32467#26463#30340#36895#24230
            end
            object Label3: TLabel
              Left = 405
              Top = 13
              Width = 156
              Height = 24
              Caption = #39764#27861#36895#24230#12290#20351#29992#39764#27861#25915#20987#26102#65292#13#21160#20316#24320#22987#21040#32467#26463#30340#36895#24230
            end
            object lbl9: TLabel
              Left = 219
              Top = 116
              Width = 60
              Height = 12
              Caption = #25915#20987#38388#38548#65306
            end
            object lbl8: TLabel
              Left = 418
              Top = 116
              Width = 60
              Height = 12
              Caption = #39764#27861#38388#38548#65306
            end
            object Label114: TLabel
              Left = 20
              Top = 117
              Width = 60
              Height = 12
              Caption = #31227#21160#38388#38548#65306
            end
            object TrackBarMoveSpeed: TTrackBar
              Left = 17
              Top = 52
              Width = 131
              Height = 27
              Max = 1000
              Min = -1000
              ParentShowHint = False
              PageSize = 100
              Frequency = 100
              Position = 80
              ShowHint = True
              TabOrder = 0
              OnChange = TrackBarMoveSpeedChange
            end
            object RzSpinnerMoveSpeed: TRzSpinner
              Left = 26
              Top = 82
              Width = 113
              Height = 22
              Max = 1000
              Min = -1000
              Value = -200
              OnChange = RzSpinnerMoveSpeedChange
              TabOrder = 3
            end
            object TrackBarAttackSpeed: TTrackBar
              Left = 215
              Top = 52
              Width = 131
              Height = 27
              Max = 1000
              Min = -1000
              ParentShowHint = False
              PageSize = 100
              Frequency = 100
              Position = 60
              ShowHint = True
              TabOrder = 1
              OnChange = TrackBarAttackSpeedChange
            end
            object RzSpinnerAttackSpeed: TRzSpinner
              Left = 224
              Top = 82
              Width = 113
              Height = 22
              Max = 1000
              Min = -1000
              Value = -100
              OnChange = RzSpinnerAttackSpeedChange
              TabOrder = 4
            end
            object TrackBarSpellSpeed: TTrackBar
              Left = 414
              Top = 52
              Width = 130
              Height = 27
              Max = 1000
              Min = -1000
              ParentShowHint = False
              PageSize = 100
              Frequency = 100
              Position = 80
              ShowHint = True
              TabOrder = 2
              OnChange = TrackBarSpellSpeedChange
            end
            object RzSpinnerSpellSpeed: TRzSpinner
              Left = 422
              Top = 82
              Width = 113
              Height = 22
              Max = 1000
              Min = -1000
              OnChange = RzSpinnerSpellSpeedChange
              ParentShowHint = False
              ShowHint = False
              TabOrder = 5
            end
            object seHitFrameTime: TSpinEditEx
              Left = 279
              Top = 112
              Width = 61
              Height = 21
              Hint = 
                #23458#25143#31471#40664#35748#25915#20987#38388#38548#25511#21046','#25968#23383#36234#23567#36895#24230#36234#24555','#19981#21487#23567#20110#24341#25806#8594#21442#25968#35774#32622#8594#28216#25103#36895#24230#8594#25915#20987#38388#38548#25511#21046#25110#32593#20851#25915#20987#38480#21046#21442#25968','#21542#21017#25552#31034#24182#25353#36229#36895#22788#29702 +
                '|'#23458#25143#31471#40664#35748#25915#20987#38388#38548#25511#21046','#25968#23383#36234#23567#36895#24230#36234#24555','#19981#21487#23567#20110#24341#25806#8594#21442#25968#35774#32622#8594#28216#25103#36895#24230#8594#25915#20987#38388#38548#25511#21046#25110#32593#20851#25915#20987#38480#21046#21442#25968','#21542#21017#25552#31034#24182#25353#36229#36895#22788 +
                #29702
              MaxValue = 2000
              MinValue = 60
              ParentShowHint = False
              ShowHint = True
              TabOrder = 7
              Value = 60
              OnChange = seHitFrameTimeChange
            end
            object seMagicHitFrameTime: TSpinEditEx
              Left = 478
              Top = 112
              Width = 60
              Height = 21
              Hint = #23458#25143#31471#40664#35748#40664#35748#39764#27861#25511#21046','#25968#23383#36234#23567#36895#24230#36234#24555','#19981#21487#23567#20110#24341#25806#8594#21442#25968#35774#32622#8594#28216#25103#38388#38548#8594#39764#27861#38388#38548#25511#21046#30340#32593#20851#39764#27861#38480#21046#21442#25968','#21542#21017#25552#31034#24182#25353#36229#36895#22788#29702
              MaxValue = 2000
              MinValue = 180
              ParentShowHint = False
              ShowHint = True
              TabOrder = 8
              Value = 180
              OnChange = seMagicHitFrameTimeChange
            end
            object seMoveFrameTime: TSpinEditEx
              Left = 80
              Top = 112
              Width = 61
              Height = 21
              Hint = 
                #23458#25143#31471#40664#35748#31227#21160#38388#38548#25511#21046','#25968#23383#36234#23567#36895#24230#36234#24555','#19981#21487#23567#20110#24341#25806#8594#21442#25968#35774#32622#8594#28216#25103#36895#24230#8594#36305#27493'/'#36208#36335#38388#38548#25511#21046#25110#32593#20851#25915#20987#38480#21046#21442#25968','#21542#21017#25552#31034#24182#25353#36229 +
                #36895#22788#29702'|'#23458#25143#31471#40664#35748#25915#20987#38388#38548#25511#21046','#25968#23383#36234#23567#36895#24230#36234#24555','#19981#21487#23567#20110#24341#25806#8594#21442#25968#35774#32622#8594#28216#25103#36895#24230#8594#25915#20987#38388#38548#25511#21046#25110#32593#20851#25915#20987#38480#21046#21442#25968','#21542#21017#25552#31034#24182#25353 +
                #36229#36895#22788#29702
              MaxValue = 2000
              MinValue = 180
              ParentShowHint = False
              ShowHint = True
              TabOrder = 6
              Value = 180
              OnChange = seMoveFrameTimeChange
            end
          end
          object grp4: TGroupBox
            Left = 9
            Top = 168
            Width = 595
            Height = 46
            Caption = #20854#20182#25511#21046
            TabOrder = 1
            object Label78: TLabel
              Left = 9
              Top = 19
              Width = 96
              Height = 12
              Caption = #20869#25346#26368#23567#21507#33647#38388#38548
            end
            object Label79: TLabel
              Left = 196
              Top = 19
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label71: TLabel
              Left = 258
              Top = 19
              Width = 48
              Height = 12
              Caption = #20869#25346#25441#36215
            end
            object sePluginMinEatItemTime: TSpinEditEx
              Left = 115
              Top = 15
              Width = 78
              Height = 21
              MaxValue = 900000000
              MinValue = 10
              TabOrder = 1
              Value = 500
              OnChange = sePluginMinEatItemTimeChange
            end
            object sePluginPickupTime: TSpinEditEx
              Left = 318
              Top = 14
              Width = 60
              Height = 21
              Hint = #20869#25346#33258#21160#25441#36215#29289#21697#26102#38388#38388#38548
              MaxValue = 0
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 0
              Value = 900
              OnChange = sePluginPickupTimeChange
            end
          end
          object GroupBox22: TGroupBox
            Left = 9
            Top = 218
            Width = 595
            Height = 95
            Caption = #35013#22791#36895#24230
            TabOrder = 2
            object Label100: TLabel
              Left = 9
              Top = 19
              Width = 96
              Height = 12
              Caption = #25915#20987#36895#24230'+1'#20943#38388#38548
            end
            object lbl17: TLabel
              Left = 196
              Top = 19
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label110: TLabel
              Left = 9
              Top = 43
              Width = 96
              Height = 12
              Caption = #31227#21160#36895#24230'+1'#20943#38388#38548
            end
            object Label111: TLabel
              Left = 196
              Top = 43
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object Label112: TLabel
              Left = 9
              Top = 67
              Width = 96
              Height = 12
              Caption = #39764#27861#36895#24230'+1'#20943#38388#38548
            end
            object Label113: TLabel
              Left = 196
              Top = 67
              Width = 24
              Height = 12
              Caption = #27627#31186
            end
            object lbl4: TLabel
              Left = 232
              Top = 67
              Width = 258
              Height = 12
              Caption = #27599#20010#25216#33021#25968#25454#24211#30340'Delay'#23383#27573#19981#21516#65292#36895#24230#20063#20250#19981#21516
              Font.Charset = GB2312_CHARSET
              Font.Color = clBlue
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
            end
            object seIncSpeedDecInterval: TSpinEditEx
              Left = 115
              Top = 14
              Width = 76
              Height = 21
              Hint = #25511#21046#35013#22791#21152#36895#24773#20917#65292#25968#23383#36234#22823#36234#23485#65292#36234#23567#19987#36234#20005#13#22914#26524#26159#21464#24577#29256#65292#26368#22909#19981#36229#36807'10'#65292#36825#26679#35013#22791#25165#26377#21152#36895#31354#38388#12290
              MaxValue = 50
              MinValue = 1
              TabOrder = 0
              Value = 50
              OnChange = seIncSpeedDecIntervalChange
            end
            object seIncMoveSpeedDecInterval: TSpinEditEx
              Left = 115
              Top = 38
              Width = 76
              Height = 21
              Hint = #25511#21046#35013#22791#21152#36895#24773#20917#65292#25968#23383#36234#22823#36234#23485#65292#36234#23567#19987#36234#20005#13#22914#26524#26159#21464#24577#29256#65292#26368#22909#19981#36229#36807'10'#65292#36825#26679#35013#22791#25165#26377#21152#36895#31354#38388#12290
              MaxValue = 50
              MinValue = 1
              TabOrder = 1
              Value = 50
              OnChange = seIncMoveSpeedDecIntervalChange
            end
            object seIncSpellSpeedDecInterval: TSpinEditEx
              Left = 115
              Top = 62
              Width = 76
              Height = 21
              Hint = #25511#21046#35013#22791#21152#36895#24773#20917#65292#25968#23383#36234#22823#36234#23485#65292#36234#23567#19987#36234#20005#13#22914#26524#26159#21464#24577#29256#65292#26368#22909#19981#36229#36807'10'#65292#36825#26679#35013#22791#25165#26377#21152#36895#31354#38388#12290
              MaxValue = 50
              MinValue = 1
              TabOrder = 2
              Value = 50
              OnChange = seIncSpellSpeedDecIntervalChange
            end
          end
        end
      end
      object ButtonGameAuxiliarySave: TButton
        Left = 539
        Top = 468
        Width = 70
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonGameAuxiliarySaveClick
      end
    end
    object TabSheet3: TTabSheet
      Caption = #35013#22791#20449#24687
      ImageIndex = 2
      object Label70: TLabel
        Left = 394
        Top = 56
        Width = 54
        Height = 12
        Caption = #29289#21697#39068#33394':'
      end
      object ButtonClientHintWindowsSave: TButton
        Left = 548
        Top = 467
        Width = 70
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 13
        OnClick = ButtonClientHintWindowsSaveClick
      end
      object RadioGroupShowItemStyle: TRadioGroup
        Left = 282
        Top = 157
        Width = 105
        Height = 89
        Hint = #26087#27169#24335#21482#26377#22312#37197#32622#22120#36873#25321'1.76'#12289'1.85'#65292#21512#20987#29256#25165#26377#25928
        Caption = #35013#22791#25552#31034#26679#24335
        Items.Strings = (
          #26087#27169#24335
          #26222#36890#24748#28014#24335
          'ASK'#24748#28014#24335)
        ParentShowHint = False
        ShowHint = True
        TabOrder = 7
        OnClick = RadioGroupShowItemStyleClick
      end
      object rgStateWindows: TRadioGroup
        Left = 393
        Top = 208
        Width = 217
        Height = 38
        Hint = #35013#22791#26639#26679#24335#21482#26377#21246#36873'['#20351#29992#21512#20987#30028#38754']'#25165#26377#25928
        Caption = #35013#22791#26639#30028#38754
        Columns = 3
        ItemIndex = 1
        Items.Strings = (
          '1.76'#26679#24335
          '1.85'#26679#24335
          #21512#20987#26679#24335)
        ParentShowHint = False
        ShowHint = True
        TabOrder = 10
        OnClick = rgStateWindowsClick
      end
      object GroupBox13: TGroupBox
        Left = 282
        Top = 10
        Width = 105
        Height = 143
        Caption = #35013#22791#26639#36873#39033
        TabOrder = 1
        object chkHideTabSheet2: TCheckBox
          Left = 8
          Top = 21
          Width = 94
          Height = 19
          Caption = #20154#29289#38544#34255#26102#35013
          TabOrder = 0
          OnClick = chkHideTabSheet2Click
        end
        object chkHideTabSheet5: TCheckBox
          Left = 8
          Top = 66
          Width = 94
          Height = 18
          Hint = #21512#20987#30028#38754#21246#36873#27492#39033#21487#38544#34255#35013#22791#26639#19979#38754'5'#26684
          Caption = #20154#29289#38544#34255#31216#21495
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = chkHideTabSheet5Click
        end
        object chkHideTabSheet7: TCheckBox
          Left = 8
          Top = 110
          Width = 79
          Height = 18
          Caption = #38544#34255#20986#25112
          TabOrder = 4
          OnClick = chkHideTabSheet7Click
        end
        object chkHeroHideTabSheet5: TCheckBox
          Left = 8
          Top = 88
          Width = 94
          Height = 18
          Hint = #21512#20987#30028#38754#21246#36873#27492#39033#21487#38544#34255#35013#22791#26639#19979#38754'5'#26684
          Caption = #33521#38596#38544#34255#31216#21495
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = chkHeroHideTabSheet5Click
        end
        object chkHeroHideTabSheet2: TCheckBox
          Left = 8
          Top = 44
          Width = 94
          Height = 18
          Caption = #33521#38596#38544#34255#26102#35013
          TabOrder = 1
          OnClick = chkHeroHideTabSheet2Click
        end
      end
      object CheckBoxUseOldSerialWindows: TCheckBox
        Left = 393
        Top = 185
        Width = 213
        Height = 18
        Caption = #20351#29992#21512#20987#30028#38754'('#19981#21246#36873#21017#29992#36830#20987#30028#38754')'
        TabOrder = 8
        OnClick = CheckBoxUseOldSerialWindowsClick
      end
      object CheckBoxEscCloseNPC: TCheckBox
        Left = 394
        Top = 12
        Width = 139
        Height = 18
        Hint = #24320#21551#21518#28216#25103#20013#25353'esc'#21487#20851#38381'npc'#23545#35805#26694
        Caption = #24320#21551'ESC'#20851#38381#23545#35805#26694
        ParentShowHint = False
        ShowHint = True
        TabOrder = 2
        OnClick = CheckBoxEscCloseNPCClick
      end
      object chkNpcDlgHintWithMouse: TCheckBox
        Left = 394
        Top = 31
        Width = 161
        Height = 19
        Hint = 'NPC'#26631#31614#22791#27880#38543#40736#26631#31227#21160
        Caption = 'NPC'#23545#35805#26694#25552#31034#38543#40736#26631#20301#32622
        ParentShowHint = False
        ShowHint = True
        TabOrder = 3
        OnClick = chkNpcDlgHintWithMouseClick
      end
      object seThrowAwayItemColor: TColorIndexEdit
        Left = 451
        Top = 52
        Width = 74
        Height = 21
        Hint = #29289#21697#20002#21040#22320#19978#26174#31034#30340#39068#33394#65292#37329#24065#20197#27492#39068#33394#20026#20934#65292#20854#20182#29289#21697#22914#26524#25968#25454#24211#20013'Color'#23383#27573#20026'251'#65292#21017#35813#39068#33394#29983#25928
        MaxLength = 3
        MaxValue = 255
        MinValue = 0
        ParentShowHint = False
        ShowHint = True
        TabOrder = 4
        Value = 100
        OnChange = seThrowAwayItemColorChange
        ShowNoneColor = False
      end
      object grp1: TGroupBox
        Left = 11
        Top = 10
        Width = 258
        Height = 84
        Caption = #24748#28014#20449#24687#26174#31034#36873#39033
        TabOrder = 0
        object chkShowHintWindowFrame: TCheckBox
          Left = 8
          Top = 14
          Width = 86
          Height = 19
          Caption = #26174#31034#36793#26694
          TabOrder = 0
          OnClick = chkShowHintWindowFrameClick
        end
        object chkShowHintLines: TCheckBox
          Left = 8
          Top = 35
          Width = 86
          Height = 19
          Caption = #26174#31034#20998#30028#32447
          TabOrder = 3
          OnClick = chkShowHintLinesClick
        end
        object chkShowItemForm: TCheckBox
          Left = 95
          Top = 15
          Width = 96
          Height = 18
          Hint = #21482#26377#22312#24748#28014#24335#25165#21487#20197#29983#25928
          Caption = #26174#31034#29289#21697#26469#28304
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkShowItemFormClick
        end
        object chkShowInsuranceInfo: TCheckBox
          Left = 95
          Top = 35
          Width = 96
          Height = 18
          Caption = #26174#31034#25237#20445#20449#24687
          TabOrder = 4
          OnClick = chkShowInsuranceInfoClick
        end
        object chkShowItemSellPrice: TCheckBox
          Left = 95
          Top = 56
          Width = 96
          Height = 18
          Hint = #26174#31034#29289#21697#21334#32473#21830#24215#20215#26684#65292#21482#26377#22312#24748#28014#24335#25165#21487#20197#29983#25928
          Caption = #26174#31034#20986#21806#20215#26684
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          OnClick = chkShowItemSellPriceClick
        end
        object chkHintWithMouse: TCheckBox
          Left = 8
          Top = 56
          Width = 86
          Height = 19
          Hint = #21482#26377#22312#24748#28014#24335#25165#21487#20197#29983#25928
          Caption = #38543#40736#26631#20301#32622
          ParentShowHint = False
          ShowHint = True
          TabOrder = 8
          OnClick = chkHintWithMouseClick
        end
        object seShowItemFormColor: TColorIndexEdit
          Left = 192
          Top = 13
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = seShowItemFormColorChange
          ShowNoneColor = False
        end
        object seShowInsuranceInfoColor: TColorIndexEdit
          Left = 192
          Top = 34
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 5
          Value = 100
          OnChange = seShowInsuranceInfoColorChange
          ShowNoneColor = False
        end
        object seShowItemSellPriceColor: TColorIndexEdit
          Left = 192
          Top = 55
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 7
          Value = 100
          OnChange = seShowItemSellPriceColorChange
          ShowNoneColor = False
        end
      end
      object grp3: TGroupBox
        Left = 393
        Top = 99
        Width = 217
        Height = 81
        Caption = #26102#35013#25511#21046
        TabOrder = 5
        object chkShowNormalFashion: TCheckBox
          Left = 10
          Top = 16
          Width = 79
          Height = 18
          Hint = #21246#36873#21518#65306#21482#25343#20102#26102#35013#27494#22120#19988#22806#26174#26102#35013#65292#20154#29289#26174#31034#36534#20307#21644#26102#35013#27494#22120
          Caption = #20256#32479#26102#35013
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkShowNormalFashionClick
        end
        object chkFashionJewelryOpen: TCheckBox
          Left = 90
          Top = 16
          Width = 104
          Height = 19
          Caption = #24320#21551#26102#35013#39318#39280
          TabOrder = 1
          OnClick = chkFashionJewelryOpenClick
        end
        object chkShowFashionHideShield: TCheckBox
          Left = 11
          Top = 56
          Width = 134
          Height = 18
          Hint = #21246#36873#21518#65306#22806#26174#26102#35013#21518#20154#29289#22806#35266#19981#26174#31034#30462#29260
          Caption = #22806#26174#26102#35013#19981#26174#31034#30462#29260
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = chkShowFashionHideShieldClick
        end
        object chkShowFashionHideHats: TCheckBox
          Left = 11
          Top = 34
          Width = 134
          Height = 19
          Hint = #21246#36873#21518#65306#22806#26174#26102#35013#21518#20154#29289#22806#35266#19981#26174#31034#30462#29260
          Caption = #22806#26174#26102#35013#19981#26174#31034#26007#31520
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = chkShowFashionHideHatsClick
        end
      end
      object GroupBox23: TGroupBox
        Left = 11
        Top = 98
        Width = 258
        Height = 45
        Caption = #24748#28014#31383#32972#26223
        TabOrder = 9
        object Label86: TLabel
          Left = 7
          Top = 20
          Width = 54
          Height = 12
          Caption = #32972#26223#39068#33394':'
        end
        object Label87: TLabel
          Left = 152
          Top = 20
          Width = 42
          Height = 12
          Caption = #36879#26126#24230':'
        end
        object seHintWindowBGColor: TColorIndexEdit
          Left = 63
          Top = 16
          Width = 60
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seHintWindowBGColorChange
          ShowNoneColor = False
        end
        object seHintWindowBGAlpha: TSpinEditEx
          Left = 197
          Top = 16
          Width = 55
          Height = 21
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seHintWindowBGAlphaChange
        end
      end
      object GroupBox24: TGroupBox
        Left = 11
        Top = 304
        Width = 258
        Height = 89
        Caption = #24748#28014#31383#25991#23383#35774#32622
        TabOrder = 11
        object lbl6: TLabel
          Left = 7
          Top = 41
          Width = 84
          Height = 12
          Caption = #29289#21697#21517#31216' '#23383#21495':'
        end
        object Label88: TLabel
          Left = 7
          Top = 18
          Width = 84
          Height = 12
          Caption = #29289#21697#21517#31216' '#23383#20307':'
        end
        object Label73: TLabel
          Left = 7
          Top = 66
          Width = 84
          Height = 12
          Caption = #20854#20182#25991#23383' '#23383#21495':'
        end
        object seShowHintNameFontSize: TSpinEditEx
          Left = 93
          Top = 37
          Width = 38
          Height = 21
          MaxValue = 15
          MinValue = 9
          TabOrder = 1
          Value = 10
          OnChange = seShowHintNameFontSizeChange
        end
        object edtShowHintFontName: TEdit
          Left = 93
          Top = 13
          Width = 156
          Height = 20
          TabOrder = 0
          OnChange = edtShowHintFontNameChange
        end
        object seShowHintOtherFontSize: TSpinEditEx
          Left = 93
          Top = 62
          Width = 38
          Height = 21
          MaxValue = 15
          MinValue = 9
          TabOrder = 4
          Value = 10
          OnChange = seShowHintOtherFontSizeChange
        end
        object cbbShowHintNameFontBold: TComboBox
          Left = 136
          Top = 37
          Width = 55
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #40664#35748
          OnChange = cbbShowHintNameFontBoldChange
          Items.Strings = (
            #40664#35748
            #27491#24120
            #31895#20307)
        end
        object cbbShowHintNameFontStroke: TComboBox
          Left = 193
          Top = 37
          Width = 59
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #40664#35748
          OnChange = cbbShowHintNameFontStrokeChange
          Items.Strings = (
            #40664#35748
            #19981#25551#36793
            #25551#36793)
        end
        object cbbShowHintOtherFontBold: TComboBox
          Left = 136
          Top = 61
          Width = 55
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 5
          Text = #40664#35748
          OnChange = cbbShowHintOtherFontBoldChange
          Items.Strings = (
            #40664#35748
            #27491#24120
            #31895#20307)
        end
        object cbbShowHintOtherFontStroke: TComboBox
          Left = 193
          Top = 61
          Width = 59
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 6
          Text = #40664#35748
          OnChange = cbbShowHintOtherFontStrokeChange
          Items.Strings = (
            #40664#35748
            #19981#25551#36793
            #25551#36793)
        end
      end
      object chkMoveItemShowID: TCheckBox
        Left = 9
        Top = 401
        Width = 209
        Height = 17
        Caption = #40736#26631#28857#20987#29289#21697#31227#21160#26102#65292#26174#31034#29289#21697'ID'
        TabOrder = 12
        OnClick = chkMoveItemShowIDClick
      end
      object GroupBox26: TGroupBox
        Left = 11
        Top = 216
        Width = 258
        Height = 83
        Caption = #29289#21697#26469#28304#26174#31034#23383#27573
        TabOrder = 6
        object chkItemFromField0: TCheckBox
          Left = 8
          Top = 17
          Width = 76
          Height = 19
          Caption = #21046#36896#26469#28304
          TabOrder = 0
          OnClick = chkItemFromField0Click
        end
        object chkItemFromField3: TCheckBox
          Tag = 3
          Left = 8
          Top = 39
          Width = 76
          Height = 19
          Caption = #26102#38388
          TabOrder = 3
          OnClick = chkItemFromField0Click
        end
        object chkItemFromField6: TCheckBox
          Tag = 6
          Left = 8
          Top = 61
          Width = 76
          Height = 18
          Caption = #20987#26432
          TabOrder = 6
          OnClick = chkItemFromField0Click
        end
        object chkItemFromField1: TCheckBox
          Tag = 1
          Left = 92
          Top = 17
          Width = 76
          Height = 19
          Caption = #21046#36896#32773
          TabOrder = 1
          OnClick = chkItemFromField0Click
        end
        object chkItemFromField2: TCheckBox
          Tag = 2
          Left = 176
          Top = 17
          Width = 76
          Height = 19
          Caption = #36141#20080#20154
          TabOrder = 2
          OnClick = chkItemFromField0Click
        end
        object chkItemFromField4: TCheckBox
          Tag = 4
          Left = 92
          Top = 39
          Width = 76
          Height = 19
          Caption = #22320#22270
          TabOrder = 4
          OnClick = chkItemFromField0Click
        end
        object chkItemFromField5: TCheckBox
          Tag = 5
          Left = 176
          Top = 39
          Width = 76
          Height = 19
          Caption = #24618#29289
          TabOrder = 5
          OnClick = chkItemFromField0Click
        end
      end
      object GroupBox29: TGroupBox
        Left = 11
        Top = 146
        Width = 258
        Height = 67
        Caption = #24748#28014#31383#36793#36317
        TabOrder = 14
        object Label104: TLabel
          Left = 7
          Top = 20
          Width = 54
          Height = 12
          Caption = #24038#36793#36317#31163':'
        end
        object Label105: TLabel
          Left = 135
          Top = 20
          Width = 54
          Height = 12
          Caption = #19978#36793#36317#31163':'
        end
        object Label106: TLabel
          Left = 7
          Top = 44
          Width = 54
          Height = 12
          Caption = #21491#36793#36317#31163':'
        end
        object Label107: TLabel
          Left = 135
          Top = 44
          Width = 54
          Height = 12
          Caption = #19979#36793#36317#31163':'
        end
        object seHintWindowBorderWidthLeft: TSpinEditEx
          Left = 63
          Top = 16
          Width = 60
          Height = 21
          MaxValue = 100
          MinValue = 4
          TabOrder = 0
          Value = 20
          OnChange = seHintWindowBorderWidthLeftChange
        end
        object seHintWindowBorderWidthTop: TSpinEditEx
          Left = 191
          Top = 16
          Width = 60
          Height = 21
          MaxValue = 100
          MinValue = 4
          TabOrder = 1
          Value = 20
          OnChange = seHintWindowBorderWidthTopChange
        end
        object seHintWindowBorderWidthRight: TSpinEditEx
          Left = 63
          Top = 40
          Width = 60
          Height = 21
          MaxValue = 100
          MinValue = 4
          TabOrder = 2
          Value = 20
          OnChange = seHintWindowBorderWidthRightChange
        end
        object seHintWindowBorderWidthBottom: TSpinEditEx
          Left = 190
          Top = 40
          Width = 60
          Height = 21
          MaxValue = 100
          MinValue = 4
          TabOrder = 3
          Value = 20
          OnChange = seHintWindowBorderWidthBottomChange
        end
      end
      object chkHelmetShowInBox: TCheckBox
        Left = 394
        Top = 76
        Width = 144
        Height = 17
        Hint = #21246#36873#21518#65292#20154#29289#20869#35266#20013#30340#22836#30420#26174#31034#22312#39318#39280#26694#20013#32780#38750#22836#39030
        Caption = #22836#30420#22312#39318#39280#26694#20013#26174#31034
        ParentShowHint = False
        ShowHint = True
        TabOrder = 15
        OnClick = chkHelmetShowInBoxClick
      end
      object RadioGroupBagFastItemCompare: TRadioGroup
        Left = 282
        Top = 252
        Width = 166
        Height = 85
        Caption = #32972#21253#23545#27604#36873#39033
        Items.Strings = (
          #26681#25454#25915#20987' '#39764#27861' '#36947#26415#23545#27604
          #26681#25454#31561#32423#23545#27604
          #26681#25454#37325#37327#23545#27604)
        ParentShowHint = False
        ShowHint = False
        TabOrder = 16
        OnClick = RadioGroupBagFastItemCompareClick
      end
    end
    object TabSheet4: TTabSheet
      Caption = #20869#25346#33647#21697
      ImageIndex = 3
      object Label27: TLabel
        Left = 146
        Top = 78
        Width = 54
        Height = 12
        Caption = #33647#21697#21517#31216':'
      end
      object Label4: TLabel
        Left = 353
        Top = 78
        Width = 66
        Height = 12
        Caption = #25903#25345'9'#20010#33647#21697
      end
      object lbl15: TLabel
        Left = 152
        Top = 184
        Width = 210
        Height = 12
        Caption = #20869#25346#33647#21697#20026#40664#35748#20869#25346#20013'"'#33258#21160#20351#29992#33647#21697'" '
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object lbl16: TLabel
        Left = 152
        Top = 204
        Width = 300
        Height = 12
        Caption = #21333#27425#21463#21040#20260#23475#36798#21040#35774#32622#20540#21518#24320#22987#33258#21160#20351#29992#33647#21697#20445#25252#30340#33647#21697
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object Label10: TLabel
        Left = 152
        Top = 224
        Width = 348
        Height = 12
        Caption = #22914#38656#20869#25346#24120#35268#20445#25252#33647#21697#40664#35748#35774#32622#65292#35831#22312#24341#25806#25991#26723#20013#25628#32034'"'#40664#35748#33647#21697'"'
        Font.Charset = GB2312_CHARSET
        Font.Color = clBlue
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object ListBoxClientItemName: TListBox
        Left = 9
        Top = 9
        Width = 130
        Height = 449
        ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        ItemHeight = 12
        TabOrder = 0
        OnClick = ListBoxClientItemNameClick
      end
      object ButtonClientItemNameeUP: TButton
        Left = 146
        Top = 9
        Width = 36
        Height = 27
        Caption = #8593
        Enabled = False
        TabOrder = 1
        OnClick = ButtonClientItemNameeUPClick
      end
      object ButtonClientItemNameDown: TButton
        Left = 146
        Top = 34
        Width = 36
        Height = 27
        Caption = #8595
        Enabled = False
        TabOrder = 2
        OnClick = ButtonClientItemNameDownClick
      end
      object EditClientItemName: TEdit
        Left = 207
        Top = 73
        Width = 130
        Height = 20
        ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 3
      end
      object ButtonClientItemNameAdd: TButton
        Left = 207
        Top = 112
        Width = 81
        Height = 27
        Caption = #22686#21152'(&A)'
        TabOrder = 4
        OnClick = ButtonClientItemNameAddClick
      end
      object ButtonClientItemNameDel: TButton
        Left = 293
        Top = 112
        Width = 81
        Height = 27
        Caption = #21024#38500'(&D)'
        Enabled = False
        TabOrder = 5
        OnClick = ButtonClientItemNameDelClick
      end
      object ButtonClientItemNameSave: TButton
        Left = 379
        Top = 112
        Width = 81
        Height = 27
        Caption = #20445#23384'(&S)'
        Enabled = False
        TabOrder = 6
        OnClick = ButtonClientItemNameSaveClick
      end
    end
    object TabSheet5: TTabSheet
      Caption = #22825#27668#31995#32479
      ImageIndex = 4
      object GroupBox4: TGroupBox
        Left = 9
        Top = 9
        Width = 130
        Height = 61
        Caption = #26159#21542#20813#34593
        TabOrder = 0
        object CheckBoxViewFog: TCheckBox
          Left = 9
          Top = 26
          Width = 78
          Height = 18
          Caption = #31105#27490#20813#34593
          TabOrder = 0
          OnClick = CheckBoxViewFogClick
        end
      end
      object ButtonWeatherSave: TButton
        Left = 534
        Top = 463
        Width = 70
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 3
        OnClick = ButtonWeatherSaveClick
      end
      object GroupBox5: TGroupBox
        Left = 146
        Top = 9
        Width = 148
        Height = 371
        Caption = #30333#22825#40657#22812
        TabOrder = 1
        object ListBoxBright: TListBox
          Left = 9
          Top = 17
          Width = 130
          Height = 346
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          Items.Strings = (
            '0'#28857
            '1'#28857
            '2'#28857
            '3'#28857
            '4'#28857
            '5'#28857
            '6'#28857
            '7'#28857
            '8'#28857
            '9'#28857
            '10'#28857
            '11'#28857
            '12'#28857
            '13'#28857
            '14'#28857
            '15'#28857
            '16'#28857
            '17'#28857
            '18'#28857
            '19'#28857
            '20'#28857
            '21'#28857
            '22'#28857
            '23'#28857)
          TabOrder = 0
          OnClick = ListBoxBrightClick
        end
      end
      object RadioGroupBright: TRadioGroup
        Left = 302
        Top = 9
        Width = 104
        Height = 113
        Caption = #20142#24230
        Items.Strings = (
          #26085#20986
          #30333#22825
          #20621#26202
          #40657#22812)
        TabOrder = 2
        OnClick = RadioGroupBrightClick
      end
    end
    object ts3: TTabSheet
      Caption = #25353#38062#33258#21160#25490#21015
      ImageIndex = 8
      object lbl12: TLabel
        Left = 6
        Top = 477
        Width = 510
        Height = 12
        Caption = #29992#20110'Npc'#21629#20196'AddArrButton, SetArrBuff'#30340#25353#38062#33258#21160#25490#21015#20301#32622#65292#25490#21015#39034#24207#20381#25454#25353#38062#24207#21495#65292#23567#36864#29983#25928
        Font.Charset = GB2312_CHARSET
        Font.Color = clTeal
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object grpArrButton1: TGroupBox
        Left = 3
        Top = 0
        Width = 613
        Height = 63
        Caption = #20998#32452'1'
        TabOrder = 0
        object lblArrBtnHorzAlign1: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX1: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign1: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY1: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart1: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset1: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX1: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY1: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign1: TComboBox
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX1: TSpinEditEx
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign1: TComboBox
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY1: TSpinEditEx
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX1: TSpinEditEx
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY1: TSpinEditEx
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object grpArrButton2: TGroupBox
        Left = 3
        Top = 67
        Width = 613
        Height = 63
        Caption = #20998#32452'2'
        TabOrder = 1
        object lblArrBtnHorzAlign2: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX2: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign2: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY2: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart2: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset2: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX2: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY2: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign2: TComboBox
          Tag = 1
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX2: TSpinEditEx
          Tag = 1
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign2: TComboBox
          Tag = 1
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY2: TSpinEditEx
          Tag = 1
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX2: TSpinEditEx
          Tag = 1
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY2: TSpinEditEx
          Tag = 1
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object grpArrButton3: TGroupBox
        Left = 3
        Top = 134
        Width = 613
        Height = 63
        Caption = #20998#32452'3'
        TabOrder = 2
        object lblArrBtnHorzAlign3: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX3: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign3: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY3: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart3: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset3: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX3: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY3: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign3: TComboBox
          Tag = 2
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX3: TSpinEditEx
          Tag = 2
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign3: TComboBox
          Tag = 2
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY3: TSpinEditEx
          Tag = 2
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX3: TSpinEditEx
          Tag = 2
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY3: TSpinEditEx
          Tag = 2
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object grpArrButton4: TGroupBox
        Left = 3
        Top = 201
        Width = 613
        Height = 63
        Caption = #20998#32452'4'
        TabOrder = 3
        object lblArrBtnHorzAlign4: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX4: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign4: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY4: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart4: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset4: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX4: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY4: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign4: TComboBox
          Tag = 3
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX4: TSpinEditEx
          Tag = 3
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign4: TComboBox
          Tag = 3
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY4: TSpinEditEx
          Tag = 3
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX4: TSpinEditEx
          Tag = 3
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY4: TSpinEditEx
          Tag = 3
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object grpArrButton5: TGroupBox
        Left = 3
        Top = 268
        Width = 613
        Height = 63
        Caption = #20998#32452'5'
        TabOrder = 4
        object lblArrBtnHorzAlign5: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX5: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign5: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY5: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart5: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset5: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX5: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY5: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign5: TComboBox
          Tag = 4
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX5: TSpinEditEx
          Tag = 4
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign5: TComboBox
          Tag = 4
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY5: TSpinEditEx
          Tag = 4
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX5: TSpinEditEx
          Tag = 4
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY5: TSpinEditEx
          Tag = 4
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object grpArrButton6: TGroupBox
        Left = 3
        Top = 335
        Width = 613
        Height = 63
        Caption = #20998#32452'6'
        TabOrder = 5
        object lblArrBtnHorzAlign6: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX6: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign6: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY6: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart6: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset6: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX6: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY6: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign6: TComboBox
          Tag = 5
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX6: TSpinEditEx
          Tag = 5
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign6: TComboBox
          Tag = 5
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY6: TSpinEditEx
          Tag = 5
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX6: TSpinEditEx
          Tag = 5
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY6: TSpinEditEx
          Tag = 5
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object grpArrButton7: TGroupBox
        Left = 3
        Top = 402
        Width = 613
        Height = 63
        Caption = #20998#32452'7'
        TabOrder = 6
        object lblArrBtnHorzAlign7: TLabel
          Left = 78
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#23545#40784
        end
        object lblArrBtnOffsetX7: TLabel
          Left = 214
          Top = 17
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblArrBtnVertAlign7: TLabel
          Left = 350
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#23545#40784
        end
        object lblArrBtnOffsetY7: TLabel
          Left = 486
          Top = 17
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object lbl1ArrBtnStart7: TLabel
          Left = 13
          Top = 17
          Width = 60
          Height = 12
          Caption = #36215#22987#25353#38062#65306
        end
        object lbl1NextArrBtnOffset7: TLabel
          Left = 13
          Top = 41
          Width = 60
          Height = 12
          Caption = #19979#20010#25353#38062#65306
        end
        object lblNextArrBtnOffsetX7: TLabel
          Left = 78
          Top = 41
          Width = 48
          Height = 12
          Caption = #27700#24179#20559#31227
        end
        object lblNextArrBtnOffsetY7: TLabel
          Left = 214
          Top = 41
          Width = 48
          Height = 12
          Caption = #22402#30452#20559#31227
        end
        object cbbArrBtnHorzAlign7: TComboBox
          Tag = 6
          Left = 130
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #23621#24038
          OnChange = cbbArrBtnHorzAlign1Change
          Items.Strings = (
            #23621#24038
            #23621#21491
            #23621#20013)
        end
        object seArrBtnOffsetX7: TSpinEditEx
          Tag = 6
          Left = 266
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          OnChange = seArrBtnOffsetX1Change
        end
        object cbbArrBtnVertAlign7: TComboBox
          Tag = 6
          Left = 402
          Top = 13
          Width = 60
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #23621#19978
          OnChange = cbbArrBtnVertAlign1Change
          Items.Strings = (
            #23621#19978
            #23621#19979
            #23621#20013)
        end
        object seArrBtnOffsetY7: TSpinEditEx
          Tag = 6
          Left = 538
          Top = 12
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          OnChange = seArrBtnOffsetY1Change
        end
        object seNextArrBtnOffsetX7: TSpinEditEx
          Tag = 6
          Left = 130
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          OnChange = seNextArrBtnOffsetX1Change
        end
        object seNextArrBtnOffsetY7: TSpinEditEx
          Tag = 6
          Left = 266
          Top = 36
          Width = 60
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          OnChange = seNextArrBtnOffsetY1Change
        end
      end
      object btnArrBtnSetting: TButton
        Left = 541
        Top = 470
        Width = 75
        Height = 25
        Caption = #20445#23384
        TabOrder = 7
        OnClick = btnArrBtnSettingClick
      end
    end
    object TabSheet6: TTabSheet
      Caption = #20854#20182#25511#21046
      ImageIndex = 5
      object GroupBox6: TGroupBox
        Left = 8
        Top = 9
        Width = 155
        Height = 95
        Caption = #20445#25252
        TabOrder = 0
        object CheckBoxMonStruckShowNumber: TCheckBox
          Left = 9
          Top = 17
          Width = 141
          Height = 19
          Caption = #24618#29289#34987#25915#20987#21518#25968#23383#26174#34880
          TabOrder = 0
          OnClick = CheckBoxMonStruckShowNumberClick
        end
        object CheckBoxHumStruckShowNumber: TCheckBox
          Left = 9
          Top = 34
          Width = 141
          Height = 19
          Caption = #20154#29289#34987#25915#20987#21518#25968#23383#26174#34880
          TabOrder = 1
          OnClick = CheckBoxHumStruckShowNumberClick
        end
        object CheckBoxCloseBookProtect: TCheckBox
          Left = 9
          Top = 52
          Width = 130
          Height = 18
          Caption = #20851#38381#33258#21160#21367#36724#20445#25252
          TabOrder = 2
          OnClick = CheckBoxCloseBookProtectClick
        end
        object CheckBoxCloseLogoutProtect: TCheckBox
          Left = 9
          Top = 69
          Width = 104
          Height = 18
          Caption = #20851#38381#23567#36864#20445#25252
          TabOrder = 3
          OnClick = CheckBoxCloseLogoutProtectClick
        end
      end
      object ButtonGameAuxiliarySave2: TButton
        Left = 539
        Top = 469
        Width = 70
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 6
        OnClick = ButtonGameAuxiliarySave2Click
      end
      object GroupBox7: TGroupBox
        Left = 8
        Top = 112
        Width = 396
        Height = 380
        Caption = #20449#24687#25511#21046
        TabOrder = 2
        object Label29: TLabel
          Left = 9
          Top = 22
          Width = 30
          Height = 12
          Caption = #20154#29289':'
        end
        object Label30: TLabel
          Left = 9
          Top = 201
          Width = 30
          Height = 12
          Caption = #33521#38596':'
        end
        object GroupBox55: TGroupBox
          Left = 9
          Top = 39
          Width = 122
          Height = 155
          Caption = #33719#24471#29289#21697
          TabOrder = 1
          object Label108: TLabel
            Left = 15
            Top = 17
            Width = 30
            Height = 12
            Caption = #25991#23383':'
          end
          object Label109: TLabel
            Left = 15
            Top = 43
            Width = 30
            Height = 12
            Caption = #32972#26223':'
          end
          object Label31: TLabel
            Left = 12
            Top = 69
            Width = 36
            Height = 12
            Caption = 'X'#22352#26631':'
          end
          object Label32: TLabel
            Left = 12
            Top = 95
            Width = 36
            Height = 12
            Caption = 'Y'#22352#26631':'
          end
          object seAddItemMsgFColor: TColorIndexEdit
            Left = 46
            Top = 13
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 0
            Value = 100
            OnChange = seAddItemMsgFColorChange
            ShowNoneColor = False
          end
          object seAddItemMsgBColor: TColorIndexEdit
            Left = 46
            Top = 39
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seAddItemMsgBColorChange
            ShowNoneColor = False
          end
          object seAddItemMsgX: TSpinEditEx
            Left = 54
            Top = 65
            Width = 56
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 100
            OnChange = seAddItemMsgXChange
          end
          object seAddItemMsgY: TSpinEditEx
            Left = 54
            Top = 90
            Width = 56
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 3
            Value = 100
            OnChange = seAddItemMsgYChange
          end
          object chkAddItemMsgXRightToLeft: TCheckBox
            Left = 18
            Top = 113
            Width = 95
            Height = 18
            Caption = 'X'#22352#26631#20174#21491#31639
            TabOrder = 4
            OnClick = chkAddItemMsgXRightToLeftClick
          end
          object chkAddItemMsgYBottomToTop: TCheckBox
            Left = 18
            Top = 132
            Width = 95
            Height = 18
            Caption = 'Y'#22352#26631#20174#19979#31639
            TabOrder = 5
            OnClick = chkAddItemMsgYBottomToTopClick
          end
        end
        object GroupBox8: TGroupBox
          Left = 137
          Top = 39
          Width = 122
          Height = 155
          Caption = #33719#24471#32463#39564
          TabOrder = 2
          object Label8: TLabel
            Left = 15
            Top = 17
            Width = 30
            Height = 12
            Caption = #25991#23383':'
          end
          object Label9: TLabel
            Left = 15
            Top = 43
            Width = 30
            Height = 12
            Caption = #32972#26223':'
          end
          object Label33: TLabel
            Left = 12
            Top = 69
            Width = 36
            Height = 12
            Caption = 'X'#22352#26631':'
          end
          object Label34: TLabel
            Left = 12
            Top = 95
            Width = 36
            Height = 12
            Caption = 'Y'#22352#26631':'
          end
          object seGetExpMsgFColor: TColorIndexEdit
            Left = 46
            Top = 13
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 0
            Value = 100
            OnChange = seGetExpMsgFColorChange
            ShowNoneColor = False
          end
          object seGetExpMsgBColor: TColorIndexEdit
            Left = 46
            Top = 39
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seGetExpMsgBColorChange
            ShowNoneColor = False
          end
          object seGetExpMsgX: TSpinEditEx
            Left = 54
            Top = 65
            Width = 56
            Height = 21
            Hint = #19981#22312#32842#22825#26694#26174#31034#26102#26377#25928
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 2
            Value = 100
            OnChange = seGetExpMsgXChange
          end
          object seGetExpMsgY: TSpinEditEx
            Left = 54
            Top = 90
            Width = 56
            Height = 21
            Hint = #19981#22312#32842#22825#26694#26174#31034#26102#26377#25928
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 3
            Value = 100
            OnChange = seGetExpMsgYChange
          end
          object chkGetExpMsgXRightToLeft: TCheckBox
            Left = 18
            Top = 113
            Width = 95
            Height = 18
            Caption = 'X'#22352#26631#20174#21491#31639
            TabOrder = 4
            OnClick = chkGetExpMsgXRightToLeftClick
          end
          object chkGetExpMsgYBottomToTop: TCheckBox
            Left = 18
            Top = 132
            Width = 95
            Height = 18
            Caption = 'Y'#22352#26631#20174#19979#31639
            TabOrder = 5
            OnClick = chkGetExpMsgYBottomToTopClick
          end
        end
        object GroupBox9: TGroupBox
          Left = 265
          Top = 39
          Width = 122
          Height = 155
          Caption = #21319#32423
          TabOrder = 3
          object Label12: TLabel
            Left = 15
            Top = 17
            Width = 30
            Height = 12
            Caption = #25991#23383':'
          end
          object Label13: TLabel
            Left = 15
            Top = 43
            Width = 30
            Height = 12
            Caption = #32972#26223':'
          end
          object Label35: TLabel
            Left = 12
            Top = 69
            Width = 36
            Height = 12
            Caption = 'X'#22352#26631':'
          end
          object Label36: TLabel
            Left = 12
            Top = 95
            Width = 36
            Height = 12
            Caption = 'Y'#22352#26631':'
          end
          object seUpLevelMsgFColor: TColorIndexEdit
            Left = 46
            Top = 13
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 0
            Value = 100
            OnChange = seUpLevelMsgFColorChange
            ShowNoneColor = False
          end
          object seUpLevelMsgBColor: TColorIndexEdit
            Left = 46
            Top = 39
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seUpLevelMsgBColorChange
            ShowNoneColor = False
          end
          object seUpLevelMsgX: TSpinEditEx
            Left = 54
            Top = 65
            Width = 56
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 100
            OnChange = seUpLevelMsgXChange
          end
          object seUpLevelMsgY: TSpinEditEx
            Left = 54
            Top = 90
            Width = 56
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 3
            Value = 100
            OnChange = seUpLevelMsgYChange
          end
          object chkUpLevelMsgXRightToLeft: TCheckBox
            Left = 18
            Top = 113
            Width = 95
            Height = 18
            Caption = 'X'#22352#26631#20174#21491#31639
            TabOrder = 4
            OnClick = chkUpLevelMsgXRightToLeftClick
          end
          object chkUpLevelMsgYBottomToTop: TCheckBox
            Left = 18
            Top = 132
            Width = 95
            Height = 18
            Caption = 'Y'#22352#26631#20174#19979#31639
            TabOrder = 5
            OnClick = chkUpLevelMsgYBottomToTopClick
          end
        end
        object GroupBox10: TGroupBox
          Left = 9
          Top = 218
          Width = 122
          Height = 155
          Caption = #33719#24471#29289#21697
          TabOrder = 5
          object Label16: TLabel
            Left = 15
            Top = 17
            Width = 30
            Height = 12
            Caption = #25991#23383':'
          end
          object Label17: TLabel
            Left = 15
            Top = 43
            Width = 30
            Height = 12
            Caption = #32972#26223':'
          end
          object Label20: TLabel
            Left = 12
            Top = 69
            Width = 36
            Height = 12
            Caption = 'X'#22352#26631':'
          end
          object Label21: TLabel
            Left = 12
            Top = 95
            Width = 36
            Height = 12
            Caption = 'Y'#22352#26631':'
          end
          object seHeroAddItemMsgFColor: TColorIndexEdit
            Left = 46
            Top = 13
            Width = 65
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 0
            Value = 100
            OnChange = seHeroAddItemMsgFColorChange
            ShowNoneColor = False
          end
          object seHeroAddItemMsgBColor: TColorIndexEdit
            Left = 46
            Top = 39
            Width = 65
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seHeroAddItemMsgBColorChange
            ShowNoneColor = False
          end
          object seHeroAddItemMsgX: TSpinEditEx
            Left = 54
            Top = 65
            Width = 57
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 100
            OnChange = seHeroAddItemMsgXChange
          end
          object seHeroAddItemMsgY: TSpinEditEx
            Left = 54
            Top = 90
            Width = 57
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 3
            Value = 100
            OnChange = seHeroAddItemMsgYChange
          end
          object chkHeroAddItemMsgXRightToLeft: TCheckBox
            Left = 18
            Top = 113
            Width = 95
            Height = 18
            Caption = 'X'#22352#26631#20174#21491#31639
            TabOrder = 4
            OnClick = chkHeroAddItemMsgXRightToLeftClick
          end
          object chkHeroAddItemMsgYBottomToTop: TCheckBox
            Left = 18
            Top = 132
            Width = 95
            Height = 18
            Caption = 'Y'#22352#26631#20174#19979#31639
            TabOrder = 5
            OnClick = chkHeroAddItemMsgYBottomToTopClick
          end
        end
        object GroupBox11: TGroupBox
          Left = 137
          Top = 217
          Width = 122
          Height = 155
          Caption = #33719#24471#32463#39564
          TabOrder = 4
          object Label22: TLabel
            Left = 15
            Top = 17
            Width = 30
            Height = 12
            Caption = #25991#23383':'
          end
          object Label23: TLabel
            Left = 15
            Top = 43
            Width = 30
            Height = 12
            Caption = #32972#26223':'
          end
          object Label26: TLabel
            Left = 12
            Top = 69
            Width = 36
            Height = 12
            Caption = 'X'#22352#26631':'
          end
          object Label28: TLabel
            Left = 12
            Top = 95
            Width = 36
            Height = 12
            Caption = 'Y'#22352#26631':'
          end
          object seHeroGetExpMsgFColor: TColorIndexEdit
            Left = 46
            Top = 13
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 0
            Value = 100
            OnChange = seHeroGetExpMsgFColorChange
            ShowNoneColor = False
          end
          object seHeroGetExpMsgBColor: TColorIndexEdit
            Left = 46
            Top = 39
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seHeroGetExpMsgBColorChange
            ShowNoneColor = False
          end
          object seHeroGetExpMsgX: TSpinEditEx
            Left = 54
            Top = 65
            Width = 56
            Height = 21
            Hint = #19981#22312#32842#22825#26694#26174#31034#26102#26377#25928
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 2
            Value = 100
            OnChange = seHeroGetExpMsgXChange
          end
          object seHeroGetExpMsgY: TSpinEditEx
            Left = 54
            Top = 90
            Width = 56
            Height = 21
            Hint = #19981#22312#32842#22825#26694#26174#31034#26102#26377#25928
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 3
            Value = 100
            OnChange = seHeroGetExpMsgYChange
          end
          object chkHeroGetExpMsgXRightToLeft: TCheckBox
            Left = 18
            Top = 113
            Width = 95
            Height = 18
            Caption = 'X'#22352#26631#20174#21491#31639
            TabOrder = 4
            OnClick = chkHeroGetExpMsgXRightToLeftClick
          end
          object chkHeroGetExpMsgYBottomToTop: TCheckBox
            Left = 18
            Top = 132
            Width = 95
            Height = 18
            Caption = 'Y'#22352#26631#20174#19979#31639
            TabOrder = 5
            OnClick = chkHeroGetExpMsgYBottomToTopClick
          end
        end
        object GroupBox12: TGroupBox
          Left = 265
          Top = 218
          Width = 122
          Height = 155
          Caption = #21319#32423
          TabOrder = 6
          object Label37: TLabel
            Left = 15
            Top = 17
            Width = 30
            Height = 12
            Caption = #25991#23383':'
          end
          object Label38: TLabel
            Left = 15
            Top = 43
            Width = 30
            Height = 12
            Caption = #32972#26223':'
          end
          object Label41: TLabel
            Left = 12
            Top = 69
            Width = 36
            Height = 12
            Caption = 'X'#22352#26631':'
          end
          object Label42: TLabel
            Left = 12
            Top = 95
            Width = 36
            Height = 12
            Caption = 'Y'#22352#26631':'
          end
          object seHeroUpLevelMsgFColor: TColorIndexEdit
            Left = 46
            Top = 13
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 0
            Value = 100
            OnChange = seHeroUpLevelMsgFColorChange
            ShowNoneColor = False
          end
          object seHeroUpLevelMsgBColor: TColorIndexEdit
            Left = 46
            Top = 39
            Width = 64
            Height = 21
            MaxLength = 3
            MaxValue = 255
            MinValue = 0
            TabOrder = 1
            Value = 100
            OnChange = seHeroUpLevelMsgBColorChange
            ShowNoneColor = False
          end
          object seHeroUpLevelMsgX: TSpinEditEx
            Left = 54
            Top = 65
            Width = 56
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 100
            OnChange = seHeroUpLevelMsgXChange
          end
          object seHeroUpLevelMsgY: TSpinEditEx
            Left = 54
            Top = 90
            Width = 56
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 3
            Value = 100
            OnChange = seHeroUpLevelMsgYChange
          end
          object chkHeroUpLevelMsgXRightToLeft: TCheckBox
            Left = 18
            Top = 113
            Width = 95
            Height = 18
            Caption = 'X'#22352#26631#20174#21491#31639
            TabOrder = 4
            OnClick = chkHeroUpLevelMsgXRightToLeftClick
          end
          object chkHeroUpLevelMsgYBottomToTop: TCheckBox
            Left = 18
            Top = 132
            Width = 95
            Height = 18
            Caption = 'Y'#22352#26631#20174#19979#31639
            TabOrder = 5
            OnClick = chkHeroUpLevelMsgYBottomToTopClick
          end
        end
        object CheckBoxGetExpMsgAddChatBoardMsg: TCheckBox
          Left = 209
          Top = 17
          Width = 156
          Height = 19
          Caption = #32463#39564#20449#24687#26174#31034#22312#32842#22825#26694
          TabOrder = 0
          OnClick = CheckBoxGetExpMsgAddChatBoardMsgClick
        end
      end
      object CheckGroupNewAbil: TRzCheckGroup
        Left = 410
        Top = 112
        Width = 199
        Height = 227
        Caption = #26174#31034#22312#23646#24615#26694#20013'('#36830#20987#30331#24405#22120#26377#25928')'
        Color = 15987699
        Columns = 2
        GroupStyle = gsStandard
        Items.Strings = (
          #26292#20987#20960#29575
          #25915#20987#20260#23475
          #20260#23475#21560#25910
          #39764#27861#38450#24481
          #24573#35270#38450#24481
          #20260#23475#21453#24377
          #20307#21147#22686#21152
          #39764#21147#22686#21152
          #24594#27668#24674#22797
          #21512#20987#25915#20987
          #20154#29289#29190#29575
          #24618#29289#29190#29575
          #38450#27490#40635#30201
          #38450#27490#25252#36523
          #38450#27490#22797#27963
          #38450#27490#20840#27602
          #38450#27490#35825#24785
          #38450#27490#28779#22681
          #38450#27490#20912#20923
          #38450#27490#34523#32593
          #33268#21629#20960#29575
          #33268#21629#20260#23475
          #33268#21629#38450#24481
          #26292#20987#25239#24615)
        SpaceEvenly = True
        StartXPos = 9
        TabOrder = 3
        Transparent = True
        OnChange = CheckGroupNewAbilChange
        CheckStates = (
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0
          0)
      end
      object GroupBox14: TGroupBox
        Left = 167
        Top = 9
        Width = 445
        Height = 95
        Caption = #32972#21253#30456#20851#25511#21046
        TabOrder = 1
        object lbl13: TLabel
          Left = 12
          Top = 46
          Width = 78
          Height = 12
          Caption = #32972#21253#21491#38190#25805#20316':'
        end
        object chkShowBagGameGoldSeparator: TCheckBox
          Left = 137
          Top = 20
          Width = 126
          Height = 17
          Caption = #25968#20540#26174#31034#21315#20301#20998#38548#31526
          TabOrder = 1
          OnClick = chkShowBagGameGoldSeparatorClick
        end
        object chkShowBagGameInfo: TCheckBox
          Left = 10
          Top = 18
          Width = 119
          Height = 19
          Hint = #21246#36873#21518#32972#21253#40664#35748#23637#31034#36135#24065#20449#24687#65292#19981#21246#36873#21017#26681#25454#40736#26631#20301#32622#26469#21028#26029#26159#21542#26174#31034
          Caption = #22987#32456#26174#31034#36135#24065#20449#24687
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkShowBagGameInfoClick
        end
        object cbbBagRightkey: TComboBox
          Left = 92
          Top = 42
          Width = 125
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 2
          Text = #20154#29289#33521#38596#29289#21697#20114#25442
          OnChange = cbbBagRightkeyChange
          Items.Strings = (
            #20154#29289#33521#38596#29289#21697#20114#25442
            #31359#35013#22791)
        end
      end
      object grp2: TGroupBox
        Left = 410
        Top = 345
        Width = 199
        Height = 40
        Caption = #31216#21495#32032#26448#35835#21462#35774#32622
        TabOrder = 4
        object lbl2: TLabel
          Left = 9
          Top = 19
          Width = 48
          Height = 12
          Caption = #35835#21462#25991#20214
        end
        object cbbTitleFileIndex: TComboBox
          Left = 66
          Top = 14
          Width = 126
          Height = 20
          Style = csDropDownList
          TabOrder = 0
          OnChange = cbbTitleFileIndexChange
        end
        object chkHideIconWithHideTitle: TCheckBox
          Left = 9
          Top = 36
          Width = 182
          Height = 19
          Caption = #38544#34255#31216#21495#21516#26102#38544#34255#39030#25140#33457#38083
          TabOrder = 1
          Visible = False
          OnClick = chkHideIconWithHideTitleClick
        end
      end
      object GroupBox15: TGroupBox
        Left = 410
        Top = 390
        Width = 199
        Height = 45
        Caption = #33521#38596#32972#21253#24555#25463#38190#20449#24687
        TabOrder = 5
        object Label75: TLabel
          Left = 58
          Top = 22
          Width = 12
          Height = 12
          Caption = 'X:'
        end
        object Label77: TLabel
          Left = 128
          Top = 22
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object chkShowHeroShortKey: TCheckBox
          Left = 9
          Top = 19
          Width = 44
          Height = 19
          Caption = #26174#31034
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = chkShowHeroShortKeyClick
        end
        object seShowHeroShortKeyX: TSpinEditEx
          Left = 73
          Top = 17
          Width = 53
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seShowHeroShortKeyXChange
        end
        object seShowHeroShortKeyY: TSpinEditEx
          Left = 140
          Top = 17
          Width = 53
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seShowHeroShortKeyYChange
        end
      end
    end
    object tsDrugAndRestore: TTabSheet
      Caption = #21151#33021#36873#39033
      ImageIndex = 6
      object Label62: TLabel
        Left = 14
        Top = 482
        Width = 432
        Height = 14
        Caption = #35843#25972#30340#21442#25968#31435#21363#29983#25928#65292#22312#32447#26102#35831#30830#35748#27492#21442#25968#30340#20316#29992#20877#35843#25972#65307#20081#35843#25972#23558#23548#33268#28216#25103#28151#20081
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clTeal
        Font.Height = -12
        Font.Name = 'Tahoma'
        Font.Style = []
        ParentFont = False
      end
      object GroupBox81: TGroupBox
        Left = 219
        Top = 360
        Width = 183
        Height = 92
        Caption = #33647#21697#24674#22797#25511#21046
        TabOrder = 7
        object Label168: TLabel
          Left = 10
          Top = 22
          Width = 96
          Height = 12
          Caption = #37329#21019#33647#24674#22797#22522#25968#65306
        end
        object Label169: TLabel
          Left = 10
          Top = 45
          Width = 96
          Height = 12
          Caption = #39764#27861#33647#24674#22797#22522#25968#65306
        end
        object Label207: TLabel
          Left = 10
          Top = 67
          Width = 96
          Height = 12
          Caption = #26222#36890#33647#24674#22797#38388#38548#65306
        end
        object sePerHealth: TSpinEditEx
          Left = 111
          Top = 17
          Width = 57
          Height = 21
          Hint = #25968#23383#36234#23567#24674#22797#36234#24555#65292#40664#35748'10'#13#24674#22797'HP'#20540'='#20154#29289#31561#32423' / '#24674#22797#22522#25968' + 5'
          MaxValue = 10000
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 1
          OnChange = sePerHealthChange
        end
        object sePerSpell: TSpinEditEx
          Left = 111
          Top = 40
          Width = 57
          Height = 21
          Hint = #25968#23383#36234#23567#24674#22797#36234#24555#65292#40664#35748'10'#13#24674#22797'MP'#20540' = '#20154#29289#31561#32423' / '#24674#22797#22522#25968' + 5'
          MaxValue = 10000
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = sePerSpellChange
        end
        object seIncHealthSpell: TSpinEditEx
          Left = 111
          Top = 63
          Width = 57
          Height = 21
          Hint = #25968#23383#36234#22823#24674#22797#38388#38548#36234#24930#65292#40664#35748'600'#13#38388#38548' = '#24674#22797#22522#25968' - '#20154#29289#31561#32423' * 10 '#20154#29289'40'#32423#20043#21518#35813#20540#22266#23450
          Increment = 10
          MaxValue = 100000
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 400
          OnChange = seIncHealthSpellChange
        end
      end
      object GroupBox89: TGroupBox
        Left = 17
        Top = 360
        Width = 183
        Height = 119
        Caption = #24618#29289#24674#22797#36895#24230
        TabOrder = 6
        object Label211: TLabel
          Left = 12
          Top = 22
          Width = 84
          Height = 12
          Caption = #24618#29289#20307#21147#36895#24230#65306
        end
        object Label212: TLabel
          Left = 12
          Top = 75
          Width = 78
          Height = 12
          Caption = #24618#29289#39764#27861#36895#24230':'
        end
        object Label48: TLabel
          Left = 12
          Top = 45
          Width = 84
          Height = 12
          Caption = #24618#29289#20307#21147#22522#25968#65306
        end
        object Label49: TLabel
          Left = 12
          Top = 98
          Width = 84
          Height = 12
          Caption = #24618#29289#39764#27861#22522#25968#65306
        end
        object Bevel5: TBevel
          Left = 6
          Top = 62
          Width = 168
          Height = 5
          Shape = bsBottomLine
        end
        object seHealthFillTime: TSpinEditEx
          Left = 110
          Top = 17
          Width = 58
          Height = 21
          Hint = #24618#29289#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555'.'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seHealthFillTimeChange
        end
        object seSpellFillTime: TSpinEditEx
          Left = 110
          Top = 70
          Width = 58
          Height = 21
          Hint = #24618#29289#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seSpellFillTimeChange
        end
        object seHealthBaseNum: TSpinEditEx
          Left = 110
          Top = 40
          Width = 58
          Height = 21
          Hint = #24618#29289#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seHealthBaseNumChange
        end
        object seSpellBaseNum: TSpinEditEx
          Left = 110
          Top = 93
          Width = 58
          Height = 21
          Hint = #24618#29289#33258#36523#20307#21147#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seSpellBaseNumChange
        end
      end
      object btnSaveDrugAndRestore: TButton
        Left = 525
        Top = 459
        Width = 79
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 9
        OnClick = btnSaveDrugAndRestoreClick
      end
      object GroupBox16: TGroupBox
        Left = 17
        Top = 129
        Width = 183
        Height = 226
        Caption = #20154#29289#24674#22797#36895#24230'('#22522#25968')'
        TabOrder = 3
        object Label15: TLabel
          Left = 12
          Top = 22
          Width = 84
          Height = 12
          Caption = #25112#22763#20307#21147#36895#24230#65306
        end
        object Label18: TLabel
          Left = 12
          Top = 75
          Width = 84
          Height = 12
          Caption = #25112#22763#39764#27861#36895#24230#65306
        end
        object Label19: TLabel
          Left = 12
          Top = 127
          Width = 84
          Height = 12
          Caption = #36947#27861#20307#21147#36895#24230#65306
        end
        object Bevel1: TBevel
          Left = 6
          Top = 116
          Width = 168
          Height = 4
          Shape = bsBottomLine
        end
        object Label44: TLabel
          Left = 12
          Top = 151
          Width = 84
          Height = 12
          Caption = #36947#27861#20307#21147#22522#25968#65306
        end
        object Label50: TLabel
          Left = 12
          Top = 45
          Width = 84
          Height = 12
          Caption = #25112#22763#20307#21147#22522#25968#65306
        end
        object Label51: TLabel
          Left = 12
          Top = 98
          Width = 84
          Height = 12
          Caption = #25112#22763#39764#27861#22522#25968#65306
        end
        object Bevel6: TBevel
          Left = 6
          Top = 62
          Width = 168
          Height = 5
          Shape = bsBottomLine
        end
        object Label24: TLabel
          Left = 12
          Top = 181
          Width = 84
          Height = 12
          Caption = #36947#27861#39764#27861#36895#24230#65306
        end
        object Label45: TLabel
          Left = 12
          Top = 204
          Width = 84
          Height = 12
          Caption = #36947#27861#39764#27861#22522#25968#65306
        end
        object Bevel3: TBevel
          Left = 8
          Top = 169
          Width = 168
          Height = 4
          Shape = bsBottomLine
        end
        object seHealthFillTime_Human_Warrior: TSpinEditEx
          Left = 110
          Top = 17
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seHealthFillTime_Human_WarriorChange
        end
        object seSpellFillTime_Human_Warrior: TSpinEditEx
          Left = 110
          Top = 70
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seSpellFillTime_Human_WarriorChange
        end
        object seHealthFillTime_Human_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 123
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 5
          OnChange = seHealthFillTime_Human_TaoistAndWizardChange
        end
        object seHealthBaseNum_Human_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 146
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 5
          OnChange = seHealthBaseNum_Human_TaoistAndWizardChange
        end
        object seHealthBaseNum_Human_Warrior: TSpinEditEx
          Left = 110
          Top = 40
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seHealthBaseNum_Human_WarriorChange
        end
        object seSpellBaseNum_Human_Warrior: TSpinEditEx
          Left = 110
          Top = 93
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seSpellBaseNum_Human_WarriorChange
        end
        object seSpellFillTime_Human_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 176
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 5
          OnChange = seSpellFillTime_Human_TaoistAndWizardChange
        end
        object seSpellBaseNum_Human_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 199
          Width = 58
          Height = 21
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 5
          OnChange = seSpellBaseNum_Human_TaoistAndWizardChange
        end
      end
      object GroupBox17: TGroupBox
        Left = 219
        Top = 129
        Width = 183
        Height = 226
        Caption = #33521#38596#24674#22797#36895#24230'('#22522#25968')'
        TabOrder = 4
        object Label25: TLabel
          Left = 12
          Top = 22
          Width = 84
          Height = 12
          Caption = #25112#22763#20307#21147#36895#24230#65306
        end
        object Label39: TLabel
          Left = 12
          Top = 75
          Width = 84
          Height = 12
          Caption = #25112#22763#39764#27861#36895#24230#65306
        end
        object Label40: TLabel
          Left = 12
          Top = 128
          Width = 84
          Height = 12
          Caption = #36947#27861#20307#21147#36895#24230#65306
        end
        object Label43: TLabel
          Left = 12
          Top = 181
          Width = 84
          Height = 12
          Caption = #36947#27861#39764#27861#36895#24230#65306
        end
        object Bevel2: TBevel
          Left = 6
          Top = 116
          Width = 168
          Height = 4
          Shape = bsBottomLine
        end
        object Label46: TLabel
          Left = 12
          Top = 151
          Width = 84
          Height = 12
          Caption = #36947#27861#20307#21147#22522#25968#65306
        end
        object Label47: TLabel
          Left = 12
          Top = 204
          Width = 84
          Height = 12
          Caption = #36947#27861#39764#27861#22522#25968#65306
        end
        object Bevel4: TBevel
          Left = 6
          Top = 169
          Width = 168
          Height = 4
          Shape = bsBottomLine
        end
        object Label52: TLabel
          Left = 12
          Top = 45
          Width = 84
          Height = 12
          Caption = #25112#22763#20307#21147#22522#25968#65306
        end
        object Bevel7: TBevel
          Left = 6
          Top = 62
          Width = 168
          Height = 5
          Shape = bsBottomLine
        end
        object Label53: TLabel
          Left = 12
          Top = 98
          Width = 84
          Height = 12
          Caption = #25112#22763#39764#27861#22522#25968#65306
        end
        object seHealthFillTime_Hero_Warrior: TSpinEditEx
          Left = 110
          Top = 17
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555'.'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seHealthFillTime_Hero_WarriorChange
        end
        object seSpellFillTime_Hero_Warrior: TSpinEditEx
          Left = 110
          Top = 70
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seSpellFillTime_Hero_WarriorChange
        end
        object seHealthFillTime_Hero_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 123
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 5
          OnChange = seHealthFillTime_Hero_TaoistAndWizardChange
        end
        object seSpellFillTime_Hero_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 176
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 5
          OnChange = seSpellFillTime_Hero_TaoistAndWizardChange
        end
        object seHealthBaseNum_Hero_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 146
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 5
          OnChange = seHealthBaseNum_Hero_TaoistAndWizardChange
        end
        object seSpellBaseNum_Hero_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 199
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 5
          OnChange = seSpellBaseNum_Hero_TaoistAndWizardChange
        end
        object seHealthBaseNum_Hero_Warrior: TSpinEditEx
          Left = 110
          Top = 40
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seHealthBaseNum_Hero_WarriorChange
        end
        object seSpellBaseNum_Hero_Warrior: TSpinEditEx
          Left = 110
          Top = 93
          Width = 58
          Height = 21
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seSpellBaseNum_Hero_WarriorChange
        end
      end
      object btnRestoreDefault: TButton
        Left = 437
        Top = 459
        Width = 81
        Height = 27
        Caption = #24674#22797#40664#35748#20540
        TabOrder = 8
        OnClick = btnRestoreDefaultClick
      end
      object GroupBox18: TGroupBox
        Left = 17
        Top = 4
        Width = 183
        Height = 121
        Caption = #20154#29289#21917#33647#26102#38388#25511#21046' ('#27627#31186#65289
        TabOrder = 0
        object Label54: TLabel
          Left = 12
          Top = 22
          Width = 96
          Height = 12
          Caption = #25112#22763#26222#36890#33647#38388#38548#65306
        end
        object Label55: TLabel
          Left = 12
          Top = 74
          Width = 96
          Height = 12
          Caption = #36947#27861#26222#36890#33647#38388#38548#65306
        end
        object Label58: TLabel
          Left = 12
          Top = 45
          Width = 96
          Height = 12
          Caption = #25112#22763#29305#27530#33647#38388#38548#65306
        end
        object Label59: TLabel
          Left = 12
          Top = 98
          Width = 96
          Height = 12
          Caption = #36947#27861#29305#27530#33647#38388#38548#65306
        end
        object Bevel9: TBevel
          Left = 6
          Top = 62
          Width = 168
          Height = 5
          Shape = bsBottomLine
        end
        object seUseOrdinaryTime_Human_Warrior: TSpinEditEx
          Left = 110
          Top = 17
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seUseOrdinaryTime_Human_WarriorChange
        end
        object seUseOrdinaryTime_Human_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 70
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seUseOrdinaryTime_Human_TaoistAndWizardChange
        end
        object seUseSpecialTime_Human_Warrior: TSpinEditEx
          Left = 110
          Top = 40
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seUseSpecialTime_Human_WarriorChange
        end
        object seUseSpecialTime_Human_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 93
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seUseSpecialTime_Human_TaoistAndWizardChange
        end
      end
      object GroupBox19: TGroupBox
        Left = 219
        Top = 4
        Width = 183
        Height = 120
        Caption = #33521#38596#21917#33647#26102#38388#25511#21046' ('#27627#31186#65289
        TabOrder = 1
        object Label56: TLabel
          Left = 12
          Top = 22
          Width = 96
          Height = 12
          Caption = #25112#22763#26222#36890#33647#38388#38548#65306
        end
        object Label57: TLabel
          Left = 12
          Top = 75
          Width = 96
          Height = 12
          Caption = #36947#27861#26222#36890#33647#38388#38548#65306
        end
        object Label60: TLabel
          Left = 12
          Top = 45
          Width = 96
          Height = 12
          Caption = #25112#22763#29305#27530#33647#38388#38548#65306
        end
        object Label61: TLabel
          Left = 12
          Top = 98
          Width = 96
          Height = 12
          Caption = #36947#27861#29305#27530#33647#38388#38548#65306
        end
        object Bevel8: TBevel
          Left = 6
          Top = 62
          Width = 168
          Height = 5
          Shape = bsBottomLine
        end
        object seUseOrdinaryTime_Hero_Warrior: TSpinEditEx
          Left = 110
          Top = 17
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seUseOrdinaryTime_Hero_WarriorChange
        end
        object seUseOrdinaryTime_Hero_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 70
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seUseOrdinaryTime_Hero_TaoistAndWizardChange
        end
        object seUseSpecialTime_Hero_Warrior: TSpinEditEx
          Left = 110
          Top = 40
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seUseSpecialTime_Hero_WarriorChange
        end
        object seUseSpecialTime_Hero_TaoistAndWizard: TSpinEditEx
          Left = 110
          Top = 93
          Width = 58
          Height = 21
          Hint = #28216#25103#20013#20108#27425#21917#33647#38388#38548#26102#38388#65292#25968#20540#36234#23567#38388#38548#26102#38388#36234#30701
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seUseSpecialTime_Hero_TaoistAndWizardChange
        end
      end
      object GroupBox180: TGroupBox
        Left = 419
        Top = 4
        Width = 183
        Height = 138
        Caption = #25216#33021#24674#22797#36873#39033
        TabOrder = 2
        object Label569: TLabel
          Left = 12
          Top = 45
          Width = 96
          Height = 12
          Caption = #27835#24840#26415#24674#22797#36895#24230#65306
        end
        object Label64: TLabel
          Left = 12
          Top = 68
          Width = 96
          Height = 12
          Caption = #27835#24840#26415#24674#22797#28857#25968#65306
        end
        object Label67: TLabel
          Left = 12
          Top = 91
          Width = 96
          Height = 12
          Caption = #32676#30103#26415#24674#22797#36895#24230#65306
        end
        object Label69: TLabel
          Left = 12
          Top = 114
          Width = 96
          Height = 12
          Caption = #32676#30103#26415#24674#22797#28857#25968#65306
        end
        object Label97: TLabel
          Left = 12
          Top = 22
          Width = 96
          Height = 12
          Caption = #25216#33021#22238#34880#24635#38480#21046#65306
        end
        object sePerHealingTime: TSpinEditEx
          Left = 110
          Top = 40
          Width = 58
          Height = 21
          Hint = #24674#22797'hp'#38388#38548#25511#21046','#25968#23383#36234#23567#24674#22797#36234#24555','#26368#23567#20540#19981#21487#20302#20110'400'#13#10#40664#35748#20540'700,'#24674#22797#36895#24230#19982#20154#29289#31561#32423#26377#23494#20999#20851#31995','#31561#32423#36234#39640#24674#22797#36234#24555
          Increment = 10
          MaxValue = 2000
          MinValue = 400
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 400
          OnChange = sePerHealingTimeChange
        end
        object sePerHealing: TSpinEditEx
          Left = 110
          Top = 63
          Width = 58
          Height = 21
          Hint = #27599#27425#24674#22797#34880#37327#30340#28857#25968','#25968#23383#36234#22823#24674#22797'hp'#28857#25968#36234#39640','#40664#35748#20540#20026'5'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = sePerHealingChange
        end
        object seBigPerHealingTime: TSpinEditEx
          Left = 110
          Top = 86
          Width = 58
          Height = 21
          Hint = #24674#22797'hp'#38388#38548#25511#21046','#25968#23383#36234#23567#24674#22797#36234#24555','#26368#23567#20540#19981#21487#20302#20110'400'#13#10#40664#35748#20540'700,'#24674#22797#36895#24230#19982#20154#29289#31561#32423#26377#23494#20999#20851#31995','#31561#32423#36234#39640#24674#22797#36234#24555
          Increment = 10
          MaxValue = 2000
          MinValue = 400
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 400
          OnChange = seBigPerHealingTimeChange
        end
        object seBigPerHealing: TSpinEditEx
          Left = 110
          Top = 109
          Width = 58
          Height = 21
          Hint = #27599#27425#24674#22797#34880#37327#30340#28857#25968','#25968#23383#36234#22823#24674#22797'hp'#28857#25968#36234#39640','#40664#35748#20540#20026'5'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 5
          OnChange = seBigPerHealingChange
        end
        object seIncHealingLimite: TSpinEditEx
          Left = 110
          Top = 17
          Width = 58
          Height = 21
          Hint = #38480#23450#27835#24840#26415'/'#32676#30103#26415#22238#34880#24635#28857#25968
          Increment = 10
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 400
          OnChange = seIncHealingLimiteChange
        end
      end
      object GroupBox25: TGroupBox
        Left = 419
        Top = 148
        Width = 183
        Height = 69
        Caption = #20854#20182#38388#38548#35774#32622
        TabOrder = 5
        object Label14: TLabel
          Left = 10
          Top = 21
          Width = 108
          Height = 12
          Caption = #20854#23427#29289#21697#20351#29992#38388#38548#65306
        end
        object Label101: TLabel
          Left = 10
          Top = 44
          Width = 108
          Height = 12
          Caption = #25915#20987#29289#21697#20351#29992#38388#38548#65306
        end
        object seUseItemIntervalTime: TSpinEditEx
          Left = 111
          Top = 17
          Width = 57
          Height = 21
          Hint = #28216#25103#20013#20154#29289#20108#27425#20351#29992#29289#21697#38388#38548#26102#38388#65292#27492#21442#25968#40664#35748#20026' 500'#27627#31186#12290
          Increment = 10
          MaxValue = 2000
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 10
          OnChange = seUseItemIntervalTimeChange
        end
        object seUseAttackItemIntervalTime: TSpinEditEx
          Left = 111
          Top = 40
          Width = 57
          Height = 21
          Hint = #28216#25103#20013#20154#29289#20108#27425#20351#29992#29289#21697#38388#38548#26102#38388#65292#27492#21442#25968#40664#35748#20026' 500'#27627#31186#12290
          Increment = 10
          MaxValue = 2000
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 10
          OnChange = seUseAttackItemIntervalTimeChange
        end
      end
    end
    object tsOption2: TTabSheet
      Caption = #21151#33021#36873#39033'2'
      ImageIndex = 7
      object GroupBox20: TGroupBox
        Left = 12
        Top = 9
        Width = 150
        Height = 156
        Caption = #23567#22320#22270#25511#21046
        TabOrder = 0
        object Label63: TLabel
          Left = 11
          Top = 105
          Width = 36
          Height = 12
          Caption = #38378#28865#65306
        end
        object lbl3: TLabel
          Left = 11
          Top = 59
          Width = 72
          Height = 12
          Caption = #23567#22320#22270#31867#22411#65306
        end
        object chkMinMapCloseRadar: TCheckBox
          Left = 11
          Top = 37
          Width = 126
          Height = 18
          Hint = #36873#20013#35813#39033#65292#23567#22320#22270#20013#27809#26377#38647#36798#26174#31034
          Caption = #20851#38381#38647#36798#26174#31034
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkMinMapCloseRadarClick
        end
        object seMinMapFlagFlash: TSpinEditEx
          Left = 45
          Top = 100
          Width = 94
          Height = 21
          Hint = #29609#23478#33258#36523#38647#36798#38378#28865#39057#29575#65292#24403#20540#23567#20110'200'#27627#31186#26102#20851#38381#38378#28865#25928#26524
          MaxValue = 2000
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 300
          OnChange = seMinMapFlagFlashChange
        end
        object chkUseFindPath: TCheckBox
          Left = 11
          Top = 17
          Width = 126
          Height = 18
          Hint = #36873#20013#27492#39033#65306#24555#25463#38190' M '#21644' Tab '#38190#65292#24320#21551#22320#22270#23547#36335#21151#33021
          Caption = #23567#22320#22270#33258#21160#23547#36335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkUseFindPathClick
        end
        object cbbMinMapType: TComboBox
          Left = 11
          Top = 76
          Width = 129
          Height = 20
          Style = csDropDownList
          ItemIndex = 0
          TabOrder = 3
          Text = #40664#35748#23567#22320#22270
          OnChange = cbbMinMapTypeChange
          Items.Strings = (
            #40664#35748#23567#22320#22270
            #22797#21476#23567#22320#22270
            #20223#39029#28216#23567#22320#22270)
        end
        object chkMinMapUseFindPath: TCheckBox
          Left = 77
          Top = 56
          Width = 68
          Height = 17
          Hint = #40664#35748#12289#22797#21476#12289#20223#39029#28216#23567#22320#22270#25903#25345#23547#36335
          Caption = #25903#25345#23547#36335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = chkMinMapUseFindPathClick
        end
        object chkLoginShowMinMap: TCheckBox
          Left = 11
          Top = 127
          Width = 131
          Height = 17
          Caption = #36827#20837#28216#25103#25171#24320#23567#22320#22270
          TabOrder = 5
          OnClick = chkLoginShowMinMapClick
        end
      end
      object GroupBox21: TGroupBox
        Left = 12
        Top = 168
        Width = 147
        Height = 215
        Caption = #23567#22320#22270#38647#36798#39068#33394#25511#21046
        TabOrder = 2
        object Label65: TLabel
          Left = 12
          Top = 26
          Width = 60
          Height = 12
          Caption = #29609#23478#33258#36523#65306
        end
        object Label68: TLabel
          Left = 12
          Top = 53
          Width = 60
          Height = 12
          Caption = #20854#23427#29609#23478#65306
        end
        object Label66: TLabel
          Left = 42
          Top = 81
          Width = 30
          Height = 12
          Caption = 'NPC'#65306
        end
        object Label72: TLabel
          Left = 38
          Top = 135
          Width = 36
          Height = 12
          Caption = #23432#21355#65306
        end
        object Label74: TLabel
          Left = 38
          Top = 108
          Width = 36
          Height = 12
          Caption = #24618#29289#65306
        end
        object Label76: TLabel
          Left = 38
          Top = 163
          Width = 36
          Height = 12
          Caption = #33521#38596#65306
        end
        object lbl1: TLabel
          Left = 36
          Top = 190
          Width = 36
          Height = 12
          Caption = 'BOSS'#65306
        end
        object seMinMapColorSelf: TColorIndexEdit
          Left = 73
          Top = 23
          Width = 66
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 0
          Value = 100
          OnChange = seMinMapColorSelfChange
          ShowNoneColor = False
        end
        object seMinMapColorOther: TColorIndexEdit
          Left = 73
          Top = 50
          Width = 66
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seMinMapColorOtherChange
          ShowNoneColor = False
        end
        object seMinMapColorNPC: TColorIndexEdit
          Left = 73
          Top = 76
          Width = 66
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 2
          Value = 100
          OnChange = seMinMapColorNPCChange
          ShowNoneColor = False
        end
        object seMinMapColorGuard: TColorIndexEdit
          Left = 73
          Top = 131
          Width = 66
          Height = 21
          Hint = #22823#20992#12289#24339#31661#25163#12289#23553#39764#35895#30340#24694#39764#24339#31661#25163#12289#30333#26085#38376#30340#24102#20992#20365#21355
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 100
          OnChange = seMinMapColorGuardChange
          ShowNoneColor = False
        end
        object seMinMapColorMonster: TColorIndexEdit
          Left = 73
          Top = 104
          Width = 66
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          TabOrder = 3
          Value = 100
          OnChange = seMinMapColorMonsterChange
          ShowNoneColor = False
        end
        object seMinMapColorHero: TColorIndexEdit
          Left = 73
          Top = 158
          Width = 66
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          ParentShowHint = False
          ShowHint = False
          TabOrder = 5
          Value = 100
          OnChange = seMinMapColorHeroChange
          ShowNoneColor = False
        end
        object seMinMapColorBoss: TColorIndexEdit
          Left = 73
          Top = 185
          Width = 66
          Height = 21
          MaxLength = 3
          MaxValue = 255
          MinValue = 0
          ParentShowHint = False
          ShowHint = False
          TabOrder = 6
          Value = 100
          OnChange = seMinMapColorBossChange
          ShowNoneColor = False
        end
      end
      object btnSaveOption2: TButton
        Left = 520
        Top = 461
        Width = 79
        Height = 27
        Caption = #20445#23384'(&S)'
        TabOrder = 9
        OnClick = btnSaveOption2Click
      end
      object chkHideItemNameNum: TCheckBox
        Left = 171
        Top = 446
        Width = 154
        Height = 18
        Caption = #38544#34255#29289#21697#21517#21518#38754#30340#25968#23383
        TabOrder = 7
        Visible = False
        OnClick = chkHideItemNameNumClick
      end
      object chkKeyTabGetActor: TCheckBox
        Left = 171
        Top = 413
        Width = 104
        Height = 18
        Caption = 'TAB'#38190#33719#21462#35282#33394
        TabOrder = 6
        OnClick = chkKeyTabGetActorClick
      end
      object chkMagicSetDir: TCheckBox
        Left = 171
        Top = 429
        Width = 105
        Height = 19
        Hint = #37322#25918#39764#27861#26102#65292#20154#29289#38754#21521#26045#23637#30446#26631
        Caption = #39764#27861#36716#21521
        ParentShowHint = False
        ShowHint = True
        TabOrder = 8
        OnClick = chkMagicSetDirClick
      end
      object grp6: TGroupBox
        Left = 173
        Top = 9
        Width = 177
        Height = 156
        Caption = #39128#34880#36873#39033
        TabOrder = 1
        object lbl7: TLabel
          Left = 13
          Top = 110
          Width = 48
          Height = 12
          Caption = #20301#32622#20559#31227
        end
        object lbl10: TLabel
          Left = 63
          Top = 110
          Width = 6
          Height = 12
          Caption = 'X'
        end
        object Label116: TLabel
          Left = 119
          Top = 110
          Width = 6
          Height = 12
          Caption = 'Y'
        end
        object Label117: TLabel
          Left = 13
          Top = 134
          Width = 48
          Height = 12
          Caption = #31227#21160#36895#24230
        end
        object lbl11: TLabel
          Left = 141
          Top = 134
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object chkHealthNumberText: TCheckBox
          Left = 12
          Top = 15
          Width = 153
          Height = 19
          Hint = #21246#36873#21518#29992#28129#20986#26041#24335#32472#21046#39128#34880#65292#19981#21246#21017#20445#25345#40664#35748#22270#29255#32472#21046#27169#24335
          Caption = #39128#34880#20351#29992#28129#20986#32472#21046#27169#24335
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkHealthNumberTextClick
        end
        object chkBlastHitShowHealthNum: TCheckBox
          Left = 12
          Top = 34
          Width = 153
          Height = 17
          Hint = #35813#36873#39033#20165#22312#21246#36873#28129#20986#32472#21046#39128#34880#27169#24335#21518#29983#25928#65292#21246#36873#21518#26222#36890#20260#23475#19981#20877#39128#34880#20165#26292#20987#39128#34880
          Caption = #20165#26292#20987#20260#23475#26102#26174#31034#39128#34880
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkBlastHitShowHealthNumClick
        end
        object chkPoisoningHideHealthNum: TCheckBox
          Left = 12
          Top = 51
          Width = 153
          Height = 18
          Caption = #27602#25481#30340#34880#19981#39128
          ParentShowHint = False
          ShowHint = False
          TabOrder = 2
          OnClick = chkPoisoningHideHealthNumClick
        end
        object chkHPStoneHideHealthNum: TCheckBox
          Left = 12
          Top = 69
          Width = 153
          Height = 18
          Hint = #27492#36873#39033#21482#36866#29992#20110#26222#36890#39128#34880#32472#21046#27169#24335
          Caption = #27668#34880#30707#12289#39764#34880#30707#19981#39128#34880
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = chkHPStoneHideHealthNumClick
        end
        object chkMPStoneHideHealthNum: TCheckBox
          Left = 12
          Top = 87
          Width = 153
          Height = 18
          Hint = #27492#36873#39033#21482#36866#29992#20110#26222#36890#39128#34880#32472#21046#27169#24335
          Caption = #39764#24187#30707#12289#39764#34880#30707#19981#39128#34013
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          OnClick = chkMPStoneHideHealthNumClick
        end
        object seHealthNumberOffsetX: TSpinEdit
          Left = 73
          Top = 106
          Width = 40
          Height = 21
          MaxValue = 2000
          MinValue = -2000
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 300
          OnChange = seHealthNumberOffsetXChange
        end
        object seHealthNumberOffsetY: TSpinEdit
          Left = 129
          Top = 106
          Width = 40
          Height = 21
          MaxValue = 2000
          MinValue = -2000
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 300
          OnChange = seHealthNumberOffsetYChange
        end
        object seHealthNumberMoveSpeed: TSpinEditEx
          Left = 73
          Top = 129
          Width = 64
          Height = 21
          MaxValue = 2000
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 300
          OnChange = seHealthNumberMoveSpeedChange
        end
      end
      object grp5: TGroupBox
        Left = 12
        Top = 392
        Width = 147
        Height = 68
        Caption = #24038#20391#32452#38431#20449#24687#22352#26631#20559#31227
        TabOrder = 5
        object Label84: TLabel
          Left = 15
          Top = 21
          Width = 66
          Height = 12
          Caption = 'X'#22352#26631#20559#31227#65306
        end
        object Label85: TLabel
          Left = 15
          Top = 44
          Width = 66
          Height = 12
          Caption = 'Y'#22352#26631#20559#31227#65306
        end
        object seNewLeftGroupInfoOffsetX: TSpinEditEx
          Left = 79
          Top = 16
          Width = 60
          Height = 21
          Hint = #23567#36864#29983#25928
          MaxValue = 2000
          MinValue = -2000
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 300
          OnChange = seNewLeftGroupInfoOffsetXChange
        end
        object seNewLeftGroupInfoOffsetY: TSpinEditEx
          Left = 79
          Top = 39
          Width = 60
          Height = 21
          Hint = #23567#36864#29983#25928
          MaxValue = 2000
          MinValue = -2000
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 300
          OnChange = seNewLeftGroupInfoOffsetYChange
        end
      end
      object GroupBox27: TGroupBox
        Left = 173
        Top = 168
        Width = 177
        Height = 143
        Caption = #34880#26465#26174#31034#35774#32622
        TabOrder = 3
        object Label89: TLabel
          Left = 11
          Top = 70
          Width = 42
          Height = 12
          Caption = #20154#29289' X:'
        end
        object Label90: TLabel
          Left = 108
          Top = 70
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object lbl14: TLabel
          Left = 8
          Top = 48
          Width = 84
          Height = 12
          Caption = #34880#26465#22352#26631#20559#31227#65306
        end
        object bvl1: TBevel
          Left = 10
          Top = 41
          Width = 157
          Height = 3
          Shape = bsTopLine
        end
        object Label91: TLabel
          Left = 17
          Top = 94
          Width = 36
          Height = 12
          Caption = 'NPC X:'
        end
        object Label92: TLabel
          Left = 108
          Top = 94
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object Label93: TLabel
          Left = 11
          Top = 118
          Width = 42
          Height = 12
          Caption = #24618#29289' X:'
        end
        object Label94: TLabel
          Left = 108
          Top = 118
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object chkShowMagicShieldHP: TCheckBox
          Left = 12
          Top = 18
          Width = 105
          Height = 18
          Caption = #26174#31034#25252#36523#34880#26465
          TabOrder = 0
          OnClick = chkShowMagicShieldHPClick
        end
        object seHumHPBarOffsetX: TSpinEditEx
          Left = 55
          Top = 65
          Width = 46
          Height = 21
          Hint = #21547#20154#29289#12289#33521#38596#21450#20154#24418#24618
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 300
          OnChange = seHumHPBarOffsetXChange
        end
        object seHumHPBarOffsetY: TSpinEditEx
          Left = 123
          Top = 65
          Width = 46
          Height = 21
          Hint = #21547#20154#29289#12289#33521#38596#21450#20154#24418#24618
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 300
          OnChange = seHumHPBarOffsetYChange
        end
        object seNpcHPBarOffsetX: TSpinEditEx
          Left = 55
          Top = 89
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 300
          OnChange = seNpcHPBarOffsetXChange
        end
        object seNpcHPBarOffsetY: TSpinEditEx
          Left = 123
          Top = 89
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 300
          OnChange = seNpcHPBarOffsetYChange
        end
        object seMonHPBarOffsetX: TSpinEditEx
          Left = 55
          Top = 113
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 300
          OnChange = seMonHPBarOffsetXChange
        end
        object seMonHPBarOffsetY: TSpinEditEx
          Left = 123
          Top = 113
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 300
          OnChange = seMonHPBarOffsetYChange
        end
      end
      object GroupBox28: TGroupBox
        Left = 173
        Top = 316
        Width = 177
        Height = 93
        Caption = #21517#23383#26174#31034#20559#31227
        TabOrder = 4
        object Label95: TLabel
          Left = 11
          Top = 22
          Width = 42
          Height = 12
          Caption = #20154#29289' X:'
        end
        object Label96: TLabel
          Left = 108
          Top = 22
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object Label98: TLabel
          Left = 17
          Top = 46
          Width = 36
          Height = 12
          Caption = 'NPC X:'
        end
        object Label99: TLabel
          Left = 108
          Top = 46
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object Label102: TLabel
          Left = 11
          Top = 70
          Width = 42
          Height = 12
          Caption = #24618#29289' X:'
        end
        object Label103: TLabel
          Left = 108
          Top = 70
          Width = 12
          Height = 12
          Caption = 'Y:'
        end
        object seHumNameOffsetX: TSpinEditEx
          Left = 55
          Top = 17
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 300
          OnChange = seHumNameOffsetXChange
        end
        object seHumNameOffsetY: TSpinEditEx
          Left = 123
          Top = 17
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 300
          OnChange = seHumNameOffsetYChange
        end
        object seNpcNameOffsetX: TSpinEditEx
          Left = 55
          Top = 41
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 300
          OnChange = seNpcNameOffsetXChange
        end
        object seNpcNameOffsetY: TSpinEditEx
          Left = 123
          Top = 41
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 300
          OnChange = seNpcNameOffsetYChange
        end
        object seMonNameOffsetX: TSpinEditEx
          Left = 55
          Top = 65
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 300
          OnChange = seMonNameOffsetXChange
        end
        object seMonNameOffsetY: TSpinEditEx
          Left = 123
          Top = 65
          Width = 46
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 300
          OnChange = seMonNameOffsetYChange
        end
      end
      object GroupBox30: TGroupBox
        Left = 361
        Top = 9
        Width = 231
        Height = 111
        Caption = #22352#26631#35774#32622
        TabOrder = 10
        object Label11: TLabel
          Left = 9
          Top = 131
          Width = 12
          Height = 12
          Caption = 'X:'
          Visible = False
        end
        object Label80: TLabel
          Left = 89
          Top = 131
          Width = 12
          Height = 12
          Caption = 'Y:'
          Visible = False
        end
        object Label81: TLabel
          Left = 9
          Top = 173
          Width = 12
          Height = 12
          Caption = 'X:'
          Visible = False
        end
        object Label82: TLabel
          Left = 89
          Top = 173
          Width = 12
          Height = 12
          Caption = 'Y:'
          Visible = False
        end
        object Label83: TLabel
          Left = 9
          Top = 215
          Width = 12
          Height = 12
          Caption = 'X:'
          Visible = False
        end
        object Label115: TLabel
          Left = 89
          Top = 215
          Width = 12
          Height = 12
          Caption = 'Y:'
          Visible = False
        end
        object Label118: TLabel
          Left = 9
          Top = 258
          Width = 12
          Height = 12
          Caption = 'X:'
          Visible = False
        end
        object Label119: TLabel
          Left = 89
          Top = 258
          Width = 12
          Height = 12
          Caption = 'Y:'
          Visible = False
        end
        object Label120: TLabel
          Left = 9
          Top = 300
          Width = 12
          Height = 12
          Caption = 'X:'
          Visible = False
        end
        object Label121: TLabel
          Left = 89
          Top = 300
          Width = 12
          Height = 12
          Caption = 'Y:'
          Visible = False
        end
        object Label122: TLabel
          Left = 9
          Top = 326
          Width = 30
          Height = 12
          Caption = #22320#22270':'
          Visible = False
        end
        object Label123: TLabel
          Left = 97
          Top = 326
          Width = 18
          Height = 12
          Caption = 'UI:'
          Visible = False
        end
        object Label124: TLabel
          Left = 16
          Top = 110
          Width = 96
          Height = 12
          Caption = #26356#22909#30340#35013#22791#22352#26631#65306
          Visible = False
        end
        object Label125: TLabel
          Left = 16
          Top = 153
          Width = 84
          Height = 12
          Caption = #30446#26631#20449#24687#22352#26631#65306
          Visible = False
        end
        object Label126: TLabel
          Left = 16
          Top = 195
          Width = 60
          Height = 12
          Caption = #25671#26438#22352#26631#65306
          Visible = False
        end
        object Label127: TLabel
          Left = 16
          Top = 238
          Width = 60
          Height = 12
          Caption = #25671#26438#33539#22260#65306
          Visible = False
        end
        object Label128: TLabel
          Left = 16
          Top = 278
          Width = 84
          Height = 12
          Caption = #25216#33021#25353#38062#22352#26631#65306
          Visible = False
        end
        object Label129: TLabel
          Left = 27
          Top = 357
          Width = 72
          Height = 12
          Caption = #40664#35748#32553#25918#20540#65306
          Visible = False
        end
        object Label130: TLabel
          Left = 20
          Top = 60
          Width = 114
          Height = 12
          Caption = #22810#21151#33021#31383#21475#25628#32034#33539#22260':'
        end
        object seBetterItemX: TSpinEdit
          Left = 27
          Top = 127
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 0
          Value = 0
          Visible = False
          OnChange = seBetterItemXChange
        end
        object seBetterItemY: TSpinEdit
          Left = 107
          Top = 127
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 1
          Value = 0
          Visible = False
          OnChange = seBetterItemYChange
        end
        object seSmallInfoX: TSpinEdit
          Left = 27
          Top = 169
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 2
          Value = 0
          Visible = False
          OnChange = seSmallInfoXChange
        end
        object seSmallInfoY: TSpinEdit
          Left = 107
          Top = 169
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 3
          Value = 0
          Visible = False
          OnChange = seSmallInfoYChange
        end
        object seJoyStickX: TSpinEdit
          Left = 27
          Top = 211
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 4
          Value = 0
          Visible = False
          OnChange = seJoyStickXChange
        end
        object seJoyStickY: TSpinEdit
          Left = 107
          Top = 211
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 5
          Value = 0
          Visible = False
          OnChange = seJoyStickYChange
        end
        object seJoyStickMaxX: TSpinEdit
          Left = 27
          Top = 254
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 6
          Value = 0
          Visible = False
          OnChange = seJoyStickMaxXChange
        end
        object seJoyStickMaxY: TSpinEdit
          Left = 107
          Top = 254
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 7
          Value = 0
          Visible = False
          OnChange = seJoyStickMaxYChange
        end
        object seSkillCtrX: TSpinEdit
          Left = 27
          Top = 295
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 8
          Value = 0
          Visible = False
          OnChange = seSkillCtrXChange
        end
        object seSkillCtrY: TSpinEdit
          Left = 107
          Top = 296
          Width = 49
          Height = 21
          MaxValue = 0
          MinValue = 0
          TabOrder = 9
          Value = 0
          Visible = False
          OnChange = seSkillCtrYChange
        end
        object seMapScale: TSpinEdit
          Left = 43
          Top = 322
          Width = 49
          Height = 21
          MaxValue = 23
          MinValue = 7
          TabOrder = 10
          Value = 7
          Visible = False
          OnChange = seMapScaleChange
        end
        object seGuiScale: TSpinEdit
          Left = 115
          Top = 322
          Width = 49
          Height = 21
          MaxValue = 20
          MinValue = 10
          TabOrder = 11
          Value = 10
          Visible = False
          OnChange = seGuiScaleChange
        end
        object chkShowExSkillIcon: TCheckBox
          Left = 19
          Top = 19
          Width = 126
          Height = 17
          Caption = #26159#21542#26174#31034#25216#33021#25353#38062
          TabOrder = 12
          OnClick = chkShowExSkillIconClick
        end
        object chkShowMulitDlg: TCheckBox
          Left = 19
          Top = 38
          Width = 126
          Height = 17
          Caption = #26159#21542#26174#31034#22810#21151#33021#31383#21475
          TabOrder = 13
          OnClick = chkShowMulitDlgClick
        end
        object seMultiViewRange: TSpinEdit
          Left = 140
          Top = 57
          Width = 49
          Height = 21
          MaxValue = 20
          MinValue = 10
          TabOrder = 14
          Value = 10
          OnChange = seMultiViewRangeChange
        end
        object chkShowBetterItem: TCheckBox
          Left = 19
          Top = 76
          Width = 126
          Height = 17
          Caption = #26159#21542#26174#31034#26356#22909#30340#35013#22791
          TabOrder = 15
          OnClick = chkShowBetterItemClick
        end
      end
    end
  end
end
