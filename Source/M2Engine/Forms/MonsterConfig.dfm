object frmMonsterConfig: TfrmMonsterConfig
  Left = 356
  Top = 427
  BorderIcons = [biSystemMenu, biMinimize]
  BorderStyle = bsSingle
  Caption = #24618#29289#35774#32622
  ClientHeight = 717
  ClientWidth = 911
  Color = clBtnFace
  Font.Charset = GB2312_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = #23435#20307
  Font.Style = []
  Position = poMainFormCenter
  OnCreate = FormCreate
  TextHeight = 12
  object PageControl1: TPageControl
    Left = 5
    Top = 6
    Width = 902
    Height = 703
    ActivePage = TabSheet1
    TabOrder = 0
    object TabSheet1: TTabSheet
      Caption = #22522#26412#21442#25968
      object GroupBox3: TGroupBox
        Left = 4
        Top = 142
        Width = 201
        Height = 50
        Caption = #23608#20307#28165#29702#26102#38388#24310#38271#35774#32622
        TabOrder = 1
        object Label9: TLabel
          Left = 9
          Top = 25
          Width = 60
          Height = 12
          Caption = #24310#38271#26102#38388#65306
        end
        object Label10: TLabel
          Left = 140
          Top = 25
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seMonButchDelayClearTime: TSpinEditEx
          Left = 66
          Top = 21
          Width = 70
          Height = 21
          Hint = #24618#29289#19968#20294#34987#25366#65292#21017#33258#21160#21551#29992#24310#26102#21151#33021#65292#20197#20813#30001#20110#35774#32622#28165#29702#24618#29289#26102#38388#22826#30701#32780#36896#25104#26080#27861#25366#21462#24618#29289#36523#19978#29289#21697#12290#40664#35748' 60'#31186
          MaxValue = 99999999
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 10
          OnChange = seMonButchDelayClearTimeChange
        end
      end
      object GroupBox10: TGroupBox
        Left = 4
        Top = 202
        Width = 201
        Height = 65
        Caption = #24618#29289#31561#32423#26174#31034#35774#32622
        TabOrder = 2
        object lbl1: TLabel
          Left = 9
          Top = 43
          Width = 60
          Height = 12
          Caption = #26174#31034#26684#24335#65306
        end
        object lbl33: TLabel
          Left = 9
          Top = 20
          Width = 60
          Height = 12
          Caption = #26174#31034#31561#32423#65306
        end
        object edtMonsterShowFormat: TEdit
          Left = 64
          Top = 39
          Width = 125
          Height = 20
          Hint = #26174#31034#26684#24335#19981#25903#25345'/'#65292#19981#28982#20250#23548#33268#24618#29289#39068#33394#38169#35823
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnChange = edtMonsterShowFormatChange
        end
        object cbbMonsterShowLevel: TComboBox
          Left = 64
          Top = 16
          Width = 125
          Height = 20
          Style = csDropDownList
          TabOrder = 1
          OnChange = cbbMonsterShowLevelChange
          Items.Strings = (
            #19981#26174#31034
            #19968#30452#26174#31034
            #40736#26631#25351#21521#26102#26174#31034)
        end
      end
      object ButtonGeneralSave: TButton
        Left = 303
        Top = 198
        Width = 65
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 3
        OnClick = ButtonGeneralSaveClick
      end
      object GroupBox1: TGroupBox
        Left = 212
        Top = 6
        Width = 156
        Height = 75
        Caption = #26234#33021#21047#24618#35774#32622
        TabOrder = 0
        object Label11: TLabel
          Left = 8
          Top = 50
          Width = 60
          Height = 12
          Caption = #28165#29702#38388#38548#65306
          ParentShowHint = False
          ShowHint = False
        end
        object Label12: TLabel
          Left = 121
          Top = 50
          Width = 24
          Height = 12
          Caption = #20998#38047
        end
        object seNoHumanClearMonTime: TSpinEditEx
          Left = 65
          Top = 46
          Width = 53
          Height = 21
          Hint = 
            #24403#22320#22270#27809#26377#20219#20309#20154#29289#23384#22312#65292#26102#38388#36798#21040#35813#35774#32622#23450#65292#31995#32479#23558#28165#38500#35813#22320#22270#25152#26377#24618#29289#65294#21482#23545#26234#33021#21047#24618#22320#22270#26377#25928#65294#13#10#13#10'BOSS'#22320#22270#24314#35758#19981#35201#28165#29702#65292#25110#32773 +
            #28165#29702#38388#38548#19981#35201#22826#30701#65281
          MaxValue = 99999999
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 10
          OnChange = seNoHumanClearMonTimeChange
        end
        object chkNoHumanClearMon: TCheckBox
          Left = 8
          Top = 23
          Width = 139
          Height = 17
          Hint = #24403#22320#22270#27809#26377#20219#20309#20154#29289#23384#22312#65292#26102#38388#36798#21040#35813#35774#32622#23450#65292#31995#32479#23558#28165#38500#35813#22320#22270#25152#26377#24618#29289#65294#21482#23545#26234#33021#21047#24618#22320#22270#26377#25928#65294
          Caption = #33258#21160#28165#38500#26080#20154#22320#22270#24618#29289
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = chkNoHumanClearMonClick
        end
      end
      object grp15: TGroupBox
        Left = 212
        Top = 88
        Width = 156
        Height = 50
        Caption = #31070#20861'/'#22307#20861' '#29228#19979#26102#38388#24310#26102
        TabOrder = 4
        object Label212: TLabel
          Left = 9
          Top = 25
          Width = 60
          Height = 12
          Caption = #24310#26102#26102#38388#65306
        end
        object Label213: TLabel
          Left = 133
          Top = 25
          Width = 12
          Height = 12
          Caption = #31186
        end
        object seElfWarriorMonsterDownDelay: TSpinEditEx
          Left = 66
          Top = 21
          Width = 64
          Height = 21
          Hint = #31070#20861#26080#25915#20987#30446#26631#21518#65292#20174#31449#31435#21040#29228#19979#30340#24310#26102#26102#38388
          MaxValue = 99999999
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 10
          OnChange = seElfWarriorMonsterDownDelayChange
        end
      end
      object GroupBox188: TGroupBox
        Left = 4
        Top = 5
        Width = 201
        Height = 127
        Caption = #29190#29289#21697#35774#32622
        TabOrder = 5
        object Label783: TLabel
          Left = 9
          Top = 86
          Width = 72
          Height = 12
          Caption = #26368#22823#37329#24065#22534#65306
        end
        object Label784: TLabel
          Left = 8
          Top = 62
          Width = 72
          Height = 12
          Caption = #29190#29289#21697#33539#22260#65306
        end
        object seMaxMapItemCount: TSpinEditEx
          Left = 130
          Top = 14
          Width = 61
          Height = 21
          Hint = #38480#21046#22320#22270#21516#19968#22352#26631#19978#20801#35768#37325#21472#22810#20214#29289#21697#65292#25968#37327#24314#35758'3'#20197#19978#65292#21542#21017#24618#29289#29190#29575#25110#20154#29289#20002#24323#33539#22260#20869#22352#26631#19978#29289#21697#36229#36807#35813#38480#20540#65292#29190#20986#25110#20002#24323#33258#21160#28040#22833
          MaxValue = 100
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 5
          OnChange = seMaxMapItemCountChange
        end
        object chkNotDropOverlapItemAll: TCheckBox
          Left = 8
          Top = 37
          Width = 145
          Height = 17
          Hint = #24403#20154#29289#25110#24618#29289#27515#20129#29190#20986#30340#29289#21697#21253#21547#21472#21152#29289#21697#26102#65292#21482#38543#26426#29190#20986#29289#21697#21472#21152#25968#37327
          Caption = #21472#21152#29289#21697#21482#29190#37096#20998#25968#37327
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkNotDropOverlapItemAllClick
        end
        object seMonOneDropGoldCount: TSpinEditEx
          Left = 77
          Top = 82
          Width = 61
          Height = 21
          MaxValue = 99999999
          MinValue = 1
          TabOrder = 2
          Value = 10
          OnChange = seMonOneDropGoldCountChange
        end
        object chkDropGoldToPlayBag: TCheckBox
          Left = 9
          Top = 105
          Width = 112
          Height = 17
          Caption = #37329#24065#30452#25509#20837#32972#21253
          TabOrder = 3
          OnClick = chkDropGoldToPlayBagClick
        end
        object seScatterItemRange: TSpinEditEx
          Left = 77
          Top = 58
          Width = 61
          Height = 21
          Hint = #27492#21442#25968#35774#32622#24618#29289#29190#29289#21697#33539#22260#65292#22914#26524#33539#22260#35774#32622#36807#23567#32780#24618#29289#29190#29289#21697#36807#22810#21017#20250#21472#21152#26174#31034
          MaxValue = 12
          MinValue = 1
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          Value = 3
          OnChange = seScatterItemRangeChange
        end
        object chkEnabledMaxMapItemCount: TCheckBox
          Left = 8
          Top = 16
          Width = 122
          Height = 17
          Caption = #21516#19968#22352#26631#29289#21697#25968#37327#65306
          TabOrder = 5
          OnClick = chkEnabledMaxMapItemCountClick
        end
      end
      object GroupBox22: TGroupBox
        Left = 212
        Top = 142
        Width = 156
        Height = 50
        Caption = #24618#29289#21518#20208#24103#24310#26102
        TabOrder = 6
        object Label222: TLabel
          Left = 9
          Top = 25
          Width = 60
          Height = 12
          Caption = #24310#26102#26102#38388#65306
        end
        object Label223: TLabel
          Left = 121
          Top = 25
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object seMonStruckFrameDelayTime: TSpinEditEx
          Left = 66
          Top = 21
          Width = 52
          Height = 21
          Hint = #24618#29289#21463#21040#25915#20987#21518#65292#21518#20208#21160#20316#24103#24310#26102#26102#38388
          MaxValue = 255
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 10
          OnChange = seMonStruckFrameDelayTimeChange
        end
      end
      object GroupBox23: TGroupBox
        Left = 4
        Top = 274
        Width = 201
        Height = 95
        Caption = #24618#29289#21463#39764#27861#25915#20987#21518#20943#24930
        TabOrder = 7
        object lbl19: TLabel
          Left = 16
          Top = 23
          Width = 84
          Height = 12
          Caption = #26368#39640#24618#29289#31561#32423#65306
        end
        object Label226: TLabel
          Left = 16
          Top = 47
          Width = 84
          Height = 12
          Caption = #22266#23450#20943#24930#26102#38388#65306
        end
        object Label227: TLabel
          Left = 16
          Top = 71
          Width = 84
          Height = 12
          Caption = #38543#26426#20943#24930#26102#38388#65306
        end
        object lbl40: TLabel
          Left = 167
          Top = 47
          Width = 24
          Height = 12
          Caption = #27627#31186
        end
        object edtMagStruckMonLevel: TSpinEditEx
          Left = 98
          Top = 18
          Width = 66
          Height = 21
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          Value = 10
          OnChange = edtMagStruckMonLevelChange
        end
        object edtMagStruckMonDecTime: TSpinEditEx
          Left = 98
          Top = 42
          Width = 66
          Height = 21
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          Value = 10
          OnChange = edtMagStruckMonDecTimeChange
        end
        object edtMagStruckMonDecRandom: TSpinEditEx
          Left = 98
          Top = 66
          Width = 66
          Height = 21
          MaxValue = 0
          MinValue = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          Value = 10
          OnChange = edtMagStruckMonDecRandomChange
        end
      end
    end
    object TabSheet2: TTabSheet
      Caption = #20154#24418#24618#35774#32622
      ImageIndex = 1
      object lbl22: TLabel
        Left = 8
        Top = 654
        Width = 414
        Height = 12
        Caption = #27880#24847#65306#35831#30830#23450'Mir200'#30446#24405#20869#26377'MonUseItems'#25991#20214#22841#65292#21542#21017#37197#32622#21518#31243#24207#21487#33021#25253#38169#65281
        Font.Charset = GB2312_CHARSET
        Font.Color = clRed
        Font.Height = -12
        Font.Name = #23435#20307
        Font.Style = []
        ParentFont = False
      end
      object GroupBox2: TGroupBox
        Left = 279
        Top = 2
        Width = 129
        Height = 526
        Caption = #20154#24418#24618#21015#34920
        TabOrder = 2
        object ListBoxMonsterList: TListBox
          Left = 8
          Top = 16
          Width = 113
          Height = 500
          Hint = 'Ctrl + F '#20154#24418#24618#26597#25214
          ItemHeight = 12
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = ListBoxMonsterListClick
          OnKeyDown = ListBoxMonsterListKeyDown
        end
      end
      object GroupBoxMonsterConfig: TGroupBox
        Left = 415
        Top = 2
        Width = 472
        Height = 526
        Caption = #37197#32622
        TabOrder = 3
        object GroupBoxMonsterUseItem: TGroupBox
          Left = 10
          Top = 15
          Width = 237
          Height = 405
          Caption = #36523#19978#35013#22791
          TabOrder = 0
          object Label31: TLabel
            Left = 8
            Top = 20
            Width = 42
            Height = 12
            Caption = #34915'  '#26381':'
          end
          object Label32: TLabel
            Tag = 1
            Left = 8
            Top = 44
            Width = 42
            Height = 12
            Caption = #27494'  '#22120':'
          end
          object Label33: TLabel
            Tag = 3
            Left = 8
            Top = 92
            Width = 42
            Height = 12
            Caption = #39033'  '#38142':'
          end
          object Label34: TLabel
            Tag = 2
            Left = 8
            Top = 68
            Width = 42
            Height = 12
            Caption = #21195'  '#31456':'
          end
          object Label35: TLabel
            Tag = 7
            Left = 8
            Top = 188
            Width = 42
            Height = 12
            Caption = #24038#25106#25351':'
          end
          object Label36: TLabel
            Tag = 6
            Left = 8
            Top = 164
            Width = 42
            Height = 12
            Caption = #21491#25163#38255':'
          end
          object Label37: TLabel
            Tag = 5
            Left = 8
            Top = 140
            Width = 42
            Height = 12
            Caption = #24038#25163#38255':'
          end
          object Label38: TLabel
            Tag = 4
            Left = 8
            Top = 116
            Width = 42
            Height = 12
            Caption = #22836'  '#30420':'
          end
          object Label39: TLabel
            Tag = 9
            Left = 8
            Top = 284
            Width = 42
            Height = 12
            Caption = #36947'  '#31526':'
          end
          object Label40: TLabel
            Tag = 8
            Left = 8
            Top = 212
            Width = 42
            Height = 12
            Caption = #21491#25106#25351':'
          end
          object Label41: TLabel
            Tag = 10
            Left = 8
            Top = 236
            Width = 42
            Height = 12
            Caption = #33136'  '#24102':'
          end
          object Label42: TLabel
            Tag = 11
            Left = 8
            Top = 260
            Width = 42
            Height = 12
            Caption = #38772'  '#23376':'
          end
          object Label43: TLabel
            Tag = 12
            Left = 8
            Top = 308
            Width = 42
            Height = 12
            Caption = #23453'  '#30707':'
          end
          object Label61: TLabel
            Tag = 12
            Left = 8
            Top = 332
            Width = 42
            Height = 12
            Caption = #26007'  '#31520':'
          end
          object Label66: TLabel
            Tag = 12
            Left = 8
            Top = 356
            Width = 42
            Height = 12
            Caption = #20891'  '#40723':'
          end
          object Label181: TLabel
            Tag = 12
            Left = 8
            Top = 380
            Width = 42
            Height = 12
            Caption = #30462'  '#29260':'
          end
          object EditDRESSNAME: TEdit
            Left = 56
            Top = 16
            Width = 172
            Height = 20
            Ctl3D = True
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            ParentCtl3D = False
            TabOrder = 0
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditWEAPONNAME: TEdit
            Tag = 1
            Left = 56
            Top = 40
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 1
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditNECKLACENAME: TEdit
            Tag = 3
            Left = 56
            Top = 88
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 3
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditRIGHTHANDNAME: TEdit
            Tag = 2
            Left = 56
            Top = 64
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 2
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditRINGLNAME: TEdit
            Tag = 7
            Left = 56
            Top = 184
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 7
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditARMRINGRNAME: TEdit
            Tag = 6
            Left = 56
            Top = 160
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 6
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditARMRINGLNAME: TEdit
            Tag = 5
            Left = 56
            Top = 136
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 5
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditHELMETNAME: TEdit
            Tag = 4
            Left = 56
            Top = 112
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 4
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditBELTNAME: TEdit
            Tag = 10
            Left = 56
            Top = 232
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 9
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditBUJUKNAME: TEdit
            Tag = 9
            Left = 56
            Top = 280
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 11
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditRINGRNAME: TEdit
            Tag = 8
            Left = 56
            Top = 208
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 8
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditBOOTSNAME: TEdit
            Tag = 11
            Left = 56
            Top = 256
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 10
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditCHARMNAME: TEdit
            Tag = 12
            Left = 56
            Top = 304
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 14
            TabOrder = 12
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditHATNAME: TEdit
            Tag = 13
            Left = 56
            Top = 328
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 30
            TabOrder = 13
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditDRUMNAME: TEdit
            Tag = 14
            Left = 56
            Top = 352
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 30
            TabOrder = 14
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object edtShield: TEdit
            Tag = 16
            Left = 56
            Top = 376
            Width = 172
            Height = 20
            ImeName = #20013#25991' ('#31616#20307') - '#32654#24335#38190#30424
            MaxLength = 30
            TabOrder = 15
            OnChange = EditDRESSNAMEChange
            OnDragDrop = GroupBoxMonsterUseItemDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
        end
        object GroupBox5: TGroupBox
          Left = 256
          Top = 15
          Width = 206
          Height = 217
          Caption = #20462#28860#39764#27861
          TabOrder = 1
          object Label7: TLabel
            Left = 138
            Top = 20
            Width = 54
            Height = 12
            Caption = #25216#33021#31561#32423':'
          end
          object Label8: TLabel
            Left = 138
            Top = 68
            Width = 54
            Height = 12
            Caption = #24378#21270#31561#32423':'
          end
          object ListBoxMonsterMagicList: TListBox
            Left = 8
            Top = 16
            Width = 123
            Height = 191
            Hint = #21452#20987#21487#20197#21024#38500#39764#27861
            ItemHeight = 12
            ParentShowHint = False
            ShowHint = True
            TabOrder = 0
            OnClick = ListBoxMonsterMagicListClick
            OnDblClick = ListBoxMonsterMagicListDblClick
            OnDragDrop = ListBoxMonsterMagicListDragDrop
            OnDragOver = ListBoxMonsterBagItemListDragOver
          end
          object EditMagicLevel: TSpinEditEx
            Left = 138
            Top = 36
            Width = 57
            Height = 21
            MaxValue = 3
            MinValue = 0
            TabOrder = 1
            Value = 0
            OnChange = EditMagicLevelChange
          end
          object EditMagicNewLevel: TSpinEditEx
            Left = 138
            Top = 84
            Width = 57
            Height = 21
            MaxValue = 9
            MinValue = 0
            TabOrder = 2
            Value = 0
            OnChange = EditMagicLevelChange
          end
        end
        object ButtonMonUseItemsSave: TButton
          Left = 138
          Top = 491
          Width = 85
          Height = 25
          Caption = #20445#23384'(&S)'
          TabOrder = 4
          OnClick = ButtonMonUseItemsSaveClick
        end
        object GroupBox4: TGroupBox
          Left = 256
          Top = 238
          Width = 206
          Height = 144
          Caption = #22522#26412#20449#24687
          TabOrder = 2
          object Label1: TLabel
            Left = 8
            Top = 20
            Width = 30
            Height = 12
            Caption = #32844#19994':'
          end
          object Label2: TLabel
            Left = 8
            Top = 42
            Width = 30
            Height = 12
            Caption = #24615#21035':'
          end
          object Label3: TLabel
            Left = 119
            Top = 42
            Width = 30
            Height = 12
            Caption = #21457#22411':'
          end
          object Label4: TLabel
            Left = 119
            Top = 66
            Width = 30
            Height = 12
            Caption = #33539#22260':'
          end
          object lbl25: TLabel
            Left = 119
            Top = 104
            Width = 30
            Height = 12
            Caption = #20960#29575':'
          end
          object seEditHair: TSpinEditEx
            Left = 151
            Top = 38
            Width = 46
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 2
            Value = 0
            OnChange = EditDRESSNAMEChange
          end
          object cbbJob: TComboBox
            Left = 40
            Top = 16
            Width = 50
            Height = 20
            Style = csDropDownList
            ItemIndex = 0
            TabOrder = 0
            Text = #25112#22763
            OnChange = cbbJobChange
            Items.Strings = (
              #25112#22763
              #27861#24072
              #36947#22763)
          end
          object cbbGender: TComboBox
            Left = 40
            Top = 38
            Width = 50
            Height = 20
            Style = csDropDownList
            ItemIndex = 0
            TabOrder = 1
            Text = #30007
            OnChange = EditDRESSNAMEChange
            Items.Strings = (
              #30007
              #22899)
          end
          object chkProtectMode: TCheckBox
            Left = 8
            Top = 63
            Width = 81
            Height = 17
            Hint = #36229#36807#35774#32622#30340#33539#22260#21518#65292#33258#21160#36820#22238#20986#29983#22320#22352#26631
            Caption = #23432#25252#27169#24335
            ParentShowHint = False
            ShowHint = True
            TabOrder = 4
            OnClick = EditDRESSNAMEChange
          end
          object EditRestrictMonsterRange: TSpinEditEx
            Left = 151
            Top = 61
            Width = 46
            Height = 21
            MaxValue = 0
            MinValue = 0
            TabOrder = 3
            Value = 10
            OnChange = EditDRESSNAMEChange
          end
          object CheckBoxNonUseSpellPoint: TCheckBox
            Left = 8
            Top = 82
            Width = 81
            Height = 17
            Hint = #21246#19978#21518#65292#26080#38656#21507#33647#65292#21487#20197#20351#29992#39764#27861#13#10#19981#21246#36873#65292#21017'MaxMP'#20026#25968#25454#24211#30340'MP'#23383#27573#20026#20934#65292#35774#32622#36807#23567#20250#23548#33268#26080#27861#20351#29992#39764#27861
            Caption = #26080#38480#39764#27861
            ParentShowHint = False
            ShowHint = True
            TabOrder = 5
            OnClick = EditDRESSNAMEChange
          end
          object chkRunWithAcctack: TCheckBox
            Left = 8
            Top = 101
            Width = 91
            Height = 17
            Hint = #21246#36873#21518#25112#22763#20154#24418#24618#36807#20110#28789#27963#65292#19981#24314#35758#25152#26377#20154#24418#24618#21246#36873'.'
            Caption = #22260#32469#30446#26631#25915#20987
            ParentShowHint = False
            ShowHint = True
            TabOrder = 6
            OnClick = EditDRESSNAMEChange
          end
          object seRunWithAcctackRate: TSpinEditEx
            Left = 151
            Top = 99
            Width = 46
            Height = 21
            Hint = #22260#32469#30446#26631#25915#20987#20960#29575#65292#25968#23383#36234#23567#65292#20960#29575#36234#22823
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 7
            Value = 10
            OnChange = EditDRESSNAMEChange
          end
          object chkNoAttackMode: TCheckBox
            Left = 8
            Top = 120
            Width = 97
            Height = 17
            Caption = #19981#25915#20987#27169#24335
            TabOrder = 8
            OnClick = EditDRESSNAMEChange
          end
        end
        object ButtonMonUseItemsChange: TButton
          Left = 33
          Top = 491
          Width = 85
          Height = 25
          Caption = #20462#25913'(&C)'
          TabOrder = 3
          OnClick = ButtonMonUseItemsChangeClick
        end
        object grp6: TGroupBox
          Left = 10
          Top = 423
          Width = 237
          Height = 62
          Caption = #27515#20129#25481#29289#21697
          TabOrder = 5
          object Label5: TLabel
            Left = 119
            Top = 20
            Width = 30
            Height = 12
            Caption = #26426#29575':'
          end
          object chkDieDropUseItem: TCheckBox
            Left = 8
            Top = 18
            Width = 81
            Height = 17
            Caption = #25481#36523#19978#35013#22791' '
            TabOrder = 0
            OnClick = EditDRESSNAMEChange
          end
          object seDieDropUseItemRate: TSpinEditEx
            Left = 151
            Top = 16
            Width = 45
            Height = 21
            Hint = #25968#23383#36234#22823#25481#29289#21697#30340#26426#29575#36234#20302
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 1
            Value = 0
            OnChange = EditDRESSNAMEChange
          end
          object chkDieDropBagItem: TCheckBox
            Left = 8
            Top = 40
            Width = 186
            Height = 17
            Caption = #25481#32972#21253#29289#21697'  [MonItems'#20013#37197#32622']'
            TabOrder = 2
            OnClick = chkDieDropBagItemClick
          end
        end
        object grp8: TGroupBox
          Left = 256
          Top = 388
          Width = 206
          Height = 129
          Caption = #25366#29289#21697#35774#32622
          TabOrder = 6
          object Label6: TLabel
            Left = 119
            Top = 21
            Width = 30
            Height = 12
            Caption = #26426#29575':'
          end
          object lbl21: TLabel
            Left = 10
            Top = 62
            Width = 48
            Height = 12
            Caption = #25910#36153#31867#22411
          end
          object Label165: TLabel
            Left = 120
            Top = 62
            Width = 30
            Height = 12
            Caption = #28857#25968':'
          end
          object chkButchUseItem: TCheckBox
            Left = 8
            Top = 18
            Width = 86
            Height = 17
            Caption = #25366#36523#19978#35013#22791
            ParentShowHint = False
            ShowHint = True
            TabOrder = 0
            OnClick = EditDRESSNAMEChange
          end
          object seButchUseItemRate: TSpinEditEx
            Left = 151
            Top = 16
            Width = 46
            Height = 21
            Hint = #25968#23383#36234#22823#25481#29289#21697#30340#26426#29575#36234#20302
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 1
            Value = 0
            OnChange = EditDRESSNAMEChange
          end
          object chkButchListItem: TCheckBox
            Left = 8
            Top = 38
            Width = 186
            Height = 17
            Caption = #25366#21015#34920#29289#21697'  ['#24618#29289#21517'-Item]'
            TabOrder = 2
            OnClick = EditDRESSNAMEChange
          end
          object cbbButchChargeMode: TComboBox
            Left = 61
            Top = 58
            Width = 49
            Height = 20
            Style = csDropDownList
            ItemIndex = 0
            TabOrder = 3
            Text = #37329#24065
            OnChange = EditDRESSNAMEChange
            Items.Strings = (
              #37329#24065
              #20803#23453
              #37329#21018#30707
              #28789#31526)
          end
          object seButchChargeCount: TSpinEditEx
            Left = 151
            Top = 57
            Width = 46
            Height = 21
            Hint = #25910#36153#28857#25968
            MaxValue = 0
            MinValue = 0
            ParentShowHint = False
            ShowHint = True
            TabOrder = 4
            Value = 0
            OnChange = EditDRESSNAMEChange
          end
          object chkButchItemTrigger: TCheckBox
            Left = 8
            Top = 104
            Width = 193
            Height = 17
            Caption = #26410#25366#21040#29289#21697#35302#21457' [@NoButchItem]'
            TabOrder = 5
            OnClick = EditDRESSNAMEChange
          end
          object chkOnlyButchItemDelGold: TCheckBox
            Left = 8
            Top = 82
            Width = 137
            Height = 17
            Hint = #20165#24471#21040#29289#21697#26102#25187#36153#65292#27809#26377#24471#21040#29289#21697#26102#19981#25187#36153
            Caption = #20165#24471#21040#29289#21697#26102#25187#36153
            TabOrder = 6
            OnClick = EditDRESSNAMEChange
          end
        end
      end
      object GroupBox6: TGroupBox
        Left = 7
        Top = 2
        Width = 129
        Height = 526
        Caption = #29289#21697#21015#34920
        TabOrder = 0
        object lstItemList: TListBox
          Left = 8
          Top = 16
          Width = 113
          Height = 500
          Hint = 'Ctrl + F '#29289#21697#26597#25214#65292#21491#38190#21487#20197#36827#34892#31579#36873#12290#13#25302#25341#36873#25321#30340#29289#21697#25918#20837#36523#19978#35013#22791#25110#21253#35065#29289#21697#21015#34920#20013#65288#25110#21452#20987#30452#25509#35013#22791#21040#36523#19978#65289
          DragCursor = crMultiDrag
          DragMode = dmAutomatic
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          PopupMenu = PopupMenuItem
          ShowHint = True
          TabOrder = 0
          OnDblClick = lstItemListDblClick
          OnKeyDown = lstItemListKeyDown
        end
      end
      object GroupBox7: TGroupBox
        Left = 143
        Top = 2
        Width = 129
        Height = 526
        Caption = #39764#27861#21015#34920
        TabOrder = 1
        object ListBoxMagicList: TListBox
          Left = 8
          Top = 16
          Width = 113
          Height = 500
          Hint = 'Ctrl + F '#39764#27861#26597#25214#13#25302#25341#25110#21452#20987#36873#25321#30340#39764#27861#65292#21487#20197#25226#39764#27861#25918#20837#20462#28860#39764#27861#21015#34920#20013
          DragCursor = crMultiDrag
          DragMode = dmAutomatic
          ItemHeight = 12
          MultiSelect = True
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnDblClick = ListBoxMagicListDblClick
          OnKeyDown = ListBoxMagicListKeyDown
        end
      end
      object ButtonMonsterConfigSave: TButton
        Left = 801
        Top = 648
        Width = 85
        Height = 25
        Caption = #20445#23384'(&S)'
        TabOrder = 5
        OnClick = ButtonMonsterConfigSaveClick
      end
      object GroupBox9: TGroupBox
        Left = 7
        Top = 533
        Width = 880
        Height = 112
        Caption = #22522#26412#35774#32622
        TabOrder = 4
        object RadioGroupMonsterNeedMagicItem: TRadioGroup
          Left = 249
          Top = 15
          Width = 145
          Height = 90
          Hint = 
            '1'#65306#36523#19978#25110#21253#35065#20013#37117#19981#38656#35201#31526#25110#27602#65292#23601#21487#20197#30452#25509#20351#29992#39764#27861#13'2'#65306#38656#35201#36523#19978#20329#25140#31526#25110#27602#65292#25165#21487#20197#20351#29992#39764#27861#13'3'#65306#39318#20808#20351#29992#36523#19978#20329#25140#31526#25110#27602#65292#22914#26524#36523#19978 +
            #27809#26377#20329#25140#65292#23601#20351#29992#21253#35065#20013#30340#31526#25110#27602#12290
          Caption = #36947#22763#25216#33021#35774#32622
          Items.Strings = (
            #19981#38656#35201#31526#25110#27602
            #20351#29992#20329#25140#30340#31526#25110#27602
            #20351#29992#21253#35065#20013#30340#31526#25110#27602)
          TabOrder = 2
          OnClick = RadioGroupMonsterNeedMagicItemClick
        end
        object GroupBox67: TGroupBox
          Left = 8
          Top = 15
          Width = 113
          Height = 91
          Caption = #25915#20987#38388#38548'('#27627#31186')'
          TabOrder = 0
          object Label158: TLabel
            Left = 8
            Top = 18
            Width = 30
            Height = 12
            Caption = #25112#22763':'
          end
          object Label162: TLabel
            Left = 8
            Top = 42
            Width = 30
            Height = 12
            Caption = #27861#24072':'
          end
          object Label163: TLabel
            Left = 8
            Top = 66
            Width = 30
            Height = 12
            Caption = #36947#22763':'
          end
          object EditMonsterWarrorAttackTime: TSpinEditEx
            Left = 42
            Top = 16
            Width = 55
            Height = 21
            MaxValue = 10000
            MinValue = 10
            TabOrder = 0
            Value = 10
            OnChange = EditMonsterWarrorAttackTimeChange
          end
          object EditMonsterTaoistAttackTime: TSpinEditEx
            Left = 42
            Top = 64
            Width = 55
            Height = 21
            MaxValue = 10000
            MinValue = 10
            TabOrder = 2
            Value = 10
            OnChange = EditMonsterTaoistAttackTimeChange
          end
          object EditMonsterWizardAttackTime: TSpinEditEx
            Left = 42
            Top = 40
            Width = 55
            Height = 21
            MaxValue = 10000
            MinValue = 10
            TabOrder = 1
            Value = 10
            OnChange = EditMonsterWizardAttackTimeChange
          end
        end
        object GroupBox66: TGroupBox
          Left = 128
          Top = 15
          Width = 113
          Height = 91
          Caption = #34892#36208#38388#38548'('#27627#31186')'
          TabOrder = 1
          object Label152: TLabel
            Left = 8
            Top = 18
            Width = 30
            Height = 12
            Caption = #25112#22763':'
          end
          object Label154: TLabel
            Left = 8
            Top = 42
            Width = 30
            Height = 12
            Caption = #27861#24072':'
          end
          object Label156: TLabel
            Left = 8
            Top = 66
            Width = 30
            Height = 12
            Caption = #36947#22763':'
          end
          object EditMonsterWarrorWalkTime: TSpinEditEx
            Left = 42
            Top = 16
            Width = 55
            Height = 21
            MaxValue = 10000
            MinValue = 10
            TabOrder = 0
            Value = 10
            OnChange = EditMonsterWarrorWalkTimeChange
          end
          object EditMonsterWizardWalkTime: TSpinEditEx
            Left = 42
            Top = 40
            Width = 55
            Height = 21
            MaxValue = 10000
            MinValue = 10
            TabOrder = 1
            Value = 10
            OnChange = EditMonsterWizardWalkTimeChange
          end
          object EditMonsterTaoistWalkTime: TSpinEditEx
            Left = 42
            Top = 64
            Width = 55
            Height = 21
            MaxValue = 10000
            MinValue = 10
            TabOrder = 2
            Value = 10
            OnChange = EditMonsterTaoistWalkTimeChange
          end
        end
        object grpSetDamage: TGroupBox
          Left = 401
          Top = 15
          Width = 145
          Height = 90
          Caption = #20260#23475#35774#32622
          TabOrder = 3
          object chkDamageLimitation: TCheckBox
            Left = 8
            Top = 16
            Width = 113
            Height = 17
            Caption = #20154#24418#24618#20260#23475#23553#39030
            TabOrder = 0
            OnClick = chkDamageLimitationClick
          end
        end
      end
    end
    object tsCustomMonster: TTabSheet
      Caption = #33258#23450#20041#24618#29289
      ImageIndex = 2
      object grpMonster: TGroupBox
        Left = 0
        Top = 0
        Width = 118
        Height = 641
        Align = alLeft
        Caption = #24618#29289#21015#34920
        TabOrder = 0
        object vstCustomMonster: TVirtualStringTree
          Left = 7
          Top = 19
          Width = 104
          Height = 616
          Hint = 'Ctrl + F '#29289#21697#26597#25214
          Colors.BorderColor = 15987699
          Colors.DisabledColor = clGray
          Colors.DropMarkColor = 15385233
          Colors.DropTargetColor = 15385233
          Colors.DropTargetBorderColor = 15987699
          Colors.FocusedSelectionColor = 15385233
          Colors.FocusedSelectionBorderColor = clWhite
          Colors.GridLineColor = 15987699
          Colors.HeaderHotColor = clBlack
          Colors.HotColor = clBlack
          Colors.SelectionRectangleBlendColor = 15385233
          Colors.SelectionRectangleBorderColor = 15385233
          Colors.SelectionTextColor = clBlack
          Colors.TreeLineColor = 9471874
          Colors.UnfocusedColor = 15716132
          Colors.UnfocusedSelectionColor = 15987699
          Colors.UnfocusedSelectionBorderColor = 15987699
          Header.AutoSizeIndex = 0
          Header.Font.Charset = DEFAULT_CHARSET
          Header.Font.Color = clWindowText
          Header.Font.Height = -11
          Header.Font.Name = 'Tahoma'
          Header.Font.Style = []
          Header.MainColumn = -1
          Indent = 0
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          TreeOptions.PaintOptions = [toShowDropmark, toThemeAware, toUseBlendedImages]
          TreeOptions.SelectionOptions = [toFullRowSelect]
          OnDrawText = vstCustomMonsterDrawText
          OnGetText = vstCustomMonsterGetText
          OnGetNodeDataSize = vstCustomMonsterGetNodeDataSize
          OnKeyDown = vstCustomMonsterKeyDown
          OnNodeClick = vstCustomMonsterNodeClick
          Columns = <>
        end
      end
      object pnlBottom: TPanel
        Left = 0
        Top = 641
        Width = 894
        Height = 34
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 1
        object lbl13: TLabel
          Left = 3
          Top = 12
          Width = 469
          Height = 12
          Caption = #33258#23450#20041#24618#29289#65306'Race=(154-157'#65289#19988'RaceImg=156'#65307#19981#21516#30340#33258#23450#20041#24618#29289'Appr'#24517#39035#19981#21516
          Font.Charset = GB2312_CHARSET
          Font.Color = clRed
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = [fsBold]
          ParentFont = False
        end
        object btnSave: TButton
          Left = 700
          Top = 6
          Width = 55
          Height = 25
          Caption = #20445#23384
          Enabled = False
          TabOrder = 0
          OnClick = btnSaveClick
        end
        object chkSendCustomMonsterConfig: TCheckBox
          Left = 576
          Top = 10
          Width = 121
          Height = 17
          Hint = #22914#26524#38598#25104#21040#30331#24405#22120#21017#21487#20197#19981#21457#36865
          Caption = #21457#36865#37197#32622#21040#23458#25143#31471
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = chkSendCustomMonsterConfigClick
        end
        object btn1: TButton
          Left = 762
          Top = 6
          Width = 129
          Height = 25
          Caption = #29983#25104#30331#24405#22120#38598#25104#25991#20214
          TabOrder = 2
          OnClick = btn1Click
        end
      end
      object pnl1: TPanel
        Left = 118
        Top = 0
        Width = 776
        Height = 641
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 2
        object pgcMain: TPageControl
          Left = 0
          Top = 0
          Width = 776
          Height = 617
          ActivePage = tsServerAttack
          Align = alClient
          TabOrder = 0
          object tsAction: TTabSheet
            Caption = #23458#25143#31471#21160#20316#21450#29305#25928
            object pnlActionBottom: TPanel
              Left = 0
              Top = 0
              Width = 768
              Height = 209
              Align = alTop
              BevelOuter = bvNone
              TabOrder = 0
              object grp1: TGroupBox
                Left = 4
                Top = 97
                Width = 184
                Height = 67
                Caption = #25209#37327#35774#32622#22270#29255#36164#28304#20301#32622
                TabOrder = 1
                object lbl3: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #21160#20316#20301#32622#65306
                end
                object Label13: TLabel
                  Left = 8
                  Top = 43
                  Width = 60
                  Height = 12
                  Caption = #29305#25928#20301#32622#65306
                end
                object cbbBatchAction: TComboBox
                  Left = 64
                  Top = 16
                  Width = 112
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbBatchActionChange
                end
                object cbbBatchEffect: TComboBox
                  Left = 64
                  Top = 39
                  Width = 112
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 1
                  OnChange = cbbBatchEffectChange
                end
              end
              object GroupBox11: TGroupBox
                Left = 4
                Top = 4
                Width = 184
                Height = 89
                Caption = #21160#20316#29305#25928#32472#21046#35774#32622
                TabOrder = 0
                object Label103: TLabel
                  Left = 8
                  Top = 20
                  Width = 90
                  Height = 12
                  Caption = #29305#25928'1'#32472#21046#27169#24335#65306
                end
                object Label104: TLabel
                  Left = 8
                  Top = 66
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#39034#24207#65306
                end
                object Label185: TLabel
                  Left = 8
                  Top = 43
                  Width = 90
                  Height = 12
                  Caption = #29305#25928'2'#32472#21046#27169#24335#65306
                end
                object cbbClientDrawMode: TComboBox
                  Left = 94
                  Top = 16
                  Width = 82
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientDrawModeChange
                end
                object cbbClientDrawOrder2: TComboBox
                  Left = 64
                  Top = 62
                  Width = 112
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 1
                  OnChange = cbbClientDrawOrder2Change
                end
                object cbbClientDrawMode2: TComboBox
                  Left = 94
                  Top = 39
                  Width = 82
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 2
                  OnChange = cbbClientDrawMode2Change
                end
              end
              object grp3: TGroupBox
                Left = 199
                Top = 4
                Width = 314
                Height = 160
                Caption = #24618#29289#22768#38899
                TabOrder = 2
                object lbl9: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #40664#35748#22768#38899#65306
                end
                object Label50: TLabel
                  Left = 8
                  Top = 43
                  Width = 60
                  Height = 12
                  Caption = #33487#37266#22768#38899#65306
                end
                object Label51: TLabel
                  Left = 8
                  Top = 67
                  Width = 60
                  Height = 12
                  Caption = #40664#35748#25915#20987#65306
                end
                object Label109: TLabel
                  Left = 8
                  Top = 91
                  Width = 60
                  Height = 12
                  Caption = #25384#25171#22768#38899#65306
                end
                object Label110: TLabel
                  Left = 8
                  Top = 114
                  Width = 60
                  Height = 12
                  Caption = #27515#20129#22768#38899#65306
                end
                object Label111: TLabel
                  Left = 165
                  Top = 20
                  Width = 42
                  Height = 12
                  Caption = #25915#20987'1'#65306
                end
                object Label112: TLabel
                  Left = 165
                  Top = 43
                  Width = 42
                  Height = 12
                  Caption = #25915#20987'2'#65306
                end
                object Label113: TLabel
                  Left = 165
                  Top = 67
                  Width = 42
                  Height = 12
                  Caption = #25915#20987'3'#65306
                end
                object Label114: TLabel
                  Left = 165
                  Top = 91
                  Width = 42
                  Height = 12
                  Caption = #25915#20987'4'#65306
                end
                object lbl10: TLabel
                  Left = 9
                  Top = 138
                  Width = 108
                  Height = 12
                  Caption = #22768#38899#25991#20214#21482#22635#25991#20214#21517
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clRed
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                end
                object Label128: TLabel
                  Left = 165
                  Top = 115
                  Width = 42
                  Height = 12
                  Caption = #25915#20987'5'#65306
                end
                object Label129: TLabel
                  Left = 165
                  Top = 139
                  Width = 42
                  Height = 12
                  Caption = #25915#20987'6'#65306
                end
                object edtSoundNormal: TEdit
                  Left = 64
                  Top = 16
                  Width = 100
                  Height = 20
                  TabOrder = 0
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack: TEdit
                  Tag = 2
                  Left = 64
                  Top = 63
                  Width = 100
                  Height = 20
                  TabOrder = 2
                  OnChange = edtSoundNormalChange
                end
                object edtSoundDigUP: TEdit
                  Tag = 1
                  Left = 64
                  Top = 39
                  Width = 100
                  Height = 20
                  TabOrder = 1
                  OnChange = edtSoundNormalChange
                end
                object edtSoundStruck: TEdit
                  Tag = 3
                  Left = 64
                  Top = 87
                  Width = 100
                  Height = 20
                  TabOrder = 3
                  OnChange = edtSoundNormalChange
                end
                object edtSoundDie: TEdit
                  Tag = 4
                  Left = 64
                  Top = 110
                  Width = 100
                  Height = 20
                  TabOrder = 4
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack1: TEdit
                  Tag = 5
                  Left = 202
                  Top = 16
                  Width = 100
                  Height = 20
                  TabOrder = 5
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack3: TEdit
                  Tag = 7
                  Left = 202
                  Top = 63
                  Width = 100
                  Height = 20
                  TabOrder = 7
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack2: TEdit
                  Tag = 6
                  Left = 202
                  Top = 39
                  Width = 100
                  Height = 20
                  TabOrder = 6
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack4: TEdit
                  Tag = 8
                  Left = 202
                  Top = 87
                  Width = 100
                  Height = 20
                  TabOrder = 8
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack5: TEdit
                  Tag = 9
                  Left = 202
                  Top = 111
                  Width = 100
                  Height = 20
                  TabOrder = 9
                  OnChange = edtSoundNormalChange
                end
                object edtSoundAttack6: TEdit
                  Tag = 10
                  Left = 202
                  Top = 135
                  Width = 100
                  Height = 20
                  TabOrder = 10
                  OnChange = edtSoundNormalChange
                end
              end
              object GroupBox17: TGroupBox
                Left = 523
                Top = 141
                Width = 243
                Height = 64
                Caption = #24555#36895#35745#31639#22270#29255#20301#32622
                TabOrder = 4
                object Label100: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #21160#20316#24320#22987#65306
                end
                object Label101: TLabel
                  Left = 8
                  Top = 43
                  Width = 60
                  Height = 12
                  Caption = #29305#25928#24320#22987#65306
                end
                object seClientStartIndex: TSpinEditEx
                  Left = 64
                  Top = 15
                  Width = 123
                  Height = 21
                  MaxValue = 2100000000
                  MinValue = -1
                  TabOrder = 0
                  Value = -1
                end
                object seClientEffectIndex: TSpinEditEx
                  Left = 64
                  Top = 38
                  Width = 123
                  Height = 21
                  MaxValue = 2100000000
                  MinValue = -1
                  TabOrder = 2
                  Value = -1
                end
                object btnCalcStartIndex: TButton
                  Left = 193
                  Top = 14
                  Width = 37
                  Height = 20
                  Caption = #35745#31639
                  TabOrder = 1
                  OnClick = btnCalcStartIndexClick
                end
                object btnCalcEffectIndex: TButton
                  Left = 193
                  Top = 39
                  Width = 37
                  Height = 20
                  Caption = #35745#31639
                  TabOrder = 3
                  OnClick = btnCalcEffectIndexClick
                end
              end
              object grp7: TGroupBox
                Left = 523
                Top = 4
                Width = 243
                Height = 133
                Caption = #24618#29289#34880#26465#35774#32622
                TabOrder = 3
                object lbl20: TLabel
                  Left = 8
                  Top = 43
                  Width = 60
                  Height = 12
                  Caption = #34880#26465#20559#31227#65306
                end
                object lbl14: TLabel
                  Left = 64
                  Top = 43
                  Width = 18
                  Height = 12
                  Caption = 'X'#65306
                end
                object Label125: TLabel
                  Left = 169
                  Top = 43
                  Width = 18
                  Height = 12
                  Caption = 'Y'#65306
                end
                object Label126: TLabel
                  Left = 22
                  Top = 65
                  Width = 60
                  Height = 12
                  Caption = #36164#28304#20301#32622#65306
                end
                object Label127: TLabel
                  Left = 22
                  Top = 89
                  Width = 60
                  Height = 12
                  Caption = #24320#22987#22270#29255#65306
                end
                object Label169: TLabel
                  Left = 8
                  Top = 113
                  Width = 60
                  Height = 12
                  Caption = #34880#20540#20559#31227#65306
                end
                object Label170: TLabel
                  Left = 64
                  Top = 113
                  Width = 18
                  Height = 12
                  Caption = 'X'#65306
                end
                object Label171: TLabel
                  Left = 169
                  Top = 113
                  Width = 18
                  Height = 12
                  Caption = 'Y'#65306
                end
                object Label182: TLabel
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 12
                  Caption = #32972#26223#20559#31227#65306
                end
                object Label183: TLabel
                  Left = 64
                  Top = 20
                  Width = 18
                  Height = 12
                  Caption = 'X'#65306
                end
                object Label184: TLabel
                  Left = 169
                  Top = 20
                  Width = 18
                  Height = 12
                  Caption = 'Y'#65306
                end
                object seHPOffsetX: TSpinEditEx
                  Left = 78
                  Top = 38
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = seHPOffsetXChange
                end
                object seHPOffsetY: TSpinEditEx
                  Left = 183
                  Top = 38
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seHPOffsetYChange
                end
                object cbbHPFile: TComboBox
                  Left = 77
                  Top = 61
                  Width = 154
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 2
                  OnChange = cbbHPFileChange
                end
                object seHPStartIndex: TSpinEditEx
                  Left = 77
                  Top = 84
                  Width = 154
                  Height = 21
                  Hint = #24320#22987#22270#29255#35774#32622#20026'-1'#26102#65292#20851#38381#33258#23450#20041#34880#26465#13#10#33258#23450#20041#34880#26465#20026#20004#24352#36830#32493#22270#29255#65292#21069#19968#24352#20026#34880#26465#36793#26694#65292#21518#19968#24352#20026#34880#26465
                  MaxValue = 2100000000
                  MinValue = -1
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 3
                  Value = -1
                  OnChange = seHPStartIndexChange
                end
                object seHPTextOffsetX: TSpinEditEx
                  Left = 78
                  Top = 108
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seHPTextOffsetXChange
                end
                object seHPTextOffsetY: TSpinEditEx
                  Left = 182
                  Top = 108
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 5
                  Value = 0
                  OnChange = seHPTextOffsetYChange
                end
                object seHPBgOffsetX: TSpinEditEx
                  Left = 78
                  Top = 15
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 6
                  Value = 0
                  OnChange = seHPBgOffsetXChange
                end
                object seHPBgOffsetY: TSpinEditEx
                  Left = 183
                  Top = 15
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 7
                  Value = 0
                  OnChange = seHPBgOffsetYChange
                end
              end
              object grp5: TGroupBox
                Left = 3
                Top = 168
                Width = 185
                Height = 31
                TabOrder = 5
                object chkDieNoCalcDir: TCheckBox
                  Left = 27
                  Top = 8
                  Width = 137
                  Height = 17
                  Hint = #21246#36873#21518#65292#27515#20129#29305#25928#23558#19981#35745#31639#26041#21521#13#10#19981#21246#36873#65292#27515#20129#29305#25928#21644#27515#20129#21160#20316#20445#25345#19968#33268
                  Alignment = taLeftJustify
                  Caption = #27515#20129#29305#25928#19981#31639#26041#21521
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clRed
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 0
                  OnClick = chkDieNoCalcDirClick
                end
              end
            end
            object vstAction: TVirtualStringTree
              Left = 0
              Top = 209
              Width = 768
              Height = 380
              Align = alClient
              Colors.BorderColor = 15987699
              Colors.DisabledColor = clGray
              Colors.DropMarkColor = 15385233
              Colors.DropTargetColor = 15385233
              Colors.DropTargetBorderColor = 15987699
              Colors.FocusedSelectionColor = 14803425
              Colors.FocusedSelectionBorderColor = 14803425
              Colors.GridLineColor = 12303291
              Colors.HeaderHotColor = clBlack
              Colors.HotColor = clBlack
              Colors.SelectionRectangleBlendColor = 15385233
              Colors.SelectionRectangleBorderColor = 15385233
              Colors.SelectionTextColor = clBlack
              Colors.TreeLineColor = 9471874
              Colors.UnfocusedColor = 15715744
              Colors.UnfocusedSelectionColor = 14803425
              Colors.UnfocusedSelectionBorderColor = 14803425
              DefaultNodeHeight = 20
              Header.AutoSizeIndex = 0
              Header.Font.Charset = DEFAULT_CHARSET
              Header.Font.Color = clWindowText
              Header.Font.Height = -11
              Header.Font.Name = 'Tahoma'
              Header.Font.Style = []
              Header.Height = 40
              Header.MainColumn = 10
              Header.Options = [hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
              HintMode = hmHint
              LineStyle = lsSolid
              Margin = 2
              ParentShowHint = False
              ShowHint = True
              TabOrder = 1
              TextMargin = 2
              TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
              TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
              TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
              OnChecked = vstActionChecked
              OnCreateEditor = vstActionCreateEditor
              OnDrawText = vstActionDrawText
              OnEditing = vstActionEditing
              OnGetText = vstActionGetText
              OnGetHint = vstActionGetHint
              OnGetNodeDataSize = vstActionGetNodeDataSize
              OnNodeClick = vstActionNodeClick
              Columns = <
                item
                  Position = 0
                  Width = 66
                  WideText = #21160#20316
                  WideHint = ' '
                end
                item
                  Position = 1
                  Width = 100
                  WideText = #21160#20316#36164#28304#20301#32622
                end
                item
                  Alignment = taRightJustify
                  CaptionAlignment = taCenter
                  Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                  Position = 2
                  Width = 55
                  WideText = #21160#20316#13#10#24320#22987
                end
                item
                  Alignment = taRightJustify
                  CaptionAlignment = taCenter
                  Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                  Position = 3
                  Width = 55
                  WideText = #25773#25918#13#10#25968#37327
                end
                item
                  Alignment = taRightJustify
                  CaptionAlignment = taCenter
                  Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                  Position = 4
                  Width = 55
                  WideText = #31354#30333#13#10#25968#37327
                end
                item
                  Alignment = taRightJustify
                  CaptionAlignment = taCenter
                  Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                  Position = 5
                  Width = 55
                  WideText = #25773#25918#13#10#36895#24230
                end
                item
                  Position = 6
                  Width = 100
                  WideText = #29305#25928#36164#28304#20301#32622
                end
                item
                  Alignment = taRightJustify
                  CaptionAlignment = taCenter
                  Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                  Position = 7
                  Width = 55
                  WideText = #29305#25928#13#10#24320#22987
                end
                item
                  Position = 8
                  Width = 100
                  WideText = #29305#25928'2'#36164#28304#20301#32622
                end
                item
                  Alignment = taRightJustify
                  CaptionAlignment = taCenter
                  Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coResizable, coShowDropMark, coVisible, coAllowFocus, coWrapCaption, coUseCaptionAlignment]
                  Position = 9
                  Width = 55
                  WideText = #29305#25928'2'#13#10#24320'  '#22987
                end
                item
                  Margin = 1
                  Position = 10
                  Width = 53
                  WideText = #31639#26041#21521
                end>
              WideDefaultText = ''
            end
          end
          object tsAttack: TTabSheet
            Caption = #23458#25143#31471#25915#20987'/'#22686#30410
            ImageIndex = 1
            object grpClientAttackConfigs: TGroupBox
              Left = 3
              Top = 52
              Width = 760
              Height = 536
              Caption = #25915#20987'1'#30340#25915#20987#25928#26524#37197#32622
              TabOrder = 1
              object lbl24: TLabel
                Left = 205
                Top = 515
                Width = 336
                Height = 12
                Caption = #37197#32622#20102#39134#34892#39764#27861#65292#30446#26631#25928#26524#21017#29992#29190#28856#25928#26524#65292#26410#37197#32622#21017#29992#30446#26631#25928#26524
                Font.Charset = GB2312_CHARSET
                Font.Color = clRed
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object grpFly: TGroupBox
                Left = 197
                Top = 18
                Width = 180
                Height = 232
                Caption = #39134#34892#39764#27861#25928#26524
                TabOrder = 0
                object lbl5: TLabel
                  Left = 13
                  Top = 21
                  Width = 60
                  Height = 12
                  Caption = #36164#28304#25991#20214#65306
                end
                object Label15: TLabel
                  Left = 13
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #24320#22987#22270#29255#65306
                end
                object Label16: TLabel
                  Left = 13
                  Top = 68
                  Width = 60
                  Height = 12
                  Caption = #25773#25918#25968#37327#65306
                end
                object Label17: TLabel
                  Left = 13
                  Top = 92
                  Width = 60
                  Height = 12
                  Caption = #31354#30333#25968#37327#65306
                end
                object Label18: TLabel
                  Left = 13
                  Top = 140
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#27169#24335#65306
                end
                object Label19: TLabel
                  Left = 13
                  Top = 163
                  Width = 60
                  Height = 12
                  Caption = #26041#21521#25968#37327#65306
                end
                object Label27: TLabel
                  Left = 13
                  Top = 116
                  Width = 60
                  Height = 12
                  Caption = #25773#25918#36895#24230#65306
                end
                object Label173: TLabel
                  Left = 13
                  Top = 208
                  Width = 60
                  Height = 12
                  Caption = #29031#20142#33539#22260#65306
                end
                object cbbClientFlyFile: TComboBox
                  Left = 70
                  Top = 17
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientFlyFileChange
                end
                object cbbClientFlyDrawMode: TComboBox
                  Left = 70
                  Top = 136
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 5
                  OnChange = cbbClientFlyDrawModeChange
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
                object cbbClientFlyDirCount: TComboBox
                  Left = 70
                  Top = 159
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 6
                  OnChange = cbbClientFlyDirCountChange
                end
                object chkClientFlyCalcDir: TCheckBox
                  Left = 13
                  Top = 183
                  Width = 116
                  Height = 17
                  Caption = #39134#34892#25928#26524#35745#31639#26041#21521
                  TabOrder = 7
                  OnClick = chkClientFlyCalcDirClick
                end
                object seClientFlyPlayTime: TSpinEditEx
                  Left = 70
                  Top = 112
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seClientFlyPlayTimeChange
                end
                object seClientFlyEmptyCount: TSpinEditEx
                  Left = 70
                  Top = 88
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seClientFlyEmptyCountChange
                end
                object seClientFlyPlayCount: TSpinEditEx
                  Left = 70
                  Top = 64
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seClientFlyPlayCountChange
                end
                object seClientFlyStartIndex: TSpinEditEx
                  Left = 70
                  Top = 40
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seClientFlyStartIndexChange
                end
                object seFlyLightRange: TSpinEditEx
                  Left = 70
                  Top = 204
                  Width = 96
                  Height = 21
                  Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#24618#29289#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#65292#26080#40657#22812#21151#33021#26080#38656#20462#25913
                  MaxValue = 5
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 8
                  Value = 0
                  OnChange = seFlyLightRangeChange
                end
              end
              object grpSelf: TGroupBox
                Left = 11
                Top = 18
                Width = 180
                Height = 257
                Caption = #33258#36523#25773#25918#39764#27861#25928#26524
                TabOrder = 2
                object Label20: TLabel
                  Left = 13
                  Top = 21
                  Width = 60
                  Height = 12
                  Caption = #36164#28304#25991#20214#65306
                end
                object Label21: TLabel
                  Left = 13
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #24320#22987#22270#29255#65306
                end
                object Label22: TLabel
                  Left = 13
                  Top = 68
                  Width = 60
                  Height = 12
                  Caption = #22270#29255#25968#37327#65306
                end
                object Label26: TLabel
                  Left = 13
                  Top = 140
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#39034#24207#65306
                end
                object Label24: TLabel
                  Left = 13
                  Top = 116
                  Width = 60
                  Height = 12
                  Caption = #25773#25918#36895#24230#65306
                end
                object Label107: TLabel
                  Left = 13
                  Top = 163
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#27169#24335#65306
                end
                object Label25: TLabel
                  Left = 13
                  Top = 92
                  Width = 60
                  Height = 12
                  Caption = #31354#30333#25968#37327#65306
                end
                object Label167: TLabel
                  Left = 13
                  Top = 236
                  Width = 60
                  Height = 12
                  Caption = #29031#20142#33539#22260#65306
                end
                object Label224: TLabel
                  Left = 13
                  Top = 186
                  Width = 60
                  Height = 12
                  Caption = #26041#21521#25968#37327#65306
                end
                object Label225: TLabel
                  Left = 13
                  Top = 211
                  Width = 60
                  Height = 12
                  Caption = #26041#21521#35745#31639#65306
                end
                object cbbClientSelfFile: TComboBox
                  Left = 70
                  Top = 17
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientSelfFileChange
                end
                object cbbClientSelfDrawOrder: TComboBox
                  Left = 70
                  Top = 136
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 5
                  OnChange = cbbClientSelfDrawOrderChange
                  Items.Strings = (
                    #20808#32472#33258#36523#20877#32472#39764#27861#25928#26524
                    #20808#32472#39764#27861#25928#26524#20877#32472#33258#36523)
                end
                object seClientSelfPlayTime: TSpinEditEx
                  Left = 70
                  Top = 112
                  Width = 96
                  Height = 21
                  Hint = #24403#20154#29289#33258#36523#25928#26524#25773#25918#19981#23436#25972#26102#65292#21487#20197#36866#24403#35843#24555#25773#25918#36895#24230
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 4
                  Value = 0
                  OnChange = seClientSelfPlayTimeChange
                end
                object seClientSelfPlayCount: TSpinEditEx
                  Left = 70
                  Top = 64
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seClientSelfPlayCountChange
                end
                object seClientSelfStartIndex: TSpinEditEx
                  Left = 70
                  Top = 40
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seClientSelfStartIndexChange
                end
                object cbbClientSelfDrawMode: TComboBox
                  Left = 70
                  Top = 159
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 6
                  OnChange = cbbClientSelfDrawModeChange
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
                object seClientSelfEmptyCount: TSpinEditEx
                  Left = 70
                  Top = 88
                  Width = 96
                  Height = 21
                  Hint = #29992#20110'8'#26041#21521#25915#20987#26102#35745#31639#26041#21521
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 3
                  Value = 0
                  OnChange = seClientSelfEmptyCountChange
                end
                object chkClientSelfPlayDelayAction: TCheckBox
                  Left = 12
                  Top = 254
                  Width = 153
                  Height = 17
                  Hint = #24403#24618#29289#21160#20316#26377#24310#32531#26102#65292#21435#25481#21246#36873#13#10#24403#24618#29289#33258#36523#25928#26524#25773#25918#19981#23436#25972#25110#26377#26102#19981#20986#30446#26631#25773#25918#25928#26524#65292#21246#36873#27492#39033
                  Caption = #25773#25928#26524#26102#24310#32531#25915#20987#21160#20316
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clWindowText
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = [fsBold]
                  ParentFont = False
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 8
                  Visible = False
                  OnClick = chkClientSelfPlayDelayActionClick
                end
                object seSelfLightRange: TSpinEditEx
                  Left = 70
                  Top = 231
                  Width = 96
                  Height = 21
                  Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#24618#29289#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#65292#26080#40657#22812#21151#33021#26080#38656#20462#25913
                  MaxValue = 5
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 7
                  Value = 0
                  OnChange = seSelfLightRangeChange
                end
                object cbbClientSelfDirCount: TComboBox
                  Left = 70
                  Top = 182
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 9
                  OnChange = cbbClientSelfDirCountChange
                end
                object cbbClientSelfDirCalcType: TComboBox
                  Left = 70
                  Top = 206
                  Width = 96
                  Height = 20
                  Hint = #27880#24847#65306#26222#36890#35745#31639#26041#24335#65292'16'#26041#21521#20063#25353'8'#26041#21521#35745#31639
                  Style = csDropDownList
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 10
                  OnChange = cbbClientSelfDirCalcTypeChange
                  Items.Strings = (
                    #19981#20998#26041#21521
                    #26222#36890#35745#31639
                    #33258#25105#20013#24515#22810#26041#21521)
                end
              end
              object grpExplosion: TGroupBox
                Left = 383
                Top = 18
                Width = 180
                Height = 375
                Caption = #29190#28856#25773#25918#25928#26524
                TabOrder = 1
                object Label28: TLabel
                  Left = 18
                  Top = 21
                  Width = 60
                  Height = 12
                  Caption = #36164#28304#25991#20214#65306
                end
                object Label29: TLabel
                  Left = 18
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #24320#22987#22270#29255#65306
                end
                object Label30: TLabel
                  Left = 18
                  Top = 92
                  Width = 60
                  Height = 12
                  Caption = #22270#29255#25968#37327#65306
                end
                object Label46: TLabel
                  Left = 18
                  Top = 117
                  Width = 60
                  Height = 12
                  Caption = #25773#25918#36895#24230#65306
                end
                object Label49: TLabel
                  Left = 18
                  Top = 140
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#27169#24335#65306
                end
                object Label172: TLabel
                  Left = 18
                  Top = 209
                  Width = 60
                  Height = 12
                  Caption = #29031#20142#33539#22260#65306
                end
                object Label207: TLabel
                  Left = 18
                  Top = 260
                  Width = 60
                  Height = 12
                  Caption = #25345#32493#26102#38388#65306
                end
                object Label208: TLabel
                  Left = 18
                  Top = 308
                  Width = 60
                  Height = 12
                  Caption = #20260#23475#33539#22260#65306
                end
                object Label209: TLabel
                  Left = 18
                  Top = 284
                  Width = 60
                  Height = 12
                  Caption = #20260#23475#38388#38548#65306
                end
                object Bevel1: TBevel
                  Left = 4
                  Top = 231
                  Width = 166
                  Height = 3
                  Shape = bsBottomLine
                end
                object Label214: TLabel
                  Left = 18
                  Top = 332
                  Width = 60
                  Height = 12
                  Caption = #29031#20142#33539#22260#65306
                end
                object Label215: TLabel
                  Left = 157
                  Top = 260
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label216: TLabel
                  Left = 157
                  Top = 284
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label217: TLabel
                  Left = 157
                  Top = 308
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object Label218: TLabel
                  Left = 12
                  Top = 69
                  Width = 66
                  Height = 12
                  Caption = #24320#22987#22270#29255'2'#65306
                end
                object Label221: TLabel
                  Left = 12
                  Top = 163
                  Width = 66
                  Height = 12
                  Caption = #32472#21046#27169#24335'2'#65306
                end
                object cbbClientExplosionFile: TComboBox
                  Left = 75
                  Top = 17
                  Width = 94
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientExplosionFileChange
                end
                object seClientExplosionPlayTime: TSpinEditEx
                  Left = 75
                  Top = 112
                  Width = 94
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seClientExplosionPlayTimeChange
                end
                object seClientExplosionPlayCount: TSpinEditEx
                  Left = 75
                  Top = 88
                  Width = 94
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seClientExplosionPlayCountChange
                end
                object seClientExplosionStartIndex: TSpinEditEx
                  Left = 75
                  Top = 40
                  Width = 94
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seClientExplosionStartIndexChange
                end
                object cbbClientExplosionDrawMode: TComboBox
                  Left = 75
                  Top = 136
                  Width = 94
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 5
                  OnChange = cbbClientExplosionDrawModeChange
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
                object chkClientExplosionLockDraw: TCheckBox
                  Left = 17
                  Top = 184
                  Width = 115
                  Height = 17
                  Caption = #25928#26524#22987#32456#36319#38543#30446#26631
                  TabOrder = 7
                  OnClick = chkClientExplosionLockDrawClick
                end
                object seExplosionLightRange: TSpinEditEx
                  Left = 75
                  Top = 205
                  Width = 94
                  Height = 21
                  Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#24618#29289#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#65292#26080#40657#22812#21151#33021#26080#38656#20462#25913
                  MaxValue = 5
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 8
                  Value = 0
                  OnChange = seExplosionLightRangeChange
                end
                object chkClientExplosionKeepPlay: TCheckBox
                  Left = 17
                  Top = 236
                  Width = 97
                  Height = 17
                  Caption = #25928#26524#25345#32493#25773#25918
                  TabOrder = 9
                  OnClick = chkClientExplosionKeepPlayClick
                end
                object seClientExplosionKeepTime: TSpinEditEx
                  Left = 75
                  Top = 255
                  Width = 78
                  Height = 21
                  MaxValue = 65535
                  MinValue = 5
                  TabOrder = 10
                  Value = 5
                  OnChange = seClientExplosionKeepTimeChange
                end
                object seClientExplosionKeepAttackRange: TSpinEditEx
                  Left = 75
                  Top = 303
                  Width = 78
                  Height = 21
                  Hint = #21322#24452#33539#22260#65307'0'#65306#34920#31034#30446#26631#22352#26631#65292'1'#65306#30446#26631#21450#21608#36793#19968#26684#20849'9'#26684
                  MaxValue = 8
                  MinValue = 0
                  TabOrder = 11
                  Value = 0
                  OnChange = seClientExplosionKeepAttackRangeChange
                end
                object seClientExplosionKeepAttackInterval: TSpinEditEx
                  Left = 75
                  Top = 279
                  Width = 78
                  Height = 21
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 12
                  Value = 1
                  OnChange = seClientExplosionKeepAttackIntervalChange
                end
                object chkClientExplosionKeepMultiPlay: TCheckBox
                  Left = 19
                  Top = 353
                  Width = 96
                  Height = 17
                  Hint = #22914#26524#20260#23475#33539#22260#20026'1('#21363'9'#26684#33539#22260')'#65292#33509#21246#36873#27492#36873#39033#65292#21017'9'#26684#27599#26684#37117#25773#25918#25928#26524#65292#21542#21017'9'#26684#33539#22260#21482#20849'1'#20010#25928#26524
                  Caption = #27599#26684#21333#29420#25773#25918
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clWindowText
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = [fsBold]
                  ParentFont = False
                  TabOrder = 14
                  OnClick = chkClientExplosionKeepMultiPlayClick
                end
                object seExplosionKeepLightRange: TSpinEditEx
                  Left = 75
                  Top = 328
                  Width = 94
                  Height = 21
                  Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
                  MaxValue = 5
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 13
                  Value = 0
                  OnChange = seExplosionKeepLightRangeChange
                end
                object seClientExplosionStartIndex2: TSpinEditEx
                  Left = 74
                  Top = 64
                  Width = 94
                  Height = 21
                  Hint = '>=0'#21017#24320#21551#29305#25928'2'#65292#29305#25928'1'#21644#29305#25928'2'#21516#27493#25773#25918
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 2
                  Value = 0
                  OnChange = seClientExplosionStartIndex2Change
                end
                object cbbClientExplosionDrawMode2: TComboBox
                  Left = 75
                  Top = 159
                  Width = 94
                  Height = 20
                  Hint = #29305#25928'2'#32472#21046#27169#24335'a'
                  Style = csDropDownList
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 6
                  OnChange = cbbClientExplosionDrawMode2Change
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
              end
              object grpTarget: TGroupBox
                Left = 569
                Top = 18
                Width = 180
                Height = 399
                Caption = #30446#26631#25773#25918#25928#26524
                TabOrder = 3
                object Label44: TLabel
                  Left = 17
                  Top = 21
                  Width = 60
                  Height = 12
                  Caption = #36164#28304#25991#20214#65306
                end
                object Label45: TLabel
                  Left = 17
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #24320#22987#22270#29255#65306
                end
                object Label47: TLabel
                  Left = 17
                  Top = 93
                  Width = 60
                  Height = 12
                  Caption = #22270#29255#25968#37327#65306
                end
                object Label48: TLabel
                  Left = 17
                  Top = 116
                  Width = 60
                  Height = 12
                  Caption = #25773#25918#36895#24230#65306
                end
                object Label108: TLabel
                  Left = 17
                  Top = 140
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#27169#24335#65306
                end
                object Label168: TLabel
                  Left = 17
                  Top = 233
                  Width = 60
                  Height = 12
                  Caption = #29031#20142#33539#22260#65306
                end
                object Label174: TLabel
                  Left = 16
                  Top = 281
                  Width = 60
                  Height = 12
                  Caption = #25345#32493#26102#38388#65306
                end
                object Label175: TLabel
                  Left = 16
                  Top = 329
                  Width = 60
                  Height = 12
                  Caption = #20260#23475#33539#22260#65306
                end
                object Label176: TLabel
                  Left = 16
                  Top = 305
                  Width = 60
                  Height = 12
                  Caption = #20260#23475#38388#38548#65306
                end
                object lbl23: TLabel
                  Left = 154
                  Top = 281
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label177: TLabel
                  Left = 154
                  Top = 328
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object Label178: TLabel
                  Left = 154
                  Top = 305
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object bvl1: TBevel
                  Left = 2
                  Top = 253
                  Width = 170
                  Height = 3
                  Shape = bsBottomLine
                end
                object Label236: TLabel
                  Left = 16
                  Top = 353
                  Width = 60
                  Height = 12
                  Caption = #29031#20142#33539#22260#65306
                end
                object Label219: TLabel
                  Left = 11
                  Top = 69
                  Width = 66
                  Height = 12
                  Caption = #24320#22987#22270#29255'2'#65306
                end
                object Label220: TLabel
                  Left = 11
                  Top = 163
                  Width = 66
                  Height = 12
                  Caption = #32472#21046#27169#24335'2'#65306
                end
                object cbbClientTargetFile: TComboBox
                  Left = 74
                  Top = 17
                  Width = 94
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientTargetFileChange
                end
                object seClientTargetPlayTime: TSpinEditEx
                  Left = 74
                  Top = 112
                  Width = 94
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seClientTargetPlayTimeChange
                end
                object seClientTargetPlayCount: TSpinEditEx
                  Left = 74
                  Top = 88
                  Width = 94
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seClientTargetPlayCountChange
                end
                object seClientTargetStartIndex: TSpinEditEx
                  Left = 74
                  Top = 42
                  Width = 94
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seClientTargetStartIndexChange
                end
                object cbbClientTargetDrawMode: TComboBox
                  Left = 74
                  Top = 136
                  Width = 94
                  Height = 20
                  Hint = #29305#25928'2'#32472#21046#27169#24335
                  Style = csDropDownList
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 5
                  OnChange = cbbClientTargetDrawModeChange
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
                object chkClientTargetMultiPlay: TCheckBox
                  Left = 16
                  Top = 184
                  Width = 80
                  Height = 17
                  Caption = #22810#30446#26631#25773#25918
                  TabOrder = 7
                  OnClick = chkClientTargetMultiPlayClick
                end
                object chkClientTargetLockDraw: TCheckBox
                  Left = 16
                  Top = 207
                  Width = 116
                  Height = 17
                  Caption = #25928#26524#22987#32456#36319#38543#30446#26631
                  TabOrder = 8
                  OnClick = chkClientTargetLockDrawClick
                end
                object seTargetLightRange: TSpinEditEx
                  Left = 74
                  Top = 228
                  Width = 94
                  Height = 21
                  Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
                  MaxValue = 5
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 9
                  Value = 0
                  OnChange = seTargetLightRangeChange
                end
                object chkTargetKeepPlay: TCheckBox
                  Left = 15
                  Top = 257
                  Width = 97
                  Height = 17
                  Caption = #25928#26524#25345#32493#25773#25918
                  TabOrder = 10
                  OnClick = chkTargetKeepPlayClick
                end
                object seTargetKeepTime: TSpinEditEx
                  Left = 70
                  Top = 278
                  Width = 78
                  Height = 21
                  MaxValue = 65535
                  MinValue = 1
                  TabOrder = 11
                  Value = 5
                  OnChange = seTargetKeepTimeChange
                end
                object seTargetKeepAttackRange: TSpinEditEx
                  Left = 73
                  Top = 324
                  Width = 78
                  Height = 21
                  Hint = #21322#24452#33539#22260#65307'0'#65306#34920#31034#30446#26631#22352#26631#65292'1'#65306#30446#26631#21450#21608#36793#19968#26684#20849'9'#26684
                  MaxValue = 8
                  MinValue = 0
                  TabOrder = 13
                  Value = 0
                  OnChange = seTargetKeepAttackRangeChange
                end
                object seTargetKeepAttackInterval: TSpinEditEx
                  Left = 70
                  Top = 301
                  Width = 78
                  Height = 21
                  MaxValue = 255
                  MinValue = 1
                  TabOrder = 12
                  Value = 1
                  OnChange = seTargetKeepAttackIntervalChange
                end
                object chkTargetKeepMultiPlay: TCheckBox
                  Left = 17
                  Top = 373
                  Width = 96
                  Height = 17
                  Hint = #22914#26524#20260#23475#33539#22260#20026'1('#21363'9'#26684#33539#22260')'#65292#33509#21246#36873#27492#36873#39033#65292#21017'9'#26684#27599#26684#37117#25773#25918#25928#26524#65292#21542#21017'9'#26684#33539#22260#21482#20849'1'#20010#25928#26524
                  Caption = #27599#26684#21333#29420#25773#25918
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clWindowText
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = [fsBold]
                  ParentFont = False
                  TabOrder = 15
                  OnClick = chkTargetKeepMultiPlayClick
                end
                object seTargetKeepLightRange: TSpinEditEx
                  Left = 73
                  Top = 348
                  Width = 94
                  Height = 21
                  Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#33258#36523#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524#12290#26080#40657#22812#21151#33021#26080#38656#20462#25913
                  MaxValue = 5
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 14
                  Value = 0
                  OnChange = seTargetKeepLightRangeChange
                end
                object seClientTargetStartIndex2: TSpinEditEx
                  Left = 73
                  Top = 64
                  Width = 94
                  Height = 21
                  Hint = '>=0'#21017#24320#21551#29305#25928'2'#65292#29305#25928'1'#21644#29305#25928'2'#21516#27493#25773#25918
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 2
                  Value = 0
                  OnChange = seClientTargetStartIndex2Change
                end
                object cbbClientTargetDrawMode2: TComboBox
                  Left = 74
                  Top = 159
                  Width = 94
                  Height = 20
                  Hint = #29305#25928'2'#32472#21046#27169#24335'a'
                  Style = csDropDownList
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 6
                  OnChange = cbbClientTargetDrawMode2Change
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
              end
              object grp2: TGroupBox
                Left = 197
                Top = 253
                Width = 180
                Height = 94
                Hint = #31867#20284#20110'magic6.wzl'#20013#30340'140'#39134#34892#31526#23545#24212#30340'310'#39134#34892#31526#29305#25928
                Caption = #39134#34892#39764#27861#29305#25928
                ParentShowHint = False
                ShowHint = True
                TabOrder = 4
                object Label14: TLabel
                  Left = 13
                  Top = 21
                  Width = 60
                  Height = 12
                  Caption = #36164#28304#25991#20214#65306
                end
                object Label105: TLabel
                  Left = 13
                  Top = 46
                  Width = 60
                  Height = 12
                  Caption = #24320#22987#22270#29255#65306
                end
                object Label106: TLabel
                  Left = 13
                  Top = 70
                  Width = 60
                  Height = 12
                  Caption = #32472#21046#27169#24335#65306
                end
                object cbbClientFlyEffFile: TComboBox
                  Left = 70
                  Top = 17
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientFlyEffFileChange
                end
                object seClientFlyEffStartIndex: TSpinEditEx
                  Left = 70
                  Top = 41
                  Width = 96
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seClientFlyEffStartIndexChange
                end
                object cbbClientFlyEffDrawMode: TComboBox
                  Left = 70
                  Top = 66
                  Width = 96
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 2
                  OnChange = cbbClientFlyEffDrawModeChange
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
              end
              object grp10: TGroupBox
                Left = 11
                Top = 278
                Width = 180
                Height = 253
                Caption = #33258#36523#25345#32493#25773#25918#39764#27861#29305#25928
                Font.Charset = DEFAULT_CHARSET
                Font.Color = clBlack
                Font.Height = -11
                Font.Name = 'Tahoma'
                Font.Style = []
                ParentFont = False
                TabOrder = 5
                object lbl42: TLabel
                  Left = 13
                  Top = 21
                  Width = 60
                  Height = 13
                  Caption = #36164#28304#25991#20214#65306
                end
                object lbl43: TLabel
                  Left = 13
                  Top = 47
                  Width = 60
                  Height = 13
                  Caption = #24320#22987#22270#29255#65306
                end
                object lbl44: TLabel
                  Left = 13
                  Top = 99
                  Width = 60
                  Height = 13
                  Caption = #22270#29255#25968#37327#65306
                end
                object lbl45: TLabel
                  Left = 13
                  Top = 125
                  Width = 60
                  Height = 13
                  Caption = #25773#25918#36895#24230#65306
                end
                object lbl46: TLabel
                  Left = 13
                  Top = 175
                  Width = 60
                  Height = 13
                  Caption = #32472#21046#27169#24335#65306
                end
                object lbl47: TLabel
                  Left = 13
                  Top = 227
                  Width = 60
                  Height = 13
                  Caption = #25345#32493#26102#38388#65306
                end
                object lbl48: TLabel
                  Left = 154
                  Top = 228
                  Width = 12
                  Height = 13
                  Caption = #31186
                end
                object Label186: TLabel
                  Left = 7
                  Top = 74
                  Width = 66
                  Height = 13
                  Caption = #24320#22987#22270#29255'2'#65306
                end
                object Label187: TLabel
                  Left = 7
                  Top = 201
                  Width = 66
                  Height = 13
                  Caption = #32472#21046#27169#24335'2'#65306
                end
                object Label188: TLabel
                  Left = 13
                  Top = 151
                  Width = 60
                  Height = 13
                  Caption = #32472#21046#39034#24207#65306
                end
                object cbbClientSelfKeepFile: TComboBox
                  Left = 70
                  Top = 17
                  Width = 96
                  Height = 21
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbClientSelfKeepFileChange
                end
                object seClientSelfKeepPlayTime: TSpinEditEx
                  Left = 70
                  Top = 121
                  Width = 96
                  Height = 22
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seClientSelfKeepPlayTimeChange
                end
                object seClientSelfKeepPlayCount: TSpinEditEx
                  Left = 70
                  Top = 95
                  Width = 96
                  Height = 22
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seClientSelfKeepPlayCountChange
                end
                object seClientSelfKeepStartIndex: TSpinEditEx
                  Left = 70
                  Top = 42
                  Width = 96
                  Height = 22
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seClientSelfKeepStartIndexChange
                end
                object cbbClientSelfKeepDrawMode: TComboBox
                  Left = 70
                  Top = 172
                  Width = 96
                  Height = 21
                  Style = csDropDownList
                  TabOrder = 4
                  OnChange = cbbClientSelfKeepDrawModeChange
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
                object seClientSelfKeepTime: TSpinEditEx
                  Left = 70
                  Top = 223
                  Width = 81
                  Height = 22
                  MaxValue = 65535
                  MinValue = 0
                  TabOrder = 5
                  Value = 5
                  OnChange = seClientSelfKeepTimeChange
                end
                object seClientSelfKeepStartIndex2: TSpinEditEx
                  Left = 70
                  Top = 69
                  Width = 96
                  Height = 22
                  Hint = '>=0'#21017#24320#21551#29305#25928'2'#65292#29305#25928'1'#21644#29305#25928'2'#21516#27493#25773#25918
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 6
                  Value = 0
                  OnChange = seClientSelfKeepStartIndex2Change
                end
                object cbbClientSelfKeepDrawMode2: TComboBox
                  Left = 70
                  Top = 197
                  Width = 96
                  Height = 21
                  Hint = #29305#25928'2'#32472#21046#27169#24335'a'
                  Style = csDropDownList
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 7
                  OnChange = cbbClientSelfKeepDrawMode2Change
                  Items.Strings = (
                    #36879#26126#32472#21046
                    #21407#22987#32472#21046)
                end
                object cbbClientSelfKeepDrawOrder: TComboBox
                  Left = 70
                  Top = 147
                  Width = 96
                  Height = 21
                  Style = csDropDownList
                  TabOrder = 8
                  OnChange = cbbClientSelfKeepDrawOrderChange
                  Items.Strings = (
                    #20808#32472#33258#36523#20877#32472#39764#27861#25928#26524
                    #20808#32472#39764#27861#25928#26524#20877#32472#33258#36523)
                end
              end
            end
            object GroupBox18: TGroupBox
              Left = 6
              Top = 5
              Width = 759
              Height = 43
              Caption = #22522#26412#35774#32622
              TabOrder = 0
              object lbl4: TLabel
                Left = 8
                Top = 19
                Width = 108
                Height = 12
                Caption = #36873#25321#35201#37197#32622#30340#25915#20987#65306
              end
              object cbbClientAttackConfig: TComboBox
                Left = 112
                Top = 15
                Width = 80
                Height = 20
                Style = csDropDownList
                TabOrder = 0
                OnChange = cbbClientAttackConfigChange
              end
            end
          end
          object tsServerAttack: TTabSheet
            Caption = #26381#21153#22120#31471#25915#20987'/'#22686#30410
            ImageIndex = 2
            object grp4: TGroupBox
              Left = 6
              Top = 5
              Width = 761
              Height = 82
              Caption = #22522#26412#35774#32622
              TabOrder = 0
              object Label55: TLabel
                Left = 8
                Top = 19
                Width = 108
                Height = 12
                Caption = #36873#25321#35201#37197#32622#30340#25915#20987#65306
              end
              object Label56: TLabel
                Left = 208
                Top = 19
                Width = 60
                Height = 12
                Caption = #35270#35273#33539#22260#65306
              end
              object lbl8: TLabel
                Left = 56
                Top = 42
                Width = 60
                Height = 12
                Caption = #24618#29289#31867#22411#65306
              end
              object Label115: TLabel
                Left = 208
                Top = 42
                Width = 60
                Height = 12
                Caption = #31227#21160#35774#32622#65306
              end
              object lblProtect: TLabel
                Left = 361
                Top = 42
                Width = 60
                Height = 12
                Caption = #23432#25252#33539#22260#65306
              end
              object lbl15: TLabel
                Left = 361
                Top = 19
                Width = 60
                Height = 12
                Caption = #25915#20987#36317#31163#65306
              end
              object Label166: TLabel
                Left = 515
                Top = 19
                Width = 60
                Height = 12
                Caption = #29031#20142#33539#22260#65306
              end
              object lbl39: TLabel
                Left = 361
                Top = 61
                Width = 354
                Height = 12
                Caption = #23432#25252#31867#22411#24618#29289#35831#22312#25968#25454#24211#35774#32622#22909#23646#24615#20026#19981#27515#31995','#31105#27490#34987#21484#21796#20316#20026#23453#23453
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object cbbServerAttackConfig: TComboBox
                Left = 112
                Top = 15
                Width = 80
                Height = 20
                Style = csDropDownList
                TabOrder = 0
                OnChange = cbbServerAttackConfigChange
              end
              object seViewRange: TSpinEditEx
                Left = 264
                Top = 14
                Width = 80
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 1
                Value = 0
                OnChange = seViewRangeChange
              end
              object cbbMonsterType: TComboBox
                Left = 112
                Top = 38
                Width = 80
                Height = 20
                Style = csDropDownList
                TabOrder = 3
                OnChange = cbbMonsterTypeChange
              end
              object cbbMoveOption: TComboBox
                Left = 264
                Top = 38
                Width = 80
                Height = 20
                Style = csDropDownList
                TabOrder = 4
                OnChange = cbbMoveOptionChange
              end
              object seProtectRange: TSpinEditEx
                Left = 417
                Top = 37
                Width = 80
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 5
                Value = 0
                OnChange = seProtectRangeChange
              end
              object seMinAttackNearRange: TSpinEditEx
                Left = 417
                Top = 14
                Width = 80
                Height = 21
                Hint = #29992#20110#35774#32622#24618#29289#19982#25915#20987#30446#26631#20043#38388#30340#31449#20301#36317#31163#65292#19968#33324#29992#20110#20307#22411#36739#22823#30340#24618#29289#13#10#13#10#35831#21153#24517#20445#35777#26368#36817#25915#20987#36317#31163#23567#20110#25110#31561#20110#26368#36817#25915#33539#22260
                MaxValue = 0
                MinValue = 0
                ParentShowHint = False
                ShowHint = True
                TabOrder = 2
                Value = 0
                OnChange = seMinAttackNearRangeChange
              end
              object seLightRange: TSpinEditEx
                Left = 571
                Top = 14
                Width = 80
                Height = 21
                Hint = #29992#20110#35774#32622#40657#22812#25928#26524#26102#24618#29289#29031#20142#33539#22260#65292#20540#36234#22823#33539#22260#36234#22823#65307'0'#34920#31034#26080#29031#20142#25928#26524
                MaxValue = 5
                MinValue = 0
                ParentShowHint = False
                ShowHint = True
                TabOrder = 6
                Value = 0
                OnChange = seLightRangeChange
              end
              object chkNoAttack: TCheckBox
                Left = 667
                Top = 16
                Width = 84
                Height = 17
                Caption = #24618#29289#19981#25915#20987
                TabOrder = 7
                OnClick = chkNoAttackClick
              end
            end
            object grpServerAttackConfigs: TGroupBox
              Left = 6
              Top = 89
              Width = 761
              Height = 496
              Caption = #25915#20987'1'#30340#37197#32622
              TabOrder = 1
              object lbl12: TLabel
                Left = 125
                Top = 22
                Width = 60
                Height = 12
                Caption = #25805#20316#27169#24335#65306
              end
              object lbl2: TLabel
                Left = 8
                Top = 473
                Width = 606
                Height = 12
                Caption = #27880#24847#65306#22914#35201#20851#38381#40664#35748#25915#20987#65292#21487#20197#35753#26368#21518#19968#20010#25915#20987#26377'100%'#30340#20351#29992#29575#65288#25915#20987#35268#21017#65306#25915#20987'1'#65292#25915#20987'2'#65292#25915#20987'3'#8230#8230#40664#35748#25915#20987#65289
                Font.Charset = GB2312_CHARSET
                Font.Color = clRed
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object lblAttackDelay: TLabel
                Left = 318
                Top = 22
                Width = 84
                Height = 12
                Caption = #20260#23475#24310#26102#26102#38388#65306
                Font.Charset = GB2312_CHARSET
                Font.Color = clBlue
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
              end
              object lblAttackDelayTime: TLabel
                Left = 459
                Top = 22
                Width = 24
                Height = 12
                Caption = #27627#31186
              end
              object chkAttackEnabled: TCheckBox
                Left = 10
                Top = 19
                Width = 100
                Height = 17
                Caption = #21551#29992#25915#20987'/'#22686#30410
                Font.Charset = GB2312_CHARSET
                Font.Color = clRed
                Font.Height = -12
                Font.Name = #23435#20307
                Font.Style = []
                ParentFont = False
                TabOrder = 0
                OnClick = chkAttackEnabledClick
              end
              object cbbOperateMode: TComboBox
                Left = 183
                Top = 18
                Width = 103
                Height = 20
                Style = csDropDownList
                TabOrder = 1
                OnChange = cbbOperateModeChange
              end
              object GroupBox12: TGroupBox
                Left = 9
                Top = 42
                Width = 166
                Height = 92
                Caption = #22522#26412#35774#32622
                TabOrder = 2
                object Label59: TLabel
                  Left = 8
                  Top = 22
                  Width = 84
                  Height = 12
                  Caption = #20351#29992#26465#20214#65306'HP%<'
                end
                object Label60: TLabel
                  Left = 8
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #20351#29992#20960#29575#65306
                end
                object lbl6: TLabel
                  Left = 151
                  Top = 22
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label57: TLabel
                  Left = 151
                  Top = 45
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label64: TLabel
                  Left = 8
                  Top = 68
                  Width = 72
                  Height = 12
                  Caption = #30446#26631#25968#37327#65306' >'
                end
                object Label71: TLabel
                  Left = 148
                  Top = 68
                  Width = 12
                  Height = 12
                  Caption = #20010
                end
                object seAttackRate: TSpinEditEx
                  Left = 65
                  Top = 41
                  Width = 80
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seAttackRateChange
                end
                object seAttackHPPercent: TSpinEditEx
                  Left = 95
                  Top = 18
                  Width = 50
                  Height = 21
                  MaxValue = 101
                  MinValue = 0
                  TabOrder = 0
                  Value = 0
                  OnChange = seAttackHPPercentChange
                end
                object seAttackTargetCount: TSpinEditEx
                  Left = 95
                  Top = 64
                  Width = 50
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seAttackTargetCountChange
                end
              end
              object grpOptions: TGroupBox
                Left = 9
                Top = 138
                Width = 166
                Height = 254
                Caption = #25915#20987#36873#39033
                TabOrder = 3
                object Label52: TLabel
                  Left = 8
                  Top = 158
                  Width = 60
                  Height = 12
                  Caption = #36817#25915#33539#22260#65306
                end
                object Label53: TLabel
                  Left = 148
                  Top = 158
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object Label54: TLabel
                  Left = 8
                  Top = 182
                  Width = 60
                  Height = 12
                  Caption = #32676#25915#33539#22260#65306
                end
                object Label72: TLabel
                  Left = 148
                  Top = 182
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object Label75: TLabel
                  Left = 151
                  Top = 91
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label62: TLabel
                  Left = 8
                  Top = 22
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#31867#22411#65306
                end
                object Label63: TLabel
                  Left = 8
                  Top = 45
                  Width = 60
                  Height = 12
                  Caption = #25915#20987#30446#26631#65306
                end
                object Label73: TLabel
                  Left = 8
                  Top = 68
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#35745#31639#65306
                end
                object Label74: TLabel
                  Left = 8
                  Top = 91
                  Width = 60
                  Height = 12
                  Caption = #23041#21147#20493#25968#65306
                end
                object lbl26: TLabel
                  Left = 8
                  Top = 228
                  Width = 96
                  Height = 12
                  Caption = #38543#30446#26631#25968#36882#22686#25915#20987
                end
                object seAttackNearRange: TSpinEditEx
                  Left = 65
                  Top = 153
                  Width = 80
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 6
                  Value = 0
                  OnChange = seAttackNearRangeChange
                end
                object seAttackGroupRange: TSpinEditEx
                  Left = 65
                  Top = 178
                  Width = 80
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 7
                  Value = 0
                  OnChange = seAttackGroupRangeChange
                end
                object cbbAttackMode: TComboBox
                  Left = 65
                  Top = 18
                  Width = 80
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 0
                  OnChange = cbbAttackModeChange
                end
                object cbbAttackTarget: TComboBox
                  Left = 65
                  Top = 41
                  Width = 80
                  Height = 20
                  Style = csDropDownList
                  TabOrder = 1
                  OnChange = cbbAttackTargetChange
                end
                object chkAttackIgnoreDefence: TCheckBox
                  Left = 8
                  Top = 115
                  Width = 129
                  Height = 17
                  Hint = #21246#36873#26102#65292#29289#29702#25915#20987#19981#35745#31639#30446#26631#30340#38450#24481#13#10#13#10#26410#21246#36873#26102#65292#35745#31639#30446#26631#30340#24573#35270#38450#24481#20960#29575
                  Caption = #29289#29702#25915#20987#26080#35270#38450#24481
                  TabOrder = 4
                  OnClick = chkAttackIgnoreDefenceClick
                end
                object cbbAttackPowerCalc: TComboBox
                  Left = 65
                  Top = 64
                  Width = 80
                  Height = 20
                  Hint = 'DC'#31639#29289#29702#25915#20987#65292#25915#20987#21518#35302#30446#26631#30340' @Struck'#13#10'SC/MC'#31639#39764#27861#25915#20987#65292#25915#20987#21518#35302#30446#26631#30340' @MagStruck'
                  Style = csDropDownList
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 2
                  OnChange = cbbAttackPowerCalcChange
                end
                object seAttackPowerRate: TSpinEditEx
                  Left = 65
                  Top = 86
                  Width = 80
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seAttackPowerRateChange
                end
                object chkAttackSelfDie: TCheckBox
                  Left = 8
                  Top = 133
                  Width = 97
                  Height = 17
                  Hint = #25915#20987#30446#26631#21518#24618#29289#33258#21160#27515#20129
                  Caption = #33258#26432#24335#25915#20987
                  Font.Charset = GB2312_CHARSET
                  Font.Color = clBlue
                  Font.Height = -12
                  Font.Name = #23435#20307
                  Font.Style = []
                  ParentFont = False
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 5
                  OnClick = chkAttackSelfDieClick
                end
                object seAttackPowerInc: TSpinEditEx
                  Left = 105
                  Top = 223
                  Width = 54
                  Height = 21
                  Hint = #35813#36873#39033#21482#38024#23545#20110#32676#25915#65292#24403#25915#20987#30446#26631#26377'3'#20010#26102#65292#36882#22686#20540#35774#32622#20026'5'#65292#34920#31034#25915#20987#21147#21152' (3-1)*5=10'
                  MaxValue = 0
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 8
                  Value = 0
                  OnChange = seAttackPowerIncChange
                end
                object chkNearAttackTargetCenter: TCheckBox
                  Left = 8
                  Top = 203
                  Width = 145
                  Height = 17
                  Hint = 
                    #27492#36873#39033#20165#23545#25915#20987#31867#22411#20026#8220#36817#25915#8221#30340#19988#25915#20987#30446#26631#20026#8220#32676#25915#25915#20987#8221#29983#25928#65292#19981#21246#36873#65292#32676#25915#33539#22260#20026#25915#20987#32773#33258#36523#20026#25915#20987#20013#24515#65292#21246#36873#21518#20197#20260#23475#33539#22260#20197#30446#26631#20026#25915#20987 +
                    #20013#24515
                  Caption = #32676#25915#33539#22260#20197#30446#26631#20026#20013#24515
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 9
                  OnClick = chkNearAttackTargetCenterClick
                end
              end
              object grpMove: TGroupBox
                Left = 183
                Top = 42
                Width = 208
                Height = 92
                TabOrder = 4
                object lbl16: TLabel
                  Left = 7
                  Top = 45
                  Width = 84
                  Height = 12
                  Caption = #30636#31227#31227#21160#36317#31163#65306
                end
                object Label65: TLabel
                  Left = 7
                  Top = 68
                  Width = 84
                  Height = 12
                  Caption = #30636#31227#20351#29992#20960#29575#65306
                end
                object Label67: TLabel
                  Left = 191
                  Top = 68
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label68: TLabel
                  Left = 7
                  Top = 22
                  Width = 84
                  Height = 12
                  Caption = #25915#20987#30446#26631#36317#31163#65306
                end
                object Label69: TLabel
                  Left = 188
                  Top = 22
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object Label70: TLabel
                  Left = 188
                  Top = 46
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object chkAttackTeleportAttack: TCheckBox
                  Left = 8
                  Top = 0
                  Width = 71
                  Height = 17
                  Caption = #30636#31227#31227#21160
                  TabOrder = 0
                  OnClick = chkAttackTeleportAttackClick
                end
                object seAttackTeleportDistance: TSpinEditEx
                  Left = 89
                  Top = 41
                  Width = 94
                  Height = 21
                  Hint = #30636#31227#26102#21521#30446#26631#38752#36817#30340#36317#31163#65292#22914#26524#20026'9'#21017#31227#21160#30446#26631#36523#36793
                  MaxValue = 9
                  MinValue = 1
                  TabOrder = 2
                  Value = 1
                  OnChange = seAttackTeleportDistanceChange
                end
                object seAttackTeleportRate: TSpinEditEx
                  Left = 89
                  Top = 64
                  Width = 94
                  Height = 21
                  Hint = #25968#20540#36234#22823#20960#29575#36234#39640#65292'0%'#26102#34920#31034#19981#30636#31227
                  MaxValue = 100
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 3
                  Value = 0
                  OnChange = seAttackTeleportRateChange
                end
                object seAttackTeleportTargetDistance: TSpinEditEx
                  Left = 89
                  Top = 18
                  Width = 94
                  Height = 21
                  Hint = #24403#19982#25915#20987#30446#26631#30456#38548#19968#23450#30340#36317#31163#26102#65292#20250#35302#21457#30636#31227
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seAttackTeleportTargetDistanceChange
                end
                object chkAttackTeleportRush: TCheckBox
                  Left = 85
                  Top = 0
                  Width = 84
                  Height = 17
                  Caption = #31361#36827#22411#30636#31227
                  TabOrder = 4
                  OnClick = chkAttackTeleportRushClick
                end
              end
              object grpCallMob: TGroupBox
                Left = 183
                Top = 138
                Width = 208
                Height = 137
                TabOrder = 5
                object lbl11: TLabel
                  Left = 7
                  Top = 22
                  Width = 42
                  Height = 12
                  Caption = #24618#29289'1'#65306
                end
                object Label97: TLabel
                  Left = 125
                  Top = 22
                  Width = 36
                  Height = 12
                  Caption = #25968#37327#65306
                end
                object Label98: TLabel
                  Left = 7
                  Top = 45
                  Width = 42
                  Height = 12
                  Caption = #24618#29289'2'#65306
                end
                object Label99: TLabel
                  Left = 125
                  Top = 45
                  Width = 36
                  Height = 12
                  Caption = #25968#37327#65306
                end
                object Label117: TLabel
                  Left = 7
                  Top = 68
                  Width = 42
                  Height = 12
                  Caption = #24618#29289'3'#65306
                end
                object Label118: TLabel
                  Left = 125
                  Top = 68
                  Width = 36
                  Height = 12
                  Caption = #25968#37327#65306
                end
                object Label121: TLabel
                  Left = 7
                  Top = 91
                  Width = 42
                  Height = 12
                  Caption = #24618#29289'4'#65306
                end
                object Label122: TLabel
                  Left = 125
                  Top = 91
                  Width = 36
                  Height = 12
                  Caption = #25968#37327#65306
                end
                object Label143: TLabel
                  Left = 7
                  Top = 116
                  Width = 84
                  Height = 12
                  Caption = #21484#21796#24618#29289#20960#29575#65306
                end
                object Label144: TLabel
                  Left = 191
                  Top = 116
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object edtCallMonster1: TEdit
                  Left = 44
                  Top = 18
                  Width = 68
                  Height = 20
                  TabOrder = 1
                  OnChange = edtCallMonster1Change
                end
                object seCallMonsterNum1: TSpinEditEx
                  Left = 156
                  Top = 17
                  Width = 42
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seCallMonsterNum1Change
                end
                object edtCallMonster2: TEdit
                  Tag = 1
                  Left = 44
                  Top = 41
                  Width = 68
                  Height = 20
                  TabOrder = 3
                  OnChange = edtCallMonster1Change
                end
                object seCallMonsterNum2: TSpinEditEx
                  Tag = 1
                  Left = 156
                  Top = 40
                  Width = 42
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seCallMonsterNum1Change
                end
                object edtCallMonster3: TEdit
                  Tag = 2
                  Left = 44
                  Top = 64
                  Width = 68
                  Height = 20
                  TabOrder = 5
                  OnChange = edtCallMonster1Change
                end
                object seCallMonsterNum3: TSpinEditEx
                  Tag = 2
                  Left = 156
                  Top = 63
                  Width = 42
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 6
                  Value = 0
                  OnChange = seCallMonsterNum1Change
                end
                object edtCallMonster4: TEdit
                  Tag = 3
                  Left = 44
                  Top = 87
                  Width = 68
                  Height = 20
                  TabOrder = 7
                  OnChange = edtCallMonster1Change
                end
                object seCallMonsterNum4: TSpinEditEx
                  Tag = 3
                  Left = 156
                  Top = 86
                  Width = 42
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 8
                  Value = 0
                  OnChange = seCallMonsterNum1Change
                end
                object chkEnabledCallMonster: TCheckBox
                  Left = 8
                  Top = 0
                  Width = 95
                  Height = 17
                  Caption = #21484#21796#24618#29289#25915#20987
                  TabOrder = 0
                  OnClick = chkEnabledCallMonsterClick
                end
                object seCallMonstersRate: TSpinEditEx
                  Left = 89
                  Top = 112
                  Width = 95
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 9
                  Value = 0
                  OnChange = seCallMonstersRateChange
                end
              end
              object grpAdditionals: TGroupBox
                Left = 399
                Top = 42
                Width = 354
                Height = 305
                Caption = #38468#21152#20260#23475#25915#20987
                TabOrder = 6
                object Label76: TLabel
                  Left = 64
                  Top = 22
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label77: TLabel
                  Left = 164
                  Top = 22
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label78: TLabel
                  Left = 64
                  Top = 47
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label79: TLabel
                  Left = 164
                  Top = 47
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label80: TLabel
                  Left = 64
                  Top = 71
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label81: TLabel
                  Left = 164
                  Top = 71
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label82: TLabel
                  Left = 64
                  Top = 94
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label83: TLabel
                  Left = 164
                  Top = 94
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label84: TLabel
                  Left = 64
                  Top = 140
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label85: TLabel
                  Left = 164
                  Top = 140
                  Width = 36
                  Height = 12
                  Caption = #26684#25968#65306
                end
                object Label86: TLabel
                  Left = 64
                  Top = 165
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label87: TLabel
                  Left = 164
                  Top = 165
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object Label88: TLabel
                  Left = 64
                  Top = 188
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label89: TLabel
                  Left = 164
                  Top = 188
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object Label90: TLabel
                  Left = 64
                  Top = 211
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label91: TLabel
                  Left = 164
                  Top = 211
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label92: TLabel
                  Left = 64
                  Top = 234
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label93: TLabel
                  Left = 164
                  Top = 234
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label94: TLabel
                  Left = 64
                  Top = 257
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label95: TLabel
                  Left = 164
                  Top = 257
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label96: TLabel
                  Left = 264
                  Top = 22
                  Width = 36
                  Height = 12
                  Caption = #25481#34880#65306
                end
                object Label141: TLabel
                  Left = 64
                  Top = 117
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object Label142: TLabel
                  Left = 164
                  Top = 117
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object lbl27: TLabel
                  Left = 64
                  Top = 280
                  Width = 36
                  Height = 12
                  Caption = #26426#29575#65306
                end
                object lbl28: TLabel
                  Left = 164
                  Top = 280
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object lbl29: TLabel
                  Left = 264
                  Top = 278
                  Width = 36
                  Height = 12
                  Caption = #26684#25968#65306
                end
                object chkAdditional0: TCheckBox
                  Left = 13
                  Top = 20
                  Width = 48
                  Height = 17
                  Caption = #32511#27602
                  TabOrder = 0
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate0: TSpinEditEx
                  Left = 96
                  Top = 18
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime0: TSpinEditEx
                  Left = 197
                  Top = 18
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional1: TCheckBox
                  Tag = 1
                  Left = 13
                  Top = 45
                  Width = 48
                  Height = 17
                  Caption = #32418#27602
                  TabOrder = 4
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate1: TSpinEditEx
                  Tag = 1
                  Left = 97
                  Top = 42
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 5
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime1: TSpinEditEx
                  Tag = 1
                  Left = 197
                  Top = 42
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 6
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional2: TCheckBox
                  Tag = 2
                  Left = 13
                  Top = 69
                  Width = 48
                  Height = 17
                  Caption = #40635#30201
                  TabOrder = 7
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate2: TSpinEditEx
                  Tag = 2
                  Left = 97
                  Top = 66
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 8
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime2: TSpinEditEx
                  Tag = 2
                  Left = 197
                  Top = 66
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 9
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional3: TCheckBox
                  Tag = 3
                  Left = 13
                  Top = 92
                  Width = 48
                  Height = 17
                  Caption = #20912#20923
                  TabOrder = 10
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate3: TSpinEditEx
                  Tag = 3
                  Left = 97
                  Top = 89
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 11
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime3: TSpinEditEx
                  Tag = 3
                  Left = 197
                  Top = 89
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 12
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional4: TCheckBox
                  Tag = 4
                  Left = 13
                  Top = 138
                  Width = 48
                  Height = 17
                  Caption = #25512#21160
                  TabOrder = 16
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate4: TSpinEditEx
                  Tag = 4
                  Left = 97
                  Top = 135
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 17
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime4: TSpinEditEx
                  Tag = 4
                  Left = 197
                  Top = 135
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 18
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional5: TCheckBox
                  Tag = 5
                  Left = 13
                  Top = 163
                  Width = 48
                  Height = 17
                  Caption = #21560#34880
                  TabOrder = 20
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate5: TSpinEditEx
                  Tag = 5
                  Left = 97
                  Top = 160
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 21
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime5: TSpinEditEx
                  Tag = 5
                  Left = 197
                  Top = 160
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 22
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional6: TCheckBox
                  Tag = 6
                  Left = 13
                  Top = 186
                  Width = 48
                  Height = 17
                  Caption = #21560#34013
                  TabOrder = 23
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate6: TSpinEditEx
                  Tag = 6
                  Left = 97
                  Top = 183
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 24
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime6: TSpinEditEx
                  Tag = 6
                  Left = 197
                  Top = 183
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 25
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional7: TCheckBox
                  Tag = 7
                  Left = 13
                  Top = 209
                  Width = 48
                  Height = 17
                  Caption = #34523#32593
                  TabOrder = 26
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate7: TSpinEditEx
                  Tag = 7
                  Left = 97
                  Top = 206
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 27
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime7: TSpinEditEx
                  Tag = 7
                  Left = 197
                  Top = 206
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 28
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional8: TCheckBox
                  Tag = 8
                  Left = 13
                  Top = 232
                  Width = 48
                  Height = 17
                  Caption = '0'#38450#24481
                  TabOrder = 29
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate8: TSpinEditEx
                  Tag = 8
                  Left = 97
                  Top = 229
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 30
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime8: TSpinEditEx
                  Tag = 8
                  Left = 197
                  Top = 229
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 31
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional9: TCheckBox
                  Tag = 9
                  Left = 13
                  Top = 255
                  Width = 48
                  Height = 17
                  Caption = '0'#39764#24481
                  TabOrder = 32
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate9: TSpinEditEx
                  Tag = 9
                  Left = 97
                  Top = 252
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 33
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime9: TSpinEditEx
                  Tag = 9
                  Left = 197
                  Top = 252
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 34
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkseAdditionaHighLevel4: TCheckBox
                  Left = 266
                  Top = 138
                  Width = 81
                  Height = 17
                  Caption = #21487#25512#39640#32423#21035
                  TabOrder = 19
                  OnClick = chkseAdditionaHighLevel4Click
                end
                object seAdditionaHP0: TSpinEditEx
                  Left = 296
                  Top = 18
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 3
                  Value = 0
                  OnChange = seAdditionaHP0Change
                end
                object chkAdditional10: TCheckBox
                  Tag = 10
                  Left = 13
                  Top = 115
                  Width = 48
                  Height = 17
                  Caption = #20912#23553
                  TabOrder = 13
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate10: TSpinEditEx
                  Tag = 10
                  Left = 97
                  Top = 112
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 14
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime10: TSpinEditEx
                  Tag = 10
                  Left = 196
                  Top = 112
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 15
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object chkAdditional11: TCheckBox
                  Tag = 11
                  Left = 13
                  Top = 278
                  Width = 48
                  Height = 17
                  Caption = #31105#38178
                  TabOrder = 35
                  OnClick = chkAdditional0Click
                end
                object seAdditionalRate11: TSpinEditEx
                  Tag = 11
                  Left = 97
                  Top = 275
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 36
                  Value = 0
                  OnChange = seAdditionalRate0Change
                end
                object seAdditionalTime11: TSpinEditEx
                  Tag = 11
                  Left = 197
                  Top = 275
                  Width = 56
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 37
                  Value = 0
                  OnChange = seAdditionalTime0Change
                end
                object seAdditionalImprisonRange: TSpinEditEx
                  Left = 296
                  Top = 274
                  Width = 56
                  Height = 21
                  MaxValue = 15
                  MinValue = 0
                  TabOrder = 38
                  Value = 0
                  OnChange = seAdditionalImprisonRangeChange
                end
              end
              object grpMoveTarget: TGroupBox
                Left = 183
                Top = 280
                Width = 209
                Height = 65
                TabOrder = 8
                object Label179: TLabel
                  Left = 7
                  Top = 22
                  Width = 84
                  Height = 12
                  Caption = #25235#21462#30446#26631#20960#29575#65306
                end
                object Label180: TLabel
                  Left = 190
                  Top = 22
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object seMoveTargetRate: TSpinEditEx
                  Left = 88
                  Top = 17
                  Width = 94
                  Height = 21
                  Hint = #25968#20540#36234#22823#20960#29575#36234#39640#65292'0%'#26102#34920#31034#19981#30636#31227
                  MaxValue = 100
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 1
                  Value = 0
                  OnChange = seMoveTargetRateChange
                end
                object chkMoveTarget: TCheckBox
                  Left = 8
                  Top = 0
                  Width = 93
                  Height = 17
                  Caption = #25235#30446#26631#21040#36523#36793
                  TabOrder = 0
                  OnClick = chkMoveTargetClick
                end
                object chkMoveTargetHighLevel: TCheckBox
                  Left = 8
                  Top = 42
                  Width = 129
                  Height = 17
                  Caption = #20801#35768#25235#39640#31561#32423#30446#26631
                  TabOrder = 2
                  OnClick = chkMoveTargetHighLevelClick
                end
              end
              object seAttackDelayTime: TSpinEditEx
                Left = 399
                Top = 17
                Width = 56
                Height = 21
                MaxValue = 0
                MinValue = 0
                TabOrder = 9
                Value = 0
                OnChange = seAttackDelayTimeChange
              end
              object grpProtect: TGroupBox
                Left = 9
                Top = 144
                Width = 382
                Height = 206
                Caption = #22686#30410#36873#39033
                TabOrder = 7
                Visible = False
                object Label130: TLabel
                  Left = 176
                  Top = 22
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object Label116: TLabel
                  Left = 255
                  Top = 22
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label119: TLabel
                  Left = 152
                  Top = 22
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label120: TLabel
                  Left = 277
                  Top = 46
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label123: TLabel
                  Left = 152
                  Top = 46
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label102: TLabel
                  Left = 152
                  Top = 69
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label124: TLabel
                  Left = 277
                  Top = 69
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label134: TLabel
                  Left = 353
                  Top = 46
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label135: TLabel
                  Left = 353
                  Top = 69
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label131: TLabel
                  Left = 8
                  Top = 161
                  Width = 84
                  Height = 12
                  Caption = #22686#30410#30446#26631#33539#22260#65306
                end
                object Label132: TLabel
                  Left = 132
                  Top = 161
                  Width = 12
                  Height = 12
                  Caption = #26684
                end
                object Label133: TLabel
                  Left = 8
                  Top = 184
                  Width = 84
                  Height = 12
                  Caption = #33258#25105#20013#24515#20960#29575#65306
                end
                object Label136: TLabel
                  Left = 255
                  Top = 92
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label138: TLabel
                  Left = 176
                  Top = 92
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object Label139: TLabel
                  Left = 135
                  Top = 184
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label137: TLabel
                  Left = 152
                  Top = 92
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label58: TLabel
                  Left = 26
                  Top = 266
                  Width = 72
                  Height = 12
                  Caption = #25915#39764#36947#31867#22411#65306
                  Visible = False
                end
                object lbl7: TLabel
                  Left = 74
                  Top = 23
                  Width = 36
                  Height = 12
                  Caption = #20960#29575#65306
                end
                object lbl17: TLabel
                  Left = 74
                  Top = 46
                  Width = 36
                  Height = 12
                  Caption = #20960#29575#65306
                end
                object lbl18: TLabel
                  Left = 74
                  Top = 69
                  Width = 36
                  Height = 12
                  Caption = #20960#29575#65306
                end
                object Label140: TLabel
                  Left = 74
                  Top = 92
                  Width = 36
                  Height = 12
                  Caption = #20960#29575#65306
                end
                object Label145: TLabel
                  Left = 277
                  Top = 91
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label146: TLabel
                  Left = 353
                  Top = 91
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label147: TLabel
                  Left = 255
                  Top = 115
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label148: TLabel
                  Left = 176
                  Top = 115
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object Label149: TLabel
                  Left = 152
                  Top = 115
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label150: TLabel
                  Left = 74
                  Top = 115
                  Width = 36
                  Height = 12
                  Caption = #20960#29575#65306
                end
                object Label151: TLabel
                  Left = 277
                  Top = 114
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label153: TLabel
                  Left = 255
                  Top = 138
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label155: TLabel
                  Left = 176
                  Top = 138
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object Label157: TLabel
                  Left = 152
                  Top = 138
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object Label159: TLabel
                  Left = 74
                  Top = 138
                  Width = 36
                  Height = 12
                  Caption = #20960#29575#65306
                end
                object Label160: TLabel
                  Left = 277
                  Top = 137
                  Width = 36
                  Height = 12
                  Caption = #26102#38388#65306
                end
                object Label161: TLabel
                  Left = 353
                  Top = 114
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object Label164: TLabel
                  Left = 353
                  Top = 137
                  Width = 12
                  Height = 12
                  Caption = #31186
                end
                object lbl30: TLabel
                  Left = 255
                  Top = 46
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object lbl31: TLabel
                  Left = 176
                  Top = 46
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object lbl32: TLabel
                  Left = 255
                  Top = 69
                  Width = 6
                  Height = 12
                  Caption = '%'
                end
                object lbl41: TLabel
                  Left = 176
                  Top = 69
                  Width = 36
                  Height = 12
                  Caption = #27604#20363#65306
                end
                object chkProtectAddHP: TCheckBox
                  Left = 8
                  Top = 20
                  Width = 60
                  Height = 17
                  Caption = #21152#34880#37327
                  TabOrder = 0
                  OnClick = chkProtectAddHPClick
                end
                object chkProtectAddDefence: TCheckBox
                  Left = 8
                  Top = 43
                  Width = 60
                  Height = 17
                  Caption = #21152#38450#24481
                  TabOrder = 3
                  OnClick = chkProtectAddDefenceClick
                end
                object chkProtectAddMagDefence: TCheckBox
                  Left = 8
                  Top = 66
                  Width = 60
                  Height = 17
                  Caption = #21152#39764#24481
                  TabOrder = 7
                  OnClick = chkProtectAddMagDefenceClick
                end
                object seProtectAddHPRate: TSpinEditEx
                  Left = 107
                  Top = 18
                  Width = 40
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 1
                  Value = 0
                  OnChange = seProtectAddHPRateChange
                end
                object seProtectAddHPPercent: TSpinEditEx
                  Left = 209
                  Top = 18
                  Width = 40
                  Height = 21
                  MaxValue = 1000
                  MinValue = 0
                  TabOrder = 2
                  Value = 0
                  OnChange = seProtectAddHPPercentChange
                end
                object seProtectAddDefenceRate: TSpinEditEx
                  Left = 107
                  Top = 41
                  Width = 40
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 4
                  Value = 0
                  OnChange = seProtectAddDefenceRateChange
                end
                object seProtectAddDefenceTime: TSpinEditEx
                  Left = 310
                  Top = 41
                  Width = 40
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 6
                  Value = 0
                  OnChange = seProtectAddDefenceTimeChange
                end
                object seProtectAddMagDefenceRate: TSpinEditEx
                  Left = 107
                  Top = 64
                  Width = 40
                  Height = 21
                  MaxValue = 100
                  MinValue = 0
                  TabOrder = 8
                  Value = 0
                  OnChange = seProtectAddMagDefenceRateChange
                end
                object seProtectAddMagDefenceTime: TSpinEditEx
                  Left = 310
                  Top = 64
                  Width = 40
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 10
                  Value = 0
                  OnChange = seProtectAddMagDefenceTimeChange
                end
                object seProtectTargetRange: TSpinEditEx
                  Left = 89
                  Top = 157
                  Width = 40
                  Height = 21
                  Hint = #20197#22686#30410#30446#26631#20026#20013#24515#65292#22686#30410#30340#38431#21451#33539#22260
                  MaxValue = 9
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 23
                  Value = 0
                  OnChange = seProtectTargetRangeChange
                end
                object seProtectSelfRate: TSpinEditEx
                  Left = 89
                  Top = 180
                  Width = 40
                  Height = 21
                  Hint = #33258#25105#22686#30410#20960#29575#65306#26159#25351#20197#33258#24049#20026#20013#24515#65292#22686#30410#21608#36793#38431#21451#30340#20960#29575#13#10'100%-'#33258#25105#22686#30410#20960#29575#65306#21363#20197#26576#20010#38431#21451#20026#20013#24515#65292#22686#30410#21608#36793#38431#21451#30340#20960#29575
                  MaxValue = 100
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 24
                  Value = 0
                  OnChange = seProtectSelfRateChange
                end
                object seProtectAddDCRate: TSpinEditEx
                  Left = 107
                  Top = 87
                  Width = 40
                  Height = 21
                  Hint = #20197#22686#30410#30446#26631#20026#20013#24515#65292#22686#30410#30340#38431#21451#33539#22260
                  MaxValue = 100
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 12
                  Value = 0
                  OnChange = seProtectAddDCRateChange
                end
                object seProtectAddDCPercent: TSpinEditEx
                  Left = 209
                  Top = 87
                  Width = 40
                  Height = 21
                  MaxValue = 1000
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 13
                  Value = 0
                  OnChange = seProtectAddDCPercentChange
                end
                object chkProtectAddDC: TCheckBox
                  Left = 8
                  Top = 89
                  Width = 60
                  Height = 17
                  Caption = #21152#25915#20987
                  TabOrder = 11
                  OnClick = chkProtectAddDCClick
                end
                object cbb1: TComboBox
                  Left = 96
                  Top = 262
                  Width = 77
                  Height = 20
                  ItemIndex = 0
                  TabOrder = 25
                  Text = 'DC'
                  Visible = False
                  Items.Strings = (
                    'DC'
                    'MC'
                    'SC'
                    'DC+MC'
                    'DC+SC'
                    'MC+SC'
                    'DC+MC+SC')
                end
                object seProtectAddDCTime: TSpinEditEx
                  Left = 310
                  Top = 87
                  Width = 40
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 14
                  Value = 0
                  OnChange = seProtectAddDCTimeChange
                end
                object seProtectAddMCRate: TSpinEditEx
                  Left = 107
                  Top = 110
                  Width = 40
                  Height = 21
                  Hint = #20197#22686#30410#30446#26631#20026#20013#24515#65292#22686#30410#30340#38431#21451#33539#22260
                  MaxValue = 100
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 16
                  Value = 0
                  OnChange = seProtectAddMCRateChange
                end
                object seProtectAddMCPercent: TSpinEditEx
                  Left = 209
                  Top = 110
                  Width = 40
                  Height = 21
                  MaxValue = 1000
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 17
                  Value = 0
                  OnChange = seProtectAddMCPercentChange
                end
                object chkProtectAddMC: TCheckBox
                  Left = 8
                  Top = 112
                  Width = 60
                  Height = 17
                  Caption = #21152#39764#27861
                  TabOrder = 15
                  OnClick = chkProtectAddMCClick
                end
                object seProtectAddMCTime: TSpinEditEx
                  Left = 310
                  Top = 110
                  Width = 40
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 18
                  Value = 0
                  OnChange = seProtectAddMCTimeChange
                end
                object seProtectAddSCRate: TSpinEditEx
                  Left = 107
                  Top = 133
                  Width = 40
                  Height = 21
                  Hint = #20197#22686#30410#30446#26631#20026#20013#24515#65292#22686#30410#30340#38431#21451#33539#22260
                  MaxValue = 100
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 20
                  Value = 0
                  OnChange = seProtectAddSCRateChange
                end
                object seProtectAddSCPercent: TSpinEditEx
                  Left = 209
                  Top = 133
                  Width = 40
                  Height = 21
                  MaxValue = 1000
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 21
                  Value = 0
                  OnChange = seProtectAddSCPercentChange
                end
                object chkProtectAddSC: TCheckBox
                  Left = 8
                  Top = 135
                  Width = 60
                  Height = 17
                  Caption = #21152#36947#26415
                  TabOrder = 19
                  OnClick = chkProtectAddSCClick
                end
                object seProtectAddSCTime: TSpinEditEx
                  Left = 310
                  Top = 133
                  Width = 40
                  Height = 21
                  MaxValue = 0
                  MinValue = 0
                  TabOrder = 22
                  Value = 0
                  OnChange = seProtectAddSCTimeChange
                end
                object seProtectAddDefencePercent: TSpinEditEx
                  Left = 209
                  Top = 41
                  Width = 40
                  Height = 21
                  MaxValue = 1000
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 5
                  Value = 0
                  OnChange = seProtectAddDefencePercentChange
                end
                object seProtectAddMagDefencePercent: TSpinEditEx
                  Left = 209
                  Top = 64
                  Width = 40
                  Height = 21
                  MaxValue = 1000
                  MinValue = 0
                  ParentShowHint = False
                  ShowHint = True
                  TabOrder = 9
                  Value = 0
                  OnChange = seProtectAddMagDefencePercentChange
                end
              end
            end
          end
        end
        object pnlMonDesc: TPanel
          Left = 0
          Top = 617
          Width = 776
          Height = 24
          Align = alBottom
          Alignment = taLeftJustify
          BevelInner = bvRaised
          BevelOuter = bvNone
          Font.Charset = GB2312_CHARSET
          Font.Color = clBlue
          Font.Height = -12
          Font.Name = #23435#20307
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 1
        end
      end
    end
    object tsDropItem: TTabSheet
      BorderWidth = 4
      Caption = #29289#21697#25481#33853#35268#21017
      ImageIndex = 5
      object lstAllItems: TListBox
        Left = 0
        Top = 0
        Width = 116
        Height = 667
        Hint = 'Ctrl + F '#29289#21697#26597#25214#12290
        Align = alLeft
        ItemHeight = 12
        TabOrder = 0
        OnDblClick = lstAllItemsDblClick
        OnKeyDown = lstAllItemsKeyDown
      end
      object pnlItemDropRight: TPanel
        Left = 116
        Top = 0
        Width = 770
        Height = 667
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 1
        object grp9: TGroupBox
          Left = 6
          Top = 0
          Width = 764
          Height = 668
          Caption = #35268#21017#29289#21697
          TabOrder = 0
          object lbl34: TLabel
            Left = 133
            Top = 617
            Width = 504
            Height = 12
            Caption = '*'#34920#31034#25152#26377#22320#22270#65307'Ctrl+Insert:'#28155#21152#35268#21017#65307'Ctrl+Delete:'#21024#38500#35268#21017#65307#21452#20987#29289#21697#21015#34920':'#26032#24314#35268#21017#29289#21697
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object Label23: TLabel
            Left = 133
            Top = 641
            Width = 264
            Height = 12
            Caption = #25511#21046#35268#21017#20462#25913#21518#23454#26102#29983#25928#65292#22312#28216#25103#20013#35831#24910#37325#25805#20316'!!'
            Font.Charset = GB2312_CHARSET
            Font.Color = clRed
            Font.Height = -12
            Font.Name = #23435#20307
            Font.Style = []
            ParentFont = False
          end
          object lbl35: TLabel
            Left = 8
            Top = 24
            Width = 36
            Height = 12
            Caption = #25628#32034#65306
          end
          object vstItemRules: TVirtualStringTree
            Left = 133
            Top = 19
            Width = 622
            Height = 506
            Colors.BorderColor = 15987699
            Colors.DisabledColor = clGray
            Colors.DropMarkColor = 15385233
            Colors.DropTargetColor = 15385233
            Colors.DropTargetBorderColor = 15987699
            Colors.FocusedSelectionColor = 15385233
            Colors.FocusedSelectionBorderColor = clWhite
            Colors.GridLineColor = 15987699
            Colors.HeaderHotColor = clBlack
            Colors.HotColor = clBlack
            Colors.SelectionRectangleBlendColor = 15385233
            Colors.SelectionRectangleBorderColor = 15385233
            Colors.SelectionTextColor = clBlack
            Colors.TreeLineColor = 9471874
            Colors.UnfocusedColor = 15715936
            Colors.UnfocusedSelectionColor = 15987699
            Colors.UnfocusedSelectionBorderColor = 15987699
            DefaultNodeHeight = 20
            Header.AutoSizeIndex = 0
            Header.Font.Charset = DEFAULT_CHARSET
            Header.Font.Color = clWindowText
            Header.Font.Height = -11
            Header.Font.Name = 'Tahoma'
            Header.Font.Style = []
            Header.Height = 24
            Header.MainColumn = 1
            Header.Options = [hoAutoResize, hoColumnResize, hoDrag, hoShowSortGlyphs, hoVisible]
            Margin = 2
            TabOrder = 0
            TreeOptions.MiscOptions = [toAcceptOLEDrop, toCheckSupport, toEditable, toFullRepaintOnResize, toGridExtensions, toInitOnSave, toToggleOnDblClick, toWheelPanning, toEditOnClick]
            TreeOptions.PaintOptions = [toShowButtons, toShowDropmark, toShowHorzGridLines, toShowTreeLines, toShowVertGridLines, toThemeAware, toUseBlendedImages, toFullVertGridLines]
            TreeOptions.SelectionOptions = [toExtendedFocus, toFullRowSelect]
            OnCreateEditor = vstItemRulesCreateEditor
            OnDrawText = vstItemRulesDrawText
            OnEditing = vstItemRulesEditing
            OnGetText = vstItemRulesGetText
            OnGetNodeDataSize = vstItemRulesGetNodeDataSize
            OnKeyUp = vstItemRulesKeyUp
            OnNodeClick = vstItemRulesNodeClick
            Columns = <
              item
                Margin = 2
                Position = 0
                Width = 87
                WideText = #22320#22270#21517#31216
              end
              item
                Alignment = taCenter
                Margin = 2
                Position = 1
                Width = 60
                WideText = #28165#38646#38388#38548
              end
              item
                Position = 2
                Width = 39
                WideText = #21333#20301
              end
              item
                Position = 3
                Width = 56
                WideText = #38388#38548'('#20998')'
              end
              item
                Alignment = taCenter
                Margin = 2
                Position = 4
                Width = 60
                WideText = #38480#23450#25968#37327' '
              end
              item
                Alignment = taCenter
                Margin = 2
                Position = 5
                Width = 46
                WideText = #24050#29190
              end
              item
                Alignment = taCenter
                Margin = 2
                Position = 6
                Width = 38
                WideText = #26410#29190
              end
              item
                Alignment = taCenter
                Margin = 2
                Position = 7
                Width = 74
                WideText = #28165#38646#26085#26399
              end
              item
                Alignment = taCenter
                Position = 8
                Width = 64
                WideText = #28165#38646#26102#38388
              end
              item
                Position = 9
                Width = 52
                WideText = #32047#35745
              end
              item
                Options = [coAllowClick, coDraggable, coEnabled, coParentBidiMode, coParentColor, coShowDropMark, coVisible, coAllowFocus]
                Position = 10
                Width = 42
                WideText = #26085#24535
              end>
          end
          object vstDropLimitItems: TVirtualStringTree
            Left = 8
            Top = 46
            Width = 119
            Height = 614
            Colors.BorderColor = 15987699
            Colors.DisabledColor = clGray
            Colors.DropMarkColor = 15385233
            Colors.DropTargetColor = 15385233
            Colors.DropTargetBorderColor = 15987699
            Colors.FocusedSelectionColor = 15385233
            Colors.FocusedSelectionBorderColor = clWhite
            Colors.GridLineColor = 15987699
            Colors.HeaderHotColor = clBlack
            Colors.HotColor = clBlack
            Colors.SelectionRectangleBlendColor = 15385233
            Colors.SelectionRectangleBorderColor = 15385233
            Colors.SelectionTextColor = clBlack
            Colors.TreeLineColor = 9471874
            Colors.UnfocusedColor = 15715936
            Colors.UnfocusedSelectionColor = 15987699
            Colors.UnfocusedSelectionBorderColor = 15987699
            Header.AutoSizeIndex = 0
            Header.Font.Charset = DEFAULT_CHARSET
            Header.Font.Color = clWindowText
            Header.Font.Height = -11
            Header.Font.Name = 'Tahoma'
            Header.Font.Style = []
            Header.MainColumn = -1
            TabOrder = 1
            TreeOptions.PaintOptions = [toShowDropmark, toThemeAware, toUseBlendedImages]
            TreeOptions.SelectionOptions = [toFullRowSelect]
            OnGetText = vstDropLimitItemsGetText
            OnGetNodeDataSize = vstDropLimitItemsGetNodeDataSize
            OnNodeClick = vstDropLimitItemsNodeClick
            Columns = <>
          end
          object btnClearItemRule: TButton
            Left = 598
            Top = 635
            Width = 75
            Height = 25
            Caption = #28165#31354#35268#21017
            TabOrder = 2
            OnClick = btnClearItemRuleClick
          end
          object Button1: TButton
            Left = 678
            Top = 635
            Width = 75
            Height = 25
            Caption = #21024#38500#29289#21697
            TabOrder = 3
            OnClick = Button1Click
          end
          object chkRecordLog: TCheckBox
            Left = 518
            Top = 639
            Width = 73
            Height = 17
            Hint = #35268#21017#20026#31354#30340#29289#21697#19981#35760#24405#26085#24535#13#10#13#10#22914#26524#29289#21697#25481#33853#20960#29575#36739#22823#65292#19981#24314#35758#35760#24405#26085#24535
            Caption = #35760#24405#26085#24535
            ParentShowHint = False
            ShowHint = True
            TabOrder = 4
            OnClick = chkRecordLogClick
          end
          object edtItemSearch: TEdit
            Left = 40
            Top = 20
            Width = 86
            Height = 20
            TabOrder = 5
            OnChange = edtItemSearchChange
            OnKeyPress = edtItemSearchKeyPress
          end
          object GroupBox8: TGroupBox
            Left = 133
            Top = 531
            Width = 623
            Height = 76
            Caption = #25481#33853#35268#21017#35828#26126
            TabOrder = 6
            object lbl36: TLabel
              Left = 17
              Top = 19
              Width = 480
              Height = 12
              Caption = #24403#29190#20986#29289#21697#36798#21040#26368#22823#20801#35768#25968#37327#30340#20840#23616#25511#21046#21518#19981#20877#29190#20986#65281#27969#31243#65306#24618#29289#29190#20986'-'#20840#23616#26816#27979'-'#22320#22270#26816#27979
            end
            object lbl37: TLabel
              Left = 17
              Top = 37
              Width = 504
              Height = 12
              Caption = #26410#36798#21040#26368#22823#25968#37327#20840#23616#65292#21017#20174#35774#32622#26368#22823#25968#37327#21040#26368#23567#36827#34892#26816#27979#65292#26410#35302#21457#20840#23616#38480#21046#21017#36827#20837#21333#20010#22320#22270#25511#21046
            end
            object lbl38: TLabel
              Left = 17
              Top = 55
              Width = 546
              Height = 12
              Caption = #19978#38754#30340#38388#38548'('#20998')'#25351#29289#21697#29190#20986#38388#38548#65307#20351#29992#27492#21151#33021#35831#28165#31354#21015#34920#20449#24687'1'#20869#30340#25481#33853#25511#21046#65292#20197#20813#20914#31361#23548#33268#35745#25968#38169#35823#65281
              Font.Charset = GB2312_CHARSET
              Font.Color = clBlue
              Font.Height = -12
              Font.Name = #23435#20307
              Font.Style = []
              ParentFont = False
            end
          end
        end
      end
    end
  end
  object PopupMenuItem: TPopupMenu
    Left = 441
    Top = 328
    object MenuItem_ShowAll: TMenuItem
      Tag = -1
      Caption = #26174#31034#20840#37096#29289#21697
      Checked = True
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowDress: TMenuItem
      Caption = #21482#26174#31034#34915#26381
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowWEAPON: TMenuItem
      Tag = 1
      Caption = #21482#26174#31034#27494#22120
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowRIGHTHAND: TMenuItem
      Tag = 2
      Caption = #21482#26174#31034#29031#26126#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowNECKLACE: TMenuItem
      Tag = 3
      Caption = #21482#26174#31034#39033#38142
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowHELMET: TMenuItem
      Tag = 4
      Caption = #21482#26174#31034#22836#30420
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowARMRING: TMenuItem
      Tag = 6
      Caption = #21482#26174#31034#25163#38255
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowRING: TMenuItem
      Tag = 8
      Caption = #21482#26174#31034#25106#25351
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowBOOTS: TMenuItem
      Tag = 11
      Caption = #21482#26174#31034#38795#23376
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowBUJUK: TMenuItem
      Tag = 9
      Caption = #21482#26174#31034#31526
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_BELT: TMenuItem
      Tag = 10
      Caption = #21482#26174#31034#33136#24102
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_CHARM: TMenuItem
      Tag = 12
      Caption = #21482#26174#31034#23453#30707
      OnClick = MenuItem_ShowAllClick
    end
    object N1: TMenuItem
      Tag = 13
      Caption = #21482#26174#31034#26007#31520
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowDrugIrem: TMenuItem
      Tag = 100
      Caption = #21482#26174#31034#33647#29289
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowBindIrem: TMenuItem
      Tag = 31
      Caption = #21482#26174#31034#25171#21253#29289#21697
      OnClick = MenuItem_ShowAllClick
    end
    object mniN2: TMenuItem
      Tag = 14
      Caption = #21482#26174#31034#20891#40723
      OnClick = MenuItem_ShowAllClick
    end
    object N2: TMenuItem
      Tag = 16
      Caption = #21482#26174#31034#30462#29260
      OnClick = MenuItem_ShowAllClick
    end
    object MenuItem_ShowOTHERITEM: TMenuItem
      Tag = 111
      Caption = #26174#31034#20854#20182
      OnClick = MenuItem_ShowAllClick
    end
  end
  object tmrFlash: TTimer
    Enabled = False
    Interval = 300
    OnTimer = tmrFlashTimer
    Left = 377
    Top = 360
  end
  object dlgSaveMonsters: TSaveDialog
    Filter = #24618#29289#37197#32622#25991#20214'(*.dat)|*.dat'
    Title = #29983#25104#30331#24405#22120#37197#32622#25991#20214
    Left = 473
    Top = 328
  end
  object dlgSaveNpcs: TSaveDialog
    Filter = 'NPC'#37197#32622#25991#20214'(*.dat)|*.dat'
    Title = #29983#25104#30331#24405#22120#37197#32622#25991#20214
    Left = 505
    Top = 328
  end
  object ilCheck: TImageList
    Height = 13
    Width = 13
    Left = 377
    Top = 328
    Bitmap = {
      494C010108000A0004000D000D00FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      000000000000360000002800000034000000270000000100200000000000B01F
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000009797CE00676AB8000000000000000000000000000000
      00000000000000000000000000003E3FA4007A7DC10000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000004045600040456000404560004045600040456000404
      560004045600040456000404560004045600000000009899CE000C0F8F000C0F
      8F007376BD0000000000000000000000000000000000000000002D2F9D000C0F
      8F003335A0000000000000000000030393000303930003039300030393000303
      9300030393000303930003039300030393000303930000000000000000000000
      0000040456000404560004045600040456000404560004045600040456000404
      5600040456000404560000000000000000000000000000000000040456007676
      A2007676A2007676A2008282AA007D7DA7007C7CA6007D7DA7006A6A9B000404
      5600000000008485C5000C0F8F000C0F8F000C0F8F008A8BC700000000000000
      000000000000222698000C0F8F00252899000000000000000000000000000707
      9B007777C7007777C7007777C7008383CC007F7FCA007E7EC9007F7FCA006D6D
      C30007079B00000000000000000000000000040456007676A2007676A2007676
      A2008282AA007D7DA7007C7CA6007D7DA7006A6A9B0004045600000000000000
      00000000000000000000040456001E1E67001E1E67001E1E670022226A002222
      6A0020206700202067001A1A64000404560000000000000000009295CC000C0F
      8F000C0F8F000C0F8F009D9DD000DBDBED001A1D94000C0F8F001D1F95000000
      00000000000000000000000000000C0CA6002525AE002525AE002525AE002929
      AF002929AF002626AE002626AE002121AC000C0CA60000000000000000000000
      0000040456001E1E67001E1E67001E1E670022226A0022226A00202067002020
      67001A1A64000404560000000000000000000000000000000000040456007676
      A2007676A2007676A2008282AA007D7DA7007C7CA6007D7DA7006A6A9B000404
      5600000000000000000000000000A6A7D5000C0F8E000C0F8F000B0E8D001316
      91000C0F8F0014189200DEDEEF00000000000000000000000000000000001111
      B2007D7DD3007D7DD3007D7DD3008888D7008484D5008282D5008484D5007272
      D0001111B200000000000000000000000000040456007676A2007676A2007676
      A2008282AA007D7DA7007C7CA6007D7DA7006A6A9B0004045600000000000000
      00000000000000000000040456001E1E67001E1E67001E1E670022226A002222
      6A0020206700202067001A1A6400040456000000000000000000000000000000
      0000BABADE000E118F000C0F8F000C0F8F0011149000D2D3E900000000000000
      00000000000000000000000000001717BF002E2EC4002E2EC4002E2EC4003232
      C5003232C5003030C4003030C4002B2BC3001717BF0000000000000000000000
      0000040456001E1E67001E1E67001E1E670022226A0022226A00202067002020
      67001A1A64000404560000000000000000000000000000000000040456008787
      AD008787AD008787AD009898B8009898B8009797B7009090B4007575A1000404
      560000000000000000000000000000000000B3B3DA000D108E000C0F8F000C0F
      8F000F128F00D0D1E80000000000000000000000000000000000000000001C1C
      CC009393E5009393E5009393E500A0A0E800A0A0E800A0A0E8009999E7008282
      E1001C1CCC00000000000000000000000000040456008787AD008787AD008787
      AD009898B8009898B8009797B7009090B4007575A10004045600000000000000
      00000000000000000000040456000C0C5B000C0C5B000C0C5B000D0D5B000D0D
      5B000D0D5B000D0D5B000B0B5A0004045600000000000000000000000000A0A2
      D2000B0E8D000C0F8F000B0E8D00141892000C0F8F0013169100DBDBED000000
      00000000000000000000000000002222D9002929D7002929D7002929D7002A2A
      D8002A2AD8002A2AD8002A2AD8002828D8002222D90000000000000000000000
      0000040456000C0C5B000C0C5B000C0C5B000D0D5B000D0D5B000D0D5B000D0D
      5B000B0B5A000404560000000000000000000000000000000000040456009393
      B6009393B6009393B600A3A3C000A3A3C000A3A3C000A3A3C0008484AB000404
      560000000000000000008D8DC9000C0F8F000C0F8F000C0F8F00A4A6D400E0E0
      EF001E2096000C0F8F001A1D9400000000000000000000000000000000002727
      E500A2A2F200A2A2F200A2A2F200B0B0F400B0B0F400B0B0F400B0B0F4009595
      F0002727E500000000000000000000000000040456009393B6009393B6009393
      B600A3A3C000A3A3C000A3A3C000A3A3C0008484AB0004045600000000000000
      0000000000000000000004045600040456000404560004045600040456000404
      560004045600040456000404560004045600000000007D7FC2000C0F8F000C0F
      8F000C0F8F009194CB00000000000000000000000000272A9A000C0F8F002226
      98000000000000000000000000002C2CF0002C2CF0002C2CF0002C2CF0002C2C
      F0002C2CF0002C2CF0002C2CF0002C2CF0002C2CF00000000000000000000000
      0000040456000404560004045600040456000404560004045600040456000404
      5600040456000404560000000000000000000000000000000000040456008A8A
      AF008A8AAF008A8AAF00A2A2BF00A3A3C0003737770000000000000000000000
      000000000000A0A2D2000C0F8F000C0F8F007C7EC20000000000000000000000
      0000000000000000000032369F000C0F8F0030329E0000000000000000003030
      F8009E9EFA009E9EFA009E9EFA00B2B2FA00B2B2FA005959F600000000000000
      000000000000000000000000000000000000040456008A8AAF008A8AAF008A8A
      AF00A2A2BF00A3A3C00037377700000000000000000000000000000000000000
      0000000000000000000004045600040456000404560004045600040456000404
      5600040456000000000000000000000000000000000000000000A0A0D2007174
      BC00000000000000000000000000000000000000000000000000000000004648
      A8008385C40000000000000000003333FF003333FF003333FF003333FF003333
      FF003333FF003333FF0000000000000000000000000000000000000000000000
      0000040456000404560004045600040456000404560004045600040456000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008F8F8E00F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F4008F8F8E008F8F8E00F4F4F400F4F4
      F400F4F4F400F5F5F500F9F9F900F8F8F800F5F5F500F4F4F400F4F4F400F4F4
      F400F4F4F4008F8F8E00000000009797D700676ACA0000000000000000000000
      0000000000000000000000000000000000003F40C1007D7ECF00000000000000
      00009797CE00676AB80000000000000000000000000000000000000000000000
      0000000000003E3FA4007A7DC100000000008F8F8E00F4F4F400CCCBCA00D5D4
      D400DCDBDB00E1E1E000E7E7E600EBEBEA00ECECEB00ECEBEB00EAE9E900F4F4
      F4008F8F8E008F8F8E00F4F4F400CCCBCA00DBDADA00E9E2DF00BA998C00BD9D
      9000F6F3F200EDEDEC00ECEBEB00EAE9E900F4F4F4008F8F8E009999D8001516
      D3001617D9007476CC0000000000000000000000000000000000000000003031
      BC001718DF003538BF00000000009899CE000C0F8F000C0F8F007376BD000000
      0000000000000000000000000000000000002D2F9D000C0F8F003335A0000000
      00008F8F8E00F4F4F400C6C4C200E9E9E900EDEDED00F0F0F000F4F4F400F6F6
      F600F6F6F600F6F6F600E6E6E600F4F4F4008F8F8E008F8F8E00F4F4F400CAC8
      C600F0ECEA00BB998B00975F4A0098614C00D1B9B000F9F9F900F6F6F600E6E6
      E600F4F4F4008F8F8E008787D2001A1DD6001D20E1001B1ED8008C8DD4000000
      000000000000000000002A2CC0001D20E1002C2EBF0000000000000000008485
      C5000C0F8F000C0F8F000C0F8F008A8BC7000000000000000000000000002226
      98000C0F8F002528990000000000000000008F8F8E00F4F4F400C2BFBC00E5E4
      E300E9E9E900EDEDED00F2F2F200F4F4F400F5F5F500F4F4F400E2E2E100F4F4
      F4008F8F8E008F8F8E00F4F4F400D1CFCD00E9E1DE00955D4800965F49009760
      4B00A4736100FAF9F800F4F4F400E2E2E100F4F4F4008F8F8E00000000009595
      D6001F22D3002427E2002023D5009F9FD900DBDBEF002426C2002427E200272A
      C300000000000000000000000000000000009295CC000C0F8F000C0F8F000C0F
      8F009D9DD000DBDBED001A1D94000C0F8F001D1F950000000000000000000000
      00008F8F8E00F4F4F400BFBBB800E1DFDD00E5E5E400EAEAEA00EFEFEF00F2F2
      F200F2F2F200F2F2F200DEDDDC00F4F4F4008F8F8E008F8F8E00F4F4F400E1E0
      DE00AA7F6E00945C4700E2D4CF00A778670097604B00D5BFB700F6F6F600DEDD
      DC00F4F4F4008F8F8E000000000000000000A9AADD002326CD002A2EE3002326
      CF002528C4002A2EE3002629C300DEDEF0000000000000000000000000000000
      000000000000A6A7D5000C0F8E000C0F8F000B0E8D00131691000C0F8F001418
      9200DEDEEF000000000000000000000000008F8F8E00F4F4F400BCB7B200DCD8
      D500DFDCDA00E3E1E000E8E8E800ECECEC00EDEDED00EDEDED00D6D5D400F4F4
      F4008F8F8E008F8F8E00F4F4F400CDC9C500DDCFC900C8AEA300EEEEED00D5C1
      BA00965E4900A5766400F8F8F800D6D5D500F4F4F4008F8F8E00000000000000
      000000000000BABDE300262ACB003136E4003136E400262CC800D2D2EC000000
      0000000000000000000000000000000000000000000000000000BABADE000E11
      8F000C0F8F000C0F8F0011149000D2D3E9000000000000000000000000000000
      00008F8F8E00F4F4F400B9B3AE00D7D1CD00D9D4D000DBD7D400DFDDDB00E3E2
      E100E6E6E500E8E8E800CDCDCC00F4F4F4008F8F8E008F8F8E00F4F4F400B9B3
      AE00DDD9D500E5E2DF00DCD8D500F4F3F200A1715E00945C4700D6C3BC00DCDC
      DB00F4F4F4008F8F8E00000000000000000000000000B4B4E0002C2FCD00373D
      E500373DE5002A2DC900D1D1EB00000000000000000000000000000000000000
      00000000000000000000B3B3DA000D108E000C0F8F000C0F8F000F128F00D0D1
      E800000000000000000000000000000000008F8F8E00F4F4F400B9B3AE00D5CF
      CB00D5CFCB00D6D1CD00DAD5D200DEDBD800E1DFDD00E4E3E200C8C7C600F4F4
      F4008F8F8E008F8F8E00F4F4F400B9B3AE00D5CFCB00D5CFCB00D6D1CD00E6E2
      E000CFB8AF00925A4500A5776500E8E7E700F4F4F4008F8F8E00000000000000
      0000A4A4DB003237D1003E45E6003137CF003033C4003E45E6003134C700DBDB
      EF000000000000000000000000000000000000000000A0A2D2000B0E8D000C0F
      8F000B0E8D00141892000C0F8F0013169100DBDBED0000000000000000000000
      00008F8F8E00F4F4F400B9B3AE00D5CFCB00D5CFCB00D5CFCB00D5CFCB00D8D3
      D000DCD8D500DFDDDB00C5C3C100F4F4F4008F8F8E008F8F8E00F4F4F400B9B3
      AE00D5CFCB00D5CFCB00D5CFCB00D6D0CC00F1EEED009D6A5700925A4400D0BF
      B900F6F6F6008F8F8E00000000009294D5003A41D800444CE7003A40D800A8AA
      DD00E0E0F100383BC400444CE700353BC5000000000000000000000000000000
      00008D8DC9000C0F8F000C0F8F000C0F8F00A4A6D400E0E0EF001E2096000C0F
      8F001A1D94000000000000000000000000008F8F8E00F4F4F400B9B3AE00B9B3
      AE00B9B3AE00B9B3AE00B9B3AE00B9B3AE00BAB4AF00BDB9B400C1BEBB00F4F4
      F4008F8F8E008F8F8E00F4F4F400B9B3AE00B9B3AE00B9B3AE00B9B3AE00B9B3
      AE00D0CCC900C0A79D00AB867700E4DFDC00F5F5F5008F8F8E008687D100424A
      DD004A53E8004149DD009898D7000000000000000000000000003F43C2004A53
      E8003E40C30000000000000000007D7FC2000C0F8F000C0F8F000C0F8F009194
      CB00000000000000000000000000272A9A000C0F8F0022269800000000000000
      00008F8F8E00F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F4008F8F8E008F8F8E00F4F4F400F4F4
      F400F4F4F400F4F4F400F4F4F400F4F4F400F4F4F400F8F8F800F9F9F900F6F6
      F600F4F4F4008F8F8E00A4A7DB00434CD9004A53E1008586D100000000000000
      00000000000000000000000000004A4EC3004F59E700474AC20000000000A0A2
      D2000C0F8F000C0F8F007C7EC200000000000000000000000000000000000000
      000032369F000C0F8F0030329E00000000008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F
      8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E008F8F8E0000000000A5A5
      DB007C7FCE000000000000000000000000000000000000000000000000000000
      0000595CC5008C8DD3000000000000000000A0A0D2007174BC00000000000000
      000000000000000000000000000000000000000000004648A8008385C4000000
      0000424D3E000000000000003E00000028000000340000002700000001000100
      00000000380100000000000000000000000000000000000000000000FFFFFF00
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000CFE7FFFFFFC0000087C6007003C00000838E007003C00000
      C01E007003C00000E01E007003C00000F03E007003C00000F03E007003C00000
      E01E007003C00000C01E007003C00000838E007003C0700087C603F01FC07000
      CFE603F01FFFF000FFFFFFFFFFFFF0000000003FFFFFF00000000027F33F9000
      00000003E21F100000000001C60E3000000000200F007000000000300F807000
      000000381FC0F000000000381FC0F000000000300F807000000000200F007000
      00000001C60E300000000003E21F100000000027F33F90000000000000000000
      0000000000000000000000000000}
  end
  object pmItemRules: TPopupMenu
    OnPopup = pmItemRulesPopup
    Left = 409
    Top = 328
    object miItemRulesLog: TMenuItem
      Caption = #26597#30475#26085#24535
    end
  end
end
