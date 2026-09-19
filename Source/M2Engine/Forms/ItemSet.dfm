object frmItemSet: TfrmItemSet
  Left = 521
  Top = 247
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #29305#27530#23646#24615#29289#21697#35774#32622
  ClientHeight = 367
  ClientWidth = 478
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  ShowHint = True
  TextHeight = 12
  object PageControl: TPageControl
    Left = 8
    Top = 8
    Width = 465
    Height = 353
    ActivePage = TabSheet8
    TabOrder = 0
    object TabSheet8: TTabSheet
      Caption = #29305#27530#23646#24615
      object ItemSetPageControl: TPageControl
        Left = 5
        Top = 4
        Width = 447
        Height = 287
        ActivePage = tsBase
        TabOrder = 0
        object tsBase: TTabSheet
          Caption = #22522#26412#36873#39033
          ImageIndex = 5
          object grp25: TGroupBox
            Left = 5
            Top = 2
            Width = 173
            Height = 127
            Caption = #22522#26412#35774#32622
            TabOrder = 0
            object Label209: TLabel
              Left = 8
              Top = 58
              Width = 120
              Height = 12
              Caption = #30456#21516#23646#24615#38262#23884#38480#21046#25968#37327
            end
            object Label211: TLabel
              Left = 8
              Top = 80
              Width = 120
              Height = 12
              Caption = #30456#21516#23453#30707#38262#23884#38480#21046#25968#37327
            end
            object Label219: TLabel
              Left = 8
              Top = 102
              Width = 120
              Height = 12
              Caption = #21333#23380#30456#21516#23453#30707#21472#21152#25968#37327
            end
            object chkOpenItemFlute: TCheckBox
              Left = 8
              Top = 17
              Width = 97
              Height = 17
              Caption = #24320#21551#20985#27133#21151#33021
              TabOrder = 0
              OnClick = chkOpenItemFluteClick
            end
            object seItemFluteStoneCount: TSpinEditEx
              Left = 131
              Top = 54
              Width = 36
              Height = 21
              Hint = #38480#21046#35013#22791#38262#23884#21516#19968#24120#35268#23646#24615#30340#23453#30707#26368#22823#25968#37327#13#10#20026'0'#26102#34920#31034#19981#38480#21046
              MaxValue = 10
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 2
              Value = 10
              OnChange = seItemFluteStoneCountChange
            end
            object chkDisableRightClickFluteStone: TCheckBox
              Left = 8
              Top = 37
              Width = 145
              Height = 17
              Caption = #31105#27490#40736#26631#21491#38190#38262#23884#23453#30707
              TabOrder = 1
              OnClick = chkDisableRightClickFluteStoneClick
            end
            object seItemFluteStoneIdxCount: TSpinEditEx
              Left = 131
              Top = 76
              Width = 36
              Height = 21
              Hint = #38480#21046#35013#22791#38262#23884#21516#19968'idx'#23453#30707#26368#22823#25968#37327#13#10#20026'0'#26102#34920#31034#19981#38480#21046
              MaxValue = 10
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 3
              Value = 10
              OnChange = seItemFluteStoneIdxCountChange
            end
            object seItemFluteStoneOverlapCount: TSpinEditEx
              Left = 131
              Top = 98
              Width = 36
              Height = 21
              Hint = #38480#21046#35013#22791#38262#23884#21516#19968#23380#20801#35768#30456#21516'Idx'#23453#30707#21472#21152#25968#37327#13#10#20026'0'#25110'1'#26102#34920#31034#19981#20801#35768#21472#21152
              MaxValue = 10
              MinValue = 0
              ParentShowHint = False
              ShowHint = True
              TabOrder = 4
              Value = 10
              OnChange = seItemFluteStoneOverlapCountChange
            end
          end
        end
        object TabSheet1: TTabSheet
          Caption = #32463#39564#32763#20493
          object GroupBox141: TGroupBox
            Left = 8
            Top = 8
            Width = 369
            Height = 161
            Caption = #32463#39564#32763#20493
            TabOrder = 0
            object Label108: TLabel
              Left = 11
              Top = 24
              Width = 30
              Height = 12
              Caption = #20493#29575':'
            end
            object Label109: TLabel
              Left = 8
              Top = 104
              Width = 353
              Height = 49
              AutoSize = False
              Caption = 
                #20493#29575#20197#25345#20037#20026#26631#20934#65292#38500#20197#35774#23450#20540#65292#20026#27491#30495#30340#20493#29575#65292#29289#21697#26368#39640#25345#20037#20026'65'#65292#20063#23601#26159'65000'#28857#65292#20197#27492#25345#20037#26469#31639#38500#20197#35774#32622#30340#25968#23383#23601#26159#20493#25968#20102#65292#22914#26524#35774 +
                #32622#20026'10000'#65292#21017#20026' 6.5'#20493#32463#39564#12290#22914#26524#36523#19978#24102#20102#22810#20010#27492#23646#24615#35013#22791#65292#20493#29575#26159#32047#21152#30340#12290
              Font.Charset = ANSI_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
              WordWrap = True
            end
            object EditItemExpRate: TSpinEditEx
              Left = 56
              Top = 20
              Width = 57
              Height = 21
              MaxValue = 60000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditItemExpRateChange
            end
            object GroupBox1: TGroupBox
              Left = 168
              Top = 16
              Width = 193
              Height = 81
              Caption = #25968#25454#24211#35774#32622#32534#21495' [141, 182]'
              TabOrder = 1
              object Label1: TLabel
                Left = 8
                Top = 16
                Width = 180
                Height = 12
                Caption = #27494#22120#12289#34593#28891#31867#20351#29992#23383#27573': AniCount'
              end
              object Label2: TLabel
                Left = 8
                Top = 32
                Width = 126
                Height = 12
                Caption = #39318#39280#31867#20351#29992#23383#27573': Shape'
              end
            end
          end
        end
        object TabSheet2: TTabSheet
          Caption = #25915#20987#32763#20493
          ImageIndex = 1
          object GroupBox142: TGroupBox
            Left = 8
            Top = 8
            Width = 369
            Height = 161
            Caption = #25915#20987#32763#20493
            TabOrder = 0
            object Label110: TLabel
              Left = 11
              Top = 24
              Width = 30
              Height = 12
              Caption = #20493#29575':'
            end
            object Label3: TLabel
              Left = 8
              Top = 104
              Width = 353
              Height = 49
              AutoSize = False
              Caption = 
                #20493#29575#20197#25345#20037#20026#26631#20934#65292#38500#20197#35774#23450#20540#65292#20026#27491#30495#30340#20493#29575#65292#29289#21697#26368#39640#25345#20037#20026'65'#65292#20063#23601#26159'65000'#28857#65292#20197#27492#25345#20037#26469#31639#38500#20197#35774#32622#30340#25968#23383#23601#26159#20493#25968#20102#65292#22914#26524#35774 +
                #32622#20026'10000'#65292#21017#20026' 6.5'#20493#32463#39564#12290#22914#26524#36523#19978#24102#20102#22810#20010#27492#23646#24615#35013#22791#65292#20493#29575#26159#32047#21152#30340#12290
              Font.Charset = ANSI_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
              WordWrap = True
            end
            object EditItemPowerRate: TSpinEditEx
              Left = 56
              Top = 20
              Width = 57
              Height = 21
              MaxValue = 60000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditItemPowerRateChange
            end
            object GroupBox2: TGroupBox
              Left = 168
              Top = 16
              Width = 193
              Height = 81
              Caption = #25968#25454#24211#35774#32622#32534#21495' [142, 183]'
              TabOrder = 1
              object Label4: TLabel
                Left = 8
                Top = 16
                Width = 180
                Height = 12
                Caption = #27494#22120#12289#34593#28891#31867#20351#29992#23383#27573': AniCount'
              end
              object Label5: TLabel
                Left = 8
                Top = 32
                Width = 126
                Height = 12
                Caption = #39318#39280#31867#20351#29992#23383#27573': Shape'
              end
            end
          end
        end
        object TabSheet4: TTabSheet
          Caption = #34892#20250#20256#36865
          ImageIndex = 3
          object GroupBox28: TGroupBox
            Left = 8
            Top = 8
            Width = 369
            Height = 161
            Caption = #34892#20250#20256#36865
            TabOrder = 0
            object Label85: TLabel
              Left = 11
              Top = 24
              Width = 54
              Height = 12
              Caption = #20351#29992#38388#38548':'
            end
            object Label86: TLabel
              Left = 8
              Top = 104
              Width = 353
              Height = 49
              AutoSize = False
              Caption = #34892#20250#20256#36865#29289#21697#65292#34892#20250#25484#38376#20154#25165#33021#20351#29992#65292#23558#25972#20010#34892#20250#25104#21592#20840#37096#38598#20013#20110#20256#36865#25484#38376#20154#36523#36793#12290#34987#20256#36865#25104#21592#65292#24517#39035#20351#29992#21629#20196#20801#35768#34892#20250#20256#36865#12290
              Font.Charset = ANSI_CHARSET
              Font.Color = clWindowText
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
              WordWrap = True
            end
            object EditGuildRecallTime: TSpinEditEx
              Left = 72
              Top = 20
              Width = 57
              Height = 21
              Hint = #37325#22797#20351#29992#27492#21151#33021#65292#25152#38656#38388#38548#26102#38388#12290#27492#35774#32622#20462#25913#21518#19981#33021#31435#21363#29983#25928#65292#38656#22312#19979#27425#20351#29992#26102#29983#25928#12290
              MaxValue = 60000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditGuildRecallTimeChange
            end
            object GroupBox29: TGroupBox
              Left = 168
              Top = 16
              Width = 193
              Height = 81
              Caption = #25968#25454#24211#35774#32622#32534#21495' [145]'
              TabOrder = 1
              object Label87: TLabel
                Left = 8
                Top = 16
                Width = 180
                Height = 12
                Caption = #27494#22120#12289#34593#28891#31867#20351#29992#23383#27573': AniCount'
              end
              object Label88: TLabel
                Left = 8
                Top = 32
                Width = 126
                Height = 12
                Caption = #39318#39280#31867#20351#29992#23383#27573': Shape'
              end
            end
          end
        end
        object TabSheet5: TTabSheet
          Caption = #29366#24577#25915#20987
          ImageIndex = 4
          object Label196: TLabel
            Left = 7
            Top = 177
            Width = 318
            Height = 12
            Caption = #27494#22120#12289#34593#28891#31867#20351#29992#23383#27573': AniCount'#65307#39318#39280#31867#20351#29992#23383#27573': Shape'
          end
          object GroupBox42: TGroupBox
            Left = 5
            Top = 2
            Width = 185
            Height = 68
            Caption = #40635#30201' - '#25968#25454#24211#32534#21495' [113]'
            TabOrder = 0
            object Label120: TLabel
              Left = 11
              Top = 21
              Width = 54
              Height = 12
              Caption = #40635#30201#26426#29575':'
            end
            object Label116: TLabel
              Left = 11
              Top = 44
              Width = 54
              Height = 12
              Caption = #40635#30201#26102#38388':'
            end
            object Label124: TLabel
              Left = 131
              Top = 44
              Width = 12
              Height = 12
              Caption = #31186
            end
            object EditAttackPosionRate: TSpinEditEx
              Left = 72
              Top = 17
              Width = 49
              Height = 21
              Hint = #40635#30201#25104#21151#26426#29575#65292#25968#23383#36234#23567#26426#29575#36234#22823#65292#27492#35774#32622#40664#35748#20026'5'
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditAttackPosionRateChange
            end
            object EditAttackPosionTime: TSpinEditEx
              Left = 72
              Top = 40
              Width = 49
              Height = 21
              Hint = #40635#30201#26102#38388#38271#24230#65292#21333#20301#31186#65292#40664#35748#35774#32622#20026'6'
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditAttackPosionTimeChange
            end
          end
          object grpMD: TGroupBox
            Left = 196
            Top = 2
            Width = 185
            Height = 68
            Caption = #39764#36947#40635#30201' - '#25968#25454#24211#32534#21495' [204]'
            TabOrder = 1
            object Label193: TLabel
              Left = 11
              Top = 21
              Width = 54
              Height = 12
              Caption = #40635#30201#26426#29575':'
            end
            object Label194: TLabel
              Left = 11
              Top = 44
              Width = 54
              Height = 12
              Caption = #40635#30201#26102#38388':'
            end
            object Label195: TLabel
              Left = 131
              Top = 44
              Width = 12
              Height = 12
              Caption = #31186
            end
            object seMDParalysisRate: TSpinEditEx
              Left = 72
              Top = 17
              Width = 49
              Height = 21
              Hint = #40635#30201#25104#21151#26426#29575#65292#25968#23383#36234#23567#26426#29575#36234#22823#65292#27492#35774#32622#40664#35748#20026'5 '#65288#20165#29992#20110#39764#27861#25915#20987#65289
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seMDParalysisRateChange
            end
            object seMDParalysisTime: TSpinEditEx
              Left = 72
              Top = 40
              Width = 49
              Height = 21
              Hint = #40635#30201#26102#38388#38271#24230#65292#21333#20301#31186#65292#40664#35748#35774#32622#20026'6'
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seMDParalysisTimeChange
            end
          end
          object grpBD: TGroupBox
            Left = 5
            Top = 79
            Width = 185
            Height = 92
            Caption = #20912#20923' - '#25968#25454#24211#32534#21495' [205]'
            TabOrder = 2
            object Label122: TLabel
              Left = 11
              Top = 21
              Width = 54
              Height = 12
              Caption = #20912#20923#26426#29575':'
            end
            object Label123: TLabel
              Left = 11
              Top = 44
              Width = 54
              Height = 12
              Caption = #20912#20923#26102#38388':'
            end
            object Label197: TLabel
              Left = 131
              Top = 44
              Width = 12
              Height = 12
              Caption = #31186
            end
            object seFrozenRate: TSpinEditEx
              Left = 72
              Top = 17
              Width = 49
              Height = 21
              Hint = #20912#20923#25104#21151#26426#29575#65292#25968#23383#36234#23567#26426#29575#36234#22823#65292#27492#35774#32622#40664#35748#20026'5'
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFrozenRateChange
            end
            object seFrozenTime: TSpinEditEx
              Left = 72
              Top = 40
              Width = 49
              Height = 21
              Hint = #20912#20923#26102#38388#38271#24230#65292#21333#20301#31186#65292#40664#35748#35774#32622#20026'6'
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFrozenTimeChange
            end
            object chkFrozenUseMagicStruck: TCheckBox
              Left = 10
              Top = 67
              Width = 97
              Height = 17
              Hint = #19981#21246#36873#26102#65292#20912#20923#21482#23545#29289#29702#25915#20987#26377#25928#65292#21246#36873#21017#23545#29289#29702#21644#39764#27861#25915#20987#37117#26377#25928
              Caption = #39764#27861#25915#20987#26377#25928
              TabOrder = 2
              OnClick = chkFrozenUseMagicStruckClick
            end
          end
          object grpZW: TGroupBox
            Left = 196
            Top = 79
            Width = 185
            Height = 91
            Caption = #34584#34523#32593' - '#25968#25454#24211#32534#21495' [206]'
            TabOrder = 3
            object Label198: TLabel
              Left = 11
              Top = 21
              Width = 54
              Height = 12
              Caption = #34523#32593#26426#29575':'
            end
            object Label199: TLabel
              Left = 11
              Top = 44
              Width = 54
              Height = 12
              Caption = #34523#32593#26102#38388':'
            end
            object Label200: TLabel
              Left = 131
              Top = 44
              Width = 12
              Height = 12
              Caption = #31186
            end
            object seCobwebWindingRate: TSpinEditEx
              Left = 72
              Top = 17
              Width = 49
              Height = 21
              Hint = #20013#34584#34523#25104#21151#26426#29575#65292#25968#23383#36234#23567#26426#29575#36234#22823#65292#27492#35774#32622#40664#35748#20026'5'
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seCobwebWindingRateChange
            end
            object seCobwebWindingTime: TSpinEditEx
              Left = 72
              Top = 40
              Width = 49
              Height = 21
              Hint = #20013#34584#34523#32593#26102#38388#38271#24230#65292#21333#20301#31186#65292#40664#35748#35774#32622#20026'6'
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seCobwebWindingTimeChange
            end
            object chkCobwebWindingUseMagicStruck: TCheckBox
              Left = 10
              Top = 67
              Width = 97
              Height = 17
              Hint = #19981#21246#36873#26102#65292#34584#34523#32593#21482#23545#29289#29702#25915#20987#26377#25928#65292#21246#36873#21017#23545#29289#29702#21644#39764#27861#25915#20987#37117#26377#25928
              Caption = #39764#27861#25915#20987#26377#25928
              TabOrder = 2
              OnClick = chkCobwebWindingUseMagicStruckClick
            end
          end
        end
        object TabSheet6: TTabSheet
          Caption = #20256#36865
          ImageIndex = 5
          object GroupBox43: TGroupBox
            Left = 8
            Top = 8
            Width = 369
            Height = 161
            Caption = #20256#36865
            TabOrder = 0
            object GroupBox46: TGroupBox
              Left = 168
              Top = 16
              Width = 193
              Height = 81
              Caption = #25968#25454#24211#35774#32622#32534#21495' [112]'
              TabOrder = 0
              object Label117: TLabel
                Left = 8
                Top = 16
                Width = 180
                Height = 12
                Caption = #27494#22120#12289#34593#28891#31867#20351#29992#23383#27573': AniCount'
              end
              object Label118: TLabel
                Left = 8
                Top = 32
                Width = 126
                Height = 12
                Caption = #39318#39280#31867#20351#29992#23383#27573': Shape'
              end
            end
            object GroupBox47: TGroupBox
              Left = 8
              Top = 16
              Width = 153
              Height = 81
              Caption = #21442#25968
              TabOrder = 1
              object Label119: TLabel
                Left = 11
                Top = 56
                Width = 54
                Height = 12
                Caption = #20351#29992#38388#38548':'
              end
              object Label121: TLabel
                Left = 123
                Top = 56
                Width = 12
                Height = 12
                Caption = #31186
              end
              object CheckBoxUserMoveCanDupObj: TCheckBox
                Left = 8
                Top = 16
                Width = 137
                Height = 17
                Hint = #20851#38381#27492#36873#39033#21518#65292#20256#36865#24231#26631#19978#26377#35282#33394#26102#23558#19981#20801#35768#20256#36865
                Caption = #20801#35768#20256#36865#35282#33394#37325#21472
                TabOrder = 0
                OnClick = CheckBoxUserMoveCanDupObjClick
              end
              object CheckBoxUserMoveCanOnItem: TCheckBox
                Left = 8
                Top = 32
                Width = 137
                Height = 17
                Hint = #20851#38381#27492#36873#39033#21518#65292#20256#36865#24231#26631#19978#26377#29289#21697#26102#23558#19981#20801#35768#20256#36865
                Caption = #20801#35768#20256#36865#29289#21697#37325#21472
                TabOrder = 1
                OnClick = CheckBoxUserMoveCanOnItemClick
              end
              object EditUserMoveTime: TSpinEditEx
                Left = 72
                Top = 52
                Width = 49
                Height = 21
                Hint = #20256#36865#21629#20196#20351#29992#38388#38548#26102#38388
                MaxValue = 100
                MinValue = 1
                TabOrder = 2
                Value = 100
                OnChange = EditUserMoveTimeChange
              end
            end
          end
        end
      end
      object ButtonItemSetSave: TButton
        Left = 387
        Top = 298
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonItemSetSaveClick
      end
    end
    object TabSheet19: TTabSheet
      Caption = #31070#31192#22871#35013
      ImageIndex = 2
      object PageControl1: TPageControl
        Left = 4
        Top = 4
        Width = 449
        Height = 287
        ActivePage = TabSheet27
        MultiLine = True
        TabOrder = 0
        TabPosition = tpBottom
        object TabSheet27: TTabSheet
          Caption = #25106#25351#31867
          ImageIndex = 7
          object GroupBox49: TGroupBox
            Left = 8
            Top = 8
            Width = 113
            Height = 65
            Caption = #25915#20987
            TabOrder = 0
            object Label152: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label153: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowRingDCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowRingDCAddRateChange
            end
            object EditUnknowRingDCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowRingDCAddValueMaxLimitChange
            end
          end
          object GroupBox50: TGroupBox
            Left = 8
            Top = 80
            Width = 113
            Height = 65
            Caption = #39764#27861
            TabOrder = 1
            object Label155: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label156: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowRingMCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowRingMCAddRateChange
            end
            object EditUnknowRingMCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowRingMCAddValueMaxLimitChange
            end
          end
          object GroupBox51: TGroupBox
            Left = 8
            Top = 152
            Width = 113
            Height = 65
            Caption = #36947#26415
            TabOrder = 2
            object Label158: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label159: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowRingSCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowRingSCAddRateChange
            end
            object EditUnknowRingSCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowRingSCAddValueMaxLimitChange
            end
          end
          object GroupBox30: TGroupBox
            Left = 128
            Top = 8
            Width = 113
            Height = 65
            Caption = #38450#24481
            TabOrder = 3
            object Label89: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label90: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowRingACAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowRingACAddRateChange
            end
            object EditUnknowRingACAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowRingACAddValueMaxLimitChange
            end
          end
          object GroupBox31: TGroupBox
            Left = 128
            Top = 80
            Width = 113
            Height = 65
            Caption = #39764#27861#38450#24481
            TabOrder = 4
            object Label91: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label92: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowRingMACAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowRingMACAddRateChange
            end
            object EditUnknowRingMACAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowRingMACAddValueMaxLimitChange
            end
          end
        end
        object TabSheet25: TTabSheet
          Caption = #25163#38255#31867
          ImageIndex = 5
          object GroupBox32: TGroupBox
            Left = 8
            Top = 152
            Width = 113
            Height = 65
            Caption = #36947#26415
            TabOrder = 0
            object Label93: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label94: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowNecklaceSCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowNecklaceSCAddRateChange
            end
            object EditUnknowNecklaceSCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowNecklaceSCAddValueMaxLimitChange
            end
          end
          object GroupBox33: TGroupBox
            Left = 128
            Top = 80
            Width = 113
            Height = 65
            Caption = #39764#27861#38450#24481
            TabOrder = 1
            object Label95: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label96: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowNecklaceMACAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowNecklaceMACAddRateChange
            end
            object EditUnknowNecklaceMACAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowNecklaceMACAddValueMaxLimitChange
            end
          end
          object GroupBox34: TGroupBox
            Left = 128
            Top = 8
            Width = 113
            Height = 65
            Caption = #38450#24481
            TabOrder = 2
            object Label97: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label98: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowNecklaceACAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowNecklaceACAddRateChange
            end
            object EditUnknowNecklaceACAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowNecklaceACAddValueMaxLimitChange
            end
          end
          object GroupBox35: TGroupBox
            Left = 8
            Top = 8
            Width = 113
            Height = 65
            Caption = #25915#20987
            TabOrder = 3
            object Label99: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label100: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowNecklaceDCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowNecklaceDCAddRateChange
            end
            object EditUnknowNecklaceDCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowNecklaceDCAddValueMaxLimitChange
            end
          end
          object GroupBox36: TGroupBox
            Left = 8
            Top = 80
            Width = 113
            Height = 65
            Caption = #39764#27861
            TabOrder = 4
            object Label101: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label102: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowNecklaceMCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowNecklaceMCAddRateChange
            end
            object EditUnknowNecklaceMCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowNecklaceMCAddValueMaxLimitChange
            end
          end
        end
        object TabSheet20: TTabSheet
          Caption = #22836#30420#31867
          ImageIndex = 2
          object GroupBox37: TGroupBox
            Left = 8
            Top = 152
            Width = 113
            Height = 65
            Caption = #36947#26415
            TabOrder = 0
            object Label103: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label104: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowHelMetSCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowHelMetSCAddRateChange
            end
            object EditUnknowHelMetSCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowHelMetSCAddValueMaxLimitChange
            end
          end
          object GroupBox38: TGroupBox
            Left = 8
            Top = 80
            Width = 113
            Height = 65
            Caption = #39764#27861
            TabOrder = 1
            object Label105: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label106: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowHelMetMCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowHelMetMCAddRateChange
            end
            object EditUnknowHelMetMCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowHelMetMCAddValueMaxLimitChange
            end
          end
          object GroupBox39: TGroupBox
            Left = 8
            Top = 8
            Width = 113
            Height = 65
            Caption = #25915#20987
            TabOrder = 2
            object Label107: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label111: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowHelMetDCAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowHelMetDCAddRateChange
            end
            object EditUnknowHelMetDCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowHelMetDCAddValueMaxLimitChange
            end
          end
          object GroupBox40: TGroupBox
            Left = 128
            Top = 8
            Width = 113
            Height = 65
            Caption = #38450#24481
            TabOrder = 3
            object Label112: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label113: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowHelMetACAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowHelMetACAddRateChange
            end
            object EditUnknowHelMetACAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowHelMetACAddValueMaxLimitChange
            end
          end
          object GroupBox41: TGroupBox
            Left = 128
            Top = 80
            Width = 113
            Height = 65
            Caption = #39764#27861#38450#24481
            TabOrder = 4
            object Label114: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label115: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object EditUnknowHelMetMACAddRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditUnknowHelMetMACAddRateChange
            end
            object EditUnknowHelMetMACAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 41
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditUnknowHelMetMACAddValueMaxLimitChange
            end
          end
        end
      end
      object ButtonUnKnowItemSave: TButton
        Left = 388
        Top = 298
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonUnKnowItemSaveClick
      end
    end
    object TabSheet9: TTabSheet
      Caption = #26497#21697#26426#29575
      ImageIndex = 1
      object lbl80: TLabel
        Left = 3
        Top = 304
        Width = 318
        Height = 12
        Caption = #24314#35758#26497#21697#26368#39640#19981#36229#36807'1'#19975#28857#65292#21542#21017#24433#21709#36816#31639#25928#29575#65292#23548#33268#24341#25806#21345
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object AddValuePageControl: TPageControl
        Left = 3
        Top = 4
        Width = 451
        Height = 290
        ActivePage = ts1
        MultiLine = True
        TabOrder = 0
        TabPosition = tpBottom
        object TabSheet10: TTabSheet
          Caption = #26426#29575#25511#21046
          object GroupBox3: TGroupBox
            Left = 8
            Top = 8
            Width = 137
            Height = 81
            Caption = #26497#21697#20986#29616#26426#29575
            TabOrder = 0
            object Label6: TLabel
              Left = 11
              Top = 24
              Width = 54
              Height = 12
              Caption = #24618#29289#25481#33853':'
            end
            object Label7: TLabel
              Left = 11
              Top = 48
              Width = 54
              Height = 12
              Caption = #21629#20196#21046#36896':'
            end
            object EditMonRandomAddValue: TSpinEditEx
              Left = 72
              Top = 20
              Width = 57
              Height = 21
              Hint = #24618#29289#27515#20129#25481#33853#29289#21697#26497#21697#20986#29616#26426#29575#65292#25968#25454#36234#22823#65292#26426#29575#36234#23567#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 0
              Value = 100
              OnChange = EditMonRandomAddValueChange
            end
            object EditMakeRandomAddValue: TSpinEditEx
              Left = 72
              Top = 44
              Width = 57
              Height = 21
              Hint = 'GM'#21629#20196#21046#36896#29289#21697#26497#21697#20986#29616#26426#29575#65292#25968#25454#36234#22823#65292#26426#29575#36234#23567#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 1
              Value = 100
              OnChange = EditMakeRandomAddValueChange
            end
          end
        end
        object TabSheet11: TTabSheet
          Caption = #27494#22120#31867
          ImageIndex = 1
          object Label32: TLabel
            Left = 152
            Top = 184
            Width = 210
            Height = 12
            Caption = #27494#22120#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 5'#25110'6'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox4: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 90
            Caption = #25915#20987
            TabOrder = 0
            object Label8: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label9: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label186: TLabel
              Left = 11
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditWeaponDCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditWeaponDCAddValueMaxLimitChange
            end
            object EditWeaponDCAddValueRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditWeaponDCAddValueRateChange
            end
            object EditWeaponDCAddRate: TSpinEditEx
              Left = 64
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditWeaponDCAddRateChange
            end
          end
          object GroupBox6: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 90
            Caption = #36947#26415
            TabOrder = 1
            object Label12: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label13: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label187: TLabel
              Left = 11
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditWeaponSCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditWeaponSCAddValueMaxLimitChange
            end
            object EditWeaponSCAddValueRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditWeaponSCAddValueRateChange
            end
            object EditWeaponSCAddRate: TSpinEditEx
              Left = 64
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditWeaponSCAddRateChange
            end
          end
          object GroupBox5: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 90
            Caption = #39764#27861
            TabOrder = 2
            object Label10: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label11: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label185: TLabel
              Left = 11
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditWeaponMCAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditWeaponMCAddValueMaxLimitChange
            end
            object EditWeaponMCAddValueRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditWeaponMCAddValueRateChange
            end
            object EditWeaponMCAddRate: TSpinEditEx
              Left = 64
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditWeaponMCAddRateChange
            end
          end
          object GroupBox69: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #25915#20987#36895#24230
            TabOrder = 3
            object Label190: TLabel
              Left = 11
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label191: TLabel
              Left = 11
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label192: TLabel
              Left = 11
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditWeaponHitSpeedAddValueMaxLimit: TSpinEditEx
              Left = 64
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditWeaponHitSpeedAddValueMaxLimitChange
            end
            object EditWeaponHitSpeedAddValueRate: TSpinEditEx
              Left = 64
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditWeaponHitSpeedAddValueRateChange
            end
            object EditWeaponHitSpeedAddRate: TSpinEditEx
              Left = 64
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditWeaponHitSpeedAddRateChange
            end
          end
        end
        object TabSheet12: TTabSheet
          Caption = #34915#26381#31867
          ImageIndex = 2
          object Label33: TLabel
            Left = 144
            Top = 184
            Width = 228
            Height = 12
            Caption = #34915#26381#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 10 '#25110'11'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox7: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object Label14: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label15: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label20: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditDressDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditDressDCAddValueMaxLimitChange
            end
            object EditDressDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditDressDCAddValueRateChange
            end
            object EditDressDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditDressDCAddRateChange
            end
          end
          object GroupBox8: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 1
            object Label16: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label17: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label21: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditDressMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditDressMCAddValueMaxLimitChange
            end
            object EditDressMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditDressMCAddValueRateChange
            end
            object EditDressMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditDressMCAddRateChange
            end
          end
          object GroupBox9: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 2
            object Label18: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label19: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label22: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditDressSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditDressSCAddValueMaxLimitChange
            end
            object EditDressSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditDressSCAddValueRateChange
            end
            object EditDressSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditDressSCAddRateChange
            end
          end
          object GroupBox67: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object Label179: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label180: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label181: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditDressACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditDressACAddValueMaxLimitChange
            end
            object EditDressACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditDressACAddValueRateChange
            end
            object EditDressACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditDressACAddRateChange
            end
          end
          object GroupBox68: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object Label182: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label183: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label125: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditDressMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditDressMACAddValueMaxLimitChange
            end
            object EditDressMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditDressMACAddValueRateChange
            end
            object EditDressMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditDressMACAddRateChange
            end
          end
        end
        object TabSheet13: TTabSheet
          Caption = #39033#38142#31867
          ImageIndex = 3
          object Label34: TLabel
            Left = 128
            Top = 184
            Width = 246
            Height = 12
            Caption = #39033#38142#31867'('#24184#36816#31867')'#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 19'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox10: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object Label23: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label24: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label25: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace19DCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace19DCAddValueMaxLimitChange
            end
            object EditNeckLace19DCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace19DCAddValueRateChange
            end
            object EditNeckLace19DCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace19DCAddRateChange
            end
          end
          object GroupBox11: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 1
            object Label26: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label27: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label28: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace19MCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace19MCAddValueMaxLimitChange
            end
            object EditNeckLace19MCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace19MCAddValueRateChange
            end
            object EditNeckLace19MCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace19MCAddRateChange
            end
          end
          object GroupBox12: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 2
            object Label29: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label30: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label31: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace19SCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace19SCAddValueMaxLimitChange
            end
            object EditNeckLace19SCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace19SCAddValueRateChange
            end
            object EditNeckLace19SCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace19SCAddRateChange
            end
          end
          object GroupBox61: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object Label161: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label162: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label163: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace19ACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace19ACAddValueMaxLimitChange
            end
            object EditNeckLace19ACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace19ACAddValueRateChange
            end
            object EditNeckLace19ACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace19ACAddRateChange
            end
          end
          object GroupBox62: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #24184#36816'(MAC2)'
            TabOrder = 4
            object Label164: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label165: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label166: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace19MACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace19MACAddValueMaxLimitChange
            end
            object EditNeckLace19MACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace19MACAddValueRateChange
            end
            object EditNeckLace19MACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace19MACAddRateChange
            end
          end
        end
        object TabSheet14: TTabSheet
          Caption = #39033#38142#25163#38255
          ImageIndex = 4
          object Label35: TLabel
            Left = 136
            Top = 184
            Width = 234
            Height = 12
            Caption = #39033#38142#25163#38255#31867#65292#25968#25454#24211#23383#27573' StdMode 20,21,24'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox13: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object Label36: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label37: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label38: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace202124DCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace202124DCAddValueMaxLimitChange
            end
            object EditNeckLace202124DCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace202124DCAddValueRateChange
            end
            object EditNeckLace202124DCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace202124DCAddRateChange
            end
          end
          object GroupBox14: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 1
            object Label39: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label40: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label41: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace202124MCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace202124MCAddValueMaxLimitChange
            end
            object EditNeckLace202124MCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace202124MCAddValueRateChange
            end
            object EditNeckLace202124MCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace202124MCAddRateChange
            end
          end
          object GroupBox15: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 2
            object Label42: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label43: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label44: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace202124SCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace202124SCAddValueMaxLimitChange
            end
            object EditNeckLace202124SCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace202124SCAddValueRateChange
            end
            object EditNeckLace202124SCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace202124SCAddRateChange
            end
          end
          object GroupBox65: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object Label173: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label174: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label175: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace202124ACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace202124ACAddValueMaxLimitChange
            end
            object EditNeckLace202124ACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace202124ACAddValueRateChange
            end
            object EditNeckLace202124ACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace202124ACAddRateChange
            end
          end
          object GroupBox66: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object Label176: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label177: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label178: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditNeckLace202124MACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditNeckLace202124MACAddValueMaxLimitChange
            end
            object EditNeckLace202124MACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditNeckLace202124MACAddValueRateChange
            end
            object EditNeckLace202124MACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditNeckLace202124MACAddRateChange
            end
          end
        end
        object TabSheet15: TTabSheet
          Caption = #25163#38255#31867
          ImageIndex = 5
          object Label54: TLabel
            Left = 176
            Top = 184
            Width = 198
            Height = 12
            Caption = #25163#38255#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 26'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox16: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 0
            object Label45: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label46: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label47: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditArmRing26MCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditArmRing26MCAddValueMaxLimitChange
            end
            object EditArmRing26MCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditArmRing26MCAddValueRateChange
            end
            object EditArmRing26MCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditArmRing26MCAddRateChange
            end
          end
          object GroupBox17: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 1
            object Label48: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label49: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label50: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditArmRing26DCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditArmRing26DCAddValueMaxLimitChange
            end
            object EditArmRing26DCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditArmRing26DCAddValueRateChange
            end
            object EditArmRing26DCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditArmRing26DCAddRateChange
            end
          end
          object GroupBox18: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 2
            object Label51: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label52: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label53: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditArmRing26SCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditArmRing26SCAddValueMaxLimitChange
            end
            object EditArmRing26SCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditArmRing26SCAddValueRateChange
            end
            object EditArmRing26SCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditArmRing26SCAddRateChange
            end
          end
          object GroupBox64: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 3
            object Label170: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label171: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label172: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditArmRing26MACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditArmRing26MACAddValueMaxLimitChange
            end
            object EditArmRing26MACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditArmRing26MACAddValueRateChange
            end
            object EditArmRing26MACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditArmRing26MACAddRateChange
            end
          end
          object GroupBox48: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 4
            object Label167: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label168: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label169: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditArmRing26ACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditArmRing26ACAddValueMaxLimitChange
            end
            object EditArmRing26ACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditArmRing26ACAddValueRateChange
            end
            object EditArmRing26ACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditArmRing26ACAddRateChange
            end
          end
        end
        object TabSheet16: TTabSheet
          Caption = #25106#25351#31867
          ImageIndex = 6
          object Label64: TLabel
            Left = 136
            Top = 184
            Width = 198
            Height = 12
            Caption = #25106#25351#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 22'
            Font.Charset = ANSI_CHARSET
            Font.Color = clBlack
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox19: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object Label55: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label56: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label57: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing22DCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing22DCAddValueMaxLimitChange
            end
            object EditRing22DCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing22DCAddValueRateChange
            end
            object EditRing22DCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing22DCAddRateChange
            end
          end
          object GroupBox20: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 1
            object Label58: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label59: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label60: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing22SCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing22SCAddValueMaxLimitChange
            end
            object EditRing22SCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing22SCAddValueRateChange
            end
            object EditRing22SCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing22SCAddRateChange
            end
          end
          object GroupBox21: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 2
            object Label61: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label62: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label63: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing22MCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing22MCAddValueMaxLimitChange
            end
            object EditRing22MCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing22MCAddValueRateChange
            end
            object EditRing22MCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing22MCAddRateChange
            end
          end
          object GroupBox45: TGroupBox
            Left = 6
            Top = 93
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object Label203: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label204: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label205: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing22ACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing22ACAddValueMaxLimitChange
            end
            object EditRing22ACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing22ACAddValueRateChange
            end
            object EditRing22ACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing22ACAddRateChange
            end
          end
          object GroupBox70: TGroupBox
            Left = 151
            Top = 93
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object Label206: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label207: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label208: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing22MACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing22MACAddValueMaxLimitChange
            end
            object EditRing22MACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing22MACAddValueRateChange
            end
            object EditRing22MACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing22MACAddRateChange
            end
          end
        end
        object TabSheet17: TTabSheet
          Caption = #25106#25351#31867
          ImageIndex = 7
          object Label74: TLabel
            Left = 173
            Top = 187
            Width = 198
            Height = 12
            Caption = #25106#25351#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 23'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox60: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 0
            object Label154: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label157: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label160: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing23MACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing23MACAddValueMaxLimitChange
            end
            object EditRing23MACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing23MACAddValueRateChange
            end
            object EditRing23MACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing23MACAddRateChange
            end
          end
          object GroupBox59: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 1
            object Label65: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label66: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label151: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing23ACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing23ACAddValueMaxLimitChange
            end
            object EditRing23ACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing23ACAddValueRateChange
            end
            object EditRing23ACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing23ACAddRateChange
            end
          end
          object GroupBox22: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 2
            object Label67: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label68: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label69: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing23DCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing23DCAddValueMaxLimitChange
            end
            object EditRing23DCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing23DCAddValueRateChange
            end
            object EditRing23DCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing23DCAddRateChange
            end
          end
          object GroupBox24: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 3
            object Label71: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label72: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label73: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing23SCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing23SCAddValueMaxLimitChange
            end
            object EditRing23SCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing23SCAddValueRateChange
            end
            object EditRing23SCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing23SCAddRateChange
            end
          end
          object GroupBox23: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 4
            object Label70: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label188: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label189: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditRing23MCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditRing23MCAddValueMaxLimitChange
            end
            object EditRing23MCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditRing23MCAddValueRateChange
            end
            object EditRing23MCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditRing23MCAddRateChange
            end
          end
        end
        object TabSheet18: TTabSheet
          Caption = #22836#30420#12289#26007#31520#31867
          ImageIndex = 8
          object Label84: TLabel
            Left = 176
            Top = 176
            Width = 198
            Height = 12
            Caption = #22836#30420#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 15'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object lbl1: TLabel
            Left = 176
            Top = 190
            Width = 198
            Height = 12
            Caption = #26007#31520#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 16'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox25: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object Label75: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label76: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label77: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditHelMetDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditHelMetDCAddValueMaxLimitChange
            end
            object EditHelMetDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditHelMetDCAddValueRateChange
            end
            object EditHelMetDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditHelMetDCAddRateChange
            end
          end
          object GroupBox26: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 1
            object Label78: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label79: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label80: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditHelMetMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditHelMetMCAddValueMaxLimitChange
            end
            object EditHelMetMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditHelMetMCAddValueRateChange
            end
            object EditHelMetMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditHelMetMCAddRateChange
            end
          end
          object GroupBox27: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 2
            object Label81: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label82: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label83: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditHelMetSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditHelMetSCAddValueMaxLimitChange
            end
            object EditHelMetSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditHelMetSCAddValueRateChange
            end
            object EditHelMetSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditHelMetSCAddRateChange
            end
          end
          object GroupBox57: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object Label143: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label144: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label145: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditHelMetACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditHelMetACAddValueMaxLimitChange
            end
            object EditHelMetACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditHelMetACAddValueRateChange
            end
            object EditHelMetACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditHelMetACAddRateChange
            end
          end
          object GroupBox58: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object Label146: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label147: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label148: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditHelMetMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditHelMetMACAddValueMaxLimitChange
            end
            object EditHelMetMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditHelMetMACAddValueRateChange
            end
            object EditHelMetMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditHelMetMACAddRateChange
            end
          end
        end
        object TabSheet22: TTabSheet
          Caption = #38795#23376#12289#33136#24102#31867
          ImageIndex = 9
          object Label149: TLabel
            Left = 164
            Top = 176
            Width = 216
            Height = 12
            Caption = #38795#23376#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 52 62'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object Label150: TLabel
            Left = 164
            Top = 189
            Width = 216
            Height = 12
            Caption = #33136#24102#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 54 64'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object GroupBox53: TGroupBox
            Left = 6
            Top = 1
            Width = 140
            Height = 86
            Caption = #25915#20987
            TabOrder = 0
            object Label129: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label130: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label131: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditBootsDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditBootsDCAddValueMaxLimitChange
            end
            object EditBootsDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditBootsDCAddValueRateChange
            end
            object EditBootsDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditBootsDCAddRateChange
            end
          end
          object GroupBox52: TGroupBox
            Left = 151
            Top = 1
            Width = 140
            Height = 86
            Caption = #36947#26415
            TabOrder = 1
            object Label126: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label127: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label128: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditBootsSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditBootsSCAddValueMaxLimitChange
            end
            object EditBootsSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditBootsSCAddValueRateChange
            end
            object EditBootsSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditBootsSCAddRateChange
            end
          end
          object GroupBox54: TGroupBox
            Left = 297
            Top = 1
            Width = 140
            Height = 86
            Caption = #39764#27861
            TabOrder = 2
            object Label133: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label134: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label135: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditBootsMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditBootsMCAddValueMaxLimitChange
            end
            object EditBootsMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditBootsMCAddValueRateChange
            end
            object EditBootsMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditBootsMCAddRateChange
            end
          end
          object GroupBox56: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 3
            object Label140: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label141: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label142: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditBootsMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditBootsMACAddValueMaxLimitChange
            end
            object EditBootsMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditBootsMACAddValueRateChange
            end
            object EditBootsMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditBootsMACAddRateChange
            end
          end
          object GroupBox55: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 4
            object Label137: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object Label138: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object Label139: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object EditBootsACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = EditBootsACAddValueMaxLimitChange
            end
            object EditBootsACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = EditBootsACAddValueRateChange
            end
            object EditBootsACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = EditBootsACAddRateChange
            end
          end
        end
        object ts1: TTabSheet
          Caption = #26102#35013#27494#22120#31867
          ImageIndex = 10
          object lbl2: TLabel
            Left = 128
            Top = 184
            Width = 246
            Height = 12
            Caption = #26102#35013#27494#22120#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 68'#25110'69'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object grp1: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 90
            Caption = #25915#20987
            TabOrder = 0
            object lbl3: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl4: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl5: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionWeaponDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionWeaponDCAddValueMaxLimitChange
            end
            object seFashionWeaponDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionWeaponDCAddValueRateChange
            end
            object seFashionWeaponDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionWeaponDCAddRateChange
            end
          end
          object grp2: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 90
            Caption = #36947#26415
            TabOrder = 1
            object lbl6: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl7: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl8: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionWeaponSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionWeaponSCAddValueMaxLimitChange
            end
            object seFashionWeaponSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionWeaponSCAddValueRateChange
            end
            object seFashionWeaponSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionWeaponSCAddRateChange
            end
          end
          object grp3: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 90
            Caption = #39764#27861
            TabOrder = 2
            object lbl9: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl10: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl11: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionWeaponMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionWeaponMCAddValueMaxLimitChange
            end
            object seFashionWeaponMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionWeaponMCAddValueRateChange
            end
            object seFashionWeaponMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionWeaponMCAddRateChange
            end
          end
          object grp4: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #25915#20987#36895#24230
            TabOrder = 3
            object lbl12: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl13: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl14: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionWeaponHitSpeedAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionWeaponHitSpeedAddValueMaxLimitChange
            end
            object seFashionWeaponHitSpeedAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionWeaponHitSpeedAddValueRateChange
            end
            object seFashionWeaponHitSpeedAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionWeaponHitSpeedAddRateChange
            end
          end
        end
        object ts2: TTabSheet
          Caption = #26102#35013#34915#26381#31867
          ImageIndex = 11
          object lbl30: TLabel
            Left = 120
            Top = 184
            Width = 258
            Height = 12
            Caption = #26102#35013#34915#26381#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 66 '#25110' 67'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object grp5: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object lbl15: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl16: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl17: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionDressDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionDressDCAddValueMaxLimitChange
            end
            object seFashionDressDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionDressDCAddValueRateChange
            end
            object seFashionDressDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionDressDCAddRateChange
            end
          end
          object grp6: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 1
            object lbl18: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl19: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl20: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionDressSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionDressSCAddValueMaxLimitChange
            end
            object seFashionDressSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionDressSCAddValueRateChange
            end
            object seFashionDressSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 1
              OnChange = seFashionDressSCAddRateChange
            end
          end
          object grp7: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 2
            object lbl21: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl22: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl23: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionDressMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionDressMCAddValueMaxLimitChange
            end
            object seFashionDressMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionDressMCAddValueRateChange
            end
            object seFashionDressMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionDressMCAddRateChange
            end
          end
          object grp8: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object lbl24: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl25: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl26: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionDressACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionDressACAddValueMaxLimitChange
            end
            object seFashionDressACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionDressACAddValueRateChange
            end
            object seFashionDressACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionDressACAddRateChange
            end
          end
          object grp9: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object lbl27: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl28: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl29: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seFashionDressMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seFashionDressMACAddValueMaxLimitChange
            end
            object seFashionDressMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seFashionDressMACAddValueRateChange
            end
            object seFashionDressMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seFashionDressMACAddRateChange
            end
          end
        end
        object ts3: TTabSheet
          Caption = #39532#29260#31867
          ImageIndex = 11
          object lbl31: TLabel
            Left = 184
            Top = 184
            Width = 198
            Height = 12
            Caption = #39532#29260#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 28'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object grp10: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object lbl32: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl33: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl34: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seHorseDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seHorseDCAddValueMaxLimitChange
            end
            object seHorseDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seHorseDCAddValueRateChange
            end
            object seHorseDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seHorseDCAddRateChange
            end
          end
          object grp11: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 1
            object lbl35: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl36: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl37: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seHorseSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seHorseSCAddValueMaxLimitChange
            end
            object seHorseSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seHorseSCAddValueRateChange
            end
            object seHorseSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seHorseSCAddRateChange
            end
          end
          object grp12: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 2
            object lbl38: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl39: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl40: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seHorseMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seHorseMCAddValueMaxLimitChange
            end
            object seHorseMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seHorseMCAddValueRateChange
            end
            object seHorseMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seHorseMCAddRateChange
            end
          end
          object grp13: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object lbl41: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl42: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl43: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seHorseACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seHorseACAddValueMaxLimitChange
            end
            object seHorseACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seHorseACAddValueRateChange
            end
            object seHorseACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seHorseACAddRateChange
            end
          end
          object grp14: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object lbl44: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl45: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl46: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seHorseMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seHorseMACAddValueMaxLimitChange
            end
            object seHorseMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seHorseMACAddValueRateChange
            end
            object seHorseMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seHorseMACAddRateChange
            end
          end
        end
        object ts4: TTabSheet
          Caption = #20891#40723#31867
          ImageIndex = 13
          object lbl47: TLabel
            Left = 160
            Top = 184
            Width = 222
            Height = 12
            Caption = #26102#35013#34915#26381#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 65'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object grp15: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object lbl48: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl49: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl50: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seDrumDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seDrumDCAddValueMaxLimitChange
            end
            object seDrumDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seDrumDCAddValueRateChange
            end
            object seDrumDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seDrumDCAddRateChange
            end
          end
          object grp16: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 1
            object lbl51: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl52: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl53: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seDrumSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seDrumSCAddValueMaxLimitChange
            end
            object seDrumSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seDrumSCAddValueRateChange
            end
            object seDrumSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 1
              OnChange = seDrumSCAddRateChange
            end
          end
          object grp17: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 2
            object lbl54: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl55: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl56: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seDrumMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seDrumMCAddValueMaxLimitChange
            end
            object seDrumMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seDrumMCAddValueRateChange
            end
            object seDrumMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seDrumMCAddRateChange
            end
          end
          object grp18: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object lbl57: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl58: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl59: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seDrumACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seDrumACAddValueMaxLimitChange
            end
            object seDrumACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seDrumACAddValueRateChange
            end
            object seDrumACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seDrumACAddRateChange
            end
          end
          object grp19: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object lbl60: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl61: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl62: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seDrumMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seDrumMACAddValueMaxLimitChange
            end
            object seDrumMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seDrumMACAddValueRateChange
            end
            object seDrumMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seDrumMACAddRateChange
            end
          end
        end
        object ts5: TTabSheet
          Caption = #30462#29260#31867
          ImageIndex = 14
          object lbl63: TLabel
            Left = 152
            Top = 184
            Width = 222
            Height = 12
            Caption = #26102#35013#34915#26381#31867#65292#29289#21697#25968#25454#24211#23383#27573' StdMode 12'
            Font.Charset = ANSI_CHARSET
            Font.Color = clWindowText
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object grp20: TGroupBox
            Left = 6
            Top = 0
            Width = 140
            Height = 89
            Caption = #25915#20987
            TabOrder = 0
            object lbl64: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl65: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl66: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seShieldDCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seShieldDCAddValueMaxLimitChange
            end
            object seShieldDCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seShieldDCAddValueRateChange
            end
            object seShieldDCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seShieldDCAddRateChange
            end
          end
          object grp21: TGroupBox
            Left = 151
            Top = 0
            Width = 140
            Height = 89
            Caption = #36947#26415
            TabOrder = 1
            object lbl67: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl68: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl69: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seShieldSCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seShieldSCAddValueMaxLimitChange
            end
            object seShieldSCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seShieldSCAddValueRateChange
            end
            object seShieldSCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seShieldSCAddRateChange
            end
          end
          object grp22: TGroupBox
            Left = 297
            Top = 0
            Width = 140
            Height = 89
            Caption = #39764#27861
            TabOrder = 2
            object lbl70: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl71: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl72: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seShieldMCAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seShieldMCAddValueMaxLimitChange
            end
            object seShieldMCAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seShieldMCAddValueRateChange
            end
            object seShieldMCAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seShieldMCAddRateChange
            end
          end
          object grp23: TGroupBox
            Left = 6
            Top = 90
            Width = 140
            Height = 86
            Caption = #38450#24481
            TabOrder = 3
            object lbl73: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl74: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl75: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seShieldACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seShieldACAddValueMaxLimitChange
            end
            object seShieldACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seShieldACAddValueRateChange
            end
            object seShieldACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seShieldACAddRateChange
            end
          end
          object grp24: TGroupBox
            Left = 151
            Top = 90
            Width = 140
            Height = 86
            Caption = #39764#24481
            TabOrder = 4
            object lbl76: TLabel
              Left = 10
              Top = 16
              Width = 54
              Height = 12
              Caption = #26368#39640#28857#25968':'
            end
            object lbl77: TLabel
              Left = 10
              Top = 40
              Width = 54
              Height = 12
              Caption = #28857#25968#26426#29575':'
            end
            object lbl78: TLabel
              Left = 10
              Top = 64
              Width = 54
              Height = 12
              Caption = #23646#24615#26426#29575':'
            end
            object seShieldMACAddValueMaxLimit: TSpinEditEx
              Left = 63
              Top = 12
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26368#39640#38480#21046#12290
              MaxValue = 1000000
              MinValue = 1
              TabOrder = 0
              Value = 100
              OnChange = seShieldMACAddValueMaxLimitChange
            end
            object seShieldMACAddValueRate: TSpinEditEx
              Left = 63
              Top = 36
              Width = 66
              Height = 21
              Hint = #26497#21697#23646#24615#28857#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
              MaxValue = 100
              MinValue = 1
              TabOrder = 1
              Value = 100
              OnChange = seShieldMACAddValueRateChange
            end
            object seShieldMACAddRate: TSpinEditEx
              Left = 63
              Top = 60
              Width = 66
              Height = 21
              Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290#13#10#13#10'0'#34920#31034#26080#20960#29575
              MaxValue = 100
              MinValue = 0
              TabOrder = 2
              Value = 100
              OnChange = seShieldMACAddRateChange
            end
          end
        end
      end
      object ButtonAddValueSave: TButton
        Left = 386
        Top = 298
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonAddValueSaveClick
      end
    end
    object TabSheet21: TTabSheet
      Caption = #26032#22686#23646#24615
      ImageIndex = 3
      object lbl79: TLabel
        Left = 3
        Top = 310
        Width = 378
        Height = 12
        AutoSize = False
        Caption = #24594#27668#24674#22797#20165#36866#21512#20110#33521#38596#20329#25140#26377#25928#65292#26292#20987#20960#29575'-'#26292#20987#25239#24615'='#26292#20987#25171#20986#20960#29575
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
        Transparent = True
      end
      object GroupBox77: TGroupBox
        Left = 2
        Top = 8
        Width = 108
        Height = 41
        Caption = #21551#29992#26032#22686#23646#24615
        TabOrder = 0
        object chkItemNewAbilAllowUse: TCheckBox
          Left = 8
          Top = 16
          Width = 97
          Height = 17
          Caption = #21551#29992#26032#22686#23646#24615
          TabOrder = 0
          OnClick = chkItemNewAbilAllowUseClick
        end
      end
      object ButtonNewAbilSave: TButton
        Left = 382
        Top = 298
        Width = 68
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 1
        OnClick = ButtonNewAbilSaveClick
      end
      object CheckGroupNewAbil: TRzCheckGroup
        Left = 114
        Top = 25
        Width = 154
        Height = 279
        Caption = #23646#24615
        Color = 15987699
        Columns = 2
        GroupStyle = gsStandard
        ItemHeight = 16
        Items.Strings = (
          #26292#20987#20960#29575
          #25915#20987#20260#23475
          #20260#23475#21560#25910
          #39764#27861#38450#24481
          #24573#35270#38450#24481
          #20260#23475#21453#24377
          #20154#29289#29190#29575
          #20307#21147#22686#21152
          #39764#21147#22686#21152
          #24594#27668#24674#22797
          #21512#20987#20260#23475
          #24618#29289#29190#29575
          #38450#29190#20986#29575
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
          #26292#20987#25239#24615
          #38543#26426#19968#31181)
        TabOrder = 2
        Transparent = True
        VerticalSpacing = 4
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
          0
          0
          0)
      end
      object GroupBox63: TGroupBox
        Left = 2
        Top = 56
        Width = 108
        Height = 97
        Caption = #20986#29616#26426#29575'%'
        TabOrder = 3
        object Label132: TLabel
          Left = 5
          Top = 24
          Width = 54
          Height = 12
          Caption = #24618#29289#25481#33853':'
        end
        object Label136: TLabel
          Left = 5
          Top = 48
          Width = 54
          Height = 12
          Caption = #21629#20196#21046#36896':'
        end
        object Label184: TLabel
          Left = 5
          Top = 72
          Width = 54
          Height = 12
          Caption = #33050#26412#21046#36896':'
        end
        object EditItemNewAbilMonRandomAddRate: TSpinEditEx
          Left = 60
          Top = 20
          Width = 42
          Height = 21
          Hint = #24618#29289#27515#20129#25481#33853#29289#21697#26497#21697#20986#29616#26426#29575#65292#25968#25454#36234#22823#65292#26426#29575#36234#23567#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 0
          Value = 100
          OnChange = EditItemNewAbilMonRandomAddRateChange
        end
        object EditItemNewAbilMakeRandomAddRate: TSpinEditEx
          Left = 60
          Top = 44
          Width = 42
          Height = 21
          Hint = 'GM'#21629#20196#21046#36896#29289#21697#26497#21697#20986#29616#26426#29575#65292#25968#25454#36234#22823#65292#26426#29575#36234#23567#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 1
          Value = 100
          OnChange = EditItemNewAbilMakeRandomAddRateChange
        end
        object EditItemNewAbilScriptRandomAddRate: TSpinEditEx
          Left = 60
          Top = 68
          Width = 42
          Height = 21
          Hint = #33050#26412#21629#20196#21046#36896#29289#21697#26497#21697#20986#29616#26426#29575#65292#25968#25454#36234#22823#65292#26426#29575#36234#23567#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 2
          Value = 100
          OnChange = EditItemNewAbilScriptRandomAddRateChange
        end
      end
      object GroupBoxNewAbil: TGroupBox
        Left = 273
        Top = 5
        Width = 179
        Height = 113
        Caption = #20854#20182#23646#24615#25511#21046
        TabOrder = 4
        object Label213: TLabel
          Left = 6
          Top = 42
          Width = 102
          Height = 12
          Caption = #20854#20182#23646#24615#26368#39640#28857#25968':'
        end
        object Label215: TLabel
          Left = 6
          Top = 65
          Width = 54
          Height = 12
          Caption = #28857#25968#26426#29575':'
        end
        object Label216: TLabel
          Left = 6
          Top = 89
          Width = 54
          Height = 12
          Caption = #23646#24615#26426#29575':'
        end
        object Label202: TLabel
          Left = 6
          Top = 18
          Width = 114
          Height = 12
          Caption = #25915#20987#20260#23475'/HP/MP'#38480#21046':'
        end
        object EditItemNewAbilAddValueMaxLimit: TSpinEditEx
          Left = 120
          Top = 38
          Width = 54
          Height = 21
          Hint = #23646#24615#28857#26368#39640#38480#21046#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = EditItemNewAbilAddValueMaxLimitChange
        end
        object EditItemNewAbilAddValueRate: TSpinEditEx
          Left = 64
          Top = 61
          Width = 52
          Height = 21
          Hint = #25968#25454#36234#22823#26426#29575#36234#23567#65292#26368#39640#19981#36229#36807#26368#39640#28857#25968#25511#21046#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 1
          Value = 100
          OnChange = EditItemNewAbilAddValueRateChange
        end
        object EditItemNewAbilAddRate: TSpinEditEx
          Left = 64
          Top = 85
          Width = 52
          Height = 21
          Hint = #21152#23646#26497#21697#23646#24615#26426#29575#65292#25968#25454#36234#22823#26426#29575#36234#23567#65292#27492#26426#29575#20915#23450#26159#21542#21152#19978#23646#24615#12290
          MaxValue = 100
          MinValue = 1
          TabOrder = 2
          Value = 100
          OnChange = EditItemNewAbilAddRateChange
        end
        object EditItemNewAbilAddValueMaxLimit2: TSpinEditEx
          Left = 120
          Top = 14
          Width = 54
          Height = 21
          Hint = #25915#20987#20260#23475#12289#20307#21147#22686#21152#12289#39764#27861#22686#21152#12289#21512#20987#20260#23475#12289#33268#21629#19968#20987#20260#23475#30340#26368#39640#28857#25968#25511#21046
          MaxValue = 50000
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = EditItemNewAbilAddValueMaxLimit2Change
        end
      end
      object ComboBoxNewAbilItemType: TComboBox
        Left = 114
        Top = 3
        Width = 155
        Height = 20
        Style = csDropDownList
        ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
        TabOrder = 5
        OnChange = ComboBoxNewAbilItemTypeChange
        Items.Strings = (
          #27494#22120#31867
          #34915#26381#31867
          #39033#38142#31867
          #25163#38255#31867
          #25106#25351#31867
          #22836#30420#31867
          #38795#23376#31867
          #33136#24102#31867
          #26007#31520
          #39532#29260
          #20891#40723
          #30462#29260
          #26102#35013#34915#26381
          #26102#35013#27494#22120
          '53'#31867#23453#30707
          '63'#31867#23453#30707
          #26102#35013#39033#38142
          #26102#35013#22836#30420
          #26102#35013#25163#38255
          #26102#35013#25106#25351
          #26102#35013#21195#31456
          #26102#35013#33136#24102
          #26102#35013#38772#23376
          #26102#35013#23453#30707
          #28789#29577)
      end
      object GroupBox44: TGroupBox
        Left = 273
        Top = 125
        Width = 179
        Height = 157
        Caption = #20260#23475#22686#21152#25511#21046'%'
        TabOrder = 6
        object Label201: TLabel
          Left = 11
          Top = 18
          Width = 78
          Height = 12
          Caption = #26292#20987#20260#23475#20493#29575':'
        end
        object Label210: TLabel
          Left = 11
          Top = 41
          Width = 78
          Height = 12
          Caption = #21453#24377#20260#23475#20493#29575':'
        end
        object Label212: TLabel
          Left = 11
          Top = 64
          Width = 96
          Height = 12
          Caption = #33268#21629#19968#20987#22522#30784#23041#21147
        end
        object Label214: TLabel
          Left = 11
          Top = 87
          Width = 96
          Height = 12
          Caption = #20250#24515#19968#20987#25152#38656#23041#21147
        end
        object Label217: TLabel
          Left = 11
          Top = 110
          Width = 96
          Height = 12
          Caption = #21331#36234#19968#20987#25152#38656#23041#21147
        end
        object Label218: TLabel
          Left = 11
          Top = 133
          Width = 96
          Height = 12
          Caption = #33268#21629#19968#20987#25152#38656#23041#21147
        end
        object lbl81: TLabel
          Left = 163
          Top = 64
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label220: TLabel
          Left = 163
          Top = 87
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label221: TLabel
          Left = 163
          Top = 110
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label222: TLabel
          Left = 163
          Top = 133
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label223: TLabel
          Left = 163
          Top = 41
          Width = 6
          Height = 12
          Caption = '%'
        end
        object Label224: TLabel
          Left = 163
          Top = 18
          Width = 6
          Height = 12
          Caption = '%'
        end
        object seCritAttackHurtRate: TSpinEditEx
          Left = 90
          Top = 14
          Width = 71
          Height = 21
          Hint = #26292#20987#20260#23475#22686#21152#20493#29575#65292#22914#26524#35774#32622#20026'10'#65292#21017#26292#20987#20260#23475#20026'110%'
          MaxValue = 10000
          MinValue = 1
          TabOrder = 0
          Value = 10
          OnChange = seCritAttackHurtRateChange
        end
        object seDamageReboundRate: TSpinEditEx
          Left = 90
          Top = 37
          Width = 71
          Height = 21
          Hint = #21453#24377#20260#23475#20493#29575#65292#22914#26524#35774#32622#20026'10'#65292#21453#24377#20260#23475#30340'10%'
          MaxValue = 10000
          MinValue = 0
          TabOrder = 1
          Value = 100
          OnChange = seDamageReboundRateChange
        end
        object seFatalBlowBasePower: TSpinEditEx
          Left = 109
          Top = 60
          Width = 51
          Height = 21
          MaxValue = 10000
          MinValue = 100
          TabOrder = 2
          Value = 100
          OnChange = seFatalBlowBasePowerChange
        end
        object seFatalBlowNeedPower1: TSpinEditEx
          Left = 109
          Top = 83
          Width = 51
          Height = 21
          Hint = 
            #24403#21069#23041#21147#35745#31639#65306#22522#30784#23041#21147' + '#33268#21629#19968#20987#20260#23475#22686#21152#13#10#13#10#26292#20987#65306#24403#21069#23041#21147' < '#20250#24515#19968#20987#25152#38656#23041#21147#13#10#20250#24515#19968#20987#65306#20250#24515#19968#20987#25152#38656#23041#21147' <= '#24403 +
            #21069#23041#21147' < '#21331#36234#19968#20987#25152#38656#23041#21147#13#10#21331#36234#19968#20987#65306#21331#36234#19968#20987#25152#38656#23041#21147' <= '#24403#21069#23041#21147' < '#33268#21629#19968#20987#25152#38656#23041#21147#13#10#33268#21629#19968#20987#65306#24403#21069#23041#21147' >= '#33268 +
            #21629#19968#20987#25152#38656#23041#21147
          MaxValue = 10000
          MinValue = 100
          TabOrder = 3
          Value = 100
          OnChange = seFatalBlowNeedPower1Change
        end
        object seFatalBlowNeedPower2: TSpinEditEx
          Left = 109
          Top = 106
          Width = 51
          Height = 21
          Hint = 
            #24403#21069#23041#21147#35745#31639#65306#22522#30784#23041#21147' + '#33268#21629#19968#20987#20260#23475#22686#21152#13#10#13#10#26292#20987#65306#24403#21069#23041#21147' < '#20250#24515#19968#20987#25152#38656#23041#21147#13#10#20250#24515#19968#20987#65306#20250#24515#19968#20987#25152#38656#23041#21147' <= '#24403 +
            #21069#23041#21147' < '#21331#36234#19968#20987#25152#38656#23041#21147#13#10#21331#36234#19968#20987#65306#21331#36234#19968#20987#25152#38656#23041#21147' <= '#24403#21069#23041#21147' < '#33268#21629#19968#20987#25152#38656#23041#21147#13#10#33268#21629#19968#20987#65306#24403#21069#23041#21147' >= '#33268 +
            #21629#19968#20987#25152#38656#23041#21147
          MaxValue = 10000
          MinValue = 100
          TabOrder = 4
          Value = 100
          OnChange = seFatalBlowNeedPower2Change
        end
        object seFatalBlowNeedPower3: TSpinEditEx
          Left = 109
          Top = 129
          Width = 51
          Height = 21
          Hint = 
            #24403#21069#23041#21147#35745#31639#65306#22522#30784#23041#21147' + '#33268#21629#19968#20987#20260#23475#22686#21152#13#10#13#10#26292#20987#65306#24403#21069#23041#21147' < '#20250#24515#19968#20987#25152#38656#23041#21147#13#10#20250#24515#19968#20987#65306#20250#24515#19968#20987#25152#38656#23041#21147' <= '#24403 +
            #21069#23041#21147' < '#21331#36234#19968#20987#25152#38656#23041#21147#13#10#21331#36234#19968#20987#65306#21331#36234#19968#20987#25152#38656#23041#21147' <= '#24403#21069#23041#21147' < '#33268#21629#19968#20987#25152#38656#23041#21147#13#10#33268#21629#19968#20987#65306#24403#21069#23041#21147' >= '#33268 +
            #21629#19968#20987#25152#38656#23041#21147
          MaxValue = 10000
          MinValue = 100
          TabOrder = 5
          Value = 100
          OnChange = seFatalBlowNeedPower3Change
        end
      end
      object chkCloseDefenseUseScale: TCheckBox
        Left = 2
        Top = 160
        Width = 106
        Height = 17
        Hint = 
          #26410#21246#36873#26102#65292#24573#35270#38450#24481#20026#20960#29575#65292#27604#22914'20'#65292#34920#31034'20%'#30340#20960#29575#24573#35270#30446#26631#25152#26377#38450#24481#13#10#21246#36873#21518#65292#24573#35270#38450#24481#20026#27604#20363#65292#27604#22914'20'#65292#34920#31034#24573#35270#30446#26631'20%'#30340#38450 +
          #24481
        Caption = #24573#35270#38450#24481#31639#27604#20363
        TabOrder = 7
        OnClick = chkCloseDefenseUseScaleClick
      end
      object chkReboundUseScale: TCheckBox
        Left = 2
        Top = 179
        Width = 106
        Height = 17
        Hint = 
          #26410#21246#36873#26102#65292#20260#23475#21453#24377#20026#20960#29575#65292#27604#22914'20'#65292#34920#31034'20%'#30340#20960#29575#21453#24377#30446#26631#20260#23475#65288#21453#24377#20540#20026#8220#21453#24377#20260#23475#20493#29575#8221#65289#13#10#21246#36873#21518#65292#20260#23475#21453#24377#20026#27604#20363#65292#27604#22914'20' +
          #65292#34920#31034#21453#24377#24403#21069#20260#23475#30340'20%'
        Caption = #20260#23475#21453#24377#31639#27604#20363
        TabOrder = 8
        OnClick = chkReboundUseScaleClick
      end
    end
  end
end
