object FrmDummySetting: TFrmDummySetting
  Left = 571
  Top = 446
  BorderStyle = bsDialog
  BorderWidth = 5
  Caption = #20551#20154#35774#32622
  ClientHeight = 371
  ClientWidth = 451
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 13
  object pgcMain: TPageControl
    Left = 0
    Top = 0
    Width = 451
    Height = 335
    ActivePage = ts1
    Align = alTop
    TabOrder = 0
    ExplicitWidth = 447
    object ts1: TTabSheet
      Caption = #30331#24405#35774#32622
      object GroupBox1: TGroupBox
        Left = 4
        Top = 2
        Width = 196
        Height = 301
        Caption = #20551#20154#21015#34920
        TabOrder = 0
        object lstDummyList: TListBox
          Left = 8
          Top = 16
          Width = 180
          Height = 277
          Style = lbOwnerDrawVariable
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 12
          MultiSelect = True
          TabOrder = 0
          OnClick = lstDummyListClick
        end
      end
      object GroupBox2: TGroupBox
        Left = 207
        Top = 2
        Width = 231
        Height = 73
        TabOrder = 1
        object Label1: TLabel
          Left = 16
          Top = 20
          Width = 52
          Height = 13
          Caption = #35282#33394#21517#31216':'
        end
        object edtDummyName: TEdit
          Left = 71
          Top = 16
          Width = 150
          Height = 21
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
        end
        object btnDummyAdd: TButton
          Left = 99
          Top = 40
          Width = 57
          Height = 25
          Caption = #22686#21152'(&A)'
          TabOrder = 1
          OnClick = btnDummyAddClick
        end
        object btnDummyDel: TButton
          Left = 163
          Top = 40
          Width = 57
          Height = 25
          Caption = #21024#38500'(&D)'
          Enabled = False
          TabOrder = 2
          OnClick = btnDummyDelClick
        end
      end
      object GroupBox3: TGroupBox
        Left = 207
        Top = 81
        Width = 231
        Height = 142
        Caption = #30331#24405#35774#32622
        TabOrder = 2
        object Label2: TLabel
          Left = 10
          Top = 44
          Width = 58
          Height = 13
          Caption = #20986#29983#22352#26631'X:'
        end
        object Label4: TLabel
          Left = 16
          Top = 20
          Width = 52
          Height = 13
          Caption = #20986#29983#22320#22270':'
        end
        object Label3: TLabel
          Left = 10
          Top = 68
          Width = 58
          Height = 13
          Caption = #20986#29983#22352#26631'Y:'
        end
        object Label31: TLabel
          Left = 16
          Top = 92
          Width = 52
          Height = 13
          Caption = #30331#24405#36895#24230':'
        end
        object Label6: TLabel
          Left = 143
          Top = 93
          Width = 78
          Height = 13
          Caption = #31186#30331#24405'1'#20010#20551#20154
        end
        object seDummyHomeX: TSpinEditEx
          Left = 71
          Top = 40
          Width = 70
          Height = 22
          MaxValue = 2000
          MinValue = 1
          TabOrder = 1
          Value = 10
          OnChange = seDummyHomeXChange
        end
        object seDummyHomeY: TSpinEditEx
          Left = 71
          Top = 64
          Width = 70
          Height = 22
          MaxValue = 2000
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = seDummyHomeYChange
        end
        object edtDummyHomeMap: TEdit
          Left = 71
          Top = 16
          Width = 150
          Height = 21
          ImeName = #20013#25991' ('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          TabOrder = 0
          Text = '3'
          OnChange = edtDummyHomeMapChange
        end
        object seDummyLogonTime: TSpinEditEx
          Left = 71
          Top = 88
          Width = 70
          Height = 22
          MaxValue = 2000
          MinValue = 1
          TabOrder = 3
          Value = 10
          OnChange = seDummyLogonTimeChange
        end
        object chkDummyLogonRand: TCheckBox
          Left = 72
          Top = 117
          Width = 97
          Height = 17
          Hint = #26410#21246#36873#21017#26681#25454#21015#34920#39034#24207#30331#24405#65292#21246#36873#21518#21017#38543#26426#30331#24405
          Caption = #20551#20154#38543#26426#30331#24405
          TabOrder = 4
          OnClick = chkDummyLogonRandClick
        end
      end
      object btnDummyLogon: TButton
        Left = 357
        Top = 229
        Width = 81
        Height = 25
        Caption = #30331#24405'(&L)'
        Enabled = False
        TabOrder = 3
        OnClick = btnDummyLogonClick
      end
    end
    object ts2: TTabSheet
      Caption = #22522#26412#35774#32622
      ImageIndex = 1
      object GroupBox5: TGroupBox
        Left = 314
        Top = 2
        Width = 126
        Height = 44
        Caption = #35013#22791#20462#29702
        TabOrder = 0
        object CheckBoxDummyAutoRepairItem: TCheckBox
          Left = 7
          Top = 19
          Width = 89
          Height = 17
          Caption = #33258#21160#20462#29702#35013#22791
          TabOrder = 0
          OnClick = CheckBoxDummyAutoRepairItemClick
        end
      end
      object GroupBox6: TGroupBox
        Left = 314
        Top = 192
        Width = 126
        Height = 89
        Caption = #25915#20987#38388#38548'('#27627#31186')'
        TabOrder = 1
        object Label7: TLabel
          Left = 8
          Top = 18
          Width = 28
          Height = 13
          Caption = #25112#22763':'
        end
        object Label8: TLabel
          Left = 8
          Top = 42
          Width = 28
          Height = 13
          Caption = #27861#24072':'
        end
        object Label9: TLabel
          Left = 8
          Top = 66
          Width = 28
          Height = 13
          Caption = #36947#22763':'
        end
        object EditDummyWarrorAttackTime: TSpinEditEx
          Left = 42
          Top = 16
          Width = 75
          Height = 22
          MaxValue = 10000
          MinValue = 10
          TabOrder = 0
          Value = 10
          OnChange = EditDummyWarrorAttackTimeChange
        end
        object EditDummyTaoistAttackTime: TSpinEditEx
          Left = 42
          Top = 64
          Width = 75
          Height = 22
          MaxValue = 10000
          MinValue = 10
          TabOrder = 2
          Value = 10
          OnChange = EditDummyTaoistAttackTimeChange
        end
        object EditDummyWizardAttackTime: TSpinEditEx
          Left = 42
          Top = 40
          Width = 75
          Height = 22
          MaxValue = 10000
          MinValue = 10
          TabOrder = 1
          Value = 10
          OnChange = EditDummyWizardAttackTimeChange
        end
      end
      object GroupBox7: TGroupBox
        Left = 314
        Top = 99
        Width = 126
        Height = 89
        Caption = #34892#36208#38388#38548'('#27627#31186')'
        TabOrder = 2
        object Label10: TLabel
          Left = 8
          Top = 18
          Width = 28
          Height = 13
          Caption = #25112#22763':'
        end
        object Label11: TLabel
          Left = 8
          Top = 42
          Width = 28
          Height = 13
          Caption = #27861#24072':'
        end
        object Label12: TLabel
          Left = 8
          Top = 66
          Width = 28
          Height = 13
          Caption = #36947#22763':'
        end
        object EditDummyWarrorWalkTime: TSpinEditEx
          Left = 42
          Top = 16
          Width = 75
          Height = 22
          MaxValue = 10000
          MinValue = 10
          TabOrder = 0
          Value = 10
          OnChange = EditDummyWarrorWalkTimeChange
        end
        object EditDummyWizardWalkTime: TSpinEditEx
          Left = 42
          Top = 40
          Width = 75
          Height = 22
          MaxValue = 10000
          MinValue = 10
          TabOrder = 1
          Value = 10
          OnChange = EditDummyWizardWalkTimeChange
        end
        object EditDummyTaoistWalkTime: TSpinEditEx
          Left = 42
          Top = 64
          Width = 75
          Height = 22
          MaxValue = 10000
          MinValue = 10
          TabOrder = 2
          Value = 10
          OnChange = EditDummyTaoistWalkTimeChange
        end
      end
      object GroupBox8: TGroupBox
        Left = 314
        Top = 50
        Width = 126
        Height = 44
        Caption = #33521#38596#27515#20129#33258#21160#21484#21796
        TabOrder = 3
        object CheckBoxDummyAutoRecallHero: TCheckBox
          Left = 6
          Top = 18
          Width = 117
          Height = 17
          Caption = #33521#38596#27515#20129#33258#21160#21484#21796
          TabOrder = 0
          OnClick = CheckBoxDummyAutoRecallHeroClick
        end
      end
      object GroupBox9: TGroupBox
        Left = 4
        Top = 50
        Width = 151
        Height = 252
        Caption = #20154#29289#24674#22797#36895#24230'('#22522#25968')'
        TabOrder = 4
        object Label13: TLabel
          Left = 7
          Top = 20
          Width = 84
          Height = 13
          Caption = #25112#22763#20307#21147#36895#24230#65306
        end
        object Label14: TLabel
          Left = 7
          Top = 81
          Width = 84
          Height = 13
          Caption = #25112#22763#39764#27861#36895#24230#65306
        end
        object Label15: TLabel
          Left = 7
          Top = 142
          Width = 84
          Height = 13
          Caption = #36947#27861#20307#21147#36895#24230#65306
        end
        object Bevel1: TBevel
          Left = 2
          Top = 129
          Width = 144
          Height = 4
          Shape = bsBottomLine
        end
        object Label16: TLabel
          Left = 7
          Top = 168
          Width = 84
          Height = 13
          Caption = #36947#27861#20307#21147#22522#25968#65306
        end
        object Label17: TLabel
          Left = 7
          Top = 46
          Width = 84
          Height = 13
          Caption = #25112#22763#20307#21147#22522#25968#65306
        end
        object Label18: TLabel
          Left = 7
          Top = 107
          Width = 84
          Height = 13
          Caption = #25112#22763#39764#27861#22522#25968#65306
        end
        object Bevel5: TBevel
          Left = 2
          Top = 68
          Width = 144
          Height = 4
          Shape = bsBottomLine
        end
        object Label19: TLabel
          Left = 7
          Top = 203
          Width = 84
          Height = 13
          Caption = #36947#27861#39764#27861#36895#24230#65306
        end
        object Label20: TLabel
          Left = 7
          Top = 229
          Width = 84
          Height = 13
          Caption = #36947#27861#39764#27861#22522#25968#65306
        end
        object Bevel3: TBevel
          Left = 3
          Top = 190
          Width = 144
          Height = 4
          Shape = bsBottomLine
        end
        object seDummyHPTime_Warrior: TSpinEditEx
          Left = 91
          Top = 16
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seDummyHPTime_WarriorChange
        end
        object seDummyMPTime_Warrior: TSpinEditEx
          Left = 91
          Top = 77
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seDummyMPTime_WarriorChange
        end
        object seDummyHPTime_DF: TSpinEditEx
          Left = 91
          Top = 138
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seDummyHPTime_DFChange
        end
        object seDummyHPBase_DF: TSpinEditEx
          Left = 91
          Top = 164
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seDummyHPBase_DFChange
        end
        object seDummyHPBase_Warrior: TSpinEditEx
          Left = 91
          Top = 41
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 5
          OnChange = seDummyHPBase_WarriorChange
        end
        object seDummyMPBase_Warrior: TSpinEditEx
          Left = 91
          Top = 103
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 5
          OnChange = seDummyMPBase_WarriorChange
        end
        object seDummyMPTime_DF: TSpinEditEx
          Left = 91
          Top = 199
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 5
          OnChange = seDummyMPTime_DFChange
        end
        object seDummyMPBase_DF: TSpinEditEx
          Left = 91
          Top = 225
          Width = 53
          Height = 22
          Hint = #20154#29289#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 5
          OnChange = seDummyMPBase_DFChange
        end
      end
      object GroupBox10: TGroupBox
        Left = 159
        Top = 50
        Width = 151
        Height = 252
        Caption = #33521#38596#24674#22797#36895#24230'('#22522#25968')'
        TabOrder = 5
        object Label21: TLabel
          Left = 7
          Top = 20
          Width = 84
          Height = 13
          Caption = #25112#22763#20307#21147#36895#24230#65306
        end
        object Label22: TLabel
          Left = 7
          Top = 81
          Width = 84
          Height = 13
          Caption = #25112#22763#39764#27861#36895#24230#65306
        end
        object Label23: TLabel
          Left = 7
          Top = 145
          Width = 84
          Height = 13
          Caption = #36947#27861#20307#21147#36895#24230#65306
        end
        object Label24: TLabel
          Left = 7
          Top = 203
          Width = 84
          Height = 13
          Caption = #36947#27861#39764#27861#36895#24230#65306
        end
        object Bevel2: TBevel
          Left = 3
          Top = 129
          Width = 142
          Height = 4
          Shape = bsBottomLine
        end
        object Label25: TLabel
          Left = 7
          Top = 168
          Width = 84
          Height = 13
          Caption = #36947#27861#20307#21147#22522#25968#65306
        end
        object Label26: TLabel
          Left = 7
          Top = 229
          Width = 84
          Height = 13
          Caption = #36947#27861#39764#27861#22522#25968#65306
        end
        object Bevel4: TBevel
          Left = 3
          Top = 190
          Width = 142
          Height = 4
          Shape = bsBottomLine
        end
        object Label27: TLabel
          Left = 7
          Top = 46
          Width = 84
          Height = 13
          Caption = #25112#22763#20307#21147#22522#25968#65306
        end
        object Bevel6: TBevel
          Left = 3
          Top = 68
          Width = 142
          Height = 4
          Shape = bsBottomLine
        end
        object Label28: TLabel
          Left = 7
          Top = 107
          Width = 84
          Height = 13
          Caption = #25112#22763#39764#27861#22522#25968#65306
        end
        object seDummyHeroHPTime_Warrior: TSpinEditEx
          Left = 91
          Top = 16
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555'.'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seDummyHeroHPTime_WarriorChange
        end
        object seDummyHeroMPTime_Warrior: TSpinEditEx
          Left = 91
          Top = 76
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 5
          OnChange = seDummyHeroMPTime_WarriorChange
        end
        object seDummyHeroHPTime_DF: TSpinEditEx
          Left = 91
          Top = 141
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797#38388#38548','#21442#25968#36234#23567#24674#22797'hp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 5
          OnChange = seDummyHeroHPTime_DFChange
        end
        object seDummyHeroMPTime_DF: TSpinEditEx
          Left = 91
          Top = 198
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797#38388#38548#38388#38548','#21442#25968#36234#23567#24674#22797'mp'#38388#38548#36234#24555
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 5
          OnChange = seDummyHeroMPTime_DFChange
        end
        object seDummyHeroHPBase_DF: TSpinEditEx
          Left = 91
          Top = 164
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 5
          OnChange = seDummyHeroHPBase_DFChange
        end
        object seDummyHeroMPBase_DF: TSpinEditEx
          Left = 91
          Top = 225
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          Value = 5
          OnChange = seDummyHeroMPBase_DFChange
        end
        object seDummyHeroHPBase_Warrior: TSpinEditEx
          Left = 91
          Top = 41
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#20307#21147#24674#22797'hp'#25968#20540','#25968#23383#36234#23567#24674#22797'hp'#20540#36234#39640','#40664#35748'75.'#27599#27425#21152'hp = MaxHP '#38500#20197' '#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          Value = 5
          OnChange = seDummyHeroHPBase_WarriorChange
        end
        object seDummyHeroMPBase_Warrior: TSpinEditEx
          Left = 91
          Top = 103
          Width = 53
          Height = 22
          Hint = #33521#38596#33258#36523#39764#27861#24674#22797'mp'#25968#20540','#25968#23383#36234#23567#24674#22797'MP'#36234#39640','#40664#35748'18.'#27599#27425'MP = MaxMP '#38500#20197#22522#25968' + 1'
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          Value = 5
          OnChange = seDummyHeroMPBase_WarriorChange
        end
      end
      object grp14: TGroupBox
        Left = 4
        Top = 2
        Width = 306
        Height = 44
        Caption = 'HP/MP'#24674#22797#25511#21046
        TabOrder = 6
        object Label29: TLabel
          Left = 126
          Top = 20
          Width = 26
          Height = 13
          Caption = '% '#26102
        end
        object Label30: TLabel
          Left = 275
          Top = 20
          Width = 26
          Height = 13
          Caption = '% '#26102
        end
        object chkDummyAutoAddHP: TCheckBox
          Left = 7
          Top = 18
          Width = 73
          Height = 17
          Caption = #24674#22797'HP<='
          TabOrder = 0
          OnClick = chkDummyAutoAddHPClick
        end
        object seDummyAddHPPercent: TSpinEditEx
          Left = 82
          Top = 16
          Width = 42
          Height = 22
          MaxValue = 100
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 60
          OnChange = seDummyAddHPPercentChange
        end
        object chkDummyAutoAddMP: TCheckBox
          Left = 157
          Top = 18
          Width = 73
          Height = 17
          Caption = #24674#22797'MP<='
          TabOrder = 2
          OnClick = chkDummyAutoAddMPClick
        end
        object seDummyAddMPPercent: TSpinEditEx
          Left = 231
          Top = 16
          Width = 42
          Height = 22
          MaxValue = 100
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Value = 60
          OnChange = seDummyAddMPPercentChange
        end
      end
    end
    object ts3: TTabSheet
      Caption = #31359#20154#35774#32622
      ImageIndex = 2
      object GroupBox11: TGroupBox
        Left = 4
        Top = 2
        Width = 183
        Height = 301
        Caption = #36305#27493#31359#20154#25511#21046
        TabOrder = 0
        object chkDisDummyRun: TCheckBox
          Left = 4
          Top = 17
          Width = 160
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20551#20154#23558#19981#20801#35768#31359#36807#24618#29289#25110#20854#23427#20154#29289
          Caption = #31105#27490#36305#27493#31359#20154
          TabOrder = 0
          OnClick = chkDisDummyRunClick
        end
        object chkDummyRunHum: TCheckBox
          Left = 20
          Top = 36
          Width = 160
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20551#20154#23558#21487#20197#31359#36807#20854#20182#20154#29289
          Caption = #20801#35768#31359#36807#20154#29289
          TabOrder = 1
          OnClick = chkDummyRunHumClick
        end
        object chkDummyRunMon: TCheckBox
          Left = 20
          Top = 55
          Width = 160
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20551#20154#23558#21487#20197#31359#36807#24618#29289
          Caption = #20801#35768#31359#36807#24618#29289
          TabOrder = 2
          OnClick = chkDummyRunMonClick
        end
        object chkDummyRunNpc: TCheckBox
          Left = 20
          Top = 74
          Width = 160
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20551#20154#23558#21487#20197#31359#36807'NPC'
          Caption = #20801#35768#31359#36807'NPC'
          TabOrder = 3
          OnClick = chkDummyRunNpcClick
        end
        object chkDummyRunGuard: TCheckBox
          Left = 20
          Top = 93
          Width = 160
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#20551#20154#23558#21487#20197#31359#36807#23432#21355'('#22823#20992#12289#24339#31661#25163')'
          Caption = #20801#35768#31359#36807#23432#21355
          TabOrder = 4
          OnClick = chkDummyRunGuardClick
        end
        object chkDummySafeArea: TCheckBox
          Left = 20
          Top = 112
          Width = 160
          Height = 17
          Caption = #23433#20840#21306#19981#21463#25511#21046
          TabOrder = 5
          OnClick = chkDummySafeAreaClick
        end
        object chkDummySafeAreaDisNpcRun: TCheckBox
          Left = 20
          Top = 169
          Width = 160
          Height = 17
          Caption = #23433#20840#21306#31105#27490#31359'NPC'
          TabOrder = 6
          OnClick = chkDummySafeAreaDisNpcRunClick
        end
        object chkSafeAreaDisShopStallDummyRun: TCheckBox
          Left = 20
          Top = 188
          Width = 160
          Height = 17
          Caption = #23433#20840#21306#31105#27490#31359#25670#25674#20154#29289
          TabOrder = 7
          OnClick = chkSafeAreaDisShopStallDummyRunClick
        end
        object chkSafeAreaDisOffLineDummyRun: TCheckBox
          Left = 20
          Top = 207
          Width = 160
          Height = 17
          Caption = #23433#20840#21306#31105#27490#31359#31163#32447#20154#29289
          TabOrder = 8
          OnClick = chkSafeAreaDisOffLineDummyRunClick
        end
        object chkDummyWarDisHumRun: TCheckBox
          Left = 20
          Top = 131
          Width = 160
          Height = 17
          Hint = #25171#24320#27492#21151#33021#21518#65292#22312#25915#22478#21306#22495#65292#23558#31105#27490#31359#20154#21450#24618#29289
          Caption = #25915#22478#21306#22495#20840#37096#31105#27490
          TabOrder = 9
          OnClick = chkDummyWarDisHumRunClick
        end
        object chkDummyWarHreoRun: TCheckBox
          Left = 20
          Top = 150
          Width = 160
          Height = 17
          Hint = #25171#24320#35813#39033#21518#25915#22478#21306#22495'['#25915#22478#26399#38388']'#23558#20801#35768#31359#36807#33521#38596
          Caption = #25915#22478#21306#22495#20801#35768#31359#33521#38596
          Enabled = False
          TabOrder = 10
          OnClick = chkDummyWarHreoRunClick
        end
      end
    end
    object ts4: TTabSheet
      Caption = #31105#27490#22320#22270
      ImageIndex = 3
      object GroupBox12: TGroupBox
        Left = 4
        Top = 2
        Width = 167
        Height = 301
        Caption = #31105#27490#22320#22270#21015#34920
        TabOrder = 0
        object lstDisableMoveMap: TListBox
          Left = 8
          Top = 16
          Width = 150
          Height = 277
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 13
          TabOrder = 0
          OnClick = lstDisableMoveMapClick
        end
      end
      object btnDisableMoveMapAdd: TButton
        Left = 184
        Top = 8
        Width = 73
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 1
        OnClick = btnDisableMoveMapAddClick
      end
      object btnDisableMoveMapDelete: TButton
        Left = 184
        Top = 40
        Width = 73
        Height = 25
        Caption = #21024#38500'(&D)'
        Enabled = False
        TabOrder = 2
        OnClick = btnDisableMoveMapDeleteClick
      end
      object btnDisableMoveMapAddAll: TButton
        Left = 184
        Top = 72
        Width = 73
        Height = 25
        Caption = #20840#37096#22686#21152'(&A)'
        TabOrder = 3
        OnClick = btnDisableMoveMapAddAllClick
      end
      object btnDisableMoveMapDeleteAll: TButton
        Left = 184
        Top = 104
        Width = 73
        Height = 25
        Caption = #20840#37096#21024#38500'(&D)'
        TabOrder = 4
        OnClick = btnDisableMoveMapDeleteAllClick
      end
      object btnDisableMoveMapSave: TButton
        Left = 184
        Top = 136
        Width = 73
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 5
        Visible = False
        OnClick = btnDisableMoveMapSaveClick
      end
      object GroupBox13: TGroupBox
        Left = 271
        Top = 2
        Width = 167
        Height = 299
        Caption = #22320#22270#21015#34920
        TabOrder = 6
        object lstMapList: TListBox
          Left = 8
          Top = 16
          Width = 150
          Height = 275
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 13
          MultiSelect = True
          TabOrder = 0
        end
      end
    end
    object ts5: TTabSheet
      Caption = #19981#20027#21160#25915#20987#24618#29289
      ImageIndex = 4
      object GroupBox14: TGroupBox
        Left = 4
        Top = 2
        Width = 167
        Height = 301
        Caption = #19981#20027#21160#25915#20987#24618#29289#21015#34920
        TabOrder = 0
        object lstNoAttackMonList: TListBox
          Left = 8
          Top = 16
          Width = 150
          Height = 278
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 13
          TabOrder = 0
          OnClick = lstNoAttackMonListClick
        end
      end
      object btnNoAttackMonDel: TButton
        Left = 184
        Top = 40
        Width = 73
        Height = 25
        Caption = #21024#38500'(&D)'
        Enabled = False
        TabOrder = 1
        OnClick = btnNoAttackMonDelClick
      end
      object btnNoAttackMonAddAll: TButton
        Left = 184
        Top = 72
        Width = 73
        Height = 25
        Caption = #20840#37096#22686#21152'(&A)'
        TabOrder = 2
        OnClick = btnNoAttackMonAddAllClick
      end
      object btnNoAttackMonDelAll: TButton
        Left = 184
        Top = 104
        Width = 73
        Height = 25
        Caption = #20840#37096#21024#38500'(&D)'
        TabOrder = 3
        OnClick = btnNoAttackMonDelAllClick
      end
      object btnNoAttackMonSave: TButton
        Left = 184
        Top = 136
        Width = 73
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 4
        Visible = False
        OnClick = btnNoAttackMonSaveClick
      end
      object GroupBox15: TGroupBox
        Left = 271
        Top = 2
        Width = 167
        Height = 301
        Caption = #24618#29289#21015#34920
        TabOrder = 5
        object lstMonList: TListBox
          Left = 8
          Top = 16
          Width = 150
          Height = 278
          ImeName = #20013#25991'('#31616#20307') - '#25628#29399#25340#38899#36755#20837#27861
          ItemHeight = 13
          MultiSelect = True
          TabOrder = 0
        end
      end
      object btnNoAttackMonAdd: TButton
        Left = 184
        Top = 8
        Width = 73
        Height = 25
        Caption = #22686#21152'(&A)'
        TabOrder = 6
        OnClick = btnNoAttackMonAddClick
      end
    end
  end
  object ButtonDummySave: TButton
    Left = 376
    Top = 343
    Width = 75
    Height = 25
    Caption = #20445#23384'(&S)'
    TabOrder = 1
    OnClick = ButtonDummySaveClick
  end
  object btn1: TButton
    Left = 290
    Top = 343
    Width = 75
    Height = 25
    Caption = #40664#35748
    TabOrder = 2
    OnClick = btn1Click
  end
end
